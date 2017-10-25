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

namespace java.nio.file
{

	/// <summary>
	/// Defines access modes used to test the accessibility of a file.
	/// 
	/// @since 1.7
	/// </summary>

	public sealed class AccessMode
	{
		/// <summary>
		/// Test read access.
		/// </summary>
		READ,
		public static readonly AccessMode READ = new AccessMode("READ", InnerEnum.READ);
		/// <summary>
		/// Test write access.
		/// </summary>
		WRITE,
		public static readonly AccessMode WRITE = new AccessMode("WRITE", InnerEnum.WRITE);
		/// <summary>
		/// Test execute access.
		/// </summary>
		EXECUTE
		public static readonly AccessMode EXECUTE = new AccessMode("EXECUTE", InnerEnum.EXECUTE);

		private static readonly IList<AccessMode> valueList = new List<AccessMode>();

		static AccessMode()
		{
			valueList.Add(READ);
			valueList.Add(WRITE);
			valueList.Add(EXECUTE);
		}

		public enum InnerEnum
		{
			READ,
			WRITE,
			EXECUTE
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<AccessMode> values()
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

		public static AccessMode valueOf(string name)
		{
			foreach (AccessMode enumInstance in AccessMode.values())
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