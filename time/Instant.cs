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
	/// An instantaneous point on the time-line.
	/// <para>
	/// This class models a single instantaneous point on the time-line.
	/// This might be used to record event time-stamps in the application.
	/// </para>
	/// <para>
	/// The range of an instant requires the storage of a number larger than a {@code long}.
	/// To achieve this, the class stores a {@code long} representing epoch-seconds and an
	/// {@code int} representing nanosecond-of-second, which will always be between 0 and 999,999,999.
	/// The epoch-seconds are measured from the standard Java epoch of {@code 1970-01-01T00:00:00Z}
	/// where instants after the epoch have positive values, and earlier instants have negative values.
	/// For both the epoch-second and nanosecond parts, a larger value is always later on the time-line
	/// than a smaller value.
	/// 
	/// <h3>Time-scale</h3>
	/// </para>
	/// <para>
	/// The length of the solar day is the standard way that humans measure time.
	/// This has traditionally been subdivided into 24 hours of 60 minutes of 60 seconds,
	/// forming a 86400 second day.
	/// </para>
	/// <para>
	/// Modern timekeeping is based on atomic clocks which precisely define an SI second
	/// relative to the transitions of a Caesium atom. The length of an SI second was defined
	/// to be very close to the 86400th fraction of a day.
	/// </para>
	/// <para>
	/// Unfortunately, as the Earth rotates the length of the day varies.
	/// In addition, over time the average length of the day is getting longer as the Earth slows.
	/// As a result, the length of a solar day in 2012 is slightly longer than 86400 SI seconds.
	/// The actual length of any given day and the amount by which the Earth is slowing
	/// are not predictable and can only be determined by measurement.
	/// The UT1 time-scale captures the accurate length of day, but is only available some
	/// time after the day has completed.
	/// </para>
	/// <para>
	/// The UTC time-scale is a standard approach to bundle up all the additional fractions
	/// of a second from UT1 into whole seconds, known as <i>leap-seconds</i>.
	/// A leap-second may be added or removed depending on the Earth's rotational changes.
	/// As such, UTC permits a day to have 86399 SI seconds or 86401 SI seconds where
	/// necessary in order to keep the day aligned with the Sun.
	/// </para>
	/// <para>
	/// The modern UTC time-scale was introduced in 1972, introducing the concept of whole leap-seconds.
	/// Between 1958 and 1972, the definition of UTC was complex, with minor sub-second leaps and
	/// alterations to the length of the notional second. As of 2012, discussions are underway
	/// to change the definition of UTC again, with the potential to remove leap seconds or
	/// introduce other changes.
	/// </para>
	/// <para>
	/// Given the complexity of accurate timekeeping described above, this Java API defines
	/// its own time-scale, the <i>Java Time-Scale</i>.
	/// </para>
	/// <para>
	/// The Java Time-Scale divides each calendar day into exactly 86400
	/// subdivisions, known as seconds.  These seconds may differ from the
	/// SI second.  It closely matches the de facto international civil time
	/// scale, the definition of which changes from time to time.
	/// </para>
	/// <para>
	/// The Java Time-Scale has slightly different definitions for different
	/// segments of the time-line, each based on the consensus international
	/// time scale that is used as the basis for civil time. Whenever the
	/// internationally-agreed time scale is modified or replaced, a new
	/// segment of the Java Time-Scale must be defined for it.  Each segment
	/// must meet these requirements:
	/// <ul>
	/// <li>the Java Time-Scale shall closely match the underlying international
	///  civil time scale;</li>
	/// <li>the Java Time-Scale shall exactly match the international civil
	///  time scale at noon each day;</li>
	/// <li>the Java Time-Scale shall have a precisely-defined relationship to
	///  the international civil time scale.</li>
	/// </ul>
	/// There are currently, as of 2013, two segments in the Java time-scale.
	/// </para>
	/// <para>
	/// For the segment from 1972-11-03 (exact boundary discussed below) until
	/// further notice, the consensus international time scale is UTC (with
	/// leap seconds).  In this segment, the Java Time-Scale is identical to
	/// <a href="http://www.cl.cam.ac.uk/~mgk25/time/utc-sls/">UTC-SLS</a>.
	/// This is identical to UTC on days that do not have a leap second.
	/// On days that do have a leap second, the leap second is spread equally
	/// over the last 1000 seconds of the day, maintaining the appearance of
	/// exactly 86400 seconds per day.
	/// </para>
	/// <para>
	/// For the segment prior to 1972-11-03, extending back arbitrarily far,
	/// the consensus international time scale is defined to be UT1, applied
	/// proleptically, which is equivalent to the (mean) solar time on the
	/// prime meridian (Greenwich). In this segment, the Java Time-Scale is
	/// identical to the consensus international time scale. The exact
	/// boundary between the two segments is the instant where UT1 = UTC
	/// between 1972-11-03T00:00 and 1972-11-04T12:00.
	/// </para>
	/// <para>
	/// Implementations of the Java time-scale using the JSR-310 API are not
	/// required to provide any clock that is sub-second accurate, or that
	/// progresses monotonically or smoothly. Implementations are therefore
	/// not required to actually perform the UTC-SLS slew or to otherwise be
	/// aware of leap seconds. JSR-310 does, however, require that
	/// implementations must document the approach they use when defining a
	/// clock representing the current instant.
	/// See <seealso cref="Clock"/> for details on the available clocks.
	/// </para>
	/// <para>
	/// The Java time-scale is used for all date-time classes.
	/// This includes {@code Instant}, {@code LocalDate}, {@code LocalTime}, {@code OffsetDateTime},
	/// {@code ZonedDateTime} and {@code Duration}.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code Instant} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Instant : Temporal, TemporalAdjuster, Comparable<Instant>
	{

		/// <summary>
		/// Constant for the 1970-01-01T00:00:00Z epoch instant.
		/// </summary>
		public static readonly Instant EPOCH = new Instant(0, 0);
		/// <summary>
		/// The minimum supported epoch second.
		/// </summary>
		private const long MIN_SECOND = -31557014167219200L;
		/// <summary>
		/// The maximum supported epoch second.
		/// </summary>
		private const long MAX_SECOND = 31556889864403199L;
		/// <summary>
		/// The minimum supported {@code Instant}, '-1000000000-01-01T00:00Z'.
		/// This could be used by an application as a "far past" instant.
		/// <para>
		/// This is one year earlier than the minimum {@code LocalDateTime}.
		/// This provides sufficient values to handle the range of {@code ZoneOffset}
		/// which affect the instant in addition to the local date-time.
		/// The value is also chosen such that the value of the year fits in
		/// an {@code int}.
		/// </para>
		/// </summary>
		public static readonly Instant MIN = Instant.OfEpochSecond(MIN_SECOND, 0);
		/// <summary>
		/// The maximum supported {@code Instant}, '1000000000-12-31T23:59:59.999999999Z'.
		/// This could be used by an application as a "far future" instant.
		/// <para>
		/// This is one year later than the maximum {@code LocalDateTime}.
		/// This provides sufficient values to handle the range of {@code ZoneOffset}
		/// which affect the instant in addition to the local date-time.
		/// The value is also chosen such that the value of the year fits in
		/// an {@code int}.
		/// </para>
		/// </summary>
		public static readonly Instant MAX = Instant.OfEpochSecond(MAX_SECOND, 999999999);

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -665713676816604388L;

		/// <summary>
		/// The number of seconds from the epoch of 1970-01-01T00:00:00Z.
		/// </summary>
		private readonly long Seconds;
		/// <summary>
		/// The number of nanoseconds, later along the time-line, from the seconds field.
		/// This is always positive, and never exceeds 999,999,999.
		/// </summary>
		private readonly int Nanos;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current instant from the system clock.
		/// <para>
		/// This will query the <seealso cref="Clock#systemUTC() system UTC clock"/> to
		/// obtain the current instant.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate time-source for
		/// testing because the clock is effectively hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current instant using the system clock, not null </returns>
		public static Instant Now()
		{
			return Clock.SystemUTC().Instant();
		}

		/// <summary>
		/// Obtains the current instant from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current time.
		/// </para>
		/// <para>
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current instant, not null </returns>
		public static Instant Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
			return clock.Instant();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Instant} using seconds from the
		/// epoch of 1970-01-01T00:00:00Z.
		/// <para>
		/// The nanosecond field is set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		/// <returns> an instant, not null </returns>
		/// <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		public static Instant OfEpochSecond(long epochSecond)
		{
			return Create(epochSecond, 0);
		}

		/// <summary>
		/// Obtains an instance of {@code Instant} using seconds from the
		/// epoch of 1970-01-01T00:00:00Z and nanosecond fraction of second.
		/// <para>
		/// This method allows an arbitrary number of nanoseconds to be passed in.
		/// The factory will alter the values of the second and nanosecond in order
		/// to ensure that the stored nanosecond is in the range 0 to 999,999,999.
		/// For example, the following will result in the exactly the same instant:
		/// <pre>
		///  Instant.ofEpochSecond(3, 1);
		///  Instant.ofEpochSecond(4, -999_999_999);
		///  Instant.ofEpochSecond(2, 1000_000_001);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		/// <param name="nanoAdjustment">  the nanosecond adjustment to the number of seconds, positive or negative </param>
		/// <returns> an instant, not null </returns>
		/// <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public static Instant OfEpochSecond(long epochSecond, long nanoAdjustment)
		{
			long secs = Math.AddExact(epochSecond, Math.FloorDiv(nanoAdjustment, NANOS_PER_SECOND));
			int nos = (int)Math.FloorMod(nanoAdjustment, NANOS_PER_SECOND);
			return Create(secs, nos);
		}

		/// <summary>
		/// Obtains an instance of {@code Instant} using milliseconds from the
		/// epoch of 1970-01-01T00:00:00Z.
		/// <para>
		/// The seconds and nanoseconds are extracted from the specified milliseconds.
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochMilli">  the number of milliseconds from 1970-01-01T00:00:00Z </param>
		/// <returns> an instant, not null </returns>
		/// <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		public static Instant OfEpochMilli(long epochMilli)
		{
			long secs = Math.FloorDiv(epochMilli, 1000);
			int mos = (int)Math.FloorMod(epochMilli, 1000);
			return Create(secs, mos * 1000000);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Instant} from a temporal object.
		/// <para>
		/// This obtains an instant based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code Instant}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		/// and <seealso cref="ChronoField#NANO_OF_SECOND NANO_OF_SECOND"/> fields.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code Instant::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the instant, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to an {@code Instant} </exception>
		public static Instant From(TemporalAccessor temporal)
		{
			if (temporal is Instant)
			{
				return (Instant) temporal;
			}
			Objects.RequireNonNull(temporal, "temporal");
			try
			{
				long instantSecs = temporal.GetLong(INSTANT_SECONDS);
				int nanoOfSecond = temporal.get(NANO_OF_SECOND);
				return Instant.OfEpochSecond(instantSecs, nanoOfSecond);
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain Instant from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Instant} from a text string such as
		/// {@code 2007-12-03T10:15:30.00Z}.
		/// <para>
		/// The string must represent a valid instant in UTC and is parsed using
		/// <seealso cref="DateTimeFormatter#ISO_INSTANT"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <returns> the parsed instant, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Instant parse(final CharSequence text)
		public static Instant Parse(CharSequence text)
		{
			return DateTimeFormatter.ISO_INSTANT.Parse(text, Instant::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Instant} using seconds and nanoseconds.
		/// </summary>
		/// <param name="seconds">  the length of the duration in seconds </param>
		/// <param name="nanoOfSecond">  the nano-of-second, from 0 to 999,999,999 </param>
		/// <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		private static Instant Create(long seconds, int nanoOfSecond)
		{
			if ((seconds | nanoOfSecond) == 0)
			{
				return EPOCH;
			}
			if (seconds < MIN_SECOND || seconds > MAX_SECOND)
			{
				throw new DateTimeException("Instant exceeds minimum or maximum instant");
			}
			return new Instant(seconds, nanoOfSecond);
		}

		/// <summary>
		/// Constructs an instance of {@code Instant} using seconds from the epoch of
		/// 1970-01-01T00:00:00Z and nanosecond fraction of second.
		/// </summary>
		/// <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		/// <param name="nanos">  the nanoseconds within the second, must be positive </param>
		private Instant(long epochSecond, int nanos) : base()
		{
			this.Seconds = epochSecond;
			this.Nanos = nanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this instant can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
		/// <li>{@code NANO_OF_SECOND}
		/// <li>{@code MICRO_OF_SECOND}
		/// <li>{@code MILLI_OF_SECOND}
		/// <li>{@code INSTANT_SECONDS}
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
		/// <returns> true if the field is supported on this instant, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field == INSTANT_SECONDS || field == NANO_OF_SECOND || field == MICRO_OF_SECOND || field == MILLI_OF_SECOND;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
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
		public bool IsSupported(TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				return unit.TimeBased || unit == DAYS;
			}
			return unit != temporal.TemporalAccessor_Fields.Null && unit.isSupportedBy(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This instant is used to enhance the accuracy of the returned range.
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
		/// Gets the value of the specified field from this instant as an {@code int}.
		/// <para>
		/// This queries this instant for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this date-time, except {@code INSTANT_SECONDS} which is too
		/// large to fit in an {@code int} and throws a {@code DateTimeException}.
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
					case NANO_OF_SECOND:
						return Nanos;
					case MICRO_OF_SECOND:
						return Nanos / 1000;
					case MILLI_OF_SECOND:
						return Nanos / 1000000;
					case INSTANT_SECONDS:
						INSTANT_SECONDS.checkValidIntValue(Seconds);
					break;
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return temporal.TemporalAccessor_Fields.range(field).checkValidIntValue(field.GetFrom(this), field);
		}

		/// <summary>
		/// Gets the value of the specified field from this instant as a {@code long}.
		/// <para>
		/// This queries this instant for the value of the specified field.
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
					case NANO_OF_SECOND:
						return Nanos;
					case MICRO_OF_SECOND:
						return Nanos / 1000;
					case MILLI_OF_SECOND:
						return Nanos / 1000000;
					case INSTANT_SECONDS:
						return Seconds;
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the number of seconds from the Java epoch of 1970-01-01T00:00:00Z.
		/// <para>
		/// The epoch second count is a simple incrementing count of seconds where
		/// second 0 is 1970-01-01T00:00:00Z.
		/// The nanosecond part of the day is returned by {@code getNanosOfSecond}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		public long EpochSecond
		{
			get
			{
				return Seconds;
			}
		}

		/// <summary>
		/// Gets the number of nanoseconds, later along the time-line, from the start
		/// of the second.
		/// <para>
		/// The nanosecond-of-second value measures the total number of nanoseconds from
		/// the second returned by {@code getEpochSecond}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the nanoseconds within the second, always positive, never exceeds 999,999,999 </returns>
		public int Nano
		{
			get
			{
				return Nanos;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this instant.
		/// <para>
		/// This returns an {@code Instant}, based on this one, with the instant adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
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
		/// <returns> an {@code Instant} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Instant With(TemporalAdjuster adjuster)
		{
			return (Instant) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified field set to a new value.
		/// <para>
		/// This returns an {@code Instant}, based on this one, with the value
		/// for the specified field changed.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code NANO_OF_SECOND} -
		///  Returns an {@code Instant} with the specified nano-of-second.
		///  The epoch-second will be unchanged.
		/// <li>{@code MICRO_OF_SECOND} -
		///  Returns an {@code Instant} with the nano-of-second replaced by the specified
		///  micro-of-second multiplied by 1,000. The epoch-second will be unchanged.
		/// <li>{@code MILLI_OF_SECOND} -
		///  Returns an {@code Instant} with the nano-of-second replaced by the specified
		///  milli-of-second multiplied by 1,000,000. The epoch-second will be unchanged.
		/// <li>{@code INSTANT_SECONDS} -
		///  Returns an {@code Instant} with the specified epoch-second.
		///  The nano-of-second will be unchanged.
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
		/// <returns> an {@code Instant} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				f.checkValidValue(newValue);
				switch (f)
				{
					case MILLI_OF_SECOND:
					{
						int nval = (int) newValue * 1000000;
						return (nval != Nanos ? Create(Seconds, nval) : this);
					}
					case MICRO_OF_SECOND:
					{
						int nval = (int) newValue * 1000;
						return (nval != Nanos ? Create(Seconds, nval) : this);
					}
					case NANO_OF_SECOND:
						return (newValue != Nanos ? Create(Seconds, (int) newValue) : this);
					case INSTANT_SECONDS:
						return (newValue != Seconds ? Create(newValue, Nanos) : this);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code Instant} truncated to the specified unit.
		/// <para>
		/// Truncating the instant returns a copy of the original with fields
		/// smaller than the specified unit set to zero.
		/// The fields are calculated on the basis of using a UTC offset as seen
		/// in {@code toString}.
		/// For example, truncating with the <seealso cref="ChronoUnit#MINUTES MINUTES"/> unit will
		/// round down to the nearest minute, setting the seconds and nanoseconds to zero.
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
		/// <returns> an {@code Instant} based on this instant with the time truncated, not null </returns>
		/// <exception cref="DateTimeException"> if the unit is invalid for truncation </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public Instant TruncatedTo(TemporalUnit unit)
		{
			if (unit == ChronoUnit.NANOS)
			{
				return this;
			}
			Duration unitDur = unit.Duration;
			if (unitDur.Seconds > LocalTime.SECONDS_PER_DAY)
			{
				throw new UnsupportedTemporalTypeException("Unit is too large to be used for truncation");
			}
			long dur = unitDur.ToNanos();
			if ((LocalTime.NANOS_PER_DAY % dur) != 0)
			{
				throw new UnsupportedTemporalTypeException("Unit must divide into a standard day without remainder");
			}
			long nod = (Seconds % LocalTime.SECONDS_PER_DAY) * LocalTime.NANOS_PER_SECOND + Nanos;
			long result = (nod / dur) * dur;
			return PlusNanos(result - nod);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this instant with the specified amount added.
		/// <para>
		/// This returns an {@code Instant}, based on this one, with the specified amount added.
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
		/// <returns> an {@code Instant} based on this instant with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Instant Plus(TemporalAmount amountToAdd)
		{
			return (Instant) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified amount added.
		/// <para>
		/// This returns an {@code Instant}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code NANOS} -
		///  Returns a {@code Instant} with the specified number of nanoseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/>.
		/// <li>{@code MICROS} -
		///  Returns a {@code Instant} with the specified number of microseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		///  multiplied by 1,000.
		/// <li>{@code MILLIS} -
		///  Returns a {@code Instant} with the specified number of milliseconds added.
		///  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		///  multiplied by 1,000,000.
		/// <li>{@code SECONDS} -
		///  Returns a {@code Instant} with the specified number of seconds added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/>.
		/// <li>{@code MINUTES} -
		///  Returns a {@code Instant} with the specified number of minutes added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		///  multiplied by 60.
		/// <li>{@code HOURS} -
		///  Returns a {@code Instant} with the specified number of hours added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		///  multiplied by 3,600.
		/// <li>{@code HALF_DAYS} -
		///  Returns a {@code Instant} with the specified number of half-days added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		///  multiplied by 43,200 (12 hours).
		/// <li>{@code DAYS} -
		///  Returns a {@code Instant} with the specified number of days added.
		///  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		///  multiplied by 86,400 (24 hours).
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
		/// <returns> an {@code Instant} based on this instant with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case NANOS:
						return PlusNanos(amountToAdd);
					case MICROS:
						return Plus(amountToAdd / 1000000, (amountToAdd % 1000000) * 1000);
					case MILLIS:
						return PlusMillis(amountToAdd);
					case SECONDS:
						return PlusSeconds(amountToAdd);
					case MINUTES:
						return PlusSeconds(Math.MultiplyExact(amountToAdd, SECONDS_PER_MINUTE));
					case HOURS:
						return PlusSeconds(Math.MultiplyExact(amountToAdd, SECONDS_PER_HOUR));
					case HALF_DAYS:
						return PlusSeconds(Math.MultiplyExact(amountToAdd, SECONDS_PER_DAY / 2));
					case DAYS:
						return PlusSeconds(Math.MultiplyExact(amountToAdd, SECONDS_PER_DAY));
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.AddTo(this, amountToAdd);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this instant with the specified duration in seconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified seconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant PlusSeconds(long secondsToAdd)
		{
			return Plus(secondsToAdd, 0);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified duration in milliseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millisToAdd">  the milliseconds to add, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified milliseconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant PlusMillis(long millisToAdd)
		{
			return Plus(millisToAdd / 1000, (millisToAdd % 1000) * 1000000);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified duration in nanoseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToAdd">  the nanoseconds to add, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified nanoseconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant PlusNanos(long nanosToAdd)
		{
			return Plus(0, nanosToAdd);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified duration added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		/// <param name="nanosToAdd">  the nanos to add, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified seconds added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		private Instant Plus(long secondsToAdd, long nanosToAdd)
		{
			if ((secondsToAdd | nanosToAdd) == 0)
			{
				return this;
			}
			long epochSec = Math.AddExact(Seconds, secondsToAdd);
			epochSec = Math.AddExact(epochSec, nanosToAdd / NANOS_PER_SECOND);
			nanosToAdd = nanosToAdd % NANOS_PER_SECOND;
			long nanoAdjustment = Nanos + nanosToAdd; // safe int+NANOS_PER_SECOND
			return OfEpochSecond(epochSec, nanoAdjustment);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this instant with the specified amount subtracted.
		/// <para>
		/// This returns an {@code Instant}, based on this one, with the specified amount subtracted.
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
		/// <returns> an {@code Instant} based on this instant with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Instant Minus(TemporalAmount amountToSubtract)
		{
			return (Instant) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified amount subtracted.
		/// <para>
		/// This returns a {@code Instant}, based on this one, with the amount
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
		/// <returns> an {@code Instant} based on this instant with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Instant Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this instant with the specified duration in seconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToSubtract">  the seconds to subtract, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified seconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant MinusSeconds(long secondsToSubtract)
		{
			if (secondsToSubtract == Long.MinValue)
			{
				return PlusSeconds(Long.MaxValue).PlusSeconds(1);
			}
			return PlusSeconds(-secondsToSubtract);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified duration in milliseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millisToSubtract">  the milliseconds to subtract, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified milliseconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant MinusMillis(long millisToSubtract)
		{
			if (millisToSubtract == Long.MinValue)
			{
				return PlusMillis(Long.MaxValue).PlusMillis(1);
			}
			return PlusMillis(-millisToSubtract);
		}

		/// <summary>
		/// Returns a copy of this instant with the specified duration in nanoseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToSubtract">  the nanoseconds to subtract, positive or negative </param>
		/// <returns> an {@code Instant} based on this instant with the specified nanoseconds subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Instant MinusNanos(long nanosToSubtract)
		{
			if (nanosToSubtract == Long.MinValue)
			{
				return PlusNanos(Long.MaxValue).PlusNanos(1);
			}
			return PlusNanos(-nanosToSubtract);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Queries this instant using the specified query.
		/// <para>
		/// This queries this instant using the specified query strategy object.
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
			if (query == TemporalQueries.Precision())
			{
				return (R) NANOS;
			}
			// inline TemporalAccessor.super.query(query) as an optimization
			if (query == TemporalQueries.Chronology() || query == TemporalQueries.ZoneId() || query == TemporalQueries.Zone() || query == TemporalQueries.Offset() || query == TemporalQueries.LocalDate() || query == TemporalQueries.LocalTime())
			{
				return temporal.TemporalAccessor_Fields.Null;
			}
			return query.QueryFrom(this);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this instant.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the instant changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// twice, passing <seealso cref="ChronoField#INSTANT_SECONDS"/> and
		/// <seealso cref="ChronoField#NANO_OF_SECOND"/> as the fields.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisInstant.adjustInto(temporal);
		///   temporal = temporal.with(thisInstant);
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
			return temporal.With(INSTANT_SECONDS, Seconds).With(NANO_OF_SECOND, Nanos);
		}

		/// <summary>
		/// Calculates the amount of time until another instant in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code Instant}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified instant.
		/// The result will be negative if the end is before the start.
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two instants.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code Instant} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in days between two dates can be calculated
		/// using {@code startInstant.until(endInstant, SECONDS)}.
		/// </para>
		/// <para>
		/// There are two equivalent ways of using this method.
		/// The first is to invoke this method.
		/// The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		/// <pre>
		///   // these two lines are equivalent
		///   amount = start.until(end, SECONDS);
		///   amount = SECONDS.between(start, end);
		/// </pre>
		/// The choice should be made based on which makes the code more readable.
		/// </para>
		/// <para>
		/// The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		/// The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		/// {@code MINUTES}, {@code HOURS}, {@code HALF_DAYS} and {@code DAYS}
		/// are supported. Other {@code ChronoUnit} values will throw an exception.
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to an {@code Instant}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this instant and the end instant </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to an {@code Instant} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			Instant end = Instant.From(endExclusive);
			if (unit is ChronoUnit)
			{
				ChronoUnit f = (ChronoUnit) unit;
				switch (f)
				{
					case NANOS:
						return NanosUntil(end);
					case MICROS:
						return NanosUntil(end) / 1000;
					case MILLIS:
						return Math.SubtractExact(end.ToEpochMilli(), ToEpochMilli());
					case SECONDS:
						return SecondsUntil(end);
					case MINUTES:
						return SecondsUntil(end) / SECONDS_PER_MINUTE;
					case HOURS:
						return SecondsUntil(end) / SECONDS_PER_HOUR;
					case HALF_DAYS:
						return SecondsUntil(end) / (12 * SECONDS_PER_HOUR);
					case DAYS:
						return SecondsUntil(end) / (SECONDS_PER_DAY);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.Between(this, end);
		}

		private long NanosUntil(Instant end)
		{
			long secsDiff = Math.SubtractExact(end.Seconds, Seconds);
			long totalNanos = Math.MultiplyExact(secsDiff, NANOS_PER_SECOND);
			return Math.AddExact(totalNanos, end.Nanos - Nanos);
		}

		private long SecondsUntil(Instant end)
		{
			long secsDiff = Math.SubtractExact(end.Seconds, Seconds);
			long nanosDiff = end.Nanos - Nanos;
			if (secsDiff > 0 && nanosDiff < 0)
			{
				secsDiff--;
			}
			else if (secsDiff < 0 && nanosDiff > 0)
			{
				secsDiff++;
			}
			return secsDiff;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this instant with an offset to create an {@code OffsetDateTime}.
		/// <para>
		/// This returns an {@code OffsetDateTime} formed from this instant at the
		/// specified offset from UTC/Greenwich. An exception will be thrown if the
		/// instant is too large to fit into an offset date-time.
		/// </para>
		/// <para>
		/// This method is equivalent to
		/// <seealso cref="OffsetDateTime#ofInstant(Instant, ZoneId) OffsetDateTime.ofInstant(this, offset)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to combine with, not null </param>
		/// <returns> the offset date-time formed from this instant and the specified offset, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public OffsetDateTime AtOffset(ZoneOffset offset)
		{
			return OffsetDateTime.OfInstant(this, offset);
		}

		/// <summary>
		/// Combines this instant with a time-zone to create a {@code ZonedDateTime}.
		/// <para>
		/// This returns an {@code ZonedDateTime} formed from this instant at the
		/// specified time-zone. An exception will be thrown if the instant is too
		/// large to fit into a zoned date-time.
		/// </para>
		/// <para>
		/// This method is equivalent to
		/// <seealso cref="ZonedDateTime#ofInstant(Instant, ZoneId) ZonedDateTime.ofInstant(this, zone)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone to combine with, not null </param>
		/// <returns> the zoned date-time formed from this instant and the specified zone, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public ZonedDateTime AtZone(ZoneId zone)
		{
			return ZonedDateTime.OfInstant(this, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Converts this instant to the number of milliseconds from the epoch
		/// of 1970-01-01T00:00:00Z.
		/// <para>
		/// If this instant represents a point on the time-line too far in the future
		/// or past to fit in a {@code long} milliseconds, then an exception is thrown.
		/// </para>
		/// <para>
		/// If this instant has greater than millisecond precision, then the conversion
		/// will drop any excess precision information as though the amount in nanoseconds
		/// was subject to integer division by one million.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of milliseconds since the epoch of 1970-01-01T00:00:00Z </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long ToEpochMilli()
		{
			long millis = Math.MultiplyExact(Seconds, 1000);
			return millis + Nanos / 1000000;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this instant to the specified instant.
		/// <para>
		/// The comparison is based on the time-line position of the instants.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherInstant">  the other instant to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		/// <exception cref="NullPointerException"> if otherInstant is null </exception>
		public int CompareTo(Instant otherInstant)
		{
			int cmp = Long.Compare(Seconds, otherInstant.Seconds);
			if (cmp != 0)
			{
				return cmp;
			}
			return Nanos - otherInstant.Nanos;
		}

		/// <summary>
		/// Checks if this instant is after the specified instant.
		/// <para>
		/// The comparison is based on the time-line position of the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherInstant">  the other instant to compare to, not null </param>
		/// <returns> true if this instant is after the specified instant </returns>
		/// <exception cref="NullPointerException"> if otherInstant is null </exception>
		public bool IsAfter(Instant otherInstant)
		{
			return CompareTo(otherInstant) > 0;
		}

		/// <summary>
		/// Checks if this instant is before the specified instant.
		/// <para>
		/// The comparison is based on the time-line position of the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherInstant">  the other instant to compare to, not null </param>
		/// <returns> true if this instant is before the specified instant </returns>
		/// <exception cref="NullPointerException"> if otherInstant is null </exception>
		public bool IsBefore(Instant otherInstant)
		{
			return CompareTo(otherInstant) < 0;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this instant is equal to the specified instant.
		/// <para>
		/// The comparison is based on the time-line position of the instants.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherInstant">  the other instant, null returns false </param>
		/// <returns> true if the other instant is equal to this one </returns>
		public override bool Equals(Object otherInstant)
		{
			if (this == otherInstant)
			{
				return true;
			}
			if (otherInstant is Instant)
			{
				Instant other = (Instant) otherInstant;
				return this.Seconds == other.Seconds && this.Nanos == other.Nanos;
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instant.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return ((int)(Seconds ^ ((long)((ulong)Seconds >> 32)))) + 51 * Nanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// A string representation of this instant using ISO-8601 representation.
		/// <para>
		/// The format used is the same as <seealso cref="DateTimeFormatter#ISO_INSTANT"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an ISO-8601 representation of this instant, not null </returns>
		public override String ToString()
		{
			return DateTimeFormatter.ISO_INSTANT.Format(this);
		}

		// -----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(2);  // identifies an Instant
		///  out.writeLong(seconds);
		///  out.writeInt(nanos);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.INSTANT_TYPE, this);
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
			@out.WriteLong(Seconds);
			@out.WriteInt(Nanos);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Instant readExternal(java.io.DataInput in) throws java.io.IOException
		internal static Instant ReadExternal(DataInput @in)
		{
			long seconds = @in.ReadLong();
			int nanos = @in.ReadInt();
			return Instant.OfEpochSecond(seconds, nanos);
		}

	}

}