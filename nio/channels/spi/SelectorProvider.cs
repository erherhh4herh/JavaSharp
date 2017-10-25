using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using GetPropertyAction = sun.security.action.GetPropertyAction;


	/// <summary>
	/// Service-provider class for selectors and selectable channels.
	/// 
	/// <para> A selector provider is a concrete subclass of this class that has a
	/// zero-argument constructor and implements the abstract methods specified
	/// below.  A given invocation of the Java virtual machine maintains a single
	/// system-wide default provider instance, which is returned by the {@link
	/// #provider() provider} method.  The first invocation of that method will locate
	/// the default provider as specified below.
	/// 
	/// </para>
	/// <para> The system-wide default provider is used by the static <tt>open</tt>
	/// methods of the {@link java.nio.channels.DatagramChannel#open
	/// DatagramChannel}, <seealso cref="java.nio.channels.Pipe#open Pipe"/>, {@link
	/// java.nio.channels.Selector#open Selector}, {@link
	/// java.nio.channels.ServerSocketChannel#open ServerSocketChannel}, and {@link
	/// java.nio.channels.SocketChannel#open SocketChannel} classes.  It is also
	/// used by the <seealso cref="java.lang.System#inheritedChannel System.inheritedChannel()"/>
	/// method. A program may make use of a provider other than the default provider
	/// by instantiating that provider and then directly invoking the <tt>open</tt>
	/// methods defined in this class.
	/// 
	/// </para>
	/// <para> All of the methods in this class are safe for use by multiple concurrent
	/// threads.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class SelectorProvider
	{

		private static readonly Object @lock = new Object();
		private static SelectorProvider Provider_Renamed = null;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("selectorProvider")</tt> </exception>
		protected internal SelectorProvider()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new RuntimePermission("selectorProvider"));
			}
		}

		private static bool LoadProviderFromProperty()
		{
			String cn = System.getProperty("java.nio.channels.spi.SelectorProvider");
			if (cn == null)
			{
				return false;
			}
			try
			{
				Class c = Class.ForName(cn, true, ClassLoader.SystemClassLoader);
				Provider_Renamed = (SelectorProvider)c.NewInstance();
				return true;
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

		private static bool LoadProviderAsService()
		{

			ServiceLoader<SelectorProvider> sl = ServiceLoader.Load(typeof(SelectorProvider), ClassLoader.SystemClassLoader);
			IEnumerator<SelectorProvider> i = sl.Iterator();
			for (;;)
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (!i.hasNext())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Provider_Renamed = i.next();
					return true;
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

		/// <summary>
		/// Returns the system-wide default selector provider for this invocation of
		/// the Java virtual machine.
		/// 
		/// <para> The first invocation of this method locates the default provider
		/// object as follows: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> If the system property
		///   <tt>java.nio.channels.spi.SelectorProvider</tt> is defined then it is
		///   taken to be the fully-qualified name of a concrete provider class.
		///   The class is loaded and instantiated; if this process fails then an
		///   unspecified error is thrown.  </para></li>
		/// 
		///   <li><para> If a provider class has been installed in a jar file that is
		///   visible to the system class loader, and that jar file contains a
		///   provider-configuration file named
		///   <tt>java.nio.channels.spi.SelectorProvider</tt> in the resource
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
		/// <returns>  The system-wide default selector provider </returns>
		public static SelectorProvider Provider()
		{
			lock (@lock)
			{
				if (Provider_Renamed != null)
				{
					return Provider_Renamed;
				}
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<SelectorProvider>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual SelectorProvider Run()
			{
					if (LoadProviderFromProperty())
					{
						return Provider_Renamed;
					}
					if (LoadProviderAsService())
					{
						return Provider_Renamed;
					}
					Provider_Renamed = sun.nio.ch.DefaultSelectorProvider.create();
					return Provider_Renamed;
			}
		}

		/// <summary>
		/// Opens a datagram channel.
		/// </summary>
		/// <returns>  The new channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract DatagramChannel openDatagramChannel() throws java.io.IOException;
		public abstract DatagramChannel OpenDatagramChannel();

		/// <summary>
		/// Opens a datagram channel.
		/// </summary>
		/// <param name="family">
		///          The protocol family
		/// </param>
		/// <returns>  A new datagram channel
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If the specified protocol family is not supported </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract DatagramChannel openDatagramChannel(java.net.ProtocolFamily family) throws java.io.IOException;
		public abstract DatagramChannel OpenDatagramChannel(ProtocolFamily family);

		/// <summary>
		/// Opens a pipe.
		/// </summary>
		/// <returns>  The new pipe
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Pipe openPipe() throws java.io.IOException;
		public abstract Pipe OpenPipe();

		/// <summary>
		/// Opens a selector.
		/// </summary>
		/// <returns>  The new selector
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract AbstractSelector openSelector() throws java.io.IOException;
		public abstract AbstractSelector OpenSelector();

		/// <summary>
		/// Opens a server-socket channel.
		/// </summary>
		/// <returns>  The new channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract ServerSocketChannel openServerSocketChannel() throws java.io.IOException;
		public abstract ServerSocketChannel OpenServerSocketChannel();

		/// <summary>
		/// Opens a socket channel.
		/// </summary>
		/// <returns>  The new channel
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract SocketChannel openSocketChannel() throws java.io.IOException;
		public abstract SocketChannel OpenSocketChannel();

		/// <summary>
		/// Returns the channel inherited from the entity that created this
		/// Java virtual machine.
		/// 
		/// <para> On many operating systems a process, such as a Java virtual
		/// machine, can be started in a manner that allows the process to
		/// inherit a channel from the entity that created the process. The
		/// manner in which this is done is system dependent, as are the
		/// possible entities to which the channel may be connected. For example,
		/// on UNIX systems, the Internet services daemon (<i>inetd</i>) is used to
		/// start programs to service requests when a request arrives on an
		/// associated network port. In this example, the process that is started,
		/// inherits a channel representing a network socket.
		/// 
		/// </para>
		/// <para> In cases where the inherited channel represents a network socket
		/// then the <seealso cref="java.nio.channels.Channel Channel"/> type returned
		/// by this method is determined as follows:
		/// 
		/// <ul>
		/// 
		/// </para>
		///  <li><para> If the inherited channel represents a stream-oriented connected
		///  socket then a <seealso cref="java.nio.channels.SocketChannel SocketChannel"/> is
		///  returned. The socket channel is, at least initially, in blocking
		///  mode, bound to a socket address, and connected to a peer.
		///  </para></li>
		/// 
		///  <li><para> If the inherited channel represents a stream-oriented listening
		///  socket then a {@link java.nio.channels.ServerSocketChannel
		///  ServerSocketChannel} is returned. The server-socket channel is, at
		///  least initially, in blocking mode, and bound to a socket address.
		///  </para></li>
		/// 
		///  <li><para> If the inherited channel is a datagram-oriented socket
		///  then a <seealso cref="java.nio.channels.DatagramChannel DatagramChannel"/> is
		///  returned. The datagram channel is, at least initially, in blocking
		///  mode, and bound to a socket address.
		///  </para></li>
		/// 
		/// </ul>
		/// 
		/// <para> In addition to the network-oriented channels described, this method
		/// may return other kinds of channels in the future.
		/// 
		/// </para>
		/// <para> The first invocation of this method creates the channel that is
		/// returned. Subsequent invocations of this method return the same
		/// channel. </para>
		/// </summary>
		/// <returns>  The inherited channel, if any, otherwise <tt>null</tt>.
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager has been installed and it denies
		///          <seealso cref="RuntimePermission"/><tt>("inheritedChannel")</tt>
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Channel inheritedChannel() throws java.io.IOException
	   public virtual Channel InheritedChannel()
	   {
			return null;
	   }

	}

}