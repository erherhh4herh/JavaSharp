/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	/// <summary>
	/// A <tt>Readable</tt> is a source of characters. Characters from
	/// a <tt>Readable</tt> are made available to callers of the read
	/// method via a <seealso cref="java.nio.CharBuffer CharBuffer"/>.
	/// 
	/// @since 1.5
	/// </summary>
	public interface Readable
	{

		/// <summary>
		/// Attempts to read characters into the specified character buffer.
		/// The buffer is used as a repository of characters as-is: the only
		/// changes made are the results of a put operation. No flipping or
		/// rewinding of the buffer is performed.
		/// </summary>
		/// <param name="cb"> the buffer to read characters into </param>
		/// <returns> The number of {@code char} values added to the buffer,
		///                 or -1 if this source of characters is at its end </returns>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
		/// <exception cref="NullPointerException"> if cb is null </exception>
		/// <exception cref="java.nio.ReadOnlyBufferException"> if cb is a read only buffer </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(java.nio.CharBuffer cb) throws java.io.IOException;
		int Read(java.nio.CharBuffer cb);
	}

}