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
	/// Checked exception thrown when a file system operation fails because a
	/// directory is not empty.
	/// 
	/// @since 1.7
	/// </summary>

	public class DirectoryNotEmptyException : FileSystemException
	{
		internal new const long SerialVersionUID = 3056667871802779003L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="dir">
		///          a string identifying the directory or {@code null} if not known </param>
		public DirectoryNotEmptyException(String dir) : base(dir)
		{
		}
	}

}