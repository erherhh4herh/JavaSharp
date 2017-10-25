using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels.spi
{


	/// <summary>
	/// Service-provider class for asynchronous channels.
	/// 
	/// <para> An asynchronous channel provider is a concrete subclass of this class that
	/// has a zero-argument constructor and implements the abstract methods specified
	/// below.  A given invocation of the Java virtual machine maintains a single
	/// system-wide default provider instance, which is returned by the {@link
	/// #provider() provider} method.  The first invocation of that method will locate
	/// the default provider as specified below.
	/// 
	/// </para>
	/// <para> All of the methods in this class are safe for use by multiple concurrent
	/// threads.  </para>
	/// 
	/// @since 1.7
	/// </summary>

	public abstract class AsynchronousChannelProvider
	{
		private static Void CheckPermission()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new RuntimePermission("asynchronousChannelProvider"));
			}
			return null;
		}
		private AsynchronousChannelProvider(Void ignore)
		{
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("asynchronousChannelProvider")</tt> </exception>
		protected internal AsynchronousChannelProvider() : this(CheckPermission())
		{
		}

		// lazy initialization of default provider
		private class ProviderHolder
		{
			internal static readonly AsynchronousChannelProvider Provider = Load();

			internal static AsynchronousChannelProvider Load()
			{
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<AsynchronousChannelProvider>
			{
				public PrivilegedActionAnonymousInnerClassHelper()
				{
				}

				public virtual AsynchronousChannelProvider Run()
				{
					AsynchronousChannelProvider p;
					p = LoadProviderFromProperty();
					if (p != null)
					{
						return p;
					}
					p = LoadProviderAsService();
					if (p != null)
					{
						return p;
					}
					return sun.nio.ch.DefaultAsynchronousChannelProvider.create();
				}
			}

			internal static AsynchronousChannelProvider LoadProviderFromProperty()
			{
				String cn = System.getProperty("java.nio.channels.spi.AsynchronousChannelProvider");
				if (cn == null)
				{
					return null;
				}
				try
				{
					Class c = Class.ForName(cn, true, ClassLoader.SystemClassLoader);
					return (AsynchronousChannelProvider)c.NewInstance();
				}
				catch (ClassNotFoundException x)
				{
					throw new ServiceConfigurationError(null, x);
				}
				catch (IllegalAccessException x)
				{
					throw new ServiceConfigurationError(null, x);
				}
				catch (InstantiationException x)
				{
					throw new ServiceConfigurationError(null, x);
				}
				catch (SecurityException x)
				{
					throw new ServiceConfigurationError(null, x);
				}
			}

			internal static AsynchronousChannelProvider LoadProviderAsService()
			{
				ServiceLoader<AsynchronousChannelProvider> sl = ServiceLoader.Load(typeof(AsynchronousChannelProvider), ClassLoader.SystemClassLoader);
				IEnumerator<AsynchronousChannelProvider> i = sl.Iterator();
				for (;;)
				{
					try
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						return (i.hasNext()) ? i.next() : null;
					}
					catch (ServiceConfigurationError sce)
					{
						if (sce.Cause is SecurityException)
						{
							// Ignore the security exception, try the next provider
							continue;
						}
						throw sce;
					}
				}
			}
		}

		/// <summary>
		/// Returns the system-wide default asynchronous channel provider for this
		/// invocation of the Java virtual machine.
		/// 
		/// <para> The first invocation of this method locates the default provider
		/// object as follows: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> If the system property
		///   <tt>java.nio.channels.spi.AsynchronousChannelProvider</tt> is defined
		///   then it is taken to be the fully-qualified name of a concrete provider class.
		///   The class is loaded and instantiated; if this process fails then an
		///   unspecified error is thrown.  </para></li>
		/// 
		///   <li><para> If a provider class has been installed in a jar file that is
		///   visible to the system class loader, and that jar file contains a
		///   provider-configuration file named
		///   <tt>java.nio.channels.spi.AsynchronousChannelProvider</tt> in the resource
		///   directory <tt>META-INF/services</tt>, then the first class name
		///   specified in that file is taken.  The class is loaded and
		///   instantiated; if this process fails then an unspecified error is
		///   thrown.  </para></li>
		/// 
		///   <li><para> Finally, if no provider has been specified by any of the above
		///   means then the system-default provider class is instantiated and the
		///   result is returned.  </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> Subsequent invocations of this method return the provider that was
		/// returned by the first invocation.  </para>
		/// </summary>
		/// <returns>  The system-wide default AsynchronousChannel provider </returns>
		public static AsynchronousChannelProvider Provider()
		{
			return ProviderHolder.Provider;
		}

		/// <summary>
		/// Constructs a new asynchronous channel group with a fixed thread pool.
		/// </summary>
		/// <param name="nThreads">
		///          The number of threads in the pool </param>
		/// <param name="threadFactory">
		///          The factory to use when creating new threads
		/// </param>
		/// <returns>  A new asynchronous channel group
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If {@code nThreads <= 0} </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <seealso cref= AsynchronousChannelGroup#withFixedThreadPool </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AsynchronousChannelGroup openAsynchronousChannelGroup(int nThreads, ThreadFactory threadFactory) throws java.io.IOException;
		public abstract AsynchronousChannelGroup OpenAsynchronousChannelGroup(int nThreads, ThreadFactory threadFactory);

		/// <summary>
		/// Constructs a new asynchronous channel group with the given thread pool.
		/// </summary>
		/// <param name="executor">
		///          The thread pool </param>
		/// <param name="initialSize">
		///          A value {@code >=0} or a negative value for implementation
		///          specific default
		/// </param>
		/// <returns>  A new asynchronous channel group
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <seealso cref= AsynchronousChannelGroup#withCachedThreadPool </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AsynchronousChannelGroup openAsynchronousChannelGroup(ExecutorService executor, int initialSize) throws java.io.IOException;
		public abstract AsynchronousChannelGroup OpenAsynchronousChannelGroup(ExecutorService executor, int initialSize);

		/// <summary>
		/// Opens an asynchronous server-socket channel.
		/// </summary>
		/// <param name="group">
		///          The group to which the channel is bound, or {@code null} to
		///          bind to the default group
		/// </param>
		/// <returns>  The new channel
		/// </returns>
		/// <exception cref="IllegalChannelGroupException">
		///          If the provider that created the group differs from this provider </exception>
		/// <exception cref="ShutdownChannelGroupException">
		///          The group is shutdown </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AsynchronousServerSocketChannel openAsynchronousServerSocketChannel(AsynchronousChannelGroup group) throws java.io.IOException;
		public abstract AsynchronousServerSocketChannel OpenAsynchronousServerSocketChannel(AsynchronousChannelGroup group);

		/// <summary>
		/// Opens an asynchronous socket channel.
		/// </summary>
		/// <param name="group">
		///          The group to which the channel is bound, or {@code null} to
		///          bind to the default group
		/// </param>
		/// <returns>  The new channel
		/// </returns>
		/// <exception cref="IllegalChannelGroupException">
		///          If the provider that created the group differs from this provider </exception>
		/// <exception cref="ShutdownChannelGroupException">
		///          The group is shutdown </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AsynchronousSocketChannel openAsynchronousSocketChannel(AsynchronousChannelGroup group) throws java.io.IOException;
		public abstract AsynchronousSocketChannel OpenAsynchronousSocketChannel(AsynchronousChannelGroup group);
	}

}