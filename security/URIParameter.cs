/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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


namespace java.security
{

	/// <summary>
	/// A parameter that contains a URI pointing to data intended for a
	/// PolicySpi or ConfigurationSpi implementation.
	/// 
	/// @since 1.6
	/// </summary>
	public class URIParameter : Policy.Parameters, javax.security.auth.login.Configuration.Parameters
	{

		private java.net.URI Uri;

		/// <summary>
		/// Constructs a URIParameter with the URI pointing to
		/// data intended for an SPI implementation.
		/// </summary>
		/// <param name="uri"> the URI pointing to the data.
		/// </param>
		/// <exception cref="NullPointerException"> if the specified URI is null. </exception>
		public URIParameter(java.net.URI uri)
		{
			if (uri == null)
			{
				throw new NullPointerException("invalid null URI");
			}
			this.Uri = uri;
		}

		/// <summary>
		/// Returns the URI.
		/// </summary>
		/// <returns> uri the URI. </returns>
		public virtual java.net.URI URI
		{
			get
			{
				return Uri;
			}
		}
	}

}