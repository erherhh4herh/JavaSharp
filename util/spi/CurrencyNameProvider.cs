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
	/// provide localized currency symbols and display names for the
	/// <seealso cref="java.util.Currency Currency"/> class.
	/// Note that currency symbols are considered names when determining
	/// behaviors described in the
	/// <seealso cref="java.util.spi.LocaleServiceProvider LocaleServiceProvider"/>
	/// specification.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class CurrencyNameProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal CurrencyNameProvider()
		{
		}

		/// <summary>
		/// Gets the symbol of the given currency code for the specified locale.
		/// For example, for "USD" (US Dollar), the symbol is "$" if the specified
		/// locale is the US, while for other locales it may be "US$". If no
		/// symbol can be determined, null should be returned.
		/// </summary>
		/// <param name="currencyCode"> the ISO 4217 currency code, which
		///     consists of three upper-case letters between 'A' (U+0041) and
		///     'Z' (U+005A) </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the symbol of the given currency code for the specified locale, or null if
		///     the symbol is not available for the locale </returns>
		/// <exception cref="NullPointerException"> if <code>currencyCode</code> or
		///     <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not in
		///     the form of three upper-case letters, or <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <seealso cref= java.util.Currency#getSymbol(java.util.Locale) </seealso>
		public abstract String GetSymbol(String currencyCode, Locale locale);

		/// <summary>
		/// Returns a name for the currency that is appropriate for display to the
		/// user.  The default implementation returns null.
		/// </summary>
		/// <param name="currencyCode"> the ISO 4217 currency code, which
		///     consists of three upper-case letters between 'A' (U+0041) and
		///     'Z' (U+005A) </param>
		/// <param name="locale"> the desired locale </param>
		/// <returns> the name for the currency that is appropriate for display to the
		///     user, or null if the name is not available for the locale </returns>
		/// <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not in
		///     the form of three upper-case letters, or <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>currencyCode</code> or
		///     <code>locale</code> is <code>null</code>
		/// @since 1.7 </exception>
		public virtual String GetDisplayName(String currencyCode, Locale locale)
		{
			if (currencyCode == null || locale == null)
			{
				throw new NullPointerException();
			}

			// Check whether the currencyCode is valid
			char[] charray = currencyCode.ToCharArray();
			if (charray.Length != 3)
			{
				throw new IllegalArgumentException("The currencyCode is not in the form of three upper-case letters.");
			}
			foreach (char c in charray)
			{
				if (c < 'A' || c > 'Z')
				{
					throw new IllegalArgumentException("The currencyCode is not in the form of three upper-case letters.");
				}
			}

			// Check whether the locale is valid
			Control c = Control.GetNoFallbackControl(Control.FORMAT_DEFAULT);
			foreach (Locale l in AvailableLocales)
			{
				if (c.GetCandidateLocales("", l).Contains(locale))
				{
					return null;
				}
			}

			throw new IllegalArgumentException("The locale is not available");
		}
	}

}