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
	/// This class implements a character buffer that can be used as an Writer.
	/// The buffer automatically grows when data is written to the stream.  The data
	/// can be retrieved using toCharArray() and toString().
	/// <P>
	/// Note: Invoking close() on this class has no effect, and methods
	/// of this class can be called after the stream has closed
	/// without generating an IOException.
	/// 
	/// @author      Herb Jellinek
	/// @since       JDK1.1
	/// </summary>
	public class CharArrayWriter : Writer
	{
		/// <summary>
		/// The buffer where data is stored.
		/// </summary>
		protected internal char[] Buf;

		/// <summary>
		/// The number of chars in the buffer.
		/// </summary>
		protected internal int Count;

		/// <summary>
		/// Creates a new CharArrayWriter.
		/// </summary>
		public CharArrayWriter() : this(32)
		{
		}

		/// <summary>
		/// Creates a new CharArrayWriter with the specified initial size.
		/// </summary>
		/// <param name="initialSize">  an int specifying the initial buffer size. </param>
		/// <exception cref="IllegalArgumentException"> if initialSize is negative </exception>
		public CharArrayWriter(int initialSize)
		{
			if (initialSize < 0)
			{
				throw new IllegalArgumentException("Negative initial size: " + initialSize);
			}
			Buf = new char[initialSize];
		}

		/// <summary>
		/// Writes a character to the buffer.
		/// </summary>
		public override void Write(int c)
		{
			lock (@lock)
			{
				int newcount = Count + 1;
				if (newcount > Buf.Length)
				{
					Buf = Arrays.CopyOf(Buf, System.Math.Max(Buf.Length << 1, newcount));
				}
				Buf[Count] = (char)c;
				Count = newcount;
			}
		}

		/// <summary>
		/// Writes characters to the buffer. </summary>
		/// <param name="c"> the data to be written </param>
		/// <param name="off">       the start offset in the data </param>
		/// <param name="len">       the number of chars that are written </param>
		public override void Write(char[] c, int off, int len)
		{
			if ((off < 0) || (off > c.Length) || (len < 0) || ((off + len) > c.Length) || ((off + len) < 0))
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return;
			}
			lock (@lock)
			{
				int newcount = Count + len;
				if (newcount > Buf.Length)
				{
					Buf = Arrays.CopyOf(Buf, System.Math.Max(Buf.Length << 1, newcount));
				}
				System.Array.Copy(c, off, Buf, Count, len);
				Count = newcount;
			}
		}

		/// <summary>
		/// Write a portion of a string to the buffer. </summary>
		/// <param name="str">  String to be written from </param>
		/// <param name="off">  Offset from which to start reading characters </param>
		/// <param name="len">  Number of characters to be written </param>
		public override void Write(String str, int off, int len)
		{
			lock (@lock)
			{
				int newcount = Count + len;
				if (newcount > Buf.Length)
				{
					Buf = Arrays.CopyOf(Buf, System.Math.Max(Buf.Length << 1, newcount));
				}
				str.GetChars(off, off + len, Buf, Count);
				Count = newcount;
			}
		}

		/// <summary>
		/// Writes the contents of the buffer to another character stream.
		/// </summary>
		/// <param name="out">       the output stream to write to </param>
		/// <exception cref="IOException"> If an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeTo(Writer out) throws IOException
		public virtual void WriteTo(Writer @out)
		{
			lock (@lock)
			{
				@out.Write(Buf, 0, Count);
			}
		}

		/// <summary>
		/// Appends the specified character sequence to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(csq.toString()) </pre>
		/// 
		/// </para>
		/// <para> Depending on the specification of <tt>toString</tt> for the
		/// character sequence <tt>csq</tt>, the entire sequence may not be
		/// appended. For instance, invoking the <tt>toString</tt> method of a
		/// character buffer will return a subsequence whose content depends upon
		/// the buffer's position and limit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence to append.  If <tt>csq</tt> is
		///         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		///         appended to this writer.
		/// </param>
		/// <returns>  This writer
		/// 
		/// @since  1.5 </returns>
		public override CharArrayWriter Append(CharSequence csq)
		{
			String s = (csq == null ? "null" : csq.ToString());
			Write(s, 0, s.Length());
			return this;
		}

		/// <summary>
		/// Appends a subsequence of the specified character sequence to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq, start,
		/// end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		/// exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(csq.subSequence(start, end).toString()) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence from which a subsequence will be
		///         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
		///         will be appended as if <tt>csq</tt> contained the four
		///         characters <tt>"null"</tt>.
		/// </param>
		/// <param name="start">
		///         The index of the first character in the subsequence
		/// </param>
		/// <param name="end">
		///         The index of the character following the last character in the
		///         subsequence
		/// </param>
		/// <returns>  This writer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		///          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		///          <tt>csq.length()</tt>
		/// 
		/// @since  1.5 </exception>
		public override CharArrayWriter Append(CharSequence csq, int start, int end)
		{
			String s = (csq == null ? "null" : csq).subSequence(start, end).ToString();
			Write(s, 0, s.Length());
			return this;
		}

		/// <summary>
		/// Appends the specified character to this writer.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(c)</tt>
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.write(c) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">
		///         The 16-bit character to append
		/// </param>
		/// <returns>  This writer
		/// 
		/// @since 1.5 </returns>
		public override CharArrayWriter Append(char c)
		{
			Write(c);
			return this;
		}

		/// <summary>
		/// Resets the buffer so that you can use it again without
		/// throwing away the already allocated buffer.
		/// </summary>
		public virtual void Reset()
		{
			Count = 0;
		}

		/// <summary>
		/// Returns a copy of the input data.
		/// </summary>
		/// <returns> an array of chars copied from the input data. </returns>
		public virtual char ToCharArray()[] {synchronized(@lock) {return Arrays.copyOf(buf, count);}} public int size()
		/// <summary>
		/// Returns the current size of the buffer.
		/// </summary>
		/// <returns> an int representing the current size of the buffer. </returns>
		{
			lock (this)
			{
				return Count;
			}
		}

		/// <summary>
		/// Converts input data to a string. </summary>
		/// <returns> the string. </returns>
		public override String ToString()
		{
			lock (@lock)
			{
				return new String(Buf, 0, Count);
			}
		}

		/// <summary>
		/// Flush the stream.
		/// </summary>
		public override void Flush()
		{
		}

		/// <summary>
		/// Close the stream.  This method does not release the buffer, since its
		/// contents might still be required. Note: Invoking this method in this class
		/// will have no effect.
		/// </summary>
		public override void Close()
		{
		}

	}

}