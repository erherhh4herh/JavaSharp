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
	/// A date without a time-zone in the ISO-8601 calendar system,
	/// such as {@code 2007-12-03}.
	/// <para>
	/// {@code LocalDate} is an immutable date-time object that represents a date,
	/// often viewed as year-month-day. Other date fields, such as day-of-year,
	/// day-of-week and week-of-year, can also be accessed.
	/// For example, the value "2nd October 2007" can be stored in a {@code LocalDate}.
	/// </para>
	/// <para>
	/// This class does not store or represent a time or time-zone.
	/// Instead, it is a description of the date, as used for birthdays.
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
	/// {@code LocalDate} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class LocalDate : Temporal, TemporalAdjuster, ChronoLocalDate
	{

		/// <summary>
		/// The minimum supported {@code LocalDate}, '-999999999-01-01'.
		/// This could be used by an application as a "far past" date.
		/// </summary>
		public static readonly LocalDate MIN = LocalDate.Of(Year.MIN_VALUE, 1, 1);
		/// <summary>
		/// The maximum supported {@code LocalDate}, '+999999999-12-31'.
		/// This could be used by an application as a "far future" date.
		/// </summary>
		public static readonly LocalDate MAX = LocalDate.Of(Year.MAX_VALUE, 12, 31);

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 2942565459149668126L;
		/// <summary>
		/// The number of days in a 400 year cycle.
		/// </summary>
		private const int DAYS_PER_CYCLE = 146097;
		/// <summary>
		/// The number of days from year zero to year 1970.
		/// There are five 400 year cycles from year zero to 2000.
		/// There are 7 leap years from 1970 to 2000.
		/// </summary>
		internal static readonly long DAYS_0000_TO_1970 = (DAYS_PER_CYCLE * 5L) - (30L * 365L + 7L);

		/// <summary>
		/// The year.
		/// </summary>
		private readonly int Year_Renamed;
		/// <summary>
		/// The month-of-year.
		/// </summary>
		private readonly short Month_Renamed;
		/// <summary>
		/// The day-of-month.
		/// </summary>
		private readonly short Day;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current date from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current date.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current date using the system clock and default time-zone, not null </returns>
		public static LocalDate Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current date from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current date using the system clock, not null </returns>
		public static LocalDate Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current date from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date - today.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date, not null </returns>
		public static LocalDate Now(Clock clock)
		{
			Objects.RequireNonNull(clock, "clock");
			// inline to avoid creating object and Instant checks
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Instant now = clock.instant();
			Instant now = clock.Instant(); // called once
			ZoneOffset offset = clock.Zone.Rules.GetOffset(now);
			long epochSec = now.EpochSecond + offset.TotalSeconds; // overflow caught later
			long epochDay = Math.FloorDiv(epochSec, SECONDS_PER_DAY);
			return LocalDate.OfEpochDay(epochDay);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a year, month and day.
		/// <para>
		/// This returns a {@code LocalDate} with the specified year, month and day-of-month.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDate Of(int year, Month month, int dayOfMonth)
		{
			YEAR.checkValidValue(year);
			Objects.RequireNonNull(month, "month");
			DAY_OF_MONTH.checkValidValue(dayOfMonth);
			return Create(year, month.Value, dayOfMonth);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a year, month and day.
		/// <para>
		/// This returns a {@code LocalDate} with the specified year, month and day-of-month.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static LocalDate Of(int year, int month, int dayOfMonth)
		{
			YEAR.checkValidValue(year);
			MONTH_OF_YEAR.checkValidValue(month);
			DAY_OF_MONTH.checkValidValue(dayOfMonth);
			return Create(year, month, dayOfMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a year and day-of-year.
		/// <para>
		/// This returns a {@code LocalDate} with the specified year and day-of-year.
		/// The day-of-year must be valid for the year, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="dayOfYear">  the day-of-year to represent, from 1 to 366 </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-year is invalid for the year </exception>
		public static LocalDate OfYearDay(int year, int dayOfYear)
		{
			YEAR.checkValidValue(year);
			DAY_OF_YEAR.checkValidValue(dayOfYear);
			bool leap = IsoChronology.INSTANCE.IsLeapYear(year);
			if (dayOfYear == 366 && leap == false)
			{
				throw new DateTimeException("Invalid date 'DayOfYear 366' as '" + year + "' is not a leap year");
			}
			Month moy = Month.of((dayOfYear - 1) / 31 + 1);
			int monthEnd = moy.firstDayOfYear(leap) + moy.length(leap) - 1;
			if (dayOfYear > monthEnd)
			{
				moy = moy.plus(1);
			}
			int dom = dayOfYear - moy.firstDayOfYear(leap) + 1;
			return new LocalDate(year, moy.Value, dom);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDate} from the epoch day count.
		/// <para>
		/// This returns a {@code LocalDate} with the specified epoch-day.
		/// The <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> is a simple incrementing count
		/// of days where day 0 is 1970-01-01. Negative numbers represent earlier days.
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochDay">  the Epoch Day to convert, based on the epoch 1970-01-01 </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if the epoch day exceeds the supported date range </exception>
		public static LocalDate OfEpochDay(long epochDay)
		{
			long zeroDay = epochDay + DAYS_0000_TO_1970;
			// find the march-based year
			zeroDay -= 60; // adjust to 0000-03-01 so leap day is at end of four year cycle
			long adjust = 0;
			if (zeroDay < 0)
			{
				// adjust negative years to positive for calculation
				long adjustCycles = (zeroDay + 1) / DAYS_PER_CYCLE - 1;
				adjust = adjustCycles * 400;
				zeroDay += -adjustCycles * DAYS_PER_CYCLE;
			}
			long yearEst = (400 * zeroDay + 591) / DAYS_PER_CYCLE;
			long doyEst = zeroDay - (365 * yearEst + yearEst / 4 - yearEst / 100 + yearEst / 400);
			if (doyEst < 0)
			{
				// fix estimate
				yearEst--;
				doyEst = zeroDay - (365 * yearEst + yearEst / 4 - yearEst / 100 + yearEst / 400);
			}
			yearEst += adjust; // reset any negative year
			int marchDoy0 = (int) doyEst;

			// convert march-based values back to january-based
			int marchMonth0 = (marchDoy0 * 5 + 2) / 153;
			int month = (marchMonth0 + 2) % 12 + 1;
			int dom = marchDoy0 - (marchMonth0 * 306 + 5) / 10 + 1;
			yearEst += marchMonth0 / 10;

			// check year now we are certain it is correct
			int year = YEAR.checkValidIntValue(yearEst);
			return new LocalDate(year, month, dom);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a temporal object.
		/// <para>
		/// This obtains a local date based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code LocalDate}.
		/// </para>
		/// <para>
		/// The conversion uses the <seealso cref="TemporalQueries#localDate()"/> query, which relies
		/// on extracting the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> field.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code LocalDate::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code LocalDate} </exception>
		public static LocalDate From(TemporalAccessor temporal)
		{
			Objects.RequireNonNull(temporal, "temporal");
			LocalDate date = temporal.query(TemporalQueries.LocalDate());
			if (date == chrono.ChronoLocalDate_Fields.Null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain LocalDate from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName);
			}
			return date;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a text string such as {@code 2007-12-03}.
		/// <para>
		/// The string must represent a valid date and is parsed using
		/// <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_DATE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "2007-12-03", not null </param>
		/// <returns> the parsed local date, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalDate Parse(CharSequence text)
		{
			return Parse(text, DateTimeFormatter.ISO_LOCAL_DATE);
		}

		/// <summary>
		/// Obtains an instance of {@code LocalDate} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a date.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed local date, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static LocalDate Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, LocalDate::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates a local date from the year, month and day fields.
		/// </summary>
		/// <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, from 1 to 12, validated </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, validated from 1 to 31 </param>
		/// <returns> the local date, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month is invalid for the month-year </exception>
		private static LocalDate Create(int year, int month, int dayOfMonth)
		{
			if (dayOfMonth > 28)
			{
				int dom = 31;
				switch (month)
				{
					case 2:
						dom = (IsoChronology.INSTANCE.IsLeapYear(year) ? 29 : 28);
						break;
					case 4:
					case 6:
					case 9:
					case 11:
						dom = 30;
						break;
				}
				if (dayOfMonth > dom)
				{
					if (dayOfMonth == 29)
					{
						throw new DateTimeException("Invalid date 'February 29' as '" + year + "' is not a leap year");
					}
					else
					{
						throw new DateTimeException("Invalid date '" + Month.of(month).name() + " " + dayOfMonth + "'");
					}
				}
			}
			return new LocalDate(year, month, dayOfMonth);
		}

		/// <summary>
		/// Resolves the date, resolving days past the end of month.
		/// </summary>
		/// <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, validated from 1 to 12 </param>
		/// <param name="day">  the day-of-month to represent, validated from 1 to 31 </param>
		/// <returns> the resolved date, not null </returns>
		private static LocalDate ResolvePreviousValid(int year, int month, int day)
		{
			switch (month)
			{
				case 2:
					day = System.Math.Min(day, IsoChronology.INSTANCE.IsLeapYear(year) ? 29 : 28);
					break;
				case 4:
				case 6:
				case 9:
				case 11:
					day = System.Math.Min(day, 30);
					break;
			}
			return new LocalDate(year, month, day);
		}

		/// <summary>
		/// Constructor, previously validated.
		/// </summary>
		/// <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, valid for year-month, from 1 to 31 </param>
		private LocalDate(int year, int month, int dayOfMonth)
		{
			this.Year_Renamed = year;
			this.Month_Renamed = (short) month;
			this.Day = (short) dayOfMonth;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this date can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
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
		/// <returns> true if the field is supported on this date, false if not </returns>
		public bool IsSupported(TemporalField field) // override for Javadoc
		{
			return ChronoLocalDate.this.isSupported(field);
		}

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to, or subtracted from, this date.
		/// If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		/// <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		/// The supported units are:
		/// <ul>
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
			return ChronoLocalDate.this.isSupported(unit);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This date is used to enhance the accuracy of the returned range.
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
				if (f.DateBased)
				{
					switch (f)
					{
						case DAY_OF_MONTH:
							return ValueRange.Of(1, LengthOfMonth());
						case DAY_OF_YEAR:
							return ValueRange.Of(1, LengthOfYear());
						case ALIGNED_WEEK_OF_MONTH:
							return ValueRange.Of(1, Month == Month.FEBRUARY && LeapYear == false ? 4 : 5);
						case YEAR_OF_ERA:
							return (Year <= 0 ? ValueRange.Of(1, Year.MAX_VALUE + 1) : ValueRange.Of(1, Year.MAX_VALUE));
					}
					return field.Range();
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.RangeRefinedBy(this);
		}

		/// <summary>
		/// Gets the value of the specified field from this date as an {@code int}.
		/// <para>
		/// This queries this date for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this date, except {@code EPOCH_DAY} and {@code PROLEPTIC_MONTH}
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
			return ChronoLocalDate.this.get(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this date as a {@code long}.
		/// <para>
		/// This queries this date for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this date.
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
				if (field == EPOCH_DAY)
				{
					return ToEpochDay();
				}
				if (field == PROLEPTIC_MONTH)
				{
					return ProlepticMonth;
				}
				return Get0(field);
			}
			return field.GetFrom(this);
		}

		private int Get0(TemporalField field)
		{
			switch ((ChronoField) field)
			{
				case DAY_OF_WEEK:
					return DayOfWeek.Value;
				case ALIGNED_DAY_OF_WEEK_IN_MONTH:
					return ((Day - 1) % 7) + 1;
				case ALIGNED_DAY_OF_WEEK_IN_YEAR:
					return ((DayOfYear - 1) % 7) + 1;
				case DAY_OF_MONTH:
					return Day;
				case DAY_OF_YEAR:
					return DayOfYear;
				case EPOCH_DAY:
					throw new UnsupportedTemporalTypeException("Invalid field 'EpochDay' for get() method, use getLong() instead");
				case ALIGNED_WEEK_OF_MONTH:
					return ((Day - 1) / 7) + 1;
				case ALIGNED_WEEK_OF_YEAR:
					return ((DayOfYear - 1) / 7) + 1;
				case MONTH_OF_YEAR:
					return Month_Renamed;
				case PROLEPTIC_MONTH:
					throw new UnsupportedTemporalTypeException("Invalid field 'ProlepticMonth' for get() method, use getLong() instead");
				case YEAR_OF_ERA:
					return (Year_Renamed >= 1 ? Year_Renamed : 1 - Year_Renamed);
				case YEAR:
					return Year_Renamed;
				case ERA:
					return (Year_Renamed >= 1 ? 1 : 0);
			}
			throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
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
		/// Gets the chronology of this date, which is the ISO calendar system.
		/// <para>
		/// The {@code Chronology} represents the calendar system in use.
		/// The ISO-8601 calendar system is the modern civil calendar system used today
		/// in most of the world. It is equivalent to the proleptic Gregorian calendar
		/// system, in which today's rules for leap years are applied for all time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the ISO chronology, not null </returns>
		public IsoChronology Chronology
		{
			get
			{
				return IsoChronology.INSTANCE;
			}
		}

		/// <summary>
		/// Gets the era applicable at this date.
		/// <para>
		/// The official ISO-8601 standard does not define eras, however {@code IsoChronology} does.
		/// It defines two eras, 'CE' from year one onwards and 'BCE' from year zero backwards.
		/// Since dates before the Julian-Gregorian cutover are not in line with history,
		/// the cutover between 'BCE' and 'CE' is also not aligned with the commonly used
		/// eras, often referred to using 'BC' and 'AD'.
		/// </para>
		/// <para>
		/// Users of this class should typically ignore this method as it exists primarily
		/// to fulfill the <seealso cref="ChronoLocalDate"/> contract where it is necessary to support
		/// the Japanese calendar system.
		/// </para>
		/// <para>
		/// The returned era will be a singleton capable of being compared with the constants
		/// in <seealso cref="IsoChronology"/> using the {@code ==} operator.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code IsoChronology} era constant applicable at this date, not null </returns>
		public override Era Era
		{
			get
			{
				return ChronoLocalDate.this.Era;
			}
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
				return Day;
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
				return Month.firstDayOfYear(LeapYear) + Day - 1;
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
				int dow0 = (int)Math.FloorMod(ToEpochDay() + 3, 7);
				return DayOfWeek.of(dow0 + 1);
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
		public override bool LeapYear
		{
			get
			{
				return IsoChronology.INSTANCE.IsLeapYear(Year_Renamed);
			}
		}

		/// <summary>
		/// Returns the length of the month represented by this date.
		/// <para>
		/// This returns the length of the month in days.
		/// For example, a date in January would return 31.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the month in days </returns>
		public int LengthOfMonth()
		{
			switch (Month_Renamed)
			{
				case 2:
					return (LeapYear ? 29 : 28);
				case 4:
				case 6:
				case 9:
				case 11:
					return 30;
				default:
					return 31;
			}
		}

		/// <summary>
		/// Returns the length of the year represented by this date.
		/// <para>
		/// This returns the length of the year in days, either 365 or 366.
		/// 
		/// </para>
		/// </summary>
		/// <returns> 366 if the year is leap, 365 otherwise </returns>
		public override int LengthOfYear() // override for Javadoc and performance
		{
			return (LeapYear ? 366 : 365);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an adjusted copy of this date.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the date adjusted.
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
		///  result = localDate.with(JULY).with(lastDayOfMonth());
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
		/// <returns> a {@code LocalDate} based on {@code this} with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDate With(TemporalAdjuster adjuster)
		{
			// optimizations
			if (adjuster is LocalDate)
			{
				return (LocalDate) adjuster;
			}
			return (LocalDate) adjuster.AdjustInto(this);
		}

		/// <summary>
		/// Returns a copy of this date with the specified field set to a new value.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the value
		/// for the specified field changed.
		/// This can be used to change any supported field, such as the year, month or day-of-month.
		/// If it is not possible to set the value, because the field is not supported or for
		/// some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// In some cases, changing the specified field can cause the resulting date to become invalid,
		/// such as changing the month from 31st January to February would make the day-of-month invalid.
		/// In cases like this, the field is responsible for resolving the date. Typically it will choose
		/// the previous valid date, which would be the last valid day of February in this example.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code DAY_OF_WEEK} -
		///  Returns a {@code LocalDate} with the specified day-of-week.
		///  The date is adjusted up to 6 days forward or backward within the boundary
		///  of a Monday to Sunday week.
		/// <li>{@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		///  Returns a {@code LocalDate} with the specified aligned-day-of-week.
		///  The date is adjusted to the specified month-based aligned-day-of-week.
		///  Aligned weeks are counted such that the first week of a given month starts
		///  on the first day of that month.
		///  This may cause the date to be moved up to 6 days into the following month.
		/// <li>{@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		///  Returns a {@code LocalDate} with the specified aligned-day-of-week.
		///  The date is adjusted to the specified year-based aligned-day-of-week.
		///  Aligned weeks are counted such that the first week of a given year starts
		///  on the first day of that year.
		///  This may cause the date to be moved up to 6 days into the following year.
		/// <li>{@code DAY_OF_MONTH} -
		///  Returns a {@code LocalDate} with the specified day-of-month.
		///  The month and year will be unchanged. If the day-of-month is invalid for the
		///  year and month, then a {@code DateTimeException} is thrown.
		/// <li>{@code DAY_OF_YEAR} -
		///  Returns a {@code LocalDate} with the specified day-of-year.
		///  The year will be unchanged. If the day-of-year is invalid for the
		///  year, then a {@code DateTimeException} is thrown.
		/// <li>{@code EPOCH_DAY} -
		///  Returns a {@code LocalDate} with the specified epoch-day.
		///  This completely replaces the date and is equivalent to <seealso cref="#ofEpochDay(long)"/>.
		/// <li>{@code ALIGNED_WEEK_OF_MONTH} -
		///  Returns a {@code LocalDate} with the specified aligned-week-of-month.
		///  Aligned weeks are counted such that the first week of a given month starts
		///  on the first day of that month.
		///  This adjustment moves the date in whole week chunks to match the specified week.
		///  The result will have the same day-of-week as this date.
		///  This may cause the date to be moved into the following month.
		/// <li>{@code ALIGNED_WEEK_OF_YEAR} -
		///  Returns a {@code LocalDate} with the specified aligned-week-of-year.
		///  Aligned weeks are counted such that the first week of a given year starts
		///  on the first day of that year.
		///  This adjustment moves the date in whole week chunks to match the specified week.
		///  The result will have the same day-of-week as this date.
		///  This may cause the date to be moved into the following year.
		/// <li>{@code MONTH_OF_YEAR} -
		///  Returns a {@code LocalDate} with the specified month-of-year.
		///  The year will be unchanged. The day-of-month will also be unchanged,
		///  unless it would be invalid for the new month and year. In that case, the
		///  day-of-month is adjusted to the maximum valid value for the new month and year.
		/// <li>{@code PROLEPTIC_MONTH} -
		///  Returns a {@code LocalDate} with the specified proleptic-month.
		///  The day-of-month will be unchanged, unless it would be invalid for the new month
		///  and year. In that case, the day-of-month is adjusted to the maximum valid value
		///  for the new month and year.
		/// <li>{@code YEAR_OF_ERA} -
		///  Returns a {@code LocalDate} with the specified year-of-era.
		///  The era and month will be unchanged. The day-of-month will also be unchanged,
		///  unless it would be invalid for the new month and year. In that case, the
		///  day-of-month is adjusted to the maximum valid value for the new month and year.
		/// <li>{@code YEAR} -
		///  Returns a {@code LocalDate} with the specified year.
		///  The month will be unchanged. The day-of-month will also be unchanged,
		///  unless it would be invalid for the new month and year. In that case, the
		///  day-of-month is adjusted to the maximum valid value for the new month and year.
		/// <li>{@code ERA} -
		///  Returns a {@code LocalDate} with the specified era.
		///  The year-of-era and month will be unchanged. The day-of-month will also be unchanged,
		///  unless it would be invalid for the new month and year. In that case, the
		///  day-of-month is adjusted to the maximum valid value for the new month and year.
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
		/// <returns> a {@code LocalDate} based on {@code this} with the specified field set, not null </returns>
		/// <exception cref="DateTimeException"> if the field cannot be set </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalDate With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				f.checkValidValue(newValue);
				switch (f)
				{
					case DAY_OF_WEEK:
						return PlusDays(newValue - DayOfWeek.Value);
					case ALIGNED_DAY_OF_WEEK_IN_MONTH:
						return PlusDays(newValue - GetLong(ALIGNED_DAY_OF_WEEK_IN_MONTH));
					case ALIGNED_DAY_OF_WEEK_IN_YEAR:
						return PlusDays(newValue - GetLong(ALIGNED_DAY_OF_WEEK_IN_YEAR));
					case DAY_OF_MONTH:
						return WithDayOfMonth((int) newValue);
					case DAY_OF_YEAR:
						return WithDayOfYear((int) newValue);
					case EPOCH_DAY:
						return LocalDate.OfEpochDay(newValue);
					case ALIGNED_WEEK_OF_MONTH:
						return PlusWeeks(newValue - GetLong(ALIGNED_WEEK_OF_MONTH));
					case ALIGNED_WEEK_OF_YEAR:
						return PlusWeeks(newValue - GetLong(ALIGNED_WEEK_OF_YEAR));
					case MONTH_OF_YEAR:
						return WithMonth((int) newValue);
					case PROLEPTIC_MONTH:
						return PlusMonths(newValue - ProlepticMonth);
					case YEAR_OF_ERA:
						return WithYear((int)(Year_Renamed >= 1 ? newValue : 1 - newValue));
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
		/// Returns a copy of this {@code LocalDate} with the year altered.
		/// <para>
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> a {@code LocalDate} based on this date with the requested year, not null </returns>
		/// <exception cref="DateTimeException"> if the year value is invalid </exception>
		public LocalDate WithYear(int year)
		{
			if (this.Year_Renamed == year)
			{
				return this;
			}
			YEAR.checkValidValue(year);
			return ResolvePreviousValid(year, Month_Renamed, Day);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the month-of-year altered.
		/// <para>
		/// If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		/// <returns> a {@code LocalDate} based on this date with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public LocalDate WithMonth(int month)
		{
			if (this.Month_Renamed == month)
			{
				return this;
			}
			MONTH_OF_YEAR.checkValidValue(month);
			return ResolvePreviousValid(Year_Renamed, month, Day);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the day-of-month altered.
		/// <para>
		/// If the resulting date is invalid, an exception is thrown.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		/// <returns> a {@code LocalDate} based on this date with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month value is invalid,
		///  or if the day-of-month is invalid for the month-year </exception>
		public LocalDate WithDayOfMonth(int dayOfMonth)
		{
			if (this.Day == dayOfMonth)
			{
				return this;
			}
			return Of(Year_Renamed, Month_Renamed, dayOfMonth);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the day-of-year altered.
		/// <para>
		/// If the resulting date is invalid, an exception is thrown.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		/// <returns> a {@code LocalDate} based on this date with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-year value is invalid,
		///  or if the day-of-year is invalid for the year </exception>
		public LocalDate WithDayOfYear(int dayOfYear)
		{
			if (this.DayOfYear == dayOfYear)
			{
				return this;
			}
			return OfYearDay(Year_Renamed, dayOfYear);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date with the specified amount added.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the specified amount added.
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
		/// <returns> a {@code LocalDate} based on this date with the addition made, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDate Plus(TemporalAmount amountToAdd)
		{
			if (amountToAdd is Period)
			{
				Period periodToAdd = (Period) amountToAdd;
				return PlusMonths(periodToAdd.ToTotalMonths()).PlusDays(periodToAdd.Days);
			}
			Objects.RequireNonNull(amountToAdd, "amountToAdd");
			return (LocalDate) amountToAdd.AddTo(this);
		}

		/// <summary>
		/// Returns a copy of this date with the specified amount added.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the amount
		/// in terms of the unit added. If it is not possible to add the amount, because the
		/// unit is not supported or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// In some cases, adding the amount can cause the resulting date to become invalid.
		/// For example, adding one month to 31st January would result in 31st February.
		/// In cases like this, the unit is responsible for resolving the date.
		/// Typically it will choose the previous valid date, which would be the last valid
		/// day of February in this example.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		/// The supported fields behave as follows:
		/// <ul>
		/// <li>{@code DAYS} -
		///  Returns a {@code LocalDate} with the specified number of days added.
		///  This is equivalent to <seealso cref="#plusDays(long)"/>.
		/// <li>{@code WEEKS} -
		///  Returns a {@code LocalDate} with the specified number of weeks added.
		///  This is equivalent to <seealso cref="#plusWeeks(long)"/> and uses a 7 day week.
		/// <li>{@code MONTHS} -
		///  Returns a {@code LocalDate} with the specified number of months added.
		///  This is equivalent to <seealso cref="#plusMonths(long)"/>.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
		/// <li>{@code YEARS} -
		///  Returns a {@code LocalDate} with the specified number of years added.
		///  This is equivalent to <seealso cref="#plusYears(long)"/>.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
		/// <li>{@code DECADES} -
		///  Returns a {@code LocalDate} with the specified number of decades added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 10.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
		/// <li>{@code CENTURIES} -
		///  Returns a {@code LocalDate} with the specified number of centuries added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 100.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
		/// <li>{@code MILLENNIA} -
		///  Returns a {@code LocalDate} with the specified number of millennia added.
		///  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		///  multiplied by 1,000.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
		/// <li>{@code ERAS} -
		///  Returns a {@code LocalDate} with the specified number of eras added.
		///  Only two eras are supported so the amount must be one, zero or minus one.
		///  If the amount is non-zero then the year is changed such that the year-of-era
		///  is unchanged.
		///  The day-of-month will be unchanged unless it would be invalid for the new
		///  month and year. In that case, the day-of-month is adjusted to the maximum
		///  valid value for the new month and year.
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
		/// <returns> a {@code LocalDate} based on this date with the specified amount added, not null </returns>
		/// <exception cref="DateTimeException"> if the addition cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public LocalDate Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				ChronoUnit f = (ChronoUnit) unit;
				switch (f)
				{
					case DAYS:
						return PlusDays(amountToAdd);
					case WEEKS:
						return PlusWeeks(amountToAdd);
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

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of years added.
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
		/// <param name="yearsToAdd">  the years to add, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate PlusYears(long yearsToAdd)
		{
			if (yearsToAdd == 0)
			{
				return this;
			}
			int newYear = YEAR.checkValidIntValue(Year_Renamed + yearsToAdd); // safe overflow
			return ResolvePreviousValid(newYear, Month_Renamed, Day);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of months added.
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
		/// <param name="monthsToAdd">  the months to add, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate PlusMonths(long monthsToAdd)
		{
			if (monthsToAdd == 0)
			{
				return this;
			}
			long monthCount = Year_Renamed * 12L + (Month_Renamed - 1);
			long calcMonths = monthCount + monthsToAdd; // safe overflow
			int newYear = YEAR.checkValidIntValue(Math.FloorDiv(calcMonths, 12));
			int newMonth = (int)Math.FloorMod(calcMonths, 12) + 1;
			return ResolvePreviousValid(newYear, newMonth, Day);
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of weeks added.
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
		/// <param name="weeksToAdd">  the weeks to add, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the weeks added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate PlusWeeks(long weeksToAdd)
		{
			return PlusDays(Math.MultiplyExact(weeksToAdd, 7));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of days added.
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
		/// <param name="daysToAdd">  the days to add, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the days added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate PlusDays(long daysToAdd)
		{
			if (daysToAdd == 0)
			{
				return this;
			}
			long mjDay = Math.AddExact(ToEpochDay(), daysToAdd);
			return LocalDate.OfEpochDay(mjDay);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the specified amount subtracted.
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
		/// <returns> a {@code LocalDate} based on this date with the subtraction made, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDate Minus(TemporalAmount amountToSubtract)
		{
			if (amountToSubtract is Period)
			{
				Period periodToSubtract = (Period) amountToSubtract;
				return MinusMonths(periodToSubtract.ToTotalMonths()).MinusDays(periodToSubtract.Days);
			}
			Objects.RequireNonNull(amountToSubtract, "amountToSubtract");
			return (LocalDate) amountToSubtract.SubtractFrom(this);
		}

		/// <summary>
		/// Returns a copy of this date with the specified amount subtracted.
		/// <para>
		/// This returns a {@code LocalDate}, based on this one, with the amount
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
		/// <returns> a {@code LocalDate} based on this date with the specified amount subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override LocalDate Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (amountToSubtract == Long.MinValue ? Plus(Long.MaxValue, unit).Plus(1, unit) : Plus(-amountToSubtract, unit));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of years subtracted.
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
		/// invalid date 2007-02-29 (standard year). Instead of returning an invalid
		/// result, the last valid day of the month, 2007-02-28, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate MinusYears(long yearsToSubtract)
		{
			return (yearsToSubtract == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-yearsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of months subtracted.
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
		/// 2007-02-31. Instead of returning an invalid result, the last valid day
		/// of the month, 2007-02-28, is selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate MinusMonths(long monthsToSubtract)
		{
			return (monthsToSubtract == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-monthsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of weeks subtracted.
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
		/// <param name="weeksToSubtract">  the weeks to subtract, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the weeks subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate MinusWeeks(long weeksToSubtract)
		{
			return (weeksToSubtract == Long.MinValue ? PlusWeeks(Long.MaxValue).PlusWeeks(1) : PlusWeeks(-weeksToSubtract));
		}

		/// <summary>
		/// Returns a copy of this {@code LocalDate} with the specified number of days subtracted.
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
		/// <param name="daysToSubtract">  the days to subtract, may be negative </param>
		/// <returns> a {@code LocalDate} based on this date with the days subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		public LocalDate MinusDays(long daysToSubtract)
		{
			return (daysToSubtract == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-daysToSubtract));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this date using the specified query.
		/// <para>
		/// This queries this date using the specified query strategy object.
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
			if (query == TemporalQueries.LocalDate())
			{
				return (R) this;
			}
			return ChronoLocalDate.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have the same date as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the date changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#EPOCH_DAY"/> as the field.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisLocalDate.adjustInto(temporal);
		///   temporal = temporal.with(thisLocalDate);
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
			return ChronoLocalDate.this.adjustInto(temporal);
		}

		/// <summary>
		/// Calculates the amount of time until another date in terms of the specified unit.
		/// <para>
		/// This calculates the amount of time between two {@code LocalDate}
		/// objects in terms of a single {@code TemporalUnit}.
		/// The start and end points are {@code this} and the specified date.
		/// The result will be negative if the end is before the start.
		/// The {@code Temporal} passed to this method is converted to a
		/// {@code LocalDate} using <seealso cref="#from(TemporalAccessor)"/>.
		/// For example, the amount in days between two dates can be calculated
		/// using {@code startDate.until(endDate, DAYS)}.
		/// </para>
		/// <para>
		/// The calculation returns a whole number, representing the number of
		/// complete units between the two dates.
		/// For example, the amount in months between 2012-06-15 and 2012-08-14
		/// will only be one month as it is one day short of two months.
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
		/// The units {@code DAYS}, {@code WEEKS}, {@code MONTHS}, {@code YEARS},
		/// {@code DECADES}, {@code CENTURIES}, {@code MILLENNIA} and {@code ERAS}
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
		/// <param name="endExclusive">  the end date, exclusive, which is converted to a {@code LocalDate}, not null </param>
		/// <param name="unit">  the unit to measure the amount in, not null </param>
		/// <returns> the amount of time between this date and the end date </returns>
		/// <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		///  temporal cannot be converted to a {@code LocalDate} </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			LocalDate end = LocalDate.From(endExclusive);
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case DAYS:
						return DaysUntil(end);
					case WEEKS:
						return DaysUntil(end) / 7;
					case MONTHS:
						return MonthsUntil(end);
					case YEARS:
						return MonthsUntil(end) / 12;
					case DECADES:
						return MonthsUntil(end) / 120;
					case CENTURIES:
						return MonthsUntil(end) / 1200;
					case MILLENNIA:
						return MonthsUntil(end) / 12000;
					case ERAS:
						return end.GetLong(ERA) - GetLong(ERA);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return unit.Between(this, end);
		}

		internal long DaysUntil(LocalDate end)
		{
			return end.ToEpochDay() - ToEpochDay(); // no overflow
		}

		private long MonthsUntil(LocalDate end)
		{
			long packed1 = ProlepticMonth * 32L + DayOfMonth; // no overflow
			long packed2 = end.ProlepticMonth * 32L + end.DayOfMonth; // no overflow
			return (packed2 - packed1) / 32;
		}

		/// <summary>
		/// Calculates the period between this date and another date as a {@code Period}.
		/// <para>
		/// This calculates the period between two dates in terms of years, months and days.
		/// The start and end points are {@code this} and the specified date.
		/// The result will be negative if the end is before the start.
		/// The negative sign will be the same in each of year, month and day.
		/// </para>
		/// <para>
		/// The calculation is performed using the ISO calendar system.
		/// If necessary, the input date will be converted to ISO.
		/// </para>
		/// <para>
		/// The start date is included, but the end date is not.
		/// The period is calculated by removing complete months, then calculating
		/// the remaining number of days, adjusting to ensure that both have the same sign.
		/// The number of months is then normalized into years and months based on a 12 month year.
		/// A month is considered to be complete if the end day-of-month is greater
		/// than or equal to the start day-of-month.
		/// For example, from {@code 2010-01-15} to {@code 2011-03-18} is "1 year, 2 months and 3 days".
		/// </para>
		/// <para>
		/// There are two equivalent ways of using this method.
		/// The first is to invoke this method.
		/// The second is to use <seealso cref="Period#between(LocalDate, LocalDate)"/>:
		/// <pre>
		///   // these two lines are equivalent
		///   period = start.until(end);
		///   period = Period.between(start, end);
		/// </pre>
		/// The choice should be made based on which makes the code more readable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="endDateExclusive">  the end date, exclusive, which may be in any chronology, not null </param>
		/// <returns> the period between this date and the end date, not null </returns>
		public Period Until(ChronoLocalDate endDateExclusive)
		{
			LocalDate end = LocalDate.From(endDateExclusive);
			long totalMonths = end.ProlepticMonth - this.ProlepticMonth; // safe
			int days = end.Day - this.Day;
			if (totalMonths > 0 && days < 0)
			{
				totalMonths--;
				LocalDate calcDate = this.PlusMonths(totalMonths);
				days = (int)(end.ToEpochDay() - calcDate.ToEpochDay()); // safe
			}
			else if (totalMonths < 0 && days > 0)
			{
				totalMonths++;
				days -= end.LengthOfMonth();
			}
			long years = totalMonths / 12; // safe
			int months = (int)(totalMonths % 12); // safe
			return Period.Of(Math.ToIntExact(years), months, days);
		}

		/// <summary>
		/// Formats this date using the specified formatter.
		/// <para>
		/// This date will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted date string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public override String Format(DateTimeFormatter formatter) // override for Javadoc and performance
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this date with a time to create a {@code LocalDateTime}.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this date at the specified time.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">  the time to combine with, not null </param>
		/// <returns> the local date-time formed from this date and the specified time, not null </returns>
		public override LocalDateTime AtTime(LocalTime time)
		{
			return LocalDateTime.Of(this, time);
		}

		/// <summary>
		/// Combines this date with a time to create a {@code LocalDateTime}.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this date at the
		/// specified hour and minute.
		/// The seconds and nanosecond fields will be set to zero.
		/// The individual time fields must be within their valid range.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		/// <returns> the local date-time formed from this date and the specified time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public LocalDateTime AtTime(int hour, int minute)
		{
			return AtTime(LocalTime.Of(hour, minute));
		}

		/// <summary>
		/// Combines this date with a time to create a {@code LocalDateTime}.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this date at the
		/// specified hour, minute and second.
		/// The nanosecond field will be set to zero.
		/// The individual time fields must be within their valid range.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <returns> the local date-time formed from this date and the specified time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public LocalDateTime AtTime(int hour, int minute, int second)
		{
			return AtTime(LocalTime.Of(hour, minute, second));
		}

		/// <summary>
		/// Combines this date with a time to create a {@code LocalDateTime}.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this date at the
		/// specified hour, minute, second and nanosecond.
		/// The individual time fields must be within their valid range.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		/// <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		/// <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		/// <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		/// <returns> the local date-time formed from this date and the specified time, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		public LocalDateTime AtTime(int hour, int minute, int second, int nanoOfSecond)
		{
			return AtTime(LocalTime.Of(hour, minute, second, nanoOfSecond));
		}

		/// <summary>
		/// Combines this date with an offset time to create an {@code OffsetDateTime}.
		/// <para>
		/// This returns an {@code OffsetDateTime} formed from this date at the specified time.
		/// All possible combinations of date and time are valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">  the time to combine with, not null </param>
		/// <returns> the offset date-time formed from this date and the specified time, not null </returns>
		public OffsetDateTime AtTime(OffsetTime time)
		{
			return OffsetDateTime.Of(LocalDateTime.Of(this, time.ToLocalTime()), time.Offset);
		}

		/// <summary>
		/// Combines this date with the time of midnight to create a {@code LocalDateTime}
		/// at the start of this date.
		/// <para>
		/// This returns a {@code LocalDateTime} formed from this date at the time of
		/// midnight, 00:00, at the start of this date.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the local date-time of midnight at the start of this date, not null </returns>
		public LocalDateTime AtStartOfDay()
		{
			return LocalDateTime.Of(this, LocalTime.MIDNIGHT);
		}

		/// <summary>
		/// Returns a zoned date-time from this date at the earliest valid time according
		/// to the rules in the time-zone.
		/// <para>
		/// Time-zone rules, such as daylight savings, mean that not every local date-time
		/// is valid for the specified zone, thus the local date-time may not be midnight.
		/// </para>
		/// <para>
		/// In most cases, there is only one valid offset for a local date-time.
		/// In the case of an overlap, there are two valid offsets, and the earlier one is used,
		/// corresponding to the first occurrence of midnight on the date.
		/// In the case of a gap, the zoned date-time will represent the instant just after the gap.
		/// </para>
		/// <para>
		/// If the zone ID is a <seealso cref="ZoneOffset"/>, then the result always has a time of midnight.
		/// </para>
		/// <para>
		/// To convert to a specific time in a given time-zone call <seealso cref="#atTime(LocalTime)"/>
		/// followed by <seealso cref="LocalDateTime#atZone(ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the zoned date-time formed from this date and the earliest valid time for the zone, not null </returns>
		public ZonedDateTime AtStartOfDay(ZoneId zone)
		{
			Objects.RequireNonNull(zone, "zone");
			// need to handle case where there is a gap from 11:30 to 00:30
			// standard ZDT factory would result in 01:00 rather than 00:30
			LocalDateTime ldt = AtTime(LocalTime.MIDNIGHT);
			if (zone is ZoneOffset == false)
			{
				ZoneRules rules = zone.Rules;
				ZoneOffsetTransition trans = rules.GetTransition(ldt);
				if (trans != chrono.ChronoLocalDate_Fields.Null && trans.Gap)
				{
					ldt = trans.DateTimeAfter;
				}
			}
			return ZonedDateTime.Of(ldt, zone);
		}

		//-----------------------------------------------------------------------
		public override long ToEpochDay()
		{
			long y = Year_Renamed;
			long m = Month_Renamed;
			long total = 0;
			total += 365 * y;
			if (y >= 0)
			{
				total += (y + 3) / 4 - (y + 99) / 100 + (y + 399) / 400;
			}
			else
			{
				total -= y / -4 - y / -100 + y / -400;
			}
			total += ((367 * m - 362) / 12);
			total += Day - 1;
			if (m > 2)
			{
				total--;
				if (LeapYear == false)
				{
					total--;
				}
			}
			return total - DAYS_0000_TO_1970;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this date to another date.
		/// <para>
		/// The comparison is primarily based on the date, from earliest to latest.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// If all the dates being compared are instances of {@code LocalDate},
		/// then the comparison will be entirely based on the date.
		/// If some dates being compared are in different chronologies, then the
		/// chronology is also considered, see <seealso cref="java.time.chrono.ChronoLocalDate#compareTo"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(ChronoLocalDate other) // override for Javadoc and performance
		{
			if (other is LocalDate)
			{
				return CompareTo0((LocalDate) other);
			}
			return ChronoLocalDate.this.CompareTo(other);
		}

		internal int CompareTo0(LocalDate otherDate)
		{
			int chrono.ChronoLocalDate_Fields.Cmp = (Year_Renamed - otherDate.Year_Renamed);
			if (chrono.ChronoLocalDate_Fields.Cmp == 0)
			{
				chrono.ChronoLocalDate_Fields.Cmp = (Month_Renamed - otherDate.Month_Renamed);
				if (chrono.ChronoLocalDate_Fields.Cmp == 0)
				{
					chrono.ChronoLocalDate_Fields.Cmp = (Day - otherDate.Day);
				}
			}
			return chrono.ChronoLocalDate_Fields.Cmp;
		}

		/// <summary>
		/// Checks if this date is after the specified date.
		/// <para>
		/// This checks to see if this date represents a point on the
		/// local time-line after the other date.
		/// <pre>
		///   LocalDate a = LocalDate.of(2012, 6, 30);
		///   LocalDate b = LocalDate.of(2012, 7, 1);
		///   a.isAfter(b) == false
		///   a.isAfter(a) == false
		///   b.isAfter(a) == true
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two dates on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>,
		/// but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date to compare to, not null </param>
		/// <returns> true if this date is after the specified date </returns>
		public override bool IsAfter(ChronoLocalDate other) // override for Javadoc and performance
		{
			if (other is LocalDate)
			{
				return CompareTo0((LocalDate) other) > 0;
			}
			return ChronoLocalDate.this.isAfter(other);
		}

		/// <summary>
		/// Checks if this date is before the specified date.
		/// <para>
		/// This checks to see if this date represents a point on the
		/// local time-line before the other date.
		/// <pre>
		///   LocalDate a = LocalDate.of(2012, 6, 30);
		///   LocalDate b = LocalDate.of(2012, 7, 1);
		///   a.isBefore(b) == true
		///   a.isBefore(a) == false
		///   b.isBefore(a) == false
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two dates on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>,
		/// but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date to compare to, not null </param>
		/// <returns> true if this date is before the specified date </returns>
		public override bool IsBefore(ChronoLocalDate other) // override for Javadoc and performance
		{
			if (other is LocalDate)
			{
				return CompareTo0((LocalDate) other) < 0;
			}
			return ChronoLocalDate.this.isBefore(other);
		}

		/// <summary>
		/// Checks if this date is equal to the specified date.
		/// <para>
		/// This checks to see if this date represents the same point on the
		/// local time-line as the other date.
		/// <pre>
		///   LocalDate a = LocalDate.of(2012, 6, 30);
		///   LocalDate b = LocalDate.of(2012, 7, 1);
		///   a.isEqual(b) == false
		///   a.isEqual(a) == true
		///   b.isEqual(a) == false
		/// </pre>
		/// </para>
		/// <para>
		/// This method only considers the position of the two dates on the local time-line.
		/// It does not take into account the chronology, or calendar system.
		/// This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>
		/// but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date to compare to, not null </param>
		/// <returns> true if this date is equal to the specified date </returns>
		public override bool IsEqual(ChronoLocalDate other) // override for Javadoc and performance
		{
			if (other is LocalDate)
			{
				return CompareTo0((LocalDate) other) == 0;
			}
			return ChronoLocalDate.this.isEqual(other);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this date is equal to another date.
		/// <para>
		/// Compares this {@code LocalDate} with another ensuring that the date is the same.
		/// </para>
		/// <para>
		/// Only objects of type {@code LocalDate} are compared, other types return false.
		/// To compare the dates of two {@code TemporalAccessor} instances, including dates
		/// in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is LocalDate)
			{
				return CompareTo0((LocalDate) obj) == 0;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			int yearValue = Year_Renamed;
			int monthValue = Month_Renamed;
			int dayValue = Day;
			return (yearValue & 0xFFFFF800) ^ ((yearValue << 11) + (monthValue << 6) + (dayValue));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this date as a {@code String}, such as {@code 2007-12-03}.
		/// <para>
		/// The output will be in the ISO-8601 format {@code uuuu-MM-dd}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date, not null </returns>
		public override String ToString()
		{
			int yearValue = Year_Renamed;
			int monthValue = Month_Renamed;
			int dayValue = Day;
			int absYear = System.Math.Abs(yearValue);
			StringBuilder buf = new StringBuilder(10);
			if (absYear < 1000)
			{
				if (yearValue < 0)
				{
					buf.Append(yearValue - 10000).DeleteCharAt(1);
				}
				else
				{
					buf.Append(yearValue + 10000).DeleteCharAt(0);
				}
			}
			else
			{
				if (yearValue > 9999)
				{
					buf.Append('+');
				}
				buf.Append(yearValue);
			}
			return buf.Append(monthValue < 10 ? "-0" : "-").Append(monthValue).Append(dayValue < 10 ? "-0" : "-").Append(dayValue).ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(3);  // identifies a LocalDate
		///  out.writeInt(year);
		///  out.writeByte(month);
		///  out.writeByte(day);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.LOCAL_DATE_TYPE, this);
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
			@out.WriteByte(Day);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static LocalDate readExternal(java.io.DataInput in) throws java.io.IOException
		internal static LocalDate ReadExternal(DataInput @in)
		{
			int year = @in.ReadInt();
			int month = @in.ReadByte();
			int dayOfMonth = @in.ReadByte();
			return LocalDate.Of(year, month, dayOfMonth);
		}

	}

}