﻿using System.Collections.Generic;

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
 * Written by Doug Lea and Josh Bloch with assistance from members of
 * JCP JSR-166 Expert Group and released to the public domain, as explained
 * at http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util
{

	/// <summary>
	/// A linear collection that supports element insertion and removal at
	/// both ends.  The name <i>deque</i> is short for "double ended queue"
	/// and is usually pronounced "deck".  Most {@code Deque}
	/// implementations place no fixed limits on the number of elements
	/// they may contain, but this interface supports capacity-restricted
	/// deques as well as those with no fixed size limit.
	/// 
	/// <para>This interface defines methods to access the elements at both
	/// ends of the deque.  Methods are provided to insert, remove, and
	/// examine the element.  Each of these methods exists in two forms:
	/// one throws an exception if the operation fails, the other returns a
	/// special value (either {@code null} or {@code false}, depending on
	/// the operation).  The latter form of the insert operation is
	/// designed specifically for use with capacity-restricted
	/// {@code Deque} implementations; in most implementations, insert
	/// operations cannot fail.
	/// 
	/// </para>
	/// <para>The twelve methods described above are summarized in the
	/// following table:
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Summary of Deque methods</caption>
	///  <tr>
	///    <td></td>
	///    <td ALIGN=CENTER COLSPAN = 2> <b>First Element (Head)</b></td>
	///    <td ALIGN=CENTER COLSPAN = 2> <b>Last Element (Tail)</b></td>
	///  </tr>
	///  <tr>
	///    <td></td>
	///    <td ALIGN=CENTER><em>Throws exception</em></td>
	///    <td ALIGN=CENTER><em>Special value</em></td>
	///    <td ALIGN=CENTER><em>Throws exception</em></td>
	///    <td ALIGN=CENTER><em>Special value</em></td>
	///  </tr>
	///  <tr>
	///    <td><b>Insert</b></td>
	///    <td><seealso cref="Deque#addFirst addFirst(e)"/></td>
	///    <td><seealso cref="Deque#offerFirst offerFirst(e)"/></td>
	///    <td><seealso cref="Deque#addLast addLast(e)"/></td>
	///    <td><seealso cref="Deque#offerLast offerLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Remove</b></td>
	///    <td><seealso cref="Deque#removeFirst removeFirst()"/></td>
	///    <td><seealso cref="Deque#pollFirst pollFirst()"/></td>
	///    <td><seealso cref="Deque#removeLast removeLast()"/></td>
	///    <td><seealso cref="Deque#pollLast pollLast()"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Examine</b></td>
	///    <td><seealso cref="Deque#getFirst getFirst()"/></td>
	///    <td><seealso cref="Deque#peekFirst peekFirst()"/></td>
	///    <td><seealso cref="Deque#getLast getLast()"/></td>
	///    <td><seealso cref="Deque#peekLast peekLast()"/></td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>This interface extends the <seealso cref="Queue"/> interface.  When a deque is
	/// used as a queue, FIFO (First-In-First-Out) behavior results.  Elements are
	/// added at the end of the deque and removed from the beginning.  The methods
	/// inherited from the {@code Queue} interface are precisely equivalent to
	/// {@code Deque} methods as indicated in the following table:
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Comparison of Queue and Deque methods</caption>
	///  <tr>
	///    <td ALIGN=CENTER> <b>{@code Queue} Method</b></td>
	///    <td ALIGN=CENTER> <b>Equivalent {@code Deque} Method</b></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#add add(e)"/></td>
	///    <td><seealso cref="#addLast addLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#offer offer(e)"/></td>
	///    <td><seealso cref="#offerLast offerLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#remove remove()"/></td>
	///    <td><seealso cref="#removeFirst removeFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#poll poll()"/></td>
	///    <td><seealso cref="#pollFirst pollFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#element element()"/></td>
	///    <td><seealso cref="#getFirst getFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="java.util.Queue#peek peek()"/></td>
	///    <td><seealso cref="#peek peekFirst()"/></td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>Deques can also be used as LIFO (Last-In-First-Out) stacks.  This
	/// interface should be used in preference to the legacy <seealso cref="Stack"/> class.
	/// When a deque is used as a stack, elements are pushed and popped from the
	/// beginning of the deque.  Stack methods are precisely equivalent to
	/// {@code Deque} methods as indicated in the table below:
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Comparison of Stack and Deque methods</caption>
	///  <tr>
	///    <td ALIGN=CENTER> <b>Stack Method</b></td>
	///    <td ALIGN=CENTER> <b>Equivalent {@code Deque} Method</b></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#push push(e)"/></td>
	///    <td><seealso cref="#addFirst addFirst(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#pop pop()"/></td>
	///    <td><seealso cref="#removeFirst removeFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#peek peek()"/></td>
	///    <td><seealso cref="#peekFirst peekFirst()"/></td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>Note that the <seealso cref="#peek peek"/> method works equally well when
	/// a deque is used as a queue or a stack; in either case, elements are
	/// drawn from the beginning of the deque.
	/// 
	/// </para>
	/// <para>This interface provides two methods to remove interior
	/// elements, <seealso cref="#removeFirstOccurrence removeFirstOccurrence"/> and
	/// <seealso cref="#removeLastOccurrence removeLastOccurrence"/>.
	/// 
	/// </para>
	/// <para>Unlike the <seealso cref="List"/> interface, this interface does not
	/// provide support for indexed access to elements.
	/// 
	/// </para>
	/// <para>While {@code Deque} implementations are not strictly required
	/// to prohibit the insertion of null elements, they are strongly
	/// encouraged to do so.  Users of any {@code Deque} implementations
	/// that do allow null elements are strongly encouraged <i>not</i> to
	/// take advantage of the ability to insert nulls.  This is so because
	/// {@code null} is used as a special return value by various methods
	/// to indicated that the deque is empty.
	/// 
	/// </para>
	/// <para>{@code Deque} implementations generally do not define
	/// element-based versions of the {@code equals} and {@code hashCode}
	/// methods, but instead inherit the identity-based versions from class
	/// {@code Object}.
	/// 
	/// </para>
	/// <para>This interface is a member of the <a
	/// href="{@docRoot}/../technotes/guides/collections/index.html"> Java Collections
	/// Framework</a>.
	/// 
	/// @author Doug Lea
	/// @author Josh Bloch
	/// @since  1.6
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	public interface Deque<E> : Queue<E>
	{
		/// <summary>
		/// Inserts the specified element at the front of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// throwing an {@code IllegalStateException} if no space is currently
		/// available.  When using a capacity-restricted deque, it is generally
		/// preferable to use method <seealso cref="#offerFirst"/>.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="IllegalStateException"> if the element cannot be added at this
		///         time due to capacity restrictions </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		void AddFirst(E e);

		/// <summary>
		/// Inserts the specified element at the end of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// throwing an {@code IllegalStateException} if no space is currently
		/// available.  When using a capacity-restricted deque, it is generally
		/// preferable to use method <seealso cref="#offerLast"/>.
		/// 
		/// <para>This method is equivalent to <seealso cref="#add"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="IllegalStateException"> if the element cannot be added at this
		///         time due to capacity restrictions </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		void AddLast(E e);

		/// <summary>
		/// Inserts the specified element at the front of this deque unless it would
		/// violate capacity restrictions.  When using a capacity-restricted deque,
		/// this method is generally preferable to the <seealso cref="#addFirst"/> method,
		/// which can fail to insert an element only by throwing an exception.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} if the element was added to this deque, else
		///         {@code false} </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		bool OfferFirst(E e);

		/// <summary>
		/// Inserts the specified element at the end of this deque unless it would
		/// violate capacity restrictions.  When using a capacity-restricted deque,
		/// this method is generally preferable to the <seealso cref="#addLast"/> method,
		/// which can fail to insert an element only by throwing an exception.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} if the element was added to this deque, else
		///         {@code false} </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		bool OfferLast(E e);

		/// <summary>
		/// Retrieves and removes the first element of this deque.  This method
		/// differs from <seealso cref="#pollFirst pollFirst"/> only in that it throws an
		/// exception if this deque is empty.
		/// </summary>
		/// <returns> the head of this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E RemoveFirst();

		/// <summary>
		/// Retrieves and removes the last element of this deque.  This method
		/// differs from <seealso cref="#pollLast pollLast"/> only in that it throws an
		/// exception if this deque is empty.
		/// </summary>
		/// <returns> the tail of this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E RemoveLast();

		/// <summary>
		/// Retrieves and removes the first element of this deque,
		/// or returns {@code null} if this deque is empty.
		/// </summary>
		/// <returns> the head of this deque, or {@code null} if this deque is empty </returns>
		E PollFirst();

		/// <summary>
		/// Retrieves and removes the last element of this deque,
		/// or returns {@code null} if this deque is empty.
		/// </summary>
		/// <returns> the tail of this deque, or {@code null} if this deque is empty </returns>
		E PollLast();

		/// <summary>
		/// Retrieves, but does not remove, the first element of this deque.
		/// 
		/// This method differs from <seealso cref="#peekFirst peekFirst"/> only in that it
		/// throws an exception if this deque is empty.
		/// </summary>
		/// <returns> the head of this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E First {get;}

		/// <summary>
		/// Retrieves, but does not remove, the last element of this deque.
		/// This method differs from <seealso cref="#peekLast peekLast"/> only in that it
		/// throws an exception if this deque is empty.
		/// </summary>
		/// <returns> the tail of this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E Last {get;}

		/// <summary>
		/// Retrieves, but does not remove, the first element of this deque,
		/// or returns {@code null} if this deque is empty.
		/// </summary>
		/// <returns> the head of this deque, or {@code null} if this deque is empty </returns>
		E PeekFirst();

		/// <summary>
		/// Retrieves, but does not remove, the last element of this deque,
		/// or returns {@code null} if this deque is empty.
		/// </summary>
		/// <returns> the tail of this deque, or {@code null} if this deque is empty </returns>
		E PeekLast();

		/// <summary>
		/// Removes the first occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>
		/// (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		bool RemoveFirstOccurrence(Object o);

		/// <summary>
		/// Removes the last occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the last element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>
		/// (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		bool RemoveLastOccurrence(Object o);

		// *** Queue methods ***

		/// <summary>
		/// Inserts the specified element into the queue represented by this deque
		/// (in other words, at the tail of this deque) if it is possible to do so
		/// immediately without violating capacity restrictions, returning
		/// {@code true} upon success and throwing an
		/// {@code IllegalStateException} if no space is currently available.
		/// When using a capacity-restricted deque, it is generally preferable to
		/// use <seealso cref="#offer(Object) offer"/>.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		/// <exception cref="IllegalStateException"> if the element cannot be added at this
		///         time due to capacity restrictions </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		bool Add(E e);

		/// <summary>
		/// Inserts the specified element into the queue represented by this deque
		/// (in other words, at the tail of this deque) if it is possible to do so
		/// immediately without violating capacity restrictions, returning
		/// {@code true} upon success and {@code false} if no space is currently
		/// available.  When using a capacity-restricted deque, this method is
		/// generally preferable to the <seealso cref="#add"/> method, which can fail to
		/// insert an element only by throwing an exception.
		/// 
		/// <para>This method is equivalent to <seealso cref="#offerLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} if the element was added to this deque, else
		///         {@code false} </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		bool Offer(E e);

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque).
		/// This method differs from <seealso cref="#poll poll"/> only in that it throws an
		/// exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E Remove();

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque), or returns
		/// {@code null} if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#pollFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first element of this deque, or {@code null} if
		///         this deque is empty </returns>
		E Poll();

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque (in other words, the first element of this deque).
		/// This method differs from <seealso cref="#peek peek"/> only in that it throws an
		/// exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#getFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E Element();

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque (in other words, the first element of this deque), or
		/// returns {@code null} if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#peekFirst()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of the queue represented by this deque, or
		///         {@code null} if this deque is empty </returns>
		E Peek();


		// *** Stack methods ***

		/// <summary>
		/// Pushes an element onto the stack represented by this deque (in other
		/// words, at the head of this deque) if it is possible to do so
		/// immediately without violating capacity restrictions, throwing an
		/// {@code IllegalStateException} if no space is currently available.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to push </param>
		/// <exception cref="IllegalStateException"> if the element cannot be added at this
		///         time due to capacity restrictions </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		void Push(E e);

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
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E Pop();


		// *** Collection methods ***

		/// <summary>
		/// Removes the first occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>
		/// (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirstOccurrence(Object)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		bool Remove(Object o);

		/// <summary>
		/// Returns {@code true} if this deque contains the specified element.
		/// More formally, returns {@code true} if and only if this deque contains
		/// at least one element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this deque is to be tested </param>
		/// <returns> {@code true} if this deque contains the specified element </returns>
		/// <exception cref="ClassCastException"> if the type of the specified element
		///         is incompatible with this deque
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         deque does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		bool Contains(Object o);

		/// <summary>
		/// Returns the number of elements in this deque.
		/// </summary>
		/// <returns> the number of elements in this deque </returns>
		int Size();

		/// <summary>
		/// Returns an iterator over the elements in this deque in proper sequence.
		/// The elements will be returned in order from first (head) to last (tail).
		/// </summary>
		/// <returns> an iterator over the elements in this deque in proper sequence </returns>
		Iterator<E> Iterator();

		/// <summary>
		/// Returns an iterator over the elements in this deque in reverse
		/// sequential order.  The elements will be returned in order from
		/// last (tail) to first (head).
		/// </summary>
		/// <returns> an iterator over the elements in this deque in reverse
		/// sequence </returns>
		Iterator<E> DescendingIterator();

	}

}