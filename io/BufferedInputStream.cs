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
	/// A <code>BufferedInputStream</code> adds
	/// functionality to another input stream-namely,
	/// the ability to buffer the input and to
	/// support the <code>mark</code> and <code>reset</code>
	/// methods. When  the <code>BufferedInputStream</code>
	/// is created, an internal buffer array is
	/// created. As bytes  from the stream are read
	/// or skipped, the internal buffer is refilled
	/// as necessary  from the contained input stream,
	/// many bytes at a time. The <code>mark</code>
	/// operation  remembers a point in the input
	/// stream and the <code>reset</code> operation
	/// causes all the  bytes read since the most
	/// recent <code>mark</code> operation to be
	/// reread before new bytes are  taken from
	/// the contained input stream.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </summary>
	public class BufferedInputStream : FilterInputStream
	{

		private static int DEFAULT_BUFFER_SIZE = 8192;

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static int MAX_BUFFER_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// The internal buffer array where the data is stored. When necessary,
		/// it may be replaced by another array of
		/// a different size.
		/// </summary>
		protected internal volatile sbyte[] Buf;

		/// <summary>
		/// Atomic updater to provide compareAndSet for buf. This is
		/// necessary because closes can be asynchronous. We use nullness
		/// of buf[] as primary indicator that this stream is closed. (The
		/// "in" field is also nulled out on close.)
		/// </summary>
		private static readonly AtomicReferenceFieldUpdater<BufferedInputStream, sbyte[]> BufUpdater = AtomicReferenceFieldUpdater.NewUpdater(typeof(BufferedInputStream), typeof(sbyte[]), "buf");

		/// <summary>
		/// The index one greater than the index of the last valid byte in
		/// the buffer.
		/// This value is always
		/// in the range <code>0</code> through <code>buf.length</code>;
		/// elements <code>buf[0]</code>  through <code>buf[count-1]
		/// </code>contain buffered input data obtained
		/// from the underlying  input stream.
		/// </summary>
		protected internal int Count;

		/// <summary>
		/// The current position in the buffer. This is the index of the next
		/// character to be read from the <code>buf</code> array.
		/// <para>
		/// This value is always in the range <code>0</code>
		/// through <code>count</code>. If it is less
		/// than <code>count</code>, then  <code>buf[pos]</code>
		/// is the next byte to be supplied as input;
		/// if it is equal to <code>count</code>, then
		/// the  next <code>read</code> or <code>skip</code>
		/// operation will require more bytes to be
		/// read from the contained  input stream.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.BufferedInputStream#buf </seealso>
		protected internal int Pos;

		/// <summary>
		/// The value of the <code>pos</code> field at the time the last
		/// <code>mark</code> method was called.
		/// <para>
		/// This value is always
		/// in the range <code>-1</code> through <code>pos</code>.
		/// If there is no marked position in  the input
		/// stream, this field is <code>-1</code>. If
		/// there is a marked position in the input
		/// stream,  then <code>buf[markpos]</code>
		/// is the first byte to be supplied as input
		/// after a <code>reset</code> operation. If
		/// <code>markpos</code> is not <code>-1</code>,
		/// then all bytes from positions <code>buf[markpos]</code>
		/// through  <code>buf[pos-1]</code> must remain
		/// in the buffer array (though they may be
		/// moved to  another place in the buffer array,
		/// with suitable adjustments to the values
		/// of <code>count</code>,  <code>pos</code>,
		/// and <code>markpos</code>); they may not
		/// be discarded unless and until the difference
		/// between <code>pos</code> and <code>markpos</code>
		/// exceeds <code>marklimit</code>.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.io.BufferedInputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.BufferedInputStream#pos </seealso>
		protected internal int Markpos = -1;

		/// <summary>
		/// The maximum read ahead allowed after a call to the
		/// <code>mark</code> method before subsequent calls to the
		/// <code>reset</code> method fail.
		/// Whenever the difference between <code>pos</code>
		/// and <code>markpos</code> exceeds <code>marklimit</code>,
		/// then the  mark may be dropped by setting
		/// <code>markpos</code> to <code>-1</code>.
		/// </summary>
		/// <seealso cref=     java.io.BufferedInputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.BufferedInputStream#reset() </seealso>
		protected internal int Marklimit;

		/// <summary>
		/// Check to make sure that underlying input stream has not been
		/// nulled out due to close; if not return it;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private InputStream getInIfOpen() throws IOException
		private InputStream InIfOpen
		{
			get
			{
				InputStream input = @in;
				if (input == null)
				{
					throw new IOException("Stream closed");
				}
				return input;
			}
		}

		/// <summary>
		/// Check to make sure that buffer has not been nulled out due to
		/// close; if not return it;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] getBufIfOpen() throws IOException
		private sbyte[] BufIfOpen
		{
			get
			{
				sbyte[] buffer = Buf;
				if (buffer == null)
				{
					throw new IOException("Stream closed");
				}
				return buffer;
			}
		}

		/// <summary>
		/// Creates a <code>BufferedInputStream</code>
		/// and saves its  argument, the input stream
		/// <code>in</code>, for later use. An internal
		/// buffer array is created and  stored in <code>buf</code>.
		/// </summary>
		/// <param name="in">   the underlying input stream. </param>
		public BufferedInputStream(InputStream @in) : this(@in, DEFAULT_BUFFER_SIZE)
		{
		}

		/// <summary>
		/// Creates a <code>BufferedInputStream</code>
		/// with the specified buffer size,
		/// and saves its  argument, the input stream
		/// <code>in</code>, for later use.  An internal
		/// buffer array of length  <code>size</code>
		/// is created and stored in <code>buf</code>.
		/// </summary>
		/// <param name="in">     the underlying input stream. </param>
		/// <param name="size">   the buffer size. </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0}. </exception>
		public BufferedInputStream(InputStream @in, int size) : base(@in)
		{
			if (size <= 0)
			{
				throw new IllegalArgumentException("Buffer size <= 0");
			}
			Buf = new sbyte[size];
		}

		/// <summary>
		/// Fills the buffer with more data, taking into account
		/// shuffling and other tricks for dealing with marks.
		/// Assumes that it is being called by a synchronized method.
		/// This method also assumes that all data has already been read in,
		/// hence pos > count.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fill() throws IOException
		private void Fill()
		{
			sbyte[] buffer = BufIfOpen;
			if (Markpos < 0)
			{
				Pos = 0; // no mark: throw away the buffer
			}
			else if (Pos >= buffer.Length) // no room left in buffer
			{
				if (Markpos > 0) // can throw away early part of the buffer
				{
					int sz = Pos - Markpos;
					System.Array.Copy(buffer, Markpos, buffer, 0, sz);
					Pos = sz;
					Markpos = 0;
				}
				else if (buffer.Length >= Marklimit)
				{
					Markpos = -1; // buffer got too big, invalidate mark
					Pos = 0; // drop buffer contents
				}
				else if (buffer.Length >= MAX_BUFFER_SIZE)
				{
					throw new OutOfMemoryError("Required array size too large");
				} // grow buffer
				else
				{
					int nsz = (Pos <= MAX_BUFFER_SIZE - Pos) ? Pos * 2 : MAX_BUFFER_SIZE;
					if (nsz > Marklimit)
					{
						nsz = Marklimit;
					}
					sbyte[] nbuf = new sbyte[nsz];
					System.Array.Copy(buffer, 0, nbuf, 0, Pos);
					if (!BufUpdater.CompareAndSet(this, buffer, nbuf))
					{
						// Can't replace buf if there was an async close.
						// Note: This would need to be changed if fill()
						// is ever made accessible to multiple threads.
						// But for now, the only way CAS can fail is via close.
						// assert buf == null;
						throw new IOException("Stream closed");
					}
					buffer = nbuf;
				}
			}
			Count = Pos;
			int n = InIfOpen.Read(buffer, Pos, buffer.Length - Pos);
			if (n > 0)
			{
				Count = n + Pos;
			}
		}

		/// <summary>
		/// See
		/// the general contract of the <code>read</code>
		/// method of <code>InputStream</code>.
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if this input stream has been closed by
		///                          invoking its <seealso cref="#close()"/> method,
		///                          or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read() throws IOException
		public override int Read()
		{
			lock (this)
			{
				if (Pos >= Count)
				{
					Fill();
					if (Pos >= Count)
					{
						return -1;
					}
				}
				return BufIfOpen[Pos++] & 0xff;
			}
		}

		/// <summary>
		/// Read characters into a portion of an array, reading from the underlying
		/// stream at most once if necessary.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int read1(byte[] b, int off, int len) throws IOException
		private int Read1(sbyte[] b, int off, int len)
		{
			int avail = Count - Pos;
			if (avail <= 0)
			{
				/* If the requested length is at least as large as the buffer, and
				   if there is no mark/reset activity, do not bother to copy the
				   bytes into the local buffer.  In this way buffered streams will
				   cascade harmlessly. */
				if (len >= BufIfOpen.Length && Markpos < 0)
				{
					return InIfOpen.Read(b, off, len);
				}
				Fill();
				avail = Count - Pos;
				if (avail <= 0)
				{
					return -1;
				}
			}
			int cnt = (avail < len) ? avail : len;
			System.Array.Copy(BufIfOpen, Pos, b, off, cnt);
			Pos += cnt;
			return cnt;
		}

		/// <summary>
		/// Reads bytes from this byte-input stream into the specified byte array,
		/// starting at the given offset.
		/// 
		/// <para> This method implements the general contract of the corresponding
		/// <code><seealso cref="InputStream#read(byte[], int, int) read"/></code> method of
		/// the <code><seealso cref="InputStream"/></code> class.  As an additional
		/// convenience, it attempts to read as many bytes as possible by repeatedly
		/// invoking the <code>read</code> method of the underlying stream.  This
		/// iterated <code>read</code> continues until one of the following
		/// conditions becomes true: <ul>
		/// 
		///   <li> The specified number of bytes have been read,
		/// 
		///   <li> The <code>read</code> method of the underlying stream returns
		///   <code>-1</code>, indicating end-of-file, or
		/// 
		///   <li> The <code>available</code> method of the underlying stream
		///   returns zero, indicating that further input requests would block.
		/// 
		/// </ul> If the first <code>read</code> on the underlying stream returns
		/// <code>-1</code> to indicate end-of-file then this method returns
		/// <code>-1</code>.  Otherwise this method returns the number of bytes
		/// actually read.
		/// 
		/// </para>
		/// <para> Subclasses of this class are encouraged, but not required, to
		/// attempt to read as many bytes as possible in the same fashion.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     destination buffer. </param>
		/// <param name="off">   offset at which to start storing bytes. </param>
		/// <param name="len">   maximum number of bytes to read. </param>
		/// <returns>     the number of bytes read, or <code>-1</code> if the end of
		///             the stream has been reached. </returns>
		/// <exception cref="IOException">  if this input stream has been closed by
		///                          invoking its <seealso cref="#close()"/> method,
		///                          or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				BufIfOpen; // Check for closed stream
				if ((off | len | (off + len) | (b.Length - (off + len))) < 0)
				{
					throw new IndexOutOfBoundsException();
				}
				else if (len == 0)
				{
					return 0;
				}
        
				int n = 0;
				for (;;)
				{
					int nread = Read1(b, off + n, len - n);
					if (nread <= 0)
					{
						return (n == 0) ? nread : n;
					}
					n += nread;
					if (n >= len)
					{
						return n;
					}
					// if not closed but no bytes available, return
					InputStream input = @in;
					if (input != null && input.Available() <= 0)
					{
						return n;
					}
				}
			}
		}

		/// <summary>
		/// See the general contract of the <code>skip</code>
		/// method of <code>InputStream</code>.
		/// </summary>
		/// <exception cref="IOException">  if the stream does not support seek,
		///                          or if this input stream has been closed by
		///                          invoking its <seealso cref="#close()"/> method, or an
		///                          I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized long skip(long n) throws IOException
		public override long Skip(long n)
		{
			lock (this)
			{
				BufIfOpen; // Check for closed stream
				if (n <= 0)
				{
					return 0;
				}
				long avail = Count - Pos;
        
				if (avail <= 0)
				{
					// If no mark position set then don't keep in buffer
					if (Markpos < 0)
					{
						return InIfOpen.Skip(n);
					}
        
					// Fill in buffer to save bytes for reset
					Fill();
					avail = Count - Pos;
					if (avail <= 0)
					{
						return 0;
					}
				}
        
				long skipped = (avail < n) ? avail : n;
				Pos += (int)skipped;
				return skipped;
			}
		}

		/// <summary>
		/// Returns an estimate of the number of bytes that can be read (or
		/// skipped over) from this input stream without blocking by the next
		/// invocation of a method for this input stream. The next invocation might be
		/// the same thread or another thread.  A single read or skip of this
		/// many bytes will not block, but may read or skip fewer bytes.
		/// <para>
		/// This method returns the sum of the number of bytes remaining to be read in
		/// the buffer (<code>count&nbsp;- pos</code>) and the result of calling the
		/// <seealso cref="java.io.FilterInputStream#in in"/>.available().
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an estimate of the number of bytes that can be read (or skipped
		///             over) from this input stream without blocking. </returns>
		/// <exception cref="IOException">  if this input stream has been closed by
		///                          invoking its <seealso cref="#close()"/> method,
		///                          or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int available() throws IOException
		public override int Available()
		{
			lock (this)
			{
				int n = Count - Pos;
				int avail = InIfOpen.Available();
				return n > (Integer.MaxValue - avail) ? Integer.MaxValue : n + avail;
			}
		}

		/// <summary>
		/// See the general contract of the <code>mark</code>
		/// method of <code>InputStream</code>.
		/// </summary>
		/// <param name="readlimit">   the maximum limit of bytes that can be read before
		///                      the mark position becomes invalid. </param>
		/// <seealso cref=     java.io.BufferedInputStream#reset() </seealso>
		public override void Mark(int readlimit)
		{
			lock (this)
			{
				Marklimit = readlimit;
				Markpos = Pos;
			}
		}

		/// <summary>
		/// See the general contract of the <code>reset</code>
		/// method of <code>InputStream</code>.
		/// <para>
		/// If <code>markpos</code> is <code>-1</code>
		/// (no mark has been set or the mark has been
		/// invalidated), an <code>IOException</code>
		/// is thrown. Otherwise, <code>pos</code> is
		/// set equal to <code>markpos</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if this stream has not been marked or,
		///                  if the mark has been invalidated, or the stream
		///                  has been closed by invoking its <seealso cref="#close()"/>
		///                  method, or an I/O error occurs. </exception>
		/// <seealso cref=        java.io.BufferedInputStream#mark(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void reset() throws IOException
		public override void Reset()
		{
			lock (this)
			{
				BufIfOpen; // Cause exception if closed
				if (Markpos < 0)
				{
					throw new IOException("Resetting to invalid mark");
				}
				Pos = Markpos;
			}
		}

		/// <summary>
		/// Tests if this input stream supports the <code>mark</code>
		/// and <code>reset</code> methods. The <code>markSupported</code>
		/// method of <code>BufferedInputStream</code> returns
		/// <code>true</code>.
		/// </summary>
		/// <returns>  a <code>boolean</code> indicating if this stream type supports
		///          the <code>mark</code> and <code>reset</code> methods. </returns>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override bool MarkSupported()
		{
			return true;
		}

		/// <summary>
		/// Closes this input stream and releases any system resources
		/// associated with the stream.
		/// Once the stream has been closed, further read(), available(), reset(),
		/// or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			sbyte[] buffer;
			while ((buffer = Buf) != null)
			{
				if (BufUpdater.CompareAndSet(this, buffer, null))
				{
					InputStream input = @in;
					@in = null;
					if (input != null)
					{
						input.Close();
					}
					return;
				}
				// Else retry in case a new buf was CASed in fill()
			}
		}
	}

}