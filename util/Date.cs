using System;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	using BaseCalendar = sun.util.calendar.BaseCalendar;
	using CalendarDate = sun.util.calendar.CalendarDate;
	using CalendarSystem = sun.util.calendar.CalendarSystem;
	using CalendarUtils = sun.util.calendar.CalendarUtils;
	using Era = sun.util.calendar.Era;
	using Gregorian = sun.util.calendar.Gregorian;
	using ZoneInfo = sun.util.calendar.ZoneInfo;

	/// <summary>
	/// The class <code>Date</code> represents a specific instant
	/// in time, with millisecond precision.
	/// <para>
	/// Prior to JDK&nbsp;1.1, the class <code>Date</code> had two additional
	/// functions.  It allowed the interpretation of dates as year, month, day, hour,
	/// minute, and second values.  It also allowed the formatting and parsing
	/// of date strings.  Unfortunately, the API for these functions was not
	/// amenable to internationalization.  As of JDK&nbsp;1.1, the
	/// <code>Calendar</code> class should be used to convert between dates and time
	/// fields and the <code>DateFormat</code> class should be used to format and
	/// parse date strings.
	/// The corresponding methods in <code>Date</code> are deprecated.
	/// </para>
	/// <para>
	/// Although the <code>Date</code> class is intended to reflect
	/// coordinated universal time (UTC), it may not do so exactly,
	/// depending on the host environment of the Java Virtual Machine.
	/// Nearly all modern operating systems assume that 1&nbsp;day&nbsp;=
	/// 24&nbsp;&times;&nbsp;60&nbsp;&times;&nbsp;60&nbsp;= 86400 seconds
	/// in all cases. In UTC, however, about once every year or two there
	/// is an extra second, called a "leap second." The leap
	/// second is always added as the last second of the day, and always
	/// on December 31 or June 30. For example, the last minute of the
	/// year 1995 was 61 seconds long, thanks to an added leap second.
	/// Most computer clocks are not accurate enough to be able to reflect
	/// the leap-second distinction.
	/// </para>
	/// <para>
	/// Some computer standards are defined in terms of Greenwich mean
	/// time (GMT), which is equivalent to universal time (UT).  GMT is
	/// the "civil" name for the standard; UT is the
	/// "scientific" name for the same standard. The
	/// distinction between UTC and UT is that UTC is based on an atomic
	/// clock and UT is based on astronomical observations, which for all
	/// practical purposes is an invisibly fine hair to split. Because the
	/// earth's rotation is not uniform (it slows down and speeds up
	/// in complicated ways), UT does not always flow uniformly. Leap
	/// seconds are introduced as needed into UTC so as to keep UTC within
	/// 0.9 seconds of UT1, which is a version of UT with certain
	/// corrections applied. There are other time and date systems as
	/// well; for example, the time scale used by the satellite-based
	/// global positioning system (GPS) is synchronized to UTC but is
	/// <i>not</i> adjusted for leap seconds. An interesting source of
	/// further information is the U.S. Naval Observatory, particularly
	/// the Directorate of Time at:
	/// <blockquote><pre>
	///     <a href=http://tycho.usno.navy.mil>http://tycho.usno.navy.mil</a>
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// and their definitions of "Systems of Time" at:
	/// <blockquote><pre>
	///     <a href=http://tycho.usno.navy.mil/systime.html>http://tycho.usno.navy.mil/systime.html</a>
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// In all methods of class <code>Date</code> that accept or return
	/// year, month, date, hours, minutes, and seconds values, the
	/// following representations are used:
	/// <ul>
	/// <li>A year <i>y</i> is represented by the integer
	///     <i>y</i>&nbsp;<code>-&nbsp;1900</code>.
	/// <li>A month is represented by an integer from 0 to 11; 0 is January,
	///     1 is February, and so forth; thus 11 is December.
	/// <li>A date (day of month) is represented by an integer from 1 to 31
	///     in the usual manner.
	/// <li>An hour is represented by an integer from 0 to 23. Thus, the hour
	///     from midnight to 1 a.m. is hour 0, and the hour from noon to 1
	///     p.m. is hour 12.
	/// <li>A minute is represented by an integer from 0 to 59 in the usual manner.
	/// <li>A second is represented by an integer from 0 to 61; the values 60 and
	///     61 occur only for leap seconds and even then only in Java
	///     implementations that actually track leap seconds correctly. Because
	///     of the manner in which leap seconds are currently introduced, it is
	///     extremely unlikely that two leap seconds will occur in the same
	///     minute, but this specification follows the date and time conventions
	///     for ISO C.
	/// </ul>
	/// </para>
	/// <para>
	/// In all cases, arguments given to methods for these purposes need
	/// not fall within the indicated ranges; for example, a date may be
	/// specified as January 32 and is interpreted as meaning February 1.
	/// 
	/// @author  James Gosling
	/// @author  Arthur van Hoff
	/// @author  Alan Liu
	/// </para>
	/// </summary>
	/// <seealso cref=     java.text.DateFormat </seealso>
	/// <seealso cref=     java.util.Calendar </seealso>
	/// <seealso cref=     java.util.TimeZone
	/// @since   JDK1.0 </seealso>
	[Serializable]
	public class Date : Cloneable, Comparable<Date>
	{
		private static readonly BaseCalendar Gcal = CalendarSystem.GregorianCalendar;
		private static BaseCalendar Jcal;

		[NonSerialized]
		private long FastTime;

		/*
		 * If cdate is null, then fastTime indicates the time in millis.
		 * If cdate.isNormalized() is true, then fastTime and cdate are in
		 * synch. Otherwise, fastTime is ignored, and cdate indicates the
		 * time.
		 */
		[NonSerialized]
		private BaseCalendar.Date Cdate;

		// Initialized just before the value is used. See parse().
		private static int DefaultCenturyStart;

		/* use serialVersionUID from modified java.util.Date for
		 * interoperability with JDK1.1. The Date was modified to write
		 * and read only the UTC time.
		 */
		private const long SerialVersionUID = 7523967970034938905L;

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it so that
		/// it represents the time at which it was allocated, measured to the
		/// nearest millisecond.
		/// </summary>
		/// <seealso cref=     java.lang.System#currentTimeMillis() </seealso>
		public Date() : this(DateTimeHelperClass.CurrentUnixTimeMillis())
		{
		}

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it to
		/// represent the specified number of milliseconds since the
		/// standard base time known as "the epoch", namely January 1,
		/// 1970, 00:00:00 GMT.
		/// </summary>
		/// <param name="date">   the milliseconds since January 1, 1970, 00:00:00 GMT. </param>
		/// <seealso cref=     java.lang.System#currentTimeMillis() </seealso>
		public Date(long date)
		{
			FastTime = date;
		}

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it so that
		/// it represents midnight, local time, at the beginning of the day
		/// specified by the <code>year</code>, <code>month</code>, and
		/// <code>date</code> arguments.
		/// </summary>
		/// <param name="year">    the year minus 1900. </param>
		/// <param name="month">   the month between 0-11. </param>
		/// <param name="date">    the day of the month between 1-31. </param>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.set(year + 1900, month, date)</code>
		/// or <code>GregorianCalendar(year + 1900, month, date)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public Date(int year, int month, int date) : this(year, month, date, 0, 0, 0)
		{
		}

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it so that
		/// it represents the instant at the start of the minute specified by
		/// the <code>year</code>, <code>month</code>, <code>date</code>,
		/// <code>hrs</code>, and <code>min</code> arguments, in the local
		/// time zone.
		/// </summary>
		/// <param name="year">    the year minus 1900. </param>
		/// <param name="month">   the month between 0-11. </param>
		/// <param name="date">    the day of the month between 1-31. </param>
		/// <param name="hrs">     the hours between 0-23. </param>
		/// <param name="min">     the minutes between 0-59. </param>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.set(year + 1900, month, date,
		/// hrs, min)</code> or <code>GregorianCalendar(year + 1900,
		/// month, date, hrs, min)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public Date(int year, int month, int date, int hrs, int min) : this(year, month, date, hrs, min, 0)
		{
		}

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it so that
		/// it represents the instant at the start of the second specified
		/// by the <code>year</code>, <code>month</code>, <code>date</code>,
		/// <code>hrs</code>, <code>min</code>, and <code>sec</code> arguments,
		/// in the local time zone.
		/// </summary>
		/// <param name="year">    the year minus 1900. </param>
		/// <param name="month">   the month between 0-11. </param>
		/// <param name="date">    the day of the month between 1-31. </param>
		/// <param name="hrs">     the hours between 0-23. </param>
		/// <param name="min">     the minutes between 0-59. </param>
		/// <param name="sec">     the seconds between 0-59. </param>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.set(year + 1900, month, date,
		/// hrs, min, sec)</code> or <code>GregorianCalendar(year + 1900,
		/// month, date, hrs, min, sec)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public Date(int year, int month, int date, int hrs, int min, int sec)
		{
			int y = year + 1900;
			// month is 0-based. So we have to normalize month to support Long.MAX_VALUE.
			if (month >= 12)
			{
				y += month / 12;
				month %= 12;
			}
			else if (month < 0)
			{
				y += CalendarUtils.floorDivide(month, 12);
				month = CalendarUtils.mod(month, 12);
			}
			BaseCalendar cal = GetCalendarSystem(y);
			Cdate = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.DefaultRef);
			Cdate.setNormalizedDate(y, month + 1, date).setTimeOfDay(hrs, min, sec, 0);
			TimeImpl;
			Cdate = null;
		}

		/// <summary>
		/// Allocates a <code>Date</code> object and initializes it so that
		/// it represents the date and time indicated by the string
		/// <code>s</code>, which is interpreted as if by the
		/// <seealso cref="Date#parse"/> method.
		/// </summary>
		/// <param name="s">   a string representation of the date. </param>
		/// <seealso cref=     java.text.DateFormat </seealso>
		/// <seealso cref=     java.util.Date#parse(java.lang.String) </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>DateFormat.parse(String s)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public Date(String s) : this(Parse(s))
		{
		}

		/// <summary>
		/// Return a copy of this object.
		/// </summary>
		public virtual Object Clone()
		{
			Date d = null;
			try
			{
				d = (Date)base.Clone();
				if (Cdate != null)
				{
					d.Cdate = (BaseCalendar.Date) Cdate.clone();
				}
			} // Won't happen
			catch (CloneNotSupportedException)
			{
			}
			return d;
		}

		/// <summary>
		/// Determines the date and time based on the arguments. The
		/// arguments are interpreted as a year, month, day of the month,
		/// hour of the day, minute within the hour, and second within the
		/// minute, exactly as for the <tt>Date</tt> constructor with six
		/// arguments, except that the arguments are interpreted relative
		/// to UTC rather than to the local time zone. The time indicated is
		/// returned represented as the distance, measured in milliseconds,
		/// of that time from the epoch (00:00:00 GMT on January 1, 1970).
		/// </summary>
		/// <param name="year">    the year minus 1900. </param>
		/// <param name="month">   the month between 0-11. </param>
		/// <param name="date">    the day of the month between 1-31. </param>
		/// <param name="hrs">     the hours between 0-23. </param>
		/// <param name="min">     the minutes between 0-59. </param>
		/// <param name="sec">     the seconds between 0-59. </param>
		/// <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT for
		///          the date and time specified by the arguments. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.set(year + 1900, month, date,
		/// hrs, min, sec)</code> or <code>GregorianCalendar(year + 1900,
		/// month, date, hrs, min, sec)</code>, using a UTC
		/// <code>TimeZone</code>, followed by <code>Calendar.getTime().getTime()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public static long UTC(int year, int month, int date, int hrs, int min, int sec)
		{
			int y = year + 1900;
			// month is 0-based. So we have to normalize month to support Long.MAX_VALUE.
			if (month >= 12)
			{
				y += month / 12;
				month %= 12;
			}
			else if (month < 0)
			{
				y += CalendarUtils.floorDivide(month, 12);
				month = CalendarUtils.mod(month, 12);
			}
			int m = month + 1;
			BaseCalendar cal = GetCalendarSystem(y);
			BaseCalendar.Date udate = (BaseCalendar.Date) cal.newCalendarDate(null);
			udate.setNormalizedDate(y, m, date).setTimeOfDay(hrs, min, sec, 0);

			// Use a Date instance to perform normalization. Its fastTime
			// is the UTC value after the normalization.
			Date d = new DateTime();
			d.Normalize(udate);
			return d.FastTime;
		}

		/// <summary>
		/// Attempts to interpret the string <tt>s</tt> as a representation
		/// of a date and time. If the attempt is successful, the time
		/// indicated is returned represented as the distance, measured in
		/// milliseconds, of that time from the epoch (00:00:00 GMT on
		/// January 1, 1970). If the attempt fails, an
		/// <tt>IllegalArgumentException</tt> is thrown.
		/// <para>
		/// It accepts many syntaxes; in particular, it recognizes the IETF
		/// standard date syntax: "Sat, 12 Aug 1995 13:30:00 GMT". It also
		/// understands the continental U.S. time-zone abbreviations, but for
		/// general use, a time-zone offset should be used: "Sat, 12 Aug 1995
		/// 13:30:00 GMT+0430" (4 hours, 30 minutes west of the Greenwich
		/// meridian). If no time zone is specified, the local time zone is
		/// assumed. GMT and UTC are considered equivalent.
		/// </para>
		/// <para>
		/// The string <tt>s</tt> is processed from left to right, looking for
		/// data of interest. Any material in <tt>s</tt> that is within the
		/// ASCII parenthesis characters <tt>(</tt> and <tt>)</tt> is ignored.
		/// Parentheses may be nested. Otherwise, the only characters permitted
		/// within <tt>s</tt> are these ASCII characters:
		/// <blockquote><pre>
		/// abcdefghijklmnopqrstuvwxyz
		/// ABCDEFGHIJKLMNOPQRSTUVWXYZ
		/// 0123456789,+-:/</pre></blockquote>
		/// </para>
		/// and whitespace characters.<para>
		/// A consecutive sequence of decimal digits is treated as a decimal
		/// number:<ul>
		/// <li>If a number is preceded by <tt>+</tt> or <tt>-</tt> and a year
		///     has already been recognized, then the number is a time-zone
		///     offset. If the number is less than 24, it is an offset measured
		///     in hours. Otherwise, it is regarded as an offset in minutes,
		///     expressed in 24-hour time format without punctuation. A
		///     preceding <tt>-</tt> means a westward offset. Time zone offsets
		///     are always relative to UTC (Greenwich). Thus, for example,
		///     <tt>-5</tt> occurring in the string would mean "five hours west
		///     of Greenwich" and <tt>+0430</tt> would mean "four hours and
		///     thirty minutes east of Greenwich." It is permitted for the
		///     string to specify <tt>GMT</tt>, <tt>UT</tt>, or <tt>UTC</tt>
		///     redundantly-for example, <tt>GMT-5</tt> or <tt>utc+0430</tt>.
		/// <li>The number is regarded as a year number if one of the
		///     following conditions is true:
		/// <ul>
		///     <li>The number is equal to or greater than 70 and followed by a
		///         space, comma, slash, or end of string
		///     <li>The number is less than 70, and both a month and a day of
		///         the month have already been recognized</li>
		/// </ul>
		///     If the recognized year number is less than 100, it is
		///     interpreted as an abbreviated year relative to a century of
		///     which dates are within 80 years before and 19 years after
		///     the time when the Date class is initialized.
		///     After adjusting the year number, 1900 is subtracted from
		///     it. For example, if the current year is 1999 then years in
		///     the range 19 to 99 are assumed to mean 1919 to 1999, while
		///     years from 0 to 18 are assumed to mean 2000 to 2018.  Note
		///     that this is slightly different from the interpretation of
		///     years less than 100 that is used in <seealso cref="java.text.SimpleDateFormat"/>.
		/// <li>If the number is followed by a colon, it is regarded as an hour,
		///     unless an hour has already been recognized, in which case it is
		///     regarded as a minute.
		/// <li>If the number is followed by a slash, it is regarded as a month
		///     (it is decreased by 1 to produce a number in the range <tt>0</tt>
		///     to <tt>11</tt>), unless a month has already been recognized, in
		///     which case it is regarded as a day of the month.
		/// <li>If the number is followed by whitespace, a comma, a hyphen, or
		///     end of string, then if an hour has been recognized but not a
		///     minute, it is regarded as a minute; otherwise, if a minute has
		///     been recognized but not a second, it is regarded as a second;
		/// </para>
		///     otherwise, it is regarded as a day of the month. </ul><para>
		/// A consecutive sequence of letters is regarded as a word and treated
		/// as follows:<ul>
		/// <li>A word that matches <tt>AM</tt>, ignoring case, is ignored (but
		///     the parse fails if an hour has not been recognized or is less
		///     than <tt>1</tt> or greater than <tt>12</tt>).
		/// <li>A word that matches <tt>PM</tt>, ignoring case, adds <tt>12</tt>
		///     to the hour (but the parse fails if an hour has not been
		///     recognized or is less than <tt>1</tt> or greater than <tt>12</tt>).
		/// <li>Any word that matches any prefix of <tt>SUNDAY, MONDAY, TUESDAY,
		///     WEDNESDAY, THURSDAY, FRIDAY</tt>, or <tt>SATURDAY</tt>, ignoring
		///     case, is ignored. For example, <tt>sat, Friday, TUE</tt>, and
		///     <tt>Thurs</tt> are ignored.
		/// <li>Otherwise, any word that matches any prefix of <tt>JANUARY,
		///     FEBRUARY, MARCH, APRIL, MAY, JUNE, JULY, AUGUST, SEPTEMBER,
		///     OCTOBER, NOVEMBER</tt>, or <tt>DECEMBER</tt>, ignoring case, and
		///     considering them in the order given here, is recognized as
		///     specifying a month and is converted to a number (<tt>0</tt> to
		///     <tt>11</tt>). For example, <tt>aug, Sept, april</tt>, and
		///     <tt>NOV</tt> are recognized as months. So is <tt>Ma</tt>, which
		///     is recognized as <tt>MARCH</tt>, not <tt>MAY</tt>.
		/// <li>Any word that matches <tt>GMT, UT</tt>, or <tt>UTC</tt>, ignoring
		///     case, is treated as referring to UTC.
		/// <li>Any word that matches <tt>EST, CST, MST</tt>, or <tt>PST</tt>,
		///     ignoring case, is recognized as referring to the time zone in
		///     North America that is five, six, seven, or eight hours west of
		///     Greenwich, respectively. Any word that matches <tt>EDT, CDT,
		///     MDT</tt>, or <tt>PDT</tt>, ignoring case, is recognized as
		///     referring to the same time zone, respectively, during daylight
		/// </para>
		///     saving time.</ul><para>
		/// Once the entire string s has been scanned, it is converted to a time
		/// result in one of two ways. If a time zone or time-zone offset has been
		/// recognized, then the year, month, day of month, hour, minute, and
		/// second are interpreted in UTC and then the time-zone offset is
		/// applied. Otherwise, the year, month, day of month, hour, minute, and
		/// second are interpreted in the local time zone.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s">   a string to be parsed as a date. </param>
		/// <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT
		///          represented by the string argument. </returns>
		/// <seealso cref=     java.text.DateFormat </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>DateFormat.parse(String s)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public static long Parse(String s)
		{
			int year = Integer.MinValue;
			int mon = -1;
			int mday = -1;
			int hour = -1;
			int min = -1;
			int sec = -1;
			int millis = -1;
			int c = -1;
			int i = 0;
			int n = -1;
			int wst = -1;
			int tzoffset = -1;
			int prevc = 0;
			{
				if (s == null)
				{
					goto syntaxBreak;
				}
				int limit = s.Length();
				while (i < limit)
				{
					c = s.CharAt(i);
					i++;
					if (c <= ' ' || c == ',')
					{
						continue;
					}
					if (c == '(') // skip comments
					{
						int depth = 1;
						while (i < limit)
						{
							c = s.CharAt(i);
							i++;
							if (c == '(')
							{
								depth++;
							}
							else if (c == ')')
							{
								if (--depth <= 0)
								{
									break;
								}
							}
						}
						continue;
					}
					if ('0' <= c && c <= '9')
					{
						n = c - '0';
						while (i < limit && '0' <= (c = s.CharAt(i)) && c <= '9')
						{
							n = n * 10 + c - '0';
							i++;
						}
						if (prevc == '+' || prevc == '-' && year != Integer.MinValue)
						{
							// timezone offset
							if (n < 24)
							{
								n = n * 60; // EG. "GMT-3"
							}
							else
							{
								n = n % 100 + n / 100 * 60; // eg "GMT-0430"
							}
							if (prevc == '+') // plus means east of GMT
							{
								n = -n;
							}
							if (tzoffset != 0 && tzoffset != -1)
							{
								goto syntaxBreak;
							}
							tzoffset = n;
						}
						else if (n >= 70)
						{
							if (year != Integer.MinValue)
							{
								goto syntaxBreak;
							}
							else if (c <= ' ' || c == ',' || c == '/' || i >= limit)
							{
								// year = n < 1900 ? n : n - 1900;
								year = n;
							}
							else
							{
								goto syntaxBreak;
							}
						}
						else if (c == ':')
						{
							if (hour < 0)
							{
								hour = (sbyte) n;
							}
							else if (min < 0)
							{
								min = (sbyte) n;
							}
							else
							{
								goto syntaxBreak;
							}
						}
						else if (c == '/')
						{
							if (mon < 0)
							{
								mon = (sbyte)(n - 1);
							}
							else if (mday < 0)
							{
								mday = (sbyte) n;
							}
							else
							{
								goto syntaxBreak;
							}
						}
						else if (i < limit && c != ',' && c > ' ' && c != '-')
						{
							goto syntaxBreak;
						}
						else if (hour >= 0 && min < 0)
						{
							min = (sbyte) n;
						}
						else if (min >= 0 && sec < 0)
						{
							sec = (sbyte) n;
						}
						else if (mday < 0)
						{
							mday = (sbyte) n;
						}
						// Handle two-digit years < 70 (70-99 handled above).
						else if (year == Integer.MinValue && mon >= 0 && mday >= 0)
						{
							year = n;
						}
						else
						{
							goto syntaxBreak;
						}
						prevc = 0;
					}
					else if (c == '/' || c == ':' || c == '+' || c == '-')
					{
						prevc = c;
					}
					else
					{
						int st = i - 1;
						while (i < limit)
						{
							c = s.CharAt(i);
							if (!('A' <= c && c <= 'Z' || 'a' <= c && c <= 'z'))
							{
								break;
							}
							i++;
						}
						if (i <= st + 1)
						{
							goto syntaxBreak;
						}
						int k;
						for (k = Wtb.Length; --k >= 0;)
						{
							if (Wtb[k].RegionMatches(true, 0, s, st, i - st))
							{
								int action = Ttb[k];
								if (action != 0)
								{
									if (action == 1) // pm
									{
										if (hour > 12 || hour < 1)
										{
											goto syntaxBreak;
										}
										else if (hour < 12)
										{
											hour += 12;
										}
									} // am
									else if (action == 14)
									{
										if (hour > 12 || hour < 1)
										{
											goto syntaxBreak;
										}
										else if (hour == 12)
										{
											hour = 0;
										}
									} // month!
									else if (action <= 13)
									{
										if (mon < 0)
										{
											mon = (sbyte)(action - 2);
										}
										else
										{
											goto syntaxBreak;
										}
									}
									else
									{
										tzoffset = action - 10000;
									}
								}
								break;
							}
						}
						if (k < 0)
						{
							goto syntaxBreak;
						}
						prevc = 0;
					}
				}
				if (year == Integer.MinValue || mon < 0 || mday < 0)
				{
					goto syntaxBreak;
				}
				// Parse 2-digit years within the correct default century.
				if (year < 100)
				{
					lock (typeof(Date))
					{
						if (DefaultCenturyStart == 0)
						{
							DefaultCenturyStart = Gcal.CalendarDate.Year - 80;
						}
					}
					year += (DefaultCenturyStart / 100) * 100;
					if (year < DefaultCenturyStart)
					{
						year += 100;
					}
				}
				if (sec < 0)
				{
					sec = 0;
				}
				if (min < 0)
				{
					min = 0;
				}
				if (hour < 0)
				{
					hour = 0;
				}
				BaseCalendar cal = GetCalendarSystem(year);
				if (tzoffset == -1) // no time zone specified, have to use local
				{
					BaseCalendar.Date ldate = (BaseCalendar.Date) cal.newCalendarDate(TimeZone.DefaultRef);
					ldate.setDate(year, mon + 1, mday);
					ldate.setTimeOfDay(hour, min, sec, 0);
					return cal.getTime(ldate);
				}
				BaseCalendar.Date udate = (BaseCalendar.Date) cal.newCalendarDate(null); // no time zone
				udate.setDate(year, mon + 1, mday);
				udate.setTimeOfDay(hour, min, sec, 0);
				return cal.getTime(udate) + tzoffset * (60 * 1000);
			}
		syntaxBreak:
			// syntax error
			throw new IllegalArgumentException();
		}
		private static readonly String[] Wtb = new String[] {"am", "pm", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday", "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december", "gmt", "ut", "utc", "est", "edt", "cst", "cdt", "mst", "mdt", "pst", "pdt"};
		private static readonly int[] Ttb = new int[] {14, 1, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 10000 + 0, 10000 + 0, 10000 + 0, 10000 + 5 * 60, 10000 + 4 * 60, 10000 + 6 * 60, 10000 + 5 * 60, 10000 + 7 * 60, 10000 + 6 * 60, 10000 + 8 * 60, 10000 + 7 * 60};

		/// <summary>
		/// Returns a value that is the result of subtracting 1900 from the
		/// year that contains or begins with the instant in time represented
		/// by this <code>Date</code> object, as interpreted in the local
		/// time zone.
		/// </summary>
		/// <returns>  the year represented by this date, minus 1900. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.YEAR) - 1900</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Year
		{
			get
			{
				return Normalize().Year - 1900;
			}
			set
			{
				CalendarDate.NormalizedYear = value + 1900;
			}
		}


		/// <summary>
		/// Returns a number representing the month that contains or begins
		/// with the instant in time represented by this <tt>Date</tt> object.
		/// The value returned is between <code>0</code> and <code>11</code>,
		/// with the value <code>0</code> representing January.
		/// </summary>
		/// <returns>  the month represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.MONTH)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Month
		{
			get
			{
				return Normalize().Month - 1; // adjust 1-based to 0-based
			}
			set
			{
				int y = 0;
				if (value >= 12)
				{
					y = value / 12;
					value %= 12;
				}
				else if (value < 0)
				{
					y = CalendarUtils.floorDivide(value, 12);
					value = CalendarUtils.mod(value, 12);
				}
				BaseCalendar.Date d = CalendarDate;
				if (y != 0)
				{
					d.NormalizedYear = d.NormalizedYear + y;
				}
				d.Month = value + 1; // adjust 0-based to 1-based value numbering
			}
		}


		/// <summary>
		/// Returns the day of the month represented by this <tt>Date</tt> object.
		/// The value returned is between <code>1</code> and <code>31</code>
		/// representing the day of the month that contains or begins with the
		/// instant in time represented by this <tt>Date</tt> object, as
		/// interpreted in the local time zone.
		/// </summary>
		/// <returns>  the day of the month represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.DAY_OF_MONTH)</code>.
		/// @deprecated 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Date
		{
			get
			{
				return Normalize().DayOfMonth;
			}
			set
			{
				CalendarDate.DayOfMonth = value;
			}
		}


		/// <summary>
		/// Returns the day of the week represented by this date. The
		/// returned value (<tt>0</tt> = Sunday, <tt>1</tt> = Monday,
		/// <tt>2</tt> = Tuesday, <tt>3</tt> = Wednesday, <tt>4</tt> =
		/// Thursday, <tt>5</tt> = Friday, <tt>6</tt> = Saturday)
		/// represents the day of the week that contains or begins with
		/// the instant in time represented by this <tt>Date</tt> object,
		/// as interpreted in the local time zone.
		/// </summary>
		/// <returns>  the day of the week represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.DAY_OF_WEEK)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Day
		{
			get
			{
				return Normalize().DayOfWeek - BaseCalendar.SUNDAY;
			}
		}

		/// <summary>
		/// Returns the hour represented by this <tt>Date</tt> object. The
		/// returned value is a number (<tt>0</tt> through <tt>23</tt>)
		/// representing the hour within the day that contains or begins
		/// with the instant in time represented by this <tt>Date</tt>
		/// object, as interpreted in the local time zone.
		/// </summary>
		/// <returns>  the hour represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.HOUR_OF_DAY)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Hours
		{
			get
			{
				return Normalize().Hours;
			}
			set
			{
				CalendarDate.Hours = value;
			}
		}


		/// <summary>
		/// Returns the number of minutes past the hour represented by this date,
		/// as interpreted in the local time zone.
		/// The value returned is between <code>0</code> and <code>59</code>.
		/// </summary>
		/// <returns>  the number of minutes past the hour represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.MINUTE)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Minutes
		{
			get
			{
				return Normalize().Minutes;
			}
			set
			{
				CalendarDate.Minutes = value;
			}
		}


		/// <summary>
		/// Returns the number of seconds past the minute represented by this date.
		/// The value returned is between <code>0</code> and <code>61</code>. The
		/// values <code>60</code> and <code>61</code> can only occur on those
		/// Java Virtual Machines that take leap seconds into account.
		/// </summary>
		/// <returns>  the number of seconds past the minute represented by this date. </returns>
		/// <seealso cref=     java.util.Calendar </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Calendar.get(Calendar.SECOND)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Seconds
		{
			get
			{
				return Normalize().Seconds;
			}
			set
			{
				CalendarDate.Seconds = value;
			}
		}


		/// <summary>
		/// Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT
		/// represented by this <tt>Date</tt> object.
		/// </summary>
		/// <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT
		///          represented by this date. </returns>
		public virtual long Time
		{
			get
			{
				return TimeImpl;
			}
			set
			{
				FastTime = value;
				Cdate = null;
			}
		}

		private long TimeImpl
		{
			get
			{
				if (Cdate != null && !Cdate.Normalized)
				{
					Normalize();
				}
				return FastTime;
			}
		}


		/// <summary>
		/// Tests if this date is before the specified date.
		/// </summary>
		/// <param name="when">   a date. </param>
		/// <returns>  <code>true</code> if and only if the instant of time
		///            represented by this <tt>Date</tt> object is strictly
		///            earlier than the instant represented by <tt>when</tt>;
		///          <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException"> if <code>when</code> is null. </exception>
		public virtual bool Before(Date when)
		{
			return GetMillisOf(this) < GetMillisOf(when);
		}

		/// <summary>
		/// Tests if this date is after the specified date.
		/// </summary>
		/// <param name="when">   a date. </param>
		/// <returns>  <code>true</code> if and only if the instant represented
		///          by this <tt>Date</tt> object is strictly later than the
		///          instant represented by <tt>when</tt>;
		///          <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException"> if <code>when</code> is null. </exception>
		public virtual bool After(Date when)
		{
			return GetMillisOf(this) > GetMillisOf(when);
		}

		/// <summary>
		/// Compares two dates for equality.
		/// The result is <code>true</code> if and only if the argument is
		/// not <code>null</code> and is a <code>Date</code> object that
		/// represents the same point in time, to the millisecond, as this object.
		/// <para>
		/// Thus, two <code>Date</code> objects are equal if and only if the
		/// <code>getTime</code> method returns the same <code>long</code>
		/// value for both.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare with. </param>
		/// <returns>  <code>true</code> if the objects are the same;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.util.Date#getTime() </seealso>
		public override bool Equals(Object obj)
		{
			return obj is Date && Time == ((Date) obj).Time;
		}

		/// <summary>
		/// Returns the millisecond value of this <code>Date</code> object
		/// without affecting its internal state.
		/// </summary>
		internal static long GetMillisOf(Date date)
		{
			if (date.Cdate == null || date.Cdate.Normalized)
			{
				return date.FastTime;
			}
			BaseCalendar.Date d = (BaseCalendar.Date) date.Cdate.clone();
			return Gcal.getTime(d);
		}

		/// <summary>
		/// Compares two Dates for ordering.
		/// </summary>
		/// <param name="anotherDate">   the <code>Date</code> to be compared. </param>
		/// <returns>  the value <code>0</code> if the argument Date is equal to
		///          this Date; a value less than <code>0</code> if this Date
		///          is before the Date argument; and a value greater than
		///      <code>0</code> if this Date is after the Date argument.
		/// @since   1.2 </returns>
		/// <exception cref="NullPointerException"> if <code>anotherDate</code> is null. </exception>
		public virtual int CompareTo(Date anotherDate)
		{
			long thisTime = GetMillisOf(this);
			long anotherTime = GetMillisOf(anotherDate);
			return (thisTime < anotherTime ? - 1 : (thisTime == anotherTime ? 0 : 1));
		}

		/// <summary>
		/// Returns a hash code value for this object. The result is the
		/// exclusive OR of the two halves of the primitive <tt>long</tt>
		/// value returned by the <seealso cref="Date#getTime"/>
		/// method. That is, the hash code is the value of the expression:
		/// <blockquote><pre>{@code
		/// (int)(this.getTime()^(this.getTime() >>> 32))
		/// }</pre></blockquote>
		/// </summary>
		/// <returns>  a hash code value for this object. </returns>
		public override int HashCode()
		{
			long ht = this.Time;
			return (int) ht ^ (int)(ht >> 32);
		}

		/// <summary>
		/// Converts this <code>Date</code> object to a <code>String</code>
		/// of the form:
		/// <blockquote><pre>
		/// dow mon dd hh:mm:ss zzz yyyy</pre></blockquote>
		/// where:<ul>
		/// <li><tt>dow</tt> is the day of the week (<tt>Sun, Mon, Tue, Wed,
		///     Thu, Fri, Sat</tt>).
		/// <li><tt>mon</tt> is the month (<tt>Jan, Feb, Mar, Apr, May, Jun,
		///     Jul, Aug, Sep, Oct, Nov, Dec</tt>).
		/// <li><tt>dd</tt> is the day of the month (<tt>01</tt> through
		///     <tt>31</tt>), as two decimal digits.
		/// <li><tt>hh</tt> is the hour of the day (<tt>00</tt> through
		///     <tt>23</tt>), as two decimal digits.
		/// <li><tt>mm</tt> is the minute within the hour (<tt>00</tt> through
		///     <tt>59</tt>), as two decimal digits.
		/// <li><tt>ss</tt> is the second within the minute (<tt>00</tt> through
		///     <tt>61</tt>, as two decimal digits.
		/// <li><tt>zzz</tt> is the time zone (and may reflect daylight saving
		///     time). Standard time zone abbreviations include those
		///     recognized by the method <tt>parse</tt>. If time zone
		///     information is not available, then <tt>zzz</tt> is empty -
		///     that is, it consists of no characters at all.
		/// <li><tt>yyyy</tt> is the year, as four decimal digits.
		/// </ul>
		/// </summary>
		/// <returns>  a string representation of this date. </returns>
		/// <seealso cref=     java.util.Date#toLocaleString() </seealso>
		/// <seealso cref=     java.util.Date#toGMTString() </seealso>
		public override String ToString()
		{
			// "EEE MMM dd HH:mm:ss zzz yyyy";
			BaseCalendar.Date date = Normalize();
			StringBuilder sb = new StringBuilder(28);
			int index = date.DayOfWeek;
			if (index == BaseCalendar.SUNDAY)
			{
				index = 8;
			}
			ConvertToAbbr(sb, Wtb[index]).Append(' '); // EEE
			ConvertToAbbr(sb, Wtb[date.Month - 1 + 2 + 7]).Append(' '); // MMM
			CalendarUtils.sprintf0d(sb, date.DayOfMonth, 2).append(' '); // dd

			CalendarUtils.sprintf0d(sb, date.Hours, 2).append(':'); // HH
			CalendarUtils.sprintf0d(sb, date.Minutes, 2).append(':'); // mm
			CalendarUtils.sprintf0d(sb, date.Seconds, 2).append(' '); // ss
			TimeZone zi = date.Zone;
			if (zi != null)
			{
				sb.Append(zi.GetDisplayName(date.DaylightTime, TimeZone.SHORT, Locale.US)); // zzz
			}
			else
			{
				sb.Append("GMT");
			}
			sb.Append(' ').Append(date.Year); // yyyy
			return sb.ToString();
		}

		/// <summary>
		/// Converts the given name to its 3-letter abbreviation (e.g.,
		/// "monday" -> "Mon") and stored the abbreviation in the given
		/// <code>StringBuilder</code>.
		/// </summary>
		private static StringBuilder ConvertToAbbr(StringBuilder sb, String name)
		{
			sb.Append(char.ToUpper(name.CharAt(0)));
			sb.Append(name.CharAt(1)).Append(name.CharAt(2));
			return sb;
		}

		/// <summary>
		/// Creates a string representation of this <tt>Date</tt> object in an
		/// implementation-dependent form. The intent is that the form should
		/// be familiar to the user of the Java application, wherever it may
		/// happen to be running. The intent is comparable to that of the
		/// "<code>%c</code>" format supported by the <code>strftime()</code>
		/// function of ISO&nbsp;C.
		/// </summary>
		/// <returns>  a string representation of this date, using the locale
		///          conventions. </returns>
		/// <seealso cref=     java.text.DateFormat </seealso>
		/// <seealso cref=     java.util.Date#toString() </seealso>
		/// <seealso cref=     java.util.Date#toGMTString() </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>DateFormat.format(Date date)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual String ToLocaleString()
		{
			DateFormat formatter = DateFormat.DateTimeInstance;
			return formatter.Format(this);
		}

		/// <summary>
		/// Creates a string representation of this <tt>Date</tt> object of
		/// the form:
		/// <blockquote><pre>
		/// d mon yyyy hh:mm:ss GMT</pre></blockquote>
		/// where:<ul>
		/// <li><i>d</i> is the day of the month (<tt>1</tt> through <tt>31</tt>),
		///     as one or two decimal digits.
		/// <li><i>mon</i> is the month (<tt>Jan, Feb, Mar, Apr, May, Jun, Jul,
		///     Aug, Sep, Oct, Nov, Dec</tt>).
		/// <li><i>yyyy</i> is the year, as four decimal digits.
		/// <li><i>hh</i> is the hour of the day (<tt>00</tt> through <tt>23</tt>),
		///     as two decimal digits.
		/// <li><i>mm</i> is the minute within the hour (<tt>00</tt> through
		///     <tt>59</tt>), as two decimal digits.
		/// <li><i>ss</i> is the second within the minute (<tt>00</tt> through
		///     <tt>61</tt>), as two decimal digits.
		/// <li><i>GMT</i> is exactly the ASCII letters "<tt>GMT</tt>" to indicate
		///     Greenwich Mean Time.
		/// </ul><para>
		/// The result does not depend on the local time zone.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a string representation of this date, using the Internet GMT
		///          conventions. </returns>
		/// <seealso cref=     java.text.DateFormat </seealso>
		/// <seealso cref=     java.util.Date#toString() </seealso>
		/// <seealso cref=     java.util.Date#toLocaleString() </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>DateFormat.format(Date date)</code>, using a
		/// GMT <code>TimeZone</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual String ToGMTString()
		{
			// d MMM yyyy HH:mm:ss 'GMT'
			long t = Time;
			BaseCalendar cal = GetCalendarSystem(t);
			BaseCalendar.Date date = (BaseCalendar.Date) cal.getCalendarDate(Time, (TimeZone)null);
			StringBuilder sb = new StringBuilder(32);
			CalendarUtils.sprintf0d(sb, date.DayOfMonth, 1).append(' '); // d
			ConvertToAbbr(sb, Wtb[date.Month - 1 + 2 + 7]).Append(' '); // MMM
			sb.Append(date.Year).Append(' '); // yyyy
			CalendarUtils.sprintf0d(sb, date.Hours, 2).append(':'); // HH
			CalendarUtils.sprintf0d(sb, date.Minutes, 2).append(':'); // mm
			CalendarUtils.sprintf0d(sb, date.Seconds, 2); // ss
			sb.Append(" GMT"); // ' GMT'
			return sb.ToString();
		}

		/// <summary>
		/// Returns the offset, measured in minutes, for the local time zone
		/// relative to UTC that is appropriate for the time represented by
		/// this <code>Date</code> object.
		/// <para>
		/// For example, in Massachusetts, five time zones west of Greenwich:
		/// <blockquote><pre>
		/// new Date(96, 1, 14).getTimezoneOffset() returns 300</pre></blockquote>
		/// because on February 14, 1996, standard time (Eastern Standard Time)
		/// is in use, which is offset five hours from UTC; but:
		/// <blockquote><pre>
		/// new Date(96, 5, 1).getTimezoneOffset() returns 240</pre></blockquote>
		/// because on June 1, 1996, daylight saving time (Eastern Daylight Time)
		/// </para>
		/// is in use, which is offset only four hours from UTC.<para>
		/// This method produces the same result as if it computed:
		/// <blockquote><pre>
		/// (this.getTime() - UTC(this.getYear(),
		///                       this.getMonth(),
		///                       this.getDate(),
		///                       this.getHours(),
		///                       this.getMinutes(),
		///                       this.getSeconds())) / (60 * 1000)
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the time-zone offset, in minutes, for the current time zone. </returns>
		/// <seealso cref=     java.util.Calendar#ZONE_OFFSET </seealso>
		/// <seealso cref=     java.util.Calendar#DST_OFFSET </seealso>
		/// <seealso cref=     java.util.TimeZone#getDefault </seealso>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>-(Calendar.get(Calendar.ZONE_OFFSET) +
		/// Calendar.get(Calendar.DST_OFFSET)) / (60 * 1000)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int TimezoneOffset
		{
			get
			{
				int zoneOffset;
				if (Cdate == null)
				{
					TimeZone tz = TimeZone.DefaultRef;
					if (tz is ZoneInfo)
					{
						zoneOffset = ((ZoneInfo)tz).getOffsets(FastTime, null);
					}
					else
					{
						zoneOffset = tz.GetOffset(FastTime);
					}
				}
				else
				{
					Normalize();
					zoneOffset = Cdate.ZoneOffset;
				}
				return -zoneOffset / 60000; // convert to minutes
			}
		}

		private BaseCalendar.Date CalendarDate
		{
			get
			{
				if (Cdate == null)
				{
					BaseCalendar cal = GetCalendarSystem(FastTime);
					Cdate = (BaseCalendar.Date) cal.getCalendarDate(FastTime, TimeZone.DefaultRef);
				}
				return Cdate;
			}
		}

		private BaseCalendar.Date Normalize()
		{
			if (Cdate == null)
			{
				BaseCalendar cal = GetCalendarSystem(FastTime);
				Cdate = (BaseCalendar.Date) cal.getCalendarDate(FastTime, TimeZone.DefaultRef);
				return Cdate;
			}

			// Normalize cdate with the TimeZone in cdate first. This is
			// required for the compatible behavior.
			if (!Cdate.Normalized)
			{
				Cdate = Normalize(Cdate);
			}

			// If the default TimeZone has changed, then recalculate the
			// fields with the new TimeZone.
			TimeZone tz = TimeZone.DefaultRef;
			if (tz != Cdate.Zone)
			{
				Cdate.Zone = tz;
				CalendarSystem cal = GetCalendarSystem(Cdate);
				cal.getCalendarDate(FastTime, Cdate);
			}
			return Cdate;
		}

		// fastTime and the returned data are in sync upon return.
		private BaseCalendar.Date Normalize(BaseCalendar.Date date)
		{
			int y = date.NormalizedYear;
			int m = date.Month;
			int d = date.DayOfMonth;
			int hh = date.Hours;
			int mm = date.Minutes;
			int ss = date.Seconds;
			int ms = date.Millis;
			TimeZone tz = date.Zone;

			// If the specified year can't be handled using a long value
			// in milliseconds, GregorianCalendar is used for full
			// compatibility with underflow and overflow. This is required
			// by some JCK tests. The limits are based max year values -
			// years that can be represented by max values of d, hh, mm,
			// ss and ms. Also, let GregorianCalendar handle the default
			// cutover year so that we don't need to worry about the
			// transition here.
			if (y == 1582 || y > 280000000 || y < -280000000)
			{
				if (tz == null)
				{
					tz = TimeZone.GetTimeZone("GMT");
				}
				GregorianCalendar gc = new GregorianCalendar(tz);
				gc.Clear();
				gc.Set(GregorianCalendar.MILLISECOND, ms);
				gc.Set(y, m - 1, d, hh, mm, ss);
				FastTime = gc.TimeInMillis;
				BaseCalendar cal = GetCalendarSystem(FastTime);
				date = (BaseCalendar.Date) cal.getCalendarDate(FastTime, tz);
				return date;
			}

			BaseCalendar cal = GetCalendarSystem(y);
			if (cal != GetCalendarSystem(date))
			{
				date = (BaseCalendar.Date) cal.newCalendarDate(tz);
				date.setNormalizedDate(y, m, d).setTimeOfDay(hh, mm, ss, ms);
			}
			// Perform the GregorianCalendar-style normalization.
			FastTime = cal.getTime(date);

			// In case the normalized date requires the other calendar
			// system, we need to recalculate it using the other one.
			BaseCalendar ncal = GetCalendarSystem(FastTime);
			if (ncal != cal)
			{
				date = (BaseCalendar.Date) ncal.newCalendarDate(tz);
				date.setNormalizedDate(y, m, d).setTimeOfDay(hh, mm, ss, ms);
				FastTime = ncal.getTime(date);
			}
			return date;
		}

		/// <summary>
		/// Returns the Gregorian or Julian calendar system to use with the
		/// given date. Use Gregorian from October 15, 1582.
		/// </summary>
		/// <param name="year"> normalized calendar year (not -1900) </param>
		/// <returns> the CalendarSystem to use for the specified date </returns>
		private static BaseCalendar GetCalendarSystem(int year)
		{
			if (year >= 1582)
			{
				return Gcal;
			}
			return JulianCalendar;
		}

		private static BaseCalendar GetCalendarSystem(long utc)
		{
			// Quickly check if the time stamp given by `utc' is the Epoch
			// or later. If it's before 1970, we convert the cutover to
			// local time to compare.
			if (utc >= 0 || utc >= GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER - TimeZone.DefaultRef.GetOffset(utc))
			{
				return Gcal;
			}
			return JulianCalendar;
		}

		private static BaseCalendar GetCalendarSystem(BaseCalendar.Date cdate)
		{
			if (Jcal == null)
			{
				return Gcal;
			}
			if (cdate.Era != null)
			{
				return Jcal;
			}
			return Gcal;
		}

		private static BaseCalendar JulianCalendar
		{
			get
			{
				lock (typeof(Date))
				{
					if (Jcal == null)
					{
						Jcal = (BaseCalendar) CalendarSystem.forName("julian");
					}
					return Jcal;
				}
			}
		}

		/// <summary>
		/// Save the state of this object to a stream (i.e., serialize it).
		/// 
		/// @serialData The value returned by <code>getTime()</code>
		///             is emitted (long).  This represents the offset from
		///             January 1, 1970, 00:00:00 GMT in milliseconds.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.WriteLong(TimeImpl);
		}

		/// <summary>
		/// Reconstitute this object from a stream (i.e., deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{
			FastTime = s.ReadLong();
		}

		/// <summary>
		/// Obtains an instance of {@code Date} from an {@code Instant} object.
		/// <para>
		/// {@code Instant} uses a precision of nanoseconds, whereas {@code Date}
		/// uses a precision of milliseconds.  The conversion will trancate any
		/// excess precision information as though the amount in nanoseconds was
		/// subject to integer division by one million.
		/// </para>
		/// <para>
		/// {@code Instant} can store points on the time-line further in the future
		/// and further in the past than {@code Date}. In this scenario, this method
		/// will throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to convert </param>
		/// <returns> a {@code Date} representing the same point on the time-line as
		///  the provided instant </returns>
		/// <exception cref="NullPointerException"> if {@code instant} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the instant is too large to
		///  represent as a {@code Date}
		/// @since 1.8 </exception>
		public static Date From(Instant instant)
		{
			try
			{
				return new Date(instant.ToEpochMilli());
			}
			catch (ArithmeticException ex)
			{
				throw new IllegalArgumentException(ex);
			}
		}

		/// <summary>
		/// Converts this {@code Date} object to an {@code Instant}.
		/// <para>
		/// The conversion creates an {@code Instant} that represents the same
		/// point on the time-line as this {@code Date}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an instant representing the same point on the time-line as
		///  this {@code Date} object
		/// @since 1.8 </returns>
		public virtual Instant ToInstant()
		{
			return Instant.OfEpochMilli(Time);
		}
	}

}