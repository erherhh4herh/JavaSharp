using System;
using System.Collections.Generic;

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

namespace java.net
{


	/// <summary>
	/// An HttpCookie object represents an HTTP cookie, which carries state
	/// information between server and user agent. Cookie is widely adopted
	/// to create stateful sessions.
	/// 
	/// <para> There are 3 HTTP cookie specifications:
	/// <blockquote>
	///   Netscape draft<br>
	///   RFC 2109 - <a href="http://www.ietf.org/rfc/rfc2109.txt">
	/// <i>http://www.ietf.org/rfc/rfc2109.txt</i></a><br>
	///   RFC 2965 - <a href="http://www.ietf.org/rfc/rfc2965.txt">
	/// <i>http://www.ietf.org/rfc/rfc2965.txt</i></a>
	/// </blockquote>
	/// 
	/// </para>
	/// <para> HttpCookie class can accept all these 3 forms of syntax.
	/// 
	/// @author Edward Wang
	/// @since 1.6
	/// </para>
	/// </summary>
	public sealed class HttpCookie : Cloneable
	{
		// ---------------- Fields --------------

		// The value of the cookie itself.
		private readonly String Name_Renamed; // NAME= ... "$Name" style is reserved
		private String Value_Renamed; // value of NAME

		// Attributes encoded in the header's cookie fields.
		private String Comment_Renamed; // Comment=VALUE ... describes cookie's use
		private String CommentURL_Renamed; // CommentURL="http URL" ... describes cookie's use
		private bool ToDiscard; // Discard ... discard cookie unconditionally
		private String Domain_Renamed; // Domain=VALUE ... domain that sees cookie
		private long MaxAge_Renamed = MAX_AGE_UNSPECIFIED; // Max-Age=VALUE ... cookies auto-expire
		private String Path_Renamed; // Path=VALUE ... URLs that see the cookie
		private String Portlist_Renamed; // Port[="portlist"] ... the port cookie may be returned to
		private bool Secure_Renamed; // Secure ... e.g. use SSL
		private bool HttpOnly_Renamed; // HttpOnly ... i.e. not accessible to scripts
		private int Version_Renamed = 1; // Version=1 ... RFC 2965 style

		// The original header this cookie was consructed from, if it was
		// constructed by parsing a header, otherwise null.
		private readonly String Header_Renamed;

		// Hold the creation time (in seconds) of the http cookie for later
		// expiration calculation
		private readonly long WhenCreated;

		// Since the positive and zero max-age have their meanings,
		// this value serves as a hint as 'not specify max-age'
		private const long MAX_AGE_UNSPECIFIED = -1;

		// date formats used by Netscape's cookie draft
		// as well as formats seen on various sites
		private static readonly String[] COOKIE_DATE_FORMATS = new String[] {"EEE',' dd-MMM-yyyy HH:mm:ss 'GMT'", "EEE',' dd MMM yyyy HH:mm:ss 'GMT'", "EEE MMM dd yyyy HH:mm:ss 'GMT'Z", "EEE',' dd-MMM-yy HH:mm:ss 'GMT'", "EEE',' dd MMM yy HH:mm:ss 'GMT'", "EEE MMM dd yy HH:mm:ss 'GMT'Z"};

		// constant strings represent set-cookie header token
		private const String SET_COOKIE = "set-cookie:";
		private const String SET_COOKIE2 = "set-cookie2:";

		// ---------------- Ctors --------------

		/// <summary>
		/// Constructs a cookie with a specified name and value.
		/// 
		/// <para> The name must conform to RFC 2965. That means it can contain
		/// only ASCII alphanumeric characters and cannot contain commas,
		/// semicolons, or white space or begin with a $ character. The cookie's
		/// name cannot be changed after creation.
		/// 
		/// </para>
		/// <para> The value can be anything the server chooses to send. Its
		/// value is probably of interest only to the server. The cookie's
		/// value can be changed after creation with the
		/// {@code setValue} method.
		/// 
		/// </para>
		/// <para> By default, cookies are created according to the RFC 2965
		/// cookie specification. The version can be changed with the
		/// {@code setVersion} method.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">
		///         a {@code String} specifying the name of the cookie
		/// </param>
		/// <param name="value">
		///         a {@code String} specifying the value of the cookie
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if the cookie name contains illegal characters </exception>
		/// <exception cref="NullPointerException">
		///          if {@code name} is {@code null}
		/// </exception>
		/// <seealso cref= #setValue </seealso>
		/// <seealso cref= #setVersion </seealso>
		public HttpCookie(String name, String value) : this(name, value, null); / /header
		{
		}

		private HttpCookie(String name, String value, String header)
		{
			name = name.Trim();
			if (name.Length() == 0 || !IsToken(name) || name.CharAt(0) == '$')
			{
				throw new IllegalArgumentException("Illegal cookie name");
			}

			this.Name_Renamed = name;
			this.Value_Renamed = value;
			ToDiscard = false;
			Secure_Renamed = false;

			WhenCreated = DateTimeHelperClass.CurrentUnixTimeMillis();
			Portlist_Renamed = null;
			this.Header_Renamed = header;
		}

		/// <summary>
		/// Constructs cookies from set-cookie or set-cookie2 header string.
		/// RFC 2965 section 3.2.2 set-cookie2 syntax indicates that one header line
		/// may contain more than one cookie definitions, so this is a static
		/// utility method instead of another constructor.
		/// </summary>
		/// <param name="header">
		///         a {@code String} specifying the set-cookie header. The header
		///         should start with "set-cookie", or "set-cookie2" token; or it
		///         should have no leading token at all.
		/// </param>
		/// <returns>  a List of cookie parsed from header line string
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          if header string violates the cookie specification's syntax or
		///          the cookie name contains illegal characters. </exception>
		/// <exception cref="NullPointerException">
		///          if the header string is {@code null} </exception>
		public static IList<HttpCookie> Parse(String header)
		{
			return Parse(header, false);
		}

		// Private version of parse() that will store the original header used to
		// create the cookie, in the cookie itself. This can be useful for filtering
		// Set-Cookie[2] headers, using the internal parsing logic defined in this
		// class.
		private static IList<HttpCookie> Parse(String header, bool retainHeader)
		{

			int version = GuessCookieVersion(header);

			// if header start with set-cookie or set-cookie2, strip it off
			if (StartsWithIgnoreCase(header, SET_COOKIE2))
			{
				header = header.Substring(SET_COOKIE2.Length());
			}
			else if (StartsWithIgnoreCase(header, SET_COOKIE))
			{
				header = header.Substring(SET_COOKIE.Length());
			}

			IList<HttpCookie> cookies = new List<HttpCookie>();
			// The Netscape cookie may have a comma in its expires attribute, while
			// the comma is the delimiter in rfc 2965/2109 cookie header string.
			// so the parse logic is slightly different
			if (version == 0)
			{
				// Netscape draft cookie
				HttpCookie cookie = ParseInternal(header, retainHeader);
				cookie.Version = 0;
				cookies.Add(cookie);
			}
			else
			{
				// rfc2965/2109 cookie
				// if header string contains more than one cookie,
				// it'll separate them with comma
				IList<String> cookieStrings = SplitMultiCookies(header);
				foreach (String cookieStr in cookieStrings)
				{
					HttpCookie cookie = ParseInternal(cookieStr, retainHeader);
					cookie.Version = 1;
					cookies.Add(cookie);
				}
			}

			return cookies;
		}

		// ---------------- Public operations --------------

		/// <summary>
		/// Reports whether this HTTP cookie has expired or not.
		/// </summary>
		/// <returns>  {@code true} to indicate this HTTP cookie has expired;
		///          otherwise, {@code false} </returns>
		public bool HasExpired()
		{
			if (MaxAge_Renamed == 0)
			{
				return true;
			}

			// if not specify max-age, this cookie should be
			// discarded when user agent is to be closed, but
			// it is not expired.
			if (MaxAge_Renamed == MAX_AGE_UNSPECIFIED)
			{
				return false;
			}

			long deltaSecond = (DateTimeHelperClass.CurrentUnixTimeMillis() - WhenCreated) / 1000;
			if (deltaSecond > MaxAge_Renamed)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Specifies a comment that describes a cookie's purpose.
		/// The comment is useful if the browser presents the cookie
		/// to the user. Comments are not supported by Netscape Version 0 cookies.
		/// </summary>
		/// <param name="purpose">
		///         a {@code String} specifying the comment to display to the user
		/// </param>
		/// <seealso cref=  #getComment </seealso>
		public String Comment
		{
			set
			{
				Comment_Renamed = value;
			}
			get
			{
				return Comment_Renamed;
			}
		}


		/// <summary>
		/// Specifies a comment URL that describes a cookie's purpose.
		/// The comment URL is useful if the browser presents the cookie
		/// to the user. Comment URL is RFC 2965 only.
		/// </summary>
		/// <param name="purpose">
		///         a {@code String} specifying the comment URL to display to the user
		/// </param>
		/// <seealso cref=  #getCommentURL </seealso>
		public String CommentURL
		{
			set
			{
				CommentURL_Renamed = value;
			}
			get
			{
				return CommentURL_Renamed;
			}
		}


		/// <summary>
		/// Specify whether user agent should discard the cookie unconditionally.
		/// This is RFC 2965 only attribute.
		/// </summary>
		/// <param name="discard">
		///         {@code true} indicates to discard cookie unconditionally
		/// </param>
		/// <seealso cref=  #getDiscard </seealso>
		public bool Discard
		{
			set
			{
				ToDiscard = value;
			}
			get
			{
				return ToDiscard;
			}
		}


		/// <summary>
		/// Specify the portlist of the cookie, which restricts the port(s)
		/// to which a cookie may be sent back in a Cookie header.
		/// </summary>
		/// <param name="ports">
		///         a {@code String} specify the port list, which is comma separated
		///         series of digits
		/// </param>
		/// <seealso cref=  #getPortlist </seealso>
		public String Portlist
		{
			set
			{
				Portlist_Renamed = value;
			}
			get
			{
				return Portlist_Renamed;
			}
		}


		/// <summary>
		/// Specifies the domain within which this cookie should be presented.
		/// 
		/// <para> The form of the domain name is specified by RFC 2965. A domain
		/// name begins with a dot ({@code .foo.com}) and means that
		/// the cookie is visible to servers in a specified Domain Name System
		/// (DNS) zone (for example, {@code www.foo.com}, but not
		/// {@code a.b.foo.com}). By default, cookies are only returned
		/// to the server that sent them.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">
		///         a {@code String} containing the domain name within which this
		///         cookie is visible; form is according to RFC 2965
		/// </param>
		/// <seealso cref=  #getDomain </seealso>
		public String Domain
		{
			set
			{
				if (value != null)
				{
					Domain_Renamed = value.ToLowerCase();
				}
				else
				{
					Domain_Renamed = value;
				}
			}
			get
			{
				return Domain_Renamed;
			}
		}


		/// <summary>
		/// Sets the maximum age of the cookie in seconds.
		/// 
		/// <para> A positive value indicates that the cookie will expire
		/// after that many seconds have passed. Note that the value is
		/// the <i>maximum</i> age when the cookie will expire, not the cookie's
		/// current age.
		/// 
		/// </para>
		/// <para> A negative value means that the cookie is not stored persistently
		/// and will be deleted when the Web browser exits. A zero value causes the
		/// cookie to be deleted.
		/// 
		/// </para>
		/// </summary>
		/// <param name="expiry">
		///         an integer specifying the maximum age of the cookie in seconds;
		///         if zero, the cookie should be discarded immediately; otherwise,
		///         the cookie's max age is unspecified.
		/// </param>
		/// <seealso cref=  #getMaxAge </seealso>
		public long MaxAge
		{
			set
			{
				MaxAge_Renamed = value;
			}
			get
			{
				return MaxAge_Renamed;
			}
		}


		/// <summary>
		/// Specifies a path for the cookie to which the client should return
		/// the cookie.
		/// 
		/// <para> The cookie is visible to all the pages in the directory
		/// you specify, and all the pages in that directory's subdirectories.
		/// A cookie's path must include the servlet that set the cookie,
		/// for example, <i>/catalog</i>, which makes the cookie
		/// visible to all directories on the server under <i>/catalog</i>.
		/// 
		/// </para>
		/// <para> Consult RFC 2965 (available on the Internet) for more
		/// information on setting path names for cookies.
		/// 
		/// </para>
		/// </summary>
		/// <param name="uri">
		///         a {@code String} specifying a path
		/// </param>
		/// <seealso cref=  #getPath </seealso>
		public String Path
		{
			set
			{
				Path_Renamed = value;
			}
			get
			{
				return Path_Renamed;
			}
		}


		/// <summary>
		/// Indicates whether the cookie should only be sent using a secure protocol,
		/// such as HTTPS or SSL.
		/// 
		/// <para> The default value is {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="flag">
		///         If {@code true}, the cookie can only be sent over a secure
		///         protocol like HTTPS. If {@code false}, it can be sent over
		///         any protocol.
		/// </param>
		/// <seealso cref=  #getSecure </seealso>
		public bool Secure
		{
			set
			{
				Secure_Renamed = value;
			}
			get
			{
				return Secure_Renamed;
			}
		}


		/// <summary>
		/// Returns the name of the cookie. The name cannot be changed after
		/// creation.
		/// </summary>
		/// <returns>  a {@code String} specifying the cookie's name </returns>
		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Assigns a new value to a cookie after the cookie is created.
		/// If you use a binary value, you may want to use BASE64 encoding.
		/// 
		/// <para> With Version 0 cookies, values should not contain white space,
		/// brackets, parentheses, equals signs, commas, double quotes, slashes,
		/// question marks, at signs, colons, and semicolons. Empty values may not
		/// behave the same way on all browsers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newValue">
		///         a {@code String} specifying the new value
		/// </param>
		/// <seealso cref=  #getValue </seealso>
		public String Value
		{
			set
			{
				Value_Renamed = value;
			}
			get
			{
				return Value_Renamed;
			}
		}


		/// <summary>
		/// Returns the version of the protocol this cookie complies with. Version 1
		/// complies with RFC 2965/2109, and version 0 complies with the original
		/// cookie specification drafted by Netscape. Cookies provided by a browser
		/// use and identify the browser's cookie version.
		/// </summary>
		/// <returns>  0 if the cookie complies with the original Netscape
		///          specification; 1 if the cookie complies with RFC 2965/2109
		/// </returns>
		/// <seealso cref=  #setVersion </seealso>
		public int Version
		{
			get
			{
				return Version_Renamed;
			}
			set
			{
				if (value != 0 && value != 1)
				{
					throw new IllegalArgumentException("cookie version should be 0 or 1");
				}
    
				Version_Renamed = value;
			}
		}


		/// <summary>
		/// Returns {@code true} if this cookie contains the <i>HttpOnly</i>
		/// attribute. This means that the cookie should not be accessible to
		/// scripting engines, like javascript.
		/// </summary>
		/// <returns>  {@code true} if this cookie should be considered HTTPOnly
		/// </returns>
		/// <seealso cref=  #setHttpOnly(boolean) </seealso>
		public bool HttpOnly
		{
			get
			{
				return HttpOnly_Renamed;
			}
			set
			{
				this.HttpOnly_Renamed = value;
			}
		}


		/// <summary>
		/// The utility method to check whether a host name is in a domain or not.
		/// 
		/// <para> This concept is described in the cookie specification.
		/// To understand the concept, some terminologies need to be defined first:
		/// <blockquote>
		/// effective host name = hostname if host name contains dot<br>
		/// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		/// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or = hostname.local if not
		/// </blockquote>
		/// </para>
		/// <para>Host A's name domain-matches host B's if:
		/// <blockquote><ul>
		///   <li>their host name strings string-compare equal; or</li>
		///   <li>A is a HDN string and has the form NB, where N is a non-empty
		///   name string, B has the form .B', and B' is a HDN string.  (So,
		///   x.y.com domain-matches .Y.com but not Y.com.)</li>
		/// </ul></blockquote>
		/// 
		/// </para>
		/// <para>A host isn't in a domain (RFC 2965 sec. 3.3.2) if:
		/// <blockquote><ul>
		///   <li>The value for the Domain attribute contains no embedded dots,
		///   and the value is not .local.</li>
		///   <li>The effective host name that derives from the request-host does
		///   not domain-match the Domain attribute.</li>
		///   <li>The request-host is a HDN (not IP address) and has the form HD,
		///   where D is the value of the Domain attribute, and H is a string
		///   that contains one or more dots.</li>
		/// </ul></blockquote>
		/// 
		/// </para>
		/// <para>Examples:
		/// <blockquote><ul>
		///   <li>A Set-Cookie2 from request-host y.x.foo.com for Domain=.foo.com
		///   would be rejected, because H is y.x and contains a dot.</li>
		///   <li>A Set-Cookie2 from request-host x.foo.com for Domain=.foo.com
		///   would be accepted.</li>
		///   <li>A Set-Cookie2 with Domain=.com or Domain=.com., will always be
		///   rejected, because there is no embedded dot.</li>
		///   <li>A Set-Cookie2 from request-host example for Domain=.local will
		///   be accepted, because the effective host name for the request-
		///   host is example.local, and example.local domain-matches .local.</li>
		/// </ul></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="domain">
		///         the domain name to check host name with
		/// </param>
		/// <param name="host">
		///         the host name in question
		/// </param>
		/// <returns>  {@code true} if they domain-matches; {@code false} if not </returns>
		public static bool DomainMatches(String domain, String host)
		{
			if (domain == null || host == null)
			{
				return false;
			}

			// if there's no embedded dot in domain and domain is not .local
			bool isLocalDomain = ".local".Equals(domain, StringComparison.CurrentCultureIgnoreCase);
			int embeddedDotInDomain = domain.IndexOf('.');
			if (embeddedDotInDomain == 0)
			{
				embeddedDotInDomain = domain.IndexOf('.', 1);
			}
			if (!isLocalDomain && (embeddedDotInDomain == -1 || embeddedDotInDomain == domain.Length() - 1))
			{
				return false;
			}

			// if the host name contains no dot and the domain name
			// is .local or host.local
			int firstDotInHost = host.IndexOf('.');
			if (firstDotInHost == -1 && (isLocalDomain || domain.EqualsIgnoreCase(host + ".local")))
			{
				return true;
			}

			int domainLength = domain.Length();
			int lengthDiff = host.Length() - domainLength;
			if (lengthDiff == 0)
			{
				// if the host name and the domain name are just string-compare euqal
				return host.EqualsIgnoreCase(domain);
			}
			else if (lengthDiff > 0)
			{
				// need to check H & D component
				String H = host.Substring(0, lengthDiff);
				String D = host.Substring(lengthDiff);

				return (H.IndexOf('.') == -1 && D.EqualsIgnoreCase(domain));
			}
			else if (lengthDiff == -1)
			{
				// if domain is actually .host
				return (domain.CharAt(0) == '.' && host.EqualsIgnoreCase(domain.Substring(1)));
			}

			return false;
		}

		/// <summary>
		/// Constructs a cookie header string representation of this cookie,
		/// which is in the format defined by corresponding cookie specification,
		/// but without the leading "Cookie:" token.
		/// </summary>
		/// <returns>  a string form of the cookie. The string has the defined format </returns>
		public override String ToString()
		{
			if (Version > 0)
			{
				return ToRFC2965HeaderString();
			}
			else
			{
				return ToNetscapeHeaderString();
			}
		}

		/// <summary>
		/// Test the equality of two HTTP cookies.
		/// 
		/// <para> The result is {@code true} only if two cookies come from same domain
		/// (case-insensitive), have same name (case-insensitive), and have same path
		/// (case-sensitive).
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code true} if two HTTP cookies equal to each other;
		///          otherwise, {@code false} </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is HttpCookie))
			{
				return false;
			}
			HttpCookie other = (HttpCookie)obj;

			// One http cookie equals to another cookie (RFC 2965 sec. 3.3.3) if:
			//   1. they come from same domain (case-insensitive),
			//   2. have same name (case-insensitive),
			//   3. and have same path (case-sensitive).
			return EqualsIgnoreCase(Name, other.Name) && EqualsIgnoreCase(Domain, other.Domain) && Objects.Equals(Path, other.Path);
		}

		/// <summary>
		/// Returns the hash code of this HTTP cookie. The result is the sum of
		/// hash code value of three significant components of this cookie: name,
		/// domain, and path. That is, the hash code is the value of the expression:
		/// <blockquote>
		/// getName().toLowerCase().hashCode()<br>
		/// + getDomain().toLowerCase().hashCode()<br>
		/// + getPath().hashCode()
		/// </blockquote>
		/// </summary>
		/// <returns>  this HTTP cookie's hash code </returns>
		public override int HashCode()
		{
			int h1 = Name_Renamed.ToLowerCase().HashCode();
			int h2 = (Domain_Renamed != null) ? Domain_Renamed.ToLowerCase().HashCode() : 0;
			int h3 = (Path_Renamed != null) ? Path_Renamed.HashCode() : 0;

			return h1 + h2 + h3;
		}

		/// <summary>
		/// Create and return a copy of this object.
		/// </summary>
		/// <returns>  a clone of this HTTP cookie </returns>
		public override Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new RuntimeException(e.Message);
			}
		}

		// ---------------- Private operations --------------

		// Note -- disabled for now to allow full Netscape compatibility
		// from RFC 2068, token special case characters
		//
		// private static final String tspecials = "()<>@,;:\\\"/[]?={} \t";
		private const String Tspecials = ",; "; // deliberately includes space

		/*
		 * Tests a string and returns true if the string counts as a token.
		 *
		 * @param  value
		 *         the {@code String} to be tested
		 *
		 * @return  {@code true} if the {@code String} is a token;
		 *          {@code false} if it is not
		 */
		private static bool IsToken(String value)
		{
			int len = value.Length();

			for (int i = 0; i < len; i++)
			{
				char c = value.CharAt(i);

				if (c < 0x20 || c >= 0x7f || Tspecials.IndexOf(c) != -1)
				{
					return false;
				}
			}
			return true;
		}

		/*
		 * Parse header string to cookie object.
		 *
		 * @param  header
		 *         header string; should contain only one NAME=VALUE pair
		 *
		 * @return  an HttpCookie being extracted
		 *
		 * @throws  IllegalArgumentException
		 *          if header string violates the cookie specification
		 */
		private static HttpCookie ParseInternal(String header, bool retainHeader)
		{
			HttpCookie cookie = null;
			String namevaluePair = null;

			StringTokenizer tokenizer = new StringTokenizer(header, ";");

			// there should always have at least on name-value pair;
			// it's cookie's name
			try
			{
				namevaluePair = tokenizer.NextToken();
				int index = namevaluePair.IndexOf('=');
				if (index != -1)
				{
					String name = namevaluePair.Substring(0, index).Trim();
					String value = namevaluePair.Substring(index + 1).Trim();
					if (retainHeader)
					{
						cookie = new HttpCookie(name, StripOffSurroundingQuote(value), header);
					}
					else
					{
						cookie = new HttpCookie(name, StripOffSurroundingQuote(value));
					}
				}
				else
				{
					// no "=" in name-value pair; it's an error
					throw new IllegalArgumentException("Invalid cookie name-value pair");
				}
			}
			catch (NoSuchElementException)
			{
				throw new IllegalArgumentException("Empty cookie header string");
			}

			// remaining name-value pairs are cookie's attributes
			while (tokenizer.HasMoreTokens())
			{
				namevaluePair = tokenizer.NextToken();
				int index = namevaluePair.IndexOf('=');
				String name, value;
				if (index != -1)
				{
					name = namevaluePair.Substring(0, index).Trim();
					value = namevaluePair.Substring(index + 1).Trim();
				}
				else
				{
					name = namevaluePair.Trim();
					value = null;
				}

				// assign attribute to cookie
				AssignAttribute(cookie, name, value);
			}

			return cookie;
		}

		/*
		 * assign cookie attribute value to attribute name;
		 * use a map to simulate method dispatch
		 */
		internal interface CookieAttributeAssignor
		{
				void Assign(HttpCookie cookie, String attrName, String attrValue);
		}
		internal static readonly IDictionary<String, CookieAttributeAssignor> Assignors = new Dictionary<String, CookieAttributeAssignor>();
		static HttpCookie()
		{
			Assignors["comment"] = new CookieAttributeAssignorAnonymousInnerClassHelper();
			Assignors["commenturl"] = new CookieAttributeAssignorAnonymousInnerClassHelper2();
			Assignors["discard"] = new CookieAttributeAssignorAnonymousInnerClassHelper3();
			Assignors["domain"] = new CookieAttributeAssignorAnonymousInnerClassHelper4();
			Assignors["max-age"] = new CookieAttributeAssignorAnonymousInnerClassHelper5();
			Assignors["path"] = new CookieAttributeAssignorAnonymousInnerClassHelper6();
			Assignors["port"] = new CookieAttributeAssignorAnonymousInnerClassHelper7();
			Assignors["secure"] = new CookieAttributeAssignorAnonymousInnerClassHelper8();
			Assignors["httponly"] = new CookieAttributeAssignorAnonymousInnerClassHelper9();
			Assignors["version"] = new CookieAttributeAssignorAnonymousInnerClassHelper10();
			Assignors["expires"] = new CookieAttributeAssignorAnonymousInnerClassHelper11(); // Netscape only
			sun.misc.SharedSecrets.setJavaNetHttpCookieAccess(new JavaNetHttpCookieAccessAnonymousInnerClassHelper()
		   );
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.Comment == null)
				{
					cookie.Comment = attrValue;
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper2 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper2()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.CommentURL == null)
				{
					cookie.CommentURL = attrValue;
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper3 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper3()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				cookie.Discard = true;
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper4 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper4()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.Domain == null)
				{
					cookie.Domain = attrValue;
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper5 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper5()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				try
				{
					long maxage = Convert.ToInt64(attrValue);
					if (cookie.MaxAge == MAX_AGE_UNSPECIFIED)
					{
						cookie.MaxAge = maxage;
					}
				}
				catch (NumberFormatException)
				{
					throw new IllegalArgumentException("Illegal cookie max-age attribute");
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper6 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper6()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.Path == null)
				{
					cookie.Path = attrValue;
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper7 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper7()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.Portlist == null)
				{
					cookie.Portlist = attrValue == null ? "" : attrValue;
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper8 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper8()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				cookie.Secure = true;
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper9 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper9()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				cookie.HttpOnly = true;
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper10 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper10()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				try
				{
					int version = Convert.ToInt32(attrValue);
					cookie.Version = version;
				}
				catch (NumberFormatException)
				{
					// Just ignore bogus version, it will default to 0 or 1
				}
			}
		}

		private class CookieAttributeAssignorAnonymousInnerClassHelper11 : CookieAttributeAssignor
		{
			public CookieAttributeAssignorAnonymousInnerClassHelper11()
			{
			}

			public virtual void Assign(HttpCookie cookie, String attrName, String attrValue)
			{
				if (cookie.MaxAge == MAX_AGE_UNSPECIFIED)
				{
					cookie.MaxAge = cookie.ExpiryDate2DeltaSeconds(attrValue);
				}
			}
		}

		private class JavaNetHttpCookieAccessAnonymousInnerClassHelper : sun.misc.JavaNetHttpCookieAccess
		{
			public JavaNetHttpCookieAccessAnonymousInnerClassHelper()
			{
			}

			public virtual IList<HttpCookie> Parse(String header)
			{
				return HttpCookie.Parse(header, true);
			}

			public virtual String Header(HttpCookie cookie)
			{
				return cookie.Header_Renamed;
			}
		}
		private static void AssignAttribute(HttpCookie cookie, String attrName, String attrValue)
		{
			// strip off the surrounding "-sign if there's any
			attrValue = StripOffSurroundingQuote(attrValue);

			CookieAttributeAssignor assignor = Assignors[attrName.ToLowerCase()];
			if (assignor != null)
			{
				assignor.Assign(cookie, attrName, attrValue);
			}
			else
			{
				// Ignore the attribute as per RFC 2965
			}
		}


		/*
		 * Returns the original header this cookie was consructed from, if it was
		 * constructed by parsing a header, otherwise null.
		 */
		private String Header()
		{
			return Header_Renamed;
		}

		/*
		 * Constructs a string representation of this cookie. The string format is
		 * as Netscape spec, but without leading "Cookie:" token.
		 */
		private String ToNetscapeHeaderString()
		{
			return Name + "=" + Value;
		}

		/*
		 * Constructs a string representation of this cookie. The string format is
		 * as RFC 2965/2109, but without leading "Cookie:" token.
		 */
		private String ToRFC2965HeaderString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Name).Append("=\"").Append(Value).Append('"');
			if (Path != null)
			{
				sb.Append(";$Path=\"").Append(Path).Append('"');
			}
			if (Domain != null)
			{
				sb.Append(";$Domain=\"").Append(Domain).Append('"');
			}
			if (Portlist != null)
			{
				sb.Append(";$Port=\"").Append(Portlist).Append('"');
			}

			return sb.ToString();
		}

		internal static readonly TimeZone GMT = TimeZone.GetTimeZone("GMT");

		/*
		 * @param  dateString
		 *         a date string in one of the formats defined in Netscape cookie spec
		 *
		 * @return  delta seconds between this cookie's creation time and the time
		 *          specified by dateString
		 */
		private long ExpiryDate2DeltaSeconds(String dateString)
		{
			DateTime cal = new GregorianCalendar(GMT);
			for (int i = 0; i < COOKIE_DATE_FORMATS.Length; i++)
			{
				SimpleDateFormat df = new SimpleDateFormat(COOKIE_DATE_FORMATS[i], Locale.US);
				cal = new DateTime(1970, 0, 1, 0, 0, 0);
				df.TimeZone = GMT;
				df.Lenient = false;
				df.Set2DigitYearStart(cal.Ticks);
				try
				{
					cal = new DateTime(df.Parse(dateString));
					if (!COOKIE_DATE_FORMATS[i].Contains("yyyy"))
					{
						// 2-digit years following the standard set
						// out it rfc 6265
						int year = cal.Year;
						year %= 100;
						if (year < 70)
						{
							year += 2000;
						}
						else
						{
							year += 1900;
						}
						cal.set(DateTime.YEAR, year);
					}
					return (cal.TimeInMillis - WhenCreated) / 1000;
				}
				catch (Exception)
				{
					// Ignore, try the next date format
				}
			}
			return 0;
		}

		/*
		 * try to guess the cookie version through set-cookie header string
		 */
		private static int GuessCookieVersion(String header)
		{
			int version = 0;

			header = header.ToLowerCase();
			if (header.IndexOf("expires=") != -1)
			{
				// only netscape cookie using 'expires'
				version = 0;
			}
			else if (header.IndexOf("version=") != -1)
			{
				// version is mandatory for rfc 2965/2109 cookie
				version = 1;
			}
			else if (header.IndexOf("max-age") != -1)
			{
				// rfc 2965/2109 use 'max-age'
				version = 1;
			}
			else if (StartsWithIgnoreCase(header, SET_COOKIE2))
			{
				// only rfc 2965 cookie starts with 'set-cookie2'
				version = 1;
			}

			return version;
		}

		private static String StripOffSurroundingQuote(String str)
		{
			if (str != null && str.Length() > 2 && str.CharAt(0) == '"' && str.CharAt(str.Length() - 1) == '"')
			{
				return str.Substring(1, str.Length() - 1 - 1);
			}
			if (str != null && str.Length() > 2 && str.CharAt(0) == '\'' && str.CharAt(str.Length() - 1) == '\'')
			{
				return str.Substring(1, str.Length() - 1 - 1);
			}
			return str;
		}

		private static bool EqualsIgnoreCase(String s, String t)
		{
			if (s == t)
			{
				return true;
			}
			if ((s != null) && (t != null))
			{
				return s.EqualsIgnoreCase(t);
			}
			return false;
		}

		private static bool StartsWithIgnoreCase(String s, String start)
		{
			if (s == null || start == null)
			{
				return false;
			}

			if (s.Length() >= start.Length() && start.EqualsIgnoreCase(s.Substring(0, start.Length())))
			{
				return true;
			}

			return false;
		}

		/*
		 * Split cookie header string according to rfc 2965:
		 *   1) split where it is a comma;
		 *   2) but not the comma surrounding by double-quotes, which is the comma
		 *      inside port list or embeded URIs.
		 *
		 * @param  header
		 *         the cookie header string to split
		 *
		 * @return  list of strings; never null
		 */
		private static IList<String> SplitMultiCookies(String header)
		{
			IList<String> cookies = new List<String>();
			int quoteCount = 0;
			int p, q;

			for (p = 0, q = 0; p < header.Length(); p++)
			{
				char c = header.CharAt(p);
				if (c == '"')
				{
					quoteCount++;
				}
				if (c == ',' && (quoteCount % 2 == 0))
				{
					// it is comma and not surrounding by double-quotes
					cookies.Add(header.Substring(q, p - q));
					q = p + 1;
				}
			}

			cookies.Add(header.Substring(q));

			return cookies;
		}
	}

}