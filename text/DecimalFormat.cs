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
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
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
	using ResourceBundleBasedAdapter = sun.util.locale.provider.ResourceBundleBasedAdapter;

	/// <summary>
	/// <code>DecimalFormat</code> is a concrete subclass of
	/// <code>NumberFormat</code> that formats decimal numbers. It has a variety of
	/// features designed to make it possible to parse and format numbers in any
	/// locale, including support for Western, Arabic, and Indic digits.  It also
	/// supports different kinds of numbers, including integers (123), fixed-point
	/// numbers (123.4), scientific notation (1.23E4), percentages (12%), and
	/// currency amounts ($123).  All of these can be localized.
	/// 
	/// <para>To obtain a <code>NumberFormat</code> for a specific locale, including the
	/// default locale, call one of <code>NumberFormat</code>'s factory methods, such
	/// as <code>getInstance()</code>.  In general, do not call the
	/// <code>DecimalFormat</code> constructors directly, since the
	/// <code>NumberFormat</code> factory methods may return subclasses other than
	/// <code>DecimalFormat</code>. If you need to customize the format object, do
	/// something like this:
	/// 
	/// <blockquote><pre>
	/// NumberFormat f = NumberFormat.getInstance(loc);
	/// if (f instanceof DecimalFormat) {
	///     ((DecimalFormat) f).setDecimalSeparatorAlwaysShown(true);
	/// }
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>A <code>DecimalFormat</code> comprises a <em>pattern</em> and a set of
	/// <em>symbols</em>.  The pattern may be set directly using
	/// <code>applyPattern()</code>, or indirectly using the API methods.  The
	/// symbols are stored in a <code>DecimalFormatSymbols</code> object.  When using
	/// the <code>NumberFormat</code> factory methods, the pattern and symbols are
	/// read from localized <code>ResourceBundle</code>s.
	/// 
	/// <h3>Patterns</h3>
	/// 
	/// <code>DecimalFormat</code> patterns have the following syntax:
	/// <blockquote><pre>
	/// <i>Pattern:</i>
	///         <i>PositivePattern</i>
	///         <i>PositivePattern</i> ; <i>NegativePattern</i>
	/// <i>PositivePattern:</i>
	///         <i>Prefix<sub>opt</sub></i> <i>Number</i> <i>Suffix<sub>opt</sub></i>
	/// <i>NegativePattern:</i>
	///         <i>Prefix<sub>opt</sub></i> <i>Number</i> <i>Suffix<sub>opt</sub></i>
	/// <i>Prefix:</i>
	///         any Unicode characters except &#92;uFFFE, &#92;uFFFF, and special characters
	/// <i>Suffix:</i>
	///         any Unicode characters except &#92;uFFFE, &#92;uFFFF, and special characters
	/// <i>Number:</i>
	///         <i>Integer</i> <i>Exponent<sub>opt</sub></i>
	///         <i>Integer</i> . <i>Fraction</i> <i>Exponent<sub>opt</sub></i>
	/// <i>Integer:</i>
	///         <i>MinimumInteger</i>
	///         #
	///         # <i>Integer</i>
	///         # , <i>Integer</i>
	/// <i>MinimumInteger:</i>
	///         0
	///         0 <i>MinimumInteger</i>
	///         0 , <i>MinimumInteger</i>
	/// <i>Fraction:</i>
	///         <i>MinimumFraction<sub>opt</sub></i> <i>OptionalFraction<sub>opt</sub></i>
	/// <i>MinimumFraction:</i>
	///         0 <i>MinimumFraction<sub>opt</sub></i>
	/// <i>OptionalFraction:</i>
	///         # <i>OptionalFraction<sub>opt</sub></i>
	/// <i>Exponent:</i>
	///         E <i>MinimumExponent</i>
	/// <i>MinimumExponent:</i>
	///         0 <i>MinimumExponent<sub>opt</sub></i>
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>A <code>DecimalFormat</code> pattern contains a positive and negative
	/// subpattern, for example, <code>"#,##0.00;(#,##0.00)"</code>.  Each
	/// subpattern has a prefix, numeric part, and suffix. The negative subpattern
	/// is optional; if absent, then the positive subpattern prefixed with the
	/// localized minus sign (<code>'-'</code> in most locales) is used as the
	/// negative subpattern. That is, <code>"0.00"</code> alone is equivalent to
	/// <code>"0.00;-0.00"</code>.  If there is an explicit negative subpattern, it
	/// serves only to specify the negative prefix and suffix; the number of digits,
	/// minimal digits, and other characteristics are all the same as the positive
	/// pattern. That means that <code>"#,##0.0#;(#)"</code> produces precisely
	/// the same behavior as <code>"#,##0.0#;(#,##0.0#)"</code>.
	/// 
	/// </para>
	/// <para>The prefixes, suffixes, and various symbols used for infinity, digits,
	/// thousands separators, decimal separators, etc. may be set to arbitrary
	/// values, and they will appear properly during formatting.  However, care must
	/// be taken that the symbols and strings do not conflict, or parsing will be
	/// unreliable.  For example, either the positive and negative prefixes or the
	/// suffixes must be distinct for <code>DecimalFormat.parse()</code> to be able
	/// to distinguish positive from negative values.  (If they are identical, then
	/// <code>DecimalFormat</code> will behave as if no negative subpattern was
	/// specified.)  Another example is that the decimal separator and thousands
	/// separator should be distinct characters, or parsing will be impossible.
	/// 
	/// </para>
	/// <para>The grouping separator is commonly used for thousands, but in some
	/// countries it separates ten-thousands. The grouping size is a constant number
	/// of digits between the grouping characters, such as 3 for 100,000,000 or 4 for
	/// 1,0000,0000.  If you supply a pattern with multiple grouping characters, the
	/// interval between the last one and the end of the integer is the one that is
	/// used. So <code>"#,##,###,####"</code> == <code>"######,####"</code> ==
	/// <code>"##,####,####"</code>.
	/// 
	/// <h4>Special Pattern Characters</h4>
	/// 
	/// </para>
	/// <para>Many characters in a pattern are taken literally; they are matched during
	/// parsing and output unchanged during formatting.  Special characters, on the
	/// other hand, stand for other characters, strings, or classes of characters.
	/// They must be quoted, unless noted otherwise, if they are to appear in the
	/// prefix or suffix as literals.
	/// 
	/// </para>
	/// <para>The characters listed here are used in non-localized patterns.  Localized
	/// patterns use the corresponding characters taken from this formatter's
	/// <code>DecimalFormatSymbols</code> object instead, and these characters lose
	/// their special status.  Two exceptions are the currency sign and quote, which
	/// are not localized.
	/// 
	/// <blockquote>
	/// <table border=0 cellspacing=3 cellpadding=0 summary="Chart showing symbol,
	///  location, localized, and meaning.">
	///     <tr style="background-color: rgb(204, 204, 255);">
	///          <th align=left>Symbol
	///          <th align=left>Location
	///          <th align=left>Localized?
	///          <th align=left>Meaning
	///     <tr valign=top>
	///          <td><code>0</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Digit
	///     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///          <td><code>#</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Digit, zero shows as absent
	///     <tr valign=top>
	///          <td><code>.</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Decimal separator or monetary decimal separator
	///     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///          <td><code>-</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Minus sign
	///     <tr valign=top>
	///          <td><code>,</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Grouping separator
	///     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///          <td><code>E</code>
	///          <td>Number
	///          <td>Yes
	///          <td>Separates mantissa and exponent in scientific notation.
	///              <em>Need not be quoted in prefix or suffix.</em>
	///     <tr valign=top>
	///          <td><code>;</code>
	///          <td>Subpattern boundary
	///          <td>Yes
	///          <td>Separates positive and negative subpatterns
	///     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///          <td><code>%</code>
	///          <td>Prefix or suffix
	///          <td>Yes
	///          <td>Multiply by 100 and show as percentage
	///     <tr valign=top>
	///          <td><code>&#92;u2030</code>
	///          <td>Prefix or suffix
	///          <td>Yes
	///          <td>Multiply by 1000 and show as per mille value
	///     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///          <td><code>&#164;</code> (<code>&#92;u00A4</code>)
	///          <td>Prefix or suffix
	///          <td>No
	///          <td>Currency sign, replaced by currency symbol.  If
	///              doubled, replaced by international currency symbol.
	///              If present in a pattern, the monetary decimal separator
	///              is used instead of the decimal separator.
	///     <tr valign=top>
	///          <td><code>'</code>
	///          <td>Prefix or suffix
	///          <td>No
	///          <td>Used to quote special characters in a prefix or suffix,
	///              for example, <code>"'#'#"</code> formats 123 to
	///              <code>"#123"</code>.  To create a single quote
	///              itself, use two in a row: <code>"# o''clock"</code>.
	/// </table>
	/// </blockquote>
	/// 
	/// <h4>Scientific Notation</h4>
	/// 
	/// </para>
	/// <para>Numbers in scientific notation are expressed as the product of a mantissa
	/// and a power of ten, for example, 1234 can be expressed as 1.234 x 10^3.  The
	/// mantissa is often in the range 1.0 &le; x {@literal <} 10.0, but it need not
	/// be.
	/// <code>DecimalFormat</code> can be instructed to format and parse scientific
	/// notation <em>only via a pattern</em>; there is currently no factory method
	/// that creates a scientific notation format.  In a pattern, the exponent
	/// character immediately followed by one or more digit characters indicates
	/// scientific notation.  Example: <code>"0.###E0"</code> formats the number
	/// 1234 as <code>"1.234E3"</code>.
	/// 
	/// <ul>
	/// <li>The number of digit characters after the exponent character gives the
	/// minimum exponent digit count.  There is no maximum.  Negative exponents are
	/// formatted using the localized minus sign, <em>not</em> the prefix and suffix
	/// from the pattern.  This allows patterns such as <code>"0.###E0 m/s"</code>.
	/// 
	/// <li>The minimum and maximum number of integer digits are interpreted
	/// together:
	/// 
	/// <ul>
	/// <li>If the maximum number of integer digits is greater than their minimum number
	/// and greater than 1, it forces the exponent to be a multiple of the maximum
	/// number of integer digits, and the minimum number of integer digits to be
	/// interpreted as 1.  The most common use of this is to generate
	/// <em>engineering notation</em>, in which the exponent is a multiple of three,
	/// e.g., <code>"##0.#####E0"</code>. Using this pattern, the number 12345
	/// formats to <code>"12.345E3"</code>, and 123456 formats to
	/// <code>"123.456E3"</code>.
	/// 
	/// <li>Otherwise, the minimum number of integer digits is achieved by adjusting the
	/// exponent.  Example: 0.00123 formatted with <code>"00.###E0"</code> yields
	/// <code>"12.3E-4"</code>.
	/// </ul>
	/// 
	/// <li>The number of significant digits in the mantissa is the sum of the
	/// <em>minimum integer</em> and <em>maximum fraction</em> digits, and is
	/// unaffected by the maximum integer digits.  For example, 12345 formatted with
	/// <code>"##0.##E0"</code> is <code>"12.3E3"</code>. To show all digits, set
	/// the significant digits count to zero.  The number of significant digits
	/// does not affect parsing.
	/// 
	/// <li>Exponential patterns may not contain grouping separators.
	/// </ul>
	/// 
	/// <h4>Rounding</h4>
	/// 
	/// <code>DecimalFormat</code> provides rounding modes defined in
	/// <seealso cref="java.math.RoundingMode"/> for formatting.  By default, it uses
	/// <seealso cref="java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN"/>.
	/// 
	/// <h4>Digits</h4>
	/// 
	/// For formatting, <code>DecimalFormat</code> uses the ten consecutive
	/// characters starting with the localized zero digit defined in the
	/// <code>DecimalFormatSymbols</code> object as digits. For parsing, these
	/// digits as well as all Unicode decimal digits, as defined by
	/// <seealso cref="Character#digit Character.digit"/>, are recognized.
	/// 
	/// <h4>Special Values</h4>
	/// 
	/// </para>
	/// <para><code>NaN</code> is formatted as a string, which typically has a single character
	/// <code>&#92;uFFFD</code>.  This string is determined by the
	/// <code>DecimalFormatSymbols</code> object.  This is the only value for which
	/// the prefixes and suffixes are not used.
	/// 
	/// </para>
	/// <para>Infinity is formatted as a string, which typically has a single character
	/// <code>&#92;u221E</code>, with the positive or negative prefixes and suffixes
	/// applied.  The infinity string is determined by the
	/// <code>DecimalFormatSymbols</code> object.
	/// 
	/// </para>
	/// <para>Negative zero (<code>"-0"</code>) parses to
	/// <ul>
	/// <li><code>BigDecimal(0)</code> if <code>isParseBigDecimal()</code> is
	/// true,
	/// <li><code>Long(0)</code> if <code>isParseBigDecimal()</code> is false
	///     and <code>isParseIntegerOnly()</code> is true,
	/// <li><code>Double(-0.0)</code> if both <code>isParseBigDecimal()</code>
	/// and <code>isParseIntegerOnly()</code> are false.
	/// </ul>
	/// 
	/// <h4><a name="synchronization">Synchronization</a></h4>
	/// 
	/// </para>
	/// <para>
	/// Decimal formats are generally not synchronized.
	/// It is recommended to create separate format instances for each thread.
	/// If multiple threads access a format concurrently, it must be synchronized
	/// externally.
	/// 
	/// <h4>Example</h4>
	/// 
	/// <blockquote><pre>{@code
	/// <strong>// Print out a number using the localized number, integer, currency,
	/// // and percent format for each locale</strong>
	/// Locale[] locales = NumberFormat.getAvailableLocales();
	/// double myNumber = -1234.56;
	/// NumberFormat form;
	/// for (int j = 0; j < 4; ++j) {
	///     System.out.println("FORMAT");
	///     for (int i = 0; i < locales.length; ++i) {
	///         if (locales[i].getCountry().length() == 0) {
	///            continue; // Skip language-only locales
	///         }
	///         System.out.print(locales[i].getDisplayName());
	///         switch (j) {
	///         case 0:
	///             form = NumberFormat.getInstance(locales[i]); break;
	///         case 1:
	///             form = NumberFormat.getIntegerInstance(locales[i]); break;
	///         case 2:
	///             form = NumberFormat.getCurrencyInstance(locales[i]); break;
	///         default:
	///             form = NumberFormat.getPercentInstance(locales[i]); break;
	///         }
	///         if (form instanceof DecimalFormat) {
	///             System.out.print(": " + ((DecimalFormat) form).toPattern());
	///         }
	///         System.out.print(" -> " + form.format(myNumber));
	///         try {
	///             System.out.println(" -> " + form.parse(form.format(myNumber)));
	///         } catch (ParseException e) {}
	///     }
	/// }
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          <a href="https://docs.oracle.com/javase/tutorial/i18n/format/decimalFormat.html">Java Tutorial</a> </seealso>
	/// <seealso cref=          NumberFormat </seealso>
	/// <seealso cref=          DecimalFormatSymbols </seealso>
	/// <seealso cref=          ParsePosition
	/// @author       Mark Davis
	/// @author       Alan Liu </seealso>
	public class DecimalFormat : NumberFormat
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			MaximumIntegerDigits_Renamed = base.MaximumIntegerDigits;
			MinimumIntegerDigits_Renamed = base.MinimumIntegerDigits;
			MaximumFractionDigits_Renamed = base.MaximumFractionDigits;
			MinimumFractionDigits_Renamed = base.MinimumFractionDigits;
		}


		/// <summary>
		/// Creates a DecimalFormat using the default pattern and symbols
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// This is a convenient way to obtain a
		/// DecimalFormat when internationalization is not the main concern.
		/// <para>
		/// To obtain standard formats for a given locale, use the factory methods
		/// on NumberFormat such as getNumberInstance. These factories will
		/// return the most appropriate sub-class of NumberFormat for a given
		/// locale.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.text.NumberFormat#getInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		public DecimalFormat()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			// Get the pattern for the default locale.
			Locale def = Locale.GetDefault(Locale.Category.FORMAT);
			LocaleProviderAdapter adapter = LocaleProviderAdapter.getAdapter(typeof(NumberFormatProvider), def);
			if (!(adapter is ResourceBundleBasedAdapter))
			{
				adapter = LocaleProviderAdapter.ResourceBundleBased;
			}
			String[] all = adapter.getLocaleResources(def).NumberPatterns;

			// Always applyPattern after the symbols are set
			this.Symbols = DecimalFormatSymbols.GetInstance(def);
			ApplyPattern(all[0], false);
		}


		/// <summary>
		/// Creates a DecimalFormat using the given pattern and the symbols
		/// for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// This is a convenient way to obtain a
		/// DecimalFormat when internationalization is not the main concern.
		/// <para>
		/// To obtain standard formats for a given locale, use the factory methods
		/// on NumberFormat such as getNumberInstance. These factories will
		/// return the most appropriate sub-class of NumberFormat for a given
		/// locale.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a non-localized pattern string. </param>
		/// <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		/// <seealso cref= java.text.NumberFormat#getInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		public DecimalFormat(String pattern)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			// Always applyPattern after the symbols are set
			this.Symbols = DecimalFormatSymbols.GetInstance(Locale.GetDefault(Locale.Category.FORMAT));
			ApplyPattern(pattern, false);
		}


		/// <summary>
		/// Creates a DecimalFormat using the given pattern and symbols.
		/// Use this constructor when you need to completely customize the
		/// behavior of the format.
		/// <para>
		/// To obtain standard formats for a given
		/// locale, use the factory methods on NumberFormat such as
		/// getInstance or getCurrencyInstance. If you need only minor adjustments
		/// to a standard format, you can modify the format returned by
		/// a NumberFormat factory method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a non-localized pattern string </param>
		/// <param name="symbols"> the set of symbols to be used </param>
		/// <exception cref="NullPointerException"> if any of the given arguments is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		/// <seealso cref= java.text.NumberFormat#getInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		/// <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		/// <seealso cref= java.text.DecimalFormatSymbols </seealso>
		public DecimalFormat(String pattern, DecimalFormatSymbols symbols)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			// Always applyPattern after the symbols are set
			this.Symbols = (DecimalFormatSymbols)symbols.Clone();
			ApplyPattern(pattern, false);
		}


		// Overrides
		/// <summary>
		/// Formats a number and appends the resulting text to the given string
		/// buffer.
		/// The number can be of any subclass of <seealso cref="java.lang.Number"/>.
		/// <para>
		/// This implementation uses the maximum precision permitted.
		/// </para>
		/// </summary>
		/// <param name="number">     the number to format </param>
		/// <param name="toAppendTo"> the <code>StringBuffer</code> to which the formatted
		///                   text is to be appended </param>
		/// <param name="pos">        On input: an alignment field, if desired.
		///                   On output: the offsets of the alignment field. </param>
		/// <returns>           the value passed in as <code>toAppendTo</code> </returns>
		/// <exception cref="IllegalArgumentException"> if <code>number</code> is
		///                   null or not an instance of <code>Number</code>. </exception>
		/// <exception cref="NullPointerException"> if <code>toAppendTo</code> or
		///                   <code>pos</code> is null </exception>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <seealso cref=              java.text.FieldPosition </seealso>
		public override sealed StringBuffer Format(Object number, StringBuffer toAppendTo, FieldPosition pos)
		{
			if (number is Long || number is Integer || number is Short || number is Byte || number is AtomicInteger || number is AtomicLong || (number is System.Numerics.BigInteger && ((System.Numerics.BigInteger)number).BitLength() < 64))
			{
				return Format(((Number)number).LongValue(), toAppendTo, pos);
			}
			else if (number is decimal)
			{
				return Format((decimal)number, toAppendTo, pos);
			}
			else if (number is System.Numerics.BigInteger)
			{
				return Format((System.Numerics.BigInteger)number, toAppendTo, pos);
			}
			else if (number is Number)
			{
				return Format(((Number)number).DoubleValue(), toAppendTo, pos);
			}
			else
			{
				throw new IllegalArgumentException("Cannot format given Object as a Number");
			}
		}

		/// <summary>
		/// Formats a double to produce a string. </summary>
		/// <param name="number">    The double to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="fieldPosition">    On input: an alignment field, if desired.
		/// On output: the offsets of the alignment field. </param>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///            mode being set to RoundingMode.UNNECESSARY </exception>
		/// <returns> The formatted number string </returns>
		/// <seealso cref= java.text.FieldPosition </seealso>
		public override StringBuffer Format(double number, StringBuffer result, FieldPosition fieldPosition)
		{
			// If fieldPosition is a DontCareFieldPosition instance we can
			// try to go to fast-path code.
			bool tryFastPath = false;
			if (fieldPosition == DontCareFieldPosition.INSTANCE)
			{
				tryFastPath = true;
			}
			else
			{
				fieldPosition.BeginIndex = 0;
				fieldPosition.EndIndex = 0;
			}

			if (tryFastPath)
			{
				String tempResult = FastFormat(number);
				if (tempResult != null)
				{
					result.Append(tempResult);
					return result;
				}
			}

			// if fast-path could not work, we fallback to standard code.
			return Format(number, result, fieldPosition.FieldDelegate);
		}

		/// <summary>
		/// Formats a double to produce a string. </summary>
		/// <param name="number">    The double to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="delegate"> notified of locations of sub fields </param>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                  mode being set to RoundingMode.UNNECESSARY </exception>
		/// <returns> The formatted number string </returns>
		private StringBuffer Format(double number, StringBuffer result, FieldDelegate @delegate)
		{
			if (Double.IsNaN(number) || (Double.IsInfinity(number) && Multiplier_Renamed == 0))
			{
				int iFieldStart = result.Length();
				result.Append(Symbols.NaN);
				@delegate.Formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.Length(), result);
				return result;
			}

			/* Detecting whether a double is negative is easy with the exception of
			 * the value -0.0.  This is a double which has a zero mantissa (and
			 * exponent), but a negative sign bit.  It is semantically distinct from
			 * a zero with a positive sign bit, and this distinction is important
			 * to certain kinds of computations.  However, it's a little tricky to
			 * detect, since (-0.0 == 0.0) and !(-0.0 < 0.0).  How then, you may
			 * ask, does it behave distinctly from +0.0?  Well, 1/(-0.0) ==
			 * -Infinity.  Proper detection of -0.0 is needed to deal with the
			 * issues raised by bugs 4106658, 4106667, and 4147706.  Liu 7/6/98.
			 */
			bool isNegative = ((number < 0.0) || (number == 0.0 && 1 / number < 0.0)) ^ (Multiplier_Renamed < 0);

			if (Multiplier_Renamed != 1)
			{
				number *= Multiplier_Renamed;
			}

			if (Double.IsInfinity(number))
			{
				if (isNegative)
				{
					Append(result, NegativePrefix_Renamed, @delegate, NegativePrefixFieldPositions, Field.SIGN);
				}
				else
				{
					Append(result, PositivePrefix_Renamed, @delegate, PositivePrefixFieldPositions, Field.SIGN);
				}

				int iFieldStart = result.Length();
				result.Append(Symbols.Infinity);
				@delegate.Formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.Length(), result);

				if (isNegative)
				{
					Append(result, NegativeSuffix_Renamed, @delegate, NegativeSuffixFieldPositions, Field.SIGN);
				}
				else
				{
					Append(result, PositiveSuffix_Renamed, @delegate, PositiveSuffixFieldPositions, Field.SIGN);
				}

				return result;
			}

			if (isNegative)
			{
				number = -number;
			}

			// at this point we are guaranteed a nonnegative finite number.
			assert(number >= 0 && !Double.IsInfinity(number));

			lock (DigitList)
			{
				int maxIntDigits = base.MaximumIntegerDigits;
				int minIntDigits = base.MinimumIntegerDigits;
				int maxFraDigits = base.MaximumFractionDigits;
				int minFraDigits = base.MinimumFractionDigits;

				DigitList.Set(isNegative, number, UseExponentialNotation ? maxIntDigits + maxFraDigits : maxFraDigits, !UseExponentialNotation);
				return Subformat(result, @delegate, isNegative, false, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits);
			}
		}

		/// <summary>
		/// Format a long to produce a string. </summary>
		/// <param name="number">    The long to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="fieldPosition">    On input: an alignment field, if desired.
		/// On output: the offsets of the alignment field. </param>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                  mode being set to RoundingMode.UNNECESSARY </exception>
		/// <returns> The formatted number string </returns>
		/// <seealso cref= java.text.FieldPosition </seealso>
		public override StringBuffer Format(long number, StringBuffer result, FieldPosition fieldPosition)
		{
			fieldPosition.BeginIndex = 0;
			fieldPosition.EndIndex = 0;

			return Format(number, result, fieldPosition.FieldDelegate);
		}

		/// <summary>
		/// Format a long to produce a string. </summary>
		/// <param name="number">    The long to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="delegate"> notified of locations of sub fields </param>
		/// <returns> The formatted number string </returns>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <seealso cref= java.text.FieldPosition </seealso>
		private StringBuffer Format(long number, StringBuffer result, FieldDelegate @delegate)
		{
			bool isNegative = (number < 0);
			if (isNegative)
			{
				number = -number;
			}

			// In general, long values always represent real finite numbers, so
			// we don't have to check for +/- Infinity or NaN.  However, there
			// is one case we have to be careful of:  The multiplier can push
			// a number near MIN_VALUE or MAX_VALUE outside the legal range.  We
			// check for this before multiplying, and if it happens we use
			// BigInteger instead.
			bool useBigInteger = false;
			if (number < 0) // This can only happen if number == Long.MIN_VALUE.
			{
				if (Multiplier_Renamed != 0)
				{
					useBigInteger = true;
				}
			}
			else if (Multiplier_Renamed != 1 && Multiplier_Renamed != 0)
			{
				long cutoff = Long.MaxValue / Multiplier_Renamed;
				if (cutoff < 0)
				{
					cutoff = -cutoff;
				}
				useBigInteger = (number > cutoff);
			}

			if (useBigInteger)
			{
				if (isNegative)
				{
					number = -number;
				}
				System.Numerics.BigInteger bigIntegerValue = System.Numerics.BigInteger.ValueOf(number);
				return Format(bigIntegerValue, result, @delegate, true);
			}

			number *= Multiplier_Renamed;
			if (number == 0)
			{
				isNegative = false;
			}
			else
			{
				if (Multiplier_Renamed < 0)
				{
					number = -number;
					isNegative = !isNegative;
				}
			}

			lock (DigitList)
			{
				int maxIntDigits = base.MaximumIntegerDigits;
				int minIntDigits = base.MinimumIntegerDigits;
				int maxFraDigits = base.MaximumFractionDigits;
				int minFraDigits = base.MinimumFractionDigits;

				DigitList.Set(isNegative, number, UseExponentialNotation ? maxIntDigits + maxFraDigits : 0);

				return Subformat(result, @delegate, isNegative, true, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits);
			}
		}

		/// <summary>
		/// Formats a BigDecimal to produce a string. </summary>
		/// <param name="number">    The BigDecimal to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="fieldPosition">    On input: an alignment field, if desired.
		/// On output: the offsets of the alignment field. </param>
		/// <returns> The formatted number string </returns>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <seealso cref= java.text.FieldPosition </seealso>
		private StringBuffer Format(decimal number, StringBuffer result, FieldPosition fieldPosition)
		{
			fieldPosition.BeginIndex = 0;
			fieldPosition.EndIndex = 0;
			return Format(number, result, fieldPosition.FieldDelegate);
		}

		/// <summary>
		/// Formats a BigDecimal to produce a string. </summary>
		/// <param name="number">    The BigDecimal to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="delegate"> notified of locations of sub fields </param>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <returns> The formatted number string </returns>
		private StringBuffer Format(decimal number, StringBuffer result, FieldDelegate @delegate)
		{
			if (Multiplier_Renamed != 1)
			{
				number = number * BigDecimalMultiplier;
			}
			bool isNegative = number.Signum() == -1;
			if (isNegative)
			{
				number = -number;
			}

			lock (DigitList)
			{
				int maxIntDigits = MaximumIntegerDigits;
				int minIntDigits = MinimumIntegerDigits;
				int maxFraDigits = MaximumFractionDigits;
				int minFraDigits = MinimumFractionDigits;
				int maximumDigits = maxIntDigits + maxFraDigits;

				DigitList.Set(isNegative, number, UseExponentialNotation ? ((maximumDigits < 0) ? Integer.MaxValue : maximumDigits) : maxFraDigits, !UseExponentialNotation);

				return Subformat(result, @delegate, isNegative, false, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits);
			}
		}

		/// <summary>
		/// Format a BigInteger to produce a string. </summary>
		/// <param name="number">    The BigInteger to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="fieldPosition">    On input: an alignment field, if desired.
		/// On output: the offsets of the alignment field. </param>
		/// <returns> The formatted number string </returns>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <seealso cref= java.text.FieldPosition </seealso>
		private StringBuffer Format(System.Numerics.BigInteger number, StringBuffer result, FieldPosition fieldPosition)
		{
			fieldPosition.BeginIndex = 0;
			fieldPosition.EndIndex = 0;

			return Format(number, result, fieldPosition.FieldDelegate, false);
		}

		/// <summary>
		/// Format a BigInteger to produce a string. </summary>
		/// <param name="number">    The BigInteger to format </param>
		/// <param name="result">    where the text is to be appended </param>
		/// <param name="delegate"> notified of locations of sub fields </param>
		/// <returns> The formatted number string </returns>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <seealso cref= java.text.FieldPosition </seealso>
		private StringBuffer Format(System.Numerics.BigInteger number, StringBuffer result, FieldDelegate @delegate, bool formatLong)
		{
			if (Multiplier_Renamed != 1)
			{
				number = number * BigIntegerMultiplier;
			}
			bool isNegative = number.signum() == -1;
			if (isNegative)
			{
				number = -number;
			}

			lock (DigitList)
			{
				int maxIntDigits, minIntDigits, maxFraDigits, minFraDigits, maximumDigits;
				if (formatLong)
				{
					maxIntDigits = base.MaximumIntegerDigits;
					minIntDigits = base.MinimumIntegerDigits;
					maxFraDigits = base.MaximumFractionDigits;
					minFraDigits = base.MinimumFractionDigits;
					maximumDigits = maxIntDigits + maxFraDigits;
				}
				else
				{
					maxIntDigits = MaximumIntegerDigits;
					minIntDigits = MinimumIntegerDigits;
					maxFraDigits = MaximumFractionDigits;
					minFraDigits = MinimumFractionDigits;
					maximumDigits = maxIntDigits + maxFraDigits;
					if (maximumDigits < 0)
					{
						maximumDigits = Integer.MaxValue;
					}
				}

				DigitList.Set(isNegative, number, UseExponentialNotation ? maximumDigits : 0);

				return Subformat(result, @delegate, isNegative, true, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits);
			}
		}

		/// <summary>
		/// Formats an Object producing an <code>AttributedCharacterIterator</code>.
		/// You can use the returned <code>AttributedCharacterIterator</code>
		/// to build the resulting String, as well as to determine information
		/// about the resulting String.
		/// <para>
		/// Each attribute key of the AttributedCharacterIterator will be of type
		/// <code>NumberFormat.Field</code>, with the attribute value being the
		/// same as the attribute key.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if obj is null. </exception>
		/// <exception cref="IllegalArgumentException"> when the Format cannot format the
		///            given object. </exception>
		/// <exception cref="ArithmeticException"> if rounding is needed with rounding
		///                   mode being set to RoundingMode.UNNECESSARY </exception>
		/// <param name="obj"> The object to format </param>
		/// <returns> AttributedCharacterIterator describing the formatted value.
		/// @since 1.4 </returns>
		public override AttributedCharacterIterator FormatToCharacterIterator(Object obj)
		{
			CharacterIteratorFieldDelegate @delegate = new CharacterIteratorFieldDelegate();
			StringBuffer sb = new StringBuffer();

			if (obj is Double || obj is Float)
			{
				Format(((Number)obj).DoubleValue(), sb, @delegate);
			}
			else if (obj is Long || obj is Integer || obj is Short || obj is Byte || obj is AtomicInteger || obj is AtomicLong)
			{
				Format(((Number)obj).LongValue(), sb, @delegate);
			}
			else if (obj is decimal)
			{
				Format((decimal)obj, sb, @delegate);
			}
			else if (obj is System.Numerics.BigInteger)
			{
				Format((System.Numerics.BigInteger)obj, sb, @delegate, false);
			}
			else if (obj == null)
			{
				throw new NullPointerException("formatToCharacterIterator must be passed non-null object");
			}
			else
			{
				throw new IllegalArgumentException("Cannot format given Object as a Number");
			}
			return @delegate.GetIterator(sb.ToString());
		}

		// ==== Begin fast-path formating logic for double =========================

		/* Fast-path formatting will be used for format(double ...) methods iff a
		 * number of conditions are met (see checkAndSetFastPathStatus()):
		 * - Only if instance properties meet the right predefined conditions.
		 * - The abs value of the double to format is <= Integer.MAX_VALUE.
		 *
		 * The basic approach is to split the binary to decimal conversion of a
		 * double value into two phases:
		 * * The conversion of the integer portion of the double.
		 * * The conversion of the fractional portion of the double
		 *   (limited to two or three digits).
		 *
		 * The isolation and conversion of the integer portion of the double is
		 * straightforward. The conversion of the fraction is more subtle and relies
		 * on some rounding properties of double to the decimal precisions in
		 * question.  Using the terminology of BigDecimal, this fast-path algorithm
		 * is applied when a double value has a magnitude less than Integer.MAX_VALUE
		 * and rounding is to nearest even and the destination format has two or
		 * three digits of *scale* (digits after the decimal point).
		 *
		 * Under a rounding to nearest even policy, the returned result is a digit
		 * string of a number in the (in this case decimal) destination format
		 * closest to the exact numerical value of the (in this case binary) input
		 * value.  If two destination format numbers are equally distant, the one
		 * with the last digit even is returned.  To compute such a correctly rounded
		 * value, some information about digits beyond the smallest returned digit
		 * position needs to be consulted.
		 *
		 * In general, a guard digit, a round digit, and a sticky *bit* are needed
		 * beyond the returned digit position.  If the discarded portion of the input
		 * is sufficiently large, the returned digit string is incremented.  In round
		 * to nearest even, this threshold to increment occurs near the half-way
		 * point between digits.  The sticky bit records if there are any remaining
		 * trailing digits of the exact input value in the new format; the sticky bit
		 * is consulted only in close to half-way rounding cases.
		 *
		 * Given the computation of the digit and bit values, rounding is then
		 * reduced to a table lookup problem.  For decimal, the even/odd cases look
		 * like this:
		 *
		 * Last   Round   Sticky
		 * 6      5       0      => 6   // exactly halfway, return even digit.
		 * 6      5       1      => 7   // a little bit more than halfway, round up.
		 * 7      5       0      => 8   // exactly halfway, round up to even.
		 * 7      5       1      => 8   // a little bit more than halfway, round up.
		 * With analogous entries for other even and odd last-returned digits.
		 *
		 * However, decimal negative powers of 5 smaller than 0.5 are *not* exactly
		 * representable as binary fraction.  In particular, 0.005 (the round limit
		 * for a two-digit scale) and 0.0005 (the round limit for a three-digit
		 * scale) are not representable. Therefore, for input values near these cases
		 * the sticky bit is known to be set which reduces the rounding logic to:
		 *
		 * Last   Round   Sticky
		 * 6      5       1      => 7   // a little bit more than halfway, round up.
		 * 7      5       1      => 8   // a little bit more than halfway, round up.
		 *
		 * In other words, if the round digit is 5, the sticky bit is known to be
		 * set.  If the round digit is something other than 5, the sticky bit is not
		 * relevant.  Therefore, some of the logic about whether or not to increment
		 * the destination *decimal* value can occur based on tests of *binary*
		 * computations of the binary input number.
		 */

		/// <summary>
		/// Check validity of using fast-path for this instance. If fast-path is valid
		/// for this instance, sets fast-path state as true and initializes fast-path
		/// utility fields as needed.
		/// 
		/// This method is supposed to be called rarely, otherwise that will break the
		/// fast-path performance. That means avoiding frequent changes of the
		/// properties of the instance, since for most properties, each time a change
		/// happens, a call to this method is needed at the next format call.
		/// 
		/// FAST-PATH RULES:
		///  Similar to the default DecimalFormat instantiation case.
		///  More precisely:
		///  - HALF_EVEN rounding mode,
		///  - isGroupingUsed() is true,
		///  - groupingSize of 3,
		///  - multiplier is 1,
		///  - Decimal separator not mandatory,
		///  - No use of exponential notation,
		///  - minimumIntegerDigits is exactly 1 and maximumIntegerDigits at least 10
		///  - For number of fractional digits, the exact values found in the default case:
		///     Currency : min = max = 2.
		///     Decimal  : min = 0. max = 3.
		/// 
		/// </summary>
		private void CheckAndSetFastPathStatus()
		{

			bool fastPathWasOn = IsFastPath;

			if ((RoundingMode_Renamed == RoundingMode.HALF_EVEN) && (GroupingUsed) && (GroupingSize_Renamed == 3) && (Multiplier_Renamed == 1) && (!DecimalSeparatorAlwaysShown_Renamed) && (!UseExponentialNotation))
			{

				// The fast-path algorithm is semi-hardcoded against
				//  minimumIntegerDigits and maximumIntegerDigits.
				IsFastPath = ((MinimumIntegerDigits_Renamed == 1) && (MaximumIntegerDigits_Renamed >= 10));

				// The fast-path algorithm is hardcoded against
				//  minimumFractionDigits and maximumFractionDigits.
				if (IsFastPath)
				{
					if (IsCurrencyFormat)
					{
						if ((MinimumFractionDigits_Renamed != 2) || (MaximumFractionDigits_Renamed != 2))
						{
							IsFastPath = false;
						}
					}
					else if ((MinimumFractionDigits_Renamed != 0) || (MaximumFractionDigits_Renamed != 3))
					{
						IsFastPath = false;
					}
				}
			}
			else
			{
				IsFastPath = false;
			}

			// Since some instance properties may have changed while still falling
			// in the fast-path case, we need to reinitialize fastPathData anyway.
			if (IsFastPath)
			{
				// We need to instantiate fastPathData if not already done.
				if (FastPathData == null)
				{
					FastPathData = new FastPathData();
				}

				// Sets up the locale specific constants used when formatting.
				// '0' is our default representation of zero.
				FastPathData.ZeroDelta = Symbols.ZeroDigit - '0';
				FastPathData.GroupingChar = Symbols.GroupingSeparator;

				// Sets up fractional constants related to currency/decimal pattern.
				FastPathData.FractionalMaxIntBound = (IsCurrencyFormat) ? 99 : 999;
				FastPathData.FractionalScaleFactor = (IsCurrencyFormat) ? 100.0d : 1000.0d;

				// Records the need for adding prefix or suffix
				FastPathData.PositiveAffixesRequired = (PositivePrefix_Renamed.Length() != 0) || (PositiveSuffix_Renamed.Length() != 0);
				FastPathData.NegativeAffixesRequired = (NegativePrefix_Renamed.Length() != 0) || (NegativeSuffix_Renamed.Length() != 0);

				// Creates a cached char container for result, with max possible size.
				int maxNbIntegralDigits = 10;
				int maxNbGroups = 3;
				int containerSize = System.Math.Max(PositivePrefix_Renamed.Length(), NegativePrefix_Renamed.Length()) + maxNbIntegralDigits + maxNbGroups + 1 + MaximumFractionDigits_Renamed + System.Math.Max(PositiveSuffix_Renamed.Length(), NegativeSuffix_Renamed.Length());

				FastPathData.FastPathContainer = new char[containerSize];

				// Sets up prefix and suffix char arrays constants.
				FastPathData.CharsPositiveSuffix = PositiveSuffix_Renamed.ToCharArray();
				FastPathData.CharsNegativeSuffix = NegativeSuffix_Renamed.ToCharArray();
				FastPathData.CharsPositivePrefix = PositivePrefix_Renamed.ToCharArray();
				FastPathData.CharsNegativePrefix = NegativePrefix_Renamed.ToCharArray();

				// Sets up fixed index positions for integral and fractional digits.
				// Sets up decimal point in cached result container.
				int longestPrefixLength = System.Math.Max(PositivePrefix_Renamed.Length(), NegativePrefix_Renamed.Length());
				int decimalPointIndex = maxNbIntegralDigits + maxNbGroups + longestPrefixLength;

				FastPathData.IntegralLastIndex = decimalPointIndex - 1;
				FastPathData.FractionalFirstIndex = decimalPointIndex + 1;
				FastPathData.FastPathContainer[decimalPointIndex] = IsCurrencyFormat ? Symbols.MonetaryDecimalSeparator : Symbols.DecimalSeparator;

			}
			else if (fastPathWasOn)
			{
				// Previous state was fast-path and is no more.
				// Resets cached array constants.
				FastPathData.FastPathContainer = null;
				FastPathData.CharsPositiveSuffix = null;
				FastPathData.CharsNegativeSuffix = null;
				FastPathData.CharsPositivePrefix = null;
				FastPathData.CharsNegativePrefix = null;
			}

			FastPathCheckNeeded = false;
		}

		/// <summary>
		/// Returns true if rounding-up must be done on {@code scaledFractionalPartAsInt},
		/// false otherwise.
		/// 
		/// This is a utility method that takes correct half-even rounding decision on
		/// passed fractional value at the scaled decimal point (2 digits for currency
		/// case and 3 for decimal case), when the approximated fractional part after
		/// scaled decimal point is exactly 0.5d.  This is done by means of exact
		/// calculations on the {@code fractionalPart} floating-point value.
		/// 
		/// This method is supposed to be called by private {@code fastDoubleFormat}
		/// method only.
		/// 
		/// The algorithms used for the exact calculations are :
		/// 
		/// The <b><i>FastTwoSum</i></b> algorithm, from T.J.Dekker, described in the
		/// papers  "<i>A  Floating-Point   Technique  for  Extending  the  Available
		/// Precision</i>"  by Dekker, and  in "<i>Adaptive  Precision Floating-Point
		/// Arithmetic and Fast Robust Geometric Predicates</i>" from J.Shewchuk.
		/// 
		/// A modified version of <b><i>Sum2S</i></b> cascaded summation described in
		/// "<i>Accurate Sum and Dot Product</i>" from Takeshi Ogita and All.  As
		/// Ogita says in this paper this is an equivalent of the Kahan-Babuska's
		/// summation algorithm because we order the terms by magnitude before summing
		/// them. For this reason we can use the <i>FastTwoSum</i> algorithm rather
		/// than the more expensive Knuth's <i>TwoSum</i>.
		/// 
		/// We do this to avoid a more expensive exact "<i>TwoProduct</i>" algorithm,
		/// like those described in Shewchuk's paper above. See comments in the code
		/// below.
		/// </summary>
		/// <param name="fractionalPart"> The  fractional value  on which  we  take rounding
		/// decision. </param>
		/// <param name="scaledFractionalPartAsInt"> The integral part of the scaled
		/// fractional value.
		/// </param>
		/// <returns> the decision that must be taken regarding half-even rounding. </returns>
		private bool ExactRoundUp(double fractionalPart, int scaledFractionalPartAsInt)
		{

			/* exactRoundUp() method is called by fastDoubleFormat() only.
			 * The precondition expected to be verified by the passed parameters is :
			 * scaledFractionalPartAsInt ==
			 *     (int) (fractionalPart * fastPathData.fractionalScaleFactor).
			 * This is ensured by fastDoubleFormat() code.
			 */

			/* We first calculate roundoff error made by fastDoubleFormat() on
			 * the scaled fractional part. We do this with exact calculation on the
			 * passed fractionalPart. Rounding decision will then be taken from roundoff.
			 */

			/* ---- TwoProduct(fractionalPart, scale factor (i.e. 1000.0d or 100.0d)).
			 *
			 * The below is an optimized exact "TwoProduct" calculation of passed
			 * fractional part with scale factor, using Ogita's Sum2S cascaded
			 * summation adapted as Kahan-Babuska equivalent by using FastTwoSum
			 * (much faster) rather than Knuth's TwoSum.
			 *
			 * We can do this because we order the summation from smallest to
			 * greatest, so that FastTwoSum can be used without any additional error.
			 *
			 * The "TwoProduct" exact calculation needs 17 flops. We replace this by
			 * a cascaded summation of FastTwoSum calculations, each involving an
			 * exact multiply by a power of 2.
			 *
			 * Doing so saves overall 4 multiplications and 1 addition compared to
			 * using traditional "TwoProduct".
			 *
			 * The scale factor is either 100 (currency case) or 1000 (decimal case).
			 * - when 1000, we replace it by (1024 - 16 - 8) = 1000.
			 * - when 100,  we replace it by (128  - 32 + 4) =  100.
			 * Every multiplication by a power of 2 (1024, 128, 32, 16, 8, 4) is exact.
			 *
			 */
			double approxMax; // Will always be positive.
			double approxMedium; // Will always be negative.
			double approxMin;

			double fastTwoSumApproximation = 0.0d;
			double fastTwoSumRoundOff = 0.0d;
			double bVirtual = 0.0d;

			if (IsCurrencyFormat)
			{
				// Scale is 100 = 128 - 32 + 4.
				// Multiply by 2**n is a shift. No roundoff. No error.
				approxMax = fractionalPart * 128.00d;
				approxMedium = - (fractionalPart * 32.00d);
				approxMin = fractionalPart * 4.00d;
			}
			else
			{
				// Scale is 1000 = 1024 - 16 - 8.
				// Multiply by 2**n is a shift. No roundoff. No error.
				approxMax = fractionalPart * 1024.00d;
				approxMedium = - (fractionalPart * 16.00d);
				approxMin = - (fractionalPart * 8.00d);
			}

			// Shewchuk/Dekker's FastTwoSum(approxMedium, approxMin).
			assert(-approxMedium >= System.Math.Abs(approxMin));
			fastTwoSumApproximation = approxMedium + approxMin;
			bVirtual = fastTwoSumApproximation - approxMedium;
			fastTwoSumRoundOff = approxMin - bVirtual;
			double approxS1 = fastTwoSumApproximation;
			double roundoffS1 = fastTwoSumRoundOff;

			// Shewchuk/Dekker's FastTwoSum(approxMax, approxS1);
			assert(approxMax >= System.Math.Abs(approxS1));
			fastTwoSumApproximation = approxMax + approxS1;
			bVirtual = fastTwoSumApproximation - approxMax;
			fastTwoSumRoundOff = approxS1 - bVirtual;
			double roundoff1000 = fastTwoSumRoundOff;
			double approx1000 = fastTwoSumApproximation;
			double roundoffTotal = roundoffS1 + roundoff1000;

			// Shewchuk/Dekker's FastTwoSum(approx1000, roundoffTotal);
			assert(approx1000 >= System.Math.Abs(roundoffTotal));
			fastTwoSumApproximation = approx1000 + roundoffTotal;
			bVirtual = fastTwoSumApproximation - approx1000;

			// Now we have got the roundoff for the scaled fractional
			double scaledFractionalRoundoff = roundoffTotal - bVirtual;

			// ---- TwoProduct(fractionalPart, scale (i.e. 1000.0d or 100.0d)) end.

			/* ---- Taking the rounding decision
			 *
			 * We take rounding decision based on roundoff and half-even rounding
			 * rule.
			 *
			 * The above TwoProduct gives us the exact roundoff on the approximated
			 * scaled fractional, and we know that this approximation is exactly
			 * 0.5d, since that has already been tested by the caller
			 * (fastDoubleFormat).
			 *
			 * Decision comes first from the sign of the calculated exact roundoff.
			 * - Since being exact roundoff, it cannot be positive with a scaled
			 *   fractional less than 0.5d, as well as negative with a scaled
			 *   fractional greater than 0.5d. That leaves us with following 3 cases.
			 * - positive, thus scaled fractional == 0.500....0fff ==> round-up.
			 * - negative, thus scaled fractional == 0.499....9fff ==> don't round-up.
			 * - is zero,  thus scaled fractioanl == 0.5 ==> half-even rounding applies :
			 *    we round-up only if the integral part of the scaled fractional is odd.
			 *
			 */
			if (scaledFractionalRoundoff > 0.0)
			{
				return true;
			}
			else if (scaledFractionalRoundoff < 0.0)
			{
				return false;
			}
			else if ((scaledFractionalPartAsInt & 1) != 0)
			{
				return true;
			}

			return false;

			// ---- Taking the rounding decision end
		}

		/// <summary>
		/// Collects integral digits from passed {@code number}, while setting
		/// grouping chars as needed. Updates {@code firstUsedIndex} accordingly.
		/// 
		/// Loops downward starting from {@code backwardIndex} position (inclusive).
		/// </summary>
		/// <param name="number">  The int value from which we collect digits. </param>
		/// <param name="digitsBuffer"> The char array container where digits and grouping chars
		///  are stored. </param>
		/// <param name="backwardIndex"> the position from which we start storing digits in
		///  digitsBuffer.
		///  </param>
		private void CollectIntegralDigits(int number, char[] digitsBuffer, int backwardIndex)
		{
			int index = backwardIndex;
			int q;
			int r;
			while (number > 999)
			{
				// Generates 3 digits per iteration.
				q = number / 1000;
				r = number - (q << 10) + (q << 4) + (q << 3); // -1024 +16 +8 = 1000.
				number = q;

				digitsBuffer[index--] = DigitArrays.DigitOnes1000[r];
				digitsBuffer[index--] = DigitArrays.DigitTens1000[r];
				digitsBuffer[index--] = DigitArrays.DigitHundreds1000[r];
				digitsBuffer[index--] = FastPathData.GroupingChar;
			}

			// Collects last 3 or less digits.
			digitsBuffer[index] = DigitArrays.DigitOnes1000[number];
			if (number > 9)
			{
				digitsBuffer[--index] = DigitArrays.DigitTens1000[number];
				if (number > 99)
				{
					digitsBuffer[--index] = DigitArrays.DigitHundreds1000[number];
				}
			}

			FastPathData.FirstUsedIndex = index;
		}

		/// <summary>
		/// Collects the 2 (currency) or 3 (decimal) fractional digits from passed
		/// {@code number}, starting at {@code startIndex} position
		/// inclusive.  There is no punctuation to set here (no grouping chars).
		/// Updates {@code fastPathData.lastFreeIndex} accordingly.
		/// 
		/// </summary>
		/// <param name="number">  The int value from which we collect digits. </param>
		/// <param name="digitsBuffer"> The char array container where digits are stored. </param>
		/// <param name="startIndex"> the position from which we start storing digits in
		///  digitsBuffer.
		///  </param>
		private void CollectFractionalDigits(int number, char[] digitsBuffer, int startIndex)
		{
			int index = startIndex;

			char digitOnes = DigitArrays.DigitOnes1000[number];
			char digitTens = DigitArrays.DigitTens1000[number];

			if (IsCurrencyFormat)
			{
				// Currency case. Always collects fractional digits.
				digitsBuffer[index++] = digitTens;
				digitsBuffer[index++] = digitOnes;
			}
			else if (number != 0)
			{
				// Decimal case. Hundreds will always be collected
				digitsBuffer[index++] = DigitArrays.DigitHundreds1000[number];

				// Ending zeros won't be collected.
				if (digitOnes != '0')
				{
					digitsBuffer[index++] = digitTens;
					digitsBuffer[index++] = digitOnes;
				}
				else if (digitTens != '0')
				{
					digitsBuffer[index++] = digitTens;
				}

			}
			else
				// This is decimal pattern and fractional part is zero.
				// We must remove decimal point from result.
			{
				index--;
			}

			FastPathData.LastFreeIndex = index;
		}

		/// <summary>
		/// Internal utility.
		/// Adds the passed {@code prefix} and {@code suffix} to {@code container}.
		/// </summary>
		/// <param name="container">  Char array container which to prepend/append the
		///  prefix/suffix. </param>
		/// <param name="prefix">     Char sequence to prepend as a prefix. </param>
		/// <param name="suffix">     Char sequence to append as a suffix.
		///  </param>
		//    private void addAffixes(boolean isNegative, char[] container) {
		private void AddAffixes(char[] container, char[] prefix, char[] suffix)
		{

			// We add affixes only if needed (affix length > 0).
			int pl = prefix.Length;
			int sl = suffix.Length;
			if (pl != 0)
			{
				PrependPrefix(prefix, pl, container);
			}
			if (sl != 0)
			{
				AppendSuffix(suffix, sl, container);
			}

		}

		/// <summary>
		/// Prepends the passed {@code prefix} chars to given result
		/// {@code container}.  Updates {@code fastPathData.firstUsedIndex}
		/// accordingly.
		/// </summary>
		/// <param name="prefix"> The prefix characters to prepend to result. </param>
		/// <param name="len"> The number of chars to prepend. </param>
		/// <param name="container"> Char array container which to prepend the prefix </param>
		private void PrependPrefix(char[] prefix, int len, char[] container)
		{

			FastPathData.FirstUsedIndex -= len;
			int startIndex = FastPathData.FirstUsedIndex;

			// If prefix to prepend is only 1 char long, just assigns this char.
			// If prefix is less or equal 4, we use a dedicated algorithm that
			//  has shown to run faster than System.arraycopy.
			// If more than 4, we use System.arraycopy.
			if (len == 1)
			{
				container[startIndex] = prefix[0];
			}
			else if (len <= 4)
			{
				int dstLower = startIndex;
				int dstUpper = dstLower + len - 1;
				int srcUpper = len - 1;
				container[dstLower] = prefix[0];
				container[dstUpper] = prefix[srcUpper];

				if (len > 2)
				{
					container[++dstLower] = prefix[1];
				}
				if (len == 4)
				{
					container[--dstUpper] = prefix[2];
				}
			}
			else
			{
				System.Array.Copy(prefix, 0, container, startIndex, len);
			}
		}

		/// <summary>
		/// Appends the passed {@code suffix} chars to given result
		/// {@code container}.  Updates {@code fastPathData.lastFreeIndex}
		/// accordingly.
		/// </summary>
		/// <param name="suffix"> The suffix characters to append to result. </param>
		/// <param name="len"> The number of chars to append. </param>
		/// <param name="container"> Char array container which to append the suffix </param>
		private void AppendSuffix(char[] suffix, int len, char[] container)
		{

			int startIndex = FastPathData.LastFreeIndex;

			// If suffix to append is only 1 char long, just assigns this char.
			// If suffix is less or equal 4, we use a dedicated algorithm that
			//  has shown to run faster than System.arraycopy.
			// If more than 4, we use System.arraycopy.
			if (len == 1)
			{
				container[startIndex] = suffix[0];
			}
			else if (len <= 4)
			{
				int dstLower = startIndex;
				int dstUpper = dstLower + len - 1;
				int srcUpper = len - 1;
				container[dstLower] = suffix[0];
				container[dstUpper] = suffix[srcUpper];

				if (len > 2)
				{
					container[++dstLower] = suffix[1];
				}
				if (len == 4)
				{
					container[--dstUpper] = suffix[2];
				}
			}
			else
			{
				System.Array.Copy(suffix, 0, container, startIndex, len);
			}

			FastPathData.LastFreeIndex += len;
		}

		/// <summary>
		/// Converts digit chars from {@code digitsBuffer} to current locale.
		/// 
		/// Must be called before adding affixes since we refer to
		/// {@code fastPathData.firstUsedIndex} and {@code fastPathData.lastFreeIndex},
		/// and do not support affixes (for speed reason).
		/// 
		/// We loop backward starting from last used index in {@code fastPathData}.
		/// </summary>
		/// <param name="digitsBuffer"> The char array container where the digits are stored. </param>
		private void LocalizeDigits(char[] digitsBuffer)
		{

			// We will localize only the digits, using the groupingSize,
			// and taking into account fractional part.

			// First take into account fractional part.
			int digitsCounter = FastPathData.LastFreeIndex - FastPathData.FractionalFirstIndex;

			// The case when there is no fractional digits.
			if (digitsCounter < 0)
			{
				digitsCounter = GroupingSize_Renamed;
			}

			// Only the digits remains to localize.
			for (int cursor = FastPathData.LastFreeIndex - 1; cursor >= FastPathData.FirstUsedIndex; cursor--)
			{
				if (digitsCounter != 0)
				{
					// This is a digit char, we must localize it.
					digitsBuffer[cursor] += FastPathData.ZeroDelta;
					digitsCounter--;
				}
				else
				{
					// Decimal separator or grouping char. Reinit counter only.
					digitsCounter = GroupingSize_Renamed;
				}
			}
		}

		/// <summary>
		/// This is the main entry point for the fast-path format algorithm.
		/// 
		/// At this point we are sure to be in the expected conditions to run it.
		/// This algorithm builds the formatted result and puts it in the dedicated
		/// {@code fastPathData.fastPathContainer}.
		/// </summary>
		/// <param name="d"> the double value to be formatted. </param>
		/// <param name="negative"> Flag precising if {@code d} is negative. </param>
		private void FastDoubleFormat(double d, bool negative)
		{

			char[] container = FastPathData.FastPathContainer;

			/*
			 * The principle of the algorithm is to :
			 * - Break the passed double into its integral and fractional parts
			 *    converted into integers.
			 * - Then decide if rounding up must be applied or not by following
			 *    the half-even rounding rule, first using approximated scaled
			 *    fractional part.
			 * - For the difficult cases (approximated scaled fractional part
			 *    being exactly 0.5d), we refine the rounding decision by calling
			 *    exactRoundUp utility method that both calculates the exact roundoff
			 *    on the approximation and takes correct rounding decision.
			 * - We round-up the fractional part if needed, possibly propagating the
			 *    rounding to integral part if we meet a "all-nine" case for the
			 *    scaled fractional part.
			 * - We then collect digits from the resulting integral and fractional
			 *   parts, also setting the required grouping chars on the fly.
			 * - Then we localize the collected digits if needed, and
			 * - Finally prepend/append prefix/suffix if any is needed.
			 */

			// Exact integral part of d.
			int integralPartAsInt = (int) d;

			// Exact fractional part of d (since we subtract it's integral part).
			double exactFractionalPart = d - (double) integralPartAsInt;

			// Approximated scaled fractional part of d (due to multiplication).
			double scaledFractional = exactFractionalPart * FastPathData.FractionalScaleFactor;

			// Exact integral part of scaled fractional above.
			int fractionalPartAsInt = (int) scaledFractional;

			// Exact fractional part of scaled fractional above.
			scaledFractional = scaledFractional - (double) fractionalPartAsInt;

			// Only when scaledFractional is exactly 0.5d do we have to do exact
			// calculations and take fine-grained rounding decision, since
			// approximated results above may lead to incorrect decision.
			// Otherwise comparing against 0.5d (strictly greater or less) is ok.
			bool roundItUp = false;
			if (scaledFractional >= 0.5d)
			{
				if (scaledFractional == 0.5d)
				{
					// Rounding need fine-grained decision.
					roundItUp = ExactRoundUp(exactFractionalPart, fractionalPartAsInt);
				}
				else
				{
					roundItUp = true;
				}

				if (roundItUp)
				{
					// Rounds up both fractional part (and also integral if needed).
					if (fractionalPartAsInt < FastPathData.FractionalMaxIntBound)
					{
						fractionalPartAsInt++;
					}
					else
					{
						// Propagates rounding to integral part since "all nines" case.
						fractionalPartAsInt = 0;
						integralPartAsInt++;
					}
				}
			}

			// Collecting digits.
			CollectFractionalDigits(fractionalPartAsInt, container, FastPathData.FractionalFirstIndex);
			CollectIntegralDigits(integralPartAsInt, container, FastPathData.IntegralLastIndex);

			// Localizing digits.
			if (FastPathData.ZeroDelta != 0)
			{
				LocalizeDigits(container);
			}

			// Adding prefix and suffix.
			if (negative)
			{
				if (FastPathData.NegativeAffixesRequired)
				{
					AddAffixes(container, FastPathData.CharsNegativePrefix, FastPathData.CharsNegativeSuffix);
				}
			}
			else if (FastPathData.PositiveAffixesRequired)
			{
				AddAffixes(container, FastPathData.CharsPositivePrefix, FastPathData.CharsPositiveSuffix);
			}
		}

		/// <summary>
		/// A fast-path shortcut of format(double) to be called by NumberFormat, or by
		/// format(double, ...) public methods.
		/// 
		/// If instance can be applied fast-path and passed double is not NaN or
		/// Infinity, is in the integer range, we call {@code fastDoubleFormat}
		/// after changing {@code d} to its positive value if necessary.
		/// 
		/// Otherwise returns null by convention since fast-path can't be exercized.
		/// </summary>
		/// <param name="d"> The double value to be formatted
		/// </param>
		/// <returns> the formatted result for {@code d} as a string. </returns>
		internal override String FastFormat(double d)
		{
			// (Re-)Evaluates fast-path status if needed.
			if (FastPathCheckNeeded)
			{
				CheckAndSetFastPathStatus();
			}

			if (!IsFastPath)
			{
				// DecimalFormat instance is not in a fast-path state.
				return null;
			}

			if (!Double.IsFinite(d))
			{
				// Should not use fast-path for Infinity and NaN.
				return null;
			}

			// Extracts and records sign of double value, possibly changing it
			// to a positive one, before calling fastDoubleFormat().
			bool negative = false;
			if (d < 0.0d)
			{
				negative = true;
				d = -d;
			}
			else if (d == 0.0d)
			{
				negative = (Math.CopySign(1.0d, d) == -1.0d);
				d = +0.0d;
			}

			if (d > MAX_INT_AS_DOUBLE)
			{
				// Filters out values that are outside expected fast-path range
				return null;
			}
			else
			{
				FastDoubleFormat(d, negative);
			}

			// Returns a new string from updated fastPathContainer.
			return new String(FastPathData.FastPathContainer, FastPathData.FirstUsedIndex, FastPathData.LastFreeIndex - FastPathData.FirstUsedIndex);

		}

		// ======== End fast-path formating logic for double =========================

		/// <summary>
		/// Complete the formatting of a finite number.  On entry, the digitList must
		/// be filled in with the correct digits.
		/// </summary>
		private StringBuffer Subformat(StringBuffer result, FieldDelegate @delegate, bool isNegative, bool isInteger, int maxIntDigits, int minIntDigits, int maxFraDigits, int minFraDigits)
		{
			// NOTE: This isn't required anymore because DigitList takes care of this.
			//
			//  // The negative of the exponent represents the number of leading
			//  // zeros between the decimal and the first non-zero digit, for
			//  // a value < 0.1 (e.g., for 0.00123, -fExponent == 2).  If this
			//  // is more than the maximum fraction digits, then we have an underflow
			//  // for the printed representation.  We recognize this here and set
			//  // the DigitList representation to zero in this situation.
			//
			//  if (-digitList.decimalAt >= getMaximumFractionDigits())
			//  {
			//      digitList.count = 0;
			//  }

			char zero = Symbols.ZeroDigit;
			int zeroDelta = zero - '0'; // '0' is the DigitList representation of zero
			char grouping = Symbols.GroupingSeparator;
			char @decimal = IsCurrencyFormat ? Symbols.MonetaryDecimalSeparator : Symbols.DecimalSeparator;

			/* Per bug 4147706, DecimalFormat must respect the sign of numbers which
			 * format as zero.  This allows sensible computations and preserves
			 * relations such as signum(1/x) = signum(x), where x is +Infinity or
			 * -Infinity.  Prior to this fix, we always formatted zero values as if
			 * they were positive.  Liu 7/6/98.
			 */
			if (DigitList.Zero)
			{
				DigitList.DecimalAt = 0; // Normalize
			}

			if (isNegative)
			{
				Append(result, NegativePrefix_Renamed, @delegate, NegativePrefixFieldPositions, Field.SIGN);
			}
			else
			{
				Append(result, PositivePrefix_Renamed, @delegate, PositivePrefixFieldPositions, Field.SIGN);
			}

			if (UseExponentialNotation)
			{
				int iFieldStart = result.Length();
				int iFieldEnd = -1;
				int fFieldStart = -1;

				// Minimum integer digits are handled in exponential format by
				// adjusting the exponent.  For example, 0.01234 with 3 minimum
				// integer digits is "123.4E-4".

				// Maximum integer digits are interpreted as indicating the
				// repeating range.  This is useful for engineering notation, in
				// which the exponent is restricted to a multiple of 3.  For
				// example, 0.01234 with 3 maximum integer digits is "12.34e-3".
				// If maximum integer digits are > 1 and are larger than
				// minimum integer digits, then minimum integer digits are
				// ignored.
				int exponent = DigitList.DecimalAt;
				int repeat = maxIntDigits;
				int minimumIntegerDigits = minIntDigits;
				if (repeat > 1 && repeat > minIntDigits)
				{
					// A repeating range is defined; adjust to it as follows.
					// If repeat == 3, we have 6,5,4=>3; 3,2,1=>0; 0,-1,-2=>-3;
					// -3,-4,-5=>-6, etc. This takes into account that the
					// exponent we have here is off by one from what we expect;
					// it is for the format 0.MMMMMx10^n.
					if (exponent >= 1)
					{
						exponent = ((exponent - 1) / repeat) * repeat;
					}
					else
					{
						// integer division rounds towards 0
						exponent = ((exponent - repeat) / repeat) * repeat;
					}
					minimumIntegerDigits = 1;
				}
				else
				{
					// No repeating range is defined; use minimum integer digits.
					exponent -= minimumIntegerDigits;
				}

				// We now output a minimum number of digits, and more if there
				// are more digits, up to the maximum number of digits.  We
				// place the decimal point after the "integer" digits, which
				// are the first (decimalAt - exponent) digits.
				int minimumDigits = minIntDigits + minFraDigits;
				if (minimumDigits < 0) // overflow?
				{
					minimumDigits = Integer.MaxValue;
				}

				// The number of integer digits is handled specially if the number
				// is zero, since then there may be no digits.
				int integerDigits = DigitList.Zero ? minimumIntegerDigits : DigitList.DecimalAt - exponent;
				if (minimumDigits < integerDigits)
				{
					minimumDigits = integerDigits;
				}
				int totalDigits = DigitList.Count;
				if (minimumDigits > totalDigits)
				{
					totalDigits = minimumDigits;
				}
				bool addedDecimalSeparator = false;

				for (int i = 0; i < totalDigits; ++i)
				{
					if (i == integerDigits)
					{
						// Record field information for caller.
						iFieldEnd = result.Length();

						result.Append(@decimal);
						addedDecimalSeparator = true;

						// Record field information for caller.
						fFieldStart = result.Length();
					}
					result.Append((i < DigitList.Count) ? (char)(DigitList.Digits[i] + zeroDelta) : zero);
				}

				if (DecimalSeparatorAlwaysShown_Renamed && totalDigits == integerDigits)
				{
					// Record field information for caller.
					iFieldEnd = result.Length();

					result.Append(@decimal);
					addedDecimalSeparator = true;

					// Record field information for caller.
					fFieldStart = result.Length();
				}

				// Record field information
				if (iFieldEnd == -1)
				{
					iFieldEnd = result.Length();
				}
				@delegate.Formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, iFieldEnd, result);
				if (addedDecimalSeparator)
				{
					@delegate.Formatted(Field.DECIMAL_SEPARATOR, Field.DECIMAL_SEPARATOR, iFieldEnd, fFieldStart, result);
				}
				if (fFieldStart == -1)
				{
					fFieldStart = result.Length();
				}
				@delegate.Formatted(FRACTION_FIELD, Field.FRACTION, Field.FRACTION, fFieldStart, result.Length(), result);

				// The exponent is output using the pattern-specified minimum
				// exponent digits.  There is no maximum limit to the exponent
				// digits, since truncating the exponent would result in an
				// unacceptable inaccuracy.
				int fieldStart = result.Length();

				result.Append(Symbols.ExponentSeparator);

				@delegate.Formatted(Field.EXPONENT_SYMBOL, Field.EXPONENT_SYMBOL, fieldStart, result.Length(), result);

				// For zero values, we force the exponent to zero.  We
				// must do this here, and not earlier, because the value
				// is used to determine integer digit count above.
				if (DigitList.Zero)
				{
					exponent = 0;
				}

				bool negativeExponent = exponent < 0;
				if (negativeExponent)
				{
					exponent = -exponent;
					fieldStart = result.Length();
					result.Append(Symbols.MinusSign);
					@delegate.Formatted(Field.EXPONENT_SIGN, Field.EXPONENT_SIGN, fieldStart, result.Length(), result);
				}
				DigitList.Set(negativeExponent, exponent);

				int eFieldStart = result.Length();

				for (int i = DigitList.DecimalAt; i < MinExponentDigits; ++i)
				{
					result.Append(zero);
				}
				for (int i = 0; i < DigitList.DecimalAt; ++i)
				{
					result.Append((i < DigitList.Count) ? (char)(DigitList.Digits[i] + zeroDelta) : zero);
				}
				@delegate.Formatted(Field.EXPONENT, Field.EXPONENT, eFieldStart, result.Length(), result);
			}
			else
			{
				int iFieldStart = result.Length();

				// Output the integer portion.  Here 'count' is the total
				// number of integer digits we will display, including both
				// leading zeros required to satisfy getMinimumIntegerDigits,
				// and actual digits present in the number.
				int count = minIntDigits;
				int digitIndex = 0; // Index into digitList.fDigits[]
				if (DigitList.DecimalAt > 0 && count < DigitList.DecimalAt)
				{
					count = DigitList.DecimalAt;
				}

				// Handle the case where getMaximumIntegerDigits() is smaller
				// than the real number of integer digits.  If this is so, we
				// output the least significant max integer digits.  For example,
				// the value 1997 printed with 2 max integer digits is just "97".
				if (count > maxIntDigits)
				{
					count = maxIntDigits;
					digitIndex = DigitList.DecimalAt - count;
				}

				int sizeBeforeIntegerPart = result.Length();
				for (int i = count - 1; i >= 0; --i)
				{
					if (i < DigitList.DecimalAt && digitIndex < DigitList.Count)
					{
						// Output a real digit
						result.Append((char)(DigitList.Digits[digitIndex++] + zeroDelta));
					}
					else
					{
						// Output a leading zero
						result.Append(zero);
					}

					// Output grouping separator if necessary.  Don't output a
					// grouping separator if i==0 though; that's at the end of
					// the integer part.
					if (GroupingUsed && i > 0 && (GroupingSize_Renamed != 0) && (i % GroupingSize_Renamed == 0))
					{
						int gStart = result.Length();
						result.Append(grouping);
						@delegate.Formatted(Field.GROUPING_SEPARATOR, Field.GROUPING_SEPARATOR, gStart, result.Length(), result);
					}
				}

				// Determine whether or not there are any printable fractional
				// digits.  If we've used up the digits we know there aren't.
				bool fractionPresent = (minFraDigits > 0) || (!isInteger && digitIndex < DigitList.Count);

				// If there is no fraction present, and we haven't printed any
				// integer digits, then print a zero.  Otherwise we won't print
				// _any_ digits, and we won't be able to parse this string.
				if (!fractionPresent && result.Length() == sizeBeforeIntegerPart)
				{
					result.Append(zero);
				}

				@delegate.Formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.Length(), result);

				// Output the decimal separator if we always do so.
				int sStart = result.Length();
				if (DecimalSeparatorAlwaysShown_Renamed || fractionPresent)
				{
					result.Append(@decimal);
				}

				if (sStart != result.Length())
				{
					@delegate.Formatted(Field.DECIMAL_SEPARATOR, Field.DECIMAL_SEPARATOR, sStart, result.Length(), result);
				}
				int fFieldStart = result.Length();

				for (int i = 0; i < maxFraDigits; ++i)
				{
					// Here is where we escape from the loop.  We escape if we've
					// output the maximum fraction digits (specified in the for
					// expression above).
					// We also stop when we've output the minimum digits and either:
					// we have an integer, so there is no fractional stuff to
					// display, or we're out of significant digits.
					if (i >= minFraDigits && (isInteger || digitIndex >= DigitList.Count))
					{
						break;
					}

					// Output leading fractional zeros. These are zeros that come
					// after the decimal but before any significant digits. These
					// are only output if abs(number being formatted) < 1.0.
					if (-1 - i > (DigitList.DecimalAt - 1))
					{
						result.Append(zero);
						continue;
					}

					// Output a digit, if we have any precision left, or a
					// zero if we don't.  We don't want to output noise digits.
					if (!isInteger && digitIndex < DigitList.Count)
					{
						result.Append((char)(DigitList.Digits[digitIndex++] + zeroDelta));
					}
					else
					{
						result.Append(zero);
					}
				}

				// Record field information for caller.
				@delegate.Formatted(FRACTION_FIELD, Field.FRACTION, Field.FRACTION, fFieldStart, result.Length(), result);
			}

			if (isNegative)
			{
				Append(result, NegativeSuffix_Renamed, @delegate, NegativeSuffixFieldPositions, Field.SIGN);
			}
			else
			{
				Append(result, PositiveSuffix_Renamed, @delegate, PositiveSuffixFieldPositions, Field.SIGN);
			}

			return result;
		}

		/// <summary>
		/// Appends the String <code>string</code> to <code>result</code>.
		/// <code>delegate</code> is notified of all  the
		/// <code>FieldPosition</code>s in <code>positions</code>.
		/// <para>
		/// If one of the <code>FieldPosition</code>s in <code>positions</code>
		/// identifies a <code>SIGN</code> attribute, it is mapped to
		/// <code>signAttribute</code>. This is used
		/// to map the <code>SIGN</code> attribute to the <code>EXPONENT</code>
		/// attribute as necessary.
		/// </para>
		/// <para>
		/// This is used by <code>subformat</code> to add the prefix/suffix.
		/// </para>
		/// </summary>
		private void Append(StringBuffer result, String @string, FieldDelegate @delegate, FieldPosition[] positions, Format.Field signAttribute)
		{
			int start = result.Length();

			if (@string.Length() > 0)
			{
				result.Append(@string);
				for (int counter = 0, max = positions.Length; counter < max; counter++)
				{
					FieldPosition fp = positions[counter];
					Format.Field attribute = fp.FieldAttribute;

					if (attribute == Field.SIGN)
					{
						attribute = signAttribute;
					}
					@delegate.Formatted(attribute, attribute, start + fp.BeginIndex, start + fp.EndIndex, result);
				}
			}
		}

		/// <summary>
		/// Parses text from a string to produce a <code>Number</code>.
		/// <para>
		/// The method attempts to parse text starting at the index given by
		/// <code>pos</code>.
		/// If parsing succeeds, then the index of <code>pos</code> is updated
		/// to the index after the last character used (parsing does not necessarily
		/// use all characters up to the end of the string), and the parsed
		/// number is returned. The updated <code>pos</code> can be used to
		/// indicate the starting point for the next call to this method.
		/// If an error occurs, then the index of <code>pos</code> is not
		/// changed, the error index of <code>pos</code> is set to the index of
		/// the character where the error occurred, and null is returned.
		/// </para>
		/// <para>
		/// The subclass returned depends on the value of <seealso cref="#isParseBigDecimal"/>
		/// as well as on the string being parsed.
		/// <ul>
		///   <li>If <code>isParseBigDecimal()</code> is false (the default),
		///       most integer values are returned as <code>Long</code>
		///       objects, no matter how they are written: <code>"17"</code> and
		///       <code>"17.000"</code> both parse to <code>Long(17)</code>.
		///       Values that cannot fit into a <code>Long</code> are returned as
		///       <code>Double</code>s. This includes values with a fractional part,
		///       infinite values, <code>NaN</code>, and the value -0.0.
		///       <code>DecimalFormat</code> does <em>not</em> decide whether to
		///       return a <code>Double</code> or a <code>Long</code> based on the
		///       presence of a decimal separator in the source string. Doing so
		///       would prevent integers that overflow the mantissa of a double,
		///       such as <code>"-9,223,372,036,854,775,808.00"</code>, from being
		///       parsed accurately.
		/// </para>
		///       <para>
		///       Callers may use the <code>Number</code> methods
		///       <code>doubleValue</code>, <code>longValue</code>, etc., to obtain
		///       the type they want.
		///   <li>If <code>isParseBigDecimal()</code> is true, values are returned
		///       as <code>BigDecimal</code> objects. The values are the ones
		///       constructed by <seealso cref="java.math.BigDecimal#BigDecimal(String)"/>
		///       for corresponding strings in locale-independent format. The
		///       special cases negative and positive infinity and NaN are returned
		///       as <code>Double</code> instances holding the values of the
		///       corresponding <code>Double</code> constants.
		/// </ul>
		/// </para>
		/// <para>
		/// <code>DecimalFormat</code> parses all Unicode characters that represent
		/// decimal digits, as defined by <code>Character.digit()</code>. In
		/// addition, <code>DecimalFormat</code> also recognizes as digits the ten
		/// consecutive characters starting with the localized zero digit defined in
		/// the <code>DecimalFormatSymbols</code> object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="text"> the string to be parsed </param>
		/// <param name="pos">  A <code>ParsePosition</code> object with index and error
		///             index information as described above. </param>
		/// <returns>     the parsed value, or <code>null</code> if the parse fails </returns>
		/// <exception cref="NullPointerException"> if <code>text</code> or
		///             <code>pos</code> is null. </exception>
		public override Number Parse(String text, ParsePosition pos)
		{
			// special case NaN
			if (text.RegionMatches(pos.Index_Renamed, Symbols.NaN, 0, Symbols.NaN.Length()))
			{
				pos.Index_Renamed = pos.Index_Renamed + Symbols.NaN.Length();
				return new Double(Double.NaN_Renamed);
			}

			bool[] status = new bool[STATUS_LENGTH];
			if (!Subparse(text, pos, PositivePrefix_Renamed, NegativePrefix_Renamed, DigitList, false, status))
			{
				return null;
			}

			// special case INFINITY
			if (status[STATUS_INFINITE])
			{
				if (status[STATUS_POSITIVE] == (Multiplier_Renamed >= 0))
				{
					return new Double(Double.PositiveInfinity);
				}
				else
				{
					return new Double(Double.NegativeInfinity);
				}
			}

			if (Multiplier_Renamed == 0)
			{
				if (DigitList.Zero)
				{
					return new Double(Double.NaN_Renamed);
				}
				else if (status[STATUS_POSITIVE])
				{
					return new Double(Double.PositiveInfinity);
				}
				else
				{
					return new Double(Double.NegativeInfinity);
				}
			}

			if (ParseBigDecimal)
			{
				decimal bigDecimalResult = DigitList.BigDecimal;

				if (Multiplier_Renamed != 1)
				{
					try
					{
						bigDecimalResult = bigDecimalResult / BigDecimalMultiplier;
					}
					catch (ArithmeticException) // non-terminating decimal expansion
					{
						bigDecimalResult = bigDecimalResult.Divide(BigDecimalMultiplier, RoundingMode_Renamed);
					}
				}

				if (!status[STATUS_POSITIVE])
				{
					bigDecimalResult = -bigDecimalResult;
				}
				return bigDecimalResult;
			}
			else
			{
				bool gotDouble = true;
				bool gotLongMinimum = false;
				double doubleResult = 0.0;
				long longResult = 0;

				// Finally, have DigitList parse the digits into a value.
				if (DigitList.FitsIntoLong(status[STATUS_POSITIVE], ParseIntegerOnly))
				{
					gotDouble = false;
					longResult = DigitList.Long;
					if (longResult < 0) // got Long.MIN_VALUE
					{
						gotLongMinimum = true;
					}
				}
				else
				{
					doubleResult = DigitList.Double;
				}

				// Divide by multiplier. We have to be careful here not to do
				// unneeded conversions between double and long.
				if (Multiplier_Renamed != 1)
				{
					if (gotDouble)
					{
						doubleResult /= Multiplier_Renamed;
					}
					else
					{
						// Avoid converting to double if we can
						if (longResult % Multiplier_Renamed == 0)
						{
							longResult /= Multiplier_Renamed;
						}
						else
						{
							doubleResult = ((double)longResult) / Multiplier_Renamed;
							gotDouble = true;
						}
					}
				}

				if (!status[STATUS_POSITIVE] && !gotLongMinimum)
				{
					doubleResult = -doubleResult;
					longResult = -longResult;
				}

				// At this point, if we divided the result by the multiplier, the
				// result may fit into a long.  We check for this case and return
				// a long if possible.
				// We must do this AFTER applying the negative (if appropriate)
				// in order to handle the case of LONG_MIN; otherwise, if we do
				// this with a positive value -LONG_MIN, the double is > 0, but
				// the long is < 0. We also must retain a double in the case of
				// -0.0, which will compare as == to a long 0 cast to a double
				// (bug 4162852).
				if (Multiplier_Renamed != 1 && gotDouble)
				{
					longResult = (long)doubleResult;
					gotDouble = ((doubleResult != (double)longResult) || (doubleResult == 0.0 && 1 / doubleResult < 0.0)) && !ParseIntegerOnly;
				}

				return gotDouble ? (Number)new Double(doubleResult) : (Number)new Long(longResult);
			}
		}

		/// <summary>
		/// Return a BigInteger multiplier.
		/// </summary>
		private System.Numerics.BigInteger BigIntegerMultiplier
		{
			get
			{
				if (BigIntegerMultiplier_Renamed == null)
				{
					BigIntegerMultiplier_Renamed = System.Numerics.BigInteger.ValueOf(Multiplier_Renamed);
				}
				return BigIntegerMultiplier_Renamed;
			}
		}
		[NonSerialized]
		private System.Numerics.BigInteger BigIntegerMultiplier_Renamed;

		/// <summary>
		/// Return a BigDecimal multiplier.
		/// </summary>
		private decimal BigDecimalMultiplier
		{
			get
			{
				if (BigDecimalMultiplier_Renamed == null)
				{
					BigDecimalMultiplier_Renamed = new decimal(Multiplier_Renamed);
				}
				return BigDecimalMultiplier_Renamed;
			}
		}
		[NonSerialized]
		private decimal BigDecimalMultiplier_Renamed;

		private const int STATUS_INFINITE = 0;
		private const int STATUS_POSITIVE = 1;
		private const int STATUS_LENGTH = 2;

		/// <summary>
		/// Parse the given text into a number.  The text is parsed beginning at
		/// parsePosition, until an unparseable character is seen. </summary>
		/// <param name="text"> The string to parse. </param>
		/// <param name="parsePosition"> The position at which to being parsing.  Upon
		/// return, the first unparseable character. </param>
		/// <param name="digits"> The DigitList to set to the parsed value. </param>
		/// <param name="isExponent"> If true, parse an exponent.  This means no
		/// infinite values and integer only. </param>
		/// <param name="status"> Upon return contains boolean status flags indicating
		/// whether the value was infinite and whether it was positive. </param>
		private bool Subparse(String text, ParsePosition parsePosition, String positivePrefix, String negativePrefix, DigitList digits, bool isExponent, bool[] status)
		{
			int position = parsePosition.Index_Renamed;
			int oldStart = parsePosition.Index_Renamed;
			int backup;
			bool gotPositive, gotNegative;

			// check for positivePrefix; take longest
			gotPositive = text.RegionMatches(position, positivePrefix, 0, positivePrefix.Length());
			gotNegative = text.RegionMatches(position, negativePrefix, 0, negativePrefix.Length());

			if (gotPositive && gotNegative)
			{
				if (positivePrefix.Length() > negativePrefix.Length())
				{
					gotNegative = false;
				}
				else if (positivePrefix.Length() < negativePrefix.Length())
				{
					gotPositive = false;
				}
			}

			if (gotPositive)
			{
				position += positivePrefix.Length();
			}
			else if (gotNegative)
			{
				position += negativePrefix.Length();
			}
			else
			{
				parsePosition.ErrorIndex_Renamed = position;
				return false;
			}

			// process digits or Inf, find decimal position
			status[STATUS_INFINITE] = false;
			if (!isExponent && text.RegionMatches(position,Symbols.Infinity,0, Symbols.Infinity.Length()))
			{
				position += Symbols.Infinity.Length();
				status[STATUS_INFINITE] = true;
			}
			else
			{
				// We now have a string of digits, possibly with grouping symbols,
				// and decimal points.  We want to process these into a DigitList.
				// We don't want to put a bunch of leading zeros into the DigitList
				// though, so we keep track of the location of the decimal point,
				// put only significant digits into the DigitList, and adjust the
				// exponent as needed.

				digits.DecimalAt = digits.Count = 0;
				char zero = Symbols.ZeroDigit;
				char @decimal = IsCurrencyFormat ? Symbols.MonetaryDecimalSeparator : Symbols.DecimalSeparator;
				char grouping = Symbols.GroupingSeparator;
				String exponentString = Symbols.ExponentSeparator;
				bool sawDecimal = false;
				bool sawExponent = false;
				bool sawDigit = false;
				int exponent = 0; // Set to the exponent value, if any

				// We have to track digitCount ourselves, because digits.count will
				// pin when the maximum allowable digits is reached.
				int digitCount = 0;

				backup = -1;
				for (; position < text.Length(); ++position)
				{
					char ch = text.CharAt(position);

					/* We recognize all digit ranges, not only the Latin digit range
					 * '0'..'9'.  We do so by using the Character.digit() method,
					 * which converts a valid Unicode digit to the range 0..9.
					 *
					 * The character 'ch' may be a digit.  If so, place its value
					 * from 0 to 9 in 'digit'.  First try using the locale digit,
					 * which may or MAY NOT be a standard Unicode digit range.  If
					 * this fails, try using the standard Unicode digit ranges by
					 * calling Character.digit().  If this also fails, digit will
					 * have a value outside the range 0..9.
					 */
					int digit = ch - zero;
					if (digit < 0 || digit > 9)
					{
						digit = Character.Digit(ch, 10);
					}

					if (digit == 0)
					{
						// Cancel out backup setting (see grouping handler below)
						backup = -1; // Do this BEFORE continue statement below!!!
						sawDigit = true;

						// Handle leading zeros
						if (digits.Count == 0)
						{
							// Ignore leading zeros in integer part of number.
							if (!sawDecimal)
							{
								continue;
							}

							// If we have seen the decimal, but no significant
							// digits yet, then we account for leading zeros by
							// decrementing the digits.decimalAt into negative
							// values.
							--digits.DecimalAt;
						}
						else
						{
							++digitCount;
							digits.Append((char)(digit + '0'));
						}
					} // [sic] digit==0 handled above
					else if (digit > 0 && digit <= 9)
					{
						sawDigit = true;
						++digitCount;
						digits.Append((char)(digit + '0'));

						// Cancel out backup setting (see grouping handler below)
						backup = -1;
					}
					else if (!isExponent && ch == @decimal)
					{
						// If we're only parsing integers, or if we ALREADY saw the
						// decimal, then don't parse this one.
						if (ParseIntegerOnly || sawDecimal)
						{
							break;
						}
						digits.DecimalAt = digitCount; // Not digits.count!
						sawDecimal = true;
					}
					else if (!isExponent && ch == grouping && GroupingUsed)
					{
						if (sawDecimal)
						{
							break;
						}
						// Ignore grouping characters, if we are using them, but
						// require that they be followed by a digit.  Otherwise
						// we backup and reprocess them.
						backup = position;
					}
					else if (!isExponent && text.RegionMatches(position, exponentString, 0, exponentString.Length()) && !sawExponent)
					{
						// Process the exponent by recursively calling this method.
						 ParsePosition pos = new ParsePosition(position + exponentString.Length());
						bool[] stat = new bool[STATUS_LENGTH];
						DigitList exponentDigits = new DigitList();

						if (Subparse(text, pos, "", char.ToString(Symbols.MinusSign), exponentDigits, true, stat) && exponentDigits.FitsIntoLong(stat[STATUS_POSITIVE], true))
						{
							position = pos.Index_Renamed; // Advance past the exponent
							exponent = (int)exponentDigits.Long;
							if (!stat[STATUS_POSITIVE])
							{
								exponent = -exponent;
							}
							sawExponent = true;
						}
						break; // Whether we fail or succeed, we exit this loop
					}
					else
					{
						break;
					}
				}

				if (backup != -1)
				{
					position = backup;
				}

				// If there was no decimal point we have an integer
				if (!sawDecimal)
				{
					digits.DecimalAt = digitCount; // Not digits.count!
				}

				// Adjust for exponent, if any
				digits.DecimalAt += exponent;

				// If none of the text string was recognized.  For example, parse
				// "x" with pattern "#0.00" (return index and error index both 0)
				// parse "$" with pattern "$#0.00". (return index 0 and error
				// index 1).
				if (!sawDigit && digitCount == 0)
				{
					parsePosition.Index_Renamed = oldStart;
					parsePosition.ErrorIndex_Renamed = oldStart;
					return false;
				}
			}

			// check for suffix
			if (!isExponent)
			{
				if (gotPositive)
				{
					gotPositive = text.RegionMatches(position,PositiveSuffix_Renamed,0, PositiveSuffix_Renamed.Length());
				}
				if (gotNegative)
				{
					gotNegative = text.RegionMatches(position,NegativeSuffix_Renamed,0, NegativeSuffix_Renamed.Length());
				}

			// if both match, take longest
			if (gotPositive && gotNegative)
			{
				if (PositiveSuffix_Renamed.Length() > NegativeSuffix_Renamed.Length())
				{
					gotNegative = false;
				}
				else if (PositiveSuffix_Renamed.Length() < NegativeSuffix_Renamed.Length())
				{
					gotPositive = false;
				}
			}

			// fail if neither or both
			if (gotPositive == gotNegative)
			{
				parsePosition.ErrorIndex_Renamed = position;
				return false;
			}

			parsePosition.Index_Renamed = position + (gotPositive ? PositiveSuffix_Renamed.Length() : NegativeSuffix_Renamed.Length()); // mark success!
			}
			else
			{
				parsePosition.Index_Renamed = position;
			}

			status[STATUS_POSITIVE] = gotPositive;
			if (parsePosition.Index_Renamed == oldStart)
			{
				parsePosition.ErrorIndex_Renamed = position;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns a copy of the decimal format symbols, which is generally not
		/// changed by the programmer or user. </summary>
		/// <returns> a copy of the desired DecimalFormatSymbols </returns>
		/// <seealso cref= java.text.DecimalFormatSymbols </seealso>
		public virtual DecimalFormatSymbols DecimalFormatSymbols
		{
			get
			{
				try
				{
					// don't allow multiple references
					return (DecimalFormatSymbols) Symbols.Clone();
				}
				catch (Exception)
				{
					return null; // should never happen
				}
			}
			set
			{
				try
				{
					// don't allow multiple references
					Symbols = (DecimalFormatSymbols) value.Clone();
					ExpandAffixes();
					FastPathCheckNeeded = true;
				}
				catch (Exception)
				{
					// should never happen
				}
			}
		}



		/// <summary>
		/// Get the positive prefix.
		/// <P>Examples: +123, $123, sFr123
		/// </summary>
		/// <returns> the positive prefix </returns>
		public virtual String PositivePrefix
		{
			get
			{
				return PositivePrefix_Renamed;
			}
			set
			{
				PositivePrefix_Renamed = value;
				PosPrefixPattern = null;
				PositivePrefixFieldPositions_Renamed = null;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Returns the FieldPositions of the fields in the prefix used for
		/// positive numbers. This is not used if the user has explicitly set
		/// a positive prefix via <code>setPositivePrefix</code>. This is
		/// lazily created.
		/// </summary>
		/// <returns> FieldPositions in positive prefix </returns>
		private FieldPosition[] PositivePrefixFieldPositions
		{
			get
			{
				if (PositivePrefixFieldPositions_Renamed == null)
				{
					if (PosPrefixPattern != null)
					{
						PositivePrefixFieldPositions_Renamed = ExpandAffix(PosPrefixPattern);
					}
					else
					{
						PositivePrefixFieldPositions_Renamed = EmptyFieldPositionArray;
					}
				}
				return PositivePrefixFieldPositions_Renamed;
			}
		}

		/// <summary>
		/// Get the negative prefix.
		/// <P>Examples: -123, ($123) (with negative suffix), sFr-123
		/// </summary>
		/// <returns> the negative prefix </returns>
		public virtual String NegativePrefix
		{
			get
			{
				return NegativePrefix_Renamed;
			}
			set
			{
				NegativePrefix_Renamed = value;
				NegPrefixPattern = null;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Returns the FieldPositions of the fields in the prefix used for
		/// negative numbers. This is not used if the user has explicitly set
		/// a negative prefix via <code>setNegativePrefix</code>. This is
		/// lazily created.
		/// </summary>
		/// <returns> FieldPositions in positive prefix </returns>
		private FieldPosition[] NegativePrefixFieldPositions
		{
			get
			{
				if (NegativePrefixFieldPositions_Renamed == null)
				{
					if (NegPrefixPattern != null)
					{
						NegativePrefixFieldPositions_Renamed = ExpandAffix(NegPrefixPattern);
					}
					else
					{
						NegativePrefixFieldPositions_Renamed = EmptyFieldPositionArray;
					}
				}
				return NegativePrefixFieldPositions_Renamed;
			}
		}

		/// <summary>
		/// Get the positive suffix.
		/// <P>Example: 123%
		/// </summary>
		/// <returns> the positive suffix </returns>
		public virtual String PositiveSuffix
		{
			get
			{
				return PositiveSuffix_Renamed;
			}
			set
			{
				PositiveSuffix_Renamed = value;
				PosSuffixPattern = null;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Returns the FieldPositions of the fields in the suffix used for
		/// positive numbers. This is not used if the user has explicitly set
		/// a positive suffix via <code>setPositiveSuffix</code>. This is
		/// lazily created.
		/// </summary>
		/// <returns> FieldPositions in positive prefix </returns>
		private FieldPosition[] PositiveSuffixFieldPositions
		{
			get
			{
				if (PositiveSuffixFieldPositions_Renamed == null)
				{
					if (PosSuffixPattern != null)
					{
						PositiveSuffixFieldPositions_Renamed = ExpandAffix(PosSuffixPattern);
					}
					else
					{
						PositiveSuffixFieldPositions_Renamed = EmptyFieldPositionArray;
					}
				}
				return PositiveSuffixFieldPositions_Renamed;
			}
		}

		/// <summary>
		/// Get the negative suffix.
		/// <P>Examples: -123%, ($123) (with positive suffixes)
		/// </summary>
		/// <returns> the negative suffix </returns>
		public virtual String NegativeSuffix
		{
			get
			{
				return NegativeSuffix_Renamed;
			}
			set
			{
				NegativeSuffix_Renamed = value;
				NegSuffixPattern = null;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Returns the FieldPositions of the fields in the suffix used for
		/// negative numbers. This is not used if the user has explicitly set
		/// a negative suffix via <code>setNegativeSuffix</code>. This is
		/// lazily created.
		/// </summary>
		/// <returns> FieldPositions in positive prefix </returns>
		private FieldPosition[] NegativeSuffixFieldPositions
		{
			get
			{
				if (NegativeSuffixFieldPositions_Renamed == null)
				{
					if (NegSuffixPattern != null)
					{
						NegativeSuffixFieldPositions_Renamed = ExpandAffix(NegSuffixPattern);
					}
					else
					{
						NegativeSuffixFieldPositions_Renamed = EmptyFieldPositionArray;
					}
				}
				return NegativeSuffixFieldPositions_Renamed;
			}
		}

		/// <summary>
		/// Gets the multiplier for use in percent, per mille, and similar
		/// formats.
		/// </summary>
		/// <returns> the multiplier </returns>
		/// <seealso cref= #setMultiplier(int) </seealso>
		public virtual int Multiplier
		{
			get
			{
				return Multiplier_Renamed;
			}
			set
			{
				Multiplier_Renamed = value;
				BigDecimalMultiplier_Renamed = null;
				BigIntegerMultiplier_Renamed = null;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override bool GroupingUsed
		{
			set
			{
				base.GroupingUsed = value;
				FastPathCheckNeeded = true;
			}
		}

		/// <summary>
		/// Return the grouping size. Grouping size is the number of digits between
		/// grouping separators in the integer portion of a number.  For example,
		/// in the number "123,456.78", the grouping size is 3.
		/// </summary>
		/// <returns> the grouping size </returns>
		/// <seealso cref= #setGroupingSize </seealso>
		/// <seealso cref= java.text.NumberFormat#isGroupingUsed </seealso>
		/// <seealso cref= java.text.DecimalFormatSymbols#getGroupingSeparator </seealso>
		public virtual int GroupingSize
		{
			get
			{
				return GroupingSize_Renamed;
			}
			set
			{
				GroupingSize_Renamed = (sbyte)value;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Allows you to get the behavior of the decimal separator with integers.
		/// (The decimal separator will always appear with decimals.)
		/// <P>Example: Decimal ON: 12345 &rarr; 12345.; OFF: 12345 &rarr; 12345
		/// </summary>
		/// <returns> {@code true} if the decimal separator is always shown;
		///         {@code false} otherwise </returns>
		public virtual bool DecimalSeparatorAlwaysShown
		{
			get
			{
				return DecimalSeparatorAlwaysShown_Renamed;
			}
			set
			{
				DecimalSeparatorAlwaysShown_Renamed = value;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Returns whether the <seealso cref="#parse(java.lang.String, java.text.ParsePosition)"/>
		/// method returns <code>BigDecimal</code>. The default value is false.
		/// </summary>
		/// <returns> {@code true} if the parse method returns BigDecimal;
		///         {@code false} otherwise </returns>
		/// <seealso cref= #setParseBigDecimal
		/// @since 1.5 </seealso>
		public virtual bool ParseBigDecimal
		{
			get
			{
				return ParseBigDecimal_Renamed;
			}
			set
			{
				ParseBigDecimal_Renamed = value;
			}
		}


		/// <summary>
		/// Standard override; no change in semantics.
		/// </summary>
		public override Object Clone()
		{
			DecimalFormat other = (DecimalFormat) base.Clone();
			other.Symbols = (DecimalFormatSymbols) Symbols.Clone();
			other.DigitList = (DigitList) DigitList.Clone();

			// Fast-path is almost stateless algorithm. The only logical state is the
			// isFastPath flag. In addition fastPathCheckNeeded is a sentinel flag
			// that forces recalculation of all fast-path fields when set to true.
			//
			// There is thus no need to clone all the fast-path fields.
			// We just only need to set fastPathCheckNeeded to true when cloning,
			// and init fastPathData to null as if it were a truly new instance.
			// Every fast-path field will be recalculated (only once) at next usage of
			// fast-path algorithm.
			other.FastPathCheckNeeded = true;
			other.IsFastPath = false;
			other.FastPathData = null;

			return other;
		}

		/// <summary>
		/// Overrides equals
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false; // super does class check
			}
			DecimalFormat other = (DecimalFormat) obj;
			return ((PosPrefixPattern == other.PosPrefixPattern && PositivePrefix_Renamed.Equals(other.PositivePrefix_Renamed)) || (PosPrefixPattern != null && PosPrefixPattern.Equals(other.PosPrefixPattern))) && ((PosSuffixPattern == other.PosSuffixPattern && PositiveSuffix_Renamed.Equals(other.PositiveSuffix_Renamed)) || (PosSuffixPattern != null && PosSuffixPattern.Equals(other.PosSuffixPattern))) && ((NegPrefixPattern == other.NegPrefixPattern && NegativePrefix_Renamed.Equals(other.NegativePrefix_Renamed)) || (NegPrefixPattern != null && NegPrefixPattern.Equals(other.NegPrefixPattern))) && ((NegSuffixPattern == other.NegSuffixPattern && NegativeSuffix_Renamed.Equals(other.NegativeSuffix_Renamed)) || (NegSuffixPattern != null && NegSuffixPattern.Equals(other.NegSuffixPattern))) && Multiplier_Renamed == other.Multiplier_Renamed && GroupingSize_Renamed == other.GroupingSize_Renamed && DecimalSeparatorAlwaysShown_Renamed == other.DecimalSeparatorAlwaysShown_Renamed && ParseBigDecimal_Renamed == other.ParseBigDecimal_Renamed && UseExponentialNotation == other.UseExponentialNotation && (!UseExponentialNotation || MinExponentDigits == other.MinExponentDigits) && MaximumIntegerDigits_Renamed == other.MaximumIntegerDigits_Renamed && MinimumIntegerDigits_Renamed == other.MinimumIntegerDigits_Renamed && MaximumFractionDigits_Renamed == other.MaximumFractionDigits_Renamed && MinimumFractionDigits_Renamed == other.MinimumFractionDigits_Renamed && RoundingMode_Renamed == other.RoundingMode_Renamed && Symbols.Equals(other.Symbols);
		}

		/// <summary>
		/// Overrides hashCode
		/// </summary>
		public override int HashCode()
		{
			return base.HashCode() * 37 + PositivePrefix_Renamed.HashCode();
			// just enough fields for a reasonable distribution
		}

		/// <summary>
		/// Synthesizes a pattern string that represents the current state
		/// of this Format object.
		/// </summary>
		/// <returns> a pattern string </returns>
		/// <seealso cref= #applyPattern </seealso>
		public virtual String ToPattern()
		{
			return ToPattern(false);
		}

		/// <summary>
		/// Synthesizes a localized pattern string that represents the current
		/// state of this Format object.
		/// </summary>
		/// <returns> a localized pattern string </returns>
		/// <seealso cref= #applyPattern </seealso>
		public virtual String ToLocalizedPattern()
		{
			return ToPattern(true);
		}

		/// <summary>
		/// Expand the affix pattern strings into the expanded affix strings.  If any
		/// affix pattern string is null, do not expand it.  This method should be
		/// called any time the symbols or the affix patterns change in order to keep
		/// the expanded affix strings up to date.
		/// </summary>
		private void ExpandAffixes()
		{
			// Reuse one StringBuffer for better performance
			StringBuffer buffer = new StringBuffer();
			if (PosPrefixPattern != null)
			{
				PositivePrefix_Renamed = ExpandAffix(PosPrefixPattern, buffer);
				PositivePrefixFieldPositions_Renamed = null;
			}
			if (PosSuffixPattern != null)
			{
				PositiveSuffix_Renamed = ExpandAffix(PosSuffixPattern, buffer);
				PositiveSuffixFieldPositions_Renamed = null;
			}
			if (NegPrefixPattern != null)
			{
				NegativePrefix_Renamed = ExpandAffix(NegPrefixPattern, buffer);
				NegativePrefixFieldPositions_Renamed = null;
			}
			if (NegSuffixPattern != null)
			{
				NegativeSuffix_Renamed = ExpandAffix(NegSuffixPattern, buffer);
				NegativeSuffixFieldPositions_Renamed = null;
			}
		}

		/// <summary>
		/// Expand an affix pattern into an affix string.  All characters in the
		/// pattern are literal unless prefixed by QUOTE.  The following characters
		/// after QUOTE are recognized: PATTERN_PERCENT, PATTERN_PER_MILLE,
		/// PATTERN_MINUS, and CURRENCY_SIGN.  If CURRENCY_SIGN is doubled (QUOTE +
		/// CURRENCY_SIGN + CURRENCY_SIGN), it is interpreted as an ISO 4217
		/// currency code.  Any other character after a QUOTE represents itself.
		/// QUOTE must be followed by another character; QUOTE may not occur by
		/// itself at the end of the pattern.
		/// </summary>
		/// <param name="pattern"> the non-null, possibly empty pattern </param>
		/// <param name="buffer"> a scratch StringBuffer; its contents will be lost </param>
		/// <returns> the expanded equivalent of pattern </returns>
		private String ExpandAffix(String pattern, StringBuffer buffer)
		{
			buffer.Length = 0;
			for (int i = 0; i < pattern.Length();)
			{
				char c = pattern.CharAt(i++);
				if (c == QUOTE)
				{
					c = pattern.CharAt(i++);
					switch (c)
					{
					case CURRENCY_SIGN:
						if (i < pattern.Length() && pattern.CharAt(i) == CURRENCY_SIGN)
						{
							++i;
							buffer.Append(Symbols.InternationalCurrencySymbol);
						}
						else
						{
							buffer.Append(Symbols.CurrencySymbol);
						}
						continue;
					case PATTERN_PERCENT:
						c = Symbols.Percent;
						break;
					case PATTERN_PER_MILLE:
						c = Symbols.PerMill;
						break;
					case PATTERN_MINUS:
						c = Symbols.MinusSign;
						break;
					}
				}
				buffer.Append(c);
			}
			return buffer.ToString();
		}

		/// <summary>
		/// Expand an affix pattern into an array of FieldPositions describing
		/// how the pattern would be expanded.
		/// All characters in the
		/// pattern are literal unless prefixed by QUOTE.  The following characters
		/// after QUOTE are recognized: PATTERN_PERCENT, PATTERN_PER_MILLE,
		/// PATTERN_MINUS, and CURRENCY_SIGN.  If CURRENCY_SIGN is doubled (QUOTE +
		/// CURRENCY_SIGN + CURRENCY_SIGN), it is interpreted as an ISO 4217
		/// currency code.  Any other character after a QUOTE represents itself.
		/// QUOTE must be followed by another character; QUOTE may not occur by
		/// itself at the end of the pattern.
		/// </summary>
		/// <param name="pattern"> the non-null, possibly empty pattern </param>
		/// <returns> FieldPosition array of the resulting fields. </returns>
		private FieldPosition[] ExpandAffix(String pattern)
		{
			List<FieldPosition> positions = null;
			int stringIndex = 0;
			for (int i = 0; i < pattern.Length();)
			{
				char c = pattern.CharAt(i++);
				if (c == QUOTE)
				{
					int field = -1;
					Format.Field fieldID = null;
					c = pattern.CharAt(i++);
					switch (c)
					{
					case CURRENCY_SIGN:
						String @string;
						if (i < pattern.Length() && pattern.CharAt(i) == CURRENCY_SIGN)
						{
							++i;
							@string = Symbols.InternationalCurrencySymbol;
						}
						else
						{
							@string = Symbols.CurrencySymbol;
						}
						if (@string.Length() > 0)
						{
							if (positions == null)
							{
								positions = new List<>(2);
							}
							FieldPosition fp = new FieldPosition(Field.CURRENCY);
							fp.BeginIndex = stringIndex;
							fp.EndIndex = stringIndex + @string.Length();
							positions.Add(fp);
							stringIndex += @string.Length();
						}
						continue;
					case PATTERN_PERCENT:
						c = Symbols.Percent;
						field = -1;
						fieldID = Field.PERCENT;
						break;
					case PATTERN_PER_MILLE:
						c = Symbols.PerMill;
						field = -1;
						fieldID = Field.PERMILLE;
						break;
					case PATTERN_MINUS:
						c = Symbols.MinusSign;
						field = -1;
						fieldID = Field.SIGN;
						break;
					}
					if (fieldID != null)
					{
						if (positions == null)
						{
							positions = new List<>(2);
						}
						FieldPosition fp = new FieldPosition(fieldID, field);
						fp.BeginIndex = stringIndex;
						fp.EndIndex = stringIndex + 1;
						positions.Add(fp);
					}
				}
				stringIndex++;
			}
			if (positions != null)
			{
				return positions.toArray(EmptyFieldPositionArray);
			}
			return EmptyFieldPositionArray;
		}

		/// <summary>
		/// Appends an affix pattern to the given StringBuffer, quoting special
		/// characters as needed.  Uses the internal affix pattern, if that exists,
		/// or the literal affix, if the internal affix pattern is null.  The
		/// appended string will generate the same affix pattern (or literal affix)
		/// when passed to toPattern().
		/// </summary>
		/// <param name="buffer"> the affix string is appended to this </param>
		/// <param name="affixPattern"> a pattern such as posPrefixPattern; may be null </param>
		/// <param name="expAffix"> a corresponding expanded affix, such as positivePrefix.
		/// Ignored unless affixPattern is null.  If affixPattern is null, then
		/// expAffix is appended as a literal affix. </param>
		/// <param name="localized"> true if the appended pattern should contain localized
		/// pattern characters; otherwise, non-localized pattern chars are appended </param>
		private void AppendAffix(StringBuffer buffer, String affixPattern, String expAffix, bool localized)
		{
			if (affixPattern == null)
			{
				AppendAffix(buffer, expAffix, localized);
			}
			else
			{
				int i;
				for (int pos = 0; pos < affixPattern.Length(); pos = i)
				{
					i = affixPattern.IndexOf(QUOTE, pos);
					if (i < 0)
					{
						AppendAffix(buffer, affixPattern.Substring(pos), localized);
						break;
					}
					if (i > pos)
					{
						AppendAffix(buffer, affixPattern.Substring(pos, i - pos), localized);
					}
					char c = affixPattern.CharAt(++i);
					++i;
					if (c == QUOTE)
					{
						buffer.Append(c);
						// Fall through and append another QUOTE below
					}
					else if (c == CURRENCY_SIGN && i < affixPattern.Length() && affixPattern.CharAt(i) == CURRENCY_SIGN)
					{
						++i;
						buffer.Append(c);
						// Fall through and append another CURRENCY_SIGN below
					}
					else if (localized)
					{
						switch (c)
						{
						case PATTERN_PERCENT:
							c = Symbols.Percent;
							break;
						case PATTERN_PER_MILLE:
							c = Symbols.PerMill;
							break;
						case PATTERN_MINUS:
							c = Symbols.MinusSign;
							break;
						}
					}
					buffer.Append(c);
				}
			}
		}

		/// <summary>
		/// Append an affix to the given StringBuffer, using quotes if
		/// there are special characters.  Single quotes themselves must be
		/// escaped in either case.
		/// </summary>
		private void AppendAffix(StringBuffer buffer, String affix, bool localized)
		{
			bool needQuote;
			if (localized)
			{
				needQuote = affix.IndexOf(Symbols.ZeroDigit) >= 0 || affix.IndexOf(Symbols.GroupingSeparator) >= 0 || affix.IndexOf(Symbols.DecimalSeparator) >= 0 || affix.IndexOf(Symbols.Percent) >= 0 || affix.IndexOf(Symbols.PerMill) >= 0 || affix.IndexOf(Symbols.Digit) >= 0 || affix.IndexOf(Symbols.PatternSeparator) >= 0 || affix.IndexOf(Symbols.MinusSign) >= 0 || affix.IndexOf(CURRENCY_SIGN) >= 0;
			}
			else
			{
				needQuote = affix.IndexOf(PATTERN_ZERO_DIGIT) >= 0 || affix.IndexOf(PATTERN_GROUPING_SEPARATOR) >= 0 || affix.IndexOf(PATTERN_DECIMAL_SEPARATOR) >= 0 || affix.IndexOf(PATTERN_PERCENT) >= 0 || affix.IndexOf(PATTERN_PER_MILLE) >= 0 || affix.IndexOf(PATTERN_DIGIT) >= 0 || affix.IndexOf(PATTERN_SEPARATOR) >= 0 || affix.IndexOf(PATTERN_MINUS) >= 0 || affix.IndexOf(CURRENCY_SIGN) >= 0;
			}
			if (needQuote)
			{
				buffer.Append('\'');
			}
			if (affix.IndexOf('\'') < 0)
			{
				buffer.Append(affix);
			}
			else
			{
				for (int j = 0; j < affix.Length(); ++j)
				{
					char c = affix.CharAt(j);
					buffer.Append(c);
					if (c == '\'')
					{
						buffer.Append(c);
					}
				}
			}
			if (needQuote)
			{
				buffer.Append('\'');
			}
		}

		/// <summary>
		/// Does the real work of generating a pattern.  
		/// </summary>
		private String ToPattern(bool localized)
		{
			StringBuffer result = new StringBuffer();
			for (int j = 1; j >= 0; --j)
			{
				if (j == 1)
				{
					AppendAffix(result, PosPrefixPattern, PositivePrefix_Renamed, localized);
				}
				else
				{
					AppendAffix(result, NegPrefixPattern, NegativePrefix_Renamed, localized);
				}
				int i;
				int digitCount = UseExponentialNotation ? MaximumIntegerDigits : System.Math.Max(GroupingSize_Renamed, MinimumIntegerDigits) + 1;
				for (i = digitCount; i > 0; --i)
				{
					if (i != digitCount && GroupingUsed && GroupingSize_Renamed != 0 && i % GroupingSize_Renamed == 0)
					{
						result.Append(localized ? Symbols.GroupingSeparator : PATTERN_GROUPING_SEPARATOR);
					}
					result.Append(i <= MinimumIntegerDigits ? (localized ? Symbols.ZeroDigit : PATTERN_ZERO_DIGIT) : (localized ? Symbols.Digit : PATTERN_DIGIT));
				}
				if (MaximumFractionDigits > 0 || DecimalSeparatorAlwaysShown_Renamed)
				{
					result.Append(localized ? Symbols.DecimalSeparator : PATTERN_DECIMAL_SEPARATOR);
				}
				for (i = 0; i < MaximumFractionDigits; ++i)
				{
					if (i < MinimumFractionDigits)
					{
						result.Append(localized ? Symbols.ZeroDigit : PATTERN_ZERO_DIGIT);
					}
					else
					{
						result.Append(localized ? Symbols.Digit : PATTERN_DIGIT);
					}
				}
			if (UseExponentialNotation)
			{
				result.Append(localized ? Symbols.ExponentSeparator : PATTERN_EXPONENT);
			for (i = 0; i < MinExponentDigits; ++i)
			{
						result.Append(localized ? Symbols.ZeroDigit : PATTERN_ZERO_DIGIT);
			}
			}
				if (j == 1)
				{
					AppendAffix(result, PosSuffixPattern, PositiveSuffix_Renamed, localized);
					if ((NegSuffixPattern == PosSuffixPattern && NegativeSuffix_Renamed.Equals(PositiveSuffix_Renamed)) || (NegSuffixPattern != null && NegSuffixPattern.Equals(PosSuffixPattern))) // n == p == null
					{
						if ((NegPrefixPattern != null && PosPrefixPattern != null && NegPrefixPattern.Equals("'-" + PosPrefixPattern)) || (NegPrefixPattern == PosPrefixPattern && NegativePrefix_Renamed.Equals(Symbols.MinusSign + PositivePrefix_Renamed))) // n == p == null
						{
							break;
						}
					}
					result.Append(localized ? Symbols.PatternSeparator : PATTERN_SEPARATOR);
				}
				else
				{
					AppendAffix(result, NegSuffixPattern, NegativeSuffix_Renamed, localized);
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Apply the given pattern to this Format object.  A pattern is a
		/// short-hand specification for the various formatting properties.
		/// These properties can also be changed individually through the
		/// various setter methods.
		/// <para>
		/// There is no limit to integer digits set
		/// by this routine, since that is the typical end-user desire;
		/// use setMaximumInteger if you want to set a real value.
		/// For negative numbers, use a second pattern, separated by a semicolon
		/// <P>Example <code>"#,#00.0#"</code> &rarr; 1,234.56
		/// <P>This means a minimum of 2 integer digits, 1 fraction digit, and
		/// a maximum of 2 fraction digits.
		/// </para>
		/// <para>Example: <code>"#,#00.0#;(#,#00.0#)"</code> for negatives in
		/// parentheses.
		/// </para>
		/// <para>In negative patterns, the minimum and maximum counts are ignored;
		/// these are presumed to be set in the positive pattern.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a new pattern </param>
		/// <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		public virtual void ApplyPattern(String pattern)
		{
			ApplyPattern(pattern, false);
		}

		/// <summary>
		/// Apply the given pattern to this Format object.  The pattern
		/// is assumed to be in a localized notation. A pattern is a
		/// short-hand specification for the various formatting properties.
		/// These properties can also be changed individually through the
		/// various setter methods.
		/// <para>
		/// There is no limit to integer digits set
		/// by this routine, since that is the typical end-user desire;
		/// use setMaximumInteger if you want to set a real value.
		/// For negative numbers, use a second pattern, separated by a semicolon
		/// <P>Example <code>"#,#00.0#"</code> &rarr; 1,234.56
		/// <P>This means a minimum of 2 integer digits, 1 fraction digit, and
		/// a maximum of 2 fraction digits.
		/// </para>
		/// <para>Example: <code>"#,#00.0#;(#,#00.0#)"</code> for negatives in
		/// parentheses.
		/// </para>
		/// <para>In negative patterns, the minimum and maximum counts are ignored;
		/// these are presumed to be set in the positive pattern.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a new pattern </param>
		/// <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		public virtual void ApplyLocalizedPattern(String pattern)
		{
			ApplyPattern(pattern, true);
		}

		/// <summary>
		/// Does the real work of applying a pattern.
		/// </summary>
		private void ApplyPattern(String pattern, bool localized)
		{
			char zeroDigit = PATTERN_ZERO_DIGIT;
			char groupingSeparator = PATTERN_GROUPING_SEPARATOR;
			char decimalSeparator = PATTERN_DECIMAL_SEPARATOR;
			char percent = PATTERN_PERCENT;
			char perMill = PATTERN_PER_MILLE;
			char digit = PATTERN_DIGIT;
			char separator = PATTERN_SEPARATOR;
			String exponent = PATTERN_EXPONENT;
			char minus = PATTERN_MINUS;
			if (localized)
			{
				zeroDigit = Symbols.ZeroDigit;
				groupingSeparator = Symbols.GroupingSeparator;
				decimalSeparator = Symbols.DecimalSeparator;
				percent = Symbols.Percent;
				perMill = Symbols.PerMill;
				digit = Symbols.Digit;
				separator = Symbols.PatternSeparator;
				exponent = Symbols.ExponentSeparator;
				minus = Symbols.MinusSign;
			}
			bool gotNegative = false;
			DecimalSeparatorAlwaysShown_Renamed = false;
			IsCurrencyFormat = false;
			UseExponentialNotation = false;

			// Two variables are used to record the subrange of the pattern
			// occupied by phase 1.  This is used during the processing of the
			// second pattern (the one representing negative numbers) to ensure
			// that no deviation exists in phase 1 between the two patterns.
			int phaseOneStart = 0;
			int phaseOneLength = 0;

			int start = 0;
			for (int j = 1; j >= 0 && start < pattern.Length(); --j)
			{
				bool inQuote = false;
				StringBuffer prefix = new StringBuffer();
				StringBuffer suffix = new StringBuffer();
				int decimalPos = -1;
				int multiplier = 1;
				int digitLeftCount = 0, zeroDigitCount = 0, digitRightCount = 0;
				sbyte groupingCount = -1;

				// The phase ranges from 0 to 2.  Phase 0 is the prefix.  Phase 1 is
				// the section of the pattern with digits, decimal separator,
				// grouping characters.  Phase 2 is the suffix.  In phases 0 and 2,
				// percent, per mille, and currency symbols are recognized and
				// translated.  The separation of the characters into phases is
				// strictly enforced; if phase 1 characters are to appear in the
				// suffix, for example, they must be quoted.
				int phase = 0;

				// The affix is either the prefix or the suffix.
				StringBuffer affix = prefix;

				for (int pos = start; pos < pattern.Length(); ++pos)
				{
					char ch = pattern.CharAt(pos);
					switch (phase)
					{
					case 0:
					case 2:
						// Process the prefix / suffix characters
						if (inQuote)
						{
							// A quote within quotes indicates either the closing
							// quote or two quotes, which is a quote literal. That
							// is, we have the second quote in 'do' or 'don''t'.
							if (ch == QUOTE)
							{
								if ((pos + 1) < pattern.Length() && pattern.CharAt(pos + 1) == QUOTE)
								{
									++pos;
									affix.Append("''"); // 'don''t'
								}
								else
								{
									inQuote = false; // 'do'
								}
								continue;
							}
						}
						else
						{
							// Process unquoted characters seen in prefix or suffix
							// phase.
							if (ch == digit || ch == zeroDigit || ch == groupingSeparator || ch == decimalSeparator)
							{
								phase = 1;
								if (j == 1)
								{
									phaseOneStart = pos;
								}
								--pos; // Reprocess this character
								continue;
							}
							else if (ch == CURRENCY_SIGN)
							{
								// Use lookahead to determine if the currency sign
								// is doubled or not.
								bool doubled = (pos + 1) < pattern.Length() && pattern.CharAt(pos + 1) == CURRENCY_SIGN;
								if (doubled) // Skip over the doubled character
								{
								 ++pos;
								}
								IsCurrencyFormat = true;
								affix.Append(doubled ? "'\u00A4\u00A4" : "'\u00A4");
								continue;
							}
							else if (ch == QUOTE)
							{
								// A quote outside quotes indicates either the
								// opening quote or two quotes, which is a quote
								// literal. That is, we have the first quote in 'do'
								// or o''clock.
								if (ch == QUOTE)
								{
									if ((pos + 1) < pattern.Length() && pattern.CharAt(pos + 1) == QUOTE)
									{
										++pos;
										affix.Append("''"); // o''clock
									}
									else
									{
										inQuote = true; // 'do'
									}
									continue;
								}
							}
							else if (ch == separator)
							{
								// Don't allow separators before we see digit
								// characters of phase 1, and don't allow separators
								// in the second pattern (j == 0).
								if (phase == 0 || j == 0)
								{
									throw new IllegalArgumentException("Unquoted special character '" + ch + "' in pattern \"" + pattern + '"');
								}
								start = pos + 1;
								pos = pattern.Length();
								continue;
							}

							// Next handle characters which are appended directly.
							else if (ch == percent)
							{
								if (multiplier != 1)
								{
									throw new IllegalArgumentException("Too many percent/per mille characters in pattern \"" + pattern + '"');
								}
								multiplier = 100;
								affix.Append("'%");
								continue;
							}
							else if (ch == perMill)
							{
								if (multiplier != 1)
								{
									throw new IllegalArgumentException("Too many percent/per mille characters in pattern \"" + pattern + '"');
								}
								multiplier = 1000;
								affix.Append("'\u2030");
								continue;
							}
							else if (ch == minus)
							{
								affix.Append("'-");
								continue;
							}
						}
						// Note that if we are within quotes, or if this is an
						// unquoted, non-special character, then we usually fall
						// through to here.
						affix.Append(ch);
						break;

					case 1:
						// Phase one must be identical in the two sub-patterns. We
						// enforce this by doing a direct comparison. While
						// processing the first sub-pattern, we just record its
						// length. While processing the second, we compare
						// characters.
						if (j == 1)
						{
							++phaseOneLength;
						}
						else
						{
							if (--phaseOneLength == 0)
							{
								phase = 2;
								affix = suffix;
							}
							continue;
						}

						// Process the digits, decimal, and grouping characters. We
						// record five pieces of information. We expect the digits
						// to occur in the pattern ####0000.####, and we record the
						// number of left digits, zero (central) digits, and right
						// digits. The position of the last grouping character is
						// recorded (should be somewhere within the first two blocks
						// of characters), as is the position of the decimal point,
						// if any (should be in the zero digits). If there is no
						// decimal point, then there should be no right digits.
						if (ch == digit)
						{
							if (zeroDigitCount > 0)
							{
								++digitRightCount;
							}
							else
							{
								++digitLeftCount;
							}
							if (groupingCount >= 0 && decimalPos < 0)
							{
								++groupingCount;
							}
						}
						else if (ch == zeroDigit)
						{
							if (digitRightCount > 0)
							{
								throw new IllegalArgumentException("Unexpected '0' in pattern \"" + pattern + '"');
							}
							++zeroDigitCount;
							if (groupingCount >= 0 && decimalPos < 0)
							{
								++groupingCount;
							}
						}
						else if (ch == groupingSeparator)
						{
							groupingCount = 0;
						}
						else if (ch == decimalSeparator)
						{
							if (decimalPos >= 0)
							{
								throw new IllegalArgumentException("Multiple decimal separators in pattern \"" + pattern + '"');
							}
							decimalPos = digitLeftCount + zeroDigitCount + digitRightCount;
						}
						else if (pattern.RegionMatches(pos, exponent, 0, exponent.Length()))
						{
							if (UseExponentialNotation)
							{
								throw new IllegalArgumentException("Multiple exponential " + "symbols in pattern \"" + pattern + '"');
							}
							UseExponentialNotation = true;
							MinExponentDigits = 0;

							// Use lookahead to parse out the exponential part
							// of the pattern, then jump into phase 2.
							pos = pos + exponent.Length();
							 while (pos < pattern.Length() && pattern.CharAt(pos) == zeroDigit)
							 {
								++MinExponentDigits;
								++phaseOneLength;
								++pos;
							 }

							if ((digitLeftCount + zeroDigitCount) < 1 || MinExponentDigits < 1)
							{
								throw new IllegalArgumentException("Malformed exponential " + "pattern \"" + pattern + '"');
							}

							// Transition to phase 2
							phase = 2;
							affix = suffix;
							--pos;
							continue;
						}
						else
						{
							phase = 2;
							affix = suffix;
							--pos;
							--phaseOneLength;
							continue;
						}
						break;
					}
				}

				// Handle patterns with no '0' pattern character. These patterns
				// are legal, but must be interpreted.  "##.###" -> "#0.###".
				// ".###" -> ".0##".
				/* We allow patterns of the form "####" to produce a zeroDigitCount
				 * of zero (got that?); although this seems like it might make it
				 * possible for format() to produce empty strings, format() checks
				 * for this condition and outputs a zero digit in this situation.
				 * Having a zeroDigitCount of zero yields a minimum integer digits
				 * of zero, which allows proper round-trip patterns.  That is, we
				 * don't want "#" to become "#0" when toPattern() is called (even
				 * though that's what it really is, semantically).
				 */
				if (zeroDigitCount == 0 && digitLeftCount > 0 && decimalPos >= 0)
				{
					// Handle "###.###" and "###." and ".###"
					int n = decimalPos;
					if (n == 0) // Handle ".###"
					{
						++n;
					}
					digitRightCount = digitLeftCount - n;
					digitLeftCount = n - 1;
					zeroDigitCount = 1;
				}

				// Do syntax checking on the digits.
				if ((decimalPos < 0 && digitRightCount > 0) || (decimalPos >= 0 && (decimalPos < digitLeftCount || decimalPos > (digitLeftCount + zeroDigitCount))) || groupingCount == 0 || inQuote)
				{
					throw new IllegalArgumentException("Malformed pattern \"" + pattern + '"');
				}

				if (j == 1)
				{
					PosPrefixPattern = prefix.ToString();
					PosSuffixPattern = suffix.ToString();
					NegPrefixPattern = PosPrefixPattern; // assume these for now
					NegSuffixPattern = PosSuffixPattern;
					int digitTotalCount = digitLeftCount + zeroDigitCount + digitRightCount;
					/* The effectiveDecimalPos is the position the decimal is at or
					 * would be at if there is no decimal. Note that if decimalPos<0,
					 * then digitTotalCount == digitLeftCount + zeroDigitCount.
					 */
					int effectiveDecimalPos = decimalPos >= 0 ? decimalPos : digitTotalCount;
					MinimumIntegerDigits = effectiveDecimalPos - digitLeftCount;
					MaximumIntegerDigits = UseExponentialNotation ? digitLeftCount + MinimumIntegerDigits : MAXIMUM_INTEGER_DIGITS;
					MaximumFractionDigits = decimalPos >= 0 ? (digitTotalCount - decimalPos) : 0;
					MinimumFractionDigits = decimalPos >= 0 ? (digitLeftCount + zeroDigitCount - decimalPos) : 0;
					GroupingUsed = groupingCount > 0;
					this.GroupingSize_Renamed = (groupingCount > 0) ? groupingCount : 0;
					this.Multiplier_Renamed = multiplier;
					DecimalSeparatorAlwaysShown = decimalPos == 0 || decimalPos == digitTotalCount;
				}
				else
				{
					NegPrefixPattern = prefix.ToString();
					NegSuffixPattern = suffix.ToString();
					gotNegative = true;
				}
			}

			if (pattern.Length() == 0)
			{
				PosPrefixPattern = PosSuffixPattern = "";
				MinimumIntegerDigits = 0;
				MaximumIntegerDigits = MAXIMUM_INTEGER_DIGITS;
				MinimumFractionDigits = 0;
				MaximumFractionDigits = MAXIMUM_FRACTION_DIGITS;
			}

			// If there was no negative pattern, or if the negative pattern is
			// identical to the positive pattern, then prepend the minus sign to
			// the positive pattern to form the negative pattern.
			if (!gotNegative || (NegPrefixPattern.Equals(PosPrefixPattern) && NegSuffixPattern.Equals(PosSuffixPattern)))
			{
				NegSuffixPattern = PosSuffixPattern;
				NegPrefixPattern = "'-" + PosPrefixPattern;
			}

			ExpandAffixes();
		}

		/// <summary>
		/// Sets the maximum number of digits allowed in the integer portion of a
		/// number.
		/// For formatting numbers other than <code>BigInteger</code> and
		/// <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		/// 309 is used. Negative input values are replaced with 0. </summary>
		/// <seealso cref= NumberFormat#setMaximumIntegerDigits </seealso>
		public override int MaximumIntegerDigits
		{
			set
			{
				MaximumIntegerDigits_Renamed = System.Math.Min(System.Math.Max(0, value), MAXIMUM_INTEGER_DIGITS);
				base.MaximumIntegerDigits = (MaximumIntegerDigits_Renamed > DOUBLE_INTEGER_DIGITS) ? DOUBLE_INTEGER_DIGITS : MaximumIntegerDigits_Renamed;
				if (MinimumIntegerDigits_Renamed > MaximumIntegerDigits_Renamed)
				{
					MinimumIntegerDigits_Renamed = MaximumIntegerDigits_Renamed;
					base.MinimumIntegerDigits = (MinimumIntegerDigits_Renamed > DOUBLE_INTEGER_DIGITS) ? DOUBLE_INTEGER_DIGITS : MinimumIntegerDigits_Renamed;
				}
				FastPathCheckNeeded = true;
			}
			get
			{
				return MaximumIntegerDigits_Renamed;
			}
		}

		/// <summary>
		/// Sets the minimum number of digits allowed in the integer portion of a
		/// number.
		/// For formatting numbers other than <code>BigInteger</code> and
		/// <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		/// 309 is used. Negative input values are replaced with 0. </summary>
		/// <seealso cref= NumberFormat#setMinimumIntegerDigits </seealso>
		public override int MinimumIntegerDigits
		{
			set
			{
				MinimumIntegerDigits_Renamed = System.Math.Min(System.Math.Max(0, value), MAXIMUM_INTEGER_DIGITS);
				base.MinimumIntegerDigits = (MinimumIntegerDigits_Renamed > DOUBLE_INTEGER_DIGITS) ? DOUBLE_INTEGER_DIGITS : MinimumIntegerDigits_Renamed;
				if (MinimumIntegerDigits_Renamed > MaximumIntegerDigits_Renamed)
				{
					MaximumIntegerDigits_Renamed = MinimumIntegerDigits_Renamed;
					base.MaximumIntegerDigits = (MaximumIntegerDigits_Renamed > DOUBLE_INTEGER_DIGITS) ? DOUBLE_INTEGER_DIGITS : MaximumIntegerDigits_Renamed;
				}
				FastPathCheckNeeded = true;
			}
			get
			{
				return MinimumIntegerDigits_Renamed;
			}
		}

		/// <summary>
		/// Sets the maximum number of digits allowed in the fraction portion of a
		/// number.
		/// For formatting numbers other than <code>BigInteger</code> and
		/// <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		/// 340 is used. Negative input values are replaced with 0. </summary>
		/// <seealso cref= NumberFormat#setMaximumFractionDigits </seealso>
		public override int MaximumFractionDigits
		{
			set
			{
				MaximumFractionDigits_Renamed = System.Math.Min(System.Math.Max(0, value), MAXIMUM_FRACTION_DIGITS);
				base.MaximumFractionDigits = (MaximumFractionDigits_Renamed > DOUBLE_FRACTION_DIGITS) ? DOUBLE_FRACTION_DIGITS : MaximumFractionDigits_Renamed;
				if (MinimumFractionDigits_Renamed > MaximumFractionDigits_Renamed)
				{
					MinimumFractionDigits_Renamed = MaximumFractionDigits_Renamed;
					base.MinimumFractionDigits = (MinimumFractionDigits_Renamed > DOUBLE_FRACTION_DIGITS) ? DOUBLE_FRACTION_DIGITS : MinimumFractionDigits_Renamed;
				}
				FastPathCheckNeeded = true;
			}
			get
			{
				return MaximumFractionDigits_Renamed;
			}
		}

		/// <summary>
		/// Sets the minimum number of digits allowed in the fraction portion of a
		/// number.
		/// For formatting numbers other than <code>BigInteger</code> and
		/// <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		/// 340 is used. Negative input values are replaced with 0. </summary>
		/// <seealso cref= NumberFormat#setMinimumFractionDigits </seealso>
		public override int MinimumFractionDigits
		{
			set
			{
				MinimumFractionDigits_Renamed = System.Math.Min(System.Math.Max(0, value), MAXIMUM_FRACTION_DIGITS);
				base.MinimumFractionDigits = (MinimumFractionDigits_Renamed > DOUBLE_FRACTION_DIGITS) ? DOUBLE_FRACTION_DIGITS : MinimumFractionDigits_Renamed;
				if (MinimumFractionDigits_Renamed > MaximumFractionDigits_Renamed)
				{
					MaximumFractionDigits_Renamed = MinimumFractionDigits_Renamed;
					base.MaximumFractionDigits = (MaximumFractionDigits_Renamed > DOUBLE_FRACTION_DIGITS) ? DOUBLE_FRACTION_DIGITS : MaximumFractionDigits_Renamed;
				}
				FastPathCheckNeeded = true;
			}
			get
			{
				return MinimumFractionDigits_Renamed;
			}
		}





		/// <summary>
		/// Gets the currency used by this decimal format when formatting
		/// currency values.
		/// The currency is obtained by calling
		/// <seealso cref="DecimalFormatSymbols#getCurrency DecimalFormatSymbols.getCurrency"/>
		/// on this number format's symbols.
		/// </summary>
		/// <returns> the currency used by this decimal format, or <code>null</code>
		/// @since 1.4 </returns>
		public override Currency Currency
		{
			get
			{
				return Symbols.Currency;
			}
			set
			{
				if (value != Symbols.Currency)
				{
					Symbols.Currency = value;
					if (IsCurrencyFormat)
					{
						ExpandAffixes();
					}
				}
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Gets the <seealso cref="java.math.RoundingMode"/> used in this DecimalFormat.
		/// </summary>
		/// <returns> The <code>RoundingMode</code> used for this DecimalFormat. </returns>
		/// <seealso cref= #setRoundingMode(RoundingMode)
		/// @since 1.6 </seealso>
		public override RoundingMode RoundingMode
		{
			get
			{
				return RoundingMode_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
    
				this.RoundingMode_Renamed = value;
				DigitList.RoundingMode = value;
				FastPathCheckNeeded = true;
			}
		}


		/// <summary>
		/// Reads the default serializable fields from the stream and performs
		/// validations and adjustments for older serialized versions. The
		/// validations and adjustments are:
		/// <ol>
		/// <li>
		/// Verify that the superclass's digit count fields correctly reflect
		/// the limits imposed on formatting numbers other than
		/// <code>BigInteger</code> and <code>BigDecimal</code> objects. These
		/// limits are stored in the superclass for serialization compatibility
		/// with older versions, while the limits for <code>BigInteger</code> and
		/// <code>BigDecimal</code> objects are kept in this class.
		/// If, in the superclass, the minimum or maximum integer digit count is
		/// larger than <code>DOUBLE_INTEGER_DIGITS</code> or if the minimum or
		/// maximum fraction digit count is larger than
		/// <code>DOUBLE_FRACTION_DIGITS</code>, then the stream data is invalid
		/// and this method throws an <code>InvalidObjectException</code>.
		/// <li>
		/// If <code>serialVersionOnStream</code> is less than 4, initialize
		/// <code>roundingMode</code> to {@link java.math.RoundingMode#HALF_EVEN
		/// RoundingMode.HALF_EVEN}.  This field is new with version 4.
		/// <li>
		/// If <code>serialVersionOnStream</code> is less than 3, then call
		/// the setters for the minimum and maximum integer and fraction digits with
		/// the values of the corresponding superclass getters to initialize the
		/// fields in this class. The fields in this class are new with version 3.
		/// <li>
		/// If <code>serialVersionOnStream</code> is less than 1, indicating that
		/// the stream was written by JDK 1.1, initialize
		/// <code>useExponentialNotation</code>
		/// to false, since it was not present in JDK 1.1.
		/// <li>
		/// Set <code>serialVersionOnStream</code> to the maximum allowed value so
		/// that default serialization will work properly if this object is streamed
		/// out again.
		/// </ol>
		/// 
		/// <para>Stream versions older than 2 will not have the affix pattern variables
		/// <code>posPrefixPattern</code> etc.  As a result, they will be initialized
		/// to <code>null</code>, which means the affix strings will be taken as
		/// literal values.  This is exactly what we want, since that corresponds to
		/// the pre-version-2 behavior.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			DigitList = new DigitList();

			// We force complete fast-path reinitialization when the instance is
			// deserialized. See clone() comment on fastPathCheckNeeded.
			FastPathCheckNeeded = true;
			IsFastPath = false;
			FastPathData = null;

			if (SerialVersionOnStream < 4)
			{
				RoundingMode = RoundingMode.HALF_EVEN;
			}
			else
			{
				RoundingMode = RoundingMode;
			}

			// We only need to check the maximum counts because NumberFormat
			// .readObject has already ensured that the maximum is greater than the
			// minimum count.
			if (base.MaximumIntegerDigits > DOUBLE_INTEGER_DIGITS || base.MaximumFractionDigits > DOUBLE_FRACTION_DIGITS)
			{
				throw new InvalidObjectException("Digit count out of range");
			}
			if (SerialVersionOnStream < 3)
			{
				MaximumIntegerDigits = base.MaximumIntegerDigits;
				MinimumIntegerDigits = base.MinimumIntegerDigits;
				MaximumFractionDigits = base.MaximumFractionDigits;
				MinimumFractionDigits = base.MinimumFractionDigits;
			}
			if (SerialVersionOnStream < 1)
			{
				// Didn't have exponential fields
				UseExponentialNotation = false;
			}
			SerialVersionOnStream = CurrentSerialVersion;
		}

		//----------------------------------------------------------------------
		// INSTANCE VARIABLES
		//----------------------------------------------------------------------

		[NonSerialized]
		private DigitList DigitList = new DigitList();

		/// <summary>
		/// The symbol used as a prefix when formatting positive numbers, e.g. "+".
		/// 
		/// @serial </summary>
		/// <seealso cref= #getPositivePrefix </seealso>
		private String PositivePrefix_Renamed = "";

		/// <summary>
		/// The symbol used as a suffix when formatting positive numbers.
		/// This is often an empty string.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getPositiveSuffix </seealso>
		private String PositiveSuffix_Renamed = "";

		/// <summary>
		/// The symbol used as a prefix when formatting negative numbers, e.g. "-".
		/// 
		/// @serial </summary>
		/// <seealso cref= #getNegativePrefix </seealso>
		private String NegativePrefix_Renamed = "-";

		/// <summary>
		/// The symbol used as a suffix when formatting negative numbers.
		/// This is often an empty string.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getNegativeSuffix </seealso>
		private String NegativeSuffix_Renamed = "";

		/// <summary>
		/// The prefix pattern for non-negative numbers.  This variable corresponds
		/// to <code>positivePrefix</code>.
		/// 
		/// <para>This pattern is expanded by the method <code>expandAffix()</code> to
		/// <code>positivePrefix</code> to update the latter to reflect changes in
		/// <code>symbols</code>.  If this variable is <code>null</code> then
		/// <code>positivePrefix</code> is taken as a literal value that does not
		/// change when <code>symbols</code> changes.  This variable is always
		/// <code>null</code> for <code>DecimalFormat</code> objects older than
		/// stream version 2 restored from stream.
		/// 
		/// @serial
		/// @since 1.3
		/// </para>
		/// </summary>
		private String PosPrefixPattern;

		/// <summary>
		/// The suffix pattern for non-negative numbers.  This variable corresponds
		/// to <code>positiveSuffix</code>.  This variable is analogous to
		/// <code>posPrefixPattern</code>; see that variable for further
		/// documentation.
		/// 
		/// @serial
		/// @since 1.3
		/// </summary>
		private String PosSuffixPattern;

		/// <summary>
		/// The prefix pattern for negative numbers.  This variable corresponds
		/// to <code>negativePrefix</code>.  This variable is analogous to
		/// <code>posPrefixPattern</code>; see that variable for further
		/// documentation.
		/// 
		/// @serial
		/// @since 1.3
		/// </summary>
		private String NegPrefixPattern;

		/// <summary>
		/// The suffix pattern for negative numbers.  This variable corresponds
		/// to <code>negativeSuffix</code>.  This variable is analogous to
		/// <code>posPrefixPattern</code>; see that variable for further
		/// documentation.
		/// 
		/// @serial
		/// @since 1.3
		/// </summary>
		private String NegSuffixPattern;

		/// <summary>
		/// The multiplier for use in percent, per mille, etc.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMultiplier </seealso>
		private int Multiplier_Renamed = 1;

		/// <summary>
		/// The number of digits between grouping separators in the integer
		/// portion of a number.  Must be greater than 0 if
		/// <code>NumberFormat.groupingUsed</code> is true.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getGroupingSize </seealso>
		/// <seealso cref= java.text.NumberFormat#isGroupingUsed </seealso>
		private sbyte GroupingSize_Renamed = 3; // invariant, > 0 if useThousands

		/// <summary>
		/// If true, forces the decimal separator to always appear in a formatted
		/// number, even if the fractional part of the number is zero.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isDecimalSeparatorAlwaysShown </seealso>
		private bool DecimalSeparatorAlwaysShown_Renamed = false;

		/// <summary>
		/// If true, parse returns BigDecimal wherever possible.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isParseBigDecimal
		/// @since 1.5 </seealso>
		private bool ParseBigDecimal_Renamed = false;


		/// <summary>
		/// True if this object represents a currency format.  This determines
		/// whether the monetary decimal separator is used instead of the normal one.
		/// </summary>
		[NonSerialized]
		private bool IsCurrencyFormat = false;

		/// <summary>
		/// The <code>DecimalFormatSymbols</code> object used by this format.
		/// It contains the symbols used to format numbers, e.g. the grouping separator,
		/// decimal separator, and so on.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setDecimalFormatSymbols </seealso>
		/// <seealso cref= java.text.DecimalFormatSymbols </seealso>
		private DecimalFormatSymbols Symbols = null; // LIU new DecimalFormatSymbols();

		/// <summary>
		/// True to force the use of exponential (i.e. scientific) notation when formatting
		/// numbers.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private bool UseExponentialNotation; // Newly persistent in the Java 2 platform v.1.2

		/// <summary>
		/// FieldPositions describing the positive prefix String. This is
		/// lazily created. Use <code>getPositivePrefixFieldPositions</code>
		/// when needed.
		/// </summary>
		[NonSerialized]
		private FieldPosition[] PositivePrefixFieldPositions_Renamed;

		/// <summary>
		/// FieldPositions describing the positive suffix String. This is
		/// lazily created. Use <code>getPositiveSuffixFieldPositions</code>
		/// when needed.
		/// </summary>
		[NonSerialized]
		private FieldPosition[] PositiveSuffixFieldPositions_Renamed;

		/// <summary>
		/// FieldPositions describing the negative prefix String. This is
		/// lazily created. Use <code>getNegativePrefixFieldPositions</code>
		/// when needed.
		/// </summary>
		[NonSerialized]
		private FieldPosition[] NegativePrefixFieldPositions_Renamed;

		/// <summary>
		/// FieldPositions describing the negative suffix String. This is
		/// lazily created. Use <code>getNegativeSuffixFieldPositions</code>
		/// when needed.
		/// </summary>
		[NonSerialized]
		private FieldPosition[] NegativeSuffixFieldPositions_Renamed;

		/// <summary>
		/// The minimum number of digits used to display the exponent when a number is
		/// formatted in exponential notation.  This field is ignored if
		/// <code>useExponentialNotation</code> is not true.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private sbyte MinExponentDigits; // Newly persistent in the Java 2 platform v.1.2

		/// <summary>
		/// The maximum number of digits allowed in the integer portion of a
		/// <code>BigInteger</code> or <code>BigDecimal</code> number.
		/// <code>maximumIntegerDigits</code> must be greater than or equal to
		/// <code>minimumIntegerDigits</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMaximumIntegerDigits
		/// @since 1.5 </seealso>
		private int MaximumIntegerDigits_Renamed;

		/// <summary>
		/// The minimum number of digits allowed in the integer portion of a
		/// <code>BigInteger</code> or <code>BigDecimal</code> number.
		/// <code>minimumIntegerDigits</code> must be less than or equal to
		/// <code>maximumIntegerDigits</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMinimumIntegerDigits
		/// @since 1.5 </seealso>
		private int MinimumIntegerDigits_Renamed;

		/// <summary>
		/// The maximum number of digits allowed in the fractional portion of a
		/// <code>BigInteger</code> or <code>BigDecimal</code> number.
		/// <code>maximumFractionDigits</code> must be greater than or equal to
		/// <code>minimumFractionDigits</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMaximumFractionDigits
		/// @since 1.5 </seealso>
		private int MaximumFractionDigits_Renamed;

		/// <summary>
		/// The minimum number of digits allowed in the fractional portion of a
		/// <code>BigInteger</code> or <code>BigDecimal</code> number.
		/// <code>minimumFractionDigits</code> must be less than or equal to
		/// <code>maximumFractionDigits</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMinimumFractionDigits
		/// @since 1.5 </seealso>
		private int MinimumFractionDigits_Renamed;

		/// <summary>
		/// The <seealso cref="java.math.RoundingMode"/> used in this DecimalFormat.
		/// 
		/// @serial
		/// @since 1.6
		/// </summary>
		private RoundingMode RoundingMode_Renamed = RoundingMode.HALF_EVEN;

		// ------ DecimalFormat fields for fast-path for double algorithm  ------

		/// <summary>
		/// Helper inner utility class for storing the data used in the fast-path
		/// algorithm. Almost all fields related to fast-path are encapsulated in
		/// this class.
		/// 
		/// Any {@code DecimalFormat} instance has a {@code fastPathData}
		/// reference field that is null unless both the properties of the instance
		/// are such that the instance is in the "fast-path" state, and a format call
		/// has been done at least once while in this state.
		/// 
		/// Almost all fields are related to the "fast-path" state only and don't
		/// change until one of the instance properties is changed.
		/// 
		/// {@code firstUsedIndex} and {@code lastFreeIndex} are the only
		/// two fields that are used and modified while inside a call to
		/// {@code fastDoubleFormat}.
		/// 
		/// </summary>
		private class FastPathData
		{
			// --- Temporary fields used in fast-path, shared by several methods.

			/// <summary>
			/// The first unused index at the end of the formatted result. </summary>
			internal int LastFreeIndex;

			/// <summary>
			/// The first used index at the beginning of the formatted result </summary>
			internal int FirstUsedIndex;

			// --- State fields related to fast-path status. Changes due to a
			//     property change only. Set by checkAndSetFastPathStatus() only.

			/// <summary>
			/// Difference between locale zero and default zero representation. </summary>
			internal int ZeroDelta;

			/// <summary>
			/// Locale char for grouping separator. </summary>
			internal char GroupingChar;

			/// <summary>
			///  Fixed index position of last integral digit of formatted result </summary>
			internal int IntegralLastIndex;

			/// <summary>
			///  Fixed index position of first fractional digit of formatted result </summary>
			internal int FractionalFirstIndex;

			/// <summary>
			/// Fractional constants depending on decimal|currency state </summary>
			internal double FractionalScaleFactor;
			internal int FractionalMaxIntBound;


			/// <summary>
			/// The char array buffer that will contain the formatted result </summary>
			internal char[] FastPathContainer;

			/// <summary>
			/// Suffixes recorded as char array for efficiency. </summary>
			internal char[] CharsPositivePrefix;
			internal char[] CharsNegativePrefix;
			internal char[] CharsPositiveSuffix;
			internal char[] CharsNegativeSuffix;
			internal bool PositiveAffixesRequired = true;
			internal bool NegativeAffixesRequired = true;
		}

		/// <summary>
		/// The format fast-path status of the instance. Logical state. </summary>
		[NonSerialized]
		private bool IsFastPath = false;

		/// <summary>
		/// Flag stating need of check and reinit fast-path status on next format call. </summary>
		[NonSerialized]
		private bool FastPathCheckNeeded = true;

		/// <summary>
		/// DecimalFormat reference to its FastPathData </summary>
		[NonSerialized]
		private FastPathData FastPathData;


		//----------------------------------------------------------------------

		internal new const int CurrentSerialVersion = 4;

		/// <summary>
		/// The internal serial version which says which version was written.
		/// Possible values are:
		/// <ul>
		/// <li><b>0</b> (default): versions before the Java 2 platform v1.2
		/// <li><b>1</b>: version for 1.2, which includes the two new fields
		///      <code>useExponentialNotation</code> and
		///      <code>minExponentDigits</code>.
		/// <li><b>2</b>: version for 1.3 and later, which adds four new fields:
		///      <code>posPrefixPattern</code>, <code>posSuffixPattern</code>,
		///      <code>negPrefixPattern</code>, and <code>negSuffixPattern</code>.
		/// <li><b>3</b>: version for 1.5 and later, which adds five new fields:
		///      <code>maximumIntegerDigits</code>,
		///      <code>minimumIntegerDigits</code>,
		///      <code>maximumFractionDigits</code>,
		///      <code>minimumFractionDigits</code>, and
		///      <code>parseBigDecimal</code>.
		/// <li><b>4</b>: version for 1.6 and later, which adds one new field:
		///      <code>roundingMode</code>.
		/// </ul>
		/// @since 1.2
		/// @serial
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		//----------------------------------------------------------------------
		// CONSTANTS
		//----------------------------------------------------------------------

		// ------ Fast-Path for double Constants ------

		/// <summary>
		/// Maximum valid integer value for applying fast-path algorithm </summary>
		private static readonly double MAX_INT_AS_DOUBLE = (double) Integer.MaxValue;

		/// <summary>
		/// The digit arrays used in the fast-path methods for collecting digits.
		/// Using 3 constants arrays of chars ensures a very fast collection of digits
		/// </summary>
		private class DigitArrays
		{
			internal static readonly char[] DigitOnes1000 = new char[1000];
			internal static readonly char[] DigitTens1000 = new char[1000];
			internal static readonly char[] DigitHundreds1000 = new char[1000];

			// initialize on demand holder class idiom for arrays of digits
			static DigitArrays()
			{
				int tenIndex = 0;
				int hundredIndex = 0;
				char digitOne = '0';
				char digitTen = '0';
				char digitHundred = '0';
				for (int i = 0; i < 1000; i++)
				{

					DigitOnes1000[i] = digitOne;
					if (digitOne == '9')
					{
						digitOne = '0';
					}
					else
					{
						digitOne++;
					}

					DigitTens1000[i] = digitTen;
					if (i == (tenIndex + 9))
					{
						tenIndex += 10;
						if (digitTen == '9')
						{
							digitTen = '0';
						}
						else
						{
							digitTen++;
						}
					}

					DigitHundreds1000[i] = digitHundred;
					if (i == (hundredIndex + 99))
					{
						digitHundred++;
						hundredIndex += 100;
					}
				}
			}
		}
		// ------ Fast-Path for double Constants end ------

		// Constants for characters used in programmatic (unlocalized) patterns.
		private const char PATTERN_ZERO_DIGIT = '0';
		private const char PATTERN_GROUPING_SEPARATOR = ',';
		private const char PATTERN_DECIMAL_SEPARATOR = '.';
		private const char PATTERN_PER_MILLE = '\u2030';
		private const char PATTERN_PERCENT = '%';
		private const char PATTERN_DIGIT = '#';
		private const char PATTERN_SEPARATOR = ';';
		private const String PATTERN_EXPONENT = "E";
		private const char PATTERN_MINUS = '-';

		/// <summary>
		/// The CURRENCY_SIGN is the standard Unicode symbol for currency.  It
		/// is used in patterns and substituted with either the currency symbol,
		/// or if it is doubled, with the international currency symbol.  If the
		/// CURRENCY_SIGN is seen in a pattern, then the decimal separator is
		/// replaced with the monetary decimal separator.
		/// 
		/// The CURRENCY_SIGN is not localized.
		/// </summary>
		private const char CURRENCY_SIGN = '\u00A4';

		private const char QUOTE = '\'';

		private static FieldPosition[] EmptyFieldPositionArray = new FieldPosition[0];

		// Upper limit on integer and fraction digits for a Java double
		internal const int DOUBLE_INTEGER_DIGITS = 309;
		internal const int DOUBLE_FRACTION_DIGITS = 340;

		// Upper limit on integer and fraction digits for BigDecimal and BigInteger
		internal const int MAXIMUM_INTEGER_DIGITS = Integer.MaxValue;
		internal const int MAXIMUM_FRACTION_DIGITS = Integer.MaxValue;

		// Proclaim JDK 1.1 serial compatibility.
		internal new const long SerialVersionUID = 864413376551465018L;
	}

}