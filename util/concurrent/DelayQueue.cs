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
	/// An unbounded <seealso cref="BlockingQueue blocking queue"/> of
	/// {@code Delayed} elements, in which an element can only be taken
	/// when its delay has expired.  The <em>head</em> of the queue is that
	/// {@code Delayed} element whose delay expired furthest in the
	/// past.  If no delay has expired there is no head and {@code poll}
	/// will return {@code null}. Expiration occurs when an element's
	/// {@code getDelay(TimeUnit.NANOSECONDS)} method returns a value less
	/// than or equal to zero.  Even though unexpired elements cannot be
	/// removed using {@code take} or {@code poll}, they are otherwise
	/// treated as normal elements. For example, the {@code size} method
	/// returns the count of both expired and unexpired elements.
	/// This queue does not permit null elements.
	/// 
	/// <para>This class and its iterator implement all of the
	/// <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	/// Iterator} interfaces.  The Iterator provided in method {@link
	/// #iterator()} is <em>not</em> guaranteed to traverse the elements of
	/// the DelayQueue in any particular order.
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
	public class DelayQueue<E> : AbstractQueue<E>, BlockingQueue<E> where E : Delayed
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Available = @lock.NewCondition();
		}


		[NonSerialized]
		private readonly ReentrantLock @lock = new ReentrantLock();
		private readonly PriorityQueue<E> q = new PriorityQueue<E>();

		/// <summary>
		/// Thread designated to wait for the element at the head of
		/// the queue.  This variant of the Leader-Follower pattern
		/// (http://www.cs.wustl.edu/~schmidt/POSA/POSA2/) serves to
		/// minimize unnecessary timed waiting.  When a thread becomes
		/// the leader, it waits only for the next delay to elapse, but
		/// other threads await indefinitely.  The leader thread must
		/// signal some other thread before returning from take() or
		/// poll(...), unless some other thread becomes leader in the
		/// interim.  Whenever the head of the queue is replaced with
		/// an element with an earlier expiration time, the leader
		/// field is invalidated by being reset to null, and some
		/// waiting thread, but not necessarily the current leader, is
		/// signalled.  So waiting threads must be prepared to acquire
		/// and lose leadership while waiting.
		/// </summary>
		private Thread Leader = null;

		/// <summary>
		/// Condition signalled when a newer element becomes available
		/// at the head of the queue or a new thread may need to
		/// become leader.
		/// </summary>
		private Condition Available;

		/// <summary>
		/// Creates a new {@code DelayQueue} that is initially empty.
		/// </summary>
		public DelayQueue()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a {@code DelayQueue} initially containing the elements of the
		/// given collection of <seealso cref="Delayed"/> instances.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public DelayQueue<T1>(Collection<T1> c) where T1 : E
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.addAll(c);
		}

		/// <summary>
		/// Inserts the specified element into this delay queue.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return Offer(e);
		}

		/// <summary>
		/// Inserts the specified element into this delay queue.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				q.Offer(e);
				if (q.Peek() == e)
				{
					Leader = null;
					Available.Signal();
				}
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts the specified element into this delay queue. As the queue is
		/// unbounded this method will never block.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual void Put(E e)
		{
			Offer(e);
		}

		/// <summary>
		/// Inserts the specified element into this delay queue. As the queue is
		/// unbounded this method will never block.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <param name="timeout"> This parameter is ignored as the method never blocks </param>
		/// <param name="unit"> This parameter is ignored as the method never blocks </param>
		/// <returns> {@code true} </returns>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{
			return Offer(e);
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, or returns {@code null}
		/// if this queue has no elements with an expired delay.
		/// </summary>
		/// <returns> the head of this queue, or {@code null} if this
		///         queue has no elements with an expired delay </returns>
		public virtual E Poll()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				E first = q.Peek();
				if (first == null || first.GetDelay(NANOSECONDS) > 0)
				{
					return null;
				}
				else
				{
					return q.Poll();
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, waiting if necessary
		/// until an element with an expired delay is available on this queue.
		/// </summary>
		/// <returns> the head of this queue </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
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
				for (;;)
				{
					E first = q.Peek();
					if (first == null)
					{
						Available.@await();
					}
					else
					{
						long delay = first.GetDelay(NANOSECONDS);
						if (delay <= 0)
						{
							return q.Poll();
						}
						first = null; // don't retain ref while waiting
						if (Leader != null)
						{
							Available.@await();
						}
						else
						{
							Thread thisThread = Thread.CurrentThread;
							Leader = thisThread;
							try
							{
								Available.AwaitNanos(delay);
							}
							finally
							{
								if (Leader == thisThread)
								{
									Leader = null;
								}
							}
						}
					}
				}
			}
			finally
			{
				if (Leader == null && q.Peek() != null)
				{
					Available.Signal();
				}
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Retrieves and removes the head of this queue, waiting if necessary
		/// until an element with an expired delay is available on this queue,
		/// or the specified wait time expires.
		/// </summary>
		/// <returns> the head of this queue, or {@code null} if the
		///         specified waiting time elapses before an element with
		///         an expired delay becomes available </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
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
				for (;;)
				{
					E first = q.Peek();
					if (first == null)
					{
						if (nanos <= 0)
						{
							return null;
						}
						else
						{
							nanos = Available.AwaitNanos(nanos);
						}
					}
					else
					{
						long delay = first.GetDelay(NANOSECONDS);
						if (delay <= 0)
						{
							return q.Poll();
						}
						if (nanos <= 0)
						{
							return null;
						}
						first = null; // don't retain ref while waiting
						if (nanos < delay || Leader != null)
						{
							nanos = Available.AwaitNanos(nanos);
						}
						else
						{
							Thread thisThread = Thread.CurrentThread;
							Leader = thisThread;
							try
							{
								long timeLeft = Available.AwaitNanos(delay);
								nanos -= delay - timeLeft;
							}
							finally
							{
								if (Leader == thisThread)
								{
									Leader = null;
								}
							}
						}
					}
				}
			}
			finally
			{
				if (Leader == null && q.Peek() != null)
				{
					Available.Signal();
				}
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Retrieves, but does not remove, the head of this queue, or
		/// returns {@code null} if this queue is empty.  Unlike
		/// {@code poll}, if no expired elements are available in the queue,
		/// this method returns the element that will expire next,
		/// if one exists.
		/// </summary>
		/// <returns> the head of this queue, or {@code null} if this
		///         queue is empty </returns>
		public virtual E Peek()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return q.Peek();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual int Size()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return q.Size();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns first element only if it is expired.
		/// Used only by drainTo.  Call only when holding lock.
		/// </summary>
		private E PeekExpired()
		{
			// assert lock.isHeldByCurrentThread();
			E first = q.Peek();
			return (first == null || first.GetDelay(NANOSECONDS) > 0) ? null : first;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				int n = 0;
				for (E e; (e = PeekExpired()) != null;)
				{
					c.Add(e); // In this order, in case add() throws.
					q.Poll();
					++n;
				}
				return n;
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
				int n = 0;
				for (E e; n < maxElements && (e = PeekExpired()) != null;)
				{
					c.Add(e); // In this order, in case add() throws.
					q.Poll();
					++n;
				}
				return n;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Atomically removes all of the elements from this delay queue.
		/// The queue will be empty after this call returns.
		/// Elements with an unexpired delay are not waited for; they are
		/// simply discarded from the queue.
		/// </summary>
		public virtual void Clear()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				q.Clear();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Always returns {@code Integer.MAX_VALUE} because
		/// a {@code DelayQueue} is not capacity constrained.
		/// </summary>
		/// <returns> {@code Integer.MAX_VALUE} </returns>
		public virtual int RemainingCapacity()
		{
			return Integer.MaxValue;
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
				return q.ToArray();
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
		/// <para>The following code can be used to dump a delay queue into a newly
		/// allocated array of {@code Delayed}:
		/// 
		/// <pre> {@code Delayed[] a = q.toArray(new Delayed[0]);}</pre>
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
				return q.ToArray(a);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Removes a single instance of the specified element from this
		/// queue, if it is present, whether or not it has expired.
		/// </summary>
		public virtual bool Remove(Object o)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return q.Remove(o);
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
				for (Iterator<E> it = q.Iterator(); it.HasNext();)
				{
					if (o == it.Next())
					{
						it.remove();
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
		/// Returns an iterator over all the elements (both expired and
		/// unexpired) in this queue. The iterator does not return the
		/// elements in any particular order.
		/// 
		/// <para>The returned iterator is
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this queue </returns>
		public virtual Iterator<E> Iterator()
		{
			return new Itr(this, ToArray());
		}

		/// <summary>
		/// Snapshot iterator that works off copy of underlying q array.
		/// </summary>
		private class Itr : Iterator<E>
		{
			private readonly DelayQueue<E> OuterInstance;

			internal readonly Object[] Array; // Array of all elements
			internal int Cursor; // index of next element to return
			internal int LastRet; // index of last element, or -1 if no such

			internal Itr(DelayQueue<E> outerInstance, Object[] array)
			{
				this.OuterInstance = outerInstance;
				LastRet = -1;
				this.Array = array;
			}

			public virtual bool HasNext()
			{
				return Cursor < Array.Length;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
			public virtual E Next()
			{
				if (Cursor >= Array.Length)
				{
					throw new NoSuchElementException();
				}
				LastRet = Cursor;
				return (E)Array[Cursor++];
			}

			public virtual void Remove()
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				outerInstance.RemoveEQ(Array[LastRet]);
				LastRet = -1;
			}
		}

	}

}