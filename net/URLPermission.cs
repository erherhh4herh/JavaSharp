using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// Represents permission to access a resource or set of resources defined by a
	/// given url, and for a given set of user-settable request methods
	/// and request headers. The <i>name</i> of the permission is the url string.
	/// The <i>actions</i> string is a concatenation of the request methods and headers.
	/// The range of method and header names is not restricted by this class.
	/// <para><b>The url</b><p>
	/// The url string has the following expected structure.
	/// <pre>
	///     scheme : // authority [ / path ]
	/// </pre>
	/// <i>scheme</i> will typically be http or https, but is not restricted by this
	/// class.
	/// <i>authority</i> is specified as:
	/// <pre>
	///     authority = [ userinfo @ ] hostrange [ : portrange ]
	///     portrange = portnumber | -portnumber | portnumber-[portnumber] | *
	///     hostrange = ([*.] dnsname) | IPv4address | IPv6address
	/// </pre>
	/// <i>dnsname</i> is a standard DNS host or domain name, ie. one or more labels
	/// separated by ".". <i>IPv4address</i> is a standard literal IPv4 address and
	/// <i>IPv6address</i> is as defined in <a href="http://www.ietf.org/rfc/rfc2732.txt">
	/// RFC 2732</a>. Literal IPv6 addresses must however, be enclosed in '[]' characters.
	/// The <i>dnsname</i> specification can be preceded by "*." which means
	/// the name will match any hostname whose right-most domain labels are the same as
	/// this name. For example, "*.oracle.com" matches "foo.bar.oracle.com"
	/// </para>
	/// <para>
	/// <i>portrange</i> is used to specify a port number, or a bounded or unbounded range of ports
	/// that this permission applies to. If portrange is absent or invalid, then a default
	/// port number is assumed if the scheme is {@code http} (default 80) or {@code https}
	/// (default 443). No default is assumed for other schemes. A wildcard may be specified
	/// which means all ports.
	/// </para>
	/// <para>
	/// <i>userinfo</i> is optional. A userinfo component if present, is ignored when
	/// creating a URLPermission, and has no effect on any other methods defined by this class.
	/// </para>
	/// <para>
	/// The <i>path</i> component comprises a sequence of path segments,
	/// separated by '/' characters. <i>path</i> may also be empty. The path is specified
	/// in a similar way to the path in <seealso cref="java.io.FilePermission"/>. There are
	/// three different ways as the following examples show:
	/// <table border>
	/// <caption>URL Examples</caption>
	/// <tr><th>Example url</th><th>Description</th></tr>
	/// <tr><td style="white-space:nowrap;">http://www.oracle.com/a/b/c.html</td>
	///   <td>A url which identifies a specific (single) resource</td>
	/// </tr>
	/// <tr><td>http://www.oracle.com/a/b/*</td>
	///   <td>The '*' character refers to all resources in the same "directory" - in
	///       other words all resources with the same number of path components, and
	///       which only differ in the final path component, represented by the '*'.
	///   </td>
	/// </tr>
	/// <tr><td>http://www.oracle.com/a/b/-</td>
	///   <td>The '-' character refers to all resources recursively below the
	///       preceding path (eg. http://www.oracle.com/a/b/c/d/e.html matches this
	///       example).
	///   </td>
	/// </tr>
	/// </table>
	/// </para>
	/// <para>
	/// The '*' and '-' may only be specified in the final segment of a path and must be
	/// the only character in that segment. Any query or fragment components of the
	/// url are ignored when constructing URLPermissions.
	/// </para>
	/// <para>
	/// As a special case, urls of the form, "scheme:*" are accepted to
	/// mean any url of the given scheme.
	/// </para>
	/// <para>
	/// The <i>scheme</i> and <i>authority</i> components of the url string are handled
	/// without regard to case. This means <seealso cref="#equals(Object)"/>,
	/// <seealso cref="#hashCode()"/> and <seealso cref="#implies(Permission)"/> are case insensitive with respect
	/// to these components. If the <i>authority</i> contains a literal IP address,
	/// then the address is normalized for comparison. The path component is case sensitive.
	/// </para>
	/// <para><b>The actions string</b><p>
	/// The actions string of a URLPermission is a concatenation of the <i>method list</i>
	/// and the <i>request headers list</i>. These are lists of the permitted request
	/// methods and permitted request headers of the permission (respectively). The two lists
	/// are separated by a colon ':' character and elements of each list are comma separated.
	/// Some examples are:
	/// <pre>
	///         "POST,GET,DELETE"
	///         "GET:X-Foo-Request,X-Bar-Request"
	///         "POST,GET:Header1,Header2"
	/// </pre>
	/// The first example specifies the methods: POST, GET and DELETE, but no request headers.
	/// The second example specifies one request method and two headers. The third
	/// example specifies two request methods, and two headers.
	/// </para>
	/// <para>
	/// The colon separator need not be present if the request headers list is empty.
	/// No white-space is permitted in the actions string. The action strings supplied to
	/// the URLPermission constructors are case-insensitive and are normalized by converting
	/// method names to upper-case and header names to the form defines in RFC2616 (lower case
	/// with initial letter of each word capitalized). Either list can contain a wild-card '*'
	/// character which signifies all request methods or headers respectively.
	/// </para>
	/// <para>
	/// Note. Depending on the context of use, some request methods and headers may be permitted
	/// at all times, and others may not be permitted at any time. For example, the
	/// HTTP protocol handler might disallow certain headers such as Content-Length
	/// from being set by application code, regardless of whether the security policy
	/// in force, permits it.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class URLPermission : Permission
	{

		private const long SerialVersionUID = -2702463814894478682L;

		[NonSerialized]
		private String Scheme;
		[NonSerialized]
		private String Ssp; // scheme specific part
		[NonSerialized]
		private String Path;
		[NonSerialized]
		private IList<String> Methods;
		[NonSerialized]
		private IList<String> RequestHeaders;
		[NonSerialized]
		private Authority Authority;

		// serialized field
		private String Actions_Renamed;

		/// <summary>
		/// Creates a new URLPermission from a url string and which permits the given
		/// request methods and user-settable request headers.
		/// The name of the permission is the url string it was created with. Only the scheme,
		/// authority and path components of the url are used internally. Any fragment or query
		/// components are ignored. The permissions action string is as specified above.
		/// </summary>
		/// <param name="url"> the url string
		/// </param>
		/// <param name="actions"> the actions string
		/// </param>
		/// <exception cref="IllegalArgumentException"> if url is invalid or if actions contains white-space. </exception>
		public URLPermission(String url, String actions) : base(url)
		{
			Init(actions);
		}

		private void Init(String actions)
		{
			ParseURI(Name);
			int colon = actions.IndexOf(':');
			if (actions.LastIndexOf(':') != colon)
			{
				throw new IllegalArgumentException("invalid actions string");
			}

			String methods, headers;
			if (colon == -1)
			{
				methods = actions;
				headers = "";
			}
			else
			{
				methods = actions.Substring(0, colon);
				headers = actions.Substring(colon + 1);
			}

			IList<String> l = NormalizeMethods(methods);
			l.Sort();
			this.Methods = Collections.UnmodifiableList(l);

			l = NormalizeHeaders(headers);
			l.Sort();
			this.RequestHeaders = Collections.UnmodifiableList(l);

			this.Actions_Renamed = Actions();
		}

		/// <summary>
		/// Creates a URLPermission with the given url string and unrestricted
		/// methods and request headers by invoking the two argument
		/// constructor as follows: URLPermission(url, "*:*")
		/// </summary>
		/// <param name="url"> the url string
		/// </param>
		/// <exception cref="IllegalArgumentException"> if url does not result in a valid <seealso cref="URI"/> </exception>
		public URLPermission(String url) : this(url, "*:*")
		{
		}

		/// <summary>
		/// Returns the normalized method list and request
		/// header list, in the form:
		/// <pre>
		///      "method-names : header-names"
		/// </pre>
		/// <para>
		/// where method-names is the list of methods separated by commas
		/// and header-names is the list of permitted headers separated by commas.
		/// There is no white space in the returned String. If header-names is empty
		/// then the colon separator will not be present.
		/// </para>
		/// </summary>
		public override String Actions
		{
			get
			{
				return Actions_Renamed;
			}
		}

		/// <summary>
		/// Checks if this URLPermission implies the given permission.
		/// Specifically, the following checks are done as if in the
		/// following sequence:
		/// <ul>
		/// <li>if 'p' is not an instance of URLPermission return false</li>
		/// <li>if any of p's methods are not in this's method list, and if
		///     this's method list is not equal to "*", then return false.</li>
		/// <li>if any of p's headers are not in this's request header list, and if
		///     this's request header list is not equal to "*", then return false.</li>
		/// <li>if this's url scheme is not equal to p's url scheme return false</li>
		/// <li>if the scheme specific part of this's url is '*' return true</li>
		/// <li>if the set of hosts defined by p's url hostrange is not a subset of
		///     this's url hostrange then return false. For example, "*.foo.oracle.com"
		///     is a subset of "*.oracle.com". "foo.bar.oracle.com" is not
		///     a subset of "*.foo.oracle.com"</li>
		/// <li>if the portrange defined by p's url is not a subset of the
		///     portrange defined by this's url then return false.
		/// <li>if the path or paths specified by p's url are contained in the
		///     set of paths specified by this's url, then return true
		/// <li>otherwise, return false</li>
		/// </ul>
		/// <para>Some examples of how paths are matched are shown below:
		/// <table border>
		/// <caption>Examples of Path Matching</caption>
		/// <tr><th>this's path</th><th>p's path</th><th>match</th></tr>
		/// <tr><td>/a/b</td><td>/a/b</td><td>yes</td></tr>
		/// <tr><td>/a/b/*</td><td>/a/b/c</td><td>yes</td></tr>
		/// <tr><td>/a/b/*</td><td>/a/b/c/d</td><td>no</td></tr>
		/// <tr><td>/a/b/-</td><td>/a/b/c/d</td><td>yes</td></tr>
		/// <tr><td>/a/b/-</td><td>/a/b/c/d/e</td><td>yes</td></tr>
		/// <tr><td>/a/b/-</td><td>/a/b/c/*</td><td>yes</td></tr>
		/// <tr><td>/a/b/*</td><td>/a/b/c/-</td><td>no</td></tr>
		/// </table>
		/// </para>
		/// </summary>
		public override bool Implies(Permission p)
		{
			if (!(p is URLPermission))
			{
				return false;
			}

			URLPermission that = (URLPermission)p;

			if (!this.Methods[0].Equals("*") && Collections.IndexOfSubList(this.Methods, that.Methods) == -1)
			{
				return false;
			}

			if (this.RequestHeaders.Count == 0 && that.RequestHeaders.Count > 0)
			{
				return false;
			}

			if (this.RequestHeaders.Count > 0 && !this.RequestHeaders[0].Equals("*") && Collections.IndexOfSubList(this.RequestHeaders, that.RequestHeaders) == -1)
			{
				return false;
			}

			if (!this.Scheme.Equals(that.Scheme))
			{
				return false;
			}

			if (this.Ssp.Equals("*"))
			{
				return true;
			}

			if (!this.Authority.Implies(that.Authority))
			{
				return false;
			}

			if (this.Path == null)
			{
				return that.Path == null;
			}
			if (that.Path == null)
			{
				return false;
			}

			if (this.Path.EndsWith("/-"))
			{
				String thisprefix = this.Path.Substring(0, this.Path.Length() - 1);
				return that.Path.StartsWith(thisprefix);
			}

			if (this.Path.EndsWith("/*"))
			{
				String thisprefix = this.Path.Substring(0, this.Path.Length() - 1);
				if (!that.Path.StartsWith(thisprefix))
				{
					return false;
				}
				String thatsuffix = that.Path.Substring(thisprefix.Length());
				// suffix must not contain '/' chars
				if (thatsuffix.IndexOf('/') != -1)
				{
					return false;
				}
				if (thatsuffix.Equals("-"))
				{
					return false;
				}
				return true;
			}
			return this.Path.Equals(that.Path);
		}


		/// <summary>
		/// Returns true if, this.getActions().equals(p.getActions())
		/// and p's url equals this's url.  Returns false otherwise.
		/// </summary>
		public override bool Equals(Object p)
		{
			if (!(p is URLPermission))
			{
				return false;
			}
			URLPermission that = (URLPermission)p;
			if (!this.Scheme.Equals(that.Scheme))
			{
				return false;
			}
			if (!this.Actions.Equals(that.Actions))
			{
				return false;
			}
			if (!this.Authority.Equals(that.Authority))
			{
				return false;
			}
			if (this.Path != null)
			{
				return this.Path.Equals(that.Path);
			}
			else
			{
				return that.Path == null;
			}
		}

		/// <summary>
		/// Returns a hashcode calculated from the hashcode of the
		/// actions String and the url string.
		/// </summary>
		public override int HashCode()
		{
			return Actions.HashCode() + Scheme.HashCode() + Authority.HashCode() + (Path == null ? 0 : Path.HashCode());
		}


		private IList<String> NormalizeMethods(String methods)
		{
			IList<String> l = new List<String>();
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < methods.Length(); i++)
			{
				char c = methods.CharAt(i);
				if (c == ',')
				{
					String s = b.ToString();
					if (s.Length() > 0)
					{
						l.Add(s);
					}
					b = new StringBuilder();
				}
				else if (c == ' ' || c == '\t')
				{
					throw new IllegalArgumentException("white space not allowed");
				}
				else
				{
					if (c >= 'a' && c <= 'z')
					{
						c += 'A' - 'a';
					}
					b.Append(c);
				}
			}
			String s = b.ToString();
			if (s.Length() > 0)
			{
				l.Add(s);
			}
			return l;
		}

		private IList<String> NormalizeHeaders(String headers)
		{
			IList<String> l = new List<String>();
			StringBuilder b = new StringBuilder();
			bool capitalizeNext = true;
			for (int i = 0; i < headers.Length(); i++)
			{
				char c = headers.CharAt(i);
				if (c >= 'a' && c <= 'z')
				{
					if (capitalizeNext)
					{
						c += 'A' - 'a';
						capitalizeNext = false;
					}
					b.Append(c);
				}
				else if (c == ' ' || c == '\t')
				{
					throw new IllegalArgumentException("white space not allowed");
				}
				else if (c == '-')
				{
						capitalizeNext = true;
					b.Append(c);
				}
				else if (c == ',')
				{
					String s = b.ToString();
					if (s.Length() > 0)
					{
						l.Add(s);
					}
					b = new StringBuilder();
					capitalizeNext = true;
				}
				else
				{
					capitalizeNext = false;
					b.Append(c);
				}
			}
			String s = b.ToString();
			if (s.Length() > 0)
			{
				l.Add(s);
			}
			return l;
		}

		private void ParseURI(String url)
		{
			int len = url.Length();
			int delim = url.IndexOf(':');
			if (delim == -1 || delim + 1 == len)
			{
				throw new IllegalArgumentException("invalid URL string");
			}
			Scheme = url.Substring(0, delim).ToLowerCase();
			this.Ssp = url.Substring(delim + 1);

			if (!Ssp.StartsWith("//"))
			{
				if (!Ssp.Equals("*"))
				{
					throw new IllegalArgumentException("invalid URL string");
				}
				this.Authority = new Authority(Scheme, "*");
				return;
			}
			String authpath = Ssp.Substring(2);

			delim = authpath.IndexOf('/');
			String auth;
			if (delim == -1)
			{
				this.Path = "";
				auth = authpath;
			}
			else
			{
				auth = authpath.Substring(0, delim);
				this.Path = authpath.Substring(delim);
			}
			this.Authority = new Authority(Scheme, auth.ToLowerCase());
		}

		private String Actions()
		{
			StringBuilder b = new StringBuilder();
			foreach (String s in Methods)
			{
				b.Append(s);
			}
			b.Append(":");
			foreach (String s in RequestHeaders)
			{
				b.Append(s);
			}
			return b.ToString();
		}

		/// <summary>
		/// restore the state of this object from stream
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField fields = s.ReadFields();
			String actions = (String)fields.Get("actions", null);

			Init(actions);
		}

		internal class Authority
		{
			internal HostPortrange p;

			internal Authority(String scheme, String authority)
			{
				int at = authority.IndexOf('@');
				if (at == -1)
				{
						p = new HostPortrange(scheme, authority);
				}
				else
				{
						p = new HostPortrange(scheme, authority.Substring(at + 1));
				}
			}

			internal virtual bool Implies(Authority other)
			{
				return ImpliesHostrange(other) && ImpliesPortrange(other);
			}

			internal virtual bool ImpliesHostrange(Authority that)
			{
				String thishost = this.p.Hostname();
				String thathost = that.p.Hostname();

				if (p.Wildcard() && thishost.Equals(""))
				{
					// this "*" implies all others
					return true;
				}
				if (that.p.Wildcard() && thathost.Equals(""))
				{
					// that "*" can only be implied by this "*"
					return false;
				}
				if (thishost.Equals(thathost))
				{
					// covers all cases of literal IP addresses and fixed
					// domain names.
					return true;
				}
				if (this.p.Wildcard())
				{
					// this "*.foo.com" implies "bub.bar.foo.com"
					return thathost.EndsWith(thishost);
				}
				return false;
			}

			internal virtual bool ImpliesPortrange(Authority that)
			{
				int[] thisrange = this.p.Portrange();
				int[] thatrange = that.p.Portrange();
				if (thisrange[0] == -1)
				{
					/* port not specified non http/s URL */
					return true;
				}
				return thisrange[0] <= thatrange[0] && thisrange[1] >= thatrange[1];
			}

			internal virtual bool Equals(Authority that)
			{
				return this.p.Equals(that.p);
			}

			public override int HashCode()
			{
				return p.HashCode();
			}
		}
	}

}