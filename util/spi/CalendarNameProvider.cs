using System.Collections.Generic;

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

namespace java.util.spi
{


	/// <summary>
	/// An abstract class for service providers that provide localized string
	/// representations (display names) of {@code Calendar} field values.
	/// 
	/// <para><a name="calendartypes"><b>Calendar Types</b></a>
	/// 
	/// </para>
	/// <para>Calendar types are used to specify calendar systems for which the {@link
	/// #getDisplayName(String, int, int, int, Locale) getDisplayName} and {@link
	/// #getDisplayNames(String, int, int, Locale) getDisplayNames} methods provide
	/// calendar field value names. See <seealso cref="Calendar#getCalendarType()"/> for details.
	/// 
	/// </para>
	/// <para><b>Calendar Fields</b>
	/// 
	/// </para>
	/// <para>Calendar fields are specified with the constants defined in {@link
	/// Calendar}. The following are calendar-common fields and their values to be
	/// supported for each calendar system.
	/// 
	/// <table style="border-bottom:1px solid" border="1" cellpadding="3" cellspacing="0" summary="Field values">
	///   <tr>
	///     <th>Field</th>
	///     <th>Value</th>
	///     <th>Description</th>
	///   </tr>
	///   <tr>
	///     <td valign="top"><seealso cref="Calendar#MONTH"/></td>
	///     <td valign="top"><seealso cref="Calendar#JANUARY"/> to <seealso cref="Calendar#UNDECIMBER"/></td>
	///     <td>Month numbering is 0-based (e.g., 0 - January, ..., 11 -
	///         December). Some calendar systems have 13 months. Month
	///         names need to be supported in both the formatting and
	///         stand-alone forms if required by the supported locales. If there's
	///         no distinction in the two forms, the same names should be returned
	///         in both of the forms.</td>
	///   </tr>
	///   <tr>
	///     <td valign="top"><seealso cref="Calendar#DAY_OF_WEEK"/></td>
	///     <td valign="top"><seealso cref="Calendar#SUNDAY"/> to <seealso cref="Calendar#SATURDAY"/></td>
	///     <td>Day-of-week numbering is 1-based starting from Sunday (i.e., 1 - Sunday,
	///         ..., 7 - Saturday).</td>
	///   </tr>
	///   <tr>
	///     <td valign="top"><seealso cref="Calendar#AM_PM"/></td>
	///     <td valign="top"><seealso cref="Calendar#AM"/> to <seealso cref="Calendar#PM"/></td>
	///     <td>0 - AM, 1 - PM</td>
	///   </tr>
	/// </table>
	/// 
	/// <p style="margin-top:20px">The following are calendar-specific fields and their values to be supported.
	/// 
	/// <table style="border-bottom:1px solid" border="1" cellpadding="3" cellspacing="0" summary="Calendar type and field values">
	///   <tr>
	///     <th>Calendar Type</th>
	///     <th>Field</th>
	///     <th>Value</th>
	///     <th>Description</th>
	///   </tr>
	///   <tr>
	///     <td rowspan="2" valign="top">{@code "gregory"}</td>
	///     <td rowspan="2" valign="top"><seealso cref="Calendar#ERA"/></td>
	///     <td>0</td>
	///     <td><seealso cref="java.util.GregorianCalendar#BC"/> (BCE)</td>
	///   </tr>
	///   <tr>
	///     <td>1</td>
	///     <td><seealso cref="java.util.GregorianCalendar#AD"/> (CE)</td>
	///   </tr>
	///   <tr>
	///     <td rowspan="2" valign="top">{@code "buddhist"}</td>
	///     <td rowspan="2" valign="top"><seealso cref="Calendar#ERA"/></td>
	///     <td>0</td>
	///     <td>BC (BCE)</td>
	///   </tr>
	///   <tr>
	///     <td>1</td>
	///     <td>B.E. (Buddhist Era)</td>
	///   </tr>
	///   <tr>
	///     <td rowspan="6" valign="top">{@code "japanese"}</td>
	///     <td rowspan="5" valign="top"><seealso cref="Calendar#ERA"/></td>
	///     <td>0</td>
	///     <td>Seireki (Before Meiji)</td>
	///   </tr>
	///   <tr>
	///     <td>1</td>
	///     <td>Meiji</td>
	///   </tr>
	///   <tr>
	///     <td>2</td>
	///     <td>Taisho</td>
	///   </tr>
	///   <tr>
	///     <td>3</td>
	///     <td>Showa</td>
	///   </tr>
	///   <tr>
	///     <td>4</td>
	///     <td >Heisei</td>
	///   </tr>
	///   <tr>
	///     <td><seealso cref="Calendar#YEAR"/></td>
	///     <td>1</td>
	///     <td>the first year in each era. It should be returned when a long
	///     style (<seealso cref="Calendar#LONG_FORMAT"/> or <seealso cref="Calendar#LONG_STANDALONE"/>) is
	///     specified. See also the <a href="../../text/SimpleDateFormat.html#year">
	///     Year representation in {@code SimpleDateFormat}</a>.</td>
	///   </tr>
	///   <tr>
	///     <td rowspan="2" valign="top">{@code "roc"}</td>
	///     <td rowspan="2" valign="top"><seealso cref="Calendar#ERA"/></td>
	///     <td>0</td>
	///     <td>Before R.O.C.</td>
	///   </tr>
	///   <tr>
	///     <td>1</td>
	///     <td>R.O.C.</td>
	///   </tr>
	///   <tr>
	///     <td rowspan="2" valign="top">{@code "islamic"}</td>
	///     <td rowspan="2" valign="top"><seealso cref="Calendar#ERA"/></td>
	///     <td>0</td>
	///     <td>Before AH</td>
	///   </tr>
	///   <tr>
	///     <td>1</td>
	///     <td>Anno Hijrah (AH)</td>
	///   </tr>
	/// </table>
	/// 
	/// </para>
	/// <para>Calendar field value names for {@code "gregory"} must be consistent with
	/// the date-time symbols provided by <seealso cref="java.text.spi.DateFormatSymbolsProvider"/>.
	/// 
	/// </para>
	/// <para>Time zone names are supported by <seealso cref="TimeZoneNameProvider"/>.
	/// 
	/// @author Masayoshi Okutsu
	/// @since 1.8
	/// </para>
	/// </summary>
	/// <seealso cref= CalendarDataProvider </seealso>
	/// <seealso cref= Locale#getUnicodeLocaleType(String) </seealso>
	public abstract class CalendarNameProvider : LocaleServiceProvider
	{
		/// <summary>
		/// Sole constructor. (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal CalendarNameProvider()
		{
		}

		/// <summary>
		/// Returns the string representation (display name) of the calendar
		/// <code>field value</code> in the given <code>style</code> and
		/// <code>locale</code>.  If no string representation is
		/// applicable, <code>null</code> is returned.
		/// 
		/// <para>{@code field} is a {@code Calendar} field index, such as {@link
		/// Calendar#MONTH}. The time zone fields, <seealso cref="Calendar#ZONE_OFFSET"/> and
		/// <seealso cref="Calendar#DST_OFFSET"/>, are <em>not</em> supported by this
		/// method. {@code null} must be returned if any time zone fields are
		/// specified.
		/// 
		/// </para>
		/// <para>{@code value} is the numeric representation of the {@code field} value.
		/// For example, if {@code field} is <seealso cref="Calendar#DAY_OF_WEEK"/>, the valid
		/// values are <seealso cref="Calendar#SUNDAY"/> to <seealso cref="Calendar#SATURDAY"/>
		/// (inclusive).
		/// 
		/// </para>
		/// <para>{@code style} gives the style of the string representation. It is one
		/// of <seealso cref="Calendar#SHORT_FORMAT"/> (<seealso cref="Calendar#SHORT SHORT"/>),
		/// <seealso cref="Calendar#SHORT_STANDALONE"/>, <seealso cref="Calendar#LONG_FORMAT"/>
		/// (<seealso cref="Calendar#LONG LONG"/>), <seealso cref="Calendar#LONG_STANDALONE"/>,
		/// <seealso cref="Calendar#NARROW_FORMAT"/>, or <seealso cref="Calendar#NARROW_STANDALONE"/>.
		/// 
		/// </para>
		/// <para>For example, the following call will return {@code "Sunday"}.
		/// <pre>
		/// getDisplayName("gregory", Calendar.DAY_OF_WEEK, Calendar.SUNDAY,
		///                Calendar.LONG_STANDALONE, Locale.ENGLISH);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="calendarType">
		///              the calendar type. (Any calendar type given by {@code locale}
		///              is ignored.) </param>
		/// <param name="field">
		///              the {@code Calendar} field index,
		///              such as <seealso cref="Calendar#DAY_OF_WEEK"/> </param>
		/// <param name="value">
		///              the value of the {@code Calendar field},
		///              such as <seealso cref="Calendar#MONDAY"/> </param>
		/// <param name="style">
		///              the string representation style: one of {@link
		///              Calendar#SHORT_FORMAT} (<seealso cref="Calendar#SHORT SHORT"/>),
		///              <seealso cref="Calendar#SHORT_STANDALONE"/>, {@link
		///              Calendar#LONG_FORMAT} (<seealso cref="Calendar#LONG LONG"/>),
		///              <seealso cref="Calendar#LONG_STANDALONE"/>,
		///              <seealso cref="Calendar#NARROW_FORMAT"/>,
		///              or <seealso cref="Calendar#NARROW_STANDALONE"/> </param>
		/// <param name="locale">
		///              the desired locale </param>
		/// <returns> the string representation of the {@code field value}, or {@code
		///         null} if the string representation is not applicable or
		///         the given calendar type is unknown </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code field} or {@code style} is invalid </exception>
		/// <exception cref="NullPointerException"> if {@code locale} is {@code null} </exception>
		/// <seealso cref= TimeZoneNameProvider </seealso>
		/// <seealso cref= java.util.Calendar#get(int) </seealso>
		/// <seealso cref= java.util.Calendar#getDisplayName(int, int, Locale) </seealso>
		public abstract String GetDisplayName(String calendarType, int field, int value, int style, Locale locale);

		/// <summary>
		/// Returns a {@code Map} containing all string representations (display
		/// names) of the {@code Calendar} {@code field} in the given {@code style}
		/// and {@code locale} and their corresponding field values.
		/// 
		/// <para>{@code field} is a {@code Calendar} field index, such as {@link
		/// Calendar#MONTH}. The time zone fields, <seealso cref="Calendar#ZONE_OFFSET"/> and
		/// <seealso cref="Calendar#DST_OFFSET"/>, are <em>not</em> supported by this
		/// method. {@code null} must be returned if any time zone fields are specified.
		/// 
		/// </para>
		/// <para>{@code style} gives the style of the string representation. It must be
		/// one of <seealso cref="Calendar#ALL_STYLES"/>, <seealso cref="Calendar#SHORT_FORMAT"/> ({@link
		/// Calendar#SHORT SHORT}), <seealso cref="Calendar#SHORT_STANDALONE"/>, {@link
		/// Calendar#LONG_FORMAT} (<seealso cref="Calendar#LONG LONG"/>), {@link
		/// Calendar#LONG_STANDALONE}, <seealso cref="Calendar#NARROW_FORMAT"/>, or
		/// <seealso cref="Calendar#NARROW_STANDALONE"/>. Note that narrow names may
		/// not be unique due to use of single characters, such as "S" for Sunday
		/// and Saturday, and that no narrow names are included in that case.
		/// 
		/// </para>
		/// <para>For example, the following call will return a {@code Map} containing
		/// {@code "January"} to <seealso cref="Calendar#JANUARY"/>, {@code "Jan"} to {@link
		/// Calendar#JANUARY}, {@code "February"} to <seealso cref="Calendar#FEBRUARY"/>,
		/// {@code "Feb"} to <seealso cref="Calendar#FEBRUARY"/>, and so on.
		/// <pre>
		/// getDisplayNames("gregory", Calendar.MONTH, Calendar.ALL_STYLES, Locale.ENGLISH);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="calendarType">
		///              the calendar type. (Any calendar type given by {@code locale}
		///              is ignored.) </param>
		/// <param name="field">
		///              the calendar field for which the display names are returned </param>
		/// <param name="style">
		///              the style applied to the display names; one of
		///              <seealso cref="Calendar#ALL_STYLES"/>, <seealso cref="Calendar#SHORT_FORMAT"/>
		///              (<seealso cref="Calendar#SHORT SHORT"/>), {@link
		///              Calendar#SHORT_STANDALONE}, <seealso cref="Calendar#LONG_FORMAT"/>
		///              (<seealso cref="Calendar#LONG LONG"/>), <seealso cref="Calendar#LONG_STANDALONE"/>,
		///              <seealso cref="Calendar#NARROW_FORMAT"/>,
		///              or <seealso cref="Calendar#NARROW_STANDALONE"/> </param>
		/// <param name="locale">
		///              the desired locale </param>
		/// <returns> a {@code Map} containing all display names of {@code field} in
		///         {@code style} and {@code locale} and their {@code field} values,
		///         or {@code null} if no display names are defined for {@code field} </returns>
		/// <exception cref="NullPointerException">
		///         if {@code locale} is {@code null} </exception>
		/// <seealso cref= Calendar#getDisplayNames(int, int, Locale) </seealso>
		public abstract IDictionary<String, Integer> GetDisplayNames(String calendarType, int field, int style, Locale locale);
	}

}