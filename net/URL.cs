using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// Class {@code URL} represents a Uniform Resource
	/// Locator, a pointer to a "resource" on the World
	/// Wide Web. A resource can be something as simple as a file or a
	/// directory, or it can be a reference to a more complicated object,
	/// such as a query to a database or to a search engine. More
	/// information on the types of URLs and their formats can be found at:
	/// <a href=
	/// "http://web.archive.org/web/20051219043731/http://archive.ncsa.uiuc.edu/SDG/Software/Mosaic/Demo/url-primer.html">
	/// <i>Types of URL</i></a>
	/// <para>
	/// In general, a URL can be broken into several parts. Consider the
	/// following example:
	/// <blockquote><pre>
	///     http://www.example.com/docs/resource1.html
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The URL above indicates that the protocol to use is
	/// {@code http} (HyperText Transfer Protocol) and that the
	/// information resides on a host machine named
	/// {@code www.example.com}. The information on that host
	/// machine is named {@code /docs/resource1.html}. The exact
	/// meaning of this name on the host machine is both protocol
	/// dependent and host dependent. The information normally resides in
	/// a file, but it could be generated on the fly. This component of
	/// the URL is called the <i>path</i> component.
	/// </para>
	/// <para>
	/// A URL can optionally specify a "port", which is the
	/// port number to which the TCP connection is made on the remote host
	/// machine. If the port is not specified, the default port for
	/// the protocol is used instead. For example, the default port for
	/// {@code http} is {@code 80}. An alternative port could be
	/// specified as:
	/// <blockquote><pre>
	///     http://www.example.com:1080/docs/resource1.html
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The syntax of {@code URL} is defined by  <a
	/// href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	/// Resource Identifiers (URI): Generic Syntax</i></a>, amended by <a
	/// href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	/// Literal IPv6 Addresses in URLs</i></a>. The Literal IPv6 address format
	/// also supports scope_ids. The syntax and usage of scope_ids is described
	/// <a href="Inet6Address.html#scoped">here</a>.
	/// </para>
	/// <para>
	/// A URL may have appended to it a "fragment", also known
	/// as a "ref" or a "reference". The fragment is indicated by the sharp
	/// sign character "#" followed by more characters. For example,
	/// <blockquote><pre>
	///     http://java.sun.com/index.html#chapter1
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// This fragment is not technically part of the URL. Rather, it
	/// indicates that after the specified resource is retrieved, the
	/// application is specifically interested in that part of the
	/// document that has the tag {@code chapter1} attached to it. The
	/// meaning of a tag is resource specific.
	/// </para>
	/// <para>
	/// An application can also specify a "relative URL",
	/// which contains only enough information to reach the resource
	/// relative to another URL. Relative URLs are frequently used within
	/// HTML pages. For example, if the contents of the URL:
	/// <blockquote><pre>
	///     http://java.sun.com/index.html
	/// </pre></blockquote>
	/// contained within it the relative URL:
	/// <blockquote><pre>
	///     FAQ.html
	/// </pre></blockquote>
	/// it would be a shorthand for:
	/// <blockquote><pre>
	///     http://java.sun.com/FAQ.html
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The relative URL need not specify all the components of a URL. If
	/// the protocol, host name, or port number is missing, the value is
	/// inherited from the fully specified URL. The file component must be
	/// specified. The optional fragment is not inherited.
	/// </para>
	/// <para>
	/// The URL class does not itself encode or decode any URL components
	/// according to the escaping mechanism defined in RFC2396. It is the
	/// responsibility of the caller to encode any fields, which need to be
	/// escaped prior to calling URL, and also to decode any escaped fields,
	/// that are returned from URL. Furthermore, because URL has no knowledge
	/// of URL escaping, it does not recognise equivalence between the encoded
	/// or decoded form of the same URL. For example, the two URLs:<br>
	/// <pre>    http://foo.com/hello world/ and http://foo.com/hello%20world</pre>
	/// would be considered not equal to each other.
	/// </para>
	/// <para>
	/// Note, the <seealso cref="java.net.URI"/> class does perform escaping of its
	/// component fields in certain circumstances. The recommended way
	/// to manage the encoding and decoding of URLs is to use <seealso cref="java.net.URI"/>,
	/// and to convert between these two classes using <seealso cref="#toURI()"/> and
	/// <seealso cref="URI#toURL()"/>.
	/// </para>
	/// <para>
	/// The <seealso cref="URLEncoder"/> and <seealso cref="URLDecoder"/> classes can also be
	/// used, but only for HTML form encoding, which is not the same
	/// as the encoding scheme defined in RFC2396.
	/// 
	/// @author  James Gosling
	/// @since JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class URL
	{

		internal const String BUILTIN_HANDLERS_PREFIX = "sun.net.www.protocol";
		internal const long SerialVersionUID = -7627629688361524110L;

		/// <summary>
		/// The property which specifies the package prefix list to be scanned
		/// for protocol handlers.  The value of this property (if any) should
		/// be a vertical bar delimited list of package names to search through
		/// for a protocol handler to load.  The policy of this class is that
		/// all protocol handlers will be in a class called <protocolname>.Handler,
		/// and each package in the list is examined in turn for a matching
		/// handler.  If none are found (or the property is not specified), the
		/// default package prefix, sun.net.www.protocol, is used.  The search
		/// proceeds from the first package in the list to the last and stops
		/// when a match is found.
		/// </summary>
		private const String ProtocolPathProp = "java.protocol.handler.pkgs";

		/// <summary>
		/// The protocol to use (ftp, http, nntp, ... etc.) .
		/// @serial
		/// </summary>
		private String Protocol_Renamed;

		/// <summary>
		/// The host name to connect to.
		/// @serial
		/// </summary>
		private String Host_Renamed;

		/// <summary>
		/// The protocol port to connect to.
		/// @serial
		/// </summary>
		private int Port_Renamed = -1;

		/// <summary>
		/// The specified file name on that host. {@code file} is
		/// defined as {@code path[?query]}
		/// @serial
		/// </summary>
		private String File_Renamed;

		/// <summary>
		/// The query part of this URL.
		/// </summary>
		[NonSerialized]
		private String Query_Renamed;

		/// <summary>
		/// The authority part of this URL.
		/// @serial
		/// </summary>
		private String Authority_Renamed;

		/// <summary>
		/// The path part of this URL.
		/// </summary>
		[NonSerialized]
		private String Path_Renamed;

		/// <summary>
		/// The userinfo part of this URL.
		/// </summary>
		[NonSerialized]
		private String UserInfo_Renamed;

		/// <summary>
		/// # reference.
		/// @serial
		/// </summary>
		private String @ref;

		/// <summary>
		/// The host's IP address, used in equals and hashCode.
		/// Computed on demand. An uninitialized or unknown hostAddress is null.
		/// </summary>
		[NonSerialized]
		internal InetAddress HostAddress;

		/// <summary>
		/// The URLStreamHandler for this URL.
		/// </summary>
		[NonSerialized]
		internal URLStreamHandler Handler;

		/* Our hash code.
		 * @serial
		 */
		private int HashCode_Renamed = -1;

		[NonSerialized]
		private UrlDeserializedState TempState;

		/// <summary>
		/// Creates a {@code URL} object from the specified
		/// {@code protocol}, {@code host}, {@code port}
		/// number, and {@code file}.<para>
		/// 
		/// {@code host} can be expressed as a host name or a literal
		/// IP address. If IPv6 literal address is used, it should be
		/// enclosed in square brackets ({@code '['} and {@code ']'}), as
		/// specified by <a
		/// href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>;
		/// However, the literal IPv6 address format defined in <a
		/// href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IP
		/// </para>
		/// Version 6 Addressing Architecture</i></a> is also accepted.<para>
		/// 
		/// Specifying a {@code port} number of {@code -1}
		/// indicates that the URL should use the default port for the
		/// </para>
		/// protocol.<para>
		/// 
		/// If this is the first URL object being created with the specified
		/// protocol, a <i>stream protocol handler</i> object, an instance of
		/// class {@code URLStreamHandler}, is created for that protocol:
		/// <ol>
		/// <li>If the application has previously set up an instance of
		///     {@code URLStreamHandlerFactory} as the stream handler factory,
		///     then the {@code createURLStreamHandler} method of that instance
		///     is called with the protocol string as an argument to create the
		///     stream protocol handler.
		/// <li>If no {@code URLStreamHandlerFactory} has yet been set up,
		///     or if the factory's {@code createURLStreamHandler} method
		///     returns {@code null}, then the constructor finds the
		///     value of the system property:
		///     <blockquote><pre>
		///         java.protocol.handler.pkgs
		///     </pre></blockquote>
		///     If the value of that system property is not {@code null},
		///     it is interpreted as a list of packages separated by a vertical
		///     slash character '{@code |}'. The constructor tries to load
		///     the class named:
		///     <blockquote><pre>
		///         &lt;<i>package</i>&gt;.&lt;<i>protocol</i>&gt;.Handler
		///     </pre></blockquote>
		///     where &lt;<i>package</i>&gt; is replaced by the name of the package
		///     and &lt;<i>protocol</i>&gt; is replaced by the name of the protocol.
		///     If this class does not exist, or if the class exists but it is not
		///     a subclass of {@code URLStreamHandler}, then the next package
		///     in the list is tried.
		/// <li>If the previous step fails to find a protocol handler, then the
		///     constructor tries to load from a system default package.
		///     <blockquote><pre>
		///         &lt;<i>system default package</i>&gt;.&lt;<i>protocol</i>&gt;.Handler
		///     </pre></blockquote>
		///     If this class does not exist, or if the class exists but it is not a
		///     subclass of {@code URLStreamHandler}, then a
		///     {@code MalformedURLException} is thrown.
		/// </ol>
		/// 
		/// </para>
		/// <para>Protocol handlers for the following protocols are guaranteed
		/// to exist on the search path :-
		/// <blockquote><pre>
		///     http, https, file, and jar
		/// </pre></blockquote>
		/// Protocol handlers for additional protocols may also be
		/// available.
		/// 
		/// </para>
		/// <para>No validation of the inputs is performed by this constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="protocol">   the name of the protocol to use. </param>
		/// <param name="host">       the name of the host. </param>
		/// <param name="port">       the port number on the host. </param>
		/// <param name="file">       the file on the host </param>
		/// <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		/// <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=        java.net.URL#setURLStreamHandlerFactory(
		///                  java.net.URLStreamHandlerFactory) </seealso>
		/// <seealso cref=        java.net.URLStreamHandler </seealso>
		/// <seealso cref=        java.net.URLStreamHandlerFactory#createURLStreamHandler(
		///                  java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(String protocol, String host, int port, String file) throws MalformedURLException
		public URL(String protocol, String host, int port, String file) : this(protocol, host, port, file, null)
		{
		}

		/// <summary>
		/// Creates a URL from the specified {@code protocol}
		/// name, {@code host} name, and {@code file} name. The
		/// default port for the specified protocol is used.
		/// <para>
		/// This method is equivalent to calling the four-argument
		/// constructor with the arguments being {@code protocol},
		/// {@code host}, {@code -1}, and {@code file}.
		/// 
		/// No validation of the inputs is performed by this constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="protocol">   the name of the protocol to use. </param>
		/// <param name="host">       the name of the host. </param>
		/// <param name="file">       the file on the host. </param>
		/// <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///                  int, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(String protocol, String host, String file) throws MalformedURLException
		public URL(String protocol, String host, String file) : this(protocol, host, -1, file)
		{
		}

		/// <summary>
		/// Creates a {@code URL} object from the specified
		/// {@code protocol}, {@code host}, {@code port}
		/// number, {@code file}, and {@code handler}. Specifying
		/// a {@code port} number of {@code -1} indicates that
		/// the URL should use the default port for the protocol. Specifying
		/// a {@code handler} of {@code null} indicates that the URL
		/// should use a default stream handler for the protocol, as outlined
		/// for:
		///     java.net.URL#URL(java.lang.String, java.lang.String, int,
		///                      java.lang.String)
		/// 
		/// <para>If the handler is not null and there is a security manager,
		/// the security manager's {@code checkPermission}
		/// method is called with a
		/// {@code NetPermission("specifyStreamHandler")} permission.
		/// This may result in a SecurityException.
		/// 
		/// No validation of the inputs is performed by this constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="protocol">   the name of the protocol to use. </param>
		/// <param name="host">       the name of the host. </param>
		/// <param name="port">       the port number on the host. </param>
		/// <param name="file">       the file on the host </param>
		/// <param name="handler">    the stream handler for the URL. </param>
		/// <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        specifying a stream handler explicitly. </exception>
		/// <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=        java.net.URL#setURLStreamHandlerFactory(
		///                  java.net.URLStreamHandlerFactory) </seealso>
		/// <seealso cref=        java.net.URLStreamHandler </seealso>
		/// <seealso cref=        java.net.URLStreamHandlerFactory#createURLStreamHandler(
		///                  java.lang.String) </seealso>
		/// <seealso cref=        SecurityManager#checkPermission </seealso>
		/// <seealso cref=        java.net.NetPermission </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(String protocol, String host, int port, String file, URLStreamHandler handler) throws MalformedURLException
		public URL(String protocol, String host, int port, String file, URLStreamHandler handler)
		{
			if (handler != null)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					// check for permission to specify a handler
					CheckSpecifyHandler(sm);
				}
			}

			protocol = protocol.ToLowerCase();
			this.Protocol_Renamed = protocol;
			if (host != null)
			{

				/// <summary>
				/// if host is a literal IPv6 address,
				/// we will make it conform to RFC 2732
				/// </summary>
				if (host.IndexOf(':') >= 0 && !host.StartsWith("["))
				{
					host = "[" + host + "]";
				}
				this.Host_Renamed = host;

				if (port < -1)
				{
					throw new MalformedURLException("Invalid port number :" + port);
				}
				this.Port_Renamed = port;
				Authority_Renamed = (port == -1) ? host : host + ":" + port;
			}

			Parts parts = new Parts(file);
			Path_Renamed = parts.Path;
			Query_Renamed = parts.Query;

			if (Query_Renamed != null)
			{
				this.File_Renamed = Path_Renamed + "?" + Query_Renamed;
			}
			else
			{
				this.File_Renamed = Path_Renamed;
			}
			@ref = parts.Ref;

			// Note: we don't do validation of the URL here. Too risky to change
			// right now, but worth considering for future reference. -br
			if (handler == null && (handler = GetURLStreamHandler(protocol)) == null)
			{
				throw new MalformedURLException("unknown protocol: " + protocol);
			}
			this.Handler = handler;
		}

		/// <summary>
		/// Creates a {@code URL} object from the {@code String}
		/// representation.
		/// <para>
		/// This constructor is equivalent to a call to the two-argument
		/// constructor with a {@code null} first argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spec">   the {@code String} to parse as a URL. </param>
		/// <exception cref="MalformedURLException">  if no protocol is specified, or an
		///               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		/// <seealso cref=        java.net.URL#URL(java.net.URL, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(String spec) throws MalformedURLException
		public URL(String spec) : this(null, spec)
		{
		}

		/// <summary>
		/// Creates a URL by parsing the given spec within a specified context.
		/// 
		/// The new URL is created from the given context URL and the spec
		/// argument as described in
		/// RFC2396 &quot;Uniform Resource Identifiers : Generic * Syntax&quot; :
		/// <blockquote><pre>
		///          &lt;scheme&gt;://&lt;authority&gt;&lt;path&gt;?&lt;query&gt;#&lt;fragment&gt;
		/// </pre></blockquote>
		/// The reference is parsed into the scheme, authority, path, query and
		/// fragment parts. If the path component is empty and the scheme,
		/// authority, and query components are undefined, then the new URL is a
		/// reference to the current document. Otherwise, the fragment and query
		/// parts present in the spec are used in the new URL.
		/// <para>
		/// If the scheme component is defined in the given spec and does not match
		/// the scheme of the context, then the new URL is created as an absolute
		/// URL based on the spec alone. Otherwise the scheme component is inherited
		/// from the context URL.
		/// </para>
		/// <para>
		/// If the authority component is present in the spec then the spec is
		/// treated as absolute and the spec authority and path will replace the
		/// context authority and path. If the authority component is absent in the
		/// spec then the authority of the new URL will be inherited from the
		/// context.
		/// </para>
		/// <para>
		/// If the spec's path component begins with a slash character
		/// &quot;/&quot; then the
		/// path is treated as absolute and the spec path replaces the context path.
		/// </para>
		/// <para>
		/// Otherwise, the path is treated as a relative path and is appended to the
		/// context path, as described in RFC2396. Also, in this case,
		/// the path is canonicalized through the removal of directory
		/// changes made by occurrences of &quot;..&quot; and &quot;.&quot;.
		/// </para>
		/// <para>
		/// For a more detailed description of URL parsing, refer to RFC2396.
		/// 
		/// </para>
		/// </summary>
		/// <param name="context">   the context in which to parse the specification. </param>
		/// <param name="spec">      the {@code String} to parse as a URL. </param>
		/// <exception cref="MalformedURLException">  if no protocol is specified, or an
		///               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///                  int, java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLStreamHandler </seealso>
		/// <seealso cref=        java.net.URLStreamHandler#parseURL(java.net.URL,
		///                  java.lang.String, int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(URL context, String spec) throws MalformedURLException
		public URL(URL context, String spec) : this(context, spec, null)
		{
		}

		/// <summary>
		/// Creates a URL by parsing the given spec with the specified handler
		/// within a specified context. If the handler is null, the parsing
		/// occurs as with the two argument constructor.
		/// </summary>
		/// <param name="context">   the context in which to parse the specification. </param>
		/// <param name="spec">      the {@code String} to parse as a URL. </param>
		/// <param name="handler">   the stream handler for the URL. </param>
		/// <exception cref="MalformedURLException">  if no protocol is specified, or an
		///               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        specifying a stream handler. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///                  int, java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLStreamHandler </seealso>
		/// <seealso cref=        java.net.URLStreamHandler#parseURL(java.net.URL,
		///                  java.lang.String, int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URL(URL context, String spec, URLStreamHandler handler) throws MalformedURLException
		public URL(URL context, String spec, URLStreamHandler handler)
		{
			String original = spec;
			int i, limit, c;
			int start = 0;
			String newProtocol = null;
			bool aRef = false;
			bool isRelative = false;

			// Check for permission to specify a handler
			if (handler != null)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					CheckSpecifyHandler(sm);
				}
			}

			try
			{
				limit = spec.Length();
				while ((limit > 0) && (spec.CharAt(limit - 1) <= ' '))
				{
					limit--; //eliminate trailing whitespace
				}
				while ((start < limit) && (spec.CharAt(start) <= ' '))
				{
					start++; // eliminate leading whitespace
				}

				if (spec.RegionMatches(true, start, "url:", 0, 4))
				{
					start += 4;
				}
				if (start < spec.Length() && spec.CharAt(start) == '#')
				{
					/* we're assuming this is a ref relative to the context URL.
					 * This means protocols cannot start w/ '#', but we must parse
					 * ref URL's like: "hello:there" w/ a ':' in them.
					 */
					aRef = true;
				}
				for (i = start ; !aRef && (i < limit) && ((c = spec.CharAt(i)) != '/') ; i++)
				{
					if (c == ':')
					{

						String s = spec.Substring(start, i - start).ToLowerCase();
						if (IsValidProtocol(s))
						{
							newProtocol = s;
							start = i + 1;
						}
						break;
					}
				}

				// Only use our context if the protocols match.
				Protocol_Renamed = newProtocol;
				if ((context != null) && ((newProtocol == null) || newProtocol.EqualsIgnoreCase(context.Protocol_Renamed)))
				{
					// inherit the protocol handler from the context
					// if not specified to the constructor
					if (handler == null)
					{
						handler = context.Handler;
					}

					// If the context is a hierarchical URL scheme and the spec
					// contains a matching scheme then maintain backwards
					// compatibility and treat it as if the spec didn't contain
					// the scheme; see 5.2.3 of RFC2396
					if (context.Path_Renamed != null && context.Path_Renamed.StartsWith("/"))
					{
						newProtocol = null;
					}

					if (newProtocol == null)
					{
						Protocol_Renamed = context.Protocol_Renamed;
						Authority_Renamed = context.Authority_Renamed;
						UserInfo_Renamed = context.UserInfo_Renamed;
						Host_Renamed = context.Host_Renamed;
						Port_Renamed = context.Port_Renamed;
						File_Renamed = context.File_Renamed;
						Path_Renamed = context.Path_Renamed;
						isRelative = true;
					}
				}

				if (Protocol_Renamed == null)
				{
					throw new MalformedURLException("no protocol: " + original);
				}

				// Get the protocol handler if not specified or the protocol
				// of the context could not be used
				if (handler == null && (handler = GetURLStreamHandler(Protocol_Renamed)) == null)
				{
					throw new MalformedURLException("unknown protocol: " + Protocol_Renamed);
				}

				this.Handler = handler;

				i = spec.IndexOf('#', start);
				if (i >= 0)
				{
					@ref = StringHelperClass.SubstringSpecial(spec, i + 1, limit);
					limit = i;
				}

				/*
				 * Handle special case inheritance of query and fragment
				 * implied by RFC2396 section 5.2.2.
				 */
				if (isRelative && start == limit)
				{
					Query_Renamed = context.Query_Renamed;
					if (@ref == null)
					{
						@ref = context.@ref;
					}
				}

				handler.ParseURL(this, spec, start, limit);

			}
			catch (MalformedURLException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				MalformedURLException exception = new MalformedURLException(e.Message);
				exception.InitCause(e);
				throw exception;
			}
		}

		/*
		 * Returns true if specified string is a valid protocol name.
		 */
		private bool IsValidProtocol(String protocol)
		{
			int len = protocol.Length();
			if (len < 1)
			{
				return false;
			}
			char c = protocol.CharAt(0);
			if (!char.IsLetter(c))
			{
				return false;
			}
			for (int i = 1; i < len; i++)
			{
				c = protocol.CharAt(i);
				if (!char.IsLetterOrDigit(c) && c != '.' && c != '+' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		/*
		 * Checks for permission to specify a stream handler.
		 */
		private void CheckSpecifyHandler(SecurityManager sm)
		{
			sm.CheckPermission(SecurityConstants.SPECIFY_HANDLER_PERMISSION);
		}

		/// <summary>
		/// Sets the fields of the URL. This is not a public method so that
		/// only URLStreamHandlers can modify URL fields. URLs are
		/// otherwise constant.
		/// </summary>
		/// <param name="protocol"> the name of the protocol to use </param>
		/// <param name="host"> the name of the host </param>
		///   <param name="port"> the port number on the host </param>
		/// <param name="file"> the file on the host </param>
		/// <param name="ref"> the internal reference in the URL </param>
		internal void Set(String protocol, String host, int port, String file, String @ref)
		{
			lock (this)
			{
				this.Protocol_Renamed = protocol;
				this.Host_Renamed = host;
				Authority_Renamed = port == -1 ? host : host + ":" + port;
				this.Port_Renamed = port;
				this.File_Renamed = file;
				this.@ref = @ref;
				/* This is very important. We must recompute this after the
				 * URL has been changed. */
				HashCode_Renamed = -1;
				HostAddress = null;
				int q = file.LastIndexOf('?');
				if (q != -1)
				{
					Query_Renamed = file.Substring(q + 1);
					Path_Renamed = file.Substring(0, q);
				}
				else
				{
					Path_Renamed = file;
				}
			}
		}

		/// <summary>
		/// Sets the specified 8 fields of the URL. This is not a public method so
		/// that only URLStreamHandlers can modify URL fields. URLs are otherwise
		/// constant.
		/// </summary>
		/// <param name="protocol"> the name of the protocol to use </param>
		/// <param name="host"> the name of the host </param>
		/// <param name="port"> the port number on the host </param>
		/// <param name="authority"> the authority part for the url </param>
		/// <param name="userInfo"> the username and password </param>
		/// <param name="path"> the file on the host </param>
		/// <param name="ref"> the internal reference in the URL </param>
		/// <param name="query"> the query part of this URL
		/// @since 1.3 </param>
		internal void Set(String protocol, String host, int port, String authority, String userInfo, String path, String query, String @ref)
		{
			lock (this)
			{
				this.Protocol_Renamed = protocol;
				this.Host_Renamed = host;
				this.Port_Renamed = port;
				this.File_Renamed = query == null ? path : path + "?" + query;
				this.UserInfo_Renamed = userInfo;
				this.Path_Renamed = path;
				this.@ref = @ref;
				/* This is very important. We must recompute this after the
				 * URL has been changed. */
				HashCode_Renamed = -1;
				HostAddress = null;
				this.Query_Renamed = query;
				this.Authority_Renamed = authority;
			}
		}

		/// <summary>
		/// Gets the query part of this {@code URL}.
		/// </summary>
		/// <returns>  the query part of this {@code URL},
		/// or <CODE>null</CODE> if one does not exist
		/// @since 1.3 </returns>
		public String Query
		{
			get
			{
				return Query_Renamed;
			}
		}

		/// <summary>
		/// Gets the path part of this {@code URL}.
		/// </summary>
		/// <returns>  the path part of this {@code URL}, or an
		/// empty string if one does not exist
		/// @since 1.3 </returns>
		public String Path
		{
			get
			{
				return Path_Renamed;
			}
		}

		/// <summary>
		/// Gets the userInfo part of this {@code URL}.
		/// </summary>
		/// <returns>  the userInfo part of this {@code URL}, or
		/// <CODE>null</CODE> if one does not exist
		/// @since 1.3 </returns>
		public String UserInfo
		{
			get
			{
				return UserInfo_Renamed;
			}
		}

		/// <summary>
		/// Gets the authority part of this {@code URL}.
		/// </summary>
		/// <returns>  the authority part of this {@code URL}
		/// @since 1.3 </returns>
		public String Authority
		{
			get
			{
				return Authority_Renamed;
			}
		}

		/// <summary>
		/// Gets the port number of this {@code URL}.
		/// </summary>
		/// <returns>  the port number, or -1 if the port is not set </returns>
		public int Port
		{
			get
			{
				return Port_Renamed;
			}
		}

		/// <summary>
		/// Gets the default port number of the protocol associated
		/// with this {@code URL}. If the URL scheme or the URLStreamHandler
		/// for the URL do not define a default port number,
		/// then -1 is returned.
		/// </summary>
		/// <returns>  the port number
		/// @since 1.4 </returns>
		public int DefaultPort
		{
			get
			{
				return Handler.DefaultPort;
			}
		}

		/// <summary>
		/// Gets the protocol name of this {@code URL}.
		/// </summary>
		/// <returns>  the protocol of this {@code URL}. </returns>
		public String Protocol
		{
			get
			{
				return Protocol_Renamed;
			}
		}

		/// <summary>
		/// Gets the host name of this {@code URL}, if applicable.
		/// The format of the host conforms to RFC 2732, i.e. for a
		/// literal IPv6 address, this method will return the IPv6 address
		/// enclosed in square brackets ({@code '['} and {@code ']'}).
		/// </summary>
		/// <returns>  the host name of this {@code URL}. </returns>
		public String Host
		{
			get
			{
				return Host_Renamed;
			}
		}

		/// <summary>
		/// Gets the file name of this {@code URL}.
		/// The returned file portion will be
		/// the same as <CODE>getPath()</CODE>, plus the concatenation of
		/// the value of <CODE>getQuery()</CODE>, if any. If there is
		/// no query portion, this method and <CODE>getPath()</CODE> will
		/// return identical results.
		/// </summary>
		/// <returns>  the file name of this {@code URL},
		/// or an empty string if one does not exist </returns>
		public String File
		{
			get
			{
				return File_Renamed;
			}
		}

		/// <summary>
		/// Gets the anchor (also known as the "reference") of this
		/// {@code URL}.
		/// </summary>
		/// <returns>  the anchor (also known as the "reference") of this
		///          {@code URL}, or <CODE>null</CODE> if one does not exist </returns>
		public String Ref
		{
			get
			{
				return @ref;
			}
		}

		/// <summary>
		/// Compares this URL for equality with another object.<para>
		/// 
		/// If the given object is not a URL then this method immediately returns
		/// </para>
		/// {@code false}.<para>
		/// 
		/// Two URL objects are equal if they have the same protocol, reference
		/// equivalent hosts, have the same port number on the host, and the same
		/// </para>
		/// file and fragment of the file.<para>
		/// 
		/// Two hosts are considered equivalent if both host names can be resolved
		/// into the same IP addresses; else if either host name can't be
		/// resolved, the host names must be equal without regard to case; or both
		/// </para>
		/// host names equal to null.<para>
		/// 
		/// Since hosts comparison requires name resolution, this operation is a
		/// </para>
		/// blocking operation. <para>
		/// 
		/// Note: The defined behavior for {@code equals} is known to
		/// be inconsistent with virtual hosting in HTTP.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the URL to compare against. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (!(obj is URL))
			{
				return false;
			}
			URL u2 = (URL)obj;

			return Handler.Equals(this, u2);
		}

		/// <summary>
		/// Creates an integer suitable for hash table indexing.<para>
		/// 
		/// The hash code is based upon all the URL components relevant for URL
		/// </para>
		/// comparison. As such, this operation is a blocking operation.<para>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a hash code for this {@code URL}. </returns>
		public override int HashCode()
		{
			lock (this)
			{
				if (HashCode_Renamed != -1)
				{
					return HashCode_Renamed;
				}
        
				HashCode_Renamed = Handler.HashCode(this);
				return HashCode_Renamed;
			}
		}

		/// <summary>
		/// Compares two URLs, excluding the fragment component.<para>
		/// 
		/// Returns {@code true} if this {@code URL} and the
		/// {@code other} argument are equal without taking the
		/// fragment component into consideration.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">   the {@code URL} to compare against. </param>
		/// <returns>  {@code true} if they reference the same remote object;
		///          {@code false} otherwise. </returns>
		public bool SameFile(URL other)
		{
			return Handler.SameFile(this, other);
		}

		/// <summary>
		/// Constructs a string representation of this {@code URL}. The
		/// string is created by calling the {@code toExternalForm}
		/// method of the stream protocol handler for this object.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		/// <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String, int,
		///                  java.lang.String) </seealso>
		/// <seealso cref=     java.net.URLStreamHandler#toExternalForm(java.net.URL) </seealso>
		public override String ToString()
		{
			return ToExternalForm();
		}

		/// <summary>
		/// Constructs a string representation of this {@code URL}. The
		/// string is created by calling the {@code toExternalForm}
		/// method of the stream protocol handler for this object.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		/// <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String,
		///                  int, java.lang.String) </seealso>
		/// <seealso cref=     java.net.URLStreamHandler#toExternalForm(java.net.URL) </seealso>
		public String ToExternalForm()
		{
			return Handler.ToExternalForm(this);
		}

		/// <summary>
		/// Returns a <seealso cref="java.net.URI"/> equivalent to this URL.
		/// This method functions in the same way as {@code new URI (this.toString())}.
		/// <para>Note, any URL instance that complies with RFC 2396 can be converted
		/// to a URI. However, some URLs that are not strictly in compliance
		/// can not be converted to a URI.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="URISyntaxException"> if this URL is not formatted strictly according to
		///            to RFC2396 and cannot be converted to a URI.
		/// </exception>
		/// <returns>    a URI instance equivalent to this URL.
		/// @since 1.5 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI toURI() throws URISyntaxException
		public URI ToURI()
		{
			return new URI(ToString());
		}

		/// <summary>
		/// Returns a <seealso cref="java.net.URLConnection URLConnection"/> instance that
		/// represents a connection to the remote object referred to by the
		/// {@code URL}.
		/// 
		/// <P>A new instance of <seealso cref="java.net.URLConnection URLConnection"/> is
		/// created every time when invoking the
		/// {@link java.net.URLStreamHandler#openConnection(URL)
		/// URLStreamHandler.openConnection(URL)} method of the protocol handler for
		/// this URL.</P>
		/// 
		/// <P>It should be noted that a URLConnection instance does not establish
		/// the actual network connection on creation. This will happen only when
		/// calling <seealso cref="java.net.URLConnection#connect() URLConnection.connect()"/>.</P>
		/// 
		/// <P>If for the URL's protocol (such as HTTP or JAR), there
		/// exists a public, specialized URLConnection subclass belonging
		/// to one of the following packages or one of their subpackages:
		/// java.lang, java.io, java.util, java.net, the connection
		/// returned will be of that subclass. For example, for HTTP an
		/// HttpURLConnection will be returned, and for JAR a
		/// JarURLConnection will be returned.</P>
		/// </summary>
		/// <returns>     a <seealso cref="java.net.URLConnection URLConnection"/> linking
		///             to the URL. </returns>
		/// <exception cref="IOException">  if an I/O exception occurs. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///             int, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URLConnection openConnection() throws java.io.IOException
		public URLConnection OpenConnection()
		{
			return Handler.OpenConnection(this);
		}

		/// <summary>
		/// Same as <seealso cref="#openConnection()"/>, except that the connection will be
		/// made through the specified proxy; Protocol handlers that do not
		/// support proxing will ignore the proxy parameter and make a
		/// normal connection.
		/// 
		/// Invoking this method preempts the system's default ProxySelector
		/// settings.
		/// </summary>
		/// <param name="proxy"> the Proxy through which this connection
		///             will be made. If direct connection is desired,
		///             Proxy.NO_PROXY should be specified. </param>
		/// <returns>     a {@code URLConnection} to the URL. </returns>
		/// <exception cref="IOException">  if an I/O exception occurs. </exception>
		/// <exception cref="SecurityException"> if a security manager is present
		///             and the caller doesn't have permission to connect
		///             to the proxy. </exception>
		/// <exception cref="IllegalArgumentException"> will be thrown if proxy is null,
		///             or proxy has the wrong type </exception>
		/// <exception cref="UnsupportedOperationException"> if the subclass that
		///             implements the protocol handler doesn't support
		///             this method. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///             int, java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLConnection </seealso>
		/// <seealso cref=        java.net.URLStreamHandler#openConnection(java.net.URL,
		///             java.net.Proxy)
		/// @since      1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URLConnection openConnection(Proxy proxy) throws java.io.IOException
		public URLConnection OpenConnection(Proxy proxy)
		{
			if (proxy == null)
			{
				throw new IllegalArgumentException("proxy can not be null");
			}

			// Create a copy of Proxy as a security measure
			Proxy p = proxy == Proxy.NO_PROXY ? Proxy.NO_PROXY : sun.net.ApplicationProxy.create(proxy);
			SecurityManager sm = System.SecurityManager;
			if (p.Type() != Proxy.Type.DIRECT && sm != null)
			{
				InetSocketAddress epoint = (InetSocketAddress) p.Address();
				if (epoint.Unresolved)
				{
					sm.CheckConnect(epoint.HostName, epoint.Port);
				}
				else
				{
					sm.CheckConnect(epoint.Address.HostAddress, epoint.Port);
				}
			}
			return Handler.OpenConnection(this, p);
		}

		/// <summary>
		/// Opens a connection to this {@code URL} and returns an
		/// {@code InputStream} for reading from that connection. This
		/// method is a shorthand for:
		/// <blockquote><pre>
		///     openConnection().getInputStream()
		/// </pre></blockquote>
		/// </summary>
		/// <returns>     an input stream for reading from the URL connection. </returns>
		/// <exception cref="IOException">  if an I/O exception occurs. </exception>
		/// <seealso cref=        java.net.URL#openConnection() </seealso>
		/// <seealso cref=        java.net.URLConnection#getInputStream() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.io.InputStream openStream() throws java.io.IOException
		public InputStream OpenStream()
		{
			return OpenConnection().InputStream;
		}

		/// <summary>
		/// Gets the contents of this URL. This method is a shorthand for:
		/// <blockquote><pre>
		///     openConnection().getContent()
		/// </pre></blockquote>
		/// </summary>
		/// <returns>     the contents of this URL. </returns>
		/// <exception cref="IOException">  if an I/O exception occurs. </exception>
		/// <seealso cref=        java.net.URLConnection#getContent() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Object getContent() throws java.io.IOException
		public Object Content
		{
			get
			{
				return OpenConnection().Content;
			}
		}

		/// <summary>
		/// Gets the contents of this URL. This method is a shorthand for:
		/// <blockquote><pre>
		///     openConnection().getContent(Class[])
		/// </pre></blockquote>
		/// </summary>
		/// <param name="classes"> an array of Java types </param>
		/// <returns>     the content object of this URL that is the first match of
		///               the types specified in the classes array.
		///               null if none of the requested types are supported. </returns>
		/// <exception cref="IOException">  if an I/O exception occurs. </exception>
		/// <seealso cref=        java.net.URLConnection#getContent(Class[])
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Object getContent(Class[] classes) throws java.io.IOException
		public Object GetContent(Class[] classes)
		{
			return OpenConnection().GetContent(classes);
		}

		/// <summary>
		/// The URLStreamHandler factory.
		/// </summary>
		internal static URLStreamHandlerFactory Factory;

		/// <summary>
		/// Sets an application's {@code URLStreamHandlerFactory}.
		/// This method can be called at most once in a given Java Virtual
		/// Machine.
		/// 
		/// <para> The {@code URLStreamHandlerFactory} instance is used to
		/// construct a stream protocol handler from a protocol name.
		/// 
		/// </para>
		/// <para> If there is a security manager, this method first calls
		/// the security manager's {@code checkSetFactory} method
		/// to ensure the operation is allowed.
		/// This could result in a SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fac">   the desired factory. </param>
		/// <exception cref="Error">  if the application has already set a factory. </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///             {@code checkSetFactory} method doesn't allow
		///             the operation. </exception>
		/// <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		///             int, java.lang.String) </seealso>
		/// <seealso cref=        java.net.URLStreamHandlerFactory </seealso>
		/// <seealso cref=        SecurityManager#checkSetFactory </seealso>
		public static URLStreamHandlerFactory URLStreamHandlerFactory
		{
			set
			{
				lock (StreamHandlerLock)
				{
					if (Factory != null)
					{
						throw new Error("factory already defined");
					}
					SecurityManager security = System.SecurityManager;
					if (security != null)
					{
						security.CheckSetFactory();
					}
					Handlers.Clear();
					Factory = value;
				}
			}
		}

		/// <summary>
		/// A table of protocol handlers.
		/// </summary>
		internal static Dictionary<String, URLStreamHandler> Handlers = new Dictionary<String, URLStreamHandler>();
		private static Object StreamHandlerLock = new Object();

		/// <summary>
		/// Returns the Stream Handler. </summary>
		/// <param name="protocol"> the protocol to use </param>
		internal static URLStreamHandler GetURLStreamHandler(String protocol)
		{

			URLStreamHandler handler = Handlers[protocol];
			if (handler == null)
			{

				bool checkedWithFactory = false;

				// Use the factory (if any)
				if (Factory != null)
				{
					handler = Factory.CreateURLStreamHandler(protocol);
					checkedWithFactory = true;
				}

				// Try java protocol handler
				if (handler == null)
				{
					String packagePrefixList = null;

				packagePrefixList = java.security.AccessController.doPrivileged(new sun.security.action.GetPropertyAction(ProtocolPathProp,""));
					if (packagePrefixList != "")
					{
					packagePrefixList += "|";
					}

					// REMIND: decide whether to allow the "null" class prefix
					// or not.
				packagePrefixList += "sun.net.www.protocol";

					StringTokenizer packagePrefixIter = new StringTokenizer(packagePrefixList, "|");

					while (handler == null && packagePrefixIter.HasMoreTokens())
					{

						String packagePrefix = packagePrefixIter.NextToken().Trim();
						try
						{
							String clsName = packagePrefix + "." + protocol + ".Handler";
							Class cls = null;
							try
							{
								cls = Class.ForName(clsName);
							}
							catch (ClassNotFoundException)
							{
								ClassLoader cl = ClassLoader.SystemClassLoader;
								if (cl != null)
								{
									cls = cl.LoadClass(clsName);
								}
							}
							if (cls != null)
							{
								handler = (URLStreamHandler)cls.NewInstance();
							}
						}
						catch (Exception)
						{
							// any number of exceptions can get thrown here
						}
					}
				}

				lock (StreamHandlerLock)
				{

					URLStreamHandler handler2 = null;

					// Check again with hashtable just in case another
					// thread created a handler since we last checked
					handler2 = Handlers[protocol];

					if (handler2 != null)
					{
						return handler2;
					}

					// Check with factory if another thread set a
					// factory since our last check
					if (!checkedWithFactory && Factory != null)
					{
						handler2 = Factory.CreateURLStreamHandler(protocol);
					}

					if (handler2 != null)
					{
						// The handler from the factory must be given more
						// importance. Discard the default handler that
						// this thread created.
						handler = handler2;
					}

					// Insert this handler into the hashtable
					if (handler != null)
					{
						Handlers[protocol] = handler;
					}

				}
			}

			return handler;

		}

		/// <summary>
		/// @serialField    protocol String
		/// 
		/// @serialField    host String
		/// 
		/// @serialField    port int
		/// 
		/// @serialField    authority String
		/// 
		/// @serialField    file String
		/// 
		/// @serialField    ref String
		/// 
		/// @serialField    hashCode int
		/// 
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("protocol", typeof(String)), new ObjectStreamField("host", typeof(String)), new ObjectStreamField("port", typeof(int)), new ObjectStreamField("authority", typeof(String)), new ObjectStreamField("file", typeof(String)), new ObjectStreamField("ref", typeof(String)), new ObjectStreamField("hashCode", typeof(int))};

		/// <summary>
		/// WriteObject is called to save the state of the URL to an
		/// ObjectOutputStream. The handler is not saved since it is
		/// specific to this system.
		/// 
		/// @serialData the default write object value. When read back in,
		/// the reader must ensure that calling getURLStreamHandler with
		/// the protocol variable returns a valid URLStreamHandler and
		/// throw an IOException if it does not.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			lock (this)
			{
				s.DefaultWriteObject(); // write the fields
			}
		}

		/// <summary>
		/// readObject is called to restore the state of the URL from the
		/// stream.  It reads the components of the URL and finds the local
		/// stream handler.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			lock (this)
			{
				GetField gf = s.ReadFields();
				String protocol = (String)gf.Get("protocol", null);
				if (GetURLStreamHandler(protocol) == null)
				{
					throw new IOException("unknown protocol: " + protocol);
				}
				String host = (String)gf.Get("host", null);
				int port = gf.Get("port", -1);
				String authority = (String)gf.Get("authority", null);
				String file = (String)gf.Get("file", null);
				String @ref = (String)gf.Get("ref", null);
				int hashCode = gf.Get("hashCode", -1);
				if (authority == null && ((host != null && host.Length() > 0) || port != -1))
				{
					if (host == null)
					{
						host = "";
					}
					authority = (port == -1) ? host : host + ":" + port;
				}
				TempState = new UrlDeserializedState(protocol, host, port, authority, file, @ref, hashCode);
			}
		}

		/// <summary>
		/// Replaces the de-serialized object with an URL object.
		/// </summary>
		/// <returns> a newly created object from the deserialzed state.
		/// </returns>
		/// <exception cref="ObjectStreamException"> if a new object replacing this
		/// object could not be created </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws java.io.ObjectStreamException
	   private Object ReadResolve()
	   {

			URLStreamHandler handler = null;
			// already been checked in readObject
			handler = GetURLStreamHandler(TempState.Protocol);

			URL replacementURL = null;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			if (IsBuiltinStreamHandler(handler.GetType().FullName))
			{
				replacementURL = FabricateNewURL();
			}
			else
			{
				replacementURL = setDeserializedFields(handler);
			}
			return replacementURL;
	   }

		private URL SetDeserializedFields(URLStreamHandler handler)
		{
			URL replacementURL;
			String userInfo = null;
			String protocol = TempState.Protocol;
			String host = TempState.Host;
			int port = TempState.Port;
			String authority = TempState.Authority;
			String file = TempState.File;
			String @ref = TempState.Ref;
			int hashCode = TempState.HashCode;


			// Construct authority part
			if (authority == null && ((host != null && host.Length() > 0) || port != -1))
			{
				if (host == null)
				{
					host = "";
				}
				authority = (port == -1) ? host : host + ":" + port;

				// Handle hosts with userInfo in them
				int at = host.LastIndexOf('@');
				if (at != -1)
				{
					userInfo = host.Substring(0, at);
					host = host.Substring(at + 1);
				}
			}
			else if (authority != null)
			{
				// Construct user info part
				int ind = authority.IndexOf('@');
				if (ind != -1)
				{
					userInfo = authority.Substring(0, ind);
				}
			}

			// Construct path and query part
			String path = null;
			String query = null;
			if (file != null)
			{
				// Fix: only do this if hierarchical?
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

			if (port == -1)
			{
				port = 0;
			}
			// Set the object fields.
			this.Protocol_Renamed = protocol;
			this.Host_Renamed = host;
			this.Port_Renamed = port;
			this.File_Renamed = file;
			this.Authority_Renamed = authority;
			this.@ref = @ref;
			this.HashCode_Renamed = hashCode;
			this.Handler = handler;
			this.Query_Renamed = query;
			this.Path_Renamed = path;
			this.UserInfo_Renamed = userInfo;
			replacementURL = this;
			return replacementURL;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private URL fabricateNewURL() throws java.io.InvalidObjectException
		private URL FabricateNewURL()
		{
			// create URL string from deserialized object
			URL replacementURL = null;
			String urlString = TempState.ReconstituteUrlString();

			try
			{
				replacementURL = new URL(urlString);
			}
			catch (MalformedURLException mEx)
			{
				ResetState();
				InvalidObjectException invoEx = new InvalidObjectException("Malformed URL: " + urlString);
				invoEx.InitCause(mEx);
				throw invoEx;
			}
			replacementURL.SerializedHashCode = TempState.HashCode;
			ResetState();
			return replacementURL;
		}

		private bool IsBuiltinStreamHandler(String handlerClassName)
		{
			return (handlerClassName.StartsWith(BUILTIN_HANDLERS_PREFIX));
		}

		private void ResetState()
		{
			this.Protocol_Renamed = null;
			this.Host_Renamed = null;
			this.Port_Renamed = -1;
			this.File_Renamed = null;
			this.Authority_Renamed = null;
			this.@ref = null;
			this.HashCode_Renamed = -1;
			this.Handler = null;
			this.Query_Renamed = null;
			this.Path_Renamed = null;
			this.UserInfo_Renamed = null;
			this.TempState = null;
		}

		private int SerializedHashCode
		{
			set
			{
				this.HashCode_Renamed = value;
			}
		}
	}

	internal class Parts
	{
		internal String Path_Renamed, Query_Renamed, @ref;

		internal Parts(String file)
		{
			int ind = file.IndexOf('#');
			@ref = ind < 0 ? null: file.Substring(ind + 1);
			file = ind < 0 ? file: file.Substring(0, ind);
			int q = file.LastIndexOf('?');
			if (q != -1)
			{
				Query_Renamed = file.Substring(q + 1);
				Path_Renamed = file.Substring(0, q);
			}
			else
			{
				Path_Renamed = file;
			}
		}

		internal virtual String Path
		{
			get
			{
				return Path_Renamed;
			}
		}

		internal virtual String Query
		{
			get
			{
				return Query_Renamed;
			}
		}

		internal virtual String Ref
		{
			get
			{
				return @ref;
			}
		}
	}

	internal sealed class UrlDeserializedState
	{
		private readonly String Protocol_Renamed;
		private readonly String Host_Renamed;
		private readonly int Port_Renamed;
		private readonly String Authority_Renamed;
		private readonly String File_Renamed;
		private readonly String @ref;
		private readonly int HashCode_Renamed;

		public UrlDeserializedState(String protocol, String host, int port, String authority, String file, String @ref, int hashCode)
		{
			this.Protocol_Renamed = protocol;
			this.Host_Renamed = host;
			this.Port_Renamed = port;
			this.Authority_Renamed = authority;
			this.File_Renamed = file;
			this.@ref = @ref;
			this.HashCode_Renamed = hashCode;
		}

		internal String Protocol
		{
			get
			{
				return Protocol_Renamed;
			}
		}

		internal String Host
		{
			get
			{
				return Host_Renamed;
			}
		}

		internal String Authority
		{
			get
			{
				return Authority_Renamed;
			}
		}

		internal int Port
		{
			get
			{
				return Port_Renamed;
			}
		}

		internal String File
		{
			get
			{
				return File_Renamed;
			}
		}

		internal String Ref
		{
			get
			{
				return @ref;
			}
		}

		internal int HashCode
		{
			get
			{
				return HashCode_Renamed;
			}
		}

		internal String ReconstituteUrlString()
		{

			// pre-compute length of StringBuilder
			int len = Protocol_Renamed.Length() + 1;
			if (Authority_Renamed != null && Authority_Renamed.Length() > 0)
			{
				len += 2 + Authority_Renamed.Length();
			}
			if (File_Renamed != null)
			{
				len += File_Renamed.Length();
			}
			if (@ref != null)
			{
				len += 1 + @ref.Length();
			}
			StringBuilder result = new StringBuilder(len);
			result.Append(Protocol_Renamed);
			result.Append(":");
			if (Authority_Renamed != null && Authority_Renamed.Length() > 0)
			{
				result.Append("//");
				result.Append(Authority_Renamed);
			}
			if (File_Renamed != null)
			{
				result.Append(File_Renamed);
			}
			if (@ref != null)
			{
				result.Append("#");
				result.Append(@ref);
			}
			return result.ToString();
		}
	}

}