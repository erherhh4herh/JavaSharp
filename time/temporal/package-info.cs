﻿/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
 *
 *
 *
 *
 *
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 *  * Neither the name of JSR-310 nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/// <summary>
/// <para>
/// Access to date and time using fields and units, and date time adjusters.
/// </para>
/// <para>
/// This package expands on the base package to provide additional functionality for
/// more powerful use cases. Support is included for:
/// </para>
/// <ul>
/// <li>Units of date-time, such as years, months, days and hours</li>
/// <li>Fields of date-time, such as month-of-year, day-of-week or hour-of-day</li>
/// <li>Date-time adjustment functions</li>
/// <li>Different definitions of weeks</li>
/// </ul>
/// 
/// <h3>Fields and Units</h3>
/// <para>
/// Dates and times are expressed in terms of fields and units.
/// A unit is used to measure an amount of time, such as years, days or minutes.
/// All units implement <seealso cref="java.time.temporal.TemporalUnit"/>.
/// The set of well known units is defined in <seealso cref="java.time.temporal.ChronoUnit"/>, such as {@code DAYS}.
/// The unit interface is designed to allow application defined units.
/// </para>
/// <para>
/// A field is used to express part of a larger date-time, such as year, month-of-year or second-of-minute.
/// All fields implement <seealso cref="java.time.temporal.TemporalField"/>.
/// The set of well known fields are defined in <seealso cref="java.time.temporal.ChronoField"/>, such as {@code HOUR_OF_DAY}.
/// Additional fields are defined by <seealso cref="java.time.temporal.JulianFields"/>, <seealso cref="java.time.temporal.WeekFields"/>
/// and <seealso cref="java.time.temporal.IsoFields"/>.
/// The field interface is designed to allow application defined fields.
/// </para>
/// <para>
/// This package provides tools that allow the units and fields of date and time to be accessed
/// in a general way most suited for frameworks.
/// <seealso cref="java.time.temporal.Temporal"/> provides the abstraction for date time types that support fields.
/// Its methods support getting the value of a field, creating a new date time with the value of
/// a field modified, and querying for additional information, typically used to extract the offset or time-zone.
/// </para>
/// <para>
/// One use of fields in application code is to retrieve fields for which there is no convenience method.
/// For example, getting the day-of-month is common enough that there is a method on {@code LocalDate}
/// called {@code getDayOfMonth()}. However for more unusual fields it is necessary to use the field.
/// For example, {@code date.get(ChronoField.ALIGNED_WEEK_OF_MONTH)}.
/// The fields also provide access to the range of valid values.
/// </para>
/// 
/// <h3>Adjustment and Query</h3>
/// <para>
/// A key part of the date-time problem space is adjusting a date to a new, related value,
/// such as the "last day of the month", or "next Wednesday".
/// These are modeled as functions that adjust a base date-time.
/// The functions implement <seealso cref="java.time.temporal.TemporalAdjuster"/> and operate on {@code Temporal}.
/// A set of common functions are provided in <seealso cref="java.time.temporal.TemporalAdjusters"/>.
/// For example, to find the first occurrence of a day-of-week after a given date, use
/// <seealso cref="java.time.temporal.TemporalAdjusters#next(DayOfWeek)"/>, such as
/// {@code date.with(next(MONDAY))}.
/// Applications can also define adjusters by implementing <seealso cref="java.time.temporal.TemporalAdjuster"/>.
/// </para>
/// <para>
/// The <seealso cref="java.time.temporal.TemporalAmount"/> interface models amounts of relative time.
/// </para>
/// <para>
/// In addition to adjusting a date-time, an interface is provided to enable querying via
/// <seealso cref="java.time.temporal.TemporalQuery"/>.
/// The most common implementations of the query interface are method references.
/// The {@code from(TemporalAccessor)} methods on major classes can all be used, such as
/// {@code LocalDate::from} or {@code Month::from}.
/// Further implementations are provided in <seealso cref="java.time.temporal.TemporalQueries"/> as static methods.
/// Applications can also define queries by implementing <seealso cref="java.time.temporal.TemporalQuery"/>.
/// </para>
/// 
/// <h3>Weeks</h3>
/// <para>
/// Different locales have different definitions of the week.
/// For example, in Europe the week typically starts on a Monday, while in the US it starts on a Sunday.
/// The <seealso cref="java.time.temporal.WeekFields"/> class models this distinction.
/// </para>
/// <para>
/// The ISO calendar system defines an additional week-based division of years.
/// This defines a year based on whole Monday to Monday weeks.
/// This is modeled in <seealso cref="java.time.temporal.IsoFields"/>.
/// </para>
/// 
/// <h3>Package specification</h3>
/// <para>
/// Unless otherwise noted, passing a null argument to a constructor or method in any class or interface
/// in this package will cause a <seealso cref="java.lang.NullPointerException NullPointerException"/> to be thrown.
/// The Javadoc "@param" definition is used to summarise the null-behavior.
/// The "@throws {@link java.lang.NullPointerException}" is not explicitly documented in each method.
/// </para>
/// <para>
/// All calculations should check for numeric overflow and throw either an <seealso cref="java.lang.ArithmeticException"/>
/// or a <seealso cref="java.time.DateTimeException"/>.
/// </para>
/// @since JDK1.8
/// </summary>
namespace java.time.temporal
{

}