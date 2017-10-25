using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{

	/// <summary>
	/// Utility class for HTML form decoding. This class contains static methods
	/// for decoding a String from the <CODE>application/x-www-form-urlencoded</CODE>
	/// MIME format.
	/// <para>
	/// The conversion process is the reverse of that used by the URLEncoder class. It is assumed
	/// that all characters in the encoded string are one of the following:
	/// &quot;{@code a}&quot; through &quot;{@code z}&quot;,
	/// &quot;{@code A}&quot; through &quot;{@code Z}&quot;,
	/// &quot;{@code 0}&quot; through &quot;{@code 9}&quot;, and
	/// &quot;{@code -}&quot;, &quot;{@code _}&quot;,
	/// &quot;{@code .}&quot;, and &quot;{@code *}&quot;. The
	/// character &quot;{@code %}&quot; is allowed but is interpreted
	/// as the start of a special escaped sequence.
	/// </para>
	/// <para>
	/// The following rules are applied in the conversion:
	/// 
	/// <ul>
	/// <li>The alphanumeric characters &quot;{@code a}&quot; through
	///     &quot;{@code z}&quot;, &quot;{@code A}&quot; through
	///     &quot;{@code Z}&quot; and &quot;{@code 0}&quot;
	///     through &quot;{@code 9}&quot; remain the same.
	/// <li>The special characters &quot;{@code .}&quot;,
	///     &quot;{@code -}&quot;, &quot;{@code *}&quot;, and
	///     &quot;{@code _}&quot; remain the same.
	/// <li>The plus sign &quot;{@code +}&quot; is converted into a
	///     space character &quot; &nbsp; &quot; .
	/// <li>A sequence of the form "<i>{@code %xy}</i>" will be
	///     treated as representing a byte where <i>xy</i> is the two-digit
	///     hexadecimal representation of the 8 bits. Then, all substrings
	///     that contain one or more of these byte sequences consecutively
	///     will be replaced by the character(s) whose encoding would result
	///     in those consecutive bytes.
	///     The encoding scheme used to decode these characters may be specified,
	///     or if unspecified, the default encoding of the platform will be used.
	/// </ul>
	/// </para>
	/// <para>
	/// There are two possible ways in which this decoder could deal with
	/// illegal strings.  It could either leave illegal characters alone or
	/// it could throw an <seealso cref="java.lang.IllegalArgumentException"/>.
	/// Which approach the decoder takes is left to the
	/// implementation.
	/// 
	/// @author  Mark Chamness
	/// @author  Michael McCloskey
	/// @since   1.2
	/// </para>
	/// </summary>

	public class URLDecoder
	{

		// The platform default encoding
		internal static String DfltEncName = URLEncoder.DfltEncName;

		/// <summary>
		/// Decodes a {@code x-www-form-urlencoded} string.
		/// The platform's default encoding is used to determine what characters
		/// are represented by any consecutive sequences of the form
		/// "<i>{@code %xy}</i>". </summary>
		/// <param name="s"> the {@code String} to decode </param>
		/// @deprecated The resulting string may vary depending on the platform's
		///          default encoding. Instead, use the decode(String,String) method
		///          to specify the encoding. 
		/// <returns> the newly decoded {@code String} </returns>
		[Obsolete("The resulting string may vary depending on the platform's")]
		public static String Decode(String s)
		{

			String str = null;

			try
			{
				str = Decode(s, DfltEncName);
			}
			catch (UnsupportedEncodingException)
			{
				// The system should always have the platform default
			}

			return str;
		}

		/// <summary>
		/// Decodes a {@code application/x-www-form-urlencoded} string using a specific
		/// encoding scheme.
		/// The supplied encoding is used to determine
		/// what characters are represented by any consecutive sequences of the
		/// form "<i>{@code %xy}</i>".
		/// <para>
		/// <em><strong>Note:</strong> The <a href=
		/// "http://www.w3.org/TR/html40/appendix/notes.html#non-ascii-chars">
		/// World Wide Web Consortium Recommendation</a> states that
		/// UTF-8 should be used. Not doing so may introduce
		/// incompatibilities.</em>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the {@code String} to decode </param>
		/// <param name="enc">   The name of a supported
		///    <a href="../lang/package-summary.html#charenc">character
		///    encoding</a>. </param>
		/// <returns> the newly decoded {@code String} </returns>
		/// <exception cref="UnsupportedEncodingException">
		///             If character encoding needs to be consulted, but
		///             named character encoding is not supported </exception>
		/// <seealso cref= URLEncoder#encode(java.lang.String, java.lang.String)
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String decode(String s, String enc) throws UnsupportedEncodingException
		public static String Decode(String s, String enc)
		{

			bool needToChange = false;
			int numChars = s.Length();
			StringBuffer sb = new StringBuffer(numChars > 500 ? numChars / 2 : numChars);
			int i = 0;

			if (enc.Length() == 0)
			{
				throw new UnsupportedEncodingException("URLDecoder: empty string enc parameter");
			}

			char c;
			sbyte[] bytes = null;
			while (i < numChars)
			{
				c = s.CharAt(i);
				switch (c)
				{
				case '+':
					sb.Append(' ');
					i++;
					needToChange = true;
					break;
				case '%':
					/*
					 * Starting with this instance of %, process all
					 * consecutive substrings of the form %xy. Each
					 * substring %xy will yield a byte. Convert all
					 * consecutive  bytes obtained this way to whatever
					 * character(s) they represent in the provided
					 * encoding.
					 */

					try
					{

						// (numChars-i)/3 is an upper bound for the number
						// of remaining bytes
						if (bytes == null)
						{
							bytes = new sbyte[(numChars - i) / 3];
						}
						int pos = 0;

						while (((i + 2) < numChars) && (c == '%'))
						{
							int v = Convert.ToInt32(StringHelperClass.SubstringSpecial(s, i + 1,i + 3),16);
							if (v < 0)
							{
								throw new IllegalArgumentException("URLDecoder: Illegal hex characters in escape (%) pattern - negative value");
							}
							bytes[pos++] = (sbyte) v;
							i += 3;
							if (i < numChars)
							{
								c = s.CharAt(i);
							}
						}

						// A trailing, incomplete byte encoding such as
						// "%x" will cause an exception to be thrown

						if ((i < numChars) && (c == '%'))
						{
							throw new IllegalArgumentException("URLDecoder: Incomplete trailing escape (%) pattern");
						}

						sb.Append(StringHelperClass.NewString(bytes, 0, pos, enc));
					}
					catch (NumberFormatException e)
					{
						throw new IllegalArgumentException("URLDecoder: Illegal hex characters in escape (%) pattern - " + e.Message);
					}
					needToChange = true;
					break;
				default:
					sb.Append(c);
					i++;
					break;
				}
			}

			return (needToChange? sb.ToString() : s);
		}
	}

}