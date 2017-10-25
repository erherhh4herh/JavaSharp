using System;
using System.Collections.Concurrent;

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
	/// A time-zone offset from Greenwich/UTC, such as {@code +02:00}.
	/// <para>
	/// A time-zone offset is the amount of time that a time-zone differs from Greenwich/UTC.
	/// This is usually a fixed number of hours and minutes.
	/// </para>
	/// <para>
	/// Different parts of the world have different time-zone offsets.
	/// The rules for how offsets vary by place and time of year are captured in the
	/// <seealso cref="ZoneId"/> class.
	/// </para>
	/// <para>
	/// For example, Paris is one hour ahead of Greenwich/UTC in winter and two hours
	/// ahead in summer. The {@code ZoneId} instance for Paris will reference two
	/// {@code ZoneOffset} instances - a {@code +01:00} instance for winter,
	/// and a {@code +02:00} instance for summer.
	/// </para>
	/// <para>
	/// In 2008, time-zone offsets around the world extended from -12:00 to +14:00.
	/// To prevent any problems with that range being extended, yet still provide
	/// validation, the range of offsets is restricted to -18:00 to 18:00 inclusive.
	/// </para>
	/// <para>
	/// This class is designed for use with the ISO calendar system.
	/// The fields of hours, minutes and seconds make assumptions that are valid for the
	/// standard ISO definitions of those fields. This class may be used with other
	/// calendar systems providing the definition of the time fields matches those
	/// of the ISO calendar system.
	/// </para>
	/// <para>
	/// Instances of {@code ZoneOffset} must be compared using <seealso cref="#equals"/>.
	/// Implementations may choose to cache certain common offsets, however
	/// applications must not rely on such caching.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code ZoneOffset} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ZoneOffset : ZoneId, TemporalAccessor, TemporalAdjuster, Comparable<ZoneOffset>
	{

		/// <summary>
		/// Cache of time-zone offset by offset in seconds. </summary>
		private static readonly ConcurrentMap<Integer, ZoneOffset> SECONDS_CACHE = new ConcurrentDictionary<Integer, ZoneOffset>(16, 0.75f, 4);
		/// <summary>
		/// Cache of time-zone offset by ID. </summary>
		private static readonly ConcurrentMap<String, ZoneOffset> ID_CACHE = new ConcurrentDictionary<String, ZoneOffset>(16, 0.75f, 4);

		/// <summary>
		/// The abs maximum seconds.
		/// </summary>
		private static readonly int MAX_SECONDS = 18 * SECONDS_PER_HOUR;
		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 2357656521762053153L;

		/// <summary>
		/// The time-zone offset for UTC, with an ID of 'Z'.
		/// </summary>
		public static readonly ZoneOffset UTC = ZoneOffset.OfTotalSeconds(0);
		/// <summary>
		/// Constant for the maximum supported offset.
		/// </summary>
		public static readonly ZoneOffset MIN = ZoneOffset.OfTotalSeconds(-MAX_SECONDS);
		/// <summary>
		/// Constant for the maximum supported offset.
		/// </summary>
		public static readonly ZoneOffset MAX = ZoneOffset.OfTotalSeconds(MAX_SECONDS);

		/// <summary>
		/// The total offset in seconds.
		/// </summary>
		private readonly int TotalSeconds_Renamed;
		/// <summary>
		/// The string form of the time-zone offset.
		/// </summary>
		[NonSerialized]
		private readonly String Id_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} using the ID.
		/// <para>
		/// This method parses the string ID of a {@code ZoneOffset} to
		/// return an instance. The parsing accepts all the formats generated by
		/// <seealso cref="#getId()"/>, plus some additional formats:
		/// <ul>
		/// <li>{@code Z} - for UTC
		/// <li>{@code +h}
		/// <li>{@code +hh}
		/// <li>{@code +hh:mm}
		/// <li>{@code -hh:mm}
		/// <li>{@code +hhmm}
		/// <li>{@code -hhmm}
		/// <li>{@code +hh:mm:ss}
		/// <li>{@code -hh:mm:ss}
		/// <li>{@code +hhmmss}
		/// <li>{@code -hhmmss}
		/// </ul>
		/// Note that &plusmn; means either the plus or minus symbol.
		/// </para>
		/// <para>
		/// The ID of the returned offset will be normalized to one of the formats
		/// described by <seealso cref="#getId()"/>.
		/// </para>
		/// <para>
		/// The maximum supported range is from +18:00 to -18:00 inclusive.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offsetId">  the offset ID, not null </param>
		/// <returns> the zone-offset, not null </returns>
		/// <exception cref="DateTimeException"> if the offset ID is invalid </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public static ZoneOffset of(String offsetId)
		public static ZoneOffset Of(String offsetId)
		{
			Objects.RequireNonNull(offsetId, "offsetId");
			// "Z" is always in the cache
			ZoneOffset offset = ID_CACHE[offsetId];
			if (offset != temporal.TemporalAccessor_Fields.Null)
			{
				return offset;
			}

			// parse - +h, +hh, +hhmm, +hh:mm, +hhmmss, +hh:mm:ss
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int hours, minutes, seconds;
			int hours, minutes, seconds;
			switch (offsetId.Length())
			{
				case 2:
					offsetId = offsetId.CharAt(0) + "0" + offsetId.CharAt(1); // fallthru
					goto case 3;
				case 3:
					hours = ParseNumber(offsetId, 1, false);
					minutes = 0;
					seconds = 0;
					break;
				case 5:
					hours = ParseNumber(offsetId, 1, false);
					minutes = ParseNumber(offsetId, 3, false);
					seconds = 0;
					break;
				case 6:
					hours = ParseNumber(offsetId, 1, false);
					minutes = ParseNumber(offsetId, 4, true);
					seconds = 0;
					break;
				case 7:
					hours = ParseNumber(offsetId, 1, false);
					minutes = ParseNumber(offsetId, 3, false);
					seconds = ParseNumber(offsetId, 5, false);
					break;
				case 9:
					hours = ParseNumber(offsetId, 1, false);
					minutes = ParseNumber(offsetId, 4, true);
					seconds = ParseNumber(offsetId, 7, true);
					break;
				default:
					throw new DateTimeException("Invalid ID for ZoneOffset, invalid format: " + offsetId);
			}
			char first = offsetId.CharAt(0);
			if (first != '+' && first != '-')
			{
				throw new DateTimeException("Invalid ID for ZoneOffset, plus/minus not found when expected: " + offsetId);
			}
			if (first == '-')
			{
				return OfHoursMinutesSeconds(-hours, -minutes, -seconds);
			}
			else
			{
				return OfHoursMinutesSeconds(hours, minutes, seconds);
			}
		}

		/// <summary>
		/// Parse a two digit zero-prefixed number.
		/// </summary>
		/// <param name="offsetId">  the offset ID, not null </param>
		/// <param name="pos">  the position to parse, valid </param>
		/// <param name="precededByColon">  should this number be prefixed by a precededByColon </param>
		/// <returns> the parsed number, from 0 to 99 </returns>
		private static int ParseNumber(CharSequence offsetId, int pos, bool precededByColon)
		{
			if (precededByColon && offsetId.CharAt(pos - 1) != ':')
			{
				throw new DateTimeException("Invalid ID for ZoneOffset, colon not found when expected: " + offsetId);
			}
			char ch1 = offsetId.CharAt(pos);
			char ch2 = offsetId.CharAt(pos + 1);
			if (ch1 < '0' || ch1 > '9' || ch2 < '0' || ch2 > '9')
			{
				throw new DateTimeException("Invalid ID for ZoneOffset, non numeric characters found: " + offsetId);
			}
			return (ch1 - 48) * 10 + (ch2 - 48);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} using an offset in hours.
		/// </summary>
		/// <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		/// <returns> the zone-offset, not null </returns>
		/// <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		public static ZoneOffset OfHours(int hours)
		{
			return OfHoursMinutesSeconds(hours, 0, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} using an offset in
		/// hours and minutes.
		/// <para>
		/// The sign of the hours and minutes components must match.
		/// Thus, if the hours is negative, the minutes must be negative or zero.
		/// If the hours is zero, the minutes may be positive, negative or zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		/// <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours </param>
		/// <returns> the zone-offset, not null </returns>
		/// <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		public static ZoneOffset OfHoursMinutes(int hours, int minutes)
		{
			return OfHoursMinutesSeconds(hours, minutes, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} using an offset in
		/// hours, minutes and seconds.
		/// <para>
		/// The sign of the hours, minutes and seconds components must match.
		/// Thus, if the hours is negative, the minutes and seconds must be negative or zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		/// <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours and seconds </param>
		/// <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59, sign matches hours and minutes </param>
		/// <returns> the zone-offset, not null </returns>
		/// <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		public static ZoneOffset OfHoursMinutesSeconds(int hours, int minutes, int seconds)
		{
			Validate(hours, minutes, seconds);
			int totalSeconds = TotalSeconds(hours, minutes, seconds);
			return OfTotalSeconds(totalSeconds);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} from a temporal object.
		/// <para>
		/// This obtains an offset based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code ZoneOffset}.
		/// </para>
		/// <para>
		/// A {@code TemporalAccessor} represents some form of date and time information.
		/// This factory converts the arbitrary temporal object to an instance of {@code ZoneOffset}.
		/// </para>
		/// <para>
		/// The conversion uses the <seealso cref="TemporalQueries#offset()"/> query, which relies
		/// on extracting the <seealso cref="ChronoField#OFFSET_SECONDS OFFSET_SECONDS"/> field.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code ZoneOffset::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the zone-offset, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to an {@code ZoneOffset} </exception>
		public static ZoneOffset From(TemporalAccessor temporal)
		{
			Objects.RequireNonNull(temporal, "temporal");
			ZoneOffset offset = temporal.query(TemporalQueries.Offset());
			if (offset == temporal.TemporalAccessor_Fields.Null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain ZoneOffset from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName);
			}
			return offset;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Validates the offset fields.
		/// </summary>
		/// <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		/// <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59 </param>
		/// <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59 </param>
		/// <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		private static void Validate(int hours, int minutes, int seconds)
		{
			if (hours < -18 || hours > 18)
			{
				throw new DateTimeException("Zone offset hours not in valid range: value " + hours + " is not in the range -18 to 18");
			}
			if (hours > 0)
			{
				if (minutes < 0 || seconds < 0)
				{
					throw new DateTimeException("Zone offset minutes and seconds must be positive because hours is positive");
				}
			}
			else if (hours < 0)
			{
				if (minutes > 0 || seconds > 0)
				{
					throw new DateTimeException("Zone offset minutes and seconds must be negative because hours is negative");
				}
			}
			else if ((minutes > 0 && seconds < 0) || (minutes < 0 && seconds > 0))
			{
				throw new DateTimeException("Zone offset minutes and seconds must have the same sign");
			}
			if (System.Math.Abs(minutes) > 59)
			{
				throw new DateTimeException("Zone offset minutes not in valid range: abs(value) " + System.Math.Abs(minutes) + " is not in the range 0 to 59");
			}
			if (System.Math.Abs(seconds) > 59)
			{
				throw new DateTimeException("Zone offset seconds not in valid range: abs(value) " + System.Math.Abs(seconds) + " is not in the range 0 to 59");
			}
			if (System.Math.Abs(hours) == 18 && (System.Math.Abs(minutes) > 0 || System.Math.Abs(seconds) > 0))
			{
				throw new DateTimeException("Zone offset not in valid range: -18:00 to +18:00");
			}
		}

		/// <summary>
		/// Calculates the total offset in seconds.
		/// </summary>
		/// <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		/// <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours and seconds </param>
		/// <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59, sign matches hours and minutes </param>
		/// <returns> the total in seconds </returns>
		private static int TotalSeconds(int hours, int minutes, int seconds)
		{
			return hours * SECONDS_PER_HOUR + minutes * SECONDS_PER_MINUTE + seconds;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneOffset} specifying the total offset in seconds
		/// <para>
		/// The offset must be in the range {@code -18:00} to {@code +18:00}, which corresponds to -64800 to +64800.
		/// 
		/// </para>
		/// </summary>
		/// <param name="totalSeconds">  the total time-zone offset in seconds, from -64800 to +64800 </param>
		/// <returns> the ZoneOffset, not null </returns>
		/// <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		public static ZoneOffset OfTotalSeconds(int totalSeconds)
		{
			if (System.Math.Abs(totalSeconds) > MAX_SECONDS)
			{
				throw new DateTimeException("Zone offset not in valid range: -18:00 to +18:00");
			}
			if (totalSeconds % (15 * SECONDS_PER_MINUTE) == 0)
			{
				Integer totalSecs = totalSeconds;
				ZoneOffset result = SECONDS_CACHE[totalSecs];
				if (result == temporal.TemporalAccessor_Fields.Null)
				{
					result = new ZoneOffset(totalSeconds);
					SECONDS_CACHE.PutIfAbsent(totalSecs, result);
					result = SECONDS_CACHE[totalSecs];
					ID_CACHE.PutIfAbsent(result.Id, result);
				}
				return result;
			}
			else
			{
				return new ZoneOffset(totalSeconds);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="totalSeconds">  the total time-zone offset in seconds, from -64800 to +64800 </param>
		private ZoneOffset(int totalSeconds) : base()
		{
			this.TotalSeconds_Renamed = totalSeconds;
			Id_Renamed = BuildId(totalSeconds);
		}

		private static String BuildId(int totalSeconds)
		{
			if (totalSeconds == 0)
			{
				return "Z";
			}
			else
			{
				int absTotalSeconds = System.Math.Abs(totalSeconds);
				StringBuilder buf = new StringBuilder();
				int absHours = absTotalSeconds / SECONDS_PER_HOUR;
				int absMinutes = (absTotalSeconds / SECONDS_PER_MINUTE) % MINUTES_PER_HOUR;
				buf.Append(totalSeconds < 0 ? "-" : "+").Append(absHours < 10 ? "0" : "").Append(absHours).Append(absMinutes < 10 ? ":0" : ":").Append(absMinutes);
				int absSeconds = absTotalSeconds % SECONDS_PER_MINUTE;
				if (absSeconds != 0)
				{
					buf.Append(absSeconds < 10 ? ":0" : ":").Append(absSeconds);
				}
				return buf.ToString();
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the total zone offset in seconds.
		/// <para>
		/// This is the primary way to access the offset amount.
		/// It returns the total of the hours, minutes and seconds fields as a
		/// single offset that can be added to a time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the total zone offset amount in seconds </returns>
		public int TotalSeconds
		{
			get
			{
				return TotalSeconds_Renamed;
			}
		}

		/// <summary>
		/// Gets the normalized zone offset ID.
		/// <para>
		/// The ID is minor variation to the standard ISO-8601 formatted string
		/// for the offset. There are three formats:
		/// <ul>
		/// <li>{@code Z} - for UTC (ISO-8601)
		/// <li>{@code +hh:mm} or {@code -hh:mm} - if the seconds are zero (ISO-8601)
		/// <li>{@code +hh:mm:ss} or {@code -hh:mm:ss} - if the seconds are non-zero (not ISO-8601)
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the zone offset ID, not null </returns>
		public override String Id
		{
			get
			{
				return Id_Renamed;
			}
		}

		/// <summary>
		/// Gets the associated time-zone rules.
		/// <para>
		/// The rules will always return this offset when queried.
		/// The implementation class is immutable, thread-safe and serializable.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the rules, not null </returns>
		public override ZoneRules Rules
		{
			get
			{
				return ZoneRules.Of(this);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this offset can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		/// <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The {@code OFFSET_SECONDS} field returns true.
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
		/// <returns> true if the field is supported on this offset, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field == OFFSET_SECONDS;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This offset is used to enhance the accuracy of the returned range.
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
			return TemporalAccessor.this.range(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this offset as an {@code int}.
		/// <para>
		/// This queries this offset for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The {@code OFFSET_SECONDS} field returns the value of the offset.
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
			if (field == OFFSET_SECONDS)
			{
				return TotalSeconds_Renamed;
			}
			else if (field is ChronoField)
			{
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return temporal.TemporalAccessor_Fields.range(field).checkValidIntValue(GetLong(field), field);
		}

		/// <summary>
		/// Gets the value of the specified field from this offset as a {@code long}.
		/// <para>
		/// This queries this offset for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The {@code OFFSET_SECONDS} field returns the value of the offset.
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
			if (field == OFFSET_SECONDS)
			{
				return TotalSeconds_Renamed;
			}
			else if (field is ChronoField)
			{
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this offset using the specified query.
		/// <para>
		/// This queries this offset using the specified query strategy object.
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
				return (R) this;
			}
			return TemporalAccessor.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have the same offset as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the offset changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#OFFSET_SECONDS"/> as the field.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisOffset.adjustInto(temporal);
		///   temporal = temporal.with(thisOffset);
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
			return temporal.With(OFFSET_SECONDS, TotalSeconds_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this offset to another offset in descending order.
		/// <para>
		/// The offsets are compared in the order that they occur for the same time
		/// of day around the world. Thus, an offset of {@code +10:00} comes before an
		/// offset of {@code +09:00} and so on down to {@code -18:00}.
		/// </para>
		/// <para>
		/// The comparison is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date to compare to, not null </param>
		/// <returns> the comparator value, negative if less, postive if greater </returns>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public int CompareTo(ZoneOffset other)
		{
			return other.TotalSeconds_Renamed - TotalSeconds_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this offset is equal to another offset.
		/// <para>
		/// The comparison is based on the amount of the offset in seconds.
		/// This is equivalent to a comparison by ID.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other offset </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
			   return true;
			}
			if (obj is ZoneOffset)
			{
				return TotalSeconds_Renamed == ((ZoneOffset) obj).TotalSeconds_Renamed;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this offset.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return TotalSeconds_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this offset as a {@code String}, using the normalized ID.
		/// </summary>
		/// <returns> a string representation of this offset, not null </returns>
		public override String ToString()
		{
			return Id_Renamed;
		}

		// -----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(8);                  // identifies a ZoneOffset
		///  int offsetByte = totalSeconds % 900 == 0 ? totalSeconds / 900 : 127;
		///  out.writeByte(offsetByte);
		///  if (offsetByte == 127) {
		///      out.writeInt(totalSeconds);
		///  }
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZONE_OFFSET_TYPE, this);
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
//ORIGINAL LINE: @Override void write(java.io.DataOutput out) throws java.io.IOException
		internal override void Write(DataOutput @out)
		{
			@out.WriteByte(Ser.ZONE_OFFSET_TYPE);
			WriteExternal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offsetSecs = totalSeconds;
			int offsetSecs = TotalSeconds_Renamed;
			int offsetByte = offsetSecs % 900 == 0 ? offsetSecs / 900 : 127; // compress to -72 to +72
			@out.WriteByte(offsetByte);
			if (offsetByte == 127)
			{
				@out.WriteInt(offsetSecs);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZoneOffset readExternal(java.io.DataInput in) throws java.io.IOException
		internal static ZoneOffset ReadExternal(DataInput @in)
		{
			int offsetByte = @in.ReadByte();
			return (offsetByte == 127 ? ZoneOffset.OfTotalSeconds(@in.ReadInt()) : ZoneOffset.OfTotalSeconds(offsetByte * 900));
		}

	}

}