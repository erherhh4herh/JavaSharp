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
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.chrono
{



	/// <summary>
	/// The ISO calendar system.
	/// <para>
	/// This chronology defines the rules of the ISO calendar system.
	/// This calendar system is based on the ISO-8601 standard, which is the
	/// <i>de facto</i> world calendar.
	/// </para>
	/// <para>
	/// The fields are defined as follows:
	/// <ul>
	/// <li>era - There are two eras, 'Current Era' (CE) and 'Before Current Era' (BCE).
	/// <li>year-of-era - The year-of-era is the same as the proleptic-year for the current CE era.
	///  For the BCE era before the ISO epoch the year increases from 1 upwards as time goes backwards.
	/// <li>proleptic-year - The proleptic year is the same as the year-of-era for the
	///  current era. For the previous era, years have zero, then negative values.
	/// <li>month-of-year - There are 12 months in an ISO year, numbered from 1 to 12.
	/// <li>day-of-month - There are between 28 and 31 days in each of the ISO month, numbered from 1 to 31.
	///  Months 4, 6, 9 and 11 have 30 days, Months 1, 3, 5, 7, 8, 10 and 12 have 31 days.
	///  Month 2 has 28 days, or 29 in a leap year.
	/// <li>day-of-year - There are 365 days in a standard ISO year and 366 in a leap year.
	///  The days are numbered from 1 to 365 or 1 to 366.
	/// <li>leap-year - Leap years occur every 4 years, except where the year is divisble by 100 and not divisble by 400.
	/// </ul>
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class IsoChronology : AbstractChronology
	{

		/// <summary>
		/// Singleton instance of the ISO chronology.
		/// </summary>
		public static readonly IsoChronology INSTANCE = new IsoChronology();

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -1440403870442975015L;

		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private IsoChronology()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the ID of the chronology - 'ISO'.
		/// <para>
		/// The ID uniquely identifies the {@code Chronology}.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology ID - 'ISO' </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		public override String Id
		{
			get
			{
				return "ISO";
			}
		}

		/// <summary>
		/// Gets the calendar type of the underlying calendar system - 'iso8601'.
		/// <para>
		/// The calendar type is an identifier defined by the
		/// <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// It can also be used as part of a locale, accessible via
		/// <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the calendar system type - 'iso8601' </returns>
		/// <seealso cref= #getId() </seealso>
		public override String CalendarType
		{
			get
			{
				return "iso8601";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an ISO local date from the era, year-of-era, month-of-year
		/// and day-of-month fields.
		/// </summary>
		/// <param name="era">  the ISO era, not null </param>
		/// <param name="yearOfEra">  the ISO year-of-era </param>
		/// <param name="month">  the ISO month-of-year </param>
		/// <param name="dayOfMonth">  the ISO day-of-month </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the type of {@code era} is not {@code IsoEra} </exception>
		public override LocalDate Date(Era era, int yearOfEra, int month, int dayOfMonth) // override with covariant return type
		{
			return Date(ProlepticYear(era, yearOfEra), month, dayOfMonth);
		}

		/// <summary>
		/// Obtains an ISO local date from the proleptic-year, month-of-year
		/// and day-of-month fields.
		/// <para>
		/// This is equivalent to <seealso cref="LocalDate#of(int, int, int)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the ISO proleptic-year </param>
		/// <param name="month">  the ISO month-of-year </param>
		/// <param name="dayOfMonth">  the ISO day-of-month </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate Date(int prolepticYear, int month, int dayOfMonth) // override with covariant return type
		{
			return LocalDate.Of(prolepticYear, month, dayOfMonth);
		}

		/// <summary>
		/// Obtains an ISO local date from the era, year-of-era and day-of-year fields.
		/// </summary>
		/// <param name="era">  the ISO era, not null </param>
		/// <param name="yearOfEra">  the ISO year-of-era </param>
		/// <param name="dayOfYear">  the ISO day-of-year </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateYearDay(Era era, int yearOfEra, int dayOfYear) // override with covariant return type
		{
			return DateYearDay(ProlepticYear(era, yearOfEra), dayOfYear);
		}

		/// <summary>
		/// Obtains an ISO local date from the proleptic-year and day-of-year fields.
		/// <para>
		/// This is equivalent to <seealso cref="LocalDate#ofYearDay(int, int)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the ISO proleptic-year </param>
		/// <param name="dayOfYear">  the ISO day-of-year </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateYearDay(int prolepticYear, int dayOfYear) // override with covariant return type
		{
			return LocalDate.OfYearDay(prolepticYear, dayOfYear);
		}

		/// <summary>
		/// Obtains an ISO local date from the epoch-day.
		/// <para>
		/// This is equivalent to <seealso cref="LocalDate#ofEpochDay(long)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="epochDay">  the epoch day </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateEpochDay(long epochDay) // override with covariant return type
		{
			return LocalDate.OfEpochDay(epochDay);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an ISO local date from another date-time object.
		/// <para>
		/// This is equivalent to <seealso cref="LocalDate#from(TemporalAccessor)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the date-time object to convert, not null </param>
		/// <returns> the ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate Date(TemporalAccessor temporal) // override with covariant return type
		{
			return LocalDate.From(temporal);
		}

		/// <summary>
		/// Obtains an ISO local date-time from another date-time object.
		/// <para>
		/// This is equivalent to <seealso cref="LocalDateTime#from(TemporalAccessor)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the date-time object to convert, not null </param>
		/// <returns> the ISO local date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date-time </exception>
		public override LocalDateTime LocalDateTime(TemporalAccessor temporal) // override with covariant return type
		{
			return LocalDateTime.From(temporal);
		}

		/// <summary>
		/// Obtains an ISO zoned date-time from another date-time object.
		/// <para>
		/// This is equivalent to <seealso cref="ZonedDateTime#from(TemporalAccessor)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the date-time object to convert, not null </param>
		/// <returns> the ISO zoned date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date-time </exception>
		public override ZonedDateTime ZonedDateTime(TemporalAccessor temporal) // override with covariant return type
		{
			return ZonedDateTime.From(temporal);
		}

		/// <summary>
		/// Obtains an ISO zoned date-time in this chronology from an {@code Instant}.
		/// <para>
		/// This is equivalent to <seealso cref="ZonedDateTime#ofInstant(Instant, ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to create the date-time from, not null </param>
		/// <param name="zone">  the time-zone, not null </param>
		/// <returns> the zoned date-time, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		public override ZonedDateTime ZonedDateTime(Instant Chronology_Fields, ZoneId Chronology_Fields)
		{
			return ZonedDateTime.OfInstant(Chronology_Fields.Instant, Chronology_Fields.Zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current ISO local date from the system clock in the default time-zone.
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
		/// <returns> the current ISO local date using the system clock and default time-zone, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateNow() // override with covariant return type
		{
			return DateNow(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current ISO local date from the system clock in the specified time-zone.
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
		/// <returns> the current ISO local date using the system clock, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateNow(ZoneId Chronology_Fields) // override with covariant return type
		{
			return DateNow(Clock.System(Chronology_Fields.Zone));
		}

		/// <summary>
		/// Obtains the current ISO local date from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date - today.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current ISO local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override LocalDate DateNow(Clock clock) // override with covariant return type
		{
			Objects.RequireNonNull(clock, "clock");
			return Date(LocalDate.Now(clock));
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
		/// <param name="prolepticYear">  the ISO proleptic year to check </param>
		/// <returns> true if the year is leap, false otherwise </returns>
		public override bool IsLeapYear(long prolepticYear)
		{
			return ((prolepticYear & 3) == 0) && ((prolepticYear % 100) != 0 || (prolepticYear % 400) == 0);
		}

		public override int ProlepticYear(Era era, int yearOfEra)
		{
			if (era is IsoEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be IsoEra");
			}
			return (era == IsoEra.CE ? yearOfEra : 1 - yearOfEra);
		}

		public override IsoEra EraOf(int eraValue)
		{
			return IsoEra.of(eraValue);
		}

		public override IList<Era> Eras()
		{
			return Arrays.AsList<Era>(IsoEra.values());
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Resolves parsed {@code ChronoField} values into a date during parsing.
		/// <para>
		/// Most {@code TemporalField} implementations are resolved using the
		/// resolve method on the field. By contrast, the {@code ChronoField} class
		/// defines fields that only have meaning relative to the chronology.
		/// As such, {@code ChronoField} date fields are resolved here in the
		/// context of a specific chronology.
		/// </para>
		/// <para>
		/// {@code ChronoField} instances on the ISO calendar system are resolved
		/// as follows.
		/// <ul>
		/// <li>{@code EPOCH_DAY} - If present, this is converted to a {@code LocalDate}
		///  and all other date fields are then cross-checked against the date.
		/// <li>{@code PROLEPTIC_MONTH} - If present, then it is split into the
		///  {@code YEAR} and {@code MONTH_OF_YEAR}. If the mode is strict or smart
		///  then the field is validated.
		/// <li>{@code YEAR_OF_ERA} and {@code ERA} - If both are present, then they
		///  are combined to form a {@code YEAR}. In lenient mode, the {@code YEAR_OF_ERA}
		///  range is not validated, in smart and strict mode it is. The {@code ERA} is
		///  validated for range in all three modes. If only the {@code YEAR_OF_ERA} is
		///  present, and the mode is smart or lenient, then the current era (CE/AD)
		///  is assumed. In strict mode, no era is assumed and the {@code YEAR_OF_ERA} is
		///  left untouched. If only the {@code ERA} is present, then it is left untouched.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH} -
		///  If all three are present, then they are combined to form a {@code LocalDate}.
		///  In all three modes, the {@code YEAR} is validated. If the mode is smart or strict,
		///  then the month and day are validated, with the day validated from 1 to 31.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first of January in the requested year, then adding
		///  the difference in months, then the difference in days.
		///  If the mode is smart, and the day-of-month is greater than the maximum for
		///  the year-month, then the day-of-month is adjusted to the last day-of-month.
		///  If the mode is strict, then the three fields must form a valid date.
		/// <li>{@code YEAR} and {@code DAY_OF_YEAR} -
		///  If both are present, then they are combined to form a {@code LocalDate}.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first of January in the requested year, then adding
		///  the difference in days.
		///  If the mode is smart or strict, then the two fields must form a valid date.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		///  {@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		///  If all four are present, then they are combined to form a {@code LocalDate}.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first of January in the requested year, then adding
		///  the difference in months, then the difference in weeks, then in days.
		///  If the mode is smart or strict, then the all four fields are validated to
		///  their outer ranges. The date is then combined in a manner equivalent to
		///  creating a date on the first day of the requested year and month, then adding
		///  the amount in weeks and days to reach their values. If the mode is strict,
		///  the date is additionally validated to check that the day and week adjustment
		///  did not change the month.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		///  {@code DAY_OF_WEEK} - If all four are present, then they are combined to
		///  form a {@code LocalDate}. The approach is the same as described above for
		///  years, months and weeks in {@code ALIGNED_DAY_OF_WEEK_IN_MONTH}.
		///  The day-of-week is adjusted as the next or same matching day-of-week once
		///  the years, months and weeks have been handled.
		/// <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		///  If all three are present, then they are combined to form a {@code LocalDate}.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first of January in the requested year, then adding
		///  the difference in weeks, then in days.
		///  If the mode is smart or strict, then the all three fields are validated to
		///  their outer ranges. The date is then combined in a manner equivalent to
		///  creating a date on the first day of the requested year, then adding
		///  the amount in weeks and days to reach their values. If the mode is strict,
		///  the date is additionally validated to check that the day and week adjustment
		///  did not change the year.
		/// <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code DAY_OF_WEEK} -
		///  If all three are present, then they are combined to form a {@code LocalDate}.
		///  The approach is the same as described above for years and weeks in
		///  {@code ALIGNED_DAY_OF_WEEK_IN_YEAR}. The day-of-week is adjusted as the
		///  next or same matching day-of-week once the years and weeks have been handled.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		/// <param name="resolverStyle">  the requested type of resolve, not null </param>
		/// <returns> the resolved date, null if insufficient information to create a date </returns>
		/// <exception cref="DateTimeException"> if the date cannot be resolved, typically
		///  because of a conflict in the input data </exception>
		public override LocalDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for performance
		{
			return (LocalDate) base.ResolveDate(fieldValues, resolverStyle);
		}

		internal override void ResolveProlepticMonth(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for better proleptic algorithm
		{
			Long pMonth = fieldValues.Remove(PROLEPTIC_MONTH);
			if (pMonth != null)
			{
				if (resolverStyle != ResolverStyle.LENIENT)
				{
					PROLEPTIC_MONTH.checkValidValue(pMonth);
				}
				AddFieldValue(fieldValues, MONTH_OF_YEAR, Math.FloorMod(pMonth, 12) + 1);
				AddFieldValue(fieldValues, YEAR, Math.FloorDiv(pMonth, 12));
			}
		}

		internal override LocalDate ResolveYearOfEra(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for enhanced behaviour
		{
			Long yoeLong = fieldValues.Remove(YEAR_OF_ERA);
			if (yoeLong != null)
			{
				if (resolverStyle != ResolverStyle.LENIENT)
				{
					YEAR_OF_ERA.checkValidValue(yoeLong);
				}
				Long era = fieldValues.Remove(ERA);
				if (era == null)
				{
					Long year = fieldValues[YEAR];
					if (resolverStyle == ResolverStyle.STRICT)
					{
						// do not invent era if strict, but do cross-check with year
						if (year != null)
						{
							AddFieldValue(fieldValues, YEAR, (year > 0 ? yoeLong: Math.SubtractExact(1, yoeLong)));
						}
						else
						{
							// reinstate the field removed earlier, no cross-check issues
							fieldValues[YEAR_OF_ERA] = yoeLong;
						}
					}
					else
					{
						// invent era
						AddFieldValue(fieldValues, YEAR, (year == null || year > 0 ? yoeLong: Math.SubtractExact(1, yoeLong)));
					}
				}
				else if (era.LongValue() == 1L)
				{
					AddFieldValue(fieldValues, YEAR, yoeLong);
				}
				else if (era.LongValue() == 0L)
				{
					AddFieldValue(fieldValues, YEAR, Math.SubtractExact(1, yoeLong));
				}
				else
				{
					throw new DateTimeException("Invalid value for era: " + era);
				}
			}
			else if (fieldValues.ContainsKey(ERA))
			{
				ERA.checkValidValue(fieldValues[ERA]); // always validated
			}
			return null;
		}

		internal override LocalDate ResolveYMD(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for performance
		{
			int y = YEAR.checkValidIntValue(fieldValues.Remove(YEAR));
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long months = Math.SubtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1);
				long days = Math.SubtractExact(fieldValues.Remove(DAY_OF_MONTH), 1);
				return LocalDate.Of(y, 1, 1).PlusMonths(months).PlusDays(days);
			}
			int moy = MONTH_OF_YEAR.checkValidIntValue(fieldValues.Remove(MONTH_OF_YEAR));
			int dom = DAY_OF_MONTH.checkValidIntValue(fieldValues.Remove(DAY_OF_MONTH));
			if (resolverStyle == ResolverStyle.SMART) // previous valid
			{
				if (moy == 4 || moy == 6 || moy == 9 || moy == 11)
				{
					dom = System.Math.Min(dom, 30);
				}
				else if (moy == 2)
				{
					dom = System.Math.Min(dom, Month.FEBRUARY.length(Year.IsLeap(y)));

				}
			}
			return LocalDate.Of(y, moy, dom);
		}

		//-----------------------------------------------------------------------
		public override ValueRange Range(ChronoField field)
		{
			return field.range();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a period for this chronology based on years, months and days.
		/// <para>
		/// This returns a period tied to the ISO chronology using the specified
		/// years, months and days. See <seealso cref="Period"/> for further details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the number of years, may be negative </param>
		/// <param name="months">  the number of years, may be negative </param>
		/// <param name="days">  the number of years, may be negative </param>
		/// <returns> the period in terms of this chronology, not null </returns>
		/// <returns> the ISO period, not null </returns>
		public override Period Period(int years, int months, int days) // override with covariant return type
		{
			return Period.Of(years, months, days);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the Chronology using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(1);     // identifies a Chronology
		///  out.writeUTF(getId());
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		internal override Object WriteReplace()
		{
			return base.WriteReplace();
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
	}

}