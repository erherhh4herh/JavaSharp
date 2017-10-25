using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Doubly-linked list implementation of the {@code List} and {@code Deque}
	/// interfaces.  Implements all optional list operations, and permits all
	/// elements (including {@code null}).
	/// 
	/// <para>All of the operations perform as could be expected for a doubly-linked
	/// list.  Operations that index into the list will traverse the list from
	/// the beginning or the end, whichever is closer to the specified index.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a linked list concurrently, and at least
	/// one of the threads modifies the list structurally, it <i>must</i> be
	/// synchronized externally.  (A structural modification is any operation
	/// that adds or deletes one or more elements; merely setting the value of
	/// an element is not a structural modification.)  This is typically
	/// accomplished by synchronizing on some object that naturally
	/// encapsulates the list.
	/// 
	/// If no such object exists, the list should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedList Collections.synchronizedList"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the list:<pre>
	///   List list = Collections.synchronizedList(new LinkedList(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by this class's {@code iterator} and
	/// {@code listIterator} methods are <i>fail-fast</i>: if the list is
	/// structurally modified at any time after the iterator is created, in
	/// any way except through the Iterator's own {@code remove} or
	/// {@code add} methods, the iterator will throw a {@link
	/// ConcurrentModificationException}.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than
	/// risking arbitrary, non-deterministic behavior at an undetermined
	/// time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness:   <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     List </seealso>
	/// <seealso cref=     ArrayList
	/// @since 1.2 </seealso>
	/// @param <E> the type of elements held in this collection </param>

	[Serializable]
	public class LinkedList<E> : AbstractSequentialList<E>, List<E>, Deque<E>, Cloneable
	{
		[NonSerialized]
		internal int Size_Renamed = 0;

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
		/// Constructs an empty list.
		/// </summary>
		public LinkedList()
		{
		}

		/// <summary>
		/// Constructs a list containing the elements of the specified
		/// collection, in the order they are returned by the collection's
		/// iterator.
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed into this list </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public LinkedList<T1>(Collection<T1> c) where T1 : E : this()
		{
			AddAll(c);
		}

		/// <summary>
		/// Links e as first element.
		/// </summary>
		private void LinkFirst(E e)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<>(null, e, f);
			Node<E> newNode = new Node<E>(null, e, f);
			First_Renamed = newNode;
			if (f == null)
			{
				Last_Renamed = newNode;
			}
			else
			{
				f.Prev = newNode;
			}
			Size_Renamed++;
			ModCount++;
		}

		/// <summary>
		/// Links e as last element.
		/// </summary>
		internal virtual void LinkLast(E e)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> l = last;
			Node<E> l = Last_Renamed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<>(l, e, null);
			Node<E> newNode = new Node<E>(l, e, null);
			Last_Renamed = newNode;
			if (l == null)
			{
				First_Renamed = newNode;
			}
			else
			{
				l.Next = newNode;
			}
			Size_Renamed++;
			ModCount++;
		}

		/// <summary>
		/// Inserts element e before non-null Node succ.
		/// </summary>
		internal virtual void LinkBefore(E e, Node<E> succ)
		{
			// assert succ != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> pred = succ.prev;
			Node<E> pred = succ.Prev;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> newNode = new Node<>(pred, e, succ);
			Node<E> newNode = new Node<E>(pred, e, succ);
			succ.Prev = newNode;
			if (pred == null)
			{
				First_Renamed = newNode;
			}
			else
			{
				pred.Next = newNode;
			}
			Size_Renamed++;
			ModCount++;
		}

		/// <summary>
		/// Unlinks non-null first node f.
		/// </summary>
		private E UnlinkFirst(Node<E> f)
		{
			// assert f == first && f != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E element = f.item;
			E element = f.Item;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> next = f.next;
			Node<E> next = f.Next;
			f.Item = null;
			f.Next = null; // help GC
			First_Renamed = next;
			if (next == null)
			{
				Last_Renamed = null;
			}
			else
			{
				next.Prev = null;
			}
			Size_Renamed--;
			ModCount++;
			return element;
		}

		/// <summary>
		/// Unlinks non-null last node l.
		/// </summary>
		private E UnlinkLast(Node<E> l)
		{
			// assert l == last && l != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E element = l.item;
			E element = l.Item;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> prev = l.prev;
			Node<E> prev = l.Prev;
			l.Item = null;
			l.Prev = null; // help GC
			Last_Renamed = prev;
			if (prev == null)
			{
				First_Renamed = null;
			}
			else
			{
				prev.Next = null;
			}
			Size_Renamed--;
			ModCount++;
			return element;
		}

		/// <summary>
		/// Unlinks non-null node x.
		/// </summary>
		internal virtual E Unlink(Node<E> x)
		{
			// assert x != null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E element = x.item;
			E element = x.Item;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> next = x.next;
			Node<E> next = x.Next;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> prev = x.prev;
			Node<E> prev = x.Prev;

			if (prev == null)
			{
				First_Renamed = next;
			}
			else
			{
				prev.Next = next;
				x.Prev = null;
			}

			if (next == null)
			{
				Last_Renamed = prev;
			}
			else
			{
				next.Prev = prev;
				x.Next = null;
			}

			x.Item = null;
			Size_Renamed--;
			ModCount++;
			return element;
		}

		/// <summary>
		/// Returns the first element in this list.
		/// </summary>
		/// <returns> the first element in this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty </exception>
		public virtual E First
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Node<E> f = first;
				Node<E> f = First_Renamed;
				if (f == null)
				{
					throw new NoSuchElementException();
				}
				return f.Item;
			}
		}

		/// <summary>
		/// Returns the last element in this list.
		/// </summary>
		/// <returns> the last element in this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty </exception>
		public virtual E Last
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Node<E> l = last;
				Node<E> l = Last_Renamed;
				if (l == null)
				{
					throw new NoSuchElementException();
				}
				return l.Item;
			}
		}

		/// <summary>
		/// Removes and returns the first element from this list.
		/// </summary>
		/// <returns> the first element from this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty </exception>
		public virtual E RemoveFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
			if (f == null)
			{
				throw new NoSuchElementException();
			}
			return UnlinkFirst(f);
		}

		/// <summary>
		/// Removes and returns the last element from this list.
		/// </summary>
		/// <returns> the last element from this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty </exception>
		public virtual E RemoveLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> l = last;
			Node<E> l = Last_Renamed;
			if (l == null)
			{
				throw new NoSuchElementException();
			}
			return UnlinkLast(l);
		}

		/// <summary>
		/// Inserts the specified element at the beginning of this list.
		/// </summary>
		/// <param name="e"> the element to add </param>
		public virtual void AddFirst(E e)
		{
			LinkFirst(e);
		}

		/// <summary>
		/// Appends the specified element to the end of this list.
		/// 
		/// <para>This method is equivalent to <seealso cref="#add"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		public virtual void AddLast(E e)
		{
			LinkLast(e);
		}

		/// <summary>
		/// Returns {@code true} if this list contains the specified element.
		/// More formally, returns {@code true} if and only if this list contains
		/// at least one element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this list is to be tested </param>
		/// <returns> {@code true} if this list contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			return IndexOf(o) != -1;
		}

		/// <summary>
		/// Returns the number of elements in this list.
		/// </summary>
		/// <returns> the number of elements in this list </returns>
		public virtual int Count
		{
			get
			{
				return Size_Renamed;
			}
		}

		/// <summary>
		/// Appends the specified element to the end of this list.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> element to be appended to this list </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		public virtual bool Add(E e)
		{
			LinkLast(e);
			return true;
		}

		/// <summary>
		/// Removes the first occurrence of the specified element from this list,
		/// if it is present.  If this list does not contain the element, it is
		/// unchanged.  More formally, removes the element with the lowest index
		/// {@code i} such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>
		/// (if such an element exists).  Returns {@code true} if this list
		/// contained the specified element (or equivalently, if this list
		/// changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this list, if present </param>
		/// <returns> {@code true} if this list contained the specified element </returns>
		public virtual bool Remove(Object o)
		{
			if (o == null)
			{
				for (Node<E> x = First_Renamed; x != null; x = x.Next)
				{
					if (x.Item == null)
					{
						Unlink(x);
						return true;
					}
				}
			}
			else
			{
				for (Node<E> x = First_Renamed; x != null; x = x.Next)
				{
					if (o.Equals(x.Item))
					{
						Unlink(x);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Appends all of the elements in the specified collection to the end of
		/// this list, in the order that they are returned by the specified
		/// collection's iterator.  The behavior of this operation is undefined if
		/// the specified collection is modified while the operation is in
		/// progress.  (Note that this will occur if the specified collection is
		/// this list, and it's nonempty.)
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			return AddAll(Size_Renamed, c);
		}

		/// <summary>
		/// Inserts all of the elements in the specified collection into this
		/// list, starting at the specified position.  Shifts the element
		/// currently at that position (if any) and any subsequent elements to
		/// the right (increases their indices).  The new elements will appear
		/// in the list in the order that they are returned by the
		/// specified collection's iterator.
		/// </summary>
		/// <param name="index"> index at which to insert the first element
		///              from the specified collection </param>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool addAll<T1>(int index, Collection<T1> c) where T1 : E
		{
			CheckPositionIndex(index);

			Object[] List_Fields.a = c.ToArray();
			int numNew = List_Fields.a.Length;
			if (numNew == 0)
			{
				return false;
			}

			Node<E> pred, succ;
			if (index == Size_Renamed)
			{
				succ = null;
				pred = Last_Renamed;
			}
			else
			{
				succ = Node(index);
				pred = succ.Prev;
			}

			foreach (Object o in List_Fields.a)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) o;
				E e = (E) o;
				Node<E> newNode = new Node<E>(pred, e, null);
				if (pred == null)
				{
					First_Renamed = newNode;
				}
				else
				{
					pred.Next = newNode;
				}
				pred = newNode;
			}

			if (succ == null)
			{
				Last_Renamed = pred;
			}
			else
			{
				pred.Next = succ;
				succ.Prev = pred;
			}

			Size_Renamed += numNew;
			ModCount++;
			return true;
		}

		/// <summary>
		/// Removes all of the elements from this list.
		/// The list will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			// Clearing all of the links between nodes is "unnecessary", but:
			// - helps a generational GC if the discarded nodes inhabit
			//   more than one generation
			// - is sure to free memory even if there is a reachable Iterator
			for (Node<E> x = First_Renamed; x != null;)
			{
				Node<E> next = x.Next;
				x.Item = null;
				x.Next = null;
				x.Prev = null;
				x = next;
			}
			First_Renamed = Last_Renamed = null;
			Size_Renamed = 0;
			ModCount++;
		}


		// Positional Access Operations

		/// <summary>
		/// Returns the element at the specified position in this list.
		/// </summary>
		/// <param name="index"> index of the element to return </param>
		/// <returns> the element at the specified position in this list </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Get(int index)
		{
			CheckElementIndex(index);
			return Node(index).Item;
		}

		/// <summary>
		/// Replaces the element at the specified position in this list with the
		/// specified element.
		/// </summary>
		/// <param name="index"> index of the element to replace </param>
		/// <param name="element"> element to be stored at the specified position </param>
		/// <returns> the element previously at the specified position </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Set(int index, E element)
		{
			CheckElementIndex(index);
			Node<E> x = Node(index);
			E oldVal = x.Item;
			x.Item = element;
			return oldVal;
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this list.
		/// Shifts the element currently at that position (if any) and any
		/// subsequent elements to the right (adds one to their indices).
		/// </summary>
		/// <param name="index"> index at which the specified element is to be inserted </param>
		/// <param name="element"> element to be inserted </param>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual void Add(int index, E element)
		{
			CheckPositionIndex(index);

			if (index == Size_Renamed)
			{
				LinkLast(element);
			}
			else
			{
				LinkBefore(element, Node(index));
			}
		}

		/// <summary>
		/// Removes the element at the specified position in this list.  Shifts any
		/// subsequent elements to the left (subtracts one from their indices).
		/// Returns the element that was removed from the list.
		/// </summary>
		/// <param name="index"> the index of the element to be removed </param>
		/// <returns> the element previously at the specified position </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Remove(int index)
		{
			CheckElementIndex(index);
			return Unlink(Node(index));
		}

		/// <summary>
		/// Tells if the argument is the index of an existing element.
		/// </summary>
		private bool IsElementIndex(int index)
		{
			return index >= 0 && index < Size_Renamed;
		}

		/// <summary>
		/// Tells if the argument is the index of a valid position for an
		/// iterator or an add operation.
		/// </summary>
		private bool IsPositionIndex(int index)
		{
			return index >= 0 && index <= Size_Renamed;
		}

		/// <summary>
		/// Constructs an IndexOutOfBoundsException detail message.
		/// Of the many possible refactorings of the error handling code,
		/// this "outlining" performs best with both server and client VMs.
		/// </summary>
		private String OutOfBoundsMsg(int index)
		{
			return "Index: " + index + ", Size: " + Size_Renamed;
		}

		private void CheckElementIndex(int index)
		{
			if (!IsElementIndex(index))
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		private void CheckPositionIndex(int index)
		{
			if (!IsPositionIndex(index))
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		/// <summary>
		/// Returns the (non-null) Node at the specified element index.
		/// </summary>
		internal virtual Node<E> Node(int index)
		{
			// assert isElementIndex(index);

			if (index < (Size_Renamed >> 1))
			{
				Node<E> x = First_Renamed;
				for (int List_Fields.i = 0; List_Fields.i < index; List_Fields.i++)
				{
					x = x.Next;
				}
				return x;
			}
			else
			{
				Node<E> x = Last_Renamed;
				for (int List_Fields.i = Size_Renamed - 1; List_Fields.i > index; List_Fields.i--)
				{
					x = x.Prev;
				}
				return x;
			}
		}

		// Search Operations

		/// <summary>
		/// Returns the index of the first occurrence of the specified element
		/// in this list, or -1 if this list does not contain the element.
		/// More formally, returns the lowest index {@code i} such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <returns> the index of the first occurrence of the specified element in
		///         this list, or -1 if this list does not contain the element </returns>
		public virtual int IndexOf(Object o)
		{
			int index = 0;
			if (o == null)
			{
				for (Node<E> x = First_Renamed; x != null; x = x.Next)
				{
					if (x.Item == null)
					{
						return index;
					}
					index++;
				}
			}
			else
			{
				for (Node<E> x = First_Renamed; x != null; x = x.Next)
				{
					if (o.Equals(x.Item))
					{
						return index;
					}
					index++;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the last occurrence of the specified element
		/// in this list, or -1 if this list does not contain the element.
		/// More formally, returns the highest index {@code i} such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <returns> the index of the last occurrence of the specified element in
		///         this list, or -1 if this list does not contain the element </returns>
		public virtual int LastIndexOf(Object o)
		{
			int index = Size_Renamed;
			if (o == null)
			{
				for (Node<E> x = Last_Renamed; x != null; x = x.Prev)
				{
					index--;
					if (x.Item == null)
					{
						return index;
					}
				}
			}
			else
			{
				for (Node<E> x = Last_Renamed; x != null; x = x.Prev)
				{
					index--;
					if (o.Equals(x.Item))
					{
						return index;
					}
				}
			}
			return -1;
		}

		// Queue operations.

		/// <summary>
		/// Retrieves, but does not remove, the head (first element) of this list.
		/// </summary>
		/// <returns> the head of this list, or {@code null} if this list is empty
		/// @since 1.5 </returns>
		public virtual E Peek()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
			return (f == null) ? null : f.Item;
		}

		/// <summary>
		/// Retrieves, but does not remove, the head (first element) of this list.
		/// </summary>
		/// <returns> the head of this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty
		/// @since 1.5 </exception>
		public virtual E Element()
		{
			return First;
		}

		/// <summary>
		/// Retrieves and removes the head (first element) of this list.
		/// </summary>
		/// <returns> the head of this list, or {@code null} if this list is empty
		/// @since 1.5 </returns>
		public virtual E Poll()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
			return (f == null) ? null : UnlinkFirst(f);
		}

		/// <summary>
		/// Retrieves and removes the head (first element) of this list.
		/// </summary>
		/// <returns> the head of this list </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty
		/// @since 1.5 </exception>
		public virtual E Remove()
		{
			return RemoveFirst();
		}

		/// <summary>
		/// Adds the specified element as the tail (last element) of this list.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>)
		/// @since 1.5 </returns>
		public virtual bool Offer(E e)
		{
			return Add(e);
		}

		// Deque operations
		/// <summary>
		/// Inserts the specified element at the front of this list.
		/// </summary>
		/// <param name="e"> the element to insert </param>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerFirst"/>)
		/// @since 1.6 </returns>
		public virtual bool OfferFirst(E e)
		{
			AddFirst(e);
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the end of this list.
		/// </summary>
		/// <param name="e"> the element to insert </param>
		/// <returns> {@code true} (as specified by <seealso cref="Deque#offerLast"/>)
		/// @since 1.6 </returns>
		public virtual bool OfferLast(E e)
		{
			AddLast(e);
			return true;
		}

		/// <summary>
		/// Retrieves, but does not remove, the first element of this list,
		/// or returns {@code null} if this list is empty.
		/// </summary>
		/// <returns> the first element of this list, or {@code null}
		///         if this list is empty
		/// @since 1.6 </returns>
		public virtual E PeekFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
			return (f == null) ? null : f.Item;
		}

		/// <summary>
		/// Retrieves, but does not remove, the last element of this list,
		/// or returns {@code null} if this list is empty.
		/// </summary>
		/// <returns> the last element of this list, or {@code null}
		///         if this list is empty
		/// @since 1.6 </returns>
		public virtual E PeekLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> l = last;
			Node<E> l = Last_Renamed;
			return (l == null) ? null : l.Item;
		}

		/// <summary>
		/// Retrieves and removes the first element of this list,
		/// or returns {@code null} if this list is empty.
		/// </summary>
		/// <returns> the first element of this list, or {@code null} if
		///     this list is empty
		/// @since 1.6 </returns>
		public virtual E PollFirst()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> f = first;
			Node<E> f = First_Renamed;
			return (f == null) ? null : UnlinkFirst(f);
		}

		/// <summary>
		/// Retrieves and removes the last element of this list,
		/// or returns {@code null} if this list is empty.
		/// </summary>
		/// <returns> the last element of this list, or {@code null} if
		///     this list is empty
		/// @since 1.6 </returns>
		public virtual E PollLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node<E> l = last;
			Node<E> l = Last_Renamed;
			return (l == null) ? null : UnlinkLast(l);
		}

		/// <summary>
		/// Pushes an element onto the stack represented by this list.  In other
		/// words, inserts the element at the front of this list.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to push
		/// @since 1.6 </param>
		public virtual void Push(E e)
		{
			AddFirst(e);
		}

		/// <summary>
		/// Pops an element from the stack represented by this list.  In other
		/// words, removes and returns the first element of this list.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the element at the front of this list (which is the top
		///         of the stack represented by this list) </returns>
		/// <exception cref="NoSuchElementException"> if this list is empty
		/// @since 1.6 </exception>
		public virtual E Pop()
		{
			return RemoveFirst();
		}

		/// <summary>
		/// Removes the first occurrence of the specified element in this
		/// list (when traversing the list from head to tail).  If the list
		/// does not contain the element, it is unchanged.
		/// </summary>
		/// <param name="o"> element to be removed from this list, if present </param>
		/// <returns> {@code true} if the list contained the specified element
		/// @since 1.6 </returns>
		public virtual bool RemoveFirstOccurrence(Object o)
		{
			return Remove(o);
		}

		/// <summary>
		/// Removes the last occurrence of the specified element in this
		/// list (when traversing the list from head to tail).  If the list
		/// does not contain the element, it is unchanged.
		/// </summary>
		/// <param name="o"> element to be removed from this list, if present </param>
		/// <returns> {@code true} if the list contained the specified element
		/// @since 1.6 </returns>
		public virtual bool RemoveLastOccurrence(Object o)
		{
			if (o == null)
			{
				for (Node<E> x = Last_Renamed; x != null; x = x.Prev)
				{
					if (x.Item == null)
					{
						Unlink(x);
						return true;
					}
				}
			}
			else
			{
				for (Node<E> x = Last_Renamed; x != null; x = x.Prev)
				{
					if (o.Equals(x.Item))
					{
						Unlink(x);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a list-iterator of the elements in this list (in proper
		/// sequence), starting at the specified position in the list.
		/// Obeys the general contract of {@code List.listIterator(int)}.<para>
		/// 
		/// The list-iterator is <i>fail-fast</i>: if the list is structurally
		/// modified at any time after the Iterator is created, in any way except
		/// through the list-iterator's own {@code remove} or {@code add}
		/// methods, the list-iterator will throw a
		/// {@code ConcurrentModificationException}.  Thus, in the face of
		/// concurrent modification, the iterator fails quickly and cleanly, rather
		/// than risking arbitrary, non-deterministic behavior at an undetermined
		/// time in the future.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> index of the first element to be returned from the
		///              list-iterator (by a call to {@code next}) </param>
		/// <returns> a ListIterator of the elements in this list (in proper
		///         sequence), starting at the specified position in the list </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <seealso cref= List#listIterator(int) </seealso>
		public virtual ListIterator<E> ListIterator(int index)
		{
			CheckPositionIndex(index);
			return new ListItr(this, this, index);
		}

		private class ListItr : ListIterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly LinkedList<E> OuterInstance;

			internal Node<E> LastReturned;
			internal Node<E> Next_Renamed;
			internal int NextIndex_Renamed;
			internal int ExpectedModCount;

			internal ListItr(LinkedList<E> outerInstance, int index)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				// assert isPositionIndex(index);
				Next_Renamed = (index == outerInstance.Size_Renamed) ? null : outerInstance.Node(index);
				NextIndex_Renamed = index;
			}

			public virtual bool HasNext()
			{
				return NextIndex_Renamed < outerInstance.Size_Renamed;
			}

			public virtual E Next()
			{
				CheckForComodification();
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}

				LastReturned = Next_Renamed;
				Next_Renamed = Next_Renamed.Next;
				NextIndex_Renamed++;
				return LastReturned.Item;
			}

			public virtual bool HasPrevious()
			{
				return NextIndex_Renamed > 0;
			}

			public virtual E Previous()
			{
				CheckForComodification();
				if (!HasPrevious())
				{
					throw new NoSuchElementException();
				}

				LastReturned = Next_Renamed = (Next_Renamed == null) ? outerInstance.Last_Renamed : Next_Renamed.Prev;
				NextIndex_Renamed--;
				return LastReturned.Item;
			}

			public virtual int NextIndex()
			{
				return NextIndex_Renamed;
			}

			public virtual int PreviousIndex()
			{
				return NextIndex_Renamed - 1;
			}

			public virtual void Remove()
			{
				CheckForComodification();
				if (LastReturned == null)
				{
					throw new IllegalStateException();
				}

				Node<E> lastNext = LastReturned.Next;
				outerInstance.Unlink(LastReturned);
				if (Next_Renamed == LastReturned)
				{
					Next_Renamed = lastNext;
				}
				else
				{
					NextIndex_Renamed--;
				}
				LastReturned = null;
				ExpectedModCount++;
			}

			public virtual void Set(E e)
			{
				if (LastReturned == null)
				{
					throw new IllegalStateException();
				}
				CheckForComodification();
				LastReturned.Item = e;
			}

			public virtual void Add(E e)
			{
				CheckForComodification();
				LastReturned = null;
				if (Next_Renamed == null)
				{
					outerInstance.LinkLast(e);
				}
				else
				{
					outerInstance.LinkBefore(e, Next_Renamed);
				}
				NextIndex_Renamed++;
				ExpectedModCount++;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public virtual void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				while (outerInstance.ModCount == ExpectedModCount && NextIndex_Renamed < outerInstance.Size_Renamed)
				{
					action.Accept(Next_Renamed.Item);
					LastReturned = Next_Renamed;
					Next_Renamed = Next_Renamed.Next;
					NextIndex_Renamed++;
				}
				CheckForComodification();
			}

			internal void CheckForComodification()
			{
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		private class Node<E>
		{
			internal E Item;
			internal Node<E> Next;
			internal Node<E> Prev;

			internal Node(Node<E> prev, E element, Node<E> next)
			{
				this.Item = element;
				this.Next = next;
				this.Prev = prev;
			}
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual Iterator<E> DescendingIterator()
		{
			return new DescendingIterator(this);
		}

		/// <summary>
		/// Adapter to provide descending iterators via ListItr.previous
		/// </summary>
		private class DescendingIterator : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				Itr = new ListItr(OuterInstance, outerInstance.Size());
			}

			private readonly LinkedList<E> OuterInstance;

			public DescendingIterator(LinkedList<E> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			internal ListItr Itr;
			public virtual bool HasNext()
			{
				return Itr.HasPrevious();
			}
			public virtual E Next()
			{
				return Itr.Previous();
			}
			public virtual void Remove()
			{
				Itr.Remove();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private LinkedList<E> superClone()
		private LinkedList<E> SuperClone()
		{
			try
			{
				return (LinkedList<E>) base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Returns a shallow copy of this {@code LinkedList}. (The elements
		/// themselves are not cloned.)
		/// </summary>
		/// <returns> a shallow copy of this {@code LinkedList} instance </returns>
		public virtual Object Clone()
		{
			LinkedList<E> clone = SuperClone();

			// Put clone into "virgin" state
			clone.First_Renamed = clone.Last_Renamed = null;
			clone.Size_Renamed = 0;
			clone.ModCount = 0;

			// Initialize clone with our elements
			for (Node<E> x = First_Renamed; x != null; x = x.Next)
			{
				clone.Add(x.Item);
			}

			return clone;
		}

		/// <summary>
		/// Returns an array containing all of the elements in this list
		/// in proper sequence (from first to last element).
		/// 
		/// <para>The returned array will be "safe" in that no references to it are
		/// maintained by this list.  (In other words, this method must allocate
		/// a new array).  The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all of the elements in this list
		///         in proper sequence </returns>
		public virtual Object[] ToArray()
		{
			Object[] result = new Object[Size_Renamed];
			int List_Fields.i = 0;
			for (Node<E> x = First_Renamed; x != null; x = x.Next)
			{
				result[List_Fields.i++] = x.Item;
			}
			return result;
		}

		/// <summary>
		/// Returns an array containing all of the elements in this list in
		/// proper sequence (from first to last element); the runtime type of
		/// the returned array is that of the specified array.  If the list fits
		/// in the specified array, it is returned therein.  Otherwise, a new
		/// array is allocated with the runtime type of the specified array and
		/// the size of this list.
		/// 
		/// <para>If the list fits in the specified array with room to spare (i.e.,
		/// the array has more elements than the list), the element in the array
		/// immediately following the end of the list is set to {@code null}.
		/// (This is useful in determining the length of the list <i>only</i> if
		/// the caller knows that the list does not contain any null elements.)
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose {@code x} is a list known to contain only strings.
		/// The following code can be used to dump the list into a newly
		/// allocated array of {@code String}:
		/// 
		/// <pre>
		///     String[] y = x.toArray(new String[0]);</pre>
		/// 
		/// Note that {@code toArray(new Object[0])} is identical in function to
		/// {@code toArray()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of the list are to
		///          be stored, if it is big enough; otherwise, a new array of the
		///          same runtime type is allocated for this purpose. </param>
		/// <returns> an array containing the elements of the list </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this list </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] List_Fields.a)
		public virtual T[] toArray<T>(T[] List_Fields)
		{
			if (List_Fields.a.Length < Size_Renamed)
			{
				List_Fields.a = (T[])java.lang.reflect.Array.NewInstance(List_Fields.a.GetType().GetElementType(), Size_Renamed);
			}
			int List_Fields.i = 0;
			Object[] result = List_Fields.a;
			for (Node<E> x = First_Renamed; x != null; x = x.Next)
			{
				result[List_Fields.i++] = x.Item;
			}

			if (List_Fields.a.Length > Size_Renamed)
			{
				List_Fields.a[Size_Renamed] = null;
			}

			return List_Fields.a;
		}

		private const long SerialVersionUID = 876323262645176354L;

		/// <summary>
		/// Saves the state of this {@code LinkedList} instance to a stream
		/// (that is, serializes it).
		/// 
		/// @serialData The size of the list (the number of elements it
		///             contains) is emitted (int), followed by all of its
		///             elements (each an Object) in the proper order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out any hidden serialization magic
			s.DefaultWriteObject();

			// Write out size
			s.WriteInt(Size_Renamed);

			// Write out all elements in the proper order.
			for (Node<E> x = First_Renamed; x != null; x = x.Next)
			{
				s.WriteObject(x.Item);
			}
		}

		/// <summary>
		/// Reconstitutes this {@code LinkedList} instance from a stream
		/// (that is, deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in any hidden serialization magic
			s.DefaultReadObject();

			// Read in size
			int size = s.ReadInt();

			// Read in all elements in the proper order.
			for (int List_Fields.i = 0; List_Fields.i < size; List_Fields.i++)
			{
				LinkLast((E)s.ReadObject());
			}
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// list.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#ORDERED"/>.  Overriding implementations should document
		/// the reporting of additional characteristic values.
		/// 
		/// @implNote
		/// The {@code Spliterator} additionally reports <seealso cref="Spliterator#SUBSIZED"/>
		/// and implements {@code trySplit} to permit limited parallelism..
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this list
		/// @since 1.8 </returns>
		public override Spliterator<E> Spliterator()
		{
			return new LLSpliterator<E>(this, -1, 0);
		}

		/// <summary>
		/// A customized variant of Spliterators.IteratorSpliterator </summary>
		internal sealed class LLSpliterator<E> : Spliterator<E>
		{
			internal static readonly int BATCH_UNIT = 1 << 10; // batch array size increment
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly LinkedList<E> List; // null OK unless traversed
			internal Node<E> Current; // current node; null until initialized
			internal int Est_Renamed; // size estimate; -1 until first needed
			internal int ExpectedModCount; // initialized when est set
			internal int Batch; // batch size for splits

			internal LLSpliterator(LinkedList<E> list, int est, int expectedModCount)
			{
				this.List = list;
				this.Est_Renamed = est;
				this.ExpectedModCount = expectedModCount;
			}

			internal int Est
			{
				get
				{
					int s; // force initialization
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final LinkedList<E> lst;
					LinkedList<E> lst;
					if ((s = Est_Renamed) < 0)
					{
						if ((lst = List) == null)
						{
							s = Est_Renamed = 0;
						}
						else
						{
							ExpectedModCount = lst.ModCount;
							Current = lst.First_Renamed;
							s = Est_Renamed = lst.Size_Renamed;
						}
					}
					return s;
				}
			}

			public long EstimateSize()
			{
				return (long) Est;
			}

			public Spliterator<E> TrySplit()
			{
				Node<E> p;
				int s = Est;
				if (s > 1 && (p = Current) != null)
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					Object[] List_Fields.a = new Object[n];
					int j = 0;
					do
					{
						List_Fields.a[j++] = p.Item;
					} while ((p = p.Next) != null && j < n);
					Current = p;
					Batch = j;
					Est_Renamed = s - j;
					return Spliterators.Spliterator(List_Fields.a, 0, j, Spliterator_Fields.ORDERED);
				}
				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				Node<E> p;
				int n;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((n = Est) > 0 && (p = Current) != null)
				{
					Current = null;
					Est_Renamed = 0;
					do
					{
						E e = p.Item;
						p = p.Next;
						action.Accept(e);
					} while (p != null && --n > 0);
				}
				if (List.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
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
				if (Est > 0 && (p = Current) != null)
				{
					--Est_Renamed;
					E e = p.Item;
					Current = p.Next;
					action.Accept(e);
					if (List.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					return true;
				}
				return false;
			}

			public int Characteristics()
			{
				return Spliterator_Fields.ORDERED | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}
		}

	}

}