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
	/// This class provides a skeletal implementation of the <tt>Collection</tt>
	/// interface, to minimize the effort required to implement this interface. <para>
	/// 
	/// To implement an unmodifiable collection, the programmer needs only to
	/// extend this class and provide implementations for the <tt>iterator</tt> and
	/// <tt>size</tt> methods.  (The iterator returned by the <tt>iterator</tt>
	/// </para>
	/// method must implement <tt>hasNext</tt> and <tt>next</tt>.)<para>
	/// 
	/// To implement a modifiable collection, the programmer must additionally
	/// override this class's <tt>add</tt> method (which otherwise throws an
	/// <tt>UnsupportedOperationException</tt>), and the iterator returned by the
	/// <tt>iterator</tt> method must additionally implement its <tt>remove</tt>
	/// </para>
	/// method.<para>
	/// 
	/// The programmer should generally provide a void (no argument) and
	/// <tt>Collection</tt> constructor, as per the recommendation in the
	/// </para>
	/// <tt>Collection</tt> interface specification.<para>
	/// 
	/// The documentation for each non-abstract method in this class describes its
	/// implementation in detail.  Each of these methods may be overridden if
	/// </para>
	/// the collection being implemented admits a more efficient implementation.<para>
	/// 
	/// This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// </para>
	/// </summary>
	/// <seealso cref= Collection
	/// @since 1.2 </seealso>

	public abstract class AbstractCollection<E> : Collection<E>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal AbstractCollection()
		{
		}

		// Query Operations

		/// <summary>
		/// Returns an iterator over the elements contained in this collection.
		/// </summary>
		/// <returns> an iterator over the elements contained in this collection </returns>
		public abstract Iterator<E> Iterator();

		public abstract int Size();

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns <tt>size() == 0</tt>.
		/// </para>
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return Size() == 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over the elements in the collection,
		/// checking each element in turn for equality with the specified element.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">   {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool Contains(Object o)
		{
			Iterator<E> it = Iterator();
			if (o == null)
			{
				while (it.HasNext())
				{
					if (it.Next() == null)
					{
						return true;
					}
				}
			}
			else
			{
				while (it.HasNext())
				{
					if (o.Equals(it.Next()))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns an array containing all the elements
		/// returned by this collection's iterator, in the same order, stored in
		/// consecutive elements of the array, starting with index {@code 0}.
		/// The length of the returned array is equal to the number of elements
		/// returned by the iterator, even if the size of this collection changes
		/// during iteration, as might happen if the collection permits
		/// concurrent modification during iteration.  The {@code size} method is
		/// called only as an optimization hint; the correct result is returned
		/// even if the iterator returns a different number of elements.
		/// 
		/// </para>
		/// <para>This method is equivalent to:
		/// 
		///  <pre> {@code
		/// List<E> list = new ArrayList<E>(size());
		/// for (E e : this)
		///     list.add(e);
		/// return list.toArray();
		/// }</pre>
		/// </para>
		/// </summary>
		public virtual Object[] ToArray()
		{
			// Estimate size of array; be prepared to see more or fewer elements
			Object[] r = new Object[Size()];
			Iterator<E> it = Iterator();
			for (int i = 0; i < r.Length; i++)
			{
				if (!it.HasNext()) // fewer elements than expected
				{
					return Arrays.CopyOf(r, i);
				}
				r[i] = it.Next();
			}
			return it.HasNext() ? FinishToArray(r, it) : r;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns an array containing all the elements
		/// returned by this collection's iterator in the same order, stored in
		/// consecutive elements of the array, starting with index {@code 0}.
		/// If the number of elements returned by the iterator is too large to
		/// fit into the specified array, then the elements are returned in a
		/// newly allocated array with length equal to the number of elements
		/// returned by the iterator, even if the size of this collection
		/// changes during iteration, as might happen if the collection permits
		/// concurrent modification during iteration.  The {@code size} method is
		/// called only as an optimization hint; the correct result is returned
		/// even if the iterator returns a different number of elements.
		/// 
		/// </para>
		/// <para>This method is equivalent to:
		/// 
		///  <pre> {@code
		/// List<E> list = new ArrayList<E>(size());
		/// for (E e : this)
		///     list.add(e);
		/// return list.toArray(a);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ArrayStoreException">  {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
		public virtual T[] toArray<T>(T[] a)
		{
			// Estimate size of array; be prepared to see more or fewer elements
			int size = Size();
			T[] r = a.Length >= size ? a : (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), size);
			Iterator<E> it = Iterator();

			for (int i = 0; i < r.Length; i++)
			{
				if (!it.HasNext()) // fewer elements than expected
				{
					if (a == r)
					{
						r[i] = null; // null-terminate
					}
					else if (a.Length < i)
					{
						return Arrays.CopyOf(r, i);
					}
					else
					{
						System.Array.Copy(r, 0, a, 0, i);
						if (a.Length > i)
						{
							a[i] = null;
						}
					}
					return a;
				}
				r[i] = (T)it.Next();
			}
			// more elements than expected
			return it.HasNext() ? FinishToArray(r, it) : r;
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Reallocates the array being used within toArray when the iterator
		/// returned more elements than expected, and finishes filling it from
		/// the iterator.
		/// </summary>
		/// <param name="r"> the array, replete with previously stored elements </param>
		/// <param name="it"> the in-progress iterator over this collection </param>
		/// <returns> array containing the elements in the given array, plus any
		///         further elements returned by the iterator, trimmed to size </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> T[] finishToArray(T[] r, Iterator<?> it)
		private static T[] finishToArray<T, T1>(T[] r, Iterator<T1> it)
		{
			int i = r.Length;
			while (it.HasNext())
			{
				int cap = r.Length;
				if (i == cap)
				{
					int newCap = cap + (cap >> 1) + 1;
					// overflow-conscious code
					if (newCap - MAX_ARRAY_SIZE > 0)
					{
						newCap = HugeCapacity(cap + 1);
					}
					r = Arrays.CopyOf(r, newCap);
				}
				r[i++] = (T)it.Next();
			}
			// trim if overallocated
			return (i == r.Length) ? r : Arrays.CopyOf(r, i);
		}

		private static int HugeCapacity(int minCapacity)
		{
			if (minCapacity < 0) // overflow
			{
				throw new OutOfMemoryError("Required array size too large");
			}
			return (minCapacity > MAX_ARRAY_SIZE) ? Integer.MaxValue : MAX_ARRAY_SIZE;
		}

		// Modification Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation always throws an
		/// <tt>UnsupportedOperationException</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		/// <exception cref="IllegalStateException">         {@inheritDoc} </exception>
		public virtual bool Add(E e)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over the collection looking for the
		/// specified element.  If it finds the element, it removes the element
		/// from the collection using the iterator's remove method.
		/// 
		/// </para>
		/// <para>Note that this implementation throws an
		/// <tt>UnsupportedOperationException</tt> if the iterator returned by this
		/// collection's iterator method does not implement the <tt>remove</tt>
		/// method and this collection contains the specified object.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		public virtual bool Remove(Object o)
		{
			Iterator<E> it = Iterator();
			if (o == null)
			{
				while (it.HasNext())
				{
					if (it.Next() == null)
					{
						it.remove();
						return true;
					}
				}
			}
			else
			{
				while (it.HasNext())
				{
					if (o.Equals(it.Next()))
					{
						it.remove();
						return true;
					}
				}
			}
			return false;
		}


		// Bulk Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over the specified collection,
		/// checking each element returned by the iterator in turn to see
		/// if it's contained in this collection.  If all elements are so
		/// contained <tt>true</tt> is returned, otherwise <tt>false</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool containsAll<T1>(Collection<T1> c)
		{
			foreach (Object e in c)
			{
				if (!Contains(e))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over the specified collection, and adds
		/// each object returned by the iterator to this collection, in turn.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> unless <tt>add</tt> is
		/// overridden (assuming the specified collection is non-empty).
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		/// <exception cref="IllegalStateException">         {@inheritDoc}
		/// </exception>
		/// <seealso cref= #add(Object) </seealso>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			bool modified = false;
			foreach (E e in c)
			{
				if (Add(e))
				{
					modified = true;
				}
			}
			return modified;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over this collection, checking each
		/// element returned by the iterator in turn to see if it's contained
		/// in the specified collection.  If it's so contained, it's removed from
		/// this collection with the iterator's <tt>remove</tt> method.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the iterator returned by the
		/// <tt>iterator</tt> method does not implement the <tt>remove</tt> method
		/// and this collection contains one or more elements in common with the
		/// specified collection.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc}
		/// </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			Objects.RequireNonNull(c);
			bool modified = false;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<?> it = iterator();
			Iterator<?> it = Iterator();
			while (it.HasNext())
			{
				if (c.Contains(it.Next()))
				{
					it.remove();
					modified = true;
				}
			}
			return modified;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over this collection, checking each
		/// element returned by the iterator in turn to see if it's contained
		/// in the specified collection.  If it's not so contained, it's removed
		/// from this collection with the iterator's <tt>remove</tt> method.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the iterator returned by the
		/// <tt>iterator</tt> method does not implement the <tt>remove</tt> method
		/// and this collection contains one or more elements not present in the
		/// specified collection.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc}
		/// </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool retainAll<T1>(Collection<T1> c)
		{
			Objects.RequireNonNull(c);
			bool modified = false;
			Iterator<E> it = Iterator();
			while (it.HasNext())
			{
				if (!c.Contains(it.Next()))
				{
					it.remove();
					modified = true;
				}
			}
			return modified;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation iterates over this collection, removing each
		/// element using the <tt>Iterator.remove</tt> operation.  Most
		/// implementations will probably choose to override this method for
		/// efficiency.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the iterator returned by this
		/// collection's <tt>iterator</tt> method does not implement the
		/// <tt>remove</tt> method and this collection is non-empty.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		public virtual void Clear()
		{
			Iterator<E> it = Iterator();
			while (it.HasNext())
			{
				it.Next();
				it.remove();
			}
		}


		//  String conversion

		/// <summary>
		/// Returns a string representation of this collection.  The string
		/// representation consists of a list of the collection's elements in the
		/// order they are returned by its iterator, enclosed in square brackets
		/// (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
		/// <tt>", "</tt> (comma and space).  Elements are converted to strings as
		/// by <seealso cref="String#valueOf(Object)"/>.
		/// </summary>
		/// <returns> a string representation of this collection </returns>
		public override String ToString()
		{
			Iterator<E> it = Iterator();
			if (!it.HasNext())
			{
				return "[]";
			}

			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			for (;;)
			{
				E e = it.Next();
				sb.Append(e == this ? "(this Collection)" : e);
				if (!it.HasNext())
				{
					return sb.Append(']').ToString();
				}
				sb.Append(',').Append(' ');
			}
		}

	}

}