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
	/// Checked exception thrown when a file system operation fails because a file
	/// is not a symbolic link.
	/// 
	/// @since 1.7
	/// </summary>

	public class NotLinkException : FileSystemException
	{
		internal new const long SerialVersionUID = -388655596416518021L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known </param>
		public NotLinkException(String file) : base(file)
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
		public NotLinkException(String file, String other, String reason) : base(file, other, reason)
		{
		}
	}

}