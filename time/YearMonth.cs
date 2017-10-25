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
	/// A year-month in the ISO-8601 calendar system, such as {@code 2007-12}.
	/// <para>
	/// {@code YearMonth} is an immutable date-time object that represents the combination
	/// of a year and month. Any field that can be derived from a year and month, such as
	/// quarter-of-year, can be obtained.
	/// </para>
	/// <para>
	/// This class does not store or represent a day, time or time-zone.
	/// For example, the value "October 2007" can be stored in a {@code YearMonth}.
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
	/// {@code YearMonth} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class YearMonth : Temporal, TemporalAdjuster, Comparable<YearMonth>
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 4183400860270640070L;
		/// <summary>
		/// Parser.
		/// </summary>
		private static readonly DateTimeFormatter PARSER = new DateTimeFormatterBuilder().AppendValue(YEAR, 4, 10, SignStyle.EXCEEDS_PAD).AppendLiteral('-').AppendValue(MONTH_OF_YEAR, 2).ToFormatter();

		/// <summary>
		/// The year.
		/// </summary>
		private readonly int Year_Renamed;
		/// <summary>
		/// The month-of-year, not null.
		/// </summary>
		private readonly int Month_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current year-month from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current year-month.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current year-month using the system clock and default time-zone, not null </returns>
		public static YearMonth Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current year-month from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current year-month.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current year-month using the system clock, not null </returns>
		public static YearMonth Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current year-month from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current year-month.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current year-month, not null </returns>
		public static YearMonth Now(Clock clock)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocalDate now = LocalDate.now(clock);
			LocalDate now = LocalDate.Now(clock); // called once
			return YearMonth.Of(now.Year, now.Month);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code YearMonth} from a year and month.
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <returns> the year-month, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public static YearMonth Of(int year, Month month)
		{
			Objects.RequireNonNull(month, "month");
			return Of(year, month.Value);
		}

		/// <summary>
		/// Obtains an instance of {@code YearMonth} from a year and month.
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <returns> the year-month, not null </returns>
		/// <exception cref="DateTimeException"> if either field value is invalid </exception>
		public static YearMonth Of(int year, int month)
		{
			YEAR.checkValidValue(year);
			MONTH_OF_YEAR.checkValidValue(month);
			return new YearMonth(year, month);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code YearMonth} from a temporal object.
		/// <para>
		/// This obtains a year-month based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code YearMonth}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#YEAR YEAR"/> and
		/// <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> fields.
		/// The extraction is only permitted if the temporal object has an ISO
		/// chronology, or can be converted to a {@code LocalDate}.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code YearMonth::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the year-month, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code YearMonth} </exception>
		public static YearMonth From(TemporalAccessor temporal)
		{
			if (temporal is YearMonth)
			{
				return (YearMonth) temporal;
			}
			Objects.RequireNonNull(temporal, "temporal");
			try
			{
				if (IsoChronology.INSTANCE.Equals(Chronology.from(temporal)) == false)
				{
					temporal = LocalDate.From(temporal);
				}
				return Of(temporal.get(YEAR), temporal.get(MONTH_OF_YEAR));
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain YearMonth from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code YearMonth} from a text string such as {@code 2007-12}.
		/// <para>
		/// The string must represent a valid year-month.
		/// The format must be {@code uuuu-MM}.
		/// Years outside the range 0000 to 9999 must be prefixed by the plus or minus symbol.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007-12", not null </param>
		/// <returns> the parsed year-month, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static YearMonth Parse(CharSequence text)
		{
			return Parse(text, PARSER);
		}

		/// <summary>
		/// Obtains an instance of {@code YearMonth} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a year-month.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed year-month, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static YearMonth Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, YearMonth::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, validated from 1 (January) to 12 (December) </param>
		private YearMonth(int year, int month)
		{
			this.Year_Renamed = year;
			this.Month_Renamed = month;
		}

		/// <summary>
		/// Returns a copy of this year-month with the new year and month, checking
		/// to see if a new object is in fact required.
		/// </summary>
		/// <param name="newYear">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		/// <param name="newMonth">  the month-of-year to represent, validated not null </param>
		/// <returns> the year-month, not null </returns>
		private YearMonth With(int newYear, int newMonth)
		{
			if (Year_Renamed == newYear && Month_Renamed == newMonth)
			{
				return this;
			}
			return new YearMonth(newYear, newMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this year-month can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
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
		/// <returns> true if the field is supported on this year-month, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field == YEAR || field == MONTH_OF_YEAR || field == PROLEPTIC_MONTH || field == YEAR_OF_ERA || field == ERA;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this year-month.
		/// If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		/// <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		/// The supported units are:
		/// <ul>
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
		public bool IsSupported(TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				return unit == MONTHS || unit == YEARS || unit == DECADES || unit == CENTURIES || unit == MILLENNIA || unit == ERAS;
			}
			return unit != temporal.TemporalAccessor_Fields.Null && unit.isSupportedBy(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This year-month is used to enhance the accuracy of the returned range.
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
			if (field == YEAR_OF_ERA)
			{
				return (Year <= 0 ? ValueRange.Of(1, Year.MAX_VALUE + 1) : ValueRange.Of(1, Year.MAX_VALUE));
			}
			return Temporal.this.range(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this year-month as an {@code int}.
		/// <para>
		/// This queries this year-month for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this year-month, except {@code PROLEPTIC_MONTH} which is too
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
		public override int Get(TemporalField field) // override for Javadoc
		{
			return temporal.TemporalAccessor_Fields.range(field).checkValidIntValue(GetLong(field), field);
		}

		/// <summary>
		/// Gets the value of the specified field from this year-month as a {@code long}.
		/// <para>
		/// This queries this year-month for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this year-month.
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
					case MONTH_OF_YEAR:
						return Month_Renamed;
					case PROLEPTIC_MONTH:
						return ProlepticMonth;
					case YEAR_OF_ERA:
						return (Year_Renamed < 1 ? 1 - Year_Renamed : Year_Renamed);
					case YEAR:
						return Year_Renamed;
					case ERA:
						return (Year_Renamed < 1 ? 0 : 1);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

		private long ProlepticMonth
		{
			get
			{
				return (Year_Renamed * 12L + Month_Renamed - 1);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the year field.
		/// <para>
		/// This method returns the primitive {@code int} value for the year.
		/// </para>
		/// <para>
		/// The year returned by this method is proleptic as per {@code get(YEAR)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the year, from MIN_YEAR to MAX_YEAR </returns>
		public int Year
		{
			get
			{
				return Year_Renamed;
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
				return Month_Renamed;
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
				return Month.of(Month_Renamed);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the year is a leap year, according to the ISO proleptic
		/// calendar system rules.
		/// <para>
		/// This method applies the current rules for leap years across the whole time-line.
		/// In general, a year is a leap year if it is divisible by four without
		/// remainder. However, years divisible by 100, are not leap years, with
		/// the exception of years divisible by 400 which are.
		/// </para>
		/// <para>
		/// For example, 1904 is a leap year it is divisible by 4.
		/// 1900 was not a leap year as it is divisible by 100, however 2000 was a
		/// leap year as it is divisible by 400.
		/// </para>
		/// <para>
		/// The calculation is proleptic - applying the same rules into the far future and far past.
		/// This is historically inaccurate, but is correct for the ISO-8601 standard.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the year is leap, false otherwise </returns>
		public bool LeapYear
		{
			get
			{
				return IsoChronology.INSTANCE.IsLeapYear(Year_Renamed);
			}
		}

		/// <summary>
		/// Checks if the day-of-month is valid for this year-month.
		/// <para>
		/// This method checks whether this year and month and the input day form
		/// a valid date.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to validate, from 1 to 31, invalid value returns false </param>
		/// <returns> true if the day is valid for this year-month </returns>
		public bool IsValidDay(int dayOfMonth)
		{
			return dayOfMonth >= 1 && dayOfMonth <= LengthOfMonth();
		}

		/// <summary>
		/// Returns the length of the month, taking account of the year.
		/// <para>
		/// This returns the length of the month in days.
		/// For example, a date in January would return 31.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the month in days, from 28 to 31 </returns>
		public int LengthOfMonth()
		{
			return Month.length(LeapYear);
		}

		/// <summary>
		/// Returns the length of the year.
		/// <para>
		/// This returns the length of the year in days, either 365 or 366.
		/// 
		/// </para>
		/// </summary>
		/// <returns> 366 if the year is leap, 365 otherwise </returns>
		public int LengthOfYear()
		{
			return (LeapYear ? 366 : 365);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this year-month.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the year-month adjusted.
		/// The adjustment takes place using the specified adjuster strategy object.
		/// Read the documentation of the adjuster to understand what adjustment will be made.
		/// </para>
		/// <para>
		/// A simple adjuster might simply set the one of the fields, such as the year field.
		/// A more complex adjuster might set the year-month to the next month that
		/// Halley's comet will pass the Earth.
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
		/// <returns> a {@code YearMonth} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override YearMonth With(TemporalAdjuster adjuster)
		{
			return (YearMonth) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this year-month with the specified field set to a new value.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the value
		/// for the specified field changed.
		/// This can be used to change any supported field, such as the year or month.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code MONTH_OF_YEAR} -
		///  Returns a {@code YearMonth} with the specified month-of-year.
		///  The year will be unchanged.
		/// <li>{@code PROLEPTIC_MONTH} -
		///  Returns a {@code YearMonth} with the specified proleptic-month.
		///  This completely replaces the year and month of this object.
		/// <li>{@code YEAR_OF_ERA} -
		///  Returns a {@code YearMonth} with the specified year-of-era
		///  The month and era will be unchanged.
		/// <li>{@code YEAR} -
		///  Returns a {@code YearMonth} with the specified year.
		///  The month will be unchanged.
		/// <li>{@code ERA} -
		///  Returns a {@code YearMonth} with the specified era.
		///  The month and year-of-era will be unchanged.
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
		/// <returns> a {@code YearMonth} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public YearMonth With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				f.checkValidValue(newValue);
				switch (f)
				{
					case MONTH_OF_YEAR:
						return WithMonth((int) newValue);
					case PROLEPTIC_MONTH:
						return PlusMonths(newValue - ProlepticMonth);
					case YEAR_OF_ERA:
						return WithYear((int)(Year_Renamed < 1 ? 1 - newValue : newValue));
					case YEAR:
						return WithYear((int) newValue);
					case ERA:
						return (GetLong(ERA) == newValue ? this : WithYear(1 - Year_Renamed));
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the year altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the returned year-month, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> a {@code YearMonth} based on this year-month with the requested year, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public YearMonth WithYear(int year)
		{
			YEAR.checkValidValue(year);
			return With(year, Month_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the month-of-year altered.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the returned year-month, from 1 (January) to 12 (December) </param>
		/// <returns> a {@code YearMonth} based on this year-month with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public YearMonth WithMonth(int month)
		{
			MONTH_OF_YEAR.checkValidValue(month);
			return With(Year_Renamed, month);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this year-month with the specified amount added.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the specified amount added.
		/// The amount is typically <seealso cref="Period"/> but may be any other type implementing
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
		/// <returns> a {@code YearMonth} based on this year-month with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override YearMonth Plus(TemporalAmount amountToAdd)
		{
			return (YearMonth) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this year-month with the specified amount added.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code MONTHS} -
		///  Returns a {@code YearMonth} with the specified number of months added.
		///  This is equivalent to <seealso cref="#plusMonths(long)"/>.
		/// <li>{@code YEARS} -
		///  Returns a {@code YearMonth} with the specified number of years added.
		///  This is equivalent to <seealso cref="#plusYears(long)"/>.
		/// <li>{@code DECADES} -
		///  Returns a {@code YearMonth} with the specified number of decades added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 10.
		/// <li>{@code CENTURIES} -
		///  Returns a {@code YearMonth} with the specified number of centuries added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 100.
		/// <li>{@code MILLENNIA} -
		///  Returns a {@code YearMonth} with the specified number of millennia added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 1,000.
		/// <li>{@code ERAS} -
		///  Returns a {@code YearMonth} with the specified number of eras added.
		///  Only two eras are supported so the amount must be one, zero or minus one.
		///  If the amount is non-zero then the year is changed such that the year-of-era
		///  is unchanged.
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
		/// <returns> a {@code YearMonth} based on this year-month with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public YearMonth Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case MONTHS:
						return PlusMonths(amountToAdd);
					case YEARS:
						return PlusYears(amountToAdd);
					case DECADES:
						return PlusYears(Math.MultiplyExact(amountToAdd, 10));
					case CENTURIES:
						return PlusYears(Math.MultiplyExact(amountToAdd, 100));
					case MILLENNIA:
						return PlusYears(Math.MultiplyExact(amountToAdd, 1000));
					case ERAS:
						return With(ERA, Math.AddExact(GetLong(ERA), amountToAdd));
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.AddTo(this, amountToAdd);
		}

		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the specified number of years added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToAdd">  the years to add, may be negative </param>
		/// <returns> a {@code YearMonth} based on this year-month with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public YearMonth PlusYears(long yearsToAdd)
		{
			if (yearsToAdd == 0)
			{
				return this;
			}
			int newYear = YEAR.checkValidIntValue(Year_Renamed + yearsToAdd); // safe overflow
			return With(newYear, Month_Renamed);
		}

		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the specified number of months added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToAdd">  the months to add, may be negative </param>
		/// <returns> a {@code YearMonth} based on this year-month with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public YearMonth PlusMonths(long monthsToAdd)
		{
			if (monthsToAdd == 0)
			{
				return this;
			}
			long monthCount = Year_Renamed * 12L + (Month_Renamed - 1);
			long calcMonths = monthCount + monthsToAdd; // safe overflow
			int newYear = YEAR.checkValidIntValue(Math.FloorDiv(calcMonths, 12));
			int newMonth = (int)Math.FloorMod(calcMonths, 12) + 1;
			return With(newYear, newMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this year-month with the specified amount subtracted.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the specified amount subtracted.
		/// The amount is typically <seealso cref="Period"/> but may be any other type implementing
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
		/// <returns> a {@code YearMonth} based on this year-month with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override YearMonth Minus(TemporalAmount amountToSubtract)
		{
			return (YearMonth) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this year-month with the specified amount subtracted.
		/// <para>
		/// This returns a {@code YearMonth}, based on this one, with the amount
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
		/// <returns> a {@code YearMonth} based on this year-month with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override YearMonth Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the specified number of years subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		/// <returns> a {@code YearMonth} based on this year-month with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public YearMonth MinusYears(long yearsToSubtract)
		{
			return (yearsToSubtract == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-yearsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this {@code YearMonth} with the specified number of months subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		/// <returns> a {@code YearMonth} based on this year-month with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public YearMonth MinusMonths(long monthsToSubtract)
		{
			return (monthsToSubtract == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-monthsToSubtract));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this year-month using the specified query.
		/// <para>
		/// This queries this year-month using the specified query strategy object.
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
			if (query == TemporalQueries.Chronology())
			{
				return (R) IsoChronology.INSTANCE;
			}
			else if (query == TemporalQueries.Precision())
			{
				return (R) MONTHS;
			}
			return Temporal.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this year-month.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the year and month changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#PROLEPTIC_MONTH"/> as the field.
		/// If the specified temporal object does not use the ISO calendar system then
		/// a {@code DateTimeException} is thrown.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisYearMonth.adjustInto(temporal);
		///   temporal = temporal.with(thisYearMonth);
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
			if (Chronology.from(temporal).Equals(IsoChronology.INSTANCE) == false)
			{
				throw new DateTimeException("Adjustment only supported on ISO date-time");
			}
			return temporal.With(PROLEPTIC_MONTH, ProlepticMonth);
		}

		/// <summary>
		/// Calculates the amount of time until another year-month in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code YearMonth}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified year-month.
		/// The result will be negative if the end is before the start.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code YearMonth} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in years between two year-months can be calculated
		/// using {@code startYearMonth.until(endYearMonth, YEARS)}.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two year-months.
		/// For example, the amount in decades between 2012-06 and 2032-05
		/// will only be one decade as it is one month short of two decades.
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
		/// The units {@code MONTHS}, {@code YEARS}, {@code DECADES},
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to a {@code YearMonth}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this year-month and the end year-month </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code YearMonth} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			YearMonth end = YearMonth.From(endExclusive);
			if (unit is ChronoUnit)
			{
				long monthsUntil = end.ProlepticMonth - ProlepticMonth; // no overflow
				switch ((ChronoUnit) unit)
				{
					case MONTHS:
						return monthsUntil;
					case YEARS:
						return monthsUntil / 12;
					case DECADES:
						return monthsUntil / 120;
					case CENTURIES:
						return monthsUntil / 1200;
					case MILLENNIA:
						return monthsUntil / 12000;
					case ERAS:
						return end.GetLong(ERA) - GetLong(ERA);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.Between(this, end);
		}

		/// <summary>
		/// Formats this year-month using the specified formatter.
		/// <para>
		/// This year-month will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted year-month string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public String Format(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this year-month with a day-of-month to create a {@code LocalDate}.
		/// <para>
		/// This returns a {@code LocalDate} formed from this year-month and the specified day-of-month.
		/// </para>
		/// <para>
		/// The day-of-month value must be valid for the year-month.
		/// </para>
		/// <para>
		/// This method can be used as part of a chain to produce a date:
		/// <pre>
		///  LocalDate date = year.atMonth(month).atDay(day);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to use, from 1 to 31 </param>
		/// <returns> the date formed from this year-month and the specified day, not null </returns>
		/// <exception cref="DateTimeException"> if the day is invalid for the year-month </exception>
		/// <seealso cref= #isValidDay(int) </seealso>
		public LocalDate AtDay(int dayOfMonth)
		{
			return LocalDate.Of(Year_Renamed, Month_Renamed, dayOfMonth);
		}

		/// <summary>
		/// Returns a {@code LocalDate} at the end of the month.
		/// <para>
		/// This returns a {@code LocalDate} based on this year-month.
		/// The day-of-month is set to the last valid day of the month, taking
		/// into account leap years.
		/// </para>
		/// <para>
		/// This method can be used as part of a chain to produce a date:
		/// <pre>
		///  LocalDate date = year.atMonth(month).atEndOfMonth();
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the last valid date of this year-month, not null </returns>
		public LocalDate AtEndOfMonth()
		{
			return LocalDate.Of(Year_Renamed, Month_Renamed, LengthOfMonth());
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this year-month to another year-month.
		/// <para>
		/// The comparison is based first on the value of the year, then on the value of the month.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other year-month to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(YearMonth other)
		{
			int cmp = (Year_Renamed - other.Year_Renamed);
			if (cmp == 0)
			{
				cmp = (Month_Renamed - other.Month_Renamed);
			}
			return cmp;
		}

		/// <summary>
		/// Checks if this year-month is after the specified year-month.
		/// </summary>
		/// <param name="other">  the other year-month to compare to, not null </param>
		/// <returns> true if this is after the specified year-month </returns>
		public bool IsAfter(YearMonth other)
		{
			return CompareTo(other) > 0;
		}

		/// <summary>
		/// Checks if this year-month is before the specified year-month.
		/// </summary>
		/// <param name="other">  the other year-month to compare to, not null </param>
		/// <returns> true if this point is before the specified year-month </returns>
		public bool IsBefore(YearMonth other)
		{
			return CompareTo(other) < 0;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this year-month is equal to another year-month.
		/// <para>
		/// The comparison is based on the time-line position of the year-months.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other year-month </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is YearMonth)
			{
				YearMonth other = (YearMonth) obj;
				return Year_Renamed == other.Year_Renamed && Month_Renamed == other.Month_Renamed;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this year-month.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Year_Renamed ^ (Month_Renamed << 27);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this year-month as a {@code String}, such as {@code 2007-12}.
		/// <para>
		/// The output will be in the format {@code uuuu-MM}:
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this year-month, not null </returns>
		public override String ToString()
		{
			int absYear = System.Math.Abs(Year_Renamed);
			StringBuilder buf = new StringBuilder(9);
			if (absYear < 1000)
			{
				if (Year_Renamed < 0)
				{
					buf.Append(Year_Renamed - 10000).DeleteCharAt(1);
				}
				else
				{
					buf.Append(Year_Renamed + 10000).DeleteCharAt(0);
				}
			}
			else
			{
				buf.Append(Year_Renamed);
			}
			return buf.Append(Month_Renamed < 10 ? "-0" : "-").Append(Month_Renamed).ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(12);  // identifies a YearMonth
		///  out.writeInt(year);
		///  out.writeByte(month);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.YEAR_MONTH_TYPE, this);
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
			@out.WriteInt(Year_Renamed);
			@out.WriteByte(Month_Renamed);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static YearMonth readExternal(java.io.DataInput in) throws java.io.IOException
		internal static YearMonth ReadExternal(DataInput @in)
		{
			int year = @in.ReadInt();
			sbyte month = @in.ReadByte();
			return YearMonth.Of(year, month);
		}

	}

}