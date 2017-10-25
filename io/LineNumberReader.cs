using System;

/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A buffered character-input stream that keeps track of line numbers.  This
	/// class defines methods <seealso cref="#setLineNumber(int)"/> and {@link
	/// #getLineNumber()} for setting and getting the current line number
	/// respectively.
	/// 
	/// <para> By default, line numbering begins at 0. This number increments at every
	/// <a href="#lt">line terminator</a> as the data is read, and can be changed
	/// with a call to <tt>setLineNumber(int)</tt>.  Note however, that
	/// <tt>setLineNumber(int)</tt> does not actually change the current position in
	/// the stream; it only changes the value that will be returned by
	/// <tt>getLineNumber()</tt>.
	/// 
	/// </para>
	/// <para> A line is considered to be <a name="lt">terminated</a> by any one of a
	/// line feed ('\n'), a carriage return ('\r'), or a carriage return followed
	/// immediately by a linefeed.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </para>
	/// </summary>

	public class LineNumberReader : BufferedReader
	{

		/// <summary>
		/// The current line number </summary>
		private int LineNumber_Renamed = 0;

		/// <summary>
		/// The line number of the mark, if any </summary>
		private int MarkedLineNumber; // Defaults to 0

		/// <summary>
		/// If the next character is a line feed, skip it </summary>
		private bool SkipLF;

		/// <summary>
		/// The skipLF flag when the mark was set </summary>
		private bool MarkedSkipLF;

		/// <summary>
		/// Create a new line-numbering reader, using the default input-buffer
		/// size.
		/// </summary>
		/// <param name="in">
		///         A Reader object to provide the underlying stream </param>
		public LineNumberReader(Reader @in) : base(@in)
		{
		}

		/// <summary>
		/// Create a new line-numbering reader, reading characters into a buffer of
		/// the given size.
		/// </summary>
		/// <param name="in">
		///         A Reader object to provide the underlying stream
		/// </param>
		/// <param name="sz">
		///         An int specifying the size of the buffer </param>
		public LineNumberReader(Reader @in, int sz) : base(@in, sz)
		{
		}

		/// <summary>
		/// Set the current line number.
		/// </summary>
		/// <param name="lineNumber">
		///         An int specifying the line number
		/// </param>
		/// <seealso cref= #getLineNumber </seealso>
		public virtual int LineNumber
		{
			set
			{
				this.LineNumber_Renamed = value;
			}
			get
			{
				return LineNumber_Renamed;
			}
		}


		/// <summary>
		/// Read a single character.  <a href="#lt">Line terminators</a> are
		/// compressed into single newline ('\n') characters.  Whenever a line
		/// terminator is read the current line number is incremented.
		/// </summary>
		/// <returns>  The character read, or -1 if the end of the stream has been
		///          reached
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public int read() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public override int Read()
		{
			lock (@lock)
			{
				int c = base.Read();
				if (SkipLF)
				{
					if (c == '\n')
					{
						c = base.Read();
					}
					SkipLF = false;
				}
				switch (c)
				{
				case '\r':
					SkipLF = true;
					goto case '\n';
				case '\n': // Fall through
					LineNumber_Renamed++;
					return '\n';
				}
				return c;
			}
		}

		/// <summary>
		/// Read characters into a portion of an array.  Whenever a <a
		/// href="#lt">line terminator</a> is read the current line number is
		/// incremented.
		/// </summary>
		/// <param name="cbuf">
		///         Destination buffer
		/// </param>
		/// <param name="off">
		///         Offset at which to start storing characters
		/// </param>
		/// <param name="len">
		///         Maximum number of characters to read
		/// </param>
		/// <returns>  The number of bytes read, or -1 if the end of the stream has
		///          already been reached
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public int read(char cbuf[] , int off, int len) throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public override int Read(char[] cbuf, int off, int len)
		{
			lock (@lock)
			{
				int n = base.Read(cbuf, off, len);

				for (int i = off; i < off + n; i++)
				{
					int c = cbuf[i];
					if (SkipLF)
					{
						SkipLF = false;
						if (c == '\n')
						{
							continue;
						}
					}
					switch (c)
					{
					case '\r':
						SkipLF = true;
						goto case '\n';
					case '\n': // Fall through
						LineNumber_Renamed++;
						break;
					}
				}

				return n;
			}
		}

		/// <summary>
		/// Read a line of text.  Whenever a <a href="#lt">line terminator</a> is
		/// read the current line number is incremented.
		/// </summary>
		/// <returns>  A String containing the contents of the line, not including
		///          any <a href="#lt">line termination characters</a>, or
		///          <tt>null</tt> if the end of the stream has been reached
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String readLine() throws IOException
		public override String ReadLine()
		{
			lock (@lock)
			{
				String l = base.ReadLine(SkipLF);
				SkipLF = false;
				if (l != null)
				{
					LineNumber_Renamed++;
				}
				return l;
			}
		}

		/// <summary>
		/// Maximum skip-buffer size </summary>
		private const int MaxSkipBufferSize = 8192;

		/// <summary>
		/// Skip buffer, null until allocated </summary>
		private char[] SkipBuffer = null;

		/// <summary>
		/// Skip characters.
		/// </summary>
		/// <param name="n">
		///         The number of characters to skip
		/// </param>
		/// <returns>  The number of characters actually skipped
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If <tt>n</tt> is negative </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException("skip() value is negative");
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
					int nc = Read(SkipBuffer, 0, (int) System.Math.Min(r, nn));
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
		/// Mark the present position in the stream.  Subsequent calls to reset()
		/// will attempt to reposition the stream to this point, and will also reset
		/// the line number appropriately.
		/// </summary>
		/// <param name="readAheadLimit">
		///         Limit on the number of characters that may be read while still
		///         preserving the mark.  After reading this many characters,
		///         attempting to reset the stream may fail.
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public override void Mark(int readAheadLimit)
		{
			lock (@lock)
			{
				base.Mark(readAheadLimit);
				MarkedLineNumber = LineNumber_Renamed;
				MarkedSkipLF = SkipLF;
			}
		}

		/// <summary>
		/// Reset the stream to the most recent mark.
		/// </summary>
		/// <exception cref="IOException">
		///          If the stream has not been marked, or if the mark has been
		///          invalidated </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			lock (@lock)
			{
				base.Reset();
				LineNumber_Renamed = MarkedLineNumber;
				SkipLF = MarkedSkipLF;
			}
		}

	}

}