﻿/*
 * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// This class provides a skeletal implementation of the <tt>List</tt>
	/// interface to minimize the effort required to implement this interface
	/// backed by a "sequential access" data store (such as a linked list).  For
	/// random access data (such as an array), <tt>AbstractList</tt> should be used
	/// in preference to this class.<para>
	/// 
	/// This class is the opposite of the <tt>AbstractList</tt> class in the sense
	/// that it implements the "random access" methods (<tt>get(int index)</tt>,
	/// <tt>set(int index, E element)</tt>, <tt>add(int index, E element)</tt> and
	/// <tt>remove(int index)</tt>) on top of the list's list iterator, instead of
	/// </para>
	/// the other way around.<para>
	/// 
	/// To implement a list the programmer needs only to extend this class and
	/// provide implementations for the <tt>listIterator</tt> and <tt>size</tt>
	/// methods.  For an unmodifiable list, the programmer need only implement the
	/// list iterator's <tt>hasNext</tt>, <tt>next</tt>, <tt>hasPrevious</tt>,
	/// </para>
	/// <tt>previous</tt> and <tt>index</tt> methods.<para>
	/// 
	/// For a modifiable list the programmer should additionally implement the list
	/// iterator's <tt>set</tt> method.  For a variable-size list the programmer
	/// should additionally implement the list iterator's <tt>remove</tt> and
	/// </para>
	/// <tt>add</tt> methods.<para>
	/// 
	/// The programmer should generally provide a void (no argument) and collection
	/// constructor, as per the recommendation in the <tt>Collection</tt> interface
	/// </para>
	/// specification.<para>
	/// 
	/// This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// </para>
	/// </summary>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= List </seealso>
	/// <seealso cref= AbstractList </seealso>
	/// <seealso cref= AbstractCollection
	/// @since 1.2 </seealso>

	public abstract class AbstractSequentialList<E> : AbstractList<E>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal AbstractSequentialList()
		{
		}

		/// <summary>
		/// Returns the element at the specified position in this list.
		/// 
		/// <para>This implementation first gets a list iterator pointing to the
		/// indexed element (with <tt>listIterator(index)</tt>).  Then, it gets
		/// the element using <tt>ListIterator.next</tt> and returns it.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public virtual E Get(int index)
		{
			try
			{
				return ListIterator(index).Next();
			}
			catch (NoSuchElementException)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
		}

		/// <summary>
		/// Replaces the element at the specified position in this list with the
		/// specified element (optional operation).
		/// 
		/// <para>This implementation first gets a list iterator pointing to the
		/// indexed element (with <tt>listIterator(index)</tt>).  Then, it gets
		/// the current element using <tt>ListIterator.next</tt> and replaces it
		/// with <tt>ListIterator.set</tt>.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the list iterator does not
		/// implement the <tt>set</tt> operation.
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
			try
			{
				ListIterator<E> e = ListIterator(index);
				E oldVal = e.Next();
				e.Set(element);
				return oldVal;
			}
			catch (NoSuchElementException)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this list
		/// (optional operation).  Shifts the element currently at that position
		/// (if any) and any subsequent elements to the right (adds one to their
		/// indices).
		/// 
		/// <para>This implementation first gets a list iterator pointing to the
		/// indexed element (with <tt>listIterator(index)</tt>).  Then, it
		/// inserts the specified element with <tt>ListIterator.add</tt>.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the list iterator does not
		/// implement the <tt>add</tt> operation.
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
			try
			{
				ListIterator(index).Add(element);
			}
			catch (NoSuchElementException)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
		}

		/// <summary>
		/// Removes the element at the specified position in this list (optional
		/// operation).  Shifts any subsequent elements to the left (subtracts one
		/// from their indices).  Returns the element that was removed from the
		/// list.
		/// 
		/// <para>This implementation first gets a list iterator pointing to the
		/// indexed element (with <tt>listIterator(index)</tt>).  Then, it removes
		/// the element with <tt>ListIterator.remove</tt>.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the list iterator does not
		/// implement the <tt>remove</tt> operation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		public virtual E Remove(int index)
		{
			try
			{
				ListIterator<E> e = ListIterator(index);
				E outCast = e.Next();
				e.Remove();
				return outCast;
			}
			catch (NoSuchElementException)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
		}


		// Bulk Operations

		/// <summary>
		/// Inserts all of the elements in the specified collection into this
		/// list at the specified position (optional operation).  Shifts the
		/// element currently at that position (if any) and any subsequent
		/// elements to the right (increases their indices).  The new elements
		/// will appear in this list in the order that they are returned by the
		/// specified collection's iterator.  The behavior of this operation is
		/// undefined if the specified collection is modified while the
		/// operation is in progress.  (Note that this will occur if the specified
		/// collection is this list, and it's nonempty.)
		/// 
		/// <para>This implementation gets an iterator over the specified collection and
		/// a list iterator over this list pointing to the indexed element (with
		/// <tt>listIterator(index)</tt>).  Then, it iterates over the specified
		/// collection, inserting the elements obtained from the iterator into this
		/// list, one at a time, using <tt>ListIterator.add</tt> followed by
		/// <tt>ListIterator.next</tt> (to skip over the added element).
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the list iterator returned by
		/// the <tt>listIterator</tt> method does not implement the <tt>add</tt>
		/// operation.
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
			try
			{
				bool modified = false;
				ListIterator<E> e1 = ListIterator(index);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends E> e2 = c.iterator();
				Iterator<?> e2 = c.Iterator();
				while (e2.HasNext())
				{
					e1.Add(e2.Next());
					modified = true;
				}
				return modified;
			}
			catch (NoSuchElementException)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}
		}


		// Iterators

		/// <summary>
		/// Returns an iterator over the elements in this list (in proper
		/// sequence).<para>
		/// 
		/// This implementation merely returns a list iterator over the list.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this list (in proper sequence) </returns>
		public virtual Iterator<E> Iterator()
		{
			return ListIterator();
		}

		/// <summary>
		/// Returns a list iterator over the elements in this list (in proper
		/// sequence).
		/// </summary>
		/// <param name="index"> index of first element to be returned from the list
		///         iterator (by a call to the <code>next</code> method) </param>
		/// <returns> a list iterator over the elements in this list (in proper
		///         sequence) </returns>
		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public abstract ListIterator<E> ListIterator(int index);
	}

}