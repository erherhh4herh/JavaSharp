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
	/// Abstract class for writing to character streams.  The only methods that a
	/// subclass must implement are write(char[], int, int), flush(), and close().
	/// Most subclasses, however, will override some of the methods defined here in
	/// order to provide higher efficiency, additional functionality, or both.
	/// </summary>
	/// <seealso cref= Writer </seealso>
	/// <seealso cref=   BufferedWriter </seealso>
	/// <seealso cref=   CharArrayWriter </seealso>
	/// <seealso cref=   FilterWriter </seealso>
	/// <seealso cref=   OutputStreamWriter </seealso>
	/// <seealso cref=     FileWriter </seealso>
	/// <seealso cref=   PipedWriter </seealso>
	/// <seealso cref=   PrintWriter </seealso>
	/// <seealso cref=   StringWriter </seealso>
	/// <seealso cref= Reader
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public abstract class Writer : Appendable, Closeable, Flushable
	{

		/// <summary>
		/// Temporary buffer used to hold writes of strings and single characters
		/// </summary>
		private char[] WriteBuffer;

		/// <summary>
		/// Size of writeBuffer, must be >= 1
		/// </summary>
		private const int WRITE_BUFFER_SIZE = 1024;

		/// <summary>
		/// The object used to synchronize operations on this stream.  For
		/// efficiency, a character-stream object may use an object other than
		/// itself to protect critical sections.  A subclass should therefore use
		/// the object in this field rather than <tt>this</tt> or a synchronized
		/// method.
		/// </summary>
		protected internal Object @lock;

		/// <summary>
		/// Creates a new character-stream writer whose critical sections will
		/// synchronize on the writer itself.
		/// </summary>
		protected internal Writer()
		{
			this.@lock = this;
		}

		/// <summary>
		/// Creates a new character-stream writer whose critical sections will
		/// synchronize on the given object.
		/// </summary>
		/// <param name="lock">
		///         Object to synchronize on </param>
		protected internal Writer(Object @lock)
		{
			if (@lock == null)
			{
				throw new NullPointerException();
			}
			this.@lock = @lock;
		}

		/// <summary>
		/// Writes a single character.  The character to be written is contained in
		/// the 16 low-order bits of the given integer value; the 16 high-order bits
		/// are ignored.
		/// 
		/// <para> Subclasses that intend to support efficient single-character output
		/// should override this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">
		///         int specifying a character to be written
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws IOException
		public virtual void Write(int c)
		{
			lock (@lock)
			{
				if (WriteBuffer == null)
				{
					WriteBuffer = new char[WRITE_BUFFER_SIZE];
				}
				WriteBuffer[0] = (char) c;
				Write(WriteBuffer, 0, 1);
			}
		}

		/// <summary>
		/// Writes an array of characters.
		/// </summary>
		/// <param name="cbuf">
		///         Array of characters to be written
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char cbuf[]) throws IOException
		public virtual void Write(char[] cbuf)
		{
			Write(cbuf, 0, cbuf.Length);
		}

		/// <summary>
		/// Writes a portion of an array of characters.
		/// </summary>
		/// <param name="cbuf">
		///         Array of characters
		/// </param>
		/// <param name="off">
		///         Offset from which to start writing characters
		/// </param>
		/// <param name="len">
		///         Number of characters to write
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void write(char cbuf[] , int off, int len) throws IOException;
		public abstract void Write(char[] cbuf, int off, int len);

		/// <summary>
		/// Writes a string.
		/// </summary>
		/// <param name="str">
		///         String to be written
		/// </param>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String str) throws IOException
		public virtual void Write(String str)
		{
			Write(str, 0, str.Length());
		}

		/// <summary>
		/// Writes a portion of a string.
		/// </summary>
		/// <param name="str">
		///         A String
		/// </param>
		/// <param name="off">
		///         Offset from which to start writing characters
		/// </param>
		/// <param name="len">
		///         Number of characters to write
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>off</tt> is negative, or <tt>len</tt> is negative,
		///          or <tt>off+len</tt> is negative or greater than the length
		///          of the given string
		/// </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String str, int off, int len) throws IOException
		public virtual void Write(String str, int off, int len)
		{
			lock (@lock)
			{
				char[] cbuf;
				if (len <= WRITE_BUFFER_SIZE)
				{
					if (WriteBuffer == null)
					{
						WriteBuffer = new char[WRITE_BUFFER_SIZE];
					}
					cbuf = WriteBuffer;
				} // Don't permanently allocate very large buffers.
				else
				{
					cbuf = new char[len];
				}
				str.GetChars(off, (off + len), cbuf, 0);
				Write(cbuf, 0, len);
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
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Writer append(CharSequence csq) throws IOException
		public virtual Writer Append(CharSequence csq)
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
		/// <tt>Appendable</tt>.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq, start,
		/// end)</tt> when <tt>csq</tt> is not <tt>null</tt> behaves in exactly the
		/// same way as the invocation
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
		/// </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since  1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Writer append(CharSequence csq, int start, int end) throws IOException
		public virtual Writer Append(CharSequence csq, int start, int end)
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
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Writer append(char c) throws IOException
		public virtual Writer Append(char c)
		{
			Write(c);
			return this;
		}

		/// <summary>
		/// Flushes the stream.  If the stream has saved any characters from the
		/// various write() methods in a buffer, write them immediately to their
		/// intended destination.  Then, if that destination is another character or
		/// byte stream, flush it.  Thus one flush() invocation will flush all the
		/// buffers in a chain of Writers and OutputStreams.
		/// 
		/// <para> If the intended destination of this stream is an abstraction provided
		/// by the underlying operating system, for example a file, then flushing the
		/// stream guarantees only that bytes previously written to the stream are
		/// passed to the operating system for writing; it does not guarantee that
		/// they are actually written to a physical device such as a disk drive.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void flush() throws IOException;
		public abstract void Flush();

		/// <summary>
		/// Closes the stream, flushing it first. Once the stream has been closed,
		/// further write() or flush() invocations will cause an IOException to be
		/// thrown. Closing a previously closed stream has no effect.
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void close() throws IOException;
		public abstract void Close();

	}

}