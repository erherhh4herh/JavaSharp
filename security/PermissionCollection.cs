using System;
using System.Collections.Generic;

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

namespace java.security
{

	/// <summary>
	/// Abstract class representing a collection of Permission objects.
	/// 
	/// <para>With a PermissionCollection, you can:
	/// <UL>
	/// <LI> add a permission to the collection using the {@code add} method.
	/// <LI> check to see if a particular permission is implied in the
	///      collection, using the {@code implies} method.
	/// <LI> enumerate all the permissions, using the {@code elements} method.
	/// </UL>
	/// 
	/// </para>
	/// <para>When it is desirable to group together a number of Permission objects
	/// of the same type, the {@code newPermissionCollection} method on that
	/// particular type of Permission object should first be called. The default
	/// behavior (from the Permission class) is to simply return null.
	/// Subclasses of class Permission override the method if they need to store
	/// their permissions in a particular PermissionCollection object in order
	/// to provide the correct semantics when the
	/// {@code PermissionCollection.implies} method is called.
	/// If a non-null value is returned, that PermissionCollection must be used.
	/// If null is returned, then the caller of {@code newPermissionCollection}
	/// is free to store permissions of the
	/// given type in any PermissionCollection they choose
	/// (one that uses a Hashtable, one that uses a Vector, etc).
	/// 
	/// </para>
	/// <para>The PermissionCollection returned by the
	/// {@code Permission.newPermissionCollection}
	/// method is a homogeneous collection, which stores only Permission objects
	/// for a given Permission type.  A PermissionCollection may also be
	/// heterogeneous.  For example, Permissions is a PermissionCollection
	/// subclass that represents a collection of PermissionCollections.
	/// That is, its members are each a homogeneous PermissionCollection.
	/// For example, a Permissions object might have a FilePermissionCollection
	/// for all the FilePermission objects, a SocketPermissionCollection for all the
	/// SocketPermission objects, and so on. Its {@code add} method adds a
	/// permission to the appropriate collection.
	/// 
	/// </para>
	/// <para>Whenever a permission is added to a heterogeneous PermissionCollection
	/// such as Permissions, and the PermissionCollection doesn't yet contain a
	/// PermissionCollection of the specified permission's type, the
	/// PermissionCollection should call
	/// the {@code newPermissionCollection} method on the permission's class
	/// to see if it requires a special PermissionCollection. If
	/// {@code newPermissionCollection}
	/// returns null, the PermissionCollection
	/// is free to store the permission in any type of PermissionCollection it
	/// desires (one using a Hashtable, one using a Vector, etc.). For example,
	/// the Permissions object uses a default PermissionCollection implementation
	/// that stores the permission objects in a Hashtable.
	/// 
	/// </para>
	/// <para> Subclass implementations of PermissionCollection should assume
	/// that they may be called simultaneously from multiple threads,
	/// and therefore should be synchronized properly.  Furthermore,
	/// Enumerations returned via the {@code elements} method are
	/// not <em>fail-fast</em>.  Modifications to a collection should not be
	/// performed while enumerating over that collection.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Permission </seealso>
	/// <seealso cref= Permissions
	/// 
	/// 
	/// @author Roland Schemers </seealso>

	[Serializable]
	public abstract class PermissionCollection
	{

		private const long SerialVersionUID = -6727011328946861783L;

		// when set, add will throw an exception.
		private volatile bool ReadOnly_Renamed;

		/// <summary>
		/// Adds a permission object to the current collection of permission objects.
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="SecurityException"> -  if this PermissionCollection object
		///                                 has been marked readonly </exception>
		/// <exception cref="IllegalArgumentException"> - if this PermissionCollection
		///                object is a homogeneous collection and the permission
		///                is not of the correct type. </exception>
		public abstract void Add(Permission permission);

		/// <summary>
		/// Checks to see if the specified permission is implied by
		/// the collection of Permission objects held in this PermissionCollection.
		/// </summary>
		/// <param name="permission"> the Permission object to compare.
		/// </param>
		/// <returns> true if "permission" is implied by the  permissions in
		/// the collection, false if not. </returns>
		public abstract bool Implies(Permission permission);

		/// <summary>
		/// Returns an enumeration of all the Permission objects in the collection.
		/// </summary>
		/// <returns> an enumeration of all the Permissions. </returns>
		public abstract IEnumerator<Permission> Elements();

		/// <summary>
		/// Marks this PermissionCollection object as "readonly". After
		/// a PermissionCollection object
		/// is marked as readonly, no new Permission objects can be added to it
		/// using {@code add}.
		/// </summary>
		public virtual void SetReadOnly()
		{
			ReadOnly_Renamed = true;
		}

		/// <summary>
		/// Returns true if this PermissionCollection object is marked as readonly.
		/// If it is readonly, no new Permission objects can be added to it
		/// using {@code add}.
		/// 
		/// <para>By default, the object is <i>not</i> readonly. It can be set to
		/// readonly by a call to {@code setReadOnly}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this PermissionCollection object is marked as readonly,
		/// false otherwise. </returns>
		public virtual bool ReadOnly
		{
			get
			{
				return ReadOnly_Renamed;
			}
		}

		/// <summary>
		/// Returns a string describing this PermissionCollection object,
		/// providing information about all the permissions it contains.
		/// The format is:
		/// <pre>
		/// super.toString() (
		///   // enumerate all the Permission
		///   // objects and call toString() on them,
		///   // one per line..
		/// )</pre>
		/// 
		/// {@code super.toString} is a call to the {@code toString}
		/// method of this
		/// object's superclass, which is Object. The result is
		/// this PermissionCollection's type name followed by this object's
		/// hashcode, thus enabling clients to differentiate different
		/// PermissionCollections object, even if they contain the same permissions.
		/// </summary>
		/// <returns> information about this PermissionCollection object,
		///         as described above.
		///  </returns>
		public override String ToString()
		{
			IEnumerator<Permission> enum_ = Elements();
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString() + " (\n");
			while (enum_.MoveNext())
			{
				try
				{
					sb.Append(" ");
					sb.Append(enum_.Current.ToString());
					sb.Append("\n");
				}
				catch (NoSuchElementException)
				{
					// ignore
				}
			}
			sb.Append(")\n");
			return sb.ToString();
		}
	}

}