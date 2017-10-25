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
	/// Writes text to a character-output stream, buffering characters so as to
	/// provide for the efficient writing of single characters, arrays, and strings.
	/// 
	/// <para> The buffer size may be specified, or the default size may be accepted.
	/// The default is large enough for most purposes.
	/// 
	/// </para>
	/// <para> A newLine() method is provided, which uses the platform's own notion of
	/// line separator as defined by the system property <tt>line.separator</tt>.
	/// Not all platforms use the newline character ('\n') to terminate lines.
	/// Calling this method to terminate each output line is therefore preferred to
	/// writing a newline character directly.
	/// 
	/// </para>
	/// <para> In general, a Writer sends its output immediately to the underlying
	/// character or byte stream.  Unless prompt output is required, it is advisable
	/// to wrap a BufferedWriter around any Writer whose write() operations may be
	/// costly, such as FileWriters and OutputStreamWriters.  For example,
	/// 
	/// <pre>
	/// PrintWriter out
	///   = new PrintWriter(new BufferedWriter(new FileWriter("foo.out")));
	/// </pre>
	/// 
	/// will buffer the PrintWriter's output to the file.  Without buffering, each
	/// invocation of a print() method would cause characters to be converted into
	/// bytes that would then be written immediately to the file, which can be very
	/// inefficient.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= PrintWriter </seealso>
	/// <seealso cref= FileWriter </seealso>
	/// <seealso cref= OutputStreamWriter </seealso>
	/// <seealso cref= java.nio.file.Files#newBufferedWriter
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public class BufferedWriter : Writer
	{

		private Writer @out;

		private char[] Cb;
		private int NChars, NextChar;

		private static int DefaultCharBufferSize = 8192;

		/// <summary>
		/// Line separator string.  This is the value of the line.separator
		/// property at the moment that the stream was created.
		/// </summary>
		private String LineSeparator;

		/// <summary>
		/// Creates a buffered character-output stream that uses a default-sized
		/// output buffer.
		/// </summary>
		/// <param name="out">  A Writer </param>
		public BufferedWriter(Writer @out) : this(@out, DefaultCharBufferSize)
		{
		}

		/// <summary>
		/// Creates a new buffered character-output stream that uses an output
		/// buffer of the given size.
		/// </summary>
		/// <param name="out">  A Writer </param>
		/// <param name="sz">   Output-buffer size, a positive integer
		/// </param>
		/// <exception cref="IllegalArgumentException">  If {@code sz <= 0} </exception>
		public BufferedWriter(Writer @out, int sz) : base(@out)
		{
			if (sz <= 0)
			{
				throw new IllegalArgumentException("Buffer size <= 0");
			}
			this.@out = @out;
			Cb = new char[sz];
			NChars = sz;
			NextChar = 0;

			LineSeparator = java.security.AccessController.doPrivileged(new sun.security.action.GetPropertyAction("line.separator"));
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws IOException
		private void EnsureOpen()
		{
			if (@out == null)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Flushes the output buffer to the underlying character stream, without
		/// flushing the stream itself.  This method is non-private only so that it
		/// may be invoked by PrintStream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void flushBuffer() throws IOException
		internal virtual void FlushBuffer()
		{
			lock (@lock)
			{
				EnsureOpen();
				if (NextChar == 0)
				{
					return;
				}
				@out.Write(Cb, 0, NextChar);
				NextChar = 0;
			}
		}

		/// <summary>
		/// Writes a single character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws IOException
		public override void Write(int c)
		{
			lock (@lock)
			{
				EnsureOpen();
				if (NextChar >= NChars)
				{
					FlushBuffer();
				}
				Cb[NextChar++] = (char) c;
			}
		}

		/// <summary>
		/// Our own little min method, to avoid loading java.lang.Math if we've run
		/// out of file descriptors and we're trying to print a stack trace.
		/// </summary>
		private int Min(int a, int b)
		{
			if (a < b)
			{
				return a;
			}
			return b;
		}

		/// <summary>
		/// Writes a portion of an array of characters.
		/// 
		/// <para> Ordinarily this method stores characters from the given array into
		/// this stream's buffer, flushing the buffer to the underlying stream as
		/// needed.  If the requested length is at least as large as the buffer,
		/// however, then this method will flush the buffer and write the characters
		/// directly to the underlying stream.  Thus redundant
		/// <code>BufferedWriter</code>s will not copy data unnecessarily.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cbuf">  A character array </param>
		/// <param name="off">   Offset from which to start reading characters </param>
		/// <param name="len">   Number of characters to write
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char cbuf[] , int off, int len) throws IOException
		public override void Write(char[] cbuf, int off, int len)
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
					return;
				}

				if (len >= NChars)
				{
					/* If the request length exceeds the size of the output buffer,
					   flush the buffer and then write the data directly.  In this
					   way buffered streams will cascade harmlessly. */
					FlushBuffer();
					@out.Write(cbuf, off, len);
					return;
				}

				int b = off, t = off + len;
				while (b < t)
				{
					int d = Min(NChars - NextChar, t - b);
					System.Array.Copy(cbuf, b, Cb, NextChar, d);
					b += d;
					NextChar += d;
					if (NextChar >= NChars)
					{
						FlushBuffer();
					}
				}
			}
		}

		/// <summary>
		/// Writes a portion of a String.
		/// 
		/// <para> If the value of the <tt>len</tt> parameter is negative then no
		/// characters are written.  This is contrary to the specification of this
		/// method in the {@link java.io.Writer#write(java.lang.String,int,int)
		/// superclass}, which requires that an <seealso cref="IndexOutOfBoundsException"/> be
		/// thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">     String to be written </param>
		/// <param name="off">   Offset from which to start reading characters </param>
		/// <param name="len">   Number of characters to be written
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String s, int off, int len) throws IOException
		public override void Write(String s, int off, int len)
		{
			lock (@lock)
			{
				EnsureOpen();

				int b = off, t = off + len;
				while (b < t)
				{
					int d = Min(NChars - NextChar, t - b);
					s.GetChars(b, b + d, Cb, NextChar);
					b += d;
					NextChar += d;
					if (NextChar >= NChars)
					{
						FlushBuffer();
					}
				}
			}
		}

		/// <summary>
		/// Writes a line separator.  The line separator string is defined by the
		/// system property <tt>line.separator</tt>, and is not necessarily a single
		/// newline ('\n') character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void newLine() throws IOException
		public virtual void NewLine()
		{
			Write(LineSeparator);
		}

		/// <summary>
		/// Flushes the stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			lock (@lock)
			{
				FlushBuffer();
				@out.Flush();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("try") public void close() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public override void Close()
		{
			lock (@lock)
			{
				if (@out == null)
				{
					return;
				}
				try
				{
						using (Writer w = @out)
						{
						FlushBuffer();
						}
				}
				finally
				{
					@out = null;
					Cb = null;
				}
			}
		}
	}

}