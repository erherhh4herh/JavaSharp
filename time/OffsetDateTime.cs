using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
 * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time
{



	/// <summary>
	/// A date-time with an offset from UTC/Greenwich in the ISO-8601 calendar system,
	/// such as {@code 2007-12-03T10:15:30+01:00}.
	/// <para>
	/// {@code OffsetDateTime} is an immutable representation of a date-time with an offset.
	/// This class stores all date and time fields, to a precision of nanoseconds,
	/// as well as the offset from UTC/Greenwich. For example, the value
	/// "2nd October 2007 at 13:45.30.123456789 +02:00" can be stored in an {@code OffsetDateTime}.
	/// </para>
	/// <para>
	/// {@code OffsetDateTime}, <seealso cref="java.time.ZonedDateTime"/> and <seealso cref="java.time.Instant"/> all store an instant
	/// on the time-line to nanosecond precision.
	/// {@code Instant} is the simplest, simply representing the instant.
	/// {@code OffsetDateTime} adds to the instant the offset from UTC/Greenwich, which allows
	/// the local date-time to be obtained.
	/// {@code ZonedDateTime} adds full time-zone rules.
	/// </para>
	/// <para>
	/// It is intended that {@code ZonedDateTime} or {@code Instant} is used to model data
	/// in simpler applications. This class may be used when modeling date-time concepts in
	/// more detail, or when communicating to a database or in a network protocol.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code OffsetDateTime} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class OffsetDateTime : Temporal, TemporalAdjuster, Comparable<OffsetDateTime>
	{

		/// <summary>
		/// The minimum supported {@code OffsetDateTime}, '-999999999-01-01T00:00:00+18:00'.
		/// This is the local date-time of midnight at the start of the minimum date
		/// in the maximum offset (larger offsets are earlier on the time-line).
		/// This combines <seealso cref="LocalDateTime#MIN"/> and <seealso cref="ZoneOffset#MAX"/>.
		/// This could be used by an application as a "far past" date-time.
		/// </summary>
		public static readonly OffsetDateTime MIN = LocalDateTime.MIN.AtOffset(ZoneOffset.MAX);
		/// <summary>
		/// The maximum supported {@code OffsetDateTime}, '+999999999-12-31T23:59:59.999999999-18:00'.
		/// This is the local date-time just before midnight at the end of the maximum date
		/// in the minimum offset (larger negative offsets are later on the time-line).
		/// This combines <seealso cref="LocalDateTime#MAX"/> and <seealso cref="ZoneOffset#MIN"/>.
		/// This could be used by an application as a "far future" date-time.
		/// </summary>
		public static readonly OffsetDateTime MAX = LocalDateTime.MAX.AtOffset(ZoneOffset.MIN);

		/// <summary>
		/// Gets a comparator that compares two {@code OffsetDateTime} instances
		/// based solely on the instant.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying instant.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a comparator that compares in time-line order
		/// </returns>
		/// <seealso cref= #isAfter </seealso>
		/// <seealso cref= #isBefore </seealso>
		/// <seealso cref= #isEqual </seealso>
		public static IComparer<OffsetDateTime> TimeLineOrder()
		{
			return OffsetDateTime::compareInstant;
		}

		/// <summary>
		/// Compares this {@code OffsetDateTime} to another date-time.
		/// The comparison is based on the instant.
		/// </summary>
		/// <param name="datetime1">  the first date-time to compare, not null </param>
		/// <param name="datetime2">  the other date-time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		private static int CompareInstant(OffsetDateTime datetime1, OffsetDateTime datetime2)
		{
			if (datetime1.Offset.Equals(datetime2.Offset))
			{
				return datetime1.ToLocalDateTime().CompareTo(datetime2.ToLocalDateTime());
			}
			int cmp = Long.Compare(datetime1.ToEpochSecond(), datetime2.ToEpochSecond());
			if (cmp == 0)
			{
				cmp = datetime1.ToLocalTime().Nano - datetime2.ToLocalTime().Nano;
			}
			return cmp;
		}

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 2287754244819255394L;

		/// <summary>
		/// The local date-time.
		/// </summary>
		private readonly LocalDateTime DateTime;
		/// <summary>
		/// The offset from UTC/Greenwich.
		/// </summary>
		private readonly ZoneOffset Offset_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current date-time from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current date-time.
		/// The offset will be calculated from the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current date-time using the system clock, not null </returns>
		public static OffsetDateTime Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current date-time from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date-time.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// The offset will be calculated from the specified time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current date-time using the system clock, not null </returns>
		public static OffsetDateTime Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current date-time from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date-time.
		/// The offset will be calculated from the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date-time, not null </returns>
		public static OffsetDateTime Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			return OfInstant(now, clock.Zone.Rules.GetOffset(now));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a date, time and offset.
		/// <para>
		/// This creates an offset date-time with the specified local date, time and offset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date">  the local date, not null </param>
		/// <param name="time">  the local time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the offset date-time, not null </returns>
		public static OffsetDateTime Of(LocalDate date, LocalTime time, ZoneOffset offset)
		{
			LocalDateTime dt = LocalDateTime.Of(date, time);
			return new OffsetDateTime(dt, offset);
		}

		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a date-time and offset.
		/// <para>
		/// This creates an offset date-time with the specified local date-time and offset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateTime">  the local date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the offset date-time, not null </returns>
		public static OffsetDateTime Of(LocalDateTime dateTime, ZoneOffset offset)
		{
			return new OffsetDateTime(dateTime, offset);
		}

		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a year, month, day,
		/// hour, minute, second, nanosecond and offset.
		/// <para>
		/// This creates an offset date-time with the seven specified fields.
		/// </para>
		/// <para>
		/// This method exists primarily for writing test cases.
		/// Non test-code will typically use other methods to create an offset time.
		/// {@code LocalDateTime} has five additional convenience variants of the
		/// equivalent factory method taking fewer arguments.
		/// They are not provided here to reduce the footprint of the API.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the offset date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range, or
		///  if the day-of-month is invalid for the month-year </exception>
		public static OffsetDateTime Of(int year, int month, int dayOfMonth, int hour, int minute, int second, int nanoOfSecond, ZoneOffset offset)
		{
			LocalDateTime dt = LocalDateTime.Of(year, month, dayOfMonth, hour, minute, second, nanoOfSecond);
			return new OffsetDateTime(dt, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from an {@code Instant} and zone ID.
		/// <para>
		/// This creates an offset date-time with the same instant as that specified.
		/// Finding the offset from UTC/Greenwich is simple as there is only one valid
		/// offset for each instant.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to create the date-time from, not null </param>
		/// <param name="zone">  the time-zone, which may be an offset, not null </param>
		/// <returns> the offset date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public static OffsetDateTime OfInstant(Instant instant, ZoneId zone)
		{
			Objects.RequireNonNull(instant, "instant");
			Objects.RequireNonNull(zone, "zone");
			ZoneRules rules = zone.Rules;
			ZoneOffset offset = rules.GetOffset(instant);
			LocalDateTime ldt = LocalDateTime.OfEpochSecond(instant.EpochSecond, instant.Nano, offset);
			return new OffsetDateTime(ldt, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a temporal object.
		/// <para>
		/// This obtains an offset date-time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code OffsetDateTime}.
		/// </para>
		/// <para>
		/// The conversion will first obtain a {@code ZoneOffset} from the temporal object.
		/// It will then try to obtain a {@code LocalDateTime}, falling back to an {@code Instant} if necessary.
		/// The result will be the combination of {@code ZoneOffset} with either
		/// with {@code LocalDateTime} or {@code Instant}.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code OffsetDateTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the offset date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to an {@code OffsetDateTime} </exception>
		public static OffsetDateTime From(TemporalAccessor temporal)
		{
			if (temporal is OffsetDateTime)
			{
				return (OffsetDateTime) temporal;
			}
			try
			{
				ZoneOffset offset = ZoneOffset.From(temporal);
				LocalDate date = temporal.query(TemporalQueries.LocalDate());
				LocalTime time = temporal.query(TemporalQueries.LocalTime());
				if (date != temporal.TemporalAccessor_Fields.Null && time != temporal.TemporalAccessor_Fields.Null)
				{
					return OffsetDateTime.Of(date, time, offset);
				}
				else
				{
					Instant instant = Instant.From(temporal);
					return OffsetDateTime.OfInstant(instant, offset);
				}
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain OffsetDateTime from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a text string
		/// such as {@code 2007-12-03T10:15:30+01:00}.
		/// <para>
		/// The string must represent a valid date-time and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_OFFSET_DATE_TIME"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007-12-03T10:15:30+01:00", not null </param>
		/// <returns> the parsed offset date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static OffsetDateTime Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_OFFSET_DATE_TIME);
		}

		/// <summary>
		/// Obtains an instance of {@code OffsetDateTime} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a date-time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed offset date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static OffsetDateTime Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, OffsetDateTime::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dateTime">  the local date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		private OffsetDateTime(LocalDateTime dateTime, ZoneOffset offset)
		{
			this.DateTime = Objects.RequireNonNull(dateTime, "dateTime");
			this.Offset_Renamed = Objects.RequireNonNull(offset, "offset");
		}

		/// <summary>
		/// Returns a new date-time based on this one, returning {@code this} where possible.
		/// </summary>
		/// <param name="dateTime">  the date-time to create with, not null </param>
		/// <param name="offset">  the zone offset to create with, not null </param>
		private OffsetDateTime With(LocalDateTime dateTime, ZoneOffset offset)
		{
			if (this.DateTime == dateTime && this.Offset_Renamed.Equals(offset))
			{
				return this;
			}
			return new OffsetDateTime(dateTime, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this date-time can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
		/// <li>{@code NANO_OF_SECOND}
		/// <li>{@code NANO_OF_DAY}
		/// <li>{@code MICRO_OF_SECOND}
		/// <li>{@code MICRO_OF_DAY}
		/// <li>{@code MILLI_OF_SECOND}
		/// <li>{@code MILLI_OF_DAY}
		/// <li>{@code SECOND_OF_MINUTE}
		/// <li>{@code SECOND_OF_DAY}
		/// <li>{@code MINUTE_OF_HOUR}
		/// <li>{@code MINUTE_OF_DAY}
		/// <li>{@code HOUR_OF_AMPM}
		/// <li>{@code CLOCK_HOUR_OF_AMPM}
		/// <li>{@code HOUR_OF_DAY}
		/// <li>{@code CLOCK_HOUR_OF_DAY}
		/// <li>{@code AMPM_OF_DAY}
		/// <li>{@code DAY_OF_WEEK}
		/// <li>{@code ALIGNED_DAY_OF_WEEK_IN_MONTH}
		/// <li>{@code ALIGNED_DAY_OF_WEEK_IN_YEAR}
		/// <li>{@code DAY_OF_MONTH}
		/// <li>{@code DAY_OF_YEAR}
		/// <li>{@code EPOCH_DAY}
		/// <li>{@code ALIGNED_WEEK_OF_MONTH}
		/// <li>{@code ALIGNED_WEEK_OF_YEAR}
		/// <li>{@code MONTH_OF_YEAR}
		/// <li>{@code PROLEPTIC_MONTH}
		/// <li>{@code YEAR_OF_ERA}
		/// <li>{@code YEAR}
		/// <li>{@code ERA}
		/// <li>{@code INSTANT_SECONDS}
		/// <li>{@code OFFSET_SECONDS}
		/// </ul>
		/// All other {@code ChronoField} instances will return false.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the field is supported is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to check, null returns false </param>
		/// <returns> true if the field is supported on this date-time, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			return field is ChronoField || (field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this));
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this date-time.
		/// If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		/// <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		/// The supported units are:
		/// <ul>
		/// <li>{@code NANOS}
		/// <li>{@code MICROS}
		/// <li>{@code MILLIS}
		/// <li>{@code SECONDS}
		/// <li>{@code MINUTES}
		/// <li>{@code HOURS}
		/// <li>{@code HALF_DAYS}
		/// <li>{@code DAYS}
		/// <li>{@code WEEKS}
		/// <li>{@code MONTHS}
		/// <li>{@code YEARS}
		/// <li>{@code DECADES}
		/// <li>{@code CENTURIES}
		/// <li>{@code MILLENNIA}
		/// <li>{@code ERAS}
		/// </ul>
		/// All other {@code ChronoUnit} instances will return false.
		/// </para>
		/// <para>
		/// If the unit is not a {@code ChronoUnit}, then the result of this method
		/// is obtained by invoking {@code TemporalUnit.isSupportedBy(Temporal)}
		/// passing {@code this} as the argument.
		/// Whether the unit is supported is determined by the unit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to check, null returns false </param>
		/// <returns> true if the unit can be added/subtracted, false if not </returns>
		public bool IsSupported(TemporalUnit unit) // override for Javadoc
		{
			if (unit is ChronoUnit)
			{
				return unit != FOREVER;
			}
			return unit != temporal.TemporalAccessor_Fields.Null && unit.isSupportedBy(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This date-time is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return
		/// appropriate range instances.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the range can be obtained is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to query the range for, not null </param>
		/// <returns> the range of valid values for the field, not null </returns>
		/// <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		public override ValueRange temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field is ChronoField)
			{
				if (field == INSTANT_SECONDS || field == OFFSET_SECONDS)
				{
					return field.Range();
				}
				return DateTime.Range(field);
			}
			return field.RangeRefinedBy(this);
		}

		/// <summary>
		/// Gets the value of the specified field from this date-time as an {@code int}.
		/// <para>
		/// This queries this date-time for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this date-time, except {@code NANO_OF_DAY}, {@code MICRO_OF_DAY},
		/// {@code EPOCH_DAY}, {@code PROLEPTIC_MONTH} and {@code INSTANT_SECONDS} which are too
		/// large to fit in an {@code int} and throw a {@code DateTimeException}.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		/// passing {@code this} as the argument. Whether the value can be obtained,
		/// and what the value represents, is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get, not null </param>
		/// <returns> the value for the field </returns>
		/// <exception cref="DateTimeException"> if a value for the field cannot be obtained or
		///         the value is outside the range of valid values for the field </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported or
		///         the range of values exceeds an {@code int} </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override int Get(TemporalField field)
		{
			if (field is ChronoField)
			{
				switch ((ChronoField) field)
				{
					case INSTANT_SECONDS:
						throw new UnsupportedTemporalTypeException("Invalid field 'InstantSeconds' for get() method, use getLong() instead");
					case OFFSET_SECONDS:
						return Offset.TotalSeconds;
				}
				return DateTime.Get(field);
			}
			return Temporal.this.get(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this date-time as a {@code long}.
		/// <para>
		/// This queries this date-time for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this date-time.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		/// passing {@code this} as the argument. Whether the value can be obtained,
		/// and what the value represents, is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get, not null </param>
		/// <returns> the value for the field </returns>
		/// <exception cref="DateTimeException"> if a value for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long GetLong(TemporalField field)
		{
			if (field is ChronoField)
			{
				switch ((ChronoField) field)
				{
					case INSTANT_SECONDS:
						return ToEpochSecond();
					case OFFSET_SECONDS:
						return Offset.TotalSeconds;
				}
				return DateTime.GetLong(field);
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the zone offset, such as '+01:00'.
		/// <para>
		/// This is the offset of the local date-time from UTC/Greenwich.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the zone offset, not null </returns>
		public ZoneOffset Offset
		{
			get
			{
				return Offset_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified offset ensuring
		/// that the result has the same local date-time.
		/// <para>
		/// This method returns an object with the same {@code LocalDateTime} and the specified {@code ZoneOffset}.
		/// No calculation is needed or performed.
		/// For example, if this time represents {@code 2007-12-03T10:30+02:00} and the offset specified is
		/// {@code +03:00}, then this method will return {@code 2007-12-03T10:30+03:00}.
		/// </para>
		/// <para>
		/// To take into account the difference between the offsets, and adjust the time fields,
		/// use <seealso cref="#withOffsetSameInstant"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the zone offset to change to, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested offset, not null </returns>
		public OffsetDateTime WithOffsetSameLocal(ZoneOffset offset)
		{
			return With(DateTime, offset);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified offset ensuring
		/// that the result is at the same instant.
		/// <para>
		/// This method returns an object with the specified {@code ZoneOffset} and a {@code LocalDateTime}
		/// adjusted by the difference between the two offsets.
		/// This will result in the old and new objects representing the same instant.
		/// This is useful for finding the local time in a different offset.
		/// For example, if this time represents {@code 2007-12-03T10:30+02:00} and the offset specified is
		/// {@code +03:00}, then this method will return {@code 2007-12-03T11:30+03:00}.
		/// </para>
		/// <para>
		/// To change the offset without adjusting the local time use <seealso cref="#withOffsetSameLocal"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the zone offset to change to, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested offset, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime WithOffsetSameInstant(ZoneOffset offset)
		{
			if (offset.Equals(this.Offset_Renamed))
			{
				return this;
			}
			int difference = offset.TotalSeconds - this.Offset_Renamed.TotalSeconds;
			LocalDateTime adjusted = DateTime.PlusSeconds(difference);
			return new OffsetDateTime(adjusted, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the {@code LocalDateTime} part of this date-time.
		/// <para>
		/// This returns a {@code LocalDateTime} with the same year, month, day and time
		/// as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the local date-time part of this date-time, not null </returns>
		public LocalDateTime ToLocalDateTime()
		{
			return DateTime;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the {@code LocalDate} part of this date-time.
		/// <para>
		/// This returns a {@code LocalDate} with the same year, month and day
		/// as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the date part of this date-time, not null </returns>
		public LocalDate ToLocalDate()
		{
			return DateTime.ToLocalDate();
		}

		/// <summary>
		/// Gets the year field.
		/// <para>
		/// This method returns the primitive {@code int} value for the year.
		/// </para>
		/// <para>
		/// The year returned by this method is proleptic as per {@code get(YEAR)}.
		/// To obtain the year-of-era, use {@code get(YEAR_OF_ERA)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the year, from MIN_YEAR to MAX_YEAR </returns>
		public int Year
		{
			get
			{
				return DateTime.Year;
			}
		}

		/// <summary>
		/// Gets the month-of-year field from 1 to 12.
		/// <para>
		/// This method returns the month as an {@code int} from 1 to 12.
		/// Application code is frequently clearer if the enum <seealso cref="Month"/>
		/// is used by calling <seealso cref="#getMonth()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month-of-year, from 1 to 12 </returns>
		/// <seealso cref= #getMonth() </seealso>
		public int MonthValue
		{
			get
			{
				return DateTime.MonthValue;
			}
		}

		/// <summary>
		/// Gets the month-of-year field using the {@code Month} enum.
		/// <para>
		/// This method returns the enum <seealso cref="Month"/> for the month.
		/// This avoids confusion as to what {@code int} values mean.
		/// If you need access to the primitive {@code int} value then the enum
		/// provides the <seealso cref="Month#getValue() int value"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month-of-year, not null </returns>
		/// <seealso cref= #getMonthValue() </seealso>
		public Month Month
		{
			get
			{
				return DateTime.Month;
			}
		}

		/// <summary>
		/// Gets the day-of-month field.
		/// <para>
		/// This method returns the primitive {@code int} value for the day-of-month.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-month, from 1 to 31 </returns>
		public int DayOfMonth
		{
			get
			{
				return DateTime.DayOfMonth;
			}
		}

		/// <summary>
		/// Gets the day-of-year field.
		/// <para>
		/// This method returns the primitive {@code int} value for the day-of-year.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-year, from 1 to 365, or 366 in a leap year </returns>
		public int DayOfYear
		{
			get
			{
				return DateTime.DayOfYear;
			}
		}

		/// <summary>
		/// Gets the day-of-week field, which is an enum {@code DayOfWeek}.
		/// <para>
		/// This method returns the enum <seealso cref="DayOfWeek"/> for the day-of-week.
		/// This avoids confusion as to what {@code int} values mean.
		/// If you need access to the primitive {@code int} value then the enum
		/// provides the <seealso cref="DayOfWeek#getValue() int value"/>.
		/// </para>
		/// <para>
		/// Additional information can be obtained from the {@code DayOfWeek}.
		/// This includes textual names of the values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-week, not null </returns>
		public DayOfWeek DayOfWeek
		{
			get
			{
				return DateTime.DayOfWeek;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the {@code LocalTime} part of this date-time.
		/// <para>
		/// This returns a {@code LocalTime} with the same hour, minute, second and
		/// nanosecond as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time part of this date-time, not null </returns>
		public LocalTime ToLocalTime()
		{
			return DateTime.ToLocalTime();
		}

		/// <summary>
		/// Gets the hour-of-day field.
		/// </summary>
		/// <returns> the hour-of-day, from 0 to 23 </returns>
		public int Hour
		{
			get
			{
				return DateTime.Hour;
			}
		}

		/// <summary>
		/// Gets the minute-of-hour field.
		/// </summary>
		/// <returns> the minute-of-hour, from 0 to 59 </returns>
		public int Minute
		{
			get
			{
				return DateTime.Minute;
			}
		}

		/// <summary>
		/// Gets the second-of-minute field.
		/// </summary>
		/// <returns> the second-of-minute, from 0 to 59 </returns>
		public int Second
		{
			get
			{
				return DateTime.Second;
			}
		}

		/// <summary>
		/// Gets the nano-of-second field.
		/// </summary>
		/// <returns> the nano-of-second, from 0 to 999,999,999 </returns>
		public int Nano
		{
			get
			{
				return DateTime.Nano;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this date-time.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the date-time adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
		/// </para>
		/// <para>
		/// A simple adjuster might simply set the one of the fields, such as the year field.
		/// A more complex adjuster might set the date to the last day of the month.
		/// A selection of common adjustments is provided in
		/// <seealso cref="java.time.temporal.TemporalAdjusters TemporalAdjusters"/>.
		/// These include finding the "last day of the month" and "next Wednesday".
		/// Key date-time classes also implement the {@code TemporalAdjuster} interface,
		/// such as <seealso cref="Month"/> and <seealso cref="java.time.MonthDay MonthDay"/>.
		/// The adjuster is responsible for handling special cases, such as the varying
		/// lengths of month and leap years.
		/// </para>
		/// <para>
		/// For example this code returns a date on the last day of July:
		/// <pre>
		///  import static java.time.Month.*;
		///  import static java.time.temporal.TemporalAdjusters.*;
		/// 
		///  result = offsetDateTime.with(JULY).with(lastDayOfMonth());
		/// </pre>
		/// </para>
		/// <para>
		/// The classes <seealso cref="LocalDate"/>, <seealso cref="LocalTime"/> and <seealso cref="ZoneOffset"/> implement
		/// {@code TemporalAdjuster}, thus this method can be used to change the date, time or offset:
		/// <pre>
		///  result = offsetDateTime.with(date);
		///  result = offsetDateTime.with(time);
		///  result = offsetDateTime.with(offset);
		/// </pre>
		/// </para>
		/// <para>
		/// The result of this method is obtained by invoking the
		/// <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		/// specified adjuster passing {@code this} as the argument.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="adjuster"> the adjuster to use, not null </param>
		/// <returns> an {@code OffsetDateTime} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetDateTime With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalDate || adjuster is LocalTime || adjuster is LocalDateTime)
			{
				return With(DateTime.With(adjuster), Offset_Renamed);
			}
			else if (adjuster is Instant)
			{
				return OfInstant((Instant) adjuster, Offset_Renamed);
			}
			else if (adjuster is ZoneOffset)
			{
				return With(DateTime, (ZoneOffset) adjuster);
			}
			else if (adjuster is OffsetDateTime)
			{
				return (OffsetDateTime) adjuster;
			}
			return (OffsetDateTime) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified field set to a new value.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the value
		/// for the specified field changed.
		/// This can be used to change any supported field, such as the year, month or day-of-month.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// In some cases, changing the specified field can cause the resulting date-time to become invalid,
		/// such as changing the month from 31st January to February would make the day-of-month invalid.
		/// In cases like this, the field is responsible for resolving the date. Typically it will choose
		/// the previous valid date, which would be the last valid day of February in this example.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// </para>
		/// <para>
		/// The {@code INSTANT_SECONDS} field will return a date-time with the specified instant.
		/// The offset and nano-of-second are unchanged.
		/// If the new instant value is outside the valid range then a {@code DateTimeException} will be thrown.
		/// </para>
		/// <para>
		/// The {@code OFFSET_SECONDS} field will return a date-time with the specified offset.
		/// The local date-time is unaltered. If the new offset value is outside the valid range
		/// then a {@code DateTimeException} will be thrown.
		/// </para>
		/// <para>
		/// The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		/// the matching method on <seealso cref="LocalDateTime#with(TemporalField, long) LocalDateTime"/>.
		/// In this case, the offset is not part of the calculation and will be unchanged.
		/// </para>
		/// <para>
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.adjustInto(Temporal, long)}
		/// passing {@code this} as the argument. In this case, the field determines
		/// whether and how to adjust the instant.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to set in the result, not null </param>
		/// <param name="newValue">  the new value of the field in the result </param>
		/// <returns> an {@code OffsetDateTime} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public OffsetDateTime With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				switch (f)
				{
					case INSTANT_SECONDS:
						return OfInstant(Instant.OfEpochSecond(newValue, Nano), Offset_Renamed);
					case OFFSET_SECONDS:
					{
						return With(DateTime, ZoneOffset.OfTotalSeconds(f.checkValidIntValue(newValue)));
					}
				}
				return With(DateTime.With(field, newValue), Offset_Renamed);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the year altered.
		/// <para>
		/// The time and offset do not affect the calculation and will be the same in the result.
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested year, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public OffsetDateTime WithYear(int year)
		{
			return With(DateTime.WithYear(year), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the month-of-year altered.
		/// <para>
		/// The time and offset do not affect the calculation and will be the same in the result.
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public OffsetDateTime WithMonth(int month)
		{
			return With(DateTime.WithMonth(month), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the day-of-month altered.
		/// <para>
		/// If the resulting {@code OffsetDateTime} is invalid, an exception is thrown.
		/// The time and offset do not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month value is invalid,
		///  or if the day-of-month is invalid for the month-year </exception>
		public OffsetDateTime WithDayOfMonth(int dayOfMonth)
		{
			return With(DateTime.WithDayOfMonth(dayOfMonth), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the day-of-year altered.
		/// <para>
		/// The time and offset do not affect the calculation and will be the same in the result.
		/// If the resulting {@code OffsetDateTime} is invalid, an exception is thrown.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		/// <returns> an {@code OffsetDateTime} based on this date with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-year value is invalid,
		///  or if the day-of-year is invalid for the year </exception>
		public OffsetDateTime WithDayOfYear(int dayOfYear)
		{
			return With(DateTime.WithDayOfYear(dayOfYear), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the hour-of-day altered.
		/// <para>
		/// The date and offset do not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested hour, not null </returns>
		/// <exception cref="DateTimeException"> if the hour value is invalid </exception>
		public OffsetDateTime WithHour(int hour)
		{
			return With(DateTime.WithHour(hour), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the minute-of-hour altered.
		/// <para>
		/// The date and offset do not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested minute, not null </returns>
		/// <exception cref="DateTimeException"> if the minute value is invalid </exception>
		public OffsetDateTime WithMinute(int minute)
		{
			return With(DateTime.WithMinute(minute), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the second-of-minute altered.
		/// <para>
		/// The date and offset do not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested second, not null </returns>
		/// <exception cref="DateTimeException"> if the second value is invalid </exception>
		public OffsetDateTime WithSecond(int second)
		{
			return With(DateTime.WithSecond(second), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the nano-of-second altered.
		/// <para>
		/// The date and offset do not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the requested nanosecond, not null </returns>
		/// <exception cref="DateTimeException"> if the nano value is invalid </exception>
		public OffsetDateTime WithNano(int nanoOfSecond)
		{
			return With(DateTime.WithNano(nanoOfSecond), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the time truncated.
		/// <para>
		/// Truncation returns a copy of the original date-time with fields
		/// smaller than the specified unit set to zero.
		/// For example, truncating with the <seealso cref="ChronoUnit#MINUTES minutes"/> unit
		/// will set the second-of-minute and nano-of-second field to zero.
		/// </para>
		/// <para>
		/// The unit must have a <seealso cref="TemporalUnit#getDuration() duration"/>
		/// that divides into the length of a standard day without remainder.
		/// This includes all supplied time units on <seealso cref="ChronoUnit"/> and
		/// <seealso cref="ChronoUnit#DAYS DAYS"/>. Other units throw an exception.
		/// </para>
		/// <para>
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to truncate to, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if unable to truncate </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public OffsetDateTime TruncatedTo(TemporalUnit unit)
		{
			return With(DateTime.TruncatedTo(unit), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the specified amount added.
		/// The amount is typically <seealso cref="Period"/> or <seealso cref="Duration"/> but may be
		/// any other type implementing the <seealso cref="TemporalAmount"/> interface.
		/// </para>
		/// <para>
		/// The calculation is delegated to the amount object by calling
		/// <seealso cref="TemporalAmount#addTo(Temporal)"/>. The amount implementation is free
		/// to implement the addition in any way it wishes, however it typically
		/// calls back to <seealso cref="#plus(long, TemporalUnit)"/>. Consult the documentation
		/// of the amount implementation to determine if it can be successfully added.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToAdd">  the amount to add, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetDateTime Plus(TemporalAmount amountToAdd)
		{
			return (OffsetDateTime) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented by
		/// <seealso cref="LocalDateTime#plus(long, TemporalUnit)"/>.
		/// The offset is not part of the calculation and will be unchanged in the result.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoUnit}, then the result of this method
		/// is obtained by invoking {@code TemporalUnit.addTo(Temporal, long)}
		/// passing {@code this} as the argument. In this case, the unit determines
		/// whether and how to perform the addition.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToAdd">  the amount of the unit to add to the result, may be negative </param>
		/// <param name="unit">  the unit of the amount to add, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public OffsetDateTime Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				return With(DateTime.Plus(amountToAdd, unit), Offset_Renamed);
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of years added.
		/// <para>
		/// This method adds the specified amount to the years field in three steps:
		/// <ol>
		/// <li>Add the input years to the year field</li>
		/// <li>Check if the resulting date would be invalid</li>
		/// <li>Adjust the day-of-month to the last valid day if necessary</li>
		/// </ol>
		/// </para>
		/// <para>
		/// For example, 2008-02-29 (leap year) plus one year would result in the
		/// invalid date 2009-02-29 (standard year). Instead of returning an invalid
		/// result, the last valid day of the month, 2009-02-28, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the years to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusYears(long years)
		{
			return With(DateTime.PlusYears(years), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of months added.
		/// <para>
		/// This method adds the specified amount to the months field in three steps:
		/// <ol>
		/// <li>Add the input months to the month-of-year field</li>
		/// <li>Check if the resulting date would be invalid</li>
		/// <li>Adjust the day-of-month to the last valid day if necessary</li>
		/// </ol>
		/// </para>
		/// <para>
		/// For example, 2007-03-31 plus one month would result in the invalid date
		/// 2007-04-31. Instead of returning an invalid result, the last valid day
		/// of the month, 2007-04-30, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusMonths(long months)
		{
			return With(DateTime.PlusMonths(months), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this OffsetDateTime with the specified number of weeks added.
		/// <para>
		/// This method adds the specified amount in weeks to the days field incrementing
		/// the month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2008-12-31 plus one week would result in 2009-01-07.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the weeks to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the weeks added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusWeeks(long weeks)
		{
			return With(DateTime.PlusWeeks(weeks), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this OffsetDateTime with the specified number of days added.
		/// <para>
		/// This method adds the specified amount to the days field incrementing the
		/// month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2008-12-31 plus one day would result in 2009-01-01.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the days added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusDays(long days)
		{
			return With(DateTime.PlusDays(days), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of hours added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the hours added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusHours(long hours)
		{
			return With(DateTime.PlusHours(hours), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of minutes added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the minutes added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusMinutes(long minutes)
		{
			return With(DateTime.PlusMinutes(minutes), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of seconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the seconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime PlusSeconds(long seconds)
		{
			return With(DateTime.PlusSeconds(seconds), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of nanoseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to add, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the nanoseconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the unit cannot be added to this type </exception>
		public OffsetDateTime PlusNanos(long nanos)
		{
			return With(DateTime.PlusNanos(nanos), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the specified amount subtracted.
		/// The amount is typically <seealso cref="Period"/> or <seealso cref="Duration"/> but may be
		/// any other type implementing the <seealso cref="TemporalAmount"/> interface.
		/// </para>
		/// <para>
		/// The calculation is delegated to the amount object by calling
		/// <seealso cref="TemporalAmount#subtractFrom(Temporal)"/>. The amount implementation is free
		/// to implement the subtraction in any way it wishes, however it typically
		/// calls back to <seealso cref="#minus(long, TemporalUnit)"/>. Consult the documentation
		/// of the amount implementation to determine if it can be successfully subtracted.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToSubtract">  the amount to subtract, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetDateTime Minus(TemporalAmount amountToSubtract)
		{
			return (OffsetDateTime) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns an {@code OffsetDateTime}, based on this one, with the amount
		/// in terms of the unit subtracted. If it is not possible to subtract the amount,
		/// because the unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// This method is equivalent to <seealso cref="#plus(long, TemporalUnit)"/> with the amount negated.
		/// See that method for a full description of how addition, and thus subtraction, works.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToSubtract">  the amount of the unit to subtract from the result, may be negative </param>
		/// <param name="unit">  the unit of the amount to subtract, not null </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetDateTime Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of years subtracted.
		/// <para>
		/// This method subtracts the specified amount from the years field in three steps:
		/// <ol>
		/// <li>Subtract the input years from the year field</li>
		/// <li>Check if the resulting date would be invalid</li>
		/// <li>Adjust the day-of-month to the last valid day if necessary</li>
		/// </ol>
		/// </para>
		/// <para>
		/// For example, 2008-02-29 (leap year) minus one year would result in the
		/// invalid date 2009-02-29 (standard year). Instead of returning an invalid
		/// result, the last valid day of the month, 2009-02-28, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the years to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusYears(long years)
		{
			return (years == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-years));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of months subtracted.
		/// <para>
		/// This method subtracts the specified amount from the months field in three steps:
		/// <ol>
		/// <li>Subtract the input months from the month-of-year field</li>
		/// <li>Check if the resulting date would be invalid</li>
		/// <li>Adjust the day-of-month to the last valid day if necessary</li>
		/// </ol>
		/// </para>
		/// <para>
		/// For example, 2007-03-31 minus one month would result in the invalid date
		/// 2007-04-31. Instead of returning an invalid result, the last valid day
		/// of the month, 2007-04-30, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusMonths(long months)
		{
			return (months == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-months));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of weeks subtracted.
		/// <para>
		/// This method subtracts the specified amount in weeks from the days field decrementing
		/// the month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2008-12-31 minus one week would result in 2009-01-07.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the weeks to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the weeks subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusWeeks(long weeks)
		{
			return (weeks == Long.MinValue ? PlusWeeks(Long.MaxValue).PlusWeeks(1) : PlusWeeks(-weeks));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of days subtracted.
		/// <para>
		/// This method subtracts the specified amount from the days field decrementing the
		/// month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2008-12-31 minus one day would result in 2009-01-01.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the days subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusDays(long days)
		{
			return (days == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-days));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of hours subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the hours subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusHours(long hours)
		{
			return (hours == Long.MinValue ? PlusHours(Long.MaxValue).PlusHours(1) : PlusHours(-hours));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of minutes subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the minutes subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusMinutes(long minutes)
		{
			return (minutes == Long.MinValue ? PlusMinutes(Long.MaxValue).PlusMinutes(1) : PlusMinutes(-minutes));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of seconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the seconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusSeconds(long seconds)
		{
			return (seconds == Long.MinValue ? PlusSeconds(Long.MaxValue).PlusSeconds(1) : PlusSeconds(-seconds));
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetDateTime} with the specified number of nanoseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to subtract, may be negative </param>
		/// <returns> an {@code OffsetDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public OffsetDateTime MinusNanos(long nanos)
		{
			return (nanos == Long.MinValue ? PlusNanos(Long.MaxValue).PlusNanos(1) : PlusNanos(-nanos));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this date-time using the specified query.
		/// <para>
		/// This queries this date-time using the specified query strategy object.
		/// The {@code TemporalQuery} object defines the logic to be used to
		/// obtain the result. Read the documentation of the query to understand
		/// what the result of this method will be.
		/// </para>
		/// <para>
		/// The result of this method is obtained by invoking the
		/// <seealso cref="TemporalQuery#queryFrom(TemporalAccessor)"/> method on the
		/// specified query passing {@code this} as the argument.
		/// 
		/// </para>
		/// </summary>
		/// @param <R> the type of the result </param>
		/// <param name="query">  the query to invoke, not null </param>
		/// <returns> the query result, null may be returned (defined by the query) </returns>
		/// <exception cref="DateTimeException"> if unable to query (defined by the query) </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs (defined by the query) </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R> R query(java.time.temporal.TemporalQuery<R> query)
		public override R query<R>(TemporalQuery<R> query)
		{
			if (query == TemporalQueries.Offset() || query == TemporalQueries.Zone())
			{
				return (R) Offset;
			}
			else if (query == TemporalQueries.ZoneId())
			{
				return temporal.TemporalAccessor_Fields.Null;
			}
			else if (query == TemporalQueries.LocalDate())
			{
				return (R) ToLocalDate();
			}
			else if (query == TemporalQueries.LocalTime())
			{
				return (R) ToLocalTime();
			}
			else if (query == TemporalQueries.Chronology())
			{
				return (R) IsoChronology.INSTANCE;
			}
			else if (query == TemporalQueries.Precision())
			{
				return (R) NANOS;
			}
			// inline TemporalAccessor.super.query(query) as an optimization
			// non-JDK classes are not permitted to make this optimization
			return query.QueryFrom(this);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have the same offset, date
		/// and time as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the offset, date and time changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// three times, passing <seealso cref="ChronoField#EPOCH_DAY"/>,
		/// <seealso cref="ChronoField#NANO_OF_DAY"/> and <seealso cref="ChronoField#OFFSET_SECONDS"/> as the fields.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisOffsetDateTime.adjustInto(temporal);
		///   temporal = temporal.with(thisOffsetDateTime);
		/// </pre>
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the target object to be adjusted, not null </param>
		/// <returns> the adjusted object, not null </returns>
		/// <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Temporal AdjustInto(Temporal temporal)
		{
			// OffsetDateTime is treated as three separate fields, not an instant
			// this produces the most consistent set of results overall
			// the offset is set after the date and time, as it is typically a small
			// tweak to the result, with ZonedDateTime frequently ignoring the offset
			return temporal.With(EPOCH_DAY, ToLocalDate().ToEpochDay()).With(NANO_OF_DAY, ToLocalTime().ToNanoOfDay()).With(OFFSET_SECONDS, Offset.TotalSeconds);
		}

		/// <summary>
		/// Calculates the amount of time until another date-time in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code OffsetDateTime}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified date-time.
		/// The result will be negative if the end is before the start.
		/// For example, the amount in days between two date-times can be calculated
		/// using {@code startDateTime.until(endDateTime, DAYS)}.
		/// </para>
		/// <para>
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code OffsetDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		/// If the offset differs between the two date-times, the specified
		/// end date-time is normalized to have the same offset as this date-time.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two date-times.
		/// For example, the amount in months between 2012-06-15T00:00Z and 2012-08-14T23:59Z
		/// will only be one month as it is one minute short of two months.
		/// </para>
		/// <para>
		/// There are two equivalent ways of using this method.
		/// The first is to invoke this method.
		/// The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		/// <pre>
		///   // these two lines are equivalent
		///   amount = start.until(end, MONTHS);
		///   amount = MONTHS.between(start, end);
		/// </pre>
		/// The choice should be made based on which makes the code more readable.
		/// </para>
		/// <para>
		/// The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		/// The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		/// {@code MINUTES}, {@code HOURS} and {@code HALF_DAYS}, {@code DAYS},
		/// {@code WEEKS}, {@code MONTHS}, {@code YEARS}, {@code DECADES},
		/// {@code CENTURIES}, {@code MILLENNIA} and {@code ERAS} are supported.
		/// Other {@code ChronoUnit} values will throw an exception.
		/// </para>
		/// <para>
		/// If the unit is not a {@code ChronoUnit}, then the result of this method
		/// is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		/// passing {@code this} as the first argument and the converted input temporal
		/// as the second argument.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="endExclusive">  the end date, exclusive, which is converted to an {@code OffsetDateTime}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this date-time and the end date-time </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to an {@code OffsetDateTime} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			OffsetDateTime end = OffsetDateTime.From(endExclusive);
			if (unit is ChronoUnit)
			{
				end = end.WithOffsetSameInstant(Offset_Renamed);
				return DateTime.Until(end.DateTime, unit);
			}
			return unit.Between(this, end);
		}

		/// <summary>
		/// Formats this date-time using the specified formatter.
		/// <para>
		/// This date-time will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted date-time string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public String Format(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this date-time with a time-zone to create a {@code ZonedDateTime}
		/// ensuring that the result has the same instant.
		/// <para>
		/// This returns a {@code ZonedDateTime} formed from this date-time and the specified time-zone.
		/// This conversion will ignore the visible local date-time and use the underlying instant instead.
		/// This avoids any problems with local time-line gaps or overlaps.
		/// The result might have different values for fields such as hour, minute an even day.
		/// </para>
		/// <para>
		/// To attempt to retain the values of the fields, use <seealso cref="#atZoneSimilarLocal(ZoneId)"/>.
		/// To use the offset as the zone ID, use <seealso cref="#toZonedDateTime()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use, not null </param>
		/// <returns> the zoned date-time formed from this date-time, not null </returns>
		public ZonedDateTime AtZoneSameInstant(ZoneId zone)
		{
			return ZonedDateTime.OfInstant(DateTime, Offset_Renamed, zone);
		}

		/// <summary>
		/// Combines this date-time with a time-zone to create a {@code ZonedDateTime}
		/// trying to keep the same local date and time.
		/// <para>
		/// This returns a {@code ZonedDateTime} formed from this date-time and the specified time-zone.
		/// Where possible, the result will have the same local date-time as this object.
		/// </para>
		/// <para>
		/// Time-zone rules, such as daylight savings, mean that not every time on the
		/// local time-line exists. If the local date-time is in a gap or overlap according to
		/// the rules then a resolver is used to determine the resultant local time and offset.
		/// This method uses <seealso cref="ZonedDateTime#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		/// to retain the offset from this instance if possible.
		/// </para>
		/// <para>
		/// Finer control over gaps and overlaps is available in two ways.
		/// If you simply want to use the later offset at overlaps then call
		/// <seealso cref="ZonedDateTime#withLaterOffsetAtOverlap()"/> immediately after this method.
		/// </para>
		/// <para>
		/// To create a zoned date-time at the same instant irrespective of the local time-line,
		/// use <seealso cref="#atZoneSameInstant(ZoneId)"/>.
		/// To use the offset as the zone ID, use <seealso cref="#toZonedDateTime()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use, not null </param>
		/// <returns> the zoned date-time formed from this date and the earliest valid time for the zone, not null </returns>
		public ZonedDateTime AtZoneSimilarLocal(ZoneId zone)
		{
			return ZonedDateTime.OfLocal(DateTime, zone, Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Converts this date-time to an {@code OffsetTime}.
		/// <para>
		/// This returns an offset time with the same local time and offset.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an OffsetTime representing the time and offset, not null </returns>
		public OffsetTime ToOffsetTime()
		{
			return OffsetTime.Of(DateTime.ToLocalTime(), Offset_Renamed);
		}

		/// <summary>
		/// Converts this date-time to a {@code ZonedDateTime} using the offset as the zone ID.
		/// <para>
		/// This creates the simplest possible {@code ZonedDateTime} using the offset
		/// as the zone ID.
		/// </para>
		/// <para>
		/// To control the time-zone used, see <seealso cref="#atZoneSameInstant(ZoneId)"/> and
		/// <seealso cref="#atZoneSimilarLocal(ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a zoned date-time representing the same local date-time and offset, not null </returns>
		public ZonedDateTime ToZonedDateTime()
		{
			return ZonedDateTime.Of(DateTime, Offset_Renamed);
		}

		/// <summary>
		/// Converts this date-time to an {@code Instant}.
		/// <para>
		/// This returns an {@code Instant} representing the same point on the
		/// time-line as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code Instant} representing the same instant, not null </returns>
		public Instant ToInstant()
		{
			return DateTime.toInstant(Offset_Renamed);
		}

		/// <summary>
		/// Converts this date-time to the number of seconds from the epoch of 1970-01-01T00:00:00Z.
		/// <para>
		/// This allows this date-time to be converted to a value of the
		/// <seealso cref="ChronoField#INSTANT_SECONDS epoch-seconds"/> field. This is primarily
		/// intended for low-level conversions rather than general application usage.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		public long ToEpochSecond()
		{
			return DateTime.toEpochSecond(Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this date-time to another date-time.
		/// <para>
		/// The comparison is based on the instant then on the local date-time.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// For example, the following is the comparator order:
		/// <ol>
		/// <li>{@code 2008-12-03T10:30+01:00}</li>
		/// <li>{@code 2008-12-03T11:00+01:00}</li>
		/// <li>{@code 2008-12-03T12:00+02:00}</li>
		/// <li>{@code 2008-12-03T11:30+01:00}</li>
		/// <li>{@code 2008-12-03T12:00+01:00}</li>
		/// <li>{@code 2008-12-03T12:30+01:00}</li>
		/// </ol>
		/// Values #2 and #3 represent the same instant on the time-line.
		/// When two values represent the same instant, the local date-time is compared
		/// to distinguish them. This step is needed to make the ordering
		/// consistent with {@code equals()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(OffsetDateTime other)
		{
			int cmp = CompareInstant(this, other);
			if (cmp == 0)
			{
				cmp = ToLocalDateTime().CompareTo(other.ToLocalDateTime());
			}
			return cmp;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the instant of this date-time is after that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/> in that it
		/// only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().isAfter(dateTime2.toInstant());}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this is after the instant of the specified date-time </returns>
		public bool IsAfter(OffsetDateTime other)
		{
			long thisEpochSec = ToEpochSecond();
			long otherEpochSec = other.ToEpochSecond();
			return thisEpochSec > otherEpochSec || (thisEpochSec == otherEpochSec && ToLocalTime().Nano > other.ToLocalTime().Nano);
		}

		/// <summary>
		/// Checks if the instant of this date-time is before that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().isBefore(dateTime2.toInstant());}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this is before the instant of the specified date-time </returns>
		public bool IsBefore(OffsetDateTime other)
		{
			long thisEpochSec = ToEpochSecond();
			long otherEpochSec = other.ToEpochSecond();
			return thisEpochSec < otherEpochSec || (thisEpochSec == otherEpochSec && ToLocalTime().Nano < other.ToLocalTime().Nano);
		}

		/// <summary>
		/// Checks if the instant of this date-time is equal to that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		/// in that it only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().equals(dateTime2.toInstant());}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if the instant equals the instant of the specified date-time </returns>
		public bool IsEqual(OffsetDateTime other)
		{
			return ToEpochSecond() == other.ToEpochSecond() && ToLocalTime().Nano == other.ToLocalTime().Nano;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this date-time is equal to another date-time.
		/// <para>
		/// The comparison is based on the local date-time and the offset.
		/// To compare for the same instant on the time-line, use <seealso cref="#isEqual"/>.
		/// Only objects of type {@code OffsetDateTime} are compared, other types return false.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date-time </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is OffsetDateTime)
			{
				OffsetDateTime other = (OffsetDateTime) obj;
				return DateTime.Equals(other.DateTime) && Offset_Renamed.Equals(other.Offset_Renamed);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date-time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return DateTime.HashCode() ^ Offset_Renamed.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this date-time as a {@code String}, such as {@code 2007-12-03T10:15:30+01:00}.
		/// <para>
		/// The output will be one of the following ISO-8601 formats:
		/// <ul>
		/// <li>{@code uuuu-MM-dd'T'HH:mmXXXXX}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ssXXXXX}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSXXXXX}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSXXXXX}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSSSSXXXXX}</li>
		/// </ul>
		/// The format used will be the shortest that outputs the full value of
		/// the time where the omitted parts are implied to be zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date-time, not null </returns>
		public override String ToString()
		{
			return DateTime.ToString() + Offset_Renamed.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(10);  // identifies an OffsetDateTime
		///  // the <a href="../../serialized-form.html#java.time.LocalDateTime">datetime</a> excluding the one byte header
		///  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.OFFSET_DATE_TIME_TYPE, this);
		}

		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.InvalidObjectException
		private void ReadObject(ObjectInputStream s)
		{
			throw new InvalidObjectException("Deserialization via serialization delegate");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		internal void WriteExternal(ObjectOutput @out)
		{
			DateTime.WriteExternal(@out);
			Offset_Renamed.WriteExternal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static OffsetDateTime readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		internal static OffsetDateTime ReadExternal(ObjectInput @in)
		{
			LocalDateTime dateTime = LocalDateTime.ReadExternal(@in);
			ZoneOffset offset = ZoneOffset.ReadExternal(@in);
			return OffsetDateTime.Of(dateTime, offset);
		}

	}

}