﻿using System;
using System.Collections;
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

namespace java.util.concurrent
{


	/// <summary>
	/// An unbounded <seealso cref="TransferQueue"/> based on linked nodes.
	/// This queue orders elements FIFO (first-in-first-out) with respect
	/// to any given producer.  The <em>head</em> of the queue is that
	/// element that has been on the queue the longest time for some
	/// producer.  The <em>tail</em> of the queue is that element that has
	/// been on the queue the shortest time for some producer.
	/// 
	/// <para>Beware that, unlike in most collections, the {@code size} method
	/// is <em>NOT</em> a constant-time operation. Because of the
	/// asynchronous nature of these queues, determining the current number
	/// of elements requires a traversal of the elements, and so may report
	/// inaccurate results if this collection is modified during traversal.
	/// Additionally, the bulk operations {@code addAll},
	/// {@code removeAll}, {@code retainAll}, {@code containsAll},
	/// {@code equals}, and {@code toArray} are <em>not</em> guaranteed
	/// to be performed atomically. For example, an iterator operating
	/// concurrently with an {@code addAll} operation might view only some
	/// of the added elements.
	/// 
	/// </para>
	/// <para>This class and its iterator implement all of the
	/// <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	/// Iterator} interfaces.
	/// 
	/// </para>
	/// <para>Memory consistency effects: As with other concurrent
	/// collections, actions in a thread prior to placing an object into a
	/// {@code LinkedTransferQueue}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that element from
	/// the {@code LinkedTransferQueue} in another thread.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class LinkedTransferQueue<E> : AbstractQueue<E>, TransferQueue<E>
	{
		private const long SerialVersionUID = -3223113410248163686L;

		/*
		 * *** Overview of Dual Queues with Slack ***
		 *
		 * Dual Queues, introduced by Scherer and Scott
		 * (http://www.cs.rice.edu/~wns1/papers/2004-DISC-DDS.pdf) are
		 * (linked) queues in which nodes may represent either data or
		 * requests.  When a thread tries to enqueue a data node, but
		 * encounters a request node, it instead "matches" and removes it;
		 * and vice versa for enqueuing requests. Blocking Dual Queues
		 * arrange that threads enqueuing unmatched requests block until
		 * other threads provide the match. Dual Synchronous Queues (see
		 * Scherer, Lea, & Scott
		 * http://www.cs.rochester.edu/u/scott/papers/2009_Scherer_CACM_SSQ.pdf)
		 * additionally arrange that threads enqueuing unmatched data also
		 * block.  Dual Transfer Queues support all of these modes, as
		 * dictated by callers.
		 *
		 * A FIFO dual queue may be implemented using a variation of the
		 * Michael & Scott (M&S) lock-free queue algorithm
		 * (http://www.cs.rochester.edu/u/scott/papers/1996_PODC_queues.pdf).
		 * It maintains two pointer fields, "head", pointing to a
		 * (matched) node that in turn points to the first actual
		 * (unmatched) queue node (or null if empty); and "tail" that
		 * points to the last node on the queue (or again null if
		 * empty). For example, here is a possible queue with four data
		 * elements:
		 *
		 *  head                tail
		 *    |                   |
		 *    v                   v
		 *    M -> U -> U -> U -> U
		 *
		 * The M&S queue algorithm is known to be prone to scalability and
		 * overhead limitations when maintaining (via CAS) these head and
		 * tail pointers. This has led to the development of
		 * contention-reducing variants such as elimination arrays (see
		 * Moir et al http://portal.acm.org/citation.cfm?id=1074013) and
		 * optimistic back pointers (see Ladan-Mozes & Shavit
		 * http://people.csail.mit.edu/edya/publications/OptimisticFIFOQueue-journal.pdf).
		 * However, the nature of dual queues enables a simpler tactic for
		 * improving M&S-style implementations when dual-ness is needed.
		 *
		 * In a dual queue, each node must atomically maintain its match
		 * status. While there are other possible variants, we implement
		 * this here as: for a data-mode node, matching entails CASing an
		 * "item" field from a non-null data value to null upon match, and
		 * vice-versa for request nodes, CASing from null to a data
		 * value. (Note that the linearization properties of this style of
		 * queue are easy to verify -- elements are made available by
		 * linking, and unavailable by matching.) Compared to plain M&S
		 * queues, this property of dual queues requires one additional
		 * successful atomic operation per enq/deq pair. But it also
		 * enables lower cost variants of queue maintenance mechanics. (A
		 * variation of this idea applies even for non-dual queues that
		 * support deletion of interior elements, such as
		 * j.u.c.ConcurrentLinkedQueue.)
		 *
		 * Once a node is matched, its match status can never again
		 * change.  We may thus arrange that the linked list of them
		 * contain a prefix of zero or more matched nodes, followed by a
		 * suffix of zero or more unmatched nodes. (Note that we allow
		 * both the prefix and suffix to be zero length, which in turn
		 * means that we do not use a dummy header.)  If we were not
		 * concerned with either time or space efficiency, we could
		 * correctly perform enqueue and dequeue operations by traversing
		 * from a pointer to the initial node; CASing the item of the
		 * first unmatched node on match and CASing the next field of the
		 * trailing node on appends. (Plus some special-casing when
		 * initially empty).  While this would be a terrible idea in
		 * itself, it does have the benefit of not requiring ANY atomic
		 * updates on head/tail fields.
		 *
		 * We introduce here an approach that lies between the extremes of
		 * never versus always updating queue (head and tail) pointers.
		 * This offers a tradeoff between sometimes requiring extra
		 * traversal steps to locate the first and/or last unmatched
		 * nodes, versus the reduced overhead and contention of fewer
		 * updates to queue pointers. For example, a possible snapshot of
		 * a queue is:
		 *
		 *  head           tail
		 *    |              |
		 *    v              v
		 *    M -> M -> U -> U -> U -> U
		 *
		 * The best value for this "slack" (the targeted maximum distance
		 * between the value of "head" and the first unmatched node, and
		 * similarly for "tail") is an empirical matter. We have found
		 * that using very small constants in the range of 1-3 work best
		 * over a range of platforms. Larger values introduce increasing
		 * costs of cache misses and risks of long traversal chains, while
		 * smaller values increase CAS contention and overhead.
		 *
		 * Dual queues with slack differ from plain M&S dual queues by
		 * virtue of only sometimes updating head or tail pointers when
		 * matching, appending, or even traversing nodes; in order to
		 * maintain a targeted slack.  The idea of "sometimes" may be
		 * operationalized in several ways. The simplest is to use a
		 * per-operation counter incremented on each traversal step, and
		 * to try (via CAS) to update the associated queue pointer
		 * whenever the count exceeds a threshold. Another, that requires
		 * more overhead, is to use random number generators to update
		 * with a given probability per traversal step.
		 *
		 * In any strategy along these lines, because CASes updating
		 * fields may fail, the actual slack may exceed targeted
		 * slack. However, they may be retried at any time to maintain
		 * targets.  Even when using very small slack values, this
		 * approach works well for dual queues because it allows all
		 * operations up to the point of matching or appending an item
		 * (hence potentially allowing progress by another thread) to be
		 * read-only, thus not introducing any further contention. As
		 * described below, we implement this by performing slack
		 * maintenance retries only after these points.
		 *
		 * As an accompaniment to such techniques, traversal overhead can
		 * be further reduced without increasing contention of head
		 * pointer updates: Threads may sometimes shortcut the "next" link
		 * path from the current "head" node to be closer to the currently
		 * known first unmatched node, and similarly for tail. Again, this
		 * may be triggered with using thresholds or randomization.
		 *
		 * These ideas must be further extended to avoid unbounded amounts
		 * of costly-to-reclaim garbage caused by the sequential "next"
		 * links of nodes starting at old forgotten head nodes: As first
		 * described in detail by Boehm
		 * (http://portal.acm.org/citation.cfm?doid=503272.503282) if a GC
		 * delays noticing that any arbitrarily old node has become
		 * garbage, all newer dead nodes will also be unreclaimed.
		 * (Similar issues arise in non-GC environments.)  To cope with
		 * this in our implementation, upon CASing to advance the head
		 * pointer, we set the "next" link of the previous head to point
		 * only to itself; thus limiting the length of connected dead lists.
		 * (We also take similar care to wipe out possibly garbage
		 * retaining values held in other Node fields.)  However, doing so
		 * adds some further complexity to traversal: If any "next"
		 * pointer links to itself, it indicates that the current thread
		 * has lagged behind a head-update, and so the traversal must
		 * continue from the "head".  Traversals trying to find the
		 * current tail starting from "tail" may also encounter
		 * self-links, in which case they also continue at "head".
		 *
		 * It is tempting in slack-based scheme to not even use CAS for
		 * updates (similarly to Ladan-Mozes & Shavit). However, this
		 * cannot be done for head updates under the above link-forgetting
		 * mechanics because an update may leave head at a detached node.
		 * And while direct writes are possible for tail updates, they
		 * increase the risk of long retraversals, and hence long garbage
		 * chains, which can be much more costly than is worthwhile
		 * considering that the cost difference of performing a CAS vs
		 * write is smaller when they are not triggered on each operation
		 * (especially considering that writes and CASes equally require
		 * additional GC bookkeeping ("write barriers") that are sometimes
		 * more costly than the writes themselves because of contention).
		 *
		 * *** Overview of implementation ***
		 *
		 * We use a threshold-based approach to updates, with a slack
		 * threshold of two -- that is, we update head/tail when the
		 * current pointer appears to be two or more steps away from the
		 * first/last node. The slack value is hard-wired: a path greater
		 * than one is naturally implemented by checking equality of
		 * traversal pointers except when the list has only one element,
		 * in which case we keep slack threshold at one. Avoiding tracking
		 * explicit counts across method calls slightly simplifies an
		 * already-messy implementation. Using randomization would
		 * probably work better if there were a low-quality dirt-cheap
		 * per-thread one available, but even ThreadLocalRandom is too
		 * heavy for these purposes.
		 *
		 * With such a small slack threshold value, it is not worthwhile
		 * to augment this with path short-circuiting (i.e., unsplicing
		 * interior nodes) except in the case of cancellation/removal (see
		 * below).
		 *
		 * We allow both the head and tail fields to be null before any
		 * nodes are enqueued; initializing upon first append.  This
		 * simplifies some other logic, as well as providing more
		 * efficient explicit control paths instead of letting JVMs insert
		 * implicit NullPointerExceptions when they are null.  While not
		 * currently fully implemented, we also leave open the possibility
		 * of re-nulling these fields when empty (which is complicated to
		 * arrange, for little benefit.)
		 *
		 * All enqueue/dequeue operations are handled by the single method
		 * "xfer" with parameters indicating whether to act as some form
		 * of offer, put, poll, take, or transfer (each possibly with
		 * timeout). The relative complexity of using one monolithic
		 * method outweighs the code bulk and maintenance problems of
		 * using separate methods for each case.
		 *
		 * Operation consists of up to three phases. The first is
		 * implemented within method xfer, the second in tryAppend, and
		 * the third in method awaitMatch.
		 *
		 * 1. Try to match an existing node
		 *
		 *    Starting at head, skip already-matched nodes until finding
		 *    an unmatched node of opposite mode, if one exists, in which
		 *    case matching it and returning, also if necessary updating
		 *    head to one past the matched node (or the node itself if the
		 *    list has no other unmatched nodes). If the CAS misses, then
		 *    a loop retries advancing head by two steps until either
		 *    success or the slack is at most two. By requiring that each
		 *    attempt advances head by two (if applicable), we ensure that
		 *    the slack does not grow without bound. Traversals also check
		 *    if the initial head is now off-list, in which case they
		 *    start at the new head.
		 *
		 *    If no candidates are found and the call was untimed
		 *    poll/offer, (argument "how" is NOW) return.
		 *
		 * 2. Try to append a new node (method tryAppend)
		 *
		 *    Starting at current tail pointer, find the actual last node
		 *    and try to append a new node (or if head was null, establish
		 *    the first node). Nodes can be appended only if their
		 *    predecessors are either already matched or are of the same
		 *    mode. If we detect otherwise, then a new node with opposite
		 *    mode must have been appended during traversal, so we must
		 *    restart at phase 1. The traversal and update steps are
		 *    otherwise similar to phase 1: Retrying upon CAS misses and
		 *    checking for staleness.  In particular, if a self-link is
		 *    encountered, then we can safely jump to a node on the list
		 *    by continuing the traversal at current head.
		 *
		 *    On successful append, if the call was ASYNC, return.
		 *
		 * 3. Await match or cancellation (method awaitMatch)
		 *
		 *    Wait for another thread to match node; instead cancelling if
		 *    the current thread was interrupted or the wait timed out. On
		 *    multiprocessors, we use front-of-queue spinning: If a node
		 *    appears to be the first unmatched node in the queue, it
		 *    spins a bit before blocking. In either case, before blocking
		 *    it tries to unsplice any nodes between the current "head"
		 *    and the first unmatched node.
		 *
		 *    Front-of-queue spinning vastly improves performance of
		 *    heavily contended queues. And so long as it is relatively
		 *    brief and "quiet", spinning does not much impact performance
		 *    of less-contended queues.  During spins threads check their
		 *    interrupt status and generate a thread-local random number
		 *    to decide to occasionally perform a Thread.yield. While
		 *    yield has underdefined specs, we assume that it might help,
		 *    and will not hurt, in limiting impact of spinning on busy
		 *    systems.  We also use smaller (1/2) spins for nodes that are
		 *    not known to be front but whose predecessors have not
		 *    blocked -- these "chained" spins avoid artifacts of
		 *    front-of-queue rules which otherwise lead to alternating
		 *    nodes spinning vs blocking. Further, front threads that
		 *    represent phase changes (from data to request node or vice
		 *    versa) compared to their predecessors receive additional
		 *    chained spins, reflecting longer paths typically required to
		 *    unblock threads during phase changes.
		 *
		 *
		 * ** Unlinking removed interior nodes **
		 *
		 * In addition to minimizing garbage retention via self-linking
		 * described above, we also unlink removed interior nodes. These
		 * may arise due to timed out or interrupted waits, or calls to
		 * remove(x) or Iterator.remove.  Normally, given a node that was
		 * at one time known to be the predecessor of some node s that is
		 * to be removed, we can unsplice s by CASing the next field of
		 * its predecessor if it still points to s (otherwise s must
		 * already have been removed or is now offlist). But there are two
		 * situations in which we cannot guarantee to make node s
		 * unreachable in this way: (1) If s is the trailing node of list
		 * (i.e., with null next), then it is pinned as the target node
		 * for appends, so can only be removed later after other nodes are
		 * appended. (2) We cannot necessarily unlink s given a
		 * predecessor node that is matched (including the case of being
		 * cancelled): the predecessor may already be unspliced, in which
		 * case some previous reachable node may still point to s.
		 * (For further explanation see Herlihy & Shavit "The Art of
		 * Multiprocessor Programming" chapter 9).  Although, in both
		 * cases, we can rule out the need for further action if either s
		 * or its predecessor are (or can be made to be) at, or fall off
		 * from, the head of list.
		 *
		 * Without taking these into account, it would be possible for an
		 * unbounded number of supposedly removed nodes to remain
		 * reachable.  Situations leading to such buildup are uncommon but
		 * can occur in practice; for example when a series of short timed
		 * calls to poll repeatedly time out but never otherwise fall off
		 * the list because of an untimed call to take at the front of the
		 * queue.
		 *
		 * When these cases arise, rather than always retraversing the
		 * entire list to find an actual predecessor to unlink (which
		 * won't help for case (1) anyway), we record a conservative
		 * estimate of possible unsplice failures (in "sweepVotes").
		 * We trigger a full sweep when the estimate exceeds a threshold
		 * ("SWEEP_THRESHOLD") indicating the maximum number of estimated
		 * removal failures to tolerate before sweeping through, unlinking
		 * cancelled nodes that were not unlinked upon initial removal.
		 * We perform sweeps by the thread hitting threshold (rather than
		 * background threads or by spreading work to other threads)
		 * because in the main contexts in which removal occurs, the
		 * caller is already timed-out, cancelled, or performing a
		 * potentially O(n) operation (e.g. remove(x)), none of which are
		 * time-critical enough to warrant the overhead that alternatives
		 * would impose on other threads.
		 *
		 * Because the sweepVotes estimate is conservative, and because
		 * nodes become unlinked "naturally" as they fall off the head of
		 * the queue, and because we allow votes to accumulate even while
		 * sweeps are in progress, there are typically significantly fewer
		 * such nodes than estimated.  Choice of a threshold value
		 * balances the likelihood of wasted effort and contention, versus
		 * providing a worst-case bound on retention of interior nodes in
		 * quiescent queues. The value defined below was chosen
		 * empirically to balance these under various timeout scenarios.
		 *
		 * Note that we cannot self-link unlinked interior nodes during
		 * sweeps. However, the associated garbage chains terminate when
		 * some successor ultimately falls off the head of the list and is
		 * self-linked.
		 */

		/// <summary>
		/// True if on multiprocessor </summary>
		private static readonly bool MP = Runtime.Runtime.availableProcessors() > 1;

		/// <summary>
		/// The number of times to spin (with randomly interspersed calls
		/// to Thread.yield) on multiprocessor before blocking when a node
		/// is apparently the first waiter in the queue.  See above for
		/// explanation. Must be a power of two. The value is empirically
		/// derived -- it works pretty well across a variety of processors,
		/// numbers of CPUs, and OSes.
		/// </summary>
		private static readonly int FRONT_SPINS = 1 << 7;

		/// <summary>
		/// The number of times to spin before blocking when a node is
		/// preceded by another node that is apparently spinning.  Also
		/// serves as an increment to FRONT_SPINS on phase changes, and as
		/// base average frequency for yielding during spins. Must be a
		/// power of two.
		/// </summary>
		private static readonly int CHAINED_SPINS = FRONT_SPINS >> > 1;

		/// <summary>
		/// The maximum number of estimated removal failures (sweepVotes)
		/// to tolerate before sweeping through the queue unlinking
		/// cancelled nodes that were not unlinked upon initial
		/// removal. See above for explanation. The value must be at least
		/// two to avoid useless sweeps when removing trailing nodes.
		/// </summary>
		internal const int SWEEP_THRESHOLD = 32;

		/// <summary>
		/// Queue nodes. Uses Object, not E, for items to allow forgetting
		/// them after use.  Relies heavily on Unsafe mechanics to minimize
		/// unnecessary ordering constraints: Writes that are intrinsically
		/// ordered wrt other accesses or CASes use simple relaxed forms.
		/// </summary>
		internal sealed class Node
		{
			internal readonly bool IsData; // false if this is a request node
			internal volatile Object Item; // initially non-null if isData; CASed to match
			internal volatile Node Next;
			internal volatile Thread Waiter; // null until waiting

			// CAS methods for fields
			internal bool CasNext(Node cmp, Node val)
			{
				return UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
			}

			internal bool CasItem(Object cmp, Object val)
			{
				// assert cmp == null || cmp.getClass() != Node.class;
				return UNSAFE.compareAndSwapObject(this, ItemOffset, cmp, val);
			}

			/// <summary>
			/// Constructs a new node.  Uses relaxed write because item can
			/// only be seen after publication via casNext.
			/// </summary>
			internal Node(Object item, bool isData)
			{
				UNSAFE.putObject(this, ItemOffset, item); // relaxed write
				this.IsData = isData;
			}

			/// <summary>
			/// Links node to itself to avoid garbage retention.  Called
			/// only after CASing head field, so uses relaxed write.
			/// </summary>
			internal void ForgetNext()
			{
				UNSAFE.putObject(this, NextOffset, this);
			}

			/// <summary>
			/// Sets item to self and waiter to null, to avoid garbage
			/// retention after matching or cancelling. Uses relaxed writes
			/// because order is already constrained in the only calling
			/// contexts: item is forgotten only after volatile/atomic
			/// mechanics that extract items.  Similarly, clearing waiter
			/// follows either CAS or return from park (if ever parked;
			/// else we don't care).
			/// </summary>
			internal void ForgetContents()
			{
				UNSAFE.putObject(this, ItemOffset, this);
				UNSAFE.putObject(this, WaiterOffset, null);
			}

			/// <summary>
			/// Returns true if this node has been matched, including the
			/// case of artificial matches due to cancellation.
			/// </summary>
			internal bool Matched
			{
				get
				{
					Object x = Item;
					return (x == this) || ((x == null) == IsData);
				}
			}

			/// <summary>
			/// Returns true if this is an unmatched request node.
			/// </summary>
			internal bool UnmatchedRequest
			{
				get
				{
					return !IsData && Item == null;
				}
			}

			/// <summary>
			/// Returns true if a node with the given mode cannot be
			/// appended to this node because this node is unmatched and
			/// has opposite data mode.
			/// </summary>
			internal bool CannotPrecede(bool haveData)
			{
				bool d = IsData;
				Object x;
				return d != haveData && (x = Item) != this && (x != null) == d;
			}

			/// <summary>
			/// Tries to artificially match a data node -- used by remove.
			/// </summary>
			internal bool TryMatchData()
			{
				// assert isData;
				Object x = Item;
				if (x != null && x != this && CasItem(x, null))
				{
					LockSupport.Unpark(Waiter);
					return true;
				}
				return false;
			}

			internal const long SerialVersionUID = -3375979862319811754L;

			// Unsafe mechanics
			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long ItemOffset;
			internal static readonly long NextOffset;
			internal static readonly long WaiterOffset;
			static Node()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class k = typeof(Node);
					ItemOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("item"));
					NextOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("next"));
					WaiterOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("waiter"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/// <summary>
		/// head of the queue; null until first enqueue </summary>
		[NonSerialized]
		internal volatile Node Head;

		/// <summary>
		/// tail of the queue; null until first append </summary>
		[NonSerialized]
		private volatile Node Tail;

		/// <summary>
		/// The number of apparent failures to unsplice removed nodes </summary>
		[NonSerialized]
		private volatile int SweepVotes;

		// CAS methods for fields
		private bool CasTail(Node cmp, Node val)
		{
			return UNSAFE.compareAndSwapObject(this, TailOffset, cmp, val);
		}

		private bool CasHead(Node cmp, Node val)
		{
			return UNSAFE.compareAndSwapObject(this, HeadOffset, cmp, val);
		}

		private bool CasSweepVotes(int cmp, int val)
		{
			return UNSAFE.compareAndSwapInt(this, SweepVotesOffset, cmp, val);
		}

		/*
		 * Possible values for "how" argument in xfer method.
		 */
		private const int NOW = 0; // for untimed poll, tryTransfer
		private const int ASYNC = 1; // for offer, put, add
		private const int SYNC = 2; // for transfer, take
		private const int TIMED = 3; // for timed poll, tryTransfer

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <E> E cast(Object item)
		internal static E cast<E>(Object item)
		{
			// assert item == null || item.getClass() != Node.class;
			return (E) item;
		}

		/// <summary>
		/// Implements all queuing methods. See above for explanation.
		/// </summary>
		/// <param name="e"> the item or null for take </param>
		/// <param name="haveData"> true if this is a put, else a take </param>
		/// <param name="how"> NOW, ASYNC, SYNC, or TIMED </param>
		/// <param name="nanos"> timeout in nanosecs, used only if mode is TIMED </param>
		/// <returns> an item if matched, else e </returns>
		/// <exception cref="NullPointerException"> if haveData mode but e is null </exception>
		private E Xfer(E e, bool haveData, int how, long nanos)
		{
			if (haveData && (e == null))
			{
				throw new NullPointerException();
			}
			Node s = null; // the node to append, if needed

			for (;;) // restart on append race
			{

				for (Node h = Head, p = h; p != null;) // find & match first node
				{
					bool isData = p.isData;
					Object item = p.item;
					if (item != p && (item != null) == isData) // unmatched
					{
						if (isData == haveData) // can't match
						{
							break;
						}
						if (p.casItem(item, e)) // match
						{
							for (Node q = p; q != h;)
							{
								Node n = q.Next; // update by 2 unless singleton
								if (Head == h && CasHead(h, n == null ? q : n))
								{
									h.ForgetNext();
									break;
								} // advance and retry
								if ((h = Head) == null || (q = h.Next) == null || !q.Matched)
								{
									break; // unless slack < 2
								}
							}
							LockSupport.Unpark(p.waiter);
							return LinkedTransferQueue.Cast<E>(item);
						}
					}
					Node n = p.next;
					p = (p != n) ? n : (h = Head); // Use head if p offlist
				}

				if (how != NOW) // No matches available
				{
					if (s == null)
					{
						s = new Node(e, haveData);
					}
					Node pred = TryAppend(s, haveData);
					if (pred == null)
					{
						goto retryContinue; // lost race vs opposite mode
					}
					if (how != ASYNC)
					{
						return AwaitMatch(s, pred, e, (how == TIMED), nanos);
					}
				}
				return e; // not waiting
				retryContinue:;
			}
			retryBreak:;
		}

		/// <summary>
		/// Tries to append node s as tail.
		/// </summary>
		/// <param name="s"> the node to append </param>
		/// <param name="haveData"> true if appending in data mode </param>
		/// <returns> null on failure due to losing race with append in
		/// different mode, else s's predecessor, or s itself if no
		/// predecessor </returns>
		private Node TryAppend(Node s, bool haveData)
		{
			for (Node t = Tail, p = t;;) // move p to last node and append
			{
				Node n, u; // temps for reads of next & tail
				if (p == null && (p = Head) == null)
				{
					if (CasHead(null, s))
					{
						return s; // initialize
					}
				}
				else if (p.cannotPrecede(haveData))
				{
					return null; // lost race vs opposite mode
				}
				else if ((n = p.next) != null) // not last; keep traversing
				{
					p = p != t && t != (u = Tail) ? (t = u) : (p != n) ? n : null; // restart if off list -  stale tail
				}
				else if (!p.casNext(null, s))
				{
					p = p.next; // re-read on CAS failure
				}
				else
				{
					if (p != t) // update if slack now >= 2
					{
						while ((Tail != t || !CasTail(t, s)) && (t = Tail) != null && (s = t.next) != null && (s = s.Next) != null && s != t); // advance and retry
					}
					return p;
				}
			}
		}

		/// <summary>
		/// Spins/yields/blocks until node s is matched or caller gives up.
		/// </summary>
		/// <param name="s"> the waiting node </param>
		/// <param name="pred"> the predecessor of s, or s itself if it has no
		/// predecessor, or null if unknown (the null case does not occur
		/// in any current calls but may in possible future extensions) </param>
		/// <param name="e"> the comparison value for checking match </param>
		/// <param name="timed"> if true, wait only until timeout elapses </param>
		/// <param name="nanos"> timeout in nanosecs, used only if timed is true </param>
		/// <returns> matched item, or e if unmatched on interrupt or timeout </returns>
		private E AwaitMatch(Node s, Node pred, E e, bool timed, long nanos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = timed ? System.nanoTime() + nanos : 0L;
			long deadline = timed ? System.nanoTime() + nanos : 0L;
			Thread w = Thread.CurrentThread;
			int spins = -1; // initialized after first item and cancel checks
			ThreadLocalRandom randomYields = null; // bound if needed

			for (;;)
			{
				Object item = s.Item;
				if (item != e) // matched
				{
					// assert item != s;
					s.ForgetContents(); // avoid garbage
					return LinkedTransferQueue.Cast<E>(item);
				}
				if ((w.Interrupted || (timed && nanos <= 0)) && s.CasItem(e, s)) // cancel
				{
					Unsplice(pred, s);
					return e;
				}

				if (spins < 0) // establish spins at/near front
				{
					if ((spins = SpinsFor(pred, s.IsData)) > 0)
					{
						randomYields = ThreadLocalRandom.Current();
					}
				}
				else if (spins > 0) // spin
				{
					--spins;
					if (randomYields.NextInt(CHAINED_SPINS) == 0)
					{
						Thread.@yield(); // occasionally yield
					}
				}
				else if (s.Waiter == null)
				{
					s.Waiter = w; // request unpark then recheck
				}
				else if (timed)
				{
					nanos = deadline - System.nanoTime();
					if (nanos > 0L)
					{
						LockSupport.ParkNanos(this, nanos);
					}
				}
				else
				{
					LockSupport.Park(this);
				}
			}
		}

		/// <summary>
		/// Returns spin/yield value for a node with given predecessor and
		/// data mode. See above for explanation.
		/// </summary>
		private static int SpinsFor(Node pred, bool haveData)
		{
			if (MP && pred != null)
			{
				if (pred.IsData != haveData) // phase change
				{
					return FRONT_SPINS + CHAINED_SPINS;
				}
				if (pred.Matched) // probably at front
				{
					return FRONT_SPINS;
				}
				if (pred.Waiter == null) // pred apparently spinning
				{
					return CHAINED_SPINS;
				}
			}
			return 0;
		}

		/* -------------- Traversal methods -------------- */

		/// <summary>
		/// Returns the successor of p, or the head node if p.next has been
		/// linked to self, which will only be true if traversing with a
		/// stale pointer that is now off the list.
		/// </summary>
		internal Node Succ(Node p)
		{
			Node next = p.Next;
			return (p == next) ? Head : next;
		}

		/// <summary>
		/// Returns the first unmatched node of the given mode, or null if
		/// none.  Used by methods isEmpty, hasWaitingConsumer.
		/// </summary>
		private Node FirstOfMode(bool isData)
		{
			for (Node p = Head; p != null; p = Succ(p))
			{
				if (!p.Matched)
				{
					return (p.IsData == isData) ? p : null;
				}
			}
			return null;
		}

		/// <summary>
		/// Version of firstOfMode used by Spliterator. Callers must
		/// recheck if the returned node's item field is null or
		/// self-linked before using.
		/// </summary>
		internal Node FirstDataNode()
		{
			for (Node p = Head; p != null;)
			{
				Object item = p.Item;
				if (p.IsData)
				{
					if (item != null && item != p)
					{
						return p;
					}
				}
				else if (item == null)
				{
					break;
				}
				if (p == (p = p.next))
				{
					p = Head;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the item in the first unmatched node with isData; or
		/// null if none.  Used by peek.
		/// </summary>
		private E FirstDataItem()
		{
			for (Node p = Head; p != null; p = Succ(p))
			{
				Object item = p.Item;
				if (p.IsData)
				{
					if (item != null && item != p)
					{
						return LinkedTransferQueue.Cast<E>(item);
					}
				}
				else if (item == null)
				{
					return null;
				}
			}
			return null;
		}

		/// <summary>
		/// Traverses and counts unmatched nodes of the given mode.
		/// Used by methods size and getWaitingConsumerCount.
		/// </summary>
		private int CountOfMode(bool data)
		{
			int count = 0;
			for (Node p = Head; p != null;)
			{
				if (!p.Matched)
				{
					if (p.IsData != data)
					{
						return 0;
					}
					if (++count == Integer.MaxValue) // saturated
					{
						break;
					}
				}
				Node n = p.next;
				if (n != p)
				{
					p = n;
				}
				else
				{
					count = 0;
					p = Head;
				}
			}
			return count;
		}

		internal sealed class Itr : Iterator<E>
		{
			private readonly LinkedTransferQueue<E> OuterInstance;

			internal Node NextNode; // next node to return item for
			internal E NextItem; // the corresponding item
			internal Node LastRet; // last returned node, to support remove
			internal Node LastPred; // predecessor to unlink lastRet

			/// <summary>
			/// Moves to next node after prev, or first node if prev null.
			/// </summary>
			internal void Advance(Node prev)
			{
				/*
				 * To track and avoid buildup of deleted nodes in the face
				 * of calls to both Queue.remove and Itr.remove, we must
				 * include variants of unsplice and sweep upon each
				 * advance: Upon Itr.remove, we may need to catch up links
				 * from lastPred, and upon other removes, we might need to
				 * skip ahead from stale nodes and unsplice deleted ones
				 * found while advancing.
				 */

				Node r, b; // reset lastPred upon possible deletion of lastRet
				if ((r = LastRet) != null && !r.Matched)
				{
					LastPred = r; // next lastPred is old lastRet
				}
				else if ((b = LastPred) == null || b.Matched)
				{
					LastPred = null; // at start of list
				}
				else
				{
					Node s, n; // help with removal of lastPred.next
					while ((s = b.Next) != null && s != b && s.Matched && (n = s.Next) != null && n != s)
					{
						b.CasNext(s, n);
					}
				}

				this.LastRet = prev;

				for (Node p = prev, s, n;;)
				{
					s = (p == null) ? outerInstance.Head : p.Next;
					if (s == null)
					{
						break;
					}
					else if (s == p)
					{
						p = null;
						continue;
					}
					Object item = s.item;
					if (s.isData)
					{
						if (item != null && item != s)
						{
							NextItem = LinkedTransferQueue.Cast<E>(item);
							NextNode = s;
							return;
						}
					}
					else if (item == null)
					{
						break;
					}
					// assert s.isMatched();
					if (p == null)
					{
						p = s;
					}
					else if ((n = s.next) == null)
					{
						break;
					}
					else if (s == n)
					{
						p = null;
					}
					else
					{
						p.casNext(s, n);
					}
				}
				NextNode = null;
				NextItem = null;
			}

			internal Itr(LinkedTransferQueue<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Advance(null);
			}

			public bool HasNext()
			{
				return NextNode != null;
			}

			public E Next()
			{
				Node p = NextNode;
				if (p == null)
				{
					throw new NoSuchElementException();
				}
				E e = NextItem;
				Advance(p);
				return e;
			}

			public void Remove()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node lastRet = this.lastRet;
				Node lastRet = this.LastRet;
				if (lastRet == null)
				{
					throw new IllegalStateException();
				}
				this.LastRet = null;
				if (lastRet.TryMatchData())
				{
					outerInstance.Unsplice(LastPred, lastRet);
				}
			}
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class LTQSpliterator<E> : Spliterator<E>
		{
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly LinkedTransferQueue<E> Queue;
			internal Node Current; // current node; null until initialized
			internal int Batch; // batch size for splits
			internal bool Exhausted; // true when no more nodes
			internal LTQSpliterator(LinkedTransferQueue<E> queue)
			{
				this.Queue = queue;
			}

			public Spliterator<E> TrySplit()
			{
				Node p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedTransferQueue<E> q = this.queue;
				LinkedTransferQueue<E> q = this.Queue;
				int b = Batch;
				int n = (b <= 0) ? 1 : (b >= MAX_BATCH) ? MAX_BATCH : b + 1;
				if (!Exhausted && ((p = Current) != null || (p = q.FirstDataNode()) != null) && p.Next != null)
				{
					Object[] a = new Object[n];
					int i = 0;
					do
					{
						Object e = p.Item;
						if (e != p && (a[i] = e) != null)
						{
							++i;
						}
						if (p == (p = p.Next))
						{
							p = q.FirstDataNode();
						}
					} while (p != null && i < n && p.IsData);
					if ((Current = p) == null)
					{
						Exhausted = true;
					}
					if (i > 0)
					{
						Batch = i;
						return Spliterators.Spliterator(a, 0, i, java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.CONCURRENT);
					}
				}
				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				Node p;
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedTransferQueue<E> q = this.queue;
				LinkedTransferQueue<E> q = this.Queue;
				if (!Exhausted && ((p = Current) != null || (p = q.FirstDataNode()) != null))
				{
					Exhausted = true;
					do
					{
						Object e = p.Item;
						if (e != null && e != p)
						{
							action.Accept((E)e);
						}
						if (p == (p = p.Next))
						{
							p = q.FirstDataNode();
						}
					} while (p != null && p.IsData);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean tryAdvance(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				Node p;
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedTransferQueue<E> q = this.queue;
				LinkedTransferQueue<E> q = this.Queue;
				if (!Exhausted && ((p = Current) != null || (p = q.FirstDataNode()) != null))
				{
					Object e;
					do
					{
						if ((e = p.Item) == p)
						{
							e = null;
						}
						if (p == (p = p.Next))
						{
							p = q.FirstDataNode();
						}
					} while (e == null && p != null && p.IsData);
					if ((Current = p) == null)
					{
						Exhausted = true;
					}
					if (e != null)
					{
						action.Accept((E)e);
						return true;
					}
				}
				return false;
			}

			public long EstimateSize()
			{
				return Long.MaxValue;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.CONCURRENT;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> over the elements in this queue.
		/// 
		/// <para>The returned spliterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#ORDERED"/>, and <seealso cref="Spliterator#NONNULL"/>.
		/// 
		/// @implNote
		/// The {@code Spliterator} implements {@code trySplit} to permit limited
		/// parallelism.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this queue
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return new LTQSpliterator<E>(this);
		}

		/* -------------- Removal methods -------------- */

		/// <summary>
		/// Unsplices (now or later) the given deleted/cancelled node with
		/// the given predecessor.
		/// </summary>
		/// <param name="pred"> a node that was at one time known to be the
		/// predecessor of s, or null or s itself if s is/was at head </param>
		/// <param name="s"> the node to be unspliced </param>
		internal void Unsplice(Node pred, Node s)
		{
			s.ForgetContents(); // forget unneeded fields
			/*
			 * See above for rationale. Briefly: if pred still points to
			 * s, try to unlink s.  If s cannot be unlinked, because it is
			 * trailing node or pred might be unlinked, and neither pred
			 * nor s are head or offlist, add to sweepVotes, and if enough
			 * votes have accumulated, sweep.
			 */
			if (pred != null && pred != s && pred.Next == s)
			{
				Node n = s.Next;
				if (n == null || (n != s && pred.CasNext(s, n) && pred.Matched))
				{
					for (;;) // check if at, or could be, head
					{
						Node h = Head;
						if (h == pred || h == s || h == null)
						{
							return; // at head or list empty
						}
						if (!h.Matched)
						{
							break;
						}
						Node hn = h.Next;
						if (hn == null)
						{
							return; // now empty
						}
						if (hn != h && CasHead(h, hn))
						{
							h.ForgetNext(); // advance head
						}
					}
					if (pred.Next != pred && s.Next != s) // recheck if offlist
					{
						for (;;) // sweep now if enough votes
						{
							int v = SweepVotes;
							if (v < SWEEP_THRESHOLD)
							{
								if (CasSweepVotes(v, v + 1))
								{
									break;
								}
							}
							else if (CasSweepVotes(v, 0))
							{
								Sweep();
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Unlinks matched (typically cancelled) nodes encountered in a
		/// traversal from head.
		/// </summary>
		private void Sweep()
		{
			for (Node p = Head, s, n; p != null && (s = p.Next) != null;)
			{
				if (!s.Matched)
				{
					// Unmatched nodes are never self-linked
					p = s;
				}
				else if ((n = s.next) == null) // trailing node is pinned
				{
					break;
				}
				else if (s == n) // stale
				{
					// No need to also check for p == s, since that implies s == n
					p = Head;
				}
				else
				{
					p.casNext(s, n);
				}
			}
		}

		/// <summary>
		/// Main implementation of remove(Object)
		/// </summary>
		private bool FindAndRemove(Object e)
		{
			if (e != null)
			{
				for (Node pred = null, p = Head; p != null;)
				{
					Object item = p.item;
					if (p.isData)
					{
						if (item != null && item != p && e.Equals(item) && p.tryMatchData())
						{
							Unsplice(pred, p);
							return true;
						}
					}
					else if (item == null)
					{
						break;
					}
					pred = p;
					if ((p = p.next) == pred) // stale
					{
						pred = null;
						p = Head;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Creates an initially empty {@code LinkedTransferQueue}.
		/// </summary>
		public LinkedTransferQueue()
		{
		}

		/// <summary>
		/// Creates a {@code LinkedTransferQueue}
		/// initially containing the elements of the given collection,
		/// added in traversal order of the collection's iterator.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public LinkedTransferQueue<T1>(ICollection<T1> c) where T1 : E : this()
		{
			AddAll(c);
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never block.
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void Put(E e)
		{
			Xfer(e, true, ASYNC, 0);
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never block or
		/// return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by
		///  {@link java.util.concurrent.BlockingQueue#offer(Object,long,TimeUnit)
		///  BlockingQueue.offer}) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{
			Xfer(e, true, ASYNC, 0);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			Xfer(e, true, ASYNC, 0);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never throw
		/// <seealso cref="IllegalStateException"/> or return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			Xfer(e, true, ASYNC, 0);
			return true;
		}

		/// <summary>
		/// Transfers the element to a waiting consumer immediately, if possible.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// otherwise returning {@code false} without enqueuing the element.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool TryTransfer(E e)
		{
			return Xfer(e, true, NOW, 0) == null;
		}

		/// <summary>
		/// Transfers the element to a consumer, waiting if necessary to do so.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// else inserts the specified element at the tail of this queue
		/// and waits until the element is received by a consumer.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transfer(E e) throws InterruptedException
		public virtual void Transfer(E e)
		{
			if (Xfer(e, true, SYNC, 0) != null)
			{
				Thread.Interrupted(); // failure possible only due to interrupt
				throw new InterruptedException();
			}
		}

		/// <summary>
		/// Transfers the element to a consumer if it is possible to do so
		/// before the timeout elapses.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// else inserts the specified element at the tail of this queue
		/// and waits until the element is received by a consumer,
		/// returning {@code false} if the specified wait time elapses
		/// before the element can be transferred.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tryTransfer(E e, long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public virtual bool TryTransfer(E e, long timeout, TimeUnit unit)
		{
			if (Xfer(e, true, TIMED, unit.ToNanos(timeout)) == null)
			{
				return true;
			}
			if (!Thread.Interrupted())
			{
				return false;
			}
			throw new InterruptedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E take() throws InterruptedException
		public virtual E Take()
		{
			E e = Xfer(null, false, SYNC, 0);
			if (e != null)
			{
				return e;
			}
			Thread.Interrupted();
			throw new InterruptedException();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E poll(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public virtual E Poll(long timeout, TimeUnit unit)
		{
			E e = Xfer(null, false, TIMED, unit.ToNanos(timeout));
			if (e != null || !Thread.Interrupted())
			{
				return e;
			}
			throw new InterruptedException();
		}

		public virtual E Poll()
		{
			return Xfer(null, false, NOW, 0);
		}

		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(java.util.Collection<? base E> c)
		public virtual int drainTo<T1>(ICollection<T1> c)
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

		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(java.util.Collection<? base E> c, int maxElements)
		public virtual int drainTo<T1>(ICollection<T1> c, int maxElements)
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

		/// <summary>
		/// Returns an iterator over the elements in this queue in proper sequence.
		/// The elements will be returned in order from first (head) to last (tail).
		/// 
		/// <para>The returned iterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this queue in proper sequence </returns>
		public virtual IEnumerator<E> Iterator()
		{
			return new Itr(this);
		}

		public virtual E Peek()
		{
			return FirstDataItem();
		}

		/// <summary>
		/// Returns {@code true} if this queue contains no elements.
		/// </summary>
		/// <returns> {@code true} if this queue contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				for (Node p = Head; p != null; p = Succ(p))
				{
					if (!p.Matched)
					{
						return !p.IsData;
					}
				}
				return true;
			}
		}

		public virtual bool HasWaitingConsumer()
		{
			return FirstOfMode(false) != null;
		}

		/// <summary>
		/// Returns the number of elements in this queue.  If this queue
		/// contains more than {@code Integer.MAX_VALUE} elements, returns
		/// {@code Integer.MAX_VALUE}.
		/// 
		/// <para>Beware that, unlike in most collections, this method is
		/// <em>NOT</em> a constant-time operation. Because of the
		/// asynchronous nature of these queues, determining the current
		/// number of elements requires an O(n) traversal.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of elements in this queue </returns>
		public virtual int Size()
		{
			return CountOfMode(true);
		}

		public virtual int WaitingConsumerCount
		{
			get
			{
				return CountOfMode(false);
			}
		}

		/// <summary>
		/// Removes a single instance of the specified element from this queue,
		/// if it is present.  More formally, removes an element {@code e} such
		/// that {@code o.equals(e)}, if this queue contains one or more such
		/// elements.
		/// Returns {@code true} if this queue contained the specified element
		/// (or equivalently, if this queue changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this queue, if present </param>
		/// <returns> {@code true} if this queue changed as a result of the call </returns>
		public virtual bool Remove(Object o)
		{
			return FindAndRemove(o);
		}

		/// <summary>
		/// Returns {@code true} if this queue contains the specified element.
		/// More formally, returns {@code true} if and only if this queue contains
		/// at least one element {@code e} such that {@code o.equals(e)}.
		/// </summary>
		/// <param name="o"> object to be checked for containment in this queue </param>
		/// <returns> {@code true} if this queue contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			if (o == null)
			{
				return false;
			}
			for (Node p = Head; p != null; p = Succ(p))
			{
				Object item = p.Item;
				if (p.IsData)
				{
					if (item != null && item != p && o.Equals(item))
					{
						return true;
					}
				}
				else if (item == null)
				{
					break;
				}
			}
			return false;
		}

		/// <summary>
		/// Always returns {@code Integer.MAX_VALUE} because a
		/// {@code LinkedTransferQueue} is not capacity constrained.
		/// </summary>
		/// <returns> {@code Integer.MAX_VALUE} (as specified by
		///         {@link java.util.concurrent.BlockingQueue#remainingCapacity()
		///         BlockingQueue.remainingCapacity}) </returns>
		public virtual int RemainingCapacity()
		{
			return Integer.MaxValue;
		}

		/// <summary>
		/// Saves this queue to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData All of the elements (each an {@code E}) in
		/// the proper order, followed by a null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject();
			foreach (E e in this)
			{
				s.WriteObject(e);
			}
			// Use trailing null as sentinel
			s.WriteObject(null);
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
			for (;;)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E item = (E) s.readObject();
				E item = (E) s.ReadObject();
				if (item == null)
				{
					break;
				}
				else
				{
					Offer(item);
				}
			}
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long HeadOffset;
		private static readonly long TailOffset;
		private static readonly long SweepVotesOffset;
		static LinkedTransferQueue()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(LinkedTransferQueue);
				HeadOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("head"));
				TailOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("tail"));
				SweepVotesOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("sweepVotes"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}