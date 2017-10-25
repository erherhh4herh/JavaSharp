/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// An iterator for lists that allows the programmer
	/// to traverse the list in either direction, modify
	/// the list during iteration, and obtain the iterator's
	/// current position in the list. A {@code ListIterator}
	/// has no current element; its <I>cursor position</I> always
	/// lies between the element that would be returned by a call
	/// to {@code previous()} and the element that would be
	/// returned by a call to {@code next()}.
	/// An iterator for a list of length {@code n} has {@code n+1} possible
	/// cursor positions, as illustrated by the carets ({@code ^}) below:
	/// <PRE>
	///                      Element(0)   Element(1)   Element(2)   ... Element(n-1)
	/// cursor positions:  ^            ^            ^            ^                  ^
	/// </PRE>
	/// Note that the <seealso cref="#remove"/> and <seealso cref="#set(Object)"/> methods are
	/// <i>not</i> defined in terms of the cursor position;  they are defined to
	/// operate on the last element returned by a call to <seealso cref="#next"/> or
	/// <seealso cref="#previous()"/>.
	/// 
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= List </seealso>
	/// <seealso cref= Iterator </seealso>
	/// <seealso cref= Enumeration </seealso>
	/// <seealso cref= List#listIterator()
	/// @since   1.2 </seealso>
	public interface ListIterator<E> : Iterator<E>
	{
		// Query Operations

		/// <summary>
		/// Returns {@code true} if this list iterator has more elements when
		/// traversing the list in the forward direction. (In other words,
		/// returns {@code true} if <seealso cref="#next"/> would return an element rather
		/// than throwing an exception.)
		/// </summary>
		/// <returns> {@code true} if the list iterator has more elements when
		///         traversing the list in the forward direction </returns>
		bool HasNext();

		/// <summary>
		/// Returns the next element in the list and advances the cursor position.
		/// This method may be called repeatedly to iterate through the list,
		/// or intermixed with calls to <seealso cref="#previous"/> to go back and forth.
		/// (Note that alternating calls to {@code next} and {@code previous}
		/// will return the same element repeatedly.)
		/// </summary>
		/// <returns> the next element in the list </returns>
		/// <exception cref="NoSuchElementException"> if the iteration has no next element </exception>
		E Next();

		/// <summary>
		/// Returns {@code true} if this list iterator has more elements when
		/// traversing the list in the reverse direction.  (In other words,
		/// returns {@code true} if <seealso cref="#previous"/> would return an element
		/// rather than throwing an exception.)
		/// </summary>
		/// <returns> {@code true} if the list iterator has more elements when
		///         traversing the list in the reverse direction </returns>
		bool HasPrevious();

		/// <summary>
		/// Returns the previous element in the list and moves the cursor
		/// position backwards.  This method may be called repeatedly to
		/// iterate through the list backwards, or intermixed with calls to
		/// <seealso cref="#next"/> to go back and forth.  (Note that alternating calls
		/// to {@code next} and {@code previous} will return the same
		/// element repeatedly.)
		/// </summary>
		/// <returns> the previous element in the list </returns>
		/// <exception cref="NoSuchElementException"> if the iteration has no previous
		///         element </exception>
		E Previous();

		/// <summary>
		/// Returns the index of the element that would be returned by a
		/// subsequent call to <seealso cref="#next"/>. (Returns list size if the list
		/// iterator is at the end of the list.)
		/// </summary>
		/// <returns> the index of the element that would be returned by a
		///         subsequent call to {@code next}, or list size if the list
		///         iterator is at the end of the list </returns>
		int NextIndex();

		/// <summary>
		/// Returns the index of the element that would be returned by a
		/// subsequent call to <seealso cref="#previous"/>. (Returns -1 if the list
		/// iterator is at the beginning of the list.)
		/// </summary>
		/// <returns> the index of the element that would be returned by a
		///         subsequent call to {@code previous}, or -1 if the list
		///         iterator is at the beginning of the list </returns>
		int PreviousIndex();


		// Modification Operations

		/// <summary>
		/// Removes from the list the last element that was returned by {@link
		/// #next} or <seealso cref="#previous"/> (optional operation).  This call can
		/// only be made once per call to {@code next} or {@code previous}.
		/// It can be made only if <seealso cref="#add"/> has not been
		/// called after the last call to {@code next} or {@code previous}.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the {@code remove}
		///         operation is not supported by this list iterator </exception>
		/// <exception cref="IllegalStateException"> if neither {@code next} nor
		///         {@code previous} have been called, or {@code remove} or
		///         {@code add} have been called after the last call to
		///         {@code next} or {@code previous} </exception>
		void Remove();

		/// <summary>
		/// Replaces the last element returned by <seealso cref="#next"/> or
		/// <seealso cref="#previous"/> with the specified element (optional operation).
		/// This call can be made only if neither <seealso cref="#remove"/> nor {@link
		/// #add} have been called after the last call to {@code next} or
		/// {@code previous}.
		/// </summary>
		/// <param name="e"> the element with which to replace the last element returned by
		///          {@code next} or {@code previous} </param>
		/// <exception cref="UnsupportedOperationException"> if the {@code set} operation
		///         is not supported by this list iterator </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this list </exception>
		/// <exception cref="IllegalArgumentException"> if some aspect of the specified
		///         element prevents it from being added to this list </exception>
		/// <exception cref="IllegalStateException"> if neither {@code next} nor
		///         {@code previous} have been called, or {@code remove} or
		///         {@code add} have been called after the last call to
		///         {@code next} or {@code previous} </exception>
		void Set(E e);

		/// <summary>
		/// Inserts the specified element into the list (optional operation).
		/// The element is inserted immediately before the element that
		/// would be returned by <seealso cref="#next"/>, if any, and after the element
		/// that would be returned by <seealso cref="#previous"/>, if any.  (If the
		/// list contains no elements, the new element becomes the sole element
		/// on the list.)  The new element is inserted before the implicit
		/// cursor: a subsequent call to {@code next} would be unaffected, and a
		/// subsequent call to {@code previous} would return the new element.
		/// (This call increases by one the value that would be returned by a
		/// call to {@code nextIndex} or {@code previousIndex}.)
		/// </summary>
		/// <param name="e"> the element to insert </param>
		/// <exception cref="UnsupportedOperationException"> if the {@code add} method is
		///         not supported by this list iterator </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this list </exception>
		/// <exception cref="IllegalArgumentException"> if some aspect of this element
		///         prevents it from being added to this list </exception>
		void Add(E e);
	}

}