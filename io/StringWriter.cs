/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// A character stream that collects its output in a string buffer, which can
	/// then be used to construct a string.
	/// <para>
	/// Closing a <tt>StringWriter</tt> has no effect. The methods in this class
	/// can be called after the stream has been closed without generating an
	/// <tt>IOException</tt>.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </para>
	/// </summary>

	public class StringWriter : Writer
	{

		private StringBuffer Buf;

		/// <summary>
		/// Create a new string writer using the default initial string-buffer
		/// size.
		/// </summary>
		public StringWriter()
		{
			Buf = new StringBuffer();
			@lock = Buf;
		}

		/// <summary>
		/// Create a new string writer using the specified initial string-buffer
		/// size.
		/// </summary>
		/// <param name="initialSize">
		///        The number of <tt>char</tt> values that will fit into this buffer
		///        before it is automatically expanded
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///         If <tt>initialSize</tt> is negative </exception>
		public StringWriter(int initialSize)
		{
			if (initialSize < 0)
			{
				throw new IllegalArgumentException("Negative buffer size");
			}
			Buf = new StringBuffer(initialSize);
			@lock = Buf;
		}

		/// <summary>
		/// Write a single character.
		/// </summary>
		public override void Write(int c)
		{
			Buf.Append((char) c);
		}

		/// <summary>
		/// Write a portion of an array of characters.
		/// </summary>
		/// <param name="cbuf">  Array of characters </param>
		/// <param name="off">   Offset from which to start writing characters </param>
		/// <param name="len">   Number of characters to write </param>
		public override void Write(char[] cbuf, int off, int len)
		{
			if ((off < 0) || (off > cbuf.Length) || (len < 0) || ((off + len) > cbuf.Length) || ((off + len) < 0))
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return;
			}
			Buf.Append(cbuf, off, len);
		}

		/// <summary>
		/// Write a string.
		/// </summary>
		public override void Write(String str)
		{
			Buf.Append(str);
		}

		/// <summary>
		/// Write a portion of a string.
		/// </summary>
		/// <param name="str">  String to be written </param>
		/// <param name="off">  Offset from which to start writing characters </param>
		/// <param name="len">  Number of characters to write </param>
		public override void Write(String str, int off, int len)
		{
			Buf.Append(str.Substring(off, len));
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
		public override StringWriter Append(CharSequence csq)
		{
			if (csq == null)
			{
				Write("null");
			}
			else
			{
				Write(csq.ToString());
			}
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
		public override StringWriter Append(CharSequence csq, int start, int end)
		{
			CharSequence cs = (csq == null ? "null" : csq);
			Write(cs.SubSequence(start, end).ToString());
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
		public override StringWriter Append(char c)
		{
			Write(c);
			return this;
		}

		/// <summary>
		/// Return the buffer's current value as a string.
		/// </summary>
		public override String ToString()
		{
			return Buf.ToString();
		}

		/// <summary>
		/// Return the string buffer itself.
		/// </summary>
		/// <returns> StringBuffer holding the current buffer value. </returns>
		public virtual StringBuffer Buffer
		{
			get
			{
				return Buf;
			}
		}

		/// <summary>
		/// Flush the stream.
		/// </summary>
		public override void Flush()
		{
		}

		/// <summary>
		/// Closing a <tt>StringWriter</tt> has no effect. The methods in this
		/// class can be called after the stream has been closed without generating
		/// an <tt>IOException</tt>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
		}

	}

}