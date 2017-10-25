/*
 * Copyright (c) 1994, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// This abstract class is the superclass of all classes representing
	/// an output stream of bytes. An output stream accepts output bytes
	/// and sends them to some sink.
	/// <para>
	/// Applications that need to define a subclass of
	/// <code>OutputStream</code> must always provide at least a method
	/// that writes one byte of output.
	/// 
	/// @author  Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.BufferedOutputStream </seealso>
	/// <seealso cref=     java.io.ByteArrayOutputStream </seealso>
	/// <seealso cref=     java.io.DataOutputStream </seealso>
	/// <seealso cref=     java.io.FilterOutputStream </seealso>
	/// <seealso cref=     java.io.InputStream </seealso>
	/// <seealso cref=     java.io.OutputStream#write(int)
	/// @since   JDK1.0 </seealso>
	public abstract class OutputStream : Closeable, Flushable
	{
		/// <summary>
		/// Writes the specified byte to this output stream. The general
		/// contract for <code>write</code> is that one byte is written
		/// to the output stream. The byte to be written is the eight
		/// low-order bits of the argument <code>b</code>. The 24
		/// high-order bits of <code>b</code> are ignored.
		/// <para>
		/// Subclasses of <code>OutputStream</code> must provide an
		/// implementation for this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the <code>byte</code>. </param>
		/// <exception cref="IOException">  if an I/O error occurs. In particular,
		///             an <code>IOException</code> may be thrown if the
		///             output stream has been closed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void write(int b) throws IOException;
		public abstract void Write(int b);

		/// <summary>
		/// Writes <code>b.length</code> bytes from the specified byte array
		/// to this output stream. The general contract for <code>write(b)</code>
		/// is that it should have exactly the same effect as the call
		/// <code>write(b, 0, b.length)</code>.
		/// </summary>
		/// <param name="b">   the data. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.OutputStream#write(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[]) throws IOException
		public virtual void Write(sbyte[] b)
		{
			Write(b, 0, b.Length);
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to this output stream.
		/// The general contract for <code>write(b, off, len)</code> is that
		/// some of the bytes in the array <code>b</code> are written to the
		/// output stream in order; element <code>b[off]</code> is the first
		/// byte written and <code>b[off+len-1]</code> is the last byte written
		/// by this operation.
		/// <para>
		/// The <code>write</code> method of <code>OutputStream</code> calls
		/// the write method of one argument on each of the bytes to be
		/// written out. Subclasses are encouraged to override this method and
		/// provide a more efficient implementation.
		/// </para>
		/// <para>
		/// If <code>b</code> is <code>null</code>, a
		/// <code>NullPointerException</code> is thrown.
		/// </para>
		/// <para>
		/// If <code>off</code> is negative, or <code>len</code> is negative, or
		/// <code>off+len</code> is greater than the length of the array
		/// <code>b</code>, then an <tt>IndexOutOfBoundsException</tt> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException">  if an I/O error occurs. In particular,
		///             an <code>IOException</code> is thrown if the output
		///             stream is closed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws IOException
		public virtual void Write(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			else if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return;
			}
			for (int i = 0 ; i < len ; i++)
			{
				Write(b[off + i]);
			}
		}

		/// <summary>
		/// Flushes this output stream and forces any buffered output bytes
		/// to be written out. The general contract of <code>flush</code> is
		/// that calling it is an indication that, if any bytes previously
		/// written have been buffered by the implementation of the output
		/// stream, such bytes should immediately be written to their
		/// intended destination.
		/// <para>
		/// If the intended destination of this stream is an abstraction provided by
		/// the underlying operating system, for example a file, then flushing the
		/// stream guarantees only that bytes previously written to the stream are
		/// passed to the operating system for writing; it does not guarantee that
		/// they are actually written to a physical device such as a disk drive.
		/// </para>
		/// <para>
		/// The <code>flush</code> method of <code>OutputStream</code> does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public virtual void Flush()
		{
		}

		/// <summary>
		/// Closes this output stream and releases any system resources
		/// associated with this stream. The general contract of <code>close</code>
		/// is that it closes the output stream. A closed stream cannot perform
		/// output operations and cannot be reopened.
		/// <para>
		/// The <code>close</code> method of <code>OutputStream</code> does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public virtual void Close()
		{
		}

	}

}