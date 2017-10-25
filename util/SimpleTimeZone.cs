using System;

/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - All Rights Reserved
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

	using CalendarSystem = sun.util.calendar.CalendarSystem;
	using CalendarUtils = sun.util.calendar.CalendarUtils;
	using BaseCalendar = sun.util.calendar.BaseCalendar;
	using Gregorian = sun.util.calendar.Gregorian;

	/// <summary>
	/// <code>SimpleTimeZone</code> is a concrete subclass of <code>TimeZone</code>
	/// that represents a time zone for use with a Gregorian calendar.
	/// The class holds an offset from GMT, called <em>raw offset</em>, and start
	/// and end rules for a daylight saving time schedule.  Since it only holds
	/// single values for each, it cannot handle historical changes in the offset
	/// from GMT and the daylight saving schedule, except that the {@link
	/// #setStartYear setStartYear} method can specify the year when the daylight
	/// saving time schedule starts in effect.
	/// <para>
	/// To construct a <code>SimpleTimeZone</code> with a daylight saving time
	/// schedule, the schedule can be described with a set of rules,
	/// <em>start-rule</em> and <em>end-rule</em>. A day when daylight saving time
	/// starts or ends is specified by a combination of <em>month</em>,
	/// <em>day-of-month</em>, and <em>day-of-week</em> values. The <em>month</em>
	/// value is represented by a Calendar <seealso cref="Calendar#MONTH MONTH"/> field
	/// value, such as <seealso cref="Calendar#MARCH"/>. The <em>day-of-week</em> value is
	/// represented by a Calendar <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> value,
	/// such as <seealso cref="Calendar#SUNDAY SUNDAY"/>. The meanings of value combinations
	/// are as follows.
	/// 
	/// <ul>
	/// <li><b>Exact day of month</b><br>
	/// To specify an exact day of month, set the <em>month</em> and
	/// <em>day-of-month</em> to an exact value, and <em>day-of-week</em> to zero. For
	/// example, to specify March 1, set the <em>month</em> to {@link Calendar#MARCH
	/// MARCH}, <em>day-of-month</em> to 1, and <em>day-of-week</em> to 0.</li>
	/// 
	/// <li><b>Day of week on or after day of month</b><br>
	/// To specify a day of week on or after an exact day of month, set the
	/// <em>month</em> to an exact month value, <em>day-of-month</em> to the day on
	/// or after which the rule is applied, and <em>day-of-week</em> to a negative {@link
	/// Calendar#DAY_OF_WEEK DAY_OF_WEEK} field value. For example, to specify the
	/// second Sunday of April, set <em>month</em> to <seealso cref="Calendar#APRIL APRIL"/>,
	/// <em>day-of-month</em> to 8, and <em>day-of-week</em> to <code>-</code>{@link
	/// Calendar#SUNDAY SUNDAY}.</li>
	/// 
	/// <li><b>Day of week on or before day of month</b><br>
	/// To specify a day of the week on or before an exact day of the month, set
	/// <em>day-of-month</em> and <em>day-of-week</em> to a negative value. For
	/// example, to specify the last Wednesday on or before the 21st of March, set
	/// <em>month</em> to <seealso cref="Calendar#MARCH MARCH"/>, <em>day-of-month</em> is -21
	/// and <em>day-of-week</em> is <code>-</code><seealso cref="Calendar#WEDNESDAY WEDNESDAY"/>. </li>
	/// 
	/// <li><b>Last day-of-week of month</b><br>
	/// To specify, the last day-of-week of the month, set <em>day-of-week</em> to a
	/// <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> value and <em>day-of-month</em> to
	/// -1. For example, to specify the last Sunday of October, set <em>month</em>
	/// to <seealso cref="Calendar#OCTOBER OCTOBER"/>, <em>day-of-week</em> to {@link
	/// Calendar#SUNDAY SUNDAY} and <em>day-of-month</em> to -1.  </li>
	/// 
	/// </ul>
	/// The time of the day at which daylight saving time starts or ends is
	/// specified by a millisecond value within the day. There are three kinds of
	/// <em>mode</em>s to specify the time: <seealso cref="#WALL_TIME"/>, {@link
	/// #STANDARD_TIME} and <seealso cref="#UTC_TIME"/>. For example, if daylight
	/// saving time ends
	/// at 2:00 am in the wall clock time, it can be specified by 7200000
	/// milliseconds in the <seealso cref="#WALL_TIME"/> mode. In this case, the wall clock time
	/// for an <em>end-rule</em> means the same thing as the daylight time.
	/// </para>
	/// <para>
	/// The following are examples of parameters for constructing time zone objects.
	/// <pre><code>
	///      // Base GMT offset: -8:00
	///      // DST starts:      at 2:00am in standard time
	///      //                  on the first Sunday in April
	///      // DST ends:        at 2:00am in daylight time
	///      //                  on the last Sunday in October
	///      // Save:            1 hour
	///      SimpleTimeZone(-28800000,
	///                     "America/Los_Angeles",
	///                     Calendar.APRIL, 1, -Calendar.SUNDAY,
	///                     7200000,
	///                     Calendar.OCTOBER, -1, Calendar.SUNDAY,
	///                     7200000,
	///                     3600000)
	/// 
	///      // Base GMT offset: +1:00
	///      // DST starts:      at 1:00am in UTC time
	///      //                  on the last Sunday in March
	///      // DST ends:        at 1:00am in UTC time
	///      //                  on the last Sunday in October
	///      // Save:            1 hour
	///      SimpleTimeZone(3600000,
	///                     "Europe/Paris",
	///                     Calendar.MARCH, -1, Calendar.SUNDAY,
	///                     3600000, SimpleTimeZone.UTC_TIME,
	///                     Calendar.OCTOBER, -1, Calendar.SUNDAY,
	///                     3600000, SimpleTimeZone.UTC_TIME,
	///                     3600000)
	/// </code></pre>
	/// These parameter rules are also applicable to the set rule methods, such as
	/// <code>setStartRule</code>.
	/// 
	/// @since 1.1
	/// </para>
	/// </summary>
	/// <seealso cref=      Calendar </seealso>
	/// <seealso cref=      GregorianCalendar </seealso>
	/// <seealso cref=      TimeZone
	/// @author   David Goldsmith, Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>

	public class SimpleTimeZone : TimeZone
	{
		/// <summary>
		/// Constructs a SimpleTimeZone with the given base time zone offset from GMT
		/// and time zone ID with no daylight saving time schedule.
		/// </summary>
		/// <param name="rawOffset">  The base time zone offset in milliseconds to GMT. </param>
		/// <param name="ID">         The time zone name that is given to this instance. </param>
		public SimpleTimeZone(int rawOffset, String ID)
		{
			this.RawOffset_Renamed = rawOffset;
			ID = ID;
			DstSavings = MillisPerHour; // In case user sets rules later
		}

		/// <summary>
		/// Constructs a SimpleTimeZone with the given base time zone offset from
		/// GMT, time zone ID, and rules for starting and ending the daylight
		/// time.
		/// Both <code>startTime</code> and <code>endTime</code> are specified to be
		/// represented in the wall clock time. The amount of daylight saving is
		/// assumed to be 3600000 milliseconds (i.e., one hour). This constructor is
		/// equivalent to:
		/// <pre><code>
		///     SimpleTimeZone(rawOffset,
		///                    ID,
		///                    startMonth,
		///                    startDay,
		///                    startDayOfWeek,
		///                    startTime,
		///                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		///                    endMonth,
		///                    endDay,
		///                    endDayOfWeek,
		///                    endTime,
		///                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		///                    3600000)
		/// </code></pre>
		/// </summary>
		/// <param name="rawOffset">       The given base time zone offset from GMT. </param>
		/// <param name="ID">              The time zone ID which is given to this object. </param>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field value (0-based. e.g., 0
		///                        for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startTime">       The daylight saving time starting time in local wall clock
		///                        time (in milliseconds within the day), which is local
		///                        standard time in this case. </param>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endTime">         The daylight saving ending time in local wall clock time,
		///                        (in milliseconds within the day) which is local daylight
		///                        time in this case. </param>
		/// <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, or time
		/// parameters are out of range for the start or end rule </exception>
		public SimpleTimeZone(int rawOffset, String ID, int startMonth, int startDay, int startDayOfWeek, int startTime, int endMonth, int endDay, int endDayOfWeek, int endTime) : this(rawOffset, ID, startMonth, startDay, startDayOfWeek, startTime, WALL_TIME, endMonth, endDay, endDayOfWeek, endTime, WALL_TIME, MillisPerHour)
		{
		}

		/// <summary>
		/// Constructs a SimpleTimeZone with the given base time zone offset from
		/// GMT, time zone ID, and rules for starting and ending the daylight
		/// time.
		/// Both <code>startTime</code> and <code>endTime</code> are assumed to be
		/// represented in the wall clock time. This constructor is equivalent to:
		/// <pre><code>
		///     SimpleTimeZone(rawOffset,
		///                    ID,
		///                    startMonth,
		///                    startDay,
		///                    startDayOfWeek,
		///                    startTime,
		///                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		///                    endMonth,
		///                    endDay,
		///                    endDayOfWeek,
		///                    endTime,
		///                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		///                    dstSavings)
		/// </code></pre>
		/// </summary>
		/// <param name="rawOffset">       The given base time zone offset from GMT. </param>
		/// <param name="ID">              The time zone ID which is given to this object. </param>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 0 for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startTime">       The daylight saving time starting time in local wall clock
		///                        time, which is local standard time in this case. </param>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endTime">         The daylight saving ending time in local wall clock time,
		///                        which is local daylight time in this case. </param>
		/// <param name="dstSavings">      The amount of time in milliseconds saved during
		///                        daylight saving time. </param>
		/// <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, or time
		/// parameters are out of range for the start or end rule
		/// @since 1.2 </exception>
		public SimpleTimeZone(int rawOffset, String ID, int startMonth, int startDay, int startDayOfWeek, int startTime, int endMonth, int endDay, int endDayOfWeek, int endTime, int dstSavings) : this(rawOffset, ID, startMonth, startDay, startDayOfWeek, startTime, WALL_TIME, endMonth, endDay, endDayOfWeek, endTime, WALL_TIME, dstSavings)
		{
		}

		/// <summary>
		/// Constructs a SimpleTimeZone with the given base time zone offset from
		/// GMT, time zone ID, and rules for starting and ending the daylight
		/// time.
		/// This constructor takes the full set of the start and end rules
		/// parameters, including modes of <code>startTime</code> and
		/// <code>endTime</code>. The mode specifies either {@link #WALL_TIME wall
		/// time} or <seealso cref="#STANDARD_TIME standard time"/> or {@link #UTC_TIME UTC
		/// time}.
		/// </summary>
		/// <param name="rawOffset">       The given base time zone offset from GMT. </param>
		/// <param name="ID">              The time zone ID which is given to this object. </param>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 0 for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startTime">       The daylight saving time starting time in the time mode
		///                        specified by <code>startTimeMode</code>. </param>
		/// <param name="startTimeMode">   The mode of the start time specified by startTime. </param>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endTime">         The daylight saving ending time in time time mode
		///                        specified by <code>endTimeMode</code>. </param>
		/// <param name="endTimeMode">     The mode of the end time specified by endTime </param>
		/// <param name="dstSavings">      The amount of time in milliseconds saved during
		///                        daylight saving time.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, time more, or
		/// time parameters are out of range for the start or end rule, or if a time mode
		/// value is invalid.
		/// </exception>
		/// <seealso cref= #WALL_TIME </seealso>
		/// <seealso cref= #STANDARD_TIME </seealso>
		/// <seealso cref= #UTC_TIME
		/// 
		/// @since 1.4 </seealso>
		public SimpleTimeZone(int rawOffset, String ID, int startMonth, int startDay, int startDayOfWeek, int startTime, int startTimeMode, int endMonth, int endDay, int endDayOfWeek, int endTime, int endTimeMode, int dstSavings)
		{

			ID = ID;
			this.RawOffset_Renamed = rawOffset;
			this.StartMonth = startMonth;
			this.StartDay = startDay;
			this.StartDayOfWeek = startDayOfWeek;
			this.StartTime = startTime;
			this.StartTimeMode = startTimeMode;
			this.EndMonth = endMonth;
			this.EndDay = endDay;
			this.EndDayOfWeek = endDayOfWeek;
			this.EndTime = endTime;
			this.EndTimeMode = endTimeMode;
			this.DstSavings = dstSavings;

			// this.useDaylight is set by decodeRules
			DecodeRules();
			if (dstSavings <= 0)
			{
				throw new IllegalArgumentException("Illegal daylight saving value: " + dstSavings);
			}
		}

		/// <summary>
		/// Sets the daylight saving time starting year.
		/// </summary>
		/// <param name="year">  The daylight saving starting year. </param>
		public virtual int StartYear
		{
			set
			{
				StartYear_Renamed = value;
				InvalidateCache();
			}
		}

		/// <summary>
		/// Sets the daylight saving time start rule. For example, if daylight saving
		/// time starts on the first Sunday in April at 2 am in local wall clock
		/// time, you can set the start rule by calling:
		/// <pre><code>setStartRule(Calendar.APRIL, 1, Calendar.SUNDAY, 2*60*60*1000);</code></pre>
		/// </summary>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 0 for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="startTime">       The daylight saving time starting time in local wall clock
		///                        time, which is local standard time in this case. </param>
		/// <exception cref="IllegalArgumentException"> if the <code>startMonth</code>, <code>startDay</code>,
		/// <code>startDayOfWeek</code>, or <code>startTime</code> parameters are out of range </exception>
		public virtual void SetStartRule(int startMonth, int startDay, int startDayOfWeek, int startTime)
		{
			this.StartMonth = startMonth;
			this.StartDay = startDay;
			this.StartDayOfWeek = startDayOfWeek;
			this.StartTime = startTime;
			StartTimeMode = WALL_TIME;
			DecodeStartRule();
			InvalidateCache();
		}

		/// <summary>
		/// Sets the daylight saving time start rule to a fixed date within a month.
		/// This method is equivalent to:
		/// <pre><code>setStartRule(startMonth, startDay, 0, startTime)</code></pre>
		/// </summary>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 0 for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts. </param>
		/// <param name="startTime">       The daylight saving time starting time in local wall clock
		///                        time, which is local standard time in this case.
		///                        See the class description for the special cases of this parameter. </param>
		/// <exception cref="IllegalArgumentException"> if the <code>startMonth</code>,
		/// <code>startDayOfMonth</code>, or <code>startTime</code> parameters are out of range
		/// @since 1.2 </exception>
		public virtual void SetStartRule(int startMonth, int startDay, int startTime)
		{
			SetStartRule(startMonth, startDay, 0, startTime);
		}

		/// <summary>
		/// Sets the daylight saving time start rule to a weekday before or after the given date within
		/// a month, e.g., the first Monday on or after the 8th.
		/// </summary>
		/// <param name="startMonth">      The daylight saving time starting month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 0 for January). </param>
		/// <param name="startDay">        The day of the month on which the daylight saving time starts. </param>
		/// <param name="startDayOfWeek">  The daylight saving time starting day-of-week. </param>
		/// <param name="startTime">       The daylight saving time starting time in local wall clock
		///                        time, which is local standard time in this case. </param>
		/// <param name="after">           If true, this rule selects the first <code>dayOfWeek</code> on or
		///                        <em>after</em> <code>dayOfMonth</code>.  If false, this rule
		///                        selects the last <code>dayOfWeek</code> on or <em>before</em>
		///                        <code>dayOfMonth</code>. </param>
		/// <exception cref="IllegalArgumentException"> if the <code>startMonth</code>, <code>startDay</code>,
		/// <code>startDayOfWeek</code>, or <code>startTime</code> parameters are out of range
		/// @since 1.2 </exception>
		public virtual void SetStartRule(int startMonth, int startDay, int startDayOfWeek, int startTime, bool after)
		{
			// TODO: this method doesn't check the initial values of dayOfMonth or dayOfWeek.
			if (after)
			{
				SetStartRule(startMonth, startDay, -startDayOfWeek, startTime);
			}
			else
			{
				SetStartRule(startMonth, -startDay, -startDayOfWeek, startTime);
			}
		}

		/// <summary>
		/// Sets the daylight saving time end rule. For example, if daylight saving time
		/// ends on the last Sunday in October at 2 am in wall clock time,
		/// you can set the end rule by calling:
		/// <code>setEndRule(Calendar.OCTOBER, -1, Calendar.SUNDAY, 2*60*60*1000);</code>
		/// </summary>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		///                        See the class description for the special cases of this parameter. </param>
		/// <param name="endTime">         The daylight saving ending time in local wall clock time,
		///                        (in milliseconds within the day) which is local daylight
		///                        time in this case. </param>
		/// <exception cref="IllegalArgumentException"> if the <code>endMonth</code>, <code>endDay</code>,
		/// <code>endDayOfWeek</code>, or <code>endTime</code> parameters are out of range </exception>
		public virtual void SetEndRule(int endMonth, int endDay, int endDayOfWeek, int endTime)
		{
			this.EndMonth = endMonth;
			this.EndDay = endDay;
			this.EndDayOfWeek = endDayOfWeek;
			this.EndTime = endTime;
			this.EndTimeMode = WALL_TIME;
			DecodeEndRule();
			InvalidateCache();
		}

		/// <summary>
		/// Sets the daylight saving time end rule to a fixed date within a month.
		/// This method is equivalent to:
		/// <pre><code>setEndRule(endMonth, endDay, 0, endTime)</code></pre>
		/// </summary>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends. </param>
		/// <param name="endTime">         The daylight saving ending time in local wall clock time,
		///                        (in milliseconds within the day) which is local daylight
		///                        time in this case. </param>
		/// <exception cref="IllegalArgumentException"> the <code>endMonth</code>, <code>endDay</code>,
		/// or <code>endTime</code> parameters are out of range
		/// @since 1.2 </exception>
		public virtual void SetEndRule(int endMonth, int endDay, int endTime)
		{
			SetEndRule(endMonth, endDay, 0, endTime);
		}

		/// <summary>
		/// Sets the daylight saving time end rule to a weekday before or after the given date within
		/// a month, e.g., the first Monday on or after the 8th.
		/// </summary>
		/// <param name="endMonth">        The daylight saving time ending month. Month is
		///                        a <seealso cref="Calendar#MONTH MONTH"/> field
		///                        value (0-based. e.g., 9 for October). </param>
		/// <param name="endDay">          The day of the month on which the daylight saving time ends. </param>
		/// <param name="endDayOfWeek">    The daylight saving time ending day-of-week. </param>
		/// <param name="endTime">         The daylight saving ending time in local wall clock time,
		///                        (in milliseconds within the day) which is local daylight
		///                        time in this case. </param>
		/// <param name="after">           If true, this rule selects the first <code>endDayOfWeek</code> on
		///                        or <em>after</em> <code>endDay</code>.  If false, this rule
		///                        selects the last <code>endDayOfWeek</code> on or before
		///                        <code>endDay</code> of the month. </param>
		/// <exception cref="IllegalArgumentException"> the <code>endMonth</code>, <code>endDay</code>,
		/// <code>endDayOfWeek</code>, or <code>endTime</code> parameters are out of range
		/// @since 1.2 </exception>
		public virtual void SetEndRule(int endMonth, int endDay, int endDayOfWeek, int endTime, bool after)
		{
			if (after)
			{
				SetEndRule(endMonth, endDay, -endDayOfWeek, endTime);
			}
			else
			{
				SetEndRule(endMonth, -endDay, -endDayOfWeek, endTime);
			}
		}

		/// <summary>
		/// Returns the offset of this time zone from UTC at the given
		/// time. If daylight saving time is in effect at the given time,
		/// the offset value is adjusted with the amount of daylight
		/// saving.
		/// </summary>
		/// <param name="date"> the time at which the time zone offset is found </param>
		/// <returns> the amount of time in milliseconds to add to UTC to get
		/// local time.
		/// @since 1.4 </returns>
		public override int GetOffset(long date)
		{
			return GetOffsets(date, null);
		}

		/// <seealso cref= TimeZone#getOffsets </seealso>
		internal override int GetOffsets(long date, int[] offsets)
		{
			int offset = RawOffset_Renamed;

			if (UseDaylight)
			{
				lock (this)
				{
					if (CacheStart != 0)
					{
						if (date >= CacheStart && date < CacheEnd)
						{
							offset += DstSavings;
							goto computeOffsetBreak;
						}
					}
				}
				BaseCalendar cal = date >= GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER ? Gcal : (BaseCalendar) CalendarSystem.forName("julian");
				BaseCalendar.Date cdate = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.NO_TIMEZONE);
				// Get the year in local time
				cal.getCalendarDate(date + RawOffset_Renamed, cdate);
				int year = cdate.NormalizedYear;
				if (year >= StartYear_Renamed)
				{
					// Clear time elements for the transition calculations
					cdate.setTimeOfDay(0, 0, 0, 0);
					offset = GetOffset(cal, cdate, year, date);
				}
			}
		  computeOffsetBreak:

			if (offsets != null)
			{
				offsets[0] = RawOffset_Renamed;
				offsets[1] = offset - RawOffset_Renamed;
			}
			return offset;
		}

	   /// <summary>
	   /// Returns the difference in milliseconds between local time and
	   /// UTC, taking into account both the raw offset and the effect of
	   /// daylight saving, for the specified date and time.  This method
	   /// assumes that the start and end month are distinct.  It also
	   /// uses a default <seealso cref="GregorianCalendar"/> object as its
	   /// underlying calendar, such as for determining leap years.  Do
	   /// not use the result of this method with a calendar other than a
	   /// default <code>GregorianCalendar</code>.
	   ///  
	   /// <para><em>Note:  In general, clients should use
	   /// <code>Calendar.get(ZONE_OFFSET) + Calendar.get(DST_OFFSET)</code>
	   /// instead of calling this method.</em>
	   ///  
	   /// </para>
	   /// </summary>
	   /// <param name="era">       The era of the given date. </param>
	   /// <param name="year">      The year in the given date. </param>
	   /// <param name="month">     The month in the given date. Month is 0-based. e.g.,
	   ///                  0 for January. </param>
	   /// <param name="day">       The day-in-month of the given date. </param>
	   /// <param name="dayOfWeek"> The day-of-week of the given date. </param>
	   /// <param name="millis">    The milliseconds in day in <em>standard</em> local time. </param>
	   /// <returns>          The milliseconds to add to UTC to get local time. </returns>
	   /// <exception cref="IllegalArgumentException"> the <code>era</code>,
	   ///                  <code>month</code>, <code>day</code>, <code>dayOfWeek</code>,
	   ///                  or <code>millis</code> parameters are out of range </exception>
		public override int GetOffset(int era, int year, int month, int day, int dayOfWeek, int millis)
		{
			if (era != GregorianCalendar.AD && era != GregorianCalendar.BC)
			{
				throw new IllegalArgumentException("Illegal era " + era);
			}

			int y = year;
			if (era == GregorianCalendar.BC)
			{
				// adjust y with the GregorianCalendar-style year numbering.
				y = 1 - y;
			}

			// If the year isn't representable with the 64-bit long
			// integer in milliseconds, convert the year to an
			// equivalent year. This is required to pass some JCK test cases
			// which are actually useless though because the specified years
			// can't be supported by the Java time system.
			if (y >= 292278994)
			{
				y = 2800 + y % 2800;
			}
			else if (y <= -292269054)
			{
				// y %= 28 also produces an equivalent year, but positive
				// year numbers would be convenient to use the UNIX cal
				// command.
				y = (int) CalendarUtils.mod((long) y, 28);
			}

			// convert year to its 1-based month value
			int m = month + 1;

			// First, calculate time as a Gregorian date.
			BaseCalendar cal = Gcal;
			BaseCalendar.Date cdate = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.NO_TIMEZONE);
			cdate.setDate(y, m, day);
			long time = cal.getTime(cdate); // normalize cdate
			time += millis - RawOffset_Renamed; // UTC time

			// If the time value represents a time before the default
			// Gregorian cutover, recalculate time using the Julian
			// calendar system. For the Julian calendar system, the
			// normalized year numbering is ..., -2 (BCE 2), -1 (BCE 1),
			// 1, 2 ... which is different from the GregorianCalendar
			// style year numbering (..., -1, 0 (BCE 1), 1, 2, ...).
			if (time < GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER)
			{
				cal = (BaseCalendar) CalendarSystem.forName("julian");
				cdate = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.NO_TIMEZONE);
				cdate.setNormalizedDate(y, m, day);
				time = cal.getTime(cdate) + millis - RawOffset_Renamed;
			}

			if ((cdate.NormalizedYear != y) || (cdate.Month != m) || (cdate.DayOfMonth != day) || (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday) || (millis < 0 || millis >= (24 * 60 * 60 * 1000)))
				// The validation should be cdate.getDayOfWeek() ==
				// dayOfWeek. However, we don't check dayOfWeek for
				// compatibility.
			{
				throw new IllegalArgumentException();
			}

			if (!UseDaylight || year < StartYear_Renamed || era != GregorianCalendar.CE)
			{
				return RawOffset_Renamed;
			}

			return GetOffset(cal, cdate, y, time);
		}

		private int GetOffset(BaseCalendar cal, BaseCalendar.Date cdate, int year, long time)
		{
			lock (this)
			{
				if (CacheStart != 0)
				{
					if (time >= CacheStart && time < CacheEnd)
					{
						return RawOffset_Renamed + DstSavings;
					}
					if (year == CacheYear)
					{
						return RawOffset_Renamed;
					}
				}
			}

			long start = GetStart(cal, cdate, year);
			long end = GetEnd(cal, cdate, year);
			int offset = RawOffset_Renamed;
			if (start <= end)
			{
				if (time >= start && time < end)
				{
					offset += DstSavings;
				}
				lock (this)
				{
					CacheYear = year;
					CacheStart = start;
					CacheEnd = end;
				}
			}
			else
			{
				if (time < end)
				{
					// TODO: support Gregorian cutover. The previous year
					// may be in the other calendar system.
					start = GetStart(cal, cdate, year - 1);
					if (time >= start)
					{
						offset += DstSavings;
					}
				}
				else if (time >= start)
				{
					// TODO: support Gregorian cutover. The next year
					// may be in the other calendar system.
					end = GetEnd(cal, cdate, year + 1);
					if (time < end)
					{
						offset += DstSavings;
					}
				}
				if (start <= end)
				{
					lock (this)
					{
						// The start and end transitions are in multiple years.
						CacheYear = (long) StartYear_Renamed - 1;
						CacheStart = start;
						CacheEnd = end;
					}
				}
			}
			return offset;
		}

		private long GetStart(BaseCalendar cal, BaseCalendar.Date cdate, int year)
		{
			int time = StartTime;
			if (StartTimeMode != UTC_TIME)
			{
				time -= RawOffset_Renamed;
			}
			return GetTransition(cal, cdate, StartMode, year, StartMonth, StartDay, StartDayOfWeek, time);
		}

		private long GetEnd(BaseCalendar cal, BaseCalendar.Date cdate, int year)
		{
			int time = EndTime;
			if (EndTimeMode != UTC_TIME)
			{
				time -= RawOffset_Renamed;
			}
			if (EndTimeMode == WALL_TIME)
			{
				time -= DstSavings;
			}
			return GetTransition(cal, cdate, EndMode, year, EndMonth, EndDay, EndDayOfWeek, time);
		}

		private long GetTransition(BaseCalendar cal, BaseCalendar.Date cdate, int mode, int year, int month, int dayOfMonth, int dayOfWeek, int timeOfDay)
		{
			cdate.NormalizedYear = year;
			cdate.Month = month + 1;
			switch (mode)
			{
			case DOM_MODE:
				cdate.DayOfMonth = dayOfMonth;
				break;

			case DOW_IN_MONTH_MODE:
				cdate.DayOfMonth = 1;
				if (dayOfMonth < 0)
				{
					cdate.DayOfMonth = cal.getMonthLength(cdate);
				}
				cdate = (BaseCalendar.Date) cal.getNthDayOfWeek(dayOfMonth, dayOfWeek, cdate);
				break;

			case DOW_GE_DOM_MODE:
				cdate.DayOfMonth = dayOfMonth;
				cdate = (BaseCalendar.Date) cal.getNthDayOfWeek(1, dayOfWeek, cdate);
				break;

			case DOW_LE_DOM_MODE:
				cdate.DayOfMonth = dayOfMonth;
				cdate = (BaseCalendar.Date) cal.getNthDayOfWeek(-1, dayOfWeek, cdate);
				break;
			}
			return cal.getTime(cdate) + timeOfDay;
		}

		/// <summary>
		/// Gets the GMT offset for this time zone. </summary>
		/// <returns> the GMT offset value in milliseconds </returns>
		/// <seealso cref= #setRawOffset </seealso>
		public override int RawOffset
		{
			get
			{
				// The given date will be taken into account while
				// we have the historical time zone data in place.
				return RawOffset_Renamed;
			}
			set
			{
				this.RawOffset_Renamed = value;
			}
		}


		/// <summary>
		/// Sets the amount of time in milliseconds that the clock is advanced
		/// during daylight saving time. </summary>
		/// <param name="millisSavedDuringDST"> the number of milliseconds the time is
		/// advanced with respect to standard time when the daylight saving time rules
		/// are in effect. A positive number, typically one hour (3600000). </param>
		/// <seealso cref= #getDSTSavings
		/// @since 1.2 </seealso>
		public virtual int DSTSavings
		{
			set
			{
				if (value <= 0)
				{
					throw new IllegalArgumentException("Illegal daylight saving value: " + value);
				}
				DstSavings = value;
			}
			get
			{
				return UseDaylight ? DstSavings : 0;
			}
		}


		/// <summary>
		/// Queries if this time zone uses daylight saving time. </summary>
		/// <returns> true if this time zone uses daylight saving time;
		/// false otherwise. </returns>
		public override bool UseDaylightTime()
		{
			return UseDaylight;
		}

		/// <summary>
		/// Returns {@code true} if this {@code SimpleTimeZone} observes
		/// Daylight Saving Time. This method is equivalent to {@link
		/// #useDaylightTime()}.
		/// </summary>
		/// <returns> {@code true} if this {@code SimpleTimeZone} observes
		/// Daylight Saving Time; {@code false} otherwise.
		/// @since 1.7 </returns>
		public override bool ObservesDaylightTime()
		{
			return UseDaylightTime();
		}

		/// <summary>
		/// Queries if the given date is in daylight saving time. </summary>
		/// <returns> true if daylight saving time is in effective at the
		/// given date; false otherwise. </returns>
		public override bool InDaylightTime(Date date)
		{
			return (GetOffset(date.Time) != RawOffset_Renamed);
		}

		/// <summary>
		/// Returns a clone of this <code>SimpleTimeZone</code> instance. </summary>
		/// <returns> a clone of this instance. </returns>
		public override Object Clone()
		{
			return base.Clone();
		}

		/// <summary>
		/// Generates the hash code for the SimpleDateFormat object. </summary>
		/// <returns> the hash code for this object </returns>
		public override int HashCode()
		{
			lock (this)
			{
				return StartMonth ^ StartDay ^ StartDayOfWeek ^ StartTime ^ EndMonth ^ EndDay ^ EndDayOfWeek ^ EndTime ^ RawOffset_Renamed;
			}
		}

		/// <summary>
		/// Compares the equality of two <code>SimpleTimeZone</code> objects.
		/// </summary>
		/// <param name="obj">  The <code>SimpleTimeZone</code> object to be compared with. </param>
		/// <returns>     True if the given <code>obj</code> is the same as this
		///             <code>SimpleTimeZone</code> object; false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is SimpleTimeZone))
			{
				return false;
			}

			SimpleTimeZone that = (SimpleTimeZone) obj;

			return ID.Equals(that.ID) && HasSameRules(that);
		}

		/// <summary>
		/// Returns <code>true</code> if this zone has the same rules and offset as another zone. </summary>
		/// <param name="other"> the TimeZone object to be compared with </param>
		/// <returns> <code>true</code> if the given zone is a SimpleTimeZone and has the
		/// same rules and offset as this one
		/// @since 1.2 </returns>
		public override bool HasSameRules(TimeZone other)
		{
			if (this == other)
			{
				return true;
			}
			if (!(other is SimpleTimeZone))
			{
				return false;
			}
			SimpleTimeZone that = (SimpleTimeZone) other;
			return RawOffset_Renamed == that.RawOffset_Renamed && UseDaylight == that.UseDaylight && (!UseDaylight || (DstSavings == that.DstSavings && StartMode == that.StartMode && StartMonth == that.StartMonth && StartDay == that.StartDay && StartDayOfWeek == that.StartDayOfWeek && StartTime == that.StartTime && StartTimeMode == that.StartTimeMode && EndMode == that.EndMode && EndMonth == that.EndMonth && EndDay == that.EndDay && EndDayOfWeek == that.EndDayOfWeek && EndTime == that.EndTime && EndTimeMode == that.EndTimeMode && StartYear_Renamed == that.StartYear_Renamed));
				 // Only check rules if using DST
		}

		/// <summary>
		/// Returns a string representation of this time zone. </summary>
		/// <returns> a string representation of this time zone. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[id=" + ID + ",offset=" + RawOffset_Renamed + ",dstSavings=" + DstSavings + ",useDaylight=" + UseDaylight + ",startYear=" + StartYear_Renamed + ",startMode=" + StartMode + ",startMonth=" + StartMonth + ",startDay=" + StartDay + ",startDayOfWeek=" + StartDayOfWeek + ",startTime=" + StartTime + ",startTimeMode=" + StartTimeMode + ",endMode=" + EndMode + ",endMonth=" + EndMonth + ",endDay=" + EndDay + ",endDayOfWeek=" + EndDayOfWeek + ",endTime=" + EndTime + ",endTimeMode=" + EndTimeMode + ']';
		}

		// =======================privates===============================

		/// <summary>
		/// The month in which daylight saving time starts.  This value must be
		/// between <code>Calendar.JANUARY</code> and
		/// <code>Calendar.DECEMBER</code> inclusive.  This value must not equal
		/// <code>endMonth</code>.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int StartMonth;

		/// <summary>
		/// This field has two possible interpretations:
		/// <dl>
		/// <dt><code>startMode == DOW_IN_MONTH</code></dt>
		/// <dd>
		/// <code>startDay</code> indicates the day of the month of
		/// <code>startMonth</code> on which daylight
		/// saving time starts, from 1 to 28, 30, or 31, depending on the
		/// <code>startMonth</code>.
		/// </dd>
		/// <dt><code>startMode != DOW_IN_MONTH</code></dt>
		/// <dd>
		/// <code>startDay</code> indicates which <code>startDayOfWeek</code> in the
		/// month <code>startMonth</code> daylight
		/// saving time starts on.  For example, a value of +1 and a
		/// <code>startDayOfWeek</code> of <code>Calendar.SUNDAY</code> indicates the
		/// first Sunday of <code>startMonth</code>.  Likewise, +2 would indicate the
		/// second Sunday, and -1 the last Sunday.  A value of 0 is illegal.
		/// </dd>
		/// </dl>
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int StartDay;

		/// <summary>
		/// The day of the week on which daylight saving time starts.  This value
		/// must be between <code>Calendar.SUNDAY</code> and
		/// <code>Calendar.SATURDAY</code> inclusive.
		/// <para>If <code>useDaylight</code> is false or
		/// <code>startMode == DAY_OF_MONTH</code>, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int StartDayOfWeek;

		/// <summary>
		/// The time in milliseconds after midnight at which daylight saving
		/// time starts.  This value is expressed as wall time, standard time,
		/// or UTC time, depending on the setting of <code>startTimeMode</code>.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int StartTime;

		/// <summary>
		/// The format of startTime, either WALL_TIME, STANDARD_TIME, or UTC_TIME.
		/// @serial
		/// @since 1.3
		/// </summary>
		private int StartTimeMode;

		/// <summary>
		/// The month in which daylight saving time ends.  This value must be
		/// between <code>Calendar.JANUARY</code> and
		/// <code>Calendar.UNDECIMBER</code>.  This value must not equal
		/// <code>startMonth</code>.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int EndMonth;

		/// <summary>
		/// This field has two possible interpretations:
		/// <dl>
		/// <dt><code>endMode == DOW_IN_MONTH</code></dt>
		/// <dd>
		/// <code>endDay</code> indicates the day of the month of
		/// <code>endMonth</code> on which daylight
		/// saving time ends, from 1 to 28, 30, or 31, depending on the
		/// <code>endMonth</code>.
		/// </dd>
		/// <dt><code>endMode != DOW_IN_MONTH</code></dt>
		/// <dd>
		/// <code>endDay</code> indicates which <code>endDayOfWeek</code> in th
		/// month <code>endMonth</code> daylight
		/// saving time ends on.  For example, a value of +1 and a
		/// <code>endDayOfWeek</code> of <code>Calendar.SUNDAY</code> indicates the
		/// first Sunday of <code>endMonth</code>.  Likewise, +2 would indicate the
		/// second Sunday, and -1 the last Sunday.  A value of 0 is illegal.
		/// </dd>
		/// </dl>
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int EndDay;

		/// <summary>
		/// The day of the week on which daylight saving time ends.  This value
		/// must be between <code>Calendar.SUNDAY</code> and
		/// <code>Calendar.SATURDAY</code> inclusive.
		/// <para>If <code>useDaylight</code> is false or
		/// <code>endMode == DAY_OF_MONTH</code>, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int EndDayOfWeek;

		/// <summary>
		/// The time in milliseconds after midnight at which daylight saving
		/// time ends.  This value is expressed as wall time, standard time,
		/// or UTC time, depending on the setting of <code>endTimeMode</code>.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int EndTime;

		/// <summary>
		/// The format of endTime, either <code>WALL_TIME</code>,
		/// <code>STANDARD_TIME</code>, or <code>UTC_TIME</code>.
		/// @serial
		/// @since 1.3
		/// </summary>
		private int EndTimeMode;

		/// <summary>
		/// The year in which daylight saving time is first observed.  This is an <seealso cref="GregorianCalendar#AD AD"/>
		/// value.  If this value is less than 1 then daylight saving time is observed
		/// for all <code>AD</code> years.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// </para>
		/// </summary>
		private int StartYear_Renamed;

		/// <summary>
		/// The offset in milliseconds between this zone and GMT.  Negative offsets
		/// are to the west of Greenwich.  To obtain local <em>standard</em> time,
		/// add the offset to GMT time.  To obtain local wall time it may also be
		/// necessary to add <code>dstSavings</code>.
		/// @serial
		/// </summary>
		private int RawOffset_Renamed;

		/// <summary>
		/// A boolean value which is true if and only if this zone uses daylight
		/// saving time.  If this value is false, several other fields are ignored.
		/// @serial
		/// </summary>
		private bool UseDaylight = false; // indicate if this time zone uses DST

		private const int MillisPerHour = 60 * 60 * 1000;
		private static readonly int MillisPerDay = 24 * MillisPerHour;

		/// <summary>
		/// This field was serialized in JDK 1.1, so we have to keep it that way
		/// to maintain serialization compatibility. However, there's no need to
		/// recreate the array each time we create a new time zone.
		/// @serial An array of bytes containing the values {31, 28, 31, 30, 31, 30,
		/// 31, 31, 30, 31, 30, 31}.  This is ignored as of the Java 2 platform v1.2, however, it must
		/// be streamed out for compatibility with JDK 1.1.
		/// </summary>
		private readonly sbyte[] MonthLength = StaticMonthLength;
		private static readonly sbyte[] StaticMonthLength = new sbyte[] {31,28,31,30,31,30,31,31,30,31,30,31};
		private static readonly sbyte[] StaticLeapMonthLength = new sbyte[] {31,29,31,30,31,30,31,31,30,31,30,31};

		/// <summary>
		/// Variables specifying the mode of the start rule.  Takes the following
		/// values:
		/// <dl>
		/// <dt><code>DOM_MODE</code></dt>
		/// <dd>
		/// Exact day of week; e.g., March 1.
		/// </dd>
		/// <dt><code>DOW_IN_MONTH_MODE</code></dt>
		/// <dd>
		/// Day of week in month; e.g., last Sunday in March.
		/// </dd>
		/// <dt><code>DOW_GE_DOM_MODE</code></dt>
		/// <dd>
		/// Day of week after day of month; e.g., Sunday on or after March 15.
		/// </dd>
		/// <dt><code>DOW_LE_DOM_MODE</code></dt>
		/// <dd>
		/// Day of week before day of month; e.g., Sunday on or before March 15.
		/// </dd>
		/// </dl>
		/// The setting of this field affects the interpretation of the
		/// <code>startDay</code> field.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// @since 1.1.4
		/// </para>
		/// </summary>
		private int StartMode;

		/// <summary>
		/// Variables specifying the mode of the end rule.  Takes the following
		/// values:
		/// <dl>
		/// <dt><code>DOM_MODE</code></dt>
		/// <dd>
		/// Exact day of week; e.g., March 1.
		/// </dd>
		/// <dt><code>DOW_IN_MONTH_MODE</code></dt>
		/// <dd>
		/// Day of week in month; e.g., last Sunday in March.
		/// </dd>
		/// <dt><code>DOW_GE_DOM_MODE</code></dt>
		/// <dd>
		/// Day of week after day of month; e.g., Sunday on or after March 15.
		/// </dd>
		/// <dt><code>DOW_LE_DOM_MODE</code></dt>
		/// <dd>
		/// Day of week before day of month; e.g., Sunday on or before March 15.
		/// </dd>
		/// </dl>
		/// The setting of this field affects the interpretation of the
		/// <code>endDay</code> field.
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// @since 1.1.4
		/// </para>
		/// </summary>
		private int EndMode;

		/// <summary>
		/// A positive value indicating the amount of time saved during DST in
		/// milliseconds.
		/// Typically one hour (3600000); sometimes 30 minutes (1800000).
		/// <para>If <code>useDaylight</code> is false, this value is ignored.
		/// @serial
		/// @since 1.1.4
		/// </para>
		/// </summary>
		private int DstSavings;

		private static readonly Gregorian Gcal = CalendarSystem.GregorianCalendar;

		/// <summary>
		/// Cache values representing a single period of daylight saving
		/// time. When the cache values are valid, cacheStart is the start
		/// time (inclusive) of daylight saving time and cacheEnd is the
		/// end time (exclusive).
		/// 
		/// cacheYear has a year value if both cacheStart and cacheEnd are
		/// in the same year. cacheYear is set to startYear - 1 if
		/// cacheStart and cacheEnd are in different years. cacheStart is 0
		/// if the cache values are void. cacheYear is a long to support
		/// Integer.MIN_VALUE - 1 (JCK requirement).
		/// </summary>
		[NonSerialized]
		private long CacheYear;
		[NonSerialized]
		private long CacheStart;
		[NonSerialized]
		private long CacheEnd;

		/// <summary>
		/// Constants specifying values of startMode and endMode.
		/// </summary>
		private const int DOM_MODE = 1; // Exact day of month, "Mar 1"
		private const int DOW_IN_MONTH_MODE = 2; // Day of week in month, "lastSun"
		private const int DOW_GE_DOM_MODE = 3; // Day of week after day of month, "Sun>=15"
		private const int DOW_LE_DOM_MODE = 4; // Day of week before day of month, "Sun<=21"

		/// <summary>
		/// Constant for a mode of start or end time specified as wall clock
		/// time.  Wall clock time is standard time for the onset rule, and
		/// daylight time for the end rule.
		/// @since 1.4
		/// </summary>
		public const int WALL_TIME = 0; // Zero for backward compatibility

		/// <summary>
		/// Constant for a mode of start or end time specified as standard time.
		/// @since 1.4
		/// </summary>
		public const int STANDARD_TIME = 1;

		/// <summary>
		/// Constant for a mode of start or end time specified as UTC. European
		/// Union rules are specified as UTC time, for example.
		/// @since 1.4
		/// </summary>
		public const int UTC_TIME = 2;

		// Proclaim compatibility with 1.1
		internal new const long SerialVersionUID = -403250971215465050L;

		// the internal serial version which says which version was written
		// - 0 (default) for version up to JDK 1.1.3
		// - 1 for version from JDK 1.1.4, which includes 3 new fields
		// - 2 for JDK 1.3, which includes 2 new fields
		internal const int CurrentSerialVersion = 2;

		/// <summary>
		/// The version of the serialized data on the stream.  Possible values:
		/// <dl>
		/// <dt><b>0</b> or not present on stream</dt>
		/// <dd>
		/// JDK 1.1.3 or earlier.
		/// </dd>
		/// <dt><b>1</b></dt>
		/// <dd>
		/// JDK 1.1.4 or later.  Includes three new fields: <code>startMode</code>,
		/// <code>endMode</code>, and <code>dstSavings</code>.
		/// </dd>
		/// <dt><b>2</b></dt>
		/// <dd>
		/// JDK 1.3 or later.  Includes two new fields: <code>startTimeMode</code>
		/// and <code>endTimeMode</code>.
		/// </dd>
		/// </dl>
		/// When streaming out this class, the most recent format
		/// and the highest allowable <code>serialVersionOnStream</code>
		/// is written.
		/// @serial
		/// @since 1.1.4
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		private void InvalidateCache()
		{
			lock (this)
			{
				CacheYear = StartYear_Renamed - 1;
				CacheStart = CacheEnd = 0;
			}
		}

		//----------------------------------------------------------------------
		// Rule representation
		//
		// We represent the following flavors of rules:
		//       5        the fifth of the month
		//       lastSun  the last Sunday in the month
		//       lastMon  the last Monday in the month
		//       Sun>=8   first Sunday on or after the eighth
		//       Sun<=25  last Sunday on or before the 25th
		// This is further complicated by the fact that we need to remain
		// backward compatible with the 1.1 FCS.  Finally, we need to minimize
		// API changes.  In order to satisfy these requirements, we support
		// three representation systems, and we translate between them.
		//
		// INTERNAL REPRESENTATION
		// This is the format SimpleTimeZone objects take after construction or
		// streaming in is complete.  Rules are represented directly, using an
		// unencoded format.  We will discuss the start rule only below; the end
		// rule is analogous.
		//   startMode      Takes on enumerated values DAY_OF_MONTH,
		//                  DOW_IN_MONTH, DOW_AFTER_DOM, or DOW_BEFORE_DOM.
		//   startDay       The day of the month, or for DOW_IN_MONTH mode, a
		//                  value indicating which DOW, such as +1 for first,
		//                  +2 for second, -1 for last, etc.
		//   startDayOfWeek The day of the week.  Ignored for DAY_OF_MONTH.
		//
		// ENCODED REPRESENTATION
		// This is the format accepted by the constructor and by setStartRule()
		// and setEndRule().  It uses various combinations of positive, negative,
		// and zero values to encode the different rules.  This representation
		// allows us to specify all the different rule flavors without altering
		// the API.
		//   MODE              startMonth    startDay    startDayOfWeek
		//   DOW_IN_MONTH_MODE >=0           !=0         >0
		//   DOM_MODE          >=0           >0          ==0
		//   DOW_GE_DOM_MODE   >=0           >0          <0
		//   DOW_LE_DOM_MODE   >=0           <0          <0
		//   (no DST)          don't care    ==0         don't care
		//
		// STREAMED REPRESENTATION
		// We must retain binary compatibility with the 1.1 FCS.  The 1.1 code only
		// handles DOW_IN_MONTH_MODE and non-DST mode, the latter indicated by the
		// flag useDaylight.  When we stream an object out, we translate into an
		// approximate DOW_IN_MONTH_MODE representation so the object can be parsed
		// and used by 1.1 code.  Following that, we write out the full
		// representation separately so that contemporary code can recognize and
		// parse it.  The full representation is written in a "packed" format,
		// consisting of a version number, a length, and an array of bytes.  Future
		// versions of this class may specify different versions.  If they wish to
		// include additional data, they should do so by storing them after the
		// packed representation below.
		//----------------------------------------------------------------------

		/// <summary>
		/// Given a set of encoded rules in startDay and startDayOfMonth, decode
		/// them and set the startMode appropriately.  Do the same for endDay and
		/// endDayOfMonth.  Upon entry, the day of week variables may be zero or
		/// negative, in order to indicate special modes.  The day of month
		/// variables may also be negative.  Upon exit, the mode variables will be
		/// set, and the day of week and day of month variables will be positive.
		/// This method also recognizes a startDay or endDay of zero as indicating
		/// no DST.
		/// </summary>
		private void DecodeRules()
		{
			DecodeStartRule();
			DecodeEndRule();
		}

		/// <summary>
		/// Decode the start rule and validate the parameters.  The parameters are
		/// expected to be in encoded form, which represents the various rule modes
		/// by negating or zeroing certain values.  Representation formats are:
		/// <para>
		/// <pre>
		///            DOW_IN_MONTH  DOM    DOW>=DOM  DOW<=DOM  no DST
		///            ------------  -----  --------  --------  ----------
		/// month       0..11        same    same      same     don't care
		/// day        -5..5         1..31   1..31    -1..-31   0
		/// dayOfWeek   1..7         0      -1..-7    -1..-7    don't care
		/// time        0..ONEDAY    same    same      same     don't care
		/// </pre>
		/// The range for month does not include UNDECIMBER since this class is
		/// really specific to GregorianCalendar, which does not use that month.
		/// The range for time includes ONEDAY (vs. ending at ONEDAY-1) because the
		/// end rule is an exclusive limit point.  That is, the range of times that
		/// are in DST include those >= the start and < the end.  For this reason,
		/// it should be possible to specify an end of ONEDAY in order to include the
		/// entire day.  Although this is equivalent to time 0 of the following day,
		/// it's not always possible to specify that, for example, on December 31.
		/// While arguably the start range should still be 0..ONEDAY-1, we keep
		/// the start and end ranges the same for consistency.
		/// </para>
		/// </summary>
		private void DecodeStartRule()
		{
			UseDaylight = (StartDay != 0) && (EndDay != 0);
			if (StartDay != 0)
			{
				if (StartMonth < 1 || StartMonth > 12)
				{
					throw new IllegalArgumentException("Illegal start month " + StartMonth);
				}
				if (StartTime < 0 || StartTime > MillisPerDay)
				{
					throw new IllegalArgumentException("Illegal start time " + StartTime);
				}
				if (StartDayOfWeek == 0)
				{
					StartMode = DOM_MODE;
				}
				else
				{
					if (StartDayOfWeek > 0)
					{
						StartMode = DOW_IN_MONTH_MODE;
					}
					else
					{
						StartDayOfWeek = -StartDayOfWeek;
						if (StartDay > 0)
						{
							StartMode = DOW_GE_DOM_MODE;
						}
						else
						{
							StartDay = -StartDay;
							StartMode = DOW_LE_DOM_MODE;
						}
					}
					if (StartDayOfWeek > DayOfWeek.Saturday)
					{
						throw new IllegalArgumentException("Illegal start day of week " + StartDayOfWeek);
					}
				}
				if (StartMode == DOW_IN_MONTH_MODE)
				{
					if (StartDay < -5 || StartDay > 5)
					{
						throw new IllegalArgumentException("Illegal start day of week in month " + StartDay);
					}
				}
				else if (StartDay < 1 || StartDay > StaticMonthLength[StartMonth])
				{
					throw new IllegalArgumentException("Illegal start day " + StartDay);
				}
			}
		}

		/// <summary>
		/// Decode the end rule and validate the parameters.  This method is exactly
		/// analogous to decodeStartRule(). </summary>
		/// <seealso cref= decodeStartRule </seealso>
		private void DecodeEndRule()
		{
			UseDaylight = (StartDay != 0) && (EndDay != 0);
			if (EndDay != 0)
			{
				if (EndMonth < 1 || EndMonth > 12)
				{
					throw new IllegalArgumentException("Illegal end month " + EndMonth);
				}
				if (EndTime < 0 || EndTime > MillisPerDay)
				{
					throw new IllegalArgumentException("Illegal end time " + EndTime);
				}
				if (EndDayOfWeek == 0)
				{
					EndMode = DOM_MODE;
				}
				else
				{
					if (EndDayOfWeek > 0)
					{
						EndMode = DOW_IN_MONTH_MODE;
					}
					else
					{
						EndDayOfWeek = -EndDayOfWeek;
						if (EndDay > 0)
						{
							EndMode = DOW_GE_DOM_MODE;
						}
						else
						{
							EndDay = -EndDay;
							EndMode = DOW_LE_DOM_MODE;
						}
					}
					if (EndDayOfWeek > DayOfWeek.Saturday)
					{
						throw new IllegalArgumentException("Illegal end day of week " + EndDayOfWeek);
					}
				}
				if (EndMode == DOW_IN_MONTH_MODE)
				{
					if (EndDay < -5 || EndDay > 5)
					{
						throw new IllegalArgumentException("Illegal end day of week in month " + EndDay);
					}
				}
				else if (EndDay < 1 || EndDay > StaticMonthLength[EndMonth])
				{
					throw new IllegalArgumentException("Illegal end day " + EndDay);
				}
			}
		}

		/// <summary>
		/// Make rules compatible to 1.1 FCS code.  Since 1.1 FCS code only understands
		/// day-of-week-in-month rules, we must modify other modes of rules to their
		/// approximate equivalent in 1.1 FCS terms.  This method is used when streaming
		/// out objects of this class.  After it is called, the rules will be modified,
		/// with a possible loss of information.  startMode and endMode will NOT be
		/// altered, even though semantically they should be set to DOW_IN_MONTH_MODE,
		/// since the rule modification is only intended to be temporary.
		/// </summary>
		private void MakeRulesCompatible()
		{
			switch (StartMode)
			{
			case DOM_MODE:
				StartDay = 1 + (StartDay / 7);
				StartDayOfWeek = DayOfWeek.Sunday;
				break;

			case DOW_GE_DOM_MODE:
				// A day-of-month of 1 is equivalent to DOW_IN_MONTH_MODE
				// that is, Sun>=1 == firstSun.
				if (StartDay != 1)
				{
					StartDay = 1 + (StartDay / 7);
				}
				break;

			case DOW_LE_DOM_MODE:
				if (StartDay >= 30)
				{
					StartDay = -1;
				}
				else
				{
					StartDay = 1 + (StartDay / 7);
				}
				break;
			}

			switch (EndMode)
			{
			case DOM_MODE:
				EndDay = 1 + (EndDay / 7);
				EndDayOfWeek = DayOfWeek.Sunday;
				break;

			case DOW_GE_DOM_MODE:
				// A day-of-month of 1 is equivalent to DOW_IN_MONTH_MODE
				// that is, Sun>=1 == firstSun.
				if (EndDay != 1)
				{
					EndDay = 1 + (EndDay / 7);
				}
				break;

			case DOW_LE_DOM_MODE:
				if (EndDay >= 30)
				{
					EndDay = -1;
				}
				else
				{
					EndDay = 1 + (EndDay / 7);
				}
				break;
			}

			/*
			 * Adjust the start and end times to wall time.  This works perfectly
			 * well unless it pushes into the next or previous day.  If that
			 * happens, we attempt to adjust the day rule somewhat crudely.  The day
			 * rules have been forced into DOW_IN_MONTH mode already, so we change
			 * the day of week to move forward or back by a day.  It's possible to
			 * make a more refined adjustment of the original rules first, but in
			 * most cases this extra effort will go to waste once we adjust the day
			 * rules anyway.
			 */
			switch (StartTimeMode)
			{
			case UTC_TIME:
				StartTime += RawOffset_Renamed;
				break;
			}
			while (StartTime < 0)
			{
				StartTime += MillisPerDay;
				StartDayOfWeek = 1 + ((StartDayOfWeek + 5) % 7); // Back 1 day
			}
			while (StartTime >= MillisPerDay)
			{
				StartTime -= MillisPerDay;
				StartDayOfWeek = 1 + (StartDayOfWeek % 7); // Forward 1 day
			}

			switch (EndTimeMode)
			{
			case UTC_TIME:
				EndTime += RawOffset_Renamed + DstSavings;
				break;
			case STANDARD_TIME:
				EndTime += DstSavings;
			break;
			}
			while (EndTime < 0)
			{
				EndTime += MillisPerDay;
				EndDayOfWeek = 1 + ((EndDayOfWeek + 5) % 7); // Back 1 day
			}
			while (EndTime >= MillisPerDay)
			{
				EndTime -= MillisPerDay;
				EndDayOfWeek = 1 + (EndDayOfWeek % 7); // Forward 1 day
			}
		}

		/// <summary>
		/// Pack the start and end rules into an array of bytes.  Only pack
		/// data which is not preserved by makeRulesCompatible.
		/// </summary>
		private sbyte[] PackRules()
		{
			sbyte[] rules = new sbyte[6];
			rules[0] = (sbyte)StartDay;
			rules[1] = (sbyte)StartDayOfWeek;
			rules[2] = (sbyte)EndDay;
			rules[3] = (sbyte)EndDayOfWeek;

			// As of serial version 2, include time modes
			rules[4] = (sbyte)StartTimeMode;
			rules[5] = (sbyte)EndTimeMode;

			return rules;
		}

		/// <summary>
		/// Given an array of bytes produced by packRules, interpret them
		/// as the start and end rules.
		/// </summary>
		private void UnpackRules(sbyte[] rules)
		{
			StartDay = rules[0];
			StartDayOfWeek = rules[1];
			EndDay = rules[2];
			EndDayOfWeek = rules[3];

			// As of serial version 2, include time modes
			if (rules.Length >= 6)
			{
				StartTimeMode = rules[4];
				EndTimeMode = rules[5];
			}
		}

		/// <summary>
		/// Pack the start and end times into an array of bytes.  This is required
		/// as of serial version 2.
		/// </summary>
		private int[] PackTimes()
		{
			int[] times = new int[2];
			times[0] = StartTime;
			times[1] = EndTime;
			return times;
		}

		/// <summary>
		/// Unpack the start and end times from an array of bytes.  This is required
		/// as of serial version 2.
		/// </summary>
		private void UnpackTimes(int[] times)
		{
			StartTime = times[0];
			EndTime = times[1];
		}

		/// <summary>
		/// Save the state of this object to a stream (i.e., serialize it).
		/// 
		/// @serialData We write out two formats, a JDK 1.1 compatible format, using
		/// <code>DOW_IN_MONTH_MODE</code> rules, in the required section, followed
		/// by the full rules, in packed format, in the optional section.  The
		/// optional section will be ignored by JDK 1.1 code upon stream in.
		/// <para> Contents of the optional section: The length of a byte array is
		/// emitted (int); this is 4 as of this release. The byte array of the given
		/// length is emitted. The contents of the byte array are the true values of
		/// the fields <code>startDay</code>, <code>startDayOfWeek</code>,
		/// <code>endDay</code>, and <code>endDayOfWeek</code>.  The values of these
		/// fields in the required section are approximate values suited to the rule
		/// mode <code>DOW_IN_MONTH_MODE</code>, which is the only mode recognized by
		/// JDK 1.1.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		private void WriteObject(ObjectOutputStream stream)
		{
			// Construct a binary rule
			sbyte[] rules = PackRules();
			int[] times = PackTimes();

			// Convert to 1.1 FCS rules.  This step may cause us to lose information.
			MakeRulesCompatible();

			// Write out the 1.1 FCS rules
			stream.DefaultWriteObject();

			// Write out the binary rules in the optional data area of the stream.
			stream.WriteInt(rules.Length);
			stream.Write(rules);
			stream.WriteObject(times);

			// Recover the original rules.  This recovers the information lost
			// by makeRulesCompatible.
			UnpackRules(rules);
			UnpackTimes(times);
		}

		/// <summary>
		/// Reconstitute this object from a stream (i.e., deserialize it).
		/// 
		/// We handle both JDK 1.1
		/// binary formats and full formats with a packed byte array.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();

			if (SerialVersionOnStream < 1)
			{
				// Fix a bug in the 1.1 SimpleTimeZone code -- namely,
				// startDayOfWeek and endDayOfWeek were usually uninitialized.  We can't do
				// too much, so we assume SUNDAY, which actually works most of the time.
				if (StartDayOfWeek == 0)
				{
					StartDayOfWeek = DayOfWeek.Sunday;
				}
				if (EndDayOfWeek == 0)
				{
					EndDayOfWeek = DayOfWeek.Sunday;
				}

				// The variables dstSavings, startMode, and endMode are post-1.1, so they
				// won't be present if we're reading from a 1.1 stream.  Fix them up.
				StartMode = EndMode = DOW_IN_MONTH_MODE;
				DstSavings = MillisPerHour;
			}
			else
			{
				// For 1.1.4, in addition to the 3 new instance variables, we also
				// store the actual rules (which have not be made compatible with 1.1)
				// in the optional area.  Read them in here and parse them.
				int length = stream.ReadInt();
				sbyte[] rules = new sbyte[length];
				stream.ReadFully(rules);
				UnpackRules(rules);
			}

			if (SerialVersionOnStream >= 2)
			{
				int[] times = (int[]) stream.ReadObject();
				UnpackTimes(times);
			}

			SerialVersionOnStream = CurrentSerialVersion;
		}
	}

}