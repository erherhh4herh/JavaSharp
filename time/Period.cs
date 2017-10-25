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
 * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
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
	/// A date-based amount of time in the ISO-8601 calendar system,
	/// such as '2 years, 3 months and 4 days'.
	/// <para>
	/// This class models a quantity or amount of time in terms of years, months and days.
	/// See <seealso cref="Duration"/> for the time-based equivalent to this class.
	/// </para>
	/// <para>
	/// Durations and periods differ in their treatment of daylight savings time
	/// when added to <seealso cref="ZonedDateTime"/>. A {@code Duration} will add an exact
	/// number of seconds, thus a duration of one day is always exactly 24 hours.
	/// By contrast, a {@code Period} will add a conceptual day, trying to maintain
	/// the local time.
	/// </para>
	/// <para>
	/// For example, consider adding a period of one day and a duration of one day to
	/// 18:00 on the evening before a daylight savings gap. The {@code Period} will add
	/// the conceptual day and result in a {@code ZonedDateTime} at 18:00 the following day.
	/// By contrast, the {@code Duration} will add exactly 24 hours, resulting in a
	/// {@code ZonedDateTime} at 19:00 the following day (assuming a one hour DST gap).
	/// </para>
	/// <para>
	/// The supported units of a period are <seealso cref="ChronoUnit#YEARS YEARS"/>,
	/// <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
	/// All three fields are always present, but may be set to zero.
	/// </para>
	/// <para>
	/// The ISO-8601 calendar system is the modern civil calendar system used today
	/// in most of the world. It is equivalent to the proleptic Gregorian calendar
	/// system, in which today's rules for leap years are applied for all time.
	/// </para>
	/// <para>
	/// The period is modeled as a directed amount of time, meaning that individual parts of the
	/// period may be negative.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code Period} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Period : ChronoPeriod
	{

		/// <summary>
		/// A constant for a period of zero.
		/// </summary>
		public static readonly Period ZERO = new Period(0, 0, 0);
		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -3587258372562876L;
		/// <summary>
		/// The pattern for parsing.
		/// </summary>
		private static readonly Pattern PATTERN = Pattern.Compile("([-+]?)P(?:([-+]?[0-9]+)Y)?(?:([-+]?[0-9]+)M)?(?:([-+]?[0-9]+)W)?(?:([-+]?[0-9]+)D)?", Pattern.CASE_INSENSITIVE);

		/// <summary>
		/// The set of supported units.
		/// </summary>
		private static readonly IList<TemporalUnit> SUPPORTED_UNITS = Collections.UnmodifiableList(Arrays.asList<TemporalUnit>(YEARS, MONTHS, DAYS));

		/// <summary>
		/// The number of years.
		/// </summary>
		private readonly int Years_Renamed;
		/// <summary>
		/// The number of months.
		/// </summary>
		private readonly int Months_Renamed;
		/// <summary>
		/// The number of days.
		/// </summary>
		private readonly int Days_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Period} representing a number of years.
		/// <para>
		/// The resulting period will have the specified years.
		/// The months and days units will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the number of years, positive or negative </param>
		/// <returns> the period of years, not null </returns>
		public static Period OfYears(int years)
		{
			return Create(years, 0, 0);
		}

		/// <summary>
		/// Obtains a {@code Period} representing a number of months.
		/// <para>
		/// The resulting period will have the specified months.
		/// The years and days units will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the number of months, positive or negative </param>
		/// <returns> the period of months, not null </returns>
		public static Period OfMonths(int months)
		{
			return Create(0, months, 0);
		}

		/// <summary>
		/// Obtains a {@code Period} representing a number of weeks.
		/// <para>
		/// The resulting period will be day-based, with the amount of days
		/// equal to the number of weeks multiplied by 7.
		/// The years and months units will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeks">  the number of weeks, positive or negative </param>
		/// <returns> the period, with the input weeks converted to days, not null </returns>
		public static Period OfWeeks(int weeks)
		{
			return Create(0, 0, Math.MultiplyExact(weeks, 7));
		}

		/// <summary>
		/// Obtains a {@code Period} representing a number of days.
		/// <para>
		/// The resulting period will have the specified days.
		/// The years and months units will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the number of days, positive or negative </param>
		/// <returns> the period of days, not null </returns>
		public static Period OfDays(int days)
		{
			return Create(0, 0, days);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Period} representing a number of years, months and days.
		/// <para>
		/// This creates an instance based on years, months and days.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the amount of years, may be negative </param>
		/// <param name="months">  the amount of months, may be negative </param>
		/// <param name="days">  the amount of days, may be negative </param>
		/// <returns> the period of years, months and days, not null </returns>
		public static Period Of(int years, int months, int days)
		{
			return Create(years, months, days);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Period} from a temporal amount.
		/// <para>
		/// This obtains a period based on the specified amount.
		/// A {@code TemporalAmount} represents an  amount of time, which may be
		/// date-based or time-based, which this factory extracts to a {@code Period}.
		/// </para>
		/// <para>
		/// The conversion loops around the set of units from the amount and uses
		/// the <seealso cref="ChronoUnit#YEARS YEARS"/>, <seealso cref="ChronoUnit#MONTHS MONTHS"/>
		/// and <seealso cref="ChronoUnit#DAYS DAYS"/> units to create a period.
		/// If any other units are found then an exception is thrown.
		/// </para>
		/// <para>
		/// If the amount is a {@code ChronoPeriod} then it must use the ISO chronology.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amount">  the temporal amount to convert, not null </param>
		/// <returns> the equivalent period, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code Period} </exception>
		/// <exception cref="ArithmeticException"> if the amount of years, months or days exceeds an int </exception>
		public static Period From(TemporalAmount amount)
		{
			if (amount is Period)
			{
				return (Period) amount;
			}
			if (amount is ChronoPeriod)
			{
				if (IsoChronology.INSTANCE.Equals(((ChronoPeriod) amount).Chronology) == chrono.ChronoPeriod_Fields.False)
				{
					throw new DateTimeException("Period requires ISO chronology: " + amount);
				}
			}
			Objects.RequireNonNull(amount, "amount");
			int years = 0;
			int months = 0;
			int days = 0;
			foreach (TemporalUnit unit in amount.Units)
			{
				long unitAmount = amount.Get(unit);
				if (unit == ChronoUnit.YEARS)
				{
					years = Math.ToIntExact(unitAmount);
				}
				else if (unit == ChronoUnit.MONTHS)
				{
					months = Math.ToIntExact(unitAmount);
				}
				else if (unit == ChronoUnit.DAYS)
				{
					days = Math.ToIntExact(unitAmount);
				}
				else
				{
					throw new DateTimeException("Unit must be Years, Months or Days, but was " + unit);
				}
			}
			return Create(years, months, days);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Period} from a text string such as {@code PnYnMnD}.
		/// <para>
		/// This will parse the string produced by {@code toString()} which is
		/// based on the ISO-8601 period formats {@code PnYnMnD} and {@code PnW}.
		/// </para>
		/// <para>
		/// The string starts with an optional sign, denoted by the ASCII negative
		/// or positive symbol. If negative, the whole period is negated.
		/// The ASCII letter "P" is next in upper or lower case.
		/// There are then four sections, each consisting of a number and a suffix.
		/// At least one of the four sections must be present.
		/// The sections have suffixes in ASCII of "Y", "M", "W" and "D" for
		/// years, months, weeks and days, accepted in upper or lower case.
		/// The suffixes must occur in order.
		/// The number part of each section must consist of ASCII digits.
		/// The number may be prefixed by the ASCII negative or positive symbol.
		/// The number must parse to an {@code int}.
		/// </para>
		/// <para>
		/// The leading plus/minus sign, and negative values for other units are
		/// not part of the ISO-8601 standard. In addition, ISO-8601 does not
		/// permit mixing between the {@code PnYnMnD} and {@code PnW} formats.
		/// Any week-based input is multiplied by 7 and treated as a number of days.
		/// </para>
		/// <para>
		/// For example, the following are valid inputs:
		/// <pre>
		///   "P2Y"             -- Period.ofYears(2)
		///   "P3M"             -- Period.ofMonths(3)
		///   "P4W"             -- Period.ofWeeks(4)
		///   "P5D"             -- Period.ofDays(5)
		///   "P1Y2M3D"         -- Period.of(1, 2, 3)
		///   "P1Y2M3W4D"       -- Period.of(1, 2, 25)
		///   "P-1Y2M"          -- Period.of(-1, 2, 0)
		///   "-P1Y2M"          -- Period.of(-1, -2, 0)
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <returns> the parsed period, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed to a period </exception>
		public static Period Parse(CharSequence text)
		{
			Objects.RequireNonNull(text, "text");
			Matcher matcher = PATTERN.Matcher(text);
			if (matcher.Matches())
			{
				int negate = ("-".Equals(matcher.Group(1)) ? - 1 : 1);
				String yearMatch = matcher.Group(2);
				String monthMatch = matcher.Group(3);
				String weekMatch = matcher.Group(4);
				String dayMatch = matcher.Group(5);
				if (yearMatch != null || monthMatch != null || dayMatch != null || weekMatch != null)
				{
					try
					{
						int years = ParseNumber(text, yearMatch, negate);
						int months = ParseNumber(text, monthMatch, negate);
						int weeks = ParseNumber(text, weekMatch, negate);
						int days = ParseNumber(text, dayMatch, negate);
						days = Math.AddExact(days, Math.MultiplyExact(weeks, 7));
						return Create(years, months, days);
					}
					catch (NumberFormatException ex)
					{
						throw new DateTimeParseException("Text cannot be parsed to a Period", text, 0, ex);
					}
				}
			}
			throw new DateTimeParseException("Text cannot be parsed to a Period", text, 0);
		}

		private static int ParseNumber(CharSequence text, String str, int negate)
		{
			if (str == null)
			{
				return 0;
			}
			int val = Convert.ToInt32(str);
			try
			{
				return Math.MultiplyExact(val, negate);
			}
			catch (ArithmeticException ex)
			{
				throw new DateTimeParseException("Text cannot be parsed to a Period", text, 0, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code Period} consisting of the number of years, months,
		/// and days between two dates.
		/// <para>
		/// The start date is included, but the end date is not.
		/// The period is calculated by removing complete months, then calculating
		/// the remaining number of days, adjusting to ensure that both have the same sign.
		/// The number of months is then split into years and months based on a 12 month year.
		/// A month is considered if the end day-of-month is greater than or equal to the start day-of-month.
		/// For example, from {@code 2010-01-15} to {@code 2011-03-18} is one year, two months and three days.
		/// </para>
		/// <para>
		/// The result of this method can be a negative period if the end is before the start.
		/// The negative sign will be the same in each of year, month and day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="startDateInclusive">  the start date, inclusive, not null </param>
		/// <param name="endDateExclusive">  the end date, exclusive, not null </param>
		/// <returns> the period between this date and the end date, not null </returns>
		/// <seealso cref= ChronoLocalDate#until(ChronoLocalDate) </seealso>
		public static Period Between(LocalDate startDateInclusive, LocalDate endDateExclusive)
		{
			return startDateInclusive.Until(endDateExclusive);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="years">  the amount </param>
		/// <param name="months">  the amount </param>
		/// <param name="days">  the amount </param>
		private static Period Create(int years, int months, int days)
		{
			if ((years | months | days) == 0)
			{
				return ZERO;
			}
			return new Period(years, months, days);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="years">  the amount </param>
		/// <param name="months">  the amount </param>
		/// <param name="days">  the amount </param>
		private Period(int years, int months, int days)
		{
			this.Years_Renamed = years;
			this.Months_Renamed = months;
			this.Days_Renamed = days;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the value of the requested unit.
		/// <para>
		/// This returns a value for each of the three supported units,
		/// <seealso cref="ChronoUnit#YEARS YEARS"/>, <seealso cref="ChronoUnit#MONTHS MONTHS"/> and
		/// <seealso cref="ChronoUnit#DAYS DAYS"/>.
		/// All other units throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit"> the {@code TemporalUnit} for which to return the value </param>
		/// <returns> the long value of the unit </returns>
		/// <exception cref="DateTimeException"> if the unit is not supported </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public long Get(TemporalUnit unit)
		{
			if (unit == ChronoUnit.YEARS)
			{
				return Years;
			}
			else if (unit == ChronoUnit.MONTHS)
			{
				return Months;
			}
			else if (unit == ChronoUnit.DAYS)
			{
				return Days;
			}
			else
			{
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
		}

		/// <summary>
		/// Gets the set of units supported by this period.
		/// <para>
		/// The supported units are <seealso cref="ChronoUnit#YEARS YEARS"/>,
		/// <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
		/// They are returned in the order years, months, days.
		/// </para>
		/// <para>
		/// This set can be used in conjunction with <seealso cref="#get(TemporalUnit)"/>
		/// to access the entire state of the period.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a list containing the years, months and days units, not null </returns>
		public IList<TemporalUnit> Units
		{
			get
			{
				return SUPPORTED_UNITS;
			}
		}

		/// <summary>
		/// Gets the chronology of this period, which is the ISO calendar system.
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

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if all three units of this period are zero.
		/// <para>
		/// A zero period has the value zero for the years, months and days units.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this period is zero-length </returns>
		public bool Zero
		{
			get
			{
				return (this == ZERO);
			}
		}

		/// <summary>
		/// Checks if any of the three units of this period are negative.
		/// <para>
		/// This checks whether the years, months or days units are less than zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if any unit of this period is negative </returns>
		public bool Negative
		{
			get
			{
				return Years_Renamed < 0 || Months_Renamed < 0 || Days_Renamed < 0;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the amount of years of this period.
		/// <para>
		/// This returns the years unit.
		/// </para>
		/// <para>
		/// The months unit is not automatically normalized with the years unit.
		/// This means that a period of "15 months" is different to a period
		/// of "1 year and 3 months".
		/// 
		/// </para>
		/// </summary>
		/// <returns> the amount of years of this period, may be negative </returns>
		public int Years
		{
			get
			{
				return Years_Renamed;
			}
		}

		/// <summary>
		/// Gets the amount of months of this period.
		/// <para>
		/// This returns the months unit.
		/// </para>
		/// <para>
		/// The months unit is not automatically normalized with the years unit.
		/// This means that a period of "15 months" is different to a period
		/// of "1 year and 3 months".
		/// 
		/// </para>
		/// </summary>
		/// <returns> the amount of months of this period, may be negative </returns>
		public int Months
		{
			get
			{
				return Months_Renamed;
			}
		}

		/// <summary>
		/// Gets the amount of days of this period.
		/// <para>
		/// This returns the days unit.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the amount of days of this period, may be negative </returns>
		public int Days
		{
			get
			{
				return Days_Renamed;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the specified amount of years.
		/// <para>
		/// This sets the amount of the years unit in a copy of this period.
		/// The months and days units are unaffected.
		/// </para>
		/// <para>
		/// The months unit is not automatically normalized with the years unit.
		/// This means that a period of "15 months" is different to a period
		/// of "1 year and 3 months".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="years">  the years to represent, may be negative </param>
		/// <returns> a {@code Period} based on this period with the requested years, not null </returns>
		public Period WithYears(int years)
		{
			if (years == this.Years_Renamed)
			{
				return this;
			}
			return Create(years, Months_Renamed, Days_Renamed);
		}

		/// <summary>
		/// Returns a copy of this period with the specified amount of months.
		/// <para>
		/// This sets the amount of the months unit in a copy of this period.
		/// The years and days units are unaffected.
		/// </para>
		/// <para>
		/// The months unit is not automatically normalized with the years unit.
		/// This means that a period of "15 months" is different to a period
		/// of "1 year and 3 months".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to represent, may be negative </param>
		/// <returns> a {@code Period} based on this period with the requested months, not null </returns>
		public Period WithMonths(int months)
		{
			if (months == this.Months_Renamed)
			{
				return this;
			}
			return Create(Years_Renamed, months, Days_Renamed);
		}

		/// <summary>
		/// Returns a copy of this period with the specified amount of days.
		/// <para>
		/// This sets the amount of the days unit in a copy of this period.
		/// The years and months units are unaffected.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to represent, may be negative </param>
		/// <returns> a {@code Period} based on this period with the requested days, not null </returns>
		public Period WithDays(int days)
		{
			if (days == this.Days_Renamed)
			{
				return this;
			}
			return Create(Years_Renamed, Months_Renamed, days);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the specified period added.
		/// <para>
		/// This operates separately on the years, months and days.
		/// No normalization is performed.
		/// </para>
		/// <para>
		/// For example, "1 year, 6 months and 3 days" plus "2 years, 2 months and 2 days"
		/// returns "3 years, 8 months and 5 days".
		/// </para>
		/// <para>
		/// The specified amount is typically an instance of {@code Period}.
		/// Other types are interpreted using <seealso cref="Period#from(TemporalAmount)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToAdd">  the amount to add, not null </param>
		/// <returns> a {@code Period} based on this period with the requested period added, not null </returns>
		/// <exception cref="DateTimeException"> if the specified amount has a non-ISO chronology or
		///  contains an invalid unit </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period Plus(TemporalAmount amountToAdd)
		{
			Period isoAmount = Period.From(amountToAdd);
			return Create(Math.AddExact(Years_Renamed, isoAmount.Years_Renamed), Math.AddExact(Months_Renamed, isoAmount.Months_Renamed), Math.AddExact(Days_Renamed, isoAmount.Days_Renamed));
		}

		/// <summary>
		/// Returns a copy of this period with the specified years added.
		/// <para>
		/// This adds the amount to the years unit in a copy of this period.
		/// The months and days units are unaffected.
		/// For example, "1 year, 6 months and 3 days" plus 2 years returns "3 years, 6 months and 3 days".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToAdd">  the years to add, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified years added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period PlusYears(long yearsToAdd)
		{
			if (yearsToAdd == 0)
			{
				return this;
			}
			return Create(Math.ToIntExact(Math.AddExact(Years_Renamed, yearsToAdd)), Months_Renamed, Days_Renamed);
		}

		/// <summary>
		/// Returns a copy of this period with the specified months added.
		/// <para>
		/// This adds the amount to the months unit in a copy of this period.
		/// The years and days units are unaffected.
		/// For example, "1 year, 6 months and 3 days" plus 2 months returns "1 year, 8 months and 3 days".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToAdd">  the months to add, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified months added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period PlusMonths(long monthsToAdd)
		{
			if (monthsToAdd == 0)
			{
				return this;
			}
			return Create(Years_Renamed, Math.ToIntExact(Math.AddExact(Months_Renamed, monthsToAdd)), Days_Renamed);
		}

		/// <summary>
		/// Returns a copy of this period with the specified days added.
		/// <para>
		/// This adds the amount to the days unit in a copy of this period.
		/// The years and months units are unaffected.
		/// For example, "1 year, 6 months and 3 days" plus 2 days returns "1 year, 6 months and 5 days".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToAdd">  the days to add, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified days added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period PlusDays(long daysToAdd)
		{
			if (daysToAdd == 0)
			{
				return this;
			}
			return Create(Years_Renamed, Months_Renamed, Math.ToIntExact(Math.AddExact(Days_Renamed, daysToAdd)));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the specified period subtracted.
		/// <para>
		/// This operates separately on the years, months and days.
		/// No normalization is performed.
		/// </para>
		/// <para>
		/// For example, "1 year, 6 months and 3 days" minus "2 years, 2 months and 2 days"
		/// returns "-1 years, 4 months and 1 day".
		/// </para>
		/// <para>
		/// The specified amount is typically an instance of {@code Period}.
		/// Other types are interpreted using <seealso cref="Period#from(TemporalAmount)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToSubtract">  the amount to subtract, not null </param>
		/// <returns> a {@code Period} based on this period with the requested period subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the specified amount has a non-ISO chronology or
		///  contains an invalid unit </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period Minus(TemporalAmount amountToSubtract)
		{
			Period isoAmount = Period.From(amountToSubtract);
			return Create(Math.SubtractExact(Years_Renamed, isoAmount.Years_Renamed), Math.SubtractExact(Months_Renamed, isoAmount.Months_Renamed), Math.SubtractExact(Days_Renamed, isoAmount.Days_Renamed));
		}

		/// <summary>
		/// Returns a copy of this period with the specified years subtracted.
		/// <para>
		/// This subtracts the amount from the years unit in a copy of this period.
		/// The months and days units are unaffected.
		/// For example, "1 year, 6 months and 3 days" minus 2 years returns "-1 years, 6 months and 3 days".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToSubtract">  the years to subtract, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified years subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period MinusYears(long yearsToSubtract)
		{
			return (yearsToSubtract == Long.MinValue ? PlusYears(Long.MaxValue).PlusYears(1) : PlusYears(-yearsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this period with the specified months subtracted.
		/// <para>
		/// This subtracts the amount from the months unit in a copy of this period.
		/// The years and days units are unaffected.
		/// For example, "1 year, 6 months and 3 days" minus 2 months returns "1 year, 4 months and 3 days".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToSubtract">  the years to subtract, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified months subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period MinusMonths(long monthsToSubtract)
		{
			return (monthsToSubtract == Long.MinValue ? PlusMonths(Long.MaxValue).PlusMonths(1) : PlusMonths(-monthsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this period with the specified days subtracted.
		/// <para>
		/// This subtracts the amount from the days unit in a copy of this period.
		/// The years and months units are unaffected.
		/// For example, "1 year, 6 months and 3 days" minus 2 days returns "1 year, 6 months and 1 day".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToSubtract">  the months to subtract, positive or negative </param>
		/// <returns> a {@code Period} based on this period with the specified days subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period MinusDays(long daysToSubtract)
		{
			return (daysToSubtract == Long.MinValue ? PlusDays(Long.MaxValue).PlusDays(1) : PlusDays(-daysToSubtract));
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a new instance with each element in this period multiplied
		/// by the specified scalar.
		/// <para>
		/// This returns a period with each of the years, months and days units
		/// individually multiplied.
		/// For example, a period of "2 years, -3 months and 4 days" multiplied by
		/// 3 will return "6 years, -9 months and 12 days".
		/// No normalization is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="scalar">  the scalar to multiply by, not null </param>
		/// <returns> a {@code Period} based on this period with the amounts multiplied by the scalar, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period MultipliedBy(int scalar)
		{
			if (this == ZERO || scalar == 1)
			{
				return this;
			}
			return Create(Math.MultiplyExact(Years_Renamed, scalar), Math.MultiplyExact(Months_Renamed, scalar), Math.MultiplyExact(Days_Renamed, scalar));
		}

		/// <summary>
		/// Returns a new instance with each amount in this period negated.
		/// <para>
		/// This returns a period with each of the years, months and days units
		/// individually negated.
		/// For example, a period of "2 years, -3 months and 4 days" will be
		/// negated to "-2 years, 3 months and -4 days".
		/// No normalization is performed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Period} based on this period with the amounts negated, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs, which only happens if
		///  one of the units has the value {@code Long.MIN_VALUE} </exception>
		public Period Negated()
		{
			return MultipliedBy(-1);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the years and months normalized.
		/// <para>
		/// This normalizes the years and months units, leaving the days unit unchanged.
		/// The months unit is adjusted to have an absolute value less than 11,
		/// with the years unit being adjusted to compensate. For example, a period of
		/// "1 Year and 15 months" will be normalized to "2 years and 3 months".
		/// </para>
		/// <para>
		/// The sign of the years and months units will be the same after normalization.
		/// For example, a period of "1 year and -25 months" will be normalized to
		/// "-1 year and -1 month".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Period} based on this period with excess months normalized to years, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Period Normalized()
		{
			long totalMonths = ToTotalMonths();
			long splitYears = totalMonths / 12;
			int splitMonths = (int)(totalMonths % 12); // no overflow
			if (splitYears == Years_Renamed && splitMonths == Months_Renamed)
			{
				return this;
			}
			return Create(Math.ToIntExact(splitYears), splitMonths, Days_Renamed);
		}

		/// <summary>
		/// Gets the total number of months in this period.
		/// <para>
		/// This returns the total number of months in the period by multiplying the
		/// number of years by 12 and adding the number of months.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the total number of months in the period, may be negative </returns>
		public long ToTotalMonths()
		{
			return Years_Renamed * 12L + Months_Renamed; // no overflow
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Adds this period to the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this period added.
		/// If the temporal has a chronology, it must be the ISO chronology.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#plus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisPeriod.addTo(dateTime);
		///   dateTime = dateTime.plus(thisPeriod);
		/// </pre>
		/// </para>
		/// <para>
		/// The calculation operates as follows.
		/// First, the chronology of the temporal is checked to ensure it is ISO chronology or null.
		/// Second, if the months are zero, the years are added if non-zero, otherwise
		/// the combination of years and months is added if non-zero.
		/// Finally, any days are added.
		/// </para>
		/// <para>
		/// This approach ensures that a partial period can be added to a partial date.
		/// For example, a period of years and/or months can be added to a {@code YearMonth},
		/// but a period including days cannot.
		/// The approach also adds years and months together when necessary, which ensures
		/// correct behaviour at the end of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to add </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Temporal AddTo(Temporal temporal)
		{
			ValidateChrono(temporal);
			if (Months_Renamed == 0)
			{
				if (Years_Renamed != 0)
				{
					temporal = temporal.Plus(Years_Renamed, YEARS);
				}
			}
			else
			{
				long totalMonths = ToTotalMonths();
				if (totalMonths != 0)
				{
					temporal = temporal.Plus(totalMonths, MONTHS);
				}
			}
			if (Days_Renamed != 0)
			{
				temporal = temporal.Plus(Days_Renamed, DAYS);
			}
			return temporal;
		}

		/// <summary>
		/// Subtracts this period from the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this period subtracted.
		/// If the temporal has a chronology, it must be the ISO chronology.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#minus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisPeriod.subtractFrom(dateTime);
		///   dateTime = dateTime.minus(thisPeriod);
		/// </pre>
		/// </para>
		/// <para>
		/// The calculation operates as follows.
		/// First, the chronology of the temporal is checked to ensure it is ISO chronology or null.
		/// Second, if the months are zero, the years are subtracted if non-zero, otherwise
		/// the combination of years and months is subtracted if non-zero.
		/// Finally, any days are subtracted.
		/// </para>
		/// <para>
		/// This approach ensures that a partial period can be subtracted from a partial date.
		/// For example, a period of years and/or months can be subtracted from a {@code YearMonth},
		/// but a period including days cannot.
		/// The approach also subtracts years and months together when necessary, which ensures
		/// correct behaviour at the end of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to subtract </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public Temporal SubtractFrom(Temporal temporal)
		{
			ValidateChrono(temporal);
			if (Months_Renamed == 0)
			{
				if (Years_Renamed != 0)
				{
					temporal = temporal.minus(Years_Renamed, YEARS);
				}
			}
			else
			{
				long totalMonths = ToTotalMonths();
				if (totalMonths != 0)
				{
					temporal = temporal.minus(totalMonths, MONTHS);
				}
			}
			if (Days_Renamed != 0)
			{
				temporal = temporal.minus(Days_Renamed, DAYS);
			}
			return temporal;
		}

		/// <summary>
		/// Validates that the temporal has the correct chronology.
		/// </summary>
		private void ValidateChrono(TemporalAccessor temporal)
		{
			Objects.RequireNonNull(temporal, "temporal");
			Chronology temporalChrono = temporal.query(TemporalQueries.Chronology());
			if (temporalChrono != null && IsoChronology.INSTANCE.Equals(temporalChrono) == chrono.ChronoPeriod_Fields.False)
			{
				throw new DateTimeException("Chronology mismatch, expected: ISO, actual: " + temporalChrono.Id);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this period is equal to another period.
		/// <para>
		/// The comparison is based on the type {@code Period} and each of the three amounts.
		/// To be equal, the years, months and days units must be individually equal.
		/// Note that this means that a period of "15 Months" is not equal to a period
		/// of "1 Year and 3 Months".
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other period </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return chrono.ChronoPeriod_Fields.True;
			}
			if (obj is Period)
			{
				Period other = (Period) obj;
				return Years_Renamed == other.Years_Renamed && Months_Renamed == other.Months_Renamed && Days_Renamed == other.Days_Renamed;
			}
			return chrono.ChronoPeriod_Fields.False;
		}

		/// <summary>
		/// A hash code for this period.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Years_Renamed + Integer.RotateLeft(Months_Renamed, 8) + Integer.RotateLeft(Days_Renamed, 16);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this period as a {@code String}, such as {@code P6Y3M1D}.
		/// <para>
		/// The output will be in the ISO-8601 period format.
		/// A zero period will be represented as zero days, 'P0D'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this period, not null </returns>
		public override String ToString()
		{
			if (this == ZERO)
			{
				return "P0D";
			}
			else
			{
				StringBuilder buf = new StringBuilder();
				buf.Append('P');
				if (Years_Renamed != 0)
				{
					buf.Append(Years_Renamed).Append('Y');
				}
				if (Months_Renamed != 0)
				{
					buf.Append(Months_Renamed).Append('M');
				}
				if (Days_Renamed != 0)
				{
					buf.Append(Days_Renamed).Append('D');
				}
				return buf.ToString();
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(14);  // identifies a Period
		///  out.writeInt(years);
		///  out.writeInt(months);
		///  out.writeInt(days);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.PERIOD_TYPE, this);
		}

		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="java.io.InvalidObjectException"> always </exception>
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
			@out.WriteInt(Years_Renamed);
			@out.WriteInt(Months_Renamed);
			@out.WriteInt(Days_Renamed);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Period readExternal(java.io.DataInput in) throws java.io.IOException
		internal static Period ReadExternal(DataInput @in)
		{
			int years = @in.ReadInt();
			int months = @in.ReadInt();
			int days = @in.ReadInt();
			return Period.Of(years, months, days);
		}

	}

}