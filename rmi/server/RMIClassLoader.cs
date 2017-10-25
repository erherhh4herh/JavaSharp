using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <code>RMIClassLoader</code> comprises static methods to support
	/// dynamic class loading with RMI.  Included are methods for loading
	/// classes from a network location (one or more URLs) and obtaining
	/// the location from which an existing class should be loaded by
	/// remote parties.  These methods are used by the RMI runtime when
	/// marshalling and unmarshalling classes contained in the arguments
	/// and return values of remote method calls, and they also may be
	/// invoked directly by applications in order to mimic RMI's dynamic
	/// class loading behavior.
	/// 
	/// <para>The implementation of the following static methods
	/// 
	/// <ul>
	/// 
	/// <li><seealso cref="#loadClass(URL,String)"/>
	/// <li><seealso cref="#loadClass(String,String)"/>
	/// <li><seealso cref="#loadClass(String,String,ClassLoader)"/>
	/// <li><seealso cref="#loadProxyClass(String,String[],ClassLoader)"/>
	/// <li><seealso cref="#getClassLoader(String)"/>
	/// <li><seealso cref="#getClassAnnotation(Class)"/>
	/// 
	/// </ul>
	/// 
	/// is provided by an instance of <seealso cref="RMIClassLoaderSpi"/>, the
	/// service provider interface for those methods.  When one of the
	/// methods is invoked, its behavior is to delegate to a corresponding
	/// method on the service provider instance.  The details of how each
	/// method delegates to the provider instance is described in the
	/// documentation for each particular method.
	/// 
	/// </para>
	/// <para>The service provider instance is chosen as follows:
	/// 
	/// <ul>
	/// 
	/// <li>If the system property
	/// <code>java.rmi.server.RMIClassLoaderSpi</code> is defined, then if
	/// its value equals the string <code>"default"</code>, the provider
	/// instance will be the value returned by an invocation of the {@link
	/// #getDefaultProviderInstance()} method, and for any other value, if
	/// a class named with the value of the property can be loaded by the
	/// system class loader (see <seealso cref="ClassLoader#getSystemClassLoader"/>)
	/// and that class is assignable to <seealso cref="RMIClassLoaderSpi"/> and has a
	/// public no-argument constructor, then that constructor will be
	/// invoked to create the provider instance.  If the property is
	/// defined but any other of those conditions are not true, then an
	/// unspecified <code>Error</code> will be thrown to code that attempts
	/// to use <code>RMIClassLoader</code>, indicating the failure to
	/// obtain a provider instance.
	/// 
	/// <li>If a resource named
	/// <code>META-INF/services/java.rmi.server.RMIClassLoaderSpi</code> is
	/// visible to the system class loader, then the contents of that
	/// resource are interpreted as a provider-configuration file, and the
	/// first class name specified in that file is used as the provider
	/// class name.  If a class with that name can be loaded by the system
	/// class loader and that class is assignable to {@link
	/// RMIClassLoaderSpi} and has a public no-argument constructor, then
	/// that constructor will be invoked to create the provider instance.
	/// If the resource is found but a provider cannot be instantiated as
	/// described, then an unspecified <code>Error</code> will be thrown to
	/// code that attempts to use <code>RMIClassLoader</code>, indicating
	/// the failure to obtain a provider instance.
	/// 
	/// <li>Otherwise, the provider instance will be the value returned by
	/// an invocation of the <seealso cref="#getDefaultProviderInstance()"/> method.
	/// 
	/// </ul>
	/// 
	/// @author      Ann Wollrath
	/// @author      Peter Jones
	/// @author      Laird Dornin
	/// </para>
	/// </summary>
	/// <seealso cref=         RMIClassLoaderSpi
	/// @since       JDK1.1 </seealso>
	public class RMIClassLoader
	{

		/// <summary>
		/// "default" provider instance </summary>
		private static readonly RMIClassLoaderSpi DefaultProvider = NewDefaultProviderInstance();

		/// <summary>
		/// provider instance </summary>
		private static readonly RMIClassLoaderSpi provider = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<RMIClassLoaderSpi>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual RMIClassLoaderSpi Run()
			{
				return InitializeProvider();
			}
		}

		/*
		 * Disallow anyone from creating one of these.
		 */
		private RMIClassLoader()
		{
		}

		/// <summary>
		/// Loads the class with the specified <code>name</code>.
		/// 
		/// <para>This method delegates to <seealso cref="#loadClass(String,String)"/>,
		/// passing <code>null</code> as the first argument and
		/// <code>name</code> as the second argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the class to load
		/// </param>
		/// <returns>  the <code>Class</code> object representing the loaded class
		/// </returns>
		/// <exception cref="MalformedURLException"> if a provider-specific URL used
		/// to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for the class
		/// could not be found at the codebase location
		/// </exception>
		/// @deprecated replaced by <code>loadClass(String,String)</code> method 
		/// <seealso cref= #loadClass(String,String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("replaced by <code>loadClass(String,String)</code> method") public static Class loadClass(String name) throws java.net.MalformedURLException, ClassNotFoundException
		[Obsolete("replaced by <code>loadClass(String,String)</code> method")]
		public static Class LoadClass(String name)
		{
			return LoadClass((String) null, name);
		}

		/// <summary>
		/// Loads a class from a codebase URL.
		/// 
		/// If <code>codebase</code> is <code>null</code>, then this method
		/// will behave the same as <seealso cref="#loadClass(String,String)"/> with a
		/// <code>null</code> <code>codebase</code> and the given class name.
		/// 
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#loadClass(String,String,ClassLoader)"/>
		/// method of the provider instance, passing the result of invoking
		/// <seealso cref="URL#toString"/> on the given URL (or <code>null</code> if
		/// <code>codebase</code> is null) as the first argument,
		/// <code>name</code> as the second argument,
		/// and <code>null</code> as the third argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the URL to load the class from, or <code>null</code>
		/// </param>
		/// <param name="name"> the name of the class to load
		/// </param>
		/// <returns>  the <code>Class</code> object representing the loaded class
		/// </returns>
		/// <exception cref="MalformedURLException"> if <code>codebase</code> is
		/// <code>null</code> and a provider-specific URL used
		/// to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for the class
		/// could not be found at the specified URL </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Class loadClass(java.net.URL codebase, String name) throws java.net.MalformedURLException, ClassNotFoundException
		public static Class LoadClass(URL codebase, String name)
		{
			return provider.loadClass(codebase != null ? codebase.ToString() : null, name, null);
		}

		/// <summary>
		/// Loads a class from a codebase URL path.
		/// 
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#loadClass(String,String,ClassLoader)"/>
		/// method of the provider instance, passing <code>codebase</code>
		/// as the first argument, <code>name</code> as the second argument,
		/// and <code>null</code> as the third argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the list of URLs (separated by spaces) to load
		/// the class from, or <code>null</code>
		/// </param>
		/// <param name="name"> the name of the class to load
		/// </param>
		/// <returns>  the <code>Class</code> object representing the loaded class
		/// </returns>
		/// <exception cref="MalformedURLException"> if <code>codebase</code> is
		/// non-<code>null</code> and contains an invalid URL, or if
		/// <code>codebase</code> is <code>null</code> and a provider-specific
		/// URL used to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for the class
		/// could not be found at the specified location
		/// 
		/// @since   1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Class loadClass(String codebase, String name) throws java.net.MalformedURLException, ClassNotFoundException
		public static Class LoadClass(String codebase, String name)
		{
			return provider.loadClass(codebase, name, null);
		}

		/// <summary>
		/// Loads a class from a codebase URL path, optionally using the
		/// supplied loader.
		/// 
		/// This method should be used when the caller would like to make
		/// available to the provider implementation an additional contextual
		/// class loader to consider, such as the loader of a caller on the
		/// stack.  Typically, a provider implementation will attempt to
		/// resolve the named class using the given <code>defaultLoader</code>,
		/// if specified, before attempting to resolve the class from the
		/// codebase URL path.
		/// 
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#loadClass(String,String,ClassLoader)"/>
		/// method of the provider instance, passing <code>codebase</code>
		/// as the first argument, <code>name</code> as the second argument,
		/// and <code>defaultLoader</code> as the third argument.
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
		/// non-<code>null</code> and contains an invalid URL, or if
		/// <code>codebase</code> is <code>null</code> and a provider-specific
		/// URL used to load classes is invalid
		/// </exception>
		/// <exception cref="ClassNotFoundException"> if a definition for the class
		/// could not be found at the specified location
		/// 
		/// @since   1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Class loadClass(String codebase, String name, ClassLoader defaultLoader) throws java.net.MalformedURLException, ClassNotFoundException
		public static Class LoadClass(String codebase, String name, ClassLoader defaultLoader)
		{
			return provider.loadClass(codebase, name, defaultLoader);
		}

		/// <summary>
		/// Loads a dynamic proxy class (see <seealso cref="java.lang.reflect.Proxy"/>)
		/// that implements a set of interfaces with the given names
		/// from a codebase URL path.
		/// 
		/// <para>The interfaces will be resolved similar to classes loaded via
		/// the <seealso cref="#loadClass(String,String)"/> method using the given
		/// <code>codebase</code>.
		/// 
		/// </para>
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#loadProxyClass(String,String[],ClassLoader)"/>
		/// method of the provider instance, passing <code>codebase</code>
		/// as the first argument, <code>interfaces</code> as the second argument,
		/// and <code>defaultLoader</code> as the third argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codebase"> the list of URLs (space-separated) to load
		/// classes from, or <code>null</code>
		/// </param>
		/// <param name="interfaces"> the names of the interfaces for the proxy class
		/// to implement
		/// </param>
		/// <param name="defaultLoader"> additional contextual class loader
		/// to use, or <code>null</code>
		/// </param>
		/// <returns>  a dynamic proxy class that implements the named interfaces
		/// </returns>
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
		/// interface list)
		/// 
		/// @since   1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Class loadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader) throws ClassNotFoundException, java.net.MalformedURLException
		public static Class LoadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader)
		{
			return provider.loadProxyClass(codebase, interfaces, defaultLoader);
		}

		/// <summary>
		/// Returns a class loader that loads classes from the given codebase
		/// URL path.
		/// 
		/// <para>The class loader returned is the class loader that the
		/// <seealso cref="#loadClass(String,String)"/> method would use to load classes
		/// for the same <code>codebase</code> argument.
		/// 
		/// </para>
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#getClassLoader(String)"/> method
		/// of the provider instance, passing <code>codebase</code> as the argument.
		/// 
		/// </para>
		/// <para>If there is a security manger, its <code>checkPermission</code>
		/// method will be invoked with a
		/// <code>RuntimePermission("getClassLoader")</code> permission;
		/// this could result in a <code>SecurityException</code>.
		/// The provider implementation of this method may also perform further
		/// security checks to verify that the calling context has permission to
		/// connect to all of the URLs in the codebase URL path.
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
		/// URLs in the codebase URL path
		/// 
		/// @since   1.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static ClassLoader getClassLoader(String codebase) throws java.net.MalformedURLException, SecurityException
		public static ClassLoader GetClassLoader(String codebase)
		{
			return provider.getClassLoader(codebase);
		}

		/// <summary>
		/// Returns the annotation string (representing a location for
		/// the class definition) that RMI will use to annotate the class
		/// descriptor when marshalling objects of the given class.
		/// 
		/// <para>This method delegates to the
		/// <seealso cref="RMIClassLoaderSpi#getClassAnnotation(Class)"/> method
		/// of the provider instance, passing <code>cl</code> as the argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cl"> the class to obtain the annotation for
		/// </param>
		/// <returns>  a string to be used to annotate the given class when
		/// it gets marshalled, or <code>null</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>cl</code> is <code>null</code>
		/// 
		/// @since   1.2 </exception>
		/*
		 * REMIND: Should we say that the returned class annotation will or
		 * should be a (space-separated) list of URLs?
		 */
		public static String GetClassAnnotation(Class cl)
		{
			return provider.getClassAnnotation(cl);
		}

		/// <summary>
		/// Returns the canonical instance of the default provider
		/// for the service provider interface <seealso cref="RMIClassLoaderSpi"/>.
		/// If the system property <code>java.rmi.server.RMIClassLoaderSpi</code>
		/// is not defined, then the <code>RMIClassLoader</code> static
		/// methods
		/// 
		/// <ul>
		/// 
		/// <li><seealso cref="#loadClass(URL,String)"/>
		/// <li><seealso cref="#loadClass(String,String)"/>
		/// <li><seealso cref="#loadClass(String,String,ClassLoader)"/>
		/// <li><seealso cref="#loadProxyClass(String,String[],ClassLoader)"/>
		/// <li><seealso cref="#getClassLoader(String)"/>
		/// <li><seealso cref="#getClassAnnotation(Class)"/>
		/// 
		/// </ul>
		/// 
		/// will use the canonical instance of the default provider
		/// as the service provider instance.
		/// 
		/// <para>If there is a security manager, its
		/// <code>checkPermission</code> method will be invoked with a
		/// <code>RuntimePermission("setFactory")</code> permission; this
		/// could result in a <code>SecurityException</code>.
		/// 
		/// </para>
		/// <para>The default service provider instance implements
		/// <seealso cref="RMIClassLoaderSpi"/> as follows:
		/// 
		/// <blockquote>
		/// 
		/// </para>
		/// <para>The <b>{@link RMIClassLoaderSpi#getClassAnnotation(Class)
		/// getClassAnnotation}</b> method returns a <code>String</code>
		/// representing the codebase URL path that a remote party should
		/// use to download the definition for the specified class.  The
		/// format of the returned string is a path of URLs separated by
		/// spaces.
		/// 
		/// The codebase string returned depends on the defining class
		/// loader of the specified class:
		/// 
		/// <ul>
		/// 
		/// </para>
		/// <li><para>If the class loader is the system class loader (see
		/// <seealso cref="ClassLoader#getSystemClassLoader"/>), a parent of the
		/// system class loader such as the loader used for installed
		/// extensions, or the bootstrap class loader (which may be
		/// represented by <code>null</code>), then the value of the
		/// <code>java.rmi.server.codebase</code> property (or possibly an
		/// earlier cached value) is returned, or
		/// <code>null</code> is returned if that property is not set.
		/// 
		/// </para>
		/// <li><para>Otherwise, if the class loader is an instance of
		/// <code>URLClassLoader</code>, then the returned string is a
		/// space-separated list of the external forms of the URLs returned
		/// by invoking the <code>getURLs</code> methods of the loader.  If
		/// the <code>URLClassLoader</code> was created by this provider to
		/// service an invocation of its <code>loadClass</code> or
		/// <code>loadProxyClass</code> methods, then no permissions are
		/// required to get the associated codebase string.  If it is an
		/// arbitrary other <code>URLClassLoader</code> instance, then if
		/// there is a security manager, its <code>checkPermission</code>
		/// method will be invoked once for each URL returned by the
		/// <code>getURLs</code> method, with the permission returned by
		/// invoking <code>openConnection().getPermission()</code> on each
		/// URL; if any of those invocations throws a
		/// <code>SecurityException</code> or an <code>IOException</code>,
		/// then the value of the <code>java.rmi.server.codebase</code>
		/// property (or possibly an earlier cached value) is returned, or
		/// <code>null</code> is returned if that property is not set.
		/// 
		/// </para>
		/// <li><para>Finally, if the class loader is not an instance of
		/// <code>URLClassLoader</code>, then the value of the
		/// <code>java.rmi.server.codebase</code> property (or possibly an
		/// earlier cached value) is returned, or
		/// <code>null</code> is returned if that property is not set.
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>For the implementations of the methods described below,
		/// which all take a <code>String</code> parameter named
		/// <code>codebase</code> that is a space-separated list of URLs,
		/// each invocation has an associated <i>codebase loader</i> that
		/// is identified using the <code>codebase</code> argument in
		/// conjunction with the current thread's context class loader (see
		/// <seealso cref="Thread#getContextClassLoader()"/>).  When there is a
		/// security manager, this provider maintains an internal table of
		/// class loader instances (which are at least instances of {@link
		/// java.net.URLClassLoader}) keyed by the pair of their parent
		/// class loader and their codebase URL path (an ordered list of
		/// URLs).  If the <code>codebase</code> argument is <code>null</code>,
		/// the codebase URL path is the value of the system property
		/// <code>java.rmi.server.codebase</code> or possibly an
		/// earlier cached value.  For a given codebase URL path passed as the
		/// <code>codebase</code> argument to an invocation of one of the
		/// below methods in a given context, the codebase loader is the
		/// loader in the table with the specified codebase URL path and
		/// the current thread's context class loader as its parent.  If no
		/// such loader exists, then one is created and added to the table.
		/// The table does not maintain strong references to its contained
		/// loaders, in order to allow them and their defined classes to be
		/// garbage collected when not otherwise reachable.  In order to
		/// prevent arbitrary untrusted code from being implicitly loaded
		/// into a virtual machine with no security manager, if there is no
		/// security manager set, the codebase loader is just the current
		/// thread's context class loader (the supplied codebase URL path
		/// is ignored, so remote class loading is disabled).
		/// 
		/// </para>
		/// <para>The <b>{@link RMIClassLoaderSpi#getClassLoader(String)
		/// getClassLoader}</b> method returns the codebase loader for the
		/// specified codebase URL path.  If there is a security manager,
		/// then if the calling context does not have permission to connect
		/// to all of the URLs in the codebase URL path, a
		/// <code>SecurityException</code> will be thrown.
		/// 
		/// </para>
		/// <para>The <b>{@link
		/// RMIClassLoaderSpi#loadClass(String,String,ClassLoader)
		/// loadClass}</b> method attempts to load the class with the
		/// specified name as follows:
		/// 
		/// <blockquote>
		/// 
		/// If the <code>defaultLoader</code> argument is
		/// non-<code>null</code>, it first attempts to load the class with the
		/// specified <code>name</code> using the
		/// <code>defaultLoader</code>, such as by evaluating
		/// 
		/// <pre>
		///     Class.forName(name, false, defaultLoader)
		/// </pre>
		/// 
		/// If the class is successfully loaded from the
		/// <code>defaultLoader</code>, that class is returned.  If an
		/// exception other than <code>ClassNotFoundException</code> is
		/// thrown, that exception is thrown to the caller.
		/// 
		/// </para>
		/// <para>Next, the <code>loadClass</code> method attempts to load the
		/// class with the specified <code>name</code> using the codebase
		/// loader for the specified codebase URL path.
		/// If there is a security manager, then the calling context
		/// must have permission to connect to all of the URLs in the
		/// codebase URL path; otherwise, the current thread's context
		/// class loader will be used instead of the codebase loader.
		/// 
		/// </blockquote>
		/// 
		/// </para>
		/// <para>The <b>{@link
		/// RMIClassLoaderSpi#loadProxyClass(String,String[],ClassLoader)
		/// loadProxyClass}</b> method attempts to return a dynamic proxy
		/// class with the named interface as follows:
		/// 
		/// <blockquote>
		/// 
		/// </para>
		/// <para>If the <code>defaultLoader</code> argument is
		/// non-<code>null</code> and all of the named interfaces can be
		/// resolved through that loader, then,
		/// 
		/// <ul>
		/// 
		/// <li>if all of the resolved interfaces are <code>public</code>,
		/// then it first attempts to obtain a dynamic proxy class (using
		/// {@link
		/// java.lang.reflect.Proxy#getProxyClass(ClassLoader,Class[])
		/// Proxy.getProxyClass}) for the resolved interfaces defined in
		/// the codebase loader; if that attempt throws an
		/// <code>IllegalArgumentException</code>, it then attempts to
		/// obtain a dynamic proxy class for the resolved interfaces
		/// defined in the <code>defaultLoader</code>.  If both attempts
		/// throw <code>IllegalArgumentException</code>, then this method
		/// throws a <code>ClassNotFoundException</code>.  If any other
		/// exception is thrown, that exception is thrown to the caller.
		/// 
		/// <li>if all of the non-<code>public</code> resolved interfaces
		/// are defined in the same class loader, then it attempts to
		/// obtain a dynamic proxy class for the resolved interfaces
		/// defined in that loader.
		/// 
		/// <li>otherwise, a <code>LinkageError</code> is thrown (because a
		/// class that implements all of the specified interfaces cannot be
		/// defined in any loader).
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>Otherwise, if all of the named interfaces can be resolved
		/// through the codebase loader, then,
		/// 
		/// <ul>
		/// 
		/// <li>if all of the resolved interfaces are <code>public</code>,
		/// then it attempts to obtain a dynamic proxy class for the
		/// resolved interfaces in the codebase loader.  If the attempt
		/// throws an <code>IllegalArgumentException</code>, then this
		/// method throws a <code>ClassNotFoundException</code>.
		/// 
		/// <li>if all of the non-<code>public</code> resolved interfaces
		/// are defined in the same class loader, then it attempts to
		/// obtain a dynamic proxy class for the resolved interfaces
		/// defined in that loader.
		/// 
		/// <li>otherwise, a <code>LinkageError</code> is thrown (because a
		/// class that implements all of the specified interfaces cannot be
		/// defined in any loader).
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>Otherwise, a <code>ClassNotFoundException</code> is thrown
		/// for one of the named interfaces that could not be resolved.
		/// 
		/// </blockquote>
		/// 
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the canonical instance of the default service provider
		/// </returns>
		/// <exception cref="SecurityException"> if there is a security manager and the
		/// invocation of its <code>checkPermission</code> method fails
		/// 
		/// @since   1.4 </exception>
		public static RMIClassLoaderSpi DefaultProviderInstance
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new RuntimePermission("setFactory"));
				}
				return DefaultProvider;
			}
		}

		/// <summary>
		/// Returns the security context of the given class loader.
		/// </summary>
		/// <param name="loader"> a class loader from which to get the security context
		/// </param>
		/// <returns>  the security context
		/// </returns>
		/// @deprecated no replacement.  As of the Java 2 platform v1.2, RMI no
		/// longer uses this method to obtain a class loader's security context. 
		/// <seealso cref= java.lang.SecurityManager#getSecurityContext() </seealso>
		[Obsolete("no replacement.  As of the Java 2 platform v1.2, RMI no")]
		public static Object GetSecurityContext(ClassLoader loader)
		{
			return sun.rmi.server.LoaderHandler.getSecurityContext(loader);
		}

		/// <summary>
		/// Creates an instance of the default provider class.
		/// </summary>
		private static RMIClassLoaderSpi NewDefaultProviderInstance()
		{
			return new RMIClassLoaderSpiAnonymousInnerClassHelper();
		}

		private class RMIClassLoaderSpiAnonymousInnerClassHelper : RMIClassLoaderSpi
		{
			public RMIClassLoaderSpiAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Class loadClass(String codebase, String name, ClassLoader defaultLoader) throws java.net.MalformedURLException, ClassNotFoundException
			public override Class LoadClass(String codebase, String name, ClassLoader defaultLoader)
			{
				return sun.rmi.server.LoaderHandler.loadClass(codebase, name, defaultLoader);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Class loadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader) throws java.net.MalformedURLException, ClassNotFoundException
			public override Class LoadProxyClass(String codebase, String[] interfaces, ClassLoader defaultLoader)
			{
				return sun.rmi.server.LoaderHandler.loadProxyClass(codebase, interfaces, defaultLoader);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ClassLoader getClassLoader(String codebase) throws java.net.MalformedURLException
			public override ClassLoader GetClassLoader(String codebase)
			{
				return sun.rmi.server.LoaderHandler.getClassLoader(codebase);
			}

			public override String GetClassAnnotation(Class cl)
			{
				return sun.rmi.server.LoaderHandler.getClassAnnotation(cl);
			}
		}

		/// <summary>
		/// Chooses provider instance, following above documentation.
		/// 
		/// This method assumes that it has been invoked in a privileged block.
		/// </summary>
		private static RMIClassLoaderSpi InitializeProvider()
		{
			/*
			 * First check for the system property being set:
			 */
			String providerClassName = System.getProperty("java.rmi.server.RMIClassLoaderSpi");

			if (providerClassName != null)
			{
				if (providerClassName.Equals("default"))
				{
					return DefaultProvider;
				}

				try
				{
					Class providerClass = Class.ForName(providerClassName, false, ClassLoader.SystemClassLoader).AsSubclass(typeof(RMIClassLoaderSpi));
					return providerClass.NewInstance();

				}
				catch (ClassNotFoundException e)
				{
					throw new NoClassDefFoundError(e.Message);
				}
				catch (IllegalAccessException e)
				{
					throw new IllegalAccessError(e.Message);
				}
				catch (InstantiationException e)
				{
					throw new InstantiationError(e.Message);
				}
				catch (ClassCastException e)
				{
					Error error = new LinkageError("provider class not assignable to RMIClassLoaderSpi");
					error.InitCause(e);
					throw error;
				}
			}

			/*
			 * Next look for a provider configuration file installed:
			 */
			IEnumerator<RMIClassLoaderSpi> iter = ServiceLoader.Load(typeof(RMIClassLoaderSpi), ClassLoader.SystemClassLoader).Iterator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (iter.hasNext())
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return iter.next();
				}
				catch (ClassCastException e)
				{
					Error error = new LinkageError("provider class not assignable to RMIClassLoaderSpi");
					error.InitCause(e);
					throw error;
				}
			}

			/*
			 * Finally, return the canonical instance of the default provider.
			 */
			return DefaultProvider;
		}
	}

}