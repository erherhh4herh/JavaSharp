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
	/// CookiePolicy implementations decide which cookies should be accepted
	/// and which should be rejected. Three pre-defined policy implementations
	/// are provided, namely ACCEPT_ALL, ACCEPT_NONE and ACCEPT_ORIGINAL_SERVER.
	/// 
	/// <para>See RFC 2965 sec. 3.3 and 7 for more detail.
	/// 
	/// @author Edward Wang
	/// @since 1.6
	/// </para>
	/// </summary>
	public interface CookiePolicy
	{
		/// <summary>
		/// One pre-defined policy which accepts all cookies.
		/// </summary>
//		public static final CookiePolicy ACCEPT_ALL = new CookiePolicyAnonymousInnerClassHelper();

		/// <summary>
		/// One pre-defined policy which accepts no cookies.
		/// </summary>
//		public static final CookiePolicy ACCEPT_NONE = new CookiePolicyAnonymousInnerClassHelper2();

		/// <summary>
		/// One pre-defined policy which only accepts cookies from original server.
		/// </summary>
//		public static final CookiePolicy ACCEPT_ORIGINAL_SERVER = new CookiePolicyAnonymousInnerClassHelper3();


		/// <summary>
		/// Will be called to see whether or not this cookie should be accepted.
		/// </summary>
		/// <param name="uri">       the URI to consult accept policy with </param>
		/// <param name="cookie">    the HttpCookie object in question </param>
		/// <returns>          {@code true} if this cookie should be accepted;
		///                  otherwise, {@code false} </returns>
		bool ShouldAccept(URI uri, HttpCookie cookie);
	}

	public static class CookiePolicy_Fields
	{
				public static readonly return True;

				private class CookiePolicyAnonymousInnerClassHelper : CookiePolicy
				{
					public CookiePolicyAnonymousInnerClassHelper()
					{
					}

					public virtual bool ShouldAccept(URI uri, HttpCookie cookie)
					{
					}
				}

				private class CookiePolicyAnonymousInnerClassHelper2 : CookiePolicy
				{
					public CookiePolicyAnonymousInnerClassHelper2()
					{
					}

					public virtual bool ShouldAccept(URI uri, HttpCookie cookie)
					{
					}
				}

				private class CookiePolicyAnonymousInnerClassHelper3 : CookiePolicy
				{
					public CookiePolicyAnonymousInnerClassHelper3()
					{
					}

					public virtual bool ShouldAccept(URI uri, HttpCookie cookie)
					{
						if (uri == null || cookie == null)
						{
						return HttpCookie.DomainMatches(cookie.Domain, uri.Host);
						}
					}
				}
				public static readonly return False;
					public static readonly return False;
	}

}