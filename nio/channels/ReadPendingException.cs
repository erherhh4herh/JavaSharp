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
	/// Unchecked exception thrown when an attempt is made to read from an
	/// asynchronous socket channel and a previous read has not completed.
	/// 
	/// @since 1.7
	/// </summary>

	public class ReadPendingException : IllegalStateException
	{

		private new const long SerialVersionUID = 1986315242191227217L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ReadPendingException()
		{
		}

	}

}