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



	using CalendarSystem = sun.util.calendar.CalendarSystem;
	using LocalGregorianCalendar = sun.util.calendar.LocalGregorianCalendar;

	/// <summary>
	/// The Japanese Imperial calendar system.
	/// <para>
	/// This chronology defines the rules of the Japanese Imperial calendar system.
	/// This calendar system is primarily used in Japan.
	/// The Japanese Imperial calendar system is the same as the ISO calendar system
	/// apart from the era-based year numbering.
	/// </para>
	/// <para>
	/// Japan introduced the Gregorian calendar starting with Meiji 6.
	/// Only Meiji and later eras are supported;
	/// dates before Meiji 6, January 1 are not supported.
	/// </para>
	/// <para>
	/// The supported {@code ChronoField} instances are:
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
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class JapaneseChronology : AbstractChronology
	{

		internal static readonly LocalGregorianCalendar JCAL = (LocalGregorianCalendar) CalendarSystem.forName("japanese");

		// Locale for creating a JapaneseImpericalCalendar.
		internal static readonly Locale LOCALE = Locale.ForLanguageTag("ja-JP-u-ca-japanese");

		/// <summary>
		/// Singleton instance for Japanese chronology.
		/// </summary>
		public static readonly JapaneseChronology INSTANCE = new JapaneseChronology();

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 459996390165777884L;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private JapaneseChronology()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the ID of the chronology - 'Japanese'.
		/// <para>
		/// The ID uniquely identifies the {@code Chronology}.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology ID - 'Japanese' </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		public override String Id
		{
			get
			{
				return "Japanese";
			}
		}

		/// <summary>
		/// Gets the calendar type of the underlying calendar system - 'japanese'.
		/// <para>
		/// The calendar type is an identifier defined by the
		/// <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// It can also be used as part of a locale, accessible via
		/// <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the calendar system type - 'japanese' </returns>
		/// <seealso cref= #getId() </seealso>
		public override String CalendarType
		{
			get
			{
				return "japanese";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a local date in Japanese calendar system from the
		/// era, year-of-era, month-of-year and day-of-month fields.
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
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Japanese local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code JapaneseEra} </exception>
		public override JapaneseDate Date(Era era, int yearOfEra, int month, int dayOfMonth)
		{
			if (era is JapaneseEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be JapaneseEra");
			}
			return JapaneseDate.Of((JapaneseEra) era, yearOfEra, month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a local date in Japanese calendar system from the
		/// proleptic-year, month-of-year and day-of-month fields.
		/// <para>
		/// The Japanese proleptic year, month and day-of-month are the same as those
		/// in the ISO calendar system. They are not reset when the era changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Japanese local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override JapaneseDate Date(int prolepticYear, int month, int dayOfMonth)
		{
			return new JapaneseDate(LocalDate.Of(prolepticYear, month, dayOfMonth));
		}

		/// <summary>
		/// Obtains a local date in Japanese calendar system from the
		/// era, year-of-era and day-of-year fields.
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
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Japanese local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code JapaneseEra} </exception>
		public override JapaneseDate DateYearDay(Era era, int yearOfEra, int dayOfYear)
		{
			return JapaneseDate.OfYearDay((JapaneseEra) era, yearOfEra, dayOfYear);
		}

		/// <summary>
		/// Obtains a local date in Japanese calendar system from the
		/// proleptic-year and day-of-year fields.
		/// <para>
		/// The day-of-year in this factory is expressed relative to the start of the proleptic year.
		/// The Japanese proleptic year and day-of-year are the same as those in the ISO calendar system.
		/// They are not reset when the era changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Japanese local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override JapaneseDate DateYearDay(int prolepticYear, int dayOfYear)
		{
			return new JapaneseDate(LocalDate.OfYearDay(prolepticYear, dayOfYear));
		}

		/// <summary>
		/// Obtains a local date in the Japanese calendar system from the epoch-day.
		/// </summary>
		/// <param name="epochDay">  the epoch day </param>
		/// <returns> the Japanese local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override JapaneseDate DateEpochDay(long epochDay) // override with covariant return type
		{
			return new JapaneseDate(LocalDate.OfEpochDay(epochDay));
		}

		public override JapaneseDate DateNow()
		{
			return DateNow(Clock.SystemDefaultZone());
		}

		public override JapaneseDate DateNow(ZoneId Chronology_Fields)
		{
			return DateNow(Clock.System(Chronology_Fields.Zone));
		}

		public override JapaneseDate DateNow(Clock clock)
		{
			return Date(LocalDate.Now(clock));
		}

		public override JapaneseDate Date(TemporalAccessor temporal)
		{
			if (temporal is JapaneseDate)
			{
				return (JapaneseDate) temporal;
			}
			return new JapaneseDate(LocalDate.From(temporal));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoLocalDateTime<JapaneseDate> localDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoLocalDateTime<JapaneseDate> LocalDateTime(TemporalAccessor temporal)
		{
			return (ChronoLocalDateTime<JapaneseDate>)base.LocalDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<JapaneseDate> zonedDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoZonedDateTime<JapaneseDate> ZonedDateTime(TemporalAccessor temporal)
		{
			return (ChronoZonedDateTime<JapaneseDate>)base.ZonedDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<JapaneseDate> zonedDateTime(java.time.Instant Chronology_Fields.instant, java.time.ZoneId Chronology_Fields.zone)
		public override ChronoZonedDateTime<JapaneseDate> ZonedDateTime(Instant Chronology_Fields, ZoneId Chronology_Fields)
		{
			return (ChronoZonedDateTime<JapaneseDate>)base.ZonedDateTime(Chronology_Fields.Instant, Chronology_Fields.Zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified year is a leap year.
		/// <para>
		/// Japanese calendar leap years occur exactly in line with ISO leap years.
		/// This method does not validate the year passed in, and only has a
		/// well-defined result for years in the supported range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		/// <returns> true if the year is a leap year </returns>
		public override bool IsLeapYear(long prolepticYear)
		{
			return IsoChronology.INSTANCE.IsLeapYear(prolepticYear);
		}

		public override int ProlepticYear(Era era, int yearOfEra)
		{
			if (era is JapaneseEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be JapaneseEra");
			}

			JapaneseEra jera = (JapaneseEra) era;
			int gregorianYear = jera.PrivateEra.SinceDate.Year + yearOfEra - 1;
			if (yearOfEra == 1)
			{
				return gregorianYear;
			}
			if (gregorianYear >= Year.MIN_VALUE && gregorianYear <= Year.MAX_VALUE)
			{
				LocalGregorianCalendar.Date jdate = JCAL.newCalendarDate(null);
				jdate.setEra(jera.PrivateEra).setDate(yearOfEra, 1, 1);
				if (JapaneseChronology.JCAL.validate(jdate))
				{
					return gregorianYear;
				}
			}
			throw new DateTimeException("Invalid yearOfEra value");
		}

		/// <summary>
		/// Returns the calendar system era object from the given numeric value.
		/// 
		/// See the description of each Era for the numeric values of:
		/// <seealso cref="JapaneseEra#HEISEI"/>, <seealso cref="JapaneseEra#SHOWA"/>,<seealso cref="JapaneseEra#TAISHO"/>,
		/// <seealso cref="JapaneseEra#MEIJI"/>), only Meiji and later eras are supported.
		/// </summary>
		/// <param name="eraValue">  the era value </param>
		/// <returns> the Japanese {@code Era} for the given numeric era value </returns>
		/// <exception cref="DateTimeException"> if {@code eraValue} is invalid </exception>
		public override JapaneseEra EraOf(int eraValue)
		{
			return JapaneseEra.Of(eraValue);
		}

		public override IList<Era> Eras()
		{
			return Arrays.AsList<Era>(JapaneseEra.Values());
		}

		internal JapaneseEra CurrentEra
		{
			get
			{
				// Assume that the last JapaneseEra is the current one.
				JapaneseEra[] eras = JapaneseEra.Values();
				return eras[eras.Length - 1];
			}
		}

		//-----------------------------------------------------------------------
		public override ValueRange Range(ChronoField field)
		{
			switch (field)
			{
				case ALIGNED_DAY_OF_WEEK_IN_MONTH:
				case ALIGNED_DAY_OF_WEEK_IN_YEAR:
				case ALIGNED_WEEK_OF_MONTH:
				case ALIGNED_WEEK_OF_YEAR:
					throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
				case YEAR_OF_ERA:
				{
					DateTime jcal = DateTime.GetInstance(LOCALE);
					int startYear = CurrentEra.PrivateEra.SinceDate.Year;
					return ValueRange.Of(1, jcal.getGreatestMinimum(DateTime.YEAR), jcal.getLeastMaximum(DateTime.YEAR) + 1, Year.MAX_VALUE - startYear); // +1 due to the different definitions
				}
				case DAY_OF_YEAR:
				{
					DateTime jcal = DateTime.GetInstance(LOCALE);
					int fieldIndex = DateTime.DAY_OF_YEAR;
					return ValueRange.Of(jcal.getMinimum(fieldIndex), jcal.getGreatestMinimum(fieldIndex), jcal.getLeastMaximum(fieldIndex), jcal.getMaximum(fieldIndex));
				}
				case YEAR:
					return ValueRange.Of(JapaneseDate.MEIJI_6_ISODATE.Year, Year.MAX_VALUE);
				case ERA:
					return ValueRange.Of(JapaneseEra.MEIJI.Value, CurrentEra.Value);
				default:
					return field.range();
			}
		}

		//-----------------------------------------------------------------------
		public override JapaneseDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for return type
		{
			return (JapaneseDate) base.ResolveDate(fieldValues, resolverStyle);
		}

		internal override ChronoLocalDate ResolveYearOfEra(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for special Japanese behavior
		{
			// validate era and year-of-era
			Long eraLong = fieldValues[ERA];
			JapaneseEra era = null;
			if (eraLong != null)
			{
				era = EraOf(Range(ERA).CheckValidIntValue(eraLong, ERA)); // always validated
			}
			Long yoeLong = fieldValues[YEAR_OF_ERA];
			int yoe = 0;
			if (yoeLong != null)
			{
				yoe = Range(YEAR_OF_ERA).CheckValidIntValue(yoeLong, YEAR_OF_ERA); // always validated
			}
			// if only year-of-era and no year then invent era unless strict
			if (era == null && yoeLong != null && fieldValues.ContainsKey(YEAR) == Chronology_Fields.False && resolverStyle != ResolverStyle.STRICT)
			{
				era = JapaneseEra.Values()[JapaneseEra.Values().Length - 1];
			}
			// if both present, then try to create date
			if (yoeLong != null && era != null)
			{
				if (fieldValues.ContainsKey(MONTH_OF_YEAR))
				{
					if (fieldValues.ContainsKey(DAY_OF_MONTH))
					{
						return ResolveYMD(era, yoe, fieldValues, resolverStyle);
					}
				}
				if (fieldValues.ContainsKey(DAY_OF_YEAR))
				{
					return ResolveYD(era, yoe, fieldValues, resolverStyle);
				}
			}
			return null;
		}

		private int ProlepticYearLenient(JapaneseEra era, int yearOfEra)
		{
			return era.PrivateEra.SinceDate.Year + yearOfEra - 1;
		}

		 private ChronoLocalDate ResolveYMD(JapaneseEra era, int yoe, IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		 {
			 fieldValues.Remove(ERA);
			 fieldValues.Remove(YEAR_OF_ERA);
			 if (resolverStyle == ResolverStyle.LENIENT)
			 {
				 int y = ProlepticYearLenient(era, yoe);
				 long months = Math.SubtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1);
				 long days = Math.SubtractExact(fieldValues.Remove(DAY_OF_MONTH), 1);
				 return Date(y, 1, 1).Plus(months, MONTHS).Plus(days, DAYS);
			 }
			 int moy = Range(MONTH_OF_YEAR).CheckValidIntValue(fieldValues.Remove(MONTH_OF_YEAR), MONTH_OF_YEAR);
			 int dom = Range(DAY_OF_MONTH).CheckValidIntValue(fieldValues.Remove(DAY_OF_MONTH), DAY_OF_MONTH);
			 if (resolverStyle == ResolverStyle.SMART) // previous valid
			 {
				 if (yoe < 1)
				 {
					 throw new DateTimeException("Invalid YearOfEra: " + yoe);
				 }
				 int y = ProlepticYearLenient(era, yoe);
				 JapaneseDate result;
				 try
				 {
					 result = Date(y, moy, dom);
				 }
				 catch (DateTimeException)
				 {
					 result = Date(y, moy, 1).With(TemporalAdjusters.LastDayOfMonth());
				 }
				 // handle the era being changed
				 // only allow if the new date is in the same Jan-Dec as the era change
				 // determine by ensuring either original yoe or result yoe is 1
				 if (result.Era != era && result.get(YEAR_OF_ERA) > 1 && yoe > 1)
				 {
					 throw new DateTimeException("Invalid YearOfEra for Era: " + era + " " + yoe);
				 }
				 return result;
			 }
			 return Date(era, yoe, moy, dom);
		 }

		private ChronoLocalDate ResolveYD(JapaneseEra era, int yoe, IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			fieldValues.Remove(ERA);
			fieldValues.Remove(YEAR_OF_ERA);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				int y = ProlepticYearLenient(era, yoe);
				long days = Math.SubtractExact(fieldValues.Remove(DAY_OF_YEAR), 1);
				return DateYearDay(y, 1).Plus(days, DAYS);
			}
			int doy = Range(DAY_OF_YEAR).CheckValidIntValue(fieldValues.Remove(DAY_OF_YEAR), DAY_OF_YEAR);
			return DateYearDay(era, yoe, doy); // smart is same as strict
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