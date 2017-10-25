using System;

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
	/// A character stream whose source is a string.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public class StringReader : Reader
	{

		private String Str;
		private int Length;
		private int Next = 0;
		private int Mark_Renamed = 0;

		/// <summary>
		/// Creates a new string reader.
		/// </summary>
		/// <param name="s">  String providing the character stream. </param>
		public StringReader(String s)
		{
			this.Str = s;
			this.Length = s.Length();
		}

		/// <summary>
		/// Check to make sure that the stream has not been closed </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws IOException
		private void EnsureOpen()
		{
			if (Str == null)
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
				if (Next >= Length)
				{
					return -1;
				}
				return Str.CharAt(Next++);
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
				if ((off < 0) || (off > cbuf.Length) || (len < 0) || ((off + len) > cbuf.Length) || ((off + len) < 0))
				{
					throw new IndexOutOfBoundsException();
				}
				else if (len == 0)
				{
					return 0;
				}
				if (Next >= Length)
				{
					return -1;
				}
				int n = System.Math.Min(Length - Next, len);
				Str.GetChars(Next, Next + n, cbuf, off);
				Next += n;
				return n;
			}
		}

		/// <summary>
		/// Skips the specified number of characters in the stream. Returns
		/// the number of characters that were skipped.
		/// 
		/// <para>The <code>ns</code> parameter may be negative, even though the
		/// <code>skip</code> method of the <seealso cref="Reader"/> superclass throws
		/// an exception in this case. Negative values of <code>ns</code> cause the
		/// stream to skip backwards. Negative return values indicate a skip
		/// backwards. It is not possible to skip backwards past the beginning of
		/// the string.
		/// 
		/// </para>
		/// <para>If the entire string has been read or skipped, then this method has
		/// no effect and always returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long ns) throws IOException
		public override long Skip(long ns)
		{
			lock (@lock)
			{
				EnsureOpen();
				if (Next >= Length)
				{
					return 0;
				}
				// Bound skip by beginning and end of the source
				long n = System.Math.Min(Length - Next, ns);
				n = System.Math.Max(-Next, n);
				Next += (int)n;
				return n;
			}
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.
		/// </summary>
		/// <returns> True if the next read() is guaranteed not to block for input
		/// </returns>
		/// <exception cref="IOException">  If the stream is closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			lock (@lock)
			{
			EnsureOpen();
			return true;
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
		///                         the stream's input comes from a string, there
		///                         is no actual limit, so this argument must not
		///                         be negative, but is otherwise ignored.
		/// </param>
		/// <exception cref="IllegalArgumentException">  If {@code readAheadLimit < 0} </exception>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public override void Mark(int readAheadLimit)
		{
			if (readAheadLimit < 0)
			{
				throw new IllegalArgumentException("Read-ahead limit < 0");
			}
			lock (@lock)
			{
				EnsureOpen();
				Mark_Renamed = Next;
			}
		}

		/// <summary>
		/// Resets the stream to the most recent mark, or to the beginning of the
		/// string if it has never been marked.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			lock (@lock)
			{
				EnsureOpen();
				Next = Mark_Renamed;
			}
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated with
		/// it. Once the stream has been closed, further read(),
		/// ready(), mark(), or reset() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
		public override void Close()
		{
			Str = null;
		}
	}

}