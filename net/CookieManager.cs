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

	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// CookieManager provides a concrete implementation of <seealso cref="CookieHandler"/>,
	/// which separates the storage of cookies from the policy surrounding accepting
	/// and rejecting cookies. A CookieManager is initialized with a <seealso cref="CookieStore"/>
	/// which manages storage, and a <seealso cref="CookiePolicy"/> object, which makes
	/// policy decisions on cookie acceptance/rejection.
	/// 
	/// <para> The HTTP cookie management in java.net package looks like:
	/// <blockquote>
	/// <pre>{@code
	///                  use
	/// CookieHandler <------- HttpURLConnection
	///       ^
	///       | impl
	///       |         use
	/// CookieManager -------> CookiePolicy
	///             |   use
	///             |--------> HttpCookie
	///             |              ^
	///             |              | use
	///             |   use        |
	///             |--------> CookieStore
	///                            ^
	///                            | impl
	///                            |
	///                  Internal in-memory implementation
	/// }</pre>
	/// <ul>
	///   <li>
	///     CookieHandler is at the core of cookie management. User can call
	///     CookieHandler.setDefault to set a concrete CookieHanlder implementation
	///     to be used.
	///   </li>
	///   <li>
	///     CookiePolicy.shouldAccept will be called by CookieManager.put to see whether
	///     or not one cookie should be accepted and put into cookie store. User can use
	///     any of three pre-defined CookiePolicy, namely ACCEPT_ALL, ACCEPT_NONE and
	///     ACCEPT_ORIGINAL_SERVER, or user can define his own CookiePolicy implementation
	///     and tell CookieManager to use it.
	///   </li>
	///   <li>
	///     CookieStore is the place where any accepted HTTP cookie is stored in.
	///     If not specified when created, a CookieManager instance will use an internal
	///     in-memory implementation. Or user can implements one and tell CookieManager
	///     to use it.
	///   </li>
	///   <li>
	///     Currently, only CookieStore.add(URI, HttpCookie) and CookieStore.get(URI)
	///     are used by CookieManager. Others are for completeness and might be needed
	///     by a more sophisticated CookieStore implementation, e.g. a NetscapeCookieSotre.
	///   </li>
	/// </ul>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>There're various ways user can hook up his own HTTP cookie management behavior, e.g.
	/// <blockquote>
	/// <ul>
	///   <li>Use CookieHandler.setDefault to set a brand new <seealso cref="CookieHandler"/> implementation
	///   <li>Let CookieManager be the default <seealso cref="CookieHandler"/> implementation,
	///       but implement user's own <seealso cref="CookieStore"/> and <seealso cref="CookiePolicy"/>
	///       and tell default CookieManager to use them:
	///     <blockquote><pre>
	///       // this should be done at the beginning of an HTTP session
	///       CookieHandler.setDefault(new CookieManager(new MyCookieStore(), new MyCookiePolicy()));
	///     </pre></blockquote>
	///   <li>Let CookieManager be the default <seealso cref="CookieHandler"/> implementation, but
	///       use customized <seealso cref="CookiePolicy"/>:
	///     <blockquote><pre>
	///       // this should be done at the beginning of an HTTP session
	///       CookieHandler.setDefault(new CookieManager());
	///       // this can be done at any point of an HTTP session
	///       ((CookieManager)CookieHandler.getDefault()).setCookiePolicy(new MyCookiePolicy());
	///     </pre></blockquote>
	/// </ul>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>The implementation conforms to <a href="http://www.ietf.org/rfc/rfc2965.txt">RFC 2965</a>, section 3.3.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CookiePolicy
	/// @author Edward Wang
	/// @since 1.6 </seealso>
	public class CookieManager : CookieHandler
	{
		/* ---------------- Fields -------------- */

		private CookiePolicy PolicyCallback;


		private CookieStore CookieJar = null;


		/* ---------------- Ctors -------------- */

		/// <summary>
		/// Create a new cookie manager.
		/// 
		/// <para>This constructor will create new cookie manager with default
		/// cookie store and accept policy. The effect is same as
		/// {@code CookieManager(null, null)}.
		/// </para>
		/// </summary>
		public CookieManager() : this(null, null)
		{
		}


		/// <summary>
		/// Create a new cookie manager with specified cookie store and cookie policy.
		/// </summary>
		/// <param name="store">     a {@code CookieStore} to be used by cookie manager.
		///                  if {@code null}, cookie manager will use a default one,
		///                  which is an in-memory CookieStore implementation. </param>
		/// <param name="cookiePolicy">      a {@code CookiePolicy} instance
		///                          to be used by cookie manager as policy callback.
		///                          if {@code null}, ACCEPT_ORIGINAL_SERVER will
		///                          be used. </param>
		public CookieManager(CookieStore store, CookiePolicy cookiePolicy)
		{
			// use default cookie policy if not specify one
			PolicyCallback = (cookiePolicy == null) ? CookiePolicy.ACCEPT_ORIGINAL_SERVER : cookiePolicy;

			// if not specify CookieStore to use, use default one
			if (store == null)
			{
				CookieJar = new InMemoryCookieStore();
			}
			else
			{
				CookieJar = store;
			}
		}


		/* ---------------- Public operations -------------- */

		/// <summary>
		/// To set the cookie policy of this cookie manager.
		/// 
		/// <para> A instance of {@code CookieManager} will have
		/// cookie policy ACCEPT_ORIGINAL_SERVER by default. Users always
		/// can call this method to set another cookie policy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cookiePolicy">      the cookie policy. Can be {@code null}, which
		///                          has no effects on current cookie policy. </param>
		public virtual CookiePolicy CookiePolicy
		{
			set
			{
				if (value != null)
				{
					PolicyCallback = value;
				}
			}
		}


		/// <summary>
		/// To retrieve current cookie store.
		/// </summary>
		/// <returns>  the cookie store currently used by cookie manager. </returns>
		public virtual CookieStore CookieStore
		{
			get
			{
				return CookieJar;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Map<String, java.util.List<String>> get(URI uri, java.util.Map<String, java.util.List<String>> requestHeaders) throws java.io.IOException
		public override IDictionary<String, IList<String>> Get(URI uri, IDictionary<String, IList<String>> requestHeaders)
		{
			// pre-condition check
			if (uri == null || requestHeaders == null)
			{
				throw new IllegalArgumentException("Argument is null");
			}

			IDictionary<String, IList<String>> cookieMap = new Dictionary<String, IList<String>>();
			// if there's no default CookieStore, no way for us to get any cookie
			if (CookieJar == null)
			{
				return Collections.UnmodifiableMap(cookieMap);
			}

			bool secureLink = "https".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase);
			IList<HttpCookie> cookies = new List<HttpCookie>();
			String path = uri.Path;
			if (path == null || path.Empty)
			{
				path = "/";
			}
			foreach (HttpCookie cookie in CookieJar.Get(uri))
			{
				// apply path-matches rule (RFC 2965 sec. 3.3.4)
				// and check for the possible "secure" tag (i.e. don't send
				// 'secure' cookies over unsecure links)
				if (PathMatches(path, cookie.Path) && (secureLink || !cookie.Secure))
				{
					// Enforce httponly attribute
					if (cookie.HttpOnly)
					{
						String s = uri.Scheme;
						if (!"http".Equals(s, StringComparison.CurrentCultureIgnoreCase) && !"https".Equals(s, StringComparison.CurrentCultureIgnoreCase))
						{
							continue;
						}
					}
					// Let's check the authorize port list if it exists
					String ports = cookie.Portlist;
					if (ports != null && !ports.Empty)
					{
						int port = uri.Port;
						if (port == -1)
						{
							port = "https".Equals(uri.Scheme) ? 443 : 80;
						}
						if (IsInPortList(ports, port))
						{
							cookies.Add(cookie);
						}
					}
					else
					{
						cookies.Add(cookie);
					}
				}
			}

			// apply sort rule (RFC 2965 sec. 3.3.4)
			IList<String> cookieHeader = SortByPath(cookies);

			cookieMap["Cookie"] = cookieHeader;
			return Collections.UnmodifiableMap(cookieMap);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void put(URI uri, java.util.Map<String, java.util.List<String>> responseHeaders) throws java.io.IOException
		public override void Put(URI uri, IDictionary<String, IList<String>> responseHeaders)
		{
			// pre-condition check
			if (uri == null || responseHeaders == null)
			{
				throw new IllegalArgumentException("Argument is null");
			}


			// if there's no default CookieStore, no need to remember any cookie
			if (CookieJar == null)
			{
				return;
			}

		PlatformLogger logger = PlatformLogger.getLogger("java.net.CookieManager");
			foreach (String headerKey in responseHeaders.Keys)
			{
				// RFC 2965 3.2.2, key must be 'Set-Cookie2'
				// we also accept 'Set-Cookie' here for backward compatibility
				if (headerKey == null || !(headerKey.EqualsIgnoreCase("Set-Cookie2") || headerKey.EqualsIgnoreCase("Set-Cookie")))
				{
					continue;
				}

				foreach (String headerValue in responseHeaders[headerKey])
				{
					try
					{
						IList<HttpCookie> cookies;
						try
						{
							cookies = HttpCookie.Parse(headerValue);
						}
						catch (IllegalArgumentException)
						{
							// Bogus header, make an empty list and log the error
							cookies = Collections.EmptyList();
							if (logger.isLoggable(PlatformLogger.Level.SEVERE))
							{
								logger.severe("Invalid cookie for " + uri + ": " + headerValue);
							}
						}
						foreach (HttpCookie cookie in cookies)
						{
							if (cookie.Path == null)
							{
								// If no path is specified, then by default
								// the path is the directory of the page/doc
								String path = uri.Path;
								if (!path.EndsWith("/"))
								{
									int i = path.LastIndexOf("/");
									if (i > 0)
									{
										path = path.Substring(0, i + 1);
									}
									else
									{
										path = "/";
									}
								}
								cookie.Path = path;
							}

							// As per RFC 2965, section 3.3.1:
							// Domain  Defaults to the effective request-host.  (Note that because
							// there is no dot at the beginning of effective request-host,
							// the default Domain can only domain-match itself.)
							if (cookie.Domain == null)
							{
								String host = uri.Host;
								if (host != null && !host.Contains("."))
								{
									host += ".local";
								}
								cookie.Domain = host;
							}
							String ports = cookie.Portlist;
							if (ports != null)
							{
								int port = uri.Port;
								if (port == -1)
								{
									port = "https".Equals(uri.Scheme) ? 443 : 80;
								}
								if (ports.Empty)
								{
									// Empty port list means this should be restricted
									// to the incoming URI port
									cookie.Portlist = "" + port;
									if (ShouldAcceptInternal(uri, cookie))
									{
										CookieJar.Add(uri, cookie);
									}
								}
								else
								{
									// Only store cookies with a port list
									// IF the URI port is in that list, as per
									// RFC 2965 section 3.3.2
									if (IsInPortList(ports, port) && ShouldAcceptInternal(uri, cookie))
									{
										CookieJar.Add(uri, cookie);
									}
								}
							}
							else
							{
								if (ShouldAcceptInternal(uri, cookie))
								{
									CookieJar.Add(uri, cookie);
								}
							}
						}
					}
					catch (IllegalArgumentException)
					{
						// invalid set-cookie header string
						// no-op
					}
				}
			}
		}


		/* ---------------- Private operations -------------- */

		// to determine whether or not accept this cookie
		private bool ShouldAcceptInternal(URI uri, HttpCookie cookie)
		{
			try
			{
				return PolicyCallback.ShouldAccept(uri, cookie);
			} // pretect against malicious callback
			catch (Exception)
			{
				return false;
			}
		}


		private static bool IsInPortList(String lst, int port)
		{
			int i = lst.IndexOf(",");
			int val = -1;
			while (i > 0)
			{
				try
				{
					val = Convert.ToInt32(lst.Substring(0, i));
					if (val == port)
					{
						return true;
					}
				}
				catch (NumberFormatException)
				{
				}
				lst = lst.Substring(i + 1);
				i = lst.IndexOf(",");
			}
			if (!lst.Empty)
			{
				try
				{
					val = Convert.ToInt32(lst);
					if (val == port)
					{
						return true;
					}
				}
				catch (NumberFormatException)
				{
				}
			}
			return false;
		}

		/*
		 * path-matches algorithm, as defined by RFC 2965
		 */
		private bool PathMatches(String path, String pathToMatchWith)
		{
			if (path == pathToMatchWith)
			{
				return true;
			}
			if (path == null || pathToMatchWith == null)
			{
				return false;
			}
			if (path.StartsWith(pathToMatchWith))
			{
				return true;
			}

			return false;
		}


		/*
		 * sort cookies with respect to their path: those with more specific Path attributes
		 * precede those with less specific, as defined in RFC 2965 sec. 3.3.4
		 */
		private IList<String> SortByPath(IList<HttpCookie> cookies)
		{
			cookies.Sort(new CookiePathComparator());

			IList<String> cookieHeader = new List<String>();
			foreach (HttpCookie cookie in cookies)
			{
				// Netscape cookie spec and RFC 2965 have different format of Cookie
				// header; RFC 2965 requires a leading $Version="1" string while Netscape
				// does not.
				// The workaround here is to add a $Version="1" string in advance
				if (cookies.IndexOf(cookie) == 0 && cookie.Version > 0)
				{
					cookieHeader.Add("$Version=\"1\"");
				}

				cookieHeader.Add(cookie.ToString());
			}
			return cookieHeader;
		}


		internal class CookiePathComparator : Comparator<HttpCookie>
		{
			public virtual int Compare(HttpCookie c1, HttpCookie c2)
			{
				if (c1 == c2)
				{
					return 0;
				}
				if (c1 == null)
				{
					return -1;
				}
				if (c2 == null)
				{
					return 1;
				}

				// path rule only applies to the cookies with same name
				if (!c1.Name.Equals(c2.Name))
				{
					return 0;
				}

				// those with more specific Path attributes precede those with less specific
				if (c1.Path.StartsWith(c2.Path))
				{
					return -1;
				}
				else if (c2.Path.StartsWith(c1.Path))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}

}