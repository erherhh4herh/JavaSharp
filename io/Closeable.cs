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

namespace java.io
{

	/// <summary>
	/// A {@code Closeable} is a source or destination of data that can be closed.
	/// The close method is invoked to release resources that the object is
	/// holding (such as open files).
	/// 
	/// @since 1.5
	/// </summary>
	public interface Closeable : AutoCloseable
	{

		/// <summary>
		/// Closes this stream and releases any system resources associated
		/// with it. If the stream is already closed then invoking this
		/// method has no effect.
		/// 
		/// <para> As noted in <seealso cref="AutoCloseable#close()"/>, cases where the
		/// close may fail require careful attention. It is strongly advised
		/// to relinquish the underlying resources and to internally
		/// <em>mark</em> the {@code Closeable} as closed, prior to throwing
		/// the {@code IOException}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException;
		void Close();
	}

}