using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{


	using Debug = sun.security.util.Debug;

	/// <summary>
	/// This class extends ClassLoader with additional support for defining
	/// classes with an associated code source and permissions which are
	/// retrieved by the system policy by default.
	/// 
	/// @author  Li Gong
	/// @author  Roland Schemers
	/// </summary>
	public class SecureClassLoader : ClassLoader
	{
		/*
		 * If initialization succeed this is set to true and security checks will
		 * succeed. Otherwise the object is not initialized and the object is
		 * useless.
		 */
		private readonly bool Initialized;

		// HashMap that maps CodeSource to ProtectionDomain
		// @GuardedBy("pdcache")
		private readonly Dictionary<CodeSource, ProtectionDomain> Pdcache = new Dictionary<CodeSource, ProtectionDomain>(11);

		private static readonly Debug Debug = Debug.getInstance("scl");

		static SecureClassLoader()
		{
			ClassLoader.RegisterAsParallelCapable();
		}

		/// <summary>
		/// Creates a new SecureClassLoader using the specified parent
		/// class loader for delegation.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's {@code checkCreateClassLoader}
		/// method  to ensure creation of a class loader is allowed.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="parent"> the parent ClassLoader </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkCreateClassLoader} method doesn't allow
		///             creation of a class loader. </exception>
		/// <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		protected internal SecureClassLoader(ClassLoader parent) : base(parent)
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Initialized = true;
		}

		/// <summary>
		/// Creates a new SecureClassLoader using the default parent class
		/// loader for delegation.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's {@code checkCreateClassLoader}
		/// method  to ensure creation of a class loader is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkCreateClassLoader} method doesn't allow
		///             creation of a class loader. </exception>
		/// <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		protected internal SecureClassLoader() : base()
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Initialized = true;
		}

		/// <summary>
		/// Converts an array of bytes into an instance of class Class,
		/// with an optional CodeSource. Before the
		/// class can be used it must be resolved.
		/// <para>
		/// If a non-null CodeSource is supplied a ProtectionDomain is
		/// constructed and associated with the class being defined.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="name"> the expected name of the class, or {@code null}
		///                  if not known, using '.' and not '/' as the separator
		///                  and without a trailing ".class" suffix. </param>
		/// <param name="b">    the bytes that make up the class data. The bytes in
		///             positions {@code off} through {@code off+len-1}
		///             should have the format of a valid class file as defined by
		///             <cite>The Java&trade; Virtual Machine Specification</cite>. </param>
		/// <param name="off">  the start offset in {@code b} of the class data </param>
		/// <param name="len">  the length of the class data </param>
		/// <param name="cs">   the associated CodeSource, or {@code null} if none </param>
		/// <returns> the {@code Class} object created from the data,
		///         and optional CodeSource. </returns>
		/// <exception cref="ClassFormatError"> if the data did not contain a valid class </exception>
		/// <exception cref="IndexOutOfBoundsException"> if either {@code off} or
		///             {@code len} is negative, or if
		///             {@code off+len} is greater than {@code b.length}.
		/// </exception>
		/// <exception cref="SecurityException"> if an attempt is made to add this class
		///             to a package that contains classes that were signed by
		///             a different set of certificates than this class, or if
		///             the class name begins with "java.". </exception>
		protected internal Class DefineClass(String name, sbyte[] b, int off, int len, CodeSource cs)
		{
			return DefineClass(name, b, off, len, GetProtectionDomain(cs));
		}

		/// <summary>
		/// Converts a <seealso cref="java.nio.ByteBuffer ByteBuffer"/>
		/// into an instance of class {@code Class}, with an optional CodeSource.
		/// Before the class can be used it must be resolved.
		/// <para>
		/// If a non-null CodeSource is supplied a ProtectionDomain is
		/// constructed and associated with the class being defined.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="name"> the expected name of the class, or {@code null}
		///                  if not known, using '.' and not '/' as the separator
		///                  and without a trailing ".class" suffix. </param>
		/// <param name="b">    the bytes that make up the class data.  The bytes from positions
		///                  {@code b.position()} through {@code b.position() + b.limit() -1}
		///                  should have the format of a valid class file as defined by
		///                  <cite>The Java&trade; Virtual Machine Specification</cite>. </param>
		/// <param name="cs">   the associated CodeSource, or {@code null} if none </param>
		/// <returns> the {@code Class} object created from the data,
		///         and optional CodeSource. </returns>
		/// <exception cref="ClassFormatError"> if the data did not contain a valid class </exception>
		/// <exception cref="SecurityException"> if an attempt is made to add this class
		///             to a package that contains classes that were signed by
		///             a different set of certificates than this class, or if
		///             the class name begins with "java.".
		/// 
		/// @since  1.5 </exception>
		protected internal Class DefineClass(String name, java.nio.ByteBuffer b, CodeSource cs)
		{
			return DefineClass(name, b, GetProtectionDomain(cs));
		}

		/// <summary>
		/// Returns the permissions for the given CodeSource object.
		/// <para>
		/// This method is invoked by the defineClass method which takes
		/// a CodeSource as an argument when it is constructing the
		/// ProtectionDomain for the class being defined.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="codesource"> the codesource.
		/// </param>
		/// <returns> the permissions granted to the codesource.
		///  </returns>
		protected internal virtual PermissionCollection GetPermissions(CodeSource codesource)
		{
			Check();
			return new Permissions(); // ProtectionDomain defers the binding
		}

		/*
		 * Returned cached ProtectionDomain for the specified CodeSource.
		 */
		private ProtectionDomain GetProtectionDomain(CodeSource cs)
		{
			if (cs == null)
			{
				return null;
			}

			ProtectionDomain pd = null;
			lock (Pdcache)
			{
				pd = Pdcache[cs];
				if (pd == null)
				{
					PermissionCollection perms = GetPermissions(cs);
					pd = new ProtectionDomain(cs, perms, this, null);
					Pdcache[cs] = pd;
					if (Debug != null)
					{
						Debug.println(" getPermissions " + pd);
						Debug.println("");
					}
				}
			}
			return pd;
		}

		/*
		 * Check to make sure the class loader has been initialized.
		 */
		private void Check()
		{
			if (!Initialized)
			{
				throw new SecurityException("ClassLoader object not initialized");
			}
		}

	}

}