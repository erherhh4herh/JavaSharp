using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code Vector} class implements a growable array of
	/// objects. Like an array, it contains components that can be
	/// accessed using an integer index. However, the size of a
	/// {@code Vector} can grow or shrink as needed to accommodate
	/// adding and removing items after the {@code Vector} has been created.
	/// 
	/// <para>Each vector tries to optimize storage management by maintaining a
	/// {@code capacity} and a {@code capacityIncrement}. The
	/// {@code capacity} is always at least as large as the vector
	/// size; it is usually larger because as components are added to the
	/// vector, the vector's storage increases in chunks the size of
	/// {@code capacityIncrement}. An application can increase the
	/// capacity of a vector before inserting a large number of
	/// components; this reduces the amount of incremental reallocation.
	/// 
	/// </para>
	/// <para><a name="fail-fast">
	/// The iterators returned by this class's <seealso cref="#iterator() iterator"/> and
	/// <seealso cref="#listIterator(int) listIterator"/> methods are <em>fail-fast</em></a>:
	/// if the vector is structurally modified at any time after the iterator is
	/// created, in any way except through the iterator's own
	/// <seealso cref="ListIterator#remove() remove"/> or
	/// <seealso cref="ListIterator#add(Object) add"/> methods, the iterator will throw a
	/// <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of
	/// concurrent modification, the iterator fails quickly and cleanly, rather
	/// than risking arbitrary, non-deterministic behavior at an undetermined
	/// time in the future.  The <seealso cref="Enumeration Enumerations"/> returned by
	/// the <seealso cref="#elements() elements"/> method are <em>not</em> fail-fast.
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
	/// <para>As of the Java 2 platform v1.2, this class was retrofitted to
	/// implement the <seealso cref="List"/> interface, making it a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.  Unlike the new collection
	/// implementations, {@code Vector} is synchronized.  If a thread-safe
	/// implementation is not needed, it is recommended to use {@link
	/// ArrayList} in place of {@code Vector}.
	/// 
	/// @author  Lee Boynton
	/// @author  Jonathan Payne
	/// </para>
	/// </summary>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= LinkedList
	/// @since   JDK1.0 </seealso>
	[Serializable]
	public class Vector<E> : AbstractList<E>, List<E>, RandomAccess, Cloneable
	{
		/// <summary>
		/// The array buffer into which the components of the vector are
		/// stored. The capacity of the vector is the length of this array buffer,
		/// and is at least large enough to contain all the vector's elements.
		/// 
		/// <para>Any array elements following the last element in the Vector are null.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		protected internal Object[] ElementData_Renamed;

		/// <summary>
		/// The number of valid components in this {@code Vector} object.
		/// Components {@code elementData[0]} through
		/// {@code elementData[elementCount-1]} are the actual items.
		/// 
		/// @serial
		/// </summary>
		protected internal int ElementCount;

		/// <summary>
		/// The amount by which the capacity of the vector is automatically
		/// incremented when its size becomes greater than its capacity.  If
		/// the capacity increment is less than or equal to zero, the capacity
		/// of the vector is doubled each time it needs to grow.
		/// 
		/// @serial
		/// </summary>
		protected internal int CapacityIncrement;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = -2767605614048989439L;

		/// <summary>
		/// Constructs an empty vector with the specified initial capacity and
		/// capacity increment.
		/// </summary>
		/// <param name="initialCapacity">     the initial capacity of the vector </param>
		/// <param name="capacityIncrement">   the amount by which the capacity is
		///                              increased when the vector overflows </param>
		/// <exception cref="IllegalArgumentException"> if the specified initial capacity
		///         is negative </exception>
		public Vector(int initialCapacity, int capacityIncrement) : base()
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException("Illegal Capacity: " + initialCapacity);
			}
			this.ElementData_Renamed = new Object[initialCapacity];
			this.CapacityIncrement = capacityIncrement;
		}

		/// <summary>
		/// Constructs an empty vector with the specified initial capacity and
		/// with its capacity increment equal to zero.
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the vector </param>
		/// <exception cref="IllegalArgumentException"> if the specified initial capacity
		///         is negative </exception>
		public Vector(int initialCapacity) : this(initialCapacity, 0)
		{
		}

		/// <summary>
		/// Constructs an empty vector so that its internal data array
		/// has size {@code 10} and its standard capacity increment is
		/// zero.
		/// </summary>
		public Vector() : this(10)
		{
		}

		/// <summary>
		/// Constructs a vector containing the elements of the specified
		/// collection, in the order they are returned by the collection's
		/// iterator.
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed into this
		///       vector </param>
		/// <exception cref="NullPointerException"> if the specified collection is null
		/// @since   1.2 </exception>
		public Vector<T1>(Collection<T1> c) where T1 : E
		{
			ElementData_Renamed = c.ToArray();
			ElementCount = ElementData_Renamed.Length;
			// c.toArray might (incorrectly) not return Object[] (see 6260652)
			if (ElementData_Renamed.GetType() != typeof(Object[]))
			{
				ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, ElementCount, typeof(Object[]));
			}
		}

		/// <summary>
		/// Copies the components of this vector into the specified array.
		/// The item at index {@code k} in this vector is copied into
		/// component {@code k} of {@code anArray}.
		/// </summary>
		/// <param name="anArray"> the array into which the components get copied </param>
		/// <exception cref="NullPointerException"> if the given array is null </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the specified array is not
		///         large enough to hold all the components of this vector </exception>
		/// <exception cref="ArrayStoreException"> if a component of this vector is not of
		///         a runtime type that can be stored in the specified array </exception>
		/// <seealso cref= #toArray(Object[]) </seealso>
		public virtual void CopyInto(Object[] anArray)
		{
			lock (this)
			{
				System.Array.Copy(ElementData_Renamed, 0, anArray, 0, ElementCount);
			}
		}

		/// <summary>
		/// Trims the capacity of this vector to be the vector's current
		/// size. If the capacity of this vector is larger than its current
		/// size, then the capacity is changed to equal the size by replacing
		/// its internal data array, kept in the field {@code elementData},
		/// with a smaller one. An application can use this operation to
		/// minimize the storage of a vector.
		/// </summary>
		public virtual void TrimToSize()
		{
			lock (this)
			{
				ModCount++;
				int oldCapacity = ElementData_Renamed.Length;
				if (ElementCount < oldCapacity)
				{
					ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, ElementCount);
				}
			}
		}

		/// <summary>
		/// Increases the capacity of this vector, if necessary, to ensure
		/// that it can hold at least the number of components specified by
		/// the minimum capacity argument.
		/// 
		/// <para>If the current capacity of this vector is less than
		/// {@code minCapacity}, then its capacity is increased by replacing its
		/// internal data array, kept in the field {@code elementData}, with a
		/// larger one.  The size of the new data array will be the old size plus
		/// {@code capacityIncrement}, unless the value of
		/// {@code capacityIncrement} is less than or equal to zero, in which case
		/// the new capacity will be twice the old capacity; but if this new size
		/// is still smaller than {@code minCapacity}, then the new capacity will
		/// be {@code minCapacity}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minCapacity"> the desired minimum capacity </param>
		public virtual void EnsureCapacity(int minCapacity)
		{
			lock (this)
			{
				if (minCapacity > 0)
				{
					ModCount++;
					EnsureCapacityHelper(minCapacity);
				}
			}
		}

		/// <summary>
		/// This implements the unsynchronized semantics of ensureCapacity.
		/// Synchronized methods in this class can internally call this
		/// method for ensuring capacity without incurring the cost of an
		/// extra synchronization.
		/// </summary>
		/// <seealso cref= #ensureCapacity(int) </seealso>
		private void EnsureCapacityHelper(int minCapacity)
		{
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

		private void Grow(int minCapacity)
		{
			// overflow-conscious code
			int oldCapacity = ElementData_Renamed.Length;
			int newCapacity = oldCapacity + ((CapacityIncrement > 0) ? CapacityIncrement : oldCapacity);
			if (newCapacity - minCapacity < 0)
			{
				newCapacity = minCapacity;
			}
			if (newCapacity - MAX_ARRAY_SIZE > 0)
			{
				newCapacity = HugeCapacity(minCapacity);
			}
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
		/// Sets the size of this vector. If the new size is greater than the
		/// current size, new {@code null} items are added to the end of
		/// the vector. If the new size is less than the current size, all
		/// components at index {@code newSize} and greater are discarded.
		/// </summary>
		/// <param name="newSize">   the new size of this vector </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the new size is negative </exception>
		public virtual int Size
		{
			set
			{
				lock (this)
				{
					ModCount++;
					if (value > ElementCount)
					{
						EnsureCapacityHelper(value);
					}
					else
					{
						for (int List_Fields.i = value ; List_Fields.i < ElementCount ; List_Fields.i++)
						{
							ElementData_Renamed[List_Fields.i] = null;
						}
					}
					ElementCount = value;
				}
			}
		}

		/// <summary>
		/// Returns the current capacity of this vector.
		/// </summary>
		/// <returns>  the current capacity (the length of its internal
		///          data array, kept in the field {@code elementData}
		///          of this vector) </returns>
		public virtual int Capacity()
		{
			lock (this)
			{
				return ElementData_Renamed.Length;
			}
		}

		/// <summary>
		/// Returns the number of components in this vector.
		/// </summary>
		/// <returns>  the number of components in this vector </returns>
		public virtual int Count
		{
			get
			{
				lock (this)
				{
					return ElementCount;
				}
			}
		}

		/// <summary>
		/// Tests if this vector has no components.
		/// </summary>
		/// <returns>  {@code true} if and only if this vector has
		///          no components, that is, its size is zero;
		///          {@code false} otherwise. </returns>
		public virtual bool Empty
		{
			get
			{
				lock (this)
				{
					return ElementCount == 0;
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of the components of this vector. The
		/// returned {@code Enumeration} object will generate all items in
		/// this vector. The first item generated is the item at index {@code 0},
		/// then the item at index {@code 1}, and so on.
		/// </summary>
		/// <returns>  an enumeration of the components of this vector </returns>
		/// <seealso cref=     Iterator </seealso>
		public virtual IEnumerator<E> Elements()
		{
			return new IteratorAnonymousInnerClassHelper(this);
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<E>
		{
			private readonly Vector<E> OuterInstance;

			public IteratorAnonymousInnerClassHelper(Vector<E> outerInstance)
			{
				this.outerInstance = outerInstance;
				count = 0;
			}

			internal int count;

			public virtual bool HasMoreElements()
			{
				return count < OuterInstance.ElementCount;
			}

			public virtual E NextElement()
			{
				lock (OuterInstance)
				{
					if (count < OuterInstance.ElementCount)
					{
						return outerInstance.ElementData(count++);
					}
				}
				throw new NoSuchElementException("Vector Enumeration");
			}
		}

		/// <summary>
		/// Returns {@code true} if this vector contains the specified element.
		/// More formally, returns {@code true} if and only if this vector
		/// contains at least one element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this vector is to be tested </param>
		/// <returns> {@code true} if this vector contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			return IndexOf(o, 0) >= 0;
		}

		/// <summary>
		/// Returns the index of the first occurrence of the specified element
		/// in this vector, or -1 if this vector does not contain the element.
		/// More formally, returns the lowest index {@code i} such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <returns> the index of the first occurrence of the specified element in
		///         this vector, or -1 if this vector does not contain the element </returns>
		public virtual int IndexOf(Object o)
		{
			return IndexOf(o, 0);
		}

		/// <summary>
		/// Returns the index of the first occurrence of the specified element in
		/// this vector, searching forwards from {@code index}, or returns -1 if
		/// the element is not found.
		/// More formally, returns the lowest index {@code i} such that
		/// <tt>(i&nbsp;&gt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i))))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <param name="index"> index to start searching from </param>
		/// <returns> the index of the first occurrence of the element in
		///         this vector at position {@code index} or later in the vector;
		///         {@code -1} if the element is not found. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		/// <seealso cref=     Object#equals(Object) </seealso>
		public virtual int IndexOf(Object o, int index)
		{
			lock (this)
			{
				if (o == null)
				{
					for (int List_Fields.i = index ; List_Fields.i < ElementCount ; List_Fields.i++)
					{
						if (ElementData_Renamed[List_Fields.i] == null)
						{
							return List_Fields.i;
						}
					}
				}
				else
				{
					for (int List_Fields.i = index ; List_Fields.i < ElementCount ; List_Fields.i++)
					{
						if (o.Equals(ElementData_Renamed[List_Fields.i]))
						{
							return List_Fields.i;
						}
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// Returns the index of the last occurrence of the specified element
		/// in this vector, or -1 if this vector does not contain the element.
		/// More formally, returns the highest index {@code i} such that
		/// <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <returns> the index of the last occurrence of the specified element in
		///         this vector, or -1 if this vector does not contain the element </returns>
		public virtual int LastIndexOf(Object o)
		{
			lock (this)
			{
				return LastIndexOf(o, ElementCount - 1);
			}
		}

		/// <summary>
		/// Returns the index of the last occurrence of the specified element in
		/// this vector, searching backwards from {@code index}, or returns -1 if
		/// the element is not found.
		/// More formally, returns the highest index {@code i} such that
		/// <tt>(i&nbsp;&lt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i))))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="o"> element to search for </param>
		/// <param name="index"> index to start searching backwards from </param>
		/// <returns> the index of the last occurrence of the element at position
		///         less than or equal to {@code index} in this vector;
		///         -1 if the element is not found. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is greater
		///         than or equal to the current size of this vector </exception>
		public virtual int LastIndexOf(Object o, int index)
		{
			lock (this)
			{
				if (index >= ElementCount)
				{
					throw new IndexOutOfBoundsException(index + " >= " + ElementCount);
				}
        
				if (o == null)
				{
					for (int List_Fields.i = index; List_Fields.i >= 0; List_Fields.i--)
					{
						if (ElementData_Renamed[List_Fields.i] == null)
						{
							return List_Fields.i;
						}
					}
				}
				else
				{
					for (int List_Fields.i = index; List_Fields.i >= 0; List_Fields.i--)
					{
						if (o.Equals(ElementData_Renamed[List_Fields.i]))
						{
							return List_Fields.i;
						}
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// Returns the component at the specified index.
		/// 
		/// <para>This method is identical in functionality to the <seealso cref="#get(int)"/>
		/// method (which is part of the <seealso cref="List"/> interface).
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   an index into this vector </param>
		/// <returns>     the component at the specified index </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index >= size()}) </exception>
		public virtual E ElementAt(int index)
		{
			lock (this)
			{
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index + " >= " + ElementCount);
				}
        
				return ElementData(index);
			}
		}

		/// <summary>
		/// Returns the first component (the item at index {@code 0}) of
		/// this vector.
		/// </summary>
		/// <returns>     the first component of this vector </returns>
		/// <exception cref="NoSuchElementException"> if this vector has no components </exception>
		public virtual E FirstElement()
		{
			lock (this)
			{
				if (ElementCount == 0)
				{
					throw new NoSuchElementException();
				}
				return ElementData(0);
			}
		}

		/// <summary>
		/// Returns the last component of the vector.
		/// </summary>
		/// <returns>  the last component of the vector, i.e., the component at index
		///          <code>size()&nbsp;-&nbsp;1</code>. </returns>
		/// <exception cref="NoSuchElementException"> if this vector is empty </exception>
		public virtual E LastElement()
		{
			lock (this)
			{
				if (ElementCount == 0)
				{
					throw new NoSuchElementException();
				}
				return ElementData(ElementCount - 1);
			}
		}

		/// <summary>
		/// Sets the component at the specified {@code index} of this
		/// vector to be the specified object. The previous component at that
		/// position is discarded.
		/// 
		/// <para>The index must be a value greater than or equal to {@code 0}
		/// and less than the current size of the vector.
		/// 
		/// </para>
		/// <para>This method is identical in functionality to the
		/// <seealso cref="#set(int, Object) set(int, E)"/>
		/// method (which is part of the <seealso cref="List"/> interface). Note that the
		/// {@code set} method reverses the order of the parameters, to more closely
		/// match array usage.  Note also that the {@code set} method returns the
		/// old value that was stored at the specified position.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">     what the component is to be set to </param>
		/// <param name="index">   the specified index </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index >= size()}) </exception>
		public virtual void SetElementAt(E obj, int index)
		{
			lock (this)
			{
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index + " >= " + ElementCount);
				}
				ElementData_Renamed[index] = obj;
			}
		}

		/// <summary>
		/// Deletes the component at the specified index. Each component in
		/// this vector with an index greater or equal to the specified
		/// {@code index} is shifted downward to have an index one
		/// smaller than the value it had previously. The size of this vector
		/// is decreased by {@code 1}.
		/// 
		/// <para>The index must be a value greater than or equal to {@code 0}
		/// and less than the current size of the vector.
		/// 
		/// </para>
		/// <para>This method is identical in functionality to the <seealso cref="#remove(int)"/>
		/// method (which is part of the <seealso cref="List"/> interface).  Note that the
		/// {@code remove} method returns the old value that was stored at the
		/// specified position.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the object to remove </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index >= size()}) </exception>
		public virtual void RemoveElementAt(int index)
		{
			lock (this)
			{
				ModCount++;
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index + " >= " + ElementCount);
				}
				else if (index < 0)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
				int j = ElementCount - index - 1;
				if (j > 0)
				{
					System.Array.Copy(ElementData_Renamed, index + 1, ElementData_Renamed, index, j);
				}
				ElementCount--;
				ElementData_Renamed[ElementCount] = null; // to let gc do its work
			}
		}

		/// <summary>
		/// Inserts the specified object as a component in this vector at the
		/// specified {@code index}. Each component in this vector with
		/// an index greater or equal to the specified {@code index} is
		/// shifted upward to have an index one greater than the value it had
		/// previously.
		/// 
		/// <para>The index must be a value greater than or equal to {@code 0}
		/// and less than or equal to the current size of the vector. (If the
		/// index is equal to the current size of the vector, the new element
		/// is appended to the Vector.)
		/// 
		/// </para>
		/// <para>This method is identical in functionality to the
		/// <seealso cref="#add(int, Object) add(int, E)"/>
		/// method (which is part of the <seealso cref="List"/> interface).  Note that the
		/// {@code add} method reverses the order of the parameters, to more closely
		/// match array usage.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">     the component to insert </param>
		/// <param name="index">   where to insert the new component </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index > size()}) </exception>
		public virtual void InsertElementAt(E obj, int index)
		{
			lock (this)
			{
				ModCount++;
				if (index > ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index + " > " + ElementCount);
				}
				EnsureCapacityHelper(ElementCount + 1);
				System.Array.Copy(ElementData_Renamed, index, ElementData_Renamed, index + 1, ElementCount - index);
				ElementData_Renamed[index] = obj;
				ElementCount++;
			}
		}

		/// <summary>
		/// Adds the specified component to the end of this vector,
		/// increasing its size by one. The capacity of this vector is
		/// increased if its size becomes greater than its capacity.
		/// 
		/// <para>This method is identical in functionality to the
		/// <seealso cref="#add(Object) add(E)"/>
		/// method (which is part of the <seealso cref="List"/> interface).
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the component to be added </param>
		public virtual void AddElement(E obj)
		{
			lock (this)
			{
				ModCount++;
				EnsureCapacityHelper(ElementCount + 1);
				ElementData_Renamed[ElementCount++] = obj;
			}
		}

		/// <summary>
		/// Removes the first (lowest-indexed) occurrence of the argument
		/// from this vector. If the object is found in this vector, each
		/// component in the vector with an index greater or equal to the
		/// object's index is shifted downward to have an index one smaller
		/// than the value it had previously.
		/// 
		/// <para>This method is identical in functionality to the
		/// <seealso cref="#remove(Object)"/> method (which is part of the
		/// <seealso cref="List"/> interface).
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the component to be removed </param>
		/// <returns>  {@code true} if the argument was a component of this
		///          vector; {@code false} otherwise. </returns>
		public virtual bool RemoveElement(Object obj)
		{
			lock (this)
			{
				ModCount++;
				int List_Fields.i = IndexOf(obj);
				if (List_Fields.i >= 0)
				{
					RemoveElementAt(List_Fields.i);
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Removes all components from this vector and sets its size to zero.
		/// 
		/// <para>This method is identical in functionality to the <seealso cref="#clear"/>
		/// method (which is part of the <seealso cref="List"/> interface).
		/// </para>
		/// </summary>
		public virtual void RemoveAllElements()
		{
			lock (this)
			{
				ModCount++;
				// Let gc do its work
				for (int List_Fields.i = 0; List_Fields.i < ElementCount; List_Fields.i++)
				{
					ElementData_Renamed[List_Fields.i] = null;
				}
        
				ElementCount = 0;
			}
		}

		/// <summary>
		/// Returns a clone of this vector. The copy will contain a
		/// reference to a clone of the internal data array, not a reference
		/// to the original internal data array of this {@code Vector} object.
		/// </summary>
		/// <returns>  a clone of this vector </returns>
		public virtual Object Clone()
		{
			lock (this)
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Vector<E> v = (Vector<E>) base.clone();
					Vector<E> v = (Vector<E>) base.Clone();
					v.ElementData_Renamed = Arrays.CopyOf(ElementData_Renamed, ElementCount);
					v.ModCount = 0;
					return v;
				}
				catch (CloneNotSupportedException e)
				{
					// this shouldn't happen, since we are Cloneable
					throw new InternalError(e);
				}
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this Vector
		/// in the correct order.
		/// 
		/// @since 1.2
		/// </summary>
		public virtual Object[] ToArray()
		{
			lock (this)
			{
				return Arrays.CopyOf(ElementData_Renamed, ElementCount);
			}
		}

		/// <summary>
		/// Returns an array containing all of the elements in this Vector in the
		/// correct order; the runtime type of the returned array is that of the
		/// specified array.  If the Vector fits in the specified array, it is
		/// returned therein.  Otherwise, a new array is allocated with the runtime
		/// type of the specified array and the size of this Vector.
		/// 
		/// <para>If the Vector fits in the specified array with room to spare
		/// (i.e., the array has more elements than the Vector),
		/// the element in the array immediately following the end of the
		/// Vector is set to null.  (This is useful in determining the length
		/// of the Vector <em>only</em> if the caller knows that the Vector
		/// does not contain any null elements.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of the Vector are to
		///          be stored, if it is big enough; otherwise, a new array of the
		///          same runtime type is allocated for this purpose. </param>
		/// <returns> an array containing the elements of the Vector </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of a is not a supertype
		/// of the runtime type of every element in this Vector </exception>
		/// <exception cref="NullPointerException"> if the given array is null
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public synchronized <T> T[] toArray(T[] List_Fields.a)
		public virtual T[] toArray<T>(T[] List_Fields)
		{
			lock (this)
			{
				if (List_Fields.a.Length < ElementCount)
				{
					return (T[]) Arrays.CopyOf(ElementData_Renamed, ElementCount, List_Fields.a.GetType());
				}
        
				System.Array.Copy(ElementData_Renamed, 0, List_Fields.a, 0, ElementCount);
        
				if (List_Fields.a.Length > ElementCount)
				{
					List_Fields.a[ElementCount] = null;
				}
        
				return List_Fields.a;
			}
		}

		// Positional Access Operations

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E elementData(int index)
		internal virtual E ElementData(int index)
		{
			return (E) ElementData_Renamed[index];
		}

		/// <summary>
		/// Returns the element at the specified position in this Vector.
		/// </summary>
		/// <param name="index"> index of the element to return </param>
		/// <returns> object at the specified index </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///            ({@code index < 0 || index >= size()})
		/// @since 1.2 </exception>
		public virtual E Get(int index)
		{
			lock (this)
			{
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
        
				return ElementData(index);
			}
		}

		/// <summary>
		/// Replaces the element at the specified position in this Vector with the
		/// specified element.
		/// </summary>
		/// <param name="index"> index of the element to replace </param>
		/// <param name="element"> element to be stored at the specified position </param>
		/// <returns> the element previously at the specified position </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index >= size()})
		/// @since 1.2 </exception>
		public virtual E Set(int index, E element)
		{
			lock (this)
			{
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
        
				E oldValue = ElementData(index);
				ElementData_Renamed[index] = element;
				return oldValue;
			}
		}

		/// <summary>
		/// Appends the specified element to the end of this Vector.
		/// </summary>
		/// <param name="e"> element to be appended to this Vector </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>)
		/// @since 1.2 </returns>
		public virtual bool Add(E e)
		{
			lock (this)
			{
				ModCount++;
				EnsureCapacityHelper(ElementCount + 1);
				ElementData_Renamed[ElementCount++] = e;
				return true;
			}
		}

		/// <summary>
		/// Removes the first occurrence of the specified element in this Vector
		/// If the Vector does not contain the element, it is unchanged.  More
		/// formally, removes the element with the lowest index i such that
		/// {@code (o==null ? get(i)==null : o.equals(get(i)))} (if such
		/// an element exists).
		/// </summary>
		/// <param name="o"> element to be removed from this Vector, if present </param>
		/// <returns> true if the Vector contained the specified element
		/// @since 1.2 </returns>
		public virtual bool Remove(Object o)
		{
			return RemoveElement(o);
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this Vector.
		/// Shifts the element currently at that position (if any) and any
		/// subsequent elements to the right (adds one to their indices).
		/// </summary>
		/// <param name="index"> index at which the specified element is to be inserted </param>
		/// <param name="element"> element to be inserted </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index > size()})
		/// @since 1.2 </exception>
		public virtual void Add(int index, E element)
		{
			InsertElementAt(element, index);
		}

		/// <summary>
		/// Removes the element at the specified position in this Vector.
		/// Shifts any subsequent elements to the left (subtracts one from their
		/// indices).  Returns the element that was removed from the Vector.
		/// </summary>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index >= size()}) </exception>
		/// <param name="index"> the index of the element to be removed </param>
		/// <returns> element that was removed
		/// @since 1.2 </returns>
		public virtual E Remove(int index)
		{
			lock (this)
			{
				ModCount++;
				if (index >= ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
				E oldValue = ElementData(index);
        
				int numMoved = ElementCount - index - 1;
				if (numMoved > 0)
				{
					System.Array.Copy(ElementData_Renamed, index + 1, ElementData_Renamed, index, numMoved);
				}
				ElementData_Renamed[--ElementCount] = null; // Let gc do its work
        
				return oldValue;
			}
		}

		/// <summary>
		/// Removes all of the elements from this Vector.  The Vector will
		/// be empty after this call returns (unless it throws an exception).
		/// 
		/// @since 1.2
		/// </summary>
		public virtual void Clear()
		{
			RemoveAllElements();
		}

		// Bulk Operations

		/// <summary>
		/// Returns true if this Vector contains all of the elements in the
		/// specified Collection.
		/// </summary>
		/// <param name="c"> a collection whose elements will be tested for containment
		///          in this Vector </param>
		/// <returns> true if this Vector contains all of the elements in the
		///         specified collection </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool containsAll<T1>(Collection<T1> c)
		{
			lock (this)
			{
				return base.ContainsAll(c);
			}
		}

		/// <summary>
		/// Appends all of the elements in the specified Collection to the end of
		/// this Vector, in the order that they are returned by the specified
		/// Collection's Iterator.  The behavior of this operation is undefined if
		/// the specified Collection is modified while the operation is in progress.
		/// (This implies that the behavior of this call is undefined if the
		/// specified Collection is this Vector, and this Vector is nonempty.)
		/// </summary>
		/// <param name="c"> elements to be inserted into this Vector </param>
		/// <returns> {@code true} if this Vector changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null
		/// @since 1.2 </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			lock (this)
			{
				ModCount++;
				Object[] List_Fields.a = c.ToArray();
				int numNew = List_Fields.a.Length;
				EnsureCapacityHelper(ElementCount + numNew);
				System.Array.Copy(List_Fields.a, 0, ElementData_Renamed, ElementCount, numNew);
				ElementCount += numNew;
				return numNew != 0;
			}
		}

		/// <summary>
		/// Removes from this Vector all of its elements that are contained in the
		/// specified Collection.
		/// </summary>
		/// <param name="c"> a collection of elements to be removed from the Vector </param>
		/// <returns> true if this Vector changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in this vector are incompatible with the specified
		///         collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this vector contains one or more null
		///         elements and the specified collection does not support null
		///         elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null
		/// @since 1.2 </exception>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			lock (this)
			{
				return base.RemoveAll(c);
			}
		}

		/// <summary>
		/// Retains only the elements in this Vector that are contained in the
		/// specified Collection.  In other words, removes from this Vector all
		/// of its elements that are not contained in the specified Collection.
		/// </summary>
		/// <param name="c"> a collection of elements to be retained in this Vector
		///          (all other elements are removed) </param>
		/// <returns> true if this Vector changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in this vector are incompatible with the specified
		///         collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this vector contains one or more null
		///         elements and the specified collection does not support null
		///         elements
		///         (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null
		/// @since 1.2 </exception>
		public virtual bool retainAll<T1>(Collection<T1> c)
		{
			lock (this)
			{
				return base.RetainAll(c);
			}
		}

		/// <summary>
		/// Inserts all of the elements in the specified Collection into this
		/// Vector at the specified position.  Shifts the element currently at
		/// that position (if any) and any subsequent elements to the right
		/// (increases their indices).  The new elements will appear in the Vector
		/// in the order that they are returned by the specified Collection's
		/// iterator.
		/// </summary>
		/// <param name="index"> index at which to insert the first element from the
		///              specified collection </param>
		/// <param name="c"> elements to be inserted into this Vector </param>
		/// <returns> {@code true} if this Vector changed as a result of the call </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		///         ({@code index < 0 || index > size()}) </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null
		/// @since 1.2 </exception>
		public virtual bool addAll<T1>(int index, Collection<T1> c) where T1 : E
		{
			lock (this)
			{
				ModCount++;
				if (index < 0 || index > ElementCount)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
        
				Object[] List_Fields.a = c.ToArray();
				int numNew = List_Fields.a.Length;
				EnsureCapacityHelper(ElementCount + numNew);
        
				int numMoved = ElementCount - index;
				if (numMoved > 0)
				{
					System.Array.Copy(ElementData_Renamed, index, ElementData_Renamed, index + numNew, numMoved);
				}
        
				System.Array.Copy(List_Fields.a, 0, ElementData_Renamed, index, numNew);
				ElementCount += numNew;
				return numNew != 0;
			}
		}

		/// <summary>
		/// Compares the specified Object with this Vector for equality.  Returns
		/// true if and only if the specified Object is also a List, both Lists
		/// have the same size, and all corresponding pairs of elements in the two
		/// Lists are <em>equal</em>.  (Two elements {@code e1} and
		/// {@code e2} are <em>equal</em> if {@code (e1==null ? e2==null :
		/// e1.equals(e2))}.)  In other words, two Lists are defined to be
		/// equal if they contain the same elements in the same order.
		/// </summary>
		/// <param name="o"> the Object to be compared for equality with this Vector </param>
		/// <returns> true if the specified Object is equal to this Vector </returns>
		public override bool Equals(Object o)
		{
			lock (this)
			{
				return base.Equals(o);
			}
		}

		/// <summary>
		/// Returns the hash code value for this Vector.
		/// </summary>
		public override int HashCode()
		{
			lock (this)
			{
				return base.HashCode();
			}
		}

		/// <summary>
		/// Returns a string representation of this Vector, containing
		/// the String representation of each element.
		/// </summary>
		public override String ToString()
		{
			lock (this)
			{
				return base.ToString();
			}
		}

		/// <summary>
		/// Returns a view of the portion of this List between fromIndex,
		/// inclusive, and toIndex, exclusive.  (If fromIndex and toIndex are
		/// equal, the returned List is empty.)  The returned List is backed by this
		/// List, so changes in the returned List are reflected in this List, and
		/// vice-versa.  The returned List supports all of the optional List
		/// operations supported by this List.
		/// 
		/// <para>This method eliminates the need for explicit range operations (of
		/// the sort that commonly exist for arrays).  Any operation that expects
		/// a List can be used as a range operation by operating on a subList view
		/// instead of a whole List.  For example, the following idiom
		/// removes a range of elements from a List:
		/// <pre>
		///      list.subList(from, to).clear();
		/// </pre>
		/// Similar idioms may be constructed for indexOf and lastIndexOf,
		/// and all of the algorithms in the Collections class can be applied to
		/// a subList.
		/// 
		/// </para>
		/// <para>The semantics of the List returned by this method become undefined if
		/// the backing list (i.e., this List) is <i>structurally modified</i> in
		/// any way other than via the returned List.  (Structural modifications are
		/// those that change the size of the List, or otherwise perturb it in such
		/// a fashion that iterations in progress may yield incorrect results.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> low endpoint (inclusive) of the subList </param>
		/// <param name="toIndex"> high endpoint (exclusive) of the subList </param>
		/// <returns> a view of the specified range within this List </returns>
		/// <exception cref="IndexOutOfBoundsException"> if an endpoint index value is out of range
		///         {@code (fromIndex < 0 || toIndex > size)} </exception>
		/// <exception cref="IllegalArgumentException"> if the endpoint indices are out of order
		///         {@code (fromIndex > toIndex)} </exception>
		public virtual List<E> SubList(int fromIndex, int toIndex)
		{
			lock (this)
			{
				return Collections.SynchronizedList(base.SubList(fromIndex, toIndex), this);
			}
		}

		/// <summary>
		/// Removes from this list all of the elements whose index is between
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		/// Shifts any succeeding elements to the left (reduces their index).
		/// This call shortens the list by {@code (toIndex - fromIndex)} elements.
		/// (If {@code toIndex==fromIndex}, this operation has no effect.)
		/// </summary>
		protected internal virtual void RemoveRange(int fromIndex, int toIndex)
		{
			lock (this)
			{
				ModCount++;
				int numMoved = ElementCount - toIndex;
				System.Array.Copy(ElementData_Renamed, toIndex, ElementData_Renamed, fromIndex, numMoved);
        
				// Let gc do its work
				int newElementCount = ElementCount - (toIndex - fromIndex);
				while (ElementCount != newElementCount)
				{
					ElementData_Renamed[--ElementCount] = null;
				}
			}
		}

		/// <summary>
		/// Save the state of the {@code Vector} instance to a stream (that
		/// is, serialize it).
		/// This method performs synchronization to ensure the consistency
		/// of the serialized data.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.ObjectOutputStream.PutField fields = s.putFields();
			java.io.ObjectOutputStream.PutField fields = s.PutFields();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] data;
			Object[] data;
			lock (this)
			{
				fields.Put("capacityIncrement", CapacityIncrement);
				fields.Put("elementCount", ElementCount);
				data = ElementData_Renamed.clone();
			}
			fields.Put("elementData", data);
			s.WriteFields();
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
			lock (this)
			{
				if (index < 0 || index > ElementCount)
				{
					throw new IndexOutOfBoundsException("Index: " + index);
				}
				return new ListItr(this, this, index);
			}
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
			lock (this)
			{
				return new ListItr(this, this, 0);
			}
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
			lock (this)
			{
				return new Itr(this, this);
			}
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

			private readonly Vector<E> OuterInstance;

			public Itr(Vector<E> outerInstance)
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
				// Racy but within spec, since modifications are checked
				// within or after synchronization in next/previous
				return Cursor != outerInstance.ElementCount;
			}

			public virtual E Next()
			{
				lock (OuterInstance)
				{
					CheckForComodification();
					int List_Fields.i = Cursor;
					if (List_Fields.i >= outerInstance.ElementCount)
					{
						throw new NoSuchElementException();
					}
					Cursor = List_Fields.i + 1;
					return outerInstance.ElementData(LastRet = List_Fields.i);
				}
			}

			public virtual void Remove()
			{
				if (LastRet == -1)
				{
					throw new IllegalStateException();
				}
				lock (OuterInstance)
				{
					CheckForComodification();
					OuterInstance.Remove(LastRet);
					ExpectedModCount = outerInstance.ModCount;
				}
				Cursor = LastRet;
				LastRet = -1;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				lock (OuterInstance)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = elementCount;
					int size = outerInstance.ElementCount;
					int List_Fields.i = Cursor;
					if (List_Fields.i >= size)
					{
						return;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final E[] elementData = (E[]) Vector.this.elementData;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			E[] elementData = (E[]) Vector.this.elementData;
					if (List_Fields.i >= elementData.Length)
					{
						throw new ConcurrentModificationException();
					}
					while (List_Fields.i != size && outerInstance.ModCount == ExpectedModCount)
					{
						action.Accept(elementData[List_Fields.i++]);
					}
					// update once at end of iteration to reduce heap write traffic
					Cursor = List_Fields.i;
					LastRet = List_Fields.i - 1;
					CheckForComodification();
				}
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
		internal sealed class ListItr : Itr, ListIterator<E>
		{
			private readonly Vector<E> OuterInstance;

			internal ListItr(Vector<E> outerInstance, int index) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				Cursor = index;
			}

			public bool HasPrevious()
			{
				return Cursor != 0;
			}

			public int NextIndex()
			{
				return Cursor;
			}

			public int PreviousIndex()
			{
				return Cursor - 1;
			}

			public E Previous()
			{
				lock (OuterInstance)
				{
					CheckForComodification();
					int List_Fields.i = Cursor - 1;
					if (List_Fields.i < 0)
					{
						throw new NoSuchElementException();
					}
					Cursor = List_Fields.i;
					return outerInstance.ElementData(LastRet = List_Fields.i);
				}
			}

			public void Set(E e)
			{
				if (LastRet == -1)
				{
					throw new IllegalStateException();
				}
				lock (OuterInstance)
				{
					CheckForComodification();
					OuterInstance.Set(LastRet, e);
				}
			}

			public void Add(E e)
			{
				int List_Fields.i = Cursor;
				lock (OuterInstance)
				{
					CheckForComodification();
					OuterInstance.Add(List_Fields.i, e);
					ExpectedModCount = outerInstance.ModCount;
				}
				Cursor = List_Fields.i + 1;
				LastRet = -1;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized void forEach(java.util.function.Consumer<? base E> action)
		public override void forEach<T1>(Consumer<T1> action)
		{
			lock (this)
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
//ORIGINAL LINE: final int elementCount = this.elementCount;
				int elementCount = this.ElementCount;
				for (int List_Fields.i = 0; ModCount == expectedModCount && List_Fields.i < elementCount; List_Fields.i++)
				{
					action.Accept(elementData[List_Fields.i]);
				}
				if (ModCount != expectedModCount)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public synchronized boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override bool removeIf<T1>(Predicate<T1> filter)
		{
			lock (this)
			{
				Objects.RequireNonNull(filter);
				// figure out which elements are to be removed
				// any exception thrown from the filter predicate at this stage
				// will leave the collection unmodified
				int removeCount = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = elementCount;
				int size = ElementCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BitSet removeSet = new BitSet(size);
				BitSet removeSet = new BitSet(size);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
				int expectedModCount = ModCount;
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
					ElementCount = newSize;
					if (ModCount != expectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					ModCount++;
				}
        
				return anyToRemove;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public synchronized void replaceAll(java.util.function.UnaryOperator<E> operator)
		public override void ReplaceAll(UnaryOperator<E> @operator)
		{
			lock (this)
			{
				Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
				int expectedModCount = ModCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = elementCount;
				int size = ElementCount;
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
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public synchronized void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override void sort<T1>(Comparator<T1> c)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
				int expectedModCount = ModCount;
				System.Array.Sort((E[]) ElementData_Renamed, 0, ElementCount, c);
				if (ModCount != expectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				ModCount++;
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
			return new VectorSpliterator<>(this, null, 0, -1, 0);
		}

		/// <summary>
		/// Similar to ArrayList Spliterator </summary>
		internal sealed class VectorSpliterator<E> : Spliterator<E>
		{
			internal readonly Vector<E> List;
			internal Object[] Array;
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // -1 until used; then one past last index
			internal int ExpectedModCount; // initialized when fence set

			/// <summary>
			/// Create new spliterator covering the given  range </summary>
			internal VectorSpliterator(Vector<E> list, Object[] array, int origin, int fence, int expectedModCount)
			{
				this.List = list;
				this.Array = array;
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
						lock (List)
						{
							Array = List.ElementData_Renamed;
							ExpectedModCount = List.ModCount;
							hi = Fence_Renamed = List.ElementCount;
						}
					}
					return hi;
				}
			}

			public Spliterator<E> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? null : new VectorSpliterator<E>(List, Array, lo, Index = mid, ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean tryAdvance(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				int List_Fields.i;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Fence > (List_Fields.i = Index))
				{
					Index = List_Fields.i + 1;
					action.Accept((E)Array[List_Fields.i]);
					if (List.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					return true;
				}
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int List_Fields.i, hi; // hoist accesses and checks from loop
				Vector<E> lst;
				Object[] List_Fields.a;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((lst = List) != null)
				{
					if ((hi = Fence_Renamed) < 0)
					{
						lock (lst)
						{
							ExpectedModCount = lst.ModCount;
							List_Fields.a = Array = lst.ElementData_Renamed;
							hi = Fence_Renamed = lst.ElementCount;
						}
					}
					else
					{
						List_Fields.a = Array;
					}
					if (List_Fields.a != null && (List_Fields.i = Index) >= 0 && (Index = hi) <= List_Fields.a.Length)
					{
						while (List_Fields.i < hi)
						{
							action.Accept((E) List_Fields.a[List_Fields.i++]);
						}
						if (lst.ModCount == ExpectedModCount)
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
	}

}