/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi
{


	/// <summary>
	/// The <code>Naming</code> class provides methods for storing and obtaining
	/// references to remote objects in a remote object registry.  Each method of
	/// the <code>Naming</code> class takes as one of its arguments a name that
	/// is a <code>java.lang.String</code> in URL format (without the
	/// scheme component) of the form:
	/// 
	/// <PRE>
	///    //host:port/name
	/// </PRE>
	/// 
	/// <P>where <code>host</code> is the host (remote or local) where the registry
	/// is located, <code>port</code> is the port number on which the registry
	/// accepts calls, and where <code>name</code> is a simple string uninterpreted
	/// by the registry. Both <code>host</code> and <code>port</code> are optional.
	/// If <code>host</code> is omitted, the host defaults to the local host. If
	/// <code>port</code> is omitted, then the port defaults to 1099, the
	/// "well-known" port that RMI's registry, <code>rmiregistry</code>, uses.
	/// 
	/// <P><em>Binding</em> a name for a remote object is associating or
	/// registering a name for a remote object that can be used at a later time to
	/// look up that remote object.  A remote object can be associated with a name
	/// using the <code>Naming</code> class's <code>bind</code> or
	/// <code>rebind</code> methods.
	/// 
	/// <P>Once a remote object is registered (bound) with the RMI registry on the
	/// local host, callers on a remote (or local) host can lookup the remote
	/// object by name, obtain its reference, and then invoke remote methods on the
	/// object.  A registry may be shared by all servers running on a host or an
	/// individual server process may create and use its own registry if desired
	/// (see <code>java.rmi.registry.LocateRegistry.createRegistry</code> method
	/// for details).
	/// 
	/// @author  Ann Wollrath
	/// @author  Roger Riggs
	/// @since   JDK1.1 </summary>
	/// <seealso cref=     java.rmi.registry.Registry </seealso>
	/// <seealso cref=     java.rmi.registry.LocateRegistry </seealso>
	/// <seealso cref=     java.rmi.registry.LocateRegistry#createRegistry(int) </seealso>
	public sealed class Naming
	{
		/// <summary>
		/// Disallow anyone from creating one of these
		/// </summary>
		private Naming()
		{
		}

		/// <summary>
		/// Returns a reference, a stub, for the remote object associated
		/// with the specified <code>name</code>.
		/// </summary>
		/// <param name="name"> a name in URL format (without the scheme component) </param>
		/// <returns> a reference for a remote object </returns>
		/// <exception cref="NotBoundException"> if name is not currently bound </exception>
		/// <exception cref="RemoteException"> if registry could not be contacted </exception>
		/// <exception cref="AccessException"> if this operation is not permitted </exception>
		/// <exception cref="MalformedURLException"> if the name is not an appropriately
		///  formatted URL
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Remote lookup(String name) throws NotBoundException, java.net.MalformedURLException, RemoteException
		public static Remote Lookup(String name)
		{
			ParsedNamingURL parsed = ParseURL(name);
			Registry registry = GetRegistry(parsed);

			if (parsed.Name == null)
			{
				return registry;
			}
			return registry.Lookup(parsed.Name);
		}

		/// <summary>
		/// Binds the specified <code>name</code> to a remote object.
		/// </summary>
		/// <param name="name"> a name in URL format (without the scheme component) </param>
		/// <param name="obj"> a reference for the remote object (usually a stub) </param>
		/// <exception cref="AlreadyBoundException"> if name is already bound </exception>
		/// <exception cref="MalformedURLException"> if the name is not an appropriately
		///  formatted URL </exception>
		/// <exception cref="RemoteException"> if registry could not be contacted </exception>
		/// <exception cref="AccessException"> if this operation is not permitted (if
		/// originating from a non-local host, for example)
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void bind(String name, Remote obj) throws AlreadyBoundException, java.net.MalformedURLException, RemoteException
		public static void Bind(String name, Remote obj)
		{
			ParsedNamingURL parsed = ParseURL(name);
			Registry registry = GetRegistry(parsed);

			if (obj == null)
			{
				throw new NullPointerException("cannot bind to null");
			}

			registry.Bind(parsed.Name, obj);
		}

		/// <summary>
		/// Destroys the binding for the specified name that is associated
		/// with a remote object.
		/// </summary>
		/// <param name="name"> a name in URL format (without the scheme component) </param>
		/// <exception cref="NotBoundException"> if name is not currently bound </exception>
		/// <exception cref="MalformedURLException"> if the name is not an appropriately
		///  formatted URL </exception>
		/// <exception cref="RemoteException"> if registry could not be contacted </exception>
		/// <exception cref="AccessException"> if this operation is not permitted (if
		/// originating from a non-local host, for example)
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void unbind(String name) throws RemoteException, NotBoundException, java.net.MalformedURLException
		public static void Unbind(String name)
		{
			ParsedNamingURL parsed = ParseURL(name);
			Registry registry = GetRegistry(parsed);

			registry.Unbind(parsed.Name);
		}

		/// <summary>
		/// Rebinds the specified name to a new remote object. Any existing
		/// binding for the name is replaced.
		/// </summary>
		/// <param name="name"> a name in URL format (without the scheme component) </param>
		/// <param name="obj"> new remote object to associate with the name </param>
		/// <exception cref="MalformedURLException"> if the name is not an appropriately
		///  formatted URL </exception>
		/// <exception cref="RemoteException"> if registry could not be contacted </exception>
		/// <exception cref="AccessException"> if this operation is not permitted (if
		/// originating from a non-local host, for example)
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void rebind(String name, Remote obj) throws RemoteException, java.net.MalformedURLException
		public static void Rebind(String name, Remote obj)
		{
			ParsedNamingURL parsed = ParseURL(name);
			Registry registry = GetRegistry(parsed);

			if (obj == null)
			{
				throw new NullPointerException("cannot bind to null");
			}

			registry.Rebind(parsed.Name, obj);
		}

		/// <summary>
		/// Returns an array of the names bound in the registry.  The names are
		/// URL-formatted (without the scheme component) strings. The array contains
		/// a snapshot of the names present in the registry at the time of the
		/// call.
		/// </summary>
		/// <param name="name"> a registry name in URL format (without the scheme
		///          component) </param>
		/// <returns>  an array of names (in the appropriate format) bound
		///          in the registry </returns>
		/// <exception cref="MalformedURLException"> if the name is not an appropriately
		///  formatted URL </exception>
		/// <exception cref="RemoteException"> if registry could not be contacted.
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String[] list(String name) throws RemoteException, java.net.MalformedURLException
		public static String[] List(String name)
		{
			ParsedNamingURL parsed = ParseURL(name);
			Registry registry = GetRegistry(parsed);

			String prefix = "";
			if (parsed.Port > 0 || !parsed.Host.Equals(""))
			{
				prefix += "//" + parsed.Host;
			}
			if (parsed.Port > 0)
			{
				prefix += ":" + parsed.Port;
			}
			prefix += "/";

			String[] names = registry.List();
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = prefix + names[i];
			}
			return names;
		}

		/// <summary>
		/// Returns a registry reference obtained from information in the URL.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Registry getRegistry(ParsedNamingURL parsed) throws RemoteException
		private static Registry GetRegistry(ParsedNamingURL parsed)
		{
			return LocateRegistry.GetRegistry(parsed.Host, parsed.Port);
		}

		/// <summary>
		/// Dissect Naming URL strings to obtain referenced host, port and
		/// object name.
		/// </summary>
		/// <returns> an object which contains each of the above
		/// components.
		/// </returns>
		/// <exception cref="MalformedURLException"> if given url string is malformed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static ParsedNamingURL parseURL(String str) throws java.net.MalformedURLException
		private static ParsedNamingURL ParseURL(String str)
		{
			try
			{
				return IntParseURL(str);
			}
			catch (URISyntaxException ex)
			{
				/* With RFC 3986 URI handling, 'rmi://:<port>' and
				 * '//:<port>' forms will result in a URI syntax exception
				 * Convert the authority to a localhost:<port> form
				 */
				MalformedURLException mue = new MalformedURLException("invalid URL String: " + str);
				mue.InitCause(ex);
				int indexSchemeEnd = str.IndexOf(':');
				int indexAuthorityBegin = str.IndexOf("//:");
				if (indexAuthorityBegin < 0)
				{
					throw mue;
				}
				if ((indexAuthorityBegin == 0) || ((indexSchemeEnd > 0) && (indexAuthorityBegin == indexSchemeEnd + 1)))
				{
					int indexHostBegin = indexAuthorityBegin + 2;
					String newStr = str.Substring(0, indexHostBegin) + "localhost" + str.Substring(indexHostBegin);
					try
					{
						return IntParseURL(newStr);
					}
					catch (URISyntaxException)
					{
						throw mue;
					}
					catch (MalformedURLException inte)
					{
						throw inte;
					}
				}
				throw mue;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static ParsedNamingURL intParseURL(String str) throws java.net.MalformedURLException, java.net.URISyntaxException
		private static ParsedNamingURL IntParseURL(String str)
		{
			URI uri = new URI(str);
			if (uri.Opaque)
			{
				throw new MalformedURLException("not a hierarchical URL: " + str);
			}
			if (uri.Fragment != null)
			{
				throw new MalformedURLException("invalid character, '#', in URL name: " + str);
			}
			else if (uri.Query != null)
			{
				throw new MalformedURLException("invalid character, '?', in URL name: " + str);
			}
			else if (uri.UserInfo != null)
			{
				throw new MalformedURLException("invalid character, '@', in URL host: " + str);
			}
			String scheme = uri.Scheme;
			if (scheme != null && !scheme.Equals("rmi"))
			{
				throw new MalformedURLException("invalid URL scheme: " + str);
			}

			String name = uri.Path;
			if (name != null)
			{
				if (name.StartsWith("/"))
				{
					name = name.Substring(1);
				}
				if (name.Length() == 0)
				{
					name = null;
				}
			}

			String host = uri.Host;
			if (host == null)
			{
				host = "";
				try
				{
					/*
					 * With 2396 URI handling, forms such as 'rmi://host:bar'
					 * or 'rmi://:<port>' are parsed into a registry based
					 * authority. We only want to allow server based naming
					 * authorities.
					 */
					uri.ParseServerAuthority();
				}
				catch (URISyntaxException)
				{
					// Check if the authority is of form ':<port>'
					String authority = uri.Authority;
					if (authority != null && authority.StartsWith(":"))
					{
						// Convert the authority to 'localhost:<port>' form
						authority = "localhost" + authority;
						try
						{
							uri = new URI(null, authority, null, null, null);
							// Make sure it now parses to a valid server based
							// naming authority
							uri.ParseServerAuthority();
						}
						catch (URISyntaxException)
						{
							throw new MalformedURLException("invalid authority: " + str);
						}
					}
					else
					{
						throw new MalformedURLException("invalid authority: " + str);
					}
				}
			}
			int port = uri.Port;
			if (port == -1)
			{
				port = Registry_Fields.REGISTRY_PORT;
			}
			return new ParsedNamingURL(host, port, name);
		}

		/// <summary>
		/// Simple class to enable multiple URL return values.
		/// </summary>
		private class ParsedNamingURL
		{
			internal String Host;
			internal int Port;
			internal String Name;

			internal ParsedNamingURL(String host, int port, String name)
			{
				this.Host = host;
				this.Port = port;
				this.Name = name;
			}
		}
	}

}