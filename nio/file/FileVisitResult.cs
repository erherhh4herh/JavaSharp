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

namespace java.nio.file
{

	/// <summary>
	/// The result type of a <seealso cref="FileVisitor FileVisitor"/>.
	/// 
	/// @since 1.7
	/// </summary>
	/// <seealso cref= Files#walkFileTree </seealso>

	public sealed class FileVisitResult
	{
		/// <summary>
		/// Continue. When returned from a {@link FileVisitor#preVisitDirectory
		/// preVisitDirectory} method then the entries in the directory should also
		/// be visited.
		/// </summary>
		CONTINUE,
		public static readonly FileVisitResult CONTINUE = new FileVisitResult("CONTINUE", InnerEnum.CONTINUE);
		/// <summary>
		/// Terminate.
		/// </summary>
		TERMINATE,
		public static readonly FileVisitResult TERMINATE = new FileVisitResult("TERMINATE", InnerEnum.TERMINATE);
		/// <summary>
		/// Continue without visiting the entries in this directory. This result
		/// is only meaningful when returned from the {@link
		/// FileVisitor#preVisitDirectory preVisitDirectory} method; otherwise
		/// this result type is the same as returning <seealso cref="#CONTINUE"/>.
		/// </summary>
		SKIP_SUBTREE,
		public static readonly FileVisitResult SKIP_SUBTREE = new FileVisitResult("SKIP_SUBTREE", InnerEnum.SKIP_SUBTREE);
		/// <summary>
		/// Continue without visiting the <em>siblings</em> of this file or directory.
		/// If returned from the {@link FileVisitor#preVisitDirectory
		/// preVisitDirectory} method then the entries in the directory are also
		/// skipped and the <seealso cref="FileVisitor#postVisitDirectory postVisitDirectory"/>
		/// method is not invoked.
		/// </summary>
		SKIP_SIBLINGS
		public static readonly FileVisitResult SKIP_SIBLINGS = new FileVisitResult("SKIP_SIBLINGS", InnerEnum.SKIP_SIBLINGS);

		private static readonly IList<FileVisitResult> valueList = new List<FileVisitResult>();

		static FileVisitResult()
		{
			valueList.Add(CONTINUE);
			valueList.Add(TERMINATE);
			valueList.Add(SKIP_SUBTREE);
			valueList.Add(SKIP_SIBLINGS);
		}

		public enum InnerEnum
		{
			CONTINUE,
			TERMINATE,
			SKIP_SUBTREE,
			SKIP_SIBLINGS
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<FileVisitResult> values()
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

		public static FileVisitResult valueOf(string name)
		{
			foreach (FileVisitResult enumInstance in FileVisitResult.values())
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