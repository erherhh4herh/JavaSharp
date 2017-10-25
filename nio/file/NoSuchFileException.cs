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
	/// Checked exception thrown when an attempt is made to access a file that does
	/// not exist.
	/// 
	/// @since 1.7
	/// </summary>

	public class NoSuchFileException : FileSystemException
	{
		internal new const long SerialVersionUID = -1390291775875351931L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known. </param>
		public NoSuchFileException(String file) : base(file)
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="file">
		///          a string identifying the file or {@code null} if not known. </param>
		/// <param name="other">
		///          a string identifying the other file or {@code null} if not known. </param>
		/// <param name="reason">
		///          a reason message with additional information or {@code null} </param>
		public NoSuchFileException(String file, String other, String reason) : base(file, other, reason)
		{
		}
	}

}