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
	/// Checked exception received by a thread when another thread interrupts it
	/// while it is blocked in an I/O operation upon a channel.  Before this
	/// exception is thrown the channel will have been closed and the interrupt
	/// status of the previously-blocked thread will have been set.
	/// 
	/// @since 1.4
	/// </summary>

	public class ClosedByInterruptException : AsynchronousCloseException
	{

		private new const long SerialVersionUID = -4488191543534286750L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ClosedByInterruptException()
		{
		}

	}

}