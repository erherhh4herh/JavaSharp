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
	/// Checked exception thrown when a file system operation is denied, typically
	/// due to a file permission or other access check.
	/// 
	/// <para> This exception is not related to the {@link
	/// java.security.AccessControlException AccessControlException} or {@link
	/// SecurityException} thrown by access controllers or security managers when
	/// access to a file is denied.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public class AccessDeniedException : FileSystemException
	{
		private new const long SerialVersionUID = 4943049599949219617L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known </param>
		public AccessDeniedException(String file) : base(file)
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
		public AccessDeniedException(String file, String other, String reason) : base(file, other, reason)
		{
		}
	}

}