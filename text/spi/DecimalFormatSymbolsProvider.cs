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

namespace java.text.spi
{


	/// <summary>
	/// An abstract class for service providers that
	/// provide instances of the
	/// <seealso cref="java.text.DecimalFormatSymbols DecimalFormatSymbols"/> class.
	/// 
	/// <para>The requested {@code Locale} may contain an <a
	/// href="../../util/Locale.html#def_locale_extension"> extension</a> for
	/// specifying the desired numbering system. For example, {@code "ar-u-nu-arab"}
	/// (in the BCP 47 language tag form) specifies Arabic with the Arabic-Indic
	/// digits and symbols, while {@code "ar-u-nu-latn"} specifies Arabic with the
	/// Latin digits and symbols. Refer to the <em>Unicode Locale Data Markup
	/// Language (LDML)</em> specification for numbering systems.
	/// 
	/// @since        1.6
	/// </para>
	/// </summary>
	/// <seealso cref= Locale#forLanguageTag(String) </seealso>
	/// <seealso cref= Locale#getExtension(char) </seealso>
	public abstract class DecimalFormatSymbolsProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal DecimalFormatSymbolsProvider()
		{
		}

		/// <summary>
		/// Returns a new <code>DecimalFormatSymbols</code> instance for the
		/// specified locale.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a <code>DecimalFormatSymbols</code> instance. </returns>
		/// <seealso cref= java.text.DecimalFormatSymbols#getInstance(java.util.Locale) </seealso>
		public abstract DecimalFormatSymbols GetInstance(Locale locale);
	}

}