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
	/// This class represents a heterogeneous collection of Permissions. That is,
	/// it contains different types of Permission objects, organized into
	/// PermissionCollections. For example, if any
	/// {@code java.io.FilePermission} objects are added to an instance of
	/// this class, they are all stored in a single
	/// PermissionCollection. It is the PermissionCollection returned by a call to
	/// the {@code newPermissionCollection} method in the FilePermission class.
	/// Similarly, any {@code java.lang.RuntimePermission} objects are
	/// stored in the PermissionCollection returned by a call to the
	/// {@code newPermissionCollection} method in the
	/// RuntimePermission class. Thus, this class represents a collection of
	/// PermissionCollections.
	/// 
	/// <para>When the {@code add} method is called to add a Permission, the
	/// Permission is stored in the appropriate PermissionCollection. If no such
	/// collection exists yet, the Permission object's class is determined and the
	/// {@code newPermissionCollection} method is called on that class to create
	/// the PermissionCollection and add it to the Permissions object. If
	/// {@code newPermissionCollection} returns null, then a default
	/// PermissionCollection that uses a hashtable will be created and used. Each
	/// hashtable entry stores a Permission object as both the key and the value.
	/// 
	/// </para>
	/// <para> Enumerations returned via the {@code elements} method are
	/// not <em>fail-fast</em>.  Modifications to a collection should not be
	/// performed while enumerating over that collection.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Permission </seealso>
	/// <seealso cref= PermissionCollection </seealso>
	/// <seealso cref= AllPermission
	/// 
	/// 
	/// @author Marianne Mueller
	/// @author Roland Schemers
	/// 
	/// @serial exclude </seealso>

	[Serializable]
	public sealed class Permissions : PermissionCollection
	{
		/// <summary>
		/// Key is permissions Class, value is PermissionCollection for that class.
		/// Not serialized; see serialization section at end of class.
		/// </summary>
		[NonSerialized]
		private IDictionary<Class, PermissionCollection> PermsMap;

		// optimization. keep track of whether unresolved permissions need to be
		// checked
		[NonSerialized]
		private bool HasUnresolved = false;

		// optimization. keep track of the AllPermission collection
		// - package private for ProtectionDomain optimization
		internal PermissionCollection AllPermission;

		/// <summary>
		/// Creates a new Permissions object containing no PermissionCollections.
		/// </summary>
		public Permissions()
		{
			PermsMap = new Dictionary<Class, PermissionCollection>(11);
			AllPermission = null;
		}

		/// <summary>
		/// Adds a permission object to the PermissionCollection for the class the
		/// permission belongs to. For example, if <i>permission</i> is a
		/// FilePermission, it is added to the FilePermissionCollection stored
		/// in this Permissions object.
		/// 
		/// This method creates
		/// a new PermissionCollection object (and adds the permission to it)
		/// if an appropriate collection does not yet exist. <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="SecurityException"> if this Permissions object is
		/// marked as readonly.
		/// </exception>
		/// <seealso cref= PermissionCollection#isReadOnly() </seealso>

		public override void Add(Permission permission)
		{
			if (ReadOnly)
			{
				throw new SecurityException("attempt to add a Permission to a readonly Permissions object");
			}

			PermissionCollection pc;

			lock (this)
			{
				pc = GetPermissionCollection(permission, true);
				pc.Add(permission);
			}

			// No sync; staleness -> optimizations delayed, which is OK
			if (permission is AllPermission)
			{
				AllPermission = pc;
			}
			if (permission is UnresolvedPermission)
			{
				HasUnresolved = true;
			}
		}

		/// <summary>
		/// Checks to see if this object's PermissionCollection for permissions of
		/// the specified permission's class implies the permissions
		/// expressed in the <i>permission</i> object. Returns true if the
		/// combination of permissions in the appropriate PermissionCollection
		/// (e.g., a FilePermissionCollection for a FilePermission) together
		/// imply the specified permission.
		/// 
		/// <para>For example, suppose there is a FilePermissionCollection in this
		/// Permissions object, and it contains one FilePermission that specifies
		/// "read" access for  all files in all subdirectories of the "/tmp"
		/// directory, and another FilePermission that specifies "write" access
		/// for all files in the "/tmp/scratch/foo" directory.
		/// Then if the {@code implies} method
		/// is called with a permission specifying both "read" and "write" access
		/// to files in the "/tmp/scratch/foo" directory, {@code true} is
		/// returned.
		/// 
		/// </para>
		/// <para>Additionally, if this PermissionCollection contains the
		/// AllPermission, this method will always return true.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="permission"> the Permission object to check.
		/// </param>
		/// <returns> true if "permission" is implied by the permissions in the
		/// PermissionCollection it
		/// belongs to, false if not. </returns>

		public override bool Implies(Permission permission)
		{
			// No sync; staleness -> skip optimization, which is OK
			if (AllPermission != null)
			{
				return true; // AllPermission has already been added
			}
			else
			{
				lock (this)
				{
					PermissionCollection pc = GetPermissionCollection(permission, false);
					if (pc != null)
					{
						return pc.Implies(permission);
					}
					else
					{
						// none found
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of all the Permission objects in all the
		/// PermissionCollections in this Permissions object.
		/// </summary>
		/// <returns> an enumeration of all the Permissions. </returns>

		public override IEnumerator<Permission> Elements()
		{
			// go through each Permissions in the hash table
			// and call their elements() function.

			lock (this)
			{
				return new PermissionsEnumerator(PermsMap.Values.GetEnumerator());
			}
		}

		/// <summary>
		/// Gets the PermissionCollection in this Permissions object for
		/// permissions whose type is the same as that of <i>p</i>.
		/// For example, if <i>p</i> is a FilePermission,
		/// the FilePermissionCollection
		/// stored in this Permissions object will be returned.
		/// 
		/// If createEmpty is true,
		/// this method creates a new PermissionCollection object for the specified
		/// type of permission objects if one does not yet exist.
		/// To do so, it first calls the {@code newPermissionCollection} method
		/// on <i>p</i>.  Subclasses of class Permission
		/// override that method if they need to store their permissions in a
		/// particular PermissionCollection object in order to provide the
		/// correct semantics when the {@code PermissionCollection.implies}
		/// method is called.
		/// If the call returns a PermissionCollection, that collection is stored
		/// in this Permissions object. If the call returns null and createEmpty
		/// is true, then
		/// this method instantiates and stores a default PermissionCollection
		/// that uses a hashtable to store its permission objects.
		/// 
		/// createEmpty is ignored when creating empty PermissionCollection
		/// for unresolved permissions because of the overhead of determining the
		/// PermissionCollection to use.
		/// 
		/// createEmpty should be set to false when this method is invoked from
		/// implies() because it incurs the additional overhead of creating and
		/// adding an empty PermissionCollection that will just return false.
		/// It should be set to true when invoked from add().
		/// </summary>
		private PermissionCollection GetPermissionCollection(Permission p, bool createEmpty)
		{
			Class c = p.GetType();

			PermissionCollection pc = PermsMap[c];

			if (!HasUnresolved && !createEmpty)
			{
				return pc;
			}
			else if (pc == null)
			{

				// Check for unresolved permissions
				pc = (HasUnresolved ? GetUnresolvedPermissions(p) : null);

				// if still null, create a new collection
				if (pc == null && createEmpty)
				{

					pc = p.NewPermissionCollection();

					// still no PermissionCollection?
					// We'll give them a PermissionsHash.
					if (pc == null)
					{
						pc = new PermissionsHash();
					}
				}

				if (pc != null)
				{
					PermsMap[c] = pc;
				}
			}
			return pc;
		}

		/// <summary>
		/// Resolves any unresolved permissions of type p.
		/// </summary>
		/// <param name="p"> the type of unresolved permission to resolve
		/// </param>
		/// <returns> PermissionCollection containing the unresolved permissions,
		///  or null if there were no unresolved permissions of type p.
		///  </returns>
		private PermissionCollection GetUnresolvedPermissions(Permission p)
		{
			// Called from within synchronized method so permsMap doesn't need lock

			UnresolvedPermissionCollection uc = (UnresolvedPermissionCollection) PermsMap[typeof(UnresolvedPermission)];

			// we have no unresolved permissions if uc is null
			if (uc == null)
			{
				return null;
			}

			IList<UnresolvedPermission> unresolvedPerms = uc.GetUnresolvedPermissions(p);

			// we have no unresolved permissions of this type if unresolvedPerms is null
			if (unresolvedPerms == null)
			{
				return null;
			}

			java.security.cert.Certificate[] certs = null;

			Object[] signers = p.GetType().Signers;

			int n = 0;
			if (signers != null)
			{
				for (int j = 0; j < signers.Length; j++)
				{
					if (signers[j] is java.security.cert.Certificate)
					{
						n++;
					}
				}
				certs = new java.security.cert.Certificate[n];
				n = 0;
				for (int j = 0; j < signers.Length; j++)
				{
					if (signers[j] is java.security.cert.Certificate)
					{
						certs[n++] = (java.security.cert.Certificate)signers[j];
					}
				}
			}

			PermissionCollection pc = null;
			lock (unresolvedPerms)
			{
				int len = unresolvedPerms.Count;
				for (int i = 0; i < len; i++)
				{
					UnresolvedPermission up = unresolvedPerms[i];
					Permission perm = up.Resolve(p, certs);
					if (perm != null)
					{
						if (pc == null)
						{
							pc = p.NewPermissionCollection();
							if (pc == null)
							{
								pc = new PermissionsHash();
							}
						}
						pc.Add(perm);
					}
				}
			}
			return pc;
		}

		private const long SerialVersionUID = 4858622370623524688L;

		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:
		// private Hashtable perms;

		/// <summary>
		/// @serialField perms java.util.Hashtable
		///     A table of the Permission classes and PermissionCollections.
		/// @serialField allPermission java.security.PermissionCollection
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("perms", typeof(Hashtable)), new ObjectStreamField("allPermission", typeof(PermissionCollection))};

		/// <summary>
		/// @serialData Default fields.
		/// </summary>
		/*
		 * Writes the contents of the permsMap field out as a Hashtable for
		 * serialization compatibility with earlier releases. allPermission
		 * unchanged.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call out.defaultWriteObject()

			// Copy perms into a Hashtable
			Dictionary<Class, PermissionCollection> perms = new Dictionary<Class, PermissionCollection>(PermsMap.Count * 2); // no sync; estimate
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				perms.putAll(PermsMap);
			}

			// Write out serializable fields
			ObjectOutputStream.PutField pfields = @out.PutFields();

			pfields.Put("allPermission", AllPermission); // no sync; staleness OK
			pfields.Put("perms", perms);
			@out.WriteFields();
		}

		/*
		 * Reads in a Hashtable of Class/PermissionCollections and saves them in the
		 * permsMap field. Reads in allPermission.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get allPermission
			AllPermission = (PermissionCollection) gfields.Get("allPermission", null);

			// Get permissions
			// writeObject writes a Hashtable<Class<?>, PermissionCollection> for
			// the perms key, so this cast is safe, unless the data is corrupt.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<Class, PermissionCollection> perms = (java.util.Hashtable<Class, PermissionCollection>)gfields.get("perms", null);
			Dictionary<Class, PermissionCollection> perms = (Dictionary<Class, PermissionCollection>)gfields.Get("perms", null);
			PermsMap = new Dictionary<Class, PermissionCollection>(perms.Count * 2);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			PermsMap.putAll(perms);

			// Set hasUnresolved
			UnresolvedPermissionCollection uc = (UnresolvedPermissionCollection) PermsMap[typeof(UnresolvedPermission)];
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			HasUnresolved = (uc != null && uc.Elements().hasMoreElements());
		}
	}

	internal sealed class PermissionsEnumerator : Iterator<Permission>
	{

		// all the perms
		private IEnumerator<PermissionCollection> Perms;
		// the current set
		private IEnumerator<Permission> Permset;

		internal PermissionsEnumerator(IEnumerator<PermissionCollection> e)
		{
			Perms = e;
			Permset = NextEnumWithMore;
		}

		// No need to synchronize; caller should sync on object as required
		public bool HasMoreElements()
		{
			// if we enter with permissionimpl null, we know
			// there are no more left.

			if (Permset == null)
			{
				return false;
			}

			// try to see if there are any left in the current one

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (Permset.hasMoreElements())
			{
				return true;
			}

			// get the next one that has something in it...
			Permset = NextEnumWithMore;

			// if it is null, we are done!
			return (Permset != null);
		}

		// No need to synchronize; caller should sync on object as required
		public Permission NextElement()
		{

			// hasMoreElements will update permset to the next permset
			// with something in it...

			if (HasMoreElements())
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return Permset.nextElement();
			}
			else
			{
				throw new NoSuchElementException("PermissionsEnumerator");
			}

		}

		private IEnumerator<Permission> NextEnumWithMore
		{
			get
			{
				while (Perms.MoveNext())
				{
					PermissionCollection pc = Perms.Current;
					IEnumerator<Permission> next = pc.Elements();
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (next.hasMoreElements())
					{
						return next;
					}
				}
				return null;
    
			}
		}
	}

	/// <summary>
	/// A PermissionsHash stores a homogeneous set of permissions in a hashtable.
	/// </summary>
	/// <seealso cref= Permission </seealso>
	/// <seealso cref= Permissions
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>

	[Serializable]
	internal sealed class PermissionsHash : PermissionCollection
	{
		/// <summary>
		/// Key and value are (same) permissions objects.
		/// Not serialized; see serialization section at end of class.
		/// </summary>
		[NonSerialized]
		private IDictionary<Permission, Permission> PermsMap;

		/// <summary>
		/// Create an empty PermissionsHash object.
		/// </summary>

		internal PermissionsHash()
		{
			PermsMap = new Dictionary<Permission, Permission>(11);
		}

		/// <summary>
		/// Adds a permission to the PermissionsHash.
		/// </summary>
		/// <param name="permission"> the Permission object to add. </param>

		public override void Add(Permission permission)
		{
			lock (this)
			{
				PermsMap[permission] = permission;
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
			// attempt a fast lookup and implies. If that fails
			// then enumerate through all the permissions.
			lock (this)
			{
				Permission p = PermsMap[permission];

				// If permission is found, then p.equals(permission)
				if (p == null)
				{
					foreach (Permission p_ in PermsMap.Values)
					{
						if (p_.Implies(permission))
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of all the Permission objects in the container.
		/// </summary>
		/// <returns> an enumeration of all the Permissions. </returns>

		public override IEnumerator<Permission> Elements()
		{
			// Convert Iterator of Map values into an Enumeration
			lock (this)
			{
				return Collections.Enumeration(PermsMap.Values);
			}
		}

		private const long SerialVersionUID = -8491988220802933440L;
		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:
		// private Hashtable perms;
		/// <summary>
		/// @serialField perms java.util.Hashtable
		///     A table of the Permissions (both key and value are same).
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("perms", typeof(Hashtable))};

		/// <summary>
		/// @serialData Default fields.
		/// </summary>
		/*
		 * Writes the contents of the permsMap field out as a Hashtable for
		 * serialization compatibility with earlier releases.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call out.defaultWriteObject()

			// Copy perms into a Hashtable
			Dictionary<Permission, Permission> perms = new Dictionary<Permission, Permission>(PermsMap.Count * 2);
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				perms.putAll(PermsMap);
			}

			// Write out serializable fields
			ObjectOutputStream.PutField pfields = @out.PutFields();
			pfields.Put("perms", perms);
			@out.WriteFields();
		}

		/*
		 * Reads in a Hashtable of Permission/Permission and saves them in the
		 * permsMap field.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get permissions
			// writeObject writes a Hashtable<Class<?>, PermissionCollection> for
			// the perms key, so this cast is safe, unless the data is corrupt.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<Permission, Permission> perms = (java.util.Hashtable<Permission, Permission>)gfields.get("perms", null);
			Dictionary<Permission, Permission> perms = (Dictionary<Permission, Permission>)gfields.Get("perms", null);
			PermsMap = new Dictionary<Permission, Permission>(perms.Count * 2);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			PermsMap.putAll(perms);
		}
	}

}