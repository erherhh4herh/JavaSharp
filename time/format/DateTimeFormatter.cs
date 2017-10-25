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
namespace java.time.format
{



	/// <summary>
	/// Formatter for printing and parsing date-time objects.
	/// <para>
	/// This class provides the main application entry point for printing and parsing
	/// and provides common implementations of {@code DateTimeFormatter}:
	/// <ul>
	/// <li>Using predefined constants, such as <seealso cref="#ISO_LOCAL_DATE"/></li>
	/// <li>Using pattern letters, such as {@code uuuu-MMM-dd}</li>
	/// <li>Using localized styles, such as {@code long} or {@code medium}</li>
	/// </ul>
	/// </para>
	/// <para>
	/// More complex formatters are provided by
	/// <seealso cref="DateTimeFormatterBuilder DateTimeFormatterBuilder"/>.
	/// 
	/// </para>
	/// <para>
	/// The main date-time classes provide two methods - one for formatting,
	/// {@code format(DateTimeFormatter formatter)}, and one for parsing,
	/// {@code parse(CharSequence text, DateTimeFormatter formatter)}.
	/// </para>
	/// <para>For example:
	/// <blockquote><pre>
	///  LocalDate date = LocalDate.now();
	///  String text = date.format(formatter);
	///  LocalDate parsedDate = LocalDate.parse(text, formatter);
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// In addition to the format, formatters can be created with desired Locale,
	/// Chronology, ZoneId, and DecimalStyle.
	/// </para>
	/// <para>
	/// The <seealso cref="#withLocale withLocale"/> method returns a new formatter that
	/// overrides the locale. The locale affects some aspects of formatting and
	/// parsing. For example, the <seealso cref="#ofLocalizedDate ofLocalizedDate"/> provides a
	/// formatter that uses the locale specific date format.
	/// </para>
	/// <para>
	/// The <seealso cref="#withChronology withChronology"/> method returns a new formatter
	/// that overrides the chronology. If overridden, the date-time value is
	/// converted to the chronology before formatting. During parsing the date-time
	/// value is converted to the chronology before it is returned.
	/// </para>
	/// <para>
	/// The <seealso cref="#withZone withZone"/> method returns a new formatter that overrides
	/// the zone. If overridden, the date-time value is converted to a ZonedDateTime
	/// with the requested ZoneId before formatting. During parsing the ZoneId is
	/// applied before the value is returned.
	/// </para>
	/// <para>
	/// The <seealso cref="#withDecimalStyle withDecimalStyle"/> method returns a new formatter that
	/// overrides the <seealso cref="DecimalStyle"/>. The DecimalStyle symbols are used for
	/// formatting and parsing.
	/// </para>
	/// <para>
	/// Some applications may need to use the older <seealso cref="Format java.text.Format"/>
	/// class for formatting. The <seealso cref="#toFormat()"/> method returns an
	/// implementation of {@code java.text.Format}.
	/// 
	/// <h3 id="predefined">Predefined Formatters</h3>
	/// <table summary="Predefined Formatters" cellpadding="2" cellspacing="3" border="0" >
	/// <thead>
	/// <tr class="tableSubHeadingColor">
	/// <th class="colFirst" align="left">Formatter</th>
	/// <th class="colFirst" align="left">Description</th>
	/// <th class="colLast" align="left">Example</th>
	/// </tr>
	/// </thead>
	/// <tbody>
	/// <tr class="rowColor">
	/// <td><seealso cref="#ofLocalizedDate ofLocalizedDate(dateStyle)"/> </td>
	/// <td> Formatter with date style from the locale </td>
	/// <td> '2011-12-03'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ofLocalizedTime ofLocalizedTime(timeStyle)"/> </td>
	/// <td> Formatter with time style from the locale </td>
	/// <td> '10:15:30'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ofLocalizedDateTime ofLocalizedDateTime(dateTimeStyle)"/> </td>
	/// <td> Formatter with a style for date and time from the locale</td>
	/// <td> '3 Jun 2008 11:05:30'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ofLocalizedDateTime ofLocalizedDateTime(dateStyle,timeStyle)"/>
	/// </td>
	/// <td> Formatter with date and time styles from the locale </td>
	/// <td> '3 Jun 2008 11:05'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#BASIC_ISO_DATE"/></td>
	/// <td>Basic ISO date </td> <td>'20111203'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_LOCAL_DATE"/></td>
	/// <td> ISO Local Date </td>
	/// <td>'2011-12-03'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_OFFSET_DATE"/></td>
	/// <td> ISO Date with offset </td>
	/// <td>'2011-12-03+01:00'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_DATE"/></td>
	/// <td> ISO Date with or without offset </td>
	/// <td> '2011-12-03+01:00'; '2011-12-03'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_LOCAL_TIME"/></td>
	/// <td> Time without offset </td>
	/// <td>'10:15:30'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_OFFSET_TIME"/></td>
	/// <td> Time with offset </td>
	/// <td>'10:15:30+01:00'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_TIME"/></td>
	/// <td> Time with or without offset </td>
	/// <td>'10:15:30+01:00'; '10:15:30'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_LOCAL_DATE_TIME"/></td>
	/// <td> ISO Local Date and Time </td>
	/// <td>'2011-12-03T10:15:30'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_OFFSET_DATE_TIME"/></td>
	/// <td> Date Time with Offset
	/// </td><td>2011-12-03T10:15:30+01:00'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_ZONED_DATE_TIME"/></td>
	/// <td> Zoned Date Time </td>
	/// <td>'2011-12-03T10:15:30+01:00[Europe/Paris]'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_DATE_TIME"/></td>
	/// <td> Date and time with ZoneId </td>
	/// <td>'2011-12-03T10:15:30+01:00[Europe/Paris]'</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_ORDINAL_DATE"/></td>
	/// <td> Year and day of year </td>
	/// <td>'2012-337'</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#ISO_WEEK_DATE"/></td>
	/// <td> Year and Week </td>
	/// <td>2012-W48-6'</td></tr>
	/// <tr class="altColor">
	/// <td> <seealso cref="#ISO_INSTANT"/></td>
	/// <td> Date and Time of an Instant </td>
	/// <td>'2011-12-03T10:15:30Z' </td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td> <seealso cref="#RFC_1123_DATE_TIME"/></td>
	/// <td> RFC 1123 / RFC 822 </td>
	/// <td>'Tue, 3 Jun 2008 11:05:30 GMT'</td>
	/// </tr>
	/// </tbody>
	/// </table>
	/// 
	/// <h3 id="patterns">Patterns for Formatting and Parsing</h3>
	/// Patterns are based on a simple sequence of letters and symbols.
	/// A pattern is used to create a Formatter using the
	/// <seealso cref="#ofPattern(String)"/> and <seealso cref="#ofPattern(String, Locale)"/> methods.
	/// For example,
	/// {@code "d MMM uuuu"} will format 2011-12-03 as '3&nbsp;Dec&nbsp;2011'.
	/// A formatter created from a pattern can be used as many times as necessary,
	/// it is immutable and is thread-safe.
	/// </para>
	/// <para>
	/// For example:
	/// <blockquote><pre>
	///  LocalDate date = LocalDate.now();
	///  DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy MM dd");
	///  String text = date.format(formatter);
	///  LocalDate parsedDate = LocalDate.parse(text, formatter);
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// All letters 'A' to 'Z' and 'a' to 'z' are reserved as pattern letters. The
	/// following pattern letters are defined:
	/// <pre>
	///  Symbol  Meaning                     Presentation      Examples
	///  ------  -------                     ------------      -------
	///   G       era                         text              AD; Anno Domini; A
	///   u       year                        year              2004; 04
	///   y       year-of-era                 year              2004; 04
	///   D       day-of-year                 number            189
	///   M/L     month-of-year               number/text       7; 07; Jul; July; J
	///   d       day-of-month                number            10
	/// 
	///   Q/q     quarter-of-year             number/text       3; 03; Q3; 3rd quarter
	///   Y       week-based-year             year              1996; 96
	///   w       week-of-week-based-year     number            27
	///   W       week-of-month               number            4
	///   E       day-of-week                 text              Tue; Tuesday; T
	///   e/c     localized day-of-week       number/text       2; 02; Tue; Tuesday; T
	///   F       week-of-month               number            3
	/// 
	///   a       am-pm-of-day                text              PM
	///   h       clock-hour-of-am-pm (1-12)  number            12
	///   K       hour-of-am-pm (0-11)        number            0
	///   k       clock-hour-of-am-pm (1-24)  number            0
	/// 
	///   H       hour-of-day (0-23)          number            0
	///   m       minute-of-hour              number            30
	///   s       second-of-minute            number            55
	///   S       fraction-of-second          fraction          978
	///   A       milli-of-day                number            1234
	///   n       nano-of-second              number            987654321
	///   N       nano-of-day                 number            1234000000
	/// 
	///   V       time-zone ID                zone-id           America/Los_Angeles; Z; -08:30
	///   z       time-zone name              zone-name         Pacific Standard Time; PST
	///   O       localized zone-offset       offset-O          GMT+8; GMT+08:00; UTC-08:00;
	///   X       zone-offset 'Z' for zero    offset-X          Z; -08; -0830; -08:30; -083015; -08:30:15;
	///   x       zone-offset                 offset-x          +0000; -08; -0830; -08:30; -083015; -08:30:15;
	///   Z       zone-offset                 offset-Z          +0000; -0800; -08:00;
	/// 
	///   p       pad next                    pad modifier      1
	/// 
	///   '       escape for text             delimiter
	///   ''      single quote                literal           '
	///   [       optional section start
	///   ]       optional section end
	///   #       reserved for future use
	///   {       reserved for future use
	///   }       reserved for future use
	/// </pre>
	/// </para>
	/// <para>
	/// The count of pattern letters determines the format.
	/// </para>
	/// <para>
	/// <b>Text</b>: The text style is determined based on the number of pattern
	/// letters used. Less than 4 pattern letters will use the
	/// <seealso cref="TextStyle#SHORT short form"/>. Exactly 4 pattern letters will use the
	/// <seealso cref="TextStyle#FULL full form"/>. Exactly 5 pattern letters will use the
	/// <seealso cref="TextStyle#NARROW narrow form"/>.
	/// Pattern letters 'L', 'c', and 'q' specify the stand-alone form of the text styles.
	/// </para>
	/// <para>
	/// <b>Number</b>: If the count of letters is one, then the value is output using
	/// the minimum number of digits and without padding. Otherwise, the count of digits
	/// is used as the width of the output field, with the value zero-padded as necessary.
	/// The following pattern letters have constraints on the count of letters.
	/// Only one letter of 'c' and 'F' can be specified.
	/// Up to two letters of 'd', 'H', 'h', 'K', 'k', 'm', and 's' can be specified.
	/// Up to three letters of 'D' can be specified.
	/// </para>
	/// <para>
	/// <b>Number/Text</b>: If the count of pattern letters is 3 or greater, use the
	/// Text rules above. Otherwise use the Number rules above.
	/// </para>
	/// <para>
	/// <b>Fraction</b>: Outputs the nano-of-second field as a fraction-of-second.
	/// The nano-of-second value has nine digits, thus the count of pattern letters
	/// is from 1 to 9. If it is less than 9, then the nano-of-second value is
	/// truncated, with only the most significant digits being output.
	/// </para>
	/// <para>
	/// <b>Year</b>: The count of letters determines the minimum field width below
	/// which padding is used. If the count of letters is two, then a
	/// <seealso cref="DateTimeFormatterBuilder#appendValueReduced reduced"/> two digit form is
	/// used. For printing, this outputs the rightmost two digits. For parsing, this
	/// will parse using the base value of 2000, resulting in a year within the range
	/// 2000 to 2099 inclusive. If the count of letters is less than four (but not
	/// two), then the sign is only output for negative years as per
	/// <seealso cref="SignStyle#NORMAL"/>. Otherwise, the sign is output if the pad width is
	/// exceeded, as per <seealso cref="SignStyle#EXCEEDS_PAD"/>.
	/// </para>
	/// <para>
	/// <b>ZoneId</b>: This outputs the time-zone ID, such as 'Europe/Paris'. If the
	/// count of letters is two, then the time-zone ID is output. Any other count of
	/// letters throws {@code IllegalArgumentException}.
	/// </para>
	/// <para>
	/// <b>Zone names</b>: This outputs the display name of the time-zone ID. If the
	/// count of letters is one, two or three, then the short name is output. If the
	/// count of letters is four, then the full name is output. Five or more letters
	/// throws {@code IllegalArgumentException}.
	/// </para>
	/// <para>
	/// <b>Offset X and x</b>: This formats the offset based on the number of pattern
	/// letters. One letter outputs just the hour, such as '+01', unless the minute
	/// is non-zero in which case the minute is also output, such as '+0130'. Two
	/// letters outputs the hour and minute, without a colon, such as '+0130'. Three
	/// letters outputs the hour and minute, with a colon, such as '+01:30'. Four
	/// letters outputs the hour and minute and optional second, without a colon,
	/// such as '+013015'. Five letters outputs the hour and minute and optional
	/// second, with a colon, such as '+01:30:15'. Six or more letters throws
	/// {@code IllegalArgumentException}. Pattern letter 'X' (upper case) will output
	/// 'Z' when the offset to be output would be zero, whereas pattern letter 'x'
	/// (lower case) will output '+00', '+0000', or '+00:00'.
	/// </para>
	/// <para>
	/// <b>Offset O</b>: This formats the localized offset based on the number of
	/// pattern letters. One letter outputs the <seealso cref="TextStyle#SHORT short"/>
	/// form of the localized offset, which is localized offset text, such as 'GMT',
	/// with hour without leading zero, optional 2-digit minute and second if
	/// non-zero, and colon, for example 'GMT+8'. Four letters outputs the
	/// <seealso cref="TextStyle#FULL full"/> form, which is localized offset text,
	/// such as 'GMT, with 2-digit hour and minute field, optional second field
	/// if non-zero, and colon, for example 'GMT+08:00'. Any other count of letters
	/// throws {@code IllegalArgumentException}.
	/// </para>
	/// <para>
	/// <b>Offset Z</b>: This formats the offset based on the number of pattern
	/// letters. One, two or three letters outputs the hour and minute, without a
	/// colon, such as '+0130'. The output will be '+0000' when the offset is zero.
	/// Four letters outputs the <seealso cref="TextStyle#FULL full"/> form of localized
	/// offset, equivalent to four letters of Offset-O. The output will be the
	/// corresponding localized offset text if the offset is zero. Five
	/// letters outputs the hour, minute, with optional second if non-zero, with
	/// colon. It outputs 'Z' if the offset is zero.
	/// Six or more letters throws {@code IllegalArgumentException}.
	/// </para>
	/// <para>
	/// <b>Optional section</b>: The optional section markers work exactly like
	/// calling <seealso cref="DateTimeFormatterBuilder#optionalStart()"/> and
	/// <seealso cref="DateTimeFormatterBuilder#optionalEnd()"/>.
	/// </para>
	/// <para>
	/// <b>Pad modifier</b>: Modifies the pattern that immediately follows to be
	/// padded with spaces. The pad width is determined by the number of pattern
	/// letters. This is the same as calling
	/// <seealso cref="DateTimeFormatterBuilder#padNext(int)"/>.
	/// </para>
	/// <para>
	/// For example, 'ppH' outputs the hour-of-day padded on the left with spaces to
	/// a width of 2.
	/// </para>
	/// <para>
	/// Any unrecognized letter is an error. Any non-letter character, other than
	/// '[', ']', '{', '}', '#' and the single quote will be output directly.
	/// Despite this, it is recommended to use single quotes around all characters
	/// that you want to output directly to ensure that future changes do not break
	/// your application.
	/// 
	/// <h3 id="resolving">Resolving</h3>
	/// Parsing is implemented as a two-phase operation.
	/// First, the text is parsed using the layout defined by the formatter, producing
	/// a {@code Map} of field to value, a {@code ZoneId} and a {@code Chronology}.
	/// Second, the parsed data is <em>resolved</em>, by validating, combining and
	/// simplifying the various fields into more useful ones.
	/// </para>
	/// <para>
	/// Five parsing methods are supplied by this class.
	/// Four of these perform both the parse and resolve phases.
	/// The fifth method, <seealso cref="#parseUnresolved(CharSequence, ParsePosition)"/>,
	/// only performs the first phase, leaving the result unresolved.
	/// As such, it is essentially a low-level operation.
	/// </para>
	/// <para>
	/// The resolve phase is controlled by two parameters, set on this class.
	/// </para>
	/// <para>
	/// The <seealso cref="ResolverStyle"/> is an enum that offers three different approaches,
	/// strict, smart and lenient. The smart option is the default.
	/// It can be set using <seealso cref="#withResolverStyle(ResolverStyle)"/>.
	/// </para>
	/// <para>
	/// The <seealso cref="#withResolverFields(TemporalField...)"/> parameter allows the
	/// set of fields that will be resolved to be filtered before resolving starts.
	/// For example, if the formatter has parsed a year, month, day-of-month
	/// and day-of-year, then there are two approaches to resolve a date:
	/// (year + month + day-of-month) and (year + day-of-year).
	/// The resolver fields allows one of the two approaches to be selected.
	/// If no resolver fields are set then both approaches must result in the same date.
	/// </para>
	/// <para>
	/// Resolving separate fields to form a complete date and time is a complex
	/// process with behaviour distributed across a number of classes.
	/// It follows these steps:
	/// <ol>
	/// <li>The chronology is determined.
	/// The chronology of the result is either the chronology that was parsed,
	/// or if no chronology was parsed, it is the chronology set on this class,
	/// or if that is null, it is {@code IsoChronology}.
	/// <li>The {@code ChronoField} date fields are resolved.
	/// This is achieved using <seealso cref="Chronology#resolveDate(Map, ResolverStyle)"/>.
	/// Documentation about field resolution is located in the implementation
	/// of {@code Chronology}.
	/// <li>The {@code ChronoField} time fields are resolved.
	/// This is documented on <seealso cref="ChronoField"/> and is the same for all chronologies.
	/// <li>Any fields that are not {@code ChronoField} are processed.
	/// This is achieved using <seealso cref="TemporalField#resolve(Map, TemporalAccessor, ResolverStyle)"/>.
	/// Documentation about field resolution is located in the implementation
	/// of {@code TemporalField}.
	/// <li>The {@code ChronoField} date and time fields are re-resolved.
	/// This allows fields in step four to produce {@code ChronoField} values
	/// and have them be processed into dates and times.
	/// <li>A {@code LocalTime} is formed if there is at least an hour-of-day available.
	/// This involves providing default values for minute, second and fraction of second.
	/// <li>Any remaining unresolved fields are cross-checked against any
	/// date and/or time that was resolved. Thus, an earlier stage would resolve
	/// (year + month + day-of-month) to a date, and this stage would check that
	/// day-of-week was valid for the date.
	/// <li>If an <seealso cref="#parsedExcessDays() excess number of days"/>
	/// was parsed then it is added to the date if a date is available.
	/// </ol>
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class DateTimeFormatter
	{

		/// <summary>
		/// The printer and/or parser to use, not null.
		/// </summary>
		private readonly CompositePrinterParser PrinterParser;
		/// <summary>
		/// The locale to use for formatting, not null.
		/// </summary>
		private readonly Locale Locale_Renamed;
		/// <summary>
		/// The symbols to use for formatting, not null.
		/// </summary>
		private readonly DecimalStyle DecimalStyle_Renamed;
		/// <summary>
		/// The resolver style to use, not null.
		/// </summary>
		private readonly ResolverStyle ResolverStyle_Renamed;
		/// <summary>
		/// The fields to use in resolving, null for all fields.
		/// </summary>
		private readonly Set<TemporalField> ResolverFields_Renamed;
		/// <summary>
		/// The chronology to use for formatting, null for no override.
		/// </summary>
		private readonly Chronology Chrono;
		/// <summary>
		/// The zone to use for formatting, null for no override.
		/// </summary>
		private readonly ZoneId Zone_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates a formatter using the specified pattern.
		/// <para>
		/// This method will create a formatter based on a simple
		/// <a href="#patterns">pattern of letters and symbols</a>
		/// as described in the class documentation.
		/// For example, {@code d MMM uuuu} will format 2011-12-03 as '3 Dec 2011'.
		/// </para>
		/// <para>
		/// The formatter will use the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		/// This can be changed using <seealso cref="DateTimeFormatter#withLocale(Locale)"/> on the returned formatter
		/// Alternatively use the <seealso cref="#ofPattern(String, Locale)"/> variant of this method.
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern to use, not null </param>
		/// <returns> the formatter based on the pattern, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		/// <seealso cref= DateTimeFormatterBuilder#appendPattern(String) </seealso>
		public static DateTimeFormatter OfPattern(String pattern)
		{
			return (new DateTimeFormatterBuilder()).AppendPattern(pattern).ToFormatter();
		}

		/// <summary>
		/// Creates a formatter using the specified pattern and locale.
		/// <para>
		/// This method will create a formatter based on a simple
		/// <a href="#patterns">pattern of letters and symbols</a>
		/// as described in the class documentation.
		/// For example, {@code d MMM uuuu} will format 2011-12-03 as '3 Dec 2011'.
		/// </para>
		/// <para>
		/// The formatter will use the specified locale.
		/// This can be changed using <seealso cref="DateTimeFormatter#withLocale(Locale)"/> on the returned formatter
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern to use, not null </param>
		/// <param name="locale">  the locale to use, not null </param>
		/// <returns> the formatter based on the pattern, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		/// <seealso cref= DateTimeFormatterBuilder#appendPattern(String) </seealso>
		public static DateTimeFormatter OfPattern(String pattern, Locale locale)
		{
			return (new DateTimeFormatterBuilder()).AppendPattern(pattern).ToFormatter(locale);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a locale specific date format for the ISO chronology.
		/// <para>
		/// This returns a formatter that will format or parse a date.
		/// The exact format pattern used varies by locale.
		/// </para>
		/// <para>
		/// The locale is determined from the formatter. The formatter returned directly by
		/// this method will use the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		/// The locale can be controlled using <seealso cref="DateTimeFormatter#withLocale(Locale) withLocale(Locale)"/>
		/// on the result of this method.
		/// </para>
		/// <para>
		/// Note that the localized pattern is looked up lazily.
		/// This {@code DateTimeFormatter} holds the style required and the locale,
		/// looking up the pattern required on demand.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateStyle">  the formatter style to obtain, not null </param>
		/// <returns> the date formatter, not null </returns>
		public static DateTimeFormatter OfLocalizedDate(FormatStyle dateStyle)
		{
			Objects.RequireNonNull(dateStyle, "dateStyle");
			return (new DateTimeFormatterBuilder()).AppendLocalized(dateStyle, null).ToFormatter(ResolverStyle.SMART, IsoChronology.INSTANCE);
		}

		/// <summary>
		/// Returns a locale specific time format for the ISO chronology.
		/// <para>
		/// This returns a formatter that will format or parse a time.
		/// The exact format pattern used varies by locale.
		/// </para>
		/// <para>
		/// The locale is determined from the formatter. The formatter returned directly by
		/// this method will use the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		/// The locale can be controlled using <seealso cref="DateTimeFormatter#withLocale(Locale) withLocale(Locale)"/>
		/// on the result of this method.
		/// </para>
		/// <para>
		/// Note that the localized pattern is looked up lazily.
		/// This {@code DateTimeFormatter} holds the style required and the locale,
		/// looking up the pattern required on demand.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeStyle">  the formatter style to obtain, not null </param>
		/// <returns> the time formatter, not null </returns>
		public static DateTimeFormatter OfLocalizedTime(FormatStyle timeStyle)
		{
			Objects.RequireNonNull(timeStyle, "timeStyle");
			return (new DateTimeFormatterBuilder()).AppendLocalized(null, timeStyle).ToFormatter(ResolverStyle.SMART, IsoChronology.INSTANCE);
		}

		/// <summary>
		/// Returns a locale specific date-time formatter for the ISO chronology.
		/// <para>
		/// This returns a formatter that will format or parse a date-time.
		/// The exact format pattern used varies by locale.
		/// </para>
		/// <para>
		/// The locale is determined from the formatter. The formatter returned directly by
		/// this method will use the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		/// The locale can be controlled using <seealso cref="DateTimeFormatter#withLocale(Locale) withLocale(Locale)"/>
		/// on the result of this method.
		/// </para>
		/// <para>
		/// Note that the localized pattern is looked up lazily.
		/// This {@code DateTimeFormatter} holds the style required and the locale,
		/// looking up the pattern required on demand.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateTimeStyle">  the formatter style to obtain, not null </param>
		/// <returns> the date-time formatter, not null </returns>
		public static DateTimeFormatter OfLocalizedDateTime(FormatStyle dateTimeStyle)
		{
			Objects.RequireNonNull(dateTimeStyle, "dateTimeStyle");
			return (new DateTimeFormatterBuilder()).AppendLocalized(dateTimeStyle, dateTimeStyle).ToFormatter(ResolverStyle.SMART, IsoChronology.INSTANCE);
		}

		/// <summary>
		/// Returns a locale specific date and time format for the ISO chronology.
		/// <para>
		/// This returns a formatter that will format or parse a date-time.
		/// The exact format pattern used varies by locale.
		/// </para>
		/// <para>
		/// The locale is determined from the formatter. The formatter returned directly by
		/// this method will use the <seealso cref="Locale#getDefault() default FORMAT locale"/>.
		/// The locale can be controlled using <seealso cref="DateTimeFormatter#withLocale(Locale) withLocale(Locale)"/>
		/// on the result of this method.
		/// </para>
		/// <para>
		/// Note that the localized pattern is looked up lazily.
		/// This {@code DateTimeFormatter} holds the style required and the locale,
		/// looking up the pattern required on demand.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateStyle">  the date formatter style to obtain, not null </param>
		/// <param name="timeStyle">  the time formatter style to obtain, not null </param>
		/// <returns> the date, time or date-time formatter, not null </returns>
		public static DateTimeFormatter OfLocalizedDateTime(FormatStyle dateStyle, FormatStyle timeStyle)
		{
			Objects.RequireNonNull(dateStyle, "dateStyle");
			Objects.RequireNonNull(timeStyle, "timeStyle");
			return (new DateTimeFormatterBuilder()).AppendLocalized(dateStyle, timeStyle).ToFormatter(ResolverStyle.SMART, IsoChronology.INSTANCE);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses a date without an
		/// offset, such as '2011-12-03'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended local date format.
		/// The format consists of:
		/// <ul>
		/// <li>Four digits or more for the <seealso cref="ChronoField#YEAR year"/>.
		/// Years in the range 0000 to 9999 will be pre-padded by zero to ensure four digits.
		/// Years outside that range will have a prefixed positive or negative symbol.
		/// <li>A dash
		/// <li>Two digits for the <seealso cref="ChronoField#MONTH_OF_YEAR month-of-year"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>A dash
		/// <li>Two digits for the <seealso cref="ChronoField#DAY_OF_MONTH day-of-month"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_LOCAL_DATE;
		static DateTimeFormatter()
		{
			ISO_LOCAL_DATE = (new DateTimeFormatterBuilder()).AppendValue(YEAR, 4, 10, SignStyle.EXCEEDS_PAD).AppendLiteral('-').AppendValue(MONTH_OF_YEAR, 2).AppendLiteral('-').AppendValue(DAY_OF_MONTH, 2).ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_OFFSET_DATE = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_DATE).AppendOffsetId().ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_DATE = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_DATE).OptionalStart().AppendOffsetId().ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_LOCAL_TIME = (new DateTimeFormatterBuilder()).AppendValue(HOUR_OF_DAY, 2).AppendLiteral(':').AppendValue(MINUTE_OF_HOUR, 2).OptionalStart().AppendLiteral(':').AppendValue(SECOND_OF_MINUTE, 2).OptionalStart().AppendFraction(NANO_OF_SECOND, 0, 9, true).ToFormatter(ResolverStyle.STRICT, null);
			ISO_OFFSET_TIME = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_TIME).AppendOffsetId().ToFormatter(ResolverStyle.STRICT, null);
			ISO_TIME = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_TIME).OptionalStart().AppendOffsetId().ToFormatter(ResolverStyle.STRICT, null);
			ISO_LOCAL_DATE_TIME = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_DATE).AppendLiteral('T').Append(ISO_LOCAL_TIME).ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_OFFSET_DATE_TIME = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().Append(ISO_LOCAL_DATE_TIME).AppendOffsetId().ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_ZONED_DATE_TIME = (new DateTimeFormatterBuilder()).Append(ISO_OFFSET_DATE_TIME).OptionalStart().AppendLiteral('[').ParseCaseSensitive().AppendZoneRegionId().AppendLiteral(']').ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_DATE_TIME = (new DateTimeFormatterBuilder()).Append(ISO_LOCAL_DATE_TIME).OptionalStart().AppendOffsetId().OptionalStart().AppendLiteral('[').ParseCaseSensitive().AppendZoneRegionId().AppendLiteral(']').ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_ORDINAL_DATE = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().AppendValue(YEAR, 4, 10, SignStyle.EXCEEDS_PAD).AppendLiteral('-').AppendValue(DAY_OF_YEAR, 3).OptionalStart().AppendOffsetId().ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_WEEK_DATE = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().AppendValue(IsoFields.WEEK_BASED_YEAR, 4, 10, SignStyle.EXCEEDS_PAD).AppendLiteral("-W").AppendValue(IsoFields.WEEK_OF_WEEK_BASED_YEAR, 2).AppendLiteral('-').AppendValue(DAY_OF_WEEK, 1).OptionalStart().AppendOffsetId().ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			ISO_INSTANT = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().AppendInstant().ToFormatter(ResolverStyle.STRICT, null);
			BASIC_ISO_DATE = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().AppendValue(YEAR, 4).AppendValue(MONTH_OF_YEAR, 2).AppendValue(DAY_OF_MONTH, 2).OptionalStart().AppendOffset("+HHMMss", "Z").ToFormatter(ResolverStyle.STRICT, IsoChronology.INSTANCE);
			// manually code maps to ensure correct data always used
			// (locale data can be changed by application code)
			IDictionary<Long, String> dow = new Dictionary<Long, String>();
			dow[1L] = "Mon";
			dow[2L] = "Tue";
			dow[3L] = "Wed";
			dow[4L] = "Thu";
			dow[5L] = "Fri";
			dow[6L] = "Sat";
			dow[7L] = "Sun";
			IDictionary<Long, String> moy = new Dictionary<Long, String>();
			moy[1L] = "Jan";
			moy[2L] = "Feb";
			moy[3L] = "Mar";
			moy[4L] = "Apr";
			moy[5L] = "May";
			moy[6L] = "Jun";
			moy[7L] = "Jul";
			moy[8L] = "Aug";
			moy[9L] = "Sep";
			moy[10L] = "Oct";
			moy[11L] = "Nov";
			moy[12L] = "Dec";
			RFC_1123_DATE_TIME = (new DateTimeFormatterBuilder()).ParseCaseInsensitive().ParseLenient().OptionalStart().AppendText(DAY_OF_WEEK, dow).AppendLiteral(", ").OptionalEnd().AppendValue(DAY_OF_MONTH, 1, 2, SignStyle.NOT_NEGATIVE).AppendLiteral(' ').AppendText(MONTH_OF_YEAR, moy).AppendLiteral(' ').AppendValue(YEAR, 4).AppendLiteral(' ').AppendValue(HOUR_OF_DAY, 2).AppendLiteral(':').AppendValue(MINUTE_OF_HOUR, 2).OptionalStart().AppendLiteral(':').AppendValue(SECOND_OF_MINUTE, 2).OptionalEnd().AppendLiteral(' ').AppendOffset("+HHMM", "GMT").ToFormatter(ResolverStyle.SMART, IsoChronology.INSTANCE); // should handle UT/Z/EST/EDT/CST/CDT/MST/MDT/PST/MDT -  2 digit year not handled
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses a date with an
		/// offset, such as '2011-12-03+01:00'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended offset date format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_DATE"/>
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_OFFSET_DATE;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses a date with the
		/// offset if available, such as '2011-12-03' or '2011-12-03+01:00'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended date format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_DATE"/>
		/// <li>If the offset is not available then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_DATE;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO time formatter that formats or parses a time without an
		/// offset, such as '10:15' or '10:15:30'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended local time format.
		/// The format consists of:
		/// <ul>
		/// <li>Two digits for the <seealso cref="ChronoField#HOUR_OF_DAY hour-of-day"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>A colon
		/// <li>Two digits for the <seealso cref="ChronoField#MINUTE_OF_HOUR minute-of-hour"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>If the second-of-minute is not available then the format is complete.
		/// <li>A colon
		/// <li>Two digits for the <seealso cref="ChronoField#SECOND_OF_MINUTE second-of-minute"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>If the nano-of-second is zero or not available then the format is complete.
		/// <li>A decimal point
		/// <li>One to nine digits for the <seealso cref="ChronoField#NANO_OF_SECOND nano-of-second"/>.
		///  As many digits will be output as required.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_LOCAL_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO time formatter that formats or parses a time with an
		/// offset, such as '10:15+01:00' or '10:15:30+01:00'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended offset time format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_TIME"/>
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_OFFSET_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO time formatter that formats or parses a time, with the
		/// offset if available, such as '10:15', '10:15:30' or '10:15:30+01:00'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended offset time format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_TIME"/>
		/// <li>If the offset is not available then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date-time formatter that formats or parses a date-time without
		/// an offset, such as '2011-12-03T10:15:30'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended offset date-time format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_DATE"/>
		/// <li>The letter 'T'. Parsing is case insensitive.
		/// <li>The <seealso cref="#ISO_LOCAL_TIME"/>
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_LOCAL_DATE_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date-time formatter that formats or parses a date-time with an
		/// offset, such as '2011-12-03T10:15:30+01:00'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended offset date-time format.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_DATE_TIME"/>
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_OFFSET_DATE_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO-like date-time formatter that formats or parses a date-time with
		/// offset and zone, such as '2011-12-03T10:15:30+01:00[Europe/Paris]'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// a format that extends the ISO-8601 extended offset date-time format
		/// to add the time-zone.
		/// The section in square brackets is not part of the ISO-8601 standard.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_OFFSET_DATE_TIME"/>
		/// <li>If the zone ID is not available or is a {@code ZoneOffset} then the format is complete.
		/// <li>An open square bracket '['.
		/// <li>The <seealso cref="ZoneId#getId() zone ID"/>. This is not part of the ISO-8601 standard.
		///  Parsing is case sensitive.
		/// <li>A close square bracket ']'.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_ZONED_DATE_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO-like date-time formatter that formats or parses a date-time with
		/// the offset and zone if available, such as '2011-12-03T10:15:30',
		/// '2011-12-03T10:15:30+01:00' or '2011-12-03T10:15:30+01:00[Europe/Paris]'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended local or offset date-time format, as well as the
		/// extended non-ISO form specifying the time-zone.
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_LOCAL_DATE_TIME"/>
		/// <li>If the offset is not available to format or parse then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		/// <li>If the zone ID is not available or is a {@code ZoneOffset} then the format is complete.
		/// <li>An open square bracket '['.
		/// <li>The <seealso cref="ZoneId#getId() zone ID"/>. This is not part of the ISO-8601 standard.
		///  Parsing is case sensitive.
		/// <li>A close square bracket ']'.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_DATE_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses the ordinal date
		/// without an offset, such as '2012-337'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended ordinal date format.
		/// The format consists of:
		/// <ul>
		/// <li>Four digits or more for the <seealso cref="ChronoField#YEAR year"/>.
		/// Years in the range 0000 to 9999 will be pre-padded by zero to ensure four digits.
		/// Years outside that range will have a prefixed positive or negative symbol.
		/// <li>A dash
		/// <li>Three digits for the <seealso cref="ChronoField#DAY_OF_YEAR day-of-year"/>.
		///  This is pre-padded by zero to ensure three digits.
		/// <li>If the offset is not available to format or parse then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_ORDINAL_DATE;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses the week-based date
		/// without an offset, such as '2012-W48-6'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 extended week-based date format.
		/// The format consists of:
		/// <ul>
		/// <li>Four digits or more for the <seealso cref="IsoFields#WEEK_BASED_YEAR week-based-year"/>.
		/// Years in the range 0000 to 9999 will be pre-padded by zero to ensure four digits.
		/// Years outside that range will have a prefixed positive or negative symbol.
		/// <li>A dash
		/// <li>The letter 'W'. Parsing is case insensitive.
		/// <li>Two digits for the <seealso cref="IsoFields#WEEK_OF_WEEK_BASED_YEAR week-of-week-based-year"/>.
		///  This is pre-padded by zero to ensure three digits.
		/// <li>A dash
		/// <li>One digit for the <seealso cref="ChronoField#DAY_OF_WEEK day-of-week"/>.
		///  The value run from Monday (1) to Sunday (7).
		/// <li>If the offset is not available to format or parse then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/>. If the offset has seconds then
		///  they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_WEEK_DATE;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO instant formatter that formats or parses an instant in UTC,
		/// such as '2011-12-03T10:15:30Z'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 instant format.
		/// When formatting, the second-of-minute is always output.
		/// The nano-of-second outputs zero, three, six or nine digits digits as necessary.
		/// When parsing, time to at least the seconds field is required.
		/// Fractional seconds from zero to nine are parsed.
		/// The localized decimal style is not used.
		/// </para>
		/// <para>
		/// This is a special case formatter intended to allow a human readable form
		/// of an <seealso cref="java.time.Instant"/>. The {@code Instant} class is designed to
		/// only represent a point in time and internally stores a value in nanoseconds
		/// from a fixed epoch of 1970-01-01Z. As such, an {@code Instant} cannot be
		/// formatted as a date or time without providing some form of time-zone.
		/// This formatter allows the {@code Instant} to be formatted, by providing
		/// a suitable conversion using {@code ZoneOffset.UTC}.
		/// </para>
		/// <para>
		/// The format consists of:
		/// <ul>
		/// <li>The <seealso cref="#ISO_OFFSET_DATE_TIME"/> where the instant is converted from
		///  <seealso cref="ChronoField#INSTANT_SECONDS"/> and <seealso cref="ChronoField#NANO_OF_SECOND"/>
		///  using the {@code UTC} offset. Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// The returned formatter has no override chronology or zone.
		/// It uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter ISO_INSTANT;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The ISO date formatter that formats or parses a date without an
		/// offset, such as '20111203'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// the ISO-8601 basic local date format.
		/// The format consists of:
		/// <ul>
		/// <li>Four digits for the <seealso cref="ChronoField#YEAR year"/>.
		///  Only years in the range 0000 to 9999 are supported.
		/// <li>Two digits for the <seealso cref="ChronoField#MONTH_OF_YEAR month-of-year"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>Two digits for the <seealso cref="ChronoField#DAY_OF_MONTH day-of-month"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>If the offset is not available to format or parse then the format is complete.
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/> without colons. If the offset has
		///  seconds then they will be handled even though this is not part of the ISO-8601 standard.
		///  Parsing is case insensitive.
		/// </ul>
		/// </para>
		/// <para>
		/// As this formatter has an optional element, it may be necessary to parse using
		/// <seealso cref="DateTimeFormatter#parseBest"/>.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#STRICT STRICT"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter BASIC_ISO_DATE;

		//-----------------------------------------------------------------------
		/// <summary>
		/// The RFC-1123 date-time formatter, such as 'Tue, 3 Jun 2008 11:05:30 GMT'.
		/// <para>
		/// This returns an immutable formatter capable of formatting and parsing
		/// most of the RFC-1123 format.
		/// RFC-1123 updates RFC-822 changing the year from two digits to four.
		/// This implementation requires a four digit year.
		/// This implementation also does not handle North American or military zone
		/// names, only 'GMT' and offset amounts.
		/// </para>
		/// <para>
		/// The format consists of:
		/// <ul>
		/// <li>If the day-of-week is not available to format or parse then jump to day-of-month.
		/// <li>Three letter <seealso cref="ChronoField#DAY_OF_WEEK day-of-week"/> in English.
		/// <li>A comma
		/// <li>A space
		/// <li>One or two digits for the <seealso cref="ChronoField#DAY_OF_MONTH day-of-month"/>.
		/// <li>A space
		/// <li>Three letter <seealso cref="ChronoField#MONTH_OF_YEAR month-of-year"/> in English.
		/// <li>A space
		/// <li>Four digits for the <seealso cref="ChronoField#YEAR year"/>.
		///  Only years in the range 0000 to 9999 are supported.
		/// <li>A space
		/// <li>Two digits for the <seealso cref="ChronoField#HOUR_OF_DAY hour-of-day"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>A colon
		/// <li>Two digits for the <seealso cref="ChronoField#MINUTE_OF_HOUR minute-of-hour"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>If the second-of-minute is not available then jump to the next space.
		/// <li>A colon
		/// <li>Two digits for the <seealso cref="ChronoField#SECOND_OF_MINUTE second-of-minute"/>.
		///  This is pre-padded by zero to ensure two digits.
		/// <li>A space
		/// <li>The <seealso cref="ZoneOffset#getId() offset ID"/> without colons or seconds.
		///  An offset of zero uses "GMT". North American zone names and military zone names are not handled.
		/// </ul>
		/// </para>
		/// <para>
		/// Parsing is case insensitive.
		/// </para>
		/// <para>
		/// The returned formatter has a chronology of ISO set to ensure dates in
		/// other calendar systems are correctly converted.
		/// It has no override zone and uses the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// </para>
		/// </summary>
		public static readonly DateTimeFormatter RFC_1123_DATE_TIME;

		//-----------------------------------------------------------------------
		/// <summary>
		/// A query that provides access to the excess days that were parsed.
		/// <para>
		/// This returns a singleton <seealso cref="TemporalQuery query"/> that provides
		/// access to additional information from the parse. The query always returns
		/// a non-null period, with a zero period returned instead of null.
		/// </para>
		/// <para>
		/// There are two situations where this query may return a non-zero period.
		/// <ul>
		/// <li>If the {@code ResolverStyle} is {@code LENIENT} and a time is parsed
		///  without a date, then the complete result of the parse consists of a
		///  {@code LocalTime} and an excess {@code Period} in days.
		/// 
		/// <li>If the {@code ResolverStyle} is {@code SMART} and a time is parsed
		///  without a date where the time is 24:00:00, then the complete result of
		///  the parse consists of a {@code LocalTime} of 00:00:00 and an excess
		///  {@code Period} of one day.
		/// </ul>
		/// </para>
		/// <para>
		/// In both cases, if a complete {@code ChronoLocalDateTime} or {@code Instant}
		/// is parsed, then the excess days are added to the date part.
		/// As a result, this query will return a zero period.
		/// </para>
		/// <para>
		/// The {@code SMART} behaviour handles the common "end of day" 24:00 value.
		/// Processing in {@code LENIENT} mode also produces the same result:
		/// <pre>
		///  Text to parse        Parsed object                         Excess days
		///  "2012-12-03T00:00"   LocalDateTime.of(2012, 12, 3, 0, 0)   ZERO
		///  "2012-12-03T24:00"   LocalDateTime.of(2012, 12, 4, 0, 0)   ZERO
		///  "00:00"              LocalTime.of(0, 0)                    ZERO
		///  "24:00"              LocalTime.of(0, 0)                    Period.ofDays(1)
		/// </pre>
		/// The query can be used as follows:
		/// <pre>
		///  TemporalAccessor parsed = formatter.parse(str);
		///  LocalTime time = parsed.query(LocalTime::from);
		///  Period extraDays = parsed.query(DateTimeFormatter.parsedExcessDays());
		/// </pre>
		/// </para>
		/// </summary>
		/// <returns> a query that provides access to the excess days that were parsed </returns>
		public static TemporalQuery<Period> ParsedExcessDays()
		{
			return PARSED_EXCESS_DAYS;
		}
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		private static readonly TemporalQuery<Period> PARSED_EXCESS_DAYS = t =>
		{
			if (t is Parsed)
			{
				return ((Parsed) t).ExcessDays;
			}
			else
			{
				return Period.ZERO;
			}
		};

		/// <summary>
		/// A query that provides access to whether a leap-second was parsed.
		/// <para>
		/// This returns a singleton <seealso cref="TemporalQuery query"/> that provides
		/// access to additional information from the parse. The query always returns
		/// a non-null boolean, true if parsing saw a leap-second, false if not.
		/// </para>
		/// <para>
		/// Instant parsing handles the special "leap second" time of '23:59:60'.
		/// Leap seconds occur at '23:59:60' in the UTC time-zone, but at other
		/// local times in different time-zones. To avoid this potential ambiguity,
		/// the handling of leap-seconds is limited to
		/// <seealso cref="DateTimeFormatterBuilder#appendInstant()"/>, as that method
		/// always parses the instant with the UTC zone offset.
		/// </para>
		/// <para>
		/// If the time '23:59:60' is received, then a simple conversion is applied,
		/// replacing the second-of-minute of 60 with 59. This query can be used
		/// on the parse result to determine if the leap-second adjustment was made.
		/// The query will return {@code true} if it did adjust to remove the
		/// leap-second, and {@code false} if not. Note that applying a leap-second
		/// smoothing mechanism, such as UTC-SLS, is the responsibility of the
		/// application, as follows:
		/// <pre>
		///  TemporalAccessor parsed = formatter.parse(str);
		///  Instant instant = parsed.query(Instant::from);
		///  if (parsed.query(DateTimeFormatter.parsedLeapSecond())) {
		///    // validate leap-second is correct and apply correct smoothing
		///  }
		/// </pre>
		/// </para>
		/// </summary>
		/// <returns> a query that provides access to whether a leap-second was parsed </returns>
		public static TemporalQuery<Boolean> ParsedLeapSecond()
		{
			return PARSED_LEAP_SECOND;
		}
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		private static readonly TemporalQuery<Boolean> PARSED_LEAP_SECOND = t =>
		{
			if (t is Parsed)
			{
				return ((Parsed) t).LeapSecond;
			}
			else
			{
				return false;
			}
		};

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="printerParser">  the printer/parser to use, not null </param>
		/// <param name="locale">  the locale to use, not null </param>
		/// <param name="decimalStyle">  the DecimalStyle to use, not null </param>
		/// <param name="resolverStyle">  the resolver style to use, not null </param>
		/// <param name="resolverFields">  the fields to use during resolving, null for all fields </param>
		/// <param name="chrono">  the chronology to use, null for no override </param>
		/// <param name="zone">  the zone to use, null for no override </param>
		internal DateTimeFormatter(CompositePrinterParser printerParser, Locale locale, DecimalStyle decimalStyle, ResolverStyle resolverStyle, Set<TemporalField> resolverFields, Chronology chrono, ZoneId zone)
		{
			this.PrinterParser = Objects.RequireNonNull(printerParser, "printerParser");
			this.ResolverFields_Renamed = resolverFields;
			this.Locale_Renamed = Objects.RequireNonNull(locale, "locale");
			this.DecimalStyle_Renamed = Objects.RequireNonNull(decimalStyle, "decimalStyle");
			this.ResolverStyle_Renamed = Objects.RequireNonNull(resolverStyle, "resolverStyle");
			this.Chrono = chrono;
			this.Zone_Renamed = zone;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the locale to be used during formatting.
		/// <para>
		/// This is used to lookup any part of the formatter needing specific
		/// localization, such as the text or localized pattern.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the locale of this formatter, not null </returns>
		public Locale Locale
		{
			get
			{
				return Locale_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new locale.
		/// <para>
		/// This is used to lookup any part of the formatter needing specific
		/// localization, such as the text or localized pattern.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale">  the new locale, not null </param>
		/// <returns> a formatter based on this formatter with the requested locale, not null </returns>
		public DateTimeFormatter WithLocale(Locale locale)
		{
			if (this.Locale_Renamed.Equals(locale))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, locale, DecimalStyle_Renamed, ResolverStyle_Renamed, ResolverFields_Renamed, Chrono, Zone_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the DecimalStyle to be used during formatting.
		/// </summary>
		/// <returns> the locale of this formatter, not null </returns>
		public DecimalStyle DecimalStyle
		{
			get
			{
				return DecimalStyle_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new DecimalStyle.
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="decimalStyle">  the new DecimalStyle, not null </param>
		/// <returns> a formatter based on this formatter with the requested DecimalStyle, not null </returns>
		public DateTimeFormatter WithDecimalStyle(DecimalStyle decimalStyle)
		{
			if (this.DecimalStyle_Renamed.Equals(decimalStyle))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, decimalStyle, ResolverStyle_Renamed, ResolverFields_Renamed, Chrono, Zone_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the overriding chronology to be used during formatting.
		/// <para>
		/// This returns the override chronology, used to convert dates.
		/// By default, a formatter has no override chronology, returning null.
		/// See <seealso cref="#withChronology(Chronology)"/> for more details on overriding.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the override chronology of this formatter, null if no override </returns>
		public Chronology Chronology
		{
			get
			{
				return Chrono;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new override chronology.
		/// <para>
		/// This returns a formatter with similar state to this formatter but
		/// with the override chronology set.
		/// By default, a formatter has no override chronology, returning null.
		/// </para>
		/// <para>
		/// If an override is added, then any date that is formatted or parsed will be affected.
		/// </para>
		/// <para>
		/// When formatting, if the temporal object contains a date, then it will
		/// be converted to a date in the override chronology.
		/// Whether the temporal contains a date is determined by querying the
		/// <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> field.
		/// Any time or zone will be retained unaltered unless overridden.
		/// </para>
		/// <para>
		/// If the temporal object does not contain a date, but does contain one
		/// or more {@code ChronoField} date fields, then a {@code DateTimeException}
		/// is thrown. In all other cases, the override chronology is added to the temporal,
		/// replacing any previous chronology, but without changing the date/time.
		/// </para>
		/// <para>
		/// When parsing, there are two distinct cases to consider.
		/// If a chronology has been parsed directly from the text, perhaps because
		/// <seealso cref="DateTimeFormatterBuilder#appendChronologyId()"/> was used, then
		/// this override chronology has no effect.
		/// If no zone has been parsed, then this override chronology will be used
		/// to interpret the {@code ChronoField} values into a date according to the
		/// date resolving rules of the chronology.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chrono">  the new chronology, null if no override </param>
		/// <returns> a formatter based on this formatter with the requested override chronology, not null </returns>
		public DateTimeFormatter WithChronology(Chronology chrono)
		{
			if (Objects.Equals(this.Chrono, chrono))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, DecimalStyle_Renamed, ResolverStyle_Renamed, ResolverFields_Renamed, chrono, Zone_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the overriding zone to be used during formatting.
		/// <para>
		/// This returns the override zone, used to convert instants.
		/// By default, a formatter has no override zone, returning null.
		/// See <seealso cref="#withZone(ZoneId)"/> for more details on overriding.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the override zone of this formatter, null if no override </returns>
		public ZoneId Zone
		{
			get
			{
				return Zone_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new override zone.
		/// <para>
		/// This returns a formatter with similar state to this formatter but
		/// with the override zone set.
		/// By default, a formatter has no override zone, returning null.
		/// </para>
		/// <para>
		/// If an override is added, then any instant that is formatted or parsed will be affected.
		/// </para>
		/// <para>
		/// When formatting, if the temporal object contains an instant, then it will
		/// be converted to a zoned date-time using the override zone.
		/// Whether the temporal is an instant is determined by querying the
		/// <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/> field.
		/// If the input has a chronology then it will be retained unless overridden.
		/// If the input does not have a chronology, such as {@code Instant}, then
		/// the ISO chronology will be used.
		/// </para>
		/// <para>
		/// If the temporal object does not contain an instant, but does contain
		/// an offset then an additional check is made. If the normalized override
		/// zone is an offset that differs from the offset of the temporal, then
		/// a {@code DateTimeException} is thrown. In all other cases, the override
		/// zone is added to the temporal, replacing any previous zone, but without
		/// changing the date/time.
		/// </para>
		/// <para>
		/// When parsing, there are two distinct cases to consider.
		/// If a zone has been parsed directly from the text, perhaps because
		/// <seealso cref="DateTimeFormatterBuilder#appendZoneId()"/> was used, then
		/// this override zone has no effect.
		/// If no zone has been parsed, then this override zone will be included in
		/// the result of the parse where it can be used to build instants and date-times.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the new override zone, null if no override </param>
		/// <returns> a formatter based on this formatter with the requested override zone, not null </returns>
		public DateTimeFormatter WithZone(ZoneId zone)
		{
			if (Objects.Equals(this.Zone_Renamed, zone))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, DecimalStyle_Renamed, ResolverStyle_Renamed, ResolverFields_Renamed, Chrono, zone);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the resolver style to use during parsing.
		/// <para>
		/// This returns the resolver style, used during the second phase of parsing
		/// when fields are resolved into dates and times.
		/// By default, a formatter has the <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// See <seealso cref="#withResolverStyle(ResolverStyle)"/> for more details.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the resolver style of this formatter, not null </returns>
		public ResolverStyle ResolverStyle
		{
			get
			{
				return ResolverStyle_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new resolver style.
		/// <para>
		/// This returns a formatter with similar state to this formatter but
		/// with the resolver style set. By default, a formatter has the
		/// <seealso cref="ResolverStyle#SMART SMART"/> resolver style.
		/// </para>
		/// <para>
		/// Changing the resolver style only has an effect during parsing.
		/// Parsing a text string occurs in two phases.
		/// Phase 1 is a basic text parse according to the fields added to the builder.
		/// Phase 2 resolves the parsed field-value pairs into date and/or time objects.
		/// The resolver style is used to control how phase 2, resolving, happens.
		/// See {@code ResolverStyle} for more information on the options available.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="resolverStyle">  the new resolver style, not null </param>
		/// <returns> a formatter based on this formatter with the requested resolver style, not null </returns>
		public DateTimeFormatter WithResolverStyle(ResolverStyle resolverStyle)
		{
			Objects.RequireNonNull(resolverStyle, "resolverStyle");
			if (Objects.Equals(this.ResolverStyle_Renamed, resolverStyle))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, DecimalStyle_Renamed, resolverStyle, ResolverFields_Renamed, Chrono, Zone_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the resolver fields to use during parsing.
		/// <para>
		/// This returns the resolver fields, used during the second phase of parsing
		/// when fields are resolved into dates and times.
		/// By default, a formatter has no resolver fields, and thus returns null.
		/// See <seealso cref="#withResolverFields(Set)"/> for more details.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the immutable set of resolver fields of this formatter, null if no fields </returns>
		public Set<TemporalField> ResolverFields
		{
			get
			{
				return ResolverFields_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of this formatter with a new set of resolver fields.
		/// <para>
		/// This returns a formatter with similar state to this formatter but with
		/// the resolver fields set. By default, a formatter has no resolver fields.
		/// </para>
		/// <para>
		/// Changing the resolver fields only has an effect during parsing.
		/// Parsing a text string occurs in two phases.
		/// Phase 1 is a basic text parse according to the fields added to the builder.
		/// Phase 2 resolves the parsed field-value pairs into date and/or time objects.
		/// The resolver fields are used to filter the field-value pairs between phase 1 and 2.
		/// </para>
		/// <para>
		/// This can be used to select between two or more ways that a date or time might
		/// be resolved. For example, if the formatter consists of year, month, day-of-month
		/// and day-of-year, then there are two ways to resolve a date.
		/// Calling this method with the arguments <seealso cref="ChronoField#YEAR YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_YEAR DAY_OF_YEAR"/> will ensure that the date is
		/// resolved using the year and day-of-year, effectively meaning that the month
		/// and day-of-month are ignored during the resolving phase.
		/// </para>
		/// <para>
		/// In a similar manner, this method can be used to ignore secondary fields that
		/// would otherwise be cross-checked. For example, if the formatter consists of year,
		/// month, day-of-month and day-of-week, then there is only one way to resolve a
		/// date, but the parsed value for day-of-week will be cross-checked against the
		/// resolved date. Calling this method with the arguments <seealso cref="ChronoField#YEAR YEAR"/>,
		/// <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_MONTH DAY_OF_MONTH"/> will ensure that the date is
		/// resolved correctly, but without any cross-check for the day-of-week.
		/// </para>
		/// <para>
		/// In implementation terms, this method behaves as follows. The result of the
		/// parsing phase can be considered to be a map of field to value. The behavior
		/// of this method is to cause that map to be filtered between phase 1 and 2,
		/// removing all fields other than those specified as arguments to this method.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="resolverFields">  the new set of resolver fields, null if no fields </param>
		/// <returns> a formatter based on this formatter with the requested resolver style, not null </returns>
		public DateTimeFormatter WithResolverFields(params TemporalField[] resolverFields)
		{
			Set<TemporalField> fields = null;
			if (resolverFields != null)
			{
				fields = Collections.UnmodifiableSet(new HashSet<>(Arrays.AsList(resolverFields)));
			}
			if (Objects.Equals(this.ResolverFields_Renamed, fields))
			{
				return this;
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, DecimalStyle_Renamed, ResolverStyle_Renamed, fields, Chrono, Zone_Renamed);
		}

		/// <summary>
		/// Returns a copy of this formatter with a new set of resolver fields.
		/// <para>
		/// This returns a formatter with similar state to this formatter but with
		/// the resolver fields set. By default, a formatter has no resolver fields.
		/// </para>
		/// <para>
		/// Changing the resolver fields only has an effect during parsing.
		/// Parsing a text string occurs in two phases.
		/// Phase 1 is a basic text parse according to the fields added to the builder.
		/// Phase 2 resolves the parsed field-value pairs into date and/or time objects.
		/// The resolver fields are used to filter the field-value pairs between phase 1 and 2.
		/// </para>
		/// <para>
		/// This can be used to select between two or more ways that a date or time might
		/// be resolved. For example, if the formatter consists of year, month, day-of-month
		/// and day-of-year, then there are two ways to resolve a date.
		/// Calling this method with the arguments <seealso cref="ChronoField#YEAR YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_YEAR DAY_OF_YEAR"/> will ensure that the date is
		/// resolved using the year and day-of-year, effectively meaning that the month
		/// and day-of-month are ignored during the resolving phase.
		/// </para>
		/// <para>
		/// In a similar manner, this method can be used to ignore secondary fields that
		/// would otherwise be cross-checked. For example, if the formatter consists of year,
		/// month, day-of-month and day-of-week, then there is only one way to resolve a
		/// date, but the parsed value for day-of-week will be cross-checked against the
		/// resolved date. Calling this method with the arguments <seealso cref="ChronoField#YEAR YEAR"/>,
		/// <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> and
		/// <seealso cref="ChronoField#DAY_OF_MONTH DAY_OF_MONTH"/> will ensure that the date is
		/// resolved correctly, but without any cross-check for the day-of-week.
		/// </para>
		/// <para>
		/// In implementation terms, this method behaves as follows. The result of the
		/// parsing phase can be considered to be a map of field to value. The behavior
		/// of this method is to cause that map to be filtered between phase 1 and 2,
		/// removing all fields other than those specified as arguments to this method.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="resolverFields">  the new set of resolver fields, null if no fields </param>
		/// <returns> a formatter based on this formatter with the requested resolver style, not null </returns>
		public DateTimeFormatter WithResolverFields(Set<TemporalField> resolverFields)
		{
			if (Objects.Equals(this.ResolverFields_Renamed, resolverFields))
			{
				return this;
			}
			if (resolverFields != null)
			{
				resolverFields = Collections.UnmodifiableSet(new HashSet<>(resolverFields));
			}
			return new DateTimeFormatter(PrinterParser, Locale_Renamed, DecimalStyle_Renamed, ResolverStyle_Renamed, resolverFields, Chrono, Zone_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Formats a date-time object using this formatter.
		/// <para>
		/// This formats the date-time to a String using the rules of the formatter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to format, not null </param>
		/// <returns> the formatted string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during formatting </exception>
		public String Format(TemporalAccessor temporal)
		{
			StringBuilder buf = new StringBuilder(32);
			FormatTo(temporal, buf);
			return buf.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Formats a date-time object to an {@code Appendable} using this formatter.
		/// <para>
		/// This outputs the formatted date-time to the specified destination.
		/// <seealso cref="Appendable"/> is a general purpose interface that is implemented by all
		/// key character output classes including {@code StringBuffer}, {@code StringBuilder},
		/// {@code PrintStream} and {@code Writer}.
		/// </para>
		/// <para>
		/// Although {@code Appendable} methods throw an {@code IOException}, this method does not.
		/// Instead, any {@code IOException} is wrapped in a runtime exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to format, not null </param>
		/// <param name="appendable">  the appendable to format to, not null </param>
		/// <exception cref="DateTimeException"> if an error occurs during formatting </exception>
		public void FormatTo(TemporalAccessor temporal, Appendable appendable)
		{
			Objects.RequireNonNull(temporal, "temporal");
			Objects.RequireNonNull(appendable, "appendable");
			try
			{
				DateTimePrintContext context = new DateTimePrintContext(temporal, this);
				if (appendable is StringBuilder)
				{
					PrinterParser.Format(context, (StringBuilder) appendable);
				}
				else
				{
					// buffer output to avoid writing to appendable in case of error
					StringBuilder buf = new StringBuilder(32);
					PrinterParser.Format(context, buf);
					appendable.Append(buf);
				}
			}
			catch (IOException ex)
			{
				throw new DateTimeException(ex.Message, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Fully parses the text producing a temporal object.
		/// <para>
		/// This parses the entire text producing a temporal object.
		/// It is typically more useful to use <seealso cref="#parse(CharSequence, TemporalQuery)"/>.
		/// The result of this method is {@code TemporalAccessor} which has been resolved,
		/// applying basic validation checks to help ensure a valid date-time.
		/// </para>
		/// <para>
		/// If the parse completes without reading the entire length of the text,
		/// or a problem occurs during parsing or merging, then an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <returns> the parsed temporal object, not null </returns>
		/// <exception cref="DateTimeParseException"> if unable to parse the requested result </exception>
		public TemporalAccessor Parse(CharSequence text)
		{
			Objects.RequireNonNull(text, "text");
			try
			{
				return ParseResolved0(text, null);
			}
			catch (DateTimeParseException ex)
			{
				throw ex;
			}
			catch (RuntimeException ex)
			{
				throw CreateError(text, ex);
			}
		}

		/// <summary>
		/// Parses the text using this formatter, providing control over the text position.
		/// <para>
		/// This parses the text without requiring the parse to start from the beginning
		/// of the string or finish at the end.
		/// The result of this method is {@code TemporalAccessor} which has been resolved,
		/// applying basic validation checks to help ensure a valid date-time.
		/// </para>
		/// <para>
		/// The text will be parsed from the specified start {@code ParsePosition}.
		/// The entire length of the text does not have to be parsed, the {@code ParsePosition}
		/// will be updated with the index at the end of parsing.
		/// </para>
		/// <para>
		/// The operation of this method is slightly different to similar methods using
		/// {@code ParsePosition} on {@code java.text.Format}. That class will return
		/// errors using the error index on the {@code ParsePosition}. By contrast, this
		/// method will throw a <seealso cref="DateTimeParseException"/> if an error occurs, with
		/// the exception containing the error index.
		/// This change in behavior is necessary due to the increased complexity of
		/// parsing and resolving dates/times in this API.
		/// </para>
		/// <para>
		/// If the formatter parses the same field more than once with different values,
		/// the result will be an error.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="position">  the position to parse from, updated with length parsed
		///  and the index of any error, not null </param>
		/// <returns> the parsed temporal object, not null </returns>
		/// <exception cref="DateTimeParseException"> if unable to parse the requested result </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the position is invalid </exception>
		public TemporalAccessor Parse(CharSequence text, ParsePosition position)
		{
			Objects.RequireNonNull(text, "text");
			Objects.RequireNonNull(position, "position");
			try
			{
				return ParseResolved0(text, position);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (DateTimeParseException | IndexOutOfBoundsException ex)
			{
				throw ex;
			}
			catch (RuntimeException ex)
			{
				throw CreateError(text, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Fully parses the text producing an object of the specified type.
		/// <para>
		/// Most applications should use this method for parsing.
		/// It parses the entire text to produce the required date-time.
		/// The query is typically a method reference to a {@code from(TemporalAccessor)} method.
		/// For example:
		/// <pre>
		///  LocalDateTime dt = parser.parse(str, LocalDateTime::from);
		/// </pre>
		/// If the parse completes without reading the entire length of the text,
		/// or a problem occurs during parsing or merging, then an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the parsed date-time </param>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="query">  the query defining the type to parse to, not null </param>
		/// <returns> the parsed date-time, not null </returns>
		/// <exception cref="DateTimeParseException"> if unable to parse the requested result </exception>
		public T parse<T>(CharSequence text, TemporalQuery<T> query)
		{
			Objects.RequireNonNull(text, "text");
			Objects.RequireNonNull(query, "query");
			try
			{
				return ParseResolved0(text, null).query(query);
			}
			catch (DateTimeParseException ex)
			{
				throw ex;
			}
			catch (RuntimeException ex)
			{
				throw CreateError(text, ex);
			}
		}

		/// <summary>
		/// Fully parses the text producing an object of one of the specified types.
		/// <para>
		/// This parse method is convenient for use when the parser can handle optional elements.
		/// For example, a pattern of 'uuuu-MM-dd HH.mm[ VV]' can be fully parsed to a {@code ZonedDateTime},
		/// or partially parsed to a {@code LocalDateTime}.
		/// The queries must be specified in order, starting from the best matching full-parse option
		/// and ending with the worst matching minimal parse option.
		/// The query is typically a method reference to a {@code from(TemporalAccessor)} method.
		/// </para>
		/// <para>
		/// The result is associated with the first type that successfully parses.
		/// Normally, applications will use {@code instanceof} to check the result.
		/// For example:
		/// <pre>
		///  TemporalAccessor dt = parser.parseBest(str, ZonedDateTime::from, LocalDateTime::from);
		///  if (dt instanceof ZonedDateTime) {
		///   ...
		///  } else {
		///   ...
		///  }
		/// </pre>
		/// If the parse completes without reading the entire length of the text,
		/// or a problem occurs during parsing or merging, then an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="queries">  the queries defining the types to attempt to parse to,
		///  must implement {@code TemporalAccessor}, not null </param>
		/// <returns> the parsed date-time, not null </returns>
		/// <exception cref="IllegalArgumentException"> if less than 2 types are specified </exception>
		/// <exception cref="DateTimeParseException"> if unable to parse the requested result </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.time.temporal.TemporalAccessor parseBest(CharSequence text, java.time.temporal.TemporalQuery<?>... queries)
		public TemporalAccessor ParseBest(CharSequence text, params TemporalQuery<?>[] queries)
		{
			Objects.RequireNonNull(text, "text");
			Objects.RequireNonNull(queries, "queries");
			if (queries.Length < 2)
			{
				throw new IllegalArgumentException("At least two queries must be specified");
			}
			try
			{
				TemporalAccessor resolved = ParseResolved0(text, null);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.time.temporal.TemporalQuery<?> query : queries)
				foreach (TemporalQuery<?> query in queries)
				{
					try
					{
						return (TemporalAccessor) resolved.query(query);
					}
					catch (RuntimeException)
					{
						// continue
					}
				}
				throw new DateTimeException("Unable to convert parsed text using any of the specified queries");
			}
			catch (DateTimeParseException ex)
			{
				throw ex;
			}
			catch (RuntimeException ex)
			{
				throw CreateError(text, ex);
			}
		}

		private DateTimeParseException CreateError(CharSequence text, RuntimeException ex)
		{
			String abbr;
			if (text.Length() > 64)
			{
				abbr = text.SubSequence(0, 64).ToString() + "...";
			}
			else
			{
				abbr = text.ToString();
			}
			return new DateTimeParseException("Text '" + abbr + "' could not be parsed: " + ex.Message, text, 0, ex);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Parses and resolves the specified text.
		/// <para>
		/// This parses to a {@code TemporalAccessor} ensuring that the text is fully parsed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="position">  the position to parse from, updated with length parsed
		///  and the index of any error, null if parsing whole string </param>
		/// <returns> the resolved result of the parse, not null </returns>
		/// <exception cref="DateTimeParseException"> if the parse fails </exception>
		/// <exception cref="DateTimeException"> if an error occurs while resolving the date or time </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the position is invalid </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.time.temporal.TemporalAccessor parseResolved0(final CharSequence text, final java.text.ParsePosition position)
		private TemporalAccessor ParseResolved0(CharSequence text, ParsePosition position)
		{
			ParsePosition pos = (position != null ? position : new ParsePosition(0));
			DateTimeParseContext context = ParseUnresolved0(text, pos);
			if (context == null || pos.ErrorIndex >= 0 || (position == null && pos.Index < text.Length()))
			{
				String abbr;
				if (text.Length() > 64)
				{
					abbr = text.SubSequence(0, 64).ToString() + "...";
				}
				else
				{
					abbr = text.ToString();
				}
				if (pos.ErrorIndex >= 0)
				{
					throw new DateTimeParseException("Text '" + abbr + "' could not be parsed at index " + pos.ErrorIndex, text, pos.ErrorIndex);
				}
				else
				{
					throw new DateTimeParseException("Text '" + abbr + "' could not be parsed, unparsed text found at index " + pos.Index, text, pos.Index);
				}
			}
			return context.ToResolved(ResolverStyle_Renamed, ResolverFields_Renamed);
		}

		/// <summary>
		/// Parses the text using this formatter, without resolving the result, intended
		/// for advanced use cases.
		/// <para>
		/// Parsing is implemented as a two-phase operation.
		/// First, the text is parsed using the layout defined by the formatter, producing
		/// a {@code Map} of field to value, a {@code ZoneId} and a {@code Chronology}.
		/// Second, the parsed data is <em>resolved</em>, by validating, combining and
		/// simplifying the various fields into more useful ones.
		/// This method performs the parsing stage but not the resolving stage.
		/// </para>
		/// <para>
		/// The result of this method is {@code TemporalAccessor} which represents the
		/// data as seen in the input. Values are not validated, thus parsing a date string
		/// of '2012-00-65' would result in a temporal with three fields - year of '2012',
		/// month of '0' and day-of-month of '65'.
		/// </para>
		/// <para>
		/// The text will be parsed from the specified start {@code ParsePosition}.
		/// The entire length of the text does not have to be parsed, the {@code ParsePosition}
		/// will be updated with the index at the end of parsing.
		/// </para>
		/// <para>
		/// Errors are returned using the error index field of the {@code ParsePosition}
		/// instead of {@code DateTimeParseException}.
		/// The returned error index will be set to an index indicative of the error.
		/// Callers must check for errors before using the result.
		/// </para>
		/// <para>
		/// If the formatter parses the same field more than once with different values,
		/// the result will be an error.
		/// </para>
		/// <para>
		/// This method is intended for advanced use cases that need access to the
		/// internal state during parsing. Typical application code should use
		/// <seealso cref="#parse(CharSequence, TemporalQuery)"/> or the parse method on the target type.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text">  the text to parse, not null </param>
		/// <param name="position">  the position to parse from, updated with length parsed
		///  and the index of any error, not null </param>
		/// <returns> the parsed text, null if the parse results in an error </returns>
		/// <exception cref="DateTimeException"> if some problem occurs during parsing </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the position is invalid </exception>
		public TemporalAccessor ParseUnresolved(CharSequence text, ParsePosition position)
		{
			DateTimeParseContext context = ParseUnresolved0(text, position);
			if (context == null)
			{
				return null;
			}
			return context.ToUnresolved();
		}

		private DateTimeParseContext ParseUnresolved0(CharSequence text, ParsePosition position)
		{
			Objects.RequireNonNull(text, "text");
			Objects.RequireNonNull(position, "position");
			DateTimeParseContext context = new DateTimeParseContext(this);
			int pos = position.Index;
			pos = PrinterParser.Parse(context, text, pos);
			if (pos < 0)
			{
				position.ErrorIndex = ~pos; // index not updated from input
				return null;
			}
			position.Index = pos; // errorIndex not updated from input
			return context;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the formatter as a composite printer parser.
		/// </summary>
		/// <param name="optional">  whether the printer/parser should be optional </param>
		/// <returns> the printer/parser, not null </returns>
		internal CompositePrinterParser ToPrinterParser(bool optional)
		{
			return PrinterParser.WithOptional(optional);
		}

		/// <summary>
		/// Returns this formatter as a {@code java.text.Format} instance.
		/// <para>
		/// The returned <seealso cref="Format"/> instance will format any <seealso cref="TemporalAccessor"/>
		/// and parses to a resolved <seealso cref="TemporalAccessor"/>.
		/// </para>
		/// <para>
		/// Exceptions will follow the definitions of {@code Format}, see those methods
		/// for details about {@code IllegalArgumentException} during formatting and
		/// {@code ParseException} or null during parsing.
		/// The format does not support attributing of the returned format string.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this formatter as a classic format instance, not null </returns>
		public Format ToFormat()
		{
			return new ClassicFormat(this, null);
		}

		/// <summary>
		/// Returns this formatter as a {@code java.text.Format} instance that will
		/// parse using the specified query.
		/// <para>
		/// The returned <seealso cref="Format"/> instance will format any <seealso cref="TemporalAccessor"/>
		/// and parses to the type specified.
		/// The type must be one that is supported by <seealso cref="#parse"/>.
		/// </para>
		/// <para>
		/// Exceptions will follow the definitions of {@code Format}, see those methods
		/// for details about {@code IllegalArgumentException} during formatting and
		/// {@code ParseException} or null during parsing.
		/// The format does not support attributing of the returned format string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parseQuery">  the query defining the type to parse to, not null </param>
		/// <returns> this formatter as a classic format instance, not null </returns>
		public Format toFormat<T1>(TemporalQuery<T1> parseQuery)
		{
			Objects.RequireNonNull(parseQuery, "parseQuery");
			return new ClassicFormat(this, parseQuery);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a description of the underlying formatters.
		/// </summary>
		/// <returns> a description of this formatter, not null </returns>
		public override String ToString()
		{
			String pattern = PrinterParser.ToString();
			pattern = pattern.StartsWith("[") ? pattern : pattern.Substring(1, pattern.Length() - 1 - 1);
			return pattern;
			// TODO: Fix tests to not depend on toString()
	//        return "DateTimeFormatter[" + locale +
	//                (chrono != null ? "," + chrono : "") +
	//                (zone != null ? "," + zone : "") +
	//                pattern + "]";
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the classic Java Format API.
		/// @serial exclude
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static class ClassicFormat extends java.text.Format
		internal class ClassicFormat : Format // not actually serializable
		{
			/// <summary>
			/// The formatter. </summary>
			internal readonly DateTimeFormatter Formatter;
			/// <summary>
			/// The type to be parsed. </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final java.time.temporal.TemporalQuery<?> parseType;
			internal readonly TemporalQuery<?> ParseType;
			/// <summary>
			/// Constructor. </summary>
			public ClassicFormat<T1>(DateTimeFormatter formatter, TemporalQuery<T1> parseType)
			{
				this.Formatter = formatter;
				this.ParseType = parseType;
			}

			public override StringBuffer Format(Object obj, StringBuffer toAppendTo, FieldPosition pos)
			{
				Objects.RequireNonNull(obj, "obj");
				Objects.RequireNonNull(toAppendTo, "toAppendTo");
				Objects.RequireNonNull(pos, "pos");
				if (obj is TemporalAccessor == false)
				{
					throw new IllegalArgumentException("Format target must implement TemporalAccessor");
				}
				pos.BeginIndex = 0;
				pos.EndIndex = 0;
				try
				{
					Formatter.FormatTo((TemporalAccessor) obj, toAppendTo);
				}
				catch (RuntimeException ex)
				{
					throw new IllegalArgumentException(ex.Message, ex);
				}
				return toAppendTo;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object parseObject(String text) throws java.text.ParseException
			public override Object ParseObject(String text)
			{
				Objects.RequireNonNull(text, "text");
				try
				{
					if (ParseType == null)
					{
						return Formatter.ParseResolved0(text, null);
					}
					return Formatter.Parse(text, ParseType);
				}
				catch (DateTimeParseException ex)
				{
					throw new ParseException(ex.Message, ex.ErrorIndex);
				}
				catch (RuntimeException ex)
				{
					throw (ParseException) (new ParseException(ex.Message, 0)).InitCause(ex);
				}
			}
			public override Object ParseObject(String text, ParsePosition pos)
			{
				Objects.RequireNonNull(text, "text");
				DateTimeParseContext context;
				try
				{
					context = Formatter.ParseUnresolved0(text, pos);
				}
				catch (IndexOutOfBoundsException)
				{
					if (pos.ErrorIndex < 0)
					{
						pos.ErrorIndex = 0;
					}
					return null;
				}
				if (context == null)
				{
					if (pos.ErrorIndex < 0)
					{
						pos.ErrorIndex = 0;
					}
					return null;
				}
				try
				{
					TemporalAccessor resolved = context.ToResolved(Formatter.ResolverStyle_Renamed, Formatter.ResolverFields_Renamed);
					if (ParseType == null)
					{
						return resolved;
					}
					return resolved.query(ParseType);
				}
				catch (RuntimeException)
				{
					pos.ErrorIndex = 0;
					return null;
				}
			}
		}

	}

}