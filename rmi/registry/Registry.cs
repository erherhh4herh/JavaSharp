/*
 * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// <code>Registry</code> is a remote interface to a simple remote
	/// object registry that provides methods for storing and retrieving
	/// remote object references bound with arbitrary string names.  The
	/// <code>bind</code>, <code>unbind</code>, and <code>rebind</code>
	/// methods are used to alter the name bindings in the registry, and
	/// the <code>lookup</code> and <code>list</code> methods are used to
	/// query the current name bindings.
	/// 
	/// <para>In its typical usage, a <code>Registry</code> enables RMI client
	/// bootstrapping: it provides a simple means for a client to obtain an
	/// initial reference to a remote object.  Therefore, a registry's
	/// remote object implementation is typically exported with a
	/// well-known address, such as with a well-known {@link
	/// java.rmi.server.ObjID#REGISTRY_ID ObjID} and TCP port number
	/// (default is <seealso cref="#REGISTRY_PORT 1099"/>).
	/// 
	/// </para>
	/// <para>The <seealso cref="LocateRegistry"/> class provides a programmatic API for
	/// constructing a bootstrap reference to a <code>Registry</code> at a
	/// remote address (see the static <code>getRegistry</code> methods)
	/// and for creating and exporting a <code>Registry</code> in the
	/// current VM on a particular local address (see the static
	/// <code>createRegistry</code> methods).
	/// 
	/// </para>
	/// <para>A <code>Registry</code> implementation may choose to restrict
	/// access to some or all of its methods (for example, methods that
	/// mutate the registry's bindings may be restricted to calls
	/// originating from the local host).  If a <code>Registry</code>
	/// method chooses to deny access for a given invocation, its
	/// implementation may throw <seealso cref="java.rmi.AccessException"/>, which
	/// (because it extends <seealso cref="java.rmi.RemoteException"/>) will be
	/// wrapped in a <seealso cref="java.rmi.ServerException"/> when caught by a
	/// remote client.
	/// 
	/// </para>
	/// <para>The names used for bindings in a <code>Registry</code> are pure
	/// strings, not parsed.  A service which stores its remote reference
	/// in a <code>Registry</code> may wish to use a package name as a
	/// prefix in the name binding to reduce the likelihood of name
	/// collisions in the registry.
	/// 
	/// @author      Ann Wollrath
	/// @author      Peter Jones
	/// @since       JDK1.1
	/// </para>
	/// </summary>
	/// <seealso cref=         LocateRegistry </seealso>
	public interface Registry : Remote
	{

		/// <summary>
		/// Well known port for registry. </summary>

		/// <summary>
		/// Returns the remote reference bound to the specified
		/// <code>name</code> in this registry.
		/// </summary>
		/// <param name="name"> the name for the remote reference to look up
		/// </param>
		/// <returns>  a reference to a remote object
		/// </returns>
		/// <exception cref="NotBoundException"> if <code>name</code> is not currently bound
		/// </exception>
		/// <exception cref="RemoteException"> if remote communication with the
		/// registry failed; if exception is a <code>ServerException</code>
		/// containing an <code>AccessException</code>, then the registry
		/// denies the caller access to perform this operation
		/// </exception>
		/// <exception cref="AccessException"> if this registry is local and it denies
		/// the caller access to perform this operation
		/// </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.rmi.Remote lookup(String name) throws java.rmi.RemoteException, java.rmi.NotBoundException, java.rmi.AccessException;
		Remote Lookup(String name);

		/// <summary>
		/// Binds a remote reference to the specified <code>name</code> in
		/// this registry.
		/// </summary>
		/// <param name="name"> the name to associate with the remote reference </param>
		/// <param name="obj"> a reference to a remote object (usually a stub)
		/// </param>
		/// <exception cref="AlreadyBoundException"> if <code>name</code> is already bound
		/// </exception>
		/// <exception cref="RemoteException"> if remote communication with the
		/// registry failed; if exception is a <code>ServerException</code>
		/// containing an <code>AccessException</code>, then the registry
		/// denies the caller access to perform this operation (if
		/// originating from a non-local host, for example)
		/// </exception>
		/// <exception cref="AccessException"> if this registry is local and it denies
		/// the caller access to perform this operation
		/// </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is
		/// <code>null</code>, or if <code>obj</code> is <code>null</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void bind(String name, java.rmi.Remote obj) throws java.rmi.RemoteException, java.rmi.AlreadyBoundException, java.rmi.AccessException;
		void Bind(String name, Remote obj);

		/// <summary>
		/// Removes the binding for the specified <code>name</code> in
		/// this registry.
		/// </summary>
		/// <param name="name"> the name of the binding to remove
		/// </param>
		/// <exception cref="NotBoundException"> if <code>name</code> is not currently bound
		/// </exception>
		/// <exception cref="RemoteException"> if remote communication with the
		/// registry failed; if exception is a <code>ServerException</code>
		/// containing an <code>AccessException</code>, then the registry
		/// denies the caller access to perform this operation (if
		/// originating from a non-local host, for example)
		/// </exception>
		/// <exception cref="AccessException"> if this registry is local and it denies
		/// the caller access to perform this operation
		/// </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unbind(String name) throws java.rmi.RemoteException, java.rmi.NotBoundException, java.rmi.AccessException;
		void Unbind(String name);

		/// <summary>
		/// Replaces the binding for the specified <code>name</code> in
		/// this registry with the supplied remote reference.  If there is
		/// an existing binding for the specified <code>name</code>, it is
		/// discarded.
		/// </summary>
		/// <param name="name"> the name to associate with the remote reference </param>
		/// <param name="obj"> a reference to a remote object (usually a stub)
		/// </param>
		/// <exception cref="RemoteException"> if remote communication with the
		/// registry failed; if exception is a <code>ServerException</code>
		/// containing an <code>AccessException</code>, then the registry
		/// denies the caller access to perform this operation (if
		/// originating from a non-local host, for example)
		/// </exception>
		/// <exception cref="AccessException"> if this registry is local and it denies
		/// the caller access to perform this operation
		/// </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is
		/// <code>null</code>, or if <code>obj</code> is <code>null</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void rebind(String name, java.rmi.Remote obj) throws java.rmi.RemoteException, java.rmi.AccessException;
		void Rebind(String name, Remote obj);

		/// <summary>
		/// Returns an array of the names bound in this registry.  The
		/// array will contain a snapshot of the names bound in this
		/// registry at the time of the given invocation of this method.
		/// </summary>
		/// <returns>  an array of the names bound in this registry
		/// </returns>
		/// <exception cref="RemoteException"> if remote communication with the
		/// registry failed; if exception is a <code>ServerException</code>
		/// containing an <code>AccessException</code>, then the registry
		/// denies the caller access to perform this operation
		/// </exception>
		/// <exception cref="AccessException"> if this registry is local and it denies
		/// the caller access to perform this operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String[] list() throws java.rmi.RemoteException, java.rmi.AccessException;
		String[] List();
	}

	public static class Registry_Fields
	{
		public const int REGISTRY_PORT = 1099;
	}

}