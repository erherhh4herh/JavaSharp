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



	/// <summary>
	/// A date in the Hijrah calendar system.
	/// <para>
	/// This date operates using one of several variants of the
	/// <seealso cref="HijrahChronology Hijrah calendar"/>.
	/// </para>
	/// <para>
	/// The Hijrah calendar has a different total of days in a year than
	/// Gregorian calendar, and the length of each month is based on the period
	/// of a complete revolution of the moon around the earth
	/// (as between successive new moons).
	/// Refer to the <seealso cref="HijrahChronology"/> for details of supported variants.
	/// </para>
	/// <para>
	/// Each HijrahDate is created bound to a particular HijrahChronology,
	/// The same chronology is propagated to each HijrahDate computed from the date.
	/// To use a different Hijrah variant, its HijrahChronology can be used
	/// to create new HijrahDate instances.
	/// Alternatively, the <seealso cref="#withVariant"/> method can be used to convert
	/// to a new HijrahChronology.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code HijrahDate} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class HijrahDate : ChronoLocalDateImpl<HijrahDate>, ChronoLocalDate
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -5207853542612002020L;
		/// <summary>
		/// The Chronology of this HijrahDate.
		/// </summary>
		[NonSerialized]
		private readonly HijrahChronology ChronoLocalDate_Fields;
		/// <summary>
		/// The proleptic year.
		/// </summary>
		[NonSerialized]
		private readonly int ProlepticYear;
		/// <summary>
		/// The month-of-year.
		/// </summary>
		[NonSerialized]
		private readonly int MonthOfYear;
		/// <summary>
		/// The day-of-month.
		/// </summary>
		[NonSerialized]
		private readonly int DayOfMonth;

		//-------------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code HijrahDate} from the Hijrah proleptic year,
		/// month-of-year and day-of-month.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic year to represent in the Hijrah calendar </param>
		/// <param name="monthOfYear">  the month-of-year to represent, from 1 to 12 </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 30 </param>
		/// <returns> the Hijrah date, never null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		internal static HijrahDate Of(HijrahChronology ChronoLocalDate_Fields, int prolepticYear, int monthOfYear, int dayOfMonth)
		{
			return new HijrahDate(ChronoLocalDate_Fields.Chrono, prolepticYear, monthOfYear, dayOfMonth);
		}

		/// <summary>
		/// Returns a HijrahDate for the chronology and epochDay. </summary>
		/// <param name="chrono"> The Hijrah chronology </param>
		/// <param name="epochDay"> the epoch day </param>
		/// <returns> a HijrahDate for the epoch day; non-null </returns>
		internal static HijrahDate OfEpochDay(HijrahChronology ChronoLocalDate_Fields, long epochDay)
		{
			return new HijrahDate(ChronoLocalDate_Fields.Chrono, epochDay);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		/// in the default time-zone.
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
		public static HijrahDate Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		/// in the specified time-zone.
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
		public static HijrahDate Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		/// from the specified clock.
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
		public static HijrahDate Now(Clock clock)
		{
			return HijrahDate.OfEpochDay(HijrahChronology.INSTANCE, LocalDate.Now(clock).ToEpochDay());
		}

		/// <summary>
		/// Obtains a {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		/// from the proleptic-year, month-of-year and day-of-month fields.
		/// <para>
		/// This returns a {@code HijrahDate} with the specified fields.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the Hijrah proleptic-year </param>
		/// <param name="month">  the Hijrah month-of-year, from 1 to 12 </param>
		/// <param name="dayOfMonth">  the Hijrah day-of-month, from 1 to 30 </param>
		/// <returns> the date in Hijrah calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static HijrahDate Of(int prolepticYear, int month, int dayOfMonth)
		{
			return HijrahChronology.INSTANCE.Date(prolepticYear, month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a {@code HijrahDate} of the Islamic Umm Al-Qura calendar from a temporal object.
		/// <para>
		/// This obtains a date in the Hijrah calendar system based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code HijrahDate}.
		/// </para>
		/// <para>
		/// The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		/// field, which is standardized across calendar systems.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code HijrahDate::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the date in Hijrah calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code HijrahDate} </exception>
		public static HijrahDate From(TemporalAccessor temporal)
		{
			return HijrahChronology.INSTANCE.Date(temporal);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructs an {@code HijrahDate} with the proleptic-year, month-of-year and
		/// day-of-month fields.
		/// </summary>
		/// <param name="chrono"> The chronology to create the date with </param>
		/// <param name="prolepticYear"> the proleptic year </param>
		/// <param name="monthOfYear"> the month of year </param>
		/// <param name="dayOfMonth"> the day of month </param>
		private HijrahDate(HijrahChronology ChronoLocalDate_Fields, int prolepticYear, int monthOfYear, int dayOfMonth)
		{
			// Computing the Gregorian day checks the valid ranges
			ChronoLocalDate_Fields.Chrono.getEpochDay(prolepticYear, monthOfYear, dayOfMonth);

			this.Chrono = ChronoLocalDate_Fields.Chrono;
			this.ProlepticYear = prolepticYear;
			this.MonthOfYear = monthOfYear;
			this.DayOfMonth = dayOfMonth;
		}

		/// <summary>
		/// Constructs an instance with the Epoch Day.
		/// </summary>
		/// <param name="epochDay">  the epochDay </param>
		private HijrahDate(HijrahChronology ChronoLocalDate_Fields, long epochDay)
		{
			int[] dateInfo = ChronoLocalDate_Fields.Chrono.getHijrahDateInfo((int)epochDay);

			this.Chrono = ChronoLocalDate_Fields.Chrono;
			this.ProlepticYear = dateInfo[0];
			this.MonthOfYear = dateInfo[1];
			this.DayOfMonth = dateInfo[2];
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the chronology of this date, which is the Hijrah calendar system.
		/// <para>
		/// The {@code Chronology} represents the calendar system in use.
		/// The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the Hijrah chronology, not null </returns>
		public HijrahChronology Chronology
		{
			get
			{
				return ChronoLocalDate_Fields.Chrono;
			}
		}

		/// <summary>
		/// Gets the era applicable at this date.
		/// <para>
		/// The Hijrah calendar system has one era, 'AH',
		/// defined by <seealso cref="HijrahEra"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era applicable at this date, not null </returns>
		public override HijrahEra Era
		{
			get
			{
				return HijrahEra.AH;
			}
		}

		/// <summary>
		/// Returns the length of the month represented by this date.
		/// <para>
		/// This returns the length of the month in days.
		/// Month lengths in the Hijrah calendar system vary between 29 and 30 days.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the month in days </returns>
		public int LengthOfMonth()
		{
			return ChronoLocalDate_Fields.Chrono.getMonthLength(ProlepticYear, MonthOfYear);
		}

		/// <summary>
		/// Returns the length of the year represented by this date.
		/// <para>
		/// This returns the length of the year in days.
		/// A Hijrah calendar system year is typically shorter than
		/// that of the ISO calendar system.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the year in days </returns>
		public override int LengthOfYear()
		{
			return ChronoLocalDate_Fields.Chrono.getYearLength(ProlepticYear);
		}

		//-----------------------------------------------------------------------
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
						case ALIGNED_WEEK_OF_MONTH: // TODO
							return ValueRange.Of(1, 5);
						// TODO does the limited range of valid years cause years to
						// start/end part way through? that would affect range
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
				switch ((ChronoField) field)
				{
					case DAY_OF_WEEK:
						return DayOfWeek;
					case ALIGNED_DAY_OF_WEEK_IN_MONTH:
						return ((DayOfWeek - 1) % 7) + 1;
					case ALIGNED_DAY_OF_WEEK_IN_YEAR:
						return ((DayOfYear - 1) % 7) + 1;
					case DAY_OF_MONTH:
						return this.DayOfMonth;
					case DAY_OF_YEAR:
						return this.DayOfYear;
					case EPOCH_DAY:
						return ToEpochDay();
					case ALIGNED_WEEK_OF_MONTH:
						return ((DayOfMonth - 1) / 7) + 1;
					case ALIGNED_WEEK_OF_YEAR:
						return ((DayOfYear - 1) / 7) + 1;
					case MONTH_OF_YEAR:
						return MonthOfYear;
					case PROLEPTIC_MONTH:
						return ProlepticMonth;
					case YEAR_OF_ERA:
						return ProlepticYear;
					case YEAR:
						return ProlepticYear;
					case ERA:
						return EraValue;
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

		private long ProlepticMonth
		{
			get
			{
				return ProlepticYear * 12L + MonthOfYear - 1;
			}
		}

		public HijrahDate With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				// not using checkValidIntValue so EPOCH_DAY and PROLEPTIC_MONTH work
				ChronoLocalDate_Fields.Chrono.Range(f).CheckValidValue(newValue, f); // TODO: validate value
				int nvalue = (int) newValue;
				switch (f)
				{
					case DAY_OF_WEEK:
						return PlusDays(newValue - DayOfWeek);
					case ALIGNED_DAY_OF_WEEK_IN_MONTH:
						return PlusDays(newValue - GetLong(ALIGNED_DAY_OF_WEEK_IN_MONTH));
					case ALIGNED_DAY_OF_WEEK_IN_YEAR:
						return PlusDays(newValue - GetLong(ALIGNED_DAY_OF_WEEK_IN_YEAR));
					case DAY_OF_MONTH:
						return ResolvePreviousValid(ProlepticYear, MonthOfYear, nvalue);
					case DAY_OF_YEAR:
						return PlusDays(System.Math.Min(nvalue, LengthOfYear()) - DayOfYear);
					case EPOCH_DAY:
						return new HijrahDate(ChronoLocalDate_Fields.Chrono, newValue);
					case ALIGNED_WEEK_OF_MONTH:
						return PlusDays((newValue - GetLong(ALIGNED_WEEK_OF_MONTH)) * 7);
					case ALIGNED_WEEK_OF_YEAR:
						return PlusDays((newValue - GetLong(ALIGNED_WEEK_OF_YEAR)) * 7);
					case MONTH_OF_YEAR:
						return ResolvePreviousValid(ProlepticYear, nvalue, DayOfMonth);
					case PROLEPTIC_MONTH:
						return PlusMonths(newValue - ProlepticMonth);
					case YEAR_OF_ERA:
						return ResolvePreviousValid(ProlepticYear >= 1 ? nvalue : 1 - nvalue, MonthOfYear, DayOfMonth);
					case YEAR:
						return ResolvePreviousValid(nvalue, MonthOfYear, DayOfMonth);
					case ERA:
						return ResolvePreviousValid(1 - ProlepticYear, MonthOfYear, DayOfMonth);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return base.With(field, newValue);
		}

		private HijrahDate ResolvePreviousValid(int prolepticYear, int month, int day)
		{
			int monthDays = ChronoLocalDate_Fields.Chrono.getMonthLength(prolepticYear, month);
			if (day > monthDays)
			{
				day = monthDays;
			}
			return HijrahDate.Of(ChronoLocalDate_Fields.Chrono, prolepticYear, month, day);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> if unable to make the adjustment.
		///     For example, if the adjuster requires an ISO chronology </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override HijrahDate With(TemporalAdjuster adjuster)
		{
			return base.With(adjuster);
		}

		/// <summary>
		/// Returns a {@code HijrahDate} with the Chronology requested.
		/// <para>
		/// The year, month, and day are checked against the new requested
		/// HijrahChronology.  If the chronology has a shorter month length
		/// for the month, the day is reduced to be the last day of the month.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chronology"> the new HijrahChonology, non-null </param>
		/// <returns> a HijrahDate with the requested HijrahChronology, non-null </returns>
		public HijrahDate WithVariant(HijrahChronology chronology)
		{
			if (ChronoLocalDate_Fields.Chrono == chronology)
			{
				return this;
			}
			// Like resolvePreviousValid the day is constrained to stay in the same month
			int monthDays = chronology.GetDayOfYear(ProlepticYear, MonthOfYear);
			return HijrahDate.Of(chronology, ProlepticYear, MonthOfYear,(DayOfMonth > monthDays) ? monthDays : DayOfMonth);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override HijrahDate Plus(TemporalAmount amount)
		{
			return base.Plus(amount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override HijrahDate Minus(TemporalAmount amount)
		{
			return base.Minus(amount);
		}

		public override long ToEpochDay()
		{
			return ChronoLocalDate_Fields.Chrono.getEpochDay(ProlepticYear, MonthOfYear, DayOfMonth);
		}

		/// <summary>
		/// Gets the day-of-year field.
		/// <para>
		/// This method returns the primitive {@code int} value for the day-of-year.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-year </returns>
		private int DayOfYear
		{
			get
			{
				return ChronoLocalDate_Fields.Chrono.getDayOfYear(ProlepticYear, MonthOfYear) + DayOfMonth;
			}
		}

		/// <summary>
		/// Gets the day-of-week value.
		/// </summary>
		/// <returns> the day-of-week; computed from the epochday </returns>
		private int DayOfWeek
		{
			get
			{
				int dow0 = (int)Math.FloorMod(ToEpochDay() + 3, 7);
				return dow0 + 1;
			}
		}

		/// <summary>
		/// Gets the Era of this date.
		/// </summary>
		/// <returns> the Era of this date; computed from epochDay </returns>
		private int EraValue
		{
			get
			{
				return (ProlepticYear > 1 ? 1 : 0);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the year is a leap year, according to the Hijrah calendar system rules.
		/// </summary>
		/// <returns> true if this date is in a leap year </returns>
		public override bool LeapYear
		{
			get
			{
				return ChronoLocalDate_Fields.Chrono.IsLeapYear(ProlepticYear);
			}
		}

		//-----------------------------------------------------------------------
		internal override HijrahDate PlusYears(long years)
		{
			if (years == 0)
			{
				return this;
			}
			int newYear = Math.AddExact(this.ProlepticYear, (int)years);
			return ResolvePreviousValid(newYear, MonthOfYear, DayOfMonth);
		}

		internal override HijrahDate PlusMonths(long monthsToAdd)
		{
			if (monthsToAdd == 0)
			{
				return this;
			}
			long monthCount = ProlepticYear * 12L + (MonthOfYear - 1);
			long calcMonths = monthCount + monthsToAdd; // safe overflow
			int newYear = ChronoLocalDate_Fields.Chrono.checkValidYear(Math.FloorDiv(calcMonths, 12L));
			int newMonth = (int)Math.FloorMod(calcMonths, 12L) + 1;
			return ResolvePreviousValid(newYear, newMonth, DayOfMonth);
		}

		internal override HijrahDate PlusWeeks(long weeksToAdd)
		{
			return base.PlusWeeks(weeksToAdd);
		}

		internal override HijrahDate PlusDays(long days)
		{
			return new HijrahDate(ChronoLocalDate_Fields.Chrono, ToEpochDay() + days);
		}

		public HijrahDate Plus(long amountToAdd, TemporalUnit unit)
		{
			return base.Plus(amountToAdd, unit);
		}

		public override HijrahDate Minus(long amountToSubtract, TemporalUnit unit)
		{
			return base.Minus(amountToSubtract, unit);
		}

		internal override HijrahDate MinusYears(long yearsToSubtract)
		{
			return base.MinusYears(yearsToSubtract);
		}

		internal override HijrahDate MinusMonths(long monthsToSubtract)
		{
			return base.MinusMonths(monthsToSubtract);
		}

		internal override HijrahDate MinusWeeks(long weeksToSubtract)
		{
			return base.MinusWeeks(weeksToSubtract);
		}

		internal override HijrahDate MinusDays(long daysToSubtract)
		{
			return base.MinusDays(daysToSubtract);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final ChronoLocalDateTime<HijrahDate> atTime(java.time.LocalTime localTime)
		public override ChronoLocalDateTime<HijrahDate> AtTime(LocalTime localTime) // for javadoc and covariant return type
		{
			return (ChronoLocalDateTime<HijrahDate>)base.AtTime(localTime);
		}

		public ChronoPeriod Until(ChronoLocalDate endDate)
		{
			// TODO: untested
			HijrahDate end = Chronology.Date(endDate);
			long totalMonths = (end.ProlepticYear - this.ProlepticYear) * 12 + (end.MonthOfYear - this.MonthOfYear); // safe
			int days = end.DayOfMonth - this.DayOfMonth;
			if (totalMonths > 0 && days < 0)
			{
				totalMonths--;
				HijrahDate calcDate = this.PlusMonths(totalMonths);
				days = (int)(end.ToEpochDay() - calcDate.ToEpochDay()); // safe
			}
			else if (totalMonths < 0 && days > 0)
			{
				totalMonths++;
				days -= end.LengthOfMonth();
			}
			long years = totalMonths / 12; // safe
			int months = (int)(totalMonths % 12); // safe
			return Chronology.period(Math.ToIntExact(years), months, days);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Compares this date to another date, including the chronology.
		/// <para>
		/// Compares this {@code HijrahDate} with another ensuring that the date is the same.
		/// </para>
		/// <para>
		/// Only objects of type {@code HijrahDate} are compared, other types return false.
		/// To compare the dates of two {@code TemporalAccessor} instances, including dates
		/// in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date and the Chronologies are equal </returns>
		public override bool Equals(Object obj) // override for performance
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is HijrahDate)
			{
				HijrahDate otherDate = (HijrahDate) obj;
				return ProlepticYear == otherDate.ProlepticYear && this.MonthOfYear == otherDate.MonthOfYear && this.DayOfMonth == otherDate.DayOfMonth && Chronology.Equals(otherDate.Chronology);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date.
		/// </summary>
		/// <returns> a suitable hash code based only on the Chronology and the date </returns>
		public override int HashCode() // override for performance
		{
			int yearValue = ProlepticYear;
			int monthValue = MonthOfYear;
			int dayValue = DayOfMonth;
			return Chronology.Id.HashCode() ^ (yearValue & 0xFFFFF800) ^ ((yearValue << 11) + (monthValue << 6) + (dayValue));
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
		///  out.writeByte(6);                 // identifies a HijrahDate
		///  out.writeObject(chrono);          // the HijrahChronology variant
		///  out.writeInt(get(YEAR));
		///  out.writeByte(get(MONTH_OF_YEAR));
		///  out.writeByte(get(DAY_OF_MONTH));
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.HIJRAH_DATE_TYPE, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		internal void WriteExternal(ObjectOutput @out)
		{
			// HijrahChronology is implicit in the Hijrah_DATE_TYPE
			@out.WriteObject(Chronology);
			@out.WriteInt(get(YEAR));
			@out.WriteByte(get(MONTH_OF_YEAR));
			@out.WriteByte(get(DAY_OF_MONTH));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static HijrahDate readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		internal static HijrahDate ReadExternal(ObjectInput @in)
		{
			HijrahChronology ChronoLocalDate_Fields.Chrono = (HijrahChronology) @in.ReadObject();
			int year = @in.ReadInt();
			int month = @in.ReadByte();
			int dayOfMonth = @in.ReadByte();
			return ChronoLocalDate_Fields.Chrono.Date(year, month, dayOfMonth);
		}

	}

}