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
	/// An optionally-bounded <seealso cref="BlockingQueue blocking queue"/> based on
	/// linked nodes.
	/// This queue orders elements FIFO (first-in-first-out).
	/// The <em>head</em> of the queue is that element that has been on the
	/// queue the longest time.
	/// The <em>tail</em> of the queue is that element that has been on the
	/// queue the shortest time. New elements
	/// are inserted at the tail of the queue, and the queue retrieval
	/// operations obtain elements at the head of the queue.
	/// Linked queues typically have higher throughput than array-based queues but
	/// less predictable performance in most concurrent applications.
	/// 
	/// <para>The optional capacity bound constructor argument serves as a
	/// way to prevent excessive queue expansion. The capacity, if unspecified,
	/// is equal to <seealso cref="Integer#MAX_VALUE"/>.  Linked nodes are
	/// dynamically created upon each insertion unless this would bring the
	/// queue above capacity.
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
	public class LinkedBlockingQueue<E> : AbstractQueue<E>, BlockingQueue<E>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			NotEmpty = TakeLock.NewCondition();
			NotFull = PutLock.NewCondition();
		}

		private const long SerialVersionUID = -6903933977591709194L;

		/*
		 * A variant of the "two lock queue" algorithm.  The putLock gates
		 * entry to put (and offer), and has an associated condition for
		 * waiting puts.  Similarly for the takeLock.  The "count" field
		 * that they both rely on is maintained as an atomic to avoid
		 * needing to get both locks in most cases. Also, to minimize need
		 * for puts to get takeLock and vice-versa, cascading notifies are
		 * used. When a put notices that it has enabled at least one take,
		 * it signals taker. That taker in turn signals others if more
		 * items have been entered since the signal. And symmetrically for
		 * takes signalling puts. Operations such as remove(Object) and
		 * iterators acquire both locks.
		 *
		 * Visibility between writers and readers is provided as follows:
		 *
		 * Whenever an element is enqueued, the putLock is acquired and
		 * count updated.  A subsequent reader guarantees visibility to the
		 * enqueued Node by either acquiring the putLock (via fullyLock)
		 * or by acquiring the takeLock, and then reading n = count.get();
		 * this gives visibility to the first n items.
		 *
		 * To implement weakly consistent iterators, it appears we need to
		 * keep all Nodes GC-reachable from a predecessor dequeued Node.
		 * That would cause two problems:
		 * - allow a rogue Iterator to cause unbounded memory retention
		 * - cause cross-generational linking of old Nodes to new Nodes if
		 *   a Node was tenured while live, which generational GCs have a
		 *   hard time dealing with, causing repeated major collections.
		 * However, only non-deleted Nodes need to be reachable from
		 * dequeued Nodes, and reachability does not necessarily have to
		 * be of the kind understood by the GC.  We use the trick of
		 * linking a Node that has just been dequeued to itself.  Such a
		 * self-link implicitly means to advance to head.next.
		 */

		/// <summary>
		/// Linked list node class
		/// </summary>
		internal class Node<E>
		{
			internal E Item;

			/// <summary>
			/// One of:
			/// - the real successor Node
			/// - this Node, meaning the successor is head.next
			/// - null, meaning there is no successor (this is the last node)
			/// </summary>
			internal Node<E> Next;

			internal Node(E x)
			{
				Item = x;
			}
		}

		/// <summary>
		/// The capacity bound, or Integer.MAX_VALUE if none </summary>
		private readonly int Capacity;

		/// <summary>
		/// Current number of elements </summary>
		private readonly AtomicInteger Count = new AtomicInteger();

		/// <summary>
		/// Head of linked list.
		/// Invariant: head.item == null
		/// </summary>
		[NonSerialized]
		internal Node<E> Head;

		/// <summary>
		/// Tail of linked list.
		/// Invariant: last.next == null
		/// </summary>
		[NonSerialized]
		private Node<E> Last;

		/// <summary>
		/// Lock held by take, poll, etc </summary>
		private readonly ReentrantLock TakeLock = new ReentrantLock();

		/// <summary>
		/// Wait queue for waiting takes </summary>
		private Condition NotEmpty;

		/// <summary>
		/// Lock held by put, offer, etc </summary>
		private readonly ReentrantLock PutLock = new ReentrantLock();

		/// <summary>
		/// Wait queue for waiting puts </summary>
		private Condition NotFull;

		/// <summary>
		/// Signals a waiting take. Called only from put/offer (which do not
		/// otherwise ordinarily lock takeLock.)
		/// </summary>
		private void SignalNotEmpty()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.@lock();
			try
			{
				NotEmpty.Signal();
			}
			finally
			{
				takeLock.Unlock();
			}
		}

		/// <summary>
		/// Signals a waiting put. Called only from take/poll.
		/// </summary>
		private void SignalNotFull()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock putLock = this.putLock;
			ReentrantLock putLock = this.PutLock;
			putLock.@lock();
			try
			{
				NotFull.Signal();
			}
			finally
			{
				putLock.Unlock();
			}
		}

		/// <summary>
		/// Links node at end of queue.
		/// </summary>
		/// <param name="node"> the node </param>
		private void Enqueue(Node<E> node)
		{
			// assert putLock.isHeldByCurrentThread();
			// assert last.next == null;
			Last = Last.Next = node;
		}

		/// <summary>
		/// Removes a node from head of queue.
		/// </summary>
		/// <returns> the node </returns>
		private E Dequeue()
		{
			// assert takeLock.isHeldByCurrentThread();
			// assert head.item == null;
			Node<E> h = Head;
			Node<E> first = h.Next;
			h.Next = h; // help GC
			Head = first;
			E x = first.Item;
			first.Item = null;
			return x;
		}

		/// <summary>
		/// Locks to prevent both puts and takes.
		/// </summary>
		internal virtual void FullyLock()
		{
			PutLock.@lock();
			TakeLock.@lock();
		}

		/// <summary>
		/// Unlocks to allow both puts and takes.
		/// </summary>
		internal virtual void FullyUnlock()
		{
			TakeLock.Unlock();
			PutLock.Unlock();
		}

	//     /**
	//      * Tells whether both locks are held by current thread.
	//      */
	//     boolean isFullyLocked() {
	//         return (putLock.isHeldByCurrentThread() &&
	//                 takeLock.isHeldByCurrentThread());
	//     }

		/// <summary>
		/// Creates a {@code LinkedBlockingQueue} with a capacity of
		/// <seealso cref="Integer#MAX_VALUE"/>.
		/// </summary>
		public LinkedBlockingQueue() : this(Integer.MaxValue)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a {@code LinkedBlockingQueue} with the given (fixed) capacity.
		/// </summary>
		/// <param name="capacity"> the capacity of this queue </param>
		/// <exception cref="IllegalArgumentException"> if {@code capacity} is not greater
		///         than zero </exception>
		public LinkedBlockingQueue(int capacity)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (capacity <= 0)
			{
				throw new IllegalArgumentException();
			}
			this.Capacity = capacity;
			Last = Head = new Node<E>(null);
		}

		/// <summary>
		/// Creates a {@code LinkedBlockingQueue} with a capacity of
		/// <seealso cref="Integer#MAX_VALUE"/>, initially containing the elements of the
		/// given collection,
		/// added in traversal order of the collection's iterator.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public LinkedBlockingQueue<T1>(ICollection<T1> c) where T1 : E : this(Integer.MaxValue)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock putLock = this.putLock;
			ReentrantLock putLock = this.PutLock;
			putLock.@lock(); // Never contended, but necessary for visibility
			try
			{
				int n = 0;
				foreach (E e in c)
				{
					if (e == null)
					{
						throw new NullPointerException();
					}
					if (n == Capacity)
					{
						throw new IllegalStateException("Queue full");
					}
					Enqueue(new Node<E>(e));
					++n;
				}
				Count.Set(n);
			}
			finally
			{
				putLock.Unlock();
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
			return Count.Get();
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
			return Capacity - Count.Get();
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue, waiting if
		/// necessary for space to become available.
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
			// Note: convention in all put/take/etc is to preset local var
			// holding count negative to indicate failure unless set.
			int c = -1;
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock putLock = this.putLock;
			ReentrantLock putLock = this.PutLock;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
			putLock.LockInterruptibly();
			try
			{
				/*
				 * Note that count is used in wait guard even though it is
				 * not protected by lock. This works because count can
				 * only decrease at this point (all other puts are shut
				 * out by lock), and we (or some other waiting put) are
				 * signalled if it ever changes from capacity. Similarly
				 * for all other uses of count in other wait guards.
				 */
				while (count.Get() == Capacity)
				{
					NotFull.@await();
				}
				Enqueue(node);
				c = count.AndIncrement;
				if (c + 1 < Capacity)
				{
					NotFull.Signal();
				}
			}
			finally
			{
				putLock.Unlock();
			}
			if (c == 0)
			{
				SignalNotEmpty();
			}
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue, waiting if
		/// necessary up to the specified wait time for space to become available.
		/// </summary>
		/// <returns> {@code true} if successful, or {@code false} if
		///         the specified waiting time elapses before space is available </returns>
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
			long nanos = unit.ToNanos(timeout);
			int c = -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock putLock = this.putLock;
			ReentrantLock putLock = this.PutLock;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
			putLock.LockInterruptibly();
			try
			{
				while (count.Get() == Capacity)
				{
					if (nanos <= 0)
					{
						return false;
					}
					nanos = NotFull.AwaitNanos(nanos);
				}
				Enqueue(new Node<E>(e));
				c = count.AndIncrement;
				if (c + 1 < Capacity)
				{
					NotFull.Signal();
				}
			}
			finally
			{
				putLock.Unlock();
			}
			if (c == 0)
			{
				SignalNotEmpty();
			}
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the tail of this queue if it is
		/// possible to do so immediately without exceeding the queue's capacity,
		/// returning {@code true} upon success and {@code false} if this queue
		/// is full.
		/// When using a capacity-restricted queue, this method is generally
		/// preferable to method <seealso cref="BlockingQueue#add add"/>, which can fail to
		/// insert an element only by throwing an exception.
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
			if (count.Get() == Capacity)
			{
				return false;
			}
			int c = -1;
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock putLock = this.putLock;
			ReentrantLock putLock = this.PutLock;
			putLock.@lock();
			try
			{
				if (count.Get() < Capacity)
				{
					Enqueue(node);
					c = count.AndIncrement;
					if (c + 1 < Capacity)
					{
						NotFull.Signal();
					}
				}
			}
			finally
			{
				putLock.Unlock();
			}
			if (c == 0)
			{
				SignalNotEmpty();
			}
			return c >= 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E take() throws InterruptedException
		public virtual E Take()
		{
			E x;
			int c = -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.LockInterruptibly();
			try
			{
				while (count.Get() == 0)
				{
					NotEmpty.@await();
				}
				x = Dequeue();
				c = count.AndDecrement;
				if (c > 1)
				{
					NotEmpty.Signal();
				}
			}
			finally
			{
				takeLock.Unlock();
			}
			if (c == Capacity)
			{
				SignalNotFull();
			}
			return x;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E poll(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E Poll(long timeout, TimeUnit unit)
		{
			E x = null;
			int c = -1;
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.LockInterruptibly();
			try
			{
				while (count.Get() == 0)
				{
					if (nanos <= 0)
					{
						return null;
					}
					nanos = NotEmpty.AwaitNanos(nanos);
				}
				x = Dequeue();
				c = count.AndDecrement;
				if (c > 1)
				{
					NotEmpty.Signal();
				}
			}
			finally
			{
				takeLock.Unlock();
			}
			if (c == Capacity)
			{
				SignalNotFull();
			}
			return x;
		}

		public virtual E Poll()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = this.count;
			AtomicInteger count = this.Count;
			if (count.Get() == 0)
			{
				return null;
			}
			E x = null;
			int c = -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.@lock();
			try
			{
				if (count.Get() > 0)
				{
					x = Dequeue();
					c = count.AndDecrement;
					if (c > 1)
					{
						NotEmpty.Signal();
					}
				}
			}
			finally
			{
				takeLock.Unlock();
			}
			if (c == Capacity)
			{
				SignalNotFull();
			}
			return x;
		}

		public virtual E Peek()
		{
			if (Count.Get() == 0)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.@lock();
			try
			{
				Node<E> first = Head.Next;
				if (first == null)
				{
					return null;
				}
				else
				{
					return first.Item;
				}
			}
			finally
			{
				takeLock.Unlock();
			}
		}

		/// <summary>
		/// Unlinks interior Node p with predecessor trail.
		/// </summary>
		internal virtual void Unlink(Node<E> p, Node<E> trail)
		{
			// assert isFullyLocked();
			// p.next is not changed, to allow iterators that are
			// traversing p to maintain their weak-consistency guarantee.
			p.Item = null;
			trail.Next = p.Next;
			if (Last == p)
			{
				Last = trail;
			}
			if (Count.AndDecrement == Capacity)
			{
				NotFull.Signal();
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
			if (o == null)
			{
				return false;
			}
			FullyLock();
			try
			{
				for (Node<E> trail = Head, p = trail.Next; p != null; trail = p, p = p.next)
				{
					if (o.Equals(p.item))
					{
						Unlink(p, trail);
						return true;
					}
				}
				return false;
			}
			finally
			{
				FullyUnlock();
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
			FullyLock();
			try
			{
				for (Node<E> p = Head.Next; p != null; p = p.Next)
				{
					if (o.Equals(p.Item))
					{
						return true;
					}
				}
				return false;
			}
			finally
			{
				FullyUnlock();
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
			FullyLock();
			try
			{
				int size = Count.Get();
				Object[] a = new Object[size];
				int k = 0;
				for (Node<E> p = Head.Next; p != null; p = p.Next)
				{
					a[k++] = p.Item;
				}
				return a;
			}
			finally
			{
				FullyUnlock();
			}
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
			FullyLock();
			try
			{
				int size = Count.Get();
				if (a.Length < size)
				{
					a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), size);
				}

				int k = 0;
				for (Node<E> p = Head.Next; p != null; p = p.Next)
				{
					a[k++] = (T)p.Item;
				}
				if (a.Length > k)
				{
					a[k] = null;
				}
				return a;
			}
			finally
			{
				FullyUnlock();
			}
		}

		public override String ToString()
		{
			FullyLock();
			try
			{
				Node<E> p = Head.Next;
				if (p == null)
				{
					return "[]";
				}

				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (;;)
				{
					E e = p.Item;
					sb.Append(e == this ? "(this Collection)" : e);
					p = p.Next;
					if (p == null)
					{
						return sb.Append(']').ToString();
					}
					sb.Append(',').Append(' ');
				}
			}
			finally
			{
				FullyUnlock();
			}
		}

		/// <summary>
		/// Atomically removes all of the elements from this queue.
		/// The queue will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			FullyLock();
			try
			{
				for (Node<E> p, h = Head; (p = h.Next) != null; h = p)
				{
					h.Next = h;
					p.item = null;
				}
				Head = Last;
				// assert head.item == null && head.next == null;
				if (Count.GetAndSet(0) == Capacity)
				{
					NotFull.Signal();
				}
			}
			finally
			{
				FullyUnlock();
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
			bool signalNotFull = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock takeLock = this.takeLock;
			ReentrantLock takeLock = this.TakeLock;
			takeLock.@lock();
			try
			{
				int n = System.Math.Min(maxElements, Count.Get());
				// count.get provides visibility to first n Nodes
				Node<E> h = Head;
				int i = 0;
				try
				{
					while (i < n)
					{
						Node<E> p = h.Next;
						c.Add(p.Item);
						p.Item = null;
						h.Next = h;
						h = p;
						++i;
					}
					return n;
				}
				finally
				{
					// Restore invariants even if c.add() threw
					if (i > 0)
					{
						// assert h.item == null;
						Head = h;
						signalNotFull = (Count.GetAndAdd(-i) == Capacity);
					}
				}
			}
			finally
			{
				takeLock.Unlock();
				if (signalNotFull)
				{
					SignalNotFull();
				}
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

		private class Itr : Iterator<E>
		{
			private readonly LinkedBlockingQueue<E> OuterInstance;

			/*
			 * Basic weakly-consistent iterator.  At all times hold the next
			 * item to hand out so that if hasNext() reports true, we will
			 * still have it to return even if lost race with a take etc.
			 */

			internal Node<E> Current;
			internal Node<E> LastRet;
			internal E CurrentElement;

			internal Itr(LinkedBlockingQueue<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				outerInstance.FullyLock();
				try
				{
					Current = outerInstance.Head.Next;
					if (Current != null)
					{
						CurrentElement = Current.Item;
					}
				}
				finally
				{
					outerInstance.FullyUnlock();
				}
			}

			public virtual bool HasNext()
			{
				return Current != null;
			}

			/// <summary>
			/// Returns the next live successor of p, or null if no such.
			/// 
			/// Unlike other traversal methods, iterators need to handle both:
			/// - dequeued nodes (p.next == p)
			/// - (possibly multiple) interior removed nodes (p.item == null)
			/// </summary>
			internal virtual Node<E> NextNode(Node<E> p)
			{
				for (;;)
				{
					Node<E> s = p.Next;
					if (s == p)
					{
						return outerInstance.Head.Next;
					}
					if (s == null || s.Item != null)
					{
						return s;
					}
					p = s;
				}
			}

			public virtual E Next()
			{
				outerInstance.FullyLock();
				try
				{
					if (Current == null)
					{
						throw new NoSuchElementException();
					}
					E x = CurrentElement;
					LastRet = Current;
					Current = NextNode(Current);
					CurrentElement = (Current == null) ? null : Current.Item;
					return x;
				}
				finally
				{
					outerInstance.FullyUnlock();
				}
			}

			public virtual void Remove()
			{
				if (LastRet == null)
				{
					throw new IllegalStateException();
				}
				outerInstance.FullyLock();
				try
				{
					Node<E> node = LastRet;
					LastRet = null;
					for (Node<E> trail = outerInstance.Head, p = trail.Next; p != null; trail = p, p = p.next)
					{
						if (p == node)
						{
							outerInstance.Unlink(p, trail);
							break;
						}
					}
				}
				finally
				{
					outerInstance.FullyUnlock();
				}
			}
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class LBQSpliterator<E> : Spliterator<E>
		{
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly LinkedBlockingQueue<E> Queue;
			internal Node<E> Current; // current node; null until initialized
			internal int Batch; // batch size for splits
			internal bool Exhausted; // true when no more nodes
			internal long Est; // size estimate
			internal LBQSpliterator(LinkedBlockingQueue<E> queue)
			{
				this.Queue = queue;
				this.Est = queue.Size();
			}

			public long EstimateSize()
			{
				return Est;
			}

			public Spliterator<E> TrySplit()
			{
				Node<E> h;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedBlockingQueue<E> q = this.queue;
				LinkedBlockingQueue<E> q = this.Queue;
				int b = Batch;
				int n = (b <= 0) ? 1 : (b >= MAX_BATCH) ? MAX_BATCH : b + 1;
				if (!Exhausted && ((h = Current) != null || (h = q.Head.Next) != null) && h.Next != null)
				{
					Object[] a = new Object[n];
					int i = 0;
					Node<E> p = Current;
					q.FullyLock();
					try
					{
						if (p != null || (p = q.Head.Next) != null)
						{
							do
							{
								if ((a[i] = p.Item) != null)
								{
									++i;
								}
							} while ((p = p.Next) != null && i < n);
						}
					}
					finally
					{
						q.FullyUnlock();
					}
					if ((Current = p) == null)
					{
						Est = 0L;
						Exhausted = true;
					}
					else if ((Est -= i) < 0L)
					{
						Est = 0L;
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
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedBlockingQueue<E> q = this.queue;
				LinkedBlockingQueue<E> q = this.Queue;
				if (!Exhausted)
				{
					Exhausted = true;
					Node<E> p = Current;
					do
					{
						E e = null;
						q.FullyLock();
						try
						{
							if (p == null)
							{
								p = q.Head.Next;
							}
							while (p != null)
							{
								e = p.Item;
								p = p.Next;
								if (e != null)
								{
									break;
								}
							}
						}
						finally
						{
							q.FullyUnlock();
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
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LinkedBlockingQueue<E> q = this.queue;
				LinkedBlockingQueue<E> q = this.Queue;
				if (!Exhausted)
				{
					E e = null;
					q.FullyLock();
					try
					{
						if (Current == null)
						{
							Current = q.Head.Next;
						}
						while (Current != null)
						{
							e = Current.Item;
							Current = Current.Next;
							if (e != null)
							{
								break;
							}
						}
					}
					finally
					{
						q.FullyUnlock();
					}
					if (Current == null)
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
			return new LBQSpliterator<E>(this);
		}

		/// <summary>
		/// Saves this queue to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData The capacity is emitted (int), followed by all of
		/// its elements (each an {@code Object}) in the proper order,
		/// followed by a null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{

			FullyLock();
			try
			{
				// Write out any hidden stuff, plus capacity
				s.DefaultWriteObject();

				// Write out all elements in the proper order.
				for (Node<E> p = Head.Next; p != null; p = p.Next)
				{
					s.WriteObject(p.Item);
				}

				// Use trailing null as sentinel
				s.WriteObject(null);
			}
			finally
			{
				FullyUnlock();
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
			// Read in capacity, and any hidden stuff
			s.DefaultReadObject();

			Count.Set(0);
			Last = Head = new Node<E>(null);

			// Read in all elements and place in queue
			for (;;)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E item = (E)s.readObject();
				E item = (E)s.ReadObject();
				if (item == null)
				{
					break;
				}
				Add(item);
			}
		}
	}

}