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
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	/// <summary>
	/// A <seealso cref="Deque"/> that additionally supports blocking operations that wait
	/// for the deque to become non-empty when retrieving an element, and wait for
	/// space to become available in the deque when storing an element.
	/// 
	/// <para>{@code BlockingDeque} methods come in four forms, with different ways
	/// of handling operations that cannot be satisfied immediately, but may be
	/// satisfied at some point in the future:
	/// one throws an exception, the second returns a special value (either
	/// {@code null} or {@code false}, depending on the operation), the third
	/// blocks the current thread indefinitely until the operation can succeed,
	/// and the fourth blocks for only a given maximum time limit before giving
	/// up.  These methods are summarized in the following table:
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Summary of BlockingDeque methods</caption>
	///  <tr>
	///    <td ALIGN=CENTER COLSPAN = 5> <b>First Element (Head)</b></td>
	///  </tr>
	///  <tr>
	///    <td></td>
	///    <td ALIGN=CENTER><em>Throws exception</em></td>
	///    <td ALIGN=CENTER><em>Special value</em></td>
	///    <td ALIGN=CENTER><em>Blocks</em></td>
	///    <td ALIGN=CENTER><em>Times out</em></td>
	///  </tr>
	///  <tr>
	///    <td><b>Insert</b></td>
	///    <td><seealso cref="#addFirst addFirst(e)"/></td>
	///    <td><seealso cref="#offerFirst(Object) offerFirst(e)"/></td>
	///    <td><seealso cref="#putFirst putFirst(e)"/></td>
	///    <td><seealso cref="#offerFirst(Object, long, TimeUnit) offerFirst(e, time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Remove</b></td>
	///    <td><seealso cref="#removeFirst removeFirst()"/></td>
	///    <td><seealso cref="#pollFirst pollFirst()"/></td>
	///    <td><seealso cref="#takeFirst takeFirst()"/></td>
	///    <td><seealso cref="#pollFirst(long, TimeUnit) pollFirst(time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Examine</b></td>
	///    <td><seealso cref="#getFirst getFirst()"/></td>
	///    <td><seealso cref="#peekFirst peekFirst()"/></td>
	///    <td><em>not applicable</em></td>
	///    <td><em>not applicable</em></td>
	///  </tr>
	///  <tr>
	///    <td ALIGN=CENTER COLSPAN = 5> <b>Last Element (Tail)</b></td>
	///  </tr>
	///  <tr>
	///    <td></td>
	///    <td ALIGN=CENTER><em>Throws exception</em></td>
	///    <td ALIGN=CENTER><em>Special value</em></td>
	///    <td ALIGN=CENTER><em>Blocks</em></td>
	///    <td ALIGN=CENTER><em>Times out</em></td>
	///  </tr>
	///  <tr>
	///    <td><b>Insert</b></td>
	///    <td><seealso cref="#addLast addLast(e)"/></td>
	///    <td><seealso cref="#offerLast(Object) offerLast(e)"/></td>
	///    <td><seealso cref="#putLast putLast(e)"/></td>
	///    <td><seealso cref="#offerLast(Object, long, TimeUnit) offerLast(e, time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Remove</b></td>
	///    <td><seealso cref="#removeLast() removeLast()"/></td>
	///    <td><seealso cref="#pollLast() pollLast()"/></td>
	///    <td><seealso cref="#takeLast takeLast()"/></td>
	///    <td><seealso cref="#pollLast(long, TimeUnit) pollLast(time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td><b>Examine</b></td>
	///    <td><seealso cref="#getLast getLast()"/></td>
	///    <td><seealso cref="#peekLast peekLast()"/></td>
	///    <td><em>not applicable</em></td>
	///    <td><em>not applicable</em></td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>Like any <seealso cref="BlockingQueue"/>, a {@code BlockingDeque} is thread safe,
	/// does not permit null elements, and may (or may not) be
	/// capacity-constrained.
	/// 
	/// </para>
	/// <para>A {@code BlockingDeque} implementation may be used directly as a FIFO
	/// {@code BlockingQueue}. The methods inherited from the
	/// {@code BlockingQueue} interface are precisely equivalent to
	/// {@code BlockingDeque} methods as indicated in the following table:
	/// 
	/// <table BORDER CELLPADDING=3 CELLSPACING=1>
	/// <caption>Comparison of BlockingQueue and BlockingDeque methods</caption>
	///  <tr>
	///    <td ALIGN=CENTER> <b>{@code BlockingQueue} Method</b></td>
	///    <td ALIGN=CENTER> <b>Equivalent {@code BlockingDeque} Method</b></td>
	///  </tr>
	///  <tr>
	///    <td ALIGN=CENTER COLSPAN = 2> <b>Insert</b></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#add(Object) add(e)"/></td>
	///    <td><seealso cref="#addLast(Object) addLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#offer(Object) offer(e)"/></td>
	///    <td><seealso cref="#offerLast(Object) offerLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#put(Object) put(e)"/></td>
	///    <td><seealso cref="#putLast(Object) putLast(e)"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#offer(Object, long, TimeUnit) offer(e, time, unit)"/></td>
	///    <td><seealso cref="#offerLast(Object, long, TimeUnit) offerLast(e, time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td ALIGN=CENTER COLSPAN = 2> <b>Remove</b></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#remove() remove()"/></td>
	///    <td><seealso cref="#removeFirst() removeFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#poll() poll()"/></td>
	///    <td><seealso cref="#pollFirst() pollFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#take() take()"/></td>
	///    <td><seealso cref="#takeFirst() takeFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#poll(long, TimeUnit) poll(time, unit)"/></td>
	///    <td><seealso cref="#pollFirst(long, TimeUnit) pollFirst(time, unit)"/></td>
	///  </tr>
	///  <tr>
	///    <td ALIGN=CENTER COLSPAN = 2> <b>Examine</b></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#element() element()"/></td>
	///    <td><seealso cref="#getFirst() getFirst()"/></td>
	///  </tr>
	///  <tr>
	///    <td><seealso cref="#peek() peek()"/></td>
	///    <td><seealso cref="#peekFirst() peekFirst()"/></td>
	///  </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>Memory consistency effects: As with other concurrent
	/// collections, actions in a thread prior to placing an object into a
	/// {@code BlockingDeque}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that element from
	/// the {@code BlockingDeque} in another thread.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.6
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	public interface BlockingDeque<E> : BlockingQueue<E>, Deque<E>
	{
		/*
		 * We have "diamond" multiple interface inheritance here, and that
		 * introduces ambiguities.  Methods might end up with different
		 * specs depending on the branch chosen by javadoc.  Thus a lot of
		 * methods specs here are copied from superinterfaces.
		 */

		/// <summary>
		/// Inserts the specified element at the front of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// throwing an {@code IllegalStateException} if no space is currently
		/// available.  When using a capacity-restricted deque, it is generally
		/// preferable to use <seealso cref="#offerFirst(Object) offerFirst"/>.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		void AddFirst(E e);

		/// <summary>
		/// Inserts the specified element at the end of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// throwing an {@code IllegalStateException} if no space is currently
		/// available.  When using a capacity-restricted deque, it is generally
		/// preferable to use <seealso cref="#offerLast(Object) offerLast"/>.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		void AddLast(E e);

		/// <summary>
		/// Inserts the specified element at the front of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// returning {@code true} upon success and {@code false} if no space is
		/// currently available.
		/// When using a capacity-restricted deque, this method is generally
		/// preferable to the <seealso cref="#addFirst(Object) addFirst"/> method, which can
		/// fail to insert an element only by throwing an exception.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		bool OfferFirst(E e);

		/// <summary>
		/// Inserts the specified element at the end of this deque if it is
		/// possible to do so immediately without violating capacity restrictions,
		/// returning {@code true} upon success and {@code false} if no space is
		/// currently available.
		/// When using a capacity-restricted deque, this method is generally
		/// preferable to the <seealso cref="#addLast(Object) addLast"/> method, which can
		/// fail to insert an element only by throwing an exception.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		bool OfferLast(E e);

		/// <summary>
		/// Inserts the specified element at the front of this deque,
		/// waiting if necessary for space to become available.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void putFirst(E e) throws InterruptedException;
		void PutFirst(E e);

		/// <summary>
		/// Inserts the specified element at the end of this deque,
		/// waiting if necessary for space to become available.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void putLast(E e) throws InterruptedException;
		void PutLast(E e);

		/// <summary>
		/// Inserts the specified element at the front of this deque,
		/// waiting up to the specified wait time if necessary for space to
		/// become available.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> {@code true} if successful, or {@code false} if
		///         the specified waiting time elapses before space is available </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean offerFirst(E e, long timeout, TimeUnit unit) throws InterruptedException;
		bool OfferFirst(E e, long timeout, TimeUnit unit);

		/// <summary>
		/// Inserts the specified element at the end of this deque,
		/// waiting up to the specified wait time if necessary for space to
		/// become available.
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> {@code true} if successful, or {@code false} if
		///         the specified waiting time elapses before space is available </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean offerLast(E e, long timeout, TimeUnit unit) throws InterruptedException;
		bool OfferLast(E e, long timeout, TimeUnit unit);

		/// <summary>
		/// Retrieves and removes the first element of this deque, waiting
		/// if necessary until an element becomes available.
		/// </summary>
		/// <returns> the head of this deque </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E takeFirst() throws InterruptedException;
		E TakeFirst();

		/// <summary>
		/// Retrieves and removes the last element of this deque, waiting
		/// if necessary until an element becomes available.
		/// </summary>
		/// <returns> the tail of this deque </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E takeLast() throws InterruptedException;
		E TakeLast();

		/// <summary>
		/// Retrieves and removes the first element of this deque, waiting
		/// up to the specified wait time if necessary for an element to
		/// become available.
		/// </summary>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> the head of this deque, or {@code null} if the specified
		///         waiting time elapses before an element is available </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E pollFirst(long timeout, TimeUnit unit) throws InterruptedException;
		E PollFirst(long timeout, TimeUnit unit);

		/// <summary>
		/// Retrieves and removes the last element of this deque, waiting
		/// up to the specified wait time if necessary for an element to
		/// become available.
		/// </summary>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> the tail of this deque, or {@code null} if the specified
		///         waiting time elapses before an element is available </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E pollLast(long timeout, TimeUnit unit) throws InterruptedException;
		E PollLast(long timeout, TimeUnit unit);

		/// <summary>
		/// Removes the first occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		bool RemoveFirstOccurrence(Object o);

		/// <summary>
		/// Removes the last occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the last element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		bool RemoveLastOccurrence(Object o);

		// *** BlockingQueue methods ***

		/// <summary>
		/// Inserts the specified element into the queue represented by this deque
		/// (in other words, at the tail of this deque) if it is possible to do so
		/// immediately without violating capacity restrictions, returning
		/// {@code true} upon success and throwing an
		/// {@code IllegalStateException} if no space is currently available.
		/// When using a capacity-restricted deque, it is generally preferable to
		/// use <seealso cref="#offer(Object) offer"/>.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addLast(Object) addLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
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
		/// <para>This method is equivalent to <seealso cref="#offerLast(Object) offerLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
		bool Offer(E e);

		/// <summary>
		/// Inserts the specified element into the queue represented by this deque
		/// (in other words, at the tail of this deque), waiting if necessary for
		/// space to become available.
		/// 
		/// <para>This method is equivalent to <seealso cref="#putLast(Object) putLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void put(E e) throws InterruptedException;
		void Put(E e);

		/// <summary>
		/// Inserts the specified element into the queue represented by this deque
		/// (in other words, at the tail of this deque), waiting up to the
		/// specified wait time if necessary for space to become available.
		/// 
		/// <para>This method is equivalent to
		/// <seealso cref="#offerLast(Object,long,TimeUnit) offerLast"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to add </param>
		/// <returns> {@code true} if the element was added to this deque, else
		///         {@code false} </returns>
		/// <exception cref="InterruptedException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this deque </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this deque </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean offer(E e, long timeout, TimeUnit unit) throws InterruptedException;
		bool Offer(E e, long timeout, TimeUnit unit);

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque).
		/// This method differs from <seealso cref="#poll poll"/> only in that it
		/// throws an exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#removeFirst() removeFirst"/>.
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
		/// <returns> the head of this deque, or {@code null} if this deque is empty </returns>
		E Poll();

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque), waiting if
		/// necessary until an element becomes available.
		/// 
		/// <para>This method is equivalent to <seealso cref="#takeFirst() takeFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of this deque </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E take() throws InterruptedException;
		E Take();

		/// <summary>
		/// Retrieves and removes the head of the queue represented by this deque
		/// (in other words, the first element of this deque), waiting up to the
		/// specified wait time if necessary for an element to become available.
		/// 
		/// <para>This method is equivalent to
		/// <seealso cref="#pollFirst(long,TimeUnit) pollFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of this deque, or {@code null} if the
		///         specified waiting time elapses before an element is available </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: E poll(long timeout, TimeUnit unit) throws InterruptedException;
		E Poll(long timeout, TimeUnit unit);

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque (in other words, the first element of this deque).
		/// This method differs from <seealso cref="#peek peek"/> only in that it throws an
		/// exception if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#getFirst() getFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of this deque </returns>
		/// <exception cref="NoSuchElementException"> if this deque is empty </exception>
		E Element();

		/// <summary>
		/// Retrieves, but does not remove, the head of the queue represented by
		/// this deque (in other words, the first element of this deque), or
		/// returns {@code null} if this deque is empty.
		/// 
		/// <para>This method is equivalent to <seealso cref="#peekFirst() peekFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the head of this deque, or {@code null} if this deque is empty </returns>
		E Peek();

		/// <summary>
		/// Removes the first occurrence of the specified element from this deque.
		/// If the deque does not contain the element, it is unchanged.
		/// More formally, removes the first element {@code e} such that
		/// {@code o.equals(e)} (if such an element exists).
		/// Returns {@code true} if this deque contained the specified element
		/// (or equivalently, if this deque changed as a result of the call).
		/// 
		/// <para>This method is equivalent to
		/// <seealso cref="#removeFirstOccurrence(Object) removeFirstOccurrence"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> element to be removed from this deque, if present </param>
		/// <returns> {@code true} if this deque changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		bool Remove(Object o);

		/// <summary>
		/// Returns {@code true} if this deque contains the specified element.
		/// More formally, returns {@code true} if and only if this deque contains
		/// at least one element {@code e} such that {@code o.equals(e)}.
		/// </summary>
		/// <param name="o"> object to be checked for containment in this deque </param>
		/// <returns> {@code true} if this deque contains the specified element </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         is incompatible with this deque
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
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

		// *** Stack methods ***

		/// <summary>
		/// Pushes an element onto the stack represented by this deque (in other
		/// words, at the head of this deque) if it is possible to do so
		/// immediately without violating capacity restrictions, throwing an
		/// {@code IllegalStateException} if no space is currently available.
		/// 
		/// <para>This method is equivalent to <seealso cref="#addFirst(Object) addFirst"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		void Push(E e);
	}

}