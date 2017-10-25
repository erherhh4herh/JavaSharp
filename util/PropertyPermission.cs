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

namespace java.util
{

	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// This class is for property permissions.
	/// 
	/// <P>
	/// The name is the name of the property ("java.home",
	/// "os.name", etc). The naming
	/// convention follows the  hierarchical property naming convention.
	/// Also, an asterisk
	/// may appear at the end of the name, following a ".", or by itself, to
	/// signify a wildcard match. For example: "java.*" and "*" signify a wildcard
	/// match, while "*java" and "a*b" do not.
	/// <P>
	/// The actions to be granted are passed to the constructor in a string containing
	/// a list of one or more comma-separated keywords. The possible keywords are
	/// "read" and "write". Their meaning is defined as follows:
	/// 
	/// <DL>
	///    <DT> read
	///    <DD> read permission. Allows <code>System.getProperty</code> to
	///         be called.
	///    <DT> write
	///    <DD> write permission. Allows <code>System.setProperty</code> to
	///         be called.
	/// </DL>
	/// <P>
	/// The actions string is converted to lowercase before processing.
	/// <P>
	/// Care should be taken before granting code permission to access
	/// certain system properties.  For example, granting permission to
	/// access the "java.home" system property gives potentially malevolent
	/// code sensitive information about the system environment (the Java
	/// installation directory).  Also, granting permission to access
	/// the "user.name" and "user.home" system properties gives potentially
	/// malevolent code sensitive information about the user environment
	/// (the user's account name and home directory).
	/// </summary>
	/// <seealso cref= java.security.BasicPermission </seealso>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection </seealso>
	/// <seealso cref= java.lang.SecurityManager
	/// 
	/// 
	/// @author Roland Schemers
	/// @since 1.2
	/// 
	/// @serial exclude </seealso>

	public sealed class PropertyPermission : BasicPermission
	{

		/// <summary>
		/// Read action.
		/// </summary>
		private const int READ = 0x1;

		/// <summary>
		/// Write action.
		/// </summary>
		private const int WRITE = 0x2;
		/// <summary>
		/// All actions (read,write);
		/// </summary>
		private static readonly int ALL = READ | WRITE;
		/// <summary>
		/// No actions.
		/// </summary>
		private const int NONE = 0x0;

		/// <summary>
		/// The actions mask.
		/// 
		/// </summary>
		[NonSerialized]
		private int Mask_Renamed;

		/// <summary>
		/// The actions string.
		/// 
		/// @serial
		/// </summary>
		private String Actions_Renamed; // Left null as long as possible, then
								// created and re-used in the getAction function.

		/// <summary>
		/// initialize a PropertyPermission object. Common to all constructors.
		/// Also called during de-serialization.
		/// </summary>
		/// <param name="mask"> the actions mask to use.
		///  </param>
		private void Init(int mask)
		{
			if ((mask & ALL) != mask)
			{
				throw new IllegalArgumentException("invalid actions mask");
			}

			if (mask == NONE)
			{
				throw new IllegalArgumentException("invalid actions mask");
			}

			if (Name == null)
			{
				throw new NullPointerException("name can't be null");
			}

			this.Mask_Renamed = mask;
		}

		/// <summary>
		/// Creates a new PropertyPermission object with the specified name.
		/// The name is the name of the system property, and
		/// <i>actions</i> contains a comma-separated list of the
		/// desired actions granted on the property. Possible actions are
		/// "read" and "write".
		/// </summary>
		/// <param name="name"> the name of the PropertyPermission. </param>
		/// <param name="actions"> the actions string.
		/// </param>
		/// <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if <code>name</code> is empty or if
		/// <code>actions</code> is invalid. </exception>
		public PropertyPermission(String name, String actions) : base(name,actions)
		{
			Init(GetMask(actions));
		}

		/// <summary>
		/// Checks if this PropertyPermission object "implies" the specified
		/// permission.
		/// <P>
		/// More specifically, this method returns true if:
		/// <ul>
		/// <li> <i>p</i> is an instanceof PropertyPermission,
		/// <li> <i>p</i>'s actions are a subset of this
		/// object's actions, and
		/// <li> <i>p</i>'s name is implied by this object's
		///      name. For example, "java.*" implies "java.home".
		/// </ul> </summary>
		/// <param name="p"> the permission to check against.
		/// </param>
		/// <returns> true if the specified permission is implied by this object,
		/// false if not. </returns>
		public override bool Implies(Permission p)
		{
			if (!(p is PropertyPermission))
			{
				return false;
			}

			PropertyPermission that = (PropertyPermission) p;

			// we get the effective mask. i.e., the "and" of this and that.
			// They must be equal to that.mask for implies to return true.

			return ((this.Mask_Renamed & that.Mask_Renamed) == that.Mask_Renamed) && base.Implies(that);
		}

		/// <summary>
		/// Checks two PropertyPermission objects for equality. Checks that <i>obj</i> is
		/// a PropertyPermission, and has the same name and actions as this object.
		/// <P> </summary>
		/// <param name="obj"> the object we are testing for equality with this object. </param>
		/// <returns> true if obj is a PropertyPermission, and has the same name and
		/// actions as this PropertyPermission object. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (!(obj is PropertyPermission))
			{
				return false;
			}

			PropertyPermission that = (PropertyPermission) obj;

			return (this.Mask_Renamed == that.Mask_Renamed) && (this.Name.Equals(that.Name));
		}

		/// <summary>
		/// Returns the hash code value for this object.
		/// The hash code used is the hash code of this permissions name, that is,
		/// <code>getName().hashCode()</code>, where <code>getName</code> is
		/// from the Permission superclass.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>
		public override int HashCode()
		{
			return this.Name.HashCode();
		}

		/// <summary>
		/// Converts an actions String to an actions mask.
		/// </summary>
		/// <param name="actions"> the action string. </param>
		/// <returns> the actions mask. </returns>
		private static int GetMask(String actions)
		{

			int mask = NONE;

			if (actions == null)
			{
				return mask;
			}

			// Use object identity comparison against known-interned strings for
			// performance benefit (these values are used heavily within the JDK).
			if (actions == SecurityConstants.PROPERTY_READ_ACTION)
			{
				return READ;
			}
			if (actions == SecurityConstants.PROPERTY_WRITE_ACTION)
			{
				return WRITE;
			}
			else if (actions == SecurityConstants.PROPERTY_RW_ACTION)
			{
				return READ | WRITE;
			}

			char[] a = actions.ToCharArray();

			int i = a.Length - 1;
			if (i < 0)
			{
				return mask;
			}

			while (i != -1)
			{
				char c;

				// skip whitespace
				while ((i != -1) && ((c = a[i]) == ' ' || c == '\r' || c == '\n' || c == '\f' || c == '\t'))
				{
					i--;
				}

				// check for the known strings
				int matchlen;

				if (i >= 3 && (a[i - 3] == 'r' || a[i - 3] == 'R') && (a[i - 2] == 'e' || a[i - 2] == 'E') && (a[i - 1] == 'a' || a[i - 1] == 'A') && (a[i] == 'd' || a[i] == 'D'))
				{
					matchlen = 4;
					mask |= READ;

				}
				else if (i >= 4 && (a[i - 4] == 'w' || a[i - 4] == 'W') && (a[i - 3] == 'r' || a[i - 3] == 'R') && (a[i - 2] == 'i' || a[i - 2] == 'I') && (a[i - 1] == 't' || a[i - 1] == 'T') && (a[i] == 'e' || a[i] == 'E'))
				{
					matchlen = 5;
					mask |= WRITE;

				}
				else
				{
					// parse error
					throw new IllegalArgumentException("invalid permission: " + actions);
				}

				// make sure we didn't just match the tail of a word
				// like "ackbarfaccept".  Also, skip to the comma.
				bool seencomma = false;
				while (i >= matchlen && !seencomma)
				{
					switch (a[i - matchlen])
					{
					case ',':
						seencomma = true;
						break;
					case ' ':
				case '\r':
			case '\n':
					case '\f':
				case '\t':
						break;
					default:
						throw new IllegalArgumentException("invalid permission: " + actions);
					}
					i--;
				}

				// point i at the location of the comma minus one (or -1).
				i -= matchlen;
			}

			return mask;
		}


		/// <summary>
		/// Return the canonical string representation of the actions.
		/// Always returns present actions in the following order:
		/// read, write.
		/// </summary>
		/// <returns> the canonical string representation of the actions. </returns>
		internal static String GetActions(int mask)
		{
			StringBuilder sb = new StringBuilder();
			bool comma = false;

			if ((mask & READ) == READ)
			{
				comma = true;
				sb.Append("read");
			}

			if ((mask & WRITE) == WRITE)
			{
				if (comma)
				{
					sb.Append(',');
				}
				else
				{
					comma = true;
				}
				sb.Append("write");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns the "canonical string representation" of the actions.
		/// That is, this method always returns present actions in the following order:
		/// read, write. For example, if this PropertyPermission object
		/// allows both write and read actions, a call to <code>getActions</code>
		/// will return the string "read,write".
		/// </summary>
		/// <returns> the canonical string representation of the actions. </returns>
		public override String Actions
		{
			get
			{
				if (Actions_Renamed == null)
				{
					Actions_Renamed = GetActions(this.Mask_Renamed);
				}
    
				return Actions_Renamed;
			}
		}

		/// <summary>
		/// Return the current action mask.
		/// Used by the PropertyPermissionCollection
		/// </summary>
		/// <returns> the actions mask. </returns>
		internal int Mask
		{
			get
			{
				return Mask_Renamed;
			}
		}

		/// <summary>
		/// Returns a new PermissionCollection object for storing
		/// PropertyPermission objects.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a new PermissionCollection object suitable for storing
		/// PropertyPermissions. </returns>
		public override PermissionCollection NewPermissionCollection()
		{
			return new PropertyPermissionCollection();
		}


		private const long SerialVersionUID = 885438825399942851L;

		/// <summary>
		/// WriteObject is called to save the state of the PropertyPermission
		/// to a stream. The actions are serialized, and the superclass
		/// takes care of the name.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
				// Write out the actions. The superclass takes care of the name
				// call getActions to make sure actions field is initialized
				if (Actions_Renamed == null)
				{
					Actions;
				}
				s.DefaultWriteObject();
			}
		}

		/// <summary>
		/// readObject is called to restore the state of the PropertyPermission from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			lock (this)
			{
				// Read in the action, then initialize the rest
				s.DefaultReadObject();
				Init(GetMask(Actions_Renamed));
			}
		}
	}

	/// <summary>
	/// A PropertyPermissionCollection stores a set of PropertyPermission
	/// permissions.
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection
	/// 
	/// 
	/// @author Roland Schemers
	/// 
	/// @serial include </seealso>
	[Serializable]
	internal sealed class PropertyPermissionCollection : PermissionCollection
	{

		/// <summary>
		/// Key is property name; value is PropertyPermission.
		/// Not serialized; see serialization section at end of class.
		/// </summary>
		[NonSerialized]
		private IDictionary<String, PropertyPermission> Perms;

		/// <summary>
		/// Boolean saying if "*" is in the collection.
		/// </summary>
		/// <seealso cref= #serialPersistentFields </seealso>
		// No sync access; OK for this to be stale.
		private bool All_allowed;

		/// <summary>
		/// Create an empty PropertyPermissionCollection object.
		/// </summary>
		public PropertyPermissionCollection()
		{
			Perms = new Dictionary<>(32); // Capacity for default policy
			All_allowed = false;
		}

		/// <summary>
		/// Adds a permission to the PropertyPermissions. The key for the hash is
		/// the name.
		/// </summary>
		/// <param name="permission"> the Permission object to add.
		/// </param>
		/// <exception cref="IllegalArgumentException"> - if the permission is not a
		///                                       PropertyPermission
		/// </exception>
		/// <exception cref="SecurityException"> - if this PropertyPermissionCollection
		///                                object has been marked readonly </exception>
		public override void Add(Permission permission)
		{
			if (!(permission is PropertyPermission))
			{
				throw new IllegalArgumentException("invalid permission: " + permission);
			}
			if (ReadOnly)
			{
				throw new SecurityException("attempt to add a Permission to a readonly PermissionCollection");
			}

			PropertyPermission pp = (PropertyPermission) permission;
			String propName = pp.Name;

			lock (this)
			{
				PropertyPermission existing = Perms[propName];

				if (existing != null)
				{
					int oldMask = existing.Mask;
					int newMask = pp.Mask;
					if (oldMask != newMask)
					{
						int effective = oldMask | newMask;
						String actions = PropertyPermission.GetActions(effective);
						Perms[propName] = new PropertyPermission(propName, actions);
					}
				}
				else
				{
					Perms[propName] = pp;
				}
			}

			if (!All_allowed)
			{
				if (propName.Equals("*"))
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
			if (!(permission is PropertyPermission))
			{
					return false;
			}

			PropertyPermission pp = (PropertyPermission) permission;
			PropertyPermission x;

			int desired = pp.Mask;
			int effective = 0;

			// short circuit if the "*" Permission was added
			if (All_allowed)
			{
				lock (this)
				{
					x = Perms["*"];
				}
				if (x != null)
				{
					effective |= x.Mask;
					if ((effective & desired) == desired)
					{
						return true;
					}
				}
			}

			// strategy:
			// Check for full match first. Then work our way up the
			// name looking for matches on a.b.*

			String name = pp.Name;
			//System.out.println("check "+name);

			lock (this)
			{
				x = Perms[name];
			}

			if (x != null)
			{
				// we have a direct hit!
				effective |= x.Mask;
				if ((effective & desired) == desired)
				{
					return true;
				}
			}

			// work our way up the tree...
			int last, offset;

			offset = name.Length() - 1;

			while ((last = name.LastIndexOf(".", offset)) != -1)
			{

				name = name.Substring(0, last + 1) + "*";
				//System.out.println("check "+name);
				lock (this)
				{
					x = Perms[name];
				}

				if (x != null)
				{
					effective |= x.Mask;
					if ((effective & desired) == desired)
					{
						return true;
					}
				}
				offset = last - 1;
			}

			// we don't have to check for "*" as it was already checked
			// at the top (all_allowed), so we just return false
			return false;
		}

		/// <summary>
		/// Returns an enumeration of all the PropertyPermission objects in the
		/// container.
		/// </summary>
		/// <returns> an enumeration of all the PropertyPermission objects. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<Permission> elements()
		public override IEnumerator<Permission> Elements()
		{
			// Convert Iterator of Map values into an Enumeration
			lock (this)
			{
				/// <summary>
				/// Casting to rawtype since Enumeration<PropertyPermission>
				/// cannot be directly cast to Enumeration<Permission>
				/// </summary>
				return (System.Collections.IEnumerator)Collections.Enumeration(Perms.Values);
			}
		}

		private const long SerialVersionUID = 7015263904581634791L;

		// Need to maintain serialization interoperability with earlier releases,
		// which had the serializable field:
		//
		// Table of permissions.
		//
		// @serial
		//
		// private Hashtable permissions;
		/// <summary>
		/// @serialField permissions java.util.Hashtable
		///     A table of the PropertyPermissions.
		/// @serialField all_allowed boolean
		///     boolean saying if "*" is in the collection.
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("permissions", typeof(Hashtable)), new ObjectStreamField("all_allowed", Boolean.TYPE)};

		/// <summary>
		/// @serialData Default fields.
		/// </summary>
		/*
		 * Writes the contents of the perms field out as a Hashtable for
		 * serialization compatibility with earlier releases. all_allowed
		 * unchanged.
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
				permissions.PutAll(Perms);
			}

			// Write out serializable fields
			ObjectOutputStream.PutField pfields = @out.PutFields();
			pfields.Put("all_allowed", All_allowed);
			pfields.Put("permissions", permissions);
			@out.WriteFields();
		}

		/*
		 * Reads in a Hashtable of PropertyPermissions and saves them in the
		 * perms field. Reads in all_allowed.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()

			// Read in serialized fields
			ObjectInputStream.GetField gfields = @in.ReadFields();

			// Get all_allowed
			All_allowed = gfields.Get("all_allowed", false);

			// Get permissions
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Hashtable<String, PropertyPermission> permissions = (java.util.Hashtable<String, PropertyPermission>)gfields.get("permissions", null);
			Dictionary<String, PropertyPermission> permissions = (Dictionary<String, PropertyPermission>)gfields.Get("permissions", null);
			Perms = new Dictionary<>(permissions.Size() * 2);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			Perms.putAll(permissions);
		}
	}

}