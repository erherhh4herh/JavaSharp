using System;
using System.Threading;

/*
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea, Bill Scherer, and Michael Scott with
 * assistance from members of JCP JSR-166 Expert Group and released to
 * the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	/// <summary>
	/// A synchronization point at which threads can pair and swap elements
	/// within pairs.  Each thread presents some object on entry to the
	/// <seealso cref="#exchange exchange"/> method, matches with a partner thread,
	/// and receives its partner's object on return.  An Exchanger may be
	/// viewed as a bidirectional form of a <seealso cref="SynchronousQueue"/>.
	/// Exchangers may be useful in applications such as genetic algorithms
	/// and pipeline designs.
	/// 
	/// <para><b>Sample Usage:</b>
	/// Here are the highlights of a class that uses an {@code Exchanger}
	/// to swap buffers between threads so that the thread filling the
	/// buffer gets a freshly emptied one when it needs it, handing off the
	/// filled one to the thread emptying the buffer.
	///  <pre> {@code
	/// class FillAndEmpty {
	///   Exchanger<DataBuffer> exchanger = new Exchanger<DataBuffer>();
	///   DataBuffer initialEmptyBuffer = ... a made-up type
	///   DataBuffer initialFullBuffer = ...
	/// 
	///   class FillingLoop implements Runnable {
	///     public void run() {
	///       DataBuffer currentBuffer = initialEmptyBuffer;
	///       try {
	///         while (currentBuffer != null) {
	///           addToBuffer(currentBuffer);
	///           if (currentBuffer.isFull())
	///             currentBuffer = exchanger.exchange(currentBuffer);
	///         }
	///       } catch (InterruptedException ex) { ... handle ... }
	///     }
	///   }
	/// 
	///   class EmptyingLoop implements Runnable {
	///     public void run() {
	///       DataBuffer currentBuffer = initialFullBuffer;
	///       try {
	///         while (currentBuffer != null) {
	///           takeFromBuffer(currentBuffer);
	///           if (currentBuffer.isEmpty())
	///             currentBuffer = exchanger.exchange(currentBuffer);
	///         }
	///       } catch (InterruptedException ex) { ... handle ...}
	///     }
	///   }
	/// 
	///   void start() {
	///     new Thread(new FillingLoop()).start();
	///     new Thread(new EmptyingLoop()).start();
	///   }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>Memory consistency effects: For each pair of threads that
	/// successfully exchange objects via an {@code Exchanger}, actions
	/// prior to the {@code exchange()} in each thread
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// those subsequent to a return from the corresponding {@code exchange()}
	/// in the other thread.
	/// 
	/// @since 1.5
	/// @author Doug Lea and Bill Scherer and Michael Scott
	/// </para>
	/// </summary>
	/// @param <V> The type of objects that may be exchanged </param>
	public class Exchanger<V>
	{

		/*
		 * Overview: The core algorithm is, for an exchange "slot",
		 * and a participant (caller) with an item:
		 *
		 * for (;;) {
		 *   if (slot is empty) {                       // offer
		 *     place item in a Node;
		 *     if (can CAS slot from empty to node) {
		 *       wait for release;
		 *       return matching item in node;
		 *     }
		 *   }
		 *   else if (can CAS slot from node to empty) { // release
		 *     get the item in node;
		 *     set matching item in node;
		 *     release waiting thread;
		 *   }
		 *   // else retry on CAS failure
		 * }
		 *
		 * This is among the simplest forms of a "dual data structure" --
		 * see Scott and Scherer's DISC 04 paper and
		 * http://www.cs.rochester.edu/research/synchronization/pseudocode/duals.html
		 *
		 * This works great in principle. But in practice, like many
		 * algorithms centered on atomic updates to a single location, it
		 * scales horribly when there are more than a few participants
		 * using the same Exchanger. So the implementation instead uses a
		 * form of elimination arena, that spreads out this contention by
		 * arranging that some threads typically use different slots,
		 * while still ensuring that eventually, any two parties will be
		 * able to exchange items. That is, we cannot completely partition
		 * across threads, but instead give threads arena indices that
		 * will on average grow under contention and shrink under lack of
		 * contention. We approach this by defining the Nodes that we need
		 * anyway as ThreadLocals, and include in them per-thread index
		 * and related bookkeeping state. (We can safely reuse per-thread
		 * nodes rather than creating them fresh each time because slots
		 * alternate between pointing to a node vs null, so cannot
		 * encounter ABA problems. However, we do need some care in
		 * resetting them between uses.)
		 *
		 * Implementing an effective arena requires allocating a bunch of
		 * space, so we only do so upon detecting contention (except on
		 * uniprocessors, where they wouldn't help, so aren't used).
		 * Otherwise, exchanges use the single-slot slotExchange method.
		 * On contention, not only must the slots be in different
		 * locations, but the locations must not encounter memory
		 * contention due to being on the same cache line (or more
		 * generally, the same coherence unit).  Because, as of this
		 * writing, there is no way to determine cacheline size, we define
		 * a value that is enough for common platforms.  Additionally,
		 * extra care elsewhere is taken to avoid other false/unintended
		 * sharing and to enhance locality, including adding padding (via
		 * sun.misc.Contended) to Nodes, embedding "bound" as an Exchanger
		 * field, and reworking some park/unpark mechanics compared to
		 * LockSupport versions.
		 *
		 * The arena starts out with only one used slot. We expand the
		 * effective arena size by tracking collisions; i.e., failed CASes
		 * while trying to exchange. By nature of the above algorithm, the
		 * only kinds of collision that reliably indicate contention are
		 * when two attempted releases collide -- one of two attempted
		 * offers can legitimately fail to CAS without indicating
		 * contention by more than one other thread. (Note: it is possible
		 * but not worthwhile to more precisely detect contention by
		 * reading slot values after CAS failures.)  When a thread has
		 * collided at each slot within the current arena bound, it tries
		 * to expand the arena size by one. We track collisions within
		 * bounds by using a version (sequence) number on the "bound"
		 * field, and conservatively reset collision counts when a
		 * participant notices that bound has been updated (in either
		 * direction).
		 *
		 * The effective arena size is reduced (when there is more than
		 * one slot) by giving up on waiting after a while and trying to
		 * decrement the arena size on expiration. The value of "a while"
		 * is an empirical matter.  We implement by piggybacking on the
		 * use of spin->yield->block that is essential for reasonable
		 * waiting performance anyway -- in a busy exchanger, offers are
		 * usually almost immediately released, in which case context
		 * switching on multiprocessors is extremely slow/wasteful.  Arena
		 * waits just omit the blocking part, and instead cancel. The spin
		 * count is empirically chosen to be a value that avoids blocking
		 * 99% of the time under maximum sustained exchange rates on a
		 * range of test machines. Spins and yields entail some limited
		 * randomness (using a cheap xorshift) to avoid regular patterns
		 * that can induce unproductive grow/shrink cycles. (Using a
		 * pseudorandom also helps regularize spin cycle duration by
		 * making branches unpredictable.)  Also, during an offer, a
		 * waiter can "know" that it will be released when its slot has
		 * changed, but cannot yet proceed until match is set.  In the
		 * mean time it cannot cancel the offer, so instead spins/yields.
		 * Note: It is possible to avoid this secondary check by changing
		 * the linearization point to be a CAS of the match field (as done
		 * in one case in the Scott & Scherer DISC paper), which also
		 * increases asynchrony a bit, at the expense of poorer collision
		 * detection and inability to always reuse per-thread nodes. So
		 * the current scheme is typically a better tradeoff.
		 *
		 * On collisions, indices traverse the arena cyclically in reverse
		 * order, restarting at the maximum index (which will tend to be
		 * sparsest) when bounds change. (On expirations, indices instead
		 * are halved until reaching 0.) It is possible (and has been
		 * tried) to use randomized, prime-value-stepped, or double-hash
		 * style traversal instead of simple cyclic traversal to reduce
		 * bunching.  But empirically, whatever benefits these may have
		 * don't overcome their added overhead: We are managing operations
		 * that occur very quickly unless there is sustained contention,
		 * so simpler/faster control policies work better than more
		 * accurate but slower ones.
		 *
		 * Because we use expiration for arena size control, we cannot
		 * throw TimeoutExceptions in the timed version of the public
		 * exchange method until the arena size has shrunken to zero (or
		 * the arena isn't enabled). This may delay response to timeout
		 * but is still within spec.
		 *
		 * Essentially all of the implementation is in methods
		 * slotExchange and arenaExchange. These have similar overall
		 * structure, but differ in too many details to combine. The
		 * slotExchange method uses the single Exchanger field "slot"
		 * rather than arena array elements. However, it still needs
		 * minimal collision detection to trigger arena construction.
		 * (The messiest part is making sure interrupt status and
		 * InterruptedExceptions come out right during transitions when
		 * both methods may be called. This is done by using null return
		 * as a sentinel to recheck interrupt status.)
		 *
		 * As is too common in this sort of code, methods are monolithic
		 * because most of the logic relies on reads of fields that are
		 * maintained as local variables so can't be nicely factored --
		 * mainly, here, bulky spin->yield->block/cancel code), and
		 * heavily dependent on intrinsics (Unsafe) to use inlined
		 * embedded CAS and related memory access operations (that tend
		 * not to be as readily inlined by dynamic compilers when they are
		 * hidden behind other methods that would more nicely name and
		 * encapsulate the intended effects). This includes the use of
		 * putOrderedX to clear fields of the per-thread Nodes between
		 * uses. Note that field Node.item is not declared as volatile
		 * even though it is read by releasing threads, because they only
		 * do so after CAS operations that must precede access, and all
		 * uses by the owning thread are otherwise acceptably ordered by
		 * other operations. (Because the actual points of atomicity are
		 * slot CASes, it would also be legal for the write to Node.match
		 * in a release to be weaker than a full volatile write. However,
		 * this is not done because it could allow further postponement of
		 * the write, delaying progress.)
		 */

		/// <summary>
		/// The byte distance (as a shift value) between any two used slots
		/// in the arena.  1 << ASHIFT should be at least cacheline size.
		/// </summary>
		private const int ASHIFT = 7;

		/// <summary>
		/// The maximum supported arena index. The maximum allocatable
		/// arena size is MMASK + 1. Must be a power of two minus one, less
		/// than (1<<(31-ASHIFT)). The cap of 255 (0xff) more than suffices
		/// for the expected scaling limits of the main algorithms.
		/// </summary>
		private const int MMASK = 0xff;

		/// <summary>
		/// Unit for sequence/version bits of bound field. Each successful
		/// change to the bound also adds SEQ.
		/// </summary>
		private static readonly int SEQ = MMASK + 1;

		/// <summary>
		/// The number of CPUs, for sizing and spin control </summary>
		private static readonly int NCPU = Runtime.Runtime.availableProcessors();

		/// <summary>
		/// The maximum slot index of the arena: The number of slots that
		/// can in principle hold all threads without contention, or at
		/// most the maximum indexable value.
		/// </summary>
		internal static readonly int FULL = (NCPU >= (MMASK << 1)) ? MMASK : NCPU >>> 1;

		/// <summary>
		/// The bound for spins while waiting for a match. The actual
		/// number of iterations will on average be about twice this value
		/// due to randomization. Note: Spinning is disabled when NCPU==1.
		/// </summary>
		private static readonly int SPINS = 1 << 10;

		/// <summary>
		/// Value representing null arguments/returns from public
		/// methods. Needed because the API originally didn't disallow null
		/// arguments, which it should have.
		/// </summary>
		private static readonly Object NULL_ITEM = new Object();

		/// <summary>
		/// Sentinel value returned by internal exchange methods upon
		/// timeout, to avoid need for separate timed versions of these
		/// methods.
		/// </summary>
		private static readonly Object TIMED_OUT = new Object();

		/// <summary>
		/// Nodes hold partially exchanged data, plus other per-thread
		/// bookkeeping. Padded via @sun.misc.Contended to reduce memory
		/// contention.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended static final class Node
		internal sealed class Node
		{
			internal int Index; // Arena index
			internal int Bound; // Last recorded value of Exchanger.bound
			internal int Collides; // Number of CAS failures at current bound
			internal int Hash; // Pseudo-random for spins
			internal Object Item; // This thread's current item
			internal volatile Object Match; // Item provided by releasing thread
			internal volatile Thread Parked; // Set to this thread when parked, else null
		}

		/// <summary>
		/// The corresponding thread local class </summary>
		internal sealed class Participant : ThreadLocal<Node>
		{
			public Node InitialValue()
			{
				return new Node();
			}
		}

		/// <summary>
		/// Per-thread state
		/// </summary>
		private readonly Participant Participant;

		/// <summary>
		/// Elimination array; null until enabled (within slotExchange).
		/// Element accesses use emulation of volatile gets and CAS.
		/// </summary>
		private volatile Node[] Arena;

		/// <summary>
		/// Slot used until contention detected.
		/// </summary>
		private volatile Node Slot;

		/// <summary>
		/// The index of the largest valid arena position, OR'ed with SEQ
		/// number in high bits, incremented on each update.  The initial
		/// update from 0 to SEQ is used to ensure that the arena array is
		/// constructed only once.
		/// </summary>
		private volatile int Bound;

		/// <summary>
		/// Exchange function when arenas enabled. See above for explanation.
		/// </summary>
		/// <param name="item"> the (non-null) item to exchange </param>
		/// <param name="timed"> true if the wait is timed </param>
		/// <param name="ns"> if timed, the maximum wait time, else 0L </param>
		/// <returns> the other thread's item; or null if interrupted; or
		/// TIMED_OUT if timed and timed out </returns>
		private Object ArenaExchange(Object item, bool timed, long ns)
		{
			Node[] a = Arena;
			Node p = Participant.Get();
			for (int i = p.Index;;) // access slot at i
			{
				int b, m, c; // j is raw array offset
				long j;
				Node q = (Node)U.getObjectVolatile(a, j = (i << ASHIFT) + ABASE);
				if (q != null && U.compareAndSwapObject(a, j, q, null))
				{
					Object v = q.Item; // release
					q.Match = item;
					Thread w = q.Parked;
					if (w != null)
					{
						U.unpark(w);
					}
					return v;
				}
				else if (i <= (m = (b = Bound) & MMASK) && q == null)
				{
					p.Item = item; // offer
					if (U.compareAndSwapObject(a, j, null, p))
					{
						long end = (timed && m == 0) ? System.nanoTime() + ns : 0L;
						Thread t = Thread.CurrentThread; // wait
						for (int h = p.Hash, spins = SPINS;;)
						{
							Object v = p.Match;
							if (v != null)
							{
								U.putOrderedObject(p, MATCH, null);
								p.Item = null; // clear for next use
								p.Hash = h;
								return v;
							}
							else if (spins > 0)
							{
								h ^= h << 1; // xorshift
								h ^= (int)((uint)h >> 3);
								h ^= h << 10;
								if (h == 0) // initialize hash
								{
									h = SPINS | (int)t.Id;
								}
								else if (h < 0 && (--spins & (((int)((uint)SPINS >> 1)) - 1)) == 0) // approx 50% true
								{
									Thread.@yield(); // two yields per wait
								}
							}
							else if (U.getObjectVolatile(a, j) != p)
							{
								spins = SPINS; // releaser hasn't set match yet
							}
							else if (!t.Interrupted && m == 0 && (!timed || (ns = end - System.nanoTime()) > 0L))
							{
								U.putObject(t, BLOCKER, this); // emulate LockSupport
								p.Parked = t; // minimize window
								if (U.getObjectVolatile(a, j) == p)
								{
									U.park(false, ns);
								}
								p.Parked = null;
								U.putObject(t, BLOCKER, null);
							}
							else if (U.getObjectVolatile(a, j) == p && U.compareAndSwapObject(a, j, p, null))
							{
								if (m != 0) // try to shrink
								{
									U.compareAndSwapInt(this, BOUND, b, b + SEQ - 1);
								}
								p.Item = null;
								p.Hash = h;
								i = p.Index = (int)((uint)p.Index >> 1); // descend
								if (Thread.Interrupted())
								{
									return null;
								}
								if (timed && m == 0 && ns <= 0L)
								{
									return TIMED_OUT;
								}
								break; // expired; restart
							}
						}
					}
					else
					{
						p.Item = null; // clear offer
					}
				}
				else
				{
					if (p.Bound != b) // stale; reset
					{
						p.Bound = b;
						p.Collides = 0;
						i = (i != m || m == 0) ? m : m - 1;
					}
					else if ((c = p.Collides) < m || m == FULL || !U.compareAndSwapInt(this, BOUND, b, b + SEQ + 1))
					{
						p.Collides = c + 1;
						i = (i == 0) ? m : i - 1; // cyclically traverse
					}
					else
					{
						i = m + 1; // grow
					}
					p.Index = i;
				}
			}
		}

		/// <summary>
		/// Exchange function used until arenas enabled. See above for explanation.
		/// </summary>
		/// <param name="item"> the item to exchange </param>
		/// <param name="timed"> true if the wait is timed </param>
		/// <param name="ns"> if timed, the maximum wait time, else 0L </param>
		/// <returns> the other thread's item; or null if either the arena
		/// was enabled or the thread was interrupted before completion; or
		/// TIMED_OUT if timed and timed out </returns>
		private Object SlotExchange(Object item, bool timed, long ns)
		{
			Node p = Participant.Get();
			Thread t = Thread.CurrentThread;
			if (t.Interrupted) // preserve interrupt status so caller can recheck
			{
				return null;
			}

			for (Node q;;)
			{
				if ((q = Slot) != null)
				{
					if (U.compareAndSwapObject(this, SLOT, q, null))
					{
						Object v = q.item;
						q.match = item;
						Thread w = q.parked;
						if (w != null)
						{
							U.unpark(w);
						}
						return v;
					}
					// create arena on contention, but continue until slot null
					if (NCPU > 1 && Bound == 0 && U.compareAndSwapInt(this, BOUND, 0, SEQ))
					{
						Arena = new Node[(FULL + 2) << ASHIFT];
					}
				}
				else if (Arena != null)
				{
					return null; // caller must reroute to arenaExchange
				}
				else
				{
					p.Item = item;
					if (U.compareAndSwapObject(this, SLOT, null, p))
					{
						break;
					}
					p.Item = null;
				}
			}

			// await release
			int h = p.Hash;
			long end = timed ? System.nanoTime() + ns : 0L;
			int spins = (NCPU > 1) ? SPINS : 1;
			Object v;
			while ((v = p.Match) == null)
			{
				if (spins > 0)
				{
					h ^= h << 1;
					h ^= (int)((uint)h >> 3);
					h ^= h << 10;
					if (h == 0)
					{
						h = SPINS | (int)t.Id;
					}
					else if (h < 0 && (--spins & (((int)((uint)SPINS >> 1)) - 1)) == 0)
					{
						Thread.@yield();
					}
				}
				else if (Slot != p)
				{
					spins = SPINS;
				}
				else if (!t.Interrupted && Arena == null && (!timed || (ns = end - System.nanoTime()) > 0L))
				{
					U.putObject(t, BLOCKER, this);
					p.Parked = t;
					if (Slot == p)
					{
						U.park(false, ns);
					}
					p.Parked = null;
					U.putObject(t, BLOCKER, null);
				}
				else if (U.compareAndSwapObject(this, SLOT, p, null))
				{
					v = timed && ns <= 0L && !t.Interrupted ? TIMED_OUT : null;
					break;
				}
			}
			U.putOrderedObject(p, MATCH, null);
			p.Item = null;
			p.Hash = h;
			return v;
		}

		/// <summary>
		/// Creates a new Exchanger.
		/// </summary>
		public Exchanger()
		{
			Participant = new Participant();
		}

		/// <summary>
		/// Waits for another thread to arrive at this exchange point (unless
		/// the current thread is <seealso cref="Thread#interrupt interrupted"/>),
		/// and then transfers the given object to it, receiving its object
		/// in return.
		/// 
		/// <para>If another thread is already waiting at the exchange point then
		/// it is resumed for thread scheduling purposes and receives the object
		/// passed in by the current thread.  The current thread returns immediately,
		/// receiving the object passed to the exchange by that other thread.
		/// 
		/// </para>
		/// <para>If no other thread is already waiting at the exchange then the
		/// current thread is disabled for thread scheduling purposes and lies
		/// dormant until one of two things happens:
		/// <ul>
		/// <li>Some other thread enters the exchange; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread.
		/// </ul>
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// for the exchange,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the object to exchange </param>
		/// <returns> the object provided by the other thread </returns>
		/// <exception cref="InterruptedException"> if the current thread was
		///         interrupted while waiting </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V exchange(V x) throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual V Exchange(V x)
		{
			Object v;
			Object item = (x == null) ? NULL_ITEM : x; // translate null args
			if ((Arena != null || (v = SlotExchange(item, false, 0L)) == null) && ((Thread.Interrupted() || (v = ArenaExchange(item, false, 0L)) == null))) // disambiguates null return
			{
				throw new InterruptedException();
			}
			return (v == NULL_ITEM) ? null : (V)v;
		}

		/// <summary>
		/// Waits for another thread to arrive at this exchange point (unless
		/// the current thread is <seealso cref="Thread#interrupt interrupted"/> or
		/// the specified waiting time elapses), and then transfers the given
		/// object to it, receiving its object in return.
		/// 
		/// <para>If another thread is already waiting at the exchange point then
		/// it is resumed for thread scheduling purposes and receives the object
		/// passed in by the current thread.  The current thread returns immediately,
		/// receiving the object passed to the exchange by that other thread.
		/// 
		/// </para>
		/// <para>If no other thread is already waiting at the exchange then the
		/// current thread is disabled for thread scheduling purposes and lies
		/// dormant until one of three things happens:
		/// <ul>
		/// <li>Some other thread enters the exchange; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// <li>The specified waiting time elapses.
		/// </ul>
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// for the exchange,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>If the specified waiting time elapses then {@link
		/// TimeoutException} is thrown.  If the time is less than or equal
		/// to zero, the method will not wait at all.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the object to exchange </param>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the {@code timeout} argument </param>
		/// <returns> the object provided by the other thread </returns>
		/// <exception cref="InterruptedException"> if the current thread was
		///         interrupted while waiting </exception>
		/// <exception cref="TimeoutException"> if the specified waiting time elapses
		///         before another thread enters the exchange </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V exchange(V x, long timeout, TimeUnit unit) throws InterruptedException, TimeoutException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual V Exchange(V x, long timeout, TimeUnit unit)
		{
			Object v;
			Object item = (x == null) ? NULL_ITEM : x;
			long ns = unit.ToNanos(timeout);
			if ((Arena != null || (v = SlotExchange(item, true, ns)) == null) && ((Thread.Interrupted() || (v = ArenaExchange(item, true, ns)) == null)))
			{
				throw new InterruptedException();
			}
			if (v == TIMED_OUT)
			{
				throw new TimeoutException();
			}
			return (v == NULL_ITEM) ? null : (V)v;
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe U;
		private static readonly long BOUND;
		private static readonly long SLOT;
		private static readonly long MATCH;
		private static readonly long BLOCKER;
		private static readonly int ABASE;
		static Exchanger()
		{
			int s;
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class ek = typeof(Exchanger);
				Class nk = typeof(Node);
				Class ak = typeof(Node[]);
				Class tk = typeof(Thread);
				BOUND = U.objectFieldOffset(ek.GetDeclaredField("bound"));
				SLOT = U.objectFieldOffset(ek.GetDeclaredField("slot"));
				MATCH = U.objectFieldOffset(nk.GetDeclaredField("match"));
				BLOCKER = U.objectFieldOffset(tk.GetDeclaredField("parkBlocker"));
				s = U.arrayIndexScale(ak);
				// ABASE absorbs padding in front of element 0
				ABASE = U.arrayBaseOffset(ak) + (1 << ASHIFT);

			}
			catch (Exception e)
			{
				throw new Error(e);
			}
			if ((s & (s - 1)) != 0 || s > (1 << ASHIFT))
			{
				throw new Error("Unsupported array scale");
			}
		}

	}

}