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
	/// Runtime exception thrown when a file system cannot be found.
	/// </summary>

	public class FileSystemNotFoundException : RuntimeException
	{
		internal new const long SerialVersionUID = 7999581764446402397L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public FileSystemNotFoundException()
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="msg">
		///          the detail message </param>
		public FileSystemNotFoundException(String msg) : base(msg)
		{
		}
	}

}