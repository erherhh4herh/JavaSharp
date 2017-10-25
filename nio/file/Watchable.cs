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
	/// An object that may be registered with a watch service so that it can be
	/// <em>watched</em> for changes and events.
	/// 
	/// <para> This interface defines the <seealso cref="#register register"/> method to register
	/// the object with a <seealso cref="WatchService"/> returning a <seealso cref="WatchKey"/> to
	/// represent the registration. An object may be registered with more than one
	/// watch service. Registration with a watch service is cancelled by invoking the
	/// key's <seealso cref="WatchKey#cancel cancel"/> method.
	/// 
	/// @since 1.7
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Path#register </seealso>

	public interface Watchable
	{

		/// <summary>
		/// Registers an object with a watch service.
		/// 
		/// <para> If the file system object identified by this object is currently
		/// registered with the watch service then the watch key, representing that
		/// registration, is returned after changing the event set or modifiers to
		/// those specified by the {@code events} and {@code modifiers} parameters.
		/// Changing the event set does not cause pending events for the object to be
		/// discarded. Objects are automatically registered for the {@link
		/// StandardWatchEventKinds#OVERFLOW OVERFLOW} event. This event is not
		/// required to be present in the array of events.
		/// 
		/// </para>
		/// <para> Otherwise the file system object has not yet been registered with the
		/// given watch service, so it is registered and the resulting new key is
		/// returned.
		/// 
		/// </para>
		/// <para> Implementations of this interface should specify the events they
		/// support.
		/// 
		/// </para>
		/// </summary>
		/// <param name="watcher">
		///          the watch service to which this object is to be registered </param>
		/// <param name="events">
		///          the events for which this object should be registered </param>
		/// <param name="modifiers">
		///          the modifiers, if any, that modify how the object is registered
		/// </param>
		/// <returns>  a key representing the registration of this object with the
		///          given watch service
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if unsupported events or modifiers are specified </exception>
		/// <exception cref="IllegalArgumentException">
		///          if an invalid of combination of events are modifiers are specified </exception>
		/// <exception cref="ClosedWatchServiceException">
		///          if the watch service is closed </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          if a security manager is installed and it denies an unspecified
		///          permission required to monitor this object. Implementations of
		///          this interface should specify the permission checks. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: WatchKey register(WatchService watcher, WatchEvent_Kind<?>[] events, WatchEvent_Modifier... modifiers) throws java.io.IOException;
		WatchKey register<T1>(WatchService watcher, WatchEvent_Kind<T1>[] events, params WatchEvent_Modifier[] modifiers);


		/// <summary>
		/// Registers an object with a watch service.
		/// 
		/// <para> An invocation of this method behaves in exactly the same way as the
		/// invocation
		/// <pre>
		///     watchable.<seealso cref="#register(WatchService,WatchEvent.Kind[],WatchEvent.Modifier[]) register"/>(watcher, events, new WatchEvent.Modifier[0]);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="watcher">
		///          the watch service to which this object is to be registered </param>
		/// <param name="events">
		///          the events for which this object should be registered
		/// </param>
		/// <returns>  a key representing the registration of this object with the
		///          given watch service
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          if unsupported events are specified </exception>
		/// <exception cref="IllegalArgumentException">
		///          if an invalid of combination of events are specified </exception>
		/// <exception cref="ClosedWatchServiceException">
		///          if the watch service is closed </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          if a security manager is installed and it denies an unspecified
		///          permission required to monitor this object. Implementations of
		///          this interface should specify the permission checks. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: WatchKey register(WatchService watcher, WatchEvent_Kind<?>... events) throws java.io.IOException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		WatchKey Register(WatchService watcher, params WatchEvent_Kind<?>[] events);
	}

}