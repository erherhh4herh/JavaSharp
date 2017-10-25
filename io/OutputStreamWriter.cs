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

	using StreamEncoder = sun.nio.cs.StreamEncoder;


	/// <summary>
	/// An OutputStreamWriter is a bridge from character streams to byte streams:
	/// Characters written to it are encoded into bytes using a specified {@link
	/// java.nio.charset.Charset charset}.  The charset that it uses
	/// may be specified by name or may be given explicitly, or the platform's
	/// default charset may be accepted.
	/// 
	/// <para> Each invocation of a write() method causes the encoding converter to be
	/// invoked on the given character(s).  The resulting bytes are accumulated in a
	/// buffer before being written to the underlying output stream.  The size of
	/// this buffer may be specified, but by default it is large enough for most
	/// purposes.  Note that the characters passed to the write() methods are not
	/// buffered.
	/// 
	/// </para>
	/// <para> For top efficiency, consider wrapping an OutputStreamWriter within a
	/// BufferedWriter so as to avoid frequent converter invocations.  For example:
	/// 
	/// <pre>
	/// Writer out
	///   = new BufferedWriter(new OutputStreamWriter(System.out));
	/// </pre>
	/// 
	/// </para>
	/// <para> A <i>surrogate pair</i> is a character represented by a sequence of two
	/// <tt>char</tt> values: A <i>high</i> surrogate in the range '&#92;uD800' to
	/// '&#92;uDBFF' followed by a <i>low</i> surrogate in the range '&#92;uDC00' to
	/// '&#92;uDFFF'.
	/// 
	/// </para>
	/// <para> A <i>malformed surrogate element</i> is a high surrogate that is not
	/// followed by a low surrogate or a low surrogate that is not preceded by a
	/// high surrogate.
	/// 
	/// </para>
	/// <para> This class always replaces malformed surrogate elements and unmappable
	/// character sequences with the charset's default <i>substitution sequence</i>.
	/// The <seealso cref="java.nio.charset.CharsetEncoder"/> class should be used when more
	/// control over the encoding process is required.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= BufferedWriter </seealso>
	/// <seealso cref= OutputStream </seealso>
	/// <seealso cref= java.nio.charset.Charset
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public class OutputStreamWriter : Writer
	{

		private readonly StreamEncoder Se;

		/// <summary>
		/// Creates an OutputStreamWriter that uses the named charset.
		/// </summary>
		/// <param name="out">
		///         An OutputStream
		/// </param>
		/// <param name="charsetName">
		///         The name of a supported
		///         <seealso cref="java.nio.charset.Charset charset"/>
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///             If the named encoding is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputStreamWriter(OutputStream out, String charsetName) throws UnsupportedEncodingException
		public OutputStreamWriter(OutputStream @out, String charsetName) : base(@out)
		{
			if (charsetName == null)
			{
				throw new NullPointerException("charsetName");
			}
			Se = StreamEncoder.forOutputStreamWriter(@out, this, charsetName);
		}

		/// <summary>
		/// Creates an OutputStreamWriter that uses the default character encoding.
		/// </summary>
		/// <param name="out">  An OutputStream </param>
		public OutputStreamWriter(OutputStream @out) : base(@out)
		{
			try
			{
				Se = StreamEncoder.forOutputStreamWriter(@out, this, (String)null);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new Error(e);
			}
		}

		/// <summary>
		/// Creates an OutputStreamWriter that uses the given charset.
		/// </summary>
		/// <param name="out">
		///         An OutputStream
		/// </param>
		/// <param name="cs">
		///         A charset
		/// 
		/// @since 1.4
		/// @spec JSR-51 </param>
		public OutputStreamWriter(OutputStream @out, Charset cs) : base(@out)
		{
			if (cs == null)
			{
				throw new NullPointerException("charset");
			}
			Se = StreamEncoder.forOutputStreamWriter(@out, this, cs);
		}

		/// <summary>
		/// Creates an OutputStreamWriter that uses the given charset encoder.
		/// </summary>
		/// <param name="out">
		///         An OutputStream
		/// </param>
		/// <param name="enc">
		///         A charset encoder
		/// 
		/// @since 1.4
		/// @spec JSR-51 </param>
		public OutputStreamWriter(OutputStream @out, CharsetEncoder enc) : base(@out)
		{
			if (enc == null)
			{
				throw new NullPointerException("charset encoder");
			}
			Se = StreamEncoder.forOutputStreamWriter(@out, this, enc);
		}

		/// <summary>
		/// Returns the name of the character encoding being used by this stream.
		/// 
		/// <para> If the encoding has an historical name then that name is returned;
		/// otherwise the encoding's canonical name is returned.
		/// 
		/// </para>
		/// <para> If this instance was created with the {@link
		/// #OutputStreamWriter(OutputStream, String)} constructor then the returned
		/// name, being unique for the encoding, may differ from the name passed to
		/// the constructor.  This method may return <tt>null</tt> if the stream has
		/// been closed. </para>
		/// </summary>
		/// <returns> The historical name of this encoding, or possibly
		///         <code>null</code> if the stream has been closed
		/// </returns>
		/// <seealso cref= java.nio.charset.Charset
		/// 
		/// @revised 1.4
		/// @spec JSR-51 </seealso>
		public virtual String Encoding
		{
			get
			{
				return Se.Encoding;
			}
		}

		/// <summary>
		/// Flushes the output buffer to the underlying byte stream, without flushing
		/// the byte stream itself.  This method is non-private only so that it may
		/// be invoked by PrintStream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void flushBuffer() throws IOException
		internal virtual void FlushBuffer()
		{
			Se.flushBuffer();
		}

		/// <summary>
		/// Writes a single character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws IOException
		public override void Write(int c)
		{
			Se.write(c);
		}

		/// <summary>
		/// Writes a portion of an array of characters.
		/// </summary>
		/// <param name="cbuf">  Buffer of characters </param>
		/// <param name="off">   Offset from which to start writing characters </param>
		/// <param name="len">   Number of characters to write
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char cbuf[] , int off, int len) throws IOException
		public override void Write(char[] cbuf, int off, int len)
		{
			Se.write(cbuf, off, len);
		}

		/// <summary>
		/// Writes a portion of a string.
		/// </summary>
		/// <param name="str">  A String </param>
		/// <param name="off">  Offset from which to start writing characters </param>
		/// <param name="len">  Number of characters to write
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String str, int off, int len) throws IOException
		public override void Write(String str, int off, int len)
		{
			Se.write(str, off, len);
		}

		/// <summary>
		/// Flushes the stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			Se.flush();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			Se.close();
		}
	}

}