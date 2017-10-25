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
	/// Checked exception thrown when a file cannot be moved as an atomic file system
	/// operation.
	/// 
	/// @since 1.7
	/// </summary>

	public class AtomicMoveNotSupportedException : FileSystemException
	{
		internal new const long SerialVersionUID = 5402760225333135579L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="source">
		///          a string identifying the source file or {@code null} if not known </param>
		/// <param name="target">
		///          a string identifying the target file or {@code null} if not known </param>
		/// <param name="reason">
		///          a reason message with additional information </param>
		public AtomicMoveNotSupportedException(String source, String target, String reason) : base(source, target, reason)
		{
		}
	}

}