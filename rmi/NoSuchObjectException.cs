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

namespace java.rmi
{

	/// <summary>
	/// A <code>NoSuchObjectException</code> is thrown if an attempt is made to
	/// invoke a method on an object that no longer exists in the remote virtual
	/// machine.  If a <code>NoSuchObjectException</code> occurs attempting to
	/// invoke a method on a remote object, the call may be retransmitted and still
	/// preserve RMI's "at most once" call semantics.
	/// 
	/// A <code>NoSuchObjectException</code> is also thrown by the method
	/// <code>java.rmi.server.RemoteObject.toStub</code> and by the
	/// <code>unexportObject</code> methods of
	/// <code>java.rmi.server.UnicastRemoteObject</code> and
	/// <code>java.rmi.activation.Activatable</code> and
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1 </summary>
	/// <seealso cref=     java.rmi.server.RemoteObject#toStub(Remote) </seealso>
	/// <seealso cref=     java.rmi.server.UnicastRemoteObject#unexportObject(Remote,boolean) </seealso>
	/// <seealso cref=     java.rmi.activation.Activatable#unexportObject(Remote,boolean) </seealso>
	public class NoSuchObjectException : RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = 6619395951570472985L;

		/// <summary>
		/// Constructs a <code>NoSuchObjectException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since   JDK1.1 </param>
		public NoSuchObjectException(String s) : base(s)
		{
		}
	}

}