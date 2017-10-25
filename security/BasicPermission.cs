using System;
using System.Collections;
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
	/// The BasicPermission class extends the Permission class, and
	/// can be used as the base class for permissions that want to
	/// follow the same naming convention as BasicPermission.
	/// <P>
	/// The name for a BasicPermission is the name of the given permission
	/// (for example, "exit",
	/// "setFactory", "print.queueJob", etc). The naming
	/// convention follows the  hierarchical property naming convention.
	/// An asterisk may appear by itself, or if immediately preceded by a "."
	/// may appear at the end of the name, to signify a wildcard match.
	/// For example, "*" and "java.*" signify a wildcard match, while "*java", "a*b",
	/// and "java*" do not.
	/// <P>
	/// The action string (inherited from Permission) is unused.
	/// Thus, BasicPermission is commonly used as the base class for
	/// "named" permissions
	/// (ones that contain a name but no actions list; you either have the
	/// named permission or you don't.)
	/// Subclasses may implement actions on top of BasicPermission,
	/// if desired.
	/// <para>
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection </seealso>
	/// <seealso cref= java.lang.SecurityManager
	/// 
	/// @author Marianne Mueller
	/// @author Roland Schemers </seealso>

	[Serializable]
	public abstract class BasicPermission : Permission
	{

		private const long SerialVersionUID = 6279438298436773498L;

		// does this permission have a wildcard at the end?
		[NonSerialized]
		private bool Wildcard;

		// the name without the wildcard on the end
		[NonSerialized]
		private String Path;

		// is this permission the old-style exitVM permission (pre JDK 1.6)?
		[NonSerialized]
		private bool ExitVM;

		/// <summary>
		/// initialize a BasicPermission object. Common to all constructors.
		/// </summary>
		private void Init(String name)
		{
			if (name == null)
			{
				throw new NullPointerException("name can't be null");
			}

			int len = name.Length();

			if (len == 0)
			{
				throw new IllegalArgumentException("name can't be empty");
			}

			char last = name.CharAt(len - 1);

			// Is wildcard or ends with ".*"?
			if (last == '*' && (len == 1 || name.CharAt(len - 2) == '.'))
			{
				Wildcard = true;
				if (len == 1)
				{
					Path = "";
				}
				else
				{
					Path = name.Substring(0, len - 1);
				}
			}
			else
			{
				if (name.Equals("exitVM"))
				{
					Wildcard = true;
					Path = "exitVM.";
					ExitVM = true;
				}
				else
				{
					Path = name;
				}
			}
		}

		/// <summary>
		/// Creates a new BasicPermission with the specified name.
		/// Name is the symbolic name of the permission, such as
		/// "setFactory",
		/// "print.queueJob", or "topLevelWindow", etc.
		/// </summary>
		/// <param name="name"> the name of the BasicPermission.
		/// </param>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		public BasicPermission(String name) : base(name)
		{
			Init(name);
		}


		/// <summary>
		/// Creates a new BasicPermission object with the specified name.
		/// The name is the symbolic name of the BasicPermission, and the
		/// actions String is currently unused.
		/// </summary>
		/// <param name="name"> the name of the BasicPermission. </param>
		/// <param name="actions"> ignored.
		/// </param>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		public BasicPermission(String name, String actions) : base(name)
		{
			Init(name);
		}

		/// <summary>
		/// Checks if the specified permission is "implied" by
		/// this object.
		/// <P>
		/// More specifically, this method returns true if:
		/// <ul>
		/// <li> <i>p</i>'s class is the same as this object's class, and
		/// <li> <i>p</i>'s name equals or (in the case of wildcards)
		///      is implied by this object's
		///      name. For example, "a.b.*" implies "a.b.c".
		/// </ul>
		/// </summary>
		/// <param name="p"> the permission to check against.
		/// </param>
		/// <returns> true if the passed permission is equal to or
		/// implied by this permission, false otherwise. </returns>
		public override bool Implies(Permission p)
		{
			if ((p == null) || (p.GetType() != this.GetType()))
			{
				return false;
			}

			BasicPermission that = (BasicPermission) p;

			if (this.Wildcard)
			{
				if (that.Wildcard)
				{
					// one wildcard can imply another
					return that.Path.StartsWith(Path);
				}
				else
				{
					// make sure ap.path is longer so a.b.* doesn't imply a.b
					return (that.Path.Length() > this.Path.Length()) && that.Path.StartsWith(this.Path);
				}
			}
			else
			{
				if (that.Wildcard)
				{
					// a non-wildcard can't imply a wildcard
					return false;
				}
				else
				{
					return this.Path.Equals(that.Path);
				}
			}
		}

		/// <summary>
		/// Checks two BasicPermission objects for equality.
		/// Checks that <i>obj</i>'s class is the same as this object's class
		/// and has the same name as this object.
		/// <P> </summary>
		/// <param name="obj"> the object we are testing for equality with this object. </param>
		/// <returns> true if <i>obj</i>'s class is the same as this object's class
		///  and has the same name as this BasicPermission object, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if ((obj == null) || (obj.GetType() != this.GetType()))
			{
				return false;
			}

			BasicPermission bp = (BasicPermission) obj;

			return Name.Equals(bp.Name);
		}


		/// <summary>
		/// Returns the hash code value for this object.
		/// The hash code used is the hash code of the name, that is,
		/// {@code getName().hashCode()}, where {@code getName} is
		/// from the Permission superclass.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>
		public override int HashCode()
		{
			return this.Name.HashCode();
		}

		/// <summary>
		/// Returns the canonical string representation of the actions,
		/// which currently is the empty string "", since there are no actions for
		/// a BasicPermission.
		/// </summary>
		/// <returns> the empty string "". </returns>
		public override String Actions
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Returns a new PermissionCollection object for storing BasicPermission
		/// objects.
		/// 
		/// <para>BasicPermission objects must be stored in a manner that allows them
		/// to be inserted in any order, but that also enables the
		/// PermissionCollection {@code implies} method
		/// to be implemented in an efficient (and consistent) manner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a new PermissionCollection object suitable for
		/// storing BasicPermissions. </returns>
		public override PermissionCollection NewPermissionCollection()
		{
			return new BasicPermissionCollection(this.GetType());
		}

		/// <summary>
		/// readObject is called to restore the state of the BasicPermission from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();
			// init is called to initialize the rest of the values.
			Init(Name);
		}

		/// <summary>
		/// Returns the canonical name of this BasicPermission.
		/// All internal invocations of getName should invoke this method, so
		/// that the pre-JDK 1.6 "exitVM" and current "exitVM.*" permission are
		/// equivalent in equals/hashCode methods.
		/// </summary>
		/// <returns> the canonical name of this BasicPermission. </returns>
		internal String CanonicalName
		{
			get
			{
				return ExitVM ? "exitVM.*" : Name;
			}
		}
	}

	/// <summary>
	/// A BasicPermissionCollection stores a collection
	/// of BasicPermission permissions. BasicPermission objects
	/// must be stored in a manner that allows them to be inserted in any
	/// order, but enable the implies function to evaluate the implies
	/// method in an efficient (and consistent) manner.
	/// 
	/// A BasicPermissionCollection handles comparing a permission like "a.b.c.d.e"
	/// with a Permission such as "a.b.*", or "*".
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>

	[Serializable]
	internal sealed class BasicPermissionCollection : PermissionCollection
	{

		private const long SerialVersionUID = 739301742472979399L;

		/// <summary>
		/// Key is name, value is permission. All permission objects in
		/// collection must be of the same type.
		/// Not serialized; see serialization section at end of class.
		/// </summary>
		[NonSerialized]
		private IDictionary<String, Permission> Perms;

		/// <summary>
		/// This is set to {@code true} if this BasicPermissionCollection
		/// contains a BasicPermission with '*' as its permission name.
		/// </summary>
		/// <seealso cref= #serialPersistentFields </seealso>
		private bool All_allowed;

		/// <summary>
		/// The class to which all BasicPermissions in this
		/// BasicPermissionCollection belongs.
		/// </summary>
		/// <seealso cref= #serialPersistentFields </seealso>
		private Class PermClass;

		/// <summary>
		/// Create an empty BasicPermissionCollection object.
		/// 
		/// </summary>

		public BasicPermissionCollection(Class clazz)
		{
			Perms = new Dictionary<String, Permission>(11);
			All_allowed = false;
			PermClass = clazz;
		}

		/// <summary>
		/// Adds a permission to the BasicPermissions. The key for the hash is
		/// permission.path.
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="IllegalArgumentException"> - if the permission is not a
		///                                       BasicPermission, or if
		///                                       the permission is not of the
		///                                       same Class as the other
		///                                       permissions in this collection.
		/// </exception>
		/// <exception cref="SecurityException"> - if this BasicPermissionCollection object
		///                                has been marked readonly </exception>
		public override void Add(Permission permission)
		{
			if (!(permission is BasicPermission))
			{
				throw new IllegalArgumentException("invalid permission: " + permission);
			}
			if (ReadOnly)
			{
				throw new SecurityException("attempt to add a Permission to a readonly PermissionCollection");
			}

			BasicPermission bp = (BasicPermission) permission;

			// make sure we only add new BasicPermissions of the same class
			// Also check null for compatibility with deserialized form from
			// previous versions.
			if (PermClass == null)
			{
				// adding first permission
				PermClass = bp.GetType();
			}
			else
			{
				if (bp.GetType() != PermClass)
				{
					throw new IllegalArgumentException("invalid permission: " + permission);
				}
			}

			lock (this)
			{
				Perms[bp.CanonicalName] = permission;
			}

			// No sync on all_allowed; staleness OK
			if (!All_allowed)
			{
				if (bp.CanonicalName.Equals("*"))
				{
					All_allowed = true;
				}
			}
		}

		/// <summary>
		/// Check and see if this set of permissions implies the permissions
		/// expressed in "permission".
		/// </summary>
		/// <param name="permission"> the Permission object to compare
		/// </param>
		/// <returns> true if "permission" is a proper subset of a permission in
		/// the set, false if not. </returns>
		public override bool Implies(Permission permission)
		{
			if (!(permission is BasicPermission))
			{
				return false;
			}

			BasicPermission bp = (BasicPermission) permission;

			// random subclasses of BasicPermission do not imply each other
			if (bp.GetType() != PermClass)
			{
				return false;
			}

			// short circuit if the "*" Permission was added
			if (All_allowed)
			{
				return true;
			}

			// strategy:
			// Check for full match first. Then work our way up the
			// path looking for matches on a.b..*

			String path = bp.CanonicalName;
			//System.out.println("check "+path);

			Permission x;

			lock (this)
			{
				x = Perms[path];
			}

			if (x != null)
			{
				// we have a direct hit!
				return x.Implies(permission);
			}

			// work our way up the tree...
			int last, offset;

			offset = path.Length() - 1;

			while ((last = path.LastIndexOf(".", offset)) != -1)
			{

				path = path.Substring(0, last + 1) + "*";
				//System.out.println("check "+path);

				lock (this)
				{
					x = Perms[path];
				}

				if (x != null)
				{
					return x.Implies(permission);
				}
				offset = last - 1;
			}

			// we don't have to check for "*" as it was already checked
			// at the top (all_allowed), so we just return false
			return false;
		}

		/// <summary>
		/// Returns an enumeration of all the BasicPermission objects in the
		/// container.
		/// </summary>
		/// <returns> an enumeration of all the BasicPermission objects. </returns>
		public override IEnumerator<Permission> Elements()
		{
			// Convert Iterator of Map values into an Enumeration
			lock (this)
			{
				return Collections.Enumeration(Perms.Values);
			}
		}

		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:
		//
		// @serial the Hashtable is indexed by the BasicPermission name
		//
		// private Hashtable permissions;
		/// <summary>
		/// @serialField permissions java.util.Hashtable
		///    The BasicPermissions in this BasicPermissionCollection.
		///    All BasicPermissions in the collection must belong to the same class.
		///    The Hashtable is indexed by the BasicPermission name; the value
		///    of the Hashtable entry is the permission.
		/// @serialField all_allowed boolean
		///   This is set to {@code true} if this BasicPermissionCollection
		///   contains a BasicPermission with '*' as its permission name.
		/// @serialField permClass java.lang.Class
		///   The class to which all BasicPermissions in this
		///   BasicPermissionCollection belongs.
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("permissions", typeof(Hashtable)), new ObjectStreamField("all_allowed", Boolean.TYPE), new ObjectStreamField("permClass", typeof(Class))};

		/// <summary>
		/// @serialData Default fields.
		/// </summary>
		/*
		 * Writes the contents of the perms field out as a Hashtable for
		 * serialization compatibility with earlier releases. all_allowed
		 * and permClass unchanged.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call out.defaultWriteObject()

			// Copy perms into a Hashtable
			Dictionary<String, Permission> permissions = new Dictionary<String, Permission>(Perms.Count * 2);

			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				permissions.putAll(Perms);
			}

			// Write out serializable fields
			ObjectOutputStream.PutField pfields = @out.PutFields();
			pfields.Put("all_allowed", All_allowed);
			pfields.Put("permissions", permissions);
			pfields.Put("permClass", PermClass);
			@out.WriteFields();
		}

		/// <summary>
		/// readObject is called to restore the state of the
		/// BasicPermissionCollection from a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get permissions
			// writeObject writes a Hashtable<String, Permission> for the
			// permissions key, so this cast is safe, unless the data is corrupt.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<String, Permission> permissions = (java.util.Hashtable<String, Permission>)gfields.get("permissions", null);
			Dictionary<String, Permission> permissions = (Dictionary<String, Permission>)gfields.Get("permissions", null);
			Perms = new Dictionary<String, Permission>(permissions.Count * 2);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			Perms.putAll(permissions);

			// Get all_allowed
			All_allowed = gfields.Get("all_allowed", false);

			// Get permClass
			PermClass = (Class) gfields.Get("permClass", null);

			if (PermClass == null)
			{
				// set permClass
				IEnumerator<Permission> e = permissions.Values.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (e.hasMoreElements())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Permission p = e.nextElement();
					PermClass = p.GetType();
				}
			}
		}
	}

}