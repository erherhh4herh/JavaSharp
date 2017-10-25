/*
 * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
 *
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
 *
 */

// -- This file was mechanically generated: Do not edit! -- //

namespace java.nio
{


	/// <summary>
	/// Unchecked exception thrown when a content-mutation method such as
	/// <tt>put</tt> or <tt>compact</tt> is invoked upon a read-only buffer.
	/// 
	/// @since 1.4
	/// </summary>

	public class ReadOnlyBufferException : UnsupportedOperationException
	{

		private new const long SerialVersionUID = -1210063976496234090L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ReadOnlyBufferException()
		{
		}

	}

}