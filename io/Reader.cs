using System;

/*
 * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Abstract class for reading character streams.  The only methods that a
	/// subclass must implement are read(char[], int, int) and close().  Most
	/// subclasses, however, will override some of the methods defined here in order
	/// to provide higher efficiency, additional functionality, or both.
	/// 
	/// </summary>
	/// <seealso cref= BufferedReader </seealso>
	/// <seealso cref=   LineNumberReader </seealso>
	/// <seealso cref= CharArrayReader </seealso>
	/// <seealso cref= InputStreamReader </seealso>
	/// <seealso cref=   FileReader </seealso>
	/// <seealso cref= FilterReader </seealso>
	/// <seealso cref=   PushbackReader </seealso>
	/// <seealso cref= PipedReader </seealso>
	/// <seealso cref= StringReader </seealso>
	/// <seealso cref= Writer
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public abstract class Reader : Readable, Closeable
	{

		/// <summary>
		/// The object used to synchronize operations on this stream.  For
		/// efficiency, a character-stream object may use an object other than
		/// itself to protect critical sections.  A subclass should therefore use
		/// the object in this field rather than <tt>this</tt> or a synchronized
		/// method.
		/// </summary>
		protected internal Object @lock;

		/// <summary>
		/// Creates a new character-stream reader whose critical sections will
		/// synchronize on the reader itself.
		/// </summary>
		protected internal Reader()
		{
			this.@lock = this;
		}

		/// <summary>
		/// Creates a new character-stream reader whose critical sections will
		/// synchronize on the given object.
		/// </summary>
		/// <param name="lock">  The Object to synchronize on. </param>
		protected internal Reader(Object @lock)
		{
			if (@lock == null)
			{
				throw new NullPointerException();
			}
			this.@lock = @lock;
		}

		/// <summary>
		/// Attempts to read characters into the specified character buffer.
		/// The buffer is used as a repository of characters as-is: the only
		/// changes made are the results of a put operation. No flipping or
		/// rewinding of the buffer is performed.
		/// </summary>
		/// <param name="target"> the buffer to read characters into </param>
		/// <returns> The number of characters added to the buffer, or
		///         -1 if this source of characters is at its end </returns>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
		/// <exception cref="NullPointerException"> if target is null </exception>
		/// <exception cref="java.nio.ReadOnlyBufferException"> if target is a read only buffer
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(java.nio.CharBuffer target) throws IOException
		public virtual int Read(java.nio.CharBuffer target)
		{
			int len = target.Remaining();
			char[] cbuf = new char[len];
			int n = Read(cbuf, 0, len);
			if (n > 0)
			{
				target.Put(cbuf, 0, n);
			}
			return n;
		}

		/// <summary>
		/// Reads a single character.  This method will block until a character is
		/// available, an I/O error occurs, or the end of the stream is reached.
		/// 
		/// <para> Subclasses that intend to support efficient single-character input
		/// should override this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     The character read, as an integer in the range 0 to 65535
		///             (<tt>0x00-0xffff</tt>), or -1 if the end of the stream has
		///             been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public virtual int Read()
		{
			char[] cb = new char[1];
			if (Read(cb, 0, 1) == -1)
			{
				return -1;
			}
			else
			{
				return cb[0];
			}
		}

		/// <summary>
		/// Reads characters into an array.  This method will block until some input
		/// is available, an I/O error occurs, or the end of the stream is reached.
		/// </summary>
		/// <param name="cbuf">  Destination buffer
		/// </param>
		/// <returns>      The number of characters read, or -1
		///              if the end of the stream
		///              has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char cbuf[]) throws IOException
		public virtual int Read(char[] cbuf)
		{
			return Read(cbuf, 0, cbuf.Length);
		}

		/// <summary>
		/// Reads characters into a portion of an array.  This method will block
		/// until some input is available, an I/O error occurs, or the end of the
		/// stream is reached.
		/// </summary>
		/// <param name="cbuf">  Destination buffer </param>
		/// <param name="off">   Offset at which to start storing characters </param>
		/// <param name="len">   Maximum number of characters to read
		/// </param>
		/// <returns>     The number of characters read, or -1 if the end of the
		///             stream has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int read(char cbuf[] , int off, int len) throws IOException;
		public abstract int Read(char[] cbuf, int off, int len);

		/// <summary>
		/// Maximum skip-buffer size </summary>
		private const int MaxSkipBufferSize = 8192;

		/// <summary>
		/// Skip buffer, null until allocated </summary>
		private char[] SkipBuffer = null;

		/// <summary>
		/// Skips characters.  This method will block until some characters are
		/// available, an I/O error occurs, or the end of the stream is reached.
		/// </summary>
		/// <param name="n">  The number of characters to skip
		/// </param>
		/// <returns>    The number of characters actually skipped
		/// </returns>
		/// <exception cref="IllegalArgumentException">  If <code>n</code> is negative. </exception>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public virtual long Skip(long n)
		{
			if (n < 0L)
			{
				throw new IllegalArgumentException("skip value is negative");
			}
			int nn = (int) System.Math.Min(n, MaxSkipBufferSize);
			lock (@lock)
			{
				if ((SkipBuffer == null) || (SkipBuffer.Length < nn))
				{
					SkipBuffer = new char[nn];
				}
				long r = n;
				while (r > 0)
				{
					int nc = Read(SkipBuffer, 0, (int)System.Math.Min(r, nn));
					if (nc == -1)
					{
						break;
					}
					r -= nc;
				}
				return n - r;
			}
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.
		/// </summary>
		/// <returns> True if the next read() is guaranteed not to block for input,
		/// false otherwise.  Note that returning false does not guarantee that the
		/// next read will block.
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public virtual bool Ready()
		{
			return false;
		}

		/// <summary>
		/// Tells whether this stream supports the mark() operation. The default
		/// implementation always returns false. Subclasses should override this
		/// method.
		/// </summary>
		/// <returns> true if and only if this stream supports the mark operation. </returns>
		public virtual bool MarkSupported()
		{
			return false;
		}

		/// <summary>
		/// Marks the present position in the stream.  Subsequent calls to reset()
		/// will attempt to reposition the stream to this point.  Not all
		/// character-input streams support the mark() operation.
		/// </summary>
		/// <param name="readAheadLimit">  Limit on the number of characters that may be
		///                         read while still preserving the mark.  After
		///                         reading this many characters, attempting to
		///                         reset the stream may fail.
		/// </param>
		/// <exception cref="IOException">  If the stream does not support mark(),
		///                          or if some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public virtual void Mark(int readAheadLimit)
		{
			throw new IOException("mark() not supported");
		}

		/// <summary>
		/// Resets the stream.  If the stream has been marked, then attempt to
		/// reposition it at the mark.  If the stream has not been marked, then
		/// attempt to reset it in some way appropriate to the particular stream,
		/// for example by repositioning it to its starting point.  Not all
		/// character-input streams support the reset() operation, and some support
		/// reset() without supporting mark().
		/// </summary>
		/// <exception cref="IOException">  If the stream has not been marked,
		///                          or if the mark has been invalidated,
		///                          or if the stream does not support reset(),
		///                          or if some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public virtual void Reset()
		{
			throw new IOException("reset() not supported");
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated with
		/// it.  Once the stream has been closed, further read(), ready(),
		/// mark(), reset(), or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void close() throws IOException;
		 public abstract void Close();

	}

}