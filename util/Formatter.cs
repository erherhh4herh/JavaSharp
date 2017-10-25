using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{



	using DoubleConsts = sun.misc.DoubleConsts;
	using FormattedFloatingDecimal = sun.misc.FormattedFloatingDecimal;

	/// <summary>
	/// An interpreter for printf-style format strings.  This class provides support
	/// for layout justification and alignment, common formats for numeric, string,
	/// and date/time data, and locale-specific output.  Common Java types such as
	/// {@code byte}, <seealso cref="java.math.BigDecimal BigDecimal"/>, and <seealso cref="Calendar"/>
	/// are supported.  Limited formatting customization for arbitrary user types is
	/// provided through the <seealso cref="Formattable"/> interface.
	/// 
	/// <para> Formatters are not necessarily safe for multithreaded access.  Thread
	/// safety is optional and is the responsibility of users of methods in this
	/// class.
	/// 
	/// </para>
	/// <para> Formatted printing for the Java language is heavily inspired by C's
	/// {@code printf}.  Although the format strings are similar to C, some
	/// customizations have been made to accommodate the Java language and exploit
	/// some of its features.  Also, Java formatting is more strict than C's; for
	/// example, if a conversion is incompatible with a flag, an exception will be
	/// thrown.  In C inapplicable flags are silently ignored.  The format strings
	/// are thus intended to be recognizable to C programmers but not necessarily
	/// completely compatible with those in C.
	/// 
	/// </para>
	/// <para> Examples of expected usage:
	/// 
	/// <blockquote><pre>
	///   StringBuilder sb = new StringBuilder();
	///   // Send all output to the Appendable object sb
	///   Formatter formatter = new Formatter(sb, Locale.US);
	/// 
	///   // Explicit argument indices may be used to re-order output.
	///   formatter.format("%4$2s %3$2s %2$2s %1$2s", "a", "b", "c", "d")
	///   // -&gt; " d  c  b  a"
	/// 
	///   // Optional locale as the first argument can be used to get
	///   // locale-specific formatting of numbers.  The precision and width can be
	///   // given to round and align the value.
	///   formatter.format(Locale.FRANCE, "e = %+10.4f", Math.E);
	///   // -&gt; "e =    +2,7183"
	/// 
	///   // The '(' numeric flag may be used to format negative numbers with
	///   // parentheses rather than a minus sign.  Group separators are
	///   // automatically inserted.
	///   formatter.format("Amount gained or lost since last statement: $ %(,.2f",
	///                    balanceDelta);
	///   // -&gt; "Amount gained or lost since last statement: $ (6,217.58)"
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> Convenience methods for common formatting requests exist as illustrated
	/// by the following invocations:
	/// 
	/// <blockquote><pre>
	///   // Writes a formatted string to System.out.
	///   System.out.format("Local time: %tT", Calendar.getInstance());
	///   // -&gt; "Local time: 13:34:18"
	/// 
	///   // Writes formatted output to System.err.
	///   System.err.printf("Unable to open file '%1$s': %2$s",
	///                     fileName, exception.getMessage());
	///   // -&gt; "Unable to open file 'food': No such file or directory"
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> Like C's {@code sprintf(3)}, Strings may be formatted using the static
	/// method <seealso cref="String#format(String,Object...) String.format"/>:
	/// 
	/// <blockquote><pre>
	///   // Format a string containing a date.
	///   import java.util.Calendar;
	///   import java.util.GregorianCalendar;
	///   import static java.util.Calendar.*;
	/// 
	///   Calendar c = new GregorianCalendar(1995, MAY, 23);
	///   String s = String.format("Duke's Birthday: %1$tb %1$te, %1$tY", c);
	///   // -&gt; s == "Duke's Birthday: May 23, 1995"
	/// </pre></blockquote>
	/// 
	/// <h3><a name="org">Organization</a></h3>
	/// 
	/// </para>
	/// <para> This specification is divided into two sections.  The first section, <a
	/// href="#summary">Summary</a>, covers the basic formatting concepts.  This
	/// section is intended for users who want to get started quickly and are
	/// familiar with formatted printing in other programming languages.  The second
	/// section, <a href="#detail">Details</a>, covers the specific implementation
	/// details.  It is intended for users who want more precise specification of
	/// formatting behavior.
	/// 
	/// <h3><a name="summary">Summary</a></h3>
	/// 
	/// </para>
	/// <para> This section is intended to provide a brief overview of formatting
	/// concepts.  For precise behavioral details, refer to the <a
	/// href="#detail">Details</a> section.
	/// 
	/// <h4><a name="syntax">Format String Syntax</a></h4>
	/// 
	/// </para>
	/// <para> Every method which produces formatted output requires a <i>format
	/// string</i> and an <i>argument list</i>.  The format string is a {@link
	/// String} which may contain fixed text and one or more embedded <i>format
	/// specifiers</i>.  Consider the following example:
	/// 
	/// <blockquote><pre>
	///   Calendar c = ...;
	///   String s = String.format("Duke's Birthday: %1$tm %1$te,%1$tY", c);
	/// </pre></blockquote>
	/// 
	/// This format string is the first argument to the {@code format} method.  It
	/// contains three format specifiers "{@code %1$tm}", "{@code %1$te}", and
	/// "{@code %1$tY}" which indicate how the arguments should be processed and
	/// where they should be inserted in the text.  The remaining portions of the
	/// format string are fixed text including {@code "Dukes Birthday: "} and any
	/// other spaces or punctuation.
	/// 
	/// The argument list consists of all arguments passed to the method after the
	/// format string.  In the above example, the argument list is of size one and
	/// consists of the <seealso cref="java.util.Calendar Calendar"/> object {@code c}.
	/// 
	/// <ul>
	/// 
	/// <li> The format specifiers for general, character, and numeric types have
	/// the following syntax:
	/// 
	/// <blockquote><pre>
	///   %[argument_index$][flags][width][.precision]conversion
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> The optional <i>argument_index</i> is a decimal integer indicating the
	/// position of the argument in the argument list.  The first argument is
	/// referenced by "{@code 1$}", the second by "{@code 2$}", etc.
	/// 
	/// </para>
	/// <para> The optional <i>flags</i> is a set of characters that modify the output
	/// format.  The set of valid flags depends on the conversion.
	/// 
	/// </para>
	/// <para> The optional <i>width</i> is a positive decimal integer indicating
	/// the minimum number of characters to be written to the output.
	/// 
	/// </para>
	/// <para> The optional <i>precision</i> is a non-negative decimal integer usually
	/// used to restrict the number of characters.  The specific behavior depends on
	/// the conversion.
	/// 
	/// </para>
	/// <para> The required <i>conversion</i> is a character indicating how the
	/// argument should be formatted.  The set of valid conversions for a given
	/// argument depends on the argument's data type.
	/// 
	/// <li> The format specifiers for types which are used to represents dates and
	/// times have the following syntax:
	/// 
	/// <blockquote><pre>
	///   %[argument_index$][flags][width]conversion
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> The optional <i>argument_index</i>, <i>flags</i> and <i>width</i> are
	/// defined as above.
	/// 
	/// </para>
	/// <para> The required <i>conversion</i> is a two character sequence.  The first
	/// character is {@code 't'} or {@code 'T'}.  The second character indicates
	/// the format to be used.  These characters are similar to but not completely
	/// identical to those defined by GNU {@code date} and POSIX
	/// {@code strftime(3c)}.
	/// 
	/// <li> The format specifiers which do not correspond to arguments have the
	/// following syntax:
	/// 
	/// <blockquote><pre>
	///   %[flags][width]conversion
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> The optional <i>flags</i> and <i>width</i> is defined as above.
	/// 
	/// </para>
	/// <para> The required <i>conversion</i> is a character indicating content to be
	/// inserted in the output.
	/// 
	/// </ul>
	/// 
	/// <h4> Conversions </h4>
	/// 
	/// </para>
	/// <para> Conversions are divided into the following categories:
	/// 
	/// <ol>
	/// 
	/// <li> <b>General</b> - may be applied to any argument
	/// type
	/// 
	/// <li> <b>Character</b> - may be applied to basic types which represent
	/// Unicode characters: {@code char}, <seealso cref="Character"/>, {@code byte}, {@link
	/// Byte}, {@code short}, and <seealso cref="Short"/>. This conversion may also be
	/// applied to the types {@code int} and <seealso cref="Integer"/> when {@link
	/// Character#isValidCodePoint} returns {@code true}
	/// 
	/// <li> <b>Numeric</b>
	/// 
	/// <ol>
	/// 
	/// <li> <b>Integral</b> - may be applied to Java integral types: {@code byte},
	/// <seealso cref="Byte"/>, {@code short}, <seealso cref="Short"/>, {@code int} and {@link
	/// Integer}, {@code long}, <seealso cref="Long"/>, and {@link java.math.BigInteger
	/// BigInteger} (but not {@code char} or <seealso cref="Character"/>)
	/// 
	/// <li><b>Floating Point</b> - may be applied to Java floating-point types:
	/// {@code float}, <seealso cref="Float"/>, {@code double}, <seealso cref="Double"/>, and {@link
	/// java.math.BigDecimal BigDecimal}
	/// 
	/// </ol>
	/// 
	/// <li> <b>Date/Time</b> - may be applied to Java types which are capable of
	/// encoding a date or time: {@code long}, <seealso cref="Long"/>, <seealso cref="Calendar"/>,
	/// <seealso cref="Date"/> and <seealso cref="TemporalAccessor TemporalAccessor"/>
	/// 
	/// <li> <b>Percent</b> - produces a literal {@code '%'}
	/// (<tt>'&#92;u0025'</tt>)
	/// 
	/// <li> <b>Line Separator</b> - produces the platform-specific line separator
	/// 
	/// </ol>
	/// 
	/// </para>
	/// <para> The following table summarizes the supported conversions.  Conversions
	/// denoted by an upper-case character (i.e. {@code 'B'}, {@code 'H'},
	/// {@code 'S'}, {@code 'C'}, {@code 'X'}, {@code 'E'}, {@code 'G'},
	/// {@code 'A'}, and {@code 'T'}) are the same as those for the corresponding
	/// lower-case conversion characters except that the result is converted to
	/// upper case according to the rules of the prevailing {@link java.util.Locale
	/// Locale}.  The result is equivalent to the following invocation of {@link
	/// String#toUpperCase()}
	/// 
	/// <pre>
	///    out.toUpperCase() </pre>
	/// 
	/// <table cellpadding=5 summary="genConv">
	/// 
	/// <tr><th valign="bottom"> Conversion
	///     <th valign="bottom"> Argument Category
	///     <th valign="bottom"> Description
	/// 
	/// <tr><td valign="top"> {@code 'b'}, {@code 'B'}
	///     <td valign="top"> general
	///     <td> If the argument <i>arg</i> is {@code null}, then the result is
	///     "{@code false}".  If <i>arg</i> is a {@code boolean} or {@link
	///     Boolean}, then the result is the string returned by {@link
	///     String#valueOf(boolean) String.valueOf(arg)}.  Otherwise, the result is
	///     "true".
	/// 
	/// <tr><td valign="top"> {@code 'h'}, {@code 'H'}
	///     <td valign="top"> general
	///     <td> If the argument <i>arg</i> is {@code null}, then the result is
	///     "{@code null}".  Otherwise, the result is obtained by invoking
	///     {@code Integer.toHexString(arg.hashCode())}.
	/// 
	/// <tr><td valign="top"> {@code 's'}, {@code 'S'}
	///     <td valign="top"> general
	///     <td> If the argument <i>arg</i> is {@code null}, then the result is
	///     "{@code null}".  If <i>arg</i> implements <seealso cref="Formattable"/>, then
	///     <seealso cref="Formattable#formatTo arg.formatTo"/> is invoked. Otherwise, the
	///     result is obtained by invoking {@code arg.toString()}.
	/// 
	/// <tr><td valign="top">{@code 'c'}, {@code 'C'}
	///     <td valign="top"> character
	///     <td> The result is a Unicode character
	/// 
	/// <tr><td valign="top">{@code 'd'}
	///     <td valign="top"> integral
	///     <td> The result is formatted as a decimal integer
	/// 
	/// <tr><td valign="top">{@code 'o'}
	///     <td valign="top"> integral
	///     <td> The result is formatted as an octal integer
	/// 
	/// <tr><td valign="top">{@code 'x'}, {@code 'X'}
	///     <td valign="top"> integral
	///     <td> The result is formatted as a hexadecimal integer
	/// 
	/// <tr><td valign="top">{@code 'e'}, {@code 'E'}
	///     <td valign="top"> floating point
	///     <td> The result is formatted as a decimal number in computerized
	///     scientific notation
	/// 
	/// <tr><td valign="top">{@code 'f'}
	///     <td valign="top"> floating point
	///     <td> The result is formatted as a decimal number
	/// 
	/// <tr><td valign="top">{@code 'g'}, {@code 'G'}
	///     <td valign="top"> floating point
	///     <td> The result is formatted using computerized scientific notation or
	///     decimal format, depending on the precision and the value after rounding.
	/// 
	/// <tr><td valign="top">{@code 'a'}, {@code 'A'}
	///     <td valign="top"> floating point
	///     <td> The result is formatted as a hexadecimal floating-point number with
	///     a significand and an exponent. This conversion is <b>not</b> supported
	///     for the {@code BigDecimal} type despite the latter's being in the
	///     <i>floating point</i> argument category.
	/// 
	/// <tr><td valign="top">{@code 't'}, {@code 'T'}
	///     <td valign="top"> date/time
	///     <td> Prefix for date and time conversion characters.  See <a
	///     href="#dt">Date/Time Conversions</a>.
	/// 
	/// <tr><td valign="top">{@code '%'}
	///     <td valign="top"> percent
	///     <td> The result is a literal {@code '%'} (<tt>'&#92;u0025'</tt>)
	/// 
	/// <tr><td valign="top">{@code 'n'}
	///     <td valign="top"> line separator
	///     <td> The result is the platform-specific line separator
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> Any characters not explicitly defined as conversions are illegal and are
	/// reserved for future extensions.
	/// 
	/// <h4><a name="dt">Date/Time Conversions</a></h4>
	/// 
	/// </para>
	/// <para> The following date and time conversion suffix characters are defined for
	/// the {@code 't'} and {@code 'T'} conversions.  The types are similar to but
	/// not completely identical to those defined by GNU {@code date} and POSIX
	/// {@code strftime(3c)}.  Additional conversion types are provided to access
	/// Java-specific functionality (e.g. {@code 'L'} for milliseconds within the
	/// second).
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting times:
	/// 
	/// <table cellpadding=5 summary="time">
	/// 
	/// <tr><td valign="top"> {@code 'H'}
	///     <td> Hour of the day for the 24-hour clock, formatted as two digits with
	///     a leading zero as necessary i.e. {@code 00 - 23}.
	/// 
	/// <tr><td valign="top">{@code 'I'}
	///     <td> Hour for the 12-hour clock, formatted as two digits with a leading
	///     zero as necessary, i.e.  {@code 01 - 12}.
	/// 
	/// <tr><td valign="top">{@code 'k'}
	///     <td> Hour of the day for the 24-hour clock, i.e. {@code 0 - 23}.
	/// 
	/// <tr><td valign="top">{@code 'l'}
	///     <td> Hour for the 12-hour clock, i.e. {@code 1 - 12}.
	/// 
	/// <tr><td valign="top">{@code 'M'}
	///     <td> Minute within the hour formatted as two digits with a leading zero
	///     as necessary, i.e.  {@code 00 - 59}.
	/// 
	/// <tr><td valign="top">{@code 'S'}
	///     <td> Seconds within the minute, formatted as two digits with a leading
	///     zero as necessary, i.e. {@code 00 - 60} ("{@code 60}" is a special
	///     value required to support leap seconds).
	/// 
	/// <tr><td valign="top">{@code 'L'}
	///     <td> Millisecond within the second formatted as three digits with
	///     leading zeros as necessary, i.e. {@code 000 - 999}.
	/// 
	/// <tr><td valign="top">{@code 'N'}
	///     <td> Nanosecond within the second, formatted as nine digits with leading
	///     zeros as necessary, i.e. {@code 000000000 - 999999999}.
	/// 
	/// <tr><td valign="top">{@code 'p'}
	///     <td> Locale-specific {@linkplain
	///     java.text.DateFormatSymbols#getAmPmStrings morning or afternoon} marker
	///     in lower case, e.g."{@code am}" or "{@code pm}". Use of the conversion
	///     prefix {@code 'T'} forces this output to upper case.
	/// 
	/// <tr><td valign="top">{@code 'z'}
	///     <td> <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC&nbsp;822</a>
	///     style numeric time zone offset from GMT, e.g. {@code -0800}.  This
	///     value will be adjusted as necessary for Daylight Saving Time.  For
	///     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	///     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	///     instance of the Java virtual machine.
	/// 
	/// <tr><td valign="top">{@code 'Z'}
	///     <td> A string representing the abbreviation for the time zone.  This
	///     value will be adjusted as necessary for Daylight Saving Time.  For
	///     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the  time zone used is
	///     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	///     instance of the Java virtual machine.  The Formatter's locale will
	///     supersede the locale of the argument (if any).
	/// 
	/// <tr><td valign="top">{@code 's'}
	///     <td> Seconds since the beginning of the epoch starting at 1 January 1970
	///     {@code 00:00:00} UTC, i.e. {@code Long.MIN_VALUE/1000} to
	///     {@code Long.MAX_VALUE/1000}.
	/// 
	/// <tr><td valign="top">{@code 'Q'}
	///     <td> Milliseconds since the beginning of the epoch starting at 1 January
	///     1970 {@code 00:00:00} UTC, i.e. {@code Long.MIN_VALUE} to
	///     {@code Long.MAX_VALUE}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting dates:
	/// 
	/// <table cellpadding=5 summary="date">
	/// 
	/// <tr><td valign="top">{@code 'B'}
	///     <td> Locale-specific {@link java.text.DateFormatSymbols#getMonths
	///     full month name}, e.g. {@code "January"}, {@code "February"}.
	/// 
	/// <tr><td valign="top">{@code 'b'}
	///     <td> Locale-specific {@linkplain
	///     java.text.DateFormatSymbols#getShortMonths abbreviated month name},
	///     e.g. {@code "Jan"}, {@code "Feb"}.
	/// 
	/// <tr><td valign="top">{@code 'h'}
	///     <td> Same as {@code 'b'}.
	/// 
	/// <tr><td valign="top">{@code 'A'}
	///     <td> Locale-specific full name of the {@linkplain
	///     java.text.DateFormatSymbols#getWeekdays day of the week},
	///     e.g. {@code "Sunday"}, {@code "Monday"}
	/// 
	/// <tr><td valign="top">{@code 'a'}
	///     <td> Locale-specific short name of the {@linkplain
	///     java.text.DateFormatSymbols#getShortWeekdays day of the week},
	///     e.g. {@code "Sun"}, {@code "Mon"}
	/// 
	/// <tr><td valign="top">{@code 'C'}
	///     <td> Four-digit year divided by {@code 100}, formatted as two digits
	///     with leading zero as necessary, i.e. {@code 00 - 99}
	/// 
	/// <tr><td valign="top">{@code 'Y'}
	///     <td> Year, formatted as at least four digits with leading zeros as
	///     necessary, e.g. {@code 0092} equals {@code 92} CE for the Gregorian
	///     calendar.
	/// 
	/// <tr><td valign="top">{@code 'y'}
	///     <td> Last two digits of the year, formatted with leading zeros as
	///     necessary, i.e. {@code 00 - 99}.
	/// 
	/// <tr><td valign="top">{@code 'j'}
	///     <td> Day of year, formatted as three digits with leading zeros as
	///     necessary, e.g. {@code 001 - 366} for the Gregorian calendar.
	/// 
	/// <tr><td valign="top">{@code 'm'}
	///     <td> Month, formatted as two digits with leading zeros as necessary,
	///     i.e. {@code 01 - 13}.
	/// 
	/// <tr><td valign="top">{@code 'd'}
	///     <td> Day of month, formatted as two digits with leading zeros as
	///     necessary, i.e. {@code 01 - 31}
	/// 
	/// <tr><td valign="top">{@code 'e'}
	///     <td> Day of month, formatted as two digits, i.e. {@code 1 - 31}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting common
	/// date/time compositions.
	/// 
	/// <table cellpadding=5 summary="composites">
	/// 
	/// <tr><td valign="top">{@code 'R'}
	///     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM"}
	/// 
	/// <tr><td valign="top">{@code 'T'}
	///     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM:%tS"}.
	/// 
	/// <tr><td valign="top">{@code 'r'}
	///     <td> Time formatted for the 12-hour clock as {@code "%tI:%tM:%tS %Tp"}.
	///     The location of the morning or afternoon marker ({@code '%Tp'}) may be
	///     locale-dependent.
	/// 
	/// <tr><td valign="top">{@code 'D'}
	///     <td> Date formatted as {@code "%tm/%td/%ty"}.
	/// 
	/// <tr><td valign="top">{@code 'F'}
	///     <td> <a href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a>
	///     complete date formatted as {@code "%tY-%tm-%td"}.
	/// 
	/// <tr><td valign="top">{@code 'c'}
	///     <td> Date and time formatted as {@code "%ta %tb %td %tT %tZ %tY"},
	///     e.g. {@code "Sun Jul 20 16:17:00 EDT 1969"}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> Any characters not explicitly defined as date/time conversion suffixes
	/// are illegal and are reserved for future extensions.
	/// 
	/// <h4> Flags </h4>
	/// 
	/// </para>
	/// <para> The following table summarizes the supported flags.  <i>y</i> means the
	/// flag is supported for the indicated argument types.
	/// 
	/// <table cellpadding=5 summary="genConv">
	/// 
	/// <tr><th valign="bottom"> Flag <th valign="bottom"> General
	///     <th valign="bottom"> Character <th valign="bottom"> Integral
	///     <th valign="bottom"> Floating Point
	///     <th valign="bottom"> Date/Time
	///     <th valign="bottom"> Description
	/// 
	/// <tr><td> '-' <td align="center" valign="top"> y
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> y
	///     <td> The result will be left-justified.
	/// 
	/// <tr><td> '#' <td align="center" valign="top"> y<sup>1</sup>
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y<sup>3</sup>
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> -
	///     <td> The result should use a conversion-dependent alternate form
	/// 
	/// <tr><td> '+' <td align="center" valign="top"> -
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y<sup>4</sup>
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> -
	///     <td> The result will always include a sign
	/// 
	/// <tr><td> '&nbsp;&nbsp;' <td align="center" valign="top"> -
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y<sup>4</sup>
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> -
	///     <td> The result will include a leading space for positive values
	/// 
	/// <tr><td> '0' <td align="center" valign="top"> -
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> y
	///     <td align="center" valign="top"> -
	///     <td> The result will be zero-padded
	/// 
	/// <tr><td> ',' <td align="center" valign="top"> -
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y<sup>2</sup>
	///     <td align="center" valign="top"> y<sup>5</sup>
	///     <td align="center" valign="top"> -
	///     <td> The result will include locale-specific {@linkplain
	///     java.text.DecimalFormatSymbols#getGroupingSeparator grouping separators}
	/// 
	/// <tr><td> '(' <td align="center" valign="top"> -
	///     <td align="center" valign="top"> -
	///     <td align="center" valign="top"> y<sup>4</sup>
	///     <td align="center" valign="top"> y<sup>5</sup>
	///     <td align="center"> -
	///     <td> The result will enclose negative numbers in parentheses
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> <sup>1</sup> Depends on the definition of <seealso cref="Formattable"/>.
	/// 
	/// </para>
	/// <para> <sup>2</sup> For {@code 'd'} conversion only.
	/// 
	/// </para>
	/// <para> <sup>3</sup> For {@code 'o'}, {@code 'x'}, and {@code 'X'}
	/// conversions only.
	/// 
	/// </para>
	/// <para> <sup>4</sup> For {@code 'd'}, {@code 'o'}, {@code 'x'}, and
	/// {@code 'X'} conversions applied to <seealso cref="java.math.BigInteger BigInteger"/>
	/// or {@code 'd'} applied to {@code byte}, <seealso cref="Byte"/>, {@code short}, {@link
	/// Short}, {@code int} and <seealso cref="Integer"/>, {@code long}, and <seealso cref="Long"/>.
	/// 
	/// </para>
	/// <para> <sup>5</sup> For {@code 'e'}, {@code 'E'}, {@code 'f'},
	/// {@code 'g'}, and {@code 'G'} conversions only.
	/// 
	/// </para>
	/// <para> Any characters not explicitly defined as flags are illegal and are
	/// reserved for future extensions.
	/// 
	/// <h4> Width </h4>
	/// 
	/// </para>
	/// <para> The width is the minimum number of characters to be written to the
	/// output.  For the line separator conversion, width is not applicable; if it
	/// is provided, an exception will be thrown.
	/// 
	/// <h4> Precision </h4>
	/// 
	/// </para>
	/// <para> For general argument types, the precision is the maximum number of
	/// characters to be written to the output.
	/// 
	/// </para>
	/// <para> For the floating-point conversions {@code 'a'}, {@code 'A'}, {@code 'e'},
	/// {@code 'E'}, and {@code 'f'} the precision is the number of digits after the
	/// radix point.  If the conversion is {@code 'g'} or {@code 'G'}, then the
	/// precision is the total number of digits in the resulting magnitude after
	/// rounding.
	/// 
	/// </para>
	/// <para> For character, integral, and date/time argument types and the percent
	/// and line separator conversions, the precision is not applicable; if a
	/// precision is provided, an exception will be thrown.
	/// 
	/// <h4> Argument Index </h4>
	/// 
	/// </para>
	/// <para> The argument index is a decimal integer indicating the position of the
	/// argument in the argument list.  The first argument is referenced by
	/// "{@code 1$}", the second by "{@code 2$}", etc.
	/// 
	/// </para>
	/// <para> Another way to reference arguments by position is to use the
	/// {@code '<'} (<tt>'&#92;u003c'</tt>) flag, which causes the argument for
	/// the previous format specifier to be re-used.  For example, the following two
	/// statements would produce identical strings:
	/// 
	/// <blockquote><pre>
	///   Calendar c = ...;
	///   String s1 = String.format("Duke's Birthday: %1$tm %1$te,%1$tY", c);
	/// 
	///   String s2 = String.format("Duke's Birthday: %1$tm %&lt;te,%&lt;tY", c);
	/// </pre></blockquote>
	/// 
	/// <hr>
	/// <h3><a name="detail">Details</a></h3>
	/// 
	/// </para>
	/// <para> This section is intended to provide behavioral details for formatting,
	/// including conditions and exceptions, supported data types, localization, and
	/// interactions between flags, conversions, and data types.  For an overview of
	/// formatting concepts, refer to the <a href="#summary">Summary</a>
	/// 
	/// </para>
	/// <para> Any characters not explicitly defined as conversions, date/time
	/// conversion suffixes, or flags are illegal and are reserved for
	/// future extensions.  Use of such a character in a format string will
	/// cause an <seealso cref="UnknownFormatConversionException"/> or {@link
	/// UnknownFormatFlagsException} to be thrown.
	/// 
	/// </para>
	/// <para> If the format specifier contains a width or precision with an invalid
	/// value or which is otherwise unsupported, then a {@link
	/// IllegalFormatWidthException} or <seealso cref="IllegalFormatPrecisionException"/>
	/// respectively will be thrown.
	/// 
	/// </para>
	/// <para> If a format specifier contains a conversion character that is not
	/// applicable to the corresponding argument, then an {@link
	/// IllegalFormatConversionException} will be thrown.
	/// 
	/// </para>
	/// <para> All specified exceptions may be thrown by any of the {@code format}
	/// methods of {@code Formatter} as well as by any {@code format} convenience
	/// methods such as <seealso cref="String#format(String,Object...) String.format"/> and
	/// <seealso cref="java.io.PrintStream#printf(String,Object...) PrintStream.printf"/>.
	/// 
	/// </para>
	/// <para> Conversions denoted by an upper-case character (i.e. {@code 'B'},
	/// {@code 'H'}, {@code 'S'}, {@code 'C'}, {@code 'X'}, {@code 'E'},
	/// {@code 'G'}, {@code 'A'}, and {@code 'T'}) are the same as those for the
	/// corresponding lower-case conversion characters except that the result is
	/// converted to upper case according to the rules of the prevailing {@link
	/// java.util.Locale Locale}.  The result is equivalent to the following
	/// invocation of <seealso cref="String#toUpperCase()"/>
	/// 
	/// <pre>
	///    out.toUpperCase() </pre>
	/// 
	/// <h4><a name="dgen">General</a></h4>
	/// 
	/// </para>
	/// <para> The following general conversions may be applied to any argument type:
	/// 
	/// <table cellpadding=5 summary="dgConv">
	/// 
	/// <tr><td valign="top"> {@code 'b'}
	///     <td valign="top"> <tt>'&#92;u0062'</tt>
	///     <td> Produces either "{@code true}" or "{@code false}" as returned by
	///     <seealso cref="Boolean#toString(boolean)"/>.
	/// 
	/// </para>
	///     <para> If the argument is {@code null}, then the result is
	///     "{@code false}".  If the argument is a {@code boolean} or {@link
	///     Boolean}, then the result is the string returned by {@link
	///     String#valueOf(boolean) String.valueOf()}.  Otherwise, the result is
	///     "{@code true}".
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given, then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'B'}
	///     <td valign="top"> <tt>'&#92;u0042'</tt>
	///     <td> The upper-case variant of {@code 'b'}.
	/// 
	/// <tr><td valign="top"> {@code 'h'}
	///     <td valign="top"> <tt>'&#92;u0068'</tt>
	///     <td> Produces a string representing the hash code value of the object.
	/// 
	/// </para>
	///     <para> If the argument, <i>arg</i> is {@code null}, then the
	///     result is "{@code null}".  Otherwise, the result is obtained
	///     by invoking {@code Integer.toHexString(arg.hashCode())}.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given, then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'H'}
	///     <td valign="top"> <tt>'&#92;u0048'</tt>
	///     <td> The upper-case variant of {@code 'h'}.
	/// 
	/// <tr><td valign="top"> {@code 's'}
	///     <td valign="top"> <tt>'&#92;u0073'</tt>
	///     <td> Produces a string.
	/// 
	/// </para>
	///     <para> If the argument is {@code null}, then the result is
	///     "{@code null}".  If the argument implements <seealso cref="Formattable"/>, then
	///     its <seealso cref="Formattable#formatTo formatTo"/> method is invoked.
	///     Otherwise, the result is obtained by invoking the argument's
	///     {@code toString()} method.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given and the argument is not a {@link
	///     Formattable} , then a <seealso cref="FormatFlagsConversionMismatchException"/>
	///     will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'S'}
	///     <td valign="top"> <tt>'&#92;u0053'</tt>
	///     <td> The upper-case variant of {@code 's'}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following <a name="dFlags">flags</a> apply to general conversions:
	/// 
	/// <table cellpadding=5 summary="dFlags">
	/// 
	/// <tr><td valign="top"> {@code '-'}
	///     <td valign="top"> <tt>'&#92;u002d'</tt>
	///     <td> Left justifies the output.  Spaces (<tt>'&#92;u0020'</tt>) will be
	///     added at the end of the converted value as required to fill the minimum
	///     width of the field.  If the width is not provided, then a {@link
	///     MissingFormatWidthException} will be thrown.  If this flag is not given
	///     then the output will be right-justified.
	/// 
	/// <tr><td valign="top"> {@code '#'}
	///     <td valign="top"> <tt>'&#92;u0023'</tt>
	///     <td> Requires the output use an alternate form.  The definition of the
	///     form is specified by the conversion.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The <a name="genWidth">width</a> is the minimum number of characters to
	/// be written to the
	/// output.  If the length of the converted value is less than the width then
	/// the output will be padded by <tt>'&nbsp;&nbsp;'</tt> (<tt>'&#92;u0020'</tt>)
	/// until the total number of characters equals the width.  The padding is on
	/// the left by default.  If the {@code '-'} flag is given, then the padding
	/// will be on the right.  If the width is not specified then there is no
	/// minimum.
	/// 
	/// </para>
	/// <para> The precision is the maximum number of characters to be written to the
	/// output.  The precision is applied before the width, thus the output will be
	/// truncated to {@code precision} characters even if the width is greater than
	/// the precision.  If the precision is not specified then there is no explicit
	/// limit on the number of characters.
	/// 
	/// <h4><a name="dchar">Character</a></h4>
	/// 
	/// This conversion may be applied to {@code char} and <seealso cref="Character"/>.  It
	/// may also be applied to the types {@code byte}, <seealso cref="Byte"/>,
	/// {@code short}, and <seealso cref="Short"/>, {@code int} and <seealso cref="Integer"/> when
	/// <seealso cref="Character#isValidCodePoint"/> returns {@code true}.  If it returns
	/// {@code false} then an <seealso cref="IllegalFormatCodePointException"/> will be
	/// thrown.
	/// 
	/// <table cellpadding=5 summary="charConv">
	/// 
	/// <tr><td valign="top"> {@code 'c'}
	///     <td valign="top"> <tt>'&#92;u0063'</tt>
	///     <td> Formats the argument as a Unicode character as described in <a
	///     href="../lang/Character.html#unicode">Unicode Character
	///     Representation</a>.  This may be more than one 16-bit {@code char} in
	///     the case where the argument represents a supplementary character.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given, then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'C'}
	///     <td valign="top"> <tt>'&#92;u0043'</tt>
	///     <td> The upper-case variant of {@code 'c'}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The {@code '-'} flag defined for <a href="#dFlags">General
	/// conversions</a> applies.  If the {@code '#'} flag is given, then a {@link
	/// FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// </para>
	/// <para> The width is defined as for <a href="#genWidth">General conversions</a>.
	/// 
	/// </para>
	/// <para> The precision is not applicable.  If the precision is specified then an
	/// <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	/// 
	/// <h4><a name="dnum">Numeric</a></h4>
	/// 
	/// </para>
	/// <para> Numeric conversions are divided into the following categories:
	/// 
	/// <ol>
	/// 
	/// <li> <a href="#dnint"><b>Byte, Short, Integer, and Long</b></a>
	/// 
	/// <li> <a href="#dnbint"><b>BigInteger</b></a>
	/// 
	/// <li> <a href="#dndec"><b>Float and Double</b></a>
	/// 
	/// <li> <a href="#dnbdec"><b>BigDecimal</b></a>
	/// 
	/// </ol>
	/// 
	/// </para>
	/// <para> Numeric types will be formatted according to the following algorithm:
	/// 
	/// </para>
	/// <para><b><a name="L10nAlgorithm"> Number Localization Algorithm</a></b>
	/// 
	/// </para>
	/// <para> After digits are obtained for the integer part, fractional part, and
	/// exponent (as appropriate for the data type), the following transformation
	/// is applied:
	/// 
	/// <ol>
	/// 
	/// <li> Each digit character <i>d</i> in the string is replaced by a
	/// locale-specific digit computed relative to the current locale's
	/// <seealso cref="java.text.DecimalFormatSymbols#getZeroDigit() zero digit"/>
	/// <i>z</i>; that is <i>d&nbsp;-&nbsp;</i> {@code '0'}
	/// <i>&nbsp;+&nbsp;z</i>.
	/// 
	/// <li> If a decimal separator is present, a locale-specific {@linkplain
	/// java.text.DecimalFormatSymbols#getDecimalSeparator decimal separator} is
	/// substituted.
	/// 
	/// <li> If the {@code ','} (<tt>'&#92;u002c'</tt>)
	/// <a name="L10nGroup">flag</a> is given, then the locale-specific {@linkplain
	/// java.text.DecimalFormatSymbols#getGroupingSeparator grouping separator} is
	/// inserted by scanning the integer part of the string from least significant
	/// to most significant digits and inserting a separator at intervals defined by
	/// the locale's {@link java.text.DecimalFormat#getGroupingSize() grouping
	/// size}.
	/// 
	/// <li> If the {@code '0'} flag is given, then the locale-specific {@linkplain
	/// java.text.DecimalFormatSymbols#getZeroDigit() zero digits} are inserted
	/// after the sign character, if any, and before the first non-zero digit, until
	/// the length of the string is equal to the requested field width.
	/// 
	/// <li> If the value is negative and the {@code '('} flag is given, then a
	/// {@code '('} (<tt>'&#92;u0028'</tt>) is prepended and a {@code ')'}
	/// (<tt>'&#92;u0029'</tt>) is appended.
	/// 
	/// <li> If the value is negative (or floating-point negative zero) and
	/// {@code '('} flag is not given, then a {@code '-'} (<tt>'&#92;u002d'</tt>)
	/// is prepended.
	/// 
	/// <li> If the {@code '+'} flag is given and the value is positive or zero (or
	/// floating-point positive zero), then a {@code '+'} (<tt>'&#92;u002b'</tt>)
	/// will be prepended.
	/// 
	/// </ol>
	/// 
	/// </para>
	/// <para> If the value is NaN or positive infinity the literal strings "NaN" or
	/// "Infinity" respectively, will be output.  If the value is negative infinity,
	/// then the output will be "(Infinity)" if the {@code '('} flag is given
	/// otherwise the output will be "-Infinity".  These values are not localized.
	/// 
	/// </para>
	/// <para><a name="dnint"><b> Byte, Short, Integer, and Long </b></a>
	/// 
	/// </para>
	/// <para> The following conversions may be applied to {@code byte}, <seealso cref="Byte"/>,
	/// {@code short}, <seealso cref="Short"/>, {@code int} and <seealso cref="Integer"/>,
	/// {@code long}, and <seealso cref="Long"/>.
	/// 
	/// <table cellpadding=5 summary="IntConv">
	/// 
	/// <tr><td valign="top"> {@code 'd'}
	///     <td valign="top"> <tt>'&#92;u0064'</tt>
	///     <td> Formats the argument as a decimal integer. The <a
	///     href="#L10nAlgorithm">localization algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> If the {@code '0'} flag is given and the value is negative, then
	///     the zero padding will occur after the sign.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'o'}
	///     <td valign="top"> <tt>'&#92;u006f'</tt>
	///     <td> Formats the argument as an integer in base eight.  No localization
	///     is applied.
	/// 
	/// </para>
	///     <para> If <i>x</i> is negative then the result will be an unsigned value
	///     generated by adding 2<sup>n</sup> to the value where {@code n} is the
	///     number of bits in the type as returned by the static {@code SIZE} field
	///     in the <seealso cref="Byte#SIZE Byte"/>, <seealso cref="Short#SIZE Short"/>,
	///     <seealso cref="Integer#SIZE Integer"/>, or <seealso cref="Long#SIZE Long"/>
	///     classes as appropriate.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then the output will always begin
	///     with the radix indicator {@code '0'}.
	/// 
	/// </para>
	///     <para> If the {@code '0'} flag is given then the output will be padded
	///     with leading zeros to the field width following any indication of sign.
	/// 
	/// </para>
	///     <para> If {@code '('}, {@code '+'}, '&nbsp;&nbsp;', or {@code ','} flags
	///     are given then a <seealso cref="FormatFlagsConversionMismatchException"/> will be
	///     thrown.
	/// 
	/// <tr><td valign="top"> {@code 'x'}
	///     <td valign="top"> <tt>'&#92;u0078'</tt>
	///     <td> Formats the argument as an integer in base sixteen. No
	///     localization is applied.
	/// 
	/// </para>
	///     <para> If <i>x</i> is negative then the result will be an unsigned value
	///     generated by adding 2<sup>n</sup> to the value where {@code n} is the
	///     number of bits in the type as returned by the static {@code SIZE} field
	///     in the <seealso cref="Byte#SIZE Byte"/>, <seealso cref="Short#SIZE Short"/>,
	///     <seealso cref="Integer#SIZE Integer"/>, or <seealso cref="Long#SIZE Long"/>
	///     classes as appropriate.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then the output will always begin
	///     with the radix indicator {@code "0x"}.
	/// 
	/// </para>
	///     <para> If the {@code '0'} flag is given then the output will be padded to
	///     the field width with leading zeros after the radix indicator or sign (if
	///     present).
	/// 
	/// </para>
	///     <para> If {@code '('}, <tt>'&nbsp;&nbsp;'</tt>, {@code '+'}, or
	///     {@code ','} flags are given then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'X'}
	///     <td valign="top"> <tt>'&#92;u0058'</tt>
	///     <td> The upper-case variant of {@code 'x'}.  The entire string
	///     representing the number will be converted to {@linkplain
	///     String#toUpperCase upper case} including the {@code 'x'} (if any) and
	///     all hexadecimal digits {@code 'a'} - {@code 'f'}
	///     (<tt>'&#92;u0061'</tt> -  <tt>'&#92;u0066'</tt>).
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> If the conversion is {@code 'o'}, {@code 'x'}, or {@code 'X'} and
	/// both the {@code '#'} and the {@code '0'} flags are given, then result will
	/// contain the radix indicator ({@code '0'} for octal and {@code "0x"} or
	/// {@code "0X"} for hexadecimal), some number of zeros (based on the width),
	/// and the value.
	/// 
	/// </para>
	/// <para> If the {@code '-'} flag is not given, then the space padding will occur
	/// before the sign.
	/// 
	/// </para>
	/// <para> The following <a name="intFlags">flags</a> apply to numeric integral
	/// conversions:
	/// 
	/// <table cellpadding=5 summary="intFlags">
	/// 
	/// <tr><td valign="top"> {@code '+'}
	///     <td valign="top"> <tt>'&#92;u002b'</tt>
	///     <td> Requires the output to include a positive sign for all positive
	///     numbers.  If this flag is not given then only negative values will
	///     include a sign.
	/// 
	/// </para>
	///     <para> If both the {@code '+'} and <tt>'&nbsp;&nbsp;'</tt> flags are given
	///     then an <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	/// 
	/// <tr><td valign="top"> <tt>'&nbsp;&nbsp;'</tt>
	///     <td valign="top"> <tt>'&#92;u0020'</tt>
	///     <td> Requires the output to include a single extra space
	///     (<tt>'&#92;u0020'</tt>) for non-negative values.
	/// 
	/// </para>
	///     <para> If both the {@code '+'} and <tt>'&nbsp;&nbsp;'</tt> flags are given
	///     then an <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	/// 
	/// <tr><td valign="top"> {@code '0'}
	///     <td valign="top"> <tt>'&#92;u0030'</tt>
	///     <td> Requires the output to be padded with leading {@linkplain
	///     java.text.DecimalFormatSymbols#getZeroDigit zeros} to the minimum field
	///     width following any sign or radix indicator except when converting NaN
	///     or infinity.  If the width is not provided, then a {@link
	///     MissingFormatWidthException} will be thrown.
	/// 
	/// </para>
	///     <para> If both the {@code '-'} and {@code '0'} flags are given then an
	///     <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	/// 
	/// <tr><td valign="top"> {@code ','}
	///     <td valign="top"> <tt>'&#92;u002c'</tt>
	///     <td> Requires the output to include the locale-specific {@linkplain
	///     java.text.DecimalFormatSymbols#getGroupingSeparator group separators} as
	///     described in the <a href="#L10nGroup">"group" section</a> of the
	///     localization algorithm.
	/// 
	/// <tr><td valign="top"> {@code '('}
	///     <td valign="top"> <tt>'&#92;u0028'</tt>
	///     <td> Requires the output to prepend a {@code '('}
	///     (<tt>'&#92;u0028'</tt>) and append a {@code ')'}
	///     (<tt>'&#92;u0029'</tt>) to negative values.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> If no <a name="intdFlags">flags</a> are given the default formatting is
	/// as follows:
	/// 
	/// <ul>
	/// 
	/// <li> The output is right-justified within the {@code width}
	/// 
	/// <li> Negative numbers begin with a {@code '-'} (<tt>'&#92;u002d'</tt>)
	/// 
	/// <li> Positive numbers and zero do not include a sign or extra leading
	/// space
	/// 
	/// <li> No grouping separators are included
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para> The <a name="intWidth">width</a> is the minimum number of characters to
	/// be written to the output.  This includes any signs, digits, grouping
	/// separators, radix indicator, and parentheses.  If the length of the
	/// converted value is less than the width then the output will be padded by
	/// spaces (<tt>'&#92;u0020'</tt>) until the total number of characters equals
	/// width.  The padding is on the left by default.  If {@code '-'} flag is
	/// given then the padding will be on the right.  If width is not specified then
	/// there is no minimum.
	/// 
	/// </para>
	/// <para> The precision is not applicable.  If precision is specified then an
	/// <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	/// 
	/// </para>
	/// <para><a name="dnbint"><b> BigInteger </b></a>
	/// 
	/// </para>
	/// <para> The following conversions may be applied to {@link
	/// java.math.BigInteger}.
	/// 
	/// <table cellpadding=5 summary="BIntConv">
	/// 
	/// <tr><td valign="top"> {@code 'd'}
	///     <td valign="top"> <tt>'&#92;u0064'</tt>
	///     <td> Requires the output to be formatted as a decimal integer. The <a
	///     href="#L10nAlgorithm">localization algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'o'}
	///     <td valign="top"> <tt>'&#92;u006f'</tt>
	///     <td> Requires the output to be formatted as an integer in base eight.
	///     No localization is applied.
	/// 
	/// </para>
	///     <para> If <i>x</i> is negative then the result will be a signed value
	///     beginning with {@code '-'} (<tt>'&#92;u002d'</tt>).  Signed output is
	///     allowed for this type because unlike the primitive types it is not
	///     possible to create an unsigned equivalent without assuming an explicit
	///     data-type size.
	/// 
	/// </para>
	///     <para> If <i>x</i> is positive or zero and the {@code '+'} flag is given
	///     then the result will begin with {@code '+'} (<tt>'&#92;u002b'</tt>).
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then the output will always begin
	///     with {@code '0'} prefix.
	/// 
	/// </para>
	///     <para> If the {@code '0'} flag is given then the output will be padded
	///     with leading zeros to the field width following any indication of sign.
	/// 
	/// </para>
	///     <para> If the {@code ','} flag is given then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'x'}
	///     <td valign="top"> <tt>'&#92;u0078'</tt>
	///     <td> Requires the output to be formatted as an integer in base
	///     sixteen.  No localization is applied.
	/// 
	/// </para>
	///     <para> If <i>x</i> is negative then the result will be a signed value
	///     beginning with {@code '-'} (<tt>'&#92;u002d'</tt>).  Signed output is
	///     allowed for this type because unlike the primitive types it is not
	///     possible to create an unsigned equivalent without assuming an explicit
	///     data-type size.
	/// 
	/// </para>
	///     <para> If <i>x</i> is positive or zero and the {@code '+'} flag is given
	///     then the result will begin with {@code '+'} (<tt>'&#92;u002b'</tt>).
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then the output will always begin
	///     with the radix indicator {@code "0x"}.
	/// 
	/// </para>
	///     <para> If the {@code '0'} flag is given then the output will be padded to
	///     the field width with leading zeros after the radix indicator or sign (if
	///     present).
	/// 
	/// </para>
	///     <para> If the {@code ','} flag is given then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'X'}
	///     <td valign="top"> <tt>'&#92;u0058'</tt>
	///     <td> The upper-case variant of {@code 'x'}.  The entire string
	///     representing the number will be converted to {@linkplain
	///     String#toUpperCase upper case} including the {@code 'x'} (if any) and
	///     all hexadecimal digits {@code 'a'} - {@code 'f'}
	///     (<tt>'&#92;u0061'</tt> - <tt>'&#92;u0066'</tt>).
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> If the conversion is {@code 'o'}, {@code 'x'}, or {@code 'X'} and
	/// both the {@code '#'} and the {@code '0'} flags are given, then result will
	/// contain the base indicator ({@code '0'} for octal and {@code "0x"} or
	/// {@code "0X"} for hexadecimal), some number of zeros (based on the width),
	/// and the value.
	/// 
	/// </para>
	/// <para> If the {@code '0'} flag is given and the value is negative, then the
	/// zero padding will occur after the sign.
	/// 
	/// </para>
	/// <para> If the {@code '-'} flag is not given, then the space padding will occur
	/// before the sign.
	/// 
	/// </para>
	/// <para> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	/// Long apply.  The <a href="#intdFlags">default behavior</a> when no flags are
	/// given is the same as for Byte, Short, Integer, and Long.
	/// 
	/// </para>
	/// <para> The specification of <a href="#intWidth">width</a> is the same as
	/// defined for Byte, Short, Integer, and Long.
	/// 
	/// </para>
	/// <para> The precision is not applicable.  If precision is specified then an
	/// <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	/// 
	/// </para>
	/// <para><a name="dndec"><b> Float and Double</b></a>
	/// 
	/// </para>
	/// <para> The following conversions may be applied to {@code float}, {@link
	/// Float}, {@code double} and <seealso cref="Double"/>.
	/// 
	/// <table cellpadding=5 summary="floatConv">
	/// 
	/// <tr><td valign="top"> {@code 'e'}
	///     <td valign="top"> <tt>'&#92;u0065'</tt>
	///     <td> Requires the output to be formatted using <a
	///     name="scientific">computerized scientific notation</a>.  The <a
	///     href="#L10nAlgorithm">localization algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> The formatting of the magnitude <i>m</i> depends upon its value.
	/// 
	/// </para>
	///     <para> If <i>m</i> is NaN or infinite, the literal strings "NaN" or
	///     "Infinity", respectively, will be output.  These values are not
	///     localized.
	/// 
	/// </para>
	///     <para> If <i>m</i> is positive-zero or negative-zero, then the exponent
	///     will be {@code "+00"}.
	/// 
	/// </para>
	///     <para> Otherwise, the result is a string that represents the sign and
	///     magnitude (absolute value) of the argument.  The formatting of the sign
	///     is described in the <a href="#L10nAlgorithm">localization
	///     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	///     value.
	/// 
	/// </para>
	///     <para> Let <i>n</i> be the unique integer such that 10<sup><i>n</i></sup>
	///     &lt;= <i>m</i> &lt; 10<sup><i>n</i>+1</sup>; then let <i>a</i> be the
	///     mathematically exact quotient of <i>m</i> and 10<sup><i>n</i></sup> so
	///     that 1 &lt;= <i>a</i> &lt; 10. The magnitude is then represented as the
	///     integer part of <i>a</i>, as a single decimal digit, followed by the
	///     decimal separator followed by decimal digits representing the fractional
	///     part of <i>a</i>, followed by the exponent symbol {@code 'e'}
	///     (<tt>'&#92;u0065'</tt>), followed by the sign of the exponent, followed
	///     by a representation of <i>n</i> as a decimal integer, as produced by the
	///     method <seealso cref="Long#toString(long, int)"/>, and zero-padded to include at
	///     least two digits.
	/// 
	/// </para>
	///     <para> The number of digits in the result for the fractional part of
	///     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	///     specified then the default value is {@code 6}. If the precision is less
	///     than the number of digits which would appear after the decimal point in
	///     the string returned by <seealso cref="Float#toString(float)"/> or {@link
	///     Double#toString(double)} respectively, then the value will be rounded
	///     using the {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	///     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	///     For a canonical representation of the value, use {@link
	///     Float#toString(float)} or <seealso cref="Double#toString(double)"/> as
	///     appropriate.
	/// 
	/// </para>
	///     <para>If the {@code ','} flag is given, then an {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'E'}
	///     <td valign="top"> <tt>'&#92;u0045'</tt>
	///     <td> The upper-case variant of {@code 'e'}.  The exponent symbol
	///     will be {@code 'E'} (<tt>'&#92;u0045'</tt>).
	/// 
	/// <tr><td valign="top"> {@code 'g'}
	///     <td valign="top"> <tt>'&#92;u0067'</tt>
	///     <td> Requires the output to be formatted in general scientific notation
	///     as described below. The <a href="#L10nAlgorithm">localization
	///     algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> After rounding for the precision, the formatting of the resulting
	///     magnitude <i>m</i> depends on its value.
	/// 
	/// </para>
	///     <para> If <i>m</i> is greater than or equal to 10<sup>-4</sup> but less
	///     than 10<sup>precision</sup> then it is represented in <i><a
	///     href="#decimal">decimal format</a></i>.
	/// 
	/// </para>
	///     <para> If <i>m</i> is less than 10<sup>-4</sup> or greater than or equal to
	///     10<sup>precision</sup>, then it is represented in <i><a
	///     href="#scientific">computerized scientific notation</a></i>.
	/// 
	/// </para>
	///     <para> The total number of significant digits in <i>m</i> is equal to the
	///     precision.  If the precision is not specified, then the default value is
	///     {@code 6}.  If the precision is {@code 0}, then it is taken to be
	///     {@code 1}.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then an {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'G'}
	///     <td valign="top"> <tt>'&#92;u0047'</tt>
	///     <td> The upper-case variant of {@code 'g'}.
	/// 
	/// <tr><td valign="top"> {@code 'f'}
	///     <td valign="top"> <tt>'&#92;u0066'</tt>
	///     <td> Requires the output to be formatted using <a name="decimal">decimal
	///     format</a>.  The <a href="#L10nAlgorithm">localization algorithm</a> is
	///     applied.
	/// 
	/// </para>
	///     <para> The result is a string that represents the sign and magnitude
	///     (absolute value) of the argument.  The formatting of the sign is
	///     described in the <a href="#L10nAlgorithm">localization
	///     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	///     value.
	/// 
	/// </para>
	///     <para> If <i>m</i> NaN or infinite, the literal strings "NaN" or
	///     "Infinity", respectively, will be output.  These values are not
	///     localized.
	/// 
	/// </para>
	///     <para> The magnitude is formatted as the integer part of <i>m</i>, with no
	///     leading zeroes, followed by the decimal separator followed by one or
	///     more decimal digits representing the fractional part of <i>m</i>.
	/// 
	/// </para>
	///     <para> The number of digits in the result for the fractional part of
	///     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	///     specified then the default value is {@code 6}. If the precision is less
	///     than the number of digits which would appear after the decimal point in
	///     the string returned by <seealso cref="Float#toString(float)"/> or {@link
	///     Double#toString(double)} respectively, then the value will be rounded
	///     using the {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	///     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	///     For a canonical representation of the value, use {@link
	///     Float#toString(float)} or <seealso cref="Double#toString(double)"/> as
	///     appropriate.
	/// 
	/// <tr><td valign="top"> {@code 'a'}
	///     <td valign="top"> <tt>'&#92;u0061'</tt>
	///     <td> Requires the output to be formatted in hexadecimal exponential
	///     form.  No localization is applied.
	/// 
	/// </para>
	///     <para> The result is a string that represents the sign and magnitude
	///     (absolute value) of the argument <i>x</i>.
	/// 
	/// </para>
	///     <para> If <i>x</i> is negative or a negative-zero value then the result
	///     will begin with {@code '-'} (<tt>'&#92;u002d'</tt>).
	/// 
	/// </para>
	///     <para> If <i>x</i> is positive or a positive-zero value and the
	///     {@code '+'} flag is given then the result will begin with {@code '+'}
	///     (<tt>'&#92;u002b'</tt>).
	/// 
	/// </para>
	///     <para> The formatting of the magnitude <i>m</i> depends upon its value.
	/// 
	///     <ul>
	/// 
	///     <li> If the value is NaN or infinite, the literal strings "NaN" or
	///     "Infinity", respectively, will be output.
	/// 
	///     <li> If <i>m</i> is zero then it is represented by the string
	///     {@code "0x0.0p0"}.
	/// 
	///     <li> If <i>m</i> is a {@code double} value with a normalized
	///     representation then substrings are used to represent the significand and
	///     exponent fields.  The significand is represented by the characters
	///     {@code "0x1."} followed by the hexadecimal representation of the rest
	///     of the significand as a fraction.  The exponent is represented by
	///     {@code 'p'} (<tt>'&#92;u0070'</tt>) followed by a decimal string of the
	///     unbiased exponent as if produced by invoking {@link
	///     Integer#toString(int) Integer.toString} on the exponent value.  If the
	///     precision is specified, the value is rounded to the given number of
	///     hexadecimal digits.
	/// 
	///     <li> If <i>m</i> is a {@code double} value with a subnormal
	///     representation then, unless the precision is specified to be in the range
	///     1 through 12, inclusive, the significand is represented by the characters
	///     {@code '0x0.'} followed by the hexadecimal representation of the rest of
	///     the significand as a fraction, and the exponent represented by
	///     {@code 'p-1022'}.  If the precision is in the interval
	///     [1,&nbsp;12], the subnormal value is normalized such that it
	///     begins with the characters {@code '0x1.'}, rounded to the number of
	///     hexadecimal digits of precision, and the exponent adjusted
	///     accordingly.  Note that there must be at least one nonzero digit in a
	///     subnormal significand.
	/// 
	///     </ul>
	/// 
	/// </para>
	///     <para> If the {@code '('} or {@code ','} flags are given, then a {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'A'}
	///     <td valign="top"> <tt>'&#92;u0041'</tt>
	///     <td> The upper-case variant of {@code 'a'}.  The entire string
	///     representing the number will be converted to upper case including the
	///     {@code 'x'} (<tt>'&#92;u0078'</tt>) and {@code 'p'}
	///     (<tt>'&#92;u0070'</tt> and all hexadecimal digits {@code 'a'} -
	///     {@code 'f'} (<tt>'&#92;u0061'</tt> - <tt>'&#92;u0066'</tt>).
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	/// Long apply.
	/// 
	/// </para>
	/// <para> If the {@code '#'} flag is given, then the decimal separator will
	/// always be present.
	/// 
	/// </para>
	/// <para> If no <a name="floatdFlags">flags</a> are given the default formatting
	/// is as follows:
	/// 
	/// <ul>
	/// 
	/// <li> The output is right-justified within the {@code width}
	/// 
	/// <li> Negative numbers begin with a {@code '-'}
	/// 
	/// <li> Positive numbers and positive zero do not include a sign or extra
	/// leading space
	/// 
	/// <li> No grouping separators are included
	/// 
	/// <li> The decimal separator will only appear if a digit follows it
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para> The <a name="floatDWidth">width</a> is the minimum number of characters
	/// to be written to the output.  This includes any signs, digits, grouping
	/// separators, decimal separators, exponential symbol, radix indicator,
	/// parentheses, and strings representing infinity and NaN as applicable.  If
	/// the length of the converted value is less than the width then the output
	/// will be padded by spaces (<tt>'&#92;u0020'</tt>) until the total number of
	/// characters equals width.  The padding is on the left by default.  If the
	/// {@code '-'} flag is given then the padding will be on the right.  If width
	/// is not specified then there is no minimum.
	/// 
	/// </para>
	/// <para> If the <a name="floatDPrec">conversion</a> is {@code 'e'},
	/// {@code 'E'} or {@code 'f'}, then the precision is the number of digits
	/// after the decimal separator.  If the precision is not specified, then it is
	/// assumed to be {@code 6}.
	/// 
	/// </para>
	/// <para> If the conversion is {@code 'g'} or {@code 'G'}, then the precision is
	/// the total number of significant digits in the resulting magnitude after
	/// rounding.  If the precision is not specified, then the default value is
	/// {@code 6}.  If the precision is {@code 0}, then it is taken to be
	/// {@code 1}.
	/// 
	/// </para>
	/// <para> If the conversion is {@code 'a'} or {@code 'A'}, then the precision
	/// is the number of hexadecimal digits after the radix point.  If the
	/// precision is not provided, then all of the digits as returned by {@link
	/// Double#toHexString(double)} will be output.
	/// 
	/// </para>
	/// <para><a name="dnbdec"><b> BigDecimal </b></a>
	/// 
	/// </para>
	/// <para> The following conversions may be applied {@link java.math.BigDecimal
	/// BigDecimal}.
	/// 
	/// <table cellpadding=5 summary="floatConv">
	/// 
	/// <tr><td valign="top"> {@code 'e'}
	///     <td valign="top"> <tt>'&#92;u0065'</tt>
	///     <td> Requires the output to be formatted using <a
	///     name="bscientific">computerized scientific notation</a>.  The <a
	///     href="#L10nAlgorithm">localization algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> The formatting of the magnitude <i>m</i> depends upon its value.
	/// 
	/// </para>
	///     <para> If <i>m</i> is positive-zero or negative-zero, then the exponent
	///     will be {@code "+00"}.
	/// 
	/// </para>
	///     <para> Otherwise, the result is a string that represents the sign and
	///     magnitude (absolute value) of the argument.  The formatting of the sign
	///     is described in the <a href="#L10nAlgorithm">localization
	///     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	///     value.
	/// 
	/// </para>
	///     <para> Let <i>n</i> be the unique integer such that 10<sup><i>n</i></sup>
	///     &lt;= <i>m</i> &lt; 10<sup><i>n</i>+1</sup>; then let <i>a</i> be the
	///     mathematically exact quotient of <i>m</i> and 10<sup><i>n</i></sup> so
	///     that 1 &lt;= <i>a</i> &lt; 10. The magnitude is then represented as the
	///     integer part of <i>a</i>, as a single decimal digit, followed by the
	///     decimal separator followed by decimal digits representing the fractional
	///     part of <i>a</i>, followed by the exponent symbol {@code 'e'}
	///     (<tt>'&#92;u0065'</tt>), followed by the sign of the exponent, followed
	///     by a representation of <i>n</i> as a decimal integer, as produced by the
	///     method <seealso cref="Long#toString(long, int)"/>, and zero-padded to include at
	///     least two digits.
	/// 
	/// </para>
	///     <para> The number of digits in the result for the fractional part of
	///     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	///     specified then the default value is {@code 6}.  If the precision is
	///     less than the number of digits to the right of the decimal point then
	///     the value will be rounded using the
	///     {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	///     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	///     For a canonical representation of the value, use {@link
	///     BigDecimal#toString()}.
	/// 
	/// </para>
	///     <para> If the {@code ','} flag is given, then an {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'E'}
	///     <td valign="top"> <tt>'&#92;u0045'</tt>
	///     <td> The upper-case variant of {@code 'e'}.  The exponent symbol
	///     will be {@code 'E'} (<tt>'&#92;u0045'</tt>).
	/// 
	/// <tr><td valign="top"> {@code 'g'}
	///     <td valign="top"> <tt>'&#92;u0067'</tt>
	///     <td> Requires the output to be formatted in general scientific notation
	///     as described below. The <a href="#L10nAlgorithm">localization
	///     algorithm</a> is applied.
	/// 
	/// </para>
	///     <para> After rounding for the precision, the formatting of the resulting
	///     magnitude <i>m</i> depends on its value.
	/// 
	/// </para>
	///     <para> If <i>m</i> is greater than or equal to 10<sup>-4</sup> but less
	///     than 10<sup>precision</sup> then it is represented in <i><a
	///     href="#bdecimal">decimal format</a></i>.
	/// 
	/// </para>
	///     <para> If <i>m</i> is less than 10<sup>-4</sup> or greater than or equal to
	///     10<sup>precision</sup>, then it is represented in <i><a
	///     href="#bscientific">computerized scientific notation</a></i>.
	/// 
	/// </para>
	///     <para> The total number of significant digits in <i>m</i> is equal to the
	///     precision.  If the precision is not specified, then the default value is
	///     {@code 6}.  If the precision is {@code 0}, then it is taken to be
	///     {@code 1}.
	/// 
	/// </para>
	///     <para> If the {@code '#'} flag is given then an {@link
	///     FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// <tr><td valign="top"> {@code 'G'}
	///     <td valign="top"> <tt>'&#92;u0047'</tt>
	///     <td> The upper-case variant of {@code 'g'}.
	/// 
	/// <tr><td valign="top"> {@code 'f'}
	///     <td valign="top"> <tt>'&#92;u0066'</tt>
	///     <td> Requires the output to be formatted using <a name="bdecimal">decimal
	///     format</a>.  The <a href="#L10nAlgorithm">localization algorithm</a> is
	///     applied.
	/// 
	/// </para>
	///     <para> The result is a string that represents the sign and magnitude
	///     (absolute value) of the argument.  The formatting of the sign is
	///     described in the <a href="#L10nAlgorithm">localization
	///     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	///     value.
	/// 
	/// </para>
	///     <para> The magnitude is formatted as the integer part of <i>m</i>, with no
	///     leading zeroes, followed by the decimal separator followed by one or
	///     more decimal digits representing the fractional part of <i>m</i>.
	/// 
	/// </para>
	///     <para> The number of digits in the result for the fractional part of
	///     <i>m</i> or <i>a</i> is equal to the precision. If the precision is not
	///     specified then the default value is {@code 6}.  If the precision is
	///     less than the number of digits to the right of the decimal point
	///     then the value will be rounded using the
	///     {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	///     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	///     For a canonical representation of the value, use {@link
	///     BigDecimal#toString()}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	/// Long apply.
	/// 
	/// </para>
	/// <para> If the {@code '#'} flag is given, then the decimal separator will
	/// always be present.
	/// 
	/// </para>
	/// <para> The <a href="#floatdFlags">default behavior</a> when no flags are
	/// given is the same as for Float and Double.
	/// 
	/// </para>
	/// <para> The specification of <a href="#floatDWidth">width</a> and <a
	/// href="#floatDPrec">precision</a> is the same as defined for Float and
	/// Double.
	/// 
	/// <h4><a name="ddt">Date/Time</a></h4>
	/// 
	/// </para>
	/// <para> This conversion may be applied to {@code long}, <seealso cref="Long"/>, {@link
	/// Calendar}, <seealso cref="Date"/> and <seealso cref="TemporalAccessor TemporalAccessor"/>
	/// 
	/// <table cellpadding=5 summary="DTConv">
	/// 
	/// <tr><td valign="top"> {@code 't'}
	///     <td valign="top"> <tt>'&#92;u0074'</tt>
	///     <td> Prefix for date and time conversion characters.
	/// <tr><td valign="top"> {@code 'T'}
	///     <td valign="top"> <tt>'&#92;u0054'</tt>
	///     <td> The upper-case variant of {@code 't'}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following date and time conversion character suffixes are defined
	/// for the {@code 't'} and {@code 'T'} conversions.  The types are similar to
	/// but not completely identical to those defined by GNU {@code date} and
	/// POSIX {@code strftime(3c)}.  Additional conversion types are provided to
	/// access Java-specific functionality (e.g. {@code 'L'} for milliseconds
	/// within the second).
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting times:
	/// 
	/// <table cellpadding=5 summary="time">
	/// 
	/// <tr><td valign="top"> {@code 'H'}
	///     <td valign="top"> <tt>'&#92;u0048'</tt>
	///     <td> Hour of the day for the 24-hour clock, formatted as two digits with
	///     a leading zero as necessary i.e. {@code 00 - 23}. {@code 00}
	///     corresponds to midnight.
	/// 
	/// <tr><td valign="top">{@code 'I'}
	///     <td valign="top"> <tt>'&#92;u0049'</tt>
	///     <td> Hour for the 12-hour clock, formatted as two digits with a leading
	///     zero as necessary, i.e.  {@code 01 - 12}.  {@code 01} corresponds to
	///     one o'clock (either morning or afternoon).
	/// 
	/// <tr><td valign="top">{@code 'k'}
	///     <td valign="top"> <tt>'&#92;u006b'</tt>
	///     <td> Hour of the day for the 24-hour clock, i.e. {@code 0 - 23}.
	///     {@code 0} corresponds to midnight.
	/// 
	/// <tr><td valign="top">{@code 'l'}
	///     <td valign="top"> <tt>'&#92;u006c'</tt>
	///     <td> Hour for the 12-hour clock, i.e. {@code 1 - 12}.  {@code 1}
	///     corresponds to one o'clock (either morning or afternoon).
	/// 
	/// <tr><td valign="top">{@code 'M'}
	///     <td valign="top"> <tt>'&#92;u004d'</tt>
	///     <td> Minute within the hour formatted as two digits with a leading zero
	///     as necessary, i.e.  {@code 00 - 59}.
	/// 
	/// <tr><td valign="top">{@code 'S'}
	///     <td valign="top"> <tt>'&#92;u0053'</tt>
	///     <td> Seconds within the minute, formatted as two digits with a leading
	///     zero as necessary, i.e. {@code 00 - 60} ("{@code 60}" is a special
	///     value required to support leap seconds).
	/// 
	/// <tr><td valign="top">{@code 'L'}
	///     <td valign="top"> <tt>'&#92;u004c'</tt>
	///     <td> Millisecond within the second formatted as three digits with
	///     leading zeros as necessary, i.e. {@code 000 - 999}.
	/// 
	/// <tr><td valign="top">{@code 'N'}
	///     <td valign="top"> <tt>'&#92;u004e'</tt>
	///     <td> Nanosecond within the second, formatted as nine digits with leading
	///     zeros as necessary, i.e. {@code 000000000 - 999999999}.  The precision
	///     of this value is limited by the resolution of the underlying operating
	///     system or hardware.
	/// 
	/// <tr><td valign="top">{@code 'p'}
	///     <td valign="top"> <tt>'&#92;u0070'</tt>
	///     <td> Locale-specific {@linkplain
	///     java.text.DateFormatSymbols#getAmPmStrings morning or afternoon} marker
	///     in lower case, e.g."{@code am}" or "{@code pm}".  Use of the
	///     conversion prefix {@code 'T'} forces this output to upper case.  (Note
	///     that {@code 'p'} produces lower-case output.  This is different from
	///     GNU {@code date} and POSIX {@code strftime(3c)} which produce
	///     upper-case output.)
	/// 
	/// <tr><td valign="top">{@code 'z'}
	///     <td valign="top"> <tt>'&#92;u007a'</tt>
	///     <td> <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC&nbsp;822</a>
	///     style numeric time zone offset from GMT, e.g. {@code -0800}.  This
	///     value will be adjusted as necessary for Daylight Saving Time.  For
	///     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	///     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	///     instance of the Java virtual machine.
	/// 
	/// <tr><td valign="top">{@code 'Z'}
	///     <td valign="top"> <tt>'&#92;u005a'</tt>
	///     <td> A string representing the abbreviation for the time zone.  This
	///     value will be adjusted as necessary for Daylight Saving Time.  For
	///     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	///     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	///     instance of the Java virtual machine.  The Formatter's locale will
	///     supersede the locale of the argument (if any).
	/// 
	/// <tr><td valign="top">{@code 's'}
	///     <td valign="top"> <tt>'&#92;u0073'</tt>
	///     <td> Seconds since the beginning of the epoch starting at 1 January 1970
	///     {@code 00:00:00} UTC, i.e. {@code Long.MIN_VALUE/1000} to
	///     {@code Long.MAX_VALUE/1000}.
	/// 
	/// <tr><td valign="top">{@code 'Q'}
	///     <td valign="top"> <tt>'&#92;u004f'</tt>
	///     <td> Milliseconds since the beginning of the epoch starting at 1 January
	///     1970 {@code 00:00:00} UTC, i.e. {@code Long.MIN_VALUE} to
	///     {@code Long.MAX_VALUE}. The precision of this value is limited by
	///     the resolution of the underlying operating system or hardware.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting dates:
	/// 
	/// <table cellpadding=5 summary="date">
	/// 
	/// <tr><td valign="top">{@code 'B'}
	///     <td valign="top"> <tt>'&#92;u0042'</tt>
	///     <td> Locale-specific {@link java.text.DateFormatSymbols#getMonths
	///     full month name}, e.g. {@code "January"}, {@code "February"}.
	/// 
	/// <tr><td valign="top">{@code 'b'}
	///     <td valign="top"> <tt>'&#92;u0062'</tt>
	///     <td> Locale-specific {@linkplain
	///     java.text.DateFormatSymbols#getShortMonths abbreviated month name},
	///     e.g. {@code "Jan"}, {@code "Feb"}.
	/// 
	/// <tr><td valign="top">{@code 'h'}
	///     <td valign="top"> <tt>'&#92;u0068'</tt>
	///     <td> Same as {@code 'b'}.
	/// 
	/// <tr><td valign="top">{@code 'A'}
	///     <td valign="top"> <tt>'&#92;u0041'</tt>
	///     <td> Locale-specific full name of the {@linkplain
	///     java.text.DateFormatSymbols#getWeekdays day of the week},
	///     e.g. {@code "Sunday"}, {@code "Monday"}
	/// 
	/// <tr><td valign="top">{@code 'a'}
	///     <td valign="top"> <tt>'&#92;u0061'</tt>
	///     <td> Locale-specific short name of the {@linkplain
	///     java.text.DateFormatSymbols#getShortWeekdays day of the week},
	///     e.g. {@code "Sun"}, {@code "Mon"}
	/// 
	/// <tr><td valign="top">{@code 'C'}
	///     <td valign="top"> <tt>'&#92;u0043'</tt>
	///     <td> Four-digit year divided by {@code 100}, formatted as two digits
	///     with leading zero as necessary, i.e. {@code 00 - 99}
	/// 
	/// <tr><td valign="top">{@code 'Y'}
	///     <td valign="top"> <tt>'&#92;u0059'</tt> <td> Year, formatted to at least
	///     four digits with leading zeros as necessary, e.g. {@code 0092} equals
	///     {@code 92} CE for the Gregorian calendar.
	/// 
	/// <tr><td valign="top">{@code 'y'}
	///     <td valign="top"> <tt>'&#92;u0079'</tt>
	///     <td> Last two digits of the year, formatted with leading zeros as
	///     necessary, i.e. {@code 00 - 99}.
	/// 
	/// <tr><td valign="top">{@code 'j'}
	///     <td valign="top"> <tt>'&#92;u006a'</tt>
	///     <td> Day of year, formatted as three digits with leading zeros as
	///     necessary, e.g. {@code 001 - 366} for the Gregorian calendar.
	///     {@code 001} corresponds to the first day of the year.
	/// 
	/// <tr><td valign="top">{@code 'm'}
	///     <td valign="top"> <tt>'&#92;u006d'</tt>
	///     <td> Month, formatted as two digits with leading zeros as necessary,
	///     i.e. {@code 01 - 13}, where "{@code 01}" is the first month of the
	///     year and ("{@code 13}" is a special value required to support lunar
	///     calendars).
	/// 
	/// <tr><td valign="top">{@code 'd'}
	///     <td valign="top"> <tt>'&#92;u0064'</tt>
	///     <td> Day of month, formatted as two digits with leading zeros as
	///     necessary, i.e. {@code 01 - 31}, where "{@code 01}" is the first day
	///     of the month.
	/// 
	/// <tr><td valign="top">{@code 'e'}
	///     <td valign="top"> <tt>'&#92;u0065'</tt>
	///     <td> Day of month, formatted as two digits, i.e. {@code 1 - 31} where
	///     "{@code 1}" is the first day of the month.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The following conversion characters are used for formatting common
	/// date/time compositions.
	/// 
	/// <table cellpadding=5 summary="composites">
	/// 
	/// <tr><td valign="top">{@code 'R'}
	///     <td valign="top"> <tt>'&#92;u0052'</tt>
	///     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM"}
	/// 
	/// <tr><td valign="top">{@code 'T'}
	///     <td valign="top"> <tt>'&#92;u0054'</tt>
	///     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM:%tS"}.
	/// 
	/// <tr><td valign="top">{@code 'r'}
	///     <td valign="top"> <tt>'&#92;u0072'</tt>
	///     <td> Time formatted for the 12-hour clock as {@code "%tI:%tM:%tS
	///     %Tp"}.  The location of the morning or afternoon marker
	///     ({@code '%Tp'}) may be locale-dependent.
	/// 
	/// <tr><td valign="top">{@code 'D'}
	///     <td valign="top"> <tt>'&#92;u0044'</tt>
	///     <td> Date formatted as {@code "%tm/%td/%ty"}.
	/// 
	/// <tr><td valign="top">{@code 'F'}
	///     <td valign="top"> <tt>'&#92;u0046'</tt>
	///     <td> <a href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a>
	///     complete date formatted as {@code "%tY-%tm-%td"}.
	/// 
	/// <tr><td valign="top">{@code 'c'}
	///     <td valign="top"> <tt>'&#92;u0063'</tt>
	///     <td> Date and time formatted as {@code "%ta %tb %td %tT %tZ %tY"},
	///     e.g. {@code "Sun Jul 20 16:17:00 EDT 1969"}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> The {@code '-'} flag defined for <a href="#dFlags">General
	/// conversions</a> applies.  If the {@code '#'} flag is given, then a {@link
	/// FormatFlagsConversionMismatchException} will be thrown.
	/// 
	/// </para>
	/// <para> The width is the minimum number of characters to
	/// be written to the output.  If the length of the converted value is less than
	/// the {@code width} then the output will be padded by spaces
	/// (<tt>'&#92;u0020'</tt>) until the total number of characters equals width.
	/// The padding is on the left by default.  If the {@code '-'} flag is given
	/// then the padding will be on the right.  If width is not specified then there
	/// is no minimum.
	/// 
	/// </para>
	/// <para> The precision is not applicable.  If the precision is specified then an
	/// <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	/// 
	/// <h4><a name="dper">Percent</a></h4>
	/// 
	/// </para>
	/// <para> The conversion does not correspond to any argument.
	/// 
	/// <table cellpadding=5 summary="DTConv">
	/// 
	/// <tr><td valign="top">{@code '%'}
	///     <td> The result is a literal {@code '%'} (<tt>'&#92;u0025'</tt>)
	/// 
	/// </para>
	/// <para> The width is the minimum number of characters to
	/// be written to the output including the {@code '%'}.  If the length of the
	/// converted value is less than the {@code width} then the output will be
	/// padded by spaces (<tt>'&#92;u0020'</tt>) until the total number of
	/// characters equals width.  The padding is on the left.  If width is not
	/// specified then just the {@code '%'} is output.
	/// 
	/// </para>
	/// <para> The {@code '-'} flag defined for <a href="#dFlags">General
	/// conversions</a> applies.  If any other flags are provided, then a
	/// <seealso cref="FormatFlagsConversionMismatchException"/> will be thrown.
	/// 
	/// </para>
	/// <para> The precision is not applicable.  If the precision is specified an
	/// <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	/// 
	/// </table>
	/// 
	/// <h4><a name="dls">Line Separator</a></h4>
	/// 
	/// </para>
	/// <para> The conversion does not correspond to any argument.
	/// 
	/// <table cellpadding=5 summary="DTConv">
	/// 
	/// <tr><td valign="top">{@code 'n'}
	///     <td> the platform-specific line separator as returned by {@link
	///     System#getProperty System.getProperty("line.separator")}.
	/// 
	/// </table>
	/// 
	/// </para>
	/// <para> Flags, width, and precision are not applicable.  If any are provided an
	/// <seealso cref="IllegalFormatFlagsException"/>, <seealso cref="IllegalFormatWidthException"/>,
	/// and <seealso cref="IllegalFormatPrecisionException"/>, respectively will be thrown.
	/// 
	/// <h4><a name="dpos">Argument Index</a></h4>
	/// 
	/// </para>
	/// <para> Format specifiers can reference arguments in three ways:
	/// 
	/// <ul>
	/// 
	/// <li> <i>Explicit indexing</i> is used when the format specifier contains an
	/// argument index.  The argument index is a decimal integer indicating the
	/// position of the argument in the argument list.  The first argument is
	/// referenced by "{@code 1$}", the second by "{@code 2$}", etc.  An argument
	/// may be referenced more than once.
	/// 
	/// </para>
	/// <para> For example:
	/// 
	/// <blockquote><pre>
	///   formatter.format("%4$s %3$s %2$s %1$s %4$s %3$s %2$s %1$s",
	///                    "a", "b", "c", "d")
	///   // -&gt; "d c b a d c b a"
	/// </pre></blockquote>
	/// 
	/// <li> <i>Relative indexing</i> is used when the format specifier contains a
	/// {@code '<'} (<tt>'&#92;u003c'</tt>) flag which causes the argument for
	/// the previous format specifier to be re-used.  If there is no previous
	/// argument, then a <seealso cref="MissingFormatArgumentException"/> is thrown.
	/// 
	/// <blockquote><pre>
	///    formatter.format("%s %s %&lt;s %&lt;s", "a", "b", "c", "d")
	///    // -&gt; "a b b b"
	///    // "c" and "d" are ignored because they are not referenced
	/// </pre></blockquote>
	/// 
	/// <li> <i>Ordinary indexing</i> is used when the format specifier contains
	/// neither an argument index nor a {@code '<'} flag.  Each format specifier
	/// which uses ordinary indexing is assigned a sequential implicit index into
	/// argument list which is independent of the indices used by explicit or
	/// relative indexing.
	/// 
	/// <blockquote><pre>
	///   formatter.format("%s %s %s %s", "a", "b", "c", "d")
	///   // -&gt; "a b c d"
	/// </pre></blockquote>
	/// 
	/// </ul>
	/// 
	/// </para>
	/// <para> It is possible to have a format string which uses all forms of indexing,
	/// for example:
	/// 
	/// <blockquote><pre>
	///   formatter.format("%2$s %s %&lt;s %s", "a", "b", "c", "d")
	///   // -&gt; "b a a b"
	///   // "c" and "d" are ignored because they are not referenced
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> The maximum number of arguments is limited by the maximum dimension of a
	/// Java array as defined by
	/// <cite>The Java&trade; Virtual Machine Specification</cite>.
	/// If the argument index is does not correspond to an
	/// available argument, then a <seealso cref="MissingFormatArgumentException"/> is thrown.
	/// 
	/// </para>
	/// <para> If there are more arguments than format specifiers, the extra arguments
	/// are ignored.
	/// 
	/// </para>
	/// <para> Unless otherwise specified, passing a {@code null} argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @author  Iris Clark
	/// @since 1.5
	/// </para>
	/// </summary>
	public sealed class Formatter : Closeable, Flushable
	{
		private Appendable a;
		private readonly Locale l;

		private IOException LastException;

		private readonly char Zero;
		private static double ScaleUp;

		// 1 (sign) + 19 (max # sig digits) + 1 ('.') + 1 ('e') + 1 (sign)
		// + 3 (max # exp digits) + 4 (error) = 30
		private const int MAX_FD_CHARS = 30;

		/// <summary>
		/// Returns a charset object for the given charset name. </summary>
		/// <exception cref="NullPointerException">          is csn is null </exception>
		/// <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.nio.charset.Charset toCharset(String csn) throws java.io.UnsupportedEncodingException
		private static Charset ToCharset(String csn)
		{
			Objects.RequireNonNull(csn, "charsetName");
			try
			{
				return Charset.ForName(csn);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalCharsetNameException | UnsupportedCharsetException unused)
			{
				// UnsupportedEncodingException should be thrown
				throw new UnsupportedEncodingException(csn);
			}
		}

		private static Appendable NonNullAppendable(Appendable a)
		{
			if (a == null)
			{
				return new StringBuilder();
			}

			return a;
		}

		/* Private constructors */
		private Formatter(Locale l, Appendable a)
		{
			this.a = a;
			this.l = l;
			this.Zero = GetZero(l);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Formatter(java.nio.charset.Charset charset, Locale l, java.io.File file) throws java.io.FileNotFoundException
		private Formatter(Charset charset, Locale l, File file) : this(l, new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file), charset)))
		{
		}

		/// <summary>
		/// Constructs a new formatter.
		/// 
		/// <para> The destination of the formatted output is a <seealso cref="StringBuilder"/>
		/// which may be retrieved by invoking <seealso cref="#out out()"/> and whose
		/// current content may be converted into a string by invoking {@link
		/// #toString toString()}.  The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// </para>
		/// </summary>
		public Formatter() : this(Locale.GetDefault(Locale.Category.FORMAT), new StringBuilder())
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified destination.
		/// 
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a">
		///         Destination for the formatted output.  If {@code a} is
		///         {@code null} then a <seealso cref="StringBuilder"/> will be created. </param>
		public Formatter(Appendable a) : this(Locale.GetDefault(Locale.Category.FORMAT), NonNullAppendable(a))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified locale.
		/// 
		/// <para> The destination of the formatted output is a <seealso cref="StringBuilder"/>
		/// which may be retrieved by invoking <seealso cref="#out out()"/> and whose current
		/// content may be converted into a string by invoking {@link #toString
		/// toString()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied. </param>
		public Formatter(Locale l) : this(l, new StringBuilder())
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified destination and locale.
		/// </summary>
		/// <param name="a">
		///         Destination for the formatted output.  If {@code a} is
		///         {@code null} then a <seealso cref="StringBuilder"/> will be created.
		/// </param>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied. </param>
		public Formatter(Appendable a, Locale l) : this(l, NonNullAppendable(a))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file name.
		/// 
		/// <para> The charset used is the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// 
		/// </para>
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this
		///         formatter.  If the file exists then it will be truncated to
		///         zero size; otherwise, a new file will be created.  The output
		///         will be written to the file and is buffered.
		/// </param>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(fileName)} denies write
		///          access to the file
		/// </exception>
		/// <exception cref="FileNotFoundException">
		///          If the given file name does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(String fileName) throws java.io.FileNotFoundException
		public Formatter(String fileName) : this(Locale.GetDefault(Locale.Category.FORMAT), new BufferedWriter(new OutputStreamWriter(new FileOutputStream(fileName))))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file name and charset.
		/// 
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this
		///         formatter.  If the file exists then it will be truncated to
		///         zero size; otherwise, a new file will be created.  The output
		///         will be written to the file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file name does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(fileName)} denies write
		///          access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(String fileName, String csn) throws java.io.FileNotFoundException, java.io.UnsupportedEncodingException
		public Formatter(String fileName, String csn) : this(fileName, csn, Locale.GetDefault(Locale.Category.FORMAT))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file name, charset, and
		/// locale.
		/// </summary>
		/// <param name="fileName">
		///         The name of the file to use as the destination of this
		///         formatter.  If the file exists then it will be truncated to
		///         zero size; otherwise, a new file will be created.  The output
		///         will be written to the file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied.
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file name does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(fileName)} denies write
		///          access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(String fileName, String csn, Locale l) throws java.io.FileNotFoundException, java.io.UnsupportedEncodingException
		public Formatter(String fileName, String csn, Locale l) : this(ToCharset(csn), l, new File(fileName))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file.
		/// 
		/// <para> The charset used is the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// 
		/// </para>
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this formatter.  If the
		///         file exists then it will be truncated to zero size; otherwise,
		///         a new file will be created.  The output will be written to the
		///         file and is buffered.
		/// </param>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		///          write access to the file
		/// </exception>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(java.io.File file) throws java.io.FileNotFoundException
		public Formatter(File file) : this(Locale.GetDefault(Locale.Category.FORMAT), new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file))))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file and charset.
		/// 
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this formatter.  If the
		///         file exists then it will be truncated to zero size; otherwise,
		///         a new file will be created.  The output will be written to the
		///         file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		///          write access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(java.io.File file, String csn) throws java.io.FileNotFoundException, java.io.UnsupportedEncodingException
		public Formatter(File file, String csn) : this(file, csn, Locale.GetDefault(Locale.Category.FORMAT))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified file, charset, and
		/// locale.
		/// </summary>
		/// <param name="file">
		///         The file to use as the destination of this formatter.  If the
		///         file exists then it will be truncated to zero size; otherwise,
		///         a new file will be created.  The output will be written to the
		///         file and is buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied.
		/// </param>
		/// <exception cref="FileNotFoundException">
		///          If the given file object does not denote an existing, writable
		///          regular file and a new regular file of that name cannot be
		///          created, or if some other error occurs while opening or
		///          creating the file
		/// </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is present and {@link
		///          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		///          write access to the file
		/// </exception>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(java.io.File file, String csn, Locale l) throws java.io.FileNotFoundException, java.io.UnsupportedEncodingException
		public Formatter(File file, String csn, Locale l) : this(ToCharset(csn), l, file)
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified print stream.
		/// 
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// <para> Characters are written to the given {@link java.io.PrintStream
		/// PrintStream} object and are therefore encoded using that object's
		/// charset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ps">
		///         The stream to use as the destination of this formatter. </param>
		public Formatter(PrintStream ps) : this(Locale.GetDefault(Locale.Category.FORMAT), (Appendable)Objects.RequireNonNull(ps))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified output stream.
		/// 
		/// <para> The charset used is the {@linkplain
		/// java.nio.charset.Charset#defaultCharset() default charset} for this
		/// instance of the Java virtual machine.
		/// 
		/// </para>
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="os">
		///         The output stream to use as the destination of this formatter.
		///         The output will be buffered. </param>
		public Formatter(OutputStream os) : this(Locale.GetDefault(Locale.Category.FORMAT), new BufferedWriter(new OutputStreamWriter(os)))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified output stream and
		/// charset.
		/// 
		/// <para> The locale used is the {@linkplain
		/// Locale#getDefault(Locale.Category) default locale} for
		/// <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		/// virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="os">
		///         The output stream to use as the destination of this formatter.
		///         The output will be buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(java.io.OutputStream os, String csn) throws java.io.UnsupportedEncodingException
		public Formatter(OutputStream os, String csn) : this(os, csn, Locale.GetDefault(Locale.Category.FORMAT))
		{
		}

		/// <summary>
		/// Constructs a new formatter with the specified output stream, charset,
		/// and locale.
		/// </summary>
		/// <param name="os">
		///         The output stream to use as the destination of this formatter.
		///         The output will be buffered.
		/// </param>
		/// <param name="csn">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied.
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formatter(java.io.OutputStream os, String csn, Locale l) throws java.io.UnsupportedEncodingException
		public Formatter(OutputStream os, String csn, Locale l) : this(l, new BufferedWriter(new OutputStreamWriter(os, csn)))
		{
		}

		private static char GetZero(Locale l)
		{
			if ((l != null) && !l.Equals(Locale.US))
			{
				DecimalFormatSymbols dfs = DecimalFormatSymbols.GetInstance(l);
				return dfs.ZeroDigit;
			}
			else
			{
				return '0';
			}
		}

		/// <summary>
		/// Returns the locale set by the construction of this formatter.
		/// 
		/// <para> The <seealso cref="#format(java.util.Locale,String,Object...) format"/> method
		/// for this object which has a locale argument does not change this value.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code null} if no localization is applied, otherwise a
		///          locale
		/// </returns>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method </exception>
		public Locale Locale()
		{
			EnsureOpen();
			return l;
		}

		/// <summary>
		/// Returns the destination for the output.
		/// </summary>
		/// <returns>  The destination for the output
		/// </returns>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method </exception>
		public Appendable @out()
		{
			EnsureOpen();
			return a;
		}

		/// <summary>
		/// Returns the result of invoking {@code toString()} on the destination
		/// for the output.  For example, the following code formats text into a
		/// <seealso cref="StringBuilder"/> then retrieves the resultant string:
		/// 
		/// <blockquote><pre>
		///   Formatter f = new Formatter();
		///   f.format("Last reboot at %tc", lastRebootDate);
		///   String s = f.toString();
		///   // -&gt; s == "Last reboot at Sat Jan 01 00:00:00 PST 2000"
		/// </pre></blockquote>
		/// 
		/// <para> An invocation of this method behaves in exactly the same way as the
		/// invocation
		/// 
		/// <pre>
		///     out().toString() </pre>
		/// 
		/// </para>
		/// <para> Depending on the specification of {@code toString} for the {@link
		/// Appendable}, the returned string may or may not contain the characters
		/// written to the destination.  For instance, buffers typically return
		/// their contents in {@code toString()}, but streams cannot since the
		/// data is discarded.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The result of invoking {@code toString()} on the destination
		///          for the output
		/// </returns>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method </exception>
		public override String ToString()
		{
			EnsureOpen();
			return a.ToString();
		}

		/// <summary>
		/// Flushes this formatter.  If the destination implements the {@link
		/// java.io.Flushable} interface, its {@code flush} method will be invoked.
		/// 
		/// <para> Flushing a formatter writes any buffered output in the destination
		/// to the underlying stream.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method </exception>
		public void Flush()
		{
			EnsureOpen();
			if (a is Flushable)
			{
				try
				{
					((Flushable)a).Flush();
				}
				catch (IOException ioe)
				{
					LastException = ioe;
				}
			}
		}

		/// <summary>
		/// Closes this formatter.  If the destination implements the {@link
		/// java.io.Closeable} interface, its {@code close} method will be invoked.
		/// 
		/// <para> Closing a formatter allows it to release resources it may be holding
		/// (such as open files).  If the formatter is already closed, then invoking
		/// this method has no effect.
		/// 
		/// </para>
		/// <para> Attempting to invoke any methods except <seealso cref="#ioException()"/> in
		/// this formatter after it has been closed will result in a {@link
		/// FormatterClosedException}.
		/// </para>
		/// </summary>
		public void Close()
		{
			if (a == null)
			{
				return;
			}
			try
			{
				if (a is Closeable)
				{
					((Closeable)a).Close();
				}
			}
			catch (IOException ioe)
			{
				LastException = ioe;
			}
			finally
			{
				a = null;
			}
		}

		private void EnsureOpen()
		{
			if (a == null)
			{
				throw new FormatterClosedException();
			}
		}

		/// <summary>
		/// Returns the {@code IOException} last thrown by this formatter's {@link
		/// Appendable}.
		/// 
		/// <para> If the destination's {@code append()} method never throws
		/// {@code IOException}, then this method will always return {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The last exception thrown by the Appendable or {@code null} if
		///          no such exception exists. </returns>
		public IOException IoException()
		{
			return LastException;
		}

		/// <summary>
		/// Writes a formatted string to this object's destination using the
		/// specified format string and arguments.  The locale used is the one
		/// defined during the construction of this formatter.
		/// </summary>
		/// <param name="format">
		///         A format string as described in <a href="#syntax">Format string
		///         syntax</a>.
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <exception cref="IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a href="#detail">Details</a>
		///          section of the formatter class specification.
		/// </exception>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method
		/// </exception>
		/// <returns>  This formatter </returns>
		public Formatter Format(String format, params Object[] args)
		{
			return Format(l, format, args);
		}

		/// <summary>
		/// Writes a formatted string to this object's destination using the
		/// specified locale, format string, and arguments.
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied.  This does not change this object's locale that was
		///         set during construction.
		/// </param>
		/// <param name="format">
		///         A format string as described in <a href="#syntax">Format string
		///         syntax</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <exception cref="IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a href="#detail">Details</a>
		///          section of the formatter class specification.
		/// </exception>
		/// <exception cref="FormatterClosedException">
		///          If this formatter has been closed by invoking its {@link
		///          #close()} method
		/// </exception>
		/// <returns>  This formatter </returns>
		public Formatter Format(Locale l, String format, params Object[] args)
		{
			EnsureOpen();

			// index of last argument referenced
			int last = -1;
			// last ordinary index
			int lasto = -1;

			FormatString[] fsa = Parse(format);
			for (int i = 0; i < fsa.Length; i++)
			{
				FormatString fs = fsa[i];
				int index = fs.Index();
				try
				{
					switch (index)
					{
					case -2: // fixed string, "%n", or "%%"
						fs.Print(null, l);
						break;
					case -1: // relative index
						if (last < 0 || (args != null && last > args.Length - 1))
						{
							throw new MissingFormatArgumentException(fs.ToString());
						}
						fs.Print((args == null ? null : args[last]), l);
						break;
					case 0: // ordinary index
						lasto++;
						last = lasto;
						if (args != null && lasto > args.Length - 1)
						{
							throw new MissingFormatArgumentException(fs.ToString());
						}
						fs.Print((args == null ? null : args[lasto]), l);
						break;
					default: // explicit index
						last = index - 1;
						if (args != null && last > args.Length - 1)
						{
							throw new MissingFormatArgumentException(fs.ToString());
						}
						fs.Print((args == null ? null : args[last]), l);
						break;
					}
				}
				catch (IOException x)
				{
					LastException = x;
				}
			}
			return this;
		}

		// %[argument_index$][flags][width][.precision][t]conversion
		private const String FormatSpecifier = "%(\\d+\\$)?([-#+ 0,(\\<]*)?(\\d+)?(\\.\\d+)?([tT])?([a-zA-Z%])";

		private static Pattern FsPattern = Pattern.Compile(FormatSpecifier);

		/// <summary>
		/// Finds format specifiers in the format string.
		/// </summary>
		private FormatString[] Parse(String s)
		{
			List<FormatString> al = new List<FormatString>();
			Matcher m = FsPattern.Matcher(s);
			for (int i = 0, len = s.Length(); i < len;)
			{
				if (m.Find(i))
				{
					// Anything between the start of the string and the beginning
					// of the format specifier is either fixed text or contains
					// an invalid format string.
					if (m.Start() != i)
					{
						// Make sure we didn't miss any invalid format specifiers
						CheckText(s, i, m.Start());
						// Assume previous characters were fixed text
						al.Add(new FixedString(this, s.Substring(i, m.Start() - i)));
					}

					al.Add(new FormatSpecifier(this, m));
					i = m.End();
				}
				else
				{
					// No more valid format specifiers.  Check for possible invalid
					// format specifiers.
					CheckText(s, i, len);
					// The rest of the string is fixed text
					al.Add(new FixedString(this, s.Substring(i)));
					break;
				}
			}
			return al.ToArray(new FormatString[al.Size()]);
		}

		private static void CheckText(String s, int start, int end)
		{
			for (int i = start; i < end; i++)
			{
				// Any '%' found in the region starts an invalid format specifier.
				if (s.CharAt(i) == '%')
				{
					char c = (i == end - 1) ? '%' : s.CharAt(i + 1);
					throw new UnknownFormatConversionException(Convert.ToString(c));
				}
			}
		}

		private interface FormatString
		{
			int Index();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void print(Object arg, Locale l) throws java.io.IOException;
			void Print(Object arg, Locale l);
			String ToString();
		}

		private class FixedString : FormatString
		{
			private readonly Formatter OuterInstance;

			internal String s;
			internal FixedString(Formatter outerInstance, String s)
			{
				this.OuterInstance = outerInstance;
				this.s = s;
			}
			public virtual int Index()
			{
				return -2;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void print(Object arg, Locale l) throws java.io.IOException
			public virtual void Print(Object arg, Locale l)
			{
				outerInstance.a.Append(s);
			}
			public override String ToString()
			{
				return s;
			}
		}

		/// <summary>
		/// Enum for {@code BigDecimal} formatting.
		/// </summary>
		public enum BigDecimalLayoutForm
		{
			/// <summary>
			/// Format the {@code BigDecimal} in computerized scientific notation.
			/// </summary>
			SCIENTIFIC,

			/// <summary>
			/// Format the {@code BigDecimal} as a decimal number.
			/// </summary>
			DECIMAL_FLOAT
		}

		private class FormatSpecifier : FormatString
		{
			private readonly Formatter OuterInstance;

			internal int Index_Renamed = -1;
			internal Flags f = Flags.NONE;
			internal int Width_Renamed;
			internal int Precision_Renamed;
			internal bool Dt = false;
			internal char c;

			internal virtual int Index(String s)
			{
				if (s != null)
				{
					try
					{
						Index_Renamed = Convert.ToInt32(s.Substring(0, s.Length() - 1));
					}
					catch (NumberFormatException)
					{
						assert(false);
					}
				}
				else
				{
					Index_Renamed = 0;
				}
				return Index_Renamed;
			}

			public virtual int Index()
			{
				return Index_Renamed;
			}

			internal virtual Flags Flags(String s)
			{
				f = Flags.Parse(s);
				if (f.Contains(Flags.PREVIOUS))
				{
					Index_Renamed = -1;
				}
				return f;
			}

			internal virtual Flags Flags()
			{
				return f;
			}

			internal virtual int Width(String s)
			{
				Width_Renamed = -1;
				if (s != null)
				{
					try
					{
						Width_Renamed = Convert.ToInt32(s);
						if (Width_Renamed < 0)
						{
							throw new IllegalFormatWidthException(Width_Renamed);
						}
					}
					catch (NumberFormatException)
					{
						assert(false);
					}
				}
				return Width_Renamed;
			}

			internal virtual int Width()
			{
				return Width_Renamed;
			}

			internal virtual int Precision(String s)
			{
				Precision_Renamed = -1;
				if (s != null)
				{
					try
					{
						// remove the '.'
						Precision_Renamed = Convert.ToInt32(s.Substring(1));
						if (Precision_Renamed < 0)
						{
							throw new IllegalFormatPrecisionException(Precision_Renamed);
						}
					}
					catch (NumberFormatException)
					{
						assert(false);
					}
				}
				return Precision_Renamed;
			}

			internal virtual int Precision()
			{
				return Precision_Renamed;
			}

			internal virtual char Conversion(String s)
			{
				c = s.CharAt(0);
				if (!Dt)
				{
					if (!Conversion.IsValid(c))
					{
						throw new UnknownFormatConversionException(Convert.ToString(c));
					}
					if (char.IsUpper(c))
					{
						f.Add(Flags.UPPERCASE);
					}
					c = char.ToLower(c);
					if (Conversion.IsText(c))
					{
						Index_Renamed = -2;
					}
				}
				return c;
			}

			internal virtual char Conversion()
			{
				return c;
			}

			internal FormatSpecifier(Formatter outerInstance, Matcher m)
			{
				this.OuterInstance = outerInstance;
				int idx = 1;

				Index(m.Group(idx++));
				Flags(m.Group(idx++));
				Width(m.Group(idx++));
				Precision(m.Group(idx++));

				String tT = m.Group(idx++);
				if (tT != null)
				{
					Dt = true;
					if (tT.Equals("T"))
					{
						f.Add(Flags.UPPERCASE);
					}
				}

				Conversion(m.Group(idx));

				if (Dt)
				{
					CheckDateTime();
				}
				else if (Conversion.IsGeneral(c))
				{
					CheckGeneral();
				}
				else if (Conversion.IsCharacter(c))
				{
					CheckCharacter();
				}
				else if (Conversion.IsInteger(c))
				{
					CheckInteger();
				}
				else if (Conversion.IsFloat(c))
				{
					CheckFloat();
				}
				else if (Conversion.IsText(c))
				{
					CheckText();
				}
				else
				{
					throw new UnknownFormatConversionException(Convert.ToString(c));
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void print(Object arg, Locale l) throws java.io.IOException
			public virtual void Print(Object arg, Locale l)
			{
				if (Dt)
				{
					PrintDateTime(arg, l);
					return;
				}
				switch (c)
				{
				case Conversion.DECIMAL_INTEGER:
				case Conversion.OCTAL_INTEGER:
				case Conversion.HEXADECIMAL_INTEGER:
					PrintInteger(arg, l);
					break;
				case Conversion.SCIENTIFIC:
				case Conversion.GENERAL:
				case Conversion.DECIMAL_FLOAT:
				case Conversion.HEXADECIMAL_FLOAT:
					PrintFloat(arg, l);
					break;
				case Conversion.CHARACTER:
				case Conversion.CHARACTER_UPPER:
					PrintCharacter(arg);
					break;
				case Conversion.BOOLEAN:
					PrintBoolean(arg);
					break;
				case Conversion.STRING:
					PrintString(arg, l);
					break;
				case Conversion.HASHCODE:
					PrintHashCode(arg);
					break;
				case Conversion.LINE_SEPARATOR:
					outerInstance.a.Append(System.lineSeparator());
					break;
				case Conversion.PERCENT_SIGN:
					outerInstance.a.Append('%');
					break;
				default:
					Debug.Assert(false);
				break;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printInteger(Object arg, Locale l) throws java.io.IOException
			internal virtual void PrintInteger(Object arg, Locale l)
			{
				if (arg == null)
				{
					Print("null");
				}
				else if (arg is Byte)
				{
					Print(((Byte)arg).ByteValue(), l);
				}
				else if (arg is Short)
				{
					Print(((Short)arg).ShortValue(), l);
				}
				else if (arg is Integer)
				{
					Print(((Integer)arg).IntValue(), l);
				}
				else if (arg is Long)
				{
					Print(((Long)arg).LongValue(), l);
				}
				else if (arg is System.Numerics.BigInteger)
				{
					Print(((System.Numerics.BigInteger)arg), l);
				}
				else
				{
					FailConversion(c, arg);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printFloat(Object arg, Locale l) throws java.io.IOException
			internal virtual void PrintFloat(Object arg, Locale l)
			{
				if (arg == null)
				{
					Print("null");
				}
				else if (arg is Float)
				{
					Print(((Float)arg).FloatValue(), l);
				}
				else if (arg is Double)
				{
					Print(((Double)arg).DoubleValue(), l);
				}
				else if (arg is decimal)
				{
					Print(((decimal)arg), l);
				}
				else
				{
					FailConversion(c, arg);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printDateTime(Object arg, Locale l) throws java.io.IOException
			internal virtual void PrintDateTime(Object arg, Locale l)
			{
				if (arg == null)
				{
					Print("null");
					return;
				}
				Calendar cal = null;

				// Instead of Calendar.setLenient(true), perhaps we should
				// wrap the IllegalArgumentException that might be thrown?
				if (arg is Long)
				{
					// Note that the following method uses an instance of the
					// default time zone (TimeZone.getDefaultRef().
					cal = Calendar.GetInstance(l == null ? Locale.US : l);
					cal.TimeInMillis = (Long)arg;
				}
				else if (arg is Date)
				{
					// Note that the following method uses an instance of the
					// default time zone (TimeZone.getDefaultRef().
					cal = Calendar.GetInstance(l == null ? Locale.US : l);
					cal.Time = (Date)arg;
				}
				else if (arg is Calendar)
				{
					cal = (Calendar)((Calendar) arg).Clone();
					cal.Lenient = true;
				}
				else if (arg is TemporalAccessor)
				{
					Print((TemporalAccessor) arg, c, l);
					return;
				}
				else
				{
					FailConversion(c, arg);
				}
				// Use the provided locale so that invocations of
				// localizedMagnitude() use optimizations for null.
				Print(cal, c, l);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printCharacter(Object arg) throws java.io.IOException
			internal virtual void PrintCharacter(Object arg)
			{
				if (arg == null)
				{
					Print("null");
					return;
				}
				String s = null;
				if (arg is Character)
				{
					s = ((Character)arg).ToString();
				}
				else if (arg is Byte)
				{
					sbyte i = ((Byte)arg).ByteValue();
					if (Character.IsValidCodePoint(i))
					{
						s = new String(Character.ToChars(i));
					}
					else
					{
						throw new IllegalFormatCodePointException(i);
					}
				}
				else if (arg is Short)
				{
					short i = ((Short)arg).ShortValue();
					if (Character.IsValidCodePoint(i))
					{
						s = new String(Character.ToChars(i));
					}
					else
					{
						throw new IllegalFormatCodePointException(i);
					}
				}
				else if (arg is Integer)
				{
					int i = ((Integer)arg).IntValue();
					if (Character.IsValidCodePoint(i))
					{
						s = new String(Character.ToChars(i));
					}
					else
					{
						throw new IllegalFormatCodePointException(i);
					}
				}
				else
				{
					FailConversion(c, arg);
				}
				Print(s);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printString(Object arg, Locale l) throws java.io.IOException
			internal virtual void PrintString(Object arg, Locale l)
			{
				if (arg is Formattable)
				{
					Formatter fmt = OuterInstance;
					if (fmt.Locale() != l)
					{
						fmt = new Formatter(fmt.@out(), l);
					}
					((Formattable)arg).FormatTo(fmt, f.ValueOf(), Width_Renamed, Precision_Renamed);
				}
				else
				{
					if (f.Contains(Flags.ALTERNATE))
					{
						FailMismatch(Flags.ALTERNATE, 's');
					}
					if (arg == null)
					{
						Print("null");
					}
					else
					{
						Print(arg.ToString());
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printBoolean(Object arg) throws java.io.IOException
			internal virtual void PrintBoolean(Object arg)
			{
				String s;
				if (arg != null)
				{
					s = ((arg is Boolean) ? ((Boolean)arg).ToString() : Convert.ToString(true));
				}
				else
				{
					s = Convert.ToString(false);
				}
				Print(s);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void printHashCode(Object arg) throws java.io.IOException
			internal virtual void PrintHashCode(Object arg)
			{
				String s = (arg == null ? "null" : arg.HashCode().ToString("x"));
				Print(s);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(String s) throws java.io.IOException
			internal virtual void Print(String s)
			{
				if (Precision_Renamed != -1 && Precision_Renamed < s.Length())
				{
					s = s.Substring(0, Precision_Renamed);
				}
				if (f.Contains(Flags.UPPERCASE))
				{
					s = s.ToUpperCase();
				}
				outerInstance.a.Append(Justify(s));
			}

			internal virtual String Justify(String s)
			{
				if (Width_Renamed == -1)
				{
					return s;
				}
				StringBuilder sb = new StringBuilder();
				bool pad = f.Contains(Flags.LEFT_JUSTIFY);
				int sp = Width_Renamed - s.Length();
				if (!pad)
				{
					for (int i = 0; i < sp; i++)
					{
						sb.Append(' ');
					}
				}
				sb.Append(s);
				if (pad)
				{
					for (int i = 0; i < sp; i++)
					{
						sb.Append(' ');
					}
				}
				return sb.ToString();
			}

			public override String ToString()
			{
				StringBuilder sb = new StringBuilder("%");
				// Flags.UPPERCASE is set internally for legal conversions.
				Flags dupf = f.Dup().Remove(Flags.UPPERCASE);
				sb.Append(dupf.ToString());
				if (Index_Renamed > 0)
				{
					sb.Append(Index_Renamed).Append('$');
				}
				if (Width_Renamed != -1)
				{
					sb.Append(Width_Renamed);
				}
				if (Precision_Renamed != -1)
				{
					sb.Append('.').Append(Precision_Renamed);
				}
				if (Dt)
				{
					sb.Append(f.Contains(Flags.UPPERCASE) ? 'T' : 't');
				}
				sb.Append(f.Contains(Flags.UPPERCASE) ? char.ToUpper(c) : c);
				return sb.ToString();
			}

			internal virtual void CheckGeneral()
			{
				if ((c == Conversion.BOOLEAN || c == Conversion.HASHCODE) && f.Contains(Flags.ALTERNATE))
				{
					FailMismatch(Flags.ALTERNATE, c);
				}
				// '-' requires a width
				if (Width_Renamed == -1 && f.Contains(Flags.LEFT_JUSTIFY))
				{
					throw new MissingFormatWidthException(ToString());
				}
				checkBadFlags(Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES);
			}

			internal virtual void CheckDateTime()
			{
				if (Precision_Renamed != -1)
				{
					throw new IllegalFormatPrecisionException(Precision_Renamed);
				}
				if (!DateTime.IsValid(c))
				{
					throw new UnknownFormatConversionException("t" + c);
				}
				checkBadFlags(Flags.ALTERNATE, Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES);
				// '-' requires a width
				if (Width_Renamed == -1 && f.Contains(Flags.LEFT_JUSTIFY))
				{
					throw new MissingFormatWidthException(ToString());
				}
			}

			internal virtual void CheckCharacter()
			{
				if (Precision_Renamed != -1)
				{
					throw new IllegalFormatPrecisionException(Precision_Renamed);
				}
				checkBadFlags(Flags.ALTERNATE, Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES);
				// '-' requires a width
				if (Width_Renamed == -1 && f.Contains(Flags.LEFT_JUSTIFY))
				{
					throw new MissingFormatWidthException(ToString());
				}
			}

			internal virtual void CheckInteger()
			{
				CheckNumeric();
				if (Precision_Renamed != -1)
				{
					throw new IllegalFormatPrecisionException(Precision_Renamed);
				}

				if (c == Conversion.DECIMAL_INTEGER)
				{
					CheckBadFlags(Flags.ALTERNATE);
				}
				else if (c == Conversion.OCTAL_INTEGER)
				{
					CheckBadFlags(Flags.GROUP);
				}
				else
				{
					CheckBadFlags(Flags.GROUP);
				}
			}

			internal virtual void CheckBadFlags(params Flags[] badFlags)
			{
				for (int i = 0; i < badFlags.Length; i++)
				{
					if (f.Contains(badFlags[i]))
					{
						FailMismatch(badFlags[i], c);
					}
				}
			}

			internal virtual void CheckFloat()
			{
				CheckNumeric();
				if (c == Conversion.DECIMAL_FLOAT)
				{
				}
				else if (c == Conversion.HEXADECIMAL_FLOAT)
				{
					checkBadFlags(Flags.PARENTHESES, Flags.GROUP);
				}
				else if (c == Conversion.SCIENTIFIC)
				{
					CheckBadFlags(Flags.GROUP);
				}
				else if (c == Conversion.GENERAL)
				{
					CheckBadFlags(Flags.ALTERNATE);
				}
			}

			internal virtual void CheckNumeric()
			{
				if (Width_Renamed != -1 && Width_Renamed < 0)
				{
					throw new IllegalFormatWidthException(Width_Renamed);
				}

				if (Precision_Renamed != -1 && Precision_Renamed < 0)
				{
					throw new IllegalFormatPrecisionException(Precision_Renamed);
				}

				// '-' and '0' require a width
				if (Width_Renamed == -1 && (f.Contains(Flags.LEFT_JUSTIFY) || f.Contains(Flags.ZERO_PAD)))
				{
					throw new MissingFormatWidthException(ToString());
				}

				// bad combination
				if ((f.Contains(Flags.PLUS) && f.Contains(Flags.LEADING_SPACE)) || (f.Contains(Flags.LEFT_JUSTIFY) && f.Contains(Flags.ZERO_PAD)))
				{
					throw new IllegalFormatFlagsException(f.ToString());
				}
			}

			internal virtual void CheckText()
			{
				if (Precision_Renamed != -1)
				{
					throw new IllegalFormatPrecisionException(Precision_Renamed);
				}
				switch (c)
				{
				case Conversion.PERCENT_SIGN:
					if (f.ValueOf() != Flags.LEFT_JUSTIFY.ValueOf() && f.ValueOf() != Flags.NONE.ValueOf())
					{
						throw new IllegalFormatFlagsException(f.ToString());
					}
					// '-' requires a width
					if (Width_Renamed == -1 && f.Contains(Flags.LEFT_JUSTIFY))
					{
						throw new MissingFormatWidthException(ToString());
					}
					break;
				case Conversion.LINE_SEPARATOR:
					if (Width_Renamed != -1)
					{
						throw new IllegalFormatWidthException(Width_Renamed);
					}
					if (f.ValueOf() != Flags.NONE.ValueOf())
					{
						throw new IllegalFormatFlagsException(f.ToString());
					}
					break;
				default:
					Debug.Assert(false);
				break;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(byte value, Locale l) throws java.io.IOException
			internal virtual void Print(sbyte value, Locale l)
			{
				long v = value;
				if (value < 0 && (c == Conversion.OCTAL_INTEGER || c == Conversion.HEXADECIMAL_INTEGER))
				{
					v += (1L << 8);
					Debug.Assert(v >= 0, v);
				}
				Print(v, l);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(short value, Locale l) throws java.io.IOException
			internal virtual void Print(short value, Locale l)
			{
				long v = value;
				if (value < 0 && (c == Conversion.OCTAL_INTEGER || c == Conversion.HEXADECIMAL_INTEGER))
				{
					v += (1L << 16);
					Debug.Assert(v >= 0, v);
				}
				Print(v, l);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(int value, Locale l) throws java.io.IOException
			internal virtual void Print(int value, Locale l)
			{
				long v = value;
				if (value < 0 && (c == Conversion.OCTAL_INTEGER || c == Conversion.HEXADECIMAL_INTEGER))
				{
					v += (1L << 32);
					Debug.Assert(v >= 0, v);
				}
				Print(v, l);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(long value, Locale l) throws java.io.IOException
			internal virtual void Print(long value, Locale l)
			{

				StringBuilder sb = new StringBuilder();

				if (c == Conversion.DECIMAL_INTEGER)
				{
					bool neg = value < 0;
					char[] va;
					if (value < 0)
					{
						va = Convert.ToString(value, 10).Substring(1).ToCharArray();
					}
					else
					{
						va = Convert.ToString(value, 10).ToCharArray();
					}

					// leading sign indicator
					LeadingSign(sb, neg);

					// the value
					LocalizedMagnitude(sb, va, f, AdjustWidth(Width_Renamed, f, neg), l);

					// trailing sign indicator
					TrailingSign(sb, neg);
				}
				else if (c == Conversion.OCTAL_INTEGER)
				{
					checkBadFlags(Flags.PARENTHESES, Flags.LEADING_SPACE, Flags.PLUS);
					String s = Long.ToOctalString(value);
					int len = (f.Contains(Flags.ALTERNATE) ? s.Length() + 1 : s.Length());

					// apply ALTERNATE (radix indicator for octal) before ZERO_PAD
					if (f.Contains(Flags.ALTERNATE))
					{
						sb.Append('0');
					}
					if (f.Contains(Flags.ZERO_PAD))
					{
						for (int i = 0; i < Width_Renamed - len; i++)
						{
							sb.Append('0');
						}
					}
					sb.Append(s);
				}
				else if (c == Conversion.HEXADECIMAL_INTEGER)
				{
					checkBadFlags(Flags.PARENTHESES, Flags.LEADING_SPACE, Flags.PLUS);
					String s = value.ToString("x");
					int len = (f.Contains(Flags.ALTERNATE) ? s.Length() + 2 : s.Length());

					// apply ALTERNATE (radix indicator for hex) before ZERO_PAD
					if (f.Contains(Flags.ALTERNATE))
					{
						sb.Append(f.Contains(Flags.UPPERCASE) ? "0X" : "0x");
					}
					if (f.Contains(Flags.ZERO_PAD))
					{
						for (int i = 0; i < Width_Renamed - len; i++)
						{
							sb.Append('0');
						}
					}
					if (f.Contains(Flags.UPPERCASE))
					{
						s = s.ToUpperCase();
					}
					sb.Append(s);
				}

				// justify based on width
				outerInstance.a.Append(Justify(sb.ToString()));
			}

			// neg := val < 0
			internal virtual StringBuilder LeadingSign(StringBuilder sb, bool neg)
			{
				if (!neg)
				{
					if (f.Contains(Flags.PLUS))
					{
						sb.Append('+');
					}
					else if (f.Contains(Flags.LEADING_SPACE))
					{
						sb.Append(' ');
					}
				}
				else
				{
					if (f.Contains(Flags.PARENTHESES))
					{
						sb.Append('(');
					}
					else
					{
						sb.Append('-');
					}
				}
				return sb;
			}

			// neg := val < 0
			internal virtual StringBuilder TrailingSign(StringBuilder sb, bool neg)
			{
				if (neg && f.Contains(Flags.PARENTHESES))
				{
					sb.Append(')');
				}
				return sb;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(java.math.BigInteger value, Locale l) throws java.io.IOException
			internal virtual void Print(System.Numerics.BigInteger value, Locale l)
			{
				StringBuilder sb = new StringBuilder();
				bool neg = value.signum() == -1;
				System.Numerics.BigInteger v = value.abs();

				// leading sign indicator
				LeadingSign(sb, neg);

				// the value
				if (c == Conversion.DECIMAL_INTEGER)
				{
					char[] va = v.ToString().ToCharArray();
					LocalizedMagnitude(sb, va, f, AdjustWidth(Width_Renamed, f, neg), l);
				}
				else if (c == Conversion.OCTAL_INTEGER)
				{
					String s = v.ToString(8);

					int len = s.Length() + sb.Length();
					if (neg && f.Contains(Flags.PARENTHESES))
					{
						len++;
					}

					// apply ALTERNATE (radix indicator for octal) before ZERO_PAD
					if (f.Contains(Flags.ALTERNATE))
					{
						len++;
						sb.Append('0');
					}
					if (f.Contains(Flags.ZERO_PAD))
					{
						for (int i = 0; i < Width_Renamed - len; i++)
						{
							sb.Append('0');
						}
					}
					sb.Append(s);
				}
				else if (c == Conversion.HEXADECIMAL_INTEGER)
				{
					String s = v.ToString(16);

					int len = s.Length() + sb.Length();
					if (neg && f.Contains(Flags.PARENTHESES))
					{
						len++;
					}

					// apply ALTERNATE (radix indicator for hex) before ZERO_PAD
					if (f.Contains(Flags.ALTERNATE))
					{
						len += 2;
						sb.Append(f.Contains(Flags.UPPERCASE) ? "0X" : "0x");
					}
					if (f.Contains(Flags.ZERO_PAD))
					{
						for (int i = 0; i < Width_Renamed - len; i++)
						{
							sb.Append('0');
						}
					}
					if (f.Contains(Flags.UPPERCASE))
					{
						s = s.ToUpperCase();
					}
					sb.Append(s);
				}

				// trailing sign indicator
				TrailingSign(sb, (value.signum() == -1));

				// justify based on width
				outerInstance.a.Append(Justify(sb.ToString()));
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(float value, Locale l) throws java.io.IOException
			internal virtual void Print(float value, Locale l)
			{
				Print((double) value, l);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(double value, Locale l) throws java.io.IOException
			internal virtual void Print(double value, Locale l)
			{
				StringBuilder sb = new StringBuilder();
				bool neg = value.CompareTo(0.0) == -1;

				if (!Double.IsNaN(value))
				{
					double v = System.Math.Abs(value);

					// leading sign indicator
					LeadingSign(sb, neg);

					// the value
					if (!Double.IsInfinity(v))
					{
						Print(sb, v, l, f, c, Precision_Renamed, neg);
					}
					else
					{
						sb.Append(f.Contains(Flags.UPPERCASE) ? "INFINITY" : "Infinity");
					}

					// trailing sign indicator
					TrailingSign(sb, neg);
				}
				else
				{
					sb.Append(f.Contains(Flags.UPPERCASE) ? "NAN" : "NaN");
				}

				// justify based on width
				outerInstance.a.Append(Justify(sb.ToString()));
			}

			// !Double.isInfinite(value) && !Double.isNaN(value)
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(StringBuilder sb, double value, Locale l, Flags f, char c, int precision, boolean neg) throws java.io.IOException
			internal virtual void Print(StringBuilder sb, double value, Locale l, Flags f, char c, int precision, bool neg)
			{
				if (c == Conversion.SCIENTIFIC)
				{
					// Create a new FormattedFloatingDecimal with the desired
					// precision.
					int prec = (precision == -1 ? 6 : precision);

					FormattedFloatingDecimal fd = FormattedFloatingDecimal.valueOf(value, prec, FormattedFloatingDecimal.Form.SCIENTIFIC);

					char[] mant = AddZeros(fd.Mantissa, prec);

					// If the precision is zero and the '#' flag is set, add the
					// requested decimal point.
					if (f.Contains(Flags.ALTERNATE) && (prec == 0))
					{
						mant = AddDot(mant);
					}

					char[] exp = (value == 0.0) ? new char[] {'+','0','0'} : fd.Exponent;

					int newW = Width_Renamed;
					if (Width_Renamed != -1)
					{
						newW = AdjustWidth(Width_Renamed - exp.Length - 1, f, neg);
					}
					LocalizedMagnitude(sb, mant, f, newW, l);

					sb.Append(f.Contains(Flags.UPPERCASE) ? 'E' : 'e');

					Flags flags = f.Dup().Remove(Flags.GROUP);
					char sign = exp[0];
					assert(sign == '+' || sign == '-');
					sb.Append(sign);

					char[] tmp = new char[exp.Length - 1];
					System.Array.Copy(exp, 1, tmp, 0, exp.Length - 1);
					sb.Append(LocalizedMagnitude(null, tmp, flags, -1, l));
				}
				else if (c == Conversion.DECIMAL_FLOAT)
				{
					// Create a new FormattedFloatingDecimal with the desired
					// precision.
					int prec = (precision == -1 ? 6 : precision);

					FormattedFloatingDecimal fd = FormattedFloatingDecimal.valueOf(value, prec, FormattedFloatingDecimal.Form.DECIMAL_FLOAT);

					char[] mant = AddZeros(fd.Mantissa, prec);

					// If the precision is zero and the '#' flag is set, add the
					// requested decimal point.
					if (f.Contains(Flags.ALTERNATE) && (prec == 0))
					{
						mant = AddDot(mant);
					}

					int newW = Width_Renamed;
					if (Width_Renamed != -1)
					{
						newW = AdjustWidth(Width_Renamed, f, neg);
					}
					LocalizedMagnitude(sb, mant, f, newW, l);
				}
				else if (c == Conversion.GENERAL)
				{
					int prec = precision;
					if (precision == -1)
					{
						prec = 6;
					}
					else if (precision == 0)
					{
						prec = 1;
					}

					char[] exp;
					char[] mant;
					int expRounded;
					if (value == 0.0)
					{
						exp = null;
						mant = new char[] {'0'};
						expRounded = 0;
					}
					else
					{
						FormattedFloatingDecimal fd = FormattedFloatingDecimal.valueOf(value, prec, FormattedFloatingDecimal.Form.GENERAL);
						exp = fd.Exponent;
						mant = fd.Mantissa;
						expRounded = fd.ExponentRounded;
					}

					if (exp != null)
					{
						prec -= 1;
					}
					else
					{
						prec -= expRounded + 1;
					}

					mant = AddZeros(mant, prec);
					// If the precision is zero and the '#' flag is set, add the
					// requested decimal point.
					if (f.Contains(Flags.ALTERNATE) && (prec == 0))
					{
						mant = AddDot(mant);
					}

					int newW = Width_Renamed;
					if (Width_Renamed != -1)
					{
						if (exp != null)
						{
							newW = AdjustWidth(Width_Renamed - exp.Length - 1, f, neg);
						}
						else
						{
							newW = AdjustWidth(Width_Renamed, f, neg);
						}
					}
					LocalizedMagnitude(sb, mant, f, newW, l);

					if (exp != null)
					{
						sb.Append(f.Contains(Flags.UPPERCASE) ? 'E' : 'e');

						Flags flags = f.Dup().Remove(Flags.GROUP);
						char sign = exp[0];
						assert(sign == '+' || sign == '-');
						sb.Append(sign);

						char[] tmp = new char[exp.Length - 1];
						System.Array.Copy(exp, 1, tmp, 0, exp.Length - 1);
						sb.Append(LocalizedMagnitude(null, tmp, flags, -1, l));
					}
				}
				else if (c == Conversion.HEXADECIMAL_FLOAT)
				{
					int prec = precision;
					if (precision == -1)
					{
						// assume that we want all of the digits
						prec = 0;
					}
					else if (precision == 0)
					{
						prec = 1;
					}

					String s = HexDouble(value, prec);

					char[] va;
					bool upper = f.Contains(Flags.UPPERCASE);
					sb.Append(upper ? "0X" : "0x");

					if (f.Contains(Flags.ZERO_PAD))
					{
						for (int i = 0; i < Width_Renamed - s.Length() - 2; i++)
						{
							sb.Append('0');
						}
					}

					int idx = s.IndexOf('p');
					va = s.Substring(0, idx).ToCharArray();
					if (upper)
					{
						String tmp = new String(va);
						// don't localize hex
						tmp = tmp.ToUpperCase(Locale.US);
						va = tmp.ToCharArray();
					}
					sb.Append(prec != 0 ? AddZeros(va, prec) : va);
					sb.Append(upper ? 'P' : 'p');
					sb.Append(s.Substring(idx + 1));
				}
			}

			// Add zeros to the requested precision.
			internal virtual char[] AddZeros(char[] v, int prec)
			{
				// Look for the dot.  If we don't find one, the we'll need to add
				// it before we add the zeros.
				int i;
				for (i = 0; i < v.Length; i++)
				{
					if (v[i] == '.')
					{
						break;
					}
				}
				bool needDot = false;
				if (i == v.Length)
				{
					needDot = true;
				}

				// Determine existing precision.
				int outPrec = v.Length - i - (needDot ? 0 : 1);
				assert(outPrec <= prec);
				if (outPrec == prec)
				{
					return v;
				}

				// Create new array with existing contents.
				char[] tmp = new char[v.Length + prec - outPrec + (needDot ? 1 : 0)];
				System.Array.Copy(v, 0, tmp, 0, v.Length);

				// Add dot if previously determined to be necessary.
				int start = v.Length;
				if (needDot)
				{
					tmp[v.Length] = '.';
					start++;
				}

				// Add zeros.
				for (int j = start; j < tmp.Length; j++)
				{
					tmp[j] = '0';
				}

				return tmp;
			}

			// Method assumes that d > 0.
			internal virtual String HexDouble(double d, int prec)
			{
				// Let Double.toHexString handle simple cases
				if (!Double.IsFinite(d) || d == 0.0 || prec == 0 || prec >= 13)
				{
					// remove "0x"
					return Double.ToHexString(d).Substring(2);
				}
				else
				{
					assert(prec >= 1 && prec <= 12);

					int exponent = Math.GetExponent(d);
					bool subnormal = (exponent == DoubleConsts.MIN_EXPONENT - 1);

					// If this is subnormal input so normalize (could be faster to
					// do as integer operation).
					if (subnormal)
					{
						ScaleUp = Math.Scalb(1.0, 54);
						d *= ScaleUp;
						// Calculate the exponent.  This is not just exponent + 54
						// since the former is not the normalized exponent.
						exponent = Math.GetExponent(d);
						Debug.Assert(exponent >= DoubleConsts.MIN_EXPONENT && exponent <= DoubleConsts.MAX_EXPONENT, exponent);
					}

					int precision = 1 + prec * 4;
					int shiftDistance = DoubleConsts.SIGNIFICAND_WIDTH - precision;
					assert(shiftDistance >= 1 && shiftDistance < DoubleConsts.SIGNIFICAND_WIDTH);

					long doppel = Double.DoubleToLongBits(d);
					// Deterime the number of bits to keep.
					long newSignif = (doppel & (DoubleConsts.EXP_BIT_MASK | DoubleConsts.SIGNIF_BIT_MASK)) >> shiftDistance;
					// Bits to round away.
					long roundingBits = doppel & ~(~0L << shiftDistance);

					// To decide how to round, look at the low-order bit of the
					// working significand, the highest order discarded bit (the
					// round bit) and whether any of the lower order discarded bits
					// are nonzero (the sticky bit).

					bool leastZero = (newSignif & 0x1L) == 0L;
					bool round = ((1L << (shiftDistance - 1)) & roundingBits) != 0L;
					bool sticky = shiftDistance > 1 && (~(1L << (shiftDistance - 1)) & roundingBits) != 0;
					if ((leastZero && round && sticky) || (!leastZero && round))
					{
						newSignif++;
					}

					long signBit = doppel & DoubleConsts.SIGN_BIT_MASK;
					newSignif = signBit | (newSignif << shiftDistance);
					double result = Double.longBitsToDouble(newSignif);

					if (Double.IsInfinity(result))
					{
						// Infinite result generated by rounding
						return "1.0p1024";
					}
					else
					{
						String res = Double.ToHexString(result).Substring(2);
						if (!subnormal)
						{
							return res;
						}
						else
						{
							// Create a normalized subnormal string.
							int idx = res.IndexOf('p');
							if (idx == -1)
							{
								// No 'p' character in hex string.
								Debug.Assert(false);
								return null;
							}
							else
							{
								// Get exponent and append at the end.
								String exp = res.Substring(idx + 1);
								int iexp = Convert.ToInt32(exp) - 54;
								return res.Substring(0, idx) + "p" + Convert.ToString(iexp);
							}
						}
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(java.math.BigDecimal value, Locale l) throws java.io.IOException
			internal virtual void Print(decimal value, Locale l)
			{
				if (c == Conversion.HEXADECIMAL_FLOAT)
				{
					FailConversion(c, value);
				}
				StringBuilder sb = new StringBuilder();
				bool neg = value.Signum() == -1;
				decimal v = value.Abs();
				// leading sign indicator
				LeadingSign(sb, neg);

				// the value
				Print(sb, v, l, f, c, Precision_Renamed, neg);

				// trailing sign indicator
				TrailingSign(sb, neg);

				// justify based on width
				outerInstance.a.Append(Justify(sb.ToString()));
			}

			// value > 0
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(StringBuilder sb, java.math.BigDecimal value, Locale l, Flags f, char c, int precision, boolean neg) throws java.io.IOException
			internal virtual void Print(StringBuilder sb, decimal value, Locale l, Flags f, char c, int precision, bool neg)
			{
				if (c == Conversion.SCIENTIFIC)
				{
					// Create a new BigDecimal with the desired precision.
					int prec = (precision == -1 ? 6 : precision);
					int scale = value.Scale();
					int origPrec = value.Precision();
					int nzeros = 0;
					int compPrec;

					if (prec > origPrec - 1)
					{
						compPrec = origPrec;
						nzeros = prec - (origPrec - 1);
					}
					else
					{
						compPrec = prec + 1;
					}

					MathContext mc = new MathContext(compPrec);
					decimal v = new decimal(value.UnscaledValue(), scale, mc);

					BigDecimalLayout bdl = new BigDecimalLayout(this, v.UnscaledValue(), v.Scale(), BigDecimalLayoutForm.SCIENTIFIC);

					char[] mant = bdl.Mantissa();

					// Add a decimal point if necessary.  The mantissa may not
					// contain a decimal point if the scale is zero (the internal
					// representation has no fractional part) or the original
					// precision is one. Append a decimal point if '#' is set or if
					// we require zero padding to get to the requested precision.
					if ((origPrec == 1 || !bdl.HasDot()) && (nzeros > 0 || (f.Contains(Flags.ALTERNATE))))
					{
						mant = AddDot(mant);
					}

					// Add trailing zeros in the case precision is greater than
					// the number of available digits after the decimal separator.
					mant = TrailingZeros(mant, nzeros);

					char[] exp = bdl.Exponent();
					int newW = Width_Renamed;
					if (Width_Renamed != -1)
					{
						newW = AdjustWidth(Width_Renamed - exp.Length - 1, f, neg);
					}
					LocalizedMagnitude(sb, mant, f, newW, l);

					sb.Append(f.Contains(Flags.UPPERCASE) ? 'E' : 'e');

					Flags flags = f.Dup().Remove(Flags.GROUP);
					char sign = exp[0];
					assert(sign == '+' || sign == '-');
					sb.Append(exp[0]);

					char[] tmp = new char[exp.Length - 1];
					System.Array.Copy(exp, 1, tmp, 0, exp.Length - 1);
					sb.Append(LocalizedMagnitude(null, tmp, flags, -1, l));
				}
				else if (c == Conversion.DECIMAL_FLOAT)
				{
					// Create a new BigDecimal with the desired precision.
					int prec = (precision == -1 ? 6 : precision);
					int scale = value.Scale();

					if (scale > prec)
					{
						// more "scale" digits than the requested "precision"
						int compPrec = value.Precision();
						if (compPrec <= scale)
						{
							// case of 0.xxxxxx
							value = value.SetScale(prec, RoundingMode.HALF_UP);
						}
						else
						{
							compPrec -= (scale - prec);
							value = new decimal(value.UnscaledValue(), scale, new MathContext(compPrec));
						}
					}
					BigDecimalLayout bdl = new BigDecimalLayout(this, value.UnscaledValue(), value.Scale(), BigDecimalLayoutForm.DECIMAL_FLOAT);

					char[] mant = bdl.Mantissa();
					int nzeros = (bdl.Scale() < prec ? prec - bdl.Scale() : 0);

					// Add a decimal point if necessary.  The mantissa may not
					// contain a decimal point if the scale is zero (the internal
					// representation has no fractional part).  Append a decimal
					// point if '#' is set or we require zero padding to get to the
					// requested precision.
					if (bdl.Scale() == 0 && (f.Contains(Flags.ALTERNATE) || nzeros > 0))
					{
						mant = AddDot(bdl.Mantissa());
					}

					// Add trailing zeros if the precision is greater than the
					// number of available digits after the decimal separator.
					mant = TrailingZeros(mant, nzeros);

					LocalizedMagnitude(sb, mant, f, AdjustWidth(Width_Renamed, f, neg), l);
				}
				else if (c == Conversion.GENERAL)
				{
					int prec = precision;
					if (precision == -1)
					{
						prec = 6;
					}
					else if (precision == 0)
					{
						prec = 1;
					}

					decimal tenToTheNegFour = decimal.ValueOf(1, 4);
					decimal tenToThePrec = decimal.ValueOf(1, -prec);
					if ((value.Equals(decimal.Zero)) || ((value.CompareTo(tenToTheNegFour) != -1) && (value.CompareTo(tenToThePrec) == -1)))
					{

						int e = - value.Scale() + (value.UnscaledValue().ToString().Length - 1);

						// xxx.yyy
						//   g precision (# sig digits) = #x + #y
						//   f precision = #y
						//   exponent = #x - 1
						// => f precision = g precision - exponent - 1
						// 0.000zzz
						//   g precision (# sig digits) = #z
						//   f precision = #0 (after '.') + #z
						//   exponent = - #0 (after '.') - 1
						// => f precision = g precision - exponent - 1
						prec = prec - e - 1;

						Print(sb, value, l, f, Conversion.DECIMAL_FLOAT, prec, neg);
					}
					else
					{
						Print(sb, value, l, f, Conversion.SCIENTIFIC, prec - 1, neg);
					}
				}
				else if (c == Conversion.HEXADECIMAL_FLOAT)
				{
					// This conversion isn't supported.  The error should be
					// reported earlier.
					Debug.Assert(false);
				}
			}

			private class BigDecimalLayout
			{
				private readonly Formatter.FormatSpecifier OuterInstance;

				internal StringBuilder Mant;
				internal StringBuilder Exp;
				internal bool Dot = false;
				internal int Scale_Renamed;

				public BigDecimalLayout(Formatter.FormatSpecifier outerInstance, System.Numerics.BigInteger intVal, int scale, BigDecimalLayoutForm form)
				{
					this.OuterInstance = outerInstance;
					Layout(intVal, scale, form);
				}

				public virtual bool HasDot()
				{
					return Dot;
				}

				public virtual int Scale()
				{
					return Scale_Renamed;
				}

				// char[] with canonical string representation
				public virtual char[] LayoutChars()
				{
					StringBuilder sb = new StringBuilder(Mant);
					if (Exp != null)
					{
						sb.Append('E');
						sb.Append(Exp);
					}
					return ToCharArray(sb);
				}

				public virtual char[] Mantissa()
				{
					return ToCharArray(Mant);
				}

				// The exponent will be formatted as a sign ('+' or '-') followed
				// by the exponent zero-padded to include at least two digits.
				public virtual char[] Exponent()
				{
					return ToCharArray(Exp);
				}

				internal virtual char[] ToCharArray(StringBuilder sb)
				{
					if (sb == null)
					{
						return null;
					}
					char[] result = new char[sb.Length()];
					sb.GetChars(0, result.Length, result, 0);
					return result;
				}

				internal virtual void Layout(System.Numerics.BigInteger intVal, int scale, BigDecimalLayoutForm form)
				{
					char[] coeff = intVal.ToString().ToCharArray();
					this.Scale_Renamed = scale;

					// Construct a buffer, with sufficient capacity for all cases.
					// If E-notation is needed, length will be: +1 if negative, +1
					// if '.' needed, +2 for "E+", + up to 10 for adjusted
					// exponent.  Otherwise it could have +1 if negative, plus
					// leading "0.00000"
					Mant = new StringBuilder(coeff.Length + 14);

					if (scale == 0)
					{
						int len = coeff.Length;
						if (len > 1)
						{
							Mant.Append(coeff[0]);
							if (form == BigDecimalLayoutForm.SCIENTIFIC)
							{
								Mant.Append('.');
								Dot = true;
								Mant.Append(coeff, 1, len - 1);
								Exp = new StringBuilder("+");
								if (len < 10)
								{
									Exp.Append("0").Append(len - 1);
								}
								else
								{
									Exp.Append(len - 1);
								}
							}
							else
							{
								Mant.Append(coeff, 1, len - 1);
							}
						}
						else
						{
							Mant.Append(coeff);
							if (form == BigDecimalLayoutForm.SCIENTIFIC)
							{
								Exp = new StringBuilder("+00");
							}
						}
						return;
					}
					long adjusted = -(long) scale + (coeff.Length - 1);
					if (form == BigDecimalLayoutForm.DECIMAL_FLOAT)
					{
						// count of padding zeros
						int pad = scale - coeff.Length;
						if (pad >= 0)
						{
							// 0.xxx form
							Mant.Append("0.");
							Dot = true;
							for (; pad > 0 ; pad--)
							{
								Mant.Append('0');
							}
							Mant.Append(coeff);
						}
						else
						{
							if (-pad < coeff.Length)
							{
								// xx.xx form
								Mant.Append(coeff, 0, -pad);
								Mant.Append('.');
								Dot = true;
								Mant.Append(coeff, -pad, scale);
							}
							else
							{
								// xx form
								Mant.Append(coeff, 0, coeff.Length);
								for (int i = 0; i < -scale; i++)
								{
									Mant.Append('0');
								}
								this.Scale_Renamed = 0;
							}
						}
					}
					else
					{
						// x.xxx form
						Mant.Append(coeff[0]);
						if (coeff.Length > 1)
						{
							Mant.Append('.');
							Dot = true;
							Mant.Append(coeff, 1, coeff.Length - 1);
						}
						Exp = new StringBuilder();
						if (adjusted != 0)
						{
							long abs = System.Math.Abs(adjusted);
							// require sign
							Exp.Append(adjusted < 0 ? '-' : '+');
							if (abs < 10)
							{
								Exp.Append('0');
							}
							Exp.Append(abs);
						}
						else
						{
							Exp.Append("+00");
						}
					}
				}
			}

			internal virtual int AdjustWidth(int width, Flags f, bool neg)
			{
				int newW = width;
				if (newW != -1 && neg && f.Contains(Flags.PARENTHESES))
				{
					newW--;
				}
				return newW;
			}

			// Add a '.' to th mantissa if required
			internal virtual char[] AddDot(char[] mant)
			{
				char[] tmp = mant;
				tmp = new char[mant.Length + 1];
				System.Array.Copy(mant, 0, tmp, 0, mant.Length);
				tmp[tmp.Length - 1] = '.';
				return tmp;
			}

			// Add trailing zeros in the case precision is greater than the number
			// of available digits after the decimal separator.
			internal virtual char[] TrailingZeros(char[] mant, int nzeros)
			{
				char[] tmp = mant;
				if (nzeros > 0)
				{
					tmp = new char[mant.Length + nzeros];
					System.Array.Copy(mant, 0, tmp, 0, mant.Length);
					for (int i = mant.Length; i < tmp.Length; i++)
					{
						tmp[i] = '0';
					}
				}
				return tmp;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(Calendar t, char c, Locale l) throws java.io.IOException
			internal virtual void Print(Calendar t, char c, Locale l)
			{
				StringBuilder sb = new StringBuilder();
				Print(sb, t, c, l);

				// justify based on width
				String s = Justify(sb.ToString());
				if (f.Contains(Flags.UPPERCASE))
				{
					s = s.ToUpperCase();
				}

				outerInstance.a.Append(s);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Appendable print(StringBuilder sb, Calendar t, char c, Locale l) throws java.io.IOException
			internal virtual Appendable Print(StringBuilder sb, Calendar t, char c, Locale l)
			{
				if (sb == null)
				{
					sb = new StringBuilder();
				}
				switch (c)
				{
				case DateTime.HOUR_OF_DAY_0: // 'H' (00 - 23)
				case DateTime.HOUR_0: // 'I' (01 - 12)
				case DateTime.HOUR_OF_DAY: // 'k' (0 - 23) -- like H
				case DateTime.HOUR: // 'l' (1 - 12) -- like I
				{
					int i = t.Get(Calendar.HOUR_OF_DAY);
					if (c == DateTime.HOUR_0 || c == DateTime.HOUR)
					{
						i = (i == 0 || i == 12 ? 12 : i % 12);
					}
					Flags flags = (c == DateTime.HOUR_OF_DAY_0 || c == DateTime.HOUR_0 ? Flags.ZERO_PAD : Flags.NONE);
					sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
					break;
				}
				case DateTime.MINUTE: // 'M' (00 - 59)
				{
					int i = t.Get(Calendar.MINUTE);
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
					break;
				}
				case DateTime.NANOSECOND: // 'N' (000000000 - 999999999)
				{
					int i = t.Get(Calendar.MILLISECOND) * 1000000;
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 9, l));
					break;
				}
				case DateTime.MILLISECOND: // 'L' (000 - 999)
				{
					int i = t.Get(Calendar.MILLISECOND);
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 3, l));
					break;
				}
				case DateTime.MILLISECOND_SINCE_EPOCH: // 'Q' (0 - 99...?)
				{
					long i = t.TimeInMillis;
					Flags flags = Flags.NONE;
					sb.Append(LocalizedMagnitude(null, i, flags, Width_Renamed, l));
					break;
				}
				case DateTime.AM_PM: // 'p' (am or pm)
				{
					// Calendar.AM = 0, Calendar.PM = 1, LocaleElements defines upper
					String[] ampm = new String[] {"AM", "PM"};
					if (l != null && l != Locale.US)
					{
						DateFormatSymbols dfs = DateFormatSymbols.GetInstance(l);
						ampm = dfs.AmPmStrings;
					}
					String s = ampm[t.Get(Calendar.AM_PM)];
					sb.Append(s.ToLowerCase(l != null ? l : Locale.US));
					break;
				}
				case DateTime.SECONDS_SINCE_EPOCH: // 's' (0 - 99...?)
				{
					long i = t.TimeInMillis / 1000;
					Flags flags = Flags.NONE;
					sb.Append(LocalizedMagnitude(null, i, flags, Width_Renamed, l));
					break;
				}
				case DateTime.SECOND: // 'S' (00 - 60 - leap second)
				{
					int i = t.Get(Calendar.SECOND);
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
					break;
				}
				case DateTime.ZONE_NUMERIC: // 'z' ({-|+}####) - ls minus?
				{
					int i = t.Get(Calendar.ZONE_OFFSET) + t.Get(Calendar.DST_OFFSET);
					bool neg = i < 0;
					sb.Append(neg ? '-' : '+');
					if (neg)
					{
						i = -i;
					}
					int min = i / 60000;
					// combine minute and hour into a single integer
					int offset = (min / 60) * 100 + (min % 60);
					Flags flags = Flags.ZERO_PAD;

					sb.Append(LocalizedMagnitude(null, offset, flags, 4, l));
					break;
				}
				case DateTime.ZONE: // 'Z' (symbol)
				{
					TimeZone tz = t.TimeZone;
					sb.Append(tz.GetDisplayName((t.Get(Calendar.DST_OFFSET) != 0), TimeZone.SHORT, (l == null) ? Locale.US : l));
					break;
				}

				// Date
				case DateTime.NAME_OF_DAY_ABBREV: // 'a'
				case DateTime.NAME_OF_DAY: // 'A'
				{
					int i = t.Get(Calendar.DAY_OF_WEEK);
					Locale lt = ((l == null) ? Locale.US : l);
					DateFormatSymbols dfs = DateFormatSymbols.GetInstance(lt);
					if (c == DateTime.NAME_OF_DAY)
					{
						sb.Append(dfs.Weekdays[i]);
					}
					else
					{
						sb.Append(dfs.ShortWeekdays[i]);
					}
					break;
				}
				case DateTime.NAME_OF_MONTH_ABBREV: // 'b'
				case DateTime.NAME_OF_MONTH_ABBREV_X: // 'h' -- same b
				case DateTime.NAME_OF_MONTH: // 'B'
				{
					int i = t.Get(Calendar.MONTH);
					Locale lt = ((l == null) ? Locale.US : l);
					DateFormatSymbols dfs = DateFormatSymbols.GetInstance(lt);
					if (c == DateTime.NAME_OF_MONTH)
					{
						sb.Append(dfs.Months[i]);
					}
					else
					{
						sb.Append(dfs.ShortMonths[i]);
					}
					break;
				}
				case DateTime.CENTURY: // 'C' (00 - 99)
				case DateTime.YEAR_2: // 'y' (00 - 99)
				case DateTime.YEAR_4: // 'Y' (0000 - 9999)
				{
					int i = t.Get(Calendar.YEAR);
					int size = 2;
					switch (c)
					{
					case DateTime.CENTURY:
						i /= 100;
						break;
					case DateTime.YEAR_2:
						i %= 100;
						break;
					case DateTime.YEAR_4:
						size = 4;
						break;
					}
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, size, l));
					break;
				}
				case DateTime.DAY_OF_MONTH_0: // 'd' (01 - 31)
				case DateTime.DAY_OF_MONTH: // 'e' (1 - 31) -- like d
				{
					int i = t.Get(Calendar.DATE);
					Flags flags = (c == DateTime.DAY_OF_MONTH_0 ? Flags.ZERO_PAD : Flags.NONE);
					sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
					break;
				}
				case DateTime.DAY_OF_YEAR: // 'j' (001 - 366)
				{
					int i = t.Get(Calendar.DAY_OF_YEAR);
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 3, l));
					break;
				}
				case DateTime.MONTH: // 'm' (01 - 12)
				{
					int i = t.Get(Calendar.MONTH) + 1;
					Flags flags = Flags.ZERO_PAD;
					sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
					break;
				}

				// Composites
				case DateTime.TIME: // 'T' (24 hour hh:mm:ss - %tH:%tM:%tS)
				case DateTime.TIME_24_HOUR: // 'R' (hh:mm same as %H:%M)
				{
					char sep = ':';
					Print(sb, t, DateTime.HOUR_OF_DAY_0, l).Append(sep);
					Print(sb, t, DateTime.MINUTE, l);
					if (c == DateTime.TIME)
					{
						sb.Append(sep);
						Print(sb, t, DateTime.SECOND, l);
					}
					break;
				}
				case DateTime.TIME_12_HOUR: // 'r' (hh:mm:ss [AP]M)
				{
					char sep = ':';
					Print(sb, t, DateTime.HOUR_0, l).Append(sep);
					Print(sb, t, DateTime.MINUTE, l).Append(sep);
					Print(sb, t, DateTime.SECOND, l).Append(' ');
					// this may be in wrong place for some locales
					StringBuilder tsb = new StringBuilder();
					Print(tsb, t, DateTime.AM_PM, l);
					sb.Append(tsb.ToString().ToUpperCase(l != null ? l : Locale.US));
					break;
				}
				case DateTime.DATE_TIME: // 'c' (Sat Nov 04 12:02:33 EST 1999)
				{
					char sep = ' ';
					Print(sb, t, DateTime.NAME_OF_DAY_ABBREV, l).Append(sep);
					Print(sb, t, DateTime.NAME_OF_MONTH_ABBREV, l).Append(sep);
					Print(sb, t, DateTime.DAY_OF_MONTH_0, l).Append(sep);
					Print(sb, t, DateTime.TIME, l).Append(sep);
					Print(sb, t, DateTime.ZONE, l).Append(sep);
					Print(sb, t, DateTime.YEAR_4, l);
					break;
				}
				case DateTime.DATE: // 'D' (mm/dd/yy)
				{
					char sep = '/';
					Print(sb, t, DateTime.MONTH, l).Append(sep);
					Print(sb, t, DateTime.DAY_OF_MONTH_0, l).Append(sep);
					Print(sb, t, DateTime.YEAR_2, l);
					break;
				}
				case DateTime.ISO_STANDARD_DATE: // 'F' (%Y-%m-%d)
				{
					char sep = '-';
					Print(sb, t, DateTime.YEAR_4, l).Append(sep);
					Print(sb, t, DateTime.MONTH, l).Append(sep);
					Print(sb, t, DateTime.DAY_OF_MONTH_0, l);
					break;
				}
				default:
					Debug.Assert(false);
				break;
				}
				return sb;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void print(java.time.temporal.TemporalAccessor t, char c, Locale l) throws java.io.IOException
			internal virtual void Print(TemporalAccessor t, char c, Locale l)
			{
				StringBuilder sb = new StringBuilder();
				Print(sb, t, c, l);
				// justify based on width
				String s = Justify(sb.ToString());
				if (f.Contains(Flags.UPPERCASE))
				{
					s = s.ToUpperCase();
				}
				outerInstance.a.Append(s);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Appendable print(StringBuilder sb, java.time.temporal.TemporalAccessor t, char c, Locale l) throws java.io.IOException
			internal virtual Appendable Print(StringBuilder sb, TemporalAccessor t, char c, Locale l)
			{
				if (sb == null)
				{
					sb = new StringBuilder();
				}
				try
				{
					switch (c)
					{
					case DateTime.HOUR_OF_DAY_0: // 'H' (00 - 23)
					{
						int i = t.get(ChronoField.HOUR_OF_DAY);
						sb.Append(LocalizedMagnitude(null, i, Flags.ZERO_PAD, 2, l));
						break;
					}
					case DateTime.HOUR_OF_DAY: // 'k' (0 - 23) -- like H
					{
						int i = t.get(ChronoField.HOUR_OF_DAY);
						sb.Append(LocalizedMagnitude(null, i, Flags.NONE, 2, l));
						break;
					}
					case DateTime.HOUR_0: // 'I' (01 - 12)
					{
						int i = t.get(ChronoField.CLOCK_HOUR_OF_AMPM);
						sb.Append(LocalizedMagnitude(null, i, Flags.ZERO_PAD, 2, l));
						break;
					}
					case DateTime.HOUR: // 'l' (1 - 12) -- like I
					{
						int i = t.get(ChronoField.CLOCK_HOUR_OF_AMPM);
						sb.Append(LocalizedMagnitude(null, i, Flags.NONE, 2, l));
						break;
					}
					case DateTime.MINUTE: // 'M' (00 - 59)
					{
						int i = t.get(ChronoField.MINUTE_OF_HOUR);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
						break;
					}
					case DateTime.NANOSECOND: // 'N' (000000000 - 999999999)
					{
						int i = t.get(ChronoField.MILLI_OF_SECOND) * 1000000;
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 9, l));
						break;
					}
					case DateTime.MILLISECOND: // 'L' (000 - 999)
					{
						int i = t.get(ChronoField.MILLI_OF_SECOND);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 3, l));
						break;
					}
					case DateTime.MILLISECOND_SINCE_EPOCH: // 'Q' (0 - 99...?)
					{
						long i = t.GetLong(ChronoField.INSTANT_SECONDS) * 1000L + t.GetLong(ChronoField.MILLI_OF_SECOND);
						Flags flags = Flags.NONE;
						sb.Append(LocalizedMagnitude(null, i, flags, Width_Renamed, l));
						break;
					}
					case DateTime.AM_PM: // 'p' (am or pm)
					{
						// Calendar.AM = 0, Calendar.PM = 1, LocaleElements defines upper
						String[] ampm = new String[] {"AM", "PM"};
						if (l != null && l != Locale.US)
						{
							DateFormatSymbols dfs = DateFormatSymbols.GetInstance(l);
							ampm = dfs.AmPmStrings;
						}
						String s = ampm[t.get(ChronoField.AMPM_OF_DAY)];
						sb.Append(s.ToLowerCase(l != null ? l : Locale.US));
						break;
					}
					case DateTime.SECONDS_SINCE_EPOCH: // 's' (0 - 99...?)
					{
						long i = t.GetLong(ChronoField.INSTANT_SECONDS);
						Flags flags = Flags.NONE;
						sb.Append(LocalizedMagnitude(null, i, flags, Width_Renamed, l));
						break;
					}
					case DateTime.SECOND: // 'S' (00 - 60 - leap second)
					{
						int i = t.get(ChronoField.SECOND_OF_MINUTE);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
						break;
					}
					case DateTime.ZONE_NUMERIC: // 'z' ({-|+}####) - ls minus?
					{
						int i = t.get(ChronoField.OFFSET_SECONDS);
						bool neg = i < 0;
						sb.Append(neg ? '-' : '+');
						if (neg)
						{
							i = -i;
						}
						int min = i / 60;
						// combine minute and hour into a single integer
						int offset = (min / 60) * 100 + (min % 60);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, offset, flags, 4, l));
						break;
					}
					case DateTime.ZONE: // 'Z' (symbol)
					{
						ZoneId zid = t.query(TemporalQueries.Zone());
						if (zid == null)
						{
							throw new IllegalFormatConversionException(c, t.GetType());
						}
						if (!(zid is ZoneOffset) && t.IsSupported(ChronoField.INSTANT_SECONDS))
						{
							Instant instant = Instant.From(t);
							sb.Append(TimeZone.GetTimeZone(zid.Id).GetDisplayName(zid.Rules.IsDaylightSavings(instant), TimeZone.SHORT, (l == null) ? Locale.US : l));
							break;
						}
						sb.Append(zid.Id);
						break;
					}
					// Date
					case DateTime.NAME_OF_DAY_ABBREV: // 'a'
					case DateTime.NAME_OF_DAY: // 'A'
					{
						int i = t.get(ChronoField.DAY_OF_WEEK) % 7 + 1;
						Locale lt = ((l == null) ? Locale.US : l);
						DateFormatSymbols dfs = DateFormatSymbols.GetInstance(lt);
						if (c == DateTime.NAME_OF_DAY)
						{
							sb.Append(dfs.Weekdays[i]);
						}
						else
						{
							sb.Append(dfs.ShortWeekdays[i]);
						}
						break;
					}
					case DateTime.NAME_OF_MONTH_ABBREV: // 'b'
					case DateTime.NAME_OF_MONTH_ABBREV_X: // 'h' -- same b
					case DateTime.NAME_OF_MONTH: // 'B'
					{
						int i = t.get(ChronoField.MONTH_OF_YEAR) - 1;
						Locale lt = ((l == null) ? Locale.US : l);
						DateFormatSymbols dfs = DateFormatSymbols.GetInstance(lt);
						if (c == DateTime.NAME_OF_MONTH)
						{
							sb.Append(dfs.Months[i]);
						}
						else
						{
							sb.Append(dfs.ShortMonths[i]);
						}
						break;
					}
					case DateTime.CENTURY: // 'C' (00 - 99)
					case DateTime.YEAR_2: // 'y' (00 - 99)
					case DateTime.YEAR_4: // 'Y' (0000 - 9999)
					{
						int i = t.get(ChronoField.YEAR_OF_ERA);
						int size = 2;
						switch (c)
						{
						case DateTime.CENTURY:
							i /= 100;
							break;
						case DateTime.YEAR_2:
							i %= 100;
							break;
						case DateTime.YEAR_4:
							size = 4;
							break;
						}
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, size, l));
						break;
					}
					case DateTime.DAY_OF_MONTH_0: // 'd' (01 - 31)
					case DateTime.DAY_OF_MONTH: // 'e' (1 - 31) -- like d
					{
						int i = t.get(ChronoField.DAY_OF_MONTH);
						Flags flags = (c == DateTime.DAY_OF_MONTH_0 ? Flags.ZERO_PAD : Flags.NONE);
						sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
						break;
					}
					case DateTime.DAY_OF_YEAR: // 'j' (001 - 366)
					{
						int i = t.get(ChronoField.DAY_OF_YEAR);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 3, l));
						break;
					}
					case DateTime.MONTH: // 'm' (01 - 12)
					{
						int i = t.get(ChronoField.MONTH_OF_YEAR);
						Flags flags = Flags.ZERO_PAD;
						sb.Append(LocalizedMagnitude(null, i, flags, 2, l));
						break;
					}

					// Composites
					case DateTime.TIME: // 'T' (24 hour hh:mm:ss - %tH:%tM:%tS)
					case DateTime.TIME_24_HOUR: // 'R' (hh:mm same as %H:%M)
					{
						char sep = ':';
						Print(sb, t, DateTime.HOUR_OF_DAY_0, l).Append(sep);
						Print(sb, t, DateTime.MINUTE, l);
						if (c == DateTime.TIME)
						{
							sb.Append(sep);
							Print(sb, t, DateTime.SECOND, l);
						}
						break;
					}
					case DateTime.TIME_12_HOUR: // 'r' (hh:mm:ss [AP]M)
					{
						char sep = ':';
						Print(sb, t, DateTime.HOUR_0, l).Append(sep);
						Print(sb, t, DateTime.MINUTE, l).Append(sep);
						Print(sb, t, DateTime.SECOND, l).Append(' ');
						// this may be in wrong place for some locales
						StringBuilder tsb = new StringBuilder();
						Print(tsb, t, DateTime.AM_PM, l);
						sb.Append(tsb.ToString().ToUpperCase(l != null ? l : Locale.US));
						break;
					}
					case DateTime.DATE_TIME: // 'c' (Sat Nov 04 12:02:33 EST 1999)
					{
						char sep = ' ';
						Print(sb, t, DateTime.NAME_OF_DAY_ABBREV, l).Append(sep);
						Print(sb, t, DateTime.NAME_OF_MONTH_ABBREV, l).Append(sep);
						Print(sb, t, DateTime.DAY_OF_MONTH_0, l).Append(sep);
						Print(sb, t, DateTime.TIME, l).Append(sep);
						Print(sb, t, DateTime.ZONE, l).Append(sep);
						Print(sb, t, DateTime.YEAR_4, l);
						break;
					}
					case DateTime.DATE: // 'D' (mm/dd/yy)
					{
						char sep = '/';
						Print(sb, t, DateTime.MONTH, l).Append(sep);
						Print(sb, t, DateTime.DAY_OF_MONTH_0, l).Append(sep);
						Print(sb, t, DateTime.YEAR_2, l);
						break;
					}
					case DateTime.ISO_STANDARD_DATE: // 'F' (%Y-%m-%d)
					{
						char sep = '-';
						Print(sb, t, DateTime.YEAR_4, l).Append(sep);
						Print(sb, t, DateTime.MONTH, l).Append(sep);
						Print(sb, t, DateTime.DAY_OF_MONTH_0, l);
						break;
					}
					default:
						Debug.Assert(false);
					break;
					}
				}
				catch (DateTimeException)
				{
					throw new IllegalFormatConversionException(c, t.GetType());
				}
				return sb;
			}

			// -- Methods to support throwing exceptions --

			internal virtual void FailMismatch(Flags f, char c)
			{
				String fs = f.ToString();
				throw new FormatFlagsConversionMismatchException(fs, c);
			}

			internal virtual void FailConversion(char c, Object arg)
			{
				throw new IllegalFormatConversionException(c, arg.GetType());
			}

			internal virtual char GetZero(Locale l)
			{
				if ((l != null) && !l.Equals(outerInstance.Locale()))
				{
					DecimalFormatSymbols dfs = DecimalFormatSymbols.GetInstance(l);
					return dfs.ZeroDigit;
				}
				return outerInstance.Zero;
			}

			internal virtual StringBuilder LocalizedMagnitude(StringBuilder sb, long value, Flags f, int width, Locale l)
			{
				char[] va = Convert.ToString(value, 10).ToCharArray();
				return LocalizedMagnitude(sb, va, f, width, l);
			}

			internal virtual StringBuilder LocalizedMagnitude(StringBuilder sb, char[] value, Flags f, int width, Locale l)
			{
				if (sb == null)
				{
					sb = new StringBuilder();
				}
				int begin = sb.Length();

				char zero = GetZero(l);

				// determine localized grouping separator and size
				char grpSep = '\0';
				int grpSize = -1;
				char decSep = '\0';

				int len = value.Length;
				int dot = len;
				for (int j = 0; j < len; j++)
				{
					if (value[j] == '.')
					{
						dot = j;
						break;
					}
				}

				if (dot < len)
				{
					if (l == null || l.Equals(Locale.US))
					{
						decSep = '.';
					}
					else
					{
						DecimalFormatSymbols dfs = DecimalFormatSymbols.GetInstance(l);
						decSep = dfs.DecimalSeparator;
					}
				}

				if (f.Contains(Flags.GROUP))
				{
					if (l == null || l.Equals(Locale.US))
					{
						grpSep = ',';
						grpSize = 3;
					}
					else
					{
						DecimalFormatSymbols dfs = DecimalFormatSymbols.GetInstance(l);
						grpSep = dfs.GroupingSeparator;
						DecimalFormat df = (DecimalFormat) NumberFormat.GetIntegerInstance(l);
						grpSize = df.GroupingSize;
					}
				}

				// localize the digits inserting group separators as necessary
				for (int j = 0; j < len; j++)
				{
					if (j == dot)
					{
						sb.Append(decSep);
						// no more group separators after the decimal separator
						grpSep = '\0';
						continue;
					}

					char c = value[j];
					sb.Append((char)((c - '0') + zero));
					if (grpSep != '\0' && j != dot - 1 && ((dot - j) % grpSize == 1))
					{
						sb.Append(grpSep);
					}
				}

				// apply zero padding
				len = sb.Length();
				if (width != -1 && f.Contains(Flags.ZERO_PAD))
				{
					for (int k = 0; k < width - len; k++)
					{
						sb.Insert(begin, zero);
					}
				}

				return sb;
			}
		}

		private class Flags
		{
			internal int Flags_Renamed;

			internal static readonly Flags NONE = new Flags(0); // ''

			// duplicate declarations from Formattable.java
			internal static readonly Flags LEFT_JUSTIFY = new Flags(1 << 0); // '-'
			internal static readonly Flags UPPERCASE = new Flags(1 << 1); // '^'
			internal static readonly Flags ALTERNATE = new Flags(1 << 2); // '#'

			// numerics
			internal static readonly Flags PLUS = new Flags(1 << 3); // '+'
			internal static readonly Flags LEADING_SPACE = new Flags(1 << 4); // ' '
			internal static readonly Flags ZERO_PAD = new Flags(1 << 5); // '0'
			internal static readonly Flags GROUP = new Flags(1 << 6); // ','
			internal static readonly Flags PARENTHESES = new Flags(1 << 7); // '('

			// indexing
			internal static readonly Flags PREVIOUS = new Flags(1 << 8); // '<'

			internal Flags(int f)
			{
				Flags_Renamed = f;
			}

			public virtual int ValueOf()
			{
				return Flags_Renamed;
			}

			public virtual bool Contains(Flags f)
			{
				return (Flags_Renamed & f.ValueOf()) == f.ValueOf();
			}

			public virtual Flags Dup()
			{
				return new Flags(Flags_Renamed);
			}

			internal virtual Flags Add(Flags f)
			{
				Flags_Renamed |= f.ValueOf();
				return this;
			}

			public virtual Flags Remove(Flags f)
			{
				Flags_Renamed &= ~f.ValueOf();
				return this;
			}

			public static Flags Parse(String s)
			{
				char[] ca = s.ToCharArray();
				Flags f = new Flags(0);
				for (int i = 0; i < ca.Length; i++)
				{
					Flags v = Parse(ca[i]);
					if (f.Contains(v))
					{
						throw new DuplicateFormatFlagsException(v.ToString());
					}
					f.Add(v);
				}
				return f;
			}

			// parse those flags which may be provided by users
			internal static Flags Parse(char c)
			{
				switch (c)
				{
				case '-':
					return LEFT_JUSTIFY;
				case '#':
					return ALTERNATE;
				case '+':
					return PLUS;
				case ' ':
					return LEADING_SPACE;
				case '0':
					return ZERO_PAD;
				case ',':
					return GROUP;
				case '(':
					return PARENTHESES;
				case '<':
					return PREVIOUS;
				default:
					throw new UnknownFormatFlagsException(Convert.ToString(c));
				}
			}

			// Returns a string representation of the current {@code Flags}.
			public static String ToString(Flags f)
			{
				return f.ToString();
			}

			public override String ToString()
			{
				StringBuilder sb = new StringBuilder();
				if (Contains(LEFT_JUSTIFY))
				{
					sb.Append('-');
				}
				if (Contains(UPPERCASE))
				{
					sb.Append('^');
				}
				if (Contains(ALTERNATE))
				{
					sb.Append('#');
				}
				if (Contains(PLUS))
				{
					sb.Append('+');
				}
				if (Contains(LEADING_SPACE))
				{
					sb.Append(' ');
				}
				if (Contains(ZERO_PAD))
				{
					sb.Append('0');
				}
				if (Contains(GROUP))
				{
					sb.Append(',');
				}
				if (Contains(PARENTHESES))
				{
					sb.Append('(');
				}
				if (Contains(PREVIOUS))
				{
					sb.Append('<');
				}
				return sb.ToString();
			}
		}

		private class Conversion
		{
			// Byte, Short, Integer, Long, BigInteger
			// (and associated primitives due to autoboxing)
			internal const char DECIMAL_INTEGER = 'd';
			internal const char OCTAL_INTEGER = 'o';
			internal const char HEXADECIMAL_INTEGER = 'x';
			internal const char HEXADECIMAL_INTEGER_UPPER = 'X';

			// Float, Double, BigDecimal
			// (and associated primitives due to autoboxing)
			internal const char SCIENTIFIC = 'e';
			internal const char SCIENTIFIC_UPPER = 'E';
			internal const char GENERAL = 'g';
			internal const char GENERAL_UPPER = 'G';
			internal const char DECIMAL_FLOAT = 'f';
			internal const char HEXADECIMAL_FLOAT = 'a';
			internal const char HEXADECIMAL_FLOAT_UPPER = 'A';

			// Character, Byte, Short, Integer
			// (and associated primitives due to autoboxing)
			internal const char CHARACTER = 'c';
			internal const char CHARACTER_UPPER = 'C';

			// java.util.Date, java.util.Calendar, long
			internal const char DATE_TIME = 't';
			internal const char DATE_TIME_UPPER = 'T';

			// if (arg.TYPE != boolean) return boolean
			// if (arg != null) return true; else return false;
			internal const char BOOLEAN = 'b';
			internal const char BOOLEAN_UPPER = 'B';
			// if (arg instanceof Formattable) arg.formatTo()
			// else arg.toString();
			internal const char STRING = 's';
			internal const char STRING_UPPER = 'S';
			// arg.hashCode()
			internal const char HASHCODE = 'h';
			internal const char HASHCODE_UPPER = 'H';

			internal const char LINE_SEPARATOR = 'n';
			internal const char PERCENT_SIGN = '%';

			internal static bool IsValid(char c)
			{
				return (IsGeneral(c) || IsInteger(c) || IsFloat(c) || IsText(c) || c == 't' || IsCharacter(c));
			}

			// Returns true iff the Conversion is applicable to all objects.
			internal static bool IsGeneral(char c)
			{
				switch (c)
				{
				case BOOLEAN:
				case BOOLEAN_UPPER:
				case STRING:
				case STRING_UPPER:
				case HASHCODE:
				case HASHCODE_UPPER:
					return true;
				default:
					return false;
				}
			}

			// Returns true iff the Conversion is applicable to character.
			internal static bool IsCharacter(char c)
			{
				switch (c)
				{
				case CHARACTER:
				case CHARACTER_UPPER:
					return true;
				default:
					return false;
				}
			}

			// Returns true iff the Conversion is an integer type.
			internal static bool IsInteger(char c)
			{
				switch (c)
				{
				case DECIMAL_INTEGER:
				case OCTAL_INTEGER:
				case HEXADECIMAL_INTEGER:
				case HEXADECIMAL_INTEGER_UPPER:
					return true;
				default:
					return false;
				}
			}

			// Returns true iff the Conversion is a floating-point type.
			internal static bool IsFloat(char c)
			{
				switch (c)
				{
				case SCIENTIFIC:
				case SCIENTIFIC_UPPER:
				case GENERAL:
				case GENERAL_UPPER:
				case DECIMAL_FLOAT:
				case HEXADECIMAL_FLOAT:
				case HEXADECIMAL_FLOAT_UPPER:
					return true;
				default:
					return false;
				}
			}

			// Returns true iff the Conversion does not require an argument
			internal static bool IsText(char c)
			{
				switch (c)
				{
				case LINE_SEPARATOR:
				case PERCENT_SIGN:
					return true;
				default:
					return false;
				}
			}
		}

		private class DateTime
		{
			internal const char HOUR_OF_DAY_0 = 'H'; // (00 - 23)
			internal const char HOUR_0 = 'I'; // (01 - 12)
			internal const char HOUR_OF_DAY = 'k'; // (0 - 23) -- like H
			internal const char HOUR = 'l'; // (1 - 12) -- like I
			internal const char MINUTE = 'M'; // (00 - 59)
			internal const char NANOSECOND = 'N'; // (000000000 - 999999999)
			internal const char MILLISECOND = 'L'; // jdk, not in gnu (000 - 999)
			internal const char MILLISECOND_SINCE_EPOCH = 'Q'; // (0 - 99...?)
			internal const char AM_PM = 'p'; // (am or pm)
			internal const char SECONDS_SINCE_EPOCH = 's'; // (0 - 99...?)
			internal const char SECOND = 'S'; // (00 - 60 - leap second)
			internal const char TIME = 'T'; // (24 hour hh:mm:ss)
			internal const char ZONE_NUMERIC = 'z'; // (-1200 - +1200) - ls minus?
			internal const char ZONE = 'Z'; // (symbol)

			// Date
			internal const char NAME_OF_DAY_ABBREV = 'a'; // 'a'
			internal const char NAME_OF_DAY = 'A'; // 'A'
			internal const char NAME_OF_MONTH_ABBREV = 'b'; // 'b'
			internal const char NAME_OF_MONTH = 'B'; // 'B'
			internal const char CENTURY = 'C'; // (00 - 99)
			internal const char DAY_OF_MONTH_0 = 'd'; // (01 - 31)
			internal const char DAY_OF_MONTH = 'e'; // (1 - 31) -- like d
	// *    static final char ISO_WEEK_OF_YEAR_2    = 'g'; // cross %y %V
	// *    static final char ISO_WEEK_OF_YEAR_4    = 'G'; // cross %Y %V
			internal const char NAME_OF_MONTH_ABBREV_X = 'h'; // -- same b
			internal const char DAY_OF_YEAR = 'j'; // (001 - 366)
			internal const char MONTH = 'm'; // (01 - 12)
	// *    static final char DAY_OF_WEEK_1         = 'u'; // (1 - 7) Monday
	// *    static final char WEEK_OF_YEAR_SUNDAY   = 'U'; // (0 - 53) Sunday+
	// *    static final char WEEK_OF_YEAR_MONDAY_01 = 'V'; // (01 - 53) Monday+
	// *    static final char DAY_OF_WEEK_0         = 'w'; // (0 - 6) Sunday
	// *    static final char WEEK_OF_YEAR_MONDAY   = 'W'; // (00 - 53) Monday
			internal const char YEAR_2 = 'y'; // (00 - 99)
			internal const char YEAR_4 = 'Y'; // (0000 - 9999)

			// Composites
			internal const char TIME_12_HOUR = 'r'; // (hh:mm:ss [AP]M)
			internal const char TIME_24_HOUR = 'R'; // (hh:mm same as %H:%M)
	// *    static final char LOCALE_TIME   = 'X'; // (%H:%M:%S) - parse format?
			internal const char DATE_TIME = 'c';
												// (Sat Nov 04 12:02:33 EST 1999)
			internal const char DATE = 'D'; // (mm/dd/yy)
			internal const char ISO_STANDARD_DATE = 'F'; // (%Y-%m-%d)
	// *    static final char LOCALE_DATE           = 'x'; // (mm/dd/yy)

			internal static bool IsValid(char c)
			{
				switch (c)
				{
				case HOUR_OF_DAY_0:
				case HOUR_0:
				case HOUR_OF_DAY:
				case HOUR:
				case MINUTE:
				case NANOSECOND:
				case MILLISECOND:
				case MILLISECOND_SINCE_EPOCH:
				case AM_PM:
				case SECONDS_SINCE_EPOCH:
				case SECOND:
				case TIME:
				case ZONE_NUMERIC:
				case ZONE:

				// Date
				case NAME_OF_DAY_ABBREV:
				case NAME_OF_DAY:
				case NAME_OF_MONTH_ABBREV:
				case NAME_OF_MONTH:
				case CENTURY:
				case DAY_OF_MONTH_0:
				case DAY_OF_MONTH:
	// *        case ISO_WEEK_OF_YEAR_2:
	// *        case ISO_WEEK_OF_YEAR_4:
				case NAME_OF_MONTH_ABBREV_X:
				case DAY_OF_YEAR:
				case MONTH:
	// *        case DAY_OF_WEEK_1:
	// *        case WEEK_OF_YEAR_SUNDAY:
	// *        case WEEK_OF_YEAR_MONDAY_01:
	// *        case DAY_OF_WEEK_0:
	// *        case WEEK_OF_YEAR_MONDAY:
				case YEAR_2:
				case YEAR_4:

				// Composites
				case TIME_12_HOUR:
				case TIME_24_HOUR:
	// *        case LOCALE_TIME:
				case DATE_TIME:
				case DATE:
				case ISO_STANDARD_DATE:
	// *        case LOCALE_DATE:
					return true;
				default:
					return false;
				}
			}
		}
	}

}