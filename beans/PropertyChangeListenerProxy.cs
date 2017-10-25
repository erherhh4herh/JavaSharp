/*
 * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	/// <summary>
	/// A class which extends the {@code EventListenerProxy}
	/// specifically for adding a {@code PropertyChangeListener}
	/// with a "bound" property.
	/// Instances of this class can be added
	/// as {@code PropertyChangeListener}s to a bean
	/// which supports firing property change events.
	/// <para>
	/// If the object has a {@code getPropertyChangeListeners} method
	/// then the array returned could be a mixture of {@code PropertyChangeListener}
	/// and {@code PropertyChangeListenerProxy} objects.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.EventListenerProxy </seealso>
	/// <seealso cref= PropertyChangeSupport#getPropertyChangeListeners
	/// @since 1.4 </seealso>
	public class PropertyChangeListenerProxy : EventListenerProxy<PropertyChangeListener>, PropertyChangeListener
	{

		private readonly String PropertyName_Renamed;

		/// <summary>
		/// Constructor which binds the {@code PropertyChangeListener}
		/// to a specific property.
		/// </summary>
		/// <param name="propertyName">  the name of the property to listen on </param>
		/// <param name="listener">      the listener object </param>
		public PropertyChangeListenerProxy(String propertyName, PropertyChangeListener listener) : base(listener)
		{
			this.PropertyName_Renamed = propertyName;
		}

		/// <summary>
		/// Forwards the property change event to the listener delegate.
		/// </summary>
		/// <param name="event">  the property change event </param>
		public virtual void PropertyChange(PropertyChangeEvent @event)
		{
			Listener.PropertyChange(@event);
		}

		/// <summary>
		/// Returns the name of the named property associated with the listener.
		/// </summary>
		/// <returns> the name of the named property associated with the listener </returns>
		public virtual String PropertyName
		{
			get
			{
				return this.PropertyName_Renamed;
			}
		}
	}

}