using System;
using System.Diagnostics;

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
	/// The {@code Integer} class wraps a value of the primitive type
	/// {@code int} in an object. An object of type {@code Integer}
	/// contains a single field whose type is {@code int}.
	/// 
	/// <para>In addition, this class provides several methods for converting
	/// an {@code int} to a {@code String} and a {@code String} to an
	/// {@code int}, as well as other constants and methods useful when
	/// dealing with an {@code int}.
	/// 
	/// </para>
	/// <para>Implementation note: The implementations of the "bit twiddling"
	/// methods (such as <seealso cref="#highestOneBit(int) highestOneBit"/> and
	/// <seealso cref="#numberOfTrailingZeros(int) numberOfTrailingZeros"/>) are
	/// based on material from Henry S. Warren, Jr.'s <i>Hacker's
	/// Delight</i>, (Addison Wesley, 2002).
	/// 
	/// @author  Lee Boynton
	/// @author  Arthur van Hoff
	/// @author  Josh Bloch
	/// @author  Joseph D. Darcy
	/// @since JDK1.0
	/// </para>
	/// </summary>
	public sealed class Integer : Number, Comparable<Integer>
	{
		/// <summary>
		/// A constant holding the minimum value an {@code int} can
		/// have, -2<sup>31</sup>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int MIN_VALUE = 0x80000000;
		public const int MIN_VALUE = unchecked((int)0x80000000);

		/// <summary>
		/// A constant holding the maximum value an {@code int} can
		/// have, 2<sup>31</sup>-1.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int MAX_VALUE = 0x7fffffff;
		public const int MAX_VALUE = 0x7fffffff;

		/// <summary>
		/// The {@code Class} instance representing the primitive type
		/// {@code int}.
		/// 
		/// @since   JDK1.1
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("int");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("int");

		/// <summary>
		/// All possible chars for representing a number as a String
		/// </summary>
		internal static readonly char[] Digits = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

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
		/// result is the ASCII minus character {@code '-'}
		/// ({@code '\u005Cu002D'}). If the first argument is not
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
		/// {@code '\u005Cu007A'}. If {@code radix} is
		/// <var>N</var>, then the first <var>N</var> of these characters
		/// are used as radix-<var>N</var> digits in the order shown. Thus,
		/// the digits for hexadecimal (radix 16) are
		/// {@code 0123456789abcdef}. If uppercase letters are
		/// desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
		/// be called on the result:
		/// 
		/// <blockquote>
		///  {@code Integer.toString(n, 16).toUpperCase()}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">       an integer to be converted to a string. </param>
		/// <param name="radix">   the radix to use in the string representation. </param>
		/// <returns>  a string representation of the argument in the specified radix. </returns>
		/// <seealso cref=     java.lang.Character#MAX_RADIX </seealso>
		/// <seealso cref=     java.lang.Character#MIN_RADIX </seealso>
		public static String ToString(int i, int radix)
		{
			if (radix < Character.MIN_RADIX || radix > Character.MAX_RADIX)
			{
				radix = 10;
			}

			/* Use the faster version */
			if (radix == 10)
			{
				return ToString(i);
			}

			char[] buf = new char[33];
			bool negative = (i < 0);
			int charPos = 32;

			if (!negative)
			{
				i = -i;
			}

			while (i <= -radix)
			{
				buf[charPos--] = Digits[-(i % radix)];
				i = i / radix;
			}
			buf[charPos] = Digits[-i];

			if (negative)
			{
				buf[--charPos] = '-';
			}

			return new String(buf, charPos, (33 - charPos));
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
		/// are the same as <seealso cref="#toString(int, int) toString"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">       an integer to be converted to an unsigned string. </param>
		/// <param name="radix">   the radix to use in the string representation. </param>
		/// <returns>  an unsigned string representation of the argument in the specified radix. </returns>
		/// <seealso cref=     #toString(int, int)
		/// @since 1.8 </seealso>
		public static String ToUnsignedString(int i, int radix)
		{
			return Long.ToUnsignedString(ToUnsignedLong(i), radix);
		}

		/// <summary>
		/// Returns a string representation of the integer argument as an
		/// unsigned integer in base&nbsp;16.
		/// 
		/// <para>The unsigned integer value is the argument plus 2<sup>32</sup>
		/// if the argument is negative; otherwise, it is equal to the
		/// argument.  This value is converted to a string of ASCII digits
		/// in hexadecimal (base&nbsp;16) with no extra leading
		/// {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Integer#parseUnsignedInt(String, int)
		/// Integer.parseUnsignedInt(s, 16)}.
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
		/// {@code '\u005Cu0039'} and {@code '\u005Cu0061'} through
		/// {@code '\u005Cu0066'}. If uppercase letters are
		/// desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
		/// be called on the result:
		/// 
		/// <blockquote>
		///  {@code Integer.toHexString(n).toUpperCase()}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   an integer to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned integer value
		///          represented by the argument in hexadecimal (base&nbsp;16). </returns>
		/// <seealso cref= #parseUnsignedInt(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(int, int)
		/// @since   JDK1.0.2 </seealso>
		public static String ToHexString(int i)
		{
			return ToUnsignedString0(i, 4);
		}

		/// <summary>
		/// Returns a string representation of the integer argument as an
		/// unsigned integer in base&nbsp;8.
		/// 
		/// <para>The unsigned integer value is the argument plus 2<sup>32</sup>
		/// if the argument is negative; otherwise, it is equal to the
		/// argument.  This value is converted to a string of ASCII digits
		/// in octal (base&nbsp;8) with no extra leading {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Integer#parseUnsignedInt(String, int)
		/// Integer.parseUnsignedInt(s, 8)}.
		/// 
		/// </para>
		/// <para>If the unsigned magnitude is zero, it is represented by a
		/// single zero character {@code '0'} ({@code '\u005Cu0030'});
		/// otherwise, the first character of the representation of the
		/// unsigned magnitude will not be the zero character. The
		/// following characters are used as octal digits:
		/// 
		/// <blockquote>
		/// {@code 01234567}
		/// </blockquote>
		/// 
		/// These are the characters {@code '\u005Cu0030'} through
		/// {@code '\u005Cu0037'}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   an integer to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned integer value
		///          represented by the argument in octal (base&nbsp;8). </returns>
		/// <seealso cref= #parseUnsignedInt(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(int, int)
		/// @since   JDK1.0.2 </seealso>
		public static String ToOctalString(int i)
		{
			return ToUnsignedString0(i, 3);
		}

		/// <summary>
		/// Returns a string representation of the integer argument as an
		/// unsigned integer in base&nbsp;2.
		/// 
		/// <para>The unsigned integer value is the argument plus 2<sup>32</sup>
		/// if the argument is negative; otherwise it is equal to the
		/// argument.  This value is converted to a string of ASCII digits
		/// in binary (base&nbsp;2) with no extra leading {@code 0}s.
		/// 
		/// </para>
		/// <para>The value of the argument can be recovered from the returned
		/// string {@code s} by calling {@link
		/// Integer#parseUnsignedInt(String, int)
		/// Integer.parseUnsignedInt(s, 2)}.
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
		/// <param name="i">   an integer to be converted to a string. </param>
		/// <returns>  the string representation of the unsigned integer value
		///          represented by the argument in binary (base&nbsp;2). </returns>
		/// <seealso cref= #parseUnsignedInt(String, int) </seealso>
		/// <seealso cref= #toUnsignedString(int, int)
		/// @since   JDK1.0.2 </seealso>
		public static String ToBinaryString(int i)
		{
			return ToUnsignedString0(i, 1);
		}

		/// <summary>
		/// Convert the integer to an unsigned number.
		/// </summary>
		private static String ToUnsignedString0(int val, int shift)
		{
			// assert shift > 0 && shift <=5 : "Illegal shift value";
			int mag = sizeof(int) - Integer.NumberOfLeadingZeros(val);
			int chars = System.Math.Max(((mag + (shift - 1)) / shift), 1);
			char[] buf = new char[chars];

			FormatUnsignedInt(val, shift, buf, 0, chars);

			// Use special constructor which takes over "buf".
			return new String(buf, true);
		}

		/// <summary>
		/// Format a long (treated as unsigned) into a character buffer. </summary>
		/// <param name="val"> the unsigned int to format </param>
		/// <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
		/// <param name="buf"> the character buffer to write to </param>
		/// <param name="offset"> the offset in the destination buffer to start at </param>
		/// <param name="len"> the number of characters to write </param>
		/// <returns> the lowest character  location used </returns>
		 internal static int FormatUnsignedInt(int val, int shift, char[] buf, int offset, int len)
		 {
			int charPos = len;
			int radix = 1 << shift;
			int mask = radix - 1;
			do
			{
				buf[offset + --charPos] = Integer.Digits[val & mask];
				val = (int)((uint)val >> shift);
			} while (val != 0 && charPos > 0);

			return charPos;
		 }

		internal static readonly char[] DigitTens = new char[] {'0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9'};

		internal static readonly char[] DigitOnes = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

			// I use the "invariant division by multiplication" trick to
			// accelerate Integer.toString.  In particular we want to
			// avoid division by 10.
			//
			// The "trick" has roughly the same performance characteristics
			// as the "classic" Integer.toString code on a non-JIT VM.
			// The trick avoids .rem and .div calls but has a longer code
			// path and is thus dominated by dispatch overhead.  In the
			// JIT case the dispatch overhead doesn't exist and the
			// "trick" is considerably faster than the classic code.
			//
			// TODO-FIXME: convert (x * 52429) into the equiv shift-add
			// sequence.
			//
			// RE:  Division by Invariant Integers using Multiplication
			//      T Gralund, P Montgomery
			//      ACM PLDI 1994
			//

		/// <summary>
		/// Returns a {@code String} object representing the
		/// specified integer. The argument is converted to signed decimal
		/// representation and returned as a string, exactly as if the
		/// argument and radix 10 were given as arguments to the {@link
		/// #toString(int, int)} method.
		/// </summary>
		/// <param name="i">   an integer to be converted. </param>
		/// <returns>  a string representation of the argument in base&nbsp;10. </returns>
		public static String ToString(int i)
		{
			if (i == Integer.MinValue)
			{
				return "-2147483648";
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
		/// 10 were given as arguments to the {@link #toUnsignedString(int,
		/// int)} method.
		/// </summary>
		/// <param name="i">  an integer to be converted to an unsigned string. </param>
		/// <returns>  an unsigned string representation of the argument. </returns>
		/// <seealso cref=     #toUnsignedString(int, int)
		/// @since 1.8 </seealso>
		public static String ToUnsignedString(int i)
		{
			return Convert.ToString(ToUnsignedLong(i));
		}

		/// <summary>
		/// Places characters representing the integer i into the
		/// character array buf. The characters are placed into
		/// the buffer backwards starting with the least significant
		/// digit at the specified index (exclusive), and working
		/// backwards from there.
		/// 
		/// Will fail if i == Integer.MIN_VALUE
		/// </summary>
		internal static void GetChars(int i, int index, char[] buf)
		{
			int q, r;
			int charPos = index;
			char sign = (char)0;

			if (i < 0)
			{
				sign = '-';
				i = -i;
			}

			// Generate two digits per iteration
			while (i >= 65536)
			{
				q = i / 100;
			// really: r = i - (q * 100);
				r = i - ((q << 6) + (q << 5) + (q << 2));
				i = q;
				buf [--charPos] = DigitOnes[r];
				buf [--charPos] = DigitTens[r];
			}

			// Fall thru to fast mode for smaller numbers
			// assert(i <= 65536, i);
			for (;;)
			{
				q = (int)((uint)(i * 52429) >> (16 + 3));
				r = i - ((q << 3) + (q << 1)); // r = i-(q*10) ...
				buf [--charPos] = Digits [r];
				i = q;
				if (i == 0)
				{
					break;
				}
			}
			if (sign != 0)
			{
				buf [--charPos] = sign;
			}
		}

		internal static readonly int[] SizeTable = new int[] {9, 99, 999, 9999, 99999, 999999, 9999999, 99999999, 999999999, Integer.MaxValue};

		// Requires positive x
		internal static int StringSize(int x)
		{
			for (int i = 0; ; i++)
			{
				if (x <= SizeTable[i])
				{
					return i + 1;
				}
			}
		}

		/// <summary>
		/// Parses the string argument as a signed integer in the radix
		/// specified by the second argument. The characters in the string
		/// must all be digits of the specified radix (as determined by
		/// whether <seealso cref="java.lang.Character#digit(char, int)"/> returns a
		/// nonnegative value), except that the first character may be an
		/// ASCII minus sign {@code '-'} ({@code '\u005Cu002D'}) to
		/// indicate a negative value or an ASCII plus sign {@code '+'}
		/// ({@code '\u005Cu002B'}) to indicate a positive value. The
		/// resulting integer value is returned.
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
		/// radix, except that the first character may be a minus sign
		/// {@code '-'} ({@code '\u005Cu002D'}) or plus sign
		/// {@code '+'} ({@code '\u005Cu002B'}) provided that the
		/// string is longer than length 1.
		/// 
		/// <li>The value represented by the string is not a value of type
		/// {@code int}.
		/// </ul>
		/// 
		/// </para>
		/// <para>Examples:
		/// <blockquote><pre>
		/// parseInt("0", 10) returns 0
		/// parseInt("473", 10) returns 473
		/// parseInt("+42", 10) returns 42
		/// parseInt("-0", 10) returns 0
		/// parseInt("-FF", 16) returns -255
		/// parseInt("1100110", 2) returns 102
		/// parseInt("2147483647", 10) returns 2147483647
		/// parseInt("-2147483648", 10) returns -2147483648
		/// parseInt("2147483648", 10) throws a NumberFormatException
		/// parseInt("99", 8) throws a NumberFormatException
		/// parseInt("Kona", 10) throws a NumberFormatException
		/// parseInt("Kona", 27) returns 411787
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the {@code String} containing the integer
		///                  representation to be parsed </param>
		/// <param name="radix">   the radix to be used while parsing {@code s}. </param>
		/// <returns>     the integer represented by the string argument in the
		///             specified radix. </returns>
		/// <exception cref="NumberFormatException"> if the {@code String}
		///             does not contain a parsable {@code int}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int parseInt(String s, int radix) throws NumberFormatException
		public static int ParseInt(String s, int radix)
		{
			/*
			 * WARNING: This method may be invoked early during VM initialization
			 * before IntegerCache is initialized. Care must be taken to not use
			 * the valueOf method.
			 */

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

			int result = 0;
			bool negative = false;
			int i = 0, len = s.Length();
			int limit = -Integer.MaxValue;
			int multmin;
			int digit;

			if (len > 0)
			{
				char firstChar = s.CharAt(0);
				if (firstChar < '0') // Possible leading "+" or "-"
				{
					if (firstChar == '-')
					{
						negative = true;
						limit = Integer.MinValue;
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
		/// Parses the string argument as a signed decimal integer. The
		/// characters in the string must all be decimal digits, except
		/// that the first character may be an ASCII minus sign {@code '-'}
		/// ({@code '\u005Cu002D'}) to indicate a negative value or an
		/// ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
		/// indicate a positive value. The resulting integer value is
		/// returned, exactly as if the argument and the radix 10 were
		/// given as arguments to the {@link #parseInt(java.lang.String,
		/// int)} method.
		/// </summary>
		/// <param name="s">    a {@code String} containing the {@code int}
		///             representation to be parsed </param>
		/// <returns>     the integer value represented by the argument in decimal. </returns>
		/// <exception cref="NumberFormatException">  if the string does not contain a
		///               parsable integer. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int parseInt(String s) throws NumberFormatException
		public static int ParseInt(String s)
		{
			return ParseInt(s,10);
		}

		/// <summary>
		/// Parses the string argument as an unsigned integer in the radix
		/// specified by the second argument.  An unsigned integer maps the
		/// values usually associated with negative numbers to positive
		/// numbers larger than {@code MAX_VALUE}.
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
		/// largest unsigned {@code int}, 2<sup>32</sup>-1.
		/// 
		/// </ul>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the {@code String} containing the unsigned integer
		///                  representation to be parsed </param>
		/// <param name="radix">   the radix to be used while parsing {@code s}. </param>
		/// <returns>     the integer represented by the string argument in the
		///             specified radix. </returns>
		/// <exception cref="NumberFormatException"> if the {@code String}
		///             does not contain a parsable {@code int}.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int parseUnsignedInt(String s, int radix) throws NumberFormatException
		public static int ParseUnsignedInt(String s, int radix)
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
					if (len <= 5 || (radix == 10 && len <= 9)) // Integer.MAX_VALUE in base 10 is 10 digits -  Integer.MAX_VALUE in Character.MAX_RADIX is 6 digits
					{
						return ParseInt(s, radix);
					}
					else
					{
						long ell = Convert.ToInt64(s, radix);
						if ((ell & 0xffff_ffff_0000_0000L) == 0)
						{
							return (int) ell;
						}
						else
						{
							throw new NumberFormatException(string.Format("String value {0} exceeds " + "range of unsigned int.", s));
						}
					}
				}
			}
			else
			{
				throw NumberFormatException.ForInputString(s);
			}
		}

		/// <summary>
		/// Parses the string argument as an unsigned decimal integer. The
		/// characters in the string must all be decimal digits, except
		/// that the first character may be an an ASCII plus sign {@code
		/// '+'} ({@code '\u005Cu002B'}). The resulting integer value
		/// is returned, exactly as if the argument and the radix 10 were
		/// given as arguments to the {@link
		/// #parseUnsignedInt(java.lang.String, int)} method.
		/// </summary>
		/// <param name="s">   a {@code String} containing the unsigned {@code int}
		///            representation to be parsed </param>
		/// <returns>    the unsigned integer value represented by the argument in decimal. </returns>
		/// <exception cref="NumberFormatException">  if the string does not contain a
		///            parsable unsigned integer.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int parseUnsignedInt(String s) throws NumberFormatException
		public static int ParseUnsignedInt(String s)
		{
			return ParseUnsignedInt(s, 10);
		}

		/// <summary>
		/// Returns an {@code Integer} object holding the value
		/// extracted from the specified {@code String} when parsed
		/// with the radix given by the second argument. The first argument
		/// is interpreted as representing a signed integer in the radix
		/// specified by the second argument, exactly as if the arguments
		/// were given to the <seealso cref="#parseInt(java.lang.String, int)"/>
		/// method. The result is an {@code Integer} object that
		/// represents the integer value specified by the string.
		/// 
		/// <para>In other words, this method returns an {@code Integer}
		/// object equal to the value of:
		/// 
		/// <blockquote>
		///  {@code new Integer(Integer.parseInt(s, radix))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the string to be parsed. </param>
		/// <param name="radix"> the radix to be used in interpreting {@code s} </param>
		/// <returns>     an {@code Integer} object holding the value
		///             represented by the string argument in the specified
		///             radix. </returns>
		/// <exception cref="NumberFormatException"> if the {@code String}
		///            does not contain a parsable {@code int}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Integer valueOf(String s, int radix) throws NumberFormatException
		public static Integer ValueOf(String s, int radix)
		{
			return Convert.ToInt32(ParseInt(s,radix));
		}

		/// <summary>
		/// Returns an {@code Integer} object holding the
		/// value of the specified {@code String}. The argument is
		/// interpreted as representing a signed decimal integer, exactly
		/// as if the argument were given to the {@link
		/// #parseInt(java.lang.String)} method. The result is an
		/// {@code Integer} object that represents the integer value
		/// specified by the string.
		/// 
		/// <para>In other words, this method returns an {@code Integer}
		/// object equal to the value of:
		/// 
		/// <blockquote>
		///  {@code new Integer(Integer.parseInt(s))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   the string to be parsed. </param>
		/// <returns>     an {@code Integer} object holding the value
		///             represented by the string argument. </returns>
		/// <exception cref="NumberFormatException">  if the string cannot be parsed
		///             as an integer. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Integer valueOf(String s) throws NumberFormatException
		public static Integer ValueOf(String s)
		{
			return Convert.ToInt32(ParseInt(s, 10));
		}

		/// <summary>
		/// Cache to support the object identity semantics of autoboxing for values between
		/// -128 and 127 (inclusive) as required by JLS.
		/// 
		/// The cache is initialized on first usage.  The size of the cache
		/// may be controlled by the {@code -XX:AutoBoxCacheMax=<size>} option.
		/// During VM initialization, java.lang.Integer.IntegerCache.high property
		/// may be set and saved in the private system properties in the
		/// sun.misc.VM class.
		/// </summary>

		private class IntegerCache
		{
			internal const int Low = -128;
			internal static readonly int High;
			internal static readonly Integer[] Cache;

			static IntegerCache()
			{
				// high value may be configured by property
				int h = 127;
				String integerCacheHighPropValue = sun.misc.VM.getSavedProperty("java.lang.Integer.IntegerCache.high");
				if (integerCacheHighPropValue != null)
				{
					try
					{
						int i = ParseInt(integerCacheHighPropValue);
						i = System.Math.Max(i, 127);
						// Maximum array size is Integer.MAX_VALUE
						h = System.Math.Min(i, Integer.MaxValue - (-Low) - 1);
					}
					catch (NumberFormatException)
					{
						// If the property cannot be parsed into an int, ignore it.
					}
				}
				High = h;

				Cache = new Integer[(High - Low) + 1];
				int j = Low;
				for (int k = 0; k < Cache.Length; k++)
				{
					Cache[k] = new Integer(j++);
				}

				// range [-128, 127] must be interned (JLS7 5.1.7)
				Debug.Assert(IntegerCache.High >= 127);
			}

			internal IntegerCache()
			{
			}
		}

		/// <summary>
		/// Returns an {@code Integer} instance representing the specified
		/// {@code int} value.  If a new {@code Integer} instance is not
		/// required, this method should generally be used in preference to
		/// the constructor <seealso cref="#Integer(int)"/>, as this method is likely
		/// to yield significantly better space and time performance by
		/// caching frequently requested values.
		/// 
		/// This method will always cache values in the range -128 to 127,
		/// inclusive, and may cache other values outside of this range.
		/// </summary>
		/// <param name="i"> an {@code int} value. </param>
		/// <returns> an {@code Integer} instance representing {@code i}.
		/// @since  1.5 </returns>
		public static Integer ValueOf(int i)
		{
			if (i >= IntegerCache.Low && i <= IntegerCache.High)
			{
				return IntegerCache.Cache[i + (-IntegerCache.Low)];
			}
			return new Integer(i);
		}

		/// <summary>
		/// The value of the {@code Integer}.
		/// 
		/// @serial
		/// </summary>
		private readonly int Value;

		/// <summary>
		/// Constructs a newly allocated {@code Integer} object that
		/// represents the specified {@code int} value.
		/// </summary>
		/// <param name="value">   the value to be represented by the
		///                  {@code Integer} object. </param>
		public Integer(int value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Constructs a newly allocated {@code Integer} object that
		/// represents the {@code int} value indicated by the
		/// {@code String} parameter. The string is converted to an
		/// {@code int} value in exactly the manner used by the
		/// {@code parseInt} method for radix 10.
		/// </summary>
		/// <param name="s">   the {@code String} to be converted to an
		///                 {@code Integer}. </param>
		/// <exception cref="NumberFormatException">  if the {@code String} does not
		///               contain a parsable integer. </exception>
		/// <seealso cref=        java.lang.Integer#parseInt(java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Integer(String s) throws NumberFormatException
		public Integer(String s)
		{
			this.Value = ParseInt(s, 10);
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as a {@code byte}
		/// after a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override sbyte ByteValue()
		{
			return (sbyte)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as a {@code short}
		/// after a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override short ShortValue()
		{
			return (short)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as an
		/// {@code int}.
		/// </summary>
		public override int IntValue()
		{
			return Value;
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as a {@code long}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions </summary>
		/// <seealso cref= Integer#toUnsignedLong(int) </seealso>
		public override long LongValue()
		{
			return (long)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as a {@code float}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override float FloatValue()
		{
			return (float)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Integer} as a {@code double}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Value;
		}

		/// <summary>
		/// Returns a {@code String} object representing this
		/// {@code Integer}'s value. The value is converted to signed
		/// decimal representation and returned as a string, exactly as if
		/// the integer value were given as an argument to the {@link
		/// java.lang.Integer#toString(int)} method.
		/// </summary>
		/// <returns>  a string representation of the value of this object in
		///          base&nbsp;10. </returns>
		public override String ToString()
		{
			return ToString(Value);
		}

		/// <summary>
		/// Returns a hash code for this {@code Integer}.
		/// </summary>
		/// <returns>  a hash code value for this object, equal to the
		///          primitive {@code int} value represented by this
		///          {@code Integer} object. </returns>
		public override int HashCode()
		{
			return Integer.HashCode(Value);
		}

		/// <summary>
		/// Returns a hash code for a {@code int} value; compatible with
		/// {@code Integer.hashCode()}.
		/// </summary>
		/// <param name="value"> the value to hash
		/// @since 1.8
		/// </param>
		/// <returns> a hash code value for a {@code int} value. </returns>
		public static int HashCode(int value)
		{
			return value;
		}

		/// <summary>
		/// Compares this object to the specified object.  The result is
		/// {@code true} if and only if the argument is not
		/// {@code null} and is an {@code Integer} object that
		/// contains the same {@code int} value as this object.
		/// </summary>
		/// <param name="obj">   the object to compare with. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Integer)
			{
				return Value == ((Integer)obj).IntValue();
			}
			return false;
		}

		/// <summary>
		/// Determines the integer value of the system property with the
		/// specified name.
		/// 
		/// <para>The first argument is treated as the name of a system
		/// property.  System properties are accessible through the {@link
		/// java.lang.System#getProperty(java.lang.String)} method. The
		/// string value of this property is then interpreted as an integer
		/// value using the grammar supported by <seealso cref="Integer#decode decode"/> and
		/// an {@code Integer} object representing this value is returned.
		/// 
		/// </para>
		/// <para>If there is no property with the specified name, if the
		/// specified name is empty or {@code null}, or if the property
		/// does not have the correct numeric format, then {@code null} is
		/// returned.
		/// 
		/// </para>
		/// <para>In other words, this method returns an {@code Integer}
		/// object equal to the value of:
		/// 
		/// <blockquote>
		///  {@code getInteger(nm, null)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">   property name. </param>
		/// <returns>  the {@code Integer} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Integer GetInteger(String nm)
		{
			return GetInteger(nm, null);
		}

		/// <summary>
		/// Determines the integer value of the system property with the
		/// specified name.
		/// 
		/// <para>The first argument is treated as the name of a system
		/// property.  System properties are accessible through the {@link
		/// java.lang.System#getProperty(java.lang.String)} method. The
		/// string value of this property is then interpreted as an integer
		/// value using the grammar supported by <seealso cref="Integer#decode decode"/> and
		/// an {@code Integer} object representing this value is returned.
		/// 
		/// </para>
		/// <para>The second argument is the default value. An {@code Integer} object
		/// that represents the value of the second argument is returned if there
		/// is no property of the specified name, if the property does not have
		/// the correct numeric format, or if the specified name is empty or
		/// {@code null}.
		/// 
		/// </para>
		/// <para>In other words, this method returns an {@code Integer} object
		/// equal to the value of:
		/// 
		/// <blockquote>
		///  {@code getInteger(nm, new Integer(val))}
		/// </blockquote>
		/// 
		/// but in practice it may be implemented in a manner such as:
		/// 
		/// <blockquote><pre>
		/// Integer result = getInteger(nm, null);
		/// return (result == null) ? new Integer(val) : result;
		/// </pre></blockquote>
		/// 
		/// to avoid the unnecessary allocation of an {@code Integer}
		/// object when the default value is not needed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">   property name. </param>
		/// <param name="val">   default value. </param>
		/// <returns>  the {@code Integer} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Integer GetInteger(String nm, int val)
		{
			Integer result = GetInteger(nm, null);
			return (result == null) ? Convert.ToInt32(val) : result;
		}

		/// <summary>
		/// Returns the integer value of the system property with the
		/// specified name.  The first argument is treated as the name of a
		/// system property.  System properties are accessible through the
		/// <seealso cref="java.lang.System#getProperty(java.lang.String)"/> method.
		/// The string value of this property is then interpreted as an
		/// integer value, as per the <seealso cref="Integer#decode decode"/> method,
		/// and an {@code Integer} object representing this value is
		/// returned; in summary:
		/// 
		/// <ul><li>If the property value begins with the two ASCII characters
		///         {@code 0x} or the ASCII character {@code #}, not
		///      followed by a minus sign, then the rest of it is parsed as a
		///      hexadecimal integer exactly as by the method
		///      <seealso cref="#valueOf(java.lang.String, int)"/> with radix 16.
		/// <li>If the property value begins with the ASCII character
		///     {@code 0} followed by another character, it is parsed as an
		///     octal integer exactly as by the method
		///     <seealso cref="#valueOf(java.lang.String, int)"/> with radix 8.
		/// <li>Otherwise, the property value is parsed as a decimal integer
		/// exactly as by the method <seealso cref="#valueOf(java.lang.String, int)"/>
		/// with radix 10.
		/// </ul>
		/// 
		/// <para>The second argument is the default value. The default value is
		/// returned if there is no property of the specified name, if the
		/// property does not have the correct numeric format, or if the
		/// specified name is empty or {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm">   property name. </param>
		/// <param name="val">   default value. </param>
		/// <returns>  the {@code Integer} value of the property. </returns>
		/// <exception cref="SecurityException"> for the same reasons as
		///          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
		/// <seealso cref=     System#getProperty(java.lang.String) </seealso>
		/// <seealso cref=     System#getProperty(java.lang.String, java.lang.String) </seealso>
		public static Integer GetInteger(String nm, Integer val)
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
					return Integer.Decode(v);
				}
				catch (NumberFormatException)
				{
				}
			}
			return val;
		}

		/// <summary>
		/// Decodes a {@code String} into an {@code Integer}.
		/// Accepts decimal, hexadecimal, and octal numbers given
		/// by the following grammar:
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
		/// Integer.parseInt} method with the indicated radix (10, 16, or
		/// 8).  This sequence of characters must represent a positive
		/// value or a <seealso cref="NumberFormatException"/> will be thrown.  The
		/// result is negated if first character of the specified {@code
		/// String} is the minus sign.  No whitespace characters are
		/// permitted in the {@code String}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm"> the {@code String} to decode. </param>
		/// <returns>    an {@code Integer} object holding the {@code int}
		///             value represented by {@code nm} </returns>
		/// <exception cref="NumberFormatException">  if the {@code String} does not
		///            contain a parsable integer. </exception>
		/// <seealso cref= java.lang.Integer#parseInt(java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Integer decode(String nm) throws NumberFormatException
		public static Integer Decode(String nm)
		{
			int radix = 10;
			int index = 0;
			bool negative = false;
			Integer result;

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
				result = Convert.ToInt32(nm.Substring(index), radix);
				result = negative ? Convert.ToInt32(-result.IntValue()) : result;
			}
			catch (NumberFormatException)
			{
				// If number is Integer.MIN_VALUE, we'll end up here. The next line
				// handles this case, and causes any genuine format error to be
				// rethrown.
				String constant = negative ? ("-" + nm.Substring(index)) : nm.Substring(index);
				result = Convert.ToInt32(constant, radix);
			}
			return result;
		}

		/// <summary>
		/// Compares two {@code Integer} objects numerically.
		/// </summary>
		/// <param name="anotherInteger">   the {@code Integer} to be compared. </param>
		/// <returns>  the value {@code 0} if this {@code Integer} is
		///          equal to the argument {@code Integer}; a value less than
		///          {@code 0} if this {@code Integer} is numerically less
		///          than the argument {@code Integer}; and a value greater
		///          than {@code 0} if this {@code Integer} is numerically
		///           greater than the argument {@code Integer} (signed
		///           comparison).
		/// @since   1.2 </returns>
		public int CompareTo(Integer anotherInteger)
		{
			return Compare(this.Value, anotherInteger.Value);
		}

		/// <summary>
		/// Compares two {@code int} values numerically.
		/// The value returned is identical to what would be returned by:
		/// <pre>
		///    Integer.valueOf(x).compareTo(Integer.valueOf(y))
		/// </pre>
		/// </summary>
		/// <param name="x"> the first {@code int} to compare </param>
		/// <param name="y"> the second {@code int} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y};
		///         a value less than {@code 0} if {@code x < y}; and
		///         a value greater than {@code 0} if {@code x > y}
		/// @since 1.7 </returns>
		public static int Compare(int x, int y)
		{
			return (x < y) ? - 1 : ((x == y) ? 0 : 1);
		}

		/// <summary>
		/// Compares two {@code int} values numerically treating the values
		/// as unsigned.
		/// </summary>
		/// <param name="x"> the first {@code int} to compare </param>
		/// <param name="y"> the second {@code int} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y}; a value less
		///         than {@code 0} if {@code x < y} as unsigned values; and
		///         a value greater than {@code 0} if {@code x > y} as
		///         unsigned values
		/// @since 1.8 </returns>
		public static int CompareUnsigned(int x, int y)
		{
			return Compare(x + MIN_VALUE, y + MIN_VALUE);
		}

		/// <summary>
		/// Converts the argument to a {@code long} by an unsigned
		/// conversion.  In an unsigned conversion to a {@code long}, the
		/// high-order 32 bits of the {@code long} are zero and the
		/// low-order 32 bits are equal to the bits of the integer
		/// argument.
		/// 
		/// Consequently, zero and positive {@code int} values are mapped
		/// to a numerically equal {@code long} value and negative {@code
		/// int} values are mapped to a {@code long} value equal to the
		/// input plus 2<sup>32</sup>.
		/// </summary>
		/// <param name="x"> the value to convert to an unsigned {@code long} </param>
		/// <returns> the argument converted to {@code long} by an unsigned
		///         conversion
		/// @since 1.8 </returns>
		public static long ToUnsignedLong(int x)
		{
			return ((long) x) & 0xffffffffL;
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
		public static int DivideUnsigned(int dividend, int divisor)
		{
			// In lieu of tricky code, for now just use long arithmetic.
			return (int)(ToUnsignedLong(dividend) / ToUnsignedLong(divisor));
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
		public static int RemainderUnsigned(int dividend, int divisor)
		{
			// In lieu of tricky code, for now just use long arithmetic.
			return (int)(ToUnsignedLong(dividend) % ToUnsignedLong(divisor));
		}


		// Bit twiddling

		/// <summary>
		/// The number of bits used to represent an {@code int} value in two's
		/// complement binary form.
		/// 
		/// @since 1.5
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int SIZE = 32;
		public const int SIZE = 32;

		/// <summary>
		/// The number of bytes used to represent a {@code int} value in two's
		/// complement binary form.
		/// 
		/// @since 1.8
		/// </summary>
		public static readonly int BYTES = SIZE / sizeof(sbyte);

		/// <summary>
		/// Returns an {@code int} value with at most a single one-bit, in the
		/// position of the highest-order ("leftmost") one-bit in the specified
		/// {@code int} value.  Returns zero if the specified value has no
		/// one-bits in its two's complement binary representation, that is, if it
		/// is equal to zero.
		/// </summary>
		/// <param name="i"> the value whose highest one bit is to be computed </param>
		/// <returns> an {@code int} value with a single one-bit, in the position
		///     of the highest-order one-bit in the specified value, or zero if
		///     the specified value is itself equal to zero.
		/// @since 1.5 </returns>
		public static int HighestOneBit(int i)
		{
			// HD, Figure 3-1
			i |= (i >> 1);
			i |= (i >> 2);
			i |= (i >> 4);
			i |= (i >> 8);
			i |= (i >> 16);
			return i - ((int)((uint)i >> 1));
		}

		/// <summary>
		/// Returns an {@code int} value with at most a single one-bit, in the
		/// position of the lowest-order ("rightmost") one-bit in the specified
		/// {@code int} value.  Returns zero if the specified value has no
		/// one-bits in its two's complement binary representation, that is, if it
		/// is equal to zero.
		/// </summary>
		/// <param name="i"> the value whose lowest one bit is to be computed </param>
		/// <returns> an {@code int} value with a single one-bit, in the position
		///     of the lowest-order one-bit in the specified value, or zero if
		///     the specified value is itself equal to zero.
		/// @since 1.5 </returns>
		public static int LowestOneBit(int i)
		{
			// HD, Section 2-1
			return i & -i;
		}

		/// <summary>
		/// Returns the number of zero bits preceding the highest-order
		/// ("leftmost") one-bit in the two's complement binary representation
		/// of the specified {@code int} value.  Returns 32 if the
		/// specified value has no one-bits in its two's complement representation,
		/// in other words if it is equal to zero.
		/// 
		/// <para>Note that this method is closely related to the logarithm base 2.
		/// For all positive {@code int} values x:
		/// <ul>
		/// <li>floor(log<sub>2</sub>(x)) = {@code 31 - numberOfLeadingZeros(x)}
		/// <li>ceil(log<sub>2</sub>(x)) = {@code 32 - numberOfLeadingZeros(x - 1)}
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose number of leading zeros is to be computed </param>
		/// <returns> the number of zero bits preceding the highest-order
		///     ("leftmost") one-bit in the two's complement binary representation
		///     of the specified {@code int} value, or 32 if the value
		///     is equal to zero.
		/// @since 1.5 </returns>
		public static int NumberOfLeadingZeros(int i)
		{
			// HD, Figure 5-6
			if (i == 0)
			{
				return 32;
			}
			int n = 1;
			if ((int)((uint)i >> 16) == 0)
			{
				n += 16;
				i <<= 16;
			}
			if ((int)((uint)i >> 24) == 0)
			{
				n += 8;
				i <<= 8;
			}
			if ((int)((uint)i >> 28) == 0)
			{
				n += 4;
				i <<= 4;
			}
			if ((int)((uint)i >> 30) == 0)
			{
				n += 2;
				i <<= 2;
			}
			n -= (int)((uint)i >> 31);
			return n;
		}

		/// <summary>
		/// Returns the number of zero bits following the lowest-order ("rightmost")
		/// one-bit in the two's complement binary representation of the specified
		/// {@code int} value.  Returns 32 if the specified value has no
		/// one-bits in its two's complement representation, in other words if it is
		/// equal to zero.
		/// </summary>
		/// <param name="i"> the value whose number of trailing zeros is to be computed </param>
		/// <returns> the number of zero bits following the lowest-order ("rightmost")
		///     one-bit in the two's complement binary representation of the
		///     specified {@code int} value, or 32 if the value is equal
		///     to zero.
		/// @since 1.5 </returns>
		public static int NumberOfTrailingZeros(int i)
		{
			// HD, Figure 5-14
			int y;
			if (i == 0)
			{
				return 32;
			}
			int n = 31;
			y = i << 16;
			if (y != 0)
			{
				n = n - 16;
				i = y;
			}
			y = i << 8;
			if (y != 0)
			{
				n = n - 8;
				i = y;
			}
			y = i << 4;
			if (y != 0)
			{
				n = n - 4;
				i = y;
			}
			y = i << 2;
			if (y != 0)
			{
				n = n - 2;
				i = y;
			}
			return n - ((int)((uint)(i << 1) >> 31));
		}

		/// <summary>
		/// Returns the number of one-bits in the two's complement binary
		/// representation of the specified {@code int} value.  This function is
		/// sometimes referred to as the <i>population count</i>.
		/// </summary>
		/// <param name="i"> the value whose bits are to be counted </param>
		/// <returns> the number of one-bits in the two's complement binary
		///     representation of the specified {@code int} value.
		/// @since 1.5 </returns>
		public static int BitCount(int i)
		{
			// HD, Figure 5-2
			i = i - (((int)((uint)i >> 1)) & 0x55555555);
			i = (i & 0x33333333) + (((int)((uint)i >> 2)) & 0x33333333);
			i = (i + ((int)((uint)i >> 4))) & 0x0f0f0f0f;
			i = i + ((int)((uint)i >> 8));
			i = i + ((int)((uint)i >> 16));
			return i & 0x3f;
		}

		/// <summary>
		/// Returns the value obtained by rotating the two's complement binary
		/// representation of the specified {@code int} value left by the
		/// specified number of bits.  (Bits shifted out of the left hand, or
		/// high-order, side reenter on the right, or low-order.)
		/// 
		/// <para>Note that left rotation with a negative distance is equivalent to
		/// right rotation: {@code rotateLeft(val, -distance) == rotateRight(val,
		/// distance)}.  Note also that rotation by any multiple of 32 is a
		/// no-op, so all but the last five bits of the rotation distance can be
		/// ignored, even if the distance is negative: {@code rotateLeft(val,
		/// distance) == rotateLeft(val, distance & 0x1F)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose bits are to be rotated left </param>
		/// <param name="distance"> the number of bit positions to rotate left </param>
		/// <returns> the value obtained by rotating the two's complement binary
		///     representation of the specified {@code int} value left by the
		///     specified number of bits.
		/// @since 1.5 </returns>
		public static int RotateLeft(int i, int distance)
		{
			return (i << distance) | ((int)((uint)i >> -distance));
		}

		/// <summary>
		/// Returns the value obtained by rotating the two's complement binary
		/// representation of the specified {@code int} value right by the
		/// specified number of bits.  (Bits shifted out of the right hand, or
		/// low-order, side reenter on the left, or high-order.)
		/// 
		/// <para>Note that right rotation with a negative distance is equivalent to
		/// left rotation: {@code rotateRight(val, -distance) == rotateLeft(val,
		/// distance)}.  Note also that rotation by any multiple of 32 is a
		/// no-op, so all but the last five bits of the rotation distance can be
		/// ignored, even if the distance is negative: {@code rotateRight(val,
		/// distance) == rotateRight(val, distance & 0x1F)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the value whose bits are to be rotated right </param>
		/// <param name="distance"> the number of bit positions to rotate right </param>
		/// <returns> the value obtained by rotating the two's complement binary
		///     representation of the specified {@code int} value right by the
		///     specified number of bits.
		/// @since 1.5 </returns>
		public static int RotateRight(int i, int distance)
		{
			return ((int)((uint)i >> distance)) | (i << -distance);
		}

		/// <summary>
		/// Returns the value obtained by reversing the order of the bits in the
		/// two's complement binary representation of the specified {@code int}
		/// value.
		/// </summary>
		/// <param name="i"> the value to be reversed </param>
		/// <returns> the value obtained by reversing order of the bits in the
		///     specified {@code int} value.
		/// @since 1.5 </returns>
		public static int Reverse(int i)
		{
			// HD, Figure 7-1
			i = (i & 0x55555555) << 1 | ((int)((uint)i >> 1)) & 0x55555555;
			i = (i & 0x33333333) << 2 | ((int)((uint)i >> 2)) & 0x33333333;
			i = (i & 0x0f0f0f0f) << 4 | ((int)((uint)i >> 4)) & 0x0f0f0f0f;
			i = (i << 24) | ((i & 0xff00) << 8) | (((int)((uint)i >> 8)) & 0xff00) | ((int)((uint)i >> 24));
			return i;
		}

		/// <summary>
		/// Returns the signum function of the specified {@code int} value.  (The
		/// return value is -1 if the specified value is negative; 0 if the
		/// specified value is zero; and 1 if the specified value is positive.)
		/// </summary>
		/// <param name="i"> the value whose signum is to be computed </param>
		/// <returns> the signum function of the specified {@code int} value.
		/// @since 1.5 </returns>
		public static int Signum(int i)
		{
			// HD, Section 2-7
			return (i >> 31) | (int)((uint)(-i >> 31));
		}

		/// <summary>
		/// Returns the value obtained by reversing the order of the bytes in the
		/// two's complement representation of the specified {@code int} value.
		/// </summary>
		/// <param name="i"> the value whose bytes are to be reversed </param>
		/// <returns> the value obtained by reversing the bytes in the specified
		///     {@code int} value.
		/// @since 1.5 </returns>
		public static int ReverseBytes(int i)
		{
			return (((int)((uint)i >> 24))) | ((i >> 8) & 0xFF00) | ((i << 8) & 0xFF0000) | ((i << 24));
		}

		/// <summary>
		/// Adds two integers together as per the + operator.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the sum of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static int Sum(int a, int b)
		{
			return a + b;
		}

		/// <summary>
		/// Returns the greater of two {@code int} values
		/// as if by calling <seealso cref="Math#max(int, int) Math.max"/>.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the greater of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static int Max(int a, int b)
		{
			return System.Math.Max(a, b);
		}

		/// <summary>
		/// Returns the smaller of two {@code int} values
		/// as if by calling <seealso cref="Math#min(int, int) Math.min"/>.
		/// </summary>
		/// <param name="a"> the first operand </param>
		/// <param name="b"> the second operand </param>
		/// <returns> the smaller of {@code a} and {@code b} </returns>
		/// <seealso cref= java.util.function.BinaryOperator
		/// @since 1.8 </seealso>
		public static int Min(int a, int b)
		{
			return System.Math.Min(a, b);
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native private static final long serialVersionUID = 1360826667806852920L;
		private const long SerialVersionUID = 1360826667806852920L;
	}

}