using System;
using System.Collections;
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
	/// Resizable-array implementation of the <tt>List</tt> interface.  Implements
	/// all optional list operations, and permits all elements, including
	/// <tt>null</tt>.  In addition to implementing the <tt>List</tt> interface,
	/// this class provides methods to manipulate the size of the array that is
	/// used internally to store the list.  (This class is roughly equivalent to
	/// <tt>Vector</tt>, except that it is unsynchronized.)
	/// 
	/// <para>The <tt>size</tt>, <tt>isEmpty</tt>, <tt>get</tt>, <tt>set</tt>,
	/// <tt>iterator</tt>, and <tt>listIterator</tt> operations run in constant
	/// time.  The <tt>add</tt> operation runs in <i>amortized constant time</i>,
	/// that is, adding n elements requires O(n) time.  All of the other operations
	/// run in linear time (roughly speaking).  The constant factor is low compared
	/// to that for the <tt>LinkedList</tt> implementation.
	/// 
	/// </para>
	/// <para>Each <tt>ArrayList</tt> instance has a <i>capacity</i>.  The capacity is
	/// the size of the array used to store the elements in the list.  It is always
	/// at least as large as the list size.  As elements are added to an ArrayList,
	/// its capacity grows automatically.  The details of the growth policy are not
	/// specified beyond the fact that adding an element has constant amortized
	/// time cost.
	/// 
	/// </para>
	/// <para>An application can increase the capacity of an <tt>ArrayList</tt> instance
	/// before adding a large number of elements using the <tt>ensureCapacity</tt>
	/// operation.  This may reduce the amount of incremental reallocation.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access an <tt>ArrayList</tt> instance concurrently,
	/// and at least one of the threads modifies the list structurally, it
	/// <i>must</i> be synchronized externally.  (A structural modification is
	/// any operation that adds or deletes one or more elements, or explicitly
	/// resizes the backing array; merely setting the value of an element is not
	/// a structural modification.)  This is typically accomplished by
	/// synchronizing on some object that naturally encapsulates the list.
	/// 
	/// If no such object exists, the list should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedList Collections.synchronizedList"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the list:<pre>
	///   List list = Collections.synchronizedList(new ArrayList(...));</pre>
	/// 
	/// </para>
	/// <para><a name="fail-fast">
	/// The iterators returned by this class's <seealso cref="#iterator() iterator"/> and
	/// <seealso cref="#listIterator(int) listIterator"/> methods are <em>fail-fast</em>:</a>
	/// if the list is structurally modified at any time after the iterator is
	/// created, in any way except through the iterator's own
	/// <seealso cref="ListIterator#remove() remove"/> or
	/// <seealso cref="ListIterator#add(Object) add"/> methods, the iterator will throw a
	/// <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of
	/// concurrent modification, the iterator fails quickly and cleanly, rather
	/// than risking arbitrary, non-deterministic behavior at an undetermined
	/// time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness:  <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// </para>
	/// </summary>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     List </seealso>
	/// <seealso cref=     LinkedList </seealso>
	/// <seealso cref=     Vector
	/// @since   1.2 </seealso>

	[Serializable]
	public class List<E> : AbstractList<E>, List<E>, RandomAccess, Cloneable
	{
		private const long SerialVersionUID = 8683452581122892189L;

		/// <summary>
		/// Default initial capacity.
		/// </summary>
		private const int DEFAULT_CAPACITY = 10;

		/// <summary>
		/// Shared empty array instance used for empty instances.
		/// </summary>
		private static readonly Object[] EMPTY_ELEMENTDATA = new Object[] {};

		/// <summary>
		/// Shared empty array instance used for default sized empty instances. We
		/// distinguish this from EMPTY_ELEMENTDATA to know how much to inflate when
		/// first element is added.
		/// </summary>
		private static readonly Object[] DEFAULTCAPACITY_EMPTY_ELEMENTDATA = new Object[] {};

		/// <summary>
		/// The array buffer into which the elements of the ArrayList are stored.
		/// The capacity of the ArrayList is the length of this array buffer. Any
		/// empty ArrayList with elementData == DEFAULTCAPACITY_EMPTY_ELEMENTDATA
		/// will be expanded to DEFAULT_CAPACITY when the first element is added.
		/// </summary>
		[NonSerialized]
		internal Object[] ElementData_Renamed; // non-private to simplify nested class access

		/// <summary>
		/// The size of the ArrayList (the number of elements it contains).
		/// 
		/// @serial
		/// </summary>
		private int Size_Renamed;

		/// <summary>
		/// Constructs an empty list with the specified initial capacity.
		/// </summary>
		/// <param name="initialCapacity">  the initial capacity of the list </param>
		/// <exception cref="IllegalArgumentException"> if the specified initial capacity
		///         is negative </exception>
		public ArrayList(int initialCapacity)
		{
			if (initialCapacity > 0)
			{
				this.ElementData_Renamed = new Object[initialCapacity];
			}
			else if (initialCapacity == 0)
			{
				this.ElementData_Renamed = EMPTY_ELEMENTDATA;
			}
			else
			{
				throw new IllegalArgumentException("Illegal Capacity: " + initialCapacity);
			}
		}

		/// <summary>
		/// Constructs an empty list with an initial capacity of ten.
		/// </summary>
		public ArrayList()
		{
			this.ElementData_Renamed = DEFAULTCAPACITY_EMPTY_ELEMENTDATA;
		}

		/// <summary>
		/// Constructs a list containing the elements of the specified
		/// collection, in the order they are returned by the collection's
		/// iterator.
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed into this list </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public List<T1>(Collection<T1> c) where T1 : E
		{
			ElementData_Renamed = c.ToArray();
			if ((Size_Renamed = ElementData_Renamed.Length) != 0)
			{
				// c.toArray might (incorrectly) not return Object[] (see 6260652)
				if (ElementData_Renamed.GetType() != typeof(Object[]))
				{
					ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, Size_Renamed, typeof(Object[]));
				}
			}
			else
			{
				// replace with empty array.
				this.ElementData_Renamed = EMPTY_ELEMENTDATA;
			}
		}

		/// <summary>
		/// Trims the capacity of this <tt>ArrayList</tt> instance to be the
		/// list's current size.  An application can use this operation to minimize
		/// the storage of an <tt>ArrayList</tt> instance.
		/// </summary>
		public virtual void TrimToSize()
		{
			ModCount++;
			if (Size_Renamed < ElementData_Renamed.Length)
			{
				ElementData_Renamed = (Size_Renamed == 0) ? EMPTY_ELEMENTDATA : Arrays.CopyOf(ElementData_Renamed, Size_Renamed);
			}
		}

		/// <summary>
		/// Increases the capacity of this <tt>ArrayList</tt> instance, if
		/// necessary, to ensure that it can hold at least the number of elements
		/// specified by the minimum capacity argument.
		/// </summary>
		/// <param name="minCapacity">   the desired minimum capacity </param>
		public virtual void EnsureCapacity(int minCapacity)
		{
			int minExpand = (ElementData_Renamed != DEFAULTCAPACITY_EMPTY_ELEMENTDATA) ? 0 : DEFAULT_CAPACITY;
				// any size if not default element table
				// larger than default for default empty table. It's already
				// supposed to be at default size.

			if (minCapacity > minExpand)
			{
				EnsureExplicitCapacity(minCapacity);
			}
		}

		private void EnsureCapacityInternal(int minCapacity)
		{
			if (ElementData_Renamed == DEFAULTCAPACITY_EMPTY_ELEMENTDATA)
			{
				minCapacity = System.Math.Max(DEFAULT_CAPACITY, minCapacity);
			}

			EnsureExplicitCapacity(minCapacity);
		}

		private void EnsureExplicitCapacity(int minCapacity)
		{
			ModCount++;

			// overflow-conscious code
			if (minCapacity - ElementData_Renamed.Length > 0)
			{
				Grow(minCapacity);
			}
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Increases the capacity to ensure that it can hold at least the
		/// number of elements specified by the minimum capacity argument.
		/// </summary>
		/// <param name="minCapacity"> the desired minimum capacity </param>
		private void Grow(int minCapacity)
		{
			// overflow-conscious code
			int oldCapacity = ElementData_Renamed.Length;
			int newCapacity = oldCapacity + (oldCapacity >> 1);
			if (newCapacity - minCapacity < 0)
			{
				newCapacity = minCapacity;
			}
			if (newCapacity - MAX_ARRAY_SIZE > 0)
			{
				newCapacity = HugeCapacity(minCapacity);
			}
			// minCapacity is usually close to size, so this is a win:
			ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, newCapacity);
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
		/// Returns <tt>true</tt> if this list contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this list contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Size_Renamed == 0;
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> if this list contains the specified element.
		/// More formally, returns <tt>true</tt> if and only if this list contains
		/// at least one element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this list is to be tested </param>
		/// <returns> <tt>true</tt> if this list contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			return IndexOf(o) >= 0;
		}

		/// <summary>
		/// Returns the index of the first occurrence of the specified element
		/// in this list, or -1 if this list does not contain the element.
		/// More formally, returns the lowest index <tt>i</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		public virtual int IndexOf(Object o)
		{
			if (o == null)
			{
				for (int List_Fields.i = 0; List_Fields.i < Size_Renamed; List_Fields.i++)
				{
					if (ElementData_Renamed[List_Fields.i] == null)
					{
						return List_Fields.i;
					}
				}
			}
			else
			{
				for (int List_Fields.i = 0; List_Fields.i < Size_Renamed; List_Fields.i++)
				{
					if (o.Equals(ElementData_Renamed[List_Fields.i]))
					{
						return List_Fields.i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the last occurrence of the specified element
		/// in this list, or -1 if this list does not contain the element.
		/// More formally, returns the highest index <tt>i</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		public virtual int LastIndexOf(Object o)
		{
			if (o == null)
			{
				for (int List_Fields.i = Size_Renamed - 1; List_Fields.i >= 0; List_Fields.i--)
				{
					if (ElementData_Renamed[List_Fields.i] == null)
					{
						return List_Fields.i;
					}
				}
			}
			else
			{
				for (int List_Fields.i = Size_Renamed - 1; List_Fields.i >= 0; List_Fields.i--)
				{
					if (o.Equals(ElementData_Renamed[List_Fields.i]))
					{
						return List_Fields.i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns a shallow copy of this <tt>ArrayList</tt> instance.  (The
		/// elements themselves are not copied.)
		/// </summary>
		/// <returns> a clone of this <tt>ArrayList</tt> instance </returns>
		public virtual Object Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ArrayList<?> v = (ArrayList<?>) base.clone();
				List<?> v = (List<?>) base.Clone();
				v.ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, Size_Renamed);
				v.ModCount = 0;
				return v;
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
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
		/// <returns> an array containing all of the elements in this list in
		///         proper sequence </returns>
		public virtual Object[] ToArray()
		{
			return Arrays.CopyOf(ElementData_Renamed, Size_Renamed);
		}

		/// <summary>
		/// Returns an array containing all of the elements in this list in proper
		/// sequence (from first to last element); the runtime type of the returned
		/// array is that of the specified array.  If the list fits in the
		/// specified array, it is returned therein.  Otherwise, a new array is
		/// allocated with the runtime type of the specified array and the size of
		/// this list.
		/// 
		/// <para>If the list fits in the specified array with room to spare
		/// (i.e., the array has more elements than the list), the element in
		/// the array immediately following the end of the collection is set to
		/// <tt>null</tt>.  (This is useful in determining the length of the
		/// list <i>only</i> if the caller knows that the list does not contain
		/// any null elements.)
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
				// Make a new array of a's runtime type, but my contents:
				return (T[]) Arrays.CopyOf(ElementData_Renamed, Size_Renamed, List_Fields.a.GetType());
			}
			System.Array.Copy(ElementData_Renamed, 0, List_Fields.a, 0, Size_Renamed);
			if (List_Fields.a.Length > Size_Renamed)
			{
				List_Fields.a[Size_Renamed] = null;
			}
			return List_Fields.a;
		}

		// Positional Access Operations

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E elementData(int index)
		internal virtual E ElementData(int index)
		{
			return (E) ElementData_Renamed[index];
		}

		/// <summary>
		/// Returns the element at the specified position in this list.
		/// </summary>
		/// <param name="index"> index of the element to return </param>
		/// <returns> the element at the specified position in this list </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Get(int index)
		{
			RangeCheck(index);

			return ElementData(index);
		}

		/// <summary>
		/// Replaces the element at the specified position in this list with
		/// the specified element.
		/// </summary>
		/// <param name="index"> index of the element to replace </param>
		/// <param name="element"> element to be stored at the specified position </param>
		/// <returns> the element previously at the specified position </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Set(int index, E element)
		{
			RangeCheck(index);

			E oldValue = ElementData(index);
			ElementData_Renamed[index] = element;
			return oldValue;
		}

		/// <summary>
		/// Appends the specified element to the end of this list.
		/// </summary>
		/// <param name="e"> element to be appended to this list </param>
		/// <returns> <tt>true</tt> (as specified by <seealso cref="Collection#add"/>) </returns>
		public virtual bool Add(E e)
		{
			EnsureCapacityInternal(Size_Renamed + 1); // Increments modCount!!
			ElementData_Renamed[Size_Renamed++] = e;
			return true;
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this
		/// list. Shifts the element currently at that position (if any) and
		/// any subsequent elements to the right (adds one to their indices).
		/// </summary>
		/// <param name="index"> index at which the specified element is to be inserted </param>
		/// <param name="element"> element to be inserted </param>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual void Add(int index, E element)
		{
			RangeCheckForAdd(index);

			EnsureCapacityInternal(Size_Renamed + 1); // Increments modCount!!
			System.Array.Copy(ElementData_Renamed, index, ElementData_Renamed, index + 1, Size_Renamed - index);
			ElementData_Renamed[index] = element;
			Size_Renamed++;
		}

		/// <summary>
		/// Removes the element at the specified position in this list.
		/// Shifts any subsequent elements to the left (subtracts one from their
		/// indices).
		/// </summary>
		/// <param name="index"> the index of the element to be removed </param>
		/// <returns> the element that was removed from the list </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Remove(int index)
		{
			RangeCheck(index);

			ModCount++;
			E oldValue = ElementData(index);

			int numMoved = Size_Renamed - index - 1;
			if (numMoved > 0)
			{
				System.Array.Copy(ElementData_Renamed, index + 1, ElementData_Renamed, index, numMoved);
			}
			ElementData_Renamed[--Size_Renamed] = null; // clear to let GC do its work

			return oldValue;
		}

		/// <summary>
		/// Removes the first occurrence of the specified element from this list,
		/// if it is present.  If the list does not contain the element, it is
		/// unchanged.  More formally, removes the element with the lowest index
		/// <tt>i</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>
		/// (if such an element exists).  Returns <tt>true</tt> if this list
		/// contained the specified element (or equivalently, if this list
		/// changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this list, if present </param>
		/// <returns> <tt>true</tt> if this list contained the specified element </returns>
		public virtual bool Remove(Object o)
		{
			if (o == null)
			{
				for (int index = 0; index < Size_Renamed; index++)
				{
					if (ElementData_Renamed[index] == null)
					{
						FastRemove(index);
						return true;
					}
				}
			}
			else
			{
				for (int index = 0; index < Size_Renamed; index++)
				{
					if (o.Equals(ElementData_Renamed[index]))
					{
						FastRemove(index);
						return true;
					}
				}
			}
			return false;
		}

		/*
		 * Private remove method that skips bounds checking and does not
		 * return the value removed.
		 */
		private void FastRemove(int index)
		{
			ModCount++;
			int numMoved = Size_Renamed - index - 1;
			if (numMoved > 0)
			{
				System.Array.Copy(ElementData_Renamed, index + 1, ElementData_Renamed, index, numMoved);
			}
			ElementData_Renamed[--Size_Renamed] = null; // clear to let GC do its work
		}

		/// <summary>
		/// Removes all of the elements from this list.  The list will
		/// be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			ModCount++;

			// clear to let GC do its work
			for (int List_Fields.i = 0; List_Fields.i < Size_Renamed; List_Fields.i++)
			{
				ElementData_Renamed[List_Fields.i] = null;
			}

			Size_Renamed = 0;
		}

		/// <summary>
		/// Appends all of the elements in the specified collection to the end of
		/// this list, in the order that they are returned by the
		/// specified collection's Iterator.  The behavior of this operation is
		/// undefined if the specified collection is modified while the operation
		/// is in progress.  (This implies that the behavior of this call is
		/// undefined if the specified collection is this list, and this
		/// list is nonempty.)
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> <tt>true</tt> if this list changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			Object[] List_Fields.a = c.ToArray();
			int numNew = List_Fields.a.Length;
			EnsureCapacityInternal(Size_Renamed + numNew); // Increments modCount
			System.Array.Copy(List_Fields.a, 0, ElementData_Renamed, Size_Renamed, numNew);
			Size_Renamed += numNew;
			return numNew != 0;
		}

		/// <summary>
		/// Inserts all of the elements in the specified collection into this
		/// list, starting at the specified position.  Shifts the element
		/// currently at that position (if any) and any subsequent elements to
		/// the right (increases their indices).  The new elements will appear
		/// in the list in the order that they are returned by the
		/// specified collection's iterator.
		/// </summary>
		/// <param name="index"> index at which to insert the first element from the
		///              specified collection </param>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> <tt>true</tt> if this list changed as a result of the call </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool addAll<T1>(int index, Collection<T1> c) where T1 : E
		{
			RangeCheckForAdd(index);

			Object[] List_Fields.a = c.ToArray();
			int numNew = List_Fields.a.Length;
			EnsureCapacityInternal(Size_Renamed + numNew); // Increments modCount

			int numMoved = Size_Renamed - index;
			if (numMoved > 0)
			{
				System.Array.Copy(ElementData_Renamed, index, ElementData_Renamed, index + numNew, numMoved);
			}

			System.Array.Copy(List_Fields.a, 0, ElementData_Renamed, index, numNew);
			Size_Renamed += numNew;
			return numNew != 0;
		}

		/// <summary>
		/// Removes from this list all of the elements whose index is between
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		/// Shifts any succeeding elements to the left (reduces their index).
		/// This call shortens the list by {@code (toIndex - fromIndex)} elements.
		/// (If {@code toIndex==fromIndex}, this operation has no effect.)
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} or
		///         {@code toIndex} is out of range
		///         ({@code fromIndex < 0 ||
		///          fromIndex >= size() ||
		///          toIndex > size() ||
		///          toIndex < fromIndex}) </exception>
		protected internal virtual void RemoveRange(int fromIndex, int toIndex)
		{
			ModCount++;
			int numMoved = Size_Renamed - toIndex;
			System.Array.Copy(ElementData_Renamed, toIndex, ElementData_Renamed, fromIndex, numMoved);

			// clear to let GC do its work
			int newSize = Size_Renamed - (toIndex - fromIndex);
			for (int List_Fields.i = newSize; List_Fields.i < Size_Renamed; List_Fields.i++)
			{
				ElementData_Renamed[List_Fields.i] = null;
			}
			Size_Renamed = newSize;
		}

		/// <summary>
		/// Checks if the given index is in range.  If not, throws an appropriate
		/// runtime exception.  This method does *not* check if the index is
		/// negative: It is always used immediately prior to an array access,
		/// which throws an ArrayIndexOutOfBoundsException if index is negative.
		/// </summary>
		private void RangeCheck(int index)
		{
			if (index >= Size_Renamed)
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		/// <summary>
		/// A version of rangeCheck used by add and addAll.
		/// </summary>
		private void RangeCheckForAdd(int index)
		{
			if (index > Size_Renamed || index < 0)
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
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

		/// <summary>
		/// Removes from this list all of its elements that are contained in the
		/// specified collection.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this list
		///         is incompatible with the specified collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this list contains a null element and the
		///         specified collection does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= Collection#contains(Object) </seealso>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			Objects.RequireNonNull(c);
			return BatchRemove(c, false);
		}

		/// <summary>
		/// Retains only the elements in this list that are contained in the
		/// specified collection.  In other words, removes from this list all
		/// of its elements that are not contained in the specified collection.
		/// </summary>
		/// <param name="c"> collection containing elements to be retained in this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this list
		///         is incompatible with the specified collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this list contains a null element and the
		///         specified collection does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= Collection#contains(Object) </seealso>
		public virtual bool retainAll<T1>(Collection<T1> c)
		{
			Objects.RequireNonNull(c);
			return BatchRemove(c, true);
		}

		private bool batchRemove<T1>(Collection<T1> c, bool complement)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] elementData = this.elementData;
			Object[] elementData = this.ElementData_Renamed;
			int r = 0, w = 0;
			bool modified = false;
			try
			{
				for (; r < Size_Renamed; r++)
				{
					if (c.Contains(elementData[r]) == complement)
					{
						elementData[w++] = elementData[r];
					}
				}
			}
			finally
			{
				// Preserve behavioral compatibility with AbstractCollection,
				// even if c.contains() throws.
				if (r != Size_Renamed)
				{
					System.Array.Copy(elementData, r, elementData, w, Size_Renamed - r);
					w += Size_Renamed - r;
				}
				if (w != Size_Renamed)
				{
					// clear to let GC do its work
					for (int List_Fields.i = w; List_Fields.i < Size_Renamed; List_Fields.i++)
					{
						elementData[List_Fields.i] = null;
					}
					ModCount += Size_Renamed - w;
					Size_Renamed = w;
					modified = true;
				}
			}
			return modified;
		}

		/// <summary>
		/// Save the state of the <tt>ArrayList</tt> instance to a stream (that
		/// is, serialize it).
		/// 
		/// @serialData The length of the array backing the <tt>ArrayList</tt>
		///             instance is emitted (int), followed by all of its elements
		///             (each an <tt>Object</tt>) in the proper order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out element count, and any hidden stuff
			int expectedModCount = ModCount;
			s.DefaultWriteObject();

			// Write out size as capacity for behavioural compatibility with clone()
			s.WriteInt(Size_Renamed);

			// Write out all elements in the proper order.
			for (int List_Fields.i = 0; List_Fields.i < Size_Renamed; List_Fields.i++)
			{
				s.WriteObject(ElementData_Renamed[List_Fields.i]);
			}

			if (ModCount != expectedModCount)
			{
				throw new ConcurrentModificationException();
			}
		}

		/// <summary>
		/// Reconstitute the <tt>ArrayList</tt> instance from a stream (that is,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			ElementData_Renamed = EMPTY_ELEMENTDATA;

			// Read in size, and any hidden stuff
			s.DefaultReadObject();

			// Read in capacity
			s.ReadInt(); // ignored

			if (Size_Renamed > 0)
			{
				// be like clone(), allocate array based upon size not capacity
				EnsureCapacityInternal(Size_Renamed);

				Object[] List_Fields.a = ElementData_Renamed;
				// Read in all elements in the proper order.
				for (int List_Fields.i = 0; List_Fields.i < Size_Renamed; List_Fields.i++)
				{
					List_Fields.a[List_Fields.i] = s.ReadObject();
				}
			}
		}

		/// <summary>
		/// Returns a list iterator over the elements in this list (in proper
		/// sequence), starting at the specified position in the list.
		/// The specified index indicates the first element that would be
		/// returned by an initial call to <seealso cref="ListIterator#next next"/>.
		/// An initial call to <seealso cref="ListIterator#previous previous"/> would
		/// return the element with the specified index minus one.
		/// 
		/// <para>The returned list iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual ListIterator<E> ListIterator(int index)
		{
			if (index < 0 || index > Size_Renamed)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
			return new ListItr(this, this, index);
		}

		/// <summary>
		/// Returns a list iterator over the elements in this list (in proper
		/// sequence).
		/// 
		/// <para>The returned list iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #listIterator(int) </seealso>
		public virtual ListIterator<E> ListIterator()
		{
			return new ListItr(this, this, 0);
		}

		/// <summary>
		/// Returns an iterator over the elements in this list in proper sequence.
		/// 
		/// <para>The returned iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this list in proper sequence </returns>
		public virtual Iterator<E> GetEnumerator()
		{
			return new Itr(this, this);
		}

		/// <summary>
		/// An optimized version of AbstractList.Itr
		/// </summary>
		private class Itr : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly List<E> OuterInstance;

			public Itr(List<E> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			internal int Cursor; // index of next element to return
			internal int LastRet = -1; // index of last element returned; -1 if no such
			internal int ExpectedModCount;

			public virtual bool HasNext()
			{
				return Cursor != outerInstance.Size_Renamed;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
			public virtual E Next()
			{
				CheckForComodification();
				int List_Fields.i = Cursor;
				if (List_Fields.i >= outerInstance.Size_Renamed)
				{
					throw new NoSuchElementException();
				}
				Object[] elementData = OuterInstance.ElementData_Renamed;
				if (List_Fields.i >= elementData.Length)
				{
					throw new ConcurrentModificationException();
				}
				Cursor = List_Fields.i + 1;
				return (E) elementData[LastRet = List_Fields.i];
			}

			public virtual void Remove()
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				CheckForComodification();

				try
				{
					OuterInstance.Remove(LastRet);
					Cursor = LastRet;
					LastRet = -1;
					ExpectedModCount = outerInstance.ModCount;
				}
				catch (IndexOutOfBoundsException)
				{
					throw new ConcurrentModificationException();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> consumer)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public override void forEachRemaining<T1>(Consumer<T1> consumer)
			{
				Objects.RequireNonNull(consumer);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = ArrayList.this.size;
				int size = OuterInstance.Size_Renamed;
				int List_Fields.i = Cursor;
				if (List_Fields.i >= size)
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] elementData = ArrayList.this.elementData;
				Object[] elementData = OuterInstance.ElementData_Renamed;
				if (List_Fields.i >= elementData.Length)
				{
					throw new ConcurrentModificationException();
				}
				while (List_Fields.i != size && outerInstance.ModCount == ExpectedModCount)
				{
					consumer.Accept((E) elementData[List_Fields.i++]);
				}
				// update once at end of iteration to reduce heap write traffic
				Cursor = List_Fields.i;
				LastRet = List_Fields.i - 1;
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

		/// <summary>
		/// An optimized version of AbstractList.ListItr
		/// </summary>
		private class ListItr : Itr, ListIterator<E>
		{
			private readonly List<E> OuterInstance;

			internal ListItr(List<E> outerInstance, int index) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				Cursor = index;
			}

			public virtual bool HasPrevious()
			{
				return Cursor != 0;
			}

			public virtual int NextIndex()
			{
				return Cursor;
			}

			public virtual int PreviousIndex()
			{
				return Cursor - 1;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E previous()
			public virtual E Previous()
			{
				CheckForComodification();
				int List_Fields.i = Cursor - 1;
				if (List_Fields.i < 0)
				{
					throw new NoSuchElementException();
				}
				Object[] elementData = OuterInstance.ElementData_Renamed;
				if (List_Fields.i >= elementData.Length)
				{
					throw new ConcurrentModificationException();
				}
				Cursor = List_Fields.i;
				return (E) elementData[LastRet = List_Fields.i];
			}

			public virtual void Set(E e)
			{
				if (LastRet < 0)
				{
					throw new IllegalStateException();
				}
				CheckForComodification();

				try
				{
					OuterInstance.Set(LastRet, e);
				}
				catch (IndexOutOfBoundsException)
				{
					throw new ConcurrentModificationException();
				}
			}

			public virtual void Add(E e)
			{
				CheckForComodification();

				try
				{
					int List_Fields.i = Cursor;
					OuterInstance.Add(List_Fields.i, e);
					Cursor = List_Fields.i + 1;
					LastRet = -1;
					ExpectedModCount = outerInstance.ModCount;
				}
				catch (IndexOutOfBoundsException)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		/// <summary>
		/// Returns a view of the portion of this list between the specified
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.  (If
		/// {@code fromIndex} and {@code toIndex} are equal, the returned list is
		/// empty.)  The returned list is backed by this list, so non-structural
		/// changes in the returned list are reflected in this list, and vice-versa.
		/// The returned list supports all of the optional list operations.
		/// 
		/// <para>This method eliminates the need for explicit range operations (of
		/// the sort that commonly exist for arrays).  Any operation that expects
		/// a list can be used as a range operation by passing a subList view
		/// instead of a whole list.  For example, the following idiom
		/// removes a range of elements from a list:
		/// <pre>
		///      list.subList(from, to).clear();
		/// </pre>
		/// Similar idioms may be constructed for <seealso cref="#indexOf(Object)"/> and
		/// <seealso cref="#lastIndexOf(Object)"/>, and all of the algorithms in the
		/// <seealso cref="Collections"/> class can be applied to a subList.
		/// 
		/// </para>
		/// <para>The semantics of the list returned by this method become undefined if
		/// the backing list (i.e., this list) is <i>structurally modified</i> in
		/// any way other than via the returned list.  (Structural modifications are
		/// those that change the size of this list, or otherwise perturb it in such
		/// a fashion that iterations in progress may yield incorrect results.)
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual List<E> SubList(int fromIndex, int toIndex)
		{
			SubListRangeCheck(fromIndex, toIndex, Size_Renamed);
			return new SubList(this, this, 0, fromIndex, toIndex);
		}

		internal static void SubListRangeCheck(int fromIndex, int toIndex, int size)
		{
			if (fromIndex < 0)
			{
				throw new IndexOutOfBoundsException("fromIndex = " + fromIndex);
			}
			if (toIndex > size)
			{
				throw new IndexOutOfBoundsException("toIndex = " + toIndex);
			}
			if (fromIndex > toIndex)
			{
				throw new IllegalArgumentException("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
			}
		}

		private class SubList : AbstractList<E>, RandomAccess
		{
			private readonly List<E> OuterInstance;

			internal readonly AbstractList<E> Parent;
			internal readonly int ParentOffset;
			internal readonly int Offset;
			internal int Size_Renamed;

			internal SubList(List<E> outerInstance, AbstractList<E> parent, int offset, int fromIndex, int toIndex)
			{
				this.OuterInstance = outerInstance;
				this.Parent = parent;
				this.ParentOffset = fromIndex;
				this.Offset = offset + fromIndex;
				this.Size_Renamed = toIndex - fromIndex;
				this.ModCount = outerInstance.ModCount;
			}

			public virtual E Set(int index, E e)
			{
				RangeCheck(index);
				CheckForComodification();
				E oldValue = OuterInstance.ElementData(Offset + index);
				OuterInstance.ElementData_Renamed[Offset + index] = e;
				return oldValue;
			}

			public virtual E Get(int index)
			{
				RangeCheck(index);
				CheckForComodification();
				return OuterInstance.ElementData(Offset + index);
			}

			public virtual int Size()
			{
				CheckForComodification();
				return this.Size_Renamed;
			}

			public virtual void Add(int index, E e)
			{
				RangeCheckForAdd(index);
				CheckForComodification();
				Parent.Add(ParentOffset + index, e);
				this.ModCount = Parent.ModCount;
				this.Size_Renamed++;
			}

			public virtual E Remove(int index)
			{
				RangeCheck(index);
				CheckForComodification();
				E result = Parent.Remove(ParentOffset + index);
				this.ModCount = Parent.ModCount;
				this.Size_Renamed--;
				return result;
			}

			protected internal virtual void RemoveRange(int fromIndex, int toIndex)
			{
				CheckForComodification();
				Parent.RemoveRange(ParentOffset + fromIndex, ParentOffset + toIndex);
				this.ModCount = Parent.ModCount;
				this.Size_Renamed -= toIndex - fromIndex;
			}

			public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
			{
				return AddAll(this.Size_Renamed, c);
			}

			public virtual bool addAll<T1>(int index, Collection<T1> c) where T1 : E
			{
				RangeCheckForAdd(index);
				int cSize = c.Size();
				if (cSize == 0)
				{
					return false;
				}

				CheckForComodification();
				Parent.AddAll(ParentOffset + index, c);
				this.ModCount = Parent.ModCount;
				this.Size_Renamed += cSize;
				return true;
			}

			public virtual Iterator<E> Iterator()
			{
				return ListIterator();
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ListIterator<E> listIterator(final int index)
			public virtual ListIterator<E> ListIterator(int index)
			{
				CheckForComodification();
				RangeCheckForAdd(index);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = this.offset;
				int offset = this.Offset;

				return new ListIteratorAnonymousInnerClassHelper(this, index, offset);
			}

			private class ListIteratorAnonymousInnerClassHelper : ListIterator<E>
			{
				private readonly SubList OuterInstance;

				private int Index;
				private int Offset;

				public ListIteratorAnonymousInnerClassHelper(SubList outerInstance, int index, int offset)
				{
					this.outerInstance = outerInstance;
					this.Index = index;
					this.Offset = offset;
					cursor = index;
					lastRet = -1;
					expectedModCount = outerInstance.outerInstance.modCount;
				}

				internal int cursor;
				internal int lastRet;
				internal int expectedModCount;

				public virtual bool HasNext()
				{
					return cursor != OuterInstance.size;
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
				public virtual E Next()
				{
					outerInstance.CheckForComodification();
					int List_Fields.i = cursor;
					if (List_Fields.i >= OuterInstance.size)
					{
						throw new NoSuchElementException();
					}
					Object[] elementData = OuterInstance.outerInstance.ElementData_Renamed;
					if (Offset + List_Fields.i >= elementData.Length)
					{
						throw new ConcurrentModificationException();
					}
					cursor = List_Fields.i + 1;
					return (E) elementData[Offset + (lastRet = List_Fields.i)];
				}

				public virtual bool HasPrevious()
				{
					return cursor != 0;
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E previous()
				public virtual E Previous()
				{
					outerInstance.CheckForComodification();
					int List_Fields.i = cursor - 1;
					if (List_Fields.i < 0)
					{
						throw new NoSuchElementException();
					}
					Object[] elementData = OuterInstance.outerInstance.ElementData_Renamed;
					if (Offset + List_Fields.i >= elementData.Length)
					{
						throw new ConcurrentModificationException();
					}
					cursor = List_Fields.i;
					return (E) elementData[Offset + (lastRet = List_Fields.i)];
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> consumer)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				public virtual void forEachRemaining<T1>(Consumer<T1> consumer)
				{
					Objects.RequireNonNull(consumer);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = SubList.this.size;
					int size = OuterInstance.size;
					int List_Fields.i = cursor;
					if (List_Fields.i >= size)
					{
						return;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] elementData = outerInstance.outerInstance.elementData;
					Object[] elementData = OuterInstance.outerInstance.ElementData_Renamed;
					if (Offset + List_Fields.i >= elementData.Length)
					{
						throw new ConcurrentModificationException();
					}
					while (List_Fields.i != size && OuterInstance.ModCount == expectedModCount)
					{
						consumer.Accept((E) elementData[Offset + (List_Fields.i++)]);
					}
					// update once at end of iteration to reduce heap write traffic
					lastRet = cursor = List_Fields.i;
					outerInstance.CheckForComodification();
				}

				public virtual int NextIndex()
				{
					return cursor;
				}

				public virtual int PreviousIndex()
				{
					return cursor - 1;
				}

				public virtual void Remove()
				{
					if (lastRet < 0)
					{
						throw new IllegalStateException();
					}
					outerInstance.CheckForComodification();

					try
					{
						OuterInstance.Remove(lastRet);
						cursor = lastRet;
						lastRet = -1;
						expectedModCount = OuterInstance.outerInstance.ModCount;
					}
					catch (IndexOutOfBoundsException)
					{
						throw new ConcurrentModificationException();
					}
				}

				public virtual void Set(E e)
				{
					if (lastRet < 0)
					{
						throw new IllegalStateException();
					}
					outerInstance.CheckForComodification();

					try
					{
						OuterInstance.outerInstance.Set(Offset + lastRet, e);
					}
					catch (IndexOutOfBoundsException)
					{
						throw new ConcurrentModificationException();
					}
				}

				public virtual void Add(E e)
				{
					outerInstance.CheckForComodification();

					try
					{
						int List_Fields.i = cursor;
						OuterInstance.Add(List_Fields.i, e);
						cursor = List_Fields.i + 1;
						lastRet = -1;
						expectedModCount = OuterInstance.outerInstance.ModCount;
					}
					catch (IndexOutOfBoundsException)
					{
						throw new ConcurrentModificationException();
					}
				}

				internal void CheckForComodification()
				{
					if (expectedModCount != OuterInstance.outerInstance.ModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}

			public virtual List<E> SubList(int fromIndex, int toIndex)
			{
				SubListRangeCheck(fromIndex, toIndex, Size_Renamed);
				return new SubList(OuterInstance, this, Offset, fromIndex, toIndex);
			}

			internal virtual void RangeCheck(int index)
			{
				if (index < 0 || index >= this.Size_Renamed)
				{
					throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
				}
			}

			internal virtual void RangeCheckForAdd(int index)
			{
				if (index < 0 || index > this.Size_Renamed)
				{
					throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
				}
			}

			internal virtual String OutOfBoundsMsg(int index)
			{
				return "Index: " + index + ", Size: " + this.Size_Renamed;
			}

			internal virtual void CheckForComodification()
			{
				if (OuterInstance.ModCount != this.ModCount)
				{
					throw new ConcurrentModificationException();
				}
			}

			public virtual Spliterator<E> Spliterator()
			{
				CheckForComodification();
				return new ArrayListSpliterator<E>(OuterInstance, Offset, Offset + this.Size_Renamed, this.ModCount);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
		public override void forEach<T1>(Consumer<T1> action)
		{
			Objects.RequireNonNull(action);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
			int expectedModCount = ModCount;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final E[] elementData = (E[]) this.elementData;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			E[] elementData = (E[]) this.ElementData_Renamed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = this.size;
			int size = this.Size_Renamed;
			for (int List_Fields.i = 0; ModCount == expectedModCount && List_Fields.i < size; List_Fields.i++)
			{
				action.Accept(elementData[List_Fields.i]);
			}
			if (ModCount != expectedModCount)
			{
				throw new ConcurrentModificationException();
			}
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// list.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, and <seealso cref="Spliterator#ORDERED"/>.
		/// Overriding implementations should document the reporting of additional
		/// characteristic values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this list
		/// @since 1.8 </returns>
		public override Spliterator<E> Spliterator()
		{
			return new ArrayListSpliterator<>(this, 0, -1, 0);
		}

		/// <summary>
		/// Index-based split-by-two, lazily initialized Spliterator </summary>
		internal sealed class ArrayListSpliterator<E> : Spliterator<E>
		{

			/*
			 * If ArrayLists were immutable, or structurally immutable (no
			 * adds, removes, etc), we could implement their spliterators
			 * with Arrays.spliterator. Instead we detect as much
			 * interference during traversal as practical without
			 * sacrificing much performance. We rely primarily on
			 * modCounts. These are not guaranteed to detect concurrency
			 * violations, and are sometimes overly conservative about
			 * within-thread interference, but detect enough problems to
			 * be worthwhile in practice. To carry this out, we (1) lazily
			 * initialize fence and expectedModCount until the latest
			 * point that we need to commit to the state we are checking
			 * against; thus improving precision.  (This doesn't apply to
			 * SubLists, that create spliterators with current non-lazy
			 * values).  (2) We perform only a single
			 * ConcurrentModificationException check at the end of forEach
			 * (the most performance-sensitive method). When using forEach
			 * (as opposed to iterators), we can normally only detect
			 * interference after actions, not before. Further
			 * CME-triggering checks apply to all other possible
			 * violations of assumptions for example null or too-small
			 * elementData array given its size(), that could only have
			 * occurred due to interference.  This allows the inner loop
			 * of forEach to run without any further checks, and
			 * simplifies lambda-resolution. While this does entail a
			 * number of checks, note that in the common case of
			 * list.stream().forEach(a), no checks or other computation
			 * occur anywhere other than inside forEach itself.  The other
			 * less-often-used methods cannot take advantage of most of
			 * these streamlinings.
			 */

			internal readonly List<E> List;
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // -1 until used; then one past last index
			internal int ExpectedModCount; // initialized when fence set

			/// <summary>
			/// Create new spliterator covering the given  range </summary>
			internal ArrayListSpliterator(List<E> list, int origin, int fence, int expectedModCount)
			{
				this.List = list; // OK if null unless traversed
				this.Index = origin;
				this.Fence_Renamed = fence;
				this.ExpectedModCount = expectedModCount;
			}

			internal int Fence
			{
				get
				{
					int hi; // (a specialized variant appears in method forEach)
					List<E> lst;
					if ((hi = Fence_Renamed) < 0)
					{
						if ((lst = List) == null)
						{
							hi = Fence_Renamed = 0;
						}
						else
						{
							ExpectedModCount = lst.ModCount;
							hi = Fence_Renamed = lst.Size_Renamed;
						}
					}
					return hi;
				}
			}

			public ArrayListSpliterator<E> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? null : new ArrayListSpliterator<E>(List, lo, Index = mid, ExpectedModCount); // divide range in half unless too small
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base E> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				int hi = Fence, List_Fields ;
				if (List_Fields.i < hi)
				{
					Index = List_Fields.i + 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E)list.elementData[List_Fields.i];
					E e = (E)List.ElementData_Renamed[List_Fields.i];
					action.Accept(e);
					if (List.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					return true;
				}
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int List_Fields.i, hi, mc; // hoist accesses and checks from loop
				List<E> lst;
				Object[] List_Fields.a;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((lst = List) != null && (List_Fields.a = lst.ElementData_Renamed) != null)
				{
					if ((hi = Fence_Renamed) < 0)
					{
						mc = lst.ModCount;
						hi = lst.Size_Renamed;
					}
					else
					{
						mc = ExpectedModCount;
					}
					if ((List_Fields.i = Index) >= 0 && (Index = hi) <= List_Fields.a.Length)
					{
						for (; List_Fields.i < hi; ++List_Fields.i)
						{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) List_Fields.a[List_Fields.i];
							E e = (E) List_Fields.a[List_Fields.i];
							action.Accept(e);
						}
						if (lst.ModCount == mc)
						{
							return;
						}
					}
				}
				throw new ConcurrentModificationException();
			}

			public long EstimateSize()
			{
				return (long)(Fence - Index);
			}

			public int Characteristics()
			{
				return Spliterator_Fields.ORDERED | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
		public override bool removeIf<T1>(Predicate<T1> filter)
		{
			Objects.RequireNonNull(filter);
			// figure out which elements are to be removed
			// any exception thrown from the filter predicate at this stage
			// will leave the collection unmodified
			int removeCount = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BitSet removeSet = new BitSet(size);
			BitSet removeSet = new BitSet(Size_Renamed);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
			int expectedModCount = ModCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = this.size;
			int size = this.Size_Renamed;
			for (int List_Fields.i = 0; ModCount == expectedModCount && List_Fields.i < size; List_Fields.i++)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final E element = (E) elementData[List_Fields.i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				E element = (E) ElementData_Renamed[List_Fields.i];
				if (filter.Test(element))
				{
					removeSet.Set(List_Fields.i);
					removeCount++;
				}
			}
			if (ModCount != expectedModCount)
			{
				throw new ConcurrentModificationException();
			}

			// shift surviving elements left over the spaces left by removed elements
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean anyToRemove = removeCount > 0;
			bool anyToRemove = removeCount > 0;
			if (anyToRemove)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newSize = size - removeCount;
				int newSize = size - removeCount;
				for (int List_Fields.i = 0, j = 0; (List_Fields.i < size) && (j < newSize); List_Fields.i++, j++)
				{
					List_Fields.i = removeSet.NextClearBit(List_Fields.i);
					ElementData_Renamed[j] = ElementData_Renamed[List_Fields.i];
				}
				for (int k = newSize; k < size; k++)
				{
					ElementData_Renamed[k] = null; // Let gc do its work
				}
				this.Size_Renamed = newSize;
				if (ModCount != expectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				ModCount++;
			}

			return anyToRemove;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void replaceAll(java.util.function.UnaryOperator<E> operator)
		public override void ReplaceAll(UnaryOperator<E> @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
			int expectedModCount = ModCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = this.size;
			int size = this.Size_Renamed;
			for (int List_Fields.i = 0; ModCount == expectedModCount && List_Fields.i < size; List_Fields.i++)
			{
				ElementData_Renamed[List_Fields.i] = @operator.Apply((E) ElementData_Renamed[List_Fields.i]);
			}
			if (ModCount != expectedModCount)
			{
				throw new ConcurrentModificationException();
			}
			ModCount++;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override void sort<T1>(Comparator<T1> c)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
			int expectedModCount = ModCount;
			System.Array.Sort((E[]) ElementData_Renamed, 0, Size_Renamed, c);
			if (ModCount != expectedModCount)
			{
				throw new ConcurrentModificationException();
			}
			ModCount++;
		}
	}

}