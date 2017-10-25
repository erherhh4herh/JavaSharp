using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using IPAddressUtil = sun.net.util.IPAddressUtil;
	using ParseUtil = sun.net.www.ParseUtil;

	/// <summary>
	/// The abstract class {@code URLStreamHandler} is the common
	/// superclass for all stream protocol handlers. A stream protocol
	/// handler knows how to make a connection for a particular protocol
	/// type, such as {@code http} or {@code https}.
	/// <para>
	/// In most cases, an instance of a {@code URLStreamHandler}
	/// subclass is not created directly by an application. Rather, the
	/// first time a protocol name is encountered when constructing a
	/// {@code URL}, the appropriate stream protocol handler is
	/// automatically loaded.
	/// 
	/// @author  James Gosling
	/// </para>
	/// </summary>
	/// <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String, int, java.lang.String)
	/// @since   JDK1.0 </seealso>
	public abstract class URLStreamHandler
	{
		/// <summary>
		/// Opens a connection to the object referenced by the
		/// {@code URL} argument.
		/// This method should be overridden by a subclass.
		/// 
		/// <para>If for the handler's protocol (such as HTTP or JAR), there
		/// exists a public, specialized URLConnection subclass belonging
		/// to one of the following packages or one of their subpackages:
		/// java.lang, java.io, java.util, java.net, the connection
		/// returned will be of that subclass. For example, for HTTP an
		/// HttpURLConnection will be returned, and for JAR a
		/// JarURLConnection will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="u">   the URL that this connects to. </param>
		/// <returns>     a {@code URLConnection} object for the {@code URL}. </returns>
		/// <exception cref="IOException">  if an I/O error occurs while opening the
		///               connection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract URLConnection openConnection(URL u) throws java.io.IOException;
		protected internal abstract URLConnection OpenConnection(URL u);

		/// <summary>
		/// Same as openConnection(URL), except that the connection will be
		/// made through the specified proxy; Protocol handlers that do not
		/// support proxying will ignore the proxy parameter and make a
		/// normal connection.
		/// 
		/// Calling this method preempts the system's default ProxySelector
		/// settings.
		/// </summary>
		/// <param name="u">   the URL that this connects to. </param>
		/// <param name="p">   the proxy through which the connection will be made.
		///                 If direct connection is desired, Proxy.NO_PROXY
		///                 should be specified. </param>
		/// <returns>     a {@code URLConnection} object for the {@code URL}. </returns>
		/// <exception cref="IOException">  if an I/O error occurs while opening the
		///               connection. </exception>
		/// <exception cref="IllegalArgumentException"> if either u or p is null,
		///               or p has the wrong type. </exception>
		/// <exception cref="UnsupportedOperationException"> if the subclass that
		///               implements the protocol doesn't support this method.
		/// @since      1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected URLConnection openConnection(URL u, Proxy p) throws java.io.IOException
		protected internal virtual URLConnection OpenConnection(URL u, Proxy p)
		{
			throw new UnsupportedOperationException("Method not implemented.");
		}

		/// <summary>
		/// Parses the string representation of a {@code URL} into a
		/// {@code URL} object.
		/// <para>
		/// If there is any inherited context, then it has already been
		/// copied into the {@code URL} argument.
		/// </para>
		/// <para>
		/// The {@code parseURL} method of {@code URLStreamHandler}
		/// parses the string representation as if it were an
		/// {@code http} specification. Most URL protocol families have a
		/// similar parsing. A stream protocol handler for a protocol that has
		/// a different syntax must override this routine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="u">       the {@code URL} to receive the result of parsing
		///                  the spec. </param>
		/// <param name="spec">    the {@code String} representing the URL that
		///                  must be parsed. </param>
		/// <param name="start">   the character index at which to begin parsing. This is
		///                  just past the '{@code :}' (if there is one) that
		///                  specifies the determination of the protocol name. </param>
		/// <param name="limit">   the character position to stop parsing at. This is the
		///                  end of the string or the position of the
		///                  "{@code #}" character, if present. All information
		///                  after the sharp sign indicates an anchor. </param>
		protected internal virtual void ParseURL(URL u, String spec, int start, int limit)
		{
			// These fields may receive context content if this was relative URL
			String protocol = u.Protocol;
			String authority = u.Authority;
			String userInfo = u.UserInfo;
			String host = u.Host;
			int port = u.Port;
			String path = u.Path;
			String query = u.Query;

			// This field has already been parsed
			String @ref = u.Ref;

			bool isRelPath = false;
			bool queryOnly = false;

	// FIX: should not assume query if opaque
			// Strip off the query part
			if (start < limit)
			{
				int queryStart = spec.IndexOf('?');
				queryOnly = queryStart == start;
				if ((queryStart != -1) && (queryStart < limit))
				{
					query = StringHelperClass.SubstringSpecial(spec, queryStart + 1, limit);
					if (limit > queryStart)
					{
						limit = queryStart;
					}
					spec = spec.Substring(0, queryStart);
				}
			}

			int i = 0;
			// Parse the authority part if any
			bool isUNCName = (start <= limit - 4) && (spec.CharAt(start) == '/') && (spec.CharAt(start + 1) == '/') && (spec.CharAt(start + 2) == '/') && (spec.CharAt(start + 3) == '/');
			if (!isUNCName && (start <= limit - 2) && (spec.CharAt(start) == '/') && (spec.CharAt(start + 1) == '/'))
			{
				start += 2;
				i = spec.IndexOf('/', start);
				if (i < 0)
				{
					i = spec.IndexOf('?', start);
					if (i < 0)
					{
						i = limit;
					}
				}

				host = authority = spec.Substring(start, i - start);

				int ind = authority.IndexOf('@');
				if (ind != -1)
				{
					userInfo = authority.Substring(0, ind);
					host = authority.Substring(ind + 1);
				}
				else
				{
					userInfo = null;
				}
				if (host != null)
				{
					// If the host is surrounded by [ and ] then its an IPv6
					// literal address as specified in RFC2732
					if (host.Length() > 0 && (host.CharAt(0) == '['))
					{
						if ((ind = host.IndexOf(']')) > 2)
						{

							String nhost = host;
							host = nhost.Substring(0,ind + 1);
							if (!IPAddressUtil.isIPv6LiteralAddress(host.Substring(1, ind - 1)))
							{
								throw new IllegalArgumentException("Invalid host: " + host);
							}

							port = -1;
							if (nhost.Length() > ind + 1)
							{
								if (nhost.CharAt(ind + 1) == ':')
								{
									++ind;
									// port can be null according to RFC2396
									if (nhost.Length() > (ind + 1))
									{
										port = Convert.ToInt32(nhost.Substring(ind + 1));
									}
								}
								else
								{
									throw new IllegalArgumentException("Invalid authority field: " + authority);
								}
							}
						}
						else
						{
							throw new IllegalArgumentException("Invalid authority field: " + authority);
						}
					}
					else
					{
						ind = host.IndexOf(':');
						port = -1;
						if (ind >= 0)
						{
							// port can be null according to RFC2396
							if (host.Length() > (ind + 1))
							{
								port = Convert.ToInt32(host.Substring(ind + 1));
							}
							host = host.Substring(0, ind);
						}
					}
				}
				else
				{
					host = "";
				}
				if (port < -1)
				{
					throw new IllegalArgumentException("Invalid port number :" + port);
				}
				start = i;
				// If the authority is defined then the path is defined by the
				// spec only; See RFC 2396 Section 5.2.4.
				if (authority != null && authority.Length() > 0)
				{
					path = "";
				}
			}

			if (host == null)
			{
				host = "";
			}

			// Parse the file path if any
			if (start < limit)
			{
				if (spec.CharAt(start) == '/')
				{
					path = spec.Substring(start, limit - start);
				}
				else if (path != null && path.Length() > 0)
				{
					isRelPath = true;
					int ind = path.LastIndexOf('/');
					String seperator = "";
					if (ind == -1 && authority != null)
					{
						seperator = "/";
					}
					path = path.Substring(0, ind + 1) + seperator + spec.Substring(start, limit - start);

				}
				else
				{
					String seperator = (authority != null) ? "/" : "";
					path = seperator + spec.Substring(start, limit - start);
				}
			}
			else if (queryOnly && path != null)
			{
				int ind = path.LastIndexOf('/');
				if (ind < 0)
				{
					ind = 0;
				}
				path = path.Substring(0, ind) + "/";
			}
			if (path == null)
			{
				path = "";
			}

			if (isRelPath)
			{
				// Remove embedded /./
				while ((i = path.IndexOf("/./")) >= 0)
				{
					path = path.Substring(0, i) + path.Substring(i + 2);
				}
				// Remove embedded /../ if possible
				i = 0;
				while ((i = path.IndexOf("/../", i)) >= 0)
				{
					/*
					 * A "/../" will cancel the previous segment and itself,
					 * unless that segment is a "/../" itself
					 * i.e. "/a/b/../c" becomes "/a/c"
					 * but "/../../a" should stay unchanged
					 */
					if (i > 0 && (limit = path.LastIndexOf('/', i - 1)) >= 0 && (path.IndexOf("/../", limit) != 0))
					{
						path = path.Substring(0, limit) + path.Substring(i + 3);
						i = 0;
					}
					else
					{
						i = i + 3;
					}
				}
				// Remove trailing .. if possible
				while (path.EndsWith("/.."))
				{
					i = path.IndexOf("/..");
					if ((limit = path.LastIndexOf('/', i - 1)) >= 0)
					{
						path = path.Substring(0, limit + 1);
					}
					else
					{
						break;
					}
				}
				// Remove starting .
				if (path.StartsWith("./") && path.Length() > 2)
				{
					path = path.Substring(2);
				}

				// Remove trailing .
				if (path.EndsWith("/."))
				{
					path = path.Substring(0, path.Length() - 1);
				}
			}

			SetURL(u, protocol, host, port, authority, userInfo, path, query, @ref);
		}

		/// <summary>
		/// Returns the default port for a URL parsed by this handler. This method
		/// is meant to be overidden by handlers with default port numbers. </summary>
		/// <returns> the default port for a {@code URL} parsed by this handler.
		/// @since 1.3 </returns>
		protected internal virtual int DefaultPort
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Provides the default equals calculation. May be overidden by handlers
		/// for other protocols that have different requirements for equals().
		/// This method requires that none of its arguments is null. This is
		/// guaranteed by the fact that it is only called by java.net.URL class. </summary>
		/// <param name="u1"> a URL object </param>
		/// <param name="u2"> a URL object </param>
		/// <returns> {@code true} if the two urls are
		/// considered equal, ie. they refer to the same
		/// fragment in the same file.
		/// @since 1.3 </returns>
		protected internal virtual bool Equals(URL u1, URL u2)
		{
			String ref1 = u1.Ref;
			String ref2 = u2.Ref;
			return (ref1 == ref2 || (ref1 != null && ref1.Equals(ref2))) && SameFile(u1, u2);
		}

		/// <summary>
		/// Provides the default hash calculation. May be overidden by handlers for
		/// other protocols that have different requirements for hashCode
		/// calculation. </summary>
		/// <param name="u"> a URL object </param>
		/// <returns> an {@code int} suitable for hash table indexing
		/// @since 1.3 </returns>
		protected internal virtual int HashCode(URL u)
		{
			int h = 0;

			// Generate the protocol part.
			String protocol = u.Protocol;
			if (protocol != null)
			{
				h += protocol.HashCode();
			}

			// Generate the host part.
			InetAddress addr = GetHostAddress(u);
			if (addr != null)
			{
				h += addr.HashCode();
			}
			else
			{
				String host = u.Host;
				if (host != null)
				{
					h += host.ToLowerCase().HashCode();
				}
			}

			// Generate the file part.
			String file = u.File;
			if (file != null)
			{
				h += file.HashCode();
			}

			// Generate the port part.
			if (u.Port == -1)
			{
				h += DefaultPort;
			}
			else
			{
				h += u.Port;
			}

			// Generate the ref part.
			String @ref = u.Ref;
			if (@ref != null)
			{
				h += @ref.HashCode();
			}

			return h;
		}

		/// <summary>
		/// Compare two urls to see whether they refer to the same file,
		/// i.e., having the same protocol, host, port, and path.
		/// This method requires that none of its arguments is null. This is
		/// guaranteed by the fact that it is only called indirectly
		/// by java.net.URL class. </summary>
		/// <param name="u1"> a URL object </param>
		/// <param name="u2"> a URL object </param>
		/// <returns> true if u1 and u2 refer to the same file
		/// @since 1.3 </returns>
		protected internal virtual bool SameFile(URL u1, URL u2)
		{
			// Compare the protocols.
			if (!((u1.Protocol == u2.Protocol) || (u1.Protocol != null && u1.Protocol.EqualsIgnoreCase(u2.Protocol))))
			{
				return false;
			}

			// Compare the files.
			if (!(u1.File == u2.File || (u1.File != null && u1.File.Equals(u2.File))))
			{
				return false;
			}

			// Compare the ports.
			int port1, port2;
			port1 = (u1.Port != -1) ? u1.Port : u1.Handler.DefaultPort;
			port2 = (u2.Port != -1) ? u2.Port : u2.Handler.DefaultPort;
			if (port1 != port2)
			{
				return false;
			}

			// Compare the hosts.
			if (!HostsEqual(u1, u2))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get the IP address of our host. An empty host field or a DNS failure
		/// will result in a null return.
		/// </summary>
		/// <param name="u"> a URL object </param>
		/// <returns> an {@code InetAddress} representing the host
		/// IP address.
		/// @since 1.3 </returns>
		protected internal virtual InetAddress GetHostAddress(URL u)
		{
			lock (this)
			{
				if (u.HostAddress != null)
				{
					return u.HostAddress;
				}
        
				String host = u.Host;
				if (host == null || host.Equals(""))
				{
					return null;
				}
				else
				{
					try
					{
						u.HostAddress = InetAddress.GetByName(host);
					}
					catch (UnknownHostException)
					{
						return null;
					}
					catch (SecurityException)
					{
						return null;
					}
				}
				return u.HostAddress;
			}
		}

		/// <summary>
		/// Compares the host components of two URLs. </summary>
		/// <param name="u1"> the URL of the first host to compare </param>
		/// <param name="u2"> the URL of the second host to compare </param>
		/// <returns>  {@code true} if and only if they
		/// are equal, {@code false} otherwise.
		/// @since 1.3 </returns>
		protected internal virtual bool HostsEqual(URL u1, URL u2)
		{
			InetAddress a1 = GetHostAddress(u1);
			InetAddress a2 = GetHostAddress(u2);
			// if we have internet address for both, compare them
			if (a1 != null && a2 != null)
			{
				return a1.Equals(a2);
			// else, if both have host names, compare them
			}
			else if (u1.Host != null && u2.Host != null)
			{
				return u1.Host.EqualsIgnoreCase(u2.Host);
			}
			 else
			 {
				return u1.Host == null && u2.Host == null;
			 }
		}

		/// <summary>
		/// Converts a {@code URL} of a specific protocol to a
		/// {@code String}.
		/// </summary>
		/// <param name="u">   the URL. </param>
		/// <returns>  a string representation of the {@code URL} argument. </returns>
		protected internal virtual String ToExternalForm(URL u)
		{

			// pre-compute length of StringBuffer
			int len = u.Protocol.Length() + 1;
			if (u.Authority != null && u.Authority.Length() > 0)
			{
				len += 2 + u.Authority.Length();
			}
			if (u.Path != null)
			{
				len += u.Path.Length();
			}
			if (u.Query != null)
			{
				len += 1 + u.Query.Length();
			}
			if (u.Ref != null)
			{
				len += 1 + u.Ref.Length();
			}

			StringBuffer result = new StringBuffer(len);
			result.Append(u.Protocol);
			result.Append(":");
			if (u.Authority != null && u.Authority.Length() > 0)
			{
				result.Append("//");
				result.Append(u.Authority);
			}
			if (u.Path != null)
			{
				result.Append(u.Path);
			}
			if (u.Query != null)
			{
				result.Append('?');
				result.Append(u.Query);
			}
			if (u.Ref != null)
			{
				result.Append("#");
				result.Append(u.Ref);
			}
			return result.ToString();
		}

		/// <summary>
		/// Sets the fields of the {@code URL} argument to the indicated values.
		/// Only classes derived from URLStreamHandler are able
		/// to use this method to set the values of the URL fields.
		/// </summary>
		/// <param name="u">         the URL to modify. </param>
		/// <param name="protocol">  the protocol name. </param>
		/// <param name="host">      the remote host value for the URL. </param>
		/// <param name="port">      the port on the remote machine. </param>
		/// <param name="authority"> the authority part for the URL. </param>
		/// <param name="userInfo"> the userInfo part of the URL. </param>
		/// <param name="path">      the path component of the URL. </param>
		/// <param name="query">     the query part for the URL. </param>
		/// <param name="ref">       the reference. </param>
		/// <exception cref="SecurityException">       if the protocol handler of the URL is
		///                                  different from this one </exception>
		/// <seealso cref=     java.net.URL#set(java.lang.String, java.lang.String, int, java.lang.String, java.lang.String)
		/// @since 1.3 </seealso>
		   protected internal virtual void SetURL(URL u, String protocol, String host, int port, String authority, String userInfo, String path, String query, String @ref)
		   {
			if (this != u.Handler)
			{
				throw new SecurityException("handler for url different from " + "this handler");
			}
			// ensure that no one can reset the protocol on a given URL.
			u.Set(u.Protocol, host, port, authority, userInfo, path, query, @ref);
		   }

		/// <summary>
		/// Sets the fields of the {@code URL} argument to the indicated values.
		/// Only classes derived from URLStreamHandler are able
		/// to use this method to set the values of the URL fields.
		/// </summary>
		/// <param name="u">         the URL to modify. </param>
		/// <param name="protocol">  the protocol name. This value is ignored since 1.2. </param>
		/// <param name="host">      the remote host value for the URL. </param>
		/// <param name="port">      the port on the remote machine. </param>
		/// <param name="file">      the file. </param>
		/// <param name="ref">       the reference. </param>
		/// <exception cref="SecurityException">       if the protocol handler of the URL is
		///                                  different from this one </exception>
		/// @deprecated Use setURL(URL, String, String, int, String, String, String,
		///             String); 
		[Obsolete("Use setURL(URL, String, String, int, String, String, String,")]
		protected internal virtual void SetURL(URL u, String protocol, String host, int port, String file, String @ref)
		{
			/*
			 * Only old URL handlers call this, so assume that the host
			 * field might contain "user:passwd@host". Fix as necessary.
			 */
			String authority = null;
			String userInfo = null;
			if (host != null && host.Length() != 0)
			{
				authority = (port == -1) ? host : host + ":" + port;
				int at = host.LastIndexOf('@');
				if (at != -1)
				{
					userInfo = host.Substring(0, at);
					host = host.Substring(at + 1);
				}
			}

			/*
			 * Assume file might contain query part. Fix as necessary.
			 */
			String path = null;
			String query = null;
			if (file != null)
			{
				int q = file.LastIndexOf('?');
				if (q != -1)
				{
					query = file.Substring(q + 1);
					path = file.Substring(0, q);
				}
				else
				{
					path = file;
				}
			}
			SetURL(u, protocol, host, port, authority, userInfo, path, query, @ref);
		}
	}

}