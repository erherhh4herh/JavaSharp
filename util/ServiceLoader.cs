using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{



	/// <summary>
	/// A simple service-provider loading facility.
	/// 
	/// <para> A <i>service</i> is a well-known set of interfaces and (usually
	/// abstract) classes.  A <i>service provider</i> is a specific implementation
	/// of a service.  The classes in a provider typically implement the interfaces
	/// and subclass the classes defined in the service itself.  Service providers
	/// can be installed in an implementation of the Java platform in the form of
	/// extensions, that is, jar files placed into any of the usual extension
	/// directories.  Providers can also be made available by adding them to the
	/// application's class path or by some other platform-specific means.
	/// 
	/// </para>
	/// <para> For the purpose of loading, a service is represented by a single type,
	/// that is, a single interface or abstract class.  (A concrete class can be
	/// used, but this is not recommended.)  A provider of a given service contains
	/// one or more concrete classes that extend this <i>service type</i> with data
	/// and code specific to the provider.  The <i>provider class</i> is typically
	/// not the entire provider itself but rather a proxy which contains enough
	/// information to decide whether the provider is able to satisfy a particular
	/// request together with code that can create the actual provider on demand.
	/// The details of provider classes tend to be highly service-specific; no
	/// single class or interface could possibly unify them, so no such type is
	/// defined here.  The only requirement enforced by this facility is that
	/// provider classes must have a zero-argument constructor so that they can be
	/// instantiated during loading.
	/// 
	/// </para>
	/// <para><a name="format"> A service provider is identified by placing a
	/// <i>provider-configuration file</i> in the resource directory
	/// <tt>META-INF/services</tt>.</a>  The file's name is the fully-qualified <a
	/// href="../lang/ClassLoader.html#name">binary name</a> of the service's type.
	/// The file contains a list of fully-qualified binary names of concrete
	/// provider classes, one per line.  Space and tab characters surrounding each
	/// name, as well as blank lines, are ignored.  The comment character is
	/// <tt>'#'</tt> (<tt>'&#92;u0023'</tt>,
	/// <font style="font-size:smaller;">NUMBER SIGN</font>); on
	/// each line all characters following the first comment character are ignored.
	/// The file must be encoded in UTF-8.
	/// 
	/// </para>
	/// <para> If a particular concrete provider class is named in more than one
	/// configuration file, or is named in the same configuration file more than
	/// once, then the duplicates are ignored.  The configuration file naming a
	/// particular provider need not be in the same jar file or other distribution
	/// unit as the provider itself.  The provider must be accessible from the same
	/// class loader that was initially queried to locate the configuration file;
	/// note that this is not necessarily the class loader from which the file was
	/// actually loaded.
	/// 
	/// </para>
	/// <para> Providers are located and instantiated lazily, that is, on demand.  A
	/// service loader maintains a cache of the providers that have been loaded so
	/// far.  Each invocation of the <seealso cref="#iterator iterator"/> method returns an
	/// iterator that first yields all of the elements of the cache, in
	/// instantiation order, and then lazily locates and instantiates any remaining
	/// providers, adding each one to the cache in turn.  The cache can be cleared
	/// via the <seealso cref="#reload reload"/> method.
	/// 
	/// </para>
	/// <para> Service loaders always execute in the security context of the caller.
	/// Trusted system code should typically invoke the methods in this class, and
	/// the methods of the iterators which they return, from within a privileged
	/// security context.
	/// 
	/// </para>
	/// <para> Instances of this class are not safe for use by multiple concurrent
	/// threads.
	/// 
	/// </para>
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method in this class will cause a <seealso cref="NullPointerException"/> to be thrown.
	/// 
	/// 
	/// </para>
	/// <para><span style="font-weight: bold; padding-right: 1em">Example</span>
	/// Suppose we have a service type <tt>com.example.CodecSet</tt> which is
	/// intended to represent sets of encoder/decoder pairs for some protocol.  In
	/// this case it is an abstract class with two abstract methods:
	/// 
	/// <blockquote><pre>
	/// public abstract Encoder getEncoder(String encodingName);
	/// public abstract Decoder getDecoder(String encodingName);</pre></blockquote>
	/// 
	/// Each method returns an appropriate object or <tt>null</tt> if the provider
	/// does not support the given encoding.  Typical providers support more than
	/// one encoding.
	/// 
	/// </para>
	/// <para> If <tt>com.example.impl.StandardCodecs</tt> is an implementation of the
	/// <tt>CodecSet</tt> service then its jar file also contains a file named
	/// 
	/// <blockquote><pre>
	/// META-INF/services/com.example.CodecSet</pre></blockquote>
	/// 
	/// </para>
	/// <para> This file contains the single line:
	/// 
	/// <blockquote><pre>
	/// com.example.impl.StandardCodecs    # Standard codecs</pre></blockquote>
	/// 
	/// </para>
	/// <para> The <tt>CodecSet</tt> class creates and saves a single service instance
	/// at initialization:
	/// 
	/// <blockquote><pre>
	/// private static ServiceLoader&lt;CodecSet&gt; codecSetLoader
	///     = ServiceLoader.load(CodecSet.class);</pre></blockquote>
	/// 
	/// </para>
	/// <para> To locate an encoder for a given encoding name it defines a static
	/// factory method which iterates through the known and available providers,
	/// returning only when it has located a suitable encoder or has run out of
	/// providers.
	/// 
	/// <blockquote><pre>
	/// public static Encoder getEncoder(String encodingName) {
	///     for (CodecSet cp : codecSetLoader) {
	///         Encoder enc = cp.getEncoder(encodingName);
	///         if (enc != null)
	///             return enc;
	///     }
	///     return null;
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para> A <tt>getDecoder</tt> method is defined similarly.
	/// 
	/// 
	/// </para>
	/// <para><span style="font-weight: bold; padding-right: 1em">Usage Note</span> If
	/// the class path of a class loader that is used for provider loading includes
	/// remote network URLs then those URLs will be dereferenced in the process of
	/// searching for provider-configuration files.
	/// 
	/// </para>
	/// <para> This activity is normal, although it may cause puzzling entries to be
	/// created in web-server logs.  If a web server is not configured correctly,
	/// however, then this activity may cause the provider-loading algorithm to fail
	/// spuriously.
	/// 
	/// </para>
	/// <para> A web server should return an HTTP 404 (Not Found) response when a
	/// requested resource does not exist.  Sometimes, however, web servers are
	/// erroneously configured to return an HTTP 200 (OK) response along with a
	/// helpful HTML error page in such cases.  This will cause a {@link
	/// ServiceConfigurationError} to be thrown when this class attempts to parse
	/// the HTML page as a provider-configuration file.  The best solution to this
	/// problem is to fix the misconfigured web server to return the correct
	/// response code (HTTP 404) along with the HTML error page.
	/// 
	/// </para>
	/// </summary>
	/// @param  <S>
	///         The type of the service to be loaded by this loader
	/// 
	/// @author Mark Reinhold
	/// @since 1.6 </param>

	public sealed class ServiceLoader<S> : Iterable<S>
	{

		private const String PREFIX = "META-INF/services/";

		// The class or interface representing the service being loaded
		private readonly Class Service;

		// The class loader used to locate, load, and instantiate providers
		private readonly ClassLoader Loader;

		// The access control context taken when the ServiceLoader is created
		private readonly AccessControlContext Acc;

		// Cached providers, in instantiation order
		private LinkedHashMap<String, S> Providers = new LinkedHashMap<String, S>();

		// The current lazy-lookup iterator
		private LazyIterator LookupIterator;

		/// <summary>
		/// Clear this loader's provider cache so that all providers will be
		/// reloaded.
		/// 
		/// <para> After invoking this method, subsequent invocations of the {@link
		/// #iterator() iterator} method will lazily look up and instantiate
		/// providers from scratch, just as is done by a newly-created loader.
		/// 
		/// </para>
		/// <para> This method is intended for use in situations in which new providers
		/// can be installed into a running Java virtual machine.
		/// </para>
		/// </summary>
		public void Reload()
		{
			Providers.Clear();
			LookupIterator = new LazyIterator(this, Service, Loader);
		}

		private ServiceLoader(Class svc, ClassLoader cl)
		{
			Service = Objects.RequireNonNull(svc, "Service interface cannot be null");
			Loader = (cl == null) ? ClassLoader.SystemClassLoader : cl;
			Acc = (System.SecurityManager != null) ? AccessController.Context : null;
			Reload();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void fail(Class service, String msg, Throwable cause) throws ServiceConfigurationError
		private static void Fail(Class service, String msg, Throwable cause)
		{
			throw new ServiceConfigurationError(service.Name + ": " + msg, cause);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void fail(Class service, String msg) throws ServiceConfigurationError
		private static void Fail(Class service, String msg)
		{
			throw new ServiceConfigurationError(service.Name + ": " + msg);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void fail(Class service, java.net.URL u, int line, String msg) throws ServiceConfigurationError
		private static void Fail(Class service, URL u, int line, String msg)
		{
			Fail(service, u + ":" + line + ": " + msg);
		}

		// Parse a single line from the given configuration file, adding the name
		// on the line to the names list.
		//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int parseLine(Class service, java.net.URL u, java.io.BufferedReader r, int lc, java.util.List<String> names) throws java.io.IOException, ServiceConfigurationError
		private int ParseLine(Class service, URL u, BufferedReader r, int lc, IList<String> names)
		{
			String ln = r.ReadLine();
			if (ln == null)
			{
				return -1;
			}
			int ci = ln.IndexOf('#');
			if (ci >= 0)
			{
				ln = ln.Substring(0, ci);
			}
			ln = ln.Trim();
			int n = ln.Length();
			if (n != 0)
			{
				if ((ln.IndexOf(' ') >= 0) || (ln.IndexOf('\t') >= 0))
				{
					Fail(service, u, lc, "Illegal configuration-file syntax");
				}
				int cp = ln.CodePointAt(0);
				if (!Character.IsJavaIdentifierStart(cp))
				{
					Fail(service, u, lc, "Illegal provider-class name: " + ln);
				}
				for (int i = Character.CharCount(cp); i < n; i += Character.CharCount(cp))
				{
					cp = ln.CodePointAt(i);
					if (!Character.IsJavaIdentifierPart(cp) && (cp != '.'))
					{
						Fail(service, u, lc, "Illegal provider-class name: " + ln);
					}
				}
				if (!Providers.ContainsKey(ln) && !names.Contains(ln))
				{
					names.Add(ln);
				}
			}
			return lc + 1;
		}

		// Parse the content of the given URL as a provider-configuration file.
		//
		// @param  service
		//         The service type for which providers are being sought;
		//         used to construct error detail strings
		//
		// @param  u
		//         The URL naming the configuration file to be parsed
		//
		// @return A (possibly empty) iterator that will yield the provider-class
		//         names in the given configuration file that are not yet members
		//         of the returned set
		//
		// @throws ServiceConfigurationError
		//         If an I/O error occurs while reading from the given URL, or
		//         if a configuration-file format error is detected
		//
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.Iterator<String> parse(Class service, java.net.URL u) throws ServiceConfigurationError
		private IEnumerator<String> Parse(Class service, URL u)
		{
			InputStream @in = null;
			BufferedReader r = null;
			List<String> names = new List<String>();
			try
			{
				@in = u.OpenStream();
				r = new BufferedReader(new InputStreamReader(@in, "utf-8"));
				int lc = 1;
				while ((lc = ParseLine(service, u, r, lc, names)) >= 0);
			}
		catch (IOException x)
		{
				Fail(service, "Error reading configuration file", x);
		}
			finally
			{
				try
				{
					if (r != null)
					{
						r.Close();
					}
					if (@in != null)
					{
						@in.Close();
					}
				}
				catch (IOException y)
				{
					Fail(service, "Error closing configuration file", y);
				}
			}
			return names.Iterator();
		}

		// Private inner class implementing fully-lazy provider lookup
		//
		private class LazyIterator : Iterator<S>
		{
			private readonly ServiceLoader<S> OuterInstance;


			internal Class Service;
			internal ClassLoader Loader;
			internal IEnumerator<URL> Configs = null;
			internal IEnumerator<String> Pending = null;
			internal String NextName = null;

			internal LazyIterator(ServiceLoader<S> outerInstance, Class service, ClassLoader loader)
			{
				this.OuterInstance = outerInstance;
				this.Service = service;
				this.Loader = loader;
			}

			internal virtual bool HasNextService()
			{
				if (NextName != null)
				{
					return true;
				}
				if (Configs == null)
				{
					try
					{
						String fullName = PREFIX + Service.Name;
						if (Loader == null)
						{
							Configs = ClassLoader.GetSystemResources(fullName);
						}
						else
						{
							Configs = Loader.GetResources(fullName);
						}
					}
					catch (IOException x)
					{
						Fail(Service, "Error locating configuration files", x);
					}
				}
				while ((Pending == null) || !Pending.MoveNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (!Configs.hasMoreElements())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Pending = outerInstance.Parse(Service, Configs.nextElement());
				}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				NextName = Pending.next();
				return true;
			}

			internal virtual S NextService()
			{
				if (!HasNextService())
				{
					throw new NoSuchElementException();
				}
				String cn = NextName;
				NextName = null;
				Class c = null;
				try
				{
					c = Class.ForName(cn, false, Loader);
				}
				catch (ClassNotFoundException)
				{
					Fail(Service, "Provider " + cn + " not found");
				}
				if (!c.IsSubclassOf(Service))
				{
					Fail(Service, "Provider " + cn + " not a subtype");
				}
				try
				{
					S p = Service.Cast(c.NewInstance());
					outerInstance.Providers.Put(cn, p);
					return p;
				}
				catch (Throwable x)
				{
					Fail(Service, "Provider " + cn + " could not be instantiated", x);
				}
				throw new Error(); // This cannot happen
			}

			public virtual bool HasNext()
			{
				if (outerInstance.Acc == null)
				{
					return HasNextService();
				}
				else
				{
					PrivilegedAction<Boolean> action = new PrivilegedActionAnonymousInnerClassHelper(this);
					return AccessController.doPrivileged(action, outerInstance.Acc);
				}
			}

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
			{
				private readonly LazyIterator OuterInstance;

				public PrivilegedActionAnonymousInnerClassHelper(LazyIterator outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual Boolean Run()
				{
					return outerInstance.HasNextService();
				}
			}

			public virtual S Next()
			{
				if (outerInstance.Acc == null)
				{
					return NextService();
				}
				else
				{
					PrivilegedAction<S> action = new PrivilegedActionAnonymousInnerClassHelper2(this);
					return AccessController.doPrivileged(action, outerInstance.Acc);
				}
			}

			private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<S>
			{
				private readonly LazyIterator OuterInstance;

				public PrivilegedActionAnonymousInnerClassHelper2(LazyIterator outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual S Run()
				{
					return outerInstance.NextService();
				}
			}

			public virtual void Remove()
			{
				throw new UnsupportedOperationException();
			}

		}

		/// <summary>
		/// Lazily loads the available providers of this loader's service.
		/// 
		/// <para> The iterator returned by this method first yields all of the
		/// elements of the provider cache, in instantiation order.  It then lazily
		/// loads and instantiates any remaining providers, adding each one to the
		/// cache in turn.
		/// 
		/// </para>
		/// <para> To achieve laziness the actual work of parsing the available
		/// provider-configuration files and instantiating providers must be done by
		/// the iterator itself.  Its <seealso cref="java.util.Iterator#hasNext hasNext"/> and
		/// <seealso cref="java.util.Iterator#next next"/> methods can therefore throw a
		/// <seealso cref="ServiceConfigurationError"/> if a provider-configuration file
		/// violates the specified format, or if it names a provider class that
		/// cannot be found and instantiated, or if the result of instantiating the
		/// class is not assignable to the service type, or if any other kind of
		/// exception or error is thrown as the next provider is located and
		/// instantiated.  To write robust code it is only necessary to catch {@link
		/// ServiceConfigurationError} when using a service iterator.
		/// 
		/// </para>
		/// <para> If such an error is thrown then subsequent invocations of the
		/// iterator will make a best effort to locate and instantiate the next
		/// available provider, but in general such recovery cannot be guaranteed.
		/// 
		/// <blockquote style="font-size: smaller; line-height: 1.2"><span
		/// style="padding-right: 1em; font-weight: bold">Design Note</span>
		/// Throwing an error in these cases may seem extreme.  The rationale for
		/// this behavior is that a malformed provider-configuration file, like a
		/// malformed class file, indicates a serious problem with the way the Java
		/// virtual machine is configured or is being used.  As such it is
		/// preferable to throw an error rather than try to recover or, even worse,
		/// fail silently.</blockquote>
		/// 
		/// </para>
		/// <para> The iterator returned by this method does not support removal.
		/// Invoking its <seealso cref="java.util.Iterator#remove() remove"/> method will
		/// cause an <seealso cref="UnsupportedOperationException"/> to be thrown.
		/// 
		/// @implNote When adding providers to the cache, the {@link #iterator
		/// Iterator} processes resources in the order that the {@link
		/// java.lang.ClassLoader#getResources(java.lang.String)
		/// ClassLoader.getResources(String)} method finds the service configuration
		/// files.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  An iterator that lazily loads providers for this loader's
		///          service </returns>
		public IEnumerator<S> GetEnumerator()
		{
			return new IteratorAnonymousInnerClassHelper(this);
		}

		private class IteratorAnonymousInnerClassHelper : Iterator<S>
		{
			private readonly ServiceLoader<S> OuterInstance;

			public IteratorAnonymousInnerClassHelper(ServiceLoader<S> outerInstance)
			{
				this.outerInstance = outerInstance;
				knownProviders = outerInstance.Providers.EntrySet().Iterator();
			}


			internal IEnumerator<Map_Entry<String, S>> knownProviders;

			public virtual bool HasNext()
			{
				if (knownProviders.hasNext())
				{
					return true;
				}
				return OuterInstance.LookupIterator.HasNext();
			}

			public virtual S Next()
			{
				if (knownProviders.hasNext())
				{
					return knownProviders.next().Value;
				}
				return OuterInstance.LookupIterator.Next();
			}

			public virtual void Remove()
			{
				throw new UnsupportedOperationException();
			}

		}

		/// <summary>
		/// Creates a new service loader for the given service type and class
		/// loader.
		/// </summary>
		/// @param  <S> the class of the service type
		/// </param>
		/// <param name="service">
		///         The interface or abstract class representing the service
		/// </param>
		/// <param name="loader">
		///         The class loader to be used to load provider-configuration files
		///         and provider classes, or <tt>null</tt> if the system class
		///         loader (or, failing that, the bootstrap class loader) is to be
		///         used
		/// </param>
		/// <returns> A new service loader </returns>
		public static ServiceLoader<S> load<S>(Class service, ClassLoader loader)
		{
			return new ServiceLoader<>(service, loader);
		}

		/// <summary>
		/// Creates a new service loader for the given service type, using the
		/// current thread's {@link java.lang.Thread#getContextClassLoader
		/// context class loader}.
		/// 
		/// <para> An invocation of this convenience method of the form
		/// 
		/// <blockquote><pre>
		/// ServiceLoader.load(<i>service</i>)</pre></blockquote>
		/// 
		/// is equivalent to
		/// 
		/// <blockquote><pre>
		/// ServiceLoader.load(<i>service</i>,
		///                    Thread.currentThread().getContextClassLoader())</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// @param  <S> the class of the service type
		/// </param>
		/// <param name="service">
		///         The interface or abstract class representing the service
		/// </param>
		/// <returns> A new service loader </returns>
		public static ServiceLoader<S> load<S>(Class service)
		{
			ClassLoader cl = Thread.CurrentThread.ContextClassLoader;
			return ServiceLoader.Load(service, cl);
		}

		/// <summary>
		/// Creates a new service loader for the given service type, using the
		/// extension class loader.
		/// 
		/// <para> This convenience method simply locates the extension class loader,
		/// call it <tt><i>extClassLoader</i></tt>, and then returns
		/// 
		/// <blockquote><pre>
		/// ServiceLoader.load(<i>service</i>, <i>extClassLoader</i>)</pre></blockquote>
		/// 
		/// </para>
		/// <para> If the extension class loader cannot be found then the system class
		/// loader is used; if there is no system class loader then the bootstrap
		/// class loader is used.
		/// 
		/// </para>
		/// <para> This method is intended for use when only installed providers are
		/// desired.  The resulting service will only find and load providers that
		/// have been installed into the current Java virtual machine; providers on
		/// the application's class path will be ignored.
		/// 
		/// </para>
		/// </summary>
		/// @param  <S> the class of the service type
		/// </param>
		/// <param name="service">
		///         The interface or abstract class representing the service
		/// </param>
		/// <returns> A new service loader </returns>
		public static ServiceLoader<S> loadInstalled<S>(Class service)
		{
			ClassLoader cl = ClassLoader.SystemClassLoader;
			ClassLoader prev = null;
			while (cl != null)
			{
				prev = cl;
				cl = cl.Parent;
			}
			return ServiceLoader.Load(service, prev);
		}

		/// <summary>
		/// Returns a string describing this service.
		/// </summary>
		/// <returns>  A descriptive string </returns>
		public override String ToString()
		{
			return "java.util.ServiceLoader[" + Service.Name + "]";
		}

	}

}