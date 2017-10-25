/*
 * Copyright (c) 1994, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// A data output stream lets an application write primitive Java data
	/// types to an output stream in a portable way. An application can
	/// then use a data input stream to read the data back in.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.io.DataInputStream
	/// @since   JDK1.0 </seealso>
	public class DataOutputStream : FilterOutputStream, DataOutput
	{
		/// <summary>
		/// The number of bytes written to the data output stream so far.
		/// If this counter overflows, it will be wrapped to Integer.MAX_VALUE.
		/// </summary>
		protected internal int Written;

		/// <summary>
		/// bytearr is initialized on demand by writeUTF
		/// </summary>
		private sbyte[] Bytearr = null;

		/// <summary>
		/// Creates a new data output stream to write data to the specified
		/// underlying output stream. The counter <code>written</code> is
		/// set to zero.
		/// </summary>
		/// <param name="out">   the underlying output stream, to be saved for later
		///                use. </param>
		/// <seealso cref=     java.io.FilterOutputStream#out </seealso>
		public DataOutputStream(OutputStream @out) : base(@out)
		{
		}

		/// <summary>
		/// Increases the written counter by the specified value
		/// until it reaches Integer.MAX_VALUE.
		/// </summary>
		private void IncCount(int value)
		{
			int temp = Written + value;
			if (temp < 0)
			{
				temp = Integer.MaxValue;
			}
			Written = temp;
		}

		/// <summary>
		/// Writes the specified byte (the low eight bits of the argument
		/// <code>b</code>) to the underlying output stream. If no exception
		/// is thrown, the counter <code>written</code> is incremented by
		/// <code>1</code>.
		/// <para>
		/// Implements the <code>write</code> method of <code>OutputStream</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the <code>byte</code> to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(int b) throws IOException
		public override void Write(int b)
		{
			lock (this)
			{
				@out.Write(b);
				IncCount(1);
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to the underlying output stream.
		/// If no exception is thrown, the counter <code>written</code> is
		/// incremented by <code>len</code>.
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(byte b[] , int off, int len) throws IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				@out.Write(b, off, len);
				IncCount(len);
			}
		}

		/// <summary>
		/// Flushes this data output stream. This forces any buffered output
		/// bytes to be written out to the stream.
		/// <para>
		/// The <code>flush</code> method of <code>DataOutputStream</code>
		/// calls the <code>flush</code> method of its underlying output stream.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
		/// <seealso cref=        java.io.OutputStream#flush() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			@out.Flush();
		}

		/// <summary>
		/// Writes a <code>boolean</code> to the underlying output stream as
		/// a 1-byte value. The value <code>true</code> is written out as the
		/// value <code>(byte)1</code>; the value <code>false</code> is
		/// written out as the value <code>(byte)0</code>. If no exception is
		/// thrown, the counter <code>written</code> is incremented by
		/// <code>1</code>.
		/// </summary>
		/// <param name="v">   a <code>boolean</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeBoolean(boolean v) throws IOException
		public void WriteBoolean(bool v)
		{
			@out.Write(v ? 1 : 0);
			IncCount(1);
		}

		/// <summary>
		/// Writes out a <code>byte</code> to the underlying output stream as
		/// a 1-byte value. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>1</code>.
		/// </summary>
		/// <param name="v">   a <code>byte</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeByte(int v) throws IOException
		public void WriteByte(int v)
		{
			@out.Write(v);
			IncCount(1);
		}

		/// <summary>
		/// Writes a <code>short</code> to the underlying output stream as two
		/// bytes, high byte first. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>2</code>.
		/// </summary>
		/// <param name="v">   a <code>short</code> to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeShort(int v) throws IOException
		public void WriteShort(int v)
		{
			@out.Write(((int)((uint)v >> 8)) & 0xFF);
			@out.Write(((int)((uint)v >> 0)) & 0xFF);
			IncCount(2);
		}

		/// <summary>
		/// Writes a <code>char</code> to the underlying output stream as a
		/// 2-byte value, high byte first. If no exception is thrown, the
		/// counter <code>written</code> is incremented by <code>2</code>.
		/// </summary>
		/// <param name="v">   a <code>char</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeChar(int v) throws IOException
		public void WriteChar(int v)
		{
			@out.Write(((int)((uint)v >> 8)) & 0xFF);
			@out.Write(((int)((uint)v >> 0)) & 0xFF);
			IncCount(2);
		}

		/// <summary>
		/// Writes an <code>int</code> to the underlying output stream as four
		/// bytes, high byte first. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>4</code>.
		/// </summary>
		/// <param name="v">   an <code>int</code> to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeInt(int v) throws IOException
		public void WriteInt(int v)
		{
			@out.Write(((int)((uint)v >> 24)) & 0xFF);
			@out.Write(((int)((uint)v >> 16)) & 0xFF);
			@out.Write(((int)((uint)v >> 8)) & 0xFF);
			@out.Write(((int)((uint)v >> 0)) & 0xFF);
			IncCount(4);
		}

		private sbyte[] WriteBuffer = new sbyte[8];

		/// <summary>
		/// Writes a <code>long</code> to the underlying output stream as eight
		/// bytes, high byte first. In no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>8</code>.
		/// </summary>
		/// <param name="v">   a <code>long</code> to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeLong(long v) throws IOException
		public void WriteLong(long v)
		{
			WriteBuffer[0] = (sbyte)((long)((ulong)v >> 56));
			WriteBuffer[1] = (sbyte)((long)((ulong)v >> 48));
			WriteBuffer[2] = (sbyte)((long)((ulong)v >> 40));
			WriteBuffer[3] = (sbyte)((long)((ulong)v >> 32));
			WriteBuffer[4] = (sbyte)((long)((ulong)v >> 24));
			WriteBuffer[5] = (sbyte)((long)((ulong)v >> 16));
			WriteBuffer[6] = (sbyte)((long)((ulong)v >> 8));
			WriteBuffer[7] = (sbyte)((long)((ulong)v >> 0));
			@out.Write(WriteBuffer, 0, 8);
			IncCount(8);
		}

		/// <summary>
		/// Converts the float argument to an <code>int</code> using the
		/// <code>floatToIntBits</code> method in class <code>Float</code>,
		/// and then writes that <code>int</code> value to the underlying
		/// output stream as a 4-byte quantity, high byte first. If no
		/// exception is thrown, the counter <code>written</code> is
		/// incremented by <code>4</code>.
		/// </summary>
		/// <param name="v">   a <code>float</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
		/// <seealso cref=        java.lang.Float#floatToIntBits(float) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeFloat(float v) throws IOException
		public void WriteFloat(float v)
		{
			WriteInt(Float.FloatToIntBits(v));
		}

		/// <summary>
		/// Converts the double argument to a <code>long</code> using the
		/// <code>doubleToLongBits</code> method in class <code>Double</code>,
		/// and then writes that <code>long</code> value to the underlying
		/// output stream as an 8-byte quantity, high byte first. If no
		/// exception is thrown, the counter <code>written</code> is
		/// incremented by <code>8</code>.
		/// </summary>
		/// <param name="v">   a <code>double</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
		/// <seealso cref=        java.lang.Double#doubleToLongBits(double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeDouble(double v) throws IOException
		public void WriteDouble(double v)
		{
			WriteLong(Double.DoubleToLongBits(v));
		}

		/// <summary>
		/// Writes out the string to the underlying output stream as a
		/// sequence of bytes. Each character in the string is written out, in
		/// sequence, by discarding its high eight bits. If no exception is
		/// thrown, the counter <code>written</code> is incremented by the
		/// length of <code>s</code>.
		/// </summary>
		/// <param name="s">   a string of bytes to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeBytes(String s) throws IOException
		public void WriteBytes(String s)
		{
			int len = s.Length();
			for (int i = 0 ; i < len ; i++)
			{
				@out.Write((sbyte)s.CharAt(i));
			}
			IncCount(len);
		}

		/// <summary>
		/// Writes a string to the underlying output stream as a sequence of
		/// characters. Each character is written to the data output stream as
		/// if by the <code>writeChar</code> method. If no exception is
		/// thrown, the counter <code>written</code> is incremented by twice
		/// the length of <code>s</code>.
		/// </summary>
		/// <param name="s">   a <code>String</code> value to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.DataOutputStream#writeChar(int) </seealso>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeChars(String s) throws IOException
		public void WriteChars(String s)
		{
			int len = s.Length();
			for (int i = 0 ; i < len ; i++)
			{
				int v = s.CharAt(i);
				@out.Write(((int)((uint)v >> 8)) & 0xFF);
				@out.Write(((int)((uint)v >> 0)) & 0xFF);
			}
			IncCount(len * 2);
		}

		/// <summary>
		/// Writes a string to the underlying output stream using
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		/// encoding in a machine-independent manner.
		/// <para>
		/// First, two bytes are written to the output stream as if by the
		/// <code>writeShort</code> method giving the number of bytes to
		/// follow. This value is the number of bytes actually written out,
		/// not the length of the string. Following the length, each character
		/// of the string is output, in sequence, using the modified UTF-8 encoding
		/// for the character. If no exception is thrown, the counter
		/// <code>written</code> is incremented by the total number of
		/// bytes written to the output stream. This will be at least two
		/// plus the length of <code>str</code>, and at most two plus
		/// thrice the length of <code>str</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   a string to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void writeUTF(String str) throws IOException
		public void WriteUTF(String str)
		{
			WriteUTF(str, this);
		}

		/// <summary>
		/// Writes a string to the specified DataOutput using
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		/// encoding in a machine-independent manner.
		/// <para>
		/// First, two bytes are written to out as if by the <code>writeShort</code>
		/// method giving the number of bytes to follow. This value is the number of
		/// bytes actually written out, not the length of the string. Following the
		/// length, each character of the string is output, in sequence, using the
		/// modified UTF-8 encoding for the character. If no exception is thrown, the
		/// counter <code>written</code> is incremented by the total number of
		/// bytes written to the output stream. This will be at least two
		/// plus the length of <code>str</code>, and at most two plus
		/// thrice the length of <code>str</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   a string to be written. </param>
		/// <param name="out">   destination to write to </param>
		/// <returns>     The number of bytes written out. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static int writeUTF(String str, DataOutput out) throws IOException
		internal static int WriteUTF(String str, DataOutput @out)
		{
			int strlen = str.Length();
			int utflen = 0;
			int c , count = 0;

			/* use charAt instead of copying String to char array */
			for (int i = 0; i < strlen; i++)
			{
				c = str.CharAt(i);
				if ((c >= 0x0001) && (c <= 0x007F))
				{
					utflen++;
				}
				else if (c > 0x07FF)
				{
					utflen += 3;
				}
				else
				{
					utflen += 2;
				}
			}

			if (utflen > 65535)
			{
				throw new UTFDataFormatException("encoded string too long: " + utflen + " bytes");
			}

			sbyte[] bytearr = null;
			if (@out is DataOutputStream)
			{
				DataOutputStream dos = (DataOutputStream)@out;
				if (dos.Bytearr == null || (dos.Bytearr.Length < (utflen + 2)))
				{
					dos.Bytearr = new sbyte[(utflen * 2) + 2];
				}
				bytearr = dos.Bytearr;
			}
			else
			{
				bytearr = new sbyte[utflen + 2];
			}

			bytearr[count++] = unchecked((sbyte)(((int)((uint)utflen >> 8)) & 0xFF));
			bytearr[count++] = unchecked((sbyte)(((int)((uint)utflen >> 0)) & 0xFF));

			int i = 0;
			for (i = 0; i < strlen; i++)
			{
			   c = str.CharAt(i);
			   if (!((c >= 0x0001) && (c <= 0x007F)))
			   {
				   break;
			   }
			   bytearr[count++] = (sbyte) c;
			}

			for (;i < strlen; i++)
			{
				c = str.CharAt(i);
				if ((c >= 0x0001) && (c <= 0x007F))
				{
					bytearr[count++] = (sbyte) c;

				}
				else if (c > 0x07FF)
				{
					bytearr[count++] = unchecked((sbyte)(0xE0 | ((c >> 12) & 0x0F)));
					bytearr[count++] = unchecked((sbyte)(0x80 | ((c >> 6) & 0x3F)));
					bytearr[count++] = unchecked((sbyte)(0x80 | ((c >> 0) & 0x3F)));
				}
				else
				{
					bytearr[count++] = unchecked((sbyte)(0xC0 | ((c >> 6) & 0x1F)));
					bytearr[count++] = unchecked((sbyte)(0x80 | ((c >> 0) & 0x3F)));
				}
			}
			@out.Write(bytearr, 0, utflen + 2);
			return utflen + 2;
		}

		/// <summary>
		/// Returns the current value of the counter <code>written</code>,
		/// the number of bytes written to this data output stream so far.
		/// If the counter overflows, it will be wrapped to Integer.MAX_VALUE.
		/// </summary>
		/// <returns>  the value of the <code>written</code> field. </returns>
		/// <seealso cref=     java.io.DataOutputStream#written </seealso>
		public int Size()
		{
			return Written;
		}
	}

}