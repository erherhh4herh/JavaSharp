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

	using StreamDecoder = sun.nio.cs.StreamDecoder;


	/// <summary>
	/// An InputStreamReader is a bridge from byte streams to character streams: It
	/// reads bytes and decodes them into characters using a specified {@link
	/// java.nio.charset.Charset charset}.  The charset that it uses
	/// may be specified by name or may be given explicitly, or the platform's
	/// default charset may be accepted.
	/// 
	/// <para> Each invocation of one of an InputStreamReader's read() methods may
	/// cause one or more bytes to be read from the underlying byte-input stream.
	/// To enable the efficient conversion of bytes to characters, more bytes may
	/// be read ahead from the underlying stream than are necessary to satisfy the
	/// current read operation.
	/// 
	/// </para>
	/// <para> For top efficiency, consider wrapping an InputStreamReader within a
	/// BufferedReader.  For example:
	/// 
	/// <pre>
	/// BufferedReader in
	///   = new BufferedReader(new InputStreamReader(System.in));
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= BufferedReader </seealso>
	/// <seealso cref= InputStream </seealso>
	/// <seealso cref= java.nio.charset.Charset
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1 </seealso>

	public class InputStreamReader : Reader
	{

		private readonly StreamDecoder Sd;

		/// <summary>
		/// Creates an InputStreamReader that uses the default charset.
		/// </summary>
		/// <param name="in">   An InputStream </param>
		public InputStreamReader(InputStream @in) : base(@in)
		{
			try
			{
				Sd = StreamDecoder.forInputStreamReader(@in, this, (String)null); // ## check lock object
			}
			catch (UnsupportedEncodingException e)
			{
				// The default encoding should always be available
				throw new Error(e);
			}
		}

		/// <summary>
		/// Creates an InputStreamReader that uses the named charset.
		/// </summary>
		/// <param name="in">
		///         An InputStream
		/// </param>
		/// <param name="charsetName">
		///         The name of a supported
		///         <seealso cref="java.nio.charset.Charset charset"/>
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///             If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputStreamReader(InputStream in, String charsetName) throws UnsupportedEncodingException
		public InputStreamReader(InputStream @in, String charsetName) : base(@in)
		{
			if (charsetName == null)
			{
				throw new NullPointerException("charsetName");
			}
			Sd = StreamDecoder.forInputStreamReader(@in, this, charsetName);
		}

		/// <summary>
		/// Creates an InputStreamReader that uses the given charset.
		/// </summary>
		/// <param name="in">       An InputStream </param>
		/// <param name="cs">       A charset
		/// 
		/// @since 1.4
		/// @spec JSR-51 </param>
		public InputStreamReader(InputStream @in, Charset cs) : base(@in)
		{
			if (cs == null)
			{
				throw new NullPointerException("charset");
			}
			Sd = StreamDecoder.forInputStreamReader(@in, this, cs);
		}

		/// <summary>
		/// Creates an InputStreamReader that uses the given charset decoder.
		/// </summary>
		/// <param name="in">       An InputStream </param>
		/// <param name="dec">      A charset decoder
		/// 
		/// @since 1.4
		/// @spec JSR-51 </param>
		public InputStreamReader(InputStream @in, CharsetDecoder dec) : base(@in)
		{
			if (dec == null)
			{
				throw new NullPointerException("charset decoder");
			}
			Sd = StreamDecoder.forInputStreamReader(@in, this, dec);
		}

		/// <summary>
		/// Returns the name of the character encoding being used by this stream.
		/// 
		/// <para> If the encoding has an historical name then that name is returned;
		/// otherwise the encoding's canonical name is returned.
		/// 
		/// </para>
		/// <para> If this instance was created with the {@link
		/// #InputStreamReader(InputStream, String)} constructor then the returned
		/// name, being unique for the encoding, may differ from the name passed to
		/// the constructor. This method will return <code>null</code> if the
		/// stream has been closed.
		/// </para> </summary>
		/// <returns> The historical name of this encoding, or
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
				return Sd.Encoding;
			}
		}

		/// <summary>
		/// Reads a single character.
		/// </summary>
		/// <returns> The character read, or -1 if the end of the stream has been
		///         reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			return Sd.read();
		}

		/// <summary>
		/// Reads characters into a portion of an array.
		/// </summary>
		/// <param name="cbuf">     Destination buffer </param>
		/// <param name="offset">   Offset at which to start storing characters </param>
		/// <param name="length">   Maximum number of characters to read
		/// </param>
		/// <returns>     The number of characters read, or -1 if the end of the
		///             stream has been reached
		/// </returns>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char cbuf[] , int offset, int length) throws IOException
		public override int Read(char[] cbuf, int offset, int length)
		{
			return Sd.read(cbuf, offset, length);
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.  An InputStreamReader is
		/// ready if its input buffer is not empty, or if bytes are available to be
		/// read from the underlying byte stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			return Sd.ready();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			Sd.close();
		}
	}

}