using System;

/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.math
{

	/// <summary>
	/// A class used to represent multiprecision integers that makes efficient
	/// use of allocated space by allowing a number to occupy only part of
	/// an array so that the arrays do not have to be reallocated as often.
	/// When performing an operation with many iterations the array used to
	/// hold a number is only reallocated when necessary and does not have to
	/// be the same size as the number it represents. A mutable number allows
	/// calculations to occur on the same number without having to create
	/// a new number for every step of the calculation as occurs with
	/// BigIntegers.
	/// </summary>
	/// <seealso cref=     BigInteger
	/// @author  Michael McCloskey
	/// @author  Timothy Buktu
	/// @since   1.3 </seealso>


	internal class MutableBigInteger
	{
		/// <summary>
		/// Holds the magnitude of this MutableBigInteger in big endian order.
		/// The magnitude may start at an offset into the value array, and it may
		/// end before the length of the value array.
		/// </summary>
		internal int[] Value;

		/// <summary>
		/// The number of ints of the value array that are currently used
		/// to hold the magnitude of this MutableBigInteger. The magnitude starts
		/// at an offset and offset + intLen may be less than value.length.
		/// </summary>
		internal int IntLen;

		/// <summary>
		/// The offset into the value array where the magnitude of this
		/// MutableBigInteger begins.
		/// </summary>
		internal int Offset = 0;

		// Constants
		/// <summary>
		/// MutableBigInteger with one element value array with the value 1. Used by
		/// BigDecimal divideAndRound to increment the quotient. Use this constant
		/// only when the method is not going to modify this object.
		/// </summary>
		internal static readonly MutableBigInteger ONE = new MutableBigInteger(1);

		/// <summary>
		/// The minimum {@code intLen} for cancelling powers of two before
		/// dividing.
		/// If the number of ints is less than this threshold,
		/// {@code divideKnuth} does not eliminate common powers of two from
		/// the dividend and divisor.
		/// </summary>
		internal const int KNUTH_POW2_THRESH_LEN = 6;

		/// <summary>
		/// The minimum number of trailing zero ints for cancelling powers of two
		/// before dividing.
		/// If the dividend and divisor don't share at least this many zero ints
		/// at the end, {@code divideKnuth} does not eliminate common powers
		/// of two from the dividend and divisor.
		/// </summary>
		internal const int KNUTH_POW2_THRESH_ZEROS = 3;

		// Constructors

		/// <summary>
		/// The default constructor. An empty MutableBigInteger is created with
		/// a one word capacity.
		/// </summary>
		internal MutableBigInteger()
		{
			Value = new int[1];
			IntLen = 0;
		}

		/// <summary>
		/// Construct a new MutableBigInteger with a magnitude specified by
		/// the int val.
		/// </summary>
		internal MutableBigInteger(int val)
		{
			Value = new int[1];
			IntLen = 1;
			Value[0] = val;
		}

		/// <summary>
		/// Construct a new MutableBigInteger with the specified value array
		/// up to the length of the array supplied.
		/// </summary>
		internal MutableBigInteger(int[] val)
		{
			Value = val;
			IntLen = val.Length;
		}

		/// <summary>
		/// Construct a new MutableBigInteger with a magnitude equal to the
		/// specified BigInteger.
		/// </summary>
		internal MutableBigInteger(BigInteger b)
		{
			IntLen = b.Mag.Length;
			Value = Arrays.CopyOf(b.Mag, IntLen);
		}

		/// <summary>
		/// Construct a new MutableBigInteger with a magnitude equal to the
		/// specified MutableBigInteger.
		/// </summary>
		internal MutableBigInteger(MutableBigInteger val)
		{
			IntLen = val.IntLen;
			Value = Arrays.CopyOfRange(val.Value, val.Offset, val.Offset + IntLen);
		}

		/// <summary>
		/// Makes this number an {@code n}-int number all of whose bits are ones.
		/// Used by Burnikel-Ziegler division. </summary>
		/// <param name="n"> number of ints in the {@code value} array </param>
		/// <returns> a number equal to {@code ((1<<(32*n)))-1} </returns>
		private void Ones(int n)
		{
			if (n > Value.Length)
			{
				Value = new int[n];
			}
			Arrays.Fill(Value, -1);
			Offset = 0;
			IntLen = n;
		}

		/// <summary>
		/// Internal helper method to return the magnitude array. The caller is not
		/// supposed to modify the returned array.
		/// </summary>
		private int[] MagnitudeArray
		{
			get
			{
				if (Offset > 0 || Value.Length != IntLen)
				{
					return Arrays.CopyOfRange(Value, Offset, Offset + IntLen);
				}
				return Value;
			}
		}

		/// <summary>
		/// Convert this MutableBigInteger to a long value. The caller has to make
		/// sure this MutableBigInteger can be fit into long.
		/// </summary>
		private long ToLong()
		{
			assert(IntLen <= 2) : "this MutableBigInteger exceeds the range of long";
			if (IntLen == 0)
			{
				return 0;
			}
			long d = Value[Offset] & LONG_MASK;
			return (IntLen == 2) ? d << 32 | (Value[Offset + 1] & LONG_MASK) : d;
		}

		/// <summary>
		/// Convert this MutableBigInteger to a BigInteger object.
		/// </summary>
		internal virtual BigInteger ToBigInteger(int sign)
		{
			if (IntLen == 0 || sign == 0)
			{
				return BigInteger.ZERO;
			}
			return new BigInteger(MagnitudeArray, sign);
		}

		/// <summary>
		/// Converts this number to a nonnegative {@code BigInteger}.
		/// </summary>
		internal virtual BigInteger ToBigInteger()
		{
			Normalize();
			return ToBigInteger(Zero ? 0 : 1);
		}

		/// <summary>
		/// Convert this MutableBigInteger to BigDecimal object with the specified sign
		/// and scale.
		/// </summary>
		internal virtual BigDecimal ToBigDecimal(int sign, int scale)
		{
			if (IntLen == 0 || sign == 0)
			{
				return BigDecimal.ZeroValueOf(scale);
			}
			int[] mag = MagnitudeArray;
			int len = mag.Length;
			int d = mag[0];
			// If this MutableBigInteger can't be fit into long, we need to
			// make a BigInteger object for the resultant BigDecimal object.
			if (len > 2 || (d < 0 && len == 2))
			{
				return new BigDecimal(new BigInteger(mag, sign), INFLATED, scale, 0);
			}
			long v = (len == 2) ? ((mag[1] & LONG_MASK) | (d & LONG_MASK) << 32) : d & LONG_MASK;
			return BigDecimal.ValueOf(sign == -1 ? - v : v, scale);
		}

		/// <summary>
		/// This is for internal use in converting from a MutableBigInteger
		/// object into a long value given a specified sign.
		/// returns INFLATED if value is not fit into long
		/// </summary>
		internal virtual long ToCompactValue(int sign)
		{
			if (IntLen == 0 || sign == 0)
			{
				return 0L;
			}
			int[] mag = MagnitudeArray;
			int len = mag.Length;
			int d = mag[0];
			// If this MutableBigInteger can not be fitted into long, we need to
			// make a BigInteger object for the resultant BigDecimal object.
			if (len > 2 || (d < 0 && len == 2))
			{
				return INFLATED;
			}
			long v = (len == 2) ? ((mag[1] & LONG_MASK) | (d & LONG_MASK) << 32) : d & LONG_MASK;
			return sign == -1 ? - v : v;
		}

		/// <summary>
		/// Clear out a MutableBigInteger for reuse.
		/// </summary>
		internal virtual void Clear()
		{
			Offset = IntLen = 0;
			for (int index = 0, n = Value.Length; index < n; index++)
			{
				Value[index] = 0;
			}
		}

		/// <summary>
		/// Set a MutableBigInteger to zero, removing its offset.
		/// </summary>
		internal virtual void Reset()
		{
			Offset = IntLen = 0;
		}

		/// <summary>
		/// Compare the magnitude of two MutableBigIntegers. Returns -1, 0 or 1
		/// as this MutableBigInteger is numerically less than, equal to, or
		/// greater than <tt>b</tt>.
		/// </summary>
		internal int Compare(MutableBigInteger b)
		{
			int blen = b.IntLen;
			if (IntLen < blen)
			{
				return -1;
			}
			if (IntLen > blen)
			{
			   return 1;
			}

			// Add Integer.MIN_VALUE to make the comparison act as unsigned integer
			// comparison.
			int[] bval = b.Value;
			for (int i = Offset, j = b.Offset; i < IntLen + Offset; i++, j++)
			{
				int b1 = Value[i] + unchecked((int)0x80000000);
				int b2 = bval[j] + unchecked((int)0x80000000);
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
		/// Returns a value equal to what {@code b.leftShift(32*ints); return compare(b);}
		/// would return, but doesn't change the value of {@code b}.
		/// </summary>
		private int CompareShifted(MutableBigInteger b, int ints)
		{
			int blen = b.IntLen;
			int alen = IntLen - ints;
			if (alen < blen)
			{
				return -1;
			}
			if (alen > blen)
			{
			   return 1;
			}

			// Add Integer.MIN_VALUE to make the comparison act as unsigned integer
			// comparison.
			int[] bval = b.Value;
			for (int i = Offset, j = b.Offset; i < alen + Offset; i++, j++)
			{
				int b1 = Value[i] + unchecked((int)0x80000000);
				int b2 = bval[j] + unchecked((int)0x80000000);
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
		/// Compare this against half of a MutableBigInteger object (Needed for
		/// remainder tests).
		/// Assumes no leading unnecessary zeros, which holds for results
		/// from divide().
		/// </summary>
		internal int CompareHalf(MutableBigInteger b)
		{
			int blen = b.IntLen;
			int len = IntLen;
			if (len <= 0)
			{
				return blen <= 0 ? 0 : -1;
			}
			if (len > blen)
			{
				return 1;
			}
			if (len < blen - 1)
			{
				return -1;
			}
			int[] bval = b.Value;
			int bstart = 0;
			int carry = 0;
			// Only 2 cases left:len == blen or len == blen - 1
			if (len != blen) // len == blen - 1
			{
				if (bval[bstart] == 1)
				{
					++bstart;
					carry = unchecked((int)0x80000000);
				}
				else
				{
					return -1;
				}
			}
			// compare values with right-shifted values of b,
			// carrying shifted-out bits across words
			int[] val = Value;
			for (int i = Offset, j = bstart; i < len + Offset;)
			{
				int bv = bval[j++];
				long hb = (((int)((uint)bv >> 1)) + carry) & LONG_MASK;
				long v = val[i++] & LONG_MASK;
				if (v != hb)
				{
					return v < hb ? - 1 : 1;
				}
				carry = (bv & 1) << 31; // carray will be either 0x80000000 or 0
			}
			return carry == 0 ? 0 : -1;
		}

		/// <summary>
		/// Return the index of the lowest set bit in this MutableBigInteger. If the
		/// magnitude of this MutableBigInteger is zero, -1 is returned.
		/// </summary>
		private int LowestSetBit
		{
			get
			{
				if (IntLen == 0)
				{
					return -1;
				}
				int j, b;
				for (j = IntLen - 1; (j > 0) && (Value[j + Offset] == 0); j--)
				{
					;
				}
				b = Value[j + Offset];
				if (b == 0)
				{
					return -1;
				}
				return ((IntLen - 1 - j) << 5) + Integer.NumberOfTrailingZeros(b);
			}
		}

		/// <summary>
		/// Return the int in use in this MutableBigInteger at the specified
		/// index. This method is not used because it is not inlined on all
		/// platforms.
		/// </summary>
		private int GetInt(int index)
		{
			return Value[Offset + index];
		}

		/// <summary>
		/// Return a long which is equal to the unsigned value of the int in
		/// use in this MutableBigInteger at the specified index. This method is
		/// not used because it is not inlined on all platforms.
		/// </summary>
		private long GetLong(int index)
		{
			return Value[Offset + index] & LONG_MASK;
		}

		/// <summary>
		/// Ensure that the MutableBigInteger is in normal form, specifically
		/// making sure that there are no leading zeros, and that if the
		/// magnitude is zero, then intLen is zero.
		/// </summary>
		internal void Normalize()
		{
			if (IntLen == 0)
			{
				Offset = 0;
				return;
			}

			int index = Offset;
			if (Value[index] != 0)
			{
				return;
			}

			int indexBound = index + IntLen;
			do
			{
				index++;
			} while (index < indexBound && Value[index] == 0);

			int numZeros = index - Offset;
			IntLen -= numZeros;
			Offset = (IntLen == 0 ? 0 : Offset + numZeros);
		}

		/// <summary>
		/// If this MutableBigInteger cannot hold len words, increase the size
		/// of the value array to len words.
		/// </summary>
		private void EnsureCapacity(int len)
		{
			if (Value.Length < len)
			{
				Value = new int[len];
				Offset = 0;
				IntLen = len;
			}
		}

		/// <summary>
		/// Convert this MutableBigInteger into an int array with no leading
		/// zeros, of a length that is equal to this MutableBigInteger's intLen.
		/// </summary>
		internal virtual int[] ToIntArray()
		{
			int[] result = new int[IntLen];
			for (int i = 0; i < IntLen; i++)
			{
				result[i] = Value[Offset + i];
			}
			return result;
		}

		/// <summary>
		/// Sets the int at index+offset in this MutableBigInteger to val.
		/// This does not get inlined on all platforms so it is not used
		/// as often as originally intended.
		/// </summary>
		internal virtual void SetInt(int index, int val)
		{
			Value[Offset + index] = val;
		}

		/// <summary>
		/// Sets this MutableBigInteger's value array to the specified array.
		/// The intLen is set to the specified length.
		/// </summary>
		internal virtual void SetValue(int[] val, int length)
		{
			Value = val;
			IntLen = length;
			Offset = 0;
		}

		/// <summary>
		/// Sets this MutableBigInteger's value array to a copy of the specified
		/// array. The intLen is set to the length of the new array.
		/// </summary>
		internal virtual void CopyValue(MutableBigInteger src)
		{
			int len = src.IntLen;
			if (Value.Length < len)
			{
				Value = new int[len];
			}
			System.Array.Copy(src.Value, src.Offset, Value, 0, len);
			IntLen = len;
			Offset = 0;
		}

		/// <summary>
		/// Sets this MutableBigInteger's value array to a copy of the specified
		/// array. The intLen is set to the length of the specified array.
		/// </summary>
		internal virtual void CopyValue(int[] val)
		{
			int len = val.Length;
			if (Value.Length < len)
			{
				Value = new int[len];
			}
			System.Array.Copy(val, 0, Value, 0, len);
			IntLen = len;
			Offset = 0;
		}

		/// <summary>
		/// Returns true iff this MutableBigInteger has a value of one.
		/// </summary>
		internal virtual bool One
		{
			get
			{
				return (IntLen == 1) && (Value[Offset] == 1);
			}
		}

		/// <summary>
		/// Returns true iff this MutableBigInteger has a value of zero.
		/// </summary>
		internal virtual bool Zero
		{
			get
			{
				return (IntLen == 0);
			}
		}

		/// <summary>
		/// Returns true iff this MutableBigInteger is even.
		/// </summary>
		internal virtual bool Even
		{
			get
			{
				return (IntLen == 0) || ((Value[Offset + IntLen - 1] & 1) == 0);
			}
		}

		/// <summary>
		/// Returns true iff this MutableBigInteger is odd.
		/// </summary>
		internal virtual bool Odd
		{
			get
			{
				return Zero ? false : ((Value[Offset + IntLen - 1] & 1) == 1);
			}
		}

		/// <summary>
		/// Returns true iff this MutableBigInteger is in normal form. A
		/// MutableBigInteger is in normal form if it has no leading zeros
		/// after the offset, and intLen + offset <= value.length.
		/// </summary>
		internal virtual bool Normal
		{
			get
			{
				if (IntLen + Offset > Value.Length)
				{
					return false;
				}
				if (IntLen == 0)
				{
					return true;
				}
				return (Value[Offset] != 0);
			}
		}

		/// <summary>
		/// Returns a String representation of this MutableBigInteger in radix 10.
		/// </summary>
		public override String ToString()
		{
			BigInteger b = ToBigInteger(1);
			return b.ToString();
		}

		/// <summary>
		/// Like <seealso cref="#rightShift(int)"/> but {@code n} can be greater than the length of the number.
		/// </summary>
		internal virtual void SafeRightShift(int n)
		{
			if (n / 32 >= IntLen)
			{
				Reset();
			}
			else
			{
				RightShift(n);
			}
		}

		/// <summary>
		/// Right shift this MutableBigInteger n bits. The MutableBigInteger is left
		/// in normal form.
		/// </summary>
		internal virtual void RightShift(int n)
		{
			if (IntLen == 0)
			{
				return;
			}
			int nInts = (int)((uint)n >> 5);
			int nBits = n & 0x1F;
			this.IntLen -= nInts;
			if (nBits == 0)
			{
				return;
			}
			int bitsInHighWord = BigInteger.BitLengthForInt(Value[Offset]);
			if (nBits >= bitsInHighWord)
			{
				this.PrimitiveLeftShift(32 - nBits);
				this.IntLen--;
			}
			else
			{
				PrimitiveRightShift(nBits);
			}
		}

		/// <summary>
		/// Like <seealso cref="#leftShift(int)"/> but {@code n} can be zero.
		/// </summary>
		internal virtual void SafeLeftShift(int n)
		{
			if (n > 0)
			{
				LeftShift(n);
			}
		}

		/// <summary>
		/// Left shift this MutableBigInteger n bits.
		/// </summary>
		internal virtual void LeftShift(int n)
		{
			/*
			 * If there is enough storage space in this MutableBigInteger already
			 * the available space will be used. Space to the right of the used
			 * ints in the value array is faster to utilize, so the extra space
			 * will be taken from the right if possible.
			 */
			if (IntLen == 0)
			{
			   return;
			}
			int nInts = (int)((uint)n >> 5);
			int nBits = n & 0x1F;
			int bitsInHighWord = BigInteger.BitLengthForInt(Value[Offset]);

			// If shift can be done without moving words, do so
			if (n <= (32 - bitsInHighWord))
			{
				PrimitiveLeftShift(nBits);
				return;
			}

			int newLen = IntLen + nInts + 1;
			if (nBits <= (32 - bitsInHighWord))
			{
				newLen--;
			}
			if (Value.Length < newLen)
			{
				// The array must grow
				int[] result = new int[newLen];
				for (int i = 0; i < IntLen; i++)
				{
					result[i] = Value[Offset + i];
				}
				SetValue(result, newLen);
			}
			else if (Value.Length - Offset >= newLen)
			{
				// Use space on right
				for (int i = 0; i < newLen - IntLen; i++)
				{
					Value[Offset + IntLen + i] = 0;
				}
			}
			else
			{
				// Must use space on left
				for (int i = 0; i < IntLen; i++)
				{
					Value[i] = Value[Offset + i];
				}
				for (int i = IntLen; i < newLen; i++)
				{
					Value[i] = 0;
				}
				Offset = 0;
			}
			IntLen = newLen;
			if (nBits == 0)
			{
				return;
			}
			if (nBits <= (32 - bitsInHighWord))
			{
				PrimitiveLeftShift(nBits);
			}
			else
			{
				PrimitiveRightShift(32 - nBits);
			}
		}

		/// <summary>
		/// A primitive used for division. This method adds in one multiple of the
		/// divisor a back to the dividend result at a specified offset. It is used
		/// when qhat was estimated too large, and must be adjusted.
		/// </summary>
		private int Divadd(int[] a, int[] result, int offset)
		{
			long carry = 0;

			for (int j = a.Length - 1; j >= 0; j--)
			{
				long sum = (a[j] & LONG_MASK) + (result[j + offset] & LONG_MASK) + carry;
				result[j + offset] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}
			return (int)carry;
		}

		/// <summary>
		/// This method is used for division. It multiplies an n word input a by one
		/// word input x, and subtracts the n word product from q. This is needed
		/// when subtracting qhat*divisor from dividend.
		/// </summary>
		private int Mulsub(int[] q, int[] a, int x, int len, int offset)
		{
			long xLong = x & LONG_MASK;
			long carry = 0;
			offset += len;

			for (int j = len - 1; j >= 0; j--)
			{
				long product = (a[j] & LONG_MASK) * xLong + carry;
				long difference = q[offset] - product;
				q[offset--] = (int)difference;
				carry = ((long)((ulong)product >> 32)) + (((difference & LONG_MASK) > (((~(int)product) & LONG_MASK))) ? 1:0);
			}
			return (int)carry;
		}

		/// <summary>
		/// The method is the same as mulsun, except the fact that q array is not
		/// updated, the only result of the method is borrow flag.
		/// </summary>
		private int MulsubBorrow(int[] q, int[] a, int x, int len, int offset)
		{
			long xLong = x & LONG_MASK;
			long carry = 0;
			offset += len;
			for (int j = len - 1; j >= 0; j--)
			{
				long product = (a[j] & LONG_MASK) * xLong + carry;
				long difference = q[offset--] - product;
				carry = ((long)((ulong)product >> 32)) + (((difference & LONG_MASK) > (((~(int)product) & LONG_MASK))) ? 1:0);
			}
			return (int)carry;
		}

		/// <summary>
		/// Right shift this MutableBigInteger n bits, where n is
		/// less than 32.
		/// Assumes that intLen > 0, n > 0 for speed
		/// </summary>
		private void PrimitiveRightShift(int n)
		{
			int[] val = Value;
			int n2 = 32 - n;
			for (int i = Offset + IntLen - 1, c = val[i]; i > Offset; i--)
			{
				int b = c;
				c = val[i - 1];
				val[i] = (c << n2) | ((int)((uint)b >> n));
			}
			val[Offset] = (int)((uint)val[Offset] >> n);
		}

		/// <summary>
		/// Left shift this MutableBigInteger n bits, where n is
		/// less than 32.
		/// Assumes that intLen > 0, n > 0 for speed
		/// </summary>
		private void PrimitiveLeftShift(int n)
		{
			int[] val = Value;
			int n2 = 32 - n;
			for (int i = Offset, c = val[i], m = i + IntLen - 1; i < m; i++)
			{
				int b = c;
				c = val[i + 1];
				val[i] = (b << n) | ((int)((uint)c >> n2));
			}
			val[Offset + IntLen - 1] <<= n;
		}

		/// <summary>
		/// Returns a {@code BigInteger} equal to the {@code n}
		/// low ints of this number.
		/// </summary>
		private BigInteger GetLower(int n)
		{
			if (Zero)
			{
				return BigInteger.ZERO;
			}
			else if (IntLen < n)
			{
				return ToBigInteger(1);
			}
			else
			{
				// strip zeros
				int len = n;
				while (len > 0 && Value[Offset + IntLen - len] == 0)
				{
					len--;
				}
				int sign = len > 0 ? 1 : 0;
				return new BigInteger(Arrays.CopyOfRange(Value, Offset + IntLen - len, Offset + IntLen), sign);
			}
		}

		/// <summary>
		/// Discards all ints whose index is greater than {@code n}.
		/// </summary>
		private void KeepLower(int n)
		{
			if (IntLen >= n)
			{
				Offset += IntLen - n;
				IntLen = n;
			}
		}

		/// <summary>
		/// Adds the contents of two MutableBigInteger objects.The result
		/// is placed within this MutableBigInteger.
		/// The contents of the addend are not changed.
		/// </summary>
		internal virtual void Add(MutableBigInteger addend)
		{
			int x = IntLen;
			int y = addend.IntLen;
			int resultLen = (IntLen > addend.IntLen ? IntLen : addend.IntLen);
			int[] result = (Value.Length < resultLen ? new int[resultLen] : Value);

			int rstart = result.Length - 1;
			long sum;
			long carry = 0;

			// Add common parts of both numbers
			while (x > 0 && y > 0)
			{
				x--;
				y--;
				sum = (Value[x + Offset] & LONG_MASK) + (addend.Value[y + addend.Offset] & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}

			// Add remainder of the longer number
			while (x > 0)
			{
				x--;
				if (carry == 0 && result == Value && rstart == (x + Offset))
				{
					return;
				}
				sum = (Value[x + Offset] & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}
			while (y > 0)
			{
				y--;
				sum = (addend.Value[y + addend.Offset] & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}

			if (carry > 0) // Result must grow in length
			{
				resultLen++;
				if (result.Length < resultLen)
				{
					int[] temp = new int[resultLen];
					// Result one word longer from carry-out; copy low-order
					// bits into new result.
					System.Array.Copy(result, 0, temp, 1, result.Length);
					temp[0] = 1;
					result = temp;
				}
				else
				{
					result[rstart--] = 1;
				}
			}

			Value = result;
			IntLen = resultLen;
			Offset = result.Length - resultLen;
		}

		/// <summary>
		/// Adds the value of {@code addend} shifted {@code n} ints to the left.
		/// Has the same effect as {@code addend.leftShift(32*ints); add(addend);}
		/// but doesn't change the value of {@code addend}.
		/// </summary>
		internal virtual void AddShifted(MutableBigInteger addend, int n)
		{
			if (addend.Zero)
			{
				return;
			}

			int x = IntLen;
			int y = addend.IntLen + n;
			int resultLen = (IntLen > y ? IntLen : y);
			int[] result = (Value.Length < resultLen ? new int[resultLen] : Value);

			int rstart = result.Length - 1;
			long sum;
			long carry = 0;

			// Add common parts of both numbers
			while (x > 0 && y > 0)
			{
				x--;
				y--;
				int bval = y + addend.Offset < addend.Value.Length ? addend.Value[y + addend.Offset] : 0;
				sum = (Value[x + Offset] & LONG_MASK) + (bval & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}

			// Add remainder of the longer number
			while (x > 0)
			{
				x--;
				if (carry == 0 && result == Value && rstart == (x + Offset))
				{
					return;
				}
				sum = (Value[x + Offset] & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}
			while (y > 0)
			{
				y--;
				int bval = y + addend.Offset < addend.Value.Length ? addend.Value[y + addend.Offset] : 0;
				sum = (bval & LONG_MASK) + carry;
				result[rstart--] = (int)sum;
				carry = (long)((ulong)sum >> 32);
			}

			if (carry > 0) // Result must grow in length
			{
				resultLen++;
				if (result.Length < resultLen)
				{
					int[] temp = new int[resultLen];
					// Result one word longer from carry-out; copy low-order
					// bits into new result.
					System.Array.Copy(result, 0, temp, 1, result.Length);
					temp[0] = 1;
					result = temp;
				}
				else
				{
					result[rstart--] = 1;
				}
			}

			Value = result;
			IntLen = resultLen;
			Offset = result.Length - resultLen;
		}

		/// <summary>
		/// Like <seealso cref="#addShifted(MutableBigInteger, int)"/> but {@code this.intLen} must
		/// not be greater than {@code n}. In other words, concatenates {@code this}
		/// and {@code addend}.
		/// </summary>
		internal virtual void AddDisjoint(MutableBigInteger addend, int n)
		{
			if (addend.Zero)
			{
				return;
			}

			int x = IntLen;
			int y = addend.IntLen + n;
			int resultLen = (IntLen > y ? IntLen : y);
			int[] result;
			if (Value.Length < resultLen)
			{
				result = new int[resultLen];
			}
			else
			{
				result = Value;
				Arrays.Fill(Value, Offset + IntLen, Value.Length, 0);
			}

			int rstart = result.Length - 1;

			// copy from this if needed
			System.Array.Copy(Value, Offset, result, rstart + 1 - x, x);
			y -= x;
			rstart -= x;

			int len = System.Math.Min(y, addend.Value.Length - addend.Offset);
			System.Array.Copy(addend.Value, addend.Offset, result, rstart + 1 - y, len);

			// zero the gap
			for (int i = rstart + 1 - y + len; i < rstart + 1; i++)
			{
				result[i] = 0;
			}

			Value = result;
			IntLen = resultLen;
			Offset = result.Length - resultLen;
		}

		/// <summary>
		/// Adds the low {@code n} ints of {@code addend}.
		/// </summary>
		internal virtual void AddLower(MutableBigInteger addend, int n)
		{
			MutableBigInteger a = new MutableBigInteger(addend);
			if (a.Offset + a.IntLen >= n)
			{
				a.Offset = a.Offset + a.IntLen - n;
				a.IntLen = n;
			}
			a.Normalize();
			Add(a);
		}

		/// <summary>
		/// Subtracts the smaller of this and b from the larger and places the
		/// result into this MutableBigInteger.
		/// </summary>
		internal virtual int Subtract(MutableBigInteger b)
		{
			MutableBigInteger a = this;

			int[] result = Value;
			int sign = a.Compare(b);

			if (sign == 0)
			{
				Reset();
				return 0;
			}
			if (sign < 0)
			{
				MutableBigInteger tmp = a;
				a = b;
				b = tmp;
			}

			int resultLen = a.IntLen;
			if (result.Length < resultLen)
			{
				result = new int[resultLen];
			}

			long diff = 0;
			int x = a.IntLen;
			int y = b.IntLen;
			int rstart = result.Length - 1;

			// Subtract common parts of both numbers
			while (y > 0)
			{
				x--;
				y--;

				diff = (a.Value[x + a.Offset] & LONG_MASK) - (b.Value[y + b.Offset] & LONG_MASK) - ((int)-(diff >> 32));
				result[rstart--] = (int)diff;
			}
			// Subtract remainder of longer number
			while (x > 0)
			{
				x--;
				diff = (a.Value[x + a.Offset] & LONG_MASK) - ((int)-(diff >> 32));
				result[rstart--] = (int)diff;
			}

			Value = result;
			IntLen = resultLen;
			Offset = Value.Length - resultLen;
			Normalize();
			return sign;
		}

		/// <summary>
		/// Subtracts the smaller of a and b from the larger and places the result
		/// into the larger. Returns 1 if the answer is in a, -1 if in b, 0 if no
		/// operation was performed.
		/// </summary>
		private int Difference(MutableBigInteger b)
		{
			MutableBigInteger a = this;
			int sign = a.Compare(b);
			if (sign == 0)
			{
				return 0;
			}
			if (sign < 0)
			{
				MutableBigInteger tmp = a;
				a = b;
				b = tmp;
			}

			long diff = 0;
			int x = a.IntLen;
			int y = b.IntLen;

			// Subtract common parts of both numbers
			while (y > 0)
			{
				x--;
				y--;
				diff = (a.Value[a.Offset + x] & LONG_MASK) - (b.Value[b.Offset + y] & LONG_MASK) - ((int)-(diff >> 32));
				a.Value[a.Offset + x] = (int)diff;
			}
			// Subtract remainder of longer number
			while (x > 0)
			{
				x--;
				diff = (a.Value[a.Offset + x] & LONG_MASK) - ((int)-(diff >> 32));
				a.Value[a.Offset + x] = (int)diff;
			}

			a.Normalize();
			return sign;
		}

		/// <summary>
		/// Multiply the contents of two MutableBigInteger objects. The result is
		/// placed into MutableBigInteger z. The contents of y are not changed.
		/// </summary>
		internal virtual void Multiply(MutableBigInteger y, MutableBigInteger z)
		{
			int xLen = IntLen;
			int yLen = y.IntLen;
			int newLen = xLen + yLen;

			// Put z into an appropriate state to receive product
			if (z.Value.Length < newLen)
			{
				z.Value = new int[newLen];
			}
			z.Offset = 0;
			z.IntLen = newLen;

			// The first iteration is hoisted out of the loop to avoid extra add
			long carry = 0;
			for (int j = yLen - 1, k = yLen + xLen - 1; j >= 0; j--, k--)
			{
					long product = (y.Value[j + y.Offset] & LONG_MASK) * (Value[xLen - 1 + Offset] & LONG_MASK) + carry;
					z.Value[k] = (int)product;
					carry = (long)((ulong)product >> 32);
			}
			z.Value[xLen - 1] = (int)carry;

			// Perform the multiplication word by word
			for (int i = xLen - 2; i >= 0; i--)
			{
				carry = 0;
				for (int j = yLen - 1, k = yLen + i; j >= 0; j--, k--)
				{
					long product = (y.Value[j + y.Offset] & LONG_MASK) * (Value[i + Offset] & LONG_MASK) + (z.Value[k] & LONG_MASK) + carry;
					z.Value[k] = (int)product;
					carry = (long)((ulong)product >> 32);
				}
				z.Value[i] = (int)carry;
			}

			// Remove leading zeros from product
			z.Normalize();
		}

		/// <summary>
		/// Multiply the contents of this MutableBigInteger by the word y. The
		/// result is placed into z.
		/// </summary>
		internal virtual void Mul(int y, MutableBigInteger z)
		{
			if (y == 1)
			{
				z.CopyValue(this);
				return;
			}

			if (y == 0)
			{
				z.Clear();
				return;
			}

			// Perform the multiplication word by word
			long ylong = y & LONG_MASK;
			int[] zval = (z.Value.Length < IntLen + 1 ? new int[IntLen + 1] : z.Value);
			long carry = 0;
			for (int i = IntLen - 1; i >= 0; i--)
			{
				long product = ylong * (Value[i + Offset] & LONG_MASK) + carry;
				zval[i + 1] = (int)product;
				carry = (long)((ulong)product >> 32);
			}

			if (carry == 0)
			{
				z.Offset = 1;
				z.IntLen = IntLen;
			}
			else
			{
				z.Offset = 0;
				z.IntLen = IntLen + 1;
				zval[0] = (int)carry;
			}
			z.Value = zval;
		}

		 /// <summary>
		 /// This method is used for division of an n word dividend by a one word
		 /// divisor. The quotient is placed into quotient. The one word divisor is
		 /// specified by divisor.
		 /// </summary>
		 /// <returns> the remainder of the division is returned.
		 ///  </returns>
		internal virtual int DivideOneWord(int divisor, MutableBigInteger quotient)
		{
			long divisorLong = divisor & LONG_MASK;

			// Special case of one word dividend
			if (IntLen == 1)
			{
				long dividendValue = Value[Offset] & LONG_MASK;
				int q = (int)(dividendValue / divisorLong);
				int r = (int)(dividendValue - q * divisorLong);
				quotient.Value[0] = q;
				quotient.IntLen = (q == 0) ? 0 : 1;
				quotient.Offset = 0;
				return r;
			}

			if (quotient.Value.Length < IntLen)
			{
				quotient.Value = new int[IntLen];
			}
			quotient.Offset = 0;
			quotient.IntLen = IntLen;

			// Normalize the divisor
			int shift = Integer.NumberOfLeadingZeros(divisor);

			int rem = Value[Offset];
			long remLong = rem & LONG_MASK;
			if (remLong < divisorLong)
			{
				quotient.Value[0] = 0;
			}
			else
			{
				quotient.Value[0] = (int)(remLong / divisorLong);
				rem = (int)(remLong - (quotient.Value[0] * divisorLong));
				remLong = rem & LONG_MASK;
			}
			int xlen = IntLen;
			while (--xlen > 0)
			{
				long dividendEstimate = (remLong << 32) | (Value[Offset + IntLen - xlen] & LONG_MASK);
				int q;
				if (dividendEstimate >= 0)
				{
					q = (int)(dividendEstimate / divisorLong);
					rem = (int)(dividendEstimate - q * divisorLong);
				}
				else
				{
					long tmp = DivWord(dividendEstimate, divisor);
					q = (int)(tmp & LONG_MASK);
					rem = (int)((long)((ulong)tmp >> 32));
				}
				quotient.Value[IntLen - xlen] = q;
				remLong = rem & LONG_MASK;
			}

			quotient.Normalize();
			// Unnormalize
			if (shift > 0)
			{
				return rem % divisor;
			}
			else
			{
				return rem;
			}
		}

		/// <summary>
		/// Calculates the quotient of this div b and places the quotient in the
		/// provided MutableBigInteger objects and the remainder object is returned.
		/// 
		/// </summary>
		internal virtual MutableBigInteger Divide(MutableBigInteger b, MutableBigInteger quotient)
		{
			return Divide(b,quotient,true);
		}

		internal virtual MutableBigInteger Divide(MutableBigInteger b, MutableBigInteger quotient, bool needRemainder)
		{
			if (b.IntLen < BigInteger.BURNIKEL_ZIEGLER_THRESHOLD || IntLen - b.IntLen < BigInteger.BURNIKEL_ZIEGLER_OFFSET)
			{
				return DivideKnuth(b, quotient, needRemainder);
			}
			else
			{
				return DivideAndRemainderBurnikelZiegler(b, quotient);
			}
		}

		/// <seealso cref= #divideKnuth(MutableBigInteger, MutableBigInteger, boolean) </seealso>
		internal virtual MutableBigInteger DivideKnuth(MutableBigInteger b, MutableBigInteger quotient)
		{
			return DivideKnuth(b,quotient,true);
		}

		/// <summary>
		/// Calculates the quotient of this div b and places the quotient in the
		/// provided MutableBigInteger objects and the remainder object is returned.
		/// 
		/// Uses Algorithm D in Knuth section 4.3.1.
		/// Many optimizations to that algorithm have been adapted from the Colin
		/// Plumb C library.
		/// It special cases one word divisors for speed. The content of b is not
		/// changed.
		/// 
		/// </summary>
		internal virtual MutableBigInteger DivideKnuth(MutableBigInteger b, MutableBigInteger quotient, bool needRemainder)
		{
			if (b.IntLen == 0)
			{
				throw new ArithmeticException("BigInteger divide by zero");
			}

			// Dividend is zero
			if (IntLen == 0)
			{
				quotient.IntLen = quotient.Offset = 0;
				return needRemainder ? new MutableBigInteger() : null;
			}

			int cmp = Compare(b);
			// Dividend less than divisor
			if (cmp < 0)
			{
				quotient.IntLen = quotient.Offset = 0;
				return needRemainder ? new MutableBigInteger(this) : null;
			}
			// Dividend equal to divisor
			if (cmp == 0)
			{
				quotient.Value[0] = quotient.IntLen = 1;
				quotient.Offset = 0;
				return needRemainder ? new MutableBigInteger() : null;
			}

			quotient.Clear();
			// Special case one word divisor
			if (b.IntLen == 1)
			{
				int r = DivideOneWord(b.Value[b.Offset], quotient);
				if (needRemainder)
				{
					if (r == 0)
					{
						return new MutableBigInteger();
					}
					return new MutableBigInteger(r);
				}
				else
				{
					return null;
				}
			}

			// Cancel common powers of two if we're above the KNUTH_POW2_* thresholds
			if (IntLen >= KNUTH_POW2_THRESH_LEN)
			{
				int trailingZeroBits = System.Math.Min(LowestSetBit, b.LowestSetBit);
				if (trailingZeroBits >= KNUTH_POW2_THRESH_ZEROS * 32)
				{
					MutableBigInteger a = new MutableBigInteger(this);
					b = new MutableBigInteger(b);
					a.RightShift(trailingZeroBits);
					b.RightShift(trailingZeroBits);
					MutableBigInteger r = a.DivideKnuth(b, quotient);
					r.LeftShift(trailingZeroBits);
					return r;
				}
			}

			return DivideMagnitude(b, quotient, needRemainder);
		}

		/// <summary>
		/// Computes {@code this/b} and {@code this%b} using the
		/// <a href="http://cr.yp.to/bib/1998/burnikel.ps"> Burnikel-Ziegler algorithm</a>.
		/// This method implements algorithm 3 from pg. 9 of the Burnikel-Ziegler paper.
		/// The parameter beta was chosen to b 2<sup>32</sup> so almost all shifts are
		/// multiples of 32 bits.<br/>
		/// {@code this} and {@code b} must be nonnegative. </summary>
		/// <param name="b"> the divisor </param>
		/// <param name="quotient"> output parameter for {@code this/b} </param>
		/// <returns> the remainder </returns>
		internal virtual MutableBigInteger DivideAndRemainderBurnikelZiegler(MutableBigInteger b, MutableBigInteger quotient)
		{
			int r = IntLen;
			int s = b.IntLen;

			// Clear the quotient
			quotient.Offset = quotient.IntLen = 0;

			if (r < s)
			{
				return this;
			}
			else
			{
				// Unlike Knuth division, we don't check for common powers of two here because
				// BZ already runs faster if both numbers contain powers of two and cancelling them has no
				// additional benefit.

				// step 1: let m = min{2^k | (2^k)*BURNIKEL_ZIEGLER_THRESHOLD > s}
				int m = 1 << (32 - Integer.NumberOfLeadingZeros(s / BigInteger.BURNIKEL_ZIEGLER_THRESHOLD));

				int j = (s + m - 1) / m; // step 2a: j = ceil(s/m)
				int n = j * m; // step 2b: block length in 32-bit units
				long n32 = 32L * n; // block length in bits
				int sigma = (int) System.Math.Max(0, n32 - b.BitLength()); // step 3: sigma = max{T | (2^T)*B < beta^n}
				MutableBigInteger bShifted = new MutableBigInteger(b);
				bShifted.SafeLeftShift(sigma); // step 4a: shift b so its length is a multiple of n
				MutableBigInteger aShifted = new MutableBigInteger(this);
				aShifted.SafeLeftShift(sigma); // step 4b: shift a by the same amount

				// step 5: t is the number of blocks needed to accommodate a plus one additional bit
				int t = (int)((aShifted.BitLength() + n32) / n32);
				if (t < 2)
				{
					t = 2;
				}

				// step 6: conceptually split a into blocks a[t-1], ..., a[0]
				MutableBigInteger a1 = aShifted.GetBlock(t - 1, t, n); // the most significant block of a

				// step 7: z[t-2] = [a[t-1], a[t-2]]
				MutableBigInteger z = aShifted.GetBlock(t - 2, t, n); // the second to most significant block
				z.AddDisjoint(a1, n); // z[t-2]

				// do schoolbook division on blocks, dividing 2-block numbers by 1-block numbers
				MutableBigInteger qi = new MutableBigInteger();
				MutableBigInteger ri;
				for (int i = t - 2; i > 0; i--)
				{
					// step 8a: compute (qi,ri) such that z=b*qi+ri
					ri = z.Divide2n1n(bShifted, qi);

					// step 8b: z = [ri, a[i-1]]
					z = aShifted.GetBlock(i - 1, t, n); // a[i-1]
					z.AddDisjoint(ri, n);
					quotient.AddShifted(qi, i * n); // update q (part of step 9)
				}
				// final iteration of step 8: do the loop one more time for i=0 but leave z unchanged
				ri = z.Divide2n1n(bShifted, qi);
				quotient.Add(qi);

				ri.RightShift(sigma); // step 9: a and b were shifted, so shift back
				return ri;
			}
		}

		/// <summary>
		/// This method implements algorithm 1 from pg. 4 of the Burnikel-Ziegler paper.
		/// It divides a 2n-digit number by a n-digit number.<br/>
		/// The parameter beta is 2<sup>32</sup> so all shifts are multiples of 32 bits.
		/// <br/>
		/// {@code this} must be a nonnegative number such that {@code this.bitLength() <= 2*b.bitLength()} </summary>
		/// <param name="b"> a positive number such that {@code b.bitLength()} is even </param>
		/// <param name="quotient"> output parameter for {@code this/b} </param>
		/// <returns> {@code this%b} </returns>
		private MutableBigInteger Divide2n1n(MutableBigInteger b, MutableBigInteger quotient)
		{
			int n = b.IntLen;

			// step 1: base case
			if (n % 2 != 0 || n < BigInteger.BURNIKEL_ZIEGLER_THRESHOLD)
			{
				return DivideKnuth(b, quotient);
			}

			// step 2: view this as [a1,a2,a3,a4] where each ai is n/2 ints or less
			MutableBigInteger aUpper = new MutableBigInteger(this);
			aUpper.SafeRightShift(32 * (n / 2)); // aUpper = [a1,a2,a3]
			KeepLower(n / 2); // this = a4

			// step 3: q1=aUpper/b, r1=aUpper%b
			MutableBigInteger q1 = new MutableBigInteger();
			MutableBigInteger r1 = aUpper.Divide3n2n(b, q1);

			// step 4: quotient=[r1,this]/b, r2=[r1,this]%b
			AddDisjoint(r1, n / 2); // this = [r1,this]
			MutableBigInteger r2 = Divide3n2n(b, quotient);

			// step 5: let quotient=[q1,quotient] and return r2
			quotient.AddDisjoint(q1, n / 2);
			return r2;
		}

		/// <summary>
		/// This method implements algorithm 2 from pg. 5 of the Burnikel-Ziegler paper.
		/// It divides a 3n-digit number by a 2n-digit number.<br/>
		/// The parameter beta is 2<sup>32</sup> so all shifts are multiples of 32 bits.<br/>
		/// <br/>
		/// {@code this} must be a nonnegative number such that {@code 2*this.bitLength() <= 3*b.bitLength()} </summary>
		/// <param name="quotient"> output parameter for {@code this/b} </param>
		/// <returns> {@code this%b} </returns>
		private MutableBigInteger Divide3n2n(MutableBigInteger b, MutableBigInteger quotient)
		{
			int n = b.IntLen / 2; // half the length of b in ints

			// step 1: view this as [a1,a2,a3] where each ai is n ints or less; let a12=[a1,a2]
			MutableBigInteger a12 = new MutableBigInteger(this);
			a12.SafeRightShift(32 * n);

			// step 2: view b as [b1,b2] where each bi is n ints or less
			MutableBigInteger b1 = new MutableBigInteger(b);
			b1.SafeRightShift(n * 32);
			BigInteger b2 = b.GetLower(n);

			MutableBigInteger r;
			MutableBigInteger d;
			if (CompareShifted(b, n) < 0)
			{
				// step 3a: if a1<b1, let quotient=a12/b1 and r=a12%b1
				r = a12.Divide2n1n(b1, quotient);

				// step 4: d=quotient*b2
				d = new MutableBigInteger(quotient.ToBigInteger().Multiply(b2));
			}
			else
			{
				// step 3b: if a1>=b1, let quotient=beta^n-1 and r=a12-b1*2^n+b1
				quotient.Ones(n);
				a12.Add(b1);
				b1.LeftShift(32 * n);
				a12.Subtract(b1);
				r = a12;

				// step 4: d=quotient*b2=(b2 << 32*n) - b2
				d = new MutableBigInteger(b2);
				d.LeftShift(32 * n);
				d.Subtract(new MutableBigInteger(b2));
			}

			// step 5: r = r*beta^n + a3 - d (paper says a4)
			// However, don't subtract d until after the while loop so r doesn't become negative
			r.LeftShift(32 * n);
			r.AddLower(this, n);

			// step 6: add b until r>=d
			while (r.Compare(d) < 0)
			{
				r.Add(b);
				quotient.Subtract(MutableBigInteger.ONE);
			}
			r.Subtract(d);

			return r;
		}

		/// <summary>
		/// Returns a {@code MutableBigInteger} containing {@code blockLength} ints from
		/// {@code this} number, starting at {@code index*blockLength}.<br/>
		/// Used by Burnikel-Ziegler division. </summary>
		/// <param name="index"> the block index </param>
		/// <param name="numBlocks"> the total number of blocks in {@code this} number </param>
		/// <param name="blockLength"> length of one block in units of 32 bits
		/// @return </param>
		private MutableBigInteger GetBlock(int index, int numBlocks, int blockLength)
		{
			int blockStart = index * blockLength;
			if (blockStart >= IntLen)
			{
				return new MutableBigInteger();
			}

			int blockEnd;
			if (index == numBlocks - 1)
			{
				blockEnd = IntLen;
			}
			else
			{
				blockEnd = (index + 1) * blockLength;
			}
			if (blockEnd > IntLen)
			{
				return new MutableBigInteger();
			}

			int[] newVal = Arrays.CopyOfRange(Value, Offset + IntLen - blockEnd, Offset + IntLen - blockStart);
			return new MutableBigInteger(newVal);
		}

		/// <seealso cref= BigInteger#bitLength() </seealso>
		internal virtual long BitLength()
		{
			if (IntLen == 0)
			{
				return 0;
			}
			return IntLen * 32L - Integer.NumberOfLeadingZeros(Value[Offset]);
		}

		/// <summary>
		/// Internally used  to calculate the quotient of this div v and places the
		/// quotient in the provided MutableBigInteger object and the remainder is
		/// returned.
		/// </summary>
		/// <returns> the remainder of the division will be returned. </returns>
		internal virtual long Divide(long v, MutableBigInteger quotient)
		{
			if (v == 0)
			{
				throw new ArithmeticException("BigInteger divide by zero");
			}

			// Dividend is zero
			if (IntLen == 0)
			{
				quotient.IntLen = quotient.Offset = 0;
				return 0;
			}
			if (v < 0)
			{
				v = -v;
			}

			int d = (int)((long)((ulong)v >> 32));
			quotient.Clear();
			// Special case on word divisor
			if (d == 0)
			{
				return DivideOneWord((int)v, quotient) & LONG_MASK;
			}
			else
			{
				return DivideLongMagnitude(v, quotient).ToLong();
			}
		}

		private static void CopyAndShift(int[] src, int srcFrom, int srcLen, int[] dst, int dstFrom, int shift)
		{
			int n2 = 32 - shift;
			int c = src[srcFrom];
			for (int i = 0; i < srcLen - 1; i++)
			{
				int b = c;
				c = src[++srcFrom];
				dst[dstFrom + i] = (b << shift) | ((int)((uint)c >> n2));
			}
			dst[dstFrom + srcLen - 1] = c << shift;
		}

		/// <summary>
		/// Divide this MutableBigInteger by the divisor.
		/// The quotient will be placed into the provided quotient object &
		/// the remainder object is returned.
		/// </summary>
		private MutableBigInteger DivideMagnitude(MutableBigInteger div, MutableBigInteger quotient, bool needRemainder)
		{
			// assert div.intLen > 1
			// D1 normalize the divisor
			int shift = Integer.NumberOfLeadingZeros(div.Value[div.Offset]);
			// Copy divisor value to protect divisor
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dlen = div.intLen;
			int dlen = div.IntLen;
			int[] divisor;
			MutableBigInteger rem; // Remainder starts as dividend with space for a leading zero
			if (shift > 0)
			{
				divisor = new int[dlen];
				CopyAndShift(div.Value,div.Offset,dlen,divisor,0,shift);
				if (Integer.NumberOfLeadingZeros(Value[Offset]) >= shift)
				{
					int[] remarr = new int[IntLen + 1];
					rem = new MutableBigInteger(remarr);
					rem.IntLen = IntLen;
					rem.Offset = 1;
					CopyAndShift(Value,Offset,IntLen,remarr,1,shift);
				}
				else
				{
					int[] remarr = new int[IntLen + 2];
					rem = new MutableBigInteger(remarr);
					rem.IntLen = IntLen + 1;
					rem.Offset = 1;
					int rFrom = Offset;
					int c = 0;
					int n2 = 32 - shift;
					for (int i = 1; i < IntLen + 1; i++,rFrom++)
					{
						int b = c;
						c = Value[rFrom];
						remarr[i] = (b << shift) | ((int)((uint)c >> n2));
					}
					remarr[IntLen + 1] = c << shift;
				}
			}
			else
			{
				divisor = Arrays.CopyOfRange(div.Value, div.Offset, div.Offset + div.IntLen);
				rem = new MutableBigInteger(new int[IntLen + 1]);
				System.Array.Copy(Value, Offset, rem.Value, 1, IntLen);
				rem.IntLen = IntLen;
				rem.Offset = 1;
			}

			int nlen = rem.IntLen;

			// Set the quotient size
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int limit = nlen - dlen + 1;
			int limit = nlen - dlen + 1;
			if (quotient.Value.Length < limit)
			{
				quotient.Value = new int[limit];
				quotient.Offset = 0;
			}
			quotient.IntLen = limit;
			int[] q = quotient.Value;


			// Must insert leading 0 in rem if its length did not change
			if (rem.IntLen == nlen)
			{
				rem.Offset = 0;
				rem.Value[0] = 0;
				rem.IntLen++;
			}

			int dh = divisor[0];
			long dhLong = dh & LONG_MASK;
			int dl = divisor[1];

			// D2 Initialize j
			for (int j = 0; j < limit - 1; j++)
			{
				// D3 Calculate qhat
				// estimate qhat
				int qhat = 0;
				int qrem = 0;
				bool skipCorrection = false;
				int nh = rem.Value[j + rem.Offset];
				int nh2 = nh + unchecked((int)0x80000000);
				int nm = rem.Value[j + 1 + rem.Offset];

				if (nh == dh)
				{
					qhat = ~0;
					qrem = nh + nm;
					skipCorrection = qrem + 0x80000000 < nh2;
				}
				else
				{
					long nChunk = (((long)nh) << 32) | (nm & LONG_MASK);
					if (nChunk >= 0)
					{
						qhat = (int)(nChunk / dhLong);
						qrem = (int)(nChunk - (qhat * dhLong));
					}
					else
					{
						long tmp = DivWord(nChunk, dh);
						qhat = (int)(tmp & LONG_MASK);
						qrem = (int)((long)((ulong)tmp >> 32));
					}
				}

				if (qhat == 0)
				{
					continue;
				}

				if (!skipCorrection) // Correct qhat
				{
					long nl = rem.Value[j + 2 + rem.Offset] & LONG_MASK;
					long rs = ((qrem & LONG_MASK) << 32) | nl;
					long estProduct = (dl & LONG_MASK) * (qhat & LONG_MASK);

					if (UnsignedLongCompare(estProduct, rs))
					{
						qhat--;
						qrem = (int)((qrem & LONG_MASK) + dhLong);
						if ((qrem & LONG_MASK) >= dhLong)
						{
							estProduct -= (dl & LONG_MASK);
							rs = ((qrem & LONG_MASK) << 32) | nl;
							if (UnsignedLongCompare(estProduct, rs))
							{
								qhat--;
							}
						}
					}
				}

				// D4 Multiply and subtract
				rem.Value[j + rem.Offset] = 0;
				int borrow = Mulsub(rem.Value, divisor, qhat, dlen, j + rem.Offset);

				// D5 Test remainder
				if (borrow + 0x80000000 > nh2)
				{
					// D6 Add back
					Divadd(divisor, rem.Value, j + 1 + rem.Offset);
					qhat--;
				}

				// Store the quotient digit
				q[j] = qhat;
			} // D7 loop on j
			// D3 Calculate qhat
			// estimate qhat
			int qhat = 0;
			int qrem = 0;
			bool skipCorrection = false;
			int nh = rem.Value[limit - 1 + rem.Offset];
			int nh2 = nh + unchecked((int)0x80000000);
			int nm = rem.Value[limit + rem.Offset];

			if (nh == dh)
			{
				qhat = ~0;
				qrem = nh + nm;
				skipCorrection = qrem + 0x80000000 < nh2;
			}
			else
			{
				long nChunk = (((long) nh) << 32) | (nm & LONG_MASK);
				if (nChunk >= 0)
				{
					qhat = (int)(nChunk / dhLong);
					qrem = (int)(nChunk - (qhat * dhLong));
				}
				else
				{
					long tmp = DivWord(nChunk, dh);
					qhat = (int)(tmp & LONG_MASK);
					qrem = (int)((long)((ulong)tmp >> 32));
				}
			}
			if (qhat != 0)
			{
				if (!skipCorrection) // Correct qhat
				{
					long nl = rem.Value[limit + 1 + rem.Offset] & LONG_MASK;
					long rs = ((qrem & LONG_MASK) << 32) | nl;
					long estProduct = (dl & LONG_MASK) * (qhat & LONG_MASK);

					if (UnsignedLongCompare(estProduct, rs))
					{
						qhat--;
						qrem = (int)((qrem & LONG_MASK) + dhLong);
						if ((qrem & LONG_MASK) >= dhLong)
						{
							estProduct -= (dl & LONG_MASK);
							rs = ((qrem & LONG_MASK) << 32) | nl;
							if (UnsignedLongCompare(estProduct, rs))
							{
								qhat--;
							}
						}
					}
				}


				// D4 Multiply and subtract
				int borrow;
				rem.Value[limit - 1 + rem.Offset] = 0;
				if (needRemainder)
				{
					borrow = Mulsub(rem.Value, divisor, qhat, dlen, limit - 1 + rem.Offset);
				}
				else
				{
					borrow = MulsubBorrow(rem.Value, divisor, qhat, dlen, limit - 1 + rem.Offset);
				}

				// D5 Test remainder
				if (borrow + 0x80000000 > nh2)
				{
					// D6 Add back
					if (needRemainder)
					{
						Divadd(divisor, rem.Value, limit - 1 + 1 + rem.Offset);
					}
					qhat--;
				}

				// Store the quotient digit
				q[(limit - 1)] = qhat;
			}


			if (needRemainder)
			{
				// D8 Unnormalize
				if (shift > 0)
				{
					rem.RightShift(shift);
				}
				rem.Normalize();
			}
			quotient.Normalize();
			return needRemainder ? rem : null;
		}

		/// <summary>
		/// Divide this MutableBigInteger by the divisor represented by positive long
		/// value. The quotient will be placed into the provided quotient object &
		/// the remainder object is returned.
		/// </summary>
		private MutableBigInteger DivideLongMagnitude(long ldivisor, MutableBigInteger quotient)
		{
			// Remainder starts as dividend with space for a leading zero
			MutableBigInteger rem = new MutableBigInteger(new int[IntLen + 1]);
			System.Array.Copy(Value, Offset, rem.Value, 1, IntLen);
			rem.IntLen = IntLen;
			rem.Offset = 1;

			int nlen = rem.IntLen;

			int limit = nlen - 2 + 1;
			if (quotient.Value.Length < limit)
			{
				quotient.Value = new int[limit];
				quotient.Offset = 0;
			}
			quotient.IntLen = limit;
			int[] q = quotient.Value;

			// D1 normalize the divisor
			int shift = Long.NumberOfLeadingZeros(ldivisor);
			if (shift > 0)
			{
				ldivisor <<= shift;
				rem.LeftShift(shift);
			}

			// Must insert leading 0 in rem if its length did not change
			if (rem.IntLen == nlen)
			{
				rem.Offset = 0;
				rem.Value[0] = 0;
				rem.IntLen++;
			}

			int dh = (int)((long)((ulong)ldivisor >> 32));
			long dhLong = dh & LONG_MASK;
			int dl = (int)(ldivisor & LONG_MASK);

			// D2 Initialize j
			for (int j = 0; j < limit; j++)
			{
				// D3 Calculate qhat
				// estimate qhat
				int qhat = 0;
				int qrem = 0;
				bool skipCorrection = false;
				int nh = rem.Value[j + rem.Offset];
				int nh2 = nh + unchecked((int)0x80000000);
				int nm = rem.Value[j + 1 + rem.Offset];

				if (nh == dh)
				{
					qhat = ~0;
					qrem = nh + nm;
					skipCorrection = qrem + 0x80000000 < nh2;
				}
				else
				{
					long nChunk = (((long) nh) << 32) | (nm & LONG_MASK);
					if (nChunk >= 0)
					{
						qhat = (int)(nChunk / dhLong);
						qrem = (int)(nChunk - (qhat * dhLong));
					}
					else
					{
						long tmp = DivWord(nChunk, dh);
						qhat = (int)(tmp & LONG_MASK);
						qrem = (int)((long)((ulong)tmp >> 32));
					}
				}

				if (qhat == 0)
				{
					continue;
				}

				if (!skipCorrection) // Correct qhat
				{
					long nl = rem.Value[j + 2 + rem.Offset] & LONG_MASK;
					long rs = ((qrem & LONG_MASK) << 32) | nl;
					long estProduct = (dl & LONG_MASK) * (qhat & LONG_MASK);

					if (UnsignedLongCompare(estProduct, rs))
					{
						qhat--;
						qrem = (int)((qrem & LONG_MASK) + dhLong);
						if ((qrem & LONG_MASK) >= dhLong)
						{
							estProduct -= (dl & LONG_MASK);
							rs = ((qrem & LONG_MASK) << 32) | nl;
							if (UnsignedLongCompare(estProduct, rs))
							{
								qhat--;
							}
						}
					}
				}

				// D4 Multiply and subtract
				rem.Value[j + rem.Offset] = 0;
				int borrow = MulsubLong(rem.Value, dh, dl, qhat, j + rem.Offset);

				// D5 Test remainder
				if (borrow + 0x80000000 > nh2)
				{
					// D6 Add back
					DivaddLong(dh,dl, rem.Value, j + 1 + rem.Offset);
					qhat--;
				}

				// Store the quotient digit
				q[j] = qhat;
			} // D7 loop on j

			// D8 Unnormalize
			if (shift > 0)
			{
				rem.RightShift(shift);
			}

			quotient.Normalize();
			rem.Normalize();
			return rem;
		}

		/// <summary>
		/// A primitive used for division by long.
		/// Specialized version of the method divadd.
		/// dh is a high part of the divisor, dl is a low part
		/// </summary>
		private int DivaddLong(int dh, int dl, int[] result, int offset)
		{
			long carry = 0;

			long sum = (dl & LONG_MASK) + (result[1 + offset] & LONG_MASK);
			result[1 + offset] = (int)sum;

			sum = (dh & LONG_MASK) + (result[offset] & LONG_MASK) + carry;
			result[offset] = (int)sum;
			carry = (long)((ulong)sum >> 32);
			return (int)carry;
		}

		/// <summary>
		/// This method is used for division by long.
		/// Specialized version of the method sulsub.
		/// dh is a high part of the divisor, dl is a low part
		/// </summary>
		private int MulsubLong(int[] q, int dh, int dl, int x, int offset)
		{
			long xLong = x & LONG_MASK;
			offset += 2;
			long product = (dl & LONG_MASK) * xLong;
			long difference = q[offset] - product;
			q[offset--] = (int)difference;
			long carry = ((long)((ulong)product >> 32)) + (((difference & LONG_MASK) > (((~(int)product) & LONG_MASK))) ? 1:0);
			product = (dh & LONG_MASK) * xLong + carry;
			difference = q[offset] - product;
			q[offset--] = (int)difference;
			carry = ((long)((ulong)product >> 32)) + (((difference & LONG_MASK) > (((~(int)product) & LONG_MASK))) ? 1:0);
			return (int)carry;
		}

		/// <summary>
		/// Compare two longs as if they were unsigned.
		/// Returns true iff one is bigger than two.
		/// </summary>
		private bool UnsignedLongCompare(long one, long two)
		{
			return (one + Long.MinValue) > (two + Long.MinValue);
		}

		/// <summary>
		/// This method divides a long quantity by an int to estimate
		/// qhat for two multi precision numbers. It is used when
		/// the signed value of n is less than zero.
		/// Returns long value where high 32 bits contain remainder value and
		/// low 32 bits contain quotient value.
		/// </summary>
		internal static long DivWord(long n, int d)
		{
			long dLong = d & LONG_MASK;
			long r;
			long q;
			if (dLong == 1)
			{
				q = (int)n;
				r = 0;
				return (r << 32) | (q & LONG_MASK);
			}

			// Approximate the quotient and remainder
			q = ((long)((ulong)n >> 1)) / ((long)((ulong)dLong >> 1));
			r = n - q * dLong;

			// Correct the approximation
			while (r < 0)
			{
				r += dLong;
				q--;
			}
			while (r >= dLong)
			{
				r -= dLong;
				q++;
			}
			// n - q*dlong == r && 0 <= r <dLong, hence we're done.
			return (r << 32) | (q & LONG_MASK);
		}

		/// <summary>
		/// Calculate GCD of this and b. This and b are changed by the computation.
		/// </summary>
		internal virtual MutableBigInteger HybridGCD(MutableBigInteger b)
		{
			// Use Euclid's algorithm until the numbers are approximately the
			// same length, then use the binary GCD algorithm to find the GCD.
			MutableBigInteger a = this;
			MutableBigInteger q = new MutableBigInteger();

			while (b.IntLen != 0)
			{
				if (System.Math.Abs(a.IntLen - b.IntLen) < 2)
				{
					return a.BinaryGCD(b);
				}

				MutableBigInteger r = a.Divide(b, q);
				a = b;
				b = r;
			}
			return a;
		}

		/// <summary>
		/// Calculate GCD of this and v.
		/// Assumes that this and v are not zero.
		/// </summary>
		private MutableBigInteger BinaryGCD(MutableBigInteger v)
		{
			// Algorithm B from Knuth section 4.5.2
			MutableBigInteger u = this;
			MutableBigInteger r = new MutableBigInteger();

			// step B1
			int s1 = u.LowestSetBit;
			int s2 = v.LowestSetBit;
			int k = (s1 < s2) ? s1 : s2;
			if (k != 0)
			{
				u.RightShift(k);
				v.RightShift(k);
			}

			// step B2
			bool uOdd = (k == s1);
			MutableBigInteger t = uOdd ? v: u;
			int tsign = uOdd ? - 1 : 1;

			int lb;
			while ((lb = t.LowestSetBit) >= 0)
			{
				// steps B3 and B4
				t.RightShift(lb);
				// step B5
				if (tsign > 0)
				{
					u = t;
				}
				else
				{
					v = t;
				}

				// Special case one word numbers
				if (u.IntLen < 2 && v.IntLen < 2)
				{
					int x = u.Value[u.Offset];
					int y = v.Value[v.Offset];
					x = BinaryGcd(x, y);
					r.Value[0] = x;
					r.IntLen = 1;
					r.Offset = 0;
					if (k > 0)
					{
						r.LeftShift(k);
					}
					return r;
				}

				// step B6
				if ((tsign = u.Difference(v)) == 0)
				{
					break;
				}
				t = (tsign >= 0) ? u : v;
			}

			if (k > 0)
			{
				u.LeftShift(k);
			}
			return u;
		}

		/// <summary>
		/// Calculate GCD of a and b interpreted as unsigned integers.
		/// </summary>
		internal static int BinaryGcd(int a, int b)
		{
			if (b == 0)
			{
				return a;
			}
			if (a == 0)
			{
				return b;
			}

			// Right shift a & b till their last bits equal to 1.
			int aZeros = Integer.NumberOfTrailingZeros(a);
			int bZeros = Integer.NumberOfTrailingZeros(b);
			a = (int)((uint)a >> aZeros);
			b = (int)((uint)b >> bZeros);

			int t = (aZeros < bZeros ? aZeros : bZeros);

			while (a != b)
			{
				if ((a + 0x80000000) > (b + 0x80000000)) // a > b as unsigned
				{
					a -= b;
					a = (int)((uint)a >> Integer.NumberOfTrailingZeros(a));
				}
				else
				{
					b -= a;
					b = (int)((uint)b >> Integer.NumberOfTrailingZeros(b));
				}
			}
			return a << t;
		}

		/// <summary>
		/// Returns the modInverse of this mod p. This and p are not affected by
		/// the operation.
		/// </summary>
		internal virtual MutableBigInteger MutableModInverse(MutableBigInteger p)
		{
			// Modulus is odd, use Schroeppel's algorithm
			if (p.Odd)
			{
				return ModInverse(p);
			}

			// Base and modulus are even, throw exception
			if (Even)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}

			// Get even part of modulus expressed as a power of 2
			int powersOf2 = p.LowestSetBit;

			// Construct odd part of modulus
			MutableBigInteger oddMod = new MutableBigInteger(p);
			oddMod.RightShift(powersOf2);

			if (oddMod.One)
			{
				return ModInverseMP2(powersOf2);
			}

			// Calculate 1/a mod oddMod
			MutableBigInteger oddPart = ModInverse(oddMod);

			// Calculate 1/a mod evenMod
			MutableBigInteger evenPart = ModInverseMP2(powersOf2);

			// Combine the results using Chinese Remainder Theorem
			MutableBigInteger y1 = ModInverseBP2(oddMod, powersOf2);
			MutableBigInteger y2 = oddMod.ModInverseMP2(powersOf2);

			MutableBigInteger temp1 = new MutableBigInteger();
			MutableBigInteger temp2 = new MutableBigInteger();
			MutableBigInteger result = new MutableBigInteger();

			oddPart.LeftShift(powersOf2);
			oddPart.Multiply(y1, result);

			evenPart.Multiply(oddMod, temp1);
			temp1.Multiply(y2, temp2);

			result.Add(temp2);
			return result.Divide(p, temp1);
		}

		/*
		 * Calculate the multiplicative inverse of this mod 2^k.
		 */
		internal virtual MutableBigInteger ModInverseMP2(int k)
		{
			if (Even)
			{
				throw new ArithmeticException("Non-invertible. (GCD != 1)");
			}

			if (k > 64)
			{
				return EuclidModInverse(k);
			}

			int t = InverseMod32(Value[Offset + IntLen - 1]);

			if (k < 33)
			{
				t = (k == 32 ? t : t & ((1 << k) - 1));
				return new MutableBigInteger(t);
			}

			long pLong = (Value[Offset + IntLen - 1] & LONG_MASK);
			if (IntLen > 1)
			{
				pLong |= ((long)Value[Offset + IntLen - 2] << 32);
			}
			long tLong = t & LONG_MASK;
			tLong = tLong * (2 - pLong * tLong); // 1 more Newton iter step
			tLong = (k == 64 ? tLong : tLong & ((1L << k) - 1));

			MutableBigInteger result = new MutableBigInteger(new int[2]);
			result.Value[0] = (int)((long)((ulong)tLong >> 32));
			result.Value[1] = (int)tLong;
			result.IntLen = 2;
			result.Normalize();
			return result;
		}

		/// <summary>
		/// Returns the multiplicative inverse of val mod 2^32.  Assumes val is odd.
		/// </summary>
		internal static int InverseMod32(int val)
		{
			// Newton's iteration!
			int t = val;
			t *= 2 - val * t;
			t *= 2 - val * t;
			t *= 2 - val * t;
			t *= 2 - val * t;
			return t;
		}

		/// <summary>
		/// Calculate the multiplicative inverse of 2^k mod mod, where mod is odd.
		/// </summary>
		internal static MutableBigInteger ModInverseBP2(MutableBigInteger mod, int k)
		{
			// Copy the mod to protect original
			return Fixup(new MutableBigInteger(1), new MutableBigInteger(mod), k);
		}

		/// <summary>
		/// Calculate the multiplicative inverse of this mod mod, where mod is odd.
		/// This and mod are not changed by the calculation.
		/// 
		/// This method implements an algorithm due to Richard Schroeppel, that uses
		/// the same intermediate representation as Montgomery Reduction
		/// ("Montgomery Form").  The algorithm is described in an unpublished
		/// manuscript entitled "Fast Modular Reciprocals."
		/// </summary>
		private MutableBigInteger ModInverse(MutableBigInteger mod)
		{
			MutableBigInteger p = new MutableBigInteger(mod);
			MutableBigInteger f = new MutableBigInteger(this);
			MutableBigInteger g = new MutableBigInteger(p);
			SignedMutableBigInteger c = new SignedMutableBigInteger(1);
			SignedMutableBigInteger d = new SignedMutableBigInteger();
			MutableBigInteger temp = null;
			SignedMutableBigInteger sTemp = null;

			int k = 0;
			// Right shift f k times until odd, left shift d k times
			if (f.Even)
			{
				int trailingZeros = f.LowestSetBit;
				f.RightShift(trailingZeros);
				d.LeftShift(trailingZeros);
				k = trailingZeros;
			}

			// The Almost Inverse Algorithm
			while (!f.One)
			{
				// If gcd(f, g) != 1, number is not invertible modulo mod
				if (f.Zero)
				{
					throw new ArithmeticException("BigInteger not invertible.");
				}

				// If f < g exchange f, g and c, d
				if (f.Compare(g) < 0)
				{
					temp = f;
					f = g;
					g = temp;
					sTemp = d;
					d = c;
					c = sTemp;
				}

				// If f == g (mod 4)
				if (((f.Value[f.Offset + f.IntLen - 1] ^ g.Value[g.Offset + g.IntLen - 1]) & 3) == 0)
				{
					f.Subtract(g);
					c.SignedSubtract(d);
				} // If f != g (mod 4)
				else
				{
					f.Add(g);
					c.SignedAdd(d);
				}

				// Right shift f k times until odd, left shift d k times
				int trailingZeros = f.LowestSetBit;
				f.RightShift(trailingZeros);
				d.LeftShift(trailingZeros);
				k += trailingZeros;
			}

			while (c.Sign < 0)
			{
			   c.SignedAdd(p);
			}

			return Fixup(c, p, k);
		}

		/// <summary>
		/// The Fixup Algorithm
		/// Calculates X such that X = C * 2^(-k) (mod P)
		/// Assumes C<P and P is odd.
		/// </summary>
		internal static MutableBigInteger Fixup(MutableBigInteger c, MutableBigInteger p, int k)
		{
			MutableBigInteger temp = new MutableBigInteger();
			// Set r to the multiplicative inverse of p mod 2^32
			int r = -InverseMod32(p.Value[p.Offset + p.IntLen - 1]);

			for (int i = 0, numWords = k >> 5; i < numWords; i++)
			{
				// V = R * c (mod 2^j)
				int v = r * c.Value[c.Offset + c.IntLen - 1];
				// c = c + (v * p)
				p.Mul(v, temp);
				c.Add(temp);
				// c = c / 2^j
				c.IntLen--;
			}
			int numBits = k & 0x1f;
			if (numBits != 0)
			{
				// V = R * c (mod 2^j)
				int v = r * c.Value[c.Offset + c.IntLen - 1];
				v &= ((1 << numBits) - 1);
				// c = c + (v * p)
				p.Mul(v, temp);
				c.Add(temp);
				// c = c / 2^j
				c.RightShift(numBits);
			}

			// In theory, c may be greater than p at this point (Very rare!)
			while (c.Compare(p) >= 0)
			{
				c.Subtract(p);
			}

			return c;
		}

		/// <summary>
		/// Uses the extended Euclidean algorithm to compute the modInverse of base
		/// mod a modulus that is a power of 2. The modulus is 2^k.
		/// </summary>
		internal virtual MutableBigInteger EuclidModInverse(int k)
		{
			MutableBigInteger b = new MutableBigInteger(1);
			b.LeftShift(k);
			MutableBigInteger mod = new MutableBigInteger(b);

			MutableBigInteger a = new MutableBigInteger(this);
			MutableBigInteger q = new MutableBigInteger();
			MutableBigInteger r = b.Divide(a, q);

			MutableBigInteger swapper = b;
			// swap b & r
			b = r;
			r = swapper;

			MutableBigInteger t1 = new MutableBigInteger(q);
			MutableBigInteger t0 = new MutableBigInteger(1);
			MutableBigInteger temp = new MutableBigInteger();

			while (!b.One)
			{
				r = a.Divide(b, q);

				if (r.IntLen == 0)
				{
					throw new ArithmeticException("BigInteger not invertible.");
				}

				swapper = r;
				a = swapper;

				if (q.IntLen == 1)
				{
					t1.Mul(q.Value[q.Offset], temp);
				}
				else
				{
					q.Multiply(t1, temp);
				}
				swapper = q;
				q = temp;
				temp = swapper;
				t0.Add(q);

				if (a.One)
				{
					return t0;
				}

				r = b.Divide(a, q);

				if (r.IntLen == 0)
				{
					throw new ArithmeticException("BigInteger not invertible.");
				}

				swapper = b;
				b = r;

				if (q.IntLen == 1)
				{
					t0.Mul(q.Value[q.Offset], temp);
				}
				else
				{
					q.Multiply(t0, temp);
				}
				swapper = q;
				q = temp;
				temp = swapper;

				t1.Add(q);
			}
			mod.Subtract(t1);
			return mod;
		}
	}

}