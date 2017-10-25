using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// A simple in-memory java.net.CookieStore implementation
	/// 
	/// @author Edward Wang
	/// @since 1.6
	/// </summary>
	internal class InMemoryCookieStore : CookieStore
	{
		// the in-memory representation of cookies
		private IList<HttpCookie> CookieJar = null;

		// the cookies are indexed by its domain and associated uri (if present)
		// CAUTION: when a cookie removed from main data structure (i.e. cookieJar),
		//          it won't be cleared in domainIndex & uriIndex. Double-check the
		//          presence of cookie when retrieve one form index store.
		private IDictionary<String, IList<HttpCookie>> DomainIndex = null;
		private IDictionary<URI, IList<HttpCookie>> UriIndex = null;

		// use ReentrantLock instead of syncronized for scalability
		private ReentrantLock @lock = null;


		/// <summary>
		/// The default ctor
		/// </summary>
		public InMemoryCookieStore()
		{
			CookieJar = new List<HttpCookie>();
			DomainIndex = new Dictionary<String, IList<HttpCookie>>();
			UriIndex = new Dictionary<URI, IList<HttpCookie>>();

			@lock = new ReentrantLock(false);
		}

		/// <summary>
		/// Add one cookie into cookie store.
		/// </summary>
		public virtual void Add(URI uri, HttpCookie cookie)
		{
			// pre-condition : argument can't be null
			if (cookie == null)
			{
				throw new NullPointerException("cookie is null");
			}


			@lock.@lock();
			try
			{
				// remove the ole cookie if there has had one
				CookieJar.Remove(cookie);

				// add new cookie if it has a non-zero max-age
				if (cookie.MaxAge != 0)
				{
					CookieJar.Add(cookie);
					// and add it to domain index
					if (cookie.Domain != null)
					{
						AddIndex(DomainIndex, cookie.Domain, cookie);
					}
					if (uri != null)
					{
						// add it to uri index, too
						AddIndex(UriIndex, GetEffectiveURI(uri), cookie);
					}
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}


		/// <summary>
		/// Get all cookies, which:
		///  1) given uri domain-matches with, or, associated with
		///     given uri when added to the cookie store.
		///  3) not expired.
		/// See RFC 2965 sec. 3.3.4 for more detail.
		/// </summary>
		public virtual IList<HttpCookie> Get(URI uri)
		{
			// argument can't be null
			if (uri == null)
			{
				throw new NullPointerException("uri is null");
			}

			IList<HttpCookie> cookies = new List<HttpCookie>();
			bool secureLink = "https".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase);
			@lock.@lock();
			try
			{
				// check domainIndex first
				GetInternal1(cookies, DomainIndex, uri.Host, secureLink);
				// check uriIndex then
				GetInternal2(cookies, UriIndex, GetEffectiveURI(uri), secureLink);
			}
			finally
			{
				@lock.Unlock();
			}

			return cookies;
		}

		/// <summary>
		/// Get all cookies in cookie store, except those have expired
		/// </summary>
		public virtual IList<HttpCookie> Cookies
		{
			get
			{
				IList<HttpCookie> rt;
    
				@lock.@lock();
				try
				{
					IEnumerator<HttpCookie> it = CookieJar.GetEnumerator();
					while (it.MoveNext())
					{
						if (it.Current.hasExpired())
						{
							it.remove();
						}
					}
				}
				finally
				{
					rt = Collections.UnmodifiableList(CookieJar);
					@lock.Unlock();
				}
    
				return rt;
			}
		}

		/// <summary>
		/// Get all URIs, which are associated with at least one cookie
		/// of this cookie store.
		/// </summary>
		public virtual IList<URI> URIs
		{
			get
			{
				IList<URI> uris = new List<URI>();
    
				@lock.@lock();
				try
				{
					IEnumerator<URI> it = UriIndex.Keys.GetEnumerator();
					while (it.MoveNext())
					{
						URI uri = it.Current;
						IList<HttpCookie> cookies = UriIndex[uri];
						if (cookies == null || cookies.Count == 0)
						{
							// no cookies list or an empty list associated with
							// this uri entry, delete it
							it.remove();
						}
					}
				}
				finally
				{
					uris.AddRange(UriIndex.Keys);
					@lock.Unlock();
				}
    
				return uris;
			}
		}


		/// <summary>
		/// Remove a cookie from store
		/// </summary>
		public virtual bool Remove(URI uri, HttpCookie ck)
		{
			// argument can't be null
			if (ck == null)
			{
				throw new NullPointerException("cookie is null");
			}

			bool modified = false;
			@lock.@lock();
			try
			{
				modified = CookieJar.Remove(ck);
			}
			finally
			{
				@lock.Unlock();
			}

			return modified;
		}


		/// <summary>
		/// Remove all cookies in this cookie store.
		/// </summary>
		public virtual bool RemoveAll()
		{
			@lock.@lock();
			try
			{
				if (CookieJar.Count == 0)
				{
					return false;
				}
				CookieJar.Clear();
				DomainIndex.Clear();
				UriIndex.Clear();
			}
			finally
			{
				@lock.Unlock();
			}

			return true;
		}


		/* ---------------- Private operations -------------- */


		/*
		 * This is almost the same as HttpCookie.domainMatches except for
		 * one difference: It won't reject cookies when the 'H' part of the
		 * domain contains a dot ('.').
		 * I.E.: RFC 2965 section 3.3.2 says that if host is x.y.domain.com
		 * and the cookie domain is .domain.com, then it should be rejected.
		 * However that's not how the real world works. Browsers don't reject and
		 * some sites, like yahoo.com do actually expect these cookies to be
		 * passed along.
		 * And should be used for 'old' style cookies (aka Netscape type of cookies)
		 */
		private bool NetscapeDomainMatches(String domain, String host)
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

			// if the host name contains no dot and the domain name is .local
			int firstDotInHost = host.IndexOf('.');
			if (firstDotInHost == -1 && isLocalDomain)
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

				return (D.EqualsIgnoreCase(domain));
			}
			else if (lengthDiff == -1)
			{
				// if domain is actually .host
				return (domain.CharAt(0) == '.' && host.EqualsIgnoreCase(domain.Substring(1)));
			}

			return false;
		}

		private void GetInternal1(IList<HttpCookie> cookies, IDictionary<String, IList<HttpCookie>> cookieIndex, String host, bool secureLink)
		{
			// Use a separate list to handle cookies that need to be removed so
			// that there is no conflict with iterators.
			List<HttpCookie> toRemove = new List<HttpCookie>();
			foreach (java.util.Map_Entry<String, IList<HttpCookie>> entry in cookieIndex)
			{
				String domain = entry.Key;
				IList<HttpCookie> lst = entry.Value;
				foreach (HttpCookie c in lst)
				{
					if ((c.Version == 0 && NetscapeDomainMatches(domain, host)) || (c.Version == 1 && HttpCookie.DomainMatches(domain, host)))
					{
						if ((CookieJar.IndexOf(c) != -1))
						{
							// the cookie still in main cookie store
							if (!c.HasExpired())
							{
								// don't add twice and make sure it's the proper
								// security level
								if ((secureLink || !c.Secure) && !cookies.Contains(c))
								{
									cookies.Add(c);
								}
							}
							else
							{
								toRemove.Add(c);
							}
						}
						else
						{
							// the cookie has beed removed from main store,
							// so also remove it from domain indexed store
							toRemove.Add(c);
						}
					}
				}
				// Clear up the cookies that need to be removed
				foreach (HttpCookie c in toRemove)
				{
					lst.Remove(c);
					CookieJar.Remove(c);

				}
				toRemove.Clear();
			}
		}

		// @param cookies           [OUT] contains the found cookies
		// @param cookieIndex       the index
		// @param comparator        the prediction to decide whether or not
		//                          a cookie in index should be returned
		private void getInternal2<T>(IList<HttpCookie> cookies, IDictionary<T, IList<HttpCookie>> cookieIndex, Comparable<T> comparator, bool secureLink)
		{
			foreach (T index in cookieIndex.Keys)
			{
				if (comparator.CompareTo(index) == 0)
				{
					IList<HttpCookie> indexedCookies = cookieIndex[index];
					// check the list of cookies associated with this domain
					if (indexedCookies != null)
					{
						IEnumerator<HttpCookie> it = indexedCookies.GetEnumerator();
						while (it.MoveNext())
						{
							HttpCookie ck = it.Current;
							if (CookieJar.IndexOf(ck) != -1)
							{
								// the cookie still in main cookie store
								if (!ck.HasExpired())
								{
									// don't add twice
									if ((secureLink || !ck.Secure) && !cookies.Contains(ck))
									{
										cookies.Add(ck);
									}
								}
								else
								{
									it.remove();
									CookieJar.Remove(ck);
								}
							}
							else
							{
								// the cookie has beed removed from main store,
								// so also remove it from domain indexed store
								it.remove();
							}
						}
					} // end of indexedCookies != null
				} // end of comparator.compareTo(index) == 0
			} // end of cookieIndex iteration
		}

		// add 'cookie' indexed by 'index' into 'indexStore'
		private void addIndex<T>(IDictionary<T, IList<HttpCookie>> indexStore, T index, HttpCookie cookie)
		{
			if (index != null)
			{
				IList<HttpCookie> cookies = indexStore[index];
				if (cookies != null)
				{
					// there may already have the same cookie, so remove it first
					cookies.Remove(cookie);

					cookies.Add(cookie);
				}
				else
				{
					cookies = new List<HttpCookie>();
					cookies.Add(cookie);
					indexStore[index] = cookies;
				}
			}
		}


		//
		// for cookie purpose, the effective uri should only be http://host
		// the path will be taken into account when path-match algorithm applied
		//
		private URI GetEffectiveURI(URI uri)
		{
			URI effectiveURI = null;
			try
			{
				effectiveURI = new URI("http", uri.Host, null, null, null); // fragment component -  query component -  path component
			}
			catch (URISyntaxException)
			{
				effectiveURI = uri;
			}

			return effectiveURI;
		}
	}

}