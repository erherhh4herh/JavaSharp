using System;
using System.Collections.Generic;

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
	/// Reads text from a character-input stream, buffering characters so as to
	/// provide for the efficient reading of characters, arrays, and lines.
	/// 
	/// <para> The buffer size may be specified, or the default size may be used.  The
	/// default is large enough for most purposes.
	/// 
	/// </para>
	/// <para> In general, each read request made of a Reader causes a corresponding
	/// read request to be made of the underlying character or byte stream.  It is
	/// therefore advisable to wrap a BufferedReader around any Reader whose read()
	/// operations may be costly, such as FileReaders and InputStreamReaders.  For
	/// example,
	/// 
	/// <pre>
	/// BufferedReader in
	///   = new BufferedReader(new FileReader("foo.in"));
	/// </pre>
	/// 
	/// will buffer the input from the specified file.  Without buffering, each
	/// invocation of read() or readLine() could cause bytes to be read from the
	/// file, converted into characters, and then returned, which can be very
	/// inefficient.
	/// 
	/// </para>
	/// <para> Programs that use DataInputStreams for textual input can be localized by
	/// replacing each DataInputStream with an appropriate BufferedReader.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= FileReader </seealso>
	/// <seealso cref= InputStreamReader </seealso>
	/// <seealso cref= java.nio.file.Files#newBufferedReader
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public class BufferedReader : Reader
	{

		private Reader @in;

		private char[] Cb;
		private int NChars, NextChar;

		private const int INVALIDATED = -2;
		private const int UNMARKED = -1;
		private int MarkedChar = UNMARKED;
		private int ReadAheadLimit = 0; // Valid only when markedChar > 0

		/// <summary>
		/// If the next character is a line feed, skip it </summary>
		private bool SkipLF = false;

		/// <summary>
		/// The skipLF flag when the mark was set </summary>
		private bool MarkedSkipLF = false;

		private static int DefaultCharBufferSize = 8192;
		private static int DefaultExpectedLineLength = 80;

		/// <summary>
		/// Creates a buffering character-input stream that uses an input buffer of
		/// the specified size.
		/// </summary>
		/// <param name="in">   A Reader </param>
		/// <param name="sz">   Input-buffer size
		/// </param>
		/// <exception cref="IllegalArgumentException">  If {@code sz <= 0} </exception>
		public BufferedReader(Reader @in, int sz) : base(@in)
		{
			if (sz <= 0)
			{
				throw new IllegalArgumentException("Buffer size <= 0");
			}
			this.@in = @in;
			Cb = new char[sz];
			NextChar = NChars = 0;
		}

		/// <summary>
		/// Creates a buffering character-input stream that uses a default-sized
		/// input buffer.
		/// </summary>
		/// <param name="in">   A Reader </param>
		public BufferedReader(Reader @in) : this(@in, DefaultCharBufferSize)
		{
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed </summary>
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
		/// Fills the input buffer, taking the mark into account if it is valid.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fill() throws IOException
		private void Fill()
		{
			int dst;
			if (MarkedChar <= UNMARKED)
			{
				/* No mark */
				dst = 0;
			}
			else
			{
				/* Marked */
				int delta = NextChar - MarkedChar;
				if (delta >= ReadAheadLimit)
				{
					/* Gone past read-ahead limit: Invalidate mark */
					MarkedChar = INVALIDATED;
					ReadAheadLimit = 0;
					dst = 0;
				}
				else
				{
					if (ReadAheadLimit <= Cb.Length)
					{
						/* Shuffle in the current buffer */
						System.Array.Copy(Cb, MarkedChar, Cb, 0, delta);
						MarkedChar = 0;
						dst = delta;
					}
					else
					{
						/* Reallocate buffer to accommodate read-ahead limit */
						char[] ncb = new char[ReadAheadLimit];
						System.Array.Copy(Cb, MarkedChar, ncb, 0, delta);
						Cb = ncb;
						MarkedChar = 0;
						dst = delta;
					}
					NextChar = NChars = delta;
				}
			}

			int n;
			do
			{
				n = @in.Read(Cb, dst, Cb.Length - dst);
			} while (n == 0);
			if (n > 0)
			{
				NChars = dst + n;
				NextChar = dst;
			}
		}

		/// <summary>
		/// Reads a single character.
		/// </summary>
		/// <returns> The character read, as an integer in the range
		///         0 to 65535 (<tt>0x00-0xffff</tt>), or -1 if the
		///         end of the stream has been reached </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			lock (@lock)
			{
				EnsureOpen();
				for (;;)
				{
					if (NextChar >= NChars)
					{
						Fill();
						if (NextChar >= NChars)
						{
							return -1;
						}
					}
					if (SkipLF)
					{
						SkipLF = false;
						if (Cb[NextChar] == '\n')
						{
							NextChar++;
							continue;
						}
					}
					return Cb[NextChar++];
				}
			}
		}

		/// <summary>
		/// Reads characters into a portion of an array, reading from the underlying
		/// stream if necessary.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int read1(char[] cbuf, int off, int len) throws IOException
		private int Read1(char[] cbuf, int off, int len)
		{
			if (NextChar >= NChars)
			{
				/* If the requested length is at least as large as the buffer, and
				   if there is no mark/reset activity, and if line feeds are not
				   being skipped, do not bother to copy the characters into the
				   local buffer.  In this way buffered streams will cascade
				   harmlessly. */
				if (len >= Cb.Length && MarkedChar <= UNMARKED && !SkipLF)
				{
					return @in.Read(cbuf, off, len);
				}
				Fill();
			}
			if (NextChar >= NChars)
			{
				return -1;
			}
			if (SkipLF)
			{
				SkipLF = false;
				if (Cb[NextChar] == '\n')
				{
					NextChar++;
					if (NextChar >= NChars)
					{
						Fill();
					}
					if (NextChar >= NChars)
					{
						return -1;
					}
				}
			}
			int n = System.Math.Min(len, NChars - NextChar);
			System.Array.Copy(Cb, NextChar, cbuf, off, n);
			NextChar += n;
			return n;
		}

		/// <summary>
		/// Reads characters into a portion of an array.
		/// 
		/// <para> This method implements the general contract of the corresponding
		/// <code><seealso cref="Reader#read(char[], int, int) read"/></code> method of the
		/// <code><seealso cref="Reader"/></code> class.  As an additional convenience, it
		/// attempts to read as many characters as possible by repeatedly invoking
		/// the <code>read</code> method of the underlying stream.  This iterated
		/// <code>read</code> continues until one of the following conditions becomes
		/// true: <ul>
		/// 
		///   <li> The specified number of characters have been read,
		/// 
		///   <li> The <code>read</code> method of the underlying stream returns
		///   <code>-1</code>, indicating end-of-file, or
		/// 
		///   <li> The <code>ready</code> method of the underlying stream
		///   returns <code>false</code>, indicating that further input requests
		///   would block.
		/// 
		/// </ul> If the first <code>read</code> on the underlying stream returns
		/// <code>-1</code> to indicate end-of-file then this method returns
		/// <code>-1</code>.  Otherwise this method returns the number of characters
		/// actually read.
		/// 
		/// </para>
		/// <para> Subclasses of this class are encouraged, but not required, to
		/// attempt to read as many characters as possible in the same fashion.
		/// 
		/// </para>
		/// <para> Ordinarily this method takes characters from this stream's character
		/// buffer, filling it from the underlying stream as necessary.  If,
		/// however, the buffer is empty, the mark is not valid, and the requested
		/// length is at least as large as the buffer, then this method will read
		/// characters directly from the underlying stream into the given array.
		/// Thus redundant <code>BufferedReader</code>s will not copy data
		/// unnecessarily.
		/// 
		/// </para>
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

				int n = Read1(cbuf, off, len);
				if (n <= 0)
				{
					return n;
				}
				while ((n < len) && @in.Ready())
				{
					int n1 = Read1(cbuf, off + n, len - n);
					if (n1 <= 0)
					{
						break;
					}
					n += n1;
				}
				return n;
			}
		}

		/// <summary>
		/// Reads a line of text.  A line is considered to be terminated by any one
		/// of a line feed ('\n'), a carriage return ('\r'), or a carriage return
		/// followed immediately by a linefeed.
		/// </summary>
		/// <param name="ignoreLF">  If true, the next '\n' will be skipped
		/// </param>
		/// <returns>     A String containing the contents of the line, not including
		///             any line-termination characters, or null if the end of the
		///             stream has been reached
		/// </returns>
		/// <seealso cref=        java.io.LineNumberReader#readLine()
		/// </seealso>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String readLine(boolean ignoreLF) throws IOException
		internal virtual String ReadLine(bool ignoreLF)
		{
			StringBuffer s = null;
			int startChar;

			lock (@lock)
			{
				EnsureOpen();
				bool omitLF = ignoreLF || SkipLF;

				for (;;)
				{

					if (NextChar >= NChars)
					{
						Fill();
					}
					if (NextChar >= NChars) // EOF
					{
						if (s != null && s.Length() > 0)
						{
							return s.ToString();
						}
						else
						{
							return null;
						}
					}
					bool eol = false;
					char c = (char)0;
					int i;

					/* Skip a leftover '\n', if necessary */
					if (omitLF && (Cb[NextChar] == '\n'))
					{
						NextChar++;
					}
					SkipLF = false;
					omitLF = false;

					for (i = NextChar; i < NChars; i++)
					{
						c = Cb[i];
						if ((c == '\n') || (c == '\r'))
						{
							eol = true;
							goto charLoopBreak;
						}
					charLoopContinue:;
					}
				charLoopBreak:

					startChar = NextChar;
					NextChar = i;

					if (eol)
					{
						String str;
						if (s == null)
						{
							str = new String(Cb, startChar, i - startChar);
						}
						else
						{
							s.Append(Cb, startChar, i - startChar);
							str = s.ToString();
						}
						NextChar++;
						if (c == '\r')
						{
							SkipLF = true;
						}
						return str;
					}

					if (s == null)
					{
						s = new StringBuffer(DefaultExpectedLineLength);
					}
					s.Append(Cb, startChar, i - startChar);
				bufferLoopContinue:;
				}
			bufferLoopBreak:;
			}
		}

		/// <summary>
		/// Reads a line of text.  A line is considered to be terminated by any one
		/// of a line feed ('\n'), a carriage return ('\r'), or a carriage return
		/// followed immediately by a linefeed.
		/// </summary>
		/// <returns>     A String containing the contents of the line, not including
		///             any line-termination characters, or null if the end of the
		///             stream has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs
		/// </exception>
		/// <seealso cref= java.nio.file.Files#readAllLines </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String readLine() throws IOException
		public virtual String ReadLine()
		{
			return ReadLine(false);
		}

		/// <summary>
		/// Skips characters.
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
				long r = n;
				while (r > 0)
				{
					if (NextChar >= NChars)
					{
						Fill();
					}
					if (NextChar >= NChars) // EOF
					{
						break;
					}
					if (SkipLF)
					{
						SkipLF = false;
						if (Cb[NextChar] == '\n')
						{
							NextChar++;
						}
					}
					long d = NChars - NextChar;
					if (r <= d)
					{
						NextChar += (int)r;
						r = 0;
						break;
					}
					else
					{
						r -= d;
						NextChar = NChars;
					}
				}
				return n - r;
			}
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.  A buffered character
		/// stream is ready if the buffer is not empty, or if the underlying
		/// character stream is ready.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			lock (@lock)
			{
				EnsureOpen();

				/*
				 * If newline needs to be skipped and the next char to be read
				 * is a newline character, then just skip it right away.
				 */
				if (SkipLF)
				{
					/* Note that in.ready() will return true if and only if the next
					 * read on the stream will not block.
					 */
					if (NextChar >= NChars && @in.Ready())
					{
						Fill();
					}
					if (NextChar < NChars)
					{
						if (Cb[NextChar] == '\n')
						{
							NextChar++;
						}
						SkipLF = false;
					}
				}
				return (NextChar < NChars) || @in.Ready();
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
		/// will attempt to reposition the stream to this point.
		/// </summary>
		/// <param name="readAheadLimit">   Limit on the number of characters that may be
		///                         read while still preserving the mark. An attempt
		///                         to reset the stream after reading characters
		///                         up to this limit or beyond may fail.
		///                         A limit value larger than the size of the input
		///                         buffer will cause a new buffer to be allocated
		///                         whose size is no smaller than limit.
		///                         Therefore large values should be used with care.
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
				this.ReadAheadLimit = readAheadLimit;
				MarkedChar = NextChar;
				MarkedSkipLF = SkipLF;
			}
		}

		/// <summary>
		/// Resets the stream to the most recent mark.
		/// </summary>
		/// <exception cref="IOException">  If the stream has never been marked,
		///                          or if the mark has been invalidated </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			lock (@lock)
			{
				EnsureOpen();
				if (MarkedChar < 0)
				{
					throw new IOException((MarkedChar == INVALIDATED) ? "Mark invalid" : "Stream not marked");
				}
				NextChar = MarkedChar;
				SkipLF = MarkedSkipLF;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			lock (@lock)
			{
				if (@in == null)
				{
					return;
				}
				try
				{
					@in.Close();
				}
				finally
				{
					@in = null;
					Cb = null;
				}
			}
		}

		/// <summary>
		/// Returns a {@code Stream}, the elements of which are lines read from
		/// this {@code BufferedReader}.  The <seealso cref="Stream"/> is lazily populated,
		/// i.e., read only occurs during the
		/// <a href="../util/stream/package-summary.html#StreamOps">terminal
		/// stream operation</a>.
		/// 
		/// <para> The reader must not be operated on during the execution of the
		/// terminal stream operation. Otherwise, the result of the terminal stream
		/// operation is undefined.
		/// 
		/// </para>
		/// <para> After execution of the terminal stream operation there are no
		/// guarantees that the reader will be at a specific position from which to
		/// read the next character or line.
		/// 
		/// </para>
		/// <para> If an <seealso cref="IOException"/> is thrown when accessing the underlying
		/// {@code BufferedReader}, it is wrapped in an {@link
		/// UncheckedIOException} which will be thrown from the {@code Stream}
		/// method that caused the read to take place. This method will return a
		/// Stream if invoked on a BufferedReader that is closed. Any operation on
		/// that stream that requires reading from the BufferedReader after it is
		/// closed, will cause an UncheckedIOException to be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Stream<String>} providing the lines of text
		///         described by this {@code BufferedReader}
		/// 
		/// @since 1.8 </returns>
		public virtual Stream<String> Lines()
		{
			IEnumerator<String> iter = new IteratorAnonymousInnerClassHelper(this);
			return StreamSupport.Stream(Spliterators.SpliteratorUnknownSize(iter, java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL), false);
		}

		private class IteratorAnonymousInnerClassHelper : Iterator<String>
		{
			private readonly BufferedReader OuterInstance;

			public IteratorAnonymousInnerClassHelper(BufferedReader outerInstance)
			{
				this.OuterInstance = outerInstance;
				nextLine = null;
			}

			internal String nextLine;

			public virtual bool HasNext()
			{
				if (nextLine != null)
				{
					return true;
				}
				else
				{
					try
					{
						nextLine = outerInstance.ReadLine();
						return (nextLine != null);
					}
					catch (IOException e)
					{
						throw new UncheckedIOException(e);
					}
				}
			}

			public virtual String Next()
			{
				if (nextLine != null || hasNext())
				{
					String line = nextLine;
					nextLine = null;
					return line;
				}
				else
				{
					throw new NoSuchElementException();
				}
			}
		}
	}

}