using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// An unbounded priority <seealso cref="Queue queue"/> based on a priority heap.
	/// The elements of the priority queue are ordered according to their
	/// <seealso cref="Comparable natural ordering"/>, or by a <seealso cref="Comparator"/>
	/// provided at queue construction time, depending on which constructor is
	/// used.  A priority queue does not permit {@code null} elements.
	/// A priority queue relying on natural ordering also does not permit
	/// insertion of non-comparable objects (doing so may result in
	/// {@code ClassCastException}).
	/// 
	/// <para>The <em>head</em> of this queue is the <em>least</em> element
	/// with respect to the specified ordering.  If multiple elements are
	/// tied for least value, the head is one of those elements -- ties are
	/// broken arbitrarily.  The queue retrieval operations {@code poll},
	/// {@code remove}, {@code peek}, and {@code element} access the
	/// element at the head of the queue.
	/// 
	/// </para>
	/// <para>A priority queue is unbounded, but has an internal
	/// <i>capacity</i> governing the size of an array used to store the
	/// elements on the queue.  It is always at least as large as the queue
	/// size.  As elements are added to a priority queue, its capacity
	/// grows automatically.  The details of the growth policy are not
	/// specified.
	/// 
	/// </para>
	/// <para>This class and its iterator implement all of the
	/// <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	/// Iterator} interfaces.  The Iterator provided in method {@link
	/// #iterator()} is <em>not</em> guaranteed to traverse the elements of
	/// the priority queue in any particular order. If you need ordered
	/// traversal, consider using {@code Arrays.sort(pq.toArray())}.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// Multiple threads should not access a {@code PriorityQueue}
	/// instance concurrently if any of the threads modifies the queue.
	/// Instead, use the thread-safe {@link
	/// java.util.concurrent.PriorityBlockingQueue} class.
	/// 
	/// </para>
	/// <para>Implementation note: this implementation provides
	/// O(log(n)) time for the enqueuing and dequeuing methods
	/// ({@code offer}, {@code poll}, {@code remove()} and {@code add});
	/// linear time for the {@code remove(Object)} and {@code contains(Object)}
	/// methods; and constant time for the retrieval methods
	/// ({@code peek}, {@code element}, and {@code size}).
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.5
	/// @author Josh Bloch, Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class PriorityQueue<E> : AbstractQueue<E>
	{

		private const long SerialVersionUID = -7720805057305804111L;

		private const int DEFAULT_INITIAL_CAPACITY = 11;

		/// <summary>
		/// Priority queue represented as a balanced binary heap: the two
		/// children of queue[n] are queue[2*n+1] and queue[2*(n+1)].  The
		/// priority queue is ordered by comparator, or by the elements'
		/// natural ordering, if comparator is null: For each node n in the
		/// heap and each descendant d of n, n <= d.  The element with the
		/// lowest value is in queue[0], assuming the queue is nonempty.
		/// </summary>
		[NonSerialized]
		internal Object[] Queue; // non-private to simplify nested class access

		/// <summary>
		/// The number of elements in the priority queue.
		/// </summary>
		private int Size_Renamed = 0;

		/// <summary>
		/// The comparator, or null if priority queue uses elements'
		/// natural ordering.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private final Comparator<? base E> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private readonly Comparator<?> Comparator_Renamed;

		/// <summary>
		/// The number of times this priority queue has been
		/// <i>structurally modified</i>.  See AbstractList for gory details.
		/// </summary>
		[NonSerialized]
		internal int ModCount = 0; // non-private to simplify nested class access

		/// <summary>
		/// Creates a {@code PriorityQueue} with the default initial
		/// capacity (11) that orders its elements according to their
		/// <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		public PriorityQueue() : this(DEFAULT_INITIAL_CAPACITY, null)
		{
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} with the specified initial
		/// capacity that orders its elements according to their
		/// <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity for this priority queue </param>
		/// <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		///         than 1 </exception>
		public PriorityQueue(int initialCapacity) : this(initialCapacity, null)
		{
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} with the default initial capacity and
		/// whose elements are ordered according to the specified comparator.
		/// </summary>
		/// <param name="comparator"> the comparator that will be used to order this
		///         priority queue.  If {@code null}, the {@link Comparable
		///         natural ordering} of the elements will be used.
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public PriorityQueue(Comparator<? base E> comparator)
		public PriorityQueue<T1>(Comparator<T1> comparator) : this(DEFAULT_INITIAL_CAPACITY, comparator)
		{
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} with the specified initial capacity
		/// that orders its elements according to the specified comparator.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity for this priority queue </param>
		/// <param name="comparator"> the comparator that will be used to order this
		///         priority queue.  If {@code null}, the {@link Comparable
		///         natural ordering} of the elements will be used. </param>
		/// <exception cref="IllegalArgumentException"> if {@code initialCapacity} is
		///         less than 1 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public PriorityQueue(int initialCapacity, Comparator<? base E> comparator)
		public PriorityQueue<T1>(int initialCapacity, Comparator<T1> comparator)
		{
			// Note: This restriction of at least one is not actually needed,
			// but continues for 1.5 compatibility
			if (initialCapacity < 1)
			{
				throw new IllegalArgumentException();
			}
			this.Queue = new Object[initialCapacity];
			this.Comparator_Renamed = comparator;
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} containing the elements in the
		/// specified collection.  If the specified collection is an instance of
		/// a <seealso cref="SortedSet"/> or is another {@code PriorityQueue}, this
		/// priority queue will be ordered according to the same ordering.
		/// Otherwise, this priority queue will be ordered according to the
		/// <seealso cref="Comparable natural ordering"/> of its elements.
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed
		///         into this priority queue </param>
		/// <exception cref="ClassCastException"> if elements of the specified collection
		///         cannot be compared to one another according to the priority
		///         queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(Collection<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(Collection<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public PriorityQueue<T1>(Collection<T1> c) where ? : E
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (c instanceof SortedSet<?>)
			if (c is SortedSet<?>)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedSet<? extends E> ss = (SortedSet<? extends E>) c;
				SortedSet<?> ss = (SortedSet<?>) c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (Comparator<? base E>) ss.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				this.Comparator_Renamed = (Comparator<?>) ss.Comparator();
				InitElementsFromCollection(ss);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: else if (c instanceof PriorityQueue<?>)
			else if (c is PriorityQueue<?>)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: PriorityQueue<? extends E> pq = (PriorityQueue<? extends E>) c;
				PriorityQueue<?> pq = (PriorityQueue<?>) c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (Comparator<? base E>) pq.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				this.Comparator_Renamed = (Comparator<?>) pq.Comparator();
				InitFromPriorityQueue(pq);
			}
			else
			{
				this.Comparator_Renamed = null;
				InitFromCollection(c);
			}
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} containing the elements in the
		/// specified priority queue.  This priority queue will be
		/// ordered according to the same ordering as the given priority
		/// queue.
		/// </summary>
		/// <param name="c"> the priority queue whose elements are to be placed
		///         into this priority queue </param>
		/// <exception cref="ClassCastException"> if elements of {@code c} cannot be
		///         compared to one another according to {@code c}'s
		///         ordering </exception>
		/// <exception cref="NullPointerException"> if the specified priority queue or any
		///         of its elements are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(PriorityQueue<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(PriorityQueue<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public PriorityQueue<T1>(PriorityQueue<T1> c) where ? : E
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (Comparator<? base E>) c.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			this.Comparator_Renamed = (Comparator<?>) c.Comparator();
			InitFromPriorityQueue(c);
		}

		/// <summary>
		/// Creates a {@code PriorityQueue} containing the elements in the
		/// specified sorted set.   This priority queue will be ordered
		/// according to the same ordering as the given sorted set.
		/// </summary>
		/// <param name="c"> the sorted set whose elements are to be placed
		///         into this priority queue </param>
		/// <exception cref="ClassCastException"> if elements of the specified sorted
		///         set cannot be compared to one another according to the
		///         sorted set's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified sorted set or any
		///         of its elements are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(SortedSet<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public PriorityQueue(SortedSet<? extends E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public PriorityQueue<T1>(SortedSet<T1> c) where ? : E
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (Comparator<? base E>) c.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			this.Comparator_Renamed = (Comparator<?>) c.Comparator();
			InitElementsFromCollection(c);
		}

		private void initFromPriorityQueue<T1>(PriorityQueue<T1> c) where T1 : E
		{
			if (c.GetType() == typeof(PriorityQueue))
			{
				this.Queue = c.ToArray();
				this.Size_Renamed = c.Size();
			}
			else
			{
				InitFromCollection(c);
			}
		}

		private void initElementsFromCollection<T1>(Collection<T1> c) where T1 : E
		{
			Object[] a = c.ToArray();
			// If c.toArray incorrectly doesn't return Object[], copy it.
			if (a.GetType() != typeof(Object[]))
			{
				a = Arrays.CopyOf(a, a.Length, typeof(Object[]));
			}
			int len = a.Length;
			if (len == 1 || this.Comparator_Renamed != null)
			{
				for (int i = 0; i < len; i++)
				{
					if (a[i] == null)
					{
						throw new NullPointerException();
					}
				}
			}
			this.Queue = a;
			this.Size_Renamed = a.Length;
		}

		/// <summary>
		/// Initializes queue array with elements from the given Collection.
		/// </summary>
		/// <param name="c"> the collection </param>
		private void initFromCollection<T1>(Collection<T1> c) where T1 : E
		{
			InitElementsFromCollection(c);
			Heapify();
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Increases the capacity of the array.
		/// </summary>
		/// <param name="minCapacity"> the desired minimum capacity </param>
		private void Grow(int minCapacity)
		{
			int oldCapacity = Queue.Length;
			// Double size if small; else grow by 50%
			int newCapacity = oldCapacity + ((oldCapacity < 64) ? (oldCapacity + 2) : (oldCapacity >> 1));
			// overflow-conscious code
			if (newCapacity - MAX_ARRAY_SIZE > 0)
			{
				newCapacity = HugeCapacity(minCapacity);
			}
			Queue = Arrays.CopyOf(Queue, newCapacity);
		}

		private static int HugeCapacity(int minCapacity)
		{
			if (minCapacity < 0) // overflow
			{
				throw new OutOfMemoryError();
			}
			return (minCapacity > MAX_ARRAY_SIZE) ? Integer.MaxValue : MAX_ARRAY_SIZE;
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with elements currently in this priority queue
		///         according to the priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return Offer(e);
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// </summary>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with elements currently in this priority queue
		///         according to the priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			ModCount++;
			int i = Size_Renamed;
			if (i >= Queue.Length)
			{
				Grow(i + 1);
			}
			Size_Renamed = i + 1;
			if (i == 0)
			{
				Queue[0] = e;
			}
			else
			{
				SiftUp(i, e);
			}
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E peek()
		public virtual E Peek()
		{
			return (Size_Renamed == 0) ? null : (E) Queue[0];
		}

		private int IndexOf(Object o)
		{
			if (o != null)
			{
				for (int i = 0; i < Size_Renamed; i++)
				{
					if (o.Equals(Queue[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Removes a single instance of the specified element from this queue,
		/// if it is present.  More formally, removes an element {@code e} such
		/// that {@code o.equals(e)}, if this queue contains one or more such
		/// elements.  Returns {@code true} if and only if this queue contained
		/// the specified element (or equivalently, if this queue changed as a
		/// result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this queue, if present </param>
		/// <returns> {@code true} if this queue changed as a result of the call </returns>
		public virtual bool Remove(Object o)
		{
			int i = IndexOf(o);
			if (i == -1)
			{
				return false;
			}
			else
			{
				RemoveAt(i);
				return true;
			}
		}

		/// <summary>
		/// Version of remove using reference equality, not equals.
		/// Needed by iterator.remove.
		/// </summary>
		/// <param name="o"> element to be removed from this queue, if present </param>
		/// <returns> {@code true} if removed </returns>
		internal virtual bool RemoveEq(Object o)
		{
			for (int i = 0; i < Size_Renamed; i++)
			{
				if (o == Queue[i])
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
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
			return IndexOf(o) != -1;
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue.
		/// The elements are in no particular order.
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
			return Arrays.CopyOf(Queue, Size_Renamed);
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue; the
		/// runtime type of the returned array is that of the specified array.
		/// The returned array elements are in no particular order.
		/// If the queue fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of the
		/// specified array and the size of this queue.
		/// 
		/// <para>If the queue fits in the specified array with room to spare
		/// (i.e., the array has more elements than the queue), the element in
		/// the array immediately following the end of the collection is set to
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
		///          same runtime type is allocated for this purpose. </param>
		/// <returns> an array containing all of the elements in this queue </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this queue </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
		public virtual T[] toArray<T>(T[] a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = this.size;
			int size = this.Size_Renamed;
			if (a.Length < size)
			{
				// Make a new array of a's runtime type, but my contents:
				return (T[]) Arrays.CopyOf(Queue, size, a.GetType());
			}
			System.Array.Copy(Queue, 0, a, 0, size);
			if (a.Length > size)
			{
				a[size] = null;
			}
			return a;
		}

		/// <summary>
		/// Returns an iterator over the elements in this queue. The iterator
		/// does not return the elements in any particular order.
		/// </summary>
		/// <returns> an iterator over the elements in this queue </returns>
		public virtual Iterator<E> Iterator()
		{
			return new Itr(this);
		}

		private sealed class Itr : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal void InitializeInstanceFields()
			{
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly PriorityQueue<E> OuterInstance;

			public Itr(PriorityQueue<E> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			/// <summary>
			/// Index (into queue array) of element to be returned by
			/// subsequent call to next.
			/// </summary>
			internal int Cursor = 0;

			/// <summary>
			/// Index of element returned by most recent call to next,
			/// unless that element came from the forgetMeNot list.
			/// Set to -1 if element is deleted by a call to remove.
			/// </summary>
			internal int LastRet = -1;

			/// <summary>
			/// A queue of elements that were moved from the unvisited portion of
			/// the heap into the visited portion as a result of "unlucky" element
			/// removals during the iteration.  (Unlucky element removals are those
			/// that require a siftup instead of a siftdown.)  We must visit all of
			/// the elements in this list to complete the iteration.  We do this
			/// after we've completed the "normal" iteration.
			/// 
			/// We expect that most iterations, even those involving removals,
			/// will not need to store elements in this field.
			/// </summary>
			internal ArrayDeque<E> ForgetMeNot = null;

			/// <summary>
			/// Element returned by the most recent call to next iff that
			/// element was drawn from the forgetMeNot list.
			/// </summary>
			internal E LastRetElt = null;

			/// <summary>
			/// The modCount value that the iterator believes that the backing
			/// Queue should have.  If this expectation is violated, the iterator
			/// has detected concurrent modification.
			/// </summary>
			internal int ExpectedModCount;

			public bool HasNext()
			{
				return Cursor < outerInstance.Size_Renamed || (ForgetMeNot != null && !ForgetMeNot.Empty);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
			public E Next()
			{
				if (ExpectedModCount != outerInstance.ModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (Cursor < outerInstance.Size_Renamed)
				{
					return (E) outerInstance.Queue[LastRet = Cursor++];
				}
				if (ForgetMeNot != null)
				{
					LastRet = -1;
					LastRetElt = ForgetMeNot.Poll();
					if (LastRetElt != null)
					{
						return LastRetElt;
					}
				}
				throw new NoSuchElementException();
			}

			public void Remove()
			{
				if (ExpectedModCount != outerInstance.ModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (LastRet != -1)
				{
					E moved = OuterInstance.RemoveAt(LastRet);
					LastRet = -1;
					if (moved == null)
					{
						Cursor--;
					}
					else
					{
						if (ForgetMeNot == null)
						{
							ForgetMeNot = new ArrayDeque<>();
						}
						ForgetMeNot.Add(moved);
					}
				}
				else if (LastRetElt != null)
				{
					OuterInstance.RemoveEq(LastRetElt);
					LastRetElt = null;
				}
				else
				{
					throw new IllegalStateException();
				}
				ExpectedModCount = outerInstance.ModCount;
			}
		}

		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Removes all of the elements from this priority queue.
		/// The queue will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			ModCount++;
			for (int i = 0; i < Size_Renamed; i++)
			{
				Queue[i] = null;
			}
			Size_Renamed = 0;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E poll()
		public virtual E Poll()
		{
			if (Size_Renamed == 0)
			{
				return null;
			}
			int s = --Size_Renamed;
			ModCount++;
			E result = (E) Queue[0];
			E x = (E) Queue[s];
			Queue[s] = null;
			if (s != 0)
			{
				SiftDown(0, x);
			}
			return result;
		}

		/// <summary>
		/// Removes the ith element from queue.
		/// 
		/// Normally this method leaves the elements at up to i-1,
		/// inclusive, untouched.  Under these circumstances, it returns
		/// null.  Occasionally, in order to maintain the heap invariant,
		/// it must swap a later element of the list with one earlier than
		/// i.  Under these circumstances, this method returns the element
		/// that was previously at the end of the list and is now at some
		/// position before i. This fact is used by iterator.remove so as to
		/// avoid missing traversing elements.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private E removeAt(int i)
		private E RemoveAt(int i)
		{
			// assert i >= 0 && i < size;
			ModCount++;
			int s = --Size_Renamed;
			if (s == i) // removed last element
			{
				Queue[i] = null;
			}
			else
			{
				E moved = (E) Queue[s];
				Queue[s] = null;
				SiftDown(i, moved);
				if (Queue[i] == moved)
				{
					SiftUp(i, moved);
					if (Queue[i] != moved)
					{
						return moved;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Inserts item x at position k, maintaining heap invariant by
		/// promoting x up the tree until it is greater than or equal to
		/// its parent, or is the root.
		/// 
		/// To simplify and speed up coercions and comparisons. the
		/// Comparable and Comparator versions are separated into different
		/// methods that are otherwise identical. (Similarly for siftDown.)
		/// </summary>
		/// <param name="k"> the position to fill </param>
		/// <param name="x"> the item to insert </param>
		private void SiftUp(int k, E x)
		{
			if (Comparator_Renamed != null)
			{
				SiftUpUsingComparator(k, x);
			}
			else
			{
				SiftUpComparable(k, x);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void siftUpComparable(int k, E x)
		private void SiftUpComparable(int k, E x)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base E> key = (Comparable<? base E>) x;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparable<?> key = (Comparable<?>) x;
			while (k > 0)
			{
				int parent = (int)((uint)(k - 1) >> 1);
				Object e = Queue[parent];
				if (key.CompareTo((E) e) >= 0)
				{
					break;
				}
				Queue[k] = e;
				k = parent;
			}
			Queue[k] = key;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void siftUpUsingComparator(int k, E x)
		private void SiftUpUsingComparator(int k, E x)
		{
			while (k > 0)
			{
				int parent = (int)((uint)(k - 1) >> 1);
				Object e = Queue[parent];
				if (Comparator_Renamed.Compare(x, (E) e) >= 0)
				{
					break;
				}
				Queue[k] = e;
				k = parent;
			}
			Queue[k] = x;
		}

		/// <summary>
		/// Inserts item x at position k, maintaining heap invariant by
		/// demoting x down the tree repeatedly until it is less than or
		/// equal to its children or is a leaf.
		/// </summary>
		/// <param name="k"> the position to fill </param>
		/// <param name="x"> the item to insert </param>
		private void SiftDown(int k, E x)
		{
			if (Comparator_Renamed != null)
			{
				SiftDownUsingComparator(k, x);
			}
			else
			{
				SiftDownComparable(k, x);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void siftDownComparable(int k, E x)
		private void SiftDownComparable(int k, E x)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base E> key = (Comparable<? base E>)x;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparable<?> key = (Comparable<?>)x;
			int half = (int)((uint)Size_Renamed >> 1); // loop while a non-leaf
			while (k < half)
			{
				int child = (k << 1) + 1; // assume left child is least
				Object c = Queue[child];
				int right = child + 1;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: if (right < size && ((Comparable<? base E>) c).compareTo((E) queue[right]) > 0)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				if (right < Size_Renamed && ((Comparable<?>) c).CompareTo((E) Queue[right]) > 0)
				{
					c = Queue[child = right];
				}
				if (key.CompareTo((E) c) <= 0)
				{
					break;
				}
				Queue[k] = c;
				k = child;
			}
			Queue[k] = key;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void siftDownUsingComparator(int k, E x)
		private void SiftDownUsingComparator(int k, E x)
		{
			int half = (int)((uint)Size_Renamed >> 1);
			while (k < half)
			{
				int child = (k << 1) + 1;
				Object c = Queue[child];
				int right = child + 1;
				if (right < Size_Renamed && Comparator_Renamed.Compare((E) c, (E) Queue[right]) > 0)
				{
					c = Queue[child = right];
				}
				if (Comparator_Renamed.Compare(x, (E) c) <= 0)
				{
					break;
				}
				Queue[k] = c;
				k = child;
			}
			Queue[k] = x;
		}

		/// <summary>
		/// Establishes the heap invariant (described above) in the entire tree,
		/// assuming nothing about the order of the elements prior to the call.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void heapify()
		private void Heapify()
		{
			for (int i = ((int)((uint)Size_Renamed >> 1)) - 1; i >= 0; i--)
			{
				SiftDown(i, (E) Queue[i]);
			}
		}

		/// <summary>
		/// Returns the comparator used to order the elements in this
		/// queue, or {@code null} if this queue is sorted according to
		/// the <seealso cref="Comparable natural ordering"/> of its elements.
		/// </summary>
		/// <returns> the comparator used to order this queue, or
		///         {@code null} if this queue is sorted according to the
		///         natural ordering of its elements </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual Comparator<?> Comparator()
		{
			return Comparator_Renamed;
		}

		/// <summary>
		/// Saves this queue to a stream (that is, serializes it).
		/// 
		/// @serialData The length of the array backing the instance is
		///             emitted (int), followed by all of its elements
		///             (each an {@code Object}) in the proper order. </summary>
		/// <param name="s"> the stream </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out element count, and any hidden stuff
			s.DefaultWriteObject();

			// Write out array length, for compatibility with 1.5 version
			s.WriteInt(System.Math.Max(2, Size_Renamed + 1));

			// Write out all elements in the "proper order".
			for (int i = 0; i < Size_Renamed; i++)
			{
				s.WriteObject(Queue[i]);
			}
		}

		/// <summary>
		/// Reconstitutes the {@code PriorityQueue} instance from a stream
		/// (that is, deserializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in size, and any hidden stuff
			s.DefaultReadObject();

			// Read in (and discard) array length
			s.ReadInt();

			Queue = new Object[Size_Renamed];

			// Read in all elements.
			for (int i = 0; i < Size_Renamed; i++)
			{
				Queue[i] = s.ReadObject();
			}

			// Elements are guaranteed to be in "proper order", but the
			// spec has never explained what that might be.
			Heapify();
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// queue.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, and <seealso cref="Spliterator#NONNULL"/>.
		/// Overriding implementations should document the reporting of additional
		/// characteristic values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this queue
		/// @since 1.8 </returns>
		public Spliterator<E> Spliterator()
		{
			return new PriorityQueueSpliterator<E>(this, 0, -1, 0);
		}

		internal sealed class PriorityQueueSpliterator<E> : Spliterator<E>
		{
			/*
			 * This is very similar to ArrayList Spliterator, except for
			 * extra null checks.
			 */
			internal readonly PriorityQueue<E> Pq;
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // -1 until first use
			internal int ExpectedModCount; // initialized when fence set

			/// <summary>
			/// Creates new spliterator covering the given range </summary>
			internal PriorityQueueSpliterator(PriorityQueue<E> pq, int origin, int fence, int expectedModCount)
			{
				this.Pq = pq;
				this.Index = origin;
				this.Fence_Renamed = fence;
				this.ExpectedModCount = expectedModCount;
			}

			internal int Fence
			{
				get
				{
					int hi;
					if ((hi = Fence_Renamed) < 0)
					{
						ExpectedModCount = Pq.ModCount;
						hi = Fence_Renamed = Pq.Size_Renamed;
					}
					return hi;
				}
			}

			public PriorityQueueSpliterator<E> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? null : new PriorityQueueSpliterator<E>(Pq, lo, Index = mid, ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int i, hi, mc; // hoist accesses and checks from loop
				PriorityQueue<E> q;
				Object[] a;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((q = Pq) != null && (a = q.Queue) != null)
				{
					if ((hi = Fence_Renamed) < 0)
					{
						mc = q.ModCount;
						hi = q.Size_Renamed;
					}
					else
					{
						mc = ExpectedModCount;
					}
					if ((i = Index) >= 0 && (Index = hi) <= a.Length)
					{
						for (E e;; ++i)
						{
							if (i < hi)
							{
								if ((e = (E) a[i]) == null) // must be CME
								{
									break;
								}
								action.Accept(e);
							}
							else if (q.ModCount != mc)
							{
								break;
							}
							else
							{
								return;
							}
						}
					}
				}
				throw new ConcurrentModificationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base E> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				int hi = Fence, lo = Index;
				if (lo >= 0 && lo < hi)
				{
					Index = lo + 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E)pq.queue[lo];
					E e = (E)Pq.Queue[lo];
					if (e == null)
					{
						throw new ConcurrentModificationException();
					}
					action.Accept(e);
					if (Pq.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					return true;
				}
				return false;
			}

			public long EstimateSize()
			{
				return (long)(Fence - Index);
			}

			public int Characteristics()
			{
				return Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.NONNULL;
			}
		}
	}

}