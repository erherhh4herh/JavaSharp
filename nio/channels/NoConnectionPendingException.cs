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
	/// Unchecked exception thrown when the {@link SocketChannel#finishConnect
	/// finishConnect} method of a <seealso cref="SocketChannel"/> is invoked without first
	/// successfully invoking its <seealso cref="SocketChannel#connect connect"/> method.
	/// 
	/// @since 1.4
	/// </summary>

	public class NoConnectionPendingException : IllegalStateException
	{

		private new const long SerialVersionUID = -8296561183633134743L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public NoConnectionPendingException()
		{
		}

	}

}