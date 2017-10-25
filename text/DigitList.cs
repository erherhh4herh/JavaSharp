using System;
using System.Diagnostics;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
 *
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{

	using FloatingDecimal = sun.misc.FloatingDecimal;

	/// <summary>
	/// Digit List. Private to DecimalFormat.
	/// Handles the transcoding
	/// between numeric values and strings of characters.  Only handles
	/// non-negative numbers.  The division of labor between DigitList and
	/// DecimalFormat is that DigitList handles the radix 10 representation
	/// issues; DecimalFormat handles the locale-specific issues such as
	/// positive/negative, grouping, decimal point, currency, and so on.
	/// 
	/// A DigitList is really a representation of a floating point value.
	/// It may be an integer value; we assume that a double has sufficient
	/// precision to represent all digits of a long.
	/// 
	/// The DigitList representation consists of a string of characters,
	/// which are the digits radix 10, from '0' to '9'.  It also has a radix
	/// 10 exponent associated with it.  The value represented by a DigitList
	/// object can be computed by mulitplying the fraction f, where 0 <= f < 1,
	/// derived by placing all the digits of the list to the right of the
	/// decimal point, by 10^exponent.
	/// </summary>
	/// <seealso cref=  Locale </seealso>
	/// <seealso cref=  Format </seealso>
	/// <seealso cref=  NumberFormat </seealso>
	/// <seealso cref=  DecimalFormat </seealso>
	/// <seealso cref=  ChoiceFormat </seealso>
	/// <seealso cref=  MessageFormat
	/// @author       Mark Davis, Alan Liu </seealso>
	internal sealed class DigitList : Cloneable
	{
		/// <summary>
		/// The maximum number of significant digits in an IEEE 754 double, that
		/// is, in a Java double.  This must not be increased, or garbage digits
		/// will be generated, and should not be decreased, or accuracy will be lost.
		/// </summary>
		public const int MAX_COUNT = 19; // == Long.toString(Long.MAX_VALUE).length()

		/// <summary>
		/// These data members are intentionally public and can be set directly.
		/// 
		/// The value represented is given by placing the decimal point before
		/// digits[decimalAt].  If decimalAt is < 0, then leading zeros between
		/// the decimal point and the first nonzero digit are implied.  If decimalAt
		/// is > count, then trailing zeros between the digits[count-1] and the
		/// decimal point are implied.
		/// 
		/// Equivalently, the represented value is given by f * 10^decimalAt.  Here
		/// f is a value 0.1 <= f < 1 arrived at by placing the digits in Digits to
		/// the right of the decimal.
		/// 
		/// DigitList is normalized, so if it is non-zero, figits[0] is non-zero.  We
		/// don't allow denormalized numbers because our exponent is effectively of
		/// unlimited magnitude.  The count value contains the number of significant
		/// digits present in digits[].
		/// 
		/// Zero is represented by any DigitList with count == 0 or with each digits[i]
		/// for all i <= count == '0'.
		/// </summary>
		public int DecimalAt = 0;
		public int Count = 0;
		public char[] Digits = new char[MAX_COUNT];

		private char[] Data;
		private RoundingMode RoundingMode_Renamed = RoundingMode.HALF_EVEN;
		private bool IsNegative = false;

		/// <summary>
		/// Return true if the represented number is zero.
		/// </summary>
		internal bool Zero
		{
			get
			{
				for (int i = 0; i < Count; ++i)
				{
					if (Digits[i] != '0')
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Set the rounding mode
		/// </summary>
		internal RoundingMode RoundingMode
		{
			set
			{
				RoundingMode_Renamed = value;
			}
		}

		/// <summary>
		/// Clears out the digits.
		/// Use before appending them.
		/// Typically, you set a series of digits with append, then at the point
		/// you hit the decimal point, you set myDigitList.decimalAt = myDigitList.count;
		/// then go on appending digits.
		/// </summary>
		public void Clear()
		{
			DecimalAt = 0;
			Count = 0;
		}

		/// <summary>
		/// Appends a digit to the list, extending the list when necessary.
		/// </summary>
		public void Append(char digit)
		{
			if (Count == Digits.Length)
			{
				char[] data = new char[Count + 100];
				System.Array.Copy(Digits, 0, data, 0, Count);
				Digits = data;
			}
			Digits[Count++] = digit;
		}

		/// <summary>
		/// Utility routine to get the value of the digit list
		/// If (count == 0) this throws a NumberFormatException, which
		/// mimics Long.parseLong().
		/// </summary>
		public double Double
		{
			get
			{
				if (Count == 0)
				{
					return 0.0;
				}
    
				StringBuffer temp = StringBuffer;
				temp.Append('.');
				temp.Append(Digits, 0, Count);
				temp.Append('E');
				temp.Append(DecimalAt);
				return Convert.ToDouble(temp.ToString());
			}
		}

		/// <summary>
		/// Utility routine to get the value of the digit list.
		/// If (count == 0) this returns 0, unlike Long.parseLong().
		/// </summary>
		public long Long
		{
			get
			{
				// for now, simple implementation; later, do proper IEEE native stuff
    
				if (Count == 0)
				{
					return 0;
				}
    
				// We have to check for this, because this is the one NEGATIVE value
				// we represent.  If we tried to just pass the digits off to parseLong,
				// we'd get a parse failure.
				if (LongMIN_VALUE)
				{
					return Long.MinValue;
				}
    
				StringBuffer temp = StringBuffer;
				temp.Append(Digits, 0, Count);
				for (int i = Count; i < DecimalAt; ++i)
				{
					temp.Append('0');
				}
				return Convert.ToInt64(temp.ToString());
			}
		}

		public decimal BigDecimal
		{
			get
			{
				if (Count == 0)
				{
					if (DecimalAt == 0)
					{
						return decimal.Zero;
					}
					else
					{
						return new decimal("0E" + DecimalAt);
					}
				}
    
			   if (DecimalAt == Count)
			   {
				   return new decimal(Digits, 0, Count);
			   }
			   else
			   {
				   return (new decimal(Digits, 0, Count)).ScaleByPowerOfTen(DecimalAt - Count);
			   }
			}
		}

		/// <summary>
		/// Return true if the number represented by this object can fit into
		/// a long. </summary>
		/// <param name="isPositive"> true if this number should be regarded as positive </param>
		/// <param name="ignoreNegativeZero"> true if -0 should be regarded as identical to
		/// +0; otherwise they are considered distinct </param>
		/// <returns> true if this number fits into a Java long </returns>
		internal bool FitsIntoLong(bool isPositive, bool ignoreNegativeZero)
		{
			// Figure out if the result will fit in a long.  We have to
			// first look for nonzero digits after the decimal point;
			// then check the size.  If the digit count is 18 or less, then
			// the value can definitely be represented as a long.  If it is 19
			// then it may be too large.

			// Trim trailing zeros.  This does not change the represented value.
			while (Count > 0 && Digits[Count - 1] == '0')
			{
				--Count;
			}

			if (Count == 0)
			{
				// Positive zero fits into a long, but negative zero can only
				// be represented as a double. - bug 4162852
				return isPositive || ignoreNegativeZero;
			}

			if (DecimalAt < Count || DecimalAt > MAX_COUNT)
			{
				return false;
			}

			if (DecimalAt < MAX_COUNT)
			{
				return true;
			}

			// At this point we have decimalAt == count, and count == MAX_COUNT.
			// The number will overflow if it is larger than 9223372036854775807
			// or smaller than -9223372036854775808.
			for (int i = 0; i < Count; ++i)
			{
				char dig = Digits[i], max = LONG_MIN_REP[i];
				if (dig > max)
				{
					return false;
				}
				if (dig < max)
				{
					return true;
				}
			}

			// At this point the first count digits match.  If decimalAt is less
			// than count, then the remaining digits are zero, and we return true.
			if (Count < DecimalAt)
			{
				return true;
			}

			// Now we have a representation of Long.MIN_VALUE, without the leading
			// negative sign.  If this represents a positive value, then it does
			// not fit; otherwise it fits.
			return !isPositive;
		}

		/// <summary>
		/// Set the digit list to a representation of the given double value.
		/// This method supports fixed-point notation. </summary>
		/// <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		/// <param name="source"> Value to be converted; must not be Inf, -Inf, Nan,
		/// or a value <= 0. </param>
		/// <param name="maximumFractionDigits"> The most fractional digits which should
		/// be converted. </param>
		internal void Set(bool isNegative, double source, int maximumFractionDigits)
		{
			Set(isNegative, source, maximumFractionDigits, true);
		}

		/// <summary>
		/// Set the digit list to a representation of the given double value.
		/// This method supports both fixed-point and exponential notation. </summary>
		/// <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		/// <param name="source"> Value to be converted; must not be Inf, -Inf, Nan,
		/// or a value <= 0. </param>
		/// <param name="maximumDigits"> The most fractional or total digits which should
		/// be converted. </param>
		/// <param name="fixedPoint"> If true, then maximumDigits is the maximum
		/// fractional digits to be converted.  If false, total digits. </param>
		internal void Set(bool isNegative, double source, int maximumDigits, bool fixedPoint)
		{

			FloatingDecimal.BinaryToASCIIConverter fdConverter = FloatingDecimal.getBinaryToASCIIConverter(source);
			bool hasBeenRoundedUp = fdConverter.digitsRoundedUp();
			bool valueExactAsDecimal = fdConverter.decimalDigitsExact();
			Debug.Assert(!fdConverter.Exceptional);
			String digitsString = fdConverter.toJavaFormatString();

			Set(isNegative, digitsString, hasBeenRoundedUp, valueExactAsDecimal, maximumDigits, fixedPoint);
		}

		/// <summary>
		/// Generate a representation of the form DDDDD, DDDDD.DDDDD, or
		/// DDDDDE+/-DDDDD. </summary>
		/// <param name="roundedUp"> whether or not rounding up has already happened. </param>
		/// <param name="valueExactAsDecimal"> whether or not collected digits provide
		/// an exact decimal representation of the value. </param>
		private void Set(bool isNegative, String s, bool roundedUp, bool valueExactAsDecimal, int maximumDigits, bool fixedPoint)
		{

			this.IsNegative = isNegative;
			int len = s.Length();
			char[] source = GetDataChars(len);
			s.GetChars(0, len, source, 0);

			DecimalAt = -1;
			Count = 0;
			int exponent = 0;
			// Number of zeros between decimal point and first non-zero digit after
			// decimal point, for numbers < 1.
			int leadingZerosAfterDecimal = 0;
			bool nonZeroDigitSeen = false;

			for (int i = 0; i < len;)
			{
				char c = source[i++];
				if (c == '.')
				{
					DecimalAt = Count;
				}
				else if (c == 'e' || c == 'E')
				{
					exponent = ParseInt(source, i, len);
					break;
				}
				else
				{
					if (!nonZeroDigitSeen)
					{
						nonZeroDigitSeen = (c != '0');
						if (!nonZeroDigitSeen && DecimalAt != -1)
						{
							++leadingZerosAfterDecimal;
						}
					}
					if (nonZeroDigitSeen)
					{
						Digits[Count++] = c;
					}
				}
			}
			if (DecimalAt == -1)
			{
				DecimalAt = Count;
			}
			if (nonZeroDigitSeen)
			{
				DecimalAt += exponent - leadingZerosAfterDecimal;
			}

			if (fixedPoint)
			{
				// The negative of the exponent represents the number of leading
				// zeros between the decimal and the first non-zero digit, for
				// a value < 0.1 (e.g., for 0.00123, -decimalAt == 2).  If this
				// is more than the maximum fraction digits, then we have an underflow
				// for the printed representation.
				if (-DecimalAt > maximumDigits)
				{
					// Handle an underflow to zero when we round something like
					// 0.0009 to 2 fractional digits.
					Count = 0;
					return;
				}
				else if (-DecimalAt == maximumDigits)
				{
					// If we round 0.0009 to 3 fractional digits, then we have to
					// create a new one digit in the least significant location.
					if (ShouldRoundUp(0, roundedUp, valueExactAsDecimal))
					{
						Count = 1;
						++DecimalAt;
						Digits[0] = '1';
					}
					else
					{
						Count = 0;
					}
					return;
				}
				// else fall through
			}

			// Eliminate trailing zeros.
			while (Count > 1 && Digits[Count - 1] == '0')
			{
				--Count;
			}

			// Eliminate digits beyond maximum digits to be displayed.
			// Round up if appropriate.
			Round(fixedPoint ? (maximumDigits + DecimalAt) : maximumDigits, roundedUp, valueExactAsDecimal);

		}

		/// <summary>
		/// Round the representation to the given number of digits. </summary>
		/// <param name="maximumDigits"> The maximum number of digits to be shown. </param>
		/// <param name="alreadyRounded"> whether or not rounding up has already happened. </param>
		/// <param name="valueExactAsDecimal"> whether or not collected digits provide
		/// an exact decimal representation of the value.
		/// 
		/// Upon return, count will be less than or equal to maximumDigits. </param>
		private void Round(int maximumDigits, bool alreadyRounded, bool valueExactAsDecimal)
		{
			// Eliminate digits beyond maximum digits to be displayed.
			// Round up if appropriate.
			if (maximumDigits >= 0 && maximumDigits < Count)
			{
				if (ShouldRoundUp(maximumDigits, alreadyRounded, valueExactAsDecimal))
				{
					// Rounding up involved incrementing digits from LSD to MSD.
					// In most cases this is simple, but in a worst case situation
					// (9999..99) we have to adjust the decimalAt value.
					for (;;)
					{
						--maximumDigits;
						if (maximumDigits < 0)
						{
							// We have all 9's, so we increment to a single digit
							// of one and adjust the exponent.
							Digits[0] = '1';
							++DecimalAt;
							maximumDigits = 0; // Adjust the count
							break;
						}

						++Digits[maximumDigits];
						if (Digits[maximumDigits] <= '9')
						{
							break;
						}
						// digits[maximumDigits] = '0'; // Unnecessary since we'll truncate this
					}
					++maximumDigits; // Increment for use as count
				}
				Count = maximumDigits;

				// Eliminate trailing zeros.
				while (Count > 1 && Digits[Count - 1] == '0')
				{
					--Count;
				}
			}
		}


		/// <summary>
		/// Return true if truncating the representation to the given number
		/// of digits will result in an increment to the last digit.  This
		/// method implements the rounding modes defined in the
		/// java.math.RoundingMode class.
		/// [bnf] </summary>
		/// <param name="maximumDigits"> the number of digits to keep, from 0 to
		/// <code>count-1</code>.  If 0, then all digits are rounded away, and
		/// this method returns true if a one should be generated (e.g., formatting
		/// 0.09 with "#.#"). </param>
		/// <param name="alreadyRounded"> whether or not rounding up has already happened. </param>
		/// <param name="valueExactAsDecimal"> whether or not collected digits provide
		/// an exact decimal representation of the value. </param>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///            mode being set to RoundingMode.UNNECESSARY </exception>
		/// <returns> true if digit <code>maximumDigits-1</code> should be
		/// incremented </returns>
		private bool ShouldRoundUp(int maximumDigits, bool alreadyRounded, bool valueExactAsDecimal)
		{
			if (maximumDigits < Count)
			{
				/*
				 * To avoid erroneous double-rounding or truncation when converting
				 * a binary double value to text, information about the exactness
				 * of the conversion result in FloatingDecimal, as well as any
				 * rounding done, is needed in this class.
				 *
				 * - For the  HALF_DOWN, HALF_EVEN, HALF_UP rounding rules below:
				 *   In the case of formating float or double, We must take into
				 *   account what FloatingDecimal has done in the binary to decimal
				 *   conversion.
				 *
				 *   Considering the tie cases, FloatingDecimal may round up the
				 *   value (returning decimal digits equal to tie when it is below),
				 *   or "truncate" the value to the tie while value is above it,
				 *   or provide the exact decimal digits when the binary value can be
				 *   converted exactly to its decimal representation given formating
				 *   rules of FloatingDecimal ( we have thus an exact decimal
				 *   representation of the binary value).
				 *
				 *   - If the double binary value was converted exactly as a decimal
				 *     value, then DigitList code must apply the expected rounding
				 *     rule.
				 *
				 *   - If FloatingDecimal already rounded up the decimal value,
				 *     DigitList should neither round up the value again in any of
				 *     the three rounding modes above.
				 *
				 *   - If FloatingDecimal has truncated the decimal value to
				 *     an ending '5' digit, DigitList should round up the value in
				 *     all of the three rounding modes above.
				 *
				 *
				 *   This has to be considered only if digit at maximumDigits index
				 *   is exactly the last one in the set of digits, otherwise there are
				 *   remaining digits after that position and we don't have to consider
				 *   what FloatingDecimal did.
				 *
				 * - Other rounding modes are not impacted by these tie cases.
				 *
				 * - For other numbers that are always converted to exact digits
				 *   (like BigInteger, Long, ...), the passed alreadyRounded boolean
				 *   have to be  set to false, and valueExactAsDecimal has to be set to
				 *   true in the upper DigitList call stack, providing the right state
				 *   for those situations..
				 */

				switch (RoundingMode_Renamed.InnerEnumValue())
				{
				case RoundingMode.InnerEnum.UP:
					for (int i = maximumDigits; i < Count; ++i)
					{
						if (Digits[i] != '0')
						{
							return true;
						}
					}
					break;
				case RoundingMode.InnerEnum.DOWN:
					break;
				case RoundingMode.InnerEnum.CEILING:
					for (int i = maximumDigits; i < Count; ++i)
					{
						if (Digits[i] != '0')
						{
							return !IsNegative;
						}
					}
					break;
				case RoundingMode.InnerEnum.FLOOR:
					for (int i = maximumDigits; i < Count; ++i)
					{
						if (Digits[i] != '0')
						{
							return IsNegative;
						}
					}
					break;
				case RoundingMode.InnerEnum.HALF_UP:
				case RoundingMode.InnerEnum.HALF_DOWN:
					if (Digits[maximumDigits] > '5')
					{
						// Value is above tie ==> must round up
						return true;
					}
					else if (Digits[maximumDigits] == '5')
					{
						// Digit at rounding position is a '5'. Tie cases.
						if (maximumDigits != (Count - 1))
						{
							// There are remaining digits. Above tie => must round up
							return true;
						}
						else
						{
							// Digit at rounding position is the last one !
							if (valueExactAsDecimal)
							{
								// Exact binary representation. On the tie.
								// Apply rounding given by roundingMode.
								return RoundingMode_Renamed == RoundingMode.HALF_UP;
							}
							else
							{
								// Not an exact binary representation.
								// Digit sequence either rounded up or truncated.
								// Round up only if it was truncated.
								return !alreadyRounded;
							}
						}
					}
					// Digit at rounding position is < '5' ==> no round up.
					// Just let do the default, which is no round up (thus break).
					break;
				case RoundingMode.InnerEnum.HALF_EVEN:
					// Implement IEEE half-even rounding
					if (Digits[maximumDigits] > '5')
					{
						return true;
					}
					else if (Digits[maximumDigits] == '5')
					{
						if (maximumDigits == (Count - 1))
						{
							// the rounding position is exactly the last index :
							if (alreadyRounded)
								// If FloatingDecimal rounded up (value was below tie),
								// then we should not round up again.
							{
								return false;
							}

							if (!valueExactAsDecimal)
								// Otherwise if the digits don't represent exact value,
								// value was above tie and FloatingDecimal truncated
								// digits to tie. We must round up.
							{
								return true;
							}
							else
							{
								// This is an exact tie value, and FloatingDecimal
								// provided all of the exact digits. We thus apply
								// HALF_EVEN rounding rule.
								return ((maximumDigits > 0) && (Digits[maximumDigits - 1] % 2 != 0));
							}
						}
						else
						{
							// Rounds up if it gives a non null digit after '5'
							for (int i = maximumDigits + 1; i < Count; ++i)
							{
								if (Digits[i] != '0')
								{
									return true;
								}
							}
						}
					}
					break;
				case RoundingMode.InnerEnum.UNNECESSARY:
					for (int i = maximumDigits; i < Count; ++i)
					{
						if (Digits[i] != '0')
						{
							throw new ArithmeticException("Rounding needed with the rounding mode being set to RoundingMode.UNNECESSARY");
						}
					}
					break;
				default:
					Debug.Assert(false);
				break;
				}
			}
			return false;
		}

		/// <summary>
		/// Utility routine to set the value of the digit list from a long
		/// </summary>
		internal void Set(bool isNegative, long source)
		{
			Set(isNegative, source, 0);
		}

		/// <summary>
		/// Set the digit list to a representation of the given long value. </summary>
		/// <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		/// <param name="source"> Value to be converted; must be >= 0 or ==
		/// Long.MIN_VALUE. </param>
		/// <param name="maximumDigits"> The most digits which should be converted.
		/// If maximumDigits is lower than the number of significant digits
		/// in source, the representation will be rounded.  Ignored if <= 0. </param>
		internal void Set(bool isNegative, long source, int maximumDigits)
		{
			this.IsNegative = isNegative;

			// This method does not expect a negative number. However,
			// "source" can be a Long.MIN_VALUE (-9223372036854775808),
			// if the number being formatted is a Long.MIN_VALUE.  In that
			// case, it will be formatted as -Long.MIN_VALUE, a number
			// which is outside the legal range of a long, but which can
			// be represented by DigitList.
			if (source <= 0)
			{
				if (source == Long.MinValue)
				{
					DecimalAt = Count = MAX_COUNT;
					System.Array.Copy(LONG_MIN_REP, 0, Digits, 0, Count);
				}
				else
				{
					DecimalAt = Count = 0; // Values <= 0 format as zero
				}
			}
			else
			{
				// Rewritten to improve performance.  I used to call
				// Long.toString(), which was about 4x slower than this code.
				int left = MAX_COUNT;
				int right;
				while (source > 0)
				{
					Digits[--left] = (char)('0' + (source % 10));
					source /= 10;
				}
				DecimalAt = MAX_COUNT - left;
				// Don't copy trailing zeros.  We are guaranteed that there is at
				// least one non-zero digit, so we don't have to check lower bounds.
				for (right = MAX_COUNT - 1; Digits[right] == '0'; --right)
				{
					;
				}
				Count = right - left + 1;
				System.Array.Copy(Digits, left, Digits, 0, Count);
			}
			if (maximumDigits > 0)
			{
				Round(maximumDigits, false, true);
			}
		}

		/// <summary>
		/// Set the digit list to a representation of the given BigDecimal value.
		/// This method supports both fixed-point and exponential notation. </summary>
		/// <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		/// <param name="source"> Value to be converted; must not be a value <= 0. </param>
		/// <param name="maximumDigits"> The most fractional or total digits which should
		/// be converted. </param>
		/// <param name="fixedPoint"> If true, then maximumDigits is the maximum
		/// fractional digits to be converted.  If false, total digits. </param>
		internal void Set(bool isNegative, decimal source, int maximumDigits, bool fixedPoint)
		{
			String s = source.ToString();
			ExtendDigits(s.Length());

			Set(isNegative, s, false, true, maximumDigits, fixedPoint);
		}

		/// <summary>
		/// Set the digit list to a representation of the given BigInteger value. </summary>
		/// <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		/// <param name="source"> Value to be converted; must be >= 0. </param>
		/// <param name="maximumDigits"> The most digits which should be converted.
		/// If maximumDigits is lower than the number of significant digits
		/// in source, the representation will be rounded.  Ignored if <= 0. </param>
		internal void Set(bool isNegative, System.Numerics.BigInteger source, int maximumDigits)
		{
			this.IsNegative = isNegative;
			String s = source.ToString();
			int len = s.Length();
			ExtendDigits(len);
			s.GetChars(0, len, Digits, 0);

			DecimalAt = len;
			int right;
			for (right = len - 1; right >= 0 && Digits[right] == '0'; --right)
			{
				;
			}
			Count = right + 1;

			if (maximumDigits > 0)
			{
				Round(maximumDigits, false, true);
			}
		}

		/// <summary>
		/// equality test between two digit lists.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (this == obj) // quick check
			{
				return true;
			}
			if (!(obj is DigitList)) // (1) same object?
			{
				return false;
			}
			DigitList other = (DigitList) obj;
			if (Count != other.Count || DecimalAt != other.DecimalAt)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				if (Digits[i] != other.Digits[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Generates the hash code for the digit list.
		/// </summary>
		public override int HashCode()
		{
			int hashcode = DecimalAt;

			for (int i = 0; i < Count; i++)
			{
				hashcode = hashcode * 37 + Digits[i];
			}

			return hashcode;
		}

		/// <summary>
		/// Creates a copy of this object. </summary>
		/// <returns> a clone of this instance. </returns>
		public Object Clone()
		{
			try
			{
				DigitList other = (DigitList) base.Clone();
				char[] newDigits = new char[Digits.Length];
				System.Array.Copy(Digits, 0, newDigits, 0, Digits.Length);
				other.Digits = newDigits;
				other.TempBuffer = null;
				return other;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Returns true if this DigitList represents Long.MIN_VALUE;
		/// false, otherwise.  This is required so that getLong() works.
		/// </summary>
		private bool LongMIN_VALUE
		{
			get
			{
				if (DecimalAt != Count || Count != MAX_COUNT)
				{
					return false;
				}
    
				for (int i = 0; i < Count; ++i)
				{
					if (Digits[i] != LONG_MIN_REP[i])
					{
						return false;
					}
				}
    
				return true;
			}
		}

		private static int ParseInt(char[] str, int offset, int strLen)
		{
			char c;
			bool positive = true;
			if ((c = str[offset]) == '-')
			{
				positive = false;
				offset++;
			}
			else if (c == '+')
			{
				offset++;
			}

			int value = 0;
			while (offset < strLen)
			{
				c = str[offset++];
				if (c >= '0' && c <= '9')
				{
					value = value * 10 + (c - '0');
				}
				else
				{
					break;
				}
			}
			return positive ? value : -value;
		}

		// The digit part of -9223372036854775808L
		private static readonly char[] LONG_MIN_REP = "9223372036854775808".ToCharArray();

		public override String ToString()
		{
			if (Zero)
			{
				return "0";
			}
			StringBuffer buf = StringBuffer;
			buf.Append("0.");
			buf.Append(Digits, 0, Count);
			buf.Append("x10^");
			buf.Append(DecimalAt);
			return buf.ToString();
		}

		private StringBuffer TempBuffer;

		private StringBuffer StringBuffer
		{
			get
			{
				if (TempBuffer == null)
				{
					TempBuffer = new StringBuffer(MAX_COUNT);
				}
				else
				{
					TempBuffer.Length = 0;
				}
				return TempBuffer;
			}
		}

		private void ExtendDigits(int len)
		{
			if (len > Digits.Length)
			{
				Digits = new char[len];
			}
		}

		private char[] GetDataChars(int length)
		{
			if (Data == null || Data.Length < length)
			{
				Data = new char[length];
			}
			return Data;
		}
	}

}