using System;

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

namespace java.lang
{

	/// <summary>
	/// The {@code Short} class wraps a value of primitive type {@code
	/// short} in an object.  An object of type {@code Short} contains a
	/// single field whose type is {@code short}.
	/// 
	/// <para>In addition, this class provides several methods for converting
	/// a {@code short} to a {@code String} and a {@code String} to a
	/// {@code short}, as well as other constants and methods useful when
	/// dealing with a {@code short}.
	/// 
	/// @author  Nakul Saraiya
	/// @author  Joseph D. Darcy
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.Number
	/// @since   JDK1.1 </seealso>
	public sealed class Short : Number, Comparable<Short>
	{

		/// <summary>
		/// A constant holding the minimum value a {@code short} can
		/// have, -2<sup>15</sup>.
		/// </summary>
		public const short MIN_VALUE = -32768;

		/// <summary>
		/// A constant holding the maximum value a {@code short} can
		/// have, 2<sup>15</sup>-1.
		/// </summary>
		public const short MAX_VALUE = 32767;

		/// <summary>
		/// The {@code Class} instance representing the primitive type
		/// {@code short}.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("short");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("short");

		/// <summary>
		/// Returns a new {@code String} object representing the
		/// specified {@code short}. The radix is assumed to be 10.
		/// </summary>
		/// <param name="s"> the {@code short} to be converted </param>
		/// <returns> the string representation of the specified {@code short} </returns>
		/// <seealso cref= java.lang.Integer#toString(int) </seealso>
		public static String ToString(short s)
		{
			return Convert.ToString((int)s, 10);
		}

		/// <summary>
		/// Parses the string argument as a signed {@code short} in the
		/// radix specified by the second argument. The characters in the
		/// string must all be digits, of the specified radix (as
		/// determined by whether {@link java.lang.Character#digit(char,
		/// int)} returns a nonnegative value) except that the first
		/// character may be an ASCII minus sign {@code '-'}
		/// ({@code '\u005Cu002D'}) to indicate a negative value or an
		/// ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
		/// indicate a positive value.  The resulting {@code short} value
		/// is returned.
		/// 
		/// <para>An exception of type {@code NumberFormatException} is
		/// thrown if any of the following situations occurs:
		/// <ul>
		/// <li> The first argument is {@code null} or is a string of
		/// length zero.
		/// 
		/// <li> The radix is either smaller than {@link
		/// java.lang.Character#MIN_RADIX} or larger than {@link
		/// java.lang.Character#MAX_RADIX}.
		/// 
		/// <li> Any character of the string is not a digit of the
		/// specified radix, except that the first character may be a minus
		/// sign {@code '-'} ({@code '\u005Cu002D'}) or plus sign
		/// {@code '+'} ({@code '\u005Cu002B'}) provided that the
		/// string is longer than length 1.
		/// 
		/// <li> The value represented by the string is not a value of type
		/// {@code short}.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">         the {@code String} containing the
		///                  {@code short} representation to be parsed </param>
		/// <param name="radix">     the radix to be used while parsing {@code s} </param>
		/// <returns>          the {@code short} represented by the string
		///                  argument in the specified radix. </returns>
		/// <exception cref="NumberFormatException"> If the {@code String}
		///                  does not contain a parsable {@code short}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static short parseShort(String s, int radix) throws NumberFormatException
		public static short ParseShort(String s, int radix)
		{
			int i = Convert.ToInt32(s, radix);
			if (i < MIN_VALUE || i > MAX_VALUE)
			{
				throw new NumberFormatException("Value out of range. Value:\"" + s + "\" Radix:" + radix);
			}
			return (short)i;
		}

		/// <summary>
		/// Parses the string argument as a signed decimal {@code
		/// short}. The characters in the string must all be decimal
		/// digits, except that the first character may be an ASCII minus
		/// sign {@code '-'} ({@code '\u005Cu002D'}) to indicate a
		/// negative value or an ASCII plus sign {@code '+'}
		/// ({@code '\u005Cu002B'}) to indicate a positive value.  The
		/// resulting {@code short} value is returned, exactly as if the
		/// argument and the radix 10 were given as arguments to the {@link
		/// #parseShort(java.lang.String, int)} method.
		/// </summary>
		/// <param name="s"> a {@code String} containing the {@code short}
		///          representation to be parsed </param>
		/// <returns>  the {@code short} value represented by the
		///          argument in decimal. </returns>
		/// <exception cref="NumberFormatException"> If the string does not
		///          contain a parsable {@code short}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static short parseShort(String s) throws NumberFormatException
		public static short ParseShort(String s)
		{
			return ParseShort(s, 10);
		}

		/// <summary>
		/// Returns a {@code Short} object holding the value
		/// extracted from the specified {@code String} when parsed
		/// with the radix given by the second argument. The first argument
		/// is interpreted as representing a signed {@code short} in
		/// the radix specified by the second argument, exactly as if the
		/// argument were given to the {@link #parseShort(java.lang.String,
		/// int)} method. The result is a {@code Short} object that
		/// represents the {@code short} value specified by the string.
		/// 
		/// <para>In other words, this method returns a {@code Short} object
		/// equal to the value of:
		/// 
		/// <blockquote>
		///  {@code new Short(Short.parseShort(s, radix))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">         the string to be parsed </param>
		/// <param name="radix">     the radix to be used in interpreting {@code s} </param>
		/// <returns>          a {@code Short} object holding the value
		///                  represented by the string argument in the
		///                  specified radix. </returns>
		/// <exception cref="NumberFormatException"> If the {@code String} does
		///                  not contain a parsable {@code short}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Short valueOf(String s, int radix) throws NumberFormatException
		public static Short ValueOf(String s, int radix)
		{
			return ValueOf(ParseShort(s, radix));
		}

		/// <summary>
		/// Returns a {@code Short} object holding the
		/// value given by the specified {@code String}. The argument
		/// is interpreted as representing a signed decimal
		/// {@code short}, exactly as if the argument were given to
		/// the <seealso cref="#parseShort(java.lang.String)"/> method. The result is
		/// a {@code Short} object that represents the
		/// {@code short} value specified by the string.
		/// 
		/// <para>In other words, this method returns a {@code Short} object
		/// equal to the value of:
		/// 
		/// <blockquote>
		///  {@code new Short(Short.parseShort(s))}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the string to be parsed </param>
		/// <returns>  a {@code Short} object holding the value
		///          represented by the string argument </returns>
		/// <exception cref="NumberFormatException"> If the {@code String} does
		///          not contain a parsable {@code short}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Short valueOf(String s) throws NumberFormatException
		public static Short ValueOf(String s)
		{
			return ValueOf(s, 10);
		}

		private class ShortCache
		{
			internal ShortCache()
			{
			}

			internal static readonly Short[] Cache = new Short[-(-128) + 127 + 1];

			static ShortCache()
			{
				for (int i = 0; i < Cache.Length; i++)
				{
					Cache[i] = new Short((short)(i - 128));
				}
			}
		}

		/// <summary>
		/// Returns a {@code Short} instance representing the specified
		/// {@code short} value.
		/// If a new {@code Short} instance is not required, this method
		/// should generally be used in preference to the constructor
		/// <seealso cref="#Short(short)"/>, as this method is likely to yield
		/// significantly better space and time performance by caching
		/// frequently requested values.
		/// 
		/// This method will always cache values in the range -128 to 127,
		/// inclusive, and may cache other values outside of this range.
		/// </summary>
		/// <param name="s"> a short value. </param>
		/// <returns> a {@code Short} instance representing {@code s}.
		/// @since  1.5 </returns>
		public static Short ValueOf(short s)
		{
			const int offset = 128;
			int sAsInt = s;
			if (sAsInt >= -128 && sAsInt <= 127) // must cache
			{
				return ShortCache.Cache[sAsInt + offset];
			}
			return new Short(s);
		}

		/// <summary>
		/// Decodes a {@code String} into a {@code Short}.
		/// Accepts decimal, hexadecimal, and octal numbers given by
		/// the following grammar:
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
		/// Short.parseShort} method with the indicated radix (10, 16, or
		/// 8).  This sequence of characters must represent a positive
		/// value or a <seealso cref="NumberFormatException"/> will be thrown.  The
		/// result is negated if first character of the specified {@code
		/// String} is the minus sign.  No whitespace characters are
		/// permitted in the {@code String}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nm"> the {@code String} to decode. </param>
		/// <returns>    a {@code Short} object holding the {@code short}
		///            value represented by {@code nm} </returns>
		/// <exception cref="NumberFormatException">  if the {@code String} does not
		///            contain a parsable {@code short}. </exception>
		/// <seealso cref= java.lang.Short#parseShort(java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Short decode(String nm) throws NumberFormatException
		public static Short Decode(String nm)
		{
			int i = Integer.Decode(nm);
			if (i < MIN_VALUE || i > MAX_VALUE)
			{
				throw new NumberFormatException("Value " + i + " out of range from input " + nm);
			}
			return ValueOf((short)i);
		}

		/// <summary>
		/// The value of the {@code Short}.
		/// 
		/// @serial
		/// </summary>
		private readonly short Value;

		/// <summary>
		/// Constructs a newly allocated {@code Short} object that
		/// represents the specified {@code short} value.
		/// </summary>
		/// <param name="value">     the value to be represented by the
		///                  {@code Short}. </param>
		public Short(short value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Constructs a newly allocated {@code Short} object that
		/// represents the {@code short} value indicated by the
		/// {@code String} parameter. The string is converted to a
		/// {@code short} value in exactly the manner used by the
		/// {@code parseShort} method for radix 10.
		/// </summary>
		/// <param name="s"> the {@code String} to be converted to a
		///          {@code Short} </param>
		/// <exception cref="NumberFormatException"> If the {@code String}
		///          does not contain a parsable {@code short}. </exception>
		/// <seealso cref=     java.lang.Short#parseShort(java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Short(String s) throws NumberFormatException
		public Short(String s)
		{
			this.Value = ParseShort(s, 10);
		}

		/// <summary>
		/// Returns the value of this {@code Short} as a {@code byte} after
		/// a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override sbyte ByteValue()
		{
			return (sbyte)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Short} as a
		/// {@code short}.
		/// </summary>
		public override short ShortValue()
		{
			return Value;
		}

		/// <summary>
		/// Returns the value of this {@code Short} as an {@code int} after
		/// a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override int IntValue()
		{
			return (int)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Short} as a {@code long} after
		/// a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override long LongValue()
		{
			return (long)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Short} as a {@code float}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override float FloatValue()
		{
			return (float)Value;
		}

		/// <summary>
		/// Returns the value of this {@code Short} as a {@code double}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Value;
		}

		/// <summary>
		/// Returns a {@code String} object representing this
		/// {@code Short}'s value.  The value is converted to signed
		/// decimal representation and returned as a string, exactly as if
		/// the {@code short} value were given as an argument to the
		/// <seealso cref="java.lang.Short#toString(short)"/> method.
		/// </summary>
		/// <returns>  a string representation of the value of this object in
		///          base&nbsp;10. </returns>
		public override String ToString()
		{
			return Convert.ToString((int)Value);
		}

		/// <summary>
		/// Returns a hash code for this {@code Short}; equal to the result
		/// of invoking {@code intValue()}.
		/// </summary>
		/// <returns> a hash code value for this {@code Short} </returns>
		public override int HashCode()
		{
			return Short.HashCode(Value);
		}

		/// <summary>
		/// Returns a hash code for a {@code short} value; compatible with
		/// {@code Short.hashCode()}.
		/// </summary>
		/// <param name="value"> the value to hash </param>
		/// <returns> a hash code value for a {@code short} value.
		/// @since 1.8 </returns>
		public static int HashCode(short value)
		{
			return (int)value;
		}

		/// <summary>
		/// Compares this object to the specified object.  The result is
		/// {@code true} if and only if the argument is not
		/// {@code null} and is a {@code Short} object that
		/// contains the same {@code short} value as this object.
		/// </summary>
		/// <param name="obj">       the object to compare with </param>
		/// <returns>          {@code true} if the objects are the same;
		///                  {@code false} otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Short)
			{
				return Value == ((Short)obj).ShortValue();
			}
			return false;
		}

		/// <summary>
		/// Compares two {@code Short} objects numerically.
		/// </summary>
		/// <param name="anotherShort">   the {@code Short} to be compared. </param>
		/// <returns>  the value {@code 0} if this {@code Short} is
		///          equal to the argument {@code Short}; a value less than
		///          {@code 0} if this {@code Short} is numerically less
		///          than the argument {@code Short}; and a value greater than
		///           {@code 0} if this {@code Short} is numerically
		///           greater than the argument {@code Short} (signed
		///           comparison).
		/// @since   1.2 </returns>
		public int CompareTo(Short anotherShort)
		{
			return Compare(this.Value, anotherShort.Value);
		}

		/// <summary>
		/// Compares two {@code short} values numerically.
		/// The value returned is identical to what would be returned by:
		/// <pre>
		///    Short.valueOf(x).compareTo(Short.valueOf(y))
		/// </pre>
		/// </summary>
		/// <param name="x"> the first {@code short} to compare </param>
		/// <param name="y"> the second {@code short} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y};
		///         a value less than {@code 0} if {@code x < y}; and
		///         a value greater than {@code 0} if {@code x > y}
		/// @since 1.7 </returns>
		public static int Compare(short x, short y)
		{
			return x - y;
		}

		/// <summary>
		/// The number of bits used to represent a {@code short} value in two's
		/// complement binary form.
		/// @since 1.5
		/// </summary>
		public const int SIZE = 16;

		/// <summary>
		/// The number of bytes used to represent a {@code short} value in two's
		/// complement binary form.
		/// 
		/// @since 1.8
		/// </summary>
		public static readonly int BYTES = SIZE / sizeof(sbyte);

		/// <summary>
		/// Returns the value obtained by reversing the order of the bytes in the
		/// two's complement representation of the specified {@code short} value.
		/// </summary>
		/// <param name="i"> the value whose bytes are to be reversed </param>
		/// <returns> the value obtained by reversing (or, equivalently, swapping)
		///     the bytes in the specified {@code short} value.
		/// @since 1.5 </returns>
		public static short ReverseBytes(short i)
		{
			return (short)(((i & 0xFF00) >> 8) | (i << 8));
		}


		/// <summary>
		/// Converts the argument to an {@code int} by an unsigned
		/// conversion.  In an unsigned conversion to an {@code int}, the
		/// high-order 16 bits of the {@code int} are zero and the
		/// low-order 16 bits are equal to the bits of the {@code short} argument.
		/// 
		/// Consequently, zero and positive {@code short} values are mapped
		/// to a numerically equal {@code int} value and negative {@code
		/// short} values are mapped to an {@code int} value equal to the
		/// input plus 2<sup>16</sup>.
		/// </summary>
		/// <param name="x"> the value to convert to an unsigned {@code int} </param>
		/// <returns> the argument converted to {@code int} by an unsigned
		///         conversion
		/// @since 1.8 </returns>
		public static int ToUnsignedInt(short x)
		{
			return ((int) x) & 0xffff;
		}

		/// <summary>
		/// Converts the argument to a {@code long} by an unsigned
		/// conversion.  In an unsigned conversion to a {@code long}, the
		/// high-order 48 bits of the {@code long} are zero and the
		/// low-order 16 bits are equal to the bits of the {@code short} argument.
		/// 
		/// Consequently, zero and positive {@code short} values are mapped
		/// to a numerically equal {@code long} value and negative {@code
		/// short} values are mapped to a {@code long} value equal to the
		/// input plus 2<sup>16</sup>.
		/// </summary>
		/// <param name="x"> the value to convert to an unsigned {@code long} </param>
		/// <returns> the argument converted to {@code long} by an unsigned
		///         conversion
		/// @since 1.8 </returns>
		public static long ToUnsignedLong(short x)
		{
			return ((long) x) & 0xffffL;
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.1. for interoperability </summary>
		private const long SerialVersionUID = 7515723908773894738L;
	}

}