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
	/// Unchecked exception thrown when an attempt is made to write to an
	/// asynchronous socket channel and a previous write has not completed.
	/// 
	/// @since 1.7
	/// </summary>

	public class WritePendingException : IllegalStateException
	{

		private new const long SerialVersionUID = 7031871839266032276L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public WritePendingException()
		{
		}

	}

}