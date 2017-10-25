using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
 * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
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

	using CalendarUtils = sun.util.calendar.CalendarUtils;
	using ZoneInfoFile = sun.util.calendar.ZoneInfoFile;
	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;

	/// <summary>
	/// <code>SimpleDateFormat</code> is a concrete class for formatting and
	/// parsing dates in a locale-sensitive manner. It allows for formatting
	/// (date &rarr; text), parsing (text &rarr; date), and normalization.
	/// 
	/// <para>
	/// <code>SimpleDateFormat</code> allows you to start by choosing
	/// any user-defined patterns for date-time formatting. However, you
	/// are encouraged to create a date-time formatter with either
	/// <code>getTimeInstance</code>, <code>getDateInstance</code>, or
	/// <code>getDateTimeInstance</code> in <code>DateFormat</code>. Each
	/// of these class methods can return a date/time formatter initialized
	/// with a default format pattern. You may modify the format pattern
	/// using the <code>applyPattern</code> methods as desired.
	/// For more information on using these methods, see
	/// <seealso cref="DateFormat"/>.
	/// 
	/// <h3>Date and Time Patterns</h3>
	/// </para>
	/// <para>
	/// Date and time formats are specified by <em>date and time pattern</em>
	/// strings.
	/// Within date and time pattern strings, unquoted letters from
	/// <code>'A'</code> to <code>'Z'</code> and from <code>'a'</code> to
	/// <code>'z'</code> are interpreted as pattern letters representing the
	/// components of a date or time string.
	/// Text can be quoted using single quotes (<code>'</code>) to avoid
	/// interpretation.
	/// <code>"''"</code> represents a single quote.
	/// All other characters are not interpreted; they're simply copied into the
	/// output string during formatting or matched against the input string
	/// during parsing.
	/// </para>
	/// <para>
	/// The following pattern letters are defined (all other characters from
	/// <code>'A'</code> to <code>'Z'</code> and from <code>'a'</code> to
	/// <code>'z'</code> are reserved):
	/// <blockquote>
	/// <table border=0 cellspacing=3 cellpadding=0 summary="Chart shows pattern letters, date/time component, presentation, and examples.">
	///     <tr style="background-color: rgb(204, 204, 255);">
	///         <th align=left>Letter
	///         <th align=left>Date or Time Component
	///         <th align=left>Presentation
	///         <th align=left>Examples
	///     <tr>
	///         <td><code>G</code>
	///         <td>Era designator
	///         <td><a href="#text">Text</a>
	///         <td><code>AD</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>y</code>
	///         <td>Year
	///         <td><a href="#year">Year</a>
	///         <td><code>1996</code>; <code>96</code>
	///     <tr>
	///         <td><code>Y</code>
	///         <td>Week year
	///         <td><a href="#year">Year</a>
	///         <td><code>2009</code>; <code>09</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>M</code>
	///         <td>Month in year (context sensitive)
	///         <td><a href="#month">Month</a>
	///         <td><code>July</code>; <code>Jul</code>; <code>07</code>
	///     <tr>
	///         <td><code>L</code>
	///         <td>Month in year (standalone form)
	///         <td><a href="#month">Month</a>
	///         <td><code>July</code>; <code>Jul</code>; <code>07</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>w</code>
	///         <td>Week in year
	///         <td><a href="#number">Number</a>
	///         <td><code>27</code>
	///     <tr>
	///         <td><code>W</code>
	///         <td>Week in month
	///         <td><a href="#number">Number</a>
	///         <td><code>2</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>D</code>
	///         <td>Day in year
	///         <td><a href="#number">Number</a>
	///         <td><code>189</code>
	///     <tr>
	///         <td><code>d</code>
	///         <td>Day in month
	///         <td><a href="#number">Number</a>
	///         <td><code>10</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>F</code>
	///         <td>Day of week in month
	///         <td><a href="#number">Number</a>
	///         <td><code>2</code>
	///     <tr>
	///         <td><code>E</code>
	///         <td>Day name in week
	///         <td><a href="#text">Text</a>
	///         <td><code>Tuesday</code>; <code>Tue</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>u</code>
	///         <td>Day number of week (1 = Monday, ..., 7 = Sunday)
	///         <td><a href="#number">Number</a>
	///         <td><code>1</code>
	///     <tr>
	///         <td><code>a</code>
	///         <td>Am/pm marker
	///         <td><a href="#text">Text</a>
	///         <td><code>PM</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>H</code>
	///         <td>Hour in day (0-23)
	///         <td><a href="#number">Number</a>
	///         <td><code>0</code>
	///     <tr>
	///         <td><code>k</code>
	///         <td>Hour in day (1-24)
	///         <td><a href="#number">Number</a>
	///         <td><code>24</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>K</code>
	///         <td>Hour in am/pm (0-11)
	///         <td><a href="#number">Number</a>
	///         <td><code>0</code>
	///     <tr>
	///         <td><code>h</code>
	///         <td>Hour in am/pm (1-12)
	///         <td><a href="#number">Number</a>
	///         <td><code>12</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>m</code>
	///         <td>Minute in hour
	///         <td><a href="#number">Number</a>
	///         <td><code>30</code>
	///     <tr>
	///         <td><code>s</code>
	///         <td>Second in minute
	///         <td><a href="#number">Number</a>
	///         <td><code>55</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>S</code>
	///         <td>Millisecond
	///         <td><a href="#number">Number</a>
	///         <td><code>978</code>
	///     <tr>
	///         <td><code>z</code>
	///         <td>Time zone
	///         <td><a href="#timezone">General time zone</a>
	///         <td><code>Pacific Standard Time</code>; <code>PST</code>; <code>GMT-08:00</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>Z</code>
	///         <td>Time zone
	///         <td><a href="#rfc822timezone">RFC 822 time zone</a>
	///         <td><code>-0800</code>
	///     <tr>
	///         <td><code>X</code>
	///         <td>Time zone
	///         <td><a href="#iso8601timezone">ISO 8601 time zone</a>
	///         <td><code>-08</code>; <code>-0800</code>;  <code>-08:00</code>
	/// </table>
	/// </blockquote>
	/// Pattern letters are usually repeated, as their number determines the
	/// exact presentation:
	/// <ul>
	/// <li><strong><a name="text">Text:</a></strong>
	///     For formatting, if the number of pattern letters is 4 or more,
	///     the full form is used; otherwise a short or abbreviated form
	///     is used if available.
	///     For parsing, both forms are accepted, independent of the number
	///     of pattern letters.<br><br></li>
	/// <li><strong><a name="number">Number:</a></strong>
	///     For formatting, the number of pattern letters is the minimum
	///     number of digits, and shorter numbers are zero-padded to this amount.
	///     For parsing, the number of pattern letters is ignored unless
	///     it's needed to separate two adjacent fields.<br><br></li>
	/// <li><strong><a name="year">Year:</a></strong>
	///     If the formatter's <seealso cref="#getCalendar() Calendar"/> is the Gregorian
	///     calendar, the following rules are applied.<br>
	///     <ul>
	///     <li>For formatting, if the number of pattern letters is 2, the year
	///         is truncated to 2 digits; otherwise it is interpreted as a
	///         <a href="#number">number</a>.
	///     <li>For parsing, if the number of pattern letters is more than 2,
	///         the year is interpreted literally, regardless of the number of
	///         digits. So using the pattern "MM/dd/yyyy", "01/11/12" parses to
	///         Jan 11, 12 A.D.
	///     <li>For parsing with the abbreviated year pattern ("y" or "yy"),
	///         <code>SimpleDateFormat</code> must interpret the abbreviated year
	///         relative to some century.  It does this by adjusting dates to be
	///         within 80 years before and 20 years after the time the <code>SimpleDateFormat</code>
	///         instance is created. For example, using a pattern of "MM/dd/yy" and a
	///         <code>SimpleDateFormat</code> instance created on Jan 1, 1997,  the string
	///         "01/11/12" would be interpreted as Jan 11, 2012 while the string "05/04/64"
	///         would be interpreted as May 4, 1964.
	///         During parsing, only strings consisting of exactly two digits, as defined by
	///         <seealso cref="Character#isDigit(char)"/>, will be parsed into the default century.
	///         Any other numeric string, such as a one digit string, a three or more digit
	///         string, or a two digit string that isn't all digits (for example, "-1"), is
	///         interpreted literally.  So "01/02/3" or "01/02/003" are parsed, using the
	///         same pattern, as Jan 2, 3 AD.  Likewise, "01/02/-3" is parsed as Jan 2, 4 BC.
	///     </ul>
	///     Otherwise, calendar system specific forms are applied.
	///     For both formatting and parsing, if the number of pattern
	///     letters is 4 or more, a calendar specific {@linkplain
	///     Calendar#LONG long form} is used. Otherwise, a calendar
	///     specific <seealso cref="Calendar#SHORT short or abbreviated form"/>
	///     is used.<br>
	///     <br>
	///     If week year {@code 'Y'} is specified and the {@linkplain
	///     #getCalendar() calendar} doesn't support any <a
	///     href="../util/GregorianCalendar.html#week_year"> week
	///     years</a>, the calendar year ({@code 'y'}) is used instead. The
	///     support of week years can be tested with a call to {@link
	///     DateFormat#getCalendar() getCalendar()}.{@link
	///     java.util.Calendar#isWeekDateSupported()
	///     isWeekDateSupported()}.<br><br></li>
	/// <li><strong><a name="month">Month:</a></strong>
	///     If the number of pattern letters is 3 or more, the month is
	///     interpreted as <a href="#text">text</a>; otherwise,
	///     it is interpreted as a <a href="#number">number</a>.<br>
	///     <ul>
	///     <li>Letter <em>M</em> produces context-sensitive month names, such as the
	///         embedded form of names. If a {@code DateFormatSymbols} has been set
	///         explicitly with constructor {@link #SimpleDateFormat(String,
	///         DateFormatSymbols)} or method {@link
	///         #setDateFormatSymbols(DateFormatSymbols)}, the month names given by
	///         the {@code DateFormatSymbols} are used.</li>
	///     <li>Letter <em>L</em> produces the standalone form of month names.</li>
	///     </ul>
	///     <br></li>
	/// <li><strong><a name="timezone">General time zone:</a></strong>
	///     Time zones are interpreted as <a href="#text">text</a> if they have
	///     names. For time zones representing a GMT offset value, the
	///     following syntax is used:
	///     <pre>
	///     <a name="GMTOffsetTimeZone"><i>GMTOffsetTimeZone:</i></a>
	///             <code>GMT</code> <i>Sign</i> <i>Hours</i> <code>:</code> <i>Minutes</i>
	///     <i>Sign:</i> one of
	///             <code>+ -</code>
	///     <i>Hours:</i>
	///             <i>Digit</i>
	///             <i>Digit</i> <i>Digit</i>
	///     <i>Minutes:</i>
	///             <i>Digit</i> <i>Digit</i>
	///     <i>Digit:</i> one of
	///             <code>0 1 2 3 4 5 6 7 8 9</code></pre>
	///     <i>Hours</i> must be between 0 and 23, and <i>Minutes</i> must be between
	///     00 and 59. The format is locale independent and digits must be taken
	///     from the Basic Latin block of the Unicode standard.
	/// </para>
	///     <para>For parsing, <a href="#rfc822timezone">RFC 822 time zones</a> are also
	///     accepted.<br><br></li>
	/// <li><strong><a name="rfc822timezone">RFC 822 time zone:</a></strong>
	///     For formatting, the RFC 822 4-digit time zone format is used:
	/// 
	///     <pre>
	///     <i>RFC822TimeZone:</i>
	///             <i>Sign</i> <i>TwoDigitHours</i> <i>Minutes</i>
	///     <i>TwoDigitHours:</i>
	///             <i>Digit Digit</i></pre>
	///     <i>TwoDigitHours</i> must be between 00 and 23. Other definitions
	///     are as for <a href="#timezone">general time zones</a>.
	/// 
	/// </para>
	///     <para>For parsing, <a href="#timezone">general time zones</a> are also
	///     accepted.
	/// <li><strong><a name="iso8601timezone">ISO 8601 Time zone:</a></strong>
	///     The number of pattern letters designates the format for both formatting
	///     and parsing as follows:
	///     <pre>
	///     <i>ISO8601TimeZone:</i>
	///             <i>OneLetterISO8601TimeZone</i>
	///             <i>TwoLetterISO8601TimeZone</i>
	///             <i>ThreeLetterISO8601TimeZone</i>
	///     <i>OneLetterISO8601TimeZone:</i>
	///             <i>Sign</i> <i>TwoDigitHours</i>
	///             {@code Z}
	///     <i>TwoLetterISO8601TimeZone:</i>
	///             <i>Sign</i> <i>TwoDigitHours</i> <i>Minutes</i>
	///             {@code Z}
	///     <i>ThreeLetterISO8601TimeZone:</i>
	///             <i>Sign</i> <i>TwoDigitHours</i> {@code :} <i>Minutes</i>
	///             {@code Z}</pre>
	///     Other definitions are as for <a href="#timezone">general time zones</a> or
	///     <a href="#rfc822timezone">RFC 822 time zones</a>.
	/// 
	/// </para>
	///     <para>For formatting, if the offset value from GMT is 0, {@code "Z"} is
	///     produced. If the number of pattern letters is 1, any fraction of an hour
	///     is ignored. For example, if the pattern is {@code "X"} and the time zone is
	///     {@code "GMT+05:30"}, {@code "+05"} is produced.
	/// 
	/// </para>
	///     <para>For parsing, {@code "Z"} is parsed as the UTC time zone designator.
	///     <a href="#timezone">General time zones</a> are <em>not</em> accepted.
	/// 
	/// </para>
	///     <para>If the number of pattern letters is 4 or more, {@link
	///     IllegalArgumentException} is thrown when constructing a {@code
	///     SimpleDateFormat} or {@link #applyPattern(String) applying a
	///     pattern}.
	/// </ul>
	/// <code>SimpleDateFormat</code> also supports <em>localized date and time
	/// pattern</em> strings. In these strings, the pattern letters described above
	/// may be replaced with other, locale dependent, pattern letters.
	/// <code>SimpleDateFormat</code> does not deal with the localization of text
	/// other than the pattern letters; that's up to the client of the class.
	/// 
	/// <h4>Examples</h4>
	/// 
	/// The following examples show how date and time patterns are interpreted in
	/// the U.S. locale. The given date and time are 2001-07-04 12:08:56 local time
	/// in the U.S. Pacific Time time zone.
	/// <blockquote>
	/// <table border=0 cellspacing=3 cellpadding=0 summary="Examples of date and time patterns interpreted in the U.S. locale">
	///     <tr style="background-color: rgb(204, 204, 255);">
	///         <th align=left>Date and Time Pattern
	///         <th align=left>Result
	///     <tr>
	///         <td><code>"yyyy.MM.dd G 'at' HH:mm:ss z"</code>
	///         <td><code>2001.07.04 AD at 12:08:56 PDT</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>"EEE, MMM d, ''yy"</code>
	///         <td><code>Wed, Jul 4, '01</code>
	///     <tr>
	///         <td><code>"h:mm a"</code>
	///         <td><code>12:08 PM</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>"hh 'o''clock' a, zzzz"</code>
	///         <td><code>12 o'clock PM, Pacific Daylight Time</code>
	///     <tr>
	///         <td><code>"K:mm a, z"</code>
	///         <td><code>0:08 PM, PDT</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>"yyyyy.MMMMM.dd GGG hh:mm aaa"</code>
	///         <td><code>02001.July.04 AD 12:08 PM</code>
	///     <tr>
	///         <td><code>"EEE, d MMM yyyy HH:mm:ss Z"</code>
	///         <td><code>Wed, 4 Jul 2001 12:08:56 -0700</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>"yyMMddHHmmssZ"</code>
	///         <td><code>010704120856-0700</code>
	///     <tr>
	///         <td><code>"yyyy-MM-dd'T'HH:mm:ss.SSSZ"</code>
	///         <td><code>2001-07-04T12:08:56.235-0700</code>
	///     <tr style="background-color: rgb(238, 238, 255);">
	///         <td><code>"yyyy-MM-dd'T'HH:mm:ss.SSSXXX"</code>
	///         <td><code>2001-07-04T12:08:56.235-07:00</code>
	///     <tr>
	///         <td><code>"YYYY-'W'ww-u"</code>
	///         <td><code>2001-W27-3</code>
	/// </table>
	/// </blockquote>
	/// 
	/// <h4><a name="synchronization">Synchronization</a></h4>
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
	/// <seealso cref=          <a href="https://docs.oracle.com/javase/tutorial/i18n/format/simpleDateFormat.html">Java Tutorial</a> </seealso>
	/// <seealso cref=          java.util.Calendar </seealso>
	/// <seealso cref=          java.util.TimeZone </seealso>
	/// <seealso cref=          DateFormat </seealso>
	/// <seealso cref=          DateFormatSymbols
	/// @author       Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>
	public class SimpleDateFormat : DateFormat
	{

		// the official serial version ID which says cryptically
		// which version we're compatible with
		internal const long SerialVersionUID = 4774881970558875024L;

		// the internal serial version which says which version was written
		// - 0 (default) for version up to JDK 1.1.3
		// - 1 for version from JDK 1.1.4, which includes a new field
		internal const int CurrentSerialVersion = 1;

		/// <summary>
		/// The version of the serialized data on the stream.  Possible values:
		/// <ul>
		/// <li><b>0</b> or not present on stream: JDK 1.1.3.  This version
		/// has no <code>defaultCenturyStart</code> on stream.
		/// <li><b>1</b> JDK 1.1.4 or later.  This version adds
		/// <code>defaultCenturyStart</code>.
		/// </ul>
		/// When streaming out this class, the most recent format
		/// and the highest allowable <code>serialVersionOnStream</code>
		/// is written.
		/// @serial
		/// @since JDK1.1.4
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		/// <summary>
		/// The pattern string of this formatter.  This is always a non-localized
		/// pattern.  May not be null.  See class documentation for details.
		/// @serial
		/// </summary>
		private String Pattern;

		/// <summary>
		/// Saved numberFormat and pattern. </summary>
		/// <seealso cref= SimpleDateFormat#checkNegativeNumberExpression </seealso>
		[NonSerialized]
		private NumberFormat OriginalNumberFormat;
		[NonSerialized]
		private String OriginalNumberPattern;

		/// <summary>
		/// The minus sign to be used with format and parse.
		/// </summary>
		[NonSerialized]
		private char MinusSign = '-';

		/// <summary>
		/// True when a negative sign follows a number.
		/// (True as default in Arabic.)
		/// </summary>
		[NonSerialized]
		private bool HasFollowingMinusSign = false;

		/// <summary>
		/// True if standalone form needs to be used.
		/// </summary>
		[NonSerialized]
		private bool ForceStandaloneForm = false;

		/// <summary>
		/// The compiled pattern.
		/// </summary>
		[NonSerialized]
		private char[] CompiledPattern;

		/// <summary>
		/// Tags for the compiled pattern.
		/// </summary>
		private const int TAG_QUOTE_ASCII_CHAR = 100;
		private const int TAG_QUOTE_CHARS = 101;

		/// <summary>
		/// Locale dependent digit zero. </summary>
		/// <seealso cref= #zeroPaddingNumber </seealso>
		/// <seealso cref= java.text.DecimalFormatSymbols#getZeroDigit </seealso>
		[NonSerialized]
		private char ZeroDigit;

		/// <summary>
		/// The symbols used by this formatter for week names, month names,
		/// etc.  May not be null.
		/// @serial </summary>
		/// <seealso cref= java.text.DateFormatSymbols </seealso>
		private DateFormatSymbols FormatData;

		/// <summary>
		/// We map dates with two-digit years into the century starting at
		/// <code>defaultCenturyStart</code>, which may be any date.  May
		/// not be null.
		/// @serial
		/// @since JDK1.1.4
		/// </summary>
		private DateTime DefaultCenturyStart;

		[NonSerialized]
		private int DefaultCenturyStartYear;

		private const int MILLIS_PER_MINUTE = 60 * 1000;

		// For time zones that have no names, use strings GMT+minutes and
		// GMT-minutes. For instance, in France the time zone is GMT+60.
		private const String GMT = "GMT";

		/// <summary>
		/// Cache NumberFormat instances with Locale key.
		/// </summary>
		private static readonly ConcurrentMap<Locale, NumberFormat> CachedNumberFormatData = new ConcurrentDictionary<Locale, NumberFormat>(3);

		/// <summary>
		/// The Locale used to instantiate this
		/// <code>SimpleDateFormat</code>. The value may be null if this object
		/// has been created by an older <code>SimpleDateFormat</code> and
		/// deserialized.
		/// 
		/// @serial
		/// @since 1.6
		/// </summary>
		private Locale Locale;

		/// <summary>
		/// Indicates whether this <code>SimpleDateFormat</code> should use
		/// the DateFormatSymbols. If true, the format and parse methods
		/// use the DateFormatSymbols values. If false, the format and
		/// parse methods call Calendar.getDisplayName or
		/// Calendar.getDisplayNames.
		/// </summary>
		[NonSerialized]
		internal bool UseDateFormatSymbols_Renamed;

		/// <summary>
		/// Constructs a <code>SimpleDateFormat</code> using the default pattern and
		/// date format symbols for the default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <b>Note:</b> This constructor may not support all locales.
		/// For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
		/// class.
		/// </summary>
		public SimpleDateFormat() : this("", Locale.GetDefault(Locale.Category.FORMAT))
		{
			ApplyPatternImpl(LocaleProviderAdapter.ResourceBundleBased.getLocaleResources(Locale).getDateTimePattern(SHORT, SHORT, Calendar_Renamed));
		}

		/// <summary>
		/// Constructs a <code>SimpleDateFormat</code> using the given pattern and
		/// the default date format symbols for the default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <b>Note:</b> This constructor may not support all locales.
		/// For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
		/// class.
		/// <para>This is equivalent to calling
		/// {@link #SimpleDateFormat(String, Locale)
		///     SimpleDateFormat(pattern, Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <param name="pattern"> the pattern describing the date and time format </param>
		/// <exception cref="NullPointerException"> if the given pattern is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		public SimpleDateFormat(String pattern) : this(pattern, Locale.GetDefault(Locale.Category.FORMAT))
		{
		}

		/// <summary>
		/// Constructs a <code>SimpleDateFormat</code> using the given pattern and
		/// the default date format symbols for the given locale.
		/// <b>Note:</b> This constructor may not support all locales.
		/// For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
		/// class.
		/// </summary>
		/// <param name="pattern"> the pattern describing the date and time format </param>
		/// <param name="locale"> the locale whose date format symbols should be used </param>
		/// <exception cref="NullPointerException"> if the given pattern or locale is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		public SimpleDateFormat(String pattern, Locale locale)
		{
			if (pattern == null || locale == null)
			{
				throw new NullPointerException();
			}

			InitializeCalendar(locale);
			this.Pattern = pattern;
			this.FormatData = DateFormatSymbols.GetInstanceRef(locale);
			this.Locale = locale;
			Initialize(locale);
		}

		/// <summary>
		/// Constructs a <code>SimpleDateFormat</code> using the given pattern and
		/// date format symbols.
		/// </summary>
		/// <param name="pattern"> the pattern describing the date and time format </param>
		/// <param name="formatSymbols"> the date format symbols to be used for formatting </param>
		/// <exception cref="NullPointerException"> if the given pattern or formatSymbols is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		public SimpleDateFormat(String pattern, DateFormatSymbols formatSymbols)
		{
			if (pattern == null || formatSymbols == null)
			{
				throw new NullPointerException();
			}

			this.Pattern = pattern;
			this.FormatData = (DateFormatSymbols) formatSymbols.Clone();
			this.Locale = Locale.GetDefault(Locale.Category.FORMAT);
			InitializeCalendar(this.Locale);
			Initialize(this.Locale);
			UseDateFormatSymbols_Renamed = true;
		}

		/* Initialize compiledPattern and numberFormat fields */
		private void Initialize(Locale loc)
		{
			// Verify and compile the given pattern.
			CompiledPattern = Compile(Pattern);

			/* try the cache first */
			NumberFormat_Renamed = CachedNumberFormatData[loc];
			if (NumberFormat_Renamed == null) // cache miss
			{
				NumberFormat_Renamed = NumberFormat.GetIntegerInstance(loc);
				NumberFormat_Renamed.GroupingUsed = false;

				/* update cache */
				CachedNumberFormatData.PutIfAbsent(loc, NumberFormat_Renamed);
			}
			NumberFormat_Renamed = (NumberFormat) NumberFormat_Renamed.Clone();

			InitializeDefaultCentury();
		}

		private void InitializeCalendar(Locale loc)
		{
			if (Calendar_Renamed == null)
			{
				Debug.Assert(loc != null);
				// The format object must be constructed using the symbols for this zone.
				// However, the calendar should use the current default TimeZone.
				// If this is not contained in the locale zone strings, then the zone
				// will be formatted using generic GMT+/-H:MM nomenclature.
				Calendar_Renamed = DateTime.GetInstance(TimeZone.Default, loc);
			}
		}

		/// <summary>
		/// Returns the compiled form of the given pattern. The syntax of
		/// the compiled pattern is:
		/// <blockquote>
		/// CompiledPattern:
		///     EntryList
		/// EntryList:
		///     Entry
		///     EntryList Entry
		/// Entry:
		///     TagField
		///     TagField data
		/// TagField:
		///     Tag Length
		///     TaggedData
		/// Tag:
		///     pattern_char_index
		///     TAG_QUOTE_CHARS
		/// Length:
		///     short_length
		///     long_length
		/// TaggedData:
		///     TAG_QUOTE_ASCII_CHAR ascii_char
		/// 
		/// </blockquote>
		/// 
		/// where `short_length' is an 8-bit unsigned integer between 0 and
		/// 254.  `long_length' is a sequence of an 8-bit integer 255 and a
		/// 32-bit signed integer value which is split into upper and lower
		/// 16-bit fields in two char's. `pattern_char_index' is an 8-bit
		/// integer between 0 and 18. `ascii_char' is an 7-bit ASCII
		/// character value. `data' depends on its Tag value.
		/// <para>
		/// If Length is short_length, Tag and short_length are packed in a
		/// single char, as illustrated below.
		/// <blockquote>
		///     char[0] = (Tag << 8) | short_length;
		/// </blockquote>
		/// 
		/// If Length is long_length, Tag and 255 are packed in the first
		/// char and a 32-bit integer, as illustrated below.
		/// <blockquote>
		///     char[0] = (Tag << 8) | 255;
		///     char[1] = (char) (long_length >>> 16);
		///     char[2] = (char) (long_length & 0xffff);
		/// </blockquote>
		/// </para>
		/// <para>
		/// If Tag is a pattern_char_index, its Length is the number of
		/// pattern characters. For example, if the given pattern is
		/// "yyyy", Tag is 1 and Length is 4, followed by no data.
		/// </para>
		/// <para>
		/// If Tag is TAG_QUOTE_CHARS, its Length is the number of char's
		/// following the TagField. For example, if the given pattern is
		/// "'o''clock'", Length is 7 followed by a char sequence of
		/// <code>o&nbs;'&nbs;c&nbs;l&nbs;o&nbs;c&nbs;k</code>.
		/// </para>
		/// <para>
		/// TAG_QUOTE_ASCII_CHAR is a special tag and has an ASCII
		/// character in place of Length. For example, if the given pattern
		/// is "'o'", the TaggedData entry is
		/// <code>((TAG_QUOTE_ASCII_CHAR&nbs;<<&nbs;8)&nbs;|&nbs;'o')</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the given pattern is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		private char[] Compile(String pattern)
		{
			int length = pattern.Length();
			bool inQuote = false;
			StringBuilder compiledCode = new StringBuilder(length * 2);
			StringBuilder tmpBuffer = null;
			int count = 0, tagcount = 0;
			int lastTag = -1, prevTag = -1;

			for (int i = 0; i < length; i++)
			{
				char c = pattern.CharAt(i);

				if (c == '\'')
				{
					// '' is treated as a single quote regardless of being
					// in a quoted section.
					if ((i + 1) < length)
					{
						c = pattern.CharAt(i + 1);
						if (c == '\'')
						{
							i++;
							if (count != 0)
							{
								Encode(lastTag, count, compiledCode);
								tagcount++;
								prevTag = lastTag;
								lastTag = -1;
								count = 0;
							}
							if (inQuote)
							{
								tmpBuffer.Append(c);
							}
							else
							{
								compiledCode.Append((char)(TAG_QUOTE_ASCII_CHAR << 8 | c));
							}
							continue;
						}
					}
					if (!inQuote)
					{
						if (count != 0)
						{
							Encode(lastTag, count, compiledCode);
							tagcount++;
							prevTag = lastTag;
							lastTag = -1;
							count = 0;
						}
						if (tmpBuffer == null)
						{
							tmpBuffer = new StringBuilder(length);
						}
						else
						{
							tmpBuffer.Length = 0;
						}
						inQuote = true;
					}
					else
					{
						int len = tmpBuffer.Length();
						if (len == 1)
						{
							char ch = tmpBuffer.CharAt(0);
							if (ch < 128)
							{
								compiledCode.Append((char)(TAG_QUOTE_ASCII_CHAR << 8 | ch));
							}
							else
							{
								compiledCode.Append((char)(TAG_QUOTE_CHARS << 8 | 1));
								compiledCode.Append(ch);
							}
						}
						else
						{
							Encode(TAG_QUOTE_CHARS, len, compiledCode);
							compiledCode.Append(tmpBuffer);
						}
						inQuote = false;
					}
					continue;
				}
				if (inQuote)
				{
					tmpBuffer.Append(c);
					continue;
				}
				if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'))
				{
					if (count != 0)
					{
						Encode(lastTag, count, compiledCode);
						tagcount++;
						prevTag = lastTag;
						lastTag = -1;
						count = 0;
					}
					if (c < 128)
					{
						// In most cases, c would be a delimiter, such as ':'.
						compiledCode.Append((char)(TAG_QUOTE_ASCII_CHAR << 8 | c));
					}
					else
					{
						// Take any contiguous non-ASCII alphabet characters and
						// put them in a single TAG_QUOTE_CHARS.
						int j;
						for (j = i + 1; j < length; j++)
						{
							char d = pattern.CharAt(j);
							if (d == '\'' || (d >= 'a' && d <= 'z' || d >= 'A' && d <= 'Z'))
							{
								break;
							}
						}
						compiledCode.Append((char)(TAG_QUOTE_CHARS << 8 | (j - i)));
						for (; i < j; i++)
						{
							compiledCode.Append(pattern.CharAt(i));
						}
						i--;
					}
					continue;
				}

				int tag;
				if ((tag = DateFormatSymbols.PatternChars.IndexOf(c)) == -1)
				{
					throw new IllegalArgumentException("Illegal pattern character " + "'" + c + "'");
				}
				if (lastTag == -1 || lastTag == tag)
				{
					lastTag = tag;
					count++;
					continue;
				}
				Encode(lastTag, count, compiledCode);
				tagcount++;
				prevTag = lastTag;
				lastTag = tag;
				count = 1;
			}

			if (inQuote)
			{
				throw new IllegalArgumentException("Unterminated quote");
			}

			if (count != 0)
			{
				Encode(lastTag, count, compiledCode);
				tagcount++;
				prevTag = lastTag;
			}

			ForceStandaloneForm = (tagcount == 1 && prevTag == PATTERN_MONTH);

			// Copy the compiled pattern to a char array
			int len = compiledCode.Length();
			char[] r = new char[len];
			compiledCode.GetChars(0, len, r, 0);
			return r;
		}

		/// <summary>
		/// Encodes the given tag and length and puts encoded char(s) into buffer.
		/// </summary>
		private static void Encode(int tag, int length, StringBuilder buffer)
		{
			if (tag == PATTERN_ISO_ZONE && length >= 4)
			{
				throw new IllegalArgumentException("invalid ISO 8601 format: length=" + length);
			}
			if (length < 255)
			{
				buffer.Append((char)(tag << 8 | length));
			}
			else
			{
				buffer.Append((char)((tag << 8) | 0xff));
				buffer.Append((char)((int)((uint)length >> 16)));
				buffer.Append((char)(length & 0xffff));
			}
		}

		/* Initialize the fields we use to disambiguate ambiguous years. Separate
		 * so we can call it from readObject().
		 */
		private void InitializeDefaultCentury()
		{
			Calendar_Renamed = DateTime.Now;
			Calendar_Renamed.AddYears(-80);
			ParseAmbiguousDatesAsAfter(Calendar_Renamed.Ticks);
		}

		/* Define one-century window into which to disambiguate dates using
		 * two-digit years.
		 */
		private void ParseAmbiguousDatesAsAfter(DateTime startDate)
		{
			DefaultCenturyStart = startDate;
			Calendar_Renamed = new DateTime(startDate);
			DefaultCenturyStartYear = Calendar_Renamed.Year;
		}

		/// <summary>
		/// Sets the 100-year period 2-digit years will be interpreted as being in
		/// to begin on the date the user specifies.
		/// </summary>
		/// <param name="startDate"> During parsing, two digit years will be placed in the range
		/// <code>startDate</code> to <code>startDate + 100 years</code>. </param>
		/// <seealso cref= #get2DigitYearStart
		/// @since 1.2 </seealso>
		public virtual void Set2DigitYearStart(DateTime startDate)
		{
			ParseAmbiguousDatesAsAfter(new DateTime(startDate.Ticks));
		}

		/// <summary>
		/// Returns the beginning date of the 100-year period 2-digit years are interpreted
		/// as being within.
		/// </summary>
		/// <returns> the start of the 100-year period into which two digit years are
		/// parsed </returns>
		/// <seealso cref= #set2DigitYearStart
		/// @since 1.2 </seealso>
		public virtual DateTime Get2DigitYearStart()
		{
			return (DateTime) DefaultCenturyStart.Clone();
		}

		/// <summary>
		/// Formats the given <code>Date</code> into a date/time string and appends
		/// the result to the given <code>StringBuffer</code>.
		/// </summary>
		/// <param name="date"> the date-time value to be formatted into a date-time string. </param>
		/// <param name="toAppendTo"> where the new date-time text is to be appended. </param>
		/// <param name="pos"> the formatting position. On input: an alignment field,
		/// if desired. On output: the offsets of the alignment field. </param>
		/// <returns> the formatted date-time string. </returns>
		/// <exception cref="NullPointerException"> if the given {@code date} is {@code null}. </exception>
		public override StringBuffer Format(DateTime date, StringBuffer toAppendTo, FieldPosition pos)
		{
			pos.BeginIndex_Renamed = pos.EndIndex_Renamed = 0;
			return Format(date, toAppendTo, pos.FieldDelegate);
		}

		// Called from Format after creating a FieldDelegate
		private StringBuffer Format(DateTime date, StringBuffer toAppendTo, FieldDelegate @delegate)
		{
			// Convert input date to time field list
			Calendar_Renamed = new DateTime(date);

			bool useDateFormatSymbols = UseDateFormatSymbols();

			for (int i = 0; i < CompiledPattern.Length;)
			{
				int tag = (int)((uint)CompiledPattern[i] >> 8);
				int count = CompiledPattern[i++] & 0xff;
				if (count == 255)
				{
					count = CompiledPattern[i++] << 16;
					count |= CompiledPattern[i++];
				}

				switch (tag)
				{
				case TAG_QUOTE_ASCII_CHAR:
					toAppendTo.Append((char)count);
					break;

				case TAG_QUOTE_CHARS:
					toAppendTo.Append(CompiledPattern, i, count);
					i += count;
					break;

				default:
					SubFormat(tag, count, @delegate, toAppendTo, useDateFormatSymbols);
					break;
				}
			}
			return toAppendTo;
		}

		/// <summary>
		/// Formats an Object producing an <code>AttributedCharacterIterator</code>.
		/// You can use the returned <code>AttributedCharacterIterator</code>
		/// to build the resulting String, as well as to determine information
		/// about the resulting String.
		/// <para>
		/// Each attribute key of the AttributedCharacterIterator will be of type
		/// <code>DateFormat.Field</code>, with the corresponding attribute value
		/// being the same as the attribute key.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if obj is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the Format cannot format the
		///            given object, or if the Format's pattern string is invalid. </exception>
		/// <param name="obj"> The object to format </param>
		/// <returns> AttributedCharacterIterator describing the formatted value.
		/// @since 1.4 </returns>
		public override AttributedCharacterIterator FormatToCharacterIterator(Object obj)
		{
			StringBuffer sb = new StringBuffer();
			CharacterIteratorFieldDelegate @delegate = new CharacterIteratorFieldDelegate();

			if (obj is DateTime)
			{
				Format((DateTime)obj, sb, @delegate);
			}
			else if (obj is Number)
			{
				Format(new DateTime(((Number)obj).LongValue()), sb, @delegate);
			}
			else if (obj == null)
			{
				throw new NullPointerException("formatToCharacterIterator must be passed non-null object");
			}
			else
			{
				throw new IllegalArgumentException("Cannot format given Object as a Date");
			}
			return @delegate.GetIterator(sb.ToString());
		}

		// Map index into pattern character string to Calendar field number
		private static readonly int[] PATTERN_INDEX_TO_CALENDAR_FIELD = new int[] {DateTime.ERA, DateTime.YEAR, DateTime.MONTH, DateTime.DATE, DateTime.HOUR_OF_DAY, DateTime.HOUR_OF_DAY, DateTime.MINUTE, DateTime.SECOND, DateTime.MILLISECOND, DateTime.DAY_OF_WEEK, DateTime.DAY_OF_YEAR, DateTime.DAY_OF_WEEK_IN_MONTH, DateTime.WEEK_OF_YEAR, DateTime.WEEK_OF_MONTH, DateTime.AM_PM, DateTime.HOUR, DateTime.HOUR, DateTime.ZONE_OFFSET, DateTime.ZONE_OFFSET, CalendarBuilder.WEEK_YEAR, CalendarBuilder.ISO_DAY_OF_WEEK, DateTime.ZONE_OFFSET, DateTime.MONTH};

		// Map index into pattern character string to DateFormat field number
		private static readonly int[] PATTERN_INDEX_TO_DATE_FORMAT_FIELD = new int[] {DateFormat.ERA_FIELD, DateFormat.YEAR_FIELD, DateFormat.MONTH_FIELD, DateFormat.DATE_FIELD, DateFormat.HOUR_OF_DAY1_FIELD, DateFormat.HOUR_OF_DAY0_FIELD, DateFormat.MINUTE_FIELD, DateFormat.SECOND_FIELD, DateFormat.MILLISECOND_FIELD, DateFormat.DAY_OF_WEEK_FIELD, DateFormat.DAY_OF_YEAR_FIELD, DateFormat.DAY_OF_WEEK_IN_MONTH_FIELD, DateFormat.WEEK_OF_YEAR_FIELD, DateFormat.WEEK_OF_MONTH_FIELD, DateFormat.AM_PM_FIELD, DateFormat.HOUR1_FIELD, DateFormat.HOUR0_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.YEAR_FIELD, DateFormat.DAY_OF_WEEK_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.MONTH_FIELD};

		// Maps from DecimalFormatSymbols index to Field constant
		private static readonly Field[] PATTERN_INDEX_TO_DATE_FORMAT_FIELD_ID = new Field[] {Field.ERA, Field.YEAR, Field.MONTH, Field.DAY_OF_MONTH, Field.HOUR_OF_DAY1, Field.HOUR_OF_DAY0, Field.MINUTE, Field.SECOND, Field.MILLISECOND, Field.DAY_OF_WEEK, Field.DAY_OF_YEAR, Field.DAY_OF_WEEK_IN_MONTH, Field.WEEK_OF_YEAR, Field.WEEK_OF_MONTH, Field.AM_PM, Field.HOUR1, Field.HOUR0, Field.TIME_ZONE, Field.TIME_ZONE, Field.YEAR, Field.DAY_OF_WEEK, Field.TIME_ZONE, Field.MONTH};

		/// <summary>
		/// Private member function that does the real date/time formatting.
		/// </summary>
		private void SubFormat(int patternCharIndex, int count, FieldDelegate @delegate, StringBuffer buffer, bool useDateFormatSymbols)
		{
			int maxIntCount = Integer.MaxValue;
			String current = null;
			int beginOffset = buffer.Length();

			int field = PATTERN_INDEX_TO_CALENDAR_FIELD[patternCharIndex];
			int value;
			if (field == CalendarBuilder.WEEK_YEAR)
			{
				if (Calendar_Renamed.WeekDateSupported)
				{
					value = Calendar_Renamed.WeekYear;
				}
				else
				{
					// use calendar year 'y' instead
					patternCharIndex = PATTERN_YEAR;
					field = PATTERN_INDEX_TO_CALENDAR_FIELD[patternCharIndex];
					value = Calendar_Renamed.get(field);
				}
			}
			else if (field == CalendarBuilder.ISO_DAY_OF_WEEK)
			{
				value = CalendarBuilder.ToISODayOfWeek(Calendar_Renamed.DayOfWeek);
			}
			else
			{
				value = Calendar_Renamed.get(field);
			}

			int style = (count >= 4) ? DateTime.LONG : DateTime.SHORT;
			if (!useDateFormatSymbols && field < DateTime.ZONE_OFFSET && patternCharIndex != PATTERN_MONTH_STANDALONE)
			{
				current = Calendar_Renamed.getDisplayName(field, style, Locale);
			}

			// Note: zeroPaddingNumber() assumes that maxDigits is either
			// 2 or maxIntCount. If we make any changes to this,
			// zeroPaddingNumber() must be fixed.

			switch (patternCharIndex)
			{
			case PATTERN_ERA: // 'G'
				if (useDateFormatSymbols)
				{
					String[] eras = FormatData.Eras;
					if (value < eras.Length)
					{
						current = eras[value];
					}
				}
				if (current == null)
				{
					current = "";
				}
				break;

			case PATTERN_WEEK_YEAR: // 'Y'
			case PATTERN_YEAR: // 'y'
				if (Calendar_Renamed is GregorianCalendar)
				{
					if (count != 2)
					{
						ZeroPaddingNumber(value, count, maxIntCount, buffer);
					}
					else
					{
						ZeroPaddingNumber(value, 2, 2, buffer);
					} // clip 1996 to 96
				}
				else
				{
					if (current == null)
					{
						ZeroPaddingNumber(value, style == DateTime.LONG ? 1 : count, maxIntCount, buffer);
					}
				}
				break;

			case PATTERN_MONTH: // 'M' (context seinsive)
				if (useDateFormatSymbols)
				{
					String[] months;
					if (count >= 4)
					{
						months = FormatData.Months;
						current = months[value];
					}
					else if (count == 3)
					{
						months = FormatData.ShortMonths;
						current = months[value];
					}
				}
				else
				{
					if (count < 3)
					{
						current = null;
					}
					else if (ForceStandaloneForm)
					{
						current = Calendar_Renamed.getDisplayName(field, style | 0x8000, Locale);
						if (current == null)
						{
							current = Calendar_Renamed.getDisplayName(field, style, Locale);
						}
					}
				}
				if (current == null)
				{
					ZeroPaddingNumber(value+1, count, maxIntCount, buffer);
				}
				break;

			case PATTERN_MONTH_STANDALONE: // 'L'
				Debug.Assert(current == null);
				if (Locale == null)
				{
					String[] months;
					if (count >= 4)
					{
						months = FormatData.Months;
						current = months[value];
					}
					else if (count == 3)
					{
						months = FormatData.ShortMonths;
						current = months[value];
					}
				}
				else
				{
					if (count >= 3)
					{
						current = Calendar_Renamed.getDisplayName(field, style | 0x8000, Locale);
					}
				}
				if (current == null)
				{
					ZeroPaddingNumber(value+1, count, maxIntCount, buffer);
				}
				break;

			case PATTERN_HOUR_OF_DAY1: // 'k' 1-based.  eg, 23:59 + 1 hour =>> 24:59
				if (current == null)
				{
					if (value == 0)
					{
						ZeroPaddingNumber(Calendar_Renamed.getMaximum(DateTime.HOUR_OF_DAY) + 1, count, maxIntCount, buffer);
					}
					else
					{
						ZeroPaddingNumber(value, count, maxIntCount, buffer);
					}
				}
				break;

			case PATTERN_DAY_OF_WEEK: // 'E'
				if (useDateFormatSymbols)
				{
					String[] weekdays;
					if (count >= 4)
					{
						weekdays = FormatData.Weekdays;
						current = weekdays[value];
					} // count < 4, use abbreviated form if exists
					else
					{
						weekdays = FormatData.ShortWeekdays;
						current = weekdays[value];
					}
				}
				break;

			case PATTERN_AM_PM: // 'a'
				if (useDateFormatSymbols)
				{
					String[] ampm = FormatData.AmPmStrings;
					current = ampm[value];
				}
				break;

			case PATTERN_HOUR1: // 'h' 1-based.  eg, 11PM + 1 hour =>> 12 AM
				if (current == null)
				{
					if (value == 0)
					{
						ZeroPaddingNumber(Calendar_Renamed.getLeastMaximum(DateTime.HOUR) + 1, count, maxIntCount, buffer);
					}
					else
					{
						ZeroPaddingNumber(value, count, maxIntCount, buffer);
					}
				}
				break;

			case PATTERN_ZONE_NAME: // 'z'
				if (current == null)
				{
					if (FormatData.Locale == null || FormatData.IsZoneStringsSet)
					{
						int zoneIndex = FormatData.GetZoneIndex(Calendar_Renamed.TimeZone.ID);
						if (zoneIndex == -1)
						{
							value = Calendar_Renamed.get(DateTime.ZONE_OFFSET) + Calendar_Renamed.get(DateTime.DST_OFFSET);
							buffer.Append(ZoneInfoFile.toCustomID(value));
						}
						else
						{
							int index = (Calendar_Renamed.get(DateTime.DST_OFFSET) == 0) ? 1: 3;
							if (count < 4)
							{
								// Use the short name
								index++;
							}
							String[][] zoneStrings = FormatData.ZoneStringsWrapper;
							buffer.Append(zoneStrings[zoneIndex][index]);
						}
					}
					else
					{
						TimeZone tz = Calendar_Renamed.TimeZone;
						bool daylight = (Calendar_Renamed.get(DateTime.DST_OFFSET) != 0);
						int tzstyle = (count < 4 ? TimeZone.SHORT : TimeZone.LONG);
						buffer.Append(tz.GetDisplayName(daylight, tzstyle, FormatData.Locale));
					}
				}
				break;

			case PATTERN_ZONE_VALUE: // 'Z' ("-/+hhmm" form)
				value = (Calendar_Renamed.get(DateTime.ZONE_OFFSET) + Calendar_Renamed.get(DateTime.DST_OFFSET)) / 60000;

				int width = 4;
				if (value >= 0)
				{
					buffer.Append('+');
				}
				else
				{
					width++;
				}

				int num = (value / 60) * 100 + (value % 60);
				CalendarUtils.sprintf0d(buffer, num, width);
				break;

			case PATTERN_ISO_ZONE: // 'X'
				value = Calendar_Renamed.get(DateTime.ZONE_OFFSET) + Calendar_Renamed.get(DateTime.DST_OFFSET);

				if (value == 0)
				{
					buffer.Append('Z');
					break;
				}

				value /= 60000;
				if (value >= 0)
				{
					buffer.Append('+');
				}
				else
				{
					buffer.Append('-');
					value = -value;
				}

				CalendarUtils.sprintf0d(buffer, value / 60, 2);
				if (count == 1)
				{
					break;
				}

				if (count == 3)
				{
					buffer.Append(':');
				}
				CalendarUtils.sprintf0d(buffer, value % 60, 2);
				break;

			default:
		 // case PATTERN_DAY_OF_MONTH:         // 'd'
		 // case PATTERN_HOUR_OF_DAY0:         // 'H' 0-based.  eg, 23:59 + 1 hour =>> 00:59
		 // case PATTERN_MINUTE:               // 'm'
		 // case PATTERN_SECOND:               // 's'
		 // case PATTERN_MILLISECOND:          // 'S'
		 // case PATTERN_DAY_OF_YEAR:          // 'D'
		 // case PATTERN_DAY_OF_WEEK_IN_MONTH: // 'F'
		 // case PATTERN_WEEK_OF_YEAR:         // 'w'
		 // case PATTERN_WEEK_OF_MONTH:        // 'W'
		 // case PATTERN_HOUR0:                // 'K' eg, 11PM + 1 hour =>> 0 AM
		 // case PATTERN_ISO_DAY_OF_WEEK:      // 'u' pseudo field, Monday = 1, ..., Sunday = 7
				if (current == null)
				{
					ZeroPaddingNumber(value, count, maxIntCount, buffer);
				}
				break;
			} // switch (patternCharIndex)

			if (current != null)
			{
				buffer.Append(current);
			}

			int fieldID = PATTERN_INDEX_TO_DATE_FORMAT_FIELD[patternCharIndex];
			Field f = PATTERN_INDEX_TO_DATE_FORMAT_FIELD_ID[patternCharIndex];

			@delegate.Formatted(fieldID, f, f, beginOffset, buffer.Length(), buffer);
		}

		/// <summary>
		/// Formats a number with the specified minimum and maximum number of digits.
		/// </summary>
		private void ZeroPaddingNumber(int value, int minDigits, int maxDigits, StringBuffer buffer)
		{
			// Optimization for 1, 2 and 4 digit numbers. This should
			// cover most cases of formatting date/time related items.
			// Note: This optimization code assumes that maxDigits is
			// either 2 or Integer.MAX_VALUE (maxIntCount in format()).
			try
			{
				if (ZeroDigit == 0)
				{
					ZeroDigit = ((DecimalFormat)NumberFormat_Renamed).DecimalFormatSymbols.ZeroDigit;
				}
				if (value >= 0)
				{
					if (value < 100 && minDigits >= 1 && minDigits <= 2)
					{
						if (value < 10)
						{
							if (minDigits == 2)
							{
								buffer.Append(ZeroDigit);
							}
							buffer.Append((char)(ZeroDigit + value));
						}
						else
						{
							buffer.Append((char)(ZeroDigit + value / 10));
							buffer.Append((char)(ZeroDigit + value % 10));
						}
						return;
					}
					else if (value >= 1000 && value < 10000)
					{
						if (minDigits == 4)
						{
							buffer.Append((char)(ZeroDigit + value / 1000));
							value %= 1000;
							buffer.Append((char)(ZeroDigit + value / 100));
							value %= 100;
							buffer.Append((char)(ZeroDigit + value / 10));
							buffer.Append((char)(ZeroDigit + value % 10));
							return;
						}
						if (minDigits == 2 && maxDigits == 2)
						{
							ZeroPaddingNumber(value % 100, 2, 2, buffer);
							return;
						}
					}
				}
			}
			catch (Exception)
			{
			}

			NumberFormat_Renamed.MinimumIntegerDigits = minDigits;
			NumberFormat_Renamed.MaximumIntegerDigits = maxDigits;
			NumberFormat_Renamed.Format((long)value, buffer, DontCareFieldPosition.INSTANCE);
		}


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
		/// 
		/// </para>
		/// <para>This parsing operation uses the {@link DateFormat#calendar
		/// calendar} to produce a {@code Date}. All of the {@code
		/// calendar}'s date-time fields are {@link Calendar#clear()
		/// cleared} before parsing, and the {@code calendar}'s default
		/// values of the date-time fields are used for any missing
		/// date-time information. For example, the year value of the
		/// parsed {@code Date} is 1970 with <seealso cref="GregorianCalendar"/> if
		/// no year value is given from the parsing operation.  The {@code
		/// TimeZone} value may be overwritten, depending on the given
		/// pattern and the time zone value in {@code text}. Any {@code
		/// TimeZone} value that has previously been set by a call to
		/// <seealso cref="#setTimeZone(java.util.TimeZone) setTimeZone"/> may need
		/// to be restored for further operations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  A <code>String</code>, part of which should be parsed. </param>
		/// <param name="pos">   A <code>ParsePosition</code> object with index and error
		///              index information as described above. </param>
		/// <returns> A <code>Date</code> parsed from the string. In case of
		///         error, returns null. </returns>
		/// <exception cref="NullPointerException"> if <code>text</code> or <code>pos</code> is null. </exception>
		public override DateTime Parse(String text, ParsePosition pos)
		{
			CheckNegativeNumberExpression();

			int start = pos.Index_Renamed;
			int oldStart = start;
			int textLength = text.Length();

			bool[] ambiguousYear = new bool[] {false};

			CalendarBuilder calb = new CalendarBuilder();

			for (int i = 0; i < CompiledPattern.Length;)
			{
				int tag = (int)((uint)CompiledPattern[i] >> 8);
				int count = CompiledPattern[i++] & 0xff;
				if (count == 255)
				{
					count = CompiledPattern[i++] << 16;
					count |= CompiledPattern[i++];
				}

				switch (tag)
				{
				case TAG_QUOTE_ASCII_CHAR:
					if (start >= textLength || text.CharAt(start) != (char)count)
					{
						pos.Index_Renamed = oldStart;
						pos.ErrorIndex_Renamed = start;
						return null;
					}
					start++;
					break;

				case TAG_QUOTE_CHARS:
					while (count-- > 0)
					{
						if (start >= textLength || text.CharAt(start) != CompiledPattern[i++])
						{
							pos.Index_Renamed = oldStart;
							pos.ErrorIndex_Renamed = start;
							return null;
						}
						start++;
					}
					break;

				default:
					// Peek the next pattern to determine if we need to
					// obey the number of pattern letters for
					// parsing. It's required when parsing contiguous
					// digit text (e.g., "20010704") with a pattern which
					// has no delimiters between fields, like "yyyyMMdd".
					bool obeyCount = false;

					// In Arabic, a minus sign for a negative number is put after
					// the number. Even in another locale, a minus sign can be
					// put after a number using DateFormat.setNumberFormat().
					// If both the minus sign and the field-delimiter are '-',
					// subParse() needs to determine whether a '-' after a number
					// in the given text is a delimiter or is a minus sign for the
					// preceding number. We give subParse() a clue based on the
					// information in compiledPattern.
					bool useFollowingMinusSignAsDelimiter = false;

					if (i < CompiledPattern.Length)
					{
						int nextTag = (int)((uint)CompiledPattern[i] >> 8);
						if (!(nextTag == TAG_QUOTE_ASCII_CHAR || nextTag == TAG_QUOTE_CHARS))
						{
							obeyCount = true;
						}

						if (HasFollowingMinusSign && (nextTag == TAG_QUOTE_ASCII_CHAR || nextTag == TAG_QUOTE_CHARS))
						{
							int c;
							if (nextTag == TAG_QUOTE_ASCII_CHAR)
							{
								c = CompiledPattern[i] & 0xff;
							}
							else
							{
								c = CompiledPattern[i + 1];
							}

							if (c == MinusSign)
							{
								useFollowingMinusSignAsDelimiter = true;
							}
						}
					}
					start = SubParse(text, start, tag, count, obeyCount, ambiguousYear, pos, useFollowingMinusSignAsDelimiter, calb);
					if (start < 0)
					{
						pos.Index_Renamed = oldStart;
						return null;
					}
				}
			}

			// At this point the fields of Calendar have been set.  Calendar
			// will fill in default values for missing fields when the time
			// is computed.

			pos.Index_Renamed = start;

			DateTime parsedDate;
			try
			{
				parsedDate = calb.Establish(Calendar_Renamed).Ticks;
				// If the year value is ambiguous,
				// then the two-digit year == the default start year
				if (ambiguousYear[0])
				{
					if (parsedDate < DefaultCenturyStart)
					{
						parsedDate = calb.AddYear(100).Establish(Calendar_Renamed).Ticks;
					}
				}
			}
			// An IllegalArgumentException will be thrown by Calendar.getTime()
			// if any fields are out of range, e.g., MONTH == 17.
			catch (IllegalArgumentException)
			{
				pos.ErrorIndex_Renamed = start;
				pos.Index_Renamed = oldStart;
				return null;
			}

			return parsedDate;
		}

		/// <summary>
		/// Private code-size reduction function used by subParse. </summary>
		/// <param name="text"> the time text being parsed. </param>
		/// <param name="start"> where to start parsing. </param>
		/// <param name="field"> the date field being parsed. </param>
		/// <param name="data"> the string array to parsed. </param>
		/// <returns> the new start position if matching succeeded; a negative number
		/// indicating matching failure, otherwise. </returns>
		private int MatchString(String text, int start, int field, String[] data, CalendarBuilder calb)
		{
			int i = 0;
			int count = data.Length;

			if (field == DateTime.DAY_OF_WEEK)
			{
				i = 1;
			}

			// There may be multiple strings in the data[] array which begin with
			// the same prefix (e.g., Cerven and Cervenec (June and July) in Czech).
			// We keep track of the longest match, and return that.  Note that this
			// unfortunately requires us to test all array elements.
			int bestMatchLength = 0, bestMatch = -1;
			for (; i < count; ++i)
			{
				int length = data[i].Length();
				// Always compare if we have no match yet; otherwise only compare
				// against potentially better matches (longer strings).
				if (length > bestMatchLength && text.RegionMatches(true, start, data[i], 0, length))
				{
					bestMatch = i;
					bestMatchLength = length;
				}
			}
			if (bestMatch >= 0)
			{
				calb.Set(field, bestMatch);
				return start + bestMatchLength;
			}
			return -start;
		}

		/// <summary>
		/// Performs the same thing as matchString(String, int, int,
		/// String[]). This method takes a Map<String, Integer> instead of
		/// String[].
		/// </summary>
		private int MatchString(String text, int start, int field, IDictionary<String, Integer> data, CalendarBuilder calb)
		{
			if (data != null)
			{
				// TODO: make this default when it's in the spec.
				if (data is SortedMap)
				{
					foreach (String name in data.Keys)
					{
						if (text.RegionMatches(true, start, name, 0, name.Length()))
						{
							calb.Set(field, data[name]);
							return start + name.Length();
						}
					}
					return -start;
				}

				String bestMatch = null;

				foreach (String name in data.Keys)
				{
					int length = name.Length();
					if (bestMatch == null || length > bestMatch.Length())
					{
						if (text.RegionMatches(true, start, name, 0, length))
						{
							bestMatch = name;
						}
					}
				}

				if (bestMatch != null)
				{
					calb.Set(field, data[bestMatch]);
					return start + bestMatch.Length();
				}
			}
			return -start;
		}

		private int MatchZoneString(String text, int start, String[] zoneNames)
		{
			for (int i = 1; i <= 4; ++i)
			{
				// Checking long and short zones [1 & 2],
				// and long and short daylight [3 & 4].
				String zoneName = zoneNames[i];
				if (text.RegionMatches(true, start, zoneName, 0, zoneName.Length()))
				{
					return i;
				}
			}
			return -1;
		}

		private bool MatchDSTString(String text, int start, int zoneIndex, int standardIndex, String[][] zoneStrings)
		{
			int index = standardIndex + 2;
			String zoneName = zoneStrings[zoneIndex][index];
			if (text.RegionMatches(true, start, zoneName, 0, zoneName.Length()))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// find time zone 'text' matched zoneStrings and set to internal
		/// calendar.
		/// </summary>
		private int SubParseZoneString(String text, int start, CalendarBuilder calb)
		{
			bool useSameName = false; // true if standard and daylight time use the same abbreviation.
			TimeZone currentTimeZone = TimeZone;

			// At this point, check for named time zones by looking through
			// the locale data from the TimeZoneNames strings.
			// Want to be able to parse both short and long forms.
			int zoneIndex = FormatData.GetZoneIndex(currentTimeZone.ID);
			TimeZone tz = null;
			String[][] zoneStrings = FormatData.ZoneStringsWrapper;
			String[] zoneNames = null;
			int nameIndex = 0;
			if (zoneIndex != -1)
			{
				zoneNames = zoneStrings[zoneIndex];
				if ((nameIndex = MatchZoneString(text, start, zoneNames)) > 0)
				{
					if (nameIndex <= 2)
					{
						// Check if the standard name (abbr) and the daylight name are the same.
						useSameName = zoneNames[nameIndex].EqualsIgnoreCase(zoneNames[nameIndex + 2]);
					}
					tz = TimeZone.GetTimeZone(zoneNames[0]);
				}
			}
			if (tz == null)
			{
				zoneIndex = FormatData.GetZoneIndex(TimeZone.Default.ID);
				if (zoneIndex != -1)
				{
					zoneNames = zoneStrings[zoneIndex];
					if ((nameIndex = MatchZoneString(text, start, zoneNames)) > 0)
					{
						if (nameIndex <= 2)
						{
							useSameName = zoneNames[nameIndex].EqualsIgnoreCase(zoneNames[nameIndex + 2]);
						}
						tz = TimeZone.GetTimeZone(zoneNames[0]);
					}
				}
			}

			if (tz == null)
			{
				int len = zoneStrings.Length;
				for (int i = 0; i < len; i++)
				{
					zoneNames = zoneStrings[i];
					if ((nameIndex = MatchZoneString(text, start, zoneNames)) > 0)
					{
						if (nameIndex <= 2)
						{
							useSameName = zoneNames[nameIndex].EqualsIgnoreCase(zoneNames[nameIndex + 2]);
						}
						tz = TimeZone.GetTimeZone(zoneNames[0]);
						break;
					}
				}
			}
			if (tz != null) // Matched any ?
			{
				if (!tz.Equals(currentTimeZone))
				{
					TimeZone = tz;
				}
				// If the time zone matched uses the same name
				// (abbreviation) for both standard and daylight time,
				// let the time zone in the Calendar decide which one.
				//
				// Also if tz.getDSTSaving() returns 0 for DST, use tz to
				// determine the local time. (6645292)
				int dstAmount = (nameIndex >= 3) ? tz.DSTSavings : 0;
				if (!(useSameName || (nameIndex >= 3 && dstAmount == 0)))
				{
					calb.Clear(DateTime.ZONE_OFFSET).Set(DateTime.DST_OFFSET, dstAmount);
				}
				return (start + zoneNames[nameIndex].Length());
			}
			return 0;
		}

		/// <summary>
		/// Parses numeric forms of time zone offset, such as "hh:mm", and
		/// sets calb to the parsed value.
		/// </summary>
		/// <param name="text">  the text to be parsed </param>
		/// <param name="start"> the character position to start parsing </param>
		/// <param name="sign">  1: positive; -1: negative </param>
		/// <param name="count"> 0: 'Z' or "GMT+hh:mm" parsing; 1 - 3: the number of 'X's </param>
		/// <param name="colon"> true - colon required between hh and mm; false - no colon required </param>
		/// <param name="calb">  a CalendarBuilder in which the parsed value is stored </param>
		/// <returns> updated parsed position, or its negative value to indicate a parsing error </returns>
		private int SubParseNumericZone(String text, int start, int sign, int count, bool colon, CalendarBuilder calb)
		{
			int index = start;

			try
			{
				char c = text.CharAt(index++);
				// Parse hh
				int hours;
				if (!IsDigit(c))
				{
					goto parseBreak;
				}
				hours = c - '0';
				c = text.CharAt(index++);
				if (IsDigit(c))
				{
					hours = hours * 10 + (c - '0');
				}
				else
				{
					// If no colon in RFC 822 or 'X' (ISO), two digits are
					// required.
					if (count > 0 || !colon)
					{
						goto parseBreak;
					}
					--index;
				}
				if (hours > 23)
				{
					goto parseBreak;
				}
				int minutes = 0;
				if (count != 1)
				{
					// Proceed with parsing mm
					c = text.CharAt(index++);
					if (colon)
					{
						if (c != ':')
						{
							goto parseBreak;
						}
						c = text.CharAt(index++);
					}
					if (!IsDigit(c))
					{
						goto parseBreak;
					}
					minutes = c - '0';
					c = text.CharAt(index++);
					if (!IsDigit(c))
					{
						goto parseBreak;
					}
					minutes = minutes * 10 + (c - '0');
					if (minutes > 59)
					{
						goto parseBreak;
					}
				}
				minutes += hours * 60;
				calb.Set(DateTime.ZONE_OFFSET, minutes * MILLIS_PER_MINUTE * sign).Set(DateTime.DST_OFFSET, 0);
				return index;
			}
		  parseBreak:
			catch (IndexOutOfBoundsException)
			{
			}
			return 1 - index; // -(index - 1)
		}

		private bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		/// <summary>
		/// Private member function that converts the parsed date strings into
		/// timeFields. Returns -start (for ParsePosition) if failed. </summary>
		/// <param name="text"> the time text to be parsed. </param>
		/// <param name="start"> where to start parsing. </param>
		/// <param name="patternCharIndex"> the index of the pattern character. </param>
		/// <param name="count"> the count of a pattern character. </param>
		/// <param name="obeyCount"> if true, then the next field directly abuts this one,
		/// and we should use the count to know when to stop parsing. </param>
		/// <param name="ambiguousYear"> return parameter; upon return, if ambiguousYear[0]
		/// is true, then a two-digit year was parsed and may need to be readjusted. </param>
		/// <param name="origPos"> origPos.errorIndex is used to return an error index
		/// at which a parse error occurred, if matching failure occurs. </param>
		/// <returns> the new start position if matching succeeded; -1 indicating
		/// matching failure, otherwise. In case matching failure occurred,
		/// an error index is set to origPos.errorIndex. </returns>
		private int SubParse(String text, int start, int patternCharIndex, int count, bool obeyCount, bool[] ambiguousYear, ParsePosition origPos, bool useFollowingMinusSignAsDelimiter, CalendarBuilder calb)
		{
			Number number;
			int value = 0;
			ParsePosition pos = new ParsePosition(0);
			pos.Index_Renamed = start;
			if (patternCharIndex == PATTERN_WEEK_YEAR && !Calendar_Renamed.WeekDateSupported)
			{
				// use calendar year 'y' instead
				patternCharIndex = PATTERN_YEAR;
			}
			int field = PATTERN_INDEX_TO_CALENDAR_FIELD[patternCharIndex];

			// If there are any spaces here, skip over them.  If we hit the end
			// of the string, then fail.
			for (;;)
			{
				if (pos.Index_Renamed >= text.Length())
				{
					origPos.ErrorIndex_Renamed = start;
					return -1;
				}
				char c = text.CharAt(pos.Index_Renamed);
				if (c != ' ' && c != '\t')
				{
					break;
				}
				++pos.Index_Renamed;
			}
			// Remember the actual start index
			int actualStart = pos.Index_Renamed;

			{
				// We handle a few special cases here where we need to parse
				// a number value.  We handle further, more generic cases below.  We need
				// to handle some of them here because some fields require extra processing on
				// the parsed value.
				if (patternCharIndex == PATTERN_HOUR_OF_DAY1 || patternCharIndex == PATTERN_HOUR1 || (patternCharIndex == PATTERN_MONTH && count <= 2) || patternCharIndex == PATTERN_YEAR || patternCharIndex == PATTERN_WEEK_YEAR)
				{
					// It would be good to unify this with the obeyCount logic below,
					// but that's going to be difficult.
					if (obeyCount)
					{
						if ((start + count) > text.Length())
						{
							goto parsingBreak;
						}
						number = NumberFormat_Renamed.Parse(text.Substring(0, start + count), pos);
					}
					else
					{
						number = NumberFormat_Renamed.Parse(text, pos);
					}
					if (number == null)
					{
						if (patternCharIndex != PATTERN_YEAR || Calendar_Renamed is GregorianCalendar)
						{
							goto parsingBreak;
						}
					}
					else
					{
						value = number.IntValue();

						if (useFollowingMinusSignAsDelimiter && (value < 0) && (((pos.Index_Renamed < text.Length()) && (text.CharAt(pos.Index_Renamed) != MinusSign)) || ((pos.Index_Renamed == text.Length()) && (text.CharAt(pos.Index_Renamed - 1) == MinusSign))))
						{
							value = -value;
							pos.Index_Renamed--;
						}
					}
				}

				bool useDateFormatSymbols = UseDateFormatSymbols();

				int index;
				switch (patternCharIndex)
				{
				case PATTERN_ERA: // 'G'
					if (useDateFormatSymbols)
					{
						if ((index = MatchString(text, start, DateTime.ERA, FormatData.Eras, calb)) > 0)
						{
							return index;
						}
					}
					else
					{
						IDictionary<String, Integer> map = GetDisplayNamesMap(field, Locale);
						if ((index = MatchString(text, start, field, map, calb)) > 0)
						{
							return index;
						}
					}
					goto parsingBreak;

				case PATTERN_WEEK_YEAR: // 'Y'
				case PATTERN_YEAR: // 'y'
					if (!(Calendar_Renamed is GregorianCalendar))
					{
						// calendar might have text representations for year values,
						// such as "\u5143" in JapaneseImperialCalendar.
						int style = (count >= 4) ? DateTime.LONG : DateTime.SHORT;
						IDictionary<String, Integer> map = Calendar_Renamed.getDisplayNames(field, style, Locale);
						if (map != null)
						{
							if ((index = MatchString(text, start, field, map, calb)) > 0)
							{
								return index;
							}
						}
						calb.Set(field, value);
						return pos.Index_Renamed;
					}

					// If there are 3 or more YEAR pattern characters, this indicates
					// that the year value is to be treated literally, without any
					// two-digit year adjustments (e.g., from "01" to 2001).  Otherwise
					// we made adjustments to place the 2-digit year in the proper
					// century, for parsed strings from "00" to "99".  Any other string
					// is treated literally:  "2250", "-1", "1", "002".
					if (count <= 2 && (pos.Index_Renamed - actualStart) == 2 && char.IsDigit(text.CharAt(actualStart)) && char.IsDigit(text.CharAt(actualStart + 1)))
					{
						// Assume for example that the defaultCenturyStart is 6/18/1903.
						// This means that two-digit years will be forced into the range
						// 6/18/1903 to 6/17/2003.  As a result, years 00, 01, and 02
						// correspond to 2000, 2001, and 2002.  Years 04, 05, etc. correspond
						// to 1904, 1905, etc.  If the year is 03, then it is 2003 if the
						// other fields specify a date before 6/18, or 1903 if they specify a
						// date afterwards.  As a result, 03 is an ambiguous year.  All other
						// two-digit years are unambiguous.
						int ambiguousTwoDigitYear = DefaultCenturyStartYear % 100;
						ambiguousYear[0] = value == ambiguousTwoDigitYear;
						value += (DefaultCenturyStartYear / 100) * 100 + (value < ambiguousTwoDigitYear ? 100 : 0);
					}
					calb.Set(field, value);
					return pos.Index_Renamed;

				case PATTERN_MONTH: // 'M'
					if (count <= 2) // i.e., M or MM.
					{
						// Don't want to parse the month if it is a string
						// while pattern uses numeric style: M or MM.
						// [We computed 'value' above.]
						calb.Set(DateTime.MONTH, value - 1);
						return pos.Index_Renamed;
					}

					if (useDateFormatSymbols)
					{
						// count >= 3 // i.e., MMM or MMMM
						// Want to be able to parse both short and long forms.
						// Try count == 4 first:
						int newStart;
						if ((newStart = MatchString(text, start, DateTime.MONTH, FormatData.Months, calb)) > 0)
						{
							return newStart;
						}
						// count == 4 failed, now try count == 3
						if ((index = MatchString(text, start, DateTime.MONTH, FormatData.ShortMonths, calb)) > 0)
						{
							return index;
						}
					}
					else
					{
						IDictionary<String, Integer> map = GetDisplayNamesMap(field, Locale);
						if ((index = MatchString(text, start, field, map, calb)) > 0)
						{
							return index;
						}
					}
					goto parsingBreak;

				case PATTERN_HOUR_OF_DAY1: // 'k' 1-based.  eg, 23:59 + 1 hour =>> 24:59
					if (!Lenient)
					{
						// Validate the hour value in non-lenient
						if (value < 1 || value > 24)
						{
							goto parsingBreak;
						}
					}
					// [We computed 'value' above.]
					if (value == Calendar_Renamed.getMaximum(DateTime.HOUR_OF_DAY) + 1)
					{
						value = 0;
					}
					calb.Set(DateTime.HOUR_OF_DAY, value);
					return pos.Index_Renamed;

				case PATTERN_DAY_OF_WEEK: // 'E'
				{
						if (useDateFormatSymbols)
						{
							// Want to be able to parse both short and long forms.
							// Try count == 4 (DDDD) first:
							int newStart;
							if ((newStart = MatchString(text, start, DateTime.DAY_OF_WEEK, FormatData.Weekdays, calb)) > 0)
							{
								return newStart;
							}
							// DDDD failed, now try DDD
							if ((index = MatchString(text, start, DateTime.DAY_OF_WEEK, FormatData.ShortWeekdays, calb)) > 0)
							{
								return index;
							}
						}
						else
						{
							int[] styles = new int[] {DateTime.LONG, DateTime.SHORT};
							foreach (int style in styles)
							{
								IDictionary<String, Integer> map = Calendar_Renamed.getDisplayNames(field, style, Locale);
								if ((index = MatchString(text, start, field, map, calb)) > 0)
								{
									return index;
								}
							}
						}
				}
					goto parsingBreak;

				case PATTERN_AM_PM: // 'a'
					if (useDateFormatSymbols)
					{
						if ((index = MatchString(text, start, DateTime.AM_PM, FormatData.AmPmStrings, calb)) > 0)
						{
							return index;
						}
					}
					else
					{
						IDictionary<String, Integer> map = GetDisplayNamesMap(field, Locale);
						if ((index = MatchString(text, start, field, map, calb)) > 0)
						{
							return index;
						}
					}
					goto parsingBreak;

				case PATTERN_HOUR1: // 'h' 1-based.  eg, 11PM + 1 hour =>> 12 AM
					if (!Lenient)
					{
						// Validate the hour value in non-lenient
						if (value < 1 || value > 12)
						{
							goto parsingBreak;
						}
					}
					// [We computed 'value' above.]
					if (value == Calendar_Renamed.getLeastMaximum(DateTime.HOUR) + 1)
					{
						value = 0;
					}
					calb.Set(DateTime.HOUR, value);
					return pos.Index_Renamed;

				case PATTERN_ZONE_NAME: // 'z'
				case PATTERN_ZONE_VALUE: // 'Z'
				{
						int sign = 0;
						try
						{
							char c = text.CharAt(pos.Index_Renamed);
							if (c == '+')
							{
								sign = 1;
							}
							else if (c == '-')
							{
								sign = -1;
							}
							if (sign == 0)
							{
								// Try parsing a custom time zone "GMT+hh:mm" or "GMT".
								if ((c == 'G' || c == 'g') && (text.Length() - start) >= GMT.Length() && text.RegionMatches(true, start, GMT, 0, GMT.Length()))
								{
									pos.Index_Renamed = start + GMT.Length();

									if ((text.Length() - pos.Index_Renamed) > 0)
									{
										c = text.CharAt(pos.Index_Renamed);
										if (c == '+')
										{
											sign = 1;
										}
										else if (c == '-')
										{
											sign = -1;
										}
									}

									if (sign == 0) // "GMT" without offset
									{
										calb.Set(DateTime.ZONE_OFFSET, 0).Set(DateTime.DST_OFFSET, 0);
										return pos.Index_Renamed;
									}

									// Parse the rest as "hh:mm"
									int i = SubParseNumericZone(text, ++pos.Index_Renamed, sign, 0, true, calb);
									if (i > 0)
									{
										return i;
									}
									pos.Index_Renamed = -i;
								}
								else
								{
									// Try parsing the text as a time zone
									// name or abbreviation.
									int i = SubParseZoneString(text, pos.Index_Renamed, calb);
									if (i > 0)
									{
										return i;
									}
									pos.Index_Renamed = -i;
								}
							}
							else
							{
								// Parse the rest as "hhmm" (RFC 822)
								int i = SubParseNumericZone(text, ++pos.Index_Renamed, sign, 0, false, calb);
								if (i > 0)
								{
									return i;
								}
								pos.Index_Renamed = -i;
							}
						}
						catch (IndexOutOfBoundsException)
						{
						}
				}
					goto parsingBreak;

				case PATTERN_ISO_ZONE: // 'X'
				{
						if ((text.Length() - pos.Index_Renamed) <= 0)
						{
							goto parsingBreak;
						}

						int sign;
						char c = text.CharAt(pos.Index_Renamed);
						if (c == 'Z')
						{
							calb.Set(DateTime.ZONE_OFFSET, 0).Set(DateTime.DST_OFFSET, 0);
							return ++pos.Index_Renamed;
						}

						// parse text as "+/-hh[[:]mm]" based on count
						if (c == '+')
						{
							sign = 1;
						}
						else if (c == '-')
						{
							sign = -1;
						}
						else
						{
							++pos.Index_Renamed;
							goto parsingBreak;
						}
						int i = SubParseNumericZone(text, ++pos.Index_Renamed, sign, count, count == 3, calb);
						if (i > 0)
						{
							return i;
						}
						pos.Index_Renamed = -i;
				}
					goto parsingBreak;

				default:
			 // case PATTERN_DAY_OF_MONTH:         // 'd'
			 // case PATTERN_HOUR_OF_DAY0:         // 'H' 0-based.  eg, 23:59 + 1 hour =>> 00:59
			 // case PATTERN_MINUTE:               // 'm'
			 // case PATTERN_SECOND:               // 's'
			 // case PATTERN_MILLISECOND:          // 'S'
			 // case PATTERN_DAY_OF_YEAR:          // 'D'
			 // case PATTERN_DAY_OF_WEEK_IN_MONTH: // 'F'
			 // case PATTERN_WEEK_OF_YEAR:         // 'w'
			 // case PATTERN_WEEK_OF_MONTH:        // 'W'
			 // case PATTERN_HOUR0:                // 'K' 0-based.  eg, 11PM + 1 hour =>> 0 AM
			 // case PATTERN_ISO_DAY_OF_WEEK:      // 'u' (pseudo field);

					// Handle "generic" fields
					if (obeyCount)
					{
						if ((start + count) > text.Length())
						{
							goto parsingBreak;
						}
						number = NumberFormat_Renamed.Parse(text.Substring(0, start + count), pos);
					}
					else
					{
						number = NumberFormat_Renamed.Parse(text, pos);
					}
					if (number != null)
					{
						value = number.IntValue();

						if (useFollowingMinusSignAsDelimiter && (value < 0) && (((pos.Index_Renamed < text.Length()) && (text.CharAt(pos.Index_Renamed) != MinusSign)) || ((pos.Index_Renamed == text.Length()) && (text.CharAt(pos.Index_Renamed - 1) == MinusSign))))
						{
							value = -value;
							pos.Index_Renamed--;
						}

						calb.Set(field, value);
						return pos.Index_Renamed;
					}
					goto parsingBreak;
				}
			}
		  parsingBreak:

			// Parsing failed.
			origPos.ErrorIndex_Renamed = pos.Index_Renamed;
			return -1;
		}

		/// <summary>
		/// Returns true if the DateFormatSymbols has been set explicitly or locale
		/// is null.
		/// </summary>
		private bool UseDateFormatSymbols()
		{
			return UseDateFormatSymbols_Renamed || Locale == null;
		}

		/// <summary>
		/// Translates a pattern, mapping each character in the from string to the
		/// corresponding character in the to string.
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		private String TranslatePattern(String pattern, String from, String to)
		{
			StringBuilder result = new StringBuilder();
			bool inQuote = false;
			for (int i = 0; i < pattern.Length(); ++i)
			{
				char c = pattern.CharAt(i);
				if (inQuote)
				{
					if (c == '\'')
					{
						inQuote = false;
					}
				}
				else
				{
					if (c == '\'')
					{
						inQuote = true;
					}
					else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
					{
						int ci = from.IndexOf(c);
						if (ci >= 0)
						{
							// patternChars is longer than localPatternChars due
							// to serialization compatibility. The pattern letters
							// unsupported by localPatternChars pass through.
							if (ci < to.Length())
							{
								c = to.CharAt(ci);
							}
						}
						else
						{
							throw new IllegalArgumentException("Illegal pattern " + " character '" + c + "'");
						}
					}
				}
				result.Append(c);
			}
			if (inQuote)
			{
				throw new IllegalArgumentException("Unfinished quote in pattern");
			}
			return result.ToString();
		}

		/// <summary>
		/// Returns a pattern string describing this date format.
		/// </summary>
		/// <returns> a pattern string describing this date format. </returns>
		public virtual String ToPattern()
		{
			return Pattern;
		}

		/// <summary>
		/// Returns a localized pattern string describing this date format.
		/// </summary>
		/// <returns> a localized pattern string describing this date format. </returns>
		public virtual String ToLocalizedPattern()
		{
			return TranslatePattern(Pattern, DateFormatSymbols.PatternChars, FormatData.LocalPatternChars);
		}

		/// <summary>
		/// Applies the given pattern string to this date format.
		/// </summary>
		/// <param name="pattern"> the new date and time pattern for this date format </param>
		/// <exception cref="NullPointerException"> if the given pattern is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		public virtual void ApplyPattern(String pattern)
		{
			ApplyPatternImpl(pattern);
		}

		private void ApplyPatternImpl(String pattern)
		{
			CompiledPattern = Compile(pattern);
			this.Pattern = pattern;
		}

		/// <summary>
		/// Applies the given localized pattern string to this date format.
		/// </summary>
		/// <param name="pattern"> a String to be mapped to the new date and time format
		///        pattern for this format </param>
		/// <exception cref="NullPointerException"> if the given pattern is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		public virtual void ApplyLocalizedPattern(String pattern)
		{
			 String p = TranslatePattern(pattern, FormatData.LocalPatternChars, DateFormatSymbols.PatternChars);
			 CompiledPattern = Compile(p);
			 this.Pattern = p;
		}

		/// <summary>
		/// Gets a copy of the date and time format symbols of this date format.
		/// </summary>
		/// <returns> the date and time format symbols of this date format </returns>
		/// <seealso cref= #setDateFormatSymbols </seealso>
		public virtual DateFormatSymbols DateFormatSymbols
		{
			get
			{
				return (DateFormatSymbols)FormatData.Clone();
			}
			set
			{
				this.FormatData = (DateFormatSymbols)value.Clone();
				UseDateFormatSymbols_Renamed = true;
			}
		}


		/// <summary>
		/// Creates a copy of this <code>SimpleDateFormat</code>. This also
		/// clones the format's date format symbols.
		/// </summary>
		/// <returns> a clone of this <code>SimpleDateFormat</code> </returns>
		public override Object Clone()
		{
			SimpleDateFormat other = (SimpleDateFormat) base.Clone();
			other.FormatData = (DateFormatSymbols) FormatData.Clone();
			return other;
		}

		/// <summary>
		/// Returns the hash code value for this <code>SimpleDateFormat</code> object.
		/// </summary>
		/// <returns> the hash code value for this <code>SimpleDateFormat</code> object. </returns>
		public override int HashCode()
		{
			return Pattern.HashCode();
			// just enough fields for a reasonable distribution
		}

		/// <summary>
		/// Compares the given object with this <code>SimpleDateFormat</code> for
		/// equality.
		/// </summary>
		/// <returns> true if the given object is equal to this
		/// <code>SimpleDateFormat</code> </returns>
		public override bool Equals(Object obj)
		{
			if (!base.Equals(obj))
			{
				return false; // super does class check
			}
			SimpleDateFormat that = (SimpleDateFormat) obj;
			return (Pattern.Equals(that.Pattern) && FormatData.Equals(that.FormatData));
		}

		private static readonly int[] REST_OF_STYLES = new int[] {DateTime.SHORT_STANDALONE, DateTime.LONG_FORMAT, DateTime.LONG_STANDALONE};
		private IDictionary<String, Integer> GetDisplayNamesMap(int field, Locale locale)
		{
			IDictionary<String, Integer> map = Calendar_Renamed.getDisplayNames(field, DateTime.SHORT_FORMAT, locale);
			// Get all SHORT and LONG styles (avoid NARROW styles).
			foreach (int style in REST_OF_STYLES)
			{
				IDictionary<String, Integer> m = Calendar_Renamed.getDisplayNames(field, style, locale);
				if (m != null)
				{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
					map.putAll(m);
				}
			}
			return map;
		}

		/// <summary>
		/// After reading an object from the input stream, the format
		/// pattern in the object is verified.
		/// <para>
		/// </para>
		/// </summary>
		/// <exception cref="InvalidObjectException"> if the pattern is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();

			try
			{
				CompiledPattern = Compile(Pattern);
			}
			catch (Exception)
			{
				throw new InvalidObjectException("invalid pattern");
			}

			if (SerialVersionOnStream < 1)
			{
				// didn't have defaultCenturyStart field
				InitializeDefaultCentury();
			}
			else
			{
				// fill in dependent transient field
				ParseAmbiguousDatesAsAfter(DefaultCenturyStart);
			}
			SerialVersionOnStream = CurrentSerialVersion;

			// If the deserialized object has a SimpleTimeZone, try
			// to replace it with a ZoneInfo equivalent in order to
			// be compatible with the SimpleTimeZone-based
			// implementation as much as possible.
			TimeZone tz = TimeZone;
			if (tz is SimpleTimeZone)
			{
				String id = tz.ID;
				TimeZone zi = TimeZone.GetTimeZone(id);
				if (zi != null && zi.HasSameRules(tz) && zi.ID.Equals(id))
				{
					TimeZone = zi;
				}
			}
		}

		/// <summary>
		/// Analyze the negative subpattern of DecimalFormat and set/update values
		/// as necessary.
		/// </summary>
		private void CheckNegativeNumberExpression()
		{
			if ((NumberFormat_Renamed is DecimalFormat) && !NumberFormat_Renamed.Equals(OriginalNumberFormat))
			{
				String numberPattern = ((DecimalFormat)NumberFormat_Renamed).ToPattern();
				if (!numberPattern.Equals(OriginalNumberPattern))
				{
					HasFollowingMinusSign = false;

					int separatorIndex = numberPattern.IndexOf(';');
					// If the negative subpattern is not absent, we have to analayze
					// it in order to check if it has a following minus sign.
					if (separatorIndex > -1)
					{
						int minusIndex = numberPattern.IndexOf('-', separatorIndex);
						if ((minusIndex > numberPattern.LastIndexOf('0')) && (minusIndex > numberPattern.LastIndexOf('#')))
						{
							HasFollowingMinusSign = true;
							MinusSign = ((DecimalFormat)NumberFormat_Renamed).DecimalFormatSymbols.MinusSign;
						}
					}
					OriginalNumberPattern = numberPattern;
				}
				OriginalNumberFormat = NumberFormat_Renamed;
			}
		}

	}

}