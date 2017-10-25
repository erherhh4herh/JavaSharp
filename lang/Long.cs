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

namespace java.lang
{



	/// <summary>
	/// The {@code Long} class wraps a value of the primitive type {@code
	/// long} in an object. An object of type {@code Long} contains a
	/// single field whose type is {@code long}.
	/// 
	/// <para> In addition, this class provides several methods for converting
	/// a {@code long} to a {@code String} and a {@code String} to a {@code
	/// long}, as well as other constants and methods useful when dealing
	/// with a {@code long}.
	/// 
	/// </para>
	/// <para>Implementation note: The implementations of the "bit twiddling"
	/// methods (such as <seealso cref="#highestOneBit(long) highestOneBit"/> and
	/// <seealso cref="#numberOfTrailingZeros(long) numberOfTrailingZeros"/>) are
	/// based on material from Henry S. Warren, Jr.'s <i>Hacker's
	/// Delight</i>, (Addison Wesley, 2002).
	/// 
	/// @author  Lee Boynton
	/// @author  Arthur van Hoff
	/// @author  Josh Bloch
	/// @author  Joseph D. Darcy
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public sealed class Long : Number, Comparable<Long>
	{
		/// <summary>
		/// A constant holding the minimum value a {@code long} can
		/// have, -2<sup>63</sup>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final long MIN_VALUE = 0x8000000000000000L;
		public const long MIN_VALUE = unchecked((long)0x8000000000000000L);

		/// <summary>
		/// A constant holding the maximum value a {@code long} can
		/// have, 2<sup>63</sup>-1.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final long MAX_VALUE = 0x7fffffffffffffffL;
		public const long MAX_VALUE = 0x7fffffffffffffffL;

		/// <summary>
		/// The {@code Class} instance representing the primitive type
		/// {@code long}.
		/// 
		/// @since   JDK1.1
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("long");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("long");

		/// <summary>
		/// Returns a string representation of the first argument in the
		/// radix specified by the second argument.
		/// 
		/// <para>If the radix is smaller than {@code Character.MIN_RADIX}
		/// or larger than {@code Character.MAX_RADIX}, then the radix
		/// {@code 10} is used instead.
		/// 
		/// </para>
		/// <para>If the first argument is negative, the first element of the
		/// result is the ASCII minus sign {@code '-'}
		/// ({@code '\u005Cu002d'}). If the first argument is not
		/// negative, no sign character appears in the result.
		/// 
		/// </para>
		/// <para>The remaining characters of the result represent the magnitude
		/// of the first argument. If the magnitude is zero, it is
		/// represented by a single zero character {@code '0'}
		/// ({@code '\u005Cu0030'}); otherwise, the first character of
		/// the representation of the magnitude will not be the zero
		/// character.  The following ASCII characters are used as digits:
		/// 
		/// <blockquote>
		///   {@code 0123456789abcdefghijklmnopqrstuvwxyz}
		/// </blockquote>
		/// 
		/// These are {@code '\u005Cu0030'} through
		/// {@code '\u005Cu0039'} and {@code '\u005Cu0061'} through
		/// {@code '\u005Cu007a'}. If {@code radix} is
		/// <var>N</var>, then the first <var>N</var> of these characters
		/// are used as radix-<var>N</var> digits in the order shown. Thus,
		/// the digits for hexadecimal (radix 16) are
		/// {@code 0123456789abcdef}. If uppercase letters are
		/// desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
		/// be called on the result:
		/// 
		/// <blockquote>
		///  {@code Long.toString(n, 16).toUpperCase()}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">       a {@code long} to be converted to a string. </param>
		/// <param name="radix">   the radix to use in the string representation. </param>
		/// <returns>  a string representation of the argument in the specified radix. </returns>
		/// <seealso cref=     java.lang.Character#MAX_RADIX </seealso>
		/// <seealso cref=     java.lang.Character#MIN_RADIX </seealso>
		public static String ToString(long i, int radix)
		{
			if (radix < Character.MIN_RADIX || radix > Character.MAX_RADIX)
			{
				radix = 10;
			}
			if (radix == 10)
			{
				return ToString(i);
			}
			char[] buf = new char[65];
			int charPos = 64;
			bool negative = (i < 0);

			if (!negative)
			{
				i = -i;
			}

			while (i <= -radix)
			{
				buf[charPos--] = Integer.Digits[(int)(-(i % radix))];
				i = i / radix;
			}
			buf[charPos] = Integer.Digits[(int)(-i)];

			if (negative)
			{
				buf[--charPos] = '-';
			}

			return new String(buf, charPos, (65 - charPos));
		}

		/// <summary>
		/// Returns a string representation of the first argument as an
		/// unsigned integer value in the radix specified by the second
		/// argument.
		/// 
		/// <para>If the radix is smaller than {@code Character.MIN_RADIX}
		/// or larger than {@code Character.MAX_RADIX}, then the radix
		/// {@code 10} is used instead.
		/// 
		/// </para>
		/// <para>Note that since the first argument is treated as an unsigned
		/// value, no leading sign character is printed.
		/// 
		/// </para>
		/// <para>If the magnitude is zero, it is represented by a single zero
		/// character {@code '0'} ({@code '\u005Cu0030'}); otherwise,
		/// the first character of the representation of the magnitude will
		/// not be the zero character.
		/// 
		/// </para>
		/// <para>The behavior of radixes and the characters used as digits
		/// are the same as <seealso cref="#toString(long, int) toString"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">       an integer to be converted to an unsigned string. </param>
		/// <param name="radix">   the radix to use in the string representation. </param>
		/// <returns>  an unsigned string representation of the argument in the specified radix. </returns>
		/// <seealso cref=     #toString(long, int)
		/// @since 1.8 </seealso>
		public static String ToUnsignedString(long i, int radix)
		{
			if (i >= 0)
			{
				return ToString(i, radix);
			}
			else
			{
				switch (radix)
				{
				case 2:
					return ToBinaryString(i);

				case 4:
					return ToUnsignedString0(i, 2);

				case 8:
					return ToOctalString(i);

				case 10:
					/*
					 * We can get the effect of an unsigned division by 10
					 * on a long value by first shifting right, yielding a
					 * positive value, and then dividing by 5.  This
					 * allows the last digit and preceding digits to be
					 * isolated more quickly than by an initial conversion
					 * to BigInteger.
					 */
					long quot = ((long)((ulong)i >> 1)) / 5;
					long rem = i - quot * 10;
					return ToString(quot) + rem;

				case 16:
					return ToHexString(i);

				case 32:
					return ToUnsignedString0(i, 5);

				default:
					return ToUnsignedBigInteger(i).ToString(radix);
				}
			}
		}

		/// <summary>
		/// Return a BigInteger equal to the unsigned value of the
		/// argument.
		/// </summary>
		private static BigInteger ToUnsignedBigInteger(long i)
		{
			if (i >= 0L)
			{
				return BigInteger.ValueOf(i);
			}
			else
			{
				int upper = (int)((long)((ulong)i >> 32));
				int lower = (int) i;

				// return (upper << 32) + lower
				return (BigInteger.ValueOf(Integer.ToUnsignedLong(upper))).ShiftLeft(32).Add(BigInteger.ValueOf(Integer.ToUnsignedLong(lower)));
			}
		}

		/// <summary>
		/// Returns a string representation of the {@code long}
		/// argument as an unsigned integer in base&nbsp;16.
		/// 
		/// <para>The unsigned {@code long} value is the argument plus
		/// 2<sup>64</sup> if the argument is negative; otherwise, it is
		/// equal to the argument.  This value is converted to a string of
		/// ASCII digits in hexadecimal (base&nbsp;16) with no extra
		/// leading {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
		/// 16)}.
		/// 
		/// </para>
		/// <para>If the unsigned magnitude is zero, it is represented by a
		/// single zero character {@code '0'} ({@code '\u005Cu0030'});
		/// otherwise, the first character of the representation of the
		/// unsigned magnitude will not be the zero character. The
		/// following characters are used as hexadecimal digits:
		/// 
		/// <blockquote>
		///  {@code 0123456789abcdef}
		/// </blockquote>
		/// 
		/// These are the characters {@code '\u005Cu0030'} through
		/// {@code '\u005Cu0039'} and  {@code '\u005Cu0061'} through
		/// {@code '\u005Cu0066'}.  If uppercase letters are desired,
		/// the <seealso cref="java.lang.String#toUpperCase()"/> method may be called
		/// on the result:
		/// 
		/// <blockquote>
		///  {@code Long.toHexString(n).toUpperCase()}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   a {@code long} to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned {@code long}
		///          value represented by the argument in hexadecimal
		///          (base&nbsp;16). </returns>
		/// <seealso cref= #parseUnsignedLong(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(long, int)
		/// @since   JDK 1.0.2 </seealso>
		public static String ToHexString(long i)
		{
			return ToUnsignedString0(i, 4);
		}

		/// <summary>
		/// Returns a string representation of the {@code long}
		/// argument as an unsigned integer in base&nbsp;8.
		/// 
		/// <para>The unsigned {@code long} value is the argument plus
		/// 2<sup>64</sup> if the argument is negative; otherwise, it is
		/// equal to the argument.  This value is converted to a string of
		/// ASCII digits in octal (base&nbsp;8) with no extra leading
		/// {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
		/// 8)}.
		/// 
		/// </para>
		/// <para>If the unsigned magnitude is zero, it is represented by a
		/// single zero character {@code '0'} ({@code '\u005Cu0030'});
		/// otherwise, the first character of the representation of the
		/// unsigned magnitude will not be the zero character. The
		/// following characters are used as octal digits:
		/// 
		/// <blockquote>
		///  {@code 01234567}
		/// </blockquote>
		/// 
		/// These are the characters {@code '\u005Cu0030'} through
		/// {@code '\u005Cu0037'}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   a {@code long} to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned {@code long}
		///          value represented by the argument in octal (base&nbsp;8). </returns>
		/// <seealso cref= #parseUnsignedLong(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(long, int)
		/// @since   JDK 1.0.2 </seealso>
		public static String ToOctalString(long i)
		{
			return ToUnsignedString0(i, 3);
		}

		/// <summary>
		/// Returns a string representation of the {@code long}
		/// argument as an unsigned integer in base&nbsp;2.
		/// 
		/// <para>The unsigned {@code long} value is the argument plus
		/// 2<sup>64</sup> if the argument is negative; otherwise, it is
		/// equal to the argument.  This value is converted to a string of
		/// ASCII digits in binary (base&nbsp;2) with no extra leading
		/// {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
		/// 2)}.
		/// 
		/// </para>
		/// <para>If the unsigned magnitude is zero, it is represented by a
		/// single zero character {@code '0'} ({@code '\u005Cu0030'});
		/// otherwise, the first character of the representation of the
		/// unsigned magnitude will not be the zero character. The
		/// characters {@code '0'} ({@code '\u005Cu0030'}) and {@code
		/// '1'} ({@code '\u005Cu0031'}) are used as binary digits.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   a {@code long} to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned {@code long}
		///          value represented by the argument in binary (base&nbsp;2). </returns>
		/// <seealso cref= #parseUnsignedLong(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(long, int)
		/// @since   JDK 1.0.2 </seealso>
		public static String ToBinaryString(long i)
		{
			return ToUnsignedString0(i, 1);
		}

		/// <summary>
		/// Format a long (treated as unsigned) into a String. </summary>
		/// <param name="val"> the value to format </param>
		/// <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
		internal static String ToUnsignedString0(long val, int shift)
		{
			// assert shift > 0 && shift <=5 : "Illegal shift value";
			int mag = sizeof(long) - Long.NumberOfLeadingZeros(val);
			int chars = System.Math.Max(((mag + (shift - 1)) / shift), 1);
			char[] buf = new char[chars];

			FormatUnsignedLong(val, shift, buf, 0, chars);
			return new String(buf, true);
		}

		/// <summary>
		/// Format a long (treated as unsigned) into a character buffer. </summary>
		/// <param name="val"> the unsigned long to format </param>
		/// <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
		/// <param name="buf"> the character buffer to write to </param>
		/// <param name="offset"> the offset in the destination buffer to start at </param>
		/// <param name="len"> the number of characters to write </param>
		/// <returns> the lowest character location used </returns>
		 internal static int FormatUnsignedLong(long val, int shift, char[] buf, int offset, int len)
		 {
			int charPos = len;
			int radix = 1 << shift;
			int mask = radix - 1;
			do
			{
				buf[offset + --charPos] = Integer.Digits[((int) val) & mask];
				val = (long)((ulong)val >> shift);
			} while (val != 0 && charPos > 0);

			return charPos;
		 }

		/// <summary>
		/// Returns a {@code String} object representing the specified
		/// {@code long}.  The argument is converted to signed decimal
		/// representation and returned as a string, exactly as if the
		/// argument and the radix 10 were given as arguments to the {@link
		/// #toString(long, int)} method.
		/// </summary>
		/// <param name="i">   a {@code long} to be converted. </param>
		/// <returns>  a string representation of the argument in base&nbsp;10. </returns>
		public static String ToString(long i)
		{
			if (i == Long.MinValue)
			{
				return "-9223372036854775808";
			}
			int size = (i < 0) ? StringSize(-i) + 1 : StringSize(i);
			char[] buf = new char[size];
			GetChars(i, size, buf);
			return new String(buf, true);
		}

		/// <summary>
		/// Returns a string representation of the argument as an unsigned
		/// decimal value.
		/// 
		/// The argument is converted to unsigned decimal representation
		/// and returned as a string exactly as if the argument and radix
		/// 10 were given as arguments to the {@link #toUnsignedString(long,
		/// int)} method.
		/// </summary>
		/// <param name="i">  an integer to be converted to an unsigned string. </param>
		/// <returns>  an unsigned string representation of the argument. </returns>
		/// <seealso cref=     #toUnsignedString(long, int)
		/// @since 1.8 </seealso>
		public static String ToUnsignedString(long i)
		{
			return ToUnsignedString(i, 10);
		}

		/// <summary>
		/// Places characters representing the integer i into the
		/// character array buf. The characters are placed into
		/// the buffer backwards starting with the least significant
		/// digit at the specified index (exclusive), and working
		/// backwards from there.
		/// 
		/// Will fail if i == Long.MIN_VALUE
		/// </summary>
		internal static void GetChars(long i, int index, char[] buf)
		{
			long q;
			int r;
			int charPos = index;
			char sign = (char)0;

			if (i < 0)
			{
				sign = '-';
				i = -i;
			}

			// Get 2 digits/iteration using longs until quotient fits into an int
			while (i > Integer.MaxValue)
			{
				q = i / 100;
				// really: r = i - (q * 100);
				r = (int)(i - ((q << 6) + (q << 5) + (q << 2)));
				i = q;
				buf[--charPos] = Integer.DigitOnes[r];
				buf[--charPos] = Integer.DigitTens[r];
			}

			// Get 2 digits/iteration using ints
			int q2;
			int i2 = (int)i;
			while (i2 >= 65536)
			{
				q2 = i2 / 100;
				// really: r = i2 - (q * 100);
				r = i2 - ((q2 << 6) + (q2 << 5) + (q2 << 2));
				i2 = q2;
				buf[--charPos] = Integer.DigitOnes[r];
				buf[--charPos] = Integer.DigitTens[r];
			}

			// Fall thru to fast mode for smaller numbers
			// assert(i2 <= 65536, i2);
			for (;;)
			{
				q2 = (int)((uint)(i2 * 52429) >> (16 + 3));
				r = i2 - ((q2 << 3) + (q2 << 1)); // r = i2-(q2*10) ...
				buf[--charPos] = Integer.Digits[r];
				i2 = q2;
				if (i2 == 0)
				{
					break;
				}
			}
			if (sign != 0)
			{
				buf[--charPos] = sign;
			}
		}

		// Requires positive x
		internal static int StringSize(long x)
		{
			long p = 10;
			for (int i = 1; i < 19; i++)
			{
				if (x < p)
				{
					return i;
				}
				p = 10 * p;
			}
			return 19;
		}

		/// <summary>
		/// Parses the string argument as a signed {@code long} in the
		/// radix specified by the second argument. The characters in the
		/// string must all be digits of the specified radix (as determined
		/// by whether <seealso cref="java.lang.Character#digit(char, int)"/> returns
		/// a nonnegative value), except that the first character may be an
		/// ASCII minus sign {@code '-'} ({@code '\u005Cu002D'}) to
		/// indicate a negative value or an ASCII plus sign {@code '+'}
		/// ({@code '\u005Cu002B'}) to indicate a positive value. The
		/// resulting {@code long} value is returned.
		/// 
		/// <para>Note that neither the character {@code L}
		/// ({@code '\u005Cu004C'}) nor {@code l}
		/// ({@code '\u005Cu006C'}) is permitted to appear at the end
		/// of the string as a type indicator, as would be permitted in
		/// Java programming language source code - except that either
		/// {@code L} or {@code l} may appear as a digit for a
		/// radix greater than or equal to 22.
		/// 
		/// </para>
		/// <para>An exception of type {@code NumberFormatException} is
		/// thrown if any of the following situations occurs:
		/// <ul>
		/// 
		/// <li>The first argument is {@code null} or is a string of
		/// length zero.
		/// 
		/// <li>The {@code radix} is either smaller than {@link
		/// java.lang.Character#MIN_RADIX} or larger than {@link
		/// java.lang.Character#MAX_RADIX}.
		/// 
		/// <li>Any character of the string is not a digit of the specified
		/// radix, except that the first character may be a minus sign
		/// {@code '-'} ({@code '\u005Cu002d'}) or plus sign {@code
		/// '+'} ({@code '\u005Cu002B'}) provided that the string is
		/// longer than length 1.
		/// 
		/// <li>The value represented by the string is not a value of type
		///      {@code long}.
		/// </ul>
		/// 
		/// </para>
		/// <para>Examples:
		/// <blockquote><pre>
		/// parseLong("0", 10) returns 0L
		/// parseLong("473", 10) returns 473L
		/// parseLong("+42", 10) returns 42L
		/// parseLong("-0", 10) returns 0L
		/// parseLong("-FF", 16) returns -255L
		/// parseLong("1100110", 2) returns 102L
		/// parseLong("99", 8) throws a NumberFormatException
		/// parseLong("Hazelnut", 10) throws a NumberFormatException
		/// parseLong("Hazelnut", 36) returns 1356099454469L
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">       the {@code String} containing the
		///                     {@code long} representation to be parsed. </param>
		/// <param name="radix">   the radix to be used while parsing {@code s}. </param>
		/// <returns>     the {@code long} represented by the string argument in
		///             the specified radix. </returns>
		/// <exception cref="NumberFormatException">  if the string does not contain a
		///             parsable {@code long}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long parseLong(String s, int radix) throws NumberFormatException
		public static long ParseLong(String s, int radix)
		{
			if (s == null)
			{
				throw new NumberFormatException("null");
			}

			if (radix < Character.MIN_RADIX)
			{
				throw new NumberFormatException("radix " + radix + " less than Character.MIN_RADIX");
			}
			if (radix > Character.MAX_RADIX)
			{
				throw new NumberFormatException("radix " + radix + " greater than Character.MAX_RADIX");
			}

			long result = 0;
			bool negative = false;
			int i = 0, len = s.Length();
			long limit = -Long.MaxValue;
			long multmin;
			int digit;

			if (len > 0)
			{
				char firstChar = s.CharAt(0);
				if (firstChar < '0') // Possible leading "+" or "-"
				{
					if (firstChar == '-')
					{
						negative = true;
						limit = Long.MinValue;
					}
					else if (firstChar != '+')
					{
						throw NumberFormatException.ForInputString(s);
					}

					if (len == 1) // Cannot have lone "+" or "-"
					{
						throw NumberFormatException.ForInputString(s);
					}
					i++;
				}
				multmin = limit / radix;
				while (i < len)
				{
					// Accumulating negatively avoids surprises near MAX_VALUE
					digit = Character.Digit(s.CharAt(i++),radix);
					if (digit < 0)
					{
						throw NumberFormatException.ForInputString(s);
					}
					if (result < multmin)
					{
						throw NumberFormatException.ForInputString(s);
					}
					result *= radix;
					if (result < limit + digit)
					{
						throw NumberFormatException.ForInputString(s);
					}
					result -= digit;
				}
			}
			else
			{
				throw NumberFormatException.ForInputString(s);
			}
			return negative ? result : -result;
		}

		/// <summary>
		/// Parses the string argument as a signed decimal {@code long}.
		/// The characters in the string must all be decimal digits, except
		/// that the first character may be an ASCII minus sign {@code '-'}
		/// ({@code \u005Cu002D'}) to indicate a negative value or an
		/// ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
		/// indicate a positive value. The resulting {@code long} value is
		/// returned, exactly as if the argument and the radix {@code 10}
		/// were given as arguments to the {@link
		/// #parseLong(java.lang.String, int)} method.
		/// 
		/// <para>Note that neither the character {@code L}
		/// ({@code '\u005Cu004C'}) nor {@code l}
		/// ({@code '\u005Cu006C'}) is permitted to appear at the end
		/// of the string as a type indicator, as would be permitted in
		/// Java programming language source code.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   a {@code String} containing the {@code long}
		///             representation to be parsed </param>
		/// <returns>     the {@code long} represented by the argument in
		///             decimal. </returns>
		/// <exception cref="NumberFormatException">  if the string does not contain a
		///             parsable {@code long}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long parseLong(String s) throws NumberFormatException
		public static long ParseLong(String s)
		{
			return ParseLong(s, 10);
		}

		/// <summary>
		/// Parses the string argument as an unsigned {@code long} in the
		/// radix specified by the second argument.  An unsigned integer
		/// maps the values usually associated with negative numbers to
		/// positive numbers larger than {@code MAX_VALUE}.
		/// 
		/// The characters in the string must all be digits of the
		/// specified radix (as determined by whether {@link
		/// java.lang.Character#digit(char, int)} returns a nonnegative
		/// value), except that the first character may be an ASCII plus
		/// sign {@code '+'} ({@code '\u005Cu002B'}). The resulting
		/// integer value is returned.
		/// 
		/// <para>An exception of type {@code NumberFormatException} is
		/// thrown if any of the following situations occurs:
		/// <ul>
		/// <li>The first argument is {@code null} or is a string of
		/// length zero.
		/// 
		/// <li>The radix is either smaller than
		/// <seealso cref="java.lang.Character#MIN_RADIX"/> or
		/// larger than <seealso cref="java.lang.Character#MAX_RADIX"/>.
		/// 
		/// <li>Any character of the string is not a digit of the specified
		/// radix, except that the first character may be a plus sign
		/// {@code '+'} ({@code '\u005Cu002B'}) provided that the
		/// string is longer than length 1.
		/// 
		/// <li>The value represented by the string is larger than the
		/// largest unsigned {@code long}, 2<sup>64</sup>-1.
		/// 
		/// </ul>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the {@code String} containing the unsigned integer
		///                  representation to be parsed </param>
		/// <param name="radix">   the radix to be used while parsing {@code s}. </param>
		/// <returns>     the unsigned {@code long} represented by the string
		///             argument in the specified radix. </returns>
		/// <exception cref="NumberFormatException"> if the {@code String}
		///             does not contain a parsable {@code long}.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long parseUnsignedLong(String s, int radix) throws NumberFormatException
		public static long ParseUnsignedLong(String s, int radix)
		{
			if (s == null)
			{
				throw new NumberFormatException("null");
			}

			int len = s.Length();
			if (len > 0)
			{
				char firstChar = s.CharAt(0);
				if (firstChar == '-')
				{
					throw new NumberFormatException(string.Format("Illegal leading minus sign " + "on unsigned string {0}.", s));
				}
				else
				{
					if (len <= 12 || (radix == 10 && len <= 18)) // Long.MAX_VALUE in base 10 is 19 digits -  Long.MAX_VALUE in Character.MAX_RADIX is 13 digits
					{
						return ParseLong(s, radix);
					}

					// No need for range checks on len due to testing above.
					long first = ParseLong(s.Substring(0, len - 1), radix);
					int second = Character.Digit(s.CharAt(len - 1), radix);
					if (second < 0)
					{
						throw new NumberFormatException("Bad digit at end of " + s);
					}
					long result = first * radix + second;
					if (CompareUnsigned(result, first) < 0)
					{
						/*
						 * The maximum unsigned value, (2^64)-1, takes at
						 * most one more digit to represent than the
						 * maximum signed value, (2^63)-1.  Therefore,
						 * parsing (len - 1) digits will be appropriately
						 * in-range of the signed parsing.  In other
						 * words, if parsing (len -1) digits overflows
						 * signed parsing, parsing len digits will
						 * certainly overflow unsigned parsing.
						 *
						 * The compareUnsigned check above catches
						 * situations where an unsigned overflow occurs
						 * incorporating the contribution of the final
						 * digit.
						 */
						throw new NumberFormatException(string.Format("String value {0} exceeds " + "range of unsigned long.", s));
					}
					return result;
				}
			}
			else
			{
				throw NumberFormatException.ForInputString(s);
			}
		}

		/// <summary>
		/// Parses the string argument as an unsigned decimal {@code long}. The
		/// characters in the string must all be decimal digits, except
		/// that the first character may be an an ASCII plus sign {@code
		/// '+'} ({@code '\u005Cu002B'}). The resulting integer value
		/// is returned, exactly as if the argument and the radix 10 were
		/// given as arguments to the {@link
		/// #parseUnsignedLong(java.lang.String, int)} method.
		/// </summary>
		/// <param name="s">   a {@code String} containing the unsigned {@code long}
		///            representation to be parsed </param>
		/// <returns>    the unsigned {@code long} value represented by the decimal string argument </returns>
		/// <exception cref="NumberFormatException">  if the string does not contain a
		///            parsable unsigned integer.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long parseUnsignedLong(String s) throws NumberFormatException
		public static long ParseUnsignedLong(String s)
		{
			return ParseUnsignedLong(s, 10);
		}

		/// <summary>
		/// Returns a {@code Long} object holding the value
		/// extracted from the specified {@code String} when parsed
		/// with the radix given by the second argument.  The first
		/// argument is interpreted as representing a signed
		/// {@code long} in the radix specified by the second
		/// argument, exactly as if the arguments were given to the {@link
		/// #parseLong(java.lang.String, int)} method. The result is a
		/// {@code Long} object that represents the {@code long}
		/// value specified by the string.
		/// 
		/// <para>In other words, this method returns a {@code Long} object equal
		/// to the value of:
		/// 
		/// <blockquote>
		///  {@code new Long(Long.parseLong(s, radix))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">       the string to be parsed </param>
		/// <param name="radix">   the radix to be used in interpreting {@code s} </param>
		/// <returns>     a {@code Long} object holding the value
		///             represented by the string argument in the specified
		///             radix. </returns>
		/// <exception cref="NumberFormatException">  If the {@code String} does not
		///             contain a parsable {@code long}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Long valueOf(String s, int radix) throws NumberFormatException
		public static Long ValueOf(String s, int radix)
		{
			return Convert.ToInt64(ParseLong(s, radix));
		}

		/// <summary>
		/// Returns a {@code Long} object holding the value
		/// of the specified {@code String}. The argument is
		/// interpreted as representing a signed decimal {@code long},
		/// exactly as if the argument were given to the {@link
		/// #parseLong(java.lang.String)} method. The result is a
		/// {@code Long} object that represents the integer value
		/// specified by the string.
		/// 
		/// <para>In other words, this method returns a {@code Long} object
		/// equal to the value of:
		/// 
		/// <blockquote>
		///  {@code new Long(Long.parseLong(s))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the string to be parsed. </param>
		/// <returns>     a {@code Long} object holding the value
		///             represented by the string argument. </returns>
		/// <exception cref="NumberFormatException">  If the string cannot be parsed
		///             as a {@code long}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Long valueOf(String s) throws NumberFormatException
		public static Long ValueOf(String s)
		{
			return Convert.ToInt64(ParseLong(s, 10));
		}

		private class LongCache
		{
			internal LongCache()
			{
			}

			internal static readonly Long[] Cache = new Long[-(-128) + 127 + 1];

			static LongCache()
			{
				for (int i = 0; i < Cache.Length; i++)
				{
					Cache[i] = new Long(i - 128);
				}
			}
		}

		/// <summary>
		/// Returns a {@code Long} instance representing the specified
		/// {@code long} value.
		/// If a new {@code Long} instance is not required, this method
		/// should generally be used in preference to the constructor
		/// <seealso cref="#Long(long)"/>, as this method is likely to yield
		/// significantly better space and time performance by caching
		/// frequently requested values.
		/// 
		/// Note that unlike the {@link Integer#valueOf(int)
		/// corresponding method} in the {@code Integer} class, this method
		/// is <em>not</em> required to cache values within a particular
		/// range.
		/// </summary>
		/// <param name="l"> a long value. </param>
		/// <returns> a {@code Long} instance representing {@code l}.
		/// @since  1.5 </returns>
		public static Long ValueOf(long l)
		{
			const int offset = 128;
			if (l >= -128 && l <= 127) // will cache
			{
				return LongCache.Cache[(int)l + offset];
			}
			return new Long(l);
		}

		/// <summary>
		/// Decodes a {@code String} into a {@code Long}.
		/// Accepts decimal, hexadecimal, and octal numbers given by the
		/// following grammar:
		/// 
		/// <blockquote>
		/// <dl>
		/// <dt><i>DecodableString:</i>
		/// <dd><i>Sign<sub>opt</sub> DecimalNumeral</i>
		/// <dd><i>Sign<sub>opt</sub></i> {@code 0x} <i>HexDigits</i>
		/// <dd><i>Sign<sub>opt</sub></i> {@code 0X} <i>HexDigits</i>
		/// <dd><i>Sign<sub>opt</sub></i> {@code #} <i>HexDigits</i>
		/// <dd><i>Sign<sub>opt</sub></i> {@code 0} <i>OctalDigits</i>
		/// 
		/// <dt><i>Sign:</i>
		/// <dd>{@code -}
		/// <dd>{@code +}
		/// </dl>
		/// </blockquote>
		/// 
		/// <i>DecimalNumeral</i>, <i>HexDigits</i>, and <i>OctalDigits</i>
		/// are as defined in section 3.10.1 of
		/// <cite>The Java&trade; Language Specification</cite>,
		/// except that underscores are not accepted between digits.
		/// 
		/// <para>The sequence of characters following an optional
		/// sign and/or radix specifier ("{@code 0x}", "{@code 0X}",
		/// "{@code #}", or leading zero) is parsed as by the {@code
		/// Long.parseLong} method with the indicated radix (10, 16, or 8).
		/// This sequence of characters must represent a positive value or
		/// a <seealso cref="NumberFormatException"/> will be thrown.  The result is
		/// negated if first character of the specified {@code String} is
		/// the minus sign.  No whitespace characters are permitted in the
		/// {@code String}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm"> the {@code String} to decode. </param>
		/// <returns>    a {@code Long} object holding the {@code long}
		///            value represented by {@code nm} </returns>
		/// <exception cref="NumberFormatException">  if the {@code String} does not
		///            contain a parsable {@code long}. </exception>
		/// <seealso cref= java.lang.Long#parseLong(String, int)
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Long decode(String nm) throws NumberFormatException
		public static Long Decode(String nm)
		{
			int radix = 10;
			int index = 0;
			bool negative = false;
			Long result;

			if (nm.Length() == 0)
			{
				throw new NumberFormatException("Zero length string");
			}
			char firstChar = nm.CharAt(0);
			// Handle sign, if present
			if (firstChar == '-')
			{
				negative = true;
				index++;
			}
			else if (firstChar == '+')
			{
				index++;
			}

			// Handle radix specifier, if present
			if (nm.StartsWith("0x", index) || nm.StartsWith("0X", index))
			{
				index += 2;
				radix = 16;
			}
			else if (nm.StartsWith("#", index))
			{
				index++;
				radix = 16;
			}
			else if (nm.StartsWith("0", index) && nm.Length() > 1 + index)
			{
				index++;
				radix = 8;
			}

			if (nm.StartsWith("-", index) || nm.StartsWith("+", index))
			{
				throw new NumberFormatException("Sign character in wrong position");
			}

			try
			{
				result = Convert.ToInt64(nm.Substring(index), radix);
				result = negative ? Convert.ToInt64(-result.LongValue()) : result;
			}
			catch (NumberFormatException)
			{
				// If number is Long.MIN_VALUE, we'll end up here. The next line
				// handles this case, and causes any genuine format error to be
				// rethrown.
				String constant = negative ? ("-" + nm.Substring(index)) : nm.Substring(index);
				result = Convert.ToInt64(constant, radix);
			}
			return result;
		}

		/// <summary>
		/// The value of the {@code Long}.
		/// 
		/// @serial
		/// </summary>
		private readonly long Value;

		/// <summary>
		/// Constructs a newly allocated {@code Long} object that
		/// represents the specified {@code long} argument.
		/// </summary>
		/// <param name="value">   the value to be represented by the
		///          {@code Long} object. </param>
		public Long(long value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Constructs a newly allocated {@code Long} object that
		/// represents the {@code long} value indicated by the
		/// {@code String} parameter. The string is converted to a
		/// {@code long} value in exactly the manner used by the
		/// {@code parseLong} method for radix 10.
		/// </summary>
		/// <param name="s">   the {@code String} to be converted to a
		///             {@code Long}. </param>
		/// <exception cref="NumberFormatException">  if the {@code String} does not
		///             contain a parsable {@code long}. </exception>
		/// <seealso cref=        java.lang.Long#parseLong(java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Long(String s) throws NumberFormatException
		public Long(String s)
		{
			this.Value = ParseLong(s, 10);
		}

		/// <summary>
		/// Returns the value of this {@code Long} as a {@code byte} after
		/// a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override sbyte ByteValue()
		{
			return (sbyte)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Long} as a {@code short} after
		/// a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override short ShortValue()
		{
			return (short)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Long} as an {@code int} after
		/// a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override int IntValue()
		{
			return (int)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Long} as a
		/// {@code long} value.
		/// </summary>
		public override long LongValue()
		{
			return Value;
		}

		/// <summary>
		/// Returns the value of this {@code Long} as a {@code float} after
		/// a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override float FloatValue()
		{
			return (float)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Long} as a {@code double}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Value;
		}

		/// <summary>
		/// Returns a {@code String} object representing this
		/// {@code Long}'s value.  The value is converted to signed
		/// decimal representation and returned as a string, exactly as if
		/// the {@code long} value were given as an argument to the
		/// <seealso cref="java.lang.Long#toString(long)"/> method.
		/// </summary>
		/// <returns>  a string representation of the value of this object in
		///          base&nbsp;10. </returns>
		public override String ToString()
		{
			return ToString(Value);
		}

		/// <summary>
		/// Returns a hash code for this {@code Long}. The result is
		/// the exclusive OR of the two halves of the primitive
		/// {@code long} value held by this {@code Long}
		/// object. That is, the hashcode is the value of the expression:
		/// 
		/// <blockquote>
		///  {@code (int)(this.longValue()^(this.longValue()>>>32))}
		/// </blockquote>
		/// </summary>
		/// <returns>  a hash code value for this object. </returns>
		public override int HashCode()
		{
			return Long.HashCode(Value);
		}

		/// <summary>
		/// Returns a hash code for a {@code long} value; compatible with
		/// {@code Long.hashCode()}.
		/// </summary>
		/// <param name="value"> the value to hash </param>
		/// <returns> a hash code value for a {@code long} value.
		/// @since 1.8 </returns>
		public static int HashCode(long value)
		{
			return (int)(value ^ ((long)((ulong)value >> 32)));
		}

		/// <summary>
		/// Compares this object to the specified object.  The result is
		/// {@code true} if and only if the argument is not
		/// {@code null} and is a {@code Long} object that
		/// contains the same {@code long} value as this object.
		/// </summary>
		/// <param name="obj">   the object to compare with. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Long)
			{
				return Value == ((Long)obj).LongValue();
			}
			return false;
		}

		/// <summary>
		/// Determines the {@code long} value of the system property
		/// with the specified name.
		/// 
		/// <para>The first argument is treated as the name of a system
		/// property.  System properties are accessible through the {@link
		/// java.lang.System#getProperty(java.lang.String)} method. The
		/// string value of this property is then interpreted as a {@code
		/// long} value using the grammar supported by <seealso cref="Long#decode decode"/>
		/// and a {@code Long} object representing this value is returned.
		/// 
		/// </para>
		/// <para>If there is no property with the specified name, if the
		/// specified name is empty or {@code null}, or if the property
		/// does not have the correct numeric format, then {@code null} is
		/// returned.
		/// 
		/// </para>
		/// <para>In other words, this method returns a {@code Long} object
		/// equal to the value of:
		/// 
		/// <blockquote>
		///  {@code getLong(nm, null)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">   property name. </param>
		/// <returns>  the {@code Long} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Long GetLong(String nm)
		{
			return GetLong(nm, null);
		}

		/// <summary>
		/// Determines the {@code long} value of the system property
		/// with the specified name.
		/// 
		/// <para>The first argument is treated as the name of a system
		/// property.  System properties are accessible through the {@link
		/// java.lang.System#getProperty(java.lang.String)} method. The
		/// string value of this property is then interpreted as a {@code
		/// long} value using the grammar supported by <seealso cref="Long#decode decode"/>
		/// and a {@code Long} object representing this value is returned.
		/// 
		/// </para>
		/// <para>The second argument is the default value. A {@code Long} object
		/// that represents the value of the second argument is returned if there
		/// is no property of the specified name, if the property does not have
		/// the correct numeric format, or if the specified name is empty or null.
		/// 
		/// </para>
		/// <para>In other words, this method returns a {@code Long} object equal
		/// to the value of:
		/// 
		/// <blockquote>
		///  {@code getLong(nm, new Long(val))}
		/// </blockquote>
		/// 
		/// but in practice it may be implemented in a manner such as:
		/// 
		/// <blockquote><pre>
		/// Long result = getLong(nm, null);
		/// return (result == null) ? new Long(val) : result;
		/// </pre></blockquote>
		/// 
		/// to avoid the unnecessary allocation of a {@code Long} object when
		/// the default value is not needed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">    property name. </param>
		/// <param name="val">   default value. </param>
		/// <returns>  the {@code Long} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Long GetLong(String nm, long val)
		{
			Long result = Long.GetLong(nm, null);
			return (result == null) ? Convert.ToInt64(val) : result;
		}

		/// <summary>
		/// Returns the {@code long} value of the system property with
		/// the specified name.  The first argument is treated as the name
		/// of a system property.  System properties are accessible through
		/// the <seealso cref="java.lang.System#getProperty(java.lang.String)"/>
		/// method. The string value of this property is then interpreted
		/// as a {@code long} value, as per the
		/// <seealso cref="Long#decode decode"/> method, and a {@code Long} object
		/// representing this value is returned; in summary:
		/// 
		/// <ul>
		/// <li>If the property value begins with the two ASCII characters
		/// {@code 0x} or the ASCII character {@code #}, not followed by
		/// a minus sign, then the rest of it is parsed as a hexadecimal integer
		/// exactly as for the method <seealso cref="#valueOf(java.lang.String, int)"/>
		/// with radix 16.
		/// <li>If the property value begins with the ASCII character
		/// {@code 0} followed by another character, it is parsed as
		/// an octal integer exactly as by the method {@link
		/// #valueOf(java.lang.String, int)} with radix 8.
		/// <li>Otherwise the property value is parsed as a decimal
		/// integer exactly as by the method
		/// <seealso cref="#valueOf(java.lang.String, int)"/> with radix 10.
		/// </ul>
		/// 
		/// <para>Note that, in every case, neither {@code L}
		/// ({@code '\u005Cu004C'}) nor {@code l}
		/// ({@code '\u005Cu006C'}) is permitted to appear at the end
		/// of the property value as a type indicator, as would be
		/// permitted in Java programming language source code.
		/// 
		/// </para>
		/// <para>The second argument is the default value. The default value is
		/// returned if there is no property of the specified name, if the
		/// property does not have the correct numeric format, or if the
		/// specified name is empty or {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">   property name. </param>
		/// <param name="val">   default value. </param>
		/// <returns>  the {@code Long} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Long GetLong(String nm, Long val)
		{
			String v = null;
			try
			{
				v = System.getProperty(nm);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalArgumentException | NullPointerException e)
			{
			}
			if (v != null)
			{
				try
				{
					return Long.Decode(v);
				}
				catch (NumberFormatException)
				{
				}
			}
			return val;
		}

		/// <summary>
		/// Compares two {@code Long} objects numerically.
		/// </summary>
		/// <param name="anotherLong">   the {@code Long} to be compared. </param>
		/// <returns>  the value {@code 0} if this {@code Long} is
		///          equal to the argument {@code Long}; a value less than
		///          {@code 0} if this {@code Long} is numerically less
		///          than the argument {@code Long}; and a value greater
		///          than {@code 0} if this {@code Long} is numerically
		///           greater than the argument {@code Long} (signed
		///           comparison).
		/// @since   1.2 </returns>
		public int CompareTo(Long anotherLong)
		{
			return Compare(this.Value, anotherLong.Value);
		}

		/// <summary>
		/// Compares two {@code long} values numerically.
		/// The value returned is identical to what would be returned by:
		/// <pre>
		///    Long.valueOf(x).compareTo(Long.valueOf(y))
		/// </pre>
		/// </summary>
		/// <param name="x"> the first {@code long} to compare </param>
		/// <param name="y"> the second {@code long} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y};
		///         a value less than {@code 0} if {@code x < y}; and
		///         a value greater than {@code 0} if {@code x > y}
		/// @since 1.7 </returns>
		public static int Compare(long x, long y)
		{
			return (x < y) ? - 1 : ((x == y) ? 0 : 1);
		}

		/// <summary>
		/// Compares two {@code long} values numerically treating the values
		/// as unsigned.
		/// </summary>
		/// <param name="x"> the first {@code long} to compare </param>
		/// <param name="y"> the second {@code long} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y}; a value less
		///         than {@code 0} if {@code x < y} as unsigned values; and
		///         a value greater than {@code 0} if {@code x > y} as
		///         unsigned values
		/// @since 1.8 </returns>
		public static int CompareUnsigned(long x, long y)
		{
			return Compare(x + MIN_VALUE, y + MIN_VALUE);
		}


		/// <summary>
		/// Returns the unsigned quotient of dividing the first argument by
		/// the second where each argument and the result is interpreted as
		/// an unsigned value.
		/// 
		/// <para>Note that in two's complement arithmetic, the three other
		/// basic arithmetic operations of add, subtract, and multiply are
		/// bit-wise identical if the two operands are regarded as both
		/// being signed or both being unsigned.  Therefore separate {@code
		/// addUnsigned}, etc. methods are not provided.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dividend"> the value to be divided </param>
		/// <param name="divisor"> the value doing the dividing </param>
		/// <returns> the unsigned quotient of the first argument divided by
		/// the second argument </returns>
		/// <seealso cref= #remainderUnsigned
		/// @since 1.8 </seealso>
		public static long DivideUnsigned(long dividend, long divisor)
		{
			if (divisor < 0L) // signed comparison
			{
				// Answer must be 0 or 1 depending on relative magnitude
				// of dividend and divisor.
				return (CompareUnsigned(dividend, divisor)) < 0 ? 0L :1L;
			}

			if (dividend > 0) //  Both inputs non-negative
			{
				return dividend / divisor;
			}
			else
			{
				/*
				 * For simple code, leveraging BigInteger.  Longer and faster
				 * code written directly in terms of operations on longs is
				 * possible; see "Hacker's Delight" for divide and remainder
				 * algorithms.
				 */
				return ToUnsignedBigInteger(dividend).Divide(ToUnsignedBigInteger(divisor)).LongValue();
			}
		}

		/// <summary>
		/// Returns the unsigned remainder from dividing the first argument
		/// by the second where each argument and the result is interpreted
		/// as an unsigned value.
		/// </summary>
		/// <param name="dividend"> the value to be divided </param>
		/// <param name="divisor"> the value doing the dividing </param>
		/// <returns> the unsigned remainder of the first argument divided by
		/// the second argument </returns>
		/// <seealso cref= #divideUnsigned
		/// @since 1.8 </seealso>
		public static long RemainderUnsigned(long dividend, long divisor)
		{
			if (dividend > 0 && divisor > 0) // signed comparisons
			{
				return dividend % divisor;
			}
			else
			{
				if (CompareUnsigned(dividend, divisor) < 0) // Avoid explicit check for 0 divisor
				{
					return dividend;
				}
				else
				{
					return ToUnsignedBigInteger(dividend).Remainder(ToUnsignedBigInteger(divisor)).LongValue();
				}
			}
		}

		// Bit Twiddling

		/// <summary>
		/// The number of bits used to represent a {@code long} value in two's
		/// complement binary form.
		/// 
		/// @since 1.5
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SIZE = 64;
		public const int SIZE = 64;

		/// <summary>
		/// The number of bytes used to represent a {@code long} value in two's
		/// complement binary form.
		/// 
		/// @since 1.8
		/// </summary>
		public static readonly int BYTES = SIZE / sizeof(sbyte);

		/// <summary>
		/// Returns a {@code long} value with at most a single one-bit, in the
		/// position of the highest-order ("leftmost") one-bit in the specified
		/// {@code long} value.  Returns zero if the specified value has no
		/// one-bits in its two's complement binary representation, that is, if it
		/// is equal to zero.
		/// </summary>
		/// <param name="i"> the value whose highest one bit is to be computed </param>
		/// <returns> a {@code long} value with a single one-bit, in the position
		///     of the highest-order one-bit in the specified value, or zero if
		///     the specified value is itself equal to zero.
		/// @since 1.5 </returns>
		public static long HighestOneBit(long i)
		{
			// HD, Figure 3-1
			i |= (i >> 1);
			i |= (i >> 2);
			i |= (i >> 4);
			i |= (i >> 8);
			i |= (i >> 16);
			i |= (i >> 32);
			return i - ((long)((ulong)i >> 1));
		}

		/// <summary>
		/// Returns a {@code long} value with at most a single one-bit, in the
		/// position of the lowest-order ("rightmost") one-bit in the specified
		/// {@code long} value.  Returns zero if the specified value has no
		/// one-bits in its two's complement binary representation, that is, if it
		/// is equal to zero.
		/// </summary>
		/// <param name="i"> the value whose lowest one bit is to be computed </param>
		/// <returns> a {@code long} value with a single one-bit, in the position
		///     of the lowest-order one-bit in the specified value, or zero if
		///     the specified value is itself equal to zero.
		/// @since 1.5 </returns>
		public static long LowestOneBit(long i)
		{
			// HD, Section 2-1
			return i & -i;
		}

		/// <summary>
		/// Returns the number of zero bits preceding the highest-order
		/// ("leftmost") one-bit in the two's complement binary representation
		/// of the specified {@code long} value.  Returns 64 if the
		/// specified value has no one-bits in its two's complement representation,
		/// in other words if it is equal to zero.
		/// 
		/// <para>Note that this method is closely related to the logarithm base 2.
		/// For all positive {@code long} values x:
		/// <ul>
		/// <li>floor(log<sub>2</sub>(x)) = {@code 63 - numberOfLeadingZeros(x)}
		/// <li>ceil(log<sub>2</sub>(x)) = {@code 64 - numberOfLeadingZeros(x - 1)}
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose number of leading zeros is to be computed </param>
		/// <returns> the number of zero bits preceding the highest-order
		///     ("leftmost") one-bit in the two's complement binary representation
		///     of the specified {@code long} value, or 64 if the value
		///     is equal to zero.
		/// @since 1.5 </returns>
		public static int NumberOfLeadingZeros(long i)
		{
			// HD, Figure 5-6
			 if (i == 0)
			 {
				return 64;
			 }
			int n = 1;
			int x = (int)((long)((ulong)i >> 32));
			if (x == 0)
			{
				n += 32;
				x = (int)i;
			}
			if ((int)((uint)x >> 16) == 0)
			{
				n += 16;
				x <<= 16;
			}
			if ((int)((uint)x >> 24) == 0)
			{
				n += 8;
				x <<= 8;
			}
			if ((int)((uint)x >> 28) == 0)
			{
				n += 4;
				x <<= 4;
			}
			if ((int)((uint)x >> 30) == 0)
			{
				n += 2;
				x <<= 2;
			}
			n -= (int)((uint)x >> 31);
			return n;
		}

		/// <summary>
		/// Returns the number of zero bits following the lowest-order ("rightmost")
		/// one-bit in the two's complement binary representation of the specified
		/// {@code long} value.  Returns 64 if the specified value has no
		/// one-bits in its two's complement representation, in other words if it is
		/// equal to zero.
		/// </summary>
		/// <param name="i"> the value whose number of trailing zeros is to be computed </param>
		/// <returns> the number of zero bits following the lowest-order ("rightmost")
		///     one-bit in the two's complement binary representation of the
		///     specified {@code long} value, or 64 if the value is equal
		///     to zero.
		/// @since 1.5 </returns>
		public static int NumberOfTrailingZeros(long i)
		{
			// HD, Figure 5-14
			int x, y;
			if (i == 0)
			{
				return 64;
			}
			int n = 63;
			y = (int)i;
			if (y != 0)
			{
				n = n - 32;
				x = y;
			}
			else
			{
				x = (int)((long)((ulong)i >> 32));
			}
			y = x << 16;
			if (y != 0)
			{
				n = n - 16;
				x = y;
			}
			y = x << 8;
			if (y != 0)
			{
				n = n - 8;
				x = y;
			}
			y = x << 4;
			if (y != 0)
			{
				n = n - 4;
				x = y;
			}
			y = x << 2;
			if (y != 0)
			{
				n = n - 2;
				x = y;
			}
			return n - ((int)((uint)(x << 1) >> 31));
		}

		/// <summary>
		/// Returns the number of one-bits in the two's complement binary
		/// representation of the specified {@code long} value.  This function is
		/// sometimes referred to as the <i>population count</i>.
		/// </summary>
		/// <param name="i"> the value whose bits are to be counted </param>
		/// <returns> the number of one-bits in the two's complement binary
		///     representation of the specified {@code long} value.
		/// @since 1.5 </returns>
		 public static int BitCount(long i)
		 {
			// HD, Figure 5-14
			i = i - (((long)((ulong)i >> 1)) & 0x5555555555555555L);
			i = (i & 0x3333333333333333L) + (((long)((ulong)i >> 2)) & 0x3333333333333333L);
			i = (i + ((long)((ulong)i >> 4))) & 0x0f0f0f0f0f0f0f0fL;
			i = i + ((long)((ulong)i >> 8));
			i = i + ((long)((ulong)i >> 16));
			i = i + ((long)((ulong)i >> 32));
			return (int)i & 0x7f;
		 }

		/// <summary>
		/// Returns the value obtained by rotating the two's complement binary
		/// representation of the specified {@code long} value left by the
		/// specified number of bits.  (Bits shifted out of the left hand, or
		/// high-order, side reenter on the right, or low-order.)
		/// 
		/// <para>Note that left rotation with a negative distance is equivalent to
		/// right rotation: {@code rotateLeft(val, -distance) == rotateRight(val,
		/// distance)}.  Note also that rotation by any multiple of 64 is a
		/// no-op, so all but the last six bits of the rotation distance can be
		/// ignored, even if the distance is negative: {@code rotateLeft(val,
		/// distance) == rotateLeft(val, distance & 0x3F)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose bits are to be rotated left </param>
		/// <param name="distance"> the number of bit positions to rotate left </param>
		/// <returns> the value obtained by rotating the two's complement binary
		///     representation of the specified {@code long} value left by the
		///     specified number of bits.
		/// @since 1.5 </returns>
		public static long RotateLeft(long i, int distance)
		{
			return (i << distance) | ((long)((ulong)i >> -distance));
		}

		/// <summary>
		/// Returns the value obtained by rotating the two's complement binary
		/// representation of the specified {@code long} value right by the
		/// specified number of bits.  (Bits shifted out of the right hand, or
		/// low-order, side reenter on the left, or high-order.)
		/// 
		/// <para>Note that right rotation with a negative distance is equivalent to
		/// left rotation: {@code rotateRight(val, -distance) == rotateLeft(val,
		/// distance)}.  Note also that rotation by any multiple of 64 is a
		/// no-op, so all but the last six bits of the rotation distance can be
		/// ignored, even if the distance is negative: {@code rotateRight(val,
		/// distance) == rotateRight(val, distance & 0x3F)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose bits are to be rotated right </param>
		/// <param name="distance"> the number of bit positions to rotate right </param>
		/// <returns> the value obtained by rotating the two's complement binary
		///     representation of the specified {@code long} value right by the
		///     specified number of bits.
		/// @since 1.5 </returns>
		public static long RotateRight(long i, int distance)
		{
			return ((long)((ulong)i >> distance)) | (i << -distance);
		}

		/// <summary>
		/// Returns the value obtained by reversing the order of the bits in the
		/// two's complement binary representation of the specified {@code long}
		/// value.
		/// </summary>
		/// <param name="i"> the value to be reversed </param>
		/// <returns> the value obtained by reversing order of the bits in the
		///     specified {@code long} value.
		/// @since 1.5 </returns>
		public static long Reverse(long i)
		{
			// HD, Figure 7-1
			i = (i & 0x5555555555555555L) << 1 | ((long)((ulong)i >> 1)) & 0x5555555555555555L;
			i = (i & 0x3333333333333333L) << 2 | ((long)((ulong)i >> 2)) & 0x3333333333333333L;
			i = (i & 0x0f0f0f0f0f0f0f0fL) << 4 | ((long)((ulong)i >> 4)) & 0x0f0f0f0f0f0f0f0fL;
			i = (i & 0x00ff00ff00ff00ffL) << 8 | ((long)((ulong)i >> 8)) & 0x00ff00ff00ff00ffL;
			i = (i << 48) | ((i & 0xffff0000L) << 16) | (((long)((ulong)i >> 16)) & 0xffff0000L) | ((long)((ulong)i >> 48));
			return i;
		}

		/// <summary>
		/// Returns the signum function of the specified {@code long} value.  (The
		/// return value is -1 if the specified value is negative; 0 if the
		/// specified value is zero; and 1 if the specified value is positive.)
		/// </summary>
		/// <param name="i"> the value whose signum is to be computed </param>
		/// <returns> the signum function of the specified {@code long} value.
		/// @since 1.5 </returns>
		public static int Signum(long i)
		{
			// HD, Section 2-7
			return (int)((i >> 63) | (int)((uint)(-i >> 63)));
		}

		/// <summary>
		/// Returns the value obtained by reversing the order of the bytes in the
		/// two's complement representation of the specified {@code long} value.
		/// </summary>
		/// <param name="i"> the value whose bytes are to be reversed </param>
		/// <returns> the value obtained by reversing the bytes in the specified
		///     {@code long} value.
		/// @since 1.5 </returns>
		public static long ReverseBytes(long i)
		{
			i = (i & 0x00ff00ff00ff00ffL) << 8 | ((long)((ulong)i >> 8)) & 0x00ff00ff00ff00ffL;
			return (i << 48) | ((i & 0xffff0000L) << 16) | (((long)((ulong)i >> 16)) & 0xffff0000L) | ((long)((ulong)i >> 48));
		}

		/// <summary>
		/// Adds two {@code long} values together as per the + operator.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the sum of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static long Sum(long a, long b)
		{
			return a + b;
		}

		/// <summary>
		/// Returns the greater of two {@code long} values
		/// as if by calling <seealso cref="Math#max(long, long) Math.max"/>.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the greater of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static long Max(long a, long b)
		{
			return System.Math.Max(a, b);
		}

		/// <summary>
		/// Returns the smaller of two {@code long} values
		/// as if by calling <seealso cref="Math#min(long, long) Math.min"/>.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the smaller of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static long Min(long a, long b)
		{
			return System.Math.Min(a, b);
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native private static final long serialVersionUID = 4290774380558885855L;
		private const long SerialVersionUID = 4290774380558885855L;
	}

}