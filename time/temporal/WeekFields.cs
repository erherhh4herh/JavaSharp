using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
 *
 *
 *
 *
 *
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.temporal
{


	using CalendarDataUtility = sun.util.locale.provider.CalendarDataUtility;
	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleResources = sun.util.locale.provider.LocaleResources;

	/// <summary>
	/// Localized definitions of the day-of-week, week-of-month and week-of-year fields.
	/// <para>
	/// A standard week is seven days long, but cultures have different definitions for some
	/// other aspects of a week. This class represents the definition of the week, for the
	/// purpose of providing <seealso cref="TemporalField"/> instances.
	/// </para>
	/// <para>
	/// WeekFields provides five fields,
	/// <seealso cref="#dayOfWeek()"/>, <seealso cref="#weekOfMonth()"/>, <seealso cref="#weekOfYear()"/>,
	/// <seealso cref="#weekOfWeekBasedYear()"/>, and <seealso cref="#weekBasedYear()"/>
	/// that provide access to the values from any <seealso cref="Temporal temporal object"/>.
	/// </para>
	/// <para>
	/// The computations for day-of-week, week-of-month, and week-of-year are based
	/// on the  <seealso cref="ChronoField#YEAR proleptic-year"/>,
	/// <seealso cref="ChronoField#MONTH_OF_YEAR month-of-year"/>,
	/// <seealso cref="ChronoField#DAY_OF_MONTH day-of-month"/>, and
	/// <seealso cref="ChronoField#DAY_OF_WEEK ISO day-of-week"/> which are based on the
	/// <seealso cref="ChronoField#EPOCH_DAY epoch-day"/> and the chronology.
	/// The values may not be aligned with the <seealso cref="ChronoField#YEAR_OF_ERA year-of-Era"/>
	/// depending on the Chronology.
	/// </para>
	/// <para>A week is defined by:
	/// <ul>
	/// <li>The first day-of-week.
	/// For example, the ISO-8601 standard considers Monday to be the first day-of-week.
	/// <li>The minimal number of days in the first week.
	/// For example, the ISO-8601 standard counts the first week as needing at least 4 days.
	/// </ul>
	/// Together these two values allow a year or month to be divided into weeks.
	/// 
	/// <h3>Week of Month</h3>
	/// One field is used: week-of-month.
	/// The calculation ensures that weeks never overlap a month boundary.
	/// The month is divided into periods where each period starts on the defined first day-of-week.
	/// The earliest period is referred to as week 0 if it has less than the minimal number of days
	/// and week 1 if it has at least the minimal number of days.
	/// 
	/// <table cellpadding="0" cellspacing="3" border="0" style="text-align: left; width: 50%;">
	/// <caption>Examples of WeekFields</caption>
	/// <tr><th>Date</th><td>Day-of-week</td>
	///  <td>First day: Monday<br>Minimal days: 4</td><td>First day: Monday<br>Minimal days: 5</td></tr>
	/// <tr><th>2008-12-31</th><td>Wednesday</td>
	///  <td>Week 5 of December 2008</td><td>Week 5 of December 2008</td></tr>
	/// <tr><th>2009-01-01</th><td>Thursday</td>
	///  <td>Week 1 of January 2009</td><td>Week 0 of January 2009</td></tr>
	/// <tr><th>2009-01-04</th><td>Sunday</td>
	///  <td>Week 1 of January 2009</td><td>Week 0 of January 2009</td></tr>
	/// <tr><th>2009-01-05</th><td>Monday</td>
	///  <td>Week 2 of January 2009</td><td>Week 1 of January 2009</td></tr>
	/// </table>
	/// 
	/// <h3>Week of Year</h3>
	/// One field is used: week-of-year.
	/// The calculation ensures that weeks never overlap a year boundary.
	/// The year is divided into periods where each period starts on the defined first day-of-week.
	/// The earliest period is referred to as week 0 if it has less than the minimal number of days
	/// and week 1 if it has at least the minimal number of days.
	/// 
	/// <h3>Week Based Year</h3>
	/// Two fields are used for week-based-year, one for the
	/// <seealso cref="#weekOfWeekBasedYear() week-of-week-based-year"/> and one for
	/// <seealso cref="#weekBasedYear() week-based-year"/>.  In a week-based-year, each week
	/// belongs to only a single year.  Week 1 of a year is the first week that
	/// starts on the first day-of-week and has at least the minimum number of days.
	/// The first and last weeks of a year may contain days from the
	/// previous calendar year or next calendar year respectively.
	/// 
	/// <table cellpadding="0" cellspacing="3" border="0" style="text-align: left; width: 50%;">
	/// <caption>Examples of WeekFields for week-based-year</caption>
	/// <tr><th>Date</th><td>Day-of-week</td>
	///  <td>First day: Monday<br>Minimal days: 4</td><td>First day: Monday<br>Minimal days: 5</td></tr>
	/// <tr><th>2008-12-31</th><td>Wednesday</td>
	///  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	/// <tr><th>2009-01-01</th><td>Thursday</td>
	///  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	/// <tr><th>2009-01-04</th><td>Sunday</td>
	///  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	/// <tr><th>2009-01-05</th><td>Monday</td>
	///  <td>Week 2 of 2009</td><td>Week 1 of 2009</td></tr>
	/// </table>
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class WeekFields
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			DayOfWeek_Renamed = ComputedDayOfField.OfDayOfWeekField(this);
			WeekOfMonth_Renamed = ComputedDayOfField.OfWeekOfMonthField(this);
			WeekOfYear_Renamed = ComputedDayOfField.OfWeekOfYearField(this);
			WeekOfWeekBasedYear_Renamed = ComputedDayOfField.OfWeekOfWeekBasedYearField(this);
			WeekBasedYear_Renamed = ComputedDayOfField.OfWeekBasedYearField(this);
		}

		// implementation notes
		// querying week-of-month or week-of-year should return the week value bound within the month/year
		// however, setting the week value should be lenient (use plus/minus weeks)
		// allow week-of-month outer range [0 to 6]
		// allow week-of-year outer range [0 to 54]
		// this is because callers shouldn't be expected to know the details of validity

		/// <summary>
		/// The cache of rules by firstDayOfWeek plus minimalDays.
		/// Initialized first to be available for definition of ISO, etc.
		/// </summary>
		private static readonly ConcurrentMap<String, WeekFields> CACHE = new ConcurrentDictionary<String, WeekFields>(4, 0.75f, 2);

		/// <summary>
		/// The ISO-8601 definition, where a week starts on Monday and the first week
		/// has a minimum of 4 days.
		/// <para>
		/// The ISO-8601 standard defines a calendar system based on weeks.
		/// It uses the week-based-year and week-of-week-based-year concepts to split
		/// up the passage of days instead of the standard year/month/day.
		/// </para>
		/// <para>
		/// Note that the first week may start in the previous calendar year.
		/// Note also that the first few days of a calendar year may be in the
		/// week-based-year corresponding to the previous calendar year.
		/// </para>
		/// </summary>
		public static readonly WeekFields ISO = new WeekFields(DayOfWeek.MONDAY, 4);

		/// <summary>
		/// The common definition of a week that starts on Sunday and the first week
		/// has a minimum of 1 day.
		/// <para>
		/// Defined as starting on Sunday and with a minimum of 1 day in the month.
		/// This week definition is in use in the US and other European countries.
		/// </para>
		/// </summary>
		public static readonly WeekFields SUNDAY_START = WeekFields.Of(DayOfWeek.SUNDAY, 1);

		/// <summary>
		/// The unit that represents week-based-years for the purpose of addition and subtraction.
		/// <para>
		/// This allows a number of week-based-years to be added to, or subtracted from, a date.
		/// The unit is equal to either 52 or 53 weeks.
		/// The estimated duration of a week-based-year is the same as that of a standard ISO
		/// year at {@code 365.2425 Days}.
		/// </para>
		/// <para>
		/// The rules for addition add the number of week-based-years to the existing value
		/// for the week-based-year field retaining the week-of-week-based-year
		/// and day-of-week, unless the week number it too large for the target year.
		/// In that case, the week is set to the last week of the year
		/// with the same day-of-week.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalUnit WEEK_BASED_YEARS = IsoFields.WEEK_BASED_YEARS;

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -1177360819670808121L;

		/// <summary>
		/// The first day-of-week.
		/// </summary>
		private readonly DayOfWeek FirstDayOfWeek_Renamed;
		/// <summary>
		/// The minimal number of days in the first week.
		/// </summary>
		private readonly int MinimalDays;
		/// <summary>
		/// The field used to access the computed DayOfWeek.
		/// </summary>
		[NonSerialized]
		private TemporalField DayOfWeek_Renamed;
		/// <summary>
		/// The field used to access the computed WeekOfMonth.
		/// </summary>
		[NonSerialized]
		private TemporalField WeekOfMonth_Renamed;
		/// <summary>
		/// The field used to access the computed WeekOfYear.
		/// </summary>
		[NonSerialized]
		private TemporalField WeekOfYear_Renamed;
		/// <summary>
		/// The field that represents the week-of-week-based-year.
		/// <para>
		/// This field allows the week of the week-based-year value to be queried and set.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		[NonSerialized]
		private TemporalField WeekOfWeekBasedYear_Renamed;
		/// <summary>
		/// The field that represents the week-based-year.
		/// <para>
		/// This field allows the week-based-year value to be queried and set.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		[NonSerialized]
		private TemporalField WeekBasedYear_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code WeekFields} appropriate for a locale.
		/// <para>
		/// This will look up appropriate values from the provider of localization data.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale">  the locale to use, not null </param>
		/// <returns> the week-definition, not null </returns>
		public static WeekFields Of(Locale locale)
		{
			Objects.RequireNonNull(locale, "locale");
			locale = new Locale(locale.Language, locale.Country); // elminate variants

			int calDow = CalendarDataUtility.retrieveFirstDayOfWeek(locale);
			DayOfWeek dow = DayOfWeek.SUNDAY.plus(calDow - 1);
			int minDays = CalendarDataUtility.retrieveMinimalDaysInFirstWeek(locale);
			return WeekFields.Of(dow, minDays);
		}

		/// <summary>
		/// Obtains an instance of {@code WeekFields} from the first day-of-week and minimal days.
		/// <para>
		/// The first day-of-week defines the ISO {@code DayOfWeek} that is day 1 of the week.
		/// The minimal number of days in the first week defines how many days must be present
		/// in a month or year, starting from the first day-of-week, before the week is counted
		/// as the first week. A value of 1 will count the first day of the month or year as part
		/// of the first week, whereas a value of 7 will require the whole seven days to be in
		/// the new month or year.
		/// </para>
		/// <para>
		/// WeekFields instances are singletons; for each unique combination
		/// of {@code firstDayOfWeek} and {@code minimalDaysInFirstWeek} the
		/// the same instance will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="firstDayOfWeek">  the first day of the week, not null </param>
		/// <param name="minimalDaysInFirstWeek">  the minimal number of days in the first week, from 1 to 7 </param>
		/// <returns> the week-definition, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the minimal days value is less than one
		///      or greater than 7 </exception>
		public static WeekFields Of(DayOfWeek firstDayOfWeek, int minimalDaysInFirstWeek)
		{
			String key = firstDayOfWeek.ToString() + minimalDaysInFirstWeek;
			WeekFields rules = CACHE[key];
			if (rules == null)
			{
				rules = new WeekFields(firstDayOfWeek, minimalDaysInFirstWeek);
				CACHE.PutIfAbsent(key, rules);
				rules = CACHE[key];
			}
			return rules;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance of the definition.
		/// </summary>
		/// <param name="firstDayOfWeek">  the first day of the week, not null </param>
		/// <param name="minimalDaysInFirstWeek">  the minimal number of days in the first week, from 1 to 7 </param>
		/// <exception cref="IllegalArgumentException"> if the minimal days value is invalid </exception>
		private WeekFields(DayOfWeek firstDayOfWeek, int minimalDaysInFirstWeek)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			Objects.RequireNonNull(firstDayOfWeek, "firstDayOfWeek");
			if (minimalDaysInFirstWeek < 1 || minimalDaysInFirstWeek > 7)
			{
				throw new IllegalArgumentException("Minimal number of days is invalid");
			}
			this.FirstDayOfWeek_Renamed = firstDayOfWeek;
			this.MinimalDays = minimalDaysInFirstWeek;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Restore the state of a WeekFields from the stream.
		/// Check that the values are valid.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> if the serialized object has an invalid
		///     value for firstDayOfWeek or minimalDays. </exception>
		/// <exception cref="ClassNotFoundException"> if a class cannot be resolved </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException, java.io.InvalidObjectException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();
			if (FirstDayOfWeek_Renamed == null)
			{
				throw new InvalidObjectException("firstDayOfWeek is null");
			}

			if (MinimalDays < 1 || MinimalDays > 7)
			{
				throw new InvalidObjectException("Minimal number of days is invalid");
			}
		}

		/// <summary>
		/// Return the singleton WeekFields associated with the
		/// {@code firstDayOfWeek} and {@code minimalDays}. </summary>
		/// <returns> the singleton WeekFields for the firstDayOfWeek and minimalDays. </returns>
		/// <exception cref="InvalidObjectException"> if the serialized object has invalid
		///     values for firstDayOfWeek or minimalDays. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws java.io.InvalidObjectException
		private Object ReadResolve()
		{
			try
			{
				return WeekFields.Of(FirstDayOfWeek_Renamed, MinimalDays);
			}
			catch (IllegalArgumentException iae)
			{
				throw new InvalidObjectException("Invalid serialized WeekFields: " + iae.Message);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the first day-of-week.
		/// <para>
		/// The first day-of-week varies by culture.
		/// For example, the US uses Sunday, while France and the ISO-8601 standard use Monday.
		/// This method returns the first day using the standard {@code DayOfWeek} enum.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first day-of-week, not null </returns>
		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				return FirstDayOfWeek_Renamed;
			}
		}

		/// <summary>
		/// Gets the minimal number of days in the first week.
		/// <para>
		/// The number of days considered to define the first week of a month or year
		/// varies by culture.
		/// For example, the ISO-8601 requires 4 days (more than half a week) to
		/// be present before counting the first week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the minimal number of days in the first week of a month or year, from 1 to 7 </returns>
		public int MinimalDaysInFirstWeek
		{
			get
			{
				return MinimalDays;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a field to access the day of week based on this {@code WeekFields}.
		/// <para>
		/// This is similar to <seealso cref="ChronoField#DAY_OF_WEEK"/> but uses values for
		/// the day-of-week based on this {@code WeekFields}.
		/// The days are numbered from 1 to 7 where the
		/// <seealso cref="#getFirstDayOfWeek() first day-of-week"/> is assigned the value 1.
		/// </para>
		/// <para>
		/// For example, if the first day-of-week is Sunday, then that will have the
		/// value 1, with other days ranging from Monday as 2 to Saturday as 7.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a localized day-of-week will be converted
		/// to a standardized {@code ChronoField} day-of-week.
		/// The day-of-week must be in the valid range 1 to 7.
		/// Other fields in this class build dates using the standardized day-of-week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a field providing access to the day-of-week with localized numbering, not null </returns>
		public TemporalField DayOfWeek()
		{
			return DayOfWeek_Renamed;
		}

		/// <summary>
		/// Returns a field to access the week of month based on this {@code WeekFields}.
		/// <para>
		/// This represents the concept of the count of weeks within the month where weeks
		/// start on a fixed day-of-week, such as Monday.
		/// This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/>.
		/// </para>
		/// <para>
		/// Week one (1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		/// where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the month.
		/// Thus, week one may start up to {@code minDays} days before the start of the month.
		/// If the first week starts after the start of the month then the period before is week zero (0).
		/// </para>
		/// <para>
		/// For example:<br>
		/// - if the 1st day of the month is a Monday, week one starts on the 1st and there is no week zero<br>
		/// - if the 2nd day of the month is a Monday, week one starts on the 2nd and the 1st is in week zero<br>
		/// - if the 4th day of the month is a Monday, week one starts on the 4th and the 1st to 3rd is in week zero<br>
		/// - if the 5th day of the month is a Monday, week two starts on the 5th and the 1st to 4th is in week one<br>
		/// </para>
		/// <para>
		/// This field can be used with any calendar system.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a year,
		/// week-of-month, month-of-year and day-of-week.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all four fields are
		/// validated against their range of valid values. The week-of-month field
		/// is validated to ensure that the resulting month is the month requested.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all four fields are
		/// validated against their range of valid values. The week-of-month field
		/// is validated from 0 to 6, meaning that the resulting date can be in a
		/// different month to that specified.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		/// are validated against the range of valid values. The resulting date is calculated
		/// equivalent to the following four stage approach.
		/// First, create a date on the first day of the first week of January in the requested year.
		/// Then take the month-of-year, subtract one, and add the amount in months to the date.
		/// Then take the week-of-month, subtract one, and add the amount in weeks to the date.
		/// Finally, adjust to the correct day-of-week within the localized week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a field providing access to the week-of-month, not null </returns>
		public TemporalField WeekOfMonth()
		{
			return WeekOfMonth_Renamed;
		}

		/// <summary>
		/// Returns a field to access the week of year based on this {@code WeekFields}.
		/// <para>
		/// This represents the concept of the count of weeks within the year where weeks
		/// start on a fixed day-of-week, such as Monday.
		/// This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/>.
		/// </para>
		/// <para>
		/// Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		/// where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		/// Thus, week one may start up to {@code minDays} days before the start of the year.
		/// If the first week starts after the start of the year then the period before is week zero (0).
		/// </para>
		/// <para>
		/// For example:<br>
		/// - if the 1st day of the year is a Monday, week one starts on the 1st and there is no week zero<br>
		/// - if the 2nd day of the year is a Monday, week one starts on the 2nd and the 1st is in week zero<br>
		/// - if the 4th day of the year is a Monday, week one starts on the 4th and the 1st to 3rd is in week zero<br>
		/// - if the 5th day of the year is a Monday, week two starts on the 5th and the 1st to 4th is in week one<br>
		/// </para>
		/// <para>
		/// This field can be used with any calendar system.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a year,
		/// week-of-year and day-of-week.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-year field
		/// is validated to ensure that the resulting year is the year requested.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-year field
		/// is validated from 0 to 54, meaning that the resulting date can be in a
		/// different year to that specified.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		/// are validated against the range of valid values. The resulting date is calculated
		/// equivalent to the following three stage approach.
		/// First, create a date on the first day of the first week in the requested year.
		/// Then take the week-of-year, subtract one, and add the amount in weeks to the date.
		/// Finally, adjust to the correct day-of-week within the localized week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a field providing access to the week-of-year, not null </returns>
		public TemporalField WeekOfYear()
		{
			return WeekOfYear_Renamed;
		}

		/// <summary>
		/// Returns a field to access the week of a week-based-year based on this {@code WeekFields}.
		/// <para>
		/// This represents the concept of the count of weeks within the year where weeks
		/// start on a fixed day-of-week, such as Monday and each week belongs to exactly one year.
		/// This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/> and
		/// <seealso cref="WeekFields#weekBasedYear()"/>.
		/// </para>
		/// <para>
		/// Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		/// where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		/// If the first week starts after the start of the year then the period before
		/// is in the last week of the previous year.
		/// </para>
		/// <para>
		/// For example:<br>
		/// - if the 1st day of the year is a Monday, week one starts on the 1st<br>
		/// - if the 2nd day of the year is a Monday, week one starts on the 2nd and
		///   the 1st is in the last week of the previous year<br>
		/// - if the 4th day of the year is a Monday, week one starts on the 4th and
		///   the 1st to 3rd is in the last week of the previous year<br>
		/// - if the 5th day of the year is a Monday, week two starts on the 5th and
		///   the 1st to 4th is in week one<br>
		/// </para>
		/// <para>
		/// This field can be used with any calendar system.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a week-based-year,
		/// week-of-year and day-of-week.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-year field
		/// is validated to ensure that the resulting week-based-year is the
		/// week-based-year requested.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-week-based-year field
		/// is validated from 1 to 53, meaning that the resulting date can be in the
		/// following week-based-year to that specified.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		/// are validated against the range of valid values. The resulting date is calculated
		/// equivalent to the following three stage approach.
		/// First, create a date on the first day of the first week in the requested week-based-year.
		/// Then take the week-of-week-based-year, subtract one, and add the amount in weeks to the date.
		/// Finally, adjust to the correct day-of-week within the localized week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a field providing access to the week-of-week-based-year, not null </returns>
		public TemporalField WeekOfWeekBasedYear()
		{
			return WeekOfWeekBasedYear_Renamed;
		}

		/// <summary>
		/// Returns a field to access the year of a week-based-year based on this {@code WeekFields}.
		/// <para>
		/// This represents the concept of the year where weeks start on a fixed day-of-week,
		/// such as Monday and each week belongs to exactly one year.
		/// This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/> and
		/// <seealso cref="WeekFields#weekOfWeekBasedYear()"/>.
		/// </para>
		/// <para>
		/// Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		/// where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		/// Thus, week one may start before the start of the year.
		/// If the first week starts after the start of the year then the period before
		/// is in the last week of the previous year.
		/// </para>
		/// <para>
		/// This field can be used with any calendar system.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a week-based-year,
		/// week-of-year and day-of-week.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-year field
		/// is validated to ensure that the resulting week-based-year is the
		/// week-based-year requested.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-week-based-year field
		/// is validated from 1 to 53, meaning that the resulting date can be in the
		/// following week-based-year to that specified.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		/// are validated against the range of valid values. The resulting date is calculated
		/// equivalent to the following three stage approach.
		/// First, create a date on the first day of the first week in the requested week-based-year.
		/// Then take the week-of-week-based-year, subtract one, and add the amount in weeks to the date.
		/// Finally, adjust to the correct day-of-week within the localized week.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a field providing access to the week-based-year, not null </returns>
		public TemporalField WeekBasedYear()
		{
			return WeekBasedYear_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this {@code WeekFields} is equal to the specified object.
		/// <para>
		/// The comparison is based on the entire state of the rules, which is
		/// the first day-of-week and minimal days.
		/// 
		/// </para>
		/// </summary>
		/// <param name="object">  the other rules to compare to, null returns false </param>
		/// <returns> true if this is equal to the specified rules </returns>
		public override bool Equals(Object @object)
		{
			if (this == @object)
			{
				return true;
			}
			if (@object is WeekFields)
			{
				return HashCode() == @object.HashCode();
			}
			return false;
		}

		/// <summary>
		/// A hash code for this {@code WeekFields}.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return FirstDayOfWeek_Renamed.ordinal() * 7 + MinimalDays;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// A string representation of this {@code WeekFields} instance.
		/// </summary>
		/// <returns> the string representation, not null </returns>
		public override String ToString()
		{
			return "WeekFields[" + FirstDayOfWeek_Renamed + ',' + MinimalDays + ']';
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Field type that computes DayOfWeek, WeekOfMonth, and WeekOfYear
		/// based on a WeekFields.
		/// A separate Field instance is required for each different WeekFields;
		/// combination of start of week and minimum number of days.
		/// Constructors are provided to create fields for DayOfWeek, WeekOfMonth,
		/// and WeekOfYear.
		/// </summary>
		internal class ComputedDayOfField : TemporalField
		{

			/// <summary>
			/// Returns a field to access the day of week,
			/// computed based on a WeekFields.
			/// <para>
			/// The WeekDefintion of the first day of the week is used with
			/// the ISO DAY_OF_WEEK field to compute week boundaries.
			/// </para>
			/// </summary>
			internal static ComputedDayOfField OfDayOfWeekField(WeekFields weekDef)
			{
				return new ComputedDayOfField("DayOfWeek", weekDef, DAYS, WEEKS, DAY_OF_WEEK_RANGE);
			}

			/// <summary>
			/// Returns a field to access the week of month,
			/// computed based on a WeekFields. </summary>
			/// <seealso cref= WeekFields#weekOfMonth() </seealso>
			internal static ComputedDayOfField OfWeekOfMonthField(WeekFields weekDef)
			{
				return new ComputedDayOfField("WeekOfMonth", weekDef, WEEKS, MONTHS, WEEK_OF_MONTH_RANGE);
			}

			/// <summary>
			/// Returns a field to access the week of year,
			/// computed based on a WeekFields. </summary>
			/// <seealso cref= WeekFields#weekOfYear() </seealso>
			internal static ComputedDayOfField OfWeekOfYearField(WeekFields weekDef)
			{
				return new ComputedDayOfField("WeekOfYear", weekDef, WEEKS, YEARS, WEEK_OF_YEAR_RANGE);
			}

			/// <summary>
			/// Returns a field to access the week of week-based-year,
			/// computed based on a WeekFields. </summary>
			/// <seealso cref= WeekFields#weekOfWeekBasedYear() </seealso>
			internal static ComputedDayOfField OfWeekOfWeekBasedYearField(WeekFields weekDef)
			{
				return new ComputedDayOfField("WeekOfWeekBasedYear", weekDef, WEEKS, IsoFields.WEEK_BASED_YEARS, WEEK_OF_WEEK_BASED_YEAR_RANGE);
			}

			/// <summary>
			/// Returns a field to access the week of week-based-year,
			/// computed based on a WeekFields. </summary>
			/// <seealso cref= WeekFields#weekBasedYear() </seealso>
			internal static ComputedDayOfField OfWeekBasedYearField(WeekFields weekDef)
			{
				return new ComputedDayOfField("WeekBasedYear", weekDef, IsoFields.WEEK_BASED_YEARS, FOREVER, ChronoField.YEAR.range());
			}

			/// <summary>
			/// Return a new week-based-year date of the Chronology, year, week-of-year,
			/// and dow of week. </summary>
			/// <param name="chrono"> The chronology of the new date </param>
			/// <param name="yowby"> the year of the week-based-year </param>
			/// <param name="wowby"> the week of the week-based-year </param>
			/// <param name="dow"> the day of the week </param>
			/// <returns> a ChronoLocalDate for the requested year, week of year, and day of week </returns>
			internal virtual ChronoLocalDate OfWeekBasedYear(Chronology chrono, int yowby, int wowby, int dow)
			{
				ChronoLocalDate date = chrono.Date(yowby, 1, 1);
				int ldow = LocalizedDayOfWeek(date);
				int offset = StartOfWeekOffset(1, ldow);

				// Clamp the week of year to keep it in the same year
				int yearLen = date.lengthOfYear();
				int newYearWeek = ComputeWeek(offset, yearLen + WeekDef.MinimalDaysInFirstWeek);
				wowby = System.Math.Min(wowby, newYearWeek - 1);

				int days = -offset + (dow - 1) + (wowby - 1) * 7;
				return date.Plus(days, DAYS);
			}

			internal readonly String Name;
			internal readonly WeekFields WeekDef;
			internal readonly TemporalUnit BaseUnit_Renamed;
			internal readonly TemporalUnit RangeUnit_Renamed;
			internal readonly ValueRange Range_Renamed;

			internal ComputedDayOfField(String name, WeekFields weekDef, TemporalUnit baseUnit, TemporalUnit rangeUnit, ValueRange range)
			{
				this.Name = name;
				this.WeekDef = weekDef;
				this.BaseUnit_Renamed = baseUnit;
				this.RangeUnit_Renamed = rangeUnit;
				this.Range_Renamed = range;
			}

			internal static readonly ValueRange DAY_OF_WEEK_RANGE = ValueRange.Of(1, 7);
			internal static readonly ValueRange WEEK_OF_MONTH_RANGE = ValueRange.Of(0, 1, 4, 6);
			internal static readonly ValueRange WEEK_OF_YEAR_RANGE = ValueRange.Of(0, 1, 52, 54);
			internal static readonly ValueRange WEEK_OF_WEEK_BASED_YEAR_RANGE = ValueRange.Of(1, 52, 53);

			public override long GetFrom(TemporalAccessor temporal)
			{
				if (RangeUnit_Renamed == WEEKS) // day-of-week
				{
					return LocalizedDayOfWeek(temporal);
				} // week-of-month
				else if (RangeUnit_Renamed == MONTHS)
				{
					return LocalizedWeekOfMonth(temporal);
				} // week-of-year
				else if (RangeUnit_Renamed == YEARS)
				{
					return LocalizedWeekOfYear(temporal);
				}
				else if (RangeUnit_Renamed == WEEK_BASED_YEARS)
				{
					return LocalizedWeekOfWeekBasedYear(temporal);
				}
				else if (RangeUnit_Renamed == FOREVER)
				{
					return LocalizedWeekBasedYear(temporal);
				}
				else
				{
					throw new IllegalStateException("unreachable, rangeUnit: " + RangeUnit_Renamed + ", this: " + this);
				}
			}

			internal virtual int LocalizedDayOfWeek(TemporalAccessor temporal)
			{
				int sow = WeekDef.FirstDayOfWeek.Value;
				int isoDow = temporal.get(DAY_OF_WEEK);
				return Math.FloorMod(isoDow - sow, 7) + 1;
			}

			internal virtual int LocalizedDayOfWeek(int isoDow)
			{
				int sow = WeekDef.FirstDayOfWeek.Value;
				return Math.FloorMod(isoDow - sow, 7) + 1;
			}

			internal virtual long LocalizedWeekOfMonth(TemporalAccessor temporal)
			{
				int dow = LocalizedDayOfWeek(temporal);
				int dom = temporal.get(DAY_OF_MONTH);
				int offset = StartOfWeekOffset(dom, dow);
				return ComputeWeek(offset, dom);
			}

			internal virtual long LocalizedWeekOfYear(TemporalAccessor temporal)
			{
				int dow = LocalizedDayOfWeek(temporal);
				int doy = temporal.get(DAY_OF_YEAR);
				int offset = StartOfWeekOffset(doy, dow);
				return ComputeWeek(offset, doy);
			}

			/// <summary>
			/// Returns the year of week-based-year for the temporal.
			/// The year can be the previous year, the current year, or the next year. </summary>
			/// <param name="temporal"> a date of any chronology, not null </param>
			/// <returns> the year of week-based-year for the date </returns>
			internal virtual int LocalizedWeekBasedYear(TemporalAccessor temporal)
			{
				int dow = LocalizedDayOfWeek(temporal);
				int year = temporal.get(YEAR);
				int doy = temporal.get(DAY_OF_YEAR);
				int offset = StartOfWeekOffset(doy, dow);
				int week = ComputeWeek(offset, doy);
				if (week == 0)
				{
					// Day is in end of week of previous year; return the previous year
					return year - 1;
				}
				else
				{
					// If getting close to end of year, use higher precision logic
					// Check if date of year is in partial week associated with next year
					ValueRange dayRange = temporal.range(DAY_OF_YEAR);
					int yearLen = (int)dayRange.Maximum;
					int newYearWeek = ComputeWeek(offset, yearLen + WeekDef.MinimalDaysInFirstWeek);
					if (week >= newYearWeek)
					{
						return year + 1;
					}
				}
				return year;
			}

			/// <summary>
			/// Returns the week of week-based-year for the temporal.
			/// The week can be part of the previous year, the current year,
			/// or the next year depending on the week start and minimum number
			/// of days. </summary>
			/// <param name="temporal">  a date of any chronology </param>
			/// <returns> the week of the year </returns>
			/// <seealso cref= #localizedWeekBasedYear(java.time.temporal.TemporalAccessor) </seealso>
			internal virtual int LocalizedWeekOfWeekBasedYear(TemporalAccessor temporal)
			{
				int dow = LocalizedDayOfWeek(temporal);
				int doy = temporal.get(DAY_OF_YEAR);
				int offset = StartOfWeekOffset(doy, dow);
				int week = ComputeWeek(offset, doy);
				if (week == 0)
				{
					// Day is in end of week of previous year
					// Recompute from the last day of the previous year
					ChronoLocalDate date = Chronology.from(temporal).date(temporal);
					date = date.minus(doy, DAYS); // Back down into previous year
					return LocalizedWeekOfWeekBasedYear(date);
				}
				else if (week > 50)
				{
					// If getting close to end of year, use higher precision logic
					// Check if date of year is in partial week associated with next year
					ValueRange dayRange = temporal.range(DAY_OF_YEAR);
					int yearLen = (int)dayRange.Maximum;
					int newYearWeek = ComputeWeek(offset, yearLen + WeekDef.MinimalDaysInFirstWeek);
					if (week >= newYearWeek)
					{
						// Overlaps with week of following year; reduce to week in following year
						week = week - newYearWeek + 1;
					}
				}
				return week;
			}

			/// <summary>
			/// Returns an offset to align week start with a day of month or day of year.
			/// </summary>
			/// <param name="day">  the day; 1 through infinity </param>
			/// <param name="dow">  the day of the week of that day; 1 through 7 </param>
			/// <returns>  an offset in days to align a day with the start of the first 'full' week </returns>
			internal virtual int StartOfWeekOffset(int day, int dow)
			{
				// offset of first day corresponding to the day of week in first 7 days (zero origin)
				int weekStart = Math.FloorMod(day - dow, 7);
				int offset = -weekStart;
				if (weekStart + 1 > WeekDef.MinimalDaysInFirstWeek)
				{
					// The previous week has the minimum days in the current month to be a 'week'
					offset = 7 - weekStart;
				}
				return offset;
			}

			/// <summary>
			/// Returns the week number computed from the reference day and reference dayOfWeek.
			/// </summary>
			/// <param name="offset"> the offset to align a date with the start of week
			///     from <seealso cref="#startOfWeekOffset"/>. </param>
			/// <param name="day">  the day for which to compute the week number </param>
			/// <returns> the week number where zero is used for a partial week and 1 for the first full week </returns>
			internal virtual int ComputeWeek(int offset, int day)
			{
				return ((7 + offset + (day - 1)) / 7);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
			public override R adjustInto<R>(R temporal, long newValue) where R : Temporal
			{
				// Check the new value and get the old value of the field
				int newVal = Range_Renamed.CheckValidIntValue(newValue, this); // lenient check range
				int currentVal = temporal.get(this);
				if (newVal == currentVal)
				{
					return temporal;
				}

				if (RangeUnit_Renamed == FOREVER) // replace year of WeekBasedYear
				{
					// Create a new date object with the same chronology,
					// the desired year and the same week and dow.
					int idow = temporal.get(WeekDef.DayOfWeek_Renamed);
					int wowby = temporal.get(WeekDef.WeekOfWeekBasedYear_Renamed);
					return (R) OfWeekBasedYear(Chronology.from(temporal), (int)newValue, wowby, idow);
				}
				else
				{
					// Compute the difference and add that using the base unit of the field
					return (R) temporal.plus(newVal - currentVal, BaseUnit_Renamed);
				}
			}

			public override ChronoLocalDate Resolve(IDictionary<TemporalField, Long> fieldValues, TemporalAccessor partialTemporal, ResolverStyle resolverStyle)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long value = fieldValues.get(this);
				long value = fieldValues[this];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newValue = Math.toIntExact(value);
				int newValue = Math.ToIntExact(value); // broad limit makes overflow checking lighter
				// first convert localized day-of-week to ISO day-of-week
				// doing this first handles case where both ISO and localized were parsed and might mismatch
				// day-of-week is always strict as two different day-of-week values makes lenient complex
				if (RangeUnit_Renamed == WEEKS) // day-of-week
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int checkedValue = range.checkValidIntValue(value, this);
					int checkedValue = Range_Renamed.CheckValidIntValue(value, this); // no leniency as too complex
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startDow = weekDef.getFirstDayOfWeek().getValue();
					int startDow = WeekDef.FirstDayOfWeek.Value;
					long isoDow = Math.FloorMod((startDow - 1) + (checkedValue - 1), 7) + 1;
					fieldValues.Remove(this);
					fieldValues[DAY_OF_WEEK] = isoDow;
					return TemporalField_Fields.Null;
				}

				// can only build date if ISO day-of-week is present
				if (fieldValues.ContainsKey(DAY_OF_WEEK) == false)
				{
					return TemporalField_Fields.Null;
				}
				int isoDow = DAY_OF_WEEK.checkValidIntValue(fieldValues[DAY_OF_WEEK]);
				int dow = LocalizedDayOfWeek(isoDow);

				// build date
				Chronology chrono = Chronology.from(partialTemporal);
				if (fieldValues.ContainsKey(YEAR))
				{
					int year = YEAR.checkValidIntValue(fieldValues[YEAR]); // validate
					if (RangeUnit_Renamed == MONTHS && fieldValues.ContainsKey(MONTH_OF_YEAR)) // week-of-month
					{
						long month = fieldValues[MONTH_OF_YEAR]; // not validated yet
						return ResolveWoM(fieldValues, chrono, year, month, newValue, dow, resolverStyle);
					}
					if (RangeUnit_Renamed == YEARS) // week-of-year
					{
						return ResolveWoY(fieldValues, chrono, year, newValue, dow, resolverStyle);
					}
				}
				else if ((RangeUnit_Renamed == WEEK_BASED_YEARS || RangeUnit_Renamed == FOREVER) && fieldValues.ContainsKey(WeekDef.WeekBasedYear_Renamed) && fieldValues.ContainsKey(WeekDef.WeekOfWeekBasedYear_Renamed)) // week-of-week-based-year and year-of-week-based-year
				{
					return ResolveWBY(fieldValues, chrono, dow, resolverStyle);
				}
				return TemporalField_Fields.Null;
			}

			internal virtual ChronoLocalDate ResolveWoM(IDictionary<TemporalField, Long> fieldValues, Chronology chrono, int year, long month, long wom, int localDow, ResolverStyle resolverStyle)
			{
				ChronoLocalDate date;
				if (resolverStyle == ResolverStyle.LENIENT)
				{
					date = chrono.Date(year, 1, 1).Plus(Math.SubtractExact(month, 1), MONTHS);
					long weeks = Math.SubtractExact(wom, LocalizedWeekOfMonth(date));
					int days = localDow - LocalizedDayOfWeek(date); // safe from overflow
					date = date.Plus(Math.AddExact(Math.MultiplyExact(weeks, 7), days), DAYS);
				}
				else
				{
					int monthValid = MONTH_OF_YEAR.checkValidIntValue(month); // validate
					date = chrono.Date(year, monthValid, 1);
					int womInt = Range_Renamed.CheckValidIntValue(wom, this); // validate
					int weeks = (int)(womInt - LocalizedWeekOfMonth(date)); // safe from overflow
					int days = localDow - LocalizedDayOfWeek(date); // safe from overflow
					date = date.Plus(weeks * 7 + days, DAYS);
					if (resolverStyle == ResolverStyle.STRICT && date.GetLong(MONTH_OF_YEAR) != month)
					{
						throw new DateTimeException("Strict mode rejected resolved date as it is in a different month");
					}
				}
				fieldValues.Remove(this);
				fieldValues.Remove(YEAR);
				fieldValues.Remove(MONTH_OF_YEAR);
				fieldValues.Remove(DAY_OF_WEEK);
				return date;
			}

			internal virtual ChronoLocalDate ResolveWoY(IDictionary<TemporalField, Long> fieldValues, Chronology chrono, int year, long woy, int localDow, ResolverStyle resolverStyle)
			{
				ChronoLocalDate date = chrono.Date(year, 1, 1);
				if (resolverStyle == ResolverStyle.LENIENT)
				{
					long weeks = Math.SubtractExact(woy, LocalizedWeekOfYear(date));
					int days = localDow - LocalizedDayOfWeek(date); // safe from overflow
					date = date.Plus(Math.AddExact(Math.MultiplyExact(weeks, 7), days), DAYS);
				}
				else
				{
					int womInt = Range_Renamed.CheckValidIntValue(woy, this); // validate
					int weeks = (int)(womInt - LocalizedWeekOfYear(date)); // safe from overflow
					int days = localDow - LocalizedDayOfWeek(date); // safe from overflow
					date = date.Plus(weeks * 7 + days, DAYS);
					if (resolverStyle == ResolverStyle.STRICT && date.GetLong(YEAR) != year)
					{
						throw new DateTimeException("Strict mode rejected resolved date as it is in a different year");
					}
				}
				fieldValues.Remove(this);
				fieldValues.Remove(YEAR);
				fieldValues.Remove(DAY_OF_WEEK);
				return date;
			}

			internal virtual ChronoLocalDate ResolveWBY(IDictionary<TemporalField, Long> fieldValues, Chronology chrono, int localDow, ResolverStyle resolverStyle)
			{
				int yowby = WeekDef.WeekBasedYear_Renamed.Range().CheckValidIntValue(fieldValues[WeekDef.WeekBasedYear_Renamed], WeekDef.WeekBasedYear_Renamed);
				ChronoLocalDate date;
				if (resolverStyle == ResolverStyle.LENIENT)
				{
					date = OfWeekBasedYear(chrono, yowby, 1, localDow);
					long wowby = fieldValues[WeekDef.WeekOfWeekBasedYear_Renamed];
					long weeks = Math.SubtractExact(wowby, 1);
					date = date.Plus(weeks, WEEKS);
				}
				else
				{
					int wowby = WeekDef.WeekOfWeekBasedYear_Renamed.Range().CheckValidIntValue(fieldValues[WeekDef.WeekOfWeekBasedYear_Renamed], WeekDef.WeekOfWeekBasedYear_Renamed); // validate
					date = OfWeekBasedYear(chrono, yowby, wowby, localDow);
					if (resolverStyle == ResolverStyle.STRICT && LocalizedWeekBasedYear(date) != yowby)
					{
						throw new DateTimeException("Strict mode rejected resolved date as it is in a different week-based-year");
					}
				}
				fieldValues.Remove(this);
				fieldValues.Remove(WeekDef.WeekBasedYear_Renamed);
				fieldValues.Remove(WeekDef.WeekOfWeekBasedYear_Renamed);
				fieldValues.Remove(DAY_OF_WEEK);
				return date;
			}

			//-----------------------------------------------------------------------
			public override String GetDisplayName(Locale locale)
			{
				Objects.RequireNonNull(locale, "locale");
				if (RangeUnit_Renamed == YEARS) // only have values for week-of-year
				{
					LocaleResources lr = LocaleProviderAdapter.ResourceBundleBased.getLocaleResources(locale);
					ResourceBundle rb = lr.JavaTimeFormatData;
					return rb.ContainsKey("field.week") ? rb.GetString("field.week") : Name;
				}
				return Name;
			}

			public override TemporalUnit BaseUnit
			{
				get
				{
					return BaseUnit_Renamed;
				}
			}

			public override TemporalUnit RangeUnit
			{
				get
				{
					return RangeUnit_Renamed;
				}
			}

			public override bool DateBased
			{
				get
				{
					return true;
				}
			}

			public override bool TimeBased
			{
				get
				{
					return false;
				}
			}

			public override ValueRange Range()
			{
				return Range_Renamed;
			}

			//-----------------------------------------------------------------------
			public override bool IsSupportedBy(TemporalAccessor temporal)
			{
				if (temporal.IsSupported(DAY_OF_WEEK))
				{
					if (RangeUnit_Renamed == WEEKS) // day-of-week
					{
						return true;
					} // week-of-month
					else if (RangeUnit_Renamed == MONTHS)
					{
						return temporal.IsSupported(DAY_OF_MONTH);
					} // week-of-year
					else if (RangeUnit_Renamed == YEARS)
					{
						return temporal.IsSupported(DAY_OF_YEAR);
					}
					else if (RangeUnit_Renamed == WEEK_BASED_YEARS)
					{
						return temporal.IsSupported(DAY_OF_YEAR);
					}
					else if (RangeUnit_Renamed == FOREVER)
					{
						return temporal.IsSupported(YEAR);
					}
				}
				return false;
			}

			public override ValueRange RangeRefinedBy(TemporalAccessor temporal)
			{
				if (RangeUnit_Renamed == ChronoUnit.WEEKS) // day-of-week
				{
					return Range_Renamed;
				} // week-of-month
				else if (RangeUnit_Renamed == MONTHS)
				{
					return RangeByWeek(temporal, DAY_OF_MONTH);
				} // week-of-year
				else if (RangeUnit_Renamed == YEARS)
				{
					return RangeByWeek(temporal, DAY_OF_YEAR);
				}
				else if (RangeUnit_Renamed == WEEK_BASED_YEARS)
				{
					return RangeWeekOfWeekBasedYear(temporal);
				}
				else if (RangeUnit_Renamed == FOREVER)
				{
					return YEAR.range();
				}
				else
				{
					throw new IllegalStateException("unreachable, rangeUnit: " + RangeUnit_Renamed + ", this: " + this);
				}
			}

			/// <summary>
			/// Map the field range to a week range </summary>
			/// <param name="temporal"> the temporal </param>
			/// <param name="field"> the field to get the range of </param>
			/// <returns> the ValueRange with the range adjusted to weeks. </returns>
			internal virtual ValueRange RangeByWeek(TemporalAccessor temporal, TemporalField field)
			{
				int dow = LocalizedDayOfWeek(temporal);
				int offset = StartOfWeekOffset(temporal.get(field), dow);
				ValueRange fieldRange = temporal.range(field);
				return ValueRange.Of(ComputeWeek(offset, (int) fieldRange.Minimum), ComputeWeek(offset, (int) fieldRange.Maximum));
			}

			/// <summary>
			/// Map the field range to a week range of a week year. </summary>
			/// <param name="temporal">  the temporal </param>
			/// <returns> the ValueRange with the range adjusted to weeks. </returns>
			internal virtual ValueRange RangeWeekOfWeekBasedYear(TemporalAccessor temporal)
			{
				if (!temporal.IsSupported(DAY_OF_YEAR))
				{
					return WEEK_OF_YEAR_RANGE;
				}
				int dow = LocalizedDayOfWeek(temporal);
				int doy = temporal.get(DAY_OF_YEAR);
				int offset = StartOfWeekOffset(doy, dow);
				int week = ComputeWeek(offset, doy);
				if (week == 0)
				{
					// Day is in end of week of previous year
					// Recompute from the last day of the previous year
					ChronoLocalDate date = Chronology.from(temporal).date(temporal);
					date = date.minus(doy + 7, DAYS); // Back down into previous year
					return RangeWeekOfWeekBasedYear(date);
				}
				// Check if day of year is in partial week associated with next year
				ValueRange dayRange = temporal.range(DAY_OF_YEAR);
				int yearLen = (int)dayRange.Maximum;
				int newYearWeek = ComputeWeek(offset, yearLen + WeekDef.MinimalDaysInFirstWeek);

				if (week >= newYearWeek)
				{
					// Overlaps with weeks of following year; recompute from a week in following year
					ChronoLocalDate date = Chronology.from(temporal).date(temporal);
					date = date.Plus(yearLen - doy + 1 + 7, ChronoUnit.DAYS);
					return RangeWeekOfWeekBasedYear(date);
				}
				return ValueRange.Of(1, newYearWeek - 1);
			}

			//-----------------------------------------------------------------------
			public override String ToString()
			{
				return Name + "[" + WeekDef.ToString() + "]";
			}
		}
	}

}