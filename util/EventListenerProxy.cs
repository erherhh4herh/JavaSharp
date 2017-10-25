/*
 * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// An abstract wrapper class for an {@code EventListener} class
	/// which associates a set of additional parameters with the listener.
	/// Subclasses must provide the storage and accessor methods
	/// for the additional arguments or parameters.
	/// <para>
	/// For example, a bean which supports named properties
	/// would have a two argument method signature for adding
	/// a {@code PropertyChangeListener} for a property:
	/// <pre>
	/// public void addPropertyChangeListener(String propertyName,
	///                                       PropertyChangeListener listener)
	/// </pre>
	/// If the bean also implemented the zero argument get listener method:
	/// <pre>
	/// public PropertyChangeListener[] getPropertyChangeListeners()
	/// </pre>
	/// then the array may contain inner {@code PropertyChangeListeners}
	/// which are also {@code PropertyChangeListenerProxy} objects.
	/// </para>
	/// <para>
	/// If the calling method is interested in retrieving the named property
	/// then it would have to test the element to see if it is a proxy class.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>
	public abstract class EventListenerProxy<T> : EventListener where T : EventListener
	{

		private readonly T Listener_Renamed;

		/// <summary>
		/// Creates a proxy for the specified listener.
		/// </summary>
		/// <param name="listener">  the listener object </param>
		public EventListenerProxy(T listener)
		{
			this.Listener_Renamed = listener;
		}

		/// <summary>
		/// Returns the listener associated with the proxy.
		/// </summary>
		/// <returns>  the listener associated with the proxy </returns>
		public virtual T Listener
		{
			get
			{
				return this.Listener_Renamed;
			}
		}
	}

}