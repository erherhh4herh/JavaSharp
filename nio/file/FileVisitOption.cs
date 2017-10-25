using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the file tree traversal options.
	/// 
	/// @since 1.7
	/// </summary>
	/// <seealso cref= Files#walkFileTree </seealso>

	public sealed class FileVisitOption
	{
		/// <summary>
		/// Follow symbolic links.
		/// </summary>
		FOLLOW_LINKS
		public static readonly FileVisitOption FOLLOW_LINKS = new FileVisitOption("FOLLOW_LINKS", InnerEnum.FOLLOW_LINKS);

		private static readonly IList<FileVisitOption> valueList = new List<FileVisitOption>();

		static FileVisitOption()
		{
			valueList.Add(FOLLOW_LINKS);
		}

		public enum InnerEnum
		{
			FOLLOW_LINKS
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<FileVisitOption> values()
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

		public static FileVisitOption valueOf(string name)
		{
			foreach (FileVisitOption enumInstance in FileVisitOption.values())
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