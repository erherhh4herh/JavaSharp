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



	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleResources = sun.util.locale.provider.LocaleResources;

	/// <summary>
	/// Fields and units specific to the ISO-8601 calendar system,
	/// including quarter-of-year and week-based-year.
	/// <para>
	/// This class defines fields and units that are specific to the ISO calendar system.
	/// 
	/// <h3>Quarter of year</h3>
	/// The ISO-8601 standard is based on the standard civic 12 month year.
	/// This is commonly divided into four quarters, often abbreviated as Q1, Q2, Q3 and Q4.
	/// </para>
	/// <para>
	/// January, February and March are in Q1.
	/// April, May and June are in Q2.
	/// July, August and September are in Q3.
	/// October, November and December are in Q4.
	/// </para>
	/// <para>
	/// The complete date is expressed using three fields:
	/// <ul>
	/// <li><seealso cref="#DAY_OF_QUARTER DAY_OF_QUARTER"/> - the day within the quarter, from 1 to 90, 91 or 92
	/// <li><seealso cref="#QUARTER_OF_YEAR QUARTER_OF_YEAR"/> - the week within the week-based-year
	/// <li><seealso cref="ChronoField#YEAR YEAR"/> - the standard ISO year
	/// </ul>
	/// 
	/// <h3>Week based years</h3>
	/// The ISO-8601 standard was originally intended as a data interchange format,
	/// defining a string format for dates and times. However, it also defines an
	/// alternate way of expressing the date, based on the concept of week-based-year.
	/// </para>
	/// <para>
	/// The date is expressed using three fields:
	/// <ul>
	/// <li><seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> - the standard field defining the
	///  day-of-week from Monday (1) to Sunday (7)
	/// <li><seealso cref="#WEEK_OF_WEEK_BASED_YEAR"/> - the week within the week-based-year
	/// <li><seealso cref="#WEEK_BASED_YEAR WEEK_BASED_YEAR"/> - the week-based-year
	/// </ul>
	/// The week-based-year itself is defined relative to the standard ISO proleptic year.
	/// It differs from the standard year in that it always starts on a Monday.
	/// </para>
	/// <para>
	/// The first week of a week-based-year is the first Monday-based week of the standard
	/// ISO year that has at least 4 days in the new year.
	/// <ul>
	/// <li>If January 1st is Monday then week 1 starts on January 1st
	/// <li>If January 1st is Tuesday then week 1 starts on December 31st of the previous standard year
	/// <li>If January 1st is Wednesday then week 1 starts on December 30th of the previous standard year
	/// <li>If January 1st is Thursday then week 1 starts on December 29th of the previous standard year
	/// <li>If January 1st is Friday then week 1 starts on January 4th
	/// <li>If January 1st is Saturday then week 1 starts on January 3rd
	/// <li>If January 1st is Sunday then week 1 starts on January 2nd
	/// </ul>
	/// There are 52 weeks in most week-based years, however on occasion there are 53 weeks.
	/// </para>
	/// <para>
	/// For example:
	/// 
	/// <table cellpadding="0" cellspacing="3" border="0" style="text-align: left; width: 50%;">
	/// <caption>Examples of Week based Years</caption>
	/// <tr><th>Date</th><th>Day-of-week</th><th>Field values</th></tr>
	/// <tr><th>2008-12-28</th><td>Sunday</td><td>Week 52 of week-based-year 2008</td></tr>
	/// <tr><th>2008-12-29</th><td>Monday</td><td>Week 1 of week-based-year 2009</td></tr>
	/// <tr><th>2008-12-31</th><td>Wednesday</td><td>Week 1 of week-based-year 2009</td></tr>
	/// <tr><th>2009-01-01</th><td>Thursday</td><td>Week 1 of week-based-year 2009</td></tr>
	/// <tr><th>2009-01-04</th><td>Sunday</td><td>Week 1 of week-based-year 2009</td></tr>
	/// <tr><th>2009-01-05</th><td>Monday</td><td>Week 2 of week-based-year 2009</td></tr>
	/// </table>
	/// 
	/// @implSpec
	/// </para>
	/// <para>
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class IsoFields
	{

		/// <summary>
		/// The field that represents the day-of-quarter.
		/// <para>
		/// This field allows the day-of-quarter value to be queried and set.
		/// The day-of-quarter has values from 1 to 90 in Q1 of a standard year, from 1 to 91
		/// in Q1 of a leap year, from 1 to 91 in Q2 and from 1 to 92 in Q3 and Q4.
		/// </para>
		/// <para>
		/// The day-of-quarter can only be calculated if the day-of-year, month-of-year and year
		/// are available.
		/// </para>
		/// <para>
		/// When setting this field, the value is allowed to be partially lenient, taking any
		/// value from 1 to 92. If the quarter has less than 92 days, then day 92, and
		/// potentially day 91, is in the following quarter.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a year,
		/// quarter-of-year and day-of-quarter.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		/// validated against their range of valid values. The day-of-quarter field
		/// is validated from 1 to 90, 91 or 92 depending on the year and quarter.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		/// validated against their range of valid values. The day-of-quarter field is
		/// validated between 1 and 92, ignoring the actual range based on the year and quarter.
		/// If the day-of-quarter exceeds the actual range by one day, then the resulting date
		/// is one day later. If the day-of-quarter exceeds the actual range by two days,
		/// then the resulting date is two days later.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, only the year is validated
		/// against the range of valid values. The resulting date is calculated equivalent to
		/// the following three stage approach. First, create a date on the first of January
		/// in the requested year. Then take the quarter-of-year, subtract one, and add the
		/// amount in quarters to the date. Finally, take the day-of-quarter, subtract one,
		/// and add the amount in days to the date.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalField DAY_OF_QUARTER = Field.DAY_OF_QUARTER;
		/// <summary>
		/// The field that represents the quarter-of-year.
		/// <para>
		/// This field allows the quarter-of-year value to be queried and set.
		/// The quarter-of-year has values from 1 to 4.
		/// </para>
		/// <para>
		/// The quarter-of-year can only be calculated if the month-of-year is available.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a year,
		/// quarter-of-year and day-of-quarter.
		/// See <seealso cref="#DAY_OF_QUARTER"/> for details.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalField QUARTER_OF_YEAR = Field.QUARTER_OF_YEAR;
		/// <summary>
		/// The field that represents the week-of-week-based-year.
		/// <para>
		/// This field allows the week of the week-based-year value to be queried and set.
		/// The week-of-week-based-year has values from 1 to 52, or 53 if the
		/// week-based-year has 53 weeks.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a
		/// week-based-year, week-of-week-based-year and day-of-week.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-week-based-year
		/// field is validated from 1 to 52 or 53 depending on the week-based-year.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		/// validated against their range of valid values. The week-of-week-based-year
		/// field is validated between 1 and 53, ignoring the week-based-year.
		/// If the week-of-week-based-year is 53, but the week-based-year only has
		/// 52 weeks, then the resulting date is in week 1 of the following week-based-year.
		/// </para>
		/// <para>
		/// In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, only the week-based-year
		/// is validated against the range of valid values. If the day-of-week is outside
		/// the range 1 to 7, then the resulting date is adjusted by a suitable number of
		/// weeks to reduce the day-of-week to the range 1 to 7. If the week-of-week-based-year
		/// value is outside the range 1 to 52, then any excess weeks are added or subtracted
		/// from the resulting date.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalField WEEK_OF_WEEK_BASED_YEAR = Field.WEEK_OF_WEEK_BASED_YEAR;
		/// <summary>
		/// The field that represents the week-based-year.
		/// <para>
		/// This field allows the week-based-year value to be queried and set.
		/// </para>
		/// <para>
		/// The field has a range that matches <seealso cref="LocalDate#MAX"/> and <seealso cref="LocalDate#MIN"/>.
		/// </para>
		/// <para>
		/// In the resolving phase of parsing, a date can be created from a
		/// week-based-year, week-of-week-based-year and day-of-week.
		/// See <seealso cref="#WEEK_OF_WEEK_BASED_YEAR"/> for details.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalField WEEK_BASED_YEAR = Field.WEEK_BASED_YEAR;
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
		/// for the week-based-year field. If the resulting week-based-year only has 52 weeks,
		/// then the date will be in week 1 of the following week-based-year.
		/// </para>
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalUnit WEEK_BASED_YEARS = Unit.WEEK_BASED_YEARS;
		/// <summary>
		/// Unit that represents the concept of a quarter-year.
		/// For the ISO calendar system, it is equal to 3 months.
		/// The estimated duration of a quarter-year is one quarter of {@code 365.2425 Days}.
		/// <para>
		/// This unit is an immutable and thread-safe singleton.
		/// </para>
		/// </summary>
		public static readonly TemporalUnit QUARTER_YEARS = Unit.QUARTER_YEARS;

		/// <summary>
		/// Restricted constructor.
		/// </summary>
		private IsoFields()
		{
			throw new AssertionError("Not instantiable");
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of the field.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: private static enum Field implements TemporalField
		private enum Field
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			DAY_OF_QUARTER
			{
				public TemporalUnit getBaseUnit()
				{
					return DAYS;
				}
				public TemporalUnit getRangeUnit()
				{
					return QUARTER_YEARS;
				}
				public ValueRange range()
				{
					return ValueRange.of(1, 90, 92);
				}
				public boolean isSupportedBy(TemporalAccessor temporal)
				{
					return = temporal
				}
				public ValueRange rangeRefinedBy(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: DayOfQuarter");
					}
					long = QUARTER_OF_YEAR
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (qoy == 1)
					{
						long = YEAR
						return = java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year) ? ValueRange.of(1, 91) : ValueRange.of(1, 90)
					}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					else if (qoy == 2)
					{
						return ValueRange.of(1, 91);
					}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					else if (qoy == 3 || qoy == 4)
					{
						return ValueRange.of(1, 92);
					} // else value not from 1 to 4, so drop through
					return = 
				}
				public long getFrom(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: DayOfQuarter");
					}
					int = DAY_OF_YEAR
					int = MONTH_OF_YEAR
					long = YEAR
					return doy - QUARTER_DAYS[((moy - 1) / 3) + (java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year) ? 4 : 0)]
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
				public <R> R adjustInto(R temporal, long newValue) where R : Temporal
				{
					// calls getFrom() to check if supported
					long = temporal
					range().checkValidValue(newValue, this); // leniently check from 1 to 92 TODO: check
					return (R) temporal.with(DAY_OF_YEAR, temporal.getLong(DAY_OF_YEAR) + (newValue - curValue));
				}
				public java.time.chrono.ChronoLocalDate resolve(java.util.Map<TemporalField, Long> fieldValues, TemporalAccessor partialTemporal, java.time.format.ResolverStyle resolverStyle)
				{
					Long = YEAR
					Long = QUARTER_OF_YEAR
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (yearLong == null || qoyLong == null)
					{
						return null;
					}
					int = yearLong // always validate
					long = DAY_OF_QUARTER
					ensureIso = partialTemporal
					java.time.LocalDate date;
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (resolverStyle == java.time.format.ResolverStyle.LENIENT)
					{
						date = Math.multiplyExact(Math.subtractExact(qoyLong, 1), 3)
						doq = Math.subtractExact(doq, 1);
					}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					else
					{
						int qoy = QUARTER_OF_YEAR.range().checkValidIntValue(qoyLong, QUARTER_OF_YEAR); // validated
						date = java.time.LocalDate.of(y, ((qoy - 1) * 3) + 1, 1);
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
						if (doq < 1 || doq > 90)
						{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							if (resolverStyle == java.time.format.ResolverStyle.STRICT)
							{
								rangeRefinedBy(date).checkValidValue(doq, this); // only allow exact range
							} // SMART
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							else
							{
								range().checkValidValue(doq, this); // allow 1-92 rolling into next quarter
							}
						}
						doq--;
					}
					fieldValues = this
					fieldValues = YEAR
					fieldValues = QUARTER_OF_YEAR
					return = doq
				}
				public String toString()
				{
					return "DayOfQuarter";
				}
			},
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			QUARTER_OF_YEAR
			{
				public TemporalUnit getBaseUnit()
				{
					return QUARTER_YEARS;
				}
				public TemporalUnit getRangeUnit()
				{
					return YEARS;
				}
				public ValueRange range()
				{
					return ValueRange.of(1, 4);
				}
				public boolean isSupportedBy(TemporalAccessor temporal)
				{
					return = temporal
				}
				public long getFrom(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: QuarterOfYear");
					}
					long = MONTH_OF_YEAR
					return = (moy + 2) / 3
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
				public <R> R adjustInto(R temporal, long newValue) where R : Temporal
				{
					// calls getFrom() to check if supported
					long = temporal
					range().checkValidValue(newValue, this); // strictly check from 1 to 4
					return (R) temporal.with(MONTH_OF_YEAR, temporal.getLong(MONTH_OF_YEAR) + (newValue - curValue) * 3);
				}
				public String toString()
				{
					return "QuarterOfYear";
				}
			},
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			WEEK_OF_WEEK_BASED_YEAR
			{
				public String getDisplayName(java.util.Locale locale)
				{
					java.util.Objects.requireNonNull(locale, "locale");
					sun = locale
					java.util.ResourceBundle rb = lr.getJavaTimeFormatData();
					return = 
				}

				public TemporalUnit getBaseUnit()
				{
					return WEEKS;
				}
				public TemporalUnit getRangeUnit()
				{
					return WEEK_BASED_YEARS;
				}
				public ValueRange range()
				{
					return ValueRange.of(1, 52, 53);
				}
				public boolean isSupportedBy(TemporalAccessor temporal)
				{
					return = temporal
				}
				public ValueRange rangeRefinedBy(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: WeekOfWeekBasedYear");
					}
					return = java.time.LocalDate.from(temporal)
				}
				public long getFrom(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: WeekOfWeekBasedYear");
					}
					return = java.time.LocalDate.from(temporal)
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
				public <R> R adjustInto(R temporal, long newValue) where R : Temporal
				{
					// calls getFrom() to check if supported
					range().checkValidValue(newValue, this); // lenient range
					return (R) temporal.plus(Math.subtractExact(newValue, getFrom(temporal)), WEEKS);
				}
				public java.time.chrono.ChronoLocalDate resolve(java.util.Map<TemporalField, Long> fieldValues, TemporalAccessor partialTemporal, java.time.format.ResolverStyle resolverStyle)
				{
					Long = WEEK_BASED_YEAR
					Long = DAY_OF_WEEK
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (wbyLong == null || dowLong == null)
					{
						return null;
					}
					int wby = WEEK_BASED_YEAR.range().checkValidIntValue(wbyLong, WEEK_BASED_YEAR); // always validate
					long = WEEK_OF_WEEK_BASED_YEAR
					ensureIso = partialTemporal
					java.time.LocalDate date = java.time.LocalDate.of(wby, 1, 4);
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (resolverStyle == java.time.format.ResolverStyle.LENIENT)
					{
						long dow = dowLong; // unvalidated
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
						if (dow > 7)
						{
							date = (dow - 1) / 7
							dow = ((dow - 1) % 7) + 1
						}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
						else if (dow < 1)
						{
							date = Math.subtractExact(dow, 7) / 7
							dow = ((dow + 6) % 7) + 1
						}
						date = date.plusWeeks(Math.subtractExact(wowby, 1)).with(DAY_OF_WEEK, dow);
					}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					else
					{
						int = dowLong // validated
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
						if (wowby < 1 || wowby > 52)
						{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							if (resolverStyle == java.time.format.ResolverStyle.STRICT)
							{
								getWeekRange(date).checkValidValue(wowby, this); // only allow exact range
							} // SMART
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							else
							{
								range().checkValidValue(wowby, this); // allow 1-53 rolling into next year
							}
						}
						date = date.plusWeeks(wowby - 1).with(DAY_OF_WEEK, dow);
					}
					fieldValues = this
					fieldValues = WEEK_BASED_YEAR
					fieldValues = DAY_OF_WEEK
					return date;
				}
				public String toString()
				{
					return "WeekOfWeekBasedYear";
				}
			},
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			WEEK_BASED_YEAR
			{
				public TemporalUnit getBaseUnit()
				{
					return WEEK_BASED_YEARS;
				}
				public TemporalUnit getRangeUnit()
				{
					return FOREVER;
				}
				public ValueRange range()
				{
					return = 
				}
				public boolean isSupportedBy(TemporalAccessor temporal)
				{
					return = temporal
				}
				public long getFrom(TemporalAccessor temporal)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: WeekBasedYear");
					}
					return = java.time.LocalDate.from(temporal)
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R adjustInto(R temporal, long newValue)
				public <R> R adjustInto(R temporal, long newValue) where R : Temporal
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (isSupportedBy(temporal) == false)
					{
						throw new UnsupportedTemporalTypeException("Unsupported field: WeekBasedYear");
					}
					int newWby = range().checkValidIntValue(newValue, WEEK_BASED_YEAR); // strict check
					java = temporal
					int = DAY_OF_WEEK
					int = date
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (week == 53 && getWeekRange(newWby) == 52)
					{
						week = 52
					}
					java.time.LocalDate resolved = java.time.LocalDate.of(newWby, 1, 4); // 4th is guaranteed to be in week one
					int = (week - 1) * 7
					resolved = days
					return = resolved
				}
				public String toString()
				{
					return "WeekBasedYear";
				}
			}

			public boolean isDateBased()
			{
				return true;
			}

			public boolean isTimeBased()
			{
				return false;
			}

			public ValueRange rangeRefinedBy(TemporalAccessor temporal)
			{
				return = 
			}

			//-------------------------------------------------------------------------
			private static final int[] QUARTER_DAYS = {0, 90, 181, 273, 0, 91, 182, 274};

			private static boolean isIso(TemporalAccessor temporal)
			{
				return = java.time.chrono.IsoChronology.INSTANCE
			}

			private static void ensureIso(TemporalAccessor temporal)
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (isIso(temporal) == false)
				{
					throw new java.time.DateTimeException("Resolve requires IsoChronology");
				}
			}

			private static ValueRange getWeekRange(java.time.LocalDate date)
			{
				int = date
				return ValueRange.of(1, getWeekRange(wby));
			}

			private static int getWeekRange(int wby)
			{
				java.time.LocalDate date = java.time.LocalDate.of(wby, 1, 1);
				// 53 weeks if standard year starts on Thursday, or Wed in a leap year
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (date.getDayOfWeek() == THURSDAY || (date.getDayOfWeek() == WEDNESDAY && date.isLeapYear()))
				{
					return 53;
				}
				return 52;
			}

			private static int getWeek(java.time.LocalDate date)
			{
				int dow0 = date.getDayOfWeek().ordinal();
				int doy0 = date.getDayOfYear() - 1;
				int = 3 - dow0 // adjust to mid-week Thursday (which is 3 indexed from zero)
				int alignedWeek = doyThu0 / 7;
				int = alignedWeek * 7
				int firstMonDoy0 = firstThuDoy0 - 3;
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (firstMonDoy0 < -3)
				{
					firstMonDoy0 += 7;
				}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (doy0 < firstMonDoy0)
				{
					return = 
				}
				int week = ((doy0 - firstMonDoy0) / 7) + 1;
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (week == 53)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if ((firstMonDoy0 == -3 || (firstMonDoy0 == -2 && date.isLeapYear())) == false)
					{
						week = 1
					}
				}
				return week;
			}

			private static int getWeekBasedYear(java.time.LocalDate date)
			{
				int year = date.getYear();
				int doy = date.getDayOfYear();
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (doy <= 3)
				{
					int dow = date.getDayOfWeek().ordinal();
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (doy - dow < -2)
					{
						year--;
					}
				}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				else if (doy >= 363)
				{
					int dow = date.getDayOfWeek().ordinal();
					doy = date.isLeapYear() ? 1 : 0
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					if (doy - dow >= 0)
					{
						year++;
					}
				}
				return year;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of the unit.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: private static enum Unit implements TemporalUnit
		private enum Unit
		{

			/// <summary>
			/// Unit that represents the concept of a week-based-year.
			/// </summary>
			WEEK_BASED_YEARS("WeekBasedYears", java.time.Duration.ofSeconds(31556952L)),
			/// <summary>
			/// Unit that represents the concept of a quarter-year.
			/// </summary>
			QUARTER_YEARS("QuarterYears", java.time.Duration.ofSeconds(31556952L / 4));

			private final String name;
			private final java.time.Duration duration;

			private Unit(String name, java.time.Duration estimatedDuration)
			{
				this.name = name
				this.duration = estimatedDuration
			}

			public java.time.Duration getDuration()
			{
				return duration;
			}

			public boolean isDurationEstimated()
			{
				return true;
			}

			public boolean isDateBased()
			{
				return true;
			}

			public boolean isTimeBased()
			{
				return false;
			}

			public boolean isSupportedBy(Temporal temporal)
			{
				return = EPOCH_DAY
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R extends Temporal> R addTo(R temporal, long amount)
			public <R> R addTo(R temporal, long amount) where R : Temporal
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				switch (this)
				{
					case WEEK_BASED_YEARS:
						return (R) temporal.with(WEEK_BASED_YEAR, Math.addExact(temporal.get(WEEK_BASED_YEAR), amount));
					case QUARTER_YEARS:
						// no overflow (256 is multiple of 4)
						return (R) temporal.plus(amount / 256, YEARS).plus((amount % 256) * 3, MONTHS);
					default:
						throw new IllegalStateException("Unreachable");
				}
			}

			public long between(Temporal temporal1Inclusive, Temporal temporal2Exclusive)
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (temporal1Inclusive.getClass() != temporal2Exclusive.getClass())
				{
					return temporal1Inclusive.until(temporal2Exclusive, this);
				}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				switch (this)
				{
					case WEEK_BASED_YEARS:
						return Math.subtractExact(temporal2Exclusive.getLong(WEEK_BASED_YEAR), temporal1Inclusive.getLong(WEEK_BASED_YEAR));
					case QUARTER_YEARS:
						return temporal1Inclusive.until(temporal2Exclusive, MONTHS) / 3
					default:
						throw new IllegalStateException("Unreachable");
				}
			}

			public String toString()
			{
				return name;
			}
		}
	}

}