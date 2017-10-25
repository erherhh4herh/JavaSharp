/*
 * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// An abstract class for service providers that
	/// provide localized time zone names for the
	/// <seealso cref="java.util.TimeZone TimeZone"/> class.
	/// The localized time zone names available from the implementations of
	/// this class are also the source for the
	/// {@link java.text.DateFormatSymbols#getZoneStrings()
	/// DateFormatSymbols.getZoneStrings()} method.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class TimeZoneNameProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal TimeZoneNameProvider()
		{
		}

		/// <summary>
		/// Returns a name for the given time zone ID that's suitable for
		/// presentation to the user in the specified locale. The given time
		/// zone ID is "GMT" or one of the names defined using "Zone" entries
		/// in the "tz database", a public domain time zone database at
		/// <a href="ftp://elsie.nci.nih.gov/pub/">ftp://elsie.nci.nih.gov/pub/</a>.
		/// The data of this database is contained in a file whose name starts with
		/// "tzdata", and the specification of the data format is part of the zic.8
		/// man page, which is contained in a file whose name starts with "tzcode".
		/// <para>
		/// If <code>daylight</code> is true, the method should return a name
		/// appropriate for daylight saving time even if the specified time zone
		/// has not observed daylight saving time in the past.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ID"> a time zone ID string </param>
		/// <param name="daylight"> if true, return the daylight saving name. </param>
		/// <param name="style"> either <seealso cref="java.util.TimeZone#LONG TimeZone.LONG"/> or
		///    <seealso cref="java.util.TimeZone#SHORT TimeZone.SHORT"/> </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the human-readable name of the given time zone in the
		///     given locale, or null if it's not available. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		///     or <code>locale</code> isn't one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>ID</code> or <code>locale</code>
		///     is null </exception>
		/// <seealso cref= java.util.TimeZone#getDisplayName(boolean, int, java.util.Locale) </seealso>
		public abstract String GetDisplayName(String ID, bool daylight, int style, Locale locale);

		/// <summary>
		/// Returns a generic name for the given time zone {@code ID} that's suitable
		/// for presentation to the user in the specified {@code locale}. Generic
		/// time zone names are neutral from standard time and daylight saving
		/// time. For example, "PT" is the short generic name of time zone ID {@code
		/// America/Los_Angeles}, while its short standard time and daylight saving
		/// time names are "PST" and "PDT", respectively. Refer to
		/// <seealso cref="#getDisplayName(String, boolean, int, Locale) getDisplayName"/>
		/// for valid time zone IDs.
		/// 
		/// <para>The default implementation of this method returns {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ID"> a time zone ID string </param>
		/// <param name="style"> either <seealso cref="java.util.TimeZone#LONG TimeZone.LONG"/> or
		///    <seealso cref="java.util.TimeZone#SHORT TimeZone.SHORT"/> </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the human-readable generic name of the given time zone in the
		///     given locale, or {@code null} if it's not available. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		///     or <code>locale</code> isn't one of the locales returned from
		///     {@link LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>ID</code> or <code>locale</code>
		///     is {@code null}
		/// @since 1.8 </exception>
		public virtual String GetGenericDisplayName(String ID, int style, Locale locale)
		{
			return null;
		}
	}

}