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
	/// a directory stream that is closed.
	/// 
	/// @since 1.7
	/// </summary>

	public class ClosedDirectoryStreamException : IllegalStateException
	{
		internal new const long SerialVersionUID = 4228386650900895400L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ClosedDirectoryStreamException()
		{
		}
	}

}