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
	/// The Thai Buddhist calendar system.
	/// <para>
	/// This chronology defines the rules of the Thai Buddhist calendar system.
	/// This calendar system is primarily used in Thailand.
	/// Dates are aligned such that {@code 2484-01-01 (Buddhist)} is {@code 1941-01-01 (ISO)}.
	/// </para>
	/// <para>
	/// The fields are defined as follows:
	/// <ul>
	/// <li>era - There are two eras, the current 'Buddhist' (ERA_BE) and the previous era (ERA_BEFORE_BE).
	/// <li>year-of-era - The year-of-era for the current era increases uniformly from the epoch at year one.
	///  For the previous era the year increases from one as time goes backwards.
	///  The value for the current era is equal to the ISO proleptic-year plus 543.
	/// <li>proleptic-year - The proleptic year is the same as the year-of-era for the
	///  current era. For the previous era, years have zero, then negative values.
	///  The value is equal to the ISO proleptic-year plus 543.
	/// <li>month-of-year - The ThaiBuddhist month-of-year exactly matches ISO.
	/// <li>day-of-month - The ThaiBuddhist day-of-month exactly matches ISO.
	/// <li>day-of-year - The ThaiBuddhist day-of-year exactly matches ISO.
	/// <li>leap-year - The ThaiBuddhist leap-year pattern exactly matches ISO, such that the two calendars
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
	public sealed class ThaiBuddhistChronology : AbstractChronology
	{

		/// <summary>
		/// Singleton instance of the Buddhist chronology.
		/// </summary>
		public static readonly ThaiBuddhistChronology INSTANCE = new ThaiBuddhistChronology();

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 2775954514031616474L;
		/// <summary>
		/// Containing the offset to add to the ISO year.
		/// </summary>
		internal const int YEARS_DIFFERENCE = 543;
		/// <summary>
		/// Narrow names for eras.
		/// </summary>
		private static readonly Dictionary<String, String[]> ERA_NARROW_NAMES = new Dictionary<String, String[]>();
		/// <summary>
		/// Short names for eras.
		/// </summary>
		private static readonly Dictionary<String, String[]> ERA_SHORT_NAMES = new Dictionary<String, String[]>();
		/// <summary>
		/// Full names for eras.
		/// </summary>
		private static readonly Dictionary<String, String[]> ERA_FULL_NAMES = new Dictionary<String, String[]>();
		/// <summary>
		/// Fallback language for the era names.
		/// </summary>
		private const String FALLBACK_LANGUAGE = "en";
		/// <summary>
		/// Language that has the era names.
		/// </summary>
		private const String TARGET_LANGUAGE = "th";
		/// <summary>
		/// Name data.
		/// </summary>
		static ThaiBuddhistChronology()
		{
			ERA_NARROW_NAMES[FALLBACK_LANGUAGE] = new String[]{"BB", "BE"};
			ERA_NARROW_NAMES[TARGET_LANGUAGE] = new String[]{"BB", "BE"};
			ERA_SHORT_NAMES[FALLBACK_LANGUAGE] = new String[]{"B.B.", "B.E."};
			ERA_SHORT_NAMES[TARGET_LANGUAGE] = new String[]{"\u0e1e.\u0e28.", "\u0e1b\u0e35\u0e01\u0e48\u0e2d\u0e19\u0e04\u0e23\u0e34\u0e2a\u0e15\u0e4c\u0e01\u0e32\u0e25\u0e17\u0e35\u0e48"};
			ERA_FULL_NAMES[FALLBACK_LANGUAGE] = new String[]{"Before Buddhist", "Budhhist Era"};
			ERA_FULL_NAMES[TARGET_LANGUAGE] = new String[]{"\u0e1e\u0e38\u0e17\u0e18\u0e28\u0e31\u0e01\u0e23\u0e32\u0e0a", "\u0e1b\u0e35\u0e01\u0e48\u0e2d\u0e19\u0e04\u0e23\u0e34\u0e2a\u0e15\u0e4c\u0e01\u0e32\u0e25\u0e17\u0e35\u0e48"};
		}

		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private ThaiBuddhistChronology()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the ID of the chronology - 'ThaiBuddhist'.
		/// <para>
		/// The ID uniquely identifies the {@code Chronology}.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology ID - 'ThaiBuddhist' </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		public override String Id
		{
			get
			{
				return "ThaiBuddhist";
			}
		}

		/// <summary>
		/// Gets the calendar type of the underlying calendar system - 'buddhist'.
		/// <para>
		/// The calendar type is an identifier defined by the
		/// <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// It can also be used as part of a locale, accessible via
		/// <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the calendar system type - 'buddhist' </returns>
		/// <seealso cref= #getId() </seealso>
		public override String CalendarType
		{
			get
			{
				return "buddhist";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a local date in Thai Buddhist calendar system from the
		/// era, year-of-era, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="era">  the Thai Buddhist era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Thai Buddhist local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code ThaiBuddhistEra} </exception>
		public override ThaiBuddhistDate Date(Era era, int yearOfEra, int month, int dayOfMonth)
		{
			return Date(ProlepticYear(era, yearOfEra), month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a local date in Thai Buddhist calendar system from the
		/// proleptic-year, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Thai Buddhist local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override ThaiBuddhistDate Date(int prolepticYear, int month, int dayOfMonth)
		{
			return new ThaiBuddhistDate(LocalDate.Of(prolepticYear - YEARS_DIFFERENCE, month, dayOfMonth));
		}

		/// <summary>
		/// Obtains a local date in Thai Buddhist calendar system from the
		/// era, year-of-era and day-of-year fields.
		/// </summary>
		/// <param name="era">  the Thai Buddhist era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Thai Buddhist local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code ThaiBuddhistEra} </exception>
		public override ThaiBuddhistDate DateYearDay(Era era, int yearOfEra, int dayOfYear)
		{
			return DateYearDay(ProlepticYear(era, yearOfEra), dayOfYear);
		}

		/// <summary>
		/// Obtains a local date in Thai Buddhist calendar system from the
		/// proleptic-year and day-of-year fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Thai Buddhist local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override ThaiBuddhistDate DateYearDay(int prolepticYear, int dayOfYear)
		{
			return new ThaiBuddhistDate(LocalDate.OfYearDay(prolepticYear - YEARS_DIFFERENCE, dayOfYear));
		}

		/// <summary>
		/// Obtains a local date in the Thai Buddhist calendar system from the epoch-day.
		/// </summary>
		/// <param name="epochDay">  the epoch day </param>
		/// <returns> the Thai Buddhist local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override ThaiBuddhistDate DateEpochDay(long epochDay) // override with covariant return type
		{
			return new ThaiBuddhistDate(LocalDate.OfEpochDay(epochDay));
		}

		public override ThaiBuddhistDate DateNow()
		{
			return DateNow(Clock.SystemDefaultZone());
		}

		public override ThaiBuddhistDate DateNow(ZoneId Chronology_Fields)
		{
			return DateNow(Clock.System(Chronology_Fields.Zone));
		}

		public override ThaiBuddhistDate DateNow(Clock clock)
		{
			return Date(LocalDate.Now(clock));
		}

		public override ThaiBuddhistDate Date(TemporalAccessor temporal)
		{
			if (temporal is ThaiBuddhistDate)
			{
				return (ThaiBuddhistDate) temporal;
			}
			return new ThaiBuddhistDate(LocalDate.From(temporal));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoLocalDateTime<ThaiBuddhistDate> localDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoLocalDateTime<ThaiBuddhistDate> LocalDateTime(TemporalAccessor temporal)
		{
			return (ChronoLocalDateTime<ThaiBuddhistDate>)base.LocalDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<ThaiBuddhistDate> zonedDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoZonedDateTime<ThaiBuddhistDate> ZonedDateTime(TemporalAccessor temporal)
		{
			return (ChronoZonedDateTime<ThaiBuddhistDate>)base.ZonedDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<ThaiBuddhistDate> zonedDateTime(java.time.Instant Chronology_Fields.instant, java.time.ZoneId Chronology_Fields.zone)
		public override ChronoZonedDateTime<ThaiBuddhistDate> ZonedDateTime(Instant Chronology_Fields, ZoneId Chronology_Fields)
		{
			return (ChronoZonedDateTime<ThaiBuddhistDate>)base.ZonedDateTime(Chronology_Fields.Instant, Chronology_Fields.Zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified year is a leap year.
		/// <para>
		/// Thai Buddhist leap years occur exactly in line with ISO leap years.
		/// This method does not validate the year passed in, and only has a
		/// well-defined result for years in the supported range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		/// <returns> true if the year is a leap year </returns>
		public override bool IsLeapYear(long prolepticYear)
		{
			return IsoChronology.INSTANCE.IsLeapYear(prolepticYear - YEARS_DIFFERENCE);
		}

		public override int ProlepticYear(Era era, int yearOfEra)
		{
			if (era is ThaiBuddhistEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be BuddhistEra");
			}
			return (era == ThaiBuddhistEra.BE ? yearOfEra : 1 - yearOfEra);
		}

		public override ThaiBuddhistEra EraOf(int eraValue)
		{
			return ThaiBuddhistEra.of(eraValue);
		}

		public override IList<Era> Eras()
		{
			return Arrays.AsList<Era>(ThaiBuddhistEra.values());
		}

		//-----------------------------------------------------------------------
		public override ValueRange Range(ChronoField field)
		{
			switch (field)
			{
				case PROLEPTIC_MONTH:
				{
					ValueRange range = PROLEPTIC_MONTH.range();
					return ValueRange.Of(range.Minimum + YEARS_DIFFERENCE * 12L, range.Maximum + YEARS_DIFFERENCE * 12L);
				}
				case YEAR_OF_ERA:
				{
					ValueRange range = YEAR.range();
					return ValueRange.Of(1, -(range.Minimum + YEARS_DIFFERENCE) + 1, range.Maximum + YEARS_DIFFERENCE);
				}
				case YEAR:
				{
					ValueRange range = YEAR.range();
					return ValueRange.Of(range.Minimum + YEARS_DIFFERENCE, range.Maximum + YEARS_DIFFERENCE);
				}
			}
			return field.range();
		}

		//-----------------------------------------------------------------------
		public override ThaiBuddhistDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for return type
		{
			return (ThaiBuddhistDate) base.ResolveDate(fieldValues, resolverStyle);
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