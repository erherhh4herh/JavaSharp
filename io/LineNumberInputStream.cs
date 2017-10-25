using System;

/*
 * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// This class is an input stream filter that provides the added
	/// functionality of keeping track of the current line number.
	/// <para>
	/// A line is a sequence of bytes ending with a carriage return
	/// character ({@code '\u005Cr'}), a newline character
	/// ({@code '\u005Cn'}), or a carriage return character followed
	/// immediately by a linefeed character. In all three cases, the line
	/// terminating character(s) are returned as a single newline character.
	/// </para>
	/// <para>
	/// The line number begins at {@code 0}, and is incremented by
	/// {@code 1} when a {@code read} returns a newline character.
	/// 
	/// @author     Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=        java.io.LineNumberReader
	/// @since      JDK1.0 </seealso>
	/// @deprecated This class incorrectly assumes that bytes adequately represent
	///             characters.  As of JDK&nbsp;1.1, the preferred way to operate on
	///             character streams is via the new character-stream classes, which
	///             include a class for counting line numbers. 
	[Obsolete("This class incorrectly assumes that bytes adequately represent")]
	public class LineNumberInputStream : FilterInputStream
	{
		internal int PushBack = -1;
		internal int LineNumber_Renamed;
		internal int MarkLineNumber;
		internal int MarkPushBack = -1;

		/// <summary>
		/// Constructs a newline number input stream that reads its input
		/// from the specified input stream.
		/// </summary>
		/// <param name="in">   the underlying input stream. </param>
		public LineNumberInputStream(InputStream @in) : base(@in)
		{
		}

		/// <summary>
		/// Reads the next byte of data from this input stream. The value
		/// byte is returned as an {@code int} in the range
		/// {@code 0} to {@code 255}. If no byte is available
		/// because the end of the stream has been reached, the value
		/// {@code -1} is returned. This method blocks until input data
		/// is available, the end of the stream is detected, or an exception
		/// is thrown.
		/// <para>
		/// The {@code read} method of
		/// {@code LineNumberInputStream} calls the {@code read}
		/// method of the underlying input stream. It checks for carriage
		/// returns and newline characters in the input, and modifies the
		/// current line number as appropriate. A carriage-return character or
		/// a carriage return followed by a newline character are both
		/// converted into a single newline character.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of data, or {@code -1} if the end of this
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.LineNumberInputStream#getLineNumber() </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public int read() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public override int Read()
		{
			int c = PushBack;

			if (c != -1)
			{
				PushBack = -1;
			}
			else
			{
				c = @in.Read();
			}

			switch (c)
			{
			  case '\r':
				PushBack = @in.Read();
				if (PushBack == '\n')
				{
					PushBack = -1;
				}
				  goto case '\n';
			  case '\n':
				LineNumber_Renamed++;
				return '\n';
			}
			return c;
		}

		/// <summary>
		/// Reads up to {@code len} bytes of data from this input stream
		/// into an array of bytes. This method blocks until some input is available.
		/// <para>
		/// The {@code read} method of
		/// {@code LineNumberInputStream} repeatedly calls the
		/// {@code read} method of zero arguments to fill in the byte array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset of the data. </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             {@code -1} if there is no more data because the end of
		///             this stream has been reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.LineNumberInputStream#read() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			else if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return 0;
			}

			int c = Read();
			if (c == -1)
			{
				return -1;
			}
			b[off] = (sbyte)c;

			int i = 1;
			try
			{
				for (; i < len ; i++)
				{
					c = Read();
					if (c == -1)
					{
						break;
					}
					if (b != null)
					{
						b[off + i] = (sbyte)c;
					}
				}
			}
			catch (IOException)
			{
			}
			return i;
		}

		/// <summary>
		/// Skips over and discards {@code n} bytes of data from this
		/// input stream. The {@code skip} method may, for a variety of
		/// reasons, end up skipping over some smaller number of bytes,
		/// possibly {@code 0}. The actual number of bytes skipped is
		/// returned.  If {@code n} is negative, no bytes are skipped.
		/// <para>
		/// The {@code skip} method of {@code LineNumberInputStream} creates
		/// a byte array and then repeatedly reads into it until
		/// {@code n} bytes have been read or the end of the stream has
		/// been reached.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   the number of bytes to be skipped. </param>
		/// <returns>     the actual number of bytes skipped. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			int chunk = 2048;
			long remaining = n;
			sbyte[] data;
			int nr;

			if (n <= 0)
			{
				return 0;
			}

			data = new sbyte[chunk];
			while (remaining > 0)
			{
				nr = Read(data, 0, (int) System.Math.Min(chunk, remaining));
				if (nr < 0)
				{
					break;
				}
				remaining -= nr;
			}

			return n - remaining;
		}

		/// <summary>
		/// Sets the line number to the specified argument.
		/// </summary>
		/// <param name="lineNumber">   the new line number. </param>
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
		/// Returns the number of bytes that can be read from this input
		/// stream without blocking.
		/// <para>
		/// Note that if the underlying input stream is able to supply
		/// <i>k</i> input characters without blocking, the
		/// {@code LineNumberInputStream} can guarantee only to provide
		/// <i>k</i>/2 characters without blocking, because the
		/// <i>k</i> characters from the underlying input stream might
		/// consist of <i>k</i>/2 pairs of {@code '\u005Cr'} and
		/// {@code '\u005Cn'}, which are converted to just
		/// <i>k</i>/2 {@code '\u005Cn'} characters.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the number of bytes that can be read from this input stream
		///             without blocking. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
		public override int Available()
		{
			return (PushBack == -1) ? base.Available() / 2 : base.Available() / 2 + 1;
		}

		/// <summary>
		/// Marks the current position in this input stream. A subsequent
		/// call to the {@code reset} method repositions this stream at
		/// the last marked position so that subsequent reads re-read the same bytes.
		/// <para>
		/// The {@code mark} method of
		/// {@code LineNumberInputStream} remembers the current line
		/// number in a private variable, and then calls the {@code mark}
		/// method of the underlying input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="readlimit">   the maximum limit of bytes that can be read before
		///                      the mark position becomes invalid. </param>
		/// <seealso cref=     java.io.FilterInputStream#in </seealso>
		/// <seealso cref=     java.io.LineNumberInputStream#reset() </seealso>
		public override void Mark(int readlimit)
		{
			MarkLineNumber = LineNumber_Renamed;
			MarkPushBack = PushBack;
			@in.Mark(readlimit);
		}

		/// <summary>
		/// Repositions this stream to the position at the time the
		/// {@code mark} method was last called on this input stream.
		/// <para>
		/// The {@code reset} method of
		/// {@code LineNumberInputStream} resets the line number to be
		/// the line number at the time the {@code mark} method was
		/// called, and then calls the {@code reset} method of the
		/// underlying input stream.
		/// </para>
		/// <para>
		/// Stream marks are intended to be used in
		/// situations where you need to read ahead a little to see what's in
		/// the stream. Often this is most easily done by invoking some
		/// general parser. If the stream is of the type handled by the
		/// parser, it just chugs along happily. If the stream is not of
		/// that type, the parser should toss an exception when it fails,
		/// which, if it happens within readlimit bytes, allows the outer
		/// code to reset the stream and try another parser.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.LineNumberInputStream#mark(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			LineNumber_Renamed = MarkLineNumber;
			PushBack = MarkPushBack;
			@in.Reset();
		}
	}

}