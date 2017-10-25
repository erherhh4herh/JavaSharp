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
	/// specifically for adding a {@code VetoableChangeListener}
	/// with a "constrained" property.
	/// Instances of this class can be added
	/// as {@code VetoableChangeListener}s to a bean
	/// which supports firing vetoable change events.
	/// <para>
	/// If the object has a {@code getVetoableChangeListeners} method
	/// then the array returned could be a mixture of {@code VetoableChangeListener}
	/// and {@code VetoableChangeListenerProxy} objects.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.EventListenerProxy </seealso>
	/// <seealso cref= VetoableChangeSupport#getVetoableChangeListeners
	/// @since 1.4 </seealso>
	public class VetoableChangeListenerProxy : EventListenerProxy<VetoableChangeListener>, VetoableChangeListener
	{

		private readonly String PropertyName_Renamed;

		/// <summary>
		/// Constructor which binds the {@code VetoableChangeListener}
		/// to a specific property.
		/// </summary>
		/// <param name="propertyName">  the name of the property to listen on </param>
		/// <param name="listener">      the listener object </param>
		public VetoableChangeListenerProxy(String propertyName, VetoableChangeListener listener) : base(listener)
		{
			this.PropertyName_Renamed = propertyName;
		}

		/// <summary>
		/// Forwards the property change event to the listener delegate.
		/// </summary>
		/// <param name="event">  the property change event
		/// </param>
		/// <exception cref="PropertyVetoException"> if the recipient wishes the property
		///                                  change to be rolled back </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void vetoableChange(PropertyChangeEvent event) throws PropertyVetoException
		public virtual void VetoableChange(PropertyChangeEvent @event)
		{
			Listener.VetoableChange(@event);
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