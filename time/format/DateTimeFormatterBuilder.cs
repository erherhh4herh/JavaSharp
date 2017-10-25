using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
 * All rights hg qreserved.
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



	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleResources = sun.util.locale.provider.LocaleResources;
	using TimeZoneNameUtility = sun.util.locale.provider.TimeZoneNameUtility;

	/// <summary>
	/// Builder to create date-time formatters.
	/// <para>
	/// This allows a {@code DateTimeFormatter} to be created.
	/// All date-time formatters are created ultimately using this builder.
	/// </para>
	/// <para>
	/// The basic elements of date-time can all be added:
	/// <ul>
	/// <li>Value - a numeric value</li>
	/// <li>Fraction - a fractional value including the decimal place. Always use this when
	/// outputting fractions to ensure that the fraction is parsed correctly</li>
	/// <li>Text - the textual equivalent for the value</li>
	/// <li>OffsetId/Offset - the <seealso cref="ZoneOffset zone offset"/></li>
	/// <li>ZoneId - the <seealso cref="ZoneId time-zone"/> id</li>
	/// <li>ZoneText - the name of the time-zone</li>
	/// <li>ChronologyId - the <seealso cref="Chronology chronology"/> id</li>
	/// <li>ChronologyText - the name of the chronology</li>
	/// <li>Literal - a text literal</li>
	/// <li>Nested and Optional - formats can be nested or made optional</li>
	/// </ul>
	/// In addition, any of the elements may be decorated by padding, either with spaces or any other character.
	/// </para>
	/// <para>
	/// Finally, a shorthand pattern, mostly compatible with {@code java.text.SimpleDateFormat SimpleDateFormat}
	/// can be used, see <seealso cref="#appendPattern(String)"/>.
	/// In practice, this simply parses the pattern and calls other methods on the builder.
	/// 
	/// @implSpec
	/// This class is a mutable builder intended for use from a single thread.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class DateTimeFormatterBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Active = this;
		}


		/// <summary>
		/// Query for a time-zone that is region-only.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		private static readonly TemporalQuery<ZoneId> QUERY_REGION_ONLY = (temporal) =>
		{
			ZoneId zone = temporal.query(TemporalQueries.ZoneId());
			return (zone != null && zone is ZoneOffset == false ? zone : null);
		};

		/// <summary>
		/// The currently active builder, used by the outermost builder.
		/// </summary>
		private DateTimeFormatterBuilder Active;
		/// <summary>
		/// The parent builder, null for the outermost builder.
		/// </summary>
		private readonly DateTimeFormatterBuilder Parent;
		/// <summary>
		/// The list of printers that will be used.
		/// </summary>
		private readonly IList<DateTimePrinterParser> PrinterParsers = new List<DateTimePrinterParser>();
		/// <summary>
		/// Whether this builder produces an optional formatter.
		/// </summary>
		private readonly bool Optional;
		/// <summary>
		/// The width to pad the next field to.
		/// </summary>
		private int PadNextWidth;
		/// <summary>
		/// The character to pad the next field with.
		/// </summary>
		private char PadNextChar;
		/// <summary>
		/// The index of the last variable width value parser.
		/// </summary>
		private int ValueParserIndex = -1;

		/// <summary>
		/// Gets the formatting pattern for date and time styles for a locale and chronology.
		/// The locale and chronology are used to lookup the locale specific format
		/// for the requested dateStyle and/or timeStyle.
		/// </summary>
		/// <param name="dateStyle">  the FormatStyle for the date, null for time-only pattern </param>
		/// <param name="timeStyle">  the FormatStyle for the time, null for date-only pattern </param>
		/// <param name="chrono">  the Chronology, non-null </param>
		/// <param name="locale">  the locale, non-null </param>
		/// <returns> the locale and Chronology specific formatting pattern </returns>
		/// <exception cref="IllegalArgumentException"> if both dateStyle and timeStyle are null </exception>
		public static String GetLocalizedDateTimePattern(FormatStyle dateStyle, FormatStyle timeStyle, Chronology chrono, Locale locale)
		{
			Objects.RequireNonNull(locale, "locale");
			Objects.RequireNonNull(chrono, "chrono");
			if (dateStyle == null && timeStyle == null)
			{
				throw new IllegalArgumentException("Either dateStyle or timeStyle must be non-null");
			}
			LocaleResources lr = LocaleProviderAdapter.ResourceBundleBased.getLocaleResources(locale);
			String pattern = lr.getJavaTimeDateTimePattern(ConvertStyle(timeStyle), ConvertStyle(dateStyle), chrono.CalendarType);
			return pattern;
		}

		/// <summary>
		/// Converts the given FormatStyle to the java.text.DateFormat style.
		/// </summary>
		/// <param name="style">  the FormatStyle style </param>
		/// <returns> the int style, or -1 if style is null, indicating un-required </returns>
		private static int ConvertStyle(FormatStyle style)
		{
			if (style == null)
			{
				return -1;
			}
			return style.ordinal(); // indices happen to align
		}

		/// <summary>
		/// Constructs a new instance of the builder.
		/// </summary>
		public DateTimeFormatterBuilder() : base()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			Parent = null;
			Optional = false;
		}

		/// <summary>
		/// Constructs a new instance of the builder.
		/// </summary>
		/// <param name="parent">  the parent builder, not null </param>
		/// <param name="optional">  whether the formatter is optional, not null </param>
		private DateTimeFormatterBuilder(DateTimeFormatterBuilder parent, bool optional) : base()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Parent = parent;
			this.Optional = optional;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Changes the parse style to be case sensitive for the remainder of the formatter.
		/// <para>
		/// Parsing can be case sensitive or insensitive - by default it is case sensitive.
		/// This method allows the case sensitivity setting of parsing to be changed.
		/// </para>
		/// <para>
		/// Calling this method changes the state of the builder such that all
		/// subsequent builder method calls will parse text in case sensitive mode.
		/// See <seealso cref="#parseCaseInsensitive"/> for the opposite setting.
		/// The parse case sensitive/insensitive methods may be called at any point
		/// in the builder, thus the parser can swap between case parsing modes
		/// multiple times during the parse.
		/// </para>
		/// <para>
		/// Since the default is case sensitive, this method should only be used after
		/// a previous call to {@code #parseCaseInsensitive}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder ParseCaseSensitive()
		{
			AppendInternal(SettingsParser.SENSITIVE);
			return this;
		}

		/// <summary>
		/// Changes the parse style to be case insensitive for the remainder of the formatter.
		/// <para>
		/// Parsing can be case sensitive or insensitive - by default it is case sensitive.
		/// This method allows the case sensitivity setting of parsing to be changed.
		/// </para>
		/// <para>
		/// Calling this method changes the state of the builder such that all
		/// subsequent builder method calls will parse text in case insensitive mode.
		/// See <seealso cref="#parseCaseSensitive()"/> for the opposite setting.
		/// The parse case sensitive/insensitive methods may be called at any point
		/// in the builder, thus the parser can swap between case parsing modes
		/// multiple times during the parse.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder ParseCaseInsensitive()
		{
			AppendInternal(SettingsParser.INSENSITIVE);
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Changes the parse style to be strict for the remainder of the formatter.
		/// <para>
		/// Parsing can be strict or lenient - by default its strict.
		/// This controls the degree of flexibility in matching the text and sign styles.
		/// </para>
		/// <para>
		/// When used, this method changes the parsing to be strict from this point onwards.
		/// As strict is the default, this is normally only needed after calling <seealso cref="#parseLenient()"/>.
		/// The change will remain in force until the end of the formatter that is eventually
		/// constructed or until {@code parseLenient} is called.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder ParseStrict()
		{
			AppendInternal(SettingsParser.STRICT);
			return this;
		}

		/// <summary>
		/// Changes the parse style to be lenient for the remainder of the formatter.
		/// Note that case sensitivity is set separately to this method.
		/// <para>
		/// Parsing can be strict or lenient - by default its strict.
		/// This controls the degree of flexibility in matching the text and sign styles.
		/// Applications calling this method should typically also call <seealso cref="#parseCaseInsensitive()"/>.
		/// </para>
		/// <para>
		/// When used, this method changes the parsing to be lenient from this point onwards.
		/// The change will remain in force until the end of the formatter that is eventually
		/// constructed or until {@code parseStrict} is called.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder ParseLenient()
		{
			AppendInternal(SettingsParser.LENIENT);
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends a default value for a field to the formatter for use in parsing.
		/// <para>
		/// This appends an instruction to the builder to inject a default value
		/// into the parsed result. This is especially useful in conjunction with
		/// optional parts of the formatter.
		/// </para>
		/// <para>
		/// For example, consider a formatter that parses the year, followed by
		/// an optional month, with a further optional day-of-month. Using such a
		/// formatter would require the calling code to check whether a full date,
		/// year-month or just a year had been parsed. This method can be used to
		/// default the month and day-of-month to a sensible value, such as the
		/// first of the month, allowing the calling code to always get a date.
		/// </para>
		/// <para>
		/// During formatting, this method has no effect.
		/// </para>
		/// <para>
		/// During parsing, the current state of the parse is inspected.
		/// If the specified field has no associated value, because it has not been
		/// parsed successfully at that point, then the specified value is injected
		/// into the parse result. Injection is immediate, thus the field-value pair
		/// will be visible to any subsequent elements in the formatter.
		/// As such, this method is normally called at the end of the builder.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to default the value of, not null </param>
		/// <param name="value">  the value to default the field to </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder ParseDefaulting(TemporalField field, long value)
		{
			Objects.RequireNonNull(field, "field");
			AppendInternal(new DefaultValueParser(field, value));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the value of a date-time field to the formatter using a normal
		/// output style.
		/// <para>
		/// The value of the field will be output during a format.
		/// If the value cannot be obtained then an exception will be thrown.
		/// </para>
		/// <para>
		/// The value will be printed as per the normal format of an integer value.
		/// Only negative numbers will be signed. No padding will be added.
		/// </para>
		/// <para>
		/// The parser for a variable width value such as this normally behaves greedily,
		/// requiring one digit, but accepting as many digits as possible.
		/// This behavior can be affected by 'adjacent value parsing'.
		/// See <seealso cref="#appendValue(java.time.temporal.TemporalField, int)"/> for full details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendValue(TemporalField field)
		{
			Objects.RequireNonNull(field, "field");
			AppendValue(new NumberPrinterParser(field, 1, 19, SignStyle.NORMAL));
			return this;
		}

		/// <summary>
		/// Appends the value of a date-time field to the formatter using a fixed
		/// width, zero-padded approach.
		/// <para>
		/// The value of the field will be output during a format.
		/// If the value cannot be obtained then an exception will be thrown.
		/// </para>
		/// <para>
		/// The value will be zero-padded on the left. If the size of the value
		/// means that it cannot be printed within the width then an exception is thrown.
		/// If the value of the field is negative then an exception is thrown during formatting.
		/// </para>
		/// <para>
		/// This method supports a special technique of parsing known as 'adjacent value parsing'.
		/// This technique solves the problem where a value, variable or fixed width, is followed by one or more
		/// fixed length values. The standard parser is greedy, and thus it would normally
		/// steal the digits that are needed by the fixed width value parsers that follow the
		/// variable width one.
		/// </para>
		/// <para>
		/// No action is required to initiate 'adjacent value parsing'.
		/// When a call to {@code appendValue} is made, the builder
		/// enters adjacent value parsing setup mode. If the immediately subsequent method
		/// call or calls on the same builder are for a fixed width value, then the parser will reserve
		/// space so that the fixed width values can be parsed.
		/// </para>
		/// <para>
		/// For example, consider {@code builder.appendValue(YEAR).appendValue(MONTH_OF_YEAR, 2);}
		/// The year is a variable width parse of between 1 and 19 digits.
		/// The month is a fixed width parse of 2 digits.
		/// Because these were appended to the same builder immediately after one another,
		/// the year parser will reserve two digits for the month to parse.
		/// Thus, the text '201106' will correctly parse to a year of 2011 and a month of 6.
		/// Without adjacent value parsing, the year would greedily parse all six digits and leave
		/// nothing for the month.
		/// </para>
		/// <para>
		/// Adjacent value parsing applies to each set of fixed width not-negative values in the parser
		/// that immediately follow any kind of value, variable or fixed width.
		/// Calling any other append method will end the setup of adjacent value parsing.
		/// Thus, in the unlikely event that you need to avoid adjacent value parsing behavior,
		/// simply add the {@code appendValue} to another {@code DateTimeFormatterBuilder}
		/// and add that to this builder.
		/// </para>
		/// <para>
		/// If adjacent parsing is active, then parsing must match exactly the specified
		/// number of digits in both strict and lenient modes.
		/// In addition, no positive or negative sign is permitted.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="width">  the width of the printed field, from 1 to 19 </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the width is invalid </exception>
		public DateTimeFormatterBuilder AppendValue(TemporalField field, int width)
		{
			Objects.RequireNonNull(field, "field");
			if (width < 1 || width > 19)
			{
				throw new IllegalArgumentException("The width must be from 1 to 19 inclusive but was " + width);
			}
			NumberPrinterParser pp = new NumberPrinterParser(field, width, width, SignStyle.NOT_NEGATIVE);
			AppendValue(pp);
			return this;
		}

		/// <summary>
		/// Appends the value of a date-time field to the formatter providing full
		/// control over formatting.
		/// <para>
		/// The value of the field will be output during a format.
		/// If the value cannot be obtained then an exception will be thrown.
		/// </para>
		/// <para>
		/// This method provides full control of the numeric formatting, including
		/// zero-padding and the positive/negative sign.
		/// </para>
		/// <para>
		/// The parser for a variable width value such as this normally behaves greedily,
		/// accepting as many digits as possible.
		/// This behavior can be affected by 'adjacent value parsing'.
		/// See <seealso cref="#appendValue(java.time.temporal.TemporalField, int)"/> for full details.
		/// </para>
		/// <para>
		/// In strict parsing mode, the minimum number of parsed digits is {@code minWidth}
		/// and the maximum is {@code maxWidth}.
		/// In lenient parsing mode, the minimum number of parsed digits is one
		/// and the maximum is 19 (except as limited by adjacent value parsing).
		/// </para>
		/// <para>
		/// If this method is invoked with equal minimum and maximum widths and a sign style of
		/// {@code NOT_NEGATIVE} then it delegates to {@code appendValue(TemporalField,int)}.
		/// In this scenario, the formatting and parsing behavior described there occur.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="minWidth">  the minimum field width of the printed field, from 1 to 19 </param>
		/// <param name="maxWidth">  the maximum field width of the printed field, from 1 to 19 </param>
		/// <param name="signStyle">  the positive/negative output style, not null </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the widths are invalid </exception>
		public DateTimeFormatterBuilder AppendValue(TemporalField field, int minWidth, int maxWidth, SignStyle signStyle)
		{
			if (minWidth == maxWidth && signStyle == SignStyle.NOT_NEGATIVE)
			{
				return AppendValue(field, maxWidth);
			}
			Objects.RequireNonNull(field, "field");
			Objects.RequireNonNull(signStyle, "signStyle");
			if (minWidth < 1 || minWidth > 19)
			{
				throw new IllegalArgumentException("The minimum width must be from 1 to 19 inclusive but was " + minWidth);
			}
			if (maxWidth < 1 || maxWidth > 19)
			{
				throw new IllegalArgumentException("The maximum width must be from 1 to 19 inclusive but was " + maxWidth);
			}
			if (maxWidth < minWidth)
			{
				throw new IllegalArgumentException("The maximum width must exceed or equal the minimum width but " + maxWidth + " < " + minWidth);
			}
			NumberPrinterParser pp = new NumberPrinterParser(field, minWidth, maxWidth, signStyle);
			AppendValue(pp);
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the reduced value of a date-time field to the formatter.
		/// <para>
		/// Since fields such as year vary by chronology, it is recommended to use the
		/// <seealso cref="#appendValueReduced(TemporalField, int, int, ChronoLocalDate)"/> date}
		/// variant of this method in most cases. This variant is suitable for
		/// simple fields or working with only the ISO chronology.
		/// </para>
		/// <para>
		/// For formatting, the {@code width} and {@code maxWidth} are used to
		/// determine the number of characters to format.
		/// If they are equal then the format is fixed width.
		/// If the value of the field is within the range of the {@code baseValue} using
		/// {@code width} characters then the reduced value is formatted otherwise the value is
		/// truncated to fit {@code maxWidth}.
		/// The rightmost characters are output to match the width, left padding with zero.
		/// </para>
		/// <para>
		/// For strict parsing, the number of characters allowed by {@code width} to {@code maxWidth} are parsed.
		/// For lenient parsing, the number of characters must be at least 1 and less than 10.
		/// If the number of digits parsed is equal to {@code width} and the value is positive,
		/// the value of the field is computed to be the first number greater than
		/// or equal to the {@code baseValue} with the same least significant characters,
		/// otherwise the value parsed is the field value.
		/// This allows a reduced value to be entered for values in range of the baseValue
		/// and width and absolute values can be entered for values outside the range.
		/// </para>
		/// <para>
		/// For example, a base value of {@code 1980} and a width of {@code 2} will have
		/// valid values from {@code 1980} to {@code 2079}.
		/// During parsing, the text {@code "12"} will result in the value {@code 2012} as that
		/// is the value within the range where the last two characters are "12".
		/// By contrast, parsing the text {@code "1915"} will result in the value {@code 1915}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="width">  the field width of the printed and parsed field, from 1 to 10 </param>
		/// <param name="maxWidth">  the maximum field width of the printed field, from 1 to 10 </param>
		/// <param name="baseValue">  the base value of the range of valid values </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the width or base value is invalid </exception>
		public DateTimeFormatterBuilder AppendValueReduced(TemporalField field, int width, int maxWidth, int baseValue)
		{
			Objects.RequireNonNull(field, "field");
			ReducedPrinterParser pp = new ReducedPrinterParser(field, width, maxWidth, baseValue, null);
			AppendValue(pp);
			return this;
		}

		/// <summary>
		/// Appends the reduced value of a date-time field to the formatter.
		/// <para>
		/// This is typically used for formatting and parsing a two digit year.
		/// </para>
		/// <para>
		/// The base date is used to calculate the full value during parsing.
		/// For example, if the base date is 1950-01-01 then parsed values for
		/// a two digit year parse will be in the range 1950-01-01 to 2049-12-31.
		/// Only the year would be extracted from the date, thus a base date of
		/// 1950-08-25 would also parse to the range 1950-01-01 to 2049-12-31.
		/// This behavior is necessary to support fields such as week-based-year
		/// or other calendar systems where the parsed value does not align with
		/// standard ISO years.
		/// </para>
		/// <para>
		/// The exact behavior is as follows. Parse the full set of fields and
		/// determine the effective chronology using the last chronology if
		/// it appears more than once. Then convert the base date to the
		/// effective chronology. Then extract the specified field from the
		/// chronology-specific base date and use it to determine the
		/// {@code baseValue} used below.
		/// </para>
		/// <para>
		/// For formatting, the {@code width} and {@code maxWidth} are used to
		/// determine the number of characters to format.
		/// If they are equal then the format is fixed width.
		/// If the value of the field is within the range of the {@code baseValue} using
		/// {@code width} characters then the reduced value is formatted otherwise the value is
		/// truncated to fit {@code maxWidth}.
		/// The rightmost characters are output to match the width, left padding with zero.
		/// </para>
		/// <para>
		/// For strict parsing, the number of characters allowed by {@code width} to {@code maxWidth} are parsed.
		/// For lenient parsing, the number of characters must be at least 1 and less than 10.
		/// If the number of digits parsed is equal to {@code width} and the value is positive,
		/// the value of the field is computed to be the first number greater than
		/// or equal to the {@code baseValue} with the same least significant characters,
		/// otherwise the value parsed is the field value.
		/// This allows a reduced value to be entered for values in range of the baseValue
		/// and width and absolute values can be entered for values outside the range.
		/// </para>
		/// <para>
		/// For example, a base value of {@code 1980} and a width of {@code 2} will have
		/// valid values from {@code 1980} to {@code 2079}.
		/// During parsing, the text {@code "12"} will result in the value {@code 2012} as that
		/// is the value within the range where the last two characters are "12".
		/// By contrast, parsing the text {@code "1915"} will result in the value {@code 1915}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="width">  the field width of the printed and parsed field, from 1 to 10 </param>
		/// <param name="maxWidth">  the maximum field width of the printed field, from 1 to 10 </param>
		/// <param name="baseDate">  the base date used to calculate the base value for the range
		///  of valid values in the parsed chronology, not null </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the width or base value is invalid </exception>
		public DateTimeFormatterBuilder AppendValueReduced(TemporalField field, int width, int maxWidth, ChronoLocalDate baseDate)
		{
			Objects.RequireNonNull(field, "field");
			Objects.RequireNonNull(baseDate, "baseDate");
			ReducedPrinterParser pp = new ReducedPrinterParser(field, width, maxWidth, 0, baseDate);
			AppendValue(pp);
			return this;
		}

		/// <summary>
		/// Appends a fixed or variable width printer-parser handling adjacent value mode.
		/// If a PrinterParser is not active then the new PrinterParser becomes
		/// the active PrinterParser.
		/// Otherwise, the active PrinterParser is modified depending on the new PrinterParser.
		/// If the new PrinterParser is fixed width and has sign style {@code NOT_NEGATIVE}
		/// then its width is added to the active PP and
		/// the new PrinterParser is forced to be fixed width.
		/// If the new PrinterParser is variable width, the active PrinterParser is changed
		/// to be fixed width and the new PrinterParser becomes the active PP.
		/// </summary>
		/// <param name="pp">  the printer-parser, not null </param>
		/// <returns> this, for chaining, not null </returns>
		private DateTimeFormatterBuilder AppendValue(NumberPrinterParser pp)
		{
			if (Active.ValueParserIndex >= 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int activeValueParser = active.valueParserIndex;
				int activeValueParser = Active.ValueParserIndex;

				// adjacent parsing mode, update setting in previous parsers
				NumberPrinterParser basePP = (NumberPrinterParser) Active.PrinterParsers[activeValueParser];
				if (pp.MinWidth == pp.MaxWidth && pp.SignStyle == SignStyle.NOT_NEGATIVE)
				{
					// Append the width to the subsequentWidth of the active parser
					basePP = basePP.WithSubsequentWidth(pp.MaxWidth);
					// Append the new parser as a fixed width
					AppendInternal(pp.WithFixedWidth());
					// Retain the previous active parser
					Active.ValueParserIndex = activeValueParser;
				}
				else
				{
					// Modify the active parser to be fixed width
					basePP = basePP.WithFixedWidth();
					// The new parser becomes the mew active parser
					Active.ValueParserIndex = AppendInternal(pp);
				}
				// Replace the modified parser with the updated one
				Active.PrinterParsers[activeValueParser] = basePP;
			}
			else
			{
				// The new Parser becomes the active parser
				Active.ValueParserIndex = AppendInternal(pp);
			}
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the fractional value of a date-time field to the formatter.
		/// <para>
		/// The fractional value of the field will be output including the
		/// preceding decimal point. The preceding value is not output.
		/// For example, the second-of-minute value of 15 would be output as {@code .25}.
		/// </para>
		/// <para>
		/// The width of the printed fraction can be controlled. Setting the
		/// minimum width to zero will cause no output to be generated.
		/// The printed fraction will have the minimum width necessary between
		/// the minimum and maximum widths - trailing zeroes are omitted.
		/// No rounding occurs due to the maximum width - digits are simply dropped.
		/// </para>
		/// <para>
		/// When parsing in strict mode, the number of parsed digits must be between
		/// the minimum and maximum width. When parsing in lenient mode, the minimum
		/// width is considered to be zero and the maximum is nine.
		/// </para>
		/// <para>
		/// If the value cannot be obtained then an exception will be thrown.
		/// If the value is negative an exception will be thrown.
		/// If the field does not have a fixed set of valid values then an
		/// exception will be thrown.
		/// If the field value in the date-time to be printed is invalid it
		/// cannot be printed and an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="minWidth">  the minimum width of the field excluding the decimal point, from 0 to 9 </param>
		/// <param name="maxWidth">  the maximum width of the field excluding the decimal point, from 1 to 9 </param>
		/// <param name="decimalPoint">  whether to output the localized decimal point symbol </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the field has a variable set of valid values or
		///  either width is invalid </exception>
		public DateTimeFormatterBuilder AppendFraction(TemporalField field, int minWidth, int maxWidth, bool decimalPoint)
		{
			AppendInternal(new FractionPrinterParser(field, minWidth, maxWidth, decimalPoint));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the text of a date-time field to the formatter using the full
		/// text style.
		/// <para>
		/// The text of the field will be output during a format.
		/// The value must be within the valid range of the field.
		/// If the value cannot be obtained then an exception will be thrown.
		/// If the field has no textual representation, then the numeric value will be used.
		/// </para>
		/// <para>
		/// The value will be printed as per the normal format of an integer value.
		/// Only negative numbers will be signed. No padding will be added.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendText(TemporalField field)
		{
			return AppendText(field, TextStyle.FULL);
		}

		/// <summary>
		/// Appends the text of a date-time field to the formatter.
		/// <para>
		/// The text of the field will be output during a format.
		/// The value must be within the valid range of the field.
		/// If the value cannot be obtained then an exception will be thrown.
		/// If the field has no textual representation, then the numeric value will be used.
		/// </para>
		/// <para>
		/// The value will be printed as per the normal format of an integer value.
		/// Only negative numbers will be signed. No padding will be added.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="textStyle">  the text style to use, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendText(TemporalField field, TextStyle textStyle)
		{
			Objects.RequireNonNull(field, "field");
			Objects.RequireNonNull(textStyle, "textStyle");
			AppendInternal(new TextPrinterParser(field, textStyle, DateTimeTextProvider.Instance));
			return this;
		}

		/// <summary>
		/// Appends the text of a date-time field to the formatter using the specified
		/// map to supply the text.
		/// <para>
		/// The standard text outputting methods use the localized text in the JDK.
		/// This method allows that text to be specified directly.
		/// The supplied map is not validated by the builder to ensure that formatting or
		/// parsing is possible, thus an invalid map may throw an error during later use.
		/// </para>
		/// <para>
		/// Supplying the map of text provides considerable flexibility in formatting and parsing.
		/// For example, a legacy application might require or supply the months of the
		/// year as "JNY", "FBY", "MCH" etc. These do not match the standard set of text
		/// for localized month names. Using this method, a map can be created which
		/// defines the connection between each value and the text:
		/// <pre>
		/// Map&lt;Long, String&gt; map = new HashMap&lt;&gt;();
		/// map.put(1L, "JNY");
		/// map.put(2L, "FBY");
		/// map.put(3L, "MCH");
		/// ...
		/// builder.appendText(MONTH_OF_YEAR, map);
		/// </pre>
		/// </para>
		/// <para>
		/// Other uses might be to output the value with a suffix, such as "1st", "2nd", "3rd",
		/// or as Roman numerals "I", "II", "III", "IV".
		/// </para>
		/// <para>
		/// During formatting, the value is obtained and checked that it is in the valid range.
		/// If text is not available for the value then it is output as a number.
		/// During parsing, the parser will match against the map of text and numeric values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to append, not null </param>
		/// <param name="textLookup">  the map from the value to the text </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendText(TemporalField field, IDictionary<Long, String> textLookup)
		{
			Objects.RequireNonNull(field, "field");
			Objects.RequireNonNull(textLookup, "textLookup");
			IDictionary<Long, String> copy = new LinkedHashMap<Long, String>(textLookup);
			IDictionary<TextStyle, IDictionary<Long, String>> map = Collections.SingletonMap(TextStyle.FULL, copy);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.time.format.DateTimeTextProvider.LocaleStore store = new java.time.format.DateTimeTextProvider.LocaleStore(map);
			LocaleStore store = new LocaleStore(map);
			DateTimeTextProvider provider = new DateTimeTextProviderAnonymousInnerClassHelper(this, field, store);
			AppendInternal(new TextPrinterParser(field, TextStyle.FULL, provider));
			return this;
		}

		private class DateTimeTextProviderAnonymousInnerClassHelper : DateTimeTextProvider
		{
			private readonly DateTimeFormatterBuilder OuterInstance;

			private TemporalField Field;
			private LocaleStore Store;

			public DateTimeTextProviderAnonymousInnerClassHelper(DateTimeFormatterBuilder outerInstance, TemporalField field, LocaleStore store)
			{
				this.OuterInstance = outerInstance;
				this.Field = field;
				this.Store = store;
			}

			public override String GetText(TemporalField field, long value, TextStyle style, Locale locale)
			{
				return Store.GetText(value, style);
			}
			public override IEnumerator<Map_Entry<String, Long>> GetTextIterator(TemporalField field, TextStyle style, Locale locale)
			{
				return Store.GetTextIterator(style);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends an instant using ISO-8601 to the formatter, formatting fractional
		/// digits in groups of three.
		/// <para>
		/// Instants have a fixed output format.
		/// They are converted to a date-time with a zone-offset of UTC and formatted
		/// using the standard ISO-8601 format.
		/// With this method, formatting nano-of-second outputs zero, three, six
		/// or nine digits digits as necessary.
		/// The localized decimal style is not used.
		/// </para>
		/// <para>
		/// The instant is obtained using <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		/// and optionally (@code NANO_OF_SECOND). The value of {@code INSTANT_SECONDS}
		/// may be outside the maximum range of {@code LocalDateTime}.
		/// </para>
		/// <para>
		/// The <seealso cref="ResolverStyle resolver style"/> has no effect on instant parsing.
		/// The end-of-day time of '24:00' is handled as midnight at the start of the following day.
		/// The leap-second time of '23:59:59' is handled to some degree, see
		/// <seealso cref="DateTimeFormatter#parsedLeapSecond()"/> for full details.
		/// </para>
		/// <para>
		/// An alternative to this method is to format/parse the instant as a single
		/// epoch-seconds value. That is achieved using {@code appendValue(INSTANT_SECONDS)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendInstant()
		{
			AppendInternal(new InstantPrinterParser(-2));
			return this;
		}

		/// <summary>
		/// Appends an instant using ISO-8601 to the formatter with control over
		/// the number of fractional digits.
		/// <para>
		/// Instants have a fixed output format, although this method provides some
		/// control over the fractional digits. They are converted to a date-time
		/// with a zone-offset of UTC and printed using the standard ISO-8601 format.
		/// The localized decimal style is not used.
		/// </para>
		/// <para>
		/// The {@code fractionalDigits} parameter allows the output of the fractional
		/// second to be controlled. Specifying zero will cause no fractional digits
		/// to be output. From 1 to 9 will output an increasing number of digits, using
		/// zero right-padding if necessary. The special value -1 is used to output as
		/// many digits as necessary to avoid any trailing zeroes.
		/// </para>
		/// <para>
		/// When parsing in strict mode, the number of parsed digits must match the
		/// fractional digits. When parsing in lenient mode, any number of fractional
		/// digits from zero to nine are accepted.
		/// </para>
		/// <para>
		/// The instant is obtained using <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		/// and optionally (@code NANO_OF_SECOND). The value of {@code INSTANT_SECONDS}
		/// may be outside the maximum range of {@code LocalDateTime}.
		/// </para>
		/// <para>
		/// The <seealso cref="ResolverStyle resolver style"/> has no effect on instant parsing.
		/// The end-of-day time of '24:00' is handled as midnight at the start of the following day.
		/// The leap-second time of '23:59:60' is handled to some degree, see
		/// <seealso cref="DateTimeFormatter#parsedLeapSecond()"/> for full details.
		/// </para>
		/// <para>
		/// An alternative to this method is to format/parse the instant as a single
		/// epoch-seconds value. That is achieved using {@code appendValue(INSTANT_SECONDS)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fractionalDigits">  the number of fractional second digits to format with,
		///  from 0 to 9, or -1 to use as many digits as necessary </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendInstant(int fractionalDigits)
		{
			if (fractionalDigits < -1 || fractionalDigits > 9)
			{
				throw new IllegalArgumentException("The fractional digits must be from -1 to 9 inclusive but was " + fractionalDigits);
			}
			AppendInternal(new InstantPrinterParser(fractionalDigits));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the zone offset, such as '+01:00', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the offset ID to the builder.
		/// This is equivalent to calling {@code appendOffset("+HH:MM:ss", "Z")}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendOffsetId()
		{
			AppendInternal(OffsetIdPrinterParser.INSTANCE_ID_Z);
			return this;
		}

		/// <summary>
		/// Appends the zone offset, such as '+01:00', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the offset ID to the builder.
		/// </para>
		/// <para>
		/// During formatting, the offset is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#offset()"/>.
		/// It will be printed using the format defined below.
		/// If the offset cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, the offset is parsed using the format defined below.
		/// If the offset cannot be parsed then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// The format of the offset is controlled by a pattern which must be one
		/// of the following:
		/// <ul>
		/// <li>{@code +HH} - hour only, ignoring minute and second
		/// <li>{@code +HHmm} - hour, with minute if non-zero, ignoring second, no colon
		/// <li>{@code +HH:mm} - hour, with minute if non-zero, ignoring second, with colon
		/// <li>{@code +HHMM} - hour and minute, ignoring second, no colon
		/// <li>{@code +HH:MM} - hour and minute, ignoring second, with colon
		/// <li>{@code +HHMMss} - hour and minute, with second if non-zero, no colon
		/// <li>{@code +HH:MM:ss} - hour and minute, with second if non-zero, with colon
		/// <li>{@code +HHMMSS} - hour, minute and second, no colon
		/// <li>{@code +HH:MM:SS} - hour, minute and second, with colon
		/// </ul>
		/// The "no offset" text controls what text is printed when the total amount of
		/// the offset fields to be output is zero.
		/// Example values would be 'Z', '+00:00', 'UTC' or 'GMT'.
		/// Three formats are accepted for parsing UTC - the "no offset" text, and the
		/// plus and minus versions of zero defined by the pattern.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern to use, not null </param>
		/// <param name="noOffsetText">  the text to use when the offset is zero, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendOffset(String pattern, String noOffsetText)
		{
			AppendInternal(new OffsetIdPrinterParser(pattern, noOffsetText));
			return this;
		}

		/// <summary>
		/// Appends the localized zone offset, such as 'GMT+01:00', to the formatter.
		/// <para>
		/// This appends a localized zone offset to the builder, the format of the
		/// localized offset is controlled by the specified <seealso cref="FormatStyle style"/>
		/// to this method:
		/// <ul>
		/// <li><seealso cref="TextStyle#FULL full"/> - formats with localized offset text, such
		/// as 'GMT, 2-digit hour and minute field, optional second field if non-zero,
		/// and colon.
		/// <li><seealso cref="TextStyle#SHORT short"/> - formats with localized offset text,
		/// such as 'GMT, hour without leading zero, optional 2-digit minute and
		/// second if non-zero, and colon.
		/// </ul>
		/// </para>
		/// <para>
		/// During formatting, the offset is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#offset()"/>.
		/// If the offset cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, the offset is parsed using the format defined above.
		/// If the offset cannot be parsed then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="style">  the format style to use, not null </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if style is neither {@link TextStyle#FULL
		/// full} nor <seealso cref="TextStyle#SHORT short"/> </exception>
		public DateTimeFormatterBuilder AppendLocalizedOffset(TextStyle style)
		{
			Objects.RequireNonNull(style, "style");
			if (style != TextStyle.FULL && style != TextStyle.SHORT)
			{
				throw new IllegalArgumentException("Style must be either full or short");
			}
			AppendInternal(new LocalizedOffsetIdPrinterParser(style));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the time-zone ID, such as 'Europe/Paris' or '+02:00', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the zone ID to the builder.
		/// The zone ID is obtained in a strict manner suitable for {@code ZonedDateTime}.
		/// By contrast, {@code OffsetDateTime} does not have a zone ID suitable
		/// for use with this method, see <seealso cref="#appendZoneOrOffsetId()"/>.
		/// </para>
		/// <para>
		/// During formatting, the zone is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		/// It will be printed using the result of <seealso cref="ZoneId#getId()"/>.
		/// If the zone cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, the text must match a known zone or offset.
		/// There are two types of zone ID, offset-based, such as '+01:30' and
		/// region-based, such as 'Europe/London'. These are parsed differently.
		/// If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		/// expects an offset-based zone and will not match region-based zones.
		/// The offset ID, such as '+02:30', may be at the start of the parse,
		/// or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		/// equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		/// arguments 'HH:MM:ss' and the no offset string '0'.
		/// If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		/// match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		/// In all other cases, the list of known region-based zones is used to
		/// find the longest available match. If no match is found, and the parse
		/// starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		/// The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		/// </para>
		/// <para>
		/// For example, the following will parse:
		/// <pre>
		///   "Europe/London"           -- ZoneId.of("Europe/London")
		///   "Z"                       -- ZoneOffset.UTC
		///   "UT"                      -- ZoneId.of("UT")
		///   "UTC"                     -- ZoneId.of("UTC")
		///   "GMT"                     -- ZoneId.of("GMT")
		///   "+01:30"                  -- ZoneOffset.of("+01:30")
		///   "UT+01:30"                -- ZoneOffset.of("+01:30")
		///   "UTC+01:30"               -- ZoneOffset.of("+01:30")
		///   "GMT+01:30"               -- ZoneOffset.of("+01:30")
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		/// <seealso cref= #appendZoneRegionId() </seealso>
		public DateTimeFormatterBuilder AppendZoneId()
		{
			AppendInternal(new ZoneIdPrinterParser(TemporalQueries.ZoneId(), "ZoneId()"));
			return this;
		}

		/// <summary>
		/// Appends the time-zone region ID, such as 'Europe/Paris', to the formatter,
		/// rejecting the zone ID if it is a {@code ZoneOffset}.
		/// <para>
		/// This appends an instruction to format/parse the zone ID to the builder
		/// only if it is a region-based ID.
		/// </para>
		/// <para>
		/// During formatting, the zone is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		/// If the zone is a {@code ZoneOffset} or it cannot be obtained then
		/// an exception is thrown unless the section of the formatter is optional.
		/// If the zone is not an offset, then the zone will be printed using
		/// the zone ID from <seealso cref="ZoneId#getId()"/>.
		/// </para>
		/// <para>
		/// During parsing, the text must match a known zone or offset.
		/// There are two types of zone ID, offset-based, such as '+01:30' and
		/// region-based, such as 'Europe/London'. These are parsed differently.
		/// If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		/// expects an offset-based zone and will not match region-based zones.
		/// The offset ID, such as '+02:30', may be at the start of the parse,
		/// or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		/// equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		/// arguments 'HH:MM:ss' and the no offset string '0'.
		/// If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		/// match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		/// In all other cases, the list of known region-based zones is used to
		/// find the longest available match. If no match is found, and the parse
		/// starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		/// The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		/// </para>
		/// <para>
		/// For example, the following will parse:
		/// <pre>
		///   "Europe/London"           -- ZoneId.of("Europe/London")
		///   "Z"                       -- ZoneOffset.UTC
		///   "UT"                      -- ZoneId.of("UT")
		///   "UTC"                     -- ZoneId.of("UTC")
		///   "GMT"                     -- ZoneId.of("GMT")
		///   "+01:30"                  -- ZoneOffset.of("+01:30")
		///   "UT+01:30"                -- ZoneOffset.of("+01:30")
		///   "UTC+01:30"               -- ZoneOffset.of("+01:30")
		///   "GMT+01:30"               -- ZoneOffset.of("+01:30")
		/// </pre>
		/// </para>
		/// <para>
		/// Note that this method is identical to {@code appendZoneId()} except
		/// in the mechanism used to obtain the zone.
		/// Note also that parsing accepts offsets, whereas formatting will never
		/// produce one.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		/// <seealso cref= #appendZoneId() </seealso>
		public DateTimeFormatterBuilder AppendZoneRegionId()
		{
			AppendInternal(new ZoneIdPrinterParser(QUERY_REGION_ONLY, "ZoneRegionId()"));
			return this;
		}

		/// <summary>
		/// Appends the time-zone ID, such as 'Europe/Paris' or '+02:00', to
		/// the formatter, using the best available zone ID.
		/// <para>
		/// This appends an instruction to format/parse the best available
		/// zone or offset ID to the builder.
		/// The zone ID is obtained in a lenient manner that first attempts to
		/// find a true zone ID, such as that on {@code ZonedDateTime}, and
		/// then attempts to find an offset, such as that on {@code OffsetDateTime}.
		/// </para>
		/// <para>
		/// During formatting, the zone is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#zone()"/>.
		/// It will be printed using the result of <seealso cref="ZoneId#getId()"/>.
		/// If the zone cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, the text must match a known zone or offset.
		/// There are two types of zone ID, offset-based, such as '+01:30' and
		/// region-based, such as 'Europe/London'. These are parsed differently.
		/// If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		/// expects an offset-based zone and will not match region-based zones.
		/// The offset ID, such as '+02:30', may be at the start of the parse,
		/// or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		/// equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		/// arguments 'HH:MM:ss' and the no offset string '0'.
		/// If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		/// match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		/// In all other cases, the list of known region-based zones is used to
		/// find the longest available match. If no match is found, and the parse
		/// starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		/// The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		/// </para>
		/// <para>
		/// For example, the following will parse:
		/// <pre>
		///   "Europe/London"           -- ZoneId.of("Europe/London")
		///   "Z"                       -- ZoneOffset.UTC
		///   "UT"                      -- ZoneId.of("UT")
		///   "UTC"                     -- ZoneId.of("UTC")
		///   "GMT"                     -- ZoneId.of("GMT")
		///   "+01:30"                  -- ZoneOffset.of("+01:30")
		///   "UT+01:30"                -- ZoneOffset.of("UT+01:30")
		///   "UTC+01:30"               -- ZoneOffset.of("UTC+01:30")
		///   "GMT+01:30"               -- ZoneOffset.of("GMT+01:30")
		/// </pre>
		/// </para>
		/// <para>
		/// Note that this method is identical to {@code appendZoneId()} except
		/// in the mechanism used to obtain the zone.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		/// <seealso cref= #appendZoneId() </seealso>
		public DateTimeFormatterBuilder AppendZoneOrOffsetId()
		{
			AppendInternal(new ZoneIdPrinterParser(TemporalQueries.Zone(), "ZoneOrOffsetId()"));
			return this;
		}

		/// <summary>
		/// Appends the time-zone name, such as 'British Summer Time', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the textual name of the zone to
		/// the builder.
		/// </para>
		/// <para>
		/// During formatting, the zone is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		/// If the zone is a {@code ZoneOffset} it will be printed using the
		/// result of <seealso cref="ZoneOffset#getId()"/>.
		/// If the zone is not an offset, the textual name will be looked up
		/// for the locale set in the <seealso cref="DateTimeFormatter"/>.
		/// If the temporal object being printed represents an instant, then the text
		/// will be the summer or winter time text as appropriate.
		/// If the lookup for text does not find any suitable result, then the
		/// <seealso cref="ZoneId#getId() ID"/> will be printed instead.
		/// If the zone cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, either the textual zone name, the zone ID or the offset
		/// is accepted. Many textual zone names are not unique, such as CST can be
		/// for both "Central Standard Time" and "China Standard Time". In this
		/// situation, the zone id will be determined by the region information from
		/// formatter's  <seealso cref="DateTimeFormatter#getLocale() locale"/> and the standard
		/// zone id for that area, for example, America/New_York for the America Eastern
		/// zone. The <seealso cref="#appendZoneText(TextStyle, Set)"/> may be used
		/// to specify a set of preferred <seealso cref="ZoneId"/> in this situation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="textStyle">  the text style to use, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendZoneText(TextStyle textStyle)
		{
			AppendInternal(new ZoneTextPrinterParser(textStyle, null));
			return this;
		}

		/// <summary>
		/// Appends the time-zone name, such as 'British Summer Time', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the textual name of the zone to
		/// the builder.
		/// </para>
		/// <para>
		/// During formatting, the zone is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		/// If the zone is a {@code ZoneOffset} it will be printed using the
		/// result of <seealso cref="ZoneOffset#getId()"/>.
		/// If the zone is not an offset, the textual name will be looked up
		/// for the locale set in the <seealso cref="DateTimeFormatter"/>.
		/// If the temporal object being printed represents an instant, then the text
		/// will be the summer or winter time text as appropriate.
		/// If the lookup for text does not find any suitable result, then the
		/// <seealso cref="ZoneId#getId() ID"/> will be printed instead.
		/// If the zone cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, either the textual zone name, the zone ID or the offset
		/// is accepted. Many textual zone names are not unique, such as CST can be
		/// for both "Central Standard Time" and "China Standard Time". In this
		/// situation, the zone id will be determined by the region information from
		/// formatter's  <seealso cref="DateTimeFormatter#getLocale() locale"/> and the standard
		/// zone id for that area, for example, America/New_York for the America Eastern
		/// zone. This method also allows a set of preferred <seealso cref="ZoneId"/> to be
		/// specified for parsing. The matched preferred zone id will be used if the
		/// textural zone name being parsed is not unique.
		/// </para>
		/// <para>
		/// If the zone cannot be parsed then an exception is thrown unless the
		/// section of the formatter is optional.
		/// 
		/// </para>
		/// </summary>
		/// <param name="textStyle">  the text style to use, not null </param>
		/// <param name="preferredZones">  the set of preferred zone ids, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendZoneText(TextStyle textStyle, Set<ZoneId> preferredZones)
		{
			Objects.RequireNonNull(preferredZones, "preferredZones");
			AppendInternal(new ZoneTextPrinterParser(textStyle, preferredZones));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the chronology ID, such as 'ISO' or 'ThaiBuddhist', to the formatter.
		/// <para>
		/// This appends an instruction to format/parse the chronology ID to the builder.
		/// </para>
		/// <para>
		/// During formatting, the chronology is obtained using a mechanism equivalent
		/// to querying the temporal with <seealso cref="TemporalQueries#chronology()"/>.
		/// It will be printed using the result of <seealso cref="Chronology#getId()"/>.
		/// If the chronology cannot be obtained then an exception is thrown unless the
		/// section of the formatter is optional.
		/// </para>
		/// <para>
		/// During parsing, the chronology is parsed and must match one of the chronologies
		/// in <seealso cref="Chronology#getAvailableChronologies()"/>.
		/// If the chronology cannot be parsed then an exception is thrown unless the
		/// section of the formatter is optional.
		/// The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendChronologyId()
		{
			AppendInternal(new ChronoPrinterParser(null));
			return this;
		}

		/// <summary>
		/// Appends the chronology name to the formatter.
		/// <para>
		/// The calendar system name will be output during a format.
		/// If the chronology cannot be obtained then an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="textStyle">  the text style to use, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendChronologyText(TextStyle textStyle)
		{
			Objects.RequireNonNull(textStyle, "textStyle");
			AppendInternal(new ChronoPrinterParser(textStyle));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends a localized date-time pattern to the formatter.
		/// <para>
		/// This appends a localized section to the builder, suitable for outputting
		/// a date, time or date-time combination. The format of the localized
		/// section is lazily looked up based on four items:
		/// <ul>
		/// <li>the {@code dateStyle} specified to this method
		/// <li>the {@code timeStyle} specified to this method
		/// <li>the {@code Locale} of the {@code DateTimeFormatter}
		/// <li>the {@code Chronology}, selecting the best available
		/// </ul>
		/// During formatting, the chronology is obtained from the temporal object
		/// being formatted, which may have been overridden by
		/// <seealso cref="DateTimeFormatter#withChronology(Chronology)"/>.
		/// </para>
		/// <para>
		/// During parsing, if a chronology has already been parsed, then it is used.
		/// Otherwise the default from {@code DateTimeFormatter.withChronology(Chronology)}
		/// is used, with {@code IsoChronology} as the fallback.
		/// </para>
		/// <para>
		/// Note that this method provides similar functionality to methods on
		/// {@code DateFormat} such as <seealso cref="java.text.DateFormat#getDateTimeInstance(int, int)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateStyle">  the date style to use, null means no date required </param>
		/// <param name="timeStyle">  the time style to use, null means no time required </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if both the date and time styles are null </exception>
		public DateTimeFormatterBuilder AppendLocalized(FormatStyle dateStyle, FormatStyle timeStyle)
		{
			if (dateStyle == null && timeStyle == null)
			{
				throw new IllegalArgumentException("Either the date or time style must be non-null");
			}
			AppendInternal(new LocalizedPrinterParser(dateStyle, timeStyle));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends a character literal to the formatter.
		/// <para>
		/// This character will be output during a format.
		/// 
		/// </para>
		/// </summary>
		/// <param name="literal">  the literal to append, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendLiteral(char literal)
		{
			AppendInternal(new CharLiteralPrinterParser(literal));
			return this;
		}

		/// <summary>
		/// Appends a string literal to the formatter.
		/// <para>
		/// This string will be output during a format.
		/// </para>
		/// <para>
		/// If the literal is empty, nothing is added to the formatter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="literal">  the literal to append, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendLiteral(String literal)
		{
			Objects.RequireNonNull(literal, "literal");
			if (literal.Length() > 0)
			{
				if (literal.Length() == 1)
				{
					AppendInternal(new CharLiteralPrinterParser(literal.CharAt(0)));
				}
				else
				{
					AppendInternal(new StringLiteralPrinterParser(literal));
				}
			}
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends all the elements of a formatter to the builder.
		/// <para>
		/// This method has the same effect as appending each of the constituent
		/// parts of the formatter directly to this builder.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to add, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder Append(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			AppendInternal(formatter.ToPrinterParser(false));
			return this;
		}

		/// <summary>
		/// Appends a formatter to the builder which will optionally format/parse.
		/// <para>
		/// This method has the same effect as appending each of the constituent
		/// parts directly to this builder surrounded by an <seealso cref="#optionalStart()"/> and
		/// <seealso cref="#optionalEnd()"/>.
		/// </para>
		/// <para>
		/// The formatter will format if data is available for all the fields contained within it.
		/// The formatter will parse if the string matches, otherwise no error is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to add, not null </param>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder AppendOptional(DateTimeFormatter formatter)
		{
			Objects.RequireNonNull(formatter, "formatter");
			AppendInternal(formatter.ToPrinterParser(true));
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends the elements defined by the specified pattern to the builder.
		/// <para>
		/// All letters 'A' to 'Z' and 'a' to 'z' are reserved as pattern letters.
		/// The characters '#', '{' and '}' are reserved for future use.
		/// The characters '[' and ']' indicate optional patterns.
		/// The following pattern letters are defined:
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
		/// The count of pattern letters determine the format.
		/// See <a href="DateTimeFormatter.html#patterns">DateTimeFormatter</a> for a user-focused description of the patterns.
		/// The following tables define how the pattern letters map to the builder.
		/// </para>
		/// <para>
		/// <b>Date fields</b>: Pattern letters to output a date.
		/// <pre>
		///  Pattern  Count  Equivalent builder methods
		///  -------  -----  --------------------------
		///    G       1      appendText(ChronoField.ERA, TextStyle.SHORT)
		///    GG      2      appendText(ChronoField.ERA, TextStyle.SHORT)
		///    GGG     3      appendText(ChronoField.ERA, TextStyle.SHORT)
		///    GGGG    4      appendText(ChronoField.ERA, TextStyle.FULL)
		///    GGGGG   5      appendText(ChronoField.ERA, TextStyle.NARROW)
		/// 
		///    u       1      appendValue(ChronoField.YEAR, 1, 19, SignStyle.NORMAL);
		///    uu      2      appendValueReduced(ChronoField.YEAR, 2, 2000);
		///    uuu     3      appendValue(ChronoField.YEAR, 3, 19, SignStyle.NORMAL);
		///    u..u    4..n   appendValue(ChronoField.YEAR, n, 19, SignStyle.EXCEEDS_PAD);
		///    y       1      appendValue(ChronoField.YEAR_OF_ERA, 1, 19, SignStyle.NORMAL);
		///    yy      2      appendValueReduced(ChronoField.YEAR_OF_ERA, 2, 2000);
		///    yyy     3      appendValue(ChronoField.YEAR_OF_ERA, 3, 19, SignStyle.NORMAL);
		///    y..y    4..n   appendValue(ChronoField.YEAR_OF_ERA, n, 19, SignStyle.EXCEEDS_PAD);
		///    Y       1      append special localized WeekFields element for numeric week-based-year
		///    YY      2      append special localized WeekFields element for reduced numeric week-based-year 2 digits;
		///    YYY     3      append special localized WeekFields element for numeric week-based-year (3, 19, SignStyle.NORMAL);
		///    Y..Y    4..n   append special localized WeekFields element for numeric week-based-year (n, 19, SignStyle.EXCEEDS_PAD);
		/// 
		///    Q       1      appendValue(IsoFields.QUARTER_OF_YEAR);
		///    QQ      2      appendValue(IsoFields.QUARTER_OF_YEAR, 2);
		///    QQQ     3      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.SHORT)
		///    QQQQ    4      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.FULL)
		///    QQQQQ   5      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.NARROW)
		///    q       1      appendValue(IsoFields.QUARTER_OF_YEAR);
		///    qq      2      appendValue(IsoFields.QUARTER_OF_YEAR, 2);
		///    qqq     3      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.SHORT_STANDALONE)
		///    qqqq    4      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.FULL_STANDALONE)
		///    qqqqq   5      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.NARROW_STANDALONE)
		/// 
		///    M       1      appendValue(ChronoField.MONTH_OF_YEAR);
		///    MM      2      appendValue(ChronoField.MONTH_OF_YEAR, 2);
		///    MMM     3      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.SHORT)
		///    MMMM    4      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.FULL)
		///    MMMMM   5      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.NARROW)
		///    L       1      appendValue(ChronoField.MONTH_OF_YEAR);
		///    LL      2      appendValue(ChronoField.MONTH_OF_YEAR, 2);
		///    LLL     3      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.SHORT_STANDALONE)
		///    LLLL    4      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.FULL_STANDALONE)
		///    LLLLL   5      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.NARROW_STANDALONE)
		/// 
		///    w       1      append special localized WeekFields element for numeric week-of-year
		///    ww      2      append special localized WeekFields element for numeric week-of-year, zero-padded
		///    W       1      append special localized WeekFields element for numeric week-of-month
		///    d       1      appendValue(ChronoField.DAY_OF_MONTH)
		///    dd      2      appendValue(ChronoField.DAY_OF_MONTH, 2)
		///    D       1      appendValue(ChronoField.DAY_OF_YEAR)
		///    DD      2      appendValue(ChronoField.DAY_OF_YEAR, 2)
		///    DDD     3      appendValue(ChronoField.DAY_OF_YEAR, 3)
		///    F       1      appendValue(ChronoField.ALIGNED_DAY_OF_WEEK_IN_MONTH)
		///    E       1      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		///    EE      2      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		///    EEE     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		///    EEEE    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL)
		///    EEEEE   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW)
		///    e       1      append special localized WeekFields element for numeric day-of-week
		///    ee      2      append special localized WeekFields element for numeric day-of-week, zero-padded
		///    eee     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		///    eeee    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL)
		///    eeeee   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW)
		///    c       1      append special localized WeekFields element for numeric day-of-week
		///    ccc     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT_STANDALONE)
		///    cccc    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL_STANDALONE)
		///    ccccc   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW_STANDALONE)
		/// </pre>
		/// </para>
		/// <para>
		/// <b>Time fields</b>: Pattern letters to output a time.
		/// <pre>
		///  Pattern  Count  Equivalent builder methods
		///  -------  -----  --------------------------
		///    a       1      appendText(ChronoField.AMPM_OF_DAY, TextStyle.SHORT)
		///    h       1      appendValue(ChronoField.CLOCK_HOUR_OF_AMPM)
		///    hh      2      appendValue(ChronoField.CLOCK_HOUR_OF_AMPM, 2)
		///    H       1      appendValue(ChronoField.HOUR_OF_DAY)
		///    HH      2      appendValue(ChronoField.HOUR_OF_DAY, 2)
		///    k       1      appendValue(ChronoField.CLOCK_HOUR_OF_DAY)
		///    kk      2      appendValue(ChronoField.CLOCK_HOUR_OF_DAY, 2)
		///    K       1      appendValue(ChronoField.HOUR_OF_AMPM)
		///    KK      2      appendValue(ChronoField.HOUR_OF_AMPM, 2)
		///    m       1      appendValue(ChronoField.MINUTE_OF_HOUR)
		///    mm      2      appendValue(ChronoField.MINUTE_OF_HOUR, 2)
		///    s       1      appendValue(ChronoField.SECOND_OF_MINUTE)
		///    ss      2      appendValue(ChronoField.SECOND_OF_MINUTE, 2)
		/// 
		///    S..S    1..n   appendFraction(ChronoField.NANO_OF_SECOND, n, n, false)
		///    A       1      appendValue(ChronoField.MILLI_OF_DAY)
		///    A..A    2..n   appendValue(ChronoField.MILLI_OF_DAY, n)
		///    n       1      appendValue(ChronoField.NANO_OF_SECOND)
		///    n..n    2..n   appendValue(ChronoField.NANO_OF_SECOND, n)
		///    N       1      appendValue(ChronoField.NANO_OF_DAY)
		///    N..N    2..n   appendValue(ChronoField.NANO_OF_DAY, n)
		/// </pre>
		/// </para>
		/// <para>
		/// <b>Zone ID</b>: Pattern letters to output {@code ZoneId}.
		/// <pre>
		///  Pattern  Count  Equivalent builder methods
		///  -------  -----  --------------------------
		///    VV      2      appendZoneId()
		///    z       1      appendZoneText(TextStyle.SHORT)
		///    zz      2      appendZoneText(TextStyle.SHORT)
		///    zzz     3      appendZoneText(TextStyle.SHORT)
		///    zzzz    4      appendZoneText(TextStyle.FULL)
		/// </pre>
		/// </para>
		/// <para>
		/// <b>Zone offset</b>: Pattern letters to output {@code ZoneOffset}.
		/// <pre>
		///  Pattern  Count  Equivalent builder methods
		///  -------  -----  --------------------------
		///    O       1      appendLocalizedOffsetPrefixed(TextStyle.SHORT);
		///    OOOO    4      appendLocalizedOffsetPrefixed(TextStyle.FULL);
		///    X       1      appendOffset("+HHmm","Z")
		///    XX      2      appendOffset("+HHMM","Z")
		///    XXX     3      appendOffset("+HH:MM","Z")
		///    XXXX    4      appendOffset("+HHMMss","Z")
		///    XXXXX   5      appendOffset("+HH:MM:ss","Z")
		///    x       1      appendOffset("+HHmm","+00")
		///    xx      2      appendOffset("+HHMM","+0000")
		///    xxx     3      appendOffset("+HH:MM","+00:00")
		///    xxxx    4      appendOffset("+HHMMss","+0000")
		///    xxxxx   5      appendOffset("+HH:MM:ss","+00:00")
		///    Z       1      appendOffset("+HHMM","+0000")
		///    ZZ      2      appendOffset("+HHMM","+0000")
		///    ZZZ     3      appendOffset("+HHMM","+0000")
		///    ZZZZ    4      appendLocalizedOffset(TextStyle.FULL);
		///    ZZZZZ   5      appendOffset("+HH:MM:ss","Z")
		/// </pre>
		/// </para>
		/// <para>
		/// <b>Modifiers</b>: Pattern letters that modify the rest of the pattern:
		/// <pre>
		///  Pattern  Count  Equivalent builder methods
		///  -------  -----  --------------------------
		///    [       1      optionalStart()
		///    ]       1      optionalEnd()
		///    p..p    1..n   padNext(n)
		/// </pre>
		/// </para>
		/// <para>
		/// Any sequence of letters not specified above, unrecognized letter or
		/// reserved character will throw an exception.
		/// Future versions may add to the set of patterns.
		/// It is recommended to use single quotes around all characters that you want
		/// to output directly to ensure that future changes do not break your application.
		/// </para>
		/// <para>
		/// Note that the pattern string is similar, but not identical, to
		/// <seealso cref="java.text.SimpleDateFormat SimpleDateFormat"/>.
		/// The pattern string is also similar, but not identical, to that defined by the
		/// Unicode Common Locale Data Repository (CLDR/LDML).
		/// Pattern letters 'X' and 'u' are aligned with Unicode CLDR/LDML.
		/// By contrast, {@code SimpleDateFormat} uses 'u' for the numeric day of week.
		/// Pattern letters 'y' and 'Y' parse years of two digits and more than 4 digits differently.
		/// Pattern letters 'n', 'A', 'N', and 'p' are added.
		/// Number types will reject large numbers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern">  the pattern to add, not null </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		public DateTimeFormatterBuilder AppendPattern(String pattern)
		{
			Objects.RequireNonNull(pattern, "pattern");
			ParsePattern(pattern);
			return this;
		}

		private void ParsePattern(String pattern)
		{
			for (int pos = 0; pos < pattern.Length(); pos++)
			{
				char cur = pattern.CharAt(pos);
				if ((cur >= 'A' && cur <= 'Z') || (cur >= 'a' && cur <= 'z'))
				{
					int start = pos++;
					for (; pos < pattern.Length() && pattern.CharAt(pos) == cur; pos++) // short loop
					{
						;
					}
					int count = pos - start;
					// padding
					if (cur == 'p')
					{
						int pad = 0;
						if (pos < pattern.Length())
						{
							cur = pattern.CharAt(pos);
							if ((cur >= 'A' && cur <= 'Z') || (cur >= 'a' && cur <= 'z'))
							{
								pad = count;
								start = pos++;
								for (; pos < pattern.Length() && pattern.CharAt(pos) == cur; pos++) // short loop
								{
									;
								}
								count = pos - start;
							}
						}
						if (pad == 0)
						{
							throw new IllegalArgumentException("Pad letter 'p' must be followed by valid pad pattern: " + pattern);
						}
						PadNext(pad); // pad and continue parsing
					}
					// main rules
					TemporalField field = FIELD_MAP[cur];
					if (field != null)
					{
						ParseField(cur, count, field);
					}
					else if (cur == 'z')
					{
						if (count > 4)
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
						else if (count == 4)
						{
							AppendZoneText(TextStyle.FULL);
						}
						else
						{
							AppendZoneText(TextStyle.SHORT);
						}
					}
					else if (cur == 'V')
					{
						if (count != 2)
						{
							throw new IllegalArgumentException("Pattern letter count must be 2: " + cur);
						}
						AppendZoneId();
					}
					else if (cur == 'Z')
					{
						if (count < 4)
						{
							AppendOffset("+HHMM", "+0000");
						}
						else if (count == 4)
						{
							AppendLocalizedOffset(TextStyle.FULL);
						}
						else if (count == 5)
						{
							AppendOffset("+HH:MM:ss","Z");
						}
						else
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
					}
					else if (cur == 'O')
					{
						if (count == 1)
						{
							AppendLocalizedOffset(TextStyle.SHORT);
						}
						else if (count == 4)
						{
							AppendLocalizedOffset(TextStyle.FULL);
						}
						else
						{
							throw new IllegalArgumentException("Pattern letter count must be 1 or 4: " + cur);
						}
					}
					else if (cur == 'X')
					{
						if (count > 5)
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
						AppendOffset(OffsetIdPrinterParser.PATTERNS[count + (count == 1 ? 0 : 1)], "Z");
					}
					else if (cur == 'x')
					{
						if (count > 5)
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
						String zero = (count == 1 ? "+00" : (count % 2 == 0 ? "+0000" : "+00:00"));
						AppendOffset(OffsetIdPrinterParser.PATTERNS[count + (count == 1 ? 0 : 1)], zero);
					}
					else if (cur == 'W')
					{
						// Fields defined by Locale
						if (count > 1)
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
						AppendInternal(new WeekBasedFieldPrinterParser(cur, count));
					}
					else if (cur == 'w')
					{
						// Fields defined by Locale
						if (count > 2)
						{
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
						}
						AppendInternal(new WeekBasedFieldPrinterParser(cur, count));
					}
					else if (cur == 'Y')
					{
						// Fields defined by Locale
						AppendInternal(new WeekBasedFieldPrinterParser(cur, count));
					}
					else
					{
						throw new IllegalArgumentException("Unknown pattern letter: " + cur);
					}
					pos--;

				}
				else if (cur == '\'')
				{
					// parse literals
					int start = pos++;
					for (; pos < pattern.Length(); pos++)
					{
						if (pattern.CharAt(pos) == '\'')
						{
							if (pos + 1 < pattern.Length() && pattern.CharAt(pos + 1) == '\'')
							{
								pos++;
							}
							else
							{
								break; // end of literal
							}
						}
					}
					if (pos >= pattern.Length())
					{
						throw new IllegalArgumentException("Pattern ends with an incomplete string literal: " + pattern);
					}
					String str = StringHelperClass.SubstringSpecial(pattern, start + 1, pos);
					if (str.Length() == 0)
					{
						AppendLiteral('\'');
					}
					else
					{
						AppendLiteral(str.Replace("''", "'"));
					}

				}
				else if (cur == '[')
				{
					OptionalStart();

				}
				else if (cur == ']')
				{
					if (Active.Parent == null)
					{
						throw new IllegalArgumentException("Pattern invalid as it contains ] without previous [");
					}
					OptionalEnd();

				}
				else if (cur == '{' || cur == '}' || cur == '#')
				{
					throw new IllegalArgumentException("Pattern includes reserved character: '" + cur + "'");
				}
				else
				{
					AppendLiteral(cur);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private void parseField(char cur, int count, java.time.temporal.TemporalField field)
		private void ParseField(char cur, int count, TemporalField field)
		{
			bool standalone = false;
			switch (cur)
			{
				case 'u':
				case 'y':
					if (count == 2)
					{
						AppendValueReduced(field, 2, 2, ReducedPrinterParser.BASE_DATE);
					}
					else if (count < 4)
					{
						AppendValue(field, count, 19, SignStyle.NORMAL);
					}
					else
					{
						AppendValue(field, count, 19, SignStyle.EXCEEDS_PAD);
					}
					break;
				case 'c':
					if (count == 2)
					{
						throw new IllegalArgumentException("Invalid pattern \"cc\"");
					}
					/*fallthrough*/
				case 'L':
				case 'q':
					standalone = true;
					/*fallthrough*/
					goto case 'M';
				case 'M':
				case 'Q':
				case 'E':
				case 'e':
					switch (count)
					{
						case 1:
						case 2:
							if (cur == 'c' || cur == 'e')
							{
								AppendInternal(new WeekBasedFieldPrinterParser(cur, count));
							}
							else if (cur == 'E')
							{
								AppendText(field, TextStyle.SHORT);
							}
							else
							{
								if (count == 1)
								{
									AppendValue(field);
								}
								else
								{
									AppendValue(field, 2);
								}
							}
							break;
						case 3:
							AppendText(field, standalone ? TextStyle.SHORT_STANDALONE : TextStyle.SHORT);
							break;
						case 4:
							AppendText(field, standalone ? TextStyle.FULL_STANDALONE : TextStyle.FULL);
							break;
						case 5:
							AppendText(field, standalone ? TextStyle.NARROW_STANDALONE : TextStyle.NARROW);
							break;
						default:
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				case 'a':
					if (count == 1)
					{
						AppendText(field, TextStyle.SHORT);
					}
					else
					{
						throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				case 'G':
					switch (count)
					{
						case 1:
						case 2:
						case 3:
							AppendText(field, TextStyle.SHORT);
							break;
						case 4:
							AppendText(field, TextStyle.FULL);
							break;
						case 5:
							AppendText(field, TextStyle.NARROW);
							break;
						default:
							throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				case 'S':
					AppendFraction(NANO_OF_SECOND, count, count, false);
					break;
				case 'F':
					if (count == 1)
					{
						AppendValue(field);
					}
					else
					{
						throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				case 'd':
				case 'h':
				case 'H':
				case 'k':
				case 'K':
				case 'm':
				case 's':
					if (count == 1)
					{
						AppendValue(field);
					}
					else if (count == 2)
					{
						AppendValue(field, count);
					}
					else
					{
						throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				case 'D':
					if (count == 1)
					{
						AppendValue(field);
					}
					else if (count <= 3)
					{
						AppendValue(field, count);
					}
					else
					{
						throw new IllegalArgumentException("Too many pattern letters: " + cur);
					}
					break;
				default:
					if (count == 1)
					{
						AppendValue(field);
					}
					else
					{
						AppendValue(field, count);
					}
					break;
			}
		}

		/// <summary>
		/// Map of letters to fields. </summary>
		private static readonly IDictionary<Character, TemporalField> FIELD_MAP = new Dictionary<Character, TemporalField>();
		static DateTimeFormatterBuilder()
		{
			// SDF = SimpleDateFormat
			FIELD_MAP['G'] = ChronoField.ERA; // SDF, LDML (different to both for 1/2 chars)
			FIELD_MAP['y'] = ChronoField.YEAR_OF_ERA; // SDF, LDML
			FIELD_MAP['u'] = ChronoField.YEAR; // LDML (different in SDF)
			FIELD_MAP['Q'] = IsoFields.QUARTER_OF_YEAR; // LDML (removed quarter from 310)
			FIELD_MAP['q'] = IsoFields.QUARTER_OF_YEAR; // LDML (stand-alone)
			FIELD_MAP['M'] = ChronoField.MONTH_OF_YEAR; // SDF, LDML
			FIELD_MAP['L'] = ChronoField.MONTH_OF_YEAR; // SDF, LDML (stand-alone)
			FIELD_MAP['D'] = ChronoField.DAY_OF_YEAR; // SDF, LDML
			FIELD_MAP['d'] = ChronoField.DAY_OF_MONTH; // SDF, LDML
			FIELD_MAP['F'] = ChronoField.ALIGNED_DAY_OF_WEEK_IN_MONTH; // SDF, LDML
			FIELD_MAP['E'] = ChronoField.DAY_OF_WEEK; // SDF, LDML (different to both for 1/2 chars)
			FIELD_MAP['c'] = ChronoField.DAY_OF_WEEK; // LDML (stand-alone)
			FIELD_MAP['e'] = ChronoField.DAY_OF_WEEK; // LDML (needs localized week number)
			FIELD_MAP['a'] = ChronoField.AMPM_OF_DAY; // SDF, LDML
			FIELD_MAP['H'] = ChronoField.HOUR_OF_DAY; // SDF, LDML
			FIELD_MAP['k'] = ChronoField.CLOCK_HOUR_OF_DAY; // SDF, LDML
			FIELD_MAP['K'] = ChronoField.HOUR_OF_AMPM; // SDF, LDML
			FIELD_MAP['h'] = ChronoField.CLOCK_HOUR_OF_AMPM; // SDF, LDML
			FIELD_MAP['m'] = ChronoField.MINUTE_OF_HOUR; // SDF, LDML
			FIELD_MAP['s'] = ChronoField.SECOND_OF_MINUTE; // SDF, LDML
			FIELD_MAP['S'] = ChronoField.NANO_OF_SECOND; // LDML (SDF uses milli-of-second number)
			FIELD_MAP['A'] = ChronoField.MILLI_OF_DAY; // LDML
			FIELD_MAP['n'] = ChronoField.NANO_OF_SECOND; // 310 (proposed for LDML)
			FIELD_MAP['N'] = ChronoField.NANO_OF_DAY; // 310 (proposed for LDML)
			// 310 - z - time-zone names, matches LDML and SimpleDateFormat 1 to 4
			// 310 - Z - matches SimpleDateFormat and LDML
			// 310 - V - time-zone id, matches LDML
			// 310 - p - prefix for padding
			// 310 - X - matches LDML, almost matches SDF for 1, exact match 2&3, extended 4&5
			// 310 - x - matches LDML
			// 310 - w, W, and Y are localized forms matching LDML
			// LDML - U - cycle year name, not supported by 310 yet
			// LDML - l - deprecated
			// LDML - j - not relevant
			// LDML - g - modified-julian-day
			// LDML - v,V - extended time-zone names
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Causes the next added printer/parser to pad to a fixed width using a space.
		/// <para>
		/// This padding will pad to a fixed width using spaces.
		/// </para>
		/// <para>
		/// During formatting, the decorated element will be output and then padded
		/// to the specified width. An exception will be thrown during formatting if
		/// the pad width is exceeded.
		/// </para>
		/// <para>
		/// During parsing, the padding and decorated element are parsed.
		/// If parsing is lenient, then the pad width is treated as a maximum.
		/// The padding is parsed greedily. Thus, if the decorated element starts with
		/// the pad character, it will not be parsed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="padWidth">  the pad width, 1 or greater </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if pad width is too small </exception>
		public DateTimeFormatterBuilder PadNext(int padWidth)
		{
			return PadNext(padWidth, ' ');
		}

		/// <summary>
		/// Causes the next added printer/parser to pad to a fixed width.
		/// <para>
		/// This padding is intended for padding other than zero-padding.
		/// Zero-padding should be achieved using the appendValue methods.
		/// </para>
		/// <para>
		/// During formatting, the decorated element will be output and then padded
		/// to the specified width. An exception will be thrown during formatting if
		/// the pad width is exceeded.
		/// </para>
		/// <para>
		/// During parsing, the padding and decorated element are parsed.
		/// If parsing is lenient, then the pad width is treated as a maximum.
		/// If parsing is case insensitive, then the pad character is matched ignoring case.
		/// The padding is parsed greedily. Thus, if the decorated element starts with
		/// the pad character, it will not be parsed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="padWidth">  the pad width, 1 or greater </param>
		/// <param name="padChar">  the pad character </param>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalArgumentException"> if pad width is too small </exception>
		public DateTimeFormatterBuilder PadNext(int padWidth, char padChar)
		{
			if (padWidth < 1)
			{
				throw new IllegalArgumentException("The pad width must be at least one but was " + padWidth);
			}
			Active.PadNextWidth = padWidth;
			Active.PadNextChar = padChar;
			Active.ValueParserIndex = -1;
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Mark the start of an optional section.
		/// <para>
		/// The output of formatting can include optional sections, which may be nested.
		/// An optional section is started by calling this method and ended by calling
		/// <seealso cref="#optionalEnd()"/> or by ending the build process.
		/// </para>
		/// <para>
		/// All elements in the optional section are treated as optional.
		/// During formatting, the section is only output if data is available in the
		/// {@code TemporalAccessor} for all the elements in the section.
		/// During parsing, the whole section may be missing from the parsed string.
		/// </para>
		/// <para>
		/// For example, consider a builder setup as
		/// {@code builder.appendValue(HOUR_OF_DAY,2).optionalStart().appendValue(MINUTE_OF_HOUR,2)}.
		/// The optional section ends automatically at the end of the builder.
		/// During formatting, the minute will only be output if its value can be obtained from the date-time.
		/// During parsing, the input will be successfully parsed whether the minute is present or not.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder OptionalStart()
		{
			Active.ValueParserIndex = -1;
			Active = new DateTimeFormatterBuilder(Active, true);
			return this;
		}

		/// <summary>
		/// Ends an optional section.
		/// <para>
		/// The output of formatting can include optional sections, which may be nested.
		/// An optional section is started by calling <seealso cref="#optionalStart()"/> and ended
		/// using this method (or at the end of the builder).
		/// </para>
		/// <para>
		/// Calling this method without having previously called {@code optionalStart}
		/// will throw an exception.
		/// Calling this method immediately after calling {@code optionalStart} has no effect
		/// on the formatter other than ending the (empty) optional section.
		/// </para>
		/// <para>
		/// All elements in the optional section are treated as optional.
		/// During formatting, the section is only output if data is available in the
		/// {@code TemporalAccessor} for all the elements in the section.
		/// During parsing, the whole section may be missing from the parsed string.
		/// </para>
		/// <para>
		/// For example, consider a builder setup as
		/// {@code builder.appendValue(HOUR_OF_DAY,2).optionalStart().appendValue(MINUTE_OF_HOUR,2).optionalEnd()}.
		/// During formatting, the minute will only be output if its value can be obtained from the date-time.
		/// During parsing, the input will be successfully parsed whether the minute is present or not.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this, for chaining, not null </returns>
		/// <exception cref="IllegalStateException"> if there was no previous call to {@code optionalStart} </exception>
		public DateTimeFormatterBuilder OptionalEnd()
		{
			if (Active.Parent == null)
			{
				throw new IllegalStateException("Cannot call optionalEnd() as there was no previous call to optionalStart()");
			}
			if (Active.PrinterParsers.Count > 0)
			{
				CompositePrinterParser cpp = new CompositePrinterParser(Active.PrinterParsers, Active.Optional);
				Active = Active.Parent;
				AppendInternal(cpp);
			}
			else
			{
				Active = Active.Parent;
			}
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Appends a printer and/or parser to the internal list handling padding.
		/// </summary>
		/// <param name="pp">  the printer-parser to add, not null </param>
		/// <returns> the index into the active parsers list </returns>
		private int AppendInternal(DateTimePrinterParser pp)
		{
			Objects.RequireNonNull(pp, "pp");
			if (Active.PadNextWidth > 0)
			{
				if (pp != null)
				{
					pp = new PadPrinterParserDecorator(pp, Active.PadNextWidth, Active.PadNextChar);
				}
				Active.PadNextWidth = 0;
				Active.PadNextChar = (char)0;
			}
			Active.PrinterParsers.Add(pp);
			Active.ValueParserIndex = -1;
			return Active.PrinterParsers.Count - 1;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Completes this builder by creating the {@code DateTimeFormatter}
		/// using the default locale.
		/// <para>
		/// This will create a formatter with the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		/// Numbers will be printed and parsed using the standard DecimalStyle.
		/// The resolver style will be <seealso cref="ResolverStyle#SMART SMART"/>.
		/// </para>
		/// <para>
		/// Calling this method will end any open optional sections by repeatedly
		/// calling <seealso cref="#optionalEnd()"/> before creating the formatter.
		/// </para>
		/// <para>
		/// This builder can still be used after creating the formatter if desired,
		/// although the state may have been changed by calls to {@code optionalEnd}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the created formatter, not null </returns>
		public DateTimeFormatter ToFormatter()
		{
			return ToFormatter(Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Completes this builder by creating the {@code DateTimeFormatter}
		/// using the specified locale.
		/// <para>
		/// This will create a formatter with the specified locale.
		/// Numbers will be printed and parsed using the standard DecimalStyle.
		/// The resolver style will be <seealso cref="ResolverStyle#SMART SMART"/>.
		/// </para>
		/// <para>
		/// Calling this method will end any open optional sections by repeatedly
		/// calling <seealso cref="#optionalEnd()"/> before creating the formatter.
		/// </para>
		/// <para>
		/// This builder can still be used after creating the formatter if desired,
		/// although the state may have been changed by calls to {@code optionalEnd}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale">  the locale to use for formatting, not null </param>
		/// <returns> the created formatter, not null </returns>
		public DateTimeFormatter ToFormatter(Locale locale)
		{
			return ToFormatter(locale, ResolverStyle.SMART, null);
		}

		/// <summary>
		/// Completes this builder by creating the formatter.
		/// This uses the default locale.
		/// </summary>
		/// <param name="resolverStyle">  the resolver style to use, not null </param>
		/// <returns> the created formatter, not null </returns>
		internal DateTimeFormatter ToFormatter(ResolverStyle resolverStyle, Chronology chrono)
		{
			return ToFormatter(Locale.GetDefault(Locale.Category.FORMAT), resolverStyle, chrono);
		}

		/// <summary>
		/// Completes this builder by creating the formatter.
		/// </summary>
		/// <param name="locale">  the locale to use for formatting, not null </param>
		/// <param name="chrono">  the chronology to use, may be null </param>
		/// <returns> the created formatter, not null </returns>
		private DateTimeFormatter ToFormatter(Locale locale, ResolverStyle resolverStyle, Chronology chrono)
		{
			Objects.RequireNonNull(locale, "locale");
			while (Active.Parent != null)
			{
				OptionalEnd();
			}
			CompositePrinterParser pp = new CompositePrinterParser(PrinterParsers, false);
			return new DateTimeFormatter(pp, locale, DecimalStyle.STANDARD, resolverStyle, null, chrono, null);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Strategy for formatting/parsing date-time information.
		/// <para>
		/// The printer may format any part, or the whole, of the input date-time object.
		/// Typically, a complete format is constructed from a number of smaller
		/// units, each outputting a single field.
		/// </para>
		/// <para>
		/// The parser may parse any piece of text from the input, storing the result
		/// in the context. Typically, each individual parser will just parse one
		/// field, such as the day-of-month, storing the value in the context.
		/// Once the parse is complete, the caller will then resolve the parsed values
		/// to create the desired object, such as a {@code LocalDate}.
		/// </para>
		/// <para>
		/// The parse position will be updated during the parse. Parsing will start at
		/// the specified index and the return value specifies the new parse position
		/// for the next parser. If an error occurs, the returned index will be negative
		/// and will have the error position encoded using the complement operator.
		/// 
		/// @implSpec
		/// This interface must be implemented with care to ensure other classes operate correctly.
		/// All implementations that can be instantiated must be final, immutable and thread-safe.
		/// </para>
		/// <para>
		/// The context is not a thread-safe object and a new instance will be created
		/// for each format that occurs. The context must not be stored in an instance
		/// variable or shared with any other threads.
		/// </para>
		/// </summary>
		internal interface DateTimePrinterParser
		{

			/// <summary>
			/// Prints the date-time object to the buffer.
			/// <para>
			/// The context holds information to use during the format.
			/// It also contains the date-time information to be printed.
			/// </para>
			/// <para>
			/// The buffer must not be mutated beyond the content controlled by the implementation.
			/// 
			/// </para>
			/// </summary>
			/// <param name="context">  the context to format using, not null </param>
			/// <param name="buf">  the buffer to append to, not null </param>
			/// <returns> false if unable to query the value from the date-time, true otherwise </returns>
			/// <exception cref="DateTimeException"> if the date-time cannot be printed successfully </exception>
			bool Format(DateTimePrintContext context, StringBuilder buf);

			/// <summary>
			/// Parses text into date-time information.
			/// <para>
			/// The context holds information to use during the parse.
			/// It is also used to store the parsed date-time information.
			/// 
			/// </para>
			/// </summary>
			/// <param name="context">  the context to use and parse into, not null </param>
			/// <param name="text">  the input text to parse, not null </param>
			/// <param name="position">  the position to start parsing at, from 0 to the text length </param>
			/// <returns> the new parse position, where negative means an error with the
			///  error position encoded using the complement ~ operator </returns>
			/// <exception cref="NullPointerException"> if the context or text is null </exception>
			/// <exception cref="IndexOutOfBoundsException"> if the position is invalid </exception>
			int Parse(DateTimeParseContext context, CharSequence text, int position);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Composite printer and parser.
		/// </summary>
		internal sealed class CompositePrinterParser : DateTimePrinterParser
		{
			internal readonly DateTimePrinterParser[] PrinterParsers;
			internal readonly bool Optional;

			internal CompositePrinterParser(IList<DateTimePrinterParser> printerParsers, bool optional) : this(printerParsers.ToArray(), optional)
			{
			}

			internal CompositePrinterParser(DateTimePrinterParser[] printerParsers, bool optional)
			{
				this.PrinterParsers = printerParsers;
				this.Optional = optional;
			}

			/// <summary>
			/// Returns a copy of this printer-parser with the optional flag changed.
			/// </summary>
			/// <param name="optional">  the optional flag to set in the copy </param>
			/// <returns> the new printer-parser, not null </returns>
			public CompositePrinterParser WithOptional(bool optional)
			{
				if (optional == this.Optional)
				{
					return this;
				}
				return new CompositePrinterParser(PrinterParsers, optional);
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				int length = buf.Length();
				if (Optional)
				{
					context.StartOptional();
				}
				try
				{
					foreach (DateTimePrinterParser pp in PrinterParsers)
					{
						if (pp.Format(context, buf) == false)
						{
							buf.Length = length; // reset buffer
							return true;
						}
					}
				}
				finally
				{
					if (Optional)
					{
						context.EndOptional();
					}
				}
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				if (Optional)
				{
					context.StartOptional();
					int pos = position;
					foreach (DateTimePrinterParser pp in PrinterParsers)
					{
						pos = pp.Parse(context, text, pos);
						if (pos < 0)
						{
							context.EndOptional(false);
							return position; // return original position
						}
					}
					context.EndOptional(true);
					return pos;
				}
				else
				{
					foreach (DateTimePrinterParser pp in PrinterParsers)
					{
						position = pp.Parse(context, text, position);
						if (position < 0)
						{
							break;
						}
					}
					return position;
				}
			}

			public override String ToString()
			{
				StringBuilder buf = new StringBuilder();
				if (PrinterParsers != null)
				{
					buf.Append(Optional ? "[" : "(");
					foreach (DateTimePrinterParser pp in PrinterParsers)
					{
						buf.Append(pp);
					}
					buf.Append(Optional ? "]" : ")");
				}
				return buf.ToString();
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Pads the output to a fixed width.
		/// </summary>
		internal sealed class PadPrinterParserDecorator : DateTimePrinterParser
		{
			internal readonly DateTimePrinterParser PrinterParser;
			internal readonly int PadWidth;
			internal readonly char PadChar;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="printerParser">  the printer, not null </param>
			/// <param name="padWidth">  the width to pad to, 1 or greater </param>
			/// <param name="padChar">  the pad character </param>
			internal PadPrinterParserDecorator(DateTimePrinterParser printerParser, int padWidth, char padChar)
			{
				// input checked by DateTimeFormatterBuilder
				this.PrinterParser = printerParser;
				this.PadWidth = padWidth;
				this.PadChar = padChar;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				int preLen = buf.Length();
				if (PrinterParser.Format(context, buf) == false)
				{
					return false;
				}
				int len = buf.Length() - preLen;
				if (len > PadWidth)
				{
					throw new DateTimeException("Cannot print as output of " + len + " characters exceeds pad width of " + PadWidth);
				}
				for (int i = 0; i < PadWidth - len; i++)
				{
					buf.Insert(preLen, PadChar);
				}
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				// cache context before changed by decorated parser
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean strict = context.isStrict();
				bool strict = context.Strict;
				// parse
				if (position > text.Length())
				{
					throw new IndexOutOfBoundsException();
				}
				if (position == text.Length())
				{
					return ~position; // no more characters in the string
				}
				int endPos = position + PadWidth;
				if (endPos > text.Length())
				{
					if (strict)
					{
						return ~position; // not enough characters in the string to meet the parse width
					}
					endPos = text.Length();
				}
				int pos = position;
				while (pos < endPos && context.CharEquals(text.CharAt(pos), PadChar))
				{
					pos++;
				}
				text = text.SubSequence(0, endPos);
				int resultPos = PrinterParser.Parse(context, text, pos);
				if (resultPos != endPos && strict)
				{
					return ~(position + pos); // parse of decorated field didn't parse to the end
				}
				return resultPos;
			}

			public override String ToString()
			{
				return "Pad(" + PrinterParser + "," + PadWidth + (PadChar == ' ' ? ")" : ",'" + PadChar + "')");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Enumeration to apply simple parse settings.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: static enum SettingsParser implements DateTimePrinterParser
		internal enum SettingsParser
		{
			SENSITIVE,
			INSENSITIVE,
			STRICT,
			LENIENT

			public boolean format(DateTimePrintContext context, StringBuilder buf)
			{
				return true; // nothing to do here
			}

			public int parse(DateTimeParseContext context, CharSequence text, int position)
			{
				// using ordinals to avoid javac synthetic inner class
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				switch (ordinal())
				{
					case 0:
						context = true
						break
					case 1:
						context = false
						break
					case 2:
						context = true
						break
					case 3:
						context = false
						break
				}
				return position;
			}

			public String toString()
			{
				// using ordinals to avoid javac synthetic inner class
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
				switch (ordinal())
				{
					case 0:
						return "ParseCaseSensitive(true)";
					case 1:
						return "ParseCaseSensitive(false)";
					case 2:
						return "ParseStrict(true)";
					case 3:
						return "ParseStrict(false)";
				}
				throw new IllegalStateException("Unreachable");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Defaults a value into the parse if not currently present.
		/// </summary>
		internal class DefaultValueParser : DateTimePrinterParser
		{
			internal readonly TemporalField Field;
			internal readonly long Value;

			internal override DefaultValueParser(TemporalField field, long value)
			{
				this.Field = field;
				this.Value = value;
			}

			public virtual bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				return true;
			}

			public virtual int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				if (context.GetParsed(Field) == null)
				{
					context.SetParsedField(Field, Value, position, position);
				}
				return position;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a character literal.
		/// </summary>
		internal sealed class CharLiteralPrinterParser : DateTimePrinterParser
		{
			internal readonly char Literal;

			internal CharLiteralPrinterParser(char literal)
			{
				this.Literal = literal;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				buf.Append(Literal);
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int length = text.Length();
				if (position == length)
				{
					return ~position;
				}
				char ch = text.CharAt(position);
				if (ch != Literal)
				{
					if (context.CaseSensitive || (char.ToUpper(ch) != char.ToUpper(Literal) && char.ToLower(ch) != char.ToLower(Literal)))
					{
						return ~position;
					}
				}
				return position + 1;
			}

			public override String ToString()
			{
				if (Literal == '\'')
				{
					return "''";
				}
				return "'" + Literal + "'";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a string literal.
		/// </summary>
		internal sealed class StringLiteralPrinterParser : DateTimePrinterParser
		{
			internal readonly String Literal;

			internal StringLiteralPrinterParser(String literal)
			{
				this.Literal = literal; // validated by caller
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				buf.Append(Literal);
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int length = text.Length();
				if (position > length || position < 0)
				{
					throw new IndexOutOfBoundsException();
				}
				if (context.SubSequenceEquals(text, position, Literal, 0, Literal.Length()) == false)
				{
					return ~position;
				}
				return position + Literal.Length();
			}

			public override String ToString()
			{
				String converted = Literal.Replace("'", "''");
				return "'" + converted + "'";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints and parses a numeric date-time field with optional padding.
		/// </summary>
		internal class NumberPrinterParser : DateTimePrinterParser
		{

			/// <summary>
			/// Array of 10 to the power of n.
			/// </summary>
			internal static readonly long[] EXCEED_POINTS = new long[] {0L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 1000000000L, 10000000000L};

			internal readonly TemporalField Field;
			internal readonly int MinWidth;
			internal readonly int MaxWidth;
			internal readonly SignStyle SignStyle;
			internal readonly int SubsequentWidth;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="field">  the field to format, not null </param>
			/// <param name="minWidth">  the minimum field width, from 1 to 19 </param>
			/// <param name="maxWidth">  the maximum field width, from minWidth to 19 </param>
			/// <param name="signStyle">  the positive/negative sign style, not null </param>
			internal NumberPrinterParser(TemporalField field, int minWidth, int maxWidth, SignStyle signStyle)
			{
				// validated by caller
				this.Field = field;
				this.MinWidth = minWidth;
				this.MaxWidth = maxWidth;
				this.SignStyle = signStyle;
				this.SubsequentWidth = 0;
			}

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="field">  the field to format, not null </param>
			/// <param name="minWidth">  the minimum field width, from 1 to 19 </param>
			/// <param name="maxWidth">  the maximum field width, from minWidth to 19 </param>
			/// <param name="signStyle">  the positive/negative sign style, not null </param>
			/// <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater,
			///  -1 if fixed width due to active adjacent parsing </param>
			protected internal NumberPrinterParser(TemporalField field, int minWidth, int maxWidth, SignStyle signStyle, int subsequentWidth)
			{
				// validated by caller
				this.Field = field;
				this.MinWidth = minWidth;
				this.MaxWidth = maxWidth;
				this.SignStyle = signStyle;
				this.SubsequentWidth = subsequentWidth;
			}

			/// <summary>
			/// Returns a new instance with fixed width flag set.
			/// </summary>
			/// <returns> a new updated printer-parser, not null </returns>
			internal virtual NumberPrinterParser WithFixedWidth()
			{
				if (SubsequentWidth == -1)
				{
					return this;
				}
				return new NumberPrinterParser(Field, MinWidth, MaxWidth, SignStyle, -1);
			}

			/// <summary>
			/// Returns a new instance with an updated subsequent width.
			/// </summary>
			/// <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater </param>
			/// <returns> a new updated printer-parser, not null </returns>
			internal virtual NumberPrinterParser WithSubsequentWidth(int subsequentWidth)
			{
				return new NumberPrinterParser(Field, MinWidth, MaxWidth, SignStyle, this.SubsequentWidth + subsequentWidth);
			}

			public virtual bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Long valueLong = context.GetValue(Field);
				if (valueLong == null)
				{
					return false;
				}
				long value = GetValue(context, valueLong);
				DecimalStyle decimalStyle = context.DecimalStyle;
				String str = (value == Long.MinValue ? "9223372036854775808" : Convert.ToString(System.Math.Abs(value)));
				if (str.Length() > MaxWidth)
				{
					throw new DateTimeException("Field " + Field + " cannot be printed as the value " + value + " exceeds the maximum print width of " + MaxWidth);
				}
				str = decimalStyle.ConvertNumberToI18N(str);

				if (value >= 0)
				{
					switch (SignStyle.InnerEnumValue())
					{
						case java.time.format.SignStyle.InnerEnum.EXCEEDS_PAD:
							if (MinWidth < 19 && value >= EXCEED_POINTS[MinWidth])
							{
								buf.Append(decimalStyle.PositiveSign);
							}
							break;
						case java.time.format.SignStyle.InnerEnum.ALWAYS:
							buf.Append(decimalStyle.PositiveSign);
							break;
					}
				}
				else
				{
					switch (SignStyle.InnerEnumValue())
					{
						case java.time.format.SignStyle.InnerEnum.NORMAL:
						case java.time.format.SignStyle.InnerEnum.EXCEEDS_PAD:
						case java.time.format.SignStyle.InnerEnum.ALWAYS:
							buf.Append(decimalStyle.NegativeSign);
							break;
						case java.time.format.SignStyle.InnerEnum.NOT_NEGATIVE:
							throw new DateTimeException("Field " + Field + " cannot be printed as the value " + value + " cannot be negative according to the SignStyle");
					}
				}
				for (int i = 0; i < MinWidth - str.Length(); i++)
				{
					buf.Append(decimalStyle.ZeroDigit);
				}
				buf.Append(str);
				return true;
			}

			/// <summary>
			/// Gets the value to output.
			/// </summary>
			/// <param name="context">  the context </param>
			/// <param name="value">  the value of the field, not null </param>
			/// <returns> the value </returns>
			internal virtual long GetValue(DateTimePrintContext context, long value)
			{
				return value;
			}

			/// <summary>
			/// For NumberPrinterParser, the width is fixed depending on the
			/// minWidth, maxWidth, signStyle and whether subsequent fields are fixed. </summary>
			/// <param name="context"> the context </param>
			/// <returns> true if the field is fixed width </returns>
			/// <seealso cref= DateTimeFormatterBuilder#appendValue(java.time.temporal.TemporalField, int) </seealso>
			internal virtual bool IsFixedWidth(DateTimeParseContext context)
			{
				return SubsequentWidth == -1 || (SubsequentWidth > 0 && MinWidth == MaxWidth && SignStyle == SignStyle.NOT_NEGATIVE);
			}

			public virtual int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int length = text.Length();
				if (position == length)
				{
					return ~position;
				}
				char sign = text.CharAt(position); // IOOBE if invalid position
				bool negative = false;
				bool positive = false;
				if (sign == context.DecimalStyle.PositiveSign)
				{
					if (SignStyle.parse(true, context.Strict, MinWidth == MaxWidth) == false)
					{
						return ~position;
					}
					positive = true;
					position++;
				}
				else if (sign == context.DecimalStyle.NegativeSign)
				{
					if (SignStyle.parse(false, context.Strict, MinWidth == MaxWidth) == false)
					{
						return ~position;
					}
					negative = true;
					position++;
				}
				else
				{
					if (SignStyle == SignStyle.ALWAYS && context.Strict)
					{
						return ~position;
					}
				}
				int effMinWidth = (context.Strict || IsFixedWidth(context) ? MinWidth : 1);
				int minEndPos = position + effMinWidth;
				if (minEndPos > length)
				{
					return ~position;
				}
				int effMaxWidth = (context.Strict || IsFixedWidth(context) ? MaxWidth : 9) + System.Math.Max(SubsequentWidth, 0);
				long total = 0;
				System.Numerics.BigInteger totalBig = null;
				int pos = position;
				for (int pass = 0; pass < 2; pass++)
				{
					int maxEndPos = System.Math.Min(pos + effMaxWidth, length);
					while (pos < maxEndPos)
					{
						char ch = text.CharAt(pos++);
						int digit = context.DecimalStyle.ConvertToDigit(ch);
						if (digit < 0)
						{
							pos--;
							if (pos < minEndPos)
							{
								return ~position; // need at least min width digits
							}
							break;
						}
						if ((pos - position) > 18)
						{
							if (totalBig == null)
							{
								totalBig = System.Numerics.BigInteger.ValueOf(total);
							}
							totalBig = totalBig * System.Numerics.BigInteger.TEN.Add(System.Numerics.BigInteger.ValueOf(digit));
						}
						else
						{
							total = total * 10 + digit;
						}
					}
					if (SubsequentWidth > 0 && pass == 0)
					{
						// re-parse now we know the correct width
						int parseLen = pos - position;
						effMaxWidth = System.Math.Max(effMinWidth, parseLen - SubsequentWidth);
						pos = position;
						total = 0;
						totalBig = null;
					}
					else
					{
						break;
					}
				}
				if (negative)
				{
					if (totalBig != null)
					{
						if (totalBig.Equals(System.Numerics.BigInteger.ZERO) && context.Strict)
						{
							return ~(position - 1); // minus zero not allowed
						}
						totalBig = -totalBig;
					}
					else
					{
						if (total == 0 && context.Strict)
						{
							return ~(position - 1); // minus zero not allowed
						}
						total = -total;
					}
				}
				else if (SignStyle == SignStyle.EXCEEDS_PAD && context.Strict)
				{
					int parseLen = pos - position;
					if (positive)
					{
						if (parseLen <= MinWidth)
						{
							return ~(position - 1); // '+' only parsed if minWidth exceeded
						}
					}
					else
					{
						if (parseLen > MinWidth)
						{
							return ~position; // '+' must be parsed if minWidth exceeded
						}
					}
				}
				if (totalBig != null)
				{
					if (totalBig.bitLength() > 63)
					{
						// overflow, parse 1 less digit
						totalBig = totalBig / System.Numerics.BigInteger.TEN;
						pos--;
					}
					return SetValue(context, (long)totalBig, position, pos);
				}
				return SetValue(context, total, position, pos);
			}

			/// <summary>
			/// Stores the value.
			/// </summary>
			/// <param name="context">  the context to store into, not null </param>
			/// <param name="value">  the value </param>
			/// <param name="errorPos">  the position of the field being parsed </param>
			/// <param name="successPos">  the position after the field being parsed </param>
			/// <returns> the new position </returns>
			internal virtual int SetValue(DateTimeParseContext context, long value, int errorPos, int successPos)
			{
				return context.SetParsedField(Field, value, errorPos, successPos);
			}

			public override String ToString()
			{
				if (MinWidth == 1 && MaxWidth == 19 && SignStyle == SignStyle.NORMAL)
				{
					return "Value(" + Field + ")";
				}
				if (MinWidth == MaxWidth && SignStyle == SignStyle.NOT_NEGATIVE)
				{
					return "Value(" + Field + "," + MinWidth + ")";
				}
				return "Value(" + Field + "," + MinWidth + "," + MaxWidth + "," + SignStyle + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints and parses a reduced numeric date-time field.
		/// </summary>
		internal sealed class ReducedPrinterParser : NumberPrinterParser
		{
			/// <summary>
			/// The base date for reduced value parsing.
			/// </summary>
			internal static readonly LocalDate BASE_DATE = LocalDate.Of(2000, 1, 1);

			internal readonly int BaseValue;
			internal readonly ChronoLocalDate BaseDate;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="field">  the field to format, validated not null </param>
			/// <param name="minWidth">  the minimum field width, from 1 to 10 </param>
			/// <param name="maxWidth">  the maximum field width, from 1 to 10 </param>
			/// <param name="baseValue">  the base value </param>
			/// <param name="baseDate">  the base date </param>
			internal ReducedPrinterParser(TemporalField field, int minWidth, int maxWidth, int baseValue, ChronoLocalDate baseDate) : this(field, minWidth, maxWidth, baseValue, baseDate, 0)
			{
				if (minWidth < 1 || minWidth > 10)
				{
					throw new IllegalArgumentException("The minWidth must be from 1 to 10 inclusive but was " + minWidth);
				}
				if (maxWidth < 1 || maxWidth > 10)
				{
					throw new IllegalArgumentException("The maxWidth must be from 1 to 10 inclusive but was " + minWidth);
				}
				if (maxWidth < minWidth)
				{
					throw new IllegalArgumentException("Maximum width must exceed or equal the minimum width but " + maxWidth + " < " + minWidth);
				}
				if (baseDate == null)
				{
					if (field.Range().IsValidValue(baseValue) == false)
					{
						throw new IllegalArgumentException("The base value must be within the range of the field");
					}
					if ((((long) baseValue) + EXCEED_POINTS[maxWidth]) > Integer.MaxValue)
					{
						throw new DateTimeException("Unable to add printer-parser as the range exceeds the capacity of an int");
					}
				}
			}

			/// <summary>
			/// Constructor.
			/// The arguments have already been checked.
			/// </summary>
			/// <param name="field">  the field to format, validated not null </param>
			/// <param name="minWidth">  the minimum field width, from 1 to 10 </param>
			/// <param name="maxWidth">  the maximum field width, from 1 to 10 </param>
			/// <param name="baseValue">  the base value </param>
			/// <param name="baseDate">  the base date </param>
			/// <param name="subsequentWidth"> the subsequentWidth for this instance </param>
			internal ReducedPrinterParser(TemporalField field, int minWidth, int maxWidth, int baseValue, ChronoLocalDate baseDate, int subsequentWidth) : base(field, minWidth, maxWidth, SignStyle.NOT_NEGATIVE, subsequentWidth)
			{
				this.BaseValue = baseValue;
				this.BaseDate = baseDate;
			}

			internal override long GetValue(DateTimePrintContext context, long value)
			{
				long absValue = System.Math.Abs(value);
				int baseValue = this.BaseValue;
				if (BaseDate != null)
				{
					Chronology chrono = Chronology.from(context.Temporal);
					baseValue = chrono.Date(BaseDate).get(Field);
				}
				if (value >= baseValue && value < baseValue + EXCEED_POINTS[MinWidth])
				{
					// Use the reduced value if it fits in minWidth
					return absValue % EXCEED_POINTS[MinWidth];
				}
				// Otherwise truncate to fit in maxWidth
				return absValue % EXCEED_POINTS[MaxWidth];
			}

			internal override int SetValue(DateTimeParseContext context, long value, int errorPos, int successPos)
			{
				int baseValue = this.BaseValue;
				if (BaseDate != null)
				{
					Chronology chrono = context.EffectiveChronology;
					baseValue = chrono.Date(BaseDate).get(Field);

					// In case the Chronology is changed later, add a callback when/if it changes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long initialValue = value;
					long initialValue = value;
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					context.addChronoChangedListener((_unused) =>
								/* Repeat the set of the field using the current Chronology
								 * The success/error position is ignored because the value is
								 * intentionally being overwritten.
								 */
					{
						SetValue(context, initialValue, errorPos, successPos);
					});
				}
				int parseLen = successPos - errorPos;
				if (parseLen == MinWidth && value >= 0)
				{
					long range = EXCEED_POINTS[MinWidth];
					long lastPart = baseValue % range;
					long basePart = baseValue - lastPart;
					if (baseValue > 0)
					{
						value = basePart + value;
					}
					else
					{
						value = basePart - value;
					}
					if (value < baseValue)
					{
						value += range;
					}
				}
				return context.SetParsedField(Field, value, errorPos, successPos);
			}

			/// <summary>
			/// Returns a new instance with fixed width flag set.
			/// </summary>
			/// <returns> a new updated printer-parser, not null </returns>
			internal override ReducedPrinterParser WithFixedWidth()
			{
				if (SubsequentWidth == -1)
				{
					return this;
				}
				return new ReducedPrinterParser(Field, MinWidth, MaxWidth, BaseValue, BaseDate, -1);
			}

			/// <summary>
			/// Returns a new instance with an updated subsequent width.
			/// </summary>
			/// <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater </param>
			/// <returns> a new updated printer-parser, not null </returns>
			internal override ReducedPrinterParser WithSubsequentWidth(int subsequentWidth)
			{
				return new ReducedPrinterParser(Field, MinWidth, MaxWidth, BaseValue, BaseDate, this.SubsequentWidth + subsequentWidth);
			}

			/// <summary>
			/// For a ReducedPrinterParser, fixed width is false if the mode is strict,
			/// otherwise it is set as for NumberPrinterParser. </summary>
			/// <param name="context"> the context </param>
			/// <returns> if the field is fixed width </returns>
			/// <seealso cref= DateTimeFormatterBuilder#appendValueReduced(java.time.temporal.TemporalField, int, int, int) </seealso>
			internal override bool IsFixedWidth(DateTimeParseContext context)
			{
			   if (context.Strict == false)
			   {
				   return false;
			   }
			   return base.IsFixedWidth(context);
			}

			public override String ToString()
			{
				return "ReducedValue(" + Field + "," + MinWidth + "," + MaxWidth + "," + (BaseDate != null ? BaseDate : BaseValue) + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints and parses a numeric date-time field with optional padding.
		/// </summary>
		internal sealed class FractionPrinterParser : DateTimePrinterParser
		{
			internal readonly TemporalField Field;
			internal readonly int MinWidth;
			internal readonly int MaxWidth;
			internal readonly bool DecimalPoint;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="field">  the field to output, not null </param>
			/// <param name="minWidth">  the minimum width to output, from 0 to 9 </param>
			/// <param name="maxWidth">  the maximum width to output, from 0 to 9 </param>
			/// <param name="decimalPoint">  whether to output the localized decimal point symbol </param>
			internal FractionPrinterParser(TemporalField field, int minWidth, int maxWidth, bool decimalPoint)
			{
				Objects.RequireNonNull(field, "field");
				if (field.Range().Fixed == false)
				{
					throw new IllegalArgumentException("Field must have a fixed set of values: " + field);
				}
				if (minWidth < 0 || minWidth > 9)
				{
					throw new IllegalArgumentException("Minimum width must be from 0 to 9 inclusive but was " + minWidth);
				}
				if (maxWidth < 1 || maxWidth > 9)
				{
					throw new IllegalArgumentException("Maximum width must be from 1 to 9 inclusive but was " + maxWidth);
				}
				if (maxWidth < minWidth)
				{
					throw new IllegalArgumentException("Maximum width must exceed or equal the minimum width but " + maxWidth + " < " + minWidth);
				}
				this.Field = field;
				this.MinWidth = minWidth;
				this.MaxWidth = maxWidth;
				this.DecimalPoint = decimalPoint;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Long value = context.GetValue(Field);
				if (value == null)
				{
					return false;
				}
				DecimalStyle decimalStyle = context.DecimalStyle;
				decimal fraction = ConvertToFraction(value);
				if (fraction.Scale() == 0) // scale is zero if value is zero
				{
					if (MinWidth > 0)
					{
						if (DecimalPoint)
						{
							buf.Append(decimalStyle.DecimalSeparator);
						}
						for (int i = 0; i < MinWidth; i++)
						{
							buf.Append(decimalStyle.ZeroDigit);
						}
					}
				}
				else
				{
					int outputScale = System.Math.Min(System.Math.Max(fraction.Scale(), MinWidth), MaxWidth);
					fraction = fraction.SetScale(outputScale, RoundingMode.FLOOR);
					String str = fraction.ToPlainString().Substring(2);
					str = decimalStyle.ConvertNumberToI18N(str);
					if (DecimalPoint)
					{
						buf.Append(decimalStyle.DecimalSeparator);
					}
					buf.Append(str);
				}
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int effectiveMin = (context.Strict ? MinWidth : 0);
				int effectiveMax = (context.Strict ? MaxWidth : 9);
				int length = text.Length();
				if (position == length)
				{
					// valid if whole field is optional, invalid if minimum width
					return (effectiveMin > 0 ?~position : position);
				}
				if (DecimalPoint)
				{
					if (text.CharAt(position) != context.DecimalStyle.DecimalSeparator)
					{
						// valid if whole field is optional, invalid if minimum width
						return (effectiveMin > 0 ?~position : position);
					}
					position++;
				}
				int minEndPos = position + effectiveMin;
				if (minEndPos > length)
				{
					return ~position; // need at least min width digits
				}
				int maxEndPos = System.Math.Min(position + effectiveMax, length);
				int total = 0; // can use int because we are only parsing up to 9 digits
				int pos = position;
				while (pos < maxEndPos)
				{
					char ch = text.CharAt(pos++);
					int digit = context.DecimalStyle.ConvertToDigit(ch);
					if (digit < 0)
					{
						if (pos < minEndPos)
						{
							return ~position; // need at least min width digits
						}
						pos--;
						break;
					}
					total = total * 10 + digit;
				}
				decimal fraction = (new decimal(total)).MovePointLeft(pos - position);
				long value = ConvertFromFraction(fraction);
				return context.SetParsedField(Field, value, position, pos);
			}

			/// <summary>
			/// Converts a value for this field to a fraction between 0 and 1.
			/// <para>
			/// The fractional value is between 0 (inclusive) and 1 (exclusive).
			/// It can only be returned if the <seealso cref="java.time.temporal.TemporalField#range() value range"/> is fixed.
			/// The fraction is obtained by calculation from the field range using 9 decimal
			/// places and a rounding mode of <seealso cref="RoundingMode#FLOOR FLOOR"/>.
			/// The calculation is inaccurate if the values do not run continuously from smallest to largest.
			/// </para>
			/// <para>
			/// For example, the second-of-minute value of 15 would be returned as 0.25,
			/// assuming the standard definition of 60 seconds in a minute.
			/// 
			/// </para>
			/// </summary>
			/// <param name="value">  the value to convert, must be valid for this rule </param>
			/// <returns> the value as a fraction within the range, from 0 to 1, not null </returns>
			/// <exception cref="DateTimeException"> if the value cannot be converted to a fraction </exception>
			internal decimal ConvertToFraction(long value)
			{
				ValueRange range = Field.Range();
				range.CheckValidValue(value, Field);
				decimal minBD = decimal.ValueOf(range.Minimum);
				decimal rangeBD = decimal.ValueOf(range.Maximum) - minBD + decimal.One;
				decimal valueBD = decimal.ValueOf(value) - minBD;
				decimal fraction = valueBD.Divide(rangeBD, 9, RoundingMode.FLOOR);
				// stripTrailingZeros bug
				return fraction.CompareTo(decimal.Zero) == 0 ? decimal.Zero : fraction.StripTrailingZeros();
			}

			/// <summary>
			/// Converts a fraction from 0 to 1 for this field to a value.
			/// <para>
			/// The fractional value must be between 0 (inclusive) and 1 (exclusive).
			/// It can only be returned if the <seealso cref="java.time.temporal.TemporalField#range() value range"/> is fixed.
			/// The value is obtained by calculation from the field range and a rounding
			/// mode of <seealso cref="RoundingMode#FLOOR FLOOR"/>.
			/// The calculation is inaccurate if the values do not run continuously from smallest to largest.
			/// </para>
			/// <para>
			/// For example, the fractional second-of-minute of 0.25 would be converted to 15,
			/// assuming the standard definition of 60 seconds in a minute.
			/// 
			/// </para>
			/// </summary>
			/// <param name="fraction">  the fraction to convert, not null </param>
			/// <returns> the value of the field, valid for this rule </returns>
			/// <exception cref="DateTimeException"> if the value cannot be converted </exception>
			internal long ConvertFromFraction(decimal fraction)
			{
				ValueRange range = Field.Range();
				decimal minBD = decimal.ValueOf(range.Minimum);
				decimal rangeBD = decimal.ValueOf(range.Maximum) - minBD + decimal.One;
				decimal valueBD = fraction * rangeBD.SetScale(0, RoundingMode.FLOOR).Add(minBD);
				return valueBD.LongValueExact();
			}

			public override String ToString()
			{
				String @decimal = (DecimalPoint ? ",DecimalPoint" : "");
				return "Fraction(" + Field + "," + MinWidth + "," + MaxWidth + @decimal + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses field text.
		/// </summary>
		internal sealed class TextPrinterParser : DateTimePrinterParser
		{
			internal readonly TemporalField Field;
			internal readonly TextStyle TextStyle;
			internal readonly DateTimeTextProvider Provider;
			/// <summary>
			/// The cached number printer parser.
			/// Immutable and volatile, so no synchronization needed.
			/// </summary>
			internal volatile NumberPrinterParser NumberPrinterParser_Renamed;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="field">  the field to output, not null </param>
			/// <param name="textStyle">  the text style, not null </param>
			/// <param name="provider">  the text provider, not null </param>
			internal TextPrinterParser(TemporalField field, TextStyle textStyle, DateTimeTextProvider provider)
			{
				// validated by caller
				this.Field = field;
				this.TextStyle = textStyle;
				this.Provider = provider;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Long value = context.GetValue(Field);
				if (value == null)
				{
					return false;
				}
				String text;
				Chronology chrono = context.Temporal.query(TemporalQueries.Chronology());
				if (chrono == null || chrono == IsoChronology.INSTANCE)
				{
					text = Provider.GetText(Field, value, TextStyle, context.Locale);
				}
				else
				{
					text = Provider.GetText(chrono, Field, value, TextStyle, context.Locale);
				}
				if (text == null)
				{
					return NumberPrinterParser().Format(context, buf);
				}
				buf.Append(text);
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence parseText, int position)
			{
				int length = parseText.Length();
				if (position < 0 || position > length)
				{
					throw new IndexOutOfBoundsException();
				}
				TextStyle style = (context.Strict ? TextStyle : null);
				Chronology chrono = context.EffectiveChronology;
				IEnumerator<Map_Entry<String, Long>> it;
				if (chrono == null || chrono == IsoChronology.INSTANCE)
				{
					it = Provider.GetTextIterator(Field, style, context.Locale);
				}
				else
				{
					it = Provider.GetTextIterator(chrono, Field, style, context.Locale);
				}
				if (it != null)
				{
					while (it.MoveNext())
					{
						Map_Entry<String, Long> entry = it.Current;
						String itText = entry.Key;
						if (context.SubSequenceEquals(itText, 0, parseText, position, itText.Length()))
						{
							return context.SetParsedField(Field, entry.Value, position, position + itText.Length());
						}
					}
					if (context.Strict)
					{
						return ~position;
					}
				}
				return NumberPrinterParser().Parse(context, parseText, position);
			}

			/// <summary>
			/// Create and cache a number printer parser. </summary>
			/// <returns> the number printer parser for this field, not null </returns>
			internal NumberPrinterParser NumberPrinterParser()
			{
				if (NumberPrinterParser_Renamed == null)
				{
					NumberPrinterParser_Renamed = new NumberPrinterParser(Field, 1, 19, SignStyle.NORMAL);
				}
				return NumberPrinterParser_Renamed;
			}

			public override String ToString()
			{
				if (TextStyle == TextStyle.FULL)
				{
					return "Text(" + Field + ")";
				}
				return "Text(" + Field + "," + TextStyle + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses an ISO-8601 instant.
		/// </summary>
		internal sealed class InstantPrinterParser : DateTimePrinterParser
		{
			// days in a 400 year cycle = 146097
			// days in a 10,000 year cycle = 146097 * 25
			// seconds per day = 86400
			internal static readonly long SECONDS_PER_10000_YEARS = 146097L * 25L * 86400L;
			internal static readonly long SECONDS_0000_TO_1970 = ((146097L * 5L) - (30L * 365L + 7L)) * 86400L;
			internal readonly int FractionalDigits;

			internal InstantPrinterParser(int fractionalDigits)
			{
				this.FractionalDigits = fractionalDigits;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				// use INSTANT_SECONDS, thus this code is not bound by Instant.MAX
				Long inSecs = context.GetValue(INSTANT_SECONDS);
				Long inNanos = null;
				if (context.Temporal.IsSupported(NANO_OF_SECOND))
				{
					inNanos = context.Temporal.GetLong(NANO_OF_SECOND);
				}
				if (inSecs == null)
				{
					return false;
				}
				long inSec = inSecs;
				int inNano = NANO_OF_SECOND.checkValidIntValue(inNanos != null ? inNanos : 0);
				// format mostly using LocalDateTime.toString
				if (inSec >= -SECONDS_0000_TO_1970)
				{
					// current era
					long zeroSecs = inSec - SECONDS_PER_10000_YEARS + SECONDS_0000_TO_1970;
					long hi = Math.FloorDiv(zeroSecs, SECONDS_PER_10000_YEARS) + 1;
					long lo = Math.FloorMod(zeroSecs, SECONDS_PER_10000_YEARS);
					LocalDateTime ldt = LocalDateTime.OfEpochSecond(lo - SECONDS_0000_TO_1970, 0, ZoneOffset.UTC);
					if (hi > 0)
					{
						buf.Append('+').Append(hi);
					}
					buf.Append(ldt);
					if (ldt.Second == 0)
					{
						buf.Append(":00");
					}
				}
				else
				{
					// before current era
					long zeroSecs = inSec + SECONDS_0000_TO_1970;
					long hi = zeroSecs / SECONDS_PER_10000_YEARS;
					long lo = zeroSecs % SECONDS_PER_10000_YEARS;
					LocalDateTime ldt = LocalDateTime.OfEpochSecond(lo - SECONDS_0000_TO_1970, 0, ZoneOffset.UTC);
					int pos = buf.Length();
					buf.Append(ldt);
					if (ldt.Second == 0)
					{
						buf.Append(":00");
					}
					if (hi < 0)
					{
						if (ldt.Year == -10000)
						{
							buf.Replace(pos, pos + 2, Convert.ToString(hi - 1));
						}
						else if (lo == 0)
						{
							buf.Insert(pos, hi);
						}
						else
						{
							buf.Insert(pos + 1, System.Math.Abs(hi));
						}
					}
				}
				// add fraction
				if ((FractionalDigits < 0 && inNano > 0) || FractionalDigits > 0)
				{
					buf.Append('.');
					int div = 100000000;
					for (int i = 0; ((FractionalDigits == -1 && inNano > 0) || (FractionalDigits == -2 && (inNano > 0 || (i % 3) != 0)) || i < FractionalDigits); i++)
					{
						int digit = inNano / div;
						buf.Append((char)(digit + '0'));
						inNano = inNano - (digit * div);
						div = div / 10;
					}
				}
				buf.Append('Z');
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				// new context to avoid overwriting fields like year/month/day
				int minDigits = (FractionalDigits < 0 ? 0 : FractionalDigits);
				int maxDigits = (FractionalDigits < 0 ? 9 : FractionalDigits);
				CompositePrinterParser parser = (new DateTimeFormatterBuilder()).Append(DateTimeFormatter.ISO_LOCAL_DATE).AppendLiteral('T').AppendValue(HOUR_OF_DAY, 2).AppendLiteral(':').AppendValue(MINUTE_OF_HOUR, 2).AppendLiteral(':').AppendValue(SECOND_OF_MINUTE, 2).AppendFraction(NANO_OF_SECOND, minDigits, maxDigits, true).AppendLiteral('Z').ToFormatter().ToPrinterParser(false);
				DateTimeParseContext newContext = context.Copy();
				int pos = parser.Parse(newContext, text, position);
				if (pos < 0)
				{
					return pos;
				}
				// parser restricts most fields to 2 digits, so definitely int
				// correctly parsed nano is also guaranteed to be valid
				long yearParsed = newContext.GetParsed(YEAR);
				int month = newContext.GetParsed(MONTH_OF_YEAR).IntValue();
				int day = newContext.GetParsed(DAY_OF_MONTH).IntValue();
				int hour = newContext.GetParsed(HOUR_OF_DAY).IntValue();
				int min = newContext.GetParsed(MINUTE_OF_HOUR).IntValue();
				Long secVal = newContext.GetParsed(SECOND_OF_MINUTE);
				Long nanoVal = newContext.GetParsed(NANO_OF_SECOND);
				int sec = (secVal != null ? secVal.IntValue() : 0);
				int nano = (nanoVal != null ? nanoVal.IntValue() : 0);
				int days = 0;
				if (hour == 24 && min == 0 && sec == 0 && nano == 0)
				{
					hour = 0;
					days = 1;
				}
				else if (hour == 23 && min == 59 && sec == 60)
				{
					context.SetParsedLeapSecond();
					sec = 59;
				}
				int year = (int) yearParsed % 10000;
				long instantSecs;
				try
				{
					LocalDateTime ldt = LocalDateTime.Of(year, month, day, hour, min, sec, 0).PlusDays(days);
					instantSecs = ldt.toEpochSecond(ZoneOffset.UTC);
					instantSecs += Math.MultiplyExact(yearParsed / 10_000L, SECONDS_PER_10000_YEARS);
				}
				catch (RuntimeException)
				{
					return ~position;
				}
				int successPos = pos;
				successPos = context.SetParsedField(INSTANT_SECONDS, instantSecs, position, successPos);
				return context.SetParsedField(NANO_OF_SECOND, nano, position, successPos);
			}

			public override String ToString()
			{
				return "Instant()";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses an offset ID.
		/// </summary>
		internal sealed class OffsetIdPrinterParser : DateTimePrinterParser
		{
			internal static readonly String[] PATTERNS = new String[] {"+HH", "+HHmm", "+HH:mm", "+HHMM", "+HH:MM", "+HHMMss", "+HH:MM:ss", "+HHMMSS", "+HH:MM:SS"};
			internal static readonly OffsetIdPrinterParser INSTANCE_ID_Z = new OffsetIdPrinterParser("+HH:MM:ss", "Z");
			internal static readonly OffsetIdPrinterParser INSTANCE_ID_ZERO = new OffsetIdPrinterParser("+HH:MM:ss", "0");

			internal readonly String NoOffsetText;
			internal readonly int Type;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="pattern">  the pattern </param>
			/// <param name="noOffsetText">  the text to use for UTC, not null </param>
			internal OffsetIdPrinterParser(String pattern, String noOffsetText)
			{
				Objects.RequireNonNull(pattern, "pattern");
				Objects.RequireNonNull(noOffsetText, "noOffsetText");
				this.Type = CheckPattern(pattern);
				this.NoOffsetText = noOffsetText;
			}

			internal int CheckPattern(String pattern)
			{
				for (int i = 0; i < PATTERNS.Length; i++)
				{
					if (PATTERNS[i].Equals(pattern))
					{
						return i;
					}
				}
				throw new IllegalArgumentException("Invalid zone offset pattern: " + pattern);
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Long offsetSecs = context.GetValue(OFFSET_SECONDS);
				if (offsetSecs == null)
				{
					return false;
				}
				int totalSecs = Math.ToIntExact(offsetSecs);
				if (totalSecs == 0)
				{
					buf.Append(NoOffsetText);
				}
				else
				{
					int absHours = System.Math.Abs((totalSecs / 3600) % 100); // anything larger than 99 silently dropped
					int absMinutes = System.Math.Abs((totalSecs / 60) % 60);
					int absSeconds = System.Math.Abs(totalSecs % 60);
					int bufPos = buf.Length();
					int output = absHours;
					buf.Append(totalSecs < 0 ? "-" : "+").Append((char)(absHours / 10 + '0')).Append((char)(absHours % 10 + '0'));
					if (Type >= 3 || (Type >= 1 && absMinutes > 0))
					{
						buf.Append((Type % 2) == 0 ? ":" : "").Append((char)(absMinutes / 10 + '0')).Append((char)(absMinutes % 10 + '0'));
						output += absMinutes;
						if (Type >= 7 || (Type >= 5 && absSeconds > 0))
						{
							buf.Append((Type % 2) == 0 ? ":" : "").Append((char)(absSeconds / 10 + '0')).Append((char)(absSeconds % 10 + '0'));
							output += absSeconds;
						}
					}
					if (output == 0)
					{
						buf.Length = bufPos;
						buf.Append(NoOffsetText);
					}
				}
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int length = text.Length();
				int noOffsetLen = NoOffsetText.Length();
				if (noOffsetLen == 0)
				{
					if (position == length)
					{
						return context.SetParsedField(OFFSET_SECONDS, 0, position, position);
					}
				}
				else
				{
					if (position == length)
					{
						return ~position;
					}
					if (context.SubSequenceEquals(text, position, NoOffsetText, 0, noOffsetLen))
					{
						return context.SetParsedField(OFFSET_SECONDS, 0, position, position + noOffsetLen);
					}
				}

				// parse normal plus/minus offset
				char sign = text.CharAt(position); // IOOBE if invalid position
				if (sign == '+' || sign == '-')
				{
					// starts
					int negative = (sign == '-' ? - 1 : 1);
					int[] array = new int[4];
					array[0] = position + 1;
					if ((ParseNumber(array, 1, text, true) || ParseNumber(array, 2, text, Type >= 3) || ParseNumber(array, 3, text, false)) == false)
					{
						// success
						long offsetSecs = negative * (array[1] * 3600L + array[2] * 60L + array[3]);
						return context.SetParsedField(OFFSET_SECONDS, offsetSecs, position, array[0]);
					}
				}
				// handle special case of empty no offset text
				if (noOffsetLen == 0)
				{
					return context.SetParsedField(OFFSET_SECONDS, 0, position, position + noOffsetLen);
				}
				return ~position;
			}

			/// <summary>
			/// Parse a two digit zero-prefixed number.
			/// </summary>
			/// <param name="array">  the array of parsed data, 0=pos,1=hours,2=mins,3=secs, not null </param>
			/// <param name="arrayIndex">  the index to parse the value into </param>
			/// <param name="parseText">  the offset ID, not null </param>
			/// <param name="required">  whether this number is required </param>
			/// <returns> true if an error occurred </returns>
			internal bool ParseNumber(int[] array, int arrayIndex, CharSequence parseText, bool required)
			{
				if ((Type + 3) / 2 < arrayIndex)
				{
					return false; // ignore seconds/minutes
				}
				int pos = array[0];
				if ((Type % 2) == 0 && arrayIndex > 1)
				{
					if (pos + 1 > parseText.Length() || parseText.CharAt(pos) != ':')
					{
						return required;
					}
					pos++;
				}
				if (pos + 2 > parseText.Length())
				{
					return required;
				}
				char ch1 = parseText.CharAt(pos++);
				char ch2 = parseText.CharAt(pos++);
				if (ch1 < '0' || ch1 > '9' || ch2 < '0' || ch2 > '9')
				{
					return required;
				}
				int value = (ch1 - 48) * 10 + (ch2 - 48);
				if (value < 0 || value > 59)
				{
					return required;
				}
				array[arrayIndex] = value;
				array[0] = pos;
				return false;
			}

			public override String ToString()
			{
				String converted = NoOffsetText.Replace("'", "''");
				return "Offset(" + PATTERNS[Type] + ",'" + converted + "')";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses an offset ID.
		/// </summary>
		internal sealed class LocalizedOffsetIdPrinterParser : DateTimePrinterParser
		{
			internal readonly TextStyle Style;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="style">  the style, not null </param>
			internal LocalizedOffsetIdPrinterParser(TextStyle style)
			{
				this.Style = style;
			}

			internal static StringBuilder AppendHMS(StringBuilder buf, int t)
			{
				return buf.Append((char)(t / 10 + '0')).Append((char)(t % 10 + '0'));
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Long offsetSecs = context.GetValue(OFFSET_SECONDS);
				if (offsetSecs == null)
				{
					return false;
				}
				String gmtText = "GMT"; // TODO: get localized version of 'GMT'
				if (gmtText != null)
				{
					buf.Append(gmtText);
				}
				int totalSecs = Math.ToIntExact(offsetSecs);
				if (totalSecs != 0)
				{
					int absHours = System.Math.Abs((totalSecs / 3600) % 100); // anything larger than 99 silently dropped
					int absMinutes = System.Math.Abs((totalSecs / 60) % 60);
					int absSeconds = System.Math.Abs(totalSecs % 60);
					buf.Append(totalSecs < 0 ? "-" : "+");
					if (Style == TextStyle.FULL)
					{
						AppendHMS(buf, absHours);
						buf.Append(':');
						AppendHMS(buf, absMinutes);
						if (absSeconds != 0)
						{
						   buf.Append(':');
						   AppendHMS(buf, absSeconds);
						}
					}
					else
					{
						if (absHours >= 10)
						{
							buf.Append((char)(absHours / 10 + '0'));
						}
						buf.Append((char)(absHours % 10 + '0'));
						if (absMinutes != 0 || absSeconds != 0)
						{
							buf.Append(':');
							AppendHMS(buf, absMinutes);
							if (absSeconds != 0)
							{
								buf.Append(':');
								AppendHMS(buf, absSeconds);
							}
						}
					}
				}
				return true;
			}

			internal int GetDigit(CharSequence text, int position)
			{
				char c = text.CharAt(position);
				if (c < '0' || c > '9')
				{
					return -1;
				}
				return c - '0';
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int pos = position;
				int end = pos + text.Length();
				String gmtText = "GMT"; // TODO: get localized version of 'GMT'
				if (gmtText != null)
				{
					if (!context.SubSequenceEquals(text, pos, gmtText, 0, gmtText.Length()))
					{
						return ~position;
					}
					pos += gmtText.Length();
				}
				// parse normal plus/minus offset
				int negative = 0;
				if (pos == end)
				{
					return context.SetParsedField(OFFSET_SECONDS, 0, position, pos);
				}
				char sign = text.CharAt(pos); // IOOBE if invalid position
				if (sign == '+')
				{
					negative = 1;
				}
				else if (sign == '-')
				{
					negative = -1;
				}
				else
				{
					return context.SetParsedField(OFFSET_SECONDS, 0, position, pos);
				}
				pos++;
				int h = 0;
				int m = 0;
				int s = 0;
				if (Style == TextStyle.FULL)
				{
					int h1 = GetDigit(text, pos++);
					int h2 = GetDigit(text, pos++);
					if (h1 < 0 || h2 < 0 || text.CharAt(pos++) != ':')
					{
						return ~position;
					}
					h = h1 * 10 + h2;
					int m1 = GetDigit(text, pos++);
					int m2 = GetDigit(text, pos++);
					if (m1 < 0 || m2 < 0)
					{
						return ~position;
					}
					m = m1 * 10 + m2;
					if (pos + 2 < end && text.CharAt(pos) == ':')
					{
						int s1 = GetDigit(text, pos + 1);
						int s2 = GetDigit(text, pos + 2);
						if (s1 >= 0 && s2 >= 0)
						{
							s = s1 * 10 + s2;
							pos += 3;
						}
					}
				}
				else
				{
					h = GetDigit(text, pos++);
					if (h < 0)
					{
						return ~position;
					}
					if (pos < end)
					{
						int h2 = GetDigit(text, pos);
						if (h2 >= 0)
						{
							h = h * 10 + h2;
							pos++;
						}
						if (pos + 2 < end && text.CharAt(pos) == ':')
						{
							if (pos + 2 < end && text.CharAt(pos) == ':')
							{
								int m1 = GetDigit(text, pos + 1);
								int m2 = GetDigit(text, pos + 2);
								if (m1 >= 0 && m2 >= 0)
								{
									m = m1 * 10 + m2;
									pos += 3;
									if (pos + 2 < end && text.CharAt(pos) == ':')
									{
										int s1 = GetDigit(text, pos + 1);
										int s2 = GetDigit(text, pos + 2);
										if (s1 >= 0 && s2 >= 0)
										{
											s = s1 * 10 + s2;
											pos += 3;
										}
									}
								}
							}
						}
					}
				}
				long offsetSecs = negative * (h * 3600L + m * 60L + s);
				return context.SetParsedField(OFFSET_SECONDS, offsetSecs, position, pos);
			}

			public override String ToString()
			{
				return "LocalizedOffset(" + Style + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a zone ID.
		/// </summary>
		internal sealed class ZoneTextPrinterParser : ZoneIdPrinterParser
		{

			/// <summary>
			/// The text style to output. </summary>
			internal readonly TextStyle TextStyle;

			/// <summary>
			/// The preferred zoneid map </summary>
			internal Set<String> PreferredZones;

			internal ZoneTextPrinterParser(TextStyle textStyle, Set<ZoneId> preferredZones) : base(TemporalQueries.Zone(), "ZoneText(" + textStyle + ")")
			{
				this.TextStyle = Objects.RequireNonNull(textStyle, "textStyle");
				if (preferredZones != null && preferredZones.Count != 0)
				{
					this.PreferredZones = new HashSet<>();
					foreach (ZoneId id in preferredZones)
					{
						this.PreferredZones.Add(id.Id);
					}
				}
			}

			internal const int STD = 0;
			internal const int DST = 1;
			internal const int GENERIC = 2;
			internal static readonly IDictionary<String, SoftReference<IDictionary<Locale, String[]>>> Cache = new ConcurrentDictionary<String, SoftReference<IDictionary<Locale, String[]>>>();

			internal String GetDisplayName(String id, int type, Locale locale)
			{
				if (TextStyle == TextStyle.NARROW)
				{
					return null;
				}
				String[] names;
				SoftReference<IDictionary<Locale, String[]>> @ref = Cache[id];
				IDictionary<Locale, String[]> perLocale = null;
				if (@ref == null || (perLocale = @ref.get()) == null || (names = perLocale[locale]) == null)
				{
					names = TimeZoneNameUtility.retrieveDisplayNames(id, locale);
					if (names == null)
					{
						return null;
					}
					names = Arrays.CopyOfRange(names, 0, 7);
					names[5] = TimeZoneNameUtility.retrieveGenericDisplayName(id, TimeZone.LONG, locale);
					if (names[5] == null)
					{
						names[5] = names[0]; // use the id
					}
					names[6] = TimeZoneNameUtility.retrieveGenericDisplayName(id, TimeZone.SHORT, locale);
					if (names[6] == null)
					{
						names[6] = names[0];
					}
					if (perLocale == null)
					{
						perLocale = new ConcurrentDictionary<>();
					}
					perLocale[locale] = names;
					Cache[id] = new SoftReference<>(perLocale);
				}
				switch (type)
				{
				case STD:
					return names[TextStyle.zoneNameStyleIndex() + 1];
				case DST:
					return names[TextStyle.zoneNameStyleIndex() + 3];
				}
				return names[TextStyle.zoneNameStyleIndex() + 5];
			}

			public override bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				ZoneId zone = context.GetValue(TemporalQueries.ZoneId());
				if (zone == null)
				{
					return false;
				}
				String zname = zone.Id;
				if (!(zone is ZoneOffset))
				{
					TemporalAccessor dt = context.Temporal;
					String name = GetDisplayName(zname, dt.IsSupported(ChronoField.INSTANT_SECONDS) ? (zone.Rules.IsDaylightSavings(Instant.From(dt)) ? DST : STD) : GENERIC, context.Locale);
					if (name != null)
					{
						zname = name;
					}
				}
				buf.Append(zname);
				return true;
			}

			// cache per instance for now
			internal readonly IDictionary<Locale, Map_Entry<Integer, SoftReference<PrefixTree>>> CachedTree = new Dictionary<Locale, Map_Entry<Integer, SoftReference<PrefixTree>>>();
			internal readonly IDictionary<Locale, Map_Entry<Integer, SoftReference<PrefixTree>>> CachedTreeCI = new Dictionary<Locale, Map_Entry<Integer, SoftReference<PrefixTree>>>();

			protected internal override PrefixTree GetTree(DateTimeParseContext context)
			{
				if (TextStyle == TextStyle.NARROW)
				{
					return base.GetTree(context);
				}
				Locale locale = context.Locale;
				bool isCaseSensitive = context.CaseSensitive;
				Set<String> regionIds = ZoneRulesProvider.AvailableZoneIds;
				int regionIdsSize = regionIds.Count;

				IDictionary<Locale, Map_Entry<Integer, SoftReference<PrefixTree>>> cached = isCaseSensitive ? CachedTree : CachedTreeCI;

				Map_Entry<Integer, SoftReference<PrefixTree>> entry = null;
				PrefixTree tree = null;
				String[][] zoneStrings = null;
				if ((entry = cached[locale]) == null || (entry.Key != regionIdsSize || (tree = entry.Value.get()) == null))
				{
					tree = PrefixTree.NewTree(context);
					zoneStrings = TimeZoneNameUtility.getZoneStrings(locale);
					foreach (String[] names in zoneStrings)
					{
						String zid = names[0];
						if (!regionIds.Contains(zid))
						{
							continue;
						}
						tree.Add(zid, zid); // don't convert zid -> metazone
						zid = ZoneName.ToZid(zid, locale);
						int i = TextStyle == TextStyle.FULL ? 1 : 2;
						for (; i < names.Length; i += 2)
						{
							tree.Add(names[i], zid);
						}
					}
					// if we have a set of preferred zones, need a copy and
					// add the preferred zones again to overwrite
					if (PreferredZones != null)
					{
						foreach (String[] names in zoneStrings)
						{
							String zid = names[0];
							if (!PreferredZones.Contains(zid) || !regionIds.Contains(zid))
							{
								continue;
							}
							int i = TextStyle == TextStyle.FULL ? 1 : 2;
							for (; i < names.Length; i += 2)
							{
								tree.Add(names[i], zid);
							}
						}
					}
					cached[locale] = new SimpleImmutableEntry<>(regionIdsSize, new SoftReference<>(tree));
				}
				return tree;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a zone ID.
		/// </summary>
		internal class ZoneIdPrinterParser : DateTimePrinterParser
		{
			internal readonly TemporalQuery<ZoneId> Query;
			internal readonly String Description;

			internal ZoneIdPrinterParser(TemporalQuery<ZoneId> query, String description)
			{
				this.Query = query;
				this.Description = description;
			}

			public virtual bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				ZoneId zone = context.GetValue(Query);
				if (zone == null)
				{
					return false;
				}
				buf.Append(zone.Id);
				return true;
			}

			/// <summary>
			/// The cached tree to speed up parsing.
			/// </summary>
			internal static volatile Map_Entry<Integer, PrefixTree> CachedPrefixTree;
			internal static volatile Map_Entry<Integer, PrefixTree> CachedPrefixTreeCI;

			protected internal virtual PrefixTree GetTree(DateTimeParseContext context)
			{
				// prepare parse tree
				Set<String> regionIds = ZoneRulesProvider.AvailableZoneIds;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int regionIdsSize = regionIds.size();
				int regionIdsSize = regionIds.Count;
				Map_Entry<Integer, PrefixTree> cached = context.CaseSensitive ? CachedPrefixTree : CachedPrefixTreeCI;
				if (cached == null || cached.Key != regionIdsSize)
				{
					lock (this)
					{
						cached = context.CaseSensitive ? CachedPrefixTree : CachedPrefixTreeCI;
						if (cached == null || cached.Key != regionIdsSize)
						{
							cached = new SimpleImmutableEntry<>(regionIdsSize, PrefixTree.NewTree(regionIds, context));
							if (context.CaseSensitive)
							{
								CachedPrefixTree = cached;
							}
							else
							{
								CachedPrefixTreeCI = cached;
							}
						}
					}
				}
				return cached.Value;
			}

			/// <summary>
			/// This implementation looks for the longest matching string.
			/// For example, parsing Etc/GMT-2 will return Etc/GMC-2 rather than just
			/// Etc/GMC although both are valid.
			/// </summary>
			public virtual int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				int length = text.Length();
				if (position > length)
				{
					throw new IndexOutOfBoundsException();
				}
				if (position == length)
				{
					return ~position;
				}

				// handle fixed time-zone IDs
				char nextChar = text.CharAt(position);
				if (nextChar == '+' || nextChar == '-')
				{
					return ParseOffsetBased(context, text, position, position, OffsetIdPrinterParser.INSTANCE_ID_Z);
				}
				else if (length >= position + 2)
				{
					char nextNextChar = text.CharAt(position + 1);
					if (context.CharEquals(nextChar, 'U') && context.CharEquals(nextNextChar, 'T'))
					{
						if (length >= position + 3 && context.CharEquals(text.CharAt(position + 2), 'C'))
						{
							return ParseOffsetBased(context, text, position, position + 3, OffsetIdPrinterParser.INSTANCE_ID_ZERO);
						}
						return ParseOffsetBased(context, text, position, position + 2, OffsetIdPrinterParser.INSTANCE_ID_ZERO);
					}
					else if (context.CharEquals(nextChar, 'G') && length >= position + 3 && context.CharEquals(nextNextChar, 'M') && context.CharEquals(text.CharAt(position + 2), 'T'))
					{
						return ParseOffsetBased(context, text, position, position + 3, OffsetIdPrinterParser.INSTANCE_ID_ZERO);
					}
				}

				// parse
				PrefixTree tree = GetTree(context);
				ParsePosition ppos = new ParsePosition(position);
				String parsedZoneId = tree.Match(text, ppos);
				if (parsedZoneId == null)
				{
					if (context.CharEquals(nextChar, 'Z'))
					{
						context.Parsed = ZoneOffset.UTC;
						return position + 1;
					}
					return ~position;
				}
				context.Parsed = ZoneId.Of(parsedZoneId);
				return ppos.Index;
			}

			/// <summary>
			/// Parse an offset following a prefix and set the ZoneId if it is valid.
			/// To matching the parsing of ZoneId.of the values are not normalized
			/// to ZoneOffsets.
			/// </summary>
			/// <param name="context"> the parse context </param>
			/// <param name="text"> the input text </param>
			/// <param name="prefixPos"> start of the prefix </param>
			/// <param name="position"> start of text after the prefix </param>
			/// <param name="parser"> parser for the value after the prefix </param>
			/// <returns> the position after the parse </returns>
			internal virtual int ParseOffsetBased(DateTimeParseContext context, CharSequence text, int prefixPos, int position, OffsetIdPrinterParser parser)
			{
				String prefix = text.ToString().Substring(prefixPos, position - prefixPos).ToUpperCase();
				if (position >= text.Length())
				{
					context.Parsed = ZoneId.Of(prefix);
					return position;
				}

				// '0' or 'Z' after prefix is not part of a valid ZoneId; use bare prefix
				if (text.CharAt(position) == '0' || context.CharEquals(text.CharAt(position), 'Z'))
				{
					context.Parsed = ZoneId.Of(prefix);
					return position;
				}

				DateTimeParseContext newContext = context.Copy();
				int endPos = parser.Parse(newContext, text, position);
				try
				{
					if (endPos < 0)
					{
						if (parser == OffsetIdPrinterParser.INSTANCE_ID_Z)
						{
							return ~prefixPos;
						}
						context.Parsed = ZoneId.Of(prefix);
						return position;
					}
					int offset = (int) newContext.GetParsed(OFFSET_SECONDS).LongValue();
					ZoneOffset zoneOffset = ZoneOffset.OfTotalSeconds(offset);
					context.Parsed = ZoneId.OfOffset(prefix, zoneOffset);
					return endPos;
				}
				catch (DateTimeException)
				{
					return ~prefixPos;
				}
			}

			public override String ToString()
			{
				return Description;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// A String based prefix tree for parsing time-zone names.
		/// </summary>
		internal class PrefixTree
		{
			protected internal String Key;
			protected internal String Value;
			protected internal char C0; // performance optimization to avoid the
								  // boundary check cost of key.charat(0)
			protected internal PrefixTree Child;
			protected internal PrefixTree Sibling;

			internal PrefixTree(String k, String v, PrefixTree child)
			{
				this.Key = k;
				this.Value = v;
				this.Child = child;
				if (k.Length() == 0)
				{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					c0 = 0xffff;
				}
				else
				{
					C0 = Key.CharAt(0);
				}
			}

			/// <summary>
			/// Creates a new prefix parsing tree based on parse context.
			/// </summary>
			/// <param name="context">  the parse context </param>
			/// <returns> the tree, not null </returns>
			public static PrefixTree NewTree(DateTimeParseContext context)
			{
				//if (!context.isStrict()) {
				//    return new LENIENT("", null, null);
				//}
				if (context.CaseSensitive)
				{
					return new PrefixTree("", null, null);
				}
				return new CI("", null, null);
			}

			/// <summary>
			/// Creates a new prefix parsing tree.
			/// </summary>
			/// <param name="keys">  a set of strings to build the prefix parsing tree, not null </param>
			/// <param name="context">  the parse context </param>
			/// <returns> the tree, not null </returns>
			public static PrefixTree NewTree(Set<String> keys, DateTimeParseContext context)
			{
				PrefixTree tree = NewTree(context);
				foreach (String k in keys)
				{
					tree.Add0(k, k);
				}
				return tree;
			}

			/// <summary>
			/// Clone a copy of this tree
			/// </summary>
			public virtual PrefixTree CopyTree()
			{
				PrefixTree copy = new PrefixTree(Key, Value, null);
				if (Child != null)
				{
					copy.Child = Child.CopyTree();
				}
				if (Sibling != null)
				{
					copy.Sibling = Sibling.CopyTree();
				}
				return copy;
			}


			/// <summary>
			/// Adds a pair of {key, value} into the prefix tree.
			/// </summary>
			/// <param name="k">  the key, not null </param>
			/// <param name="v">  the value, not null </param>
			/// <returns>  true if the pair is added successfully </returns>
			public virtual bool Add(String k, String v)
			{
				return Add0(k, v);
			}

			internal virtual bool Add0(String k, String v)
			{
				k = ToKey(k);
				int prefixLen = PrefixLength(k);
				if (prefixLen == Key.Length())
				{
					if (prefixLen < k.Length()) // down the tree
					{
						String subKey = k.Substring(prefixLen);
						PrefixTree c = Child;
						while (c != null)
						{
							if (IsEqual(c.C0, subKey.CharAt(0)))
							{
								return c.Add0(subKey, v);
							}
							c = c.Sibling;
						}
						// add the node as the child of the current node
						c = NewNode(subKey, v, null);
						c.Sibling = Child;
						Child = c;
						return true;
					}
					// have an existing <key, value> already, overwrite it
					// if (value != null) {
					//    return false;
					//}
					Value = v;
					return true;
				}
				// split the existing node
				PrefixTree n1 = NewNode(Key.Substring(prefixLen), Value, Child);
				Key = k.Substring(0, prefixLen);
				Child = n1;
				if (prefixLen < k.Length())
				{
					PrefixTree n2 = NewNode(k.Substring(prefixLen), v, null);
					Child.Sibling = n2;
					Value = null;
				}
				else
				{
					Value = v;
				}
				return true;
			}

			/// <summary>
			/// Match text with the prefix tree.
			/// </summary>
			/// <param name="text">  the input text to parse, not null </param>
			/// <param name="off">  the offset position to start parsing at </param>
			/// <param name="end">  the end position to stop parsing </param>
			/// <returns> the resulting string, or null if no match found. </returns>
			public virtual String Match(CharSequence text, int off, int end)
			{
				if (!PrefixOf(text, off, end))
				{
					return null;
				}
				if (Child != null && (off += Key.Length()) != end)
				{
					PrefixTree c = Child;
					do
					{
						if (IsEqual(c.C0, text.CharAt(off)))
						{
							String found = c.Match(text, off, end);
							if (found != null)
							{
								return found;
							}
							return Value;
						}
						c = c.Sibling;
					} while (c != null);
				}
				return Value;
			}

			/// <summary>
			/// Match text with the prefix tree.
			/// </summary>
			/// <param name="text">  the input text to parse, not null </param>
			/// <param name="pos">  the position to start parsing at, from 0 to the text
			///  length. Upon return, position will be updated to the new parse
			///  position, or unchanged, if no match found. </param>
			/// <returns> the resulting string, or null if no match found. </returns>
			public virtual String Match(CharSequence text, ParsePosition pos)
			{
				int off = pos.Index;
				int end = text.Length();
				if (!PrefixOf(text, off, end))
				{
					return null;
				}
				off += Key.Length();
				if (Child != null && off != end)
				{
					PrefixTree c = Child;
					do
					{
						if (IsEqual(c.C0, text.CharAt(off)))
						{
							pos.Index = off;
							String found = c.Match(text, pos);
							if (found != null)
							{
								return found;
							}
							break;
						}
						c = c.Sibling;
					} while (c != null);
				}
				pos.Index = off;
				return Value;
			}

			protected internal virtual String ToKey(String k)
			{
				return k;
			}

			protected internal virtual PrefixTree NewNode(String k, String v, PrefixTree child)
			{
				return new PrefixTree(k, v, child);
			}

			protected internal virtual bool IsEqual(char c1, char c2)
			{
				return c1 == c2;
			}

			protected internal virtual bool PrefixOf(CharSequence text, int off, int end)
			{
				if (text is String)
				{
					return ((String)text).StartsWith(Key, off);
				}
				int len = Key.Length();
				if (len > end - off)
				{
					return false;
				}
				int off0 = 0;
				while (len-- > 0)
				{
					if (!IsEqual(Key.CharAt(off0++), text.CharAt(off++)))
					{
						return false;
					}
				}
				return true;
			}

			internal virtual int PrefixLength(String k)
			{
				int off = 0;
				while (off < k.Length() && off < Key.Length())
				{
					if (!IsEqual(k.CharAt(off), Key.CharAt(off)))
					{
						return off;
					}
					off++;
				}
				return off;
			}

			/// <summary>
			/// Case Insensitive prefix tree.
			/// </summary>
			private class CI : PrefixTree
			{

				internal CI(String k, String v, PrefixTree child) : base(k, v, child)
				{
				}

				protected internal override CI NewNode(String k, String v, PrefixTree child)
				{
					return new CI(k, v, child);
				}

				protected internal override bool IsEqual(char c1, char c2)
				{
					return DateTimeParseContext.CharEqualsIgnoreCase(c1, c2);
				}

				protected internal override bool PrefixOf(CharSequence text, int off, int end)
				{
					int len = Key.Length();
					if (len > end - off)
					{
						return false;
					}
					int off0 = 0;
					while (len-- > 0)
					{
						if (!IsEqual(Key.CharAt(off0++), text.CharAt(off++)))
						{
							return false;
						}
					}
					return true;
				}
			}

			/// <summary>
			/// Lenient prefix tree. Case insensitive and ignores characters
			/// like space, underscore and slash.
			/// </summary>
			private class LENIENT : CI
			{

				internal LENIENT(String k, String v, PrefixTree child) : base(k, v, child)
				{
				}

				protected internal override CI NewNode(String k, String v, PrefixTree child)
				{
					return new LENIENT(k, v, child);
				}

				internal virtual bool IsLenientChar(char c)
				{
					return c == ' ' || c == '_' || c == '/';
				}

				protected internal override String ToKey(String k)
				{
					for (int i = 0; i < k.Length(); i++)
					{
						if (IsLenientChar(k.CharAt(i)))
						{
							StringBuilder sb = new StringBuilder(k.Length());
							sb.Append(k, 0, i);
							i++;
							while (i < k.Length())
							{
								if (!IsLenientChar(k.CharAt(i)))
								{
									sb.Append(k.CharAt(i));
								}
								i++;
							}
							return sb.ToString();
						}
					}
					return k;
				}

				public override String Match(CharSequence text, ParsePosition pos)
				{
					int off = pos.Index;
					int end = text.Length();
					int len = Key.Length();
					int koff = 0;
					while (koff < len && off < end)
					{
						if (IsLenientChar(text.CharAt(off)))
						{
							off++;
							continue;
						}
						if (!IsEqual(Key.CharAt(koff++), text.CharAt(off++)))
						{
							return null;
						}
					}
					if (koff != len)
					{
						return null;
					}
					if (Child != null && off != end)
					{
						int off0 = off;
						while (off0 < end && IsLenientChar(text.CharAt(off0)))
						{
							off0++;
						}
						if (off0 < end)
						{
							PrefixTree c = Child;
							do
							{
								if (IsEqual(c.C0, text.CharAt(off0)))
								{
									pos.Index = off0;
									String found = c.Match(text, pos);
									if (found != null)
									{
										return found;
									}
									break;
								}
								c = c.Sibling;
							} while (c != null);
						}
					}
					pos.Index = off;
					return Value;
				}
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a chronology.
		/// </summary>
		internal sealed class ChronoPrinterParser : DateTimePrinterParser
		{
			/// <summary>
			/// The text style to output, null means the ID. </summary>
			internal readonly TextStyle TextStyle;

			internal ChronoPrinterParser(TextStyle textStyle)
			{
				// validated by caller
				this.TextStyle = textStyle;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Chronology chrono = context.GetValue(TemporalQueries.Chronology());
				if (chrono == null)
				{
					return false;
				}
				if (TextStyle == null)
				{
					buf.Append(chrono.Id);
				}
				else
				{
					buf.Append(GetChronologyName(chrono, context.Locale));
				}
				return true;
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				// simple looping parser to find the chronology
				if (position < 0 || position > text.Length())
				{
					throw new IndexOutOfBoundsException();
				}
				Set<Chronology> chronos = Chronology.AvailableChronologies;
				Chronology bestMatch = null;
				int matchLen = -1;
				foreach (Chronology chrono in chronos)
				{
					String name;
					if (TextStyle == null)
					{
						name = chrono.Id;
					}
					else
					{
						name = GetChronologyName(chrono, context.Locale);
					}
					int nameLen = name.Length();
					if (nameLen > matchLen && context.SubSequenceEquals(text, position, name, 0, nameLen))
					{
						bestMatch = chrono;
						matchLen = nameLen;
					}
				}
				if (bestMatch == null)
				{
					return ~position;
				}
				context.Parsed = bestMatch;
				return position + matchLen;
			}

			/// <summary>
			/// Returns the chronology name of the given chrono in the given locale
			/// if available, or the chronology Id otherwise. The regular ResourceBundle
			/// search path is used for looking up the chronology name.
			/// </summary>
			/// <param name="chrono">  the chronology, not null </param>
			/// <param name="locale">  the locale, not null </param>
			/// <returns> the chronology name of chrono in locale, or the id if no name is available </returns>
			/// <exception cref="NullPointerException"> if chrono or locale is null </exception>
			internal String GetChronologyName(Chronology chrono, Locale locale)
			{
				String key = "calendarname." + chrono.CalendarType;
				String name = DateTimeTextProvider.GetLocalizedResource(key, locale);
				return name != null ? name : chrono.Id;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a localized pattern.
		/// </summary>
		internal sealed class LocalizedPrinterParser : DateTimePrinterParser
		{
			/// <summary>
			/// Cache of formatters. </summary>
			internal static readonly ConcurrentMap<String, DateTimeFormatter> FORMATTER_CACHE = new ConcurrentDictionary<String, DateTimeFormatter>(16, 0.75f, 2);

			internal readonly FormatStyle DateStyle;
			internal readonly FormatStyle TimeStyle;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="dateStyle">  the date style to use, may be null </param>
			/// <param name="timeStyle">  the time style to use, may be null </param>
			internal LocalizedPrinterParser(FormatStyle dateStyle, FormatStyle timeStyle)
			{
				// validated by caller
				this.DateStyle = dateStyle;
				this.TimeStyle = timeStyle;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				Chronology chrono = Chronology.from(context.Temporal);
				return Formatter(context.Locale, chrono).ToPrinterParser(false).Format(context, buf);
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				Chronology chrono = context.EffectiveChronology;
				return Formatter(context.Locale, chrono).ToPrinterParser(false).Parse(context, text, position);
			}

			/// <summary>
			/// Gets the formatter to use.
			/// <para>
			/// The formatter will be the most appropriate to use for the date and time style in the locale.
			/// For example, some locales will use the month name while others will use the number.
			/// 
			/// </para>
			/// </summary>
			/// <param name="locale">  the locale to use, not null </param>
			/// <param name="chrono">  the chronology to use, not null </param>
			/// <returns> the formatter, not null </returns>
			/// <exception cref="IllegalArgumentException"> if the formatter cannot be found </exception>
			internal DateTimeFormatter Formatter(Locale locale, Chronology chrono)
			{
				String key = chrono.Id + '|' + locale.ToString() + '|' + DateStyle + TimeStyle;
				DateTimeFormatter formatter = FORMATTER_CACHE[key];
				if (formatter == null)
				{
					String pattern = GetLocalizedDateTimePattern(DateStyle, TimeStyle, chrono, locale);
					formatter = (new DateTimeFormatterBuilder()).AppendPattern(pattern).ToFormatter(locale);
					DateTimeFormatter old = FORMATTER_CACHE.PutIfAbsent(key, formatter);
					if (old != null)
					{
						formatter = old;
					}
				}
				return formatter;
			}

			public override String ToString()
			{
				return "Localized(" + (DateStyle != null ? DateStyle : "") + "," + (TimeStyle != null ? TimeStyle : "") + ")";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Prints or parses a localized pattern from a localized field.
		/// The specific formatter and parameters is not selected until the
		/// the field is to be printed or parsed.
		/// The locale is needed to select the proper WeekFields from which
		/// the field for day-of-week, week-of-month, or week-of-year is selected.
		/// </summary>
		internal sealed class WeekBasedFieldPrinterParser : DateTimePrinterParser
		{
			internal char Chr;
			internal int Count;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="chr"> the pattern format letter that added this PrinterParser. </param>
			/// <param name="count"> the repeat count of the format letter </param>
			internal WeekBasedFieldPrinterParser(char chr, int count)
			{
				this.Chr = chr;
				this.Count = count;
			}

			public bool Format(DateTimePrintContext context, StringBuilder buf)
			{
				return PrinterParser(context.Locale).Format(context, buf);
			}

			public int Parse(DateTimeParseContext context, CharSequence text, int position)
			{
				return PrinterParser(context.Locale).Parse(context, text, position);
			}

			/// <summary>
			/// Gets the printerParser to use based on the field and the locale.
			/// </summary>
			/// <param name="locale">  the locale to use, not null </param>
			/// <returns> the formatter, not null </returns>
			/// <exception cref="IllegalArgumentException"> if the formatter cannot be found </exception>
			internal DateTimePrinterParser PrinterParser(Locale locale)
			{
				WeekFields weekDef = WeekFields.Of(locale);
				TemporalField field = null;
				switch (Chr)
				{
					case 'Y':
						field = weekDef.WeekBasedYear();
						if (Count == 2)
						{
							return new ReducedPrinterParser(field, 2, 2, 0, ReducedPrinterParser.BASE_DATE, 0);
						}
						else
						{
							return new NumberPrinterParser(field, Count, 19, (Count < 4) ? SignStyle.NORMAL : SignStyle.EXCEEDS_PAD, -1);
						}
					case 'e':
					case 'c':
						field = weekDef.DayOfWeek();
						break;
					case 'w':
						field = weekDef.WeekOfWeekBasedYear();
						break;
					case 'W':
						field = weekDef.WeekOfMonth();
						break;
					default:
						throw new IllegalStateException("unreachable");
				}
				return new NumberPrinterParser(field, (Count == 2 ? 2 : 1), 2, SignStyle.NOT_NEGATIVE);
			}

			public override String ToString()
			{
				StringBuilder sb = new StringBuilder(30);
				sb.Append("Localized(");
				if (Chr == 'Y')
				{
					if (Count == 1)
					{
						sb.Append("WeekBasedYear");
					}
					else if (Count == 2)
					{
						sb.Append("ReducedValue(WeekBasedYear,2,2,2000-01-01)");
					}
					else
					{
						sb.Append("WeekBasedYear,").Append(Count).Append(",").Append(19).Append(",").Append((Count < 4) ? SignStyle.NORMAL : SignStyle.EXCEEDS_PAD);
					}
				}
				else
				{
					switch (Chr)
					{
						case 'c':
						case 'e':
							sb.Append("DayOfWeek");
							break;
						case 'w':
							sb.Append("WeekOfWeekBasedYear");
							break;
						case 'W':
							sb.Append("WeekOfMonth");
							break;
						default:
							break;
					}
					sb.Append(",");
					sb.Append(Count);
				}
				sb.Append(")");
				return sb.ToString();
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Length comparator.
		/// </summary>
		internal static readonly IComparer<String> LENGTH_SORT = new ComparatorAnonymousInnerClassHelper();

		private class ComparatorAnonymousInnerClassHelper : Comparator<String>
		{
			public ComparatorAnonymousInnerClassHelper()
			{
			}

			public virtual int Compare(String str1, String str2)
			{
				return str1.Length() == str2.Length() ? str1.CompareTo(str2) : str1.Length() - str2.Length();
			}
		}
	}

}