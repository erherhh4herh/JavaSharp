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
	using LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool;

	/// <summary>
	/// <code>NumberFormat</code> is the abstract base class for all number
	/// formats. This class provides the interface for formatting and parsing
	/// numbers. <code>NumberFormat</code> also provides methods for determining
	/// which locales have number formats, and what their names are.
	/// 
	/// <para>
	/// <code>NumberFormat</code> helps you to format and parse numbers for any locale.
	/// Your code can be completely independent of the locale conventions for
	/// decimal points, thousands-separators, or even the particular decimal
	/// digits used, or whether the number format is even decimal.
	/// 
	/// </para>
	/// <para>
	/// To format a number for the current Locale, use one of the factory
	/// class methods:
	/// <blockquote>
	/// <pre>{@code
	/// myString = NumberFormat.getInstance().format(myNumber);
	/// }</pre>
	/// </blockquote>
	/// If you are formatting multiple numbers, it is
	/// more efficient to get the format and use it multiple times so that
	/// the system doesn't have to fetch the information about the local
	/// language and country conventions multiple times.
	/// <blockquote>
	/// <pre>{@code
	/// NumberFormat nf = NumberFormat.getInstance();
	/// for (int i = 0; i < myNumber.length; ++i) {
	///     output.println(nf.format(myNumber[i]) + "; ");
	/// }
	/// }</pre>
	/// </blockquote>
	/// To format a number for a different Locale, specify it in the
	/// call to <code>getInstance</code>.
	/// <blockquote>
	/// <pre>{@code
	/// NumberFormat nf = NumberFormat.getInstance(Locale.FRENCH);
	/// }</pre>
	/// </blockquote>
	/// You can also use a <code>NumberFormat</code> to parse numbers:
	/// <blockquote>
	/// <pre>{@code
	/// myNumber = nf.parse(myString);
	/// }</pre>
	/// </blockquote>
	/// Use <code>getInstance</code> or <code>getNumberInstance</code> to get the
	/// normal number format. Use <code>getIntegerInstance</code> to get an
	/// integer number format. Use <code>getCurrencyInstance</code> to get the
	/// currency number format. And use <code>getPercentInstance</code> to get a
	/// format for displaying percentages. With this format, a fraction like
	/// 0.53 is displayed as 53%.
	/// 
	/// </para>
	/// <para>
	/// You can also control the display of numbers with such methods as
	/// <code>setMinimumFractionDigits</code>.
	/// If you want even more control over the format or parsing,
	/// or want to give your users more control,
	/// you can try casting the <code>NumberFormat</code> you get from the factory methods
	/// to a <code>DecimalFormat</code>. This will work for the vast majority
	/// of locales; just remember to put it in a <code>try</code> block in case you
	/// encounter an unusual one.
	/// 
	/// </para>
	/// <para>
	/// NumberFormat and DecimalFormat are designed such that some controls
	/// work for formatting and others work for parsing.  The following is
	/// the detailed description for each these control methods,
	/// </para>
	/// <para>
	/// setParseIntegerOnly : only affects parsing, e.g.
	/// if true,  "3456.78" &rarr; 3456 (and leaves the parse position just after index 6)
	/// if false, "3456.78" &rarr; 3456.78 (and leaves the parse position just after index 8)
	/// This is independent of formatting.  If you want to not show a decimal point
	/// where there might be no digits after the decimal point, use
	/// setDecimalSeparatorAlwaysShown.
	/// </para>
	/// <para>
	/// setDecimalSeparatorAlwaysShown : only affects formatting, and only where
	/// there might be no digits after the decimal point, such as with a pattern
	/// like "#,##0.##", e.g.,
	/// if true,  3456.00 &rarr; "3,456."
	/// if false, 3456.00 &rarr; "3456"
	/// This is independent of parsing.  If you want parsing to stop at the decimal
	/// point, use setParseIntegerOnly.
	/// 
	/// </para>
	/// <para>
	/// You can also use forms of the <code>parse</code> and <code>format</code>
	/// methods with <code>ParsePosition</code> and <code>FieldPosition</code> to
	/// allow you to:
	/// <ul>
	/// <li> progressively parse through pieces of a string
	/// <li> align the decimal point and other areas
	/// </ul>
	/// For example, you can align numbers in two ways:
	/// <ol>
	/// <li> If you are using a monospaced font with spacing for alignment,
	///      you can pass the <code>FieldPosition</code> in your format call, with
	///      <code>field</code> = <code>INTEGER_FIELD</code>. On output,
	///      <code>getEndIndex</code> will be set to the offset between the
	///      last character of the integer and the decimal. Add
	///      (desiredSpaceCount - getEndIndex) spaces at the front of the string.
	/// 
	/// <li> If you are using proportional fonts,
	///      instead of padding with spaces, measure the width
	///      of the string in pixels from the start to <code>getEndIndex</code>.
	///      Then move the pen by
	///      (desiredPixelWidth - widthToAlignmentPoint) before drawing the text.
	///      It also works where there is no decimal, but possibly additional
	///      characters at the end, e.g., with parentheses in negative
	///      numbers: "(12)" for -12.
	/// </ol>
	/// 
	/// <h3><a name="synchronization">Synchronization</a></h3>
	/// 
	/// </para>
	/// <para>
	/// Number formats are generally not synchronized.
	/// It is recommended to create separate format instances for each thread.
	/// If multiple threads access a format concurrently, it must be synchronized
	/// externally.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          DecimalFormat </seealso>
	/// <seealso cref=          ChoiceFormat
	/// @author       Mark Davis
	/// @author       Helena Shih </seealso>
	public abstract class NumberFormat : Format
	{

		/// <summary>
		/// Field constant used to construct a FieldPosition object. Signifies that
		/// the position of the integer part of a formatted number should be returned. </summary>
		/// <seealso cref= java.text.FieldPosition </seealso>
		public const int INTEGER_FIELD = 0;

		/// <summary>
		/// Field constant used to construct a FieldPosition object. Signifies that
		/// the position of the fraction part of a formatted number should be returned. </summary>
		/// <seealso cref= java.text.FieldPosition </seealso>
		public const int FRACTION_FIELD = 1;

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal NumberFormat()
		{
		}

		/// <summary>
		/// Formats a number and appends the resulting text to the given string
		/// buffer.
		/// The number can be of any subclass of <seealso cref="java.lang.Number"/>.
		/// <para>
		/// This implementation extracts the number's value using
		/// <seealso cref="java.lang.Number#longValue()"/> for all integral type values that
		/// can be converted to <code>long</code> without loss of information,
		/// including <code>BigInteger</code> values with a
		/// <seealso cref="java.math.BigInteger#bitLength() bit length"/> of less than 64,
		/// and <seealso cref="java.lang.Number#doubleValue()"/> for all other types. It
		/// then calls
		/// <seealso cref="#format(long,java.lang.StringBuffer,java.text.FieldPosition)"/>
		/// or <seealso cref="#format(double,java.lang.StringBuffer,java.text.FieldPosition)"/>.
		/// This may result in loss of magnitude information and precision for
		/// <code>BigInteger</code> and <code>BigDecimal</code> values.
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
		public override StringBuffer Format(Object number, StringBuffer toAppendTo, FieldPosition pos)
		{
			if (number is Long || number is Integer || number is Short || number is Byte || number is AtomicInteger || number is AtomicLong || (number is System.Numerics.BigInteger && ((System.Numerics.BigInteger)number).BitLength() < 64))
			{
				return Format(((Number)number).LongValue(), toAppendTo, pos);
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
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on number parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		/// <param name="pos"> A <code>ParsePosition</code> object with index and error
		///            index information as described above. </param>
		/// <returns> A <code>Number</code> parsed from the string. In case of
		///         error, returns null. </returns>
		/// <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		public override sealed Object ParseObject(String source, ParsePosition pos)
		{
			return Parse(source, pos);
		}

	   /// <summary>
	   /// Specialization of format.
	   /// </summary>
	   /// <param name="number"> the double number to format </param>
	   /// <returns> the formatted String </returns>
	   /// <exception cref="ArithmeticException"> if rounding is needed with rounding
	   ///                   mode being set to RoundingMode.UNNECESSARY </exception>
	   /// <seealso cref= java.text.Format#format </seealso>
		public String Format(double number)
		{
			// Use fast-path for double result if that works
			String result = FastFormat(number);
			if (result != null)
			{
				return result;
			}

			return Format(number, new StringBuffer(), DontCareFieldPosition.INSTANCE).ToString();
		}

		/*
		 * fastFormat() is supposed to be implemented in concrete subclasses only.
		 * Default implem always returns null.
		 */
		internal virtual String FastFormat(double number)
		{
			return null;
		}

	   /// <summary>
	   /// Specialization of format.
	   /// </summary>
	   /// <param name="number"> the long number to format </param>
	   /// <returns> the formatted String </returns>
	   /// <exception cref="ArithmeticException"> if rounding is needed with rounding
	   ///                   mode being set to RoundingMode.UNNECESSARY </exception>
	   /// <seealso cref= java.text.Format#format </seealso>
		public String Format(long number)
		{
			return Format(number, new StringBuffer(), DontCareFieldPosition.INSTANCE).ToString();
		}

	   /// <summary>
	   /// Specialization of format.
	   /// </summary>
	   /// <param name="number">     the double number to format </param>
	   /// <param name="toAppendTo"> the StringBuffer to which the formatted text is to be
	   ///                   appended </param>
	   /// <param name="pos">        the field position </param>
	   /// <returns> the formatted StringBuffer </returns>
	   /// <exception cref="ArithmeticException"> if rounding is needed with rounding
	   ///                   mode being set to RoundingMode.UNNECESSARY </exception>
	   /// <seealso cref= java.text.Format#format </seealso>
		public abstract StringBuffer Format(double number, StringBuffer toAppendTo, FieldPosition pos);

	   /// <summary>
	   /// Specialization of format.
	   /// </summary>
	   /// <param name="number">     the long number to format </param>
	   /// <param name="toAppendTo"> the StringBuffer to which the formatted text is to be
	   ///                   appended </param>
	   /// <param name="pos">        the field position </param>
	   /// <returns> the formatted StringBuffer </returns>
	   /// <exception cref="ArithmeticException"> if rounding is needed with rounding
	   ///                   mode being set to RoundingMode.UNNECESSARY </exception>
	   /// <seealso cref= java.text.Format#format </seealso>
		public abstract StringBuffer Format(long number, StringBuffer toAppendTo, FieldPosition pos);

	   /// <summary>
	   /// Returns a Long if possible (e.g., within the range [Long.MIN_VALUE,
	   /// Long.MAX_VALUE] and with no decimals), otherwise a Double.
	   /// If IntegerOnly is set, will stop at a decimal
	   /// point (or equivalent; e.g., for rational numbers "1 2/3", will stop
	   /// after the 1).
	   /// Does not throw an exception; if no object can be parsed, index is
	   /// unchanged!
	   /// </summary>
	   /// <param name="source"> the String to parse </param>
	   /// <param name="parsePosition"> the parse position </param>
	   /// <returns> the parsed value </returns>
	   /// <seealso cref= java.text.NumberFormat#isParseIntegerOnly </seealso>
	   /// <seealso cref= java.text.Format#parseObject </seealso>
		public abstract Number Parse(String source, ParsePosition parsePosition);

		/// <summary>
		/// Parses text from the beginning of the given string to produce a number.
		/// The method may not use the entire text of the given string.
		/// <para>
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on number parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		/// <returns> A <code>Number</code> parsed from the string. </returns>
		/// <exception cref="ParseException"> if the beginning of the specified string
		///            cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Number parse(String source) throws ParseException
		public virtual Number Parse(String source)
		{
			ParsePosition parsePosition = new ParsePosition(0);
			Number result = Parse(source, parsePosition);
			if (parsePosition.Index_Renamed == 0)
			{
				throw new ParseException("Unparseable number: \"" + source + "\"", parsePosition.ErrorIndex_Renamed);
			}
			return result;
		}

		/// <summary>
		/// Returns true if this format will parse numbers as integers only.
		/// For example in the English locale, with ParseIntegerOnly true, the
		/// string "1234." would be parsed as the integer value 1234 and parsing
		/// would stop at the "." character.  Of course, the exact format accepted
		/// by the parse operation is locale dependant and determined by sub-classes
		/// of NumberFormat.
		/// </summary>
		/// <returns> {@code true} if numbers should be parsed as integers only;
		///         {@code false} otherwise </returns>
		public virtual bool ParseIntegerOnly
		{
			get
			{
				return ParseIntegerOnly_Renamed;
			}
			set
			{
				ParseIntegerOnly_Renamed = value;
			}
		}


		//============== Locale Stuff =====================

		/// <summary>
		/// Returns a general-purpose number format for the current default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// This is the same as calling
		/// <seealso cref="#getNumberInstance() getNumberInstance()"/>.
		/// </summary>
		/// <returns> the {@code NumberFormat} instance for general-purpose number
		/// formatting </returns>
		public static NumberFormat Instance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), NUMBERSTYLE);
			}
		}

		/// <summary>
		/// Returns a general-purpose number format for the specified locale.
		/// This is the same as calling
		/// <seealso cref="#getNumberInstance(java.util.Locale) getNumberInstance(inLocale)"/>.
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/// <returns> the {@code NumberFormat} instance for general-purpose number
		/// formatting </returns>
		public static NumberFormat GetInstance(Locale inLocale)
		{
			return GetInstance(inLocale, NUMBERSTYLE);
		}

		/// <summary>
		/// Returns a general-purpose number format for the current default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getNumberInstance(Locale)
		///     getNumberInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code NumberFormat} instance for general-purpose number
		/// formatting </returns>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		public static NumberFormat NumberInstance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), NUMBERSTYLE);
			}
		}

		/// <summary>
		/// Returns a general-purpose number format for the specified locale.
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/// <returns> the {@code NumberFormat} instance for general-purpose number
		/// formatting </returns>
		public static NumberFormat GetNumberInstance(Locale inLocale)
		{
			return GetInstance(inLocale, NUMBERSTYLE);
		}

		/// <summary>
		/// Returns an integer number format for the current default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale. The
		/// returned number format is configured to round floating point numbers
		/// to the nearest integer using half-even rounding (see {@link
		/// java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN}) for formatting,
		/// and to parse only the integer part of an input string (see {@link
		/// #isParseIntegerOnly isParseIntegerOnly}).
		/// <para>This is equivalent to calling
		/// {@link #getIntegerInstance(Locale)
		///     getIntegerInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getRoundingMode() </seealso>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a number format for integer values
		/// @since 1.4 </returns>
		public static NumberFormat IntegerInstance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), INTEGERSTYLE);
			}
		}

		/// <summary>
		/// Returns an integer number format for the specified locale. The
		/// returned number format is configured to round floating point numbers
		/// to the nearest integer using half-even rounding (see {@link
		/// java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN}) for formatting,
		/// and to parse only the integer part of an input string (see {@link
		/// #isParseIntegerOnly isParseIntegerOnly}).
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/// <seealso cref= #getRoundingMode() </seealso>
		/// <returns> a number format for integer values
		/// @since 1.4 </returns>
		public static NumberFormat GetIntegerInstance(Locale inLocale)
		{
			return GetInstance(inLocale, INTEGERSTYLE);
		}

		/// <summary>
		/// Returns a currency format for the current default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getCurrencyInstance(Locale)
		///     getCurrencyInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code NumberFormat} instance for currency formatting </returns>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		public static NumberFormat CurrencyInstance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), CURRENCYSTYLE);
			}
		}

		/// <summary>
		/// Returns a currency format for the specified locale.
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/// <returns> the {@code NumberFormat} instance for currency formatting </returns>
		public static NumberFormat GetCurrencyInstance(Locale inLocale)
		{
			return GetInstance(inLocale, CURRENCYSTYLE);
		}

		/// <summary>
		/// Returns a percentage format for the current default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>This is equivalent to calling
		/// {@link #getPercentInstance(Locale)
		///     getPercentInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code NumberFormat} instance for percentage formatting </returns>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		public static NumberFormat PercentInstance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), PERCENTSTYLE);
			}
		}

		/// <summary>
		/// Returns a percentage format for the specified locale.
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/// <returns> the {@code NumberFormat} instance for percentage formatting </returns>
		public static NumberFormat GetPercentInstance(Locale inLocale)
		{
			return GetInstance(inLocale, PERCENTSTYLE);
		}

		/// <summary>
		/// Returns a scientific format for the current default locale.
		/// </summary>
		/*public*/
	 internal static NumberFormat ScientificInstance
	 {
		 get
		 {
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT), SCIENTIFICSTYLE);
		 }
	 }

		/// <summary>
		/// Returns a scientific format for the specified locale.
		/// </summary>
		/// <param name="inLocale"> the desired locale </param>
		/*public*/	 internal static NumberFormat GetScientificInstance(Locale inLocale)
	 {
			return GetInstance(inLocale, SCIENTIFICSTYLE);
	 }

		/// <summary>
		/// Returns an array of all locales for which the
		/// <code>get*Instance</code> methods of this class can return
		/// localized instances.
		/// The returned array represents the union of locales supported by the Java
		/// runtime and by installed
		/// <seealso cref="java.text.spi.NumberFormatProvider NumberFormatProvider"/> implementations.
		/// It must contain at least a <code>Locale</code> instance equal to
		/// <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of locales for which localized
		///         <code>NumberFormat</code> instances are available. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(NumberFormatProvider));
				return pool.AvailableLocales;
			}
		}

		/// <summary>
		/// Overrides hashCode.
		/// </summary>
		public override int HashCode()
		{
			return MaximumIntegerDigits_Renamed * 37 + MaxFractionDigits;
			// just enough fields for a reasonable distribution
		}

		/// <summary>
		/// Overrides equals.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			NumberFormat other = (NumberFormat) obj;
			return (MaximumIntegerDigits_Renamed == other.MaximumIntegerDigits_Renamed && MinimumIntegerDigits_Renamed == other.MinimumIntegerDigits_Renamed && MaximumFractionDigits_Renamed == other.MaximumFractionDigits_Renamed && MinimumFractionDigits_Renamed == other.MinimumFractionDigits_Renamed && GroupingUsed_Renamed == other.GroupingUsed_Renamed && ParseIntegerOnly_Renamed == other.ParseIntegerOnly_Renamed);
		}

		/// <summary>
		/// Overrides Cloneable.
		/// </summary>
		public override Object Clone()
		{
			NumberFormat other = (NumberFormat) base.Clone();
			return other;
		}

		/// <summary>
		/// Returns true if grouping is used in this format. For example, in the
		/// English locale, with grouping on, the number 1234567 might be formatted
		/// as "1,234,567". The grouping separator as well as the size of each group
		/// is locale dependant and is determined by sub-classes of NumberFormat.
		/// </summary>
		/// <returns> {@code true} if grouping is used;
		///         {@code false} otherwise </returns>
		/// <seealso cref= #setGroupingUsed </seealso>
		public virtual bool GroupingUsed
		{
			get
			{
				return GroupingUsed_Renamed;
			}
			set
			{
				GroupingUsed_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the maximum number of digits allowed in the integer portion of a
		/// number.
		/// </summary>
		/// <returns> the maximum number of digits </returns>
		/// <seealso cref= #setMaximumIntegerDigits </seealso>
		public virtual int MaximumIntegerDigits
		{
			get
			{
				return MaximumIntegerDigits_Renamed;
			}
			set
			{
				MaximumIntegerDigits_Renamed = System.Math.Max(0,value);
				if (MinimumIntegerDigits_Renamed > MaximumIntegerDigits_Renamed)
				{
					MinimumIntegerDigits_Renamed = MaximumIntegerDigits_Renamed;
				}
			}
		}


		/// <summary>
		/// Returns the minimum number of digits allowed in the integer portion of a
		/// number.
		/// </summary>
		/// <returns> the minimum number of digits </returns>
		/// <seealso cref= #setMinimumIntegerDigits </seealso>
		public virtual int MinimumIntegerDigits
		{
			get
			{
				return MinimumIntegerDigits_Renamed;
			}
			set
			{
				MinimumIntegerDigits_Renamed = System.Math.Max(0,value);
				if (MinimumIntegerDigits_Renamed > MaximumIntegerDigits_Renamed)
				{
					MaximumIntegerDigits_Renamed = MinimumIntegerDigits_Renamed;
				}
			}
		}


		/// <summary>
		/// Returns the maximum number of digits allowed in the fraction portion of a
		/// number.
		/// </summary>
		/// <returns> the maximum number of digits. </returns>
		/// <seealso cref= #setMaximumFractionDigits </seealso>
		public virtual int MaximumFractionDigits
		{
			get
			{
				return MaximumFractionDigits_Renamed;
			}
			set
			{
				MaximumFractionDigits_Renamed = System.Math.Max(0,value);
				if (MaximumFractionDigits_Renamed < MinimumFractionDigits_Renamed)
				{
					MinimumFractionDigits_Renamed = MaximumFractionDigits_Renamed;
				}
			}
		}


		/// <summary>
		/// Returns the minimum number of digits allowed in the fraction portion of a
		/// number.
		/// </summary>
		/// <returns> the minimum number of digits </returns>
		/// <seealso cref= #setMinimumFractionDigits </seealso>
		public virtual int MinimumFractionDigits
		{
			get
			{
				return MinimumFractionDigits_Renamed;
			}
			set
			{
				MinimumFractionDigits_Renamed = System.Math.Max(0,value);
				if (MaximumFractionDigits_Renamed < MinimumFractionDigits_Renamed)
				{
					MaximumFractionDigits_Renamed = MinimumFractionDigits_Renamed;
				}
			}
		}


		/// <summary>
		/// Gets the currency used by this number format when formatting
		/// currency values. The initial value is derived in a locale dependent
		/// way. The returned value may be null if no valid
		/// currency could be determined and no currency has been set using
		/// <seealso cref="#setCurrency(java.util.Currency) setCurrency"/>.
		/// <para>
		/// The default implementation throws
		/// <code>UnsupportedOperationException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the currency used by this number format, or <code>null</code> </returns>
		/// <exception cref="UnsupportedOperationException"> if the number format class
		/// doesn't implement currency formatting
		/// @since 1.4 </exception>
		public virtual Currency Currency
		{
			get
			{
				throw new UnsupportedOperationException();
			}
			set
			{
				throw new UnsupportedOperationException();
			}
		}


		/// <summary>
		/// Gets the <seealso cref="java.math.RoundingMode"/> used in this NumberFormat.
		/// The default implementation of this method in NumberFormat
		/// always throws <seealso cref="java.lang.UnsupportedOperationException"/>.
		/// Subclasses which handle different rounding modes should override
		/// this method.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> The default implementation
		///     always throws this exception </exception>
		/// <returns> The <code>RoundingMode</code> used for this NumberFormat. </returns>
		/// <seealso cref= #setRoundingMode(RoundingMode)
		/// @since 1.6 </seealso>
		public virtual RoundingMode RoundingMode
		{
			get
			{
				throw new UnsupportedOperationException();
			}
			set
			{
				throw new UnsupportedOperationException();
			}
		}


		// =======================privates===============================

		private static NumberFormat GetInstance(Locale desiredLocale, int choice)
		{
			LocaleProviderAdapter adapter;
			adapter = LocaleProviderAdapter.getAdapter(typeof(NumberFormatProvider), desiredLocale);
			NumberFormat numberFormat = GetInstance(adapter, desiredLocale, choice);
			if (numberFormat == null)
			{
				numberFormat = GetInstance(LocaleProviderAdapter.forJRE(), desiredLocale, choice);
			}
			return numberFormat;
		}

		private static NumberFormat GetInstance(LocaleProviderAdapter adapter, Locale locale, int choice)
		{
			NumberFormatProvider provider = adapter.NumberFormatProvider;
			NumberFormat numberFormat = null;
			switch (choice)
			{
			case NUMBERSTYLE:
				numberFormat = provider.GetNumberInstance(locale);
				break;
			case PERCENTSTYLE:
				numberFormat = provider.GetPercentInstance(locale);
				break;
			case CURRENCYSTYLE:
				numberFormat = provider.GetCurrencyInstance(locale);
				break;
			case INTEGERSTYLE:
				numberFormat = provider.GetIntegerInstance(locale);
				break;
			}
			return numberFormat;
		}

		/// <summary>
		/// First, read in the default serializable data.
		/// 
		/// Then, if <code>serialVersionOnStream</code> is less than 1, indicating that
		/// the stream was written by JDK 1.1,
		/// set the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		/// to be equal to the <code>byte</code> fields such as <code>maxIntegerDigits</code>,
		/// since the <code>int</code> fields were not present in JDK 1.1.
		/// Finally, set serialVersionOnStream back to the maximum allowed value so that
		/// default serialization will work properly if this object is streamed out again.
		/// 
		/// <para>If <code>minimumIntegerDigits</code> is greater than
		/// <code>maximumIntegerDigits</code> or <code>minimumFractionDigits</code>
		/// is greater than <code>maximumFractionDigits</code>, then the stream data
		/// is invalid and this method throws an <code>InvalidObjectException</code>.
		/// In addition, if any of these values is negative, then this method throws
		/// an <code>InvalidObjectException</code>.
		/// 
		/// @since 1.2
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			if (SerialVersionOnStream < 1)
			{
				// Didn't have additional int fields, reassign to use them.
				MaximumIntegerDigits_Renamed = MaxIntegerDigits;
				MinimumIntegerDigits_Renamed = MinIntegerDigits;
				MaximumFractionDigits_Renamed = MaxFractionDigits;
				MinimumFractionDigits_Renamed = MinFractionDigits;
			}
			if (MinimumIntegerDigits_Renamed > MaximumIntegerDigits_Renamed || MinimumFractionDigits_Renamed > MaximumFractionDigits_Renamed || MinimumIntegerDigits_Renamed < 0 || MinimumFractionDigits_Renamed < 0)
			{
				throw new InvalidObjectException("Digit count range invalid");
			}
			SerialVersionOnStream = CurrentSerialVersion;
		}

		/// <summary>
		/// Write out the default serializable data, after first setting
		/// the <code>byte</code> fields such as <code>maxIntegerDigits</code> to be
		/// equal to the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		/// (or to <code>Byte.MAX_VALUE</code>, whichever is smaller), for compatibility
		/// with the JDK 1.1 version of the stream format.
		/// 
		/// @since 1.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		private void WriteObject(ObjectOutputStream stream)
		{
			MaxIntegerDigits = (MaximumIntegerDigits_Renamed > Byte.MaxValue) ? Byte.MaxValue : (sbyte)MaximumIntegerDigits_Renamed;
			MinIntegerDigits = (MinimumIntegerDigits_Renamed > Byte.MaxValue) ? Byte.MaxValue : (sbyte)MinimumIntegerDigits_Renamed;
			MaxFractionDigits = (MaximumFractionDigits_Renamed > Byte.MaxValue) ? Byte.MaxValue : (sbyte)MaximumFractionDigits_Renamed;
			MinFractionDigits = (MinimumFractionDigits_Renamed > Byte.MaxValue) ? Byte.MaxValue : (sbyte)MinimumFractionDigits_Renamed;
			stream.DefaultWriteObject();
		}

		// Constants used by factory methods to specify a style of format.
		private const int NUMBERSTYLE = 0;
		private const int CURRENCYSTYLE = 1;
		private const int PERCENTSTYLE = 2;
		private const int SCIENTIFICSTYLE = 3;
		private const int INTEGERSTYLE = 4;

		/// <summary>
		/// True if the grouping (i.e. thousands) separator is used when
		/// formatting and parsing numbers.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isGroupingUsed </seealso>
		private bool GroupingUsed_Renamed = true;

		/// <summary>
		/// The maximum number of digits allowed in the integer portion of a
		/// number.  <code>maxIntegerDigits</code> must be greater than or equal to
		/// <code>minIntegerDigits</code>.
		/// <para>
		/// <strong>Note:</strong> This field exists only for serialization
		/// compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		/// <code>int</code> field <code>maximumIntegerDigits</code> is used instead.
		/// When writing to a stream, <code>maxIntegerDigits</code> is set to
		/// <code>maximumIntegerDigits</code> or <code>Byte.MAX_VALUE</code>,
		/// whichever is smaller.  When reading from a stream, this field is used
		/// only if <code>serialVersionOnStream</code> is less than 1.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #getMaximumIntegerDigits </seealso>
		private sbyte MaxIntegerDigits = 40;

		/// <summary>
		/// The minimum number of digits allowed in the integer portion of a
		/// number.  <code>minimumIntegerDigits</code> must be less than or equal to
		/// <code>maximumIntegerDigits</code>.
		/// <para>
		/// <strong>Note:</strong> This field exists only for serialization
		/// compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		/// <code>int</code> field <code>minimumIntegerDigits</code> is used instead.
		/// When writing to a stream, <code>minIntegerDigits</code> is set to
		/// <code>minimumIntegerDigits</code> or <code>Byte.MAX_VALUE</code>,
		/// whichever is smaller.  When reading from a stream, this field is used
		/// only if <code>serialVersionOnStream</code> is less than 1.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #getMinimumIntegerDigits </seealso>
		private sbyte MinIntegerDigits = 1;

		/// <summary>
		/// The maximum number of digits allowed in the fractional portion of a
		/// number.  <code>maximumFractionDigits</code> must be greater than or equal to
		/// <code>minimumFractionDigits</code>.
		/// <para>
		/// <strong>Note:</strong> This field exists only for serialization
		/// compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		/// <code>int</code> field <code>maximumFractionDigits</code> is used instead.
		/// When writing to a stream, <code>maxFractionDigits</code> is set to
		/// <code>maximumFractionDigits</code> or <code>Byte.MAX_VALUE</code>,
		/// whichever is smaller.  When reading from a stream, this field is used
		/// only if <code>serialVersionOnStream</code> is less than 1.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #getMaximumFractionDigits </seealso>
		private sbyte MaxFractionDigits = 3; // invariant, >= minFractionDigits

		/// <summary>
		/// The minimum number of digits allowed in the fractional portion of a
		/// number.  <code>minimumFractionDigits</code> must be less than or equal to
		/// <code>maximumFractionDigits</code>.
		/// <para>
		/// <strong>Note:</strong> This field exists only for serialization
		/// compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		/// <code>int</code> field <code>minimumFractionDigits</code> is used instead.
		/// When writing to a stream, <code>minFractionDigits</code> is set to
		/// <code>minimumFractionDigits</code> or <code>Byte.MAX_VALUE</code>,
		/// whichever is smaller.  When reading from a stream, this field is used
		/// only if <code>serialVersionOnStream</code> is less than 1.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #getMinimumFractionDigits </seealso>
		private sbyte MinFractionDigits = 0;

		/// <summary>
		/// True if this format will parse numbers as integers only.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isParseIntegerOnly </seealso>
		private bool ParseIntegerOnly_Renamed = false;

		// new fields for 1.2.  byte is too small for integer digits.

		/// <summary>
		/// The maximum number of digits allowed in the integer portion of a
		/// number.  <code>maximumIntegerDigits</code> must be greater than or equal to
		/// <code>minimumIntegerDigits</code>.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #getMaximumIntegerDigits </seealso>
		private int MaximumIntegerDigits_Renamed = 40;

		/// <summary>
		/// The minimum number of digits allowed in the integer portion of a
		/// number.  <code>minimumIntegerDigits</code> must be less than or equal to
		/// <code>maximumIntegerDigits</code>.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #getMinimumIntegerDigits </seealso>
		private int MinimumIntegerDigits_Renamed = 1;

		/// <summary>
		/// The maximum number of digits allowed in the fractional portion of a
		/// number.  <code>maximumFractionDigits</code> must be greater than or equal to
		/// <code>minimumFractionDigits</code>.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #getMaximumFractionDigits </seealso>
		private int MaximumFractionDigits_Renamed = 3; // invariant, >= minFractionDigits

		/// <summary>
		/// The minimum number of digits allowed in the fractional portion of a
		/// number.  <code>minimumFractionDigits</code> must be less than or equal to
		/// <code>maximumFractionDigits</code>.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #getMinimumFractionDigits </seealso>
		private int MinimumFractionDigits_Renamed = 0;

		internal const int CurrentSerialVersion = 1;

		/// <summary>
		/// Describes the version of <code>NumberFormat</code> present on the stream.
		/// Possible values are:
		/// <ul>
		/// <li><b>0</b> (or uninitialized): the JDK 1.1 version of the stream format.
		///     In this version, the <code>int</code> fields such as
		///     <code>maximumIntegerDigits</code> were not present, and the <code>byte</code>
		///     fields such as <code>maxIntegerDigits</code> are used instead.
		/// 
		/// <li><b>1</b>: the 1.2 version of the stream format.  The values of the
		///     <code>byte</code> fields such as <code>maxIntegerDigits</code> are ignored,
		///     and the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		///     are used instead.
		/// </ul>
		/// When streaming out a <code>NumberFormat</code>, the most recent format
		/// (corresponding to the highest allowable <code>serialVersionOnStream</code>)
		/// is always written.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		// Removed "implements Cloneable" clause.  Needs to update serialization
		// ID for backward compatibility.
		internal const long SerialVersionUID = -2308460125733713944L;


		//
		// class for AttributedCharacterIterator attributes
		//
		/// <summary>
		/// Defines constants that are used as attribute keys in the
		/// <code>AttributedCharacterIterator</code> returned
		/// from <code>NumberFormat.formatToCharacterIterator</code> and as
		/// field identifiers in <code>FieldPosition</code>.
		/// 
		/// @since 1.4
		/// </summary>
		public class Field : Format.Field
		{

			// Proclaim serial compatibility with 1.4 FCS
			internal new const long SerialVersionUID = 7494728892700160890L;

			// table of all instances in this class, used by readResolve
			internal new static readonly IDictionary<String, Field> InstanceMap = new Dictionary<String, Field>(11);

			/// <summary>
			/// Creates a Field instance with the specified
			/// name.
			/// </summary>
			/// <param name="name"> Name of the attribute </param>
			protected internal Field(String name) : base(name)
			{
				if (this.GetType() == typeof(NumberFormat.Field))
				{
					InstanceMap[name] = this;
				}
			}

			/// <summary>
			/// Resolves instances being deserialized to the predefined constants.
			/// </summary>
			/// <exception cref="InvalidObjectException"> if the constant could not be resolved. </exception>
			/// <returns> resolved NumberFormat.Field constant </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected Object readResolve() throws java.io.InvalidObjectException
			protected internal override Object ReadResolve()
			{
				if (this.GetType() != typeof(NumberFormat.Field))
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

			/// <summary>
			/// Constant identifying the integer field.
			/// </summary>
			public static readonly Field INTEGER = new Field("integer");

			/// <summary>
			/// Constant identifying the fraction field.
			/// </summary>
			public static readonly Field FRACTION = new Field("fraction");

			/// <summary>
			/// Constant identifying the exponent field.
			/// </summary>
			public static readonly Field EXPONENT = new Field("exponent");

			/// <summary>
			/// Constant identifying the decimal separator field.
			/// </summary>
			public static readonly Field DECIMAL_SEPARATOR = new Field("decimal separator");

			/// <summary>
			/// Constant identifying the sign field.
			/// </summary>
			public static readonly Field SIGN = new Field("sign");

			/// <summary>
			/// Constant identifying the grouping separator field.
			/// </summary>
			public static readonly Field GROUPING_SEPARATOR = new Field("grouping separator");

			/// <summary>
			/// Constant identifying the exponent symbol field.
			/// </summary>
			public static readonly Field EXPONENT_SYMBOL = new Field("exponent symbol");

			/// <summary>
			/// Constant identifying the percent field.
			/// </summary>
			public static readonly Field PERCENT = new Field("percent");

			/// <summary>
			/// Constant identifying the permille field.
			/// </summary>
			public static readonly Field PERMILLE = new Field("per mille");

			/// <summary>
			/// Constant identifying the currency field.
			/// </summary>
			public static readonly Field CURRENCY = new Field("currency");

			/// <summary>
			/// Constant identifying the exponent sign field.
			/// </summary>
			public static readonly Field EXPONENT_SIGN = new Field("exponent sign");
		}
	}

}