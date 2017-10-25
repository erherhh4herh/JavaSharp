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
	/// A year in the ISO-8601 calendar system, such as {@code 2007}.
	/// <para>
	/// {@code Year} is an immutable date-time object that represents a year.
	/// Any field that can be derived from a year can be obtained.
	/// </para>
	/// <para>
	/// <b>Note that years in the ISO chronology only align with years in the
	/// Gregorian-Julian system for modern years. Parts of Russia did not switch to the
	/// modern Gregorian/ISO rules until 1920.
	/// As such, historical years must be treated with caution.</b>
	/// </para>
	/// <para>
	/// This class does not store or represent a month, day, time or time-zone.
	/// For example, the value "2007" can be stored in a {@code Year}.
	/// </para>
	/// <para>
	/// Years represented by this class follow the ISO-8601 standard and use
	/// the proleptic numbering system. Year 1 is preceded by year 0, then by year -1.
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
	/// {@code Year} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Year : Temporal, TemporalAdjuster, Comparable<Year>
	{

		/// <summary>
		/// The minimum supported year, '-999,999,999'.
		/// </summary>
		public const int MIN_VALUE = -999999999;
		/// <summary>
		/// The maximum supported year, '+999,999,999'.
		/// </summary>
		public const int MAX_VALUE = 999999999;

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -23038383694477807L;
		/// <summary>
		/// Parser.
		/// </summary>
		private static readonly DateTimeFormatter PARSER = new DateTimeFormatterBuilder().AppendValue(YEAR, 4, 10, SignStyle.EXCEEDS_PAD).ToFormatter();

		/// <summary>
		/// The year being represented.
		/// </summary>
		private readonly int Year_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current year from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current year.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current year using the system clock and default time-zone, not null </returns>
		public static Year Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current year from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current year.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current year using the system clock, not null </returns>
		public static Year Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current year from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current year.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current year, not null </returns>
		public static Year Now(Clock clock)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocalDate now = LocalDate.now(clock);
			LocalDate now = LocalDate.Now(clock); // called once
			return Year.Of(now.Year);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Year}.
		/// <para>
		/// This method accepts a year value from the proleptic ISO calendar system.
		/// </para>
		/// <para>
		/// The year 2AD/CE is represented by 2.<br>
		/// The year 1AD/CE is represented by 1.<br>
		/// The year 1BC/BCE is represented by 0.<br>
		/// The year 2BC/BCE is represented by -1.<br>
		/// 
		/// </para>
		/// </summary>
		/// <param name="isoYear">  the ISO proleptic year to represent, from {@code MIN_VALUE} to {@code MAX_VALUE} </param>
		/// <returns> the year, not null </returns>
		/// <exception cref="DateTimeException"> if the field is invalid </exception>
		public static Year Of(int isoYear)
		{
			YEAR.checkValidValue(isoYear);
			return new Year(isoYear);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Year} from a temporal object.
		/// <para>
		/// This obtains a year based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code Year}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#YEAR year"/> field.
		/// The extraction is only permitted if the temporal object has an ISO
		/// chronology, or can be converted to a {@code LocalDate}.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code Year::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the year, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code Year} </exception>
		public static Year From(TemporalAccessor temporal)
		{
			if (temporal is Year)
			{
				return (Year) temporal;
			}
			Objects.RequireNonNull(temporal, "temporal");
			try
			{
				if (IsoChronology.INSTANCE.Equals(Chronology.from(temporal)) == false)
				{
					temporal = LocalDate.From(temporal);
				}
				return Of(temporal.get(YEAR));
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain Year from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Year} from a text string such as {@code 2007}.
		/// <para>
		/// The string must represent a valid year.
		/// Years outside the range 0000 to 9999 must be prefixed by the plus or minus symbol.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007", not null </param>
		/// <returns> the parsed year, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static Year Parse(CharSequence text)
		{
			return Parse(text, PARSER);
		}

		/// <summary>
		/// Obtains an instance of {@code Year} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a year.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed year, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static Year Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, Year::from);
		}

		//-------------------------------------------------------------------------
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
		/// <param name="year">  the year to check </param>
		/// <returns> true if the year is leap, false otherwise </returns>
		public static bool IsLeap(long year)
		{
			return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="year">  the year to represent </param>
		private Year(int year)
		{
			this.Year_Renamed = year;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the year value.
		/// <para>
		/// The year returned by this method is proleptic as per {@code get(YEAR)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the year, {@code MIN_VALUE} to {@code MAX_VALUE} </returns>
		public int Value
		{
			get
			{
				return Year_Renamed;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this year can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
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
		/// <returns> true if the field is supported on this year, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field == YEAR || field == YEAR_OF_ERA || field == ERA;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this year.
		/// If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		/// <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		/// The supported units are:
		/// <ul>
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
				return unit == YEARS || unit == DECADES || unit == CENTURIES || unit == MILLENNIA || unit == ERAS;
			}
			return unit != temporal.TemporalAccessor_Fields.Null && unit.isSupportedBy(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This year is used to enhance the accuracy of the returned range.
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
				return (Year_Renamed <= 0 ? ValueRange.Of(1, MAX_VALUE + 1) : ValueRange.Of(1, MAX_VALUE));
			}
			return Temporal.this.range(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this year as an {@code int}.
		/// <para>
		/// This queries this year for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this year.
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
		/// Gets the value of the specified field from this year as a {@code long}.
		/// <para>
		/// This queries this year for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this year.
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
		public bool Leap
		{
			get
			{
				return Year.IsLeap(Year_Renamed);
			}
		}

		/// <summary>
		/// Checks if the month-day is valid for this year.
		/// <para>
		/// This method checks whether this year and the input month and day form
		/// a valid date.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthDay">  the month-day to validate, null returns false </param>
		/// <returns> true if the month and day are valid for this year </returns>
		public bool IsValidMonthDay(MonthDay monthDay)
		{
			return monthDay != temporal.TemporalAccessor_Fields.Null && monthDay.IsValidYear(Year_Renamed);
		}

		/// <summary>
		/// Gets the length of this year in days.
		/// </summary>
		/// <returns> the length of this year in days, 365 or 366 </returns>
		public int Length()
		{
			return Leap ? 366 : 365;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this year.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the year adjusted.
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
		/// <returns> a {@code Year} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Year With(TemporalAdjuster adjuster)
		{
			return (Year) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this year with the specified field set to a new value.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the value
		/// for the specified field changed.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code YEAR_OF_ERA} -
		///  Returns a {@code Year} with the specified year-of-era
		///  The era will be unchanged.
		/// <li>{@code YEAR} -
		///  Returns a {@code Year} with the specified year.
		///  This completely replaces the date and is equivalent to <seealso cref="#of(int)"/>.
		/// <li>{@code ERA} -
		///  Returns a {@code Year} with the specified era.
		///  The year-of-era will be unchanged.
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
		/// <returns> a {@code Year} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Year With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				f.checkValidValue(newValue);
				switch (f)
				{
					case YEAR_OF_ERA:
						return Year.Of((int)(Year_Renamed < 1 ? 1 - newValue : newValue));
					case YEAR:
						return Year.Of((int) newValue);
					case ERA:
						return (GetLong(ERA) == newValue ? this : Year.Of(1 - Year_Renamed));
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.AdjustInto(this, newValue);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this year with the specified amount added.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the specified amount added.
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
		/// <returns> a {@code Year} based on this year with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Year Plus(TemporalAmount amountToAdd)
		{
			return (Year) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this year with the specified amount added.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code YEARS} -
		///  Returns a {@code Year} with the specified number of years added.
		///  This is equivalent to <seealso cref="#plusYears(long)"/>.
		/// <li>{@code DECADES} -
		///  Returns a {@code Year} with the specified number of decades added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 10.
		/// <li>{@code CENTURIES} -
		///  Returns a {@code Year} with the specified number of centuries added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 100.
		/// <li>{@code MILLENNIA} -
		///  Returns a {@code Year} with the specified number of millennia added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 1,000.
		/// <li>{@code ERAS} -
		///  Returns a {@code Year} with the specified number of eras added.
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
		/// <returns> a {@code Year} based on this year with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Year Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
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
		/// Returns a copy of this {@code Year} with the specified number of years added.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToAdd">  the years to add, may be negative </param>
		/// <returns> a {@code Year} based on this year with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public Year PlusYears(long yearsToAdd)
		{
			if (yearsToAdd == 0)
			{
				return this;
			}
			return Of(YEAR.checkValidIntValue(Year_Renamed + yearsToAdd)); // overflow safe
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this year with the specified amount subtracted.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the specified amount subtracted.
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
		/// <returns> a {@code Year} based on this year with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Year Minus(TemporalAmount amountToSubtract)
		{
			return (Year) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this year with the specified amount subtracted.
		/// <para>
		/// This returns a {@code Year}, based on this one, with the amount
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
		/// <returns> a {@code Year} based on this year with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override Year Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		/// <summary>
		/// Returns a copy of this {@code Year} with the specified number of years subtracted.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		/// <returns> a {@code Year} based on this year with the year subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public Year MinusYears(long yearsToSubtract)
		{
			return (yearsToSubtract == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-yearsToSubtract));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this year using the specified query.
		/// <para>
		/// This queries this year using the specified query strategy object.
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
				return (R) YEARS;
			}
			return Temporal.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this year.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the year changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#YEAR"/> as the field.
		/// If the specified temporal object does not use the ISO calendar system then
		/// a {@code DateTimeException} is thrown.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisYear.adjustInto(temporal);
		///   temporal = temporal.with(thisYear);
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
			return temporal.With(YEAR, Year_Renamed);
		}

		/// <summary>
		/// Calculates the amount of time until another year in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code Year}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified year.
		/// The result will be negative if the end is before the start.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code Year} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in decades between two year can be calculated
		/// using {@code startYear.until(endYear, DECADES)}.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two years.
		/// For example, the amount in decades between 2012 and 2031
		/// will only be one decade as it is one year short of two decades.
		/// </para>
		/// <para>
		/// There are two equivalent ways of using this method.
		/// The first is to invoke this method.
		/// The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		/// <pre>
		///   // these two lines are equivalent
		///   amount = start.until(end, YEARS);
		///   amount = YEARS.between(start, end);
		/// </pre>
		/// The choice should be made based on which makes the code more readable.
		/// </para>
		/// <para>
		/// The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		/// The units {@code YEARS}, {@code DECADES}, {@code CENTURIES},
		/// {@code MILLENNIA} and {@code ERAS} are supported.
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to a {@code Year}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this year and the end year </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code Year} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			Year end = Year.From(endExclusive);
			if (unit is ChronoUnit)
			{
				long yearsUntil = ((long) end.Year_Renamed) - Year_Renamed; // no overflow
				switch ((ChronoUnit) unit)
				{
					case YEARS:
						return yearsUntil;
					case DECADES:
						return yearsUntil / 10;
					case CENTURIES:
						return yearsUntil / 100;
					case MILLENNIA:
						return yearsUntil / 1000;
					case ERAS:
						return end.GetLong(ERA) - GetLong(ERA);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.Between(this, end);
		}

		/// <summary>
		/// Formats this year using the specified formatter.
		/// <para>
		/// This year will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted year string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public String Format(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this year with a day-of-year to create a {@code LocalDate}.
		/// <para>
		/// This returns a {@code LocalDate} formed from this year and the specified day-of-year.
		/// </para>
		/// <para>
		/// The day-of-year value 366 is only valid in a leap year.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfYear">  the day-of-year to use, from 1 to 365-366 </param>
		/// <returns> the local date formed from this year and the specified date of year, not null </returns>
		/// <exception cref="DateTimeException"> if the day of year is zero or less, 366 or greater or equal
		///  to 366 and this is not a leap year </exception>
		public LocalDate AtDay(int dayOfYear)
		{
			return LocalDate.OfYearDay(Year_Renamed, dayOfYear);
		}

		/// <summary>
		/// Combines this year with a month to create a {@code YearMonth}.
		/// <para>
		/// This returns a {@code YearMonth} formed from this year and the specified month.
		/// All possible combinations of year and month are valid.
		/// </para>
		/// <para>
		/// This method can be used as part of a chain to produce a date:
		/// <pre>
		///  LocalDate date = year.atMonth(month).atDay(day);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to use, not null </param>
		/// <returns> the year-month formed from this year and the specified month, not null </returns>
		public YearMonth AtMonth(Month month)
		{
			return YearMonth.Of(Year_Renamed, month);
		}

		/// <summary>
		/// Combines this year with a month to create a {@code YearMonth}.
		/// <para>
		/// This returns a {@code YearMonth} formed from this year and the specified month.
		/// All possible combinations of year and month are valid.
		/// </para>
		/// <para>
		/// This method can be used as part of a chain to produce a date:
		/// <pre>
		///  LocalDate date = year.atMonth(month).atDay(day);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to use, from 1 (January) to 12 (December) </param>
		/// <returns> the year-month formed from this year and the specified month, not null </returns>
		/// <exception cref="DateTimeException"> if the month is invalid </exception>
		public YearMonth AtMonth(int month)
		{
			return YearMonth.Of(Year_Renamed, month);
		}

		/// <summary>
		/// Combines this year with a month-day to create a {@code LocalDate}.
		/// <para>
		/// This returns a {@code LocalDate} formed from this year and the specified month-day.
		/// </para>
		/// <para>
		/// A month-day of February 29th will be adjusted to February 28th in the resulting
		/// date if the year is not a leap year.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthDay">  the month-day to use, not null </param>
		/// <returns> the local date formed from this year and the specified month-day, not null </returns>
		public LocalDate AtMonthDay(MonthDay monthDay)
		{
			return monthDay.AtYear(Year_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this year to another year.
		/// <para>
		/// The comparison is based on the value of the year.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other year to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(Year other)
		{
			return Year_Renamed - other.Year_Renamed;
		}

		/// <summary>
		/// Checks if this year is after the specified year.
		/// </summary>
		/// <param name="other">  the other year to compare to, not null </param>
		/// <returns> true if this is after the specified year </returns>
		public bool IsAfter(Year other)
		{
			return Year_Renamed > other.Year_Renamed;
		}

		/// <summary>
		/// Checks if this year is before the specified year.
		/// </summary>
		/// <param name="other">  the other year to compare to, not null </param>
		/// <returns> true if this point is before the specified year </returns>
		public bool IsBefore(Year other)
		{
			return Year_Renamed < other.Year_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this year is equal to another year.
		/// <para>
		/// The comparison is based on the time-line position of the years.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other year </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is Year)
			{
				return Year_Renamed == ((Year) obj).Year_Renamed;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this year.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Year_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this year as a {@code String}.
		/// </summary>
		/// <returns> a string representation of this year, not null </returns>
		public override String ToString()
		{
			return Convert.ToString(Year_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(11);  // identifies a Year
		///  out.writeInt(year);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.YEAR_TYPE, this);
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
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Year readExternal(java.io.DataInput in) throws java.io.IOException
		internal static Year ReadExternal(DataInput @in)
		{
			return Year.Of(@in.ReadInt());
		}

	}

}