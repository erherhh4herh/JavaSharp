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
	/// An unbounded thread-safe <seealso cref="Queue queue"/> based on linked nodes.
	/// This queue orders elements FIFO (first-in-first-out).
	/// The <em>head</em> of the queue is that element that has been on the
	/// queue the longest time.
	/// The <em>tail</em> of the queue is that element that has been on the
	/// queue the shortest time. New elements
	/// are inserted at the tail of the queue, and the queue retrieval
	/// operations obtain elements at the head of the queue.
	/// A {@code ConcurrentLinkedQueue} is an appropriate choice when
	/// many threads will share access to a common collection.
	/// Like most other concurrent collection implementations, this class
	/// does not permit the use of {@code null} elements.
	/// 
	/// <para>This implementation employs an efficient <em>non-blocking</em>
	/// algorithm based on one described in <a
	/// href="http://www.cs.rochester.edu/u/michael/PODC96.html"> Simple,
	/// Fast, and Practical Non-Blocking and Blocking Concurrent Queue
	/// Algorithms</a> by Maged M. Michael and Michael L. Scott.
	/// 
	/// </para>
	/// <para>Iterators are <i>weakly consistent</i>, returning elements
	/// reflecting the state of the queue at some point at or since the
	/// creation of the iterator.  They do <em>not</em> throw {@link
	/// java.util.ConcurrentModificationException}, and may proceed concurrently
	/// with other operations.  Elements contained in the queue since the creation
	/// of the iterator will be returned exactly once.
	/// 
	/// </para>
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
	/// <para>This class and its iterator implement all of the <em>optional</em>
	/// methods of the <seealso cref="Queue"/> and <seealso cref="Iterator"/> interfaces.
	/// 
	/// </para>
	/// <para>Memory consistency effects: As with other concurrent
	/// collections, actions in a thread prior to placing an object into a
	/// {@code ConcurrentLinkedQueue}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that element from
	/// the {@code ConcurrentLinkedQueue} in another thread.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class ConcurrentLinkedQueue<E> : AbstractQueue<E>, Queue<E>
	{
		private const long SerialVersionUID = 196745693267521676L;

		/*
		 * This is a modification of the Michael & Scott algorithm,
		 * adapted for a garbage-collected environment, with support for
		 * interior node deletion (to support remove(Object)).  For
		 * explanation, read the paper.
		 *
		 * Note that like most non-blocking algorithms in this package,
		 * this implementation relies on the fact that in garbage
		 * collected systems, there is no possibility of ABA problems due
		 * to recycled nodes, so there is no need to use "counted
		 * pointers" or related techniques seen in versions used in
		 * non-GC'ed settings.
		 *
		 * The fundamental invariants are:
		 * - There is exactly one (last) Node with a null next reference,
		 *   which is CASed when enqueueing.  This last Node can be
		 *   reached in O(1) time from tail, but tail is merely an
		 *   optimization - it can always be reached in O(N) time from
		 *   head as well.
		 * - The elements contained in the queue are the non-null items in
		 *   Nodes that are reachable from head.  CASing the item
		 *   reference of a Node to null atomically removes it from the
		 *   queue.  Reachability of all elements from head must remain
		 *   true even in the case of concurrent modifications that cause
		 *   head to advance.  A dequeued Node may remain in use
		 *   indefinitely due to creation of an Iterator or simply a
		 *   poll() that has lost its time slice.
		 *
		 * The above might appear to imply that all Nodes are GC-reachable
		 * from a predecessor dequeued Node.  That would cause two problems:
		 * - allow a rogue Iterator to cause unbounded memory retention
		 * - cause cross-generational linking of old Nodes to new Nodes if
		 *   a Node was tenured while live, which generational GCs have a
		 *   hard time dealing with, causing repeated major collections.
		 * However, only non-deleted Nodes need to be reachable from
		 * dequeued Nodes, and reachability does not necessarily have to
		 * be of the kind understood by the GC.  We use the trick of
		 * linking a Node that has just been dequeued to itself.  Such a
		 * self-link implicitly means to advance to head.
		 *
		 * Both head and tail are permitted to lag.  In fact, failing to
		 * update them every time one could is a significant optimization
		 * (fewer CASes). As with LinkedTransferQueue (see the internal
		 * documentation for that class), we use a slack threshold of two;
		 * that is, we update head/tail when the current pointer appears
		 * to be two or more steps away from the first/last node.
		 *
		 * Since head and tail are updated concurrently and independently,
		 * it is possible for tail to lag behind head (why not)?
		 *
		 * CASing a Node's item reference to null atomically removes the
		 * element from the queue.  Iterators skip over Nodes with null
		 * items.  Prior implementations of this class had a race between
		 * poll() and remove(Object) where the same element would appear
		 * to be successfully removed by two concurrent operations.  The
		 * method remove(Object) also lazily unlinks deleted Nodes, but
		 * this is merely an optimization.
		 *
		 * When constructing a Node (before enqueuing it) we avoid paying
		 * for a volatile write to item by using Unsafe.putObject instead
		 * of a normal write.  This allows the cost of enqueue to be
		 * "one-and-a-half" CASes.
		 *
		 * Both head and tail may or may not point to a Node with a
		 * non-null item.  If the queue is empty, all items must of course
		 * be null.  Upon creation, both head and tail refer to a dummy
		 * Node with null item.  Both head and tail are only updated using
		 * CAS, so they never regress, although again this is merely an
		 * optimization.
		 */

		private class Node<E>
		{
			internal volatile E Item;
			internal volatile Node<E> Next;

			/// <summary>
			/// Constructs a new node.  Uses relaxed write because item can
			/// only be seen after publication via casNext.
			/// </summary>
			internal Node(E item)
			{
				UNSAFE.putObject(this, ItemOffset, item);
			}

			internal virtual bool CasItem(E cmp, E val)
			{
				return UNSAFE.compareAndSwapObject(this, ItemOffset, cmp, val);
			}

			internal virtual void LazySetNext(Node<E> val)
			{
				UNSAFE.putOrderedObject(this, NextOffset, val);
			}

			internal virtual bool CasNext(Node<E> cmp, Node<E> val)
			{
				return UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
			}

			// Unsafe mechanics

			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long ItemOffset;
			internal static readonly long NextOffset;

			static Node()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class k = typeof(Node);
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
		/// A node from which the first live (non-deleted) node (if any)
		/// can be reached in O(1) time.
		/// Invariants:
		/// - all live nodes are reachable from head via succ()
		/// - head != null
		/// - (tmp = head).next != tmp || tmp != head
		/// Non-invariants:
		/// - head.item may or may not be null.
		/// - it is permitted for tail to lag behind head, that is, for tail
		///   to not be reachable from head!
		/// </summary>
		[NonSerialized]
		private volatile Node<E> Head;

		/// <summary>
		/// A node from which the last node on list (that is, the unique
		/// node with node.next == null) can be reached in O(1) time.
		/// Invariants:
		/// - the last node is always reachable from tail via succ()
		/// - tail != null
		/// Non-invariants:
		/// - tail.item may or may not be null.
		/// - it is permitted for tail to lag behind head, that is, for tail
		///   to not be reachable from head!
		/// - tail.next may or may not be self-pointing to tail.
		/// </summary>
		[NonSerialized]
		private volatile Node<E> Tail;

		/// <summary>
		/// Creates a {@code ConcurrentLinkedQueue} that is initially empty.
		/// </summary>
		public ConcurrentLinkedQueue()
		{
			Head = Tail = new Node<E>(null);
		}

		/// <summary>
		/// Creates a {@code ConcurrentLinkedQueue}
		/// initially containing the elements of the given collection,
		/// added in traversal order of the collection's iterator.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public ConcurrentLinkedQueue<T1>(ICollection<T1> c) where T1 : E
		{
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
					t = newNode;
				}
			}
			if (h == null)
			{
				h = t = new Node<E>(null);
			}
			Head = h;
			Tail = t;
		}

		// Have to override just to update the javadoc

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never throw
		/// <seealso cref="IllegalStateException"/> or return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return Offer(e);
		}

		/// <summary>
		/// Tries to CAS head to p. If successful, repoint old head to itself
		/// as sentinel for succ(), below.
		/// </summary>
		internal void UpdateHead(Node<E> h, Node<E> p)
		{
			if (h != p && CasHead(h, p))
			{
				h.LazySetNext(h);
			}
		}

		/// <summary>
		/// Returns the successor of p, or the head node if p.next has been
		/// linked to self, which will only be true if traversing with a
		/// stale pointer that is now off the list.
		/// </summary>
		internal Node<E> Succ(Node<E> p)
		{
			Node<E> next = p.Next;
			return (p == next) ? Head : next;
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue.
		/// As the queue is unbounded, this method will never return {@code false}.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			CheckNotNull(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<E>(e);
			Node<E> newNode = new Node<E>(e);

			for (Node<E> t = Tail, p = t;;)
			{
				Node<E> q = p.next;
				if (q == null)
				{
					// p is last node
					if (p.casNext(null, newNode))
					{
						// Successful CAS is the linearization point
						// for e to become an element of this queue,
						// and for newNode to become "live".
						if (p != t) // hop two nodes at a time
						{
							CasTail(t, newNode); // Failure is OK.
						}
						return true;
					}
					// Lost CAS race to another thread; re-read next
				}
				else if (p == q)
					// We have fallen off list.  If tail is unchanged, it
					// will also be off-list, in which case we need to
					// jump to head, from which all live nodes are always
					// reachable.  Else the new tail is a better bet.
				{
					p = (t != (t = Tail)) ? t : Head;
				}
				else
				{
					// Check for tail updates after two hops.
					p = (p != t && t != (t = Tail)) ? t : q;
				}
			}
		}

		public virtual E Poll()
		{
			for (;;)
			{
				for (Node<E> h = Head, p = h, q;;)
				{
					E item = p.item;

					if (item != null && p.casItem(item, null))
					{
						// Successful CAS is the linearization point
						// for item to be removed from this queue.
						if (p != h) // hop two nodes at a time
						{
							UpdateHead(h, ((q = p.next) != null) ? q : p);
						}
						return item;
					}
					else if ((q = p.next) == null)
					{
						UpdateHead(h, p);
						return null;
					}
					else if (p == q)
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

		public virtual E Peek()
		{
			for (;;)
			{
				for (Node<E> h = Head, p = h, q;;)
				{
					E item = p.item;
					if (item != null || (q = p.next) == null)
					{
						UpdateHead(h, p);
						return item;
					}
					else if (p == q)
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
		/// Returns the first live (non-deleted) node on list, or null if none.
		/// This is yet another variant of poll/peek; here returning the
		/// first node, not element.  We could make peek() a wrapper around
		/// first(), but that would cost an extra volatile read of item,
		/// and the need to add a retry loop to deal with the possibility
		/// of losing a race to a concurrent poll().
		/// </summary>
		internal virtual Node<E> First()
		{
			for (;;)
			{
				for (Node<E> h = Head, p = h, q;;)
				{
					bool hasItem = (p.item != null);
					if (hasItem || (q = p.next) == null)
					{
						UpdateHead(h, p);
						return hasItem ? p : null;
					}
					else if (p == q)
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
		/// Returns {@code true} if this queue contains no elements.
		/// </summary>
		/// <returns> {@code true} if this queue contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return First() == null;
			}
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
		/// Additionally, if elements are added or removed during execution
		/// of this method, the returned result may be inaccurate.  Thus,
		/// this method is typically not very useful in concurrent
		/// applications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of elements in this queue </returns>
		public virtual int Count
		{
			get
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
			if (o == null)
			{
				return false;
			}
			Node<E> pred = null;
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null && o.Equals(item) && p.CasItem(item, null))
				{
					Node<E> next = Succ(p);
					if (pred != null && next != null)
					{
						pred.CasNext(p, next);
					}
					return true;
				}
				pred = p;
			}
			return false;
		}

		/// <summary>
		/// Appends all of the elements in the specified collection to the end of
		/// this queue, in the order that they are returned by the specified
		/// collection's iterator.  Attempts to {@code addAll} of a queue to
		/// itself result in {@code IllegalArgumentException}.
		/// </summary>
		/// <param name="c"> the elements to be inserted into this queue </param>
		/// <returns> {@code true} if this queue changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		/// <exception cref="IllegalArgumentException"> if the collection is this queue </exception>
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
					last = newNode;
				}
			}
			if (beginningOfTheEnd == null)
			{
				return false;
			}

			// Atomically append the chain at the tail of this collection
			for (Node<E> t = Tail, p = t;;)
			{
				Node<E> q = p.next;
				if (q == null)
				{
					// p is last node
					if (p.casNext(null, beginningOfTheEnd))
					{
						// Successful CAS is the linearization point
						// for all elements to be added to this queue.
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
				else if (p == q)
					// We have fallen off list.  If tail is unchanged, it
					// will also be off-list, in which case we need to
					// jump to head, from which all live nodes are always
					// reachable.  Else the new tail is a better bet.
				{
					p = (t != (t = Tail)) ? t : Head;
				}
				else
				{
					// Check for tail updates after two hops.
					p = (p != t && t != (t = Tail)) ? t : q;
				}
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue, in
		/// proper sequence.
		/// 
		/// <para>The returned array will be "safe" in that no references to it are
		/// maintained by this queue.  (In other words, this method must allocate
		/// a new array).  The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all of the elements in this queue </returns>
		public virtual Object[] ToArray()
		{
			// Use ArrayList to deal with resizing.
			List<E> al = new List<E>();
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				E item = p.Item;
				if (item != null)
				{
					al.Add(item);
				}
			}
			return al.ToArray();
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue, in
		/// proper sequence; the runtime type of the returned array is that of
		/// the specified array.  If the queue fits in the specified array, it
		/// is returned therein.  Otherwise, a new array is allocated with the
		/// runtime type of the specified array and the size of this queue.
		/// 
		/// <para>If this queue fits in the specified array with room to spare
		/// (i.e., the array has more elements than this queue), the element in
		/// the array immediately following the end of the queue is set to
		/// {@code null}.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose {@code x} is a queue known to contain only strings.
		/// The following code can be used to dump the queue into a newly
		/// allocated array of {@code String}:
		/// 
		///  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		/// 
		/// Note that {@code toArray(new Object[0])} is identical in function to
		/// {@code toArray()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of the queue are to
		///          be stored, if it is big enough; otherwise, a new array of the
		///          same runtime type is allocated for this purpose </param>
		/// <returns> an array containing all of the elements in this queue </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this queue </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
		public virtual T[] toArray<T>(T[] a)
		{
			// try to use sent-in array
			int k = 0;
			Node<E> p;
			for (p = First(); p != null && k < a.Length; p = Succ(p))
			{
				E item = p.Item;
				if (item != null)
				{
					a[k++] = (T)item;
				}
			}
			if (p == null)
			{
				if (k < a.Length)
				{
					a[k] = null;
				}
				return a;
			}

			// If won't fit, use ArrayList version
			List<E> al = new List<E>();
			for (Node<E> q = First(); q != null; q = Succ(q))
			{
				E item = q.Item;
				if (item != null)
				{
					al.Add(item);
				}
			}
			return al.ToArray(a);
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
		public virtual IEnumerator<E> GetEnumerator()
		{
			return new Itr(this);
		}

		private class Itr : Iterator<E>
		{
			private readonly ConcurrentLinkedQueue<E> OuterInstance;

			/// <summary>
			/// Next node to return item for.
			/// </summary>
			internal Node<E> NextNode;

			/// <summary>
			/// nextItem holds on to item fields because once we claim
			/// that an element exists in hasNext(), we must return it in
			/// the following next() call even if it was in the process of
			/// being removed when hasNext() was called.
			/// </summary>
			internal E NextItem;

			/// <summary>
			/// Node of the last returned item, to support remove.
			/// </summary>
			internal Node<E> LastRet;

			internal Itr(ConcurrentLinkedQueue<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Advance();
			}

			/// <summary>
			/// Moves to next valid node and returns item to return for
			/// next(), or null if no such.
			/// </summary>
			internal virtual E Advance()
			{
				LastRet = NextNode;
				E x = NextItem;

				Node<E> pred, p;
				if (NextNode == null)
				{
					p = outerInstance.First();
					pred = null;
				}
				else
				{
					pred = NextNode;
					p = outerInstance.Succ(NextNode);
				}

				for (;;)
				{
					if (p == null)
					{
						NextNode = null;
						NextItem = null;
						return x;
					}
					E item = p.Item;
					if (item != null)
					{
						NextNode = p;
						NextItem = item;
						return x;
					}
					else
					{
						// skip over nulls
						Node<E> next = outerInstance.Succ(p);
						if (pred != null && next != null)
						{
							pred.CasNext(p, next);
						}
						p = next;
					}
				}
			}

			public virtual bool HasNext()
			{
				return NextNode != null;
			}

			public virtual E Next()
			{
				if (NextNode == null)
				{
					throw new NoSuchElementException();
				}
				return Advance();
			}

			public virtual void Remove()
			{
				Node<E> l = LastRet;
				if (l == null)
				{
					throw new IllegalStateException();
				}
				// rely on a future traversal to relink.
				l.Item = null;
				LastRet = null;
			}
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

			// Write out any hidden stuff
			s.DefaultWriteObject();

			// Write out all elements in the proper order.
			for (Node<E> p = First(); p != null; p = Succ(p))
			{
				Object item = p.Item;
				if (item != null)
				{
					s.WriteObject(item);
				}
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
					t = newNode;
				}
			}
			if (h == null)
			{
				h = t = new Node<E>(null);
			}
			Head = h;
			Tail = t;
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class CLQSpliterator<E> : Spliterator<E>
		{
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly ConcurrentLinkedQueue<E> Queue;
			internal Node<E> Current; // current node; null until initialized
			internal int Batch; // batch size for splits
			internal bool Exhausted; // true when no more nodes
			internal CLQSpliterator(ConcurrentLinkedQueue<E> queue)
			{
				this.Queue = queue;
			}

			public Spliterator<E> TrySplit()
			{
				Node<E> p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConcurrentLinkedQueue<E> q = this.queue;
				ConcurrentLinkedQueue<E> q = this.Queue;
				int b = Batch;
				int n = (b <= 0) ? 1 : (b >= MAX_BATCH) ? MAX_BATCH : b + 1;
				if (!Exhausted && ((p = Current) != null || (p = q.First()) != null) && p.Next != null)
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
//ORIGINAL LINE: final ConcurrentLinkedQueue<E> q = this.queue;
				ConcurrentLinkedQueue<E> q = this.Queue;
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
//ORIGINAL LINE: final ConcurrentLinkedQueue<E> q = this.queue;
				ConcurrentLinkedQueue<E> q = this.Queue;
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
		public override Spliterator<E> Spliterator()
		{
			return new CLQSpliterator<E>(this);
		}

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

		private bool CasTail(Node<E> cmp, Node<E> val)
		{
			return UNSAFE.compareAndSwapObject(this, TailOffset, cmp, val);
		}

		private bool CasHead(Node<E> cmp, Node<E> val)
		{
			return UNSAFE.compareAndSwapObject(this, HeadOffset, cmp, val);
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long HeadOffset;
		private static readonly long TailOffset;
		static ConcurrentLinkedQueue()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(ConcurrentLinkedQueue);
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