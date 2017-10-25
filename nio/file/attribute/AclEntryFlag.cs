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
	/// Defines the flags for used by the flags component of an ACL {@link AclEntry
	/// entry}.
	/// 
	/// <para> In this release, this class does not define flags related to {@link
	/// AclEntryType#AUDIT} and <seealso cref="AclEntryType#ALARM"/> entry types.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public sealed class AclEntryFlag
	{

		/// <summary>
		/// Can be placed on a directory and indicates that the ACL entry should be
		/// added to each new non-directory file created.
		/// </summary>
		FILE_INHERIT,
		public static readonly AclEntryFlag FILE_INHERIT = new AclEntryFlag("FILE_INHERIT", InnerEnum.FILE_INHERIT);

		/// <summary>
		/// Can be placed on a directory and indicates that the ACL entry should be
		/// added to each new directory created.
		/// </summary>
		DIRECTORY_INHERIT,
		public static readonly AclEntryFlag DIRECTORY_INHERIT = new AclEntryFlag("DIRECTORY_INHERIT", InnerEnum.DIRECTORY_INHERIT);

		/// <summary>
		/// Can be placed on a directory to indicate that the ACL entry should not
		/// be placed on the newly created directory which is inheritable by
		/// subdirectories of the created directory.
		/// </summary>
		NO_PROPAGATE_INHERIT,
		public static readonly AclEntryFlag NO_PROPAGATE_INHERIT = new AclEntryFlag("NO_PROPAGATE_INHERIT", InnerEnum.NO_PROPAGATE_INHERIT);

		/// <summary>
		/// Can be placed on a directory but does not apply to the directory,
		/// only to newly created files/directories as specified by the
		/// <seealso cref="#FILE_INHERIT"/> and <seealso cref="#DIRECTORY_INHERIT"/> flags.
		/// </summary>
		INHERIT_ONLY
		public static readonly AclEntryFlag INHERIT_ONLY = new AclEntryFlag("INHERIT_ONLY", InnerEnum.INHERIT_ONLY);

		private static readonly IList<AclEntryFlag> valueList = new List<AclEntryFlag>();

		static AclEntryFlag()
		{
			valueList.Add(FILE_INHERIT);
			valueList.Add(DIRECTORY_INHERIT);
			valueList.Add(NO_PROPAGATE_INHERIT);
			valueList.Add(INHERIT_ONLY);
		}

		public enum InnerEnum
		{
			FILE_INHERIT,
			DIRECTORY_INHERIT,
			NO_PROPAGATE_INHERIT,
			INHERIT_ONLY
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<AclEntryFlag> values()
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

		public static AclEntryFlag valueOf(string name)
		{
			foreach (AclEntryFlag enumInstance in AclEntryFlag.values())
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