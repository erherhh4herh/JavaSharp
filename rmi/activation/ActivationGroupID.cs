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

	/// <summary>
	/// The identifier for a registered activation group serves several
	/// purposes: <ul>
	/// <li>identifies the group uniquely within the activation system, and
	/// <li>contains a reference to the group's activation system so that the
	/// group can contact its activation system when necessary.</ul><para>
	/// 
	/// The <code>ActivationGroupID</code> is returned from the call to
	/// <code>ActivationSystem.registerGroup</code> and is used to identify
	/// the group within the activation system. This group id is passed
	/// as one of the arguments to the activation group's special constructor
	/// when an activation group is created/recreated.
	/// 
	/// @author      Ann Wollrath
	/// </para>
	/// </summary>
	/// <seealso cref=         ActivationGroup </seealso>
	/// <seealso cref=         ActivationGroupDesc
	/// @since       1.2 </seealso>
	[Serializable]
	public class ActivationGroupID
	{
		/// <summary>
		/// @serial The group's activation system.
		/// </summary>
		private ActivationSystem System_Renamed;

		/// <summary>
		/// @serial The group's unique id.
		/// </summary>
		private UID Uid = new UID();

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private const long SerialVersionUID = -1648432278909740833L;

		/// <summary>
		/// Constructs a unique group id.
		/// </summary>
		/// <param name="system"> the group's activation system </param>
		/// <exception cref="UnsupportedOperationException"> if and only if activation is
		///         not supported by this implementation
		/// @since 1.2 </exception>
		public ActivationGroupID(ActivationSystem system)
		{
			this.System_Renamed = system;
		}

		/// <summary>
		/// Returns the group's activation system. </summary>
		/// <returns> the group's activation system
		/// @since 1.2 </returns>
		public virtual ActivationSystem System
		{
			get
			{
				return System_Renamed;
			}
		}

		/// <summary>
		/// Returns a hashcode for the group's identifier.  Two group
		/// identifiers that refer to the same remote group will have the
		/// same hash code.
		/// </summary>
		/// <seealso cref= java.util.Hashtable
		/// @since 1.2 </seealso>
		public override int HashCode()
		{
			return Uid.HashCode();
		}

		/// <summary>
		/// Compares two group identifiers for content equality.
		/// Returns true if both of the following conditions are true:
		/// 1) the unique identifiers are equivalent (by content), and
		/// 2) the activation system specified in each
		///    refers to the same remote object.
		/// </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since 1.2 </seealso>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			else if (obj is ActivationGroupID)
			{
				ActivationGroupID id = (ActivationGroupID)obj;
				return (Uid.Equals(id.Uid) && System_Renamed.Equals(id.System_Renamed));
			}
			else
			{
				return false;
			}
		}
	}

}