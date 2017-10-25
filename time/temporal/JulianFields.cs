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
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
	/// A set of date fields that provide access to Julian Days.
	/// <para>
	/// The Julian Day is a standard way of expressing date and time commonly used in the scientific community.
	/// It is expressed as a decimal number of whole days where days start at midday.
	/// This class represents variations on Julian Days that count whole days from midnight.
	/// </para>
	/// <para>
	/// The fields are implemented relative to <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>.
	/// The fields are supported, and can be queried and set if {@code EPOCH_DAY} is available.
	/// The fields work with all chronologies.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe class.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class JulianFields
	{

		/// <summary>
		/// The offset from Julian to EPOCH DAY.
		/// </summary>
		private const long JULIAN_DAY_OFFSET = 2440588L;

		/// <summary>
		/// Julian Day field.
		/// <para>
		/// This is an integer-based version of the Julian Day Number.
		/// Julian Day is a well-known system that represents the count of whole days since day 0,
		/// which is defined to be January 1, 4713 BCE in the Julian calendar, and -4713-11-24 Gregorian.
		/// The field  has "JulianDay" as 'name', and 'DAYS' as 'baseUnit'.
		/// The field always refers to the local date-time, ignoring the offset or zone.
		/// </para>
		/// <para>
		/// For date-times, 'JULIAN_DAY.getFrom()' assumes the same value from
		/// midnight until just before the next midnight.
		/// When 'JULIAN_DAY.adjustInto()' is applied to a date-time, the time of day portion remains unaltered.
		/// 'JULIAN_DAY.adjustInto()' and 'JULIAN_DAY.getFrom()' only apply to {@code Temporal} objects that
		/// can be converted into <seealso cref="ChronoField#EPOCH_DAY"/>.
		/// An <seealso cref="UnsupportedTemporalTypeException"/> is thrown for any other type of object.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a Julian Day field.
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/> and <seealso cref="ResolverStyle#SMART smart mode"/>
		/// the Julian Day value is validated against the range of valid values.
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/> no validation occurs.
		/// 
		/// <h3>Astronomical and Scientific Notes</h3>
		/// The standard astronomical definition uses a fraction to indicate the time-of-day,
		/// thus 3.25 would represent the time 18:00, since days start at midday.
		/// This implementation uses an integer and days starting at midnight.
		/// The integer value for the Julian Day Number is the astronomical Julian Day value at midday
		/// of the date in question.
		/// This amounts to the astronomical Julian Day, rounded to an integer {@code JDN = floor(JD + 0.5)}.
		/// 
		/// <pre>
		///  | ISO date          |  Julian Day Number | Astronomical Julian Day |
		///  | 1970-01-01T00:00  |         2,440,588  |         2,440,587.5     |
		///  | 1970-01-01T06:00  |         2,440,588  |         2,440,587.75    |
		///  | 1970-01-01T12:00  |         2,440,588  |         2,440,588.0     |
		///  | 1970-01-01T18:00  |         2,440,588  |         2,440,588.25    |
		///  | 1970-01-02T00:00  |         2,440,589  |         2,440,588.5     |
		///  | 1970-01-02T06:00  |         2,440,589  |         2,440,588.75    |
		///  | 1970-01-02T12:00  |         2,440,589  |         2,440,589.0     |
		/// </pre>
		/// </para>
		/// <para>
		/// Julian Days are sometimes taken to imply Universal Time or UTC, but this
		/// implementation always uses the Julian Day number for the local date,
		/// regardless of the offset or time-zone.
		/// </para>
		/// </summary>
		public static readonly TemporalField JULIAN_DAY = Field.JULIAN_DAY;

		/// <summary>
		/// Modified Julian Day field.
		/// <para>
		/// This is an integer-based version of the Modified Julian Day Number.
		/// Modified Julian Day (MJD) is a well-known system that counts days continuously.
		/// It is defined relative to astronomical Julian Day as  {@code MJD = JD - 2400000.5}.
		/// Each Modified Julian Day runs from midnight to midnight.
		/// The field always refers to the local date-time, ignoring the offset or zone.
		/// </para>
		/// <para>
		/// For date-times, 'MODIFIED_JULIAN_DAY.getFrom()' assumes the same value from
		/// midnight until just before the next midnight.
		/// When 'MODIFIED_JULIAN_DAY.adjustInto()' is applied to a date-time, the time of day portion remains unaltered.
		/// 'MODIFIED_JULIAN_DAY.adjustInto()' and 'MODIFIED_JULIAN_DAY.getFrom()' only apply to {@code Temporal} objects
		/// that can be converted into <seealso cref="ChronoField#EPOCH_DAY"/>.
		/// An <seealso cref="UnsupportedTemporalTypeException"/> is thrown for any other type of object.
		/// </para>
		/// <para>
		/// This implementation is an integer version of MJD with the decimal part rounded to floor.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a Modified Julian Day field.
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/> and <seealso cref="ResolverStyle#SMART smart mode"/>
		/// the Modified Julian Day value is validated against the range of valid values.
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/> no validation occurs.
		/// 
		/// <h3>Astronomical and Scientific Notes</h3>
		/// <pre>
		///  | ISO date          | Modified Julian Day |      Decimal MJD |
		///  | 1970-01-01T00:00  |             40,587  |       40,587.0   |
		///  | 1970-01-01T06:00  |             40,587  |       40,587.25  |
		///  | 1970-01-01T12:00  |             40,587  |       40,587.5   |
		///  | 1970-01-01T18:00  |             40,587  |       40,587.75  |
		///  | 1970-01-02T00:00  |             40,588  |       40,588.0   |
		///  | 1970-01-02T06:00  |             40,588  |       40,588.25  |
		///  | 1970-01-02T12:00  |             40,588  |       40,588.5   |
		/// </pre>
		/// 
		/// Modified Julian Days are sometimes taken to imply Universal Time or UTC, but this
		/// implementation always uses the Modified Julian Day for the local date,
		/// regardless of the offset or time-zone.
		/// </para>
		/// </summary>
		public static readonly TemporalField MODIFIED_JULIAN_DAY = Field.MODIFIED_JULIAN_DAY;

		/// <summary>
		/// Rata Die field.
		/// <para>
		/// Rata Die counts whole days continuously starting day 1 at midnight at the beginning of 0001-01-01 (ISO).
		/// The field always refers to the local date-time, ignoring the offset or zone.
		/// </para>
		/// <para>
		/// For date-times, 'RATA_DIE.getFrom()' assumes the same value from
		/// midnight until just before the next midnight.
		/// When 'RATA_DIE.adjustInto()' is applied to a date-time, the time of day portion remains unaltered.
		/// 'RATA_DIE.adjustInto()' and 'RATA_DIE.getFrom()' only apply to {@code Temporal} objects
		/// that can be converted into <seealso cref="ChronoField#EPOCH_DAY"/>.
		/// An <seealso cref="UnsupportedTemporalTypeException"/> is thrown for any other type of object.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a Rata Die field.
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/> and <seealso cref="ResolverStyle#SMART smart mode"/>
		/// the Rata Die value is validated against the range of valid values.
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/> no validation occurs.
		/// </para>
		/// </summary>
		public static readonly TemporalField RATA_DIE = Field.RATA_DIE;

		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private JulianFields()
		{
			throw new AssertionError("Not instantiable");
		}

		/// <summary>
		/// Implementation of JulianFields.  Each instance is a singleton.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: private static enum Field implements TemporalField
		private enum Field
		{
			JULIAN_DAY("JulianDay", DAYS, FOREVER, JULIAN_DAY_OFFSET),
			MODIFIED_JULIAN_DAY("ModifiedJulianDay", DAYS, FOREVER, 40587L),
			RATA_DIE("RataDie", DAYS, FOREVER, 719163L);

			private static final long serialVersionUID = -7501623920830201812L;

			private final transient String name;
			private final transient TemporalUnit baseUnit;
			private final transient TemporalUnit rangeUnit;
			private final transient ValueRange range;
			private final transient long offset;

			private Field(String name, TemporalUnit baseUnit, TemporalUnit rangeUnit, long offset)
			{
				this.name = name
				this.baseUnit = baseUnit
				this.rangeUnit = rangeUnit
				this.range = ValueRange.of(-365243219162L + offset, 365241780471L + offset);
				this.offset = offset
			}

			//-----------------------------------------------------------------------
			public TemporalUnit getBaseUnit()
			{
				return baseUnit;
			}

			public TemporalUnit getRangeUnit()
			{
				return rangeUnit;
			}

			public boolean isDateBased()
			{
				return true;
			}

			public boolean isTimeBased()
			{
				return false;
			}

			public ValueRange range()
			{
				return range;
			}

			//-----------------------------------------------------------------------
			public boolean isSupportedBy(TemporalAccessor temporal)
			{
				return = EPOCH_DAY
			}

			public ValueRange rangeRefinedBy(TemporalAccessor temporal)
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (isSupportedBy(temporal) == false)
				{
					throw new java.time.DateTimeException("Unsupported field: " + this);
				}
				return = 
			}

			public long getFrom(TemporalAccessor temporal)
			{
				return temporal.getLong(EPOCH_DAY) + offset
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
			public <R> R adjustInto(R temporal, long newValue) where R : Temporal
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (range().isValidValue(newValue) == false)
				{
					throw new java.time.DateTimeException("Invalid value: " + name + " " + newValue);
				}
				return (R) temporal.with(EPOCH_DAY, Math.subtractExact(newValue, offset));
			}

			//-----------------------------------------------------------------------
			public java.time.chrono.ChronoLocalDate resolve(java.util.Map<TemporalField, Long> fieldValues, TemporalAccessor partialTemporal, java.time.format.ResolverStyle resolverStyle)
			{
				long = this
				java = partialTemporal
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (resolverStyle == java.time.format.ResolverStyle.LENIENT)
				{
					return = Math.subtractExact(value, offset)
				}
				range().checkValidValue(value, this);
				return = value - offset
			}

			//-----------------------------------------------------------------------
			public String toString()
			{
				return name;
			}
		}
	}

}