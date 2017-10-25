using System.Collections.Generic;

/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// This is an abstract class that provides base functionality
	/// for the <seealso cref="PropertyChangeSupport PropertyChangeSupport"/> class
	/// and the <seealso cref="VetoableChangeSupport VetoableChangeSupport"/> class.
	/// </summary>
	/// <seealso cref= PropertyChangeListenerMap </seealso>
	/// <seealso cref= VetoableChangeListenerMap
	/// 
	/// @author Sergey A. Malenkov </seealso>
	internal abstract class ChangeListenerMap<L> where L : java.util.EventListener
	{
		private IDictionary<String, L[]> Map;

		/// <summary>
		/// Creates an array of listeners.
		/// This method can be optimized by using
		/// the same instance of the empty array
		/// when {@code length} is equal to {@code 0}.
		/// </summary>
		/// <param name="length">  the array length </param>
		/// <returns>        an array with specified length </returns>
		protected internal abstract L[] NewArray(int length);

		/// <summary>
		/// Creates a proxy listener for the specified property.
		/// </summary>
		/// <param name="name">      the name of the property to listen on </param>
		/// <param name="listener">  the listener to process events </param>
		/// <returns>          a proxy listener </returns>
		protected internal abstract L NewProxy(String name, L listener);

		/// <summary>
		/// Adds a listener to the list of listeners for the specified property.
		/// This listener is called as many times as it was added.
		/// </summary>
		/// <param name="name">      the name of the property to listen on </param>
		/// <param name="listener">  the listener to process events </param>
		public void Add(String name, L listener)
		{
			lock (this)
			{
				if (this.Map == null)
				{
					this.Map = new Dictionary<>();
				}
				L[] array = this.Map[name];
				int size = (array != null) ? array.Length : 0;
        
				L[] clone = NewArray(size + 1);
				clone[size] = listener;
				if (array != null)
				{
					System.Array.Copy(array, 0, clone, 0, size);
				}
				this.Map[name] = clone;
			}
		}

		/// <summary>
		/// Removes a listener from the list of listeners for the specified property.
		/// If the listener was added more than once to the same event source,
		/// this listener will be notified one less time after being removed.
		/// </summary>
		/// <param name="name">      the name of the property to listen on </param>
		/// <param name="listener">  the listener to process events </param>
		public void Remove(String name, L listener)
		{
			lock (this)
			{
				if (this.Map != null)
				{
					L[] array = this.Map[name];
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (listener.Equals(array[i]))
							{
								int size = array.Length - 1;
								if (size > 0)
								{
									L[] clone = NewArray(size);
									System.Array.Copy(array, 0, clone, 0, i);
									System.Array.Copy(array, i + 1, clone, i, size - i);
									this.Map[name] = clone;
								}
								else
								{
									this.Map.Remove(name);
									if (this.Map.Count == 0)
									{
										this.Map = null;
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the list of listeners for the specified property.
		/// </summary>
		/// <param name="name">  the name of the property </param>
		/// <returns>      the corresponding list of listeners </returns>
		public L[] Get(String name)
		{
			lock (this)
			{
				return (this.Map != null) ? this.Map[name] : null;
			}
		}

		/// <summary>
		/// Sets new list of listeners for the specified property.
		/// </summary>
		/// <param name="name">       the name of the property </param>
		/// <param name="listeners">  new list of listeners </param>
		public void Set(String name, L[] listeners)
		{
			if (listeners != null)
			{
				if (this.Map == null)
				{
					this.Map = new Dictionary<>();
				}
				this.Map[name] = listeners;
			}
			else if (this.Map != null)
			{
				this.Map.Remove(name);
				if (this.Map.Count == 0)
				{
					this.Map = null;
				}
			}
		}

		/// <summary>
		/// Returns all listeners in the map.
		/// </summary>
		/// <returns> an array of all listeners </returns>
		public L[] Listeners
		{
			get
			{
				lock (this)
				{
					if (this.Map == null)
					{
						return NewArray(0);
					}
					IList<L> list = new List<L>();
            
					L[] listeners = this.Map[null];
					if (listeners != null)
					{
						foreach (L listener in listeners)
						{
							list.Add(listener);
						}
					}
					foreach (Map_Entry<String, L[]> entry in this.Map)
					{
						String name = entry.Key;
						if (name != null)
						{
							foreach (L listener in entry.Value)
							{
								list.Add(NewProxy(name, listener));
							}
						}
					}
					return list.toArray(NewArray(list.Count));
				}
			}
		}

		/// <summary>
		/// Returns listeners that have been associated with the named property.
		/// </summary>
		/// <param name="name">  the name of the property </param>
		/// <returns> an array of listeners for the named property </returns>
		public L[] GetListeners(String name)
		{
			if (name != null)
			{
				L[] listeners = Get(name);
				if (listeners != null)
				{
					return listeners.clone();
				}
			}
			return NewArray(0);
		}

		/// <summary>
		/// Indicates whether the map contains
		/// at least one listener to be notified.
		/// </summary>
		/// <param name="name">  the name of the property </param>
		/// <returns>      {@code true} if at least one listener exists or
		///              {@code false} otherwise </returns>
		public bool HasListeners(String name)
		{
			lock (this)
			{
				if (this.Map == null)
				{
					return false;
				}
				L[] array = this.Map[null];
				return (array != null) || ((name != null) && (null != this.Map[name]));
			}
		}

		/// <summary>
		/// Returns a set of entries from the map.
		/// Each entry is a pair consisted of the property name
		/// and the corresponding list of listeners.
		/// </summary>
		/// <returns> a set of entries from the map </returns>
		public Set<Map_Entry<String, L[]>> Entries
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
				return (this.Map != null) ? this.Map.entrySet() : System.Linq.Enumerable.Empty<Map_Entry<String, L[]>>();
			}
		}

		/// <summary>
		/// Extracts a real listener from the proxy listener.
		/// It is necessary because default proxy class is not serializable.
		/// </summary>
		/// <returns> a real listener </returns>
		public abstract L Extract(L listener);
	}

}