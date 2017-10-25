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
	/// Unchecked exception thrown when an attempt is made to update an object
	/// associated with a <seealso cref="FileSystem#isReadOnly() read-only"/> {@code FileSystem}.
	/// </summary>

	public class ReadOnlyFileSystemException : UnsupportedOperationException
	{
		internal new const long SerialVersionUID = -6822409595617487197L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ReadOnlyFileSystemException()
		{
		}
	}

}