using System;
using System.Collections.Generic;
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

namespace java.util.concurrent.locks
{
	using Unsafe = sun.misc.Unsafe;

	/// <summary>
	/// A version of <seealso cref="AbstractQueuedSynchronizer"/> in
	/// which synchronization state is maintained as a {@code long}.
	/// This class has exactly the same structure, properties, and methods
	/// as {@code AbstractQueuedSynchronizer} with the exception
	/// that all state-related parameters and results are defined
	/// as {@code long} rather than {@code int}. This class
	/// may be useful when creating synchronizers such as
	/// multilevel locks and barriers that require
	/// 64 bits of state.
	/// 
	/// <para>See <seealso cref="AbstractQueuedSynchronizer"/> for usage
	/// notes and examples.
	/// 
	/// @since 1.6
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public abstract class AbstractQueuedLongSynchronizer : AbstractOwnableSynchronizer
	{

		private const long SerialVersionUID = 7373984972572414692L;

		/*
		  To keep sources in sync, the remainder of this source file is
		  exactly cloned from AbstractQueuedSynchronizer, replacing class
		  name and changing ints related with sync state to longs. Please
		  keep it that way.
		*/

		/// <summary>
		/// Creates a new {@code AbstractQueuedLongSynchronizer} instance
		/// with initial synchronization state of zero.
		/// </summary>
		protected internal AbstractQueuedLongSynchronizer()
		{
		}

		/// <summary>
		/// Wait queue node class.
		/// 
		/// <para>The wait queue is a variant of a "CLH" (Craig, Landin, and
		/// Hagersten) lock queue. CLH locks are normally used for
		/// spinlocks.  We instead use them for blocking synchronizers, but
		/// use the same basic tactic of holding some of the control
		/// information about a thread in the predecessor of its node.  A
		/// "status" field in each node keeps track of whether a thread
		/// should block.  A node is signalled when its predecessor
		/// releases.  Each node of the queue otherwise serves as a
		/// specific-notification-style monitor holding a single waiting
		/// thread. The status field does NOT control whether threads are
		/// granted locks etc though.  A thread may try to acquire if it is
		/// first in the queue. But being first does not guarantee success;
		/// it only gives the right to contend.  So the currently released
		/// contender thread may need to rewait.
		/// 
		/// </para>
		/// <para>To enqueue into a CLH lock, you atomically splice it in as new
		/// tail. To dequeue, you just set the head field.
		/// <pre>
		///      +------+  prev +-----+       +-----+
		/// head |      | <---- |     | <---- |     |  tail
		///      +------+       +-----+       +-----+
		/// </pre>
		/// 
		/// </para>
		/// <para>Insertion into a CLH queue requires only a single atomic
		/// operation on "tail", so there is a simple atomic point of
		/// demarcation from unqueued to queued. Similarly, dequeuing
		/// involves only updating the "head". However, it takes a bit
		/// more work for nodes to determine who their successors are,
		/// in part to deal with possible cancellation due to timeouts
		/// and interrupts.
		/// 
		/// </para>
		/// <para>The "prev" links (not used in original CLH locks), are mainly
		/// needed to handle cancellation. If a node is cancelled, its
		/// successor is (normally) relinked to a non-cancelled
		/// predecessor. For explanation of similar mechanics in the case
		/// of spin locks, see the papers by Scott and Scherer at
		/// http://www.cs.rochester.edu/u/scott/synchronization/
		/// 
		/// </para>
		/// <para>We also use "next" links to implement blocking mechanics.
		/// The thread id for each node is kept in its own node, so a
		/// predecessor signals the next node to wake up by traversing
		/// next link to determine which thread it is.  Determination of
		/// successor must avoid races with newly queued nodes to set
		/// the "next" fields of their predecessors.  This is solved
		/// when necessary by checking backwards from the atomically
		/// updated "tail" when a node's successor appears to be null.
		/// (Or, said differently, the next-links are an optimization
		/// so that we don't usually need a backward scan.)
		/// 
		/// </para>
		/// <para>Cancellation introduces some conservatism to the basic
		/// algorithms.  Since we must poll for cancellation of other
		/// nodes, we can miss noticing whether a cancelled node is
		/// ahead or behind us. This is dealt with by always unparking
		/// successors upon cancellation, allowing them to stabilize on
		/// a new predecessor, unless we can identify an uncancelled
		/// predecessor who will carry this responsibility.
		/// 
		/// </para>
		/// <para>CLH queues need a dummy header node to get started. But
		/// we don't create them on construction, because it would be wasted
		/// effort if there is never contention. Instead, the node
		/// is constructed and head and tail pointers are set upon first
		/// contention.
		/// 
		/// </para>
		/// <para>Threads waiting on Conditions use the same nodes, but
		/// use an additional link. Conditions only need to link nodes
		/// in simple (non-concurrent) linked queues because they are
		/// only accessed when exclusively held.  Upon await, a node is
		/// inserted into a condition queue.  Upon signal, the node is
		/// transferred to the main queue.  A special value of status
		/// field is used to mark which queue a node is on.
		/// 
		/// </para>
		/// <para>Thanks go to Dave Dice, Mark Moir, Victor Luchangco, Bill
		/// Scherer and Michael Scott, along with members of JSR-166
		/// expert group, for helpful ideas, discussions, and critiques
		/// on the design of this class.
		/// </para>
		/// </summary>
		internal sealed class Node
		{
			/// <summary>
			/// Marker to indicate a node is waiting in shared mode </summary>
			internal static readonly Node SHARED = new Node();
			/// <summary>
			/// Marker to indicate a node is waiting in exclusive mode </summary>
			internal const Node EXCLUSIVE = null;

			/// <summary>
			/// waitStatus value to indicate thread has cancelled </summary>
			internal const int CANCELLED = 1;
			/// <summary>
			/// waitStatus value to indicate successor's thread needs unparking </summary>
			internal const int SIGNAL = -1;
			/// <summary>
			/// waitStatus value to indicate thread is waiting on condition </summary>
			internal const int CONDITION = -2;
			/// <summary>
			/// waitStatus value to indicate the next acquireShared should
			/// unconditionally propagate
			/// </summary>
			internal const int PROPAGATE = -3;

			/// <summary>
			/// Status field, taking on only the values:
			///   SIGNAL:     The successor of this node is (or will soon be)
			///               blocked (via park), so the current node must
			///               unpark its successor when it releases or
			///               cancels. To avoid races, acquire methods must
			///               first indicate they need a signal,
			///               then retry the atomic acquire, and then,
			///               on failure, block.
			///   CANCELLED:  This node is cancelled due to timeout or interrupt.
			///               Nodes never leave this state. In particular,
			///               a thread with cancelled node never again blocks.
			///   CONDITION:  This node is currently on a condition queue.
			///               It will not be used as a sync queue node
			///               until transferred, at which time the status
			///               will be set to 0. (Use of this value here has
			///               nothing to do with the other uses of the
			///               field, but simplifies mechanics.)
			///   PROPAGATE:  A releaseShared should be propagated to other
			///               nodes. This is set (for head node only) in
			///               doReleaseShared to ensure propagation
			///               continues, even if other operations have
			///               since intervened.
			///   0:          None of the above
			/// 
			/// The values are arranged numerically to simplify use.
			/// Non-negative values mean that a node doesn't need to
			/// signal. So, most code doesn't need to check for particular
			/// values, just for sign.
			/// 
			/// The field is initialized to 0 for normal sync nodes, and
			/// CONDITION for condition nodes.  It is modified using CAS
			/// (or when possible, unconditional volatile writes).
			/// </summary>
			internal volatile int WaitStatus;

			/// <summary>
			/// Link to predecessor node that current node/thread relies on
			/// for checking waitStatus. Assigned during enqueuing, and nulled
			/// out (for sake of GC) only upon dequeuing.  Also, upon
			/// cancellation of a predecessor, we short-circuit while
			/// finding a non-cancelled one, which will always exist
			/// because the head node is never cancelled: A node becomes
			/// head only as a result of successful acquire. A
			/// cancelled thread never succeeds in acquiring, and a thread only
			/// cancels itself, not any other node.
			/// </summary>
			internal volatile Node Prev;

			/// <summary>
			/// Link to the successor node that the current node/thread
			/// unparks upon release. Assigned during enqueuing, adjusted
			/// when bypassing cancelled predecessors, and nulled out (for
			/// sake of GC) when dequeued.  The enq operation does not
			/// assign next field of a predecessor until after attachment,
			/// so seeing a null next field does not necessarily mean that
			/// node is at end of queue. However, if a next field appears
			/// to be null, we can scan prev's from the tail to
			/// double-check.  The next field of cancelled nodes is set to
			/// point to the node itself instead of null, to make life
			/// easier for isOnSyncQueue.
			/// </summary>
			internal volatile Node Next;

			/// <summary>
			/// The thread that enqueued this node.  Initialized on
			/// construction and nulled out after use.
			/// </summary>
			internal volatile Thread Thread;

			/// <summary>
			/// Link to next node waiting on condition, or the special
			/// value SHARED.  Because condition queues are accessed only
			/// when holding in exclusive mode, we just need a simple
			/// linked queue to hold nodes while they are waiting on
			/// conditions. They are then transferred to the queue to
			/// re-acquire. And because conditions can only be exclusive,
			/// we save a field by using special value to indicate shared
			/// mode.
			/// </summary>
			internal Node NextWaiter;

			/// <summary>
			/// Returns true if node is waiting in shared mode.
			/// </summary>
			internal bool Shared
			{
				get
				{
					return NextWaiter == SHARED;
				}
			}

			/// <summary>
			/// Returns previous node, or throws NullPointerException if null.
			/// Use when predecessor cannot be null.  The null check could
			/// be elided, but is present to help the VM.
			/// </summary>
			/// <returns> the predecessor of this node </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: final Node predecessor() throws NullPointerException
			internal Node Predecessor()
			{
				Node p = Prev;
				if (p == null)
				{
					throw new NullPointerException();
				}
				else
				{
					return p;
				}
			}

			internal Node() // Used to establish initial head or SHARED marker
			{
			}

			internal Node(Thread thread, Node mode) // Used by addWaiter
			{
				this.NextWaiter = mode;
				this.Thread = thread;
			}

			internal Node(Thread thread, int waitStatus) // Used by Condition
			{
				this.WaitStatus = waitStatus;
				this.Thread = thread;
			}
		}

		/// <summary>
		/// Head of the wait queue, lazily initialized.  Except for
		/// initialization, it is modified only via method setHead.  Note:
		/// If head exists, its waitStatus is guaranteed not to be
		/// CANCELLED.
		/// </summary>
		[NonSerialized]
		private volatile Node Head_Renamed;

		/// <summary>
		/// Tail of the wait queue, lazily initialized.  Modified only via
		/// method enq to add new wait node.
		/// </summary>
		[NonSerialized]
		private volatile Node Tail;

		/// <summary>
		/// The synchronization state.
		/// </summary>
		private volatile long State_Renamed;

		/// <summary>
		/// Returns the current value of synchronization state.
		/// This operation has memory semantics of a {@code volatile} read. </summary>
		/// <returns> current state value </returns>
		protected internal long State
		{
			get
			{
				return State_Renamed;
			}
			set
			{
				State_Renamed = value;
			}
		}


		/// <summary>
		/// Atomically sets synchronization state to the given updated
		/// value if the current state value equals the expected value.
		/// This operation has memory semantics of a {@code volatile} read
		/// and write.
		/// </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that the actual
		///         value was not equal to the expected value. </returns>
		protected internal bool CompareAndSetState(long expect, long update)
		{
			// See below for intrinsics setup to support this
			return @unsafe.compareAndSwapLong(this, StateOffset, expect, update);
		}

		// Queuing utilities

		/// <summary>
		/// The number of nanoseconds for which it is faster to spin
		/// rather than to use timed park. A rough estimate suffices
		/// to improve responsiveness with very short timeouts.
		/// </summary>
		internal const long SpinForTimeoutThreshold = 1000L;

		/// <summary>
		/// Inserts node into queue, initializing if necessary. See picture above. </summary>
		/// <param name="node"> the node to insert </param>
		/// <returns> node's predecessor </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Node enq(final Node node)
		private Node Enq(Node node)
		{
			for (;;)
			{
				Node t = Tail;
				if (t == null) // Must initialize
				{
					if (CompareAndSetHead(new Node()))
					{
						Tail = Head_Renamed;
					}
				}
				else
				{
					node.Prev = t;
					if (CompareAndSetTail(t, node))
					{
						t.Next = node;
						return t;
					}
				}
			}
		}

		/// <summary>
		/// Creates and enqueues node for current thread and given mode.
		/// </summary>
		/// <param name="mode"> Node.EXCLUSIVE for exclusive, Node.SHARED for shared </param>
		/// <returns> the new node </returns>
		private Node AddWaiter(Node mode)
		{
			Node node = new Node(Thread.CurrentThread, mode);
			// Try the fast path of enq; backup to full enq on failure
			Node pred = Tail;
			if (pred != null)
			{
				node.Prev = pred;
				if (CompareAndSetTail(pred, node))
				{
					pred.Next = node;
					return node;
				}
			}
			Enq(node);
			return node;
		}

		/// <summary>
		/// Sets head of queue to be node, thus dequeuing. Called only by
		/// acquire methods.  Also nulls out unused fields for sake of GC
		/// and to suppress unnecessary signals and traversals.
		/// </summary>
		/// <param name="node"> the node </param>
		private Node Head
		{
			set
			{
				Head_Renamed = value;
				value.Thread = null;
				value.Prev = null;
			}
		}

		/// <summary>
		/// Wakes up node's successor, if one exists.
		/// </summary>
		/// <param name="node"> the node </param>
		private void UnparkSuccessor(Node node)
		{
			/*
			 * If status is negative (i.e., possibly needing signal) try
			 * to clear in anticipation of signalling.  It is OK if this
			 * fails or if status is changed by waiting thread.
			 */
			int ws = node.WaitStatus;
			if (ws < 0)
			{
				CompareAndSetWaitStatus(node, ws, 0);
			}

			/*
			 * Thread to unpark is held in successor, which is normally
			 * just the next node.  But if cancelled or apparently null,
			 * traverse backwards from tail to find the actual
			 * non-cancelled successor.
			 */
			Node s = node.Next;
			if (s == null || s.WaitStatus > 0)
			{
				s = null;
				for (Node t = Tail; t != null && t != node; t = t.Prev)
				{
					if (t.WaitStatus <= 0)
					{
						s = t;
					}
				}
			}
			if (s != null)
			{
				LockSupport.Unpark(s.Thread);
			}
		}

		/// <summary>
		/// Release action for shared mode -- signals successor and ensures
		/// propagation. (Note: For exclusive mode, release just amounts
		/// to calling unparkSuccessor of head if it needs signal.)
		/// </summary>
		private void DoReleaseShared()
		{
			/*
			 * Ensure that a release propagates, even if there are other
			 * in-progress acquires/releases.  This proceeds in the usual
			 * way of trying to unparkSuccessor of head if it needs
			 * signal. But if it does not, status is set to PROPAGATE to
			 * ensure that upon release, propagation continues.
			 * Additionally, we must loop in case a new node is added
			 * while we are doing this. Also, unlike other uses of
			 * unparkSuccessor, we need to know if CAS to reset status
			 * fails, if so rechecking.
			 */
			for (;;)
			{
				Node h = Head_Renamed;
				if (h != null && h != Tail)
				{
					int ws = h.WaitStatus;
					if (ws == Node.SIGNAL)
					{
						if (!CompareAndSetWaitStatus(h, Node.SIGNAL, 0))
						{
							continue; // loop to recheck cases
						}
						UnparkSuccessor(h);
					}
					else if (ws == 0 && !CompareAndSetWaitStatus(h, 0, Node.PROPAGATE))
					{
						continue; // loop on failed CAS
					}
				}
				if (h == Head_Renamed) // loop if head changed
				{
					break;
				}
			}
		}

		/// <summary>
		/// Sets head of queue, and checks if successor may be waiting
		/// in shared mode, if so propagating if either propagate > 0 or
		/// PROPAGATE status was set.
		/// </summary>
		/// <param name="node"> the node </param>
		/// <param name="propagate"> the return value from a tryAcquireShared </param>
		private void SetHeadAndPropagate(Node node, long propagate)
		{
			Node h = Head_Renamed; // Record old head for check below
			Head = node;
			/*
			 * Try to signal next queued node if:
			 *   Propagation was indicated by caller,
			 *     or was recorded (as h.waitStatus either before
			 *     or after setHead) by a previous operation
			 *     (note: this uses sign-check of waitStatus because
			 *      PROPAGATE status may transition to SIGNAL.)
			 * and
			 *   The next node is waiting in shared mode,
			 *     or we don't know, because it appears null
			 *
			 * The conservatism in both of these checks may cause
			 * unnecessary wake-ups, but only when there are multiple
			 * racing acquires/releases, so most need signals now or soon
			 * anyway.
			 */
			if (propagate > 0 || h == null || h.WaitStatus < 0 || (h = Head_Renamed) == null || h.WaitStatus < 0)
			{
				Node s = node.Next;
				if (s == null || s.Shared)
				{
					DoReleaseShared();
				}
			}
		}

		// Utilities for various versions of acquire

		/// <summary>
		/// Cancels an ongoing attempt to acquire.
		/// </summary>
		/// <param name="node"> the node </param>
		private void CancelAcquire(Node node)
		{
			// Ignore if node doesn't exist
			if (node == null)
			{
				return;
			}

			node.Thread = null;

			// Skip cancelled predecessors
			Node pred = node.Prev;
			while (pred.WaitStatus > 0)
			{
				node.Prev = pred = pred.Prev;
			}

			// predNext is the apparent node to unsplice. CASes below will
			// fail if not, in which case, we lost race vs another cancel
			// or signal, so no further action is necessary.
			Node predNext = pred.Next;

			// Can use unconditional write instead of CAS here.
			// After this atomic step, other Nodes can skip past us.
			// Before, we are free of interference from other threads.
			node.WaitStatus = Node.CANCELLED;

			// If we are the tail, remove ourselves.
			if (node == Tail && CompareAndSetTail(node, pred))
			{
				CompareAndSetNext(pred, predNext, null);
			}
			else
			{
				// If successor needs signal, try to set pred's next-link
				// so it will get one. Otherwise wake it up to propagate.
				int ws;
				if (pred != Head_Renamed && ((ws = pred.WaitStatus) == Node.SIGNAL || (ws <= 0 && CompareAndSetWaitStatus(pred, ws, Node.SIGNAL))) && pred.Thread != null)
				{
					Node next = node.Next;
					if (next != null && next.WaitStatus <= 0)
					{
						CompareAndSetNext(pred, predNext, next);
					}
				}
				else
				{
					UnparkSuccessor(node);
				}

				node.Next = node; // help GC
			}
		}

		/// <summary>
		/// Checks and updates status for a node that failed to acquire.
		/// Returns true if thread should block. This is the main signal
		/// control in all acquire loops.  Requires that pred == node.prev.
		/// </summary>
		/// <param name="pred"> node's predecessor holding status </param>
		/// <param name="node"> the node </param>
		/// <returns> {@code true} if thread should block </returns>
		private static bool ShouldParkAfterFailedAcquire(Node pred, Node node)
		{
			int ws = pred.WaitStatus;
			if (ws == Node.SIGNAL)
				/*
				 * This node has already set status asking a release
				 * to signal it, so it can safely park.
				 */
			{
				return true;
			}
			if (ws > 0)
			{
				/*
				 * Predecessor was cancelled. Skip over predecessors and
				 * indicate retry.
				 */
				do
				{
					node.Prev = pred = pred.Prev;
				} while (pred.WaitStatus > 0);
				pred.Next = node;
			}
			else
			{
				/*
				 * waitStatus must be 0 or PROPAGATE.  Indicate that we
				 * need a signal, but don't park yet.  Caller will need to
				 * retry to make sure it cannot acquire before parking.
				 */
				CompareAndSetWaitStatus(pred, ws, Node.SIGNAL);
			}
			return false;
		}

		/// <summary>
		/// Convenience method to interrupt current thread.
		/// </summary>
		internal static void SelfInterrupt()
		{
			Thread.CurrentThread.Interrupt();
		}

		/// <summary>
		/// Convenience method to park and then check if interrupted
		/// </summary>
		/// <returns> {@code true} if interrupted </returns>
		private bool ParkAndCheckInterrupt()
		{
			LockSupport.Park(this);
			return Thread.Interrupted();
		}

		/*
		 * Various flavors of acquire, varying in exclusive/shared and
		 * control modes.  Each is mostly the same, but annoyingly
		 * different.  Only a little bit of factoring is possible due to
		 * interactions of exception mechanics (including ensuring that we
		 * cancel if tryAcquire throws exception) and other control, at
		 * least not without hurting performance too much.
		 */

		/// <summary>
		/// Acquires in exclusive uninterruptible mode for thread already in
		/// queue. Used by condition wait methods as well as acquire.
		/// </summary>
		/// <param name="node"> the node </param>
		/// <param name="arg"> the acquire argument </param>
		/// <returns> {@code true} if interrupted while waiting </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: final boolean acquireQueued(final Node node, long arg)
		internal bool AcquireQueued(Node node, long arg)
		{
			bool failed = true;
			try
			{
				bool interrupted = false;
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed && TryAcquire(arg))
					{
						Head = node;
						p.Next = null; // help GC
						failed = false;
						return interrupted;
					}
					if (ShouldParkAfterFailedAcquire(p, node) && ParkAndCheckInterrupt())
					{
						interrupted = true;
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		/// <summary>
		/// Acquires in exclusive interruptible mode. </summary>
		/// <param name="arg"> the acquire argument </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void doAcquireInterruptibly(long arg) throws InterruptedException
		private void DoAcquireInterruptibly(long arg)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node node = addWaiter(Node.EXCLUSIVE);
			Node node = AddWaiter(Node.EXCLUSIVE);
			bool failed = true;
			try
			{
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed && TryAcquire(arg))
					{
						Head = node;
						p.Next = null; // help GC
						failed = false;
						return;
					}
					if (ShouldParkAfterFailedAcquire(p, node) && ParkAndCheckInterrupt())
					{
						throw new InterruptedException();
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		/// <summary>
		/// Acquires in exclusive timed mode.
		/// </summary>
		/// <param name="arg"> the acquire argument </param>
		/// <param name="nanosTimeout"> max wait time </param>
		/// <returns> {@code true} if acquired </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean doAcquireNanos(long arg, long nanosTimeout) throws InterruptedException
		private bool DoAcquireNanos(long arg, long nanosTimeout)
		{
			if (nanosTimeout <= 0L)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = System.nanoTime() + nanosTimeout;
			long deadline = System.nanoTime() + nanosTimeout;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node node = addWaiter(Node.EXCLUSIVE);
			Node node = AddWaiter(Node.EXCLUSIVE);
			bool failed = true;
			try
			{
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed && TryAcquire(arg))
					{
						Head = node;
						p.Next = null; // help GC
						failed = false;
						return true;
					}
					nanosTimeout = deadline - System.nanoTime();
					if (nanosTimeout <= 0L)
					{
						return false;
					}
					if (ShouldParkAfterFailedAcquire(p, node) && nanosTimeout > SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanosTimeout);
					}
					if (Thread.Interrupted())
					{
						throw new InterruptedException();
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		/// <summary>
		/// Acquires in shared uninterruptible mode. </summary>
		/// <param name="arg"> the acquire argument </param>
		private void DoAcquireShared(long arg)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node node = addWaiter(Node.SHARED);
			Node node = AddWaiter(Node.SHARED);
			bool failed = true;
			try
			{
				bool interrupted = false;
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed)
					{
						long r = TryAcquireShared(arg);
						if (r >= 0)
						{
							SetHeadAndPropagate(node, r);
							p.Next = null; // help GC
							if (interrupted)
							{
								SelfInterrupt();
							}
							failed = false;
							return;
						}
					}
					if (ShouldParkAfterFailedAcquire(p, node) && ParkAndCheckInterrupt())
					{
						interrupted = true;
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		/// <summary>
		/// Acquires in shared interruptible mode. </summary>
		/// <param name="arg"> the acquire argument </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void doAcquireSharedInterruptibly(long arg) throws InterruptedException
		private void DoAcquireSharedInterruptibly(long arg)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node node = addWaiter(Node.SHARED);
			Node node = AddWaiter(Node.SHARED);
			bool failed = true;
			try
			{
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed)
					{
						long r = TryAcquireShared(arg);
						if (r >= 0)
						{
							SetHeadAndPropagate(node, r);
							p.Next = null; // help GC
							failed = false;
							return;
						}
					}
					if (ShouldParkAfterFailedAcquire(p, node) && ParkAndCheckInterrupt())
					{
						throw new InterruptedException();
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		/// <summary>
		/// Acquires in shared timed mode.
		/// </summary>
		/// <param name="arg"> the acquire argument </param>
		/// <param name="nanosTimeout"> max wait time </param>
		/// <returns> {@code true} if acquired </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean doAcquireSharedNanos(long arg, long nanosTimeout) throws InterruptedException
		private bool DoAcquireSharedNanos(long arg, long nanosTimeout)
		{
			if (nanosTimeout <= 0L)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = System.nanoTime() + nanosTimeout;
			long deadline = System.nanoTime() + nanosTimeout;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node node = addWaiter(Node.SHARED);
			Node node = AddWaiter(Node.SHARED);
			bool failed = true;
			try
			{
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node p = node.predecessor();
					Node p = node.Predecessor();
					if (p == Head_Renamed)
					{
						long r = TryAcquireShared(arg);
						if (r >= 0)
						{
							SetHeadAndPropagate(node, r);
							p.Next = null; // help GC
							failed = false;
							return true;
						}
					}
					nanosTimeout = deadline - System.nanoTime();
					if (nanosTimeout <= 0L)
					{
						return false;
					}
					if (ShouldParkAfterFailedAcquire(p, node) && nanosTimeout > SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanosTimeout);
					}
					if (Thread.Interrupted())
					{
						throw new InterruptedException();
					}
				}
			}
			finally
			{
				if (failed)
				{
					CancelAcquire(node);
				}
			}
		}

		// Main exported methods

		/// <summary>
		/// Attempts to acquire in exclusive mode. This method should query
		/// if the state of the object permits it to be acquired in the
		/// exclusive mode, and if so to acquire it.
		/// 
		/// <para>This method is always invoked by the thread performing
		/// acquire.  If this method reports failure, the acquire method
		/// may queue the thread, if it is not already queued, until it is
		/// signalled by a release from some other thread. This can be used
		/// to implement method <seealso cref="Lock#tryLock()"/>.
		/// 
		/// </para>
		/// <para>The default
		/// implementation throws <seealso cref="UnsupportedOperationException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arg"> the acquire argument. This value is always the one
		///        passed to an acquire method, or is the value saved on entry
		///        to a condition wait.  The value is otherwise uninterpreted
		///        and can represent anything you like. </param>
		/// <returns> {@code true} if successful. Upon success, this object has
		///         been acquired. </returns>
		/// <exception cref="IllegalMonitorStateException"> if acquiring would place this
		///         synchronizer in an illegal state. This exception must be
		///         thrown in a consistent fashion for synchronization to work
		///         correctly. </exception>
		/// <exception cref="UnsupportedOperationException"> if exclusive mode is not supported </exception>
		protected internal virtual bool TryAcquire(long arg)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Attempts to set the state to reflect a release in exclusive
		/// mode.
		/// 
		/// <para>This method is always invoked by the thread performing release.
		/// 
		/// </para>
		/// <para>The default implementation throws
		/// <seealso cref="UnsupportedOperationException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arg"> the release argument. This value is always the one
		///        passed to a release method, or the current state value upon
		///        entry to a condition wait.  The value is otherwise
		///        uninterpreted and can represent anything you like. </param>
		/// <returns> {@code true} if this object is now in a fully released
		///         state, so that any waiting threads may attempt to acquire;
		///         and {@code false} otherwise. </returns>
		/// <exception cref="IllegalMonitorStateException"> if releasing would place this
		///         synchronizer in an illegal state. This exception must be
		///         thrown in a consistent fashion for synchronization to work
		///         correctly. </exception>
		/// <exception cref="UnsupportedOperationException"> if exclusive mode is not supported </exception>
		protected internal virtual bool TryRelease(long arg)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Attempts to acquire in shared mode. This method should query if
		/// the state of the object permits it to be acquired in the shared
		/// mode, and if so to acquire it.
		/// 
		/// <para>This method is always invoked by the thread performing
		/// acquire.  If this method reports failure, the acquire method
		/// may queue the thread, if it is not already queued, until it is
		/// signalled by a release from some other thread.
		/// 
		/// </para>
		/// <para>The default implementation throws {@link
		/// UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arg"> the acquire argument. This value is always the one
		///        passed to an acquire method, or is the value saved on entry
		///        to a condition wait.  The value is otherwise uninterpreted
		///        and can represent anything you like. </param>
		/// <returns> a negative value on failure; zero if acquisition in shared
		///         mode succeeded but no subsequent shared-mode acquire can
		///         succeed; and a positive value if acquisition in shared
		///         mode succeeded and subsequent shared-mode acquires might
		///         also succeed, in which case a subsequent waiting thread
		///         must check availability. (Support for three different
		///         return values enables this method to be used in contexts
		///         where acquires only sometimes act exclusively.)  Upon
		///         success, this object has been acquired. </returns>
		/// <exception cref="IllegalMonitorStateException"> if acquiring would place this
		///         synchronizer in an illegal state. This exception must be
		///         thrown in a consistent fashion for synchronization to work
		///         correctly. </exception>
		/// <exception cref="UnsupportedOperationException"> if shared mode is not supported </exception>
		protected internal virtual long TryAcquireShared(long arg)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Attempts to set the state to reflect a release in shared mode.
		/// 
		/// <para>This method is always invoked by the thread performing release.
		/// 
		/// </para>
		/// <para>The default implementation throws
		/// <seealso cref="UnsupportedOperationException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arg"> the release argument. This value is always the one
		///        passed to a release method, or the current state value upon
		///        entry to a condition wait.  The value is otherwise
		///        uninterpreted and can represent anything you like. </param>
		/// <returns> {@code true} if this release of shared mode may permit a
		///         waiting acquire (shared or exclusive) to succeed; and
		///         {@code false} otherwise </returns>
		/// <exception cref="IllegalMonitorStateException"> if releasing would place this
		///         synchronizer in an illegal state. This exception must be
		///         thrown in a consistent fashion for synchronization to work
		///         correctly. </exception>
		/// <exception cref="UnsupportedOperationException"> if shared mode is not supported </exception>
		protected internal virtual bool TryReleaseShared(long arg)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Returns {@code true} if synchronization is held exclusively with
		/// respect to the current (calling) thread.  This method is invoked
		/// upon each call to a non-waiting <seealso cref="ConditionObject"/> method.
		/// (Waiting methods instead invoke <seealso cref="#release"/>.)
		/// 
		/// <para>The default implementation throws {@link
		/// UnsupportedOperationException}. This method is invoked
		/// internally only within <seealso cref="ConditionObject"/> methods, so need
		/// not be defined if conditions are not used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if synchronization is held exclusively;
		///         {@code false} otherwise </returns>
		/// <exception cref="UnsupportedOperationException"> if conditions are not supported </exception>
		protected internal virtual bool HeldExclusively
		{
			get
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Acquires in exclusive mode, ignoring interrupts.  Implemented
		/// by invoking at least once <seealso cref="#tryAcquire"/>,
		/// returning on success.  Otherwise the thread is queued, possibly
		/// repeatedly blocking and unblocking, invoking {@link
		/// #tryAcquire} until success.  This method can be used
		/// to implement method <seealso cref="Lock#lock"/>.
		/// </summary>
		/// <param name="arg"> the acquire argument.  This value is conveyed to
		///        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		///        can represent anything you like. </param>
		public void Acquire(long arg)
		{
			if (!TryAcquire(arg) && AcquireQueued(AddWaiter(Node.EXCLUSIVE), arg))
			{
				SelfInterrupt();
			}
		}

		/// <summary>
		/// Acquires in exclusive mode, aborting if interrupted.
		/// Implemented by first checking interrupt status, then invoking
		/// at least once <seealso cref="#tryAcquire"/>, returning on
		/// success.  Otherwise the thread is queued, possibly repeatedly
		/// blocking and unblocking, invoking <seealso cref="#tryAcquire"/>
		/// until success or the thread is interrupted.  This method can be
		/// used to implement method <seealso cref="Lock#lockInterruptibly"/>.
		/// </summary>
		/// <param name="arg"> the acquire argument.  This value is conveyed to
		///        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		///        can represent anything you like. </param>
		/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void acquireInterruptibly(long arg) throws InterruptedException
		public void AcquireInterruptibly(long arg)
		{
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			if (!TryAcquire(arg))
			{
				DoAcquireInterruptibly(arg);
			}
		}

		/// <summary>
		/// Attempts to acquire in exclusive mode, aborting if interrupted,
		/// and failing if the given timeout elapses.  Implemented by first
		/// checking interrupt status, then invoking at least once {@link
		/// #tryAcquire}, returning on success.  Otherwise, the thread is
		/// queued, possibly repeatedly blocking and unblocking, invoking
		/// <seealso cref="#tryAcquire"/> until success or the thread is interrupted
		/// or the timeout elapses.  This method can be used to implement
		/// method <seealso cref="Lock#tryLock(long, TimeUnit)"/>.
		/// </summary>
		/// <param name="arg"> the acquire argument.  This value is conveyed to
		///        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		///        can represent anything you like. </param>
		/// <param name="nanosTimeout"> the maximum number of nanoseconds to wait </param>
		/// <returns> {@code true} if acquired; {@code false} if timed out </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean tryAcquireNanos(long arg, long nanosTimeout) throws InterruptedException
		public bool TryAcquireNanos(long arg, long nanosTimeout)
		{
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			return TryAcquire(arg) || DoAcquireNanos(arg, nanosTimeout);
		}

		/// <summary>
		/// Releases in exclusive mode.  Implemented by unblocking one or
		/// more threads if <seealso cref="#tryRelease"/> returns true.
		/// This method can be used to implement method <seealso cref="Lock#unlock"/>.
		/// </summary>
		/// <param name="arg"> the release argument.  This value is conveyed to
		///        <seealso cref="#tryRelease"/> but is otherwise uninterpreted and
		///        can represent anything you like. </param>
		/// <returns> the value returned from <seealso cref="#tryRelease"/> </returns>
		public bool Release(long arg)
		{
			if (TryRelease(arg))
			{
				Node h = Head_Renamed;
				if (h != null && h.WaitStatus != 0)
				{
					UnparkSuccessor(h);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Acquires in shared mode, ignoring interrupts.  Implemented by
		/// first invoking at least once <seealso cref="#tryAcquireShared"/>,
		/// returning on success.  Otherwise the thread is queued, possibly
		/// repeatedly blocking and unblocking, invoking {@link
		/// #tryAcquireShared} until success.
		/// </summary>
		/// <param name="arg"> the acquire argument.  This value is conveyed to
		///        <seealso cref="#tryAcquireShared"/> but is otherwise uninterpreted
		///        and can represent anything you like. </param>
		public void AcquireShared(long arg)
		{
			if (TryAcquireShared(arg) < 0)
			{
				DoAcquireShared(arg);
			}
		}

		/// <summary>
		/// Acquires in shared mode, aborting if interrupted.  Implemented
		/// by first checking interrupt status, then invoking at least once
		/// <seealso cref="#tryAcquireShared"/>, returning on success.  Otherwise the
		/// thread is queued, possibly repeatedly blocking and unblocking,
		/// invoking <seealso cref="#tryAcquireShared"/> until success or the thread
		/// is interrupted. </summary>
		/// <param name="arg"> the acquire argument.
		/// This value is conveyed to <seealso cref="#tryAcquireShared"/> but is
		/// otherwise uninterpreted and can represent anything
		/// you like. </param>
		/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void acquireSharedInterruptibly(long arg) throws InterruptedException
		public void AcquireSharedInterruptibly(long arg)
		{
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			if (TryAcquireShared(arg) < 0)
			{
				DoAcquireSharedInterruptibly(arg);
			}
		}

		/// <summary>
		/// Attempts to acquire in shared mode, aborting if interrupted, and
		/// failing if the given timeout elapses.  Implemented by first
		/// checking interrupt status, then invoking at least once {@link
		/// #tryAcquireShared}, returning on success.  Otherwise, the
		/// thread is queued, possibly repeatedly blocking and unblocking,
		/// invoking <seealso cref="#tryAcquireShared"/> until success or the thread
		/// is interrupted or the timeout elapses.
		/// </summary>
		/// <param name="arg"> the acquire argument.  This value is conveyed to
		///        <seealso cref="#tryAcquireShared"/> but is otherwise uninterpreted
		///        and can represent anything you like. </param>
		/// <param name="nanosTimeout"> the maximum number of nanoseconds to wait </param>
		/// <returns> {@code true} if acquired; {@code false} if timed out </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean tryAcquireSharedNanos(long arg, long nanosTimeout) throws InterruptedException
		public bool TryAcquireSharedNanos(long arg, long nanosTimeout)
		{
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			return TryAcquireShared(arg) >= 0 || DoAcquireSharedNanos(arg, nanosTimeout);
		}

		/// <summary>
		/// Releases in shared mode.  Implemented by unblocking one or more
		/// threads if <seealso cref="#tryReleaseShared"/> returns true.
		/// </summary>
		/// <param name="arg"> the release argument.  This value is conveyed to
		///        <seealso cref="#tryReleaseShared"/> but is otherwise uninterpreted
		///        and can represent anything you like. </param>
		/// <returns> the value returned from <seealso cref="#tryReleaseShared"/> </returns>
		public bool ReleaseShared(long arg)
		{
			if (TryReleaseShared(arg))
			{
				DoReleaseShared();
				return true;
			}
			return false;
		}

		// Queue inspection methods

		/// <summary>
		/// Queries whether any threads are waiting to acquire. Note that
		/// because cancellations due to interrupts and timeouts may occur
		/// at any time, a {@code true} return does not guarantee that any
		/// other thread will ever acquire.
		/// 
		/// <para>In this implementation, this operation returns in
		/// constant time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if there may be other threads waiting to acquire </returns>
		public bool HasQueuedThreads()
		{
			return Head_Renamed != Tail;
		}

		/// <summary>
		/// Queries whether any threads have ever contended to acquire this
		/// synchronizer; that is if an acquire method has ever blocked.
		/// 
		/// <para>In this implementation, this operation returns in
		/// constant time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if there has ever been contention </returns>
		public bool HasContended()
		{
			return Head_Renamed != null;
		}

		/// <summary>
		/// Returns the first (longest-waiting) thread in the queue, or
		/// {@code null} if no threads are currently queued.
		/// 
		/// <para>In this implementation, this operation normally returns in
		/// constant time, but may iterate upon contention if other threads are
		/// concurrently modifying the queue.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first (longest-waiting) thread in the queue, or
		///         {@code null} if no threads are currently queued </returns>
		public Thread FirstQueuedThread
		{
			get
			{
				// handle only fast path, else relay
				return (Head_Renamed == Tail) ? null : FullGetFirstQueuedThread();
			}
		}

		/// <summary>
		/// Version of getFirstQueuedThread called when fastpath fails
		/// </summary>
		private Thread FullGetFirstQueuedThread()
		{
			/*
			 * The first node is normally head.next. Try to get its
			 * thread field, ensuring consistent reads: If thread
			 * field is nulled out or s.prev is no longer head, then
			 * some other thread(s) concurrently performed setHead in
			 * between some of our reads. We try this twice before
			 * resorting to traversal.
			 */
			Node h, s;
			Thread st;
			if (((h = Head_Renamed) != null && (s = h.Next) != null && s.Prev == Head_Renamed && (st = s.Thread) != null) || ((h = Head_Renamed) != null && (s = h.Next) != null && s.Prev == Head_Renamed && (st = s.Thread) != null))
			{
				return st;
			}

			/*
			 * Head's next field might not have been set yet, or may have
			 * been unset after setHead. So we must check to see if tail
			 * is actually first node. If not, we continue on, safely
			 * traversing from tail back to head to find first,
			 * guaranteeing termination.
			 */

			Node t = Tail;
			Thread firstThread = null;
			while (t != null && t != Head_Renamed)
			{
				Thread tt = t.Thread;
				if (tt != null)
				{
					firstThread = tt;
				}
				t = t.Prev;
			}
			return firstThread;
		}

		/// <summary>
		/// Returns true if the given thread is currently queued.
		/// 
		/// <para>This implementation traverses the queue to determine
		/// presence of the given thread.
		/// 
		/// </para>
		/// </summary>
		/// <param name="thread"> the thread </param>
		/// <returns> {@code true} if the given thread is on the queue </returns>
		/// <exception cref="NullPointerException"> if the thread is null </exception>
		public bool IsQueued(Thread thread)
		{
			if (thread == null)
			{
				throw new NullPointerException();
			}
			for (Node p = Tail; p != null; p = p.Prev)
			{
				if (p.Thread == thread)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns {@code true} if the apparent first queued thread, if one
		/// exists, is waiting in exclusive mode.  If this method returns
		/// {@code true}, and the current thread is attempting to acquire in
		/// shared mode (that is, this method is invoked from {@link
		/// #tryAcquireShared}) then it is guaranteed that the current thread
		/// is not the first queued thread.  Used only as a heuristic in
		/// ReentrantReadWriteLock.
		/// </summary>
		internal bool ApparentlyFirstQueuedIsExclusive()
		{
			Node h, s;
			return (h = Head_Renamed) != null && (s = h.Next) != null && !s.Shared && s.Thread != null;
		}

		/// <summary>
		/// Queries whether any threads have been waiting to acquire longer
		/// than the current thread.
		/// 
		/// <para>An invocation of this method is equivalent to (but may be
		/// more efficient than):
		///  <pre> {@code
		/// getFirstQueuedThread() != Thread.currentThread() &&
		/// hasQueuedThreads()}</pre>
		/// 
		/// </para>
		/// <para>Note that because cancellations due to interrupts and
		/// timeouts may occur at any time, a {@code true} return does not
		/// guarantee that some other thread will acquire before the current
		/// thread.  Likewise, it is possible for another thread to win a
		/// race to enqueue after this method has returned {@code false},
		/// due to the queue being empty.
		/// 
		/// </para>
		/// <para>This method is designed to be used by a fair synchronizer to
		/// avoid <a href="AbstractQueuedSynchronizer.html#barging">barging</a>.
		/// Such a synchronizer's <seealso cref="#tryAcquire"/> method should return
		/// {@code false}, and its <seealso cref="#tryAcquireShared"/> method should
		/// return a negative value, if this method returns {@code true}
		/// (unless this is a reentrant acquire).  For example, the {@code
		/// tryAcquire} method for a fair, reentrant, exclusive mode
		/// synchronizer might look like this:
		/// 
		///  <pre> {@code
		/// protected boolean tryAcquire(int arg) {
		///   if (isHeldExclusively()) {
		///     // A reentrant acquire; increment hold count
		///     return true;
		///   } else if (hasQueuedPredecessors()) {
		///     return false;
		///   } else {
		///     // try to acquire normally
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if there is a queued thread preceding the
		///         current thread, and {@code false} if the current thread
		///         is at the head of the queue or the queue is empty
		/// @since 1.7 </returns>
		public bool HasQueuedPredecessors()
		{
			// The correctness of this depends on head being initialized
			// before tail and on head.next being accurate if the current
			// thread is first in queue.
			Node t = Tail; // Read fields in reverse initialization order
			Node h = Head_Renamed;
			Node s;
			return h != t && ((s = h.Next) == null || s.Thread != Thread.CurrentThread);
		}


		// Instrumentation and monitoring methods

		/// <summary>
		/// Returns an estimate of the number of threads waiting to
		/// acquire.  The value is only an estimate because the number of
		/// threads may change dynamically while this method traverses
		/// internal data structures.  This method is designed for use in
		/// monitoring system state, not for synchronization
		/// control.
		/// </summary>
		/// <returns> the estimated number of threads waiting to acquire </returns>
		public int QueueLength
		{
			get
			{
				int n = 0;
				for (Node p = Tail; p != null; p = p.Prev)
				{
					if (p.Thread != null)
					{
						++n;
					}
				}
				return n;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire.  Because the actual set of threads may change
		/// dynamically while constructing this result, the returned
		/// collection is only a best-effort estimate.  The elements of the
		/// returned collection are in no particular order.  This method is
		/// designed to facilitate construction of subclasses that provide
		/// more extensive monitoring facilities.
		/// </summary>
		/// <returns> the collection of threads </returns>
		public ICollection<Thread> QueuedThreads
		{
			get
			{
				List<Thread> list = new List<Thread>();
				for (Node p = Tail; p != null; p = p.Prev)
				{
					Thread t = p.Thread;
					if (t != null)
					{
						list.Add(t);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire in exclusive mode. This has the same properties
		/// as <seealso cref="#getQueuedThreads"/> except that it only returns
		/// those threads waiting due to an exclusive acquire.
		/// </summary>
		/// <returns> the collection of threads </returns>
		public ICollection<Thread> ExclusiveQueuedThreads
		{
			get
			{
				List<Thread> list = new List<Thread>();
				for (Node p = Tail; p != null; p = p.Prev)
				{
					if (!p.Shared)
					{
						Thread t = p.Thread;
						if (t != null)
						{
							list.Add(t);
						}
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire in shared mode. This has the same properties
		/// as <seealso cref="#getQueuedThreads"/> except that it only returns
		/// those threads waiting due to a shared acquire.
		/// </summary>
		/// <returns> the collection of threads </returns>
		public ICollection<Thread> SharedQueuedThreads
		{
			get
			{
				List<Thread> list = new List<Thread>();
				for (Node p = Tail; p != null; p = p.Prev)
				{
					if (p.Shared)
					{
						Thread t = p.Thread;
						if (t != null)
						{
							list.Add(t);
						}
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Returns a string identifying this synchronizer, as well as its state.
		/// The state, in brackets, includes the String {@code "State ="}
		/// followed by the current value of <seealso cref="#getState"/>, and either
		/// {@code "nonempty"} or {@code "empty"} depending on whether the
		/// queue is empty.
		/// </summary>
		/// <returns> a string identifying this synchronizer, as well as its state </returns>
		public override String ToString()
		{
			long s = State;
			String q = HasQueuedThreads() ? "non" : "";
			return base.ToString() + "[State = " + s + ", " + q + "empty queue]";
		}


		// Internal support methods for Conditions

		/// <summary>
		/// Returns true if a node, always one that was initially placed on
		/// a condition queue, is now waiting to reacquire on sync queue. </summary>
		/// <param name="node"> the node </param>
		/// <returns> true if is reacquiring </returns>
		internal bool IsOnSyncQueue(Node node)
		{
			if (node.WaitStatus == Node.CONDITION || node.Prev == null)
			{
				return false;
			}
			if (node.Next != null) // If has successor, it must be on queue
			{
				return true;
			}
			/*
			 * node.prev can be non-null, but not yet on queue because
			 * the CAS to place it on queue can fail. So we have to
			 * traverse from tail to make sure it actually made it.  It
			 * will always be near the tail in calls to this method, and
			 * unless the CAS failed (which is unlikely), it will be
			 * there, so we hardly ever traverse much.
			 */
			return FindNodeFromTail(node);
		}

		/// <summary>
		/// Returns true if node is on sync queue by searching backwards from tail.
		/// Called only when needed by isOnSyncQueue. </summary>
		/// <returns> true if present </returns>
		private bool FindNodeFromTail(Node node)
		{
			Node t = Tail;
			for (;;)
			{
				if (t == node)
				{
					return true;
				}
				if (t == null)
				{
					return false;
				}
				t = t.Prev;
			}
		}

		/// <summary>
		/// Transfers a node from a condition queue onto sync queue.
		/// Returns true if successful. </summary>
		/// <param name="node"> the node </param>
		/// <returns> true if successfully transferred (else the node was
		/// cancelled before signal) </returns>
		internal bool TransferForSignal(Node node)
		{
			/*
			 * If cannot change waitStatus, the node has been cancelled.
			 */
			if (!CompareAndSetWaitStatus(node, Node.CONDITION, 0))
			{
				return false;
			}

			/*
			 * Splice onto queue and try to set waitStatus of predecessor to
			 * indicate that thread is (probably) waiting. If cancelled or
			 * attempt to set waitStatus fails, wake up to resync (in which
			 * case the waitStatus can be transiently and harmlessly wrong).
			 */
			Node p = Enq(node);
			int ws = p.WaitStatus;
			if (ws > 0 || !CompareAndSetWaitStatus(p, ws, Node.SIGNAL))
			{
				LockSupport.Unpark(node.Thread);
			}
			return true;
		}

		/// <summary>
		/// Transfers node, if necessary, to sync queue after a cancelled wait.
		/// Returns true if thread was cancelled before being signalled.
		/// </summary>
		/// <param name="node"> the node </param>
		/// <returns> true if cancelled before the node was signalled </returns>
		internal bool TransferAfterCancelledWait(Node node)
		{
			if (CompareAndSetWaitStatus(node, Node.CONDITION, 0))
			{
				Enq(node);
				return true;
			}
			/*
			 * If we lost out to a signal(), then we can't proceed
			 * until it finishes its enq().  Cancelling during an
			 * incomplete transfer is both rare and transient, so just
			 * spin.
			 */
			while (!IsOnSyncQueue(node))
			{
				Thread.@yield();
			}
			return false;
		}

		/// <summary>
		/// Invokes release with current state value; returns saved state.
		/// Cancels node and throws exception on failure. </summary>
		/// <param name="node"> the condition node for this wait </param>
		/// <returns> previous sync state </returns>
		internal long FullyRelease(Node node)
		{
			bool failed = true;
			try
			{
				long savedState = State;
				if (Release(savedState))
				{
					failed = false;
					return savedState;
				}
				else
				{
					throw new IllegalMonitorStateException();
				}
			}
			finally
			{
				if (failed)
				{
					node.WaitStatus = Node.CANCELLED;
				}
			}
		}

		// Instrumentation methods for conditions

		/// <summary>
		/// Queries whether the given ConditionObject
		/// uses this synchronizer as its lock.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> {@code true} if owned </returns>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public bool Owns(ConditionObject condition)
		{
			return condition.IsOwnedBy(this);
		}

		/// <summary>
		/// Queries whether any threads are waiting on the given condition
		/// associated with this synchronizer. Note that because timeouts
		/// and interrupts may occur at any time, a {@code true} return
		/// does not guarantee that a future {@code signal} will awaken
		/// any threads.  This method is designed primarily for use in
		/// monitoring of the system state.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> {@code true} if there are any waiting threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		///         is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this synchronizer </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public bool HasWaiters(ConditionObject condition)
		{
			if (!Owns(condition))
			{
				throw new IllegalArgumentException("Not owner");
			}
			return condition.HasWaiters();
		}

		/// <summary>
		/// Returns an estimate of the number of threads waiting on the
		/// given condition associated with this synchronizer. Note that
		/// because timeouts and interrupts may occur at any time, the
		/// estimate serves only as an upper bound on the actual number of
		/// waiters.  This method is designed for use in monitoring of the
		/// system state, not for synchronization control.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> the estimated number of waiting threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		///         is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this synchronizer </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public int GetWaitQueueLength(ConditionObject condition)
		{
			if (!Owns(condition))
			{
				throw new IllegalArgumentException("Not owner");
			}
			return condition.WaitQueueLength;
		}

		/// <summary>
		/// Returns a collection containing those threads that may be
		/// waiting on the given condition associated with this
		/// synchronizer.  Because the actual set of threads may change
		/// dynamically while constructing this result, the returned
		/// collection is only a best-effort estimate. The elements of the
		/// returned collection are in no particular order.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> the collection of threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		///         is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this synchronizer </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public ICollection<Thread> GetWaitingThreads(ConditionObject condition)
		{
			if (!Owns(condition))
			{
				throw new IllegalArgumentException("Not owner");
			}
			return condition.WaitingThreads;
		}

		/// <summary>
		/// Condition implementation for a {@link
		/// AbstractQueuedLongSynchronizer} serving as the basis of a {@link
		/// Lock} implementation.
		/// 
		/// <para>Method documentation for this class describes mechanics,
		/// not behavioral specifications from the point of view of Lock
		/// and Condition users. Exported versions of this class will in
		/// general need to be accompanied by documentation describing
		/// condition semantics that rely on those of the associated
		/// {@code AbstractQueuedLongSynchronizer}.
		/// 
		/// </para>
		/// <para>This class is Serializable, but all fields are transient,
		/// so deserialized conditions have no waiters.
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		[Serializable]
		public class ConditionObject : Condition
		{
			private readonly AbstractQueuedLongSynchronizer OuterInstance;

			internal const long SerialVersionUID = 1173984872572414699L;
			/// <summary>
			/// First node of condition queue. </summary>
			[NonSerialized]
			internal Node FirstWaiter;
			/// <summary>
			/// Last node of condition queue. </summary>
			[NonSerialized]
			internal Node LastWaiter;

			/// <summary>
			/// Creates a new {@code ConditionObject} instance.
			/// </summary>
			public ConditionObject(AbstractQueuedLongSynchronizer outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			// Internal methods

			/// <summary>
			/// Adds a new waiter to wait queue. </summary>
			/// <returns> its new wait node </returns>
			internal virtual Node AddConditionWaiter()
			{
				Node t = LastWaiter;
				// If lastWaiter is cancelled, clean out.
				if (t != null && t.WaitStatus != Node.CONDITION)
				{
					UnlinkCancelledWaiters();
					t = LastWaiter;
				}
				Node node = new Node(Thread.CurrentThread, Node.CONDITION);
				if (t == null)
				{
					FirstWaiter = node;
				}
				else
				{
					t.NextWaiter = node;
				}
				LastWaiter = node;
				return node;
			}

			/// <summary>
			/// Removes and transfers nodes until hit non-cancelled one or
			/// null. Split out from signal in part to encourage compilers
			/// to inline the case of no waiters. </summary>
			/// <param name="first"> (non-null) the first node on condition queue </param>
			internal virtual void DoSignal(Node first)
			{
				do
				{
					if ((FirstWaiter = first.NextWaiter) == null)
					{
						LastWaiter = null;
					}
					first.NextWaiter = null;
				} while (!outerInstance.TransferForSignal(first) && (first = FirstWaiter) != null);
			}

			/// <summary>
			/// Removes and transfers all nodes. </summary>
			/// <param name="first"> (non-null) the first node on condition queue </param>
			internal virtual void DoSignalAll(Node first)
			{
				LastWaiter = FirstWaiter = null;
				do
				{
					Node next = first.NextWaiter;
					first.NextWaiter = null;
					outerInstance.TransferForSignal(first);
					first = next;
				} while (first != null);
			}

			/// <summary>
			/// Unlinks cancelled waiter nodes from condition queue.
			/// Called only while holding lock. This is called when
			/// cancellation occurred during condition wait, and upon
			/// insertion of a new waiter when lastWaiter is seen to have
			/// been cancelled. This method is needed to avoid garbage
			/// retention in the absence of signals. So even though it may
			/// require a full traversal, it comes into play only when
			/// timeouts or cancellations occur in the absence of
			/// signals. It traverses all nodes rather than stopping at a
			/// particular target to unlink all pointers to garbage nodes
			/// without requiring many re-traversals during cancellation
			/// storms.
			/// </summary>
			internal virtual void UnlinkCancelledWaiters()
			{
				Node t = FirstWaiter;
				Node trail = null;
				while (t != null)
				{
					Node next = t.NextWaiter;
					if (t.WaitStatus != Node.CONDITION)
					{
						t.NextWaiter = null;
						if (trail == null)
						{
							FirstWaiter = next;
						}
						else
						{
							trail.NextWaiter = next;
						}
						if (next == null)
						{
							LastWaiter = trail;
						}
					}
					else
					{
						trail = t;
					}
					t = next;
				}
			}

			// public methods

			/// <summary>
			/// Moves the longest-waiting thread, if one exists, from the
			/// wait queue for this condition to the wait queue for the
			/// owning lock.
			/// </summary>
			/// <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			///         returns {@code false} </exception>
			public void Signal()
			{
				if (!outerInstance.HeldExclusively)
				{
					throw new IllegalMonitorStateException();
				}
				Node first = FirstWaiter;
				if (first != null)
				{
					DoSignal(first);
				}
			}

			/// <summary>
			/// Moves all threads from the wait queue for this condition to
			/// the wait queue for the owning lock.
			/// </summary>
			/// <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			///         returns {@code false} </exception>
			public void SignalAll()
			{
				if (!outerInstance.HeldExclusively)
				{
					throw new IllegalMonitorStateException();
				}
				Node first = FirstWaiter;
				if (first != null)
				{
					DoSignalAll(first);
				}
			}

			/// <summary>
			/// Implements uninterruptible condition wait.
			/// <ol>
			/// <li> Save lock state returned by <seealso cref="#getState"/>.
			/// <li> Invoke <seealso cref="#release"/> with saved state as argument,
			///      throwing IllegalMonitorStateException if it fails.
			/// <li> Block until signalled.
			/// <li> Reacquire by invoking specialized version of
			///      <seealso cref="#acquire"/> with saved state as argument.
			/// </ol>
			/// </summary>
			public void AwaitUninterruptibly()
			{
				Node node = AddConditionWaiter();
				long savedState = outerInstance.FullyRelease(node);
				bool interrupted = false;
				while (!outerInstance.IsOnSyncQueue(node))
				{
					LockSupport.Park(this);
					if (Thread.Interrupted())
					{
						interrupted = true;
					}
				}
				if (outerInstance.AcquireQueued(node, savedState) || interrupted)
				{
					SelfInterrupt();
				}
			}

			/*
			 * For interruptible waits, we need to track whether to throw
			 * InterruptedException, if interrupted while blocked on
			 * condition, versus reinterrupt current thread, if
			 * interrupted while blocked waiting to re-acquire.
			 */

			/// <summary>
			/// Mode meaning to reinterrupt on exit from wait </summary>
			internal const int REINTERRUPT = 1;
			/// <summary>
			/// Mode meaning to throw InterruptedException on exit from wait </summary>
			internal const int THROW_IE = -1;

			/// <summary>
			/// Checks for interrupt, returning THROW_IE if interrupted
			/// before signalled, REINTERRUPT if after signalled, or
			/// 0 if not interrupted.
			/// </summary>
			internal virtual int CheckInterruptWhileWaiting(Node node)
			{
				return Thread.Interrupted() ? (outerInstance.TransferAfterCancelledWait(node) ? THROW_IE : REINTERRUPT) : 0;
			}

			/// <summary>
			/// Throws InterruptedException, reinterrupts current thread, or
			/// does nothing, depending on mode.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void reportInterruptAfterWait(int interruptMode) throws InterruptedException
			internal virtual void ReportInterruptAfterWait(int interruptMode)
			{
				if (interruptMode == THROW_IE)
				{
					throw new InterruptedException();
				}
				else if (interruptMode == REINTERRUPT)
				{
					SelfInterrupt();
				}
			}

			/// <summary>
			/// Implements interruptible condition wait.
			/// <ol>
			/// <li> If current thread is interrupted, throw InterruptedException.
			/// <li> Save lock state returned by <seealso cref="#getState"/>.
			/// <li> Invoke <seealso cref="#release"/> with saved state as argument,
			///      throwing IllegalMonitorStateException if it fails.
			/// <li> Block until signalled or interrupted.
			/// <li> Reacquire by invoking specialized version of
			///      <seealso cref="#acquire"/> with saved state as argument.
			/// <li> If interrupted while blocked in step 4, throw InterruptedException.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void await() throws InterruptedException
			public void @await()
			{
				if (Thread.Interrupted())
				{
					throw new InterruptedException();
				}
				Node node = AddConditionWaiter();
				long savedState = outerInstance.FullyRelease(node);
				int interruptMode = 0;
				while (!outerInstance.IsOnSyncQueue(node))
				{
					LockSupport.Park(this);
					if ((interruptMode = CheckInterruptWhileWaiting(node)) != 0)
					{
						break;
					}
				}
				if (outerInstance.AcquireQueued(node, savedState) && interruptMode != THROW_IE)
				{
					interruptMode = REINTERRUPT;
				}
				if (node.NextWaiter != null) // clean up if cancelled
				{
					UnlinkCancelledWaiters();
				}
				if (interruptMode != 0)
				{
					ReportInterruptAfterWait(interruptMode);
				}
			}

			/// <summary>
			/// Implements timed condition wait.
			/// <ol>
			/// <li> If current thread is interrupted, throw InterruptedException.
			/// <li> Save lock state returned by <seealso cref="#getState"/>.
			/// <li> Invoke <seealso cref="#release"/> with saved state as argument,
			///      throwing IllegalMonitorStateException if it fails.
			/// <li> Block until signalled, interrupted, or timed out.
			/// <li> Reacquire by invoking specialized version of
			///      <seealso cref="#acquire"/> with saved state as argument.
			/// <li> If interrupted while blocked in step 4, throw InterruptedException.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final long awaitNanos(long nanosTimeout) throws InterruptedException
			public long AwaitNanos(long nanosTimeout)
			{
				if (Thread.Interrupted())
				{
					throw new InterruptedException();
				}
				Node node = AddConditionWaiter();
				long savedState = outerInstance.FullyRelease(node);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = System.nanoTime() + nanosTimeout;
				long deadline = System.nanoTime() + nanosTimeout;
				int interruptMode = 0;
				while (!outerInstance.IsOnSyncQueue(node))
				{
					if (nanosTimeout <= 0L)
					{
						outerInstance.TransferAfterCancelledWait(node);
						break;
					}
					if (nanosTimeout >= SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanosTimeout);
					}
					if ((interruptMode = CheckInterruptWhileWaiting(node)) != 0)
					{
						break;
					}
					nanosTimeout = deadline - System.nanoTime();
				}
				if (outerInstance.AcquireQueued(node, savedState) && interruptMode != THROW_IE)
				{
					interruptMode = REINTERRUPT;
				}
				if (node.NextWaiter != null)
				{
					UnlinkCancelledWaiters();
				}
				if (interruptMode != 0)
				{
					ReportInterruptAfterWait(interruptMode);
				}
				return deadline - System.nanoTime();
			}

			/// <summary>
			/// Implements absolute timed condition wait.
			/// <ol>
			/// <li> If current thread is interrupted, throw InterruptedException.
			/// <li> Save lock state returned by <seealso cref="#getState"/>.
			/// <li> Invoke <seealso cref="#release"/> with saved state as argument,
			///      throwing IllegalMonitorStateException if it fails.
			/// <li> Block until signalled, interrupted, or timed out.
			/// <li> Reacquire by invoking specialized version of
			///      <seealso cref="#acquire"/> with saved state as argument.
			/// <li> If interrupted while blocked in step 4, throw InterruptedException.
			/// <li> If timed out while blocked in step 4, return false, else true.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean awaitUntil(java.util.Date deadline) throws InterruptedException
			public bool AwaitUntil(DateTime deadline)
			{
				long abstime = deadline.Ticks;
				if (Thread.Interrupted())
				{
					throw new InterruptedException();
				}
				Node node = AddConditionWaiter();
				long savedState = outerInstance.FullyRelease(node);
				bool timedout = false;
				int interruptMode = 0;
				while (!outerInstance.IsOnSyncQueue(node))
				{
					if (DateTimeHelperClass.CurrentUnixTimeMillis() > abstime)
					{
						timedout = outerInstance.TransferAfterCancelledWait(node);
						break;
					}
					LockSupport.ParkUntil(this, abstime);
					if ((interruptMode = CheckInterruptWhileWaiting(node)) != 0)
					{
						break;
					}
				}
				if (outerInstance.AcquireQueued(node, savedState) && interruptMode != THROW_IE)
				{
					interruptMode = REINTERRUPT;
				}
				if (node.NextWaiter != null)
				{
					UnlinkCancelledWaiters();
				}
				if (interruptMode != 0)
				{
					ReportInterruptAfterWait(interruptMode);
				}
				return !timedout;
			}

			/// <summary>
			/// Implements timed condition wait.
			/// <ol>
			/// <li> If current thread is interrupted, throw InterruptedException.
			/// <li> Save lock state returned by <seealso cref="#getState"/>.
			/// <li> Invoke <seealso cref="#release"/> with saved state as argument,
			///      throwing IllegalMonitorStateException if it fails.
			/// <li> Block until signalled, interrupted, or timed out.
			/// <li> Reacquire by invoking specialized version of
			///      <seealso cref="#acquire"/> with saved state as argument.
			/// <li> If interrupted while blocked in step 4, throw InterruptedException.
			/// <li> If timed out while blocked in step 4, return false, else true.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean await(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException
			public bool @await(long time, TimeUnit unit)
			{
				long nanosTimeout = unit.ToNanos(time);
				if (Thread.Interrupted())
				{
					throw new InterruptedException();
				}
				Node node = AddConditionWaiter();
				long savedState = outerInstance.FullyRelease(node);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = System.nanoTime() + nanosTimeout;
				long deadline = System.nanoTime() + nanosTimeout;
				bool timedout = false;
				int interruptMode = 0;
				while (!outerInstance.IsOnSyncQueue(node))
				{
					if (nanosTimeout <= 0L)
					{
						timedout = outerInstance.TransferAfterCancelledWait(node);
						break;
					}
					if (nanosTimeout >= SpinForTimeoutThreshold)
					{
						LockSupport.ParkNanos(this, nanosTimeout);
					}
					if ((interruptMode = CheckInterruptWhileWaiting(node)) != 0)
					{
						break;
					}
					nanosTimeout = deadline - System.nanoTime();
				}
				if (outerInstance.AcquireQueued(node, savedState) && interruptMode != THROW_IE)
				{
					interruptMode = REINTERRUPT;
				}
				if (node.NextWaiter != null)
				{
					UnlinkCancelledWaiters();
				}
				if (interruptMode != 0)
				{
					ReportInterruptAfterWait(interruptMode);
				}
				return !timedout;
			}

			//  support for instrumentation

			/// <summary>
			/// Returns true if this condition was created by the given
			/// synchronization object.
			/// </summary>
			/// <returns> {@code true} if owned </returns>
			internal bool IsOwnedBy(AbstractQueuedLongSynchronizer sync)
			{
				return sync == OuterInstance;
			}

			/// <summary>
			/// Queries whether any threads are waiting on this condition.
			/// Implements <seealso cref="AbstractQueuedLongSynchronizer#hasWaiters(ConditionObject)"/>.
			/// </summary>
			/// <returns> {@code true} if there are any waiting threads </returns>
			/// <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			///         returns {@code false} </exception>
			protected internal bool HasWaiters()
			{
				if (!outerInstance.HeldExclusively)
				{
					throw new IllegalMonitorStateException();
				}
				for (Node w = FirstWaiter; w != null; w = w.NextWaiter)
				{
					if (w.WaitStatus == Node.CONDITION)
					{
						return true;
					}
				}
				return false;
			}

			/// <summary>
			/// Returns an estimate of the number of threads waiting on
			/// this condition.
			/// Implements <seealso cref="AbstractQueuedLongSynchronizer#getWaitQueueLength(ConditionObject)"/>.
			/// </summary>
			/// <returns> the estimated number of waiting threads </returns>
			/// <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			///         returns {@code false} </exception>
			protected internal int WaitQueueLength
			{
				get
				{
					if (!outerInstance.HeldExclusively)
					{
						throw new IllegalMonitorStateException();
					}
					int n = 0;
					for (Node w = FirstWaiter; w != null; w = w.NextWaiter)
					{
						if (w.WaitStatus == Node.CONDITION)
						{
							++n;
						}
					}
					return n;
				}
			}

			/// <summary>
			/// Returns a collection containing those threads that may be
			/// waiting on this Condition.
			/// Implements <seealso cref="AbstractQueuedLongSynchronizer#getWaitingThreads(ConditionObject)"/>.
			/// </summary>
			/// <returns> the collection of threads </returns>
			/// <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			///         returns {@code false} </exception>
			protected internal ICollection<Thread> WaitingThreads
			{
				get
				{
					if (!outerInstance.HeldExclusively)
					{
						throw new IllegalMonitorStateException();
					}
					List<Thread> list = new List<Thread>();
					for (Node w = FirstWaiter; w != null; w = w.NextWaiter)
					{
						if (w.WaitStatus == Node.CONDITION)
						{
							Thread t = w.Thread;
							if (t != null)
							{
								list.Add(t);
							}
						}
					}
					return list;
				}
			}
		}

		/// <summary>
		/// Setup to support compareAndSet. We need to natively implement
		/// this here: For the sake of permitting future enhancements, we
		/// cannot explicitly subclass AtomicLong, which would be
		/// efficient and useful otherwise. So, as the lesser of evils, we
		/// natively implement using hotspot intrinsics API. And while we
		/// are at it, we do the same for other CASable fields (which could
		/// otherwise be done with atomic field updaters).
		/// </summary>
		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long StateOffset;
		private static readonly long HeadOffset;
		private static readonly long TailOffset;
		private static readonly long WaitStatusOffset;
		private static readonly long NextOffset;

		static AbstractQueuedLongSynchronizer()
		{
			try
			{
				StateOffset = @unsafe.objectFieldOffset(typeof(AbstractQueuedLongSynchronizer).getDeclaredField("state"));
				HeadOffset = @unsafe.objectFieldOffset(typeof(AbstractQueuedLongSynchronizer).getDeclaredField("head"));
				TailOffset = @unsafe.objectFieldOffset(typeof(AbstractQueuedLongSynchronizer).getDeclaredField("tail"));
				WaitStatusOffset = @unsafe.objectFieldOffset(typeof(Node).getDeclaredField("waitStatus"));
				NextOffset = @unsafe.objectFieldOffset(typeof(Node).getDeclaredField("next"));

			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		/// <summary>
		/// CAS head field. Used only by enq.
		/// </summary>
		private bool CompareAndSetHead(Node update)
		{
			return @unsafe.compareAndSwapObject(this, HeadOffset, null, update);
		}

		/// <summary>
		/// CAS tail field. Used only by enq.
		/// </summary>
		private bool CompareAndSetTail(Node expect, Node update)
		{
			return @unsafe.compareAndSwapObject(this, TailOffset, expect, update);
		}

		/// <summary>
		/// CAS waitStatus field of a node.
		/// </summary>
		private static bool CompareAndSetWaitStatus(Node node, int expect, int update)
		{
			return @unsafe.compareAndSwapInt(node, WaitStatusOffset, expect, update);
		}

		/// <summary>
		/// CAS next field of a node.
		/// </summary>
		private static bool CompareAndSetNext(Node node, Node expect, Node update)
		{
			return @unsafe.compareAndSwapObject(node, NextOffset, expect, update);
		}
	}

}