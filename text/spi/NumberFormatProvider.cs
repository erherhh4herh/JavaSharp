/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// provide concrete implementations of the
	/// <seealso cref="java.text.NumberFormat NumberFormat"/> class.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class NumberFormatProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal NumberFormatProvider()
		{
		}

		/// <summary>
		/// Returns a new <code>NumberFormat</code> instance which formats
		/// monetary values for the specified locale.
		/// </summary>
		/// <param name="locale"> the desired locale. </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a currency formatter </returns>
		/// <seealso cref= java.text.NumberFormat#getCurrencyInstance(java.util.Locale) </seealso>
		public abstract NumberFormat GetCurrencyInstance(Locale locale);

		/// <summary>
		/// Returns a new <code>NumberFormat</code> instance which formats
		/// integer values for the specified locale.
		/// The returned number format is configured to
		/// round floating point numbers to the nearest integer using
		/// half-even rounding (see <seealso cref="java.math.RoundingMode#HALF_EVEN HALF_EVEN"/>)
		/// for formatting, and to parse only the integer part of
		/// an input string (see {@link
		/// java.text.NumberFormat#isParseIntegerOnly isParseIntegerOnly}).
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a number format for integer values </returns>
		/// <seealso cref= java.text.NumberFormat#getIntegerInstance(java.util.Locale) </seealso>
		public abstract NumberFormat GetIntegerInstance(Locale locale);

		/// <summary>
		/// Returns a new general-purpose <code>NumberFormat</code> instance for
		/// the specified locale.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a general-purpose number formatter </returns>
		/// <seealso cref= java.text.NumberFormat#getNumberInstance(java.util.Locale) </seealso>
		public abstract NumberFormat GetNumberInstance(Locale locale);

		/// <summary>
		/// Returns a new <code>NumberFormat</code> instance which formats
		/// percentage values for the specified locale.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a percent formatter </returns>
		/// <seealso cref= java.text.NumberFormat#getPercentInstance(java.util.Locale) </seealso>
		public abstract NumberFormat GetPercentInstance(Locale locale);
	}

}