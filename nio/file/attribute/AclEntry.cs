using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.attribute
{

	/// <summary>
	/// An entry in an access control list (ACL).
	/// 
	/// <para> The ACL entry represented by this class is based on the ACL model
	/// specified in <a href="http://www.ietf.org/rfc/rfc3530.txt"><i>RFC&nbsp;3530:
	/// Network File System (NFS) version 4 Protocol</i></a>. Each entry has four
	/// components as follows:
	/// 
	/// <ol>
	/// </para>
	///    <li><para> The <seealso cref="#type() type"/> component determines if the entry
	///    grants or denies access. </para></li>
	/// 
	///    <li><para> The <seealso cref="#principal() principal"/> component, sometimes called the
	///    "who" component, is a <seealso cref="UserPrincipal"/> corresponding to the identity
	///    that the entry grants or denies access
	///    </para></li>
	/// 
	///    <li><para> The <seealso cref="#permissions permissions"/> component is a set of
	///    <seealso cref="AclEntryPermission permissions"/>
	///    </para></li>
	/// 
	///    <li><para> The <seealso cref="#flags flags"/> component is a set of {@link AclEntryFlag
	///    flags} to indicate how entries are inherited and propagated </para></li>
	/// </ol>
	/// 
	/// <para> ACL entries are created using an associated <seealso cref="Builder"/> object by
	/// invoking its <seealso cref="Builder#build build"/> method.
	/// 
	/// </para>
	/// <para> ACL entries are immutable and are safe for use by multiple concurrent
	/// threads.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public sealed class AclEntry
	{

		private readonly AclEntryType Type_Renamed;
		private readonly UserPrincipal Who;
		private readonly Set<AclEntryPermission> Perms;
		private readonly Set<AclEntryFlag> Flags_Renamed;

		// cached hash code
		private volatile int Hash_Renamed;

		// private constructor
		private AclEntry(AclEntryType type, UserPrincipal who, Set<AclEntryPermission> perms, Set<AclEntryFlag> flags)
		{
			this.Type_Renamed = type;
			this.Who = who;
			this.Perms = perms;
			this.Flags_Renamed = flags;
		}

		/// <summary>
		/// A builder of <seealso cref="AclEntry"/> objects.
		/// 
		/// <para> A {@code Builder} object is obtained by invoking one of the {@link
		/// AclEntry#newBuilder newBuilder} methods defined by the {@code AclEntry}
		/// class.
		/// 
		/// </para>
		/// <para> Builder objects are mutable and are not safe for use by multiple
		/// concurrent threads without appropriate synchronization.
		/// 
		/// @since 1.7
		/// </para>
		/// </summary>
		public sealed class Builder
		{
			internal AclEntryType Type_Renamed;
			internal UserPrincipal Who;
			internal Set<AclEntryPermission> Perms;
			internal Set<AclEntryFlag> Flags_Renamed;

			internal Builder(AclEntryType type, UserPrincipal who, Set<AclEntryPermission> perms, Set<AclEntryFlag> flags)
			{
				Debug.Assert(perms != null && flags != null);
				this.Type_Renamed = type;
				this.Who = who;
				this.Perms = perms;
				this.Flags_Renamed = flags;
			}

			/// <summary>
			/// Constructs an <seealso cref="AclEntry"/> from the components of this builder.
			/// The type and who components are required to have been set in order
			/// to construct an {@code AclEntry}.
			/// </summary>
			/// <returns>  a new ACL entry
			/// </returns>
			/// <exception cref="IllegalStateException">
			///          if the type or who component have not been set </exception>
			public AclEntry Build()
			{
				if (Type_Renamed == null)
				{
					throw new IllegalStateException("Missing type component");
				}
				if (Who == null)
				{
					throw new IllegalStateException("Missing who component");
				}
				return new AclEntry(Type_Renamed, Who, Perms, Flags_Renamed);
			}

			/// <summary>
			/// Sets the type component of this builder.
			/// </summary>
			/// <param name="type">  the component type </param>
			/// <returns>  this builder </returns>
			public Builder SetType(AclEntryType type)
			{
				if (type == null)
				{
					throw new NullPointerException();
				}
				this.Type_Renamed = type;
				return this;
			}

			/// <summary>
			/// Sets the principal component of this builder.
			/// </summary>
			/// <param name="who">  the principal component </param>
			/// <returns>  this builder </returns>
			public Builder SetPrincipal(UserPrincipal who)
			{
				if (who == null)
				{
					throw new NullPointerException();
				}
				this.Who = who;
				return this;
			}

			// check set only contains elements of the given type
			internal static void checkSet<T1>(Set<T1> set, Class type)
			{
				foreach (Object e in set)
				{
					if (e == null)
					{
						throw new NullPointerException();
					}
					type.Cast(e);
				}
			}

			/// <summary>
			/// Sets the permissions component of this builder. On return, the
			/// permissions component of this builder is a copy of the given set.
			/// </summary>
			/// <param name="perms">  the permissions component </param>
			/// <returns>  this builder
			/// </returns>
			/// <exception cref="ClassCastException">
			///          if the set contains elements that are not of type {@code
			///          AclEntryPermission} </exception>
			public Builder SetPermissions(Set<AclEntryPermission> perms)
			{
				if (perms.Count == 0)
				{
					// EnumSet.copyOf does not allow empty set
					perms = java.util.Collections.EmptySet();
				}
				else
				{
					// copy and check for erroneous elements
					perms = EnumSet.CopyOf(perms);
					CheckSet(perms, typeof(AclEntryPermission));
				}

				this.Perms = perms;
				return this;
			}

			/// <summary>
			/// Sets the permissions component of this builder. On return, the
			/// permissions component of this builder is a copy of the permissions in
			/// the given array.
			/// </summary>
			/// <param name="perms">  the permissions component </param>
			/// <returns>  this builder </returns>
			public Builder SetPermissions(params AclEntryPermission[] perms)
			{
				Set<AclEntryPermission> set = EnumSet.NoneOf(typeof(AclEntryPermission));
				// copy and check for null elements
				foreach (AclEntryPermission p in perms)
				{
					if (p == null)
					{
						throw new NullPointerException();
					}
					set.Add(p);
				}
				this.Perms = set;
				return this;
			}

			/// <summary>
			/// Sets the flags component of this builder. On return, the flags
			/// component of this builder is a copy of the given set.
			/// </summary>
			/// <param name="flags">  the flags component </param>
			/// <returns>  this builder
			/// </returns>
			/// <exception cref="ClassCastException">
			///          if the set contains elements that are not of type {@code
			///          AclEntryFlag} </exception>
			public Builder SetFlags(Set<AclEntryFlag> flags)
			{
				if (flags.Count == 0)
				{
					// EnumSet.copyOf does not allow empty set
					flags = java.util.Collections.EmptySet();
				}
				else
				{
					// copy and check for erroneous elements
					flags = EnumSet.CopyOf(flags);
					CheckSet(flags, typeof(AclEntryFlag));
				}

				this.Flags_Renamed = flags;
				return this;
			}

			/// <summary>
			/// Sets the flags component of this builder. On return, the flags
			/// component of this builder is a copy of the flags in the given
			/// array.
			/// </summary>
			/// <param name="flags">  the flags component </param>
			/// <returns>  this builder </returns>
			public Builder SetFlags(params AclEntryFlag[] flags)
			{
				Set<AclEntryFlag> set = EnumSet.NoneOf(typeof(AclEntryFlag));
				// copy and check for null elements
				foreach (AclEntryFlag f in flags)
				{
					if (f == null)
					{
						throw new NullPointerException();
					}
					set.Add(f);
				}
				this.Flags_Renamed = set;
				return this;
			}
		}

		/// <summary>
		/// Constructs a new builder. The initial value of the type and who
		/// components is {@code null}. The initial value of the permissions and
		/// flags components is the empty set.
		/// </summary>
		/// <returns>  a new builder </returns>
		public static Builder NewBuilder()
		{
			Set<AclEntryPermission> perms = java.util.Collections.EmptySet();
			Set<AclEntryFlag> flags = java.util.Collections.EmptySet();
			return new Builder(null, null, perms, flags);
		}

		/// <summary>
		/// Constructs a new builder with the components of an existing ACL entry.
		/// </summary>
		/// <param name="entry">  an ACL entry </param>
		/// <returns>  a new builder </returns>
		public static Builder NewBuilder(AclEntry entry)
		{
			return new Builder(entry.Type_Renamed, entry.Who, entry.Perms, entry.Flags_Renamed);
		}

		/// <summary>
		/// Returns the ACL entry type.
		/// </summary>
		/// <returns> the ACL entry type </returns>
		public AclEntryType Type()
		{
			return Type_Renamed;
		}

		/// <summary>
		/// Returns the principal component.
		/// </summary>
		/// <returns> the principal component </returns>
		public UserPrincipal Principal()
		{
			return Who;
		}

		/// <summary>
		/// Returns a copy of the permissions component.
		/// 
		/// <para> The returned set is a modifiable copy of the permissions.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the permissions component </returns>
		public Set<AclEntryPermission> Permissions()
		{
			return new HashSet<AclEntryPermission>(Perms);
		}

		/// <summary>
		/// Returns a copy of the flags component.
		/// 
		/// <para> The returned set is a modifiable copy of the flags.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the flags component </returns>
		public Set<AclEntryFlag> Flags()
		{
			return new HashSet<AclEntryFlag>(Flags_Renamed);
		}

		/// <summary>
		/// Compares the specified object with this ACL entry for equality.
		/// 
		/// <para> If the given object is not an {@code AclEntry} then this method
		/// immediately returns {@code false}.
		/// 
		/// </para>
		/// <para> For two ACL entries to be considered equals requires that they are
		/// both the same type, their who components are equal, their permissions
		/// components are equal, and their flags components are equal.
		/// 
		/// </para>
		/// <para> This method satisfies the general contract of the {@link
		/// java.lang.Object#equals(Object) Object.equals} method. </para>
		/// </summary>
		/// <param name="ob">   the object to which this object is to be compared
		/// </param>
		/// <returns>  {@code true} if, and only if, the given object is an AclEntry that
		///          is identical to this AclEntry </returns>
		public override bool Equals(Object ob)
		{
			if (ob == this)
			{
				return true;
			}
			if (ob == null || !(ob is AclEntry))
			{
				return false;
			}
			AclEntry other = (AclEntry)ob;
			if (this.Type_Renamed != other.Type_Renamed)
			{
				return false;
			}
			if (!this.Who.Equals(other.Who))
			{
				return false;
			}
			if (!this.Perms.Equals(other.Perms))
			{
				return false;
			}
			if (!this.Flags_Renamed.Equals(other.Flags_Renamed))
			{
				return false;
			}
			return true;
		}

		private static int Hash(int h, Object o)
		{
			return h * 127 + o.HashCode();
		}

		/// <summary>
		/// Returns the hash-code value for this ACL entry.
		/// 
		/// <para> This method satisfies the general contract of the {@link
		/// Object#hashCode} method.
		/// </para>
		/// </summary>
		public override int HashCode()
		{
			// return cached hash if available
			if (Hash_Renamed != 0)
			{
				return Hash_Renamed;
			}
			int h = Type_Renamed.HashCode();
			h = Hash(h, Who);
			h = Hash(h, Perms);
			h = Hash(h, Flags_Renamed);
			Hash_Renamed = h;
			return Hash_Renamed;
		}

		/// <summary>
		/// Returns the string representation of this ACL entry.
		/// </summary>
		/// <returns>  the string representation of this entry </returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();

			// who
			sb.Append(Who.Name);
			sb.Append(':');

			// permissions
			foreach (AclEntryPermission perm in Perms)
			{
				sb.Append(perm.name());
				sb.Append('/');
			}
			sb.Length = sb.Length() - 1; // drop final slash
			sb.Append(':');

			// flags
			if (Flags_Renamed.Count > 0)
			{
				foreach (AclEntryFlag flag in Flags_Renamed)
				{
					sb.Append(flag.name());
					sb.Append('/');
				}
				sb.Length = sb.Length() - 1; // drop final slash
				sb.Append(':');
			}

			// type
			sb.Append(Type_Renamed.name());
			return sb.ToString();
		}
	}

}