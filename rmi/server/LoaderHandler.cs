using System;

/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// <code>LoaderHandler</code> is an interface used internally by the RMI
	/// runtime in previous implementation versions.  It should never be accessed
	/// by application code.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </summary>
	/// @deprecated no replacement 
	[Obsolete("no replacement")]
	public interface LoaderHandler
	{

		/// <summary>
		/// package of system <code>LoaderHandler</code> implementation. </summary>

		/// <summary>
		/// Loads a class from the location specified by the
		/// <code>java.rmi.server.codebase</code> property.
		/// </summary>
		/// <param name="name"> the name of the class to load </param>
		/// <returns> the <code>Class</code> object representing the loaded class </returns>
		/// <exception cref="MalformedURLException">
		///            if the system property <b>java.rmi.server.codebase</b>
		///            contains an invalid URL </exception>
		/// <exception cref="ClassNotFoundException">
		///            if a definition for the class could not
		///            be found at the codebase location.
		/// @since JDK1.1 </exception>
		/// @deprecated no replacement 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("no replacement") Class loadClass(String name) throws java.net.MalformedURLException, ClassNotFoundException;
		[Obsolete("no replacement")]
		Class LoadClass(String name);

		/// <summary>
		/// Loads a class from a URL.
		/// </summary>
		/// <param name="codebase">  the URL from which to load the class </param>
		/// <param name="name">      the name of the class to load </param>
		/// <returns> the <code>Class</code> object representing the loaded class </returns>
		/// <exception cref="MalformedURLException">
		///            if the <code>codebase</code> paramater
		///            contains an invalid URL </exception>
		/// <exception cref="ClassNotFoundException">
		///            if a definition for the class could not
		///            be found at the specified URL
		/// @since JDK1.1 </exception>
		/// @deprecated no replacement 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("no replacement") Class loadClass(java.net.URL codebase, String name) throws java.net.MalformedURLException, ClassNotFoundException;
		[Obsolete("no replacement")]
		Class LoadClass(URL codebase, String name);

		/// <summary>
		/// Returns the security context of the given class loader.
		/// </summary>
		/// <param name="loader">  a class loader from which to get the security context </param>
		/// <returns> the security context
		/// @since JDK1.1 </returns>
		/// @deprecated no replacement 
		[Obsolete("no replacement")]
		Object GetSecurityContext(ClassLoader loader);
	}

	public static class LoaderHandler_Fields
	{
		public const String PackagePrefix = "sun.rmi.server";
	}

}