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
	/// A time-based amount of time, such as '34.5 seconds'.
	/// <para>
	/// This class models a quantity or amount of time in terms of seconds and nanoseconds.
	/// It can be accessed using other duration-based units, such as minutes and hours.
	/// In addition, the <seealso cref="ChronoUnit#DAYS DAYS"/> unit can be used and is treated as
	/// exactly equal to 24 hours, thus ignoring daylight savings effects.
	/// See <seealso cref="Period"/> for the date-based equivalent to this class.
	/// </para>
	/// <para>
	/// A physical duration could be of infinite length.
	/// For practicality, the duration is stored with constraints similar to <seealso cref="Instant"/>.
	/// The duration uses nanosecond resolution with a maximum value of the seconds that can
	/// be held in a {@code long}. This is greater than the current estimated age of the universe.
	/// </para>
	/// <para>
	/// The range of a duration requires the storage of a number larger than a {@code long}.
	/// To achieve this, the class stores a {@code long} representing seconds and an {@code int}
	/// representing nanosecond-of-second, which will always be between 0 and 999,999,999.
	/// The model is of a directed duration, meaning that the duration may be negative.
	/// </para>
	/// <para>
	/// The duration is measured in "seconds", but these are not necessarily identical to
	/// the scientific "SI second" definition based on atomic clocks.
	/// This difference only impacts durations measured near a leap-second and should not affect
	/// most applications.
	/// See <seealso cref="Instant"/> for a discussion as to the meaning of the second and time-scales.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code Duration} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Duration : TemporalAmount, Comparable<Duration>
	{

		/// <summary>
		/// Constant for a duration of zero.
		/// </summary>
		public static readonly Duration ZERO = new Duration(0, 0);
		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 3078945930695997490L;
		/// <summary>
		/// Constant for nanos per second.
		/// </summary>
		private static readonly System.Numerics.BigInteger BI_NANOS_PER_SECOND = System.Numerics.BigInteger.ValueOf(NANOS_PER_SECOND);
		/// <summary>
		/// The pattern for parsing.
		/// </summary>
		private static readonly Pattern PATTERN = Pattern.Compile("([-+]?)P(?:([-+]?[0-9]+)D)?" + "(T(?:([-+]?[0-9]+)H)?(?:([-+]?[0-9]+)M)?(?:([-+]?[0-9]+)(?:[.,]([0-9]{0,9}))?S)?)?", Pattern.CASE_INSENSITIVE);

		/// <summary>
		/// The number of seconds in the duration.
		/// </summary>
		private readonly long Seconds_Renamed;
		/// <summary>
		/// The number of nanoseconds in the duration, expressed as a fraction of the
		/// number of seconds. This is always positive, and never exceeds 999,999,999.
		/// </summary>
		private readonly int Nanos;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing a number of standard 24 hour days.
		/// <para>
		/// The seconds are calculated based on the standard definition of a day,
		/// where each day is 86400 seconds which implies a 24 hour day.
		/// The nanosecond in second field is set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the number of days, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="ArithmeticException"> if the input days exceeds the capacity of {@code Duration} </exception>
		public static Duration OfDays(long days)
		{
			return Create(Math.MultiplyExact(days, SECONDS_PER_DAY), 0);
		}

		/// <summary>
		/// Obtains a {@code Duration} representing a number of standard hours.
		/// <para>
		/// The seconds are calculated based on the standard definition of an hour,
		/// where each hour is 3600 seconds.
		/// The nanosecond in second field is set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hours">  the number of hours, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="ArithmeticException"> if the input hours exceeds the capacity of {@code Duration} </exception>
		public static Duration OfHours(long hours)
		{
			return Create(Math.MultiplyExact(hours, SECONDS_PER_HOUR), 0);
		}

		/// <summary>
		/// Obtains a {@code Duration} representing a number of standard minutes.
		/// <para>
		/// The seconds are calculated based on the standard definition of a minute,
		/// where each minute is 60 seconds.
		/// The nanosecond in second field is set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutes">  the number of minutes, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="ArithmeticException"> if the input minutes exceeds the capacity of {@code Duration} </exception>
		public static Duration OfMinutes(long minutes)
		{
			return Create(Math.MultiplyExact(minutes, SECONDS_PER_MINUTE), 0);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing a number of seconds.
		/// <para>
		/// The nanosecond in second field is set to zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the number of seconds, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		public static Duration OfSeconds(long seconds)
		{
			return Create(seconds, 0);
		}

		/// <summary>
		/// Obtains a {@code Duration} representing a number of seconds and an
		/// adjustment in nanoseconds.
		/// <para>
		/// This method allows an arbitrary number of nanoseconds to be passed in.
		/// The factory will alter the values of the second and nanosecond in order
		/// to ensure that the stored nanosecond is in the range 0 to 999,999,999.
		/// For example, the following will result in the exactly the same duration:
		/// <pre>
		///  Duration.ofSeconds(3, 1);
		///  Duration.ofSeconds(4, -999_999_999);
		///  Duration.ofSeconds(2, 1000_000_001);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the number of seconds, positive or negative </param>
		/// <param name="nanoAdjustment">  the nanosecond adjustment to the number of seconds, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="ArithmeticException"> if the adjustment causes the seconds to exceed the capacity of {@code Duration} </exception>
		public static Duration OfSeconds(long seconds, long nanoAdjustment)
		{
			long secs = Math.AddExact(seconds, Math.FloorDiv(nanoAdjustment, NANOS_PER_SECOND));
			int nos = (int) Math.FloorMod(nanoAdjustment, NANOS_PER_SECOND);
			return Create(secs, nos);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing a number of milliseconds.
		/// <para>
		/// The seconds and nanoseconds are extracted from the specified milliseconds.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millis">  the number of milliseconds, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		public static Duration OfMillis(long millis)
		{
			long secs = millis / 1000;
			int mos = (int)(millis % 1000);
			if (mos < 0)
			{
				mos += 1000;
				secs--;
			}
			return Create(secs, mos * 1000000);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing a number of nanoseconds.
		/// <para>
		/// The seconds and nanoseconds are extracted from the specified nanoseconds.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos">  the number of nanoseconds, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		public static Duration OfNanos(long nanos)
		{
			long secs = nanos / NANOS_PER_SECOND;
			int nos = (int)(nanos % NANOS_PER_SECOND);
			if (nos < 0)
			{
				nos += NANOS_PER_SECOND;
				secs--;
			}
			return Create(secs, nos);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing an amount in the specified unit.
		/// <para>
		/// The parameters represent the two parts of a phrase like '6 Hours'. For example:
		/// <pre>
		///  Duration.of(3, SECONDS);
		///  Duration.of(465, HOURS);
		/// </pre>
		/// Only a subset of units are accepted by this method.
		/// The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		/// be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amount">  the amount of the duration, measured in terms of the unit, positive or negative </param>
		/// <param name="unit">  the unit that the duration is measured in, must have an exact duration, not null </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="DateTimeException"> if the period unit has an estimated duration </exception>
		/// <exception cref="ArithmeticException"> if a numeric overflow occurs </exception>
		public static Duration Of(long amount, TemporalUnit unit)
		{
			return ZERO.Plus(amount, unit);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Duration} from a temporal amount.
		/// <para>
		/// This obtains a duration based on the specified amount.
		/// A {@code TemporalAmount} represents an  amount of time, which may be
		/// date-based or time-based, which this factory extracts to a duration.
		/// </para>
		/// <para>
		/// The conversion loops around the set of units from the amount and uses
		/// the <seealso cref="TemporalUnit#getDuration() duration"/> of the unit to
		/// calculate the total {@code Duration}.
		/// Only a subset of units are accepted by this method. The unit must either
		/// have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/>
		/// or be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours.
		/// If any other units are found then an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amount">  the temporal amount to convert, not null </param>
		/// <returns> the equivalent duration, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code Duration} </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public static Duration From(TemporalAmount amount)
		{
			Objects.RequireNonNull(amount, "amount");
			Duration duration = ZERO;
			foreach (TemporalUnit unit in amount.Units)
			{
				duration = duration.Plus(amount.Get(unit), unit);
			}
			return duration;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} from a text string such as {@code PnDTnHnMn.nS}.
		/// <para>
		/// This will parse a textual representation of a duration, including the
		/// string produced by {@code toString()}. The formats accepted are based
		/// on the ISO-8601 duration format {@code PnDTnHnMn.nS} with days
		/// considered to be exactly 24 hours.
		/// </para>
		/// <para>
		/// The string starts with an optional sign, denoted by the ASCII negative
		/// or positive symbol. If negative, the whole period is negated.
		/// The ASCII letter "P" is next in upper or lower case.
		/// There are then four sections, each consisting of a number and a suffix.
		/// The sections have suffixes in ASCII of "D", "H", "M" and "S" for
		/// days, hours, minutes and seconds, accepted in upper or lower case.
		/// The suffixes must occur in order. The ASCII letter "T" must occur before
		/// the first occurrence, if any, of an hour, minute or second section.
		/// At least one of the four sections must be present, and if "T" is present
		/// there must be at least one section after the "T".
		/// The number part of each section must consist of one or more ASCII digits.
		/// The number may be prefixed by the ASCII negative or positive symbol.
		/// The number of days, hours and minutes must parse to an {@code long}.
		/// The number of seconds must parse to an {@code long} with optional fraction.
		/// The decimal point may be either a dot or a comma.
		/// The fractional part may have from zero to 9 digits.
		/// </para>
		/// <para>
		/// The leading plus/minus sign, and negative values for other units are
		/// not part of the ISO-8601 standard.
		/// </para>
		/// <para>
		/// Examples:
		/// <pre>
		///    "PT20.345S" -- parses as "20.345 seconds"
		///    "PT15M"     -- parses as "15 minutes" (where a minute is 60 seconds)
		///    "PT10H"     -- parses as "10 hours" (where an hour is 3600 seconds)
		///    "P2D"       -- parses as "2 days" (where a day is 24 hours or 86400 seconds)
		///    "P2DT3H4M"  -- parses as "2 days, 3 hours and 4 minutes"
		///    "P-6H3M"    -- parses as "-6 hours and +3 minutes"
		///    "-P6H3M"    -- parses as "-6 hours and -3 minutes"
		///    "-P-6H+3M"  -- parses as "+6 hours and -3 minutes"
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <returns> the parsed duration, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed to a duration </exception>
		public static Duration Parse(CharSequence text)
		{
			Objects.RequireNonNull(text, "text");
			Matcher matcher = PATTERN.Matcher(text);
			if (matcher.Matches())
			{
				// check for letter T but no time sections
				if ("T".Equals(matcher.Group(3)) == false)
				{
					bool negate = "-".Equals(matcher.Group(1));
					String dayMatch = matcher.Group(2);
					String hourMatch = matcher.Group(4);
					String minuteMatch = matcher.Group(5);
					String secondMatch = matcher.Group(6);
					String fractionMatch = matcher.Group(7);
					if (dayMatch != null || hourMatch != null || minuteMatch != null || secondMatch != null)
					{
						long daysAsSecs = ParseNumber(text, dayMatch, SECONDS_PER_DAY, "days");
						long hoursAsSecs = ParseNumber(text, hourMatch, SECONDS_PER_HOUR, "hours");
						long minsAsSecs = ParseNumber(text, minuteMatch, SECONDS_PER_MINUTE, "minutes");
						long seconds = ParseNumber(text, secondMatch, 1, "seconds");
						int nanos = ParseFraction(text, fractionMatch, seconds < 0 ? - 1 : 1);
						try
						{
							return Create(negate, daysAsSecs, hoursAsSecs, minsAsSecs, seconds, nanos);
						}
						catch (ArithmeticException ex)
						{
							throw (DateTimeParseException) (new DateTimeParseException("Text cannot be parsed to a Duration: overflow", text, 0)).InitCause(ex);
						}
					}
				}
			}
			throw new DateTimeParseException("Text cannot be parsed to a Duration", text, 0);
		}

		private static long ParseNumber(CharSequence text, String parsed, int multiplier, String errorText)
		{
			// regex limits to [-+]?[0-9]+
			if (parsed == null)
			{
				return 0;
			}
			try
			{
				long val = Convert.ToInt64(parsed);
				return Math.MultiplyExact(val, multiplier);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (NumberFormatException | ArithmeticException ex)
			{
				throw (DateTimeParseException) (new DateTimeParseException("Text cannot be parsed to a Duration: " + errorText, text, 0)).InitCause(ex);
			}
		}

		private static int ParseFraction(CharSequence text, String parsed, int negate)
		{
			// regex limits to [0-9]{0,9}
			if (parsed == null || parsed.Length() == 0)
			{
				return 0;
			}
			try
			{
				parsed = (parsed + "000000000").Substring(0, 9);
				return Convert.ToInt32(parsed) * negate;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (NumberFormatException | ArithmeticException ex)
			{
				throw (DateTimeParseException) (new DateTimeParseException("Text cannot be parsed to a Duration: fraction", text, 0)).InitCause(ex);
			}
		}

		private static Duration Create(bool negate, long daysAsSecs, long hoursAsSecs, long minsAsSecs, long secs, int nanos)
		{
			long seconds = Math.AddExact(daysAsSecs, Math.AddExact(hoursAsSecs, Math.AddExact(minsAsSecs, secs)));
			if (negate)
			{
				return OfSeconds(seconds, nanos).Negated();
			}
			return OfSeconds(seconds, nanos);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Duration} representing the duration between two temporal objects.
		/// <para>
		/// This calculates the duration between two temporal objects. If the objects
		/// are of different types, then the duration is calculated based on the type
		/// of the first object. For example, if the first argument is a {@code LocalTime}
		/// then the second argument is converted to a {@code LocalTime}.
		/// </para>
		/// <para>
		/// The specified temporal objects must support the <seealso cref="ChronoUnit#SECONDS SECONDS"/> unit.
		/// For full accuracy, either the <seealso cref="ChronoUnit#NANOS NANOS"/> unit or the
		/// <seealso cref="ChronoField#NANO_OF_SECOND NANO_OF_SECOND"/> field should be supported.
		/// </para>
		/// <para>
		/// The result of this method can be a negative period if the end is before the start.
		/// To guarantee to obtain a positive duration call <seealso cref="#abs()"/> on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="startInclusive">  the start instant, inclusive, not null </param>
		/// <param name="endExclusive">  the end instant, exclusive, not null </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="DateTimeException"> if the seconds between the temporals cannot be obtained </exception>
		/// <exception cref="ArithmeticException"> if the calculation exceeds the capacity of {@code Duration} </exception>
		public static Duration Between(Temporal startInclusive, Temporal endExclusive)
		{
			try
			{
				return OfNanos(startInclusive.Until(endExclusive, NANOS));
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (DateTimeException | ArithmeticException ex)
			{
				long secs = startInclusive.Until(endExclusive, SECONDS);
				long nanos;
				try
				{
					nanos = endExclusive.GetLong(NANO_OF_SECOND) - startInclusive.GetLong(NANO_OF_SECOND);
					if (secs > 0 && nanos < 0)
					{
						secs++;
					}
					else if (secs < 0 && nanos > 0)
					{
						secs--;
					}
				}
				catch (DateTimeException)
				{
					nanos = 0;
				}
				return OfSeconds(secs, nanos);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Duration} using seconds and nanoseconds.
		/// </summary>
		/// <param name="seconds">  the length of the duration in seconds, positive or negative </param>
		/// <param name="nanoAdjustment">  the nanosecond adjustment within the second, from 0 to 999,999,999 </param>
		private static Duration Create(long seconds, int nanoAdjustment)
		{
			if ((seconds | nanoAdjustment) == 0)
			{
				return ZERO;
			}
			return new Duration(seconds, nanoAdjustment);
		}

		/// <summary>
		/// Constructs an instance of {@code Duration} using seconds and nanoseconds.
		/// </summary>
		/// <param name="seconds">  the length of the duration in seconds, positive or negative </param>
		/// <param name="nanos">  the nanoseconds within the second, from 0 to 999,999,999 </param>
		private Duration(long seconds, int nanos) : base()
		{
			this.Seconds_Renamed = seconds;
			this.Nanos = nanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the value of the requested unit.
		/// <para>
		/// This returns a value for each of the two supported units,
		/// <seealso cref="ChronoUnit#SECONDS SECONDS"/> and <seealso cref="ChronoUnit#NANOS NANOS"/>.
		/// All other units throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit"> the {@code TemporalUnit} for which to return the value </param>
		/// <returns> the long value of the unit </returns>
		/// <exception cref="DateTimeException"> if the unit is not supported </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public long Get(TemporalUnit unit)
		{
			if (unit == SECONDS)
			{
				return Seconds_Renamed;
			}
			else if (unit == NANOS)
			{
				return Nanos;
			}
			else
			{
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
		}

		/// <summary>
		/// Gets the set of units supported by this duration.
		/// <para>
		/// The supported units are <seealso cref="ChronoUnit#SECONDS SECONDS"/>,
		/// and <seealso cref="ChronoUnit#NANOS NANOS"/>.
		/// They are returned in the order seconds, nanos.
		/// </para>
		/// <para>
		/// This set can be used in conjunction with <seealso cref="#get(TemporalUnit)"/>
		/// to access the entire state of the duration.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a list containing the seconds and nanos units, not null </returns>
		public IList<TemporalUnit> Units
		{
			get
			{
				return DurationUnits.UNITS;
			}
		}

		/// <summary>
		/// Private class to delay initialization of this list until needed.
		/// The circular dependency between Duration and ChronoUnit prevents
		/// the simple initialization in Duration.
		/// </summary>
		private class DurationUnits
		{
			internal static readonly IList<TemporalUnit> UNITS = Collections.UnmodifiableList(Arrays.asList<TemporalUnit>(SECONDS, NANOS));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this duration is zero length.
		/// <para>
		/// A {@code Duration} represents a directed distance between two points on
		/// the time-line and can therefore be positive, zero or negative.
		/// This method checks whether the length is zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this duration has a total length equal to zero </returns>
		public bool Zero
		{
			get
			{
				return (Seconds_Renamed | Nanos) == 0;
			}
		}

		/// <summary>
		/// Checks if this duration is negative, excluding zero.
		/// <para>
		/// A {@code Duration} represents a directed distance between two points on
		/// the time-line and can therefore be positive, zero or negative.
		/// This method checks whether the length is less than zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this duration has a total length less than zero </returns>
		public bool Negative
		{
			get
			{
				return Seconds_Renamed < 0;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the number of seconds in this duration.
		/// <para>
		/// The length of the duration is stored using two fields - seconds and nanoseconds.
		/// The nanoseconds part is a value from 0 to 999,999,999 that is an adjustment to
		/// the length in seconds.
		/// The total duration is defined by calling this method and <seealso cref="#getNano()"/>.
		/// </para>
		/// <para>
		/// A {@code Duration} represents a directed distance between two points on the time-line.
		/// A negative duration is expressed by the negative sign of the seconds part.
		/// A duration of -1 nanosecond is stored as -1 seconds plus 999,999,999 nanoseconds.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the whole seconds part of the length of the duration, positive or negative </returns>
		public long Seconds
		{
			get
			{
				return Seconds_Renamed;
			}
		}

		/// <summary>
		/// Gets the number of nanoseconds within the second in this duration.
		/// <para>
		/// The length of the duration is stored using two fields - seconds and nanoseconds.
		/// The nanoseconds part is a value from 0 to 999,999,999 that is an adjustment to
		/// the length in seconds.
		/// The total duration is defined by calling this method and <seealso cref="#getSeconds()"/>.
		/// </para>
		/// <para>
		/// A {@code Duration} represents a directed distance between two points on the time-line.
		/// A negative duration is expressed by the negative sign of the seconds part.
		/// A duration of -1 nanosecond is stored as -1 seconds plus 999,999,999 nanoseconds.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the nanoseconds within the second part of the length of the duration, from 0 to 999,999,999 </returns>
		public int Nano
		{
			get
			{
				return Nanos;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the specified amount of seconds.
		/// <para>
		/// This returns a duration with the specified seconds, retaining the
		/// nano-of-second part of this duration.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seconds">  the seconds to represent, may be negative </param>
		/// <returns> a {@code Duration} based on this period with the requested seconds, not null </returns>
		public Duration WithSeconds(long seconds)
		{
			return Create(seconds, Nanos);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified nano-of-second.
		/// <para>
		/// This returns a duration with the specified nano-of-second, retaining the
		/// seconds part of this duration.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <returns> a {@code Duration} based on this period with the requested nano-of-second, not null </returns>
		/// <exception cref="DateTimeException"> if the nano-of-second is invalid </exception>
		public Duration WithNanos(int nanoOfSecond)
		{
			NANO_OF_SECOND.checkValidIntValue(nanoOfSecond);
			return Create(Seconds_Renamed, nanoOfSecond);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the specified duration added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="duration">  the duration to add, positive or negative, not null </param>
		/// <returns> a {@code Duration} based on this duration with the specified duration added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Plus(Duration duration)
		{
			return Plus(duration.Seconds, duration.Nano);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration added.
		/// <para>
		/// The duration amount is measured in terms of the specified unit.
		/// Only a subset of units are accepted by this method.
		/// The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		/// be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToAdd">  the amount to add, measured in terms of the unit, positive or negative </param>
		/// <param name="unit">  the unit that the amount is measured in, must have an exact duration, not null </param>
		/// <returns> a {@code Duration} based on this duration with the specified duration added, not null </returns>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Plus(long amountToAdd, TemporalUnit unit)
		{
			Objects.RequireNonNull(unit, "unit");
			if (unit == DAYS)
			{
				return Plus(Math.MultiplyExact(amountToAdd, SECONDS_PER_DAY), 0);
			}
			if (unit.DurationEstimated)
			{
				throw new UnsupportedTemporalTypeException("Unit must not have an estimated duration");
			}
			if (amountToAdd == 0)
			{
				return this;
			}
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case NANOS:
						return PlusNanos(amountToAdd);
					case MICROS:
						return PlusSeconds((amountToAdd / (1000_000L * 1000)) * 1000).PlusNanos((amountToAdd % (1000_000L * 1000)) * 1000);
					case MILLIS:
						return PlusMillis(amountToAdd);
					case SECONDS:
						return PlusSeconds(amountToAdd);
				}
				return PlusSeconds(Math.MultiplyExact(unit.Duration.Seconds_Renamed, amountToAdd));
			}
			Duration duration = unit.Duration.MultipliedBy(amountToAdd);
			return PlusSeconds(duration.Seconds).PlusNanos(duration.Nano);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the specified duration in standard 24 hour days added.
		/// <para>
		/// The number of days is multiplied by 86400 to obtain the number of seconds to add.
		/// This is based on the standard definition of a day as 24 hours.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToAdd">  the days to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified days added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusDays(long daysToAdd)
		{
			return Plus(Math.MultiplyExact(daysToAdd, SECONDS_PER_DAY), 0);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in hours added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hoursToAdd">  the hours to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified hours added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusHours(long hoursToAdd)
		{
			return Plus(Math.MultiplyExact(hoursToAdd, SECONDS_PER_HOUR), 0);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in minutes added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutesToAdd">  the minutes to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified minutes added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusMinutes(long minutesToAdd)
		{
			return Plus(Math.MultiplyExact(minutesToAdd, SECONDS_PER_MINUTE), 0);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in seconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified seconds added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusSeconds(long secondsToAdd)
		{
			return Plus(secondsToAdd, 0);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in milliseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millisToAdd">  the milliseconds to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified milliseconds added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusMillis(long millisToAdd)
		{
			return Plus(millisToAdd / 1000, (millisToAdd % 1000) * 1000000);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in nanoseconds added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToAdd">  the nanoseconds to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified nanoseconds added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration PlusNanos(long nanosToAdd)
		{
			return Plus(0, nanosToAdd);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		/// <param name="nanosToAdd">  the nanos to add, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified seconds added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		private Duration Plus(long secondsToAdd, long nanosToAdd)
		{
			if ((secondsToAdd | nanosToAdd) == 0)
			{
				return this;
			}
			long epochSec = Math.AddExact(Seconds_Renamed, secondsToAdd);
			epochSec = Math.AddExact(epochSec, nanosToAdd / NANOS_PER_SECOND);
			nanosToAdd = nanosToAdd % NANOS_PER_SECOND;
			long nanoAdjustment = Nanos + nanosToAdd; // safe int+NANOS_PER_SECOND
			return OfSeconds(epochSec, nanoAdjustment);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the specified duration subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="duration">  the duration to subtract, positive or negative, not null </param>
		/// <returns> a {@code Duration} based on this duration with the specified duration subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Minus(Duration duration)
		{
			long secsToSubtract = duration.Seconds;
			int nanosToSubtract = duration.Nano;
			if (secsToSubtract == Long.MinValue)
			{
				return Plus(Long.MaxValue, -nanosToSubtract).Plus(1, 0);
			}
			return Plus(-secsToSubtract, -nanosToSubtract);
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration subtracted.
		/// <para>
		/// The duration amount is measured in terms of the specified unit.
		/// Only a subset of units are accepted by this method.
		/// The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		/// be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToSubtract">  the amount to subtract, measured in terms of the unit, positive or negative </param>
		/// <param name="unit">  the unit that the amount is measured in, must have an exact duration, not null </param>
		/// <returns> a {@code Duration} based on this duration with the specified duration subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the specified duration in standard 24 hour days subtracted.
		/// <para>
		/// The number of days is multiplied by 86400 to obtain the number of seconds to subtract.
		/// This is based on the standard definition of a day as 24 hours.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToSubtract">  the days to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified days subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusDays(long daysToSubtract)
		{
			return (daysToSubtract == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-daysToSubtract));
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in hours subtracted.
		/// <para>
		/// The number of hours is multiplied by 3600 to obtain the number of seconds to subtract.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hoursToSubtract">  the hours to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified hours subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusHours(long hoursToSubtract)
		{
			return (hoursToSubtract == Long.MinValue ? PlusHours(Long.MaxValue).PlusHours(1) : PlusHours(-hoursToSubtract));
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in minutes subtracted.
		/// <para>
		/// The number of hours is multiplied by 60 to obtain the number of seconds to subtract.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minutesToSubtract">  the minutes to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified minutes subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusMinutes(long minutesToSubtract)
		{
			return (minutesToSubtract == Long.MinValue ? PlusMinutes(Long.MaxValue).PlusMinutes(1) : PlusMinutes(-minutesToSubtract));
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in seconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="secondsToSubtract">  the seconds to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified seconds subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusSeconds(long secondsToSubtract)
		{
			return (secondsToSubtract == Long.MinValue ? PlusSeconds(Long.MaxValue).PlusSeconds(1) : PlusSeconds(-secondsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in milliseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millisToSubtract">  the milliseconds to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified milliseconds subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusMillis(long millisToSubtract)
		{
			return (millisToSubtract == Long.MinValue ? PlusMillis(Long.MaxValue).PlusMillis(1) : PlusMillis(-millisToSubtract));
		}

		/// <summary>
		/// Returns a copy of this duration with the specified duration in nanoseconds subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosToSubtract">  the nanoseconds to subtract, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration with the specified nanoseconds subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MinusNanos(long nanosToSubtract)
		{
			return (nanosToSubtract == Long.MinValue ? PlusNanos(Long.MaxValue).PlusNanos(1) : PlusNanos(-nanosToSubtract));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration multiplied by the scalar.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="multiplicand">  the value to multiply the duration by, positive or negative </param>
		/// <returns> a {@code Duration} based on this duration multiplied by the specified scalar, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration MultipliedBy(long multiplicand)
		{
			if (multiplicand == 0)
			{
				return ZERO;
			}
			if (multiplicand == 1)
			{
				return this;
			}
			return Create(ToSeconds() * decimal.ValueOf(multiplicand));
		}

		/// <summary>
		/// Returns a copy of this duration divided by the specified value.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="divisor">  the value to divide the duration by, positive or negative, not zero </param>
		/// <returns> a {@code Duration} based on this duration divided by the specified divisor, not null </returns>
		/// <exception cref="ArithmeticException"> if the divisor is zero or if numeric overflow occurs </exception>
		public Duration DividedBy(long divisor)
		{
			if (divisor == 0)
			{
				throw new ArithmeticException("Cannot divide by zero");
			}
			if (divisor == 1)
			{
				return this;
			}
			return Create(ToSeconds().Divide(decimal.ValueOf(divisor), RoundingMode.DOWN));
		}

		/// <summary>
		/// Converts this duration to the total length in seconds and
		/// fractional nanoseconds expressed as a {@code BigDecimal}.
		/// </summary>
		/// <returns> the total length of the duration in seconds, with a scale of 9, not null </returns>
		private decimal ToSeconds()
		{
			return decimal.ValueOf(Seconds_Renamed) + decimal.ValueOf(Nanos, 9);
		}

		/// <summary>
		/// Creates an instance of {@code Duration} from a number of seconds.
		/// </summary>
		/// <param name="seconds">  the number of seconds, up to scale 9, positive or negative </param>
		/// <returns> a {@code Duration}, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		private static Duration Create(decimal seconds)
		{
			System.Numerics.BigInteger nanos = seconds.MovePointRight(9).ToBigIntegerExact();
			System.Numerics.BigInteger[] divRem = nanos.divideAndRemainder(BI_NANOS_PER_SECOND);
			if (divRem[0].bitLength() > 63)
			{
				throw new ArithmeticException("Exceeds capacity of Duration: " + nanos);
			}
			return OfSeconds((long)divRem[0], (int)divRem[1]);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this duration with the length negated.
		/// <para>
		/// This method swaps the sign of the total length of this duration.
		/// For example, {@code PT1.3S} will be returned as {@code PT-1.3S}.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Duration} based on this duration with the amount negated, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Negated()
		{
			return MultipliedBy(-1);
		}

		/// <summary>
		/// Returns a copy of this duration with a positive length.
		/// <para>
		/// This method returns a positive duration by effectively removing the sign from any negative total length.
		/// For example, {@code PT-1.3S} will be returned as {@code PT1.3S}.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Duration} based on this duration with an absolute length, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Duration Abs()
		{
			return Negative ? Negated() : this;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Adds this duration to the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this duration added.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#plus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisDuration.addTo(dateTime);
		///   dateTime = dateTime.plus(thisDuration);
		/// </pre>
		/// </para>
		/// <para>
		/// The calculation will add the seconds, then nanos.
		/// Only non-zero amounts will be added.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to add </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Temporal AddTo(Temporal temporal)
		{
			if (Seconds_Renamed != 0)
			{
				temporal = temporal.Plus(Seconds_Renamed, SECONDS);
			}
			if (Nanos != 0)
			{
				temporal = temporal.Plus(Nanos, NANOS);
			}
			return temporal;
		}

		/// <summary>
		/// Subtracts this duration from the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this duration subtracted.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#minus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisDuration.subtractFrom(dateTime);
		///   dateTime = dateTime.minus(thisDuration);
		/// </pre>
		/// </para>
		/// <para>
		/// The calculation will subtract the seconds, then nanos.
		/// Only non-zero amounts will be added.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to subtract </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Temporal SubtractFrom(Temporal temporal)
		{
			if (Seconds_Renamed != 0)
			{
				temporal = temporal.minus(Seconds_Renamed, SECONDS);
			}
			if (Nanos != 0)
			{
				temporal = temporal.minus(Nanos, NANOS);
			}
			return temporal;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the number of days in this duration.
		/// <para>
		/// This returns the total number of days in the duration by dividing the
		/// number of seconds by 86400.
		/// This is based on the standard definition of a day as 24 hours.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of days in the duration, may be negative </returns>
		public long ToDays()
		{
			return Seconds_Renamed / SECONDS_PER_DAY;
		}

		/// <summary>
		/// Gets the number of hours in this duration.
		/// <para>
		/// This returns the total number of hours in the duration by dividing the
		/// number of seconds by 3600.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of hours in the duration, may be negative </returns>
		public long ToHours()
		{
			return Seconds_Renamed / SECONDS_PER_HOUR;
		}

		/// <summary>
		/// Gets the number of minutes in this duration.
		/// <para>
		/// This returns the total number of minutes in the duration by dividing the
		/// number of seconds by 60.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of minutes in the duration, may be negative </returns>
		public long ToMinutes()
		{
			return Seconds_Renamed / SECONDS_PER_MINUTE;
		}

		/// <summary>
		/// Converts this duration to the total length in milliseconds.
		/// <para>
		/// If this duration is too large to fit in a {@code long} milliseconds, then an
		/// exception is thrown.
		/// </para>
		/// <para>
		/// If this duration has greater than millisecond precision, then the conversion
		/// will drop any excess precision information as though the amount in nanoseconds
		/// was subject to integer division by one million.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the total length of the duration in milliseconds </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long ToMillis()
		{
			long millis = Math.MultiplyExact(Seconds_Renamed, 1000);
			millis = Math.AddExact(millis, Nanos / 1000000);
			return millis;
		}

		/// <summary>
		/// Converts this duration to the total length in nanoseconds expressed as a {@code long}.
		/// <para>
		/// If this duration is too large to fit in a {@code long} nanoseconds, then an
		/// exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the total length of the duration in nanoseconds </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long ToNanos()
		{
			long totalNanos = Math.MultiplyExact(Seconds_Renamed, NANOS_PER_SECOND);
			totalNanos = Math.AddExact(totalNanos, Nanos);
			return totalNanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this duration to the specified {@code Duration}.
		/// <para>
		/// The comparison is based on the total length of the durations.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherDuration">  the other duration to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(Duration otherDuration)
		{
			int cmp = Long.Compare(Seconds_Renamed, otherDuration.Seconds_Renamed);
			if (cmp != 0)
			{
				return cmp;
			}
			return Nanos - otherDuration.Nanos;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this duration is equal to the specified {@code Duration}.
		/// <para>
		/// The comparison is based on the total length of the durations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherDuration">  the other duration, null returns false </param>
		/// <returns> true if the other duration is equal to this one </returns>
		public override bool Equals(Object otherDuration)
		{
			if (this == otherDuration)
			{
				return true;
			}
			if (otherDuration is Duration)
			{
				Duration other = (Duration) otherDuration;
				return this.Seconds_Renamed == other.Seconds_Renamed && this.Nanos == other.Nanos;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this duration.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return ((int)(Seconds_Renamed ^ ((long)((ulong)Seconds_Renamed >> 32)))) + (51 * Nanos);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// A string representation of this duration using ISO-8601 seconds
		/// based representation, such as {@code PT8H6M12.345S}.
		/// <para>
		/// The format of the returned string will be {@code PTnHnMnS}, where n is
		/// the relevant hours, minutes or seconds part of the duration.
		/// Any fractional seconds are placed after a decimal point i the seconds section.
		/// If a section has a zero value, it is omitted.
		/// The hours, minutes and seconds will all have the same sign.
		/// </para>
		/// <para>
		/// Examples:
		/// <pre>
		///    "20.345 seconds"                 -- "PT20.345S
		///    "15 minutes" (15 * 60 seconds)   -- "PT15M"
		///    "10 hours" (10 * 3600 seconds)   -- "PT10H"
		///    "2 days" (2 * 86400 seconds)     -- "PT48H"
		/// </pre>
		/// Note that multiples of 24 hours are not output as days to avoid confusion
		/// with {@code Period}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an ISO-8601 representation of this duration, not null </returns>
		public override String ToString()
		{
			if (this == ZERO)
			{
				return "PT0S";
			}
			long hours = Seconds_Renamed / SECONDS_PER_HOUR;
			int minutes = (int)((Seconds_Renamed % SECONDS_PER_HOUR) / SECONDS_PER_MINUTE);
			int secs = (int)(Seconds_Renamed % SECONDS_PER_MINUTE);
			StringBuilder buf = new StringBuilder(24);
			buf.Append("PT");
			if (hours != 0)
			{
				buf.Append(hours).Append('H');
			}
			if (minutes != 0)
			{
				buf.Append(minutes).Append('M');
			}
			if (secs == 0 && Nanos == 0 && buf.Length() > 2)
			{
				return buf.ToString();
			}
			if (secs < 0 && Nanos > 0)
			{
				if (secs == -1)
				{
					buf.Append("-0");
				}
				else
				{
					buf.Append(secs + 1);
				}
			}
			else
			{
				buf.Append(secs);
			}
			if (Nanos > 0)
			{
				int pos = buf.Length();
				if (secs < 0)
				{
					buf.Append(2 * NANOS_PER_SECOND - Nanos);
				}
				else
				{
					buf.Append(Nanos + NANOS_PER_SECOND);
				}
				while (buf.CharAt(buf.Length() - 1) == '0')
				{
					buf.Length = buf.Length() - 1;
				}
				buf.SetCharAt(pos, '.');
			}
			buf.Append('S');
			return buf.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(1);  // identifies a Duration
		///  out.writeLong(seconds);
		///  out.writeInt(nanos);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.DURATION_TYPE, this);
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
			@out.WriteLong(Seconds_Renamed);
			@out.WriteInt(Nanos);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Duration readExternal(java.io.DataInput in) throws java.io.IOException
		internal static Duration ReadExternal(DataInput @in)
		{
			long seconds = @in.ReadLong();
			int nanos = @in.ReadInt();
			return Duration.OfSeconds(seconds, nanos);
		}

	}

}