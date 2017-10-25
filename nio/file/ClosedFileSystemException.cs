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
	/// Unchecked exception thrown when an attempt is made to invoke an operation on
	/// a file and the file system is closed.
	/// </summary>

	public class ClosedFileSystemException : IllegalStateException
	{
		internal new const long SerialVersionUID = -8158336077256193488L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ClosedFileSystemException()
		{
		}
	}

}