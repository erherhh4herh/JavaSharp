using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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



	using CalendarDate = sun.util.calendar.CalendarDate;
	using LocalGregorianCalendar = sun.util.calendar.LocalGregorianCalendar;

	/// <summary>
	/// A date in the Japanese Imperial calendar system.
	/// <para>
	/// This date operates using the <seealso cref="JapaneseChronology Japanese Imperial calendar"/>.
	/// This calendar system is primarily used in Japan.
	/// </para>
	/// <para>
	/// The Japanese Imperial calendar system is the same as the ISO calendar system
	/// apart from the era-based year numbering. The proleptic-year is defined to be
	/// equal to the ISO proleptic-year.
	/// </para>
	/// <para>
	/// Japan introduced the Gregorian calendar starting with Meiji 6.
	/// Only Meiji and later eras are supported;
	/// dates before Meiji 6, January 1 are not supported.
	/// </para>
	/// <para>
	/// For example, the Japanese year "Heisei 24" corresponds to ISO year "2012".<br>
	/// Calling {@code japaneseDate.get(YEAR_OF_ERA)} will return 24.<br>
	/// Calling {@code japaneseDate.get(YEAR)} will return 2012.<br>
	/// Calling {@code japaneseDate.get(ERA)} will return 2, corresponding to
	/// {@code JapaneseChronology.ERA_HEISEI}.<br>
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code JapaneseDate} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class JapaneseDate : ChronoLocalDateImpl<JapaneseDate>, ChronoLocalDate
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -305327627230580483L;

		/// <summary>
		/// The underlying ISO local date.
		/// </summary>
		[NonSerialized]
		private readonly LocalDate IsoDate;
		/// <summary>
		/// The JapaneseEra of this date.
		/// </summary>
		[NonSerialized]
		private JapaneseEra Era_Renamed;
		/// <summary>
		/// The Japanese imperial calendar year of this date.
		/// </summary>
		[NonSerialized]
		private int YearOfEra;

		/// <summary>
		/// The first day supported by the JapaneseChronology is Meiji 6, January 1st.
		/// </summary>
		internal static readonly LocalDate MEIJI_6_ISODATE = LocalDate.Of(1873, 1, 1);

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current {@code JapaneseDate} from the system clock in the default time-zone.
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
		public static JapaneseDate Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current {@code JapaneseDate} from the system clock in the specified time-zone.
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
		public static JapaneseDate Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current {@code JapaneseDate} from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date - today.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date, not null </returns>
		/// <exception cref="DateTimeException"> if the current date cannot be obtained </exception>
		public static JapaneseDate Now(Clock clock)
		{
			return new JapaneseDate(LocalDate.Now(clock));
		}

		/// <summary>
		/// Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		/// system from the era, year-of-era, month-of-year and day-of-month fields.
		/// <para>
		/// This returns a {@code JapaneseDate} with the specified fields.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// </para>
		/// <para>
		/// The Japanese month and day-of-month are the same as those in the
		/// ISO calendar system. They are not reset when the era changes.
		/// For example:
		/// <pre>
		///  6th Jan Showa 64 = ISO 1989-01-06
		///  7th Jan Showa 64 = ISO 1989-01-07
		///  8th Jan Heisei 1 = ISO 1989-01-08
		///  9th Jan Heisei 1 = ISO 1989-01-09
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="era">  the Japanese era, not null </param>
		/// <param name="yearOfEra">  the Japanese year-of-era </param>
		/// <param name="month">  the Japanese month-of-year, from 1 to 12 </param>
		/// <param name="dayOfMonth">  the Japanese day-of-month, from 1 to 31 </param>
		/// <returns> the date in Japanese calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year,
		///  or if the date is not a Japanese era </exception>
		public static JapaneseDate Of(JapaneseEra era, int yearOfEra, int month, int dayOfMonth)
		{
			Objects.RequireNonNull(era, "era");
			LocalGregorianCalendar.Date jdate = JapaneseChronology.JCAL.newCalendarDate(ChronoLocalDate_Fields.Null);
			jdate.setEra(era.PrivateEra).setDate(yearOfEra, month, dayOfMonth);
			if (!JapaneseChronology.JCAL.validate(jdate))
			{
				throw new DateTimeException("year, month, and day not valid for Era");
			}
			LocalDate date = LocalDate.Of(jdate.NormalizedYear, month, dayOfMonth);
			return new JapaneseDate(era, yearOfEra, date);
		}

		/// <summary>
		/// Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		/// system from the proleptic-year, month-of-year and day-of-month fields.
		/// <para>
		/// This returns a {@code JapaneseDate} with the specified fields.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// </para>
		/// <para>
		/// The Japanese proleptic year, month and day-of-month are the same as those
		/// in the ISO calendar system. They are not reset when the era changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the Japanese proleptic-year </param>
		/// <param name="month">  the Japanese month-of-year, from 1 to 12 </param>
		/// <param name="dayOfMonth">  the Japanese day-of-month, from 1 to 31 </param>
		/// <returns> the date in Japanese calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static JapaneseDate Of(int prolepticYear, int month, int dayOfMonth)
		{
			return new JapaneseDate(LocalDate.Of(prolepticYear, month, dayOfMonth));
		}

		/// <summary>
		/// Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		/// system from the era, year-of-era and day-of-year fields.
		/// <para>
		/// This returns a {@code JapaneseDate} with the specified fields.
		/// The day must be valid for the year, otherwise an exception will be thrown.
		/// </para>
		/// <para>
		/// The day-of-year in this factory is expressed relative to the start of the year-of-era.
		/// This definition changes the normal meaning of day-of-year only in those years
		/// where the year-of-era is reset to one due to a change in the era.
		/// For example:
		/// <pre>
		///  6th Jan Showa 64 = day-of-year 6
		///  7th Jan Showa 64 = day-of-year 7
		///  8th Jan Heisei 1 = day-of-year 1
		///  9th Jan Heisei 1 = day-of-year 2
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="era">  the Japanese era, not null </param>
		/// <param name="yearOfEra">  the Japanese year-of-era </param>
		/// <param name="dayOfYear">  the chronology day-of-year, from 1 to 366 </param>
		/// <returns> the date in Japanese calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-year is invalid for the year </exception>
		internal static JapaneseDate OfYearDay(JapaneseEra era, int yearOfEra, int dayOfYear)
		{
			Objects.RequireNonNull(era, "era");
			CalendarDate firstDay = era.PrivateEra.SinceDate;
			LocalGregorianCalendar.Date jdate = JapaneseChronology.JCAL.newCalendarDate(ChronoLocalDate_Fields.Null);
			jdate.Era = era.PrivateEra;
			if (yearOfEra == 1)
			{
				jdate.setDate(yearOfEra, firstDay.Month, firstDay.DayOfMonth + dayOfYear - 1);
			}
			else
			{
				jdate.setDate(yearOfEra, 1, dayOfYear);
			}
			JapaneseChronology.JCAL.normalize(jdate);
			if (era.PrivateEra != jdate.Era || yearOfEra != jdate.Year)
			{
				throw new DateTimeException("Invalid parameters");
			}
			LocalDate localdate = LocalDate.Of(jdate.NormalizedYear, jdate.Month, jdate.DayOfMonth);
			return new JapaneseDate(era, yearOfEra, localdate);
		}

		/// <summary>
		/// Obtains a {@code JapaneseDate} from a temporal object.
		/// <para>
		/// This obtains a date in the Japanese calendar system based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code JapaneseDate}.
		/// </para>
		/// <para>
		/// The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		/// field, which is standardized across calendar systems.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code JapaneseDate::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the date in Japanese calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code JapaneseDate} </exception>
		public static JapaneseDate From(TemporalAccessor temporal)
		{
			return JapaneseChronology.INSTANCE.Date(temporal);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance from an ISO date.
		/// </summary>
		/// <param name="isoDate">  the standard local date, validated not null </param>
		internal JapaneseDate(LocalDate isoDate)
		{
			if (isoDate.IsBefore(MEIJI_6_ISODATE))
			{
				throw new DateTimeException("JapaneseDate before Meiji 6 is not supported");
			}
			LocalGregorianCalendar.Date jdate = ToPrivateJapaneseDate(isoDate);
			this.Era_Renamed = JapaneseEra.ToJapaneseEra(jdate.Era);
			this.YearOfEra = jdate.Year;
			this.IsoDate = isoDate;
		}

		/// <summary>
		/// Constructs a {@code JapaneseDate}. This constructor does NOT validate the given parameters,
		/// and {@code era} and {@code year} must agree with {@code isoDate}.
		/// </summary>
		/// <param name="era">  the era, validated not null </param>
		/// <param name="year">  the year-of-era, validated </param>
		/// <param name="isoDate">  the standard local date, validated not null </param>
		internal JapaneseDate(JapaneseEra era, int year, LocalDate isoDate)
		{
			if (isoDate.IsBefore(MEIJI_6_ISODATE))
			{
				throw new DateTimeException("JapaneseDate before Meiji 6 is not supported");
			}
			this.Era_Renamed = era;
			this.YearOfEra = year;
			this.IsoDate = isoDate;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the chronology of this date, which is the Japanese calendar system.
		/// <para>
		/// The {@code Chronology} represents the calendar system in use.
		/// The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the Japanese chronology, not null </returns>
		public JapaneseChronology Chronology
		{
			get
			{
				return JapaneseChronology.INSTANCE;
			}
		}

		/// <summary>
		/// Gets the era applicable at this date.
		/// <para>
		/// The Japanese calendar system has multiple eras defined by <seealso cref="JapaneseEra"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era applicable at this date, not null </returns>
		public override JapaneseEra Era
		{
			get
			{
				return Era_Renamed;
			}
		}

		/// <summary>
		/// Returns the length of the month represented by this date.
		/// <para>
		/// This returns the length of the month in days.
		/// Month lengths match those of the ISO calendar system.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the month in days </returns>
		public int LengthOfMonth()
		{
			return IsoDate.LengthOfMonth();
		}

		public override int LengthOfYear()
		{
			DateTime jcal = DateTime.GetInstance(JapaneseChronology.LOCALE);
			jcal.set(DateTime.ERA, Era_Renamed.Value + JapaneseEra.ERA_OFFSET);
			jcal = new DateTime(YearOfEra, IsoDate.MonthValue - 1, IsoDate.DayOfMonth);
			return jcal.getActualMaximum(DateTime.DAY_OF_YEAR);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this date can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		/// <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
		/// <li>{@code DAY_OF_WEEK}
		/// <li>{@code DAY_OF_MONTH}
		/// <li>{@code DAY_OF_YEAR}
		/// <li>{@code EPOCH_DAY}
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
		public bool IsSupported(TemporalField field)
		{
			if (field == ALIGNED_DAY_OF_WEEK_IN_MONTH || field == ALIGNED_DAY_OF_WEEK_IN_YEAR || field == ALIGNED_WEEK_OF_MONTH || field == ALIGNED_WEEK_OF_YEAR)
			{
				return false;
			}
			return ChronoLocalDate.this.isSupported(field);
		}

		public override ValueRange java.time.temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field is ChronoField)
			{
				if (IsSupported(field))
				{
					ChronoField f = (ChronoField) field;
					switch (f)
					{
						case DAY_OF_MONTH:
							return ValueRange.Of(1, LengthOfMonth());
						case DAY_OF_YEAR:
							return ValueRange.Of(1, LengthOfYear());
						case YEAR_OF_ERA:
						{
							DateTime jcal = DateTime.GetInstance(JapaneseChronology.LOCALE);
							jcal.set(DateTime.ERA, Era_Renamed.Value + JapaneseEra.ERA_OFFSET);
							jcal = new DateTime(YearOfEra, IsoDate.MonthValue - 1, IsoDate.DayOfMonth);
							return ValueRange.Of(1, jcal.getActualMaximum(DateTime.YEAR));
						}
					}
					return Chronology.Range(f);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.RangeRefinedBy(this);
		}

		public long GetLong(TemporalField field)
		{
			if (field is ChronoField)
			{
				// same as ISO:
				// DAY_OF_WEEK, DAY_OF_MONTH, EPOCH_DAY, MONTH_OF_YEAR, PROLEPTIC_MONTH, YEAR
				//
				// calendar specific fields
				// DAY_OF_YEAR, YEAR_OF_ERA, ERA
				switch ((ChronoField) field)
				{
					case ALIGNED_DAY_OF_WEEK_IN_MONTH:
					case ALIGNED_DAY_OF_WEEK_IN_YEAR:
					case ALIGNED_WEEK_OF_MONTH:
					case ALIGNED_WEEK_OF_YEAR:
						throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
					case YEAR_OF_ERA:
						return YearOfEra;
					case ERA:
						return Era_Renamed.Value;
					case DAY_OF_YEAR:
						DateTime jcal = DateTime.GetInstance(JapaneseChronology.LOCALE);
						jcal.set(DateTime.ERA, Era_Renamed.Value + JapaneseEra.ERA_OFFSET);
						jcal = new DateTime(YearOfEra, IsoDate.MonthValue - 1, IsoDate.DayOfMonth);
						return jcal.DayOfYear;
				}
				return IsoDate.GetLong(field);
			}
			return field.GetFrom(this);
		}

		/// <summary>
		/// Returns a {@code LocalGregorianCalendar.Date} converted from the given {@code isoDate}.
		/// </summary>
		/// <param name="isoDate">  the local date, not null </param>
		/// <returns> a {@code LocalGregorianCalendar.Date}, not null </returns>
		private static LocalGregorianCalendar.Date ToPrivateJapaneseDate(LocalDate isoDate)
		{
			LocalGregorianCalendar.Date jdate = JapaneseChronology.JCAL.newCalendarDate(ChronoLocalDate_Fields.Null);
			sun.util.calendar.Era sunEra = JapaneseEra.PrivateEraFrom(isoDate);
			int year = isoDate.Year;
			if (sunEra != ChronoLocalDate_Fields.Null)
			{
				year -= sunEra.SinceDate.Year - 1;
			}
			jdate.setEra(sunEra).setYear(year).setMonth(isoDate.MonthValue).setDayOfMonth(isoDate.DayOfMonth);
			JapaneseChronology.JCAL.normalize(jdate);
			return jdate;
		}

		//-----------------------------------------------------------------------
		public JapaneseDate With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				if (GetLong(f) == newValue) // getLong() validates for supported fields
				{
					return this;
				}
				switch (f)
				{
					case YEAR_OF_ERA:
					case YEAR:
					case ERA:
					{
						int nvalue = Chronology.Range(f).CheckValidIntValue(newValue, f);
						switch (f)
						{
							case YEAR_OF_ERA:
								return this.WithYear(nvalue);
							case YEAR:
								return With(IsoDate.WithYear(nvalue));
							case ERA:
							{
								return this.WithYear(JapaneseEra.Of(nvalue), YearOfEra);
							}
						}
					}
				break;
				}
				// YEAR, PROLEPTIC_MONTH and others are same as ISO
				return With(IsoDate.With(field, newValue));
			}
			return base.With(field, newValue);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override JapaneseDate With(TemporalAdjuster adjuster)
		{
			return base.With(adjuster);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override JapaneseDate Plus(TemporalAmount amount)
		{
			return base.Plus(amount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override JapaneseDate Minus(TemporalAmount amount)
		{
			return base.Minus(amount);
		}
		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date with the year altered.
		/// <para>
		/// This method changes the year of the date.
		/// If the month-day is invalid for the year, then the previous valid day
		/// will be selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="era">  the era to set in the result, not null </param>
		/// <param name="yearOfEra">  the year-of-era to set in the returned date </param>
		/// <returns> a {@code JapaneseDate} based on this date with the requested year, never null </returns>
		/// <exception cref="DateTimeException"> if {@code year} is invalid </exception>
		private JapaneseDate WithYear(JapaneseEra era, int yearOfEra)
		{
			int year = JapaneseChronology.INSTANCE.ProlepticYear(era, yearOfEra);
			return With(IsoDate.WithYear(year));
		}

		/// <summary>
		/// Returns a copy of this date with the year-of-era altered.
		/// <para>
		/// This method changes the year-of-era of the date.
		/// If the month-day is invalid for the year, then the previous valid day
		/// will be selected instead.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to set in the returned date </param>
		/// <returns> a {@code JapaneseDate} based on this date with the requested year-of-era, never null </returns>
		/// <exception cref="DateTimeException"> if {@code year} is invalid </exception>
		private JapaneseDate WithYear(int year)
		{
			return WithYear(Era, year);
		}

		//-----------------------------------------------------------------------
		internal override JapaneseDate PlusYears(long years)
		{
			return With(IsoDate.PlusYears(years));
		}

		internal override JapaneseDate PlusMonths(long months)
		{
			return With(IsoDate.PlusMonths(months));
		}

		internal override JapaneseDate PlusWeeks(long weeksToAdd)
		{
			return With(IsoDate.PlusWeeks(weeksToAdd));
		}

		internal override JapaneseDate PlusDays(long days)
		{
			return With(IsoDate.PlusDays(days));
		}

		public JapaneseDate Plus(long amountToAdd, TemporalUnit unit)
		{
			return base.Plus(amountToAdd, unit);
		}

		public override JapaneseDate Minus(long amountToAdd, TemporalUnit unit)
		{
			return base.Minus(amountToAdd, unit);
		}

		internal override JapaneseDate MinusYears(long yearsToSubtract)
		{
			return base.MinusYears(yearsToSubtract);
		}

		internal override JapaneseDate MinusMonths(long monthsToSubtract)
		{
			return base.MinusMonths(monthsToSubtract);
		}

		internal override JapaneseDate MinusWeeks(long weeksToSubtract)
		{
			return base.MinusWeeks(weeksToSubtract);
		}

		internal override JapaneseDate MinusDays(long daysToSubtract)
		{
			return base.MinusDays(daysToSubtract);
		}

		private JapaneseDate With(LocalDate newDate)
		{
			return (newDate.Equals(IsoDate) ? this : new JapaneseDate(newDate));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final ChronoLocalDateTime<JapaneseDate> atTime(java.time.LocalTime localTime)
		public override ChronoLocalDateTime<JapaneseDate> AtTime(LocalTime localTime) // for javadoc and covariant return type
		{
			return (ChronoLocalDateTime<JapaneseDate>)base.AtTime(localTime);
		}

		public ChronoPeriod Until(ChronoLocalDate endDate)
		{
			Period period = IsoDate.Until(endDate);
			return Chronology.period(period.Years, period.Months, period.Days);
		}

		public override long ToEpochDay() // override for performance
		{
			return IsoDate.ToEpochDay();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Compares this date to another date, including the chronology.
		/// <para>
		/// Compares this {@code JapaneseDate} with another ensuring that the date is the same.
		/// </para>
		/// <para>
		/// Only objects of type {@code JapaneseDate} are compared, other types return false.
		/// To compare the dates of two {@code TemporalAccessor} instances, including dates
		/// in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date </returns>
		public override bool Equals(Object obj) // override for performance
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is JapaneseDate)
			{
				JapaneseDate otherDate = (JapaneseDate) obj;
				return this.IsoDate.Equals(otherDate.IsoDate);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date.
		/// </summary>
		/// <returns> a suitable hash code based only on the Chronology and the date </returns>
		public override int HashCode() // override for performance
		{
			return Chronology.Id.HashCode() ^ IsoDate.HashCode();
		}

		//-----------------------------------------------------------------------
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

		/// <summary>
		/// Writes the object using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(4);                 // identifies a JapaneseDate
		///  out.writeInt(get(YEAR));
		///  out.writeByte(get(MONTH_OF_YEAR));
		///  out.writeByte(get(DAY_OF_MONTH));
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.JAPANESE_DATE_TYPE, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			// JapaneseChronology is implicit in the JAPANESE_DATE_TYPE
			@out.WriteInt(get(YEAR));
			@out.WriteByte(get(MONTH_OF_YEAR));
			@out.WriteByte(get(DAY_OF_MONTH));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static JapaneseDate readExternal(java.io.DataInput in) throws java.io.IOException
		internal static JapaneseDate ReadExternal(DataInput @in)
		{
			int year = @in.ReadInt();
			int month = @in.ReadByte();
			int dayOfMonth = @in.ReadByte();
			return JapaneseChronology.INSTANCE.Date(year, month, dayOfMonth);
		}

	}

}