using System;

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
	/// A date-time without a time-zone in the ISO-8601 calendar system,
	/// such as {@code 2007-12-03T10:15:30}.
	/// <para>
	/// {@code LocalDateTime} is an immutable date-time object that represents a date-time,
	/// often viewed as year-month-day-hour-minute-second. Other date and time fields,
	/// such as day-of-year, day-of-week and week-of-year, can also be accessed.
	/// Time is represented to nanosecond precision.
	/// For example, the value "2nd October 2007 at 13:45.30.123456789" can be
	/// stored in a {@code LocalDateTime}.
	/// </para>
	/// <para>
	/// This class does not store or represent a time-zone.
	/// Instead, it is a description of the date, as used for birthdays, combined with
	/// the local time as seen on a wall clock.
	/// It cannot represent an instant on the time-line without additional information
	/// such as an offset or time-zone.
	/// </para>
	/// <para>
	/// The ISO-8601 calendar system is the modern civil calendar system used today
	/// in most of the world. It is equivalent to the proleptic Gregorian calendar
	/// system, in which today's rules for leap years are applied for all time.
	/// For most applications written today, the ISO-8601 rules are entirely suitable.
	/// However, any application that makes use of historical dates, and requires them
	/// to be accurate will find the ISO-8601 approach unsuitable.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code LocalDateTime} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class LocalDateTime : Temporal, TemporalAdjuster, ChronoLocalDateTime<LocalDate>
	{

		/// <summary>
		/// The minimum supported {@code LocalDateTime}, '-999999999-01-01T00:00:00'.
		/// This is the local date-time of midnight at the start of the minimum date.
		/// This combines <seealso cref="LocalDate#MIN"/> and <seealso cref="LocalTime#MIN"/>.
		/// This could be used by an application as a "far past" date-time.
		/// </summary>
		public static readonly LocalDateTime MIN = LocalDateTime.Of(LocalDate.MIN, LocalTime.MIN);
		/// <summary>
		/// The maximum supported {@code LocalDateTime}, '+999999999-12-31T23:59:59.999999999'.
		/// This is the local date-time just before midnight at the end of the maximum date.
		/// This combines <seealso cref="LocalDate#MAX"/> and <seealso cref="LocalTime#MAX"/>.
		/// This could be used by an application as a "far future" date-time.
		/// </summary>
		public static readonly LocalDateTime MAX = LocalDateTime.Of(LocalDate.MAX, LocalTime.MAX);

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 6207766400415563566L;

		/// <summary>
		/// The date part.
		/// </summary>
		private readonly LocalDate Date;
		/// <summary>
		/// The time part.
		/// </summary>
		private readonly LocalTime Time;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current date-time from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current date-time.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current date-time using the system clock and default time-zone, not null </returns>
		public static LocalDateTime Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current date-time from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date-time.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current date-time using the system clock, not null </returns>
		public static LocalDateTime Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current date-time from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date-time.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date-time, not null </returns>
		public static LocalDateTime Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			ZoneOffset offset = clock.Zone.Rules.GetOffset(now);
			return OfEpochSecond(now.EpochSecond, now.Nano, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour and minute, setting the second and nanosecond to zero.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour and minute.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// The second and nanosecond fields will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, Month month, int dayOfMonth, int hour, int minute)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute);
			return new LocalDateTime(date, time);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour, minute and second, setting the nanosecond to zero.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour, minute and second.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// The nanosecond field will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, Month month, int dayOfMonth, int hour, int minute, int second)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute, second);
			return new LocalDateTime(date, time);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour, minute, second and nanosecond.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour, minute, second and nanosecond.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, Month month, int dayOfMonth, int hour, int minute, int second, int nanoOfSecond)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute, second, nanoOfSecond);
			return new LocalDateTime(date, time);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour and minute, setting the second and nanosecond to zero.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour and minute.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// The second and nanosecond fields will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, int month, int dayOfMonth, int hour, int minute)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute);
			return new LocalDateTime(date, time);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour, minute and second, setting the nanosecond to zero.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour, minute and second.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// The nanosecond field will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, int month, int dayOfMonth, int hour, int minute, int second)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute, second);
			return new LocalDateTime(date, time);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from year, month,
		/// day, hour, minute, second and nanosecond.
		/// <para>
		/// This returns a {@code LocalDateTime} with the specified year, month,
		/// day-of-month, hour, minute, second and nanosecond.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
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
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDateTime Of(int year, int month, int dayOfMonth, int hour, int minute, int second, int nanoOfSecond)
		{
			LocalDate date = LocalDate.Of(year, month, dayOfMonth);
			LocalTime time = LocalTime.Of(hour, minute, second, nanoOfSecond);
			return new LocalDateTime(date, time);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from a date and time.
		/// </summary>
		/// <param name="date">  the local date, not null </param>
		/// <param name="time">  the local time, not null </param>
		/// <returns> the local date-time, not null </returns>
		public static LocalDateTime Of(LocalDate date, LocalTime time)
		{
			Objects.RequireNonNull(date, "date");
			Objects.RequireNonNull(time, "time");
			return new LocalDateTime(date, time);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from an {@code Instant} and zone ID.
		/// <para>
		/// This creates a local date-time based on the specified instant.
		/// First, the offset from UTC/Greenwich is obtained using the zone ID and instant,
		/// which is simple as there is only one valid offset for each instant.
		/// Then, the instant and offset are used to calculate the local date-time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to create the date-time from, not null </param>
		/// <param name="zone">  the time-zone, which may be an offset, not null </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public static LocalDateTime OfInstant(Instant instant, ZoneId zone)
		{
			Objects.RequireNonNull(instant, "instant");
			Objects.RequireNonNull(zone, "zone");
			ZoneRules rules = zone.Rules;
			ZoneOffset offset = rules.GetOffset(instant);
			return OfEpochSecond(instant.EpochSecond, instant.Nano, offset);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} using seconds from the
		/// epoch of 1970-01-01T00:00:00Z.
		/// <para>
		/// This allows the <seealso cref="ChronoField#INSTANT_SECONDS epoch-second"/> field
		/// to be converted to a local date-time. This is primarily intended for
		/// low-level conversions rather than general application usage.
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochSecond">  the number of seconds from the epoch of 1970-01-01T00:00:00Z </param>
		/// <param name="nanoOfSecond">  the nanosecond within the second, from 0 to 999,999,999 </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range,
		///  or if the nano-of-second is invalid </exception>
		public static LocalDateTime OfEpochSecond(long epochSecond, int nanoOfSecond, ZoneOffset offset)
		{
			Objects.RequireNonNull(offset, "offset");
			NANO_OF_SECOND.checkValidValue(nanoOfSecond);
			long localSecond = epochSecond + offset.TotalSeconds; // overflow caught later
			long localEpochDay = Math.FloorDiv(localSecond, SECONDS_PER_DAY);
			int secsOfDay = (int)Math.FloorMod(localSecond, SECONDS_PER_DAY);
			LocalDate date = LocalDate.OfEpochDay(localEpochDay);
			LocalTime time = LocalTime.OfNanoOfDay(secsOfDay * NANOS_PER_SECOND + nanoOfSecond);
			return new LocalDateTime(date, time);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from a temporal object.
		/// <para>
		/// This obtains a local date-time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code LocalDateTime}.
		/// </para>
		/// <para>
		/// The conversion extracts and combines the {@code LocalDate} and the
		/// {@code LocalTime} from the temporal object.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code LocalDateTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code LocalDateTime} </exception>
		public static LocalDateTime From(TemporalAccessor temporal)
		{
			if (temporal is LocalDateTime)
			{
				return (LocalDateTime) temporal;
			}
			else if (temporal is ZonedDateTime)
			{
				return ((ZonedDateTime) temporal).ToLocalDateTime();
			}
			else if (temporal is OffsetDateTime)
			{
				return ((OffsetDateTime) temporal).ToLocalDateTime();
			}
			try
			{
				LocalDate date = LocalDate.From(temporal);
				LocalTime time = LocalTime.From(temporal);
				return new LocalDateTime(date, time);
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain LocalDateTime from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from a text string such as {@code 2007-12-03T10:15:30}.
		/// <para>
		/// The string must represent a valid date-time and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_DATE_TIME"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007-12-03T10:15:30", not null </param>
		/// <returns> the parsed local date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalDateTime Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_LOCAL_DATE_TIME);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDateTime} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a date-time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed local date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalDateTime Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, LocalDateTime::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="date">  the date part of the date-time, validated not null </param>
		/// <param name="time">  the time part of the date-time, validated not null </param>
		private LocalDateTime(LocalDate date, LocalTime time)
		{
			this.Date = date;
			this.Time = time;
		}

		/// <summary>
		/// Returns a copy of this date-time with the new date and time, checking
		/// to see if a new object is in fact required.
		/// </summary>
		/// <param name="newDate">  the date of the new date-time, not null </param>
		/// <param name="newTime">  the time of the new date-time, not null </param>
		/// <returns> the date-time, not null </returns>
		private LocalDateTime With(LocalDate newDate, LocalTime newTime)
		{
			if (Date == newDate && Time == newTime)
			{
				return this;
			}
			return new LocalDateTime(newDate, newTime);
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
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				return f.DateBased || f.TimeBased;
			}
			return field != chrono.ChronoLocalDateTime_Fields.Null && field.IsSupportedBy(this);
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
			return ChronoLocalDateTime.this.isSupported(unit);
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
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.Range(field) : Date.Range(field));
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
		/// {@code EPOCH_DAY} and {@code PROLEPTIC_MONTH} which are too large to fit in
		/// an {@code int} and throw a {@code DateTimeException}.
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
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.Get(field) : Date.Get(field));
			}
			return ChronoLocalDateTime.this.get(field);
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
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.GetLong(field) : Date.GetLong(field));
			}
			return field.GetFrom(this);
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
		public override LocalDate ToLocalDate()
		{
			return Date;
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
				return Date.Year;
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
				return Date.MonthValue;
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
				return Date.Month;
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
				return Date.DayOfMonth;
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
				return Date.DayOfYear;
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
				return Date.DayOfWeek;
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
		public override LocalTime ToLocalTime()
		{
			return Time;
		}

		/// <summary>
		/// Gets the hour-of-day field.
		/// </summary>
		/// <returns> the hour-of-day, from 0 to 23 </returns>
		public int Hour
		{
			get
			{
				return Time.Hour;
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
				return Time.Minute;
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
				return Time.Second;
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
				return Time.Nano;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this date-time.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the date-time adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
		/// </para>
		/// <para>
		/// A simple adjuster might simply set the one of the fields, such as the year field.
		/// A more complex adjuster might set the date to the last day of the month.
		/// </para>
		/// <para>
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
		///  result = localDateTime.with(JULY).with(lastDayOfMonth());
		/// </pre>
		/// </para>
		/// <para>
		/// The classes <seealso cref="LocalDate"/> and <seealso cref="LocalTime"/> implement {@code TemporalAdjuster},
		/// thus this method can be used to change the date, time or offset:
		/// <pre>
		///  result = localDateTime.with(date);
		///  result = localDateTime.with(time);
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
		/// <returns> a {@code LocalDateTime} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDateTime With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalDate)
			{
				return With((LocalDate) adjuster, Time);
			}
			else if (adjuster is LocalTime)
			{
				return With(Date, (LocalTime) adjuster);
			}
			else if (adjuster is LocalDateTime)
			{
				return (LocalDateTime) adjuster;
			}
			return (LocalDateTime) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified field set to a new value.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the value
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
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		/// the matching method on <seealso cref="LocalDate#with(TemporalField, long) LocalDate"/>
		/// or <seealso cref="LocalTime#with(TemporalField, long) LocalTime"/>.
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
		/// <returns> a {@code LocalDateTime} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalDateTime With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				if (f.TimeBased)
				{
					return With(Date, Time.With(field, newValue));
				}
				else
				{
					return With(Date.With(field, newValue), Time);
				}
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the year altered.
		/// <para>
		/// The time does not affect the calculation and will be the same in the result.
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested year, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public LocalDateTime WithYear(int year)
		{
			return With(Date.WithYear(year), Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the month-of-year altered.
		/// <para>
		/// The time does not affect the calculation and will be the same in the result.
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public LocalDateTime WithMonth(int month)
		{
			return With(Date.WithMonth(month), Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the day-of-month altered.
		/// <para>
		/// If the resulting date-time is invalid, an exception is thrown.
		/// The time does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month value is invalid,
		///  or if the day-of-month is invalid for the month-year </exception>
		public LocalDateTime WithDayOfMonth(int dayOfMonth)
		{
			return With(Date.WithDayOfMonth(dayOfMonth), Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the day-of-year altered.
		/// <para>
		/// If the resulting date-time is invalid, an exception is thrown.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		/// <returns> a {@code LocalDateTime} based on this date with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-year value is invalid,
		///  or if the day-of-year is invalid for the year </exception>
		public LocalDateTime WithDayOfYear(int dayOfYear)
		{
			return With(Date.WithDayOfYear(dayOfYear), Time);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the hour-of-day altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested hour, not null </returns>
		/// <exception cref="DateTimeException"> if the hour value is invalid </exception>
		public LocalDateTime WithHour(int hour)
		{
			LocalTime newTime = Time.WithHour(hour);
			return With(Date, newTime);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the minute-of-hour altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested minute, not null </returns>
		/// <exception cref="DateTimeException"> if the minute value is invalid </exception>
		public LocalDateTime WithMinute(int minute)
		{
			LocalTime newTime = Time.WithMinute(minute);
			return With(Date, newTime);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the second-of-minute altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested second, not null </returns>
		/// <exception cref="DateTimeException"> if the second value is invalid </exception>
		public LocalDateTime WithSecond(int second)
		{
			LocalTime newTime = Time.WithSecond(second);
			return With(Date, newTime);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the nano-of-second altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the requested nanosecond, not null </returns>
		/// <exception cref="DateTimeException"> if the nano value is invalid </exception>
		public LocalDateTime WithNano(int nanoOfSecond)
		{
			LocalTime newTime = Time.WithNano(nanoOfSecond);
			return With(Date, newTime);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the time truncated.
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
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to truncate to, not null </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if unable to truncate </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public LocalDateTime TruncatedTo(TemporalUnit unit)
		{
			return With(Date, Time.TruncatedTo(unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the specified amount added.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDateTime Plus(TemporalAmount amountToAdd)
		{
			if (amountToAdd is Period)
			{
				Period periodToAdd = (Period) amountToAdd;
				return With(Date.Plus(periodToAdd), Time);
			}
			Objects.RequireNonNull(amountToAdd, "amountToAdd");
			return (LocalDateTime) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// Date units are added as per <seealso cref="LocalDate#plus(long, TemporalUnit)"/>.
		/// Time units are added as per <seealso cref="LocalTime#plus(long, TemporalUnit)"/> with
		/// any overflow in days added equivalent to using <seealso cref="#plusDays(long)"/>.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalDateTime Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				ChronoUnit f = (ChronoUnit) unit;
				switch (f)
				{
					case NANOS:
						return PlusNanos(amountToAdd);
					case MICROS:
						return PlusDays(amountToAdd / MICROS_PER_DAY).PlusNanos((amountToAdd % MICROS_PER_DAY) * 1000);
					case MILLIS:
						return PlusDays(amountToAdd / MILLIS_PER_DAY).PlusNanos((amountToAdd % MILLIS_PER_DAY) * 1000000);
					case SECONDS:
						return PlusSeconds(amountToAdd);
					case MINUTES:
						return PlusMinutes(amountToAdd);
					case HOURS:
						return PlusHours(amountToAdd);
					case HALF_DAYS: // no overflow (256 is multiple of 2)
						return PlusDays(amountToAdd / 256).PlusHours((amountToAdd % 256) * 12);
				}
				return With(Date.Plus(amountToAdd, unit), Time);
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of years added.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusYears(long years)
		{
			LocalDate newDate = Date.PlusYears(years);
			return With(newDate, Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of months added.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusMonths(long months)
		{
			LocalDate newDate = Date.PlusMonths(months);
			return With(newDate, Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of weeks added.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the weeks added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusWeeks(long weeks)
		{
			LocalDate newDate = Date.PlusWeeks(weeks);
			return With(newDate, Time);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of days added.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the days added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusDays(long days)
		{
			LocalDate newDate = Date.PlusDays(days);
			return With(newDate, Time);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of hours added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to add, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the hours added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusHours(long hours)
		{
			return PlusWithOverflow(Date, hours, 0, 0, 0, 1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of minutes added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to add, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the minutes added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusMinutes(long minutes)
		{
			return PlusWithOverflow(Date, 0, minutes, 0, 0, 1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of seconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to add, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the seconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusSeconds(long seconds)
		{
			return PlusWithOverflow(Date, 0, 0, seconds, 0, 1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of nanoseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to add, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the nanoseconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime PlusNanos(long nanos)
		{
			return PlusWithOverflow(Date, 0, 0, 0, nanos, 1);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the specified amount subtracted.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDateTime Minus(TemporalAmount amountToSubtract)
		{
			if (amountToSubtract is Period)
			{
				Period periodToSubtract = (Period) amountToSubtract;
				return With(Date.Minus(periodToSubtract), Time);
			}
			Objects.RequireNonNull(amountToSubtract, "amountToSubtract");
			return (LocalDateTime) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalDateTime}, based on this one, with the amount
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDateTime Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of years subtracted.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusYears(long years)
		{
			return (years == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-years));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of months subtracted.
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
		/// <returns> a {@code LocalDateTime} based on this date-time with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusMonths(long months)
		{
			return (months == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-months));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of weeks subtracted.
		/// <para>
		/// This method subtracts the specified amount in weeks from the days field decrementing
		/// the month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2009-01-07 minus one week would result in 2008-12-31.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the weeks to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the weeks subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusWeeks(long weeks)
		{
			return (weeks == Long.MinValue ? PlusWeeks(Long.MaxValue).PlusWeeks(1) : PlusWeeks(-weeks));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of days subtracted.
		/// <para>
		/// This method subtracts the specified amount from the days field decrementing the
		/// month and year fields as necessary to ensure the result remains valid.
		/// The result is only invalid if the maximum/minimum year is exceeded.
		/// </para>
		/// <para>
		/// For example, 2009-01-01 minus one day would result in 2008-12-31.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the days subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusDays(long days)
		{
			return (days == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-days));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of hours subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the hours subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusHours(long hours)
		{
			return PlusWithOverflow(Date, hours, 0, 0, 0, -1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of minutes subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the minutes subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusMinutes(long minutes)
		{
			return PlusWithOverflow(Date, 0, minutes, 0, 0, -1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of seconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the seconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusSeconds(long seconds)
		{
			return PlusWithOverflow(Date, 0, 0, seconds, 0, -1);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified number of nanoseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to subtract, may be negative </param>
		/// <returns> a {@code LocalDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDateTime MinusNanos(long nanos)
		{
			return PlusWithOverflow(Date, 0, 0, 0, nanos, -1);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDateTime} with the specified period added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newDate">  the new date to base the calculation on, not null </param>
		/// <param name="hours">  the hours to add, may be negative </param>
		/// <param name="minutes"> the minutes to add, may be negative </param>
		/// <param name="seconds"> the seconds to add, may be negative </param>
		/// <param name="nanos"> the nanos to add, may be negative </param>
		/// <param name="sign">  the sign to determine add or subtract </param>
		/// <returns> the combined result, not null </returns>
		private LocalDateTime PlusWithOverflow(LocalDate newDate, long hours, long minutes, long seconds, long nanos, int sign)
		{
			// 9223372036854775808 long, 2147483648 int
			if ((hours | minutes | seconds | nanos) == 0)
			{
				return With(newDate, Time);
			}
			long totDays = nanos / NANOS_PER_DAY + seconds / SECONDS_PER_DAY + minutes / MINUTES_PER_DAY + hours / HOURS_PER_DAY; //   max/24 -    max/24*60 -    max/24*60*60 -    max/24*60*60*1B
			totDays *= sign; // total max*0.4237...
			long totNanos = nanos % NANOS_PER_DAY + (seconds % SECONDS_PER_DAY) * NANOS_PER_SECOND + (minutes % MINUTES_PER_DAY) * NANOS_PER_MINUTE + (hours % HOURS_PER_DAY) * NANOS_PER_HOUR; //   max  86400000000000 -    max  86400000000000 -    max  86400000000000 -    max  86400000000000
			long curNoD = Time.ToNanoOfDay(); //   max  86400000000000
			totNanos = totNanos * sign + curNoD; // total 432000000000000
			totDays += Math.FloorDiv(totNanos, NANOS_PER_DAY);
			long newNoD = Math.FloorMod(totNanos, NANOS_PER_DAY);
			LocalTime newTime = (newNoD == curNoD ? Time : LocalTime.OfNanoOfDay(newNoD));
			return With(newDate.PlusDays(totDays), newTime);
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
		public override R query<R>(TemporalQuery<R> query) // override for Javadoc
		{
			if (query == TemporalQueries.LocalDate())
			{
				return (R) Date;
			}
			return ChronoLocalDateTime.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have the same date and time as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the date and time changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// twice, passing <seealso cref="ChronoField#EPOCH_DAY"/> and
		/// <seealso cref="ChronoField#NANO_OF_DAY"/> as the fields.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisLocalDateTime.adjustInto(temporal);
		///   temporal = temporal.with(thisLocalDateTime);
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
		public Temporal AdjustInto(Temporal temporal) // override for Javadoc
		{
			return ChronoLocalDateTime.this.adjustInto(temporal);
		}

		/// <summary>
		/// Calculates the amount of time until another date-time in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code LocalDateTime}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified date-time.
		/// The result will be negative if the end is before the start.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code LocalDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in days between two date-times can be calculated
		/// using {@code startDateTime.until(endDateTime, DAYS)}.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two date-times.
		/// For example, the amount in months between 2012-06-15T00:00 and 2012-08-14T23:59
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to a {@code LocalDateTime}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this date-time and the end date-time </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code LocalDateTime} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			LocalDateTime end = LocalDateTime.From(endExclusive);
			if (unit is ChronoUnit)
			{
				if (unit.TimeBased)
				{
					long amount = Date.DaysUntil(end.Date);
					if (amount == 0)
					{
						return Time.Until(end.Time, unit);
					}
					long timePart = end.Time.ToNanoOfDay() - Time.ToNanoOfDay();
					if (amount > 0)
					{
						amount--; // safe
						timePart += NANOS_PER_DAY; // safe
					}
					else
					{
						amount++; // safe
						timePart -= NANOS_PER_DAY; // safe
					}
					switch ((ChronoUnit) unit)
					{
						case NANOS:
							amount = Math.MultiplyExact(amount, NANOS_PER_DAY);
							break;
						case MICROS:
							amount = Math.MultiplyExact(amount, MICROS_PER_DAY);
							timePart = timePart / 1000;
							break;
						case MILLIS:
							amount = Math.MultiplyExact(amount, MILLIS_PER_DAY);
							timePart = timePart / 1000000;
							break;
						case SECONDS:
							amount = Math.MultiplyExact(amount, SECONDS_PER_DAY);
							timePart = timePart / NANOS_PER_SECOND;
							break;
						case MINUTES:
							amount = Math.MultiplyExact(amount, MINUTES_PER_DAY);
							timePart = timePart / NANOS_PER_MINUTE;
							break;
						case HOURS:
							amount = Math.MultiplyExact(amount, HOURS_PER_DAY);
							timePart = timePart / NANOS_PER_HOUR;
							break;
						case HALF_DAYS:
							amount = Math.MultiplyExact(amount, 2);
							timePart = timePart / (NANOS_PER_HOUR * 12);
							break;
					}
					return Math.AddExact(amount, timePart);
				}
				LocalDate endDate = end.Date;
				if (endDate.IsAfter(Date) && end.Time.IsBefore(Time))
				{
					endDate = endDate.MinusDays(1);
				}
				else if (endDate.IsBefore(Date) && end.Time.IsAfter(Time))
				{
					endDate = endDate.PlusDays(1);
				}
				return Date.Until(endDate, unit);
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
		public override String Format(DateTimeFormatter formatter) // override for Javadoc and performance
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this date-time with an offset to create an {@code OffsetDateTime}.
		/// <para>
		/// This returns an {@code OffsetDateTime} formed from this date-time at the specified offset.
		/// All possible combinations of date-time and offset are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to combine with, not null </param>
		/// <returns> the offset date-time formed from this date-time and the specified offset, not null </returns>
		public OffsetDateTime AtOffset(ZoneOffset offset)
		{
			return OffsetDateTime.Of(this, offset);
		}

		/// <summary>
		/// Combines this date-time with a time-zone to create a {@code ZonedDateTime}.
		/// <para>
		/// This returns a {@code ZonedDateTime} formed from this date-time at the
		/// specified time-zone. The result will match this date-time as closely as possible.
		/// Time-zone rules, such as daylight savings, mean that not every local date-time
		/// is valid for the specified zone, thus the local date-time may be adjusted.
		/// </para>
		/// <para>
		/// The local date-time is resolved to a single instant on the time-line.
		/// This is achieved by finding a valid offset from UTC/Greenwich for the local
		/// date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		/// </para>
		/// <para>
		/// In most cases, there is only one valid offset for a local date-time.
		/// In the case of an overlap, where clocks are set back, there are two valid offsets.
		/// This method uses the earlier offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, where clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// To obtain the later offset during an overlap, call
		/// <seealso cref="ZonedDateTime#withLaterOffsetAtOverlap()"/> on the result of this method.
		/// To throw an exception when there is a gap or overlap, use
		/// <seealso cref="ZonedDateTime#ofStrict(LocalDateTime, ZoneOffset, ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use, not null </param>
		/// <returns> the zoned date-time formed from this date-time, not null </returns>
		public override ZonedDateTime AtZone(ZoneId zone)
		{
			return ZonedDateTime.Of(this, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this date-time to another date-time.
		/// <para>
		/// The comparison is primarily based on the date-time, from earliest to latest.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// If all the date-times being compared are instances of {@code LocalDateTime},
		/// then the comparison will be entirely based on the date-time.
		/// If some dates being compared are in different chronologies, then the
		/// chronology is also considered, see <seealso cref="ChronoLocalDateTime#compareTo"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public override int compareTo<T1>(ChronoLocalDateTime<T1> other) // override for Javadoc and performance
		{
			if (other is LocalDateTime)
			{
				return CompareTo0((LocalDateTime) other);
			}
			return ChronoLocalDateTime.this.CompareTo(other);
		}

		private int CompareTo0(LocalDateTime other)
		{
			int chrono.ChronoLocalDateTime_Fields.Cmp = Date.CompareTo0(other.ToLocalDate());
			if (chrono.ChronoLocalDateTime_Fields.Cmp == 0)
			{
				chrono.ChronoLocalDateTime_Fields.Cmp = Time.CompareTo(other.ToLocalTime());
			}
			return chrono.ChronoLocalDateTime_Fields.Cmp;
		}

		/// <summary>
		/// Checks if this date-time is after the specified date-time.
		/// <para>
		/// This checks to see if this date-time represents a point on the
		/// local time-line after the other date-time.
		/// <pre>
		///   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		///   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		///   a.isAfter(b) == false
		///   a.isAfter(a) == false
		///   b.isAfter(a) == true
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two date-times on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		/// but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this date-time is after the specified date-time </returns>
		public override bool isAfter<T1>(ChronoLocalDateTime<T1> other) // override for Javadoc and performance
		{
			if (other is LocalDateTime)
			{
				return CompareTo0((LocalDateTime) other) > 0;
			}
			return ChronoLocalDateTime.this.isAfter(other);
		}

		/// <summary>
		/// Checks if this date-time is before the specified date-time.
		/// <para>
		/// This checks to see if this date-time represents a point on the
		/// local time-line before the other date-time.
		/// <pre>
		///   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		///   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		///   a.isBefore(b) == true
		///   a.isBefore(a) == false
		///   b.isBefore(a) == false
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two date-times on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		/// but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this date-time is before the specified date-time </returns>
		public override bool isBefore<T1>(ChronoLocalDateTime<T1> other) // override for Javadoc and performance
		{
			if (other is LocalDateTime)
			{
				return CompareTo0((LocalDateTime) other) < 0;
			}
			return ChronoLocalDateTime.this.isBefore(other);
		}

		/// <summary>
		/// Checks if this date-time is equal to the specified date-time.
		/// <para>
		/// This checks to see if this date-time represents the same point on the
		/// local time-line as the other date-time.
		/// <pre>
		///   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		///   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		///   a.isEqual(b) == false
		///   a.isEqual(a) == true
		///   b.isEqual(a) == false
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two date-times on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		/// but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this date-time is equal to the specified date-time </returns>
		public override bool isEqual<T1>(ChronoLocalDateTime<T1> other) // override for Javadoc and performance
		{
			if (other is LocalDateTime)
			{
				return CompareTo0((LocalDateTime) other) == 0;
			}
			return ChronoLocalDateTime.this.isEqual(other);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this date-time is equal to another date-time.
		/// <para>
		/// Compares this {@code LocalDateTime} with another ensuring that the date-time is the same.
		/// Only objects of type {@code LocalDateTime} are compared, other types return false.
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
			if (obj is LocalDateTime)
			{
				LocalDateTime other = (LocalDateTime) obj;
				return Date.Equals(other.Date) && Time.Equals(other.Time);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date-time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Date.HashCode() ^ Time.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this date-time as a {@code String}, such as {@code 2007-12-03T10:15:30}.
		/// <para>
		/// The output will be one of the following ISO-8601 formats:
		/// <ul>
		/// <li>{@code uuuu-MM-dd'T'HH:mm}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSS}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSS}</li>
		/// <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSSSS}</li>
		/// </ul>
		/// The format used will be the shortest that outputs the full value of
		/// the time where the omitted parts are implied to be zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date-time, not null </returns>
		public override String ToString()
		{
			return Date.ToString() + 'T' + Time.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(5);  // identifies a LocalDateTime
		///  // the <a href="../../serialized-form.html#java.time.LocalDate">date</a> excluding the one byte header
		///  // the <a href="../../serialized-form.html#java.time.LocalTime">time</a> excluding the one byte header
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.LOCAL_DATE_TIME_TYPE, this);
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
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			Date.WriteExternal(@out);
			Time.WriteExternal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static LocalDateTime readExternal(java.io.DataInput in) throws java.io.IOException
		internal static LocalDateTime ReadExternal(DataInput @in)
		{
			LocalDate date = LocalDate.ReadExternal(@in);
			LocalTime time = LocalTime.ReadExternal(@in);
			return LocalDateTime.Of(date, time);
		}

	}

}