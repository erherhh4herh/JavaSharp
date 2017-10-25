using System;
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
	using ResourceBundleBasedAdapter = sun.util.locale.provider.ResourceBundleBasedAdapter;
	using TimeZoneNameUtility = sun.util.locale.provider.TimeZoneNameUtility;

	/// <summary>
	/// <code>DateFormatSymbols</code> is a public class for encapsulating
	/// localizable date-time formatting data, such as the names of the
	/// months, the names of the days of the week, and the time zone data.
	/// <code>SimpleDateFormat</code> uses
	/// <code>DateFormatSymbols</code> to encapsulate this information.
	/// 
	/// <para>
	/// Typically you shouldn't use <code>DateFormatSymbols</code> directly.
	/// Rather, you are encouraged to create a date-time formatter with the
	/// <code>DateFormat</code> class's factory methods: <code>getTimeInstance</code>,
	/// <code>getDateInstance</code>, or <code>getDateTimeInstance</code>.
	/// These methods automatically create a <code>DateFormatSymbols</code> for
	/// the formatter so that you don't have to. After the
	/// formatter is created, you may modify its format pattern using the
	/// <code>setPattern</code> method. For more information about
	/// creating formatters using <code>DateFormat</code>'s factory methods,
	/// see <seealso cref="DateFormat"/>.
	/// 
	/// </para>
	/// <para>
	/// If you decide to create a date-time formatter with a specific
	/// format pattern for a specific locale, you can do so with:
	/// <blockquote>
	/// <pre>
	/// new SimpleDateFormat(aPattern, DateFormatSymbols.getInstance(aLocale)).
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// <code>DateFormatSymbols</code> objects are cloneable. When you obtain
	/// a <code>DateFormatSymbols</code> object, feel free to modify the
	/// date-time formatting data. For instance, you can replace the localized
	/// date-time format pattern characters with the ones that you feel easy
	/// to remember. Or you can change the representative cities
	/// to your favorite ones.
	/// 
	/// </para>
	/// <para>
	/// New <code>DateFormatSymbols</code> subclasses may be added to support
	/// <code>SimpleDateFormat</code> for date-time formatting for additional locales.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          DateFormat </seealso>
	/// <seealso cref=          SimpleDateFormat </seealso>
	/// <seealso cref=          java.util.SimpleTimeZone
	/// @author       Chen-Lieh Huang </seealso>
	[Serializable]
	public class DateFormatSymbols : Cloneable
	{

		/// <summary>
		/// Construct a DateFormatSymbols object by loading format data from
		/// resources for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/>
		/// locale. This constructor can only
		/// construct instances for the locales supported by the Java
		/// runtime environment, not for those supported by installed
		/// <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		/// implementations. For full locale coverage, use the
		/// <seealso cref="#getInstance(Locale) getInstance"/> method.
		/// <para>This is equivalent to calling
		/// {@link #DateFormatSymbols(Locale)
		///     DateFormatSymbols(Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= #getInstance() </seealso>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <exception cref="java.util.MissingResourceException">
		///             if the resources for the default locale cannot be
		///             found or cannot be loaded. </exception>
		public DateFormatSymbols()
		{
			InitializeData(Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Construct a DateFormatSymbols object by loading format data from
		/// resources for the given locale. This constructor can only
		/// construct instances for the locales supported by the Java
		/// runtime environment, not for those supported by installed
		/// <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		/// implementations. For full locale coverage, use the
		/// <seealso cref="#getInstance(Locale) getInstance"/> method.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <seealso cref= #getInstance(Locale) </seealso>
		/// <exception cref="java.util.MissingResourceException">
		///             if the resources for the specified locale cannot be
		///             found or cannot be loaded. </exception>
		public DateFormatSymbols(Locale locale)
		{
			InitializeData(locale);
		}

		/// <summary>
		/// Era strings. For example: "AD" and "BC".  An array of 2 strings,
		/// indexed by <code>Calendar.BC</code> and <code>Calendar.AD</code>.
		/// @serial
		/// </summary>
		internal String[] Eras_Renamed = null;

		/// <summary>
		/// Month strings. For example: "January", "February", etc.  An array
		/// of 13 strings (some calendars have 13 months), indexed by
		/// <code>Calendar.JANUARY</code>, <code>Calendar.FEBRUARY</code>, etc.
		/// @serial
		/// </summary>
		internal String[] Months_Renamed = null;

		/// <summary>
		/// Short month strings. For example: "Jan", "Feb", etc.  An array of
		/// 13 strings (some calendars have 13 months), indexed by
		/// <code>Calendar.JANUARY</code>, <code>Calendar.FEBRUARY</code>, etc.
		/// 
		/// @serial
		/// </summary>
		internal String[] ShortMonths_Renamed = null;

		/// <summary>
		/// Weekday strings. For example: "Sunday", "Monday", etc.  An array
		/// of 8 strings, indexed by <code>Calendar.SUNDAY</code>,
		/// <code>Calendar.MONDAY</code>, etc.
		/// The element <code>weekdays[0]</code> is ignored.
		/// @serial
		/// </summary>
		internal String[] Weekdays_Renamed = null;

		/// <summary>
		/// Short weekday strings. For example: "Sun", "Mon", etc.  An array
		/// of 8 strings, indexed by <code>Calendar.SUNDAY</code>,
		/// <code>Calendar.MONDAY</code>, etc.
		/// The element <code>shortWeekdays[0]</code> is ignored.
		/// @serial
		/// </summary>
		internal String[] ShortWeekdays_Renamed = null;

		/// <summary>
		/// AM and PM strings. For example: "AM" and "PM".  An array of
		/// 2 strings, indexed by <code>Calendar.AM</code> and
		/// <code>Calendar.PM</code>.
		/// @serial
		/// </summary>
		internal String[] Ampms = null;

		/// <summary>
		/// Localized names of time zones in this locale.  This is a
		/// two-dimensional array of strings of size <em>n</em> by <em>m</em>,
		/// where <em>m</em> is at least 5.  Each of the <em>n</em> rows is an
		/// entry containing the localized names for a single <code>TimeZone</code>.
		/// Each such row contains (with <code>i</code> ranging from
		/// 0..<em>n</em>-1):
		/// <ul>
		/// <li><code>zoneStrings[i][0]</code> - time zone ID</li>
		/// <li><code>zoneStrings[i][1]</code> - long name of zone in standard
		/// time</li>
		/// <li><code>zoneStrings[i][2]</code> - short name of zone in
		/// standard time</li>
		/// <li><code>zoneStrings[i][3]</code> - long name of zone in daylight
		/// saving time</li>
		/// <li><code>zoneStrings[i][4]</code> - short name of zone in daylight
		/// saving time</li>
		/// </ul>
		/// The zone ID is <em>not</em> localized; it's one of the valid IDs of
		/// the <seealso cref="java.util.TimeZone TimeZone"/> class that are not
		/// <a href="../java/util/TimeZone.html#CustomID">custom IDs</a>.
		/// All other entries are localized names. </summary>
		/// <seealso cref= java.util.TimeZone
		/// @serial </seealso>
		internal String[][] ZoneStrings_Renamed = null;

		/// <summary>
		/// Indicates that zoneStrings is set externally with setZoneStrings() method.
		/// </summary>
		[NonSerialized]
		internal bool IsZoneStringsSet = false;

		/// <summary>
		/// Unlocalized date-time pattern characters. For example: 'y', 'd', etc.
		/// All locales use the same these unlocalized pattern characters.
		/// </summary>
		internal const String PatternChars = "GyMdkHmsSEDFwWahKzZYuXL";

		internal const int PATTERN_ERA = 0; // G
		internal const int PATTERN_YEAR = 1; // y
		internal const int PATTERN_MONTH = 2; // M
		internal const int PATTERN_DAY_OF_MONTH = 3; // d
		internal const int PATTERN_HOUR_OF_DAY1 = 4; // k
		internal const int PATTERN_HOUR_OF_DAY0 = 5; // H
		internal const int PATTERN_MINUTE = 6; // m
		internal const int PATTERN_SECOND = 7; // s
		internal const int PATTERN_MILLISECOND = 8; // S
		internal const int PATTERN_DAY_OF_WEEK = 9; // E
		internal const int PATTERN_DAY_OF_YEAR = 10; // D
		internal const int PATTERN_DAY_OF_WEEK_IN_MONTH = 11; // F
		internal const int PATTERN_WEEK_OF_YEAR = 12; // w
		internal const int PATTERN_WEEK_OF_MONTH = 13; // W
		internal const int PATTERN_AM_PM = 14; // a
		internal const int PATTERN_HOUR1 = 15; // h
		internal const int PATTERN_HOUR0 = 16; // K
		internal const int PATTERN_ZONE_NAME = 17; // z
		internal const int PATTERN_ZONE_VALUE = 18; // Z
		internal const int PATTERN_WEEK_YEAR = 19; // Y
		internal const int PATTERN_ISO_DAY_OF_WEEK = 20; // u
		internal const int PATTERN_ISO_ZONE = 21; // X
		internal const int PATTERN_MONTH_STANDALONE = 22; // L

		/// <summary>
		/// Localized date-time pattern characters. For example, a locale may
		/// wish to use 'u' rather than 'y' to represent years in its date format
		/// pattern strings.
		/// This string must be exactly 18 characters long, with the index of
		/// the characters described by <code>DateFormat.ERA_FIELD</code>,
		/// <code>DateFormat.YEAR_FIELD</code>, etc.  Thus, if the string were
		/// "Xz...", then localized patterns would use 'X' for era and 'z' for year.
		/// @serial
		/// </summary>
		internal String LocalPatternChars_Renamed = null;

		/// <summary>
		/// The locale which is used for initializing this DateFormatSymbols object.
		/// 
		/// @since 1.6
		/// @serial
		/// </summary>
		internal Locale Locale = null;

		/* use serialVersionUID from JDK 1.1.4 for interoperability */
		internal const long SerialVersionUID = -5987973545549424702L;

		/// <summary>
		/// Returns an array of all locales for which the
		/// <code>getInstance</code> methods of this class can return
		/// localized instances.
		/// The returned array represents the union of locales supported by the
		/// Java runtime and by installed
		/// <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		/// implementations.  It must contain at least a <code>Locale</code>
		/// instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of locales for which localized
		///         <code>DateFormatSymbols</code> instances are available.
		/// @since 1.6 </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(DateFormatSymbolsProvider));
				return pool.AvailableLocales;
			}
		}

		/// <summary>
		/// Gets the <code>DateFormatSymbols</code> instance for the default
		/// locale.  This method provides access to <code>DateFormatSymbols</code>
		/// instances for locales supported by the Java runtime itself as well
		/// as for those supported by installed
		/// <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		/// implementations.
		/// <para>This is equivalent to calling {@link #getInstance(Locale)
		///     getInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> a <code>DateFormatSymbols</code> instance.
		/// @since 1.6 </returns>
		public static DateFormatSymbols Instance
		{
			get
			{
				return GetInstance(Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets the <code>DateFormatSymbols</code> instance for the specified
		/// locale.  This method provides access to <code>DateFormatSymbols</code>
		/// instances for locales supported by the Java runtime itself as well
		/// as for those supported by installed
		/// <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		/// implementations. </summary>
		/// <param name="locale"> the given locale. </param>
		/// <returns> a <code>DateFormatSymbols</code> instance. </returns>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null
		/// @since 1.6 </exception>
		public static DateFormatSymbols GetInstance(Locale locale)
		{
			DateFormatSymbols dfs = GetProviderInstance(locale);
			if (dfs != null)
			{
				return dfs;
			}
			throw new RuntimeException("DateFormatSymbols instance creation failed.");
		}

		/// <summary>
		/// Returns a DateFormatSymbols provided by a provider or found in
		/// the cache. Note that this method returns a cached instance,
		/// not its clone. Therefore, the instance should never be given to
		/// an application.
		/// </summary>
		internal static DateFormatSymbols GetInstanceRef(Locale locale)
		{
			DateFormatSymbols dfs = GetProviderInstance(locale);
			if (dfs != null)
			{
				return dfs;
			}
			throw new RuntimeException("DateFormatSymbols instance creation failed.");
		}

		private static DateFormatSymbols GetProviderInstance(Locale locale)
		{
			LocaleProviderAdapter adapter = LocaleProviderAdapter.getAdapter(typeof(DateFormatSymbolsProvider), locale);
			DateFormatSymbolsProvider provider = adapter.DateFormatSymbolsProvider;
			DateFormatSymbols dfsyms = provider.GetInstance(locale);
			if (dfsyms == null)
			{
				provider = LocaleProviderAdapter.forJRE().DateFormatSymbolsProvider;
				dfsyms = provider.GetInstance(locale);
			}
			return dfsyms;
		}

		/// <summary>
		/// Gets era strings. For example: "AD" and "BC". </summary>
		/// <returns> the era strings. </returns>
		public virtual String[] Eras
		{
			get
			{
				return Arrays.CopyOf(Eras_Renamed, Eras_Renamed.Length);
			}
			set
			{
				Eras_Renamed = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets month strings. For example: "January", "February", etc.
		/// 
		/// <para>If the language requires different forms for formatting and
		/// stand-alone usages, this method returns month names in the
		/// formatting form. For example, the preferred month name for
		/// January in the Czech language is <em>ledna</em> in the
		/// formatting form, while it is <em>leden</em> in the stand-alone
		/// form. This method returns {@code "ledna"} in this case. Refer
		/// to the <a href="http://unicode.org/reports/tr35/#Calendar_Elements">
		/// Calendar Elements in the Unicode Locale Data Markup Language
		/// (LDML) specification</a> for more details.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month strings. </returns>
		public virtual String[] Months
		{
			get
			{
				return Arrays.CopyOf(Months_Renamed, Months_Renamed.Length);
			}
			set
			{
				Months_Renamed = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets short month strings. For example: "Jan", "Feb", etc.
		/// 
		/// <para>If the language requires different forms for formatting and
		/// stand-alone usages, This method returns short month names in
		/// the formatting form. For example, the preferred abbreviation
		/// for January in the Catalan language is <em>de gen.</em> in the
		/// formatting form, while it is <em>gen.</em> in the stand-alone
		/// form. This method returns {@code "de gen."} in this case. Refer
		/// to the <a href="http://unicode.org/reports/tr35/#Calendar_Elements">
		/// Calendar Elements in the Unicode Locale Data Markup Language
		/// (LDML) specification</a> for more details.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the short month strings. </returns>
		public virtual String[] ShortMonths
		{
			get
			{
				return Arrays.CopyOf(ShortMonths_Renamed, ShortMonths_Renamed.Length);
			}
			set
			{
				ShortMonths_Renamed = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets weekday strings. For example: "Sunday", "Monday", etc. </summary>
		/// <returns> the weekday strings. Use <code>Calendar.SUNDAY</code>,
		/// <code>Calendar.MONDAY</code>, etc. to index the result array. </returns>
		public virtual String[] Weekdays
		{
			get
			{
				return Arrays.CopyOf(Weekdays_Renamed, Weekdays_Renamed.Length);
			}
			set
			{
				Weekdays_Renamed = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets short weekday strings. For example: "Sun", "Mon", etc. </summary>
		/// <returns> the short weekday strings. Use <code>Calendar.SUNDAY</code>,
		/// <code>Calendar.MONDAY</code>, etc. to index the result array. </returns>
		public virtual String[] ShortWeekdays
		{
			get
			{
				return Arrays.CopyOf(ShortWeekdays_Renamed, ShortWeekdays_Renamed.Length);
			}
			set
			{
				ShortWeekdays_Renamed = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets ampm strings. For example: "AM" and "PM". </summary>
		/// <returns> the ampm strings. </returns>
		public virtual String[] AmPmStrings
		{
			get
			{
				return Arrays.CopyOf(Ampms, Ampms.Length);
			}
			set
			{
				Ampms = Arrays.CopyOf(value, value.Length);
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets time zone strings.  Use of this method is discouraged; use
		/// <seealso cref="java.util.TimeZone#getDisplayName() TimeZone.getDisplayName()"/>
		/// instead.
		/// <para>
		/// The value returned is a
		/// two-dimensional array of strings of size <em>n</em> by <em>m</em>,
		/// where <em>m</em> is at least 5.  Each of the <em>n</em> rows is an
		/// entry containing the localized names for a single <code>TimeZone</code>.
		/// Each such row contains (with <code>i</code> ranging from
		/// 0..<em>n</em>-1):
		/// <ul>
		/// <li><code>zoneStrings[i][0]</code> - time zone ID</li>
		/// <li><code>zoneStrings[i][1]</code> - long name of zone in standard
		/// time</li>
		/// <li><code>zoneStrings[i][2]</code> - short name of zone in
		/// standard time</li>
		/// <li><code>zoneStrings[i][3]</code> - long name of zone in daylight
		/// saving time</li>
		/// <li><code>zoneStrings[i][4]</code> - short name of zone in daylight
		/// saving time</li>
		/// </ul>
		/// The zone ID is <em>not</em> localized; it's one of the valid IDs of
		/// the <seealso cref="java.util.TimeZone TimeZone"/> class that are not
		/// <a href="../util/TimeZone.html#CustomID">custom IDs</a>.
		/// All other entries are localized names.  If a zone does not implement
		/// daylight saving time, the daylight saving time names should not be used.
		/// </para>
		/// <para>
		/// If <seealso cref="#setZoneStrings(String[][]) setZoneStrings"/> has been called
		/// on this <code>DateFormatSymbols</code> instance, then the strings
		/// provided by that call are returned. Otherwise, the returned array
		/// contains names provided by the Java runtime and by installed
		/// <seealso cref="java.util.spi.TimeZoneNameProvider TimeZoneNameProvider"/>
		/// implementations.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time zone strings. </returns>
		/// <seealso cref= #setZoneStrings(String[][]) </seealso>
		public virtual String[][] ZoneStrings
		{
			get
			{
				return GetZoneStringsImpl(true);
			}
			set
			{
				String[][] aCopy = new String[value.Length][];
				for (int i = 0; i < value.Length; ++i)
				{
					int len = value[i].Length;
					if (len < 5)
					{
						throw new IllegalArgumentException();
					}
					aCopy[i] = Arrays.CopyOf(value[i], len);
				}
				ZoneStrings_Renamed = aCopy;
				IsZoneStringsSet = true;
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Gets localized date-time pattern characters. For example: 'u', 't', etc. </summary>
		/// <returns> the localized date-time pattern characters. </returns>
		public virtual String LocalPatternChars
		{
			get
			{
				return LocalPatternChars_Renamed;
			}
			set
			{
				// Call toString() to throw an NPE in case the argument is null
				LocalPatternChars_Renamed = value.ToString();
				CachedHashCode = 0;
			}
		}


		/// <summary>
		/// Overrides Cloneable
		/// </summary>
		public virtual Object Clone()
		{
			try
			{
				DateFormatSymbols other = (DateFormatSymbols)base.Clone();
				CopyMembers(this, other);
				return other;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Override hashCode.
		/// Generates a hash code for the DateFormatSymbols object.
		/// </summary>
		public override int HashCode()
		{
			int hashCode = CachedHashCode;
			if (hashCode == 0)
			{
				hashCode = 5;
				hashCode = 11 * hashCode + Arrays.HashCode(Eras_Renamed);
				hashCode = 11 * hashCode + Arrays.HashCode(Months_Renamed);
				hashCode = 11 * hashCode + Arrays.HashCode(ShortMonths_Renamed);
				hashCode = 11 * hashCode + Arrays.HashCode(Weekdays_Renamed);
				hashCode = 11 * hashCode + Arrays.HashCode(ShortWeekdays_Renamed);
				hashCode = 11 * hashCode + Arrays.HashCode(Ampms);
				hashCode = 11 * hashCode + Arrays.DeepHashCode(ZoneStringsWrapper);
				hashCode = 11 * hashCode + Objects.HashCode(LocalPatternChars_Renamed);
				CachedHashCode = hashCode;
			}

			return hashCode;
		}

		/// <summary>
		/// Override equals
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
			DateFormatSymbols that = (DateFormatSymbols) obj;
			return (Arrays.Equals(Eras_Renamed, that.Eras_Renamed) && Arrays.Equals(Months_Renamed, that.Months_Renamed) && Arrays.Equals(ShortMonths_Renamed, that.ShortMonths_Renamed) && Arrays.Equals(Weekdays_Renamed, that.Weekdays_Renamed) && Arrays.Equals(ShortWeekdays_Renamed, that.ShortWeekdays_Renamed) && Arrays.Equals(Ampms, that.Ampms) && Arrays.DeepEquals(ZoneStringsWrapper, that.ZoneStringsWrapper) && ((LocalPatternChars_Renamed != null && LocalPatternChars_Renamed.Equals(that.LocalPatternChars_Renamed)) || (LocalPatternChars_Renamed == null && that.LocalPatternChars_Renamed == null)));
		}

		// =======================privates===============================

		/// <summary>
		/// Useful constant for defining time zone offsets.
		/// </summary>
		internal const int MillisPerHour = 60 * 60 * 1000;

		/// <summary>
		/// Cache to hold DateFormatSymbols instances per Locale.
		/// </summary>
		private static readonly ConcurrentMap<Locale, SoftReference<DateFormatSymbols>> CachedInstances = new ConcurrentDictionary<Locale, SoftReference<DateFormatSymbols>>(3);

		[NonSerialized]
		private int LastZoneIndex = 0;

		/// <summary>
		/// Cached hash code
		/// </summary>
		[NonSerialized]
		internal volatile int CachedHashCode = 0;

		private void InitializeData(Locale desiredLocale)
		{
			Locale = desiredLocale;

			// Copy values of a cached instance if any.
			SoftReference<DateFormatSymbols> @ref = CachedInstances[Locale];
			DateFormatSymbols dfs;
			if (@ref != null && (dfs = @ref.get()) != null)
			{
				CopyMembers(dfs, this);
				return;
			}

			// Initialize the fields from the ResourceBundle for locale.
			LocaleProviderAdapter adapter = LocaleProviderAdapter.getAdapter(typeof(DateFormatSymbolsProvider), Locale);
			// Avoid any potential recursions
			if (!(adapter is ResourceBundleBasedAdapter))
			{
				adapter = LocaleProviderAdapter.ResourceBundleBased;
			}
			ResourceBundle resource = ((ResourceBundleBasedAdapter)adapter).LocaleData.getDateFormatData(Locale);

			// JRE and CLDR use different keys
			// JRE: Eras, short.Eras and narrow.Eras
			// CLDR: long.Eras, Eras and narrow.Eras
			if (resource.ContainsKey("Eras"))
			{
				Eras_Renamed = resource.GetStringArray("Eras");
			}
			else if (resource.ContainsKey("long.Eras"))
			{
				Eras_Renamed = resource.GetStringArray("long.Eras");
			}
			else if (resource.ContainsKey("short.Eras"))
			{
				Eras_Renamed = resource.GetStringArray("short.Eras");
			}
			Months_Renamed = resource.GetStringArray("MonthNames");
			ShortMonths_Renamed = resource.GetStringArray("MonthAbbreviations");
			Ampms = resource.GetStringArray("AmPmMarkers");
			LocalPatternChars_Renamed = resource.GetString("DateTimePatternChars");

			// Day of week names are stored in a 1-based array.
			Weekdays_Renamed = ToOneBasedArray(resource.GetStringArray("DayNames"));
			ShortWeekdays_Renamed = ToOneBasedArray(resource.GetStringArray("DayAbbreviations"));

			// Put a clone in the cache
			@ref = new SoftReference<>((DateFormatSymbols)this.Clone());
			SoftReference<DateFormatSymbols> x = CachedInstances.PutIfAbsent(Locale, @ref);
			if (x != null)
			{
				DateFormatSymbols y = x.get();
				if (y == null)
				{
					// Replace the empty SoftReference with ref.
					CachedInstances[Locale] = @ref;
				}
			}
		}

		private static String[] ToOneBasedArray(String[] src)
		{
			int len = src.Length;
			String[] dst = new String[len + 1];
			dst[0] = "";
			for (int i = 0; i < len; i++)
			{
				dst[i + 1] = src[i];
			}
			return dst;
		}

		/// <summary>
		/// Package private: used by SimpleDateFormat
		/// Gets the index for the given time zone ID to obtain the time zone
		/// strings for formatting. The time zone ID is just for programmatic
		/// lookup. NOT LOCALIZED!!! </summary>
		/// <param name="ID"> the given time zone ID. </param>
		/// <returns> the index of the given time zone ID.  Returns -1 if
		/// the given time zone ID can't be located in the DateFormatSymbols object. </returns>
		/// <seealso cref= java.util.SimpleTimeZone </seealso>
		internal int GetZoneIndex(String ID)
		{
			String[][] zoneStrings = ZoneStringsWrapper;

			/*
			 * getZoneIndex has been re-written for performance reasons. instead of
			 * traversing the zoneStrings array every time, we cache the last used zone
			 * index
			 */
			if (LastZoneIndex < zoneStrings.Length && ID.Equals(zoneStrings[LastZoneIndex][0]))
			{
				return LastZoneIndex;
			}

			/* slow path, search entire list */
			for (int index = 0; index < zoneStrings.Length; index++)
			{
				if (ID.Equals(zoneStrings[index][0]))
				{
					LastZoneIndex = index;
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// Wrapper method to the getZoneStrings(), which is called from inside
		/// the java.text package and not to mutate the returned arrays, so that
		/// it does not need to create a defensive copy.
		/// </summary>
		internal String[][] ZoneStringsWrapper
		{
			get
			{
				if (SubclassObject)
				{
					return ZoneStrings;
				}
				else
				{
					return GetZoneStringsImpl(false);
				}
			}
		}

		private String[][] GetZoneStringsImpl(bool needsCopy)
		{
			if (ZoneStrings_Renamed == null)
			{
				ZoneStrings_Renamed = TimeZoneNameUtility.getZoneStrings(Locale);
			}

			if (!needsCopy)
			{
				return ZoneStrings_Renamed;
			}

			int len = ZoneStrings_Renamed.Length;
			String[][] aCopy = new String[len][];
			for (int i = 0; i < len; i++)
			{
				aCopy[i] = Arrays.CopyOf(ZoneStrings_Renamed[i], ZoneStrings_Renamed[i].Length);
			}
			return aCopy;
		}

		private bool SubclassObject
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return !this.GetType().FullName.Equals("java.text.DateFormatSymbols");
			}
		}

		/// <summary>
		/// Clones all the data members from the source DateFormatSymbols to
		/// the target DateFormatSymbols. This is only for subclasses. </summary>
		/// <param name="src"> the source DateFormatSymbols. </param>
		/// <param name="dst"> the target DateFormatSymbols. </param>
		private void CopyMembers(DateFormatSymbols src, DateFormatSymbols dst)
		{
			dst.Eras_Renamed = Arrays.CopyOf(src.Eras_Renamed, src.Eras_Renamed.Length);
			dst.Months_Renamed = Arrays.CopyOf(src.Months_Renamed, src.Months_Renamed.Length);
			dst.ShortMonths_Renamed = Arrays.CopyOf(src.ShortMonths_Renamed, src.ShortMonths_Renamed.Length);
			dst.Weekdays_Renamed = Arrays.CopyOf(src.Weekdays_Renamed, src.Weekdays_Renamed.Length);
			dst.ShortWeekdays_Renamed = Arrays.CopyOf(src.ShortWeekdays_Renamed, src.ShortWeekdays_Renamed.Length);
			dst.Ampms = Arrays.CopyOf(src.Ampms, src.Ampms.Length);
			if (src.ZoneStrings_Renamed != null)
			{
				dst.ZoneStrings_Renamed = src.GetZoneStringsImpl(true);
			}
			else
			{
				dst.ZoneStrings_Renamed = null;
			}
			dst.LocalPatternChars_Renamed = src.LocalPatternChars_Renamed;
			dst.CachedHashCode = 0;
		}

		/// <summary>
		/// Write out the default serializable data, after ensuring the
		/// <code>zoneStrings</code> field is initialized in order to make
		/// sure the backward compatibility.
		/// 
		/// @since 1.6
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		private void WriteObject(ObjectOutputStream stream)
		{
			if (ZoneStrings_Renamed == null)
			{
				ZoneStrings_Renamed = TimeZoneNameUtility.getZoneStrings(Locale);
			}
			stream.DefaultWriteObject();
		}
	}

}