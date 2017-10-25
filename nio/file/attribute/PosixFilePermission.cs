using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the bits for use with the {@link PosixFileAttributes#permissions()
	/// permissions} attribute.
	/// 
	/// <para> The <seealso cref="PosixFilePermissions"/> class defines methods for manipulating
	/// set of permissions.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public sealed class PosixFilePermission
	{

		/// <summary>
		/// Read permission, owner.
		/// </summary>
		OWNER_READ,
		public static readonly PosixFilePermission OWNER_READ = new PosixFilePermission("OWNER_READ", InnerEnum.OWNER_READ);

		/// <summary>
		/// Write permission, owner.
		/// </summary>
		OWNER_WRITE,
		public static readonly PosixFilePermission OWNER_WRITE = new PosixFilePermission("OWNER_WRITE", InnerEnum.OWNER_WRITE);

		/// <summary>
		/// Execute/search permission, owner.
		/// </summary>
		OWNER_EXECUTE,
		public static readonly PosixFilePermission OWNER_EXECUTE = new PosixFilePermission("OWNER_EXECUTE", InnerEnum.OWNER_EXECUTE);

		/// <summary>
		/// Read permission, group.
		/// </summary>
		GROUP_READ,
		public static readonly PosixFilePermission GROUP_READ = new PosixFilePermission("GROUP_READ", InnerEnum.GROUP_READ);

		/// <summary>
		/// Write permission, group.
		/// </summary>
		GROUP_WRITE,
		public static readonly PosixFilePermission GROUP_WRITE = new PosixFilePermission("GROUP_WRITE", InnerEnum.GROUP_WRITE);

		/// <summary>
		/// Execute/search permission, group.
		/// </summary>
		GROUP_EXECUTE,
		public static readonly PosixFilePermission GROUP_EXECUTE = new PosixFilePermission("GROUP_EXECUTE", InnerEnum.GROUP_EXECUTE);

		/// <summary>
		/// Read permission, others.
		/// </summary>
		OTHERS_READ,
		public static readonly PosixFilePermission OTHERS_READ = new PosixFilePermission("OTHERS_READ", InnerEnum.OTHERS_READ);

		/// <summary>
		/// Write permission, others.
		/// </summary>
		OTHERS_WRITE,
		public static readonly PosixFilePermission OTHERS_WRITE = new PosixFilePermission("OTHERS_WRITE", InnerEnum.OTHERS_WRITE);

		/// <summary>
		/// Execute/search permission, others.
		/// </summary>
		OTHERS_EXECUTE
		public static readonly PosixFilePermission OTHERS_EXECUTE = new PosixFilePermission("OTHERS_EXECUTE", InnerEnum.OTHERS_EXECUTE);

		private static readonly IList<PosixFilePermission> valueList = new List<PosixFilePermission>();

		static PosixFilePermission()
		{
			valueList.Add(OWNER_READ);
			valueList.Add(OWNER_WRITE);
			valueList.Add(OWNER_EXECUTE);
			valueList.Add(GROUP_READ);
			valueList.Add(GROUP_WRITE);
			valueList.Add(GROUP_EXECUTE);
			valueList.Add(OTHERS_READ);
			valueList.Add(OTHERS_WRITE);
			valueList.Add(OTHERS_EXECUTE);
		}

		public enum InnerEnum
		{
			OWNER_READ,
			OWNER_WRITE,
			OWNER_EXECUTE,
			GROUP_READ,
			GROUP_WRITE,
			GROUP_EXECUTE,
			OTHERS_READ,
			OTHERS_WRITE,
			OTHERS_EXECUTE
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<PosixFilePermission> values()
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

		public static PosixFilePermission valueOf(string name)
		{
			foreach (PosixFilePermission enumInstance in PosixFilePermission.values())
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