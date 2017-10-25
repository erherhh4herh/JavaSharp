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



	/// <summary>
	/// The Minguo calendar system.
	/// <para>
	/// This chronology defines the rules of the Minguo calendar system.
	/// This calendar system is primarily used in the Republic of China, often known as Taiwan.
	/// Dates are aligned such that {@code 0001-01-01 (Minguo)} is {@code 1912-01-01 (ISO)}.
	/// </para>
	/// <para>
	/// The fields are defined as follows:
	/// <ul>
	/// <li>era - There are two eras, the current 'Republic' (ERA_ROC) and the previous era (ERA_BEFORE_ROC).
	/// <li>year-of-era - The year-of-era for the current era increases uniformly from the epoch at year one.
	///  For the previous era the year increases from one as time goes backwards.
	///  The value for the current era is equal to the ISO proleptic-year minus 1911.
	/// <li>proleptic-year - The proleptic year is the same as the year-of-era for the
	///  current era. For the previous era, years have zero, then negative values.
	///  The value is equal to the ISO proleptic-year minus 1911.
	/// <li>month-of-year - The Minguo month-of-year exactly matches ISO.
	/// <li>day-of-month - The Minguo day-of-month exactly matches ISO.
	/// <li>day-of-year - The Minguo day-of-year exactly matches ISO.
	/// <li>leap-year - The Minguo leap-year pattern exactly matches ISO, such that the two calendars
	///  are never out of step.
	/// </ul>
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class MinguoChronology : AbstractChronology
	{

		/// <summary>
		/// Singleton instance for the Minguo chronology.
		/// </summary>
		public static readonly MinguoChronology INSTANCE = new MinguoChronology();

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 1039765215346859963L;
		/// <summary>
		/// The difference in years between ISO and Minguo.
		/// </summary>
		internal const int YEARS_DIFFERENCE = 1911;

		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private MinguoChronology()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the ID of the chronology - 'Minguo'.
		/// <para>
		/// The ID uniquely identifies the {@code Chronology}.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology ID - 'Minguo' </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		public override String Id
		{
			get
			{
				return "Minguo";
			}
		}

		/// <summary>
		/// Gets the calendar type of the underlying calendar system - 'roc'.
		/// <para>
		/// The calendar type is an identifier defined by the
		/// <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// It can also be used as part of a locale, accessible via
		/// <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the calendar system type - 'roc' </returns>
		/// <seealso cref= #getId() </seealso>
		public override String CalendarType
		{
			get
			{
				return "roc";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a local date in Minguo calendar system from the
		/// era, year-of-era, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="era">  the Minguo era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Minguo local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code MinguoEra} </exception>
		public override MinguoDate Date(Era era, int yearOfEra, int month, int dayOfMonth)
		{
			return Date(ProlepticYear(era, yearOfEra), month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a local date in Minguo calendar system from the
		/// proleptic-year, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Minguo local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override MinguoDate Date(int prolepticYear, int month, int dayOfMonth)
		{
			return new MinguoDate(LocalDate.Of(prolepticYear + YEARS_DIFFERENCE, month, dayOfMonth));
		}

		/// <summary>
		/// Obtains a local date in Minguo calendar system from the
		/// era, year-of-era and day-of-year fields.
		/// </summary>
		/// <param name="era">  the Minguo era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Minguo local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code MinguoEra} </exception>
		public override MinguoDate DateYearDay(Era era, int yearOfEra, int dayOfYear)
		{
			return DateYearDay(ProlepticYear(era, yearOfEra), dayOfYear);
		}

		/// <summary>
		/// Obtains a local date in Minguo calendar system from the
		/// proleptic-year and day-of-year fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Minguo local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override MinguoDate DateYearDay(int prolepticYear, int dayOfYear)
		{
			return new MinguoDate(LocalDate.OfYearDay(prolepticYear + YEARS_DIFFERENCE, dayOfYear));
		}

		/// <summary>
		/// Obtains a local date in the Minguo calendar system from the epoch-day.
		/// </summary>
		/// <param name="epochDay">  the epoch day </param>
		/// <returns> the Minguo local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override MinguoDate DateEpochDay(long epochDay) // override with covariant return type
		{
			return new MinguoDate(LocalDate.OfEpochDay(epochDay));
		}

		public override MinguoDate DateNow()
		{
			return DateNow(Clock.SystemDefaultZone());
		}

		public override MinguoDate DateNow(ZoneId Chronology_Fields)
		{
			return DateNow(Clock.System(Chronology_Fields.Zone));
		}

		public override MinguoDate DateNow(Clock clock)
		{
			return Date(LocalDate.Now(clock));
		}

		public override MinguoDate Date(TemporalAccessor temporal)
		{
			if (temporal is MinguoDate)
			{
				return (MinguoDate) temporal;
			}
			return new MinguoDate(LocalDate.From(temporal));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoLocalDateTime<MinguoDate> localDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoLocalDateTime<MinguoDate> LocalDateTime(TemporalAccessor temporal)
		{
			return (ChronoLocalDateTime<MinguoDate>)base.LocalDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<MinguoDate> zonedDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoZonedDateTime<MinguoDate> ZonedDateTime(TemporalAccessor temporal)
		{
			return (ChronoZonedDateTime<MinguoDate>)base.ZonedDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<MinguoDate> zonedDateTime(java.time.Instant Chronology_Fields.instant, java.time.ZoneId Chronology_Fields.zone)
		public override ChronoZonedDateTime<MinguoDate> ZonedDateTime(Instant Chronology_Fields, ZoneId Chronology_Fields)
		{
			return (ChronoZonedDateTime<MinguoDate>)base.ZonedDateTime(Chronology_Fields.Instant, Chronology_Fields.Zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified year is a leap year.
		/// <para>
		/// Minguo leap years occur exactly in line with ISO leap years.
		/// This method does not validate the year passed in, and only has a
		/// well-defined result for years in the supported range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		/// <returns> true if the year is a leap year </returns>
		public override bool IsLeapYear(long prolepticYear)
		{
			return IsoChronology.INSTANCE.IsLeapYear(prolepticYear + YEARS_DIFFERENCE);
		}

		public override int ProlepticYear(Era era, int yearOfEra)
		{
			if (era is MinguoEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be MinguoEra");
			}
			return (era == MinguoEra.ROC ? yearOfEra : 1 - yearOfEra);
		}

		public override MinguoEra EraOf(int eraValue)
		{
			return MinguoEra.of(eraValue);
		}

		public override IList<Era> Eras()
		{
			return Arrays.AsList<Era>(MinguoEra.values());
		}

		//-----------------------------------------------------------------------
		public override ValueRange Range(ChronoField field)
		{
			switch (field)
			{
				case PROLEPTIC_MONTH:
				{
					ValueRange range = PROLEPTIC_MONTH.range();
					return ValueRange.Of(range.Minimum - YEARS_DIFFERENCE * 12L, range.Maximum - YEARS_DIFFERENCE * 12L);
				}
				case YEAR_OF_ERA:
				{
					ValueRange range = YEAR.range();
					return ValueRange.Of(1, range.Maximum - YEARS_DIFFERENCE, -range.Minimum + 1 + YEARS_DIFFERENCE);
				}
				case YEAR:
				{
					ValueRange range = YEAR.range();
					return ValueRange.Of(range.Minimum - YEARS_DIFFERENCE, range.Maximum - YEARS_DIFFERENCE);
				}
			}
			return field.range();
		}

		//-----------------------------------------------------------------------
		public override MinguoDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for return type
		{
			return (MinguoDate) base.ResolveDate(fieldValues, resolverStyle);
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