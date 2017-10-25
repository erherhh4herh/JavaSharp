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
	/// A <seealso cref="BlockingQueue blocking queue"/> in which each insert
	/// operation must wait for a corresponding remove operation by another
	/// thread, and vice versa.  A synchronous queue does not have any
	/// internal capacity, not even a capacity of one.  You cannot
	/// {@code peek} at a synchronous queue because an element is only
	/// present when you try to remove it; you cannot insert an element
	/// (using any method) unless another thread is trying to remove it;
	/// you cannot iterate as there is nothing to iterate.  The
	/// <em>head</em> of the queue is the element that the first queued
	/// inserting thread is trying to add to the queue; if there is no such
	/// queued thread then no element is available for removal and
	/// {@code poll()} will return {@code null}.  For purposes of other
	/// {@code Collection} methods (for example {@code contains}), a
	/// {@code SynchronousQueue} acts as an empty collection.  This queue
	/// does not permit {@code null} elements.
	/// 
	/// <para>Synchronous queues are similar to rendezvous channels used in
	/// CSP and Ada. They are well suited for handoff designs, in which an
	/// object running in one thread must sync up with an object running
	/// in another thread in order to hand it some information, event, or
	/// task.
	/// 
	/// </para>
	/// <para>This class supports an optional fairness policy for ordering
	/// waiting producer and consumer threads.  By default, this ordering
	/// is not guaranteed. However, a queue constructed with fairness set
	/// to {@code true} grants threads access in FIFO order.
	/// 
	/// </para>
	/// <para>This class and its iterator implement all of the
	/// <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	/// Iterator} interfaces.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.5
	/// @author Doug Lea and Bill Scherer and Michael Scott
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class SynchronousQueue<E> : AbstractQueue<E>, BlockingQueue<E>
	{
		private const long SerialVersionUID = -3223113410248163686L;

		/*
		 * This class implements extensions of the dual stack and dual
		 * queue algorithms described in "Nonblocking Concurrent Objects
		 * with Condition Synchronization", by W. N. Scherer III and
		 * M. L. Scott.  18th Annual Conf. on Distributed Computing,
		 * Oct. 2004 (see also
		 * http://www.cs.rochester.edu/u/scott/synchronization/pseudocode/duals.html).
		 * The (Lifo) stack is used for non-fair mode, and the (Fifo)
		 * queue for fair mode. The performance of the two is generally
		 * similar. Fifo usually supports higher throughput under
		 * contention but Lifo maintains higher thread locality in common
		 * applications.
		 *
		 * A dual queue (and similarly stack) is one that at any given
		 * time either holds "data" -- items provided by put operations,
		 * or "requests" -- slots representing take operations, or is
		 * empty. A call to "fulfill" (i.e., a call requesting an item
		 * from a queue holding data or vice versa) dequeues a
		 * complementary node.  The most interesting feature of these
		 * queues is that any operation can figure out which mode the
		 * queue is in, and act accordingly without needing locks.
		 *
		 * Both the queue and stack extend abstract class Transferer
		 * defining the single method transfer that does a put or a
		 * take. These are unified into a single method because in dual
		 * data structures, the put and take operations are symmetrical,
		 * so nearly all code can be combined. The resulting transfer
		 * methods are on the long side, but are easier to follow than
		 * they would be if broken up into nearly-duplicated parts.
		 *
		 * The queue and stack data structures share many conceptual
		 * similarities but very few concrete details. For simplicity,
		 * they are kept distinct so that they can later evolve
		 * separately.
		 *
		 * The algorithms here differ from the versions in the above paper
		 * in extending them for use in synchronous queues, as well as
		 * dealing with cancellation. The main differences include:
		 *
		 *  1. The original algorithms used bit-marked pointers, but
		 *     the ones here use mode bits in nodes, leading to a number
		 *     of further adaptations.
		 *  2. SynchronousQueues must block threads waiting to become
		 *     fulfilled.
		 *  3. Support for cancellation via timeout and interrupts,
		 *     including cleaning out cancelled nodes/threads
		 *     from lists to avoid garbage retention and memory depletion.
		 *
		 * Blocking is mainly accomplished using LockSupport park/unpark,
		 * except that nodes that appear to be the next ones to become
		 * fulfilled first spin a bit (on multiprocessors only). On very
		 * busy synchronous queues, spinning can dramatically improve
		 * throughput. And on less busy ones, the amount of spinning is
		 * small enough not to be noticeable.
		 *
		 * Cleaning is done in different ways in queues vs stacks.  For
		 * queues, we can almost always remove a node immediately in O(1)
		 * time (modulo retries for consistency checks) when it is
		 * cancelled. But if it may be pinned as the current tail, it must
		 * wait until some subsequent cancellation. For stacks, we need a
		 * potentially O(n) traversal to be sure that we can remove the
		 * node, but this can run concurrently with other threads
		 * accessing the stack.
		 *
		 * While garbage collection takes care of most node reclamation
		 * issues that otherwise complicate nonblocking algorithms, care
		 * is taken to "forget" references to data, other nodes, and
		 * threads that might be held on to long-term by blocked
		 * threads. In cases where setting to null would otherwise
		 * conflict with main algorithms, this is done by changing a
		 * node's link to now point to the node itself. This doesn't arise
		 * much for Stack nodes (because blocked threads do not hang on to
		 * old head pointers), but references in Queue nodes must be
		 * aggressively forgotten to avoid reachability of everything any
		 * node has ever referred to since arrival.
		 */

		/// <summary>
		/// Shared internal API for dual stacks and queues.
		/// </summary>
		internal abstract class Transferer<E>
		{
			/// <summary>
			/// Performs a put or take.
			/// </summary>
			/// <param name="e"> if non-null, the item to be handed to a consumer;
			///          if null, requests that transfer return an item
			///          offered by producer. </param>
			/// <param name="timed"> if this operation should timeout </param>
			/// <param name="nanos"> the timeout, in nanoseconds </param>
			/// <returns> if non-null, the item provided or received; if null,
			///         the operation failed due to timeout or interrupt --
			///         the caller can distinguish which of these occurred
			///         by checking Thread.interrupted. </returns>
			internal abstract E Transfer(E e, bool timed, long nanos);
		}

		/// <summary>
		/// The number of CPUs, for spin control </summary>
		internal static readonly int NCPUS = Runtime.Runtime.availableProcessors();

		/// <summary>
		/// The number of times to spin before blocking in timed waits.
		/// The value is empirically derived -- it works well across a
		/// variety of processors and OSes. Empirically, the best value
		/// seems not to vary with number of CPUs (beyond 2) so is just
		/// a constant.
		/// </summary>
		internal static readonly int MaxTimedSpins = (NCPUS < 2) ? 0 : 32;

		/// <summary>
		/// The number of times to spin before blocking in untimed waits.
		/// This is greater than timed value because untimed waits spin
		/// faster since they don't need to check times on each spin.
		/// </summary>
		internal static readonly int MaxUntimedSpins = MaxTimedSpins * 16;

		/// <summary>
		/// The number of nanoseconds for which it is faster to spin
		/// rather than to use timed park. A rough estimate suffices.
		/// </summary>
		internal const long SpinForTimeoutThreshold = 1000L;

		/// <summary>
		/// Dual stack </summary>
		internal sealed class TransferStack<E> : Transferer<E>
		{
			/*
			 * This extends Scherer-Scott dual stack algorithm, differing,
			 * among other ways, by using "covering" nodes rather than
			 * bit-marked pointers: Fulfilling operations push on marker
			 * nodes (with FULFILLING bit set in mode) to reserve a spot
			 * to match a waiting node.
			 */

			/* Modes for SNodes, ORed together in node fields */
			/// <summary>
			/// Node represents an unfulfilled consumer </summary>
			internal const int REQUEST = 0;
			/// <summary>
			/// Node represents an unfulfilled producer </summary>
			internal const int DATA = 1;
			/// <summary>
			/// Node is fulfilling another unfulfilled DATA or REQUEST </summary>
			internal const int FULFILLING = 2;

			/// <summary>
			/// Returns true if m has fulfilling bit set. </summary>
			internal static bool IsFulfilling(int m)
			{
				return (m & FULFILLING) != 0;
			}

			/// <summary>
			/// Node class for TransferStacks. </summary>
			internal sealed class SNode
			{
				internal volatile SNode Next; // next node in stack
				internal volatile SNode Match; // the node matched to this
				internal volatile Thread Waiter; // to control park/unpark
				internal Object Item; // data; or null for REQUESTs
				internal int Mode;
				// Note: item and mode fields don't need to be volatile
				// since they are always written before, and read after,
				// other volatile/atomic operations.

				internal SNode(Object item)
				{
					this.Item = item;
				}

				internal bool CasNext(SNode cmp, SNode val)
				{
					return cmp == Next && UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
				}

				/// <summary>
				/// Tries to match node s to this node, if so, waking up thread.
				/// Fulfillers call tryMatch to identify their waiters.
				/// Waiters block until they have been matched.
				/// </summary>
				/// <param name="s"> the node to match </param>
				/// <returns> true if successfully matched to s </returns>
				internal bool TryMatch(SNode s)
				{
					if (Match == null && UNSAFE.compareAndSwapObject(this, MatchOffset, null, s))
					{
						Thread w = Waiter;
						if (w != null) // waiters need at most one unpark
						{
							Waiter = null;
							LockSupport.Unpark(w);
						}
						return true;
					}
					return Match == s;
				}

				/// <summary>
				/// Tries to cancel a wait by matching node to itself.
				/// </summary>
				internal void TryCancel()
				{
					UNSAFE.compareAndSwapObject(this, MatchOffset, null, this);
				}

				internal bool Cancelled
				{
					get
					{
						return Match == this;
					}
				}

				// Unsafe mechanics
				internal static readonly sun.misc.Unsafe UNSAFE;
				internal static readonly long MatchOffset;
				internal static readonly long NextOffset;

				static SNode()
				{
					try
					{
						UNSAFE = sun.misc.Unsafe.Unsafe;
						Class k = typeof(SNode);
						MatchOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("match"));
						NextOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("next"));
					}
					catch (Exception e)
					{
						throw new Error(e);
					}
				}
			}

			/// <summary>
			/// The head (top) of the stack </summary>
			internal volatile SNode Head;

			internal bool CasHead(SNode h, SNode nh)
			{
				return h == Head && UNSAFE.compareAndSwapObject(this, HeadOffset, h, nh);
			}

			/// <summary>
			/// Creates or resets fields of a node. Called only from transfer
			/// where the node to push on stack is lazily created and
			/// reused when possible to help reduce intervals between reads
			/// and CASes of head and to avoid surges of garbage when CASes
			/// to push nodes fail due to contention.
			/// </summary>
			internal static SNode Snode(SNode s, Object e, SNode next, int mode)
			{
				if (s == null)
				{
					s = new SNode(e);
				}
				s.Mode = mode;
				s.Next = next;
				return s;
			}

			/// <summary>
			/// Puts or takes an item.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E transfer(E e, boolean timed, long nanos)
			internal E Transfer(E e, bool timed, long nanos)
			{
				/*
				 * Basic algorithm is to loop trying one of three actions:
				 *
				 * 1. If apparently empty or already containing nodes of same
				 *    mode, try to push node on stack and wait for a match,
				 *    returning it, or null if cancelled.
				 *
				 * 2. If apparently containing node of complementary mode,
				 *    try to push a fulfilling node on to stack, match
				 *    with corresponding waiting node, pop both from
				 *    stack, and return matched item. The matching or
				 *    unlinking might not actually be necessary because of
				 *    other threads performing action 3:
				 *
				 * 3. If top of stack already holds another fulfilling node,
				 *    help it out by doing its match and/or pop
				 *    operations, and then continue. The code for helping
				 *    is essentially the same as for fulfilling, except
				 *    that it doesn't return the item.
				 */

				SNode s = null; // constructed/reused as needed
				int mode = (e == null) ? REQUEST : DATA;

				for (;;)
				{
					SNode h = Head;
					if (h == null || h.Mode == mode) // empty or same-mode
					{
						if (timed && nanos <= 0) // can't wait
						{
							if (h != null && h.Cancelled)
							{
								CasHead(h, h.Next); // pop cancelled node
							}
							else
							{
								return null;
							}
						}
						else if (CasHead(h, s = Snode(s, e, h, mode)))
						{
							SNode m = AwaitFulfill(s, timed, nanos);
							if (m == s) // wait was cancelled
							{
								Clean(s);
								return null;
							}
							if ((h = Head) != null && h.Next == s)
							{
								CasHead(h, s.Next); // help s's fulfiller
							}
							return (E)((mode == REQUEST) ? m.Item : s.Item);
						}
					} // try to fulfill
					else if (!IsFulfilling(h.Mode))
					{
						if (h.Cancelled) // already cancelled
						{
							CasHead(h, h.Next); // pop and retry
						}
						else if (CasHead(h, s = Snode(s, e, h, FULFILLING | mode)))
						{
							for (;;) // loop until matched or waiters disappear
							{
								SNode m = s.Next; // m is s's match
								if (m == null) // all waiters are gone
								{
									CasHead(s, null); // pop fulfill node
									s = null; // use new node next time
									break; // restart main loop
								}
								SNode mn = m.Next;
								if (m.TryMatch(s))
								{
									CasHead(s, mn); // pop both s and m
									return (E)((mode == REQUEST) ? m.Item : s.Item);
								} // lost match
								else
								{
									s.CasNext(m, mn); // help unlink
								}
							}
						}
					} // help a fulfiller
					else
					{
						SNode m = h.Next; // m is h's match
						if (m == null) // waiter is gone
						{
							CasHead(h, null); // pop fulfilling node
						}
						else
						{
							SNode mn = m.Next;
							if (m.TryMatch(h)) // help match
							{
								CasHead(h, mn); // pop both h and m
							}
							else // lost match
							{
								h.CasNext(m, mn); // help unlink
							}
						}
					}
				}
			}

			/// <summary>
			/// Spins/blocks until node s is matched by a fulfill operation.
			/// </summary>
			/// <param name="s"> the waiting node </param>
			/// <param name="timed"> true if timed wait </param>
			/// <param name="nanos"> timeout value </param>
			/// <returns> matched node, or s if cancelled </returns>
			internal SNode AwaitFulfill(SNode s, bool timed, long nanos)
			{
				/*
				 * When a node/thread is about to block, it sets its waiter
				 * field and then rechecks state at least one more time
				 * before actually parking, thus covering race vs
				 * fulfiller noticing that waiter is non-null so should be
				 * woken.
				 *
				 * When invoked by nodes that appear at the point of call
				 * to be at the head of the stack, calls to park are
				 * preceded by spins to avoid blocking when producers and
				 * consumers are arriving very close in time.  This can
				 * happen enough to bother only on multiprocessors.
				 *
				 * The order of checks for returning out of main loop
				 * reflects fact that interrupts have precedence over
				 * normal returns, which have precedence over
				 * timeouts. (So, on timeout, one last check for match is
				 * done before giving up.) Except that calls from untimed
				 * SynchronousQueue.{poll/offer} don't check interrupts
				 * and don't wait at all, so are trapped in transfer
				 * method rather than calling awaitFulfill.
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = timed ? System.nanoTime() + nanos : 0L;
				long deadline = timed ? System.nanoTime() + nanos : 0L;
				Thread w = Thread.CurrentThread;
				int spins = (ShouldSpin(s) ? (timed ? MaxTimedSpins : MaxUntimedSpins) : 0);
				for (;;)
				{
					if (w.Interrupted)
					{
						s.TryCancel();
					}
					SNode m = s.Match;
					if (m != null)
					{
						return m;
					}
					if (timed)
					{
						nanos = deadline - System.nanoTime();
						if (nanos <= 0L)
						{
							s.TryCancel();
							continue;
						}
					}
					if (spins > 0)
					{
						spins = ShouldSpin(s) ? (spins - 1) : 0;
					}
					else if (s.Waiter == null)
					{
						s.Waiter = w; // establish waiter so can park next iter
					}
					else if (!timed)
					{
						LockSupport.Park(this);
					}
					else if (nanos > SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanos);
					}
				}
			}

			/// <summary>
			/// Returns true if node s is at head or there is an active
			/// fulfiller.
			/// </summary>
			internal bool ShouldSpin(SNode s)
			{
				SNode h = Head;
				return (h == s || h == null || IsFulfilling(h.Mode));
			}

			/// <summary>
			/// Unlinks s from the stack.
			/// </summary>
			internal void Clean(SNode s)
			{
				s.Item = null; // forget item
				s.Waiter = null; // forget thread

				/*
				 * At worst we may need to traverse entire stack to unlink
				 * s. If there are multiple concurrent calls to clean, we
				 * might not see s if another thread has already removed
				 * it. But we can stop when we see any node known to
				 * follow s. We use s.next unless it too is cancelled, in
				 * which case we try the node one past. We don't check any
				 * further because we don't want to doubly traverse just to
				 * find sentinel.
				 */

				SNode past = s.Next;
				if (past != null && past.Cancelled)
				{
					past = past.Next;
				}

				// Absorb cancelled nodes at head
				SNode p;
				while ((p = Head) != null && p != past && p.Cancelled)
				{
					CasHead(p, p.Next);
				}

				// Unsplice embedded nodes
				while (p != null && p != past)
				{
					SNode n = p.Next;
					if (n != null && n.Cancelled)
					{
						p.CasNext(n, n.Next);
					}
					else
					{
						p = n;
					}
				}
			}

			// Unsafe mechanics
			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long HeadOffset;
			static TransferStack()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class k = typeof(TransferStack);
					HeadOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("head"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/// <summary>
		/// Dual Queue </summary>
		internal sealed class TransferQueue<E> : Transferer<E>
		{
			/*
			 * This extends Scherer-Scott dual queue algorithm, differing,
			 * among other ways, by using modes within nodes rather than
			 * marked pointers. The algorithm is a little simpler than
			 * that for stacks because fulfillers do not need explicit
			 * nodes, and matching is done by CAS'ing QNode.item field
			 * from non-null to null (for put) or vice versa (for take).
			 */

			/// <summary>
			/// Node class for TransferQueue. </summary>
			internal sealed class QNode
			{
				internal volatile QNode Next; // next node in queue
				internal volatile Object Item; // CAS'ed to or from null
				internal volatile Thread Waiter; // to control park/unpark
				internal readonly bool IsData;

				internal QNode(Object item, bool isData)
				{
					this.Item = item;
					this.IsData = isData;
				}

				internal bool CasNext(QNode cmp, QNode val)
				{
					return Next == cmp && UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
				}

				internal bool CasItem(Object cmp, Object val)
				{
					return Item == cmp && UNSAFE.compareAndSwapObject(this, ItemOffset, cmp, val);
				}

				/// <summary>
				/// Tries to cancel by CAS'ing ref to this as item.
				/// </summary>
				internal void TryCancel(Object cmp)
				{
					UNSAFE.compareAndSwapObject(this, ItemOffset, cmp, this);
				}

				internal bool Cancelled
				{
					get
					{
						return Item == this;
					}
				}

				/// <summary>
				/// Returns true if this node is known to be off the queue
				/// because its next pointer has been forgotten due to
				/// an advanceHead operation.
				/// </summary>
				internal bool OffList
				{
					get
					{
						return Next == this;
					}
				}

				// Unsafe mechanics
				internal static readonly sun.misc.Unsafe UNSAFE;
				internal static readonly long ItemOffset;
				internal static readonly long NextOffset;

				static QNode()
				{
					try
					{
						UNSAFE = sun.misc.Unsafe.Unsafe;
						Class k = typeof(QNode);
						ItemOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("item"));
						NextOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("next"));
					}
					catch (Exception e)
					{
						throw new Error(e);
					}
				}
			}

			/// <summary>
			/// Head of queue </summary>
			[NonSerialized]
			internal volatile QNode Head;
			/// <summary>
			/// Tail of queue </summary>
			[NonSerialized]
			internal volatile QNode Tail;
			/// <summary>
			/// Reference to a cancelled node that might not yet have been
			/// unlinked from queue because it was the last inserted node
			/// when it was cancelled.
			/// </summary>
			[NonSerialized]
			internal volatile QNode CleanMe;

			internal TransferQueue()
			{
				QNode h = new QNode(null, false); // initialize to dummy node.
				Head = h;
				Tail = h;
			}

			/// <summary>
			/// Tries to cas nh as new head; if successful, unlink
			/// old head's next node to avoid garbage retention.
			/// </summary>
			internal void AdvanceHead(QNode h, QNode nh)
			{
				if (h == Head && UNSAFE.compareAndSwapObject(this, HeadOffset, h, nh))
				{
					h.Next = h; // forget old next
				}
			}

			/// <summary>
			/// Tries to cas nt as new tail.
			/// </summary>
			internal void AdvanceTail(QNode t, QNode nt)
			{
				if (Tail == t)
				{
					UNSAFE.compareAndSwapObject(this, TailOffset, t, nt);
				}
			}

			/// <summary>
			/// Tries to CAS cleanMe slot.
			/// </summary>
			internal bool CasCleanMe(QNode cmp, QNode val)
			{
				return CleanMe == cmp && UNSAFE.compareAndSwapObject(this, CleanMeOffset, cmp, val);
			}

			/// <summary>
			/// Puts or takes an item.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E transfer(E e, boolean timed, long nanos)
			internal E Transfer(E e, bool timed, long nanos)
			{
				/* Basic algorithm is to loop trying to take either of
				 * two actions:
				 *
				 * 1. If queue apparently empty or holding same-mode nodes,
				 *    try to add node to queue of waiters, wait to be
				 *    fulfilled (or cancelled) and return matching item.
				 *
				 * 2. If queue apparently contains waiting items, and this
				 *    call is of complementary mode, try to fulfill by CAS'ing
				 *    item field of waiting node and dequeuing it, and then
				 *    returning matching item.
				 *
				 * In each case, along the way, check for and try to help
				 * advance head and tail on behalf of other stalled/slow
				 * threads.
				 *
				 * The loop starts off with a null check guarding against
				 * seeing uninitialized head or tail values. This never
				 * happens in current SynchronousQueue, but could if
				 * callers held non-volatile/final ref to the
				 * transferer. The check is here anyway because it places
				 * null checks at top of loop, which is usually faster
				 * than having them implicitly interspersed.
				 */

				QNode s = null; // constructed/reused as needed
				bool isData = (e != null);

				for (;;)
				{
					QNode t = Tail;
					QNode h = Head;
					if (t == null || h == null) // saw uninitialized value
					{
						continue; // spin
					}

					if (h == t || t.IsData == isData) // empty or same-mode
					{
						QNode tn = t.Next;
						if (t != Tail) // inconsistent read
						{
							continue;
						}
						if (tn != null) // lagging tail
						{
							AdvanceTail(t, tn);
							continue;
						}
						if (timed && nanos <= 0) // can't wait
						{
							return null;
						}
						if (s == null)
						{
							s = new QNode(e, isData);
						}
						if (!t.CasNext(null, s)) // failed to link in
						{
							continue;
						}

						AdvanceTail(t, s); // swing tail and wait
						Object x = AwaitFulfill(s, e, timed, nanos);
						if (x == s) // wait was cancelled
						{
							Clean(t, s);
							return null;
						}

						if (!s.OffList) // not already unlinked
						{
							AdvanceHead(t, s); // unlink if head
							if (x != null) // and forget fields
							{
								s.Item = s;
							}
							s.Waiter = null;
						}
						return (x != null) ? (E)x : e;

					} // complementary-mode
					else
					{
						QNode m = h.Next; // node to fulfill
						if (t != Tail || m == null || h != Head)
						{
							continue; // inconsistent read
						}

						Object x = m.Item;
						if (isData == (x != null) || x == m || !m.CasItem(x, e)) // lost CAS -  m cancelled -  m already fulfilled
						{
							AdvanceHead(h, m); // dequeue and retry
							continue;
						}

						AdvanceHead(h, m); // successfully fulfilled
						LockSupport.Unpark(m.Waiter);
						return (x != null) ? (E)x : e;
					}
				}
			}

			/// <summary>
			/// Spins/blocks until node s is fulfilled.
			/// </summary>
			/// <param name="s"> the waiting node </param>
			/// <param name="e"> the comparison value for checking match </param>
			/// <param name="timed"> true if timed wait </param>
			/// <param name="nanos"> timeout value </param>
			/// <returns> matched item, or s if cancelled </returns>
			internal Object AwaitFulfill(QNode s, E e, bool timed, long nanos)
			{
				/* Same idea as TransferStack.awaitFulfill */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = timed ? System.nanoTime() + nanos : 0L;
				long deadline = timed ? System.nanoTime() + nanos : 0L;
				Thread w = Thread.CurrentThread;
				int spins = ((Head.Next == s) ? (timed ? MaxTimedSpins : MaxUntimedSpins) : 0);
				for (;;)
				{
					if (w.Interrupted)
					{
						s.TryCancel(e);
					}
					Object x = s.Item;
					if (x != e)
					{
						return x;
					}
					if (timed)
					{
						nanos = deadline - System.nanoTime();
						if (nanos <= 0L)
						{
							s.TryCancel(e);
							continue;
						}
					}
					if (spins > 0)
					{
						--spins;
					}
					else if (s.Waiter == null)
					{
						s.Waiter = w;
					}
					else if (!timed)
					{
						LockSupport.Park(this);
					}
					else if (nanos > SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanos);
					}
				}
			}

			/// <summary>
			/// Gets rid of cancelled node s with original predecessor pred.
			/// </summary>
			internal void Clean(QNode pred, QNode s)
			{
				s.Waiter = null; // forget thread
				/*
				 * At any given time, exactly one node on list cannot be
				 * deleted -- the last inserted node. To accommodate this,
				 * if we cannot delete s, we save its predecessor as
				 * "cleanMe", deleting the previously saved version
				 * first. At least one of node s or the node previously
				 * saved can always be deleted, so this always terminates.
				 */
				while (pred.Next == s) // Return early if already unlinked
				{
					QNode h = Head;
					QNode hn = h.Next; // Absorb cancelled first node as head
					if (hn != null && hn.Cancelled)
					{
						AdvanceHead(h, hn);
						continue;
					}
					QNode t = Tail; // Ensure consistent read for tail
					if (t == h)
					{
						return;
					}
					QNode tn = t.Next;
					if (t != Tail)
					{
						continue;
					}
					if (tn != null)
					{
						AdvanceTail(t, tn);
						continue;
					}
					if (s != t) // If not tail, try to unsplice
					{
						QNode sn = s.Next;
						if (sn == s || pred.CasNext(s, sn))
						{
							return;
						}
					}
					QNode dp = CleanMe;
					if (dp != null) // Try unlinking previous cancelled node
					{
						QNode d = dp.Next;
						QNode dn;
						if (d == null || d == dp || !d.Cancelled || (d != t && (dn = d.Next) != null && dn != d && dp.CasNext(d, dn))) // d unspliced -    that is on list -    has successor -  d not tail and -  d not cancelled or -  d is off list or -  d is gone or
						{
							CasCleanMe(dp, null);
						}
						if (dp == pred)
						{
							return; // s is already saved node
						}
					}
					else if (CasCleanMe(null, pred))
					{
						return; // Postpone cleaning s
					}
				}
			}

			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long HeadOffset;
			internal static readonly long TailOffset;
			internal static readonly long CleanMeOffset;
			static TransferQueue()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class k = typeof(TransferQueue);
					HeadOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("head"));
					TailOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("tail"));
					CleanMeOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("cleanMe"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/// <summary>
		/// The transferer. Set only in constructor, but cannot be declared
		/// as final without further complicating serialization.  Since
		/// this is accessed only at most once per public method, there
		/// isn't a noticeable performance penalty for using volatile
		/// instead of final here.
		/// </summary>
		[NonSerialized]
		private volatile Transferer<E> Transferer;

		/// <summary>
		/// Creates a {@code SynchronousQueue} with nonfair access policy.
		/// </summary>
		public SynchronousQueue() : this(false)
		{
		}

		/// <summary>
		/// Creates a {@code SynchronousQueue} with the specified fairness policy.
		/// </summary>
		/// <param name="fair"> if true, waiting threads contend in FIFO order for
		///        access; otherwise the order is unspecified. </param>
		public SynchronousQueue(bool fair)
		{
			Transferer = fair ? new TransferQueue<E>() : new TransferStack<E>();
		}

		/// <summary>
		/// Adds the specified element to this queue, waiting if necessary for
		/// another thread to receive it.
		/// </summary>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void put(E e) throws InterruptedException
		public virtual void Put(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			if (Transferer.Transfer(e, false, 0) == null)
			{
				Thread.Interrupted();
				throw new InterruptedException();
			}
		}

		/// <summary>
		/// Inserts the specified element into this queue, waiting if necessary
		/// up to the specified wait time for another thread to receive it.
		/// </summary>
		/// <returns> {@code true} if successful, or {@code false} if the
		///         specified waiting time elapses before a consumer appears </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean offer(E e, long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			if (Transferer.Transfer(e, true, unit.ToNanos(timeout)) != null)
			{
				return true;
			}
			if (!Thread.Interrupted())
			{
				return false;
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Inserts the specified element into this queue, if another thread is
		/// waiting to receive it.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} if the element was added to this queue, else
		///         {@code false} </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			return Transferer.Transfer(e, true, 0) != null;
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, waiting if necessary
		/// for another thread to insert it.
		/// </summary>
		/// <returns> the head of this queue </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E take() throws InterruptedException
		public virtual E Take()
		{
			E e = Transferer.Transfer(null, false, 0);
			if (e != null)
			{
				return e;
			}
			Thread.Interrupted();
			throw new InterruptedException();
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, waiting
		/// if necessary up to the specified wait time, for another thread
		/// to insert it.
		/// </summary>
		/// <returns> the head of this queue, or {@code null} if the
		///         specified waiting time elapses before an element is present </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E poll(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E Poll(long timeout, TimeUnit unit)
		{
			E e = Transferer.Transfer(null, true, unit.ToNanos(timeout));
			if (e != null || !Thread.Interrupted())
			{
				return e;
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, if another thread
		/// is currently making an element available.
		/// </summary>
		/// <returns> the head of this queue, or {@code null} if no
		///         element is available </returns>
		public virtual E Poll()
		{
			return Transferer.Transfer(null, true, 0);
		}

		/// <summary>
		/// Always returns {@code true}.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <returns> {@code true} </returns>
		public virtual bool Empty
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Always returns zero.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <returns> zero </returns>
		public virtual int Size()
		{
			return 0;
		}

		/// <summary>
		/// Always returns zero.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <returns> zero </returns>
		public virtual int RemainingCapacity()
		{
			return 0;
		}

		/// <summary>
		/// Does nothing.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		public virtual void Clear()
		{
		}

		/// <summary>
		/// Always returns {@code false}.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <param name="o"> the element </param>
		/// <returns> {@code false} </returns>
		public virtual bool Contains(Object o)
		{
			return false;
		}

		/// <summary>
		/// Always returns {@code false}.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <param name="o"> the element to remove </param>
		/// <returns> {@code false} </returns>
		public virtual bool Remove(Object o)
		{
			return false;
		}

		/// <summary>
		/// Returns {@code false} unless the given collection is empty.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <param name="c"> the collection </param>
		/// <returns> {@code false} unless given collection is empty </returns>
		public virtual bool containsAll<T1>(Collection<T1> c)
		{
			return c.Empty;
		}

		/// <summary>
		/// Always returns {@code false}.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <param name="c"> the collection </param>
		/// <returns> {@code false} </returns>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			return false;
		}

		/// <summary>
		/// Always returns {@code false}.
		/// A {@code SynchronousQueue} has no internal capacity.
		/// </summary>
		/// <param name="c"> the collection </param>
		/// <returns> {@code false} </returns>
		public virtual bool retainAll<T1>(Collection<T1> c)
		{
			return false;
		}

		/// <summary>
		/// Always returns {@code null}.
		/// A {@code SynchronousQueue} does not return elements
		/// unless actively waited on.
		/// </summary>
		/// <returns> {@code null} </returns>
		public virtual E Peek()
		{
			return null;
		}

		/// <summary>
		/// Returns an empty iterator in which {@code hasNext} always returns
		/// {@code false}.
		/// </summary>
		/// <returns> an empty iterator </returns>
		public virtual Iterator<E> Iterator()
		{
			return Collections.EmptyIterator();
		}

		/// <summary>
		/// Returns an empty spliterator in which calls to
		/// <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		/// </summary>
		/// <returns> an empty spliterator
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return Spliterators.EmptySpliterator();
		}

		/// <summary>
		/// Returns a zero-length array. </summary>
		/// <returns> a zero-length array </returns>
		public virtual Object[] ToArray()
		{
			return new Object[0];
		}

		/// <summary>
		/// Sets the zeroeth element of the specified array to {@code null}
		/// (if the array has non-zero length) and returns it.
		/// </summary>
		/// <param name="a"> the array </param>
		/// <returns> the specified array </returns>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		public virtual T[] toArray<T>(T[] a)
		{
			if (a.Length > 0)
			{
				a[0] = null;
			}
			return a;
		}

		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(Collection<? base E> c)
		public virtual int drainTo<T1>(Collection<T1> c)
		{
			if (c == null)
			{
				throw new NullPointerException();
			}
			if (c == this)
			{
				throw new IllegalArgumentException();
			}
			int n = 0;
			for (E e; (e = Poll()) != null;)
			{
				c.Add(e);
				++n;
			}
			return n;
		}

		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(Collection<? base E> c, int maxElements)
		public virtual int drainTo<T1>(Collection<T1> c, int maxElements)
		{
			if (c == null)
			{
				throw new NullPointerException();
			}
			if (c == this)
			{
				throw new IllegalArgumentException();
			}
			int n = 0;
			for (E e; n < maxElements && (e = Poll()) != null;)
			{
				c.Add(e);
				++n;
			}
			return n;
		}

		/*
		 * To cope with serialization strategy in the 1.5 version of
		 * SynchronousQueue, we declare some unused classes and fields
		 * that exist solely to enable serializability across versions.
		 * These fields are never used, so are initialized only if this
		 * object is ever serialized or deserialized.
		 */

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static class WaitQueue implements java.io.Serializable
		[Serializable]
		internal class WaitQueue
		{
		}
		internal class LifoWaitQueue : WaitQueue
		{
			internal const long SerialVersionUID = -3633113410248163686L;
		}
		internal class FifoWaitQueue : WaitQueue
		{
			internal const long SerialVersionUID = -3623113410248163686L;
		}
		private ReentrantLock Qlock;
		private WaitQueue WaitingProducers;
		private WaitQueue WaitingConsumers;

		/// <summary>
		/// Saves this queue to a stream (that is, serializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			bool fair = Transferer is TransferQueue;
			if (fair)
			{
				Qlock = new ReentrantLock(true);
				WaitingProducers = new FifoWaitQueue();
				WaitingConsumers = new FifoWaitQueue();
			}
			else
			{
				Qlock = new ReentrantLock();
				WaitingProducers = new LifoWaitQueue();
				WaitingConsumers = new LifoWaitQueue();
			}
			s.DefaultWriteObject();
		}

		/// <summary>
		/// Reconstitutes this queue from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			if (WaitingProducers is FifoWaitQueue)
			{
				Transferer = new TransferQueue<E>();
			}
			else
			{
				Transferer = new TransferStack<E>();
			}
		}

		// Unsafe mechanics
		internal static long ObjectFieldOffset(sun.misc.Unsafe UNSAFE, String field, Class klazz)
		{
			try
			{
				return UNSAFE.objectFieldOffset(klazz.GetDeclaredField(field));
			}
			catch (NoSuchFieldException e)
			{
				// Convert Exception to corresponding Error
				NoSuchFieldError error = new NoSuchFieldError(field);
				error.InitCause(e);
				throw error;
			}
		}

	}

}