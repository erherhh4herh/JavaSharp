using System;

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
	/// A month-day in the ISO-8601 calendar system, such as {@code --12-03}.
	/// <para>
	/// {@code MonthDay} is an immutable date-time object that represents the combination
	/// of a month and day-of-month. Any field that can be derived from a month and day,
	/// such as quarter-of-year, can be obtained.
	/// </para>
	/// <para>
	/// This class does not store or represent a year, time or time-zone.
	/// For example, the value "December 3rd" can be stored in a {@code MonthDay}.
	/// </para>
	/// <para>
	/// Since a {@code MonthDay} does not possess a year, the leap day of
	/// February 29th is considered valid.
	/// </para>
	/// <para>
	/// This class implements <seealso cref="TemporalAccessor"/> rather than <seealso cref="Temporal"/>.
	/// This is because it is not possible to define whether February 29th is valid or not
	/// without external information, preventing the implementation of plus/minus.
	/// Related to this, {@code MonthDay} only provides access to query and set the fields
	/// {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH}.
	/// </para>
	/// <para>
	/// The ISO-8601 calendar system is the modern civil calendar system used today
	/// in most of the world. It is equivalent to the proleptic Gregorian calendar
	/// system, in which today's rules for leap years are applied for all time.
	/// For most applications written today, the ISO-8601 rules are entirely suitable.
	/// However, any application that makes use of historical dates, and requires them
	/// to be accurate will find the ISO-8601 approach unsuitable.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code MonthDay} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class MonthDay : TemporalAccessor, TemporalAdjuster, Comparable<MonthDay>
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -939150713474957432L;
		/// <summary>
		/// Parser.
		/// </summary>
		private static readonly DateTimeFormatter PARSER = new DateTimeFormatterBuilder().AppendLiteral("--").AppendValue(MONTH_OF_YEAR, 2).AppendLiteral('-').AppendValue(DAY_OF_MONTH, 2).ToFormatter();

		/// <summary>
		/// The month-of-year, not null.
		/// </summary>
		private readonly int Month_Renamed;
		/// <summary>
		/// The day-of-month.
		/// </summary>
		private readonly int Day;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current month-day from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current month-day.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current month-day using the system clock and default time-zone, not null </returns>
		public static MonthDay Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current month-day from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current month-day.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current month-day using the system clock, not null </returns>
		public static MonthDay Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current month-day from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current month-day.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current month-day, not null </returns>
		public static MonthDay Now(Clock clock)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocalDate now = LocalDate.now(clock);
			LocalDate now = LocalDate.Now(clock); // called once
			return MonthDay.Of(now.Month, now.DayOfMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code MonthDay}.
		/// <para>
		/// The day-of-month must be valid for the month within a leap year.
		/// Hence, for February, day 29 is valid.
		/// </para>
		/// <para>
		/// For example, passing in April and day 31 will throw an exception, as
		/// there can never be April 31st in any year. By contrast, passing in
		/// February 29th is permitted, as that month-day can sometimes be valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to represent, not null </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <returns> the month-day, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month </exception>
		public static MonthDay Of(Month month, int dayOfMonth)
		{
			Objects.RequireNonNull(month, "month");
			DAY_OF_MONTH.checkValidValue(dayOfMonth);
			if (dayOfMonth > month.maxLength())
			{
				throw new DateTimeException("Illegal value for DayOfMonth field, value " + dayOfMonth + " is not valid for month " + month.name());
			}
			return new MonthDay(month.Value, dayOfMonth);
		}

		/// <summary>
		/// Obtains an instance of {@code MonthDay}.
		/// <para>
		/// The day-of-month must be valid for the month within a leap year.
		/// Hence, for month 2 (February), day 29 is valid.
		/// </para>
		/// <para>
		/// For example, passing in month 4 (April) and day 31 will throw an exception, as
		/// there can never be April 31st in any year. By contrast, passing in
		/// February 29th is permitted, as that month-day can sometimes be valid.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		/// <returns> the month-day, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month </exception>
		public static MonthDay Of(int month, int dayOfMonth)
		{
			return Of(Month.of(month), dayOfMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code MonthDay} from a temporal object.
		/// <para>
		/// This obtains a month-day based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code MonthDay}.
		/// </para>
		/// <para>
		/// The conversion extracts the <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_MONTH DAY_OF_MONTH"/> fields.
		/// The extraction is only permitted if the temporal object has an ISO
		/// chronology, or can be converted to a {@code LocalDate}.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code MonthDay::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the month-day, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code MonthDay} </exception>
		public static MonthDay From(TemporalAccessor temporal)
		{
			if (temporal is MonthDay)
			{
				return (MonthDay) temporal;
			}
			try
			{
				if (IsoChronology.INSTANCE.Equals(Chronology.from(temporal)) == false)
				{
					temporal = LocalDate.From(temporal);
				}
				return Of(temporal.get(MONTH_OF_YEAR), temporal.get(DAY_OF_MONTH));
			}
			catch (DateTimeException ex)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain MonthDay from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code MonthDay} from a text string such as {@code --12-03}.
		/// <para>
		/// The string must represent a valid month-day.
		/// The format is {@code --MM-dd}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse such as "--12-03", not null </param>
		/// <returns> the parsed month-day, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static MonthDay Parse(CharSequence text)
		{
			return Parse(text, PARSER);
		}

		/// <summary>
		/// Obtains an instance of {@code MonthDay} from a text string using a specific formatter.
		/// <para>
		/// The text is parsed using the formatter, returning a month-day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the parsed month-day, not null </returns>
		/// <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		public static MonthDay Parse(CharSequence text, DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Parse(text, MonthDay::from);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor, previously validated.
		/// </summary>
		/// <param name="month">  the month-of-year to represent, validated from 1 to 12 </param>
		/// <param name="dayOfMonth">  the day-of-month to represent, validated from 1 to 29-31 </param>
		private MonthDay(int month, int dayOfMonth)
		{
			this.Month_Renamed = month;
			this.Day = dayOfMonth;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if this month-day can be queried for the specified field.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		/// <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The supported fields are:
		/// <ul>
		/// <li>{@code MONTH_OF_YEAR}
		/// <li>{@code YEAR}
		/// </ul>
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
		/// <returns> true if the field is supported on this month-day, false if not </returns>
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				return field == MONTH_OF_YEAR || field == DAY_OF_MONTH;
			}
			return field != temporal.TemporalAccessor_Fields.Null && field.IsSupportedBy(this);
		}

		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This month-day is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return
		/// appropriate range instances.
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
		public override ValueRange temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field == MONTH_OF_YEAR)
			{
				return field.Range();
			}
			else if (field == DAY_OF_MONTH)
			{
				return ValueRange.Of(1, Month.minLength(), Month.maxLength());
			}
			return TemporalAccessor.this.range(field);
		}

		/// <summary>
		/// Gets the value of the specified field from this month-day as an {@code int}.
		/// <para>
		/// This queries this month-day for the value of the specified field.
		/// The returned value will always be within the valid range of values for the field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this month-day.
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
		/// <exception cref="DateTimeException"> if a value for the field cannot be obtained or
		///         the value is outside the range of valid values for the field </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the field is not supported or
		///         the range of values exceeds an {@code int} </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		public override int Get(TemporalField field) // override for Javadoc
		{
			return temporal.TemporalAccessor_Fields.range(field).checkValidIntValue(GetLong(field), field);
		}

		/// <summary>
		/// Gets the value of the specified field from this month-day as a {@code long}.
		/// <para>
		/// This queries this month-day for the value of the specified field.
		/// If it is not possible to return the value, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		/// values based on this month-day.
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
		public long GetLong(TemporalField field)
		{
			if (field is ChronoField)
			{
				switch ((ChronoField) field)
				{
					// alignedDOW and alignedWOM not supported because they cannot be set in with()
					case DAY_OF_MONTH:
						return Day;
					case MONTH_OF_YEAR:
						return Month_Renamed;
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the month-of-year field from 1 to 12.
		/// <para>
		/// This method returns the month as an {@code int} from 1 to 12.
		/// Application code is frequently clearer if the enum <seealso cref="Month"/>
		/// is used by calling <seealso cref="#getMonth()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month-of-year, from 1 to 12 </returns>
		/// <seealso cref= #getMonth() </seealso>
		public int MonthValue
		{
			get
			{
				return Month_Renamed;
			}
		}

		/// <summary>
		/// Gets the month-of-year field using the {@code Month} enum.
		/// <para>
		/// This method returns the enum <seealso cref="Month"/> for the month.
		/// This avoids confusion as to what {@code int} values mean.
		/// If you need access to the primitive {@code int} value then the enum
		/// provides the <seealso cref="Month#getValue() int value"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month-of-year, not null </returns>
		/// <seealso cref= #getMonthValue() </seealso>
		public Month Month
		{
			get
			{
				return Month.of(Month_Renamed);
			}
		}

		/// <summary>
		/// Gets the day-of-month field.
		/// <para>
		/// This method returns the primitive {@code int} value for the day-of-month.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-month, from 1 to 31 </returns>
		public int DayOfMonth
		{
			get
			{
				return Day;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if the year is valid for this month-day.
		/// <para>
		/// This method checks whether this month and day and the input year form
		/// a valid date. This can only return false for February 29th.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to validate </param>
		/// <returns> true if the year is valid for this month-day </returns>
		/// <seealso cref= Year#isValidMonthDay(MonthDay) </seealso>
		public bool IsValidYear(int year)
		{
			return (Day == 29 && Month_Renamed == 2 && Year.IsLeap(year) == false) == false;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this {@code MonthDay} with the month-of-year altered.
		/// <para>
		/// This returns a month-day with the specified month.
		/// If the day-of-month is invalid for the specified month, the day will
		/// be adjusted to the last valid day-of-month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the returned month-day, from 1 (January) to 12 (December) </param>
		/// <returns> a {@code MonthDay} based on this month-day with the requested month, not null </returns>
		/// <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		public MonthDay WithMonth(int month)
		{
			return With(Month.of(month));
		}

		/// <summary>
		/// Returns a copy of this {@code MonthDay} with the month-of-year altered.
		/// <para>
		/// This returns a month-day with the specified month.
		/// If the day-of-month is invalid for the specified month, the day will
		/// be adjusted to the last valid day-of-month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month-of-year to set in the returned month-day, not null </param>
		/// <returns> a {@code MonthDay} based on this month-day with the requested month, not null </returns>
		public MonthDay With(Month month)
		{
			Objects.RequireNonNull(month, "month");
			if (month.Value == this.Month_Renamed)
			{
				return this;
			}
			int day = System.Math.Min(this.Day, month.maxLength());
			return new MonthDay(month.Value, day);
		}

		/// <summary>
		/// Returns a copy of this {@code MonthDay} with the day-of-month altered.
		/// <para>
		/// This returns a month-day with the specified day-of-month.
		/// If the day-of-month is invalid for the month, an exception is thrown.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dayOfMonth">  the day-of-month to set in the return month-day, from 1 to 31 </param>
		/// <returns> a {@code MonthDay} based on this month-day with the requested day, not null </returns>
		/// <exception cref="DateTimeException"> if the day-of-month value is invalid,
		///  or if the day-of-month is invalid for the month </exception>
		public MonthDay WithDayOfMonth(int dayOfMonth)
		{
			if (dayOfMonth == this.Day)
			{
				return this;
			}
			return Of(Month_Renamed, dayOfMonth);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this month-day using the specified query.
		/// <para>
		/// This queries this month-day using the specified query strategy object.
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
		public override R query<R>(TemporalQuery<R> query)
		{
			if (query == TemporalQueries.Chronology())
			{
				return (R) IsoChronology.INSTANCE;
			}
			return TemporalAccessor.this.query(query);
		}

		/// <summary>
		/// Adjusts the specified temporal object to have this month-day.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the month and day-of-month changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// twice, passing <seealso cref="ChronoField#MONTH_OF_YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_MONTH"/> as the fields.
		/// If the specified temporal object does not use the ISO calendar system then
		/// a {@code DateTimeException} is thrown.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisMonthDay.adjustInto(temporal);
		///   temporal = temporal.with(thisMonthDay);
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
		public Temporal AdjustInto(Temporal temporal)
		{
			if (Chronology.from(temporal).Equals(IsoChronology.INSTANCE) == false)
			{
				throw new DateTimeException("Adjustment only supported on ISO date-time");
			}
			temporal = temporal.With(MONTH_OF_YEAR, Month_Renamed);
			return temporal.With(DAY_OF_MONTH, System.Math.Min(temporal.range(DAY_OF_MONTH).Maximum, Day));
		}

		/// <summary>
		/// Formats this month-day using the specified formatter.
		/// <para>
		/// This month-day will be passed to the formatter to produce a string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted month-day string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
		public String Format(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			return formatter.Format(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Combines this month-day with a year to create a {@code LocalDate}.
		/// <para>
		/// This returns a {@code LocalDate} formed from this month-day and the specified year.
		/// </para>
		/// <para>
		/// A month-day of February 29th will be adjusted to February 28th in the resulting
		/// date if the year is not a leap year.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to use, from MIN_YEAR to MAX_YEAR </param>
		/// <returns> the local date formed from this month-day and the specified year, not null </returns>
		/// <exception cref="DateTimeException"> if the year is outside the valid range of years </exception>
		public LocalDate AtYear(int year)
		{
			return LocalDate.Of(year, Month_Renamed, IsValidYear(year) ? Day : 28);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this month-day to another month-day.
		/// <para>
		/// The comparison is based first on value of the month, then on the value of the day.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other month-day to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(MonthDay other)
		{
			int cmp = (Month_Renamed - other.Month_Renamed);
			if (cmp == 0)
			{
				cmp = (Day - other.Day);
			}
			return cmp;
		}

		/// <summary>
		/// Checks if this month-day is after the specified month-day.
		/// </summary>
		/// <param name="other">  the other month-day to compare to, not null </param>
		/// <returns> true if this is after the specified month-day </returns>
		public bool IsAfter(MonthDay other)
		{
			return CompareTo(other) > 0;
		}

		/// <summary>
		/// Checks if this month-day is before the specified month-day.
		/// </summary>
		/// <param name="other">  the other month-day to compare to, not null </param>
		/// <returns> true if this point is before the specified month-day </returns>
		public bool IsBefore(MonthDay other)
		{
			return CompareTo(other) < 0;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this month-day is equal to another month-day.
		/// <para>
		/// The comparison is based on the time-line position of the month-day within a year.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other month-day </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is MonthDay)
			{
				MonthDay other = (MonthDay) obj;
				return Month_Renamed == other.Month_Renamed && Day == other.Day;
			}
			return false;
		}

		/// <summary>
		/// A hash code for this month-day.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return (Month_Renamed << 6) + Day;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this month-day as a {@code String}, such as {@code --12-03}.
		/// <para>
		/// The output will be in the format {@code --MM-dd}:
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this month-day, not null </returns>
		public override String ToString()
		{
			return (new StringBuilder(10)).Append("--").Append(Month_Renamed < 10 ? "0" : "").Append(Month_Renamed).Append(Day < 10 ? "-0" : "-").Append(Day).ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(13);  // identifies a MonthDay
		///  out.writeByte(month);
		///  out.writeByte(day);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.MONTH_DAY_TYPE, this);
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			@out.WriteByte(Month_Renamed);
			@out.WriteByte(Day);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static MonthDay readExternal(java.io.DataInput in) throws java.io.IOException
		internal static MonthDay ReadExternal(DataInput @in)
		{
			sbyte month = @in.ReadByte();
			sbyte day = @in.ReadByte();
			return MonthDay.Of(month, day);
		}

	}

}