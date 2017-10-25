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
 * Portions Copyright (c) 1995  Colin Plumb.  All rights reserved.
 */

namespace java.math
{

	using DoubleConsts = sun.misc.DoubleConsts;
	using FloatConsts = sun.misc.FloatConsts;

	/// <summary>
	/// Immutable arbitrary-precision integers.  All operations behave as if
	/// BigIntegers were represented in two's-complement notation (like Java's
	/// primitive integer types).  BigInteger provides analogues to all of Java's
	/// primitive integer operators, and all relevant methods from java.lang.Math.
	/// Additionally, BigInteger provides operations for modular arithmetic, GCD
	/// calculation, primality testing, prime generation, bit manipulation,
	/// and a few other miscellaneous operations.
	/// 
	/// <para>Semantics of arithmetic operations exactly mimic those of Java's integer
	/// arithmetic operators, as defined in <i>The Java Language Specification</i>.
	/// For example, division by zero throws an {@code ArithmeticException}, and
	/// division of a negative by a positive yields a negative (or zero) remainder.
	/// All of the details in the Spec concerning overflow are ignored, as
	/// BigIntegers are made as large as necessary to accommodate the results of an
	/// operation.
	/// 
	/// </para>
	/// <para>Semantics of shift operations extend those of Java's shift operators
	/// to allow for negative shift distances.  A right-shift with a negative
	/// shift distance results in a left shift, and vice-versa.  The unsigned
	/// right shift operator ({@code >>>}) is omitted, as this operation makes
	/// little sense in combination with the "infinite word size" abstraction
	/// provided by this class.
	/// 
	/// </para>
	/// <para>Semantics of bitwise logical operations exactly mimic those of Java's
	/// bitwise integer operators.  The binary operators ({@code and},
	/// {@code or}, {@code xor}) implicitly perform sign extension on the shorter
	/// of the two operands prior to performing the operation.
	/// 
	/// </para>
	/// <para>Comparison operations perform signed integer comparisons, analogous to
	/// those performed by Java's relational and equality operators.
	/// 
	/// </para>
	/// <para>Modular arithmetic operations are provided to compute residues, perform
	/// exponentiation, and compute multiplicative inverses.  These methods always
	/// return a non-negative result, between {@code 0} and {@code (modulus - 1)},
	/// inclusive.
	/// 
	/// </para>
	/// <para>Bit operations operate on a single bit of the two's-complement
	/// representation of their operand.  If necessary, the operand is sign-
	/// extended so that it contains the designated bit.  None of the single-bit
	/// operations can produce a BigInteger with a different sign from the
	/// BigInteger being operated on, as they affect only a single bit, and the
	/// "infinite word size" abstraction provided by this class ensures that there
	/// are infinitely many "virtual sign bits" preceding each BigInteger.
	/// 
	/// </para>
	/// <para>For the sake of brevity and clarity, pseudo-code is used throughout the
	/// descriptions of BigInteger methods.  The pseudo-code expression
	/// {@code (i + j)} is shorthand for "a BigInteger whose value is
	/// that of the BigInteger {@code i} plus that of the BigInteger {@code j}."
	/// The pseudo-code expression {@code (i == j)} is shorthand for
	/// "{@code true} if and only if the BigInteger {@code i} represents the same
	/// value as the BigInteger {@code j}."  Other pseudo-code expressions are
	/// interpreted similarly.
	/// 
	/// </para>
	/// <para>All methods and constructors in this class throw
	/// {@code NullPointerException} when passed
	/// a null object reference for any input parameter.
	/// 
	/// BigInteger must support values in the range
	/// -2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive) to
	/// +2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive)
	/// and may support values outside of that range.
	/// 
	/// The range of probable prime values is limited and may be less than
	/// the full supported positive range of {@code BigInteger}.
	/// The range must be at least 1 to 2<sup>500000000</sup>.
	/// 
	/// @implNote
	/// BigInteger constructors and operations throw {@code ArithmeticException} when
	/// the result is out of the supported range of
	/// -2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive) to
	/// +2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive).
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     BigDecimal
	/// @author  Josh Bloch
	/// @author  Michael McCloskey
	/// @author  Alan Eliasen
	/// @author  Timothy Buktu
	/// @since JDK1.1 </seealso>

	public class BigInteger : Number, Comparable<BigInteger>
	{
		/// <summary>
		/// The signum of this BigInteger: -1 for negative, 0 for zero, or
		/// 1 for positive.  Note that the BigInteger zero <i>must</i> have
		/// a signum of 0.  This is necessary to ensures that there is exactly one
		/// representation for each BigInteger value.
		/// 
		/// @serial
		/// </summary>
		internal readonly int Signum_Renamed;

		/// <summary>
		/// The magnitude of this BigInteger, in <i>big-endian</i> order: the
		/// zeroth element of this array is the most-significant int of the
		/// magnitude.  The magnitude must be "minimal" in that the most-significant
		/// int ({@code mag[0]}) must be non-zero.  This is necessary to
		/// ensure that there is exactly one representation for each BigInteger
		/// value.  Note that this implies that the BigInteger zero has a
		/// zero-length mag array.
		/// </summary>
		internal readonly int[] Mag;

		// These "redundant fields" are initialized with recognizable nonsense
		// values, and cached the first time they are needed (or never, if they
		// aren't needed).

		 /// <summary>
		 /// One plus the bitCount of this BigInteger. Zeros means unitialized.
		 /// 
		 /// @serial </summary>
		 /// <seealso cref= #bitCount </seealso>
		 /// @deprecated Deprecated since logical value is offset from stored
		 /// value and correction factor is applied in accessor method. 
		[Obsolete("Deprecated since logical value is offset from stored")]
		private int BitCount_Renamed;

		/// <summary>
		/// One plus the bitLength of this BigInteger. Zeros means unitialized.
		/// (either value is acceptable).
		/// 
		/// @serial </summary>
		/// <seealso cref= #bitLength() </seealso>
		/// @deprecated Deprecated since logical value is offset from stored
		/// value and correction factor is applied in accessor method. 
		[Obsolete("Deprecated since logical value is offset from stored")]
		private int BitLength_Renamed;

		/// <summary>
		/// Two plus the lowest set bit of this BigInteger, as returned by
		/// getLowestSetBit().
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLowestSetBit </seealso>
		/// @deprecated Deprecated since logical value is offset from stored
		/// value and correction factor is applied in accessor method. 
		[Obsolete("Deprecated since logical value is offset from stored")]
		private int LowestSetBit_Renamed;

		/// <summary>
		/// Two plus the index of the lowest-order int in the magnitude of this
		/// BigInteger that contains a nonzero int, or -2 (either value is acceptable).
		/// The least significant int has int-number 0, the next int in order of
		/// increasing significance has int-number 1, and so forth. </summary>
		/// @deprecated Deprecated since logical value is offset from stored
		/// value and correction factor is applied in accessor method. 
		[Obsolete("Deprecated since logical value is offset from stored")]
		private int FirstNonzeroIntNum_Renamed;

		/// <summary>
		/// This mask is used to obtain the value of an int as if it were unsigned.
		/// </summary>
		internal const long LONG_MASK = 0xffffffffL;

		/// <summary>
		/// This constant limits {@code mag.length} of BigIntegers to the supported
		/// range.
		/// </summary>
		private static readonly int MAX_MAG_LENGTH = Integer.MaxValue / sizeof(int) + 1; // (1 << 26)

		/// <summary>
		/// Bit lengths larger than this constant can cause overflow in searchLen
		/// calculation and in BitSieve.singleSearch method.
		/// </summary>
		private const int PRIME_SEARCH_BIT_LENGTH_LIMIT = 500000000;

		/// <summary>
		/// The threshold value for using Karatsuba multiplication.  If the number
		/// of ints in both mag arrays are greater than this number, then
		/// Karatsuba multiplication will be used.   This value is found
		/// experimentally to work well.
		/// </summary>
		private const int KARATSUBA_THRESHOLD = 80;

		/// <summary>
		/// The threshold value for using 3-way Toom-Cook multiplication.
		/// If the number of ints in each mag array is greater than the
		/// Karatsuba threshold, and the number of ints in at least one of
		/// the mag arrays is greater than this threshold, then Toom-Cook
		/// multiplication will be used.
		/// </summary>
		private const int TOOM_COOK_THRESHOLD = 240;

		/// <summary>
		/// The threshold value for using Karatsuba squaring.  If the number
		/// of ints in the number are larger than this value,
		/// Karatsuba squaring will be used.   This value is found
		/// experimentally to work well.
		/// </summary>
		private const int KARATSUBA_SQUARE_THRESHOLD = 128;

		/// <summary>
		/// The threshold value for using Toom-Cook squaring.  If the number
		/// of ints in the number are larger than this value,
		/// Toom-Cook squaring will be used.   This value is found
		/// experimentally to work well.
		/// </summary>
		private const int TOOM_COOK_SQUARE_THRESHOLD = 216;

		/// <summary>
		/// The threshold value for using Burnikel-Ziegler division.  If the number
		/// of ints in the divisor are larger than this value, Burnikel-Ziegler
		/// division may be used.  This value is found experimentally to work well.
		/// </summary>
		internal const int BURNIKEL_ZIEGLER_THRESHOLD = 80;

		/// <summary>
		/// The offset value for using Burnikel-Ziegler division.  If the number
		/// of ints in the divisor exceeds the Burnikel-Ziegler threshold, and the
		/// number of ints in the dividend is greater than the number of ints in the
		/// divisor plus this value, Burnikel-Ziegler division will be used.  This
		/// value is found experimentally to work well.
		/// </summary>
		internal const int BURNIKEL_ZIEGLER_OFFSET = 40;

		/// <summary>
		/// The threshold value for using Schoenhage recursive base conversion. If
		/// the number of ints in the number are larger than this value,
		/// the Schoenhage algorithm will be used.  In practice, it appears that the
		/// Schoenhage routine is faster for any threshold down to 2, and is
		/// relatively flat for thresholds between 2-25, so this choice may be
		/// varied within this range for very small effect.
		/// </summary>
		private const int SCHOENHAGE_BASE_CONVERSION_THRESHOLD = 20;

		/// <summary>
		/// The threshold value for using squaring code to perform multiplication
		/// of a {@code BigInteger} instance by itself.  If the number of ints in
		/// the number are larger than this value, {@code multiply(this)} will
		/// return {@code square()}.
		/// </summary>
		private const int MULTIPLY_SQUARE_THRESHOLD = 20;

		// Constructors

		/// <summary>
		/// Translates a byte array containing the two's-complement binary
		/// representation of a BigInteger into a BigInteger.  The input array is
		/// assumed to be in <i>big-endian</i> byte-order: the most significant
		/// byte is in the zeroth element.
		/// </summary>
		/// <param name="val"> big-endian two's-complement binary representation of
		///         BigInteger. </param>
		/// <exception cref="NumberFormatException"> {@code val} is zero bytes long. </exception>
		public BigInteger(sbyte[] val)
		{
			if (val.Length == 0)
			{
				throw new NumberFormatException("Zero length BigInteger");
			}

			if (val[0] < 0)
			{
				Mag = MakePositive(val);
				Signum_Renamed = -1;
			}
			else
			{
				Mag = StripLeadingZeroBytes(val);
				Signum_Renamed = (Mag.Length == 0 ? 0 : 1);
			}
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// This private constructor translates an int array containing the
		/// two's-complement binary representation of a BigInteger into a
		/// BigInteger. The input array is assumed to be in <i>big-endian</i>
		/// int-order: the most significant int is in the zeroth element.
		/// </summary>
		private BigInteger(int[] val)
		{
			if (val.Length == 0)
			{
				throw new NumberFormatException("Zero length BigInteger");
			}

			if (val[0] < 0)
			{
				Mag = MakePositive(val);
				Signum_Renamed = -1;
			}
			else
			{
				Mag = TrustedStripLeadingZeroInts(val);
				Signum_Renamed = (Mag.Length == 0 ? 0 : 1);
			}
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// Translates the sign-magnitude representation of a BigInteger into a
		/// BigInteger.  The sign is represented as an integer signum value: -1 for
		/// negative, 0 for zero, or 1 for positive.  The magnitude is a byte array
		/// in <i>big-endian</i> byte-order: the most significant byte is in the
		/// zeroth element.  A zero-length magnitude array is permissible, and will
		/// result in a BigInteger value of 0, whether signum is -1, 0 or 1.
		/// </summary>
		/// <param name="signum"> signum of the number (-1 for negative, 0 for zero, 1
		///         for positive). </param>
		/// <param name="magnitude"> big-endian binary representation of the magnitude of
		///         the number. </param>
		/// <exception cref="NumberFormatException"> {@code signum} is not one of the three
		///         legal values (-1, 0, and 1), or {@code signum} is 0 and
		///         {@code magnitude} contains one or more non-zero bytes. </exception>
		public BigInteger(int signum, sbyte[] magnitude)
		{
			this.Mag = StripLeadingZeroBytes(magnitude);

			if (signum < -1 || signum > 1)
			{
				throw (new NumberFormatException("Invalid signum value"));
			}

			if (this.Mag.Length == 0)
			{
				this.Signum_Renamed = 0;
			}
			else
			{
				if (signum == 0)
				{
					throw (new NumberFormatException("signum-magnitude mismatch"));
				}
				this.Signum_Renamed = signum;
			}
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// A constructor for internal use that translates the sign-magnitude
		/// representation of a BigInteger into a BigInteger. It checks the
		/// arguments and copies the magnitude so this constructor would be
		/// safe for external use.
		/// </summary>
		private BigInteger(int signum, int[] magnitude)
		{
			this.Mag = StripLeadingZeroInts(magnitude);

			if (signum < -1 || signum > 1)
			{
				throw (new NumberFormatException("Invalid signum value"));
			}

			if (this.Mag.Length == 0)
			{
				this.Signum_Renamed = 0;
			}
			else
			{
				if (signum == 0)
				{
					throw (new NumberFormatException("signum-magnitude mismatch"));
				}
				this.Signum_Renamed = signum;
			}
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// Translates the String representation of a BigInteger in the
		/// specified radix into a BigInteger.  The String representation
		/// consists of an optional minus or plus sign followed by a
		/// sequence of one or more digits in the specified radix.  The
		/// character-to-digit mapping is provided by {@code
		/// Character.digit}.  The String may not contain any extraneous
		/// characters (whitespace, for example).
		/// </summary>
		/// <param name="val"> String representation of BigInteger. </param>
		/// <param name="radix"> radix to be used in interpreting {@code val}. </param>
		/// <exception cref="NumberFormatException"> {@code val} is not a valid representation
		///         of a BigInteger in the specified radix, or {@code radix} is
		///         outside the range from <seealso cref="Character#MIN_RADIX"/> to
		///         <seealso cref="Character#MAX_RADIX"/>, inclusive. </exception>
		/// <seealso cref=    Character#digit </seealso>
		public BigInteger(String val, int radix)
		{
			int cursor = 0, numDigits ;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = val.length();
			int len = val.Length();

			if (radix < Character.MIN_RADIX || radix > Character.MAX_RADIX)
			{
				throw new NumberFormatException("Radix out of range");
			}
			if (len == 0)
			{
				throw new NumberFormatException("Zero length BigInteger");
			}

			// Check for at most one leading sign
			int sign = 1;
			int index1 = val.LastIndexOf('-');
			int index2 = val.LastIndexOf('+');
			if (index1 >= 0)
			{
				if (index1 != 0 || index2 >= 0)
				{
					throw new NumberFormatException("Illegal embedded sign character");
				}
				sign = -1;
				cursor = 1;
			}
			else if (index2 >= 0)
			{
				if (index2 != 0)
				{
					throw new NumberFormatException("Illegal embedded sign character");
				}
				cursor = 1;
			}
			if (cursor == len)
			{
				throw new NumberFormatException("Zero length BigInteger");
			}

			// Skip leading zeros and compute number of digits in magnitude
			while (cursor < len && Character.Digit(val.CharAt(cursor), radix) == 0)
			{
				cursor++;
			}

			if (cursor == len)
			{
				Signum_Renamed = 0;
				Mag = ZERO.Mag;
				return;
			}

			numDigits = len - cursor;
			Signum_Renamed = sign;

			// Pre-allocate array of expected size. May be too large but can
			// never be too small. Typically exact.
			long numBits = ((int)((uint)(numDigits * BitsPerDigit[radix]) >> 10)) + 1;
			if (numBits + 31 >= (1L << 32))
			{
				ReportOverflow();
			}
			int numWords = (int)((uint)(int)(numBits + 31) >> 5);
			int[] magnitude = new int[numWords];

			// Process first (potentially short) digit group
			int firstGroupLen = numDigits % DigitsPerInt[radix];
			if (firstGroupLen == 0)
			{
				firstGroupLen = DigitsPerInt[radix];
			}
			String group = val.Substring(cursor, cursor += firstGroupLen - cursor);
			magnitude[numWords - 1] = Convert.ToInt32(group, radix);
			if (magnitude[numWords - 1] < 0)
			{
				throw new NumberFormatException("Illegal digit");
			}

			// Process remaining digit groups
			int superRadix = IntRadix[radix];
			int groupVal = 0;
			while (cursor < len)
			{
				group = val.Substring(cursor, cursor += DigitsPerInt[radix] - cursor);
				groupVal = Convert.ToInt32(group, radix);
				if (groupVal < 0)
				{
					throw new NumberFormatException("Illegal digit");
				}
				DestructiveMulAdd(magnitude, superRadix, groupVal);
			}
			// Required for cases where the array was overallocated.
			Mag = TrustedStripLeadingZeroInts(magnitude);
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/*
		 * Constructs a new BigInteger using a char array with radix=10.
		 * Sign is precalculated outside and not allowed in the val.
		 */
		internal BigInteger(char[] val, int sign, int len)
		{
			int cursor = 0, numDigits ;

			// Skip leading zeros and compute number of digits in magnitude
			while (cursor < len && Character.Digit(val[cursor], 10) == 0)
			{
				cursor++;
			}
			if (cursor == len)
			{
				Signum_Renamed = 0;
				Mag = ZERO.Mag;
				return;
			}

			numDigits = len - cursor;
			Signum_Renamed = sign;
			// Pre-allocate array of expected size
			int numWords;
			if (len < 10)
			{
				numWords = 1;
			}
			else
			{
				long numBits = ((int)((uint)(numDigits * BitsPerDigit[10]) >> 10)) + 1;
				if (numBits + 31 >= (1L << 32))
				{
					ReportOverflow();
				}
				numWords = (int)((uint)(int)(numBits + 31) >> 5);
			}
			int[] magnitude = new int[numWords];

			// Process first (potentially short) digit group
			int firstGroupLen = numDigits % DigitsPerInt[10];
			if (firstGroupLen == 0)
			{
				firstGroupLen = DigitsPerInt[10];
			}
			magnitude[numWords - 1] = ParseInt(val, cursor, cursor += firstGroupLen);

			// Process remaining digit groups
			while (cursor < len)
			{
				int groupVal = ParseInt(val, cursor, cursor += DigitsPerInt[10]);
				DestructiveMulAdd(magnitude, IntRadix[10], groupVal);
			}
			Mag = TrustedStripLeadingZeroInts(magnitude);
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		// Create an integer with the digits between the two indexes
		// Assumes start < end. The result may be negative, but it
		// is to be treated as an unsigned value.
		private int ParseInt(char[] source, int start, int end)
		{
			int result = Character.Digit(source[start++], 10);
			if (result == -1)
			{
				throw new NumberFormatException(new String(source));
			}

			for (int index = start; index < end; index++)
			{
				int nextVal = Character.Digit(source[index], 10);
				if (nextVal == -1)
				{
					throw new NumberFormatException(new String(source));
				}
				result = 10 * result + nextVal;
			}

			return result;
		}

		// bitsPerDigit in the given radix times 1024
		// Rounded up to avoid underallocation.
		private static long[] BitsPerDigit = new long[] {0, 0, 1024, 1624, 2048, 2378, 2648, 2875, 3072, 3247, 3402, 3543, 3672, 3790, 3899, 4001, 4096, 4186, 4271, 4350, 4426, 4498, 4567, 4633, 4696, 4756, 4814, 4870, 4923, 4975, 5025, 5074, 5120, 5166, 5210, 5253, 5295};

		// Multiply x array times word y in place, and add word z
		private static void DestructiveMulAdd(int[] x, int y, int z)
		{
			// Perform the multiplication word by word
			long ylong = y & LONG_MASK;
			long zlong = z & LONG_MASK;
			int len = x.Length;

			long product = 0;
			long carry = 0;
			for (int i = len - 1; i >= 0; i--)
			{
				product = ylong * (x[i] & LONG_MASK) + carry;
				x[i] = (int)product;
				carry = (long)((ulong)product >> 32);
			}

			// Perform the addition
			long sum = (x[len - 1] & LONG_MASK) + zlong;
			x[len - 1] = (int)sum;
			carry = (long)((ulong)sum >> 32);
			for (int i = len - 2; i >= 0; i--)
			{
				sum = (x[i] & LONG_MASK) + carry;
				x[i] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}
		}

		/// <summary>
		/// Translates the decimal String representation of a BigInteger into a
		/// BigInteger.  The String representation consists of an optional minus
		/// sign followed by a sequence of one or more decimal digits.  The
		/// character-to-digit mapping is provided by {@code Character.digit}.
		/// The String may not contain any extraneous characters (whitespace, for
		/// example).
		/// </summary>
		/// <param name="val"> decimal String representation of BigInteger. </param>
		/// <exception cref="NumberFormatException"> {@code val} is not a valid representation
		///         of a BigInteger. </exception>
		/// <seealso cref=    Character#digit </seealso>
		public BigInteger(String val) : this(val, 10)
		{
		}

		/// <summary>
		/// Constructs a randomly generated BigInteger, uniformly distributed over
		/// the range 0 to (2<sup>{@code numBits}</sup> - 1), inclusive.
		/// The uniformity of the distribution assumes that a fair source of random
		/// bits is provided in {@code rnd}.  Note that this constructor always
		/// constructs a non-negative BigInteger.
		/// </summary>
		/// <param name="numBits"> maximum bitLength of the new BigInteger. </param>
		/// <param name="rnd"> source of randomness to be used in computing the new
		///         BigInteger. </param>
		/// <exception cref="IllegalArgumentException"> {@code numBits} is negative. </exception>
		/// <seealso cref= #bitLength() </seealso>
		public BigInteger(int numBits, Random rnd) : this(1, RandomBits(numBits, rnd))
		{
		}

		private static sbyte[] RandomBits(int numBits, Random rnd)
		{
			if (numBits < 0)
			{
				throw new IllegalArgumentException("numBits must be non-negative");
			}
			int numBytes = (int)(((long)numBits + 7) / 8); // avoid overflow
			sbyte[] randomBits = new sbyte[numBytes];

			// Generate random bytes and mask out any excess bits
			if (numBytes > 0)
			{
				rnd.nextBytes(randomBits);
				int excessBits = 8 * numBytes - numBits;
				randomBits[0] &= (sbyte)((1 << (8 - excessBits)) - 1);
			}
			return randomBits;
		}

		/// <summary>
		/// Constructs a randomly generated positive BigInteger that is probably
		/// prime, with the specified bitLength.
		/// 
		/// <para>It is recommended that the <seealso cref="#probablePrime probablePrime"/>
		/// method be used in preference to this constructor unless there
		/// is a compelling need to specify a certainty.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bitLength"> bitLength of the returned BigInteger. </param>
		/// <param name="certainty"> a measure of the uncertainty that the caller is
		///         willing to tolerate.  The probability that the new BigInteger
		///         represents a prime number will exceed
		///         (1 - 1/2<sup>{@code certainty}</sup>).  The execution time of
		///         this constructor is proportional to the value of this parameter. </param>
		/// <param name="rnd"> source of random bits used to select candidates to be
		///         tested for primality. </param>
		/// <exception cref="ArithmeticException"> {@code bitLength < 2} or {@code bitLength} is too large. </exception>
		/// <seealso cref=    #bitLength() </seealso>
		public BigInteger(int bitLength, int certainty, Random rnd)
		{
			BigInteger prime;

			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}
			prime = (bitLength < SMALL_PRIME_THRESHOLD ? SmallPrime(bitLength, certainty, rnd) : LargePrime(bitLength, certainty, rnd));
			Signum_Renamed = 1;
			Mag = prime.Mag;
		}

		// Minimum size in bits that the requested prime number has
		// before we use the large prime number generating algorithms.
		// The cutoff of 95 was chosen empirically for best performance.
		private const int SMALL_PRIME_THRESHOLD = 95;

		// Certainty required to meet the spec of probablePrime
		private const int DEFAULT_PRIME_CERTAINTY = 100;

		/// <summary>
		/// Returns a positive BigInteger that is probably prime, with the
		/// specified bitLength. The probability that a BigInteger returned
		/// by this method is composite does not exceed 2<sup>-100</sup>.
		/// </summary>
		/// <param name="bitLength"> bitLength of the returned BigInteger. </param>
		/// <param name="rnd"> source of random bits used to select candidates to be
		///         tested for primality. </param>
		/// <returns> a BigInteger of {@code bitLength} bits that is probably prime </returns>
		/// <exception cref="ArithmeticException"> {@code bitLength < 2} or {@code bitLength} is too large. </exception>
		/// <seealso cref=    #bitLength()
		/// @since 1.4 </seealso>
		public static BigInteger ProbablePrime(int bitLength, Random rnd)
		{
			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}

			return (bitLength < SMALL_PRIME_THRESHOLD ? SmallPrime(bitLength, DEFAULT_PRIME_CERTAINTY, rnd) : LargePrime(bitLength, DEFAULT_PRIME_CERTAINTY, rnd));
		}

		/// <summary>
		/// Find a random number of the specified bitLength that is probably prime.
		/// This method is used for smaller primes, its performance degrades on
		/// larger bitlengths.
		/// 
		/// This method assumes bitLength > 1.
		/// </summary>
		private static BigInteger SmallPrime(int bitLength, int certainty, Random rnd)
		{
			int magLen = (int)((uint)(bitLength + 31) >> 5);
			int[] temp = new int[magLen];
			int highBit = 1 << ((bitLength + 31) & 0x1f); // High bit of high int
			int highMask = (highBit << 1) - 1; // Bits to keep in high int

			while (true)
			{
				// Construct a candidate
				for (int i = 0; i < magLen; i++)
				{
					temp[i] = rnd.Next();
				}
				temp[0] = (temp[0] & highMask) | highBit; // Ensure exact length
				if (bitLength > 2)
				{
					temp[magLen - 1] |= 1; // Make odd if bitlen > 2
				}

				BigInteger p = new BigInteger(temp, 1);

				// Do cheap "pre-test" if applicable
				if (bitLength > 6)
				{
					long r = p.Remainder(SMALL_PRIME_PRODUCT).LongValue();
					if ((r % 3 == 0) || (r % 5 == 0) || (r % 7 == 0) || (r % 11 == 0) || (r % 13 == 0) || (r % 17 == 0) || (r % 19 == 0) || (r % 23 == 0) || (r % 29 == 0) || (r % 31 == 0) || (r % 37 == 0) || (r % 41 == 0))
					{
						continue; // Candidate is composite; try another
					}
				}

				// All candidates of bitLength 2 and 3 are prime by this point
				if (bitLength < 4)
				{
					return p;
				}

				// Do expensive test if we survive pre-test (or it's inapplicable)
				if (p.PrimeToCertainty(certainty, rnd))
				{
					return p;
				}
			}
		}

		private static readonly BigInteger SMALL_PRIME_PRODUCT = ValueOf(3L * 5 * 7 * 11 * 13 * 17 * 19 * 23 * 29 * 31 * 37 * 41);

		/// <summary>
		/// Find a random number of the specified bitLength that is probably prime.
		/// This method is more appropriate for larger bitlengths since it uses
		/// a sieve to eliminate most composites before using a more expensive
		/// test.
		/// </summary>
		private static BigInteger LargePrime(int bitLength, int certainty, Random rnd)
		{
			BigInteger p;
			p = (new BigInteger(bitLength, rnd)).setBit(bitLength - 1);
			p.Mag[p.Mag.Length - 1] &= unchecked((int)0xfffffffe);

			// Use a sieve length likely to contain the next prime number
			int searchLen = GetPrimeSearchLen(bitLength);
			BitSieve searchSieve = new BitSieve(p, searchLen);
			BigInteger candidate = searchSieve.Retrieve(p, certainty, rnd);

			while ((candidate == null) || (candidate.BitLength() != bitLength))
			{
				p = p.Add(BigInteger.ValueOf(2 * searchLen));
				if (p.BitLength() != bitLength)
				{
					p = (new BigInteger(bitLength, rnd)).setBit(bitLength - 1);
				}
				p.Mag[p.Mag.Length - 1] &= unchecked((int)0xfffffffe);
				searchSieve = new BitSieve(p, searchLen);
				candidate = searchSieve.Retrieve(p, certainty, rnd);
			}
			return candidate;
		}

	   /// <summary>
	   /// Returns the first integer greater than this {@code BigInteger} that
	   /// is probably prime.  The probability that the number returned by this
	   /// method is composite does not exceed 2<sup>-100</sup>. This method will
	   /// never skip over a prime when searching: if it returns {@code p}, there
	   /// is no prime {@code q} such that {@code this < q < p}.
	   /// </summary>
	   /// <returns> the first integer greater than this {@code BigInteger} that
	   ///         is probably prime. </returns>
	   /// <exception cref="ArithmeticException"> {@code this < 0} or {@code this} is too large.
	   /// @since 1.5 </exception>
		public virtual BigInteger NextProbablePrime()
		{
			if (this.Signum_Renamed < 0)
			{
				throw new ArithmeticException("start < 0: " + this);
			}

			// Handle trivial cases
			if ((this.Signum_Renamed == 0) || this.Equals(ONE))
			{
				return TWO;
			}

			BigInteger result = this.Add(ONE);

			// Fastpath for small numbers
			if (result.BitLength() < SMALL_PRIME_THRESHOLD)
			{

				// Ensure an odd number
				if (!result.TestBit(0))
				{
					result = result.Add(ONE);
				}

				while (true)
				{
					// Do cheap "pre-test" if applicable
					if (result.BitLength() > 6)
					{
						long r = result.Remainder(SMALL_PRIME_PRODUCT).LongValue();
						if ((r % 3 == 0) || (r % 5 == 0) || (r % 7 == 0) || (r % 11 == 0) || (r % 13 == 0) || (r % 17 == 0) || (r % 19 == 0) || (r % 23 == 0) || (r % 29 == 0) || (r % 31 == 0) || (r % 37 == 0) || (r % 41 == 0))
						{
							result = result.Add(TWO);
							continue; // Candidate is composite; try another
						}
					}

					// All candidates of bitLength 2 and 3 are prime by this point
					if (result.BitLength() < 4)
					{
						return result;
					}

					// The expensive test
					if (result.PrimeToCertainty(DEFAULT_PRIME_CERTAINTY, null))
					{
						return result;
					}

					result = result.Add(TWO);
				}
			}

			// Start at previous even number
			if (result.TestBit(0))
			{
				result = result.Subtract(ONE);
			}

			// Looking for the next large prime
			int searchLen = GetPrimeSearchLen(result.BitLength());

			while (true)
			{
			   BitSieve searchSieve = new BitSieve(result, searchLen);
			   BigInteger candidate = searchSieve.Retrieve(result, DEFAULT_PRIME_CERTAINTY, null);
			   if (candidate != null)
			   {
				   return candidate;
			   }
			   result = result.Add(BigInteger.ValueOf(2 * searchLen));
			}
		}

		private static int GetPrimeSearchLen(int bitLength)
		{
			if (bitLength > PRIME_SEARCH_BIT_LENGTH_LIMIT + 1)
			{
				throw new ArithmeticException("Prime search implementation restriction on bitLength");
			}
			return bitLength / 20 * 64;
		}

		/// <summary>
		/// Returns {@code true} if this BigInteger is probably prime,
		/// {@code false} if it's definitely composite.
		/// 
		/// This method assumes bitLength > 2.
		/// </summary>
		/// <param name="certainty"> a measure of the uncertainty that the caller is
		///         willing to tolerate: if the call returns {@code true}
		///         the probability that this BigInteger is prime exceeds
		///         {@code (1 - 1/2<sup>certainty</sup>)}.  The execution time of
		///         this method is proportional to the value of this parameter. </param>
		/// <returns> {@code true} if this BigInteger is probably prime,
		///         {@code false} if it's definitely composite. </returns>
		internal virtual bool PrimeToCertainty(int certainty, Random random)
		{
			int rounds = 0;
			int n = (System.Math.Min(certainty, Integer.MaxValue-1) + 1) / 2;

			// The relationship between the certainty and the number of rounds
			// we perform is given in the draft standard ANSI X9.80, "PRIME
			// NUMBER GENERATION, PRIMALITY TESTING, AND PRIMALITY CERTIFICATES".
			int sizeInBits = this.BitLength();
			if (sizeInBits < 100)
			{
				rounds = 50;
				rounds = n < rounds ? n : rounds;
				return PassesMillerRabin(rounds, random);
			}

			if (sizeInBits < 256)
			{
				rounds = 27;
			}
			else if (sizeInBits < 512)
			{
				rounds = 15;
			}
			else if (sizeInBits < 768)
			{
				rounds = 8;
			}
			else if (sizeInBits < 1024)
			{
				rounds = 4;
			}
			else
			{
				rounds = 2;
			}
			rounds = n < rounds ? n : rounds;

			return PassesMillerRabin(rounds, random) && PassesLucasLehmer();
		}

		/// <summary>
		/// Returns true iff this BigInteger is a Lucas-Lehmer probable prime.
		/// 
		/// The following assumptions are made:
		/// This BigInteger is a positive, odd number.
		/// </summary>
		private bool PassesLucasLehmer()
		{
			BigInteger thisPlusOne = this.Add(ONE);

			// Step 1
			int d = 5;
			while (JacobiSymbol(d, this) != -1)
			{
				// 5, -7, 9, -11, ...
				d = (d < 0) ? System.Math.Abs(d) + 2 : -(d + 2);
			}

			// Step 2
			BigInteger u = LucasLehmerSequence(d, thisPlusOne, this);

			// Step 3
			return u.Mod(this).Equals(ZERO);
		}

		/// <summary>
		/// Computes Jacobi(p,n).
		/// Assumes n positive, odd, n>=3.
		/// </summary>
		private static int JacobiSymbol(int p, BigInteger n)
		{
			if (p == 0)
			{
				return 0;
			}

			// Algorithm and comments adapted from Colin Plumb's C library.
			int j = 1;
			int u = n.Mag[n.Mag.Length - 1];

			// Make p positive
			if (p < 0)
			{
				p = -p;
				int n8 = u & 7;
				if ((n8 == 3) || (n8 == 7))
				{
					j = -j; // 3 (011) or 7 (111) mod 8
				}
			}

			// Get rid of factors of 2 in p
			while ((p & 3) == 0)
			{
				p >>= 2;
			}
			if ((p & 1) == 0)
			{
				p >>= 1;
				if (((u ^ (u >> 1)) & 2) != 0)
				{
					j = -j; // 3 (011) or 5 (101) mod 8
				}
			}
			if (p == 1)
			{
				return j;
			}
			// Then, apply quadratic reciprocity
			if ((p & u & 2) != 0) // p = u = 3 (mod 4)?
			{
				j = -j;
			}
			// And reduce u mod p
			u = n.Mod(BigInteger.ValueOf(p)).IntValue();

			// Now compute Jacobi(u,p), u < p
			while (u != 0)
			{
				while ((u & 3) == 0)
				{
					u >>= 2;
				}
				if ((u & 1) == 0)
				{
					u >>= 1;
					if (((p ^ (p >> 1)) & 2) != 0)
					{
						j = -j; // 3 (011) or 5 (101) mod 8
					}
				}
				if (u == 1)
				{
					return j;
				}
				// Now both u and p are odd, so use quadratic reciprocity
				assert(u < p);
				int t = u;
				u = p;
				p = t;
				if ((u & p & 2) != 0) // u = p = 3 (mod 4)?
				{
					j = -j;
				}
				// Now u >= p, so it can be reduced
				u %= p;
			}
			return 0;
		}

		private static BigInteger LucasLehmerSequence(int z, BigInteger k, BigInteger n)
		{
			BigInteger d = BigInteger.ValueOf(z);
			BigInteger u = ONE;
			BigInteger u2;
			BigInteger v = ONE;
			BigInteger v2;

			for (int i = k.BitLength() - 2; i >= 0; i--)
			{
				u2 = u.Multiply(v).Mod(n);

				v2 = v.Square().Add(d.Multiply(u.Square())).Mod(n);
				if (v2.TestBit(0))
				{
					v2 = v2.Subtract(n);
				}

				v2 = v2.ShiftRight(1);

				u = u2;
				v = v2;
				if (k.TestBit(i))
				{
					u2 = u.Add(v).Mod(n);
					if (u2.TestBit(0))
					{
						u2 = u2.Subtract(n);
					}

					u2 = u2.ShiftRight(1);
					v2 = v.Add(d.Multiply(u)).Mod(n);
					if (v2.TestBit(0))
					{
						v2 = v2.Subtract(n);
					}
					v2 = v2.ShiftRight(1);

					u = u2;
					v = v2;
				}
			}
			return u;
		}

		/// <summary>
		/// Returns true iff this BigInteger passes the specified number of
		/// Miller-Rabin tests. This test is taken from the DSA spec (NIST FIPS
		/// 186-2).
		/// 
		/// The following assumptions are made:
		/// This BigInteger is a positive, odd number greater than 2.
		/// iterations<=50.
		/// </summary>
		private bool PassesMillerRabin(int iterations, Random rnd)
		{
			// Find a and m such that m is odd and this == 1 + 2**a * m
			BigInteger thisMinusOne = this.Subtract(ONE);
			BigInteger m = thisMinusOne;
			int a = m.LowestSetBit;
			m = m.ShiftRight(a);

			// Do the tests
			if (rnd == null)
			{
				rnd = ThreadLocalRandom.Current();
			}
			for (int i = 0; i < iterations; i++)
			{
				// Generate a uniform random on (1, this)
				BigInteger b;
				do
				{
					b = new BigInteger(this.BitLength(), rnd);
				} while (b.CompareTo(ONE) <= 0 || b.CompareTo(this) >= 0);

				int j = 0;
				BigInteger z = b.ModPow(m, this);
				while (!((j == 0 && z.Equals(ONE)) || z.Equals(thisMinusOne)))
				{
					if (j > 0 && z.Equals(ONE) || ++j == a)
					{
						return false;
					}
					z = z.ModPow(TWO, this);
				}
			}
			return true;
		}

		/// <summary>
		/// This internal constructor differs from its public cousin
		/// with the arguments reversed in two ways: it assumes that its
		/// arguments are correct, and it doesn't copy the magnitude array.
		/// </summary>
		internal BigInteger(int[] magnitude, int signum)
		{
			this.Signum_Renamed = (magnitude.Length == 0 ? 0 : signum);
			this.Mag = magnitude;
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// This private constructor is for internal use and assumes that its
		/// arguments are correct.
		/// </summary>
		private BigInteger(sbyte[] magnitude, int signum)
		{
			this.Signum_Renamed = (magnitude.Length == 0 ? 0 : signum);
			this.Mag = StripLeadingZeroBytes(magnitude);
			if (Mag.Length >= MAX_MAG_LENGTH)
			{
				CheckRange();
			}
		}

		/// <summary>
		/// Throws an {@code ArithmeticException} if the {@code BigInteger} would be
		/// out of the supported range.
		/// </summary>
		/// <exception cref="ArithmeticException"> if {@code this} exceeds the supported range. </exception>
		private void CheckRange()
		{
			if (Mag.Length > MAX_MAG_LENGTH || Mag.Length == MAX_MAG_LENGTH && Mag[0] < 0)
			{
				ReportOverflow();
			}
		}

		private static void ReportOverflow()
		{
			throw new ArithmeticException("BigInteger would overflow supported range");
		}

		//Static Factory Methods

		/// <summary>
		/// Returns a BigInteger whose value is equal to that of the
		/// specified {@code long}.  This "static factory method" is
		/// provided in preference to a ({@code long}) constructor
		/// because it allows for reuse of frequently used BigIntegers.
		/// </summary>
		/// <param name="val"> value of the BigInteger to return. </param>
		/// <returns> a BigInteger with the specified value. </returns>
		public static BigInteger ValueOf(long val)
		{
			// If -MAX_CONSTANT < val < MAX_CONSTANT, return stashed constant
			if (val == 0)
			{
				return ZERO;
			}
			if (val > 0 && val <= MAX_CONSTANT)
			{
				return PosConst[(int) val];
			}
			else if (val < 0 && val >= -MAX_CONSTANT)
			{
				return NegConst[(int) -val];
			}

			return new BigInteger(val);
		}

		/// <summary>
		/// Constructs a BigInteger with the specified value, which may not be zero.
		/// </summary>
		private BigInteger(long val)
		{
			if (val < 0)
			{
				val = -val;
				Signum_Renamed = -1;
			}
			else
			{
				Signum_Renamed = 1;
			}

			int highWord = (int)((long)((ulong)val >> 32));
			if (highWord == 0)
			{
				Mag = new int[1];
				Mag[0] = (int)val;
			}
			else
			{
				Mag = new int[2];
				Mag[0] = highWord;
				Mag[1] = (int)val;
			}
		}

		/// <summary>
		/// Returns a BigInteger with the given two's complement representation.
		/// Assumes that the input array will not be modified (the returned
		/// BigInteger will reference the input array if feasible).
		/// </summary>
		private static BigInteger ValueOf(int[] val)
		{
			return (val[0] > 0 ? new BigInteger(val, 1) : new BigInteger(val));
		}

		// Constants

		/// <summary>
		/// Initialize static constant array when class is loaded.
		/// </summary>
		private const int MAX_CONSTANT = 16;
		private static BigInteger[] PosConst = new BigInteger[MAX_CONSTANT + 1];
		private static BigInteger[] NegConst = new BigInteger[MAX_CONSTANT + 1];

		/// <summary>
		/// The cache of powers of each radix.  This allows us to not have to
		/// recalculate powers of radix^(2^n) more than once.  This speeds
		/// Schoenhage recursive base conversion significantly.
		/// </summary>
		private static volatile BigInteger[][] PowerCache;

		/// <summary>
		/// The cache of logarithms of radices for base conversion. </summary>
		private static readonly double[] LogCache;

		/// <summary>
		/// The natural log of 2.  This is used in computing cache indices. </summary>
		private static readonly double LOG_TWO = System.Math.Log(2.0);

		static BigInteger()
		{
			for (int i = 1; i <= MAX_CONSTANT; i++)
			{
				int[] magnitude = new int[1];
				magnitude[0] = i;
				PosConst[i] = new BigInteger(magnitude, 1);
				NegConst[i] = new BigInteger(magnitude, -1);
			}

			/*
			 * Initialize the cache of radix^(2^x) values used for base conversion
			 * with just the very first value.  Additional values will be created
			 * on demand.
			 */
			PowerCache = new BigInteger[Character.MAX_RADIX + 1][];
			LogCache = new double[Character.MAX_RADIX + 1];

			for (int i = Character.MIN_RADIX; i <= Character.MAX_RADIX; i++)
			{
				PowerCache[i] = new BigInteger[] {BigInteger.ValueOf(i)};
				LogCache[i] = System.Math.Log(i);
			}
			Zeros[63] = "000000000000000000000000000000000000000000000000000000000000000";
			for (int i = 0; i < 63; i++)
			{
				Zeros[i] = Zeros[63].Substring(0, i);
			}
				try
				{
					@unsafe = sun.misc.Unsafe.Unsafe;
					signumOffset = @unsafe.objectFieldOffset(typeof(BigInteger).getDeclaredField("signum"));
					magOffset = @unsafe.objectFieldOffset(typeof(BigInteger).getDeclaredField("mag"));
				}
				catch (Exception ex)
				{
					throw new ExceptionInInitializerError(ex);
				}
		}

		/// <summary>
		/// The BigInteger constant zero.
		/// 
		/// @since   1.2
		/// </summary>
		public static readonly BigInteger ZERO = new BigInteger(new int[0], 0);

		/// <summary>
		/// The BigInteger constant one.
		/// 
		/// @since   1.2
		/// </summary>
		public static readonly BigInteger ONE = ValueOf(1);

		/// <summary>
		/// The BigInteger constant two.  (Not exported.)
		/// </summary>
		private static readonly BigInteger TWO = ValueOf(2);

		/// <summary>
		/// The BigInteger constant -1.  (Not exported.)
		/// </summary>
		private static readonly BigInteger NEGATIVE_ONE = ValueOf(-1);

		/// <summary>
		/// The BigInteger constant ten.
		/// 
		/// @since   1.5
		/// </summary>
		public static readonly BigInteger TEN = ValueOf(10);

		// Arithmetic Operations

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this + val)}.
		/// </summary>
		/// <param name="val"> value to be added to this BigInteger. </param>
		/// <returns> {@code this + val} </returns>
		public virtual BigInteger Add(BigInteger val)
		{
			if (val.Signum_Renamed == 0)
			{
				return this;
			}
			if (Signum_Renamed == 0)
			{
				return val;
			}
			if (val.Signum_Renamed == Signum_Renamed)
			{
				return new BigInteger(Add(Mag, val.Mag), Signum_Renamed);
			}

			int cmp = CompareMagnitude(val);
			if (cmp == 0)
			{
				return ZERO;
			}
			int[] resultMag = (cmp > 0 ? Subtract(Mag, val.Mag) : Subtract(val.Mag, Mag));
			resultMag = TrustedStripLeadingZeroInts(resultMag);

			return new BigInteger(resultMag, cmp == Signum_Renamed ? 1 : -1);
		}

		/// <summary>
		/// Package private methods used by BigDecimal code to add a BigInteger
		/// with a long. Assumes val is not equal to INFLATED.
		/// </summary>
		internal virtual BigInteger Add(long val)
		{
			if (val == 0)
			{
				return this;
			}
			if (Signum_Renamed == 0)
			{
				return ValueOf(val);
			}
			if (Long.Signum(val) == Signum_Renamed)
			{
				return new BigInteger(Add(Mag, System.Math.Abs(val)), Signum_Renamed);
			}
			int cmp = CompareMagnitude(val);
			if (cmp == 0)
			{
				return ZERO;
			}
			int[] resultMag = (cmp > 0 ? Subtract(Mag, System.Math.Abs(val)) : Subtract(System.Math.Abs(val), Mag));
			resultMag = TrustedStripLeadingZeroInts(resultMag);
			return new BigInteger(resultMag, cmp == Signum_Renamed ? 1 : -1);
		}

		/// <summary>
		/// Adds the contents of the int array x and long value val. This
		/// method allocates a new int array to hold the answer and returns
		/// a reference to that array.  Assumes x.length &gt; 0 and val is
		/// non-negative
		/// </summary>
		private static int[] Add(int[] x, long val)
		{
			int[] y;
			long sum = 0;
			int xIndex = x.Length;
			int[] result;
			int highWord = (int)((long)((ulong)val >> 32));
			if (highWord == 0)
			{
				result = new int[xIndex];
				sum = (x[--xIndex] & LONG_MASK) + val;
				result[xIndex] = (int)sum;
			}
			else
			{
				if (xIndex == 1)
				{
					result = new int[2];
					sum = val + (x[0] & LONG_MASK);
					result[1] = (int)sum;
					result[0] = (int)((long)((ulong)sum >> 32));
					return result;
				}
				else
				{
					result = new int[xIndex];
					sum = (x[--xIndex] & LONG_MASK) + (val & LONG_MASK);
					result[xIndex] = (int)sum;
					sum = (x[--xIndex] & LONG_MASK) + (highWord & LONG_MASK) + ((long)((ulong)sum >> 32));
					result[xIndex] = (int)sum;
				}
			}
			// Copy remainder of longer number while carry propagation is required
			bool carry = ((long)((ulong)sum >> 32) != 0);
			while (xIndex > 0 && carry)
			{
				carry = ((result[--xIndex] = x[xIndex] + 1) == 0);
			}
			// Copy remainder of longer number
			while (xIndex > 0)
			{
				result[--xIndex] = x[xIndex];
			}
			// Grow result if necessary
			if (carry)
			{
				int[] bigger = new int[result.Length + 1];
				System.Array.Copy(result, 0, bigger, 1, result.Length);
				bigger[0] = 0x01;
				return bigger;
			}
			return result;
		}

		/// <summary>
		/// Adds the contents of the int arrays x and y. This method allocates
		/// a new int array to hold the answer and returns a reference to that
		/// array.
		/// </summary>
		private static int[] Add(int[] x, int[] y)
		{
			// If x is shorter, swap the two arrays
			if (x.Length < y.Length)
			{
				int[] tmp = x;
				x = y;
				y = tmp;
			}

			int xIndex = x.Length;
			int yIndex = y.Length;
			int[] result = new int[xIndex];
			long sum = 0;
			if (yIndex == 1)
			{
				sum = (x[--xIndex] & LONG_MASK) + (y[0] & LONG_MASK);
				result[xIndex] = (int)sum;
			}
			else
			{
				// Add common parts of both numbers
				while (yIndex > 0)
				{
					sum = (x[--xIndex] & LONG_MASK) + (y[--yIndex] & LONG_MASK) + ((long)((ulong)sum >> 32));
					result[xIndex] = (int)sum;
				}
			}
			// Copy remainder of longer number while carry propagation is required
			bool carry = ((long)((ulong)sum >> 32) != 0);
			while (xIndex > 0 && carry)
			{
				carry = ((result[--xIndex] = x[xIndex] + 1) == 0);
			}

			// Copy remainder of longer number
			while (xIndex > 0)
			{
				result[--xIndex] = x[xIndex];
			}

			// Grow result if necessary
			if (carry)
			{
				int[] bigger = new int[result.Length + 1];
				System.Array.Copy(result, 0, bigger, 1, result.Length);
				bigger[0] = 0x01;
				return bigger;
			}
			return result;
		}

		private static int[] Subtract(long val, int[] little)
		{
			int highWord = (int)((long)((ulong)val >> 32));
			if (highWord == 0)
			{
				int[] result = new int[1];
				result[0] = (int)(val - (little[0] & LONG_MASK));
				return result;
			}
			else
			{
				int[] result = new int[2];
				if (little.Length == 1)
				{
					long difference = ((int)val & LONG_MASK) - (little[0] & LONG_MASK);
					result[1] = (int)difference;
					// Subtract remainder of longer number while borrow propagates
					bool borrow = (difference >> 32 != 0);
					if (borrow)
					{
						result[0] = highWord - 1;
					} // Copy remainder of longer number
					else
					{
						result[0] = highWord;
					}
					return result;
				} // little.length == 2
				else
				{
					long difference = ((int)val & LONG_MASK) - (little[1] & LONG_MASK);
					result[1] = (int)difference;
					difference = (highWord & LONG_MASK) - (little[0] & LONG_MASK) + (difference >> 32);
					result[0] = (int)difference;
					return result;
				}
			}
		}

		/// <summary>
		/// Subtracts the contents of the second argument (val) from the
		/// first (big).  The first int array (big) must represent a larger number
		/// than the second.  This method allocates the space necessary to hold the
		/// answer.
		/// assumes val &gt;= 0
		/// </summary>
		private static int[] Subtract(int[] big, long val)
		{
			int highWord = (int)((long)((ulong)val >> 32));
			int bigIndex = big.Length;
			int[] result = new int[bigIndex];
			long difference = 0;

			if (highWord == 0)
			{
				difference = (big[--bigIndex] & LONG_MASK) - val;
				result[bigIndex] = (int)difference;
			}
			else
			{
				difference = (big[--bigIndex] & LONG_MASK) - (val & LONG_MASK);
				result[bigIndex] = (int)difference;
				difference = (big[--bigIndex] & LONG_MASK) - (highWord & LONG_MASK) + (difference >> 32);
				result[bigIndex] = (int)difference;
			}

			// Subtract remainder of longer number while borrow propagates
			bool borrow = (difference >> 32 != 0);
			while (bigIndex > 0 && borrow)
			{
				borrow = ((result[--bigIndex] = big[bigIndex] - 1) == -1);
			}

			// Copy remainder of longer number
			while (bigIndex > 0)
			{
				result[--bigIndex] = big[bigIndex];
			}

			return result;
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this - val)}.
		/// </summary>
		/// <param name="val"> value to be subtracted from this BigInteger. </param>
		/// <returns> {@code this - val} </returns>
		public virtual BigInteger Subtract(BigInteger val)
		{
			if (val.Signum_Renamed == 0)
			{
				return this;
			}
			if (Signum_Renamed == 0)
			{
				return val.Negate();
			}
			if (val.Signum_Renamed != Signum_Renamed)
			{
				return new BigInteger(Add(Mag, val.Mag), Signum_Renamed);
			}

			int cmp = CompareMagnitude(val);
			if (cmp == 0)
			{
				return ZERO;
			}
			int[] resultMag = (cmp > 0 ? Subtract(Mag, val.Mag) : Subtract(val.Mag, Mag));
			resultMag = TrustedStripLeadingZeroInts(resultMag);
			return new BigInteger(resultMag, cmp == Signum_Renamed ? 1 : -1);
		}

		/// <summary>
		/// Subtracts the contents of the second int arrays (little) from the
		/// first (big).  The first int array (big) must represent a larger number
		/// than the second.  This method allocates the space necessary to hold the
		/// answer.
		/// </summary>
		private static int[] Subtract(int[] big, int[] little)
		{
			int bigIndex = big.Length;
			int[] result = new int[bigIndex];
			int littleIndex = little.Length;
			long difference = 0;

			// Subtract common parts of both numbers
			while (littleIndex > 0)
			{
				difference = (big[--bigIndex] & LONG_MASK) - (little[--littleIndex] & LONG_MASK) + (difference >> 32);
				result[bigIndex] = (int)difference;
			}

			// Subtract remainder of longer number while borrow propagates
			bool borrow = (difference >> 32 != 0);
			while (bigIndex > 0 && borrow)
			{
				borrow = ((result[--bigIndex] = big[bigIndex] - 1) == -1);
			}

			// Copy remainder of longer number
			while (bigIndex > 0)
			{
				result[--bigIndex] = big[bigIndex];
			}

			return result;
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this * val)}.
		/// 
		/// @implNote An implementation may offer better algorithmic
		/// performance when {@code val == this}.
		/// </summary>
		/// <param name="val"> value to be multiplied by this BigInteger. </param>
		/// <returns> {@code this * val} </returns>
		public virtual BigInteger Multiply(BigInteger val)
		{
			if (val.Signum_Renamed == 0 || Signum_Renamed == 0)
			{
				return ZERO;
			}

			int xlen = Mag.Length;

			if (val == this && xlen > MULTIPLY_SQUARE_THRESHOLD)
			{
				return Square();
			}

			int ylen = val.Mag.Length;

			if ((xlen < KARATSUBA_THRESHOLD) || (ylen < KARATSUBA_THRESHOLD))
			{
				int resultSign = Signum_Renamed == val.Signum_Renamed ? 1 : -1;
				if (val.Mag.Length == 1)
				{
					return MultiplyByInt(Mag,val.Mag[0], resultSign);
				}
				if (Mag.Length == 1)
				{
					return MultiplyByInt(val.Mag,Mag[0], resultSign);
				}
				int[] result = MultiplyToLen(Mag, xlen, val.Mag, ylen, null);
				result = TrustedStripLeadingZeroInts(result);
				return new BigInteger(result, resultSign);
			}
			else
			{
				if ((xlen < TOOM_COOK_THRESHOLD) && (ylen < TOOM_COOK_THRESHOLD))
				{
					return MultiplyKaratsuba(this, val);
				}
				else
				{
					return MultiplyToomCook3(this, val);
				}
			}
		}

		private static BigInteger MultiplyByInt(int[] x, int y, int sign)
		{
			if (Integer.BitCount(y) == 1)
			{
				return new BigInteger(ShiftLeft(x,Integer.NumberOfTrailingZeros(y)), sign);
			}
			int xlen = x.Length;
			int[] rmag = new int[xlen + 1];
			long carry = 0;
			long yl = y & LONG_MASK;
			int rstart = rmag.Length - 1;
			for (int i = xlen - 1; i >= 0; i--)
			{
				long product = (x[i] & LONG_MASK) * yl + carry;
				rmag[rstart--] = (int)product;
				carry = (long)((ulong)product >> 32);
			}
			if (carry == 0L)
			{
				rmag = Arrays.CopyOfRange(rmag, 1, rmag.Length);
			}
			else
			{
				rmag[rstart] = (int)carry;
			}
			return new BigInteger(rmag, sign);
		}

		/// <summary>
		/// Package private methods used by BigDecimal code to multiply a BigInteger
		/// with a long. Assumes v is not equal to INFLATED.
		/// </summary>
		internal virtual BigInteger Multiply(long v)
		{
			if (v == 0 || Signum_Renamed == 0)
			{
			  return ZERO;
			}
			if (v == BigDecimal.INFLATED)
			{
				return Multiply(BigInteger.ValueOf(v));
			}
			int rsign = (v > 0 ? Signum_Renamed : -Signum_Renamed);
			if (v < 0)
			{
				v = -v;
			}
			long dh = (long)((ulong)v >> 32); // higher order bits
			long dl = v & LONG_MASK; // lower order bits

			int xlen = Mag.Length;
			int[] value = Mag;
			int[] rmag = (dh == 0L) ? (new int[xlen + 1]) : (new int[xlen + 2]);
			long carry = 0;
			int rstart = rmag.Length - 1;
			for (int i = xlen - 1; i >= 0; i--)
			{
				long product = (value[i] & LONG_MASK) * dl + carry;
				rmag[rstart--] = (int)product;
				carry = (long)((ulong)product >> 32);
			}
			rmag[rstart] = (int)carry;
			if (dh != 0L)
			{
				carry = 0;
				rstart = rmag.Length - 2;
				for (int i = xlen - 1; i >= 0; i--)
				{
					long product = (value[i] & LONG_MASK) * dh + (rmag[rstart] & LONG_MASK) + carry;
					rmag[rstart--] = (int)product;
					carry = (long)((ulong)product >> 32);
				}
				rmag[0] = (int)carry;
			}
			if (carry == 0L)
			{
				rmag = Arrays.CopyOfRange(rmag, 1, rmag.Length);
			}
			return new BigInteger(rmag, rsign);
		}

		/// <summary>
		/// Multiplies int arrays x and y to the specified lengths and places
		/// the result into z. There will be no leading zeros in the resultant array.
		/// </summary>
		private int[] MultiplyToLen(int[] x, int xlen, int[] y, int ylen, int[] z)
		{
			int xstart = xlen - 1;
			int ystart = ylen - 1;

			if (z == null || z.Length < (xlen + ylen))
			{
				z = new int[xlen + ylen];
			}

			long carry = 0;
			for (int j = ystart, k = ystart + 1 + xstart; j >= 0; j--, k--)
			{
				long product = (y[j] & LONG_MASK) * (x[xstart] & LONG_MASK) + carry;
				z[k] = (int)product;
				carry = (long)((ulong)product >> 32);
			}
			z[xstart] = (int)carry;

			for (int i = xstart - 1; i >= 0; i--)
			{
				carry = 0;
				for (int j = ystart, k = ystart + 1 + i; j >= 0; j--, k--)
				{
					long product = (y[j] & LONG_MASK) * (x[i] & LONG_MASK) + (z[k] & LONG_MASK) + carry;
					z[k] = (int)product;
					carry = (long)((ulong)product >> 32);
				}
				z[i] = (int)carry;
			}
			return z;
		}

		/// <summary>
		/// Multiplies two BigIntegers using the Karatsuba multiplication
		/// algorithm.  This is a recursive divide-and-conquer algorithm which is
		/// more efficient for large numbers than what is commonly called the
		/// "grade-school" algorithm used in multiplyToLen.  If the numbers to be
		/// multiplied have length n, the "grade-school" algorithm has an
		/// asymptotic complexity of O(n^2).  In contrast, the Karatsuba algorithm
		/// has complexity of O(n^(log2(3))), or O(n^1.585).  It achieves this
		/// increased performance by doing 3 multiplies instead of 4 when
		/// evaluating the product.  As it has some overhead, should be used when
		/// both numbers are larger than a certain threshold (found
		/// experimentally).
		/// 
		/// See:  http://en.wikipedia.org/wiki/Karatsuba_algorithm
		/// </summary>
		private static BigInteger MultiplyKaratsuba(BigInteger x, BigInteger y)
		{
			int xlen = x.Mag.Length;
			int ylen = y.Mag.Length;

			// The number of ints in each half of the number.
			int half = (System.Math.Max(xlen, ylen) + 1) / 2;

			// xl and yl are the lower halves of x and y respectively,
			// xh and yh are the upper halves.
			BigInteger xl = x.GetLower(half);
			BigInteger xh = x.GetUpper(half);
			BigInteger yl = y.GetLower(half);
			BigInteger yh = y.GetUpper(half);

			BigInteger p1 = xh.Multiply(yh); // p1 = xh*yh
			BigInteger p2 = xl.Multiply(yl); // p2 = xl*yl

			// p3=(xh+xl)*(yh+yl)
			BigInteger p3 = xh.Add(xl).Multiply(yh.Add(yl));

			// result = p1 * 2^(32*2*half) + (p3 - p1 - p2) * 2^(32*half) + p2
			BigInteger result = p1.ShiftLeft(32 * half).Add(p3.Subtract(p1).Subtract(p2)).ShiftLeft(32 * half).Add(p2);

			if (x.Signum_Renamed != y.Signum_Renamed)
			{
				return result.Negate();
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Multiplies two BigIntegers using a 3-way Toom-Cook multiplication
		/// algorithm.  This is a recursive divide-and-conquer algorithm which is
		/// more efficient for large numbers than what is commonly called the
		/// "grade-school" algorithm used in multiplyToLen.  If the numbers to be
		/// multiplied have length n, the "grade-school" algorithm has an
		/// asymptotic complexity of O(n^2).  In contrast, 3-way Toom-Cook has a
		/// complexity of about O(n^1.465).  It achieves this increased asymptotic
		/// performance by breaking each number into three parts and by doing 5
		/// multiplies instead of 9 when evaluating the product.  Due to overhead
		/// (additions, shifts, and one division) in the Toom-Cook algorithm, it
		/// should only be used when both numbers are larger than a certain
		/// threshold (found experimentally).  This threshold is generally larger
		/// than that for Karatsuba multiplication, so this algorithm is generally
		/// only used when numbers become significantly larger.
		/// 
		/// The algorithm used is the "optimal" 3-way Toom-Cook algorithm outlined
		/// by Marco Bodrato.
		/// 
		///  See: http://bodrato.it/toom-cook/
		///       http://bodrato.it/papers/#WAIFI2007
		/// 
		/// "Towards Optimal Toom-Cook Multiplication for Univariate and
		/// Multivariate Polynomials in Characteristic 2 and 0." by Marco BODRATO;
		/// In C.Carlet and B.Sunar, Eds., "WAIFI'07 proceedings", p. 116-133,
		/// LNCS #4547. Springer, Madrid, Spain, June 21-22, 2007.
		/// 
		/// </summary>
		private static BigInteger MultiplyToomCook3(BigInteger a, BigInteger b)
		{
			int alen = a.Mag.Length;
			int blen = b.Mag.Length;

			int largest = System.Math.Max(alen, blen);

			// k is the size (in ints) of the lower-order slices.
			int k = (largest + 2) / 3; // Equal to ceil(largest/3)

			// r is the size (in ints) of the highest-order slice.
			int r = largest - 2 * k;

			// Obtain slices of the numbers. a2 and b2 are the most significant
			// bits of the numbers a and b, and a0 and b0 the least significant.
			BigInteger a0, a1, a2, b0, b1, b2;
			a2 = a.GetToomSlice(k, r, 0, largest);
			a1 = a.GetToomSlice(k, r, 1, largest);
			a0 = a.GetToomSlice(k, r, 2, largest);
			b2 = b.GetToomSlice(k, r, 0, largest);
			b1 = b.GetToomSlice(k, r, 1, largest);
			b0 = b.GetToomSlice(k, r, 2, largest);

			BigInteger v0, v1, v2, vm1, vinf, t1, t2, tm1, da1, db1;

			v0 = a0.Multiply(b0);
			da1 = a2.Add(a0);
			db1 = b2.Add(b0);
			vm1 = da1.Subtract(a1).Multiply(db1.Subtract(b1));
			da1 = da1.Add(a1);
			db1 = db1.Add(b1);
			v1 = da1.Multiply(db1);
			v2 = da1.Add(a2).ShiftLeft(1).Subtract(a0).Multiply(db1.Add(b2).ShiftLeft(1).Subtract(b0));
			vinf = a2.Multiply(b2);

			// The algorithm requires two divisions by 2 and one by 3.
			// All divisions are known to be exact, that is, they do not produce
			// remainders, and all results are positive.  The divisions by 2 are
			// implemented as right shifts which are relatively efficient, leaving
			// only an exact division by 3, which is done by a specialized
			// linear-time algorithm.
			t2 = v2.Subtract(vm1).ExactDivideBy3();
			tm1 = v1.Subtract(vm1).ShiftRight(1);
			t1 = v1.Subtract(v0);
			t2 = t2.Subtract(t1).ShiftRight(1);
			t1 = t1.Subtract(tm1).Subtract(vinf);
			t2 = t2.Subtract(vinf.ShiftLeft(1));
			tm1 = tm1.Subtract(t2);

			// Number of bits to shift left.
			int ss = k * 32;

			BigInteger result = vinf.ShiftLeft(ss).Add(t2).ShiftLeft(ss).Add(t1).ShiftLeft(ss).Add(tm1).ShiftLeft(ss).Add(v0);

			if (a.Signum_Renamed != b.Signum_Renamed)
			{
				return result.Negate();
			}
			else
			{
				return result;
			}
		}


		/// <summary>
		/// Returns a slice of a BigInteger for use in Toom-Cook multiplication.
		/// </summary>
		/// <param name="lowerSize"> The size of the lower-order bit slices. </param>
		/// <param name="upperSize"> The size of the higher-order bit slices. </param>
		/// <param name="slice"> The index of which slice is requested, which must be a
		/// number from 0 to size-1. Slice 0 is the highest-order bits, and slice
		/// size-1 are the lowest-order bits. Slice 0 may be of different size than
		/// the other slices. </param>
		/// <param name="fullsize"> The size of the larger integer array, used to align
		/// slices to the appropriate position when multiplying different-sized
		/// numbers. </param>
		private BigInteger GetToomSlice(int lowerSize, int upperSize, int slice, int fullsize)
		{
			int start, end, sliceSize, len, offset;

			len = Mag.Length;
			offset = fullsize - len;

			if (slice == 0)
			{
				start = 0 - offset;
				end = upperSize - 1 - offset;
			}
			else
			{
				start = upperSize + (slice-1) * lowerSize - offset;
				end = start + lowerSize - 1;
			}

			if (start < 0)
			{
				start = 0;
			}
			if (end < 0)
			{
			   return ZERO;
			}

			sliceSize = (end - start) + 1;

			if (sliceSize <= 0)
			{
				return ZERO;
			}

			// While performing Toom-Cook, all slices are positive and
			// the sign is adjusted when the final number is composed.
			if (start == 0 && sliceSize >= len)
			{
				return this.Abs();
			}

			int[] intSlice = new int[sliceSize];
			System.Array.Copy(Mag, start, intSlice, 0, sliceSize);

			return new BigInteger(TrustedStripLeadingZeroInts(intSlice), 1);
		}

		/// <summary>
		/// Does an exact division (that is, the remainder is known to be zero)
		/// of the specified number by 3.  This is used in Toom-Cook
		/// multiplication.  This is an efficient algorithm that runs in linear
		/// time.  If the argument is not exactly divisible by 3, results are
		/// undefined.  Note that this is expected to be called with positive
		/// arguments only.
		/// </summary>
		private BigInteger ExactDivideBy3()
		{
			int len = Mag.Length;
			int[] result = new int[len];
			long x, w, q, borrow;
			borrow = 0L;
			for (int i = len - 1; i >= 0; i--)
			{
				x = (Mag[i] & LONG_MASK);
				w = x - borrow;
				if (borrow > x) // Did we make the number go negative?
				{
					borrow = 1L;
				}
				else
				{
					borrow = 0L;
				}

				// 0xAAAAAAAB is the modular inverse of 3 (mod 2^32).  Thus,
				// the effect of this is to divide by 3 (mod 2^32).
				// This is much faster than division on most architectures.
				q = (w * 0xAAAAAAABL) & LONG_MASK;
				result[i] = (int) q;

				// Now check the borrow. The second check can of course be
				// eliminated if the first fails.
				if (q >= 0x55555556L)
				{
					borrow++;
					if (q >= 0xAAAAAAABL)
					{
						borrow++;
					}
				}
			}
			result = TrustedStripLeadingZeroInts(result);
			return new BigInteger(result, Signum_Renamed);
		}

		/// <summary>
		/// Returns a new BigInteger representing n lower ints of the number.
		/// This is used by Karatsuba multiplication and Karatsuba squaring.
		/// </summary>
		private BigInteger GetLower(int n)
		{
			int len = Mag.Length;

			if (len <= n)
			{
				return Abs();
			}

			int[] lowerInts = new int[n];
			System.Array.Copy(Mag, len - n, lowerInts, 0, n);

			return new BigInteger(TrustedStripLeadingZeroInts(lowerInts), 1);
		}

		/// <summary>
		/// Returns a new BigInteger representing mag.length-n upper
		/// ints of the number.  This is used by Karatsuba multiplication and
		/// Karatsuba squaring.
		/// </summary>
		private BigInteger GetUpper(int n)
		{
			int len = Mag.Length;

			if (len <= n)
			{
				return ZERO;
			}

			int upperLen = len - n;
			int[] upperInts = new int[upperLen];
			System.Array.Copy(Mag, 0, upperInts, 0, upperLen);

			return new BigInteger(TrustedStripLeadingZeroInts(upperInts), 1);
		}

		// Squaring

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this<sup>2</sup>)}.
		/// </summary>
		/// <returns> {@code this<sup>2</sup>} </returns>
		private BigInteger Square()
		{
			if (Signum_Renamed == 0)
			{
				return ZERO;
			}
			int len = Mag.Length;

			if (len < KARATSUBA_SQUARE_THRESHOLD)
			{
				int[] z = SquareToLen(Mag, len, null);
				return new BigInteger(TrustedStripLeadingZeroInts(z), 1);
			}
			else
			{
				if (len < TOOM_COOK_SQUARE_THRESHOLD)
				{
					return SquareKaratsuba();
				}
				else
				{
					return SquareToomCook3();
				}
			}
		}

		/// <summary>
		/// Squares the contents of the int array x. The result is placed into the
		/// int array z.  The contents of x are not changed.
		/// </summary>
		private static int[] SquareToLen(int[] x, int len, int[] z)
		{
			/*
			 * The algorithm used here is adapted from Colin Plumb's C library.
			 * Technique: Consider the partial products in the multiplication
			 * of "abcde" by itself:
			 *
			 *               a  b  c  d  e
			 *            *  a  b  c  d  e
			 *          ==================
			 *              ae be ce de ee
			 *           ad bd cd dd de
			 *        ac bc cc cd ce
			 *     ab bb bc bd be
			 *  aa ab ac ad ae
			 *
			 * Note that everything above the main diagonal:
			 *              ae be ce de = (abcd) * e
			 *           ad bd cd       = (abc) * d
			 *        ac bc             = (ab) * c
			 *     ab                   = (a) * b
			 *
			 * is a copy of everything below the main diagonal:
			 *                       de
			 *                 cd ce
			 *           bc bd be
			 *     ab ac ad ae
			 *
			 * Thus, the sum is 2 * (off the diagonal) + diagonal.
			 *
			 * This is accumulated beginning with the diagonal (which
			 * consist of the squares of the digits of the input), which is then
			 * divided by two, the off-diagonal added, and multiplied by two
			 * again.  The low bit is simply a copy of the low bit of the
			 * input, so it doesn't need special care.
			 */
			int zlen = len << 1;
			if (z == null || z.Length < zlen)
			{
				z = new int[zlen];
			}

			// Store the squares, right shifted one bit (i.e., divided by 2)
			int lastProductLowWord = 0;
			for (int j = 0, i = 0; j < len; j++)
			{
				long piece = (x[j] & LONG_MASK);
				long product = piece * piece;
				z[i++] = (lastProductLowWord << 31) | (int)((long)((ulong)product >> 33));
				z[i++] = (int)((long)((ulong)product >> 1));
				lastProductLowWord = (int)product;
			}

			// Add in off-diagonal sums
			for (int i = len, offset = 1; i > 0; i--, offset += 2)
			{
				int t = x[i - 1];
				t = MulAdd(z, x, offset, i - 1, t);
				AddOne(z, offset - 1, i, t);
			}

			// Shift back up and set low bit
			PrimitiveLeftShift(z, zlen, 1);
			z[zlen - 1] |= x[len - 1] & 1;

			return z;
		}

		/// <summary>
		/// Squares a BigInteger using the Karatsuba squaring algorithm.  It should
		/// be used when both numbers are larger than a certain threshold (found
		/// experimentally).  It is a recursive divide-and-conquer algorithm that
		/// has better asymptotic performance than the algorithm used in
		/// squareToLen.
		/// </summary>
		private BigInteger SquareKaratsuba()
		{
			int half = (Mag.Length + 1) / 2;

			BigInteger xl = GetLower(half);
			BigInteger xh = GetUpper(half);

			BigInteger xhs = xh.Square(); // xhs = xh^2
			BigInteger xls = xl.Square(); // xls = xl^2

			// xh^2 << 64  +  (((xl+xh)^2 - (xh^2 + xl^2)) << 32) + xl^2
			return xhs.ShiftLeft(half * 32).Add(xl.Add(xh).Square().Subtract(xhs.Add(xls))).ShiftLeft(half * 32).Add(xls);
		}

		/// <summary>
		/// Squares a BigInteger using the 3-way Toom-Cook squaring algorithm.  It
		/// should be used when both numbers are larger than a certain threshold
		/// (found experimentally).  It is a recursive divide-and-conquer algorithm
		/// that has better asymptotic performance than the algorithm used in
		/// squareToLen or squareKaratsuba.
		/// </summary>
		private BigInteger SquareToomCook3()
		{
			int len = Mag.Length;

			// k is the size (in ints) of the lower-order slices.
			int k = (len + 2) / 3; // Equal to ceil(largest/3)

			// r is the size (in ints) of the highest-order slice.
			int r = len - 2 * k;

			// Obtain slices of the numbers. a2 is the most significant
			// bits of the number, and a0 the least significant.
			BigInteger a0, a1, a2;
			a2 = GetToomSlice(k, r, 0, len);
			a1 = GetToomSlice(k, r, 1, len);
			a0 = GetToomSlice(k, r, 2, len);
			BigInteger v0, v1, v2, vm1, vinf, t1, t2, tm1, da1;

			v0 = a0.Square();
			da1 = a2.Add(a0);
			vm1 = da1.Subtract(a1).Square();
			da1 = da1.Add(a1);
			v1 = da1.Square();
			vinf = a2.Square();
			v2 = da1.Add(a2).ShiftLeft(1).Subtract(a0).Square();

			// The algorithm requires two divisions by 2 and one by 3.
			// All divisions are known to be exact, that is, they do not produce
			// remainders, and all results are positive.  The divisions by 2 are
			// implemented as right shifts which are relatively efficient, leaving
			// only a division by 3.
			// The division by 3 is done by an optimized algorithm for this case.
			t2 = v2.Subtract(vm1).ExactDivideBy3();
			tm1 = v1.Subtract(vm1).ShiftRight(1);
			t1 = v1.Subtract(v0);
			t2 = t2.Subtract(t1).ShiftRight(1);
			t1 = t1.Subtract(tm1).Subtract(vinf);
			t2 = t2.Subtract(vinf.ShiftLeft(1));
			tm1 = tm1.Subtract(t2);

			// Number of bits to shift left.
			int ss = k * 32;

			return vinf.ShiftLeft(ss).Add(t2).ShiftLeft(ss).Add(t1).ShiftLeft(ss).Add(tm1).ShiftLeft(ss).Add(v0);
		}

		// Division

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this / val)}.
		/// </summary>
		/// <param name="val"> value by which this BigInteger is to be divided. </param>
		/// <returns> {@code this / val} </returns>
		/// <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		public virtual BigInteger Divide(BigInteger val)
		{
			if (val.Mag.Length < BURNIKEL_ZIEGLER_THRESHOLD || Mag.Length - val.Mag.Length < BURNIKEL_ZIEGLER_OFFSET)
			{
				return DivideKnuth(val);
			}
			else
			{
				return DivideBurnikelZiegler(val);
			}
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this / val)} using an O(n^2) algorithm from Knuth.
		/// </summary>
		/// <param name="val"> value by which this BigInteger is to be divided. </param>
		/// <returns> {@code this / val} </returns>
		/// <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		/// <seealso cref= MutableBigInteger#divideKnuth(MutableBigInteger, MutableBigInteger, boolean) </seealso>
		private BigInteger DivideKnuth(BigInteger val)
		{
			MutableBigInteger q = new MutableBigInteger(), a = new MutableBigInteger(this.Mag), b = new MutableBigInteger(val.Mag);

			a.DivideKnuth(b, q, false);
			return q.ToBigInteger(this.Signum_Renamed * val.Signum_Renamed);
		}

		/// <summary>
		/// Returns an array of two BigIntegers containing {@code (this / val)}
		/// followed by {@code (this % val)}.
		/// </summary>
		/// <param name="val"> value by which this BigInteger is to be divided, and the
		///         remainder computed. </param>
		/// <returns> an array of two BigIntegers: the quotient {@code (this / val)}
		///         is the initial element, and the remainder {@code (this % val)}
		///         is the final element. </returns>
		/// <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		public virtual BigInteger[] DivideAndRemainder(BigInteger val)
		{
			if (val.Mag.Length < BURNIKEL_ZIEGLER_THRESHOLD || Mag.Length - val.Mag.Length < BURNIKEL_ZIEGLER_OFFSET)
			{
				return DivideAndRemainderKnuth(val);
			}
			else
			{
				return DivideAndRemainderBurnikelZiegler(val);
			}
		}

		/// <summary>
		/// Long division </summary>
		private BigInteger[] DivideAndRemainderKnuth(BigInteger val)
		{
			BigInteger[] result = new BigInteger[2];
			MutableBigInteger q = new MutableBigInteger(), a = new MutableBigInteger(this.Mag), b = new MutableBigInteger(val.Mag);
			MutableBigInteger r = a.DivideKnuth(b, q);
			result[0] = q.ToBigInteger(this.Signum_Renamed == val.Signum_Renamed ? 1 : -1);
			result[1] = r.ToBigInteger(this.Signum_Renamed);
			return result;
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this % val)}.
		/// </summary>
		/// <param name="val"> value by which this BigInteger is to be divided, and the
		///         remainder computed. </param>
		/// <returns> {@code this % val} </returns>
		/// <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		public virtual BigInteger Remainder(BigInteger val)
		{
			if (val.Mag.Length < BURNIKEL_ZIEGLER_THRESHOLD || Mag.Length - val.Mag.Length < BURNIKEL_ZIEGLER_OFFSET)
			{
				return RemainderKnuth(val);
			}
			else
			{
				return RemainderBurnikelZiegler(val);
			}
		}

		/// <summary>
		/// Long division </summary>
		private BigInteger RemainderKnuth(BigInteger val)
		{
			MutableBigInteger q = new MutableBigInteger(), a = new MutableBigInteger(this.Mag), b = new MutableBigInteger(val.Mag);

			return a.DivideKnuth(b, q).ToBigInteger(this.Signum_Renamed);
		}

		/// <summary>
		/// Calculates {@code this / val} using the Burnikel-Ziegler algorithm. </summary>
		/// <param name="val"> the divisor </param>
		/// <returns> {@code this / val} </returns>
		private BigInteger DivideBurnikelZiegler(BigInteger val)
		{
			return DivideAndRemainderBurnikelZiegler(val)[0];
		}

		/// <summary>
		/// Calculates {@code this % val} using the Burnikel-Ziegler algorithm. </summary>
		/// <param name="val"> the divisor </param>
		/// <returns> {@code this % val} </returns>
		private BigInteger RemainderBurnikelZiegler(BigInteger val)
		{
			return DivideAndRemainderBurnikelZiegler(val)[1];
		}

		/// <summary>
		/// Computes {@code this / val} and {@code this % val} using the
		/// Burnikel-Ziegler algorithm. </summary>
		/// <param name="val"> the divisor </param>
		/// <returns> an array containing the quotient and remainder </returns>
		private BigInteger[] DivideAndRemainderBurnikelZiegler(BigInteger val)
		{
			MutableBigInteger q = new MutableBigInteger();
			MutableBigInteger r = (new MutableBigInteger(this)).DivideAndRemainderBurnikelZiegler(new MutableBigInteger(val), q);
			BigInteger qBigInt = q.Zero ? ZERO : q.ToBigInteger(Signum_Renamed * val.Signum_Renamed);
			BigInteger rBigInt = r.Zero ? ZERO : r.ToBigInteger(Signum_Renamed);
			return new BigInteger[] {qBigInt, rBigInt};
		}

		/// <summary>
		/// Returns a BigInteger whose value is <tt>(this<sup>exponent</sup>)</tt>.
		/// Note that {@code exponent} is an integer rather than a BigInteger.
		/// </summary>
		/// <param name="exponent"> exponent to which this BigInteger is to be raised. </param>
		/// <returns> <tt>this<sup>exponent</sup></tt> </returns>
		/// <exception cref="ArithmeticException"> {@code exponent} is negative.  (This would
		///         cause the operation to yield a non-integer value.) </exception>
		public virtual BigInteger Pow(int exponent)
		{
			if (exponent < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			if (Signum_Renamed == 0)
			{
				return (exponent == 0 ? ONE : this);
			}

			BigInteger partToSquare = this.Abs();

			// Factor out powers of two from the base, as the exponentiation of
			// these can be done by left shifts only.
			// The remaining part can then be exponentiated faster.  The
			// powers of two will be multiplied back at the end.
			int powersOfTwo = partToSquare.LowestSetBit;
			long bitsToShift = (long)powersOfTwo * exponent;
			if (bitsToShift > Integer.MaxValue)
			{
				ReportOverflow();
			}

			int remainingBits;

			// Factor the powers of two out quickly by shifting right, if needed.
			if (powersOfTwo > 0)
			{
				partToSquare = partToSquare.ShiftRight(powersOfTwo);
				remainingBits = partToSquare.BitLength();
				if (remainingBits == 1) // Nothing left but +/- 1?
				{
					if (Signum_Renamed < 0 && (exponent & 1) == 1)
					{
						return NEGATIVE_ONE.ShiftLeft(powersOfTwo * exponent);
					}
					else
					{
						return ONE.ShiftLeft(powersOfTwo * exponent);
					}
				}
			}
			else
			{
				remainingBits = partToSquare.BitLength();
				if (remainingBits == 1) // Nothing left but +/- 1?
				{
					if (Signum_Renamed < 0 && (exponent & 1) == 1)
					{
						return NEGATIVE_ONE;
					}
					else
					{
						return ONE;
					}
				}
			}

			// This is a quick way to approximate the size of the result,
			// similar to doing log2[n] * exponent.  This will give an upper bound
			// of how big the result can be, and which algorithm to use.
			long scaleFactor = (long)remainingBits * exponent;

			// Use slightly different algorithms for small and large operands.
			// See if the result will safely fit into a long. (Largest 2^63-1)
			if (partToSquare.Mag.Length == 1 && scaleFactor <= 62)
			{
				// Small number algorithm.  Everything fits into a long.
				int newSign = (Signum_Renamed < 0 && (exponent & 1) == 1 ? - 1 : 1);
				long result = 1;
				long baseToPow2 = partToSquare.Mag[0] & LONG_MASK;

				int workingExponent = exponent;

				// Perform exponentiation using repeated squaring trick
				while (workingExponent != 0)
				{
					if ((workingExponent & 1) == 1)
					{
						result = result * baseToPow2;
					}

					if ((workingExponent = (int)((uint)workingExponent >> 1)) != 0)
					{
						baseToPow2 = baseToPow2 * baseToPow2;
					}
				}

				// Multiply back the powers of two (quickly, by shifting left)
				if (powersOfTwo > 0)
				{
					if (bitsToShift + scaleFactor <= 62) // Fits in long?
					{
						return ValueOf((result << bitsToShift) * newSign);
					}
					else
					{
						return ValueOf(result * newSign).ShiftLeft((int) bitsToShift);
					}
				}
				else
				{
					return ValueOf(result * newSign);
				}
			}
			else
			{
				// Large number algorithm.  This is basically identical to
				// the algorithm above, but calls multiply() and square()
				// which may use more efficient algorithms for large numbers.
				BigInteger answer = ONE;

				int workingExponent = exponent;
				// Perform exponentiation using repeated squaring trick
				while (workingExponent != 0)
				{
					if ((workingExponent & 1) == 1)
					{
						answer = answer.Multiply(partToSquare);
					}

					if ((workingExponent = (int)((uint)workingExponent >> 1)) != 0)
					{
						partToSquare = partToSquare.Square();
					}
				}
				// Multiply back the (exponentiated) powers of two (quickly,
				// by shifting left)
				if (powersOfTwo > 0)
				{
					answer = answer.ShiftLeft(powersOfTwo * exponent);
				}

				if (Signum_Renamed < 0 && (exponent & 1) == 1)
				{
					return answer.Negate();
				}
				else
				{
					return answer;
				}
			}
		}

		/// <summary>
		/// Returns a BigInteger whose value is the greatest common divisor of
		/// {@code abs(this)} and {@code abs(val)}.  Returns 0 if
		/// {@code this == 0 && val == 0}.
		/// </summary>
		/// <param name="val"> value with which the GCD is to be computed. </param>
		/// <returns> {@code GCD(abs(this), abs(val))} </returns>
		public virtual BigInteger Gcd(BigInteger val)
		{
			if (val.Signum_Renamed == 0)
			{
				return this.Abs();
			}
			else if (this.Signum_Renamed == 0)
			{
				return val.Abs();
			}

			MutableBigInteger a = new MutableBigInteger(this);
			MutableBigInteger b = new MutableBigInteger(val);

			MutableBigInteger result = a.HybridGCD(b);

			return result.ToBigInteger(1);
		}

		/// <summary>
		/// Package private method to return bit length for an integer.
		/// </summary>
		internal static int BitLengthForInt(int n)
		{
			return 32 - Integer.NumberOfLeadingZeros(n);
		}

		/// <summary>
		/// Left shift int array a up to len by n bits. Returns the array that
		/// results from the shift since space may have to be reallocated.
		/// </summary>
		private static int[] LeftShift(int[] a, int len, int n)
		{
			int nInts = (int)((uint)n >> 5);
			int nBits = n & 0x1F;
			int bitsInHighWord = BitLengthForInt(a[0]);

			// If shift can be done without recopy, do so
			if (n <= (32 - bitsInHighWord))
			{
				PrimitiveLeftShift(a, len, nBits);
				return a;
			} // Array must be resized
			else
			{
				if (nBits <= (32 - bitsInHighWord))
				{
					int[] result = new int[nInts + len];
					System.Array.Copy(a, 0, result, 0, len);
					PrimitiveLeftShift(result, result.Length, nBits);
					return result;
				}
				else
				{
					int[] result = new int[nInts + len + 1];
					System.Array.Copy(a, 0, result, 0, len);
					PrimitiveRightShift(result, result.Length, 32 - nBits);
					return result;
				}
			}
		}

		// shifts a up to len right n bits assumes no leading zeros, 0<n<32
		internal static void PrimitiveRightShift(int[] a, int len, int n)
		{
			int n2 = 32 - n;
			for (int i = len - 1, c = a[i]; i > 0; i--)
			{
				int b = c;
				c = a[i - 1];
				a[i] = (c << n2) | ((int)((uint)b >> n));
			}
			a[0] = (int)((uint)a[0] >> n);
		}

		// shifts a up to len left n bits assumes no leading zeros, 0<=n<32
		internal static void PrimitiveLeftShift(int[] a, int len, int n)
		{
			if (len == 0 || n == 0)
			{
				return;
			}

			int n2 = 32 - n;
			for (int i = 0, c = a[i], m = i + len - 1; i < m; i++)
			{
				int b = c;
				c = a[i + 1];
				a[i] = (b << n) | ((int)((uint)c >> n2));
			}
			a[len - 1] <<= n;
		}

		/// <summary>
		/// Calculate bitlength of contents of the first len elements an int array,
		/// assuming there are no leading zero ints.
		/// </summary>
		private static int BitLength(int[] val, int len)
		{
			if (len == 0)
			{
				return 0;
			}
			return ((len - 1) << 5) + BitLengthForInt(val[0]);
		}

		/// <summary>
		/// Returns a BigInteger whose value is the absolute value of this
		/// BigInteger.
		/// </summary>
		/// <returns> {@code abs(this)} </returns>
		public virtual BigInteger Abs()
		{
			return (Signum_Renamed >= 0 ? this : this.Negate());
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (-this)}.
		/// </summary>
		/// <returns> {@code -this} </returns>
		public virtual BigInteger Negate()
		{
			return new BigInteger(this.Mag, -this.Signum_Renamed);
		}

		/// <summary>
		/// Returns the signum function of this BigInteger.
		/// </summary>
		/// <returns> -1, 0 or 1 as the value of this BigInteger is negative, zero or
		///         positive. </returns>
		public virtual int Signum()
		{
			return this.Signum_Renamed;
		}

		// Modular Arithmetic Operations

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this mod m}).  This method
		/// differs from {@code remainder} in that it always returns a
		/// <i>non-negative</i> BigInteger.
		/// </summary>
		/// <param name="m"> the modulus. </param>
		/// <returns> {@code this mod m} </returns>
		/// <exception cref="ArithmeticException"> {@code m} &le; 0 </exception>
		/// <seealso cref=    #remainder </seealso>
		public virtual BigInteger Mod(BigInteger m)
		{
			if (m.Signum_Renamed <= 0)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}

			BigInteger result = this.Remainder(m);
			return (result.Signum_Renamed >= 0 ? result : result.Add(m));
		}

		/// <summary>
		/// Returns a BigInteger whose value is
		/// <tt>(this<sup>exponent</sup> mod m)</tt>.  (Unlike {@code pow}, this
		/// method permits negative exponents.)
		/// </summary>
		/// <param name="exponent"> the exponent. </param>
		/// <param name="m"> the modulus. </param>
		/// <returns> <tt>this<sup>exponent</sup> mod m</tt> </returns>
		/// <exception cref="ArithmeticException"> {@code m} &le; 0 or the exponent is
		///         negative and this BigInteger is not <i>relatively
		///         prime</i> to {@code m}. </exception>
		/// <seealso cref=    #modInverse </seealso>
		public virtual BigInteger ModPow(BigInteger exponent, BigInteger m)
		{
			if (m.Signum_Renamed <= 0)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}

			// Trivial cases
			if (exponent.Signum_Renamed == 0)
			{
				return (m.Equals(ONE) ? ZERO : ONE);
			}

			if (this.Equals(ONE))
			{
				return (m.Equals(ONE) ? ZERO : ONE);
			}

			if (this.Equals(ZERO) && exponent.Signum_Renamed >= 0)
			{
				return ZERO;
			}

			if (this.Equals(NegConst[1]) && (!exponent.TestBit(0)))
			{
				return (m.Equals(ONE) ? ZERO : ONE);
			}

			bool invertResult;
			if ((invertResult = (exponent.Signum_Renamed < 0)))
			{
				exponent = exponent.Negate();
			}

			BigInteger @base = (this.Signum_Renamed < 0 || this.CompareTo(m) >= 0 ? this.Mod(m) : this);
			BigInteger result;
			if (m.TestBit(0)) // odd modulus
			{
				result = @base.OddModPow(exponent, m);
			}
			else
			{
				/*
				 * Even modulus.  Tear it into an "odd part" (m1) and power of two
				 * (m2), exponentiate mod m1, manually exponentiate mod m2, and
				 * use Chinese Remainder Theorem to combine results.
				 */

				// Tear m apart into odd part (m1) and power of 2 (m2)
				int p = m.LowestSetBit; // Max pow of 2 that divides m

				BigInteger m1 = m.ShiftRight(p); // m/2**p
				BigInteger m2 = ONE.ShiftLeft(p); // 2**p

				// Calculate new base from m1
				BigInteger base2 = (this.Signum_Renamed < 0 || this.CompareTo(m1) >= 0 ? this.Mod(m1) : this);

				// Caculate (base ** exponent) mod m1.
				BigInteger a1 = (m1.Equals(ONE) ? ZERO : base2.OddModPow(exponent, m1));

				// Calculate (this ** exponent) mod m2
				BigInteger a2 = @base.ModPow2(exponent, p);

				// Combine results using Chinese Remainder Theorem
				BigInteger y1 = m2.ModInverse(m1);
				BigInteger y2 = m1.ModInverse(m2);

				if (m.Mag.Length < MAX_MAG_LENGTH / 2)
				{
					result = a1.Multiply(m2).Multiply(y1).Add(a2.Multiply(m1).Multiply(y2)).Mod(m);
				}
				else
				{
					MutableBigInteger t1 = new MutableBigInteger();
					(new MutableBigInteger(a1.Multiply(m2))).Multiply(new MutableBigInteger(y1), t1);
					MutableBigInteger t2 = new MutableBigInteger();
					(new MutableBigInteger(a2.Multiply(m1))).Multiply(new MutableBigInteger(y2), t2);
					t1.Add(t2);
					MutableBigInteger q = new MutableBigInteger();
					result = t1.Divide(new MutableBigInteger(m), q).ToBigInteger();
				}
			}

			return (invertResult ? result.ModInverse(m) : result);
		}

		internal static int[] BnExpModThreshTable = new int[] {7, 25, 81, 241, 673, 1793, Integer.MaxValue};

		/// <summary>
		/// Returns a BigInteger whose value is x to the power of y mod z.
		/// Assumes: z is odd && x < z.
		/// </summary>
		private BigInteger OddModPow(BigInteger y, BigInteger z)
		{
		/*
		 * The algorithm is adapted from Colin Plumb's C library.
		 *
		 * The window algorithm:
		 * The idea is to keep a running product of b1 = n^(high-order bits of exp)
		 * and then keep appending exponent bits to it.  The following patterns
		 * apply to a 3-bit window (k = 3):
		 * To append   0: square
		 * To append   1: square, multiply by n^1
		 * To append  10: square, multiply by n^1, square
		 * To append  11: square, square, multiply by n^3
		 * To append 100: square, multiply by n^1, square, square
		 * To append 101: square, square, square, multiply by n^5
		 * To append 110: square, square, multiply by n^3, square
		 * To append 111: square, square, square, multiply by n^7
		 *
		 * Since each pattern involves only one multiply, the longer the pattern
		 * the better, except that a 0 (no multiplies) can be appended directly.
		 * We precompute a table of odd powers of n, up to 2^k, and can then
		 * multiply k bits of exponent at a time.  Actually, assuming random
		 * exponents, there is on average one zero bit between needs to
		 * multiply (1/2 of the time there's none, 1/4 of the time there's 1,
		 * 1/8 of the time, there's 2, 1/32 of the time, there's 3, etc.), so
		 * you have to do one multiply per k+1 bits of exponent.
		 *
		 * The loop walks down the exponent, squaring the result buffer as
		 * it goes.  There is a wbits+1 bit lookahead buffer, buf, that is
		 * filled with the upcoming exponent bits.  (What is read after the
		 * end of the exponent is unimportant, but it is filled with zero here.)
		 * When the most-significant bit of this buffer becomes set, i.e.
		 * (buf & tblmask) != 0, we have to decide what pattern to multiply
		 * by, and when to do it.  We decide, remember to do it in future
		 * after a suitable number of squarings have passed (e.g. a pattern
		 * of "100" in the buffer requires that we multiply by n^1 immediately;
		 * a pattern of "110" calls for multiplying by n^3 after one more
		 * squaring), clear the buffer, and continue.
		 *
		 * When we start, there is one more optimization: the result buffer
		 * is implcitly one, so squaring it or multiplying by it can be
		 * optimized away.  Further, if we start with a pattern like "100"
		 * in the lookahead window, rather than placing n into the buffer
		 * and then starting to square it, we have already computed n^2
		 * to compute the odd-powers table, so we can place that into
		 * the buffer and save a squaring.
		 *
		 * This means that if you have a k-bit window, to compute n^z,
		 * where z is the high k bits of the exponent, 1/2 of the time
		 * it requires no squarings.  1/4 of the time, it requires 1
		 * squaring, ... 1/2^(k-1) of the time, it reqires k-2 squarings.
		 * And the remaining 1/2^(k-1) of the time, the top k bits are a
		 * 1 followed by k-1 0 bits, so it again only requires k-2
		 * squarings, not k-1.  The average of these is 1.  Add that
		 * to the one squaring we have to do to compute the table,
		 * and you'll see that a k-bit window saves k-2 squarings
		 * as well as reducing the multiplies.  (It actually doesn't
		 * hurt in the case k = 1, either.)
		 */
			// Special case for exponent of one
			if (y.Equals(ONE))
			{
				return this;
			}

			// Special case for base of zero
			if (Signum_Renamed == 0)
			{
				return ZERO;
			}

			int[] @base = Mag.clone();
			int[] exp = y.Mag;
			int[] mod = z.Mag;
			int modLen = mod.Length;

			// Select an appropriate window size
			int wbits = 0;
			int ebits = BitLength(exp, exp.Length);
			// if exponent is 65537 (0x10001), use minimum window size
			if ((ebits != 17) || (exp[0] != 65537))
			{
				while (ebits > BnExpModThreshTable[wbits])
				{
					wbits++;
				}
			}

			// Calculate appropriate table size
			int tblmask = 1 << wbits;

			// Allocate table for precomputed odd powers of base in Montgomery form
			int[][] table = new int[tblmask][];
			for (int i = 0; i < tblmask; i++)
			{
				table[i] = new int[modLen];
			}

			// Compute the modular inverse
			int inv = -MutableBigInteger.InverseMod32(mod[modLen - 1]);

			// Convert base to Montgomery form
			int[] a = LeftShift(@base, @base.Length, modLen << 5);

			MutableBigInteger q = new MutableBigInteger(), a2 = new MutableBigInteger(a), b2 = new MutableBigInteger(mod);

			MutableBigInteger r = a2.Divide(b2, q);
			table[0] = r.ToIntArray();

			// Pad table[0] with leading zeros so its length is at least modLen
			if (table[0].Length < modLen)
			{
			   int offset = modLen - table[0].Length;
			   int[] t2 = new int[modLen];
			   for (int i = 0; i < table[0].Length; i++)
			   {
				   t2[i + offset] = table[0][i];
			   }
			   table[0] = t2;
			}

			// Set b to the square of the base
			int[] b = SquareToLen(table[0], modLen, null);
			b = MontReduce(b, mod, modLen, inv);

			// Set t to high half of b
			int[] t = Arrays.CopyOf(b, modLen);

			// Fill in the table with odd powers of the base
			for (int i = 1; i < tblmask; i++)
			{
				int[] prod = MultiplyToLen(t, modLen, table[i - 1], modLen, null);
				table[i] = MontReduce(prod, mod, modLen, inv);
			}

			// Pre load the window that slides over the exponent
			int bitpos = 1 << ((ebits - 1) & (32 - 1));

			int buf = 0;
			int elen = exp.Length;
			int eIndex = 0;
			for (int i = 0; i <= wbits; i++)
			{
				buf = (buf << 1) | (((exp[eIndex] & bitpos) != 0)?1:0);
				bitpos = (int)((uint)bitpos >> 1);
				if (bitpos == 0)
				{
					eIndex++;
					bitpos = 1 << (32 - 1);
					elen--;
				}
			}

			int multpos = ebits;

			// The first iteration, which is hoisted out of the main loop
			ebits--;
			bool isone = true;

			multpos = ebits - wbits;
			while ((buf & 1) == 0)
			{
				buf = (int)((uint)buf >> 1);
				multpos++;
			}

			int[] mult = table[(int)((uint)buf >> 1)];

			buf = 0;
			if (multpos == ebits)
			{
				isone = false;
			}

			// The main loop
			while (true)
			{
				ebits--;
				// Advance the window
				buf <<= 1;

				if (elen != 0)
				{
					buf |= ((exp[eIndex] & bitpos) != 0) ? 1 : 0;
					bitpos = (int)((uint)bitpos >> 1);
					if (bitpos == 0)
					{
						eIndex++;
						bitpos = 1 << (32 - 1);
						elen--;
					}
				}

				// Examine the window for pending multiplies
				if ((buf & tblmask) != 0)
				{
					multpos = ebits - wbits;
					while ((buf & 1) == 0)
					{
						buf = (int)((uint)buf >> 1);
						multpos++;
					}
					mult = table[(int)((uint)buf >> 1)];
					buf = 0;
				}

				// Perform multiply
				if (ebits == multpos)
				{
					if (isone)
					{
						b = mult.clone();
						isone = false;
					}
					else
					{
						t = b;
						a = MultiplyToLen(t, modLen, mult, modLen, a);
						a = MontReduce(a, mod, modLen, inv);
						t = a;
						a = b;
						b = t;
					}
				}

				// Check if done
				if (ebits == 0)
				{
					break;
				}

				// Square the input
				if (!isone)
				{
					t = b;
					a = SquareToLen(t, modLen, a);
					a = MontReduce(a, mod, modLen, inv);
					t = a;
					a = b;
					b = t;
				}
			}

			// Convert result out of Montgomery form and return
			int[] t2 = new int[2 * modLen];
			System.Array.Copy(b, 0, t2, modLen, modLen);

			b = MontReduce(t2, mod, modLen, inv);

			t2 = Arrays.CopyOf(b, modLen);

			return new BigInteger(1, t2);
		}

		/// <summary>
		/// Montgomery reduce n, modulo mod.  This reduces modulo mod and divides
		/// by 2^(32*mlen). Adapted from Colin Plumb's C library.
		/// </summary>
		private static int[] MontReduce(int[] n, int[] mod, int mlen, int inv)
		{
			int c = 0;
			int len = mlen;
			int offset = 0;

			do
			{
				int nEnd = n[n.Length - 1 - offset];
				int carry = MulAdd(n, mod, offset, mlen, inv * nEnd);
				c += AddOne(n, offset, mlen, carry);
				offset++;
			} while (--len > 0);

			while (c > 0)
			{
				c += SubN(n, mod, mlen);
			}

			while (IntArrayCmpToLen(n, mod, mlen) >= 0)
			{
				SubN(n, mod, mlen);
			}

			return n;
		}


		/*
		 * Returns -1, 0 or +1 as big-endian unsigned int array arg1 is less than,
		 * equal to, or greater than arg2 up to length len.
		 */
		private static int IntArrayCmpToLen(int[] arg1, int[] arg2, int len)
		{
			for (int i = 0; i < len; i++)
			{
				long b1 = arg1[i] & LONG_MASK;
				long b2 = arg2[i] & LONG_MASK;
				if (b1 < b2)
				{
					return -1;
				}
				if (b1 > b2)
				{
					return 1;
				}
			}
			return 0;
		}

		/// <summary>
		/// Subtracts two numbers of same length, returning borrow.
		/// </summary>
		private static int SubN(int[] a, int[] b, int len)
		{
			long sum = 0;

			while (--len >= 0)
			{
				sum = (a[len] & LONG_MASK) - (b[len] & LONG_MASK) + (sum >> 32);
				a[len] = (int)sum;
			}

			return (int)(sum >> 32);
		}

		/// <summary>
		/// Multiply an array by one word k and add to result, return the carry
		/// </summary>
		internal static int MulAdd(int[] @out, int[] @in, int offset, int len, int k)
		{
			long kLong = k & LONG_MASK;
			long carry = 0;

			offset = @out.Length - offset - 1;
			for (int j = len - 1; j >= 0; j--)
			{
				long product = (@in[j] & LONG_MASK) * kLong + (@out[offset] & LONG_MASK) + carry;
				@out[offset--] = (int)product;
				carry = (long)((ulong)product >> 32);
			}
			return (int)carry;
		}

		/// <summary>
		/// Add one word to the number a mlen words into a. Return the resulting
		/// carry.
		/// </summary>
		internal static int AddOne(int[] a, int offset, int mlen, int carry)
		{
			offset = a.Length - 1 - mlen - offset;
			long t = (a[offset] & LONG_MASK) + (carry & LONG_MASK);

			a[offset] = (int)t;
			if (((long)((ulong)t >> 32)) == 0)
			{
				return 0;
			}
			while (--mlen >= 0)
			{
				if (--offset < 0) // Carry out of number
				{
					return 1;
				}
				else
				{
					a[offset]++;
					if (a[offset] != 0)
					{
						return 0;
					}
				}
			}
			return 1;
		}

		/// <summary>
		/// Returns a BigInteger whose value is (this ** exponent) mod (2**p)
		/// </summary>
		private BigInteger ModPow2(BigInteger exponent, int p)
		{
			/*
			 * Perform exponentiation using repeated squaring trick, chopping off
			 * high order bits as indicated by modulus.
			 */
			BigInteger result = ONE;
			BigInteger baseToPow2 = this.Mod2(p);
			int expOffset = 0;

			int limit = exponent.BitLength();

			if (this.TestBit(0))
			{
			   limit = (p - 1) < limit ? (p - 1) : limit;
			}

			while (expOffset < limit)
			{
				if (exponent.TestBit(expOffset))
				{
					result = result.Multiply(baseToPow2).Mod2(p);
				}
				expOffset++;
				if (expOffset < limit)
				{
					baseToPow2 = baseToPow2.Square().Mod2(p);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a BigInteger whose value is this mod(2**p).
		/// Assumes that this {@code BigInteger >= 0} and {@code p > 0}.
		/// </summary>
		private BigInteger Mod2(int p)
		{
			if (BitLength() <= p)
			{
				return this;
			}

			// Copy remaining ints of mag
			int numInts = (int)((uint)(p + 31) >> 5);
			int[] mag = new int[numInts];
			System.Array.Copy(this.Mag, (this.Mag.Length - numInts), mag, 0, numInts);

			// Mask out any excess bits
			int excessBits = (numInts << 5) - p;
			mag[0] &= (1L << (32 - excessBits)) - 1;

			return (mag[0] == 0 ? new BigInteger(1, mag) : new BigInteger(mag, 1));
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this}<sup>-1</sup> {@code mod m)}.
		/// </summary>
		/// <param name="m"> the modulus. </param>
		/// <returns> {@code this}<sup>-1</sup> {@code mod m}. </returns>
		/// <exception cref="ArithmeticException"> {@code  m} &le; 0, or this BigInteger
		///         has no multiplicative inverse mod m (that is, this BigInteger
		///         is not <i>relatively prime</i> to m). </exception>
		public virtual BigInteger ModInverse(BigInteger m)
		{
			if (m.Signum_Renamed != 1)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}

			if (m.Equals(ONE))
			{
				return ZERO;
			}

			// Calculate (this mod m)
			BigInteger modVal = this;
			if (Signum_Renamed < 0 || (this.CompareMagnitude(m) >= 0))
			{
				modVal = this.Mod(m);
			}

			if (modVal.Equals(ONE))
			{
				return ONE;
			}

			MutableBigInteger a = new MutableBigInteger(modVal);
			MutableBigInteger b = new MutableBigInteger(m);

			MutableBigInteger result = a.MutableModInverse(b);
			return result.ToBigInteger(1);
		}

		// Shift Operations

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this << n)}.
		/// The shift distance, {@code n}, may be negative, in which case
		/// this method performs a right shift.
		/// (Computes <tt>floor(this * 2<sup>n</sup>)</tt>.)
		/// </summary>
		/// <param name="n"> shift distance, in bits. </param>
		/// <returns> {@code this << n} </returns>
		/// <seealso cref= #shiftRight </seealso>
		public virtual BigInteger ShiftLeft(int n)
		{
			if (Signum_Renamed == 0)
			{
				return ZERO;
			}
			if (n > 0)
			{
				return new BigInteger(ShiftLeft(Mag, n), Signum_Renamed);
			}
			else if (n == 0)
			{
				return this;
			}
			else
			{
				// Possible int overflow in (-n) is not a trouble,
				// because shiftRightImpl considers its argument unsigned
				return ShiftRightImpl(-n);
			}
		}

		/// <summary>
		/// Returns a magnitude array whose value is {@code (mag << n)}.
		/// The shift distance, {@code n}, is considered unnsigned.
		/// (Computes <tt>this * 2<sup>n</sup></tt>.)
		/// </summary>
		/// <param name="mag"> magnitude, the most-significant int ({@code mag[0]}) must be non-zero. </param>
		/// <param name="n"> unsigned shift distance, in bits. </param>
		/// <returns> {@code mag << n} </returns>
		private static int[] ShiftLeft(int[] mag, int n)
		{
			int nInts = (int)((uint)n >> 5);
			int nBits = n & 0x1f;
			int magLen = mag.Length;
			int[] newMag = null;

			if (nBits == 0)
			{
				newMag = new int[magLen + nInts];
				System.Array.Copy(mag, 0, newMag, 0, magLen);
			}
			else
			{
				int i = 0;
				int nBits2 = 32 - nBits;
				int highBits = (int)((uint)mag[0] >> nBits2);
				if (highBits != 0)
				{
					newMag = new int[magLen + nInts + 1];
					newMag[i++] = highBits;
				}
				else
				{
					newMag = new int[magLen + nInts];
				}
				int j = 0;
				while (j < magLen - 1)
				{
					newMag[i++] = mag[j++] << nBits | (int)((uint)mag[j] >> nBits2);
				}
				newMag[i] = mag[j] << nBits;
			}
			return newMag;
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this >> n)}.  Sign
		/// extension is performed.  The shift distance, {@code n}, may be
		/// negative, in which case this method performs a left shift.
		/// (Computes <tt>floor(this / 2<sup>n</sup>)</tt>.)
		/// </summary>
		/// <param name="n"> shift distance, in bits. </param>
		/// <returns> {@code this >> n} </returns>
		/// <seealso cref= #shiftLeft </seealso>
		public virtual BigInteger ShiftRight(int n)
		{
			if (Signum_Renamed == 0)
			{
				return ZERO;
			}
			if (n > 0)
			{
				return ShiftRightImpl(n);
			}
			else if (n == 0)
			{
				return this;
			}
			else
			{
				// Possible int overflow in {@code -n} is not a trouble,
				// because shiftLeft considers its argument unsigned
				return new BigInteger(ShiftLeft(Mag, -n), Signum_Renamed);
			}
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this >> n)}. The shift
		/// distance, {@code n}, is considered unsigned.
		/// (Computes <tt>floor(this * 2<sup>-n</sup>)</tt>.)
		/// </summary>
		/// <param name="n"> unsigned shift distance, in bits. </param>
		/// <returns> {@code this >> n} </returns>
		private BigInteger ShiftRightImpl(int n)
		{
			int nInts = (int)((uint)n >> 5);
			int nBits = n & 0x1f;
			int magLen = Mag.Length;
			int[] newMag = null;

			// Special case: entire contents shifted off the end
			if (nInts >= magLen)
			{
				return (Signum_Renamed >= 0 ? ZERO : NegConst[1]);
			}

			if (nBits == 0)
			{
				int newMagLen = magLen - nInts;
				newMag = Arrays.CopyOf(Mag, newMagLen);
			}
			else
			{
				int i = 0;
				int highBits = (int)((uint)Mag[0] >> nBits);
				if (highBits != 0)
				{
					newMag = new int[magLen - nInts];
					newMag[i++] = highBits;
				}
				else
				{
					newMag = new int[magLen - nInts - 1];
				}

				int nBits2 = 32 - nBits;
				int j = 0;
				while (j < magLen - nInts - 1)
				{
					newMag[i++] = (Mag[j++] << nBits2) | ((int)((uint)Mag[j] >> nBits));
				}
			}

			if (Signum_Renamed < 0)
			{
				// Find out whether any one-bits were shifted off the end.
				bool onesLost = false;
				for (int i = magLen - 1, j = magLen - nInts; i >= j && !onesLost; i--)
				{
					onesLost = (Mag[i] != 0);
				}
				if (!onesLost && nBits != 0)
				{
					onesLost = (Mag[magLen - nInts - 1] << (32 - nBits) != 0);
				}

				if (onesLost)
				{
					newMag = JavaIncrement(newMag);
				}
			}

			return new BigInteger(newMag, Signum_Renamed);
		}

		internal virtual int[] JavaIncrement(int[] val)
		{
			int lastSum = 0;
			for (int i = val.Length - 1; i >= 0 && lastSum == 0; i--)
			{
				lastSum = (val[i] += 1);
			}
			if (lastSum == 0)
			{
				val = new int[val.Length + 1];
				val[0] = 1;
			}
			return val;
		}

		// Bitwise Operations

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this & val)}.  (This
		/// method returns a negative BigInteger if and only if this and val are
		/// both negative.)
		/// </summary>
		/// <param name="val"> value to be AND'ed with this BigInteger. </param>
		/// <returns> {@code this & val} </returns>
		public virtual BigInteger And(BigInteger val)
		{
			int[] result = new int[System.Math.Max(IntLength(), val.IntLength())];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (GetInt(result.Length - i - 1) & val.GetInt(result.Length - i - 1));
			}

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this | val)}.  (This method
		/// returns a negative BigInteger if and only if either this or val is
		/// negative.)
		/// </summary>
		/// <param name="val"> value to be OR'ed with this BigInteger. </param>
		/// <returns> {@code this | val} </returns>
		public virtual BigInteger Or(BigInteger val)
		{
			int[] result = new int[System.Math.Max(IntLength(), val.IntLength())];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (GetInt(result.Length - i - 1) | val.GetInt(result.Length - i - 1));
			}

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this ^ val)}.  (This method
		/// returns a negative BigInteger if and only if exactly one of this and
		/// val are negative.)
		/// </summary>
		/// <param name="val"> value to be XOR'ed with this BigInteger. </param>
		/// <returns> {@code this ^ val} </returns>
		public virtual BigInteger Xor(BigInteger val)
		{
			int[] result = new int[System.Math.Max(IntLength(), val.IntLength())];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (GetInt(result.Length - i - 1) ^ val.GetInt(result.Length - i - 1));
			}

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (~this)}.  (This method
		/// returns a negative value if and only if this BigInteger is
		/// non-negative.)
		/// </summary>
		/// <returns> {@code ~this} </returns>
		public virtual BigInteger Not()
		{
			int[] result = new int[IntLength()];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = ~GetInt(result.Length - i - 1);
			}

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is {@code (this & ~val)}.  This
		/// method, which is equivalent to {@code and(val.not())}, is provided as
		/// a convenience for masking operations.  (This method returns a negative
		/// BigInteger if and only if {@code this} is negative and {@code val} is
		/// positive.)
		/// </summary>
		/// <param name="val"> value to be complemented and AND'ed with this BigInteger. </param>
		/// <returns> {@code this & ~val} </returns>
		public virtual BigInteger AndNot(BigInteger val)
		{
			int[] result = new int[System.Math.Max(IntLength(), val.IntLength())];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (GetInt(result.Length - i - 1) & ~val.GetInt(result.Length - i - 1));
			}

			return ValueOf(result);
		}


		// Single Bit Operations

		/// <summary>
		/// Returns {@code true} if and only if the designated bit is set.
		/// (Computes {@code ((this & (1<<n)) != 0)}.)
		/// </summary>
		/// <param name="n"> index of bit to test. </param>
		/// <returns> {@code true} if and only if the designated bit is set. </returns>
		/// <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		public virtual bool TestBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}

			return (GetInt((int)((uint)n >> 5)) & (1 << (n & 31))) != 0;
		}

		/// <summary>
		/// Returns a BigInteger whose value is equivalent to this BigInteger
		/// with the designated bit set.  (Computes {@code (this | (1<<n))}.)
		/// </summary>
		/// <param name="n"> index of bit to set. </param>
		/// <returns> {@code this | (1<<n)} </returns>
		/// <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		public virtual BigInteger SetBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}

			int intNum = (int)((uint)n >> 5);
			int[] result = new int[System.Math.Max(IntLength(), intNum + 2)];

			for (int i = 0; i < result.Length; i++)
			{
				result[result.Length - i - 1] = GetInt(i);
			}

			result[result.Length - intNum - 1] |= (1 << (n & 31));

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is equivalent to this BigInteger
		/// with the designated bit cleared.
		/// (Computes {@code (this & ~(1<<n))}.)
		/// </summary>
		/// <param name="n"> index of bit to clear. </param>
		/// <returns> {@code this & ~(1<<n)} </returns>
		/// <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		public virtual BigInteger ClearBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}

			int intNum = (int)((uint)n >> 5);
			int[] result = new int[System.Math.Max(IntLength(), ((int)((uint)(n + 1) >> 5)) + 1)];

			for (int i = 0; i < result.Length; i++)
			{
				result[result.Length - i - 1] = GetInt(i);
			}

			result[result.Length - intNum - 1] &= ~(1 << (n & 31));

			return ValueOf(result);
		}

		/// <summary>
		/// Returns a BigInteger whose value is equivalent to this BigInteger
		/// with the designated bit flipped.
		/// (Computes {@code (this ^ (1<<n))}.)
		/// </summary>
		/// <param name="n"> index of bit to flip. </param>
		/// <returns> {@code this ^ (1<<n)} </returns>
		/// <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		public virtual BigInteger FlipBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}

			int intNum = (int)((uint)n >> 5);
			int[] result = new int[System.Math.Max(IntLength(), intNum + 2)];

			for (int i = 0; i < result.Length; i++)
			{
				result[result.Length - i - 1] = GetInt(i);
			}

			result[result.Length - intNum - 1] ^= (1 << (n & 31));

			return ValueOf(result);
		}

		/// <summary>
		/// Returns the index of the rightmost (lowest-order) one bit in this
		/// BigInteger (the number of zero bits to the right of the rightmost
		/// one bit).  Returns -1 if this BigInteger contains no one bits.
		/// (Computes {@code (this == 0? -1 : log2(this & -this))}.)
		/// </summary>
		/// <returns> index of the rightmost one bit in this BigInteger. </returns>
		public virtual int LowestSetBit
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("deprecation") int lsb = lowestSetBit - 2;
				int lsb = LowestSetBit_Renamed - 2;
				if (lsb == -2) // lowestSetBit not initialized yet
				{
					lsb = 0;
					if (Signum_Renamed == 0)
					{
						lsb -= 1;
					}
					else
					{
						// Search for lowest order nonzero int
						int i, b;
						for (i = 0; (b = GetInt(i)) == 0; i++)
						{
							;
						}
						lsb += (i << 5) + Integer.NumberOfTrailingZeros(b);
					}
					LowestSetBit_Renamed = lsb + 2;
				}
				return lsb;
			}
		}


		// Miscellaneous Bit Operations

		/// <summary>
		/// Returns the number of bits in the minimal two's-complement
		/// representation of this BigInteger, <i>excluding</i> a sign bit.
		/// For positive BigIntegers, this is equivalent to the number of bits in
		/// the ordinary binary representation.  (Computes
		/// {@code (ceil(log2(this < 0 ? -this : this+1)))}.)
		/// </summary>
		/// <returns> number of bits in the minimal two's-complement
		///         representation of this BigInteger, <i>excluding</i> a sign bit. </returns>
		public virtual int BitLength()
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") int n = bitLength - 1;
			int n = BitLength_Renamed - 1;
			if (n == -1) // bitLength not initialized yet
			{
				int[] m = Mag;
				int len = m.Length;
				if (len == 0)
				{
					n = 0; // offset by one to initialize
				}
				else
				{
					// Calculate the bit length of the magnitude
					int magBitLength = ((len - 1) << 5) + BitLengthForInt(Mag[0]);
					 if (Signum_Renamed < 0)
					 {
						 // Check if magnitude is a power of two
						 bool pow2 = (Integer.BitCount(Mag[0]) == 1);
						 for (int i = 1; i < len && pow2; i++)
						 {
							 pow2 = (Mag[i] == 0);
						 }

						 n = (pow2 ? magBitLength - 1 : magBitLength);
					 }
					 else
					 {
						 n = magBitLength;
					 }
				}
				BitLength_Renamed = n + 1;
			}
			return n;
		}

		/// <summary>
		/// Returns the number of bits in the two's complement representation
		/// of this BigInteger that differ from its sign bit.  This method is
		/// useful when implementing bit-vector style sets atop BigIntegers.
		/// </summary>
		/// <returns> number of bits in the two's complement representation
		///         of this BigInteger that differ from its sign bit. </returns>
		public virtual int BitCount()
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") int bc = bitCount - 1;
			int bc = BitCount_Renamed - 1;
			if (bc == -1) // bitCount not initialized yet
			{
				bc = 0; // offset by one to initialize
				// Count the bits in the magnitude
				for (int i = 0; i < Mag.Length; i++)
				{
					bc += Integer.BitCount(Mag[i]);
				}
				if (Signum_Renamed < 0)
				{
					// Count the trailing zeros in the magnitude
					int magTrailingZeroCount = 0, j ;
					for (j = Mag.Length - 1; Mag[j] == 0; j--)
					{
						magTrailingZeroCount += 32;
					}
					magTrailingZeroCount += Integer.NumberOfTrailingZeros(Mag[j]);
					bc += magTrailingZeroCount - 1;
				}
				BitCount_Renamed = bc + 1;
			}
			return bc;
		}

		// Primality Testing

		/// <summary>
		/// Returns {@code true} if this BigInteger is probably prime,
		/// {@code false} if it's definitely composite.  If
		/// {@code certainty} is &le; 0, {@code true} is
		/// returned.
		/// </summary>
		/// <param name="certainty"> a measure of the uncertainty that the caller is
		///         willing to tolerate: if the call returns {@code true}
		///         the probability that this BigInteger is prime exceeds
		///         (1 - 1/2<sup>{@code certainty}</sup>).  The execution time of
		///         this method is proportional to the value of this parameter. </param>
		/// <returns> {@code true} if this BigInteger is probably prime,
		///         {@code false} if it's definitely composite. </returns>
		public virtual bool IsProbablePrime(int certainty)
		{
			if (certainty <= 0)
			{
				return true;
			}
			BigInteger w = this.Abs();
			if (w.Equals(TWO))
			{
				return true;
			}
			if (!w.TestBit(0) || w.Equals(ONE))
			{
				return false;
			}

			return w.PrimeToCertainty(certainty, null);
		}

		// Comparison Operations

		/// <summary>
		/// Compares this BigInteger with the specified BigInteger.  This
		/// method is provided in preference to individual methods for each
		/// of the six boolean comparison operators ({@literal <}, ==,
		/// {@literal >}, {@literal >=}, !=, {@literal <=}).  The suggested
		/// idiom for performing these comparisons is: {@code
		/// (x.compareTo(y)} &lt;<i>op</i>&gt; {@code 0)}, where
		/// &lt;<i>op</i>&gt; is one of the six comparison operators.
		/// </summary>
		/// <param name="val"> BigInteger to which this BigInteger is to be compared. </param>
		/// <returns> -1, 0 or 1 as this BigInteger is numerically less than, equal
		///         to, or greater than {@code val}. </returns>
		public virtual int CompareTo(BigInteger val)
		{
			if (Signum_Renamed == val.Signum_Renamed)
			{
				switch (Signum_Renamed)
				{
				case 1:
					return CompareMagnitude(val);
				case -1:
					return val.CompareMagnitude(this);
				default:
					return 0;
				}
			}
			return Signum_Renamed > val.Signum_Renamed ? 1 : -1;
		}

		/// <summary>
		/// Compares the magnitude array of this BigInteger with the specified
		/// BigInteger's. This is the version of compareTo ignoring sign.
		/// </summary>
		/// <param name="val"> BigInteger whose magnitude array to be compared. </param>
		/// <returns> -1, 0 or 1 as this magnitude array is less than, equal to or
		///         greater than the magnitude aray for the specified BigInteger's. </returns>
		internal int CompareMagnitude(BigInteger val)
		{
			int[] m1 = Mag;
			int len1 = m1.Length;
			int[] m2 = val.Mag;
			int len2 = m2.Length;
			if (len1 < len2)
			{
				return -1;
			}
			if (len1 > len2)
			{
				return 1;
			}
			for (int i = 0; i < len1; i++)
			{
				int a = m1[i];
				int b = m2[i];
				if (a != b)
				{
					return ((a & LONG_MASK) < (b & LONG_MASK)) ? - 1 : 1;
				}
			}
			return 0;
		}

		/// <summary>
		/// Version of compareMagnitude that compares magnitude with long value.
		/// val can't be Long.MIN_VALUE.
		/// </summary>
		internal int CompareMagnitude(long val)
		{
			Debug.Assert(val != Long.MinValue);
			int[] m1 = Mag;
			int len = m1.Length;
			if (len > 2)
			{
				return 1;
			}
			if (val < 0)
			{
				val = -val;
			}
			int highWord = (int)((long)((ulong)val >> 32));
			if (highWord == 0)
			{
				if (len < 1)
				{
					return -1;
				}
				if (len > 1)
				{
					return 1;
				}
				int a = m1[0];
				int b = (int)val;
				if (a != b)
				{
					return ((a & LONG_MASK) < (b & LONG_MASK))? - 1 : 1;
				}
				return 0;
			}
			else
			{
				if (len < 2)
				{
					return -1;
				}
				int a = m1[0];
				int b = highWord;
				if (a != b)
				{
					return ((a & LONG_MASK) < (b & LONG_MASK))? - 1 : 1;
				}
				a = m1[1];
				b = (int)val;
				if (a != b)
				{
					return ((a & LONG_MASK) < (b & LONG_MASK))? - 1 : 1;
				}
				return 0;
			}
		}

		/// <summary>
		/// Compares this BigInteger with the specified Object for equality.
		/// </summary>
		/// <param name="x"> Object to which this BigInteger is to be compared. </param>
		/// <returns> {@code true} if and only if the specified Object is a
		///         BigInteger whose value is numerically equal to this BigInteger. </returns>
		public override bool Equals(Object x)
		{
			// This test is just an optimization, which may or may not help
			if (x == this)
			{
				return true;
			}

			if (!(x is BigInteger))
			{
				return false;
			}

			BigInteger xInt = (BigInteger) x;
			if (xInt.Signum_Renamed != Signum_Renamed)
			{
				return false;
			}

			int[] m = Mag;
			int len = m.Length;
			int[] xm = xInt.Mag;
			if (len != xm.Length)
			{
				return false;
			}

			for (int i = 0; i < len; i++)
			{
				if (xm[i] != m[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns the minimum of this BigInteger and {@code val}.
		/// </summary>
		/// <param name="val"> value with which the minimum is to be computed. </param>
		/// <returns> the BigInteger whose value is the lesser of this BigInteger and
		///         {@code val}.  If they are equal, either may be returned. </returns>
		public virtual BigInteger Min(BigInteger val)
		{
			return (CompareTo(val) < 0 ? this : val);
		}

		/// <summary>
		/// Returns the maximum of this BigInteger and {@code val}.
		/// </summary>
		/// <param name="val"> value with which the maximum is to be computed. </param>
		/// <returns> the BigInteger whose value is the greater of this and
		///         {@code val}.  If they are equal, either may be returned. </returns>
		public virtual BigInteger Max(BigInteger val)
		{
			return (CompareTo(val) > 0 ? this : val);
		}


		// Hash Function

		/// <summary>
		/// Returns the hash code for this BigInteger.
		/// </summary>
		/// <returns> hash code for this BigInteger. </returns>
		public override int HashCode()
		{
			int hashCode = 0;

			for (int i = 0; i < Mag.Length; i++)
			{
				hashCode = (int)(31 * hashCode + (Mag[i] & LONG_MASK));
			}

			return hashCode * Signum_Renamed;
		}

		/// <summary>
		/// Returns the String representation of this BigInteger in the
		/// given radix.  If the radix is outside the range from {@link
		/// Character#MIN_RADIX} to <seealso cref="Character#MAX_RADIX"/> inclusive,
		/// it will default to 10 (as is the case for
		/// {@code Integer.toString}).  The digit-to-character mapping
		/// provided by {@code Character.forDigit} is used, and a minus
		/// sign is prepended if appropriate.  (This representation is
		/// compatible with the {@link #BigInteger(String, int) (String,
		/// int)} constructor.)
		/// </summary>
		/// <param name="radix">  radix of the String representation. </param>
		/// <returns> String representation of this BigInteger in the given radix. </returns>
		/// <seealso cref=    Integer#toString </seealso>
		/// <seealso cref=    Character#forDigit </seealso>
		/// <seealso cref=    #BigInteger(java.lang.String, int) </seealso>
		public virtual String ToString(int radix)
		{
			if (Signum_Renamed == 0)
			{
				return "0";
			}
			if (radix < Character.MIN_RADIX || radix > Character.MAX_RADIX)
			{
				radix = 10;
			}

			// If it's small enough, use smallToString.
			if (Mag.Length <= SCHOENHAGE_BASE_CONVERSION_THRESHOLD)
			{
			   return SmallToString(radix);
			}

			// Otherwise use recursive toString, which requires positive arguments.
			// The results will be concatenated into this StringBuilder
			StringBuilder sb = new StringBuilder();
			if (Signum_Renamed < 0)
			{
				ToString(this.Negate(), sb, radix, 0);
				sb.Insert(0, '-');
			}
			else
			{
				ToString(this, sb, radix, 0);
			}

			return sb.ToString();
		}

		/// <summary>
		/// This method is used to perform toString when arguments are small. </summary>
		private String SmallToString(int radix)
		{
			if (Signum_Renamed == 0)
			{
				return "0";
			}

			// Compute upper bound on number of digit groups and allocate space
			int maxNumDigitGroups = (4 * Mag.Length + 6) / 7;
			String[] digitGroup = new String[maxNumDigitGroups];

			// Translate number to string, a digit group at a time
			BigInteger tmp = this.Abs();
			int numGroups = 0;
			while (tmp.Signum_Renamed != 0)
			{
				BigInteger d = LongRadix[radix];

				MutableBigInteger q = new MutableBigInteger(), a = new MutableBigInteger(tmp.Mag), b = new MutableBigInteger(d.Mag);
				MutableBigInteger r = a.Divide(b, q);
				BigInteger q2 = q.ToBigInteger(tmp.Signum_Renamed * d.Signum_Renamed);
				BigInteger r2 = r.ToBigInteger(tmp.Signum_Renamed * d.Signum_Renamed);

				digitGroup[numGroups++] = Convert.ToString(r2.LongValue(), radix);
				tmp = q2;
			}

			// Put sign (if any) and first digit group into result buffer
			StringBuilder buf = new StringBuilder(numGroups * DigitsPerLong[radix] + 1);
			if (Signum_Renamed < 0)
			{
				buf.Append('-');
			}
			buf.Append(digitGroup[numGroups - 1]);

			// Append remaining digit groups padded with leading zeros
			for (int i = numGroups - 2; i >= 0; i--)
			{
				// Prepend (any) leading zeros for this digit group
				int numLeadingZeros = DigitsPerLong[radix] - digitGroup[i].Length();
				if (numLeadingZeros != 0)
				{
					buf.Append(Zeros[numLeadingZeros]);
				}
				buf.Append(digitGroup[i]);
			}
			return buf.ToString();
		}

		/// <summary>
		/// Converts the specified BigInteger to a string and appends to
		/// {@code sb}.  This implements the recursive Schoenhage algorithm
		/// for base conversions.
		/// <p/>
		/// See Knuth, Donald,  _The Art of Computer Programming_, Vol. 2,
		/// Answers to Exercises (4.4) Question 14.
		/// </summary>
		/// <param name="u">      The number to convert to a string. </param>
		/// <param name="sb">     The StringBuilder that will be appended to in place. </param>
		/// <param name="radix">  The base to convert to. </param>
		/// <param name="digits"> The minimum number of digits to pad to. </param>
		private static void ToString(BigInteger u, StringBuilder sb, int radix, int digits)
		{
			/* If we're smaller than a certain threshold, use the smallToString
			   method, padding with leading zeroes when necessary. */
			if (u.Mag.Length <= SCHOENHAGE_BASE_CONVERSION_THRESHOLD)
			{
				String s = u.SmallToString(radix);

				// Pad with internal zeros if necessary.
				// Don't pad if we're at the beginning of the string.
				if ((s.Length() < digits) && (sb.Length() > 0))
				{
					for (int i = s.Length(); i < digits; i++) // May be a faster way to
					{
						sb.Append('0'); // do this?
					}
				}

				sb.Append(s);
				return;
			}

			int b, n;
			b = u.BitLength();

			// Calculate a value for n in the equation radix^(2^n) = u
			// and subtract 1 from that value.  This is used to find the
			// cache index that contains the best value to divide u.
			n = (int) System.Math.Round(System.Math.Log(b * LOG_TWO / LogCache[radix]) / LOG_TWO - 1.0);
			BigInteger v = GetRadixConversionCache(radix, n);
			BigInteger[] results;
			results = u.DivideAndRemainder(v);

			int expectedDigits = 1 << n;

			// Now recursively build the two halves of each number.
			ToString(results[0], sb, radix, digits - expectedDigits);
			ToString(results[1], sb, radix, expectedDigits);
		}

		/// <summary>
		/// Returns the value radix^(2^exponent) from the cache.
		/// If this value doesn't already exist in the cache, it is added.
		/// <p/>
		/// This could be changed to a more complicated caching method using
		/// {@code Future}.
		/// </summary>
		private static BigInteger GetRadixConversionCache(int radix, int exponent)
		{
			BigInteger[] cacheLine = PowerCache[radix]; // volatile read
			if (exponent < cacheLine.Length)
			{
				return cacheLine[exponent];
			}

			int oldLength = cacheLine.Length;
			cacheLine = Arrays.CopyOf(cacheLine, exponent + 1);
			for (int i = oldLength; i <= exponent; i++)
			{
				cacheLine[i] = cacheLine[i - 1].Pow(2);
			}

			BigInteger[][] pc = PowerCache; // volatile read again
			if (exponent >= pc[radix].Length)
			{
				pc = pc.clone();
				pc[radix] = cacheLine;
				PowerCache = pc; // volatile write, publish
			}
			return cacheLine[exponent];
		}

		/* zero[i] is a string of i consecutive zeros. */
		private static String[] Zeros = new String[64];

		/// <summary>
		/// Returns the decimal String representation of this BigInteger.
		/// The digit-to-character mapping provided by
		/// {@code Character.forDigit} is used, and a minus sign is
		/// prepended if appropriate.  (This representation is compatible
		/// with the <seealso cref="#BigInteger(String) (String)"/> constructor, and
		/// allows for String concatenation with Java's + operator.)
		/// </summary>
		/// <returns> decimal String representation of this BigInteger. </returns>
		/// <seealso cref=    Character#forDigit </seealso>
		/// <seealso cref=    #BigInteger(java.lang.String) </seealso>
		public override String ToString()
		{
			return ToString(10);
		}

		/// <summary>
		/// Returns a byte array containing the two's-complement
		/// representation of this BigInteger.  The byte array will be in
		/// <i>big-endian</i> byte-order: the most significant byte is in
		/// the zeroth element.  The array will contain the minimum number
		/// of bytes required to represent this BigInteger, including at
		/// least one sign bit, which is {@code (ceil((this.bitLength() +
		/// 1)/8))}.  (This representation is compatible with the
		/// <seealso cref="#BigInteger(byte[]) (byte[])"/> constructor.)
		/// </summary>
		/// <returns> a byte array containing the two's-complement representation of
		///         this BigInteger. </returns>
		/// <seealso cref=    #BigInteger(byte[]) </seealso>
		public virtual sbyte[] ToByteArray()
		{
			int byteLen = BitLength() / 8 + 1;
			sbyte[] byteArray = new sbyte[byteLen];

			for (int i = byteLen - 1, bytesCopied = 4, nextInt = 0, intIndex = 0; i >= 0; i--)
			{
				if (bytesCopied == 4)
				{
					nextInt = GetInt(intIndex++);
					bytesCopied = 1;
				}
				else
				{
					nextInt = (int)((uint)nextInt >> 8);
					bytesCopied++;
				}
				byteArray[i] = (sbyte)nextInt;
			}
			return byteArray;
		}

		/// <summary>
		/// Converts this BigInteger to an {@code int}.  This
		/// conversion is analogous to a
		/// <i>narrowing primitive conversion</i> from {@code long} to
		/// {@code int} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this BigInteger is too big to fit in an
		/// {@code int}, only the low-order 32 bits are returned.
		/// Note that this conversion can lose information about the
		/// overall magnitude of the BigInteger value as well as return a
		/// result with the opposite sign.
		/// </summary>
		/// <returns> this BigInteger converted to an {@code int}. </returns>
		/// <seealso cref= #intValueExact() </seealso>
		public override int IntValue()
		{
			int result = 0;
			result = GetInt(0);
			return result;
		}

		/// <summary>
		/// Converts this BigInteger to a {@code long}.  This
		/// conversion is analogous to a
		/// <i>narrowing primitive conversion</i> from {@code long} to
		/// {@code int} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this BigInteger is too big to fit in a
		/// {@code long}, only the low-order 64 bits are returned.
		/// Note that this conversion can lose information about the
		/// overall magnitude of the BigInteger value as well as return a
		/// result with the opposite sign.
		/// </summary>
		/// <returns> this BigInteger converted to a {@code long}. </returns>
		/// <seealso cref= #longValueExact() </seealso>
		public override long LongValue()
		{
			long result = 0;

			for (int i = 1; i >= 0; i--)
			{
				result = (result << 32) + (GetInt(i) & LONG_MASK);
			}
			return result;
		}

		/// <summary>
		/// Converts this BigInteger to a {@code float}.  This
		/// conversion is similar to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code float} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this BigInteger has too great a magnitude
		/// to represent as a {@code float}, it will be converted to
		/// <seealso cref="Float#NEGATIVE_INFINITY"/> or {@link
		/// Float#POSITIVE_INFINITY} as appropriate.  Note that even when
		/// the return value is finite, this conversion can lose
		/// information about the precision of the BigInteger value.
		/// </summary>
		/// <returns> this BigInteger converted to a {@code float}. </returns>
		public override float FloatValue()
		{
			if (Signum_Renamed == 0)
			{
				return 0.0f;
			}

			int exponent = ((Mag.Length - 1) << 5) + BitLengthForInt(Mag[0]) - 1;

			// exponent == floor(log2(abs(this)))
			if (exponent < sizeof(long) - 1)
			{
				return LongValue();
			}
			else if (exponent > Float.MAX_EXPONENT)
			{
				return Signum_Renamed > 0 ? Float.PositiveInfinity : Float.NegativeInfinity;
			}

			/*
			 * We need the top SIGNIFICAND_WIDTH bits, including the "implicit"
			 * one bit. To make rounding easier, we pick out the top
			 * SIGNIFICAND_WIDTH + 1 bits, so we have one to help us round up or
			 * down. twiceSignifFloor will contain the top SIGNIFICAND_WIDTH + 1
			 * bits, and signifFloor the top SIGNIFICAND_WIDTH.
			 *
			 * It helps to consider the real number signif = abs(this) *
			 * 2^(SIGNIFICAND_WIDTH - 1 - exponent).
			 */
			int shift = exponent - FloatConsts.SIGNIFICAND_WIDTH;

			int twiceSignifFloor;
			// twiceSignifFloor will be == abs().shiftRight(shift).intValue()
			// We do the shift into an int directly to improve performance.

			int nBits = shift & 0x1f;
			int nBits2 = 32 - nBits;

			if (nBits == 0)
			{
				twiceSignifFloor = Mag[0];
			}
			else
			{
				twiceSignifFloor = (int)((uint)Mag[0] >> nBits);
				if (twiceSignifFloor == 0)
				{
					twiceSignifFloor = (Mag[0] << nBits2) | ((int)((uint)Mag[1] >> nBits));
				}
			}

			int signifFloor = twiceSignifFloor >> 1;
			signifFloor &= FloatConsts.SIGNIF_BIT_MASK; // remove the implied bit

			/*
			 * We round up if either the fractional part of signif is strictly
			 * greater than 0.5 (which is true if the 0.5 bit is set and any lower
			 * bit is set), or if the fractional part of signif is >= 0.5 and
			 * signifFloor is odd (which is true if both the 0.5 bit and the 1 bit
			 * are set). This is equivalent to the desired HALF_EVEN rounding.
			 */
			bool increment = (twiceSignifFloor & 1) != 0 && ((signifFloor & 1) != 0 || Abs().LowestSetBit < shift);
			int signifRounded = increment ? signifFloor + 1 : signifFloor;
			int bits = ((exponent + FloatConsts.EXP_BIAS)) << (FloatConsts.SIGNIFICAND_WIDTH - 1);
			bits += signifRounded;
			/*
			 * If signifRounded == 2^24, we'd need to set all of the significand
			 * bits to zero and add 1 to the exponent. This is exactly the behavior
			 * we get from just adding signifRounded to bits directly. If the
			 * exponent is Float.MAX_EXPONENT, we round up (correctly) to
			 * Float.POSITIVE_INFINITY.
			 */
			bits |= Signum_Renamed & FloatConsts.SIGN_BIT_MASK;
			return Float.intBitsToFloat(bits);
		}

		/// <summary>
		/// Converts this BigInteger to a {@code double}.  This
		/// conversion is similar to the
		/// <i>narrowing primitive conversion</i> from {@code double} to
		/// {@code float} as defined in section 5.1.3 of
		/// <cite>The Java&trade; Language Specification</cite>:
		/// if this BigInteger has too great a magnitude
		/// to represent as a {@code double}, it will be converted to
		/// <seealso cref="Double#NEGATIVE_INFINITY"/> or {@link
		/// Double#POSITIVE_INFINITY} as appropriate.  Note that even when
		/// the return value is finite, this conversion can lose
		/// information about the precision of the BigInteger value.
		/// </summary>
		/// <returns> this BigInteger converted to a {@code double}. </returns>
		public override double DoubleValue()
		{
			if (Signum_Renamed == 0)
			{
				return 0.0;
			}

			int exponent = ((Mag.Length - 1) << 5) + BitLengthForInt(Mag[0]) - 1;

			// exponent == floor(log2(abs(this))Double)
			if (exponent < sizeof(long) - 1)
			{
				return LongValue();
			}
			else if (exponent > Double.MAX_EXPONENT)
			{
				return Signum_Renamed > 0 ? Double.PositiveInfinity : Double.NegativeInfinity;
			}

			/*
			 * We need the top SIGNIFICAND_WIDTH bits, including the "implicit"
			 * one bit. To make rounding easier, we pick out the top
			 * SIGNIFICAND_WIDTH + 1 bits, so we have one to help us round up or
			 * down. twiceSignifFloor will contain the top SIGNIFICAND_WIDTH + 1
			 * bits, and signifFloor the top SIGNIFICAND_WIDTH.
			 *
			 * It helps to consider the real number signif = abs(this) *
			 * 2^(SIGNIFICAND_WIDTH - 1 - exponent).
			 */
			int shift = exponent - DoubleConsts.SIGNIFICAND_WIDTH;

			long twiceSignifFloor;
			// twiceSignifFloor will be == abs().shiftRight(shift).longValue()
			// We do the shift into a long directly to improve performance.

			int nBits = shift & 0x1f;
			int nBits2 = 32 - nBits;

			int highBits;
			int lowBits;
			if (nBits == 0)
			{
				highBits = Mag[0];
				lowBits = Mag[1];
			}
			else
			{
				highBits = (int)((uint)Mag[0] >> nBits);
				lowBits = (Mag[0] << nBits2) | ((int)((uint)Mag[1] >> nBits));
				if (highBits == 0)
				{
					highBits = lowBits;
					lowBits = (Mag[1] << nBits2) | ((int)((uint)Mag[2] >> nBits));
				}
			}

			twiceSignifFloor = ((highBits & LONG_MASK) << 32) | (lowBits & LONG_MASK);

			long signifFloor = twiceSignifFloor >> 1;
			signifFloor &= DoubleConsts.SIGNIF_BIT_MASK; // remove the implied bit

			/*
			 * We round up if either the fractional part of signif is strictly
			 * greater than 0.5 (which is true if the 0.5 bit is set and any lower
			 * bit is set), or if the fractional part of signif is >= 0.5 and
			 * signifFloor is odd (which is true if both the 0.5 bit and the 1 bit
			 * are set). This is equivalent to the desired HALF_EVEN rounding.
			 */
			bool increment = (twiceSignifFloor & 1) != 0 && ((signifFloor & 1) != 0 || Abs().LowestSetBit < shift);
			long signifRounded = increment ? signifFloor + 1 : signifFloor;
			long bits = (long)((exponent + DoubleConsts.EXP_BIAS)) << (DoubleConsts.SIGNIFICAND_WIDTH - 1);
			bits += signifRounded;
			/*
			 * If signifRounded == 2^53, we'd need to set all of the significand
			 * bits to zero and add 1 to the exponent. This is exactly the behavior
			 * we get from just adding signifRounded to bits directly. If the
			 * exponent is Double.MAX_EXPONENT, we round up (correctly) to
			 * Double.POSITIVE_INFINITY.
			 */
			bits |= Signum_Renamed & DoubleConsts.SIGN_BIT_MASK;
			return Double.longBitsToDouble(bits);
		}

		/// <summary>
		/// Returns a copy of the input array stripped of any leading zero bytes.
		/// </summary>
		private static int[] StripLeadingZeroInts(int[] val)
		{
			int vlen = val.Length;
			int keep;

			// Find first nonzero byte
			for (keep = 0; keep < vlen && val[keep] == 0; keep++)
			{
				;
			}
			return Arrays.CopyOfRange(val, keep, vlen);
		}

		/// <summary>
		/// Returns the input array stripped of any leading zero bytes.
		/// Since the source is trusted the copying may be skipped.
		/// </summary>
		private static int[] TrustedStripLeadingZeroInts(int[] val)
		{
			int vlen = val.Length;
			int keep;

			// Find first nonzero byte
			for (keep = 0; keep < vlen && val[keep] == 0; keep++)
			{
				;
			}
			return keep == 0 ? val : Arrays.CopyOfRange(val, keep, vlen);
		}

		/// <summary>
		/// Returns a copy of the input array stripped of any leading zero bytes.
		/// </summary>
		private static int[] StripLeadingZeroBytes(sbyte[] a)
		{
			int byteLength = a.Length;
			int keep;

			// Find first nonzero byte
			for (keep = 0; keep < byteLength && a[keep] == 0; keep++)
			{
				;
			}

			// Allocate new array and copy relevant part of input array
			int intLength = (int)((uint)((byteLength - keep) + 3) >> 2);
			int[] result = new int[intLength];
			int b = byteLength - 1;
			for (int i = intLength - 1; i >= 0; i--)
			{
				result[i] = a[b--] & 0xff;
				int bytesRemaining = b - keep + 1;
				int bytesToTransfer = System.Math.Min(3, bytesRemaining);
				for (int j = 8; j <= (bytesToTransfer << 3); j += 8)
				{
					result[i] |= ((a[b--] & 0xff) << j);
				}
			}
			return result;
		}

		/// <summary>
		/// Takes an array a representing a negative 2's-complement number and
		/// returns the minimal (no leading zero bytes) unsigned whose value is -a.
		/// </summary>
		private static int[] MakePositive(sbyte[] a)
		{
			int keep, k;
			int byteLength = a.Length;

			// Find first non-sign (0xff) byte of input
			for (keep = 0; keep < byteLength && a[keep] == -1; keep++)
			{
				;
			}


			/* Allocate output array.  If all non-sign bytes are 0x00, we must
			 * allocate space for one extra output byte. */
			for (k = keep; k < byteLength && a[k] == 0; k++)
			{
				;
			}

			int extraByte = (k == byteLength) ? 1 : 0;
			int intLength = (int)((uint)((byteLength - keep + extraByte) + 3) >> 2);
			int[] result = new int[intLength];

			/* Copy one's complement of input into output, leaving extra
			 * byte (if it exists) == 0x00 */
			int b = byteLength - 1;
			for (int i = intLength - 1; i >= 0; i--)
			{
				result[i] = a[b--] & 0xff;
				int numBytesToTransfer = System.Math.Min(3, b - keep + 1);
				if (numBytesToTransfer < 0)
				{
					numBytesToTransfer = 0;
				}
				for (int j = 8; j <= 8 * numBytesToTransfer; j += 8)
				{
					result[i] |= ((a[b--] & 0xff) << j);
				}

				// Mask indicates which bits must be complemented
				int mask = (int)((uint)-1 >> (8 * (3 - numBytesToTransfer)));
				result[i] = ~result[i] & mask;
			}

			// Add one to one's complement to generate two's complement
			for (int i = result.Length - 1; i >= 0; i--)
			{
				result[i] = (int)((result[i] & LONG_MASK) + 1);
				if (result[i] != 0)
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Takes an array a representing a negative 2's-complement number and
		/// returns the minimal (no leading zero ints) unsigned whose value is -a.
		/// </summary>
		private static int[] MakePositive(int[] a)
		{
			int keep, j;

			// Find first non-sign (0xffffffff) int of input
			for (keep = 0; keep < a.Length && a[keep] == -1; keep++)
			{
				;
			}

			/* Allocate output array.  If all non-sign ints are 0x00, we must
			 * allocate space for one extra output int. */
			for (j = keep; j < a.Length && a[j] == 0; j++)
			{
				;
			}
			int extraInt = (j == a.Length ? 1 : 0);
			int[] result = new int[a.Length - keep + extraInt];

			/* Copy one's complement of input into output, leaving extra
			 * int (if it exists) == 0x00 */
			for (int i = keep; i < a.Length; i++)
			{
				result[i - keep + extraInt] = ~a[i];
			}

			// Add one to one's complement to generate two's complement
			for (int i = result.Length - 1; ++result[i] == 0; i--)
			{
				;
			}

			return result;
		}

		/*
		 * The following two arrays are used for fast String conversions.  Both
		 * are indexed by radix.  The first is the number of digits of the given
		 * radix that can fit in a Java long without "going negative", i.e., the
		 * highest integer n such that radix**n < 2**63.  The second is the
		 * "long radix" that tears each number into "long digits", each of which
		 * consists of the number of digits in the corresponding element in
		 * digitsPerLong (longRadix[i] = i**digitPerLong[i]).  Both arrays have
		 * nonsense values in their 0 and 1 elements, as radixes 0 and 1 are not
		 * used.
		 */
		private static int[] DigitsPerLong = new int[] {0, 0, 62, 39, 31, 27, 24, 22, 20, 19, 18, 18, 17, 17, 16, 16, 15, 15, 15, 14, 14, 14, 14, 13, 13, 13, 13, 13, 13, 12, 12, 12, 12, 12, 12, 12, 12};

		private static BigInteger[] LongRadix = new BigInteger[] {null, null, ValueOf(0x4000000000000000L), ValueOf(0x383d9170b85ff80bL), ValueOf(0x4000000000000000L), ValueOf(0x6765c793fa10079dL), ValueOf(0x41c21cb8e1000000L), ValueOf(0x3642798750226111L), ValueOf(0x1000000000000000L), ValueOf(0x12bf307ae81ffd59L), ValueOf(0xde0b6b3a7640000L), ValueOf(0x4d28cb56c33fa539L), ValueOf(0x1eca170c00000000L), ValueOf(0x780c7372621bd74dL), ValueOf(0x1e39a5057d810000L), ValueOf(0x5b27ac993df97701L), ValueOf(0x1000000000000000L), ValueOf(0x27b95e997e21d9f1L), ValueOf(0x5da0e1e53c5c8000L), ValueOf(0xb16a458ef403f19L), ValueOf(0x16bcc41e90000000L), ValueOf(0x2d04b7fdd9c0ef49L), ValueOf(0x5658597bcaa24000L), ValueOf(0x6feb266931a75b7L), ValueOf(0xc29e98000000000L), ValueOf(0x14adf4b7320334b9L), ValueOf(0x226ed36478bfa000L), ValueOf(0x383d9170b85ff80bL), ValueOf(0x5a3c23e39c000000L), ValueOf(0x4e900abb53e6b71L), ValueOf(0x7600ec618141000L), ValueOf(0xaee5720ee830681L), ValueOf(0x1000000000000000L), ValueOf(0x172588ad4f5f0981L), ValueOf(0x211e44f7d02c1000L), ValueOf(0x2ee56725f06e5c71L), ValueOf(0x41c21cb8e1000000L)};

		/*
		 * These two arrays are the integer analogue of above.
		 */
		private static int[] DigitsPerInt = new int[] {0, 0, 30, 19, 15, 13, 11, 11, 10, 9, 9, 8, 8, 8, 8, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5};

		private static int[] IntRadix = new int[] {0, 0, 0x40000000, 0x4546b3db, 0x40000000, 0x48c27395, 0x159fd800, 0x75db9c97, 0x40000000, 0x17179149, 0x3b9aca00, 0xcc6db61, 0x19a10000, 0x309f1021, 0x57f6c100, 0xa2f1b6f, 0x10000000, 0x18754571, 0x247dbc80, 0x3547667b, 0x4c4b4000, 0x6b5a6e1d, 0x6c20a40, 0x8d2d931, 0xb640000, 0xe8d4a51, 0x1269ae40, 0x17179149, 0x1cb91000, 0x23744899, 0x2b73a840, 0x34e63b41, 0x40000000, 0x4cfa3cc1, 0x5c13d840, 0x6d91b519, 0x39aa400};

		/// <summary>
		/// These routines provide access to the two's complement representation
		/// of BigIntegers.
		/// </summary>

		/// <summary>
		/// Returns the length of the two's complement representation in ints,
		/// including space for at least one sign bit.
		/// </summary>
		private int IntLength()
		{
			return ((int)((uint)BitLength() >> 5)) + 1;
		}

		/* Returns sign bit */
		private int SignBit()
		{
			return Signum_Renamed < 0 ? 1 : 0;
		}

		/* Returns an int of sign bits */
		private int SignInt()
		{
			return Signum_Renamed < 0 ? - 1 : 0;
		}

		/// <summary>
		/// Returns the specified int of the little-endian two's complement
		/// representation (int 0 is the least significant).  The int number can
		/// be arbitrarily high (values are logically preceded by infinitely many
		/// sign ints).
		/// </summary>
		private int GetInt(int n)
		{
			if (n < 0)
			{
				return 0;
			}
			if (n >= Mag.Length)
			{
				return SignInt();
			}

			int magInt = Mag[Mag.Length - n - 1];

			return (Signum_Renamed >= 0 ? magInt : (n <= FirstNonzeroIntNum() ? - magInt :~magInt));
		}

		/// <summary>
		/// Returns the index of the int that contains the first nonzero int in the
		/// little-endian binary representation of the magnitude (int 0 is the
		/// least significant). If the magnitude is zero, return value is undefined.
		/// </summary>
		private int FirstNonzeroIntNum()
		{
			int fn = FirstNonzeroIntNum_Renamed - 2;
			if (fn == -2) // firstNonzeroIntNum not initialized yet
			{
				fn = 0;

				// Search for the first nonzero int
				int i;
				int mlen = Mag.Length;
				for (i = mlen - 1; i >= 0 && Mag[i] == 0; i--)
				{
					;
				}
				fn = mlen - i - 1;
				FirstNonzeroIntNum_Renamed = fn + 2; // offset by two to initialize
			}
			return fn;
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.1. for interoperability </summary>
		private const long SerialVersionUID = -8287574255936472291L;

		/// <summary>
		/// Serializable fields for BigInteger.
		/// 
		/// @serialField signum  int
		///              signum of this BigInteger.
		/// @serialField magnitude int[]
		///              magnitude array of this BigInteger.
		/// @serialField bitCount  int
		///              number of bits in this BigInteger
		/// @serialField bitLength int
		///              the number of bits in the minimal two's-complement
		///              representation of this BigInteger
		/// @serialField lowestSetBit int
		///              lowest set bit in the twos complement representation
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("signum", Integer.TYPE), new ObjectStreamField("magnitude", typeof(sbyte[])), new ObjectStreamField("bitCount", Integer.TYPE), new ObjectStreamField("bitLength", Integer.TYPE), new ObjectStreamField("firstNonzeroByteNum", Integer.TYPE), new ObjectStreamField("lowestSetBit", Integer.TYPE)};

		/// <summary>
		/// Reconstitute the {@code BigInteger} instance from a stream (that is,
		/// deserialize it). The magnitude is read in as an array of bytes
		/// for historical reasons, but it is converted to an array of ints
		/// and the byte array is discarded.
		/// Note:
		/// The current convention is to initialize the cache fields, bitCount,
		/// bitLength and lowestSetBit, to 0 rather than some other marker value.
		/// Therefore, no explicit action to set these fields needs to be taken in
		/// readObject because those fields already have a 0 value be default since
		/// defaultReadObject is not being used.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			/*
			 * In order to maintain compatibility with previous serialized forms,
			 * the magnitude of a BigInteger is serialized as an array of bytes.
			 * The magnitude field is used as a temporary store for the byte array
			 * that is deserialized. The cached computation fields should be
			 * transient but are serialized for compatibility reasons.
			 */

			// prepare to read the alternate persistent fields
			ObjectInputStream.GetField fields = s.ReadFields();

			// Read the alternate persistent fields that we care about
			int sign = fields.Get("signum", -2);
			sbyte[] magnitude = (sbyte[])fields.Get("magnitude", null);

			// Validate signum
			if (sign < -1 || sign > 1)
			{
				String message = "BigInteger: Invalid signum value";
				if (fields.Defaulted("signum"))
				{
					message = "BigInteger: Signum not present in stream";
				}
				throw new java.io.StreamCorruptedException(message);
			}
			int[] mag = StripLeadingZeroBytes(magnitude);
			if ((mag.Length == 0) != (sign == 0))
			{
				String message = "BigInteger: signum-magnitude mismatch";
				if (fields.Defaulted("magnitude"))
				{
					message = "BigInteger: Magnitude not present in stream";
				}
				throw new java.io.StreamCorruptedException(message);
			}

			// Commit final fields via Unsafe
			UnsafeHolder.PutSign(this, sign);

			// Calculate mag field from magnitude and discard magnitude
			UnsafeHolder.PutMag(this, mag);
			if (mag.Length >= MAX_MAG_LENGTH)
			{
				try
				{
					CheckRange();
				}
				catch (ArithmeticException)
				{
					throw new java.io.StreamCorruptedException("BigInteger: Out of the supported range");
				}
			}
		}

		// Support for resetting final fields while deserializing
		private class UnsafeHolder
		{
			internal static readonly sun.misc.Unsafe @unsafe;
			internal static readonly long SignumOffset;
			internal static readonly long MagOffset;

			internal static void PutSign(BigInteger bi, int sign)
			{
				@unsafe.putIntVolatile(bi, SignumOffset, sign);
			}

			internal static void PutMag(BigInteger bi, int[] magnitude)
			{
				@unsafe.putObjectVolatile(bi, MagOffset, magnitude);
			}
		}

		/// <summary>
		/// Save the {@code BigInteger} instance to a stream.
		/// The magnitude of a BigInteger is serialized as a byte array for
		/// historical reasons.
		/// 
		/// @serialData two necessary fields are written as well as obsolete
		///             fields for compatibility with older versions.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			// set the values of the Serializable fields
			ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("signum", Signum_Renamed);
			fields.Put("magnitude", MagSerializedForm());
			// The values written for cached fields are compatible with older
			// versions, but are ignored in readObject so don't otherwise matter.
			fields.Put("bitCount", -1);
			fields.Put("bitLength", -1);
			fields.Put("lowestSetBit", -2);
			fields.Put("firstNonzeroByteNum", -2);

			// save them
			s.WriteFields();
		}

		/// <summary>
		/// Returns the mag array as an array of bytes.
		/// </summary>
		private sbyte[] MagSerializedForm()
		{
			int len = Mag.Length;

			int bitLen = (len == 0 ? 0 : ((len - 1) << 5) + BitLengthForInt(Mag[0]));
			int byteLen = (int)((uint)(bitLen + 7) >> 3);
			sbyte[] result = new sbyte[byteLen];

			for (int i = byteLen - 1, bytesCopied = 4, intIndex = len - 1, nextInt = 0; i >= 0; i--)
			{
				if (bytesCopied == 4)
				{
					nextInt = Mag[intIndex--];
					bytesCopied = 1;
				}
				else
				{
					nextInt = (int)((uint)nextInt >> 8);
					bytesCopied++;
				}
				result[i] = (sbyte)nextInt;
			}
			return result;
		}

		/// <summary>
		/// Converts this {@code BigInteger} to a {@code long}, checking
		/// for lost information.  If the value of this {@code BigInteger}
		/// is out of the range of the {@code long} type, then an
		/// {@code ArithmeticException} is thrown.
		/// </summary>
		/// <returns> this {@code BigInteger} converted to a {@code long}. </returns>
		/// <exception cref="ArithmeticException"> if the value of {@code this} will
		/// not exactly fit in a {@code long}. </exception>
		/// <seealso cref= BigInteger#longValue
		/// @since  1.8 </seealso>
		public virtual long LongValueExact()
		{
			if (Mag.Length <= 2 && BitLength() <= 63)
			{
				return LongValue();
			}
			else
			{
				throw new ArithmeticException("BigInteger out of long range");
			}
		}

		/// <summary>
		/// Converts this {@code BigInteger} to an {@code int}, checking
		/// for lost information.  If the value of this {@code BigInteger}
		/// is out of the range of the {@code int} type, then an
		/// {@code ArithmeticException} is thrown.
		/// </summary>
		/// <returns> this {@code BigInteger} converted to an {@code int}. </returns>
		/// <exception cref="ArithmeticException"> if the value of {@code this} will
		/// not exactly fit in a {@code int}. </exception>
		/// <seealso cref= BigInteger#intValue
		/// @since  1.8 </seealso>
		public virtual int IntValueExact()
		{
			if (Mag.Length <= 1 && BitLength() <= 31)
			{
				return IntValue();
			}
			else
			{
				throw new ArithmeticException("BigInteger out of int range");
			}
		}

		/// <summary>
		/// Converts this {@code BigInteger} to a {@code short}, checking
		/// for lost information.  If the value of this {@code BigInteger}
		/// is out of the range of the {@code short} type, then an
		/// {@code ArithmeticException} is thrown.
		/// </summary>
		/// <returns> this {@code BigInteger} converted to a {@code short}. </returns>
		/// <exception cref="ArithmeticException"> if the value of {@code this} will
		/// not exactly fit in a {@code short}. </exception>
		/// <seealso cref= BigInteger#shortValue
		/// @since  1.8 </seealso>
		public virtual short ShortValueExact()
		{
			if (Mag.Length <= 1 && BitLength() <= 31)
			{
				int value = IntValue();
				if (value >= Short.MinValue && value <= Short.MaxValue)
				{
					return ShortValue();
				}
			}
			throw new ArithmeticException("BigInteger out of short range");
		}

		/// <summary>
		/// Converts this {@code BigInteger} to a {@code byte}, checking
		/// for lost information.  If the value of this {@code BigInteger}
		/// is out of the range of the {@code byte} type, then an
		/// {@code ArithmeticException} is thrown.
		/// </summary>
		/// <returns> this {@code BigInteger} converted to a {@code byte}. </returns>
		/// <exception cref="ArithmeticException"> if the value of {@code this} will
		/// not exactly fit in a {@code byte}. </exception>
		/// <seealso cref= BigInteger#byteValue
		/// @since  1.8 </seealso>
		public virtual sbyte ByteValueExact()
		{
			if (Mag.Length <= 1 && BitLength() <= 31)
			{
				int value = IntValue();
				if (value >= Byte.MinValue && value <= Byte.MaxValue)
				{
					return ByteValue();
				}
			}
			throw new ArithmeticException("BigInteger out of byte range");
		}
	}

}