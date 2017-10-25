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
 * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
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
	/// A day-of-week, such as 'Tuesday'.
	/// <para>
	/// {@code DayOfWeek} is an enum representing the 7 days of the week -
	/// Monday, Tuesday, Wednesday, Thursday, Friday, Saturday and Sunday.
	/// </para>
	/// <para>
	/// In addition to the textual enum name, each day-of-week has an {@code int} value.
	/// The {@code int} value follows the ISO-8601 standard, from 1 (Monday) to 7 (Sunday).
	/// It is recommended that applications use the enum rather than the {@code int} value
	/// to ensure code clarity.
	/// </para>
	/// <para>
	/// This enum provides access to the localized textual form of the day-of-week.
	/// Some locales also assign different numeric values to the days, declaring
	/// Sunday to have the value 1, however this class provides no support for this.
	/// See <seealso cref="WeekFields"/> for localized week-numbering.
	/// </para>
	/// <para>
	/// <b>Do not use {@code ordinal()} to obtain the numeric representation of {@code DayOfWeek}.
	/// Use {@code getValue()} instead.</b>
	/// </para>
	/// <para>
	/// This enum represents a common concept that is found in many calendar systems.
	/// As such, this enum may be used by any calendar system that has the day-of-week
	/// concept defined exactly equivalent to the ISO calendar system.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum DayOfWeek implements TemporalAccessor, TemporalAdjuster
	public enum DayOfWeek
	{

		/// <summary>
		/// The singleton instance for the day-of-week of Monday.
		/// This has the numeric value of {@code 1}.
		/// </summary>
		MONDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Tuesday.
		/// This has the numeric value of {@code 2}.
		/// </summary>
		TUESDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Wednesday.
		/// This has the numeric value of {@code 3}.
		/// </summary>
		WEDNESDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Thursday.
		/// This has the numeric value of {@code 4}.
		/// </summary>
		THURSDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Friday.
		/// This has the numeric value of {@code 5}.
		/// </summary>
		FRIDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Saturday.
		/// This has the numeric value of {@code 6}.
		/// </summary>
		SATURDAY,
		/// <summary>
		/// The singleton instance for the day-of-week of Sunday.
		/// This has the numeric value of {@code 7}.
		/// </summary>
		SUNDAY
		/// <summary>
		/// Private cache of all the constants.
		/// </summary>
		private static final DayOfWeek[] ENUMS = DayOfWeek.values();

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code DayOfWeek} from an {@code int} value.
		/// <para>
		/// {@code DayOfWeek} is an enum representing the 7 days of the week.
		/// This factory allows the enum to be obtained from the {@code int} value.
		/// The {@code int} value follows the ISO-8601 standard, from 1 (Monday) to 7 (Sunday).
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfWeek">  the day-of-week to represent, from 1 (Monday) to 7 (Sunday) </param>
		/// <returns> the day-of-week singleton, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-week is invalid </exception>
		public static DayOfWeek of(int dayOfWeek)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (dayOfWeek < 1 || dayOfWeek > 7)
			{
				throw new DateTimeException("Invalid value for DayOfWeek: " + dayOfWeek);
			}
			return ENUMS[dayOfWeek - 1];
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code DayOfWeek} from a temporal object.
		/// <para>
		/// This obtains a day-of-week based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code DayOfWeek}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> field.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code DayOfWeek::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the day-of-week, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code DayOfWeek} </exception>
		public static DayOfWeek from(java.time.temporal.TemporalAccessor temporal)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (temporal instanceof DayOfWeek)
			{
				return (DayOfWeek) temporal
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			try
			{
				return = temporal.get(DAY_OF_WEEK)
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			catch (DateTimeException ex)
			{
				throw new DateTimeException("Unable to obtain DayOfWeek from TemporalAccessor: " + temporal + " of type " + temporal.getClass().getName(), ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the day-of-week {@code int} value.
		/// <para>
		/// The values are numbered following the ISO-8601 standard, from 1 (Monday) to 7 (Sunday).
		/// See <seealso cref="java.time.temporal.WeekFields#dayOfWeek()"/> for localized week-numbering.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-week, from 1 (Monday) to 7 (Sunday) </returns>
		public int getValue()
		{
			return ordinal() + 1
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the textual representation, such as 'Mon' or 'Friday'.
		/// <para>
		/// This returns the textual name used to identify the day-of-week,
		/// suitable for presentation to the user.
		/// The parameters control the style of the returned text and the locale.
		/// </para>
		/// <para>
		/// If no textual mapping is found then the <seealso cref="#getValue() numeric value"/> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="style">  the length of the text required, not null </param>
		/// <param name="locale">  the locale to use, not null </param>
		/// <returns> the text value of the day-of-week, not null </returns>
		public String getDisplayName(java.time.format.TextStyle style, java.util.Locale locale)
		{
			return = this
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this day-of-week can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		/// <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> then
		/// this method returns true.
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
		/// <returns> true if the field is supported on this day-of-week, false if not </returns>
		public boolean isSupported(java.time.temporal.TemporalField field)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field instanceof java.time.temporal.ChronoField)
			{
				return field == DAY_OF_WEEK;
			}
			return = this
		}

		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This day-of-week is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> then the
		/// range of the day-of-week, from 1 to 7, will be returned.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the range can be obtained is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to query the range for, not null </param>
		/// <returns> the range of valid values for the field, not null </returns>
		/// <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		public java.time.temporal.ValueRange range(java.time.temporal.TemporalField field)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field == DAY_OF_WEEK)
			{
				return = 
			}
			return = field
		}

		/// <summary>
		/// Gets the value of the specified field from this day-of-week as an {@code int}.
		/// <para>
		/// This queries this day-of-week for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> then the
		/// value of the day-of-week, from 1 to 7, will be returned.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		/// passing {@code this} as the argument. Whether the value can be obtained,
		/// and what the value represents, is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get, not null </param>
		/// <returns> the value for the field, within the valid range of values </returns>
		/// <exception cref="DateTimeException"> if a value for the field cannot be obtained or
		///         the value is outside the range of valid values for the field </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported or
		///         the range of values exceeds an {@code int} </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public int get(java.time.temporal.TemporalField field)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field == DAY_OF_WEEK)
			{
				return = 
			}
			return = field
		}

		/// <summary>
		/// Gets the value of the specified field from this day-of-week as a {@code long}.
		/// <para>
		/// This queries this day-of-week for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#DAY_OF_WEEK DAY_OF_WEEK"/> then the
		/// value of the day-of-week, from 1 to 7, will be returned.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		/// passing {@code this} as the argument. Whether the value can be obtained,
		/// and what the value represents, is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get, not null </param>
		/// <returns> the value for the field </returns>
		/// <exception cref="DateTimeException"> if a value for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public long getLong(java.time.temporal.TemporalField field)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field == DAY_OF_WEEK)
			{
				return = 
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			else if (field instanceof java.time.temporal.ChronoField)
			{
				throw new java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return = this
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the day-of-week that is the specified number of days after this one.
		/// <para>
		/// The calculation rolls around the end of the week from Sunday to Monday.
		/// The specified period may be negative.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to add, positive or negative </param>
		/// <returns> the resulting day-of-week, not null </returns>
		public DayOfWeek plus(long days)
		{
			int = days % 7
			return ENUMS[(ordinal() + (amount + 7)) % 7]
		}

		/// <summary>
		/// Returns the day-of-week that is the specified number of days before this one.
		/// <para>
		/// The calculation rolls around the start of the year from Monday to Sunday.
		/// The specified period may be negative.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="days">  the days to subtract, positive or negative </param>
		/// <returns> the resulting day-of-week, not null </returns>
		public DayOfWeek minus(long days)
		{
			return = -(days % 7)
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this day-of-week using the specified query.
		/// <para>
		/// This queries this day-of-week using the specified query strategy object.
		/// The {@code TemporalQuery} object defines the logic to be used to
		/// obtain the result. Read the documentation of the query to understand
		/// what the result of this method will be.
		/// </para>
		/// <para>
		/// The result of this method is obtained by invoking the
		/// <seealso cref="TemporalQuery#queryFrom(TemporalAccessor)"/> method on the
		/// specified query passing {@code this} as the argument.
		/// 
		/// </para>
		/// </summary>
		/// @param <R> the type of the result </param>
		/// <param name="query">  the query to invoke, not null </param>
		/// <returns> the query result, null may be returned (defined by the query) </returns>
		/// <exception cref="DateTimeException"> if unable to query (defined by the query) </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs (defined by the query) </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R> R query(java.time.temporal.TemporalQuery<R> query)
		public <R> R query(java.time.temporal.TemporalQuery<R> query)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (query == java.time.temporal.TemporalQueries.precision())
			{
				return (R) DAYS
			}
			return = query
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this day-of-week.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the day-of-week changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#DAY_OF_WEEK"/> as the field.
		/// Note that this adjusts forwards or backwards within a Monday to Sunday week.
		/// See <seealso cref="java.time.temporal.WeekFields#dayOfWeek()"/> for localized week start days.
		/// See {@code TemporalAdjuster} for other adjusters with more control,
		/// such as {@code next(MONDAY)}.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisDayOfWeek.adjustInto(temporal);
		///   temporal = temporal.with(thisDayOfWeek);
		/// </pre>
		/// </para>
		/// <para>
		/// For example, given a date that is a Wednesday, the following are output:
		/// <pre>
		///   dateOnWed.with(MONDAY);     // two days earlier
		///   dateOnWed.with(TUESDAY);    // one day earlier
		///   dateOnWed.with(WEDNESDAY);  // same date
		///   dateOnWed.with(THURSDAY);   // one day later
		///   dateOnWed.with(FRIDAY);     // two days later
		///   dateOnWed.with(SATURDAY);   // three days later
		///   dateOnWed.with(SUNDAY);     // four days later
		/// </pre>
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the target object to be adjusted, not null </param>
		/// <returns> the adjusted object, not null </returns>
		/// <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public java.time.temporal.Temporal adjustInto(java.time.temporal.Temporal temporal)
		{
			return temporal.with(DAY_OF_WEEK, getValue());
		}

	}

}