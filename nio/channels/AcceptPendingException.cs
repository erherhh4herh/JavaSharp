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
	/// Unchecked exception thrown when an attempt is made to initiate an accept
	/// operation on a channel and a previous accept operation has not completed.
	/// 
	/// @since 1.7
	/// </summary>

	public class AcceptPendingException : IllegalStateException
	{

		private new const long SerialVersionUID = 2721339977965416421L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public AcceptPendingException()
		{
		}

	}

}