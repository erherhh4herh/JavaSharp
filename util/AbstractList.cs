using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// This class provides a skeletal implementation of the <seealso cref="List"/>
	/// interface to minimize the effort required to implement this interface
	/// backed by a "random access" data store (such as an array).  For sequential
	/// access data (such as a linked list), <seealso cref="AbstractSequentialList"/> should
	/// be used in preference to this class.
	/// 
	/// <para>To implement an unmodifiable list, the programmer needs only to extend
	/// this class and provide implementations for the <seealso cref="#get(int)"/> and
	/// <seealso cref="List#size() size()"/> methods.
	/// 
	/// </para>
	/// <para>To implement a modifiable list, the programmer must additionally
	/// override the <seealso cref="#set(int, Object) set(int, E)"/> method (which otherwise
	/// throws an {@code UnsupportedOperationException}).  If the list is
	/// variable-size the programmer must additionally override the
	/// <seealso cref="#add(int, Object) add(int, E)"/> and <seealso cref="#remove(int)"/> methods.
	/// 
	/// </para>
	/// <para>The programmer should generally provide a void (no argument) and collection
	/// constructor, as per the recommendation in the <seealso cref="Collection"/> interface
	/// specification.
	/// 
	/// </para>
	/// <para>Unlike the other abstract collection implementations, the programmer does
	/// <i>not</i> have to provide an iterator implementation; the iterator and
	/// list iterator are implemented by this class, on top of the "random access"
	/// methods:
	/// <seealso cref="#get(int)"/>,
	/// <seealso cref="#set(int, Object) set(int, E)"/>,
	/// <seealso cref="#add(int, Object) add(int, E)"/> and
	/// <seealso cref="#remove(int)"/>.
	/// 
	/// </para>
	/// <para>The documentation for each non-abstract method in this class describes its
	/// implementation in detail.  Each of these methods may be overridden if the
	/// collection being implemented admits a more efficient implementation.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// @since 1.2
	/// </para>
	/// </summary>

	public abstract class AbstractList<E> : AbstractCollection<E>, List<E>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal AbstractList()
		{
		}

		/// <summary>
		/// Appends the specified element to the end of this list (optional
		/// operation).
		/// 
		/// <para>Lists that support this operation may place limitations on what
		/// elements may be added to this list.  In particular, some
		/// lists will refuse to add null elements, and others will impose
		/// restrictions on the type of elements that may be added.  List
		/// classes should clearly specify in their documentation any restrictions
		/// on what elements may be added.
		/// 
		/// </para>
		/// <para>This implementation calls {@code add(size(), e)}.
		/// 
		/// </para>
		/// <para>Note that this implementation throws an
		/// {@code UnsupportedOperationException} unless
		/// <seealso cref="#add(int, Object) add(int, E)"/> is overridden.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> element to be appended to this list </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="UnsupportedOperationException"> if the {@code add} operation
		///         is not supported by this list </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this list </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         list does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of this element
		///         prevents it from being added to this list </exception>
		public virtual bool Add(E e)
		{
			Add(Size(), e);
			return true;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public abstract E Get(int index);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation always throws an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		/// <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		public virtual E Set(int index, E element)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation always throws an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		/// <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		public virtual void Add(int index, E element)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation always throws an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		public virtual E Remove(int index)
		{
			throw new UnsupportedOperationException();
		}


		// Search Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation first gets a list iterator (with
		/// {@code listIterator()}).  Then, it iterates over the list until the
		/// specified element is found or the end of the list is reached.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">   {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual int IndexOf(Object o)
		{
			ListIterator<E> it = ListIterator();
			if (o == null)
			{
				while (it.HasNext())
				{
					if (it.Next() == null)
					{
						return it.PreviousIndex();
					}
				}
			}
			else
			{
				while (it.HasNext())
				{
					if (o.Equals(it.Next()))
					{
						return it.PreviousIndex();
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation first gets a list iterator that points to the end
		/// of the list (with {@code listIterator(size())}).  Then, it iterates
		/// backwards over the list until the specified element is found, or the
		/// beginning of the list is reached.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">   {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual int LastIndexOf(Object o)
		{
			ListIterator<E> it = ListIterator(Size());
			if (o == null)
			{
				while (it.HasPrevious())
				{
					if (it.Previous() == null)
					{
						return it.NextIndex();
					}
				}
			}
			else
			{
				while (it.HasPrevious())
				{
					if (o.Equals(it.Previous()))
					{
						return it.NextIndex();
					}
				}
			}
			return -1;
		}


		// Bulk Operations

		/// <summary>
		/// Removes all of the elements from this list (optional operation).
		/// The list will be empty after this call returns.
		/// 
		/// <para>This implementation calls {@code removeRange(0, size())}.
		/// 
		/// </para>
		/// <para>Note that this implementation throws an
		/// {@code UnsupportedOperationException} unless {@code remove(int
		/// index)} or {@code removeRange(int fromIndex, int toIndex)} is
		/// overridden.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the {@code clear} operation
		///         is not supported by this list </exception>
		public virtual void Clear()
		{
			RemoveRange(0, Size());
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation gets an iterator over the specified collection
		/// and iterates over it, inserting the elements obtained from the
		/// iterator into this list at the appropriate position, one at a time,
		/// using {@code add(int, E)}.
		/// Many implementations will override this method for efficiency.
		/// 
		/// </para>
		/// <para>Note that this implementation throws an
		/// {@code UnsupportedOperationException} unless
		/// <seealso cref="#add(int, Object) add(int, E)"/> is overridden.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		/// <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		public virtual bool addAll<T1>(int index, Collection<T1> c) where T1 : E
		{
			RangeCheckForAdd(index);
			bool modified = false;
			foreach (E e in c)
			{
				Add(index++, e);
				modified = true;
			}
			return modified;
		}


		// Iterators

		/// <summary>
		/// Returns an iterator over the elements in this list in proper sequence.
		/// 
		/// <para>This implementation returns a straightforward implementation of the
		/// iterator interface, relying on the backing list's {@code size()},
		/// {@code get(int)}, and {@code remove(int)} methods.
		/// 
		/// </para>
		/// <para>Note that the iterator returned by this method will throw an
		/// <seealso cref="UnsupportedOperationException"/> in response to its
		/// {@code remove} method unless the list's {@code remove(int)} method is
		/// overridden.
		/// 
		/// </para>
		/// <para>This implementation can be made to throw runtime exceptions in the
		/// face of concurrent modification, as described in the specification
		/// for the (protected) <seealso cref="#modCount"/> field.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this list in proper sequence </returns>
		public virtual Iterator<E> GetEnumerator()
		{
			return new Itr(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns {@code listIterator(0)}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #listIterator(int) </seealso>
		public virtual ListIterator<E> ListIterator()
		{
			return ListIterator(0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns a straightforward implementation of the
		/// {@code ListIterator} interface that extends the implementation of the
		/// {@code Iterator} interface returned by the {@code iterator()} method.
		/// The {@code ListIterator} implementation relies on the backing list's
		/// {@code get(int)}, {@code set(int, E)}, {@code add(int, E)}
		/// and {@code remove(int)} methods.
		/// 
		/// </para>
		/// <para>Note that the list iterator returned by this implementation will
		/// throw an <seealso cref="UnsupportedOperationException"/> in response to its
		/// {@code remove}, {@code set} and {@code add} methods unless the
		/// list's {@code remove(int)}, {@code set(int, E)}, and
		/// {@code add(int, E)} methods are overridden.
		/// 
		/// </para>
		/// <para>This implementation can be made to throw runtime exceptions in the
		/// face of concurrent modification, as described in the specification for
		/// the (protected) <seealso cref="#modCount"/> field.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ListIterator<E> listIterator(final int index)
		public virtual ListIterator<E> ListIterator(int index)
		{
			RangeCheckForAdd(index);

			return new ListItr(this, index);
		}

		private class Itr : Iterator<E>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly AbstractList<E> OuterInstance;

			public Itr(AbstractList<E> outerInstance)
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
			internal int Cursor = 0;

			/// <summary>
			/// Index of element returned by most recent call to next or
			/// previous.  Reset to -1 if this element is deleted by a call
			/// to remove.
			/// </summary>
			internal int LastRet = -1;

			/// <summary>
			/// The modCount value that the iterator believes that the backing
			/// List should have.  If this expectation is violated, the iterator
			/// has detected concurrent modification.
			/// </summary>
			internal int ExpectedModCount;

			public virtual bool HasNext()
			{
				return Cursor != outerInstance.Size();
			}

			public virtual E Next()
			{
				CheckForComodification();
				try
				{
					int List_Fields.i = Cursor;
					E next = outerInstance.Get(List_Fields.i);
					LastRet = List_Fields.i;
					Cursor = List_Fields.i + 1;
					return next;
				}
				catch (IndexOutOfBoundsException)
				{
					CheckForComodification();
					throw new NoSuchElementException();
				}
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
					if (LastRet < Cursor)
					{
						Cursor--;
					}
					LastRet = -1;
					ExpectedModCount = outerInstance.ModCount;
				}
				catch (IndexOutOfBoundsException)
				{
					throw new ConcurrentModificationException();
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

		private class ListItr : Itr, ListIterator<E>
		{
			private readonly AbstractList<E> OuterInstance;

			internal ListItr(AbstractList<E> outerInstance, int index) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				Cursor = index;
			}

			public virtual bool HasPrevious()
			{
				return Cursor != 0;
			}

			public virtual E Previous()
			{
				CheckForComodification();
				try
				{
					int List_Fields.i = Cursor - 1;
					E previous = outerInstance.Get(List_Fields.i);
					LastRet = Cursor = List_Fields.i;
					return previous;
				}
				catch (IndexOutOfBoundsException)
				{
					CheckForComodification();
					throw new NoSuchElementException();
				}
			}

			public virtual int NextIndex()
			{
				return Cursor;
			}

			public virtual int PreviousIndex()
			{
				return Cursor - 1;
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
					ExpectedModCount = outerInstance.ModCount;
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
					LastRet = -1;
					Cursor = List_Fields.i + 1;
					ExpectedModCount = outerInstance.ModCount;
				}
				catch (IndexOutOfBoundsException)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>This implementation returns a list that subclasses
		/// {@code AbstractList}.  The subclass stores, in private fields, the
		/// offset of the subList within the backing list, the size of the subList
		/// (which can change over its lifetime), and the expected
		/// {@code modCount} value of the backing list.  There are two variants
		/// of the subclass, one of which implements {@code RandomAccess}.
		/// If this list implements {@code RandomAccess} the returned list will
		/// be an instance of the subclass that implements {@code RandomAccess}.
		/// 
		/// </para>
		/// <para>The subclass's {@code set(int, E)}, {@code get(int)},
		/// {@code add(int, E)}, {@code remove(int)}, {@code addAll(int,
		/// Collection)} and {@code removeRange(int, int)} methods all
		/// delegate to the corresponding methods on the backing abstract list,
		/// after bounds-checking the index and adjusting for the offset.  The
		/// {@code addAll(Collection c)} method merely returns {@code addAll(size,
		/// c)}.
		/// 
		/// </para>
		/// <para>The {@code listIterator(int)} method returns a "wrapper object"
		/// over a list iterator on the backing list, which is created with the
		/// corresponding method on the backing list.  The {@code iterator} method
		/// merely returns {@code listIterator()}, and the {@code size} method
		/// merely returns the subclass's {@code size} field.
		/// 
		/// </para>
		/// <para>All methods first check to see if the actual {@code modCount} of
		/// the backing list is equal to its expected value, and throw a
		/// {@code ConcurrentModificationException} if it is not.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> if an endpoint index value is out of range
		///         {@code (fromIndex < 0 || toIndex > size)} </exception>
		/// <exception cref="IllegalArgumentException"> if the endpoint indices are out of order
		///         {@code (fromIndex > toIndex)} </exception>
		public virtual List<E> SubList(int fromIndex, int toIndex)
		{
			return (this is RandomAccess ? new RandomAccessSubList<>(this, fromIndex, toIndex) : new SubList<>(this, fromIndex, toIndex));
		}

		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this list for equality.  Returns
		/// {@code true} if and only if the specified object is also a list, both
		/// lists have the same size, and all corresponding pairs of elements in
		/// the two lists are <i>equal</i>.  (Two elements {@code e1} and
		/// {@code e2} are <i>equal</i> if {@code (e1==null ? e2==null :
		/// e1.equals(e2))}.)  In other words, two lists are defined to be
		/// equal if they contain the same elements in the same order.<para>
		/// 
		/// This implementation first checks if the specified object is this
		/// list. If so, it returns {@code true}; if not, it checks if the
		/// specified object is a list. If not, it returns {@code false}; if so,
		/// it iterates over both lists, comparing corresponding pairs of elements.
		/// If any comparison returns {@code false}, this method returns
		/// {@code false}.  If either iterator runs out of elements before the
		/// other it returns {@code false} (as the lists are of unequal length);
		/// otherwise it returns {@code true} when the iterations complete.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> the object to be compared for equality with this list </param>
		/// <returns> {@code true} if the specified object is equal to this list </returns>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return true;
			}
			if (!(o is List))
			{
				return false;
			}

			ListIterator<E> e1 = ListIterator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<?> e2 = ((List<?>) o).listIterator();
			ListIterator<?> e2 = ((List<?>) o).ListIterator();
			while (e1.HasNext() && e2.HasNext())
			{
				E o1 = e1.Next();
				Object o2 = e2.Next();
				if (!(o1 == null ? o2 == null : o1.Equals(o2)))
				{
					return false;
				}
			}
			return !(e1.HasNext() || e2.HasNext());
		}

		/// <summary>
		/// Returns the hash code value for this list.
		/// 
		/// <para>This implementation uses exactly the code that is used to define the
		/// list hash function in the documentation for the <seealso cref="List#hashCode"/>
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this list </returns>
		public override int HashCode()
		{
			int hashCode = 1;
			foreach (E e in this)
			{
				hashCode = 31 * hashCode + (e == null ? 0 : e.HashCode());
			}
			return hashCode;
		}

		/// <summary>
		/// Removes from this list all of the elements whose index is between
		/// {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		/// Shifts any succeeding elements to the left (reduces their index).
		/// This call shortens the list by {@code (toIndex - fromIndex)} elements.
		/// (If {@code toIndex==fromIndex}, this operation has no effect.)
		/// 
		/// <para>This method is called by the {@code clear} operation on this list
		/// and its subLists.  Overriding this method to take advantage of
		/// the internals of the list implementation can <i>substantially</i>
		/// improve the performance of the {@code clear} operation on this list
		/// and its subLists.
		/// 
		/// </para>
		/// <para>This implementation gets a list iterator positioned before
		/// {@code fromIndex}, and repeatedly calls {@code ListIterator.next}
		/// followed by {@code ListIterator.remove} until the entire range has
		/// been removed.  <b>Note: if {@code ListIterator.remove} requires linear
		/// time, this implementation requires quadratic time.</b>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> index of first element to be removed </param>
		/// <param name="toIndex"> index after last element to be removed </param>
		protected internal virtual void RemoveRange(int fromIndex, int toIndex)
		{
			ListIterator<E> it = ListIterator(fromIndex);
			for (int List_Fields.i = 0, n = toIndex - fromIndex; List_Fields.i < n; List_Fields.i++)
			{
				it.Next();
				it.Remove();
			}
		}

		/// <summary>
		/// The number of times this list has been <i>structurally modified</i>.
		/// Structural modifications are those that change the size of the
		/// list, or otherwise perturb it in such a fashion that iterations in
		/// progress may yield incorrect results.
		/// 
		/// <para>This field is used by the iterator and list iterator implementation
		/// returned by the {@code iterator} and {@code listIterator} methods.
		/// If the value of this field changes unexpectedly, the iterator (or list
		/// iterator) will throw a {@code ConcurrentModificationException} in
		/// response to the {@code next}, {@code remove}, {@code previous},
		/// {@code set} or {@code add} operations.  This provides
		/// <i>fail-fast</i> behavior, rather than non-deterministic behavior in
		/// the face of concurrent modification during iteration.
		/// 
		/// </para>
		/// <para><b>Use of this field by subclasses is optional.</b> If a subclass
		/// wishes to provide fail-fast iterators (and list iterators), then it
		/// merely has to increment this field in its {@code add(int, E)} and
		/// {@code remove(int)} methods (and any other methods that it overrides
		/// that result in structural modifications to the list).  A single call to
		/// {@code add(int, E)} or {@code remove(int)} must add no more than
		/// one to this field, or the iterators (and list iterators) will throw
		/// bogus {@code ConcurrentModificationExceptions}.  If an implementation
		/// does not wish to provide fail-fast iterators, this field may be
		/// ignored.
		/// </para>
		/// </summary>
		[NonSerialized]
		protected internal int ModCount = 0;

		private void RangeCheckForAdd(int index)
		{
			if (index < 0 || index > Size())
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		private String OutOfBoundsMsg(int index)
		{
			return "Index: " + index + ", Size: " + Size();
		}
	}

	internal class SubList<E> : AbstractList<E>
	{
		private readonly AbstractList<E> l;
		private readonly int Offset;
		private int Size_Renamed;

		internal SubList(AbstractList<E> list, int fromIndex, int toIndex)
		{
			if (fromIndex < 0)
			{
				throw new IndexOutOfBoundsException("fromIndex = " + fromIndex);
			}
			if (toIndex > list.Size())
			{
				throw new IndexOutOfBoundsException("toIndex = " + toIndex);
			}
			if (fromIndex > toIndex)
			{
				throw new IllegalArgumentException("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
			}
			l = list;
			Offset = fromIndex;
			Size_Renamed = toIndex - fromIndex;
			this.ModCount = l.ModCount;
		}

		public virtual E Set(int index, E element)
		{
			RangeCheck(index);
			CheckForComodification();
			return l.Set(index + Offset, element);
		}

		public virtual E Get(int index)
		{
			RangeCheck(index);
			CheckForComodification();
			return l.Get(index + Offset);
		}

		public virtual int Size()
		{
			CheckForComodification();
			return Size_Renamed;
		}

		public virtual void Add(int index, E element)
		{
			RangeCheckForAdd(index);
			CheckForComodification();
			l.Add(index + Offset, element);
			this.ModCount = l.ModCount;
			Size_Renamed++;
		}

		public virtual E Remove(int index)
		{
			RangeCheck(index);
			CheckForComodification();
			E result = l.Remove(index + Offset);
			this.ModCount = l.ModCount;
			Size_Renamed--;
			return result;
		}

		protected internal virtual void RemoveRange(int fromIndex, int toIndex)
		{
			CheckForComodification();
			l.RemoveRange(fromIndex + Offset, toIndex + Offset);
			this.ModCount = l.ModCount;
			Size_Renamed -= (toIndex - fromIndex);
		}

		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			return AddAll(Size_Renamed, c);
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
			l.AddAll(Offset + index, c);
			this.ModCount = l.ModCount;
			Size_Renamed += cSize;
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

			return new ListIteratorAnonymousInnerClassHelper(this, index);
		}

		private class ListIteratorAnonymousInnerClassHelper : ListIterator<E>
		{
			private readonly SubList<E> OuterInstance;

			private int Index;

			public ListIteratorAnonymousInnerClassHelper(SubList<E> outerInstance, int index)
			{
				this.outerInstance = outerInstance;
				this.Index = index;
				List_Fields.i = outerInstance.l.ListIterator(index + outerInstance.Offset);
			}

			private readonly ListIterator<E> List_Fields;

			public virtual bool HasNext()
			{
				return nextIndex() < OuterInstance.Size_Renamed;
			}

			public virtual E Next()
			{
				if (hasNext())
				{
					return List_Fields.i.Next();
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual bool HasPrevious()
			{
				return previousIndex() >= 0;
			}

			public virtual E Previous()
			{
				if (hasPrevious())
				{
					return List_Fields.i.Previous();
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual int NextIndex()
			{
				return List_Fields.i.NextIndex() - OuterInstance.Offset;
			}

			public virtual int PreviousIndex()
			{
				return List_Fields.i.PreviousIndex() - OuterInstance.Offset;
			}

			public virtual void Remove()
			{
				List_Fields.i.Remove();
				OuterInstance.ModCount = OuterInstance.l.ModCount;
				OuterInstance.Size_Renamed--;
			}

			public virtual void Set(E e)
			{
				List_Fields.i.Set(e);
			}

			public virtual void Add(E e)
			{
				List_Fields.i.Add(e);
				OuterInstance.ModCount = OuterInstance.l.ModCount;
				OuterInstance.Size_Renamed++;
			}
		}

		public virtual List<E> SubList(int fromIndex, int toIndex)
		{
			return new SubList<>(this, fromIndex, toIndex);
		}

		private void RangeCheck(int index)
		{
			if (index < 0 || index >= Size_Renamed)
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		private void RangeCheckForAdd(int index)
		{
			if (index < 0 || index > Size_Renamed)
			{
				throw new IndexOutOfBoundsException(OutOfBoundsMsg(index));
			}
		}

		private String OutOfBoundsMsg(int index)
		{
			return "Index: " + index + ", Size: " + Size_Renamed;
		}

		private void CheckForComodification()
		{
			if (this.ModCount != l.ModCount)
			{
				throw new ConcurrentModificationException();
			}
		}
	}

	internal class RandomAccessSubList<E> : SubList<E>, RandomAccess
	{
		internal RandomAccessSubList(AbstractList<E> list, int fromIndex, int toIndex) : base(list, fromIndex, toIndex)
		{
		}

		public virtual List<E> SubList(int fromIndex, int toIndex)
		{
			return new RandomAccessSubList<>(this, fromIndex, toIndex);
		}
	}

}