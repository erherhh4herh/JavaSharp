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
	/// A <seealso cref="BlockingQueue"/> in which producers may wait for consumers
	/// to receive elements.  A {@code TransferQueue} may be useful for
	/// example in message passing applications in which producers
	/// sometimes (using method <seealso cref="#transfer"/>) await receipt of
	/// elements by consumers invoking {@code take} or {@code poll}, while
	/// at other times enqueue elements (via method {@code put}) without
	/// waiting for receipt.
	/// <seealso cref="#tryTransfer(Object) Non-blocking"/> and
	/// <seealso cref="#tryTransfer(Object,long,TimeUnit) time-out"/> versions of
	/// {@code tryTransfer} are also available.
	/// A {@code TransferQueue} may also be queried, via {@link
	/// #hasWaitingConsumer}, whether there are any threads waiting for
	/// items, which is a converse analogy to a {@code peek} operation.
	/// 
	/// <para>Like other blocking queues, a {@code TransferQueue} may be
	/// capacity bounded.  If so, an attempted transfer operation may
	/// initially block waiting for available space, and/or subsequently
	/// block waiting for reception by a consumer.  Note that in a queue
	/// with zero capacity, such as <seealso cref="SynchronousQueue"/>, {@code put}
	/// and {@code transfer} are effectively synonymous.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements held in this collection </param>
	public interface TransferQueue<E> : BlockingQueue<E>
	{
		/// <summary>
		/// Transfers the element to a waiting consumer immediately, if possible.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// otherwise returning {@code false} without enqueuing the element.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to transfer </param>
		/// <returns> {@code true} if the element was transferred, else
		///         {@code false} </returns>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this queue </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this queue </exception>
		bool TryTransfer(E e);

		/// <summary>
		/// Transfers the element to a consumer, waiting if necessary to do so.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// else waits until the element is received by a consumer.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to transfer </param>
		/// <exception cref="InterruptedException"> if interrupted while waiting,
		///         in which case the element is not left enqueued </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this queue </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this queue </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void transfer(E e) throws InterruptedException;
		void Transfer(E e);

		/// <summary>
		/// Transfers the element to a consumer if it is possible to do so
		/// before the timeout elapses.
		/// 
		/// <para>More precisely, transfers the specified element immediately
		/// if there exists a consumer already waiting to receive it (in
		/// <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		/// else waits until the element is received by a consumer,
		/// returning {@code false} if the specified wait time elapses
		/// before the element can be transferred.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the element to transfer </param>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> {@code true} if successful, or {@code false} if
		///         the specified waiting time elapses before completion,
		///         in which case the element is not left enqueued </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting,
		///         in which case the element is not left enqueued </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this queue </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified
		///         element prevents it from being added to this queue </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean tryTransfer(E e, long timeout, TimeUnit unit) throws InterruptedException;
		bool TryTransfer(E e, long timeout, TimeUnit unit);

		/// <summary>
		/// Returns {@code true} if there is at least one consumer waiting
		/// to receive an element via <seealso cref="#take"/> or
		/// timed <seealso cref="#poll(long,TimeUnit) poll"/>.
		/// The return value represents a momentary state of affairs.
		/// </summary>
		/// <returns> {@code true} if there is at least one waiting consumer </returns>
		bool HasWaitingConsumer();

		/// <summary>
		/// Returns an estimate of the number of consumers waiting to
		/// receive elements via <seealso cref="#take"/> or timed
		/// <seealso cref="#poll(long,TimeUnit) poll"/>.  The return value is an
		/// approximation of a momentary state of affairs, that may be
		/// inaccurate if consumers have completed or given up waiting.
		/// The value may be useful for monitoring and heuristics, but
		/// not for synchronization control.  Implementations of this
		/// method are likely to be noticeably slower than those for
		/// <seealso cref="#hasWaitingConsumer"/>.
		/// </summary>
		/// <returns> the number of consumers waiting to receive elements </returns>
		int WaitingConsumerCount {get;}
	}

}