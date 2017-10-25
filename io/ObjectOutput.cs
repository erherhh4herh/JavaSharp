/*
 * Copyright (c) 1996, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// ObjectOutput extends the DataOutput interface to include writing of objects.
	/// DataOutput includes methods for output of primitive types, ObjectOutput
	/// extends that interface to include objects, arrays, and Strings.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref= java.io.InputStream </seealso>
	/// <seealso cref= java.io.ObjectOutputStream </seealso>
	/// <seealso cref= java.io.ObjectInputStream
	/// @since   JDK1.1 </seealso>
	public interface ObjectOutput : DataOutput, AutoCloseable
	{
		/// <summary>
		/// Write an object to the underlying storage or stream.  The
		/// class that implements this interface defines how the object is
		/// written.
		/// </summary>
		/// <param name="obj"> the object to be written </param>
		/// <exception cref="IOException"> Any of the usual Input/Output related exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeObject(Object obj) throws IOException;
		void WriteObject(Object obj);

		/// <summary>
		/// Writes a byte. This method will block until the byte is actually
		/// written. </summary>
		/// <param name="b"> the byte </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException;
		void Write(int b);

		/// <summary>
		/// Writes an array of bytes. This method will block until the bytes
		/// are actually written. </summary>
		/// <param name="b"> the data to be written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[]) throws IOException;
		void Write(sbyte[] b);

		/// <summary>
		/// Writes a sub array of bytes. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off">       the start offset in the data </param>
		/// <param name="len">       the number of bytes that are written </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws IOException;
		void Write(sbyte[] b, int off, int len);

		/// <summary>
		/// Flushes the stream. This will write any buffered
		/// output bytes. </summary>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException;
		void Flush();

		/// <summary>
		/// Closes the stream. This method must be called
		/// to release any resources associated with the
		/// stream. </summary>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException;
		void Close();
	}

}