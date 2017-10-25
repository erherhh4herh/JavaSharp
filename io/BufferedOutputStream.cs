/*
 * Copyright (c) 1994, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// The class implements a buffered output stream. By setting up such
	/// an output stream, an application can write bytes to the underlying
	/// output stream without necessarily causing a call to the underlying
	/// system for each byte written.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </summary>
	public class BufferedOutputStream : FilterOutputStream
	{
		/// <summary>
		/// The internal buffer where data is stored.
		/// </summary>
		protected internal sbyte[] Buf;

		/// <summary>
		/// The number of valid bytes in the buffer. This value is always
		/// in the range <tt>0</tt> through <tt>buf.length</tt>; elements
		/// <tt>buf[0]</tt> through <tt>buf[count-1]</tt> contain valid
		/// byte data.
		/// </summary>
		protected internal int Count;

		/// <summary>
		/// Creates a new buffered output stream to write data to the
		/// specified underlying output stream.
		/// </summary>
		/// <param name="out">   the underlying output stream. </param>
		public BufferedOutputStream(OutputStream @out) : this(@out, 8192)
		{
		}

		/// <summary>
		/// Creates a new buffered output stream to write data to the
		/// specified underlying output stream with the specified buffer
		/// size.
		/// </summary>
		/// <param name="out">    the underlying output stream. </param>
		/// <param name="size">   the buffer size. </param>
		/// <exception cref="IllegalArgumentException"> if size &lt;= 0. </exception>
		public BufferedOutputStream(OutputStream @out, int size) : base(@out)
		{
			if (size <= 0)
			{
				throw new IllegalArgumentException("Buffer size <= 0");
			}
			Buf = new sbyte[size];
		}

		/// <summary>
		/// Flush the internal buffer </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void flushBuffer() throws IOException
		private void FlushBuffer()
		{
			if (Count > 0)
			{
				@out.Write(Buf, 0, Count);
				Count = 0;
			}
		}

		/// <summary>
		/// Writes the specified byte to this buffered output stream.
		/// </summary>
		/// <param name="b">   the byte to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(int b) throws IOException
		public override void Write(int b)
		{
			lock (this)
			{
				if (Count >= Buf.Length)
				{
					FlushBuffer();
				}
				Buf[Count++] = (sbyte)b;
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to this buffered output stream.
		/// 
		/// <para> Ordinarily this method stores bytes from the given array into this
		/// stream's buffer, flushing the buffer to the underlying output stream as
		/// needed.  If the requested length is at least as large as this stream's
		/// buffer, however, then this method will flush the buffer and write the
		/// bytes directly to the underlying output stream.  Thus redundant
		/// <code>BufferedOutputStream</code>s will not copy data unnecessarily.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(byte b[] , int off, int len) throws IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				if (len >= Buf.Length)
				{
					/* If the request length exceeds the size of the output buffer,
					   flush the output buffer and then write the data directly.
					   In this way buffered streams will cascade harmlessly. */
					FlushBuffer();
					@out.Write(b, off, len);
					return;
				}
				if (len > Buf.Length - Count)
				{
					FlushBuffer();
				}
				System.Array.Copy(b, off, Buf, Count, len);
				Count += len;
			}
		}

		/// <summary>
		/// Flushes this buffered output stream. This forces any buffered
		/// output bytes to be written out to the underlying output stream.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void flush() throws IOException
		public override void Flush()
		{
			lock (this)
			{
				FlushBuffer();
				@out.Flush();
			}
		}
	}

}