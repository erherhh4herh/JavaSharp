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

namespace java.security
{

	/// <summary>
	/// Abstract class for representing access to a system resource.
	/// All permissions have a name (whose interpretation depends on the subclass),
	/// as well as abstract functions for defining the semantics of the
	/// particular Permission subclass.
	/// 
	/// <para>Most Permission objects also include an "actions" list that tells the actions
	/// that are permitted for the object.  For example,
	/// for a {@code java.io.FilePermission} object, the permission name is
	/// the pathname of a file (or directory), and the actions list
	/// (such as "read, write") specifies which actions are granted for the
	/// specified file (or for files in the specified directory).
	/// The actions list is optional for Permission objects, such as
	/// {@code java.lang.RuntimePermission},
	/// that don't need such a list; you either have the named permission (such
	/// as "system.exit") or you don't.
	/// 
	/// </para>
	/// <para>An important method that must be implemented by each subclass is
	/// the {@code implies} method to compare Permissions. Basically,
	/// "permission p1 implies permission p2" means that
	/// if one is granted permission p1, one is naturally granted permission p2.
	/// Thus, this is not an equality test, but rather more of a
	/// subset test.
	/// 
	/// <P> Permission objects are similar to String objects in that they
	/// are immutable once they have been created. Subclasses should not
	/// provide methods that can change the state of a permission
	/// once it has been created.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Permissions </seealso>
	/// <seealso cref= PermissionCollection
	/// 
	/// 
	/// @author Marianne Mueller
	/// @author Roland Schemers </seealso>

	[Serializable]
	public abstract class Permission : Guard
	{

		private const long SerialVersionUID = -5636570222231596674L;

		private String Name_Renamed;

		/// <summary>
		/// Constructs a permission with the specified name.
		/// </summary>
		/// <param name="name"> name of the Permission object being created.
		///  </param>

		public Permission(String name)
		{
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Implements the guard interface for a permission. The
		/// {@code SecurityManager.checkPermission} method is called,
		/// passing this permission object as the permission to check.
		/// Returns silently if access is granted. Otherwise, throws
		/// a SecurityException.
		/// </summary>
		/// <param name="object"> the object being guarded (currently ignored).
		/// </param>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow access.
		/// </exception>
		/// <seealso cref= Guard </seealso>
		/// <seealso cref= GuardedObject </seealso>
		/// <seealso cref= SecurityManager#checkPermission
		///  </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkGuard(Object object) throws SecurityException
		public virtual void CheckGuard(Object @object)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(this);
			}
		}

		/// <summary>
		/// Checks if the specified permission's actions are "implied by"
		/// this object's actions.
		/// <P>
		/// This must be implemented by subclasses of Permission, as they are the
		/// only ones that can impose semantics on a Permission object.
		/// 
		/// <para>The {@code implies} method is used by the AccessController to determine
		/// whether or not a requested permission is implied by another permission that
		/// is known to be valid in the current execution context.
		/// 
		/// </para>
		/// </summary>
		/// <param name="permission"> the permission to check against.
		/// </param>
		/// <returns> true if the specified permission is implied by this object,
		/// false if not. </returns>

		public abstract bool Implies(Permission permission);

		/// <summary>
		/// Checks two Permission objects for equality.
		/// <P>
		/// Do not use the {@code equals} method for making access control
		/// decisions; use the {@code implies} method.
		/// </summary>
		/// <param name="obj"> the object we are testing for equality with this object.
		/// </param>
		/// <returns> true if both Permission objects are equivalent. </returns>

		public override abstract bool Equals(Object obj);

		/// <summary>
		/// Returns the hash code value for this Permission object.
		/// <P>
		/// The required {@code hashCode} behavior for Permission Objects is
		/// the following:
		/// <ul>
		/// <li>Whenever it is invoked on the same Permission object more than
		///     once during an execution of a Java application, the
		///     {@code hashCode} method
		///     must consistently return the same integer. This integer need not
		///     remain consistent from one execution of an application to another
		///     execution of the same application.
		/// <li>If two Permission objects are equal according to the
		///     {@code equals}
		///     method, then calling the {@code hashCode} method on each of the
		///     two Permission objects must produce the same integer result.
		/// </ul>
		/// </summary>
		/// <returns> a hash code value for this object. </returns>

		public override abstract int HashCode();

		/// <summary>
		/// Returns the name of this Permission.
		/// For example, in the case of a {@code java.io.FilePermission},
		/// the name will be a pathname.
		/// </summary>
		/// <returns> the name of this Permission.
		///  </returns>

		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the actions as a String. This is abstract
		/// so subclasses can defer creating a String representation until
		/// one is needed. Subclasses should always return actions in what they
		/// consider to be their
		/// canonical form. For example, two FilePermission objects created via
		/// the following:
		/// 
		/// <pre>
		///   perm1 = new FilePermission(p1,"read,write");
		///   perm2 = new FilePermission(p2,"write,read");
		/// </pre>
		/// 
		/// both return
		/// "read,write" when the {@code getActions} method is invoked.
		/// </summary>
		/// <returns> the actions of this Permission.
		///  </returns>

		public abstract String Actions {get;}

		/// <summary>
		/// Returns an empty PermissionCollection for a given Permission object, or null if
		/// one is not defined. Subclasses of class Permission should
		/// override this if they need to store their permissions in a particular
		/// PermissionCollection object in order to provide the correct semantics
		/// when the {@code PermissionCollection.implies} method is called.
		/// If null is returned,
		/// then the caller of this method is free to store permissions of this
		/// type in any PermissionCollection they choose (one that uses a Hashtable,
		/// one that uses a Vector, etc).
		/// </summary>
		/// <returns> a new PermissionCollection object for this type of Permission, or
		/// null if one is not defined. </returns>

		public virtual PermissionCollection NewPermissionCollection()
		{
			return null;
		}

		/// <summary>
		/// Returns a string describing this Permission.  The convention is to
		/// specify the class name, the permission name, and the actions in
		/// the following format: '("ClassName" "name" "actions")', or
		/// '("ClassName" "name")' if actions list is null or empty.
		/// </summary>
		/// <returns> information about this Permission. </returns>
		public override String ToString()
		{
			String actions = Actions;
			if ((actions == null) || (actions.Length() == 0)) // OPTIONAL
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return "(\"" + this.GetType().FullName + "\" \"" + Name_Renamed + "\")";
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return "(\"" + this.GetType().FullName + "\" \"" + Name_Renamed + "\" \"" + actions + "\")";
			}
		}
	}

}