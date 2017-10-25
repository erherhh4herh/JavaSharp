using System;
using System.Diagnostics;

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

	using BaseCalendar = sun.util.calendar.BaseCalendar;
	using CalendarDate = sun.util.calendar.CalendarDate;
	using CalendarSystem = sun.util.calendar.CalendarSystem;
	using CalendarUtils = sun.util.calendar.CalendarUtils;
	using Era = sun.util.calendar.Era;
	using Gregorian = sun.util.calendar.Gregorian;
	using JulianCalendar = sun.util.calendar.JulianCalendar;
	using ZoneInfo = sun.util.calendar.ZoneInfo;

	/// <summary>
	/// <code>GregorianCalendar</code> is a concrete subclass of
	/// <code>Calendar</code> and provides the standard calendar system
	/// used by most of the world.
	/// 
	/// <para> <code>GregorianCalendar</code> is a hybrid calendar that
	/// supports both the Julian and Gregorian calendar systems with the
	/// support of a single discontinuity, which corresponds by default to
	/// the Gregorian date when the Gregorian calendar was instituted
	/// (October 15, 1582 in some countries, later in others).  The cutover
	/// date may be changed by the caller by calling {@link
	/// #setGregorianChange(Date) setGregorianChange()}.
	/// 
	/// </para>
	/// <para>
	/// Historically, in those countries which adopted the Gregorian calendar first,
	/// October 4, 1582 (Julian) was thus followed by October 15, 1582 (Gregorian). This calendar models
	/// this correctly.  Before the Gregorian cutover, <code>GregorianCalendar</code>
	/// implements the Julian calendar.  The only difference between the Gregorian
	/// and the Julian calendar is the leap year rule. The Julian calendar specifies
	/// leap years every four years, whereas the Gregorian calendar omits century
	/// years which are not divisible by 400.
	/// 
	/// </para>
	/// <para>
	/// <code>GregorianCalendar</code> implements <em>proleptic</em> Gregorian and
	/// Julian calendars. That is, dates are computed by extrapolating the current
	/// rules indefinitely far backward and forward in time. As a result,
	/// <code>GregorianCalendar</code> may be used for all years to generate
	/// meaningful and consistent results. However, dates obtained using
	/// <code>GregorianCalendar</code> are historically accurate only from March 1, 4
	/// AD onward, when modern Julian calendar rules were adopted.  Before this date,
	/// leap year rules were applied irregularly, and before 45 BC the Julian
	/// calendar did not even exist.
	/// 
	/// </para>
	/// <para>
	/// Prior to the institution of the Gregorian calendar, New Year's Day was
	/// March 25. To avoid confusion, this calendar always uses January 1. A manual
	/// adjustment may be made if desired for dates that are prior to the Gregorian
	/// changeover and which fall between January 1 and March 24.
	/// 
	/// <h3><a name="week_and_year">Week Of Year and Week Year</a></h3>
	/// 
	/// </para>
	/// <para>Values calculated for the {@link Calendar#WEEK_OF_YEAR
	/// WEEK_OF_YEAR} field range from 1 to 53. The first week of a
	/// calendar year is the earliest seven day period starting on {@link
	/// Calendar#getFirstDayOfWeek() getFirstDayOfWeek()} that contains at
	/// least {@link Calendar#getMinimalDaysInFirstWeek()
	/// getMinimalDaysInFirstWeek()} days from that year. It thus depends
	/// on the values of {@code getMinimalDaysInFirstWeek()}, {@code
	/// getFirstDayOfWeek()}, and the day of the week of January 1. Weeks
	/// between week 1 of one year and week 1 of the following year
	/// (exclusive) are numbered sequentially from 2 to 52 or 53 (except
	/// for year(s) involved in the Julian-Gregorian transition).
	/// 
	/// </para>
	/// <para>The {@code getFirstDayOfWeek()} and {@code
	/// getMinimalDaysInFirstWeek()} values are initialized using
	/// locale-dependent resources when constructing a {@code
	/// GregorianCalendar}. <a name="iso8601_compatible_setting">The week
	/// determination is compatible</a> with the ISO 8601 standard when {@code
	/// getFirstDayOfWeek()} is {@code MONDAY} and {@code
	/// getMinimalDaysInFirstWeek()} is 4, which values are used in locales
	/// where the standard is preferred. These values can explicitly be set by
	/// calling <seealso cref="Calendar#setFirstDayOfWeek(int) setFirstDayOfWeek()"/> and
	/// {@link Calendar#setMinimalDaysInFirstWeek(int)
	/// setMinimalDaysInFirstWeek()}.
	/// 
	/// </para>
	/// <para>A <a name="week_year"><em>week year</em></a> is in sync with a
	/// {@code WEEK_OF_YEAR} cycle. All weeks between the first and last
	/// weeks (inclusive) have the same <em>week year</em> value.
	/// Therefore, the first and last days of a week year may have
	/// different calendar year values.
	/// 
	/// </para>
	/// <para>For example, January 1, 1998 is a Thursday. If {@code
	/// getFirstDayOfWeek()} is {@code MONDAY} and {@code
	/// getMinimalDaysInFirstWeek()} is 4 (ISO 8601 standard compatible
	/// setting), then week 1 of 1998 starts on December 29, 1997, and ends
	/// on January 4, 1998. The week year is 1998 for the last three days
	/// of calendar year 1997. If, however, {@code getFirstDayOfWeek()} is
	/// {@code SUNDAY}, then week 1 of 1998 starts on January 4, 1998, and
	/// ends on January 10, 1998; the first three days of 1998 then are
	/// part of week 53 of 1997 and their week year is 1997.
	/// 
	/// <h4>Week Of Month</h4>
	/// 
	/// </para>
	/// <para>Values calculated for the <code>WEEK_OF_MONTH</code> field range from 0
	/// to 6.  Week 1 of a month (the days with <code>WEEK_OF_MONTH =
	/// 1</code>) is the earliest set of at least
	/// <code>getMinimalDaysInFirstWeek()</code> contiguous days in that month,
	/// ending on the day before <code>getFirstDayOfWeek()</code>.  Unlike
	/// week 1 of a year, week 1 of a month may be shorter than 7 days, need
	/// not start on <code>getFirstDayOfWeek()</code>, and will not include days of
	/// the previous month.  Days of a month before week 1 have a
	/// <code>WEEK_OF_MONTH</code> of 0.
	/// 
	/// </para>
	/// <para>For example, if <code>getFirstDayOfWeek()</code> is <code>SUNDAY</code>
	/// and <code>getMinimalDaysInFirstWeek()</code> is 4, then the first week of
	/// January 1998 is Sunday, January 4 through Saturday, January 10.  These days
	/// have a <code>WEEK_OF_MONTH</code> of 1.  Thursday, January 1 through
	/// Saturday, January 3 have a <code>WEEK_OF_MONTH</code> of 0.  If
	/// <code>getMinimalDaysInFirstWeek()</code> is changed to 3, then January 1
	/// through January 3 have a <code>WEEK_OF_MONTH</code> of 1.
	/// 
	/// <h4>Default Fields Values</h4>
	/// 
	/// </para>
	/// <para>The <code>clear</code> method sets calendar field(s)
	/// undefined. <code>GregorianCalendar</code> uses the following
	/// default value for each calendar field if its value is undefined.
	/// 
	/// <table cellpadding="0" cellspacing="3" border="0"
	///        summary="GregorianCalendar default field values"
	///        style="text-align: left; width: 66%;">
	///   <tbody>
	///     <tr>
	///       <th style="vertical-align: top; background-color: rgb(204, 204, 255);
	///           text-align: center;">Field<br>
	///       </th>
	///       <th style="vertical-align: top; background-color: rgb(204, 204, 255);
	///           text-align: center;">Default Value<br>
	///       </th>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle;">
	///              <code>ERA<br></code>
	///       </td>
	///       <td style="vertical-align: middle;">
	///              <code>AD<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	///              <code>YEAR<br></code>
	///       </td>
	///       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	///              <code>1970<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle;">
	///              <code>MONTH<br></code>
	///       </td>
	///       <td style="vertical-align: middle;">
	///              <code>JANUARY<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///              <code>DAY_OF_MONTH<br></code>
	///       </td>
	///       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///              <code>1<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle;">
	///              <code>DAY_OF_WEEK<br></code>
	///       </td>
	///       <td style="vertical-align: middle;">
	///              <code>the first day of week<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///              <code>WEEK_OF_MONTH<br></code>
	///       </td>
	///       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	///              <code>0<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: top;">
	///              <code>DAY_OF_WEEK_IN_MONTH<br></code>
	///       </td>
	///       <td style="vertical-align: top;">
	///              <code>1<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	///              <code>AM_PM<br></code>
	///       </td>
	///       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	///              <code>AM<br></code>
	///       </td>
	///     </tr>
	///     <tr>
	///       <td style="vertical-align: middle;">
	///              <code>HOUR, HOUR_OF_DAY, MINUTE, SECOND, MILLISECOND<br></code>
	///       </td>
	///       <td style="vertical-align: middle;">
	///              <code>0<br></code>
	///       </td>
	///     </tr>
	///   </tbody>
	/// </table>
	/// <br>Default values are not applicable for the fields not listed above.
	/// 
	/// </para>
	/// <para>
	/// <strong>Example:</strong>
	/// <blockquote>
	/// <pre>
	/// // get the supported ids for GMT-08:00 (Pacific Standard Time)
	/// String[] ids = TimeZone.getAvailableIDs(-8 * 60 * 60 * 1000);
	/// // if no ids were returned, something is wrong. get out.
	/// if (ids.length == 0)
	///     System.exit(0);
	/// 
	///  // begin output
	/// System.out.println("Current Time");
	/// 
	/// // create a Pacific Standard Time time zone
	/// SimpleTimeZone pdt = new SimpleTimeZone(-8 * 60 * 60 * 1000, ids[0]);
	/// 
	/// // set up rules for Daylight Saving Time
	/// pdt.setStartRule(Calendar.APRIL, 1, Calendar.SUNDAY, 2 * 60 * 60 * 1000);
	/// pdt.setEndRule(Calendar.OCTOBER, -1, Calendar.SUNDAY, 2 * 60 * 60 * 1000);
	/// 
	/// // create a GregorianCalendar with the Pacific Daylight time zone
	/// // and the current date and time
	/// Calendar calendar = new GregorianCalendar(pdt);
	/// Date trialTime = new Date();
	/// calendar.setTime(trialTime);
	/// 
	/// // print out a bunch of interesting things
	/// System.out.println("ERA: " + calendar.get(Calendar.ERA));
	/// System.out.println("YEAR: " + calendar.get(Calendar.YEAR));
	/// System.out.println("MONTH: " + calendar.get(Calendar.MONTH));
	/// System.out.println("WEEK_OF_YEAR: " + calendar.get(Calendar.WEEK_OF_YEAR));
	/// System.out.println("WEEK_OF_MONTH: " + calendar.get(Calendar.WEEK_OF_MONTH));
	/// System.out.println("DATE: " + calendar.get(Calendar.DATE));
	/// System.out.println("DAY_OF_MONTH: " + calendar.get(Calendar.DAY_OF_MONTH));
	/// System.out.println("DAY_OF_YEAR: " + calendar.get(Calendar.DAY_OF_YEAR));
	/// System.out.println("DAY_OF_WEEK: " + calendar.get(Calendar.DAY_OF_WEEK));
	/// System.out.println("DAY_OF_WEEK_IN_MONTH: "
	///                    + calendar.get(Calendar.DAY_OF_WEEK_IN_MONTH));
	/// System.out.println("AM_PM: " + calendar.get(Calendar.AM_PM));
	/// System.out.println("HOUR: " + calendar.get(Calendar.HOUR));
	/// System.out.println("HOUR_OF_DAY: " + calendar.get(Calendar.HOUR_OF_DAY));
	/// System.out.println("MINUTE: " + calendar.get(Calendar.MINUTE));
	/// System.out.println("SECOND: " + calendar.get(Calendar.SECOND));
	/// System.out.println("MILLISECOND: " + calendar.get(Calendar.MILLISECOND));
	/// System.out.println("ZONE_OFFSET: "
	///                    + (calendar.get(Calendar.ZONE_OFFSET)/(60*60*1000)));
	/// System.out.println("DST_OFFSET: "
	///                    + (calendar.get(Calendar.DST_OFFSET)/(60*60*1000)));
	/// 
	/// System.out.println("Current Time, with hour reset to 3");
	/// calendar.clear(Calendar.HOUR_OF_DAY); // so doesn't override
	/// calendar.set(Calendar.HOUR, 3);
	/// System.out.println("ERA: " + calendar.get(Calendar.ERA));
	/// System.out.println("YEAR: " + calendar.get(Calendar.YEAR));
	/// System.out.println("MONTH: " + calendar.get(Calendar.MONTH));
	/// System.out.println("WEEK_OF_YEAR: " + calendar.get(Calendar.WEEK_OF_YEAR));
	/// System.out.println("WEEK_OF_MONTH: " + calendar.get(Calendar.WEEK_OF_MONTH));
	/// System.out.println("DATE: " + calendar.get(Calendar.DATE));
	/// System.out.println("DAY_OF_MONTH: " + calendar.get(Calendar.DAY_OF_MONTH));
	/// System.out.println("DAY_OF_YEAR: " + calendar.get(Calendar.DAY_OF_YEAR));
	/// System.out.println("DAY_OF_WEEK: " + calendar.get(Calendar.DAY_OF_WEEK));
	/// System.out.println("DAY_OF_WEEK_IN_MONTH: "
	///                    + calendar.get(Calendar.DAY_OF_WEEK_IN_MONTH));
	/// System.out.println("AM_PM: " + calendar.get(Calendar.AM_PM));
	/// System.out.println("HOUR: " + calendar.get(Calendar.HOUR));
	/// System.out.println("HOUR_OF_DAY: " + calendar.get(Calendar.HOUR_OF_DAY));
	/// System.out.println("MINUTE: " + calendar.get(Calendar.MINUTE));
	/// System.out.println("SECOND: " + calendar.get(Calendar.SECOND));
	/// System.out.println("MILLISECOND: " + calendar.get(Calendar.MILLISECOND));
	/// System.out.println("ZONE_OFFSET: "
	///        + (calendar.get(Calendar.ZONE_OFFSET)/(60*60*1000))); // in hours
	/// System.out.println("DST_OFFSET: "
	///        + (calendar.get(Calendar.DST_OFFSET)/(60*60*1000))); // in hours
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          TimeZone
	/// @author David Goldsmith, Mark Davis, Chen-Lieh Huang, Alan Liu
	/// @since JDK1.1 </seealso>
	public class GregorianCalendar : Calendar
	{
		/*
		 * Implementation Notes
		 *
		 * The epoch is the number of days or milliseconds from some defined
		 * starting point. The epoch for java.util.Date is used here; that is,
		 * milliseconds from January 1, 1970 (Gregorian), midnight UTC.  Other
		 * epochs which are used are January 1, year 1 (Gregorian), which is day 1
		 * of the Gregorian calendar, and December 30, year 0 (Gregorian), which is
		 * day 1 of the Julian calendar.
		 *
		 * We implement the proleptic Julian and Gregorian calendars.  This means we
		 * implement the modern definition of the calendar even though the
		 * historical usage differs.  For example, if the Gregorian change is set
		 * to new Date(Long.MIN_VALUE), we have a pure Gregorian calendar which
		 * labels dates preceding the invention of the Gregorian calendar in 1582 as
		 * if the calendar existed then.
		 *
		 * Likewise, with the Julian calendar, we assume a consistent
		 * 4-year leap year rule, even though the historical pattern of
		 * leap years is irregular, being every 3 years from 45 BCE
		 * through 9 BCE, then every 4 years from 8 CE onwards, with no
		 * leap years in-between.  Thus date computations and functions
		 * such as isLeapYear() are not intended to be historically
		 * accurate.
		 */

	//////////////////
	// Class Variables
	//////////////////

		/// <summary>
		/// Value of the <code>ERA</code> field indicating
		/// the period before the common era (before Christ), also known as BCE.
		/// The sequence of years at the transition from <code>BC</code> to <code>AD</code> is
		/// ..., 2 BC, 1 BC, 1 AD, 2 AD,...
		/// </summary>
		/// <seealso cref= #ERA </seealso>
		public const int BC = 0;

		/// <summary>
		/// Value of the <seealso cref="#ERA"/> field indicating
		/// the period before the common era, the same value as <seealso cref="#BC"/>.
		/// </summary>
		/// <seealso cref= #CE </seealso>
		internal const int BCE = 0;

		/// <summary>
		/// Value of the <code>ERA</code> field indicating
		/// the common era (Anno Domini), also known as CE.
		/// The sequence of years at the transition from <code>BC</code> to <code>AD</code> is
		/// ..., 2 BC, 1 BC, 1 AD, 2 AD,...
		/// </summary>
		/// <seealso cref= #ERA </seealso>
		public const int AD = 1;

		/// <summary>
		/// Value of the <seealso cref="#ERA"/> field indicating
		/// the common era, the same value as <seealso cref="#AD"/>.
		/// </summary>
		/// <seealso cref= #BCE </seealso>
		internal const int CE = 1;

		private const int EPOCH_OFFSET = 719163; // Fixed date of January 1, 1970 (Gregorian)
		private const int EPOCH_YEAR = 1970;

		internal static readonly int[] MONTH_LENGTH = new int[] {31,28,31,30,31,30,31,31,30,31,30,31}; // 0-based
		internal static readonly int[] LEAP_MONTH_LENGTH = new int[] {31,29,31,30,31,30,31,31,30,31,30,31}; // 0-based

		// Useful millisecond constants.  Although ONE_DAY and ONE_WEEK can fit
		// into ints, they must be longs in order to prevent arithmetic overflow
		// when performing (bug 4173516).
		private const int ONE_SECOND = 1000;
		private static readonly int ONE_MINUTE = 60 * ONE_SECOND;
		private static readonly int ONE_HOUR = 60 * ONE_MINUTE;
		private static readonly long ONE_DAY = 24 * ONE_HOUR;
		private static readonly long ONE_WEEK = 7 * ONE_DAY;

		/*
		 * <pre>
		 *                            Greatest       Least
		 * Field name        Minimum   Minimum     Maximum     Maximum
		 * ----------        -------   -------     -------     -------
		 * ERA                     0         0           1           1
		 * YEAR                    1         1   292269054   292278994
		 * MONTH                   0         0          11          11
		 * WEEK_OF_YEAR            1         1          52*         53
		 * WEEK_OF_MONTH           0         0           4*          6
		 * DAY_OF_MONTH            1         1          28*         31
		 * DAY_OF_YEAR             1         1         365*        366
		 * DAY_OF_WEEK             1         1           7           7
		 * DAY_OF_WEEK_IN_MONTH   -1        -1           4*          6
		 * AM_PM                   0         0           1           1
		 * HOUR                    0         0          11          11
		 * HOUR_OF_DAY             0         0          23          23
		 * MINUTE                  0         0          59          59
		 * SECOND                  0         0          59          59
		 * MILLISECOND             0         0         999         999
		 * ZONE_OFFSET        -13:00    -13:00       14:00       14:00
		 * DST_OFFSET           0:00      0:00        0:20        2:00
		 * </pre>
		 * *: depends on the Gregorian change date
		 */
		internal static readonly int[] MIN_VALUES = new int[] {BCE, 1, JANUARY, 1, 0, 1, 1, SUNDAY, 1, AM, 0, 0, 0, 0, 0, -13 * ONE_HOUR, 0};
		internal static readonly int[] LEAST_MAX_VALUES = new int[] {CE, 292269054, DECEMBER, 52, 4, 28, 365, SATURDAY, 4, PM, 11, 23, 59, 59, 999, 14 * ONE_HOUR, 20 * ONE_MINUTE};
		internal static readonly int[] MAX_VALUES = new int[] {CE, 292278994, DECEMBER, 53, 6, 31, 366, SATURDAY, 6, PM, 11, 23, 59, 59, 999, 14 * ONE_HOUR, 2 * ONE_HOUR};

		// Proclaim serialization compatibility with JDK 1.1
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("FieldNameHidesFieldInSuperclass") static final long serialVersionUID = -8125100834729963327L;
		internal new const long SerialVersionUID = -8125100834729963327L;

		// Reference to the sun.util.calendar.Gregorian instance (singleton).
		private static readonly Gregorian Gcal = CalendarSystem.GregorianCalendar;

		// Reference to the JulianCalendar instance (singleton), set as needed. See
		// getJulianCalendarSystem().
		private static JulianCalendar Jcal;

		// JulianCalendar eras. See getJulianCalendarSystem().
		private static Era[] Jeras;

		// The default value of gregorianCutover.
		internal const long DEFAULT_GREGORIAN_CUTOVER = -12219292800000L;

	/////////////////////
	// Instance Variables
	/////////////////////

		/// <summary>
		/// The point at which the Gregorian calendar rules are used, measured in
		/// milliseconds from the standard epoch.  Default is October 15, 1582
		/// (Gregorian) 00:00:00 UTC or -12219292800000L.  For this value, October 4,
		/// 1582 (Julian) is followed by October 15, 1582 (Gregorian).  This
		/// corresponds to Julian day number 2299161.
		/// @serial
		/// </summary>
		private long GregorianCutover = DEFAULT_GREGORIAN_CUTOVER;

		/// <summary>
		/// The fixed date of the gregorianCutover.
		/// </summary>
		[NonSerialized]
		private long GregorianCutoverDate_Renamed = (((DEFAULT_GREGORIAN_CUTOVER + 1) / ONE_DAY) - 1) + EPOCH_OFFSET; // == 577736

		/// <summary>
		/// The normalized year of the gregorianCutover in Gregorian, with
		/// 0 representing 1 BCE, -1 representing 2 BCE, etc.
		/// </summary>
		[NonSerialized]
		private int GregorianCutoverYear = 1582;

		/// <summary>
		/// The normalized year of the gregorianCutover in Julian, with 0
		/// representing 1 BCE, -1 representing 2 BCE, etc.
		/// </summary>
		[NonSerialized]
		private int GregorianCutoverYearJulian = 1582;

		/// <summary>
		/// gdate always has a sun.util.calendar.Gregorian.Date instance to
		/// avoid overhead of creating it. The assumption is that most
		/// applications will need only Gregorian calendar calculations.
		/// </summary>
		[NonSerialized]
		private BaseCalendar.Date Gdate;

		/// <summary>
		/// Reference to either gdate or a JulianCalendar.Date
		/// instance. After calling complete(), this value is guaranteed to
		/// be set.
		/// </summary>
		[NonSerialized]
		private BaseCalendar.Date Cdate;

		/// <summary>
		/// The CalendarSystem used to calculate the date in cdate. After
		/// calling complete(), this value is guaranteed to be set and
		/// consistent with the cdate value.
		/// </summary>
		[NonSerialized]
		private BaseCalendar Calsys;

		/// <summary>
		/// Temporary int[2] to get time zone offsets. zoneOffsets[0] gets
		/// the GMT offset value and zoneOffsets[1] gets the DST saving
		/// value.
		/// </summary>
		[NonSerialized]
		private int[] ZoneOffsets;

		/// <summary>
		/// Temporary storage for saving original fields[] values in
		/// non-lenient mode.
		/// </summary>
		[NonSerialized]
		private int[] OriginalFields;

	///////////////
	// Constructors
	///////////////

		/// <summary>
		/// Constructs a default <code>GregorianCalendar</code> using the current time
		/// in the default time zone with the default
		/// <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		/// </summary>
		public GregorianCalendar() : this(TimeZone.DefaultRef, Locale.GetDefault(Locale.Category.FORMAT))
		{
			ZoneShared = true;
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> based on the current time
		/// in the given time zone with the default
		/// <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		/// </summary>
		/// <param name="zone"> the given time zone. </param>
		public GregorianCalendar(TimeZone zone) : this(zone, Locale.GetDefault(Locale.Category.FORMAT))
		{
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> based on the current time
		/// in the default time zone with the given locale.
		/// </summary>
		/// <param name="aLocale"> the given locale. </param>
		public GregorianCalendar(Locale aLocale) : this(TimeZone.DefaultRef, aLocale)
		{
			ZoneShared = true;
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> based on the current time
		/// in the given time zone with the given locale.
		/// </summary>
		/// <param name="zone"> the given time zone. </param>
		/// <param name="aLocale"> the given locale. </param>
		public GregorianCalendar(TimeZone zone, Locale aLocale) : base(zone, aLocale)
		{
			Gdate = (BaseCalendar.Date) Gcal.newCalendarDate(zone);
			TimeInMillis = DateTimeHelperClass.CurrentUnixTimeMillis();
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> with the given date set
		/// in the default time zone with the default locale.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		public GregorianCalendar(int year, int month, int dayOfMonth) : this(year, month, dayOfMonth, 0, 0, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> with the given date
		/// and time set for the default time zone with the default locale.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		/// <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		/// in the calendar. </param>
		/// <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		/// in the calendar. </param>
		public GregorianCalendar(int year, int month, int dayOfMonth, int hourOfDay, int minute) : this(year, month, dayOfMonth, hourOfDay, minute, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a GregorianCalendar with the given date
		/// and time set for the default time zone with the default locale.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		/// <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		/// in the calendar. </param>
		/// <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		/// in the calendar. </param>
		/// <param name="second"> the value used to set the <code>SECOND</code> calendar field
		/// in the calendar. </param>
		public GregorianCalendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second) : this(year, month, dayOfMonth, hourOfDay, minute, second, 0)
		{
		}

		/// <summary>
		/// Constructs a <code>GregorianCalendar</code> with the given date
		/// and time set for the default time zone with the default locale.
		/// </summary>
		/// <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		/// <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		/// Month value is 0-based. e.g., 0 for January. </param>
		/// <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		/// <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		/// in the calendar. </param>
		/// <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		/// in the calendar. </param>
		/// <param name="second"> the value used to set the <code>SECOND</code> calendar field
		/// in the calendar. </param>
		/// <param name="millis"> the value used to set the <code>MILLISECOND</code> calendar field </param>
		internal GregorianCalendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second, int millis) : base()
		{
			Gdate = (BaseCalendar.Date) Gcal.newCalendarDate(Zone);
			this.Set(YEAR, year);
			this.Set(MONTH, month);
			this.Set(DAY_OF_MONTH, dayOfMonth);

			// Set AM_PM and HOUR here to set their stamp values before
			// setting HOUR_OF_DAY (6178071).
			if (hourOfDay >= 12 && hourOfDay <= 23)
			{
				// If hourOfDay is a valid PM hour, set the correct PM values
				// so that it won't throw an exception in case it's set to
				// non-lenient later.
				this.InternalSet(AM_PM, PM);
				this.InternalSet(HOUR, hourOfDay - 12);
			}
			else
			{
				// The default value for AM_PM is AM.
				// We don't care any out of range value here for leniency.
				this.InternalSet(HOUR, hourOfDay);
			}
			// The stamp values of AM_PM and HOUR must be COMPUTED. (6440854)
			FieldsComputed = HOUR_MASK | AM_PM_MASK;

			this.Set(HOUR_OF_DAY, hourOfDay);
			this.Set(MINUTE, minute);
			this.Set(SECOND, second);
			// should be changed to set() when this constructor is made
			// public.
			this.InternalSet(MILLISECOND, millis);
		}

		/// <summary>
		/// Constructs an empty GregorianCalendar.
		/// </summary>
		/// <param name="zone">    the given time zone </param>
		/// <param name="aLocale"> the given locale </param>
		/// <param name="flag">    the flag requesting an empty instance </param>
		internal GregorianCalendar(TimeZone zone, Locale locale, bool flag) : base(zone, locale)
		{
			Gdate = (BaseCalendar.Date) Gcal.newCalendarDate(Zone);
		}

	/////////////////
	// Public methods
	/////////////////

		/// <summary>
		/// Sets the <code>GregorianCalendar</code> change date. This is the point when the switch
		/// from Julian dates to Gregorian dates occurred. Default is October 15,
		/// 1582 (Gregorian). Previous to this, dates will be in the Julian calendar.
		/// <para>
		/// To obtain a pure Julian calendar, set the change date to
		/// <code>Date(Long.MAX_VALUE)</code>.  To obtain a pure Gregorian calendar,
		/// set the change date to <code>Date(Long.MIN_VALUE)</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date"> the given Gregorian cutover date. </param>
		public virtual Date GregorianChange
		{
			set
			{
				long cutoverTime = value.Time;
				if (cutoverTime == GregorianCutover)
				{
					return;
				}
				// Before changing the cutover value, make sure to have the
				// time of this calendar.
				Complete();
				GregorianChange = cutoverTime;
			}
			get
			{
				return new Date(GregorianCutover);
			}
		}

		private long GregorianChange
		{
			set
			{
				GregorianCutover = value;
				GregorianCutoverDate_Renamed = CalendarUtils.floorDivide(value, ONE_DAY) + EPOCH_OFFSET;
    
				// To provide the "pure" Julian calendar as advertised.
				// Strictly speaking, the last millisecond should be a
				// Gregorian date. However, the API doc specifies that setting
				// the cutover date to Long.MAX_VALUE will make this calendar
				// a pure Julian calendar. (See 4167995)
				if (value == Long.MaxValue)
				{
					GregorianCutoverDate_Renamed++;
				}
    
				BaseCalendar.Date d = GregorianCutoverDate;
    
				// Set the cutover year (in the Gregorian year numbering)
				GregorianCutoverYear = d.Year;
    
				BaseCalendar julianCal = JulianCalendarSystem;
				d = (BaseCalendar.Date) julianCal.newCalendarDate(TimeZone.NO_TIMEZONE);
				julianCal.getCalendarDateFromFixedDate(d, GregorianCutoverDate_Renamed - 1);
				GregorianCutoverYearJulian = d.NormalizedYear;
    
				if (Time_Renamed < GregorianCutover)
				{
					// The field values are no longer valid under the new
					// cutover date.
					SetUnnormalized();
				}
			}
		}


		/// <summary>
		/// Determines if the given year is a leap year. Returns <code>true</code> if
		/// the given year is a leap year. To specify BC year numbers,
		/// <code>1 - year number</code> must be given. For example, year BC 4 is
		/// specified as -3.
		/// </summary>
		/// <param name="year"> the given year. </param>
		/// <returns> <code>true</code> if the given year is a leap year; <code>false</code> otherwise. </returns>
		public virtual bool IsLeapYear(int year)
		{
			if ((year & 3) != 0)
			{
				return false;
			}

			if (year > GregorianCutoverYear)
			{
				return (year % 100 != 0) || (year % 400 == 0); // Gregorian
			}
			if (year < GregorianCutoverYearJulian)
			{
				return true; // Julian
			}
			bool gregorian;
			// If the given year is the Gregorian cutover year, we need to
			// determine which calendar system to be applied to February in the year.
			if (GregorianCutoverYear == GregorianCutoverYearJulian)
			{
				BaseCalendar.Date d = GetCalendarDate(GregorianCutoverDate_Renamed); // Gregorian
				gregorian = d.Month < BaseCalendar.MARCH;
			}
			else
			{
				gregorian = year == GregorianCutoverYear;
			}
			return gregorian ? (year % 100 != 0) || (year % 400 == 0) : true;
		}

		/// <summary>
		/// Returns {@code "gregory"} as the calendar type.
		/// </summary>
		/// <returns> {@code "gregory"}
		/// @since 1.8 </returns>
		public override String CalendarType
		{
			get
			{
				return "gregory";
			}
		}

		/// <summary>
		/// Compares this <code>GregorianCalendar</code> to the specified
		/// <code>Object</code>. The result is <code>true</code> if and
		/// only if the argument is a <code>GregorianCalendar</code> object
		/// that represents the same time value (millisecond offset from
		/// the <a href="Calendar.html#Epoch">Epoch</a>) under the same
		/// <code>Calendar</code> parameters and Gregorian change date as
		/// this object.
		/// </summary>
		/// <param name="obj"> the object to compare with. </param>
		/// <returns> <code>true</code> if this object is equal to <code>obj</code>;
		/// <code>false</code> otherwise. </returns>
		/// <seealso cref= Calendar#compareTo(Calendar) </seealso>
		public override bool Equals(Object obj)
		{
			return obj is GregorianCalendar && base.Equals(obj) && GregorianCutover == ((GregorianCalendar)obj).GregorianCutover;
		}

		/// <summary>
		/// Generates the hash code for this <code>GregorianCalendar</code> object.
		/// </summary>
		public override int HashCode()
		{
			return base.HashCode() ^ (int)GregorianCutoverDate_Renamed;
		}

		/// <summary>
		/// Adds the specified (signed) amount of time to the given calendar field,
		/// based on the calendar's rules.
		/// 
		/// <para><em>Add rule 1</em>. The value of <code>field</code>
		/// after the call minus the value of <code>field</code> before the
		/// call is <code>amount</code>, modulo any overflow that has occurred in
		/// <code>field</code>. Overflow occurs when a field value exceeds its
		/// range and, as a result, the next larger field is incremented or
		/// decremented and the field value is adjusted back into its range.</para>
		/// 
		/// <para><em>Add rule 2</em>. If a smaller field is expected to be
		/// invariant, but it is impossible for it to be equal to its
		/// prior value because of changes in its minimum or maximum after
		/// <code>field</code> is changed, then its value is adjusted to be as close
		/// as possible to its expected value. A smaller field represents a
		/// smaller unit of time. <code>HOUR</code> is a smaller field than
		/// <code>DAY_OF_MONTH</code>. No adjustment is made to smaller fields
		/// that are not expected to be invariant. The calendar system
		/// determines what fields are expected to be invariant.</para>
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <param name="amount"> the amount of date or time to be added to the field. </param>
		/// <exception cref="IllegalArgumentException"> if <code>field</code> is
		/// <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		/// or if any calendar fields have out-of-range values in
		/// non-lenient mode. </exception>
		public override void Add(int field, int amount)
		{
			// If amount == 0, do nothing even the given field is out of
			// range. This is tested by JCK.
			if (amount == 0)
			{
				return; // Do nothing!
			}

			if (field < 0 || field >= ZONE_OFFSET)
			{
				throw new IllegalArgumentException();
			}

			// Sync the time and calendar fields.
			Complete();

			if (field == YEAR)
			{
				int year = InternalGet(YEAR);
				if (InternalGetEra() == CE)
				{
					year += amount;
					if (year > 0)
					{
						Set(YEAR, year);
					} // year <= 0
					else
					{
						Set(YEAR, 1 - year);
						// if year == 0, you get 1 BCE.
						Set(ERA, BCE);
					}
				}
				else // era == BCE
				{
					year -= amount;
					if (year > 0)
					{
						Set(YEAR, year);
					} // year <= 0
					else
					{
						Set(YEAR, 1 - year);
						// if year == 0, you get 1 CE
						Set(ERA, CE);
					}
				}
				PinDayOfMonth();
			}
			else if (field == MONTH)
			{
				int month = InternalGet(MONTH) + amount;
				int year = InternalGet(YEAR);
				int y_amount;

				if (month >= 0)
				{
					y_amount = month / 12;
				}
				else
				{
					y_amount = (month + 1) / 12 - 1;
				}
				if (y_amount != 0)
				{
					if (InternalGetEra() == CE)
					{
						year += y_amount;
						if (year > 0)
						{
							Set(YEAR, year);
						} // year <= 0
						else
						{
							Set(YEAR, 1 - year);
							// if year == 0, you get 1 BCE
							Set(ERA, BCE);
						}
					}
					else // era == BCE
					{
						year -= y_amount;
						if (year > 0)
						{
							Set(YEAR, year);
						} // year <= 0
						else
						{
							Set(YEAR, 1 - year);
							// if year == 0, you get 1 CE
							Set(ERA, CE);
						}
					}
				}

				if (month >= 0)
				{
					Set(MONTH, month % 12);
				}
				else
				{
					// month < 0
					month %= 12;
					if (month < 0)
					{
						month += 12;
					}
					Set(MONTH, JANUARY + month);
				}
				PinDayOfMonth();
			}
			else if (field == ERA)
			{
				int era = InternalGet(ERA) + amount;
				if (era < 0)
				{
					era = 0;
				}
				if (era > 1)
				{
					era = 1;
				}
				Set(ERA, era);
			}
			else
			{
				long delta = amount;
				long timeOfDay = 0;
				switch (field)
				{
				// Handle the time fields here. Convert the given
				// amount to milliseconds and call setTimeInMillis.
				case HOUR:
				case HOUR_OF_DAY:
					delta *= 60 * 60 * 1000; // hours to minutes
					break;

				case MINUTE:
					delta *= 60 * 1000; // minutes to seconds
					break;

				case SECOND:
					delta *= 1000; // seconds to milliseconds
					break;

				case MILLISECOND:
					break;

				// Handle week, day and AM_PM fields which involves
				// time zone offset change adjustment. Convert the
				// given amount to the number of days.
				case WEEK_OF_YEAR:
				case WEEK_OF_MONTH:
				case DAY_OF_WEEK_IN_MONTH:
					delta *= 7;
					break;

				case DAY_OF_MONTH: // synonym of DATE
				case DAY_OF_YEAR:
				case DAY_OF_WEEK:
					break;

				case AM_PM:
					// Convert the amount to the number of days (delta)
					// and +12 or -12 hours (timeOfDay).
					delta = amount / 2;
					timeOfDay = 12 * (amount % 2);
					break;
				}

				// The time fields don't require time zone offset change
				// adjustment.
				if (field >= HOUR)
				{
					TimeInMillis = Time_Renamed + delta;
					return;
				}

				// The rest of the fields (week, day or AM_PM fields)
				// require time zone offset (both GMT and DST) change
				// adjustment.

				// Translate the current time to the fixed date and time
				// of the day.
				long fd = CurrentFixedDate;
				timeOfDay += InternalGet(HOUR_OF_DAY);
				timeOfDay *= 60;
				timeOfDay += InternalGet(MINUTE);
				timeOfDay *= 60;
				timeOfDay += InternalGet(SECOND);
				timeOfDay *= 1000;
				timeOfDay += InternalGet(MILLISECOND);
				if (timeOfDay >= ONE_DAY)
				{
					fd++;
					timeOfDay -= ONE_DAY;
				}
				else if (timeOfDay < 0)
				{
					fd--;
					timeOfDay += ONE_DAY;
				}

				fd += delta; // fd is the expected fixed date after the calculation
				int zoneOffset = InternalGet(ZONE_OFFSET) + InternalGet(DST_OFFSET);
				TimeInMillis = (fd - EPOCH_OFFSET) * ONE_DAY + timeOfDay - zoneOffset;
				zoneOffset -= InternalGet(ZONE_OFFSET) + InternalGet(DST_OFFSET);
				// If the time zone offset has changed, then adjust the difference.
				if (zoneOffset != 0)
				{
					TimeInMillis = Time_Renamed + zoneOffset;
					long fd2 = CurrentFixedDate;
					// If the adjustment has changed the date, then take
					// the previous one.
					if (fd2 != fd)
					{
						TimeInMillis = Time_Renamed - zoneOffset;
					}
				}
			}
		}

		/// <summary>
		/// Adds or subtracts (up/down) a single unit of time on the given time
		/// field without changing larger fields.
		/// <para>
		/// <em>Example</em>: Consider a <code>GregorianCalendar</code>
		/// originally set to December 31, 1999. Calling <seealso cref="#roll(int,boolean) roll(Calendar.MONTH, true)"/>
		/// sets the calendar to January 31, 1999.  The <code>YEAR</code> field is unchanged
		/// because it is a larger field than <code>MONTH</code>.</para>
		/// </summary>
		/// <param name="up"> indicates if the value of the specified calendar field is to be
		/// rolled up or rolled down. Use <code>true</code> if rolling up, <code>false</code> otherwise. </param>
		/// <exception cref="IllegalArgumentException"> if <code>field</code> is
		/// <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		/// or if any calendar fields have out-of-range values in
		/// non-lenient mode. </exception>
		/// <seealso cref= #add(int,int) </seealso>
		/// <seealso cref= #set(int,int) </seealso>
		public override void Roll(int field, bool up)
		{
			Roll(field, up ? + 1 : -1);
		}

		/// <summary>
		/// Adds a signed amount to the specified calendar field without changing larger fields.
		/// A negative roll amount means to subtract from field without changing
		/// larger fields. If the specified amount is 0, this method performs nothing.
		/// 
		/// <para>This method calls <seealso cref="#complete()"/> before adding the
		/// amount so that all the calendar fields are normalized. If there
		/// is any calendar field having an out-of-range value in non-lenient mode, then an
		/// <code>IllegalArgumentException</code> is thrown.
		/// 
		/// </para>
		/// <para>
		/// <em>Example</em>: Consider a <code>GregorianCalendar</code>
		/// originally set to August 31, 1999. Calling <code>roll(Calendar.MONTH,
		/// 8)</code> sets the calendar to April 30, <strong>1999</strong>. Using a
		/// <code>GregorianCalendar</code>, the <code>DAY_OF_MONTH</code> field cannot
		/// be 31 in the month April. <code>DAY_OF_MONTH</code> is set to the closest possible
		/// value, 30. The <code>YEAR</code> field maintains the value of 1999 because it
		/// is a larger field than <code>MONTH</code>.
		/// </para>
		/// <para>
		/// <em>Example</em>: Consider a <code>GregorianCalendar</code>
		/// originally set to Sunday June 6, 1999. Calling
		/// <code>roll(Calendar.WEEK_OF_MONTH, -1)</code> sets the calendar to
		/// Tuesday June 1, 1999, whereas calling
		/// <code>add(Calendar.WEEK_OF_MONTH, -1)</code> sets the calendar to
		/// Sunday May 30, 1999. This is because the roll rule imposes an
		/// additional constraint: The <code>MONTH</code> must not change when the
		/// <code>WEEK_OF_MONTH</code> is rolled. Taken together with add rule 1,
		/// the resultant date must be between Tuesday June 1 and Saturday June
		/// 5. According to add rule 2, the <code>DAY_OF_WEEK</code>, an invariant
		/// when changing the <code>WEEK_OF_MONTH</code>, is set to Tuesday, the
		/// closest possible value to Sunday (where Sunday is the first day of the
		/// week).</para>
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <param name="amount"> the signed amount to add to <code>field</code>. </param>
		/// <exception cref="IllegalArgumentException"> if <code>field</code> is
		/// <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		/// or if any calendar fields have out-of-range values in
		/// non-lenient mode. </exception>
		/// <seealso cref= #roll(int,boolean) </seealso>
		/// <seealso cref= #add(int,int) </seealso>
		/// <seealso cref= #set(int,int)
		/// @since 1.2 </seealso>
		public override void Roll(int field, int amount)
		{
			// If amount == 0, do nothing even the given field is out of
			// range. This is tested by JCK.
			if (amount == 0)
			{
				return;
			}

			if (field < 0 || field >= ZONE_OFFSET)
			{
				throw new IllegalArgumentException();
			}

			// Sync the time and calendar fields.
			Complete();

			int min = GetMinimum(field);
			int max = GetMaximum(field);

			switch (field)
			{
			case AM_PM:
			case ERA:
			case YEAR:
			case MINUTE:
			case SECOND:
			case MILLISECOND:
				// These fields are handled simply, since they have fixed minima
				// and maxima.  The field DAY_OF_MONTH is almost as simple.  Other
				// fields are complicated, since the range within they must roll
				// varies depending on the date.
				break;

			case HOUR:
			case HOUR_OF_DAY:
			{
					int unit = max + 1; // 12 or 24 hours
					int h = InternalGet(field);
					int nh = (h + amount) % unit;
					if (nh < 0)
					{
						nh += unit;
					}
					Time_Renamed += ONE_HOUR * (nh - h);

					// The day might have changed, which could happen if
					// the daylight saving time transition brings it to
					// the next day, although it's very unlikely. But we
					// have to make sure not to change the larger fields.
					CalendarDate d = Calsys.getCalendarDate(Time_Renamed, Zone);
					if (InternalGet(DAY_OF_MONTH) != d.DayOfMonth)
					{
						d.setDate(InternalGet(YEAR), InternalGet(MONTH) + 1, InternalGet(DAY_OF_MONTH));
						if (field == HOUR)
						{
							assert(InternalGet(AM_PM) == PM);
							d.addHours(+12); // restore PM
						}
						Time_Renamed = Calsys.getTime(d);
					}
					int hourOfDay = d.Hours;
					InternalSet(field, hourOfDay % unit);
					if (field == HOUR)
					{
						InternalSet(HOUR_OF_DAY, hourOfDay);
					}
					else
					{
						InternalSet(AM_PM, hourOfDay / 12);
						InternalSet(HOUR, hourOfDay % 12);
					}

					// Time zone offset and/or daylight saving might have changed.
					int zoneOffset = d.ZoneOffset;
					int saving = d.DaylightSaving;
					InternalSet(ZONE_OFFSET, zoneOffset - saving);
					InternalSet(DST_OFFSET, saving);
					return;
			}

			case MONTH:
				// Rolling the month involves both pinning the final value to [0, 11]
				// and adjusting the DAY_OF_MONTH if necessary.  We only adjust the
				// DAY_OF_MONTH if, after updating the MONTH field, it is illegal.
				// E.g., <jan31>.roll(MONTH, 1) -> <feb28> or <feb29>.
			{
					if (!IsCutoverYear(Cdate.NormalizedYear))
					{
						int mon = (InternalGet(MONTH) + amount) % 12;
						if (mon < 0)
						{
							mon += 12;
						}
						Set(MONTH, mon);

						// Keep the day of month in the range.  We don't want to spill over
						// into the next month; e.g., we don't want jan31 + 1 mo -> feb31 ->
						// mar3.
						int monthLen = MonthLength(mon);
						if (InternalGet(DAY_OF_MONTH) > monthLen)
						{
							Set(DAY_OF_MONTH, monthLen);
						}
					}
					else
					{
						// We need to take care of different lengths in
						// year and month due to the cutover.
						int yearLength = GetActualMaximum(MONTH) + 1;
						int mon = (InternalGet(MONTH) + amount) % yearLength;
						if (mon < 0)
						{
							mon += yearLength;
						}
						Set(MONTH, mon);
						int monthLen = GetActualMaximum(DAY_OF_MONTH);
						if (InternalGet(DAY_OF_MONTH) > monthLen)
						{
							Set(DAY_OF_MONTH, monthLen);
						}
					}
					return;
			}

			case WEEK_OF_YEAR:
			{
					int y = Cdate.NormalizedYear;
					max = GetActualMaximum(WEEK_OF_YEAR);
					Set(DAY_OF_WEEK, InternalGet(DAY_OF_WEEK));
					int woy = InternalGet(WEEK_OF_YEAR);
					int value = woy + amount;
					if (!IsCutoverYear(y))
					{
						int weekYear = WeekYear;
						if (weekYear == y)
						{
							// If the new value is in between min and max
							// (exclusive), then we can use the value.
							if (value > min && value < max)
							{
								Set(WEEK_OF_YEAR, value);
								return;
							}
							long fd = CurrentFixedDate;
							// Make sure that the min week has the current DAY_OF_WEEK
							// in the calendar year
							long day1 = fd - (7 * (woy - min));
							if (Calsys.getYearFromFixedDate(day1) != y)
							{
								min++;
							}

							// Make sure the same thing for the max week
							fd += 7 * (max - InternalGet(WEEK_OF_YEAR));
							if (Calsys.getYearFromFixedDate(fd) != y)
							{
								max--;
							}
						}
						else
						{
							// When WEEK_OF_YEAR and YEAR are out of sync,
							// adjust woy and amount to stay in the calendar year.
							if (weekYear > y)
							{
								if (amount < 0)
								{
									amount++;
								}
								woy = max;
							}
							else
							{
								if (amount > 0)
								{
									amount -= woy - max;
								}
								woy = min;
							}
						}
						Set(field, GetRolledValue(woy, amount, min, max));
						return;
					}

					// Handle cutover here.
					long fd = CurrentFixedDate;
					BaseCalendar cal;
					if (GregorianCutoverYear == GregorianCutoverYearJulian)
					{
						cal = CutoverCalendarSystem;
					}
					else if (y == GregorianCutoverYear)
					{
						cal = Gcal;
					}
					else
					{
						cal = JulianCalendarSystem;
					}
					long day1 = fd - (7 * (woy - min));
					// Make sure that the min week has the current DAY_OF_WEEK
					if (cal.getYearFromFixedDate(day1) != y)
					{
						min++;
					}

					// Make sure the same thing for the max week
					fd += 7 * (max - woy);
					cal = (fd >= GregorianCutoverDate_Renamed) ? Gcal : JulianCalendarSystem;
					if (cal.getYearFromFixedDate(fd) != y)
					{
						max--;
					}
					// value: the new WEEK_OF_YEAR which must be converted
					// to month and day of month.
					value = GetRolledValue(woy, amount, min, max) - 1;
					BaseCalendar.Date d = GetCalendarDate(day1 + value * 7);
					Set(MONTH, d.Month - 1);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case WEEK_OF_MONTH:
			{
					bool isCutoverYear = IsCutoverYear(Cdate.NormalizedYear);
					// dow: relative day of week from first day of week
					int dow = InternalGet(DAY_OF_WEEK) - FirstDayOfWeek;
					if (dow < 0)
					{
						dow += 7;
					}

					long fd = CurrentFixedDate;
					long month1; // fixed date of the first day (usually 1) of the month
					int monthLength; // actual month length
					if (isCutoverYear)
					{
						month1 = GetFixedDateMonth1(Cdate, fd);
						monthLength = ActualMonthLength();
					}
					else
					{
						month1 = fd - InternalGet(DAY_OF_MONTH) + 1;
						monthLength = Calsys.getMonthLength(Cdate);
					}

					// the first day of week of the month.
					long monthDay1st = BaseCalendar.getDayOfWeekDateOnOrBefore(month1 + 6, FirstDayOfWeek);
					// if the week has enough days to form a week, the
					// week starts from the previous month.
					if ((int)(monthDay1st - month1) >= MinimalDaysInFirstWeek)
					{
						monthDay1st -= 7;
					}
					max = GetActualMaximum(field);

					// value: the new WEEK_OF_MONTH value
					int value = GetRolledValue(InternalGet(field), amount, 1, max) - 1;

					// nfd: fixed date of the rolled date
					long nfd = monthDay1st + value * 7 + dow;

					// Unlike WEEK_OF_YEAR, we need to change day of week if the
					// nfd is out of the month.
					if (nfd < month1)
					{
						nfd = month1;
					}
					else if (nfd >= (month1 + monthLength))
					{
						nfd = month1 + monthLength - 1;
					}
					int dayOfMonth;
					if (isCutoverYear)
					{
						// If we are in the cutover year, convert nfd to
						// its calendar date and use dayOfMonth.
						BaseCalendar.Date d = GetCalendarDate(nfd);
						dayOfMonth = d.DayOfMonth;
					}
					else
					{
						dayOfMonth = (int)(nfd - month1) + 1;
					}
					Set(DAY_OF_MONTH, dayOfMonth);
					return;
			}

			case DAY_OF_MONTH:
			{
					if (!IsCutoverYear(Cdate.NormalizedYear))
					{
						max = Calsys.getMonthLength(Cdate);
						break;
					}

					// Cutover year handling
					long fd = CurrentFixedDate;
					long month1 = GetFixedDateMonth1(Cdate, fd);
					// It may not be a regular month. Convert the date and range to
					// the relative values, perform the roll, and
					// convert the result back to the rolled date.
					int value = GetRolledValue((int)(fd - month1), amount, 0, ActualMonthLength() - 1);
					BaseCalendar.Date d = GetCalendarDate(month1 + value);
					Debug.Assert(d.Month - 1 == InternalGet(MONTH));
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case DAY_OF_YEAR:
			{
					max = GetActualMaximum(field);
					if (!IsCutoverYear(Cdate.NormalizedYear))
					{
						break;
					}

					// Handle cutover here.
					long fd = CurrentFixedDate;
					long jan1 = fd - InternalGet(DAY_OF_YEAR) + 1;
					int value = GetRolledValue((int)(fd - jan1) + 1, amount, min, max);
					BaseCalendar.Date d = GetCalendarDate(jan1 + value - 1);
					Set(MONTH, d.Month - 1);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case DAY_OF_WEEK:
			{
					if (!IsCutoverYear(Cdate.NormalizedYear))
					{
						// If the week of year is in the same year, we can
						// just change DAY_OF_WEEK.
						int weekOfYear = InternalGet(WEEK_OF_YEAR);
						if (weekOfYear > 1 && weekOfYear < 52)
						{
							Set(WEEK_OF_YEAR, weekOfYear); // update stamp[WEEK_OF_YEAR]
							max = SATURDAY;
							break;
						}
					}

					// We need to handle it in a different way around year
					// boundaries and in the cutover year. Note that
					// changing era and year values violates the roll
					// rule: not changing larger calendar fields...
					amount %= 7;
					if (amount == 0)
					{
						return;
					}
					long fd = CurrentFixedDate;
					long dowFirst = BaseCalendar.getDayOfWeekDateOnOrBefore(fd, FirstDayOfWeek);
					fd += amount;
					if (fd < dowFirst)
					{
						fd += 7;
					}
					else if (fd >= dowFirst + 7)
					{
						fd -= 7;
					}
					BaseCalendar.Date d = GetCalendarDate(fd);
					Set(ERA, (d.NormalizedYear <= 0 ? BCE : CE));
					Set(d.Year, d.Month - 1, d.DayOfMonth);
					return;
			}

			case DAY_OF_WEEK_IN_MONTH:
			{
					min = 1; // after normalized, min should be 1.
					if (!IsCutoverYear(Cdate.NormalizedYear))
					{
						int dom = InternalGet(DAY_OF_MONTH);
						int monthLength = Calsys.getMonthLength(Cdate);
						int lastDays = monthLength % 7;
						max = monthLength / 7;
						int x = (dom - 1) % 7;
						if (x < lastDays)
						{
							max++;
						}
						Set(DAY_OF_WEEK, InternalGet(DAY_OF_WEEK));
						break;
					}

					// Cutover year handling
					long fd = CurrentFixedDate;
					long month1 = GetFixedDateMonth1(Cdate, fd);
					int monthLength = ActualMonthLength();
					int lastDays = monthLength % 7;
					max = monthLength / 7;
					int x = (int)(fd - month1) % 7;
					if (x < lastDays)
					{
						max++;
					}
					int value = GetRolledValue(InternalGet(field), amount, min, max) - 1;
					fd = month1 + value * 7 + x;
					BaseCalendar cal = (fd >= GregorianCutoverDate_Renamed) ? Gcal : JulianCalendarSystem;
					BaseCalendar.Date d = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.NO_TIMEZONE);
					cal.getCalendarDateFromFixedDate(d, fd);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}
			}

			Set(field, GetRolledValue(InternalGet(field), amount, min, max));
		}

		/// <summary>
		/// Returns the minimum value for the given calendar field of this
		/// <code>GregorianCalendar</code> instance. The minimum value is
		/// defined as the smallest value returned by the {@link
		/// Calendar#get(int) get} method for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the minimum value for the given calendar field. </returns>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public override int GetMinimum(int field)
		{
			return MIN_VALUES[field];
		}

		/// <summary>
		/// Returns the maximum value for the given calendar field of this
		/// <code>GregorianCalendar</code> instance. The maximum value is
		/// defined as the largest value returned by the {@link
		/// Calendar#get(int) get} method for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the maximum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public override int GetMaximum(int field)
		{
			switch (field)
			{
			case MONTH:
			case DAY_OF_MONTH:
			case DAY_OF_YEAR:
			case WEEK_OF_YEAR:
			case WEEK_OF_MONTH:
			case DAY_OF_WEEK_IN_MONTH:
			case YEAR:
			{
					// On or after Gregorian 200-3-1, Julian and Gregorian
					// calendar dates are the same or Gregorian dates are
					// larger (i.e., there is a "gap") after 300-3-1.
					if (GregorianCutoverYear > 200)
					{
						break;
					}
					// There might be "overlapping" dates.
					GregorianCalendar gc = (GregorianCalendar) Clone();
					gc.Lenient = true;
					gc.TimeInMillis = GregorianCutover;
					int v1 = gc.GetActualMaximum(field);
					gc.TimeInMillis = GregorianCutover - 1;
					int v2 = gc.GetActualMaximum(field);
					return System.Math.Max(MAX_VALUES[field], System.Math.Max(v1, v2));
			}
			}
			return MAX_VALUES[field];
		}

		/// <summary>
		/// Returns the highest minimum value for the given calendar field
		/// of this <code>GregorianCalendar</code> instance. The highest
		/// minimum value is defined as the largest value returned by
		/// <seealso cref="#getActualMinimum(int)"/> for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <returns> the highest minimum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public override int GetGreatestMinimum(int field)
		{
			if (field == DAY_OF_MONTH)
			{
				BaseCalendar.Date d = GregorianCutoverDate;
				long mon1 = GetFixedDateMonth1(d, GregorianCutoverDate_Renamed);
				d = GetCalendarDate(mon1);
				return System.Math.Max(MIN_VALUES[field], d.DayOfMonth);
			}
			return MIN_VALUES[field];
		}

		/// <summary>
		/// Returns the lowest maximum value for the given calendar field
		/// of this <code>GregorianCalendar</code> instance. The lowest
		/// maximum value is defined as the smallest value returned by
		/// <seealso cref="#getActualMaximum(int)"/> for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the lowest maximum value for the given calendar field. </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public override int GetLeastMaximum(int field)
		{
			switch (field)
			{
			case MONTH:
			case DAY_OF_MONTH:
			case DAY_OF_YEAR:
			case WEEK_OF_YEAR:
			case WEEK_OF_MONTH:
			case DAY_OF_WEEK_IN_MONTH:
			case YEAR:
			{
					GregorianCalendar gc = (GregorianCalendar) Clone();
					gc.Lenient = true;
					gc.TimeInMillis = GregorianCutover;
					int v1 = gc.GetActualMaximum(field);
					gc.TimeInMillis = GregorianCutover - 1;
					int v2 = gc.GetActualMaximum(field);
					return System.Math.Min(LEAST_MAX_VALUES[field], System.Math.Min(v1, v2));
			}
			}
			return LEAST_MAX_VALUES[field];
		}

		/// <summary>
		/// Returns the minimum value that this calendar field could have,
		/// taking into consideration the given time value and the current
		/// values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// 
		/// <para>For example, if the Gregorian change date is January 10,
		/// 1970 and the date of this <code>GregorianCalendar</code> is
		/// January 20, 1970, the actual minimum value of the
		/// <code>DAY_OF_MONTH</code> field is 10 because the previous date
		/// of January 10, 1970 is December 27, 1996 (in the Julian
		/// calendar). Therefore, December 28, 1969 to January 9, 1970
		/// don't exist.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the minimum of the given field for the time value of
		/// this <code>GregorianCalendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int)
		/// @since 1.2 </seealso>
		public override int GetActualMinimum(int field)
		{
			if (field == DAY_OF_MONTH)
			{
				GregorianCalendar gc = NormalizedCalendar;
				int year = gc.Cdate.NormalizedYear;
				if (year == GregorianCutoverYear || year == GregorianCutoverYearJulian)
				{
					long month1 = GetFixedDateMonth1(gc.Cdate, gc.Calsys.getFixedDate(gc.Cdate));
					BaseCalendar.Date d = GetCalendarDate(month1);
					return d.DayOfMonth;
				}
			}
			return GetMinimum(field);
		}

		/// <summary>
		/// Returns the maximum value that this calendar field could have,
		/// taking into consideration the given time value and the current
		/// values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// <seealso cref="#getGregorianChange() getGregorianChange"/> and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// For example, if the date of this instance is February 1, 2004,
		/// the actual maximum value of the <code>DAY_OF_MONTH</code> field
		/// is 29 because 2004 is a leap year, and if the date of this
		/// instance is February 1, 2005, it's 28.
		/// 
		/// <para>This method calculates the maximum value of {@link
		/// Calendar#WEEK_OF_YEAR WEEK_OF_YEAR} based on the {@link
		/// Calendar#YEAR YEAR} (calendar year) value, not the <a
		/// href="#week_year">week year</a>. Call {@link
		/// #getWeeksInWeekYear()} to get the maximum value of {@code
		/// WEEK_OF_YEAR} in the week year of this {@code GregorianCalendar}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the maximum of the given field for the time value of
		/// this <code>GregorianCalendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int)
		/// @since 1.2 </seealso>
		public override int GetActualMaximum(int field)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fieldsForFixedMax = ERA_MASK|DAY_OF_WEEK_MASK|HOUR_MASK|AM_PM_MASK| HOUR_OF_DAY_MASK|MINUTE_MASK|SECOND_MASK|MILLISECOND_MASK| ZONE_OFFSET_MASK|DST_OFFSET_MASK;
			int fieldsForFixedMax = ERA_MASK | DAY_OF_WEEK_MASK | HOUR_MASK | AM_PM_MASK | HOUR_OF_DAY_MASK | MINUTE_MASK | SECOND_MASK | MILLISECOND_MASK | ZONE_OFFSET_MASK | DST_OFFSET_MASK;
			if ((fieldsForFixedMax & (1 << field)) != 0)
			{
				return GetMaximum(field);
			}

			GregorianCalendar gc = NormalizedCalendar;
			BaseCalendar.Date date = gc.Cdate;
			BaseCalendar cal = gc.Calsys;
			int normalizedYear = date.NormalizedYear;

			int value = -1;
			switch (field)
			{
			case MONTH:
			{
					if (!gc.IsCutoverYear(normalizedYear))
					{
						value = DECEMBER;
						break;
					}

					// January 1 of the next year may or may not exist.
					long nextJan1;
					do
					{
						nextJan1 = Gcal.getFixedDate(++normalizedYear, BaseCalendar.JANUARY, 1, null);
					} while (nextJan1 < GregorianCutoverDate_Renamed);
					BaseCalendar.Date d = (BaseCalendar.Date) date.clone();
					cal.getCalendarDateFromFixedDate(d, nextJan1 - 1);
					value = d.Month - 1;
			}
				break;

			case DAY_OF_MONTH:
			{
					value = cal.getMonthLength(date);
					if (!gc.IsCutoverYear(normalizedYear) || date.DayOfMonth == value)
					{
						break;
					}

					// Handle cutover year.
					long fd = gc.CurrentFixedDate;
					if (fd >= GregorianCutoverDate_Renamed)
					{
						break;
					}
					int monthLength = gc.ActualMonthLength();
					long monthEnd = gc.GetFixedDateMonth1(gc.Cdate, fd) + monthLength - 1;
					// Convert the fixed date to its calendar date.
					BaseCalendar.Date d = gc.GetCalendarDate(monthEnd);
					value = d.DayOfMonth;
			}
				break;

			case DAY_OF_YEAR:
			{
					if (!gc.IsCutoverYear(normalizedYear))
					{
						value = cal.getYearLength(date);
						break;
					}

					// Handle cutover year.
					long jan1;
					if (GregorianCutoverYear == GregorianCutoverYearJulian)
					{
						BaseCalendar cocal = gc.CutoverCalendarSystem;
						jan1 = cocal.getFixedDate(normalizedYear, 1, 1, null);
					}
					else if (normalizedYear == GregorianCutoverYearJulian)
					{
						jan1 = cal.getFixedDate(normalizedYear, 1, 1, null);
					}
					else
					{
						jan1 = GregorianCutoverDate_Renamed;
					}
					// January 1 of the next year may or may not exist.
					long nextJan1 = Gcal.getFixedDate(++normalizedYear, 1, 1, null);
					if (nextJan1 < GregorianCutoverDate_Renamed)
					{
						nextJan1 = GregorianCutoverDate_Renamed;
					}
					Debug.Assert(jan1 <= cal.getFixedDate(date.NormalizedYear, date.Month, date.DayOfMonth, date));
					Debug.Assert(nextJan1 >= cal.getFixedDate(date.NormalizedYear, date.Month, date.DayOfMonth, date));
					value = (int)(nextJan1 - jan1);
			}
				break;

			case WEEK_OF_YEAR:
			{
					if (!gc.IsCutoverYear(normalizedYear))
					{
						// Get the day of week of January 1 of the year
						CalendarDate d = cal.newCalendarDate(TimeZone.NO_TIMEZONE);
						d.setDate(date.Year, BaseCalendar.JANUARY, 1);
						int dayOfWeek = cal.getDayOfWeek(d);
						// Normalize the day of week with the firstDayOfWeek value
						dayOfWeek -= FirstDayOfWeek;
						if (dayOfWeek < 0)
						{
							dayOfWeek += 7;
						}
						value = 52;
						int magic = dayOfWeek + MinimalDaysInFirstWeek - 1;
						if ((magic == 6) || (date.LeapYear && (magic == 5 || magic == 12)))
						{
							value++;
						}
						break;
					}

					if (gc == this)
					{
						gc = (GregorianCalendar) gc.Clone();
					}
					int maxDayOfYear = GetActualMaximum(DAY_OF_YEAR);
					gc.Set(DAY_OF_YEAR, maxDayOfYear);
					value = gc.Get(WEEK_OF_YEAR);
					if (InternalGet(YEAR) != gc.WeekYear)
					{
						gc.Set(DAY_OF_YEAR, maxDayOfYear - 7);
						value = gc.Get(WEEK_OF_YEAR);
					}
			}
				break;

			case WEEK_OF_MONTH:
			{
					if (!gc.IsCutoverYear(normalizedYear))
					{
						CalendarDate d = cal.newCalendarDate(null);
						d.setDate(date.Year, date.Month, 1);
						int dayOfWeek = cal.getDayOfWeek(d);
						int monthLength = cal.getMonthLength(d);
						dayOfWeek -= FirstDayOfWeek;
						if (dayOfWeek < 0)
						{
							dayOfWeek += 7;
						}
						int nDaysFirstWeek = 7 - dayOfWeek; // # of days in the first week
						value = 3;
						if (nDaysFirstWeek >= MinimalDaysInFirstWeek)
						{
							value++;
						}
						monthLength -= nDaysFirstWeek + 7 * 3;
						if (monthLength > 0)
						{
							value++;
							if (monthLength > 7)
							{
								value++;
							}
						}
						break;
					}

					// Cutover year handling
					if (gc == this)
					{
						gc = (GregorianCalendar) gc.Clone();
					}
					int y = gc.InternalGet(YEAR);
					int m = gc.InternalGet(MONTH);
					do
					{
						value = gc.Get(WEEK_OF_MONTH);
						gc.Add(WEEK_OF_MONTH, +1);
					} while (gc.Get(YEAR) == y && gc.Get(MONTH) == m);
			}
				break;

			case DAY_OF_WEEK_IN_MONTH:
			{
					// may be in the Gregorian cutover month
					int ndays, dow1;
					int dow = date.DayOfWeek;
					if (!gc.IsCutoverYear(normalizedYear))
					{
						BaseCalendar.Date d = (BaseCalendar.Date) date.clone();
						ndays = cal.getMonthLength(d);
						d.DayOfMonth = 1;
						cal.normalize(d);
						dow1 = d.DayOfWeek;
					}
					else
					{
						// Let a cloned GregorianCalendar take care of the cutover cases.
						if (gc == this)
						{
							gc = (GregorianCalendar) Clone();
						}
						ndays = gc.ActualMonthLength();
						gc.Set(DAY_OF_MONTH, gc.GetActualMinimum(DAY_OF_MONTH));
						dow1 = gc.Get(DAY_OF_WEEK);
					}
					int x = dow - dow1;
					if (x < 0)
					{
						x += 7;
					}
					ndays -= x;
					value = (ndays + 6) / 7;
			}
				break;

			case YEAR:
				/* The year computation is no different, in principle, from the
				 * others, however, the range of possible maxima is large.  In
				 * addition, the way we know we've exceeded the range is different.
				 * For these reasons, we use the special case code below to handle
				 * this field.
				 *
				 * The actual maxima for YEAR depend on the type of calendar:
				 *
				 *     Gregorian = May 17, 292275056 BCE - Aug 17, 292278994 CE
				 *     Julian    = Dec  2, 292269055 BCE - Jan  3, 292272993 CE
				 *     Hybrid    = Dec  2, 292269055 BCE - Aug 17, 292278994 CE
				 *
				 * We know we've exceeded the maximum when either the month, date,
				 * time, or era changes in response to setting the year.  We don't
				 * check for month, date, and time here because the year and era are
				 * sufficient to detect an invalid year setting.  NOTE: If code is
				 * added to check the month and date in the future for some reason,
				 * Feb 29 must be allowed to shift to Mar 1 when setting the year.
				 */
			{
					if (gc == this)
					{
						gc = (GregorianCalendar) Clone();
					}

					// Calculate the millisecond offset from the beginning
					// of the year of this calendar and adjust the max
					// year value if we are beyond the limit in the max
					// year.
					long current = gc.YearOffsetInMillis;

					if (gc.InternalGetEra() == CE)
					{
						gc.TimeInMillis = Long.MaxValue;
						value = gc.Get(YEAR);
						long maxEnd = gc.YearOffsetInMillis;
						if (current > maxEnd)
						{
							value--;
						}
					}
					else
					{
						CalendarSystem mincal = gc.TimeInMillis >= GregorianCutover ? Gcal : JulianCalendarSystem;
						CalendarDate d = mincal.getCalendarDate(Long.MinValue, Zone);
						long maxEnd = (cal.getDayOfYear(d) - 1) * 24 + d.Hours;
						maxEnd *= 60;
						maxEnd += d.Minutes;
						maxEnd *= 60;
						maxEnd += d.Seconds;
						maxEnd *= 1000;
						maxEnd += d.Millis;
						value = d.Year;
						if (value <= 0)
						{
							Debug.Assert(mincal == Gcal);
							value = 1 - value;
						}
						if (current < maxEnd)
						{
							value--;
						}
					}
			}
				break;

			default:
				throw new ArrayIndexOutOfBoundsException(field);
			}
			return value;
		}

		/// <summary>
		/// Returns the millisecond offset from the beginning of this
		/// year. This Calendar object must have been normalized.
		/// </summary>
		private long YearOffsetInMillis
		{
			get
			{
				long t = (InternalGet(DAY_OF_YEAR) - 1) * 24;
				t += InternalGet(HOUR_OF_DAY);
				t *= 60;
				t += InternalGet(MINUTE);
				t *= 60;
				t += InternalGet(SECOND);
				t *= 1000;
				return t + InternalGet(MILLISECOND) - (InternalGet(ZONE_OFFSET) + InternalGet(DST_OFFSET));
			}
		}

		public override Object Clone()
		{
			GregorianCalendar other = (GregorianCalendar) base.Clone();

			other.Gdate = (BaseCalendar.Date) Gdate.clone();
			if (Cdate != null)
			{
				if (Cdate != Gdate)
				{
					other.Cdate = (BaseCalendar.Date) Cdate.clone();
				}
				else
				{
					other.Cdate = other.Gdate;
				}
			}
			other.OriginalFields = null;
			other.ZoneOffsets = null;
			return other;
		}

		public override TimeZone TimeZone
		{
			get
			{
				TimeZone zone = base.TimeZone;
				// To share the zone by CalendarDates
				Gdate.Zone = zone;
				if (Cdate != null && Cdate != Gdate)
				{
					Cdate.Zone = zone;
				}
				return zone;
			}
			set
			{
				base.TimeZone = value;
				// To share the value by CalendarDates
				Gdate.Zone = value;
				if (Cdate != null && Cdate != Gdate)
				{
					Cdate.Zone = value;
				}
			}
		}


		/// <summary>
		/// Returns {@code true} indicating this {@code GregorianCalendar}
		/// supports week dates.
		/// </summary>
		/// <returns> {@code true} (always) </returns>
		/// <seealso cref= #getWeekYear() </seealso>
		/// <seealso cref= #setWeekDate(int,int,int) </seealso>
		/// <seealso cref= #getWeeksInWeekYear()
		/// @since 1.7 </seealso>
		public override sealed bool WeekDateSupported
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns the <a href="#week_year">week year</a> represented by this
		/// {@code GregorianCalendar}. The dates in the weeks between 1 and the
		/// maximum week number of the week year have the same week year value
		/// that may be one year before or after the <seealso cref="Calendar#YEAR YEAR"/>
		/// (calendar year) value.
		/// 
		/// <para>This method calls <seealso cref="Calendar#complete()"/> before
		/// calculating the week year.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the week year represented by this {@code GregorianCalendar}.
		///         If the <seealso cref="Calendar#ERA ERA"/> value is <seealso cref="#BC"/>, the year is
		///         represented by 0 or a negative number: BC 1 is 0, BC 2
		///         is -1, BC 3 is -2, and so on. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if any of the calendar fields is invalid in non-lenient mode. </exception>
		/// <seealso cref= #isWeekDateSupported() </seealso>
		/// <seealso cref= #getWeeksInWeekYear() </seealso>
		/// <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
		/// <seealso cref= Calendar#getMinimalDaysInFirstWeek()
		/// @since 1.7 </seealso>
		public override int WeekYear
		{
			get
			{
				int year = Get(YEAR); // implicitly calls complete()
				if (InternalGetEra() == BCE)
				{
					year = 1 - year;
				}
    
				// Fast path for the Gregorian calendar years that are never
				// affected by the Julian-Gregorian transition
				if (year > GregorianCutoverYear + 1)
				{
					int weekOfYear = InternalGet(WEEK_OF_YEAR);
					if (InternalGet(MONTH) == JANUARY)
					{
						if (weekOfYear >= 52)
						{
							--year;
						}
					}
					else
					{
						if (weekOfYear == 1)
						{
							++year;
						}
					}
					return year;
				}
    
				// General (slow) path
				int dayOfYear = InternalGet(DAY_OF_YEAR);
				int maxDayOfYear = GetActualMaximum(DAY_OF_YEAR);
				int minimalDays = MinimalDaysInFirstWeek;
    
				// Quickly check the possibility of year adjustments before
				// cloning this GregorianCalendar.
				if (dayOfYear > minimalDays && dayOfYear < (maxDayOfYear - 6))
				{
					return year;
				}
    
				// Create a clone to work on the calculation
				GregorianCalendar cal = (GregorianCalendar) Clone();
				cal.Lenient = true;
				// Use GMT so that intermediate date calculations won't
				// affect the time of day fields.
				cal.TimeZone = TimeZone.GetTimeZone("GMT");
				// Go to the first day of the year, which is usually January 1.
				cal.Set(DAY_OF_YEAR, 1);
				cal.Complete();
    
				// Get the first day of the first day-of-week in the year.
				int delta = FirstDayOfWeek - cal.Get(DAY_OF_WEEK);
				if (delta != 0)
				{
					if (delta < 0)
					{
						delta += 7;
					}
					cal.Add(DAY_OF_YEAR, delta);
				}
				int minDayOfYear = cal.Get(DAY_OF_YEAR);
				if (dayOfYear < minDayOfYear)
				{
					if (minDayOfYear <= minimalDays)
					{
						--year;
					}
				}
				else
				{
					cal.Set(YEAR, year + 1);
					cal.Set(DAY_OF_YEAR, 1);
					cal.Complete();
					int del = FirstDayOfWeek - cal.Get(DAY_OF_WEEK);
					if (del != 0)
					{
						if (del < 0)
						{
							del += 7;
						}
						cal.Add(DAY_OF_YEAR, del);
					}
					minDayOfYear = cal.Get(DAY_OF_YEAR) - 1;
					if (minDayOfYear == 0)
					{
						minDayOfYear = 7;
					}
					if (minDayOfYear >= minimalDays)
					{
						int days = maxDayOfYear - dayOfYear + 1;
						if (days <= (7 - minDayOfYear))
						{
							++year;
						}
					}
				}
				return year;
			}
		}

		/// <summary>
		/// Sets this {@code GregorianCalendar} to the date given by the
		/// date specifiers - <a href="#week_year">{@code weekYear}</a>,
		/// {@code weekOfYear}, and {@code dayOfWeek}. {@code weekOfYear}
		/// follows the <a href="#week_and_year">{@code WEEK_OF_YEAR}
		/// numbering</a>.  The {@code dayOfWeek} value must be one of the
		/// <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> values: {@link
		/// Calendar#SUNDAY SUNDAY} to <seealso cref="Calendar#SATURDAY SATURDAY"/>.
		/// 
		/// <para>Note that the numeric day-of-week representation differs from
		/// the ISO 8601 standard, and that the {@code weekOfYear}
		/// numbering is compatible with the standard when {@code
		/// getFirstDayOfWeek()} is {@code MONDAY} and {@code
		/// getMinimalDaysInFirstWeek()} is 4.
		/// 
		/// </para>
		/// <para>Unlike the {@code set} method, all of the calendar fields
		/// and the instant of time value are calculated upon return.
		/// 
		/// </para>
		/// <para>If {@code weekOfYear} is out of the valid week-of-year
		/// range in {@code weekYear}, the {@code weekYear}
		/// and {@code weekOfYear} values are adjusted in lenient
		/// mode, or an {@code IllegalArgumentException} is thrown in
		/// non-lenient mode.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weekYear">    the week year </param>
		/// <param name="weekOfYear">  the week number based on {@code weekYear} </param>
		/// <param name="dayOfWeek">   the day of week value: one of the constants
		///                    for the <seealso cref="#DAY_OF_WEEK DAY_OF_WEEK"/> field:
		///                    <seealso cref="Calendar#SUNDAY SUNDAY"/>, ...,
		///                    <seealso cref="Calendar#SATURDAY SATURDAY"/>. </param>
		/// <exception cref="IllegalArgumentException">
		///            if any of the given date specifiers is invalid,
		///            or if any of the calendar fields are inconsistent
		///            with the given date specifiers in non-lenient mode </exception>
		/// <seealso cref= GregorianCalendar#isWeekDateSupported() </seealso>
		/// <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
		/// <seealso cref= Calendar#getMinimalDaysInFirstWeek()
		/// @since 1.7 </seealso>
		public override void SetWeekDate(int weekYear, int weekOfYear, int dayOfWeek)
		{
			if (dayOfWeek < SUNDAY || dayOfWeek > SATURDAY)
			{
				throw new IllegalArgumentException("invalid dayOfWeek: " + dayOfWeek);
			}

			// To avoid changing the time of day fields by date
			// calculations, use a clone with the GMT time zone.
			GregorianCalendar gc = (GregorianCalendar) Clone();
			gc.Lenient = true;
			int era = gc.Get(ERA);
			gc.Clear();
			gc.TimeZone = TimeZone.GetTimeZone("GMT");
			gc.Set(ERA, era);
			gc.Set(YEAR, weekYear);
			gc.Set(WEEK_OF_YEAR, 1);
			gc.Set(DAY_OF_WEEK, FirstDayOfWeek);
			int days = dayOfWeek - FirstDayOfWeek;
			if (days < 0)
			{
				days += 7;
			}
			days += 7 * (weekOfYear - 1);
			if (days != 0)
			{
				gc.Add(DAY_OF_YEAR, days);
			}
			else
			{
				gc.Complete();
			}

			if (!Lenient && (gc.WeekYear != weekYear || gc.InternalGet(WEEK_OF_YEAR) != weekOfYear || gc.InternalGet(DAY_OF_WEEK) != dayOfWeek))
			{
				throw new IllegalArgumentException();
			}

			Set(ERA, gc.InternalGet(ERA));
			Set(YEAR, gc.InternalGet(YEAR));
			Set(MONTH, gc.InternalGet(MONTH));
			Set(DAY_OF_MONTH, gc.InternalGet(DAY_OF_MONTH));

			// to avoid throwing an IllegalArgumentException in
			// non-lenient, set WEEK_OF_YEAR internally
			InternalSet(WEEK_OF_YEAR, weekOfYear);
			Complete();
		}

		/// <summary>
		/// Returns the number of weeks in the <a href="#week_year">week year</a>
		/// represented by this {@code GregorianCalendar}.
		/// 
		/// <para>For example, if this {@code GregorianCalendar}'s date is
		/// December 31, 2008 with <a href="#iso8601_compatible_setting">the ISO
		/// 8601 compatible setting</a>, this method will return 53 for the
		/// period: December 29, 2008 to January 3, 2010 while {@link
		/// #getActualMaximum(int) getActualMaximum(WEEK_OF_YEAR)} will return
		/// 52 for the period: December 31, 2007 to December 28, 2008.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of weeks in the week year. </returns>
		/// <seealso cref= Calendar#WEEK_OF_YEAR </seealso>
		/// <seealso cref= #getWeekYear() </seealso>
		/// <seealso cref= #getActualMaximum(int)
		/// @since 1.7 </seealso>
		public override int WeeksInWeekYear
		{
			get
			{
				GregorianCalendar gc = NormalizedCalendar;
				int weekYear = gc.WeekYear;
				if (weekYear == gc.InternalGet(YEAR))
				{
					return gc.GetActualMaximum(WEEK_OF_YEAR);
				}
    
				// Use the 2nd week for calculating the max of WEEK_OF_YEAR
				if (gc == this)
				{
					gc = (GregorianCalendar) gc.Clone();
				}
				gc.SetWeekDate(weekYear, 2, InternalGet(DAY_OF_WEEK));
				return gc.GetActualMaximum(WEEK_OF_YEAR);
			}
		}

	/////////////////////////////
	// Time => Fields computation
	/////////////////////////////

		/// <summary>
		/// The fixed date corresponding to gdate. If the value is
		/// Long.MIN_VALUE, the fixed date value is unknown. Currently,
		/// Julian calendar dates are not cached.
		/// </summary>
		[NonSerialized]
		private long CachedFixedDate = Long.MinValue;

		/// <summary>
		/// Converts the time value (millisecond offset from the <a
		/// href="Calendar.html#Epoch">Epoch</a>) to calendar field values.
		/// The time is <em>not</em>
		/// recomputed first; to recompute the time, then the fields, call the
		/// <code>complete</code> method.
		/// </summary>
		/// <seealso cref= Calendar#complete </seealso>
		protected internal override void ComputeFields()
		{
			int mask;
			if (PartiallyNormalized)
			{
				// Determine which calendar fields need to be computed.
				mask = SetStateFields;
				int fieldMask = ~mask & ALL_FIELDS;
				// We have to call computTime in case calsys == null in
				// order to set calsys and cdate. (6263644)
				if (fieldMask != 0 || Calsys == null)
				{
					mask |= ComputeFields(fieldMask, mask & (ZONE_OFFSET_MASK | DST_OFFSET_MASK));
					Debug.Assert(mask == ALL_FIELDS);
				}
			}
			else
			{
				mask = ALL_FIELDS;
				ComputeFields(mask, 0);
			}
			// After computing all the fields, set the field state to `COMPUTED'.
			FieldsComputed = mask;
		}

		/// <summary>
		/// This computeFields implements the conversion from UTC
		/// (millisecond offset from the Epoch) to calendar
		/// field values. fieldMask specifies which fields to change the
		/// setting state to COMPUTED, although all fields are set to
		/// the correct values. This is required to fix 4685354.
		/// </summary>
		/// <param name="fieldMask"> a bit mask to specify which fields to change
		/// the setting state. </param>
		/// <param name="tzMask"> a bit mask to specify which time zone offset
		/// fields to be used for time calculations </param>
		/// <returns> a new field mask that indicates what field values have
		/// actually been set. </returns>
		private int ComputeFields(int fieldMask, int tzMask)
		{
			int zoneOffset = 0;
			TimeZone tz = Zone;
			if (ZoneOffsets == null)
			{
				ZoneOffsets = new int[2];
			}
			if (tzMask != (ZONE_OFFSET_MASK | DST_OFFSET_MASK))
			{
				if (tz is ZoneInfo)
				{
					zoneOffset = ((ZoneInfo)tz).getOffsets(Time_Renamed, ZoneOffsets);
				}
				else
				{
					zoneOffset = tz.GetOffset(Time_Renamed);
					ZoneOffsets[0] = tz.RawOffset;
					ZoneOffsets[1] = zoneOffset - ZoneOffsets[0];
				}
			}
			if (tzMask != 0)
			{
				if (IsFieldSet(tzMask, ZONE_OFFSET))
				{
					ZoneOffsets[0] = InternalGet(ZONE_OFFSET);
				}
				if (IsFieldSet(tzMask, DST_OFFSET))
				{
					ZoneOffsets[1] = InternalGet(DST_OFFSET);
				}
				zoneOffset = ZoneOffsets[0] + ZoneOffsets[1];
			}

			// By computing time and zoneOffset separately, we can take
			// the wider range of time+zoneOffset than the previous
			// implementation.
			long fixedDate = zoneOffset / ONE_DAY;
			int timeOfDay = zoneOffset % (int)ONE_DAY;
			fixedDate += Time_Renamed / ONE_DAY;
			timeOfDay += (int)(Time_Renamed % ONE_DAY);
			if (timeOfDay >= ONE_DAY)
			{
				timeOfDay -= (int)ONE_DAY;
				++fixedDate;
			}
			else
			{
				while (timeOfDay < 0)
				{
					timeOfDay += (int)ONE_DAY;
					--fixedDate;
				}
			}
			fixedDate += EPOCH_OFFSET;

			int era = CE;
			int year;
			if (fixedDate >= GregorianCutoverDate_Renamed)
			{
				// Handle Gregorian dates.
				Debug.Assert(CachedFixedDate == Long.MinValue || Gdate.Normalized, "cache control: not normalized");
				Debug.Assert(CachedFixedDate == Long.MinValue || Gcal.getFixedDate(Gdate.NormalizedYear, Gdate.Month, Gdate.DayOfMonth, Gdate) == CachedFixedDate, "cache control: inconsictency" + ", cachedFixedDate=" + CachedFixedDate + ", computed=" + Gcal.getFixedDate(Gdate.NormalizedYear, Gdate.Month, Gdate.DayOfMonth, Gdate) + ", date=" + Gdate);

				// See if we can use gdate to avoid date calculation.
				if (fixedDate != CachedFixedDate)
				{
					Gcal.getCalendarDateFromFixedDate(Gdate, fixedDate);
					CachedFixedDate = fixedDate;
				}

				year = Gdate.Year;
				if (year <= 0)
				{
					year = 1 - year;
					era = BCE;
				}
				Calsys = Gcal;
				Cdate = Gdate;
				Debug.Assert(Cdate.DayOfWeek > 0, "dow=" + Cdate.DayOfWeek + ", date=" + Cdate);
			}
			else
			{
				// Handle Julian calendar dates.
				Calsys = JulianCalendarSystem;
				Cdate = (BaseCalendar.Date) Jcal.newCalendarDate(Zone);
				Jcal.getCalendarDateFromFixedDate(Cdate, fixedDate);
				Era e = Cdate.Era;
				if (e == Jeras[0])
				{
					era = BCE;
				}
				year = Cdate.Year;
			}

			// Always set the ERA and YEAR values.
			InternalSet(ERA, era);
			InternalSet(YEAR, year);
			int mask = fieldMask | (ERA_MASK | YEAR_MASK);

			int month = Cdate.Month - 1; // 0-based
			int dayOfMonth = Cdate.DayOfMonth;

			// Set the basic date fields.
			if ((fieldMask & (MONTH_MASK | DAY_OF_MONTH_MASK | DAY_OF_WEEK_MASK)) != 0)
			{
				InternalSet(MONTH, month);
				InternalSet(DAY_OF_MONTH, dayOfMonth);
				InternalSet(DAY_OF_WEEK, Cdate.DayOfWeek);
				mask |= MONTH_MASK | DAY_OF_MONTH_MASK | DAY_OF_WEEK_MASK;
			}

			if ((fieldMask & (HOUR_OF_DAY_MASK | AM_PM_MASK | HOUR_MASK | MINUTE_MASK | SECOND_MASK | MILLISECOND_MASK)) != 0)
			{
				if (timeOfDay != 0)
				{
					int hours = timeOfDay / ONE_HOUR;
					InternalSet(HOUR_OF_DAY, hours);
					InternalSet(AM_PM, hours / 12); // Assume AM == 0
					InternalSet(HOUR, hours % 12);
					int r = timeOfDay % ONE_HOUR;
					InternalSet(MINUTE, r / ONE_MINUTE);
					r %= ONE_MINUTE;
					InternalSet(SECOND, r / ONE_SECOND);
					InternalSet(MILLISECOND, r % ONE_SECOND);
				}
				else
				{
					InternalSet(HOUR_OF_DAY, 0);
					InternalSet(AM_PM, AM);
					InternalSet(HOUR, 0);
					InternalSet(MINUTE, 0);
					InternalSet(SECOND, 0);
					InternalSet(MILLISECOND, 0);
				}
				mask |= (HOUR_OF_DAY_MASK | AM_PM_MASK | HOUR_MASK | MINUTE_MASK | SECOND_MASK | MILLISECOND_MASK);
			}

			if ((fieldMask & (ZONE_OFFSET_MASK | DST_OFFSET_MASK)) != 0)
			{
				InternalSet(ZONE_OFFSET, ZoneOffsets[0]);
				InternalSet(DST_OFFSET, ZoneOffsets[1]);
				mask |= (ZONE_OFFSET_MASK | DST_OFFSET_MASK);
			}

			if ((fieldMask & (DAY_OF_YEAR_MASK | WEEK_OF_YEAR_MASK | WEEK_OF_MONTH_MASK | DAY_OF_WEEK_IN_MONTH_MASK)) != 0)
			{
				int normalizedYear = Cdate.NormalizedYear;
				long fixedDateJan1 = Calsys.getFixedDate(normalizedYear, 1, 1, Cdate);
				int dayOfYear = (int)(fixedDate - fixedDateJan1) + 1;
				long fixedDateMonth1 = fixedDate - dayOfMonth + 1;
				int cutoverGap = 0;
				int cutoverYear = (Calsys == Gcal) ? GregorianCutoverYear : GregorianCutoverYearJulian;
				int relativeDayOfMonth = dayOfMonth - 1;

				// If we are in the cutover year, we need some special handling.
				if (normalizedYear == cutoverYear)
				{
					// Need to take care of the "missing" days.
					if (GregorianCutoverYearJulian <= GregorianCutoverYear)
					{
						// We need to find out where we are. The cutover
						// gap could even be more than one year.  (One
						// year difference in ~48667 years.)
						fixedDateJan1 = GetFixedDateJan1(Cdate, fixedDate);
						if (fixedDate >= GregorianCutoverDate_Renamed)
						{
							fixedDateMonth1 = GetFixedDateMonth1(Cdate, fixedDate);
						}
					}
					int realDayOfYear = (int)(fixedDate - fixedDateJan1) + 1;
					cutoverGap = dayOfYear - realDayOfYear;
					dayOfYear = realDayOfYear;
					relativeDayOfMonth = (int)(fixedDate - fixedDateMonth1);
				}
				InternalSet(DAY_OF_YEAR, dayOfYear);
				InternalSet(DAY_OF_WEEK_IN_MONTH, relativeDayOfMonth / 7 + 1);

				int weekOfYear = GetWeekNumber(fixedDateJan1, fixedDate);

				// The spec is to calculate WEEK_OF_YEAR in the
				// ISO8601-style. This creates problems, though.
				if (weekOfYear == 0)
				{
					// If the date belongs to the last week of the
					// previous year, use the week number of "12/31" of
					// the "previous" year. Again, if the previous year is
					// the Gregorian cutover year, we need to take care of
					// it.  Usually the previous day of January 1 is
					// December 31, which is not always true in
					// GregorianCalendar.
					long fixedDec31 = fixedDateJan1 - 1;
					long prevJan1 = fixedDateJan1 - 365;
					if (normalizedYear > (cutoverYear + 1))
					{
						if (CalendarUtils.isGregorianLeapYear(normalizedYear - 1))
						{
							--prevJan1;
						}
					}
					else if (normalizedYear <= GregorianCutoverYearJulian)
					{
						if (CalendarUtils.isJulianLeapYear(normalizedYear - 1))
						{
							--prevJan1;
						}
					}
					else
					{
						BaseCalendar calForJan1 = Calsys;
						//int prevYear = normalizedYear - 1;
						int prevYear = GetCalendarDate(fixedDec31).NormalizedYear;
						if (prevYear == GregorianCutoverYear)
						{
							calForJan1 = CutoverCalendarSystem;
							if (calForJan1 == Jcal)
							{
								prevJan1 = calForJan1.getFixedDate(prevYear, BaseCalendar.JANUARY, 1, null);
							}
							else
							{
								prevJan1 = GregorianCutoverDate_Renamed;
								calForJan1 = Gcal;
							}
						}
						else if (prevYear <= GregorianCutoverYearJulian)
						{
							calForJan1 = JulianCalendarSystem;
							prevJan1 = calForJan1.getFixedDate(prevYear, BaseCalendar.JANUARY, 1, null);
						}
					}
					weekOfYear = GetWeekNumber(prevJan1, fixedDec31);
				}
				else
				{
					if (normalizedYear > GregorianCutoverYear || normalizedYear < (GregorianCutoverYearJulian - 1))
					{
						// Regular years
						if (weekOfYear >= 52)
						{
							long nextJan1 = fixedDateJan1 + 365;
							if (Cdate.LeapYear)
							{
								nextJan1++;
							}
							long nextJan1st = BaseCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, FirstDayOfWeek);
							int ndays = (int)(nextJan1st - nextJan1);
							if (ndays >= MinimalDaysInFirstWeek && fixedDate >= (nextJan1st - 7))
							{
								// The first days forms a week in which the date is included.
								weekOfYear = 1;
							}
						}
					}
					else
					{
						BaseCalendar calForJan1 = Calsys;
						int nextYear = normalizedYear + 1;
						if (nextYear == (GregorianCutoverYearJulian + 1) && nextYear < GregorianCutoverYear)
						{
							// In case the gap is more than one year.
							nextYear = GregorianCutoverYear;
						}
						if (nextYear == GregorianCutoverYear)
						{
							calForJan1 = CutoverCalendarSystem;
						}

						long nextJan1;
						if (nextYear > GregorianCutoverYear || GregorianCutoverYearJulian == GregorianCutoverYear || nextYear == GregorianCutoverYearJulian)
						{
							nextJan1 = calForJan1.getFixedDate(nextYear, BaseCalendar.JANUARY, 1, null);
						}
						else
						{
							nextJan1 = GregorianCutoverDate_Renamed;
							calForJan1 = Gcal;
						}

						long nextJan1st = BaseCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, FirstDayOfWeek);
						int ndays = (int)(nextJan1st - nextJan1);
						if (ndays >= MinimalDaysInFirstWeek && fixedDate >= (nextJan1st - 7))
						{
							// The first days forms a week in which the date is included.
							weekOfYear = 1;
						}
					}
				}
				InternalSet(WEEK_OF_YEAR, weekOfYear);
				InternalSet(WEEK_OF_MONTH, GetWeekNumber(fixedDateMonth1, fixedDate));
				mask |= (DAY_OF_YEAR_MASK | WEEK_OF_YEAR_MASK | WEEK_OF_MONTH_MASK | DAY_OF_WEEK_IN_MONTH_MASK);
			}
			return mask;
		}

		/// <summary>
		/// Returns the number of weeks in a period between fixedDay1 and
		/// fixedDate. The getFirstDayOfWeek-getMinimalDaysInFirstWeek rule
		/// is applied to calculate the number of weeks.
		/// </summary>
		/// <param name="fixedDay1"> the fixed date of the first day of the period </param>
		/// <param name="fixedDate"> the fixed date of the last day of the period </param>
		/// <returns> the number of weeks of the given period </returns>
		private int GetWeekNumber(long fixedDay1, long fixedDate)
		{
			// We can always use `gcal' since Julian and Gregorian are the
			// same thing for this calculation.
			long fixedDay1st = Gregorian.getDayOfWeekDateOnOrBefore(fixedDay1 + 6, FirstDayOfWeek);
			int ndays = (int)(fixedDay1st - fixedDay1);
			Debug.Assert(ndays <= 7);
			if (ndays >= MinimalDaysInFirstWeek)
			{
				fixedDay1st -= 7;
			}
			int normalizedDayOfPeriod = (int)(fixedDate - fixedDay1st);
			if (normalizedDayOfPeriod >= 0)
			{
				return normalizedDayOfPeriod / 7 + 1;
			}
			return CalendarUtils.floorDivide(normalizedDayOfPeriod, 7) + 1;
		}

		/// <summary>
		/// Converts calendar field values to the time value (millisecond
		/// offset from the <a href="Calendar.html#Epoch">Epoch</a>).
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if any calendar fields are invalid. </exception>
		protected internal override void ComputeTime()
		{
			// In non-lenient mode, perform brief checking of calendar
			// fields which have been set externally. Through this
			// checking, the field values are stored in originalFields[]
			// to see if any of them are normalized later.
			if (!Lenient)
			{
				if (OriginalFields == null)
				{
					OriginalFields = new int[FIELD_COUNT];
				}
				for (int field = 0; field < FIELD_COUNT; field++)
				{
					int value = InternalGet(field);
					if (IsExternallySet(field))
					{
						// Quick validation for any out of range values
						if (value < GetMinimum(field) || value > GetMaximum(field))
						{
							throw new IllegalArgumentException(GetFieldName(field));
						}
					}
					OriginalFields[field] = value;
				}
			}

			// Let the super class determine which calendar fields to be
			// used to calculate the time.
			int fieldMask = SelectFields();

			// The year defaults to the epoch start. We don't check
			// fieldMask for YEAR because YEAR is a mandatory field to
			// determine the date.
			int year = IsSet(YEAR) ? InternalGet(YEAR) : EPOCH_YEAR;

			int era = InternalGetEra();
			if (era == BCE)
			{
				year = 1 - year;
			}
			else if (era != CE)
			{
				// Even in lenient mode we disallow ERA values other than CE & BCE.
				// (The same normalization rule as add()/roll() could be
				// applied here in lenient mode. But this checking is kept
				// unchanged for compatibility as of 1.5.)
				throw new IllegalArgumentException("Invalid era");
			}

			// If year is 0 or negative, we need to set the ERA value later.
			if (year <= 0 && !IsSet(ERA))
			{
				fieldMask |= ERA_MASK;
				FieldsComputed = ERA_MASK;
			}

			// Calculate the time of day. We rely on the convention that
			// an UNSET field has 0.
			long timeOfDay = 0;
			if (IsFieldSet(fieldMask, HOUR_OF_DAY))
			{
				timeOfDay += (long) InternalGet(HOUR_OF_DAY);
			}
			else
			{
				timeOfDay += InternalGet(HOUR);
				// The default value of AM_PM is 0 which designates AM.
				if (IsFieldSet(fieldMask, AM_PM))
				{
					timeOfDay += 12 * InternalGet(AM_PM);
				}
			}
			timeOfDay *= 60;
			timeOfDay += InternalGet(MINUTE);
			timeOfDay *= 60;
			timeOfDay += InternalGet(SECOND);
			timeOfDay *= 1000;
			timeOfDay += InternalGet(MILLISECOND);

			// Convert the time of day to the number of days and the
			// millisecond offset from midnight.
			long fixedDate = timeOfDay / ONE_DAY;
			timeOfDay %= ONE_DAY;
			while (timeOfDay < 0)
			{
				timeOfDay += ONE_DAY;
				--fixedDate;
			}

			{
			// Calculate the fixed date since January 1, 1 (Gregorian).
				long gfd, jfd;
				if (year > GregorianCutoverYear && year > GregorianCutoverYearJulian)
				{
					gfd = fixedDate + GetFixedDate(Gcal, year, fieldMask);
					if (gfd >= GregorianCutoverDate_Renamed)
					{
						fixedDate = gfd;
						goto calculateFixedDateBreak;
					}
					jfd = fixedDate + GetFixedDate(JulianCalendarSystem, year, fieldMask);
				}
				else if (year < GregorianCutoverYear && year < GregorianCutoverYearJulian)
				{
					jfd = fixedDate + GetFixedDate(JulianCalendarSystem, year, fieldMask);
					if (jfd < GregorianCutoverDate_Renamed)
					{
						fixedDate = jfd;
						goto calculateFixedDateBreak;
					}
					gfd = jfd;
				}
				else
				{
					jfd = fixedDate + GetFixedDate(JulianCalendarSystem, year, fieldMask);
					gfd = fixedDate + GetFixedDate(Gcal, year, fieldMask);
				}

				// Now we have to determine which calendar date it is.

				// If the date is relative from the beginning of the year
				// in the Julian calendar, then use jfd;
				if (IsFieldSet(fieldMask, DAY_OF_YEAR) || IsFieldSet(fieldMask, WEEK_OF_YEAR))
				{
					if (GregorianCutoverYear == GregorianCutoverYearJulian)
					{
						fixedDate = jfd;
						goto calculateFixedDateBreak;
					}
					else if (year == GregorianCutoverYear)
					{
						fixedDate = gfd;
						goto calculateFixedDateBreak;
					}
				}

				if (gfd >= GregorianCutoverDate_Renamed)
				{
					if (jfd >= GregorianCutoverDate_Renamed)
					{
						fixedDate = gfd;
					}
					else
					{
						// The date is in an "overlapping" period. No way
						// to disambiguate it. Determine it using the
						// previous date calculation.
						if (Calsys == Gcal || Calsys == null)
						{
							fixedDate = gfd;
						}
						else
						{
							fixedDate = jfd;
						}
					}
				}
				else
				{
					if (jfd < GregorianCutoverDate_Renamed)
					{
						fixedDate = jfd;
					}
					else
					{
						// The date is in a "missing" period.
						if (!Lenient)
						{
							throw new IllegalArgumentException("the specified date doesn't exist");
						}
						// Take the Julian date for compatibility, which
						// will produce a Gregorian date.
						fixedDate = jfd;
					}
				}
			}
			calculateFixedDateBreak:

			// millis represents local wall-clock time in milliseconds.
			long millis = (fixedDate - EPOCH_OFFSET) * ONE_DAY + timeOfDay;

			// Compute the time zone offset and DST offset.  There are two potential
			// ambiguities here.  We'll assume a 2:00 am (wall time) switchover time
			// for discussion purposes here.
			// 1. The transition into DST.  Here, a designated time of 2:00 am - 2:59 am
			//    can be in standard or in DST depending.  However, 2:00 am is an invalid
			//    representation (the representation jumps from 1:59:59 am Std to 3:00:00 am DST).
			//    We assume standard time.
			// 2. The transition out of DST.  Here, a designated time of 1:00 am - 1:59 am
			//    can be in standard or DST.  Both are valid representations (the rep
			//    jumps from 1:59:59 DST to 1:00:00 Std).
			//    Again, we assume standard time.
			// We use the TimeZone object, unless the user has explicitly set the ZONE_OFFSET
			// or DST_OFFSET fields; then we use those fields.
			TimeZone zone = Zone;
			if (ZoneOffsets == null)
			{
				ZoneOffsets = new int[2];
			}
			int tzMask = fieldMask & (ZONE_OFFSET_MASK | DST_OFFSET_MASK);
			if (tzMask != (ZONE_OFFSET_MASK | DST_OFFSET_MASK))
			{
				if (zone is ZoneInfo)
				{
					((ZoneInfo)zone).getOffsetsByWall(millis, ZoneOffsets);
				}
				else
				{
					int gmtOffset = IsFieldSet(fieldMask, ZONE_OFFSET) ? InternalGet(ZONE_OFFSET) : zone.RawOffset;
					zone.GetOffsets(millis - gmtOffset, ZoneOffsets);
				}
			}
			if (tzMask != 0)
			{
				if (IsFieldSet(tzMask, ZONE_OFFSET))
				{
					ZoneOffsets[0] = InternalGet(ZONE_OFFSET);
				}
				if (IsFieldSet(tzMask, DST_OFFSET))
				{
					ZoneOffsets[1] = InternalGet(DST_OFFSET);
				}
			}

			// Adjust the time zone offset values to get the UTC time.
			millis -= ZoneOffsets[0] + ZoneOffsets[1];

			// Set this calendar's time in milliseconds
			Time_Renamed = millis;

			int mask = ComputeFields(fieldMask | SetStateFields, tzMask);

			if (!Lenient)
			{
				for (int field = 0; field < FIELD_COUNT; field++)
				{
					if (!IsExternallySet(field))
					{
						continue;
					}
					if (OriginalFields[field] != InternalGet(field))
					{
						String s = OriginalFields[field] + " -> " + InternalGet(field);
						// Restore the original field values
						System.Array.Copy(OriginalFields, 0, Fields, 0, Fields.Length);
						throw new IllegalArgumentException(GetFieldName(field) + ": " + s);
					}
				}
			}
			FieldsNormalized = mask;
		}

		/// <summary>
		/// Computes the fixed date under either the Gregorian or the
		/// Julian calendar, using the given year and the specified calendar fields.
		/// </summary>
		/// <param name="cal"> the CalendarSystem to be used for the date calculation </param>
		/// <param name="year"> the normalized year number, with 0 indicating the
		/// year 1 BCE, -1 indicating 2 BCE, etc. </param>
		/// <param name="fieldMask"> the calendar fields to be used for the date calculation </param>
		/// <returns> the fixed date </returns>
		/// <seealso cref= Calendar#selectFields </seealso>
		private long GetFixedDate(BaseCalendar cal, int year, int fieldMask)
		{
			int month = JANUARY;
			if (IsFieldSet(fieldMask, MONTH))
			{
				// No need to check if MONTH has been set (no isSet(MONTH)
				// call) since its unset value happens to be JANUARY (0).
				month = InternalGet(MONTH);

				// If the month is out of range, adjust it into range
				if (month > DECEMBER)
				{
					year += month / 12;
					month %= 12;
				}
				else if (month < JANUARY)
				{
					int[] rem = new int[1];
					year += CalendarUtils.floorDivide(month, 12, rem);
					month = rem[0];
				}
			}

			// Get the fixed date since Jan 1, 1 (Gregorian). We are on
			// the first day of either `month' or January in 'year'.
			long fixedDate = cal.getFixedDate(year, month + 1, 1, cal == Gcal ? Gdate : null);
			if (IsFieldSet(fieldMask, MONTH))
			{
				// Month-based calculations
				if (IsFieldSet(fieldMask, DAY_OF_MONTH))
				{
					// We are on the first day of the month. Just add the
					// offset if DAY_OF_MONTH is set. If the isSet call
					// returns false, that means DAY_OF_MONTH has been
					// selected just because of the selected
					// combination. We don't need to add any since the
					// default value is the 1st.
					if (IsSet(DAY_OF_MONTH))
					{
						// To avoid underflow with DAY_OF_MONTH-1, add
						// DAY_OF_MONTH, then subtract 1.
						fixedDate += InternalGet(DAY_OF_MONTH);
						fixedDate--;
					}
				}
				else
				{
					if (IsFieldSet(fieldMask, WEEK_OF_MONTH))
					{
						long firstDayOfWeek = BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate + 6, FirstDayOfWeek);
						// If we have enough days in the first week, then
						// move to the previous week.
						if ((firstDayOfWeek - fixedDate) >= MinimalDaysInFirstWeek)
						{
							firstDayOfWeek -= 7;
						}
						if (IsFieldSet(fieldMask, DAY_OF_WEEK))
						{
							firstDayOfWeek = BaseCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek + 6, InternalGet(DAY_OF_WEEK));
						}
						// In lenient mode, we treat days of the previous
						// months as a part of the specified
						// WEEK_OF_MONTH. See 4633646.
						fixedDate = firstDayOfWeek + 7 * (InternalGet(WEEK_OF_MONTH) - 1);
					}
					else
					{
						int dayOfWeek;
						if (IsFieldSet(fieldMask, DAY_OF_WEEK))
						{
							dayOfWeek = InternalGet(DAY_OF_WEEK);
						}
						else
						{
							dayOfWeek = FirstDayOfWeek;
						}
						// We are basing this on the day-of-week-in-month.  The only
						// trickiness occurs if the day-of-week-in-month is
						// negative.
						int dowim;
						if (IsFieldSet(fieldMask, DAY_OF_WEEK_IN_MONTH))
						{
							dowim = InternalGet(DAY_OF_WEEK_IN_MONTH);
						}
						else
						{
							dowim = 1;
						}
						if (dowim >= 0)
						{
							fixedDate = BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate + (7 * dowim) - 1, dayOfWeek);
						}
						else
						{
							// Go to the first day of the next week of
							// the specified week boundary.
							int lastDate = MonthLength(month, year) + (7 * (dowim + 1));
							// Then, get the day of week date on or before the last date.
							fixedDate = BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate + lastDate - 1, dayOfWeek);
						}
					}
				}
			}
			else
			{
				if (year == GregorianCutoverYear && cal == Gcal && fixedDate < GregorianCutoverDate_Renamed && GregorianCutoverYear != GregorianCutoverYearJulian)
				{
					// January 1 of the year doesn't exist.  Use
					// gregorianCutoverDate as the first day of the
					// year.
					fixedDate = GregorianCutoverDate_Renamed;
				}
				// We are on the first day of the year.
				if (IsFieldSet(fieldMask, DAY_OF_YEAR))
				{
					// Add the offset, then subtract 1. (Make sure to avoid underflow.)
					fixedDate += InternalGet(DAY_OF_YEAR);
					fixedDate--;
				}
				else
				{
					long firstDayOfWeek = BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate + 6, FirstDayOfWeek);
					// If we have enough days in the first week, then move
					// to the previous week.
					if ((firstDayOfWeek - fixedDate) >= MinimalDaysInFirstWeek)
					{
						firstDayOfWeek -= 7;
					}
					if (IsFieldSet(fieldMask, DAY_OF_WEEK))
					{
						int dayOfWeek = InternalGet(DAY_OF_WEEK);
						if (dayOfWeek != FirstDayOfWeek)
						{
							firstDayOfWeek = BaseCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek + 6, dayOfWeek);
						}
					}
					fixedDate = firstDayOfWeek + 7 * ((long)InternalGet(WEEK_OF_YEAR) - 1);
				}
			}

			return fixedDate;
		}

		/// <summary>
		/// Returns this object if it's normalized (all fields and time are
		/// in sync). Otherwise, a cloned object is returned after calling
		/// complete() in lenient mode.
		/// </summary>
		private GregorianCalendar NormalizedCalendar
		{
			get
			{
				GregorianCalendar gc;
				if (FullyNormalized)
				{
					gc = this;
				}
				else
				{
					// Create a clone and normalize the calendar fields
					gc = (GregorianCalendar) this.Clone();
					gc.Lenient = true;
					gc.Complete();
				}
				return gc;
			}
		}

		/// <summary>
		/// Returns the Julian calendar system instance (singleton). 'jcal'
		/// and 'jeras' are set upon the return.
		/// </summary>
		private static BaseCalendar JulianCalendarSystem
		{
			get
			{
				lock (typeof(GregorianCalendar))
				{
					if (Jcal == null)
					{
						Jcal = (JulianCalendar) CalendarSystem.forName("julian");
						Jeras = Jcal.Eras;
					}
					return Jcal;
				}
			}
		}

		/// <summary>
		/// Returns the calendar system for dates before the cutover date
		/// in the cutover year. If the cutover date is January 1, the
		/// method returns Gregorian. Otherwise, Julian.
		/// </summary>
		private BaseCalendar CutoverCalendarSystem
		{
			get
			{
				if (GregorianCutoverYearJulian < GregorianCutoverYear)
				{
					return Gcal;
				}
				return JulianCalendarSystem;
			}
		}

		/// <summary>
		/// Determines if the specified year (normalized) is the Gregorian
		/// cutover year. This object must have been normalized.
		/// </summary>
		private bool IsCutoverYear(int normalizedYear)
		{
			int cutoverYear = (Calsys == Gcal) ? GregorianCutoverYear : GregorianCutoverYearJulian;
			return normalizedYear == cutoverYear;
		}

		/// <summary>
		/// Returns the fixed date of the first day of the year (usually
		/// January 1) before the specified date.
		/// </summary>
		/// <param name="date"> the date for which the first day of the year is
		/// calculated. The date has to be in the cut-over year (Gregorian
		/// or Julian). </param>
		/// <param name="fixedDate"> the fixed date representation of the date </param>
		private long GetFixedDateJan1(BaseCalendar.Date date, long fixedDate)
		{
			Debug.Assert(date.NormalizedYear == GregorianCutoverYear || date.NormalizedYear == GregorianCutoverYearJulian);
			if (GregorianCutoverYear != GregorianCutoverYearJulian)
			{
				if (fixedDate >= GregorianCutoverDate_Renamed)
				{
					// Dates before the cutover date don't exist
					// in the same (Gregorian) year. So, no
					// January 1 exists in the year. Use the
					// cutover date as the first day of the year.
					return GregorianCutoverDate_Renamed;
				}
			}
			// January 1 of the normalized year should exist.
			BaseCalendar juliancal = JulianCalendarSystem;
			return juliancal.getFixedDate(date.NormalizedYear, BaseCalendar.JANUARY, 1, null);
		}

		/// <summary>
		/// Returns the fixed date of the first date of the month (usually
		/// the 1st of the month) before the specified date.
		/// </summary>
		/// <param name="date"> the date for which the first day of the month is
		/// calculated. The date has to be in the cut-over year (Gregorian
		/// or Julian). </param>
		/// <param name="fixedDate"> the fixed date representation of the date </param>
		private long GetFixedDateMonth1(BaseCalendar.Date date, long fixedDate)
		{
			Debug.Assert(date.NormalizedYear == GregorianCutoverYear || date.NormalizedYear == GregorianCutoverYearJulian);
			BaseCalendar.Date gCutover = GregorianCutoverDate;
			if (gCutover.Month == BaseCalendar.JANUARY && gCutover.DayOfMonth == 1)
			{
				// The cutover happened on January 1.
				return fixedDate - date.DayOfMonth + 1;
			}

			long fixedDateMonth1;
			// The cutover happened sometime during the year.
			if (date.Month == gCutover.Month)
			{
				// The cutover happened in the month.
				BaseCalendar.Date jLastDate = LastJulianDate;
				if (GregorianCutoverYear == GregorianCutoverYearJulian && gCutover.Month == jLastDate.Month)
				{
					// The "gap" fits in the same month.
					fixedDateMonth1 = Jcal.getFixedDate(date.NormalizedYear, date.Month, 1, null);
				}
				else
				{
					// Use the cutover date as the first day of the month.
					fixedDateMonth1 = GregorianCutoverDate_Renamed;
				}
			}
			else
			{
				// The cutover happened before the month.
				fixedDateMonth1 = fixedDate - date.DayOfMonth + 1;
			}

			return fixedDateMonth1;
		}

		/// <summary>
		/// Returns a CalendarDate produced from the specified fixed date.
		/// </summary>
		/// <param name="fd"> the fixed date </param>
		private BaseCalendar.Date GetCalendarDate(long fd)
		{
			BaseCalendar cal = (fd >= GregorianCutoverDate_Renamed) ? Gcal : JulianCalendarSystem;
			BaseCalendar.Date d = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.NO_TIMEZONE);
			cal.getCalendarDateFromFixedDate(d, fd);
			return d;
		}

		/// <summary>
		/// Returns the Gregorian cutover date as a BaseCalendar.Date. The
		/// date is a Gregorian date.
		/// </summary>
		private BaseCalendar.Date GregorianCutoverDate
		{
			get
			{
				return GetCalendarDate(GregorianCutoverDate_Renamed);
			}
		}

		/// <summary>
		/// Returns the day before the Gregorian cutover date as a
		/// BaseCalendar.Date. The date is a Julian date.
		/// </summary>
		private BaseCalendar.Date LastJulianDate
		{
			get
			{
				return GetCalendarDate(GregorianCutoverDate_Renamed - 1);
			}
		}

		/// <summary>
		/// Returns the length of the specified month in the specified
		/// year. The year number must be normalized.
		/// </summary>
		/// <seealso cref= #isLeapYear(int) </seealso>
		private int MonthLength(int month, int year)
		{
			return IsLeapYear(year) ? LEAP_MONTH_LENGTH[month] : MONTH_LENGTH[month];
		}

		/// <summary>
		/// Returns the length of the specified month in the year provided
		/// by internalGet(YEAR).
		/// </summary>
		/// <seealso cref= #isLeapYear(int) </seealso>
		private int MonthLength(int month)
		{
			int year = InternalGet(YEAR);
			if (InternalGetEra() == BCE)
			{
				year = 1 - year;
			}
			return MonthLength(month, year);
		}

		private int ActualMonthLength()
		{
			int year = Cdate.NormalizedYear;
			if (year != GregorianCutoverYear && year != GregorianCutoverYearJulian)
			{
				return Calsys.getMonthLength(Cdate);
			}
			BaseCalendar.Date date = (BaseCalendar.Date) Cdate.clone();
			long fd = Calsys.getFixedDate(date);
			long month1 = GetFixedDateMonth1(date, fd);
			long next1 = month1 + Calsys.getMonthLength(date);
			if (next1 < GregorianCutoverDate_Renamed)
			{
				return (int)(next1 - month1);
			}
			if (Cdate != Gdate)
			{
				date = (BaseCalendar.Date) Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
			}
			Gcal.getCalendarDateFromFixedDate(date, next1);
			next1 = GetFixedDateMonth1(date, next1);
			return (int)(next1 - month1);
		}

		/// <summary>
		/// Returns the length (in days) of the specified year. The year
		/// must be normalized.
		/// </summary>
		private int YearLength(int year)
		{
			return IsLeapYear(year) ? 366 : 365;
		}

		/// <summary>
		/// Returns the length (in days) of the year provided by
		/// internalGet(YEAR).
		/// </summary>
		private int YearLength()
		{
			int year = InternalGet(YEAR);
			if (InternalGetEra() == BCE)
			{
				year = 1 - year;
			}
			return YearLength(year);
		}

		/// <summary>
		/// After adjustments such as add(MONTH), add(YEAR), we don't want the
		/// month to jump around.  E.g., we don't want Jan 31 + 1 month to go to Mar
		/// 3, we want it to go to Feb 28.  Adjustments which might run into this
		/// problem call this method to retain the proper month.
		/// </summary>
		private void PinDayOfMonth()
		{
			int year = InternalGet(YEAR);
			int monthLen;
			if (year > GregorianCutoverYear || year < GregorianCutoverYearJulian)
			{
				monthLen = MonthLength(InternalGet(MONTH));
			}
			else
			{
				GregorianCalendar gc = NormalizedCalendar;
				monthLen = gc.GetActualMaximum(DAY_OF_MONTH);
			}
			int dom = InternalGet(DAY_OF_MONTH);
			if (dom > monthLen)
			{
				Set(DAY_OF_MONTH, monthLen);
			}
		}

		/// <summary>
		/// Returns the fixed date value of this object. The time value and
		/// calendar fields must be in synch.
		/// </summary>
		private long CurrentFixedDate
		{
			get
			{
				return (Calsys == Gcal) ? CachedFixedDate : Calsys.getFixedDate(Cdate);
			}
		}

		/// <summary>
		/// Returns the new value after 'roll'ing the specified value and amount.
		/// </summary>
		private static int GetRolledValue(int value, int amount, int min, int max)
		{
			Debug.Assert(value >= min && value <= max);
			int range = max - min + 1;
			amount %= range;
			int n = value + amount;
			if (n > max)
			{
				n -= range;
			}
			else if (n < min)
			{
				n += range;
			}
			Debug.Assert(n >= min && n <= max);
			return n;
		}

		/// <summary>
		/// Returns the ERA.  We need a special method for this because the
		/// default ERA is CE, but a zero (unset) ERA is BCE.
		/// </summary>
		private int InternalGetEra()
		{
			return IsSet(ERA) ? InternalGet(ERA) : CE;
		}

		/// <summary>
		/// Updates internal state.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			if (Gdate == null)
			{
				Gdate = (BaseCalendar.Date) Gcal.newCalendarDate(Zone);
				CachedFixedDate = Long.MinValue;
			}
			GregorianChange = GregorianCutover;
		}

		/// <summary>
		/// Converts this object to a {@code ZonedDateTime} that represents
		/// the same point on the time-line as this {@code GregorianCalendar}.
		/// <para>
		/// Since this object supports a Julian-Gregorian cutover date and
		/// {@code ZonedDateTime} does not, it is possible that the resulting year,
		/// month and day will have different values.  The result will represent the
		/// correct date in the ISO calendar system, which will also be the same value
		/// for Modified Julian Days.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a zoned date-time representing the same point on the time-line
		///  as this gregorian calendar
		/// @since 1.8 </returns>
		public virtual ZonedDateTime ToZonedDateTime()
		{
			return ZonedDateTime.OfInstant(Instant.OfEpochMilli(TimeInMillis), TimeZone.ToZoneId());
		}

		/// <summary>
		/// Obtains an instance of {@code GregorianCalendar} with the default locale
		/// from a {@code ZonedDateTime} object.
		/// <para>
		/// Since {@code ZonedDateTime} does not support a Julian-Gregorian cutover
		/// date and uses ISO calendar system, the return GregorianCalendar is a pure
		/// Gregorian calendar and uses ISO 8601 standard for week definitions,
		/// which has {@code MONDAY} as the {@link Calendar#getFirstDayOfWeek()
		/// FirstDayOfWeek} and {@code 4} as the value of the
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() MinimalDaysInFirstWeek"/>.
		/// </para>
		/// <para>
		/// {@code ZoneDateTime} can store points on the time-line further in the
		/// future and further in the past than {@code GregorianCalendar}. In this
		/// scenario, this method will throw an {@code IllegalArgumentException}
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zdt">  the zoned date-time object to convert </param>
		/// <returns>  the gregorian calendar representing the same point on the
		///  time-line as the zoned date-time provided </returns>
		/// <exception cref="NullPointerException"> if {@code zdt} is null </exception>
		/// <exception cref="IllegalArgumentException"> if the zoned date-time is too
		/// large to represent as a {@code GregorianCalendar}
		/// @since 1.8 </exception>
		public static GregorianCalendar From(ZonedDateTime zdt)
		{
			GregorianCalendar cal = new GregorianCalendar(TimeZone.GetTimeZone(zdt.Zone));
			cal.GregorianChange = new Date(Long.MinValue);
			cal.FirstDayOfWeek = MONDAY;
			cal.MinimalDaysInFirstWeek = 4;
			try
			{
				cal.TimeInMillis = Math.AddExact(Math.MultiplyExact(zdt.toEpochSecond(), 1000), zdt.Get(ChronoField.MILLI_OF_SECOND));
			}
			catch (ArithmeticException ex)
			{
				throw new IllegalArgumentException(ex);
			}
			return cal;
		}
	}

}