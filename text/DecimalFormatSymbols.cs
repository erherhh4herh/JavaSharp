using System;

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
	using ResourceBundleBasedAdapter = sun.util.locale.provider.ResourceBundleBasedAdapter;

	/// <summary>
	/// This class represents the set of symbols (such as the decimal separator,
	/// the grouping separator, and so on) needed by <code>DecimalFormat</code>
	/// to format numbers. <code>DecimalFormat</code> creates for itself an instance of
	/// <code>DecimalFormatSymbols</code> from its locale data.  If you need to change any
	/// of these symbols, you can get the <code>DecimalFormatSymbols</code> object from
	/// your <code>DecimalFormat</code> and modify it.
	/// </summary>
	/// <seealso cref=          java.util.Locale </seealso>
	/// <seealso cref=          DecimalFormat
	/// @author       Mark Davis
	/// @author       Alan Liu </seealso>

	[Serializable]
	public class DecimalFormatSymbols : Cloneable
	{

		/// <summary>
		/// Create a DecimalFormatSymbols object for the default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// This constructor can only construct instances for the locales
		/// supported by the Java runtime environment, not for those
		/// supported by installed
		/// <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		/// implementations. For full locale coverage, use the
		/// <seealso cref="#getInstance(Locale) getInstance"/> method.
		/// <para>This is equivalent to calling
		/// {@link #DecimalFormatSymbols(Locale)
		///     DecimalFormatSymbols(Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		public DecimalFormatSymbols()
		{
			Initialize(Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Create a DecimalFormatSymbols object for the given locale.
		/// This constructor can only construct instances for the locales
		/// supported by the Java runtime environment, not for those
		/// supported by installed
		/// <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		/// implementations. For full locale coverage, use the
		/// <seealso cref="#getInstance(Locale) getInstance"/> method.
		/// If the specified locale contains the <seealso cref="java.util.Locale#UNICODE_LOCALE_EXTENSION"/>
		/// for the numbering system, the instance is initialized with the specified numbering
		/// system if the JRE implementation supports it. For example,
		/// <pre>
		/// NumberFormat.getNumberInstance(Locale.forLanguageTag("th-TH-u-nu-thai"))
		/// </pre>
		/// This may return a {@code NumberFormat} instance with the Thai numbering system,
		/// instead of the Latin numbering system.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		public DecimalFormatSymbols(Locale locale)
		{
			Initialize(locale);
		}

		/// <summary>
		/// Returns an array of all locales for which the
		/// <code>getInstance</code> methods of this class can return
		/// localized instances.
		/// The returned array represents the union of locales supported by the Java
		/// runtime and by installed
		/// <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		/// implementations.  It must contain at least a <code>Locale</code>
		/// instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> an array of locales for which localized
		///         <code>DecimalFormatSymbols</code> instances are available.
		/// @since 1.6 </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(DecimalFormatSymbolsProvider));
				return pool.AvailableLocales;
			}
		}

		/// <summary>
		/// Gets the <code>DecimalFormatSymbols</code> instance for the default
		/// locale.  This method provides access to <code>DecimalFormatSymbols</code>
		/// instances for locales supported by the Java runtime itself as well
		/// as for those supported by installed
		/// {@link java.text.spi.DecimalFormatSymbolsProvider
		/// DecimalFormatSymbolsProvider} implementations.
		/// <para>This is equivalent to calling
		/// {@link #getInstance(Locale)
		///     getInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a <code>DecimalFormatSymbols</code> instance.
		/// @since 1.6 </returns>
		public static DecimalFormatSymbols Instance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets the <code>DecimalFormatSymbols</code> instance for the specified
		/// locale.  This method provides access to <code>DecimalFormatSymbols</code>
		/// instances for locales supported by the Java runtime itself as well
		/// as for those supported by installed
		/// {@link java.text.spi.DecimalFormatSymbolsProvider
		/// DecimalFormatSymbolsProvider} implementations.
		/// If the specified locale contains the <seealso cref="java.util.Locale#UNICODE_LOCALE_EXTENSION"/>
		/// for the numbering system, the instance is initialized with the specified numbering
		/// system if the JRE implementation supports it. For example,
		/// <pre>
		/// NumberFormat.getNumberInstance(Locale.forLanguageTag("th-TH-u-nu-thai"))
		/// </pre>
		/// This may return a {@code NumberFormat} instance with the Thai numbering system,
		/// instead of the Latin numbering system.
		/// </summary>
		/// <param name="locale"> the desired locale. </param>
		/// <returns> a <code>DecimalFormatSymbols</code> instance. </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null
		/// @since 1.6 </exception>
		public static DecimalFormatSymbols GetInstance(Locale locale)
		{
			LocaleProviderAdapter adapter;
			adapter = LocaleProviderAdapter.getAdapter(typeof(DecimalFormatSymbolsProvider), locale);
			DecimalFormatSymbolsProvider provider = adapter.DecimalFormatSymbolsProvider;
			DecimalFormatSymbols dfsyms = provider.GetInstance(locale);
			if (dfsyms == null)
			{
				provider = LocaleProviderAdapter.forJRE().DecimalFormatSymbolsProvider;
				dfsyms = provider.GetInstance(locale);
			}
			return dfsyms;
		}

		/// <summary>
		/// Gets the character used for zero. Different for Arabic, etc.
		/// </summary>
		/// <returns> the character used for zero </returns>
		public virtual char ZeroDigit
		{
			get
			{
				return ZeroDigit_Renamed;
			}
			set
			{
				this.ZeroDigit_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used for thousands separator. Different for French, etc.
		/// </summary>
		/// <returns> the grouping separator </returns>
		public virtual char GroupingSeparator
		{
			get
			{
				return GroupingSeparator_Renamed;
			}
			set
			{
				this.GroupingSeparator_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used for decimal sign. Different for French, etc.
		/// </summary>
		/// <returns> the character used for decimal sign </returns>
		public virtual char DecimalSeparator
		{
			get
			{
				return DecimalSeparator_Renamed;
			}
			set
			{
				this.DecimalSeparator_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used for per mille sign. Different for Arabic, etc.
		/// </summary>
		/// <returns> the character used for per mille sign </returns>
		public virtual char PerMill
		{
			get
			{
				return PerMill_Renamed;
			}
			set
			{
				this.PerMill_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used for percent sign. Different for Arabic, etc.
		/// </summary>
		/// <returns> the character used for percent sign </returns>
		public virtual char Percent
		{
			get
			{
				return Percent_Renamed;
			}
			set
			{
				this.Percent_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used for a digit in a pattern.
		/// </summary>
		/// <returns> the character used for a digit in a pattern </returns>
		public virtual char Digit
		{
			get
			{
				return Digit_Renamed;
			}
			set
			{
				this.Digit_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used to separate positive and negative subpatterns
		/// in a pattern.
		/// </summary>
		/// <returns> the pattern separator </returns>
		public virtual char PatternSeparator
		{
			get
			{
				return PatternSeparator_Renamed;
			}
			set
			{
				this.PatternSeparator_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the string used to represent infinity. Almost always left
		/// unchanged.
		/// </summary>
		/// <returns> the string representing infinity </returns>
		public virtual String Infinity
		{
			get
			{
				return Infinity_Renamed;
			}
			set
			{
				this.Infinity_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the string used to represent "not a number". Almost always left
		/// unchanged.
		/// </summary>
		/// <returns> the string representing "not a number" </returns>
		public virtual String NaN
		{
			get
			{
				return NaN_Renamed;
			}
			set
			{
				this.NaN_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the character used to represent minus sign. If no explicit
		/// negative format is specified, one is formed by prefixing
		/// minusSign to the positive format.
		/// </summary>
		/// <returns> the character representing minus sign </returns>
		public virtual char MinusSign
		{
			get
			{
				return MinusSign_Renamed;
			}
			set
			{
				this.MinusSign_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the currency symbol for the currency of these
		/// DecimalFormatSymbols in their locale.
		/// </summary>
		/// <returns> the currency symbol
		/// @since 1.2 </returns>
		public virtual String CurrencySymbol
		{
			get
			{
				return CurrencySymbol_Renamed;
			}
			set
			{
				CurrencySymbol_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the ISO 4217 currency code of the currency of these
		/// DecimalFormatSymbols.
		/// </summary>
		/// <returns> the currency code
		/// @since 1.2 </returns>
		public virtual String InternationalCurrencySymbol
		{
			get
			{
				return IntlCurrencySymbol;
			}
			set
			{
				IntlCurrencySymbol = value;
				Currency_Renamed = null;
				if (value != null)
				{
					try
					{
						Currency_Renamed = Currency.GetInstance(value);
						CurrencySymbol_Renamed = Currency_Renamed.Symbol;
					}
					catch (IllegalArgumentException)
					{
					}
				}
			}
		}


		/// <summary>
		/// Gets the currency of these DecimalFormatSymbols. May be null if the
		/// currency symbol attribute was previously set to a value that's not
		/// a valid ISO 4217 currency code.
		/// </summary>
		/// <returns> the currency used, or null
		/// @since 1.4 </returns>
		public virtual Currency Currency
		{
			get
			{
				return Currency_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
				this.Currency_Renamed = value;
				IntlCurrencySymbol = value.CurrencyCode;
				CurrencySymbol_Renamed = value.GetSymbol(Locale);
			}
		}



		/// <summary>
		/// Returns the monetary decimal separator.
		/// </summary>
		/// <returns> the monetary decimal separator
		/// @since 1.2 </returns>
		public virtual char MonetaryDecimalSeparator
		{
			get
			{
				return MonetarySeparator;
			}
			set
			{
				MonetarySeparator = value;
			}
		}


		//------------------------------------------------------------
		// BEGIN   Package Private methods ... to be made public later
		//------------------------------------------------------------

		/// <summary>
		/// Returns the character used to separate the mantissa from the exponent.
		/// </summary>
		internal virtual char ExponentialSymbol
		{
			get
			{
				return Exponential;
			}
			set
			{
				Exponential = value;
			}
		}
	  /// <summary>
	  /// Returns the string used to separate the mantissa from the exponent.
	  /// Examples: "x10^" for 1.23x10^4, "E" for 1.23E4.
	  /// </summary>
	  /// <returns> the exponent separator string </returns>
	  /// <seealso cref= #setExponentSeparator(java.lang.String)
	  /// @since 1.6 </seealso>
		public virtual String ExponentSeparator
		{
			get
			{
				return ExponentialSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
				ExponentialSeparator = value;
			}
		}




		//------------------------------------------------------------
		// END     Package Private methods ... to be made public later
		//------------------------------------------------------------

		/// <summary>
		/// Standard override.
		/// </summary>
		public override Object Clone()
		{
			try
			{
				return (DecimalFormatSymbols)base.Clone();
				// other fields are bit-copied
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Override equals.
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
			DecimalFormatSymbols other = (DecimalFormatSymbols) obj;
			return (ZeroDigit_Renamed == other.ZeroDigit_Renamed && GroupingSeparator_Renamed == other.GroupingSeparator_Renamed && DecimalSeparator_Renamed == other.DecimalSeparator_Renamed && Percent_Renamed == other.Percent_Renamed && PerMill_Renamed == other.PerMill_Renamed && Digit_Renamed == other.Digit_Renamed && MinusSign_Renamed == other.MinusSign_Renamed && PatternSeparator_Renamed == other.PatternSeparator_Renamed && Infinity_Renamed.Equals(other.Infinity_Renamed) && NaN_Renamed.Equals(other.NaN_Renamed) && CurrencySymbol_Renamed.Equals(other.CurrencySymbol_Renamed) && IntlCurrencySymbol.Equals(other.IntlCurrencySymbol) && Currency_Renamed == other.Currency_Renamed && MonetarySeparator == other.MonetarySeparator && ExponentialSeparator.Equals(other.ExponentialSeparator) && Locale.Equals(other.Locale));
		}

		/// <summary>
		/// Override hashCode.
		/// </summary>
		public override int HashCode()
		{
				int result = ZeroDigit_Renamed;
				result = result * 37 + GroupingSeparator_Renamed;
				result = result * 37 + DecimalSeparator_Renamed;
				return result;
		}

		/// <summary>
		/// Initializes the symbols from the FormatData resource bundle.
		/// </summary>
		private void Initialize(Locale locale)
		{
			this.Locale = locale;

			// get resource bundle data
			LocaleProviderAdapter adapter = LocaleProviderAdapter.getAdapter(typeof(DecimalFormatSymbolsProvider), locale);
			// Avoid potential recursions
			if (!(adapter is ResourceBundleBasedAdapter))
			{
				adapter = LocaleProviderAdapter.ResourceBundleBased;
			}
			Object[] data = adapter.getLocaleResources(locale).DecimalFormatSymbolsData;
			String[] numberElements = (String[]) data[0];

			DecimalSeparator_Renamed = numberElements[0].CharAt(0);
			GroupingSeparator_Renamed = numberElements[1].CharAt(0);
			PatternSeparator_Renamed = numberElements[2].CharAt(0);
			Percent_Renamed = numberElements[3].CharAt(0);
			ZeroDigit_Renamed = numberElements[4].CharAt(0); //different for Arabic,etc.
			Digit_Renamed = numberElements[5].CharAt(0);
			MinusSign_Renamed = numberElements[6].CharAt(0);
			Exponential = numberElements[7].CharAt(0);
			ExponentialSeparator = numberElements[7]; //string representation new since 1.6
			PerMill_Renamed = numberElements[8].CharAt(0);
			Infinity_Renamed = numberElements[9];
			NaN_Renamed = numberElements[10];

			// Try to obtain the currency used in the locale's country.
			// Check for empty country string separately because it's a valid
			// country ID for Locale (and used for the C locale), but not a valid
			// ISO 3166 country code, and exceptions are expensive.
			if (locale.Country.Length() > 0)
			{
				try
				{
					Currency_Renamed = Currency.GetInstance(locale);
				}
				catch (IllegalArgumentException)
				{
					// use default values below for compatibility
				}
			}
			if (Currency_Renamed != null)
			{
				IntlCurrencySymbol = Currency_Renamed.CurrencyCode;
				if (data[1] != null && data[1] == IntlCurrencySymbol)
				{
					CurrencySymbol_Renamed = (String) data[2];
				}
				else
				{
					CurrencySymbol_Renamed = Currency_Renamed.GetSymbol(locale);
					data[1] = IntlCurrencySymbol;
					data[2] = CurrencySymbol_Renamed;
				}
			}
			else
			{
				// default values
				IntlCurrencySymbol = "XXX";
				try
				{
					Currency_Renamed = Currency.GetInstance(IntlCurrencySymbol);
				}
				catch (IllegalArgumentException)
				{
				}
				CurrencySymbol_Renamed = "\u00A4";
			}
			// Currently the monetary decimal separator is the same as the
			// standard decimal separator for all locales that we support.
			// If that changes, add a new entry to NumberElements.
			MonetarySeparator = DecimalSeparator_Renamed;
		}

		/// <summary>
		/// Reads the default serializable fields, provides default values for objects
		/// in older serial versions, and initializes non-serializable fields.
		/// If <code>serialVersionOnStream</code>
		/// is less than 1, initializes <code>monetarySeparator</code> to be
		/// the same as <code>decimalSeparator</code> and <code>exponential</code>
		/// to be 'E'.
		/// If <code>serialVersionOnStream</code> is less than 2,
		/// initializes <code>locale</code>to the root locale, and initializes
		/// If <code>serialVersionOnStream</code> is less than 3, it initializes
		/// <code>exponentialSeparator</code> using <code>exponential</code>.
		/// Sets <code>serialVersionOnStream</code> back to the maximum allowed value so that
		/// default serialization will work properly if this object is streamed out again.
		/// Initializes the currency from the intlCurrencySymbol field.
		/// 
		/// @since JDK 1.1.6
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			if (SerialVersionOnStream < 1)
			{
				// Didn't have monetarySeparator or exponential field;
				// use defaults.
				MonetarySeparator = DecimalSeparator_Renamed;
				Exponential = 'E';
			}
			if (SerialVersionOnStream < 2)
			{
				// didn't have locale; use root locale
				Locale = Locale.ROOT;
			}
			if (SerialVersionOnStream < 3)
			{
				// didn't have exponentialSeparator. Create one using exponential
				ExponentialSeparator = char.ToString(Exponential);
			}
			SerialVersionOnStream = CurrentSerialVersion;

			if (IntlCurrencySymbol != null)
			{
				try
				{
					 Currency_Renamed = Currency.GetInstance(IntlCurrencySymbol);
				}
				catch (IllegalArgumentException)
				{
				}
			}
		}

		/// <summary>
		/// Character used for zero.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getZeroDigit </seealso>
		private char ZeroDigit_Renamed;

		/// <summary>
		/// Character used for thousands separator.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getGroupingSeparator </seealso>
		private char GroupingSeparator_Renamed;

		/// <summary>
		/// Character used for decimal sign.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getDecimalSeparator </seealso>
		private char DecimalSeparator_Renamed;

		/// <summary>
		/// Character used for per mille sign.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getPerMill </seealso>
		private char PerMill_Renamed;

		/// <summary>
		/// Character used for percent sign.
		/// @serial </summary>
		/// <seealso cref= #getPercent </seealso>
		private char Percent_Renamed;

		/// <summary>
		/// Character used for a digit in a pattern.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getDigit </seealso>
		private char Digit_Renamed;

		/// <summary>
		/// Character used to separate positive and negative subpatterns
		/// in a pattern.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getPatternSeparator </seealso>
		private char PatternSeparator_Renamed;

		/// <summary>
		/// String used to represent infinity.
		/// @serial </summary>
		/// <seealso cref= #getInfinity </seealso>
		private String Infinity_Renamed;

		/// <summary>
		/// String used to represent "not a number".
		/// @serial </summary>
		/// <seealso cref= #getNaN </seealso>
		private String NaN_Renamed;

		/// <summary>
		/// Character used to represent minus sign.
		/// @serial </summary>
		/// <seealso cref= #getMinusSign </seealso>
		private char MinusSign_Renamed;

		/// <summary>
		/// String denoting the local currency, e.g. "$".
		/// @serial </summary>
		/// <seealso cref= #getCurrencySymbol </seealso>
		private String CurrencySymbol_Renamed;

		/// <summary>
		/// ISO 4217 currency code denoting the local currency, e.g. "USD".
		/// @serial </summary>
		/// <seealso cref= #getInternationalCurrencySymbol </seealso>
		private String IntlCurrencySymbol;

		/// <summary>
		/// The decimal separator used when formatting currency values.
		/// @serial
		/// @since JDK 1.1.6 </summary>
		/// <seealso cref= #getMonetaryDecimalSeparator </seealso>
		private char MonetarySeparator; // Field new in JDK 1.1.6

		/// <summary>
		/// The character used to distinguish the exponent in a number formatted
		/// in exponential notation, e.g. 'E' for a number such as "1.23E45".
		/// <para>
		/// Note that the public API provides no way to set this field,
		/// even though it is supported by the implementation and the stream format.
		/// The intent is that this will be added to the API in the future.
		/// 
		/// @serial
		/// @since JDK 1.1.6
		/// </para>
		/// </summary>
		private char Exponential; // Field new in JDK 1.1.6

	  /// <summary>
	  /// The string used to separate the mantissa from the exponent.
	  /// Examples: "x10^" for 1.23x10^4, "E" for 1.23E4.
	  /// <para>
	  /// If both <code>exponential</code> and <code>exponentialSeparator</code>
	  /// exist, this <code>exponentialSeparator</code> has the precedence.
	  /// 
	  /// @serial
	  /// @since 1.6
	  /// </para>
	  /// </summary>
		private String ExponentialSeparator; // Field new in JDK 1.6

		/// <summary>
		/// The locale of these currency format symbols.
		/// 
		/// @serial
		/// @since 1.4
		/// </summary>
		private Locale Locale;

		// currency; only the ISO code is serialized.
		[NonSerialized]
		private Currency Currency_Renamed;

		// Proclaim JDK 1.1 FCS compatibility
		internal const long SerialVersionUID = 5772796243397350300L;

		// The internal serial version which says which version was written
		// - 0 (default) for version up to JDK 1.1.5
		// - 1 for version from JDK 1.1.6, which includes two new fields:
		//     monetarySeparator and exponential.
		// - 2 for version from J2SE 1.4, which includes locale field.
		// - 3 for version from J2SE 1.6, which includes exponentialSeparator field.
		private const int CurrentSerialVersion = 3;

		/// <summary>
		/// Describes the version of <code>DecimalFormatSymbols</code> present on the stream.
		/// Possible values are:
		/// <ul>
		/// <li><b>0</b> (or uninitialized): versions prior to JDK 1.1.6.
		/// 
		/// <li><b>1</b>: Versions written by JDK 1.1.6 or later, which include
		///      two new fields: <code>monetarySeparator</code> and <code>exponential</code>.
		/// <li><b>2</b>: Versions written by J2SE 1.4 or later, which include a
		///      new <code>locale</code> field.
		/// <li><b>3</b>: Versions written by J2SE 1.6 or later, which include a
		///      new <code>exponentialSeparator</code> field.
		/// </ul>
		/// When streaming out a <code>DecimalFormatSymbols</code>, the most recent format
		/// (corresponding to the highest allowable <code>serialVersionOnStream</code>)
		/// is always written.
		/// 
		/// @serial
		/// @since JDK 1.1.6
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;
	}

}