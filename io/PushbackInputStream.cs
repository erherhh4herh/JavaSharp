/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>PushbackInputStream</code> adds
	/// functionality to another input stream, namely
	/// the  ability to "push back" or "unread"
	/// one byte. This is useful in situations where
	/// it is  convenient for a fragment of code
	/// to read an indefinite number of data bytes
	/// that  are delimited by a particular byte
	/// value; after reading the terminating byte,
	/// the  code fragment can "unread" it, so that
	/// the next read operation on the input stream
	/// will reread the byte that was pushed back.
	/// For example, bytes representing the  characters
	/// constituting an identifier might be terminated
	/// by a byte representing an  operator character;
	/// a method whose job is to read just an identifier
	/// can read until it  sees the operator and
	/// then push the operator back to be re-read.
	/// 
	/// @author  David Connelly
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </summary>
	public class PushbackInputStream : FilterInputStream
	{
		/// <summary>
		/// The pushback buffer.
		/// @since   JDK1.1
		/// </summary>
		protected internal sbyte[] Buf;

		/// <summary>
		/// The position within the pushback buffer from which the next byte will
		/// be read.  When the buffer is empty, <code>pos</code> is equal to
		/// <code>buf.length</code>; when the buffer is full, <code>pos</code> is
		/// equal to zero.
		/// 
		/// @since   JDK1.1
		/// </summary>
		protected internal int Pos;

		/// <summary>
		/// Check to make sure that this stream has not been closed
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws IOException
		private void EnsureOpen()
		{
			if (@in == null)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Creates a <code>PushbackInputStream</code>
		/// with a pushback buffer of the specified <code>size</code>,
		/// and saves its  argument, the input stream
		/// <code>in</code>, for later use. Initially,
		/// there is no pushed-back byte  (the field
		/// <code>pushBack</code> is initialized to
		/// <code>-1</code>).
		/// </summary>
		/// <param name="in">    the input stream from which bytes will be read. </param>
		/// <param name="size">  the size of the pushback buffer. </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0}
		/// @since  JDK1.1 </exception>
		public PushbackInputStream(InputStream @in, int size) : base(@in)
		{
			if (size <= 0)
			{
				throw new IllegalArgumentException("size <= 0");
			}
			this.Buf = new sbyte[size];
			this.Pos = size;
		}

		/// <summary>
		/// Creates a <code>PushbackInputStream</code>
		/// and saves its  argument, the input stream
		/// <code>in</code>, for later use. Initially,
		/// there is no pushed-back byte  (the field
		/// <code>pushBack</code> is initialized to
		/// <code>-1</code>).
		/// </summary>
		/// <param name="in">   the input stream from which bytes will be read. </param>
		public PushbackInputStream(InputStream @in) : this(@in, 1)
		{
		}

		/// <summary>
		/// Reads the next byte of data from this input stream. The value
		/// byte is returned as an <code>int</code> in the range
		/// <code>0</code> to <code>255</code>. If no byte is available
		/// because the end of the stream has been reached, the value
		/// <code>-1</code> is returned. This method blocks until input data
		/// is available, the end of the stream is detected, or an exception
		/// is thrown.
		/// 
		/// <para> This method returns the most recently pushed-back byte, if there is
		/// one, and otherwise calls the <code>read</code> method of its underlying
		/// input stream and returns whatever value that method returns.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream has been reached. </returns>
		/// <exception cref="IOException">  if this input stream has been closed by
		///             invoking its <seealso cref="#close()"/> method,
		///             or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.InputStream#read() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			EnsureOpen();
			if (Pos < Buf.Length)
			{
				return Buf[Pos++] & 0xff;
			}
			return base.Read();
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this input stream into
		/// an array of bytes.  This method first reads any pushed-back bytes; after
		/// that, if fewer than <code>len</code> bytes have been read then it
		/// reads from the underlying input stream. If <code>len</code> is not zero, the method
		/// blocks until at least 1 byte of input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned.
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset in the destination array <code>b</code> </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the stream has been reached. </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="IOException">  if this input stream has been closed by
		///             invoking its <seealso cref="#close()"/> method,
		///             or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			EnsureOpen();
			if (b == null)
			{
				throw new NullPointerException();
			}
			else if (off < 0 || len < 0 || len > b.Length - off)
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return 0;
			}

			int avail = Buf.Length - Pos;
			if (avail > 0)
			{
				if (len < avail)
				{
					avail = len;
				}
				System.Array.Copy(Buf, Pos, b, off, avail);
				Pos += avail;
				off += avail;
				len -= avail;
			}
			if (len > 0)
			{
				len = base.Read(b, off, len);
				if (len == -1)
				{
					return avail == 0 ? - 1 : avail;
				}
				return avail + len;
			}
			return avail;
		}

		/// <summary>
		/// Pushes back a byte by copying it to the front of the pushback buffer.
		/// After this method returns, the next byte to be read will have the value
		/// <code>(byte)b</code>.
		/// </summary>
		/// <param name="b">   the <code>int</code> value whose low-order
		///                  byte is to be pushed back. </param>
		/// <exception cref="IOException"> If there is not enough room in the pushback
		///            buffer for the byte, or this input stream has been closed by
		///            invoking its <seealso cref="#close()"/> method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(int b) throws IOException
		public virtual void Unread(int b)
		{
			EnsureOpen();
			if (Pos == 0)
			{
				throw new IOException("Push back buffer is full");
			}
			Buf[--Pos] = (sbyte)b;
		}

		/// <summary>
		/// Pushes back a portion of an array of bytes by copying it to the front
		/// of the pushback buffer.  After this method returns, the next byte to be
		/// read will have the value <code>b[off]</code>, the byte after that will
		/// have the value <code>b[off+1]</code>, and so forth.
		/// </summary>
		/// <param name="b"> the byte array to push back. </param>
		/// <param name="off"> the start offset of the data. </param>
		/// <param name="len"> the number of bytes to push back. </param>
		/// <exception cref="IOException"> If there is not enough room in the pushback
		///            buffer for the specified number of bytes,
		///            or this input stream has been closed by
		///            invoking its <seealso cref="#close()"/> method.
		/// @since     JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(byte[] b, int off, int len) throws IOException
		public virtual void Unread(sbyte[] b, int off, int len)
		{
			EnsureOpen();
			if (len > Pos)
			{
				throw new IOException("Push back buffer is full");
			}
			Pos -= len;
			System.Array.Copy(b, off, Buf, Pos, len);
		}

		/// <summary>
		/// Pushes back an array of bytes by copying it to the front of the
		/// pushback buffer.  After this method returns, the next byte to be read
		/// will have the value <code>b[0]</code>, the byte after that will have the
		/// value <code>b[1]</code>, and so forth.
		/// </summary>
		/// <param name="b"> the byte array to push back </param>
		/// <exception cref="IOException"> If there is not enough room in the pushback
		///            buffer for the specified number of bytes,
		///            or this input stream has been closed by
		///            invoking its <seealso cref="#close()"/> method.
		/// @since     JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(byte[] b) throws IOException
		public virtual void Unread(sbyte[] b)
		{
			Unread(b, 0, b.Length);
		}

		/// <summary>
		/// Returns an estimate of the number of bytes that can be read (or
		/// skipped over) from this input stream without blocking by the next
		/// invocation of a method for this input stream. The next invocation might be
		/// the same thread or another thread.  A single read or skip of this
		/// many bytes will not block, but may read or skip fewer bytes.
		/// 
		/// <para> The method returns the sum of the number of bytes that have been
		/// pushed back and the value returned by {@link
		/// java.io.FilterInputStream#available available}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the number of bytes that can be read (or skipped over) from
		///             the input stream without blocking. </returns>
		/// <exception cref="IOException">  if this input stream has been closed by
		///             invoking its <seealso cref="#close()"/> method,
		///             or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.InputStream#available() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
		public override int Available()
		{
			EnsureOpen();
			int n = Buf.Length - Pos;
			int avail = base.Available();
			return n > (Integer.MaxValue - avail) ? Integer.MaxValue : n + avail;
		}

		/// <summary>
		/// Skips over and discards <code>n</code> bytes of data from this
		/// input stream. The <code>skip</code> method may, for a variety of
		/// reasons, end up skipping over some smaller number of bytes,
		/// possibly zero.  If <code>n</code> is negative, no bytes are skipped.
		/// 
		/// <para> The <code>skip</code> method of <code>PushbackInputStream</code>
		/// first skips over the bytes in the pushback buffer, if any.  It then
		/// calls the <code>skip</code> method of the underlying input stream if
		/// more bytes need to be skipped.  The actual number of bytes skipped
		/// is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">  {@inheritDoc} </param>
		/// <returns>     {@inheritDoc} </returns>
		/// <exception cref="IOException">  if the stream does not support seek,
		///            or the stream has been closed by
		///            invoking its <seealso cref="#close()"/> method,
		///            or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.InputStream#skip(long n)
		/// @since      1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			EnsureOpen();
			if (n <= 0)
			{
				return 0;
			}

			long pskip = Buf.Length - Pos;
			if (pskip > 0)
			{
				if (n < pskip)
				{
					pskip = n;
				}
				Pos += (int)pskip;
				n -= pskip;
			}
			if (n > 0)
			{
				pskip += base.Skip(n);
			}
			return pskip;
		}

		/// <summary>
		/// Tests if this input stream supports the <code>mark</code> and
		/// <code>reset</code> methods, which it does not.
		/// </summary>
		/// <returns>   <code>false</code>, since this class does not support the
		///           <code>mark</code> and <code>reset</code> methods. </returns>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override bool MarkSupported()
		{
			return false;
		}

		/// <summary>
		/// Marks the current position in this input stream.
		/// 
		/// <para> The <code>mark</code> method of <code>PushbackInputStream</code>
		/// does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="readlimit">   the maximum limit of bytes that can be read before
		///                      the mark position becomes invalid. </param>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override void Mark(int readlimit)
		{
			lock (this)
			{
			}
		}

		/// <summary>
		/// Repositions this stream to the position at the time the
		/// <code>mark</code> method was last called on this input stream.
		/// 
		/// <para> The method <code>reset</code> for class
		/// <code>PushbackInputStream</code> does nothing except throw an
		/// <code>IOException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if this method is invoked. </exception>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.IOException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void reset() throws IOException
		public override void Reset()
		{
			lock (this)
			{
				throw new IOException("mark/reset not supported");
			}
		}

		/// <summary>
		/// Closes this input stream and releases any system resources
		/// associated with the stream.
		/// Once the stream has been closed, further read(), unread(),
		/// available(), reset(), or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void close() throws IOException
		public override void Close()
		{
			lock (this)
			{
				if (@in == null)
				{
					return;
				}
				@in.Close();
				@in = null;
				Buf = null;
			}
		}
	}

}