using System;

/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// An <code>ServerNotActiveException</code> is an <code>Exception</code>
	/// thrown during a call to <code>RemoteServer.getClientHost</code> if
	/// the getClientHost method is called outside of servicing a remote
	/// method call.
	/// 
	/// @author  Roger Riggs
	/// @since   JDK1.1 </summary>
	/// <seealso cref= java.rmi.server.RemoteServer#getClientHost() </seealso>
	public class ServerNotActiveException : Exception
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = 4687940720827538231L;

		/// <summary>
		/// Constructs an <code>ServerNotActiveException</code> with no specified
		/// detail message.
		/// @since JDK1.1
		/// </summary>
		public ServerNotActiveException()
		{
		}

		/// <summary>
		/// Constructs an <code>ServerNotActiveException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message.
		/// @since JDK1.1 </param>
		public ServerNotActiveException(String s) : base(s)
		{
		}
	}

}