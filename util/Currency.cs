using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

/*
 * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool;
	using PlatformLogger = sun.util.logging.PlatformLogger;


	/// <summary>
	/// Represents a currency. Currencies are identified by their ISO 4217 currency
	/// codes. Visit the <a href="http://www.iso.org/iso/home/standards/currency_codes.htm">
	/// ISO web site</a> for more information.
	/// <para>
	/// The class is designed so that there's never more than one
	/// <code>Currency</code> instance for any given currency. Therefore, there's
	/// no public constructor. You obtain a <code>Currency</code> instance using
	/// the <code>getInstance</code> methods.
	/// </para>
	/// <para>
	/// Users can supersede the Java runtime currency data by means of the system
	/// property {@code java.util.currency.data}. If this system property is
	/// defined then its value is the location of a properties file, the contents of
	/// which are key/value pairs of the ISO 3166 country codes and the ISO 4217
	/// currency data respectively.  The value part consists of three ISO 4217 values
	/// of a currency, i.e., an alphabetic code, a numeric code, and a minor unit.
	/// Those three ISO 4217 values are separated by commas.
	/// The lines which start with '#'s are considered comment lines. An optional UTC
	/// timestamp may be specified per currency entry if users need to specify a
	/// cutover date indicating when the new data comes into effect. The timestamp is
	/// appended to the end of the currency properties and uses a comma as a separator.
	/// If a UTC datestamp is present and valid, the JRE will only use the new currency
	/// properties if the current UTC date is later than the date specified at class
	/// loading time. The format of the timestamp must be of ISO 8601 format :
	/// {@code 'yyyy-MM-dd'T'HH:mm:ss'}. For example,
	/// </para>
	/// <para>
	/// <code>
	/// #Sample currency properties<br>
	/// JP=JPZ,999,0
	/// </code>
	/// </para>
	/// <para>
	/// will supersede the currency data for Japan.
	/// 
	/// </para>
	/// <para>
	/// <code>
	/// #Sample currency properties with cutover date<br>
	/// JP=JPZ,999,0,2014-01-01T00:00:00
	/// </code>
	/// </para>
	/// <para>
	/// will supersede the currency data for Japan if {@code Currency} class is loaded after
	/// 1st January 2014 00:00:00 GMT.
	/// </para>
	/// <para>
	/// Where syntactically malformed entries are encountered, the entry is ignored
	/// and the remainder of entries in file are processed. For instances where duplicate
	/// country code entries exist, the behavior of the Currency information for that
	/// {@code Currency} is undefined and the remainder of entries in file are processed.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Currency
	{

		private const long SerialVersionUID = -158308464356906721L;

		/// <summary>
		/// ISO 4217 currency code for this currency.
		/// 
		/// @serial
		/// </summary>
		private readonly String CurrencyCode_Renamed;

		/// <summary>
		/// Default fraction digits for this currency.
		/// Set from currency data tables.
		/// </summary>
		[NonSerialized]
		private readonly int DefaultFractionDigits_Renamed;

		/// <summary>
		/// ISO 4217 numeric code for this currency.
		/// Set from currency data tables.
		/// </summary>
		[NonSerialized]
		private readonly int NumericCode_Renamed;


		// class data: instance map

		private static ConcurrentMap<String, Currency> Instances = new ConcurrentDictionary<String, Currency>(7);
		private static HashSet<Currency> Available;

		// Class data: currency data obtained from currency.data file.
		// Purpose:
		// - determine valid country codes
		// - determine valid currency codes
		// - map country codes to currency codes
		// - obtain default fraction digits for currency codes
		//
		// sc = special case; dfd = default fraction digits
		// Simple countries are those where the country code is a prefix of the
		// currency code, and there are no known plans to change the currency.
		//
		// table formats:
		// - mainTable:
		//   - maps country code to 32-bit int
		//   - 26*26 entries, corresponding to [A-Z]*[A-Z]
		//   - \u007F -> not valid country
		//   - bits 20-31: unused
		//   - bits 10-19: numeric code (0 to 1023)
		//   - bit 9: 1 - special case, bits 0-4 indicate which one
		//            0 - simple country, bits 0-4 indicate final char of currency code
		//   - bits 5-8: fraction digits for simple countries, 0 for special cases
		//   - bits 0-4: final char for currency code for simple country, or ID of special case
		// - special case IDs:
		//   - 0: country has no currency
		//   - other: index into sc* arrays + 1
		// - scCutOverTimes: cut-over time in millis as returned by
		//   System.currentTimeMillis for special case countries that are changing
		//   currencies; Long.MAX_VALUE for countries that are not changing currencies
		// - scOldCurrencies: old currencies for special case countries
		// - scNewCurrencies: new currencies for special case countries that are
		//   changing currencies; null for others
		// - scOldCurrenciesDFD: default fraction digits for old currencies
		// - scNewCurrenciesDFD: default fraction digits for new currencies, 0 for
		//   countries that are not changing currencies
		// - otherCurrencies: concatenation of all currency codes that are not the
		//   main currency of a simple country, separated by "-"
		// - otherCurrenciesDFD: decimal format digits for currencies in otherCurrencies, same order

		internal static int FormatVersion;
		internal static int DataVersion;
		internal static int[] MainTable;
		internal static long[] ScCutOverTimes;
		internal static String[] ScOldCurrencies;
		internal static String[] ScNewCurrencies;
		internal static int[] ScOldCurrenciesDFD;
		internal static int[] ScNewCurrenciesDFD;
		internal static int[] ScOldCurrenciesNumericCode;
		internal static int[] ScNewCurrenciesNumericCode;
		internal static String OtherCurrencies;
		internal static int[] OtherCurrenciesDFD;
		internal static int[] OtherCurrenciesNumericCode;

		// handy constants - must match definitions in GenerateCurrencyData
		// magic number
		private const int MAGIC_NUMBER = 0x43757244;
		// number of characters from A to Z
		private static readonly int A_TO_Z = ('Z' - 'A') + 1;
		// entry for invalid country codes
		private const int INVALID_COUNTRY_ENTRY = 0x0000007F;
		// entry for countries without currency
		private const int COUNTRY_WITHOUT_CURRENCY_ENTRY = 0x00000200;
		// mask for simple case country entries
		private const int SIMPLE_CASE_COUNTRY_MASK = 0x00000000;
		// mask for simple case country entry final character
		private const int SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK = 0x0000001F;
		// mask for simple case country entry default currency digits
		private const int SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK = 0x000001E0;
		// shift count for simple case country entry default currency digits
		private const int SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT = 5;
		// maximum number for simple case country entry default currency digits
		private const int SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS = 9;
		// mask for special case country entries
		private const int SPECIAL_CASE_COUNTRY_MASK = 0x00000200;
		// mask for special case country index
		private const int SPECIAL_CASE_COUNTRY_INDEX_MASK = 0x0000001F;
		// delta from entry index component in main table to index into special case tables
		private const int SPECIAL_CASE_COUNTRY_INDEX_DELTA = 1;
		// mask for distinguishing simple and special case countries
		private static readonly int COUNTRY_TYPE_MASK = SIMPLE_CASE_COUNTRY_MASK | SPECIAL_CASE_COUNTRY_MASK;
		// mask for the numeric code of the currency
		private const int NUMERIC_CODE_MASK = 0x000FFC00;
		// shift count for the numeric code of the currency
		private const int NUMERIC_CODE_SHIFT = 10;

		// Currency data format version
		private const int VALID_FORMAT_VERSION = 2;

		static Currency()
		{
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
				String homeDir = System.getProperty("java.home");
				try
				{
					String dataFile = homeDir + File.separator + "lib" + File.separator + "currency.data";
					using (DataInputStream dis = new DataInputStream(new BufferedInputStream(new FileInputStream(dataFile))))
					{
						if (dis.readInt() != MAGIC_NUMBER)
						{
							throw new InternalError("Currency data is possibly corrupted");
						}
						FormatVersion = dis.readInt();
						if (FormatVersion != VALID_FORMAT_VERSION)
						{
							throw new InternalError("Currency data format is incorrect");
						}
						DataVersion = dis.readInt();
						MainTable = ReadIntArray(dis, A_TO_Z * A_TO_Z);
						int scCount = dis.readInt();
						ScCutOverTimes = ReadLongArray(dis, scCount);
						ScOldCurrencies = ReadStringArray(dis, scCount);
						ScNewCurrencies = ReadStringArray(dis, scCount);
						ScOldCurrenciesDFD = ReadIntArray(dis, scCount);
						ScNewCurrenciesDFD = ReadIntArray(dis, scCount);
						ScOldCurrenciesNumericCode = ReadIntArray(dis, scCount);
						ScNewCurrenciesNumericCode = ReadIntArray(dis, scCount);
						int ocCount = dis.readInt();
						OtherCurrencies = dis.readUTF();
						OtherCurrenciesDFD = ReadIntArray(dis, ocCount);
						OtherCurrenciesNumericCode = ReadIntArray(dis, ocCount);
					}
				}
				catch (IOException e)
				{
					throw new InternalError(e);
				}

				// look for the properties file for overrides
				String propsFile = System.getProperty("java.util.currency.data");
				if (propsFile == null)
				{
					propsFile = homeDir + File.separator + "lib" + File.separator + "currency.properties";
				}
				try
				{
					File propFile = new File(propsFile);
					if (propFile.Exists())
					{
						Properties props = new Properties();
						using (FileReader fr = new FileReader(propFile))
						{
							props.Load(fr);
						}
						Set<String> keys = props.StringPropertyNames();
						Pattern propertiesPattern = Pattern.Compile("([A-Z]{3})\\s*,\\s*(\\d{3})\\s*,\\s*" + "(\\d+)\\s*,?\\s*(\\d{4}-\\d{2}-\\d{2}T\\d{2}:" + "\\d{2}:\\d{2})?");
						foreach (String key in keys)
						{
						   ReplaceCurrencyData(propertiesPattern, key.ToUpperCase(Locale.ROOT), props.GetProperty(key).ToUpperCase(Locale.ROOT));
						}
					}
				}
				catch (IOException e)
				{
					Info("currency.properties is ignored because of an IOException", e);
				}
				return null;
			}
		}

		/// <summary>
		/// Constants for retrieving localized names from the name providers.
		/// </summary>
		private const int SYMBOL = 0;
		private const int DISPLAYNAME = 1;


		/// <summary>
		/// Constructs a <code>Currency</code> instance. The constructor is private
		/// so that we can insure that there's never more than one instance for a
		/// given currency.
		/// </summary>
		private Currency(String currencyCode, int defaultFractionDigits, int numericCode)
		{
			this.CurrencyCode_Renamed = currencyCode;
			this.DefaultFractionDigits_Renamed = defaultFractionDigits;
			this.NumericCode_Renamed = numericCode;
		}

		/// <summary>
		/// Returns the <code>Currency</code> instance for the given currency code.
		/// </summary>
		/// <param name="currencyCode"> the ISO 4217 code of the currency </param>
		/// <returns> the <code>Currency</code> instance for the given currency code </returns>
		/// <exception cref="NullPointerException"> if <code>currencyCode</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not
		/// a supported ISO 4217 code. </exception>
		public static Currency GetInstance(String currencyCode)
		{
			return GetInstance(currencyCode, Integer.MinValue, 0);
		}

		private static Currency GetInstance(String currencyCode, int defaultFractionDigits, int numericCode)
		{
			// Try to look up the currency code in the instances table.
			// This does the null pointer check as a side effect.
			// Also, if there already is an entry, the currencyCode must be valid.
			Currency instance = Instances[currencyCode];
			if (instance != null)
			{
				return instance;
			}

			if (defaultFractionDigits == Integer.MinValue)
			{
				// Currency code not internally generated, need to verify first
				// A currency code must have 3 characters and exist in the main table
				// or in the list of other currencies.
				if (currencyCode.Length() != 3)
				{
					throw new IllegalArgumentException();
				}
				char char1 = currencyCode.CharAt(0);
				char char2 = currencyCode.CharAt(1);
				int tableEntry = GetMainTableEntry(char1, char2);
				if ((tableEntry & COUNTRY_TYPE_MASK) == SIMPLE_CASE_COUNTRY_MASK && tableEntry != INVALID_COUNTRY_ENTRY && currencyCode.CharAt(2) - 'A' == (tableEntry & SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK))
				{
					defaultFractionDigits = (tableEntry & SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT;
					numericCode = (tableEntry & NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT;
				}
				else
				{
					// Check for '-' separately so we don't get false hits in the table.
					if (currencyCode.CharAt(2) == '-')
					{
						throw new IllegalArgumentException();
					}
					int index = OtherCurrencies.IndexOf(currencyCode);
					if (index == -1)
					{
						throw new IllegalArgumentException();
					}
					defaultFractionDigits = OtherCurrenciesDFD[index / 4];
					numericCode = OtherCurrenciesNumericCode[index / 4];
				}
			}

			Currency currencyVal = new Currency(currencyCode, defaultFractionDigits, numericCode);
			instance = Instances.PutIfAbsent(currencyCode, currencyVal);
			return (instance != null ? instance : currencyVal);
		}

		/// <summary>
		/// Returns the <code>Currency</code> instance for the country of the
		/// given locale. The language and variant components of the locale
		/// are ignored. The result may vary over time, as countries change their
		/// currencies. For example, for the original member countries of the
		/// European Monetary Union, the method returns the old national currencies
		/// until December 31, 2001, and the Euro from January 1, 2002, local time
		/// of the respective countries.
		/// <para>
		/// The method returns <code>null</code> for territories that don't
		/// have a currency, such as Antarctica.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale"> the locale for whose country a <code>Currency</code>
		/// instance is needed </param>
		/// <returns> the <code>Currency</code> instance for the country of the given
		/// locale, or {@code null} </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> or its country
		/// code is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if the country of the given {@code locale}
		/// is not a supported ISO 3166 country code. </exception>
		public static Currency GetInstance(Locale locale)
		{
			String country = locale.Country;
			if (country == null)
			{
				throw new NullPointerException();
			}

			if (country.Length() != 2)
			{
				throw new IllegalArgumentException();
			}

			char char1 = country.CharAt(0);
			char char2 = country.CharAt(1);
			int tableEntry = GetMainTableEntry(char1, char2);
			if ((tableEntry & COUNTRY_TYPE_MASK) == SIMPLE_CASE_COUNTRY_MASK && tableEntry != INVALID_COUNTRY_ENTRY)
			{
				char finalChar = (char)((tableEntry & SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK) + 'A');
				int defaultFractionDigits = (tableEntry & SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT;
				int numericCode = (tableEntry & NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT;
				StringBuilder sb = new StringBuilder(country);
				sb.Append(finalChar);
				return GetInstance(sb.ToString(), defaultFractionDigits, numericCode);
			}
			else
			{
				// special cases
				if (tableEntry == INVALID_COUNTRY_ENTRY)
				{
					throw new IllegalArgumentException();
				}
				if (tableEntry == COUNTRY_WITHOUT_CURRENCY_ENTRY)
				{
					return null;
				}
				else
				{
					int index = (tableEntry & SPECIAL_CASE_COUNTRY_INDEX_MASK) - SPECIAL_CASE_COUNTRY_INDEX_DELTA;
					if (ScCutOverTimes[index] == Long.MaxValue || DateTimeHelperClass.CurrentUnixTimeMillis() < ScCutOverTimes[index])
					{
						return GetInstance(ScOldCurrencies[index], ScOldCurrenciesDFD[index], ScOldCurrenciesNumericCode[index]);
					}
					else
					{
						return GetInstance(ScNewCurrencies[index], ScNewCurrenciesDFD[index], ScNewCurrenciesNumericCode[index]);
					}
				}
			}
		}

		/// <summary>
		/// Gets the set of available currencies.  The returned set of currencies
		/// contains all of the available currencies, which may include currencies
		/// that represent obsolete ISO 4217 codes.  The set can be modified
		/// without affecting the available currencies in the runtime.
		/// </summary>
		/// <returns> the set of available currencies.  If there is no currency
		///    available in the runtime, the returned set is empty.
		/// @since 1.7 </returns>
		public static Set<Currency> AvailableCurrencies
		{
			get
			{
				lock (typeof(Currency))
				{
					if (Available == null)
					{
						Available = new HashSet<>(256);
    
						// Add simple currencies first
						for (char c1 = 'A'; c1 <= 'Z'; c1++)
						{
							for (char c2 = 'A'; c2 <= 'Z'; c2++)
							{
								int tableEntry = GetMainTableEntry(c1, c2);
								if ((tableEntry & COUNTRY_TYPE_MASK) == SIMPLE_CASE_COUNTRY_MASK && tableEntry != INVALID_COUNTRY_ENTRY)
								{
									char finalChar = (char)((tableEntry & SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK) + 'A');
									int defaultFractionDigits = (tableEntry & SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT;
									int numericCode = (tableEntry & NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT;
									StringBuilder sb = new StringBuilder();
									sb.Append(c1);
									sb.Append(c2);
									sb.Append(finalChar);
									Available.Add(GetInstance(sb.ToString(), defaultFractionDigits, numericCode));
								}
							}
						}
    
						// Now add other currencies
						StringTokenizer st = new StringTokenizer(OtherCurrencies, "-");
						while (st.HasMoreElements())
						{
							Available.Add(GetInstance((String)st.NextElement()));
						}
					}
				}
    
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") Set<Currency> result = (Set<Currency>) available.clone();
				Set<Currency> result = (Set<Currency>) Available.Clone();
				return result;
			}
		}

		/// <summary>
		/// Gets the ISO 4217 currency code of this currency.
		/// </summary>
		/// <returns> the ISO 4217 currency code of this currency. </returns>
		public String CurrencyCode
		{
			get
			{
				return CurrencyCode_Renamed;
			}
		}

		/// <summary>
		/// Gets the symbol of this currency for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		/// For example, for the US Dollar, the symbol is "$" if the default
		/// locale is the US, while for other locales it may be "US$". If no
		/// symbol can be determined, the ISO 4217 currency code is returned.
		/// <para>
		/// This is equivalent to calling
		/// {@link #getSymbol(Locale)
		///     getSymbol(Locale.getDefault(Locale.Category.DISPLAY))}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the symbol of this currency for the default
		///     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale </returns>
		public String Symbol
		{
			get
			{
				return GetSymbol(Locale.GetDefault(Locale.Category.DISPLAY));
			}
		}

		/// <summary>
		/// Gets the symbol of this currency for the specified locale.
		/// For example, for the US Dollar, the symbol is "$" if the specified
		/// locale is the US, while for other locales it may be "US$". If no
		/// symbol can be determined, the ISO 4217 currency code is returned.
		/// </summary>
		/// <param name="locale"> the locale for which a display name for this currency is
		/// needed </param>
		/// <returns> the symbol of this currency for the specified locale </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		public String GetSymbol(Locale locale)
		{
			LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(CurrencyNameProvider));
			String symbol = pool.getLocalizedObject(CurrencyNameGetter.INSTANCE, locale, CurrencyCode_Renamed, SYMBOL);
			if (symbol != null)
			{
				return symbol;
			}

			// use currency code as symbol of last resort
			return CurrencyCode_Renamed;
		}

		/// <summary>
		/// Gets the default number of fraction digits used with this currency.
		/// For example, the default number of fraction digits for the Euro is 2,
		/// while for the Japanese Yen it's 0.
		/// In the case of pseudo-currencies, such as IMF Special Drawing Rights,
		/// -1 is returned.
		/// </summary>
		/// <returns> the default number of fraction digits used with this currency </returns>
		public int DefaultFractionDigits
		{
			get
			{
				return DefaultFractionDigits_Renamed;
			}
		}

		/// <summary>
		/// Returns the ISO 4217 numeric code of this currency.
		/// </summary>
		/// <returns> the ISO 4217 numeric code of this currency
		/// @since 1.7 </returns>
		public int NumericCode
		{
			get
			{
				return NumericCode_Renamed;
			}
		}

		/// <summary>
		/// Gets the name that is suitable for displaying this currency for
		/// the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		/// If there is no suitable display name found
		/// for the default locale, the ISO 4217 currency code is returned.
		/// <para>
		/// This is equivalent to calling
		/// {@link #getDisplayName(Locale)
		///     getDisplayName(Locale.getDefault(Locale.Category.DISPLAY))}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the display name of this currency for the default
		///     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
		/// @since 1.7 </returns>
		public String DisplayName
		{
			get
			{
				return GetDisplayName(Locale.GetDefault(Locale.Category.DISPLAY));
			}
		}

		/// <summary>
		/// Gets the name that is suitable for displaying this currency for
		/// the specified locale.  If there is no suitable display name found
		/// for the specified locale, the ISO 4217 currency code is returned.
		/// </summary>
		/// <param name="locale"> the locale for which a display name for this currency is
		/// needed </param>
		/// <returns> the display name of this currency for the specified locale </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null
		/// @since 1.7 </exception>
		public String GetDisplayName(Locale locale)
		{
			LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(CurrencyNameProvider));
			String result = pool.getLocalizedObject(CurrencyNameGetter.INSTANCE, locale, CurrencyCode_Renamed, DISPLAYNAME);
			if (result != null)
			{
				return result;
			}

			// use currency code as symbol of last resort
			return CurrencyCode_Renamed;
		}

		/// <summary>
		/// Returns the ISO 4217 currency code of this currency.
		/// </summary>
		/// <returns> the ISO 4217 currency code of this currency </returns>
		public override String ToString()
		{
			return CurrencyCode_Renamed;
		}

		/// <summary>
		/// Resolves instances being deserialized to a single instance per currency.
		/// </summary>
		private Object ReadResolve()
		{
			return GetInstance(CurrencyCode_Renamed);
		}

		/// <summary>
		/// Gets the main table entry for the country whose country code consists
		/// of char1 and char2.
		/// </summary>
		private static int GetMainTableEntry(char char1, char char2)
		{
			if (char1 < 'A' || char1 > 'Z' || char2 < 'A' || char2 > 'Z')
			{
				throw new IllegalArgumentException();
			}
			return MainTable[(char1 - 'A') * A_TO_Z + (char2 - 'A')];
		}

		/// <summary>
		/// Sets the main table entry for the country whose country code consists
		/// of char1 and char2.
		/// </summary>
		private static void SetMainTableEntry(char char1, char char2, int entry)
		{
			if (char1 < 'A' || char1 > 'Z' || char2 < 'A' || char2 > 'Z')
			{
				throw new IllegalArgumentException();
			}
			MainTable[(char1 - 'A') * A_TO_Z + (char2 - 'A')] = entry;
		}

		/// <summary>
		/// Obtains a localized currency names from a CurrencyNameProvider
		/// implementation.
		/// </summary>
		private class CurrencyNameGetter : LocaleServiceProviderPool.LocalizedObjectGetter<CurrencyNameProvider, String>
		{
			internal static readonly CurrencyNameGetter INSTANCE = new CurrencyNameGetter();

			public override String GetObject(CurrencyNameProvider currencyNameProvider, Locale locale, String key, params Object[] @params)
			{
				Debug.Assert(@params.Length == 1);
				int type = (Integer)@params[0];

				switch (type)
				{
				case SYMBOL:
					return currencyNameProvider.GetSymbol(key, locale);
				case DISPLAYNAME:
					return currencyNameProvider.GetDisplayName(key, locale);
				default:
					Debug.Assert(false); // shouldn't happen
				break;
				}

				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int[] readIntArray(java.io.DataInputStream dis, int count) throws java.io.IOException
		private static int[] ReadIntArray(DataInputStream dis, int count)
		{
			int[] ret = new int[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = dis.ReadInt();
			}

			return ret;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long[] readLongArray(java.io.DataInputStream dis, int count) throws java.io.IOException
		private static long[] ReadLongArray(DataInputStream dis, int count)
		{
			long[] ret = new long[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = dis.ReadLong();
			}

			return ret;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String[] readStringArray(java.io.DataInputStream dis, int count) throws java.io.IOException
		private static String[] ReadStringArray(DataInputStream dis, int count)
		{
			String[] ret = new String[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = dis.ReadUTF();
			}

			return ret;
		}

		/// <summary>
		/// Replaces currency data found in the currencydata.properties file
		/// </summary>
		/// <param name="pattern"> regex pattern for the properties </param>
		/// <param name="ctry"> country code </param>
		/// <param name="curdata"> currency data.  This is a comma separated string that
		///    consists of "three-letter alphabet code", "three-digit numeric code",
		///    and "one-digit (0-9) default fraction digit".
		///    For example, "JPZ,392,0".
		///    An optional UTC date can be appended to the string (comma separated)
		///    to allow a currency change take effect after date specified.
		///    For example, "JP=JPZ,999,0,2014-01-01T00:00:00" has no effect unless
		///    UTC time is past 1st January 2014 00:00:00 GMT. </param>
		private static void ReplaceCurrencyData(Pattern pattern, String ctry, String curdata)
		{

			if (ctry.Length() != 2)
			{
				// ignore invalid country code
				Info("currency.properties entry for " + ctry + " is ignored because of the invalid country code.", null);
				return;
			}

			Matcher m = pattern.Matcher(curdata);
			if (!m.Find() || (m.Group(4) == null && CountOccurrences(curdata, ',') >= 3))
			{
				// format is not recognized.  ignore the data
				// if group(4) date string is null and we've 4 values, bad date value
				Info("currency.properties entry for " + ctry + " ignored because the value format is not recognized.", null);
				return;
			}

			try
			{
				if (m.Group(4) != null && !IsPastCutoverDate(m.Group(4)))
				{
					Info("currency.properties entry for " + ctry + " ignored since cutover date has not passed :" + curdata, null);
					return;
				}
			}
			catch (ParseException ex)
			{
				Info("currency.properties entry for " + ctry + " ignored since exception encountered :" + ex.Message, null);
				return;
			}

			String code = m.Group(1);
			int numeric = Convert.ToInt32(m.Group(2));
			int entry = numeric << NUMERIC_CODE_SHIFT;
			int fraction = Convert.ToInt32(m.Group(3));
			if (fraction > SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS)
			{
				Info("currency.properties entry for " + ctry + " ignored since the fraction is more than " + SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS + ":" + curdata, null);
				return;
			}

			int index;
			for (index = 0; index < ScOldCurrencies.Length; index++)
			{
				if (ScOldCurrencies[index].Equals(code))
				{
					break;
				}
			}

			if (index == ScOldCurrencies.Length)
			{
				// simple case
				entry |= (fraction << SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT) | (code.CharAt(2) - 'A');
			}
			else
			{
				// special case
				entry |= SPECIAL_CASE_COUNTRY_MASK | (index + SPECIAL_CASE_COUNTRY_INDEX_DELTA);
			}
			SetMainTableEntry(ctry.CharAt(0), ctry.CharAt(1), entry);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static boolean isPastCutoverDate(String s) throws java.text.ParseException
		private static bool IsPastCutoverDate(String s)
		{
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.ROOT);
			format.TimeZone = TimeZone.GetTimeZone("UTC");
			format.Lenient = false;
			long time = format.Parse(s.Trim()).Ticks;
			return DateTimeHelperClass.CurrentUnixTimeMillis() > time;

		}

		private static int CountOccurrences(String value, char match)
		{
			int count = 0;
			foreach (char c in value.ToCharArray())
			{
				if (c == match)
				{
				   ++count;
				}
			}
			return count;
		}

		private static void Info(String message, Throwable t)
		{
			PlatformLogger logger = PlatformLogger.getLogger("java.util.Currency");
			if (logger.isLoggable(PlatformLogger.Level.INFO))
			{
				if (t != null)
				{
					logger.info(message, t);
				}
				else
				{
					logger.info(message);
				}
			}
		}
	}

}