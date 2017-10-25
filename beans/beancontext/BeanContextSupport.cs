using System;
using System.Collections;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This helper class provides a utility implementation of the
	/// java.beans.beancontext.BeanContext interface.
	/// <para>
	/// Since this class directly implements the BeanContext interface, the class
	/// can, and is intended to be used either by subclassing this implementation,
	/// or via ad-hoc delegation of an instance of this class from another.
	/// </para>
	/// 
	/// @author Laurence P. G. Cable
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class BeanContextSupport : BeanContextChildSupport, BeanContext, PropertyChangeListener, VetoableChangeListener
	{

		// Fix for bug 4282900 to pass JCK regression test
		internal new const long SerialVersionUID = -4879613978649577204L;

		/// 
		/// <summary>
		/// Construct a BeanContextSupport instance
		/// 
		/// </summary>
		/// <param name="peer">      The peer <tt>BeanContext</tt> we are
		///                  supplying an implementation for,
		///                  or <tt>null</tt>
		///                  if this object is its own peer </param>
		/// <param name="lcle">      The current Locale for this BeanContext. If
		///                  <tt>lcle</tt> is <tt>null</tt>, the default locale
		///                  is assigned to the <tt>BeanContext</tt> instance. </param>
		/// <param name="dTime">     The initial state,
		///                  <tt>true</tt> if in design mode,
		///                  <tt>false</tt> if runtime. </param>
		/// <param name="visible">   The initial visibility. </param>
		/// <seealso cref= java.util.Locale#getDefault() </seealso>
		/// <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		public BeanContextSupport(BeanContext peer, Locale lcle, bool dTime, bool visible) : base(peer)
		{

			Locale_Renamed = lcle != null ? lcle : Locale.Default;
			DesignTime_Renamed = dTime;
			OkToUseGui_Renamed = visible;

			Initialize();
		}

		/// <summary>
		/// Create an instance using the specified Locale and design mode.
		/// </summary>
		/// <param name="peer">      The peer <tt>BeanContext</tt> we
		///                  are supplying an implementation for,
		///                  or <tt>null</tt> if this object is its own peer </param>
		/// <param name="lcle">      The current Locale for this <tt>BeanContext</tt>. If
		///                  <tt>lcle</tt> is <tt>null</tt>, the default locale
		///                  is assigned to the <tt>BeanContext</tt> instance. </param>
		/// <param name="dtime">     The initial state, <tt>true</tt>
		///                  if in design mode,
		///                  <tt>false</tt> if runtime. </param>
		/// <seealso cref= java.util.Locale#getDefault() </seealso>
		/// <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		public BeanContextSupport(BeanContext peer, Locale lcle, bool dtime) : this(peer, lcle, dtime, true)
		{
		}

		/// <summary>
		/// Create an instance using the specified locale
		/// </summary>
		/// <param name="peer">      The peer BeanContext we are
		///                  supplying an implementation for,
		///                  or <tt>null</tt> if this object
		///                  is its own peer </param>
		/// <param name="lcle">      The current Locale for this
		///                  <tt>BeanContext</tt>. If
		///                  <tt>lcle</tt> is <tt>null</tt>,
		///                  the default locale
		///                  is assigned to the <tt>BeanContext</tt>
		///                  instance. </param>
		/// <seealso cref= java.util.Locale#getDefault() </seealso>
		/// <seealso cref= java.util.Locale#setDefault(java.util.Locale) </seealso>
		public BeanContextSupport(BeanContext peer, Locale lcle) : this(peer, lcle, false, true)
		{
		}

		/// <summary>
		/// Create an instance using with a default locale
		/// </summary>
		/// <param name="peer">      The peer <tt>BeanContext</tt> we are
		///                  supplying an implementation for,
		///                  or <tt>null</tt> if this object
		///                  is its own peer </param>
		public BeanContextSupport(BeanContext peer) : this(peer, null, false, true)
		{
		}

		/// <summary>
		/// Create an instance that is not a delegate of another object
		/// </summary>

		public BeanContextSupport() : this(null, null, false, true)
		{
		}

		/// <summary>
		/// Gets the instance of <tt>BeanContext</tt> that
		/// this object is providing the implementation for. </summary>
		/// <returns> the BeanContext instance </returns>
		public virtual BeanContext BeanContextPeer
		{
			get
			{
				return (BeanContext)BeanContextChildPeer;
			}
		}

		/// <summary>
		/// <para>
		/// The instantiateChild method is a convenience hook
		/// in BeanContext to simplify
		/// the task of instantiating a Bean, nested,
		/// into a <tt>BeanContext</tt>.
		/// </para>
		/// <para>
		/// The semantics of the beanName parameter are defined by java.beans.Beans.instantiate.
		/// </para>
		/// </summary>
		/// <param name="beanName"> the name of the Bean to instantiate within this BeanContext </param>
		/// <exception cref="IOException"> if there is an I/O error when the bean is being deserialized </exception>
		/// <exception cref="ClassNotFoundException"> if the class
		/// identified by the beanName parameter is not found </exception>
		/// <returns> the new object </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object instantiateChild(String beanName) throws java.io.IOException, ClassNotFoundException
		public virtual Object InstantiateChild(String beanName)
		{
			BeanContext bc = BeanContextPeer;

			return Beans.Instantiate(bc.GetType().ClassLoader, beanName, bc);
		}

		/// <summary>
		/// Gets the number of children currently nested in
		/// this BeanContext.
		/// </summary>
		/// <returns> number of children </returns>
		public virtual int Size()
		{
			lock (Children)
			{
				return Children.size();
			}
		}

		/// <summary>
		/// Reports whether or not this
		/// <tt>BeanContext</tt> is empty.
		/// A <tt>BeanContext</tt> is considered
		/// empty when it contains zero
		/// nested children. </summary>
		/// <returns> if there are not children </returns>
		public virtual bool Empty
		{
			get
			{
				lock (Children)
				{
					return Children.Empty;
				}
			}
		}

		/// <summary>
		/// Determines whether or not the specified object
		/// is currently a child of this <tt>BeanContext</tt>. </summary>
		/// <param name="o"> the Object in question </param>
		/// <returns> if this object is a child </returns>
		public virtual bool Contains(Object o)
		{
			lock (Children)
			{
				return Children.containsKey(o);
			}
		}

		/// <summary>
		/// Determines whether or not the specified object
		/// is currently a child of this <tt>BeanContext</tt>. </summary>
		/// <param name="o"> the Object in question </param>
		/// <returns> if this object is a child </returns>
		public virtual bool ContainsKey(Object o)
		{
			lock (Children)
			{
				return Children.containsKey(o);
			}
		}

		/// <summary>
		/// Gets all JavaBean or <tt>BeanContext</tt> instances
		/// currently nested in this <tt>BeanContext</tt>. </summary>
		/// <returns> an <tt>Iterator</tt> of the nested children </returns>
		public virtual IEnumerator Iterator()
		{
			lock (Children)
			{
				return new BCSIterator(Children.Keys.Iterator());
			}
		}

		/// <summary>
		/// Gets all JavaBean or <tt>BeanContext</tt>
		/// instances currently nested in this BeanContext.
		/// </summary>
		public virtual Object[] ToArray()
		{
			lock (Children)
			{
				return Children.Keys.ToArray();
			}
		}

		/// <summary>
		/// Gets an array containing all children of
		/// this <tt>BeanContext</tt> that match
		/// the types contained in arry. </summary>
		/// <param name="arry"> The array of object
		/// types that are of interest. </param>
		/// <returns> an array of children </returns>
		public virtual Object[] ToArray(Object[] arry)
		{
			lock (Children)
			{
				return Children.Keys.ToArray(arry);
			}
		}


		/// <summary>
		///********************************************************************* </summary>

		/// <summary>
		/// protected final subclass that encapsulates an iterator but implements
		/// a noop remove() method.
		/// </summary>

		protected internal sealed class BCSIterator : Iterator
		{
			internal BCSIterator(IEnumerator i) : base()
			{
				Src = i;
			}

			public bool HasNext()
			{
				return Src.hasNext();
			}
			public Object Next()
			{
				return Src.next();
			}
			public void Remove() // do nothing
			{
			}

			internal IEnumerator Src;
		}

		/// <summary>
		///********************************************************************* </summary>

		/*
		 * protected nested class containing per child information, an instance
		 * of which is associated with each child in the "children" hashtable.
		 * subclasses can extend this class to include their own per-child state.
		 *
		 * Note that this 'value' is serialized with the corresponding child 'key'
		 * when the BeanContextSupport is serialized.
		 */

		[Serializable]
		protected internal class BCSChild
		{
			private readonly BeanContextSupport OuterInstance;


		internal const long SerialVersionUID = -5815286101609939109L;

			internal BCSChild(BeanContextSupport outerInstance, Object bcc, Object peer) : base()
			{
				this.OuterInstance = outerInstance;

				Child_Renamed = bcc;
				ProxyPeer_Renamed = peer;
			}

			internal virtual Object Child
			{
				get
				{
					return Child_Renamed;
				}
			}

			internal virtual bool RemovePending
			{
				set
				{
					RemovePending_Renamed = value;
				}
				get
				{
					return RemovePending_Renamed;
				}
			}


			internal virtual bool ProxyPeer
			{
				get
				{
					return ProxyPeer_Renamed != null;
				}
			}

			internal virtual Object ProxyPeer
			{
				get
				{
					return ProxyPeer_Renamed;
				}
			}
			/*
			 * fields
			 */


			internal Object Child_Renamed;
			internal Object ProxyPeer_Renamed;

			[NonSerialized]
			internal bool RemovePending_Renamed;
		}

		/// <summary>
		/// <para>
		/// Subclasses can override this method to insert their own subclass
		/// of Child without having to override add() or the other Collection
		/// methods that add children to the set.
		/// </para> </summary>
		/// <param name="targetChild"> the child to create the Child on behalf of </param> </param>
		/// <param name="peer">        the peer if the tragetChild and the peer are related by an implementation of BeanContextProxy     * <returns> Subtype-specific subclass of Child without overriding collection methods </returns>

		protected internal virtual BCSChild CreateBCSChild(Object targetChild, Object peer)
		{
			return new BCSChild(this, targetChild, peer);
		}

		/// <summary>
		///********************************************************************* </summary>

		/// <summary>
		/// Adds/nests a child within this <tt>BeanContext</tt>.
		/// <para>
		/// Invoked as a side effect of java.beans.Beans.instantiate().
		/// If the child object is not valid for adding then this method
		/// throws an IllegalStateException.
		/// </para>
		/// 
		/// </summary>
		/// <param name="targetChild"> The child objects to nest
		/// within this <tt>BeanContext</tt> </param>
		/// <returns> true if the child was added successfully. </returns>
		/// <seealso cref= #validatePendingAdd </seealso>
		public virtual bool Add(Object targetChild)
		{

			if (targetChild == null)
			{
				throw new IllegalArgumentException();
			}

			// The specification requires that we do nothing if the child
			// is already nested herein.

			if (Children.containsKey(targetChild)) // test before locking
			{
				return false;
			}

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (Children.containsKey(targetChild)) // check again
				{
					return false;
				}

				if (!ValidatePendingAdd(targetChild))
				{
					throw new IllegalStateException();
				}


				// The specification requires that we invoke setBeanContext() on the
				// newly added child if it implements the java.beans.beancontext.BeanContextChild interface

				BeanContextChild cbcc = GetChildBeanContextChild(targetChild);
				BeanContextChild bccp = null;

				lock (targetChild)
				{

					if (targetChild is BeanContextProxy)
					{
						bccp = ((BeanContextProxy)targetChild).BeanContextProxy;

						if (bccp == null)
						{
							throw new NullPointerException("BeanContextPeer.getBeanContextProxy()");
						}
					}

					BCSChild bcsc = CreateBCSChild(targetChild, bccp);
					BCSChild pbcsc = null;

					lock (Children)
					{
						Children.put(targetChild, bcsc);

						if (bccp != null)
						{
							Children.put(bccp, pbcsc = CreateBCSChild(bccp, targetChild));
						}
					}

					if (cbcc != null)
					{
						lock (cbcc)
						{
						try
						{
							cbcc.BeanContext = BeanContextPeer;
						}
						catch (PropertyVetoException)
						{

							lock (Children)
							{
								Children.remove(targetChild);

								if (bccp != null)
								{
									Children.remove(bccp);
								}
							}

							throw new IllegalStateException();
						}

						cbcc.AddPropertyChangeListener("beanContext", ChildPCL);
						cbcc.AddVetoableChangeListener("beanContext", ChildVCL);
						}
					}

					Visibility v = GetChildVisibility(targetChild);

					if (v != null)
					{
						if (OkToUseGui_Renamed)
						{
							v.OkToUseGui();
						}
						else
						{
							v.DontUseGui();
						}
					}

					if (GetChildSerializable(targetChild) != null)
					{
						Serializable++;
					}

					ChildJustAddedHook(targetChild, bcsc);

					if (bccp != null)
					{
						v = GetChildVisibility(bccp);

						if (v != null)
						{
							if (OkToUseGui_Renamed)
							{
								v.OkToUseGui();
							}
							else
							{
								v.DontUseGui();
							}
						}

						if (GetChildSerializable(bccp) != null)
						{
							Serializable++;
						}

						ChildJustAddedHook(bccp, pbcsc);
					}


				}

				// The specification requires that we fire a notification of the change

				FireChildrenAdded(new BeanContextMembershipEvent(BeanContextPeer, bccp == null ? new Object[] {targetChild} : new Object[] {targetChild, bccp}));

			}

			return true;
		}

		/// <summary>
		/// Removes a child from this BeanContext.  If the child object is not
		/// for adding then this method throws an IllegalStateException. </summary>
		/// <param name="targetChild"> The child objects to remove </param>
		/// <seealso cref= #validatePendingRemove </seealso>
		public virtual bool Remove(Object targetChild)
		{
			return Remove(targetChild, true);
		}

		/// <summary>
		/// internal remove used when removal caused by
		/// unexpected <tt>setBeanContext</tt> or
		/// by <tt>remove()</tt> invocation. </summary>
		/// <param name="targetChild"> the JavaBean, BeanContext, or Object to be removed </param>
		/// <param name="callChildSetBC"> used to indicate that
		/// the child should be notified that it is no
		/// longer nested in this <tt>BeanContext</tt>. </param>
		/// <returns> whether or not was present before being removed </returns>
		protected internal virtual bool Remove(Object targetChild, bool callChildSetBC)
		{

			if (targetChild == null)
			{
				throw new IllegalArgumentException();
			}

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (!ContainsKey(targetChild))
				{
					return false;
				}

				if (!ValidatePendingRemove(targetChild))
				{
					throw new IllegalStateException();
				}

				BCSChild bcsc = (BCSChild)Children.get(targetChild);
				BCSChild pbcsc = null;
				Object peer = null;

				// we are required to notify the child that it is no longer nested here if
				// it implements java.beans.beancontext.BeanContextChild

				lock (targetChild)
				{
					if (callChildSetBC)
					{
						BeanContextChild cbcc = GetChildBeanContextChild(targetChild);
						if (cbcc != null)
						{
							lock (cbcc)
							{
							cbcc.RemovePropertyChangeListener("beanContext", ChildPCL);
							cbcc.RemoveVetoableChangeListener("beanContext", ChildVCL);

							try
							{
								cbcc.BeanContext = null;
							}
							catch (PropertyVetoException)
							{
								cbcc.AddPropertyChangeListener("beanContext", ChildPCL);
								cbcc.AddVetoableChangeListener("beanContext", ChildVCL);
								throw new IllegalStateException();
							}

							}
						}
					}

					lock (Children)
					{
						Children.remove(targetChild);

						if (bcsc.ProxyPeer)
						{
							pbcsc = (BCSChild)Children.get(peer = bcsc.ProxyPeer);
							Children.remove(peer);
						}
					}

					if (GetChildSerializable(targetChild) != null)
					{
						Serializable--;
					}

					ChildJustRemovedHook(targetChild, bcsc);

					if (peer != null)
					{
						if (GetChildSerializable(peer) != null)
						{
							Serializable--;
						}

						ChildJustRemovedHook(peer, pbcsc);
					}
				}

				FireChildrenRemoved(new BeanContextMembershipEvent(BeanContextPeer, peer == null ? new Object[] {targetChild} : new Object[] {targetChild, peer}));

			}

			return true;
		}

		/// <summary>
		/// Tests to see if all objects in the
		/// specified <tt>Collection</tt> are children of
		/// this <tt>BeanContext</tt>. </summary>
		/// <param name="c"> the specified <tt>Collection</tt>
		/// </param>
		/// <returns> <tt>true</tt> if all objects
		/// in the collection are children of
		/// this <tt>BeanContext</tt>, false if not. </returns>
		public virtual bool ContainsAll(ICollection c)
		{
			lock (Children)
			{
				IEnumerator i = c.GetEnumerator();
				while (i.hasNext())
				{
					if (!Contains(i.next()))
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// add Collection to set of Children (Unsupported)
		/// implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		/// <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		/// <returns> this implementation unconditionally throws {@code UnsupportedOperationException} </returns>
		public virtual bool AddAll(ICollection c)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// remove all specified children (Unsupported)
		/// implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		/// <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		/// <returns> this implementation unconditionally throws {@code UnsupportedOperationException}
		///  </returns>
		public virtual bool RemoveAll(ICollection c)
		{
			throw new UnsupportedOperationException();
		}


		/// <summary>
		/// retain only specified children (Unsupported)
		/// implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		/// <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		/// <returns> this implementation unconditionally throws {@code UnsupportedOperationException} </returns>
		public virtual bool RetainAll(ICollection c)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// clear the children (Unsupported)
		/// implementations must synchronized on the hierarchy lock and "children" protected field </summary>
		/// <exception cref="UnsupportedOperationException"> thrown unconditionally by this implementation </exception>
		public virtual void Clear()
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Adds a BeanContextMembershipListener
		/// </summary>
		/// <param name="bcml"> the BeanContextMembershipListener to add </param>
		/// <exception cref="NullPointerException"> if the argument is null </exception>

		public virtual void AddBeanContextMembershipListener(BeanContextMembershipListener bcml)
		{
			if (bcml == null)
			{
				throw new NullPointerException("listener");
			}

			lock (BcmListeners)
			{
				if (BcmListeners.contains(bcml))
				{
					return;
				}
				else
				{
					BcmListeners.add(bcml);
				}
			}
		}

		/// <summary>
		/// Removes a BeanContextMembershipListener
		/// </summary>
		/// <param name="bcml"> the BeanContextMembershipListener to remove </param>
		/// <exception cref="NullPointerException"> if the argument is null </exception>

		public virtual void RemoveBeanContextMembershipListener(BeanContextMembershipListener bcml)
		{
			if (bcml == null)
			{
				throw new NullPointerException("listener");
			}

			lock (BcmListeners)
			{
				if (!BcmListeners.contains(bcml))
				{
					return;
				}
				else
				{
					BcmListeners.remove(bcml);
				}
			}
		}

		/// <param name="name"> the name of the resource requested. </param>
		/// <param name="bcc">  the child object making the request.
		/// </param>
		/// <returns>  the requested resource as an InputStream </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>

		public virtual InputStream GetResourceAsStream(String name, BeanContextChild bcc)
		{
			if (name == null)
			{
				throw new NullPointerException("name");
			}
			if (bcc == null)
			{
				throw new NullPointerException("bcc");
			}

			if (ContainsKey(bcc))
			{
				ClassLoader cl = bcc.GetType().ClassLoader;

				return cl != null ? cl.GetResourceAsStream(name) : ClassLoader.GetSystemResourceAsStream(name);
			}
			else
			{
				throw new IllegalArgumentException("Not a valid child");
			}
		}

		/// <param name="name"> the name of the resource requested. </param>
		/// <param name="bcc">  the child object making the request.
		/// </param>
		/// <returns> the requested resource as an InputStream </returns>

		public virtual URL GetResource(String name, BeanContextChild bcc)
		{
			if (name == null)
			{
				throw new NullPointerException("name");
			}
			if (bcc == null)
			{
				throw new NullPointerException("bcc");
			}

			if (ContainsKey(bcc))
			{
				ClassLoader cl = bcc.GetType().ClassLoader;

				return cl != null ? cl.GetResource(name) : ClassLoader.GetSystemResource(name);
			}
			else
			{
				throw new IllegalArgumentException("Not a valid child");
			}
		}

		/// <summary>
		/// Sets the new design time value for this <tt>BeanContext</tt>. </summary>
		/// <param name="dTime"> the new designTime value </param>
		public virtual bool DesignTime
		{
			set
			{
				lock (this)
				{
					if (DesignTime_Renamed != value)
					{
						DesignTime_Renamed = value;
            
						FirePropertyChange("designMode", Convert.ToBoolean(!value), Convert.ToBoolean(value));
					}
				}
			}
			get
			{
				lock (this)
				{
					return DesignTime_Renamed;
				}
			}
		}



		/// <summary>
		/// Sets the locale of this BeanContext. </summary>
		/// <param name="newLocale"> the new locale. This method call will have
		///        no effect if newLocale is <CODE>null</CODE>. </param>
		/// <exception cref="PropertyVetoException"> if the new value is rejected </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setLocale(java.util.Locale newLocale) throws java.beans.PropertyVetoException
		public virtual Locale Locale
		{
			set
			{
				lock (this)
				{
            
					if ((Locale_Renamed != null && !Locale_Renamed.Equals(value)) && value != null)
					{
						Locale old = Locale_Renamed;
            
						FireVetoableChange("locale", old, value); // throws
            
						Locale_Renamed = value;
            
						FirePropertyChange("locale", old, value);
					}
				}
			}
			get
			{
				lock (this)
				{
					return Locale_Renamed;
				}
			}
		}


		/// <summary>
		/// <para>
		/// This method is typically called from the environment in order to determine
		/// if the implementor "needs" a GUI.
		/// </para>
		/// <para>
		/// The algorithm used herein tests the BeanContextPeer, and its current children
		/// to determine if they are either Containers, Components, or if they implement
		/// Visibility and return needsGui() == true.
		/// </para> </summary>
		/// <returns> <tt>true</tt> if the implementor needs a GUI </returns>
		public virtual bool NeedsGui()
		{
			lock (this)
			{
				BeanContext bc = BeanContextPeer;
        
				if (bc != this)
				{
					if (bc is Visibility)
					{
						return ((Visibility)bc).NeedsGui();
					}
        
					if (bc is Container || bc is Component)
					{
						return true;
					}
				}
        
				lock (Children)
				{
					for (IEnumerator i = Children.Keys.Iterator(); i.hasNext();)
					{
						Object c = i.next();
        
						try
						{
								return ((Visibility)c).NeedsGui();
						}
							catch (ClassCastException)
							{
								// do nothing ...
							}
        
							if (c is Container || c is Component)
							{
								return true;
							}
					}
				}
        
				return false;
			}
		}

		/// <summary>
		/// notify this instance that it may no longer render a GUI.
		/// </summary>

		public virtual void DontUseGui()
		{
			lock (this)
			{
				if (OkToUseGui_Renamed)
				{
					OkToUseGui_Renamed = false;
        
					// lets also tell the Children that can that they may not use their GUI's
					lock (Children)
					{
						for (IEnumerator i = Children.Keys.Iterator(); i.hasNext();)
						{
							Visibility v = GetChildVisibility(i.next());
        
							if (v != null)
							{
								v.DontUseGui();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Notify this instance that it may now render a GUI
		/// </summary>

		public virtual void OkToUseGui()
		{
			lock (this)
			{
				if (!OkToUseGui_Renamed)
				{
					OkToUseGui_Renamed = true;
        
					// lets also tell the Children that can that they may use their GUI's
					lock (Children)
					{
						for (IEnumerator i = Children.Keys.Iterator(); i.hasNext();)
						{
							Visibility v = GetChildVisibility(i.next());
        
							if (v != null)
							{
								v.OkToUseGui();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Used to determine if the <tt>BeanContext</tt>
		/// child is avoiding using its GUI. </summary>
		/// <returns> is this instance avoiding using its GUI? </returns>
		/// <seealso cref= Visibility </seealso>
		public virtual bool AvoidingGui()
		{
			return !OkToUseGui_Renamed && NeedsGui();
		}

		/// <summary>
		/// Is this <tt>BeanContext</tt> in the
		/// process of being serialized? </summary>
		/// <returns> if this <tt>BeanContext</tt> is
		/// currently being serialized </returns>
		public virtual bool Serializing
		{
			get
			{
				return Serializing_Renamed;
			}
		}

		/// <summary>
		/// Returns an iterator of all children
		/// of this <tt>BeanContext</tt>. </summary>
		/// <returns> an iterator for all the current BCSChild values </returns>
		protected internal virtual IEnumerator BcsChildren()
		{
			lock (Children)
			{
				return Children.values().Iterator();
			}
		}

		/// <summary>
		/// called by writeObject after defaultWriteObject() but prior to
		/// serialization of currently serializable children.
		/// 
		/// This method may be overridden by subclasses to perform custom
		/// serialization of their state prior to this superclass serializing
		/// the children.
		/// 
		/// This method should not however be used by subclasses to replace their
		/// own implementation (if any) of writeObject(). </summary>
		/// <param name="oos"> the {@code ObjectOutputStream} to use during serialization </param>
		/// <exception cref="IOException"> if serialization failed </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void bcsPreSerializationHook(java.io.ObjectOutputStream oos) throws java.io.IOException
		protected internal virtual void BcsPreSerializationHook(ObjectOutputStream oos)
		{
		}

		/// <summary>
		/// called by readObject after defaultReadObject() but prior to
		/// deserialization of any children.
		/// 
		/// This method may be overridden by subclasses to perform custom
		/// deserialization of their state prior to this superclass deserializing
		/// the children.
		/// 
		/// This method should not however be used by subclasses to replace their
		/// own implementation (if any) of readObject(). </summary>
		/// <param name="ois"> the {@code ObjectInputStream} to use during deserialization </param>
		/// <exception cref="IOException"> if deserialization failed </exception>
		/// <exception cref="ClassNotFoundException"> if needed classes are not found </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void bcsPreDeserializationHook(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		protected internal virtual void BcsPreDeserializationHook(ObjectInputStream ois)
		{
		}

		/// <summary>
		/// Called by readObject with the newly deserialized child and BCSChild. </summary>
		/// <param name="child"> the newly deserialized child </param>
		/// <param name="bcsc"> the newly deserialized BCSChild </param>
		protected internal virtual void ChildDeserializedHook(Object child, BCSChild bcsc)
		{
			lock (Children)
			{
				Children.put(child, bcsc);
			}
		}

		/// <summary>
		/// Used by writeObject to serialize a Collection. </summary>
		/// <param name="oos"> the <tt>ObjectOutputStream</tt>
		/// to use during serialization </param>
		/// <param name="coll"> the <tt>Collection</tt> to serialize </param>
		/// <exception cref="IOException"> if serialization failed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void serialize(java.io.ObjectOutputStream oos, java.util.Collection coll) throws java.io.IOException
		protected internal void Serialize(ObjectOutputStream oos, ICollection coll)
		{
			int count = 0;
			Object[] objects = coll.ToArray();

			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] is Serializable)
				{
					count++;
				}
				else
				{
					objects[i] = null;
				}
			}

			oos.WriteInt(count); // number of subsequent objects

			for (int i = 0; count > 0; i++)
			{
				Object o = objects[i];

				if (o != null)
				{
					oos.WriteObject(o);
					count--;
				}
			}
		}

		/// <summary>
		/// used by readObject to deserialize a collection. </summary>
		/// <param name="ois"> the ObjectInputStream to use </param>
		/// <param name="coll"> the Collection </param>
		/// <exception cref="IOException"> if deserialization failed </exception>
		/// <exception cref="ClassNotFoundException"> if needed classes are not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void deserialize(java.io.ObjectInputStream ois, java.util.Collection coll) throws java.io.IOException, ClassNotFoundException
		protected internal void Deserialize(ObjectInputStream ois, ICollection coll)
		{
			int count = 0;

			count = ois.ReadInt();

			while (count-- > 0)
			{
				coll.Add(ois.ReadObject());
			}
		}

		/// <summary>
		/// Used to serialize all children of
		/// this <tt>BeanContext</tt>. </summary>
		/// <param name="oos"> the <tt>ObjectOutputStream</tt>
		/// to use during serialization </param>
		/// <exception cref="IOException"> if serialization failed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeChildren(java.io.ObjectOutputStream oos) throws java.io.IOException
		public void WriteChildren(ObjectOutputStream oos)
		{
			if (Serializable <= 0)
			{
				return;
			}

			bool prev = Serializing_Renamed;

			Serializing_Renamed = true;

			int count = 0;

			lock (Children)
			{
				IEnumerator i = Children.entrySet().Iterator();

				while (i.hasNext() && count < Serializable)
				{
					java.util.Map_Entry entry = (java.util.Map_Entry)i.next();

					if (entry.Key is Serializable)
					{
						try
						{
							oos.WriteObject(entry.Key); // child
							oos.WriteObject(entry.Value); // BCSChild
						}
						catch (IOException ioe)
						{
							Serializing_Renamed = prev;
							throw ioe;
						}
						count++;
					}
				}
			}

			Serializing_Renamed = prev;

			if (count != Serializable)
			{
				throw new IOException("wrote different number of children than expected");
			}

		}

		/// <summary>
		/// Serialize the BeanContextSupport, if this instance has a distinct
		/// peer (that is this object is acting as a delegate for another) then
		/// the children of this instance are not serialized here due to a
		/// 'chicken and egg' problem that occurs on deserialization of the
		/// children at the same time as this instance.
		/// 
		/// Therefore in situations where there is a distinct peer to this instance
		/// it should always call writeObject() followed by writeChildren() and
		/// readObject() followed by readChildren().
		/// </summary>
		/// <param name="oos"> the ObjectOutputStream </param>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException, ClassNotFoundException
		private void WriteObject(ObjectOutputStream oos)
		{
			lock (this)
			{
				Serializing_Renamed = true;
        
				lock (BeanContext_Fields.GlobalHierarchyLock)
				{
					try
					{
						oos.DefaultWriteObject(); // serialize the BeanContextSupport object
        
						BcsPreSerializationHook(oos);
        
						if (Serializable > 0 && this.Equals(BeanContextPeer))
						{
							WriteChildren(oos);
						}
        
						Serialize(oos, (ICollection)BcmListeners);
					}
					finally
					{
						Serializing_Renamed = false;
					}
				}
			}
		}

		/// <summary>
		/// When an instance of this class is used as a delegate for the
		/// implementation of the BeanContext protocols (and its subprotocols)
		/// there exists a 'chicken and egg' problem during deserialization </summary>
		/// <param name="ois"> the ObjectInputStream to use </param>
		/// <exception cref="IOException"> if deserialization failed </exception>
		/// <exception cref="ClassNotFoundException"> if needed classes are not found </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void readChildren(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		public void ReadChildren(ObjectInputStream ois)
		{
			int count = Serializable;

			while (count-- > 0)
			{
				Object child = null;
				BeanContextSupport.BCSChild bscc = null;

				try
				{
					child = ois.ReadObject();
					bscc = (BeanContextSupport.BCSChild)ois.ReadObject();
				}
				catch (IOException)
				{
					continue;
				}
				catch (ClassNotFoundException)
				{
					continue;
				}


				lock (child)
				{
					BeanContextChild bcc = null;

					try
					{
						bcc = (BeanContextChild)child;
					}
					catch (ClassCastException)
					{
						// do nothing;
					}

					if (bcc != null)
					{
						try
						{
							bcc.BeanContext = BeanContextPeer;

						   bcc.AddPropertyChangeListener("beanContext", ChildPCL);
						   bcc.AddVetoableChangeListener("beanContext", ChildVCL);

						}
						catch (PropertyVetoException)
						{
							continue;
						}
					}

					ChildDeserializedHook(child, bscc);
				}
			}
		}

		/// <summary>
		/// deserialize contents ... if this instance has a distinct peer the
		/// children are *not* serialized here, the peer's readObject() must call
		/// readChildren() after deserializing this instance.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream ois)
		{
			lock (this)
			{
        
				lock (BeanContext_Fields.GlobalHierarchyLock)
				{
					ois.DefaultReadObject();
        
					Initialize();
        
					BcsPreDeserializationHook(ois);
        
					if (Serializable > 0 && this.Equals(BeanContextPeer))
					{
						ReadChildren(ois);
					}
        
					Deserialize(ois, BcmListeners = new ArrayList(1));
				}
			}
		}

		/// <summary>
		/// subclasses may envelope to monitor veto child property changes.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void vetoableChange(java.beans.PropertyChangeEvent pce) throws java.beans.PropertyVetoException
		public virtual void VetoableChange(PropertyChangeEvent pce)
		{
			String propertyName = pce.PropertyName;
			Object source = pce.Source;

			lock (Children)
			{
				if ("beanContext".Equals(propertyName) && ContainsKey(source) && !BeanContextPeer.Equals(pce.NewValue))
				{
					if (!ValidatePendingRemove(source))
					{
						throw new PropertyVetoException("current BeanContext vetoes setBeanContext()", pce);
					}
					else
					{
						((BCSChild)Children.get(source)).RemovePending = true;
					}
				}
			}
		}

		/// <summary>
		/// subclasses may envelope to monitor child property changes.
		/// </summary>

		public virtual void PropertyChange(PropertyChangeEvent pce)
		{
			String propertyName = pce.PropertyName;
			Object source = pce.Source;

			lock (Children)
			{
				if ("beanContext".Equals(propertyName) && ContainsKey(source) && ((BCSChild)Children.get(source)).RemovePending)
				{
					BeanContext bc = BeanContextPeer;

					if (bc.Equals(pce.OldValue) && !bc.Equals(pce.NewValue))
					{
						Remove(source, false);
					}
					else
					{
						((BCSChild)Children.get(source)).RemovePending = false;
					}
				}
			}
		}

		/// <summary>
		/// <para>
		/// Subclasses of this class may override, or envelope, this method to
		/// add validation behavior for the BeanContext to examine child objects
		/// immediately prior to their being added to the BeanContext.
		/// </para>
		/// </summary>
		/// <param name="targetChild"> the child to create the Child on behalf of </param>
		/// <returns> true iff the child may be added to this BeanContext, otherwise false. </returns>

		protected internal virtual bool ValidatePendingAdd(Object targetChild)
		{
			return true;
		}

		/// <summary>
		/// <para>
		/// Subclasses of this class may override, or envelope, this method to
		/// add validation behavior for the BeanContext to examine child objects
		/// immediately prior to their being removed from the BeanContext.
		/// </para>
		/// </summary>
		/// <param name="targetChild"> the child to create the Child on behalf of </param>
		/// <returns> true iff the child may be removed from this BeanContext, otherwise false. </returns>

		protected internal virtual bool ValidatePendingRemove(Object targetChild)
		{
			return true;
		}

		/// <summary>
		/// subclasses may override this method to simply extend add() semantics
		/// after the child has been added and before the event notification has
		/// occurred. The method is called with the child synchronized. </summary>
		/// <param name="child"> the child </param>
		/// <param name="bcsc"> the BCSChild </param>

		protected internal virtual void ChildJustAddedHook(Object child, BCSChild bcsc)
		{
		}

		/// <summary>
		/// subclasses may override this method to simply extend remove() semantics
		/// after the child has been removed and before the event notification has
		/// occurred. The method is called with the child synchronized. </summary>
		/// <param name="child"> the child </param>
		/// <param name="bcsc"> the BCSChild </param>

		protected internal virtual void ChildJustRemovedHook(Object child, BCSChild bcsc)
		{
		}

		/// <summary>
		/// Gets the Component (if any) associated with the specified child. </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the Component (if any) associated with the specified child. </returns>
		protected internal static Visibility GetChildVisibility(Object child)
		{
			try
			{
				return (Visibility)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the Serializable (if any) associated with the specified Child </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the Serializable (if any) associated with the specified Child </returns>
		protected internal static Serializable GetChildSerializable(Object child)
		{
			try
			{
				return (Serializable)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the PropertyChangeListener
		/// (if any) of the specified child </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the PropertyChangeListener (if any) of the specified child </returns>
		protected internal static PropertyChangeListener GetChildPropertyChangeListener(Object child)
		{
			try
			{
				return (PropertyChangeListener)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the VetoableChangeListener
		/// (if any) of the specified child </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the VetoableChangeListener (if any) of the specified child </returns>
		protected internal static VetoableChangeListener GetChildVetoableChangeListener(Object child)
		{
			try
			{
				return (VetoableChangeListener)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the BeanContextMembershipListener
		/// (if any) of the specified child </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the BeanContextMembershipListener (if any) of the specified child </returns>
		protected internal static BeanContextMembershipListener GetChildBeanContextMembershipListener(Object child)
		{
			try
			{
				return (BeanContextMembershipListener)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the BeanContextChild (if any) of the specified child </summary>
		/// <param name="child"> the specified child </param>
		/// <returns>  the BeanContextChild (if any) of the specified child </returns>
		/// <exception cref="IllegalArgumentException"> if child implements both BeanContextChild and BeanContextProxy </exception>
		protected internal static BeanContextChild GetChildBeanContextChild(Object child)
		{
			try
			{
				BeanContextChild bcc = (BeanContextChild)child;

				if (child is BeanContextChild && child is BeanContextProxy)
				{
					throw new IllegalArgumentException("child cannot implement both BeanContextChild and BeanContextProxy");
				}
				else
				{
					return bcc;
				}
			}
			catch (ClassCastException)
			{
				try
				{
					return ((BeanContextProxy)child).BeanContextProxy;
				}
				catch (ClassCastException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Fire a BeanContextshipEvent on the BeanContextMembershipListener interface </summary>
		/// <param name="bcme"> the event to fire </param>

		protected internal void FireChildrenAdded(BeanContextMembershipEvent bcme)
		{
			Object[] copy;

			lock (BcmListeners)
			{
				copy = BcmListeners.toArray();
			}

			for (int i = 0; i < copy.Length; i++)
			{
				((BeanContextMembershipListener)copy[i]).ChildrenAdded(bcme);
			}
		}

		/// <summary>
		/// Fire a BeanContextshipEvent on the BeanContextMembershipListener interface </summary>
		/// <param name="bcme"> the event to fire </param>

		protected internal void FireChildrenRemoved(BeanContextMembershipEvent bcme)
		{
			Object[] copy;

			lock (BcmListeners)
			{
				copy = BcmListeners.toArray();
			}

			for (int i = 0; i < copy.Length; i++)
			{
				((BeanContextMembershipListener)copy[i]).ChildrenRemoved(bcme);
			}
		}

		/// <summary>
		/// protected method called from constructor and readObject to initialize
		/// transient state of BeanContextSupport instance.
		/// 
		/// This class uses this method to instantiate inner class listeners used
		/// to monitor PropertyChange and VetoableChange events on children.
		/// 
		/// subclasses may envelope this method to add their own initialization
		/// behavior
		/// </summary>

		protected internal virtual void Initialize()
		{
			lock (this)
			{
				Children = new Hashtable(Serializable + 1);
				BcmListeners = new ArrayList(1);
        
				ChildPCL = new PropertyChangeListenerAnonymousInnerClassHelper(this);
        
				ChildVCL = new VetoableChangeListenerAnonymousInnerClassHelper(this);
			}
		}

		private class PropertyChangeListenerAnonymousInnerClassHelper : PropertyChangeListener
		{
			private readonly BeanContextSupport OuterInstance;

			public PropertyChangeListenerAnonymousInnerClassHelper(BeanContextSupport outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


						/*
						 * this adaptor is used by the BeanContextSupport class to forward
						 * property changes from a child to the BeanContext, avoiding
						 * accidential serialization of the BeanContext by a badly
						 * behaved Serializable child.
						 */

			public virtual void PropertyChange(PropertyChangeEvent pce)
			{
				OuterInstance.PropertyChange(pce);
			}
		}

		private class VetoableChangeListenerAnonymousInnerClassHelper : VetoableChangeListener
		{
			private readonly BeanContextSupport OuterInstance;

			public VetoableChangeListenerAnonymousInnerClassHelper(BeanContextSupport outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


						/*
						 * this adaptor is used by the BeanContextSupport class to forward
						 * vetoable changes from a child to the BeanContext, avoiding
						 * accidential serialization of the BeanContext by a badly
						 * behaved Serializable child.
						 */

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void vetoableChange(java.beans.PropertyChangeEvent pce) throws java.beans.PropertyVetoException
			public virtual void VetoableChange(PropertyChangeEvent pce)
			{
				OuterInstance.VetoableChange(pce);
			}
		}

		/// <summary>
		/// Gets a copy of the this BeanContext's children. </summary>
		/// <returns> a copy of the current nested children </returns>
		protected internal Object[] CopyChildren()
		{
			lock (Children)
			{
				return Children.Keys.ToArray();
			}
		}

		/// <summary>
		/// Tests to see if two class objects,
		/// or their names are equal. </summary>
		/// <param name="first"> the first object </param>
		/// <param name="second"> the second object </param>
		/// <returns> true if equal, false if not </returns>
		protected internal static bool ClassEquals(Class first, Class second)
		{
			return first.Equals(second) || first.Name.Equals(second.Name);
		}


		/*
		 * fields
		 */


		/// <summary>
		/// all accesses to the <code> protected HashMap children </code> field
		/// shall be synchronized on that object.
		/// </summary>
		[NonSerialized]
		protected internal Hashtable Children;

		private int Serializable = 0; // children serializable

		/// <summary>
		/// all accesses to the <code> protected ArrayList bcmListeners </code> field
		/// shall be synchronized on that object.
		/// </summary>
		[NonSerialized]
		protected internal ArrayList BcmListeners;

		//

		/// <summary>
		/// The current locale of this BeanContext.
		/// </summary>
		protected internal Locale Locale_Renamed;

		/// <summary>
		/// A <tt>boolean</tt> indicating if this
		/// instance may now render a GUI.
		/// </summary>
		protected internal bool OkToUseGui_Renamed;


		/// <summary>
		/// A <tt>boolean</tt> indicating whether or not
		/// this object is currently in design time mode.
		/// </summary>
		protected internal bool DesignTime_Renamed;

		/*
		 * transient
		 */

		[NonSerialized]
		private PropertyChangeListener ChildPCL;

		[NonSerialized]
		private VetoableChangeListener ChildVCL;

		[NonSerialized]
		private bool Serializing_Renamed;
	}

}