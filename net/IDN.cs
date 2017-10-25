using System;
using System.Diagnostics;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using StringPrep = sun.net.idn.StringPrep;
	using Punycode = sun.net.idn.Punycode;
	using UCharacterIterator = sun.text.normalizer.UCharacterIterator;

	/// <summary>
	/// Provides methods to convert internationalized domain names (IDNs) between
	/// a normal Unicode representation and an ASCII Compatible Encoding (ACE) representation.
	/// Internationalized domain names can use characters from the entire range of
	/// Unicode, while traditional domain names are restricted to ASCII characters.
	/// ACE is an encoding of Unicode strings that uses only ASCII characters and
	/// can be used with software (such as the Domain Name System) that only
	/// understands traditional domain names.
	/// 
	/// <para>Internationalized domain names are defined in <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
	/// RFC 3490 defines two operations: ToASCII and ToUnicode. These 2 operations employ
	/// <a href="http://www.ietf.org/rfc/rfc3491.txt">Nameprep</a> algorithm, which is a
	/// profile of <a href="http://www.ietf.org/rfc/rfc3454.txt">Stringprep</a>, and
	/// <a href="http://www.ietf.org/rfc/rfc3492.txt">Punycode</a> algorithm to convert
	/// domain name string back and forth.
	/// 
	/// </para>
	/// <para>The behavior of aforementioned conversion process can be adjusted by various flags:
	///   <ul>
	///     <li>If the ALLOW_UNASSIGNED flag is used, the domain name string to be converted
	///         can contain code points that are unassigned in Unicode 3.2, which is the
	///         Unicode version on which IDN conversion is based. If the flag is not used,
	///         the presence of such unassigned code points is treated as an error.
	///     <li>If the USE_STD3_ASCII_RULES flag is used, ASCII strings are checked against <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC 1122</a> and <a href="http://www.ietf.org/rfc/rfc1123.txt">RFC 1123</a>.
	///         It is an error if they don't meet the requirements.
	///   </ul>
	/// These flags can be logically OR'ed together.
	/// 
	/// </para>
	/// <para>The security consideration is important with respect to internationalization
	/// domain name support. For example, English domain names may be <i>homographed</i>
	/// - maliciously misspelled by substitution of non-Latin letters.
	/// <a href="http://www.unicode.org/reports/tr36/">Unicode Technical Report #36</a>
	/// discusses security issues of IDN support as well as possible solutions.
	/// Applications are responsible for taking adequate security measures when using
	/// international domain names.
	/// 
	/// @author Edward Wang
	/// @since 1.6
	/// 
	/// </para>
	/// </summary>
	public sealed class IDN
	{
		/// <summary>
		/// Flag to allow processing of unassigned code points
		/// </summary>
		public const int ALLOW_UNASSIGNED = 0x01;

		/// <summary>
		/// Flag to turn on the check against STD-3 ASCII rules
		/// </summary>
		public const int USE_STD3_ASCII_RULES = 0x02;


		/// <summary>
		/// Translates a string from Unicode to ASCII Compatible Encoding (ACE),
		/// as defined by the ToASCII operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		/// 
		/// <para>ToASCII operation can fail. ToASCII fails if any step of it fails.
		/// If ToASCII operation fails, an IllegalArgumentException will be thrown.
		/// In this case, the input string should not be used in an internationalized domain name.
		/// 
		/// </para>
		/// <para> A label is an individual part of a domain name. The original ToASCII operation,
		/// as defined in RFC 3490, only operates on a single label. This method can handle
		/// both label and entire domain name, by assuming that labels in a domain name are
		/// always separated by dots. The following characters are recognized as dots:
		/// &#0092;u002E (full stop), &#0092;u3002 (ideographic full stop), &#0092;uFF0E (fullwidth full stop),
		/// and &#0092;uFF61 (halfwidth ideographic full stop). if dots are
		/// used as label separators, this method also changes all of them to &#0092;u002E (full stop)
		/// in output translated string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">     the string to be processed </param>
		/// <param name="flag">      process flag; can be 0 or any logical OR of possible flags
		/// </param>
		/// <returns>          the translated {@code String}
		/// </returns>
		/// <exception cref="IllegalArgumentException">   if the input string doesn't conform to RFC 3490 specification </exception>
		public static String ToASCII(String input, int flag)
		{
			int p = 0, q = 0;
			StringBuffer @out = new StringBuffer();

			if (IsRootLabel(input))
			{
				return ".";
			}

			while (p < input.Length())
			{
				q = SearchDots(input, p);
				@out.Append(ToASCIIInternal(input.Substring(p, q - p), flag));
				if (q != (input.Length()))
				{
				   // has more labels, or keep the trailing dot as at present
				   @out.Append('.');
				}
				p = q + 1;
			}

			return @out.ToString();
		}


		/// <summary>
		/// Translates a string from Unicode to ASCII Compatible Encoding (ACE),
		/// as defined by the ToASCII operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		/// 
		/// <para> This convenience method works as if by invoking the
		/// two-argument counterpart as follows:
		/// <blockquote>
		/// <seealso cref="#toASCII(String, int) toASCII"/>(input,&nbsp;0);
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">     the string to be processed
		/// </param>
		/// <returns>          the translated {@code String}
		/// </returns>
		/// <exception cref="IllegalArgumentException">   if the input string doesn't conform to RFC 3490 specification </exception>
		public static String ToASCII(String input)
		{
			return ToASCII(input, 0);
		}


		/// <summary>
		/// Translates a string from ASCII Compatible Encoding (ACE) to Unicode,
		/// as defined by the ToUnicode operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		/// 
		/// <para>ToUnicode never fails. In case of any error, the input string is returned unmodified.
		/// 
		/// </para>
		/// <para> A label is an individual part of a domain name. The original ToUnicode operation,
		/// as defined in RFC 3490, only operates on a single label. This method can handle
		/// both label and entire domain name, by assuming that labels in a domain name are
		/// always separated by dots. The following characters are recognized as dots:
		/// &#0092;u002E (full stop), &#0092;u3002 (ideographic full stop), &#0092;uFF0E (fullwidth full stop),
		/// and &#0092;uFF61 (halfwidth ideographic full stop).
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">     the string to be processed </param>
		/// <param name="flag">      process flag; can be 0 or any logical OR of possible flags
		/// </param>
		/// <returns>          the translated {@code String} </returns>
		public static String ToUnicode(String input, int flag)
		{
			int p = 0, q = 0;
			StringBuffer @out = new StringBuffer();

			if (IsRootLabel(input))
			{
				return ".";
			}

			while (p < input.Length())
			{
				q = SearchDots(input, p);
				@out.Append(ToUnicodeInternal(input.Substring(p, q - p), flag));
				if (q != (input.Length()))
				{
				   // has more labels, or keep the trailing dot as at present
				   @out.Append('.');
				}
				p = q + 1;
			}

			return @out.ToString();
		}


		/// <summary>
		/// Translates a string from ASCII Compatible Encoding (ACE) to Unicode,
		/// as defined by the ToUnicode operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		/// 
		/// <para> This convenience method works as if by invoking the
		/// two-argument counterpart as follows:
		/// <blockquote>
		/// <seealso cref="#toUnicode(String, int) toUnicode"/>(input,&nbsp;0);
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">     the string to be processed
		/// </param>
		/// <returns>          the translated {@code String} </returns>
		public static String ToUnicode(String input)
		{
			return ToUnicode(input, 0);
		}


		/* ---------------- Private members -------------- */

		// ACE Prefix is "xn--"
		private const String ACE_PREFIX = "xn--";
		private static readonly int ACE_PREFIX_LENGTH = ACE_PREFIX.Length();

		private const int MAX_LABEL_LENGTH = 63;

		// single instance of nameprep
		private static StringPrep NamePrep = null;

		static IDN()
		{
			InputStream stream = null;

			try
			{
				const String IDN_PROFILE = "uidna.spp";
				if (System.SecurityManager != null)
				{
					stream = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(IDN_PROFILE));
				}
				else
				{
					stream = typeof(StringPrep).getResourceAsStream(IDN_PROFILE);
				}

				NamePrep = new StringPrep(stream);
				stream.Close();
			}
			catch (IOException)
			{
				// should never reach here
				Debug.Assert(false);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<InputStream>
		{
			private string IDN_PROFILE;

			public PrivilegedActionAnonymousInnerClassHelper(string IDN_PROFILE)
			{
				this.IDN_PROFILE = IDN_PROFILE;
			}

			public virtual InputStream Run()
			{
				return typeof(StringPrep).getResourceAsStream(IDN_PROFILE);
			}
		}


		/* ---------------- Private operations -------------- */


		//
		// to suppress the default zero-argument constructor
		//
		private IDN()
		{
		}

		//
		// toASCII operation; should only apply to a single label
		//
		private static String ToASCIIInternal(String label, int flag)
		{
			// step 1
			// Check if the string contains code points outside the ASCII range 0..0x7c.
			bool isASCII = IsAllASCII(label);
			StringBuffer dest;

			// step 2
			// perform the nameprep operation; flag ALLOW_UNASSIGNED is used here
			if (!isASCII)
			{
				UCharacterIterator iter = UCharacterIterator.getInstance(label);
				try
				{
					dest = NamePrep.prepare(iter, flag);
				}
				catch (java.text.ParseException e)
				{
					throw new IllegalArgumentException(e);
				}
			}
			else
			{
				dest = new StringBuffer(label);
			}

			// step 8, move forward to check the smallest number of the code points
			// the length must be inside 1..63
			if (dest.Length() == 0)
			{
				throw new IllegalArgumentException("Empty label is not a legal name");
			}

			// step 3
			// Verify the absence of non-LDH ASCII code points
			//   0..0x2c, 0x2e..0x2f, 0x3a..0x40, 0x5b..0x60, 0x7b..0x7f
			// Verify the absence of leading and trailing hyphen
			bool useSTD3ASCIIRules = ((flag & USE_STD3_ASCII_RULES) != 0);
			if (useSTD3ASCIIRules)
			{
				for (int i = 0; i < dest.Length(); i++)
				{
					int c = dest.CharAt(i);
					if (IsNonLDHAsciiCodePoint(c))
					{
						throw new IllegalArgumentException("Contains non-LDH ASCII characters");
					}
				}

				if (dest.CharAt(0) == '-' || dest.CharAt(dest.Length() - 1) == '-')
				{

					throw new IllegalArgumentException("Has leading or trailing hyphen");
				}
			}

			if (!isASCII)
			{
				// step 4
				// If all code points are inside 0..0x7f, skip to step 8
				if (!IsAllASCII(dest.ToString()))
				{
					// step 5
					// verify the sequence does not begin with ACE prefix
					if (!StartsWithACEPrefix(dest))
					{

						// step 6
						// encode the sequence with punycode
						try
						{
							dest = Punycode.encode(dest, null);
						}
						catch (java.text.ParseException e)
						{
							throw new IllegalArgumentException(e);
						}

						dest = ToASCIILower(dest);

						// step 7
						// prepend the ACE prefix
						dest.Insert(0, ACE_PREFIX);
					}
					else
					{
						throw new IllegalArgumentException("The input starts with the ACE Prefix");
					}

				}
			}

			// step 8
			// the length must be inside 1..63
			if (dest.Length() > MAX_LABEL_LENGTH)
			{
				throw new IllegalArgumentException("The label in the input is too long");
			}

			return dest.ToString();
		}

		//
		// toUnicode operation; should only apply to a single label
		//
		private static String ToUnicodeInternal(String label, int flag)
		{
			bool[] caseFlags = null;
			StringBuffer dest;

			// step 1
			// find out if all the codepoints in input are ASCII
			bool isASCII = IsAllASCII(label);

			if (!isASCII)
			{
				// step 2
				// perform the nameprep operation; flag ALLOW_UNASSIGNED is used here
				try
				{
					UCharacterIterator iter = UCharacterIterator.getInstance(label);
					dest = NamePrep.prepare(iter, flag);
				}
				catch (Exception)
				{
					// toUnicode never fails; if any step fails, return the input string
					return label;
				}
			}
			else
			{
				dest = new StringBuffer(label);
			}

			// step 3
			// verify ACE Prefix
			if (StartsWithACEPrefix(dest))
			{

				// step 4
				// Remove the ACE Prefix
				String temp = dest.Substring(ACE_PREFIX_LENGTH, dest.Length() - ACE_PREFIX_LENGTH);

				try
				{
					// step 5
					// Decode using punycode
					StringBuffer decodeOut = Punycode.decode(new StringBuffer(temp), null);

					// step 6
					// Apply toASCII
					String toASCIIOut = ToASCII(decodeOut.ToString(), flag);

					// step 7
					// verify
					if (toASCIIOut.EqualsIgnoreCase(dest.ToString()))
					{
						// step 8
						// return output of step 5
						return decodeOut.ToString();
					}
				}
				catch (Exception)
				{
					// no-op
				}
			}

			// just return the input
			return label;
		}


		//
		// LDH stands for "letter/digit/hyphen", with characters restricted to the
		// 26-letter Latin alphabet <A-Z a-z>, the digits <0-9>, and the hyphen
		// <->.
		// Non LDH refers to characters in the ASCII range, but which are not
		// letters, digits or the hypen.
		//
		// non-LDH = 0..0x2C, 0x2E..0x2F, 0x3A..0x40, 0x5B..0x60, 0x7B..0x7F
		//
		private static bool IsNonLDHAsciiCodePoint(int ch)
		{
			return (0x0000 <= ch && ch <= 0x002C) || (0x002E <= ch && ch <= 0x002F) || (0x003A <= ch && ch <= 0x0040) || (0x005B <= ch && ch <= 0x0060) || (0x007B <= ch && ch <= 0x007F);
		}

		//
		// search dots in a string and return the index of that character;
		// or if there is no dots, return the length of input string
		// dots might be: \u002E (full stop), \u3002 (ideographic full stop), \uFF0E (fullwidth full stop),
		// and \uFF61 (halfwidth ideographic full stop).
		//
		private static int SearchDots(String s, int start)
		{
			int i;
			for (i = start; i < s.Length(); i++)
			{
				if (IsLabelSeparator(s.CharAt(i)))
				{
					break;
				}
			}

			return i;
		}

		//
		// to check if a string is a root label, ".".
		//
		private static bool IsRootLabel(String s)
		{
			return (s.Length() == 1 && IsLabelSeparator(s.CharAt(0)));
		}

		//
		// to check if a character is a label separator, i.e. a dot character.
		//
		private static bool IsLabelSeparator(char c)
		{
			return (c == '.' || c == '\u3002' || c == '\uFF0E' || c == '\uFF61');
		}

		//
		// to check if a string only contains US-ASCII code point
		//
		private static bool IsAllASCII(String input)
		{
			bool isASCII = true;
			for (int i = 0; i < input.Length(); i++)
			{
				int c = input.CharAt(i);
				if (c > 0x7F)
				{
					isASCII = false;
					break;
				}
			}
			return isASCII;
		}

		//
		// to check if a string starts with ACE-prefix
		//
		private static bool StartsWithACEPrefix(StringBuffer input)
		{
			bool startsWithPrefix = true;

			if (input.Length() < ACE_PREFIX_LENGTH)
			{
				return false;
			}
			for (int i = 0; i < ACE_PREFIX_LENGTH; i++)
			{
				if (ToASCIILower(input.CharAt(i)) != ACE_PREFIX.CharAt(i))
				{
					startsWithPrefix = false;
				}
			}
			return startsWithPrefix;
		}

		private static char ToASCIILower(char ch)
		{
			if ('A' <= ch && ch <= 'Z')
			{
				return (char)(ch + 'a' - 'A');
			}
			return ch;
		}

		private static StringBuffer ToASCIILower(StringBuffer input)
		{
			StringBuffer dest = new StringBuffer();
			for (int i = 0; i < input.Length();i++)
			{
				dest.Append(ToASCIILower(input.CharAt(i)));
			}
			return dest;
		}
	}

}