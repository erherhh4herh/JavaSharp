using System;

/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.registry
{

	using RegistryImpl = sun.rmi.registry.RegistryImpl;
	using UnicastRef2 = sun.rmi.server.UnicastRef2;
	using UnicastRef = sun.rmi.server.UnicastRef;
	using Util = sun.rmi.server.Util;
	using LiveRef = sun.rmi.transport.LiveRef;
	using TCPEndpoint = sun.rmi.transport.tcp.TCPEndpoint;

	/// <summary>
	/// <code>LocateRegistry</code> is used to obtain a reference to a bootstrap
	/// remote object registry on a particular host (including the local host), or
	/// to create a remote object registry that accepts calls on a specific port.
	/// 
	/// <para> Note that a <code>getRegistry</code> call does not actually make a
	/// connection to the remote host.  It simply creates a local reference to
	/// the remote registry and will succeed even if no registry is running on
	/// the remote host.  Therefore, a subsequent method invocation to a remote
	/// registry returned as a result of this method may fail.
	/// 
	/// @author  Ann Wollrath
	/// @author  Peter Jones
	/// @since   JDK1.1
	/// </para>
	/// </summary>
	/// <seealso cref=     java.rmi.registry.Registry </seealso>
	public sealed class LocateRegistry
	{

		/// <summary>
		/// Private constructor to disable public construction.
		/// </summary>
		private LocateRegistry()
		{
		}

		/// <summary>
		/// Returns a reference to the the remote object <code>Registry</code> for
		/// the local host on the default registry port of 1099.
		/// </summary>
		/// <returns> reference (a stub) to the remote object registry </returns>
		/// <exception cref="RemoteException"> if the reference could not be created
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry getRegistry() throws java.rmi.RemoteException
		public static Registry Registry
		{
			get
			{
				return GetRegistry(null, Registry_Fields.REGISTRY_PORT);
			}
		}

		/// <summary>
		/// Returns a reference to the the remote object <code>Registry</code> for
		/// the local host on the specified <code>port</code>.
		/// </summary>
		/// <param name="port"> port on which the registry accepts requests </param>
		/// <returns> reference (a stub) to the remote object registry </returns>
		/// <exception cref="RemoteException"> if the reference could not be created
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry getRegistry(int port) throws java.rmi.RemoteException
		public static Registry GetRegistry(int port)
		{
			return GetRegistry(null, port);
		}

		/// <summary>
		/// Returns a reference to the remote object <code>Registry</code> on the
		/// specified <code>host</code> on the default registry port of 1099.  If
		/// <code>host</code> is <code>null</code>, the local host is used.
		/// </summary>
		/// <param name="host"> host for the remote registry </param>
		/// <returns> reference (a stub) to the remote object registry </returns>
		/// <exception cref="RemoteException"> if the reference could not be created
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry getRegistry(String host) throws java.rmi.RemoteException
		public static Registry GetRegistry(String host)
		{
			return GetRegistry(host, Registry_Fields.REGISTRY_PORT);
		}

		/// <summary>
		/// Returns a reference to the remote object <code>Registry</code> on the
		/// specified <code>host</code> and <code>port</code>. If <code>host</code>
		/// is <code>null</code>, the local host is used.
		/// </summary>
		/// <param name="host"> host for the remote registry </param>
		/// <param name="port"> port on which the registry accepts requests </param>
		/// <returns> reference (a stub) to the remote object registry </returns>
		/// <exception cref="RemoteException"> if the reference could not be created
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry getRegistry(String host, int port) throws java.rmi.RemoteException
		public static Registry GetRegistry(String host, int port)
		{
			return GetRegistry(host, port, null);
		}

		/// <summary>
		/// Returns a locally created remote reference to the remote object
		/// <code>Registry</code> on the specified <code>host</code> and
		/// <code>port</code>.  Communication with this remote registry will
		/// use the supplied <code>RMIClientSocketFactory</code> <code>csf</code>
		/// to create <code>Socket</code> connections to the registry on the
		/// remote <code>host</code> and <code>port</code>.
		/// </summary>
		/// <param name="host"> host for the remote registry </param>
		/// <param name="port"> port on which the registry accepts requests </param>
		/// <param name="csf">  client-side <code>Socket</code> factory used to
		///      make connections to the registry.  If <code>csf</code>
		///      is null, then the default client-side <code>Socket</code>
		///      factory will be used in the registry stub. </param>
		/// <returns> reference (a stub) to the remote registry </returns>
		/// <exception cref="RemoteException"> if the reference could not be created
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry getRegistry(String host, int port, java.rmi.server.RMIClientSocketFactory csf) throws java.rmi.RemoteException
		public static Registry GetRegistry(String host, int port, RMIClientSocketFactory csf)
		{
			Registry registry = null;

			if (port <= 0)
			{
				port = Registry_Fields.REGISTRY_PORT;
			}

			if (host == null || host.Length() == 0)
			{
				// If host is blank (as returned by "file:" URL in 1.0.2 used in
				// java.rmi.Naming), try to convert to real local host name so
				// that the RegistryImpl's checkAccess will not fail.
				try
				{
					host = java.net.InetAddress.LocalHost.HostAddress;
				}
				catch (Exception)
				{
					// If that failed, at least try "" (localhost) anyway...
					host = "";
				}
			}

			/*
			 * Create a proxy for the registry with the given host, port, and
			 * client socket factory.  If the supplied client socket factory is
			 * null, then the ref type is a UnicastRef, otherwise the ref type
			 * is a UnicastRef2.  If the property
			 * java.rmi.server.ignoreStubClasses is true, then the proxy
			 * returned is an instance of a dynamic proxy class that implements
			 * the Registry interface; otherwise the proxy returned is an
			 * instance of the pregenerated stub class for RegistryImpl.
			 **/
			LiveRef liveRef = new LiveRef(new ObjID(ObjID.REGISTRY_ID), new TCPEndpoint(host, port, csf, null), false);
			RemoteRef @ref = (csf == null) ? new UnicastRef(liveRef) : new UnicastRef2(liveRef);

			return (Registry) Util.createProxy(typeof(RegistryImpl), @ref, false);
		}

		/// <summary>
		/// Creates and exports a <code>Registry</code> instance on the local
		/// host that accepts requests on the specified <code>port</code>.
		/// 
		/// <para>The <code>Registry</code> instance is exported as if the static
		/// {@link UnicastRemoteObject#exportObject(Remote,int)
		/// UnicastRemoteObject.exportObject} method is invoked, passing the
		/// <code>Registry</code> instance and the specified <code>port</code> as
		/// arguments, except that the <code>Registry</code> instance is
		/// exported with a well-known object identifier, an <seealso cref="ObjID"/>
		/// instance constructed with the value <seealso cref="ObjID#REGISTRY_ID"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port"> the port on which the registry accepts requests </param>
		/// <returns> the registry </returns>
		/// <exception cref="RemoteException"> if the registry could not be exported
		/// @since JDK1.1
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry createRegistry(int port) throws java.rmi.RemoteException
		public static Registry CreateRegistry(int port)
		{
			return new RegistryImpl(port);
		}

		/// <summary>
		/// Creates and exports a <code>Registry</code> instance on the local
		/// host that uses custom socket factories for communication with that
		/// instance.  The registry that is created listens for incoming
		/// requests on the given <code>port</code> using a
		/// <code>ServerSocket</code> created from the supplied
		/// <code>RMIServerSocketFactory</code>.
		/// 
		/// <para>The <code>Registry</code> instance is exported as if
		/// the static {@link
		/// UnicastRemoteObject#exportObject(Remote,int,RMIClientSocketFactory,RMIServerSocketFactory)
		/// UnicastRemoteObject.exportObject} method is invoked, passing the
		/// <code>Registry</code> instance, the specified <code>port</code>, the
		/// specified <code>RMIClientSocketFactory</code>, and the specified
		/// <code>RMIServerSocketFactory</code> as arguments, except that the
		/// <code>Registry</code> instance is exported with a well-known object
		/// identifier, an <seealso cref="ObjID"/> instance constructed with the value
		/// <seealso cref="ObjID#REGISTRY_ID"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="port"> port on which the registry accepts requests </param>
		/// <param name="csf">  client-side <code>Socket</code> factory used to
		///      make connections to the registry </param>
		/// <param name="ssf">  server-side <code>ServerSocket</code> factory
		///      used to accept connections to the registry </param>
		/// <returns> the registry </returns>
		/// <exception cref="RemoteException"> if the registry could not be exported
		/// @since 1.2
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Registry createRegistry(int port, java.rmi.server.RMIClientSocketFactory csf, java.rmi.server.RMIServerSocketFactory ssf) throws java.rmi.RemoteException
		public static Registry CreateRegistry(int port, RMIClientSocketFactory csf, RMIServerSocketFactory ssf)
		{
			return new RegistryImpl(port, csf, ssf);
		}
	}

}