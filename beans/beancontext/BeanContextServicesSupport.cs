using System;
using System.Collections;

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
	/// This helper class provides a utility implementation of the
	/// java.beans.beancontext.BeanContextServices interface.
	/// </para>
	/// <para>
	/// Since this class directly implements the BeanContextServices interface,
	/// the class can, and is intended to be used either by subclassing this
	/// implementation, or via delegation of an instance of this class
	/// from another through the BeanContextProxy interface.
	/// </para>
	/// 
	/// @author Laurence P. G. Cable
	/// @since 1.2
	/// </summary>

	public class BeanContextServicesSupport : BeanContextSupport, BeanContextServices
	{
		private new const long SerialVersionUID = -8494482757288719206L;

		/// <summary>
		/// <para>
		/// Construct a BeanContextServicesSupport instance
		/// </para>
		/// </summary>
		/// <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		/// <param name="lcle">      The current Locale for this BeanContext. </param>
		/// <param name="dTime">     The initial state, true if in design mode, false if runtime. </param>
		/// <param name="visible">   The initial visibility.
		///  </param>

		public BeanContextServicesSupport(BeanContextServices peer, Locale lcle, bool dTime, bool visible) : base(peer, lcle, dTime, visible)
		{
		}

		/// <summary>
		/// Create an instance using the specified Locale and design mode.
		/// </summary>
		/// <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		/// <param name="lcle">      The current Locale for this BeanContext. </param>
		/// <param name="dtime">     The initial state, true if in design mode, false if runtime. </param>

		public BeanContextServicesSupport(BeanContextServices peer, Locale lcle, bool dtime) : this(peer, lcle, dtime, true)
		{
		}

		/// <summary>
		/// Create an instance using the specified locale
		/// </summary>
		/// <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		/// <param name="lcle">      The current Locale for this BeanContext. </param>

		public BeanContextServicesSupport(BeanContextServices peer, Locale lcle) : this(peer, lcle, false, true)
		{
		}

		/// <summary>
		/// Create an instance with a peer
		/// </summary>
		/// <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>

		public BeanContextServicesSupport(BeanContextServices peer) : this(peer, null, false, true)
		{
		}

		/// <summary>
		/// Create an instance that is not a delegate of another object
		/// </summary>

		public BeanContextServicesSupport() : this(null, null, false, true)
		{
		}

		/// <summary>
		/// called by BeanContextSupport superclass during construction and
		/// deserialization to initialize subclass transient state.
		/// 
		/// subclasses may envelope this method, but should not override it or
		/// call it directly.
		/// </summary>

		public override void Initialize()
		{
			base.Initialize();

			Services = new Hashtable(Serializable + 1);
			BcsListeners = new ArrayList(1);
		}

		/// <summary>
		/// Gets the <tt>BeanContextServices</tt> associated with this
		/// <tt>BeanContextServicesSupport</tt>.
		/// </summary>
		/// <returns> the instance of <tt>BeanContext</tt>
		/// this object is providing the implementation for. </returns>
		public virtual BeanContextServices BeanContextServicesPeer
		{
			get
			{
				return (BeanContextServices)BeanContextChildPeer;
			}
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

		protected internal class BCSSChild : BeanContextSupport.BCSChild
		{
			private readonly BeanContextServicesSupport OuterInstance;


			internal const long SerialVersionUID = -3263851306889194873L;

			/*
			 * private nested class to map serviceClass to Provider and requestors
			 * listeners.
			 */

			internal class BCSSCServiceClassRef
			{
				private readonly BeanContextServicesSupport.BCSSChild OuterInstance;


				// create an instance of a service ref

				internal BCSSCServiceClassRef(BeanContextServicesSupport.BCSSChild outerInstance, Class sc, BeanContextServiceProvider bcsp, bool delegated) : base()
				{
					this.OuterInstance = outerInstance;

					ServiceClass_Renamed = sc;

					if (delegated)
					{
						DelegateProvider_Renamed = bcsp;
					}
					else
					{
						ServiceProvider_Renamed = bcsp;
					}
				}

				// add a requestor and assoc listener

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addRequestor(Object requestor, BeanContextServiceRevokedListener bcsrl) throws java.util.TooManyListenersException
				internal virtual void AddRequestor(Object requestor, BeanContextServiceRevokedListener bcsrl)
				{
					BeanContextServiceRevokedListener cbcsrl = (BeanContextServiceRevokedListener)Requestors.get(requestor);

					if (cbcsrl != null && !cbcsrl.Equals(bcsrl))
					{
						throw new TooManyListenersException();
					}

					Requestors.put(requestor, bcsrl);
				}

				// remove a requestor

				internal virtual void RemoveRequestor(Object requestor)
				{
					Requestors.remove(requestor);
				}

				// check a requestors listener

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void verifyRequestor(Object requestor, BeanContextServiceRevokedListener bcsrl) throws java.util.TooManyListenersException
				internal virtual void VerifyRequestor(Object requestor, BeanContextServiceRevokedListener bcsrl)
				{
					BeanContextServiceRevokedListener cbcsrl = (BeanContextServiceRevokedListener)Requestors.get(requestor);

					if (cbcsrl != null && !cbcsrl.Equals(bcsrl))
					{
						throw new TooManyListenersException();
					}
				}

				internal virtual void VerifyAndMaybeSetProvider(BeanContextServiceProvider bcsp, bool isDelegated)
				{
					BeanContextServiceProvider current;

					if (isDelegated) // the provider is delegated
					{
						current = DelegateProvider_Renamed;

						if (current == null || bcsp == null)
						{
							DelegateProvider_Renamed = bcsp;
							return;
						}
					} // the provider is registered with this BCS
					else
					{
						current = ServiceProvider_Renamed;

						if (current == null || bcsp == null)
						{
							ServiceProvider_Renamed = bcsp;
							return;
						}
					}

					if (!current.Equals(bcsp))
					{
						throw new UnsupportedOperationException("existing service reference obtained from different BeanContextServiceProvider not supported");
					}

				}

				internal virtual IEnumerator CloneOfEntries()
				{
					return ((Hashtable)Requestors.clone()).entrySet().Iterator();
				}

				internal virtual IEnumerator Entries()
				{
					return Requestors.entrySet().Iterator();
				}

				internal virtual bool Empty
				{
					get
					{
						return Requestors.Empty;
					}
				}

				internal virtual Class ServiceClass
				{
					get
					{
						return ServiceClass_Renamed;
					}
				}

				internal virtual BeanContextServiceProvider ServiceProvider
				{
					get
					{
						return ServiceProvider_Renamed;
					}
				}

				internal virtual BeanContextServiceProvider DelegateProvider
				{
					get
					{
						return DelegateProvider_Renamed;
					}
				}

				internal virtual bool Delegated
				{
					get
					{
						return DelegateProvider_Renamed != null;
					}
				}

				internal virtual void AddRef(bool delegated)
				{
					if (delegated)
					{
						DelegateRefs_Renamed++;
					}
					else
					{
						ServiceRefs_Renamed++;
					}
				}


				internal virtual void ReleaseRef(bool delegated)
				{
					if (delegated)
					{
						if (--DelegateRefs_Renamed == 0)
						{
							DelegateProvider_Renamed = null;
						}
					}
					else
					{
						if (--ServiceRefs_Renamed <= 0)
						{
							ServiceProvider_Renamed = null;
						}
					}
				}

				internal virtual int Refs
				{
					get
					{
						return ServiceRefs_Renamed + DelegateRefs_Renamed;
					}
				}

				internal virtual int DelegateRefs
				{
					get
					{
						return DelegateRefs_Renamed;
					}
				}

				internal virtual int ServiceRefs
				{
					get
					{
						return ServiceRefs_Renamed;
					}
				}

				/*
				 * fields
				 */

				internal Class ServiceClass_Renamed;

				internal BeanContextServiceProvider ServiceProvider_Renamed;
				internal int ServiceRefs_Renamed;

				internal BeanContextServiceProvider DelegateProvider_Renamed; // proxy
				internal int DelegateRefs_Renamed;

				internal Hashtable Requestors = new Hashtable(1);
			}

			/*
			 * per service reference info ...
			 */

			internal class BCSSCServiceRef
			{
				private readonly BeanContextServicesSupport.BCSSChild OuterInstance;

				internal BCSSCServiceRef(BeanContextServicesSupport.BCSSChild outerInstance, BCSSCServiceClassRef scref, bool isDelegated)
				{
					this.OuterInstance = outerInstance;
					ServiceClassRef_Renamed = scref;
					Delegated_Renamed = isDelegated;
				}

				internal virtual void AddRef()
				{
					RefCnt++;
				}
				internal virtual int Release()
				{
					return --RefCnt;
				}

				internal virtual BCSSCServiceClassRef ServiceClassRef
				{
					get
					{
						return ServiceClassRef_Renamed;
					}
				}

				internal virtual bool Delegated
				{
					get
					{
						return Delegated_Renamed;
					}
				}

				/*
				 * fields
				 */

				internal BCSSCServiceClassRef ServiceClassRef_Renamed;
				internal int RefCnt = 1;
				internal bool Delegated_Renamed = false;
			}

			internal BCSSChild(BeanContextServicesSupport outerInstance, Object bcc, Object peer) : base(outerInstance, bcc, peer)
			{
				this.OuterInstance = outerInstance;
			}

			// note usage of service per requestor, per service

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void usingService(Object requestor, Object service, Class serviceClass, BeanContextServiceProvider bcsp, boolean isDelegated, BeanContextServiceRevokedListener bcsrl) throws java.util.TooManyListenersException, UnsupportedOperationException
			internal virtual void UsingService(Object requestor, Object service, Class serviceClass, BeanContextServiceProvider bcsp, bool isDelegated, BeanContextServiceRevokedListener bcsrl)
			{
				lock (this)
				{
        
					// first, process mapping from serviceClass to requestor(s)
        
					BCSSCServiceClassRef serviceClassRef = null;
        
					if (ServiceClasses == null)
					{
						ServiceClasses = new Hashtable(1);
					}
					else
					{
						serviceClassRef = (BCSSCServiceClassRef)ServiceClasses.get(serviceClass);
					}
        
					if (serviceClassRef == null) // new service being used ...
					{
						serviceClassRef = new BCSSCServiceClassRef(this, serviceClass, bcsp, isDelegated);
						ServiceClasses.put(serviceClass, serviceClassRef);
        
					} // existing service ...
					else
					{
						serviceClassRef.VerifyAndMaybeSetProvider(bcsp, isDelegated); // throws
						serviceClassRef.VerifyRequestor(requestor, bcsrl); // throws
					}
        
					serviceClassRef.AddRequestor(requestor, bcsrl);
					serviceClassRef.AddRef(isDelegated);
        
					// now handle mapping from requestor to service(s)
        
					BCSSCServiceRef serviceRef = null;
					IDictionary services = null;
        
					if (ServiceRequestors == null)
					{
						ServiceRequestors = new Hashtable(1);
					}
					else
					{
						services = (IDictionary)ServiceRequestors.get(requestor);
					}
        
					if (services == null)
					{
						services = new Hashtable(1);
        
						ServiceRequestors.put(requestor, services);
					}
					else
					{
						serviceRef = (BCSSCServiceRef)services[service];
					}
        
					if (serviceRef == null)
					{
						serviceRef = new BCSSCServiceRef(this, serviceClassRef, isDelegated);
        
						services[service] = serviceRef;
					}
					else
					{
						serviceRef.AddRef();
					}
				}
			}

			// release a service reference

			internal virtual void ReleaseService(Object requestor, Object service)
			{
				lock (this)
				{
					if (ServiceRequestors == null)
					{
						return;
					}
        
					IDictionary services = (IDictionary)ServiceRequestors.get(requestor);
        
					if (services == null) // oops its not there anymore!
					{
						return;
					}
        
					BCSSCServiceRef serviceRef = (BCSSCServiceRef)services[service];
        
					if (serviceRef == null) // oops its not there anymore!
					{
						return;
					}
        
					BCSSCServiceClassRef serviceClassRef = serviceRef.ServiceClassRef;
					bool isDelegated = serviceRef.Delegated;
					BeanContextServiceProvider bcsp = isDelegated ? serviceClassRef.DelegateProvider : serviceClassRef.ServiceProvider;
        
					bcsp.ReleaseService(OuterInstance.BeanContextServicesPeer, requestor, service);
        
					serviceClassRef.ReleaseRef(isDelegated);
					serviceClassRef.RemoveRequestor(requestor);
        
					if (serviceRef.Release() == 0)
					{
        
						services.Remove(service);
        
						if (services.Count == 0)
						{
							ServiceRequestors.remove(requestor);
							serviceClassRef.RemoveRequestor(requestor);
						}
        
						if (ServiceRequestors.Empty)
						{
							ServiceRequestors = null;
						}
        
						if (serviceClassRef.Empty)
						{
							ServiceClasses.remove(serviceClassRef.ServiceClass);
						}
        
						if (ServiceClasses.Empty)
						{
							ServiceClasses = null;
						}
					}
				}
			}

			// revoke a service

			internal virtual void RevokeService(Class serviceClass, bool isDelegated, bool revokeNow)
			{
				lock (this)
				{
					if (ServiceClasses == null)
					{
						return;
					}
        
					BCSSCServiceClassRef serviceClassRef = (BCSSCServiceClassRef)ServiceClasses.get(serviceClass);
        
					if (serviceClassRef == null)
					{
						return;
					}
        
					IEnumerator i = serviceClassRef.CloneOfEntries();
        
					BeanContextServiceRevokedEvent bcsre = new BeanContextServiceRevokedEvent(OuterInstance.BeanContextServicesPeer, serviceClass, revokeNow);
					bool noMoreRefs = false;
        
					while (i.hasNext() && ServiceRequestors != null)
					{
						Map_Entry entry = (Map_Entry)i.next();
						BeanContextServiceRevokedListener listener = (BeanContextServiceRevokedListener)entry.Value;
        
						if (revokeNow)
						{
							Object requestor = entry.Key;
							IDictionary services = (IDictionary)ServiceRequestors.get(requestor);
        
							if (services != null)
							{
								IEnumerator i1 = services.GetEnumerator();
        
								while (i1.hasNext())
								{
									Map_Entry tmp = (Map_Entry)i1.next();
        
									BCSSCServiceRef serviceRef = (BCSSCServiceRef)tmp.Value;
									if (serviceRef.ServiceClassRef.Equals(serviceClassRef) && isDelegated == serviceRef.Delegated)
									{
										i1.remove();
									}
								}
        
								if (noMoreRefs = services.Count == 0)
								{
									ServiceRequestors.remove(requestor);
								}
							}
        
							if (noMoreRefs)
							{
								serviceClassRef.RemoveRequestor(requestor);
							}
						}
        
						listener.ServiceRevoked(bcsre);
					}
        
					if (revokeNow && ServiceClasses != null)
					{
						if (serviceClassRef.Empty)
						{
							ServiceClasses.remove(serviceClass);
						}
        
						if (ServiceClasses.Empty)
						{
							ServiceClasses = null;
						}
					}
        
					if (ServiceRequestors != null && ServiceRequestors.Empty)
					{
						ServiceRequestors = null;
					}
				}
			}

			// release all references for this child since it has been unnested.

			internal virtual void CleanupReferences()
			{

				if (ServiceRequestors == null)
				{
					return;
				}

				IEnumerator requestors = ServiceRequestors.entrySet().Iterator();

				while (requestors.hasNext())
				{
					Map_Entry tmp = (Map_Entry)requestors.next();
					Object requestor = tmp.Key;
					IEnumerator services = ((IDictionary)tmp.Value).GetEnumerator();

					requestors.remove();

					while (services.hasNext())
					{
						Map_Entry entry = (Map_Entry)services.next();
						Object service = entry.Key;
						BCSSCServiceRef sref = (BCSSCServiceRef)entry.Value;

						BCSSCServiceClassRef scref = sref.ServiceClassRef;

						BeanContextServiceProvider bcsp = sref.Delegated ? scref.DelegateProvider : scref.ServiceProvider;

						scref.RemoveRequestor(requestor);
						services.remove();

						while (sref.Release() >= 0)
						{
							bcsp.ReleaseService(OuterInstance.BeanContextServicesPeer, requestor, service);
						}
					}
				}

				ServiceRequestors = null;
				ServiceClasses = null;
			}

			internal virtual void RevokeAllDelegatedServicesNow()
			{
				if (ServiceClasses == null)
				{
					return;
				}

				IEnumerator serviceClassRefs = (new HashSet(ServiceClasses.values())).GetEnumerator();

				while (serviceClassRefs.hasNext())
				{
					BCSSCServiceClassRef serviceClassRef = (BCSSCServiceClassRef)serviceClassRefs.next();

					if (!serviceClassRef.Delegated)
					{
						continue;
					}

					IEnumerator i = serviceClassRef.CloneOfEntries();
					BeanContextServiceRevokedEvent bcsre = new BeanContextServiceRevokedEvent(OuterInstance.BeanContextServicesPeer, serviceClassRef.ServiceClass, true);
					bool noMoreRefs = false;

					while (i.hasNext())
					{
						Map_Entry entry = (Map_Entry)i.next();
						BeanContextServiceRevokedListener listener = (BeanContextServiceRevokedListener)entry.Value;

						Object requestor = entry.Key;
						IDictionary services = (IDictionary)ServiceRequestors.get(requestor);

						if (services != null)
						{
							IEnumerator i1 = services.GetEnumerator();

							while (i1.hasNext())
							{
								Map_Entry tmp = (Map_Entry)i1.next();

								BCSSCServiceRef serviceRef = (BCSSCServiceRef)tmp.Value;
								if (serviceRef.ServiceClassRef.Equals(serviceClassRef) && serviceRef.Delegated)
								{
									i1.remove();
								}
							}

							if (noMoreRefs = services.Count == 0)
							{
								ServiceRequestors.remove(requestor);
							}
						}

						if (noMoreRefs)
						{
							serviceClassRef.RemoveRequestor(requestor);
						}

						listener.ServiceRevoked(bcsre);

						if (serviceClassRef.Empty)
						{
							ServiceClasses.remove(serviceClassRef.ServiceClass);
						}
					}
				}

				if (ServiceClasses.Empty)
				{
					ServiceClasses = null;
				}

				if (ServiceRequestors != null && ServiceRequestors.Empty)
				{
					ServiceRequestors = null;
				}
			}

			/*
			 * fields
			 */

			[NonSerialized]
			internal Hashtable ServiceClasses;
			[NonSerialized]
			internal Hashtable ServiceRequestors;
		}

		/// <summary>
		/// <para>
		/// Subclasses can override this method to insert their own subclass
		/// of Child without having to override add() or the other Collection
		/// methods that add children to the set.
		/// </para>
		/// </summary>
		/// <param name="targetChild"> the child to create the Child on behalf of </param>
		/// <param name="peer">        the peer if the targetChild and peer are related by BeanContextProxy </param>

		protected internal override BCSChild CreateBCSChild(Object targetChild, Object peer)
		{
			return new BCSSChild(this, targetChild, peer);
		}

		/// <summary>
		///********************************************************************* </summary>

			/// <summary>
			/// subclasses may subclass this nested class to add behaviors for
			/// each BeanContextServicesProvider.
			/// </summary>

			[Serializable]
			protected internal class BCSSServiceProvider
			{
				internal const long SerialVersionUID = 861278251667444782L;

				internal BCSSServiceProvider(Class sc, BeanContextServiceProvider bcsp) : base()
				{

					ServiceProvider_Renamed = bcsp;
				}

				/// <summary>
				/// Returns the service provider. </summary>
				/// <returns> the service provider </returns>
				protected internal virtual BeanContextServiceProvider ServiceProvider
				{
					get
					{
						return ServiceProvider_Renamed;
					}
				}

				/// <summary>
				/// The service provider.
				/// </summary>

				protected internal BeanContextServiceProvider ServiceProvider_Renamed;
			}

			/// <summary>
			/// subclasses can override this method to create new subclasses of
			/// BCSSServiceProvider without having to override addService() in
			/// order to instantiate. </summary>
			/// <param name="sc"> the class </param>
			/// <param name="bcsp"> the service provider </param>
			/// <returns> a service provider without overriding addService() </returns>

			protected internal virtual BCSSServiceProvider CreateBCSSServiceProvider(Class sc, BeanContextServiceProvider bcsp)
			{
				return new BCSSServiceProvider(sc, bcsp);
			}

		/// <summary>
		///********************************************************************* </summary>

		/// <summary>
		/// add a BeanContextServicesListener
		/// </summary>
		/// <exception cref="NullPointerException"> if the argument is null </exception>

		public virtual void AddBeanContextServicesListener(BeanContextServicesListener bcsl)
		{
			if (bcsl == null)
			{
				throw new NullPointerException("bcsl");
			}

			lock (BcsListeners)
			{
				if (BcsListeners.contains(bcsl))
				{
					return;
				}
				else
				{
					BcsListeners.add(bcsl);
				}
			}
		}

		/// <summary>
		/// remove a BeanContextServicesListener
		/// </summary>

		public virtual void RemoveBeanContextServicesListener(BeanContextServicesListener bcsl)
		{
			if (bcsl == null)
			{
				throw new NullPointerException("bcsl");
			}

			lock (BcsListeners)
			{
				if (!BcsListeners.contains(bcsl))
				{
					return;
				}
				else
				{
					BcsListeners.remove(bcsl);
				}
			}
		}

		/// <summary>
		/// add a service </summary>
		/// <param name="serviceClass"> the service class </param>
		/// <param name="bcsp"> the service provider </param>

		public virtual bool AddService(Class serviceClass, BeanContextServiceProvider bcsp)
		{
			return AddService(serviceClass, bcsp, true);
		}

		/// <summary>
		/// add a service </summary>
		/// <param name="serviceClass"> the service class </param>
		/// <param name="bcsp"> the service provider </param>
		/// <param name="fireEvent"> whether or not an event should be fired </param>
		/// <returns> true if the service was successfully added </returns>

		protected internal virtual bool AddService(Class serviceClass, BeanContextServiceProvider bcsp, bool fireEvent)
		{

			if (serviceClass == null)
			{
				throw new NullPointerException("serviceClass");
			}
			if (bcsp == null)
			{
				throw new NullPointerException("bcsp");
			}

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (Services.containsKey(serviceClass))
				{
					return false;
				}
				else
				{
					Services.put(serviceClass, CreateBCSSServiceProvider(serviceClass, bcsp));

					if (bcsp is Serializable)
					{
						Serializable++;
					}

					if (!fireEvent)
					{
						return true;
					}


					BeanContextServiceAvailableEvent bcssae = new BeanContextServiceAvailableEvent(BeanContextServicesPeer, serviceClass);

					FireServiceAdded(bcssae);

					lock (Children)
					{
						IEnumerator i = Children.Keys.Iterator();

						while (i.hasNext())
						{
							Object c = i.next();

							if (c is BeanContextServices)
							{
								((BeanContextServicesListener)c).ServiceAvailable(bcssae);
							}
						}
					}

					return true;
				}
			}
		}

		/// <summary>
		/// remove a service </summary>
		/// <param name="serviceClass"> the service class </param>
		/// <param name="bcsp"> the service provider </param>
		/// <param name="revokeCurrentServicesNow"> whether or not to revoke the service </param>

		public virtual void RevokeService(Class serviceClass, BeanContextServiceProvider bcsp, bool revokeCurrentServicesNow)
		{

			if (serviceClass == null)
			{
				throw new NullPointerException("serviceClass");
			}
			if (bcsp == null)
			{
				throw new NullPointerException("bcsp");
			}

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (!Services.containsKey(serviceClass))
				{
					return;
				}

				BCSSServiceProvider bcsssp = (BCSSServiceProvider)Services.get(serviceClass);

				if (!bcsssp.ServiceProvider.Equals(bcsp))
				{
					throw new IllegalArgumentException("service provider mismatch");
				}

				Services.remove(serviceClass);

				if (bcsp is Serializable)
				{
					Serializable--;
				}

				IEnumerator i = BcsChildren(); // get the BCSChild values.

				while (i.hasNext())
				{
					((BCSSChild)i.next()).RevokeService(serviceClass, false, revokeCurrentServicesNow);
				}

				FireServiceRevoked(serviceClass, revokeCurrentServicesNow);
			}
		}

		/// <summary>
		/// has a service, which may be delegated
		/// </summary>

		public virtual bool HasService(Class serviceClass)
		{
			lock (this)
			{
				if (serviceClass == null)
				{
					throw new NullPointerException("serviceClass");
				}
        
				lock (BeanContext_Fields.GlobalHierarchyLock)
				{
					if (Services.containsKey(serviceClass))
					{
						return true;
					}
        
					BeanContextServices bcs = null;
        
					try
					{
						bcs = (BeanContextServices)BeanContext;
					}
					catch (ClassCastException)
					{
						return false;
					}
        
					return bcs == null ? false : bcs.HasService(serviceClass);
				}
			}
		}

		/// <summary>
		///********************************************************************* </summary>

		/*
		 * a nested subclass used to represent a proxy for serviceClasses delegated
		 * to an enclosing BeanContext.
		 */

		protected internal class BCSSProxyServiceProvider : BeanContextServiceProvider, BeanContextServiceRevokedListener
		{
			private readonly BeanContextServicesSupport OuterInstance;


			internal BCSSProxyServiceProvider(BeanContextServicesSupport outerInstance, BeanContextServices bcs) : base()
			{
				this.OuterInstance = outerInstance;

				NestingCtxt = bcs;
			}

			public virtual Object GetService(BeanContextServices bcs, Object requestor, Class serviceClass, Object serviceSelector)
			{
				Object service = null;

				try
				{
					service = NestingCtxt.GetService(bcs, requestor, serviceClass, serviceSelector, this);
				}
				catch (TooManyListenersException)
				{
					return null;
				}

				return service;
			}

			public virtual void ReleaseService(BeanContextServices bcs, Object requestor, Object service)
			{
				NestingCtxt.ReleaseService(bcs, requestor, service);
			}

			public virtual IEnumerator GetCurrentServiceSelectors(BeanContextServices bcs, Class serviceClass)
			{
				return NestingCtxt.GetCurrentServiceSelectors(serviceClass);
			}

			public virtual void ServiceRevoked(BeanContextServiceRevokedEvent bcsre)
			{
				IEnumerator i = outerInstance.BcsChildren(); // get the BCSChild values.

				while (i.hasNext())
				{
					((BCSSChild)i.next()).RevokeService(bcsre.ServiceClass, true, bcsre.CurrentServiceInvalidNow);
				}
			}

			/*
			 * fields
			 */

			internal BeanContextServices NestingCtxt;
		}

		/// <summary>
		///********************************************************************* </summary>

		/// <summary>
		/// obtain a service which may be delegated
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getService(BeanContextChild child, Object requestor, Class serviceClass, Object serviceSelector, BeanContextServiceRevokedListener bcsrl) throws java.util.TooManyListenersException
		 public virtual Object GetService(BeanContextChild child, Object requestor, Class serviceClass, Object serviceSelector, BeanContextServiceRevokedListener bcsrl)
		 {
			if (child == null)
			{
				throw new NullPointerException("child");
			}
			if (serviceClass == null)
			{
				throw new NullPointerException("serviceClass");
			}
			if (requestor == null)
			{
				throw new NullPointerException("requestor");
			}
			if (bcsrl == null)
			{
				throw new NullPointerException("bcsrl");
			}

			Object service = null;
			BCSSChild bcsc;
			BeanContextServices bcssp = BeanContextServicesPeer;

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				lock (Children)
				{
					bcsc = (BCSSChild)Children.get(child);
				}

				if (bcsc == null) // not a child ...
				{
					throw new IllegalArgumentException("not a child of this context");
				}

				BCSSServiceProvider bcsssp = (BCSSServiceProvider)Services.get(serviceClass);

				if (bcsssp != null)
				{
					BeanContextServiceProvider bcsp = bcsssp.ServiceProvider;
					service = bcsp.GetService(bcssp, requestor, serviceClass, serviceSelector);
					if (service != null) // do bookkeeping ...
					{
						try
						{
							bcsc.UsingService(requestor, service, serviceClass, bcsp, false, bcsrl);
						}
						catch (TooManyListenersException tmle)
						{
							bcsp.ReleaseService(bcssp, requestor, service);
							throw tmle;
						}
						catch (UnsupportedOperationException uope)
						{
							bcsp.ReleaseService(bcssp, requestor, service);
							throw uope; // unchecked rt exception
						}

						return service;
					}
				}


				if (Proxy != null)
				{

					// try to delegate ...

					service = Proxy.GetService(bcssp, requestor, serviceClass, serviceSelector);

					if (service != null) // do bookkeeping ...
					{
						try
						{
							bcsc.UsingService(requestor, service, serviceClass, Proxy, true, bcsrl);
						}
						catch (TooManyListenersException tmle)
						{
							Proxy.ReleaseService(bcssp, requestor, service);
							throw tmle;
						}
						catch (UnsupportedOperationException uope)
						{
							Proxy.ReleaseService(bcssp, requestor, service);
							throw uope; // unchecked rt exception
						}

						return service;
					}
				}
			}

			return null;
		 }

		/// <summary>
		/// release a service
		/// </summary>

		public virtual void ReleaseService(BeanContextChild child, Object requestor, Object service)
		{
			if (child == null)
			{
				throw new NullPointerException("child");
			}
			if (requestor == null)
			{
				throw new NullPointerException("requestor");
			}
			if (service == null)
			{
				throw new NullPointerException("service");
			}

			BCSSChild bcsc;

			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
					lock (Children)
					{
						bcsc = (BCSSChild)Children.get(child);
					}

					if (bcsc != null)
					{
						bcsc.ReleaseService(requestor, service);
					}
					else
					{
					   throw new IllegalArgumentException("child actual is not a child of this BeanContext");
					}
			}
		}

		/// <returns> an iterator for all the currently registered service classes. </returns>

		public virtual IEnumerator CurrentServiceClasses
		{
			get
			{
				return new BCSIterator(Services.Keys.Iterator());
			}
		}

		/// <returns> an iterator for all the currently available service selectors
		/// (if any) available for the specified service. </returns>

		public virtual IEnumerator GetCurrentServiceSelectors(Class serviceClass)
		{

			BCSSServiceProvider bcsssp = (BCSSServiceProvider)Services.get(serviceClass);

			return bcsssp != null ? new BCSIterator(bcsssp.ServiceProvider.GetCurrentServiceSelectors(BeanContextServicesPeer, serviceClass)) : null;
		}

		/// <summary>
		/// BeanContextServicesListener callback, propagates event to all
		/// currently registered listeners and BeanContextServices children,
		/// if this BeanContextService does not already implement this service
		/// itself.
		/// 
		/// subclasses may override or envelope this method to implement their
		/// own propagation semantics.
		/// </summary>

		 public override void ServiceAvailable(BeanContextServiceAvailableEvent bcssae)
		 {
			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (Services.containsKey(bcssae.ServiceClass))
				{
					return;
				}

				FireServiceAdded(bcssae);

				IEnumerator i;

				lock (Children)
				{
					i = Children.Keys.Iterator();
				}

				while (i.hasNext())
				{
					Object c = i.next();

					if (c is BeanContextServices)
					{
						((BeanContextServicesListener)c).ServiceAvailable(bcssae);
					}
				}
			}
		 }

		/// <summary>
		/// BeanContextServicesListener callback, propagates event to all
		/// currently registered listeners and BeanContextServices children,
		/// if this BeanContextService does not already implement this service
		/// itself.
		/// 
		/// subclasses may override or envelope this method to implement their
		/// own propagation semantics.
		/// </summary>

		public override void ServiceRevoked(BeanContextServiceRevokedEvent bcssre)
		{
			lock (BeanContext_Fields.GlobalHierarchyLock)
			{
				if (Services.containsKey(bcssre.ServiceClass))
				{
					return;
				}

				FireServiceRevoked(bcssre);

				IEnumerator i;

				lock (Children)
				{
					i = Children.Keys.Iterator();
				}

				while (i.hasNext())
				{
					Object c = i.next();

					if (c is BeanContextServices)
					{
						((BeanContextServicesListener)c).ServiceRevoked(bcssre);
					}
				}
			}
		}

		/// <summary>
		/// Gets the <tt>BeanContextServicesListener</tt> (if any) of the specified
		/// child.
		/// </summary>
		/// <param name="child"> the specified child </param>
		/// <returns> the BeanContextServicesListener (if any) of the specified child </returns>
		protected internal static BeanContextServicesListener GetChildBeanContextServicesListener(Object child)
		{
			try
			{
				return (BeanContextServicesListener)child;
			}
			catch (ClassCastException)
			{
				return null;
			}
		}

		/// <summary>
		/// called from superclass child removal operations after a child
		/// has been successfully removed. called with child synchronized.
		/// 
		/// This subclass uses this hook to immediately revoke any services
		/// being used by this child if it is a BeanContextChild.
		/// 
		/// subclasses may envelope this method in order to implement their
		/// own child removal side-effects.
		/// </summary>

		protected internal override void ChildJustRemovedHook(Object child, BCSChild bcsc)
		{
			BCSSChild bcssc = (BCSSChild)bcsc;

			bcssc.CleanupReferences();
		}

		/// <summary>
		/// called from setBeanContext to notify a BeanContextChild
		/// to release resources obtained from the nesting BeanContext.
		/// 
		/// This method revokes any services obtained from its parent.
		/// 
		/// subclasses may envelope this method to implement their own semantics.
		/// </summary>

		protected internal override void ReleaseBeanContextResources()
		{
			lock (this)
			{
				Object[] bcssc;
        
				base.ReleaseBeanContextResources();
        
				lock (Children)
				{
					if (Children.Empty)
					{
						return;
					}
        
					bcssc = Children.values().ToArray();
				}
        
        
				for (int i = 0; i < bcssc.Length; i++)
				{
					((BCSSChild)bcssc[i]).RevokeAllDelegatedServicesNow();
				}
        
				Proxy = null;
			}
		}

		/// <summary>
		/// called from setBeanContext to notify a BeanContextChild
		/// to allocate resources obtained from the nesting BeanContext.
		/// 
		/// subclasses may envelope this method to implement their own semantics.
		/// </summary>

		protected internal override void InitializeBeanContextResources()
		{
			lock (this)
			{
				base.InitializeBeanContextResources();
        
				BeanContext nbc = BeanContext;
        
				if (nbc == null)
				{
					return;
				}
        
				try
				{
					BeanContextServices bcs = (BeanContextServices)nbc;
        
					Proxy = new BCSSProxyServiceProvider(this, bcs);
				}
				catch (ClassCastException)
				{
					// do nothing ...
				}
			}
		}

		/// <summary>
		/// Fires a <tt>BeanContextServiceEvent</tt> notifying of a new service. </summary>
		/// <param name="serviceClass"> the service class </param>
		protected internal void FireServiceAdded(Class serviceClass)
		{
			BeanContextServiceAvailableEvent bcssae = new BeanContextServiceAvailableEvent(BeanContextServicesPeer, serviceClass);

			FireServiceAdded(bcssae);
		}

		/// <summary>
		/// Fires a <tt>BeanContextServiceAvailableEvent</tt> indicating that a new
		/// service has become available.
		/// </summary>
		/// <param name="bcssae"> the <tt>BeanContextServiceAvailableEvent</tt> </param>
		protected internal void FireServiceAdded(BeanContextServiceAvailableEvent bcssae)
		{
			Object[] copy;

			lock (BcsListeners)
			{
				copy = BcsListeners.toArray();
			}

			for (int i = 0; i < copy.Length; i++)
			{
				((BeanContextServicesListener)copy[i]).ServiceAvailable(bcssae);
			}
		}

		/// <summary>
		/// Fires a <tt>BeanContextServiceEvent</tt> notifying of a service being revoked.
		/// </summary>
		/// <param name="bcsre"> the <tt>BeanContextServiceRevokedEvent</tt> </param>
		protected internal void FireServiceRevoked(BeanContextServiceRevokedEvent bcsre)
		{
			Object[] copy;

			lock (BcsListeners)
			{
				copy = BcsListeners.toArray();
			}

			for (int i = 0; i < copy.Length; i++)
			{
				((BeanContextServiceRevokedListener)copy[i]).ServiceRevoked(bcsre);
			}
		}

		/// <summary>
		/// Fires a <tt>BeanContextServiceRevokedEvent</tt>
		/// indicating that a particular service is
		/// no longer available. </summary>
		/// <param name="serviceClass"> the service class </param>
		/// <param name="revokeNow"> whether or not the event should be revoked now </param>
		protected internal void FireServiceRevoked(Class serviceClass, bool revokeNow)
		{
			Object[] copy;
			BeanContextServiceRevokedEvent bcsre = new BeanContextServiceRevokedEvent(BeanContextServicesPeer, serviceClass, revokeNow);

			lock (BcsListeners)
			{
				copy = BcsListeners.toArray();
			}

			for (int i = 0; i < copy.Length; i++)
			{
				((BeanContextServicesListener)copy[i]).ServiceRevoked(bcsre);
			}
		}

		/// <summary>
		/// called from BeanContextSupport writeObject before it serializes the
		/// children ...
		/// 
		/// This class will serialize any Serializable BeanContextServiceProviders
		/// herein.
		/// 
		/// subclasses may envelope this method to insert their own serialization
		/// processing that has to occur prior to serialization of the children
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bcsPreSerializationHook(java.io.ObjectOutputStream oos) throws java.io.IOException
		protected internal override void BcsPreSerializationHook(ObjectOutputStream oos)
		{
			lock (this)
			{
        
				oos.WriteInt(Serializable);
        
				if (Serializable <= 0)
				{
					return;
				}
        
				int count = 0;
        
				IEnumerator i = Services.entrySet().Iterator();
        
				while (i.hasNext() && count < Serializable)
				{
					Map_Entry entry = (Map_Entry)i.next();
					BCSSServiceProvider bcsp = null;
        
					 try
					 {
						bcsp = (BCSSServiceProvider)entry.Value;
					 }
					 catch (ClassCastException)
					 {
						continue;
					 }
        
					 if (bcsp.ServiceProvider is Serializable)
					 {
						oos.WriteObject(entry.Key);
						oos.WriteObject(bcsp);
						count++;
					 }
				}
        
				if (count != Serializable)
				{
					throw new IOException("wrote different number of service providers than expected");
				}
			}
		}

		/// <summary>
		/// called from BeanContextSupport readObject before it deserializes the
		/// children ...
		/// 
		/// This class will deserialize any Serializable BeanContextServiceProviders
		/// serialized earlier thus making them available to the children when they
		/// deserialized.
		/// 
		/// subclasses may envelope this method to insert their own serialization
		/// processing that has to occur prior to serialization of the children
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void bcsPreDeserializationHook(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		protected internal override void BcsPreDeserializationHook(ObjectInputStream ois)
		{
			lock (this)
			{
        
				Serializable = ois.ReadInt();
        
				int count = Serializable;
        
				while (count > 0)
				{
					Services.put(ois.ReadObject(), ois.ReadObject());
					count--;
				}
			}
		}

		/// <summary>
		/// serialize the instance
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(ObjectOutputStream oos)
		{
			lock (this)
			{
				oos.DefaultWriteObject();
        
				Serialize(oos, (ICollection)BcsListeners);
			}
		}

		/// <summary>
		/// deserialize the instance
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream ois)
		{
			lock (this)
			{
        
				ois.DefaultReadObject();
        
				Deserialize(ois, (ICollection)BcsListeners);
			}
		}


		/*
		 * fields
		 */

		/// <summary>
		/// all accesses to the <code> protected transient HashMap services </code>
		/// field should be synchronized on that object
		/// </summary>
		[NonSerialized]
		protected internal Hashtable Services;

		/// <summary>
		/// The number of instances of a serializable <tt>BeanContextServceProvider</tt>.
		/// </summary>
		[NonSerialized]
		protected internal int Serializable = 0;


		/// <summary>
		/// Delegate for the <tt>BeanContextServiceProvider</tt>.
		/// </summary>
		[NonSerialized]
		protected internal BCSSProxyServiceProvider Proxy;


		/// <summary>
		/// List of <tt>BeanContextServicesListener</tt> objects.
		/// </summary>
		[NonSerialized]
		protected internal ArrayList BcsListeners;
	}

}