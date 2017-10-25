/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{



	/// <summary>
	/// A multiplexor of <seealso cref="SelectableChannel"/> objects.
	/// 
	/// <para> A selector may be created by invoking the <seealso cref="#open open"/> method of
	/// this class, which will use the system's default {@link
	/// java.nio.channels.spi.SelectorProvider selector provider} to
	/// create a new selector.  A selector may also be created by invoking the
	/// <seealso cref="java.nio.channels.spi.SelectorProvider#openSelector openSelector"/>
	/// method of a custom selector provider.  A selector remains open until it is
	/// closed via its <seealso cref="#close close"/> method.
	/// 
	/// <a name="ks"></a>
	/// 
	/// </para>
	/// <para> A selectable channel's registration with a selector is represented by a
	/// <seealso cref="SelectionKey"/> object.  A selector maintains three sets of selection
	/// keys:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> The <i>key set</i> contains the keys representing the current
	///   channel registrations of this selector.  This set is returned by the
	///   <seealso cref="#keys() keys"/> method. </para></li>
	/// 
	///   <li><para> The <i>selected-key set</i> is the set of keys such that each
	///   key's channel was detected to be ready for at least one of the operations
	///   identified in the key's interest set during a prior selection operation.
	///   This set is returned by the <seealso cref="#selectedKeys() selectedKeys"/> method.
	///   The selected-key set is always a subset of the key set. </para></li>
	/// 
	///   <li><para> The <i>cancelled-key</i> set is the set of keys that have been
	///   cancelled but whose channels have not yet been deregistered.  This set is
	///   not directly accessible.  The cancelled-key set is always a subset of the
	///   key set. </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> All three sets are empty in a newly-created selector.
	/// 
	/// </para>
	/// <para> A key is added to a selector's key set as a side effect of registering a
	/// channel via the channel's {@link SelectableChannel#register(Selector,int)
	/// register} method.  Cancelled keys are removed from the key set during
	/// selection operations.  The key set itself is not directly modifiable.
	/// 
	/// </para>
	/// <para> A key is added to its selector's cancelled-key set when it is cancelled,
	/// whether by closing its channel or by invoking its {@link SelectionKey#cancel
	/// cancel} method.  Cancelling a key will cause its channel to be deregistered
	/// during the next selection operation, at which time the key will removed from
	/// all of the selector's key sets.
	/// 
	/// </para>
	/// <a name="sks"></a><para> Keys are added to the selected-key set by selection
	/// operations.  A key may be removed directly from the selected-key set by
	/// invoking the set's <seealso cref="java.util.Set#remove(java.lang.Object) remove"/>
	/// method or by invoking the <seealso cref="java.util.Iterator#remove() remove"/> method
	/// of an <seealso cref="java.util.Iterator iterator"/> obtained from the
	/// set.  Keys are never removed from the selected-key set in any other way;
	/// they are not, in particular, removed as a side effect of selection
	/// operations.  Keys may not be added directly to the selected-key set. </para>
	/// 
	/// 
	/// <a name="selop"></a>
	/// <h2>Selection</h2>
	/// 
	/// <para> During each selection operation, keys may be added to and removed from a
	/// selector's selected-key set and may be removed from its key and
	/// cancelled-key sets.  Selection is performed by the <seealso cref="#select()"/>, {@link
	/// #select(long)}, and <seealso cref="#selectNow()"/> methods, and involves three steps:
	/// </para>
	/// 
	/// <ol>
	/// 
	///   <li><para> Each key in the cancelled-key set is removed from each key set of
	///   which it is a member, and its channel is deregistered.  This step leaves
	///   the cancelled-key set empty. </para></li>
	/// 
	///   <li><para> The underlying operating system is queried for an update as to the
	///   readiness of each remaining channel to perform any of the operations
	///   identified by its key's interest set as of the moment that the selection
	///   operation began.  For a channel that is ready for at least one such
	///   operation, one of the following two actions is performed: </para>
	/// 
	///   <ol>
	/// 
	///     <li><para> If the channel's key is not already in the selected-key set then
	///     it is added to that set and its ready-operation set is modified to
	///     identify exactly those operations for which the channel is now reported
	///     to be ready.  Any readiness information previously recorded in the ready
	///     set is discarded.  </para></li>
	/// 
	///     <li><para> Otherwise the channel's key is already in the selected-key set,
	///     so its ready-operation set is modified to identify any new operations
	///     for which the channel is reported to be ready.  Any readiness
	///     information previously recorded in the ready set is preserved; in other
	///     words, the ready set returned by the underlying system is
	///     bitwise-disjoined into the key's current ready set. </para></li>
	/// 
	///   </ol>
	/// 
	///   If all of the keys in the key set at the start of this step have empty
	///   interest sets then neither the selected-key set nor any of the keys'
	///   ready-operation sets will be updated.
	/// 
	///   <li><para> If any keys were added to the cancelled-key set while step (2) was
	///   in progress then they are processed as in step (1). </para></li>
	/// 
	/// </ol>
	/// 
	/// <para> Whether or not a selection operation blocks to wait for one or more
	/// channels to become ready, and if so for how long, is the only essential
	/// difference between the three selection methods. </para>
	/// 
	/// 
	/// <h2>Concurrency</h2>
	/// 
	/// <para> Selectors are themselves safe for use by multiple concurrent threads;
	/// their key sets, however, are not.
	/// 
	/// </para>
	/// <para> The selection operations synchronize on the selector itself, on the key
	/// set, and on the selected-key set, in that order.  They also synchronize on
	/// the cancelled-key set during steps (1) and (3) above.
	/// 
	/// </para>
	/// <para> Changes made to the interest sets of a selector's keys while a
	/// selection operation is in progress have no effect upon that operation; they
	/// will be seen by the next selection operation.
	/// 
	/// </para>
	/// <para> Keys may be cancelled and channels may be closed at any time.  Hence the
	/// presence of a key in one or more of a selector's key sets does not imply
	/// that the key is valid or that its channel is open.  Application code should
	/// be careful to synchronize and check these conditions as necessary if there
	/// is any possibility that another thread will cancel a key or close a channel.
	/// 
	/// </para>
	/// <para> A thread blocked in one of the <seealso cref="#select()"/> or {@link
	/// #select(long)} methods may be interrupted by some other thread in one of
	/// three ways:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> By invoking the selector's <seealso cref="#wakeup wakeup"/> method,
	///   </para></li>
	/// 
	///   <li><para> By invoking the selector's <seealso cref="#close close"/> method, or
	///   </para></li>
	/// 
	///   <li><para> By invoking the blocked thread's {@link
	///   java.lang.Thread#interrupt() interrupt} method, in which case its
	///   interrupt status will be set and the selector's <seealso cref="#wakeup wakeup"/>
	///   method will be invoked. </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> The <seealso cref="#close close"/> method synchronizes on the selector and all
	/// three key sets in the same order as in a selection operation.
	/// 
	/// <a name="ksc"></a>
	/// 
	/// </para>
	/// <para> A selector's key and selected-key sets are not, in general, safe for use
	/// by multiple concurrent threads.  If such a thread might modify one of these
	/// sets directly then access should be controlled by synchronizing on the set
	/// itself.  The iterators returned by these sets' {@link
	/// java.util.Set#iterator() iterator} methods are <i>fail-fast:</i> If the set
	/// is modified after the iterator is created, in any way except by invoking the
	/// iterator's own <seealso cref="java.util.Iterator#remove() remove"/> method, then a
	/// <seealso cref="java.util.ConcurrentModificationException"/> will be thrown. </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>
	/// <seealso cref= SelectableChannel </seealso>
	/// <seealso cref= SelectionKey </seealso>

	public abstract class Selector : Closeable
	{

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal Selector()
		{
		}

		/// <summary>
		/// Opens a selector.
		/// 
		/// <para> The new selector is created by invoking the {@link
		/// java.nio.channels.spi.SelectorProvider#openSelector openSelector} method
		/// of the system-wide default {@link
		/// java.nio.channels.spi.SelectorProvider} object.  </para>
		/// </summary>
		/// <returns>  A new selector
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Selector open() throws java.io.IOException
		public static Selector Open()
		{
			return SelectorProvider.Provider().OpenSelector();
		}

		/// <summary>
		/// Tells whether or not this selector is open.
		/// </summary>
		/// <returns> <tt>true</tt> if, and only if, this selector is open </returns>
		public abstract bool Open {get;}

		/// <summary>
		/// Returns the provider that created this channel.
		/// </summary>
		/// <returns>  The provider that created this channel </returns>
		public abstract SelectorProvider Provider();

		/// <summary>
		/// Returns this selector's key set.
		/// 
		/// <para> The key set is not directly modifiable.  A key is removed only after
		/// it has been cancelled and its channel has been deregistered.  Any
		/// attempt to modify the key set will cause an {@link
		/// UnsupportedOperationException} to be thrown.
		/// 
		/// </para>
		/// <para> The key set is <a href="#ksc">not thread-safe</a>. </para>
		/// </summary>
		/// <returns>  This selector's key set
		/// </returns>
		/// <exception cref="ClosedSelectorException">
		///          If this selector is closed </exception>
		public abstract Set<SelectionKey> Keys();

		/// <summary>
		/// Returns this selector's selected-key set.
		/// 
		/// <para> Keys may be removed from, but not directly added to, the
		/// selected-key set.  Any attempt to add an object to the key set will
		/// cause an <seealso cref="UnsupportedOperationException"/> to be thrown.
		/// 
		/// </para>
		/// <para> The selected-key set is <a href="#ksc">not thread-safe</a>. </para>
		/// </summary>
		/// <returns>  This selector's selected-key set
		/// </returns>
		/// <exception cref="ClosedSelectorException">
		///          If this selector is closed </exception>
		public abstract Set<SelectionKey> SelectedKeys();

		/// <summary>
		/// Selects a set of keys whose corresponding channels are ready for I/O
		/// operations.
		/// 
		/// <para> This method performs a non-blocking <a href="#selop">selection
		/// operation</a>.  If no channels have become selectable since the previous
		/// selection operation then this method immediately returns zero.
		/// 
		/// </para>
		/// <para> Invoking this method clears the effect of any previous invocations
		/// of the <seealso cref="#wakeup wakeup"/> method.  </para>
		/// </summary>
		/// <returns>  The number of keys, possibly zero, whose ready-operation sets
		///          were updated by the selection operation
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <exception cref="ClosedSelectorException">
		///          If this selector is closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int selectNow() throws java.io.IOException;
		public abstract int SelectNow();

		/// <summary>
		/// Selects a set of keys whose corresponding channels are ready for I/O
		/// operations.
		/// 
		/// <para> This method performs a blocking <a href="#selop">selection
		/// operation</a>.  It returns only after at least one channel is selected,
		/// this selector's <seealso cref="#wakeup wakeup"/> method is invoked, the current
		/// thread is interrupted, or the given timeout period expires, whichever
		/// comes first.
		/// 
		/// </para>
		/// <para> This method does not offer real-time guarantees: It schedules the
		/// timeout as if by invoking the <seealso cref="Object#wait(long)"/> method. </para>
		/// </summary>
		/// <param name="timeout">  If positive, block for up to <tt>timeout</tt>
		///                  milliseconds, more or less, while waiting for a
		///                  channel to become ready; if zero, block indefinitely;
		///                  must not be negative
		/// </param>
		/// <returns>  The number of keys, possibly zero,
		///          whose ready-operation sets were updated
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <exception cref="ClosedSelectorException">
		///          If this selector is closed
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the value of the timeout argument is negative </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int select(long timeout) throws java.io.IOException;
		public abstract int Select(long timeout);

		/// <summary>
		/// Selects a set of keys whose corresponding channels are ready for I/O
		/// operations.
		/// 
		/// <para> This method performs a blocking <a href="#selop">selection
		/// operation</a>.  It returns only after at least one channel is selected,
		/// this selector's <seealso cref="#wakeup wakeup"/> method is invoked, or the current
		/// thread is interrupted, whichever comes first.  </para>
		/// </summary>
		/// <returns>  The number of keys, possibly zero,
		///          whose ready-operation sets were updated
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <exception cref="ClosedSelectorException">
		///          If this selector is closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int select() throws java.io.IOException;
		public abstract int Select();

		/// <summary>
		/// Causes the first selection operation that has not yet returned to return
		/// immediately.
		/// 
		/// <para> If another thread is currently blocked in an invocation of the
		/// <seealso cref="#select()"/> or <seealso cref="#select(long)"/> methods then that invocation
		/// will return immediately.  If no selection operation is currently in
		/// progress then the next invocation of one of these methods will return
		/// immediately unless the <seealso cref="#selectNow()"/> method is invoked in the
		/// meantime.  In any case the value returned by that invocation may be
		/// non-zero.  Subsequent invocations of the <seealso cref="#select()"/> or {@link
		/// #select(long)} methods will block as usual unless this method is invoked
		/// again in the meantime.
		/// 
		/// </para>
		/// <para> Invoking this method more than once between two successive selection
		/// operations has the same effect as invoking it just once.  </para>
		/// </summary>
		/// <returns>  This selector </returns>
		public abstract Selector Wakeup();

		/// <summary>
		/// Closes this selector.
		/// 
		/// <para> If a thread is currently blocked in one of this selector's selection
		/// methods then it is interrupted as if by invoking the selector's {@link
		/// #wakeup wakeup} method.
		/// 
		/// </para>
		/// <para> Any uncancelled keys still associated with this selector are
		/// invalidated, their channels are deregistered, and any other resources
		/// associated with this selector are released.
		/// 
		/// </para>
		/// <para> If this selector is already closed then invoking this method has no
		/// effect.
		/// 
		/// </para>
		/// <para> After a selector is closed, any further attempt to use it, except by
		/// invoking this method or the <seealso cref="#wakeup wakeup"/> method, will cause a
		/// <seealso cref="ClosedSelectorException"/> to be thrown. </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void close() throws java.io.IOException;
		public abstract void Close();

	}

}