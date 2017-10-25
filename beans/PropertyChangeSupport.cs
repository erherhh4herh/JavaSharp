using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This is a utility class that can be used by beans that support bound
	/// properties.  It manages a list of listeners and dispatches
	/// <seealso cref="PropertyChangeEvent"/>s to them.  You can use an instance of this class
	/// as a member field of your bean and delegate these types of work to it.
	/// The <seealso cref="PropertyChangeListener"/> can be registered for all properties
	/// or for a property specified by name.
	/// <para>
	/// Here is an example of {@code PropertyChangeSupport} usage that follows
	/// the rules and recommendations laid out in the JavaBeans&trade; specification:
	/// <pre>
	/// public class MyBean {
	///     private final PropertyChangeSupport pcs = new PropertyChangeSupport(this);
	/// 
	///     public void addPropertyChangeListener(PropertyChangeListener listener) {
	///         this.pcs.addPropertyChangeListener(listener);
	///     }
	/// 
	///     public void removePropertyChangeListener(PropertyChangeListener listener) {
	///         this.pcs.removePropertyChangeListener(listener);
	///     }
	/// 
	///     private String value;
	/// 
	///     public String getValue() {
	///         return this.value;
	///     }
	/// 
	///     public void setValue(String newValue) {
	///         String oldValue = this.value;
	///         this.value = newValue;
	///         this.pcs.firePropertyChange("value", oldValue, newValue);
	///     }
	/// 
	///     [...]
	/// }
	/// </pre>
	/// </para>
	/// <para>
	/// A {@code PropertyChangeSupport} instance is thread-safe.
	/// </para>
	/// <para>
	/// This class is serializable.  When it is serialized it will save
	/// (and restore) any listeners that are themselves serializable.  Any
	/// non-serializable listeners will be skipped during serialization.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= VetoableChangeSupport </seealso>
	[Serializable]
	public class PropertyChangeSupport
	{
		private PropertyChangeListenerMap Map = new PropertyChangeListenerMap();

		/// <summary>
		/// Constructs a <code>PropertyChangeSupport</code> object.
		/// </summary>
		/// <param name="sourceBean">  The bean to be given as the source for any events. </param>
		public PropertyChangeSupport(Object sourceBean)
		{
			if (sourceBean == null)
			{
				throw new NullPointerException();
			}
			Source = sourceBean;
		}

		/// <summary>
		/// Add a PropertyChangeListener to the listener list.
		/// The listener is registered for all properties.
		/// The same listener object may be added more than once, and will be called
		/// as many times as it is added.
		/// If <code>listener</code> is null, no exception is thrown and no action
		/// is taken.
		/// </summary>
		/// <param name="listener">  The PropertyChangeListener to be added </param>
		public virtual void AddPropertyChangeListener(PropertyChangeListener listener)
		{
			if (listener == null)
			{
				return;
			}
			if (listener is PropertyChangeListenerProxy)
			{
				PropertyChangeListenerProxy proxy = (PropertyChangeListenerProxy)listener;
				// Call two argument add method.
				AddPropertyChangeListener(proxy.PropertyName, proxy.Listener);
			}
			else
			{
				this.Map.Add(null, listener);
			}
		}

		/// <summary>
		/// Remove a PropertyChangeListener from the listener list.
		/// This removes a PropertyChangeListener that was registered
		/// for all properties.
		/// If <code>listener</code> was added more than once to the same event
		/// source, it will be notified one less time after being removed.
		/// If <code>listener</code> is null, or was never added, no exception is
		/// thrown and no action is taken.
		/// </summary>
		/// <param name="listener">  The PropertyChangeListener to be removed </param>
		public virtual void RemovePropertyChangeListener(PropertyChangeListener listener)
		{
			if (listener == null)
			{
				return;
			}
			if (listener is PropertyChangeListenerProxy)
			{
				PropertyChangeListenerProxy proxy = (PropertyChangeListenerProxy)listener;
				// Call two argument remove method.
				RemovePropertyChangeListener(proxy.PropertyName, proxy.Listener);
			}
			else
			{
				this.Map.Remove(null, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the listeners that were added to the
		/// PropertyChangeSupport object with addPropertyChangeListener().
		/// <para>
		/// If some listeners have been added with a named property, then
		/// the returned array will be a mixture of PropertyChangeListeners
		/// and <code>PropertyChangeListenerProxy</code>s. If the calling
		/// method is interested in distinguishing the listeners then it must
		/// test each element to see if it's a
		/// <code>PropertyChangeListenerProxy</code>, perform the cast, and examine
		/// the parameter.
		/// 
		/// <pre>{@code
		/// PropertyChangeListener[] listeners = bean.getPropertyChangeListeners();
		/// for (int i = 0; i < listeners.length; i++) {
		///   if (listeners[i] instanceof PropertyChangeListenerProxy) {
		///     PropertyChangeListenerProxy proxy =
		///                    (PropertyChangeListenerProxy)listeners[i];
		///     if (proxy.getPropertyName().equals("foo")) {
		///       // proxy is a PropertyChangeListener which was associated
		///       // with the property named "foo"
		///     }
		///   }
		/// }
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= PropertyChangeListenerProxy </seealso>
		/// <returns> all of the <code>PropertyChangeListeners</code> added or an
		///         empty array if no listeners have been added
		/// @since 1.4 </returns>
		public virtual PropertyChangeListener[] PropertyChangeListeners
		{
			get
			{
				return this.Map.Listeners;
			}
		}

		/// <summary>
		/// Add a PropertyChangeListener for a specific property.  The listener
		/// will be invoked only when a call on firePropertyChange names that
		/// specific property.
		/// The same listener object may be added more than once.  For each
		/// property,  the listener will be invoked the number of times it was added
		/// for that property.
		/// If <code>propertyName</code> or <code>listener</code> is null, no
		/// exception is thrown and no action is taken.
		/// </summary>
		/// <param name="propertyName">  The name of the property to listen on. </param>
		/// <param name="listener">  The PropertyChangeListener to be added </param>
		public virtual void AddPropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			if (listener == null || propertyName == null)
			{
				return;
			}
			listener = this.Map.Extract(listener);
			if (listener != null)
			{
				this.Map.Add(propertyName, listener);
			}
		}

		/// <summary>
		/// Remove a PropertyChangeListener for a specific property.
		/// If <code>listener</code> was added more than once to the same event
		/// source for the specified property, it will be notified one less time
		/// after being removed.
		/// If <code>propertyName</code> is null,  no exception is thrown and no
		/// action is taken.
		/// If <code>listener</code> is null, or was never added for the specified
		/// property, no exception is thrown and no action is taken.
		/// </summary>
		/// <param name="propertyName">  The name of the property that was listened on. </param>
		/// <param name="listener">  The PropertyChangeListener to be removed </param>
		public virtual void RemovePropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			if (listener == null || propertyName == null)
			{
				return;
			}
			listener = this.Map.Extract(listener);
			if (listener != null)
			{
				this.Map.Remove(propertyName, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the listeners which have been associated
		/// with the named property.
		/// </summary>
		/// <param name="propertyName">  The name of the property being listened to </param>
		/// <returns> all of the <code>PropertyChangeListeners</code> associated with
		///         the named property.  If no such listeners have been added,
		///         or if <code>propertyName</code> is null, an empty array is
		///         returned.
		/// @since 1.4 </returns>
		public virtual PropertyChangeListener[] GetPropertyChangeListeners(String propertyName)
		{
			return this.Map.GetListeners(propertyName);
		}

		/// <summary>
		/// Reports a bound property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal and non-null.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#firePropertyChange(PropertyChangeEvent)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		public virtual void FirePropertyChange(String propertyName, Object oldValue, Object newValue)
		{
			if (oldValue == null || newValue == null || !oldValue.Equals(newValue))
			{
				FirePropertyChange(new PropertyChangeEvent(this.Source, propertyName, oldValue, newValue));
			}
		}

		/// <summary>
		/// Reports an integer bound property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#firePropertyChange(String, Object, Object)"/>  method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		public virtual void FirePropertyChange(String propertyName, int oldValue, int newValue)
		{
			if (oldValue != newValue)
			{
				FirePropertyChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
			}
		}

		/// <summary>
		/// Reports a boolean bound property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#firePropertyChange(String, Object, Object)"/>  method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		public virtual void FirePropertyChange(String propertyName, bool oldValue, bool newValue)
		{
			if (oldValue != newValue)
			{
				FirePropertyChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
			}
		}

		/// <summary>
		/// Fires a property change event to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if the given event's old and new values are equal and non-null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="event">  the {@code PropertyChangeEvent} to be fired </param>
		public virtual void FirePropertyChange(PropertyChangeEvent @event)
		{
			Object oldValue = @event.OldValue;
			Object newValue = @event.NewValue;
			if (oldValue == null || newValue == null || !oldValue.Equals(newValue))
			{
				String name = @event.PropertyName;

				PropertyChangeListener[] common = this.Map.Get(null);
				PropertyChangeListener[] named = (name != null) ? this.Map.Get(name) : null;

				Fire(common, @event);
				Fire(named, @event);
			}
		}

		private static void Fire(PropertyChangeListener[] listeners, PropertyChangeEvent @event)
		{
			if (listeners != null)
			{
				foreach (PropertyChangeListener listener in listeners)
				{
					listener.PropertyChange(@event);
				}
			}
		}

		/// <summary>
		/// Reports a bound indexed property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal and non-null.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#firePropertyChange(PropertyChangeEvent)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="index">         the index of the property element that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property
		/// @since 1.5 </param>
		public virtual void FireIndexedPropertyChange(String propertyName, int index, Object oldValue, Object newValue)
		{
			if (oldValue == null || newValue == null || !oldValue.Equals(newValue))
			{
				FirePropertyChange(new IndexedPropertyChangeEvent(Source, propertyName, oldValue, newValue, index));
			}
		}

		/// <summary>
		/// Reports an integer bound indexed property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#fireIndexedPropertyChange(String, int, Object, Object)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="index">         the index of the property element that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property
		/// @since 1.5 </param>
		public virtual void FireIndexedPropertyChange(String propertyName, int index, int oldValue, int newValue)
		{
			if (oldValue != newValue)
			{
				FireIndexedPropertyChange(propertyName, index, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
			}
		}

		/// <summary>
		/// Reports a boolean bound indexed property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#fireIndexedPropertyChange(String, int, Object, Object)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that was changed </param>
		/// <param name="index">         the index of the property element that was changed </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property
		/// @since 1.5 </param>
		public virtual void FireIndexedPropertyChange(String propertyName, int index, bool oldValue, bool newValue)
		{
			if (oldValue != newValue)
			{
				FireIndexedPropertyChange(propertyName, index, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
			}
		}

		/// <summary>
		/// Check if there are any listeners for a specific property, including
		/// those registered on all properties.  If <code>propertyName</code>
		/// is null, only check for listeners registered on all properties.
		/// </summary>
		/// <param name="propertyName">  the property name. </param>
		/// <returns> true if there are one or more listeners for the given property </returns>
		public virtual bool HasListeners(String propertyName)
		{
			return this.Map.HasListeners(propertyName);
		}

		/// <summary>
		/// @serialData Null terminated list of <code>PropertyChangeListeners</code>.
		/// <para>
		/// At serialization time we skip non-serializable listeners and
		/// only serialize the serializable listeners.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			Dictionary<String, PropertyChangeSupport> children = null;
			PropertyChangeListener[] listeners = null;
			lock (this.Map)
			{
				foreach (Map_Entry<String, PropertyChangeListener[]> entry in this.Map.Entries)
				{
					String property = entry.Key;
					if (property == null)
					{
						listeners = entry.Value;
					}
					else
					{
						if (children == null)
						{
							children = new Dictionary<>();
						}
						PropertyChangeSupport pcs = new PropertyChangeSupport(this.Source);
						pcs.Map.Set(null, entry.Value);
						children[property] = pcs;
					}
				}
			}
			ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("children", children);
			fields.Put("source", this.Source);
			fields.Put("propertyChangeSupportSerializedDataVersion", 2);
			s.WriteFields();

			if (listeners != null)
			{
				foreach (PropertyChangeListener l in listeners)
				{
					if (l is Serializable)
					{
						s.WriteObject(l);
					}
				}
			}
			s.WriteObject(null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			this.Map = new PropertyChangeListenerMap();

			ObjectInputStream.GetField fields = s.ReadFields();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<String, PropertyChangeSupport> children = (java.util.Hashtable<String, PropertyChangeSupport>) fields.get("children", null);
			Dictionary<String, PropertyChangeSupport> children = (Dictionary<String, PropertyChangeSupport>) fields.Get("children", null);
			this.Source = fields.Get("source", null);
			fields.Get("propertyChangeSupportSerializedDataVersion", 2);

			Object listenerOrNull;
			while (null != (listenerOrNull = s.ReadObject()))
			{
				this.Map.Add(null, (PropertyChangeListener)listenerOrNull);
			}
			if (children != null)
			{
				foreach (Map_Entry<String, PropertyChangeSupport> entry in children)
				{
					foreach (PropertyChangeListener listener in entry.Value.PropertyChangeListeners)
					{
						this.Map.Add(entry.Key, listener);
					}
				}
			}
		}

		/// <summary>
		/// The object to be provided as the "source" for any generated events.
		/// </summary>
		private Object Source;

		/// <summary>
		/// @serialField children                                   Hashtable
		/// @serialField source                                     Object
		/// @serialField propertyChangeSupportSerializedDataVersion int
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("children", typeof(Hashtable)), new ObjectStreamField("source", typeof(Object)), new ObjectStreamField("propertyChangeSupportSerializedDataVersion", Integer.TYPE)};

		/// <summary>
		/// Serialization version ID, so we're compatible with JDK 1.1
		/// </summary>
		internal const long SerialVersionUID = 6401253773779951803L;

		/// <summary>
		/// This is a <seealso cref="ChangeListenerMap ChangeListenerMap"/> implementation
		/// that works with <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
		/// </summary>
		private sealed class PropertyChangeListenerMap : ChangeListenerMap<PropertyChangeListener>
		{
			internal static readonly PropertyChangeListener[] EMPTY = new PropertyChangeListener[] {};

			/// <summary>
			/// Creates an array of <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
			/// This method uses the same instance of the empty array
			/// when {@code length} equals {@code 0}.
			/// </summary>
			/// <param name="length">  the array length </param>
			/// <returns>        an array with specified length </returns>
			protected internal override PropertyChangeListener[] NewArray(int length)
			{
				return (0 < length) ? new PropertyChangeListener[length] : EMPTY;
			}

			/// <summary>
			/// Creates a <seealso cref="PropertyChangeListenerProxy PropertyChangeListenerProxy"/>
			/// object for the specified property.
			/// </summary>
			/// <param name="name">      the name of the property to listen on </param>
			/// <param name="listener">  the listener to process events </param>
			/// <returns>          a {@code PropertyChangeListenerProxy} object </returns>
			protected internal override PropertyChangeListener NewProxy(String name, PropertyChangeListener listener)
			{
				return new PropertyChangeListenerProxy(name, listener);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			public PropertyChangeListener Extract(PropertyChangeListener listener)
			{
				while (listener is PropertyChangeListenerProxy)
				{
					listener = ((PropertyChangeListenerProxy) listener).Listener;
				}
				return listener;
			}
		}
	}

}