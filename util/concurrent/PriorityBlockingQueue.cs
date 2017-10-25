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
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{


	/// <summary>
	/// An unbounded <seealso cref="BlockingQueue blocking queue"/> that uses
	/// the same ordering rules as class <seealso cref="PriorityQueue"/> and supplies
	/// blocking retrieval operations.  While this queue is logically
	/// unbounded, attempted additions may fail due to resource exhaustion
	/// (causing {@code OutOfMemoryError}). This class does not permit
	/// {@code null} elements.  A priority queue relying on {@linkplain
	/// Comparable natural ordering} also does not permit insertion of
	/// non-comparable objects (doing so results in
	/// {@code ClassCastException}).
	/// 
	/// <para>This class and its iterator implement all of the
	/// <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	/// Iterator} interfaces.  The Iterator provided in method {@link
	/// #iterator()} is <em>not</em> guaranteed to traverse the elements of
	/// the PriorityBlockingQueue in any particular order. If you need
	/// ordered traversal, consider using
	/// {@code Arrays.sort(pq.toArray())}.  Also, method {@code drainTo}
	/// can be used to <em>remove</em> some or all elements in priority
	/// order and place them in another collection.
	/// 
	/// </para>
	/// <para>Operations on this class make no guarantees about the ordering
	/// of elements with equal priority. If you need to enforce an
	/// ordering, you can define custom classes or comparators that use a
	/// secondary key to break ties in primary priority values.  For
	/// example, here is a class that applies first-in-first-out
	/// tie-breaking to comparable elements. To use it, you would insert a
	/// {@code new FIFOEntry(anEntry)} instead of a plain entry object.
	/// 
	///  <pre> {@code
	/// class FIFOEntry<E extends Comparable<? super E>>
	///     implements Comparable<FIFOEntry<E>> {
	///   static final AtomicLong seq = new AtomicLong(0);
	///   final long seqNum;
	///   final E entry;
	///   public FIFOEntry(E entry) {
	///     seqNum = seq.getAndIncrement();
	///     this.entry = entry;
	///   }
	///   public E getEntry() { return entry; }
	///   public int compareTo(FIFOEntry<E> other) {
	///     int res = entry.compareTo(other.entry);
	///     if (res == 0 && other.entry != this.entry)
	///       res = (seqNum < other.seqNum ? -1 : 1);
	///     return res;
	///   }
	/// }}</pre>
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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public class PriorityBlockingQueue<E> extends java.util.AbstractQueue<E> implements BlockingQueue<E>, java.io.Serializable
	[Serializable]
	public class PriorityBlockingQueue<E> : AbstractQueue<E>, BlockingQueue<E>
	{
		private const long SerialVersionUID = 5595510919245408276L;

		/*
		 * The implementation uses an array-based binary heap, with public
		 * operations protected with a single lock. However, allocation
		 * during resizing uses a simple spinlock (used only while not
		 * holding main lock) in order to allow takes to operate
		 * concurrently with allocation.  This avoids repeated
		 * postponement of waiting consumers and consequent element
		 * build-up. The need to back away from lock during allocation
		 * makes it impossible to simply wrap delegated
		 * java.util.PriorityQueue operations within a lock, as was done
		 * in a previous version of this class. To maintain
		 * interoperability, a plain PriorityQueue is still used during
		 * serialization, which maintains compatibility at the expense of
		 * transiently doubling overhead.
		 */

		/// <summary>
		/// Default array capacity.
		/// </summary>
		private const int DEFAULT_INITIAL_CAPACITY = 11;

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Priority queue represented as a balanced binary heap: the two
		/// children of queue[n] are queue[2*n+1] and queue[2*(n+1)].  The
		/// priority queue is ordered by comparator, or by the elements'
		/// natural ordering, if comparator is null: For each node n in the
		/// heap and each descendant d of n, n <= d.  The element with the
		/// lowest value is in queue[0], assuming the queue is nonempty.
		/// </summary>
		[NonSerialized]
		private Object[] Queue;

		/// <summary>
		/// The number of elements in the priority queue.
		/// </summary>
		[NonSerialized]
		private int Size_Renamed;

		/// <summary>
		/// The comparator, or null if priority queue uses elements'
		/// natural ordering.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private transient java.util.Comparator<? base E> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		[NonSerialized]
		private IComparer<?> Comparator_Renamed;

		/// <summary>
		/// Lock used for all public operations
		/// </summary>
		private readonly ReentrantLock @lock;

		/// <summary>
		/// Condition for blocking when empty
		/// </summary>
		private readonly Condition NotEmpty;

		/// <summary>
		/// Spinlock for allocation, acquired via CAS.
		/// </summary>
		[NonSerialized]
		private volatile int AllocationSpinLock;

		/// <summary>
		/// A plain PriorityQueue used only for serialization,
		/// to maintain compatibility with previous versions
		/// of this class. Non-null only during serialization/deserialization.
		/// </summary>
		private PriorityQueue<E> q;

		/// <summary>
		/// Creates a {@code PriorityBlockingQueue} with the default
		/// initial capacity (11) that orders its elements according to
		/// their <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		public PriorityBlockingQueue() : this(DEFAULT_INITIAL_CAPACITY, null)
		{
		}

		/// <summary>
		/// Creates a {@code PriorityBlockingQueue} with the specified
		/// initial capacity that orders its elements according to their
		/// <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity for this priority queue </param>
		/// <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		///         than 1 </exception>
		public PriorityBlockingQueue(int initialCapacity) : this(initialCapacity, null)
		{
		}

		/// <summary>
		/// Creates a {@code PriorityBlockingQueue} with the specified initial
		/// capacity that orders its elements according to the specified
		/// comparator.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity for this priority queue </param>
		/// <param name="comparator"> the comparator that will be used to order this
		///         priority queue.  If {@code null}, the {@link Comparable
		///         natural ordering} of the elements will be used. </param>
		/// <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		///         than 1 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public PriorityBlockingQueue(int initialCapacity, java.util.Comparator<? base E> comparator)
		public PriorityBlockingQueue<T1>(int initialCapacity, IComparer<T1> comparator)
		{
			if (initialCapacity < 1)
			{
				throw new IllegalArgumentException();
			}
			this.@lock = new ReentrantLock();
			this.NotEmpty = @lock.NewCondition();
			this.Comparator_Renamed = comparator;
			this.Queue = new Object[initialCapacity];
		}

		/// <summary>
		/// Creates a {@code PriorityBlockingQueue} containing the elements
		/// in the specified collection.  If the specified collection is a
		/// <seealso cref="SortedSet"/> or a <seealso cref="PriorityQueue"/>, this
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
		public PriorityBlockingQueue<T1>(ICollection<T1> c) where T1 : E
		{
			this.@lock = new ReentrantLock();
			this.NotEmpty = @lock.NewCondition();
			bool heapify = true; // true if not known to be in heap order
			bool screen = true; // true if must screen for nulls
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (c instanceof java.util.SortedSet<?>)
			if (c is SortedSet<?>)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.SortedSet<? extends E> ss = (java.util.SortedSet<? extends E>) c;
				SortedSet<?> ss = (SortedSet<?>) c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (java.util.Comparator<? base E>) ss.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				this.Comparator_Renamed = (IComparer<?>) ss.Comparator();
				heapify = false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: else if (c instanceof PriorityBlockingQueue<?>)
			else if (c is PriorityBlockingQueue<?>)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: PriorityBlockingQueue<? extends E> pq = (PriorityBlockingQueue<? extends E>) c;
				PriorityBlockingQueue<?> pq = (PriorityBlockingQueue<?>) c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: this.comparator = (java.util.Comparator<? base E>) pq.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				this.Comparator_Renamed = (IComparer<?>) pq.Comparator();
				screen = false;
				if (pq.GetType() == typeof(PriorityBlockingQueue)) // exact match
				{
					heapify = false;
				}
			}
			Object[] a = c.ToArray();
			int n = a.Length;
			// If c.toArray incorrectly doesn't return Object[], copy it.
			if (a.GetType() != typeof(Object[]))
			{
				a = Arrays.CopyOf(a, n, typeof(Object[]));
			}
			if (screen && (n == 1 || this.Comparator_Renamed != null))
			{
				for (int i = 0; i < n; ++i)
				{
					if (a[i] == null)
					{
						throw new NullPointerException();
					}
				}
			}
			this.Queue = a;
			this.Size_Renamed = n;
			if (heapify)
			{
				Heapify();
			}
		}

		/// <summary>
		/// Tries to grow array to accommodate at least one more element
		/// (but normally expand by about 50%), giving up (allowing retry)
		/// on contention (which we expect to be rare). Call only while
		/// holding lock.
		/// </summary>
		/// <param name="array"> the heap array </param>
		/// <param name="oldCap"> the length of the array </param>
		private void TryGrow(Object[] array, int oldCap)
		{
			@lock.Unlock(); // must release and then re-acquire main lock
			Object[] newArray = null;
			if (AllocationSpinLock == 0 && UNSAFE.compareAndSwapInt(this, AllocationSpinLockOffset, 0, 1))
			{
				try
				{
					int newCap = oldCap + ((oldCap < 64) ? (oldCap + 2) : (oldCap >> 1)); // grow faster if small
					if (newCap - MAX_ARRAY_SIZE > 0) // possible overflow
					{
						int minCap = oldCap + 1;
						if (minCap < 0 || minCap > MAX_ARRAY_SIZE)
						{
							throw new OutOfMemoryError();
						}
						newCap = MAX_ARRAY_SIZE;
					}
					if (newCap > oldCap && Queue == array)
					{
						newArray = new Object[newCap];
					}
				}
				finally
				{
					AllocationSpinLock = 0;
				}
			}
			if (newArray == null) // back off if another thread is allocating
			{
				Thread.@yield();
			}
			@lock.@lock();
			if (newArray != null && Queue == array)
			{
				Queue = newArray;
				System.Array.Copy(array, 0, newArray, 0, oldCap);
			}
		}

		/// <summary>
		/// Mechanics for poll().  Call only while holding lock.
		/// </summary>
		private E Dequeue()
		{
			int n = Size_Renamed - 1;
			if (n < 0)
			{
				return null;
			}
			else
			{
				Object[] array = Queue;
				E result = (E) array[0];
				E x = (E) array[n];
				array[n] = null;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base E> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator_Renamed;
				if (cmp == null)
				{
					SiftDownComparable(0, x, array, n);
				}
				else
				{
					SiftDownUsingComparator(0, x, array, n, cmp);
				}
				Size_Renamed = n;
				return result;
			}
		}

		/// <summary>
		/// Inserts item x at position k, maintaining heap invariant by
		/// promoting x up the tree until it is greater than or equal to
		/// its parent, or is the root.
		/// 
		/// To simplify and speed up coercions and comparisons. the
		/// Comparable and Comparator versions are separated into different
		/// methods that are otherwise identical. (Similarly for siftDown.)
		/// These methods are static, with heap state as arguments, to
		/// simplify use in light of possible comparator exceptions.
		/// </summary>
		/// <param name="k"> the position to fill </param>
		/// <param name="x"> the item to insert </param>
		/// <param name="array"> the heap array </param>
		private static void siftUpComparable<T>(int k, T x, Object[] array)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base T> key = (Comparable<? base T>) x;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparable<?> key = (Comparable<?>) x;
			while (k > 0)
			{
				int parent = (int)((uint)(k - 1) >> 1);
				Object e = array[parent];
				if (key.CompareTo((T) e) >= 0)
				{
					break;
				}
				array[k] = e;
				k = parent;
			}
			array[k] = key;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> void siftUpUsingComparator(int k, T x, Object[] array, java.util.Comparator<? base T> cmp)
		private static void siftUpUsingComparator<T, T1>(int k, T x, Object[] array, IComparer<T1> cmp)
		{
			while (k > 0)
			{
				int parent = (int)((uint)(k - 1) >> 1);
				Object e = array[parent];
				if (cmp.Compare(x, (T) e) >= 0)
				{
					break;
				}
				array[k] = e;
				k = parent;
			}
			array[k] = x;
		}

		/// <summary>
		/// Inserts item x at position k, maintaining heap invariant by
		/// demoting x down the tree repeatedly until it is less than or
		/// equal to its children or is a leaf.
		/// </summary>
		/// <param name="k"> the position to fill </param>
		/// <param name="x"> the item to insert </param>
		/// <param name="array"> the heap array </param>
		/// <param name="n"> heap size </param>
		private static void siftDownComparable<T>(int k, T x, Object[] array, int n)
		{
			if (n > 0)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base T> key = (Comparable<? base T>)x;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Comparable<?> key = (Comparable<?>)x;
				int half = (int)((uint)n >> 1); // loop while a non-leaf
				while (k < half)
				{
					int child = (k << 1) + 1; // assume left child is least
					Object c = array[child];
					int right = child + 1;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: if (right < n && ((Comparable<? base T>) c).compareTo((T) array[right]) > 0)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					if (right < n && ((Comparable<?>) c).CompareTo((T) array[right]) > 0)
					{
						c = array[child = right];
					}
					if (key.CompareTo((T) c) <= 0)
					{
						break;
					}
					array[k] = c;
					k = child;
				}
				array[k] = key;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> void siftDownUsingComparator(int k, T x, Object[] array, int n, java.util.Comparator<? base T> cmp)
		private static void siftDownUsingComparator<T, T1>(int k, T x, Object[] array, int n, IComparer<T1> cmp)
		{
			if (n > 0)
			{
				int half = (int)((uint)n >> 1);
				while (k < half)
				{
					int child = (k << 1) + 1;
					Object c = array[child];
					int right = child + 1;
					if (right < n && cmp.Compare((T) c, (T) array[right]) > 0)
					{
						c = array[child = right];
					}
					if (cmp.Compare(x, (T) c) <= 0)
					{
						break;
					}
					array[k] = c;
					k = child;
				}
				array[k] = x;
			}
		}

		/// <summary>
		/// Establishes the heap invariant (described above) in the entire tree,
		/// assuming nothing about the order of the elements prior to the call.
		/// </summary>
		private void Heapify()
		{
			Object[] array = Queue;
			int n = Size_Renamed;
			int half = ((int)((uint)n >> 1)) - 1;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base E> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			if (cmp == null)
			{
				for (int i = half; i >= 0; i--)
				{
					SiftDownComparable(i, (E) array[i], array, n);
				}
			}
			else
			{
				for (int i = half; i >= 0; i--)
				{
					SiftDownUsingComparator(i, (E) array[i], array, n, cmp);
				}
			}
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be compared
		///         with elements currently in the priority queue according to the
		///         priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return Offer(e);
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// As the queue is unbounded, this method will never return {@code false}.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be compared
		///         with elements currently in the priority queue according to the
		///         priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			int n, cap;
			Object[] array;
			while ((n = Size_Renamed) >= (cap = (array = Queue).Length))
			{
				TryGrow(array, cap);
			}
			try
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base E> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator_Renamed;
				if (cmp == null)
				{
					SiftUpComparable(n, e, array);
				}
				else
				{
					SiftUpUsingComparator(n, e, array, cmp);
				}
				Size_Renamed = n + 1;
				NotEmpty.Signal();
			}
			finally
			{
				@lock.Unlock();
			}
			return true;
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// As the queue is unbounded, this method will never block.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="ClassCastException"> if the specified element cannot be compared
		///         with elements currently in the priority queue according to the
		///         priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void Put(E e)
		{
			Offer(e); // never need to block
		}

		/// <summary>
		/// Inserts the specified element into this priority queue.
		/// As the queue is unbounded, this method will never block or
		/// return {@code false}.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <param name="timeout"> This parameter is ignored as the method never blocks </param>
		/// <param name="unit"> This parameter is ignored as the method never blocks </param>
		/// <returns> {@code true} (as specified by
		///  <seealso cref="BlockingQueue#offer(Object,long,TimeUnit) BlockingQueue.offer"/>) </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be compared
		///         with elements currently in the priority queue according to the
		///         priority queue's ordering </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{
			return Offer(e); // never need to block
		}

		public virtual E Poll()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return Dequeue();
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E take() throws InterruptedException
		public virtual E Take()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			E result;
			try
			{
				while ((result = Dequeue()) == null)
				{
					NotEmpty.@await();
				}
			}
			finally
			{
				@lock.Unlock();
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E poll(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E Poll(long timeout, TimeUnit unit)
		{
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			E result;
			try
			{
				while ((result = Dequeue()) == null && nanos > 0)
				{
					nanos = NotEmpty.AwaitNanos(nanos);
				}
			}
			finally
			{
				@lock.Unlock();
			}
			return result;
		}

		public virtual E Peek()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return (Size_Renamed == 0) ? null : (E) Queue[0];
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns the comparator used to order the elements in this queue,
		/// or {@code null} if this queue uses the {@link Comparable
		/// natural ordering} of its elements.
		/// </summary>
		/// <returns> the comparator used to order the elements in this queue,
		///         or {@code null} if this queue uses the natural
		///         ordering of its elements </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public java.util.Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual IComparer<?> Comparator()
		{
			return Comparator_Renamed;
		}

		public virtual int Size()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return Size_Renamed;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Always returns {@code Integer.MAX_VALUE} because
		/// a {@code PriorityBlockingQueue} is not capacity constrained. </summary>
		/// <returns> {@code Integer.MAX_VALUE} always </returns>
		public virtual int RemainingCapacity()
		{
			return Integer.MaxValue;
		}

		private int IndexOf(Object o)
		{
			if (o != null)
			{
				Object[] array = Queue;
				int n = Size_Renamed;
				for (int i = 0; i < n; i++)
				{
					if (o.Equals(array[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Removes the ith element from queue.
		/// </summary>
		private void RemoveAt(int i)
		{
			Object[] array = Queue;
			int n = Size_Renamed - 1;
			if (n == i) // removed last element
			{
				array[i] = null;
			}
			else
			{
				E moved = (E) array[n];
				array[n] = null;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base E> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator_Renamed;
				if (cmp == null)
				{
					SiftDownComparable(i, moved, array, n);
				}
				else
				{
					SiftDownUsingComparator(i, moved, array, n, cmp);
				}
				if (array[i] == moved)
				{
					if (cmp == null)
					{
						SiftUpComparable(i, moved, array);
					}
					else
					{
						SiftUpUsingComparator(i, moved, array, cmp);
					}
				}
			}
			Size_Renamed = n;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int i = IndexOf(o);
				if (i == -1)
				{
					return false;
				}
				RemoveAt(i);
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Identity-based version for use in Itr.remove
		/// </summary>
		internal virtual void RemoveEQ(Object o)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] array = Queue;
				for (int i = 0, n = Size_Renamed; i < n; i++)
				{
					if (o == array[i])
					{
						RemoveAt(i);
						break;
					}
				}
			}
			finally
			{
				@lock.Unlock();
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return IndexOf(o) != -1;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue.
		/// The returned array elements are in no particular order.
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return Arrays.CopyOf(Queue, Size_Renamed);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int n = Size_Renamed;
				if (n == 0)
				{
					return "[]";
				}
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (int i = 0; i < n; ++i)
				{
					Object e = Queue[i];
					sb.Append(e == this ? "(this Collection)" : e);
					if (i != n - 1)
					{
						sb.Append(',').Append(' ');
					}
				}
				return sb.Append(']').ToString();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(java.util.Collection<? base E> c)
		public virtual int drainTo<T1>(ICollection<T1> c)
		{
			return DrainTo(c, Integer.MaxValue);
		}

		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
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
			if (maxElements <= 0)
			{
				return 0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int n = System.Math.Min(Size_Renamed, maxElements);
				for (int i = 0; i < n; i++)
				{
					c.Add((E) Queue[0]); // In this order, in case add() throws.
					Dequeue();
				}
				return n;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Atomically removes all of the elements from this queue.
		/// The queue will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] array = Queue;
				int n = Size_Renamed;
				Size_Renamed = 0;
				for (int i = 0; i < n; i++)
				{
					array[i] = null;
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this queue; the
		/// runtime type of the returned array is that of the specified array.
		/// The returned array elements are in no particular order.
		/// If the queue fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of the
		/// specified array and the size of this queue.
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
		public virtual T[] toArray<T>(T[] a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int n = Size_Renamed;
				if (a.Length < n)
				{
					// Make a new array of a's runtime type, but my contents:
					return (T[]) Arrays.CopyOf(Queue, Size_Renamed, a.GetType());
				}
				System.Array.Copy(Queue, 0, a, 0, n);
				if (a.Length > n)
				{
					a[n] = null;
				}
				return a;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns an iterator over the elements in this queue. The
		/// iterator does not return the elements in any particular order.
		/// 
		/// <para>The returned iterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this queue </returns>
		public virtual IEnumerator<E> Iterator()
		{
			return new Itr(this, ToArray());
		}

		/// <summary>
		/// Snapshot iterator that works off copy of underlying q array.
		/// </summary>
		internal sealed class Itr : Iterator<E>
		{
			private readonly PriorityBlockingQueue<E> OuterInstance;

			internal readonly Object[] Array; // Array of all elements
			internal int Cursor; // index of next element to return
			internal int LastRet; // index of last element, or -1 if no such

			internal Itr(PriorityBlockingQueue<E> outerInstance, Object[] array)
			{
				this.OuterInstance = outerInstance;
				LastRet = -1;
				this.Array = array;
			}

			public bool HasNext()
			{
				return Cursor < Array.Length;
			}

			public E Next()
			{
				if (Cursor >= Array.Length)
				{
					throw new NoSuchElementException();
				}
				LastRet = Cursor;
				return (E)Array[Cursor++];
			}

			public void Remove()
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				outerInstance.RemoveEQ(Array[LastRet]);
				LastRet = -1;
			}
		}

		/// <summary>
		/// Saves this queue to a stream (that is, serializes it).
		/// 
		/// For compatibility with previous version of this class, elements
		/// are first copied to a java.util.PriorityQueue, which is then
		/// serialized.
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			@lock.@lock();
			try
			{
				// avoid zero capacity argument
				q = new PriorityQueue<E>(System.Math.Max(Size_Renamed, 1), Comparator_Renamed);
				q.AddAll(this);
				s.DefaultWriteObject();
			}
			finally
			{
				q = null;
				@lock.Unlock();
			}
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
			try
			{
				s.DefaultReadObject();
				this.Queue = new Object[q.Size()];
				Comparator_Renamed = q.Comparator();
				AddAll(q);
			}
			finally
			{
				q = null;
			}
		}

		// Similar to Collections.ArraySnapshotSpliterator but avoids
		// commitment to toArray until needed
		internal sealed class PBQSpliterator<E> : Spliterator<E>
		{
			internal readonly PriorityBlockingQueue<E> Queue;
			internal Object[] Array;
			internal int Index;
			internal int Fence_Renamed;

			internal PBQSpliterator(PriorityBlockingQueue<E> queue, Object[] array, int index, int fence)
			{
				this.Queue = queue;
				this.Array = array;
				this.Index = index;
				this.Fence_Renamed = fence;
			}

			internal int Fence
			{
				get
				{
					int hi;
					if ((hi = Fence_Renamed) < 0)
					{
						hi = Fence_Renamed = (Array = Queue.ToArray()).Length;
					}
					return hi;
				}
			}

			public Spliterator<E> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? null : new PBQSpliterator<E>(Queue, Array, lo, Index = mid);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				Object[] a; // hoist accesses and checks from loop
				int i, hi;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((a = Array) == null)
				{
					Fence_Renamed = (a = Queue.ToArray()).Length;
				}
				if ((hi = Fence_Renamed) <= a.Length && (i = Index) >= 0 && i < (Index = hi))
				{
					do
					{
						action.Accept((E)a[i]);
					} while (++i < hi);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base E> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Fence > Index && Index >= 0)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) array[index++];
					E e = (E) Array[Index++];
					action.Accept(e);
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
				return java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> over the elements in this queue.
		/// 
		/// <para>The returned spliterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#NONNULL"/>.
		/// 
		/// @implNote
		/// The {@code Spliterator} additionally reports <seealso cref="Spliterator#SUBSIZED"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this queue
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return new PBQSpliterator<E>(this, null, 0, -1);
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long AllocationSpinLockOffset;
		static PriorityBlockingQueue()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(PriorityBlockingQueue);
				AllocationSpinLockOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("allocationSpinLock"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}