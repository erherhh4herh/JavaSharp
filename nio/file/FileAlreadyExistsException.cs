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
	/// Checked exception thrown when an attempt is made to create a file or
	/// directory and a file of that name already exists.
	/// 
	/// @since 1.7
	/// </summary>

	public class FileAlreadyExistsException : FileSystemException
	{
		internal new const long SerialVersionUID = 7579540934498831181L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known </param>
		public FileAlreadyExistsException(String file) : base(file)
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known </param>
		/// <param name="other">
		///          a string identifying the other file or {@code null} if not known </param>
		/// <param name="reason">
		///          a reason message with additional information or {@code null} </param>
		public FileAlreadyExistsException(String file, String other, String reason) : base(file, other, reason)
		{
		}
	}

}