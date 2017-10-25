using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the permissions for use with the permissions component of an ACL
	/// <seealso cref="AclEntry entry"/>.
	/// 
	/// @since 1.7
	/// </summary>

	public sealed class AclEntryPermission
	{

		/// <summary>
		/// Permission to read the data of the file.
		/// </summary>
		READ_DATA,
		public static readonly AclEntryPermission READ_DATA = new AclEntryPermission("READ_DATA", InnerEnum.READ_DATA);

		/// <summary>
		/// Permission to modify the file's data.
		/// </summary>
		WRITE_DATA,
		public static readonly AclEntryPermission WRITE_DATA = new AclEntryPermission("WRITE_DATA", InnerEnum.WRITE_DATA);

		/// <summary>
		/// Permission to append data to a file.
		/// </summary>
		APPEND_DATA,
		public static readonly AclEntryPermission APPEND_DATA = new AclEntryPermission("APPEND_DATA", InnerEnum.APPEND_DATA);

		/// <summary>
		/// Permission to read the named attributes of a file.
		/// 
		/// <para> <a href="http://www.ietf.org/rfc/rfc3530.txt">RFC&nbsp;3530: Network
		/// File System (NFS) version 4 Protocol</a> defines <em>named attributes</em>
		/// as opaque files associated with a file in the file system.
		/// </para>
		/// </summary>
		READ_NAMED_ATTRS,
		public static readonly AclEntryPermission READ_NAMED_ATTRS = new AclEntryPermission("READ_NAMED_ATTRS", InnerEnum.READ_NAMED_ATTRS);

		/// <summary>
		/// Permission to write the named attributes of a file.
		/// 
		/// <para> <a href="http://www.ietf.org/rfc/rfc3530.txt">RFC&nbsp;3530: Network
		/// File System (NFS) version 4 Protocol</a> defines <em>named attributes</em>
		/// as opaque files associated with a file in the file system.
		/// </para>
		/// </summary>
		WRITE_NAMED_ATTRS,
		public static readonly AclEntryPermission WRITE_NAMED_ATTRS = new AclEntryPermission("WRITE_NAMED_ATTRS", InnerEnum.WRITE_NAMED_ATTRS);

		/// <summary>
		/// Permission to execute a file.
		/// </summary>
		EXECUTE,
		public static readonly AclEntryPermission EXECUTE = new AclEntryPermission("EXECUTE", InnerEnum.EXECUTE);

		/// <summary>
		/// Permission to delete a file or directory within a directory.
		/// </summary>
		DELETE_CHILD,
		public static readonly AclEntryPermission DELETE_CHILD = new AclEntryPermission("DELETE_CHILD", InnerEnum.DELETE_CHILD);

		/// <summary>
		/// The ability to read (non-acl) file attributes.
		/// </summary>
		READ_ATTRIBUTES,
		public static readonly AclEntryPermission READ_ATTRIBUTES = new AclEntryPermission("READ_ATTRIBUTES", InnerEnum.READ_ATTRIBUTES);

		/// <summary>
		/// The ability to write (non-acl) file attributes.
		/// </summary>
		WRITE_ATTRIBUTES,
		public static readonly AclEntryPermission WRITE_ATTRIBUTES = new AclEntryPermission("WRITE_ATTRIBUTES", InnerEnum.WRITE_ATTRIBUTES);

		/// <summary>
		/// Permission to delete the file.
		/// </summary>
		DELETE,
		public static readonly AclEntryPermission DELETE = new AclEntryPermission("DELETE", InnerEnum.DELETE);

		/// <summary>
		/// Permission to read the ACL attribute.
		/// </summary>
		READ_ACL,
		public static readonly AclEntryPermission READ_ACL = new AclEntryPermission("READ_ACL", InnerEnum.READ_ACL);

		/// <summary>
		/// Permission to write the ACL attribute.
		/// </summary>
		WRITE_ACL,
		public static readonly AclEntryPermission WRITE_ACL = new AclEntryPermission("WRITE_ACL", InnerEnum.WRITE_ACL);

		/// <summary>
		/// Permission to change the owner.
		/// </summary>
		WRITE_OWNER,
		public static readonly AclEntryPermission WRITE_OWNER = new AclEntryPermission("WRITE_OWNER", InnerEnum.WRITE_OWNER);

		/// <summary>
		/// Permission to access file locally at the server with synchronous reads
		/// and writes.
		/// </summary>
		SYNCHRONIZE
		public static readonly AclEntryPermission SYNCHRONIZE = new AclEntryPermission("SYNCHRONIZE", InnerEnum.SYNCHRONIZE);

		private static readonly IList<AclEntryPermission> valueList = new List<AclEntryPermission>();

		static AclEntryPermission()
		{
			valueList.Add(READ_DATA);
			valueList.Add(WRITE_DATA);
			valueList.Add(APPEND_DATA);
			valueList.Add(READ_NAMED_ATTRS);
			valueList.Add(WRITE_NAMED_ATTRS);
			valueList.Add(EXECUTE);
			valueList.Add(DELETE_CHILD);
			valueList.Add(READ_ATTRIBUTES);
			valueList.Add(WRITE_ATTRIBUTES);
			valueList.Add(DELETE);
			valueList.Add(READ_ACL);
			valueList.Add(WRITE_ACL);
			valueList.Add(WRITE_OWNER);
			valueList.Add(SYNCHRONIZE);
		}

		public enum InnerEnum
		{
			READ_DATA,
			WRITE_DATA,
			APPEND_DATA,
			READ_NAMED_ATTRS,
			WRITE_NAMED_ATTRS,
			EXECUTE,
			DELETE_CHILD,
			READ_ATTRIBUTES,
			WRITE_ATTRIBUTES,
			DELETE,
			READ_ACL,
			WRITE_ACL,
			WRITE_OWNER,
			SYNCHRONIZE
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		/// <summary>
		/// Permission to list the entries of a directory (equal to <seealso cref="#READ_DATA"/>)
		/// </summary>
		public static readonly AclEntryPermission LIST_DIRECTORY = READ_DATA;

		/// <summary>
		/// Permission to add a new file to a directory (equal to <seealso cref="#WRITE_DATA"/>)
		/// </summary>
		public static readonly AclEntryPermission ADD_FILE = WRITE_DATA;

		/// <summary>
		/// Permission to create a subdirectory to a directory (equal to <seealso cref="#APPEND_DATA"/>)
		/// </summary>
		public static readonly AclEntryPermission ADD_SUBDIRECTORY = APPEND_DATA;

		public static IList<AclEntryPermission> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static AclEntryPermission valueOf(string name)
		{
			foreach (AclEntryPermission enumInstance in AclEntryPermission.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}