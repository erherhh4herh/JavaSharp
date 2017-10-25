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
	/// An <code>RMIClientSocketFactory</code> instance is used by the RMI runtime
	/// in order to obtain client sockets for RMI calls.  A remote object can be
	/// associated with an <code>RMIClientSocketFactory</code> when it is
	/// created/exported via the constructors or <code>exportObject</code> methods
	/// of <code>java.rmi.server.UnicastRemoteObject</code> and
	/// <code>java.rmi.activation.Activatable</code> .
	/// 
	/// <para>An <code>RMIClientSocketFactory</code> instance associated with a remote
	/// object will be downloaded to clients when the remote object's reference is
	/// transmitted in an RMI call.  This <code>RMIClientSocketFactory</code> will
	/// be used to create connections to the remote object for remote method calls.
	/// 
	/// </para>
	/// <para>An <code>RMIClientSocketFactory</code> instance can also be associated
	/// with a remote object registry so that clients can use custom socket
	/// communication with a remote object registry.
	/// 
	/// </para>
	/// <para>An implementation of this interface should be serializable and
	/// should implement <seealso cref="Object#equals"/> to return <code>true</code> when
	/// passed an instance that represents the same (functionally equivalent)
	/// client socket factory, and <code>false</code> otherwise (and it should also
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
	public interface RMIClientSocketFactory
	{

		/// <summary>
		/// Create a client socket connected to the specified host and port. </summary>
		/// <param name="host">   the host name </param>
		/// <param name="port">   the port number </param>
		/// <returns> a socket connected to the specified host and port. </returns>
		/// <exception cref="IOException"> if an I/O error occurs during socket creation
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Socket createSocket(String host, int port) throws IOException;
		Socket CreateSocket(String host, int port);
	}

}