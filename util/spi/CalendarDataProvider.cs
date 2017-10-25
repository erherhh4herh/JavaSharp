/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// An abstract class for service providers that provide locale-dependent {@link
	/// Calendar} parameters.
	/// 
	/// @author Masayoshi Okutsu
	/// @since 1.8 </summary>
	/// <seealso cref= CalendarNameProvider </seealso>
	public abstract class CalendarDataProvider : LocaleServiceProvider
	{

		/// <summary>
		/// Sole constructor. (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal CalendarDataProvider()
		{
		}

		/// <summary>
		/// Returns the first day of a week in the given {@code locale}. This
		/// information is required by <seealso cref="Calendar"/> to support operations on the
		/// week-related calendar fields.
		/// </summary>
		/// <param name="locale">
		///        the desired locale </param>
		/// <returns> the first day of a week; one of <seealso cref="Calendar#SUNDAY"/> ..
		///         <seealso cref="Calendar#SATURDAY"/>,
		///         or 0 if the value isn't available for the {@code locale} </returns>
		/// <exception cref="NullPointerException">
		///         if {@code locale} is {@code null}. </exception>
		/// <seealso cref= java.util.Calendar#getFirstDayOfWeek() </seealso>
		/// <seealso cref= <a href="../Calendar.html#first_week">First Week</a> </seealso>
		public abstract int GetFirstDayOfWeek(Locale locale);

		/// <summary>
		/// Returns the minimal number of days required in the first week of a
		/// year. This information is required by <seealso cref="Calendar"/> to determine the
		/// first week of a year. Refer to the description of <a
		/// href="../Calendar.html#first_week"> how {@code Calendar} determines
		/// the first week</a>.
		/// </summary>
		/// <param name="locale">
		///        the desired locale </param>
		/// <returns> the minimal number of days of the first week,
		///         or 0 if the value isn't available for the {@code locale} </returns>
		/// <exception cref="NullPointerException">
		///         if {@code locale} is {@code null}. </exception>
		/// <seealso cref= java.util.Calendar#getMinimalDaysInFirstWeek() </seealso>
		public abstract int GetMinimalDaysInFirstWeek(Locale locale);
	}

}