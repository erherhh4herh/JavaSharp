/*
 * Copyright (c) 2011, Oracle and/or its affiliates. All rights reserved.
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
namespace java.nio.charset
{

	/// <summary>
	/// Constant definitions for the standard <seealso cref="Charset Charsets"/>. These
	/// charsets are guaranteed to be available on every implementation of the Java
	/// platform.
	/// </summary>
	/// <seealso cref= <a href="Charset#standard">Standard Charsets</a>
	/// @since 1.7 </seealso>
	public sealed class StandardCharsets
	{

		private StandardCharsets()
		{
			throw new AssertionError("No java.nio.charset.StandardCharsets instances for you!");
		}
		/// <summary>
		/// Seven-bit ASCII, a.k.a. ISO646-US, a.k.a. the Basic Latin block of the
		/// Unicode character set
		/// </summary>
		public static readonly Charset US_ASCII = Charset.ForName("US-ASCII");
		/// <summary>
		/// ISO Latin Alphabet No. 1, a.k.a. ISO-LATIN-1
		/// </summary>
		public static readonly Charset ISO_8859_1 = Charset.ForName("ISO-8859-1");
		/// <summary>
		/// Eight-bit UCS Transformation Format
		/// </summary>
		public static readonly Charset UTF_8 = Charset.ForName("UTF-8");
		/// <summary>
		/// Sixteen-bit UCS Transformation Format, big-endian byte order
		/// </summary>
		public static readonly Charset UTF_16BE = Charset.ForName("UTF-16BE");
		/// <summary>
		/// Sixteen-bit UCS Transformation Format, little-endian byte order
		/// </summary>
		public static readonly Charset UTF_16LE = Charset.ForName("UTF-16LE");
		/// <summary>
		/// Sixteen-bit UCS Transformation Format, byte order identified by an
		/// optional byte-order mark
		/// </summary>
		public static readonly Charset UTF_16 = Charset.ForName("UTF-16");
	}

}