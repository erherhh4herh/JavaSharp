using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{

	using Resource = sun.misc.Resource;
	using URLClassPath = sun.misc.URLClassPath;
	using ParseUtil = sun.net.www.ParseUtil;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// This class loader is used to load classes and resources from a search
	/// path of URLs referring to both JAR files and directories. Any URL that
	/// ends with a '/' is assumed to refer to a directory. Otherwise, the URL
	/// is assumed to refer to a JAR file which will be opened as needed.
	/// <para>
	/// The AccessControlContext of the thread that created the instance of
	/// URLClassLoader will be used when subsequently loading classes and
	/// resources.
	/// </para>
	/// <para>
	/// The classes that are loaded are by default granted permission only to
	/// access the URLs specified when the URLClassLoader was created.
	/// 
	/// @author  David Connelly
	/// @since   1.2
	/// </para>
	/// </summary>
	public class URLClassLoader : SecureClassLoader, Closeable
	{
		/* The search path for classes and resources */
		private readonly URLClassPath Ucp;

		/* The context to be used when loading classes and resources */
		private readonly AccessControlContext Acc;

		/// <summary>
		/// Constructs a new URLClassLoader for the given URLs. The URLs will be
		/// searched in the order specified for classes and resources after first
		/// searching in the specified parent class loader. Any URL that ends with
		/// a '/' is assumed to refer to a directory. Otherwise, the URL is assumed
		/// to refer to a JAR file which will be downloaded and opened as needed.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's {@code checkCreateClassLoader} method
		/// to ensure creation of a class loader is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="urls"> the URLs from which to load classes and resources </param>
		/// <param name="parent"> the parent class loader for delegation </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkCreateClassLoader} method doesn't allow
		///             creation of a class loader. </exception>
		/// <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		/// <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		public URLClassLoader(URL[] urls, ClassLoader parent) : base(parent)
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Ucp = new URLClassPath(urls);
			this.Acc = AccessController.Context;
		}

		internal URLClassLoader(URL[] urls, ClassLoader parent, AccessControlContext acc) : base(parent)
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Ucp = new URLClassPath(urls);
			this.Acc = acc;
		}

		/// <summary>
		/// Constructs a new URLClassLoader for the specified URLs using the
		/// default delegation parent {@code ClassLoader}. The URLs will
		/// be searched in the order specified for classes and resources after
		/// first searching in the parent class loader. Any URL that ends with
		/// a '/' is assumed to refer to a directory. Otherwise, the URL is
		/// assumed to refer to a JAR file which will be downloaded and opened
		/// as needed.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's {@code checkCreateClassLoader} method
		/// to ensure creation of a class loader is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="urls"> the URLs from which to load classes and resources
		/// </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkCreateClassLoader} method doesn't allow
		///             creation of a class loader. </exception>
		/// <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		/// <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		public URLClassLoader(URL[] urls) : base()
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Ucp = new URLClassPath(urls);
			this.Acc = AccessController.Context;
		}

		internal URLClassLoader(URL[] urls, AccessControlContext acc) : base()
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Ucp = new URLClassPath(urls);
			this.Acc = acc;
		}

		/// <summary>
		/// Constructs a new URLClassLoader for the specified URLs, parent
		/// class loader, and URLStreamHandlerFactory. The parent argument
		/// will be used as the parent class loader for delegation. The
		/// factory argument will be used as the stream handler factory to
		/// obtain protocol handlers when creating new jar URLs.
		/// 
		/// <para>If there is a security manager, this method first
		/// calls the security manager's {@code checkCreateClassLoader} method
		/// to ensure creation of a class loader is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="urls"> the URLs from which to load classes and resources </param>
		/// <param name="parent"> the parent class loader for delegation </param>
		/// <param name="factory"> the URLStreamHandlerFactory to use when creating URLs
		/// </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkCreateClassLoader} method doesn't allow
		///             creation of a class loader. </exception>
		/// <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		/// <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		public URLClassLoader(URL[] urls, ClassLoader parent, URLStreamHandlerFactory factory) : base(parent)
		{
			// this is to make the stack depth consistent with 1.1
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			Ucp = new URLClassPath(urls, factory);
			Acc = AccessController.Context;
		}

		/* A map (used as a set) to keep track of closeable local resources
		 * (either JarFiles or FileInputStreams). We don't care about
		 * Http resources since they don't need to be closed.
		 *
		 * If the resource is coming from a jar file
		 * we keep a (weak) reference to the JarFile object which can
		 * be closed if URLClassLoader.close() called. Due to jar file
		 * caching there will typically be only one JarFile object
		 * per underlying jar file.
		 *
		 * For file resources, which is probably a less common situation
		 * we have to keep a weak reference to each stream.
		 */

		private WeakHashMap<Closeable, Void> Closeables = new WeakHashMap<Closeable, Void>();

		/// <summary>
		/// Returns an input stream for reading the specified resource.
		/// If this loader is closed, then any resources opened by this method
		/// will be closed.
		/// 
		/// <para> The search order is described in the documentation for {@link
		/// #getResource(String)}.  </para>
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An input stream for reading the resource, or {@code null}
		///          if the resource could not be found
		/// 
		/// @since  1.7 </returns>
		public override InputStream GetResourceAsStream(String name)
		{
			URL url = GetResource(name);
			try
			{
				if (url == null)
				{
					return null;
				}
				URLConnection urlc = url.OpenConnection();
				InputStream @is = urlc.InputStream;
				if (urlc is JarURLConnection)
				{
					JarURLConnection juc = (JarURLConnection)urlc;
					JarFile jar = juc.JarFile;
					lock (Closeables)
					{
						if (!Closeables.ContainsKey(jar))
						{
							Closeables.Put(jar, null);
						}
					}
				}
				else if (urlc is sun.net.www.protocol.file.FileURLConnection)
				{
					lock (Closeables)
					{
						Closeables.Put(@is, null);
					}
				}
				return @is;
			}
			catch (IOException)
			{
				return null;
			}
		}

	   /// <summary>
	   /// Closes this URLClassLoader, so that it can no longer be used to load
	   /// new classes or resources that are defined by this loader.
	   /// Classes and resources defined by any of this loader's parents in the
	   /// delegation hierarchy are still accessible. Also, any classes or resources
	   /// that are already loaded, are still accessible.
	   /// <para>
	   /// In the case of jar: and file: URLs, it also closes any files
	   /// that were opened by it. If another thread is loading a
	   /// class when the {@code close} method is invoked, then the result of
	   /// that load is undefined.
	   /// </para>
	   /// <para>
	   /// The method makes a best effort attempt to close all opened files,
	   /// by catching <seealso cref="IOException"/>s internally. Unchecked exceptions
	   /// and errors are not caught. Calling close on an already closed
	   /// loader has no effect.
	   /// </para>
	   /// <para>
	   /// </para>
	   /// </summary>
	   /// <exception cref="IOException"> if closing any file opened by this class loader
	   /// resulted in an IOException. Any such exceptions are caught internally.
	   /// If only one is caught, then it is re-thrown. If more than one exception
	   /// is caught, then the second and following exceptions are added
	   /// as suppressed exceptions of the first one caught, which is then re-thrown.
	   /// </exception>
	   /// <exception cref="SecurityException"> if a security manager is set, and it denies
	   ///   <seealso cref="RuntimePermission"/>{@code ("closeClassLoader")}
	   /// 
	   /// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public virtual void Close()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(new RuntimePermission("closeClassLoader"));
			}
			IList<IOException> errors = Ucp.closeLoaders();

			// now close any remaining streams.

			lock (Closeables)
			{
				WeakHashMap<Closeable, Void>.KeyCollection keys = Closeables.KeySet();
				foreach (Closeable c in keys)
				{
					try
					{
						c.Close();
					}
					catch (IOException ioex)
					{
						errors.Add(ioex);
					}
				}
				Closeables.Clear();
			}

			if (errors.Count == 0)
			{
				return;
			}

			IOException firstex = errors.Remove(0);

			// Suppress any remaining exceptions

			foreach (IOException error in errors)
			{
				firstex.AddSuppressed(error);
			}
			throw firstex;
		}

		/// <summary>
		/// Appends the specified URL to the list of URLs to search for
		/// classes and resources.
		/// <para>
		/// If the URL specified is {@code null} or is already in the
		/// list of URLs, or if this loader is closed, then invoking this
		/// method has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url"> the URL to be added to the search path of URLs </param>
		protected internal virtual void AddURL(URL url)
		{
			Ucp.addURL(url);
		}

		/// <summary>
		/// Returns the search path of URLs for loading classes and resources.
		/// This includes the original list of URLs specified to the constructor,
		/// along with any URLs subsequently appended by the addURL() method. </summary>
		/// <returns> the search path of URLs for loading classes and resources. </returns>
		public virtual URL[] URLs
		{
			get
			{
				return Ucp.URLs;
			}
		}

		/// <summary>
		/// Finds and loads the class with the specified name from the URL search
		/// path. Any URLs referring to JAR files are loaded and opened as needed
		/// until the class is found.
		/// </summary>
		/// <param name="name"> the name of the class </param>
		/// <returns> the resulting class </returns>
		/// <exception cref="ClassNotFoundException"> if the class could not be found,
		///            or if the loader is closed. </exception>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class findClass(final String name) throws ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal override Class FindClass(String name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class result;
			Class result;
			try
			{
				result = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, name), Acc);
			}
			catch (java.security.PrivilegedActionException pae)
			{
				throw (ClassNotFoundException) pae.Exception;
			}
			if (result == null)
			{
				throw new ClassNotFoundException(name);
			}
			return result;
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Class>
		{
			private readonly URLClassLoader OuterInstance;

			private string Name;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(URLClassLoader outerInstance, string name)
			{
				this.OuterInstance = outerInstance;
				this.Name = name;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Class run() throws ClassNotFoundException
			public virtual Class Run()
			{
				String path = Name.Replace('.', '/') + ".class";
				Resource res = OuterInstance.Ucp.getResource(path, false);
				if (res != null)
				{
					try
					{
						return outerInstance.DefineClass(Name, res);
					}
					catch (IOException e)
					{
						throw new ClassNotFoundException(Name, e);
					}
				}
				else
				{
					return null;
				}
			}
		}

		/*
		 * Retrieve the package using the specified package name.
		 * If non-null, verify the package using the specified code
		 * source and manifest.
		 */
		private Package GetAndVerifyPackage(String pkgname, Manifest man, URL url)
		{
			Package pkg = GetPackage(pkgname);
			if (pkg != null)
			{
				// Package found, so check package sealing.
				if (pkg.Sealed)
				{
					// Verify that code source URL is the same.
					if (!pkg.IsSealed(url))
					{
						throw new SecurityException("sealing violation: package " + pkgname + " is sealed");
					}
				}
				else
				{
					// Make sure we are not attempting to seal the package
					// at this code source URL.
					if ((man != null) && IsSealed(pkgname, man))
					{
						throw new SecurityException("sealing violation: can't seal package " + pkgname + ": already loaded");
					}
				}
			}
			return pkg;
		}

		// Also called by VM to define Package for classes loaded from the CDS
		// archive
		private void DefinePackageInternal(String pkgname, Manifest man, URL url)
		{
			if (GetAndVerifyPackage(pkgname, man, url) == null)
			{
				try
				{
					if (man != null)
					{
						DefinePackage(pkgname, man, url);
					}
					else
					{
						DefinePackage(pkgname, null, null, null, null, null, null, null);
					}
				}
				catch (IllegalArgumentException)
				{
					// parallel-capable class loaders: re-verify in case of a
					// race condition
					if (GetAndVerifyPackage(pkgname, man, url) == null)
					{
						// Should never happen
						throw new AssertionError("Cannot find package " + pkgname);
					}
				}
			}
		}

		/*
		 * Defines a Class using the class bytes obtained from the specified
		 * Resource. The resulting Class must be resolved before it can be
		 * used.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class defineClass(String name, sun.misc.Resource res) throws java.io.IOException
		private Class DefineClass(String name, Resource res)
		{
			long t0 = System.nanoTime();
			int i = name.LastIndexOf('.');
			URL url = res.CodeSourceURL;
			if (i != -1)
			{
				String pkgname = name.Substring(0, i);
				// Check if package already loaded.
				Manifest man = res.Manifest;
				DefinePackageInternal(pkgname, man, url);
			}
			// Now read the class bytes and define the class
			java.nio.ByteBuffer bb = res.ByteBuffer;
			if (bb != null)
			{
				// Use (direct) ByteBuffer:
				CodeSigner[] signers = res.CodeSigners;
				CodeSource cs = new CodeSource(url, signers);
				sun.misc.PerfCounter.ReadClassBytesTime.addElapsedTimeFrom(t0);
				return DefineClass(name, bb, cs);
			}
			else
			{
				sbyte[] b = res.Bytes;
				// must read certificates AFTER reading bytes.
				CodeSigner[] signers = res.CodeSigners;
				CodeSource cs = new CodeSource(url, signers);
				sun.misc.PerfCounter.ReadClassBytesTime.addElapsedTimeFrom(t0);
				return DefineClass(name, b, 0, b.Length, cs);
			}
		}

		/// <summary>
		/// Defines a new package by name in this ClassLoader. The attributes
		/// contained in the specified Manifest will be used to obtain package
		/// version and sealing information. For sealed packages, the additional
		/// URL specifies the code source URL from which the package was loaded.
		/// </summary>
		/// <param name="name">  the package name </param>
		/// <param name="man">   the Manifest containing package version and sealing
		///              information </param>
		/// <param name="url">   the code source url for the package, or null if none </param>
		/// <exception cref="IllegalArgumentException"> if the package name duplicates
		///              an existing package either in this class loader or one
		///              of its ancestors </exception>
		/// <returns> the newly defined Package object </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Package definePackage(String name, java.util.jar.Manifest man, URL url) throws IllegalArgumentException
		protected internal virtual Package DefinePackage(String name, Manifest man, URL url)
		{
			String path = name.Replace('.', '/') + "/";
			String specTitle = null, specVersion = null, specVendor = null;
			String implTitle = null, implVersion = null, implVendor = null;
			String @sealed = null;
			URL sealBase = null;

			Attributes attr = man.GetAttributes(path);
			if (attr != null)
			{
				specTitle = attr.GetValue(Attributes.Name.SPECIFICATION_TITLE);
				specVersion = attr.GetValue(Attributes.Name.SPECIFICATION_VERSION);
				specVendor = attr.GetValue(Attributes.Name.SPECIFICATION_VENDOR);
				implTitle = attr.GetValue(Attributes.Name.IMPLEMENTATION_TITLE);
				implVersion = attr.GetValue(Attributes.Name.IMPLEMENTATION_VERSION);
				implVendor = attr.GetValue(Attributes.Name.IMPLEMENTATION_VENDOR);
				@sealed = attr.GetValue(Attributes.Name.SEALED);
			}
			attr = man.MainAttributes;
			if (attr != null)
			{
				if (specTitle == null)
				{
					specTitle = attr.GetValue(Attributes.Name.SPECIFICATION_TITLE);
				}
				if (specVersion == null)
				{
					specVersion = attr.GetValue(Attributes.Name.SPECIFICATION_VERSION);
				}
				if (specVendor == null)
				{
					specVendor = attr.GetValue(Attributes.Name.SPECIFICATION_VENDOR);
				}
				if (implTitle == null)
				{
					implTitle = attr.GetValue(Attributes.Name.IMPLEMENTATION_TITLE);
				}
				if (implVersion == null)
				{
					implVersion = attr.GetValue(Attributes.Name.IMPLEMENTATION_VERSION);
				}
				if (implVendor == null)
				{
					implVendor = attr.GetValue(Attributes.Name.IMPLEMENTATION_VENDOR);
				}
				if (@sealed == null)
				{
					@sealed = attr.GetValue(Attributes.Name.SEALED);
				}
			}
			if ("true".Equals(@sealed, StringComparison.CurrentCultureIgnoreCase))
			{
				sealBase = url;
			}
			return DefinePackage(name, specTitle, specVersion, specVendor, implTitle, implVersion, implVendor, sealBase);
		}

		/*
		 * Returns true if the specified package name is sealed according to the
		 * given manifest.
		 */
		private bool IsSealed(String name, Manifest man)
		{
			String path = name.Replace('.', '/') + "/";
			Attributes attr = man.GetAttributes(path);
			String @sealed = null;
			if (attr != null)
			{
				@sealed = attr.GetValue(Attributes.Name.SEALED);
			}
			if (@sealed == null)
			{
				if ((attr = man.MainAttributes) != null)
				{
					@sealed = attr.GetValue(Attributes.Name.SEALED);
				}
			}
			return "true".Equals(@sealed, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		/// Finds the resource with the specified name on the URL search path.
		/// </summary>
		/// <param name="name"> the name of the resource </param>
		/// <returns> a {@code URL} for the resource, or {@code null}
		/// if the resource could not be found, or if the loader is closed. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public URL findResource(final String name)
		public override URL FindResource(String name)
		{
			/*
			 * The same restriction to finding classes applies to resources
			 */
			URL url = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, name), Acc);

			return url != null ? Ucp.checkURL(url) : null;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<URL>
		{
			private readonly URLClassLoader OuterInstance;

			private string Name;

			public PrivilegedActionAnonymousInnerClassHelper(URLClassLoader outerInstance, string name)
			{
				this.OuterInstance = outerInstance;
				this.Name = name;
			}

			public virtual URL Run()
			{
				return OuterInstance.Ucp.findResource(Name, true);
			}
		}

		/// <summary>
		/// Returns an Enumeration of URLs representing all of the resources
		/// on the URL search path having the specified name.
		/// </summary>
		/// <param name="name"> the resource name </param>
		/// <exception cref="IOException"> if an I/O exception occurs </exception>
		/// <returns> an {@code Enumeration} of {@code URL}s
		///         If the loader is closed, the Enumeration will be empty. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Iterator<URL> findResources(final String name) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override IEnumerator<URL> FindResources(String name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<URL> e = ucp.findResources(name, true);
			IEnumerator<URL> e = Ucp.findResources(name, true);

			return new IteratorAnonymousInnerClassHelper(this, e);
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<URL>
		{
			private readonly URLClassLoader OuterInstance;

			private IEnumerator<URL> e;

			public IteratorAnonymousInnerClassHelper(URLClassLoader outerInstance, IEnumerator<URL> e)
			{
				this.OuterInstance = outerInstance;
				this.e = e;
				url = null;
			}

			private URL url;

			private bool Next()
			{
				if (url != null)
				{
					return true;
				}
				do
				{
					URL u = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this), OuterInstance.Acc);
					if (u == null)
					{
						break;
					}
					url = OuterInstance.Ucp.checkURL(u);
				} while (url == null);
				return url != null;
			}

			private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<URL>
			{
				private readonly IteratorAnonymousInnerClassHelper OuterInstance;

				public PrivilegedActionAnonymousInnerClassHelper2(IteratorAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual URL Run()
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (!OuterInstance.e.hasMoreElements())
					{
						return null;
					}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return OuterInstance.e.nextElement();
				}
			}

			public virtual URL NextElement()
			{
				if (!next())
				{
					throw new NoSuchElementException();
				}
				URL u = url;
				url = null;
				return u;
			}

			public virtual bool HasMoreElements()
			{
				return next();
			}
		}

		/// <summary>
		/// Returns the permissions for the given codesource object.
		/// The implementation of this method first calls super.getPermissions
		/// and then adds permissions based on the URL of the codesource.
		/// <para>
		/// If the protocol of this URL is "jar", then the permission granted
		/// is based on the permission that is required by the URL of the Jar
		/// file.
		/// </para>
		/// <para>
		/// If the protocol is "file" and there is an authority component, then
		/// permission to connect to and accept connections from that authority
		/// may be granted. If the protocol is "file"
		/// and the path specifies a file, then permission to read that
		/// file is granted. If protocol is "file" and the path is
		/// a directory, permission is granted to read all files
		/// and (recursively) all files and subdirectories contained in
		/// that directory.
		/// </para>
		/// <para>
		/// If the protocol is not "file", then permission
		/// to connect to and accept connections from the URL's host is granted.
		/// </para>
		/// </summary>
		/// <param name="codesource"> the codesource </param>
		/// <exception cref="NullPointerException"> if {@code codesource} is {@code null}. </exception>
		/// <returns> the permissions granted to the codesource </returns>
		protected internal override PermissionCollection GetPermissions(CodeSource codesource)
		{
			PermissionCollection perms = base.GetPermissions(codesource);

			URL url = codesource.Location;

			Permission p;
			URLConnection urlConnection;

			try
			{
				urlConnection = url.OpenConnection();
				p = urlConnection.Permission;
			}
			catch (IOException)
			{
				p = null;
				urlConnection = null;
			}

			if (p is FilePermission)
			{
				// if the permission has a separator char on the end,
				// it means the codebase is a directory, and we need
				// to add an additional permission to read recursively
				String path = p.Name;
				if (path.EndsWith(File.Separator))
				{
					path += "-";
					p = new FilePermission(path, SecurityConstants.FILE_READ_ACTION);
				}
			}
			else if ((p == null) && (url.Protocol.Equals("file")))
			{
				String path = url.File.Replace('/', System.IO.Path.DirectorySeparatorChar);
				path = ParseUtil.decode(path);
				if (path.EndsWith(File.Separator))
				{
					path += "-";
				}
				p = new FilePermission(path, SecurityConstants.FILE_READ_ACTION);
			}
			else
			{
				/// <summary>
				/// Not loading from a 'file:' URL so we want to give the class
				/// permission to connect to and accept from the remote host
				/// after we've made sure the host is the correct one and is valid.
				/// </summary>
				URL locUrl = url;
				if (urlConnection is JarURLConnection)
				{
					locUrl = ((JarURLConnection)urlConnection).JarFileURL;
				}
				String host = locUrl.Host;
				if (host != null && (host.Length() > 0))
				{
					p = new SocketPermission(host, SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION);
				}
			}

			// make sure the person that created this class loader
			// would have this permission

			if (p != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecurityManager sm = System.getSecurityManager();
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.Permission fp = p;
					Permission fp = p;
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(this, sm, fp), Acc);
				}
				perms.Add(p);
			}
			return perms;
		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<Void>
		{
			private readonly URLClassLoader OuterInstance;

			private java.lang.SecurityManager Sm;
			private Permission Fp;

			public PrivilegedActionAnonymousInnerClassHelper3(URLClassLoader outerInstance, java.lang.SecurityManager sm, Permission fp)
			{
				this.OuterInstance = outerInstance;
				this.Sm = sm;
				this.Fp = fp;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws SecurityException
			public virtual Void Run()
			{
				Sm.CheckPermission(Fp);
				return null;
			}
		}

		/// <summary>
		/// Creates a new instance of URLClassLoader for the specified
		/// URLs and parent class loader. If a security manager is
		/// installed, the {@code loadClass} method of the URLClassLoader
		/// returned by this method will invoke the
		/// {@code SecurityManager.checkPackageAccess} method before
		/// loading the class.
		/// </summary>
		/// <param name="urls"> the URLs to search for classes and resources </param>
		/// <param name="parent"> the parent class loader for delegation </param>
		/// <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		/// <returns> the resulting class loader </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static URLClassLoader newInstance(final URL[] urls, final ClassLoader parent)
		public static URLClassLoader NewInstance(URL[] urls, ClassLoader parent)
		{
			// Save the caller's context
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.AccessControlContext acc = java.security.AccessController.getContext();
			AccessControlContext acc = AccessController.Context;
			// Need a privileged block to create the class loader
			URLClassLoader ucl = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper4(urls, parent, acc));
			return ucl;
		}

		private class PrivilegedActionAnonymousInnerClassHelper4 : PrivilegedAction<URLClassLoader>
		{
			private java.net.URL[] Urls;
			private java.lang.ClassLoader Parent;
			private AccessControlContext Acc;

			public PrivilegedActionAnonymousInnerClassHelper4(java.net.URL[] urls, java.lang.ClassLoader parent, AccessControlContext acc)
			{
				this.Urls = urls;
				this.Parent = parent;
				this.Acc = acc;
			}

			public virtual URLClassLoader Run()
			{
				return new FactoryURLClassLoader(Urls, Parent, Acc);
			}
		}

		/// <summary>
		/// Creates a new instance of URLClassLoader for the specified
		/// URLs and default parent class loader. If a security manager is
		/// installed, the {@code loadClass} method of the URLClassLoader
		/// returned by this method will invoke the
		/// {@code SecurityManager.checkPackageAccess} before
		/// loading the class.
		/// </summary>
		/// <param name="urls"> the URLs to search for classes and resources </param>
		/// <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		/// <returns> the resulting class loader </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static URLClassLoader newInstance(final URL[] urls)
		public static URLClassLoader NewInstance(URL[] urls)
		{
			// Save the caller's context
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.AccessControlContext acc = java.security.AccessController.getContext();
			AccessControlContext acc = AccessController.Context;
			// Need a privileged block to create the class loader
			URLClassLoader ucl = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper5(urls, acc));
			return ucl;
		}

		private class PrivilegedActionAnonymousInnerClassHelper5 : PrivilegedAction<URLClassLoader>
		{
			private java.net.URL[] Urls;
			private AccessControlContext Acc;

			public PrivilegedActionAnonymousInnerClassHelper5(java.net.URL[] urls, AccessControlContext acc)
			{
				this.Urls = urls;
				this.Acc = acc;
			}

			public virtual URLClassLoader Run()
			{
				return new FactoryURLClassLoader(Urls, Acc);
			}
		}

		static URLClassLoader()
		{
			sun.misc.SharedSecrets.setJavaNetAccess(new JavaNetAccessAnonymousInnerClassHelper()
		   );
			ClassLoader.RegisterAsParallelCapable();
		}

		private class JavaNetAccessAnonymousInnerClassHelper : sun.misc.JavaNetAccess
		{
			public JavaNetAccessAnonymousInnerClassHelper()
			{
			}

			public virtual URLClassPath GetURLClassPath(URLClassLoader u)
			{
				return u.Ucp;
			}

			public virtual String GetOriginalHostName(InetAddress ia)
			{
				return ia.Holder_Renamed.OriginalHostName;
			}
		}
	}

	internal sealed class FactoryURLClassLoader : URLClassLoader
	{

		static FactoryURLClassLoader()
		{
			ClassLoader.RegisterAsParallelCapable();
		}

		internal FactoryURLClassLoader(URL[] urls, ClassLoader parent, AccessControlContext acc) : base(urls, parent, acc)
		{
		}

		internal FactoryURLClassLoader(URL[] urls, AccessControlContext acc) : base(urls, acc)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Class loadClass(String name, boolean resolve) throws ClassNotFoundException
		public override Class LoadClass(String name, bool resolve)
		{
			// First check if we have permission to access the package. This
			// should go away once we've added support for exported packages.
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				int i = name.LastIndexOf('.');
				if (i != -1)
				{
					sm.CheckPackageAccess(name.Substring(0, i));
				}
			}
			return base.LoadClass(name, resolve);
		}
	}

}