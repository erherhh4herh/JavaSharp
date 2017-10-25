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
	/// An <code>ActivationMonitor</code> is specific to an
	/// <code>ActivationGroup</code> and is obtained when a group is
	/// reported active via a call to
	/// <code>ActivationSystem.activeGroup</code> (this is done
	/// internally). An activation group is responsible for informing its
	/// <code>ActivationMonitor</code> when either: its objects become active or
	/// inactive, or the group as a whole becomes inactive.
	/// 
	/// @author      Ann Wollrath </summary>
	/// <seealso cref=         Activator </seealso>
	/// <seealso cref=         ActivationSystem </seealso>
	/// <seealso cref=         ActivationGroup
	/// @since       1.2 </seealso>
	public interface ActivationMonitor : Remote
	{

	   /// <summary>
	   /// An activation group calls its monitor's
	   /// <code>inactiveObject</code> method when an object in its group
	   /// becomes inactive (deactivates).  An activation group discovers
	   /// that an object (that it participated in activating) in its VM
	   /// is no longer active, via calls to the activation group's
	   /// <code>inactiveObject</code> method. <para>
	   ///  
	   /// The <code>inactiveObject</code> call informs the
	   /// <code>ActivationMonitor</code> that the remote object reference
	   /// it holds for the object with the activation identifier,
	   /// <code>id</code>, is no longer valid. The monitor considers the
	   /// reference associated with <code>id</code> as a stale reference.
	   /// Since the reference is considered stale, a subsequent
	   /// <code>activate</code> call for the same activation identifier
	   /// </para>
	   /// results in re-activating the remote object.<para>
	   ///  
	   /// </para>
	   /// </summary>
	   /// <param name="id"> the object's activation identifier </param>
	   /// <exception cref="UnknownObjectException"> if object is unknown </exception>
	   /// <exception cref="RemoteException"> if remote call fails
	   /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void inactiveObject(ActivationID id) throws java.rmi.activation.UnknownObjectException, java.rmi.RemoteException;
		void InactiveObject(ActivationID id);

		/// <summary>
		/// Informs that an object is now active. An <code>ActivationGroup</code>
		/// informs its monitor if an object in its group becomes active by
		/// other means than being activated directly (i.e., the object
		/// is registered and "activated" itself).
		/// </summary>
		/// <param name="id"> the active object's id </param>
		/// <param name="obj"> the marshalled form of the object's stub </param>
		/// <exception cref="UnknownObjectException"> if object is unknown </exception>
		/// <exception cref="RemoteException"> if remote call fails
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void activeObject(ActivationID id, java.rmi.MarshalledObject<? extends java.rmi.Remote> obj) throws java.rmi.activation.UnknownObjectException, java.rmi.RemoteException;
		void activeObject<T1>(ActivationID id, MarshalledObject<T1> obj) where T1 : java.rmi.Remote;

		/// <summary>
		/// Informs that the group is now inactive. The group will be
		/// recreated upon a subsequent request to activate an object
		/// within the group. A group becomes inactive when all objects
		/// in the group report that they are inactive.
		/// </summary>
		/// <param name="id"> the group's id </param>
		/// <param name="incarnation"> the group's incarnation number </param>
		/// <exception cref="UnknownGroupException"> if group is unknown </exception>
		/// <exception cref="RemoteException"> if remote call fails
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void inactiveGroup(ActivationGroupID id, long incarnation) throws java.rmi.activation.UnknownGroupException, java.rmi.RemoteException;
		void InactiveGroup(ActivationGroupID id, long incarnation);

	}

}