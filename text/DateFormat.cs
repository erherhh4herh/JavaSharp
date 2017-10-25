using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - All Rights Reserved
 *
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{

	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool;

	/// <summary>
	/// {@code DateFormat} is an abstract class for date/time formatting subclasses which
	/// formats and parses dates or time in a language-independent manner.
	/// The date/time formatting subclass, such as <seealso cref="SimpleDateFormat"/>, allows for
	/// formatting (i.e., date &rarr; text), parsing (text &rarr; date), and
	/// normalization.  The date is represented as a <code>Date</code> object or
	/// as the milliseconds since January 1, 1970, 00:00:00 GMT.
	/// 
	/// <para>{@code DateFormat} provides many class methods for obtaining default date/time
	/// formatters based on the default or a given locale and a number of formatting
	/// styles. The formatting styles include <seealso cref="#FULL"/>, <seealso cref="#LONG"/>, <seealso cref="#MEDIUM"/>, and <seealso cref="#SHORT"/>. More
	/// detail and examples of using these styles are provided in the method
	/// descriptions.
	/// 
	/// </para>
	/// <para>{@code DateFormat} helps you to format and parse dates for any locale.
	/// Your code can be completely independent of the locale conventions for
	/// months, days of the week, or even the calendar format: lunar vs. solar.
	/// 
	/// </para>
	/// <para>To format a date for the current Locale, use one of the
	/// static factory methods:
	/// <blockquote>
	/// <pre>{@code
	/// myString = DateFormat.getDateInstance().format(myDate);
	/// }</pre>
	/// </blockquote>
	/// </para>
	/// <para>If you are formatting multiple dates, it is
	/// more efficient to get the format and use it multiple times so that
	/// the system doesn't have to fetch the information about the local
	/// language and country conventions multiple times.
	/// <blockquote>
	/// <pre>{@code
	/// DateFormat df = DateFormat.getDateInstance();
	/// for (int i = 0; i < myDate.length; ++i) {
	///     output.println(df.format(myDate[i]) + "; ");
	/// }
	/// }</pre>
	/// </blockquote>
	/// </para>
	/// <para>To format a date for a different Locale, specify it in the
	/// call to <seealso cref="#getDateInstance(int, Locale) getDateInstance()"/>.
	/// <blockquote>
	/// <pre>{@code
	/// DateFormat df = DateFormat.getDateInstance(DateFormat.LONG, Locale.FRANCE);
	/// }</pre>
	/// </blockquote>
	/// </para>
	/// <para>You can use a DateFormat to parse also.
	/// <blockquote>
	/// <pre>{@code
	/// myDate = df.parse(myString);
	/// }</pre>
	/// </blockquote>
	/// </para>
	/// <para>Use {@code getDateInstance} to get the normal date format for that country.
	/// There are other static factory methods available.
	/// Use {@code getTimeInstance} to get the time format for that country.
	/// Use {@code getDateTimeInstance} to get a date and time format. You can pass in
	/// different options to these factory methods to control the length of the
	/// result; from <seealso cref="#SHORT"/> to <seealso cref="#MEDIUM"/> to <seealso cref="#LONG"/> to <seealso cref="#FULL"/>. The exact result depends
	/// on the locale, but generally:
	/// <ul><li><seealso cref="#SHORT"/> is completely numeric, such as {@code 12.13.52} or {@code 3:30pm}
	/// <li><seealso cref="#MEDIUM"/> is longer, such as {@code Jan 12, 1952}
	/// <li><seealso cref="#LONG"/> is longer, such as {@code January 12, 1952} or {@code 3:30:32pm}
	/// <li><seealso cref="#FULL"/> is pretty completely specified, such as
	/// {@code Tuesday, April 12, 1952 AD or 3:30:42pm PST}.
	/// </ul>
	/// 
	/// </para>
	/// <para>You can also set the time zone on the format if you wish.
	/// If you want even more control over the format or parsing,
	/// (or want to give your users more control),
	/// you can try casting the {@code DateFormat} you get from the factory methods
	/// to a <seealso cref="SimpleDateFormat"/>. This will work for the majority
	/// of countries; just remember to put it in a {@code try} block in case you
	/// encounter an unusual one.
	/// 
	/// </para>
	/// <para>You can also use forms of the parse and format methods with
	/// <seealso cref="ParsePosition"/> and <seealso cref="FieldPosition"/> to
	/// allow you to
	/// <ul><li>progressively parse through pieces of a string.
	/// <li>align any particular field, or find out where it is for selection
	/// on the screen.
	/// </ul>
	/// 
	/// <h3><a name="synchronization">Synchronization</a></h3>
	/// 
	/// </para>
	/// <para>
	/// Date formats are not synchronized.
	/// It is recommended to create separate format instances for each thread.
	/// If multiple threads access a format concurrently, it must be synchronized
	/// externally.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          Format </seealso>
	/// <seealso cref=          NumberFormat </seealso>
	/// <seealso cref=          SimpleDateFormat </seealso>
	/// <seealso cref=          java.util.Calendar </seealso>
	/// <seealso cref=          java.util.GregorianCalendar </seealso>
	/// <seealso cref=          java.util.TimeZone
	/// @author       Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>
	public abstract class DateFormat : Format
	{

		/// <summary>
		/// The <seealso cref="Calendar"/> instance used for calculating the date-time fields
		/// and the instant of time. This field is used for both formatting and
		/// parsing.
		/// 
		/// <para>Subclasses should initialize this field to a <seealso cref="Calendar"/>
		/// appropriate for the <seealso cref="Locale"/> associated with this
		/// <code>DateFormat</code>.
		/// @serial
		/// </para>
		/// </summary>
		protected internal DateTime Calendar_Renamed;

		/// <summary>
		/// The number formatter that <code>DateFormat</code> uses to format numbers
		/// in dates and times.  Subclasses should initialize this to a number format
		/// appropriate for the locale associated with this <code>DateFormat</code>.
		/// @serial
		/// </summary>
		protected internal NumberFormat NumberFormat_Renamed;

		/// <summary>
		/// Useful constant for ERA field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int ERA_FIELD = 0;
		/// <summary>
		/// Useful constant for YEAR field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int YEAR_FIELD = 1;
		/// <summary>
		/// Useful constant for MONTH field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int MONTH_FIELD = 2;
		/// <summary>
		/// Useful constant for DATE field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int DATE_FIELD = 3;
		/// <summary>
		/// Useful constant for one-based HOUR_OF_DAY field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// HOUR_OF_DAY1_FIELD is used for the one-based 24-hour clock.
		/// For example, 23:59 + 01:00 results in 24:59.
		/// </summary>
		public const int HOUR_OF_DAY1_FIELD = 4;
		/// <summary>
		/// Useful constant for zero-based HOUR_OF_DAY field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// HOUR_OF_DAY0_FIELD is used for the zero-based 24-hour clock.
		/// For example, 23:59 + 01:00 results in 00:59.
		/// </summary>
		public const int HOUR_OF_DAY0_FIELD = 5;
		/// <summary>
		/// Useful constant for MINUTE field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int MINUTE_FIELD = 6;
		/// <summary>
		/// Useful constant for SECOND field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int SECOND_FIELD = 7;
		/// <summary>
		/// Useful constant for MILLISECOND field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int MILLISECOND_FIELD = 8;
		/// <summary>
		/// Useful constant for DAY_OF_WEEK field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int DAY_OF_WEEK_FIELD = 9;
		/// <summary>
		/// Useful constant for DAY_OF_YEAR field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int DAY_OF_YEAR_FIELD = 10;
		/// <summary>
		/// Useful constant for DAY_OF_WEEK_IN_MONTH field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int DAY_OF_WEEK_IN_MONTH_FIELD = 11;
		/// <summary>
		/// Useful constant for WEEK_OF_YEAR field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int WEEK_OF_YEAR_FIELD = 12;
		/// <summary>
		/// Useful constant for WEEK_OF_MONTH field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int WEEK_OF_MONTH_FIELD = 13;
		/// <summary>
		/// Useful constant for AM_PM field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int AM_PM_FIELD = 14;
		/// <summary>
		/// Useful constant for one-based HOUR field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// HOUR1_FIELD is used for the one-based 12-hour clock.
		/// For example, 11:30 PM + 1 hour results in 12:30 AM.
		/// </summary>
		public const int HOUR1_FIELD = 15;
		/// <summary>
		/// Useful constant for zero-based HOUR field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// HOUR0_FIELD is used for the zero-based 12-hour clock.
		/// For example, 11:30 PM + 1 hour results in 00:30 AM.
		/// </summary>
		public const int HOUR0_FIELD = 16;
		/// <summary>
		/// Useful constant for TIMEZONE field alignment.
		/// Used in FieldPosition of date/time formatting.
		/// </summary>
		public const int TIMEZONE_FIELD = 17;

		// Proclaim serial compatibility with 1.1 FCS
		private const long SerialVersionUID = 7218322306649953788L;

		/// <summary>
		/// Overrides Format.
		/// Formats a time object into a time string. Examples of time objects
		/// are a time value expressed in milliseconds and a Date object. </summary>
		/// <param name="obj"> must be a Number or a Date. </param>
		/// <param name="toAppendTo"> the string buffer for the returning time string. </param>
		/// <returns> the string buffer passed in as toAppendTo, with formatted text appended. </returns>
		/// <param name="fieldPosition"> keeps track of the position of the field
		/// within the returned string.
		/// On input: an alignment field,
		/// if desired. On output: the offsets of the alignment field. For
		/// example, given a time text "1996.07.10 AD at 15:08:56 PDT",
		/// if the given fieldPosition is DateFormat.YEAR_FIELD, the
		/// begin index and end index of fieldPosition will be set to
		/// 0 and 4, respectively.
		/// Notice that if the same time field appears
		/// more than once in a pattern, the fieldPosition will be set for the first
		/// occurrence of that time field. For instance, formatting a Date to
		/// the time string "1 PM PDT (Pacific Daylight Time)" using the pattern
		/// "h a z (zzzz)" and the alignment field DateFormat.TIMEZONE_FIELD,
		/// the begin index and end index of fieldPosition will be set to
		/// 5 and 8, respectively, for the first occurrence of the timezone
		/// pattern character 'z'. </param>
		/// <seealso cref= java.text.Format </seealso>
		public sealed override StringBuffer Format(Object obj, StringBuffer toAppendTo, FieldPosition fieldPosition)
		{
			if (obj is DateTime)
			{
				return Format((DateTime)obj, toAppendTo, fieldPosition);
			}
			else if (obj is Number)
			{
				return Format(new DateTime(((Number)obj).LongValue()), toAppendTo, fieldPosition);
			}
			else
			{
				throw new IllegalArgumentException("Cannot format given Object as a Date");
			}
		}

		/// <summary>
		/// Formats a Date into a date/time string. </summary>
		/// <param name="date"> a Date to be formatted into a date/time string. </param>
		/// <param name="toAppendTo"> the string buffer for the returning date/time string. </param>
		/// <param name="fieldPosition"> keeps track of the position of the field
		/// within the returned string.
		/// On input: an alignment field,
		/// if desired. On output: the offsets of the alignment field. For
		/// example, given a time text "1996.07.10 AD at 15:08:56 PDT",
		/// if the given fieldPosition is DateFormat.YEAR_FIELD, the
		/// begin index and end index of fieldPosition will be set to
		/// 0 and 4, respectively.
		/// Notice that if the same time field appears
		/// more than once in a pattern, the fieldPosition will be set for the first
		/// occurrence of that time field. For instance, formatting a Date to
		/// the time string "1 PM PDT (Pacific Daylight Time)" using the pattern
		/// "h a z (zzzz)" and the alignment field DateFormat.TIMEZONE_FIELD,
		/// the begin index and end index of fieldPosition will be set to
		/// 5 and 8, respectively, for the first occurrence of the timezone
		/// pattern character 'z'. </param>
		/// <returns> the string buffer passed in as toAppendTo, with formatted text appended. </returns>
		public abstract StringBuffer Format(DateTime date, StringBuffer toAppendTo, FieldPosition fieldPosition);

		/// <summary>
		/// Formats a Date into a date/time string. </summary>
		/// <param name="date"> the time value to be formatted into a time string. </param>
		/// <returns> the formatted time string. </returns>
		public String Format(DateTime date)
		{
			return Format(date, new StringBuffer(), DontCareFieldPosition.INSTANCE).ToString();
		}

		/// <summary>
		/// Parses text from the beginning of the given string to produce a date.
		/// The method may not use the entire text of the given string.
		/// <para>
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on date parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		/// <returns> A <code>Date</code> parsed from the string. </returns>
		/// <exception cref="ParseException"> if the beginning of the specified string
		///            cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Date parse(String source) throws ParseException
		public virtual DateTime Parse(String source)
		{
			ParsePosition pos = new ParsePosition(0);
			DateTime result = Parse(source, pos);
			if (pos.Index_Renamed == 0)
			{
				throw new ParseException("Unparseable date: \"" + source + "\"", pos.ErrorIndex_Renamed);
			}
			return result;
		}

		/// <summary>
		/// Parse a date/time string according to the given parse position.  For
		/// example, a time text {@code "07/10/96 4:5 PM, PDT"} will be parsed into a {@code Date}
		/// that is equivalent to {@code Date(837039900000L)}.
		/// 
		/// <para> By default, parsing is lenient: If the input is not in the form used
		/// by this object's format method but can still be parsed as a date, then
		/// the parse succeeds.  Clients may insist on strict adherence to the
		/// format by calling <seealso cref="#setLenient(boolean) setLenient(false)"/>.
		/// 
		/// </para>
		/// <para>This parsing operation uses the <seealso cref="#calendar"/> to produce
		/// a {@code Date}. As a result, the {@code calendar}'s date-time
		/// fields and the {@code TimeZone} value may have been
		/// overwritten, depending on subclass implementations. Any {@code
		/// TimeZone} value that has previously been set by a call to
		/// <seealso cref="#setTimeZone(java.util.TimeZone) setTimeZone"/> may need
		/// to be restored for further operations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">  The date/time string to be parsed
		/// </param>
		/// <param name="pos">   On input, the position at which to start parsing; on
		///              output, the position at which parsing terminated, or the
		///              start position if the parse failed.
		/// </param>
		/// <returns>      A {@code Date}, or {@code null} if the input could not be parsed </returns>
		public abstract DateTime Parse(String source, ParsePosition pos);

		/// <summary>
		/// Parses text from a string to produce a <code>Date</code>.
		/// <para>
		/// The method attempts to parse text starting at the index given by
		/// <code>pos</code>.
		/// If parsing succeeds, then the index of <code>pos</code> is updated
		/// to the index after the last character used (parsing does not necessarily
		/// use all characters up to the end of the string), and the parsed
		/// date is returned. The updated <code>pos</code> can be used to
		/// indicate the starting point for the next call to this method.
		/// If an error occurs, then the index of <code>pos</code> is not
		/// changed, the error index of <code>pos</code> is set to the index of
		/// the character where the error occurred, and null is returned.
		/// </para>
		/// <para>
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on date parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		/// <param name="pos"> A <code>ParsePosition</code> object with index and error
		///            index information as described above. </param>
		/// <returns> A <code>Date</code> parsed from the string. In case of
		///         error, returns null. </returns>
		/// <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		public override Object ParseObject(String source, ParsePosition pos)
		{
			return Parse(source, pos);
		}

		/// <summary>
		/// Constant for full style pattern.
		/// </summary>
		public const int FULL = 0;
		/// <summary>
		/// Constant for long style pattern.
		/// </summary>
		public const int LONG = 1;
		/// <summary>
		/// Constant for medium style pattern.
		/// </summary>
		public const int MEDIUM = 2;
		/// <summary>
		/// Constant for short style pattern.
		/// </summary>
		public const int SHORT = 3;
		/// <summary>
		/// Constant for default style pattern.  Its value is MEDIUM.
		/// </summary>
		public const int DEFAULT = MEDIUM;

		/// <summary>
		/// Gets the time formatter with the default formatting style
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getTimeInstance(int, Locale) getTimeInstance(DEFAULT,
		///     Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a time formatter. </returns>
		public static DateFormat TimeInstance
		{
			get
			{
				return Get(DEFAULT, 0, 1, Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets the time formatter with the given formatting style
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getTimeInstance(int, Locale) getTimeInstance(style,
		///     Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <param name="style"> the given formatting style. For example,
		/// SHORT for "h:mm a" in the US locale. </param>
		/// <returns> a time formatter. </returns>
		public static DateFormat GetTimeInstance(int style)
		{
			return Get(style, 0, 1, Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Gets the time formatter with the given formatting style
		/// for the given locale. </summary>
		/// <param name="style"> the given formatting style. For example,
		/// SHORT for "h:mm a" in the US locale. </param>
		/// <param name="aLocale"> the given locale. </param>
		/// <returns> a time formatter. </returns>
		public static DateFormat GetTimeInstance(int style, Locale aLocale)
		{
			return Get(style, 0, 1, aLocale);
		}

		/// <summary>
		/// Gets the date formatter with the default formatting style
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getDateInstance(int, Locale) getDateInstance(DEFAULT,
		///     Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a date formatter. </returns>
		public static DateFormat DateInstance
		{
			get
			{
				return Get(0, DEFAULT, 2, Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets the date formatter with the given formatting style
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getDateInstance(int, Locale) getDateInstance(style,
		///     Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <param name="style"> the given formatting style. For example,
		/// SHORT for "M/d/yy" in the US locale. </param>
		/// <returns> a date formatter. </returns>
		public static DateFormat GetDateInstance(int style)
		{
			return Get(0, style, 2, Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Gets the date formatter with the given formatting style
		/// for the given locale. </summary>
		/// <param name="style"> the given formatting style. For example,
		/// SHORT for "M/d/yy" in the US locale. </param>
		/// <param name="aLocale"> the given locale. </param>
		/// <returns> a date formatter. </returns>
		public static DateFormat GetDateInstance(int style, Locale aLocale)
		{
			return Get(0, style, 2, aLocale);
		}

		/// <summary>
		/// Gets the date/time formatter with the default formatting style
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getDateTimeInstance(int, int, Locale) getDateTimeInstance(DEFAULT,
		///     DEFAULT, Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a date/time formatter. </returns>
		public static DateFormat DateTimeInstance
		{
			get
			{
				return Get(DEFAULT, DEFAULT, 3, Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets the date/time formatter with the given date and time
		/// formatting styles for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getDateTimeInstance(int, int, Locale) getDateTimeInstance(dateStyle,
		///     timeStyle, Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <param name="dateStyle"> the given date formatting style. For example,
		/// SHORT for "M/d/yy" in the US locale. </param>
		/// <param name="timeStyle"> the given time formatting style. For example,
		/// SHORT for "h:mm a" in the US locale. </param>
		/// <returns> a date/time formatter. </returns>
		public static DateFormat GetDateTimeInstance(int dateStyle, int timeStyle)
		{
			return Get(timeStyle, dateStyle, 3, Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Gets the date/time formatter with the given formatting styles
		/// for the given locale. </summary>
		/// <param name="dateStyle"> the given date formatting style. </param>
		/// <param name="timeStyle"> the given time formatting style. </param>
		/// <param name="aLocale"> the given locale. </param>
		/// <returns> a date/time formatter. </returns>
		public static DateFormat GetDateTimeInstance(int dateStyle, int timeStyle, Locale aLocale)
		{
			return Get(timeStyle, dateStyle, 3, aLocale);
		}

		/// <summary>
		/// Get a default date/time formatter that uses the SHORT style for both the
		/// date and the time.
		/// </summary>
		/// <returns> a date/time formatter </returns>
		public static DateFormat Instance
		{
			get
			{
				return GetDateTimeInstance(SHORT, SHORT);
			}
		}

		/// <summary>
		/// Returns an array of all locales for which the
		/// <code>get*Instance</code> methods of this class can return
		/// localized instances.
		/// The returned array represents the union of locales supported by the Java
		/// runtime and by installed
		/// <seealso cref="java.text.spi.DateFormatProvider DateFormatProvider"/> implementations.
		/// It must contain at least a <code>Locale</code> instance equal to
		/// <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of locales for which localized
		///         <code>DateFormat</code> instances are available. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(DateFormatProvider));
				return pool.AvailableLocales;
			}
		}

		/// <summary>
		/// Set the calendar to be used by this date format.  Initially, the default
		/// calendar for the specified or default locale is used.
		/// 
		/// <para>Any <seealso cref="java.util.TimeZone TimeZone"/> and {@linkplain
		/// #isLenient() leniency} values that have previously been set are
		/// overwritten by {@code newCalendar}'s values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newCalendar"> the new {@code Calendar} to be used by the date format </param>
		public virtual DateTime Calendar
		{
			set
			{
				this.Calendar_Renamed = value;
			}
			get
			{
				return Calendar_Renamed;
			}
		}


		/// <summary>
		/// Allows you to set the number formatter. </summary>
		/// <param name="newNumberFormat"> the given new NumberFormat. </param>
		public virtual NumberFormat NumberFormat
		{
			set
			{
				this.NumberFormat_Renamed = value;
			}
			get
			{
				return NumberFormat_Renamed;
			}
		}


		/// <summary>
		/// Sets the time zone for the calendar of this {@code DateFormat} object.
		/// This method is equivalent to the following call.
		/// <blockquote><pre>{@code
		/// getCalendar().setTimeZone(zone)
		/// }</pre></blockquote>
		/// 
		/// <para>The {@code TimeZone} set by this method is overwritten by a
		/// <seealso cref="#setCalendar(java.util.Calendar) setCalendar"/> call.
		/// 
		/// </para>
		/// <para>The {@code TimeZone} set by this method may be overwritten as
		/// a result of a call to the parse method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone"> the given new time zone. </param>
		public virtual TimeZone TimeZone
		{
			set
			{
				Calendar_Renamed.TimeZone = value;
			}
			get
			{
				return Calendar_Renamed.TimeZone;
			}
		}


		/// <summary>
		/// Specify whether or not date/time parsing is to be lenient.  With
		/// lenient parsing, the parser may use heuristics to interpret inputs that
		/// do not precisely match this object's format.  With strict parsing,
		/// inputs must match this object's format.
		/// 
		/// <para>This method is equivalent to the following call.
		/// <blockquote><pre>{@code
		/// getCalendar().setLenient(lenient)
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// <para>This leniency value is overwritten by a call to {@link
		/// #setCalendar(java.util.Calendar) setCalendar()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="lenient"> when {@code true}, parsing is lenient </param>
		/// <seealso cref= java.util.Calendar#setLenient(boolean) </seealso>
		public virtual bool Lenient
		{
			set
			{
				Calendar_Renamed.Lenient = value;
			}
			get
			{
				return Calendar_Renamed.Lenient;
			}
		}


		/// <summary>
		/// Overrides hashCode
		/// </summary>
		public override int HashCode()
		{
			return NumberFormat_Renamed.HashCode();
			// just enough fields for a reasonable distribution
		}

		/// <summary>
		/// Overrides equals
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			DateFormat other = (DateFormat) obj;
			return (Calendar_Renamed.FirstDayOfWeek == other.Calendar_Renamed.FirstDayOfWeek && Calendar_Renamed.MinimalDaysInFirstWeek == other.Calendar_Renamed.MinimalDaysInFirstWeek && Calendar_Renamed.Lenient == other.Calendar_Renamed.Lenient && Calendar_Renamed.TimeZone.Equals(other.Calendar_Renamed.TimeZone) && NumberFormat_Renamed.Equals(other.NumberFormat_Renamed)); // calendar.equivalentTo(other.calendar) // THIS API DOESN'T EXIST YET!
		}

		/// <summary>
		/// Overrides Cloneable
		/// </summary>
		public override Object Clone()
		{
			DateFormat other = (DateFormat) base.Clone();
			other.Calendar_Renamed = (DateTime) Calendar_Renamed.Clone();
			other.NumberFormat_Renamed = (NumberFormat) NumberFormat_Renamed.Clone();
			return other;
		}

		/// <summary>
		/// Creates a DateFormat with the given time and/or date style in the given
		/// locale. </summary>
		/// <param name="timeStyle"> a value from 0 to 3 indicating the time format,
		/// ignored if flags is 2 </param>
		/// <param name="dateStyle"> a value from 0 to 3 indicating the time format,
		/// ignored if flags is 1 </param>
		/// <param name="flags"> either 1 for a time format, 2 for a date format,
		/// or 3 for a date/time format </param>
		/// <param name="loc"> the locale for the format </param>
		private static DateFormat Get(int timeStyle, int dateStyle, int flags, Locale loc)
		{
			if ((flags & 1) != 0)
			{
				if (timeStyle < 0 || timeStyle > 3)
				{
					throw new IllegalArgumentException("Illegal time style " + timeStyle);
				}
			}
			else
			{
				timeStyle = -1;
			}
			if ((flags & 2) != 0)
			{
				if (dateStyle < 0 || dateStyle > 3)
				{
					throw new IllegalArgumentException("Illegal date style " + dateStyle);
				}
			}
			else
			{
				dateStyle = -1;
			}

			LocaleProviderAdapter adapter = LocaleProviderAdapter.getAdapter(typeof(DateFormatProvider), loc);
			DateFormat dateFormat = Get(adapter, timeStyle, dateStyle, loc);
			if (dateFormat == null)
			{
				dateFormat = Get(LocaleProviderAdapter.forJRE(), timeStyle, dateStyle, loc);
			}
			return dateFormat;
		}

		private static DateFormat Get(LocaleProviderAdapter adapter, int timeStyle, int dateStyle, Locale loc)
		{
			DateFormatProvider provider = adapter.DateFormatProvider;
			DateFormat dateFormat;
			if (timeStyle == -1)
			{
				dateFormat = provider.GetDateInstance(dateStyle, loc);
			}
			else
			{
				if (dateStyle == -1)
				{
					dateFormat = provider.GetTimeInstance(timeStyle, loc);
				}
				else
				{
					dateFormat = provider.GetDateTimeInstance(dateStyle, timeStyle, loc);
				}
			}
			return dateFormat;
		}

		/// <summary>
		/// Create a new date format.
		/// </summary>
		protected internal DateFormat()
		{
		}

		/// <summary>
		/// Defines constants that are used as attribute keys in the
		/// <code>AttributedCharacterIterator</code> returned
		/// from <code>DateFormat.formatToCharacterIterator</code> and as
		/// field identifiers in <code>FieldPosition</code>.
		/// <para>
		/// The class also provides two methods to map
		/// between its constants and the corresponding Calendar constants.
		/// 
		/// @since 1.4
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Calendar </seealso>
		public class Field : Format.Field
		{

			// Proclaim serial compatibility with 1.4 FCS
			internal new const long SerialVersionUID = 7441350119349544720L;

			// table of all instances in this class, used by readResolve
			internal new static readonly IDictionary<String, Field> InstanceMap = new Dictionary<String, Field>(18);
			// Maps from Calendar constant (such as Calendar.ERA) to Field
			// constant (such as Field.ERA).
			internal static readonly Field[] CalendarToFieldMapping = new Field[DateTime.FIELD_COUNT];

			/// <summary>
			/// Calendar field. </summary>
			internal int CalendarField_Renamed;

			/// <summary>
			/// Returns the <code>Field</code> constant that corresponds to
			/// the <code>Calendar</code> constant <code>calendarField</code>.
			/// If there is no direct mapping between the <code>Calendar</code>
			/// constant and a <code>Field</code>, null is returned.
			/// </summary>
			/// <exception cref="IllegalArgumentException"> if <code>calendarField</code> is
			///         not the value of a <code>Calendar</code> field constant. </exception>
			/// <param name="calendarField"> Calendar field constant </param>
			/// <returns> Field instance representing calendarField. </returns>
			/// <seealso cref= java.util.Calendar </seealso>
			public static Field OfCalendarField(int calendarField)
			{
				if (calendarField < 0 || calendarField >= CalendarToFieldMapping.Length)
				{
					throw new IllegalArgumentException("Unknown Calendar constant " + calendarField);
				}
				return CalendarToFieldMapping[calendarField];
			}

			/// <summary>
			/// Creates a <code>Field</code>.
			/// </summary>
			/// <param name="name"> the name of the <code>Field</code> </param>
			/// <param name="calendarField"> the <code>Calendar</code> constant this
			///        <code>Field</code> corresponds to; any value, even one
			///        outside the range of legal <code>Calendar</code> values may
			///        be used, but <code>-1</code> should be used for values
			///        that don't correspond to legal <code>Calendar</code> values </param>
			protected internal Field(String name, int calendarField) : base(name)
			{
				this.CalendarField_Renamed = calendarField;
				if (this.GetType() == typeof(DateFormat.Field))
				{
					InstanceMap[name] = this;
					if (calendarField >= 0)
					{
						// assert(calendarField < Calendar.FIELD_COUNT);
						CalendarToFieldMapping[calendarField] = this;
					}
				}
			}

			/// <summary>
			/// Returns the <code>Calendar</code> field associated with this
			/// attribute. For example, if this represents the hours field of
			/// a <code>Calendar</code>, this would return
			/// <code>Calendar.HOUR</code>. If there is no corresponding
			/// <code>Calendar</code> constant, this will return -1.
			/// </summary>
			/// <returns> Calendar constant for this field </returns>
			/// <seealso cref= java.util.Calendar </seealso>
			public virtual int CalendarField
			{
				get
				{
					return CalendarField_Renamed;
				}
			}

			/// <summary>
			/// Resolves instances being deserialized to the predefined constants.
			/// </summary>
			/// <exception cref="InvalidObjectException"> if the constant could not be
			///         resolved. </exception>
			/// <returns> resolved DateFormat.Field constant </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected Object readResolve() throws java.io.InvalidObjectException
			protected internal override Object ReadResolve()
			{
				if (this.GetType() != typeof(DateFormat.Field))
				{
					throw new InvalidObjectException("subclass didn't correctly implement readResolve");
				}

				Object instance = InstanceMap[Name];
				if (instance != null)
				{
					return instance;
				}
				else
				{
					throw new InvalidObjectException("unknown attribute name");
				}
			}

			//
			// The constants
			//

			/// <summary>
			/// Constant identifying the era field.
			/// </summary>
			public static readonly Field ERA = new Field("era", DateTime.ERA);

			/// <summary>
			/// Constant identifying the year field.
			/// </summary>
			public static readonly Field YEAR = new Field("year", DateTime.YEAR);

			/// <summary>
			/// Constant identifying the month field.
			/// </summary>
			public static readonly Field MONTH = new Field("month", DateTime.MONTH);

			/// <summary>
			/// Constant identifying the day of month field.
			/// </summary>
			public static readonly Field DAY_OF_MONTH = new Field("day of month", DateTime.DAY_OF_MONTH);

			/// <summary>
			/// Constant identifying the hour of day field, where the legal values
			/// are 1 to 24.
			/// </summary>
			public static readonly Field HOUR_OF_DAY1 = new Field("hour of day 1",-1);

			/// <summary>
			/// Constant identifying the hour of day field, where the legal values
			/// are 0 to 23.
			/// </summary>
			public static readonly Field HOUR_OF_DAY0 = new Field("hour of day", DateTime.HOUR_OF_DAY);

			/// <summary>
			/// Constant identifying the minute field.
			/// </summary>
			public static readonly Field MINUTE = new Field("minute", DateTime.MINUTE);

			/// <summary>
			/// Constant identifying the second field.
			/// </summary>
			public static readonly Field SECOND = new Field("second", DateTime.SECOND);

			/// <summary>
			/// Constant identifying the millisecond field.
			/// </summary>
			public static readonly Field MILLISECOND = new Field("millisecond", DateTime.MILLISECOND);

			/// <summary>
			/// Constant identifying the day of week field.
			/// </summary>
			public static readonly Field DAY_OF_WEEK = new Field("day of week", DateTime.DAY_OF_WEEK);

			/// <summary>
			/// Constant identifying the day of year field.
			/// </summary>
			public static readonly Field DAY_OF_YEAR = new Field("day of year", DateTime.DAY_OF_YEAR);

			/// <summary>
			/// Constant identifying the day of week field.
			/// </summary>
			public static readonly Field DAY_OF_WEEK_IN_MONTH = new Field("day of week in month", DateTime.DAY_OF_WEEK_IN_MONTH);

			/// <summary>
			/// Constant identifying the week of year field.
			/// </summary>
			public static readonly Field WEEK_OF_YEAR = new Field("week of year", DateTime.WEEK_OF_YEAR);

			/// <summary>
			/// Constant identifying the week of month field.
			/// </summary>
			public static readonly Field WEEK_OF_MONTH = new Field("week of month", DateTime.WEEK_OF_MONTH);

			/// <summary>
			/// Constant identifying the time of day indicator
			/// (e.g. "a.m." or "p.m.") field.
			/// </summary>
			public static readonly Field AM_PM = new Field("am pm", DateTime.AM_PM);

			/// <summary>
			/// Constant identifying the hour field, where the legal values are
			/// 1 to 12.
			/// </summary>
			public static readonly Field HOUR1 = new Field("hour 1", -1);

			/// <summary>
			/// Constant identifying the hour field, where the legal values are
			/// 0 to 11.
			/// </summary>
			public static readonly Field HOUR0 = new Field("hour", DateTime.HOUR);

			/// <summary>
			/// Constant identifying the time zone field.
			/// </summary>
			public static readonly Field TIME_ZONE = new Field("time zone", -1);
		}
	}

}