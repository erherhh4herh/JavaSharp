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
	/// provide instances of the
	/// <seealso cref="java.text.DateFormatSymbols DateFormatSymbols"/> class.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class DateFormatSymbolsProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal DateFormatSymbolsProvider()
		{
		}

		/// <summary>
		/// Returns a new <code>DateFormatSymbols</code> instance for the
		/// specified locale.
		/// </summary>
		/// <param name="locale"> the desired locale </param>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <returns> a <code>DateFormatSymbols</code> instance. </returns>
		/// <seealso cref= java.text.DateFormatSymbols#getInstance(java.util.Locale) </seealso>
		public abstract DateFormatSymbols GetInstance(Locale locale);
	}

}