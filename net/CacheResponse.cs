using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// Represent channels for retrieving resources from the
	/// ResponseCache. Instances of such a class provide an
	/// InputStream that returns the entity body, and also a
	/// getHeaders() method which returns the associated response headers.
	/// 
	/// @author Yingxian Wang
	/// @since 1.5
	/// </summary>
	public abstract class CacheResponse
	{

		/// <summary>
		/// Returns the response headers as a Map.
		/// </summary>
		/// <returns> An immutable Map from response header field names to
		///         lists of field values. The status line has null as its
		///         field name. </returns>
		/// <exception cref="IOException"> if an I/O error occurs
		///            while getting the response headers </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.Map<String, java.util.List<String>> getHeaders() throws java.io.IOException;
		public abstract IDictionary<String, IList<String>> Headers {get;}

		/// <summary>
		/// Returns the response body as an InputStream.
		/// </summary>
		/// <returns> an InputStream from which the response body can
		///         be accessed </returns>
		/// <exception cref="IOException"> if an I/O error occurs while
		///         getting the response body </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.io.InputStream getBody() throws java.io.IOException;
		public abstract InputStream Body {get;}
	}

}