using System;
using System.Collections;
using System.Collections.Generic;

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
 * Written by Doug Lea and Martin Buchholz with assistance from members of
 * JCP JSR-166 Expert Group and released to the public domain, as explained
 * at http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{


	/// <summary>
	/// An unbounded concurrent <seealso cref="Deque deque"/> based on linked nodes.
	/// Concurrent insertion, removal, and access operations execute safely
	/// across multiple threads.
	/// A {@code ConcurrentLinkedDeque} is an appropriate choice when
	/// many threads will share access to a common collection.
	/// Like most other concurrent collection implementations, this class
	/// does not permit the use of {@code null} elements.
	/// 
	/// <para>Iterators and spliterators are
	/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	/// 
	/// </para>
	/// <para>Beware that, unlike in most collections, the {@code size} method
	/// is <em>NOT</em> a constant-time operation. Because of the
	/// asynchronous nature of these deques, determining the current number
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
	/// <para>This class and its iterator implement all of the <em>optional</em>
	/// methods of the <seealso cref="Deque"/> and <seealso cref="Iterator"/> interfaces.
	/// 
	/// </para>
	/// <para>Memory consistency effects: As with other concurrent collections,
	/// actions in a thread prior to placing an object into a
	/// {@code ConcurrentLinkedDeque}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that element from
	/// the {@code ConcurrentLinkedDeque} in another thread.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// @author Martin Buchholz
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class ConcurrentLinkedDeque<E> : AbstractCollection<E>, Deque<E>
	{

		/*
		 * This is an implementation of a concurrent lock-free deque
		 * supporting interior removes but not interior insertions, as
		 * required to support the entire Deque interface.
		 *
		 * We extend the techniques developed for ConcurrentLinkedQueue and
		 * LinkedTransferQueue (see the internal docs for those classes).
		 * Understanding the ConcurrentLinkedQueue implementation is a
		 * prerequisite for understanding the implementation of this class.
		 *
		 * The data structure is a symmetrical doubly-linked "GC-robust"
		 * linked list of nodes.  We minimize the number of volatile writes
		 * using two techniques: advancing multiple hops with a single CAS
		 * and mixing volatile and non-volatile writes of the same memory
		 * locations.
		 *
		 * A node contains the expected E ("item") and links to predecessor
		 * ("prev") and successor ("next") nodes:
		 *
		 * class Node<E> { volatile Node<E> prev, next; volatile E item; }
		 *
		 * A node p is considered "live" if it contains a non-null item
		 * (p.item != null).  When an item is CASed to null, the item is
		 * atomically logically deleted from the collection.
		 *
		 * At any time, there is precisely one "first" node with a null
		 * prev reference that terminates any chain of prev references
		 * starting at a live node.  Similarly there is precisely one
		 * "last" node terminating any chain of next references starting at
		 * a live node.  The "first" and "last" nodes may or may not be live.
		 * The "first" and "last" nodes are always mutually reachable.
		 *
		 * A new element is added atomically by CASing the null prev or
		 * next reference in the first or last node to a fresh node
		 * containing the element.  The element's node atomically becomes
		 * "live" at that point.
		 *
		 * A node is considered "active" if it is a live node, or the
		 * first or last node.  Active nodes cannot be unlinked.
		 *
		 * A "self-link" is a next or prev reference that is the same node:
		 *   p.prev == p  or  p.next == p
		 * Self-links are used in the node unlinking process.  Active nodes
		 * never have self-links.
		 *
		 * A node p is active if and only if:
		 *
		 * p.item != null ||
		 * (p.prev == null && p.next != p) ||
		 * (p.next == null && p.prev != p)
		 *
		 * The deque object has two node references, "head" and "tail".
		 * The head and tail are only approximations to the first and last
		 * nodes of the deque.  The first node can always be found by
		 * following prev pointers from head; likewise for tail.  However,
		 * it is permissible for head and tail to be referring to deleted
		 * nodes that have been unlinked and so may not be reachable from
		 * any live node.
		 *
		 * There are 3 stages of node deletion;
		 * "logical deletion", "unlinking", and "gc-unlinking".
		 *
		 * 1. "logical deletion" by CASing item to null atomically removes
		 * the element from the collection, and makes the containing node
		 * eligible for unlinking.
		 *
		 * 2. "unlinking" makes a deleted node unreachable from active
		 * nodes, and thus eventually reclaimable by GC.  Unlinked nodes
		 * may remain reachable indefinitely from an iterator.
		 *
		 * Physical node unlinking is merely an optimization (albeit a
		 * critical one), and so can be performed at our convenience.  At
		 * any time, the set of live nodes maintained by prev and next
		 * links are identical, that is, the live nodes found via next
		 * links from the first node is equal to the elements found via
		 * prev links from the last node.  However, this is not true for
		 * nodes that have already been logically deleted - such nodes may
		 * be reachable in one direction only.
		 *
		 * 3. "gc-unlinking" takes unlinking further by making active
		 * nodes unreachable from deleted nodes, making it easier for the
		 * GC to reclaim future deleted nodes.  This step makes the data
		 * structure "gc-robust", as first described in detail by Boehm
		 * (http://portal.acm.org/citation.cfm?doid=503272.503282).
		 *
		 * GC-unlinked nodes may remain reachable indefinitely from an
		 * iterator, but unlike unlinked nodes, are never reachable from
		 * head or tail.
		 *
		 * Making the data structure GC-robust will eliminate the risk of
		 * unbounded memory retention with conservative GCs and is likely
		 * to improve performance with generational GCs.
		 *
		 * When a node is dequeued at either end, e.g. via poll(), we would
		 * like to break any references from the node to active nodes.  We
		 * develop further the use of self-links that was very effective in
		 * other concurrent collection classes.  The idea is to replace
		 * prev and next pointers with special values that are interpreted
		 * to mean off-the-list-at-one-end.  These are approximations, but
		 * good enough to preserve the properties we want in our
		 * traversals, e.g. we guarantee that a traversal will never visit
		 * the same element twice, but we don't guarantee whether a
		 * traversal that runs out of elements will be able to see more
		 * elements later after enqueues at that end.  Doing gc-unlinking
		 * safely is particularly tricky, since any node can be in use
		 * indefinitely (for example by an iterator).  We must ensure that
		 * the nodes pointed at by head/tail never get gc-unlinked, since
		 * head/tail are needed to get "back on track" by other nodes that
		 * are gc-unlinked.  gc-unlinking accounts for much of the
		 * implementation complexity.
		 *
		 * Since neither unlinking nor gc-unlinking are necessary for
		 * correctness, there are many implementation choices regarding
		 * frequency (eagerness) of these operations.  Since volatile
		 * reads are likely to be much cheaper than CASes, saving CASes by
		 * unlinking multiple adjacent nodes at a time may be a win.
		 * gc-unlinking can be performed rarely and still be effective,
		 * since it is most important that long chains of deleted nodes
		 * are occasionally broken.
		 *
		 * The actual representation we use is that p.next == p means to
		 * goto the first node (which in turn is reached by following prev
		 * pointers from head), and p.next == null && p.prev == p means
		 * that the iteration is at an end and that p is a (static final)
		 * dummy node, NEXT_TERMINATOR, and not the last active node.
		 * Finishing the iteration when encountering such a TERMINATOR is
		 * good enough for read-only traversals, so such traversals can use
		 * p.next == null as the termination condition.  When we need to
		 * find the last (active) node, for enqueueing a new node, we need
		 * to check whether we have reached a TERMINATOR node; if so,
		 * restart traversal from tail.
		 *
		 * The implementation is completely directionally symmetrical,
		 * except that most public methods that iterate through the list
		 * follow next pointers ("forward" direction).
		 *
		 * We believe (without full proof) that all single-element deque
		 * operations (e.g., addFirst, peekLast, pollLast) are linearizable
		 * (see Herlihy and Shavit's book).  However, some combinations of
		 * operations are known not to be linearizable.  In particular,
		 * when an addFirst(A) is racing with pollFirst() removing B, it is
		 * possible for an observer iterating over the elements to observe
		 * A B C and subsequently observe A C, even though no interior
		 * removes are ever performed.  Nevertheless, iterators behave
		 * reasonably, providing the "weakly consistent" guarantees.
		 *
		 * Empirically, microbenchmarks suggest that this class adds about
		 * 40% overhead relative to ConcurrentLinkedQueue, which feels as
		 * good as we can hope for.
		 */

		private const long SerialVersionUID = 876323262645176354L;

		/// <summary>
		/// A node from which the first node on list (that is, the unique node p
		/// with p.prev == null && p.next != p) can be reached in O(1) time.
		/// Invariants:
		/// - the first node is always O(1) reachable from head via prev links
		/// - all live nodes are reachable from the first node via succ()
		/// - head != null
		/// - (tmp = head).next != tmp || tmp != head
		/// - head is never gc-unlinked (but may be unlinked)
		/// Non-invariants:
		/// - head.item may or may not be null
		/// - head may not be reachable from the first or last node, or from tail
		/// </summary>
		[NonSerialized]
		private volatile Node<E> Head;

		/// <summary>
		/// A node from which the last node on list (that is, the unique node p
		/// with p.next == null && p.prev != p) can be reached in O(1) time.
		/// Invariants:
		/// - the last node is always O(1) reachable from tail via next links
		/// - all live nodes are reachable from the last node via pred()
		/// - tail != null
		/// - tail is never gc-unlinked (but may be unlinked)
		/// Non-invariants:
		/// - tail.item may or may not be null
		/// - tail may not be reachable from the first or last node, or from head
		/// </summary>
		[NonSerialized]
		private volatile Node<E> Tail;

		private static readonly Node<Object> PREV_TERMINATOR, NEXT_TERMINATOR;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<E> prevTerminator()
		internal virtual Node<E> PrevTerminator()
		{
			return (Node<E>) PREV_TERMINATOR;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<E> nextTerminator()
		internal virtual Node<E> NextTerminator()
		{
			return (Node<E>) NEXT_TERMINATOR;
		}

		internal sealed class Node<E>
		{
			internal volatile Node<E> Prev;
			internal volatile E Item;
			internal volatile Node<E> Next;

			internal Node() // default constructor for NEXT_TERMINATOR, PREV_TERMINATOR
			{
			}

			/// <summary>
			/// Constructs a new node.  Uses relaxed write because item can
			/// only be seen after publication via casNext or casPrev.
			/// </summary>
			internal Node(E item)
			{
				UNSAFE.putObject(this, ItemOffset, item);
			}

			internal bool CasItem(E cmp, E val)
			{
				return UNSAFE.compareAndSwapObject(this, ItemOffset, cmp, val);
			}

			internal void LazySetNext(Node<E> val)
			{
				UNSAFE.putOrderedObject(this, NextOffset, val);
			}

			internal bool CasNext(Node<E> cmp, Node<E> val)
			{
				return UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
			}

			internal void LazySetPrev(Node<E> val)
			{
				UNSAFE.putOrderedObject(this, PrevOffset, val);
			}

			internal bool CasPrev(Node<E> cmp, Node<E> val)
			{
				return UNSAFE.compareAndSwapObject(this, PrevOffset, cmp, val);
			}

			// Unsafe mechanics

			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long PrevOffset;
			internal static readonly long ItemOffset;
			internal static readonly long NextOffset;

			static Node()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class k = typeof(Node);
					PrevOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("prev"));
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
		/// Links e as first element.
		/// </summary>
		private void LinkFirst(E e)
		{
			CheckNotNull(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<E>(e);
			Node<E> newNode = new Node<E>(e);

			for (;;)
			{
				for (Node<E> h = Head, p = h, q;;)
				{
					if ((q = p.prev) != null && (q = (p = q).prev) != null)
						// Check for head updates every other hop.
						// If p == q, we are sure to follow head instead.
					{
						p = (h != (h = Head)) ? h : q;
					}
					else if (p.next == p) // PREV_TERMINATOR
					{
						goto restartFromHeadContinue;
					}
					else
					{
						// p is first node
						newNode.LazySetNext(p); // CAS piggyback
						if (p.casPrev(null, newNode))
						{
							// Successful CAS is the linearization point
							// for e to become an element of this deque,
							// and for newNode to become "live".
							if (p != h) // hop two nodes at a time
							{
								CasHead(h, newNode); // Failure is OK.
							}
							return;
						}
						// Lost CAS race to another thread; re-read prev
					}
				}
				restartFromHeadContinue:;
			}
			restartFromHeadBreak:;
		}

		/// <summary>
		/// Links e as last element.
		/// </summary>
		private void LinkLast(E e)
		{
			CheckNotNull(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<E>(e);
			Node<E> newNode = new Node<E>(e);

			for (;;)
			{
				for (Node<E> t = Tail, p = t, q;;)
				{
					if ((q = p.next) != null && (q = (p = q).next) != null)
						// Check for tail updates every other hop.
						// If p == q, we are sure to follow tail instead.
					{
						p = (t != (t = Tail)) ? t : q;
					}
					else if (p.prev == p) // NEXT_TERMINATOR
					{
						goto restartFromTailContinue;
					}
					else
					{
						// p is last node
						newNode.LazySetPrev(p); // CAS piggyback
						if (p.casNext(null, newNode))
						{
							// Successful CAS is the linearization point
							// for e to become an element of this deque,
							// and for newNode to become "live".
							if (p != t) // hop two nodes at a time
							{
								CasTail(t, newNode); // Failure is OK.
							}
							return;
						}
						// Lost CAS race to another thread; re-read next
					}
				}
				restartFromTailContinue:;
			}
			restartFromTailBreak:;
		}

		private const int HOPS = 2;

		/// <summary>
		/// Unlinks non-null node x.
		/// </summary>
		internal virtual void Unlink(Node<E> x)
		{
			// assert x != null;
			// assert x.item == null;
			// assert x != PREV_TERMINATOR;
			// assert x != NEXT_TERMINATOR;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> prev = x.prev;
			Node<E> prev = x.Prev;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> next = x.next;
			Node<E> next = x.Next;
			if (prev == null)
			{
				UnlinkFirst(x, next);
			}
			else if (next == null)
			{
				UnlinkLast(x, prev);
			}
			else
			{
				// Unlink interior node.
				//
				// This is the common case, since a series of polls at the
				// same end will be "interior" removes, except perhaps for
				// the first one, since end nodes cannot be unlinked.
				//
				// At any time, all active nodes are mutually reachable by
				// following a sequence of either next or prev pointers.
				//
				// Our strategy is to find the unique active predecessor
				// and successor of x.  Try to fix up their links so that
				// they point to each other, leaving x unreachable from
				// active nodes.  If successful, and if x has no live
				// predecessor/successor, we additionally try to gc-unlink,
				// leaving active nodes unreachable from x, by rechecking
				// that the status of predecessor and successor are
				// unchanged and ensuring that x is not reachable from
				// tail/head, before setting x's prev/next links to their
				// logical approximate replacements, self/TERMINATOR.
				Node<E> activePred, activeSucc;
				bool isFirst, isLast;
				int hops = 1;

				// Find active predecessor
				for (Node<E> p = prev; ; ++hops)
				{
					if (p.Item != null)
					{
						activePred = p;
						isFirst = false;
						break;
					}
					Node<E> q = p.prev;
					if (q == null)
					{
						if (p.next == p)
						{
							return;
						}
						activePred = p;
						isFirst = true;
						break;
					}
					else if (p == q)
					{
						return;
					}
					else
					{
						p = q;
					}
				}

				// Find active successor
				for (Node<E> p = next; ; ++hops)
				{
					if (p.Item != null)
					{
						activeSucc = p;
						isLast = false;
						break;
					}
					Node<E> q = p.next;
					if (q == null)
					{
						if (p.prev == p)
						{
							return;
						}
						activeSucc = p;
						isLast = true;
						break;
					}
					else if (p == q)
					{
						return;
					}
					else
					{
						p = q;
					}
				}

				// TODO: better HOP heuristics
				if (hops < HOPS && (isFirst | isLast))
				{
					// always squeeze out interior deleted nodes
					return;
				}

				// Squeeze out deleted nodes between activePred and
				// activeSucc, including x.
				SkipDeletedSuccessors(activePred);
				SkipDeletedPredecessors(activeSucc);

				// Try to gc-unlink, if possible
				if ((isFirst | isLast) && (activePred.Next == activeSucc) && (activeSucc.Prev == activePred) && (isFirst ? activePred.Prev == null : activePred.Item != null) && (isLast ? activeSucc.Next == null : activeSucc.Item != null))
				{
					// Recheck expected state of predecessor and successor

					UpdateHead(); // Ensure x is not reachable from head
					UpdateTail(); // Ensure x is not reachable from tail

					// Finally, actually gc-unlink
					x.LazySetPrev(isFirst ? PrevTerminator() : x);
					x.LazySetNext(isLast ? NextTerminator() : x);
				}
			}
		}

		/// <summary>
		/// Unlinks non-null first node.
		/// </summary>
		private void UnlinkFirst(Node<E> first, Node<E> next)
		{
			// assert first != null;
			// assert next != null;
			// assert first.item == null;
			for (Node<E> o = null, p = next, q;;)
			{
				if (p.item != null || (q = p.next) == null)
				{
					if (o != null && p.prev != p && first.CasNext(next, p))
					{
						SkipDeletedPredecessors(p);
						if (first.Prev == null && (p.next == null || p.item != null) && p.prev == first)
						{

							UpdateHead(); // Ensure o is not reachable from head
							UpdateTail(); // Ensure o is not reachable from tail

							// Finally, actually gc-unlink
							o.LazySetNext(o);
							o.LazySetPrev(PrevTerminator());
						}
					}
					return;
				}
				else if (p == q)
				{
					return;
				}
				else
				{
					o = p;
					p = q;
				}
			}
		}

		/// <summary>
		/// Unlinks non-null last node.
		/// </summary>
		private void UnlinkLast(Node<E> last, Node<E> prev)
		{
			// assert last != null;
			// assert prev != null;
			// assert last.item == null;
			for (Node<E> o = null, p = prev, q;;)
			{
				if (p.item != null || (q = p.prev) == null)
				{
					if (o != null && p.next != p && last.CasPrev(prev, p))
					{
						SkipDeletedSuccessors(p);
						if (last.Next == null && (p.prev == null || p.item != null) && p.next == last)
						{

							UpdateHead(); // Ensure o is not reachable from head
							UpdateTail(); // Ensure o is not reachable from tail

							// Finally, actually gc-unlink
							o.LazySetPrev(o);
							o.LazySetNext(NextTerminator());
						}
					}
					return;
				}
				else if (p == q)
				{
					return;
				}
				else
				{
					o = p;
					p = q;
				}
			}
		}

		/// <summary>
		/// Guarantees that any node which was unlinked before a call to
		/// this method will be unreachable from head after it returns.
		/// Does not guarantee to eliminate slack, only that head will
		/// point to a node that was active while this method was running.
		/// </summary>
		private void UpdateHead()
		{
			// Either head already points to an active node, or we keep
			// trying to cas it to the first node until it does.
			Node<E> h, p, q;
			while ((h = Head).item == null && (p = h.Prev) != null)
			{
				for (;;)
				{
					if ((q = p.Prev) == null || (q = (p = q).prev) == null)
					{
						// It is possible that p is PREV_TERMINATOR,
						// but if so, the CAS is guaranteed to fail.
						if (CasHead(h, p))
						{
							return;
						}
						else
						{
							goto restartFromHeadContinue;
						}
					}
					else if (h != Head)
					{
						goto restartFromHeadContinue;
					}
					else
					{
						p = q;
					}
				}
				restartFromHeadContinue:;
			}
			restartFromHeadBreak:;
		}

		/// <summary>
		/// Guarantees that any node which was unlinked before a call to
		/// this method will be unreachable from tail after it returns.
		/// Does not guarantee to eliminate slack, only that tail will
		/// point to a node that was active while this method was running.
		/// </summary>
		private void UpdateTail()
		{
			// Either tail already points to an active node, or we keep
			// trying to cas it to the last node until it does.
			Node<E> t, p, q;
			while ((t = Tail).item == null && (p = t.Next) != null)
			{
				for (;;)
				{
					if ((q = p.Next) == null || (q = (p = q).next) == null)
					{
						// It is possible that p is NEXT_TERMINATOR,
						// but if so, the CAS is guaranteed to fail.
						if (CasTail(t, p))
						{
							return;
						}
						else
						{
							goto restartFromTailContinue;
						}
					}
					else if (t != Tail)
					{
						goto restartFromTailContinue;
					}
					else
					{
						p = q;
					}
				}
				restartFromTailContinue:;
			}
			restartFromTailBreak:;
		}

		private void SkipDeletedPredecessors(Node<E> x)
		{
			do
			{
				Node<E> prev = x.Prev;
				// assert prev != null;
				// assert x != NEXT_TERMINATOR;
				// assert x != PREV_TERMINATOR;
				Node<E> p = prev;
				for (;;)
				{
					if (p.Item != null)
					{
						goto findActiveBreak;
					}
					Node<E> q = p.Prev;
					if (q == null)
					{
						if (p.Next == p)
						{
							goto whileActiveContinue;
						}
						goto findActiveBreak;
					}
					else if (p == q)
					{
						goto whileActiveContinue;
					}
					else
					{
						p = q;
					}
					findActiveContinue:;
				}
				findActiveBreak:

				// found active CAS target
				if (prev == p || x.CasPrev(prev, p))
				{
					return;
				}

			} while (x.Item != null || x.Next == null);
				whileActiveContinue:;
			whileActiveBreak:;
		}

		private void SkipDeletedSuccessors(Node<E> x)
		{
			do
			{
				Node<E> next = x.Next;
				// assert next != null;
				// assert x != NEXT_TERMINATOR;
				// assert x != PREV_TERMINATOR;
				Node<E> p = next;
				for (;;)
				{
					if (p.Item != null)
					{
						goto findActiveBreak;
					}
					Node<E> q = p.Next;
					if (q == null)
					{
						if (p.Prev == p)
						{
							goto whileActiveContinue;
						}
						goto findActiveBreak;
					}
					else if (p == q)
					{
						goto whileActiveContinue;
					}
					else
					{
						p = q;
					}
					findActiveContinue:;
				}
				findActiveBreak:

				// found active CAS target
				if (next == p || x.CasNext(next, p))
				{
					return;
				}

			} while (x.Item != null || x.Prev == null);
				whileActiveContinue:;
			whileActiveBreak:;
		}

		/// <summary>
		/// Returns the successor of p, or the first node if p.next has been
		/// linked to self, which will only be true if traversing with a
		/// stale pointer that is now off the list.
		/// </summary>
		internal Node<E> Succ(Node<E> p)
		{
			// TODO: should we skip deleted nodes here?
			Node<E> q = p.Next;
			return (p == q) ? First() : q;
		}

		/// <summary>
		/// Returns the predecessor of p, or the last node if p.prev has been
		/// linked to self, which will only be true if traversing with a
		/// stale pointer that is now off the list.
		/// </summary>
		internal Node<E> Pred(Node<E> p)
		{
			Node<E> q = p.Prev;
			return (p == q) ? Last() : q;
		}

		/// <summary>
		/// Returns the first node, the unique node p for which:
		///     p.prev == null && p.next != p
		/// The returned node may or may not be logically deleted.
		/// Guarantees that head is set to the returned node.
		/// </summary>
		internal virtual Node<E> First()
		{
			for (;;)
			{
				for (Node<E> h = Head, p = h, q;;)
				{
					if ((q = p.prev) != null && (q = (p = q).prev) != null)
						// Check for head updates every other hop.
						// If p == q, we are sure to follow head instead.
					{
						p = (h != (h = Head)) ? h : q;
					}
					else if (p == h || CasHead(h, p))
							 // It is possible that p is PREV_TERMINATOR,
							 // but if so, the CAS is guaranteed to fail.
					{
						return p;
					}
					else
					{
						goto restartFromHeadContinue;
					}
				}
				restartFromHeadContinue:;
			}
			restartFromHeadBreak:;
		}

		/// <summary>
		/// Returns the last node, the unique node p for which:
		///     p.next == null && p.prev != p
		/// The returned node may or may not be logically deleted.
		/// Guarantees that tail is set to the returned node.
		/// </summary>
		internal virtual Node<E> Last()
		{
			for (;;)
			{
				for (Node<E> t = Tail, p = t, q;;)
				{
					if ((q = p.next) != null && (q = (p = q).next) != null)
						// Check for tail updates every other hop.
						// If p == q, we are sure to follow tail instead.
					{
						p = (t != (t = Tail)) ? t : q;
					}
					else if (p == t || CasTail(t, p))
							 // It is possible that p is NEXT_TERMINATOR,
							 // but if so, the CAS is guaranteed to fail.
					{
						return p;
					}
					else
					{
						goto restartFromTailContinue;
					}
				}
				restartFromTailContinue:;
			}
			restartFromTailBreak:;
		}

		// Minor convenience utilities

		/// <summary>
		/// Throws NullPointerException if argument is null.
		/// </summary>
		/// <param name="v"> the element </param>
		private static void CheckNotNull(Object v)
		{
			if (v == null)
			{
				throw new NullPointerException();
			}
		}

		/// <summary>
		/// Returns element unless it is null, in which case throws
		/// NoSuchElementException.
		/// </summary>
		/// <param name="v"> the element </param>
		/// <returns> the element </returns>
		private E ScreenNullResult(E v)
		{
			if (v == null)
			{
				throw new NoSuchElementException();
			}
			return v;
		}

		/// <summary>
		/// Creates an array list and fills it with elements of this list.
		/// Used by toArray.
		/// </summary>
		/// <returns> the array list </returns>
		private List<E> ToArrayList()
		{
			List<E> list = new List<E>();
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null)
				{
					list.Add(item);
				}
			}
			return list;
		}

		/// <summary>
		/// Constructs an empty deque.
		/// </summary>
		public ConcurrentLinkedDeque()
		{
			Head = Tail = new Node<E>(null);
		}

		/// <summary>
		/// Constructs a deque initially containing the elements of
		/// the given collection, added in traversal order of the
		/// collection's iterator.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public ConcurrentLinkedDeque<T1>(ICollection<T1> c) where T1 : E
		{
			// Copy c into a private chain of Nodes
			Node<E> h = null, t = null;
			foreach (E e in c)
			{
				CheckNotNull(e);
				Node<E> newNode = new Node<E>(e);
				if (h == null)
				{
					h = t = newNode;
				}
				else
				{
					t.LazySetNext(newNode);
					newNode.LazySetPrev(t);
					t = newNode;
				}
			}
			InitHeadTail(h, t);
		}

		/// <summary>
		/// Initializes head and tail, ensuring invariants hold.
		/// </summary>
		private void InitHeadTail(Node<E> h, Node<E> t)
		{
			if (h == t)
			{
				if (h == null)
				{
					h = t = new Node<E>(null);
				}
				else
				{
					// Avoid edge case of a single Node with non-null item.
					Node<E> newNode = new Node<E>(null);
					t.LazySetNext(newNode);
					newNode.LazySetPrev(t);
					t = newNode;
				}
			}
			Head = h;
			Tail = t;
		}

		/// <summary>
		/// Inserts the specified element at the front of this deque.
		/// As the deque is unbounded, this method will never throw
		/// <seealso cref="IllegalStateException"/>.
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void AddFirst(E e)
		{
			LinkFirst(e);
		}

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// As the deque is unbounded, this method will never throw
		/// <seealso cref="IllegalStateException"/>.
		/// 
		/// <para>This method is equivalent to <seealso cref="#add"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void AddLast(E e)
		{
			LinkLast(e);
		}

		/// <summary>
		/// Inserts the specified element at the front of this deque.
		/// As the deque is unbounded, this method will never return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerFirst"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool OfferFirst(E e)
		{
			LinkFirst(e);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// As the deque is unbounded, this method will never return {@code false}.
		/// 
		/// <para>This method is equivalent to <seealso cref="#add"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerLast"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool OfferLast(E e)
		{
			LinkLast(e);
			return true;
		}

		public virtual E PeekFirst()
		{
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null)
				{
					return item;
				}
			}
			return null;
		}

		public virtual E PeekLast()
		{
			for (Node<E> p = Last(); p != null; p = Pred(p))
			{
				E item = p.Item;
				if (item != null)
				{
					return item;
				}
			}
			return null;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E First
		{
			get
			{
				return ScreenNullResult(PeekFirst());
			}
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Last
		{
			get
			{
				return ScreenNullResult(PeekLast());
			}
		}

		public virtual E PollFirst()
		{
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null && p.CasItem(item, null))
				{
					Unlink(p);
					return item;
				}
			}
			return null;
		}

		public virtual E PollLast()
		{
			for (Node<E> p = Last(); p != null; p = Pred(p))
			{
				E item = p.Item;
				if (item != null && p.CasItem(item, null))
				{
					Unlink(p);
					return item;
				}
			}
			return null;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E RemoveFirst()
		{
			return ScreenNullResult(PollFirst());
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E RemoveLast()
		{
			return ScreenNullResult(PollLast());
		}

		// *** Queue and stack methods ***

		/// <summary>
		/// Inserts the specified element at the tail of this deque.
		/// As the deque is unbounded, this method will never return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			return OfferLast(e);
		}

		/// <summary>
		/// Inserts the specified element at the tail of this deque.
		/// As the deque is unbounded, this method will never throw
		/// <seealso cref="IllegalStateException"/> or return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return OfferLast(e);
		}

		public virtual E Poll()
		{
			return PollFirst();
		}
		public virtual E Peek()
		{
			return PeekFirst();
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Remove()
		{
			return RemoveFirst();
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Pop()
		{
			return RemoveFirst();
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Element()
		{
			return First;
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual void Push(E e)
		{
			AddFirst(e);
		}

		/// <summary>
		/// Removes the first element {@code e} such that
		/// {@code o.equals(e)}, if such an element exists in this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if the deque contained the specified element </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool RemoveFirstOccurrence(Object o)
		{
			CheckNotNull(o);
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null && o.Equals(item) && p.CasItem(item, null))
				{
					Unlink(p);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Removes the last element {@code e} such that
		/// {@code o.equals(e)}, if such an element exists in this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if the deque contained the specified element </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool RemoveLastOccurrence(Object o)
		{
			CheckNotNull(o);
			for (Node<E> p = Last(); p != null; p = Pred(p))
			{
				E item = p.Item;
				if (item != null && o.Equals(item) && p.CasItem(item, null))
				{
					Unlink(p);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns {@code true} if this deque contains at least one
		/// element {@code e} such that {@code o.equals(e)}.
		/// </summary>
		/// <param name="o"> element whose presence in this deque is to be tested </param>
		/// <returns> {@code true} if this deque contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			if (o == null)
			{
				return false;
			}
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null && o.Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns {@code true} if this collection contains no elements.
		/// </summary>
		/// <returns> {@code true} if this collection contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return PeekFirst() == null;
			}
		}

		/// <summary>
		/// Returns the number of elements in this deque.  If this deque
		/// contains more than {@code Integer.MAX_VALUE} elements, it
		/// returns {@code Integer.MAX_VALUE}.
		/// 
		/// <para>Beware that, unlike in most collections, this method is
		/// <em>NOT</em> a constant-time operation. Because of the
		/// asynchronous nature of these deques, determining the current
		/// number of elements requires traversing them all to count them.
		/// Additionally, it is possible for the size to change during
		/// execution of this method, in which case the returned result
		/// will be inaccurate. Thus, this method is typically not very
		/// useful in concurrent applications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of elements in this deque </returns>
		public virtual int Size()
		{
			int count = 0;
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				if (p.Item != null)
				{
					// Collection.size() spec says to max out
					if (++count == Integer.MaxValue)
					{
						break;
					}
				}
			}
			return count;
		}

		/// <summary>
		/// Removes the first element {@code e} such that
		/// {@code o.equals(e)}, if such an element exists in this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if the deque contained the specified element </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Remove(Object o)
		{
			return RemoveFirstOccurrence(o);
		}

		/// <summary>
		/// Appends all of the elements in the specified collection to the end of
		/// this deque, in the order that they are returned by the specified
		/// collection's iterator.  Attempts to {@code addAll} of a deque to
		/// itself result in {@code IllegalArgumentException}.
		/// </summary>
		/// <param name="c"> the elements to be inserted into this deque </param>
		/// <returns> {@code true} if this deque changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		/// <exception cref="IllegalArgumentException"> if the collection is this deque </exception>
		public virtual bool addAll<T1>(ICollection<T1> c) where T1 : E
		{
			if (c == this)
			{
				// As historically specified in AbstractQueue#addAll
				throw new IllegalArgumentException();
			}

			// Copy c into a private chain of Nodes
			Node<E> beginningOfTheEnd = null, last = null;
			foreach (E e in c)
			{
				CheckNotNull(e);
				Node<E> newNode = new Node<E>(e);
				if (beginningOfTheEnd == null)
				{
					beginningOfTheEnd = last = newNode;
				}
				else
				{
					last.LazySetNext(newNode);
					newNode.LazySetPrev(last);
					last = newNode;
				}
			}
			if (beginningOfTheEnd == null)
			{
				return false;
			}

			// Atomically append the chain at the tail of this collection
			for (;;)
			{
				for (Node<E> t = Tail, p = t, q;;)
				{
					if ((q = p.next) != null && (q = (p = q).next) != null)
						// Check for tail updates every other hop.
						// If p == q, we are sure to follow tail instead.
					{
						p = (t != (t = Tail)) ? t : q;
					}
					else if (p.prev == p) // NEXT_TERMINATOR
					{
						goto restartFromTailContinue;
					}
					else
					{
						// p is last node
						beginningOfTheEnd.LazySetPrev(p); // CAS piggyback
						if (p.casNext(null, beginningOfTheEnd))
						{
							// Successful CAS is the linearization point
							// for all elements to be added to this deque.
							if (!CasTail(t, last))
							{
								// Try a little harder to update tail,
								// since we may be adding many elements.
								t = Tail;
								if (last.Next == null)
								{
									CasTail(t, last);
								}
							}
							return true;
						}
						// Lost CAS race to another thread; re-read next
					}
				}
				restartFromTailContinue:;
			}
			restartFromTailBreak:;
		}

		/// <summary>
		/// Removes all of the elements from this deque.
		/// </summary>
		public virtual void Clear()
		{
			while (PollFirst() != null)
			{
				;
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this deque, in
		/// proper sequence (from first to last element).
		/// 
		/// <para>The returned array will be "safe" in that no references to it are
		/// maintained by this deque.  (In other words, this method must allocate
		/// a new array).  The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all of the elements in this deque </returns>
		public virtual Object[] ToArray()
		{
			return ToArrayList().ToArray();
		}

		/// <summary>
		/// Returns an array containing all of the elements in this deque,
		/// in proper sequence (from first to last element); the runtime
		/// type of the returned array is that of the specified array.  If
		/// the deque fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of
		/// the specified array and the size of this deque.
		/// 
		/// <para>If this deque fits in the specified array with room to spare
		/// (i.e., the array has more elements than this deque), the element in
		/// the array immediately following the end of the deque is set to
		/// {@code null}.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as
		/// bridge between array-based and collection-based APIs.  Further,
		/// this method allows precise control over the runtime type of the
		/// output array, and may, under certain circumstances, be used to
		/// save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose {@code x} is a deque known to contain only strings.
		/// The following code can be used to dump the deque into a newly
		/// allocated array of {@code String}:
		/// 
		///  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		/// 
		/// Note that {@code toArray(new Object[0])} is identical in function to
		/// {@code toArray()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of the deque are to
		///          be stored, if it is big enough; otherwise, a new array of the
		///          same runtime type is allocated for this purpose </param>
		/// <returns> an array containing all of the elements in this deque </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this deque </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		public virtual T[] toArray<T>(T[] a)
		{
			return ToArrayList().ToArray(a);
		}

		/// <summary>
		/// Returns an iterator over the elements in this deque in proper sequence.
		/// The elements will be returned in order from first (head) to last (tail).
		/// 
		/// <para>The returned iterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this deque in proper sequence </returns>
		public virtual IEnumerator<E> Iterator()
		{
			return new Itr(this);
		}

		/// <summary>
		/// Returns an iterator over the elements in this deque in reverse
		/// sequential order.  The elements will be returned in order from
		/// last (tail) to first (head).
		/// 
		/// <para>The returned iterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this deque in reverse order </returns>
		public virtual IEnumerator<E> DescendingIterator()
		{
			return new DescendingItr(this);
		}

		private abstract class AbstractItr : Iterator<E>
		{
			private readonly ConcurrentLinkedDeque<E> OuterInstance;

			/// <summary>
			/// Next node to return item for.
			/// </summary>
			internal Node<E> NextNode_Renamed;

			/// <summary>
			/// nextItem holds on to item fields because once we claim
			/// that an element exists in hasNext(), we must return it in
			/// the following next() call even if it was in the process of
			/// being removed when hasNext() was called.
			/// </summary>
			internal E NextItem;

			/// <summary>
			/// Node returned by most recent call to next. Needed by remove.
			/// Reset to null if this element is deleted by a call to remove.
			/// </summary>
			internal Node<E> LastRet;

			internal abstract Node<E> StartNode();
			internal abstract Node<E> NextNode(Node<E> p);

			internal AbstractItr(ConcurrentLinkedDeque<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Advance();
			}

			/// <summary>
			/// Sets nextNode and nextItem to next valid node, or to null
			/// if no such.
			/// </summary>
			internal virtual void Advance()
			{
				LastRet = NextNode_Renamed;

				Node<E> p = (NextNode_Renamed == null) ? StartNode() : NextNode(NextNode_Renamed);
				for (;; p = NextNode(p))
				{
					if (p == null)
					{
						// p might be active end or TERMINATOR node; both are OK
						NextNode_Renamed = null;
						NextItem = null;
						break;
					}
					E item = p.Item;
					if (item != null)
					{
						NextNode_Renamed = p;
						NextItem = item;
						break;
					}
				}
			}

			public virtual bool HasNext()
			{
				return NextItem != null;
			}

			public virtual E Next()
			{
				E item = NextItem;
				if (item == null)
				{
					throw new NoSuchElementException();
				}
				Advance();
				return item;
			}

			public virtual void Remove()
			{
				Node<E> l = LastRet;
				if (l == null)
				{
					throw new IllegalStateException();
				}
				l.Item = null;
				outerInstance.Unlink(l);
				LastRet = null;
			}
		}

		/// <summary>
		/// Forward iterator </summary>
		private class Itr : AbstractItr
		{
			private readonly ConcurrentLinkedDeque<E> OuterInstance;

			public Itr(ConcurrentLinkedDeque<E> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal override Node<E> StartNode()
			{
				return outerInstance.First();
			}
			internal override Node<E> NextNode(Node<E> p)
			{
				return outerInstance.Succ(p);
			}
		}

		/// <summary>
		/// Descending iterator </summary>
		private class DescendingItr : AbstractItr
		{
			private readonly ConcurrentLinkedDeque<E> OuterInstance;

			public DescendingItr(ConcurrentLinkedDeque<E> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal override Node<E> StartNode()
			{
				return outerInstance.Last();
			}
			internal override Node<E> NextNode(Node<E> p)
			{
				return outerInstance.Pred(p);
			}
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class CLDSpliterator<E> : Spliterator<E>
		{
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly ConcurrentLinkedDeque<E> Queue;
			internal Node<E> Current; // current node; null until initialized
			internal int Batch; // batch size for splits
			internal bool Exhausted; // true when no more nodes
			internal CLDSpliterator(ConcurrentLinkedDeque<E> queue)
			{
				this.Queue = queue;
			}

			public Spliterator<E> TrySplit()
			{
				Node<E> p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConcurrentLinkedDeque<E> q = this.queue;
				ConcurrentLinkedDeque<E> q = this.Queue;
				int b = Batch;
				int n = (b <= 0) ? 1 : (b >= MAX_BATCH) ? MAX_BATCH : b + 1;
				if (!Exhausted && ((p = Current) != null || (p = q.First()) != null))
				{
					if (p.Item == null && p == (p = p.Next))
					{
						Current = p = q.First();
					}
					if (p != null && p.Next != null)
					{
						Object[] a = new Object[n];
						int i = 0;
						do
						{
							if ((a[i] = p.Item) != null)
							{
								++i;
							}
							if (p == (p = p.Next))
							{
								p = q.First();
							}
						} while (p != null && i < n);
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
				}
				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				Node<E> p;
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConcurrentLinkedDeque<E> q = this.queue;
				ConcurrentLinkedDeque<E> q = this.Queue;
				if (!Exhausted && ((p = Current) != null || (p = q.First()) != null))
				{
					Exhausted = true;
					do
					{
						E e = p.Item;
						if (p == (p = p.Next))
						{
							p = q.First();
						}
						if (e != null)
						{
							action.Accept(e);
						}
					} while (p != null);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base E> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				Node<E> p;
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConcurrentLinkedDeque<E> q = this.queue;
				ConcurrentLinkedDeque<E> q = this.Queue;
				if (!Exhausted && ((p = Current) != null || (p = q.First()) != null))
				{
					E e;
					do
					{
						e = p.Item;
						if (p == (p = p.Next))
						{
							p = q.First();
						}
					} while (e == null && p != null);
					if ((Current = p) == null)
					{
						Exhausted = true;
					}
					if (e != null)
					{
						action.Accept(e);
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
		/// Returns a <seealso cref="Spliterator"/> over the elements in this deque.
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
		/// <returns> a {@code Spliterator} over the elements in this deque
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return new CLDSpliterator<E>(this);
		}

		/// <summary>
		/// Saves this deque to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData All of the elements (each an {@code E}) in
		/// the proper order, followed by a null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{

			// Write out any hidden stuff
			s.DefaultWriteObject();

			// Write out all elements in the proper order.
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null)
				{
					s.WriteObject(item);
				}
			}

			// Use trailing null as sentinel
			s.WriteObject(null);
		}

		/// <summary>
		/// Reconstitutes this deque from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();

			// Read in elements until trailing null sentinel found
			Node<E> h = null, t = null;
			Object item;
			while ((item = s.ReadObject()) != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<E> newNode = new Node<E>((E) item);
				Node<E> newNode = new Node<E>((E) item);
				if (h == null)
				{
					h = t = newNode;
				}
				else
				{
					t.LazySetNext(newNode);
					newNode.LazySetPrev(t);
					t = newNode;
				}
			}
			InitHeadTail(h, t);
		}

		private bool CasHead(Node<E> cmp, Node<E> val)
		{
			return UNSAFE.compareAndSwapObject(this, HeadOffset, cmp, val);
		}

		private bool CasTail(Node<E> cmp, Node<E> val)
		{
			return UNSAFE.compareAndSwapObject(this, TailOffset, cmp, val);
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long HeadOffset;
		private static readonly long TailOffset;
		static ConcurrentLinkedDeque()
		{
			PREV_TERMINATOR = new Node<Object>();
			PREV_TERMINATOR.Next = PREV_TERMINATOR;
			NEXT_TERMINATOR = new Node<Object>();
			NEXT_TERMINATOR.Prev = NEXT_TERMINATOR;
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(ConcurrentLinkedDeque);
				HeadOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("head"));
				TailOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("tail"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}