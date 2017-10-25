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
	/// An optionally-bounded <seealso cref="BlockingDeque blocking deque"/> based on
	/// linked nodes.
	/// 
	/// <para>The optional capacity bound constructor argument serves as a
	/// way to prevent excessive expansion. The capacity, if unspecified,
	/// is equal to <seealso cref="Integer#MAX_VALUE"/>.  Linked nodes are
	/// dynamically created upon each insertion unless this would bring the
	/// deque above capacity.
	/// 
	/// </para>
	/// <para>Most operations run in constant time (ignoring time spent
	/// blocking).  Exceptions include <seealso cref="#remove(Object) remove"/>,
	/// <seealso cref="#removeFirstOccurrence removeFirstOccurrence"/>, {@link
	/// #removeLastOccurrence removeLastOccurrence}, {@link #contains
	/// contains}, <seealso cref="#iterator iterator.remove()"/>, and the bulk
	/// operations, all of which run in linear time.
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
	/// @since 1.6
	/// @author  Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class LinkedBlockingDeque<E> : AbstractQueue<E>, BlockingDeque<E>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			NotEmpty = @lock.NewCondition();
			NotFull = @lock.NewCondition();
		}


		/*
		 * Implemented as a simple doubly-linked list protected by a
		 * single lock and using conditions to manage blocking.
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
		 * self-link implicitly means to jump to "first" (for next links)
		 * or "last" (for prev links).
		 */

		/*
		 * We have "diamond" multiple interface/abstract class inheritance
		 * here, and that introduces ambiguities. Often we want the
		 * BlockingDeque javadoc combined with the AbstractQueue
		 * implementation, so a lot of method specs are duplicated here.
		 */

		private const long SerialVersionUID = -387911632671998426L;

		/// <summary>
		/// Doubly-linked list node class </summary>
		internal sealed class Node<E>
		{
			/// <summary>
			/// The item, or null if this node has been removed.
			/// </summary>
			internal E Item;

			/// <summary>
			/// One of:
			/// - the real predecessor Node
			/// - this Node, meaning the predecessor is tail
			/// - null, meaning there is no predecessor
			/// </summary>
			internal Node<E> Prev;

			/// <summary>
			/// One of:
			/// - the real successor Node
			/// - this Node, meaning the successor is head
			/// - null, meaning there is no successor
			/// </summary>
			internal Node<E> Next;

			internal Node(E x)
			{
				Item = x;
			}
		}

		/// <summary>
		/// Pointer to first node.
		/// Invariant: (first == null && last == null) ||
		///            (first.prev == null && first.item != null)
		/// </summary>
		[NonSerialized]
		internal Node<E> First_Renamed;

		/// <summary>
		/// Pointer to last node.
		/// Invariant: (first == null && last == null) ||
		///            (last.next == null && last.item != null)
		/// </summary>
		[NonSerialized]
		internal Node<E> Last_Renamed;

		/// <summary>
		/// Number of items in the deque </summary>
		[NonSerialized]
		private int Count;

		/// <summary>
		/// Maximum number of items in the deque </summary>
		private readonly int Capacity;

		/// <summary>
		/// Main lock guarding all access </summary>
		internal readonly ReentrantLock @lock = new ReentrantLock();

		/// <summary>
		/// Condition for waiting takes </summary>
		private Condition NotEmpty;

		/// <summary>
		/// Condition for waiting puts </summary>
		private Condition NotFull;

		/// <summary>
		/// Creates a {@code LinkedBlockingDeque} with a capacity of
		/// <seealso cref="Integer#MAX_VALUE"/>.
		/// </summary>
		public LinkedBlockingDeque() : this(Integer.MaxValue)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a {@code LinkedBlockingDeque} with the given (fixed) capacity.
		/// </summary>
		/// <param name="capacity"> the capacity of this deque </param>
		/// <exception cref="IllegalArgumentException"> if {@code capacity} is less than 1 </exception>
		public LinkedBlockingDeque(int capacity)
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
		}

		/// <summary>
		/// Creates a {@code LinkedBlockingDeque} with a capacity of
		/// <seealso cref="Integer#MAX_VALUE"/>, initially containing the elements of
		/// the given collection, added in traversal order of the
		/// collection's iterator.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public LinkedBlockingDeque<T1>(ICollection<T1> c) where T1 : E : this(Integer.MaxValue)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock(); // Never contended, but necessary for visibility
			try
			{
				foreach (E e in c)
				{
					if (e == null)
					{
						throw new NullPointerException();
					}
					if (!LinkLast(new Node<E>(e)))
					{
						throw new IllegalStateException("Deque full");
					}
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}


		// Basic linking and unlinking operations, called only while holding lock

		/// <summary>
		/// Links node as first element, or returns false if full.
		/// </summary>
		private bool LinkFirst(Node<E> node)
		{
			// assert lock.isHeldByCurrentThread();
			if (Count >= Capacity)
			{
				return false;
			}
			Node<E> f = First_Renamed;
			node.Next = f;
			First_Renamed = node;
			if (Last_Renamed == null)
			{
				Last_Renamed = node;
			}
			else
			{
				f.Prev = node;
			}
			++Count;
			NotEmpty.Signal();
			return true;
		}

		/// <summary>
		/// Links node as last element, or returns false if full.
		/// </summary>
		private bool LinkLast(Node<E> node)
		{
			// assert lock.isHeldByCurrentThread();
			if (Count >= Capacity)
			{
				return false;
			}
			Node<E> l = Last_Renamed;
			node.Prev = l;
			Last_Renamed = node;
			if (First_Renamed == null)
			{
				First_Renamed = node;
			}
			else
			{
				l.Next = node;
			}
			++Count;
			NotEmpty.Signal();
			return true;
		}

		/// <summary>
		/// Removes and returns first element, or null if empty.
		/// </summary>
		private E UnlinkFirst()
		{
			// assert lock.isHeldByCurrentThread();
			Node<E> f = First_Renamed;
			if (f == null)
			{
				return null;
			}
			Node<E> n = f.Next;
			E item = f.Item;
			f.Item = null;
			f.Next = f; // help GC
			First_Renamed = n;
			if (n == null)
			{
				Last_Renamed = null;
			}
			else
			{
				n.Prev = null;
			}
			--Count;
			NotFull.Signal();
			return item;
		}

		/// <summary>
		/// Removes and returns last element, or null if empty.
		/// </summary>
		private E UnlinkLast()
		{
			// assert lock.isHeldByCurrentThread();
			Node<E> l = Last_Renamed;
			if (l == null)
			{
				return null;
			}
			Node<E> p = l.Prev;
			E item = l.Item;
			l.Item = null;
			l.Prev = l; // help GC
			Last_Renamed = p;
			if (p == null)
			{
				First_Renamed = null;
			}
			else
			{
				p.Next = null;
			}
			--Count;
			NotFull.Signal();
			return item;
		}

		/// <summary>
		/// Unlinks x.
		/// </summary>
		internal virtual void Unlink(Node<E> x)
		{
			// assert lock.isHeldByCurrentThread();
			Node<E> p = x.Prev;
			Node<E> n = x.Next;
			if (p == null)
			{
				UnlinkFirst();
			}
			else if (n == null)
			{
				UnlinkLast();
			}
			else
			{
				p.Next = n;
				n.Prev = p;
				x.Item = null;
				// Don't mess with x's links.  They may still be in use by
				// an iterator.
				--Count;
				NotFull.Signal();
			}
		}

		// BlockingDeque methods

		/// <exception cref="IllegalStateException"> if this deque is full </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual void AddFirst(E e)
		{
			if (!OfferFirst(e))
			{
				throw new IllegalStateException("Deque full");
			}
		}

		/// <exception cref="IllegalStateException"> if this deque is full </exception>
		/// <exception cref="NullPointerException">  {@inheritDoc} </exception>
		public virtual void AddLast(E e)
		{
			if (!OfferLast(e))
			{
				throw new IllegalStateException("Deque full");
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool OfferFirst(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return LinkFirst(node);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool OfferLast(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return LinkLast(node);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putFirst(E e) throws InterruptedException
		public virtual void PutFirst(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				while (!LinkFirst(node))
				{
					NotFull.@await();
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putLast(E e) throws InterruptedException
		public virtual void PutLast(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				while (!LinkLast(node))
				{
					NotFull.@await();
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean offerFirst(E e, long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool OfferFirst(E e, long timeout, TimeUnit unit)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				while (!LinkFirst(node))
				{
					if (nanos <= 0)
					{
						return false;
					}
					nanos = NotFull.AwaitNanos(nanos);
				}
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean offerLast(E e, long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool OfferLast(E e, long timeout, TimeUnit unit)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Node<E> node = new Node<E>(e);
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				while (!LinkLast(node))
				{
					if (nanos <= 0)
					{
						return false;
					}
					nanos = NotFull.AwaitNanos(nanos);
				}
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E RemoveFirst()
		{
			E x = PollFirst();
			if (x == null)
			{
				throw new NoSuchElementException();
			}
			return x;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E RemoveLast()
		{
			E x = PollLast();
			if (x == null)
			{
				throw new NoSuchElementException();
			}
			return x;
		}

		public virtual E PollFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return UnlinkFirst();
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual E PollLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return UnlinkLast();
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E takeFirst() throws InterruptedException
		public virtual E TakeFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				E x;
				while ((x = UnlinkFirst()) == null)
				{
					NotEmpty.@await();
				}
				return x;
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E takeLast() throws InterruptedException
		public virtual E TakeLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				E x;
				while ((x = UnlinkLast()) == null)
				{
					NotEmpty.@await();
				}
				return x;
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E pollFirst(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E PollFirst(long timeout, TimeUnit unit)
		{
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				E x;
				while ((x = UnlinkFirst()) == null)
				{
					if (nanos <= 0)
					{
						return null;
					}
					nanos = NotEmpty.AwaitNanos(nanos);
				}
				return x;
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E pollLast(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E PollLast(long timeout, TimeUnit unit)
		{
			long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.LockInterruptibly();
			try
			{
				E x;
				while ((x = UnlinkLast()) == null)
				{
					if (nanos <= 0)
					{
						return null;
					}
					nanos = NotEmpty.AwaitNanos(nanos);
				}
				return x;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E First
		{
			get
			{
				E x = PeekFirst();
				if (x == null)
				{
					throw new NoSuchElementException();
				}
				return x;
			}
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Last
		{
			get
			{
				E x = PeekLast();
				if (x == null)
				{
					throw new NoSuchElementException();
				}
				return x;
			}
		}

		public virtual E PeekFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return (First_Renamed == null) ? null : First_Renamed.Item;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual E PeekLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				return (Last_Renamed == null) ? null : Last_Renamed.Item;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual bool RemoveFirstOccurrence(Object o)
		{
			if (o == null)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				for (Node<E> p = First_Renamed; p != null; p = p.Next)
				{
					if (o.Equals(p.Item))
					{
						Unlink(p);
						return true;
					}
				}
				return false;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		public virtual bool RemoveLastOccurrence(Object o)
		{
			if (o == null)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				for (Node<E> p = Last_Renamed; p != null; p = p.Prev)
				{
					if (o.Equals(p.Item))
					{
						Unlink(p);
						return true;
					}
				}
				return false;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		// BlockingQueue methods

		/// <summary>
		/// Inserts the specified element at the end of this deque unless it would
		/// violate capacity restrictions.  When using a capacity-restricted deque,
		/// it is generally preferable to use method <seealso cref="#offer(Object) offer"/>.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalStateException"> if this deque is full </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			AddLast(e);
			return true;
		}

		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			return OfferLast(e);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void put(E e) throws InterruptedException
		public virtual void Put(E e)
		{
			PutLast(e);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean offer(E e, long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool Offer(E e, long timeout, TimeUnit unit)
		{
			return OfferLast(e, timeout, unit);
		}

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque.
		/// This method differs from <seealso cref="#poll poll"/> only in that it throws an
		/// exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst() removeFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		public virtual E Remove()
		{
			return RemoveFirst();
		}

		public virtual E Poll()
		{
			return PollFirst();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E take() throws InterruptedException
		public virtual E Take()
		{
			return TakeFirst();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public E poll(long timeout, TimeUnit unit) throws InterruptedException
		public virtual E Poll(long timeout, TimeUnit unit)
		{
			return PollFirst(timeout, unit);
		}

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque.  This method differs from <seealso cref="#peek peek"/> only in that
		/// it throws an exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#getFirst() getFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		public virtual E Element()
		{
			return First;
		}

		public virtual E Peek()
		{
			return PeekFirst();
		}

		/// <summary>
		/// Returns the number of additional elements that this deque can ideally
		/// (in the absence of memory or resource constraints) accept without
		/// blocking. This is always equal to the initial capacity of this deque
		/// less the current {@code size} of this deque.
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
				return Capacity - Count;
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
				int n = System.Math.Min(maxElements, Count);
				for (int i = 0; i < n; i++)
				{
					c.Add(First_Renamed.Item); // In this order, in case add() throws.
					UnlinkFirst();
				}
				return n;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		// Stack methods

		/// <exception cref="IllegalStateException"> if this deque is full </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual void Push(E e)
		{
			AddFirst(e);
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Pop()
		{
			return RemoveFirst();
		}

		// Collection methods

		/// <summary>
		/// Removes the first occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// 
		/// <para>This method is equivalent to
		/// <seealso cref="#removeFirstOccurrence(Object) removeFirstOccurrence"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if this deque changed as a result of the call </returns>
		public virtual bool Remove(Object o)
		{
			return RemoveFirstOccurrence(o);
		}

		/// <summary>
		/// Returns the number of elements in this deque.
		/// </summary>
		/// <returns> the number of elements in this deque </returns>
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

		/// <summary>
		/// Returns {@code true} if this deque contains the specified element.
		/// More formally, returns {@code true} if and only if this deque contains
		/// at least one element {@code e} such that {@code o.equals(e)}.
		/// </summary>
		/// <param name="o"> object to be checked for containment in this deque </param>
		/// <returns> {@code true} if this deque contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			if (o == null)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				for (Node<E> p = First_Renamed; p != null; p = p.Next)
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
				@lock.Unlock();
			}
		}

		/*
		 * TODO: Add support for more efficient bulk operations.
		 *
		 * We don't want to acquire the lock for every iteration, but we
		 * also want other threads a chance to interact with the
		 * collection, especially when count is close to capacity.
		 */

	//     /**
	//      * Adds all of the elements in the specified collection to this
	//      * queue.  Attempts to addAll of a queue to itself result in
	//      * {@code IllegalArgumentException}. Further, the behavior of
	//      * this operation is undefined if the specified collection is
	//      * modified while the operation is in progress.
	//      *
	//      * @param c collection containing elements to be added to this queue
	//      * @return {@code true} if this queue changed as a result of the call
	//      * @throws ClassCastException            {@inheritDoc}
	//      * @throws NullPointerException          {@inheritDoc}
	//      * @throws IllegalArgumentException      {@inheritDoc}
	//      * @throws IllegalStateException if this deque is full
	//      * @see #add(Object)
	//      */
	//     public boolean addAll(Collection<? extends E> c) {
	//         if (c == null)
	//             throw new NullPointerException();
	//         if (c == this)
	//             throw new IllegalArgumentException();
	//         final ReentrantLock lock = this.lock;
	//         lock.lock();
	//         try {
	//             boolean modified = false;
	//             for (E e : c)
	//                 if (linkLast(e))
	//                     modified = true;
	//             return modified;
	//         } finally {
	//             lock.unlock();
	//         }
	//     }

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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object[] toArray()
		public virtual Object[] ToArray()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] a = new Object[Count];
				int k = 0;
				for (Node<E> p = First_Renamed; p != null; p = p.Next)
				{
					a[k++] = p.Item;
				}
				return a;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this deque, in
		/// proper sequence; the runtime type of the returned array is that of
		/// the specified array.  If the deque fits in the specified array, it
		/// is returned therein.  Otherwise, a new array is allocated with the
		/// runtime type of the specified array and the size of this deque.
		/// 
		/// <para>If this deque fits in the specified array with room to spare
		/// (i.e., the array has more elements than this deque), the element in
		/// the array immediately following the end of the deque is set to
		/// {@code null}.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
		public virtual T[] toArray<T>(T[] a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				if (a.Length < Count)
				{
					a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), Count);
				}

				int k = 0;
				for (Node<E> p = First_Renamed; p != null; p = p.Next)
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
				Node<E> p = First_Renamed;
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
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Atomically removes all of the elements from this deque.
		/// The deque will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				for (Node<E> f = First_Renamed; f != null;)
				{
					f.Item = null;
					Node<E> n = f.Next;
					f.Prev = null;
					f.Next = null;
					f = n;
				}
				First_Renamed = Last_Renamed = null;
				Count = 0;
				NotFull.SignalAll();
			}
			finally
			{
				@lock.Unlock();
			}
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

		/// <summary>
		/// Base class for Iterators for LinkedBlockingDeque
		/// </summary>
		private abstract class AbstractItr : Iterator<E>
		{
			private readonly LinkedBlockingDeque<E> OuterInstance;

			/// <summary>
			/// The next node to return in next()
			/// </summary>
			internal Node<E> Next_Renamed;

			/// <summary>
			/// nextItem holds on to item fields because once we claim that
			/// an element exists in hasNext(), we must return item read
			/// under lock (in advance()) even if it was in the process of
			/// being removed when hasNext() was called.
			/// </summary>
			internal E NextItem;

			/// <summary>
			/// Node returned by most recent call to next. Needed by remove.
			/// Reset to null if this element is deleted by a call to remove.
			/// </summary>
			internal Node<E> LastRet;

			internal abstract Node<E> FirstNode();
			internal abstract Node<E> NextNode(Node<E> n);

			internal AbstractItr(LinkedBlockingDeque<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				// set to initial position
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = LinkedBlockingDeque.this.lock;
				ReentrantLock @lock = outerInstance.@lock;
				@lock.@lock();
				try
				{
					Next_Renamed = FirstNode();
					NextItem = (Next_Renamed == null) ? null : Next_Renamed.Item;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			/// <summary>
			/// Returns the successor node of the given non-null, but
			/// possibly previously deleted, node.
			/// </summary>
			internal virtual Node<E> Succ(Node<E> n)
			{
				// Chains of deleted nodes ending in null or self-links
				// are possible if multiple interior nodes are removed.
				for (;;)
				{
					Node<E> s = NextNode(n);
					if (s == null)
					{
						return null;
					}
					else if (s.Item != null)
					{
						return s;
					}
					else if (s == n)
					{
						return FirstNode();
					}
					else
					{
						n = s;
					}
				}
			}

			/// <summary>
			/// Advances next.
			/// </summary>
			internal virtual void Advance()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = LinkedBlockingDeque.this.lock;
				ReentrantLock @lock = OuterInstance.@lock;
				@lock.@lock();
				try
				{
					// assert next != null;
					Next_Renamed = Succ(Next_Renamed);
					NextItem = (Next_Renamed == null) ? null : Next_Renamed.Item;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool HasNext()
			{
				return Next_Renamed != null;
			}

			public virtual E Next()
			{
				if (Next_Renamed == null)
				{
					throw new NoSuchElementException();
				}
				LastRet = Next_Renamed;
				E x = NextItem;
				Advance();
				return x;
			}

			public virtual void Remove()
			{
				Node<E> n = LastRet;
				if (n == null)
				{
					throw new IllegalStateException();
				}
				LastRet = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = LinkedBlockingDeque.this.lock;
				ReentrantLock @lock = OuterInstance.@lock;
				@lock.@lock();
				try
				{
					if (n.Item != null)
					{
						outerInstance.Unlink(n);
					}
				}
				finally
				{
					@lock.Unlock();
				}
			}
		}

		/// <summary>
		/// Forward iterator </summary>
		private class Itr : AbstractItr
		{
			private readonly LinkedBlockingDeque<E> OuterInstance;

			public Itr(LinkedBlockingDeque<E> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal override Node<E> FirstNode()
			{
				return outerInstance.First_Renamed;
			}
			internal override Node<E> NextNode(Node<E> n)
			{
				return n.Next;
			}
		}

		/// <summary>
		/// Descending iterator </summary>
		private class DescendingItr : AbstractItr
		{
			private readonly LinkedBlockingDeque<E> OuterInstance;

			public DescendingItr(LinkedBlockingDeque<E> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal override Node<E> FirstNode()
			{
				return outerInstance.Last_Renamed;
			}
			internal override Node<E> NextNode(Node<E> n)
			{
				return n.Prev;
			}
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class LBDSpliterator<E> : Spliterator<E>
		{
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly LinkedBlockingDeque<E> Queue;
			internal Node<E> Current; // current node; null until initialized
			internal int Batch; // batch size for splits
			internal bool Exhausted; // true when no more nodes
			internal long Est; // size estimate
			internal LBDSpliterator(LinkedBlockingDeque<E> queue)
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
//ORIGINAL LINE: final LinkedBlockingDeque<E> q = this.queue;
				LinkedBlockingDeque<E> q = this.Queue;
				int b = Batch;
				int n = (b <= 0) ? 1 : (b >= MAX_BATCH) ? MAX_BATCH : b + 1;
				if (!Exhausted && ((h = Current) != null || (h = q.First_Renamed) != null) && h.Next != null)
				{
					Object[] a = new Object[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = q.lock;
					ReentrantLock @lock = q.@lock;
					int i = 0;
					Node<E> p = Current;
					@lock.@lock();
					try
					{
						if (p != null || (p = q.First_Renamed) != null)
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
						@lock.Unlock();
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
//ORIGINAL LINE: final LinkedBlockingDeque<E> q = this.queue;
				LinkedBlockingDeque<E> q = this.Queue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = q.lock;
				ReentrantLock @lock = q.@lock;
				if (!Exhausted)
				{
					Exhausted = true;
					Node<E> p = Current;
					do
					{
						E e = null;
						@lock.@lock();
						try
						{
							if (p == null)
							{
								p = q.First_Renamed;
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
							@lock.Unlock();
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
//ORIGINAL LINE: final LinkedBlockingDeque<E> q = this.queue;
				LinkedBlockingDeque<E> q = this.Queue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = q.lock;
				ReentrantLock @lock = q.@lock;
				if (!Exhausted)
				{
					E e = null;
					@lock.@lock();
					try
					{
						if (Current == null)
						{
							Current = q.First_Renamed;
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
						@lock.Unlock();
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
			return new LBDSpliterator<E>(this);
		}

		/// <summary>
		/// Saves this deque to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData The capacity (int), followed by elements (each an
		/// {@code Object}) in the proper order, followed by a null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				// Write out capacity and any hidden stuff
				s.DefaultWriteObject();
				// Write out all elements in the proper order.
				for (Node<E> p = First_Renamed; p != null; p = p.Next)
				{
					s.WriteObject(p.Item);
				}
				// Use trailing null as sentinel
				s.WriteObject(null);
			}
			finally
			{
				@lock.Unlock();
			}
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
			Count = 0;
			First_Renamed = null;
			Last_Renamed = null;
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