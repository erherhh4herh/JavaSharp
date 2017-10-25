/*
 * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// The <code>Activator</code> facilitates remote object activation. A
	/// "faulting" remote reference calls the activator's
	/// <code>activate</code> method to obtain a "live" reference to a
	/// "activatable" remote object. Upon receiving a request for activation,
	/// the activator looks up the activation descriptor for the activation
	/// identifier, <code>id</code>, determines the group in which the
	/// object should be activated initiates object re-creation via the
	/// group's <code>ActivationInstantiator</code> (via a call to the
	/// <code>newInstance</code> method). The activator initiates the
	/// execution of activation groups as necessary. For example, if an
	/// activation group for a specific group identifier is not already
	/// executing, the activator initiates the execution of a VM for the
	/// group. <para>
	/// 
	/// The <code>Activator</code> works closely with
	/// <code>ActivationSystem</code>, which provides a means for registering
	/// groups and objects within those groups, and <code>ActivationMonitor</code>,
	/// which recives information about active and inactive objects and inactive
	/// </para>
	/// groups. <para>
	/// 
	/// The activator is responsible for monitoring and detecting when
	/// activation groups fail so that it can remove stale remote references
	/// </para>
	/// to groups and active object's within those groups.<para>
	/// 
	/// @author      Ann Wollrath
	/// </para>
	/// </summary>
	/// <seealso cref=         ActivationInstantiator </seealso>
	/// <seealso cref=         ActivationGroupDesc </seealso>
	/// <seealso cref=         ActivationGroupID
	/// @since       1.2 </seealso>
	public interface Activator : Remote
	{
		/// <summary>
		/// Activate the object associated with the activation identifier,
		/// <code>id</code>. If the activator knows the object to be active
		/// already, and <code>force</code> is false , the stub with a
		/// "live" reference is returned immediately to the caller;
		/// otherwise, if the activator does not know that corresponding
		/// the remote object is active, the activator uses the activation
		/// descriptor information (previously registered) to determine the
		/// group (VM) in which the object should be activated. If an
		/// <code>ActivationInstantiator</code> corresponding to the
		/// object's group descriptor already exists, the activator invokes
		/// the activation group's <code>newInstance</code> method passing
		/// it the object's id and descriptor. <para>
		/// 
		/// If the activation group for the object's group descriptor does
		/// not yet exist, the activator starts an
		/// <code>ActivationInstantiator</code> executing (by spawning a
		/// child process, for example). When the activator receives the
		/// activation group's call back (via the
		/// <code>ActivationSystem</code>'s <code>activeGroup</code>
		/// method) specifying the activation group's reference, the
		/// activator can then invoke that activation instantiator's
		/// <code>newInstance</code> method to forward each pending
		/// activation request to the activation group and return the
		/// result (a marshalled remote object reference, a stub) to the
		/// </para>
		/// caller.<para>
		/// 
		/// Note that the activator receives a "marshalled" object instead of a
		/// Remote object so that the activator does not need to load the
		/// code for that object, or participate in distributed garbage
		/// collection for that object. If the activator kept a strong
		/// reference to the remote object, the activator would then
		/// prevent the object from being garbage collected under the
		/// </para>
		/// normal distributed garbage collection mechanism. <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the activation identifier for the object being activated </param>
		/// <param name="force"> if true, the activator contacts the group to obtain
		/// the remote object's reference; if false, returning the cached value
		/// is allowed. </param>
		/// <returns> the remote object (a stub) in a marshalled form </returns>
		/// <exception cref="ActivationException"> if object activation fails </exception>
		/// <exception cref="UnknownObjectException"> if object is unknown (not registered) </exception>
		/// <exception cref="RemoteException"> if remote call fails
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.rmi.MarshalledObject<? extends java.rmi.Remote> activate(ActivationID id, boolean force) throws ActivationException, java.rmi.activation.UnknownObjectException, java.rmi.RemoteException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.rmi.MarshalledObject<? extends java.rmi.Remote> activate(ActivationID id, boolean force) throws ActivationException, java.rmi.activation.UnknownObjectException, java.rmi.RemoteException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		MarshalledObject<?> Activate(ActivationID id, bool force) where ? : java.rmi.Remote;

	}

}