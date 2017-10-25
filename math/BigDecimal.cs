using System;
using System.Diagnostics;

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

/*
 * Portions Copyright IBM Corporation, 2001. All Rights Reserved.
 */

namespace java.math
{


	/// <summary>
	/// Immutable, arbitrary-precision signed decimal numbers.  A
	/// {@code BigDecimal} consists of an arbitrary precision integer
	/// <i>unscaled value</i> and a 32-bit integer <i>scale</i>.  If zero
	/// or positive, the scale is the number of digits to the right of the
	/// decimal point.  If negative, the unscaled value of the number is
	/// multiplied by ten to the power of the negation of the scale.  The
	/// value of the number represented by the {@code BigDecimal} is
	/// therefore <tt>(unscaledValue &times; 10<sup>-scale</sup>)</tt>.
	/// 
	/// <para>The {@code BigDecimal} class provides operations for
	/// arithmetic, scale manipulation, rounding, comparison, hashing, and
	/// format conversion.  The <seealso cref="#toString"/> method provides a
	/// canonical representation of a {@code BigDecimal}.
	/// 
	/// </para>
	/// <para>The {@code BigDecimal} class gives its user complete control
	/// over rounding behavior.  If no rounding mode is specified and the
	/// exact result cannot be represented, an exception is thrown;
	/// otherwise, calculations can be carried out to a chosen precision
	/// and rounding mode by supplying an appropriate <seealso cref="MathContext"/>
	/// object to the operation.  In either case, eight <em>rounding
	/// modes</em> are provided for the control of rounding.  Using the
	/// integer fields in this class (such as <seealso cref="#ROUND_HALF_UP"/>) to
	/// represent rounding mode is largely obsolete; the enumeration values
	/// of the {@code RoundingMode} {@code enum}, (such as {@link
	/// RoundingMode#HALF_UP}) should be used instead.
	/// 
	/// </para>
	/// <para>When a {@code MathContext} object is supplied with a precision
	/// setting of 0 (for example, <seealso cref="MathContext#UNLIMITED"/>),
	/// arithmetic operations are exact, as are the arithmetic methods
	/// which take no {@code MathContext} object.  (This is the only
	/// behavior that was supported in releases prior to 5.)  As a
	/// corollary of computing the exact result, the rounding mode setting
	/// of a {@code MathContext} object with a precision setting of 0 is
	/// not used and thus irrelevant.  In the case of divide, the exact
	/// quotient could have an infinitely long decimal expansion; for
	/// example, 1 divided by 3.  If the quotient has a nonterminating
	/// decimal expansion and the operation is specified to return an exact
	/// result, an {@code ArithmeticException} is thrown.  Otherwise, the
	/// exact result of the division is returned, as done for other
	/// operations.
	/// 
	/// </para>
	/// <para>When the precision setting is not 0, the rules of
	/// {@code BigDecimal} arithmetic are broadly compatible with selected
	/// modes of operation of the arithmetic defined in ANSI X3.274-1996
	/// and ANSI X3.274-1996/AM 1-2000 (section 7.4).  Unlike those
	/// standards, {@code BigDecimal} includes many rounding modes, which
	/// were mandatory for division in {@code BigDecimal} releases prior
	/// to 5.  Any conflicts between these ANSI standards and the
	/// {@code BigDecimal} specification are resolved in favor of
	/// {@code BigDecimal}.
	/// 
	/// </para>
	/// <para>Since the same numerical value can have different
	/// representations (with different scales), the rules of arithmetic
	/// and rounding must specify both the numerical result and the scale
	/// used in the result's representation.
	/// 
	/// 
	/// </para>
	/// <para>In general the rounding modes and precision setting determine
	/// how operations return results with a limited number of digits when
	/// the exact result has more digits (perhaps infinitely many in the
	/// case of division) than the number of digits returned.
	/// 
	/// First, the
	/// total number of digits to return is specified by the
	/// {@code MathContext}'s {@code precision} setting; this determines
	/// the result's <i>precision</i>.  The digit count starts from the
	/// leftmost nonzero digit of the exact result.  The rounding mode
	/// determines how any discarded trailing digits affect the returned
	/// result.
	/// 
	/// </para>
	/// <para>For all arithmetic operators , the operation is carried out as
	/// though an exact intermediate result were first calculated and then
	/// rounded to the number of digits specified by the precision setting
	/// (if necessary), using the selected rounding mode.  If the exact
	/// result is not returned, some digit positions of the exact result
	/// are discarded.  When rounding increases the magnitude of the
	/// returned result, it is possible for a new digit position to be
	/// created by a carry propagating to a leading {@literal "9"} digit.
	/// For example, rounding the value 999.9 to three digits rounding up
	/// would be numerically equal to one thousand, represented as
	/// 100&times;10<sup>1</sup>.  In such cases, the new {@literal "1"} is
	/// the leading digit position of the returned result.
	/// 
	/// </para>
	/// <para>Besides a logical exact result, each arithmetic operation has a
	/// preferred scale for representing a result.  The preferred
	/// scale for each operation is listed in the table below.
	/// 
	/// <table border>
	/// <caption><b>Preferred Scales for Results of Arithmetic Operations
	/// </b></caption>
	/// <tr><th>Operation</th><th>Preferred Scale of Result</th></tr>
	/// <tr><td>Add</td><td>max(addend.scale(), augend.scale())</td>
	/// <tr><td>Subtract</td><td>max(minuend.scale(), subtrahend.scale())</td>
	/// <tr><td>Multiply</td><td>multiplier.scale() + multiplicand.scale()</td>
	/// <tr><td>Divide</td><td>dividend.scale() - divisor.scale()</td>
	/// </table>
	/// 
	/// These scales are the ones used by the methods which return exact
	/// arithmetic results; except that an exact divide may have to use a
	/// larger scale since the exact result may have more digits.  For
	/// example, {@code 1/32} is {@code 0.03125}.
	/// 
	/// </para>
	/// <para>Before rounding, the scale of the logical exact intermediate
	/// result is the preferred scale for that operation.  If the exact
	/// numerical result cannot be represented in {@code precision}
	/// digits, rounding selects the set of digits to return and the scale
	/// of the result is reduced from the scale of the intermediate result
	/// to the least scale which can represent the {@code precision}
	/// digits actually returned.  If the exact result can be represented
	/// with at most {@code precision} digits, the representation
	/// of the result with the scale closest to the preferred scale is
	/// returned.  In particular, an exactly representable quotient may be
	/// represented in fewer than {@code precision} digits by removing
	/// trailing zeros and decreasing the scale.  For example, rounding to
	/// three digits using the <seealso cref="RoundingMode#FLOOR floor"/>
	/// rounding mode, <br>
	/// 
	/// {@code 19/100 = 0.19   // integer=19,  scale=2} <br>
	/// 
	/// but<br>
	/// 
	/// {@code 21/110 = 0.190  // integer=190, scale=3} <br>
	/// 
	/// </para>
	/// <para>Note that for add, subtract, and multiply, the reduction in
	/// scale will equal the number of digit positions of the exact result
	/// which are discarded. If the rounding causes a carry propagation to
	/// create a new high-order digit position, an additional digit of the
	/// result is discarded than when no new digit position is created.
	/// 
	/// </para>
	/// <para>Other methods may have slightly different rounding semantics.
	/// For example, the result of the {@code pow} method using the
	/// <seealso cref="#pow(int, MathContext) specified algorithm"/> can
	/// occasionally differ from the rounded mathematical result by more
	/// than one unit in the last place, one <i><seealso cref="#ulp() ulp"/></i>.
	/// 
	/// </para>
	/// <para>Two types of operations are provided for manipulating the scale
	/// of a {@code BigDecimal}: scaling/rounding operations and decimal
	/// point motion operations.  Scaling/rounding operations ({@link
	/// #setScale setScale} and <seealso cref="#round round"/>) return a
	/// {@code BigDecimal} whose value is approximately (or exactly) equal
	/// to that of the operand, but whose scale or precision is the
	/// specified value; that is, they increase or decrease the precision
	/// of the stored number with minimal effect on its value.  Decimal
	/// point motion operations (<seealso cref="#movePointLeft movePointLeft"/> and
	/// <seealso cref="#movePointRight movePointRight"/>) return a
	/// {@code BigDecimal} created from the operand by moving the decimal
	/// point a specified distance in the specified direction.
	/// 
	/// </para>
	/// <para>For the sake of brevity and clarity, pseudo-code is used
	/// throughout the descriptions of {@code BigDecimal} methods.  The
	/// pseudo-code expression {@code (i + j)} is shorthand for "a
	/// {@code BigDecimal} whose value is that of the {@code BigDecimal}
	/// {@code i} added to that of the {@code BigDecimal}
	/// {@code j}." The pseudo-code expression {@code (i == j)} is
	/// shorthand for "{@code true} if and only if the
	/// {@code BigDecimal} {@code i} represents the same value as the
	/// {@code BigDecimal} {@code j}." Other pseudo-code expressions
	/// are interpreted similarly.  Square brackets are used to represent
	/// the particular {@code BigInteger} and scale pair defining a
	/// {@code BigDecimal} value; for example [19, 2] is the
	/// {@code BigDecimal} numerically equal to 0.19 having a scale of 2.
	/// 
	/// </para>
	/// <para>Note: care should be exercised if {@code BigDecimal} objects
	/// are used as keys in a <seealso cref="java.util.SortedMap SortedMap"/> or
	/// elements in a <seealso cref="java.util.SortedSet SortedSet"/> since
	/// {@code BigDecimal}'s <i>natural ordering</i> is <i>inconsistent
	/// with equals</i>.  See <seealso cref="Comparable"/>, {@link
	/// java.util.SortedMap} or <seealso cref="java.util.SortedSet"/> for more
	/// information.
	/// 
	/// </para>
	/// <para>All methods and constructors for this class throw
	/// {@code NullPointerException} when passed a {@code null} object
	/// reference for any input parameter.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     BigInteger </seealso>
	/// <seealso cref=     MathContext </seealso>
	/// <seealso cref=     RoundingMode </seealso>
	/// <seealso cref=     java.util.SortedMap </seealso>
	/// <seealso cref=     java.util.SortedSet
	/// @author  Josh Bloch
	/// @author  Mike Cowlishaw
	/// @author  Joseph D. Darcy
	/// @author  Sergey V. Kuksenko </seealso>
	public class BigDecimal : Number, Comparable<BigDecimal>
	{
		/// <summary>
		/// The unscaled value of this BigDecimal, as returned by {@link
		/// #unscaledValue}.
		/// 
		/// @serial </summary>
		/// <seealso cref= #unscaledValue </seealso>
		private readonly BigInteger IntVal;

		/// <summary>
		/// The scale of this BigDecimal, as returned by <seealso cref="#scale"/>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #scale </seealso>
		private readonly int Scale_Renamed; // Note: this may have any value, so
								  // calculations must be done in longs

		/// <summary>
		/// The number of decimal digits in this BigDecimal, or 0 if the
		/// number of digits are not known (lookaside information).  If
		/// nonzero, the value is guaranteed correct.  Use the precision()
		/// method to obtain and set the value if it might be 0.  This
		/// field is mutable until set nonzero.
		/// 
		/// @since  1.5
		/// </summary>
		[NonSerialized]
		private int Precision_Renamed;

		/// <summary>
		/// Used to store the canonical string representation, if computed.
		/// </summary>
		[NonSerialized]
		private String StringCache;

		/// <summary>
		/// Sentinel value for <seealso cref="#intCompact"/> indicating the
		/// significand information is only available from {@code intVal}.
		/// </summary>
		internal const long INFLATED = Long.MinValue;

		private static readonly BigInteger INFLATED_BIGINT = BigInteger.ValueOf(INFLATED);

		/// <summary>
		/// If the absolute value of the significand of this BigDecimal is
		/// less than or equal to {@code Long.MAX_VALUE}, the value can be
		/// compactly stored in this field and used in computations.
		/// </summary>
		[NonSerialized]
		private readonly long IntCompact;

		// All 18-digit base ten strings fit into a long; not all 19-digit
		// strings will
		private const int MAX_COMPACT_DIGITS = 18;

		/* Appease the serialization gods */
		private const long SerialVersionUID = 6108874887143696463L;

		private static readonly ThreadLocal<StringBuilderHelper> threadLocalStringBuilderHelper = new ThreadLocalAnonymousInnerClassHelper();

		private class ThreadLocalAnonymousInnerClassHelper : ThreadLocal<StringBuilderHelper>
		{
			public ThreadLocalAnonymousInnerClassHelper()
			{
			}

			protected internal override StringBuilderHelper InitialValue()
			{
				return new StringBuilderHelper();
			}
		}

		// Cache of common small BigDecimal values.
		private static readonly BigDecimal[] ZeroThroughTen = new BigDecimal[] {new BigDecimal(BigInteger.ZERO, 0, 0, 1), new BigDecimal(BigInteger.ONE, 1, 0, 1), new BigDecimal(BigInteger.ValueOf(2), 2, 0, 1), new BigDecimal(BigInteger.ValueOf(3), 3, 0, 1), new BigDecimal(BigInteger.ValueOf(4), 4, 0, 1), new BigDecimal(BigInteger.ValueOf(5), 5, 0, 1), new BigDecimal(BigInteger.ValueOf(6), 6, 0, 1), new BigDecimal(BigInteger.ValueOf(7), 7, 0, 1), new BigDecimal(BigInteger.ValueOf(8), 8, 0, 1), new BigDecimal(BigInteger.ValueOf(9), 9, 0, 1), new BigDecimal(BigInteger.TEN, 10, 0, 2)};

		// Cache of zero scaled by 0 - 15
		private static readonly BigDecimal[] ZERO_SCALED_BY = new BigDecimal[] {ZeroThroughTen[0], new BigDecimal(BigInteger.ZERO, 0, 1, 1), new BigDecimal(BigInteger.ZERO, 0, 2, 1), new BigDecimal(BigInteger.ZERO, 0, 3, 1), new BigDecimal(BigInteger.ZERO, 0, 4, 1), new BigDecimal(BigInteger.ZERO, 0, 5, 1), new BigDecimal(BigInteger.ZERO, 0, 6, 1), new BigDecimal(BigInteger.ZERO, 0, 7, 1), new BigDecimal(BigInteger.ZERO, 0, 8, 1), new BigDecimal(BigInteger.ZERO, 0, 9, 1), new BigDecimal(BigInteger.ZERO, 0, 10, 1), new BigDecimal(BigInteger.ZERO, 0, 11, 1), new BigDecimal(BigInteger.ZERO, 0, 12, 1), new BigDecimal(BigInteger.ZERO, 0, 13, 1), new BigDecimal(BigInteger.ZERO, 0, 14, 1), new BigDecimal(BigInteger.ZERO, 0, 15, 1)};

		// Half of Long.MIN_VALUE & Long.MAX_VALUE.
		private static readonly long HALF_LONG_MAX_VALUE = Long.MaxValue / 2;
		private static readonly long HALF_LONG_MIN_VALUE = Long.MinValue / 2;

		// Constants
		/// <summary>
		/// The value 0, with a scale of 0.
		/// 
		/// @since  1.5
		/// </summary>
		public static readonly BigDecimal ZERO = ZeroThroughTen[0];

		/// <summary>
		/// The value 1, with a scale of 0.
		/// 
		/// @since  1.5
		/// </summary>
		public static readonly BigDecimal ONE = ZeroThroughTen[1];

		/// <summary>
		/// The value 10, with a scale of 0.
		/// 
		/// @since  1.5
		/// </summary>
		public static readonly BigDecimal TEN = ZeroThroughTen[10];

		// Constructors

		/// <summary>
		/// Trusted package private constructor.
		/// Trusted simply means if val is INFLATED, intVal could not be null and
		/// if intVal is null, val could not be INFLATED.
		/// </summary>
		internal BigDecimal(BigInteger intVal, long val, int scale, int prec)
		{
			this.Scale_Renamed = scale;
			this.Precision_Renamed = prec;
			this.IntCompact = val;
			this.IntVal = intVal;
		}

		/// <summary>
		/// Translates a character array representation of a
		/// {@code BigDecimal} into a {@code BigDecimal}, accepting the
		/// same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
		/// constructor, while allowing a sub-array to be specified.
		/// 
		/// <para>Note that if the sequence of characters is already available
		/// within a character array, using this constructor is faster than
		/// converting the {@code char} array to string and using the
		/// {@code BigDecimal(String)} constructor .
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> {@code char} array that is the source of characters. </param>
		/// <param name="offset"> first character in the array to inspect. </param>
		/// <param name="len"> number of characters to consider. </param>
		/// <exception cref="NumberFormatException"> if {@code in} is not a valid
		///         representation of a {@code BigDecimal} or the defined subarray
		///         is not wholly within {@code in}.
		/// @since  1.5 </exception>
		public BigDecimal(char[] @in, int offset, int len) : this(@in,offset,len,MathContext.UNLIMITED)
		{
		}

		/// <summary>
		/// Translates a character array representation of a
		/// {@code BigDecimal} into a {@code BigDecimal}, accepting the
		/// same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
		/// constructor, while allowing a sub-array to be specified and
		/// with rounding according to the context settings.
		/// 
		/// <para>Note that if the sequence of characters is already available
		/// within a character array, using this constructor is faster than
		/// converting the {@code char} array to string and using the
		/// {@code BigDecimal(String)} constructor .
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> {@code char} array that is the source of characters. </param>
		/// <param name="offset"> first character in the array to inspect. </param>
		/// <param name="len"> number of characters to consider.. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}. </exception>
		/// <exception cref="NumberFormatException"> if {@code in} is not a valid
		///         representation of a {@code BigDecimal} or the defined subarray
		///         is not wholly within {@code in}.
		/// @since  1.5 </exception>
		public BigDecimal(char[] @in, int offset, int len, MathContext mc)
		{
			// protect against huge length.
			if (offset + len > @in.Length || offset < 0)
			{
				throw new NumberFormatException("Bad offset or len arguments for char[] input.");
			}
			// This is the primary string to BigDecimal constructor; all
			// incoming strings end up here; it uses explicit (inline)
			// parsing for speed and generates at most one intermediate
			// (temporary) object (a char[] array) for non-compact case.

			// Use locals for all fields values until completion
			int prec = 0; // record precision value
			int scl = 0; // record scale value
			long rs = 0; // the compact value in long
			BigInteger rb = null; // the inflated value in BigInteger
			// use array bounds checking to handle too-long, len == 0,
			// bad offset, etc.
			try
			{
				// handle the sign
				bool isneg = false; // assume positive
				if (@in[offset] == '-')
				{
					isneg = true; // leading minus means negative
					offset++;
					len--;
				} // leading + allowed
				else if (@in[offset] == '+')
				{
					offset++;
					len--;
				}

				// should now be at numeric part of the significand
				bool dot = false; // true when there is a '.'
				long exp = 0; // exponent
				char c; // current character
				bool isCompact = (len <= MAX_COMPACT_DIGITS);
				// integer significand array & idx is the index to it. The array
				// is ONLY used when we can't use a compact representation.
				int idx = 0;
				if (isCompact)
				{
					// First compact case, we need not to preserve the character
					// and we can just compute the value in place.
					for (; len > 0; offset++, len--)
					{
						c = @in[offset];
						if ((c == '0')) // have zero
						{
							if (prec == 0)
							{
								prec = 1;
							}
							else if (rs != 0)
							{
								rs *= 10;
								++prec;
							} // else digit is a redundant leading zero
							if (dot)
							{
								++scl;
							}
						} // have digit
						else if ((c >= '1' && c <= '9'))
						{
							int digit = c - '0';
							if (prec != 1 || rs != 0)
							{
								++prec; // prec unchanged if preceded by 0s
							}
							rs = rs * 10 + digit;
							if (dot)
							{
								++scl;
							}
						} // have dot
						else if (c == '.')
						{
							// have dot
							if (dot) // two dots
							{
								throw new NumberFormatException();
							}
							dot = true;
						} // slow path
						else if (char.IsDigit(c))
						{
							int digit = Character.Digit(c, 10);
							if (digit == 0)
							{
								if (prec == 0)
								{
									prec = 1;
								}
								else if (rs != 0)
								{
									rs *= 10;
									++prec;
								} // else digit is a redundant leading zero
							}
							else
							{
								if (prec != 1 || rs != 0)
								{
									++prec; // prec unchanged if preceded by 0s
								}
								rs = rs * 10 + digit;
							}
							if (dot)
							{
								++scl;
							}
						}
						else if ((c == 'e') || (c == 'E'))
						{
							exp = ParseExp(@in, offset, len);
							// Next test is required for backwards compatibility
							if ((int) exp != exp) // overflow
							{
								throw new NumberFormatException();
							}
							break; // [saves a test]
						}
						else
						{
							throw new NumberFormatException();
						}
					}
					if (prec == 0) // no digits found
					{
						throw new NumberFormatException();
					}
					// Adjust scale if exp is not zero.
					if (exp != 0) // had significant exponent
					{
						scl = AdjustScale(scl, exp);
					}
					rs = isneg ? - rs : rs;
					int mcp = mc.Precision_Renamed;
					int drop = prec - mcp; // prec has range [1, MAX_INT], mcp has range [0, MAX_INT];
										   // therefore, this subtract cannot overflow
					if (mcp > 0 && drop > 0) // do rounding
					{
						while (drop > 0)
						{
							scl = CheckScaleNonZero((long) scl - drop);
							rs = DivideAndRound(rs, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
							prec = LongDigitLength(rs);
							drop = prec - mcp;
						}
					}
				}
				else
				{
					char[] coeff = new char[len];
					for (; len > 0; offset++, len--)
					{
						c = @in[offset];
						// have digit
						if ((c >= '0' && c <= '9') || char.IsDigit(c))
						{
							// First compact case, we need not to preserve the character
							// and we can just compute the value in place.
							if (c == '0' || Character.Digit(c, 10) == 0)
							{
								if (prec == 0)
								{
									coeff[idx] = c;
									prec = 1;
								}
								else if (idx != 0)
								{
									coeff[idx++] = c;
									++prec;
								} // else c must be a redundant leading zero
							}
							else
							{
								if (prec != 1 || idx != 0)
								{
									++prec; // prec unchanged if preceded by 0s
								}
								coeff[idx++] = c;
							}
							if (dot)
							{
								++scl;
							}
							continue;
						}
						// have dot
						if (c == '.')
						{
							// have dot
							if (dot) // two dots
							{
								throw new NumberFormatException();
							}
							dot = true;
							continue;
						}
						// exponent expected
						if ((c != 'e') && (c != 'E'))
						{
							throw new NumberFormatException();
						}
						exp = ParseExp(@in, offset, len);
						// Next test is required for backwards compatibility
						if ((int) exp != exp) // overflow
						{
							throw new NumberFormatException();
						}
						break; // [saves a test]
					}
					// here when no characters left
					if (prec == 0) // no digits found
					{
						throw new NumberFormatException();
					}
					// Adjust scale if exp is not zero.
					if (exp != 0) // had significant exponent
					{
						scl = AdjustScale(scl, exp);
					}
					// Remove leading zeros from precision (digits count)
					rb = new BigInteger(coeff, isneg ? - 1 : 1, prec);
					rs = CompactValFor(rb);
					int mcp = mc.Precision_Renamed;
					if (mcp > 0 && (prec > mcp))
					{
						if (rs == INFLATED)
						{
							int drop = prec - mcp;
							while (drop > 0)
							{
								scl = CheckScaleNonZero((long) scl - drop);
								rb = DivideAndRoundByTenPow(rb, drop, mc.RoundingMode_Renamed.oldMode);
								rs = CompactValFor(rb);
								if (rs != INFLATED)
								{
									prec = LongDigitLength(rs);
									break;
								}
								prec = BigDigitLength(rb);
								drop = prec - mcp;
							}
						}
						if (rs != INFLATED)
						{
							int drop = prec - mcp;
							while (drop > 0)
							{
								scl = CheckScaleNonZero((long) scl - drop);
								rs = DivideAndRound(rs, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
								prec = LongDigitLength(rs);
								drop = prec - mcp;
							}
							rb = null;
						}
					}
				}
			}
			catch (ArrayIndexOutOfBoundsException)
			{
				throw new NumberFormatException();
			}
			catch (NegativeArraySizeException)
			{
				throw new NumberFormatException();
			}
			this.Scale_Renamed = scl;
			this.Precision_Renamed = prec;
			this.IntCompact = rs;
			this.IntVal = rb;
		}

		private int AdjustScale(int scl, long exp)
		{
			long adjustedScale = scl - exp;
			if (adjustedScale > Integer.MaxValue || adjustedScale < Integer.MinValue)
			{
				throw new NumberFormatException("Scale out of range.");
			}
			scl = (int) adjustedScale;
			return scl;
		}

		/*
		 * parse exponent
		 */
		private static long ParseExp(char[] @in, int offset, int len)
		{
			long exp = 0;
			offset++;
			char c = @in[offset];
			len--;
			bool negexp = (c == '-');
			// optional sign
			if (negexp || c == '+')
			{
				offset++;
				c = @in[offset];
				len--;
			}
			if (len <= 0) // no exponent digits
			{
				throw new NumberFormatException();
			}
			// skip leading zeros in the exponent
			while (len > 10 && (c == '0' || (Character.Digit(c, 10) == 0)))
			{
				offset++;
				c = @in[offset];
				len--;
			}
			if (len > 10) // too many nonzero exponent digits
			{
				throw new NumberFormatException();
			}
			// c now holds first digit of exponent
			for (;; len--)
			{
				int v;
				if (c >= '0' && c <= '9')
				{
					v = c - '0';
				}
				else
				{
					v = Character.Digit(c, 10);
					if (v < 0) // not a digit
					{
						throw new NumberFormatException();
					}
				}
				exp = exp * 10 + v;
				if (len == 1)
				{
					break; // that was final character
				}
				offset++;
				c = @in[offset];
			}
			if (negexp) // apply sign
			{
				exp = -exp;
			}
			return exp;
		}

		/// <summary>
		/// Translates a character array representation of a
		/// {@code BigDecimal} into a {@code BigDecimal}, accepting the
		/// same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
		/// constructor.
		/// 
		/// <para>Note that if the sequence of characters is already available
		/// as a character array, using this constructor is faster than
		/// converting the {@code char} array to string and using the
		/// {@code BigDecimal(String)} constructor .
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> {@code char} array that is the source of characters. </param>
		/// <exception cref="NumberFormatException"> if {@code in} is not a valid
		///         representation of a {@code BigDecimal}.
		/// @since  1.5 </exception>
		public BigDecimal(char[] @in) : this(@in, 0, @in.Length)
		{
		}

		/// <summary>
		/// Translates a character array representation of a
		/// {@code BigDecimal} into a {@code BigDecimal}, accepting the
		/// same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
		/// constructor and with rounding according to the context
		/// settings.
		/// 
		/// <para>Note that if the sequence of characters is already available
		/// as a character array, using this constructor is faster than
		/// converting the {@code char} array to string and using the
		/// {@code BigDecimal(String)} constructor .
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> {@code char} array that is the source of characters. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}. </exception>
		/// <exception cref="NumberFormatException"> if {@code in} is not a valid
		///         representation of a {@code BigDecimal}.
		/// @since  1.5 </exception>
		public BigDecimal(char[] @in, MathContext mc) : this(@in, 0, @in.Length, mc)
		{
		}

		/// <summary>
		/// Translates the string representation of a {@code BigDecimal}
		/// into a {@code BigDecimal}.  The string representation consists
		/// of an optional sign, {@code '+'} (<tt> '&#92;u002B'</tt>) or
		/// {@code '-'} (<tt>'&#92;u002D'</tt>), followed by a sequence of
		/// zero or more decimal digits ("the integer"), optionally
		/// followed by a fraction, optionally followed by an exponent.
		/// 
		/// <para>The fraction consists of a decimal point followed by zero
		/// or more decimal digits.  The string must contain at least one
		/// digit in either the integer or the fraction.  The number formed
		/// by the sign, the integer and the fraction is referred to as the
		/// <i>significand</i>.
		/// 
		/// </para>
		/// <para>The exponent consists of the character {@code 'e'}
		/// (<tt>'&#92;u0065'</tt>) or {@code 'E'} (<tt>'&#92;u0045'</tt>)
		/// followed by one or more decimal digits.  The value of the
		/// exponent must lie between -<seealso cref="Integer#MAX_VALUE"/> ({@link
		/// Integer#MIN_VALUE}+1) and <seealso cref="Integer#MAX_VALUE"/>, inclusive.
		/// 
		/// </para>
		/// <para>More formally, the strings this constructor accepts are
		/// described by the following grammar:
		/// <blockquote>
		/// <dl>
		/// <dt><i>BigDecimalString:</i>
		/// <dd><i>Sign<sub>opt</sub> Significand Exponent<sub>opt</sub></i>
		/// <dt><i>Sign:</i>
		/// <dd>{@code +}
		/// <dd>{@code -}
		/// <dt><i>Significand:</i>
		/// <dd><i>IntegerPart</i> {@code .} <i>FractionPart<sub>opt</sub></i>
		/// <dd>{@code .} <i>FractionPart</i>
		/// <dd><i>IntegerPart</i>
		/// <dt><i>IntegerPart:</i>
		/// <dd><i>Digits</i>
		/// <dt><i>FractionPart:</i>
		/// <dd><i>Digits</i>
		/// <dt><i>Exponent:</i>
		/// <dd><i>ExponentIndicator SignedInteger</i>
		/// <dt><i>ExponentIndicator:</i>
		/// <dd>{@code e}
		/// <dd>{@code E}
		/// <dt><i>SignedInteger:</i>
		/// <dd><i>Sign<sub>opt</sub> Digits</i>
		/// <dt><i>Digits:</i>
		/// <dd><i>Digit</i>
		/// <dd><i>Digits Digit</i>
		/// <dt><i>Digit:</i>
		/// <dd>any character for which <seealso cref="Character#isDigit"/>
		/// returns {@code true}, including 0, 1, 2 ...
		/// </dl>
		/// </blockquote>
		/// 
		/// </para>
		/// <para>The scale of the returned {@code BigDecimal} will be the
		/// number of digits in the fraction, or zero if the string
		/// contains no decimal point, subject to adjustment for any
		/// exponent; if the string contains an exponent, the exponent is
		/// subtracted from the scale.  The value of the resulting scale
		/// must lie between {@code Integer.MIN_VALUE} and
		/// {@code Integer.MAX_VALUE}, inclusive.
		/// 
		/// </para>
		/// <para>The character-to-digit mapping is provided by {@link
		/// java.lang.Character#digit} set to convert to radix 10.  The
		/// String may not contain any extraneous characters (whitespace,
		/// for example).
		/// 
		/// </para>
		/// <para><b>Examples:</b><br>
		/// The value of the returned {@code BigDecimal} is equal to
		/// <i>significand</i> &times; 10<sup>&nbsp;<i>exponent</i></sup>.
		/// For each string on the left, the resulting representation
		/// [{@code BigInteger}, {@code scale}] is shown on the right.
		/// <pre>
		/// "0"            [0,0]
		/// "0.00"         [0,2]
		/// "123"          [123,0]
		/// "-123"         [-123,0]
		/// "1.23E3"       [123,-1]
		/// "1.23E+3"      [123,-1]
		/// "12.3E+7"      [123,-6]
		/// "12.0"         [120,1]
		/// "12.3"         [123,1]
		/// "0.00123"      [123,5]
		/// "-1.23E-12"    [-123,14]
		/// "1234.5E-4"    [12345,5]
		/// "0E+7"         [0,-7]
		/// "-0"           [0,0]
		/// </pre>
		/// 
		/// </para>
		/// <para>Note: For values other than {@code float} and
		/// {@code double} NaN and &plusmn;Infinity, this constructor is
		/// compatible with the values returned by <seealso cref="Float#toString"/>
		/// and <seealso cref="Double#toString"/>.  This is generally the preferred
		/// way to convert a {@code float} or {@code double} into a
		/// BigDecimal, as it doesn't suffer from the unpredictability of
		/// the <seealso cref="#BigDecimal(double)"/> constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> String representation of {@code BigDecimal}.
		/// </param>
		/// <exception cref="NumberFormatException"> if {@code val} is not a valid
		///         representation of a {@code BigDecimal}. </exception>
		public BigDecimal(String val) : this(val.ToCharArray(), 0, val.Length())
		{
		}

		/// <summary>
		/// Translates the string representation of a {@code BigDecimal}
		/// into a {@code BigDecimal}, accepting the same strings as the
		/// <seealso cref="#BigDecimal(String)"/> constructor, with rounding
		/// according to the context settings.
		/// </summary>
		/// <param name="val"> string representation of a {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}. </exception>
		/// <exception cref="NumberFormatException"> if {@code val} is not a valid
		///         representation of a BigDecimal.
		/// @since  1.5 </exception>
		public BigDecimal(String val, MathContext mc) : this(val.ToCharArray(), 0, val.Length(), mc)
		{
		}

		/// <summary>
		/// Translates a {@code double} into a {@code BigDecimal} which
		/// is the exact decimal representation of the {@code double}'s
		/// binary floating-point value.  The scale of the returned
		/// {@code BigDecimal} is the smallest value such that
		/// <tt>(10<sup>scale</sup> &times; val)</tt> is an integer.
		/// <para>
		/// <b>Notes:</b>
		/// <ol>
		/// <li>
		/// The results of this constructor can be somewhat unpredictable.
		/// One might assume that writing {@code new BigDecimal(0.1)} in
		/// Java creates a {@code BigDecimal} which is exactly equal to
		/// 0.1 (an unscaled value of 1, with a scale of 1), but it is
		/// actually equal to
		/// 0.1000000000000000055511151231257827021181583404541015625.
		/// This is because 0.1 cannot be represented exactly as a
		/// {@code double} (or, for that matter, as a binary fraction of
		/// any finite length).  Thus, the value that is being passed
		/// <i>in</i> to the constructor is not exactly equal to 0.1,
		/// appearances notwithstanding.
		/// 
		/// <li>
		/// The {@code String} constructor, on the other hand, is
		/// perfectly predictable: writing {@code new BigDecimal("0.1")}
		/// creates a {@code BigDecimal} which is <i>exactly</i> equal to
		/// 0.1, as one would expect.  Therefore, it is generally
		/// recommended that the {@link #BigDecimal(String)
		/// <tt>String</tt> constructor} be used in preference to this one.
		/// 
		/// <li>
		/// When a {@code double} must be used as a source for a
		/// {@code BigDecimal}, note that this constructor provides an
		/// exact conversion; it does not give the same result as
		/// converting the {@code double} to a {@code String} using the
		/// <seealso cref="Double#toString(double)"/> method and then using the
		/// <seealso cref="#BigDecimal(String)"/> constructor.  To get that result,
		/// use the {@code static} <seealso cref="#valueOf(double)"/> method.
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> {@code double} value to be converted to
		///        {@code BigDecimal}. </param>
		/// <exception cref="NumberFormatException"> if {@code val} is infinite or NaN. </exception>
		public BigDecimal(double val) : this(val,MathContext.UNLIMITED)
		{
		}

		/// <summary>
		/// Translates a {@code double} into a {@code BigDecimal}, with
		/// rounding according to the context settings.  The scale of the
		/// {@code BigDecimal} is the smallest value such that
		/// <tt>(10<sup>scale</sup> &times; val)</tt> is an integer.
		/// 
		/// <para>The results of this constructor can be somewhat unpredictable
		/// and its use is generally not recommended; see the notes under
		/// the <seealso cref="#BigDecimal(double)"/> constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> {@code double} value to be converted to
		///         {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         RoundingMode is UNNECESSARY. </exception>
		/// <exception cref="NumberFormatException"> if {@code val} is infinite or NaN.
		/// @since  1.5 </exception>
		public BigDecimal(double val, MathContext mc)
		{
			if (Double.IsInfinity(val) || Double.IsNaN(val))
			{
				throw new NumberFormatException("Infinite or NaN");
			}
			// Translate the double into sign, exponent and significand, according
			// to the formulae in JLS, Section 20.10.22.
			long valBits = Double.DoubleToLongBits(val);
			int sign = ((valBits >> 63) == 0 ? 1 : -1);
			int exponent = (int)((valBits >> 52) & 0x7ffL);
			long significand = (exponent == 0 ? (valBits & ((1L << 52) - 1)) << 1 : (valBits & ((1L << 52) - 1)) | (1L << 52));
			exponent -= 1075;
			// At this point, val == sign * significand * 2**exponent.

			/*
			 * Special case zero to supress nonterminating normalization and bogus
			 * scale calculation.
			 */
			if (significand == 0)
			{
				this.IntVal = BigInteger.ZERO;
				this.Scale_Renamed = 0;
				this.IntCompact = 0;
				this.Precision_Renamed = 1;
				return;
			}
			// Normalize
			while ((significand & 1) == 0) // i.e., significand is even
			{
				significand >>= 1;
				exponent++;
			}
			int scale = 0;
			// Calculate intVal and scale
			BigInteger intVal;
			long compactVal = sign * significand;
			if (exponent == 0)
			{
				intVal = (compactVal == INFLATED) ? INFLATED_BIGINT : null;
			}
			else
			{
				if (exponent < 0)
				{
					intVal = BigInteger.ValueOf(5).Pow(-exponent).Multiply(compactVal);
					scale = -exponent;
				} //  (exponent > 0)
				else
				{
					intVal = BigInteger.ValueOf(2).Pow(exponent).Multiply(compactVal);
				}
				compactVal = CompactValFor(intVal);
			}
			int prec = 0;
			int mcp = mc.Precision_Renamed;
			if (mcp > 0) // do rounding
			{
				int mode = mc.RoundingMode_Renamed.oldMode;
				int drop;
				if (compactVal == INFLATED)
				{
					prec = BigDigitLength(intVal);
					drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						intVal = DivideAndRoundByTenPow(intVal, drop, mode);
						compactVal = CompactValFor(intVal);
						if (compactVal != INFLATED)
						{
							break;
						}
						prec = BigDigitLength(intVal);
						drop = prec - mcp;
					}
				}
				if (compactVal != INFLATED)
				{
					prec = LongDigitLength(compactVal);
					drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
						prec = LongDigitLength(compactVal);
						drop = prec - mcp;
					}
					intVal = null;
				}
			}
			this.IntVal = intVal;
			this.IntCompact = compactVal;
			this.Scale_Renamed = scale;
			this.Precision_Renamed = prec;
		}

		/// <summary>
		/// Translates a {@code BigInteger} into a {@code BigDecimal}.
		/// The scale of the {@code BigDecimal} is zero.
		/// </summary>
		/// <param name="val"> {@code BigInteger} value to be converted to
		///            {@code BigDecimal}. </param>
		public BigDecimal(BigInteger val)
		{
			Scale_Renamed = 0;
			IntVal = val;
			IntCompact = CompactValFor(val);
		}

		/// <summary>
		/// Translates a {@code BigInteger} into a {@code BigDecimal}
		/// rounding according to the context settings.  The scale of the
		/// {@code BigDecimal} is zero.
		/// </summary>
		/// <param name="val"> {@code BigInteger} value to be converted to
		///            {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public BigDecimal(BigInteger val, MathContext mc) : this(val,0,mc)
		{
		}

		/// <summary>
		/// Translates a {@code BigInteger} unscaled value and an
		/// {@code int} scale into a {@code BigDecimal}.  The value of
		/// the {@code BigDecimal} is
		/// <tt>(unscaledVal &times; 10<sup>-scale</sup>)</tt>.
		/// </summary>
		/// <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
		/// <param name="scale"> scale of the {@code BigDecimal}. </param>
		public BigDecimal(BigInteger unscaledVal, int scale)
		{
			// Negative scales are now allowed
			this.IntVal = unscaledVal;
			this.IntCompact = CompactValFor(unscaledVal);
			this.Scale_Renamed = scale;
		}

		/// <summary>
		/// Translates a {@code BigInteger} unscaled value and an
		/// {@code int} scale into a {@code BigDecimal}, with rounding
		/// according to the context settings.  The value of the
		/// {@code BigDecimal} is <tt>(unscaledVal &times;
		/// 10<sup>-scale</sup>)</tt>, rounded according to the
		/// {@code precision} and rounding mode settings.
		/// </summary>
		/// <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
		/// <param name="scale"> scale of the {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public BigDecimal(BigInteger unscaledVal, int scale, MathContext mc)
		{
			long compactVal = CompactValFor(unscaledVal);
			int mcp = mc.Precision_Renamed;
			int prec = 0;
			if (mcp > 0) // do rounding
			{
				int mode = mc.RoundingMode_Renamed.oldMode;
				if (compactVal == INFLATED)
				{
					prec = BigDigitLength(unscaledVal);
					int drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						unscaledVal = DivideAndRoundByTenPow(unscaledVal, drop, mode);
						compactVal = CompactValFor(unscaledVal);
						if (compactVal != INFLATED)
						{
							break;
						}
						prec = BigDigitLength(unscaledVal);
						drop = prec - mcp;
					}
				}
				if (compactVal != INFLATED)
				{
					prec = LongDigitLength(compactVal);
					int drop = prec - mcp; // drop can't be more than 18
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mode);
						prec = LongDigitLength(compactVal);
						drop = prec - mcp;
					}
					unscaledVal = null;
				}
			}
			this.IntVal = unscaledVal;
			this.IntCompact = compactVal;
			this.Scale_Renamed = scale;
			this.Precision_Renamed = prec;
		}

		/// <summary>
		/// Translates an {@code int} into a {@code BigDecimal}.  The
		/// scale of the {@code BigDecimal} is zero.
		/// </summary>
		/// <param name="val"> {@code int} value to be converted to
		///            {@code BigDecimal}.
		/// @since  1.5 </param>
		public BigDecimal(int val)
		{
			this.IntCompact = val;
			this.Scale_Renamed = 0;
			this.IntVal = null;
		}

		/// <summary>
		/// Translates an {@code int} into a {@code BigDecimal}, with
		/// rounding according to the context settings.  The scale of the
		/// {@code BigDecimal}, before any rounding, is zero.
		/// </summary>
		/// <param name="val"> {@code int} value to be converted to {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public BigDecimal(int val, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			long compactVal = val;
			int scale = 0;
			int prec = 0;
			if (mcp > 0) // do rounding
			{
				prec = LongDigitLength(compactVal);
				int drop = prec - mcp; // drop can't be more than 18
				while (drop > 0)
				{
					scale = CheckScaleNonZero((long) scale - drop);
					compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
					prec = LongDigitLength(compactVal);
					drop = prec - mcp;
				}
			}
			this.IntVal = null;
			this.IntCompact = compactVal;
			this.Scale_Renamed = scale;
			this.Precision_Renamed = prec;
		}

		/// <summary>
		/// Translates a {@code long} into a {@code BigDecimal}.  The
		/// scale of the {@code BigDecimal} is zero.
		/// </summary>
		/// <param name="val"> {@code long} value to be converted to {@code BigDecimal}.
		/// @since  1.5 </param>
		public BigDecimal(long val)
		{
			this.IntCompact = val;
			this.IntVal = (val == INFLATED) ? INFLATED_BIGINT : null;
			this.Scale_Renamed = 0;
		}

		/// <summary>
		/// Translates a {@code long} into a {@code BigDecimal}, with
		/// rounding according to the context settings.  The scale of the
		/// {@code BigDecimal}, before any rounding, is zero.
		/// </summary>
		/// <param name="val"> {@code long} value to be converted to {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public BigDecimal(long val, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			int mode = mc.RoundingMode_Renamed.oldMode;
			int prec = 0;
			int scale = 0;
			BigInteger intVal = (val == INFLATED) ? INFLATED_BIGINT : null;
			if (mcp > 0) // do rounding
			{
				if (val == INFLATED)
				{
					prec = 19;
					int drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						intVal = DivideAndRoundByTenPow(intVal, drop, mode);
						val = CompactValFor(intVal);
						if (val != INFLATED)
						{
							break;
						}
						prec = BigDigitLength(intVal);
						drop = prec - mcp;
					}
				}
				if (val != INFLATED)
				{
					prec = LongDigitLength(val);
					int drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						val = DivideAndRound(val, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
						prec = LongDigitLength(val);
						drop = prec - mcp;
					}
					intVal = null;
				}
			}
			this.IntVal = intVal;
			this.IntCompact = val;
			this.Scale_Renamed = scale;
			this.Precision_Renamed = prec;
		}

		// Static Factory Methods

		/// <summary>
		/// Translates a {@code long} unscaled value and an
		/// {@code int} scale into a {@code BigDecimal}.  This
		/// {@literal "static factory method"} is provided in preference to
		/// a ({@code long}, {@code int}) constructor because it
		/// allows for reuse of frequently used {@code BigDecimal} values..
		/// </summary>
		/// <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
		/// <param name="scale"> scale of the {@code BigDecimal}. </param>
		/// <returns> a {@code BigDecimal} whose value is
		///         <tt>(unscaledVal &times; 10<sup>-scale</sup>)</tt>. </returns>
		public static BigDecimal ValueOf(long unscaledVal, int scale)
		{
			if (scale == 0)
			{
				return ValueOf(unscaledVal);
			}
			else if (unscaledVal == 0)
			{
				return ZeroValueOf(scale);
			}
			return new BigDecimal(unscaledVal == INFLATED ? INFLATED_BIGINT : null, unscaledVal, scale, 0);
		}

		/// <summary>
		/// Translates a {@code long} value into a {@code BigDecimal}
		/// with a scale of zero.  This {@literal "static factory method"}
		/// is provided in preference to a ({@code long}) constructor
		/// because it allows for reuse of frequently used
		/// {@code BigDecimal} values.
		/// </summary>
		/// <param name="val"> value of the {@code BigDecimal}. </param>
		/// <returns> a {@code BigDecimal} whose value is {@code val}. </returns>
		public static BigDecimal ValueOf(long val)
		{
			if (val >= 0 && val < ZeroThroughTen.Length)
			{
				return ZeroThroughTen[(int)val];
			}
			else if (val != INFLATED)
			{
				return new BigDecimal(null, val, 0, 0);
			}
			return new BigDecimal(INFLATED_BIGINT, val, 0, 0);
		}

		internal static BigDecimal ValueOf(long unscaledVal, int scale, int prec)
		{
			if (scale == 0 && unscaledVal >= 0 && unscaledVal < ZeroThroughTen.Length)
			{
				return ZeroThroughTen[(int) unscaledVal];
			}
			else if (unscaledVal == 0)
			{
				return ZeroValueOf(scale);
			}
			return new BigDecimal(unscaledVal == INFLATED ? INFLATED_BIGINT : null, unscaledVal, scale, prec);
		}

		internal static BigDecimal ValueOf(BigInteger intVal, int scale, int prec)
		{
			long val = CompactValFor(intVal);
			if (val == 0)
			{
				return ZeroValueOf(scale);
			}
			else if (scale == 0 && val >= 0 && val < ZeroThroughTen.Length)
			{
				return ZeroThroughTen[(int) val];
			}
			return new BigDecimal(intVal, val, scale, prec);
		}

		internal static BigDecimal ZeroValueOf(int scale)
		{
			if (scale >= 0 && scale < ZERO_SCALED_BY.Length)
			{
				return ZERO_SCALED_BY[scale];
			}
			else
			{
				return new BigDecimal(BigInteger.ZERO, 0, scale, 1);
			}
		}

		/// <summary>
		/// Translates a {@code double} into a {@code BigDecimal}, using
		/// the {@code double}'s canonical string representation provided
		/// by the <seealso cref="Double#toString(double)"/> method.
		/// 
		/// <para><b>Note:</b> This is generally the preferred way to convert
		/// a {@code double} (or {@code float}) into a
		/// {@code BigDecimal}, as the value returned is equal to that
		/// resulting from constructing a {@code BigDecimal} from the
		/// result of using <seealso cref="Double#toString(double)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> {@code double} to convert to a {@code BigDecimal}. </param>
		/// <returns> a {@code BigDecimal} whose value is equal to or approximately
		///         equal to the value of {@code val}. </returns>
		/// <exception cref="NumberFormatException"> if {@code val} is infinite or NaN.
		/// @since  1.5 </exception>
		public static BigDecimal ValueOf(double val)
		{
			// Reminder: a zero double returns '0.0', so we cannot fastpath
			// to use the constant ZERO.  This might be important enough to
			// justify a factory approach, a cache, or a few private
			// constants, later.
			return new BigDecimal(Convert.ToString(val));
		}

		// Arithmetic Operations
		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this +
		/// augend)}, and whose scale is {@code max(this.scale(),
		/// augend.scale())}.
		/// </summary>
		/// <param name="augend"> value to be added to this {@code BigDecimal}. </param>
		/// <returns> {@code this + augend} </returns>
		public virtual BigDecimal Add(BigDecimal augend)
		{
			if (this.IntCompact != INFLATED)
			{
				if ((augend.IntCompact != INFLATED))
				{
					return Add(this.IntCompact, this.Scale_Renamed, augend.IntCompact, augend.Scale_Renamed);
				}
				else
				{
					return Add(this.IntCompact, this.Scale_Renamed, augend.IntVal, augend.Scale_Renamed);
				}
			}
			else
			{
				if ((augend.IntCompact != INFLATED))
				{
					return Add(augend.IntCompact, augend.Scale_Renamed, this.IntVal, this.Scale_Renamed);
				}
				else
				{
					return Add(this.IntVal, this.Scale_Renamed, augend.IntVal, augend.Scale_Renamed);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this + augend)},
		/// with rounding according to the context settings.
		/// 
		/// If either number is zero and the precision setting is nonzero then
		/// the other number, rounded if necessary, is used as the result.
		/// </summary>
		/// <param name="augend"> value to be added to this {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this + augend}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public virtual BigDecimal Add(BigDecimal augend, MathContext mc)
		{
			if (mc.Precision_Renamed == 0)
			{
				return Add(augend);
			}
			BigDecimal lhs = this;

			// If either number is zero then the other number, rounded and
			// scaled if necessary, is used as the result.
			{
				bool lhsIsZero = lhs.Signum() == 0;
				bool augendIsZero = augend.Signum() == 0;

				if (lhsIsZero || augendIsZero)
				{
					int preferredScale = System.Math.Max(lhs.Scale(), augend.Scale());
					BigDecimal result;

					if (lhsIsZero && augendIsZero)
					{
						return ZeroValueOf(preferredScale);
					}
					result = lhsIsZero ? DoRound(augend, mc) : DoRound(lhs, mc);

					if (result.Scale() == preferredScale)
					{
						return result;
					}
					else if (result.Scale() > preferredScale)
					{
						return StripZerosToMatchScale(result.IntVal, result.IntCompact, result.Scale_Renamed, preferredScale);
					} // result.scale < preferredScale
					else
					{
						int precisionDiff = mc.Precision_Renamed - result.Precision();
						int scaleDiff = preferredScale - result.Scale();

						if (precisionDiff >= scaleDiff)
						{
							return result.setScale(preferredScale); // can achieve target scale
						}
						else
						{
							return result.setScale(result.Scale() + precisionDiff);
						}
					}
				}
			}

			long padding = (long) lhs.Scale_Renamed - augend.Scale_Renamed;
			if (padding != 0) // scales differ; alignment needed
			{
				BigDecimal[] arg = PreAlign(lhs, augend, padding, mc);
				MatchScale(arg);
				lhs = arg[0];
				augend = arg[1];
			}
			return DoRound(lhs.Inflated().Add(augend.Inflated()), lhs.Scale_Renamed, mc);
		}

		/// <summary>
		/// Returns an array of length two, the sum of whose entries is
		/// equal to the rounded sum of the {@code BigDecimal} arguments.
		/// 
		/// <para>If the digit positions of the arguments have a sufficient
		/// gap between them, the value smaller in magnitude can be
		/// condensed into a {@literal "sticky bit"} and the end result will
		/// round the same way <em>if</em> the precision of the final
		/// result does not include the high order digit of the small
		/// magnitude operand.
		/// 
		/// </para>
		/// <para>Note that while strictly speaking this is an optimization,
		/// it makes a much wider range of additions practical.
		/// 
		/// </para>
		/// <para>This corresponds to a pre-shift operation in a fixed
		/// precision floating-point adder; this method is complicated by
		/// variable precision of the result as determined by the
		/// MathContext.  A more nuanced operation could implement a
		/// {@literal "right shift"} on the smaller magnitude operand so
		/// that the number of digits of the smaller operand could be
		/// reduced even though the significands partially overlapped.
		/// </para>
		/// </summary>
		private BigDecimal[] PreAlign(BigDecimal lhs, BigDecimal augend, long padding, MathContext mc)
		{
			Debug.Assert(padding != 0);
			BigDecimal big;
			BigDecimal small;

			if (padding < 0) // lhs is big; augend is small
			{
				big = lhs;
				small = augend;
			} // lhs is small; augend is big
			else
			{
				big = augend;
				small = lhs;
			}

			/*
			 * This is the estimated scale of an ulp of the result; it assumes that
			 * the result doesn't have a carry-out on a true add (e.g. 999 + 1 =>
			 * 1000) or any subtractive cancellation on borrowing (e.g. 100 - 1.2 =>
			 * 98.8)
			 */
			long estResultUlpScale = (long) big.Scale_Renamed - big.Precision() + mc.Precision_Renamed;

			/*
			 * The low-order digit position of big is big.scale().  This
			 * is true regardless of whether big has a positive or
			 * negative scale.  The high-order digit position of small is
			 * small.scale - (small.precision() - 1).  To do the full
			 * condensation, the digit positions of big and small must be
			 * disjoint *and* the digit positions of small should not be
			 * directly visible in the result.
			 */
			long smallHighDigitPos = (long) small.Scale_Renamed - small.Precision() + 1;
			if (smallHighDigitPos > big.Scale_Renamed + 2 && smallHighDigitPos > estResultUlpScale + 2) // small digits not visible -  big and small disjoint
			{
				small = BigDecimal.ValueOf(small.Signum(), this.CheckScale(System.Math.Max(big.Scale_Renamed, estResultUlpScale) + 3));
			}

			// Since addition is symmetric, preserving input order in
			// returned operands doesn't matter
			BigDecimal[] result = new BigDecimal[] {big, small};
			return result;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this -
		/// subtrahend)}, and whose scale is {@code max(this.scale(),
		/// subtrahend.scale())}.
		/// </summary>
		/// <param name="subtrahend"> value to be subtracted from this {@code BigDecimal}. </param>
		/// <returns> {@code this - subtrahend} </returns>
		public virtual BigDecimal Subtract(BigDecimal subtrahend)
		{
			if (this.IntCompact != INFLATED)
			{
				if ((subtrahend.IntCompact != INFLATED))
				{
					return Add(this.IntCompact, this.Scale_Renamed, -subtrahend.IntCompact, subtrahend.Scale_Renamed);
				}
				else
				{
					return Add(this.IntCompact, this.Scale_Renamed, subtrahend.IntVal.Negate(), subtrahend.Scale_Renamed);
				}
			}
			else
			{
				if ((subtrahend.IntCompact != INFLATED))
				{
					// Pair of subtrahend values given before pair of
					// values from this BigDecimal to avoid need for
					// method overloading on the specialized add method
					return Add(-subtrahend.IntCompact, subtrahend.Scale_Renamed, this.IntVal, this.Scale_Renamed);
				}
				else
				{
					return Add(this.IntVal, this.Scale_Renamed, subtrahend.IntVal.Negate(), subtrahend.Scale_Renamed);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this - subtrahend)},
		/// with rounding according to the context settings.
		/// 
		/// If {@code subtrahend} is zero then this, rounded if necessary, is used as the
		/// result.  If this is zero then the result is {@code subtrahend.negate(mc)}.
		/// </summary>
		/// <param name="subtrahend"> value to be subtracted from this {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this - subtrahend}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public virtual BigDecimal Subtract(BigDecimal subtrahend, MathContext mc)
		{
			if (mc.Precision_Renamed == 0)
			{
				return Subtract(subtrahend);
			}
			// share the special rounding code in add()
			return Add(subtrahend.Negate(), mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is <tt>(this &times;
		/// multiplicand)</tt>, and whose scale is {@code (this.scale() +
		/// multiplicand.scale())}.
		/// </summary>
		/// <param name="multiplicand"> value to be multiplied by this {@code BigDecimal}. </param>
		/// <returns> {@code this * multiplicand} </returns>
		public virtual BigDecimal Multiply(BigDecimal multiplicand)
		{
			int productScale = CheckScale((long) Scale_Renamed + multiplicand.Scale_Renamed);
			if (this.IntCompact != INFLATED)
			{
				if ((multiplicand.IntCompact != INFLATED))
				{
					return Multiply(this.IntCompact, multiplicand.IntCompact, productScale);
				}
				else
				{
					return Multiply(this.IntCompact, multiplicand.IntVal, productScale);
				}
			}
			else
			{
				if ((multiplicand.IntCompact != INFLATED))
				{
					return Multiply(multiplicand.IntCompact, this.IntVal, productScale);
				}
				else
				{
					return Multiply(this.IntVal, multiplicand.IntVal, productScale);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is <tt>(this &times;
		/// multiplicand)</tt>, with rounding according to the context settings.
		/// </summary>
		/// <param name="multiplicand"> value to be multiplied by this {@code BigDecimal}. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this * multiplicand}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public virtual BigDecimal Multiply(BigDecimal multiplicand, MathContext mc)
		{
			if (mc.Precision_Renamed == 0)
			{
				return Multiply(multiplicand);
			}
			int productScale = CheckScale((long) Scale_Renamed + multiplicand.Scale_Renamed);
			if (this.IntCompact != INFLATED)
			{
				if ((multiplicand.IntCompact != INFLATED))
				{
					return MultiplyAndRound(this.IntCompact, multiplicand.IntCompact, productScale, mc);
				}
				else
				{
					return MultiplyAndRound(this.IntCompact, multiplicand.IntVal, productScale, mc);
				}
			}
			else
			{
				if ((multiplicand.IntCompact != INFLATED))
				{
					return MultiplyAndRound(multiplicand.IntCompact, this.IntVal, productScale, mc);
				}
				else
				{
					return MultiplyAndRound(this.IntVal, multiplicand.IntVal, productScale, mc);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, and whose scale is as specified.  If rounding must
		/// be performed to generate a result with the specified scale, the
		/// specified rounding mode is applied.
		/// 
		/// <para>The new <seealso cref="#divide(BigDecimal, int, RoundingMode)"/> method
		/// should be used in preference to this legacy method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="scale"> scale of the {@code BigDecimal} quotient to be returned. </param>
		/// <param name="roundingMode"> rounding mode to apply. </param>
		/// <returns> {@code this / divisor} </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor} is zero,
		///         {@code roundingMode==ROUND_UNNECESSARY} and
		///         the specified scale is insufficient to represent the result
		///         of the division exactly. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
		///         represent a valid rounding mode. </exception>
		/// <seealso cref=    #ROUND_UP </seealso>
		/// <seealso cref=    #ROUND_DOWN </seealso>
		/// <seealso cref=    #ROUND_CEILING </seealso>
		/// <seealso cref=    #ROUND_FLOOR </seealso>
		/// <seealso cref=    #ROUND_HALF_UP </seealso>
		/// <seealso cref=    #ROUND_HALF_DOWN </seealso>
		/// <seealso cref=    #ROUND_HALF_EVEN </seealso>
		/// <seealso cref=    #ROUND_UNNECESSARY </seealso>
		public virtual BigDecimal Divide(BigDecimal divisor, int scale, int roundingMode)
		{
			if (roundingMode < ROUND_UP || roundingMode > ROUND_UNNECESSARY)
			{
				throw new IllegalArgumentException("Invalid rounding mode");
			}
			if (this.IntCompact != INFLATED)
			{
				if ((divisor.IntCompact != INFLATED))
				{
					return Divide(this.IntCompact, this.Scale_Renamed, divisor.IntCompact, divisor.Scale_Renamed, scale, roundingMode);
				}
				else
				{
					return Divide(this.IntCompact, this.Scale_Renamed, divisor.IntVal, divisor.Scale_Renamed, scale, roundingMode);
				}
			}
			else
			{
				if ((divisor.IntCompact != INFLATED))
				{
					return Divide(this.IntVal, this.Scale_Renamed, divisor.IntCompact, divisor.Scale_Renamed, scale, roundingMode);
				}
				else
				{
					return Divide(this.IntVal, this.Scale_Renamed, divisor.IntVal, divisor.Scale_Renamed, scale, roundingMode);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, and whose scale is as specified.  If rounding must
		/// be performed to generate a result with the specified scale, the
		/// specified rounding mode is applied.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="scale"> scale of the {@code BigDecimal} quotient to be returned. </param>
		/// <param name="roundingMode"> rounding mode to apply. </param>
		/// <returns> {@code this / divisor} </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor} is zero,
		///         {@code roundingMode==RoundingMode.UNNECESSARY} and
		///         the specified scale is insufficient to represent the result
		///         of the division exactly.
		/// @since 1.5 </exception>
		public virtual BigDecimal Divide(BigDecimal divisor, int scale, RoundingMode roundingMode)
		{
			return Divide(divisor, scale, roundingMode.oldMode);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, and whose scale is {@code this.scale()}.  If
		/// rounding must be performed to generate a result with the given
		/// scale, the specified rounding mode is applied.
		/// 
		/// <para>The new <seealso cref="#divide(BigDecimal, RoundingMode)"/> method
		/// should be used in preference to this legacy method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="roundingMode"> rounding mode to apply. </param>
		/// <returns> {@code this / divisor} </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0}, or
		///         {@code roundingMode==ROUND_UNNECESSARY} and
		///         {@code this.scale()} is insufficient to represent the result
		///         of the division exactly. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
		///         represent a valid rounding mode. </exception>
		/// <seealso cref=    #ROUND_UP </seealso>
		/// <seealso cref=    #ROUND_DOWN </seealso>
		/// <seealso cref=    #ROUND_CEILING </seealso>
		/// <seealso cref=    #ROUND_FLOOR </seealso>
		/// <seealso cref=    #ROUND_HALF_UP </seealso>
		/// <seealso cref=    #ROUND_HALF_DOWN </seealso>
		/// <seealso cref=    #ROUND_HALF_EVEN </seealso>
		/// <seealso cref=    #ROUND_UNNECESSARY </seealso>
		public virtual BigDecimal Divide(BigDecimal divisor, int roundingMode)
		{
			return this.Divide(divisor, Scale_Renamed, roundingMode);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, and whose scale is {@code this.scale()}.  If
		/// rounding must be performed to generate a result with the given
		/// scale, the specified rounding mode is applied.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="roundingMode"> rounding mode to apply. </param>
		/// <returns> {@code this / divisor} </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0}, or
		///         {@code roundingMode==RoundingMode.UNNECESSARY} and
		///         {@code this.scale()} is insufficient to represent the result
		///         of the division exactly.
		/// @since 1.5 </exception>
		public virtual BigDecimal Divide(BigDecimal divisor, RoundingMode roundingMode)
		{
			return this.Divide(divisor, Scale_Renamed, roundingMode.oldMode);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, and whose preferred scale is {@code (this.scale() -
		/// divisor.scale())}; if the exact quotient cannot be
		/// represented (because it has a non-terminating decimal
		/// expansion) an {@code ArithmeticException} is thrown.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <exception cref="ArithmeticException"> if the exact quotient does not have a
		///         terminating decimal expansion </exception>
		/// <returns> {@code this / divisor}
		/// @since 1.5
		/// @author Joseph D. Darcy </returns>
		public virtual BigDecimal Divide(BigDecimal divisor)
		{
			/*
			 * Handle zero cases first.
			 */
			if (divisor.Signum() == 0) // x/0
			{
				if (this.Signum() == 0) // 0/0
				{
					throw new ArithmeticException("Division undefined"); // NaN
				}
				throw new ArithmeticException("Division by zero");
			}

			// Calculate preferred scale
			int preferredScale = SaturateLong((long) this.Scale_Renamed - divisor.Scale_Renamed);

			if (this.Signum() == 0) // 0/y
			{
				return ZeroValueOf(preferredScale);
			}
			else
			{
				/*
				 * If the quotient this/divisor has a terminating decimal
				 * expansion, the expansion can have no more than
				 * (a.precision() + ceil(10*b.precision)/3) digits.
				 * Therefore, create a MathContext object with this
				 * precision and do a divide with the UNNECESSARY rounding
				 * mode.
				 */
				MathContext mc = new MathContext((int)System.Math.Min(this.Precision() + (long)System.Math.Ceiling(10.0 * divisor.Precision() / 3.0), Integer.MaxValue), RoundingMode.UNNECESSARY);
				BigDecimal quotient;
				try
				{
					quotient = this.Divide(divisor, mc);
				}
				catch (ArithmeticException)
				{
					throw new ArithmeticException("Non-terminating decimal expansion; " + "no exact representable decimal result.");
				}

				int quotientScale = quotient.Scale();

				// divide(BigDecimal, mc) tries to adjust the quotient to
				// the desired one by removing trailing zeros; since the
				// exact divide method does not have an explicit digit
				// limit, we can add zeros too.
				if (preferredScale > quotientScale)
				{
					return quotient.SetScale(preferredScale, ROUND_UNNECESSARY);
				}

				return quotient;
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this /
		/// divisor)}, with rounding according to the context settings.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this / divisor}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY} or
		///         {@code mc.precision == 0} and the quotient has a
		///         non-terminating decimal expansion.
		/// @since  1.5 </exception>
		public virtual BigDecimal Divide(BigDecimal divisor, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			if (mcp == 0)
			{
				return Divide(divisor);
			}

			BigDecimal dividend = this;
			long preferredScale = (long)dividend.Scale_Renamed - divisor.Scale_Renamed;
			// Now calculate the answer.  We use the existing
			// divide-and-round method, but as this rounds to scale we have
			// to normalize the values here to achieve the desired result.
			// For x/y we first handle y=0 and x=0, and then normalize x and
			// y to give x' and y' with the following constraints:
			//   (a) 0.1 <= x' < 1
			//   (b)  x' <= y' < 10*x'
			// Dividing x'/y' with the required scale set to mc.precision then
			// will give a result in the range 0.1 to 1 rounded to exactly
			// the right number of digits (except in the case of a result of
			// 1.000... which can arise when x=y, or when rounding overflows
			// The 1.000... case will reduce properly to 1.
			if (divisor.Signum() == 0) // x/0
			{
				if (dividend.Signum() == 0) // 0/0
				{
					throw new ArithmeticException("Division undefined"); // NaN
				}
				throw new ArithmeticException("Division by zero");
			}
			if (dividend.Signum() == 0) // 0/y
			{
				return ZeroValueOf(SaturateLong(preferredScale));
			}
			int xscale = dividend.Precision();
			int yscale = divisor.Precision();
			if (dividend.IntCompact != INFLATED)
			{
				if (divisor.IntCompact != INFLATED)
				{
					return Divide(dividend.IntCompact, xscale, divisor.IntCompact, yscale, preferredScale, mc);
				}
				else
				{
					return Divide(dividend.IntCompact, xscale, divisor.IntVal, yscale, preferredScale, mc);
				}
			}
			else
			{
				if (divisor.IntCompact != INFLATED)
				{
					return Divide(dividend.IntVal, xscale, divisor.IntCompact, yscale, preferredScale, mc);
				}
				else
				{
					return Divide(dividend.IntVal, xscale, divisor.IntVal, yscale, preferredScale, mc);
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is the integer part
		/// of the quotient {@code (this / divisor)} rounded down.  The
		/// preferred scale of the result is {@code (this.scale() -
		/// divisor.scale())}.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <returns> The integer part of {@code this / divisor}. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0}
		/// @since  1.5 </exception>
		public virtual BigDecimal DivideToIntegralValue(BigDecimal divisor)
		{
			// Calculate preferred scale
			int preferredScale = SaturateLong((long) this.Scale_Renamed - divisor.Scale_Renamed);
			if (this.CompareMagnitude(divisor) < 0)
			{
				// much faster when this << divisor
				return ZeroValueOf(preferredScale);
			}

			if (this.Signum() == 0 && divisor.Signum() != 0)
			{
				return this.SetScale(preferredScale, ROUND_UNNECESSARY);
			}

			// Perform a divide with enough digits to round to a correct
			// integer value; then remove any fractional digits

			int maxDigits = (int)System.Math.Min(this.Precision() + (long)System.Math.Ceiling(10.0 * divisor.Precision() / 3.0) + System.Math.Abs((long)this.Scale() - divisor.Scale()) + 2, Integer.MaxValue);
			BigDecimal quotient = this.Divide(divisor, new MathContext(maxDigits, RoundingMode.DOWN));
			if (quotient.Scale_Renamed > 0)
			{
				quotient = quotient.SetScale(0, RoundingMode.DOWN);
				quotient = StripZerosToMatchScale(quotient.IntVal, quotient.IntCompact, quotient.Scale_Renamed, preferredScale);
			}

			if (quotient.Scale_Renamed < preferredScale)
			{
				// pad with zeros if necessary
				quotient = quotient.SetScale(preferredScale, ROUND_UNNECESSARY);
			}

			return quotient;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is the integer part
		/// of {@code (this / divisor)}.  Since the integer part of the
		/// exact quotient does not depend on the rounding mode, the
		/// rounding mode does not affect the values returned by this
		/// method.  The preferred scale of the result is
		/// {@code (this.scale() - divisor.scale())}.  An
		/// {@code ArithmeticException} is thrown if the integer part of
		/// the exact quotient needs more than {@code mc.precision}
		/// digits.
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> The integer part of {@code this / divisor}. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
		/// <exception cref="ArithmeticException"> if {@code mc.precision} {@literal >} 0 and the result
		///         requires a precision of more than {@code mc.precision} digits.
		/// @since  1.5
		/// @author Joseph D. Darcy </exception>
		public virtual BigDecimal DivideToIntegralValue(BigDecimal divisor, MathContext mc)
		{
			if (mc.Precision_Renamed == 0 || (this.CompareMagnitude(divisor) < 0)) // zero result -  exact result
			{
				return DivideToIntegralValue(divisor);
			}

			// Calculate preferred scale
			int preferredScale = SaturateLong((long)this.Scale_Renamed - divisor.Scale_Renamed);

			/*
			 * Perform a normal divide to mc.precision digits.  If the
			 * remainder has absolute value less than the divisor, the
			 * integer portion of the quotient fits into mc.precision
			 * digits.  Next, remove any fractional digits from the
			 * quotient and adjust the scale to the preferred value.
			 */
			BigDecimal result = this.Divide(divisor, new MathContext(mc.Precision_Renamed, RoundingMode.DOWN));

			if (result.Scale() < 0)
			{
				/*
				 * Result is an integer. See if quotient represents the
				 * full integer portion of the exact quotient; if it does,
				 * the computed remainder will be less than the divisor.
				 */
				BigDecimal product = result.Multiply(divisor);
				// If the quotient is the full integer value,
				// |dividend-product| < |divisor|.
				if (this.Subtract(product).CompareMagnitude(divisor) >= 0)
				{
					throw new ArithmeticException("Division impossible");
				}
			}
			else if (result.Scale() > 0)
			{
				/*
				 * Integer portion of quotient will fit into precision
				 * digits; recompute quotient to scale 0 to avoid double
				 * rounding and then try to adjust, if necessary.
				 */
				result = result.SetScale(0, RoundingMode.DOWN);
			}
			// else result.scale() == 0;

			int precisionDiff;
			if ((preferredScale > result.Scale()) && (precisionDiff = mc.Precision_Renamed - result.Precision()) > 0)
			{
				return result.setScale(result.Scale() + System.Math.Min(precisionDiff, preferredScale - result.Scale_Renamed));
			}
			else
			{
				return StripZerosToMatchScale(result.IntVal,result.IntCompact,result.Scale_Renamed,preferredScale);
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this % divisor)}.
		/// 
		/// <para>The remainder is given by
		/// {@code this.subtract(this.divideToIntegralValue(divisor).multiply(divisor))}.
		/// Note that this is not the modulo operation (the result can be
		/// negative).
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <returns> {@code this % divisor}. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0}
		/// @since  1.5 </exception>
		public virtual BigDecimal Remainder(BigDecimal divisor)
		{
			BigDecimal[] divrem = this.DivideAndRemainder(divisor);
			return divrem[1];
		}


		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (this %
		/// divisor)}, with rounding according to the context settings.
		/// The {@code MathContext} settings affect the implicit divide
		/// used to compute the remainder.  The remainder computation
		/// itself is by definition exact.  Therefore, the remainder may
		/// contain more than {@code mc.getPrecision()} digits.
		/// 
		/// <para>The remainder is given by
		/// {@code this.subtract(this.divideToIntegralValue(divisor,
		/// mc).multiply(divisor))}.  Note that this is not the modulo
		/// operation (the result can be negative).
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this % divisor}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}, or {@code mc.precision}
		///         {@literal >} 0 and the result of {@code this.divideToIntgralValue(divisor)} would
		///         require a precision of more than {@code mc.precision} digits. </exception>
		/// <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext)
		/// @since  1.5 </seealso>
		public virtual BigDecimal Remainder(BigDecimal divisor, MathContext mc)
		{
			BigDecimal[] divrem = this.DivideAndRemainder(divisor, mc);
			return divrem[1];
		}

		/// <summary>
		/// Returns a two-element {@code BigDecimal} array containing the
		/// result of {@code divideToIntegralValue} followed by the result of
		/// {@code remainder} on the two operands.
		/// 
		/// <para>Note that if both the integer quotient and remainder are
		/// needed, this method is faster than using the
		/// {@code divideToIntegralValue} and {@code remainder} methods
		/// separately because the division need only be carried out once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided,
		///         and the remainder computed. </param>
		/// <returns> a two element {@code BigDecimal} array: the quotient
		///         (the result of {@code divideToIntegralValue}) is the initial element
		///         and the remainder is the final element. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
		/// <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext) </seealso>
		/// <seealso cref=    #remainder(java.math.BigDecimal, java.math.MathContext)
		/// @since  1.5 </seealso>
		public virtual BigDecimal[] DivideAndRemainder(BigDecimal divisor)
		{
			// we use the identity  x = i * y + r to determine r
			BigDecimal[] result = new BigDecimal[2];

			result[0] = this.DivideToIntegralValue(divisor);
			result[1] = this.Subtract(result[0].Multiply(divisor));
			return result;
		}

		/// <summary>
		/// Returns a two-element {@code BigDecimal} array containing the
		/// result of {@code divideToIntegralValue} followed by the result of
		/// {@code remainder} on the two operands calculated with rounding
		/// according to the context settings.
		/// 
		/// <para>Note that if both the integer quotient and remainder are
		/// needed, this method is faster than using the
		/// {@code divideToIntegralValue} and {@code remainder} methods
		/// separately because the division need only be carried out once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor"> value by which this {@code BigDecimal} is to be divided,
		///         and the remainder computed. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> a two element {@code BigDecimal} array: the quotient
		///         (the result of {@code divideToIntegralValue}) is the
		///         initial element and the remainder is the final element. </returns>
		/// <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}, or {@code mc.precision}
		///         {@literal >} 0 and the result of {@code this.divideToIntgralValue(divisor)} would
		///         require a precision of more than {@code mc.precision} digits. </exception>
		/// <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext) </seealso>
		/// <seealso cref=    #remainder(java.math.BigDecimal, java.math.MathContext)
		/// @since  1.5 </seealso>
		public virtual BigDecimal[] DivideAndRemainder(BigDecimal divisor, MathContext mc)
		{
			if (mc.Precision_Renamed == 0)
			{
				return DivideAndRemainder(divisor);
			}

			BigDecimal[] result = new BigDecimal[2];
			BigDecimal lhs = this;

			result[0] = lhs.DivideToIntegralValue(divisor, mc);
			result[1] = lhs.Subtract(result[0].Multiply(divisor));
			return result;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is
		/// <tt>(this<sup>n</sup>)</tt>, The power is computed exactly, to
		/// unlimited precision.
		/// 
		/// <para>The parameter {@code n} must be in the range 0 through
		/// 999999999, inclusive.  {@code ZERO.pow(0)} returns {@link
		/// #ONE}.
		/// 
		/// Note that future releases may expand the allowable exponent
		/// range of this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n"> power to raise this {@code BigDecimal} to. </param>
		/// <returns> <tt>this<sup>n</sup></tt> </returns>
		/// <exception cref="ArithmeticException"> if {@code n} is out of range.
		/// @since  1.5 </exception>
		public virtual BigDecimal Pow(int n)
		{
			if (n < 0 || n > 999999999)
			{
				throw new ArithmeticException("Invalid operation");
			}
			// No need to calculate pow(n) if result will over/underflow.
			// Don't attempt to support "supernormal" numbers.
			int newScale = CheckScale((long)Scale_Renamed * n);
			return new BigDecimal(this.Inflated().Pow(n), newScale);
		}


		/// <summary>
		/// Returns a {@code BigDecimal} whose value is
		/// <tt>(this<sup>n</sup>)</tt>.  The current implementation uses
		/// the core algorithm defined in ANSI standard X3.274-1996 with
		/// rounding according to the context settings.  In general, the
		/// returned numerical value is within two ulps of the exact
		/// numerical value for the chosen precision.  Note that future
		/// releases may use a different algorithm with a decreased
		/// allowable error bound and increased allowable exponent range.
		/// 
		/// <para>The X3.274-1996 algorithm is:
		/// 
		/// <ul>
		/// <li> An {@code ArithmeticException} exception is thrown if
		///  <ul>
		///    <li>{@code abs(n) > 999999999}
		///    <li>{@code mc.precision == 0} and {@code n < 0}
		///    <li>{@code mc.precision > 0} and {@code n} has more than
		///    {@code mc.precision} decimal digits
		///  </ul>
		/// 
		/// <li> if {@code n} is zero, <seealso cref="#ONE"/> is returned even if
		/// {@code this} is zero, otherwise
		/// <ul>
		///   <li> if {@code n} is positive, the result is calculated via
		///   the repeated squaring technique into a single accumulator.
		///   The individual multiplications with the accumulator use the
		///   same math context settings as in {@code mc} except for a
		///   precision increased to {@code mc.precision + elength + 1}
		///   where {@code elength} is the number of decimal digits in
		///   {@code n}.
		/// 
		///   <li> if {@code n} is negative, the result is calculated as if
		///   {@code n} were positive; this value is then divided into one
		///   using the working precision specified above.
		/// 
		///   <li> The final value from either the positive or negative case
		///   is then rounded to the destination precision.
		///   </ul>
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="n"> power to raise this {@code BigDecimal} to. </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> <tt>this<sup>n</sup></tt> using the ANSI standard X3.274-1996
		///         algorithm </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}, or {@code n} is out
		///         of range.
		/// @since  1.5 </exception>
		public virtual BigDecimal Pow(int n, MathContext mc)
		{
			if (mc.Precision_Renamed == 0)
			{
				return Pow(n);
			}
			if (n < -999999999 || n > 999999999)
			{
				throw new ArithmeticException("Invalid operation");
			}
			if (n == 0)
			{
				return ONE; // x**0 == 1 in X3.274
			}
			BigDecimal lhs = this;
			MathContext workmc = mc; // working settings
			int mag = System.Math.Abs(n); // magnitude of n
			if (mc.Precision_Renamed > 0)
			{
				int elength = LongDigitLength(mag); // length of n in digits
				if (elength > mc.Precision_Renamed) // X3.274 rule
				{
					throw new ArithmeticException("Invalid operation");
				}
				workmc = new MathContext(mc.Precision_Renamed + elength + 1, mc.RoundingMode_Renamed);
			}
			// ready to carry out power calculation...
			BigDecimal acc = ONE; // accumulator
			bool seenbit = false; // set once we've seen a 1-bit
			for (int i = 1;;i++) // for each bit [top bit ignored]
			{
				mag += mag; // shift left 1 bit
				if (mag < 0) // top bit is set
				{
					seenbit = true; // OK, we're off
					acc = acc.Multiply(lhs, workmc); // acc=acc*x
				}
				if (i == 31)
				{
					break; // that was the last bit
				}
				if (seenbit)
				{
					acc = acc.Multiply(acc, workmc); // acc=acc*acc [square]
				}
					// else (!seenbit) no point in squaring ONE
			}
			// if negative n, calculate the reciprocal using working precision
			if (n < 0) // [hence mc.precision>0]
			{
				acc = ONE.Divide(acc, workmc);
			}
			// round to final precision and strip zeros
			return DoRound(acc, mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is the absolute value
		/// of this {@code BigDecimal}, and whose scale is
		/// {@code this.scale()}.
		/// </summary>
		/// <returns> {@code abs(this)} </returns>
		public virtual BigDecimal Abs()
		{
			return (Signum() < 0 ? Negate() : this);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is the absolute value
		/// of this {@code BigDecimal}, with rounding according to the
		/// context settings.
		/// </summary>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code abs(this)}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since 1.5 </exception>
		public virtual BigDecimal Abs(MathContext mc)
		{
			return (Signum() < 0 ? Negate(mc) : Plus(mc));
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (-this)},
		/// and whose scale is {@code this.scale()}.
		/// </summary>
		/// <returns> {@code -this}. </returns>
		public virtual BigDecimal Negate()
		{
			if (IntCompact == INFLATED)
			{
				return new BigDecimal(IntVal.Negate(), INFLATED, Scale_Renamed, Precision_Renamed);
			}
			else
			{
				return ValueOf(-IntCompact, Scale_Renamed, Precision_Renamed);
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (-this)},
		/// with rounding according to the context settings.
		/// </summary>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code -this}, rounded as necessary. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}.
		/// @since  1.5 </exception>
		public virtual BigDecimal Negate(MathContext mc)
		{
			return Negate().Plus(mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (+this)}, and whose
		/// scale is {@code this.scale()}.
		/// 
		/// <para>This method, which simply returns this {@code BigDecimal}
		/// is included for symmetry with the unary minus method {@link
		/// #negate()}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code this}. </returns>
		/// <seealso cref= #negate()
		/// @since  1.5 </seealso>
		public virtual BigDecimal Plus()
		{
			return this;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (+this)},
		/// with rounding according to the context settings.
		/// 
		/// <para>The effect of this method is identical to that of the {@link
		/// #round(MathContext)} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mc"> the context to use. </param>
		/// <returns> {@code this}, rounded as necessary.  A zero result will
		///         have a scale of 0. </returns>
		/// <exception cref="ArithmeticException"> if the result is inexact but the
		///         rounding mode is {@code UNNECESSARY}. </exception>
		/// <seealso cref=    #round(MathContext)
		/// @since  1.5 </seealso>
		public virtual BigDecimal Plus(MathContext mc)
		{
			if (mc.Precision_Renamed == 0) // no rounding please
			{
				return this;
			}
			return DoRound(this, mc);
		}

		/// <summary>
		/// Returns the signum function of this {@code BigDecimal}.
		/// </summary>
		/// <returns> -1, 0, or 1 as the value of this {@code BigDecimal}
		///         is negative, zero, or positive. </returns>
		public virtual int Signum()
		{
			return (IntCompact != INFLATED)? Long.Signum(IntCompact): IntVal.Signum();
		}

		/// <summary>
		/// Returns the <i>scale</i> of this {@code BigDecimal}.  If zero
		/// or positive, the scale is the number of digits to the right of
		/// the decimal point.  If negative, the unscaled value of the
		/// number is multiplied by ten to the power of the negation of the
		/// scale.  For example, a scale of {@code -3} means the unscaled
		/// value is multiplied by 1000.
		/// </summary>
		/// <returns> the scale of this {@code BigDecimal}. </returns>
		public virtual int Scale()
		{
			return Scale_Renamed;
		}

		/// <summary>
		/// Returns the <i>precision</i> of this {@code BigDecimal}.  (The
		/// precision is the number of digits in the unscaled value.)
		/// 
		/// <para>The precision of a zero value is 1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the precision of this {@code BigDecimal}.
		/// @since  1.5 </returns>
		public virtual int Precision()
		{
			int result = Precision_Renamed;
			if (result == 0)
			{
				long s = IntCompact;
				if (s != INFLATED)
				{
					result = LongDigitLength(s);
				}
				else
				{
					result = BigDigitLength(IntVal);
				}
				Precision_Renamed = result;
			}
			return result;
		}


		/// <summary>
		/// Returns a {@code BigInteger} whose value is the <i>unscaled
		/// value</i> of this {@code BigDecimal}.  (Computes <tt>(this *
		/// 10<sup>this.scale()</sup>)</tt>.)
		/// </summary>
		/// <returns> the unscaled value of this {@code BigDecimal}.
		/// @since  1.2 </returns>
		public virtual BigInteger UnscaledValue()
		{
			return this.Inflated();
		}

		// Rounding Modes

		/// <summary>
		/// Rounding mode to round away from zero.  Always increments the
		/// digit prior to a nonzero discarded fraction.  Note that this rounding
		/// mode never decreases the magnitude of the calculated value.
		/// </summary>
		public const int ROUND_UP = 0;

		/// <summary>
		/// Rounding mode to round towards zero.  Never increments the digit
		/// prior to a discarded fraction (i.e., truncates).  Note that this
		/// rounding mode never increases the magnitude of the calculated value.
		/// </summary>
		public const int ROUND_DOWN = 1;

		/// <summary>
		/// Rounding mode to round towards positive infinity.  If the
		/// {@code BigDecimal} is positive, behaves as for
		/// {@code ROUND_UP}; if negative, behaves as for
		/// {@code ROUND_DOWN}.  Note that this rounding mode never
		/// decreases the calculated value.
		/// </summary>
		public const int ROUND_CEILING = 2;

		/// <summary>
		/// Rounding mode to round towards negative infinity.  If the
		/// {@code BigDecimal} is positive, behave as for
		/// {@code ROUND_DOWN}; if negative, behave as for
		/// {@code ROUND_UP}.  Note that this rounding mode never
		/// increases the calculated value.
		/// </summary>
		public const int ROUND_FLOOR = 3;

		/// <summary>
		/// Rounding mode to round towards {@literal "nearest neighbor"}
		/// unless both neighbors are equidistant, in which case round up.
		/// Behaves as for {@code ROUND_UP} if the discarded fraction is
		/// &ge; 0.5; otherwise, behaves as for {@code ROUND_DOWN}.  Note
		/// that this is the rounding mode that most of us were taught in
		/// grade school.
		/// </summary>
		public const int ROUND_HALF_UP = 4;

		/// <summary>
		/// Rounding mode to round towards {@literal "nearest neighbor"}
		/// unless both neighbors are equidistant, in which case round
		/// down.  Behaves as for {@code ROUND_UP} if the discarded
		/// fraction is {@literal >} 0.5; otherwise, behaves as for
		/// {@code ROUND_DOWN}.
		/// </summary>
		public const int ROUND_HALF_DOWN = 5;

		/// <summary>
		/// Rounding mode to round towards the {@literal "nearest neighbor"}
		/// unless both neighbors are equidistant, in which case, round
		/// towards the even neighbor.  Behaves as for
		/// {@code ROUND_HALF_UP} if the digit to the left of the
		/// discarded fraction is odd; behaves as for
		/// {@code ROUND_HALF_DOWN} if it's even.  Note that this is the
		/// rounding mode that minimizes cumulative error when applied
		/// repeatedly over a sequence of calculations.
		/// </summary>
		public const int ROUND_HALF_EVEN = 6;

		/// <summary>
		/// Rounding mode to assert that the requested operation has an exact
		/// result, hence no rounding is necessary.  If this rounding mode is
		/// specified on an operation that yields an inexact result, an
		/// {@code ArithmeticException} is thrown.
		/// </summary>
		public const int ROUND_UNNECESSARY = 7;


		// Scaling/Rounding Operations

		/// <summary>
		/// Returns a {@code BigDecimal} rounded according to the
		/// {@code MathContext} settings.  If the precision setting is 0 then
		/// no rounding takes place.
		/// 
		/// <para>The effect of this method is identical to that of the
		/// <seealso cref="#plus(MathContext)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mc"> the context to use. </param>
		/// <returns> a {@code BigDecimal} rounded according to the
		///         {@code MathContext} settings. </returns>
		/// <exception cref="ArithmeticException"> if the rounding mode is
		///         {@code UNNECESSARY} and the
		///         {@code BigDecimal}  operation would require rounding. </exception>
		/// <seealso cref=    #plus(MathContext)
		/// @since  1.5 </seealso>
		public virtual BigDecimal Round(MathContext mc)
		{
			return Plus(mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose scale is the specified
		/// value, and whose unscaled value is determined by multiplying or
		/// dividing this {@code BigDecimal}'s unscaled value by the
		/// appropriate power of ten to maintain its overall value.  If the
		/// scale is reduced by the operation, the unscaled value must be
		/// divided (rather than multiplied), and the value may be changed;
		/// in this case, the specified rounding mode is applied to the
		/// division.
		/// 
		/// <para>Note that since BigDecimal objects are immutable, calls of
		/// this method do <i>not</i> result in the original object being
		/// modified, contrary to the usual convention of having methods
		/// named <tt>set<i>X</i></tt> mutate field <i>{@code X}</i>.
		/// Instead, {@code setScale} returns an object with the proper
		/// scale; the returned object may or may not be newly allocated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
		/// <param name="roundingMode"> The rounding mode to apply. </param>
		/// <returns> a {@code BigDecimal} whose scale is the specified value,
		///         and whose unscaled value is determined by multiplying or
		///         dividing this {@code BigDecimal}'s unscaled value by the
		///         appropriate power of ten to maintain its overall value. </returns>
		/// <exception cref="ArithmeticException"> if {@code roundingMode==UNNECESSARY}
		///         and the specified scaling operation would require
		///         rounding. </exception>
		/// <seealso cref=    RoundingMode
		/// @since  1.5 </seealso>
		public virtual BigDecimal SetScale(int newScale, RoundingMode roundingMode)
		{
			return SetScale(newScale, roundingMode.oldMode);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose scale is the specified
		/// value, and whose unscaled value is determined by multiplying or
		/// dividing this {@code BigDecimal}'s unscaled value by the
		/// appropriate power of ten to maintain its overall value.  If the
		/// scale is reduced by the operation, the unscaled value must be
		/// divided (rather than multiplied), and the value may be changed;
		/// in this case, the specified rounding mode is applied to the
		/// division.
		/// 
		/// <para>Note that since BigDecimal objects are immutable, calls of
		/// this method do <i>not</i> result in the original object being
		/// modified, contrary to the usual convention of having methods
		/// named <tt>set<i>X</i></tt> mutate field <i>{@code X}</i>.
		/// Instead, {@code setScale} returns an object with the proper
		/// scale; the returned object may or may not be newly allocated.
		/// 
		/// </para>
		/// <para>The new <seealso cref="#setScale(int, RoundingMode)"/> method should
		/// be used in preference to this legacy method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
		/// <param name="roundingMode"> The rounding mode to apply. </param>
		/// <returns> a {@code BigDecimal} whose scale is the specified value,
		///         and whose unscaled value is determined by multiplying or
		///         dividing this {@code BigDecimal}'s unscaled value by the
		///         appropriate power of ten to maintain its overall value. </returns>
		/// <exception cref="ArithmeticException"> if {@code roundingMode==ROUND_UNNECESSARY}
		///         and the specified scaling operation would require
		///         rounding. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
		///         represent a valid rounding mode. </exception>
		/// <seealso cref=    #ROUND_UP </seealso>
		/// <seealso cref=    #ROUND_DOWN </seealso>
		/// <seealso cref=    #ROUND_CEILING </seealso>
		/// <seealso cref=    #ROUND_FLOOR </seealso>
		/// <seealso cref=    #ROUND_HALF_UP </seealso>
		/// <seealso cref=    #ROUND_HALF_DOWN </seealso>
		/// <seealso cref=    #ROUND_HALF_EVEN </seealso>
		/// <seealso cref=    #ROUND_UNNECESSARY </seealso>
		public virtual BigDecimal SetScale(int newScale, int roundingMode)
		{
			if (roundingMode < ROUND_UP || roundingMode > ROUND_UNNECESSARY)
			{
				throw new IllegalArgumentException("Invalid rounding mode");
			}

			int oldScale = this.Scale_Renamed;
			if (newScale == oldScale) // easy case
			{
				return this;
			}
			if (this.Signum() == 0) // zero can have any scale
			{
				return ZeroValueOf(newScale);
			}
			if (this.IntCompact != INFLATED)
			{
				long rs = this.IntCompact;
				if (newScale > oldScale)
				{
					int raise = CheckScale((long) newScale - oldScale);
					if ((rs = LongMultiplyPowerTen(rs, raise)) != INFLATED)
					{
						return ValueOf(rs,newScale);
					}
					BigInteger rb = BigMultiplyPowerTen(raise);
					return new BigDecimal(rb, INFLATED, newScale, (Precision_Renamed > 0) ? Precision_Renamed + raise : 0);
				}
				else
				{
					// newScale < oldScale -- drop some digits
					// Can't predict the precision due to the effect of rounding.
					int drop = CheckScale((long) oldScale - newScale);
					if (drop < LONG_TEN_POWERS_TABLE.Length)
					{
						return DivideAndRound(rs, LONG_TEN_POWERS_TABLE[drop], newScale, roundingMode, newScale);
					}
					else
					{
						return DivideAndRound(this.Inflated(), BigTenToThe(drop), newScale, roundingMode, newScale);
					}
				}
			}
			else
			{
				if (newScale > oldScale)
				{
					int raise = CheckScale((long) newScale - oldScale);
					BigInteger rb = BigMultiplyPowerTen(this.IntVal,raise);
					return new BigDecimal(rb, INFLATED, newScale, (Precision_Renamed > 0) ? Precision_Renamed + raise : 0);
				}
				else
				{
					// newScale < oldScale -- drop some digits
					// Can't predict the precision due to the effect of rounding.
					int drop = CheckScale((long) oldScale - newScale);
					if (drop < LONG_TEN_POWERS_TABLE.Length)
					{
						return DivideAndRound(this.IntVal, LONG_TEN_POWERS_TABLE[drop], newScale, roundingMode, newScale);
					}
					else
					{
						return DivideAndRound(this.IntVal, BigTenToThe(drop), newScale, roundingMode, newScale);
					}
				}
			}
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose scale is the specified
		/// value, and whose value is numerically equal to this
		/// {@code BigDecimal}'s.  Throws an {@code ArithmeticException}
		/// if this is not possible.
		/// 
		/// <para>This call is typically used to increase the scale, in which
		/// case it is guaranteed that there exists a {@code BigDecimal}
		/// of the specified scale and the correct value.  The call can
		/// also be used to reduce the scale if the caller knows that the
		/// {@code BigDecimal} has sufficiently many zeros at the end of
		/// its fractional part (i.e., factors of ten in its integer value)
		/// to allow for the rescaling without changing its value.
		/// 
		/// </para>
		/// <para>This method returns the same result as the two-argument
		/// versions of {@code setScale}, but saves the caller the trouble
		/// of specifying a rounding mode in cases where it is irrelevant.
		/// 
		/// </para>
		/// <para>Note that since {@code BigDecimal} objects are immutable,
		/// calls of this method do <i>not</i> result in the original
		/// object being modified, contrary to the usual convention of
		/// having methods named <tt>set<i>X</i></tt> mutate field
		/// <i>{@code X}</i>.  Instead, {@code setScale} returns an
		/// object with the proper scale; the returned object may or may
		/// not be newly allocated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
		/// <returns> a {@code BigDecimal} whose scale is the specified value, and
		///         whose unscaled value is determined by multiplying or dividing
		///         this {@code BigDecimal}'s unscaled value by the appropriate
		///         power of ten to maintain its overall value. </returns>
		/// <exception cref="ArithmeticException"> if the specified scaling operation would
		///         require rounding. </exception>
		/// <seealso cref=    #setScale(int, int) </seealso>
		/// <seealso cref=    #setScale(int, RoundingMode) </seealso>
		public virtual BigDecimal SetScale(int newScale)
		{
			return SetScale(newScale, ROUND_UNNECESSARY);
		}

		// Decimal Point Motion Operations

		/// <summary>
		/// Returns a {@code BigDecimal} which is equivalent to this one
		/// with the decimal point moved {@code n} places to the left.  If
		/// {@code n} is non-negative, the call merely adds {@code n} to
		/// the scale.  If {@code n} is negative, the call is equivalent
		/// to {@code movePointRight(-n)}.  The {@code BigDecimal}
		/// returned by this call has value <tt>(this &times;
		/// 10<sup>-n</sup>)</tt> and scale {@code max(this.scale()+n,
		/// 0)}.
		/// </summary>
		/// <param name="n"> number of places to move the decimal point to the left. </param>
		/// <returns> a {@code BigDecimal} which is equivalent to this one with the
		///         decimal point moved {@code n} places to the left. </returns>
		/// <exception cref="ArithmeticException"> if scale overflows. </exception>
		public virtual BigDecimal MovePointLeft(int n)
		{
			// Cannot use movePointRight(-n) in case of n==Integer.MIN_VALUE
			int newScale = CheckScale((long)Scale_Renamed + n);
			BigDecimal num = new BigDecimal(IntVal, IntCompact, newScale, 0);
			return num.Scale_Renamed < 0 ? num.SetScale(0, ROUND_UNNECESSARY) : num;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} which is equivalent to this one
		/// with the decimal point moved {@code n} places to the right.
		/// If {@code n} is non-negative, the call merely subtracts
		/// {@code n} from the scale.  If {@code n} is negative, the call
		/// is equivalent to {@code movePointLeft(-n)}.  The
		/// {@code BigDecimal} returned by this call has value <tt>(this
		/// &times; 10<sup>n</sup>)</tt> and scale {@code max(this.scale()-n,
		/// 0)}.
		/// </summary>
		/// <param name="n"> number of places to move the decimal point to the right. </param>
		/// <returns> a {@code BigDecimal} which is equivalent to this one
		///         with the decimal point moved {@code n} places to the right. </returns>
		/// <exception cref="ArithmeticException"> if scale overflows. </exception>
		public virtual BigDecimal MovePointRight(int n)
		{
			// Cannot use movePointLeft(-n) in case of n==Integer.MIN_VALUE
			int newScale = CheckScale((long)Scale_Renamed - n);
			BigDecimal num = new BigDecimal(IntVal, IntCompact, newScale, 0);
			return num.Scale_Renamed < 0 ? num.SetScale(0, ROUND_UNNECESSARY) : num;
		}

		/// <summary>
		/// Returns a BigDecimal whose numerical value is equal to
		/// ({@code this} * 10<sup>n</sup>).  The scale of
		/// the result is {@code (this.scale() - n)}.
		/// </summary>
		/// <param name="n"> the exponent power of ten to scale by </param>
		/// <returns> a BigDecimal whose numerical value is equal to
		/// ({@code this} * 10<sup>n</sup>) </returns>
		/// <exception cref="ArithmeticException"> if the scale would be
		///         outside the range of a 32-bit integer.
		/// 
		/// @since 1.5 </exception>
		public virtual BigDecimal ScaleByPowerOfTen(int n)
		{
			return new BigDecimal(IntVal, IntCompact, CheckScale((long)Scale_Renamed - n), Precision_Renamed);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} which is numerically equal to
		/// this one but with any trailing zeros removed from the
		/// representation.  For example, stripping the trailing zeros from
		/// the {@code BigDecimal} value {@code 600.0}, which has
		/// [{@code BigInteger}, {@code scale}] components equals to
		/// [6000, 1], yields {@code 6E2} with [{@code BigInteger},
		/// {@code scale}] components equals to [6, -2].  If
		/// this BigDecimal is numerically equal to zero, then
		/// {@code BigDecimal.ZERO} is returned.
		/// </summary>
		/// <returns> a numerically equal {@code BigDecimal} with any
		/// trailing zeros removed.
		/// @since 1.5 </returns>
		public virtual BigDecimal StripTrailingZeros()
		{
			if (IntCompact == 0 || (IntVal != null && IntVal.Signum() == 0))
			{
				return decimal.Zero;
			}
			else if (IntCompact != INFLATED)
			{
				return CreateAndStripZerosToMatchScale(IntCompact, Scale_Renamed, Long.MinValue);
			}
			else
			{
				return CreateAndStripZerosToMatchScale(IntVal, Scale_Renamed, Long.MinValue);
			}
		}

		// Comparison Operations

		/// <summary>
		/// Compares this {@code BigDecimal} with the specified
		/// {@code BigDecimal}.  Two {@code BigDecimal} objects that are
		/// equal in value but have a different scale (like 2.0 and 2.00)
		/// are considered equal by this method.  This method is provided
		/// in preference to individual methods for each of the six boolean
		/// comparison operators ({@literal <}, ==,
		/// {@literal >}, {@literal >=}, !=, {@literal <=}).  The
		/// suggested idiom for performing these comparisons is:
		/// {@code (x.compareTo(y)} &lt;<i>op</i>&gt; {@code 0)}, where
		/// &lt;<i>op</i>&gt; is one of the six comparison operators.
		/// </summary>
		/// <param name="val"> {@code BigDecimal} to which this {@code BigDecimal} is
		///         to be compared. </param>
		/// <returns> -1, 0, or 1 as this {@code BigDecimal} is numerically
		///          less than, equal to, or greater than {@code val}. </returns>
		public virtual int CompareTo(BigDecimal val)
		{
			// Quick path for equal scale and non-inflated case.
			if (Scale_Renamed == val.Scale_Renamed)
			{
				long xs = IntCompact;
				long ys = val.IntCompact;
				if (xs != INFLATED && ys != INFLATED)
				{
					return xs != ys ? ((xs > ys) ? 1 : -1) : 0;
				}
			}
			int xsign = this.Signum();
			int ysign = val.Signum();
			if (xsign != ysign)
			{
				return (xsign > ysign) ? 1 : -1;
			}
			if (xsign == 0)
			{
				return 0;
			}
			int cmp = CompareMagnitude(val);
			return (xsign > 0) ? cmp : -cmp;
		}

		/// <summary>
		/// Version of compareTo that ignores sign.
		/// </summary>
		private int CompareMagnitude(BigDecimal val)
		{
			// Match scales, avoid unnecessary inflation
			long ys = val.IntCompact;
			long xs = this.IntCompact;
			if (xs == 0)
			{
				return (ys == 0) ? 0 : -1;
			}
			if (ys == 0)
			{
				return 1;
			}

			long sdiff = (long)this.Scale_Renamed - val.Scale_Renamed;
			if (sdiff != 0)
			{
				// Avoid matching scales if the (adjusted) exponents differ
				long xae = (long)this.Precision() - this.Scale_Renamed; // [-1]
				long yae = (long)val.Precision() - val.Scale_Renamed; // [-1]
				if (xae < yae)
				{
					return -1;
				}
				if (xae > yae)
				{
					return 1;
				}
				BigInteger rb = null;
				if (sdiff < 0)
				{
					// The cases sdiff <= Integer.MIN_VALUE intentionally fall through.
					if (sdiff > Integer.MinValue && (xs == INFLATED || (xs = LongMultiplyPowerTen(xs, (int)-sdiff)) == INFLATED) && ys == INFLATED)
					{
						rb = BigMultiplyPowerTen((int)-sdiff);
						return rb.CompareMagnitude(val.IntVal);
					}
				} // sdiff > 0
				else
				{
					// The cases sdiff > Integer.MAX_VALUE intentionally fall through.
					if (sdiff <= Integer.MaxValue && (ys == INFLATED || (ys = LongMultiplyPowerTen(ys, (int)sdiff)) == INFLATED) && xs == INFLATED)
					{
						rb = val.BigMultiplyPowerTen((int)sdiff);
						return this.IntVal.CompareMagnitude(rb);
					}
				}
			}
			if (xs != INFLATED)
			{
				return (ys != INFLATED) ? LongCompareMagnitude(xs, ys) : -1;
			}
			else if (ys != INFLATED)
			{
				return 1;
			}
			else
			{
				return this.IntVal.CompareMagnitude(val.IntVal);
			}
		}

		/// <summary>
		/// Compares this {@code BigDecimal} with the specified
		/// {@code Object} for equality.  Unlike {@link
		/// #compareTo(BigDecimal) compareTo}, this method considers two
		/// {@code BigDecimal} objects equal only if they are equal in
		/// value and scale (thus 2.0 is not equal to 2.00 when compared by
		/// this method).
		/// </summary>
		/// <param name="x"> {@code Object} to which this {@code BigDecimal} is
		///         to be compared. </param>
		/// <returns> {@code true} if and only if the specified {@code Object} is a
		///         {@code BigDecimal} whose value and scale are equal to this
		///         {@code BigDecimal}'s. </returns>
		/// <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
		/// <seealso cref=    #hashCode </seealso>
		public override bool Equals(Object x)
		{
			if (!(x is BigDecimal))
			{
				return false;
			}
			BigDecimal xDec = (BigDecimal) x;
			if (x == this)
			{
				return true;
			}
			if (Scale_Renamed != xDec.Scale_Renamed)
			{
				return false;
			}
			long s = this.IntCompact;
			long xs = xDec.IntCompact;
			if (s != INFLATED)
			{
				if (xs == INFLATED)
				{
					xs = CompactValFor(xDec.IntVal);
				}
				return xs == s;
			}
			else if (xs != INFLATED)
			{
				return xs == CompactValFor(this.IntVal);
			}

			return this.Inflated().Equals(xDec.Inflated());
		}

		/// <summary>
		/// Returns the minimum of this {@code BigDecimal} and
		/// {@code val}.
		/// </summary>
		/// <param name="val"> value with which the minimum is to be computed. </param>
		/// <returns> the {@code BigDecimal} whose value is the lesser of this
		///         {@code BigDecimal} and {@code val}.  If they are equal,
		///         as defined by the <seealso cref="#compareTo(BigDecimal) compareTo"/>
		///         method, {@code this} is returned. </returns>
		/// <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
		public virtual BigDecimal Min(BigDecimal val)
		{
			return (CompareTo(val) <= 0 ? this : val);
		}

		/// <summary>
		/// Returns the maximum of this {@code BigDecimal} and {@code val}.
		/// </summary>
		/// <param name="val"> value with which the maximum is to be computed. </param>
		/// <returns> the {@code BigDecimal} whose value is the greater of this
		///         {@code BigDecimal} and {@code val}.  If they are equal,
		///         as defined by the <seealso cref="#compareTo(BigDecimal) compareTo"/>
		///         method, {@code this} is returned. </returns>
		/// <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
		public virtual BigDecimal Max(BigDecimal val)
		{
			return (CompareTo(val) >= 0 ? this : val);
		}

		// Hash Function

		/// <summary>
		/// Returns the hash code for this {@code BigDecimal}.  Note that
		/// two {@code BigDecimal} objects that are numerically equal but
		/// differ in scale (like 2.0 and 2.00) will generally <i>not</i>
		/// have the same hash code.
		/// </summary>
		/// <returns> hash code for this {@code BigDecimal}. </returns>
		/// <seealso cref= #equals(Object) </seealso>
		public override int HashCode()
		{
			if (IntCompact != INFLATED)
			{
				long val2 = (IntCompact < 0)? - IntCompact : IntCompact;
				int temp = (int)(((int)((long)((ulong)val2 >> 32))) * 31 + (val2 & LONG_MASK));
				return 31 * ((IntCompact < 0) ? - temp:temp) + Scale_Renamed;
			}
			else
			{
				return 31 * IntVal.HashCode() + Scale_Renamed;
			}
		}

		// Format Converters

		/// <summary>
		/// Returns the string representation of this {@code BigDecimal},
		/// using scientific notation if an exponent is needed.
		/// 
		/// <para>A standard canonical string form of the {@code BigDecimal}
		/// is created as though by the following steps: first, the
		/// absolute value of the unscaled value of the {@code BigDecimal}
		/// is converted to a string in base ten using the characters
		/// {@code '0'} through {@code '9'} with no leading zeros (except
		/// if its value is zero, in which case a single {@code '0'}
		/// character is used).
		/// 
		/// </para>
		/// <para>Next, an <i>adjusted exponent</i> is calculated; this is the
		/// negated scale, plus the number of characters in the converted
		/// unscaled value, less one.  That is,
		/// {@code -scale+(ulength-1)}, where {@code ulength} is the
		/// length of the absolute value of the unscaled value in decimal
		/// digits (its <i>precision</i>).
		/// 
		/// </para>
		/// <para>If the scale is greater than or equal to zero and the
		/// adjusted exponent is greater than or equal to {@code -6}, the
		/// number will be converted to a character form without using
		/// exponential notation.  In this case, if the scale is zero then
		/// no decimal point is added and if the scale is positive a
		/// decimal point will be inserted with the scale specifying the
		/// number of characters to the right of the decimal point.
		/// {@code '0'} characters are added to the left of the converted
		/// unscaled value as necessary.  If no character precedes the
		/// decimal point after this insertion then a conventional
		/// {@code '0'} character is prefixed.
		/// 
		/// </para>
		/// <para>Otherwise (that is, if the scale is negative, or the
		/// adjusted exponent is less than {@code -6}), the number will be
		/// converted to a character form using exponential notation.  In
		/// this case, if the converted {@code BigInteger} has more than
		/// one digit a decimal point is inserted after the first digit.
		/// An exponent in character form is then suffixed to the converted
		/// unscaled value (perhaps with inserted decimal point); this
		/// comprises the letter {@code 'E'} followed immediately by the
		/// adjusted exponent converted to a character form.  The latter is
		/// in base ten, using the characters {@code '0'} through
		/// {@code '9'} with no leading zeros, and is always prefixed by a
		/// sign character {@code '-'} (<tt>'&#92;u002D'</tt>) if the
		/// adjusted exponent is negative, {@code '+'}
		/// (<tt>'&#92;u002B'</tt>) otherwise).
		/// 
		/// </para>
		/// <para>Finally, the entire string is prefixed by a minus sign
		/// character {@code '-'} (<tt>'&#92;u002D'</tt>) if the unscaled
		/// value is less than zero.  No sign character is prefixed if the
		/// unscaled value is zero or positive.
		/// 
		/// </para>
		/// <para><b>Examples:</b>
		/// </para>
		/// <para>For each representation [<i>unscaled value</i>, <i>scale</i>]
		/// on the left, the resulting string is shown on the right.
		/// <pre>
		/// [123,0]      "123"
		/// [-123,0]     "-123"
		/// [123,-1]     "1.23E+3"
		/// [123,-3]     "1.23E+5"
		/// [123,1]      "12.3"
		/// [123,5]      "0.00123"
		/// [123,10]     "1.23E-8"
		/// [-123,12]    "-1.23E-10"
		/// </pre>
		/// 
		/// <b>Notes:</b>
		/// <ol>
		/// 
		/// <li>There is a one-to-one mapping between the distinguishable
		/// {@code BigDecimal} values and the result of this conversion.
		/// That is, every distinguishable {@code BigDecimal} value
		/// (unscaled value and scale) has a unique string representation
		/// as a result of using {@code toString}.  If that string
		/// representation is converted back to a {@code BigDecimal} using
		/// the <seealso cref="#BigDecimal(String)"/> constructor, then the original
		/// value will be recovered.
		/// 
		/// <li>The string produced for a given number is always the same;
		/// it is not affected by locale.  This means that it can be used
		/// as a canonical string representation for exchanging decimal
		/// data, or as a key for a Hashtable, etc.  Locale-sensitive
		/// number formatting and parsing is handled by the {@link
		/// java.text.NumberFormat} class and its subclasses.
		/// 
		/// <li>The <seealso cref="#toEngineeringString"/> method may be used for
		/// presenting numbers with exponents in engineering notation, and the
		/// <seealso cref="#setScale(int,RoundingMode) setScale"/> method may be used for
		/// rounding a {@code BigDecimal} so it has a known number of digits after
		/// the decimal point.
		/// 
		/// <li>The digit-to-character mapping provided by
		/// {@code Character.forDigit} is used.
		/// 
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <returns> string representation of this {@code BigDecimal}. </returns>
		/// <seealso cref=    Character#forDigit </seealso>
		/// <seealso cref=    #BigDecimal(java.lang.String) </seealso>
		public override String ToString()
		{
			String sc = StringCache;
			if (sc == null)
			{
				StringCache = sc = LayoutChars(true);
			}
			return sc;
		}

		/// <summary>
		/// Returns a string representation of this {@code BigDecimal},
		/// using engineering notation if an exponent is needed.
		/// 
		/// <para>Returns a string that represents the {@code BigDecimal} as
		/// described in the <seealso cref="#toString()"/> method, except that if
		/// exponential notation is used, the power of ten is adjusted to
		/// be a multiple of three (engineering notation) such that the
		/// integer part of nonzero values will be in the range 1 through
		/// 999.  If exponential notation is used for zero values, a
		/// decimal point and one or two fractional zero digits are used so
		/// that the scale of the zero value is preserved.  Note that
		/// unlike the output of <seealso cref="#toString()"/>, the output of this
		/// method is <em>not</em> guaranteed to recover the same [integer,
		/// scale] pair of this {@code BigDecimal} if the output string is
		/// converting back to a {@code BigDecimal} using the {@linkplain
		/// #BigDecimal(String) string constructor}.  The result of this method meets
		/// the weaker constraint of always producing a numerically equal
		/// result from applying the string constructor to the method's output.
		/// 
		/// </para>
		/// </summary>
		/// <returns> string representation of this {@code BigDecimal}, using
		///         engineering notation if an exponent is needed.
		/// @since  1.5 </returns>
		public virtual String ToEngineeringString()
		{
			return LayoutChars(false);
		}

		/// <summary>
		/// Returns a string representation of this {@code BigDecimal}
		/// without an exponent field.  For values with a positive scale,
		/// the number of digits to the right of the decimal point is used
		/// to indicate scale.  For values with a zero or negative scale,
		/// the resulting string is generated as if the value were
		/// converted to a numerically equal value with zero scale and as
		/// if all the trailing zeros of the zero scale value were present
		/// in the result.
		/// 
		/// The entire string is prefixed by a minus sign character '-'
		/// (<tt>'&#92;u002D'</tt>) if the unscaled value is less than
		/// zero. No sign character is prefixed if the unscaled value is
		/// zero or positive.
		/// 
		/// Note that if the result of this method is passed to the
		/// <seealso cref="#BigDecimal(String) string constructor"/>, only the
		/// numerical value of this {@code BigDecimal} will necessarily be
		/// recovered; the representation of the new {@code BigDecimal}
		/// may have a different scale.  In particular, if this
		/// {@code BigDecimal} has a negative scale, the string resulting
		/// from this method will have a scale of zero when processed by
		/// the string constructor.
		/// 
		/// (This method behaves analogously to the {@code toString}
		/// method in 1.4 and earlier releases.)
		/// </summary>
		/// <returns> a string representation of this {@code BigDecimal}
		/// without an exponent field.
		/// @since 1.5 </returns>
		/// <seealso cref= #toString() </seealso>
		/// <seealso cref= #toEngineeringString() </seealso>
		public virtual String ToPlainString()
		{
			if (Scale_Renamed == 0)
			{
				if (IntCompact != INFLATED)
				{
					return Convert.ToString(IntCompact);
				}
				else
				{
					return IntVal.ToString();
				}
			}
			if (this.Scale_Renamed < 0) // No decimal point
			{
				if (Signum() == 0)
				{
					return "0";
				}
				int tailingZeros = CheckScaleNonZero((-(long)Scale_Renamed));
				StringBuilder buf;
				if (IntCompact != INFLATED)
				{
					buf = new StringBuilder(20 + tailingZeros);
					buf.Append(IntCompact);
				}
				else
				{
					String str = IntVal.ToString();
					buf = new StringBuilder(str.Length() + tailingZeros);
					buf.Append(str);
				}
				for (int i = 0; i < tailingZeros; i++)
				{
					buf.Append('0');
				}
				return buf.ToString();
			}
			String str;
			if (IntCompact != INFLATED)
			{
				str = Convert.ToString(System.Math.Abs(IntCompact));
			}
			else
			{
				str = IntVal.Abs().ToString();
			}
			return GetValueString(Signum(), str, Scale_Renamed);
		}

		/* Returns a digit.digit string */
		private String GetValueString(int signum, String intString, int scale)
		{
			/* Insert decimal point */
			StringBuilder buf;
			int insertionPoint = intString.Length() - scale;
			if (insertionPoint == 0) // Point goes right before intVal
			{
				return (signum < 0 ? "-0." : "0.") + intString;
			} // Point goes inside intVal
			else if (insertionPoint > 0)
			{
				buf = new StringBuilder(intString);
				buf.Insert(insertionPoint, '.');
				if (signum < 0)
				{
					buf.Insert(0, '-');
				}
			} // We must insert zeros between point and intVal
			else
			{
				buf = new StringBuilder(3 - insertionPoint + intString.Length());
				buf.Append(signum < 0 ? "-0." : "0.");
				for (int i = 0; i < -insertionPoint; i++)
				{
					buf.Append('0');
				}
				buf.Append(intString);
			}
			return buf.ToString();
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code BigInteger}.
		/// This conversion is analogous to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code long} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// any fractional part of this
		/// {@code BigDecimal} will be discarded.  Note that this
		/// conversion can lose information about the precision of the
		/// {@code BigDecimal} value.
		/// <para>
		/// To have an exception thrown if the conversion is inexact (in
		/// other words if a nonzero fractional part is discarded), use the
		/// <seealso cref="#toBigIntegerExact()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code BigInteger}. </returns>
		public virtual BigInteger ToBigInteger()
		{
			// force to an integer, quietly
			return this.SetScale(0, ROUND_DOWN).Inflated();
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code BigInteger},
		/// checking for lost information.  An exception is thrown if this
		/// {@code BigDecimal} has a nonzero fractional part.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code BigInteger}. </returns>
		/// <exception cref="ArithmeticException"> if {@code this} has a nonzero
		///         fractional part.
		/// @since  1.5 </exception>
		public virtual BigInteger ToBigIntegerExact()
		{
			// round to an integer, with Exception if decimal part non-0
			return this.SetScale(0, ROUND_UNNECESSARY).Inflated();
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code long}.
		/// This conversion is analogous to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code short} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// any fractional part of this
		/// {@code BigDecimal} will be discarded, and if the resulting
		/// "{@code BigInteger}" is too big to fit in a
		/// {@code long}, only the low-order 64 bits are returned.
		/// Note that this conversion can lose information about the
		/// overall magnitude and precision of this {@code BigDecimal} value as well
		/// as return a result with the opposite sign.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code long}. </returns>
		public override long LongValue()
		{
			return (IntCompact != INFLATED && Scale_Renamed == 0) ? IntCompact: ToBigInteger().LongValue();
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code long}, checking
		/// for lost information.  If this {@code BigDecimal} has a
		/// nonzero fractional part or is out of the possible range for a
		/// {@code long} result then an {@code ArithmeticException} is
		/// thrown.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code long}. </returns>
		/// <exception cref="ArithmeticException"> if {@code this} has a nonzero
		///         fractional part, or will not fit in a {@code long}.
		/// @since  1.5 </exception>
		public virtual long LongValueExact()
		{
			if (IntCompact != INFLATED && Scale_Renamed == 0)
			{
				return IntCompact;
			}
			// If more than 19 digits in integer part it cannot possibly fit
			if ((Precision() - Scale_Renamed) > 19) // [OK for negative scale too]
			{
				throw new java.lang.ArithmeticException("Overflow");
			}
			// Fastpath zero and < 1.0 numbers (the latter can be very slow
			// to round if very small)
			if (this.Signum() == 0)
			{
				return 0;
			}
			if ((this.Precision() - this.Scale_Renamed) <= 0)
			{
				throw new ArithmeticException("Rounding necessary");
			}
			// round to an integer, with Exception if decimal part non-0
			BigDecimal num = this.SetScale(0, ROUND_UNNECESSARY);
			if (num.Precision() >= 19) // need to check carefully
			{
				LongOverflow.Check(num);
			}
			return num.Inflated().LongValue();
		}

		private class LongOverflow
		{
			/// <summary>
			/// BigInteger equal to Long.MIN_VALUE. </summary>
			internal static readonly BigInteger LONGMIN = BigInteger.ValueOf(Long.MinValue);

			/// <summary>
			/// BigInteger equal to Long.MAX_VALUE. </summary>
			internal static readonly BigInteger LONGMAX = BigInteger.ValueOf(Long.MaxValue);

			public static void Check(BigDecimal num)
			{
				BigInteger intVal = num.Inflated();
				if (intVal.CompareTo(LONGMIN) < 0 || intVal.CompareTo(LONGMAX) > 0)
				{
					throw new java.lang.ArithmeticException("Overflow");
				}
			}
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to an {@code int}.
		/// This conversion is analogous to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code short} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// any fractional part of this
		/// {@code BigDecimal} will be discarded, and if the resulting
		/// "{@code BigInteger}" is too big to fit in an
		/// {@code int}, only the low-order 32 bits are returned.
		/// Note that this conversion can lose information about the
		/// overall magnitude and precision of this {@code BigDecimal}
		/// value as well as return a result with the opposite sign.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to an {@code int}. </returns>
		public override int IntValue()
		{
			return (IntCompact != INFLATED && Scale_Renamed == 0) ? (int)IntCompact : ToBigInteger().IntValue();
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to an {@code int}, checking
		/// for lost information.  If this {@code BigDecimal} has a
		/// nonzero fractional part or is out of the possible range for an
		/// {@code int} result then an {@code ArithmeticException} is
		/// thrown.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to an {@code int}. </returns>
		/// <exception cref="ArithmeticException"> if {@code this} has a nonzero
		///         fractional part, or will not fit in an {@code int}.
		/// @since  1.5 </exception>
		public virtual int IntValueExact()
		{
		   long num;
		   num = this.LongValueExact(); // will check decimal part
		   if ((int)num != num)
		   {
			   throw new java.lang.ArithmeticException("Overflow");
		   }
		   return (int)num;
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code short}, checking
		/// for lost information.  If this {@code BigDecimal} has a
		/// nonzero fractional part or is out of the possible range for a
		/// {@code short} result then an {@code ArithmeticException} is
		/// thrown.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code short}. </returns>
		/// <exception cref="ArithmeticException"> if {@code this} has a nonzero
		///         fractional part, or will not fit in a {@code short}.
		/// @since  1.5 </exception>
		public virtual short ShortValueExact()
		{
		   long num;
		   num = this.LongValueExact(); // will check decimal part
		   if ((short)num != num)
		   {
			   throw new java.lang.ArithmeticException("Overflow");
		   }
		   return (short)num;
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code byte}, checking
		/// for lost information.  If this {@code BigDecimal} has a
		/// nonzero fractional part or is out of the possible range for a
		/// {@code byte} result then an {@code ArithmeticException} is
		/// thrown.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code byte}. </returns>
		/// <exception cref="ArithmeticException"> if {@code this} has a nonzero
		///         fractional part, or will not fit in a {@code byte}.
		/// @since  1.5 </exception>
		public virtual sbyte ByteValueExact()
		{
		   long num;
		   num = this.LongValueExact(); // will check decimal part
		   if ((sbyte)num != num)
		   {
			   throw new java.lang.ArithmeticException("Overflow");
		   }
		   return (sbyte)num;
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code float}.
		/// This conversion is similar to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code float} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this {@code BigDecimal} has too great a
		/// magnitude to represent as a {@code float}, it will be
		/// converted to <seealso cref="Float#NEGATIVE_INFINITY"/> or {@link
		/// Float#POSITIVE_INFINITY} as appropriate.  Note that even when
		/// the return value is finite, this conversion can lose
		/// information about the precision of the {@code BigDecimal}
		/// value.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code float}. </returns>
		public override float FloatValue()
		{
			if (IntCompact != INFLATED)
			{
				if (Scale_Renamed == 0)
				{
					return (float)IntCompact;
				}
				else
				{
					/*
					 * If both intCompact and the scale can be exactly
					 * represented as float values, perform a single float
					 * multiply or divide to compute the (properly
					 * rounded) result.
					 */
					if (System.Math.Abs(IntCompact) < 1L << 22)
					{
						// Don't have too guard against
						// Math.abs(MIN_VALUE) because of outer check
						// against INFLATED.
						if (Scale_Renamed > 0 && Scale_Renamed < Float10pow.Length)
						{
							return (float)IntCompact / Float10pow[Scale_Renamed];
						}
						else if (Scale_Renamed < 0 && Scale_Renamed > -Float10pow.Length)
						{
							return (float)IntCompact * Float10pow[-Scale_Renamed];
						}
					}
				}
			}
			// Somewhat inefficient, but guaranteed to work.
			return Convert.ToSingle(this.ToString());
		}

		/// <summary>
		/// Converts this {@code BigDecimal} to a {@code double}.
		/// This conversion is similar to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code float} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this {@code BigDecimal} has too great a
		/// magnitude represent as a {@code double}, it will be
		/// converted to <seealso cref="Double#NEGATIVE_INFINITY"/> or {@link
		/// Double#POSITIVE_INFINITY} as appropriate.  Note that even when
		/// the return value is finite, this conversion can lose
		/// information about the precision of the {@code BigDecimal}
		/// value.
		/// </summary>
		/// <returns> this {@code BigDecimal} converted to a {@code double}. </returns>
		public override double DoubleValue()
		{
			if (IntCompact != INFLATED)
			{
				if (Scale_Renamed == 0)
				{
					return (double)IntCompact;
				}
				else
				{
					/*
					 * If both intCompact and the scale can be exactly
					 * represented as double values, perform a single
					 * double multiply or divide to compute the (properly
					 * rounded) result.
					 */
					if (System.Math.Abs(IntCompact) < 1L << 52)
					{
						// Don't have too guard against
						// Math.abs(MIN_VALUE) because of outer check
						// against INFLATED.
						if (Scale_Renamed > 0 && Scale_Renamed < Double10pow.Length)
						{
							return (double)IntCompact / Double10pow[Scale_Renamed];
						}
						else if (Scale_Renamed < 0 && Scale_Renamed > -Double10pow.Length)
						{
							return (double)IntCompact * Double10pow[-Scale_Renamed];
						}
					}
				}
			}
			// Somewhat inefficient, but guaranteed to work.
			return Convert.ToDouble(this.ToString());
		}

		/// <summary>
		/// Powers of 10 which can be represented exactly in {@code
		/// double}.
		/// </summary>
		private static readonly double[] Double10pow = new double[] {1.0e0, 1.0e1, 1.0e2, 1.0e3, 1.0e4, 1.0e5, 1.0e6, 1.0e7, 1.0e8, 1.0e9, 1.0e10, 1.0e11, 1.0e12, 1.0e13, 1.0e14, 1.0e15, 1.0e16, 1.0e17, 1.0e18, 1.0e19, 1.0e20, 1.0e21, 1.0e22};

		/// <summary>
		/// Powers of 10 which can be represented exactly in {@code
		/// float}.
		/// </summary>
		private static readonly float[] Float10pow = new float[] {1.0e0f, 1.0e1f, 1.0e2f, 1.0e3f, 1.0e4f, 1.0e5f, 1.0e6f, 1.0e7f, 1.0e8f, 1.0e9f, 1.0e10f};

		/// <summary>
		/// Returns the size of an ulp, a unit in the last place, of this
		/// {@code BigDecimal}.  An ulp of a nonzero {@code BigDecimal}
		/// value is the positive distance between this value and the
		/// {@code BigDecimal} value next larger in magnitude with the
		/// same number of digits.  An ulp of a zero value is numerically
		/// equal to 1 with the scale of {@code this}.  The result is
		/// stored with the same scale as {@code this} so the result
		/// for zero and nonzero values is equal to {@code [1,
		/// this.scale()]}.
		/// </summary>
		/// <returns> the size of an ulp of {@code this}
		/// @since 1.5 </returns>
		public virtual BigDecimal Ulp()
		{
			return BigDecimal.ValueOf(1, this.Scale(), 1);
		}

		// Private class to build a string representation for BigDecimal object.
		// "StringBuilderHelper" is constructed as a thread local variable so it is
		// thread safe. The StringBuilder field acts as a buffer to hold the temporary
		// representation of BigDecimal. The cmpCharArray holds all the characters for
		// the compact representation of BigDecimal (except for '-' sign' if it is
		// negative) if its intCompact field is not INFLATED. It is shared by all
		// calls to toString() and its variants in that particular thread.
		internal class StringBuilderHelper
		{
			internal readonly StringBuilder Sb; // Placeholder for BigDecimal string
			internal readonly char[] CmpCharArray; // character array to place the intCompact

			internal StringBuilderHelper()
			{
				Sb = new StringBuilder();
				// All non negative longs can be made to fit into 19 character array.
				CmpCharArray = new char[19];
			}

			// Accessors.
			internal virtual StringBuilder StringBuilder
			{
				get
				{
					Sb.Length = 0;
					return Sb;
				}
			}

			internal virtual char[] CompactCharArray
			{
				get
				{
					return CmpCharArray;
				}
			}

			/// <summary>
			/// Places characters representing the intCompact in {@code long} into
			/// cmpCharArray and returns the offset to the array where the
			/// representation starts.
			/// </summary>
			/// <param name="intCompact"> the number to put into the cmpCharArray. </param>
			/// <returns> offset to the array where the representation starts.
			/// Note: intCompact must be greater or equal to zero. </returns>
			internal virtual int PutIntCompact(long intCompact)
			{
				Debug.Assert(intCompact >= 0);

				long q;
				int r;
				// since we start from the least significant digit, charPos points to
				// the last character in cmpCharArray.
				int charPos = CmpCharArray.Length;

				// Get 2 digits/iteration using longs until quotient fits into an int
				while (intCompact > Integer.MaxValue)
				{
					q = intCompact / 100;
					r = (int)(intCompact - q * 100);
					intCompact = q;
					CmpCharArray[--charPos] = DIGIT_ONES[r];
					CmpCharArray[--charPos] = DIGIT_TENS[r];
				}

				// Get 2 digits/iteration using ints when i2 >= 100
				int q2;
				int i2 = (int)intCompact;
				while (i2 >= 100)
				{
					q2 = i2 / 100;
					r = i2 - q2 * 100;
					i2 = q2;
					CmpCharArray[--charPos] = DIGIT_ONES[r];
					CmpCharArray[--charPos] = DIGIT_TENS[r];
				}

				CmpCharArray[--charPos] = DIGIT_ONES[i2];
				if (i2 >= 10)
				{
					CmpCharArray[--charPos] = DIGIT_TENS[i2];
				}

				return charPos;
			}

			internal static readonly char[] DIGIT_TENS = new char[] {'0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9'};

			internal static readonly char[] DIGIT_ONES = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
		}

		/// <summary>
		/// Lay out this {@code BigDecimal} into a {@code char[]} array.
		/// The Java 1.2 equivalent to this was called {@code getValueString}.
		/// </summary>
		/// <param name="sci"> {@code true} for Scientific exponential notation;
		///          {@code false} for Engineering </param>
		/// <returns> string with canonical string representation of this
		///         {@code BigDecimal} </returns>
		private String LayoutChars(bool sci)
		{
			if (Scale_Renamed == 0) // zero scale is trivial
			{
				return (IntCompact != INFLATED) ? Convert.ToString(IntCompact): IntVal.ToString();
			}
			if (Scale_Renamed == 2 && IntCompact >= 0 && IntCompact < Integer.MaxValue)
			{
				// currency fast path
				int lowInt = (int)IntCompact % 100;
				int highInt = (int)IntCompact / 100;
				return (Convert.ToString(highInt) + '.' + StringBuilderHelper.DIGIT_TENS[lowInt] + StringBuilderHelper.DIGIT_ONES[lowInt]);
			}

			StringBuilderHelper sbHelper = threadLocalStringBuilderHelper.get();
			char[] coeff;
			int offset; // offset is the starting index for coeff array
			// Get the significand as an absolute value
			if (IntCompact != INFLATED)
			{
				offset = sbHelper.PutIntCompact(System.Math.Abs(IntCompact));
				coeff = sbHelper.CompactCharArray;
			}
			else
			{
				offset = 0;
				coeff = IntVal.Abs().ToString().ToCharArray();
			}

			// Construct a buffer, with sufficient capacity for all cases.
			// If E-notation is needed, length will be: +1 if negative, +1
			// if '.' needed, +2 for "E+", + up to 10 for adjusted exponent.
			// Otherwise it could have +1 if negative, plus leading "0.00000"
			StringBuilder buf = sbHelper.StringBuilder;
			if (Signum() < 0) // prefix '-' if negative
			{
				buf.Append('-');
			}
			int coeffLen = coeff.Length - offset;
			long adjusted = -(long)Scale_Renamed + (coeffLen - 1);
			if ((Scale_Renamed >= 0) && (adjusted >= -6)) // plain number
			{
				int pad = Scale_Renamed - coeffLen; // count of padding zeros
				if (pad >= 0) // 0.xxx form
				{
					buf.Append('0');
					buf.Append('.');
					for (; pad > 0; pad--)
					{
						buf.Append('0');
					}
					buf.Append(coeff, offset, coeffLen);
				} // xx.xx form
				else
				{
					buf.Append(coeff, offset, -pad);
					buf.Append('.');
					buf.Append(coeff, -pad + offset, Scale_Renamed);
				}
			} // E-notation is needed
			else
			{
				if (sci) // Scientific notation
				{
					buf.Append(coeff[offset]); // first character
					if (coeffLen > 1) // more to come
					{
						buf.Append('.');
						buf.Append(coeff, offset + 1, coeffLen - 1);
					}
				} // Engineering notation
				else
				{
					int sig = (int)(adjusted % 3);
					if (sig < 0)
					{
						sig += 3; // [adjusted was negative]
					}
					adjusted -= sig; // now a multiple of 3
					sig++;
					if (Signum() == 0)
					{
						switch (sig)
						{
						case 1:
							buf.Append('0'); // exponent is a multiple of three
							break;
						case 2:
							buf.Append("0.00");
							adjusted += 3;
							break;
						case 3:
							buf.Append("0.0");
							adjusted += 3;
							break;
						default:
							throw new AssertionError("Unexpected sig value " + sig);
						}
					} // significand all in integer
					else if (sig >= coeffLen)
					{
						buf.Append(coeff, offset, coeffLen);
						// may need some zeros, too
						for (int i = sig - coeffLen; i > 0; i--)
						{
							buf.Append('0');
						}
					} // xx.xxE form
					else
					{
						buf.Append(coeff, offset, sig);
						buf.Append('.');
						buf.Append(coeff, offset + sig, coeffLen - sig);
					}
				}
				if (adjusted != 0) // [!sci could have made 0]
				{
					buf.Append('E');
					if (adjusted > 0) // force sign for positive
					{
						buf.Append('+');
					}
					buf.Append(adjusted);
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Return 10 to the power n, as a {@code BigInteger}.
		/// </summary>
		/// <param name="n"> the power of ten to be returned (>=0) </param>
		/// <returns> a {@code BigInteger} with the value (10<sup>n</sup>) </returns>
		private static BigInteger BigTenToThe(int n)
		{
			if (n < 0)
			{
				return BigInteger.ZERO;
			}

			if (n < BIG_TEN_POWERS_TABLE_MAX)
			{
				BigInteger[] pows = BIG_TEN_POWERS_TABLE;
				if (n < pows.Length)
				{
					return pows[n];
				}
				else
				{
					return ExpandBigIntegerTenPowers(n);
				}
			}

			return BigInteger.TEN.Pow(n);
		}

		/// <summary>
		/// Expand the BIG_TEN_POWERS_TABLE array to contain at least 10**n.
		/// </summary>
		/// <param name="n"> the power of ten to be returned (>=0) </param>
		/// <returns> a {@code BigDecimal} with the value (10<sup>n</sup>) and
		///         in the meantime, the BIG_TEN_POWERS_TABLE array gets
		///         expanded to the size greater than n. </returns>
		private static BigInteger ExpandBigIntegerTenPowers(int n)
		{
			lock (typeof(BigDecimal))
			{
				BigInteger[] pows = BIG_TEN_POWERS_TABLE;
				int curLen = pows.Length;
				// The following comparison and the above synchronized statement is
				// to prevent multiple threads from expanding the same array.
				if (curLen <= n)
				{
					int newLen = curLen << 1;
					while (newLen <= n)
					{
						newLen <<= 1;
					}
					pows = Arrays.CopyOf(pows, newLen);
					for (int i = curLen; i < newLen; i++)
					{
						pows[i] = pows[i - 1].Multiply(BigInteger.TEN);
					}
					// Based on the following facts:
					// 1. pows is a private local varible;
					// 2. the following store is a volatile store.
					// the newly created array elements can be safely published.
					BIG_TEN_POWERS_TABLE = pows;
				}
				return pows[n];
			}
		}

		private static readonly long[] LONG_TEN_POWERS_TABLE = new long[] {1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000L, 100000000000L, 1000000000000L, 10000000000000L, 100000000000000L, 1000000000000000L, 10000000000000000L, 100000000000000000L, 1000000000000000000L};

		private static volatile BigInteger[] BIG_TEN_POWERS_TABLE = new BigInteger[] {BigInteger.ONE, BigInteger.ValueOf(10), BigInteger.ValueOf(100), BigInteger.ValueOf(1000), BigInteger.ValueOf(10000), BigInteger.ValueOf(100000), BigInteger.ValueOf(1000000), BigInteger.ValueOf(10000000), BigInteger.ValueOf(100000000), BigInteger.ValueOf(1000000000), BigInteger.ValueOf(10000000000L), BigInteger.ValueOf(100000000000L), BigInteger.ValueOf(1000000000000L), BigInteger.ValueOf(10000000000000L), BigInteger.ValueOf(100000000000000L), BigInteger.ValueOf(1000000000000000L), BigInteger.ValueOf(10000000000000000L), BigInteger.ValueOf(100000000000000000L), BigInteger.ValueOf(1000000000000000000L)};

		private static readonly int BIG_TEN_POWERS_TABLE_INITLEN = BIG_TEN_POWERS_TABLE.Length;
		private static readonly int BIG_TEN_POWERS_TABLE_MAX = 16 * BIG_TEN_POWERS_TABLE_INITLEN;

		private static readonly long[] THRESHOLDS_TABLE = new long[] {Long.MaxValue, Long.MaxValue / 10L, Long.MaxValue / 100L, Long.MaxValue / 1000L, Long.MaxValue / 10000L, Long.MaxValue / 100000L, Long.MaxValue / 1000000L, Long.MaxValue / 10000000L, Long.MaxValue / 100000000L, Long.MaxValue / 1000000000L, Long.MaxValue / 10000000000L, Long.MaxValue / 100000000000L, Long.MaxValue / 1000000000000L, Long.MaxValue / 10000000000000L, Long.MaxValue / 100000000000000L, Long.MaxValue / 1000000000000000L, Long.MaxValue / 10000000000000000L, Long.MaxValue / 100000000000000000L, Long.MaxValue / 1000000000000000000L};

		/// <summary>
		/// Compute val * 10 ^ n; return this product if it is
		/// representable as a long, INFLATED otherwise.
		/// </summary>
		private static long LongMultiplyPowerTen(long val, int n)
		{
			if (val == 0 || n <= 0)
			{
				return val;
			}
			long[] tab = LONG_TEN_POWERS_TABLE;
			long[] bounds = THRESHOLDS_TABLE;
			if (n < tab.Length && n < bounds.Length)
			{
				long tenpower = tab[n];
				if (val == 1)
				{
					return tenpower;
				}
				if (System.Math.Abs(val) <= bounds[n])
				{
					return val * tenpower;
				}
			}
			return INFLATED;
		}

		/// <summary>
		/// Compute this * 10 ^ n.
		/// Needed mainly to allow special casing to trap zero value
		/// </summary>
		private BigInteger BigMultiplyPowerTen(int n)
		{
			if (n <= 0)
			{
				return this.Inflated();
			}

			if (IntCompact != INFLATED)
			{
				return BigTenToThe(n).Multiply(IntCompact);
			}
			else
			{
				return IntVal.Multiply(BigTenToThe(n));
			}
		}

		/// <summary>
		/// Returns appropriate BigInteger from intVal field if intVal is
		/// null, i.e. the compact representation is in use.
		/// </summary>
		private BigInteger Inflated()
		{
			if (IntVal == null)
			{
				return BigInteger.ValueOf(IntCompact);
			}
			return IntVal;
		}

		/// <summary>
		/// Match the scales of two {@code BigDecimal}s to align their
		/// least significant digits.
		/// 
		/// <para>If the scales of val[0] and val[1] differ, rescale
		/// (non-destructively) the lower-scaled {@code BigDecimal} so
		/// they match.  That is, the lower-scaled reference will be
		/// replaced by a reference to a new object with the same scale as
		/// the other {@code BigDecimal}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> array of two elements referring to the two
		///         {@code BigDecimal}s to be aligned. </param>
		private static void MatchScale(BigDecimal[] val)
		{
			if (val[0].Scale_Renamed == val[1].Scale_Renamed)
			{
				return;
			}
			else if (val[0].Scale_Renamed < val[1].Scale_Renamed)
			{
				val[0] = val[0].SetScale(val[1].Scale_Renamed, ROUND_UNNECESSARY);
			}
			else if (val[1].Scale_Renamed < val[0].Scale_Renamed)
			{
				val[1] = val[1].SetScale(val[0].Scale_Renamed, ROUND_UNNECESSARY);
			}
		}

		private class UnsafeHolder
		{
			internal static readonly sun.misc.Unsafe @unsafe;
			internal static readonly long IntCompactOffset;
			internal static readonly long IntValOffset;
			static UnsafeHolder()
			{
				try
				{
					@unsafe = sun.misc.Unsafe.Unsafe;
					IntCompactOffset = @unsafe.objectFieldOffset(typeof(BigDecimal).getDeclaredField("intCompact"));
					IntValOffset = @unsafe.objectFieldOffset(typeof(BigDecimal).getDeclaredField("intVal"));
				}
				catch (Exception ex)
				{
					throw new ExceptionInInitializerError(ex);
				}
			}
			internal static void SetIntCompactVolatile(BigDecimal bd, long val)
			{
				@unsafe.putLongVolatile(bd, IntCompactOffset, val);
			}

			internal static void SetIntValVolatile(BigDecimal bd, BigInteger val)
			{
				@unsafe.putObjectVolatile(bd, IntValOffset, val);
			}
		}

		/// <summary>
		/// Reconstitute the {@code BigDecimal} instance from a stream (that is,
		/// deserialize it).
		/// </summary>
		/// <param name="s"> the stream being read. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in all fields
			s.DefaultReadObject();
			// validate possibly bad fields
			if (IntVal == null)
			{
				String message = "BigDecimal: null intVal in stream";
				throw new java.io.StreamCorruptedException(message);
			// [all values of scale are now allowed]
			}
			UnsafeHolder.SetIntCompactVolatile(this, CompactValFor(IntVal));
		}

	   /// <summary>
	   /// Serialize this {@code BigDecimal} to the stream in question
	   /// </summary>
	   /// <param name="s"> the stream to serialize to. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
	   private void WriteObject(java.io.ObjectOutputStream s)
	   {
		   // Must inflate to maintain compatible serial form.
		   if (this.IntVal == null)
		   {
			   UnsafeHolder.SetIntValVolatile(this, BigInteger.ValueOf(this.IntCompact));
		   }
		   // Could reset intVal back to null if it has to be set.
		   s.DefaultWriteObject();
	   }

		/// <summary>
		/// Returns the length of the absolute value of a {@code long}, in decimal
		/// digits.
		/// </summary>
		/// <param name="x"> the {@code long} </param>
		/// <returns> the length of the unscaled value, in deciaml digits. </returns>
		internal static int LongDigitLength(long x)
		{
			/*
			 * As described in "Bit Twiddling Hacks" by Sean Anderson,
			 * (http://graphics.stanford.edu/~seander/bithacks.html)
			 * integer log 10 of x is within 1 of (1233/4096)* (1 +
			 * integer log 2 of x). The fraction 1233/4096 approximates
			 * log10(2). So we first do a version of log2 (a variant of
			 * Long class with pre-checks and opposite directionality) and
			 * then scale and check against powers table. This is a little
			 * simpler in present context than the version in Hacker's
			 * Delight sec 11-4. Adding one to bit length allows comparing
			 * downward from the LONG_TEN_POWERS_TABLE that we need
			 * anyway.
			 */
			Debug.Assert(x != BigDecimal.INFLATED);
			if (x < 0)
			{
				x = -x;
			}
			if (x < 10) // must screen for 0, might as well 10
			{
				return 1;
			}
			int r = (int)((uint)((64 - Long.NumberOfLeadingZeros(x) + 1) * 1233) >> 12);
			long[] tab = LONG_TEN_POWERS_TABLE;
			// if r >= length, must have max possible digits for long
			return (r >= tab.Length || x < tab[r]) ? r : r + 1;
		}

		/// <summary>
		/// Returns the length of the absolute value of a BigInteger, in
		/// decimal digits.
		/// </summary>
		/// <param name="b"> the BigInteger </param>
		/// <returns> the length of the unscaled value, in decimal digits </returns>
		private static int BigDigitLength(BigInteger b)
		{
			/*
			 * Same idea as the long version, but we need a better
			 * approximation of log10(2). Using 646456993/2^31
			 * is accurate up to max possible reported bitLength.
			 */
			if (b.Signum_Renamed == 0)
			{
				return 1;
			}
			int r = (int)((int)((uint)(((long)b.BitLength() + 1) * 646456993) >> 31));
			return b.CompareMagnitude(BigTenToThe(r)) < 0? r : r + 1;
		}

		/// <summary>
		/// Check a scale for Underflow or Overflow.  If this BigDecimal is
		/// nonzero, throw an exception if the scale is outof range. If this
		/// is zero, saturate the scale to the extreme value of the right
		/// sign if the scale is out of range.
		/// </summary>
		/// <param name="val"> The new scale. </param>
		/// <exception cref="ArithmeticException"> (overflow or underflow) if the new
		///         scale is out of range. </exception>
		/// <returns> validated scale as an int. </returns>
		private int CheckScale(long val)
		{
			int asInt = (int)val;
			if (asInt != val)
			{
				asInt = val > Integer.MaxValue ? Integer.MaxValue : Integer.MinValue;
				BigInteger b;
				if (IntCompact != 0 && ((b = IntVal) == null || b.Signum() != 0))
				{
					throw new ArithmeticException(asInt > 0 ? "Underflow":"Overflow");
				}
			}
			return asInt;
		}

	   /// <summary>
	   /// Returns the compact value for given {@code BigInteger}, or
	   /// INFLATED if too big. Relies on internal representation of
	   /// {@code BigInteger}.
	   /// </summary>
		private static long CompactValFor(BigInteger b)
		{
			int[] m = b.Mag;
			int len = m.Length;
			if (len == 0)
			{
				return 0;
			}
			int d = m[0];
			if (len > 2 || (len == 2 && d < 0))
			{
				return INFLATED;
			}

			long u = (len == 2)? (((long) m[1] & LONG_MASK) + (((long)d) << 32)) : (((long)d) & LONG_MASK);
			return (b.Signum_Renamed < 0)? - u : u;
		}

		private static int LongCompareMagnitude(long x, long y)
		{
			if (x < 0)
			{
				x = -x;
			}
			if (y < 0)
			{
				y = -y;
			}
			return (x < y) ? - 1 : ((x == y) ? 0 : 1);
		}

		private static int SaturateLong(long s)
		{
			int i = (int)s;
			return (s == i) ? i : (s < 0 ? Integer.MinValue : Integer.MaxValue);
		}

		/*
		 * Internal printing routine
		 */
		private static void Print(String name, BigDecimal bd)
		{
			System.err.format("%s:\tintCompact %d\tintVal %d\tscale %d\tprecision %d%n", name, bd.IntCompact, bd.IntVal, bd.Scale_Renamed, bd.Precision_Renamed);
		}

		/// <summary>
		/// Check internal invariants of this BigDecimal.  These invariants
		/// include:
		/// 
		/// <ul>
		/// 
		/// <li>The object must be initialized; either intCompact must not be
		/// INFLATED or intVal is non-null.  Both of these conditions may
		/// be true.
		/// 
		/// <li>If both intCompact and intVal and set, their values must be
		/// consistent.
		/// 
		/// <li>If precision is nonzero, it must have the right value.
		/// </ul>
		/// 
		/// Note: Since this is an audit method, we are not supposed to change the
		/// state of this BigDecimal object.
		/// </summary>
		private BigDecimal Audit()
		{
			if (IntCompact == INFLATED)
			{
				if (IntVal == null)
				{
					Print("audit", this);
					throw new AssertionError("null intVal");
				}
				// Check precision
				if (Precision_Renamed > 0 && Precision_Renamed != BigDigitLength(IntVal))
				{
					Print("audit", this);
					throw new AssertionError("precision mismatch");
				}
			}
			else
			{
				if (IntVal != null)
				{
					long val = IntVal.LongValue();
					if (val != IntCompact)
					{
						Print("audit", this);
						throw new AssertionError("Inconsistent state, intCompact=" + IntCompact + "\t intVal=" + val);
					}
				}
				// Check precision
				if (Precision_Renamed > 0 && Precision_Renamed != LongDigitLength(IntCompact))
				{
					Print("audit", this);
					throw new AssertionError("precision mismatch");
				}
			}
			return this;
		}

		/* the same as checkScale where value!=0 */
		private static int CheckScaleNonZero(long val)
		{
			int asInt = (int)val;
			if (asInt != val)
			{
				throw new ArithmeticException(asInt > 0 ? "Underflow":"Overflow");
			}
			return asInt;
		}

		private static int CheckScale(long intCompact, long val)
		{
			int asInt = (int)val;
			if (asInt != val)
			{
				asInt = val > Integer.MaxValue ? Integer.MaxValue : Integer.MinValue;
				if (intCompact != 0)
				{
					throw new ArithmeticException(asInt > 0 ? "Underflow":"Overflow");
				}
			}
			return asInt;
		}

		private static int CheckScale(BigInteger intVal, long val)
		{
			int asInt = (int)val;
			if (asInt != val)
			{
				asInt = val > Integer.MaxValue ? Integer.MaxValue : Integer.MinValue;
				if (intVal.Signum() != 0)
				{
					throw new ArithmeticException(asInt > 0 ? "Underflow":"Overflow");
				}
			}
			return asInt;
		}

		/// <summary>
		/// Returns a {@code BigDecimal} rounded according to the MathContext
		/// settings;
		/// If rounding is needed a new {@code BigDecimal} is created and returned.
		/// </summary>
		/// <param name="val"> the value to be rounded </param>
		/// <param name="mc"> the context to use. </param>
		/// <returns> a {@code BigDecimal} rounded according to the MathContext
		///         settings.  May return {@code value}, if no rounding needed. </returns>
		/// <exception cref="ArithmeticException"> if the rounding mode is
		///         {@code RoundingMode.UNNECESSARY} and the
		///         result is inexact. </exception>
		private static BigDecimal DoRound(BigDecimal val, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			bool wasDivided = false;
			if (mcp > 0)
			{
				BigInteger intVal = val.IntVal;
				long compactVal = val.IntCompact;
				int scale = val.Scale_Renamed;
				int prec = val.Precision();
				int mode = mc.RoundingMode_Renamed.oldMode;
				int drop;
				if (compactVal == INFLATED)
				{
					drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						intVal = DivideAndRoundByTenPow(intVal, drop, mode);
						wasDivided = true;
						compactVal = CompactValFor(intVal);
						if (compactVal != INFLATED)
						{
							prec = LongDigitLength(compactVal);
							break;
						}
						prec = BigDigitLength(intVal);
						drop = prec - mcp;
					}
				}
				if (compactVal != INFLATED)
				{
					drop = prec - mcp; // drop can't be more than 18
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
						wasDivided = true;
						prec = LongDigitLength(compactVal);
						drop = prec - mcp;
						intVal = null;
					}
				}
				return wasDivided ? new BigDecimal(intVal,compactVal,scale,prec) : val;
			}
			return val;
		}

		/*
		 * Returns a {@code BigDecimal} created from {@code long} value with
		 * given scale rounded according to the MathContext settings
		 */
		private static BigDecimal DoRound(long compactVal, int scale, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			if (mcp > 0 && mcp < 19)
			{
				int prec = LongDigitLength(compactVal);
				int drop = prec - mcp; // drop can't be more than 18
				while (drop > 0)
				{
					scale = CheckScaleNonZero((long) scale - drop);
					compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
					prec = LongDigitLength(compactVal);
					drop = prec - mcp;
				}
				return ValueOf(compactVal, scale, prec);
			}
			return ValueOf(compactVal, scale);
		}

		/*
		 * Returns a {@code BigDecimal} created from {@code BigInteger} value with
		 * given scale rounded according to the MathContext settings
		 */
		private static BigDecimal DoRound(BigInteger intVal, int scale, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			int prec = 0;
			if (mcp > 0)
			{
				long compactVal = CompactValFor(intVal);
				int mode = mc.RoundingMode_Renamed.oldMode;
				int drop;
				if (compactVal == INFLATED)
				{
					prec = BigDigitLength(intVal);
					drop = prec - mcp;
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						intVal = DivideAndRoundByTenPow(intVal, drop, mode);
						compactVal = CompactValFor(intVal);
						if (compactVal != INFLATED)
						{
							break;
						}
						prec = BigDigitLength(intVal);
						drop = prec - mcp;
					}
				}
				if (compactVal != INFLATED)
				{
					prec = LongDigitLength(compactVal);
					drop = prec - mcp; // drop can't be more than 18
					while (drop > 0)
					{
						scale = CheckScaleNonZero((long) scale - drop);
						compactVal = DivideAndRound(compactVal, LONG_TEN_POWERS_TABLE[drop], mc.RoundingMode_Renamed.oldMode);
						prec = LongDigitLength(compactVal);
						drop = prec - mcp;
					}
					return ValueOf(compactVal,scale,prec);
				}
			}
			return new BigDecimal(intVal,INFLATED,scale,prec);
		}

		/*
		 * Divides {@code BigInteger} value by ten power.
		 */
		private static BigInteger DivideAndRoundByTenPow(BigInteger intVal, int tenPow, int roundingMode)
		{
			if (tenPow < LONG_TEN_POWERS_TABLE.Length)
			{
				intVal = DivideAndRound(intVal, LONG_TEN_POWERS_TABLE[tenPow], roundingMode);
			}
			else
			{
				intVal = DivideAndRound(intVal, BigTenToThe(tenPow), roundingMode);
			}
			return intVal;
		}

		/// <summary>
		/// Internally used for division operation for division {@code long} by
		/// {@code long}.
		/// The returned {@code BigDecimal} object is the quotient whose scale is set
		/// to the passed in scale. If the remainder is not zero, it will be rounded
		/// based on the passed in roundingMode. Also, if the remainder is zero and
		/// the last parameter, i.e. preferredScale is NOT equal to scale, the
		/// trailing zeros of the result is stripped to match the preferredScale.
		/// </summary>
		private static BigDecimal DivideAndRound(long ldividend, long ldivisor, int scale, int roundingMode, int preferredScale)
		{

			int qsign; // quotient sign
			long q = ldividend / ldivisor; // store quotient in long
			if (roundingMode == ROUND_DOWN && scale == preferredScale)
			{
				return ValueOf(q, scale);
			}
			long r = ldividend % ldivisor; // store remainder in long
			qsign = ((ldividend < 0) == (ldivisor < 0)) ? 1 : -1;
			if (r != 0)
			{
				bool increment = NeedIncrement(ldivisor, roundingMode, qsign, q, r);
				return ValueOf((increment ? q + qsign : q), scale);
			}
			else
			{
				if (preferredScale != scale)
				{
					return CreateAndStripZerosToMatchScale(q, scale, preferredScale);
				}
				else
				{
					return ValueOf(q, scale);
				}
			}
		}

		/// <summary>
		/// Divides {@code long} by {@code long} and do rounding based on the
		/// passed in roundingMode.
		/// </summary>
		private static long DivideAndRound(long ldividend, long ldivisor, int roundingMode)
		{
			int qsign; // quotient sign
			long q = ldividend / ldivisor; // store quotient in long
			if (roundingMode == ROUND_DOWN)
			{
				return q;
			}
			long r = ldividend % ldivisor; // store remainder in long
			qsign = ((ldividend < 0) == (ldivisor < 0)) ? 1 : -1;
			if (r != 0)
			{
				bool increment = NeedIncrement(ldivisor, roundingMode, qsign, q, r);
				return increment ? q + qsign : q;
			}
			else
			{
				return q;
			}
		}

		/// <summary>
		/// Shared logic of need increment computation.
		/// </summary>
		private static bool CommonNeedIncrement(int roundingMode, int qsign, int cmpFracHalf, bool oddQuot)
		{
			switch (roundingMode)
			{
			case ROUND_UNNECESSARY:
				throw new ArithmeticException("Rounding necessary");

			case ROUND_UP: // Away from zero
				return true;

			case ROUND_DOWN: // Towards zero
				return false;

			case ROUND_CEILING: // Towards +infinity
				return qsign > 0;

			case ROUND_FLOOR: // Towards -infinity
				return qsign < 0;

			default: // Some kind of half-way rounding
				Debug.Assert(roundingMode >= ROUND_HALF_UP && roundingMode <= ROUND_HALF_EVEN, "Unexpected rounding mode" + RoundingMode.ValueOf(roundingMode));

				if (cmpFracHalf < 0) // We're closer to higher digit
				{
					return false;
				}
				else if (cmpFracHalf > 0) // We're closer to lower digit
				{
					return true;
				}
				else // half-way
				{
					Debug.Assert(cmpFracHalf == 0);

					switch (roundingMode)
					{
					case ROUND_HALF_DOWN:
						return false;

					case ROUND_HALF_UP:
						return true;

					case ROUND_HALF_EVEN:
						return oddQuot;

					default:
						throw new AssertionError("Unexpected rounding mode" + roundingMode);
					}
				}
			break;
			}
		}

		/// <summary>
		/// Tests if quotient has to be incremented according the roundingMode
		/// </summary>
		private static bool NeedIncrement(long ldivisor, int roundingMode, int qsign, long q, long r)
		{
			Debug.Assert(r != 0L);

			int cmpFracHalf;
			if (r <= HALF_LONG_MIN_VALUE || r > HALF_LONG_MAX_VALUE)
			{
				cmpFracHalf = 1; // 2 * r can't fit into long
			}
			else
			{
				cmpFracHalf = LongCompareMagnitude(2 * r, ldivisor);
			}

			return CommonNeedIncrement(roundingMode, qsign, cmpFracHalf, (q & 1L) != 0L);
		}

		/// <summary>
		/// Divides {@code BigInteger} value by {@code long} value and
		/// do rounding based on the passed in roundingMode.
		/// </summary>
		private static BigInteger DivideAndRound(BigInteger bdividend, long ldivisor, int roundingMode)
		{
			bool isRemainderZero; // record remainder is zero or not
			int qsign; // quotient sign
			long r = 0; // store quotient & remainder in long
			MutableBigInteger mq = null; // store quotient
			// Descend into mutables for faster remainder checks
			MutableBigInteger mdividend = new MutableBigInteger(bdividend.Mag);
			mq = new MutableBigInteger();
			r = mdividend.Divide(ldivisor, mq);
			isRemainderZero = (r == 0);
			qsign = (ldivisor < 0) ? - bdividend.Signum_Renamed : bdividend.Signum_Renamed;
			if (!isRemainderZero)
			{
				if (NeedIncrement(ldivisor, roundingMode, qsign, mq, r))
				{
					mq.Add(MutableBigInteger.ONE);
				}
			}
			return mq.ToBigInteger(qsign);
		}

		/// <summary>
		/// Internally used for division operation for division {@code BigInteger}
		/// by {@code long}.
		/// The returned {@code BigDecimal} object is the quotient whose scale is set
		/// to the passed in scale. If the remainder is not zero, it will be rounded
		/// based on the passed in roundingMode. Also, if the remainder is zero and
		/// the last parameter, i.e. preferredScale is NOT equal to scale, the
		/// trailing zeros of the result is stripped to match the preferredScale.
		/// </summary>
		private static BigDecimal DivideAndRound(BigInteger bdividend, long ldivisor, int scale, int roundingMode, int preferredScale)
		{
			bool isRemainderZero; // record remainder is zero or not
			int qsign; // quotient sign
			long r = 0; // store quotient & remainder in long
			MutableBigInteger mq = null; // store quotient
			// Descend into mutables for faster remainder checks
			MutableBigInteger mdividend = new MutableBigInteger(bdividend.Mag);
			mq = new MutableBigInteger();
			r = mdividend.Divide(ldivisor, mq);
			isRemainderZero = (r == 0);
			qsign = (ldivisor < 0) ? - bdividend.Signum_Renamed : bdividend.Signum_Renamed;
			if (!isRemainderZero)
			{
				if (NeedIncrement(ldivisor, roundingMode, qsign, mq, r))
				{
					mq.Add(MutableBigInteger.ONE);
				}
				return mq.ToBigDecimal(qsign, scale);
			}
			else
			{
				if (preferredScale != scale)
				{
					long compactVal = mq.ToCompactValue(qsign);
					if (compactVal != INFLATED)
					{
						return CreateAndStripZerosToMatchScale(compactVal, scale, preferredScale);
					}
					BigInteger intVal = mq.ToBigInteger(qsign);
					return CreateAndStripZerosToMatchScale(intVal,scale, preferredScale);
				}
				else
				{
					return mq.ToBigDecimal(qsign, scale);
				}
			}
		}

		/// <summary>
		/// Tests if quotient has to be incremented according the roundingMode
		/// </summary>
		private static bool NeedIncrement(long ldivisor, int roundingMode, int qsign, MutableBigInteger mq, long r)
		{
			Debug.Assert(r != 0L);

			int cmpFracHalf;
			if (r <= HALF_LONG_MIN_VALUE || r > HALF_LONG_MAX_VALUE)
			{
				cmpFracHalf = 1; // 2 * r can't fit into long
			}
			else
			{
				cmpFracHalf = LongCompareMagnitude(2 * r, ldivisor);
			}

			return CommonNeedIncrement(roundingMode, qsign, cmpFracHalf, mq.Odd);
		}

		/// <summary>
		/// Divides {@code BigInteger} value by {@code BigInteger} value and
		/// do rounding based on the passed in roundingMode.
		/// </summary>
		private static BigInteger DivideAndRound(BigInteger bdividend, BigInteger bdivisor, int roundingMode)
		{
			bool isRemainderZero; // record remainder is zero or not
			int qsign; // quotient sign
			// Descend into mutables for faster remainder checks
			MutableBigInteger mdividend = new MutableBigInteger(bdividend.Mag);
			MutableBigInteger mq = new MutableBigInteger();
			MutableBigInteger mdivisor = new MutableBigInteger(bdivisor.Mag);
			MutableBigInteger mr = mdividend.Divide(mdivisor, mq);
			isRemainderZero = mr.Zero;
			qsign = (bdividend.Signum_Renamed != bdivisor.Signum_Renamed) ? - 1 : 1;
			if (!isRemainderZero)
			{
				if (NeedIncrement(mdivisor, roundingMode, qsign, mq, mr))
				{
					mq.Add(MutableBigInteger.ONE);
				}
			}
			return mq.ToBigInteger(qsign);
		}

		/// <summary>
		/// Internally used for division operation for division {@code BigInteger}
		/// by {@code BigInteger}.
		/// The returned {@code BigDecimal} object is the quotient whose scale is set
		/// to the passed in scale. If the remainder is not zero, it will be rounded
		/// based on the passed in roundingMode. Also, if the remainder is zero and
		/// the last parameter, i.e. preferredScale is NOT equal to scale, the
		/// trailing zeros of the result is stripped to match the preferredScale.
		/// </summary>
		private static BigDecimal DivideAndRound(BigInteger bdividend, BigInteger bdivisor, int scale, int roundingMode, int preferredScale)
		{
			bool isRemainderZero; // record remainder is zero or not
			int qsign; // quotient sign
			// Descend into mutables for faster remainder checks
			MutableBigInteger mdividend = new MutableBigInteger(bdividend.Mag);
			MutableBigInteger mq = new MutableBigInteger();
			MutableBigInteger mdivisor = new MutableBigInteger(bdivisor.Mag);
			MutableBigInteger mr = mdividend.Divide(mdivisor, mq);
			isRemainderZero = mr.Zero;
			qsign = (bdividend.Signum_Renamed != bdivisor.Signum_Renamed) ? - 1 : 1;
			if (!isRemainderZero)
			{
				if (NeedIncrement(mdivisor, roundingMode, qsign, mq, mr))
				{
					mq.Add(MutableBigInteger.ONE);
				}
				return mq.ToBigDecimal(qsign, scale);
			}
			else
			{
				if (preferredScale != scale)
				{
					long compactVal = mq.ToCompactValue(qsign);
					if (compactVal != INFLATED)
					{
						return CreateAndStripZerosToMatchScale(compactVal, scale, preferredScale);
					}
					BigInteger intVal = mq.ToBigInteger(qsign);
					return CreateAndStripZerosToMatchScale(intVal, scale, preferredScale);
				}
				else
				{
					return mq.ToBigDecimal(qsign, scale);
				}
			}
		}

		/// <summary>
		/// Tests if quotient has to be incremented according the roundingMode
		/// </summary>
		private static bool NeedIncrement(MutableBigInteger mdivisor, int roundingMode, int qsign, MutableBigInteger mq, MutableBigInteger mr)
		{
			Debug.Assert(!mr.Zero);
			int cmpFracHalf = mr.CompareHalf(mdivisor);
			return CommonNeedIncrement(roundingMode, qsign, cmpFracHalf, mq.Odd);
		}

		/// <summary>
		/// Remove insignificant trailing zeros from this
		/// {@code BigInteger} value until the preferred scale is reached or no
		/// more zeros can be removed.  If the preferred scale is less than
		/// Integer.MIN_VALUE, all the trailing zeros will be removed.
		/// </summary>
		/// <returns> new {@code BigDecimal} with a scale possibly reduced
		/// to be closed to the preferred scale. </returns>
		private static BigDecimal CreateAndStripZerosToMatchScale(BigInteger intVal, int scale, long preferredScale)
		{
			BigInteger[] qr; // quotient-remainder pair
			while (intVal.CompareMagnitude(BigInteger.TEN) >= 0 && scale > preferredScale)
			{
				if (intVal.TestBit(0))
				{
					break; // odd number cannot end in 0
				}
				qr = intVal.DivideAndRemainder(BigInteger.TEN);
				if (qr[1].Signum() != 0)
				{
					break; // non-0 remainder
				}
				intVal = qr[0];
				scale = CheckScale(intVal,(long) scale - 1); // could Overflow
			}
			return ValueOf(intVal, scale, 0);
		}

		/// <summary>
		/// Remove insignificant trailing zeros from this
		/// {@code long} value until the preferred scale is reached or no
		/// more zeros can be removed.  If the preferred scale is less than
		/// Integer.MIN_VALUE, all the trailing zeros will be removed.
		/// </summary>
		/// <returns> new {@code BigDecimal} with a scale possibly reduced
		/// to be closed to the preferred scale. </returns>
		private static BigDecimal CreateAndStripZerosToMatchScale(long compactVal, int scale, long preferredScale)
		{
			while (System.Math.Abs(compactVal) >= 10L && scale > preferredScale)
			{
				if ((compactVal & 1L) != 0L)
				{
					break; // odd number cannot end in 0
				}
				long r = compactVal % 10L;
				if (r != 0L)
				{
					break; // non-0 remainder
				}
				compactVal /= 10;
				scale = CheckScale(compactVal, (long) scale - 1); // could Overflow
			}
			return ValueOf(compactVal, scale);
		}

		private static BigDecimal StripZerosToMatchScale(BigInteger intVal, long intCompact, int scale, int preferredScale)
		{
			if (intCompact != INFLATED)
			{
				return CreateAndStripZerosToMatchScale(intCompact, scale, preferredScale);
			}
			else
			{
				return CreateAndStripZerosToMatchScale(intVal == null ? INFLATED_BIGINT : intVal, scale, preferredScale);
			}
		}

		/*
		 * returns INFLATED if oveflow
		 */
		private static long Add(long xs, long ys)
		{
			long sum = xs + ys;
			// See "Hacker's Delight" section 2-12 for explanation of
			// the overflow test.
			if ((((sum ^ xs) & (sum ^ ys))) >= 0L) // not overflowed
			{
				return sum;
			}
			return INFLATED;
		}

		private static BigDecimal Add(long xs, long ys, int scale)
		{
			long sum = Add(xs, ys);
			if (sum != INFLATED)
			{
				return BigDecimal.ValueOf(sum, scale);
			}
			return new BigDecimal(BigInteger.ValueOf(xs) + ys, scale);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static BigDecimal add(final long xs, int scale1, final long ys, int scale2)
		private static BigDecimal Add(long xs, int scale1, long ys, int scale2)
		{
			long sdiff = (long) scale1 - scale2;
			if (sdiff == 0)
			{
				return Add(xs, ys, scale1);
			}
			else if (sdiff < 0)
			{
				int raise = CheckScale(xs,-sdiff);
				long scaledX = LongMultiplyPowerTen(xs, raise);
				if (scaledX != INFLATED)
				{
					return Add(scaledX, ys, scale2);
				}
				else
				{
					BigInteger bigsum = BigMultiplyPowerTen(xs,raise).Add(ys);
					return ((xs ^ ys) >= 0) ? new BigDecimal(bigsum, INFLATED, scale2, 0) : ValueOf(bigsum, scale2, 0); // same sign test
				}
			}
			else
			{
				int raise = CheckScale(ys,sdiff);
				long scaledY = LongMultiplyPowerTen(ys, raise);
				if (scaledY != INFLATED)
				{
					return Add(xs, scaledY, scale1);
				}
				else
				{
					BigInteger bigsum = BigMultiplyPowerTen(ys,raise).Add(xs);
					return ((xs ^ ys) >= 0) ? new BigDecimal(bigsum, INFLATED, scale1, 0) : ValueOf(bigsum, scale1, 0);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static BigDecimal add(final long xs, int scale1, BigInteger snd, int scale2)
		private static BigDecimal Add(long xs, int scale1, BigInteger snd, int scale2)
		{
			int rscale = scale1;
			long sdiff = (long)rscale - scale2;
			bool sameSigns = (Long.Signum(xs) == snd.Signum_Renamed);
			BigInteger sum;
			if (sdiff < 0)
			{
				int raise = CheckScale(xs,-sdiff);
				rscale = scale2;
				long scaledX = LongMultiplyPowerTen(xs, raise);
				if (scaledX == INFLATED)
				{
					sum = snd.Add(BigMultiplyPowerTen(xs,raise));
				}
				else
				{
					sum = snd.Add(scaledX);
				}
			} //if (sdiff > 0) {
			else
			{
				int raise = CheckScale(snd,sdiff);
				snd = BigMultiplyPowerTen(snd,raise);
				sum = snd.Add(xs);
			}
			return (sameSigns) ? new BigDecimal(sum, INFLATED, rscale, 0) : ValueOf(sum, rscale, 0);
		}

		private static BigDecimal Add(BigInteger fst, int scale1, BigInteger snd, int scale2)
		{
			int rscale = scale1;
			long sdiff = (long)rscale - scale2;
			if (sdiff != 0)
			{
				if (sdiff < 0)
				{
					int raise = CheckScale(fst,-sdiff);
					rscale = scale2;
					fst = BigMultiplyPowerTen(fst,raise);
				}
				else
				{
					int raise = CheckScale(snd,sdiff);
					snd = BigMultiplyPowerTen(snd,raise);
				}
			}
			BigInteger sum = fst.Add(snd);
			return (fst.Signum_Renamed == snd.Signum_Renamed) ? new BigDecimal(sum, INFLATED, rscale, 0) : ValueOf(sum, rscale, 0);
		}

		private static BigInteger BigMultiplyPowerTen(long value, int n)
		{
			if (n <= 0)
			{
				return BigInteger.ValueOf(value);
			}
			return BigTenToThe(n).Multiply(value);
		}

		private static BigInteger BigMultiplyPowerTen(BigInteger value, int n)
		{
			if (n <= 0)
			{
				return value;
			}
			if (n < LONG_TEN_POWERS_TABLE.Length)
			{
					return value.Multiply(LONG_TEN_POWERS_TABLE[n]);
			}
			return value.Multiply(BigTenToThe(n));
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (xs /
		/// ys)}, with rounding according to the context settings.
		/// 
		/// Fast path - used only when (xscale <= yscale && yscale < 18
		///  && mc.presision<18) {
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static BigDecimal divideSmallFastPath(final long xs, int xscale, final long ys, int yscale, long preferredScale, MathContext mc)
		private static BigDecimal DivideSmallFastPath(long xs, int xscale, long ys, int yscale, long preferredScale, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			int roundingMode = mc.RoundingMode_Renamed.oldMode;

			assert(xscale <= yscale) && (yscale < 18) && (mcp < 18);
			int xraise = yscale - xscale; // xraise >=0
			long scaledX = (xraise == 0) ? xs : LongMultiplyPowerTen(xs, xraise); // can't overflow here!
			BigDecimal quotient;

			int cmp = LongCompareMagnitude(scaledX, ys);
			if (cmp > 0) // satisfy constraint (b)
			{
				yscale -= 1; // [that is, divisor *= 10]
				int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
				if (CheckScaleNonZero((long) mcp + yscale - xscale) > 0)
				{
					// assert newScale >= xscale
					int raise = CheckScaleNonZero((long) mcp + yscale - xscale);
					long scaledXs;
					if ((scaledXs = LongMultiplyPowerTen(xs, raise)) == INFLATED)
					{
						quotient = null;
						if ((mcp - 1) >= 0 && (mcp - 1) < LONG_TEN_POWERS_TABLE.Length)
						{
							quotient = MultiplyDivideAndRound(LONG_TEN_POWERS_TABLE[mcp - 1], scaledX, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
						}
						if (quotient == null)
						{
							BigInteger rb = BigMultiplyPowerTen(scaledX,mcp - 1);
							quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
						}
					}
					else
					{
						quotient = DivideAndRound(scaledXs, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
					}
				}
				else
				{
					int newScale = CheckScaleNonZero((long) xscale - mcp);
					// assert newScale >= yscale
					if (newScale == yscale) // easy case
					{
						quotient = DivideAndRound(xs, ys, scl, roundingMode,CheckScaleNonZero(preferredScale));
					}
					else
					{
						int raise = CheckScaleNonZero((long) newScale - yscale);
						long scaledYs;
						if ((scaledYs = LongMultiplyPowerTen(ys, raise)) == INFLATED)
						{
							BigInteger rb = BigMultiplyPowerTen(ys,raise);
							quotient = DivideAndRound(BigInteger.ValueOf(xs), rb, scl, roundingMode,CheckScaleNonZero(preferredScale));
						}
						else
						{
							quotient = DivideAndRound(xs, scaledYs, scl, roundingMode,CheckScaleNonZero(preferredScale));
						}
					}
				}
			}
			else
			{
				// abs(scaledX) <= abs(ys)
				// result is "scaledX * 10^msp / ys"
				int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
				if (cmp == 0)
				{
					// abs(scaleX)== abs(ys) => result will be scaled 10^mcp + correct sign
					quotient = RoundedTenPower(((scaledX < 0) == (ys < 0)) ? 1 : -1, mcp, scl, CheckScaleNonZero(preferredScale));
				}
				else
				{
					// abs(scaledX) < abs(ys)
					long scaledXs;
					if ((scaledXs = LongMultiplyPowerTen(scaledX, mcp)) == INFLATED)
					{
						quotient = null;
						if (mcp < LONG_TEN_POWERS_TABLE.Length)
						{
							quotient = MultiplyDivideAndRound(LONG_TEN_POWERS_TABLE[mcp], scaledX, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
						}
						if (quotient == null)
						{
							BigInteger rb = BigMultiplyPowerTen(scaledX,mcp);
							quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
						}
					}
					else
					{
						quotient = DivideAndRound(scaledXs, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
					}
				}
			}
			// doRound, here, only affects 1000000000 case.
			return DoRound(quotient,mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (xs /
		/// ys)}, with rounding according to the context settings.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static BigDecimal divide(final long xs, int xscale, final long ys, int yscale, long preferredScale, MathContext mc)
		private static BigDecimal Divide(long xs, int xscale, long ys, int yscale, long preferredScale, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			if (xscale <= yscale && yscale < 18 && mcp < 18)
			{
				return DivideSmallFastPath(xs, xscale, ys, yscale, preferredScale, mc);
			}
			if (CompareMagnitudeNormalized(xs, xscale, ys, yscale) > 0) // satisfy constraint (b)
			{
				yscale -= 1; // [that is, divisor *= 10]
			}
			int roundingMode = mc.RoundingMode_Renamed.oldMode;
			// In order to find out whether the divide generates the exact result,
			// we avoid calling the above divide method. 'quotient' holds the
			// return BigDecimal object whose scale will be set to 'scl'.
			int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
			BigDecimal quotient;
			if (CheckScaleNonZero((long) mcp + yscale - xscale) > 0)
			{
				int raise = CheckScaleNonZero((long) mcp + yscale - xscale);
				long scaledXs;
				if ((scaledXs = LongMultiplyPowerTen(xs, raise)) == INFLATED)
				{
					BigInteger rb = BigMultiplyPowerTen(xs,raise);
					quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
				}
				else
				{
					quotient = DivideAndRound(scaledXs, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
				}
			}
			else
			{
				int newScale = CheckScaleNonZero((long) xscale - mcp);
				// assert newScale >= yscale
				if (newScale == yscale) // easy case
				{
					quotient = DivideAndRound(xs, ys, scl, roundingMode,CheckScaleNonZero(preferredScale));
				}
				else
				{
					int raise = CheckScaleNonZero((long) newScale - yscale);
					long scaledYs;
					if ((scaledYs = LongMultiplyPowerTen(ys, raise)) == INFLATED)
					{
						BigInteger rb = BigMultiplyPowerTen(ys,raise);
						quotient = DivideAndRound(BigInteger.ValueOf(xs), rb, scl, roundingMode,CheckScaleNonZero(preferredScale));
					}
					else
					{
						quotient = DivideAndRound(xs, scaledYs, scl, roundingMode,CheckScaleNonZero(preferredScale));
					}
				}
			}
			// doRound, here, only affects 1000000000 case.
			return DoRound(quotient,mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (xs /
		/// ys)}, with rounding according to the context settings.
		/// </summary>
		private static BigDecimal Divide(BigInteger xs, int xscale, long ys, int yscale, long preferredScale, MathContext mc)
		{
			// Normalize dividend & divisor so that both fall into [0.1, 0.999...]
			if ((-CompareMagnitudeNormalized(ys, yscale, xs, xscale)) > 0) // satisfy constraint (b)
			{
				yscale -= 1; // [that is, divisor *= 10]
			}
			int mcp = mc.Precision_Renamed;
			int roundingMode = mc.RoundingMode_Renamed.oldMode;

			// In order to find out whether the divide generates the exact result,
			// we avoid calling the above divide method. 'quotient' holds the
			// return BigDecimal object whose scale will be set to 'scl'.
			BigDecimal quotient;
			int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
			if (CheckScaleNonZero((long) mcp + yscale - xscale) > 0)
			{
				int raise = CheckScaleNonZero((long) mcp + yscale - xscale);
				BigInteger rb = BigMultiplyPowerTen(xs,raise);
				quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
			}
			else
			{
				int newScale = CheckScaleNonZero((long) xscale - mcp);
				// assert newScale >= yscale
				if (newScale == yscale) // easy case
				{
					quotient = DivideAndRound(xs, ys, scl, roundingMode,CheckScaleNonZero(preferredScale));
				}
				else
				{
					int raise = CheckScaleNonZero((long) newScale - yscale);
					long scaledYs;
					if ((scaledYs = LongMultiplyPowerTen(ys, raise)) == INFLATED)
					{
						BigInteger rb = BigMultiplyPowerTen(ys,raise);
						quotient = DivideAndRound(xs, rb, scl, roundingMode,CheckScaleNonZero(preferredScale));
					}
					else
					{
						quotient = DivideAndRound(xs, scaledYs, scl, roundingMode,CheckScaleNonZero(preferredScale));
					}
				}
			}
			// doRound, here, only affects 1000000000 case.
			return DoRound(quotient, mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (xs /
		/// ys)}, with rounding according to the context settings.
		/// </summary>
		private static BigDecimal Divide(long xs, int xscale, BigInteger ys, int yscale, long preferredScale, MathContext mc)
		{
			// Normalize dividend & divisor so that both fall into [0.1, 0.999...]
			if (CompareMagnitudeNormalized(xs, xscale, ys, yscale) > 0) // satisfy constraint (b)
			{
				yscale -= 1; // [that is, divisor *= 10]
			}
			int mcp = mc.Precision_Renamed;
			int roundingMode = mc.RoundingMode_Renamed.oldMode;

			// In order to find out whether the divide generates the exact result,
			// we avoid calling the above divide method. 'quotient' holds the
			// return BigDecimal object whose scale will be set to 'scl'.
			BigDecimal quotient;
			int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
			if (CheckScaleNonZero((long) mcp + yscale - xscale) > 0)
			{
				int raise = CheckScaleNonZero((long) mcp + yscale - xscale);
				BigInteger rb = BigMultiplyPowerTen(xs,raise);
				quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
			}
			else
			{
				int newScale = CheckScaleNonZero((long) xscale - mcp);
				int raise = CheckScaleNonZero((long) newScale - yscale);
				BigInteger rb = BigMultiplyPowerTen(ys,raise);
				quotient = DivideAndRound(BigInteger.ValueOf(xs), rb, scl, roundingMode,CheckScaleNonZero(preferredScale));
			}
			// doRound, here, only affects 1000000000 case.
			return DoRound(quotient, mc);
		}

		/// <summary>
		/// Returns a {@code BigDecimal} whose value is {@code (xs /
		/// ys)}, with rounding according to the context settings.
		/// </summary>
		private static BigDecimal Divide(BigInteger xs, int xscale, BigInteger ys, int yscale, long preferredScale, MathContext mc)
		{
			// Normalize dividend & divisor so that both fall into [0.1, 0.999...]
			if (CompareMagnitudeNormalized(xs, xscale, ys, yscale) > 0) // satisfy constraint (b)
			{
				yscale -= 1; // [that is, divisor *= 10]
			}
			int mcp = mc.Precision_Renamed;
			int roundingMode = mc.RoundingMode_Renamed.oldMode;

			// In order to find out whether the divide generates the exact result,
			// we avoid calling the above divide method. 'quotient' holds the
			// return BigDecimal object whose scale will be set to 'scl'.
			BigDecimal quotient;
			int scl = CheckScaleNonZero(preferredScale + yscale - xscale + mcp);
			if (CheckScaleNonZero((long) mcp + yscale - xscale) > 0)
			{
				int raise = CheckScaleNonZero((long) mcp + yscale - xscale);
				BigInteger rb = BigMultiplyPowerTen(xs,raise);
				quotient = DivideAndRound(rb, ys, scl, roundingMode, CheckScaleNonZero(preferredScale));
			}
			else
			{
				int newScale = CheckScaleNonZero((long) xscale - mcp);
				int raise = CheckScaleNonZero((long) newScale - yscale);
				BigInteger rb = BigMultiplyPowerTen(ys,raise);
				quotient = DivideAndRound(xs, rb, scl, roundingMode,CheckScaleNonZero(preferredScale));
			}
			// doRound, here, only affects 1000000000 case.
			return DoRound(quotient, mc);
		}

		/*
		 * performs divideAndRound for (dividend0*dividend1, divisor)
		 * returns null if quotient can't fit into long value;
		 */
		private static BigDecimal MultiplyDivideAndRound(long dividend0, long dividend1, long divisor, int scale, int roundingMode, int preferredScale)
		{
			int qsign = Long.Signum(dividend0) * Long.Signum(dividend1) * Long.Signum(divisor);
			dividend0 = System.Math.Abs(dividend0);
			dividend1 = System.Math.Abs(dividend1);
			divisor = System.Math.Abs(divisor);
			// multiply dividend0 * dividend1
			long d0_hi = (long)((ulong)dividend0 >> 32);
			long d0_lo = dividend0 & LONG_MASK;
			long d1_hi = (long)((ulong)dividend1 >> 32);
			long d1_lo = dividend1 & LONG_MASK;
			long product = d0_lo * d1_lo;
			long d0 = product & LONG_MASK;
			long d1 = (long)((ulong)product >> 32);
			product = d0_hi * d1_lo + d1;
			d1 = product & LONG_MASK;
			long d2 = (long)((ulong)product >> 32);
			product = d0_lo * d1_hi + d1;
			d1 = product & LONG_MASK;
			d2 += (long)((ulong)product >> 32);
			long d3 = (long)((ulong)d2 >> 32);
			d2 &= LONG_MASK;
			product = d0_hi * d1_hi + d2;
			d2 = product & LONG_MASK;
			d3 = (((long)((ulong)product >> 32)) + d3) & LONG_MASK;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long dividendHi = make64(d3,d2);
			long dividendHi = Make64(d3,d2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long dividendLo = make64(d1,d0);
			long dividendLo = Make64(d1,d0);
			// divide
			return DivideAndRound128(dividendHi, dividendLo, divisor, qsign, scale, roundingMode, preferredScale);
		}

		private static readonly long DIV_NUM_BASE = (1L << 32); // Number base (32 bits).

		/*
		 * divideAndRound 128-bit value by long divisor.
		 * returns null if quotient can't fit into long value;
		 * Specialized version of Knuth's division
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static BigDecimal divideAndRound128(final long dividendHi, final long dividendLo, long divisor, int sign, int scale, int roundingMode, int preferredScale)
		private static BigDecimal DivideAndRound128(long dividendHi, long dividendLo, long divisor, int sign, int scale, int roundingMode, int preferredScale)
		{
			if (dividendHi >= divisor)
			{
				return null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int shift = Long.numberOfLeadingZeros(divisor);
			int shift = Long.NumberOfLeadingZeros(divisor);
			divisor <<= shift;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long v1 = divisor >>> 32;
			long v1 = (long)((ulong)divisor >> 32);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long v0 = divisor & LONG_MASK;
			long v0 = divisor & LONG_MASK;

			long tmp = dividendLo << shift;
			long u1 = (long)((ulong)tmp >> 32);
			long u0 = tmp & LONG_MASK;

			tmp = (dividendHi << shift) | ((long)((ulong)dividendLo >> 64 - shift));
			long u2 = tmp & LONG_MASK;
			long q1, r_tmp;
			if (v1 == 1)
			{
				q1 = tmp;
				r_tmp = 0;
			}
			else if (tmp >= 0)
			{
				q1 = tmp / v1;
				r_tmp = tmp - q1 * v1;
			}
			else
			{
				long[] rq = DivRemNegativeLong(tmp, v1);
				q1 = rq[1];
				r_tmp = rq[0];
			}

			while (q1 >= DIV_NUM_BASE || UnsignedLongCompare(q1 * v0, Make64(r_tmp, u1)))
			{
				q1--;
				r_tmp += v1;
				if (r_tmp >= DIV_NUM_BASE)
				{
					break;
				}
			}

			tmp = Mulsub(u2,u1,v1,v0,q1);
			u1 = tmp & LONG_MASK;
			long q0;
			if (v1 == 1)
			{
				q0 = tmp;
				r_tmp = 0;
			}
			else if (tmp >= 0)
			{
				q0 = tmp / v1;
				r_tmp = tmp - q0 * v1;
			}
			else
			{
				long[] rq = DivRemNegativeLong(tmp, v1);
				q0 = rq[1];
				r_tmp = rq[0];
			}

			while (q0 >= DIV_NUM_BASE || UnsignedLongCompare(q0 * v0,Make64(r_tmp,u0)))
			{
				q0--;
				r_tmp += v1;
				if (r_tmp >= DIV_NUM_BASE)
				{
					break;
				}
			}

			if ((int)q1 < 0)
			{
				// result (which is positive and unsigned here)
				// can't fit into long due to sign bit is used for value
				MutableBigInteger mq = new MutableBigInteger(new int[]{(int)q1, (int)q0});
				if (roundingMode == ROUND_DOWN && scale == preferredScale)
				{
					return mq.ToBigDecimal(sign, scale);
				}
				long r = (int)((uint)Mulsub(u1, u0, v1, v0, q0) >> shift);
				if (r != 0)
				{
					if (NeedIncrement((long)((ulong)divisor >> shift), roundingMode, sign, mq, r))
					{
						mq.Add(MutableBigInteger.ONE);
					}
					return mq.ToBigDecimal(sign, scale);
				}
				else
				{
					if (preferredScale != scale)
					{
						BigInteger intVal = mq.ToBigInteger(sign);
						return CreateAndStripZerosToMatchScale(intVal,scale, preferredScale);
					}
					else
					{
						return mq.ToBigDecimal(sign, scale);
					}
				}
			}

			long q = Make64(q1,q0);
			q *= sign;

			if (roundingMode == ROUND_DOWN && scale == preferredScale)
			{
				return ValueOf(q, scale);
			}

			long r = (int)((uint)Mulsub(u1, u0, v1, v0, q0) >> shift);
			if (r != 0)
			{
				bool increment = NeedIncrement((long)((ulong)divisor >> shift), roundingMode, sign, q, r);
				return ValueOf((increment ? q + sign : q), scale);
			}
			else
			{
				if (preferredScale != scale)
				{
					return CreateAndStripZerosToMatchScale(q, scale, preferredScale);
				}
				else
				{
					return ValueOf(q, scale);
				}
			}
		}

		/*
		 * calculate divideAndRound for ldividend*10^raise / divisor
		 * when abs(dividend)==abs(divisor);
		 */
		private static BigDecimal RoundedTenPower(int qsign, int raise, int scale, int preferredScale)
		{
			if (scale > preferredScale)
			{
				int diff = scale - preferredScale;
				if (diff < raise)
				{
					return ScaledTenPow(raise - diff, qsign, preferredScale);
				}
				else
				{
					return ValueOf(qsign,scale - raise);
				}
			}
			else
			{
				return ScaledTenPow(raise, qsign, scale);
			}
		}

		internal static BigDecimal ScaledTenPow(int n, int sign, int scale)
		{
			if (n < LONG_TEN_POWERS_TABLE.Length)
			{
				return ValueOf(sign * LONG_TEN_POWERS_TABLE[n],scale);
			}
			else
			{
				BigInteger unscaledVal = BigTenToThe(n);
				if (sign == -1)
				{
					unscaledVal = unscaledVal.Negate();
				}
				return new BigDecimal(unscaledVal, INFLATED, scale, n + 1);
			}
		}

		/// <summary>
		/// Calculate the quotient and remainder of dividing a negative long by
		/// another long.
		/// </summary>
		/// <param name="n"> the numerator; must be negative </param>
		/// <param name="d"> the denominator; must not be unity </param>
		/// <returns> a two-element {@long} array with the remainder and quotient in
		///         the initial and final elements, respectively </returns>
		private static long[] DivRemNegativeLong(long n, long d)
		{
			Debug.Assert(n < 0, "Non-negative numerator " + n);
			Debug.Assert(d != 1, "Unity denominator");

			// Approximate the quotient and remainder
			long q = ((long)((ulong)n >> 1)) / ((long)((ulong)d >> 1));
			long r = n - q * d;

			// Correct the approximation
			while (r < 0)
			{
				r += d;
				q--;
			}
			while (r >= d)
			{
				r -= d;
				q++;
			}

			// n - q*d == r && 0 <= r < d, hence we're done.
			return new long[] {r, q};
		}

		private static long Make64(long hi, long lo)
		{
			return hi << 32 | lo;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static long mulsub(long u1, long u0, final long v1, final long v0, long q0)
		private static long Mulsub(long u1, long u0, long v1, long v0, long q0)
		{
			long tmp = u0 - q0 * v0;
			return Make64(u1 + ((long)((ulong)tmp >> 32)) - q0 * v1,tmp & LONG_MASK);
		}

		private static bool UnsignedLongCompare(long one, long two)
		{
			return (one + Long.MinValue) > (two + Long.MinValue);
		}

		private static bool UnsignedLongCompareEq(long one, long two)
		{
			return (one + Long.MinValue) >= (two + Long.MinValue);
		}


		// Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
		private static int CompareMagnitudeNormalized(long xs, int xscale, long ys, int yscale)
		{
			// assert xs!=0 && ys!=0
			int sdiff = xscale - yscale;
			if (sdiff != 0)
			{
				if (sdiff < 0)
				{
					xs = LongMultiplyPowerTen(xs, -sdiff);
				} // sdiff > 0
				else
				{
					ys = LongMultiplyPowerTen(ys, sdiff);
				}
			}
			if (xs != INFLATED)
			{
				return (ys != INFLATED) ? LongCompareMagnitude(xs, ys) : -1;
			}
			else
			{
				return 1;
			}
		}

		// Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
		private static int CompareMagnitudeNormalized(long xs, int xscale, BigInteger ys, int yscale)
		{
			// assert "ys can't be represented as long"
			if (xs == 0)
			{
				return -1;
			}
			int sdiff = xscale - yscale;
			if (sdiff < 0)
			{
				if (LongMultiplyPowerTen(xs, -sdiff) == INFLATED)
				{
					return BigMultiplyPowerTen(xs, -sdiff).CompareMagnitude(ys);
				}
			}
			return -1;
		}

		// Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
		private static int CompareMagnitudeNormalized(BigInteger xs, int xscale, BigInteger ys, int yscale)
		{
			int sdiff = xscale - yscale;
			if (sdiff < 0)
			{
				return BigMultiplyPowerTen(xs, -sdiff).CompareMagnitude(ys);
			} // sdiff >= 0
			else
			{
				return xs.CompareMagnitude(BigMultiplyPowerTen(ys, sdiff));
			}
		}

		private static long Multiply(long x, long y)
		{
					long product = x * y;
			long ax = System.Math.Abs(x);
			long ay = System.Math.Abs(y);
			if (((int)((uint)(ax | ay) >> 31) == 0) || (y == 0) || (product / y == x))
			{
							return product;
			}
			return INFLATED;
		}

		private static BigDecimal Multiply(long x, long y, int scale)
		{
			long product = Multiply(x, y);
			if (product != INFLATED)
			{
				return ValueOf(product,scale);
			}
			return new BigDecimal(BigInteger.ValueOf(x) * y,INFLATED,scale,0);
		}

		private static BigDecimal Multiply(long x, BigInteger y, int scale)
		{
			if (x == 0)
			{
				return ZeroValueOf(scale);
			}
			return new BigDecimal(y.Multiply(x),INFLATED,scale,0);
		}

		private static BigDecimal Multiply(BigInteger x, BigInteger y, int scale)
		{
			return new BigDecimal(x.Multiply(y),INFLATED,scale,0);
		}

		/// <summary>
		/// Multiplies two long values and rounds according {@code MathContext}
		/// </summary>
		private static BigDecimal MultiplyAndRound(long x, long y, int scale, MathContext mc)
		{
			long product = Multiply(x, y);
			if (product != INFLATED)
			{
				return DoRound(product, scale, mc);
			}
			// attempt to do it in 128 bits
			int rsign = 1;
			if (x < 0)
			{
				x = -x;
				rsign = -1;
			}
			if (y < 0)
			{
				y = -y;
				rsign *= -1;
			}
			// multiply dividend0 * dividend1
			long m0_hi = (long)((ulong)x >> 32);
			long m0_lo = x & LONG_MASK;
			long m1_hi = (long)((ulong)y >> 32);
			long m1_lo = y & LONG_MASK;
			product = m0_lo * m1_lo;
			long m0 = product & LONG_MASK;
			long m1 = (long)((ulong)product >> 32);
			product = m0_hi * m1_lo + m1;
			m1 = product & LONG_MASK;
			long m2 = (long)((ulong)product >> 32);
			product = m0_lo * m1_hi + m1;
			m1 = product & LONG_MASK;
			m2 += (long)((ulong)product >> 32);
			long m3 = (long)((ulong)m2 >> 32);
			m2 &= LONG_MASK;
			product = m0_hi * m1_hi + m2;
			m2 = product & LONG_MASK;
			m3 = (((long)((ulong)product >> 32)) + m3) & LONG_MASK;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long mHi = make64(m3,m2);
			long mHi = Make64(m3,m2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long mLo = make64(m1,m0);
			long mLo = Make64(m1,m0);
			BigDecimal res = DoRound128(mHi, mLo, rsign, scale, mc);
			if (res != null)
			{
				return res;
			}
			res = new BigDecimal(BigInteger.ValueOf(x) * (y * rsign), INFLATED, scale, 0);
			return DoRound(res,mc);
		}

		private static BigDecimal MultiplyAndRound(long x, BigInteger y, int scale, MathContext mc)
		{
			if (x == 0)
			{
				return ZeroValueOf(scale);
			}
			return DoRound(y.Multiply(x), scale, mc);
		}

		private static BigDecimal MultiplyAndRound(BigInteger x, BigInteger y, int scale, MathContext mc)
		{
			return DoRound(x.Multiply(y), scale, mc);
		}

		/// <summary>
		/// rounds 128-bit value according {@code MathContext}
		/// returns null if result can't be repsented as compact BigDecimal.
		/// </summary>
		private static BigDecimal DoRound128(long hi, long lo, int sign, int scale, MathContext mc)
		{
			int mcp = mc.Precision_Renamed;
			int drop;
			BigDecimal res = null;
			if (((drop = Precision(hi, lo) - mcp) > 0) && (drop < LONG_TEN_POWERS_TABLE.Length))
			{
				scale = CheckScaleNonZero((long)scale - drop);
				res = DivideAndRound128(hi, lo, LONG_TEN_POWERS_TABLE[drop], sign, scale, mc.RoundingMode_Renamed.oldMode, scale);
			}
			if (res != null)
			{
				return DoRound(res,mc);
			}
			return null;
		}

		private static readonly long[][] LONGLONG_TEN_POWERS_TABLE = new long[][] {new long[] {0L, unchecked((long)0x8AC7230489E80000L)}, new long[] {0x5L, 0x6bc75e2d63100000L}, new long[] {0x36L, 0x35c9adc5dea00000L}, new long[] {0x21eL, 0x19e0c9bab2400000L}, new long[] {0x152dL, 0x02c7e14af6800000L}, new long[] {0xd3c2L, 0x1bcecceda1000000L}, new long[] {0x84595L, 0x161401484a000000L}, new long[] {0x52b7d2L, unchecked((long)0xdcc80cd2e4000000L)}, new long[] {0x33b2e3cL, unchecked((long)0x9fd0803ce8000000L)}, new long[] {0x204fce5eL, 0x3e25026110000000L}, new long[] {0x1431e0faeL, 0x6d7217caa0000000L}, new long[] {0xc9f2c9cd0L, 0x4674edea40000000L}, new long[] {0x7e37be2022L, unchecked((long)0xc0914b2680000000L)}, new long[] {0x4ee2d6d415bL, unchecked((long)0x85acef8100000000L)}, new long[] {0x314dc6448d93L, 0x38c15b0a00000000L}, new long[] {0x1ed09bead87c0L, 0x378d8e6400000000L}, new long[] {0x13426172c74d82L, 0x2b878fe800000000L}, new long[] {0xc097ce7bc90715L, unchecked((long)0xb34b9f1000000000L)}, new long[] {0x785ee10d5da46d9L, 0x00f436a000000000L}, new long[] {0x4b3b4ca85a86c47aL, 0x098a224000000000L}};

		/*
		 * returns precision of 128-bit value
		 */
		private static int Precision(long hi, long lo)
		{
			if (hi == 0)
			{
				if (lo >= 0)
				{
					return LongDigitLength(lo);
				}
				return (UnsignedLongCompareEq(lo, LONGLONG_TEN_POWERS_TABLE[0][1])) ? 20 : 19;
				// 0x8AC7230489E80000L  = unsigned 2^19
			}
			int r = (int)((uint)((128 - Long.NumberOfLeadingZeros(hi) + 1) * 1233) >> 12);
			int idx = r - 19;
			return (idx >= LONGLONG_TEN_POWERS_TABLE.Length || LongLongCompareMagnitude(hi, lo, LONGLONG_TEN_POWERS_TABLE[idx][0], LONGLONG_TEN_POWERS_TABLE[idx][1])) ? r : r + 1;
		}

		/*
		 * returns true if 128 bit number <hi0,lo0> is less then <hi1,lo1>
		 * hi0 & hi1 should be non-negative
		 */
		private static bool LongLongCompareMagnitude(long hi0, long lo0, long hi1, long lo1)
		{
			if (hi0 != hi1)
			{
				return hi0 < hi1;
			}
			return (lo0 + Long.MinValue) < (lo1 + Long.MinValue);
		}

		private static BigDecimal Divide(long dividend, int dividendScale, long divisor, int divisorScale, int scale, int roundingMode)
		{
			if (CheckScale(dividend,(long)scale + divisorScale) > dividendScale)
			{
				int newScale = scale + divisorScale;
				int raise = newScale - dividendScale;
				if (raise < LONG_TEN_POWERS_TABLE.Length)
				{
					long xs = dividend;
					if ((xs = LongMultiplyPowerTen(xs, raise)) != INFLATED)
					{
						return DivideAndRound(xs, divisor, scale, roundingMode, scale);
					}
					BigDecimal q = MultiplyDivideAndRound(LONG_TEN_POWERS_TABLE[raise], dividend, divisor, scale, roundingMode, scale);
					if (q != null)
					{
						return q;
					}
				}
				BigInteger scaledDividend = BigMultiplyPowerTen(dividend, raise);
				return DivideAndRound(scaledDividend, divisor, scale, roundingMode, scale);
			}
			else
			{
				int newScale = CheckScale(divisor,(long)dividendScale - scale);
				int raise = newScale - divisorScale;
				if (raise < LONG_TEN_POWERS_TABLE.Length)
				{
					long ys = divisor;
					if ((ys = LongMultiplyPowerTen(ys, raise)) != INFLATED)
					{
						return DivideAndRound(dividend, ys, scale, roundingMode, scale);
					}
				}
				BigInteger scaledDivisor = BigMultiplyPowerTen(divisor, raise);
				return DivideAndRound(BigInteger.ValueOf(dividend), scaledDivisor, scale, roundingMode, scale);
			}
		}

		private static BigDecimal Divide(BigInteger dividend, int dividendScale, long divisor, int divisorScale, int scale, int roundingMode)
		{
			if (CheckScale(dividend,(long)scale + divisorScale) > dividendScale)
			{
				int newScale = scale + divisorScale;
				int raise = newScale - dividendScale;
				BigInteger scaledDividend = BigMultiplyPowerTen(dividend, raise);
				return DivideAndRound(scaledDividend, divisor, scale, roundingMode, scale);
			}
			else
			{
				int newScale = CheckScale(divisor,(long)dividendScale - scale);
				int raise = newScale - divisorScale;
				if (raise < LONG_TEN_POWERS_TABLE.Length)
				{
					long ys = divisor;
					if ((ys = LongMultiplyPowerTen(ys, raise)) != INFLATED)
					{
						return DivideAndRound(dividend, ys, scale, roundingMode, scale);
					}
				}
				BigInteger scaledDivisor = BigMultiplyPowerTen(divisor, raise);
				return DivideAndRound(dividend, scaledDivisor, scale, roundingMode, scale);
			}
		}

		private static BigDecimal Divide(long dividend, int dividendScale, BigInteger divisor, int divisorScale, int scale, int roundingMode)
		{
			if (CheckScale(dividend,(long)scale + divisorScale) > dividendScale)
			{
				int newScale = scale + divisorScale;
				int raise = newScale - dividendScale;
				BigInteger scaledDividend = BigMultiplyPowerTen(dividend, raise);
				return DivideAndRound(scaledDividend, divisor, scale, roundingMode, scale);
			}
			else
			{
				int newScale = CheckScale(divisor,(long)dividendScale - scale);
				int raise = newScale - divisorScale;
				BigInteger scaledDivisor = BigMultiplyPowerTen(divisor, raise);
				return DivideAndRound(BigInteger.ValueOf(dividend), scaledDivisor, scale, roundingMode, scale);
			}
		}

		private static BigDecimal Divide(BigInteger dividend, int dividendScale, BigInteger divisor, int divisorScale, int scale, int roundingMode)
		{
			if (CheckScale(dividend,(long)scale + divisorScale) > dividendScale)
			{
				int newScale = scale + divisorScale;
				int raise = newScale - dividendScale;
				BigInteger scaledDividend = BigMultiplyPowerTen(dividend, raise);
				return DivideAndRound(scaledDividend, divisor, scale, roundingMode, scale);
			}
			else
			{
				int newScale = CheckScale(divisor,(long)dividendScale - scale);
				int raise = newScale - divisorScale;
				BigInteger scaledDivisor = BigMultiplyPowerTen(divisor, raise);
				return DivideAndRound(dividend, scaledDivisor, scale, roundingMode, scale);
			}
		}

	}

}