using System;
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
	/// A bounded <seealso cref="BlockingQueue blocking queue"/> backed by an
	/// array.  This queue orders elements FIFO (first-in-first-out).  The
	/// <em>head</em> of the queue is that element that has been on the
	/// queue the longest time.  The <em>tail</em> of the queue is that
	/// element that has been on the queue the shortest time. New elements
	/// are inserted at the tail of the queue, and the queue retrieval
	/// operations obtain elements at the head of the queue.
	/// 
	/// <para>This is a classic &quot;bounded buffer&quot;, in which a
	/// fixed-sized array holds elements inserted by producers and
	/// extracted by consumers.  Once created, the capacity cannot be
	/// changed.  Attempts to {@code put} an element into a full queue
	/// will result in the operation blocking; attempts to {@code take} an
	/// element from an empty queue will similarly block.
	/// 
	/// </para>
	/// <para>This class supports an optional fairness policy for ordering
	/// waiting producer and consumer threads.  By default, this ordering
	/// is not guaranteed. However, a queue constructed with fairness set
	/// to {@code true} grants threads access in FIFO order. Fairness
	/// generally decreases throughput but reduces variability and avoids
	/// starvation.
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
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class ArrayBlockingQueue<E> : AbstractQueue<E>, BlockingQueue<E>
	{

		/// <summary>
		/// Serialization ID. This class relies on default serialization
		/// even for the items array, which is default-serialized, even if
		/// it is empty. Otherwise it could not be declared final, which is
		/// necessary here.
		/// </summary>
		private const long SerialVersionUID = -817911632652898426L;

		/// <summary>
		/// The queued items </summary>
		internal readonly Object[] Items;

		/// <summary>
		/// items index for next take, poll, peek or remove </summary>
		internal int TakeIndex;

		/// <summary>
		/// items index for next put, offer, or add </summary>
		internal int PutIndex;

		/// <summary>
		/// Number of elements in the queue </summary>
		internal int Count;

		/*
		 * Concurrency control uses the classic two-condition algorithm
		 * found in any textbook.
		 */

		/// <summary>
		/// Main lock guarding all access </summary>
		internal readonly ReentrantLock @lock;

		/// <summary>
		/// Condition for waiting takes </summary>
		private readonly Condition NotEmpty;

		/// <summary>
		/// Condition for waiting puts </summary>
		private readonly Condition NotFull;

		/// <summary>
		/// Shared state for currently active iterators, or null if there
		/// are known not to be any.  Allows queue operations to update
		/// iterator state.
		/// </summary>
		[NonSerialized]
		internal Itrs Itrs = null;

		// Internal helper methods

		/// <summary>
		/// Circularly decrement i.
		/// </summary>
		internal int Dec(int i)
		{
			return ((i == 0) ? Items.Length : i) - 1;
		}

		/// <summary>
		/// Returns item at index i.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final E itemAt(int i)
		internal E ItemAt(int i)
		{
			return (E) Items[i];
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

		/// <summary>
		/// Inserts element at current put position, advances, and signals.
		/// Call only when holding lock.
		/// </summary>
		private void Enqueue(E x)
		{
			// assert lock.getHoldCount() == 1;
			// assert items[putIndex] == null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
			items[PutIndex] = x;
			if (++PutIndex == items.Length)
			{
				PutIndex = 0;
			}
			Count++;
			NotEmpty.Signal();
		}

		/// <summary>
		/// Extracts element at current take position, advances, and signals.
		/// Call only when holding lock.
		/// </summary>
		private E Dequeue()
		{
			// assert lock.getHoldCount() == 1;
			// assert items[takeIndex] != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E x = (E) items[takeIndex];
			E x = (E) items[TakeIndex];
			items[TakeIndex] = null;
			if (++TakeIndex == items.Length)
			{
				TakeIndex = 0;
			}
			Count--;
			if (Itrs != null)
			{
				Itrs.ElementDequeued();
			}
			NotFull.Signal();
			return x;
		}

		/// <summary>
		/// Deletes item at array index removeIndex.
		/// Utility for remove(Object) and iterator.remove.
		/// Call only when holding lock.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void removeAt(final int removeIndex)
		internal virtual void RemoveAt(int removeIndex)
		{
			// assert lock.getHoldCount() == 1;
			// assert items[removeIndex] != null;
			// assert removeIndex >= 0 && removeIndex < items.length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
			if (removeIndex == TakeIndex)
			{
				// removing front item; just advance
				items[TakeIndex] = null;
				if (++TakeIndex == items.Length)
				{
					TakeIndex = 0;
				}
				Count--;
				if (Itrs != null)
				{
					Itrs.ElementDequeued();
				}
			}
			else
			{
				// an "interior" remove

				// slide over all others up through putIndex.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int putIndex = this.putIndex;
				int putIndex = this.PutIndex;
				for (int i = removeIndex;;)
				{
					int next = i + 1;
					if (next == items.Length)
					{
						next = 0;
					}
					if (next != putIndex)
					{
						items[i] = items[next];
						i = next;
					}
					else
					{
						items[i] = null;
						this.PutIndex = i;
						break;
					}
				}
				Count--;
				if (Itrs != null)
				{
					Itrs.RemovedAt(removeIndex);
				}
			}
			NotFull.Signal();
		}

		/// <summary>
		/// Creates an {@code ArrayBlockingQueue} with the given (fixed)
		/// capacity and default access policy.
		/// </summary>
		/// <param name="capacity"> the capacity of this queue </param>
		/// <exception cref="IllegalArgumentException"> if {@code capacity < 1} </exception>
		public ArrayBlockingQueue(int capacity) : this(capacity, false)
		{
		}

		/// <summary>
		/// Creates an {@code ArrayBlockingQueue} with the given (fixed)
		/// capacity and the specified access policy.
		/// </summary>
		/// <param name="capacity"> the capacity of this queue </param>
		/// <param name="fair"> if {@code true} then queue accesses for threads blocked
		///        on insertion or removal, are processed in FIFO order;
		///        if {@code false} the access order is unspecified. </param>
		/// <exception cref="IllegalArgumentException"> if {@code capacity < 1} </exception>
		public ArrayBlockingQueue(int capacity, bool fair)
		{
			if (capacity <= 0)
			{
				throw new IllegalArgumentException();
			}
			this.Items = new Object[capacity];
			@lock = new ReentrantLock(fair);
			NotEmpty = @lock.NewCondition();
			NotFull = @lock.NewCondition();
		}

		/// <summary>
		/// Creates an {@code ArrayBlockingQueue} with the given (fixed)
		/// capacity, the specified access policy and initially containing the
		/// elements of the given collection,
		/// added in traversal order of the collection's iterator.
		/// </summary>
		/// <param name="capacity"> the capacity of this queue </param>
		/// <param name="fair"> if {@code true} then queue accesses for threads blocked
		///        on insertion or removal, are processed in FIFO order;
		///        if {@code false} the access order is unspecified. </param>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="IllegalArgumentException"> if {@code capacity} is less than
		///         {@code c.size()}, or less than 1. </exception>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public ArrayBlockingQueue<T1>(int capacity, bool fair, ICollection<T1> c) where T1 : E : this(capacity, fair)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock(); // Lock only for visibility, not mutual exclusion
			try
			{
				int i = 0;
				try
				{
					foreach (E e in c)
					{
						CheckNotNull(e);
						Items[i++] = e;
					}
				}
				catch (ArrayIndexOutOfBoundsException)
				{
					throw new IllegalArgumentException();
				}
				Count = i;
				PutIndex = (i == capacity) ? 0 : i;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue if it is
		/// possible to do so immediately without exceeding the queue's capacity,
		/// returning {@code true} upon success and throwing an
		/// {@code IllegalStateException} if this queue is full.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="IllegalStateException"> if this queue is full </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return base.Add(e);
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue if it is
		/// possible to do so immediately without exceeding the queue's capacity,
		/// returning {@code true} upon success and {@code false} if this queue
		/// is full.  This method is generally preferable to method <seealso cref="#add"/>,
		/// which can fail to insert an element only by throwing an exception.
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			CheckNotNull(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				if (Count == Items.Length)
				{
					return false;
				}
				else
				{
					Enqueue(e);
					return true;
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue, waiting
		/// for space to become available if the queue is full.
		/// </summary>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void put(E e) throws InterruptedException
		public virtual void Put(E e)
		{
			CheckNotNull(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				while (Count == Items.Length)
				{
					NotFull.@await();
				}
				Enqueue(e);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue, waiting
		/// up to the specified wait time for space to become available if
		/// the queue is full.
		/// </summary>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean offer(E e, long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{

			CheckNotNull(e);
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				while (Count == Items.Length)
				{
					if (nanos <= 0)
					{
						return false;
					}
					nanos = NotFull.AwaitNanos(nanos);
				}
				Enqueue(e);
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual E Poll()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return (Count == 0) ? null : Dequeue();
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
			try
			{
				while (Count == 0)
				{
					NotEmpty.@await();
				}
				return Dequeue();
			}
			finally
			{
				@lock.Unlock();
			}
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
			try
			{
				while (Count == 0)
				{
					if (nanos <= 0)
					{
						return null;
					}
					nanos = NotEmpty.AwaitNanos(nanos);
				}
				return Dequeue();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual E Peek()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return ItemAt(TakeIndex); // null when queue is empty
			}
			finally
			{
				@lock.Unlock();
			}
		}

		// this doc comment is overridden to remove the reference to collections
		// greater in size than Integer.MAX_VALUE
		/// <summary>
		/// Returns the number of elements in this queue.
		/// </summary>
		/// <returns> the number of elements in this queue </returns>
		public virtual int Size()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return Count;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		// this doc comment is a modified copy of the inherited doc comment,
		// without the reference to unlimited queues.
		/// <summary>
		/// Returns the number of additional elements that this queue can ideally
		/// (in the absence of memory or resource constraints) accept without
		/// blocking. This is always equal to the initial capacity of this queue
		/// less the current {@code size} of this queue.
		/// 
		/// <para>Note that you <em>cannot</em> always tell if an attempt to insert
		/// an element will succeed by inspecting {@code remainingCapacity}
		/// because it may be the case that another thread is about to
		/// insert or remove an element.
		/// </para>
		/// </summary>
		public virtual int RemainingCapacity()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return Items.Length - Count;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Removes a single instance of the specified element from this queue,
		/// if it is present.  More formally, removes an element {@code e} such
		/// that {@code o.equals(e)}, if this queue contains one or more such
		/// elements.
		/// Returns {@code true} if this queue contained the specified element
		/// (or equivalently, if this queue changed as a result of the call).
		/// 
		/// <para>Removal of interior elements in circular array based queues
		/// is an intrinsically slow and disruptive operation, so should
		/// be undertaken only in exceptional circumstances, ideally
		/// only when the queue is known not to be accessible by other
		/// threads.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> element to be removed from this queue, if present </param>
		/// <returns> {@code true} if this queue changed as a result of the call </returns>
		public virtual bool Remove(Object o)
		{
			if (o == null)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				if (Count > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int putIndex = this.putIndex;
					int putIndex = this.PutIndex;
					int i = TakeIndex;
					do
					{
						if (o.Equals(items[i]))
						{
							RemoveAt(i);
							return true;
						}
						if (++i == items.Length)
						{
							i = 0;
						}
					} while (i != putIndex);
				}
				return false;
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
			if (o == null)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				if (Count > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int putIndex = this.putIndex;
					int putIndex = this.PutIndex;
					int i = TakeIndex;
					do
					{
						if (o.Equals(items[i]))
						{
							return true;
						}
						if (++i == items.Length)
						{
							i = 0;
						}
					} while (i != putIndex);
				}
				return false;
			}
			finally
			{
				@lock.Unlock();
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
			Object[] a;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = this.count;
				int count = this.Count;
				a = new Object[count];
				int n = Items.Length - TakeIndex;
				if (count <= n)
				{
					System.Array.Copy(Items, TakeIndex, a, 0, count);
				}
				else
				{
					System.Array.Copy(Items, TakeIndex, a, 0, n);
					System.Array.Copy(Items, 0, a, n, count - n);
				}
			}
			finally
			{
				@lock.Unlock();
			}
			return a;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = this.count;
				int count = this.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = a.length;
				int len = a.Length;
				if (len < count)
				{
					a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), count);
				}
				int n = items.Length - TakeIndex;
				if (count <= n)
				{
					System.Array.Copy(items, TakeIndex, a, 0, count);
				}
				else
				{
					System.Array.Copy(items, TakeIndex, a, 0, n);
					System.Array.Copy(items, 0, a, n, count - n);
				}
				if (len > count)
				{
					a[count] = null;
				}
			}
			finally
			{
				@lock.Unlock();
			}
			return a;
		}

		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int k = Count;
				if (k == 0)
				{
					return "[]";
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
				Object[] items = this.Items;
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (int i = TakeIndex; ;)
				{
					Object e = items[i];
					sb.Append(e == this ? "(this Collection)" : e);
					if (--k == 0)
					{
						return sb.Append(']').ToString();
					}
					sb.Append(',').Append(' ');
					if (++i == items.Length)
					{
						i = 0;
					}
				}
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
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int k = Count;
				if (k > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int putIndex = this.putIndex;
					int putIndex = this.PutIndex;
					int i = TakeIndex;
					do
					{
						items[i] = null;
						if (++i == items.Length)
						{
							i = 0;
						}
					} while (i != putIndex);
					TakeIndex = putIndex;
					Count = 0;
					if (Itrs != null)
					{
						Itrs.QueueIsEmpty();
					}
					for (; k > 0 && @lock.HasWaiters(NotFull); k--)
					{
						NotFull.Signal();
					}
				}
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
			CheckNotNull(c);
			if (c == this)
			{
				throw new IllegalArgumentException();
			}
			if (maxElements <= 0)
			{
				return 0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] items = this.items;
			Object[] items = this.Items;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int n = System.Math.Min(maxElements, Count);
				int take = TakeIndex;
				int i = 0;
				try
				{
					while (i < n)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E x = (E) items[take];
						E x = (E) items[take];
						c.Add(x);
						items[take] = null;
						if (++take == items.Length)
						{
							take = 0;
						}
						i++;
					}
					return n;
				}
				finally
				{
					// Restore invariants even if c.add() threw
					if (i > 0)
					{
						Count -= i;
						TakeIndex = take;
						if (Itrs != null)
						{
							if (Count == 0)
							{
								Itrs.QueueIsEmpty();
							}
							else if (i > take)
							{
								Itrs.TakeIndexWrapped();
							}
						}
						for (; i > 0 && @lock.HasWaiters(NotFull); i--)
						{
							NotFull.Signal();
						}
					}
				}
			}
			finally
			{
				@lock.Unlock();
			}
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

		/// <summary>
		/// Shared data between iterators and their queue, allowing queue
		/// modifications to update iterators when elements are removed.
		/// 
		/// This adds a lot of complexity for the sake of correctly
		/// handling some uncommon operations, but the combination of
		/// circular-arrays and supporting interior removes (i.e., those
		/// not at head) would cause iterators to sometimes lose their
		/// places and/or (re)report elements they shouldn't.  To avoid
		/// this, when a queue has one or more iterators, it keeps iterator
		/// state consistent by:
		/// 
		/// (1) keeping track of the number of "cycles", that is, the
		///     number of times takeIndex has wrapped around to 0.
		/// (2) notifying all iterators via the callback removedAt whenever
		///     an interior element is removed (and thus other elements may
		///     be shifted).
		/// 
		/// These suffice to eliminate iterator inconsistencies, but
		/// unfortunately add the secondary responsibility of maintaining
		/// the list of iterators.  We track all active iterators in a
		/// simple linked list (accessed only when the queue's lock is
		/// held) of weak references to Itr.  The list is cleaned up using
		/// 3 different mechanisms:
		/// 
		/// (1) Whenever a new iterator is created, do some O(1) checking for
		///     stale list elements.
		/// 
		/// (2) Whenever takeIndex wraps around to 0, check for iterators
		///     that have been unused for more than one wrap-around cycle.
		/// 
		/// (3) Whenever the queue becomes empty, all iterators are notified
		///     and this entire data structure is discarded.
		/// 
		/// So in addition to the removedAt callback that is necessary for
		/// correctness, iterators have the shutdown and takeIndexWrapped
		/// callbacks that help remove stale iterators from the list.
		/// 
		/// Whenever a list element is examined, it is expunged if either
		/// the GC has determined that the iterator is discarded, or if the
		/// iterator reports that it is "detached" (does not need any
		/// further state updates).  Overhead is maximal when takeIndex
		/// never advances, iterators are discarded before they are
		/// exhausted, and all removals are interior removes, in which case
		/// all stale iterators are discovered by the GC.  But even in this
		/// case we don't increase the amortized complexity.
		/// 
		/// Care must be taken to keep list sweeping methods from
		/// reentrantly invoking another such method, causing subtle
		/// corruption bugs.
		/// </summary>
		internal class Itrs
		{
			private readonly ArrayBlockingQueue<E> OuterInstance;


			/// <summary>
			/// Node in a linked list of weak iterator references.
			/// </summary>
			private class Node : WeakReference<Itr>
			{
				private readonly ArrayBlockingQueue.Itrs OuterInstance;

				internal Node Next;

				internal Node(ArrayBlockingQueue.Itrs outerInstance, Itr iterator, Node next) : base(iterator)
				{
					this.OuterInstance = outerInstance;
					this.Next = next;
				}
			}

			/// <summary>
			/// Incremented whenever takeIndex wraps around to 0 </summary>
			internal int Cycles = 0;

			/// <summary>
			/// Linked list of weak iterator references </summary>
			internal Node Head;

			/// <summary>
			/// Used to expunge stale iterators </summary>
			internal Node Sweeper = null;

			internal const int SHORT_SWEEP_PROBES = 4;
			internal const int LONG_SWEEP_PROBES = 16;

			internal Itrs(ArrayBlockingQueue<E> outerInstance, Itr initial)
			{
				this.OuterInstance = outerInstance;
				Register(initial);
			}

			/// <summary>
			/// Sweeps itrs, looking for and expunging stale iterators.
			/// If at least one was found, tries harder to find more.
			/// Called only from iterating thread.
			/// </summary>
			/// <param name="tryHarder"> whether to start in try-harder mode, because
			/// there is known to be at least one iterator to collect </param>
			internal virtual void DoSomeSweeping(bool tryHarder)
			{
				// assert lock.getHoldCount() == 1;
				// assert head != null;
				int probes = tryHarder ? LONG_SWEEP_PROBES : SHORT_SWEEP_PROBES;
				Node o, p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node sweeper = this.sweeper;
				Node sweeper = this.Sweeper;
				bool passedGo; // to limit search to one full sweep

				if (sweeper == null)
				{
					o = null;
					p = Head;
					passedGo = true;
				}
				else
				{
					o = sweeper;
					p = o.Next;
					passedGo = false;
				}

				for (; probes > 0; probes--)
				{
					if (p == null)
					{
						if (passedGo)
						{
							break;
						}
						o = null;
						p = Head;
						passedGo = true;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Itr it = p.get();
					Itr it = p.get();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node next = p.next;
					Node next = p.Next;
					if (it == null || it.Detached)
					{
						// found a discarded/exhausted iterator
						probes = LONG_SWEEP_PROBES; // "try harder"
						// unlink p
						p.clear();
						p.Next = null;
						if (o == null)
						{
							Head = next;
							if (next == null)
							{
								// We've run out of iterators to track; retire
								outerInstance.Itrs = null;
								return;
							}
						}
						else
						{
							o.Next = next;
						}
					}
					else
					{
						o = p;
					}
					p = next;
				}

				this.Sweeper = (p == null) ? null : o;
			}

			/// <summary>
			/// Adds a new iterator to the linked list of tracked iterators.
			/// </summary>
			internal virtual void Register(Itr itr)
			{
				// assert lock.getHoldCount() == 1;
				Head = new Node(this, itr, Head);
			}

			/// <summary>
			/// Called whenever takeIndex wraps around to 0.
			/// 
			/// Notifies all iterators, and expunges any that are now stale.
			/// </summary>
			internal virtual void TakeIndexWrapped()
			{
				// assert lock.getHoldCount() == 1;
				Cycles++;
				for (Node o = null, p = Head; p != null;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Itr it = p.get();
					Itr it = p.get();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node next = p.next;
					Node next = p.next;
					if (it == null || it.TakeIndexWrapped())
					{
						// unlink p
						// assert it == null || it.isDetached();
						p.clear();
						p.next = null;
						if (o == null)
						{
							Head = next;
						}
						else
						{
							o.Next = next;
						}
					}
					else
					{
						o = p;
					}
					p = next;
				}
				if (Head == null) // no more iterators to track
				{
					outerInstance.Itrs = null;
				}
			}

			/// <summary>
			/// Called whenever an interior remove (not at takeIndex) occurred.
			/// 
			/// Notifies all iterators, and expunges any that are now stale.
			/// </summary>
			internal virtual void RemovedAt(int removedIndex)
			{
				for (Node o = null, p = Head; p != null;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Itr it = p.get();
					Itr it = p.get();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node next = p.next;
					Node next = p.next;
					if (it == null || it.RemovedAt(removedIndex))
					{
						// unlink p
						// assert it == null || it.isDetached();
						p.clear();
						p.next = null;
						if (o == null)
						{
							Head = next;
						}
						else
						{
							o.Next = next;
						}
					}
					else
					{
						o = p;
					}
					p = next;
				}
				if (Head == null) // no more iterators to track
				{
					outerInstance.Itrs = null;
				}
			}

			/// <summary>
			/// Called whenever the queue becomes empty.
			/// 
			/// Notifies all active iterators that the queue is empty,
			/// clears all weak refs, and unlinks the itrs datastructure.
			/// </summary>
			internal virtual void QueueIsEmpty()
			{
				// assert lock.getHoldCount() == 1;
				for (Node p = Head; p != null; p = p.Next)
				{
					Itr it = p.get();
					if (it != null)
					{
						p.clear();
						it.Shutdown();
					}
				}
				Head = null;
				outerInstance.Itrs = null;
			}

			/// <summary>
			/// Called whenever an element has been dequeued (at takeIndex).
			/// </summary>
			internal virtual void ElementDequeued()
			{
				// assert lock.getHoldCount() == 1;
				if (outerInstance.Count == 0)
				{
					QueueIsEmpty();
				}
				else if (outerInstance.TakeIndex == 0)
				{
					TakeIndexWrapped();
				}
			}
		}

		/// <summary>
		/// Iterator for ArrayBlockingQueue.
		/// 
		/// To maintain weak consistency with respect to puts and takes, we
		/// read ahead one slot, so as to not report hasNext true but then
		/// not have an element to return.
		/// 
		/// We switch into "detached" mode (allowing prompt unlinking from
		/// itrs without help from the GC) when all indices are negative, or
		/// when hasNext returns false for the first time.  This allows the
		/// iterator to track concurrent updates completely accurately,
		/// except for the corner case of the user calling Iterator.remove()
		/// after hasNext() returned false.  Even in this case, we ensure
		/// that we don't remove the wrong element by keeping track of the
		/// expected element to remove, in lastItem.  Yes, we may fail to
		/// remove lastItem from the queue if it moved due to an interleaved
		/// interior remove while in detached mode.
		/// </summary>
		private class Itr : Iterator<E>
		{
			private readonly ArrayBlockingQueue<E> OuterInstance;

			/// <summary>
			/// Index to look for new nextItem; NONE at end </summary>
			internal int Cursor;

			/// <summary>
			/// Element to be returned by next call to next(); null if none </summary>
			internal E NextItem;

			/// <summary>
			/// Index of nextItem; NONE if none, REMOVED if removed elsewhere </summary>
			internal int NextIndex;

			/// <summary>
			/// Last element returned; null if none or not detached. </summary>
			internal E LastItem;

			/// <summary>
			/// Index of lastItem, NONE if none, REMOVED if removed elsewhere </summary>
			internal int LastRet;

			/// <summary>
			/// Previous value of takeIndex, or DETACHED when detached </summary>
			internal int PrevTakeIndex;

			/// <summary>
			/// Previous value of iters.cycles </summary>
			internal int PrevCycles;

			/// <summary>
			/// Special index value indicating "not available" or "undefined" </summary>
			internal const int NONE = -1;

			/// <summary>
			/// Special index value indicating "removed elsewhere", that is,
			/// removed by some operation other than a call to this.remove().
			/// </summary>
			internal const int REMOVED = -2;

			/// <summary>
			/// Special value for prevTakeIndex indicating "detached mode" </summary>
			internal const int DETACHED = -3;

			internal Itr(ArrayBlockingQueue<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				// assert lock.getHoldCount() == 0;
				LastRet = NONE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = ArrayBlockingQueue.this.lock;
				ReentrantLock @lock = outerInstance.@lock;
				@lock.@lock();
				try
				{
					if (outerInstance.Count == 0)
					{
						// assert itrs == null;
						Cursor = NONE;
						NextIndex = NONE;
						PrevTakeIndex = DETACHED;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int takeIndex = ArrayBlockingQueue.this.takeIndex;
						int takeIndex = outerInstance.TakeIndex;
						PrevTakeIndex = takeIndex;
						NextItem = outerInstance.ItemAt(NextIndex = takeIndex);
						Cursor = IncCursor(takeIndex);
						if (outerInstance.Itrs == null)
						{
							outerInstance.Itrs = new Itrs(outerInstance, this);
						}
						else
						{
							outerInstance.Itrs.Register(this); // in this order
							outerInstance.Itrs.DoSomeSweeping(false);
						}
						PrevCycles = outerInstance.Itrs.Cycles;
						// assert takeIndex >= 0;
						// assert prevTakeIndex == takeIndex;
						// assert nextIndex >= 0;
						// assert nextItem != null;
					}
				}
				finally
				{
					@lock.Unlock();
				}
			}

			internal virtual bool Detached
			{
				get
				{
					// assert lock.getHoldCount() == 1;
					return PrevTakeIndex < 0;
				}
			}

			internal virtual int IncCursor(int index)
			{
				// assert lock.getHoldCount() == 1;
				if (++index == outerInstance.Items.Length)
				{
					index = 0;
				}
				if (index == outerInstance.PutIndex)
				{
					index = NONE;
				}
				return index;
			}

			/// <summary>
			/// Returns true if index is invalidated by the given number of
			/// dequeues, starting from prevTakeIndex.
			/// </summary>
			internal virtual bool Invalidated(int index, int prevTakeIndex, long dequeues, int length)
			{
				if (index < 0)
				{
					return false;
				}
				int distance = index - prevTakeIndex;
				if (distance < 0)
				{
					distance += length;
				}
				return dequeues > distance;
			}

			/// <summary>
			/// Adjusts indices to incorporate all dequeues since the last
			/// operation on this iterator.  Call only from iterating thread.
			/// </summary>
			internal virtual void IncorporateDequeues()
			{
				// assert lock.getHoldCount() == 1;
				// assert itrs != null;
				// assert !isDetached();
				// assert count > 0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cycles = itrs.cycles;
				int cycles = outerInstance.Itrs.Cycles;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int takeIndex = ArrayBlockingQueue.this.takeIndex;
				int takeIndex = OuterInstance.TakeIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prevCycles = this.prevCycles;
				int prevCycles = this.PrevCycles;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prevTakeIndex = this.prevTakeIndex;
				int prevTakeIndex = this.PrevTakeIndex;

				if (cycles != prevCycles || takeIndex != prevTakeIndex)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = items.length;
					int len = outerInstance.Items.Length;
					// how far takeIndex has advanced since the previous
					// operation of this iterator
					long dequeues = (cycles - prevCycles) * len + (takeIndex - prevTakeIndex);

					// Check indices for invalidation
					if (Invalidated(LastRet, prevTakeIndex, dequeues, len))
					{
						LastRet = REMOVED;
					}
					if (Invalidated(NextIndex, prevTakeIndex, dequeues, len))
					{
						NextIndex = REMOVED;
					}
					if (Invalidated(Cursor, prevTakeIndex, dequeues, len))
					{
						Cursor = takeIndex;
					}

					if (Cursor < 0 && NextIndex < 0 && LastRet < 0)
					{
						Detach();
					}
					else
					{
						this.PrevCycles = cycles;
						this.PrevTakeIndex = takeIndex;
					}
				}
			}

			/// <summary>
			/// Called when itrs should stop tracking this iterator, either
			/// because there are no more indices to update (cursor < 0 &&
			/// nextIndex < 0 && lastRet < 0) or as a special exception, when
			/// lastRet >= 0, because hasNext() is about to return false for the
			/// first time.  Call only from iterating thread.
			/// </summary>
			internal virtual void Detach()
			{
				// Switch to detached mode
				// assert lock.getHoldCount() == 1;
				// assert cursor == NONE;
				// assert nextIndex < 0;
				// assert lastRet < 0 || nextItem == null;
				// assert lastRet < 0 ^ lastItem != null;
				if (PrevTakeIndex >= 0)
				{
					// assert itrs != null;
					PrevTakeIndex = DETACHED;
					// try to unlink from itrs (but not too hard)
					outerInstance.Itrs.DoSomeSweeping(true);
				}
			}

			/// <summary>
			/// For performance reasons, we would like not to acquire a lock in
			/// hasNext in the common case.  To allow for this, we only access
			/// fields (i.e. nextItem) that are not modified by update operations
			/// triggered by queue modifications.
			/// </summary>
			public virtual bool HasNext()
			{
				// assert lock.getHoldCount() == 0;
				if (NextItem != null)
				{
					return true;
				}
				NoNext();
				return false;
			}

			internal virtual void NoNext()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = ArrayBlockingQueue.this.lock;
				ReentrantLock @lock = OuterInstance.@lock;
				@lock.@lock();
				try
				{
					// assert cursor == NONE;
					// assert nextIndex == NONE;
					if (!Detached)
					{
						// assert lastRet >= 0;
						IncorporateDequeues(); // might update lastRet
						if (LastRet >= 0)
						{
							LastItem = outerInstance.ItemAt(LastRet);
							// assert lastItem != null;
							Detach();
						}
					}
					// assert isDetached();
					// assert lastRet < 0 ^ lastItem != null;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual E Next()
			{
				// assert lock.getHoldCount() == 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E x = nextItem;
				E x = NextItem;
				if (x == null)
				{
					throw new NoSuchElementException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = ArrayBlockingQueue.this.lock;
				ReentrantLock @lock = OuterInstance.@lock;
				@lock.@lock();
				try
				{
					if (!Detached)
					{
						IncorporateDequeues();
					}
					// assert nextIndex != NONE;
					// assert lastItem == null;
					LastRet = NextIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cursor = this.cursor;
					int cursor = this.Cursor;
					if (cursor >= 0)
					{
						NextItem = outerInstance.ItemAt(NextIndex = cursor);
						// assert nextItem != null;
						this.Cursor = IncCursor(cursor);
					}
					else
					{
						NextIndex = NONE;
						NextItem = null;
					}
				}
				finally
				{
					@lock.Unlock();
				}
				return x;
			}

			public virtual void Remove()
			{
				// assert lock.getHoldCount() == 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = ArrayBlockingQueue.this.lock;
				ReentrantLock @lock = OuterInstance.@lock;
				@lock.@lock();
				try
				{
					if (!Detached)
					{
						IncorporateDequeues(); // might update lastRet or detach
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastRet = this.lastRet;
					int lastRet = this.LastRet;
					this.LastRet = NONE;
					if (lastRet >= 0)
					{
						if (!Detached)
						{
							outerInstance.RemoveAt(lastRet);
						}
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E lastItem = this.lastItem;
							E lastItem = this.LastItem;
							// assert lastItem != null;
							this.LastItem = null;
							if (outerInstance.ItemAt(lastRet) == lastItem)
							{
								outerInstance.RemoveAt(lastRet);
							}
						}
					}
					else if (lastRet == NONE)
					{
						throw new IllegalStateException();
					}
					// else lastRet == REMOVED and the last returned element was
					// previously asynchronously removed via an operation other
					// than this.remove(), so nothing to do.

					if (Cursor < 0 && NextIndex < 0)
					{
						Detach();
					}
				}
				finally
				{
					@lock.Unlock();
					// assert lastRet == NONE;
					// assert lastItem == null;
				}
			}

			/// <summary>
			/// Called to notify the iterator that the queue is empty, or that it
			/// has fallen hopelessly behind, so that it should abandon any
			/// further iteration, except possibly to return one more element
			/// from next(), as promised by returning true from hasNext().
			/// </summary>
			internal virtual void Shutdown()
			{
				// assert lock.getHoldCount() == 1;
				Cursor = NONE;
				if (NextIndex >= 0)
				{
					NextIndex = REMOVED;
				}
				if (LastRet >= 0)
				{
					LastRet = REMOVED;
					LastItem = null;
				}
				PrevTakeIndex = DETACHED;
				// Don't set nextItem to null because we must continue to be
				// able to return it on next().
				//
				// Caller will unlink from itrs when convenient.
			}

			internal virtual int Distance(int index, int prevTakeIndex, int length)
			{
				int distance = index - prevTakeIndex;
				if (distance < 0)
				{
					distance += length;
				}
				return distance;
			}

			/// <summary>
			/// Called whenever an interior remove (not at takeIndex) occurred.
			/// </summary>
			/// <returns> true if this iterator should be unlinked from itrs </returns>
			internal virtual bool RemovedAt(int removedIndex)
			{
				// assert lock.getHoldCount() == 1;
				if (Detached)
				{
					return true;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cycles = itrs.cycles;
				int cycles = outerInstance.Itrs.Cycles;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int takeIndex = ArrayBlockingQueue.this.takeIndex;
				int takeIndex = OuterInstance.TakeIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prevCycles = this.prevCycles;
				int prevCycles = this.PrevCycles;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prevTakeIndex = this.prevTakeIndex;
				int prevTakeIndex = this.PrevTakeIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = items.length;
				int len = outerInstance.Items.Length;
				int cycleDiff = cycles - prevCycles;
				if (removedIndex < takeIndex)
				{
					cycleDiff++;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int removedDistance = (cycleDiff * len) + (removedIndex - prevTakeIndex);
				int removedDistance = (cycleDiff * len) + (removedIndex - prevTakeIndex);
				// assert removedDistance >= 0;
				int cursor = this.Cursor;
				if (cursor >= 0)
				{
					int x = Distance(cursor, prevTakeIndex, len);
					if (x == removedDistance)
					{
						if (cursor == outerInstance.PutIndex)
						{
							this.Cursor = cursor = NONE;
						}
					}
					else if (x > removedDistance)
					{
						// assert cursor != prevTakeIndex;
						this.Cursor = cursor = outerInstance.Dec(cursor);
					}
				}
				int lastRet = this.LastRet;
				if (lastRet >= 0)
				{
					int x = Distance(lastRet, prevTakeIndex, len);
					if (x == removedDistance)
					{
						this.LastRet = lastRet = REMOVED;
					}
					else if (x > removedDistance)
					{
						this.LastRet = lastRet = outerInstance.Dec(lastRet);
					}
				}
				int nextIndex = this.NextIndex;
				if (nextIndex >= 0)
				{
					int x = Distance(nextIndex, prevTakeIndex, len);
					if (x == removedDistance)
					{
						this.NextIndex = nextIndex = REMOVED;
					}
					else if (x > removedDistance)
					{
						this.NextIndex = nextIndex = outerInstance.Dec(nextIndex);
					}
				}
				else if (cursor < 0 && nextIndex < 0 && lastRet < 0)
				{
					this.PrevTakeIndex = DETACHED;
					return true;
				}
				return false;
			}

			/// <summary>
			/// Called whenever takeIndex wraps around to zero.
			/// </summary>
			/// <returns> true if this iterator should be unlinked from itrs </returns>
			internal virtual bool TakeIndexWrapped()
			{
				// assert lock.getHoldCount() == 1;
				if (Detached)
				{
					return true;
				}
				if (outerInstance.Itrs.Cycles - PrevCycles > 1)
				{
					// All the elements that existed at the time of the last
					// operation are gone, so abandon further iteration.
					Shutdown();
					return true;
				}
				return false;
			}

	//         /** Uncomment for debugging. */
	//         public String toString() {
	//             return ("cursor=" + cursor + " " +
	//                     "nextIndex=" + nextIndex + " " +
	//                     "lastRet=" + lastRet + " " +
	//                     "nextItem=" + nextItem + " " +
	//                     "lastItem=" + lastItem + " " +
	//                     "prevCycles=" + prevCycles + " " +
	//                     "prevTakeIndex=" + prevTakeIndex + " " +
	//                     "size()=" + size() + " " +
	//                     "remainingCapacity()=" + remainingCapacity());
	//         }
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
			return Spliterators.Spliterator(this, java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.CONCURRENT);
		}

	}

}