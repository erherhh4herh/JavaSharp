using System;
using System.Diagnostics;

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
 * Written by Josh Bloch of Google Inc. and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/.
 */

namespace java.util
{


	/// <summary>
	/// Resizable-array implementation of the <seealso cref="Deque"/> interface.  Array
	/// deques have no capacity restrictions; they grow as necessary to support
	/// usage.  They are not thread-safe; in the absence of external
	/// synchronization, they do not support concurrent access by multiple threads.
	/// Null elements are prohibited.  This class is likely to be faster than
	/// <seealso cref="Stack"/> when used as a stack, and faster than <seealso cref="LinkedList"/>
	/// when used as a queue.
	/// 
	/// <para>Most {@code ArrayDeque} operations run in amortized constant time.
	/// Exceptions include <seealso cref="#remove(Object) remove"/>, {@link
	/// #removeFirstOccurrence removeFirstOccurrence}, {@link #removeLastOccurrence
	/// removeLastOccurrence}, <seealso cref="#contains contains"/>, {@link #iterator
	/// iterator.remove()}, and the bulk operations, all of which run in linear
	/// time.
	/// 
	/// </para>
	/// <para>The iterators returned by this class's {@code iterator} method are
	/// <i>fail-fast</i>: If the deque is modified at any time after the iterator
	/// is created, in any way except through the iterator's own {@code remove}
	/// method, the iterator will generally throw a {@link
	/// ConcurrentModificationException}.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than risking
	/// arbitrary, non-deterministic behavior at an undetermined time in the
	/// future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness: <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
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
	/// @author  Josh Bloch and Doug Lea
	/// @since   1.6
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class ArrayDeque<E> : AbstractCollection<E>, Deque<E>, Cloneable
	{
		/// <summary>
		/// The array in which the elements of the deque are stored.
		/// The capacity of the deque is the length of this array, which is
		/// always a power of two. The array is never allowed to become
		/// full, except transiently within an addX method where it is
		/// resized (see doubleCapacity) immediately upon becoming full,
		/// thus avoiding head and tail wrapping around to equal each
		/// other.  We also guarantee that all array cells not holding
		/// deque elements are always null.
		/// </summary>
		[NonSerialized]
		internal Object[] Elements; // non-private to simplify nested class access

		/// <summary>
		/// The index of the element at the head of the deque (which is the
		/// element that would be removed by remove() or pop()); or an
		/// arbitrary number equal to tail if the deque is empty.
		/// </summary>
		[NonSerialized]
		internal int Head;

		/// <summary>
		/// The index at which the next element would be added to the tail
		/// of the deque (via addLast(E), add(E), or push(E)).
		/// </summary>
		[NonSerialized]
		internal int Tail;

		/// <summary>
		/// The minimum capacity that we'll use for a newly created deque.
		/// Must be a power of 2.
		/// </summary>
		private const int MIN_INITIAL_CAPACITY = 8;

		// ******  Array allocation and resizing utilities ******

		/// <summary>
		/// Allocates empty array to hold the given number of elements.
		/// </summary>
		/// <param name="numElements">  the number of elements to hold </param>
		private void AllocateElements(int numElements)
		{
			int initialCapacity = MIN_INITIAL_CAPACITY;
			// Find the best power of two to hold elements.
			// Tests "<=" because arrays aren't kept full.
			if (numElements >= initialCapacity)
			{
				initialCapacity = numElements;
				initialCapacity |= ((int)((uint)initialCapacity >> 1));
				initialCapacity |= ((int)((uint)initialCapacity >> 2));
				initialCapacity |= ((int)((uint)initialCapacity >> 4));
				initialCapacity |= ((int)((uint)initialCapacity >> 8));
				initialCapacity |= ((int)((uint)initialCapacity >> 16));
				initialCapacity++;

				if (initialCapacity < 0) // Too many elements, must back off
				{
					initialCapacity = (int)((uint)initialCapacity >> 1); // Good luck allocating 2 ^ 30 elements
				}
			}
			Elements = new Object[initialCapacity];
		}

		/// <summary>
		/// Doubles the capacity of this deque.  Call only when full, i.e.,
		/// when head and tail have wrapped around to become equal.
		/// </summary>
		private void DoubleCapacity()
		{
			Debug.Assert(Head == Tail);
			int p = Head;
			int n = Elements.Length;
			int r = n - p; // number of elements to the right of p
			int newCapacity = n << 1;
			if (newCapacity < 0)
			{
				throw new IllegalStateException("Sorry, deque too big");
			}
			Object[] a = new Object[newCapacity];
			System.Array.Copy(Elements, p, a, 0, r);
			System.Array.Copy(Elements, 0, a, r, p);
			Elements = a;
			Head = 0;
			Tail = n;
		}

		/// <summary>
		/// Copies the elements from our element array into the specified array,
		/// in order (from first to last element in the deque).  It is assumed
		/// that the array is large enough to hold all elements in the deque.
		/// </summary>
		/// <returns> its argument </returns>
		private T[] copyElements<T>(T[] a)
		{
			if (Head < Tail)
			{
				System.Array.Copy(Elements, Head, a, 0, Size());
			}
			else if (Head > Tail)
			{
				int headPortionLen = Elements.Length - Head;
				System.Array.Copy(Elements, Head, a, 0, headPortionLen);
				System.Array.Copy(Elements, 0, a, headPortionLen, Tail);
			}
			return a;
		}

		/// <summary>
		/// Constructs an empty array deque with an initial capacity
		/// sufficient to hold 16 elements.
		/// </summary>
		public ArrayDeque()
		{
			Elements = new Object[16];
		}

		/// <summary>
		/// Constructs an empty array deque with an initial capacity
		/// sufficient to hold the specified number of elements.
		/// </summary>
		/// <param name="numElements">  lower bound on initial capacity of the deque </param>
		public ArrayDeque(int numElements)
		{
			AllocateElements(numElements);
		}

		/// <summary>
		/// Constructs a deque containing the elements of the specified
		/// collection, in the order they are returned by the collection's
		/// iterator.  (The first element returned by the collection's
		/// iterator becomes the first element, or <i>front</i> of the
		/// deque.)
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed into the deque </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public ArrayDeque<T1>(Collection<T1> c) where T1 : E
		{
			AllocateElements(c.Size());
			AddAll(c);
		}

		// The main insertion and extraction methods are addFirst,
		// addLast, pollFirst, pollLast. The other methods are defined in
		// terms of these.

		/// <summary>
		/// Inserts the specified element at the front of this deque.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void AddFirst(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Elements[Head = (Head - 1) & (Elements.Length - 1)] = e;
			if (Head == Tail)
			{
				DoubleCapacity();
			}
		}

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// 
		/// <para>This method is equivalent to <seealso cref="#add"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void AddLast(E e)
		{
			if (e == null)
			{
				throw new NullPointerException();
			}
			Elements[Tail] = e;
			if ((Tail = (Tail + 1) & (Elements.Length - 1)) == Head)
			{
				DoubleCapacity();
			}
		}

		/// <summary>
		/// Inserts the specified element at the front of this deque.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerFirst"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool OfferFirst(E e)
		{
			AddFirst(e);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerLast"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool OfferLast(E e)
		{
			AddLast(e);
			return true;
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
			int h = Head;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[h];
			E result = (E) Elements[h];
			// Element is null if deque empty
			if (result == null)
			{
				return null;
			}
			Elements[h] = null; // Must null out slot
			Head = (h + 1) & (Elements.Length - 1);
			return result;
		}

		public virtual E PollLast()
		{
			int t = (Tail - 1) & (Elements.Length - 1);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[t];
			E result = (E) Elements[t];
			if (result == null)
			{
				return null;
			}
			Elements[t] = null;
			Tail = t;
			return result;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E First
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[head];
				E result = (E) Elements[Head];
				if (result == null)
				{
					throw new NoSuchElementException();
				}
				return result;
			}
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Last
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[(tail - 1) & (elements.length - 1)];
				E result = (E) Elements[(Tail - 1) & (Elements.Length - 1)];
				if (result == null)
				{
					throw new NoSuchElementException();
				}
				return result;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E peekFirst()
		public virtual E PeekFirst()
		{
			// elements[head] is null if deque empty
			return (E) Elements[Head];
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E peekLast()
		public virtual E PeekLast()
		{
			return (E) Elements[(Tail - 1) & (Elements.Length - 1)];
		}

		/// <summary>
		/// Removes the first occurrence of the specified element in this
		/// deque (when traversing the deque from head to tail).
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if the deque contained the specified element </returns>
		public virtual bool RemoveFirstOccurrence(Object o)
		{
			if (o == null)
			{
				return false;
			}
			int mask = Elements.Length - 1;
			int i = Head;
			Object x;
			while ((x = Elements[i]) != null)
			{
				if (o.Equals(x))
				{
					Delete(i);
					return true;
				}
				i = (i + 1) & mask;
			}
			return false;
		}

		/// <summary>
		/// Removes the last occurrence of the specified element in this
		/// deque (when traversing the deque from head to tail).
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the last element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if the deque contained the specified element </returns>
		public virtual bool RemoveLastOccurrence(Object o)
		{
			if (o == null)
			{
				return false;
			}
			int mask = Elements.Length - 1;
			int i = (Tail - 1) & mask;
			Object x;
			while ((x = Elements[i]) != null)
			{
				if (o.Equals(x))
				{
					Delete(i);
					return true;
				}
				i = (i - 1) & mask;
			}
			return false;
		}

		// *** Queue methods ***

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			AddLast(e);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the end of this deque.
		/// 
		/// <para>This method is equivalent to <seealso cref="#offerLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Offer(E e)
		{
			return OfferLast(e);
		}

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque.
		/// 
		/// This method differs from <seealso cref="#poll poll"/> only in that it throws an
		/// exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Remove()
		{
			return RemoveFirst();
		}

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque), or returns
		/// {@code null} if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#pollFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque, or
		///         {@code null} if this deque is empty </returns>
		public virtual E Poll()
		{
			return PollFirst();
		}

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque.  This method differs from <seealso cref="#peek peek"/> only in
		/// that it throws an exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#getFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Element()
		{
			return First;
		}

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque, or returns {@code null} if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#peekFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque, or
		///         {@code null} if this deque is empty </returns>
		public virtual E Peek()
		{
			return PeekFirst();
		}

		// *** Stack methods ***

		/// <summary>
		/// Pushes an element onto the stack represented by this deque.  In other
		/// words, inserts the element at the front of this deque.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to push </param>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual void Push(E e)
		{
			AddFirst(e);
		}

		/// <summary>
		/// Pops an element from the stack represented by this deque.  In other
		/// words, removes and returns the first element of this deque.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the element at the front of this deque (which is the top
		///         of the stack represented by this deque) </returns>
		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Pop()
		{
			return RemoveFirst();
		}

		private void CheckInvariants()
		{
			Debug.Assert(Elements[Tail] == null);
			Debug.Assert(Head == Tail ? Elements[Head] == null, (Elements[Head] != null && Elements[(Tail - 1) & (Elements.Length - 1)] != null));
			Debug.Assert(Elements[(Head - 1) & (Elements.Length - 1)] == null);
		}

		/// <summary>
		/// Removes the element at the specified position in the elements array,
		/// adjusting head and tail as necessary.  This can result in motion of
		/// elements backwards or forwards in the array.
		/// 
		/// <para>This method is called delete rather than remove to emphasize
		/// that its semantics differ from those of <seealso cref="List#remove(int)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if elements moved backwards </returns>
		private bool Delete(int i)
		{
			CheckInvariants();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] elements = this.elements;
			Object[] elements = this.Elements;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mask = elements.length - 1;
			int mask = elements.Length - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int h = head;
			int h = Head;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int t = tail;
			int t = Tail;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int front = (i - h) & mask;
			int front = (i - h) & mask;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int back = (t - i) & mask;
			int back = (t - i) & mask;

			// Invariant: head <= i < tail mod circularity
			if (front >= ((t - h) & mask))
			{
				throw new ConcurrentModificationException();
			}

			// Optimize for least element motion
			if (front < back)
			{
				if (h <= i)
				{
					System.Array.Copy(elements, h, elements, h + 1, front);
				} // Wrap around
				else
				{
					System.Array.Copy(elements, 0, elements, 1, i);
					elements[0] = elements[mask];
					System.Array.Copy(elements, h, elements, h + 1, mask - h);
				}
				elements[h] = null;
				Head = (h + 1) & mask;
				return false;
			}
			else
			{
				if (i < t) // Copy the null tail as well
				{
					System.Array.Copy(elements, i + 1, elements, i, back);
					Tail = t - 1;
				} // Wrap around
				else
				{
					System.Array.Copy(elements, i + 1, elements, i, mask - i);
					elements[mask] = elements[0];
					System.Array.Copy(elements, 1, elements, 0, t);
					Tail = (t - 1) & mask;
				}
				return true;
			}
		}

		// *** Collection Methods ***

		/// <summary>
		/// Returns the number of elements in this deque.
		/// </summary>
		/// <returns> the number of elements in this deque </returns>
		public virtual int Size()
		{
			return (Tail - Head) & (Elements.Length - 1);
		}

		/// <summary>
		/// Returns {@code true} if this deque contains no elements.
		/// </summary>
		/// <returns> {@code true} if this deque contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Head == Tail;
			}
		}

		/// <summary>
		/// Returns an iterator over the elements in this deque.  The elements
		/// will be ordered from first (head) to last (tail).  This is the same
		/// order that elements would be dequeued (via successive calls to
		/// <seealso cref="#remove"/> or popped (via successive calls to <seealso cref="#pop"/>).
		/// </summary>
		/// <returns> an iterator over the elements in this deque </returns>
		public virtual Iterator<E> Iterator()
		{
			return new DeqIterator(this);
		}

		public virtual Iterator<E> DescendingIterator()
		{
			return new DescendingIterator(this);
		}

		private class DeqIterator : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				Cursor = outerInstance.Head;
				Fence = outerInstance.Tail;
			}

			private readonly ArrayDeque<E> OuterInstance;

			public DeqIterator(ArrayDeque<E> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			/// <summary>
			/// Index of element to be returned by subsequent call to next.
			/// </summary>
			internal int Cursor;

			/// <summary>
			/// Tail recorded at construction (also in remove), to stop
			/// iterator and also to check for comodification.
			/// </summary>
			internal int Fence;

			/// <summary>
			/// Index of element returned by most recent call to next.
			/// Reset to -1 if element is deleted by a call to remove.
			/// </summary>
			internal int LastRet = -1;

			public virtual bool HasNext()
			{
				return Cursor != Fence;
			}

			public virtual E Next()
			{
				if (Cursor == Fence)
				{
					throw new NoSuchElementException();
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[cursor];
				E result = (E) outerInstance.Elements[Cursor];
				// This check doesn't catch all possible comodifications,
				// but does catch the ones that corrupt traversal
				if (outerInstance.Tail != Fence || result == null)
				{
					throw new ConcurrentModificationException();
				}
				LastRet = Cursor;
				Cursor = (Cursor + 1) & (outerInstance.Elements.Length - 1);
				return result;
			}

			public virtual void Remove()
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				if (outerInstance.Delete(LastRet)) // if left-shifted, undo increment in next()
				{
					Cursor = (Cursor - 1) & (outerInstance.Elements.Length - 1);
					Fence = outerInstance.Tail;
				}
				LastRet = -1;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public virtual void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				Object[] a = outerInstance.Elements;
				int m = a.Length - 1, f = Fence, i = Cursor;
				Cursor = f;
				while (i != f)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E)a[i];
					E e = (E)a[i];
					i = (i + 1) & m;
					if (e == null)
					{
						throw new ConcurrentModificationException();
					}
					action.Accept(e);
				}
			}
		}

		private class DescendingIterator : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				Cursor = outerInstance.Tail;
				Fence = outerInstance.Head;
			}

			private readonly ArrayDeque<E> OuterInstance;

			public DescendingIterator(ArrayDeque<E> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			/*
			 * This class is nearly a mirror-image of DeqIterator, using
			 * tail instead of head for initial cursor, and head instead of
			 * tail for fence.
			 */
			internal int Cursor;
			internal int Fence;
			internal int LastRet = -1;

			public virtual bool HasNext()
			{
				return Cursor != Fence;
			}

			public virtual E Next()
			{
				if (Cursor == Fence)
				{
					throw new NoSuchElementException();
				}
				Cursor = (Cursor - 1) & (outerInstance.Elements.Length - 1);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E result = (E) elements[cursor];
				E result = (E) outerInstance.Elements[Cursor];
				if (outerInstance.Head != Fence || result == null)
				{
					throw new ConcurrentModificationException();
				}
				LastRet = Cursor;
				return result;
			}

			public virtual void Remove()
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				if (!outerInstance.Delete(LastRet))
				{
					Cursor = (Cursor + 1) & (outerInstance.Elements.Length - 1);
					Fence = outerInstance.Head;
				}
				LastRet = -1;
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
			int mask = Elements.Length - 1;
			int i = Head;
			Object x;
			while ((x = Elements[i]) != null)
			{
				if (o.Equals(x))
				{
					return true;
				}
				i = (i + 1) & mask;
			}
			return false;
		}

		/// <summary>
		/// Removes a single instance of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirstOccurrence(Object)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if this deque contained the specified element </returns>
		public virtual bool Remove(Object o)
		{
			return RemoveFirstOccurrence(o);
		}

		/// <summary>
		/// Removes all of the elements from this deque.
		/// The deque will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			int h = Head;
			int t = Tail;
			if (h != t) // clear all cells
			{
				Head = Tail = 0;
				int i = h;
				int mask = Elements.Length - 1;
				do
				{
					Elements[i] = null;
					i = (i + 1) & mask;
				} while (i != t);
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this deque
		/// in proper sequence (from first to last element).
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
			return CopyElements(new Object[Size()]);
		}

		/// <summary>
		/// Returns an array containing all of the elements in this deque in
		/// proper sequence (from first to last element); the runtime type of the
		/// returned array is that of the specified array.  If the deque fits in
		/// the specified array, it is returned therein.  Otherwise, a new array
		/// is allocated with the runtime type of the specified array and the
		/// size of this deque.
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
			int size = Size();
			if (a.Length < size)
			{
				a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), size);
			}
			CopyElements(a);
			if (a.Length > size)
			{
				a[size] = null;
			}
			return a;
		}

		// *** Object methods ***

		/// <summary>
		/// Returns a copy of this deque.
		/// </summary>
		/// <returns> a copy of this deque </returns>
		public virtual ArrayDeque<E> Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ArrayDeque<E> result = (ArrayDeque<E>) base.clone();
				ArrayDeque<E> result = (ArrayDeque<E>) base.Clone();
				result.Elements = Arrays.CopyOf(Elements, Elements.Length);
				return result;
			}
			catch (CloneNotSupportedException)
			{
				throw new AssertionError();
			}
		}

		private const long SerialVersionUID = 2340985798034038923L;

		/// <summary>
		/// Saves this deque to a stream (that is, serializes it).
		/// 
		/// @serialData The current size ({@code int}) of the deque,
		/// followed by all of its elements (each an object reference) in
		/// first-to-last order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			// Write out size
			s.WriteInt(Size());

			// Write out elements in order.
			int mask = Elements.Length - 1;
			for (int i = Head; i != Tail; i = (i + 1) & mask)
			{
				s.WriteObject(Elements[i]);
			}
		}

		/// <summary>
		/// Reconstitutes this deque from a stream (that is, deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();

			// Read in size and allocate array
			int size = s.ReadInt();
			AllocateElements(size);
			Head = 0;
			Tail = size;

			// Read in all elements in the proper order.
			for (int i = 0; i < size; i++)
			{
				Elements[i] = s.ReadObject();
			}
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// deque.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#NONNULL"/>.  Overriding implementations should document
		/// the reporting of additional characteristic values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this deque
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return new DeqSpliterator<E>(this, -1, -1);
		}

		internal sealed class DeqSpliterator<E> : Spliterator<E>
		{
			internal readonly ArrayDeque<E> Deq;
			internal int Fence_Renamed; // -1 until first use
			internal int Index; // current index, modified on traverse/split

			/// <summary>
			/// Creates new spliterator covering the given array and range </summary>
			internal DeqSpliterator(ArrayDeque<E> deq, int origin, int fence)
			{
				this.Deq = deq;
				this.Index = origin;
				this.Fence_Renamed = fence;
			}

			internal int Fence
			{
				get
				{
					int t;
					if ((t = Fence_Renamed) < 0)
					{
						t = Fence_Renamed = Deq.Tail;
						Index = Deq.Head;
					}
					return t;
				}
			}

			public DeqSpliterator<E> TrySplit()
			{
				int t = Fence, h = Index, n = Deq.Elements.Length;
				if (h != t && ((h + 1) & (n - 1)) != t)
				{
					if (h > t)
					{
						t += n;
					}
					int m = ((int)((uint)(h + t) >> 1)) & (n - 1);
					return new DeqSpliterator<>(Deq, h, Index = m);
				}
				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> consumer)
			public void forEachRemaining<T1>(Consumer<T1> consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				Object[] a = Deq.Elements;
				int m = a.Length - 1, f = Fence, i = Index;
				Index = f;
				while (i != f)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E)a[i];
					E e = (E)a[i];
					i = (i + 1) & m;
					if (e == null)
					{
						throw new ConcurrentModificationException();
					}
					consumer.Accept(e);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base E> consumer)
			public bool tryAdvance<T1>(Consumer<T1> consumer)
			{
				if (consumer == null)
				{
					throw new NullPointerException();
				}
				Object[] a = Deq.Elements;
				int m = a.Length - 1, f = Fence, i = Index;
				if (i != Fence_Renamed)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E)a[i];
					E e = (E)a[i];
					Index = (i + 1) & m;
					if (e == null)
					{
						throw new ConcurrentModificationException();
					}
					consumer.Accept(e);
					return true;
				}
				return false;
			}

			public long EstimateSize()
			{
				int n = Fence - Index;
				if (n < 0)
				{
					n += Deq.Elements.Length;
				}
				return (long) n;
			}

			public override int Characteristics()
			{
				return Spliterator_Fields.ORDERED | Spliterator_Fields.SIZED | Spliterator_Fields.NONNULL | Spliterator_Fields.SUBSIZED;
			}
		}

	}

}