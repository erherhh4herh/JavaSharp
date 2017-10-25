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

namespace java.nio.channels
{


	/// <summary>
	/// Checked exception thrown when an attempt is made to invoke or complete an
	/// I/O operation upon channel that is closed, or at least closed to that
	/// operation.  That this exception is thrown does not necessarily imply that
	/// the channel is completely closed.  A socket channel whose write half has
	/// been shut down, for example, may still be open for reading.
	/// 
	/// @since 1.4
	/// </summary>

	public class ClosedChannelException : java.io.IOException
	{

		private new const long SerialVersionUID = 882777185433553857L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ClosedChannelException()
		{
		}

	}

}