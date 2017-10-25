/*
 * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// An <code>RMIServerSocketFactory</code> instance is used by the RMI runtime
	/// in order to obtain server sockets for RMI calls.  A remote object can be
	/// associated with an <code>RMIServerSocketFactory</code> when it is
	/// created/exported via the constructors or <code>exportObject</code> methods
	/// of <code>java.rmi.server.UnicastRemoteObject</code> and
	/// <code>java.rmi.activation.Activatable</code> .
	/// 
	/// <para>An <code>RMIServerSocketFactory</code> instance associated with a remote
	/// object is used to obtain the <code>ServerSocket</code> used to accept
	/// incoming calls from clients.
	/// 
	/// </para>
	/// <para>An <code>RMIServerSocketFactory</code> instance can also be associated
	/// with a remote object registry so that clients can use custom socket
	/// communication with a remote object registry.
	/// 
	/// </para>
	/// <para>An implementation of this interface
	/// should implement <seealso cref="Object#equals"/> to return <code>true</code> when
	/// passed an instance that represents the same (functionally equivalent)
	/// server socket factory, and <code>false</code> otherwise (and it should also
	/// implement <seealso cref="Object#hashCode"/> consistently with its
	/// <code>Object.equals</code> implementation).
	/// 
	/// @author  Ann Wollrath
	/// @author  Peter Jones
	/// @since   1.2
	/// </para>
	/// </summary>
	/// <seealso cref=     java.rmi.server.UnicastRemoteObject </seealso>
	/// <seealso cref=     java.rmi.activation.Activatable </seealso>
	/// <seealso cref=     java.rmi.registry.LocateRegistry </seealso>
	public interface RMIServerSocketFactory
	{

		/// <summary>
		/// Create a server socket on the specified port (port 0 indicates
		/// an anonymous port). </summary>
		/// <param name="port"> the port number </param>
		/// <returns> the server socket on the specified port </returns>
		/// <exception cref="IOException"> if an I/O error occurs during server socket
		/// creation
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ServerSocket createServerSocket(int port) throws IOException;
		ServerSocket CreateServerSocket(int port);
	}

}