using System;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements an output stream in which the data is
	/// written into a byte array. The buffer automatically grows as data
	/// is written to it.
	/// The data can be retrieved using <code>toByteArray()</code> and
	/// <code>toString()</code>.
	/// <para>
	/// Closing a <tt>ByteArrayOutputStream</tt> has no effect. The methods in
	/// this class can be called after the stream has been closed without
	/// generating an <tt>IOException</tt>.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </para>
	/// </summary>

	public class ByteArrayOutputStream : OutputStream
	{

		/// <summary>
		/// The buffer where data is stored.
		/// </summary>
		protected internal sbyte[] Buf;

		/// <summary>
		/// The number of valid bytes in the buffer.
		/// </summary>
		protected internal int Count;

		/// <summary>
		/// Creates a new byte array output stream. The buffer capacity is
		/// initially 32 bytes, though its size increases if necessary.
		/// </summary>
		public ByteArrayOutputStream() : this(32)
		{
		}

		/// <summary>
		/// Creates a new byte array output stream, with a buffer capacity of
		/// the specified size, in bytes.
		/// </summary>
		/// <param name="size">   the initial size. </param>
		/// <exception cref="IllegalArgumentException"> if size is negative. </exception>
		public ByteArrayOutputStream(int size)
		{
			if (size < 0)
			{
				throw new IllegalArgumentException("Negative initial size: " + size);
			}
			Buf = new sbyte[size];
		}

		/// <summary>
		/// Increases the capacity if necessary to ensure that it can hold
		/// at least the number of elements specified by the minimum
		/// capacity argument.
		/// </summary>
		/// <param name="minCapacity"> the desired minimum capacity </param>
		/// <exception cref="OutOfMemoryError"> if {@code minCapacity < 0}.  This is
		/// interpreted as a request for the unsatisfiably large capacity
		/// {@code (long) Integer.MAX_VALUE + (minCapacity - Integer.MAX_VALUE)}. </exception>
		private void EnsureCapacity(int minCapacity)
		{
			// overflow-conscious code
			if (minCapacity - Buf.Length > 0)
			{
				Grow(minCapacity);
			}
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Increases the capacity to ensure that it can hold at least the
		/// number of elements specified by the minimum capacity argument.
		/// </summary>
		/// <param name="minCapacity"> the desired minimum capacity </param>
		private void Grow(int minCapacity)
		{
			// overflow-conscious code
			int oldCapacity = Buf.Length;
			int newCapacity = oldCapacity << 1;
			if (newCapacity - minCapacity < 0)
			{
				newCapacity = minCapacity;
			}
			if (newCapacity - MAX_ARRAY_SIZE > 0)
			{
				newCapacity = HugeCapacity(minCapacity);
			}
			Buf = Arrays.CopyOf(Buf, newCapacity);
		}

		private static int HugeCapacity(int minCapacity)
		{
			if (minCapacity < 0) // overflow
			{
				throw new OutOfMemoryError();
			}
			return (minCapacity > MAX_ARRAY_SIZE) ? Integer.MaxValue : MAX_ARRAY_SIZE;
		}

		/// <summary>
		/// Writes the specified byte to this byte array output stream.
		/// </summary>
		/// <param name="b">   the byte to be written. </param>
		public override void Write(int b)
		{
			lock (this)
			{
				EnsureCapacity(Count + 1);
				Buf[Count] = (sbyte) b;
				Count += 1;
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to this byte array output stream.
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		public override void Write(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) - b.Length > 0))
				{
					throw new IndexOutOfBoundsException();
				}
				EnsureCapacity(Count + len);
				System.Array.Copy(b, off, Buf, Count, len);
				Count += len;
			}
		}

		/// <summary>
		/// Writes the complete contents of this byte array output stream to
		/// the specified output stream argument, as if by calling the output
		/// stream's write method using <code>out.write(buf, 0, count)</code>.
		/// </summary>
		/// <param name="out">   the output stream to which to write the data. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void writeTo(OutputStream out) throws IOException
		public virtual void WriteTo(OutputStream @out)
		{
			lock (this)
			{
				@out.Write(Buf, 0, Count);
			}
		}

		/// <summary>
		/// Resets the <code>count</code> field of this byte array output
		/// stream to zero, so that all currently accumulated output in the
		/// output stream is discarded. The output stream can be used again,
		/// reusing the already allocated buffer space.
		/// </summary>
		/// <seealso cref=     java.io.ByteArrayInputStream#count </seealso>
		public virtual void Reset()
		{
			lock (this)
			{
				Count = 0;
			}
		}

		/// <summary>
		/// Creates a newly allocated byte array. Its size is the current
		/// size of this output stream and the valid contents of the buffer
		/// have been copied into it.
		/// </summary>
		/// <returns>  the current contents of this output stream, as a byte array. </returns>
		/// <seealso cref=     java.io.ByteArrayOutputStream#size() </seealso>
		public virtual sbyte ToByteArray()[] {return Arrays.copyOf(buf, count);} public synchronized int size()
		/// <summary>
		/// Returns the current size of the buffer.
		/// </summary>
		/// <returns>  the value of the <code>count</code> field, which is the number
		///          of valid bytes in this output stream. </returns>
		/// <seealso cref=     java.io.ByteArrayOutputStream#count </seealso>
		{
			lock (this)
			{
				return Count;
			}
		}

		/// <summary>
		/// Converts the buffer's contents into a string decoding bytes using the
		/// platform's default character set. The length of the new <tt>String</tt>
		/// is a function of the character set, and hence may not be equal to the
		/// size of the buffer.
		/// 
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with the default replacement string for the platform's
		/// default character set. The <seealso cref="java.nio.charset.CharsetDecoder"/>
		/// class should be used when more control over the decoding process is
		/// required.
		/// 
		/// </para>
		/// </summary>
		/// <returns> String decoded from the buffer's contents.
		/// @since  JDK1.1 </returns>
		public override String ToString()
		{
			lock (this)
			{
				return StringHelperClass.NewString(Buf, 0, Count);
			}
		}

		/// <summary>
		/// Converts the buffer's contents into a string by decoding the bytes using
		/// the named <seealso cref="java.nio.charset.Charset charset"/>. The length of the new
		/// <tt>String</tt> is a function of the charset, and hence may not be equal
		/// to the length of the byte array.
		/// 
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement string. The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="charsetName">  the name of a supported
		///             <seealso cref="java.nio.charset.Charset charset"/> </param>
		/// <returns>     String decoded from the buffer's contents. </returns>
		/// <exception cref="UnsupportedEncodingException">
		///             If the named charset is not supported
		/// @since      JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized String toString(String charsetName) throws UnsupportedEncodingException
		public virtual String ToString(String charsetName)
		{
			lock (this)
			{
				return StringHelperClass.NewString(Buf, 0, Count, charsetName);
			}
		}

		/// <summary>
		/// Creates a newly allocated string. Its size is the current size of
		/// the output stream and the valid contents of the buffer have been
		/// copied into it. Each character <i>c</i> in the resulting string is
		/// constructed from the corresponding element <i>b</i> in the byte
		/// array such that:
		/// <blockquote><pre>
		///     c == (char)(((hibyte &amp; 0xff) &lt;&lt; 8) | (b &amp; 0xff))
		/// </pre></blockquote>
		/// </summary>
		/// @deprecated This method does not properly convert bytes into characters.
		/// As of JDK&nbsp;1.1, the preferred way to do this is via the
		/// <code>toString(String enc)</code> method, which takes an encoding-name
		/// argument, or the <code>toString()</code> method, which uses the
		/// platform's default character encoding.
		/// 
		/// <param name="hibyte">    the high byte of each resulting Unicode character. </param>
		/// <returns>     the current contents of the output stream, as a string. </returns>
		/// <seealso cref=        java.io.ByteArrayOutputStream#size() </seealso>
		/// <seealso cref=        java.io.ByteArrayOutputStream#toString(String) </seealso>
		/// <seealso cref=        java.io.ByteArrayOutputStream#toString() </seealso>
		[Obsolete("This method does not properly convert bytes into characters.")]
		public virtual String ToString(int hibyte)
		{
			lock (this)
			{
				return StringHelperClass.NewString(Buf, hibyte, 0, Count);
			}
		}

		/// <summary>
		/// Closing a <tt>ByteArrayOutputStream</tt> has no effect. The methods in
		/// this class can be called after the stream has been closed without
		/// generating an <tt>IOException</tt>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
		}

	}

}