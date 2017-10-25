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
	/// This is a utility class that can be used by beans that support constrained
	/// properties.  It manages a list of listeners and dispatches
	/// <seealso cref="PropertyChangeEvent"/>s to them.  You can use an instance of this class
	/// as a member field of your bean and delegate these types of work to it.
	/// The <seealso cref="VetoableChangeListener"/> can be registered for all properties
	/// or for a property specified by name.
	/// <para>
	/// Here is an example of {@code VetoableChangeSupport} usage that follows
	/// the rules and recommendations laid out in the JavaBeans&trade; specification:
	/// <pre>{@code
	/// public class MyBean {
	///     private final VetoableChangeSupport vcs = new VetoableChangeSupport(this);
	/// 
	///     public void addVetoableChangeListener(VetoableChangeListener listener) {
	///         this.vcs.addVetoableChangeListener(listener);
	///     }
	/// 
	///     public void removeVetoableChangeListener(VetoableChangeListener listener) {
	///         this.vcs.removeVetoableChangeListener(listener);
	///     }
	/// 
	///     private String value;
	/// 
	///     public String getValue() {
	///         return this.value;
	///     }
	/// 
	///     public void setValue(String newValue) throws PropertyVetoException {
	///         String oldValue = this.value;
	///         this.vcs.fireVetoableChange("value", oldValue, newValue);
	///         this.value = newValue;
	///     }
	/// 
	///     [...]
	/// }
	/// }</pre>
	/// </para>
	/// <para>
	/// A {@code VetoableChangeSupport} instance is thread-safe.
	/// </para>
	/// <para>
	/// This class is serializable.  When it is serialized it will save
	/// (and restore) any listeners that are themselves serializable.  Any
	/// non-serializable listeners will be skipped during serialization.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= PropertyChangeSupport </seealso>
	[Serializable]
	public class VetoableChangeSupport
	{
		private VetoableChangeListenerMap Map = new VetoableChangeListenerMap();

		/// <summary>
		/// Constructs a <code>VetoableChangeSupport</code> object.
		/// </summary>
		/// <param name="sourceBean">  The bean to be given as the source for any events. </param>
		public VetoableChangeSupport(Object sourceBean)
		{
			if (sourceBean == null)
			{
				throw new NullPointerException();
			}
			Source = sourceBean;
		}

		/// <summary>
		/// Add a VetoableChangeListener to the listener list.
		/// The listener is registered for all properties.
		/// The same listener object may be added more than once, and will be called
		/// as many times as it is added.
		/// If <code>listener</code> is null, no exception is thrown and no action
		/// is taken.
		/// </summary>
		/// <param name="listener">  The VetoableChangeListener to be added </param>
		public virtual void AddVetoableChangeListener(VetoableChangeListener listener)
		{
			if (listener == null)
			{
				return;
			}
			if (listener is VetoableChangeListenerProxy)
			{
				VetoableChangeListenerProxy proxy = (VetoableChangeListenerProxy)listener;
				// Call two argument add method.
				AddVetoableChangeListener(proxy.PropertyName, proxy.Listener);
			}
			else
			{
				this.Map.Add(null, listener);
			}
		}

		/// <summary>
		/// Remove a VetoableChangeListener from the listener list.
		/// This removes a VetoableChangeListener that was registered
		/// for all properties.
		/// If <code>listener</code> was added more than once to the same event
		/// source, it will be notified one less time after being removed.
		/// If <code>listener</code> is null, or was never added, no exception is
		/// thrown and no action is taken.
		/// </summary>
		/// <param name="listener">  The VetoableChangeListener to be removed </param>
		public virtual void RemoveVetoableChangeListener(VetoableChangeListener listener)
		{
			if (listener == null)
			{
				return;
			}
			if (listener is VetoableChangeListenerProxy)
			{
				VetoableChangeListenerProxy proxy = (VetoableChangeListenerProxy)listener;
				// Call two argument remove method.
				RemoveVetoableChangeListener(proxy.PropertyName, proxy.Listener);
			}
			else
			{
				this.Map.Remove(null, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the listeners that were added to the
		/// VetoableChangeSupport object with addVetoableChangeListener().
		/// <para>
		/// If some listeners have been added with a named property, then
		/// the returned array will be a mixture of VetoableChangeListeners
		/// and <code>VetoableChangeListenerProxy</code>s. If the calling
		/// method is interested in distinguishing the listeners then it must
		/// test each element to see if it's a
		/// <code>VetoableChangeListenerProxy</code>, perform the cast, and examine
		/// the parameter.
		/// 
		/// <pre>{@code
		/// VetoableChangeListener[] listeners = bean.getVetoableChangeListeners();
		/// for (int i = 0; i < listeners.length; i++) {
		///        if (listeners[i] instanceof VetoableChangeListenerProxy) {
		///     VetoableChangeListenerProxy proxy =
		///                    (VetoableChangeListenerProxy)listeners[i];
		///     if (proxy.getPropertyName().equals("foo")) {
		///       // proxy is a VetoableChangeListener which was associated
		///       // with the property named "foo"
		///     }
		///   }
		/// }
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= VetoableChangeListenerProxy </seealso>
		/// <returns> all of the <code>VetoableChangeListeners</code> added or an
		///         empty array if no listeners have been added
		/// @since 1.4 </returns>
		public virtual VetoableChangeListener[] VetoableChangeListeners
		{
			get
			{
				return this.Map.Listeners;
			}
		}

		/// <summary>
		/// Add a VetoableChangeListener for a specific property.  The listener
		/// will be invoked only when a call on fireVetoableChange names that
		/// specific property.
		/// The same listener object may be added more than once.  For each
		/// property,  the listener will be invoked the number of times it was added
		/// for that property.
		/// If <code>propertyName</code> or <code>listener</code> is null, no
		/// exception is thrown and no action is taken.
		/// </summary>
		/// <param name="propertyName">  The name of the property to listen on. </param>
		/// <param name="listener">  The VetoableChangeListener to be added </param>
		public virtual void AddVetoableChangeListener(String propertyName, VetoableChangeListener listener)
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
		/// Remove a VetoableChangeListener for a specific property.
		/// If <code>listener</code> was added more than once to the same event
		/// source for the specified property, it will be notified one less time
		/// after being removed.
		/// If <code>propertyName</code> is null, no exception is thrown and no
		/// action is taken.
		/// If <code>listener</code> is null, or was never added for the specified
		/// property, no exception is thrown and no action is taken.
		/// </summary>
		/// <param name="propertyName">  The name of the property that was listened on. </param>
		/// <param name="listener">  The VetoableChangeListener to be removed </param>
		public virtual void RemoveVetoableChangeListener(String propertyName, VetoableChangeListener listener)
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
		/// <returns> all the <code>VetoableChangeListeners</code> associated with
		///         the named property.  If no such listeners have been added,
		///         or if <code>propertyName</code> is null, an empty array is
		///         returned.
		/// @since 1.4 </returns>
		public virtual VetoableChangeListener[] GetVetoableChangeListeners(String propertyName)
		{
			return this.Map.GetListeners(propertyName);
		}

		/// <summary>
		/// Reports a constrained property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// Any listener can throw a {@code PropertyVetoException} to veto the update.
		/// If one of the listeners vetoes the update, this method passes
		/// a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		/// to all listeners that already confirmed this update
		/// and throws the {@code PropertyVetoException} again.
		/// </para>
		/// <para>
		/// No event is fired if old and new values are equal and non-null.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#fireVetoableChange(PropertyChangeEvent)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that is about to change </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		/// <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireVetoableChange(String propertyName, Object oldValue, Object newValue) throws PropertyVetoException
		public virtual void FireVetoableChange(String propertyName, Object oldValue, Object newValue)
		{
			if (oldValue == null || newValue == null || !oldValue.Equals(newValue))
			{
				FireVetoableChange(new PropertyChangeEvent(this.Source, propertyName, oldValue, newValue));
			}
		}

		/// <summary>
		/// Reports an integer constrained property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// Any listener can throw a {@code PropertyVetoException} to veto the update.
		/// If one of the listeners vetoes the update, this method passes
		/// a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		/// to all listeners that already confirmed this update
		/// and throws the {@code PropertyVetoException} again.
		/// </para>
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#fireVetoableChange(String, Object, Object)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that is about to change </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		/// <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireVetoableChange(String propertyName, int oldValue, int newValue) throws PropertyVetoException
		public virtual void FireVetoableChange(String propertyName, int oldValue, int newValue)
		{
			if (oldValue != newValue)
			{
				FireVetoableChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
			}
		}

		/// <summary>
		/// Reports a boolean constrained property update to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// Any listener can throw a {@code PropertyVetoException} to veto the update.
		/// If one of the listeners vetoes the update, this method passes
		/// a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		/// to all listeners that already confirmed this update
		/// and throws the {@code PropertyVetoException} again.
		/// </para>
		/// <para>
		/// No event is fired if old and new values are equal.
		/// </para>
		/// <para>
		/// This is merely a convenience wrapper around the more general
		/// <seealso cref="#fireVetoableChange(String, Object, Object)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName">  the programmatic name of the property that is about to change </param>
		/// <param name="oldValue">      the old value of the property </param>
		/// <param name="newValue">      the new value of the property </param>
		/// <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireVetoableChange(String propertyName, boolean oldValue, boolean newValue) throws PropertyVetoException
		public virtual void FireVetoableChange(String propertyName, bool oldValue, bool newValue)
		{
			if (oldValue != newValue)
			{
				FireVetoableChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
			}
		}

		/// <summary>
		/// Fires a property change event to listeners
		/// that have been registered to track updates of
		/// all properties or a property with the specified name.
		/// <para>
		/// Any listener can throw a {@code PropertyVetoException} to veto the update.
		/// If one of the listeners vetoes the update, this method passes
		/// a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		/// to all listeners that already confirmed this update
		/// and throws the {@code PropertyVetoException} again.
		/// </para>
		/// <para>
		/// No event is fired if the given event's old and new values are equal and non-null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="event">  the {@code PropertyChangeEvent} to be fired </param>
		/// <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireVetoableChange(PropertyChangeEvent event) throws PropertyVetoException
		public virtual void FireVetoableChange(PropertyChangeEvent @event)
		{
			Object oldValue = @event.OldValue;
			Object newValue = @event.NewValue;
			if (oldValue == null || newValue == null || !oldValue.Equals(newValue))
			{
				String name = @event.PropertyName;

				VetoableChangeListener[] common = this.Map.Get(null);
				VetoableChangeListener[] named = (name != null) ? this.Map.Get(name) : null;

				VetoableChangeListener[] listeners;
				if (common == null)
				{
					listeners = named;
				}
				else if (named == null)
				{
					listeners = common;
				}
				else
				{
					listeners = new VetoableChangeListener[common.Length + named.Length];
					System.Array.Copy(common, 0, listeners, 0, common.Length);
					System.Array.Copy(named, 0, listeners, common.Length, named.Length);
				}
				if (listeners != null)
				{
					int current = 0;
					try
					{
						while (current < listeners.Length)
						{
							listeners[current].VetoableChange(@event);
							current++;
						}
					}
					catch (PropertyVetoException veto)
					{
						@event = new PropertyChangeEvent(this.Source, name, newValue, oldValue);
						for (int i = 0; i < current; i++)
						{
							try
							{
								listeners[i].VetoableChange(@event);
							}
							catch (PropertyVetoException)
							{
								// ignore exceptions that occur during rolling back
							}
						}
						throw veto; // rethrow the veto exception
					}
				}
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
		/// @serialData Null terminated list of <code>VetoableChangeListeners</code>.
		/// <para>
		/// At serialization time we skip non-serializable listeners and
		/// only serialize the serializable listeners.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			Dictionary<String, VetoableChangeSupport> children = null;
			VetoableChangeListener[] listeners = null;
			lock (this.Map)
			{
				foreach (Map_Entry<String, VetoableChangeListener[]> entry in this.Map.Entries)
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
						VetoableChangeSupport vcs = new VetoableChangeSupport(this.Source);
						vcs.Map.Set(null, entry.Value);
						children[property] = vcs;
					}
				}
			}
			ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("children", children);
			fields.Put("source", this.Source);
			fields.Put("vetoableChangeSupportSerializedDataVersion", 2);
			s.WriteFields();

			if (listeners != null)
			{
				foreach (VetoableChangeListener l in listeners)
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
			this.Map = new VetoableChangeListenerMap();

			ObjectInputStream.GetField fields = s.ReadFields();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<String, VetoableChangeSupport> children = (java.util.Hashtable<String, VetoableChangeSupport>)fields.get("children", null);
			Dictionary<String, VetoableChangeSupport> children = (Dictionary<String, VetoableChangeSupport>)fields.Get("children", null);
			this.Source = fields.Get("source", null);
			fields.Get("vetoableChangeSupportSerializedDataVersion", 2);

			Object listenerOrNull;
			while (null != (listenerOrNull = s.ReadObject()))
			{
				this.Map.Add(null, (VetoableChangeListener)listenerOrNull);
			}
			if (children != null)
			{
				foreach (Map_Entry<String, VetoableChangeSupport> entry in children)
				{
					foreach (VetoableChangeListener listener in entry.Value.VetoableChangeListeners)
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
		/// @serialField vetoableChangeSupportSerializedDataVersion int
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("children", typeof(Hashtable)), new ObjectStreamField("source", typeof(Object)), new ObjectStreamField("vetoableChangeSupportSerializedDataVersion", Integer.TYPE)};

		/// <summary>
		/// Serialization version ID, so we're compatible with JDK 1.1
		/// </summary>
		internal const long SerialVersionUID = -5090210921595982017L;

		/// <summary>
		/// This is a <seealso cref="ChangeListenerMap ChangeListenerMap"/> implementation
		/// that works with <seealso cref="VetoableChangeListener VetoableChangeListener"/> objects.
		/// </summary>
		private sealed class VetoableChangeListenerMap : ChangeListenerMap<VetoableChangeListener>
		{
			internal static readonly VetoableChangeListener[] EMPTY = new VetoableChangeListener[] {};

			/// <summary>
			/// Creates an array of <seealso cref="VetoableChangeListener VetoableChangeListener"/> objects.
			/// This method uses the same instance of the empty array
			/// when {@code length} equals {@code 0}.
			/// </summary>
			/// <param name="length">  the array length </param>
			/// <returns>        an array with specified length </returns>
			protected internal override VetoableChangeListener[] NewArray(int length)
			{
				return (0 < length) ? new VetoableChangeListener[length] : EMPTY;
			}

			/// <summary>
			/// Creates a <seealso cref="VetoableChangeListenerProxy VetoableChangeListenerProxy"/>
			/// object for the specified property.
			/// </summary>
			/// <param name="name">      the name of the property to listen on </param>
			/// <param name="listener">  the listener to process events </param>
			/// <returns>          a {@code VetoableChangeListenerProxy} object </returns>
			protected internal override VetoableChangeListener NewProxy(String name, VetoableChangeListener listener)
			{
				return new VetoableChangeListenerProxy(name, listener);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			public VetoableChangeListener Extract(VetoableChangeListener listener)
			{
				while (listener is VetoableChangeListenerProxy)
				{
					listener = ((VetoableChangeListenerProxy) listener).Listener;
				}
				return listener;
			}
		}
	}

}