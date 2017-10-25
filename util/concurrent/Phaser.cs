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
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{


	/// <summary>
	/// A reusable synchronization barrier, similar in functionality to
	/// <seealso cref="java.util.concurrent.CyclicBarrier CyclicBarrier"/> and
	/// <seealso cref="java.util.concurrent.CountDownLatch CountDownLatch"/>
	/// but supporting more flexible usage.
	/// 
	/// <para><b>Registration.</b> Unlike the case for other barriers, the
	/// number of parties <em>registered</em> to synchronize on a phaser
	/// may vary over time.  Tasks may be registered at any time (using
	/// methods <seealso cref="#register"/>, <seealso cref="#bulkRegister"/>, or forms of
	/// constructors establishing initial numbers of parties), and
	/// optionally deregistered upon any arrival (using {@link
	/// #arriveAndDeregister}).  As is the case with most basic
	/// synchronization constructs, registration and deregistration affect
	/// only internal counts; they do not establish any further internal
	/// bookkeeping, so tasks cannot query whether they are registered.
	/// (However, you can introduce such bookkeeping by subclassing this
	/// class.)
	/// 
	/// </para>
	/// <para><b>Synchronization.</b> Like a {@code CyclicBarrier}, a {@code
	/// Phaser} may be repeatedly awaited.  Method {@link
	/// #arriveAndAwaitAdvance} has effect analogous to {@link
	/// java.util.concurrent.CyclicBarrier#await CyclicBarrier.await}. Each
	/// generation of a phaser has an associated phase number. The phase
	/// number starts at zero, and advances when all parties arrive at the
	/// phaser, wrapping around to zero after reaching {@code
	/// Integer.MAX_VALUE}. The use of phase numbers enables independent
	/// control of actions upon arrival at a phaser and upon awaiting
	/// others, via two kinds of methods that may be invoked by any
	/// registered party:
	/// 
	/// <ul>
	/// 
	///   <li> <b>Arrival.</b> Methods <seealso cref="#arrive"/> and
	///       <seealso cref="#arriveAndDeregister"/> record arrival.  These methods
	///       do not block, but return an associated <em>arrival phase
	///       number</em>; that is, the phase number of the phaser to which
	///       the arrival applied. When the final party for a given phase
	///       arrives, an optional action is performed and the phase
	///       advances.  These actions are performed by the party
	///       triggering a phase advance, and are arranged by overriding
	///       method <seealso cref="#onAdvance(int, int)"/>, which also controls
	///       termination. Overriding this method is similar to, but more
	///       flexible than, providing a barrier action to a {@code
	///       CyclicBarrier}.
	/// 
	///   <li> <b>Waiting.</b> Method <seealso cref="#awaitAdvance"/> requires an
	///       argument indicating an arrival phase number, and returns when
	///       the phaser advances to (or is already at) a different phase.
	///       Unlike similar constructions using {@code CyclicBarrier},
	///       method {@code awaitAdvance} continues to wait even if the
	///       waiting thread is interrupted. Interruptible and timeout
	///       versions are also available, but exceptions encountered while
	///       tasks wait interruptibly or with timeout do not change the
	///       state of the phaser. If necessary, you can perform any
	///       associated recovery within handlers of those exceptions,
	///       often after invoking {@code forceTermination}.  Phasers may
	///       also be used by tasks executing in a <seealso cref="ForkJoinPool"/>,
	///       which will ensure sufficient parallelism to execute tasks
	///       when others are blocked waiting for a phase to advance.
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para><b>Termination.</b> A phaser may enter a <em>termination</em>
	/// state, that may be checked using method <seealso cref="#isTerminated"/>. Upon
	/// termination, all synchronization methods immediately return without
	/// waiting for advance, as indicated by a negative return value.
	/// Similarly, attempts to register upon termination have no effect.
	/// Termination is triggered when an invocation of {@code onAdvance}
	/// returns {@code true}. The default implementation returns {@code
	/// true} if a deregistration has caused the number of registered
	/// parties to become zero.  As illustrated below, when phasers control
	/// actions with a fixed number of iterations, it is often convenient
	/// to override this method to cause termination when the current phase
	/// number reaches a threshold. Method <seealso cref="#forceTermination"/> is
	/// also available to abruptly release waiting threads and allow them
	/// to terminate.
	/// 
	/// </para>
	/// <para><b>Tiering.</b> Phasers may be <em>tiered</em> (i.e.,
	/// constructed in tree structures) to reduce contention. Phasers with
	/// large numbers of parties that would otherwise experience heavy
	/// synchronization contention costs may instead be set up so that
	/// groups of sub-phasers share a common parent.  This may greatly
	/// increase throughput even though it incurs greater per-operation
	/// overhead.
	/// 
	/// </para>
	/// <para>In a tree of tiered phasers, registration and deregistration of
	/// child phasers with their parent are managed automatically.
	/// Whenever the number of registered parties of a child phaser becomes
	/// non-zero (as established in the <seealso cref="#Phaser(Phaser,int)"/>
	/// constructor, <seealso cref="#register"/>, or <seealso cref="#bulkRegister"/>), the
	/// child phaser is registered with its parent.  Whenever the number of
	/// registered parties becomes zero as the result of an invocation of
	/// <seealso cref="#arriveAndDeregister"/>, the child phaser is deregistered
	/// from its parent.
	/// 
	/// </para>
	/// <para><b>Monitoring.</b> While synchronization methods may be invoked
	/// only by registered parties, the current state of a phaser may be
	/// monitored by any caller.  At any given moment there are {@link
	/// #getRegisteredParties} parties in total, of which {@link
	/// #getArrivedParties} have arrived at the current phase ({@link
	/// #getPhase}).  When the remaining (<seealso cref="#getUnarrivedParties"/>)
	/// parties arrive, the phase advances.  The values returned by these
	/// methods may reflect transient states and so are not in general
	/// useful for synchronization control.  Method <seealso cref="#toString"/>
	/// returns snapshots of these state queries in a form convenient for
	/// informal monitoring.
	/// 
	/// </para>
	/// <para><b>Sample usages:</b>
	/// 
	/// </para>
	/// <para>A {@code Phaser} may be used instead of a {@code CountDownLatch}
	/// to control a one-shot action serving a variable number of parties.
	/// The typical idiom is for the method setting this up to first
	/// register, then start the actions, then deregister, as in:
	/// 
	///  <pre> {@code
	/// void runTasks(List<Runnable> tasks) {
	///   final Phaser phaser = new Phaser(1); // "1" to register self
	///   // create and start threads
	///   for (final Runnable task : tasks) {
	///     phaser.register();
	///     new Thread() {
	///       public void run() {
	///         phaser.arriveAndAwaitAdvance(); // await all creation
	///         task.run();
	///       }
	///     }.start();
	///   }
	/// 
	///   // allow threads to start and deregister self
	///   phaser.arriveAndDeregister();
	/// }}</pre>
	/// 
	/// </para>
	/// <para>One way to cause a set of threads to repeatedly perform actions
	/// for a given number of iterations is to override {@code onAdvance}:
	/// 
	///  <pre> {@code
	/// void startTasks(List<Runnable> tasks, final int iterations) {
	///   final Phaser phaser = new Phaser() {
	///     protected boolean onAdvance(int phase, int registeredParties) {
	///       return phase >= iterations || registeredParties == 0;
	///     }
	///   };
	///   phaser.register();
	///   for (final Runnable task : tasks) {
	///     phaser.register();
	///     new Thread() {
	///       public void run() {
	///         do {
	///           task.run();
	///           phaser.arriveAndAwaitAdvance();
	///         } while (!phaser.isTerminated());
	///       }
	///     }.start();
	///   }
	///   phaser.arriveAndDeregister(); // deregister self, don't wait
	/// }}</pre>
	/// 
	/// If the main task must later await termination, it
	/// may re-register and then execute a similar loop:
	///  <pre> {@code
	///   // ...
	///   phaser.register();
	///   while (!phaser.isTerminated())
	///     phaser.arriveAndAwaitAdvance();}</pre>
	/// 
	/// </para>
	/// <para>Related constructions may be used to await particular phase numbers
	/// in contexts where you are sure that the phase will never wrap around
	/// {@code Integer.MAX_VALUE}. For example:
	/// 
	///  <pre> {@code
	/// void awaitPhase(Phaser phaser, int phase) {
	///   int p = phaser.register(); // assumes caller not already registered
	///   while (p < phase) {
	///     if (phaser.isTerminated())
	///       // ... deal with unexpected termination
	///     else
	///       p = phaser.arriveAndAwaitAdvance();
	///   }
	///   phaser.arriveAndDeregister();
	/// }}</pre>
	/// 
	/// 
	/// </para>
	/// <para>To create a set of {@code n} tasks using a tree of phasers, you
	/// could use code of the following form, assuming a Task class with a
	/// constructor accepting a {@code Phaser} that it registers with upon
	/// construction. After invocation of {@code build(new Task[n], 0, n,
	/// new Phaser())}, these tasks could then be started, for example by
	/// submitting to a pool:
	/// 
	///  <pre> {@code
	/// void build(Task[] tasks, int lo, int hi, Phaser ph) {
	///   if (hi - lo > TASKS_PER_PHASER) {
	///     for (int i = lo; i < hi; i += TASKS_PER_PHASER) {
	///       int j = Math.min(i + TASKS_PER_PHASER, hi);
	///       build(tasks, i, j, new Phaser(ph));
	///     }
	///   } else {
	///     for (int i = lo; i < hi; ++i)
	///       tasks[i] = new Task(ph);
	///       // assumes new Task(ph) performs ph.register()
	///   }
	/// }}</pre>
	/// 
	/// The best value of {@code TASKS_PER_PHASER} depends mainly on
	/// expected synchronization rates. A value as low as four may
	/// be appropriate for extremely small per-phase task bodies (thus
	/// high rates), or up to hundreds for extremely large ones.
	/// 
	/// </para>
	/// <para><b>Implementation notes</b>: This implementation restricts the
	/// maximum number of parties to 65535. Attempts to register additional
	/// parties result in {@code IllegalStateException}. However, you can and
	/// should create tiered phasers to accommodate arbitrarily large sets
	/// of participants.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public class Phaser
	{
		/*
		 * This class implements an extension of X10 "clocks".  Thanks to
		 * Vijay Saraswat for the idea, and to Vivek Sarkar for
		 * enhancements to extend functionality.
		 */

		/// <summary>
		/// Primary state representation, holding four bit-fields:
		/// 
		/// unarrived  -- the number of parties yet to hit barrier (bits  0-15)
		/// parties    -- the number of parties to wait            (bits 16-31)
		/// phase      -- the generation of the barrier            (bits 32-62)
		/// terminated -- set if barrier is terminated             (bit  63 / sign)
		/// 
		/// Except that a phaser with no registered parties is
		/// distinguished by the otherwise illegal state of having zero
		/// parties and one unarrived parties (encoded as EMPTY below).
		/// 
		/// To efficiently maintain atomicity, these values are packed into
		/// a single (atomic) long. Good performance relies on keeping
		/// state decoding and encoding simple, and keeping race windows
		/// short.
		/// 
		/// All state updates are performed via CAS except initial
		/// registration of a sub-phaser (i.e., one with a non-null
		/// parent).  In this (relatively rare) case, we use built-in
		/// synchronization to lock while first registering with its
		/// parent.
		/// 
		/// The phase of a subphaser is allowed to lag that of its
		/// ancestors until it is actually accessed -- see method
		/// reconcileState.
		/// </summary>
		private volatile long State;

		private const int MAX_PARTIES = 0xffff;
		private const int MAX_PHASE = Integer.MaxValue;
		private const int PARTIES_SHIFT = 16;
		private const int PHASE_SHIFT = 32;
		private const int UNARRIVED_MASK = 0xffff; // to mask ints
		private const long PARTIES_MASK = 0xffff0000L; // to mask longs
		private const long COUNTS_MASK = 0xffffffffL;
		private static readonly long TERMINATION_BIT = 1L << 63;

		// some special values
		private const int ONE_ARRIVAL = 1;
		private static readonly int ONE_PARTY = 1 << PARTIES_SHIFT;
		private static readonly int ONE_DEREGISTER = ONE_ARRIVAL | ONE_PARTY;
		private const int EMPTY = 1;

		// The following unpacking methods are usually manually inlined

		private static int UnarrivedOf(long s)
		{
			int counts = (int)s;
			return (counts == EMPTY) ? 0 : (counts & UNARRIVED_MASK);
		}

		private static int PartiesOf(long s)
		{
			(int)((uint)return (int)s >> PARTIES_SHIFT);
		}

		private static int PhaseOf(long s)
		{
			return (int)((long)((ulong)s >> PHASE_SHIFT));
		}

		private static int ArrivedOf(long s)
		{
			int counts = (int)s;
			return (counts == EMPTY) ? 0 : ((int)((uint)counts >> PARTIES_SHIFT)) - (counts & UNARRIVED_MASK);
		}

		/// <summary>
		/// The parent of this phaser, or null if none
		/// </summary>
		private readonly Phaser Parent_Renamed;

		/// <summary>
		/// The root of phaser tree. Equals this if not in a tree.
		/// </summary>
		private readonly Phaser Root_Renamed;

		/// <summary>
		/// Heads of Treiber stacks for waiting threads. To eliminate
		/// contention when releasing some threads while adding others, we
		/// use two of them, alternating across even and odd phases.
		/// Subphasers share queues with root to speed up releases.
		/// </summary>
		private readonly AtomicReference<QNode> EvenQ;
		private readonly AtomicReference<QNode> OddQ;

		private AtomicReference<QNode> QueueFor(int phase)
		{
			return ((phase & 1) == 0) ? EvenQ : OddQ;
		}

		/// <summary>
		/// Returns message string for bounds exceptions on arrival.
		/// </summary>
		private String BadArrive(long s)
		{
			return "Attempted arrival of unregistered party for " + StateToString(s);
		}

		/// <summary>
		/// Returns message string for bounds exceptions on registration.
		/// </summary>
		private String BadRegister(long s)
		{
			return "Attempt to register more than " + MAX_PARTIES + " parties for " + StateToString(s);
		}

		/// <summary>
		/// Main implementation for methods arrive and arriveAndDeregister.
		/// Manually tuned to speed up and minimize race windows for the
		/// common case of just decrementing unarrived field.
		/// </summary>
		/// <param name="adjust"> value to subtract from state;
		///               ONE_ARRIVAL for arrive,
		///               ONE_DEREGISTER for arriveAndDeregister </param>
		private int DoArrive(int adjust)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			for (;;)
			{
				long s = (root == this) ? State : ReconcileState();
				int phase = (int)((long)((ulong)s >> PHASE_SHIFT));
				if (phase < 0)
				{
					return phase;
				}
				int counts = (int)s;
				int unarrived = (counts == EMPTY) ? 0 : (counts & UNARRIVED_MASK);
				if (unarrived <= 0)
				{
					throw new IllegalStateException(BadArrive(s));
				}
				if (UNSAFE.compareAndSwapLong(this, StateOffset, s, s -= adjust))
				{
					if (unarrived == 1)
					{
						long n = s & PARTIES_MASK; // base of next state
						int nextUnarrived = (int)((uint)(int)n >> PARTIES_SHIFT);
						if (root == this)
						{
							if (OnAdvance(phase, nextUnarrived))
							{
								n |= TERMINATION_BIT;
							}
							else if (nextUnarrived == 0)
							{
								n |= EMPTY;
							}
							else
							{
								n |= nextUnarrived;
							}
							int nextPhase = (phase + 1) & MAX_PHASE;
							n |= (long)nextPhase << PHASE_SHIFT;
							UNSAFE.compareAndSwapLong(this, StateOffset, s, n);
							ReleaseWaiters(phase);
						}
						else if (nextUnarrived == 0) // propagate deregistration
						{
							phase = Parent_Renamed.DoArrive(ONE_DEREGISTER);
							UNSAFE.compareAndSwapLong(this, StateOffset, s, s | EMPTY);
						}
						else
						{
							phase = Parent_Renamed.DoArrive(ONE_ARRIVAL);
						}
					}
					return phase;
				}
			}
		}

		/// <summary>
		/// Implementation of register, bulkRegister
		/// </summary>
		/// <param name="registrations"> number to add to both parties and
		/// unarrived fields. Must be greater than zero. </param>
		private int DoRegister(int registrations)
		{
			// adjustment to state
			long adjust = ((long)registrations << PARTIES_SHIFT) | registrations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser parent = this.parent;
			Phaser parent = this.Parent_Renamed;
			int phase;
			for (;;)
			{
				long s = (parent == null) ? State : ReconcileState();
				int counts = (int)s;
				int parties = (int)((uint)counts >> PARTIES_SHIFT);
				int unarrived = counts & UNARRIVED_MASK;
				if (registrations > MAX_PARTIES - parties)
				{
					throw new IllegalStateException(BadRegister(s));
				}
				phase = (int)((long)((ulong)s >> PHASE_SHIFT));
				if (phase < 0)
				{
					break;
				}
				if (counts != EMPTY) // not 1st registration
				{
					if (parent == null || ReconcileState() == s)
					{
						if (unarrived == 0) // wait out advance
						{
							Root_Renamed.InternalAwaitAdvance(phase, null);
						}
						else if (UNSAFE.compareAndSwapLong(this, StateOffset, s, s + adjust))
						{
							break;
						}
					}
				}
				else if (parent == null) // 1st root registration
				{
					long next = ((long)phase << PHASE_SHIFT) | adjust;
					if (UNSAFE.compareAndSwapLong(this, StateOffset, s, next))
					{
						break;
					}
				}
				else
				{
					lock (this) // 1st sub registration
					{
						if (State == s) // recheck under lock
						{
							phase = parent.DoRegister(1);
							if (phase < 0)
							{
								break;
							}
							// finish registration whenever parent registration
							// succeeded, even when racing with termination,
							// since these are part of the same "transaction".
							while (!UNSAFE.compareAndSwapLong(this, StateOffset, s, ((long)phase << PHASE_SHIFT) | adjust))
							{
								s = State;
								phase = (int)((long)((ulong)Root_Renamed.State >> PHASE_SHIFT));
								// assert (int)s == EMPTY;
							}
							break;
						}
					}
				}
			}
			return phase;
		}

		/// <summary>
		/// Resolves lagged phase propagation from root if necessary.
		/// Reconciliation normally occurs when root has advanced but
		/// subphasers have not yet done so, in which case they must finish
		/// their own advance by setting unarrived to parties (or if
		/// parties is zero, resetting to unregistered EMPTY state).
		/// </summary>
		/// <returns> reconciled state </returns>
		private long ReconcileState()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			long s = State;
			if (root != this)
			{
				int phase, p;
				// CAS to root phase with current parties, tripping unarrived
				while ((phase = (int)((long)((ulong)root.State >> PHASE_SHIFT))) != (int)((long)((ulong)s >> PHASE_SHIFT)) && !UNSAFE.compareAndSwapLong(this, StateOffset, s, s = (((long)phase << PHASE_SHIFT) | ((phase < 0) ? (s & COUNTS_MASK) : (((p = (int)((uint)(int)s >> PARTIES_SHIFT)) == 0) ? EMPTY : ((s & PARTIES_MASK) | p))))))
				{
					s = State;
				}
			}
			return s;
		}

		/// <summary>
		/// Creates a new phaser with no initially registered parties, no
		/// parent, and initial phase number 0. Any thread using this
		/// phaser will need to first register for it.
		/// </summary>
		public Phaser() : this(null, 0)
		{
		}

		/// <summary>
		/// Creates a new phaser with the given number of registered
		/// unarrived parties, no parent, and initial phase number 0.
		/// </summary>
		/// <param name="parties"> the number of parties required to advance to the
		/// next phase </param>
		/// <exception cref="IllegalArgumentException"> if parties less than zero
		/// or greater than the maximum number of parties supported </exception>
		public Phaser(int parties) : this(null, parties)
		{
		}

		/// <summary>
		/// Equivalent to <seealso cref="#Phaser(Phaser, int) Phaser(parent, 0)"/>.
		/// </summary>
		/// <param name="parent"> the parent phaser </param>
		public Phaser(Phaser parent) : this(parent, 0)
		{
		}

		/// <summary>
		/// Creates a new phaser with the given parent and number of
		/// registered unarrived parties.  When the given parent is non-null
		/// and the given number of parties is greater than zero, this
		/// child phaser is registered with its parent.
		/// </summary>
		/// <param name="parent"> the parent phaser </param>
		/// <param name="parties"> the number of parties required to advance to the
		/// next phase </param>
		/// <exception cref="IllegalArgumentException"> if parties less than zero
		/// or greater than the maximum number of parties supported </exception>
		public Phaser(Phaser parent, int parties)
		{
			if ((int)((uint)parties >> PARTIES_SHIFT) != 0)
			{
				throw new IllegalArgumentException("Illegal number of parties");
			}
			int phase = 0;
			this.Parent_Renamed = parent;
			if (parent != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = parent.root;
				Phaser root = parent.Root_Renamed;
				this.Root_Renamed = root;
				this.EvenQ = root.EvenQ;
				this.OddQ = root.OddQ;
				if (parties != 0)
				{
					phase = parent.DoRegister(1);
				}
			}
			else
			{
				this.Root_Renamed = this;
				this.EvenQ = new AtomicReference<QNode>();
				this.OddQ = new AtomicReference<QNode>();
			}
			this.State = (parties == 0) ? (long)EMPTY : ((long)phase << PHASE_SHIFT) | ((long)parties << PARTIES_SHIFT) | ((long)parties);
		}

		/// <summary>
		/// Adds a new unarrived party to this phaser.  If an ongoing
		/// invocation of <seealso cref="#onAdvance"/> is in progress, this method
		/// may await its completion before returning.  If this phaser has
		/// a parent, and this phaser previously had no registered parties,
		/// this child phaser is also registered with its parent. If
		/// this phaser is terminated, the attempt to register has
		/// no effect, and a negative value is returned.
		/// </summary>
		/// <returns> the arrival phase number to which this registration
		/// applied.  If this value is negative, then this phaser has
		/// terminated, in which case registration has no effect. </returns>
		/// <exception cref="IllegalStateException"> if attempting to register more
		/// than the maximum supported number of parties </exception>
		public virtual int Register()
		{
			return DoRegister(1);
		}

		/// <summary>
		/// Adds the given number of new unarrived parties to this phaser.
		/// If an ongoing invocation of <seealso cref="#onAdvance"/> is in progress,
		/// this method may await its completion before returning.  If this
		/// phaser has a parent, and the given number of parties is greater
		/// than zero, and this phaser previously had no registered
		/// parties, this child phaser is also registered with its parent.
		/// If this phaser is terminated, the attempt to register has no
		/// effect, and a negative value is returned.
		/// </summary>
		/// <param name="parties"> the number of additional parties required to
		/// advance to the next phase </param>
		/// <returns> the arrival phase number to which this registration
		/// applied.  If this value is negative, then this phaser has
		/// terminated, in which case registration has no effect. </returns>
		/// <exception cref="IllegalStateException"> if attempting to register more
		/// than the maximum supported number of parties </exception>
		/// <exception cref="IllegalArgumentException"> if {@code parties < 0} </exception>
		public virtual int BulkRegister(int parties)
		{
			if (parties < 0)
			{
				throw new IllegalArgumentException();
			}
			if (parties == 0)
			{
				return Phase;
			}
			return DoRegister(parties);
		}

		/// <summary>
		/// Arrives at this phaser, without waiting for others to arrive.
		/// 
		/// <para>It is a usage error for an unregistered party to invoke this
		/// method.  However, this error may result in an {@code
		/// IllegalStateException} only upon some subsequent operation on
		/// this phaser, if ever.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the arrival phase number, or a negative value if terminated </returns>
		/// <exception cref="IllegalStateException"> if not terminated and the number
		/// of unarrived parties would become negative </exception>
		public virtual int Arrive()
		{
			return DoArrive(ONE_ARRIVAL);
		}

		/// <summary>
		/// Arrives at this phaser and deregisters from it without waiting
		/// for others to arrive. Deregistration reduces the number of
		/// parties required to advance in future phases.  If this phaser
		/// has a parent, and deregistration causes this phaser to have
		/// zero parties, this phaser is also deregistered from its parent.
		/// 
		/// <para>It is a usage error for an unregistered party to invoke this
		/// method.  However, this error may result in an {@code
		/// IllegalStateException} only upon some subsequent operation on
		/// this phaser, if ever.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the arrival phase number, or a negative value if terminated </returns>
		/// <exception cref="IllegalStateException"> if not terminated and the number
		/// of registered or unarrived parties would become negative </exception>
		public virtual int ArriveAndDeregister()
		{
			return DoArrive(ONE_DEREGISTER);
		}

		/// <summary>
		/// Arrives at this phaser and awaits others. Equivalent in effect
		/// to {@code awaitAdvance(arrive())}.  If you need to await with
		/// interruption or timeout, you can arrange this with an analogous
		/// construction using one of the other forms of the {@code
		/// awaitAdvance} method.  If instead you need to deregister upon
		/// arrival, use {@code awaitAdvance(arriveAndDeregister())}.
		/// 
		/// <para>It is a usage error for an unregistered party to invoke this
		/// method.  However, this error may result in an {@code
		/// IllegalStateException} only upon some subsequent operation on
		/// this phaser, if ever.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the arrival phase number, or the (negative)
		/// <seealso cref="#getPhase() current phase"/> if terminated </returns>
		/// <exception cref="IllegalStateException"> if not terminated and the number
		/// of unarrived parties would become negative </exception>
		public virtual int ArriveAndAwaitAdvance()
		{
			// Specialization of doArrive+awaitAdvance eliminating some reads/paths
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			for (;;)
			{
				long s = (root == this) ? State : ReconcileState();
				int phase = (int)((long)((ulong)s >> PHASE_SHIFT));
				if (phase < 0)
				{
					return phase;
				}
				int counts = (int)s;
				int unarrived = (counts == EMPTY) ? 0 : (counts & UNARRIVED_MASK);
				if (unarrived <= 0)
				{
					throw new IllegalStateException(BadArrive(s));
				}
				if (UNSAFE.compareAndSwapLong(this, StateOffset, s, s -= ONE_ARRIVAL))
				{
					if (unarrived > 1)
					{
						return root.InternalAwaitAdvance(phase, null);
					}
					if (root != this)
					{
						return Parent_Renamed.ArriveAndAwaitAdvance();
					}
					long n = s & PARTIES_MASK; // base of next state
					int nextUnarrived = (int)((uint)(int)n >> PARTIES_SHIFT);
					if (OnAdvance(phase, nextUnarrived))
					{
						n |= TERMINATION_BIT;
					}
					else if (nextUnarrived == 0)
					{
						n |= EMPTY;
					}
					else
					{
						n |= nextUnarrived;
					}
					int nextPhase = (phase + 1) & MAX_PHASE;
					n |= (long)nextPhase << PHASE_SHIFT;
					if (!UNSAFE.compareAndSwapLong(this, StateOffset, s, n))
					{
						return (int)((long)((ulong)State >> PHASE_SHIFT)); // terminated
					}
					ReleaseWaiters(phase);
					return nextPhase;
				}
			}
		}

		/// <summary>
		/// Awaits the phase of this phaser to advance from the given phase
		/// value, returning immediately if the current phase is not equal
		/// to the given phase value or this phaser is terminated.
		/// </summary>
		/// <param name="phase"> an arrival phase number, or negative value if
		/// terminated; this argument is normally the value returned by a
		/// previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		/// <returns> the next arrival phase number, or the argument if it is
		/// negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		/// if terminated </returns>
		public virtual int AwaitAdvance(int phase)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			long s = (root == this) ? State : ReconcileState();
			int p = (int)((long)((ulong)s >> PHASE_SHIFT));
			if (phase < 0)
			{
				return phase;
			}
			if (p == phase)
			{
				return root.InternalAwaitAdvance(phase, null);
			}
			return p;
		}

		/// <summary>
		/// Awaits the phase of this phaser to advance from the given phase
		/// value, throwing {@code InterruptedException} if interrupted
		/// while waiting, or returning immediately if the current phase is
		/// not equal to the given phase value or this phaser is
		/// terminated.
		/// </summary>
		/// <param name="phase"> an arrival phase number, or negative value if
		/// terminated; this argument is normally the value returned by a
		/// previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		/// <returns> the next arrival phase number, or the argument if it is
		/// negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		/// if terminated </returns>
		/// <exception cref="InterruptedException"> if thread interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int awaitAdvanceInterruptibly(int phase) throws InterruptedException
		public virtual int AwaitAdvanceInterruptibly(int phase)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			long s = (root == this) ? State : ReconcileState();
			int p = (int)((long)((ulong)s >> PHASE_SHIFT));
			if (phase < 0)
			{
				return phase;
			}
			if (p == phase)
			{
				QNode node = new QNode(this, phase, true, false, 0L);
				p = root.InternalAwaitAdvance(phase, node);
				if (node.WasInterrupted)
				{
					throw new InterruptedException();
				}
			}
			return p;
		}

		/// <summary>
		/// Awaits the phase of this phaser to advance from the given phase
		/// value or the given timeout to elapse, throwing {@code
		/// InterruptedException} if interrupted while waiting, or
		/// returning immediately if the current phase is not equal to the
		/// given phase value or this phaser is terminated.
		/// </summary>
		/// <param name="phase"> an arrival phase number, or negative value if
		/// terminated; this argument is normally the value returned by a
		/// previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> the next arrival phase number, or the argument if it is
		/// negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		/// if terminated </returns>
		/// <exception cref="InterruptedException"> if thread interrupted while waiting </exception>
		/// <exception cref="TimeoutException"> if timed out while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int awaitAdvanceInterruptibly(int phase, long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.TimeoutException
		public virtual int AwaitAdvanceInterruptibly(int phase, long timeout, TimeUnit unit)
		{
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			long s = (root == this) ? State : ReconcileState();
			int p = (int)((long)((ulong)s >> PHASE_SHIFT));
			if (phase < 0)
			{
				return phase;
			}
			if (p == phase)
			{
				QNode node = new QNode(this, phase, true, true, nanos);
				p = root.InternalAwaitAdvance(phase, node);
				if (node.WasInterrupted)
				{
					throw new InterruptedException();
				}
				else if (p == phase)
				{
					throw new TimeoutException();
				}
			}
			return p;
		}

		/// <summary>
		/// Forces this phaser to enter termination state.  Counts of
		/// registered parties are unaffected.  If this phaser is a member
		/// of a tiered set of phasers, then all of the phasers in the set
		/// are terminated.  If this phaser is already terminated, this
		/// method has no effect.  This method may be useful for
		/// coordinating recovery after one or more tasks encounter
		/// unexpected exceptions.
		/// </summary>
		public virtual void ForceTermination()
		{
			// Only need to change root state
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Phaser root = this.root;
			Phaser root = this.Root_Renamed;
			long s;
			while ((s = root.State) >= 0)
			{
				if (UNSAFE.compareAndSwapLong(root, StateOffset, s, s | TERMINATION_BIT))
				{
					// signal all threads
					ReleaseWaiters(0); // Waiters on evenQ
					ReleaseWaiters(1); // Waiters on oddQ
					return;
				}
			}
		}

		/// <summary>
		/// Returns the current phase number. The maximum phase number is
		/// {@code Integer.MAX_VALUE}, after which it restarts at
		/// zero. Upon termination, the phase number is negative,
		/// in which case the prevailing phase prior to termination
		/// may be obtained via {@code getPhase() + Integer.MIN_VALUE}.
		/// </summary>
		/// <returns> the phase number, or a negative value if terminated </returns>
		public int Phase
		{
			get
			{
				return (int)((long)((ulong)Root_Renamed.State >> PHASE_SHIFT));
			}
		}

		/// <summary>
		/// Returns the number of parties registered at this phaser.
		/// </summary>
		/// <returns> the number of parties </returns>
		public virtual int RegisteredParties
		{
			get
			{
				return PartiesOf(State);
			}
		}

		/// <summary>
		/// Returns the number of registered parties that have arrived at
		/// the current phase of this phaser. If this phaser has terminated,
		/// the returned value is meaningless and arbitrary.
		/// </summary>
		/// <returns> the number of arrived parties </returns>
		public virtual int ArrivedParties
		{
			get
			{
				return ArrivedOf(ReconcileState());
			}
		}

		/// <summary>
		/// Returns the number of registered parties that have not yet
		/// arrived at the current phase of this phaser. If this phaser has
		/// terminated, the returned value is meaningless and arbitrary.
		/// </summary>
		/// <returns> the number of unarrived parties </returns>
		public virtual int UnarrivedParties
		{
			get
			{
				return UnarrivedOf(ReconcileState());
			}
		}

		/// <summary>
		/// Returns the parent of this phaser, or {@code null} if none.
		/// </summary>
		/// <returns> the parent of this phaser, or {@code null} if none </returns>
		public virtual Phaser Parent
		{
			get
			{
				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Returns the root ancestor of this phaser, which is the same as
		/// this phaser if it has no parent.
		/// </summary>
		/// <returns> the root ancestor of this phaser </returns>
		public virtual Phaser Root
		{
			get
			{
				return Root_Renamed;
			}
		}

		/// <summary>
		/// Returns {@code true} if this phaser has been terminated.
		/// </summary>
		/// <returns> {@code true} if this phaser has been terminated </returns>
		public virtual bool Terminated
		{
			get
			{
				return Root_Renamed.State < 0L;
			}
		}

		/// <summary>
		/// Overridable method to perform an action upon impending phase
		/// advance, and to control termination. This method is invoked
		/// upon arrival of the party advancing this phaser (when all other
		/// waiting parties are dormant).  If this method returns {@code
		/// true}, this phaser will be set to a final termination state
		/// upon advance, and subsequent calls to <seealso cref="#isTerminated"/>
		/// will return true. Any (unchecked) Exception or Error thrown by
		/// an invocation of this method is propagated to the party
		/// attempting to advance this phaser, in which case no advance
		/// occurs.
		/// 
		/// <para>The arguments to this method provide the state of the phaser
		/// prevailing for the current transition.  The effects of invoking
		/// arrival, registration, and waiting methods on this phaser from
		/// within {@code onAdvance} are unspecified and should not be
		/// relied on.
		/// 
		/// </para>
		/// <para>If this phaser is a member of a tiered set of phasers, then
		/// {@code onAdvance} is invoked only for its root phaser on each
		/// advance.
		/// 
		/// </para>
		/// <para>To support the most common use cases, the default
		/// implementation of this method returns {@code true} when the
		/// number of registered parties has become zero as the result of a
		/// party invoking {@code arriveAndDeregister}.  You can disable
		/// this behavior, thus enabling continuation upon future
		/// registrations, by overriding this method to always return
		/// {@code false}:
		/// 
		/// <pre> {@code
		/// Phaser phaser = new Phaser() {
		///   protected boolean onAdvance(int phase, int parties) { return false; }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="phase"> the current phase number on entry to this method,
		/// before this phaser is advanced </param>
		/// <param name="registeredParties"> the current number of registered parties </param>
		/// <returns> {@code true} if this phaser should terminate </returns>
		protected internal virtual bool OnAdvance(int phase, int registeredParties)
		{
			return registeredParties == 0;
		}

		/// <summary>
		/// Returns a string identifying this phaser, as well as its
		/// state.  The state, in brackets, includes the String {@code
		/// "phase = "} followed by the phase number, {@code "parties = "}
		/// followed by the number of registered parties, and {@code
		/// "arrived = "} followed by the number of arrived parties.
		/// </summary>
		/// <returns> a string identifying this phaser, as well as its state </returns>
		public override String ToString()
		{
			return StateToString(ReconcileState());
		}

		/// <summary>
		/// Implementation of toString and string-based error messages
		/// </summary>
		private String StateToString(long s)
		{
			return base.ToString() + "[phase = " + PhaseOf(s) + " parties = " + PartiesOf(s) + " arrived = " + ArrivedOf(s) + "]";
		}

		// Waiting mechanics

		/// <summary>
		/// Removes and signals threads from queue for phase.
		/// </summary>
		private void ReleaseWaiters(int phase)
		{
			QNode q; // first element of queue
			Thread t; // its thread
			AtomicReference<QNode> head = (phase & 1) == 0 ? EvenQ : OddQ;
			while ((q = head.Get()) != null && q.Phase != (int)((long)((ulong)Root_Renamed.State >> PHASE_SHIFT)))
			{
				if (head.CompareAndSet(q, q.Next) && (t = q.Thread) != null)
				{
					q.Thread = null;
					LockSupport.Unpark(t);
				}
			}
		}

		/// <summary>
		/// Variant of releaseWaiters that additionally tries to remove any
		/// nodes no longer waiting for advance due to timeout or
		/// interrupt. Currently, nodes are removed only if they are at
		/// head of queue, which suffices to reduce memory footprint in
		/// most usages.
		/// </summary>
		/// <returns> current phase on exit </returns>
		private int AbortWait(int phase)
		{
			AtomicReference<QNode> head = (phase & 1) == 0 ? EvenQ : OddQ;
			for (;;)
			{
				Thread t;
				QNode q = head.Get();
				int p = (int)((long)((ulong)Root_Renamed.State >> PHASE_SHIFT));
				if (q == null || ((t = q.Thread) != null && q.Phase == p))
				{
					return p;
				}
				if (head.CompareAndSet(q, q.Next) && t != null)
				{
					q.Thread = null;
					LockSupport.Unpark(t);
				}
			}
		}

		/// <summary>
		/// The number of CPUs, for spin control </summary>
		private static readonly int NCPU = Runtime.Runtime.availableProcessors();

		/// <summary>
		/// The number of times to spin before blocking while waiting for
		/// advance, per arrival while waiting. On multiprocessors, fully
		/// blocking and waking up a large number of threads all at once is
		/// usually a very slow process, so we use rechargeable spins to
		/// avoid it when threads regularly arrive: When a thread in
		/// internalAwaitAdvance notices another arrival before blocking,
		/// and there appear to be enough CPUs available, it spins
		/// SPINS_PER_ARRIVAL more times before blocking. The value trades
		/// off good-citizenship vs big unnecessary slowdowns.
		/// </summary>
		internal static readonly int SPINS_PER_ARRIVAL = (NCPU < 2) ? 1 : 1 << 8;

		/// <summary>
		/// Possibly blocks and waits for phase to advance unless aborted.
		/// Call only on root phaser.
		/// </summary>
		/// <param name="phase"> current phase </param>
		/// <param name="node"> if non-null, the wait node to track interrupt and timeout;
		/// if null, denotes noninterruptible wait </param>
		/// <returns> current phase </returns>
		private int InternalAwaitAdvance(int phase, QNode node)
		{
			// assert root == this;
			ReleaseWaiters(phase-1); // ensure old queue clean
			bool queued = false; // true when node is enqueued
			int lastUnarrived = 0; // to increase spins upon change
			int spins = SPINS_PER_ARRIVAL;
			long s;
			int p;
			while ((p = (int)((int)((uint)(s = State) >> PHASE_SHIFT))) == phase)
			{
				if (node == null) // spinning in noninterruptible mode
				{
					int unarrived = (int)s & UNARRIVED_MASK;
					if (unarrived != lastUnarrived && (lastUnarrived = unarrived) < NCPU)
					{
						spins += SPINS_PER_ARRIVAL;
					}
					bool interrupted = Thread.Interrupted();
					if (interrupted || --spins < 0) // need node to record intr
					{
						node = new QNode(this, phase, false, false, 0L);
						node.WasInterrupted = interrupted;
					}
				}
				else if (node.Releasable) // done or aborted
				{
					break;
				}
				else if (!queued) // push onto queue
				{
					AtomicReference<QNode> head = (phase & 1) == 0 ? EvenQ : OddQ;
					QNode q = node.Next = head.Get();
					if ((q == null || q.Phase == phase) && (int)((long)((ulong)State >> PHASE_SHIFT)) == phase) // avoid stale enq
					{
						queued = head.CompareAndSet(q, node);
					}
				}
				else
				{
					try
					{
						ForkJoinPool.ManagedBlock(node);
					}
					catch (InterruptedException)
					{
						node.WasInterrupted = true;
					}
				}
			}

			if (node != null)
			{
				if (node.Thread != null)
				{
					node.Thread = null; // avoid need for unpark()
				}
				if (node.WasInterrupted && !node.Interruptible)
				{
					Thread.CurrentThread.Interrupt();
				}
				if (p == phase && (p = (int)((long)((ulong)State >> PHASE_SHIFT))) == phase)
				{
					return AbortWait(phase); // possibly clean up on abort
				}
			}
			ReleaseWaiters(phase);
			return p;
		}

		/// <summary>
		/// Wait nodes for Treiber stack representing wait queue
		/// </summary>
		internal sealed class QNode : ForkJoinPool.ManagedBlocker
		{
			internal readonly Phaser Phaser;
			internal readonly int Phase;
			internal readonly bool Interruptible;
			internal readonly bool Timed;
			internal bool WasInterrupted;
			internal long Nanos;
			internal readonly long Deadline;
			internal volatile Thread Thread; // nulled to cancel wait
			internal QNode Next;

			internal QNode(Phaser phaser, int phase, bool interruptible, bool timed, long nanos)
			{
				this.Phaser = phaser;
				this.Phase = phase;
				this.Interruptible = interruptible;
				this.Nanos = nanos;
				this.Timed = timed;
				this.Deadline = timed ? System.nanoTime() + nanos : 0L;
				Thread = Thread.CurrentThread;
			}

			public bool Releasable
			{
				get
				{
					if (Thread == null)
					{
						return true;
					}
					if (Phaser.Phase != Phase)
					{
						Thread = null;
						return true;
					}
					if (Thread.Interrupted())
					{
						WasInterrupted = true;
					}
					if (WasInterrupted && Interruptible)
					{
						Thread = null;
						return true;
					}
					if (Timed)
					{
						if (Nanos > 0L)
						{
							Nanos = Deadline - System.nanoTime();
						}
						if (Nanos <= 0L)
						{
							Thread = null;
							return true;
						}
					}
					return false;
				}
			}

			public bool Block()
			{
				if (Releasable)
				{
					return true;
				}
				else if (!Timed)
				{
					LockSupport.Park(this);
				}
				else if (Nanos > 0L)
				{
					LockSupport.ParkNanos(this, Nanos);
				}
				return Releasable;
			}
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long StateOffset;
		static Phaser()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(Phaser);
				StateOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("state"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}