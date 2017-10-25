using System;
using System.Collections.Generic;
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
 * (C) Copyright Taligent, Inc. 1996-1998 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
 *
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.util
{

	using BuddhistCalendar = sun.util.BuddhistCalendar;
	using ZoneInfo = sun.util.calendar.ZoneInfo;
	using CalendarDataUtility = sun.util.locale.provider.CalendarDataUtility;
	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using CalendarProvider = sun.util.spi.CalendarProvider;

	/// <summary>
	/// The <code>Calendar</code> class is an abstract class that provides methods
	/// for converting between a specific instant in time and a set of {@link
	/// #fields calendar fields} such as <code>YEAR</code>, <code>MONTH</code>,
	/// <code>DAY_OF_MONTH</code>, <code>HOUR</code>, and so on, and for
	/// manipulating the calendar fields, such as getting the date of the next
	/// week. An instant in time can be represented by a millisecond value that is
	/// an offset from the <a name="Epoch"><em>Epoch</em></a>, January 1, 1970
	/// 00:00:00.000 GMT (Gregorian).
	/// 
	/// <para>The class also provides additional fields and methods for
	/// implementing a concrete calendar system outside the package. Those
	/// fields and methods are defined as <code>protected</code>.
	/// 
	/// </para>
	/// <para>
	/// Like other locale-sensitive classes, <code>Calendar</code> provides a
	/// class method, <code>getInstance</code>, for getting a generally useful
	/// object of this type. <code>Calendar</code>'s <code>getInstance</code> method
	/// returns a <code>Calendar</code> object whose
	/// calendar fields have been initialized with the current date and time:
	/// <blockquote>
	/// <pre>
	///     Calendar rightNow = Calendar.getInstance();
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>A <code>Calendar</code> object can produce all the calendar field values
	/// needed to implement the date-time formatting for a particular language and
	/// calendar style (for example, Japanese-Gregorian, Japanese-Traditional).
	/// <code>Calendar</code> defines the range of values returned by
	/// certain calendar fields, as well as their meaning.  For example,
	/// the first month of the calendar system has value <code>MONTH ==
	/// JANUARY</code> for all calendars.  Other values are defined by the
	/// concrete subclass, such as <code>ERA</code>.  See individual field
	/// documentation and subclass documentation for details.
	/// 
	/// <h3>Getting and Setting Calendar Field Values</h3>
	/// 
	/// </para>
	/// <para>The calendar field values can be set by calling the <code>set</code>
	/// methods. Any field values set in a <code>Calendar</code> will not be
	/// interpreted until it needs to calculate its time value (milliseconds from
	/// the Epoch) or values of the calendar fields. Calling the
	/// <code>get</code>, <code>getTimeInMillis</code>, <code>getTime</code>,
	/// <code>add</code> and <code>roll</code> involves such calculation.
	/// 
	/// <h4>Leniency</h4>
	/// 
	/// </para>
	/// <para><code>Calendar</code> has two modes for interpreting the calendar
	/// fields, <em>lenient</em> and <em>non-lenient</em>.  When a
	/// <code>Calendar</code> is in lenient mode, it accepts a wider range of
	/// calendar field values than it produces.  When a <code>Calendar</code>
	/// recomputes calendar field values for return by <code>get()</code>, all of
	/// the calendar fields are normalized. For example, a lenient
	/// <code>GregorianCalendar</code> interprets <code>MONTH == JANUARY</code>,
	/// <code>DAY_OF_MONTH == 32</code> as February 1.
	/// 
	/// </para>
	/// <para>When a <code>Calendar</code> is in non-lenient mode, it throws an
	/// exception if there is any inconsistency in its calendar fields. For
	/// example, a <code>GregorianCalendar</code> always produces
	/// <code>DAY_OF_MONTH</code> values between 1 and the length of the month. A
	/// non-lenient <code>GregorianCalendar</code> throws an exception upon
	/// calculating its time or calendar field values if any out-of-range field
	/// value has been set.
	/// 
	/// <h4><a name="first_week">First Week</a></h4>
	/// 
	/// <code>Calendar</code> defines a locale-specific seven day week using two
	/// parameters: the first day of the week and the minimal days in first week
	/// (from 1 to 7).  These numbers are taken from the locale resource data when a
	/// <code>Calendar</code> is constructed.  They may also be specified explicitly
	/// through the methods for setting their values.
	/// 
	/// </para>
	/// <para>When setting or getting the <code>WEEK_OF_MONTH</code> or
	/// <code>WEEK_OF_YEAR</code> fields, <code>Calendar</code> must determine the
	/// first week of the month or year as a reference point.  The first week of a
	/// month or year is defined as the earliest seven day period beginning on
	/// <code>getFirstDayOfWeek()</code> and containing at least
	/// <code>getMinimalDaysInFirstWeek()</code> days of that month or year.  Weeks
	/// numbered ..., -1, 0 precede the first week; weeks numbered 2, 3,... follow
	/// it.  Note that the normalized numbering returned by <code>get()</code> may be
	/// different.  For example, a specific <code>Calendar</code> subclass may
	/// designate the week before week 1 of a year as week <code><i>n</i></code> of
	/// the previous year.
	/// 
	/// <h4>Calendar Fields Resolution</h4>
	/// 
	/// When computing a date and time from the calendar fields, there
	/// may be insufficient information for the computation (such as only
	/// year and month with no day of month), or there may be inconsistent
	/// information (such as Tuesday, July 15, 1996 (Gregorian) -- July 15,
	/// 1996 is actually a Monday). <code>Calendar</code> will resolve
	/// calendar field values to determine the date and time in the
	/// following way.
	/// 
	/// </para>
	/// <para><a name="resolution">If there is any conflict in calendar field values,
	/// <code>Calendar</code> gives priorities to calendar fields that have been set
	/// more recently.</a> The following are the default combinations of the
	/// calendar fields. The most recent combination, as determined by the
	/// most recently set single field, will be used.
	/// 
	/// </para>
	/// <para><a name="date_resolution">For the date fields</a>:
	/// <blockquote>
	/// <pre>
	/// YEAR + MONTH + DAY_OF_MONTH
	/// YEAR + MONTH + WEEK_OF_MONTH + DAY_OF_WEEK
	/// YEAR + MONTH + DAY_OF_WEEK_IN_MONTH + DAY_OF_WEEK
	/// YEAR + DAY_OF_YEAR
	/// YEAR + DAY_OF_WEEK + WEEK_OF_YEAR
	/// </pre></blockquote>
	/// 
	/// <a name="time_resolution">For the time of day fields</a>:
	/// <blockquote>
	/// <pre>
	/// HOUR_OF_DAY
	/// AM_PM + HOUR
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>If there are any calendar fields whose values haven't been set in the selected
	/// field combination, <code>Calendar</code> uses their default values. The default
	/// value of each field may vary by concrete calendar systems. For example, in
	/// <code>GregorianCalendar</code>, the default of a field is the same as that
	/// of the start of the Epoch: i.e., <code>YEAR = 1970</code>, <code>MONTH =
	/// JANUARY</code>, <code>DAY_OF_MONTH = 1</code>, etc.
	/// 
	/// </para>
	/// <para>
	/// <strong>Note:</strong> There are certain possible ambiguities in
	/// interpretation of certain singular times, which are resolved in the
	/// following ways:
	/// <ol>
	///     <li> 23:59 is the last minute of the day and 00:00 is the first
	///          minute of the next day. Thus, 23:59 on Dec 31, 1999 &lt; 00:00 on
	///          Jan 1, 2000 &lt; 00:01 on Jan 1, 2000.
	/// 
	///     <li> Although historically not precise, midnight also belongs to "am",
	///          and noon belongs to "pm", so on the same day,
	///          12:00 am (midnight) &lt; 12:01 am, and 12:00 pm (noon) &lt; 12:01 pm
	/// </ol>
	/// 
	/// </para>
	/// <para>
	/// The date or time format strings are not part of the definition of a
	/// calendar, as those must be modifiable or overridable by the user at
	/// runtime. Use <seealso cref="DateFormat"/>
	/// to format dates.
	/// 
	/// <h4>Field Manipulation</h4>
	/// 
	/// The calendar fields can be changed using three methods:
	/// <code>set()</code>, <code>add()</code>, and <code>roll()</code>.
	/// 
	/// </para>
	/// <para><strong><code>set(f, value)</code></strong> changes calendar field
	/// <code>f</code> to <code>value</code>.  In addition, it sets an
	/// internal member variable to indicate that calendar field <code>f</code> has
	/// been changed. Although calendar field <code>f</code> is changed immediately,
	/// the calendar's time value in milliseconds is not recomputed until the next call to
	/// <code>get()</code>, <code>getTime()</code>, <code>getTimeInMillis()</code>,
	/// <code>add()</code>, or <code>roll()</code> is made. Thus, multiple calls to
	/// <code>set()</code> do not trigger multiple, unnecessary
	/// computations. As a result of changing a calendar field using
	/// <code>set()</code>, other calendar fields may also change, depending on the
	/// calendar field, the calendar field value, and the calendar system. In addition,
	/// <code>get(f)</code> will not necessarily return <code>value</code> set by
	/// the call to the <code>set</code> method
	/// after the calendar fields have been recomputed. The specifics are determined by
	/// the concrete calendar class.</para>
	/// 
	/// <para><em>Example</em>: Consider a <code>GregorianCalendar</code>
	/// originally set to August 31, 1999. Calling <code>set(Calendar.MONTH,
	/// Calendar.SEPTEMBER)</code> sets the date to September 31,
	/// 1999. This is a temporary internal representation that resolves to
	/// October 1, 1999 if <code>getTime()</code>is then called. However, a
	/// call to <code>set(Calendar.DAY_OF_MONTH, 30)</code> before the call to
	/// <code>getTime()</code> sets the date to September 30, 1999, since
	/// no recomputation occurs after <code>set()</code> itself.</para>
	/// 
	/// <para><strong><code>add(f, delta)</code></strong> adds <code>delta</code>
	/// to field <code>f</code>.  This is equivalent to calling <code>set(f,
	/// get(f) + delta)</code> with two adjustments:</para>
	/// 
	/// <blockquote>
	///   <para><strong>Add rule 1</strong>. The value of field <code>f</code>
	///   after the call minus the value of field <code>f</code> before the
	///   call is <code>delta</code>, modulo any overflow that has occurred in
	///   field <code>f</code>. Overflow occurs when a field value exceeds its
	///   range and, as a result, the next larger field is incremented or
	///   decremented and the field value is adjusted back into its range.</para>
	/// 
	///   <para><strong>Add rule 2</strong>. If a smaller field is expected to be
	///   invariant, but it is impossible for it to be equal to its
	///   prior value because of changes in its minimum or maximum after field
	///   <code>f</code> is changed or other constraints, such as time zone
	///   offset changes, then its value is adjusted to be as close
	///   as possible to its expected value. A smaller field represents a
	///   smaller unit of time. <code>HOUR</code> is a smaller field than
	///   <code>DAY_OF_MONTH</code>. No adjustment is made to smaller fields
	///   that are not expected to be invariant. The calendar system
	///   determines what fields are expected to be invariant.</para>
	/// </blockquote>
	/// 
	/// <para>In addition, unlike <code>set()</code>, <code>add()</code> forces
	/// an immediate recomputation of the calendar's milliseconds and all
	/// fields.</para>
	/// 
	/// <para><em>Example</em>: Consider a <code>GregorianCalendar</code>
	/// originally set to August 31, 1999. Calling <code>add(Calendar.MONTH,
	/// 13)</code> sets the calendar to September 30, 2000. <strong>Add rule
	/// 1</strong> sets the <code>MONTH</code> field to September, since
	/// adding 13 months to August gives September of the next year. Since
	/// <code>DAY_OF_MONTH</code> cannot be 31 in September in a
	/// <code>GregorianCalendar</code>, <strong>add rule 2</strong> sets the
	/// <code>DAY_OF_MONTH</code> to 30, the closest possible value. Although
	/// it is a smaller field, <code>DAY_OF_WEEK</code> is not adjusted by
	/// rule 2, since it is expected to change when the month changes in a
	/// <code>GregorianCalendar</code>.</para>
	/// 
	/// <para><strong><code>roll(f, delta)</code></strong> adds
	/// <code>delta</code> to field <code>f</code> without changing larger
	/// fields. This is equivalent to calling <code>add(f, delta)</code> with
	/// the following adjustment:</para>
	/// 
	/// <blockquote>
	///   <para><strong>Roll rule</strong>. Larger fields are unchanged after the
	///   call. A larger field represents a larger unit of
	///   time. <code>DAY_OF_MONTH</code> is a larger field than
	///   <code>HOUR</code>.</para>
	/// </blockquote>
	/// 
	/// <para><em>Example</em>: See <seealso cref="java.util.GregorianCalendar#roll(int, int)"/>.
	/// 
	/// </para>
	/// <para><strong>Usage model</strong>. To motivate the behavior of
	/// <code>add()</code> and <code>roll()</code>, consider a user interface
	/// component with increment and decrement buttons for the month, day, and
	/// year, and an underlying <code>GregorianCalendar</code>. If the
	/// interface reads January 31, 1999 and the user presses the month
	/// increment button, what should it read? If the underlying
	/// implementation uses <code>set()</code>, it might read March 3, 1999. A
	/// better result would be February 28, 1999. Furthermore, if the user
	/// presses the month increment button again, it should read March 31,
	/// 1999, not March 28, 1999. By saving the original date and using either
	/// <code>add()</code> or <code>roll()</code>, depending on whether larger
	/// fields should be affected, the user interface can behave as most users
	/// will intuitively expect.</para>
	/// </summary>
	/// <seealso cref=          java.lang.System#currentTimeMillis() </seealso>
	/// <seealso cref=          Date </seealso>
	/// <seealso cref=          GregorianCalendar </seealso>
	/// <seealso cref=          TimeZone </seealso>
	/// <seealso cref=          java.text.DateFormat
	/// @author Mark Davis, David Goldsmith, Chen-Lieh Huang, Alan Liu
	/// @since JDK1.1 </seealso>
	[Serializable]
	public abstract class Calendar : Cloneable, Comparable<Calendar>
	{

		// Data flow in Calendar
		// ---------------------

		// The current time is represented in two ways by Calendar: as UTC
		// milliseconds from the epoch (1 January 1970 0:00 UTC), and as local
		// fields such as MONTH, HOUR, AM_PM, etc.  It is possible to compute the
		// millis from the fields, and vice versa.  The data needed to do this
		// conversion is encapsulated by a TimeZone object owned by the Calendar.
		// The data provided by the TimeZone object may also be overridden if the
		// user sets the ZONE_OFFSET and/or DST_OFFSET fields directly. The class
		// keeps track of what information was most recently set by the caller, and
		// uses that to compute any other information as needed.

		// If the user sets the fields using set(), the data flow is as follows.
		// This is implemented by the Calendar subclass's computeTime() method.
		// During this process, certain fields may be ignored.  The disambiguation
		// algorithm for resolving which fields to pay attention to is described
		// in the class documentation.

		//   local fields (YEAR, MONTH, DATE, HOUR, MINUTE, etc.)
		//           |
		//           | Using Calendar-specific algorithm
		//           V
		//   local standard millis
		//           |
		//           | Using TimeZone or user-set ZONE_OFFSET / DST_OFFSET
		//           V
		//   UTC millis (in time data member)

		// If the user sets the UTC millis using setTime() or setTimeInMillis(),
		// the data flow is as follows.  This is implemented by the Calendar
		// subclass's computeFields() method.

		//   UTC millis (in time data member)
		//           |
		//           | Using TimeZone getOffset()
		//           V
		//   local standard millis
		//           |
		//           | Using Calendar-specific algorithm
		//           V
		//   local fields (YEAR, MONTH, DATE, HOUR, MINUTE, etc.)

		// In general, a round trip from fields, through local and UTC millis, and
		// back out to fields is made when necessary.  This is implemented by the
		// complete() method.  Resolving a partial set of fields into a UTC millis
		// value allows all remaining fields to be generated from that value.  If
		// the Calendar is lenient, the fields are also renormalized to standard
		// ranges when they are regenerated.

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// era, e.g., AD or BC in the Julian calendar. This is a calendar-specific
		/// value; see subclass documentation.
		/// </summary>
		/// <seealso cref= GregorianCalendar#AD </seealso>
		/// <seealso cref= GregorianCalendar#BC </seealso>
		public const int ERA = 0;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// year. This is a calendar-specific value; see subclass documentation.
		/// </summary>
		public const int YEAR = 1;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// month. This is a calendar-specific value. The first month of
		/// the year in the Gregorian and Julian calendars is
		/// <code>JANUARY</code> which is 0; the last depends on the number
		/// of months in a year.
		/// </summary>
		/// <seealso cref= #JANUARY </seealso>
		/// <seealso cref= #FEBRUARY </seealso>
		/// <seealso cref= #MARCH </seealso>
		/// <seealso cref= #APRIL </seealso>
		/// <seealso cref= #MAY </seealso>
		/// <seealso cref= #JUNE </seealso>
		/// <seealso cref= #JULY </seealso>
		/// <seealso cref= #AUGUST </seealso>
		/// <seealso cref= #SEPTEMBER </seealso>
		/// <seealso cref= #OCTOBER </seealso>
		/// <seealso cref= #NOVEMBER </seealso>
		/// <seealso cref= #DECEMBER </seealso>
		/// <seealso cref= #UNDECIMBER </seealso>
		public const int MONTH = 2;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// week number within the current year.  The first week of the year, as
		/// defined by <code>getFirstDayOfWeek()</code> and
		/// <code>getMinimalDaysInFirstWeek()</code>, has value 1.  Subclasses define
		/// the value of <code>WEEK_OF_YEAR</code> for days before the first week of
		/// the year.
		/// </summary>
		/// <seealso cref= #getFirstDayOfWeek </seealso>
		/// <seealso cref= #getMinimalDaysInFirstWeek </seealso>
		public const int WEEK_OF_YEAR = 3;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// week number within the current month.  The first week of the month, as
		/// defined by <code>getFirstDayOfWeek()</code> and
		/// <code>getMinimalDaysInFirstWeek()</code>, has value 1.  Subclasses define
		/// the value of <code>WEEK_OF_MONTH</code> for days before the first week of
		/// the month.
		/// </summary>
		/// <seealso cref= #getFirstDayOfWeek </seealso>
		/// <seealso cref= #getMinimalDaysInFirstWeek </seealso>
		public const int WEEK_OF_MONTH = 4;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// day of the month. This is a synonym for <code>DAY_OF_MONTH</code>.
		/// The first day of the month has value 1.
		/// </summary>
		/// <seealso cref= #DAY_OF_MONTH </seealso>
		public const int DATE = 5;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// day of the month. This is a synonym for <code>DATE</code>.
		/// The first day of the month has value 1.
		/// </summary>
		/// <seealso cref= #DATE </seealso>
		public const int DAY_OF_MONTH = 5;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the day
		/// number within the current year.  The first day of the year has value 1.
		/// </summary>
		public const int DAY_OF_YEAR = 6;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the day
		/// of the week.  This field takes values <code>SUNDAY</code>,
		/// <code>MONDAY</code>, <code>TUESDAY</code>, <code>WEDNESDAY</code>,
		/// <code>THURSDAY</code>, <code>FRIDAY</code>, and <code>SATURDAY</code>.
		/// </summary>
		/// <seealso cref= #SUNDAY </seealso>
		/// <seealso cref= #MONDAY </seealso>
		/// <seealso cref= #TUESDAY </seealso>
		/// <seealso cref= #WEDNESDAY </seealso>
		/// <seealso cref= #THURSDAY </seealso>
		/// <seealso cref= #FRIDAY </seealso>
		/// <seealso cref= #SATURDAY </seealso>
		public const int DAY_OF_WEEK = 7;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// ordinal number of the day of the week within the current month. Together
		/// with the <code>DAY_OF_WEEK</code> field, this uniquely specifies a day
		/// within a month.  Unlike <code>WEEK_OF_MONTH</code> and
		/// <code>WEEK_OF_YEAR</code>, this field's value does <em>not</em> depend on
		/// <code>getFirstDayOfWeek()</code> or
		/// <code>getMinimalDaysInFirstWeek()</code>.  <code>DAY_OF_MONTH 1</code>
		/// through <code>7</code> always correspond to <code>DAY_OF_WEEK_IN_MONTH
		/// 1</code>; <code>8</code> through <code>14</code> correspond to
		/// <code>DAY_OF_WEEK_IN_MONTH 2</code>, and so on.
		/// <code>DAY_OF_WEEK_IN_MONTH 0</code> indicates the week before
		/// <code>DAY_OF_WEEK_IN_MONTH 1</code>.  Negative values count back from the
		/// end of the month, so the last Sunday of a month is specified as
		/// <code>DAY_OF_WEEK = SUNDAY, DAY_OF_WEEK_IN_MONTH = -1</code>.  Because
		/// negative values count backward they will usually be aligned differently
		/// within the month than positive values.  For example, if a month has 31
		/// days, <code>DAY_OF_WEEK_IN_MONTH -1</code> will overlap
		/// <code>DAY_OF_WEEK_IN_MONTH 5</code> and the end of <code>4</code>.
		/// </summary>
		/// <seealso cref= #DAY_OF_WEEK </seealso>
		/// <seealso cref= #WEEK_OF_MONTH </seealso>
		public const int DAY_OF_WEEK_IN_MONTH = 8;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating
		/// whether the <code>HOUR</code> is before or after noon.
		/// E.g., at 10:04:15.250 PM the <code>AM_PM</code> is <code>PM</code>.
		/// </summary>
		/// <seealso cref= #AM </seealso>
		/// <seealso cref= #PM </seealso>
		/// <seealso cref= #HOUR </seealso>
		public const int AM_PM = 9;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// hour of the morning or afternoon. <code>HOUR</code> is used for the
		/// 12-hour clock (0 - 11). Noon and midnight are represented by 0, not by 12.
		/// E.g., at 10:04:15.250 PM the <code>HOUR</code> is 10.
		/// </summary>
		/// <seealso cref= #AM_PM </seealso>
		/// <seealso cref= #HOUR_OF_DAY </seealso>
		public const int HOUR = 10;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// hour of the day. <code>HOUR_OF_DAY</code> is used for the 24-hour clock.
		/// E.g., at 10:04:15.250 PM the <code>HOUR_OF_DAY</code> is 22.
		/// </summary>
		/// <seealso cref= #HOUR </seealso>
		public const int HOUR_OF_DAY = 11;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// minute within the hour.
		/// E.g., at 10:04:15.250 PM the <code>MINUTE</code> is 4.
		/// </summary>
		public const int MINUTE = 12;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// second within the minute.
		/// E.g., at 10:04:15.250 PM the <code>SECOND</code> is 15.
		/// </summary>
		public const int SECOND = 13;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// millisecond within the second.
		/// E.g., at 10:04:15.250 PM the <code>MILLISECOND</code> is 250.
		/// </summary>
		public const int MILLISECOND = 14;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code>
		/// indicating the raw offset from GMT in milliseconds.
		/// <para>
		/// This field reflects the correct GMT offset value of the time
		/// zone of this <code>Calendar</code> if the
		/// <code>TimeZone</code> implementation subclass supports
		/// historical GMT offset changes.
		/// </para>
		/// </summary>
		public const int ZONE_OFFSET = 15;

		/// <summary>
		/// Field number for <code>get</code> and <code>set</code> indicating the
		/// daylight saving offset in milliseconds.
		/// <para>
		/// This field reflects the correct daylight saving offset value of
		/// the time zone of this <code>Calendar</code> if the
		/// <code>TimeZone</code> implementation subclass supports
		/// historical Daylight Saving Time schedule changes.
		/// </para>
		/// </summary>
		public const int DST_OFFSET = 16;

		/// <summary>
		/// The number of distinct fields recognized by <code>get</code> and <code>set</code>.
		/// Field numbers range from <code>0..FIELD_COUNT-1</code>.
		/// </summary>
		public const int FIELD_COUNT = 17;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Sunday.
		/// </summary>
		public const int SUNDAY = 1;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Monday.
		/// </summary>
		public const int MONDAY = 2;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Tuesday.
		/// </summary>
		public const int TUESDAY = 3;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Wednesday.
		/// </summary>
		public const int WEDNESDAY = 4;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Thursday.
		/// </summary>
		public const int THURSDAY = 5;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Friday.
		/// </summary>
		public const int FRIDAY = 6;

		/// <summary>
		/// Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		/// Saturday.
		/// </summary>
		public const int SATURDAY = 7;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// first month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int JANUARY = 0;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// second month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int FEBRUARY = 1;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// third month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int MARCH = 2;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// fourth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int APRIL = 3;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// fifth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int MAY = 4;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// sixth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int JUNE = 5;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// seventh month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int JULY = 6;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// eighth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int AUGUST = 7;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// ninth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int SEPTEMBER = 8;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// tenth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int OCTOBER = 9;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// eleventh month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int NOVEMBER = 10;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// twelfth month of the year in the Gregorian and Julian calendars.
		/// </summary>
		public const int DECEMBER = 11;

		/// <summary>
		/// Value of the <seealso cref="#MONTH"/> field indicating the
		/// thirteenth month of the year. Although <code>GregorianCalendar</code>
		/// does not use this value, lunar calendars do.
		/// </summary>
		public const int UNDECIMBER = 12;

		/// <summary>
		/// Value of the <seealso cref="#AM_PM"/> field indicating the
		/// period of the day from midnight to just before noon.
		/// </summary>
		public const int AM = 0;

		/// <summary>
		/// Value of the <seealso cref="#AM_PM"/> field indicating the
		/// period of the day from noon to just before midnight.
		/// </summary>
		public const int PM = 1;

		/// <summary>
		/// A style specifier for {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating names in all styles, such as
		/// "January" and "Jan".
		/// </summary>
		/// <seealso cref= #SHORT_FORMAT </seealso>
		/// <seealso cref= #LONG_FORMAT </seealso>
		/// <seealso cref= #SHORT_STANDALONE </seealso>
		/// <seealso cref= #LONG_STANDALONE </seealso>
		/// <seealso cref= #SHORT </seealso>
		/// <seealso cref= #LONG
		/// @since 1.6 </seealso>
		public const int ALL_STYLES = 0;

		internal const int STANDALONE_MASK = 0x8000;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} equivalent to <seealso cref="#SHORT_FORMAT"/>.
		/// </summary>
		/// <seealso cref= #SHORT_STANDALONE </seealso>
		/// <seealso cref= #LONG
		/// @since 1.6 </seealso>
		public const int SHORT = 1;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} equivalent to <seealso cref="#LONG_FORMAT"/>.
		/// </summary>
		/// <seealso cref= #LONG_STANDALONE </seealso>
		/// <seealso cref= #SHORT
		/// @since 1.6 </seealso>
		public const int LONG = 2;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a narrow name used for format. Narrow names
		/// are typically single character strings, such as "M" for Monday.
		/// </summary>
		/// <seealso cref= #NARROW_STANDALONE </seealso>
		/// <seealso cref= #SHORT_FORMAT </seealso>
		/// <seealso cref= #LONG_FORMAT
		/// @since 1.8 </seealso>
		public const int NARROW_FORMAT = 4;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a narrow name independently. Narrow names
		/// are typically single character strings, such as "M" for Monday.
		/// </summary>
		/// <seealso cref= #NARROW_FORMAT </seealso>
		/// <seealso cref= #SHORT_STANDALONE </seealso>
		/// <seealso cref= #LONG_STANDALONE
		/// @since 1.8 </seealso>
		public static readonly int NARROW_STANDALONE = NARROW_FORMAT | STANDALONE_MASK;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a short name used for format.
		/// </summary>
		/// <seealso cref= #SHORT_STANDALONE </seealso>
		/// <seealso cref= #LONG_FORMAT </seealso>
		/// <seealso cref= #LONG_STANDALONE
		/// @since 1.8 </seealso>
		public const int SHORT_FORMAT = 1;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a long name used for format.
		/// </summary>
		/// <seealso cref= #LONG_STANDALONE </seealso>
		/// <seealso cref= #SHORT_FORMAT </seealso>
		/// <seealso cref= #SHORT_STANDALONE
		/// @since 1.8 </seealso>
		public const int LONG_FORMAT = 2;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a short name used independently,
		/// such as a month abbreviation as calendar headers.
		/// </summary>
		/// <seealso cref= #SHORT_FORMAT </seealso>
		/// <seealso cref= #LONG_FORMAT </seealso>
		/// <seealso cref= #LONG_STANDALONE
		/// @since 1.8 </seealso>
		public static readonly int SHORT_STANDALONE = SHORT | STANDALONE_MASK;

		/// <summary>
		/// A style specifier for {@link #getDisplayName(int, int, Locale)
		/// getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		/// getDisplayNames} indicating a long name used independently,
		/// such as a month name as calendar headers.
		/// </summary>
		/// <seealso cref= #LONG_FORMAT </seealso>
		/// <seealso cref= #SHORT_FORMAT </seealso>
		/// <seealso cref= #SHORT_STANDALONE
		/// @since 1.8 </seealso>
		public static readonly int LONG_STANDALONE = LONG | STANDALONE_MASK;

		// Internal notes:
		// Calendar contains two kinds of time representations: current "time" in
		// milliseconds, and a set of calendar "fields" representing the current time.
		// The two representations are usually in sync, but can get out of sync
		// as follows.
		// 1. Initially, no fields are set, and the time is invalid.
		// 2. If the time is set, all fields are computed and in sync.
		// 3. If a single field is set, the time is invalid.
		// Recomputation of the time and fields happens when the object needs
		// to return a result to the user, or use a result for a computation.

		/// <summary>
		/// The calendar field values for the currently set time for this calendar.
		/// This is an array of <code>FIELD_COUNT</code> integers, with index values
		/// <code>ERA</code> through <code>DST_OFFSET</code>.
		/// @serial
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("ProtectedField") protected int fields[];
		protected internal int[] Fields;

		/// <summary>
		/// The flags which tell if a specified calendar field for the calendar is set.
		/// A new object has no fields set.  After the first call to a method
		/// which generates the fields, they all remain set after that.
		/// This is an array of <code>FIELD_COUNT</code> booleans, with index values
		/// <code>ERA</code> through <code>DST_OFFSET</code>.
		/// @serial
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("ProtectedField") protected boolean isSet[];
		protected internal bool[] IsSet_Renamed;

		/// <summary>
		/// Pseudo-time-stamps which specify when each field was set. There
		/// are two special values, UNSET and COMPUTED. Values from
		/// MINIMUM_USER_SET to Integer.MAX_VALUE are legal user set values.
		/// </summary>
		[NonSerialized]
		private int[] Stamp;

		/// <summary>
		/// The currently set time for this calendar, expressed in milliseconds after
		/// January 1, 1970, 0:00:00 GMT. </summary>
		/// <seealso cref= #isTimeSet
		/// @serial </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("ProtectedField") protected long time;
		protected internal long Time_Renamed;

		/// <summary>
		/// True if then the value of <code>time</code> is valid.
		/// The time is made invalid by a change to an item of <code>field[]</code>. </summary>
		/// <seealso cref= #time
		/// @serial </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("ProtectedField") protected boolean isTimeSet;
		protected internal bool IsTimeSet;

		/// <summary>
		/// True if <code>fields[]</code> are in sync with the currently set time.
		/// If false, then the next attempt to get the value of a field will
		/// force a recomputation of all fields from the current value of
		/// <code>time</code>.
		/// @serial
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("ProtectedField") protected boolean areFieldsSet;
		protected internal bool AreFieldsSet;

		/// <summary>
		/// True if all fields have been set.
		/// @serial
		/// </summary>
		[NonSerialized]
		internal bool AreAllFieldsSet;

		/// <summary>
		/// <code>True</code> if this calendar allows out-of-range field values during computation
		/// of <code>time</code> from <code>fields[]</code>. </summary>
		/// <seealso cref= #setLenient </seealso>
		/// <seealso cref= #isLenient
		/// @serial </seealso>
		private bool Lenient_Renamed = true;

		/// <summary>
		/// The <code>TimeZone</code> used by this calendar. <code>Calendar</code>
		/// uses the time zone data to translate between locale and GMT time.
		/// @serial
		/// </summary>
		private TimeZone Zone_Renamed;

		/// <summary>
		/// <code>True</code> if zone references to a shared TimeZone object.
		/// </summary>
		[NonSerialized]
		private bool SharedZone = false;

		/// <summary>
		/// The first day of the week, with possible values <code>SUNDAY</code>,
		/// <code>MONDAY</code>, etc.  This is a locale-dependent value.
		/// @serial
		/// </summary>
		private int FirstDayOfWeek_Renamed;

		/// <summary>
		/// The number of days required for the first week in a month or year,
		/// with possible values from 1 to 7.  This is a locale-dependent value.
		/// @serial
		/// </summary>
		private int MinimalDaysInFirstWeek_Renamed;

		/// <summary>
		/// Cache to hold the firstDayOfWeek and minimalDaysInFirstWeek
		/// of a Locale.
		/// </summary>
		private static readonly ConcurrentMap<Locale, int[]> CachedLocaleData = new ConcurrentDictionary<Locale, int[]>(3);

		// Special values of stamp[]
		/// <summary>
		/// The corresponding fields[] has no value.
		/// </summary>
		private const int UNSET = 0;

		/// <summary>
		/// The value of the corresponding fields[] has been calculated internally.
		/// </summary>
		private const int COMPUTED = 1;

		/// <summary>
		/// The value of the corresponding fields[] has been set externally. Stamp
		/// values which are greater than 1 represents the (pseudo) time when the
		/// corresponding fields[] value was set.
		/// </summary>
		private const int MINIMUM_USER_STAMP = 2;

		/// <summary>
		/// The mask value that represents all of the fields.
		/// </summary>
		internal static readonly int ALL_FIELDS = (1 << FIELD_COUNT) - 1;

		/// <summary>
		/// The next available value for <code>stamp[]</code>, an internal array.
		/// This actually should not be written out to the stream, and will probably
		/// be removed from the stream in the near future.  In the meantime,
		/// a value of <code>MINIMUM_USER_STAMP</code> should be used.
		/// @serial
		/// </summary>
		private int NextStamp = MINIMUM_USER_STAMP;

		// the internal serial version which says which version was written
		// - 0 (default) for version up to JDK 1.1.5
		// - 1 for version from JDK 1.1.6, which writes a correct 'time' value
		//     as well as compatible values for other fields.  This is a
		//     transitional format.
		// - 2 (not implemented yet) a future version, in which fields[],
		//     areFieldsSet, and isTimeSet become transient, and isSet[] is
		//     removed. In JDK 1.1.6 we write a format compatible with version 2.
		internal const int CurrentSerialVersion = 1;

		/// <summary>
		/// The version of the serialized data on the stream.  Possible values:
		/// <dl>
		/// <dt><b>0</b> or not present on stream</dt>
		/// <dd>
		/// JDK 1.1.5 or earlier.
		/// </dd>
		/// <dt><b>1</b></dt>
		/// <dd>
		/// JDK 1.1.6 or later.  Writes a correct 'time' value
		/// as well as compatible values for other fields.  This is a
		/// transitional format.
		/// </dd>
		/// </dl>
		/// When streaming out this class, the most recent format
		/// and the highest allowable <code>serialVersionOnStream</code>
		/// is written.
		/// @serial
		/// @since JDK1.1.6
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		// Proclaim serialization compatibility with JDK 1.1
		internal const long SerialVersionUID = -1807547505821590642L;

		// Mask values for calendar fields
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("PointlessBitwiseExpression") final static int ERA_MASK = (1 << ERA);
		internal static readonly int ERA_MASK = (1 << ERA);
		internal static readonly int YEAR_MASK = (1 << YEAR);
		internal static readonly int MONTH_MASK = (1 << MONTH);
		internal static readonly int WEEK_OF_YEAR_MASK = (1 << WEEK_OF_YEAR);
		internal static readonly int WEEK_OF_MONTH_MASK = (1 << WEEK_OF_MONTH);
		internal static readonly int DAY_OF_MONTH_MASK = (1 << DAY_OF_MONTH);
		internal static readonly int DATE_MASK = DAY_OF_MONTH_MASK;
		internal static readonly int DAY_OF_YEAR_MASK = (1 << DAY_OF_YEAR);
		internal static readonly int DAY_OF_WEEK_MASK = (1 << DAY_OF_WEEK);
		internal static readonly int DAY_OF_WEEK_IN_MONTH_MASK = (1 << DAY_OF_WEEK_IN_MONTH);
		internal static readonly int AM_PM_MASK = (1 << AM_PM);
		internal static readonly int HOUR_MASK = (1 << HOUR);
		internal static readonly int HOUR_OF_DAY_MASK = (1 << HOUR_OF_DAY);
		internal static readonly int MINUTE_MASK = (1 << MINUTE);
		internal static readonly int SECOND_MASK = (1 << SECOND);
		internal static readonly int MILLISECOND_MASK = (1 << MILLISECOND);
		internal static readonly int ZONE_OFFSET_MASK = (1 << ZONE_OFFSET);
		internal static readonly int DST_OFFSET_MASK = (1 << DST_OFFSET);

		/// <summary>
		/// {@code Calendar.Builder} is used for creating a {@code Calendar} from
		/// various date-time parameters.
		/// 
		/// <para>There are two ways to set a {@code Calendar} to a date-time value. One
		/// is to set the instant parameter to a millisecond offset from the <a
		/// href="Calendar.html#Epoch">Epoch</a>. The other is to set individual
		/// field parameters, such as <seealso cref="Calendar#YEAR YEAR"/>, to their desired
		/// values. These two ways can't be mixed. Trying to set both the instant and
		/// individual fields will cause an <seealso cref="IllegalStateException"/> to be
		/// thrown. However, it is permitted to override previous values of the
		/// instant or field parameters.
		/// 
		/// </para>
		/// <para>If no enough field parameters are given for determining date and/or
		/// time, calendar specific default values are used when building a
		/// {@code Calendar}. For example, if the <seealso cref="Calendar#YEAR YEAR"/> value
		/// isn't given for the Gregorian calendar, 1970 will be used. If there are
		/// any conflicts among field parameters, the <a
		/// href="Calendar.html#resolution"> resolution rules</a> are applied.
		/// Therefore, the order of field setting matters.
		/// 
		/// </para>
		/// <para>In addition to the date-time parameters,
		/// the <seealso cref="#setLocale(Locale) locale"/>,
		/// <seealso cref="#setTimeZone(TimeZone) time zone"/>,
		/// <seealso cref="#setWeekDefinition(int, int) week definition"/>, and
		/// <seealso cref="#setLenient(boolean) leniency mode"/> parameters can be set.
		/// 
		/// </para>
		/// <para><b>Examples</b>
		/// </para>
		/// <para>The following are sample usages. Sample code assumes that the
		/// {@code Calendar} constants are statically imported.
		/// 
		/// </para>
		/// <para>The following code produces a {@code Calendar} with date 2012-12-31
		/// (Gregorian) because Monday is the first day of a week with the <a
		/// href="GregorianCalendar.html#iso8601_compatible_setting"> ISO 8601
		/// compatible week parameters</a>.
		/// <pre>
		///   Calendar cal = new Calendar.Builder().setCalendarType("iso8601")
		///                        .setWeekDate(2013, 1, MONDAY).build();</pre>
		/// </para>
		/// <para>The following code produces a Japanese {@code Calendar} with date
		/// 1989-01-08 (Gregorian), assuming that the default <seealso cref="Calendar#ERA ERA"/>
		/// is <em>Heisei</em> that started on that day.
		/// <pre>
		///   Calendar cal = new Calendar.Builder().setCalendarType("japanese")
		///                        .setFields(YEAR, 1, DAY_OF_YEAR, 1).build();</pre>
		/// 
		/// @since 1.8
		/// </para>
		/// </summary>
		/// <seealso cref= Calendar#getInstance(TimeZone, Locale) </seealso>
		/// <seealso cref= Calendar#fields </seealso>
		public class Builder
		{
			internal static readonly int NFIELDS = FIELD_COUNT + 1; // +1 for WEEK_YEAR
			internal const int WEEK_YEAR = FIELD_COUNT;

			internal long Instant_Renamed;
			// Calendar.stamp[] (lower half) and Calendar.fields[] (upper half) combined
			internal int[] Fields_Renamed;
			// Pseudo timestamp starting from MINIMUM_USER_STAMP.
			// (COMPUTED is used to indicate that the instant has been set.)
			internal int NextStamp;
			// maxFieldIndex keeps the max index of fields which have been set.
			// (WEEK_YEAR is never included.)
			internal int MaxFieldIndex;
			internal String Type;
			internal TimeZone Zone;
			internal bool Lenient_Renamed = true;
			internal Locale Locale_Renamed;
			internal int FirstDayOfWeek, MinimalDaysInFirstWeek;

			/// <summary>
			/// Constructs a {@code Calendar.Builder}.
			/// </summary>
			public Builder()
			{
			}

			/// <summary>
			/// Sets the instant parameter to the given {@code instant} value that is
			/// a millisecond offset from <a href="Calendar.html#Epoch">the
			/// Epoch</a>.
			/// </summary>
			/// <param name="instant"> a millisecond offset from the Epoch </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="IllegalStateException"> if any of the field parameters have
			///                               already been set </exception>
			/// <seealso cref= Calendar#setTime(Date) </seealso>
			/// <seealso cref= Calendar#setTimeInMillis(long) </seealso>
			/// <seealso cref= Calendar#time </seealso>
			public virtual Builder SetInstant(long instant)
			{
				if (Fields_Renamed != null)
				{
					throw new IllegalStateException();
				}
				this.Instant_Renamed = instant;
				NextStamp = COMPUTED;
				return this;
			}

			/// <summary>
			/// Sets the instant parameter to the {@code instant} value given by a
			/// <seealso cref="Date"/>. This method is equivalent to a call to
			/// <seealso cref="#setInstant(long) setInstant(instant.getTime())"/>.
			/// </summary>
			/// <param name="instant"> a {@code Date} representing a millisecond offset from
			///                the Epoch </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="NullPointerException">  if {@code instant} is {@code null} </exception>
			/// <exception cref="IllegalStateException"> if any of the field parameters have
			///                               already been set </exception>
			/// <seealso cref= Calendar#setTime(Date) </seealso>
			/// <seealso cref= Calendar#setTimeInMillis(long) </seealso>
			/// <seealso cref= Calendar#time </seealso>
			public virtual Builder SetInstant(Date instant)
			{
				return SetInstant(instant.Time); // NPE if instant == null
			}

			/// <summary>
			/// Sets the {@code field} parameter to the given {@code value}.
			/// {@code field} is an index to the <seealso cref="Calendar#fields"/>, such as
			/// <seealso cref="Calendar#DAY_OF_MONTH DAY_OF_MONTH"/>. Field value validation is
			/// not performed in this method. Any out of range values are either
			/// normalized in lenient mode or detected as an invalid value in
			/// non-lenient mode when building a {@code Calendar}.
			/// </summary>
			/// <param name="field"> an index to the {@code Calendar} fields </param>
			/// <param name="value"> the field value </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="IllegalArgumentException"> if {@code field} is invalid </exception>
			/// <exception cref="IllegalStateException"> if the instant value has already been set,
			///                      or if fields have been set too many
			///                      (approximately <seealso cref="Integer#MAX_VALUE"/>) times. </exception>
			/// <seealso cref= Calendar#set(int, int) </seealso>
			public virtual Builder Set(int field, int value)
			{
				// Note: WEEK_YEAR can't be set with this method.
				if (field < 0 || field >= FIELD_COUNT)
				{
					throw new IllegalArgumentException("field is invalid");
				}
				if (InstantSet)
				{
					throw new IllegalStateException("instant has been set");
				}
				AllocateFields();
				InternalSet(field, value);
				return this;
			}

			/// <summary>
			/// Sets field parameters to their values given by
			/// {@code fieldValuePairs} that are pairs of a field and its value.
			/// For example,
			/// <pre>
			///   setFeilds(Calendar.YEAR, 2013,
			///             Calendar.MONTH, Calendar.DECEMBER,
			///             Calendar.DAY_OF_MONTH, 23);</pre>
			/// is equivalent to the sequence of the following
			/// <seealso cref="#set(int, int) set"/> calls:
			/// <pre>
			///   set(Calendar.YEAR, 2013)
			///   .set(Calendar.MONTH, Calendar.DECEMBER)
			///   .set(Calendar.DAY_OF_MONTH, 23);</pre>
			/// </summary>
			/// <param name="fieldValuePairs"> field-value pairs </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="NullPointerException"> if {@code fieldValuePairs} is {@code null} </exception>
			/// <exception cref="IllegalArgumentException"> if any of fields are invalid,
			///             or if {@code fieldValuePairs.length} is an odd number. </exception>
			/// <exception cref="IllegalStateException">    if the instant value has been set,
			///             or if fields have been set too many (approximately
			///             <seealso cref="Integer#MAX_VALUE"/>) times. </exception>
			public virtual Builder SetFields(params int[] fieldValuePairs)
			{
				int len = fieldValuePairs.Length;
				if ((len % 2) != 0)
				{
					throw new IllegalArgumentException();
				}
				if (InstantSet)
				{
					throw new IllegalStateException("instant has been set");
				}
				if ((NextStamp + len / 2) < 0)
				{
					throw new IllegalStateException("stamp counter overflow");
				}
				AllocateFields();
				for (int i = 0; i < len;)
				{
					int field = fieldValuePairs[i++];
					// Note: WEEK_YEAR can't be set with this method.
					if (field < 0 || field >= FIELD_COUNT)
					{
						throw new IllegalArgumentException("field is invalid");
					}
					InternalSet(field, fieldValuePairs[i++]);
				}
				return this;
			}

			/// <summary>
			/// Sets the date field parameters to the values given by {@code year},
			/// {@code month}, and {@code dayOfMonth}. This method is equivalent to
			/// a call to:
			/// <pre>
			///   setFields(Calendar.YEAR, year,
			///             Calendar.MONTH, month,
			///             Calendar.DAY_OF_MONTH, dayOfMonth);</pre>
			/// </summary>
			/// <param name="year">       the <seealso cref="Calendar#YEAR YEAR"/> value </param>
			/// <param name="month">      the <seealso cref="Calendar#MONTH MONTH"/> value
			///                   (the month numbering is <em>0-based</em>). </param>
			/// <param name="dayOfMonth"> the <seealso cref="Calendar#DAY_OF_MONTH DAY_OF_MONTH"/> value </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			public virtual Builder SetDate(int year, int month, int dayOfMonth)
			{
				return setFields(YEAR, year, MONTH, month, DAY_OF_MONTH, dayOfMonth);
			}

			/// <summary>
			/// Sets the time of day field parameters to the values given by
			/// {@code hourOfDay}, {@code minute}, and {@code second}. This method is
			/// equivalent to a call to:
			/// <pre>
			///   setTimeOfDay(hourOfDay, minute, second, 0);</pre>
			/// </summary>
			/// <param name="hourOfDay"> the <seealso cref="Calendar#HOUR_OF_DAY HOUR_OF_DAY"/> value
			///                  (24-hour clock) </param>
			/// <param name="minute">    the <seealso cref="Calendar#MINUTE MINUTE"/> value </param>
			/// <param name="second">    the <seealso cref="Calendar#SECOND SECOND"/> value </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			public virtual Builder SetTimeOfDay(int hourOfDay, int minute, int second)
			{
				return SetTimeOfDay(hourOfDay, minute, second, 0);
			}

			/// <summary>
			/// Sets the time of day field parameters to the values given by
			/// {@code hourOfDay}, {@code minute}, {@code second}, and
			/// {@code millis}. This method is equivalent to a call to:
			/// <pre>
			///   setFields(Calendar.HOUR_OF_DAY, hourOfDay,
			///             Calendar.MINUTE, minute,
			///             Calendar.SECOND, second,
			///             Calendar.MILLISECOND, millis);</pre>
			/// </summary>
			/// <param name="hourOfDay"> the <seealso cref="Calendar#HOUR_OF_DAY HOUR_OF_DAY"/> value
			///                  (24-hour clock) </param>
			/// <param name="minute">    the <seealso cref="Calendar#MINUTE MINUTE"/> value </param>
			/// <param name="second">    the <seealso cref="Calendar#SECOND SECOND"/> value </param>
			/// <param name="millis">    the <seealso cref="Calendar#MILLISECOND MILLISECOND"/> value </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			public virtual Builder SetTimeOfDay(int hourOfDay, int minute, int second, int millis)
			{
				return setFields(HOUR_OF_DAY, hourOfDay, MINUTE, minute, SECOND, second, MILLISECOND, millis);
			}

			/// <summary>
			/// Sets the week-based date parameters to the values with the given
			/// date specifiers - week year, week of year, and day of week.
			/// 
			/// <para>If the specified calendar doesn't support week dates, the
			/// <seealso cref="#build() build"/> method will throw an <seealso cref="IllegalArgumentException"/>.
			/// 
			/// </para>
			/// </summary>
			/// <param name="weekYear">   the week year </param>
			/// <param name="weekOfYear"> the week number based on {@code weekYear} </param>
			/// <param name="dayOfWeek">  the day of week value: one of the constants
			///     for the <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> field:
			///     <seealso cref="Calendar#SUNDAY SUNDAY"/>, ..., <seealso cref="Calendar#SATURDAY SATURDAY"/>. </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <seealso cref= Calendar#setWeekDate(int, int, int) </seealso>
			/// <seealso cref= Calendar#isWeekDateSupported() </seealso>
			public virtual Builder SetWeekDate(int weekYear, int weekOfYear, int dayOfWeek)
			{
				AllocateFields();
				InternalSet(WEEK_YEAR, weekYear);
				InternalSet(WEEK_OF_YEAR, weekOfYear);
				InternalSet(DAY_OF_WEEK, dayOfWeek);
				return this;
			}

			/// <summary>
			/// Sets the time zone parameter to the given {@code zone}. If no time
			/// zone parameter is given to this {@code Caledar.Builder}, the
			/// {@link TimeZone#getDefault() default
			/// <code>TimeZone</code>} will be used in the <seealso cref="#build() build"/>
			/// method.
			/// </summary>
			/// <param name="zone"> the <seealso cref="TimeZone"/> </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="NullPointerException"> if {@code zone} is {@code null} </exception>
			/// <seealso cref= Calendar#setTimeZone(TimeZone) </seealso>
			public virtual Builder SetTimeZone(TimeZone zone)
			{
				if (zone == null)
				{
					throw new NullPointerException();
				}
				this.Zone = zone;
				return this;
			}

			/// <summary>
			/// Sets the lenient mode parameter to the value given by {@code lenient}.
			/// If no lenient parameter is given to this {@code Calendar.Builder},
			/// lenient mode will be used in the <seealso cref="#build() build"/> method.
			/// </summary>
			/// <param name="lenient"> {@code true} for lenient mode;
			///                {@code false} for non-lenient mode </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <seealso cref= Calendar#setLenient(boolean) </seealso>
			public virtual Builder SetLenient(bool lenient)
			{
				this.Lenient_Renamed = lenient;
				return this;
			}

			/// <summary>
			/// Sets the calendar type parameter to the given {@code type}. The
			/// calendar type given by this method has precedence over any explicit
			/// or implicit calendar type given by the
			/// <seealso cref="#setLocale(Locale) locale"/>.
			/// 
			/// <para>In addition to the available calendar types returned by the
			/// <seealso cref="Calendar#getAvailableCalendarTypes() Calendar.getAvailableCalendarTypes"/>
			/// method, {@code "gregorian"} and {@code "iso8601"} as aliases of
			/// {@code "gregory"} can be used with this method.
			/// 
			/// </para>
			/// </summary>
			/// <param name="type"> the calendar type </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="NullPointerException"> if {@code type} is {@code null} </exception>
			/// <exception cref="IllegalArgumentException"> if {@code type} is unknown </exception>
			/// <exception cref="IllegalStateException"> if another calendar type has already been set </exception>
			/// <seealso cref= Calendar#getCalendarType() </seealso>
			/// <seealso cref= Calendar#getAvailableCalendarTypes() </seealso>
			public virtual Builder SetCalendarType(String type)
			{
				if (type.Equals("gregorian")) // NPE if type == null
				{
					type = "gregory";
				}
				if (!Calendar.AvailableCalendarTypes.Contains(type) && !type.Equals("iso8601"))
				{
					throw new IllegalArgumentException("unknown calendar type: " + type);
				}
				if (this.Type == null)
				{
					this.Type = type;
				}
				else
				{
					if (!this.Type.Equals(type))
					{
						throw new IllegalStateException("calendar type override");
					}
				}
				return this;
			}

			/// <summary>
			/// Sets the locale parameter to the given {@code locale}. If no locale
			/// is given to this {@code Calendar.Builder}, the {@linkplain
			/// Locale#getDefault(Locale.Category) default <code>Locale</code>}
			/// for <seealso cref="Locale.Category#FORMAT"/> will be used.
			/// 
			/// <para>If no calendar type is explicitly given by a call to the
			/// <seealso cref="#setCalendarType(String) setCalendarType"/> method,
			/// the {@code Locale} value is used to determine what type of
			/// {@code Calendar} to be built.
			/// 
			/// </para>
			/// <para>If no week definition parameters are explicitly given by a call to
			/// the <seealso cref="#setWeekDefinition(int,int) setWeekDefinition"/> method, the
			/// {@code Locale}'s default values are used.
			/// 
			/// </para>
			/// </summary>
			/// <param name="locale"> the <seealso cref="Locale"/> </param>
			/// <exception cref="NullPointerException"> if {@code locale} is {@code null} </exception>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <seealso cref= Calendar#getInstance(Locale) </seealso>
			public virtual Builder SetLocale(Locale locale)
			{
				if (locale == null)
				{
					throw new NullPointerException();
				}
				this.Locale_Renamed = locale;
				return this;
			}

			/// <summary>
			/// Sets the week definition parameters to the values given by
			/// {@code firstDayOfWeek} and {@code minimalDaysInFirstWeek} that are
			/// used to determine the <a href="Calendar.html#First_Week">first
			/// week</a> of a year. The parameters given by this method have
			/// precedence over the default values given by the
			/// <seealso cref="#setLocale(Locale) locale"/>.
			/// </summary>
			/// <param name="firstDayOfWeek"> the first day of a week; one of
			///                       <seealso cref="Calendar#SUNDAY"/> to <seealso cref="Calendar#SATURDAY"/> </param>
			/// <param name="minimalDaysInFirstWeek"> the minimal number of days in the first
			///                               week (1..7) </param>
			/// <returns> this {@code Calendar.Builder} </returns>
			/// <exception cref="IllegalArgumentException"> if {@code firstDayOfWeek} or
			///                                  {@code minimalDaysInFirstWeek} is invalid </exception>
			/// <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
			/// <seealso cref= Calendar#getMinimalDaysInFirstWeek() </seealso>
			public virtual Builder SetWeekDefinition(int firstDayOfWeek, int minimalDaysInFirstWeek)
			{
				if (!IsValidWeekParameter(firstDayOfWeek) || !IsValidWeekParameter(minimalDaysInFirstWeek))
				{
					throw new IllegalArgumentException();
				}
				this.FirstDayOfWeek = firstDayOfWeek;
				this.MinimalDaysInFirstWeek = minimalDaysInFirstWeek;
				return this;
			}

			/// <summary>
			/// Returns a {@code Calendar} built from the parameters set by the
			/// setter methods. The calendar type given by the {@link #setCalendarType(String)
			/// setCalendarType} method or the <seealso cref="#setLocale(Locale) locale"/> is
			/// used to determine what {@code Calendar} to be created. If no explicit
			/// calendar type is given, the locale's default calendar is created.
			/// 
			/// <para>If the calendar type is {@code "iso8601"}, the
			/// <seealso cref="GregorianCalendar#setGregorianChange(Date) Gregorian change date"/>
			/// of a <seealso cref="GregorianCalendar"/> is set to {@code Date(Long.MIN_VALUE)}
			/// to be the <em>proleptic</em> Gregorian calendar. Its week definition
			/// parameters are also set to be <a
			/// href="GregorianCalendar.html#iso8601_compatible_setting">compatible
			/// with the ISO 8601 standard</a>. Note that the
			/// <seealso cref="GregorianCalendar#getCalendarType() getCalendarType"/> method of
			/// a {@code GregorianCalendar} created with {@code "iso8601"} returns
			/// {@code "gregory"}.
			/// 
			/// </para>
			/// <para>The default values are used for locale and time zone if these
			/// parameters haven't been given explicitly.
			/// 
			/// </para>
			/// <para>Any out of range field values are either normalized in lenient
			/// mode or detected as an invalid value in non-lenient mode.
			/// 
			/// </para>
			/// </summary>
			/// <returns> a {@code Calendar} built with parameters of this {@code
			///         Calendar.Builder} </returns>
			/// <exception cref="IllegalArgumentException"> if the calendar type is unknown, or
			///             if any invalid field values are given in non-lenient mode, or
			///             if a week date is given for the calendar type that doesn't
			///             support week dates. </exception>
			/// <seealso cref= Calendar#getInstance(TimeZone, Locale) </seealso>
			/// <seealso cref= Locale#getDefault(Locale.Category) </seealso>
			/// <seealso cref= TimeZone#getDefault() </seealso>
			public virtual Calendar Build()
			{
				if (Locale_Renamed == null)
				{
					Locale_Renamed = Locale.Default;
				}
				if (Zone == null)
				{
					Zone = TimeZone.Default;
				}
				Calendar cal;
				if (Type == null)
				{
					Type = Locale_Renamed.GetUnicodeLocaleType("ca");
				}
				if (Type == null)
				{
					if (Locale_Renamed.Country == "TH" && Locale_Renamed.Language == "th")
					{
						Type = "buddhist";
					}
					else
					{
						Type = "gregory";
					}
				}
				switch (Type)
				{
				case "gregory":
					cal = new GregorianCalendar(Zone, Locale_Renamed, true);
					break;
				case "iso8601":
					GregorianCalendar gcal = new GregorianCalendar(Zone, Locale_Renamed, true);
					// make gcal a proleptic Gregorian
					gcal.GregorianChange = new Date(Long.MinValue);
					// and week definition to be compatible with ISO 8601
					SetWeekDefinition(MONDAY, 4);
					cal = gcal;
					break;
				case "buddhist":
					cal = new BuddhistCalendar(Zone, Locale_Renamed);
					cal.Clear();
					break;
				case "japanese":
					cal = new JapaneseImperialCalendar(Zone, Locale_Renamed, true);
					break;
				default:
					throw new IllegalArgumentException("unknown calendar type: " + Type);
				}
				cal.Lenient = Lenient_Renamed;
				if (FirstDayOfWeek != 0)
				{
					cal.FirstDayOfWeek = FirstDayOfWeek;
					cal.MinimalDaysInFirstWeek = MinimalDaysInFirstWeek;
				}
				if (InstantSet)
				{
					cal.TimeInMillis = Instant_Renamed;
					cal.Complete();
					return cal;
				}

				if (Fields_Renamed != null)
				{
					bool weekDate = IsSet(WEEK_YEAR) && Fields_Renamed[WEEK_YEAR] > Fields_Renamed[YEAR];
					if (weekDate && !cal.WeekDateSupported)
					{
						throw new IllegalArgumentException("week date is unsupported by " + Type);
					}

					// Set the fields from the min stamp to the max stamp so that
					// the fields resolution works in the Calendar.
					for (int stamp = MINIMUM_USER_STAMP; stamp < NextStamp; stamp++)
					{
						for (int index = 0; index <= MaxFieldIndex; index++)
						{
							if (Fields_Renamed[index] == stamp)
							{
								cal.Set(index, Fields_Renamed[NFIELDS + index]);
								break;
							}
						}
					}

					if (weekDate)
					{
						int weekOfYear = IsSet(WEEK_OF_YEAR) ? Fields_Renamed[NFIELDS + WEEK_OF_YEAR] : 1;
						int dayOfWeek = IsSet(DAY_OF_WEEK) ? Fields_Renamed[NFIELDS + DAY_OF_WEEK] : cal.FirstDayOfWeek;
						cal.SetWeekDate(Fields_Renamed[NFIELDS + WEEK_YEAR], weekOfYear, dayOfWeek);
					}
					cal.Complete();
				}

				return cal;
			}

			internal virtual void AllocateFields()
			{
				if (Fields_Renamed == null)
				{
					Fields_Renamed = new int[NFIELDS * 2];
					NextStamp = MINIMUM_USER_STAMP;
					MaxFieldIndex = -1;
				}
			}

			internal virtual void InternalSet(int field, int value)
			{
				Fields_Renamed[field] = NextStamp++;
				if (NextStamp < 0)
				{
					throw new IllegalStateException("stamp counter overflow");
				}
				Fields_Renamed[NFIELDS + field] = value;
				if (field > MaxFieldIndex && field < WEEK_YEAR)
				{
					MaxFieldIndex = field;
				}
			}

			internal virtual bool InstantSet
			{
				get
				{
					return NextStamp == COMPUTED;
				}
			}

			internal virtual bool IsSet(int index)
			{
				return Fields_Renamed != null && Fields_Renamed[index] > UNSET;
			}

			internal virtual bool IsValidWeekParameter(int value)
			{
				return value > 0 && value <= 7;
			}
		}

		/// <summary>
		/// Constructs a Calendar with the default time zone
		/// and the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/>
		/// locale. </summary>
		/// <seealso cref=     TimeZone#getDefault </seealso>
		protected internal Calendar() : this(TimeZone.DefaultRef, Locale.GetDefault(Locale.Category.FORMAT))
		{
			SharedZone = true;
		}

		/// <summary>
		/// Constructs a calendar with the specified time zone and locale.
		/// </summary>
		/// <param name="zone"> the time zone to use </param>
		/// <param name="aLocale"> the locale for the week data </param>
		protected internal Calendar(TimeZone zone, Locale aLocale)
		{
			Fields = new int[FIELD_COUNT];
			IsSet_Renamed = new bool[FIELD_COUNT];
			Stamp = new int[FIELD_COUNT];

			this.Zone_Renamed = zone;
			WeekCountData = aLocale;
		}

		/// <summary>
		/// Gets a calendar using the default time zone and locale. The
		/// <code>Calendar</code> returned is based on the current time
		/// in the default time zone with the default
		/// <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		/// </summary>
		/// <returns> a Calendar. </returns>
		public static Calendar Instance
		{
			get
			{
				return CreateCalendar(TimeZone.Default, Locale.GetDefault(Locale.Category.FORMAT));
			}
		}

		/// <summary>
		/// Gets a calendar using the specified time zone and default locale.
		/// The <code>Calendar</code> returned is based on the current time
		/// in the given time zone with the default
		/// <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		/// </summary>
		/// <param name="zone"> the time zone to use </param>
		/// <returns> a Calendar. </returns>
		public static Calendar GetInstance(TimeZone zone)
		{
			return CreateCalendar(zone, Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Gets a calendar using the default time zone and specified locale.
		/// The <code>Calendar</code> returned is based on the current time
		/// in the default time zone with the given locale.
		/// </summary>
		/// <param name="aLocale"> the locale for the week data </param>
		/// <returns> a Calendar. </returns>
		public static Calendar GetInstance(Locale aLocale)
		{
			return CreateCalendar(TimeZone.Default, aLocale);
		}

		/// <summary>
		/// Gets a calendar with the specified time zone and locale.
		/// The <code>Calendar</code> returned is based on the current time
		/// in the given time zone with the given locale.
		/// </summary>
		/// <param name="zone"> the time zone to use </param>
		/// <param name="aLocale"> the locale for the week data </param>
		/// <returns> a Calendar. </returns>
		public static Calendar GetInstance(TimeZone zone, Locale aLocale)
		{
			return CreateCalendar(zone, aLocale);
		}

		private static Calendar CreateCalendar(TimeZone zone, Locale aLocale)
		{
			CalendarProvider provider = LocaleProviderAdapter.getAdapter(typeof(CalendarProvider), aLocale).CalendarProvider;
			if (provider != null)
			{
				try
				{
					return provider.getInstance(zone, aLocale);
				}
				catch (IllegalArgumentException)
				{
					// fall back to the default instantiation
				}
			}

			Calendar cal = null;

			if (aLocale.HasExtensions())
			{
				String caltype = aLocale.GetUnicodeLocaleType("ca");
				if (caltype != null)
				{
					switch (caltype)
					{
					case "buddhist":
					cal = new BuddhistCalendar(zone, aLocale);
						break;
					case "japanese":
						cal = new JapaneseImperialCalendar(zone, aLocale);
						break;
					case "gregory":
						cal = new GregorianCalendar(zone, aLocale);
						break;
					}
				}
			}
			if (cal == null)
			{
				// If no known calendar type is explicitly specified,
				// perform the traditional way to create a Calendar:
				// create a BuddhistCalendar for th_TH locale,
				// a JapaneseImperialCalendar for ja_JP_JP locale, or
				// a GregorianCalendar for any other locales.
				// NOTE: The language, country and variant strings are interned.
				if (aLocale.Language == "th" && aLocale.Country == "TH")
				{
					cal = new BuddhistCalendar(zone, aLocale);
				}
				else if (aLocale.Variant == "JP" && aLocale.Language == "ja" && aLocale.Country == "JP")
				{
					cal = new JapaneseImperialCalendar(zone, aLocale);
				}
				else
				{
					cal = new GregorianCalendar(zone, aLocale);
				}
			}
			return cal;
		}

		/// <summary>
		/// Returns an array of all locales for which the <code>getInstance</code>
		/// methods of this class can return localized instances.
		/// The array returned must contain at least a <code>Locale</code>
		/// instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of locales for which localized
		///         <code>Calendar</code> instances are available. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				lock (typeof(Calendar))
				{
					return DateFormat.AvailableLocales;
				}
			}
		}

		/// <summary>
		/// Converts the current calendar field values in <seealso cref="#fields fields[]"/>
		/// to the millisecond time value
		/// <seealso cref="#time"/>.
		/// </summary>
		/// <seealso cref= #complete() </seealso>
		/// <seealso cref= #computeFields() </seealso>
		protected internal abstract void ComputeTime();

		/// <summary>
		/// Converts the current millisecond time value <seealso cref="#time"/>
		/// to calendar field values in <seealso cref="#fields fields[]"/>.
		/// This allows you to sync up the calendar field values with
		/// a new time that is set for the calendar.  The time is <em>not</em>
		/// recomputed first; to recompute the time, then the fields, call the
		/// <seealso cref="#complete()"/> method.
		/// </summary>
		/// <seealso cref= #computeTime() </seealso>
		protected internal abstract void ComputeFields();

		/// <summary>
		/// Returns a <code>Date</code> object representing this
		/// <code>Calendar</code>'s time value (millisecond offset from the <a
		/// href="#Epoch">Epoch</a>").
		/// </summary>
		/// <returns> a <code>Date</code> representing the time value. </returns>
		/// <seealso cref= #setTime(Date) </seealso>
		/// <seealso cref= #getTimeInMillis() </seealso>
		public Date Time
		{
			get
			{
				return new Date(TimeInMillis);
			}
			set
			{
				TimeInMillis = value.Time;
			}
		}


		/// <summary>
		/// Returns this Calendar's time value in milliseconds.
		/// </summary>
		/// <returns> the current time as UTC milliseconds from the epoch. </returns>
		/// <seealso cref= #getTime() </seealso>
		/// <seealso cref= #setTimeInMillis(long) </seealso>
		public virtual long TimeInMillis
		{
			get
			{
				if (!IsTimeSet)
				{
					UpdateTime();
				}
				return Time_Renamed;
			}
			set
			{
				// If we don't need to recalculate the calendar field values,
				// do nothing.
				if (Time_Renamed == value && IsTimeSet && AreFieldsSet && AreAllFieldsSet && (Zone_Renamed is ZoneInfo) && !((ZoneInfo)Zone_Renamed).Dirty)
				{
					return;
				}
				Time_Renamed = value;
				IsTimeSet = true;
				AreFieldsSet = false;
				ComputeFields();
				AreAllFieldsSet = AreFieldsSet = true;
			}
		}


		/// <summary>
		/// Returns the value of the given calendar field. In lenient mode,
		/// all calendar fields are normalized. In non-lenient mode, all
		/// calendar fields are validated and this method throws an
		/// exception if any calendar fields have out-of-range values. The
		/// normalization and validation are handled by the
		/// <seealso cref="#complete()"/> method, which process is calendar
		/// system dependent.
		/// </summary>
		/// <param name="field"> the given calendar field. </param>
		/// <returns> the value for the given calendar field. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the specified field is out of range
		///             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		/// <seealso cref= #set(int,int) </seealso>
		/// <seealso cref= #complete() </seealso>
		public virtual int Get(int field)
		{
			Complete();
			return InternalGet(field);
		}

		/// <summary>
		/// Returns the value of the given calendar field. This method does
		/// not involve normalization or validation of the field value.
		/// </summary>
		/// <param name="field"> the given calendar field. </param>
		/// <returns> the value for the given calendar field. </returns>
		/// <seealso cref= #get(int) </seealso>
		protected internal int InternalGet(int field)
		{
			return Fields[field];
		}

		/// <summary>
		/// Sets the value of the given calendar field. This method does
		/// not affect any setting state of the field in this
		/// <code>Calendar</code> instance.
		/// </summary>
		/// <exception cref="IndexOutOfBoundsException"> if the specified field is out of range
		///             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		/// <seealso cref= #areFieldsSet </seealso>
		/// <seealso cref= #isTimeSet </seealso>
		/// <seealso cref= #areAllFieldsSet </seealso>
		/// <seealso cref= #set(int,int) </seealso>
		internal void InternalSet(int field, int value)
		{
			Fields[field] = value;
		}

		/// <summary>
		/// Sets the given calendar field to the given value. The value is not
		/// interpreted by this method regardless of the leniency mode.
		/// </summary>
		/// <param name="field"> the given calendar field. </param>
		/// <param name="value"> the value to be set for the given calendar field. </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the specified field is out of range
		///             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>).
		/// in non-lenient mode. </exception>
		/// <seealso cref= #set(int,int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int,int) </seealso>
		/// <seealso cref= #get(int) </seealso>
		public virtual void Set(int field, int value)
		{
			// If the fields are partially normalized, calculate all the
			// fields before changing any fields.
			if (AreFieldsSet && !AreAllFieldsSet)
			{
				ComputeFields();
			}
			InternalSet(field, value);
			IsTimeSet = false;
			AreFieldsSet = false;
			IsSet_Renamed[field] = true;
			Stamp[field] = NextStamp++;
			if (NextStamp == Integer.MaxValue)
			{
				AdjustStamp();
			}
		}

		/// <summary>
		/// Sets the values for the calendar fields <code>YEAR</code>,
		/// <code>MONTH</code>, and <code>DAY_OF_MONTH</code>.
		/// Previous values of other calendar fields are retained.  If this is not desired,
		/// call <seealso cref="#clear()"/> first.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		/// <seealso cref= #set(int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int,int) </seealso>
		public void Set(int year, int month, int date)
		{
			Set(YEAR, year);
			Set(MONTH, month);
			Set(DATE, date);
		}

		/// <summary>
		/// Sets the values for the calendar fields <code>YEAR</code>,
		/// <code>MONTH</code>, <code>DAY_OF_MONTH</code>,
		/// <code>HOUR_OF_DAY</code>, and <code>MINUTE</code>.
		/// Previous values of other fields are retained.  If this is not desired,
		/// call <seealso cref="#clear()"/> first.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		/// <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field. </param>
		/// <param name="minute"> the value used to set the <code>MINUTE</code> calendar field. </param>
		/// <seealso cref= #set(int,int) </seealso>
		/// <seealso cref= #set(int,int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int,int) </seealso>
		public void Set(int year, int month, int date, int hourOfDay, int minute)
		{
			Set(YEAR, year);
			Set(MONTH, month);
			Set(DATE, date);
			Set(HOUR_OF_DAY, hourOfDay);
			Set(MINUTE, minute);
		}

		/// <summary>
		/// Sets the values for the fields <code>YEAR</code>, <code>MONTH</code>,
		/// <code>DAY_OF_MONTH</code>, <code>HOUR_OF_DAY</code>, <code>MINUTE</code>, and
		/// <code>SECOND</code>.
		/// Previous values of other fields are retained.  If this is not desired,
		/// call <seealso cref="#clear()"/> first.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		/// <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field. </param>
		/// <param name="minute"> the value used to set the <code>MINUTE</code> calendar field. </param>
		/// <param name="second"> the value used to set the <code>SECOND</code> calendar field. </param>
		/// <seealso cref= #set(int,int) </seealso>
		/// <seealso cref= #set(int,int,int) </seealso>
		/// <seealso cref= #set(int,int,int,int,int) </seealso>
		public void Set(int year, int month, int date, int hourOfDay, int minute, int second)
		{
			Set(YEAR, year);
			Set(MONTH, month);
			Set(DATE, date);
			Set(HOUR_OF_DAY, hourOfDay);
			Set(MINUTE, minute);
			Set(SECOND, second);
		}

		/// <summary>
		/// Sets all the calendar field values and the time value
		/// (millisecond offset from the <a href="#Epoch">Epoch</a>) of
		/// this <code>Calendar</code> undefined. This means that {@link
		/// #isSet(int) isSet()} will return <code>false</code> for all the
		/// calendar fields, and the date and time calculations will treat
		/// the fields as if they had never been set. A
		/// <code>Calendar</code> implementation class may use its specific
		/// default field values for date/time calculations. For example,
		/// <code>GregorianCalendar</code> uses 1970 if the
		/// <code>YEAR</code> field value is undefined.
		/// </summary>
		/// <seealso cref= #clear(int) </seealso>
		public void Clear()
		{
			for (int i = 0; i < Fields.Length;)
			{
				Stamp[i] = Fields[i] = 0; // UNSET == 0
				IsSet_Renamed[i++] = false;
			}
			AreAllFieldsSet = AreFieldsSet = false;
			IsTimeSet = false;
		}

		/// <summary>
		/// Sets the given calendar field value and the time value
		/// (millisecond offset from the <a href="#Epoch">Epoch</a>) of
		/// this <code>Calendar</code> undefined. This means that {@link
		/// #isSet(int) isSet(field)} will return <code>false</code>, and
		/// the date and time calculations will treat the field as if it
		/// had never been set. A <code>Calendar</code> implementation
		/// class may use the field's specific default value for date and
		/// time calculations.
		/// 
		/// <para>The <seealso cref="#HOUR_OF_DAY"/>, <seealso cref="#HOUR"/> and <seealso cref="#AM_PM"/>
		/// fields are handled independently and the <a
		/// href="#time_resolution">the resolution rule for the time of
		/// day</a> is applied. Clearing one of the fields doesn't reset
		/// the hour of day value of this <code>Calendar</code>. Use {@link
		/// #set(int,int) set(Calendar.HOUR_OF_DAY, 0)} to reset the hour
		/// value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field to be cleared. </param>
		/// <seealso cref= #clear() </seealso>
		public void Clear(int field)
		{
			Fields[field] = 0;
			Stamp[field] = UNSET;
			IsSet_Renamed[field] = false;

			AreAllFieldsSet = AreFieldsSet = false;
			IsTimeSet = false;
		}

		/// <summary>
		/// Determines if the given calendar field has a value set,
		/// including cases that the value has been set by internal fields
		/// calculations triggered by a <code>get</code> method call.
		/// </summary>
		/// <param name="field"> the calendar field to test </param>
		/// <returns> <code>true</code> if the given calendar field has a value set;
		/// <code>false</code> otherwise. </returns>
		public bool IsSet(int field)
		{
			return Stamp[field] != UNSET;
		}

		/// <summary>
		/// Returns the string representation of the calendar
		/// <code>field</code> value in the given <code>style</code> and
		/// <code>locale</code>.  If no string representation is
		/// applicable, <code>null</code> is returned. This method calls
		/// <seealso cref="Calendar#get(int) get(field)"/> to get the calendar
		/// <code>field</code> value if the string representation is
		/// applicable to the given calendar <code>field</code>.
		/// 
		/// <para>For example, if this <code>Calendar</code> is a
		/// <code>GregorianCalendar</code> and its date is 2005-01-01, then
		/// the string representation of the <seealso cref="#MONTH"/> field would be
		/// "January" in the long style in an English locale or "Jan" in
		/// the short style. However, no string representation would be
		/// available for the <seealso cref="#DAY_OF_MONTH"/> field, and this method
		/// would return <code>null</code>.
		/// 
		/// </para>
		/// <para>The default implementation supports the calendar fields for
		/// which a <seealso cref="DateFormatSymbols"/> has names in the given
		/// <code>locale</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">
		///        the calendar field for which the string representation
		///        is returned </param>
		/// <param name="style">
		///        the style applied to the string representation; one of {@link
		///        #SHORT_FORMAT} (<seealso cref="#SHORT"/>), <seealso cref="#SHORT_STANDALONE"/>,
		///        <seealso cref="#LONG_FORMAT"/> (<seealso cref="#LONG"/>), <seealso cref="#LONG_STANDALONE"/>,
		///        <seealso cref="#NARROW_FORMAT"/>, or <seealso cref="#NARROW_STANDALONE"/>. </param>
		/// <param name="locale">
		///        the locale for the string representation
		///        (any calendar types specified by {@code locale} are ignored) </param>
		/// <returns> the string representation of the given
		///        {@code field} in the given {@code style}, or
		///        {@code null} if no string representation is
		///        applicable. </returns>
		/// <exception cref="IllegalArgumentException">
		///        if {@code field} or {@code style} is invalid,
		///        or if this {@code Calendar} is non-lenient and any
		///        of the calendar fields have invalid values </exception>
		/// <exception cref="NullPointerException">
		///        if {@code locale} is null
		/// @since 1.6 </exception>
		public virtual String GetDisplayName(int field, int style, Locale locale)
		{
			if (!CheckDisplayNameParams(field, style, SHORT, NARROW_FORMAT, locale, ERA_MASK | MONTH_MASK | DAY_OF_WEEK_MASK | AM_PM_MASK))
			{
				return null;
			}

			String calendarType = CalendarType;
			int fieldValue = Get(field);
			// the standalone and narrow styles are supported only through CalendarDataProviders.
			if (IsStandaloneStyle(style) || IsNarrowFormatStyle(style))
			{
				String val = CalendarDataUtility.retrieveFieldValueName(calendarType, field, fieldValue, style, locale);
				// Perform fallback here to follow the CLDR rules
				if (val == null)
				{
					if (IsNarrowFormatStyle(style))
					{
						val = CalendarDataUtility.retrieveFieldValueName(calendarType, field, fieldValue, ToStandaloneStyle(style), locale);
					}
					else if (IsStandaloneStyle(style))
					{
						val = CalendarDataUtility.retrieveFieldValueName(calendarType, field, fieldValue, GetBaseStyle(style), locale);
					}
				}
				return val;
			}

			DateFormatSymbols symbols = DateFormatSymbols.GetInstance(locale);
			String[] strings = GetFieldStrings(field, style, symbols);
			if (strings != null)
			{
				if (fieldValue < strings.Length)
				{
					return strings[fieldValue];
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a {@code Map} containing all names of the calendar
		/// {@code field} in the given {@code style} and
		/// {@code locale} and their corresponding field values. For
		/// example, if this {@code Calendar} is a {@link
		/// GregorianCalendar}, the returned map would contain "Jan" to
		/// <seealso cref="#JANUARY"/>, "Feb" to <seealso cref="#FEBRUARY"/>, and so on, in the
		/// <seealso cref="#SHORT short"/> style in an English locale.
		/// 
		/// <para>Narrow names may not be unique due to use of single characters,
		/// such as "S" for Sunday and Saturday. In that case narrow names are not
		/// included in the returned {@code Map}.
		/// 
		/// </para>
		/// <para>The values of other calendar fields may be taken into
		/// account to determine a set of display names. For example, if
		/// this {@code Calendar} is a lunisolar calendar system and
		/// the year value given by the <seealso cref="#YEAR"/> field has a leap
		/// month, this method would return month names containing the leap
		/// month name, and month names are mapped to their values specific
		/// for the year.
		/// 
		/// </para>
		/// <para>The default implementation supports display names contained in
		/// a <seealso cref="DateFormatSymbols"/>. For example, if {@code field}
		/// is <seealso cref="#MONTH"/> and {@code style} is {@link
		/// #ALL_STYLES}, this method returns a {@code Map} containing
		/// all strings returned by <seealso cref="DateFormatSymbols#getShortMonths()"/>
		/// and <seealso cref="DateFormatSymbols#getMonths()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">
		///        the calendar field for which the display names are returned </param>
		/// <param name="style">
		///        the style applied to the string representation; one of {@link
		///        #SHORT_FORMAT} (<seealso cref="#SHORT"/>), <seealso cref="#SHORT_STANDALONE"/>,
		///        <seealso cref="#LONG_FORMAT"/> (<seealso cref="#LONG"/>), <seealso cref="#LONG_STANDALONE"/>,
		///        <seealso cref="#NARROW_FORMAT"/>, or <seealso cref="#NARROW_STANDALONE"/> </param>
		/// <param name="locale">
		///        the locale for the display names </param>
		/// <returns> a {@code Map} containing all display names in
		///        {@code style} and {@code locale} and their
		///        field values, or {@code null} if no display names
		///        are defined for {@code field} </returns>
		/// <exception cref="IllegalArgumentException">
		///        if {@code field} or {@code style} is invalid,
		///        or if this {@code Calendar} is non-lenient and any
		///        of the calendar fields have invalid values </exception>
		/// <exception cref="NullPointerException">
		///        if {@code locale} is null
		/// @since 1.6 </exception>
		public virtual Map<String, Integer> GetDisplayNames(int field, int style, Locale locale)
		{
			if (!CheckDisplayNameParams(field, style, ALL_STYLES, NARROW_FORMAT, locale, ERA_MASK | MONTH_MASK | DAY_OF_WEEK_MASK | AM_PM_MASK))
			{
				return null;
			}

			String calendarType = CalendarType;
			if (style == ALL_STYLES || IsStandaloneStyle(style) || IsNarrowFormatStyle(style))
			{
				Map<String, Integer> map;
				map = CalendarDataUtility.retrieveFieldValueNames(calendarType, field, style, locale);

				// Perform fallback here to follow the CLDR rules
				if (map == null)
				{
					if (IsNarrowFormatStyle(style))
					{
						map = CalendarDataUtility.retrieveFieldValueNames(calendarType, field, ToStandaloneStyle(style), locale);
					}
					else if (style != ALL_STYLES)
					{
						map = CalendarDataUtility.retrieveFieldValueNames(calendarType, field, GetBaseStyle(style), locale);
					}
				}
				return map;
			}

			// SHORT or LONG
			return GetDisplayNamesImpl(field, style, locale);
		}

		private Map<String, Integer> GetDisplayNamesImpl(int field, int style, Locale locale)
		{
			DateFormatSymbols symbols = DateFormatSymbols.GetInstance(locale);
			String[] strings = GetFieldStrings(field, style, symbols);
			if (strings != null)
			{
				Map<String, Integer> names = new HashMap<String, Integer>();
				for (int i = 0; i < strings.Length; i++)
				{
					if (strings[i].Length() == 0)
					{
						continue;
					}
					names.Put(strings[i], i);
				}
				return names;
			}
			return null;
		}

		internal virtual bool CheckDisplayNameParams(int field, int style, int minStyle, int maxStyle, Locale locale, int fieldMask)
		{
			int baseStyle = GetBaseStyle(style); // Ignore the standalone mask
			if (field < 0 || field >= Fields.Length || baseStyle < minStyle || baseStyle > maxStyle)
			{
				throw new IllegalArgumentException();
			}
			if (locale == null)
			{
				throw new NullPointerException();
			}
			return IsFieldSet(fieldMask, field);
		}

		private String[] GetFieldStrings(int field, int style, DateFormatSymbols symbols)
		{
			int baseStyle = GetBaseStyle(style); // ignore the standalone mask

			// DateFormatSymbols doesn't support any narrow names.
			if (baseStyle == NARROW_FORMAT)
			{
				return null;
			}

			String[] strings = null;
			switch (field)
			{
			case ERA:
				strings = symbols.Eras;
				break;

			case MONTH:
				strings = (baseStyle == LONG) ? symbols.Months : symbols.ShortMonths;
				break;

			case DAY_OF_WEEK:
				strings = (baseStyle == LONG) ? symbols.Weekdays : symbols.ShortWeekdays;
				break;

			case AM_PM:
				strings = symbols.AmPmStrings;
				break;
			}
			return strings;
		}

		/// <summary>
		/// Fills in any unset fields in the calendar fields. First, the {@link
		/// #computeTime()} method is called if the time value (millisecond offset
		/// from the <a href="#Epoch">Epoch</a>) has not been calculated from
		/// calendar field values. Then, the <seealso cref="#computeFields()"/> method is
		/// called to calculate all calendar field values.
		/// </summary>
		protected internal virtual void Complete()
		{
			if (!IsTimeSet)
			{
				UpdateTime();
			}
			if (!AreFieldsSet || !AreAllFieldsSet)
			{
				ComputeFields(); // fills in unset fields
				AreAllFieldsSet = AreFieldsSet = true;
			}
		}

		/// <summary>
		/// Returns whether the value of the specified calendar field has been set
		/// externally by calling one of the setter methods rather than by the
		/// internal time calculation.
		/// </summary>
		/// <returns> <code>true</code> if the field has been set externally,
		/// <code>false</code> otherwise. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified
		///                <code>field</code> is out of range
		///               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		/// <seealso cref= #selectFields() </seealso>
		/// <seealso cref= #setFieldsComputed(int) </seealso>
		internal bool IsExternallySet(int field)
		{
			return Stamp[field] >= MINIMUM_USER_STAMP;
		}

		/// <summary>
		/// Returns a field mask (bit mask) indicating all calendar fields that
		/// have the state of externally or internally set.
		/// </summary>
		/// <returns> a bit mask indicating set state fields </returns>
		internal int SetStateFields
		{
			get
			{
				int mask = 0;
				for (int i = 0; i < Fields.Length; i++)
				{
					if (Stamp[i] != UNSET)
					{
						mask |= 1 << i;
					}
				}
				return mask;
			}
		}

		/// <summary>
		/// Sets the state of the specified calendar fields to
		/// <em>computed</em>. This state means that the specified calendar fields
		/// have valid values that have been set by internal time calculation
		/// rather than by calling one of the setter methods.
		/// </summary>
		/// <param name="fieldMask"> the field to be marked as computed. </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified
		///                <code>field</code> is out of range
		///               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		/// <seealso cref= #isExternallySet(int) </seealso>
		/// <seealso cref= #selectFields() </seealso>
		internal int FieldsComputed
		{
			set
			{
				if (value == ALL_FIELDS)
				{
					for (int i = 0; i < Fields.Length; i++)
					{
						Stamp[i] = COMPUTED;
						IsSet_Renamed[i] = true;
					}
					AreFieldsSet = AreAllFieldsSet = true;
				}
				else
				{
					for (int i = 0; i < Fields.Length; i++)
					{
						if ((value & 1) == 1)
						{
							Stamp[i] = COMPUTED;
							IsSet_Renamed[i] = true;
						}
						else
						{
							if (AreAllFieldsSet && !IsSet_Renamed[i])
							{
								AreAllFieldsSet = false;
							}
						}
						value = (int)((uint)value >> 1);
					}
				}
			}
		}

		/// <summary>
		/// Sets the state of the calendar fields that are <em>not</em> specified
		/// by <code>fieldMask</code> to <em>unset</em>. If <code>fieldMask</code>
		/// specifies all the calendar fields, then the state of this
		/// <code>Calendar</code> becomes that all the calendar fields are in sync
		/// with the time value (millisecond offset from the Epoch).
		/// </summary>
		/// <param name="fieldMask"> the field mask indicating which calendar fields are in
		/// sync with the time value. </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified
		///                <code>field</code> is out of range
		///               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		/// <seealso cref= #isExternallySet(int) </seealso>
		/// <seealso cref= #selectFields() </seealso>
		internal int FieldsNormalized
		{
			set
			{
				if (value != ALL_FIELDS)
				{
					for (int i = 0; i < Fields.Length; i++)
					{
						if ((value & 1) == 0)
						{
							Stamp[i] = Fields[i] = 0; // UNSET == 0
							IsSet_Renamed[i] = false;
						}
						value >>= 1;
					}
				}
    
				// Some or all of the fields are in sync with the
				// milliseconds, but the stamp values are not normalized yet.
				AreFieldsSet = true;
				AreAllFieldsSet = false;
			}
		}

		/// <summary>
		/// Returns whether the calendar fields are partially in sync with the time
		/// value or fully in sync but not stamp values are not normalized yet.
		/// </summary>
		internal bool PartiallyNormalized
		{
			get
			{
				return AreFieldsSet && !AreAllFieldsSet;
			}
		}

		/// <summary>
		/// Returns whether the calendar fields are fully in sync with the time
		/// value.
		/// </summary>
		internal bool FullyNormalized
		{
			get
			{
				return AreFieldsSet && AreAllFieldsSet;
			}
		}

		/// <summary>
		/// Marks this Calendar as not sync'd.
		/// </summary>
		internal void SetUnnormalized()
		{
			AreFieldsSet = AreAllFieldsSet = false;
		}

		/// <summary>
		/// Returns whether the specified <code>field</code> is on in the
		/// <code>fieldMask</code>.
		/// </summary>
		internal static bool IsFieldSet(int fieldMask, int field)
		{
			return (fieldMask & (1 << field)) != 0;
		}

		/// <summary>
		/// Returns a field mask indicating which calendar field values
		/// to be used to calculate the time value. The calendar fields are
		/// returned as a bit mask, each bit of which corresponds to a field, i.e.,
		/// the mask value of <code>field</code> is <code>(1 &lt;&lt;
		/// field)</code>. For example, 0x26 represents the <code>YEAR</code>,
		/// <code>MONTH</code>, and <code>DAY_OF_MONTH</code> fields (i.e., 0x26 is
		/// equal to
		/// <code>(1&lt;&lt;YEAR)|(1&lt;&lt;MONTH)|(1&lt;&lt;DAY_OF_MONTH))</code>.
		/// 
		/// <para>This method supports the calendar fields resolution as described in
		/// the class description. If the bit mask for a given field is on and its
		/// field has not been set (i.e., <code>isSet(field)</code> is
		/// <code>false</code>), then the default value of the field has to be
		/// used, which case means that the field has been selected because the
		/// selected combination involves the field.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a bit mask of selected fields </returns>
		/// <seealso cref= #isExternallySet(int) </seealso>
		internal int SelectFields()
		{
			// This implementation has been taken from the GregorianCalendar class.

			// The YEAR field must always be used regardless of its SET
			// state because YEAR is a mandatory field to determine the date
			// and the default value (EPOCH_YEAR) may change through the
			// normalization process.
			int fieldMask = YEAR_MASK;

			if (Stamp[ERA] != UNSET)
			{
				fieldMask |= ERA_MASK;
			}
			// Find the most recent group of fields specifying the day within
			// the year.  These may be any of the following combinations:
			//   MONTH + DAY_OF_MONTH
			//   MONTH + WEEK_OF_MONTH + DAY_OF_WEEK
			//   MONTH + DAY_OF_WEEK_IN_MONTH + DAY_OF_WEEK
			//   DAY_OF_YEAR
			//   WEEK_OF_YEAR + DAY_OF_WEEK
			// We look for the most recent of the fields in each group to determine
			// the age of the group.  For groups involving a week-related field such
			// as WEEK_OF_MONTH, DAY_OF_WEEK_IN_MONTH, or WEEK_OF_YEAR, both the
			// week-related field and the DAY_OF_WEEK must be set for the group as a
			// whole to be considered.  (See bug 4153860 - liu 7/24/98.)
			int dowStamp = Stamp[DAY_OF_WEEK];
			int monthStamp = Stamp[MONTH];
			int domStamp = Stamp[DAY_OF_MONTH];
			int womStamp = AggregateStamp(Stamp[WEEK_OF_MONTH], dowStamp);
			int dowimStamp = AggregateStamp(Stamp[DAY_OF_WEEK_IN_MONTH], dowStamp);
			int doyStamp = Stamp[DAY_OF_YEAR];
			int woyStamp = AggregateStamp(Stamp[WEEK_OF_YEAR], dowStamp);

			int bestStamp = domStamp;
			if (womStamp > bestStamp)
			{
				bestStamp = womStamp;
			}
			if (dowimStamp > bestStamp)
			{
				bestStamp = dowimStamp;
			}
			if (doyStamp > bestStamp)
			{
				bestStamp = doyStamp;
			}
			if (woyStamp > bestStamp)
			{
				bestStamp = woyStamp;
			}

			/* No complete combination exists.  Look for WEEK_OF_MONTH,
			 * DAY_OF_WEEK_IN_MONTH, or WEEK_OF_YEAR alone.  Treat DAY_OF_WEEK alone
			 * as DAY_OF_WEEK_IN_MONTH.
			 */
			if (bestStamp == UNSET)
			{
				womStamp = Stamp[WEEK_OF_MONTH];
				dowimStamp = System.Math.Max(Stamp[DAY_OF_WEEK_IN_MONTH], dowStamp);
				woyStamp = Stamp[WEEK_OF_YEAR];
				bestStamp = System.Math.Max(System.Math.Max(womStamp, dowimStamp), woyStamp);

				/* Treat MONTH alone or no fields at all as DAY_OF_MONTH.  This may
				 * result in bestStamp = domStamp = UNSET if no fields are set,
				 * which indicates DAY_OF_MONTH.
				 */
				if (bestStamp == UNSET)
				{
					bestStamp = domStamp = monthStamp;
				}
			}

			if (bestStamp == domStamp || (bestStamp == womStamp && Stamp[WEEK_OF_MONTH] >= Stamp[WEEK_OF_YEAR]) || (bestStamp == dowimStamp && Stamp[DAY_OF_WEEK_IN_MONTH] >= Stamp[WEEK_OF_YEAR]))
			{
				fieldMask |= MONTH_MASK;
				if (bestStamp == domStamp)
				{
					fieldMask |= DAY_OF_MONTH_MASK;
				}
				else
				{
					assert(bestStamp == womStamp || bestStamp == dowimStamp);
					if (dowStamp != UNSET)
					{
						fieldMask |= DAY_OF_WEEK_MASK;
					}
					if (womStamp == dowimStamp)
					{
						// When they are equal, give the priority to
						// WEEK_OF_MONTH for compatibility.
						if (Stamp[WEEK_OF_MONTH] >= Stamp[DAY_OF_WEEK_IN_MONTH])
						{
							fieldMask |= WEEK_OF_MONTH_MASK;
						}
						else
						{
							fieldMask |= DAY_OF_WEEK_IN_MONTH_MASK;
						}
					}
					else
					{
						if (bestStamp == womStamp)
						{
							fieldMask |= WEEK_OF_MONTH_MASK;
						}
						else
						{
							assert(bestStamp == dowimStamp);
							if (Stamp[DAY_OF_WEEK_IN_MONTH] != UNSET)
							{
								fieldMask |= DAY_OF_WEEK_IN_MONTH_MASK;
							}
						}
					}
				}
			}
			else
			{
				assert(bestStamp == doyStamp || bestStamp == woyStamp || bestStamp == UNSET);
				if (bestStamp == doyStamp)
				{
					fieldMask |= DAY_OF_YEAR_MASK;
				}
				else
				{
					assert(bestStamp == woyStamp);
					if (dowStamp != UNSET)
					{
						fieldMask |= DAY_OF_WEEK_MASK;
					}
					fieldMask |= WEEK_OF_YEAR_MASK;
				}
			}

			// Find the best set of fields specifying the time of day.  There
			// are only two possibilities here; the HOUR_OF_DAY or the
			// AM_PM and the HOUR.
			int hourOfDayStamp = Stamp[HOUR_OF_DAY];
			int hourStamp = AggregateStamp(Stamp[HOUR], Stamp[AM_PM]);
			bestStamp = (hourStamp > hourOfDayStamp) ? hourStamp : hourOfDayStamp;

			// if bestStamp is still UNSET, then take HOUR or AM_PM. (See 4846659)
			if (bestStamp == UNSET)
			{
				bestStamp = System.Math.Max(Stamp[HOUR], Stamp[AM_PM]);
			}

			// Hours
			if (bestStamp != UNSET)
			{
				if (bestStamp == hourOfDayStamp)
				{
					fieldMask |= HOUR_OF_DAY_MASK;
				}
				else
				{
					fieldMask |= HOUR_MASK;
					if (Stamp[AM_PM] != UNSET)
					{
						fieldMask |= AM_PM_MASK;
					}
				}
			}
			if (Stamp[MINUTE] != UNSET)
			{
				fieldMask |= MINUTE_MASK;
			}
			if (Stamp[SECOND] != UNSET)
			{
				fieldMask |= SECOND_MASK;
			}
			if (Stamp[MILLISECOND] != UNSET)
			{
				fieldMask |= MILLISECOND_MASK;
			}
			if (Stamp[ZONE_OFFSET] >= MINIMUM_USER_STAMP)
			{
					fieldMask |= ZONE_OFFSET_MASK;
			}
			if (Stamp[DST_OFFSET] >= MINIMUM_USER_STAMP)
			{
				fieldMask |= DST_OFFSET_MASK;
			}

			return fieldMask;
		}

		internal virtual int GetBaseStyle(int style)
		{
			return style & ~STANDALONE_MASK;
		}

		private int ToStandaloneStyle(int style)
		{
			return style | STANDALONE_MASK;
		}

		private bool IsStandaloneStyle(int style)
		{
			return (style & STANDALONE_MASK) != 0;
		}

		private bool IsNarrowStyle(int style)
		{
			return style == NARROW_FORMAT || style == NARROW_STANDALONE;
		}

		private bool IsNarrowFormatStyle(int style)
		{
			return style == NARROW_FORMAT;
		}

		/// <summary>
		/// Returns the pseudo-time-stamp for two fields, given their
		/// individual pseudo-time-stamps.  If either of the fields
		/// is unset, then the aggregate is unset.  Otherwise, the
		/// aggregate is the later of the two stamps.
		/// </summary>
		private static int AggregateStamp(int stamp_a, int stamp_b)
		{
			if (stamp_a == UNSET || stamp_b == UNSET)
			{
				return UNSET;
			}
			return (stamp_a > stamp_b) ? stamp_a : stamp_b;
		}

		/// <summary>
		/// Returns an unmodifiable {@code Set} containing all calendar types
		/// supported by {@code Calendar} in the runtime environment. The available
		/// calendar types can be used for the <a
		/// href="Locale.html#def_locale_extension">Unicode locale extensions</a>.
		/// The {@code Set} returned contains at least {@code "gregory"}. The
		/// calendar types don't include aliases, such as {@code "gregorian"} for
		/// {@code "gregory"}.
		/// </summary>
		/// <returns> an unmodifiable {@code Set} containing all available calendar types
		/// @since 1.8 </returns>
		/// <seealso cref= #getCalendarType() </seealso>
		/// <seealso cref= Calendar.Builder#setCalendarType(String) </seealso>
		/// <seealso cref= Locale#getUnicodeLocaleType(String) </seealso>
		public static Set<String> AvailableCalendarTypes
		{
			get
			{
				return AvailableCalendarTypes.SET;
			}
		}

		private class AvailableCalendarTypes
		{
			internal static readonly Set<String> SET;
			static AvailableCalendarTypes()
			{
				Set<String> set = new HashSet<String>(3);
				set.Add("gregory");
				set.Add("buddhist");
				set.Add("japanese");
				SET = Collections.UnmodifiableSet(set);
			}
			internal AvailableCalendarTypes()
			{
			}
		}

		/// <summary>
		/// Returns the calendar type of this {@code Calendar}. Calendar types are
		/// defined by the <em>Unicode Locale Data Markup Language (LDML)</em>
		/// specification.
		/// 
		/// <para>The default implementation of this method returns the class name of
		/// this {@code Calendar} instance. Any subclasses that implement
		/// LDML-defined calendar systems should override this method to return
		/// appropriate calendar types.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the LDML-defined calendar type or the class name of this
		///         {@code Calendar} instance
		/// @since 1.8 </returns>
		/// <seealso cref= <a href="Locale.html#def_extensions">Locale extensions</a> </seealso>
		/// <seealso cref= Locale.Builder#setLocale(Locale) </seealso>
		/// <seealso cref= Locale.Builder#setUnicodeLocaleKeyword(String, String) </seealso>
		public virtual String CalendarType
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName;
			}
		}

		/// <summary>
		/// Compares this <code>Calendar</code> to the specified
		/// <code>Object</code>.  The result is <code>true</code> if and only if
		/// the argument is a <code>Calendar</code> object of the same calendar
		/// system that represents the same time value (millisecond offset from the
		/// <a href="#Epoch">Epoch</a>) under the same
		/// <code>Calendar</code> parameters as this object.
		/// 
		/// <para>The <code>Calendar</code> parameters are the values represented
		/// by the <code>isLenient</code>, <code>getFirstDayOfWeek</code>,
		/// <code>getMinimalDaysInFirstWeek</code> and <code>getTimeZone</code>
		/// methods. If there is any difference in those parameters
		/// between the two <code>Calendar</code>s, this method returns
		/// <code>false</code>.
		/// 
		/// </para>
		/// <para>Use the <seealso cref="#compareTo(Calendar) compareTo"/> method to
		/// compare only the time values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object to compare with. </param>
		/// <returns> <code>true</code> if this object is equal to <code>obj</code>;
		/// <code>false</code> otherwise. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("EqualsWhichDoesntCheckParameterClass") @Override public boolean equals(Object obj)
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			try
			{
				Calendar that = (Calendar)obj;
				return CompareTo(GetMillisOf(that)) == 0 && Lenient_Renamed == that.Lenient_Renamed && FirstDayOfWeek_Renamed == that.FirstDayOfWeek_Renamed && MinimalDaysInFirstWeek_Renamed == that.MinimalDaysInFirstWeek_Renamed && Zone_Renamed.Equals(that.Zone_Renamed);
			}
			catch (Exception)
			{
				// Note: GregorianCalendar.computeTime throws
				// IllegalArgumentException if the ERA value is invalid
				// even it's in lenient mode.
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this calendar.
		/// </summary>
		/// <returns> a hash code value for this object.
		/// @since 1.2 </returns>
		public override int HashCode()
		{
			// 'otheritems' represents the hash code for the previous versions.
			int otheritems = (Lenient_Renamed ? 1 : 0) | (FirstDayOfWeek_Renamed << 1) | (MinimalDaysInFirstWeek_Renamed << 4) | (Zone_Renamed.HashCode() << 7);
			long t = GetMillisOf(this);
			return (int) t ^ (int)(t >> 32) ^ otheritems;
		}

		/// <summary>
		/// Returns whether this <code>Calendar</code> represents a time
		/// before the time represented by the specified
		/// <code>Object</code>. This method is equivalent to:
		/// <pre>{@code
		///         compareTo(when) < 0
		/// }</pre>
		/// if and only if <code>when</code> is a <code>Calendar</code>
		/// instance. Otherwise, the method returns <code>false</code>.
		/// </summary>
		/// <param name="when"> the <code>Object</code> to be compared </param>
		/// <returns> <code>true</code> if the time of this
		/// <code>Calendar</code> is before the time represented by
		/// <code>when</code>; <code>false</code> otherwise. </returns>
		/// <seealso cref=     #compareTo(Calendar) </seealso>
		public virtual bool Before(Object when)
		{
			return when is Calendar && CompareTo((Calendar)when) < 0;
		}

		/// <summary>
		/// Returns whether this <code>Calendar</code> represents a time
		/// after the time represented by the specified
		/// <code>Object</code>. This method is equivalent to:
		/// <pre>{@code
		///         compareTo(when) > 0
		/// }</pre>
		/// if and only if <code>when</code> is a <code>Calendar</code>
		/// instance. Otherwise, the method returns <code>false</code>.
		/// </summary>
		/// <param name="when"> the <code>Object</code> to be compared </param>
		/// <returns> <code>true</code> if the time of this <code>Calendar</code> is
		/// after the time represented by <code>when</code>; <code>false</code>
		/// otherwise. </returns>
		/// <seealso cref=     #compareTo(Calendar) </seealso>
		public virtual bool After(Object when)
		{
			return when is Calendar && CompareTo((Calendar)when) > 0;
		}

		/// <summary>
		/// Compares the time values (millisecond offsets from the <a
		/// href="#Epoch">Epoch</a>) represented by two
		/// <code>Calendar</code> objects.
		/// </summary>
		/// <param name="anotherCalendar"> the <code>Calendar</code> to be compared. </param>
		/// <returns> the value <code>0</code> if the time represented by the argument
		/// is equal to the time represented by this <code>Calendar</code>; a value
		/// less than <code>0</code> if the time of this <code>Calendar</code> is
		/// before the time represented by the argument; and a value greater than
		/// <code>0</code> if the time of this <code>Calendar</code> is after the
		/// time represented by the argument. </returns>
		/// <exception cref="NullPointerException"> if the specified <code>Calendar</code> is
		///            <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the time value of the
		/// specified <code>Calendar</code> object can't be obtained due to
		/// any invalid calendar values.
		/// @since   1.5 </exception>
		public virtual int CompareTo(Calendar anotherCalendar)
		{
			return CompareTo(GetMillisOf(anotherCalendar));
		}

		/// <summary>
		/// Adds or subtracts the specified amount of time to the given calendar field,
		/// based on the calendar's rules. For example, to subtract 5 days from
		/// the current time of the calendar, you can achieve it by calling:
		/// <para><code>add(Calendar.DAY_OF_MONTH, -5)</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <param name="amount"> the amount of date or time to be added to the field. </param>
		/// <seealso cref= #roll(int,int) </seealso>
		/// <seealso cref= #set(int,int) </seealso>
		public abstract void Add(int field, int amount);

		/// <summary>
		/// Adds or subtracts (up/down) a single unit of time on the given time
		/// field without changing larger fields. For example, to roll the current
		/// date up by one day, you can achieve it by calling:
		/// <para>roll(Calendar.DATE, true).
		/// When rolling on the year or Calendar.YEAR field, it will roll the year
		/// value in the range between 1 and the value returned by calling
		/// <code>getMaximum(Calendar.YEAR)</code>.
		/// When rolling on the month or Calendar.MONTH field, other fields like
		/// date might conflict and, need to be changed. For instance,
		/// rolling the month on the date 01/31/96 will result in 02/29/96.
		/// When rolling on the hour-in-day or Calendar.HOUR_OF_DAY field, it will
		/// roll the hour value in the range between 0 and 23, which is zero-based.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the time field. </param>
		/// <param name="up"> indicates if the value of the specified time field is to be
		/// rolled up or rolled down. Use true if rolling up, false otherwise. </param>
		/// <seealso cref= Calendar#add(int,int) </seealso>
		/// <seealso cref= Calendar#set(int,int) </seealso>
		public abstract void Roll(int field, bool up);

		/// <summary>
		/// Adds the specified (signed) amount to the specified calendar field
		/// without changing larger fields.  A negative amount means to roll
		/// down.
		/// 
		/// <para>NOTE:  This default implementation on <code>Calendar</code> just repeatedly calls the
		/// version of <seealso cref="#roll(int,boolean) roll()"/> that rolls by one unit.  This may not
		/// always do the right thing.  For example, if the <code>DAY_OF_MONTH</code> field is 31,
		/// rolling through February will leave it set to 28.  The <code>GregorianCalendar</code>
		/// version of this function takes care of this problem.  Other subclasses
		/// should also provide overrides of this function that do the right thing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <param name="amount"> the signed amount to add to the calendar <code>field</code>.
		/// @since 1.2 </param>
		/// <seealso cref= #roll(int,boolean) </seealso>
		/// <seealso cref= #add(int,int) </seealso>
		/// <seealso cref= #set(int,int) </seealso>
		public virtual void Roll(int field, int amount)
		{
			while (amount > 0)
			{
				Roll(field, true);
				amount--;
			}
			while (amount < 0)
			{
				Roll(field, false);
				amount++;
			}
		}

		/// <summary>
		/// Sets the time zone with the given time zone value.
		/// </summary>
		/// <param name="value"> the given time zone. </param>
		public virtual TimeZone TimeZone
		{
			set
			{
				Zone_Renamed = value;
				SharedZone = false;
				/* Recompute the fields from the time using the new zone.  This also
				 * works if isTimeSet is false (after a call to set()).  In that case
				 * the time will be computed from the fields using the new zone, then
				 * the fields will get recomputed from that.  Consider the sequence of
				 * calls: cal.setTimeZone(EST); cal.set(HOUR, 1); cal.setTimeZone(PST).
				 * Is cal set to 1 o'clock EST or 1 o'clock PST?  Answer: PST.  More
				 * generally, a call to setTimeZone() affects calls to set() BEFORE AND
				 * AFTER it up to the next call to complete().
				 */
				AreAllFieldsSet = AreFieldsSet = false;
			}
			get
			{
				// If the TimeZone object is shared by other Calendar instances, then
				// create a clone.
				if (SharedZone)
				{
					Zone_Renamed = (TimeZone) Zone_Renamed.Clone();
					SharedZone = false;
				}
				return Zone_Renamed;
			}
		}


		/// <summary>
		/// Returns the time zone (without cloning).
		/// </summary>
		internal virtual TimeZone Zone
		{
			get
			{
				return Zone_Renamed;
			}
		}

		/// <summary>
		/// Sets the sharedZone flag to <code>shared</code>.
		/// </summary>
		internal virtual bool ZoneShared
		{
			set
			{
				SharedZone = value;
			}
		}

		/// <summary>
		/// Specifies whether or not date/time interpretation is to be lenient.  With
		/// lenient interpretation, a date such as "February 942, 1996" will be
		/// treated as being equivalent to the 941st day after February 1, 1996.
		/// With strict (non-lenient) interpretation, such dates will cause an exception to be
		/// thrown. The default is lenient.
		/// </summary>
		/// <param name="lenient"> <code>true</code> if the lenient mode is to be turned
		/// on; <code>false</code> if it is to be turned off. </param>
		/// <seealso cref= #isLenient() </seealso>
		/// <seealso cref= java.text.DateFormat#setLenient </seealso>
		public virtual bool Lenient
		{
			set
			{
				this.Lenient_Renamed = value;
			}
			get
			{
				return Lenient_Renamed;
			}
		}


		/// <summary>
		/// Sets what the first day of the week is; e.g., <code>SUNDAY</code> in the U.S.,
		/// <code>MONDAY</code> in France.
		/// </summary>
		/// <param name="value"> the given first day of the week. </param>
		/// <seealso cref= #getFirstDayOfWeek() </seealso>
		/// <seealso cref= #getMinimalDaysInFirstWeek() </seealso>
		public virtual int FirstDayOfWeek
		{
			set
			{
				if (FirstDayOfWeek_Renamed == value)
				{
					return;
				}
				FirstDayOfWeek_Renamed = value;
				InvalidateWeekFields();
			}
			get
			{
				return FirstDayOfWeek_Renamed;
			}
		}


		/// <summary>
		/// Sets what the minimal days required in the first week of the year are;
		/// For example, if the first week is defined as one that contains the first
		/// day of the first month of a year, call this method with value 1. If it
		/// must be a full week, use value 7.
		/// </summary>
		/// <param name="value"> the given minimal days required in the first week
		/// of the year. </param>
		/// <seealso cref= #getMinimalDaysInFirstWeek() </seealso>
		public virtual int MinimalDaysInFirstWeek
		{
			set
			{
				if (MinimalDaysInFirstWeek_Renamed == value)
				{
					return;
				}
				MinimalDaysInFirstWeek_Renamed = value;
				InvalidateWeekFields();
			}
			get
			{
				return MinimalDaysInFirstWeek_Renamed;
			}
		}


		/// <summary>
		/// Returns whether this {@code Calendar} supports week dates.
		/// 
		/// <para>The default implementation of this method returns {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if this {@code Calendar} supports week dates;
		///         {@code false} otherwise. </returns>
		/// <seealso cref= #getWeekYear() </seealso>
		/// <seealso cref= #setWeekDate(int,int,int) </seealso>
		/// <seealso cref= #getWeeksInWeekYear()
		/// @since 1.7 </seealso>
		public virtual bool WeekDateSupported
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the week year represented by this {@code Calendar}. The
		/// week year is in sync with the week cycle. The {@linkplain
		/// #getFirstDayOfWeek() first day of the first week} is the first
		/// day of the week year.
		/// 
		/// <para>The default implementation of this method throws an
		/// <seealso cref="UnsupportedOperationException"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the week year of this {@code Calendar} </returns>
		/// <exception cref="UnsupportedOperationException">
		///            if any week year numbering isn't supported
		///            in this {@code Calendar}. </exception>
		/// <seealso cref= #isWeekDateSupported() </seealso>
		/// <seealso cref= #getFirstDayOfWeek() </seealso>
		/// <seealso cref= #getMinimalDaysInFirstWeek()
		/// @since 1.7 </seealso>
		public virtual int WeekYear
		{
			get
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Sets the date of this {@code Calendar} with the the given date
		/// specifiers - week year, week of year, and day of week.
		/// 
		/// <para>Unlike the {@code set} method, all of the calendar fields
		/// and {@code time} values are calculated upon return.
		/// 
		/// </para>
		/// <para>If {@code weekOfYear} is out of the valid week-of-year range
		/// in {@code weekYear}, the {@code weekYear} and {@code
		/// weekOfYear} values are adjusted in lenient mode, or an {@code
		/// IllegalArgumentException} is thrown in non-lenient mode.
		/// 
		/// </para>
		/// <para>The default implementation of this method throws an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weekYear">   the week year </param>
		/// <param name="weekOfYear"> the week number based on {@code weekYear} </param>
		/// <param name="dayOfWeek">  the day of week value: one of the constants
		///                   for the <seealso cref="#DAY_OF_WEEK"/> field: {@link
		///                   #SUNDAY}, ..., <seealso cref="#SATURDAY"/>. </param>
		/// <exception cref="IllegalArgumentException">
		///            if any of the given date specifiers is invalid
		///            or any of the calendar fields are inconsistent
		///            with the given date specifiers in non-lenient mode </exception>
		/// <exception cref="UnsupportedOperationException">
		///            if any week year numbering isn't supported in this
		///            {@code Calendar}. </exception>
		/// <seealso cref= #isWeekDateSupported() </seealso>
		/// <seealso cref= #getFirstDayOfWeek() </seealso>
		/// <seealso cref= #getMinimalDaysInFirstWeek()
		/// @since 1.7 </seealso>
		public virtual void SetWeekDate(int weekYear, int weekOfYear, int dayOfWeek)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Returns the number of weeks in the week year represented by this
		/// {@code Calendar}.
		/// 
		/// <para>The default implementation of this method throws an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of weeks in the week year. </returns>
		/// <exception cref="UnsupportedOperationException">
		///            if any week year numbering isn't supported in this
		///            {@code Calendar}. </exception>
		/// <seealso cref= #WEEK_OF_YEAR </seealso>
		/// <seealso cref= #isWeekDateSupported() </seealso>
		/// <seealso cref= #getWeekYear() </seealso>
		/// <seealso cref= #getActualMaximum(int)
		/// @since 1.7 </seealso>
		public virtual int WeeksInWeekYear
		{
			get
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Returns the minimum value for the given calendar field of this
		/// <code>Calendar</code> instance. The minimum value is defined as
		/// the smallest value returned by the <seealso cref="#get(int) get"/> method
		/// for any possible time value.  The minimum value depends on
		/// calendar system specific parameters of the instance.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the minimum value for the given calendar field. </returns>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public abstract int GetMinimum(int field);

		/// <summary>
		/// Returns the maximum value for the given calendar field of this
		/// <code>Calendar</code> instance. The maximum value is defined as
		/// the largest value returned by the <seealso cref="#get(int) get"/> method
		/// for any possible time value. The maximum value depends on
		/// calendar system specific parameters of the instance.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the maximum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public abstract int GetMaximum(int field);

		/// <summary>
		/// Returns the highest minimum value for the given calendar field
		/// of this <code>Calendar</code> instance. The highest minimum
		/// value is defined as the largest value returned by {@link
		/// #getActualMinimum(int)} for any possible time value. The
		/// greatest minimum value depends on calendar system specific
		/// parameters of the instance.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the highest minimum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public abstract int GetGreatestMinimum(int field);

		/// <summary>
		/// Returns the lowest maximum value for the given calendar field
		/// of this <code>Calendar</code> instance. The lowest maximum
		/// value is defined as the smallest value returned by {@link
		/// #getActualMaximum(int)} for any possible time value. The least
		/// maximum value depends on calendar system specific parameters of
		/// the instance. For example, a <code>Calendar</code> for the
		/// Gregorian calendar system returns 28 for the
		/// <code>DAY_OF_MONTH</code> field, because the 28th is the last
		/// day of the shortest month of this calendar, February in a
		/// common year.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the lowest maximum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public abstract int GetLeastMaximum(int field);

		/// <summary>
		/// Returns the minimum value that the specified calendar field
		/// could have, given the time value of this <code>Calendar</code>.
		/// 
		/// <para>The default implementation of this method uses an iterative
		/// algorithm to determine the actual minimum value for the
		/// calendar field. Subclasses should, if possible, override this
		/// with a more efficient implementation - in many cases, they can
		/// simply return <code>getMinimum()</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the minimum of the given calendar field for the time
		/// value of this <code>Calendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int)
		/// @since 1.2 </seealso>
		public virtual int GetActualMinimum(int field)
		{
			int fieldValue = GetGreatestMinimum(field);
			int endValue = GetMinimum(field);

			// if we know that the minimum value is always the same, just return it
			if (fieldValue == endValue)
			{
				return fieldValue;
			}

			// clone the calendar so we don't mess with the real one, and set it to
			// accept anything for the field values
			Calendar work = (Calendar)this.Clone();
			work.Lenient = true;

			// now try each value from getLeastMaximum() to getMaximum() one by one until
			// we get a value that normalizes to another value.  The last value that
			// normalizes to itself is the actual minimum for the current date
			int result = fieldValue;

			do
			{
				work.Set(field, fieldValue);
				if (work.Get(field) != fieldValue)
				{
					break;
				}
				else
				{
					result = fieldValue;
					fieldValue--;
				}
			} while (fieldValue >= endValue);

			return result;
		}

		/// <summary>
		/// Returns the maximum value that the specified calendar field
		/// could have, given the time value of this
		/// <code>Calendar</code>. For example, the actual maximum value of
		/// the <code>MONTH</code> field is 12 in some years, and 13 in
		/// other years in the Hebrew calendar system.
		/// 
		/// <para>The default implementation of this method uses an iterative
		/// algorithm to determine the actual maximum value for the
		/// calendar field. Subclasses should, if possible, override this
		/// with a more efficient implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the maximum of the given calendar field for the time
		/// value of this <code>Calendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int)
		/// @since 1.2 </seealso>
		public virtual int GetActualMaximum(int field)
		{
			int fieldValue = GetLeastMaximum(field);
			int endValue = GetMaximum(field);

			// if we know that the maximum value is always the same, just return it.
			if (fieldValue == endValue)
			{
				return fieldValue;
			}

			// clone the calendar so we don't mess with the real one, and set it to
			// accept anything for the field values.
			Calendar work = (Calendar)this.Clone();
			work.Lenient = true;

			// if we're counting weeks, set the day of the week to Sunday.  We know the
			// last week of a month or year will contain the first day of the week.
			if (field == WEEK_OF_YEAR || field == WEEK_OF_MONTH)
			{
				work.Set(DAY_OF_WEEK, FirstDayOfWeek_Renamed);
			}

			// now try each value from getLeastMaximum() to getMaximum() one by one until
			// we get a value that normalizes to another value.  The last value that
			// normalizes to itself is the actual maximum for the current date
			int result = fieldValue;

			do
			{
				work.Set(field, fieldValue);
				if (work.Get(field) != fieldValue)
				{
					break;
				}
				else
				{
					result = fieldValue;
					fieldValue++;
				}
			} while (fieldValue <= endValue);

			return result;
		}

		/// <summary>
		/// Creates and returns a copy of this object.
		/// </summary>
		/// <returns> a copy of this object. </returns>
		public override Object Clone()
		{
			try
			{
				Calendar other = (Calendar) base.Clone();

				other.Fields = new int[FIELD_COUNT];
				other.IsSet_Renamed = new bool[FIELD_COUNT];
				other.Stamp = new int[FIELD_COUNT];
				for (int i = 0; i < FIELD_COUNT; i++)
				{
					other.Fields[i] = Fields[i];
					other.Stamp[i] = Stamp[i];
					other.IsSet_Renamed[i] = IsSet_Renamed[i];
				}
				other.Zone_Renamed = (TimeZone) Zone_Renamed.Clone();
				return other;
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}

		private static readonly String[] FIELD_NAME = new String[] {"ERA", "YEAR", "MONTH", "WEEK_OF_YEAR", "WEEK_OF_MONTH", "DAY_OF_MONTH", "DAY_OF_YEAR", "DAY_OF_WEEK", "DAY_OF_WEEK_IN_MONTH", "AM_PM", "HOUR", "HOUR_OF_DAY", "MINUTE", "SECOND", "MILLISECOND", "ZONE_OFFSET", "DST_OFFSET"};

		/// <summary>
		/// Returns the name of the specified calendar field.
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the calendar field name </returns>
		/// <exception cref="IndexOutOfBoundsException"> if <code>field</code> is negative,
		/// equal to or greater then <code>FIELD_COUNT</code>. </exception>
		internal static String GetFieldName(int field)
		{
			return FIELD_NAME[field];
		}

		/// <summary>
		/// Return a string representation of this calendar. This method
		/// is intended to be used only for debugging purposes, and the
		/// format of the returned string may vary between implementations.
		/// The returned string may be empty but may not be <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this calendar. </returns>
		public override String ToString()
		{
			// NOTE: BuddhistCalendar.toString() interprets the string
			// produced by this method so that the Gregorian year number
			// is substituted by its B.E. year value. It relies on
			// "...,YEAR=<year>,..." or "...,YEAR=?,...".
			StringBuilder buffer = new StringBuilder(800);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			buffer.Append(this.GetType().FullName).Append('[');
			AppendValue(buffer, "time", IsTimeSet, Time_Renamed);
			buffer.Append(",areFieldsSet=").Append(AreFieldsSet);
			buffer.Append(",areAllFieldsSet=").Append(AreAllFieldsSet);
			buffer.Append(",lenient=").Append(Lenient_Renamed);
			buffer.Append(",zone=").Append(Zone_Renamed);
			AppendValue(buffer, ",firstDayOfWeek", true, (long) FirstDayOfWeek_Renamed);
			AppendValue(buffer, ",minimalDaysInFirstWeek", true, (long) MinimalDaysInFirstWeek_Renamed);
			for (int i = 0; i < FIELD_COUNT; ++i)
			{
				buffer.Append(',');
				AppendValue(buffer, FIELD_NAME[i], IsSet(i), (long) Fields[i]);
			}
			buffer.Append(']');
			return buffer.ToString();
		}

		// =======================privates===============================

		private static void AppendValue(StringBuilder sb, String item, bool valid, long value)
		{
			sb.Append(item).Append('=');
			if (valid)
			{
				sb.Append(value);
			}
			else
			{
				sb.Append('?');
			}
		}

		/// <summary>
		/// Both firstDayOfWeek and minimalDaysInFirstWeek are locale-dependent.
		/// They are used to figure out the week count for a specific date for
		/// a given locale. These must be set when a Calendar is constructed. </summary>
		/// <param name="desiredLocale"> the given locale. </param>
		private Locale WeekCountData
		{
			set
			{
				/* try to get the Locale data from the cache */
				int[] data = CachedLocaleData[value];
				if (data == null) // cache miss
				{
					data = new int[2];
					data[0] = CalendarDataUtility.retrieveFirstDayOfWeek(value);
					data[1] = CalendarDataUtility.retrieveMinimalDaysInFirstWeek(value);
					CachedLocaleData.PutIfAbsent(value, data);
				}
				FirstDayOfWeek_Renamed = data[0];
				MinimalDaysInFirstWeek_Renamed = data[1];
			}
		}

		/// <summary>
		/// Recomputes the time and updates the status fields isTimeSet
		/// and areFieldsSet.  Callers should check isTimeSet and only
		/// call this method if isTimeSet is false.
		/// </summary>
		private void UpdateTime()
		{
			ComputeTime();
			// The areFieldsSet and areAllFieldsSet values are no longer
			// controlled here (as of 1.5).
			IsTimeSet = true;
		}

		private int CompareTo(long t)
		{
			long thisTime = GetMillisOf(this);
			return (thisTime > t) ? 1 : (thisTime == t) ? 0 : -1;
		}

		private static long GetMillisOf(Calendar calendar)
		{
			if (calendar.IsTimeSet)
			{
				return calendar.Time_Renamed;
			}
			Calendar cal = (Calendar) calendar.Clone();
			cal.Lenient = true;
			return cal.TimeInMillis;
		}

		/// <summary>
		/// Adjusts the stamp[] values before nextStamp overflow. nextStamp
		/// is set to the next stamp value upon the return.
		/// </summary>
		private void AdjustStamp()
		{
			int max = MINIMUM_USER_STAMP;
			int newStamp = MINIMUM_USER_STAMP;

			for (;;)
			{
				int min = Integer.MaxValue;
				for (int i = 0; i < Stamp.Length; i++)
				{
					int v = Stamp[i];
					if (v >= newStamp && min > v)
					{
						min = v;
					}
					if (max < v)
					{
						max = v;
					}
				}
				if (max != min && min == Integer.MaxValue)
				{
					break;
				}
				for (int i = 0; i < Stamp.Length; i++)
				{
					if (Stamp[i] == min)
					{
						Stamp[i] = newStamp;
					}
				}
				newStamp++;
				if (min == max)
				{
					break;
				}
			}
			NextStamp = newStamp;
		}

		/// <summary>
		/// Sets the WEEK_OF_MONTH and WEEK_OF_YEAR fields to new values with the
		/// new parameter value if they have been calculated internally.
		/// </summary>
		private void InvalidateWeekFields()
		{
			if (Stamp[WEEK_OF_MONTH] != COMPUTED && Stamp[WEEK_OF_YEAR] != COMPUTED)
			{
				return;
			}

			// We have to check the new values of these fields after changing
			// firstDayOfWeek and/or minimalDaysInFirstWeek. If the field values
			// have been changed, then set the new values. (4822110)
			Calendar cal = (Calendar) Clone();
			cal.Lenient = true;
			cal.Clear(WEEK_OF_MONTH);
			cal.Clear(WEEK_OF_YEAR);

			if (Stamp[WEEK_OF_MONTH] == COMPUTED)
			{
				int weekOfMonth = cal.Get(WEEK_OF_MONTH);
				if (Fields[WEEK_OF_MONTH] != weekOfMonth)
				{
					Fields[WEEK_OF_MONTH] = weekOfMonth;
				}
			}

			if (Stamp[WEEK_OF_YEAR] == COMPUTED)
			{
				int weekOfYear = cal.Get(WEEK_OF_YEAR);
				if (Fields[WEEK_OF_YEAR] != weekOfYear)
				{
					Fields[WEEK_OF_YEAR] = weekOfYear;
				}
			}
		}

		/// <summary>
		/// Save the state of this object to a stream (i.e., serialize it).
		/// 
		/// Ideally, <code>Calendar</code> would only write out its state data and
		/// the current time, and not write any field data out, such as
		/// <code>fields[]</code>, <code>isTimeSet</code>, <code>areFieldsSet</code>,
		/// and <code>isSet[]</code>.  <code>nextStamp</code> also should not be part
		/// of the persistent state. Unfortunately, this didn't happen before JDK 1.1
		/// shipped. To be compatible with JDK 1.1, we will always have to write out
		/// the field values and state flags.  However, <code>nextStamp</code> can be
		/// removed from the serialization stream; this will probably happen in the
		/// near future.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		private void WriteObject(ObjectOutputStream stream)
		{
			lock (this)
			{
				// Try to compute the time correctly, for the future (stream
				// version 2) in which we don't write out fields[] or isSet[].
				if (!IsTimeSet)
				{
					try
					{
						UpdateTime();
					}
					catch (IllegalArgumentException)
					{
					}
				}
        
				// If this Calendar has a ZoneInfo, save it and set a
				// SimpleTimeZone equivalent (as a single DST schedule) for
				// backward compatibility.
				TimeZone savedZone = null;
				if (Zone_Renamed is ZoneInfo)
				{
					SimpleTimeZone stz = ((ZoneInfo)Zone_Renamed).LastRuleInstance;
					if (stz == null)
					{
						stz = new SimpleTimeZone(Zone_Renamed.RawOffset, Zone_Renamed.ID);
					}
					savedZone = Zone_Renamed;
					Zone_Renamed = stz;
				}
        
				// Write out the 1.1 FCS object.
				stream.DefaultWriteObject();
        
				// Write out the ZoneInfo object
				// 4802409: we write out even if it is null, a temporary workaround
				// the real fix for bug 4844924 in corba-iiop
				stream.WriteObject(savedZone);
				if (savedZone != null)
				{
					Zone_Renamed = savedZone;
				}
			}
		}

		private class CalendarAccessControlContext
		{
			internal static readonly AccessControlContext INSTANCE;
			static CalendarAccessControlContext()
			{
				RuntimePermission perm = new RuntimePermission("accessClassInPackage.sun.util.calendar");
				PermissionCollection perms = perm.NewPermissionCollection();
				perms.Add(perm);
				INSTANCE = new AccessControlContext(new ProtectionDomain[] { new ProtectionDomain(null, perms)
			});
		}
			internal CalendarAccessControlContext()
			{
			}
	}

		/// <summary>
		/// Reconstitutes this object from a stream (i.e., deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.ObjectInputStream input = stream;
			ObjectInputStream input = stream;
			input.DefaultReadObject();

			Stamp = new int[FIELD_COUNT];

			// Starting with version 2 (not implemented yet), we expect that
			// fields[], isSet[], isTimeSet, and areFieldsSet may not be
			// streamed out anymore.  We expect 'time' to be correct.
			if (SerialVersionOnStream >= 2)
			{
				IsTimeSet = true;
				if (Fields == null)
				{
					Fields = new int[FIELD_COUNT];
				}
				if (IsSet_Renamed == null)
				{
					IsSet_Renamed = new bool[FIELD_COUNT];
				}
			}
			else if (SerialVersionOnStream >= 0)
			{
				for (int i = 0; i < FIELD_COUNT; ++i)
				{
					Stamp[i] = IsSet_Renamed[i] ? COMPUTED : UNSET;
				}
			}

			SerialVersionOnStream = CurrentSerialVersion;

			// If there's a ZoneInfo object, use it for zone.
			ZoneInfo zi = null;
			try
			{
				zi = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, input),
						CalendarAccessControlContext.INSTANCE);
			}
			catch (PrivilegedActionException pae)
			{
				Exception e = pae.Exception;
				if (!(e is OptionalDataException))
				{
					if (e is RuntimeException)
					{
						throw (RuntimeException) e;
					}
					else if (e is IOException)
					{
						throw (IOException) e;
					}
					else if (e is ClassNotFoundException)
					{
						throw (ClassNotFoundException) e;
					}
					throw new RuntimeException(e);
				}
			}
			if (zi != null)
			{
				Zone_Renamed = zi;
			}

			// If the deserialized object has a SimpleTimeZone, try to
			// replace it with a ZoneInfo equivalent (as of 1.4) in order
			// to be compatible with the SimpleTimeZone-based
			// implementation as much as possible.
			if (Zone_Renamed is SimpleTimeZone)
			{
				String id = Zone_Renamed.ID;
				TimeZone tz = TimeZone.GetTimeZone(id);
				if (tz != null && tz.HasSameRules(Zone_Renamed) && tz.ID.Equals(id))
				{
					Zone_Renamed = tz;
				}
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<ZoneInfo>
		{
			private readonly Calendar OuterInstance;

			private ObjectInputStream Input;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(Calendar outerInstance, ObjectInputStream input)
			{
				this.OuterInstance = outerInstance;
				this.Input = input;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public sun.util.calendar.ZoneInfo run() throws Exception
			public virtual ZoneInfo Run()
			{
				return (ZoneInfo) Input.ReadObject();
			}
		}

		/// <summary>
		/// Converts this object to an <seealso cref="Instant"/>.
		/// <para>
		/// The conversion creates an {@code Instant} that represents the
		/// same point on the time-line as this {@code Calendar}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the instant representing the same point on the time-line
		/// @since 1.8 </returns>
		public Instant ToInstant()
		{
			return Instant.OfEpochMilli(TimeInMillis);
		}
}

}