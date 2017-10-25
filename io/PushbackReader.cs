/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A character-stream reader that allows characters to be pushed back into the
	/// stream.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public class PushbackReader : FilterReader
	{

		/// <summary>
		/// Pushback buffer </summary>
		private char[] Buf;

		/// <summary>
		/// Current position in buffer </summary>
		private int Pos;

		/// <summary>
		/// Creates a new pushback reader with a pushback buffer of the given size.
		/// </summary>
		/// <param name="in">   The reader from which characters will be read </param>
		/// <param name="size"> The size of the pushback buffer </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		public PushbackReader(Reader @in, int size) : base(@in)
		{
			if (size <= 0)
			{
				throw new IllegalArgumentException("size <= 0");
			}
			this.Buf = new char[size];
			this.Pos = size;
		}

		/// <summary>
		/// Creates a new pushback reader with a one-character pushback buffer.
		/// </summary>
		/// <param name="in">  The reader from which characters will be read </param>
		public PushbackReader(Reader @in) : this(@in, 1)
		{
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed. </summary>
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
		/// <returns>     The character read, or -1 if the end of the stream has been
		///             reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			lock (@lock)
			{
				EnsureOpen();
				if (Pos < Buf.Length)
				{
					return Buf[Pos++];
				}
				else
				{
					return base.Read();
				}
			}
		}

		/// <summary>
		/// Reads characters into a portion of an array.
		/// </summary>
		/// <param name="cbuf">  Destination buffer </param>
		/// <param name="off">   Offset at which to start writing characters </param>
		/// <param name="len">   Maximum number of characters to read
		/// </param>
		/// <returns>     The number of characters read, or -1 if the end of the
		///             stream has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char cbuf[] , int off, int len) throws IOException
		public override int Read(char[] cbuf, int off, int len)
		{
			lock (@lock)
			{
				EnsureOpen();
				try
				{
					if (len <= 0)
					{
						if (len < 0)
						{
							throw new IndexOutOfBoundsException();
						}
						else if ((off < 0) || (off > cbuf.Length))
						{
							throw new IndexOutOfBoundsException();
						}
						return 0;
					}
					int avail = Buf.Length - Pos;
					if (avail > 0)
					{
						if (len < avail)
						{
							avail = len;
						}
						System.Array.Copy(Buf, Pos, cbuf, off, avail);
						Pos += avail;
						off += avail;
						len -= avail;
					}
					if (len > 0)
					{
						len = base.Read(cbuf, off, len);
						if (len == -1)
						{
							return (avail == 0) ? - 1 : avail;
						}
						return avail + len;
					}
					return avail;
				}
				catch (ArrayIndexOutOfBoundsException)
				{
					throw new IndexOutOfBoundsException();
				}
			}
		}

		/// <summary>
		/// Pushes back a single character by copying it to the front of the
		/// pushback buffer. After this method returns, the next character to be read
		/// will have the value <code>(char)c</code>.
		/// </summary>
		/// <param name="c">  The int value representing a character to be pushed back
		/// </param>
		/// <exception cref="IOException">  If the pushback buffer is full,
		///                          or if some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(int c) throws IOException
		public virtual void Unread(int c)
		{
			lock (@lock)
			{
				EnsureOpen();
				if (Pos == 0)
				{
					throw new IOException("Pushback buffer overflow");
				}
				Buf[--Pos] = (char) c;
			}
		}

		/// <summary>
		/// Pushes back a portion of an array of characters by copying it to the
		/// front of the pushback buffer.  After this method returns, the next
		/// character to be read will have the value <code>cbuf[off]</code>, the
		/// character after that will have the value <code>cbuf[off+1]</code>, and
		/// so forth.
		/// </summary>
		/// <param name="cbuf">  Character array </param>
		/// <param name="off">   Offset of first character to push back </param>
		/// <param name="len">   Number of characters to push back
		/// </param>
		/// <exception cref="IOException">  If there is insufficient room in the pushback
		///                          buffer, or if some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(char cbuf[] , int off, int len) throws IOException
		public virtual void Unread(char[] cbuf, int off, int len)
		{
			lock (@lock)
			{
				EnsureOpen();
				if (len > Pos)
				{
					throw new IOException("Pushback buffer overflow");
				}
				Pos -= len;
				System.Array.Copy(cbuf, off, Buf, Pos, len);
			}
		}

		/// <summary>
		/// Pushes back an array of characters by copying it to the front of the
		/// pushback buffer.  After this method returns, the next character to be
		/// read will have the value <code>cbuf[0]</code>, the character after that
		/// will have the value <code>cbuf[1]</code>, and so forth.
		/// </summary>
		/// <param name="cbuf">  Character array to push back
		/// </param>
		/// <exception cref="IOException">  If there is insufficient room in the pushback
		///                          buffer, or if some other I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unread(char cbuf[]) throws IOException
		public virtual void Unread(char[] cbuf)
		{
			Unread(cbuf, 0, cbuf.Length);
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			lock (@lock)
			{
				EnsureOpen();
				return (Pos < Buf.Length) || base.Ready();
			}
		}

		/// <summary>
		/// Marks the present position in the stream. The <code>mark</code>
		/// for class <code>PushbackReader</code> always throws an exception.
		/// </summary>
		/// <exception cref="IOException">  Always, since mark is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public override void Mark(int readAheadLimit)
		{
			throw new IOException("mark/reset not supported");
		}

		/// <summary>
		/// Resets the stream. The <code>reset</code> method of
		/// <code>PushbackReader</code> always throws an exception.
		/// </summary>
		/// <exception cref="IOException">  Always, since reset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			throw new IOException("mark/reset not supported");
		}

		/// <summary>
		/// Tells whether this stream supports the mark() operation, which it does
		/// not.
		/// </summary>
		public override bool MarkSupported()
		{
			return false;
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated with
		/// it. Once the stream has been closed, further read(),
		/// unread(), ready(), or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			base.Close();
			Buf = null;
		}

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
		public override long Skip(long n)
		{
			if (n < 0L)
			{
				throw new IllegalArgumentException("skip value is negative");
			}
			lock (@lock)
			{
				EnsureOpen();
				int avail = Buf.Length - Pos;
				if (avail > 0)
				{
					if (n <= avail)
					{
						Pos += (int)n;
						return n;
					}
					else
					{
						Pos = Buf.Length;
						n -= avail;
					}
				}
				return avail + base.Skip(n);
			}
		}
	}

}