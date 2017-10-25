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


	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// The Hijrah calendar is a lunar calendar supporting Islamic calendars.
	/// <para>
	/// The HijrahChronology follows the rules of the Hijrah calendar system. The Hijrah
	/// calendar has several variants based on differences in when the new moon is
	/// determined to have occurred and where the observation is made.
	/// In some variants the length of each month is
	/// computed algorithmically from the astronomical data for the moon and earth and
	/// in others the length of the month is determined by an authorized sighting
	/// of the new moon. For the algorithmically based calendars the calendar
	/// can project into the future.
	/// For sighting based calendars only historical data from past
	/// sightings is available.
	/// </para>
	/// <para>
	/// The length of each month is 29 or 30 days.
	/// Ordinary years have 354 days; leap years have 355 days.
	/// 
	/// </para>
	/// <para>
	/// CLDR and LDML identify variants:
	/// <table cellpadding="2" summary="Variants of Hijrah Calendars">
	/// <thead>
	/// <tr class="tableSubHeadingColor">
	/// <th class="colFirst" align="left" >Chronology ID</th>
	/// <th class="colFirst" align="left" >Calendar Type</th>
	/// <th class="colFirst" align="left" >Locale extension, see <seealso cref="java.util.Locale"/></th>
	/// <th class="colLast" align="left" >Description</th>
	/// </tr>
	/// </thead>
	/// <tbody>
	/// <tr class="altColor">
	/// <td>Hijrah-umalqura</td>
	/// <td>islamic-umalqura</td>
	/// <td>ca-islamic-umalqura</td>
	/// <td>Islamic - Umm Al-Qura calendar of Saudi Arabia</td>
	/// </tr>
	/// </tbody>
	/// </table>
	/// </para>
	/// <para>Additional variants may be available through <seealso cref="Chronology#getAvailableChronologies()"/>.
	/// 
	/// </para>
	/// <para>Example</para>
	/// <para>
	/// Selecting the chronology from the locale uses <seealso cref="Chronology#ofLocale"/>
	/// to find the Chronology based on Locale supported BCP 47 extension mechanism
	/// to request a specific calendar ("ca"). For example,
	/// </para>
	/// <pre>
	///      Locale locale = Locale.forLanguageTag("en-US-u-ca-islamic-umalqura");
	///      Chronology chrono = Chronology.ofLocale(locale);
	/// </pre>
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @implNote
	/// Each Hijrah variant is configured individually. Each variant is defined by a
	/// property resource that defines the {@code ID}, the {@code calendar type},
	/// the start of the calendar, the alignment with the
	/// ISO calendar, and the length of each month for a range of years.
	/// The variants are identified in the {@code calendars.properties} file.
	/// The new properties are prefixed with {@code "calendars.hijrah."}:
	/// <table cellpadding="2" border="0" summary="Configuration of Hijrah Calendar Variants">
	/// <thead>
	/// <tr class="tableSubHeadingColor">
	/// <th class="colFirst" align="left">Property Name</th>
	/// <th class="colFirst" align="left">Property value</th>
	/// <th class="colLast" align="left">Description </th>
	/// </tr>
	/// </thead>
	/// <tbody>
	/// <tr class="altColor">
	/// <td>calendars.hijrah.{ID}</td>
	/// <td>The property resource defining the {@code {ID}} variant</td>
	/// <td>The property resource is located with the {@code calendars.properties} file</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td>calendars.hijrah.{ID}.type</td>
	/// <td>The calendar type</td>
	/// <td>LDML defines the calendar type names</td>
	/// </tr>
	/// </tbody>
	/// </table>
	/// <para>
	/// The Hijrah property resource is a set of properties that describe the calendar.
	/// The syntax is defined by {@code java.util.Properties#load(Reader)}.
	/// <table cellpadding="2" summary="Configuration of Hijrah Calendar">
	/// <thead>
	/// <tr class="tableSubHeadingColor">
	/// <th class="colFirst" align="left" > Property Name</th>
	/// <th class="colFirst" align="left" > Property value</th>
	/// <th class="colLast" align="left" > Description </th>
	/// </tr>
	/// </thead>
	/// <tbody>
	/// <tr class="altColor">
	/// <td>id</td>
	/// <td>Chronology Id, for example, "Hijrah-umalqura"</td>
	/// <td>The Id of the calendar in common usage</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td>type</td>
	/// <td>Calendar type, for example, "islamic-umalqura"</td>
	/// <td>LDML defines the calendar types</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td>version</td>
	/// <td>Version, for example: "1.8.0_1"</td>
	/// <td>The version of the Hijrah variant data</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td>iso-start</td>
	/// <td>ISO start date, formatted as {@code yyyy-MM-dd}, for example: "1900-04-30"</td>
	/// <td>The ISO date of the first day of the minimum Hijrah year.</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td>yyyy - a numeric 4 digit year, for example "1434"</td>
	/// <td>The value is a sequence of 12 month lengths,
	/// for example: "29 30 29 30 29 30 30 30 29 30 29 29"</td>
	/// <td>The lengths of the 12 months of the year separated by whitespace.
	/// A numeric year property must be present for every year without any gaps.
	/// The month lengths must be between 29-32 inclusive.
	/// </td>
	/// </tr>
	/// </tbody>
	/// </table>
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class HijrahChronology : AbstractChronology
	{

		/// <summary>
		/// The Hijrah Calendar id.
		/// </summary>
		[NonSerialized]
		private readonly String TypeId;
		/// <summary>
		/// The Hijrah calendarType.
		/// </summary>
		[NonSerialized]
		private readonly String CalendarType_Renamed;
		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 3127340209035924785L;
		/// <summary>
		/// Singleton instance of the Islamic Umm Al-Qura calendar of Saudi Arabia.
		/// Other Hijrah chronology variants may be available from
		/// <seealso cref="Chronology#getAvailableChronologies"/>.
		/// </summary>
		public static readonly HijrahChronology INSTANCE;
		/// <summary>
		/// Flag to indicate the initialization of configuration data is complete. </summary>
		/// <seealso cref= #checkCalendarInit() </seealso>
		[NonSerialized]
		private volatile bool InitComplete;
		/// <summary>
		/// Array of epoch days indexed by Hijrah Epoch month.
		/// Computed by <seealso cref="#loadCalendarData"/>.
		/// </summary>
		[NonSerialized]
		private int[] HijrahEpochMonthStartDays;
		/// <summary>
		/// The minimum epoch day of this Hijrah calendar.
		/// Computed by <seealso cref="#loadCalendarData"/>.
		/// </summary>
		[NonSerialized]
		private int MinEpochDay;
		/// <summary>
		/// The maximum epoch day for which calendar data is available.
		/// Computed by <seealso cref="#loadCalendarData"/>.
		/// </summary>
		[NonSerialized]
		private int MaxEpochDay;
		/// <summary>
		/// The minimum epoch month.
		/// Computed by <seealso cref="#loadCalendarData"/>.
		/// </summary>
		[NonSerialized]
		private int HijrahStartEpochMonth;
		/// <summary>
		/// The minimum length of a month.
		/// Computed by <seealso cref="#createEpochMonths"/>.
		/// </summary>
		[NonSerialized]
		private int MinMonthLength;
		/// <summary>
		/// The maximum length of a month.
		/// Computed by <seealso cref="#createEpochMonths"/>.
		/// </summary>
		[NonSerialized]
		private int MaxMonthLength;
		/// <summary>
		/// The minimum length of a year in days.
		/// Computed by <seealso cref="#createEpochMonths"/>.
		/// </summary>
		[NonSerialized]
		private int MinYearLength;
		/// <summary>
		/// The maximum length of a year in days.
		/// Computed by <seealso cref="#createEpochMonths"/>.
		/// </summary>
		[NonSerialized]
		private int MaxYearLength;
		/// <summary>
		/// A reference to the properties stored in
		/// ${java.home}/lib/calendars.properties
		/// </summary>
		[NonSerialized]
		private readonly static Properties CalendarProperties;

		/// <summary>
		/// Prefix of property names for Hijrah calendar variants.
		/// </summary>
		private const String PROP_PREFIX = "calendar.hijrah.";
		/// <summary>
		/// Suffix of property names containing the calendar type of a variant.
		/// </summary>
		private const String PROP_TYPE_SUFFIX = ".type";

		/// <summary>
		/// Static initialization of the predefined calendars found in the
		/// lib/calendars.properties file.
		/// </summary>
		static HijrahChronology()
		{
			try
			{
				CalendarProperties = sun.util.calendar.BaseCalendar.CalendarProperties;
			}
			catch (IOException ioe)
			{
				throw new InternalError("Can't initialize lib/calendars.properties", ioe);
			}

			try
			{
				INSTANCE = new HijrahChronology("Hijrah-umalqura");
				// Register it by its aliases
				AbstractChronology.RegisterChrono(INSTANCE, "Hijrah");
				AbstractChronology.RegisterChrono(INSTANCE, "islamic");
			}
			catch (DateTimeException ex)
			{
				// Absence of Hijrah calendar is fatal to initializing this class.
				PlatformLogger logger = PlatformLogger.getLogger("java.time.chrono");
				logger.severe("Unable to initialize Hijrah calendar: Hijrah-umalqura", ex);
				throw new RuntimeException("Unable to initialize Hijrah-umalqura calendar", ex.InnerException);
			}
			RegisterVariants();
		}

		/// <summary>
		/// For each Hijrah variant listed, create the HijrahChronology and register it.
		/// Exceptions during initialization are logged but otherwise ignored.
		/// </summary>
		private static void RegisterVariants()
		{
			foreach (String name in CalendarProperties.StringPropertyNames())
			{
				if (name.StartsWith(PROP_PREFIX))
				{
					String id = name.Substring(PROP_PREFIX.Length());
					if (id.IndexOf('.') >= 0)
					{
						continue; // no name or not a simple name of a calendar
					}
					if (id.Equals(INSTANCE.Id))
					{
						continue; // do not duplicate the default
					}
					try
					{
						// Create and register the variant
						HijrahChronology chrono = new HijrahChronology(id);
						AbstractChronology.RegisterChrono(chrono);
					}
					catch (DateTimeException ex)
					{
						// Log error and continue
						PlatformLogger logger = PlatformLogger.getLogger("java.time.chrono");
						logger.severe("Unable to initialize Hijrah calendar: " + id, ex);
					}
				}
			}
		}

		/// <summary>
		/// Create a HijrahChronology for the named variant.
		/// The resource and calendar type are retrieved from properties
		/// in the {@code calendars.properties}.
		/// The property names are {@code "calendar.hijrah." + id}
		/// and  {@code "calendar.hijrah." + id + ".type"} </summary>
		/// <param name="id"> the id of the calendar </param>
		/// <exception cref="DateTimeException"> if the calendar type is missing from the properties file. </exception>
		/// <exception cref="IllegalArgumentException"> if the id is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private HijrahChronology(String id) throws java.time.DateTimeException
		private HijrahChronology(String id)
		{
			if (id.Empty)
			{
				throw new IllegalArgumentException("calendar id is empty");
			}
			String propName = PROP_PREFIX + id + PROP_TYPE_SUFFIX;
			String calType = CalendarProperties.GetProperty(propName);
			if (calType == null || calType.Empty)
			{
				throw new DateTimeException("calendarType is missing or empty for: " + propName);
			}
			this.TypeId = id;
			this.CalendarType_Renamed = calType;
		}

		/// <summary>
		/// Check and ensure that the calendar data has been initialized.
		/// The initialization check is performed at the boundary between
		/// public and package methods.  If a public calls another public method
		/// a check is not necessary in the caller.
		/// The constructors of HijrahDate call <seealso cref="#getEpochDay"/> or
		/// <seealso cref="#getHijrahDateInfo"/> so every call from HijrahDate to a
		/// HijrahChronology via package private methods has been checked.
		/// </summary>
		/// <exception cref="DateTimeException"> if the calendar data configuration is
		///     malformed or IOExceptions occur loading the data </exception>
		private void CheckCalendarInit()
		{
			// Keep this short so it can be inlined for performance
			if (InitComplete == Chronology_Fields.False)
			{
				LoadCalendarData();
				InitComplete = true;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the ID of the chronology.
		/// <para>
		/// The ID uniquely identifies the {@code Chronology}. It can be used to
		/// lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology ID, non-null </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		public override String Id
		{
			get
			{
				return TypeId;
			}
		}

		/// <summary>
		/// Gets the calendar type of the Islamic calendar.
		/// <para>
		/// The calendar type is an identifier defined by the
		/// <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		/// It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the calendar system type; non-null if the calendar has
		///    a standard type, otherwise null </returns>
		/// <seealso cref= #getId() </seealso>
		public override String CalendarType
		{
			get
			{
				return CalendarType_Renamed;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a local date in Hijrah calendar system from the
		/// era, year-of-era, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="era">  the Hijrah era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Hijrah local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code HijrahEra} </exception>
		public override HijrahDate Date(Era era, int yearOfEra, int month, int dayOfMonth)
		{
			return Date(ProlepticYear(era, yearOfEra), month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a local date in Hijrah calendar system from the
		/// proleptic-year, month-of-year and day-of-month fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="month">  the month-of-year </param>
		/// <param name="dayOfMonth">  the day-of-month </param>
		/// <returns> the Hijrah local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override HijrahDate Date(int prolepticYear, int month, int dayOfMonth)
		{
			return HijrahDate.Of(this, prolepticYear, month, dayOfMonth);
		}

		/// <summary>
		/// Obtains a local date in Hijrah calendar system from the
		/// era, year-of-era and day-of-year fields.
		/// </summary>
		/// <param name="era">  the Hijrah era, not null </param>
		/// <param name="yearOfEra">  the year-of-era </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Hijrah local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		/// <exception cref="ClassCastException"> if the {@code era} is not a {@code HijrahEra} </exception>
		public override HijrahDate DateYearDay(Era era, int yearOfEra, int dayOfYear)
		{
			return DateYearDay(ProlepticYear(era, yearOfEra), dayOfYear);
		}

		/// <summary>
		/// Obtains a local date in Hijrah calendar system from the
		/// proleptic-year and day-of-year fields.
		/// </summary>
		/// <param name="prolepticYear">  the proleptic-year </param>
		/// <param name="dayOfYear">  the day-of-year </param>
		/// <returns> the Hijrah local date, not null </returns>
		/// <exception cref="DateTimeException"> if the value of the year is out of range,
		///  or if the day-of-year is invalid for the year </exception>
		public override HijrahDate DateYearDay(int prolepticYear, int dayOfYear)
		{
			HijrahDate date = HijrahDate.Of(this, prolepticYear, 1, 1);
			if (dayOfYear > date.LengthOfYear())
			{
				throw new DateTimeException("Invalid dayOfYear: " + dayOfYear);
			}
			return date.PlusDays(dayOfYear - 1);
		}

		/// <summary>
		/// Obtains a local date in the Hijrah calendar system from the epoch-day.
		/// </summary>
		/// <param name="epochDay">  the epoch day </param>
		/// <returns> the Hijrah local date, not null </returns>
		/// <exception cref="DateTimeException"> if unable to create the date </exception>
		public override HijrahDate DateEpochDay(long epochDay) // override with covariant return type
		{
			return HijrahDate.OfEpochDay(this, epochDay);
		}

		public override HijrahDate DateNow()
		{
			return DateNow(Clock.SystemDefaultZone());
		}

		public override HijrahDate DateNow(ZoneId Chronology_Fields)
		{
			return DateNow(Clock.System(Chronology_Fields.Zone));
		}

		public override HijrahDate DateNow(Clock clock)
		{
			return Date(LocalDate.Now(clock));
		}

		public override HijrahDate Date(TemporalAccessor temporal)
		{
			if (temporal is HijrahDate)
			{
				return (HijrahDate) temporal;
			}
			return HijrahDate.OfEpochDay(this, temporal.GetLong(EPOCH_DAY));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoLocalDateTime<HijrahDate> localDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoLocalDateTime<HijrahDate> LocalDateTime(TemporalAccessor temporal)
		{
			return (ChronoLocalDateTime<HijrahDate>) base.LocalDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<HijrahDate> zonedDateTime(java.time.temporal.TemporalAccessor temporal)
		public override ChronoZonedDateTime<HijrahDate> ZonedDateTime(TemporalAccessor temporal)
		{
			return (ChronoZonedDateTime<HijrahDate>) base.ZonedDateTime(temporal);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ChronoZonedDateTime<HijrahDate> zonedDateTime(java.time.Instant Chronology_Fields.instant, java.time.ZoneId Chronology_Fields.zone)
		public override ChronoZonedDateTime<HijrahDate> ZonedDateTime(Instant Chronology_Fields, ZoneId Chronology_Fields)
		{
			return (ChronoZonedDateTime<HijrahDate>) base.ZonedDateTime(Chronology_Fields.Instant, Chronology_Fields.Zone);
		}

		//-----------------------------------------------------------------------
		public override bool IsLeapYear(long prolepticYear)
		{
			CheckCalendarInit();
			int epochMonth = YearToEpochMonth((int) prolepticYear);
			if (epochMonth < 0 || epochMonth > MaxEpochDay)
			{
				throw new DateTimeException("Hijrah date out of range");
			}
			int len = GetYearLength((int) prolepticYear);
			return (len > 354);
		}

		public override int ProlepticYear(Era era, int yearOfEra)
		{
			if (era is HijrahEra == Chronology_Fields.False)
			{
				throw new ClassCastException("Era must be HijrahEra");
			}
			return yearOfEra;
		}

		public override HijrahEra EraOf(int eraValue)
		{
			switch (eraValue)
			{
				case 1:
					return HijrahEra.AH;
				default:
					throw new DateTimeException("invalid Hijrah era");
			}
		}

		public override IList<Era> Eras()
		{
			return Arrays.AsList<Era>(HijrahEra.values());
		}

		//-----------------------------------------------------------------------
		public override ValueRange Range(ChronoField field)
		{
			CheckCalendarInit();
			if (field is ChronoField)
			{
				ChronoField f = field;
				switch (f)
				{
					case DAY_OF_MONTH:
						return ValueRange.Of(1, 1, MinimumMonthLength, MaximumMonthLength);
					case DAY_OF_YEAR:
						return ValueRange.Of(1, MaximumDayOfYear);
					case ALIGNED_WEEK_OF_MONTH:
						return ValueRange.Of(1, 5);
					case YEAR:
					case YEAR_OF_ERA:
						return ValueRange.Of(MinimumYear, MaximumYear);
					case ERA:
						return ValueRange.Of(1, 1);
					default:
						return field.range();
				}
			}
			return field.range();
		}

		//-----------------------------------------------------------------------
		public override HijrahDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle) // override for return type
		{
			return (HijrahDate) base.ResolveDate(fieldValues, resolverStyle);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Check the validity of a year.
		/// </summary>
		/// <param name="prolepticYear"> the year to check </param>
		internal int CheckValidYear(long prolepticYear)
		{
			if (prolepticYear < MinimumYear || prolepticYear > MaximumYear)
			{
				throw new DateTimeException("Invalid Hijrah year: " + prolepticYear);
			}
			return (int) prolepticYear;
		}

		internal void CheckValidDayOfYear(int dayOfYear)
		{
			if (dayOfYear < 1 || dayOfYear > MaximumDayOfYear)
			{
				throw new DateTimeException("Invalid Hijrah day of year: " + dayOfYear);
			}
		}

		internal void CheckValidMonth(int month)
		{
			if (month < 1 || month > 12)
			{
				throw new DateTimeException("Invalid Hijrah month: " + month);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns an array containing the Hijrah year, month and day
		/// computed from the epoch day.
		/// </summary>
		/// <param name="epochDay">  the EpochDay </param>
		/// <returns> int[0] = YEAR, int[1] = MONTH, int[2] = DATE </returns>
		internal int[] GetHijrahDateInfo(int epochDay)
		{
			CheckCalendarInit(); // ensure that the chronology is initialized
			if (epochDay < MinEpochDay || epochDay >= MaxEpochDay)
			{
				throw new DateTimeException("Hijrah date out of range");
			}

			int epochMonth = EpochDayToEpochMonth(epochDay);
			int year = EpochMonthToYear(epochMonth);
			int month = EpochMonthToMonth(epochMonth);
			int day1 = EpochMonthToEpochDay(epochMonth);
			int date = epochDay - day1; // epochDay - dayOfEpoch(year, month);

			int[] dateInfo = new int[3];
			dateInfo[0] = year;
			dateInfo[1] = month + 1; // change to 1-based.
			dateInfo[2] = date + 1; // change to 1-based.
			return dateInfo;
		}

		/// <summary>
		/// Return the epoch day computed from Hijrah year, month, and day.
		/// </summary>
		/// <param name="prolepticYear"> the year to represent, 0-origin </param>
		/// <param name="monthOfYear"> the month-of-year to represent, 1-origin </param>
		/// <param name="dayOfMonth"> the day-of-month to represent, 1-origin </param>
		/// <returns> the epoch day </returns>
		internal long GetEpochDay(int prolepticYear, int monthOfYear, int dayOfMonth)
		{
			CheckCalendarInit(); // ensure that the chronology is initialized
			CheckValidMonth(monthOfYear);
			int epochMonth = YearToEpochMonth(prolepticYear) + (monthOfYear - 1);
			if (epochMonth < 0 || epochMonth >= HijrahEpochMonthStartDays.Length)
			{
				throw new DateTimeException("Invalid Hijrah date, year: " + prolepticYear + ", month: " + monthOfYear);
			}
			if (dayOfMonth < 1 || dayOfMonth > GetMonthLength(prolepticYear, monthOfYear))
			{
				throw new DateTimeException("Invalid Hijrah day of month: " + dayOfMonth);
			}
			return EpochMonthToEpochDay(epochMonth) + (dayOfMonth - 1);
		}

		/// <summary>
		/// Returns day of year for the year and month.
		/// </summary>
		/// <param name="prolepticYear"> a proleptic year </param>
		/// <param name="month"> a month, 1-origin </param>
		/// <returns> the day of year, 1-origin </returns>
		internal int GetDayOfYear(int prolepticYear, int month)
		{
			return YearMonthToDayOfYear(prolepticYear, (month - 1));
		}

		/// <summary>
		/// Returns month length for the year and month.
		/// </summary>
		/// <param name="prolepticYear"> a proleptic year </param>
		/// <param name="monthOfYear"> a month, 1-origin. </param>
		/// <returns> the length of the month </returns>
		internal int GetMonthLength(int prolepticYear, int monthOfYear)
		{
			int epochMonth = YearToEpochMonth(prolepticYear) + (monthOfYear - 1);
			if (epochMonth < 0 || epochMonth >= HijrahEpochMonthStartDays.Length)
			{
				throw new DateTimeException("Invalid Hijrah date, year: " + prolepticYear + ", month: " + monthOfYear);
			}
			return EpochMonthLength(epochMonth);
		}

		/// <summary>
		/// Returns year length.
		/// Note: The 12th month must exist in the data.
		/// </summary>
		/// <param name="prolepticYear"> a proleptic year </param>
		/// <returns> year length in days </returns>
		internal int GetYearLength(int prolepticYear)
		{
			return YearMonthToDayOfYear(prolepticYear, 12);
		}

		/// <summary>
		/// Return the minimum supported Hijrah year.
		/// </summary>
		/// <returns> the minimum </returns>
		internal int MinimumYear
		{
			get
			{
				return EpochMonthToYear(0);
			}
		}

		/// <summary>
		/// Return the maximum supported Hijrah ear.
		/// </summary>
		/// <returns> the minimum </returns>
		internal int MaximumYear
		{
			get
			{
				return EpochMonthToYear(HijrahEpochMonthStartDays.Length - 1) - 1;
			}
		}

		/// <summary>
		/// Returns maximum day-of-month.
		/// </summary>
		/// <returns> maximum day-of-month </returns>
		internal int MaximumMonthLength
		{
			get
			{
				return MaxMonthLength;
			}
		}

		/// <summary>
		/// Returns smallest maximum day-of-month.
		/// </summary>
		/// <returns> smallest maximum day-of-month </returns>
		internal int MinimumMonthLength
		{
			get
			{
				return MinMonthLength;
			}
		}

		/// <summary>
		/// Returns maximum day-of-year.
		/// </summary>
		/// <returns> maximum day-of-year </returns>
		internal int MaximumDayOfYear
		{
			get
			{
				return MaxYearLength;
			}
		}

		/// <summary>
		/// Returns smallest maximum day-of-year.
		/// </summary>
		/// <returns> smallest maximum day-of-year </returns>
		internal int SmallestMaximumDayOfYear
		{
			get
			{
				return MinYearLength;
			}
		}

		/// <summary>
		/// Returns the epochMonth found by locating the epochDay in the table. The
		/// epochMonth is the index in the table
		/// </summary>
		/// <param name="epochDay"> </param>
		/// <returns> The index of the element of the start of the month containing the
		/// epochDay. </returns>
		private int EpochDayToEpochMonth(int epochDay)
		{
			// binary search
			int ndx = Arrays.BinarySearch(HijrahEpochMonthStartDays, epochDay);
			if (ndx < 0)
			{
				ndx = -ndx - 2;
			}
			return ndx;
		}

		/// <summary>
		/// Returns the year computed from the epochMonth
		/// </summary>
		/// <param name="epochMonth"> the epochMonth </param>
		/// <returns> the Hijrah Year </returns>
		private int EpochMonthToYear(int epochMonth)
		{
			return (epochMonth + HijrahStartEpochMonth) / 12;
		}

		/// <summary>
		/// Returns the epochMonth for the Hijrah Year.
		/// </summary>
		/// <param name="year"> the HijrahYear </param>
		/// <returns> the epochMonth for the beginning of the year. </returns>
		private int YearToEpochMonth(int year)
		{
			return (year * 12) - HijrahStartEpochMonth;
		}

		/// <summary>
		/// Returns the Hijrah month from the epochMonth.
		/// </summary>
		/// <param name="epochMonth"> the epochMonth </param>
		/// <returns> the month of the Hijrah Year </returns>
		private int EpochMonthToMonth(int epochMonth)
		{
			return (epochMonth + HijrahStartEpochMonth) % 12;
		}

		/// <summary>
		/// Returns the epochDay for the start of the epochMonth.
		/// </summary>
		/// <param name="epochMonth"> the epochMonth </param>
		/// <returns> the epochDay for the start of the epochMonth. </returns>
		private int EpochMonthToEpochDay(int epochMonth)
		{
			return HijrahEpochMonthStartDays[epochMonth];

		}

		/// <summary>
		/// Returns the day of year for the requested HijrahYear and month.
		/// </summary>
		/// <param name="prolepticYear"> the Hijrah year </param>
		/// <param name="month"> the Hijrah month </param>
		/// <returns> the day of year for the start of the month of the year </returns>
		private int YearMonthToDayOfYear(int prolepticYear, int month)
		{
			int epochMonthFirst = YearToEpochMonth(prolepticYear);
			return EpochMonthToEpochDay(epochMonthFirst + month) - EpochMonthToEpochDay(epochMonthFirst);
		}

		/// <summary>
		/// Returns the length of the epochMonth. It is computed from the start of
		/// the following month minus the start of the requested month.
		/// </summary>
		/// <param name="epochMonth"> the epochMonth; assumed to be within range </param>
		/// <returns> the length in days of the epochMonth </returns>
		private int EpochMonthLength(int epochMonth)
		{
			// The very last entry in the epochMonth table is not the start of a month
			return HijrahEpochMonthStartDays[epochMonth + 1] - HijrahEpochMonthStartDays[epochMonth];
		}

		//-----------------------------------------------------------------------
		private const String KEY_ID = "id";
		private const String KEY_TYPE = "type";
		private const String KEY_VERSION = "version";
		private const String KEY_ISO_START = "iso-start";

		/// <summary>
		/// Return the configuration properties from the resource.
		/// <para>
		/// The default location of the variant configuration resource is:
		/// <pre>
		///   "$java.home/lib/" + resource-name
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="resource"> the name of the calendar property resource </param>
		/// <returns> a Properties containing the properties read from the resource. </returns>
		/// <exception cref="Exception"> if access to the property resource fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.util.Properties readConfigProperties(final String resource) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static Properties ReadConfigProperties(String resource)
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return AccessController.doPrivileged((java.security.PrivilegedExceptionAction<Properties>)() =>
				{
					String libDir = System.getProperty("java.home") + File.separator + "lib";
					File file = new File(libDir, resource);
					Properties props = new Properties();
					using (InputStream @is = new FileInputStream(file))
					{
						props.Load(@is);
					}
					return props;
				});
			}
			catch (PrivilegedActionException pax)
			{
				throw pax.Exception;
			}
		}

		/// <summary>
		/// Loads and processes the Hijrah calendar properties file for this calendarType.
		/// The starting Hijrah date and the corresponding ISO date are
		/// extracted and used to calculate the epochDate offset.
		/// The version number is identified and ignored.
		/// Everything else is the data for a year with containing the length of each
		/// of 12 months.
		/// </summary>
		/// <exception cref="DateTimeException"> if initialization of the calendar data from the
		///     resource fails </exception>
		private void LoadCalendarData()
		{
			try
			{
				String resourceName = CalendarProperties.GetProperty(PROP_PREFIX + TypeId);
				Objects.RequireNonNull(resourceName, "Resource missing for calendar: " + PROP_PREFIX + TypeId);
				Properties props = ReadConfigProperties(resourceName);

				IDictionary<Integer, int[]> years = new Dictionary<Integer, int[]>();
				int minYear = Integer.MaxValue;
				int maxYear = Integer.MinValue;
				String id = null;
				String type = null;
				String version = null;
				int isoStart = 0;
				foreach (java.util.Map_Entry<Object, Object> entry in props)
				{
					String key = (String) entry.Key;
					switch (key)
					{
						case KEY_ID:
							id = (String)entry.Value;
							break;
						case KEY_TYPE:
							type = (String)entry.Value;
							break;
						case KEY_VERSION:
							version = (String)entry.Value;
							break;
						case KEY_ISO_START:
						{
							int[] ymd = ParseYMD((String) entry.Value);
							isoStart = (int) LocalDate.Of(ymd[0], ymd[1], ymd[2]).ToEpochDay();
							break;
						}
						default:
							try
							{
								// Everything else is either a year or invalid
								int year = Convert.ToInt32(key);
								int[] months = ParseMonths((String) entry.Value);
								years[year] = months;
								maxYear = System.Math.Max(maxYear, year);
								minYear = System.Math.Min(minYear, year);
							}
							catch (NumberFormatException)
							{
								throw new IllegalArgumentException("bad key: " + key);
							}
					}
				}

				if (!Id.Equals(id))
				{
					throw new IllegalArgumentException("Configuration is for a different calendar: " + id);
				}
				if (!CalendarType.Equals(type))
				{
					throw new IllegalArgumentException("Configuration is for a different calendar type: " + type);
				}
				if (version == null || version.Empty)
				{
					throw new IllegalArgumentException("Configuration does not contain a version");
				}
				if (isoStart == 0)
				{
					throw new IllegalArgumentException("Configuration does not contain a ISO start date");
				}

				// Now create and validate the array of epochDays indexed by epochMonth
				HijrahStartEpochMonth = minYear * 12;
				MinEpochDay = isoStart;
				HijrahEpochMonthStartDays = CreateEpochMonths(MinEpochDay, minYear, maxYear, years);
				MaxEpochDay = HijrahEpochMonthStartDays[HijrahEpochMonthStartDays.Length - 1];

				// Compute the min and max year length in days.
				for (int year = minYear; year < maxYear; year++)
				{
					int length = GetYearLength(year);
					MinYearLength = System.Math.Min(MinYearLength, length);
					MaxYearLength = System.Math.Max(MaxYearLength, length);
				}
			}
			catch (Exception ex)
			{
				// Log error and throw a DateTimeException
				PlatformLogger logger = PlatformLogger.getLogger("java.time.chrono");
				logger.severe("Unable to initialize Hijrah calendar proxy: " + TypeId, ex);
				throw new DateTimeException("Unable to initialize HijrahCalendar: " + TypeId, ex);
			}
		}

		/// <summary>
		/// Converts the map of year to month lengths ranging from minYear to maxYear
		/// into a linear contiguous array of epochDays. The index is the hijrahMonth
		/// computed from year and month and offset by minYear. The value of each
		/// entry is the epochDay corresponding to the first day of the month.
		/// </summary>
		/// <param name="minYear"> The minimum year for which data is provided </param>
		/// <param name="maxYear"> The maximum year for which data is provided </param>
		/// <param name="years"> a Map of year to the array of 12 month lengths </param>
		/// <returns> array of epochDays for each month from min to max </returns>
		private int[] CreateEpochMonths(int epochDay, int minYear, int maxYear, IDictionary<Integer, int[]> years)
		{
			// Compute the size for the array of dates
			int numMonths = (maxYear - minYear + 1) * 12 + 1;

			// Initialize the running epochDay as the corresponding ISO Epoch day
			int epochMonth = 0; // index into array of epochMonths
			int[] epochMonths = new int[numMonths];
			MinMonthLength = Integer.MaxValue;
			MaxMonthLength = Integer.MinValue;

			// Only whole years are valid, any zero's in the array are illegal
			for (int year = minYear; year <= maxYear; year++)
			{
				int[] months = years[year]; // must not be gaps
				for (int month = 0; month < 12; month++)
				{
					int length = months[month];
					epochMonths[epochMonth++] = epochDay;

					if (length < 29 || length > 32)
					{
						throw new IllegalArgumentException("Invalid month length in year: " + minYear);
					}
					epochDay += length;
					MinMonthLength = System.Math.Min(MinMonthLength, length);
					MaxMonthLength = System.Math.Max(MaxMonthLength, length);
				}
			}

			// Insert the final epochDay
			epochMonths[epochMonth++] = epochDay;

			if (epochMonth != epochMonths.Length)
			{
				throw new IllegalStateException("Did not fill epochMonths exactly: ndx = " + epochMonth + " should be " + epochMonths.Length);
			}

			return epochMonths;
		}

		/// <summary>
		/// Parses the 12 months lengths from a property value for a specific year.
		/// </summary>
		/// <param name="line"> the value of a year property </param>
		/// <returns> an array of int[12] containing the 12 month lengths </returns>
		/// <exception cref="IllegalArgumentException"> if the number of months is not 12 </exception>
		/// <exception cref="NumberFormatException"> if the 12 tokens are not numbers </exception>
		private int[] ParseMonths(String line)
		{
			int[] months = new int[12];
			String[] numbers = line.Split("\\s");
			if (numbers.Length != 12)
			{
				throw new IllegalArgumentException("wrong number of months on line: " + Arrays.ToString(numbers) + "; count: " + numbers.Length);
			}
			for (int i = 0; i < 12; i++)
			{
				try
				{
					months[i] = Convert.ToInt32(numbers[i]);
				}
				catch (NumberFormatException)
				{
					throw new IllegalArgumentException("bad key: " + numbers[i]);
				}
			}
			return months;
		}

		/// <summary>
		/// Parse yyyy-MM-dd into a 3 element array [yyyy, mm, dd].
		/// </summary>
		/// <param name="string"> the input string </param>
		/// <returns> the 3 element array with year, month, day </returns>
		private int[] ParseYMD(String @string)
		{
			// yyyy-MM-dd
			@string = @string.Trim();
			try
			{
				if (@string.CharAt(4) != '-' || @string.CharAt(7) != '-')
				{
					throw new IllegalArgumentException("date must be yyyy-MM-dd");
				}
				int[] ymd = new int[3];
				ymd[0] = Convert.ToInt32(@string.Substring(0, 4));
				ymd[1] = Convert.ToInt32(@string.Substring(5, 2));
				ymd[2] = Convert.ToInt32(@string.Substring(8, 2));
				return ymd;
			}
			catch (NumberFormatException ex)
			{
				throw new IllegalArgumentException("date must be yyyy-MM-dd", ex);
			}
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