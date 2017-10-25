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
	/// Runtime exception thrown when an attempt is made to create a file system that
	/// already exists.
	/// </summary>

	public class FileSystemAlreadyExistsException : RuntimeException
	{
		internal new const long SerialVersionUID = -5438419127181131148L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public FileSystemAlreadyExistsException()
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="msg">
		///          the detail message </param>
		public FileSystemAlreadyExistsException(String msg) : base(msg)
		{
		}
	}

}