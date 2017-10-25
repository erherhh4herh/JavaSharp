using System;

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

namespace java.rmi.activation
{

	using GetIntegerAction = sun.security.action.GetIntegerAction;

	/// <summary>
	/// An <code>ActivationGroup</code> is responsible for creating new
	/// instances of "activatable" objects in its group, informing its
	/// <code>ActivationMonitor</code> when either: its object's become
	/// active or inactive, or the group as a whole becomes inactive. <para>
	/// 
	/// An <code>ActivationGroup</code> is <i>initially</i> created in one
	/// of several ways: <ul>
	/// <li>as a side-effect of creating an <code>ActivationDesc</code>
	///     without an explicit <code>ActivationGroupID</code> for the
	///     first activatable object in the group, or
	/// <li>via the <code>ActivationGroup.createGroup</code> method
	/// <li>as a side-effect of activating the first object in a group
	/// </para>
	///     whose <code>ActivationGroupDesc</code> was only registered.</ul><para>
	/// 
	/// Only the activator can <i>recreate</i> an
	/// <code>ActivationGroup</code>.  The activator spawns, as needed, a
	/// separate VM (as a child process, for example) for each registered
	/// activation group and directs activation requests to the appropriate
	/// group. It is implementation specific how VMs are spawned. An
	/// activation group is created via the
	/// <code>ActivationGroup.createGroup</code> static method. The
	/// <code>createGroup</code> method has two requirements on the group
	/// to be created: 1) the group must be a concrete subclass of
	/// <code>ActivationGroup</code>, and 2) the group must have a
	/// constructor that takes two arguments:
	/// 
	/// <ul>
	/// <li> the group's <code>ActivationGroupID</code>, and
	/// <li> the group's initialization data (in a
	/// </para>
	///      <code>java.rmi.MarshalledObject</code>)</ul><para>
	/// 
	/// When created, the default implementation of
	/// <code>ActivationGroup</code> will override the system properties
	/// with the properties requested when its
	/// <code>ActivationGroupDesc</code> was created, and will set a
	/// <seealso cref="SecurityManager"/> as the default system
	/// security manager.  If your application requires specific properties
	/// to be set when objects are activated in the group, the application
	/// should create a special <code>Properties</code> object containing
	/// these properties, then create an <code>ActivationGroupDesc</code>
	/// with the <code>Properties</code> object, and use
	/// <code>ActivationGroup.createGroup</code> before creating any
	/// <code>ActivationDesc</code>s (before the default
	/// <code>ActivationGroupDesc</code> is created).  If your application
	/// requires the use of a security manager other than
	/// <seealso cref="SecurityManager"/>, in the
	/// ActivativationGroupDescriptor properties list you can set
	/// <code>java.security.manager</code> property to the name of the security
	/// manager you would like to install.
	/// 
	/// @author      Ann Wollrath
	/// </para>
	/// </summary>
	/// <seealso cref=         ActivationInstantiator </seealso>
	/// <seealso cref=         ActivationGroupDesc </seealso>
	/// <seealso cref=         ActivationGroupID
	/// @since       1.2 </seealso>
	public abstract class ActivationGroup : UnicastRemoteObject, ActivationInstantiator
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.rmi.MarshalledObject<JavaToDotNetGenericWildcard> newInstance(ActivationID id, ActivationDesc desc);
		public abstract MarshalledObject<?> NewInstance(ActivationID id, ActivationDesc desc);
		/// <summary>
		/// @serial the group's identifier
		/// </summary>
		private ActivationGroupID GroupID;

		/// <summary>
		/// @serial the group's monitor
		/// </summary>
		private ActivationMonitor Monitor_Renamed;

		/// <summary>
		/// @serial the group's incarnation number
		/// </summary>
		private long Incarnation;

		/// <summary>
		/// the current activation group for this VM </summary>
		private static ActivationGroup CurrGroup;
		/// <summary>
		/// the current group's identifier </summary>
		private static ActivationGroupID CurrGroupID;
		/// <summary>
		/// the current group's activation system </summary>
		private static ActivationSystem CurrSystem;
		/// <summary>
		/// used to control a group being created only once </summary>
		private static bool CanCreate = true;

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private const long SerialVersionUID = -7696947875314805420L;

		/// <summary>
		/// Constructs an activation group with the given activation group
		/// identifier.  The group is exported as a
		/// <code>java.rmi.server.UnicastRemoteObject</code>.
		/// </summary>
		/// <param name="groupID"> the group's identifier </param>
		/// <exception cref="RemoteException"> if this group could not be exported </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		///          not supported by this implementation
		/// @since   1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ActivationGroup(ActivationGroupID groupID) throws java.rmi.RemoteException
		protected internal ActivationGroup(ActivationGroupID groupID) : base()
		{
			// call super constructor to export the object
			this.GroupID = groupID;
		}

		/// <summary>
		/// The group's <code>inactiveObject</code> method is called
		/// indirectly via a call to the <code>Activatable.inactive</code>
		/// method. A remote object implementation must call
		/// <code>Activatable</code>'s <code>inactive</code> method when
		/// that object deactivates (the object deems that it is no longer
		/// active). If the object does not call
		/// <code>Activatable.inactive</code> when it deactivates, the
		/// object will never be garbage collected since the group keeps
		/// strong references to the objects it creates.
		/// 
		/// <para>The group's <code>inactiveObject</code> method unexports the
		/// remote object from the RMI runtime so that the object can no
		/// longer receive incoming RMI calls. An object will only be unexported
		/// if the object has no pending or executing calls.
		/// The subclass of <code>ActivationGroup</code> must override this
		/// method and unexport the object.
		/// 
		/// </para>
		/// <para>After removing the object from the RMI runtime, the group
		/// must inform its <code>ActivationMonitor</code> (via the monitor's
		/// <code>inactiveObject</code> method) that the remote object is
		/// not currently active so that the remote object will be
		/// re-activated by the activator upon a subsequent activation
		/// request.
		/// 
		/// </para>
		/// <para>This method simply informs the group's monitor that the object
		/// is inactive.  It is up to the concrete subclass of ActivationGroup
		/// </para>
		/// to fulfill the additional requirement of unexporting the object. <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the object's activation identifier </param>
		/// <returns> true if the object was successfully deactivated; otherwise
		///         returns false. </returns>
		/// <exception cref="UnknownObjectException"> if object is unknown (may already
		/// be inactive) </exception>
		/// <exception cref="RemoteException"> if call informing monitor fails </exception>
		/// <exception cref="ActivationException"> if group is inactive
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean inactiveObject(ActivationID id) throws ActivationException, java.rmi.activation.UnknownObjectException, java.rmi.RemoteException
		public virtual bool InactiveObject(ActivationID id)
		{
			Monitor.InactiveObject(id);
			return true;
		}

		/// <summary>
		/// The group's <code>activeObject</code> method is called when an
		/// object is exported (either by <code>Activatable</code> object
		/// construction or an explicit call to
		/// <code>Activatable.exportObject</code>. The group must inform its
		/// <code>ActivationMonitor</code> that the object is active (via
		/// the monitor's <code>activeObject</code> method) if the group
		/// hasn't already done so.
		/// </summary>
		/// <param name="id"> the object's identifier </param>
		/// <param name="obj"> the remote object implementation </param>
		/// <exception cref="UnknownObjectException"> if object is not registered </exception>
		/// <exception cref="RemoteException"> if call informing monitor fails </exception>
		/// <exception cref="ActivationException"> if group is inactive
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void activeObject(ActivationID id, java.rmi.Remote obj) throws ActivationException, java.rmi.activation.UnknownObjectException, java.rmi.RemoteException;
		public abstract void ActiveObject(ActivationID id, Remote obj);

		/// <summary>
		/// Create and set the activation group for the current VM.  The
		/// activation group can only be set if it is not currently set.
		/// An activation group is set using the <code>createGroup</code>
		/// method when the <code>Activator</code> initiates the
		/// re-creation of an activation group in order to carry out
		/// incoming <code>activate</code> requests. A group must first be
		/// registered with the <code>ActivationSystem</code> before it can
		/// be created via this method.
		/// 
		/// <para>The group class specified by the
		/// <code>ActivationGroupDesc</code> must be a concrete subclass of
		/// <code>ActivationGroup</code> and have a public constructor that
		/// takes two arguments: the <code>ActivationGroupID</code> for the
		/// group and the <code>MarshalledObject</code> containing the
		/// group's initialization data (obtained from the
		/// <code>ActivationGroupDesc</code>.
		/// 
		/// </para>
		/// <para>If the group class name specified in the
		/// <code>ActivationGroupDesc</code> is <code>null</code>, then
		/// this method will behave as if the group descriptor contained
		/// the name of the default activation group implementation class.
		/// 
		/// </para>
		/// <para>Note that if your application creates its own custom
		/// activation group, a security manager must be set for that
		/// group.  Otherwise objects cannot be activated in the group.
		/// <seealso cref="SecurityManager"/> is set by default.
		/// 
		/// </para>
		/// <para>If a security manager is already set in the group VM, this
		/// method first calls the security manager's
		/// <code>checkSetFactory</code> method.  This could result in a
		/// <code>SecurityException</code>. If your application needs to
		/// set a different security manager, you must ensure that the
		/// policy file specified by the group's
		/// <code>ActivationGroupDesc</code> grants the group the necessary
		/// permissions to set a new security manager.  (Note: This will be
		/// necessary if your group downloads and sets a security manager).
		/// 
		/// </para>
		/// <para>After the group is created, the
		/// <code>ActivationSystem</code> is informed that the group is
		/// active by calling the <code>activeGroup</code> method which
		/// returns the <code>ActivationMonitor</code> for the group. The
		/// application need not call <code>activeGroup</code>
		/// independently since it is taken care of by this method.
		/// 
		/// </para>
		/// <para>Once a group is created, subsequent calls to the
		/// <code>currentGroupID</code> method will return the identifier
		/// for this group until the group becomes inactive.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the activation group's identifier </param>
		/// <param name="desc"> the activation group's descriptor </param>
		/// <param name="incarnation"> the group's incarnation number (zero on group's
		/// initial creation) </param>
		/// <returns> the activation group for the VM </returns>
		/// <exception cref="ActivationException"> if group already exists or if error
		/// occurs during group creation </exception>
		/// <exception cref="SecurityException"> if permission to create group is denied.
		/// (Note: The default implementation of the security manager
		/// <code>checkSetFactory</code>
		/// method requires the RuntimePermission "setFactory") </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation </exception>
		/// <seealso cref= SecurityManager#checkSetFactory
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized ActivationGroup createGroup(ActivationGroupID id, final ActivationGroupDesc desc, long incarnation) throws ActivationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static ActivationGroup CreateGroup(ActivationGroupID id, ActivationGroupDesc desc, long incarnation)
		{
			lock (typeof(ActivationGroup))
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckSetFactory();
				}
        
				if (CurrGroup != null)
				{
					throw new ActivationException("group already exists");
				}
        
				if (CanCreate == false)
				{
					throw new ActivationException("group deactivated and " + "cannot be recreated");
				}
        
				try
				{
					// load group's class
					String groupClassName = desc.ClassName;
					Class cl;
					Class defaultGroupClass = typeof(sun.rmi.server.ActivationGroupImpl);
					if (groupClassName == null || groupClassName.Equals(defaultGroupClass.Name)) // see 4252236
					{
						cl = defaultGroupClass;
					}
					else
					{
						Class cl0;
						try
						{
							cl0 = RMIClassLoader.LoadClass(desc.Location, groupClassName);
						}
						catch (Exception ex)
						{
							throw new ActivationException("Could not load group implementation class", ex);
						}
						if (cl0.IsSubclassOf(typeof(ActivationGroup)))
						{
							cl = cl0.AsSubclass(typeof(ActivationGroup));
						}
						else
						{
							throw new ActivationException("group not correct class: " + cl0.Name);
						}
					}
        
					// create group
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<? extends ActivationGroup> constructor = cl.getConstructor(ActivationGroupID.class, java.rmi.MarshalledObject.class);
					Constructor<?> constructor = cl.getConstructor(typeof(ActivationGroupID), typeof(MarshalledObject));
					ActivationGroup newGroup = constructor.newInstance(id, desc.Data);
					CurrSystem = id.System;
					newGroup.Incarnation = incarnation;
					newGroup.Monitor_Renamed = CurrSystem.ActiveGroup(id, newGroup, incarnation);
					CurrGroup = newGroup;
					CurrGroupID = id;
					CanCreate = false;
				}
				catch (InvocationTargetException e)
				{
						e.TargetException.printStackTrace();
						throw new ActivationException("exception in group constructor", e.TargetException);
        
				}
				catch (ActivationException e)
				{
					throw e;
        
				}
				catch (Exception e)
				{
					throw new ActivationException("exception creating group", e);
				}
        
				return CurrGroup;
			}
		}

		/// <summary>
		/// Returns the current activation group's identifier.  Returns null
		/// if no group is currently active for this VM. </summary>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation </exception>
		/// <returns> the activation group's identifier
		/// @since 1.2 </returns>
		public static ActivationGroupID CurrentGroupID()
		{
			lock (typeof(ActivationGroup))
			{
				return CurrGroupID;
			}
		}

		/// <summary>
		/// Returns the activation group identifier for the VM.  If an
		/// activation group does not exist for this VM, a default
		/// activation group is created. A group can be created only once,
		/// so if a group has already become active and deactivated.
		/// </summary>
		/// <returns> the activation group identifier </returns>
		/// <exception cref="ActivationException"> if error occurs during group
		/// creation, if security manager is not set, or if the group
		/// has already been created and deactivated. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static synchronized ActivationGroupID internalCurrentGroupID() throws ActivationException
		internal static ActivationGroupID InternalCurrentGroupID()
		{
			lock (typeof(ActivationGroup))
			{
				if (CurrGroupID == null)
				{
					throw new ActivationException("nonexistent group");
				}
        
				return CurrGroupID;
			}
		}

		/// <summary>
		/// Set the activation system for the VM.  The activation system can
		/// only be set it if no group is currently active. If the activation
		/// system is not set via this call, then the <code>getSystem</code>
		/// method attempts to obtain a reference to the
		/// <code>ActivationSystem</code> by looking up the name
		/// "java.rmi.activation.ActivationSystem" in the Activator's
		/// registry. By default, the port number used to look up the
		/// activation system is defined by
		/// <code>ActivationSystem.SYSTEM_PORT</code>. This port can be overridden
		/// by setting the property <code>java.rmi.activation.port</code>.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's <code>checkSetFactory</code> method.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="system"> remote reference to the <code>ActivationSystem</code> </param>
		/// <exception cref="ActivationException"> if activation system is already set </exception>
		/// <exception cref="SecurityException"> if permission to set the activation system is denied.
		/// (Note: The default implementation of the security manager
		/// <code>checkSetFactory</code>
		/// method requires the RuntimePermission "setFactory") </exception>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		/// not supported by this implementation </exception>
		/// <seealso cref= #getSystem </seealso>
		/// <seealso cref= SecurityManager#checkSetFactory
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void setSystem(ActivationSystem system) throws ActivationException
		public static ActivationSystem System
		{
			set
			{
				lock (typeof(ActivationGroup))
				{
					SecurityManager security = System.SecurityManager;
					if (security != null)
					{
						security.CheckSetFactory();
					}
            
					if (CurrSystem != null)
					{
						throw new ActivationException("activation system already set");
					}
            
					CurrSystem = value;
				}
			}
			get
			{
				lock (typeof(ActivationGroup))
				{
					if (CurrSystem == null)
					{
						try
						{
							int port = AccessController.doPrivileged(new GetIntegerAction("java.rmi.activation.port", ActivationSystem_Fields.SYSTEM_PORT));
							CurrSystem = (ActivationSystem) Naming.Lookup("//:" + port + "/java.rmi.activation.ActivationSystem");
						}
						catch (Exception e)
						{
							throw new ActivationException("unable to obtain ActivationSystem", e);
						}
					}
					return CurrSystem;
				}
			}
		}


		/// <summary>
		/// This protected method is necessary for subclasses to
		/// make the <code>activeObject</code> callback to the group's
		/// monitor. The call is simply forwarded to the group's
		/// <code>ActivationMonitor</code>.
		/// </summary>
		/// <param name="id"> the object's identifier </param>
		/// <param name="mobj"> a marshalled object containing the remote object's stub </param>
		/// <exception cref="UnknownObjectException"> if object is not registered </exception>
		/// <exception cref="RemoteException"> if call informing monitor fails </exception>
		/// <exception cref="ActivationException"> if an activation error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void activeObject(ActivationID id, java.rmi.MarshalledObject<? extends java.rmi.Remote> mobj) throws ActivationException, java.rmi.activation.UnknownObjectException, java.rmi.RemoteException
		protected internal virtual void activeObject<T1>(ActivationID id, MarshalledObject<T1> mobj) where T1 : java.rmi.Remote
		{
			Monitor.ActiveObject(id, mobj);
		}

		/// <summary>
		/// This protected method is necessary for subclasses to
		/// make the <code>inactiveGroup</code> callback to the group's
		/// monitor. The call is simply forwarded to the group's
		/// <code>ActivationMonitor</code>. Also, the current group
		/// for the VM is set to null.
		/// </summary>
		/// <exception cref="UnknownGroupException"> if group is not registered </exception>
		/// <exception cref="RemoteException"> if call informing monitor fails
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void inactiveGroup() throws java.rmi.activation.UnknownGroupException, java.rmi.RemoteException
		protected internal virtual void InactiveGroup()
		{
			try
			{
				Monitor.InactiveGroup(GroupID, Incarnation);
			}
			finally
			{
				DestroyGroup();
			}
		}

		/// <summary>
		/// Returns the monitor for the activation group.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ActivationMonitor getMonitor() throws java.rmi.RemoteException
		private ActivationMonitor Monitor
		{
			get
			{
				lock (typeof(ActivationGroup))
				{
					if (Monitor_Renamed != null)
					{
						return Monitor_Renamed;
					}
				}
				throw new RemoteException("monitor not received");
			}
		}

		/// <summary>
		/// Destroys the current group.
		/// </summary>
		private static void DestroyGroup()
		{
			lock (typeof(ActivationGroup))
			{
				CurrGroup = null;
				CurrGroupID = null;
				// NOTE: don't set currSystem to null since it may be needed
			}
		}

		/// <summary>
		/// Returns the current group for the VM. </summary>
		/// <exception cref="ActivationException"> if current group is null (not active) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static synchronized ActivationGroup currentGroup() throws ActivationException
		internal static ActivationGroup CurrentGroup()
		{
			lock (typeof(ActivationGroup))
			{
				if (CurrGroup == null)
				{
					throw new ActivationException("group is not active");
				}
				return CurrGroup;
			}
		}

	}

}