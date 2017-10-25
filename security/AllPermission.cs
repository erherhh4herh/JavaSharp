using System;
using System.Collections.Generic;

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

namespace java.security
{

	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// The AllPermission is a permission that implies all other permissions.
	/// <para>
	/// <b>Note:</b> Granting AllPermission should be done with extreme care,
	/// as it implies all other permissions. Thus, it grants code the ability
	/// to run with security
	/// disabled.  Extreme caution should be taken before granting such
	/// a permission to code.  This permission should be used only during testing,
	/// or in extremely rare cases where an application or applet is
	/// completely trusted and adding the necessary permissions to the policy
	/// is prohibitively cumbersome.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.AccessController </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection </seealso>
	/// <seealso cref= java.lang.SecurityManager
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial exclude </seealso>

	public sealed class AllPermission : Permission
	{

		private const long SerialVersionUID = -2916474571451318075L;

		/// <summary>
		/// Creates a new AllPermission object.
		/// </summary>
		public AllPermission() : base("<all permissions>")
		{
		}


		/// <summary>
		/// Creates a new AllPermission object. This
		/// constructor exists for use by the {@code Policy} object
		/// to instantiate new Permission objects.
		/// </summary>
		/// <param name="name"> ignored </param>
		/// <param name="actions"> ignored. </param>
		public AllPermission(String name, String actions) : this()
		{
		}

		/// <summary>
		/// Checks if the specified permission is "implied" by
		/// this object. This method always returns true.
		/// </summary>
		/// <param name="p"> the permission to check against.
		/// </param>
		/// <returns> return </returns>
		public override bool Implies(Permission p)
		{
			 return true;
		}

		/// <summary>
		/// Checks two AllPermission objects for equality. Two AllPermission
		/// objects are always equal.
		/// </summary>
		/// <param name="obj"> the object we are testing for equality with this object. </param>
		/// <returns> true if <i>obj</i> is an AllPermission, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			return (obj is AllPermission);
		}

		/// <summary>
		/// Returns the hash code value for this object.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>

		public override int HashCode()
		{
			return 1;
		}

		/// <summary>
		/// Returns the canonical string representation of the actions.
		/// </summary>
		/// <returns> the actions. </returns>
		public override String Actions
		{
			get
			{
				return "<all actions>";
			}
		}

		/// <summary>
		/// Returns a new PermissionCollection object for storing AllPermission
		/// objects.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a new PermissionCollection object suitable for
		/// storing AllPermissions. </returns>
		public override PermissionCollection NewPermissionCollection()
		{
			return new AllPermissionCollection();
		}

	}

	/// <summary>
	/// A AllPermissionCollection stores a collection
	/// of AllPermission permissions. AllPermission objects
	/// must be stored in a manner that allows them to be inserted in any
	/// order, but enable the implies function to evaluate the implies
	/// method in an efficient (and consistent) manner.
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>

	[Serializable]
	internal sealed class AllPermissionCollection : PermissionCollection
	{

		// use serialVersionUID from JDK 1.2.2 for interoperability
		private const long SerialVersionUID = -4023755556366636806L;

		private bool All_allowed; // true if any all permissions have been added

		/// <summary>
		/// Create an empty AllPermissions object.
		/// 
		/// </summary>

		public AllPermissionCollection()
		{
			All_allowed = false;
		}

		/// <summary>
		/// Adds a permission to the AllPermissions. The key for the hash is
		/// permission.path.
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="IllegalArgumentException"> - if the permission is not a
		///                                       AllPermission
		/// </exception>
		/// <exception cref="SecurityException"> - if this AllPermissionCollection object
		///                                has been marked readonly </exception>

		public override void Add(Permission permission)
		{
			if (!(permission is AllPermission))
			{
				throw new IllegalArgumentException("invalid permission: " + permission);
			}
			if (ReadOnly)
			{
				throw new SecurityException("attempt to add a Permission to a readonly PermissionCollection");
			}

			All_allowed = true; // No sync; staleness OK
		}

		/// <summary>
		/// Check and see if this set of permissions implies the permissions
		/// expressed in "permission".
		/// </summary>
		/// <param name="permission"> the Permission object to compare
		/// </param>
		/// <returns> always returns true. </returns>

		public override bool Implies(Permission permission)
		{
			return All_allowed; // No sync; staleness OK
		}

		/// <summary>
		/// Returns an enumeration of all the AllPermission objects in the
		/// container.
		/// </summary>
		/// <returns> an enumeration of all the AllPermission objects. </returns>
		public override IEnumerator<Permission> Elements()
		{
			return new IteratorAnonymousInnerClassHelper(this);
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<Permission>
		{
			private readonly AllPermissionCollection OuterInstance;

			public IteratorAnonymousInnerClassHelper(AllPermissionCollection outerInstance)
			{
				this.OuterInstance = outerInstance;
				hasMore = outerInstance.All_allowed;
			}

			private bool hasMore;

			public virtual bool HasMoreElements()
			{
				return hasMore;
			}

			public virtual Permission NextElement()
			{
				hasMore = false;
				return SecurityConstants.ALL_PERMISSION;
			}
		}
	}

}