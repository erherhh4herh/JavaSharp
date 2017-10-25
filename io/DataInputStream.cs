using System;

/*
 * Copyright (c) 1994, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// A data input stream lets an application read primitive Java data
	/// types from an underlying input stream in a machine-independent
	/// way. An application uses a data output stream to write data that
	/// can later be read by a data input stream.
	/// <para>
	/// DataInputStream is not necessarily safe for multithreaded access.
	/// Thread safety is optional and is the responsibility of users of
	/// methods in this class.
	/// 
	/// @author  Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.DataOutputStream
	/// @since   JDK1.0 </seealso>
	public class DataInputStream : FilterInputStream, DataInput
	{

		/// <summary>
		/// Creates a DataInputStream that uses the specified
		/// underlying InputStream.
		/// </summary>
		/// <param name="in">   the specified input stream </param>
		public DataInputStream(InputStream @in) : base(@in)
		{
		}

		/// <summary>
		/// working arrays initialized on demand by readUTF
		/// </summary>
		private sbyte[] Bytearr = new sbyte[80];
		private char[] Chararr = new char[80];

		/// <summary>
		/// Reads some number of bytes from the contained input stream and
		/// stores them into the buffer array <code>b</code>. The number of
		/// bytes actually read is returned as an integer. This method blocks
		/// until input data is available, end of file is detected, or an
		/// exception is thrown.
		/// 
		/// <para>If <code>b</code> is null, a <code>NullPointerException</code> is
		/// thrown. If the length of <code>b</code> is zero, then no bytes are
		/// read and <code>0</code> is returned; otherwise, there is an attempt
		/// to read at least one byte. If no byte is available because the
		/// stream is at end of file, the value <code>-1</code> is returned;
		/// otherwise, at least one byte is read and stored into <code>b</code>.
		/// 
		/// </para>
		/// <para>The first byte read is stored into element <code>b[0]</code>, the
		/// next one into <code>b[1]</code>, and so on. The number of bytes read
		/// is, at most, equal to the length of <code>b</code>. Let <code>k</code>
		/// be the number of bytes actually read; these bytes will be stored in
		/// elements <code>b[0]</code> through <code>b[k-1]</code>, leaving
		/// elements <code>b[k]</code> through <code>b[b.length-1]</code>
		/// unaffected.
		/// 
		/// </para>
		/// <para>The <code>read(b)</code> method has the same effect as:
		/// <blockquote><pre>
		/// read(b, 0, b.length)
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the buffer into which the data is read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end
		///             of the stream has been reached. </returns>
		/// <exception cref="IOException"> if the first byte cannot be read for any reason
		/// other than end of file, the stream has been closed and the underlying
		/// input stream does not support reading after close, or another I/O
		/// error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int read(byte b[]) throws IOException
		public sealed override int Read(sbyte[] b)
		{
			return @in.Read(b, 0, b.Length);
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from the contained
		/// input stream into an array of bytes.  An attempt is made to read
		/// as many as <code>len</code> bytes, but a smaller number may be read,
		/// possibly zero. The number of bytes actually read is returned as an
		/// integer.
		/// 
		/// <para> This method blocks until input data is available, end of file is
		/// detected, or an exception is thrown.
		/// 
		/// </para>
		/// <para> If <code>len</code> is zero, then no bytes are read and
		/// <code>0</code> is returned; otherwise, there is an attempt to read at
		/// least one byte. If no byte is available because the stream is at end of
		/// file, the value <code>-1</code> is returned; otherwise, at least one
		/// byte is read and stored into <code>b</code>.
		/// 
		/// </para>
		/// <para> The first byte read is stored into element <code>b[off]</code>, the
		/// next one into <code>b[off+1]</code>, and so on. The number of bytes read
		/// is, at most, equal to <code>len</code>. Let <i>k</i> be the number of
		/// bytes actually read; these bytes will be stored in elements
		/// <code>b[off]</code> through <code>b[off+</code><i>k</i><code>-1]</code>,
		/// leaving elements <code>b[off+</code><i>k</i><code>]</code> through
		/// <code>b[off+len-1]</code> unaffected.
		/// 
		/// </para>
		/// <para> In every case, elements <code>b[0]</code> through
		/// <code>b[off]</code> and elements <code>b[off+len]</code> through
		/// <code>b[b.length-1]</code> are unaffected.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end
		///             of the stream has been reached. </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="IOException"> if the first byte cannot be read for any reason
		/// other than end of file, the stream has been closed and the underlying
		/// input stream does not support reading after close, or another I/O
		/// error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int read(byte b[] , int off, int len) throws IOException
		public sealed override int Read(sbyte[] b, int off, int len)
		{
			return @in.Read(b, off, len);
		}

		/// <summary>
		/// See the general contract of the <code>readFully</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the buffer into which the data is read. </param>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///             reading all the bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void readFully(byte b[]) throws IOException
		public void ReadFully(sbyte[] b)
		{
			ReadFully(b, 0, b.Length);
		}

		/// <summary>
		/// See the general contract of the <code>readFully</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset of the data. </param>
		/// <param name="len">   the number of bytes to read. </param>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading all the bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void readFully(byte b[] , int off, int len) throws IOException
		public void ReadFully(sbyte[] b, int off, int len)
		{
			if (len < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			int n = 0;
			while (n < len)
			{
				int count = @in.Read(b, off + n, len - n);
				if (count < 0)
				{
					throw new EOFException();
				}
				n += count;
			}
		}

		/// <summary>
		/// See the general contract of the <code>skipBytes</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   the number of bytes to be skipped. </param>
		/// <returns>     the actual number of bytes skipped. </returns>
		/// <exception cref="IOException">  if the contained input stream does not support
		///             seek, or the stream has been closed and
		///             the contained input stream does not support
		///             reading after close, or another I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int skipBytes(int n) throws IOException
		public int SkipBytes(int n)
		{
			int total = 0;
			int cur = 0;

			while ((total < n) && ((cur = (int) @in.Skip(n - total)) > 0))
			{
				total += cur;
			}

			return total;
		}

		/// <summary>
		/// See the general contract of the <code>readBoolean</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the <code>boolean</code> value read. </returns>
		/// <exception cref="EOFException">  if this input stream has reached the end. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean readBoolean() throws IOException
		public bool ReadBoolean()
		{
			int ch = @in.Read();
			if (ch < 0)
			{
				throw new EOFException();
			}
			return (ch != 0);
		}

		/// <summary>
		/// See the general contract of the <code>readByte</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of this input stream as a signed 8-bit
		///             <code>byte</code>. </returns>
		/// <exception cref="EOFException">  if this input stream has reached the end. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final byte readByte() throws IOException
		public sbyte ReadByte()
		{
			int ch = @in.Read();
			if (ch < 0)
			{
				throw new EOFException();
			}
			return (sbyte)(ch);
		}

		/// <summary>
		/// See the general contract of the <code>readUnsignedByte</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of this input stream, interpreted as an
		///             unsigned 8-bit number. </returns>
		/// <exception cref="EOFException">  if this input stream has reached the end. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=         java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int readUnsignedByte() throws IOException
		public int ReadUnsignedByte()
		{
			int ch = @in.Read();
			if (ch < 0)
			{
				throw new EOFException();
			}
			return ch;
		}

		/// <summary>
		/// See the general contract of the <code>readShort</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next two bytes of this input stream, interpreted as a
		///             signed 16-bit number. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading two bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final short readShort() throws IOException
		public short ReadShort()
		{
			int ch1 = @in.Read();
			int ch2 = @in.Read();
			if ((ch1 | ch2) < 0)
			{
				throw new EOFException();
			}
			return (short)((ch1 << 8) + (ch2 << 0));
		}

		/// <summary>
		/// See the general contract of the <code>readUnsignedShort</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next two bytes of this input stream, interpreted as an
		///             unsigned 16-bit integer. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///             reading two bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int readUnsignedShort() throws IOException
		public int ReadUnsignedShort()
		{
			int ch1 = @in.Read();
			int ch2 = @in.Read();
			if ((ch1 | ch2) < 0)
			{
				throw new EOFException();
			}
			return (ch1 << 8) + (ch2 << 0);
		}

		/// <summary>
		/// See the general contract of the <code>readChar</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next two bytes of this input stream, interpreted as a
		///             <code>char</code>. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading two bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final char readChar() throws IOException
		public char ReadChar()
		{
			int ch1 = @in.Read();
			int ch2 = @in.Read();
			if ((ch1 | ch2) < 0)
			{
				throw new EOFException();
			}
			return (char)((ch1 << 8) + (ch2 << 0));
		}

		/// <summary>
		/// See the general contract of the <code>readInt</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next four bytes of this input stream, interpreted as an
		///             <code>int</code>. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading four bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int readInt() throws IOException
		public int ReadInt()
		{
			int ch1 = @in.Read();
			int ch2 = @in.Read();
			int ch3 = @in.Read();
			int ch4 = @in.Read();
			if ((ch1 | ch2 | ch3 | ch4) < 0)
			{
				throw new EOFException();
			}
			return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
		}

		private sbyte[] ReadBuffer = new sbyte[8];

		/// <summary>
		/// See the general contract of the <code>readLong</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next eight bytes of this input stream, interpreted as a
		///             <code>long</code>. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading eight bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final long readLong() throws IOException
		public long ReadLong()
		{
			ReadFully(ReadBuffer, 0, 8);
			return (((long)ReadBuffer[0] << 56) + ((long)(ReadBuffer[1] & 255) << 48) + ((long)(ReadBuffer[2] & 255) << 40) + ((long)(ReadBuffer[3] & 255) << 32) + ((long)(ReadBuffer[4] & 255) << 24) + ((ReadBuffer[5] & 255) << 16) + ((ReadBuffer[6] & 255) << 8) + ((ReadBuffer[7] & 255) << 0));
		}

		/// <summary>
		/// See the general contract of the <code>readFloat</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next four bytes of this input stream, interpreted as a
		///             <code>float</code>. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading four bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.DataInputStream#readInt() </seealso>
		/// <seealso cref=        java.lang.Float#intBitsToFloat(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final float readFloat() throws IOException
		public float ReadFloat()
		{
			return Float.intBitsToFloat(ReadInt());
		}

		/// <summary>
		/// See the general contract of the <code>readDouble</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next eight bytes of this input stream, interpreted as a
		///             <code>double</code>. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading eight bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <seealso cref=        java.io.DataInputStream#readLong() </seealso>
		/// <seealso cref=        java.lang.Double#longBitsToDouble(long) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final double readDouble() throws IOException
		public double ReadDouble()
		{
			return Double.longBitsToDouble(ReadLong());
		}

		private char[] LineBuffer;

		/// <summary>
		/// See the general contract of the <code>readLine</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// @deprecated This method does not properly convert bytes to characters.
		/// As of JDK&nbsp;1.1, the preferred way to read lines of text is via the
		/// <code>BufferedReader.readLine()</code> method.  Programs that use the
		/// <code>DataInputStream</code> class to read lines can be converted to use
		/// the <code>BufferedReader</code> class by replacing code of the form:
		/// <blockquote><pre>
		///     DataInputStream d =&nbsp;new&nbsp;DataInputStream(in);
		/// </pre></blockquote>
		/// with:
		/// <blockquote><pre>
		///     BufferedReader d
		///          =&nbsp;new&nbsp;BufferedReader(new&nbsp;InputStreamReader(in));
		/// </pre></blockquote>
		/// 
		/// <returns>     the next line of text from this input stream. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.BufferedReader#readLine() </seealso>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("This method does not properly convert bytes to characters.") public final String readLine() throws IOException
		[Obsolete("This method does not properly convert bytes to characters.")]
		public String ReadLine()
		{
			char[] buf = LineBuffer;

			if (buf == null)
			{
				buf = LineBuffer = new char[128];
			}

			int room = buf.Length;
			int offset = 0;
			int c;

	while (true)
	{
				switch (c = @in.Read())
				{
				  case -1:
				  case '\n':
					goto loopBreak;

				  case '\r':
					int c2 = @in.Read();
					if ((c2 != '\n') && (c2 != -1))
					{
						if (!(@in is PushbackInputStream))
						{
							this.@in = new PushbackInputStream(@in);
						}((PushbackInputStream)@in).unread(c2);
					}
					goto loopBreak;

				  default:
					if (--room < 0)
					{
						buf = new char[offset + 128];
						room = buf.Length - offset - 1;
						System.Array.Copy(LineBuffer, 0, buf, 0, offset);
						LineBuffer = buf;
					}
					buf[offset++] = (char) c;
					break;
				}
		loopContinue:;
	}
	loopBreak:
			if ((c == -1) && (offset == 0))
			{
				return null;
			}
			return String.CopyValueOf(buf, 0, offset);
		}

		/// <summary>
		/// See the general contract of the <code>readUTF</code>
		/// method of <code>DataInput</code>.
		/// <para>
		/// Bytes
		/// for this operation are read from the contained
		/// input stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     a Unicode string. </returns>
		/// <exception cref="EOFException">  if this input stream reaches the end before
		///               reading all the bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <exception cref="UTFDataFormatException"> if the bytes do not represent a valid
		///             modified UTF-8 encoding of a string. </exception>
		/// <seealso cref=        java.io.DataInputStream#readUTF(java.io.DataInput) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String readUTF() throws IOException
		public String ReadUTF()
		{
			return ReadUTF(this);
		}

		/// <summary>
		/// Reads from the
		/// stream <code>in</code> a representation
		/// of a Unicode  character string encoded in
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a> format;
		/// this string of characters is then returned as a <code>String</code>.
		/// The details of the modified UTF-8 representation
		/// are  exactly the same as for the <code>readUTF</code>
		/// method of <code>DataInput</code>.
		/// </summary>
		/// <param name="in">   a data input stream. </param>
		/// <returns>     a Unicode string. </returns>
		/// <exception cref="EOFException">            if the input stream reaches the end
		///               before all the bytes. </exception>
		/// <exception cref="IOException">   the stream has been closed and the contained
		///             input stream does not support reading after close, or
		///             another I/O error occurs. </exception>
		/// <exception cref="UTFDataFormatException">  if the bytes do not represent a
		///               valid modified UTF-8 encoding of a Unicode string. </exception>
		/// <seealso cref=        java.io.DataInputStream#readUnsignedShort() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final static String readUTF(DataInput in) throws IOException
		public static String ReadUTF(DataInput @in)
		{
			int utflen = @in.ReadUnsignedShort();
			sbyte[] bytearr = null;
			char[] chararr = null;
			if (@in is DataInputStream)
			{
				DataInputStream dis = (DataInputStream)@in;
				if (dis.Bytearr.Length < utflen)
				{
					dis.Bytearr = new sbyte[utflen * 2];
					dis.Chararr = new char[utflen * 2];
				}
				chararr = dis.Chararr;
				bytearr = dis.Bytearr;
			}
			else
			{
				bytearr = new sbyte[utflen];
				chararr = new char[utflen];
			}

			int c, char2, char3;
			int count = 0;
			int chararr_count = 0;

			@in.ReadFully(bytearr, 0, utflen);

			while (count < utflen)
			{
				c = (int) bytearr[count] & 0xff;
				if (c > 127)
				{
					break;
				}
				count++;
				chararr[chararr_count++] = (char)c;
			}

			while (count < utflen)
			{
				c = (int) bytearr[count] & 0xff;
				switch (c >> 4)
				{
					case 0:
				case 1:
			case 2:
		case 3:
	case 4:
	case 5:
	case 6:
	case 7:
						/* 0xxxxxxx*/
						count++;
						chararr[chararr_count++] = (char)c;
						break;
					case 12:
				case 13:
						/* 110x xxxx   10xx xxxx*/
						count += 2;
						if (count > utflen)
						{
							throw new UTFDataFormatException("malformed input: partial character at end");
						}
						char2 = (int) bytearr[count - 1];
						if ((char2 & 0xC0) != 0x80)
						{
							throw new UTFDataFormatException("malformed input around byte " + count);
						}
						chararr[chararr_count++] = (char)(((c & 0x1F) << 6) | (char2 & 0x3F));
						break;
					case 14:
						/* 1110 xxxx  10xx xxxx  10xx xxxx */
						count += 3;
						if (count > utflen)
						{
							throw new UTFDataFormatException("malformed input: partial character at end");
						}
						char2 = (int) bytearr[count - 2];
						char3 = (int) bytearr[count - 1];
						if (((char2 & 0xC0) != 0x80) || ((char3 & 0xC0) != 0x80))
						{
							throw new UTFDataFormatException("malformed input around byte " + (count - 1));
						}
						chararr[chararr_count++] = (char)(((c & 0x0F) << 12) | ((char2 & 0x3F) << 6) | ((char3 & 0x3F) << 0));
						break;
					default:
						/* 10xx xxxx,  1111 xxxx */
						throw new UTFDataFormatException("malformed input around byte " + count);
				}
			}
			// The number of chars produced may be less than utflen
			return new String(chararr, 0, chararr_count);
		}
	}

}