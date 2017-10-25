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
	/// <seealso cref="java.text.DateFormat DateFormat"/> class.
	/// 
	/// @since        1.6
	/// </summary>
	public abstract class DateFormatProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal DateFormatProvider()
		{
		}

		/// <summary>
		/// Returns a new <code>DateFormat</code> instance which formats time
		/// with the given formatting style for the specified locale. </summary>
		/// <param name="style"> the given formatting style.  Either one of
		///     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		///     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		///     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		///     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		/// <param name="locale"> the desired locale. </param>
		/// <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		///     or if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <returns> a time formatter. </returns>
		/// <seealso cref= java.text.DateFormat#getTimeInstance(int, java.util.Locale) </seealso>
		public abstract DateFormat GetTimeInstance(int style, Locale locale);

		/// <summary>
		/// Returns a new <code>DateFormat</code> instance which formats date
		/// with the given formatting style for the specified locale. </summary>
		/// <param name="style"> the given formatting style.  Either one of
		///     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		///     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		///     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		///     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		/// <param name="locale"> the desired locale. </param>
		/// <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		///     or if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <returns> a date formatter. </returns>
		/// <seealso cref= java.text.DateFormat#getDateInstance(int, java.util.Locale) </seealso>
		public abstract DateFormat GetDateInstance(int style, Locale locale);

		/// <summary>
		/// Returns a new <code>DateFormat</code> instance which formats date and time
		/// with the given formatting style for the specified locale. </summary>
		/// <param name="dateStyle"> the given date formatting style.  Either one of
		///     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		///     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		///     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		///     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		/// <param name="timeStyle"> the given time formatting style.  Either one of
		///     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		///     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		///     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		///     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		/// <param name="locale"> the desired locale. </param>
		/// <exception cref="IllegalArgumentException"> if <code>dateStyle</code> or
		///     <code>timeStyle</code> is invalid,
		///     or if <code>locale</code> isn't
		///     one of the locales returned from
		///     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		///     getAvailableLocales()}. </exception>
		/// <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		/// <returns> a date/time formatter. </returns>
		/// <seealso cref= java.text.DateFormat#getDateTimeInstance(int, int, java.util.Locale) </seealso>
		public abstract DateFormat GetDateTimeInstance(int dateStyle, int timeStyle, Locale locale);
	}

}