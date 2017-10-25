using System;
using System.Collections;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using GetBooleanAction = sun.security.action.GetBooleanAction;
	using GetPropertyAction = sun.security.action.GetPropertyAction;

	/// <summary>
	/// Utility class for HTML form encoding. This class contains static methods
	/// for converting a String to the <CODE>application/x-www-form-urlencoded</CODE> MIME
	/// format. For more information about HTML form encoding, consult the HTML
	/// <A HREF="http://www.w3.org/TR/html4/">specification</A>.
	/// 
	/// <para>
	/// When encoding a String, the following rules apply:
	/// 
	/// <ul>
	/// <li>The alphanumeric characters &quot;{@code a}&quot; through
	///     &quot;{@code z}&quot;, &quot;{@code A}&quot; through
	///     &quot;{@code Z}&quot; and &quot;{@code 0}&quot;
	///     through &quot;{@code 9}&quot; remain the same.
	/// <li>The special characters &quot;{@code .}&quot;,
	///     &quot;{@code -}&quot;, &quot;{@code *}&quot;, and
	///     &quot;{@code _}&quot; remain the same.
	/// <li>The space character &quot; &nbsp; &quot; is
	///     converted into a plus sign &quot;{@code +}&quot;.
	/// <li>All other characters are unsafe and are first converted into
	///     one or more bytes using some encoding scheme. Then each byte is
	///     represented by the 3-character string
	///     &quot;<i>{@code %xy}</i>&quot;, where <i>xy</i> is the
	///     two-digit hexadecimal representation of the byte.
	///     The recommended encoding scheme to use is UTF-8. However,
	///     for compatibility reasons, if an encoding is not specified,
	///     then the default encoding of the platform is used.
	/// </ul>
	/// 
	/// </para>
	/// <para>
	/// For example using UTF-8 as the encoding scheme the string &quot;The
	/// string &#252;@foo-bar&quot; would get converted to
	/// &quot;The+string+%C3%BC%40foo-bar&quot; because in UTF-8 the character
	/// &#252; is encoded as two bytes C3 (hex) and BC (hex), and the
	/// character @ is encoded as one byte 40 (hex).
	/// 
	/// @author  Herb Jellinek
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class URLEncoder
	{
		internal static BitArray DontNeedEncoding;
		internal static readonly int CaseDiff = ('a' - 'A');
		internal static String DfltEncName = null;

		static URLEncoder()
		{

			/* The list of characters that are not encoded has been
			 * determined as follows:
			 *
			 * RFC 2396 states:
			 * -----
			 * Data characters that are allowed in a URI but do not have a
			 * reserved purpose are called unreserved.  These include upper
			 * and lower case letters, decimal digits, and a limited set of
			 * punctuation marks and symbols.
			 *
			 * unreserved  = alphanum | mark
			 *
			 * mark        = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
			 *
			 * Unreserved characters can be escaped without changing the
			 * semantics of the URI, but this should not be done unless the
			 * URI is being used in a context that does not allow the
			 * unescaped character to appear.
			 * -----
			 *
			 * It appears that both Netscape and Internet Explorer escape
			 * all special characters from this list with the exception
			 * of "-", "_", ".", "*". While it is not clear why they are
			 * escaping the other characters, perhaps it is safest to
			 * assume that there might be contexts in which the others
			 * are unsafe if not escaped. Therefore, we will use the same
			 * list. It is also noteworthy that this is consistent with
			 * O'Reilly's "HTML: The Definitive Guide" (page 164).
			 *
			 * As a last note, Intenet Explorer does not encode the "@"
			 * character which is clearly not unreserved according to the
			 * RFC. We are being consistent with the RFC in this matter,
			 * as is Netscape.
			 *
			 */

			DontNeedEncoding = new BitArray(256);
			int i;
			for (i = 'a'; i <= 'z'; i++)
			{
				DontNeedEncoding.Set(i, true);
			}
			for (i = 'A'; i <= 'Z'; i++)
			{
				DontNeedEncoding.Set(i, true);
			}
			for (i = '0'; i <= '9'; i++)
			{
				DontNeedEncoding.Set(i, true);
			}
			DontNeedEncoding.Set(' ', true); /* encoding a space to a + is done
	                                    * in the encode() method */
			DontNeedEncoding.Set('-', true);
			DontNeedEncoding.Set('_', true);
			DontNeedEncoding.Set('.', true);
			DontNeedEncoding.Set('*', true);

			DfltEncName = AccessController.doPrivileged(new GetPropertyAction("file.encoding")
		   );
		}

		/// <summary>
		/// You can't call the constructor.
		/// </summary>
		private URLEncoder()
		{
		}

		/// <summary>
		/// Translates a string into {@code x-www-form-urlencoded}
		/// format. This method uses the platform's default encoding
		/// as the encoding scheme to obtain the bytes for unsafe characters.
		/// </summary>
		/// <param name="s">   {@code String} to be translated. </param>
		/// @deprecated The resulting string may vary depending on the platform's
		///             default encoding. Instead, use the encode(String,String)
		///             method to specify the encoding. 
		/// <returns>  the translated {@code String}. </returns>
		[Obsolete("The resulting string may vary depending on the platform's")]
		public static String Encode(String s)
		{

			String str = null;

			try
			{
				str = Encode(s, DfltEncName);
			}
			catch (UnsupportedEncodingException)
			{
				// The system should always have the platform default
			}

			return str;
		}

		/// <summary>
		/// Translates a string into {@code application/x-www-form-urlencoded}
		/// format using a specific encoding scheme. This method uses the
		/// supplied encoding scheme to obtain the bytes for unsafe
		/// characters.
		/// <para>
		/// <em><strong>Note:</strong> The <a href=
		/// "http://www.w3.org/TR/html40/appendix/notes.html#non-ascii-chars">
		/// World Wide Web Consortium Recommendation</a> states that
		/// UTF-8 should be used. Not doing so may introduce
		/// incompatibilities.</em>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   {@code String} to be translated. </param>
		/// <param name="enc">   The name of a supported
		///    <a href="../lang/package-summary.html#charenc">character
		///    encoding</a>. </param>
		/// <returns>  the translated {@code String}. </returns>
		/// <exception cref="UnsupportedEncodingException">
		///             If the named encoding is not supported </exception>
		/// <seealso cref= URLDecoder#decode(java.lang.String, java.lang.String)
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String encode(String s, String enc) throws java.io.UnsupportedEncodingException
		public static String Encode(String s, String enc)
		{

			bool needToChange = false;
			StringBuffer @out = new StringBuffer(s.Length());
			Charset charset;
			CharArrayWriter charArrayWriter = new CharArrayWriter();

			if (enc == null)
			{
				throw new NullPointerException("charsetName");
			}

			try
			{
				charset = Charset.ForName(enc);
			}
			catch (IllegalCharsetNameException)
			{
				throw new UnsupportedEncodingException(enc);
			}
			catch (UnsupportedCharsetException)
			{
				throw new UnsupportedEncodingException(enc);
			}

			for (int i = 0; i < s.Length();)
			{
				int c = (int) s.CharAt(i);
				//System.out.println("Examining character: " + c);
				if (DontNeedEncoding.Get(c))
				{
					if (c == ' ')
					{
						c = '+';
						needToChange = true;
					}
					//System.out.println("Storing: " + c);
					@out.Append((char)c);
					i++;
				}
				else
				{
					// convert to external encoding before hex conversion
					do
					{
						charArrayWriter.Write(c);
						/*
						 * If this character represents the start of a Unicode
						 * surrogate pair, then pass in two characters. It's not
						 * clear what should be done if a bytes reserved in the
						 * surrogate pairs range occurs outside of a legal
						 * surrogate pair. For now, just treat it as if it were
						 * any other character.
						 */
						if (c >= 0xD800 && c <= 0xDBFF)
						{
							/*
							  System.out.println(Integer.toHexString(c)
							  + " is high surrogate");
							*/
							if ((i + 1) < s.Length())
							{
								int d = (int) s.CharAt(i + 1);
								/*
								  System.out.println("\tExamining "
								  + Integer.toHexString(d));
								*/
								if (d >= 0xDC00 && d <= 0xDFFF)
								{
									/*
									  System.out.println("\t"
									  + Integer.toHexString(d)
									  + " is low surrogate");
									*/
									charArrayWriter.Write(d);
									i++;
								}
							}
						}
						i++;
					} while (i < s.Length() && !DontNeedEncoding.Get((c = (int) s.CharAt(i))));

					charArrayWriter.Flush();
					String str = new String(charArrayWriter.ToCharArray());
					sbyte[] ba = str.GetBytes(charset);
					for (int j = 0; j < ba.Length; j++)
					{
						@out.Append('%');
						char ch = Character.ForDigit((ba[j] >> 4) & 0xF, 16);
						// converting to use uppercase letter as part of
						// the hex value if ch is a letter.
						if (char.IsLetter(ch))
						{
							ch -= CaseDiff;
						}
						@out.Append(ch);
						ch = Character.ForDigit(ba[j] & 0xF, 16);
						if (char.IsLetter(ch))
						{
							ch -= CaseDiff;
						}
						@out.Append(ch);
					}
					charArrayWriter.Reset();
					needToChange = true;
				}
			}

			return (needToChange? @out.ToString() : s);
		}
	}

}