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
	/// A month-of-year, such as 'July'.
	/// <para>
	/// {@code Month} is an enum representing the 12 months of the year -
	/// January, February, March, April, May, June, July, August, September, October,
	/// November and December.
	/// </para>
	/// <para>
	/// In addition to the textual enum name, each month-of-year has an {@code int} value.
	/// The {@code int} value follows normal usage and the ISO-8601 standard,
	/// from 1 (January) to 12 (December). It is recommended that applications use the enum
	/// rather than the {@code int} value to ensure code clarity.
	/// </para>
	/// <para>
	/// <b>Do not use {@code ordinal()} to obtain the numeric representation of {@code Month}.
	/// Use {@code getValue()} instead.</b>
	/// </para>
	/// <para>
	/// This enum represents a common concept that is found in many calendar systems.
	/// As such, this enum may be used by any calendar system that has the month-of-year
	/// concept defined exactly equivalent to the ISO-8601 calendar system.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum Month implements TemporalAccessor, TemporalAdjuster
	public enum Month
	{

		/// <summary>
		/// The singleton instance for the month of January with 31 days.
		/// This has the numeric value of {@code 1}.
		/// </summary>
		JANUARY,
		/// <summary>
		/// The singleton instance for the month of February with 28 days, or 29 in a leap year.
		/// This has the numeric value of {@code 2}.
		/// </summary>
		FEBRUARY,
		/// <summary>
		/// The singleton instance for the month of March with 31 days.
		/// This has the numeric value of {@code 3}.
		/// </summary>
		MARCH,
		/// <summary>
		/// The singleton instance for the month of April with 30 days.
		/// This has the numeric value of {@code 4}.
		/// </summary>
		APRIL,
		/// <summary>
		/// The singleton instance for the month of May with 31 days.
		/// This has the numeric value of {@code 5}.
		/// </summary>
		MAY,
		/// <summary>
		/// The singleton instance for the month of June with 30 days.
		/// This has the numeric value of {@code 6}.
		/// </summary>
		JUNE,
		/// <summary>
		/// The singleton instance for the month of July with 31 days.
		/// This has the numeric value of {@code 7}.
		/// </summary>
		JULY,
		/// <summary>
		/// The singleton instance for the month of August with 31 days.
		/// This has the numeric value of {@code 8}.
		/// </summary>
		AUGUST,
		/// <summary>
		/// The singleton instance for the month of September with 30 days.
		/// This has the numeric value of {@code 9}.
		/// </summary>
		SEPTEMBER,
		/// <summary>
		/// The singleton instance for the month of October with 31 days.
		/// This has the numeric value of {@code 10}.
		/// </summary>
		OCTOBER,
		/// <summary>
		/// The singleton instance for the month of November with 30 days.
		/// This has the numeric value of {@code 11}.
		/// </summary>
		NOVEMBER,
		/// <summary>
		/// The singleton instance for the month of December with 31 days.
		/// This has the numeric value of {@code 12}.
		/// </summary>
		DECEMBER
		/// <summary>
		/// Private cache of all the constants.
		/// </summary>
		private static final Month[] ENUMS = Month.values();

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Month} from an {@code int} value.
		/// <para>
		/// {@code Month} is an enum representing the 12 months of the year.
		/// This factory allows the enum to be obtained from the {@code int} value.
		/// The {@code int} value follows the ISO-8601 standard, from 1 (January) to 12 (December).
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <returns> the month-of-year, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year is invalid </exception>
		public static Month of(int month)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (month < 1 || month > 12)
			{
				throw new DateTimeException("Invalid value for MonthOfYear: " + month);
			}
			return ENUMS[month - 1];
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Month} from a temporal object.
		/// <para>
		/// This obtains a month based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code Month}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> field.
		/// The extraction is only permitted if the temporal object has an ISO
		/// chronology, or can be converted to a {@code LocalDate}.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code Month::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the month-of-year, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code Month} </exception>
		public static Month from(java.time.temporal.TemporalAccessor temporal)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (temporal instanceof Month)
			{
				return (Month) temporal
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			try
			{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				if (java.time.chrono.IsoChronology.INSTANCE.equals(java.time.chrono.Chronology.from(temporal)) == false)
				{
					temporal = temporal
				}
				return = temporal.get(MONTH_OF_YEAR)
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			catch (DateTimeException ex)
			{
				throw new DateTimeException("Unable to obtain Month from TemporalAccessor: " + temporal + " of type " + temporal.getClass().getName(), ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the month-of-year {@code int} value.
		/// <para>
		/// The values are numbered following the ISO-8601 standard,
		/// from 1 (January) to 12 (December).
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month-of-year, from 1 (January) to 12 (December) </returns>
		public int getValue()
		{
			return ordinal() + 1
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the textual representation, such as 'Jan' or 'December'.
		/// <para>
		/// This returns the textual name used to identify the month-of-year,
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
		/// <returns> the text value of the month-of-year, not null </returns>
		public String getDisplayName(java.time.format.TextStyle style, java.util.Locale locale)
		{
			return = this
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this month-of-year can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		/// <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> then
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
		/// <returns> true if the field is supported on this month-of-year, false if not </returns>
		public boolean isSupported(java.time.temporal.TemporalField field)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field instanceof java.time.temporal.ChronoField)
			{
				return field == MONTH_OF_YEAR;
			}
			return = this
		}

		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This month is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> then the
		/// range of the month-of-year, from 1 to 12, will be returned.
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
			if (field == MONTH_OF_YEAR)
			{
				return = 
			}
			return = field
		}

		/// <summary>
		/// Gets the value of the specified field from this month-of-year as an {@code int}.
		/// <para>
		/// This queries this month for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> then the
		/// value of the month-of-year, from 1 to 12, will be returned.
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
			if (field == MONTH_OF_YEAR)
			{
				return = 
			}
			return = field
		}

		/// <summary>
		/// Gets the value of the specified field from this month-of-year as a {@code long}.
		/// <para>
		/// This queries this month for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> then the
		/// value of the month-of-year, from 1 to 12, will be returned.
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
			if (field == MONTH_OF_YEAR)
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
		/// Returns the month-of-year that is the specified number of quarters after this one.
		/// <para>
		/// The calculation rolls around the end of the year from December to January.
		/// The specified period may be negative.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to add, positive or negative </param>
		/// <returns> the resulting month, not null </returns>
		public Month plus(long months)
		{
			int = months % 12
			return ENUMS[(ordinal() + (amount + 12)) % 12]
		}

		/// <summary>
		/// Returns the month-of-year that is the specified number of months before this one.
		/// <para>
		/// The calculation rolls around the start of the year from January to December.
		/// The specified period may be negative.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="months">  the months to subtract, positive or negative </param>
		/// <returns> the resulting month, not null </returns>
		public Month minus(long months)
		{
			return = -(months % 12)
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the length of this month in days.
		/// <para>
		/// This takes a flag to determine whether to return the length for a leap year or not.
		/// </para>
		/// <para>
		/// February has 28 days in a standard year and 29 days in a leap year.
		/// April, June, September and November have 30 days.
		/// All other months have 31 days.
		/// 
		/// </para>
		/// </summary>
		/// <param name="leapYear">  true if the length is required for a leap year </param>
		/// <returns> the length of this month in days, from 28 to 31 </returns>
		public int length(boolean leapYear)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			switch (this)
			{
				case FEBRUARY:
					return = leapYear ? 29 : 28
				case APRIL:
				case JUNE:
				case SEPTEMBER:
				case NOVEMBER:
					return 30;
				default:
					return 31;
			}
		}

		/// <summary>
		/// Gets the minimum length of this month in days.
		/// <para>
		/// February has a minimum length of 28 days.
		/// April, June, September and November have 30 days.
		/// All other months have 31 days.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the minimum length of this month in days, from 28 to 31 </returns>
		public int minLength()
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			switch (this)
			{
				case FEBRUARY:
					return 28;
				case APRIL:
				case JUNE:
				case SEPTEMBER:
				case NOVEMBER:
					return 30;
				default:
					return 31;
			}
		}

		/// <summary>
		/// Gets the maximum length of this month in days.
		/// <para>
		/// February has a maximum length of 29 days.
		/// April, June, September and November have 30 days.
		/// All other months have 31 days.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the maximum length of this month in days, from 29 to 31 </returns>
		public int maxLength()
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			switch (this)
			{
				case FEBRUARY:
					return 29;
				case APRIL:
				case JUNE:
				case SEPTEMBER:
				case NOVEMBER:
					return 30;
				default:
					return 31;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the day-of-year corresponding to the first day of this month.
		/// <para>
		/// This returns the day-of-year that this month begins on, using the leap
		/// year flag to determine the length of February.
		/// 
		/// </para>
		/// </summary>
		/// <param name="leapYear">  true if the length is required for a leap year </param>
		/// <returns> the day of year corresponding to the first day of this month, from 1 to 336 </returns>
		public int firstDayOfYear(boolean leapYear)
		{
			int leap = leapYear ? 1 : 0;
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			switch (this)
			{
				case JANUARY:
					return 1;
				case FEBRUARY:
					return 32;
				case MARCH:
					return 60 + leap;
				case APRIL:
					return 91 + leap;
				case MAY:
					return 121 + leap;
				case JUNE:
					return 152 + leap;
				case JULY:
					return 182 + leap;
				case AUGUST:
					return 213 + leap;
				case SEPTEMBER:
					return 244 + leap;
				case OCTOBER:
					return 274 + leap;
				case NOVEMBER:
					return 305 + leap;
				case DECEMBER:
				default:
					return 335 + leap;
			}
		}

		/// <summary>
		/// Gets the month corresponding to the first month of this quarter.
		/// <para>
		/// The year can be divided into four quarters.
		/// This method returns the first month of the quarter for the base month.
		/// January, February and March return January.
		/// April, May and June return April.
		/// July, August and September return July.
		/// October, November and December return October.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the first month of the quarter corresponding to this month, not null </returns>
		public Month firstMonthOfQuarter()
		{
			return ENUMS[(ordinal() / 3) * 3]
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this month-of-year using the specified query.
		/// <para>
		/// This queries this month-of-year using the specified query strategy object.
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
			if (query == java.time.temporal.TemporalQueries.chronology())
			{
				return (R) java.time.chrono.IsoChronology.INSTANCE
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			else if (query == java.time.temporal.TemporalQueries.precision())
			{
				return (R) MONTHS
			}
			return = query
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this month-of-year.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the month-of-year changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// passing <seealso cref="ChronoField#MONTH_OF_YEAR"/> as the field.
		/// If the specified temporal object does not use the ISO calendar system then
		/// a {@code DateTimeException} is thrown.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisMonth.adjustInto(temporal);
		///   temporal = temporal.with(thisMonth);
		/// </pre>
		/// </para>
		/// <para>
		/// For example, given a date in May, the following are output:
		/// <pre>
		///   dateInMay.with(JANUARY);    // four months earlier
		///   dateInMay.with(APRIL);      // one months earlier
		///   dateInMay.with(MAY);        // same date
		///   dateInMay.with(JUNE);       // one month later
		///   dateInMay.with(DECEMBER);   // seven months later
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
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (java.time.chrono.Chronology.from(temporal).equals(java.time.chrono.IsoChronology.INSTANCE) == false)
			{
				throw new DateTimeException("Adjustment only supported on ISO date-time");
			}
			return temporal.with(MONTH_OF_YEAR, getValue());
		}

	}

}