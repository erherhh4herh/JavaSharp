using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
 *
 *
 *
 *
 *
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 *  * Neither the name of JSR-310 nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace java.time.temporal
{


	/// <summary>
	/// The range of valid values for a date-time field.
	/// <para>
	/// All <seealso cref="TemporalField"/> instances have a valid range of values.
	/// For example, the ISO day-of-month runs from 1 to somewhere between 28 and 31.
	/// This class captures that valid range.
	/// </para>
	/// <para>
	/// It is important to be aware of the limitations of this class.
	/// Only the minimum and maximum values are provided.
	/// It is possible for there to be invalid values within the outer range.
	/// For example, a weird field may have valid values of 1, 2, 4, 6, 7, thus
	/// have a range of '1 - 7', despite that fact that values 3 and 5 are invalid.
	/// </para>
	/// <para>
	/// Instances of this class are not tied to a specific field.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ValueRange
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -7317881728594519368L;

		/// <summary>
		/// The smallest minimum value.
		/// </summary>
		private readonly long MinSmallest;
		/// <summary>
		/// The largest minimum value.
		/// </summary>
		private readonly long MinLargest;
		/// <summary>
		/// The smallest maximum value.
		/// </summary>
		private readonly long MaxSmallest;
		/// <summary>
		/// The largest maximum value.
		/// </summary>
		private readonly long MaxLargest;

		/// <summary>
		/// Obtains a fixed value range.
		/// <para>
		/// This factory obtains a range where the minimum and maximum values are fixed.
		/// For example, the ISO month-of-year always runs from 1 to 12.
		/// 
		/// </para>
		/// </summary>
		/// <param name="min">  the minimum value </param>
		/// <param name="max">  the maximum value </param>
		/// <returns> the ValueRange for min, max, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the minimum is greater than the maximum </exception>
		public static ValueRange Of(long min, long max)
		{
			if (min > max)
			{
				throw new IllegalArgumentException("Minimum value must be less than maximum value");
			}
			return new ValueRange(min, min, max, max);
		}

		/// <summary>
		/// Obtains a variable value range.
		/// <para>
		/// This factory obtains a range where the minimum value is fixed and the maximum value may vary.
		/// For example, the ISO day-of-month always starts at 1, but ends between 28 and 31.
		/// 
		/// </para>
		/// </summary>
		/// <param name="min">  the minimum value </param>
		/// <param name="maxSmallest">  the smallest maximum value </param>
		/// <param name="maxLargest">  the largest maximum value </param>
		/// <returns> the ValueRange for min, smallest max, largest max, not null </returns>
		/// <exception cref="IllegalArgumentException"> if
		///     the minimum is greater than the smallest maximum,
		///  or the smallest maximum is greater than the largest maximum </exception>
		public static ValueRange Of(long min, long maxSmallest, long maxLargest)
		{
			return Of(min, min, maxSmallest, maxLargest);
		}

		/// <summary>
		/// Obtains a fully variable value range.
		/// <para>
		/// This factory obtains a range where both the minimum and maximum value may vary.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minSmallest">  the smallest minimum value </param>
		/// <param name="minLargest">  the largest minimum value </param>
		/// <param name="maxSmallest">  the smallest maximum value </param>
		/// <param name="maxLargest">  the largest maximum value </param>
		/// <returns> the ValueRange for smallest min, largest min, smallest max, largest max, not null </returns>
		/// <exception cref="IllegalArgumentException"> if
		///     the smallest minimum is greater than the smallest maximum,
		///  or the smallest maximum is greater than the largest maximum
		///  or the largest minimum is greater than the largest maximum </exception>
		public static ValueRange Of(long minSmallest, long minLargest, long maxSmallest, long maxLargest)
		{
			if (minSmallest > minLargest)
			{
				throw new IllegalArgumentException("Smallest minimum value must be less than largest minimum value");
			}
			if (maxSmallest > maxLargest)
			{
				throw new IllegalArgumentException("Smallest maximum value must be less than largest maximum value");
			}
			if (minLargest > maxLargest)
			{
				throw new IllegalArgumentException("Minimum value must be less than maximum value");
			}
			return new ValueRange(minSmallest, minLargest, maxSmallest, maxLargest);
		}

		/// <summary>
		/// Restrictive constructor.
		/// </summary>
		/// <param name="minSmallest">  the smallest minimum value </param>
		/// <param name="minLargest">  the largest minimum value </param>
		/// <param name="maxSmallest">  the smallest minimum value </param>
		/// <param name="maxLargest">  the largest minimum value </param>
		private ValueRange(long minSmallest, long minLargest, long maxSmallest, long maxLargest)
		{
			this.MinSmallest = minSmallest;
			this.MinLargest = minLargest;
			this.MaxSmallest = maxSmallest;
			this.MaxLargest = maxLargest;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Is the value range fixed and fully known.
		/// <para>
		/// For example, the ISO day-of-month runs from 1 to between 28 and 31.
		/// Since there is uncertainty about the maximum value, the range is not fixed.
		/// However, for the month of January, the range is always 1 to 31, thus it is fixed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the set of values is fixed </returns>
		public bool Fixed
		{
			get
			{
				return MinSmallest == MinLargest && MaxSmallest == MaxLargest;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the minimum value that the field can take.
		/// <para>
		/// For example, the ISO day-of-month always starts at 1.
		/// The minimum is therefore 1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the minimum value for this field </returns>
		public long Minimum
		{
			get
			{
				return MinSmallest;
			}
		}

		/// <summary>
		/// Gets the largest possible minimum value that the field can take.
		/// <para>
		/// For example, the ISO day-of-month always starts at 1.
		/// The largest minimum is therefore 1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the largest possible minimum value for this field </returns>
		public long LargestMinimum
		{
			get
			{
				return MinLargest;
			}
		}

		/// <summary>
		/// Gets the smallest possible maximum value that the field can take.
		/// <para>
		/// For example, the ISO day-of-month runs to between 28 and 31 days.
		/// The smallest maximum is therefore 28.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the smallest possible maximum value for this field </returns>
		public long SmallestMaximum
		{
			get
			{
				return MaxSmallest;
			}
		}

		/// <summary>
		/// Gets the maximum value that the field can take.
		/// <para>
		/// For example, the ISO day-of-month runs to between 28 and 31 days.
		/// The maximum is therefore 31.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the maximum value for this field </returns>
		public long Maximum
		{
			get
			{
				return MaxLargest;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if all values in the range fit in an {@code int}.
		/// <para>
		/// This checks that all valid values are within the bounds of an {@code int}.
		/// </para>
		/// <para>
		/// For example, the ISO month-of-year has values from 1 to 12, which fits in an {@code int}.
		/// By comparison, ISO nano-of-day runs from 1 to 86,400,000,000,000 which does not fit in an {@code int}.
		/// </para>
		/// <para>
		/// This implementation uses <seealso cref="#getMinimum()"/> and <seealso cref="#getMaximum()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if a valid value always fits in an {@code int} </returns>
		public bool IntValue
		{
			get
			{
				return Minimum >= Integer.MinValue && Maximum <= Integer.MaxValue;
			}
		}

		/// <summary>
		/// Checks if the value is within the valid range.
		/// <para>
		/// This checks that the value is within the stored range of values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">  the value to check </param>
		/// <returns> true if the value is valid </returns>
		public bool IsValidValue(long value)
		{
			return (value >= Minimum && value <= Maximum);
		}

		/// <summary>
		/// Checks if the value is within the valid range and that all values
		/// in the range fit in an {@code int}.
		/// <para>
		/// This method combines <seealso cref="#isIntValue()"/> and <seealso cref="#isValidValue(long)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">  the value to check </param>
		/// <returns> true if the value is valid and fits in an {@code int} </returns>
		public bool IsValidIntValue(long value)
		{
			return IntValue && IsValidValue(value);
		}

		/// <summary>
		/// Checks that the specified value is valid.
		/// <para>
		/// This validates that the value is within the valid range of values.
		/// The field is only used to improve the error message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">  the value to check </param>
		/// <param name="field">  the field being checked, may be null </param>
		/// <returns> the value that was passed in </returns>
		/// <seealso cref= #isValidValue(long) </seealso>
		public long CheckValidValue(long value, TemporalField field)
		{
			if (IsValidValue(value) == false)
			{
				throw new DateTimeException(GenInvalidFieldMessage(field, value));
			}
			return value;
		}

		/// <summary>
		/// Checks that the specified value is valid and fits in an {@code int}.
		/// <para>
		/// This validates that the value is within the valid range of values and that
		/// all valid values are within the bounds of an {@code int}.
		/// The field is only used to improve the error message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">  the value to check </param>
		/// <param name="field">  the field being checked, may be null </param>
		/// <returns> the value that was passed in </returns>
		/// <seealso cref= #isValidIntValue(long) </seealso>
		public int CheckValidIntValue(long value, TemporalField field)
		{
			if (IsValidIntValue(value) == false)
			{
				throw new DateTimeException(GenInvalidFieldMessage(field, value));
			}
			return (int) value;
		}

		private String GenInvalidFieldMessage(TemporalField field, long value)
		{
			if (field != null)
			{
				return "Invalid value for " + field + " (valid values " + this + "): " + value;
			}
			else
			{
				return "Invalid value (valid values " + this + "): " + value;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Restore the state of an ValueRange from the stream.
		/// Check that the values are valid.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> if
		///     the smallest minimum is greater than the smallest maximum,
		///  or the smallest maximum is greater than the largest maximum
		///  or the largest minimum is greater than the largest maximum </exception>
		/// <exception cref="ClassNotFoundException"> if a class cannot be resolved </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException, java.io.InvalidObjectException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();
			if (MinSmallest > MinLargest)
			{
				throw new InvalidObjectException("Smallest minimum value must be less than largest minimum value");
			}
			if (MaxSmallest > MaxLargest)
			{
				throw new InvalidObjectException("Smallest maximum value must be less than largest maximum value");
			}
			if (MinLargest > MaxLargest)
			{
				throw new InvalidObjectException("Minimum value must be less than maximum value");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this range is equal to another range.
		/// <para>
		/// The comparison is based on the four values, minimum, largest minimum,
		/// smallest maximum and maximum.
		/// Only objects of type {@code ValueRange} are compared, other types return false.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other range </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is ValueRange)
			{
				ValueRange other = (ValueRange) obj;
			   return MinSmallest == other.MinSmallest && MinLargest == other.MinLargest && MaxSmallest == other.MaxSmallest && MaxLargest == other.MaxLargest;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this range.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			long hash = MinSmallest + MinLargest << 16 + MinLargest >> 48 + MaxSmallest << 32 + MaxSmallest >> 32 + MaxLargest << 48 + MaxLargest >> 16;
			return (int)(hash ^ ((long)((ulong)hash >> 32)));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this range as a {@code String}.
		/// <para>
		/// The format will be '{min}/{largestMin} - {smallestMax}/{max}',
		/// where the largestMin or smallestMax sections may be omitted, together
		/// with associated slash, if they are the same as the min or max.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this range, not null </returns>
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(MinSmallest);
			if (MinSmallest != MinLargest)
			{
				buf.Append('/').Append(MinLargest);
			}
			buf.Append(" - ").Append(MaxSmallest);
			if (MaxSmallest != MaxLargest)
			{
				buf.Append('/').Append(MaxLargest);
			}
			return buf.ToString();
		}

	}

}