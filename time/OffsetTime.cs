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
	/// A time with an offset from UTC/Greenwich in the ISO-8601 calendar system,
	/// such as {@code 10:15:30+01:00}.
	/// <para>
	/// {@code OffsetTime} is an immutable date-time object that represents a time, often
	/// viewed as hour-minute-second-offset.
	/// This class stores all time fields, to a precision of nanoseconds,
	/// as well as a zone offset.
	/// For example, the value "13:45.30.123456789+02:00" can be stored
	/// in an {@code OffsetTime}.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code OffsetTime} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class OffsetTime : Temporal, TemporalAdjuster, Comparable<OffsetTime>
	{

		/// <summary>
		/// The minimum supported {@code OffsetTime}, '00:00:00+18:00'.
		/// This is the time of midnight at the start of the day in the maximum offset
		/// (larger offsets are earlier on the time-line).
		/// This combines <seealso cref="LocalTime#MIN"/> and <seealso cref="ZoneOffset#MAX"/>.
		/// This could be used by an application as a "far past" date.
		/// </summary>
		public static readonly OffsetTime MIN = LocalTime.MIN.AtOffset(ZoneOffset.MAX);
		/// <summary>
		/// The maximum supported {@code OffsetTime}, '23:59:59.999999999-18:00'.
		/// This is the time just before midnight at the end of the day in the minimum offset
		/// (larger negative offsets are later on the time-line).
		/// This combines <seealso cref="LocalTime#MAX"/> and <seealso cref="ZoneOffset#MIN"/>.
		/// This could be used by an application as a "far future" date.
		/// </summary>
		public static readonly OffsetTime MAX = LocalTime.MAX.AtOffset(ZoneOffset.MIN);

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 7264499704384272492L;

		/// <summary>
		/// The local date-time.
		/// </summary>
		private readonly LocalTime Time;
		/// <summary>
		/// The offset from UTC/Greenwich.
		/// </summary>
		private readonly ZoneOffset Offset_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current time from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current time.
		/// The offset will be calculated from the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current time using the system clock and default time-zone, not null </returns>
		public static OffsetTime Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current time from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current time.
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
		/// <returns> the current time using the system clock, not null </returns>
		public static OffsetTime Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current time from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current time.
		/// The offset will be calculated from the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current time, not null </returns>
		public static OffsetTime Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			return OfInstant(now, clock.Zone.Rules.GetOffset(now));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from a local time and an offset.
		/// </summary>
		/// <param name="time">  the local time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the offset time, not null </returns>
		public static OffsetTime Of(LocalTime time, ZoneOffset offset)
		{
			return new OffsetTime(time, offset);
		}

		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from an hour, minute, second and nanosecond.
		/// <para>
		/// This creates an offset time with the four specified fields.
		/// </para>
		/// <para>
		/// This method exists primarily for writing test cases.
		/// Non test-code will typically use other methods to create an offset time.
		/// {@code LocalTime} has two additional convenience variants of the
		/// equivalent factory method taking fewer arguments.
		/// They are not provided here to reduce the footprint of the API.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <returns> the offset time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public static OffsetTime Of(int hour, int minute, int second, int nanoOfSecond, ZoneOffset offset)
		{
			return new OffsetTime(LocalTime.Of(hour, minute, second, nanoOfSecond), offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from an {@code Instant} and zone ID.
		/// <para>
		/// This creates an offset time with the same instant as that specified.
		/// Finding the offset from UTC/Greenwich is simple as there is only one valid
		/// offset for each instant.
		/// </para>
		/// <para>
		/// The date component of the instant is dropped during the conversion.
		/// This means that the conversion can never fail due to the instant being
		/// out of the valid range of dates.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to create the time from, not null </param>
		/// <param name="zone">  the time-zone, which may be an offset, not null </param>
		/// <returns> the offset time, not null </returns>
		public static OffsetTime OfInstant(Instant instant, ZoneId zone)
		{
			Objects.RequireNonNull(instant, "instant");
			Objects.RequireNonNull(zone, "zone");
			ZoneRules rules = zone.Rules;
			ZoneOffset offset = rules.GetOffset(instant);
			long localSecond = instant.EpochSecond + offset.TotalSeconds; // overflow caught later
			int secsOfDay = (int) Math.FloorMod(localSecond, SECONDS_PER_DAY);
			LocalTime time = LocalTime.OfNanoOfDay(secsOfDay * NANOS_PER_SECOND + instant.Nano);
			return new OffsetTime(time, offset);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from a temporal object.
		/// <para>
		/// This obtains an offset time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code OffsetTime}.
		/// </para>
		/// <para>
		/// The conversion extracts and combines the {@code ZoneOffset} and the
		/// {@code LocalTime} from the temporal object.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code OffsetTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the offset time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to an {@code OffsetTime} </exception>
		public static OffsetTime From(TemporalAccessor temporal)
		{
			if (temporal is OffsetTime)
			{
				return (OffsetTime) temporal;
			}
			try
			{
				LocalTime time = LocalTime.From(temporal);
				ZoneOffset offset = ZoneOffset.From(temporal);
				return new OffsetTime(time, offset);
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain OffsetTime from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from a text string such as {@code 10:15:30+01:00}.
		/// <para>
		/// The string must represent a valid time and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_OFFSET_TIME"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "10:15:30+01:00", not null </param>
		/// <returns> the parsed local time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static OffsetTime Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_OFFSET_TIME);
		}

		/// <summary>
		/// Obtains an instance of {@code OffsetTime} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed offset time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static OffsetTime Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, OffsetTime::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="time">  the local time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		private OffsetTime(LocalTime time, ZoneOffset offset)
		{
			this.Time = Objects.RequireNonNull(time, "time");
			this.Offset_Renamed = Objects.RequireNonNull(offset, "offset");
		}

		/// <summary>
		/// Returns a new time based on this one, returning {@code this} where possible.
		/// </summary>
		/// <param name="time">  the time to create with, not null </param>
		/// <param name="offset">  the zone offset to create with, not null </param>
		private OffsetTime With(LocalTime time, ZoneOffset offset)
		{
			if (this.Time == time && this.Offset_Renamed.Equals(offset))
			{
				return this;
			}
			return new OffsetTime(time, offset);
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
		/// <returns> true if the field is supported on this time, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field.TimeBased || field == OFFSET_SECONDS;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this offset-time.
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
		public override ValueRange temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field is ChronoField)
			{
				if (field == OFFSET_SECONDS)
				{
					return field.Range();
				}
				return Time.Range(field);
			}
			return field.RangeRefinedBy(this);
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
		public override int Get(TemporalField field) // override for Javadoc
		{
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
				if (field == OFFSET_SECONDS)
				{
					return Offset_Renamed.TotalSeconds;
				}
				return Time.GetLong(field);
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the zone offset, such as '+01:00'.
		/// <para>
		/// This is the offset of the local time from UTC/Greenwich.
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
		/// Returns a copy of this {@code OffsetTime} with the specified offset ensuring
		/// that the result has the same local time.
		/// <para>
		/// This method returns an object with the same {@code LocalTime} and the specified {@code ZoneOffset}.
		/// No calculation is needed or performed.
		/// For example, if this time represents {@code 10:30+02:00} and the offset specified is
		/// {@code +03:00}, then this method will return {@code 10:30+03:00}.
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
		/// <returns> an {@code OffsetTime} based on this time with the requested offset, not null </returns>
		public OffsetTime WithOffsetSameLocal(ZoneOffset offset)
		{
			return offset != temporal.TemporalAccessor_Fields.Null && offset.Equals(this.Offset_Renamed) ? this : new OffsetTime(Time, offset);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified offset ensuring
		/// that the result is at the same instant on an implied day.
		/// <para>
		/// This method returns an object with the specified {@code ZoneOffset} and a {@code LocalTime}
		/// adjusted by the difference between the two offsets.
		/// This will result in the old and new objects representing the same instant on an implied day.
		/// This is useful for finding the local time in a different offset.
		/// For example, if this time represents {@code 10:30+02:00} and the offset specified is
		/// {@code +03:00}, then this method will return {@code 11:30+03:00}.
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
		/// <returns> an {@code OffsetTime} based on this time with the requested offset, not null </returns>
		public OffsetTime WithOffsetSameInstant(ZoneOffset offset)
		{
			if (offset.Equals(this.Offset_Renamed))
			{
				return this;
			}
			int difference = offset.TotalSeconds - this.Offset_Renamed.TotalSeconds;
			LocalTime adjusted = Time.PlusSeconds(difference);
			return new OffsetTime(adjusted, offset);
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
			return Time;
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
		/// Returns an adjusted copy of this time.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the time adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
		/// </para>
		/// <para>
		/// A simple adjuster might simply set the one of the fields, such as the hour field.
		/// A more complex adjuster might set the time to the last hour of the day.
		/// </para>
		/// <para>
		/// The classes <seealso cref="LocalTime"/> and <seealso cref="ZoneOffset"/> implement {@code TemporalAdjuster},
		/// thus this method can be used to change the time or offset:
		/// <pre>
		///  result = offsetTime.with(time);
		///  result = offsetTime.with(offset);
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
		/// <returns> an {@code OffsetTime} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetTime With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalTime)
			{
				return With((LocalTime) adjuster, Offset_Renamed);
			}
			else if (adjuster is ZoneOffset)
			{
				return With(Time, (ZoneOffset) adjuster);
			}
			else if (adjuster is OffsetTime)
			{
				return (OffsetTime) adjuster;
			}
			return (OffsetTime) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified field set to a new value.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the value
		/// for the specified field changed.
		/// This can be used to change any supported field, such as the hour, minute or second.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// </para>
		/// <para>
		/// The {@code OFFSET_SECONDS} field will return a time with the specified offset.
		/// The local time is unaltered. If the new offset value is outside the valid range
		/// then a {@code DateTimeException} will be thrown.
		/// </para>
		/// <para>
		/// The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		/// the matching method on <seealso cref="LocalTime#with(TemporalField, long)"/> LocalTime}.
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
		/// <returns> an {@code OffsetTime} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public OffsetTime With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				if (field == OFFSET_SECONDS)
				{
					ChronoField f = (ChronoField) field;
					return With(Time, ZoneOffset.OfTotalSeconds(f.checkValidIntValue(newValue)));
				}
				return With(Time.With(field, newValue), Offset_Renamed);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the hour-of-day altered.
		/// <para>
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		/// <returns> an {@code OffsetTime} based on this time with the requested hour, not null </returns>
		/// <exception cref="DateTimeException"> if the hour value is invalid </exception>
		public OffsetTime WithHour(int hour)
		{
			return With(Time.WithHour(hour), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the minute-of-hour altered.
		/// <para>
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		/// <returns> an {@code OffsetTime} based on this time with the requested minute, not null </returns>
		/// <exception cref="DateTimeException"> if the minute value is invalid </exception>
		public OffsetTime WithMinute(int minute)
		{
			return With(Time.WithMinute(minute), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the second-of-minute altered.
		/// <para>
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		/// <returns> an {@code OffsetTime} based on this time with the requested second, not null </returns>
		/// <exception cref="DateTimeException"> if the second value is invalid </exception>
		public OffsetTime WithSecond(int second)
		{
			return With(Time.WithSecond(second), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the nano-of-second altered.
		/// <para>
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		/// <returns> an {@code OffsetTime} based on this time with the requested nanosecond, not null </returns>
		/// <exception cref="DateTimeException"> if the nanos value is invalid </exception>
		public OffsetTime WithNano(int nanoOfSecond)
		{
			return With(Time.WithNano(nanoOfSecond), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the time truncated.
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
		/// The offset does not affect the calculation and will be the same in the result.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to truncate to, not null </param>
		/// <returns> an {@code OffsetTime} based on this time with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if unable to truncate </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public OffsetTime TruncatedTo(TemporalUnit unit)
		{
			return With(Time.TruncatedTo(unit), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this time with the specified amount added.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the specified amount added.
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
		/// <returns> an {@code OffsetTime} based on this time with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetTime Plus(TemporalAmount amountToAdd)
		{
			return (OffsetTime) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified amount added.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented by
		/// <seealso cref="LocalTime#plus(long, TemporalUnit)"/>.
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
		/// <returns> an {@code OffsetTime} based on this time with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public OffsetTime Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				return With(Time.Plus(amountToAdd, unit), Offset_Renamed);
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of hours added.
		/// <para>
		/// This adds the specified number of hours to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to add, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the hours added, not null </returns>
		public OffsetTime PlusHours(long hours)
		{
			return With(Time.PlusHours(hours), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of minutes added.
		/// <para>
		/// This adds the specified number of minutes to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to add, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the minutes added, not null </returns>
		public OffsetTime PlusMinutes(long minutes)
		{
			return With(Time.PlusMinutes(minutes), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of seconds added.
		/// <para>
		/// This adds the specified number of seconds to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to add, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the seconds added, not null </returns>
		public OffsetTime PlusSeconds(long seconds)
		{
			return With(Time.PlusSeconds(seconds), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of nanoseconds added.
		/// <para>
		/// This adds the specified number of nanoseconds to this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to add, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the nanoseconds added, not null </returns>
		public OffsetTime PlusNanos(long nanos)
		{
			return With(Time.PlusNanos(nanos), Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this time with the specified amount subtracted.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the specified amount subtracted.
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
		/// <returns> an {@code OffsetTime} based on this time with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetTime Minus(TemporalAmount amountToSubtract)
		{
			return (OffsetTime) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this time with the specified amount subtracted.
		/// <para>
		/// This returns an {@code OffsetTime}, based on this one, with the amount
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
		/// <returns> an {@code OffsetTime} based on this time with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override OffsetTime Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of hours subtracted.
		/// <para>
		/// This subtracts the specified number of hours from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to subtract, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the hours subtracted, not null </returns>
		public OffsetTime MinusHours(long hours)
		{
			return With(Time.MinusHours(hours), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of minutes subtracted.
		/// <para>
		/// This subtracts the specified number of minutes from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to subtract, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the minutes subtracted, not null </returns>
		public OffsetTime MinusMinutes(long minutes)
		{
			return With(Time.MinusMinutes(minutes), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of seconds subtracted.
		/// <para>
		/// This subtracts the specified number of seconds from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to subtract, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the seconds subtracted, not null </returns>
		public OffsetTime MinusSeconds(long seconds)
		{
			return With(Time.MinusSeconds(seconds), Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code OffsetTime} with the specified number of nanoseconds subtracted.
		/// <para>
		/// This subtracts the specified number of nanoseconds from this time, returning a new time.
		/// The calculation wraps around midnight.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to subtract, may be negative </param>
		/// <returns> an {@code OffsetTime} based on this time with the nanoseconds subtracted, not null </returns>
		public OffsetTime MinusNanos(long nanos)
		{
			return With(Time.MinusNanos(nanos), Offset_Renamed);
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
			if (query == TemporalQueries.Offset() || query == TemporalQueries.Zone())
			{
				return (R) Offset_Renamed;
			}
			else if (query == TemporalQueries.ZoneId() | query == TemporalQueries.Chronology() || query == TemporalQueries.LocalDate())
			{
				return temporal.TemporalAccessor_Fields.Null;
			}
			else if (query == TemporalQueries.LocalTime())
			{
				return (R) Time;
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
		/// Adjusts the specified temporal object to have the same offset and time
		/// as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the offset and time changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// twice, passing <seealso cref="ChronoField#NANO_OF_DAY"/> and
		/// <seealso cref="ChronoField#OFFSET_SECONDS"/> as the fields.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisOffsetTime.adjustInto(temporal);
		///   temporal = temporal.with(thisOffsetTime);
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
			return temporal.With(NANO_OF_DAY, Time.ToNanoOfDay()).With(OFFSET_SECONDS, Offset_Renamed.TotalSeconds);
		}

		/// <summary>
		/// Calculates the amount of time until another time in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code OffsetTime}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified time.
		/// The result will be negative if the end is before the start.
		/// For example, the amount in hours between two times can be calculated
		/// using {@code startTime.until(endTime, HOURS)}.
		/// </para>
		/// <para>
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code OffsetTime} using <seealso cref="#from(TemporalAccessor)"/>.
		/// If the offset differs between the two times, then the specified
		/// end time is normalized to have the same offset as this time.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two times.
		/// For example, the amount in hours between 11:30Z and 13:29Z will only
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
		/// <param name="endExclusive">  the end time, exclusive, which is converted to an {@code OffsetTime}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this time and the end time </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to an {@code OffsetTime} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			OffsetTime end = OffsetTime.From(endExclusive);
			if (unit is ChronoUnit)
			{
				long nanosUntil = end.ToEpochNano() - ToEpochNano(); // no overflow
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
		/// Combines this time with a date to create an {@code OffsetDateTime}.
		/// <para>
		/// This returns an {@code OffsetDateTime} formed from this time and the specified date.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date">  the date to combine with, not null </param>
		/// <returns> the offset date-time formed from this time and the specified date, not null </returns>
		public OffsetDateTime AtDate(LocalDate date)
		{
			return OffsetDateTime.Of(date, Time, Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Converts this time to epoch nanos based on 1970-01-01Z.
		/// </summary>
		/// <returns> the epoch nanos value </returns>
		private long ToEpochNano()
		{
			long nod = Time.ToNanoOfDay();
			long offsetNanos = Offset_Renamed.TotalSeconds * NANOS_PER_SECOND;
			return nod - offsetNanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this {@code OffsetTime} to another time.
		/// <para>
		/// The comparison is based first on the UTC equivalent instant, then on the local time.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// For example, the following is the comparator order:
		/// <ol>
		/// <li>{@code 10:30+01:00}</li>
		/// <li>{@code 11:00+01:00}</li>
		/// <li>{@code 12:00+02:00}</li>
		/// <li>{@code 11:30+01:00}</li>
		/// <li>{@code 12:00+01:00}</li>
		/// <li>{@code 12:30+01:00}</li>
		/// </ol>
		/// Values #2 and #3 represent the same instant on the time-line.
		/// When two values represent the same instant, the local time is compared
		/// to distinguish them. This step is needed to make the ordering
		/// consistent with {@code equals()}.
		/// </para>
		/// <para>
		/// To compare the underlying local time of two {@code TemporalAccessor} instances,
		/// use <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public int CompareTo(OffsetTime other)
		{
			if (Offset_Renamed.Equals(other.Offset_Renamed))
			{
				return Time.CompareTo(other.Time);
			}
			int compare = Long.Compare(ToEpochNano(), other.ToEpochNano());
			if (compare == 0)
			{
				compare = Time.CompareTo(other.Time);
			}
			return compare;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the instant of this {@code OffsetTime} is after that of the
		/// specified time applying both times to a common date.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the instant of the time. This is equivalent to converting both
		/// times to an instant using the same date and comparing the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> true if this is after the instant of the specified time </returns>
		public bool IsAfter(OffsetTime other)
		{
			return ToEpochNano() > other.ToEpochNano();
		}

		/// <summary>
		/// Checks if the instant of this {@code OffsetTime} is before that of the
		/// specified time applying both times to a common date.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the instant of the time. This is equivalent to converting both
		/// times to an instant using the same date and comparing the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> true if this is before the instant of the specified time </returns>
		public bool IsBefore(OffsetTime other)
		{
			return ToEpochNano() < other.ToEpochNano();
		}

		/// <summary>
		/// Checks if the instant of this {@code OffsetTime} is equal to that of the
		/// specified time applying both times to a common date.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		/// in that it only compares the instant of the time. This is equivalent to converting both
		/// times to an instant using the same date and comparing the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other time to compare to, not null </param>
		/// <returns> true if this is equal to the instant of the specified time </returns>
		public bool IsEqual(OffsetTime other)
		{
			return ToEpochNano() == other.ToEpochNano();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this time is equal to another time.
		/// <para>
		/// The comparison is based on the local-time and the offset.
		/// To compare for the same instant on the time-line, use <seealso cref="#isEqual(OffsetTime)"/>.
		/// </para>
		/// <para>
		/// Only objects of type {@code OffsetTime} are compared, other types return false.
		/// To compare the underlying local time of two {@code TemporalAccessor} instances,
		/// use <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
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
			if (obj is OffsetTime)
			{
				OffsetTime other = (OffsetTime) obj;
				return Time.Equals(other.Time) && Offset_Renamed.Equals(other.Offset_Renamed);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Time.HashCode() ^ Offset_Renamed.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this time as a {@code String}, such as {@code 10:15:30+01:00}.
		/// <para>
		/// The output will be one of the following ISO-8601 formats:
		/// <ul>
		/// <li>{@code HH:mmXXXXX}</li>
		/// <li>{@code HH:mm:ssXXXXX}</li>
		/// <li>{@code HH:mm:ss.SSSXXXXX}</li>
		/// <li>{@code HH:mm:ss.SSSSSSXXXXX}</li>
		/// <li>{@code HH:mm:ss.SSSSSSSSSXXXXX}</li>
		/// </ul>
		/// The format used will be the shortest that outputs the full value of
		/// the time where the omitted parts are implied to be zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this time, not null </returns>
		public override String ToString()
		{
			return Time.ToString() + Offset_Renamed.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(9);  // identifies an OffsetTime
		///  // the <a href="../../serialized-form.html#java.time.LocalTime">time</a> excluding the one byte header
		///  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.OFFSET_TIME_TYPE, this);
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
			Time.WriteExternal(@out);
			Offset_Renamed.WriteExternal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static OffsetTime readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		internal static OffsetTime ReadExternal(ObjectInput @in)
		{
			LocalTime time = LocalTime.ReadExternal(@in);
			ZoneOffset offset = ZoneOffset.ReadExternal(@in);
			return OffsetTime.Of(time, offset);
		}

	}

}