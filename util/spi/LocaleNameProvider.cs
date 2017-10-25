/*
 * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// provide localized names for the
	/// <seealso cref="java.util.Locale Locale"/> class.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class LocaleNameProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal LocaleNameProvider()
		{
		}

		/// <summary>
		/// Returns a localized name for the given <a href="http://www.rfc-editor.org/rfc/bcp/bcp47.txt">
		/// IETF BCP47</a> language code and the given locale that is appropriate for
		/// display to the user.
		/// For example, if <code>languageCode</code> is "fr" and <code>locale</code>
		/// is en_US, getDisplayLanguage() will return "French"; if <code>languageCode</code>
		/// is "en" and <code>locale</code> is fr_FR, getDisplayLanguage() will return "anglais".
		/// If the name returned cannot be localized according to <code>locale</code>,
		/// (say, the provider does not have a Japanese name for Croatian),
		/// this method returns null. </summary>
		/// <param name="languageCode"> the language code string in the form of two to eight
		///     lower-case letters between 'a' (U+0061) and 'z' (U+007A) </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the name of the given language code for the specified locale, or null if it's not
		///     available. </returns>
		/// <exception cref="NullPointerException"> if <code>languageCode</code> or <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>languageCode</code> is not in the form of
		///     two or three lower-case letters, or <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <seealso cref= java.util.Locale#getDisplayLanguage(java.util.Locale) </seealso>
		public abstract String GetDisplayLanguage(String languageCode, Locale locale);

		/// <summary>
		/// Returns a localized name for the given <a href="http://www.rfc-editor.org/rfc/bcp/bcp47.txt">
		/// IETF BCP47</a> script code and the given locale that is appropriate for
		/// display to the user.
		/// For example, if <code>scriptCode</code> is "Latn" and <code>locale</code>
		/// is en_US, getDisplayScript() will return "Latin"; if <code>scriptCode</code>
		/// is "Cyrl" and <code>locale</code> is fr_FR, getDisplayScript() will return "cyrillique".
		/// If the name returned cannot be localized according to <code>locale</code>,
		/// (say, the provider does not have a Japanese name for Cyrillic),
		/// this method returns null. The default implementation returns null. </summary>
		/// <param name="scriptCode"> the four letter script code string in the form of title-case
		///     letters (the first letter is upper-case character between 'A' (U+0041) and
		///     'Z' (U+005A) followed by three lower-case character between 'a' (U+0061)
		///     and 'z' (U+007A)). </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the name of the given script code for the specified locale, or null if it's not
		///     available. </returns>
		/// <exception cref="NullPointerException"> if <code>scriptCode</code> or <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>scriptCode</code> is not in the form of
		///     four title case letters, or <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <seealso cref= java.util.Locale#getDisplayScript(java.util.Locale)
		/// @since 1.7 </seealso>
		public virtual String GetDisplayScript(String scriptCode, Locale locale)
		{
			return null;
		}

		/// <summary>
		/// Returns a localized name for the given <a href="http://www.rfc-editor.org/rfc/bcp/bcp47.txt">
		/// IETF BCP47</a> region code (either ISO 3166 country code or UN M.49 area
		/// codes) and the given locale that is appropriate for display to the user.
		/// For example, if <code>countryCode</code> is "FR" and <code>locale</code>
		/// is en_US, getDisplayCountry() will return "France"; if <code>countryCode</code>
		/// is "US" and <code>locale</code> is fr_FR, getDisplayCountry() will return "Etats-Unis".
		/// If the name returned cannot be localized according to <code>locale</code>,
		/// (say, the provider does not have a Japanese name for Croatia),
		/// this method returns null. </summary>
		/// <param name="countryCode"> the country(region) code string in the form of two
		///     upper-case letters between 'A' (U+0041) and 'Z' (U+005A) or the UN M.49 area code
		///     in the form of three digit letters between '0' (U+0030) and '9' (U+0039). </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the name of the given country code for the specified locale, or null if it's not
		///     available. </returns>
		/// <exception cref="NullPointerException"> if <code>countryCode</code> or <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>countryCode</code> is not in the form of
		///     two upper-case letters or three digit letters, or <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <seealso cref= java.util.Locale#getDisplayCountry(java.util.Locale) </seealso>
		public abstract String GetDisplayCountry(String countryCode, Locale locale);

		/// <summary>
		/// Returns a localized name for the given variant code and the given locale that
		/// is appropriate for display to the user.
		/// If the name returned cannot be localized according to <code>locale</code>,
		/// this method returns null. </summary>
		/// <param name="variant"> the variant string </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the name of the given variant string for the specified locale, or null if it's not
		///     available. </returns>
		/// <exception cref="NullPointerException"> if <code>variant</code> or <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <seealso cref= java.util.Locale#getDisplayVariant(java.util.Locale) </seealso>
		public abstract String GetDisplayVariant(String variant, Locale locale);
	}

}