using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{


	/// <summary>
	/// This class consists exclusively of static methods for obtaining
	/// encoders and decoders for the Base64 encoding scheme. The
	/// implementation of this class supports the following types of Base64
	/// as specified in
	/// <a href="http://www.ietf.org/rfc/rfc4648.txt">RFC 4648</a> and
	/// <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>.
	/// 
	/// <ul>
	/// <li><a name="basic"><b>Basic</b></a>
	/// <para> Uses "The Base64 Alphabet" as specified in Table 1 of
	///     RFC 4648 and RFC 2045 for encoding and decoding operation.
	///     The encoder does not add any line feed (line separator)
	///     character. The decoder rejects data that contains characters
	///     outside the base64 alphabet.</para></li>
	/// 
	/// <li><a name="url"><b>URL and Filename safe</b></a>
	/// <para> Uses the "URL and Filename safe Base64 Alphabet" as specified
	///     in Table 2 of RFC 4648 for encoding and decoding. The
	///     encoder does not add any line feed (line separator) character.
	///     The decoder rejects data that contains characters outside the
	///     base64 alphabet.</para></li>
	/// 
	/// <li><a name="mime"><b>MIME</b></a>
	/// <para> Uses the "The Base64 Alphabet" as specified in Table 1 of
	///     RFC 2045 for encoding and decoding operation. The encoded output
	///     must be represented in lines of no more than 76 characters each
	///     and uses a carriage return {@code '\r'} followed immediately by
	///     a linefeed {@code '\n'} as the line separator. No line separator
	///     is added to the end of the encoded output. All line separators
	///     or other characters not found in the base64 alphabet table are
	///     ignored in decoding operation.</para></li>
	/// </ul>
	/// 
	/// <para> Unless otherwise noted, passing a {@code null} argument to a
	/// method of this class will cause a {@link java.lang.NullPointerException
	/// NullPointerException} to be thrown.
	/// 
	/// @author  Xueming Shen
	/// @since   1.8
	/// </para>
	/// </summary>

	public class Base64
	{

		private Base64()
		{
		}

		/// <summary>
		/// Returns a <seealso cref="Encoder"/> that encodes using the
		/// <a href="#basic">Basic</a> type base64 encoding scheme.
		/// </summary>
		/// <returns>  A Base64 encoder. </returns>
		public static Encoder Encoder
		{
			get
			{
				 return Encoder.RFC4648;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Encoder"/> that encodes using the
		/// <a href="#url">URL and Filename safe</a> type base64
		/// encoding scheme.
		/// </summary>
		/// <returns>  A Base64 encoder. </returns>
		public static Encoder UrlEncoder
		{
			get
			{
				 return Encoder.RFC4648_URLSAFE;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Encoder"/> that encodes using the
		/// <a href="#mime">MIME</a> type base64 encoding scheme.
		/// </summary>
		/// <returns>  A Base64 encoder. </returns>
		public static Encoder MimeEncoder
		{
			get
			{
				return Encoder.RFC2045;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Encoder"/> that encodes using the
		/// <a href="#mime">MIME</a> type base64 encoding scheme
		/// with specified line length and line separators.
		/// </summary>
		/// <param name="lineLength">
		///          the length of each output line (rounded down to nearest multiple
		///          of 4). If {@code lineLength <= 0} the output will not be separated
		///          in lines </param>
		/// <param name="lineSeparator">
		///          the line separator for each output line
		/// </param>
		/// <returns>  A Base64 encoder.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code lineSeparator} includes any
		///          character of "The Base64 Alphabet" as specified in Table 1 of
		///          RFC 2045. </exception>
		public static Encoder GetMimeEncoder(int lineLength, sbyte[] lineSeparator)
		{
			 Objects.RequireNonNull(lineSeparator);
			 int[] base64 = Decoder.FromBase64;
			 foreach (sbyte b in lineSeparator)
			 {
				 if (base64[b & 0xff] != -1)
				 {
					 throw new IllegalArgumentException("Illegal base64 line separator character 0x" + Convert.ToString(b, 16));
				 }
			 }
			 if (lineLength <= 0)
			 {
				 return Encoder.RFC4648;
			 }
			 return new Encoder(false, lineSeparator, lineLength >> 2 << 2, true);
		}

		/// <summary>
		/// Returns a <seealso cref="Decoder"/> that decodes using the
		/// <a href="#basic">Basic</a> type base64 encoding scheme.
		/// </summary>
		/// <returns>  A Base64 decoder. </returns>
		public static Decoder Decoder
		{
			get
			{
				 return Decoder.RFC4648;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Decoder"/> that decodes using the
		/// <a href="#url">URL and Filename safe</a> type base64
		/// encoding scheme.
		/// </summary>
		/// <returns>  A Base64 decoder. </returns>
		public static Decoder UrlDecoder
		{
			get
			{
				 return Decoder.RFC4648_URLSAFE;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Decoder"/> that decodes using the
		/// <a href="#mime">MIME</a> type base64 decoding scheme.
		/// </summary>
		/// <returns>  A Base64 decoder. </returns>
		public static Decoder MimeDecoder
		{
			get
			{
				 return Decoder.RFC2045;
			}
		}

		/// <summary>
		/// This class implements an encoder for encoding byte data using
		/// the Base64 encoding scheme as specified in RFC 4648 and RFC 2045.
		/// 
		/// <para> Instances of <seealso cref="Encoder"/> class are safe for use by
		/// multiple concurrent threads.
		/// 
		/// </para>
		/// <para> Unless otherwise noted, passing a {@code null} argument to
		/// a method of this class will cause a
		/// <seealso cref="java.lang.NullPointerException NullPointerException"/> to
		/// be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     Decoder
		/// @since   1.8 </seealso>
		public class Encoder
		{

			internal readonly sbyte[] Newline;
			internal readonly int Linemax;
			internal readonly bool IsURL;
			internal readonly bool DoPadding;

			internal Encoder(bool isURL, sbyte[] newline, int linemax, bool doPadding)
			{
				this.IsURL = isURL;
				this.Newline = newline;
				this.Linemax = linemax;
				this.DoPadding = doPadding;
			}

			/// <summary>
			/// This array is a lookup table that translates 6-bit positive integer
			/// index values into their "Base64 Alphabet" equivalents as specified
			/// in "Table 1: The Base64 Alphabet" of RFC 2045 (and RFC 4648).
			/// </summary>
			internal static readonly char[] ToBase64 = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'};

			/// <summary>
			/// It's the lookup table for "URL and Filename safe Base64" as specified
			/// in Table 2 of the RFC 4648, with the '+' and '/' changed to '-' and
			/// '_'. This table is used when BASE64_URL is specified.
			/// </summary>
			internal static readonly char[] ToBase64URL = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

			internal const int MIMELINEMAX = 76;
			internal static readonly sbyte[] CRLF = new sbyte[] {(sbyte)'\r', (sbyte)'\n'};

			internal static readonly Encoder RFC4648 = new Encoder(false, null, -1, true);
			internal static readonly Encoder RFC4648_URLSAFE = new Encoder(true, null, -1, true);
			internal static readonly Encoder RFC2045 = new Encoder(false, CRLF, MIMELINEMAX, true);

			internal int OutLength(int srclen)
			{
				int len = 0;
				if (DoPadding)
				{
					len = 4 * ((srclen + 2) / 3);
				}
				else
				{
					int n = srclen % 3;
					len = 4 * (srclen / 3) + (n == 0 ? 0 : n + 1);
				}
				if (Linemax > 0) // line separators
				{
					len += (len - 1) / Linemax * Newline.Length;
				}
				return len;
			}

			/// <summary>
			/// Encodes all bytes from the specified byte array into a newly-allocated
			/// byte array using the <seealso cref="Base64"/> encoding scheme. The returned byte
			/// array is of the length of the resulting bytes.
			/// </summary>
			/// <param name="src">
			///          the byte array to encode </param>
			/// <returns>  A newly-allocated byte array containing the resulting
			///          encoded bytes. </returns>
			public virtual sbyte[] Encode(sbyte[] src)
			{
				int len = OutLength(src.Length); // dst array size
				sbyte[] dst = new sbyte[len];
				int ret = Encode0(src, 0, src.Length, dst);
				if (ret != dst.Length)
				{
					 return Arrays.CopyOf(dst, ret);
				}
				return dst;
			}

			/// <summary>
			/// Encodes all bytes from the specified byte array using the
			/// <seealso cref="Base64"/> encoding scheme, writing the resulting bytes to the
			/// given output byte array, starting at offset 0.
			/// 
			/// <para> It is the responsibility of the invoker of this method to make
			/// sure the output byte array {@code dst} has enough space for encoding
			/// all bytes from the input byte array. No bytes will be written to the
			/// output byte array if the output byte array is not big enough.
			/// 
			/// </para>
			/// </summary>
			/// <param name="src">
			///          the byte array to encode </param>
			/// <param name="dst">
			///          the output byte array </param>
			/// <returns>  The number of bytes written to the output byte array
			/// </returns>
			/// <exception cref="IllegalArgumentException"> if {@code dst} does not have enough
			///          space for encoding all input bytes. </exception>
			public virtual int Encode(sbyte[] src, sbyte[] dst)
			{
				int len = OutLength(src.Length); // dst array size
				if (dst.Length < len)
				{
					throw new IllegalArgumentException("Output byte array is too small for encoding all input bytes");
				}
				return Encode0(src, 0, src.Length, dst);
			}

			/// <summary>
			/// Encodes the specified byte array into a String using the <seealso cref="Base64"/>
			/// encoding scheme.
			/// 
			/// <para> This method first encodes all input bytes into a base64 encoded
			/// byte array and then constructs a new String by using the encoded byte
			/// array and the {@link java.nio.charset.StandardCharsets#ISO_8859_1
			/// ISO-8859-1} charset.
			/// 
			/// </para>
			/// <para> In other words, an invocation of this method has exactly the same
			/// effect as invoking
			/// {@code new String(encode(src), StandardCharsets.ISO_8859_1)}.
			/// 
			/// </para>
			/// </summary>
			/// <param name="src">
			///          the byte array to encode </param>
			/// <returns>  A String containing the resulting Base64 encoded characters </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public String encodeToString(byte[] src)
			public virtual String EncodeToString(sbyte[] src)
			{
				sbyte[] encoded = Encode(src);
				return StringHelperClass.NewString(encoded, 0, 0, encoded.Length);
			}

			/// <summary>
			/// Encodes all remaining bytes from the specified byte buffer into
			/// a newly-allocated ByteBuffer using the <seealso cref="Base64"/> encoding
			/// scheme.
			/// 
			/// Upon return, the source buffer's position will be updated to
			/// its limit; its limit will not have been changed. The returned
			/// output buffer's position will be zero and its limit will be the
			/// number of resulting encoded bytes.
			/// </summary>
			/// <param name="buffer">
			///          the source ByteBuffer to encode </param>
			/// <returns>  A newly-allocated byte buffer containing the encoded bytes. </returns>
			public virtual ByteBuffer Encode(ByteBuffer buffer)
			{
				int len = OutLength(buffer.Remaining());
				sbyte[] dst = new sbyte[len];
				int ret = 0;
				if (buffer.HasArray())
				{
					ret = Encode0(buffer.Array(), buffer.ArrayOffset() + buffer.Position(), buffer.ArrayOffset() + buffer.Limit(), dst);
					buffer.Position(buffer.Limit());
				}
				else
				{
					sbyte[] src = new sbyte[buffer.Remaining()];
					buffer.Get(src);
					ret = Encode0(src, 0, src.Length, dst);
				}
				if (ret != dst.Length)
				{
					 dst = Arrays.CopyOf(dst, ret);
				}
				return ByteBuffer.Wrap(dst);
			}

			/// <summary>
			/// Wraps an output stream for encoding byte data using the <seealso cref="Base64"/>
			/// encoding scheme.
			/// 
			/// <para> It is recommended to promptly close the returned output stream after
			/// use, during which it will flush all possible leftover bytes to the underlying
			/// output stream. Closing the returned output stream will close the underlying
			/// output stream.
			/// 
			/// </para>
			/// </summary>
			/// <param name="os">
			///          the output stream. </param>
			/// <returns>  the output stream for encoding the byte data into the
			///          specified Base64 encoded format </returns>
			public virtual OutputStream Wrap(OutputStream os)
			{
				Objects.RequireNonNull(os);
				return new EncOutputStream(os, IsURL ? ToBase64URL : ToBase64, Newline, Linemax, DoPadding);
			}

			/// <summary>
			/// Returns an encoder instance that encodes equivalently to this one,
			/// but without adding any padding character at the end of the encoded
			/// byte data.
			/// 
			/// <para> The encoding scheme of this encoder instance is unaffected by
			/// this invocation. The returned encoder instance should be used for
			/// non-padding encoding operation.
			/// 
			/// </para>
			/// </summary>
			/// <returns> an equivalent encoder that encodes without adding any
			///         padding character at the end </returns>
			public virtual Encoder WithoutPadding()
			{
				if (!DoPadding)
				{
					return this;
				}
				return new Encoder(IsURL, Newline, Linemax, false);
			}

			internal virtual int Encode0(sbyte[] src, int off, int end, sbyte[] dst)
			{
				char[] base64 = IsURL ? ToBase64URL : ToBase64;
				int sp = off;
				int slen = (end - off) / 3 * 3;
				int sl = off + slen;
				if (Linemax > 0 && slen > Linemax / 4 * 3)
				{
					slen = Linemax / 4 * 3;
				}
				int dp = 0;
				while (sp < sl)
				{
					int sl0 = System.Math.Min(sp + slen, sl);
					for (int sp0 = sp, dp0 = dp ; sp0 < sl0;)
					{
						int bits = (src[sp0++] & 0xff) << 16 | (src[sp0++] & 0xff) << 8 | (src[sp0++] & 0xff);
						dst[dp0++] = (sbyte)base64[((int)((uint)bits >> 18)) & 0x3f];
						dst[dp0++] = (sbyte)base64[((int)((uint)bits >> 12)) & 0x3f];
						dst[dp0++] = (sbyte)base64[((int)((uint)bits >> 6)) & 0x3f];
						dst[dp0++] = (sbyte)base64[bits & 0x3f];
					}
					int dlen = (sl0 - sp) / 3 * 4;
					dp += dlen;
					sp = sl0;
					if (dlen == Linemax && sp < end)
					{
						foreach (sbyte b in Newline)
						{
							dst[dp++] = b;
						}
					}
				}
				if (sp < end) // 1 or 2 leftover bytes
				{
					int b0 = src[sp++] & 0xff;
					dst[dp++] = (sbyte)base64[b0 >> 2];
					if (sp == end)
					{
						dst[dp++] = (sbyte)base64[(b0 << 4) & 0x3f];
						if (DoPadding)
						{
							dst[dp++] = (sbyte)'=';
							dst[dp++] = (sbyte)'=';
						}
					}
					else
					{
						int b1 = src[sp++] & 0xff;
						dst[dp++] = (sbyte)base64[(b0 << 4) & 0x3f | (b1 >> 4)];
						dst[dp++] = (sbyte)base64[(b1 << 2) & 0x3f];
						if (DoPadding)
						{
							dst[dp++] = (sbyte)'=';
						}
					}
				}
				return dp;
			}
		}

		/// <summary>
		/// This class implements a decoder for decoding byte data using the
		/// Base64 encoding scheme as specified in RFC 4648 and RFC 2045.
		/// 
		/// <para> The Base64 padding character {@code '='} is accepted and
		/// interpreted as the end of the encoded byte data, but is not
		/// required. So if the final unit of the encoded byte data only has
		/// two or three Base64 characters (without the corresponding padding
		/// character(s) padded), they are decoded as if followed by padding
		/// character(s). If there is a padding character present in the
		/// final unit, the correct number of padding character(s) must be
		/// present, otherwise {@code IllegalArgumentException} (
		/// {@code IOException} when reading from a Base64 stream) is thrown
		/// during decoding.
		/// 
		/// </para>
		/// <para> Instances of <seealso cref="Decoder"/> class are safe for use by
		/// multiple concurrent threads.
		/// 
		/// </para>
		/// <para> Unless otherwise noted, passing a {@code null} argument to
		/// a method of this class will cause a
		/// <seealso cref="java.lang.NullPointerException NullPointerException"/> to
		/// be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     Encoder
		/// @since   1.8 </seealso>
		public class Decoder
		{

			internal readonly bool IsURL;
			internal readonly bool IsMIME;

			internal Decoder(bool isURL, bool isMIME)
			{
				this.IsURL = isURL;
				this.IsMIME = isMIME;
			}

			/// <summary>
			/// Lookup table for decoding unicode characters drawn from the
			/// "Base64 Alphabet" (as specified in Table 1 of RFC 2045) into
			/// their 6-bit positive integer equivalents.  Characters that
			/// are not in the Base64 alphabet but fall within the bounds of
			/// the array are encoded to -1.
			/// 
			/// </summary>
			internal static readonly int[] FromBase64 = new int[256];
			static Decoder()
			{
				Arrays.Fill(FromBase64, -1);
				for (int i = 0; i < Encoder.ToBase64.Length; i++)
				{
					FromBase64[Encoder.ToBase64[i]] = i;
				}
				FromBase64['='] = -2;
				Arrays.Fill(FromBase64URL, -1);
				for (int i = 0; i < Encoder.ToBase64URL.Length; i++)
				{
					FromBase64URL[Encoder.ToBase64URL[i]] = i;
				}
				FromBase64URL['='] = -2;
			}

			/// <summary>
			/// Lookup table for decoding "URL and Filename safe Base64 Alphabet"
			/// as specified in Table2 of the RFC 4648.
			/// </summary>
			internal static readonly int[] FromBase64URL = new int[256];


			internal static readonly Decoder RFC4648 = new Decoder(false, false);
			internal static readonly Decoder RFC4648_URLSAFE = new Decoder(true, false);
			internal static readonly Decoder RFC2045 = new Decoder(false, true);

			/// <summary>
			/// Decodes all bytes from the input byte array using the <seealso cref="Base64"/>
			/// encoding scheme, writing the results into a newly-allocated output
			/// byte array. The returned byte array is of the length of the resulting
			/// bytes.
			/// </summary>
			/// <param name="src">
			///          the byte array to decode
			/// </param>
			/// <returns>  A newly-allocated byte array containing the decoded bytes.
			/// </returns>
			/// <exception cref="IllegalArgumentException">
			///          if {@code src} is not in valid Base64 scheme </exception>
			public virtual sbyte[] Decode(sbyte[] src)
			{
				sbyte[] dst = new sbyte[OutLength(src, 0, src.Length)];
				int ret = Decode0(src, 0, src.Length, dst);
				if (ret != dst.Length)
				{
					dst = Arrays.CopyOf(dst, ret);
				}
				return dst;
			}

			/// <summary>
			/// Decodes a Base64 encoded String into a newly-allocated byte array
			/// using the <seealso cref="Base64"/> encoding scheme.
			/// 
			/// <para> An invocation of this method has exactly the same effect as invoking
			/// {@code decode(src.getBytes(StandardCharsets.ISO_8859_1))}
			/// 
			/// </para>
			/// </summary>
			/// <param name="src">
			///          the string to decode
			/// </param>
			/// <returns>  A newly-allocated byte array containing the decoded bytes.
			/// </returns>
			/// <exception cref="IllegalArgumentException">
			///          if {@code src} is not in valid Base64 scheme </exception>
			public virtual sbyte[] Decode(String src)
			{
				return Decode(src.GetBytes(StandardCharsets.ISO_8859_1));
			}

			/// <summary>
			/// Decodes all bytes from the input byte array using the <seealso cref="Base64"/>
			/// encoding scheme, writing the results into the given output byte array,
			/// starting at offset 0.
			/// 
			/// <para> It is the responsibility of the invoker of this method to make
			/// sure the output byte array {@code dst} has enough space for decoding
			/// all bytes from the input byte array. No bytes will be be written to
			/// the output byte array if the output byte array is not big enough.
			/// 
			/// </para>
			/// <para> If the input byte array is not in valid Base64 encoding scheme
			/// then some bytes may have been written to the output byte array before
			/// IllegalargumentException is thrown.
			/// 
			/// </para>
			/// </summary>
			/// <param name="src">
			///          the byte array to decode </param>
			/// <param name="dst">
			///          the output byte array
			/// </param>
			/// <returns>  The number of bytes written to the output byte array
			/// </returns>
			/// <exception cref="IllegalArgumentException">
			///          if {@code src} is not in valid Base64 scheme, or {@code dst}
			///          does not have enough space for decoding all input bytes. </exception>
			public virtual int Decode(sbyte[] src, sbyte[] dst)
			{
				int len = OutLength(src, 0, src.Length);
				if (dst.Length < len)
				{
					throw new IllegalArgumentException("Output byte array is too small for decoding all input bytes");
				}
				return Decode0(src, 0, src.Length, dst);
			}

			/// <summary>
			/// Decodes all bytes from the input byte buffer using the <seealso cref="Base64"/>
			/// encoding scheme, writing the results into a newly-allocated ByteBuffer.
			/// 
			/// <para> Upon return, the source buffer's position will be updated to
			/// its limit; its limit will not have been changed. The returned
			/// output buffer's position will be zero and its limit will be the
			/// number of resulting decoded bytes
			/// 
			/// </para>
			/// <para> {@code IllegalArgumentException} is thrown if the input buffer
			/// is not in valid Base64 encoding scheme. The position of the input
			/// buffer will not be advanced in this case.
			/// 
			/// </para>
			/// </summary>
			/// <param name="buffer">
			///          the ByteBuffer to decode
			/// </param>
			/// <returns>  A newly-allocated byte buffer containing the decoded bytes
			/// </returns>
			/// <exception cref="IllegalArgumentException">
			///          if {@code src} is not in valid Base64 scheme. </exception>
			public virtual ByteBuffer Decode(ByteBuffer buffer)
			{
				int pos0 = buffer.Position();
				try
				{
					sbyte[] src;
					int sp, sl;
					if (buffer.HasArray())
					{
						src = buffer.Array();
						sp = buffer.ArrayOffset() + buffer.Position();
						sl = buffer.ArrayOffset() + buffer.Limit();
						buffer.Position(buffer.Limit());
					}
					else
					{
						src = new sbyte[buffer.Remaining()];
						buffer.Get(src);
						sp = 0;
						sl = src.Length;
					}
					sbyte[] dst = new sbyte[OutLength(src, sp, sl)];
					return ByteBuffer.Wrap(dst, 0, Decode0(src, sp, sl, dst));
				}
				catch (IllegalArgumentException iae)
				{
					buffer.Position(pos0);
					throw iae;
				}
			}

			/// <summary>
			/// Returns an input stream for decoding <seealso cref="Base64"/> encoded byte stream.
			/// 
			/// <para> The {@code read}  methods of the returned {@code InputStream} will
			/// throw {@code IOException} when reading bytes that cannot be decoded.
			/// 
			/// </para>
			/// <para> Closing the returned input stream will close the underlying
			/// input stream.
			/// 
			/// </para>
			/// </summary>
			/// <param name="is">
			///          the input stream
			/// </param>
			/// <returns>  the input stream for decoding the specified Base64 encoded
			///          byte stream </returns>
			public virtual InputStream Wrap(InputStream @is)
			{
				Objects.RequireNonNull(@is);
				return new DecInputStream(@is, IsURL ? FromBase64URL : FromBase64, IsMIME);
			}

			internal virtual int OutLength(sbyte[] src, int sp, int sl)
			{
				int[] base64 = IsURL ? FromBase64URL : FromBase64;
				int paddings = 0;
				int len = sl - sp;
				if (len == 0)
				{
					return 0;
				}
				if (len < 2)
				{
					if (IsMIME && base64[0] == -1)
					{
						return 0;
					}
					throw new IllegalArgumentException("Input byte[] should at least have 2 bytes for base64 bytes");
				}
				if (IsMIME)
				{
					// scan all bytes to fill out all non-alphabet. a performance
					// trade-off of pre-scan or Arrays.copyOf
					int n = 0;
					while (sp < sl)
					{
						int b = src[sp++] & 0xff;
						if (b == '=')
						{
							len -= (sl - sp + 1);
							break;
						}
						if ((b = base64[b]) == -1)
						{
							n++;
						}
					}
					len -= n;
				}
				else
				{
					if (src[sl - 1] == '=')
					{
						paddings++;
						if (src[sl - 2] == '=')
						{
							paddings++;
						}
					}
				}
				if (paddings == 0 && (len & 0x3) != 0)
				{
					paddings = 4 - (len & 0x3);
				}
				return 3 * ((len + 3) / 4) - paddings;
			}

			internal virtual int Decode0(sbyte[] src, int sp, int sl, sbyte[] dst)
			{
				int[] base64 = IsURL ? FromBase64URL : FromBase64;
				int dp = 0;
				int bits = 0;
				int shiftto = 18; // pos of first byte of 4-byte atom
				while (sp < sl)
				{
					int b = src[sp++] & 0xff;
					if ((b = base64[b]) < 0)
					{
						if (b == -2) // padding byte '='
						{
							// =     shiftto==18 unnecessary padding
							// x=    shiftto==12 a dangling single x
							// x     to be handled together with non-padding case
							// xx=   shiftto==6&&sp==sl missing last =
							// xx=y  shiftto==6 last is not =
							if (shiftto == 6 && (sp == sl || src[sp++] != '=') || shiftto == 18)
							{
								throw new IllegalArgumentException("Input byte array has wrong 4-byte ending unit");
							}
							break;
						}
						if (IsMIME) // skip if for rfc2045
						{
							continue;
						}
						else
						{
							throw new IllegalArgumentException("Illegal base64 character " + Convert.ToString(src[sp - 1], 16));
						}
					}
					bits |= (b << shiftto);
					shiftto -= 6;
					if (shiftto < 0)
					{
						dst[dp++] = (sbyte)(bits >> 16);
						dst[dp++] = (sbyte)(bits >> 8);
						dst[dp++] = (sbyte)(bits);
						shiftto = 18;
						bits = 0;
					}
				}
				// reached end of byte array or hit padding '=' characters.
				if (shiftto == 6)
				{
					dst[dp++] = (sbyte)(bits >> 16);
				}
				else if (shiftto == 0)
				{
					dst[dp++] = (sbyte)(bits >> 16);
					dst[dp++] = (sbyte)(bits >> 8);
				}
				else if (shiftto == 12)
				{
					// dangling single "x", incorrectly encoded.
					throw new IllegalArgumentException("Last unit does not have enough valid bits");
				}
				// anything left is invalid, if is not MIME.
				// if MIME, ignore all non-base64 character
				while (sp < sl)
				{
					if (IsMIME && base64[src[sp++]] < 0)
					{
						continue;
					}
					throw new IllegalArgumentException("Input byte array has incorrect ending byte at " + sp);
				}
				return dp;
			}
		}

		/*
		 * An output stream for encoding bytes into the Base64.
		 */
		private class EncOutputStream : FilterOutputStream
		{

			internal int Leftover = 0;
			internal int B0, B1, B2;
			internal bool Closed = false;

			internal readonly char[] Base64; // byte->base64 mapping
			internal readonly sbyte[] Newline; // line separator, if needed
			internal readonly int Linemax;
			internal readonly bool DoPadding; // whether or not to pad
			internal int Linepos = 0;

			internal EncOutputStream(OutputStream os, char[] base64, sbyte[] newline, int linemax, bool doPadding) : base(os)
			{
				this.Base64 = base64;
				this.Newline = newline;
				this.Linemax = linemax;
				this.DoPadding = doPadding;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void write(int b) throws java.io.IOException
			public override void Write(int b)
			{
				sbyte[] buf = new sbyte[1];
				buf[0] = unchecked((sbyte)(b & 0xff));
				Write(buf, 0, 1);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkNewline() throws java.io.IOException
			internal virtual void CheckNewline()
			{
				if (Linepos == Linemax)
				{
					@out.Write(Newline);
					Linepos = 0;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void write(byte[] b, int off, int len) throws java.io.IOException
			public override void Write(sbyte[] b, int off, int len)
			{
				if (Closed)
				{
					throw new IOException("Stream is closed");
				}
				if (off < 0 || len < 0 || off + len > b.Length)
				{
					throw new ArrayIndexOutOfBoundsException();
				}
				if (len == 0)
				{
					return;
				}
				if (Leftover != 0)
				{
					if (Leftover == 1)
					{
						B1 = b[off++] & 0xff;
						len--;
						if (len == 0)
						{
							Leftover++;
							return;
						}
					}
					B2 = b[off++] & 0xff;
					len--;
					CheckNewline();
					@out.Write(Base64[B0 >> 2]);
					@out.Write(Base64[(B0 << 4) & 0x3f | (B1 >> 4)]);
					@out.Write(Base64[(B1 << 2) & 0x3f | (B2 >> 6)]);
					@out.Write(Base64[B2 & 0x3f]);
					Linepos += 4;
				}
				int nBits24 = len / 3;
				Leftover = len - (nBits24 * 3);
				while (nBits24-- > 0)
				{
					CheckNewline();
					int bits = (b[off++] & 0xff) << 16 | (b[off++] & 0xff) << 8 | (b[off++] & 0xff);
					@out.Write(Base64[((int)((uint)bits >> 18)) & 0x3f]);
					@out.Write(Base64[((int)((uint)bits >> 12)) & 0x3f]);
					@out.Write(Base64[((int)((uint)bits >> 6)) & 0x3f]);
					@out.Write(Base64[bits & 0x3f]);
					Linepos += 4;
				}
				if (Leftover == 1)
				{
					B0 = b[off++] & 0xff;
				}
				else if (Leftover == 2)
				{
					B0 = b[off++] & 0xff;
					B1 = b[off++] & 0xff;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			public override void Close()
			{
				if (!Closed)
				{
					Closed = true;
					if (Leftover == 1)
					{
						CheckNewline();
						@out.Write(Base64[B0 >> 2]);
						@out.Write(Base64[(B0 << 4) & 0x3f]);
						if (DoPadding)
						{
							@out.Write('=');
							@out.Write('=');
						}
					}
					else if (Leftover == 2)
					{
						CheckNewline();
						@out.Write(Base64[B0 >> 2]);
						@out.Write(Base64[(B0 << 4) & 0x3f | (B1 >> 4)]);
						@out.Write(Base64[(B1 << 2) & 0x3f]);
						if (DoPadding)
						{
						   @out.Write('=');
						}
					}
					Leftover = 0;
					@out.Close();
				}
			}
		}

		/*
		 * An input stream for decoding Base64 bytes
		 */
		private class DecInputStream : InputStream
		{

			internal readonly InputStream @is;
			internal readonly bool IsMIME;
			internal readonly int[] Base64; // base64 -> byte mapping
			internal int Bits = 0; // 24-bit buffer for decoding
			internal int Nextin = 18; // next available "off" in "bits" for input;
											 // -> 18, 12, 6, 0
			internal int Nextout = -8; // next available "off" in "bits" for output;
											 // -> 8, 0, -8 (no byte for output)
			internal bool Eof = false;
			internal bool Closed = false;

			internal DecInputStream(InputStream @is, int[] base64, bool isMIME)
			{
				this.@is = @is;
				this.Base64 = base64;
				this.IsMIME = isMIME;
			}

			internal sbyte[] SbBuf = new sbyte[1];

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read() throws java.io.IOException
			public override int Read()
			{
				return Read(SbBuf, 0, 1) == -1 ? - 1 : SbBuf[0] & 0xff;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read(byte[] b, int off, int len) throws java.io.IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				if (Closed)
				{
					throw new IOException("Stream is closed");
				}
				if (Eof && Nextout < 0) // eof and no leftover
				{
					return -1;
				}
				if (off < 0 || len < 0 || len > b.Length - off)
				{
					throw new IndexOutOfBoundsException();
				}
				int oldOff = off;
				if (Nextout >= 0) // leftover output byte(s) in bits buf
				{
					do
					{
						if (len == 0)
						{
							return off - oldOff;
						}
						b[off++] = (sbyte)(Bits >> Nextout);
						len--;
						Nextout -= 8;
					} while (Nextout >= 0);
					Bits = 0;
				}
				while (len > 0)
				{
					int v = @is.Read();
					if (v == -1)
					{
						Eof = true;
						if (Nextin != 18)
						{
							if (Nextin == 12)
							{
								throw new IOException("Base64 stream has one un-decoded dangling byte.");
							}
							// treat ending xx/xxx without padding character legal.
							// same logic as v == '=' below
							b[off++] = (sbyte)(Bits >> (16));
							len--;
							if (Nextin == 0) // only one padding byte
							{
								if (len == 0) // no enough output space
								{
									Bits >>= 8; // shift to lowest byte
									Nextout = 0;
								}
								else
								{
									b[off++] = (sbyte)(Bits >> 8);
								}
							}
						}
						if (off == oldOff)
						{
							return -1;
						}
						else
						{
							return off - oldOff;
						}
					}
					if (v == '=') // padding byte(s)
					{
						// =     shiftto==18 unnecessary padding
						// x=    shiftto==12 dangling x, invalid unit
						// xx=   shiftto==6 && missing last '='
						// xx=y  or last is not '='
						if (Nextin == 18 || Nextin == 12 || Nextin == 6 && @is.Read() != '=')
						{
							throw new IOException("Illegal base64 ending sequence:" + Nextin);
						}
						b[off++] = (sbyte)(Bits >> (16));
						len--;
						if (Nextin == 0) // only one padding byte
						{
							if (len == 0) // no enough output space
							{
								Bits >>= 8; // shift to lowest byte
								Nextout = 0;
							}
							else
							{
								b[off++] = (sbyte)(Bits >> 8);
							}
						}
						Eof = true;
						break;
					}
					if ((v = Base64[v]) == -1)
					{
						if (IsMIME) // skip if for rfc2045
						{
							continue;
						}
						else
						{
							throw new IOException("Illegal base64 character " + Convert.ToString(v, 16));
						}
					}
					Bits |= (v << Nextin);
					if (Nextin == 0)
					{
						Nextin = 18; // clear for next
						Nextout = 16;
						while (Nextout >= 0)
						{
							b[off++] = (sbyte)(Bits >> Nextout);
							len--;
							Nextout -= 8;
							if (len == 0 && Nextout >= 0) // don't clean "bits"
							{
								return off - oldOff;
							}
						}
						Bits = 0;
					}
					else
					{
						Nextin -= 6;
					}
				}
				return off - oldOff;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int available() throws java.io.IOException
			public override int Available()
			{
				if (Closed)
				{
					throw new IOException("Stream is closed");
				}
				return @is.Available(); // TBD:
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			public override void Close()
			{
				if (!Closed)
				{
					Closed = true;
					@is.Close();
				}
			}
		}
	}

}