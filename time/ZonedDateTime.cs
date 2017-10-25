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
	/// A date-time with a time-zone in the ISO-8601 calendar system,
	/// such as {@code 2007-12-03T10:15:30+01:00 Europe/Paris}.
	/// <para>
	/// {@code ZonedDateTime} is an immutable representation of a date-time with a time-zone.
	/// This class stores all date and time fields, to a precision of nanoseconds,
	/// and a time-zone, with a zone offset used to handle ambiguous local date-times.
	/// For example, the value
	/// "2nd October 2007 at 13:45.30.123456789 +02:00 in the Europe/Paris time-zone"
	/// can be stored in a {@code ZonedDateTime}.
	/// </para>
	/// <para>
	/// This class handles conversion from the local time-line of {@code LocalDateTime}
	/// to the instant time-line of {@code Instant}.
	/// The difference between the two time-lines is the offset from UTC/Greenwich,
	/// represented by a {@code ZoneOffset}.
	/// </para>
	/// <para>
	/// Converting between the two time-lines involves calculating the offset using the
	/// <seealso cref="ZoneRules rules"/> accessed from the {@code ZoneId}.
	/// Obtaining the offset for an instant is simple, as there is exactly one valid
	/// offset for each instant. By contrast, obtaining the offset for a local date-time
	/// is not straightforward. There are three cases:
	/// <ul>
	/// <li>Normal, with one valid offset. For the vast majority of the year, the normal
	///  case applies, where there is a single valid offset for the local date-time.</li>
	/// <li>Gap, with zero valid offsets. This is when clocks jump forward typically
	///  due to the spring daylight savings change from "winter" to "summer".
	///  In a gap there are local date-time values with no valid offset.</li>
	/// <li>Overlap, with two valid offsets. This is when clocks are set back typically
	///  due to the autumn daylight savings change from "summer" to "winter".
	///  In an overlap there are local date-time values with two valid offsets.</li>
	/// </ul>
	/// </para>
	/// <para>
	/// Any method that converts directly or implicitly from a local date-time to an
	/// instant by obtaining the offset has the potential to be complicated.
	/// </para>
	/// <para>
	/// For Gaps, the general strategy is that if the local date-time falls in the
	/// middle of a Gap, then the resulting zoned date-time will have a local date-time
	/// shifted forwards by the length of the Gap, resulting in a date-time in the later
	/// offset, typically "summer" time.
	/// </para>
	/// <para>
	/// For Overlaps, the general strategy is that if the local date-time falls in the
	/// middle of an Overlap, then the previous offset will be retained. If there is no
	/// previous offset, or the previous offset is invalid, then the earlier offset is
	/// used, typically "summer" time.. Two additional methods,
	/// <seealso cref="#withEarlierOffsetAtOverlap()"/> and <seealso cref="#withLaterOffsetAtOverlap()"/>,
	/// help manage the case of an overlap.
	/// </para>
	/// <para>
	/// In terms of design, this class should be viewed primarily as the combination
	/// of a {@code LocalDateTime} and a {@code ZoneId}. The {@code ZoneOffset} is
	/// a vital, but secondary, piece of information, used to ensure that the class
	/// represents an instant, especially during a daylight savings overlap.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code ZonedDateTime} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// A {@code ZonedDateTime} holds state equivalent to three separate objects,
	/// a {@code LocalDateTime}, a {@code ZoneId} and the resolved {@code ZoneOffset}.
	/// The offset and local date-time are used to define an instant when necessary.
	/// The zone ID is used to obtain the rules for how and when the offset changes.
	/// The offset cannot be freely set, as the zone controls which offsets are valid.
	/// </para>
	/// <para>
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ZonedDateTime : Temporal, ChronoZonedDateTime<LocalDate>
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -6260982410461394882L;

		/// <summary>
		/// The local date-time.
		/// </summary>
		private readonly LocalDateTime DateTime;
		/// <summary>
		/// The offset from UTC/Greenwich.
		/// </summary>
		private readonly ZoneOffset Offset_Renamed;
		/// <summary>
		/// The time-zone.
		/// </summary>
		private readonly ZoneId Zone_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current date-time from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current date-time.
		/// The zone and offset will be set based on the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current date-time using the system clock, not null </returns>
		public static ZonedDateTime Now()
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
		public static ZonedDateTime Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current date-time from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date-time.
		/// The zone and offset will be set based on the time-zone in the clock.
		/// </para>
		/// <para>
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date-time, not null </returns>
		public static ZonedDateTime Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			return OfInstant(now, clock.Zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a local date and time.
		/// <para>
		/// This creates a zoned date-time matching the input local date and time as closely as possible.
		/// Time-zone rules, such as daylight savings, mean that not every local date-time
		/// is valid for the specified zone, thus the local date-time may be adjusted.
		/// </para>
		/// <para>
		/// The local date time and first combined to form a local date-time.
		/// The local date-time is then resolved to a single instant on the time-line.
		/// This is achieved by finding a valid offset from UTC/Greenwich for the local
		/// date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		/// </para>
		/// <para>
		/// In most cases, there is only one valid offset for a local date-time.
		/// In the case of an overlap, when clocks are set back, there are two valid offsets.
		/// This method uses the earlier offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, when clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
		/// 
		/// </para>
		/// </summary>
		/// <param name="date">  the local date, not null </param>
		/// <param name="time">  the local time, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the offset date-time, not null </returns>
		public static ZonedDateTime Of(LocalDate date, LocalTime time, ZoneId zone)
		{
			return Of(LocalDateTime.Of(date, time), zone);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a local date-time.
		/// <para>
		/// This creates a zoned date-time matching the input local date-time as closely as possible.
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
		/// In the case of an overlap, when clocks are set back, there are two valid offsets.
		/// This method uses the earlier offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, when clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		public static ZonedDateTime Of(LocalDateTime localDateTime, ZoneId zone)
		{
			return OfLocal(localDateTime, zone, temporal.TemporalAccessor_Fields.Null);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a year, month, day,
		/// hour, minute, second, nanosecond and time-zone.
		/// <para>
		/// This creates a zoned date-time matching the local date-time of the seven
		/// specified fields as closely as possible.
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
		/// In the case of an overlap, when clocks are set back, there are two valid offsets.
		/// This method uses the earlier offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, when clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
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
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the offset date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range, or
		///  if the day-of-month is invalid for the month-year </exception>
		public static ZonedDateTime Of(int year, int month, int dayOfMonth, int hour, int minute, int second, int nanoOfSecond, ZoneId zone)
		{
			LocalDateTime dt = LocalDateTime.Of(year, month, dayOfMonth, hour, minute, second, nanoOfSecond);
			return OfLocal(dt, zone, temporal.TemporalAccessor_Fields.Null);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a local date-time
		/// using the preferred offset if possible.
		/// <para>
		/// The local date-time is resolved to a single instant on the time-line.
		/// This is achieved by finding a valid offset from UTC/Greenwich for the local
		/// date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		/// </para>
		/// <para>
		/// In most cases, there is only one valid offset for a local date-time.
		/// In the case of an overlap, where clocks are set back, there are two valid offsets.
		/// If the preferred offset is one of the valid offsets then it is used.
		/// Otherwise the earlier valid offset is used, typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, where clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <param name="preferredOffset">  the zone offset, null if no preference </param>
		/// <returns> the zoned date-time, not null </returns>
		public static ZonedDateTime OfLocal(LocalDateTime localDateTime, ZoneId zone, ZoneOffset preferredOffset)
		{
			Objects.RequireNonNull(localDateTime, "localDateTime");
			Objects.RequireNonNull(zone, "zone");
			if (zone is ZoneOffset)
			{
				return new ZonedDateTime(localDateTime, (ZoneOffset) zone, zone);
			}
			ZoneRules rules = zone.Rules;
			IList<ZoneOffset> validOffsets = rules.GetValidOffsets(localDateTime);
			ZoneOffset offset;
			if (validOffsets.Count == 1)
			{
				offset = validOffsets[0];
			}
			else if (validOffsets.Count == 0)
			{
				ZoneOffsetTransition trans = rules.GetTransition(localDateTime);
				localDateTime = localDateTime.PlusSeconds(trans.Duration.Seconds);
				offset = trans.OffsetAfter;
			}
			else
			{
				if (preferredOffset != temporal.TemporalAccessor_Fields.Null && validOffsets.Contains(preferredOffset))
				{
					offset = preferredOffset;
				}
				else
				{
					offset = Objects.RequireNonNull(validOffsets[0], "offset"); // protect against bad ZoneRules
				}
			}
			return new ZonedDateTime(localDateTime, offset, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from an {@code Instant}.
		/// <para>
		/// This creates a zoned date-time with the same instant as that specified.
		/// Calling <seealso cref="#toInstant()"/> will return an instant equal to the one used here.
		/// </para>
		/// <para>
		/// Converting an instant to a zoned date-time is simple as there is only one valid
		/// offset for each instant.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to create the date-time from, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public static ZonedDateTime OfInstant(Instant instant, ZoneId zone)
		{
			Objects.RequireNonNull(instant, "instant");
			Objects.RequireNonNull(zone, "zone");
			return Create(instant.EpochSecond, instant.Nano, zone);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from the instant formed by combining
		/// the local date-time and offset.
		/// <para>
		/// This creates a zoned date-time by <seealso cref="LocalDateTime#toInstant(ZoneOffset) combining"/>
		/// the {@code LocalDateTime} and {@code ZoneOffset}.
		/// This combination uniquely specifies an instant without ambiguity.
		/// </para>
		/// <para>
		/// Converting an instant to a zoned date-time is simple as there is only one valid
		/// offset for each instant. If the valid offset is different to the offset specified,
		/// then the date-time and offset of the zoned date-time will differ from those specified.
		/// </para>
		/// <para>
		/// If the {@code ZoneId} to be used is a {@code ZoneOffset}, this method is equivalent
		/// to <seealso cref="#of(LocalDateTime, ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		public static ZonedDateTime OfInstant(LocalDateTime localDateTime, ZoneOffset offset, ZoneId zone)
		{
			Objects.RequireNonNull(localDateTime, "localDateTime");
			Objects.RequireNonNull(offset, "offset");
			Objects.RequireNonNull(zone, "zone");
			if (zone.Rules.IsValidOffset(localDateTime, offset))
			{
				return new ZonedDateTime(localDateTime, offset, zone);
			}
			return Create(localDateTime.toEpochSecond(offset), localDateTime.Nano, zone);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} using seconds from the
		/// epoch of 1970-01-01T00:00:00Z.
		/// </summary>
		/// <param name="epochSecond">  the number of seconds from the epoch of 1970-01-01T00:00:00Z </param>
		/// <param name="nanoOfSecond">  the nanosecond within the second, from 0 to 999,999,999 </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		private static ZonedDateTime Create(long epochSecond, int nanoOfSecond, ZoneId zone)
		{
			ZoneRules rules = zone.Rules;
			Instant instant = Instant.OfEpochSecond(epochSecond, nanoOfSecond); // TODO: rules should be queryable by epochSeconds
			ZoneOffset offset = rules.GetOffset(instant);
			LocalDateTime ldt = LocalDateTime.OfEpochSecond(epochSecond, nanoOfSecond, offset);
			return new ZonedDateTime(ldt, offset, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} strictly validating the
		/// combination of local date-time, offset and zone ID.
		/// <para>
		/// This creates a zoned date-time ensuring that the offset is valid for the
		/// local date-time according to the rules of the specified zone.
		/// If the offset is invalid, an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		public static ZonedDateTime OfStrict(LocalDateTime localDateTime, ZoneOffset offset, ZoneId zone)
		{
			Objects.RequireNonNull(localDateTime, "localDateTime");
			Objects.RequireNonNull(offset, "offset");
			Objects.RequireNonNull(zone, "zone");
			ZoneRules rules = zone.Rules;
			if (rules.IsValidOffset(localDateTime, offset) == false)
			{
				ZoneOffsetTransition trans = rules.GetTransition(localDateTime);
				if (trans != temporal.TemporalAccessor_Fields.Null && trans.Gap)
				{
					// error message says daylight savings for simplicity
					// even though there are other kinds of gaps
					throw new DateTimeException("LocalDateTime '" + localDateTime + "' does not exist in zone '" + zone + "' due to a gap in the local time-line, typically caused by daylight savings");
				}
				throw new DateTimeException("ZoneOffset '" + offset + "' is not valid for LocalDateTime '" + localDateTime + "' in zone '" + zone + "'");
			}
			return new ZonedDateTime(localDateTime, offset, zone);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} leniently, for advanced use cases,
		/// allowing any combination of local date-time, offset and zone ID.
		/// <para>
		/// This creates a zoned date-time with no checks other than no nulls.
		/// This means that the resulting zoned date-time may have an offset that is in conflict
		/// with the zone ID.
		/// </para>
		/// <para>
		/// This method is intended for advanced use cases.
		/// For example, consider the case where a zoned date-time with valid fields is created
		/// and then stored in a database or serialization-based store. At some later point,
		/// the object is then re-loaded. However, between those points in time, the government
		/// that defined the time-zone has changed the rules, such that the originally stored
		/// local date-time now does not occur. This method can be used to create the object
		/// in an "invalid" state, despite the change in rules.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		private static ZonedDateTime OfLenient(LocalDateTime localDateTime, ZoneOffset offset, ZoneId zone)
		{
			Objects.RequireNonNull(localDateTime, "localDateTime");
			Objects.RequireNonNull(offset, "offset");
			Objects.RequireNonNull(zone, "zone");
			if (zone is ZoneOffset && offset.Equals(zone) == false)
			{
				throw new IllegalArgumentException("ZoneId must match ZoneOffset");
			}
			return new ZonedDateTime(localDateTime, offset, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a temporal object.
		/// <para>
		/// This obtains a zoned date-time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code ZonedDateTime}.
		/// </para>
		/// <para>
		/// The conversion will first obtain a {@code ZoneId} from the temporal object,
		/// falling back to a {@code ZoneOffset} if necessary. It will then try to obtain
		/// an {@code Instant}, falling back to a {@code LocalDateTime} if necessary.
		/// The result will be either the combination of {@code ZoneId} or {@code ZoneOffset}
		/// with {@code Instant} or {@code LocalDateTime}.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code ZonedDateTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to an {@code ZonedDateTime} </exception>
		public static ZonedDateTime From(TemporalAccessor temporal)
		{
			if (temporal is ZonedDateTime)
			{
				return (ZonedDateTime) temporal;
			}
			try
			{
				ZoneId zone = ZoneId.From(temporal);
				if (temporal.IsSupported(INSTANT_SECONDS))
				{
					long epochSecond = temporal.GetLong(INSTANT_SECONDS);
					int nanoOfSecond = temporal.get(NANO_OF_SECOND);
					return Create(epochSecond, nanoOfSecond, zone);
				}
				else
				{
					LocalDate date = LocalDate.From(temporal);
					LocalTime time = LocalTime.From(temporal);
					return Of(date, time, zone);
				}
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain ZonedDateTime from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a text string such as
		/// {@code 2007-12-03T10:15:30+01:00[Europe/Paris]}.
		/// <para>
		/// The string must represent a valid date-time and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_ZONED_DATE_TIME"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007-12-03T10:15:30+01:00[Europe/Paris]", not null </param>
		/// <returns> the parsed zoned date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static ZonedDateTime Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_ZONED_DATE_TIME);
		}

		/// <summary>
		/// Obtains an instance of {@code ZonedDateTime} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a date-time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed zoned date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static ZonedDateTime Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, ZonedDateTime::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dateTime">  the date-time, validated as not null </param>
		/// <param name="offset">  the zone offset, validated as not null </param>
		/// <param name="zone">  the time-zone, validated as not null </param>
		private ZonedDateTime(LocalDateTime dateTime, ZoneOffset offset, ZoneId zone)
		{
			this.DateTime = dateTime;
			this.Offset_Renamed = offset;
			this.Zone_Renamed = zone;
		}

		/// <summary>
		/// Resolves the new local date-time using this zone ID, retaining the offset if possible.
		/// </summary>
		/// <param name="newDateTime">  the new local date-time, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		private ZonedDateTime ResolveLocal(LocalDateTime newDateTime)
		{
			return OfLocal(newDateTime, Zone_Renamed, Offset_Renamed);
		}

		/// <summary>
		/// Resolves the new local date-time using the offset to identify the instant.
		/// </summary>
		/// <param name="newDateTime">  the new local date-time, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		private ZonedDateTime ResolveInstant(LocalDateTime newDateTime)
		{
			return OfInstant(newDateTime, Offset_Renamed, Zone_Renamed);
		}

		/// <summary>
		/// Resolves the offset into this zoned date-time for the with methods.
		/// <para>
		/// This typically ignores the offset, unless it can be used to switch offset in a DST overlap.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		private ZonedDateTime ResolveOffset(ZoneOffset offset)
		{
			if (offset.Equals(this.Offset_Renamed) == false && Zone_Renamed.Rules.IsValidOffset(DateTime, offset))
			{
				return new ZonedDateTime(DateTime, offset, Zone_Renamed);
			}
			return this;
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
			return ChronoZonedDateTime.this.isSupported(unit);
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
		public override int Get(TemporalField field) // override for Javadoc and performance
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
			return ChronoZonedDateTime.this.get(field);
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
						return toEpochSecond();
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
		public override ZoneOffset Offset
		{
			get
			{
				return Offset_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this date-time changing the zone offset to the
		/// earlier of the two valid offsets at a local time-line overlap.
		/// <para>
		/// This method only has any effect when the local time-line overlaps, such as
		/// at an autumn daylight savings cutover. In this scenario, there are two
		/// valid offsets for the local date-time. Calling this method will return
		/// a zoned date-time with the earlier of the two selected.
		/// </para>
		/// <para>
		/// If this method is called when it is not an overlap, {@code this}
		/// is returned.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the earlier offset, not null </returns>
		public override ZonedDateTime WithEarlierOffsetAtOverlap()
		{
			ZoneOffsetTransition trans = Zone.Rules.GetTransition(DateTime);
			if (trans != temporal.TemporalAccessor_Fields.Null && trans.Overlap)
			{
				ZoneOffset earlierOffset = trans.OffsetBefore;
				if (earlierOffset.Equals(Offset_Renamed) == false)
				{
					return new ZonedDateTime(DateTime, earlierOffset, Zone_Renamed);
				}
			}
			return this;
		}

		/// <summary>
		/// Returns a copy of this date-time changing the zone offset to the
		/// later of the two valid offsets at a local time-line overlap.
		/// <para>
		/// This method only has any effect when the local time-line overlaps, such as
		/// at an autumn daylight savings cutover. In this scenario, there are two
		/// valid offsets for the local date-time. Calling this method will return
		/// a zoned date-time with the later of the two selected.
		/// </para>
		/// <para>
		/// If this method is called when it is not an overlap, {@code this}
		/// is returned.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the later offset, not null </returns>
		public override ZonedDateTime WithLaterOffsetAtOverlap()
		{
			ZoneOffsetTransition trans = Zone.Rules.GetTransition(ToLocalDateTime());
			if (trans != temporal.TemporalAccessor_Fields.Null)
			{
				ZoneOffset laterOffset = trans.OffsetAfter;
				if (laterOffset.Equals(Offset_Renamed) == false)
				{
					return new ZonedDateTime(DateTime, laterOffset, Zone_Renamed);
				}
			}
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the time-zone, such as 'Europe/Paris'.
		/// <para>
		/// This returns the zone ID. This identifies the time-zone <seealso cref="ZoneRules rules"/>
		/// that determine when and how the offset from UTC/Greenwich changes.
		/// </para>
		/// <para>
		/// The zone ID may be same as the <seealso cref="#getOffset() offset"/>.
		/// If this is true, then any future calculations, such as addition or subtraction,
		/// have no complex edge cases due to time-zone rules.
		/// See also <seealso cref="#withFixedOffsetZone()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time-zone, not null </returns>
		public override ZoneId Zone
		{
			get
			{
				return Zone_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this date-time with a different time-zone,
		/// retaining the local date-time if possible.
		/// <para>
		/// This method changes the time-zone and retains the local date-time.
		/// The local date-time is only changed if it is invalid for the new zone,
		/// determined using the same approach as
		/// <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>.
		/// </para>
		/// <para>
		/// To change the zone and adjust the local date-time,
		/// use <seealso cref="#withZoneSameInstant(ZoneId)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to change to, not null </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested zone, not null </returns>
		public override ZonedDateTime WithZoneSameLocal(ZoneId zone)
		{
			Objects.RequireNonNull(zone, "zone");
			return this.Zone_Renamed.Equals(zone) ? this : OfLocal(DateTime, zone, Offset_Renamed);
		}

		/// <summary>
		/// Returns a copy of this date-time with a different time-zone,
		/// retaining the instant.
		/// <para>
		/// This method changes the time-zone and retains the instant.
		/// This normally results in a change to the local date-time.
		/// </para>
		/// <para>
		/// This method is based on retaining the same instant, thus gaps and overlaps
		/// in the local time-line have no effect on the result.
		/// </para>
		/// <para>
		/// To change the offset while keeping the local time,
		/// use <seealso cref="#withZoneSameLocal(ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to change to, not null </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested zone, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public override ZonedDateTime WithZoneSameInstant(ZoneId zone)
		{
			Objects.RequireNonNull(zone, "zone");
			return this.Zone_Renamed.Equals(zone) ? this : Create(DateTime.toEpochSecond(Offset_Renamed), DateTime.Nano, zone);
		}

		/// <summary>
		/// Returns a copy of this date-time with the zone ID set to the offset.
		/// <para>
		/// This returns a zoned date-time where the zone ID is the same as <seealso cref="#getOffset()"/>.
		/// The local date-time, offset and instant of the result will be the same as in this date-time.
		/// </para>
		/// <para>
		/// Setting the date-time to a fixed single offset means that any future
		/// calculations, such as addition or subtraction, have no complex edge cases
		/// due to time-zone rules.
		/// This might also be useful when sending a zoned date-time across a network,
		/// as most protocols, such as ISO-8601, only handle offsets,
		/// and not region-based zone IDs.
		/// </para>
		/// <para>
		/// This is equivalent to {@code ZonedDateTime.of(zdt.toLocalDateTime(), zdt.getOffset())}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ZonedDateTime} with the zone ID set to the offset, not null </returns>
		public ZonedDateTime WithFixedOffsetZone()
		{
			return this.Zone_Renamed.Equals(Offset_Renamed) ? this : new ZonedDateTime(DateTime, Offset_Renamed, Offset_Renamed);
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
		public override LocalDateTime ToLocalDateTime() // override for return type
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
		public override LocalDate ToLocalDate() // override for return type
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
		public override LocalTime ToLocalTime() // override for Javadoc and performance
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
		/// This returns a {@code ZonedDateTime}, based on this one, with the date-time adjusted.
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
		///  result = zonedDateTime.with(JULY).with(lastDayOfMonth());
		/// </pre>
		/// </para>
		/// <para>
		/// The classes <seealso cref="LocalDate"/> and <seealso cref="LocalTime"/> implement {@code TemporalAdjuster},
		/// thus this method can be used to change the date, time or offset:
		/// <pre>
		///  result = zonedDateTime.with(date);
		///  result = zonedDateTime.with(time);
		/// </pre>
		/// </para>
		/// <para>
		/// <seealso cref="ZoneOffset"/> also implements {@code TemporalAdjuster} however using it
		/// as an argument typically has no effect. The offset of a {@code ZonedDateTime} is
		/// controlled primarily by the time-zone. As such, changing the offset does not generally
		/// make sense, because there is only one valid offset for the local date-time and zone.
		/// If the zoned date-time is in a daylight savings overlap, then the offset is used
		/// to switch between the two valid offsets. In all other cases, the offset is ignored.
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
		/// <returns> a {@code ZonedDateTime} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override ZonedDateTime With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalDate)
			{
				return ResolveLocal(LocalDateTime.Of((LocalDate) adjuster, DateTime.ToLocalTime()));
			}
			else if (adjuster is LocalTime)
			{
				return ResolveLocal(LocalDateTime.Of(DateTime.ToLocalDate(), (LocalTime) adjuster));
			}
			else if (adjuster is LocalDateTime)
			{
				return ResolveLocal((LocalDateTime) adjuster);
			}
			else if (adjuster is OffsetDateTime)
			{
				OffsetDateTime odt = (OffsetDateTime) adjuster;
				return OfLocal(odt.ToLocalDateTime(), Zone_Renamed, odt.Offset);
			}
			else if (adjuster is Instant)
			{
				Instant instant = (Instant) adjuster;
				return Create(instant.EpochSecond, instant.Nano, Zone_Renamed);
			}
			else if (adjuster is ZoneOffset)
			{
				return ResolveOffset((ZoneOffset) adjuster);
			}
			return (ZonedDateTime) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified field set to a new value.
		/// <para>
		/// This returns a {@code ZonedDateTime}, based on this one, with the value
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
		/// The zone and nano-of-second are unchanged.
		/// The result will have an offset derived from the new instant and original zone.
		/// If the new instant value is outside the valid range then a {@code DateTimeException} will be thrown.
		/// </para>
		/// <para>
		/// The {@code OFFSET_SECONDS} field will typically be ignored.
		/// The offset of a {@code ZonedDateTime} is controlled primarily by the time-zone.
		/// As such, changing the offset does not generally make sense, because there is only
		/// one valid offset for the local date-time and zone.
		/// If the zoned date-time is in a daylight savings overlap, then the offset is used
		/// to switch between the two valid offsets. In all other cases, the offset is ignored.
		/// If the new offset value is outside the valid range then a {@code DateTimeException} will be thrown.
		/// </para>
		/// <para>
		/// The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		/// the matching method on <seealso cref="LocalDateTime#with(TemporalField, long) LocalDateTime"/>.
		/// The zone is not part of the calculation and will be unchanged.
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
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
		/// <returns> a {@code ZonedDateTime} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public ZonedDateTime With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				switch (f)
				{
					case INSTANT_SECONDS:
						return Create(newValue, Nano, Zone_Renamed);
					case OFFSET_SECONDS:
						ZoneOffset offset = ZoneOffset.OfTotalSeconds(f.checkValidIntValue(newValue));
						return ResolveOffset(offset);
				}
				return ResolveLocal(DateTime.With(field, newValue));
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the year altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withYear(int) changing the year"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested year, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public ZonedDateTime WithYear(int year)
		{
			return ResolveLocal(DateTime.WithYear(year));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the month-of-year altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withMonth(int) changing the month"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public ZonedDateTime WithMonth(int month)
		{
			return ResolveLocal(DateTime.WithMonth(month));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the day-of-month altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withDayOfMonth(int) changing the day-of-month"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month value is invalid,
		///  or if the day-of-month is invalid for the month-year </exception>
		public ZonedDateTime WithDayOfMonth(int dayOfMonth)
		{
			return ResolveLocal(DateTime.WithDayOfMonth(dayOfMonth));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the day-of-year altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withDayOfYear(int) changing the day-of-year"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		/// <returns> a {@code ZonedDateTime} based on this date with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-year value is invalid,
		///  or if the day-of-year is invalid for the year </exception>
		public ZonedDateTime WithDayOfYear(int dayOfYear)
		{
			return ResolveLocal(DateTime.WithDayOfYear(dayOfYear));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the hour-of-day altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withHour(int) changing the time"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested hour, not null </returns>
		/// <exception cref="DateTimeException"> if the hour value is invalid </exception>
		public ZonedDateTime WithHour(int hour)
		{
			return ResolveLocal(DateTime.WithHour(hour));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the minute-of-hour altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withMinute(int) changing the time"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested minute, not null </returns>
		/// <exception cref="DateTimeException"> if the minute value is invalid </exception>
		public ZonedDateTime WithMinute(int minute)
		{
			return ResolveLocal(DateTime.WithMinute(minute));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the second-of-minute altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withSecond(int) changing the time"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested second, not null </returns>
		/// <exception cref="DateTimeException"> if the second value is invalid </exception>
		public ZonedDateTime WithSecond(int second)
		{
			return ResolveLocal(DateTime.WithSecond(second));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the nano-of-second altered.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#withNano(int) changing the time"/> of the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the requested nanosecond, not null </returns>
		/// <exception cref="DateTimeException"> if the nano value is invalid </exception>
		public ZonedDateTime WithNano(int nanoOfSecond)
		{
			return ResolveLocal(DateTime.WithNano(nanoOfSecond));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the time truncated.
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
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#truncatedTo(TemporalUnit) truncating"/>
		/// the underlying local date-time. This is then converted back to a
		/// {@code ZonedDateTime}, using the zone ID to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to truncate to, not null </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if unable to truncate </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public ZonedDateTime TruncatedTo(TemporalUnit unit)
		{
			return ResolveLocal(DateTime.TruncatedTo(unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns a {@code ZonedDateTime}, based on this one, with the specified amount added.
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
		/// <returns> a {@code ZonedDateTime} based on this date-time with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override ZonedDateTime Plus(TemporalAmount amountToAdd)
		{
			if (amountToAdd is Period)
			{
				Period periodToAdd = (Period) amountToAdd;
				return ResolveLocal(DateTime.Plus(periodToAdd));
			}
			Objects.RequireNonNull(amountToAdd, "amountToAdd");
			return (ZonedDateTime) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount added.
		/// <para>
		/// This returns a {@code ZonedDateTime}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The zone is not part of the calculation and will be unchanged in the result.
		/// The calculation for date and time units differ.
		/// </para>
		/// <para>
		/// Date units operate on the local time-line.
		/// The period is first added to the local date-time, then converted back
		/// to a zoned date-time using the zone ID.
		/// The conversion uses <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		/// with the offset before the addition.
		/// </para>
		/// <para>
		/// Time units operate on the instant time-line.
		/// The period is first added to the local date-time, then converted back to
		/// a zoned date-time using the zone ID.
		/// The conversion uses <seealso cref="#ofInstant(LocalDateTime, ZoneOffset, ZoneId)"/>
		/// with the offset before the addition.
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
		/// <returns> a {@code ZonedDateTime} based on this date-time with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public ZonedDateTime Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				if (unit.DateBased)
				{
					return ResolveLocal(DateTime.Plus(amountToAdd, unit));
				}
				else
				{
					return ResolveInstant(DateTime.Plus(amountToAdd, unit));
				}
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of years added.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#plusYears(long) adding years"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the years to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusYears(long years)
		{
			return ResolveLocal(DateTime.PlusYears(years));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of months added.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#plusMonths(long) adding months"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusMonths(long months)
		{
			return ResolveLocal(DateTime.PlusMonths(months));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of weeks added.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#plusWeeks(long) adding weeks"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the weeks to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the weeks added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusWeeks(long weeks)
		{
			return ResolveLocal(DateTime.PlusWeeks(weeks));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of days added.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#plusDays(long) adding days"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the days added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusDays(long days)
		{
			return ResolveLocal(DateTime.PlusDays(days));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of hours added.
		/// <para>
		/// This operates on the instant time-line, such that adding one hour will
		/// always be a duration of one hour later.
		/// This may cause the local date-time to change by an amount other than one hour.
		/// Note that this is a different approach to that used by days, months and years,
		/// thus adding one day is not the same as adding 24 hours.
		/// </para>
		/// <para>
		/// For example, consider a time-zone where the spring DST cutover means that the
		/// local times 01:00 to 01:59 occur twice changing from offset +02:00 to +01:00.
		/// <ul>
		/// <li>Adding one hour to 00:30+02:00 will result in 01:30+02:00
		/// <li>Adding one hour to 01:30+02:00 will result in 01:30+01:00
		/// <li>Adding one hour to 01:30+01:00 will result in 02:30+01:00
		/// <li>Adding three hours to 00:30+02:00 will result in 02:30+01:00
		/// </ul>
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the hours added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusHours(long hours)
		{
			return ResolveInstant(DateTime.PlusHours(hours));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of minutes added.
		/// <para>
		/// This operates on the instant time-line, such that adding one minute will
		/// always be a duration of one minute later.
		/// This may cause the local date-time to change by an amount other than one minute.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the minutes added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusMinutes(long minutes)
		{
			return ResolveInstant(DateTime.PlusMinutes(minutes));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of seconds added.
		/// <para>
		/// This operates on the instant time-line, such that adding one second will
		/// always be a duration of one second later.
		/// This may cause the local date-time to change by an amount other than one second.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the seconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusSeconds(long seconds)
		{
			return ResolveInstant(DateTime.PlusSeconds(seconds));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of nanoseconds added.
		/// <para>
		/// This operates on the instant time-line, such that adding one nano will
		/// always be a duration of one nano later.
		/// This may cause the local date-time to change by an amount other than one nano.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to add, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the nanoseconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime PlusNanos(long nanos)
		{
			return ResolveInstant(DateTime.PlusNanos(nanos));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code ZonedDateTime}, based on this one, with the specified amount subtracted.
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
		/// <returns> a {@code ZonedDateTime} based on this date-time with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override ZonedDateTime Minus(TemporalAmount amountToSubtract)
		{
			if (amountToSubtract is Period)
			{
				Period periodToSubtract = (Period) amountToSubtract;
				return ResolveLocal(DateTime.Minus(periodToSubtract));
			}
			Objects.RequireNonNull(amountToSubtract, "amountToSubtract");
			return (ZonedDateTime) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this date-time with the specified amount subtracted.
		/// <para>
		/// This returns a {@code ZonedDateTime}, based on this one, with the amount
		/// in terms of the unit subtracted. If it is not possible to subtract the amount,
		/// because the unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// The calculation for date and time units differ.
		/// </para>
		/// <para>
		/// Date units operate on the local time-line.
		/// The period is first subtracted from the local date-time, then converted back
		/// to a zoned date-time using the zone ID.
		/// The conversion uses <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		/// with the offset before the subtraction.
		/// </para>
		/// <para>
		/// Time units operate on the instant time-line.
		/// The period is first subtracted from the local date-time, then converted back to
		/// a zoned date-time using the zone ID.
		/// The conversion uses <seealso cref="#ofInstant(LocalDateTime, ZoneOffset, ZoneId)"/>
		/// with the offset before the subtraction.
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
		/// <returns> a {@code ZonedDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override ZonedDateTime Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of years subtracted.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#minusYears(long) subtracting years"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the years to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusYears(long years)
		{
			return (years == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-years));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of months subtracted.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#minusMonths(long) subtracting months"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusMonths(long months)
		{
			return (months == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-months));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of weeks subtracted.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#minusWeeks(long) subtracting weeks"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the weeks to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the weeks subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusWeeks(long weeks)
		{
			return (weeks == Long.MinValue ? PlusWeeks(Long.MaxValue).PlusWeeks(1) : PlusWeeks(-weeks));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of days subtracted.
		/// <para>
		/// This operates on the local time-line,
		/// <seealso cref="LocalDateTime#minusDays(long) subtracting days"/> to the local date-time.
		/// This is then converted back to a {@code ZonedDateTime}, using the zone ID
		/// to obtain the offset.
		/// </para>
		/// <para>
		/// When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		/// then the offset will be retained if possible, otherwise the earlier offset will be used.
		/// If in a gap, the local date-time will be adjusted forward by the length of the gap.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the days subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusDays(long days)
		{
			return (days == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-days));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of hours subtracted.
		/// <para>
		/// This operates on the instant time-line, such that subtracting one hour will
		/// always be a duration of one hour earlier.
		/// This may cause the local date-time to change by an amount other than one hour.
		/// Note that this is a different approach to that used by days, months and years,
		/// thus subtracting one day is not the same as adding 24 hours.
		/// </para>
		/// <para>
		/// For example, consider a time-zone where the spring DST cutover means that the
		/// local times 01:00 to 01:59 occur twice changing from offset +02:00 to +01:00.
		/// <ul>
		/// <li>Subtracting one hour from 02:30+01:00 will result in 01:30+02:00
		/// <li>Subtracting one hour from 01:30+01:00 will result in 01:30+02:00
		/// <li>Subtracting one hour from 01:30+02:00 will result in 00:30+01:00
		/// <li>Subtracting three hours from 02:30+01:00 will result in 00:30+02:00
		/// </ul>
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the hours to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the hours subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusHours(long hours)
		{
			return (hours == Long.MinValue ? PlusHours(Long.MaxValue).PlusHours(1) : PlusHours(-hours));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of minutes subtracted.
		/// <para>
		/// This operates on the instant time-line, such that subtracting one minute will
		/// always be a duration of one minute earlier.
		/// This may cause the local date-time to change by an amount other than one minute.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the minutes to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the minutes subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusMinutes(long minutes)
		{
			return (minutes == Long.MinValue ? PlusMinutes(Long.MaxValue).PlusMinutes(1) : PlusMinutes(-minutes));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of seconds subtracted.
		/// <para>
		/// This operates on the instant time-line, such that subtracting one second will
		/// always be a duration of one second earlier.
		/// This may cause the local date-time to change by an amount other than one second.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the seconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusSeconds(long seconds)
		{
			return (seconds == Long.MinValue ? PlusSeconds(Long.MaxValue).PlusSeconds(1) : PlusSeconds(-seconds));
		}

		/// <summary>
		/// Returns a copy of this {@code ZonedDateTime} with the specified number of nanoseconds subtracted.
		/// <para>
		/// This operates on the instant time-line, such that subtracting one nano will
		/// always be a duration of one nano earlier.
		/// This may cause the local date-time to change by an amount other than one nano.
		/// Note that this is a different approach to that used by days, months and years.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the nanos to subtract, may be negative </param>
		/// <returns> a {@code ZonedDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public ZonedDateTime MinusNanos(long nanos)
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
		public override R query<R>(TemporalQuery<R> query) // override for Javadoc
		{
			if (query == TemporalQueries.LocalDate())
			{
				return (R) ToLocalDate();
			}
			return ChronoZonedDateTime.this.query(query);
		}

		/// <summary>
		/// Calculates the amount of time until another date-time in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code ZonedDateTime}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified date-time.
		/// The result will be negative if the end is before the start.
		/// For example, the amount in days between two date-times can be calculated
		/// using {@code startDateTime.until(endDateTime, DAYS)}.
		/// </para>
		/// <para>
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code ZonedDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		/// If the time-zone differs between the two zoned date-times, the specified
		/// end date-time is normalized to have the same zone as this date-time.
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
		/// The calculation for date and time units differ.
		/// </para>
		/// <para>
		/// Date units operate on the local time-line, using the local date-time.
		/// For example, the period from noon on day 1 to noon the following day
		/// in days will always be counted as exactly one day, irrespective of whether
		/// there was a daylight savings change or not.
		/// </para>
		/// <para>
		/// Time units operate on the instant time-line.
		/// The calculation effectively converts both zoned date-times to instants
		/// and then calculates the period between the instants.
		/// For example, the period from noon on day 1 to noon the following day
		/// in hours may be 23, 24 or 25 hours (or some other amount) depending on
		/// whether there was a daylight savings change or not.
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to a {@code ZonedDateTime}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this date-time and the end date-time </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code ZonedDateTime} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			ZonedDateTime end = ZonedDateTime.From(endExclusive);
			if (unit is ChronoUnit)
			{
				end = end.WithZoneSameInstant(Zone_Renamed);
				if (unit.DateBased)
				{
					return DateTime.Until(end.DateTime, unit);
				}
				else
				{
					return ToOffsetDateTime().Until(end.ToOffsetDateTime(), unit);
				}
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
		/// Converts this date-time to an {@code OffsetDateTime}.
		/// <para>
		/// This creates an offset date-time using the local date-time and offset.
		/// The zone ID is ignored.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an offset date-time representing the same local date-time and offset, not null </returns>
		public OffsetDateTime ToOffsetDateTime()
		{
			return OffsetDateTime.Of(DateTime, Offset_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this date-time is equal to another date-time.
		/// <para>
		/// The comparison is based on the offset date-time and the zone.
		/// Only objects of type {@code ZonedDateTime} are compared, other types return false.
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
			if (obj is ZonedDateTime)
			{
				ZonedDateTime other = (ZonedDateTime) obj;
				return DateTime.Equals(other.DateTime) && Offset_Renamed.Equals(other.Offset_Renamed) && Zone_Renamed.Equals(other.Zone_Renamed);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date-time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return DateTime.HashCode() ^ Offset_Renamed.HashCode() ^ Integer.RotateLeft(Zone_Renamed.HashCode(), 3);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this date-time as a {@code String}, such as
		/// {@code 2007-12-03T10:15:30+01:00[Europe/Paris]}.
		/// <para>
		/// The format consists of the {@code LocalDateTime} followed by the {@code ZoneOffset}.
		/// If the {@code ZoneId} is not the same as the offset, then the ID is output.
		/// The output is compatible with ISO-8601 if the offset and ID are the same.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date-time, not null </returns>
		public override String ToString() // override for Javadoc
		{
			String str = DateTime.ToString() + Offset_Renamed.ToString();
			if (Offset_Renamed != Zone_Renamed)
			{
				str += '[' + Zone_Renamed.ToString() + ']';
			}
			return str;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(6);  // identifies a ZonedDateTime
		///  // the <a href="../../serialized-form.html#java.time.LocalDateTime">dateTime</a> excluding the one byte header
		///  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		///  // the <a href="../../serialized-form.html#java.time.ZoneId">zone ID</a> excluding the one byte header
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZONE_DATE_TIME_TYPE, this);
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
			DateTime.WriteExternal(@out);
			Offset_Renamed.WriteExternal(@out);
			Zone_Renamed.Write(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZonedDateTime readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		internal static ZonedDateTime ReadExternal(ObjectInput @in)
		{
			LocalDateTime dateTime = LocalDateTime.ReadExternal(@in);
			ZoneOffset offset = ZoneOffset.ReadExternal(@in);
			ZoneId zone = (ZoneId) Ser.Read(@in);
			return ZonedDateTime.OfLenient(dateTime, offset, zone);
		}

	}

}