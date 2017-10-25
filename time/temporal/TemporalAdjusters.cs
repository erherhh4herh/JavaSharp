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
 * Copyright (c) 2012-2013, Stephen Colebourne & Michael Nascimento Santos
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



	/// <summary>
	/// Common and useful TemporalAdjusters.
	/// <para>
	/// Adjusters are a key tool for modifying temporal objects.
	/// They exist to externalize the process of adjustment, permitting different
	/// approaches, as per the strategy design pattern.
	/// Examples might be an adjuster that sets the date avoiding weekends, or one that
	/// sets the date to the last day of the month.
	/// </para>
	/// <para>
	/// There are two equivalent ways of using a {@code TemporalAdjuster}.
	/// The first is to invoke the method on the interface directly.
	/// The second is to use <seealso cref="Temporal#with(TemporalAdjuster)"/>:
	/// <pre>
	///   // these two lines are equivalent, but the second approach is recommended
	///   temporal = thisAdjuster.adjustInto(temporal);
	///   temporal = temporal.with(thisAdjuster);
	/// </pre>
	/// It is recommended to use the second approach, {@code with(TemporalAdjuster)},
	/// as it is a lot clearer to read in code.
	/// </para>
	/// <para>
	/// This class contains a standard set of adjusters, available as static methods.
	/// These include:
	/// <ul>
	/// <li>finding the first or last day of the month
	/// <li>finding the first day of next month
	/// <li>finding the first or last day of the year
	/// <li>finding the first day of next year
	/// <li>finding the first or last day-of-week within a month, such as "first Wednesday in June"
	/// <li>finding the next or previous day-of-week, such as "next Thursday"
	/// </ul>
	/// 
	/// @implSpec
	/// All the implementations supplied by the static methods are immutable.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= TemporalAdjuster
	/// @since 1.8 </seealso>
	public sealed class TemporalAdjusters
	{

		/// <summary>
		/// Private constructor since this is a utility class.
		/// </summary>
		private TemporalAdjusters()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a {@code TemporalAdjuster} that wraps a date adjuster.
		/// <para>
		/// The {@code TemporalAdjuster} is based on the low level {@code Temporal} interface.
		/// This method allows an adjustment from {@code LocalDate} to {@code LocalDate}
		/// to be wrapped to match the temporal-based interface.
		/// This is provided for convenience to make user-written adjusters simpler.
		/// </para>
		/// <para>
		/// In general, user-written adjusters should be static constants:
		/// <pre>{@code
		///  static TemporalAdjuster TWO_DAYS_LATER =
		///       TemporalAdjusters.ofDateAdjuster(date -> date.plusDays(2));
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateBasedAdjuster">  the date-based adjuster, not null </param>
		/// <returns> the temporal adjuster wrapping on the date adjuster, not null </returns>
		public static TemporalAdjuster OfDateAdjuster(UnaryOperator<LocalDate> dateBasedAdjuster)
		{
			Objects.RequireNonNull(dateBasedAdjuster, "dateBasedAdjuster");
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) =>
			{
				LocalDate input = LocalDate.From(temporal);
				LocalDate output = dateBasedAdjuster.Apply(input);
				return temporal.with(output);
			};
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the "first day of month" adjuster, which returns a new date set to
		/// the first day of the current month.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2011-01-01.<br>
		/// The input 2011-02-15 will return 2011-02-01.
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  temporal.with(DAY_OF_MONTH, 1);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first day-of-month adjuster, not null </returns>
		public static TemporalAdjuster FirstDayOfMonth()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_MONTH, 1);
		}

		/// <summary>
		/// Returns the "last day of month" adjuster, which returns a new date set to
		/// the last day of the current month.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2011-01-31.<br>
		/// The input 2011-02-15 will return 2011-02-28.<br>
		/// The input 2012-02-15 will return 2012-02-29 (leap year).<br>
		/// The input 2011-04-15 will return 2011-04-30.
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  long lastDay = temporal.range(DAY_OF_MONTH).getMaximum();
		///  temporal.with(DAY_OF_MONTH, lastDay);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the last day-of-month adjuster, not null </returns>
		public static TemporalAdjuster LastDayOfMonth()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_MONTH, temporal.range(DAY_OF_MONTH).Maximum);
		}

		/// <summary>
		/// Returns the "first day of next month" adjuster, which returns a new date set to
		/// the first day of the next month.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2011-02-01.<br>
		/// The input 2011-02-15 will return 2011-03-01.
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  temporal.with(DAY_OF_MONTH, 1).plus(1, MONTHS);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first day of next month adjuster, not null </returns>
		public static TemporalAdjuster FirstDayOfNextMonth()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_MONTH, 1).plus(1, MONTHS);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the "first day of year" adjuster, which returns a new date set to
		/// the first day of the current year.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2011-01-01.<br>
		/// The input 2011-02-15 will return 2011-01-01.<br>
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  temporal.with(DAY_OF_YEAR, 1);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first day-of-year adjuster, not null </returns>
		public static TemporalAdjuster FirstDayOfYear()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_YEAR, 1);
		}

		/// <summary>
		/// Returns the "last day of year" adjuster, which returns a new date set to
		/// the last day of the current year.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2011-12-31.<br>
		/// The input 2011-02-15 will return 2011-12-31.<br>
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  long lastDay = temporal.range(DAY_OF_YEAR).getMaximum();
		///  temporal.with(DAY_OF_YEAR, lastDay);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the last day-of-year adjuster, not null </returns>
		public static TemporalAdjuster LastDayOfYear()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_YEAR, temporal.range(DAY_OF_YEAR).Maximum);
		}

		/// <summary>
		/// Returns the "first day of next year" adjuster, which returns a new date set to
		/// the first day of the next year.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 will return 2012-01-01.
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It is equivalent to:
		/// <pre>
		///  temporal.with(DAY_OF_YEAR, 1).plus(1, YEARS);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first day of next month adjuster, not null </returns>
		public static TemporalAdjuster FirstDayOfNextYear()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) => temporal.with(DAY_OF_YEAR, 1).plus(1, YEARS);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the first in month adjuster, which returns a new date
		/// in the same month with the first matching day-of-week.
		/// This is used for expressions like 'first Tuesday in March'.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-12-15 for (MONDAY) will return 2011-12-05.<br>
		/// The input 2011-12-15 for (FRIDAY) will return 2011-12-02.<br>
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} and {@code DAY_OF_MONTH} fields
		/// and the {@code DAYS} unit, and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week, not null </param>
		/// <returns> the first in month adjuster, not null </returns>
		public static TemporalAdjuster FirstInMonth(DayOfWeek dayOfWeek)
		{
			return TemporalAdjusters.DayOfWeekInMonth(1, dayOfWeek);
		}

		/// <summary>
		/// Returns the last in month adjuster, which returns a new date
		/// in the same month with the last matching day-of-week.
		/// This is used for expressions like 'last Tuesday in March'.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-12-15 for (MONDAY) will return 2011-12-26.<br>
		/// The input 2011-12-15 for (FRIDAY) will return 2011-12-30.<br>
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} and {@code DAY_OF_MONTH} fields
		/// and the {@code DAYS} unit, and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week, not null </param>
		/// <returns> the first in month adjuster, not null </returns>
		public static TemporalAdjuster LastInMonth(DayOfWeek dayOfWeek)
		{
			return TemporalAdjusters.DayOfWeekInMonth(-1, dayOfWeek);
		}

		/// <summary>
		/// Returns the day-of-week in month adjuster, which returns a new date
		/// in the same month with the ordinal day-of-week.
		/// This is used for expressions like the 'second Tuesday in March'.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-12-15 for (1,TUESDAY) will return 2011-12-06.<br>
		/// The input 2011-12-15 for (2,TUESDAY) will return 2011-12-13.<br>
		/// The input 2011-12-15 for (3,TUESDAY) will return 2011-12-20.<br>
		/// The input 2011-12-15 for (4,TUESDAY) will return 2011-12-27.<br>
		/// The input 2011-12-15 for (5,TUESDAY) will return 2012-01-03.<br>
		/// The input 2011-12-15 for (-1,TUESDAY) will return 2011-12-27 (last in month).<br>
		/// The input 2011-12-15 for (-4,TUESDAY) will return 2011-12-06 (3 weeks before last in month).<br>
		/// The input 2011-12-15 for (-5,TUESDAY) will return 2011-11-29 (4 weeks before last in month).<br>
		/// The input 2011-12-15 for (0,TUESDAY) will return 2011-11-29 (last in previous month).<br>
		/// </para>
		/// <para>
		/// For a positive or zero ordinal, the algorithm is equivalent to finding the first
		/// day-of-week that matches within the month and then adding a number of weeks to it.
		/// For a negative ordinal, the algorithm is equivalent to finding the last
		/// day-of-week that matches within the month and then subtracting a number of weeks to it.
		/// The ordinal number of weeks is not validated and is interpreted leniently
		/// according to this algorithm. This definition means that an ordinal of zero finds
		/// the last matching day-of-week in the previous month.
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} and {@code DAY_OF_MONTH} fields
		/// and the {@code DAYS} unit, and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ordinal">  the week within the month, unbounded but typically from -5 to 5 </param>
		/// <param name="dayOfWeek">  the day-of-week, not null </param>
		/// <returns> the day-of-week in month adjuster, not null </returns>
		public static TemporalAdjuster DayOfWeekInMonth(int ordinal, DayOfWeek dayOfWeek)
		{
			Objects.RequireNonNull(dayOfWeek, "dayOfWeek");
			int dowValue = dayOfWeek.Value;
			if (ordinal >= 0)
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return (temporal) =>
				{
					Temporal temp = temporal.with(DAY_OF_MONTH, 1);
					int curDow = temp.get(DAY_OF_WEEK);
					int dowDiff = (dowValue - curDow + 7) % 7;
					dowDiff += (int)((ordinal - 1L) * 7L); // safe from overflow
					return temp.Plus(dowDiff, DAYS);
				};
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return (temporal) =>
				{
					Temporal temp = temporal.with(DAY_OF_MONTH, temporal.range(DAY_OF_MONTH).Maximum);
					int curDow = temp.get(DAY_OF_WEEK);
					int daysDiff = dowValue - curDow;
					daysDiff = (daysDiff == 0 ? 0 : (daysDiff > 0 ? daysDiff - 7 : daysDiff));
					daysDiff -= (int)((-ordinal - 1L) * 7L); // safe from overflow
					return temp.Plus(daysDiff, DAYS);
				};
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the next day-of-week adjuster, which adjusts the date to the
		/// first occurrence of the specified day-of-week after the date being adjusted.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 (a Saturday) for parameter (MONDAY) will return 2011-01-17 (two days later).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (WEDNESDAY) will return 2011-01-19 (four days later).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (SATURDAY) will return 2011-01-22 (seven days later).
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} field and the {@code DAYS} unit,
		/// and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week to move the date to, not null </param>
		/// <returns> the next day-of-week adjuster, not null </returns>
		public static TemporalAdjuster Next(DayOfWeek dayOfWeek)
		{
			int dowValue = dayOfWeek.Value;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) =>
			{
				int calDow = temporal.get(DAY_OF_WEEK);
				int daysDiff = calDow - dowValue;
				return temporal.plus(daysDiff >= 0 ? 7 - daysDiff : -daysDiff, DAYS);
			};
		}

		/// <summary>
		/// Returns the next-or-same day-of-week adjuster, which adjusts the date to the
		/// first occurrence of the specified day-of-week after the date being adjusted
		/// unless it is already on that day in which case the same object is returned.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 (a Saturday) for parameter (MONDAY) will return 2011-01-17 (two days later).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (WEDNESDAY) will return 2011-01-19 (four days later).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (SATURDAY) will return 2011-01-15 (same as input).
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} field and the {@code DAYS} unit,
		/// and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week to check for or move the date to, not null </param>
		/// <returns> the next-or-same day-of-week adjuster, not null </returns>
		public static TemporalAdjuster NextOrSame(DayOfWeek dayOfWeek)
		{
			int dowValue = dayOfWeek.Value;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) =>
			{
				int calDow = temporal.get(DAY_OF_WEEK);
				if (calDow == dowValue)
				{
					return temporal;
				}
				int daysDiff = calDow - dowValue;
				return temporal.plus(daysDiff >= 0 ? 7 - daysDiff : -daysDiff, DAYS);
			};
		}

		/// <summary>
		/// Returns the previous day-of-week adjuster, which adjusts the date to the
		/// first occurrence of the specified day-of-week before the date being adjusted.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 (a Saturday) for parameter (MONDAY) will return 2011-01-10 (five days earlier).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (WEDNESDAY) will return 2011-01-12 (three days earlier).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (SATURDAY) will return 2011-01-08 (seven days earlier).
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} field and the {@code DAYS} unit,
		/// and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week to move the date to, not null </param>
		/// <returns> the previous day-of-week adjuster, not null </returns>
		public static TemporalAdjuster Previous(DayOfWeek dayOfWeek)
		{
			int dowValue = dayOfWeek.Value;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) =>
			{
				int calDow = temporal.get(DAY_OF_WEEK);
				int daysDiff = dowValue - calDow;
				return temporal.minus(daysDiff >= 0 ? 7 - daysDiff : -daysDiff, DAYS);
			};
		}

		/// <summary>
		/// Returns the previous-or-same day-of-week adjuster, which adjusts the date to the
		/// first occurrence of the specified day-of-week before the date being adjusted
		/// unless it is already on that day in which case the same object is returned.
		/// <para>
		/// The ISO calendar system behaves as follows:<br>
		/// The input 2011-01-15 (a Saturday) for parameter (MONDAY) will return 2011-01-10 (five days earlier).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (WEDNESDAY) will return 2011-01-12 (three days earlier).<br>
		/// The input 2011-01-15 (a Saturday) for parameter (SATURDAY) will return 2011-01-15 (same as input).
		/// </para>
		/// <para>
		/// The behavior is suitable for use with most calendar systems.
		/// It uses the {@code DAY_OF_WEEK} field and the {@code DAYS} unit,
		/// and assumes a seven day week.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week to check for or move the date to, not null </param>
		/// <returns> the previous-or-same day-of-week adjuster, not null </returns>
		public static TemporalAdjuster PreviousOrSame(DayOfWeek dayOfWeek)
		{
			int dowValue = dayOfWeek.Value;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return (temporal) =>
			{
				int calDow = temporal.get(DAY_OF_WEEK);
				if (calDow == dowValue)
				{
					return temporal;
				}
				int daysDiff = dowValue - calDow;
				return temporal.minus(daysDiff >= 0 ? 7 - daysDiff : -daysDiff, DAYS);
			};
		}

	}

}