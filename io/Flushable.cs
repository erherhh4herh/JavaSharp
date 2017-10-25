/*
 * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// A <tt>Flushable</tt> is a destination of data that can be flushed.  The
	/// flush method is invoked to write any buffered output to the underlying
	/// stream.
	/// 
	/// @since 1.5
	/// </summary>
	public interface Flushable
	{

		/// <summary>
		/// Flushes this stream by writing any buffered output to the underlying
		/// stream.
		/// </summary>
		/// <exception cref="IOException"> If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void flush() throws java.io.IOException;
		void Flush();
	}

}