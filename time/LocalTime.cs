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
	/// A time without a time-zone in the ISO-8601 calendar system,
	/// such as {@code 10:15:30}.
	/// <para>
	/// {@code LocalTime} is an immutable date-time object that represents a time,
	/// often viewed as hour-minute-second.
	/// Time is represented to nanosecond precision.
	/// For example, the value "13:45.30.123456789" can be stored in a {@code LocalTime}.
	/// </para>
	/// <para>
	/// This class does not store or represent a date or time-zone.
	/// Instead, it is a description of the local time as seen on a wall clock.
	/// It cannot represent an instant on the time-line without additional information
	/// such as an offset or time-zone.
	/// </para>
	/// <para>
	/// The ISO-8601 calendar system is the modern civil calendar system used today
	/// in most of the world. This API assumes that all calendar systems use the same
	/// representation, this class, for time-of-day.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code LocalTime} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class LocalTime : Temporal, TemporalAdjuster, Comparable<LocalTime>
	{

		/// <summary>
		/// The minimum supported {@code LocalTime}, '00:00'.
		/// This is the time of midnight at the start of the day.
		/// </summary>
		public static readonly LocalTime MIN;
		/// <summary>
		/// The maximum supported {@code LocalTime}, '23:59:59.999999999'.
		/// This is the time just before midnight at the end of the day.
		/// </summary>
		public static readonly LocalTime MAX;
		/// <summary>
		/// The time of midnight at the start of the day, '00:00'.
		/// </summary>
		public static readonly LocalTime MIDNIGHT;
		/// <summary>
		/// The time of noon in the middle of the day, '12:00'.
		/// </summary>
		public static readonly LocalTime NOON;
		/// <summary>
		/// Constants for the local time of each hour.
		/// </summary>
		private static readonly LocalTime[] HOURS = new LocalTime[24];
		static LocalTime()
		{
			for (int i = 0; i < HOURS.Length; i++)
			{
				HOURS[i] = new LocalTime(i, 0, 0, 0);
			}
			MIDNIGHT = HOURS[0];
			NOON = HOURS[12];
			MIN = HOURS[0];
			MAX = new LocalTime(23, 59, 59, 999999999);
		}

		/// <summary>
		/// Hours per day.
		/// </summary>
		internal const int HOURS_PER_DAY = 24;
		/// <summary>
		/// Minutes per hour.
		/// </summary>
		internal const int MINUTES_PER_HOUR = 60;
		/// <summary>
		/// Minutes per day.
		/// </summary>
		internal static readonly int MINUTES_PER_DAY = MINUTES_PER_HOUR * HOURS_PER_DAY;
		/// <summary>
		/// Seconds per minute.
		/// </summary>
		internal const int SECONDS_PER_MINUTE = 60;
		/// <summary>
		/// Seconds per hour.
		/// </summary>
		internal static readonly int SECONDS_PER_HOUR = SECONDS_PER_MINUTE * MINUTES_PER_HOUR;
		/// <summary>
		/// Seconds per day.
		/// </summary>
		internal static readonly int SECONDS_PER_DAY = SECONDS_PER_HOUR * HOURS_PER_DAY;
		/// <summary>
		/// Milliseconds per day.
		/// </summary>
		internal static readonly long MILLIS_PER_DAY = SECONDS_PER_DAY * 1000L;
		/// <summary>
		/// Microseconds per day.
		/// </summary>
		internal static readonly long MICROS_PER_DAY = SECONDS_PER_DAY * 1000_000L;
		/// <summary>
		/// Nanos per second.
		/// </summary>
		internal static readonly long NANOS_PER_SECOND = 1000_000_000L;
		/// <summary>
		/// Nanos per minute.
		/// </summary>
		internal static readonly long NANOS_PER_MINUTE = NANOS_PER_SECOND * SECONDS_PER_MINUTE;
		/// <summary>
		/// Nanos per hour.
		/// </summary>
		internal static readonly long NANOS_PER_HOUR = NANOS_PER_MINUTE * MINUTES_PER_HOUR;
		/// <summary>
		/// Nanos per day.
		/// </summary>
		internal static readonly long NANOS_PER_DAY = NANOS_PER_HOUR * HOURS_PER_DAY;

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 6414437269572265201L;

		/// <summary>
		/// The hour.
		/// </summary>
		private readonly sbyte Hour_Renamed;
		/// <summary>
		/// The minute.
		/// </summary>
		private readonly sbyte Minute_Renamed;
		/// <summary>
		/// The second.
		/// </summary>
		private readonly sbyte Second_Renamed;
		/// <summary>
		/// The nanosecond.
		/// </summary>
		private readonly int Nano_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current time from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current time.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current time using the system clock and default time-zone, not null </returns>
		public static LocalTime Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current time from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current time.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current time using the system clock, not null </returns>
		public static LocalTime Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current time from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current time.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current time, not null </returns>
		public static LocalTime Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
			// inline OffsetTime factory to avoid creating object and InstantProvider checks
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			ZoneOffset offset = clock.Zone.Rules.GetOffset(now);
			long localSecond = now.EpochSecond + offset.TotalSeconds; // overflow caught later
			int secsOfDay = (int) Math.FloorMod(localSecond, SECONDS_PER_DAY);
			return OfNanoOfDay(secsOfDay * NANOS_PER_SECOND + now.Nano);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalTime} from an hour and minute.
		/// <para>
		/// This returns a {@code LocalTime} with the specified hour and minute.
		/// The second and nanosecond fields will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public static LocalTime Of(int hour, int minute)
		{
			HOUR_OF_DAY.checkValidValue(hour);
			if (minute == 0)
			{
				return HOURS[hour]; // for performance
			}
			MINUTE_OF_HOUR.checkValidValue(minute);
			return new LocalTime(hour, minute, 0, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalTime} from an hour, minute and second.
		/// <para>
		/// This returns a {@code LocalTime} with the specified hour, minute and second.
		/// The nanosecond field will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public static LocalTime Of(int hour, int minute, int second)
		{
			HOUR_OF_DAY.checkValidValue(hour);
			if ((minute | second) == 0)
			{
				return HOURS[hour]; // for performance
			}
			MINUTE_OF_HOUR.checkValidValue(minute);
			SECOND_OF_MINUTE.checkValidValue(second);
			return new LocalTime(hour, minute, second, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalTime} from an hour, minute, second and nanosecond.
		/// <para>
		/// This returns a {@code LocalTime} with the specified hour, minute, second and nanosecond.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public static LocalTime Of(int hour, int minute, int second, int nanoOfSecond)
		{
			HOUR_OF_DAY.checkValidValue(hour);
			MINUTE_OF_HOUR.checkValidValue(minute);
			SECOND_OF_MINUTE.checkValidValue(second);
			NANO_OF_SECOND.checkValidValue(nanoOfSecond);
			return Create(hour, minute, second, nanoOfSecond);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalTime} from a second-of-day value.
		/// <para>
		/// This returns a {@code LocalTime} with the specified second-of-day.
		/// The nanosecond field will be set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondOfDay">  the second-of-day, from {@code 0} to {@code 24 * 60 * 60 - 1} </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if the second-of-day value is invalid </exception>
		public static LocalTime OfSecondOfDay(long secondOfDay)
		{
			SECOND_OF_DAY.checkValidValue(secondOfDay);
			int hours = (int)(secondOfDay / SECONDS_PER_HOUR);
			secondOfDay -= hours * SECONDS_PER_HOUR;
			int minutes = (int)(secondOfDay / SECONDS_PER_MINUTE);
			secondOfDay -= minutes * SECONDS_PER_MINUTE;
			return Create(hours, minutes, (int) secondOfDay, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalTime} from a nanos-of-day value.
		/// <para>
		/// This returns a {@code LocalTime} with the specified nanosecond-of-day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfDay">  the nano of day, from {@code 0} to {@code 24 * 60 * 60 * 1,000,000,000 - 1} </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if the nanos of day value is invalid </exception>
		public static LocalTime OfNanoOfDay(long nanoOfDay)
		{
			NANO_OF_DAY.checkValidValue(nanoOfDay);
			int hours = (int)(nanoOfDay / NANOS_PER_HOUR);
			nanoOfDay -= hours * NANOS_PER_HOUR;
			int minutes = (int)(nanoOfDay / NANOS_PER_MINUTE);
			nanoOfDay -= minutes * NANOS_PER_MINUTE;
			int seconds = (int)(nanoOfDay / NANOS_PER_SECOND);
			nanoOfDay -= seconds * NANOS_PER_SECOND;
			return Create(hours, minutes, seconds, (int) nanoOfDay);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalTime} from a temporal object.
		/// <para>
		/// This obtains a local time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code LocalTime}.
		/// </para>
		/// <para>
		/// The conversion uses the <seealso cref="TemporalQueries#localTime()"/> query, which relies
		/// on extracting the <seealso cref="ChronoField#NANO_OF_DAY NANO_OF_DAY"/> field.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code LocalTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the local time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code LocalTime} </exception>
		public static LocalTime From(TemporalAccessor temporal)
		{
			Objects.RequireNonNull(temporal, "temporal");
			LocalTime time = temporal.query(TemporalQueries.LocalTime());
			if (time == temporal.TemporalAccessor_Fields.Null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain LocalTime from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName);
			}
			return time;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalTime} from a text string such as {@code 10:15}.
		/// <para>
		/// The string must represent a valid time and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_TIME"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "10:15:30", not null </param>
		/// <returns> the parsed local time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalTime Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_LOCAL_TIME);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalTime} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed local time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalTime Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, LocalTime::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates a local time from the hour, minute, second and nanosecond fields.
		/// <para>
		/// This factory may return a cached value, but applications must not rely on this.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, validated from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, validated from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, validated from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, validated from 0 to 999,999,999 </param>
		/// <returns> the local time, not null </returns>
		private static LocalTime Create(int hour, int minute, int second, int nanoOfSecond)
		{
			if ((minute | second | nanoOfSecond) == 0)
			{
				return HOURS[hour];
			}
			return new LocalTime(hour, minute, second, nanoOfSecond);
		}

		/// <summary>
		/// Constructor, previously validated.
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, validated from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, validated from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, validated from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, validated from 0 to 999,999,999 </param>
		private LocalTime(int hour, int minute, int second, int nanoOfSecond)
		{
			this.Hour_Renamed = (sbyte) hour;
			this.Minute_Renamed = (sbyte) minute;
			this.Second_Renamed = (sbyte) second;
			this.Nano_Renamed = nanoOfSecond;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this time can be queried for the specified field.
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
		/// <returns> true if the field is supported on this time, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field.TimeBased;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this time.
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
				return unit.TimeBased;
			}
			return unit != temporal.TemporalAccessor_Fields.Null && unit.isSupportedBy(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This time is used to enhance the accuracy of the returned range.
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
		public override ValueRange temporal.TemporalAccessor_Fields.range(TemporalField field) // override for Javadoc
		{
			return Temporal.this.range(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this time as an {@code int}.
		/// <para>
		/// This queries this time for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this time, except {@code NANO_OF_DAY} and {@code MICRO_OF_DAY}
		/// which are too large to fit in an {@code int} and throw a {@code DateTimeException}.
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
		public override int Get(TemporalField field) // override for Javadoc and performance
		{
			if (field is ChronoField)
			{
				return Get0(field);
			}
			return Temporal.this.get(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this time as a {@code long}.
		/// <para>
		/// This queries this time for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this time.
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
				if (field == NANO_OF_DAY)
				{
					return ToNanoOfDay();
				}
				if (field == MICRO_OF_DAY)
				{
					return ToNanoOfDay() / 1000;
				}
				return Get0(field);
			}
			return field.GetFrom(this);
		}

		private int Get0(TemporalField field)
		{
			switch ((ChronoField) field)
			{
				case NANO_OF_SECOND:
					return Nano_Renamed;
				case NANO_OF_DAY:
					throw new UnsupportedTemporalTypeException("Invalid field 'NanoOfDay' for get() method, use getLong() instead");
				case MICRO_OF_SECOND:
					return Nano_Renamed / 1000;
				case MICRO_OF_DAY:
					throw new UnsupportedTemporalTypeException("Invalid field 'MicroOfDay' for get() method, use getLong() instead");
				case MILLI_OF_SECOND:
					return Nano_Renamed / 1000000;
				case MILLI_OF_DAY:
					return (int)(ToNanoOfDay() / 1000000);
				case SECOND_OF_MINUTE:
					return Second_Renamed;
				case SECOND_OF_DAY:
					return ToSecondOfDay();
				case MINUTE_OF_HOUR:
					return Minute_Renamed;
				case MINUTE_OF_DAY:
					return Hour_Renamed * 60 + Minute_Renamed;
				case HOUR_OF_AMPM:
					return Hour_Renamed % 12;
				case CLOCK_HOUR_OF_AMPM:
					int ham = Hour_Renamed % 12;
					return (ham % 12 == 0 ? 12 : ham);
				case HOUR_OF_DAY:
					return Hour_Renamed;
				case CLOCK_HOUR_OF_DAY:
					return (Hour_Renamed == 0 ? 24 : Hour_Renamed);
				case AMPM_OF_DAY:
					return Hour_Renamed / 12;
			}
			throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the hour-of-day field.
		/// </summary>
		/// <returns> the hour-of-day, from 0 to 23 </returns>
		public int Hour
		{
			get
			{
				return Hour_Renamed;
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
				return Minute_Renamed;
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
				return Second_Renamed;
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
				return Nano_Renamed;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this time.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the time adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
		/// </para>
		/// <para>
		/// A simple adjuster might simply set the one of the fields, such as the hour field.
		/// A more complex adjuster might set the time to the last hour of the day.
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
		/// <returns> a {@code LocalTime} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalTime With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalTime)
			{
				return (LocalTime) adjuster;
			}
			return (LocalTime) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified field set to a new value.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the value
		/// for the specified field changed.
		/// This can be used to change any supported field, such as the hour, minute or second.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code NANO_OF_SECOND} -
		///  Returns a {@code LocalTime} with the specified nano-of-second.
		///  The hour, minute and second will be unchanged.
		/// <li>{@code NANO_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified nano-of-day.
		///  This completely replaces the time and is equivalent to <seealso cref="#ofNanoOfDay(long)"/>.
		/// <li>{@code MICRO_OF_SECOND} -
		///  Returns a {@code LocalTime} with the nano-of-second replaced by the specified
		///  micro-of-second multiplied by 1,000.
		///  The hour, minute and second will be unchanged.
		/// <li>{@code MICRO_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified micro-of-day.
		///  This completely replaces the time and is equivalent to using <seealso cref="#ofNanoOfDay(long)"/>
		///  with the micro-of-day multiplied by 1,000.
		/// <li>{@code MILLI_OF_SECOND} -
		///  Returns a {@code LocalTime} with the nano-of-second replaced by the specified
		///  milli-of-second multiplied by 1,000,000.
		///  The hour, minute and second will be unchanged.
		/// <li>{@code MILLI_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified milli-of-day.
		///  This completely replaces the time and is equivalent to using <seealso cref="#ofNanoOfDay(long)"/>
		///  with the milli-of-day multiplied by 1,000,000.
		/// <li>{@code SECOND_OF_MINUTE} -
		///  Returns a {@code LocalTime} with the specified second-of-minute.
		///  The hour, minute and nano-of-second will be unchanged.
		/// <li>{@code SECOND_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified second-of-day.
		///  The nano-of-second will be unchanged.
		/// <li>{@code MINUTE_OF_HOUR} -
		///  Returns a {@code LocalTime} with the specified minute-of-hour.
		///  The hour, second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code MINUTE_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified minute-of-day.
		///  The second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code HOUR_OF_AMPM} -
		///  Returns a {@code LocalTime} with the specified hour-of-am-pm.
		///  The AM/PM, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code CLOCK_HOUR_OF_AMPM} -
		///  Returns a {@code LocalTime} with the specified clock-hour-of-am-pm.
		///  The AM/PM, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code HOUR_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified hour-of-day.
		///  The minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code CLOCK_HOUR_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified clock-hour-of-day.
		///  The minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		/// <li>{@code AMPM_OF_DAY} -
		///  Returns a {@code LocalTime} with the specified AM/PM.
		///  The hour-of-am-pm, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		/// </ul>
		/// </para>
		/// <para>
		/// In all cases, if the new value is outside the valid range of values for the field
		/// then a {@code DateTimeException} will be thrown.
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
		/// <returns> a {@code LocalTime} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalTime With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				f.checkValidValue(newValue);
				switch (f)
				{
					case NANO_OF_SECOND:
						return WithNano((int) newValue);
					case NANO_OF_DAY:
						return LocalTime.OfNanoOfDay(newValue);
					case MICRO_OF_SECOND:
						return WithNano((int) newValue * 1000);
					case MICRO_OF_DAY:
						return LocalTime.OfNanoOfDay(newValue * 1000);
					case MILLI_OF_SECOND:
						return WithNano((int) newValue * 1000000);
					case MILLI_OF_DAY:
						return LocalTime.OfNanoOfDay(newValue * 1000000);
					case SECOND_OF_MINUTE:
						return WithSecond((int) newValue);
					case SECOND_OF_DAY:
						return PlusSeconds(newValue - ToSecondOfDay());
					case MINUTE_OF_HOUR:
						return WithMinute((int) newValue);
					case MINUTE_OF_DAY:
						return PlusMinutes(newValue - (Hour_Renamed * 60 + Minute_Renamed));
					case HOUR_OF_AMPM:
						return PlusHours(newValue - (Hour_Renamed % 12));
					case CLOCK_HOUR_OF_AMPM:
						return PlusHours((newValue == 12 ? 0 : newValue) - (Hour_Renamed % 12));
					case HOUR_OF_DAY:
						return WithHour((int) newValue);
					case CLOCK_HOUR_OF_DAY:
						return WithHour((int)(newValue == 24 ? 0 : newValue));
					case AMPM_OF_DAY:
						return PlusHours((newValue - (Hour_Renamed / 12)) * 12);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the hour-of-day altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		/// <returns> a {@code LocalTime} based on this time with the requested hour, not null </returns>
		/// <exception cref="DateTimeException"> if the hour value is invalid </exception>
		public LocalTime WithHour(int hour)
		{
			if (this.Hour_Renamed == hour)
			{
				return this;
			}
			HOUR_OF_DAY.checkValidValue(hour);
			return Create(hour, Minute_Renamed, Second_Renamed, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the minute-of-hour altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		/// <returns> a {@code LocalTime} based on this time with the requested minute, not null </returns>
		/// <exception cref="DateTimeException"> if the minute value is invalid </exception>
		public LocalTime WithMinute(int minute)
		{
			if (this.Minute_Renamed == minute)
			{
				return this;
			}
			MINUTE_OF_HOUR.checkValidValue(minute);
			return Create(Hour_Renamed, minute, Second_Renamed, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the second-of-minute altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		/// <returns> a {@code LocalTime} based on this time with the requested second, not null </returns>
		/// <exception cref="DateTimeException"> if the second value is invalid </exception>
		public LocalTime WithSecond(int second)
		{
			if (this.Second_Renamed == second)
			{
				return this;
			}
			SECOND_OF_MINUTE.checkValidValue(second);
			return Create(Hour_Renamed, Minute_Renamed, second, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the nano-of-second altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		/// <returns> a {@code LocalTime} based on this time with the requested nanosecond, not null </returns>
		/// <exception cref="DateTimeException"> if the nanos value is invalid </exception>
		public LocalTime WithNano(int nanoOfSecond)
		{
			if (this.Nano_Renamed == nanoOfSecond)
			{
				return this;
			}
			NANO_OF_SECOND.checkValidValue(nanoOfSecond);
			return Create(Hour_Renamed, Minute_Renamed, Second_Renamed, nanoOfSecond);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the time truncated.
		/// <para>
		/// Truncation returns a copy of the original time with fields
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
		/// <returns> a {@code LocalTime} based on this time with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if unable to truncate </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public LocalTime TruncatedTo(TemporalUnit unit)
		{
			if (unit == ChronoUnit.NANOS)
			{
				return this;
			}
			Duration unitDur = unit.Duration;
			if (unitDur.Seconds > SECONDS_PER_DAY)
			{
				throw new UnsupportedTemporalTypeException("Unit is too large to be used for truncation");
			}
			long dur = unitDur.ToNanos();
			if ((NANOS_PER_DAY % dur) != 0)
			{
				throw new UnsupportedTemporalTypeException("Unit must divide into a standard day without remainder");
			}
			long nod = ToNanoOfDay();
			return OfNanoOfDay((nod / dur) * dur);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this time with the specified amount added.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the specified amount added.
		/// The amount is typically <seealso cref="Duration"/> but may be any other type implementing
		/// the <seealso cref="TemporalAmount"/> interface.
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
		/// <returns> a {@code LocalTime} based on this time with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalTime Plus(TemporalAmount amountToAdd)
		{
			return (LocalTime) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified amount added.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code NANOS} -
		///  Returns a {@code LocalTime} with the specified number of nanoseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/>.
		/// <li>{@code MICROS} -
		///  Returns a {@code LocalTime} with the specified number of microseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		///  multiplied by 1,000.
		/// <li>{@code MILLIS} -
		///  Returns a {@code LocalTime} with the specified number of milliseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		///  multiplied by 1,000,000.
		/// <li>{@code SECONDS} -
		///  Returns a {@code LocalTime} with the specified number of seconds added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/>.
		/// <li>{@code MINUTES} -
		///  Returns a {@code LocalTime} with the specified number of minutes added.
		///  This is equivalent to <seealso cref="#plusMinutes(long)"/>.
		/// <li>{@code HOURS} -
		///  Returns a {@code LocalTime} with the specified number of hours added.
		///  This is equivalent to <seealso cref="#plusHours(long)"/>.
		/// <li>{@code HALF_DAYS} -
		///  Returns a {@code LocalTime} with the specified number of half-days added.
		///  This is equivalent to <seealso cref="#plusHours(long)"/> with the amount
		///  multiplied by 12.
		/// </ul>
		/// </para>
		/// <para>
		/// All other {@code ChronoUnit} instances will throw an {@code UnsupportedTemporalTypeException}.
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
		/// <returns> a {@code LocalTime} based on this time with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalTime Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case NANOS:
						return PlusNanos(amountToAdd);
					case MICROS:
						return PlusNanos((amountToAdd % MICROS_PER_DAY) * 1000);
					case MILLIS:
						return PlusNanos((amountToAdd % MILLIS_PER_DAY) * 1000000);
					case SECONDS:
						return PlusSeconds(amountToAdd);
					case MINUTES:
						return PlusMinutes(amountToAdd);
					case HOURS:
						return PlusHours(amountToAdd);
					case HALF_DAYS:
						return PlusHours((amountToAdd % 2) * 12);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of hours added.
		/// <para>
		/// This adds the specified number of hours to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hoursToAdd">  the hours to add, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the hours added, not null </returns>
		public LocalTime PlusHours(long hoursToAdd)
		{
			if (hoursToAdd == 0)
			{
				return this;
			}
			int newHour = ((int)(hoursToAdd % HOURS_PER_DAY) + Hour_Renamed + HOURS_PER_DAY) % HOURS_PER_DAY;
			return Create(newHour, Minute_Renamed, Second_Renamed, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of minutes added.
		/// <para>
		/// This adds the specified number of minutes to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutesToAdd">  the minutes to add, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the minutes added, not null </returns>
		public LocalTime PlusMinutes(long minutesToAdd)
		{
			if (minutesToAdd == 0)
			{
				return this;
			}
			int mofd = Hour_Renamed * MINUTES_PER_HOUR + Minute_Renamed;
			int newMofd = ((int)(minutesToAdd % MINUTES_PER_DAY) + mofd + MINUTES_PER_DAY) % MINUTES_PER_DAY;
			if (mofd == newMofd)
			{
				return this;
			}
			int newHour = newMofd / MINUTES_PER_HOUR;
			int newMinute = newMofd % MINUTES_PER_HOUR;
			return Create(newHour, newMinute, Second_Renamed, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of seconds added.
		/// <para>
		/// This adds the specified number of seconds to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondstoAdd">  the seconds to add, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the seconds added, not null </returns>
		public LocalTime PlusSeconds(long secondstoAdd)
		{
			if (secondstoAdd == 0)
			{
				return this;
			}
			int sofd = Hour_Renamed * SECONDS_PER_HOUR + Minute_Renamed * SECONDS_PER_MINUTE + Second_Renamed;
			int newSofd = ((int)(secondstoAdd % SECONDS_PER_DAY) + sofd + SECONDS_PER_DAY) % SECONDS_PER_DAY;
			if (sofd == newSofd)
			{
				return this;
			}
			int newHour = newSofd / SECONDS_PER_HOUR;
			int newMinute = (newSofd / SECONDS_PER_MINUTE) % MINUTES_PER_HOUR;
			int newSecond = newSofd % SECONDS_PER_MINUTE;
			return Create(newHour, newMinute, newSecond, Nano_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of nanoseconds added.
		/// <para>
		/// This adds the specified number of nanoseconds to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToAdd">  the nanos to add, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the nanoseconds added, not null </returns>
		public LocalTime PlusNanos(long nanosToAdd)
		{
			if (nanosToAdd == 0)
			{
				return this;
			}
			long nofd = ToNanoOfDay();
			long newNofd = ((nanosToAdd % NANOS_PER_DAY) + nofd + NANOS_PER_DAY) % NANOS_PER_DAY;
			if (nofd == newNofd)
			{
				return this;
			}
			int newHour = (int)(newNofd / NANOS_PER_HOUR);
			int newMinute = (int)((newNofd / NANOS_PER_MINUTE) % MINUTES_PER_HOUR);
			int newSecond = (int)((newNofd / NANOS_PER_SECOND) % SECONDS_PER_MINUTE);
			int newNano = (int)(newNofd % NANOS_PER_SECOND);
			return Create(newHour, newMinute, newSecond, newNano);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the specified amount subtracted.
		/// The amount is typically <seealso cref="Duration"/> but may be any other type implementing
		/// the <seealso cref="TemporalAmount"/> interface.
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
		/// <returns> a {@code LocalTime} based on this time with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalTime Minus(TemporalAmount amountToSubtract)
		{
			return (LocalTime) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalTime}, based on this one, with the amount
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
		/// <returns> a {@code LocalTime} based on this time with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalTime Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of hours subtracted.
		/// <para>
		/// This subtracts the specified number of hours from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hoursToSubtract">  the hours to subtract, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the hours subtracted, not null </returns>
		public LocalTime MinusHours(long hoursToSubtract)
		{
			return PlusHours(-(hoursToSubtract % HOURS_PER_DAY));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of minutes subtracted.
		/// <para>
		/// This subtracts the specified number of minutes from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutesToSubtract">  the minutes to subtract, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the minutes subtracted, not null </returns>
		public LocalTime MinusMinutes(long minutesToSubtract)
		{
			return PlusMinutes(-(minutesToSubtract % MINUTES_PER_DAY));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of seconds subtracted.
		/// <para>
		/// This subtracts the specified number of seconds from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToSubtract">  the seconds to subtract, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the seconds subtracted, not null </returns>
		public LocalTime MinusSeconds(long secondsToSubtract)
		{
			return PlusSeconds(-(secondsToSubtract % SECONDS_PER_DAY));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalTime} with the specified number of nanoseconds subtracted.
		/// <para>
		/// This subtracts the specified number of nanoseconds from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToSubtract">  the nanos to subtract, may be negative </param>
		/// <returns> a {@code LocalTime} based on this time with the nanoseconds subtracted, not null </returns>
		public LocalTime MinusNanos(long nanosToSubtract)
		{
			return PlusNanos(-(nanosToSubtract % NANOS_PER_DAY));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this time using the specified query.
		/// <para>
		/// This queries this time using the specified query strategy object.
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
			if (query == TemporalQueries.Chronology() || query == TemporalQueries.ZoneId() || query == TemporalQueries.Zone() || query == TemporalQueries.Offset())
			{
				return temporal.TemporalAccessor_Fields.Null;
			}
			else if (query == TemporalQueries.LocalTime())
			{
				return (R) this;
			}
			else if (query == TemporalQueries.LocalDate())
			{
				return temporal.TemporalAccessor_Fields.Null;
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
		/// Adjusts the specified temporal object to have the same time as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the time changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#NANO_OF_DAY"/> as the field.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisLocalTime.adjustInto(temporal);
		///   temporal = temporal.with(thisLocalTime);
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
			return temporal.With(NANO_OF_DAY, ToNanoOfDay());
		}

		/// <summary>
		/// Calculates the amount of time until another time in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code LocalTime}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified time.
		/// The result will be negative if the end is before the start.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code LocalTime} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in hours between two times can be calculated
		/// using {@code startTime.until(endTime, HOURS)}.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two times.
		/// For example, the amount in hours between 11:30 and 13:29 will only
		/// be one hour as it is one minute short of two hours.
		/// </para>
		/// <para>
		/// There are two equivalent ways of using this method.
		/// The first is to invoke this method.
		/// The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		/// <pre>
		///   // these two lines are equivalent
		///   amount = start.until(end, MINUTES);
		///   amount = MINUTES.between(start, end);
		/// </pre>
		/// The choice should be made based on which makes the code more readable.
		/// </para>
		/// <para>
		/// The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		/// The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		/// {@code MINUTES}, {@code HOURS} and {@code HALF_DAYS} are supported.
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
		/// <param name="endExclusive">  the end time, exclusive, which is converted to a {@code LocalTime}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this time and the end time </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code LocalTime} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			LocalTime end = LocalTime.From(endExclusive);
			if (unit is ChronoUnit)
			{
				long nanosUntil = end.ToNanoOfDay() - ToNanoOfDay(); // no overflow
				switch ((ChronoUnit) unit)
				{
					case NANOS:
						return nanosUntil;
					case MICROS:
						return nanosUntil / 1000;
					case MILLIS:
						return nanosUntil / 1000000;
					case SECONDS:
						return nanosUntil / NANOS_PER_SECOND;
					case MINUTES:
						return nanosUntil / NANOS_PER_MINUTE;
					case HOURS:
						return nanosUntil / NANOS_PER_HOUR;
					case HALF_DAYS:
						return nanosUntil / (12 * NANOS_PER_HOUR);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.Between(this, end);
		}

		/// <summary>
		/// Formats this time using the specified formatter.
		/// <para>
		/// This time will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted time string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public String Format(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this time with a date to create a {@code LocalDateTime}.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this time at the specified date.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date">  the date to combine with, not null </param>
		/// <returns> the local date-time formed from this time and the specified date, not null </returns>
		public LocalDateTime AtDate(LocalDate date)
		{
			return LocalDateTime.Of(date, this);
		}

		/// <summary>
		/// Combines this time with an offset to create an {@code OffsetTime}.
		/// <para>
		/// This returns an {@code OffsetTime} formed from this time at the specified offset.
		/// All possible combinations of time and offset are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to combine with, not null </param>
		/// <returns> the offset time formed from this time and the specified offset, not null </returns>
		public OffsetTime AtOffset(ZoneOffset offset)
		{
			return OffsetTime.Of(this, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Extracts the time as seconds of day,
		/// from {@code 0} to {@code 24 * 60 * 60 - 1}.
		/// </summary>
		/// <returns> the second-of-day equivalent to this time </returns>
		public int ToSecondOfDay()
		{
			int total = Hour_Renamed * SECONDS_PER_HOUR;
			total += Minute_Renamed * SECONDS_PER_MINUTE;
			total += Second_Renamed;
			return total;
		}

		/// <summary>
		/// Extracts the time as nanos of day,
		/// from {@code 0} to {@code 24 * 60 * 60 * 1,000,000,000 - 1}.
		/// </summary>
		/// <returns> the nano of day equivalent to this time </returns>
		public long ToNanoOfDay()
		{
			long total = Hour_Renamed * NANOS_PER_HOUR;
			total += Minute_Renamed * NANOS_PER_MINUTE;
			total += Second_Renamed * NANOS_PER_SECOND;
			total += Nano_Renamed;
			return total;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this time to another time.
		/// <para>
		/// The comparison is based on the time-line position of the local times within a day.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public int CompareTo(LocalTime other)
		{
			int cmp = Integer.Compare(Hour_Renamed, other.Hour_Renamed);
			if (cmp == 0)
			{
				cmp = Integer.Compare(Minute_Renamed, other.Minute_Renamed);
				if (cmp == 0)
				{
					cmp = Integer.Compare(Second_Renamed, other.Second_Renamed);
					if (cmp == 0)
					{
						cmp = Integer.Compare(Nano_Renamed, other.Nano_Renamed);
					}
				}
			}
			return cmp;
		}

		/// <summary>
		/// Checks if this time is after the specified time.
		/// <para>
		/// The comparison is based on the time-line position of the time within a day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> true if this is after the specified time </returns>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public bool IsAfter(LocalTime other)
		{
			return CompareTo(other) > 0;
		}

		/// <summary>
		/// Checks if this time is before the specified time.
		/// <para>
		/// The comparison is based on the time-line position of the time within a day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> true if this point is before the specified time </returns>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public bool IsBefore(LocalTime other)
		{
			return CompareTo(other) < 0;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this time is equal to another time.
		/// <para>
		/// The comparison is based on the time-line position of the time within a day.
		/// </para>
		/// <para>
		/// Only objects of type {@code LocalTime} are compared, other types return false.
		/// To compare the date of two {@code TemporalAccessor} instances, use
		/// <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other time </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is LocalTime)
			{
				LocalTime other = (LocalTime) obj;
				return Hour_Renamed == other.Hour_Renamed && Minute_Renamed == other.Minute_Renamed && Second_Renamed == other.Second_Renamed && Nano_Renamed == other.Nano_Renamed;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			long nod = ToNanoOfDay();
			return (int)(nod ^ ((long)((ulong)nod >> 32)));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this time as a {@code String}, such as {@code 10:15}.
		/// <para>
		/// The output will be one of the following ISO-8601 formats:
		/// <ul>
		/// <li>{@code HH:mm}</li>
		/// <li>{@code HH:mm:ss}</li>
		/// <li>{@code HH:mm:ss.SSS}</li>
		/// <li>{@code HH:mm:ss.SSSSSS}</li>
		/// <li>{@code HH:mm:ss.SSSSSSSSS}</li>
		/// </ul>
		/// The format used will be the shortest that outputs the full value of
		/// the time where the omitted parts are implied to be zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this time, not null </returns>
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder(18);
			int hourValue = Hour_Renamed;
			int minuteValue = Minute_Renamed;
			int secondValue = Second_Renamed;
			int nanoValue = Nano_Renamed;
			buf.Append(hourValue < 10 ? "0" : "").Append(hourValue).Append(minuteValue < 10 ? ":0" : ":").Append(minuteValue);
			if (secondValue > 0 || nanoValue > 0)
			{
				buf.Append(secondValue < 10 ? ":0" : ":").Append(secondValue);
				if (nanoValue > 0)
				{
					buf.Append('.');
					if (nanoValue % 1000000 == 0)
					{
						buf.Append(Convert.ToString((nanoValue / 1000000) + 1000).Substring(1));
					}
					else if (nanoValue % 1000 == 0)
					{
						buf.Append(Convert.ToString((nanoValue / 1000) + 1000000).Substring(1));
					}
					else
					{
						buf.Append(Convert.ToString((nanoValue) + 1000000000).Substring(1));
					}
				}
			}
			return buf.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// A twos-complement value indicates the remaining values are not in the stream
		/// and should be set to zero.
		/// <pre>
		///  out.writeByte(4);  // identifies a LocalTime
		///  if (nano == 0) {
		///    if (second == 0) {
		///      if (minute == 0) {
		///        out.writeByte(~hour);
		///      } else {
		///        out.writeByte(hour);
		///        out.writeByte(~minute);
		///      }
		///    } else {
		///      out.writeByte(hour);
		///      out.writeByte(minute);
		///      out.writeByte(~second);
		///    }
		///  } else {
		///    out.writeByte(hour);
		///    out.writeByte(minute);
		///    out.writeByte(second);
		///    out.writeInt(nano);
		///  }
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.LOCAL_TIME_TYPE, this);
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
			if (Nano_Renamed == 0)
			{
				if (Second_Renamed == 0)
				{
					if (Minute_Renamed == 0)
					{
						@out.WriteByte(~Hour_Renamed);
					}
					else
					{
						@out.WriteByte(Hour_Renamed);
						@out.WriteByte(~Minute_Renamed);
					}
				}
				else
				{
					@out.WriteByte(Hour_Renamed);
					@out.WriteByte(Minute_Renamed);
					@out.WriteByte(~Second_Renamed);
				}
			}
			else
			{
				@out.WriteByte(Hour_Renamed);
				@out.WriteByte(Minute_Renamed);
				@out.WriteByte(Second_Renamed);
				@out.WriteInt(Nano_Renamed);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static LocalTime readExternal(java.io.DataInput in) throws java.io.IOException
		internal static LocalTime ReadExternal(DataInput @in)
		{
			int hour = @in.ReadByte();
			int minute = 0;
			int second = 0;
			int nano = 0;
			if (hour < 0)
			{
				hour = ~hour;
			}
			else
			{
				minute = @in.ReadByte();
				if (minute < 0)
				{
					minute = ~minute;
				}
				else
				{
					second = @in.ReadByte();
					if (second < 0)
					{
						second = ~second;
					}
					else
					{
						nano = @in.ReadInt();
					}
				}
			}
			return LocalTime.Of(hour, minute, second, nano);
		}

	}

}