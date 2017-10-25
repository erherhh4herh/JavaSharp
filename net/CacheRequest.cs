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
	/// Represents channels for storing resources in the
	/// ResponseCache. Instances of such a class provide an
	/// OutputStream object which is called by protocol handlers to
	/// store the resource data into the cache, and also an abort() method
	/// which allows a cache store operation to be interrupted and
	/// abandoned. If an IOException is encountered while reading the
	/// response or writing to the cache, the current cache store operation
	/// will be aborted.
	/// 
	/// @author Yingxian Wang
	/// @since 1.5
	/// </summary>
	public abstract class CacheRequest
	{

		/// <summary>
		/// Returns an OutputStream to which the response body can be
		/// written.
		/// </summary>
		/// <returns> an OutputStream to which the response body can
		///         be written </returns>
		/// <exception cref="IOException"> if an I/O error occurs while
		///         writing the response body </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.io.OutputStream getBody() throws java.io.IOException;
		public abstract OutputStream Body {get;}

		/// <summary>
		/// Aborts the attempt to cache the response. If an IOException is
		/// encountered while reading the response or writing to the cache,
		/// the current cache store operation will be abandoned.
		/// </summary>
		public abstract void Abort();
	}

}