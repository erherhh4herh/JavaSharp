using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// A token representing the registration of a <seealso cref="Watchable watchable"/> object
	/// with a <seealso cref="WatchService"/>.
	/// 
	/// <para> A watch key is created when a watchable object is registered with a watch
	/// service. The key remains <seealso cref="#isValid valid"/> until:
	/// <ol>
	///   <li> It is cancelled, explicitly, by invoking its <seealso cref="#cancel cancel"/>
	///     method, or</li>
	///   <li> Cancelled implicitly, because the object is no longer accessible,
	///     or </li>
	///   <li> By <seealso cref="WatchService#close closing"/> the watch service. </li>
	/// </ol>
	/// 
	/// </para>
	/// <para> A watch key has a state. When initially created the key is said to be
	/// <em>ready</em>. When an event is detected then the key is <em>signalled</em>
	/// and queued so that it can be retrieved by invoking the watch service's {@link
	/// WatchService#poll() poll} or <seealso cref="WatchService#take() take"/> methods. Once
	/// signalled, a key remains in this state until its <seealso cref="#reset reset"/> method
	/// is invoked to return the key to the ready state. Events detected while the
	/// key is in the signalled state are queued but do not cause the key to be
	/// re-queued for retrieval from the watch service. Events are retrieved by
	/// invoking the key's <seealso cref="#pollEvents pollEvents"/> method. This method
	/// retrieves and removes all events accumulated for the object. When initially
	/// created, a watch key has no pending events. Typically events are retrieved
	/// when the key is in the signalled state leading to the following idiom:
	/// 
	/// <pre>
	///     for (;;) {
	///         // retrieve key
	///         WatchKey key = watcher.take();
	/// 
	///         // process events
	///         for (WatchEvent&lt;?&gt; event: key.pollEvents()) {
	///             :
	///         }
	/// 
	///         // reset the key
	///         boolean valid = key.reset();
	///         if (!valid) {
	///             // object no longer registered
	///         }
	///     }
	/// </pre>
	/// 
	/// </para>
	/// <para> Watch keys are safe for use by multiple concurrent threads. Where there
	/// are several threads retrieving signalled keys from a watch service then care
	/// should be taken to ensure that the {@code reset} method is only invoked after
	/// the events for the object have been processed. This ensures that one thread
	/// is processing the events for an object at any time.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface WatchKey
	{

		/// <summary>
		/// Tells whether or not this watch key is valid.
		/// 
		/// <para> A watch key is valid upon creation and remains until it is cancelled,
		/// or its watch service is closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code true} if, and only if, this watch key is valid </returns>
		bool Valid {get;}

		/// <summary>
		/// Retrieves and removes all pending events for this watch key, returning
		/// a {@code List} of the events that were retrieved.
		/// 
		/// <para> Note that this method does not wait if there are no events pending.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the list of the events retrieved; may be empty </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<WatchEvent<?>> pollEvents();
		IList<WatchEvent<?>> PollEvents();

		/// <summary>
		/// Resets this watch key.
		/// 
		/// <para> If this watch key has been cancelled or this watch key is already in
		/// the ready state then invoking this method has no effect. Otherwise
		/// if there are pending events for the object then this watch key is
		/// immediately re-queued to the watch service. If there are no pending
		/// events then the watch key is put into the ready state and will remain in
		/// that state until an event is detected or the watch key is cancelled.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code true} if the watch key is valid and has been reset, and
		///          {@code false} if the watch key could not be reset because it is
		///          no longer <seealso cref="#isValid valid"/> </returns>
		bool Reset();

		/// <summary>
		/// Cancels the registration with the watch service. Upon return the watch key
		/// will be invalid. If the watch key is enqueued, waiting to be retrieved
		/// from the watch service, then it will remain in the queue until it is
		/// removed. Pending events, if any, remain pending and may be retrieved by
		/// invoking the <seealso cref="#pollEvents pollEvents"/> method after the key is
		/// cancelled.
		/// 
		/// <para> If this watch key has already been cancelled then invoking this
		/// method has no effect.  Once cancelled, a watch key remains forever invalid.
		/// </para>
		/// </summary>
		void Cancel();

		/// <summary>
		/// Returns the object for which this watch key was created. This method will
		/// continue to return the object even after the key is cancelled.
		/// 
		/// <para> As the {@code WatchService} is intended to map directly on to the
		/// native file event notification facility (where available) then many of
		/// details on how registered objects are watched is highly implementation
		/// specific. When watching a directory for changes for example, and the
		/// directory is moved or renamed in the file system, there is no guarantee
		/// that the watch key will be cancelled and so the object returned by this
		/// method may no longer be a valid path to the directory.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the object for which this watch key was created </returns>
		Watchable Watchable();
	}

}