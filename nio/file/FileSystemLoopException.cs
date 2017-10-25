/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// Checked exception thrown when a file system loop, or cycle, is encountered.
	/// 
	/// @since 1.7 </summary>
	/// <seealso cref= Files#walkFileTree </seealso>

	public class FileSystemLoopException : FileSystemException
	{
		private new const long SerialVersionUID = 4843039591949217617L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file causing the cycle or {@code null} if
		///          not known </param>
		public FileSystemLoopException(String file) : base(file)
		{
		}
	}

}