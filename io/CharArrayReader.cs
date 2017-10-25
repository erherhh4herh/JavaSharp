using System;

/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements a character buffer that can be used as a
	/// character-input stream.
	/// 
	/// @author      Herb Jellinek
	/// @since       JDK1.1
	/// </summary>
	public class CharArrayReader : Reader
	{
		/// <summary>
		/// The character buffer. </summary>
		protected internal char[] Buf;

		/// <summary>
		/// The current buffer position. </summary>
		protected internal int Pos;

		/// <summary>
		/// The position of mark in buffer. </summary>
		protected internal int MarkedPos = 0;

		/// <summary>
		///  The index of the end of this buffer.  There is not valid
		///  data at or beyond this index.
		/// </summary>
		protected internal int Count;

		/// <summary>
		/// Creates a CharArrayReader from the specified array of chars. </summary>
		/// <param name="buf">       Input buffer (not copied) </param>
		public CharArrayReader(char[] buf)
		{
			this.Buf = buf;
			this.Pos = 0;
			this.Count = buf.Length;
		}

		/// <summary>
		/// Creates a CharArrayReader from the specified array of chars.
		/// 
		/// <para> The resulting reader will start reading at the given
		/// <tt>offset</tt>.  The total number of <tt>char</tt> values that can be
		/// read from this reader will be either <tt>length</tt> or
		/// <tt>buf.length-offset</tt>, whichever is smaller.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalArgumentException">
		///         If <tt>offset</tt> is negative or greater than
		///         <tt>buf.length</tt>, or if <tt>length</tt> is negative, or if
		///         the sum of these two values is negative.
		/// </exception>
		/// <param name="buf">       Input buffer (not copied) </param>
		/// <param name="offset">    Offset of the first char to read </param>
		/// <param name="length">    Number of chars to read </param>
		public CharArrayReader(char[] buf, int offset, int length)
		{
			if ((offset < 0) || (offset > buf.Length) || (length < 0) || ((offset + length) < 0))
			{
				throw new IllegalArgumentException();
			}
			this.Buf = buf;
			this.Pos = offset;
			this.Count = System.Math.Min(offset + length, buf.Length);
			this.MarkedPos = offset;
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws IOException
		private void EnsureOpen()
		{
			if (Buf == null)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Reads a single character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			lock (@lock)
			{
				EnsureOpen();
				if (Pos >= Count)
				{
					return -1;
				}
				else
				{
					return Buf[Pos++];
				}
			}
		}

		/// <summary>
		/// Reads characters into a portion of an array. </summary>
		/// <param name="b">  Destination buffer </param>
		/// <param name="off">  Offset at which to start storing characters </param>
		/// <param name="len">   Maximum number of characters to read </param>
		/// <returns>  The actual number of characters read, or -1 if
		///          the end of the stream has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char b[] , int off, int len) throws IOException
		public override int Read(char[] b, int off, int len)
		{
			lock (@lock)
			{
				EnsureOpen();
				if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
				{
					throw new IndexOutOfBoundsException();
				}
				else if (len == 0)
				{
					return 0;
				}

				if (Pos >= Count)
				{
					return -1;
				}
				if (Pos + len > Count)
				{
					len = Count - Pos;
				}
				if (len <= 0)
				{
					return 0;
				}
				System.Array.Copy(Buf, Pos, b, off, len);
				Pos += len;
				return len;
			}
		}

		/// <summary>
		/// Skips characters.  Returns the number of characters that were skipped.
		/// 
		/// <para>The <code>n</code> parameter may be negative, even though the
		/// <code>skip</code> method of the <seealso cref="Reader"/> superclass throws
		/// an exception in this case. If <code>n</code> is negative, then
		/// this method does nothing and returns <code>0</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n"> The number of characters to skip </param>
		/// <returns>       The number of characters actually skipped </returns>
		/// <exception cref="IOException"> If the stream is closed, or an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			lock (@lock)
			{
				EnsureOpen();
				if (Pos + n > Count)
				{
					n = Count - Pos;
				}
				if (n < 0)
				{
					return 0;
				}
				Pos += (int)n;
				return n;
			}
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.  Character-array readers
		/// are always ready to be read.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			lock (@lock)
			{
				EnsureOpen();
				return (Count - Pos) > 0;
			}
		}

		/// <summary>
		/// Tells whether this stream supports the mark() operation, which it does.
		/// </summary>
		public override bool MarkSupported()
		{
			return true;
		}

		/// <summary>
		/// Marks the present position in the stream.  Subsequent calls to reset()
		/// will reposition the stream to this point.
		/// </summary>
		/// <param name="readAheadLimit">  Limit on the number of characters that may be
		///                         read while still preserving the mark.  Because
		///                         the stream's input comes from a character array,
		///                         there is no actual limit; hence this argument is
		///                         ignored.
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public override void Mark(int readAheadLimit)
		{
			lock (@lock)
			{
				EnsureOpen();
				MarkedPos = Pos;
			}
		}

		/// <summary>
		/// Resets the stream to the most recent mark, or to the beginning if it has
		/// never been marked.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			lock (@lock)
			{
				EnsureOpen();
				Pos = MarkedPos;
			}
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated with
		/// it.  Once the stream has been closed, further read(), ready(),
		/// mark(), reset(), or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		public override void Close()
		{
			Buf = null;
		}
	}

}