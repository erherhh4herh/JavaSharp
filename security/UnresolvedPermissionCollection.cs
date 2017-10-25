using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A UnresolvedPermissionCollection stores a collection
	/// of UnresolvedPermission permissions.
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.UnresolvedPermission
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>

	[Serializable]
	internal sealed class UnresolvedPermissionCollection : PermissionCollection
	{
		/// <summary>
		/// Key is permission type, value is a list of the UnresolvedPermissions
		/// of the same type.
		/// Not serialized; see serialization section at end of class.
		/// </summary>
		[NonSerialized]
		private Map<String, List<UnresolvedPermission>> Perms;

		/// <summary>
		/// Create an empty UnresolvedPermissionCollection object.
		/// 
		/// </summary>
		public UnresolvedPermissionCollection()
		{
			Perms = new HashMap<String, List<UnresolvedPermission>>(11);
		}

		/// <summary>
		/// Adds a permission to this UnresolvedPermissionCollection.
		/// The key for the hash is the unresolved permission's type (class) name.
		/// </summary>
		/// <param name="permission"> the Permission object to add. </param>

		public override void Add(Permission permission)
		{
			if (!(permission is UnresolvedPermission))
			{
				throw new IllegalArgumentException("invalid permission: " + permission);
			}
			UnresolvedPermission up = (UnresolvedPermission) permission;

			List<UnresolvedPermission> v;
			lock (this)
			{
				v = Perms.Get(up.Name);
				if (v == null)
				{
					v = new List<UnresolvedPermission>();
					Perms.Put(up.Name, v);
				}
			}
			lock (v)
			{
				v.Add(up);
			}
		}

		/// <summary>
		/// get any unresolved permissions of the same type as p,
		/// and return the List containing them.
		/// </summary>
		internal List<UnresolvedPermission> GetUnresolvedPermissions(Permission p)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return Perms.Get(p.GetType().FullName);
			}
		}

		/// <summary>
		/// always returns false for unresolved permissions
		/// 
		/// </summary>
		public override bool Implies(Permission permission)
		{
			return false;
		}

		/// <summary>
		/// Returns an enumeration of all the UnresolvedPermission lists in the
		/// container.
		/// </summary>
		/// <returns> an enumeration of all the UnresolvedPermission objects. </returns>

		public override IEnumerator<Permission> Elements()
		{
			List<Permission> results = new List<Permission>(); // where results are stored

			// Get iterator of Map values (which are lists of permissions)
			lock (this)
			{
				foreach (List<UnresolvedPermission> l in Perms.Values())
				{
					lock (l)
					{
						results.AddAll(l);
					}
				}
			}

			return Collections.Enumeration(results);
		}

		private const long SerialVersionUID = -7176153071733132400L;

		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:
		// private Hashtable permissions; // keyed on type

		/// <summary>
		/// @serialField permissions java.util.Hashtable
		///     A table of the UnresolvedPermissions keyed on type, value is Vector
		///     of permissions
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("permissions", typeof(Hashtable))};

		/// <summary>
		/// @serialData Default field.
		/// </summary>
		/*
		 * Writes the contents of the perms field out as a Hashtable
		 * in which the values are Vectors for
		 * serialization compatibility with earlier releases.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call out.defaultWriteObject()

			// Copy perms into a Hashtable
			Dictionary<String, Vector<UnresolvedPermission>> permissions = new Dictionary<String, Vector<UnresolvedPermission>>(Perms.Size() * 2);

			// Convert each entry (List) into a Vector
			lock (this)
			{
				Set<Map_Entry<String, List<UnresolvedPermission>>> set = Perms.EntrySet();
				foreach (Map_Entry<String, List<UnresolvedPermission>> e in set)
				{
					// Convert list into Vector
					List<UnresolvedPermission> list = e.Value;
					Vector<UnresolvedPermission> vec = new Vector<UnresolvedPermission>(list.Count);
					lock (list)
					{
						vec.AddAll(list);
					}

					// Add to Hashtable being serialized
					permissions.Put(e.Key, vec);
				}
			}

			// Write out serializable fields
			ObjectOutputStream.PutField pfields = @out.PutFields();
			pfields.Put("permissions", permissions);
			@out.WriteFields();
		}

		/*
		 * Reads in a Hashtable in which the values are Vectors of
		 * UnresolvedPermissions and saves them in the perms field.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get permissions
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Hashtable<String, Vector<UnresolvedPermission>> permissions = (Hashtable<String, Vector<UnresolvedPermission>>) gfields.get("permissions", null);
			Dictionary<String, Vector<UnresolvedPermission>> permissions = (Dictionary<String, Vector<UnresolvedPermission>>) gfields.Get("permissions", null);
			// writeObject writes a Hashtable<String, Vector<UnresolvedPermission>>
			// for the permissions key, so this cast is safe, unless the data is corrupt.
			Perms = new HashMap<String, List<UnresolvedPermission>>(permissions.Size() * 2);

			// Convert each entry (Vector) into a List
			Set<Map_Entry<String, Vector<UnresolvedPermission>>> set = permissions.EntrySet();
			foreach (Map_Entry<String, Vector<UnresolvedPermission>> e in set)
			{
				// Convert Vector into ArrayList
				Vector<UnresolvedPermission> vec = e.Value;
				List<UnresolvedPermission> list = new List<UnresolvedPermission>(vec.Size());
				list.AddAll(vec);

				// Add to Hashtable being serialized
				Perms.Put(e.Key, list);
			}
		}
	}

}