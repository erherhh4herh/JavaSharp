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
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group.  Adapted and released, under explicit permission,
 * from JDK ArrayList.java which carries the following copyright:
 *
 * Copyright 1997 by Sun Microsystems, Inc.,
 * 901 San Antonio Road, Palo Alto, California, 94303, U.S.A.
 * All rights reserved.
 */

namespace java.util.concurrent
{

	/// <summary>
	/// A thread-safe variant of <seealso cref="java.util.ArrayList"/> in which all mutative
	/// operations ({@code add}, {@code set}, and so on) are implemented by
	/// making a fresh copy of the underlying array.
	/// 
	/// <para>This is ordinarily too costly, but may be <em>more</em> efficient
	/// than alternatives when traversal operations vastly outnumber
	/// mutations, and is useful when you cannot or don't want to
	/// synchronize traversals, yet need to preclude interference among
	/// concurrent threads.  The "snapshot" style iterator method uses a
	/// reference to the state of the array at the point that the iterator
	/// was created. This array never changes during the lifetime of the
	/// iterator, so interference is impossible and the iterator is
	/// guaranteed not to throw {@code ConcurrentModificationException}.
	/// The iterator will not reflect additions, removals, or changes to
	/// the list since the iterator was created.  Element-changing
	/// operations on iterators themselves ({@code remove}, {@code set}, and
	/// {@code add}) are not supported. These methods throw
	/// {@code UnsupportedOperationException}.
	/// 
	/// </para>
	/// <para>All elements are permitted, including {@code null}.
	/// 
	/// </para>
	/// <para>Memory consistency effects: As with other concurrent
	/// collections, actions in a thread prior to placing an object into a
	/// {@code CopyOnWriteArrayList}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that element from
	/// the {@code CopyOnWriteArrayList} in another thread.
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
	public class CopyOnWriteArrayList<E> : List<E>, RandomAccess, Cloneable
	{
		private const long SerialVersionUID = 8673264195747942595L;

		/// <summary>
		/// The lock protecting all mutators </summary>
		[NonSerialized]
		internal readonly ReentrantLock @lock = new ReentrantLock();

		/// <summary>
		/// The array, accessed only via getArray/setArray. </summary>
		[NonSerialized]
		private volatile Object[] Array_Renamed;

		/// <summary>
		/// Gets the array.  Non-private so as to also be accessible
		/// from CopyOnWriteArraySet class.
		/// </summary>
		internal Object[] Array
		{
			get
			{
				return Array_Renamed;
			}
			set
			{
				Array_Renamed = value.util.List_Fields.a;
			}
		}


		/// <summary>
		/// Creates an empty list.
		/// </summary>
		public CopyOnWriteArrayList()
		{
			Array = new Object[0];
		}

		/// <summary>
		/// Creates a list containing the elements of the specified
		/// collection, in the order they are returned by the collection's
		/// iterator.
		/// </summary>
		/// <param name="c"> the collection of initially held elements </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public CopyOnWriteArrayList<T1>(ICollection<T1> c) where T1 : E
		{
			Object[] elements;
			if (c.GetType() == typeof(CopyOnWriteArrayList))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: elements = ((CopyOnWriteArrayList<?>)c).getArray();
				elements = ((CopyOnWriteArrayList<?>)c).Array;
			}
			else
			{
				elements = c.ToArray();
				// c.toArray might (incorrectly) not return Object[] (see 6260652)
				if (elements.GetType() != typeof(Object[]))
				{
					elements = Arrays.CopyOf(elements, elements.Length, typeof(Object[]));
				}
			}
			Array = elements;
		}

		/// <summary>
		/// Creates a list holding a copy of the given array.
		/// </summary>
		/// <param name="toCopyIn"> the array (a copy of this array is used as the
		///        internal array) </param>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		public CopyOnWriteArrayList(E[] toCopyIn)
		{
			Array = Arrays.CopyOf(toCopyIn, toCopyIn.Length, typeof(Object[]));
		}

		/// <summary>
		/// Returns the number of elements in this list.
		/// </summary>
		/// <returns> the number of elements in this list </returns>
		public virtual int Count
		{
			get
			{
				return Array.Length;
			}
		}

		/// <summary>
		/// Returns {@code true} if this list contains no elements.
		/// </summary>
		/// <returns> {@code true} if this list contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Size() == 0;
			}
		}

		/// <summary>
		/// Tests for equality, coping with nulls.
		/// </summary>
		private static bool Eq(Object o1, Object o2)
		{
			return (o1 == null) ? o2 == null : o1.Equals(o2);
		}

		/// <summary>
		/// static version of indexOf, to allow repeated calls without
		/// needing to re-acquire array each time. </summary>
		/// <param name="o"> element to search for </param>
		/// <param name="elements"> the array </param>
		/// <param name="index"> first index to search </param>
		/// <param name="fence"> one past last index to search </param>
		/// <returns> index of element, or -1 if absent </returns>
		private static int IndexOf(Object o, Object[] elements, int index, int fence)
		{
			if (o == null)
			{
				for (int java.util.List_Fields.i = index; java.util.List_Fields.i < fence; java.util.List_Fields.i++)
				{
					if (elements[java.util.List_Fields.i] == null)
					{
						return java.util.List_Fields.i;
					}
				}
			}
			else
			{
				for (int java.util.List_Fields.i = index; java.util.List_Fields.i < fence; java.util.List_Fields.i++)
				{
					if (o.Equals(elements[java.util.List_Fields.i]))
					{
						return java.util.List_Fields.i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// static version of lastIndexOf. </summary>
		/// <param name="o"> element to search for </param>
		/// <param name="elements"> the array </param>
		/// <param name="index"> first index to search </param>
		/// <returns> index of element, or -1 if absent </returns>
		private static int LastIndexOf(Object o, Object[] elements, int index)
		{
			if (o == null)
			{
				for (int java.util.List_Fields.i = index; java.util.List_Fields.i >= 0; java.util.List_Fields.i--)
				{
					if (elements[java.util.List_Fields.i] == null)
					{
						return java.util.List_Fields.i;
					}
				}
			}
			else
			{
				for (int java.util.List_Fields.i = index; java.util.List_Fields.i >= 0; java.util.List_Fields.i--)
				{
					if (o.Equals(elements[java.util.List_Fields.i]))
					{
						return java.util.List_Fields.i;
					}
				}
			}
			return -1;
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
			Object[] elements = Array;
			return IndexOf(o, elements, 0, elements.Length) >= 0;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int IndexOf(Object o)
		{
			Object[] elements = Array;
			return IndexOf(o, elements, 0, elements.Length);
		}

		/// <summary>
		/// Returns the index of the first occurrence of the specified element in
		/// this list, searching forwards from {@code index}, or returns -1 if
		/// the element is not found.
		/// More formally, returns the lowest index {@code i} such that
		/// <tt>(i&nbsp;&gt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(e==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;e.equals(get(i))))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="e"> element to search for </param>
		/// <param name="index"> index to start searching from </param>
		/// <returns> the index of the first occurrence of the element in
		///         this list at position {@code index} or later in the list;
		///         {@code -1} if the element is not found. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		public virtual int IndexOf(E e, int index)
		{
			Object[] elements = Array;
			return IndexOf(e, elements, index, elements.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int LastIndexOf(Object o)
		{
			Object[] elements = Array;
			return LastIndexOf(o, elements, elements.Length - 1);
		}

		/// <summary>
		/// Returns the index of the last occurrence of the specified element in
		/// this list, searching backwards from {@code index}, or returns -1 if
		/// the element is not found.
		/// More formally, returns the highest index {@code i} such that
		/// <tt>(i&nbsp;&lt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(e==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;e.equals(get(i))))</tt>,
		/// or -1 if there is no such index.
		/// </summary>
		/// <param name="e"> element to search for </param>
		/// <param name="index"> index to start searching backwards from </param>
		/// <returns> the index of the last occurrence of the element at position
		///         less than or equal to {@code index} in this list;
		///         -1 if the element is not found. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is greater
		///         than or equal to the current size of this list </exception>
		public virtual int LastIndexOf(E e, int index)
		{
			Object[] elements = Array;
			return LastIndexOf(e, elements, index);
		}

		/// <summary>
		/// Returns a shallow copy of this list.  (The elements themselves
		/// are not copied.)
		/// </summary>
		/// <returns> a clone of this list </returns>
		public virtual Object Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") CopyOnWriteArrayList<E> clone = (CopyOnWriteArrayList<E>) base.clone();
				CopyOnWriteArrayList<E> clone = (CopyOnWriteArrayList<E>) base.Clone();
				clone.ResetLock();
				return clone;
			}
			catch (CloneNotSupportedException)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError();
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
		/// <returns> an array containing all the elements in this list </returns>
		public virtual Object[] ToArray()
		{
			Object[] elements = Array;
			return Arrays.CopyOf(elements, elements.Length);
		}

		/// <summary>
		/// Returns an array containing all of the elements in this list in
		/// proper sequence (from first to last element); the runtime type of
		/// the returned array is that of the specified array.  If the list fits
		/// in the specified array, it is returned therein.  Otherwise, a new
		/// array is allocated with the runtime type of the specified array and
		/// the size of this list.
		/// 
		/// <para>If this list fits in the specified array with room to spare
		/// (i.e., the array has more elements than this list), the element in
		/// the array immediately following the end of the list is set to
		/// {@code null}.  (This is useful in determining the length of this
		/// list <i>only</i> if the caller knows that this list does not contain
		/// any null elements.)
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
		///  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		/// 
		/// Note that {@code toArray(new Object[0])} is identical in function to
		/// {@code toArray()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of the list are to
		///          be stored, if it is big enough; otherwise, a new array of the
		///          same runtime type is allocated for this purpose. </param>
		/// <returns> an array containing all the elements in this list </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this list </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T java.util.List_Fields.a[])
		public virtual T[] toArray<T>(T java)
		{
			Object[] elements = Array;
			int len = elements.Length;
			if (java.util.List_Fields.a.Length < len)
			{
				return (T[]) Arrays.CopyOf(elements, len, java.util.List_Fields.a.GetType());
			}
			else
			{
				System.Array.Copy(elements, 0, java.util.List_Fields.a, 0, len);
				if (java.util.List_Fields.a.Length > len)
				{
					java.util.List_Fields.a[len] = null;
				}
				return java.util.List_Fields.a;
			}
		}

		// Positional Access Operations

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private E get(Object[] java.util.List_Fields.a, int index)
		private E GetJavaToDotNetIndexer(Object[] java, int index)
		{
			get
			{
				return (E) java.util.List_Fields.a[index];
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E GetJavaToDotNetIndexer(int index)
		{
			get
			{
				return Get(Array, index);
			}
		}

		/// <summary>
		/// Replaces the element at the specified position in this list with the
		/// specified element.
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E SetJavaToDotNetIndexer(int index, E element)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				E oldValue = Get(elements, index);

				if (oldValue != element)
				{
					int len = elements.Length;
					Object[] newElements = Arrays.CopyOf(elements, len);
					newElements[index] = element;
					Array = newElements;
				}
				else
				{
					// Not quite a no-op; ensures volatile write semantics
					Array = elements;
				}
				return oldValue;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Appends the specified element to the end of this list.
		/// </summary>
		/// <param name="e"> element to be appended to this list </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		public virtual bool Add(E e)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				Object[] newElements = Arrays.CopyOf(elements, len + 1);
				newElements[len] = e;
				Array = newElements;
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this
		/// list. Shifts the element currently at that position (if any) and
		/// any subsequent elements to the right (adds one to their indices).
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual void Insert(int index, E element)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (index > len || index < 0)
				{
					throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + len);
				}
				Object[] newElements;
				int numMoved = len - index;
				if (numMoved == 0)
				{
					newElements = Arrays.CopyOf(elements, len + 1);
				}
				else
				{
					newElements = new Object[len + 1];
					System.Array.Copy(elements, 0, newElements, 0, index);
					System.Array.Copy(elements, index, newElements, index + 1, numMoved);
				}
				newElements[index] = element;
				Array = newElements;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Removes the element at the specified position in this list.
		/// Shifts any subsequent elements to the left (subtracts one from their
		/// indices).  Returns the element that was removed from the list.
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Remove(int index)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				E oldValue = Get(elements, index);
				int numMoved = len - index - 1;
				if (numMoved == 0)
				{
					Array = Arrays.CopyOf(elements, len - 1);
				}
				else
				{
					Object[] newElements = new Object[len - 1];
					System.Array.Copy(elements, 0, newElements, 0, index);
					System.Array.Copy(elements, index + 1, newElements, index, numMoved);
					Array = newElements;
				}
				return oldValue;
			}
			finally
			{
				@lock.Unlock();
			}
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
			Object[] snapshot = Array;
			int index = IndexOf(o, snapshot, 0, snapshot.Length);
			return (index < 0) ? false : Remove(o, snapshot, index);
		}

		/// <summary>
		/// A version of remove(Object) using the strong hint that given
		/// recent snapshot contains o at the given index.
		/// </summary>
		private bool Remove(Object o, Object[] snapshot, int index)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] current = Array;
				int len = current.Length;
				if (snapshot != current)
				{
				{
					int prefix = System.Math.Min(index, len);
					for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < prefix; java.util.List_Fields.i++)
					{
						if (current[java.util.List_Fields.i] != snapshot[java.util.List_Fields.i] && Eq(o, current[java.util.List_Fields.i]))
						{
							index = java.util.List_Fields.i;
							goto findIndexBreak;
						}
					}
					if (index >= len)
					{
						return false;
					}
					if (current[index] == o)
					{
						goto findIndexBreak;
					}
					index = IndexOf(o, current, index, len);
					if (index < 0)
					{
						return false;
					}
				}
					findIndexBreak:
				}
				Object[] newElements = new Object[len - 1];
				System.Array.Copy(current, 0, newElements, 0, index);
				System.Array.Copy(current, index + 1, newElements, index, len - index - 1);
				Array = newElements;
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Removes from this list all of the elements whose index is between
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		/// Shifts any succeeding elements to the left (reduces their index).
		/// This call shortens the list by {@code (toIndex - fromIndex)} elements.
		/// (If {@code toIndex==fromIndex}, this operation has no effect.)
		/// </summary>
		/// <param name="fromIndex"> index of first element to be removed </param>
		/// <param name="toIndex"> index after last element to be removed </param>
		/// <exception cref="IndexOutOfBoundsException"> if fromIndex or toIndex out of range
		///         ({@code fromIndex < 0 || toIndex > size() || toIndex < fromIndex}) </exception>
		internal virtual void RemoveRange(int fromIndex, int toIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;

				if (fromIndex < 0 || toIndex > len || toIndex < fromIndex)
				{
					throw new IndexOutOfBoundsException();
				}
				int newlen = len - (toIndex - fromIndex);
				int numMoved = len - toIndex;
				if (numMoved == 0)
				{
					Array = Arrays.CopyOf(elements, newlen);
				}
				else
				{
					Object[] newElements = new Object[newlen];
					System.Array.Copy(elements, 0, newElements, 0, fromIndex);
					System.Array.Copy(elements, toIndex, newElements, fromIndex, numMoved);
					Array = newElements;
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Appends the element, if not present.
		/// </summary>
		/// <param name="e"> element to be added to this list, if absent </param>
		/// <returns> {@code true} if the element was added </returns>
		public virtual bool AddIfAbsent(E e)
		{
			Object[] snapshot = Array;
			return IndexOf(e, snapshot, 0, snapshot.Length) >= 0 ? false : AddIfAbsent(e, snapshot);
		}

		/// <summary>
		/// A version of addIfAbsent using the strong hint that given
		/// recent snapshot does not contain e.
		/// </summary>
		private bool AddIfAbsent(E e, Object[] snapshot)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] current = Array;
				int len = current.Length;
				if (snapshot != current)
				{
					// Optimize for lost race to another addXXX operation
					int common = System.Math.Min(snapshot.Length, len);
					for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < common; java.util.List_Fields.i++)
					{
						if (current[java.util.List_Fields.i] != snapshot[java.util.List_Fields.i] && Eq(e, current[java.util.List_Fields.i]))
						{
							return false;
						}
					}
					if (IndexOf(e, current, common, len) >= 0)
					{
							return false;
					}
				}
				Object[] newElements = Arrays.CopyOf(current, len + 1);
				newElements[len] = e;
				Array = newElements;
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns {@code true} if this list contains all of the elements of the
		/// specified collection.
		/// </summary>
		/// <param name="c"> collection to be checked for containment in this list </param>
		/// <returns> {@code true} if this list contains all of the elements of the
		///         specified collection </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool containsAll<T1>(ICollection<T1> c)
		{
			Object[] elements = Array;
			int len = elements.Length;
			foreach (Object e in c)
			{
				if (IndexOf(e, elements, 0, len) < 0)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Removes from this list all of its elements that are contained in
		/// the specified collection. This is a particularly expensive operation
		/// in this class because of the need for an internal temporary array.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this list
		///         is incompatible with the specified collection
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this list contains a null element and the
		///         specified collection does not permit null elements
		///         (<a href="../Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		public virtual bool removeAll<T1>(ICollection<T1> c)
		{
			if (c == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (len != 0)
				{
					// temp array holds those elements we know we want to keep
					int newlen = 0;
					Object[] temp = new Object[len];
					for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
					{
						Object element = elements[java.util.List_Fields.i];
						if (!c.Contains(element))
						{
							temp[newlen++] = element;
						}
					}
					if (newlen != len)
					{
						Array = Arrays.CopyOf(temp, newlen);
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

		/// <summary>
		/// Retains only the elements in this list that are contained in the
		/// specified collection.  In other words, removes from this list all of
		/// its elements that are not contained in the specified collection.
		/// </summary>
		/// <param name="c"> collection containing elements to be retained in this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this list
		///         is incompatible with the specified collection
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this list contains a null element and the
		///         specified collection does not permit null elements
		///         (<a href="../Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		public virtual bool retainAll<T1>(ICollection<T1> c)
		{
			if (c == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (len != 0)
				{
					// temp array holds those elements we know we want to keep
					int newlen = 0;
					Object[] temp = new Object[len];
					for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
					{
						Object element = elements[java.util.List_Fields.i];
						if (c.Contains(element))
						{
							temp[newlen++] = element;
						}
					}
					if (newlen != len)
					{
						Array = Arrays.CopyOf(temp, newlen);
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

		/// <summary>
		/// Appends all of the elements in the specified collection that
		/// are not already contained in this list, to the end of
		/// this list, in the order that they are returned by the
		/// specified collection's iterator.
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> the number of elements added </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #addIfAbsent(Object) </seealso>
		public virtual int addAllAbsent<T1>(ICollection<T1> c) where T1 : E
		{
			Object[] cs = c.ToArray();
			if (cs.Length == 0)
			{
				return 0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				int added = 0;
				// uniquify and compact elements in cs
				for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < cs.Length; ++java.util.List_Fields.i)
				{
					Object e = cs[java.util.List_Fields.i];
					if (IndexOf(e, elements, 0, len) < 0 && IndexOf(e, cs, 0, added) < 0)
					{
						cs[added++] = e;
					}
				}
				if (added > 0)
				{
					Object[] newElements = Arrays.CopyOf(elements, len + added);
					System.Array.Copy(cs, 0, newElements, len, added);
					Array = newElements;
				}
				return added;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Removes all of the elements from this list.
		/// The list will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Array = new Object[0];
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Appends all of the elements in the specified collection to the end
		/// of this list, in the order that they are returned by the specified
		/// collection's iterator.
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #add(Object) </seealso>
		public virtual bool addAll<T1>(ICollection<T1> c) where T1 : E
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Object[] cs = (c.getClass() == CopyOnWriteArrayList.class) ? ((CopyOnWriteArrayList<?>)c).getArray() : c.toArray();
			Object[] cs = (c.GetType() == typeof(CopyOnWriteArrayList)) ? ((CopyOnWriteArrayList<?>)c).Array : c.ToArray();
			if (cs.Length == 0)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (len == 0 && cs.GetType() == typeof(Object[]))
				{
					Array = cs;
				}
				else
				{
					Object[] newElements = Arrays.CopyOf(elements, len + cs.Length);
					System.Array.Copy(cs, 0, newElements, len, cs.Length);
					Array = newElements;
				}
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Inserts all of the elements in the specified collection into this
		/// list, starting at the specified position.  Shifts the element
		/// currently at that position (if any) and any subsequent elements to
		/// the right (increases their indices).  The new elements will appear
		/// in this list in the order that they are returned by the
		/// specified collection's iterator.
		/// </summary>
		/// <param name="index"> index at which to insert the first element
		///        from the specified collection </param>
		/// <param name="c"> collection containing elements to be added to this list </param>
		/// <returns> {@code true} if this list changed as a result of the call </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #add(int,Object) </seealso>
		public virtual bool addAll<T1>(int index, ICollection<T1> c) where T1 : E
		{
			Object[] cs = c.ToArray();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (index > len || index < 0)
				{
					throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + len);
				}
				if (cs.Length == 0)
				{
					return false;
				}
				int numMoved = len - index;
				Object[] newElements;
				if (numMoved == 0)
				{
					newElements = Arrays.CopyOf(elements, len + cs.Length);
				}
				else
				{
					newElements = new Object[len + cs.Length];
					System.Array.Copy(elements, 0, newElements, 0, index);
					System.Array.Copy(elements, index, newElements, index + cs.Length, numMoved);
				}
				System.Array.Copy(cs, 0, newElements, index, cs.Length);
				Array = newElements;
				return true;
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base E> action)
		public virtual void forEach<T1>(Consumer<T1> action)
		{
			if (action == null)
			{
				throw new NullPointerException();
			}
			Object[] elements = Array;
			int len = elements.Length;
			for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
				E e = (E) elements[java.util.List_Fields.i];
				action.Accept(e);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean removeIf(java.util.function.Predicate<? base E> filter)
		public virtual bool removeIf<T1>(Predicate<T1> filter)
		{
			if (filter == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (len != 0)
				{
					int newlen = 0;
					Object[] temp = new Object[len];
					for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
						E e = (E) elements[java.util.List_Fields.i];
						if (!filter.Test(e))
						{
							temp[newlen++] = e;
						}
					}
					if (newlen != len)
					{
						Array = Arrays.CopyOf(temp, newlen);
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

		public virtual void ReplaceAll(UnaryOperator<E> @operator)
		{
			if (@operator == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				Object[] newElements = Arrays.CopyOf(elements, len);
				for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
					E e = (E) elements[java.util.List_Fields.i];
					newElements[java.util.List_Fields.i] = @operator.Apply(e);
				}
				Array = newElements;
			}
			finally
			{
				@lock.Unlock();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void sort(java.util.Comparator<? base E> c)
		public virtual void sort<T1>(IComparer<T1> c)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				Object[] newElements = Arrays.CopyOf(elements, elements.Length);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E[] es = (E[])newElements;
				E[] es = (E[])newElements;
				Arrays.Sort(es, c);
				Array = newElements;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Saves this list to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData The length of the array backing the list is emitted
		///               (int), followed by all of its elements (each an Object)
		///               in the proper order. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{

			s.DefaultWriteObject();

			Object[] elements = Array;
			// Write out array length
			s.WriteInt(elements.Length);

			// Write out all elements in the proper order.
			foreach (Object element in elements)
			{
				s.WriteObject(element);
			}
		}

		/// <summary>
		/// Reconstitutes this list from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{

			s.DefaultReadObject();

			// bind to new lock
			ResetLock();

			// Read in array length and allocate array
			int len = s.ReadInt();
			Object[] elements = new Object[len];

			// Read in all elements in the proper order.
			for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; java.util.List_Fields.i++)
			{
				elements[java.util.List_Fields.i] = s.ReadObject();
			}
			Array = elements;
		}

		/// <summary>
		/// Returns a string representation of this list.  The string
		/// representation consists of the string representations of the list's
		/// elements in the order they are returned by its iterator, enclosed in
		/// square brackets ({@code "[]"}).  Adjacent elements are separated by
		/// the characters {@code ", "} (comma and space).  Elements are
		/// converted to strings as by <seealso cref="String#valueOf(Object)"/>.
		/// </summary>
		/// <returns> a string representation of this list </returns>
		public override String ToString()
		{
			return Arrays.ToString(Array);
		}

		/// <summary>
		/// Compares the specified object with this list for equality.
		/// Returns {@code true} if the specified object is the same object
		/// as this object, or if it is also a <seealso cref="List"/> and the sequence
		/// of elements returned by an <seealso cref="List#iterator() iterator"/>
		/// over the specified list is the same as the sequence returned by
		/// an iterator over this list.  The two sequences are considered to
		/// be the same if they have the same length and corresponding
		/// elements at the same position in the sequence are <em>equal</em>.
		/// Two elements {@code e1} and {@code e2} are considered
		/// <em>equal</em> if {@code (e1==null ? e2==null : e1.equals(e2))}.
		/// </summary>
		/// <param name="o"> the object to be compared for equality with this list </param>
		/// <returns> {@code true} if the specified object is equal to this list </returns>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return true;
			}
			if (!(o is IList))
			{
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> list = (java.util.List<?>)(o);
			IList<?> list = (IList<?>)(o);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> it = list.iterator();
			IEnumerator<?> it = list.GetEnumerator();
			Object[] elements = Array;
			int len = elements.Length;
			for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (!it.hasNext() || !Eq(elements[java.util.List_Fields.i], it.next()))
				{
					return false;
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the hash code value for this list.
		/// 
		/// <para>This implementation uses the definition in <seealso cref="List#hashCode"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this list </returns>
		public override int HashCode()
		{
			int hashCode = 1;
			Object[] elements = Array;
			int len = elements.Length;
			for (int java.util.List_Fields.i = 0; java.util.List_Fields.i < len; ++java.util.List_Fields.i)
			{
				Object obj = elements[java.util.List_Fields.i];
				hashCode = 31 * hashCode + (obj == null ? 0 : obj.HashCode());
			}
			return hashCode;
		}

		/// <summary>
		/// Returns an iterator over the elements in this list in proper sequence.
		/// 
		/// <para>The returned iterator provides a snapshot of the state of the list
		/// when the iterator was constructed. No synchronization is needed while
		/// traversing the iterator. The iterator does <em>NOT</em> support the
		/// {@code remove} method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this list in proper sequence </returns>
		public virtual IEnumerator<E> GetEnumerator()
		{
			return new COWIterator<E>(Array, 0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>The returned iterator provides a snapshot of the state of the list
		/// when the iterator was constructed. No synchronization is needed while
		/// traversing the iterator. The iterator does <em>NOT</em> support the
		/// {@code remove}, {@code set} or {@code add} methods.
		/// </para>
		/// </summary>
		public virtual IEnumerator<E> ListIterator()
		{
			return new COWIterator<E>(Array, 0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>The returned iterator provides a snapshot of the state of the list
		/// when the iterator was constructed. No synchronization is needed while
		/// traversing the iterator. The iterator does <em>NOT</em> support the
		/// {@code remove}, {@code set} or {@code add} methods.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual IEnumerator<E> ListIterator(int index)
		{
			Object[] elements = Array;
			int len = elements.Length;
			if (index < 0 || index > len)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}

			return new COWIterator<E>(elements, index);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> over the elements in this list.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#IMMUTABLE"/>,
		/// <seealso cref="Spliterator#ORDERED"/>, <seealso cref="Spliterator#SIZED"/>, and
		/// <seealso cref="Spliterator#SUBSIZED"/>.
		/// 
		/// </para>
		/// <para>The spliterator provides a snapshot of the state of the list
		/// when the spliterator was constructed. No synchronization is needed while
		/// operating on the spliterator.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this list
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return Spliterators.Spliterator(Array, java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.ORDERED);
		}

		internal sealed class COWIterator<E> : ListIterator<E>
		{
			/// <summary>
			/// Snapshot of the array </summary>
			internal readonly Object[] Snapshot;
			/// <summary>
			/// Index of element to be returned by subsequent call to next. </summary>
			internal int Cursor;

			internal COWIterator(Object[] elements, int initialCursor)
			{
				Cursor = initialCursor;
				Snapshot = elements;
			}

			public bool HasNext()
			{
				return Cursor < Snapshot.Length;
			}

			public bool HasPrevious()
			{
				return Cursor > 0;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
			public E Next()
			{
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}
				return (E) Snapshot[Cursor++];
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E previous()
			public E Previous()
			{
				if (!HasPrevious())
				{
					throw new NoSuchElementException();
				}
				return (E) Snapshot[--Cursor];
			}

			public int NextIndex()
			{
				return Cursor;
			}

			public int PreviousIndex()
			{
				return Cursor - 1;
			}

			/// <summary>
			/// Not supported. Always throws UnsupportedOperationException. </summary>
			/// <exception cref="UnsupportedOperationException"> always; {@code remove}
			///         is not supported by this iterator. </exception>
			public void Remove()
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// Not supported. Always throws UnsupportedOperationException. </summary>
			/// <exception cref="UnsupportedOperationException"> always; {@code set}
			///         is not supported by this iterator. </exception>
			public void Set(E e)
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// Not supported. Always throws UnsupportedOperationException. </summary>
			/// <exception cref="UnsupportedOperationException"> always; {@code add}
			///         is not supported by this iterator. </exception>
			public void Add(E e)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				Object[] elements = Snapshot;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = elements.length;
				int size = elements.Length;
				for (int java.util.List_Fields.i = Cursor; java.util.List_Fields.i < size; java.util.List_Fields.i++)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
					E e = (E) elements[java.util.List_Fields.i];
					action.Accept(e);
				}
				Cursor = size;
			}
		}

		/// <summary>
		/// Returns a view of the portion of this list between
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		/// The returned list is backed by this list, so changes in the
		/// returned list are reflected in this list.
		/// 
		/// <para>The semantics of the list returned by this method become
		/// undefined if the backing list (i.e., this list) is modified in
		/// any way other than via the returned list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> low endpoint (inclusive) of the subList </param>
		/// <param name="toIndex"> high endpoint (exclusive) of the subList </param>
		/// <returns> a view of the specified range within this list </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual IList<E> SubList(int fromIndex, int toIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				Object[] elements = Array;
				int len = elements.Length;
				if (fromIndex < 0 || toIndex > len || fromIndex > toIndex)
				{
					throw new IndexOutOfBoundsException();
				}
				return new COWSubList<E>(this, fromIndex, toIndex);
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Sublist for CopyOnWriteArrayList.
		/// This class extends AbstractList merely for convenience, to
		/// avoid having to define addAll, etc. This doesn't hurt, but
		/// is wasteful.  This class does not need or use modCount
		/// mechanics in AbstractList, but does need to check for
		/// concurrent modification using similar mechanics.  On each
		/// operation, the array that we expect the backing list to use
		/// is checked and updated.  Since we do this for all of the
		/// base operations invoked by those defined in AbstractList,
		/// all is well.  While inefficient, this is not worth
		/// improving.  The kinds of list operations inherited from
		/// AbstractList are already so slow on COW sublists that
		/// adding a bit more space/time doesn't seem even noticeable.
		/// </summary>
		private class COWSubList<E> : AbstractList<E>, RandomAccess
		{
			internal readonly CopyOnWriteArrayList<E> l;
			internal readonly int Offset;
			internal int Size_Renamed;
			internal Object[] ExpectedArray;

			// only call this holding l's lock
			internal COWSubList(CopyOnWriteArrayList<E> list, int fromIndex, int toIndex)
			{
				l = list;
				ExpectedArray = l.Array;
				Offset = fromIndex;
				Size_Renamed = toIndex - fromIndex;
			}

			// only call this holding l's lock
			internal virtual void CheckForComodification()
			{
				if (l.Array != ExpectedArray)
				{
					throw new ConcurrentModificationException();
				}
			}

			// only call this holding l's lock
			internal virtual void RangeCheck(int index)
			{
				if (index < 0 || index >= Size_Renamed)
				{
					throw new IndexOutOfBoundsException("Index: " + index + ",Size: " + Size_Renamed);
				}
			}

			public virtual E Set(int index, E element)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					RangeCheck(index);
					CheckForComodification();
					E x = l[index + Offset] = element;
					ExpectedArray = l.Array;
					return x;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual E Get(int index)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					RangeCheck(index);
					CheckForComodification();
					return l[index + Offset];
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual int Size()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					return Size_Renamed;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual void Add(int index, E element)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					if (index < 0 || index > Size_Renamed)
					{
						throw new IndexOutOfBoundsException();
					}
					l.Insert(index + Offset, element);
					ExpectedArray = l.Array;
					Size_Renamed++;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual void Clear()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					l.RemoveRange(Offset, Offset + Size_Renamed - Offset);
					ExpectedArray = l.Array;
					Size_Renamed = 0;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual E Remove(int index)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					RangeCheck(index);
					CheckForComodification();
					E result = l.Remove(index + Offset);
					ExpectedArray = l.Array;
					Size_Renamed--;
					return result;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool Remove(Object o)
			{
				int index = IndexOf(o);
				if (index == -1)
				{
					return false;
				}
				Remove(index);
				return true;
			}

			public virtual IEnumerator<E> Iterator()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					return new COWSubListIterator<E>(l, 0, Offset, Size_Renamed);
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual IEnumerator<E> ListIterator(int index)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					if (index < 0 || index > Size_Renamed)
					{
						throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + Size_Renamed);
					}
					return new COWSubListIterator<E>(l, index, Offset, Size_Renamed);
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual IList<E> SubList(int fromIndex, int toIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					CheckForComodification();
					if (fromIndex < 0 || toIndex > Size_Renamed || fromIndex > toIndex)
					{
						throw new IndexOutOfBoundsException();
					}
					return new COWSubList<E>(l, fromIndex + Offset, toIndex + Offset);
				}
				finally
				{
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base E> action)
			public virtual void forEach<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				int lo = Offset;
				int hi = Offset + Size_Renamed;
				Object[] java.util.List_Fields.a = ExpectedArray;
				if (l.Array != java.util.List_Fields.a)
				{
					throw new ConcurrentModificationException();
				}
				if (lo < 0 || hi > java.util.List_Fields.a.Length)
				{
					throw new IndexOutOfBoundsException();
				}
				for (int java.util.List_Fields.i = lo; java.util.List_Fields.i < hi; ++java.util.List_Fields.i)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) java.util.List_Fields.a[java.util.List_Fields.i];
					E e = (E) java.util.List_Fields.a[java.util.List_Fields.i];
					action.Accept(e);
				}
			}

			public virtual void ReplaceAll(UnaryOperator<E> @operator)
			{
				if (@operator == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					int lo = Offset;
					int hi = Offset + Size_Renamed;
					Object[] elements = ExpectedArray;
					if (l.Array != elements)
					{
						throw new ConcurrentModificationException();
					}
					int len = elements.Length;
					if (lo < 0 || hi > len)
					{
						throw new IndexOutOfBoundsException();
					}
					Object[] newElements = Arrays.CopyOf(elements, len);
					for (int java.util.List_Fields.i = lo; java.util.List_Fields.i < hi; ++java.util.List_Fields.i)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
						E e = (E) elements[java.util.List_Fields.i];
						newElements[java.util.List_Fields.i] = @operator.Apply(e);
					}
					l.Array = ExpectedArray = newElements;
				}
				finally
				{
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void sort(java.util.Comparator<? base E> c)
			public virtual void sort<T1>(IComparer<T1> c)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					int lo = Offset;
					int hi = Offset + Size_Renamed;
					Object[] elements = ExpectedArray;
					if (l.Array != elements)
					{
						throw new ConcurrentModificationException();
					}
					int len = elements.Length;
					if (lo < 0 || hi > len)
					{
						throw new IndexOutOfBoundsException();
					}
					Object[] newElements = Arrays.CopyOf(elements, len);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E[] es = (E[])newElements;
					E[] es = (E[])newElements;
					Arrays.Sort(es, lo, hi, c);
					l.Array = ExpectedArray = newElements;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool removeAll<T1>(ICollection<T1> c)
			{
				if (c == null)
				{
					throw new NullPointerException();
				}
				bool java.util.Collection_Fields.Removed = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					int n = Size_Renamed;
					if (n > 0)
					{
						int lo = Offset;
						int hi = Offset + n;
						Object[] elements = ExpectedArray;
						if (l.Array != elements)
						{
							throw new ConcurrentModificationException();
						}
						int len = elements.Length;
						if (lo < 0 || hi > len)
						{
							throw new IndexOutOfBoundsException();
						}
						int newSize = 0;
						Object[] temp = new Object[n];
						for (int java.util.List_Fields.i = lo; java.util.List_Fields.i < hi; ++java.util.List_Fields.i)
						{
							Object element = elements[java.util.List_Fields.i];
							if (!c.Contains(element))
							{
								temp[newSize++] = element;
							}
						}
						if (newSize != n)
						{
							Object[] newElements = new Object[len - n + newSize];
							System.Array.Copy(elements, 0, newElements, 0, lo);
							System.Array.Copy(temp, 0, newElements, lo, newSize);
							System.Array.Copy(elements, hi, newElements, lo + newSize, len - hi);
							Size_Renamed = newSize;
							java.util.Collection_Fields.Removed = true;
							l.Array = ExpectedArray = newElements;
						}
					}
				}
				finally
				{
					@lock.Unlock();
				}
				return java.util.Collection_Fields.Removed;
			}

			public virtual bool retainAll<T1>(ICollection<T1> c)
			{
				if (c == null)
				{
					throw new NullPointerException();
				}
				bool java.util.Collection_Fields.Removed = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					int n = Size_Renamed;
					if (n > 0)
					{
						int lo = Offset;
						int hi = Offset + n;
						Object[] elements = ExpectedArray;
						if (l.Array != elements)
						{
							throw new ConcurrentModificationException();
						}
						int len = elements.Length;
						if (lo < 0 || hi > len)
						{
							throw new IndexOutOfBoundsException();
						}
						int newSize = 0;
						Object[] temp = new Object[n];
						for (int java.util.List_Fields.i = lo; java.util.List_Fields.i < hi; ++java.util.List_Fields.i)
						{
							Object element = elements[java.util.List_Fields.i];
							if (c.Contains(element))
							{
								temp[newSize++] = element;
							}
						}
						if (newSize != n)
						{
							Object[] newElements = new Object[len - n + newSize];
							System.Array.Copy(elements, 0, newElements, 0, lo);
							System.Array.Copy(temp, 0, newElements, lo, newSize);
							System.Array.Copy(elements, hi, newElements, lo + newSize, len - hi);
							Size_Renamed = newSize;
							java.util.Collection_Fields.Removed = true;
							l.Array = ExpectedArray = newElements;
						}
					}
				}
				finally
				{
					@lock.Unlock();
				}
				return java.util.Collection_Fields.Removed;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean removeIf(java.util.function.Predicate<? base E> filter)
			public virtual bool removeIf<T1>(Predicate<T1> filter)
			{
				if (filter == null)
				{
					throw new NullPointerException();
				}
				bool java.util.Collection_Fields.Removed = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = l.lock;
				ReentrantLock @lock = l.@lock;
				@lock.@lock();
				try
				{
					int n = Size_Renamed;
					if (n > 0)
					{
						int lo = Offset;
						int hi = Offset + n;
						Object[] elements = ExpectedArray;
						if (l.Array != elements)
						{
							throw new ConcurrentModificationException();
						}
						int len = elements.Length;
						if (lo < 0 || hi > len)
						{
							throw new IndexOutOfBoundsException();
						}
						int newSize = 0;
						Object[] temp = new Object[n];
						for (int java.util.List_Fields.i = lo; java.util.List_Fields.i < hi; ++java.util.List_Fields.i)
						{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) elements[java.util.List_Fields.i];
							E e = (E) elements[java.util.List_Fields.i];
							if (!filter.Test(e))
							{
								temp[newSize++] = e;
							}
						}
						if (newSize != n)
						{
							Object[] newElements = new Object[len - n + newSize];
							System.Array.Copy(elements, 0, newElements, 0, lo);
							System.Array.Copy(temp, 0, newElements, lo, newSize);
							System.Array.Copy(elements, hi, newElements, lo + newSize, len - hi);
							Size_Renamed = newSize;
							java.util.Collection_Fields.Removed = true;
							l.Array = ExpectedArray = newElements;
						}
					}
				}
				finally
				{
					@lock.Unlock();
				}
				return java.util.Collection_Fields.Removed;
			}

			public virtual Spliterator<E> Spliterator()
			{
				int lo = Offset;
				int hi = Offset + Size_Renamed;
				Object[] java.util.List_Fields.a = ExpectedArray;
				if (l.Array != java.util.List_Fields.a)
				{
					throw new ConcurrentModificationException();
				}
				if (lo < 0 || hi > java.util.List_Fields.a.Length)
				{
					throw new IndexOutOfBoundsException();
				}
				return Spliterators.Spliterator(java.util.List_Fields.a, lo, hi, java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.ORDERED);
			}

		}

		private class COWSubListIterator<E> : ListIterator<E>
		{
			internal readonly IEnumerator<E> It;
			internal readonly int Offset;
			internal readonly int Size;

			internal COWSubListIterator(IList<E> l, int index, int offset, int size)
			{
				this.Offset = offset;
				this.Size = size;
				It = l.listIterator(index + offset);
			}

			public virtual bool HasNext()
			{
				return NextIndex() < Size;
			}

			public virtual E Next()
			{
				if (HasNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return It.next();
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual bool HasPrevious()
			{
				return PreviousIndex() >= 0;
			}

			public virtual E Previous()
			{
				if (HasPrevious())
				{
					return It.previous();
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual int NextIndex()
			{
				return It.nextIndex() - Offset;
			}

			public virtual int PreviousIndex()
			{
				return It.previousIndex() - Offset;
			}

			public virtual void Remove()
			{
				throw new UnsupportedOperationException();
			}

			public virtual void Set(E e)
			{
				throw new UnsupportedOperationException();
			}

			public virtual void Add(E e)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base E> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				int s = Size;
				IEnumerator<E> java.util.List_Fields.i = It;
				while (NextIndex() < s)
				{
					action.Accept(java.util.List_Fields.i.Next());
				}
			}
		}

		// Support for resetting lock while deserializing
		private void ResetLock()
		{
			UNSAFE.putObjectVolatile(this, LockOffset, new ReentrantLock());
		}
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long LockOffset;
		static CopyOnWriteArrayList()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(CopyOnWriteArrayList);
				LockOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("lock"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}