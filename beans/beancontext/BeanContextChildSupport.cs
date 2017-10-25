using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans.beancontext
{




	/// <summary>
	/// <para>
	/// This is a general support class to provide support for implementing the
	/// BeanContextChild protocol.
	/// 
	/// This class may either be directly subclassed, or encapsulated and delegated
	/// to in order to implement this interface for a given component.
	/// </para>
	/// 
	/// @author      Laurence P. G. Cable
	/// @since       1.2
	/// </summary>
	/// <seealso cref= java.beans.beancontext.BeanContext </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextServices </seealso>
	/// <seealso cref= java.beans.beancontext.BeanContextChild </seealso>

	[Serializable]
	public class BeanContextChildSupport : BeanContextChild, BeanContextServicesListener
	{

		internal const long SerialVersionUID = 6328947014421475877L;

		/// <summary>
		/// construct a BeanContextChildSupport where this class has been
		/// subclassed in order to implement the JavaBean component itself.
		/// </summary>

		public BeanContextChildSupport() : base()
		{

			BeanContextChildPeer_Renamed = this;

			PcSupport = new PropertyChangeSupport(BeanContextChildPeer_Renamed);
			VcSupport = new VetoableChangeSupport(BeanContextChildPeer_Renamed);
		}

		/// <summary>
		/// construct a BeanContextChildSupport where the JavaBean component
		/// itself implements BeanContextChild, and encapsulates this, delegating
		/// that interface to this implementation </summary>
		/// <param name="bcc"> the underlying bean context child </param>

		public BeanContextChildSupport(BeanContextChild bcc) : base()
		{

			BeanContextChildPeer_Renamed = (bcc != null) ? bcc : this;

			PcSupport = new PropertyChangeSupport(BeanContextChildPeer_Renamed);
			VcSupport = new VetoableChangeSupport(BeanContextChildPeer_Renamed);
		}

		/// <summary>
		/// Sets the <code>BeanContext</code> for
		/// this <code>BeanContextChildSupport</code>. </summary>
		/// <param name="bc"> the new value to be assigned to the <code>BeanContext</code>
		/// property </param>
		/// <exception cref="PropertyVetoException"> if the change is rejected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setBeanContext(BeanContext bc) throws java.beans.PropertyVetoException
		public virtual BeanContext BeanContext
		{
			set
			{
				lock (this)
				{
					if (value == BeanContext_Renamed)
					{
						return;
					}
            
					BeanContext oldValue = BeanContext_Renamed;
					BeanContext newValue = value;
            
					if (!RejectedSetBCOnce)
					{
						if (RejectedSetBCOnce = !ValidatePendingSetBeanContext(value))
						{
							throw new PropertyVetoException("setBeanContext() change rejected:", new PropertyChangeEvent(BeanContextChildPeer_Renamed, "beanContext", oldValue, newValue)
						   );
						}
            
						try
						{
							FireVetoableChange("beanContext", oldValue, newValue);
						}
						catch (PropertyVetoException pve)
						{
							RejectedSetBCOnce = true;
            
							throw pve; // re-throw
						}
					}
            
					if (BeanContext_Renamed != null)
					{
						ReleaseBeanContextResources();
					}
            
					BeanContext_Renamed = newValue;
					RejectedSetBCOnce = false;
            
					FirePropertyChange("beanContext", oldValue, newValue);
            
					if (BeanContext_Renamed != null)
					{
						InitializeBeanContextResources();
					}
				}
			}
			get
			{
				lock (this)
				{
					return BeanContext_Renamed;
				}
			}
		}


		/// <summary>
		/// Add a PropertyChangeListener for a specific property.
		/// The same listener object may be added more than once.  For each
		/// property,  the listener will be invoked the number of times it was added
		/// for that property.
		/// If <code>name</code> or <code>pcl</code> is null, no exception is thrown
		/// and no action is taken.
		/// </summary>
		/// <param name="name"> The name of the property to listen on </param>
		/// <param name="pcl"> The <code>PropertyChangeListener</code> to be added </param>
		public virtual void AddPropertyChangeListener(String name, PropertyChangeListener pcl)
		{
			PcSupport.AddPropertyChangeListener(name, pcl);
		}

		/// <summary>
		/// Remove a PropertyChangeListener for a specific property.
		/// If <code>pcl</code> was added more than once to the same event
		/// source for the specified property, it will be notified one less time
		/// after being removed.
		/// If <code>name</code> is null, no exception is thrown
		/// and no action is taken.
		/// If <code>pcl</code> is null, or was never added for the specified
		/// property, no exception is thrown and no action is taken.
		/// </summary>
		/// <param name="name"> The name of the property that was listened on </param>
		/// <param name="pcl"> The PropertyChangeListener to be removed </param>
		public virtual void RemovePropertyChangeListener(String name, PropertyChangeListener pcl)
		{
			PcSupport.RemovePropertyChangeListener(name, pcl);
		}

		/// <summary>
		/// Add a VetoableChangeListener for a specific property.
		/// The same listener object may be added more than once.  For each
		/// property,  the listener will be invoked the number of times it was added
		/// for that property.
		/// If <code>name</code> or <code>vcl</code> is null, no exception is thrown
		/// and no action is taken.
		/// </summary>
		/// <param name="name"> The name of the property to listen on </param>
		/// <param name="vcl"> The <code>VetoableChangeListener</code> to be added </param>
		public virtual void AddVetoableChangeListener(String name, VetoableChangeListener vcl)
		{
			VcSupport.AddVetoableChangeListener(name, vcl);
		}

		/// <summary>
		/// Removes a <code>VetoableChangeListener</code>.
		/// If <code>pcl</code> was added more than once to the same event
		/// source for the specified property, it will be notified one less time
		/// after being removed.
		/// If <code>name</code> is null, no exception is thrown
		/// and no action is taken.
		/// If <code>vcl</code> is null, or was never added for the specified
		/// property, no exception is thrown and no action is taken.
		/// </summary>
		/// <param name="name"> The name of the property that was listened on </param>
		/// <param name="vcl"> The <code>VetoableChangeListener</code> to be removed </param>
		public virtual void RemoveVetoableChangeListener(String name, VetoableChangeListener vcl)
		{
			VcSupport.RemoveVetoableChangeListener(name, vcl);
		}

		/// <summary>
		/// A service provided by the nesting BeanContext has been revoked.
		/// 
		/// Subclasses may override this method in order to implement their own
		/// behaviors. </summary>
		/// <param name="bcsre"> The <code>BeanContextServiceRevokedEvent</code> fired as a
		/// result of a service being revoked </param>
		public virtual void ServiceRevoked(BeanContextServiceRevokedEvent bcsre)
		{
		}

		/// <summary>
		/// A new service is available from the nesting BeanContext.
		/// 
		/// Subclasses may override this method in order to implement their own
		/// behaviors </summary>
		/// <param name="bcsae"> The BeanContextServiceAvailableEvent fired as a
		/// result of a service becoming available
		///  </param>
		public virtual void ServiceAvailable(BeanContextServiceAvailableEvent bcsae)
		{
		}

		/// <summary>
		/// Gets the <tt>BeanContextChild</tt> associated with this
		/// <tt>BeanContextChildSupport</tt>.
		/// </summary>
		/// <returns> the <tt>BeanContextChild</tt> peer of this class </returns>
		public virtual BeanContextChild BeanContextChildPeer
		{
			get
			{
				return BeanContextChildPeer_Renamed;
			}
		}

		/// <summary>
		/// Reports whether or not this class is a delegate of another.
		/// </summary>
		/// <returns> true if this class is a delegate of another </returns>
		public virtual bool Delegated
		{
			get
			{
				return !this.Equals(BeanContextChildPeer_Renamed);
			}
		}

		/// <summary>
		/// Report a bound property update to any registered listeners. No event is
		/// fired if old and new are equal and non-null. </summary>
		/// <param name="name"> The programmatic name of the property that was changed </param>
		/// <param name="oldValue">  The old value of the property </param>
		/// <param name="newValue">  The new value of the property </param>
		public virtual void FirePropertyChange(String name, Object oldValue, Object newValue)
		{
			PcSupport.FirePropertyChange(name, oldValue, newValue);
		}

		/// <summary>
		/// Report a vetoable property update to any registered listeners.
		/// If anyone vetos the change, then fire a new event
		/// reverting everyone to the old value and then rethrow
		/// the PropertyVetoException. <P>
		/// 
		/// No event is fired if old and new are equal and non-null.
		/// <P> </summary>
		/// <param name="name"> The programmatic name of the property that is about to
		/// change
		/// </param>
		/// <param name="oldValue"> The old value of the property </param>
		/// <param name="newValue"> - The new value of the property
		/// </param>
		/// <exception cref="PropertyVetoException"> if the recipient wishes the property
		/// change to be rolled back. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireVetoableChange(String name, Object oldValue, Object newValue) throws java.beans.PropertyVetoException
		public virtual void FireVetoableChange(String name, Object oldValue, Object newValue)
		{
			VcSupport.FireVetoableChange(name, oldValue, newValue);
		}

		/// <summary>
		/// Called from setBeanContext to validate (or otherwise) the
		/// pending change in the nesting BeanContext property value.
		/// Returning false will cause setBeanContext to throw
		/// PropertyVetoException. </summary>
		/// <param name="newValue"> the new value that has been requested for
		///  the BeanContext property </param>
		/// <returns> <code>true</code> if the change operation is to be vetoed </returns>
		public virtual bool ValidatePendingSetBeanContext(BeanContext newValue)
		{
			return true;
		}

		/// <summary>
		/// This method may be overridden by subclasses to provide their own
		/// release behaviors. When invoked any resources held by this instance
		/// obtained from its current BeanContext property should be released
		/// since the object is no longer nested within that BeanContext.
		/// </summary>

		protected internal virtual void ReleaseBeanContextResources()
		{
			// do nothing
		}

		/// <summary>
		/// This method may be overridden by subclasses to provide their own
		/// initialization behaviors. When invoked any resources required by the
		/// BeanContextChild should be obtained from the current BeanContext.
		/// </summary>

		protected internal virtual void InitializeBeanContextResources()
		{
			// do nothing
		}

		/// <summary>
		/// Write the persistence state of the object.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(ObjectOutputStream oos)
		{

			/*
			 * don't serialize if we are delegated and the delegator is not also
			 * serializable.
			 */

			if (!Equals(BeanContextChildPeer_Renamed) && !(BeanContextChildPeer_Renamed is Serializable))
			{
				throw new IOException("BeanContextChildSupport beanContextChildPeer not Serializable");
			}

			else
			{
				oos.DefaultWriteObject();
			}

		}


		/// <summary>
		/// Restore a persistent object, must wait for subsequent setBeanContext()
		/// to fully restore any resources obtained from the new nesting
		/// BeanContext
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream ois)
		{
			ois.DefaultReadObject();
		}

		/*
		 * fields
		 */

		/// <summary>
		/// The <code>BeanContext</code> in which
		/// this <code>BeanContextChild</code> is nested.
		/// </summary>
		public BeanContextChild BeanContextChildPeer_Renamed;

	   /// <summary>
	   /// The <tt>PropertyChangeSupport</tt> associated with this
	   /// <tt>BeanContextChildSupport</tt>.
	   /// </summary>
		protected internal PropertyChangeSupport PcSupport;

	   /// <summary>
	   /// The <tt>VetoableChangeSupport</tt> associated with this
	   /// <tt>BeanContextChildSupport</tt>.
	   /// </summary>
		protected internal VetoableChangeSupport VcSupport;

		/// <summary>
		/// The bean context.
		/// </summary>
		[NonSerialized]
		protected internal BeanContext BeanContext_Renamed;

	   /// <summary>
	   /// A flag indicating that there has been
	   /// at least one <code>PropertyChangeVetoException</code>
	   /// thrown for the attempted setBeanContext operation.
	   /// </summary>
		[NonSerialized]
		protected internal bool RejectedSetBCOnce;

	}

}