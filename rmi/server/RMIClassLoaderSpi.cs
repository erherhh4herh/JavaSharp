/*
 * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// <code>RMIClassLoaderSpi</code> is the service provider interface for
	/// <code>RMIClassLoader</code>.
	/// 
	/// In particular, an <code>RMIClassLoaderSpi</code> instance provides an
	/// implementation of the following static methods of
	/// <code>RMIClassLoader</code>:
	/// 
	/// <ul>
	/// 
	/// <li><seealso cref="RMIClassLoader#loadClass(URL,String)"/>
	/// <li><seealso cref="RMIClassLoader#loadClass(String,String)"/>
	/// <li><seealso cref="RMIClassLoader#loadClass(String,String,ClassLoader)"/>
	/// <li><seealso cref="RMIClassLoader#loadProxyClass(String,String[],ClassLoader)"/>
	/// <li><seealso cref="RMIClassLoader#getClassLoader(String)"/>
	/// <li><seealso cref="RMIClassLoader#getClassAnnotation(Class)"/>
	/// 
	/// </ul>
	/// 
	/// When one of those methods is invoked, its behavior is to delegate
	/// to a corresponding method on an instance of this class.
	/// The details of how each method delegates to the provider instance is
	/// described in the documentation for each particular method.
	/// See the documentation for <seealso cref="RMIClassLoader"/> for a description
	/// of how a provider instance is chosen.
	/// 
	/// @author      Peter Jones
	/// @author      Laird Dornin </summary>
	/// <seealso cref=         RMIClassLoader
	/// @since       1.4 </seealso>
	public abstract class RMIClassLoaderSpi
	{

		/// <summary>
		/// Provides the implementation for
		/// <seealso cref="RMIClassLoader#loadClass(URL,String)"/>,
		/// <seealso cref="RMIClassLoader#loadClass(String,String)"/>, and
		/// <seealso cref="RMIClassLoader#loadClass(String,String,ClassLoader)"/>.
		/// 
		/// Loads a class from a codebase URL path, optionally using the
		/// supplied loader.
		/// 
		/// Typically, a provider implementation will attempt to
		/// resolve the named class using the given <code>defaultLoader</code>,
		/// if specified, before attempting to resolve the class from the
		/// codebase URL path.
		/// 
		/// <para>An implementation of this method must either return a class
		/// with the given name or throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the list of URLs (separated by spaces) to load
		/// the class from, or <code>null</code>
		/// </param>
		/// <param name="name"> the name of the class to load
		/// </param>
		/// <param name="defaultLoader"> additional contextual class loader
		/// to use, or <code>null</code>
		/// </param>
		/// <returns>  the <code>Class</code> object representing the loaded class
		/// </returns>
		/// <exception cref="MalformedURLException"> if <code>codebase</code> is
		/// non-<code>null</code> and contains an invalid URL, or
		/// if <code>codebase</code> is <code>null</code> and a provider-specific
		/// URL used to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for the class
		/// could not be found at the specified location </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Class loadClass(String codebase, String name, ClassLoader defaultLoader) throws java.net.MalformedURLException, ClassNotFoundException;
		public abstract Class LoadClass(String codebase, String name, ClassLoader defaultLoader);

		/// <summary>
		/// Provides the implementation for
		/// <seealso cref="RMIClassLoader#loadProxyClass(String,String[],ClassLoader)"/>.
		/// 
		/// Loads a dynamic proxy class (see <seealso cref="java.lang.reflect.Proxy"/>
		/// that implements a set of interfaces with the given names
		/// from a codebase URL path, optionally using the supplied loader.
		/// 
		/// <para>An implementation of this method must either return a proxy
		/// class that implements the named interfaces or throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the list of URLs (space-separated) to load
		/// classes from, or <code>null</code>
		/// </param>
		/// <param name="interfaces"> the names of the interfaces for the proxy class
		/// to implement
		/// </param>
		/// <returns>  a dynamic proxy class that implements the named interfaces
		/// </returns>
		/// <param name="defaultLoader"> additional contextual class loader
		/// to use, or <code>null</code>
		/// </param>
		/// <exception cref="MalformedURLException"> if <code>codebase</code> is
		/// non-<code>null</code> and contains an invalid URL, or
		/// if <code>codebase</code> is <code>null</code> and a provider-specific
		/// URL used to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for one of
		/// the named interfaces could not be found at the specified location,
		/// or if creation of the dynamic proxy class failed (such as if
		/// <seealso cref="java.lang.reflect.Proxy#getProxyClass(ClassLoader,Class[])"/>
		/// would throw an <code>IllegalArgumentException</code> for the given
		/// interface list) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Class loadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader) throws java.net.MalformedURLException, ClassNotFoundException;
		public abstract Class LoadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader);

		/// <summary>
		/// Provides the implementation for
		/// <seealso cref="RMIClassLoader#getClassLoader(String)"/>.
		/// 
		/// Returns a class loader that loads classes from the given codebase
		/// URL path.
		/// 
		/// <para>If there is a security manger, its <code>checkPermission</code>
		/// method will be invoked with a
		/// <code>RuntimePermission("getClassLoader")</code> permission;
		/// this could result in a <code>SecurityException</code>.
		/// The implementation of this method may also perform further security
		/// checks to verify that the calling context has permission to connect
		/// to all of the URLs in the codebase URL path.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the list of URLs (space-separated) from which
		/// the returned class loader will load classes from, or <code>null</code>
		/// </param>
		/// <returns> a class loader that loads classes from the given codebase URL
		/// path
		/// </returns>
		/// <exception cref="MalformedURLException"> if <code>codebase</code> is
		/// non-<code>null</code> and contains an invalid URL, or
		/// if <code>codebase</code> is <code>null</code> and a provider-specific
		/// URL used to identify the class loader is invalid
		/// </exception>
		/// <exception cref="SecurityException"> if there is a security manager and the
		/// invocation of its <code>checkPermission</code> method fails, or
		/// if the caller does not have permission to connect to all of the
		/// URLs in the codebase URL path </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract ClassLoader getClassLoader(String codebase) throws java.net.MalformedURLException;
		public abstract ClassLoader GetClassLoader(String codebase); // SecurityException

		/// <summary>
		/// Provides the implementation for
		/// <seealso cref="RMIClassLoader#getClassAnnotation(Class)"/>.
		/// 
		/// Returns the annotation string (representing a location for
		/// the class definition) that RMI will use to annotate the class
		/// descriptor when marshalling objects of the given class.
		/// </summary>
		/// <param name="cl"> the class to obtain the annotation for
		/// </param>
		/// <returns>  a string to be used to annotate the given class when
		/// it gets marshalled, or <code>null</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>cl</code> is <code>null</code> </exception>
		public abstract String GetClassAnnotation(Class cl);
	}

}