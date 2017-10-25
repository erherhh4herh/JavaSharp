using System;

/*
 * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// This class allows an application to create an input stream in
	/// which the bytes read are supplied by the contents of a string.
	/// Applications can also read bytes from a byte array by using a
	/// <code>ByteArrayInputStream</code>.
	/// <para>
	/// Only the low eight bits of each character in the string are used by
	/// this class.
	/// 
	/// @author     Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=        java.io.ByteArrayInputStream </seealso>
	/// <seealso cref=        java.io.StringReader
	/// @since      JDK1.0 </seealso>
	/// @deprecated This class does not properly convert characters into bytes.  As
	///             of JDK&nbsp;1.1, the preferred way to create a stream from a
	///             string is via the <code>StringReader</code> class. 
	[Obsolete("This class does not properly convert characters into bytes.  As")]
	public class StringBufferInputStream : InputStream
	{
		/// <summary>
		/// The string from which bytes are read.
		/// </summary>
		protected internal String Buffer;

		/// <summary>
		/// The index of the next character to read from the input stream buffer.
		/// </summary>
		/// <seealso cref=        java.io.StringBufferInputStream#buffer </seealso>
		protected internal int Pos;

		/// <summary>
		/// The number of valid characters in the input stream buffer.
		/// </summary>
		/// <seealso cref=        java.io.StringBufferInputStream#buffer </seealso>
		protected internal int Count;

		/// <summary>
		/// Creates a string input stream to read data from the specified string.
		/// </summary>
		/// <param name="s">   the underlying input buffer. </param>
		public StringBufferInputStream(String s)
		{
			this.Buffer = s;
			Count = s.Length();
		}

		/// <summary>
		/// Reads the next byte of data from this input stream. The value
		/// byte is returned as an <code>int</code> in the range
		/// <code>0</code> to <code>255</code>. If no byte is available
		/// because the end of the stream has been reached, the value
		/// <code>-1</code> is returned.
		/// <para>
		/// The <code>read</code> method of
		/// <code>StringBufferInputStream</code> cannot block. It returns the
		/// low eight bits of the next character in this input stream's buffer.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		public override int Read()
		{
			lock (this)
			{
				return (Pos < Count) ? (Buffer.CharAt(Pos++) & 0xFF) : -1;
			}
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this input stream
		/// into an array of bytes.
		/// <para>
		/// The <code>read</code> method of
		/// <code>StringBufferInputStream</code> cannot block. It copies the
		/// low eight bits from the characters in this input stream's buffer into
		/// the byte array argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset of the data. </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the stream has been reached. </returns>
		public override int Read(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				if (b == null)
				{
					throw new NullPointerException();
				}
				else if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
				{
					throw new IndexOutOfBoundsException();
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
				String s = Buffer;
				int cnt = len;
				while (--cnt >= 0)
				{
					b[off++] = (sbyte)s.CharAt(Pos++);
				}
        
				return len;
			}
		}

		/// <summary>
		/// Skips <code>n</code> bytes of input from this input stream. Fewer
		/// bytes might be skipped if the end of the input stream is reached.
		/// </summary>
		/// <param name="n">   the number of bytes to be skipped. </param>
		/// <returns>     the actual number of bytes skipped. </returns>
		public override long Skip(long n)
		{
			lock (this)
			{
				if (n < 0)
				{
					return 0;
				}
				if (n > Count - Pos)
				{
					n = Count - Pos;
				}
				Pos += (int)n;
				return n;
			}
		}

		/// <summary>
		/// Returns the number of bytes that can be read from the input
		/// stream without blocking.
		/// </summary>
		/// <returns>     the value of <code>count&nbsp;-&nbsp;pos</code>, which is the
		///             number of bytes remaining to be read from the input buffer. </returns>
		public override int Available()
		{
			lock (this)
			{
				return Count - Pos;
			}
		}

		/// <summary>
		/// Resets the input stream to begin reading from the first character
		/// of this input stream's underlying buffer.
		/// </summary>
		public override void Reset()
		{
			lock (this)
			{
				Pos = 0;
			}
		}
	}

}