using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using CalendarDataUtility = sun.util.locale.provider.CalendarDataUtility;
	using BaseCalendar = sun.util.calendar.BaseCalendar;
	using CalendarDate = sun.util.calendar.CalendarDate;
	using CalendarSystem = sun.util.calendar.CalendarSystem;
	using CalendarUtils = sun.util.calendar.CalendarUtils;
	using Era = sun.util.calendar.Era;
	using Gregorian = sun.util.calendar.Gregorian;
	using LocalGregorianCalendar = sun.util.calendar.LocalGregorianCalendar;
	using ZoneInfo = sun.util.calendar.ZoneInfo;

	/// <summary>
	/// <code>JapaneseImperialCalendar</code> implements a Japanese
	/// calendar system in which the imperial era-based year numbering is
	/// supported from the Meiji era. The following are the eras supported
	/// by this calendar system.
	/// <pre><tt>
	/// ERA value   Era name    Since (in Gregorian)
	/// ------------------------------------------------------
	///     0       N/A         N/A
	///     1       Meiji       1868-01-01 midnight local time
	///     2       Taisho      1912-07-30 midnight local time
	///     3       Showa       1926-12-25 midnight local time
	///     4       Heisei      1989-01-08 midnight local time
	/// ------------------------------------------------------
	/// </tt></pre>
	/// 
	/// <para><code>ERA</code> value 0 specifies the years before Meiji and
	/// the Gregorian year values are used. Unlike {@link
	/// GregorianCalendar}, the Julian to Gregorian transition is not
	/// supported because it doesn't make any sense to the Japanese
	/// calendar systems used before Meiji. To represent the years before
	/// Gregorian year 1, 0 and negative values are used. The Japanese
	/// Imperial rescripts and government decrees don't specify how to deal
	/// with time differences for applying the era transitions. This
	/// calendar implementation assumes local time for all transitions.
	/// 
	/// @author Masayoshi Okutsu
	/// @since 1.6
	/// </para>
	/// </summary>
	internal class JapaneseImperialCalendar : Calendar
	{
		/*
		 * Implementation Notes
		 *
		 * This implementation uses
		 * sun.util.calendar.LocalGregorianCalendar to perform most of the
		 * calendar calculations. LocalGregorianCalendar is configurable
		 * and reads <JRE_HOME>/lib/calendars.properties at the start-up.
		 */

		/// <summary>
		/// The ERA constant designating the era before Meiji.
		/// </summary>
		public const int BEFORE_MEIJI = 0;

		/// <summary>
		/// The ERA constant designating the Meiji era.
		/// </summary>
		public const int MEIJI = 1;

		/// <summary>
		/// The ERA constant designating the Taisho era.
		/// </summary>
		public const int TAISHO = 2;

		/// <summary>
		/// The ERA constant designating the Showa era.
		/// </summary>
		public const int SHOWA = 3;

		/// <summary>
		/// The ERA constant designating the Heisei era.
		/// </summary>
		public const int HEISEI = 4;

		private const int EPOCH_OFFSET = 719163; // Fixed date of January 1, 1970 (Gregorian)
		private const int EPOCH_YEAR = 1970;

		// Useful millisecond constants.  Although ONE_DAY and ONE_WEEK can fit
		// into ints, they must be longs in order to prevent arithmetic overflow
		// when performing (bug 4173516).
		private const int ONE_SECOND = 1000;
		private static readonly int ONE_MINUTE = 60 * ONE_SECOND;
		private static readonly int ONE_HOUR = 60 * ONE_MINUTE;
		private static readonly long ONE_DAY = 24 * ONE_HOUR;
		private static readonly long ONE_WEEK = 7 * ONE_DAY;

		// Reference to the sun.util.calendar.LocalGregorianCalendar instance (singleton).
		private static readonly LocalGregorianCalendar Jcal = (LocalGregorianCalendar) CalendarSystem.forName("japanese");

		// Gregorian calendar instance. This is required because era
		// transition dates are given in Gregorian dates.
		private static readonly Gregorian Gcal = CalendarSystem.GregorianCalendar;

		// The Era instance representing "before Meiji".
		private static readonly Era BEFORE_MEIJI_ERA = new Era("BeforeMeiji", "BM", Long.MinValue, false);

		// Imperial eras. The sun.util.calendar.LocalGregorianCalendar
		// doesn't have an Era representing before Meiji, which is
		// inconvenient for a Calendar. So, era[0] is a reference to
		// BEFORE_MEIJI_ERA.
		private static readonly Era[] Eras;

		// Fixed date of the first date of each era.
		private static readonly long[] SinceFixedDates;

		/*
		 * <pre>
		 *                                 Greatest       Least
		 * Field name             Minimum   Minimum     Maximum     Maximum
		 * ----------             -------   -------     -------     -------
		 * ERA                          0         0           1           1
		 * YEAR                -292275055         1           ?           ?
		 * MONTH                        0         0          11          11
		 * WEEK_OF_YEAR                 1         1          52*         53
		 * WEEK_OF_MONTH                0         0           4*          6
		 * DAY_OF_MONTH                 1         1          28*         31
		 * DAY_OF_YEAR                  1         1         365*        366
		 * DAY_OF_WEEK                  1         1           7           7
		 * DAY_OF_WEEK_IN_MONTH        -1        -1           4*          6
		 * AM_PM                        0         0           1           1
		 * HOUR                         0         0          11          11
		 * HOUR_OF_DAY                  0         0          23          23
		 * MINUTE                       0         0          59          59
		 * SECOND                       0         0          59          59
		 * MILLISECOND                  0         0         999         999
		 * ZONE_OFFSET             -13:00    -13:00       14:00       14:00
		 * DST_OFFSET                0:00      0:00        0:20        2:00
		 * </pre>
		 * *: depends on eras
		 */
		internal static readonly int[] MIN_VALUES = new int[] {0, -292275055, JANUARY, 1, 0, 1, 1, SUNDAY, 1, AM, 0, 0, 0, 0, 0, -13 * ONE_HOUR, 0};
		internal static readonly int[] LEAST_MAX_VALUES = new int[] {0, 0, JANUARY, 0, 4, 28, 0, SATURDAY, 4, PM, 11, 23, 59, 59, 999, 14 * ONE_HOUR, 20 * ONE_MINUTE};
		internal static readonly int[] MAX_VALUES = new int[] {0, 292278994, DECEMBER, 53, 6, 31, 366, SATURDAY, 6, PM, 11, 23, 59, 59, 999, 14 * ONE_HOUR, 2 * ONE_HOUR};

		// Proclaim serialization compatibility with JDK 1.6
		private new const long SerialVersionUID = -3364572813905467929L;

		static JapaneseImperialCalendar()
		{
			Era[] es = Jcal.Eras;
			int length = es.Length + 1;
			Eras = new Era[length];
			SinceFixedDates = new long[length];

			// eras[BEFORE_MEIJI] and sinceFixedDate[BEFORE_MEIJI] are the
			// same as Gregorian.
			int index = BEFORE_MEIJI;
			SinceFixedDates[index] = Gcal.getFixedDate(BEFORE_MEIJI_ERA.SinceDate);
			Eras[index++] = BEFORE_MEIJI_ERA;
			foreach (Era e in es)
			{
				CalendarDate d = e.SinceDate;
				SinceFixedDates[index] = Gcal.getFixedDate(d);
				Eras[index++] = e;
			}

			LEAST_MAX_VALUES[ERA] = MAX_VALUES[ERA] = Eras.Length - 1;

			// Calculate the least maximum year and least day of Year
			// values. The following code assumes that there's at most one
			// era transition in a Gregorian year.
			int year = Integer.MaxValue;
			int dayOfYear = Integer.MaxValue;
			CalendarDate date = Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
			for (int i = 1; i < Eras.Length; i++)
			{
				long fd = SinceFixedDates[i];
				CalendarDate transitionDate = Eras[i].SinceDate;
				date.setDate(transitionDate.Year, BaseCalendar.JANUARY, 1);
				long fdd = Gcal.getFixedDate(date);
				if (fd != fdd)
				{
					dayOfYear = System.Math.Min((int)(fd - fdd) + 1, dayOfYear);
				}
				date.setDate(transitionDate.Year, BaseCalendar.DECEMBER, 31);
				fdd = Gcal.getFixedDate(date);
				if (fd != fdd)
				{
					dayOfYear = System.Math.Min((int)(fdd - fd) + 1, dayOfYear);
				}
				LocalGregorianCalendar.Date lgd = GetCalendarDate(fd - 1);
				int y = lgd.Year;
				// Unless the first year starts from January 1, the actual
				// max value could be one year short. For example, if it's
				// Showa 63 January 8, 63 is the actual max value since
				// Showa 64 January 8 doesn't exist.
				if (!(lgd.Month == BaseCalendar.JANUARY && lgd.DayOfMonth == 1))
				{
					y--;
				}
				year = System.Math.Min(y, year);
			}
			LEAST_MAX_VALUES[YEAR] = year; // Max year could be smaller than this value.
			LEAST_MAX_VALUES[DAY_OF_YEAR] = dayOfYear;
		}

		/// <summary>
		/// jdate always has a sun.util.calendar.LocalGregorianCalendar.Date instance to
		/// avoid overhead of creating it for each calculation.
		/// </summary>
		[NonSerialized]
		private LocalGregorianCalendar.Date Jdate;

		/// <summary>
		/// Temporary int[2] to get time zone offsets. zoneOffsets[0] gets
		/// the GMT offset value and zoneOffsets[1] gets the daylight saving
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

		/// <summary>
		/// Constructs a <code>JapaneseImperialCalendar</code> based on the current time
		/// in the given time zone with the given locale.
		/// </summary>
		/// <param name="zone"> the given time zone. </param>
		/// <param name="aLocale"> the given locale. </param>
		internal JapaneseImperialCalendar(TimeZone zone, Locale aLocale) : base(zone, aLocale)
		{
			Jdate = Jcal.newCalendarDate(zone);
			TimeInMillis = DateTimeHelperClass.CurrentUnixTimeMillis();
		}

		/// <summary>
		/// Constructs an "empty" {@code JapaneseImperialCalendar}.
		/// </summary>
		/// <param name="zone">    the given time zone </param>
		/// <param name="aLocale"> the given locale </param>
		/// <param name="flag">    the flag requesting an empty instance </param>
		internal JapaneseImperialCalendar(TimeZone zone, Locale aLocale, bool flag) : base(zone, aLocale)
		{
			Jdate = Jcal.newCalendarDate(zone);
		}

		/// <summary>
		/// Returns {@code "japanese"} as the calendar type of this {@code
		/// JapaneseImperialCalendar}.
		/// </summary>
		/// <returns> {@code "japanese"} </returns>
		public override String CalendarType
		{
			get
			{
				return "japanese";
			}
		}

		/// <summary>
		/// Compares this <code>JapaneseImperialCalendar</code> to the specified
		/// <code>Object</code>. The result is <code>true</code> if and
		/// only if the argument is a <code>JapaneseImperialCalendar</code> object
		/// that represents the same time value (millisecond offset from
		/// the <a href="Calendar.html#Epoch">Epoch</a>) under the same
		/// <code>Calendar</code> parameters.
		/// </summary>
		/// <param name="obj"> the object to compare with. </param>
		/// <returns> <code>true</code> if this object is equal to <code>obj</code>;
		/// <code>false</code> otherwise. </returns>
		/// <seealso cref= Calendar#compareTo(Calendar) </seealso>
		public override bool Equals(Object obj)
		{
			return obj is JapaneseImperialCalendar && base.Equals(obj);
		}

		/// <summary>
		/// Generates the hash code for this
		/// <code>JapaneseImperialCalendar</code> object.
		/// </summary>
		public override int HashCode()
		{
			return base.HashCode() ^ Jdate.HashCode();
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
				LocalGregorianCalendar.Date d = (LocalGregorianCalendar.Date) Jdate.clone();
				d.addYear(amount);
				PinDayOfMonth(d);
				Set(ERA, GetEraIndex(d));
				Set(YEAR, d.Year);
				Set(MONTH, d.Month - 1);
				Set(DAY_OF_MONTH, d.DayOfMonth);
			}
			else if (field == MONTH)
			{
				LocalGregorianCalendar.Date d = (LocalGregorianCalendar.Date) Jdate.clone();
				d.addMonth(amount);
				PinDayOfMonth(d);
				Set(ERA, GetEraIndex(d));
				Set(YEAR, d.Year);
				Set(MONTH, d.Month - 1);
				Set(DAY_OF_MONTH, d.DayOfMonth);
			}
			else if (field == ERA)
			{
				int era = InternalGet(ERA) + amount;
				if (era < 0)
				{
					era = 0;
				}
				else if (era > Eras.Length - 1)
				{
					era = Eras.Length - 1;
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
					delta *= 60 * 60 * 1000; // hours to milliseconds
					break;

				case MINUTE:
					delta *= 60 * 1000; // minutes to milliseconds
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
				long fd = CachedFixedDate;
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
					long fd2 = CachedFixedDate;
					// If the adjustment has changed the date, then take
					// the previous one.
					if (fd2 != fd)
					{
						TimeInMillis = Time_Renamed - zoneOffset;
					}
				}
			}
		}

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
		/// </summary>
		/// <param name="field"> the calendar field. </param>
		/// <param name="amount"> the signed amount to add to <code>field</code>. </param>
		/// <exception cref="IllegalArgumentException"> if <code>field</code> is
		/// <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		/// or if any calendar fields have out-of-range values in
		/// non-lenient mode. </exception>
		/// <seealso cref= #roll(int,boolean) </seealso>
		/// <seealso cref= #add(int,int) </seealso>
		/// <seealso cref= #set(int,int) </seealso>
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
			case ERA:
			case AM_PM:
			case MINUTE:
			case SECOND:
			case MILLISECOND:
				// These fields are handled simply, since they have fixed
				// minima and maxima. Other fields are complicated, since
				// the range within they must roll varies depending on the
				// date, a time zone and the era transitions.
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
					CalendarDate d = Jcal.getCalendarDate(Time_Renamed, Zone);
					if (InternalGet(DAY_OF_MONTH) != d.DayOfMonth)
					{
						d.Era = Jdate.Era;
						d.setDate(InternalGet(YEAR), InternalGet(MONTH) + 1, InternalGet(DAY_OF_MONTH));
						if (field == HOUR)
						{
							assert(InternalGet(AM_PM) == PM);
							d.addHours(+12); // restore PM
						}
						Time_Renamed = Jcal.getTime(d);
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

			case YEAR:
				min = GetActualMinimum(field);
				max = GetActualMaximum(field);
				break;

			case MONTH:
				// Rolling the month involves both pinning the final value to [0, 11]
				// and adjusting the DAY_OF_MONTH if necessary.  We only adjust the
				// DAY_OF_MONTH if, after updating the MONTH field, it is illegal.
				// E.g., <jan31>.roll(MONTH, 1) -> <feb28> or <feb29>.
			{
					if (!IsTransitionYear(Jdate.NormalizedYear))
					{
						int year = Jdate.Year;
						if (year == GetMaximum(YEAR))
						{
							CalendarDate jd = Jcal.getCalendarDate(Time_Renamed, Zone);
							CalendarDate d = Jcal.getCalendarDate(Long.MaxValue, Zone);
							max = d.Month - 1;
							int n = GetRolledValue(InternalGet(field), amount, min, max);
							if (n == max)
							{
								// To avoid overflow, use an equivalent year.
								jd.addYear(-400);
								jd.Month = n + 1;
								if (jd.DayOfMonth > d.DayOfMonth)
								{
									jd.DayOfMonth = d.DayOfMonth;
									Jcal.normalize(jd);
								}
								if (jd.DayOfMonth == d.DayOfMonth && jd.TimeOfDay > d.TimeOfDay)
								{
									jd.Month = n + 1;
									jd.DayOfMonth = d.DayOfMonth - 1;
									Jcal.normalize(jd);
									// Month may have changed by the normalization.
									n = jd.Month - 1;
								}
								Set(DAY_OF_MONTH, jd.DayOfMonth);
							}
							Set(MONTH, n);
						}
						else if (year == GetMinimum(YEAR))
						{
							CalendarDate jd = Jcal.getCalendarDate(Time_Renamed, Zone);
							CalendarDate d = Jcal.getCalendarDate(Long.MinValue, Zone);
							min = d.Month - 1;
							int n = GetRolledValue(InternalGet(field), amount, min, max);
							if (n == min)
							{
								// To avoid underflow, use an equivalent year.
								jd.addYear(+400);
								jd.Month = n + 1;
								if (jd.DayOfMonth < d.DayOfMonth)
								{
									jd.DayOfMonth = d.DayOfMonth;
									Jcal.normalize(jd);
								}
								if (jd.DayOfMonth == d.DayOfMonth && jd.TimeOfDay < d.TimeOfDay)
								{
									jd.Month = n + 1;
									jd.DayOfMonth = d.DayOfMonth + 1;
									Jcal.normalize(jd);
									// Month may have changed by the normalization.
									n = jd.Month - 1;
								}
								Set(DAY_OF_MONTH, jd.DayOfMonth);
							}
							Set(MONTH, n);
						}
						else
						{
							int mon = (InternalGet(MONTH) + amount) % 12;
							if (mon < 0)
							{
								mon += 12;
							}
							Set(MONTH, mon);

							// Keep the day of month in the range.  We
							// don't want to spill over into the next
							// month; e.g., we don't want jan31 + 1 mo ->
							// feb31 -> mar3.
							int monthLen = MonthLength(mon);
							if (InternalGet(DAY_OF_MONTH) > monthLen)
							{
								Set(DAY_OF_MONTH, monthLen);
							}
						}
					}
					else
					{
						int eraIndex = GetEraIndex(Jdate);
						CalendarDate transition = null;
						if (Jdate.Year == 1)
						{
							transition = Eras[eraIndex].SinceDate;
							min = transition.Month - 1;
						}
						else
						{
							if (eraIndex < Eras.Length - 1)
							{
								transition = Eras[eraIndex + 1].SinceDate;
								if (transition.Year == Jdate.NormalizedYear)
								{
									max = transition.Month - 1;
									if (transition.DayOfMonth == 1)
									{
										max--;
									}
								}
							}
						}

						if (min == max)
						{
							// The year has only one month. No need to
							// process further. (Showa Gan-nen (year 1)
							// and the last year have only one month.)
							return;
						}
						int n = GetRolledValue(InternalGet(field), amount, min, max);
						Set(MONTH, n);
						if (n == min)
						{
							if (!(transition.Month == BaseCalendar.JANUARY && transition.DayOfMonth == 1))
							{
								if (Jdate.DayOfMonth < transition.DayOfMonth)
								{
									Set(DAY_OF_MONTH, transition.DayOfMonth);
								}
							}
						}
						else if (n == max && (transition.Month - 1 == n))
						{
							int dom = transition.DayOfMonth;
							if (Jdate.DayOfMonth >= dom)
							{
								Set(DAY_OF_MONTH, dom - 1);
							}
						}
					}
					return;
			}

			case WEEK_OF_YEAR:
			{
					int y = Jdate.NormalizedYear;
					max = GetActualMaximum(WEEK_OF_YEAR);
					Set(DAY_OF_WEEK, InternalGet(DAY_OF_WEEK)); // update stamp[field]
					int woy = InternalGet(WEEK_OF_YEAR);
					int value = woy + amount;
					if (!IsTransitionYear(Jdate.NormalizedYear))
					{
						int year = Jdate.Year;
						if (year == GetMaximum(YEAR))
						{
							max = GetActualMaximum(WEEK_OF_YEAR);
						}
						else if (year == GetMinimum(YEAR))
						{
							min = GetActualMinimum(WEEK_OF_YEAR);
							max = GetActualMaximum(WEEK_OF_YEAR);
							if (value > min && value < max)
							{
								Set(WEEK_OF_YEAR, value);
								return;
							}

						}
						// If the new value is in between min and max
						// (exclusive), then we can use the value.
						if (value > min && value < max)
						{
							Set(WEEK_OF_YEAR, value);
							return;
						}
						long fd = CachedFixedDate;
						// Make sure that the min week has the current DAY_OF_WEEK
						long day1 = fd - (7 * (woy - min));
						if (year != GetMinimum(YEAR))
						{
							if (Gcal.getYearFromFixedDate(day1) != y)
							{
								min++;
							}
						}
						else
						{
							CalendarDate d = Jcal.getCalendarDate(Long.MinValue, Zone);
							if (day1 < Jcal.getFixedDate(d))
							{
								min++;
							}
						}

						// Make sure the same thing for the max week
						fd += 7 * (max - InternalGet(WEEK_OF_YEAR));
						if (Gcal.getYearFromFixedDate(fd) != y)
						{
							max--;
						}
						break;
					}

					// Handle transition here.
					long fd = CachedFixedDate;
					long day1 = fd - (7 * (woy - min));
					// Make sure that the min week has the current DAY_OF_WEEK
					LocalGregorianCalendar.Date d = GetCalendarDate(day1);
					if (!(d.Era == Jdate.Era && d.Year == Jdate.Year))
					{
						min++;
					}

					// Make sure the same thing for the max week
					fd += 7 * (max - woy);
					Jcal.getCalendarDateFromFixedDate(d, fd);
					if (!(d.Era == Jdate.Era && d.Year == Jdate.Year))
					{
						max--;
					}
					// value: the new WEEK_OF_YEAR which must be converted
					// to month and day of month.
					value = GetRolledValue(woy, amount, min, max) - 1;
					d = GetCalendarDate(day1 + value * 7);
					Set(MONTH, d.Month - 1);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case WEEK_OF_MONTH:
			{
					bool isTransitionYear = IsTransitionYear(Jdate.NormalizedYear);
					// dow: relative day of week from the first day of week
					int dow = InternalGet(DAY_OF_WEEK) - FirstDayOfWeek;
					if (dow < 0)
					{
						dow += 7;
					}

					long fd = CachedFixedDate;
					long month1; // fixed date of the first day (usually 1) of the month
					int monthLength; // actual month length
					if (isTransitionYear)
					{
						month1 = GetFixedDateMonth1(Jdate, fd);
						monthLength = ActualMonthLength();
					}
					else
					{
						month1 = fd - InternalGet(DAY_OF_MONTH) + 1;
						monthLength = Jcal.getMonthLength(Jdate);
					}

					// the first day of week of the month.
					long monthDay1st = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(month1 + 6, FirstDayOfWeek);
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
					Set(DAY_OF_MONTH, (int)(nfd - month1) + 1);
					return;
			}

			case DAY_OF_MONTH:
			{
					if (!IsTransitionYear(Jdate.NormalizedYear))
					{
						max = Jcal.getMonthLength(Jdate);
						break;
					}

					// TODO: Need to change the spec to be usable DAY_OF_MONTH rolling...

					// Transition handling. We can't change year and era
					// values here due to the Calendar roll spec!
					long month1 = GetFixedDateMonth1(Jdate, CachedFixedDate);

					// It may not be a regular month. Convert the date and range to
					// the relative values, perform the roll, and
					// convert the result back to the rolled date.
					int value = GetRolledValue((int)(CachedFixedDate - month1), amount, 0, ActualMonthLength() - 1);
					LocalGregorianCalendar.Date d = GetCalendarDate(month1 + value);
					Debug.Assert(GetEraIndex(d) == InternalGetEra() && d.Year == InternalGet(YEAR) && d.Month - 1 == InternalGet(MONTH));
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case DAY_OF_YEAR:
			{
					max = GetActualMaximum(field);
					if (!IsTransitionYear(Jdate.NormalizedYear))
					{
						break;
					}

					// Handle transition. We can't change year and era values
					// here due to the Calendar roll spec.
					int value = GetRolledValue(InternalGet(DAY_OF_YEAR), amount, min, max);
					long jan0 = CachedFixedDate - InternalGet(DAY_OF_YEAR);
					LocalGregorianCalendar.Date d = GetCalendarDate(jan0 + value);
					Debug.Assert(GetEraIndex(d) == InternalGetEra() && d.Year == InternalGet(YEAR));
					Set(MONTH, d.Month - 1);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}

			case DAY_OF_WEEK:
			{
					int normalizedYear = Jdate.NormalizedYear;
					if (!IsTransitionYear(normalizedYear) && !IsTransitionYear(normalizedYear - 1))
					{
						// If the week of year is in the same year, we can
						// just change DAY_OF_WEEK.
						int weekOfYear = InternalGet(WEEK_OF_YEAR);
						if (weekOfYear > 1 && weekOfYear < 52)
						{
							Set(WEEK_OF_YEAR, InternalGet(WEEK_OF_YEAR));
							max = SATURDAY;
							break;
						}
					}

					// We need to handle it in a different way around year
					// boundaries and in the transition year. Note that
					// changing era and year values violates the roll
					// rule: not changing larger calendar fields...
					amount %= 7;
					if (amount == 0)
					{
						return;
					}
					long fd = CachedFixedDate;
					long dowFirst = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fd, FirstDayOfWeek);
					fd += amount;
					if (fd < dowFirst)
					{
						fd += 7;
					}
					else if (fd >= dowFirst + 7)
					{
						fd -= 7;
					}
					LocalGregorianCalendar.Date d = GetCalendarDate(fd);
					Set(ERA, GetEraIndex(d));
					Set(d.Year, d.Month - 1, d.DayOfMonth);
					return;
			}

			case DAY_OF_WEEK_IN_MONTH:
			{
					min = 1; // after having normalized, min should be 1.
					if (!IsTransitionYear(Jdate.NormalizedYear))
					{
						int dom = InternalGet(DAY_OF_MONTH);
						int monthLength = Jcal.getMonthLength(Jdate);
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

					// Transition year handling.
					long fd = CachedFixedDate;
					long month1 = GetFixedDateMonth1(Jdate, fd);
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
					LocalGregorianCalendar.Date d = GetCalendarDate(fd);
					Set(DAY_OF_MONTH, d.DayOfMonth);
					return;
			}
			}

			Set(field, GetRolledValue(InternalGet(field), amount, min, max));
		}

		public override String GetDisplayName(int field, int style, Locale locale)
		{
			if (!CheckDisplayNameParams(field, style, SHORT, NARROW_FORMAT, locale, ERA_MASK | YEAR_MASK | MONTH_MASK | DAY_OF_WEEK_MASK | AM_PM_MASK))
			{
				return null;
			}

			int fieldValue = Get(field);

			// "GanNen" is supported only in the LONG style.
			if (field == YEAR && (GetBaseStyle(style) != LONG || fieldValue != 1 || Get(ERA) == 0))
			{
				return null;
			}

			String name = CalendarDataUtility.retrieveFieldValueName(CalendarType, field, fieldValue, style, locale);
			// If the ERA value is null, then
			// try to get its name or abbreviation from the Era instance.
			if (name == null && field == ERA && fieldValue < Eras.Length)
			{
				Era era = Eras[fieldValue];
				name = (style == SHORT) ? era.Abbreviation : era.Name;
			}
			return name;
		}

		public override Map<String, Integer> GetDisplayNames(int field, int style, Locale locale)
		{
			if (!CheckDisplayNameParams(field, style, ALL_STYLES, NARROW_FORMAT, locale, ERA_MASK | YEAR_MASK | MONTH_MASK | DAY_OF_WEEK_MASK | AM_PM_MASK))
			{
				return null;
			}
			Map<String, Integer> names;
			names = CalendarDataUtility.retrieveFieldValueNames(CalendarType, field, style, locale);
			// If strings[] has fewer than eras[], get more names from eras[].
			if (names != null)
			{
				if (field == ERA)
				{
					int size = names.Size();
					if (style == ALL_STYLES)
					{
						Set<Integer> values = new HashSet<Integer>();
						// count unique era values
						foreach (String key in names.KeySet())
						{
							values.Add(names.Get(key));
						}
						size = values.Count;
					}
					if (size < Eras.Length)
					{
						int baseStyle = GetBaseStyle(style);
						for (int i = size; i < Eras.Length; i++)
						{
							Era era = Eras[i];
							if (baseStyle == ALL_STYLES || baseStyle == SHORT || baseStyle == NARROW_FORMAT)
							{
								names.Put(era.Abbreviation, i);
							}
							if (baseStyle == ALL_STYLES || baseStyle == LONG)
							{
								names.Put(era.Name, i);
							}
						}
					}
				}
			}
			return names;
		}

		/// <summary>
		/// Returns the minimum value for the given calendar field of this
		/// <code>Calendar</code> instance. The minimum value is
		/// defined as the smallest value returned by the {@link
		/// Calendar#get(int) get} method for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
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
		/// and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
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
			case YEAR:
			{
					// The value should depend on the time zone of this calendar.
					LocalGregorianCalendar.Date d = Jcal.getCalendarDate(Long.MaxValue, Zone);
					return System.Math.Max(LEAST_MAX_VALUES[YEAR], d.Year);
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
		/// and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
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
			return field == YEAR ? 1 : MIN_VALUES[field];
		}

		/// <summary>
		/// Returns the lowest maximum value for the given calendar field
		/// of this <code>GregorianCalendar</code> instance. The lowest
		/// maximum value is defined as the smallest value returned by
		/// <seealso cref="#getActualMaximum(int)"/> for any possible time value,
		/// taking into consideration the current values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
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
			case YEAR:
			{
					return System.Math.Min(LEAST_MAX_VALUES[YEAR], GetMaximum(YEAR));
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
		/// and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the minimum of the given field for the time value of
		/// this <code>JapaneseImperialCalendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMaximum(int) </seealso>
		public override int GetActualMinimum(int field)
		{
			if (!IsFieldSet(YEAR_MASK | MONTH_MASK | WEEK_OF_YEAR_MASK, field))
			{
				return GetMinimum(field);
			}

			int value = 0;
			JapaneseImperialCalendar jc = NormalizedCalendar;
			// Get a local date which includes time of day and time zone,
			// which are missing in jc.jdate.
			LocalGregorianCalendar.Date jd = Jcal.getCalendarDate(jc.TimeInMillis, Zone);
			int eraIndex = GetEraIndex(jd);
			switch (field)
			{
			case YEAR:
			{
					if (eraIndex > BEFORE_MEIJI)
					{
						value = 1;
						long since = Eras[eraIndex].getSince(Zone);
						CalendarDate d = Jcal.getCalendarDate(since, Zone);
						// Use the same year in jd to take care of leap
						// years. i.e., both jd and d must agree on leap
						// or common years.
						jd.Year = d.Year;
						Jcal.normalize(jd);
						Debug.Assert(jd.LeapYear == d.LeapYear);
						if (GetYearOffsetInMillis(jd) < GetYearOffsetInMillis(d))
						{
							value++;
						}
					}
					else
					{
						value = GetMinimum(field);
						CalendarDate d = Jcal.getCalendarDate(Long.MinValue, Zone);
						// Use an equvalent year of d.getYear() if
						// possible. Otherwise, ignore the leap year and
						// common year difference.
						int y = d.Year;
						if (y > 400)
						{
							y -= 400;
						}
						jd.Year = y;
						Jcal.normalize(jd);
						if (GetYearOffsetInMillis(jd) < GetYearOffsetInMillis(d))
						{
							value++;
						}
					}
			}
				break;

			case MONTH:
			{
					// In Before Meiji and Meiji, January is the first month.
					if (eraIndex > MEIJI && jd.Year == 1)
					{
						long since = Eras[eraIndex].getSince(Zone);
						CalendarDate d = Jcal.getCalendarDate(since, Zone);
						value = d.Month - 1;
						if (jd.DayOfMonth < d.DayOfMonth)
						{
							value++;
						}
					}
			}
				break;

			case WEEK_OF_YEAR:
			{
					value = 1;
					CalendarDate d = Jcal.getCalendarDate(Long.MinValue, Zone);
					// shift 400 years to avoid underflow
					d.addYear(+400);
					Jcal.normalize(d);
					jd.Era = d.Era;
					jd.Year = d.Year;
					Jcal.normalize(jd);

					long jan1 = Jcal.getFixedDate(d);
					long fd = Jcal.getFixedDate(jd);
					int woy = GetWeekNumber(jan1, fd);
					long day1 = fd - (7 * (woy - 1));
					if ((day1 < jan1) || (day1 == jan1 && jd.TimeOfDay < d.TimeOfDay))
					{
						value++;
					}
			}
				break;
			}
			return value;
		}

		/// <summary>
		/// Returns the maximum value that this calendar field could have,
		/// taking into consideration the given time value and the current
		/// values of the
		/// <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		/// <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		/// and
		/// <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		/// For example, if the date of this instance is Heisei 16February 1,
		/// the actual maximum value of the <code>DAY_OF_MONTH</code> field
		/// is 29 because Heisei 16 is a leap year, and if the date of this
		/// instance is Heisei 17 February 1, it's 28.
		/// </summary>
		/// <param name="field"> the calendar field </param>
		/// <returns> the maximum of the given field for the time value of
		/// this <code>JapaneseImperialCalendar</code> </returns>
		/// <seealso cref= #getMinimum(int) </seealso>
		/// <seealso cref= #getMaximum(int) </seealso>
		/// <seealso cref= #getGreatestMinimum(int) </seealso>
		/// <seealso cref= #getLeastMaximum(int) </seealso>
		/// <seealso cref= #getActualMinimum(int) </seealso>
		public override int GetActualMaximum(int field)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fieldsForFixedMax = ERA_MASK|DAY_OF_WEEK_MASK|HOUR_MASK|AM_PM_MASK| HOUR_OF_DAY_MASK|MINUTE_MASK|SECOND_MASK|MILLISECOND_MASK| ZONE_OFFSET_MASK|DST_OFFSET_MASK;
			int fieldsForFixedMax = ERA_MASK | DAY_OF_WEEK_MASK | HOUR_MASK | AM_PM_MASK | HOUR_OF_DAY_MASK | MINUTE_MASK | SECOND_MASK | MILLISECOND_MASK | ZONE_OFFSET_MASK | DST_OFFSET_MASK;
			if ((fieldsForFixedMax & (1 << field)) != 0)
			{
				return GetMaximum(field);
			}

			JapaneseImperialCalendar jc = NormalizedCalendar;
			LocalGregorianCalendar.Date date = jc.Jdate;
			int normalizedYear = date.NormalizedYear;

			int value = -1;
			switch (field)
			{
			case MONTH:
			{
					value = DECEMBER;
					if (IsTransitionYear(date.NormalizedYear))
					{
						// TODO: there may be multiple transitions in a year.
						int eraIndex = GetEraIndex(date);
						if (date.Year != 1)
						{
							eraIndex++;
							Debug.Assert(eraIndex < Eras.Length);
						}
						long transition = SinceFixedDates[eraIndex];
						long fd = jc.CachedFixedDate;
						if (fd < transition)
						{
							LocalGregorianCalendar.Date ldate = (LocalGregorianCalendar.Date) date.clone();
							Jcal.getCalendarDateFromFixedDate(ldate, transition - 1);
							value = ldate.Month - 1;
						}
					}
					else
					{
						LocalGregorianCalendar.Date d = Jcal.getCalendarDate(Long.MaxValue, Zone);
						if (date.Era == d.Era && date.Year == d.Year)
						{
							value = d.Month - 1;
						}
					}
			}
				break;

			case DAY_OF_MONTH:
				value = Jcal.getMonthLength(date);
				break;

			case DAY_OF_YEAR:
			{
					if (IsTransitionYear(date.NormalizedYear))
					{
						// Handle transition year.
						// TODO: there may be multiple transitions in a year.
						int eraIndex = GetEraIndex(date);
						if (date.Year != 1)
						{
							eraIndex++;
							Debug.Assert(eraIndex < Eras.Length);
						}
						long transition = SinceFixedDates[eraIndex];
						long fd = jc.CachedFixedDate;
						CalendarDate d = Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
						d.setDate(date.NormalizedYear, BaseCalendar.JANUARY, 1);
						if (fd < transition)
						{
							value = (int)(transition - Gcal.getFixedDate(d));
						}
						else
						{
							d.addYear(+1);
							value = (int)(Gcal.getFixedDate(d) - transition);
						}
					}
					else
					{
						LocalGregorianCalendar.Date d = Jcal.getCalendarDate(Long.MaxValue, Zone);
						if (date.Era == d.Era && date.Year == d.Year)
						{
							long fd = Jcal.getFixedDate(d);
							long jan1 = GetFixedDateJan1(d, fd);
							value = (int)(fd - jan1) + 1;
						}
						else if (date.Year == GetMinimum(YEAR))
						{
							CalendarDate d1 = Jcal.getCalendarDate(Long.MinValue, Zone);
							long fd1 = Jcal.getFixedDate(d1);
							d1.addYear(1);
							d1.setMonth(BaseCalendar.JANUARY).setDayOfMonth(1);
							Jcal.normalize(d1);
							long fd2 = Jcal.getFixedDate(d1);
							value = (int)(fd2 - fd1);
						}
						else
						{
							value = Jcal.getYearLength(date);
						}
					}
			}
				break;

			case WEEK_OF_YEAR:
			{
					if (!IsTransitionYear(date.NormalizedYear))
					{
						LocalGregorianCalendar.Date jd = Jcal.getCalendarDate(Long.MaxValue, Zone);
						if (date.Era == jd.Era && date.Year == jd.Year)
						{
							long fd = Jcal.getFixedDate(jd);
							long jan1 = GetFixedDateJan1(jd, fd);
							value = GetWeekNumber(jan1, fd);
						}
						else if (date.Era == null && date.Year == GetMinimum(YEAR))
						{
							CalendarDate d = Jcal.getCalendarDate(Long.MinValue, Zone);
							// shift 400 years to avoid underflow
							d.addYear(+400);
							Jcal.normalize(d);
							jd.Era = d.Era;
							jd.setDate(d.Year + 1, BaseCalendar.JANUARY, 1);
							Jcal.normalize(jd);
							long jan1 = Jcal.getFixedDate(d);
							long nextJan1 = Jcal.getFixedDate(jd);
							long nextJan1st = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, FirstDayOfWeek);
							int ndays = (int)(nextJan1st - nextJan1);
							if (ndays >= MinimalDaysInFirstWeek)
							{
								nextJan1st -= 7;
							}
							value = GetWeekNumber(jan1, nextJan1st);
						}
						else
						{
							// Get the day of week of January 1 of the year
							CalendarDate d = Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
							d.setDate(date.NormalizedYear, BaseCalendar.JANUARY, 1);
							int dayOfWeek = Gcal.getDayOfWeek(d);
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
						}
						break;
					}

					if (jc == this)
					{
						jc = (JapaneseImperialCalendar) jc.Clone();
					}
					int max = GetActualMaximum(DAY_OF_YEAR);
					jc.Set(DAY_OF_YEAR, max);
					value = jc.Get(WEEK_OF_YEAR);
					if (value == 1 && max > 7)
					{
						jc.Add(WEEK_OF_YEAR, -1);
						value = jc.Get(WEEK_OF_YEAR);
					}
			}
				break;

			case WEEK_OF_MONTH:
			{
					LocalGregorianCalendar.Date jd = Jcal.getCalendarDate(Long.MaxValue, Zone);
					if (!(date.Era == jd.Era && date.Year == jd.Year))
					{
						CalendarDate d = Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
						d.setDate(date.NormalizedYear, date.Month, 1);
						int dayOfWeek = Gcal.getDayOfWeek(d);
						int monthLength = Gcal.getMonthLength(d);
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
					}
					else
					{
						long fd = Jcal.getFixedDate(jd);
						long month1 = fd - jd.DayOfMonth + 1;
						value = GetWeekNumber(month1, fd);
					}
			}
				break;

			case DAY_OF_WEEK_IN_MONTH:
			{
					int ndays, dow1;
					int dow = date.DayOfWeek;
					BaseCalendar.Date d = (BaseCalendar.Date) date.clone();
					ndays = Jcal.getMonthLength(d);
					d.DayOfMonth = 1;
					Jcal.normalize(d);
					dow1 = d.DayOfWeek;
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
			{
					CalendarDate jd = Jcal.getCalendarDate(jc.TimeInMillis, Zone);
					CalendarDate d;
					int eraIndex = GetEraIndex(date);
					if (eraIndex == Eras.Length - 1)
					{
						d = Jcal.getCalendarDate(Long.MaxValue, Zone);
						value = d.Year;
						// Use an equivalent year for the
						// getYearOffsetInMillis call to avoid overflow.
						if (value > 400)
						{
							jd.Year = value - 400;
						}
					}
					else
					{
						d = Jcal.getCalendarDate(Eras[eraIndex + 1].getSince(Zone) - 1, Zone);
						value = d.Year;
						// Use the same year as d.getYear() to be
						// consistent with leap and common years.
						jd.Year = value;
					}
					Jcal.normalize(jd);
					if (GetYearOffsetInMillis(jd) > GetYearOffsetInMillis(d))
					{
						value--;
					}
			}
				break;

			default:
				throw new ArrayIndexOutOfBoundsException(field);
			}
			return value;
		}

		/// <summary>
		/// Returns the millisecond offset from the beginning of the
		/// year. In the year for Long.MIN_VALUE, it's a pseudo value
		/// beyond the limit. The given CalendarDate object must have been
		/// normalized before calling this method.
		/// </summary>
		private long GetYearOffsetInMillis(CalendarDate date)
		{
			long t = (Jcal.getDayOfYear(date) - 1) * ONE_DAY;
			return t + date.TimeOfDay - date.ZoneOffset;
		}

		public override Object Clone()
		{
			JapaneseImperialCalendar other = (JapaneseImperialCalendar) base.Clone();

			other.Jdate = (LocalGregorianCalendar.Date) Jdate.clone();
			other.OriginalFields = null;
			other.ZoneOffsets = null;
			return other;
		}

		public override TimeZone TimeZone
		{
			get
			{
				TimeZone zone = base.TimeZone;
				// To share the zone by the CalendarDate
				Jdate.Zone = zone;
				return zone;
			}
			set
			{
				base.TimeZone = value;
				// To share the value by the CalendarDate
				Jdate.Zone = value;
			}
		}


		/// <summary>
		/// The fixed date corresponding to jdate. If the value is
		/// Long.MIN_VALUE, the fixed date value is unknown.
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
			int mask = 0;
			if (PartiallyNormalized)
			{
				// Determine which calendar fields need to be computed.
				mask = SetStateFields;
				int fieldMask = ~mask & ALL_FIELDS;
				if (fieldMask != 0 || CachedFixedDate == Long.MinValue)
				{
					mask |= ComputeFields(fieldMask, mask & (ZONE_OFFSET_MASK | DST_OFFSET_MASK));
					Debug.Assert(mask == ALL_FIELDS);
				}
			}
			else
			{
				// Specify all fields
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

			// See if we can use jdate to avoid date calculation.
			if (fixedDate != CachedFixedDate || fixedDate < 0)
			{
				Jcal.getCalendarDateFromFixedDate(Jdate, fixedDate);
				CachedFixedDate = fixedDate;
			}
			int era = GetEraIndex(Jdate);
			int year = Jdate.Year;

			// Always set the ERA and YEAR values.
			InternalSet(ERA, era);
			InternalSet(YEAR, year);
			int mask = fieldMask | (ERA_MASK | YEAR_MASK);

			int month = Jdate.Month - 1; // 0-based
			int dayOfMonth = Jdate.DayOfMonth;

			// Set the basic date fields.
			if ((fieldMask & (MONTH_MASK | DAY_OF_MONTH_MASK | DAY_OF_WEEK_MASK)) != 0)
			{
				InternalSet(MONTH, month);
				InternalSet(DAY_OF_MONTH, dayOfMonth);
				InternalSet(DAY_OF_WEEK, Jdate.DayOfWeek);
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
				int normalizedYear = Jdate.NormalizedYear;
				// If it's a year of an era transition, we need to handle
				// irregular year boundaries.
				bool transitionYear = IsTransitionYear(Jdate.NormalizedYear);
				int dayOfYear;
				long fixedDateJan1;
				if (transitionYear)
				{
					fixedDateJan1 = GetFixedDateJan1(Jdate, fixedDate);
					dayOfYear = (int)(fixedDate - fixedDateJan1) + 1;
				}
				else if (normalizedYear == MIN_VALUES[YEAR])
				{
					CalendarDate dx = Jcal.getCalendarDate(Long.MinValue, Zone);
					fixedDateJan1 = Jcal.getFixedDate(dx);
					dayOfYear = (int)(fixedDate - fixedDateJan1) + 1;
				}
				else
				{
					dayOfYear = (int) Jcal.getDayOfYear(Jdate);
					fixedDateJan1 = fixedDate - dayOfYear + 1;
				}
				long fixedDateMonth1 = transitionYear ? GetFixedDateMonth1(Jdate, fixedDate) : fixedDate - dayOfMonth + 1;

				InternalSet(DAY_OF_YEAR, dayOfYear);
				InternalSet(DAY_OF_WEEK_IN_MONTH, (dayOfMonth - 1) / 7 + 1);

				int weekOfYear = GetWeekNumber(fixedDateJan1, fixedDate);

				// The spec is to calculate WEEK_OF_YEAR in the
				// ISO8601-style. This creates problems, though.
				if (weekOfYear == 0)
				{
					// If the date belongs to the last week of the
					// previous year, use the week number of "12/31" of
					// the "previous" year. Again, if the previous year is
					// a transition year, we need to take care of it.
					// Usually the previous day of the first day of a year
					// is December 31, which is not always true in the
					// Japanese imperial calendar system.
					long fixedDec31 = fixedDateJan1 - 1;
					long prevJan1;
					LocalGregorianCalendar.Date d = GetCalendarDate(fixedDec31);
					if (!(transitionYear || IsTransitionYear(d.NormalizedYear)))
					{
						prevJan1 = fixedDateJan1 - 365;
						if (d.LeapYear)
						{
							--prevJan1;
						}
					}
					else if (transitionYear)
					{
						if (Jdate.Year == 1)
						{
							// As of Heisei (since Meiji) there's no case
							// that there are multiple transitions in a
							// year.  Historically there was such
							// case. There might be such case again in the
							// future.
							if (era > HEISEI)
							{
								CalendarDate pd = Eras[era - 1].SinceDate;
								if (normalizedYear == pd.Year)
								{
									d.setMonth(pd.Month).setDayOfMonth(pd.DayOfMonth);
								}
							}
							else
							{
								d.setMonth(LocalGregorianCalendar.JANUARY).setDayOfMonth(1);
							}
							Jcal.normalize(d);
							prevJan1 = Jcal.getFixedDate(d);
						}
						else
						{
							prevJan1 = fixedDateJan1 - 365;
							if (d.LeapYear)
							{
								--prevJan1;
							}
						}
					}
					else
					{
						CalendarDate cd = Eras[GetEraIndex(Jdate)].SinceDate;
						d.setMonth(cd.Month).setDayOfMonth(cd.DayOfMonth);
						Jcal.normalize(d);
						prevJan1 = Jcal.getFixedDate(d);
					}
					weekOfYear = GetWeekNumber(prevJan1, fixedDec31);
				}
				else
				{
					if (!transitionYear)
					{
						// Regular years
						if (weekOfYear >= 52)
						{
							long nextJan1 = fixedDateJan1 + 365;
							if (Jdate.LeapYear)
							{
								nextJan1++;
							}
							long nextJan1st = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, FirstDayOfWeek);
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
						LocalGregorianCalendar.Date d = (LocalGregorianCalendar.Date) Jdate.clone();
						long nextJan1;
						if (Jdate.Year == 1)
						{
							d.addYear(+1);
							d.setMonth(LocalGregorianCalendar.JANUARY).setDayOfMonth(1);
							nextJan1 = Jcal.getFixedDate(d);
						}
						else
						{
							int nextEraIndex = GetEraIndex(d) + 1;
							CalendarDate cd = Eras[nextEraIndex].SinceDate;
							d.Era = Eras[nextEraIndex];
							d.setDate(1, cd.Month, cd.DayOfMonth);
							Jcal.normalize(d);
							nextJan1 = Jcal.getFixedDate(d);
						}
						long nextJan1st = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, FirstDayOfWeek);
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
			// We can always use `jcal' since Julian and Gregorian are the
			// same thing for this calculation.
			long fixedDay1st = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDay1 + 6, FirstDayOfWeek);
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

			int year;
			int era;

			if (IsSet(ERA))
			{
				era = InternalGet(ERA);
				year = IsSet(YEAR) ? InternalGet(YEAR) : 1;
			}
			else
			{
				if (IsSet(YEAR))
				{
					era = Eras.Length - 1;
					year = InternalGet(YEAR);
				}
				else
				{
					// Equivalent to 1970 (Gregorian)
					era = SHOWA;
					year = 45;
				}
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

			// Calculate the fixed date since January 1, 1 (Gregorian).
			fixedDate += GetFixedDate(era, year, fieldMask);

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
					zone.GetOffsets(millis - zone.RawOffset, ZoneOffsets);
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
						int wrongValue = InternalGet(field);
						// Restore the original field values
						System.Array.Copy(OriginalFields, 0, Fields, 0, Fields.Length);
						throw new IllegalArgumentException(GetFieldName(field) + "=" + wrongValue + ", expected " + OriginalFields[field]);
					}
				}
			}
			FieldsNormalized = mask;
		}

		/// <summary>
		/// Computes the fixed date under either the Gregorian or the
		/// Julian calendar, using the given year and the specified calendar fields.
		/// </summary>
		/// <param name="era"> era index </param>
		/// <param name="year"> the normalized year number, with 0 indicating the
		/// year 1 BCE, -1 indicating 2 BCE, etc. </param>
		/// <param name="fieldMask"> the calendar fields to be used for the date calculation </param>
		/// <returns> the fixed date </returns>
		/// <seealso cref= Calendar#selectFields </seealso>
		private long GetFixedDate(int era, int year, int fieldMask)
		{
			int month = JANUARY;
			int firstDayOfMonth = 1;
			if (IsFieldSet(fieldMask, MONTH))
			{
				// No need to check if MONTH has been set (no isSet(MONTH)
				// call) since its unset value happens to be JANUARY (0).
				month = InternalGet(MONTH);

				// If the month is out of range, adjust it into range.
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
			else
			{
				if (year == 1 && era != 0)
				{
					CalendarDate d = Eras[era].SinceDate;
					month = d.Month - 1;
					firstDayOfMonth = d.DayOfMonth;
				}
			}

			// Adjust the base date if year is the minimum value.
			if (year == MIN_VALUES[YEAR])
			{
				CalendarDate dx = Jcal.getCalendarDate(Long.MinValue, Zone);
				int m = dx.Month - 1;
				if (month < m)
				{
					month = m;
				}
				if (month == m)
				{
					firstDayOfMonth = dx.DayOfMonth;
				}
			}

			LocalGregorianCalendar.Date date = Jcal.newCalendarDate(TimeZone.NO_TIMEZONE);
			date.Era = era > 0 ? Eras[era] : null;
			date.setDate(year, month + 1, firstDayOfMonth);
			Jcal.normalize(date);

			// Get the fixed date since Jan 1, 1 (Gregorian). We are on
			// the first day of either `month' or January in 'year'.
			long fixedDate = Jcal.getFixedDate(date);

			if (IsFieldSet(fieldMask, MONTH))
			{
				// Month-based calculations
				if (IsFieldSet(fieldMask, DAY_OF_MONTH))
				{
					// We are on the "first day" of the month (which may
					// not be 1). Just add the offset if DAY_OF_MONTH is
					// set. If the isSet call returns false, that means
					// DAY_OF_MONTH has been selected just because of the
					// selected combination. We don't need to add any
					// since the default value is the "first day".
					if (IsSet(DAY_OF_MONTH))
					{
						// To avoid underflow with DAY_OF_MONTH-firstDayOfMonth, add
						// DAY_OF_MONTH, then subtract firstDayOfMonth.
						fixedDate += InternalGet(DAY_OF_MONTH);
						fixedDate -= firstDayOfMonth;
					}
				}
				else
				{
					if (IsFieldSet(fieldMask, WEEK_OF_MONTH))
					{
						long firstDayOfWeek = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate + 6, FirstDayOfWeek);
						// If we have enough days in the first week, then
						// move to the previous week.
						if ((firstDayOfWeek - fixedDate) >= MinimalDaysInFirstWeek)
						{
							firstDayOfWeek -= 7;
						}
						if (IsFieldSet(fieldMask, DAY_OF_WEEK))
						{
							firstDayOfWeek = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek + 6, InternalGet(DAY_OF_WEEK));
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
							fixedDate = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate + (7 * dowim) - 1, dayOfWeek);
						}
						else
						{
							// Go to the first day of the next week of
							// the specified week boundary.
							int lastDate = MonthLength(month, year) + (7 * (dowim + 1));
							// Then, get the day of week date on or before the last date.
							fixedDate = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate + lastDate - 1, dayOfWeek);
						}
					}
				}
			}
			else
			{
				// We are on the first day of the year.
				if (IsFieldSet(fieldMask, DAY_OF_YEAR))
				{
					if (IsTransitionYear(date.NormalizedYear))
					{
						fixedDate = GetFixedDateJan1(date, fixedDate);
					}
					// Add the offset, then subtract 1. (Make sure to avoid underflow.)
					fixedDate += InternalGet(DAY_OF_YEAR);
					fixedDate--;
				}
				else
				{
					long firstDayOfWeek = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate + 6, FirstDayOfWeek);
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
							firstDayOfWeek = LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek + 6, dayOfWeek);
						}
					}
					fixedDate = firstDayOfWeek + 7 * ((long)InternalGet(WEEK_OF_YEAR) - 1);
				}
			}
			return fixedDate;
		}

		/// <summary>
		/// Returns the fixed date of the first day of the year (usually
		/// January 1) before the specified date.
		/// </summary>
		/// <param name="date"> the date for which the first day of the year is
		/// calculated. The date has to be in the cut-over year. </param>
		/// <param name="fixedDate"> the fixed date representation of the date </param>
		private long GetFixedDateJan1(LocalGregorianCalendar.Date date, long fixedDate)
		{
			Era era = date.Era;
			if (date.Era != null && date.Year == 1)
			{
				for (int eraIndex = GetEraIndex(date); eraIndex > 0; eraIndex--)
				{
					CalendarDate d = Eras[eraIndex].SinceDate;
					long fd = Gcal.getFixedDate(d);
					// There might be multiple era transitions in a year.
					if (fd > fixedDate)
					{
						continue;
					}
					return fd;
				}
			}
			CalendarDate d = Gcal.newCalendarDate(TimeZone.NO_TIMEZONE);
			d.setDate(date.NormalizedYear, Gregorian.JANUARY, 1);
			return Gcal.getFixedDate(d);
		}

		/// <summary>
		/// Returns the fixed date of the first date of the month (usually
		/// the 1st of the month) before the specified date.
		/// </summary>
		/// <param name="date"> the date for which the first day of the month is
		/// calculated. The date must be in the era transition year. </param>
		/// <param name="fixedDate"> the fixed date representation of the date </param>
		private long GetFixedDateMonth1(LocalGregorianCalendar.Date date, long fixedDate)
		{
			int eraIndex = GetTransitionEraIndex(date);
			if (eraIndex != -1)
			{
				long transition = SinceFixedDates[eraIndex];
				// If the given date is on or after the transition date, then
				// return the transition date.
				if (transition <= fixedDate)
				{
					return transition;
				}
			}

			// Otherwise, we can use the 1st day of the month.
			return fixedDate - date.DayOfMonth + 1;
		}

		/// <summary>
		/// Returns a LocalGregorianCalendar.Date produced from the specified fixed date.
		/// </summary>
		/// <param name="fd"> the fixed date </param>
		private static LocalGregorianCalendar.Date GetCalendarDate(long fd)
		{
			LocalGregorianCalendar.Date d = Jcal.newCalendarDate(TimeZone.NO_TIMEZONE);
			Jcal.getCalendarDateFromFixedDate(d, fd);
			return d;
		}

		/// <summary>
		/// Returns the length of the specified month in the specified
		/// Gregorian year. The year number must be normalized.
		/// </summary>
		/// <seealso cref= GregorianCalendar#isLeapYear(int) </seealso>
		private int MonthLength(int month, int gregorianYear)
		{
			return CalendarUtils.isGregorianLeapYear(gregorianYear) ? GregorianCalendar.LEAP_MONTH_LENGTH[month] : GregorianCalendar.MONTH_LENGTH[month];
		}

		/// <summary>
		/// Returns the length of the specified month in the year provided
		/// by internalGet(YEAR).
		/// </summary>
		/// <seealso cref= GregorianCalendar#isLeapYear(int) </seealso>
		private int MonthLength(int month)
		{
			Debug.Assert(Jdate.Normalized);
			return Jdate.LeapYear ? GregorianCalendar.LEAP_MONTH_LENGTH[month] : GregorianCalendar.MONTH_LENGTH[month];
		}

		private int ActualMonthLength()
		{
			int length = Jcal.getMonthLength(Jdate);
			int eraIndex = GetTransitionEraIndex(Jdate);
			if (eraIndex == -1)
			{
				long transitionFixedDate = SinceFixedDates[eraIndex];
				CalendarDate d = Eras[eraIndex].SinceDate;
				if (transitionFixedDate <= CachedFixedDate)
				{
					length -= d.DayOfMonth - 1;
				}
				else
				{
					length = d.DayOfMonth - 1;
				}
			}
			return length;
		}

		/// <summary>
		/// Returns the index to the new era if the given date is in a
		/// transition month.  For example, if the give date is Heisei 1
		/// (1989) January 20, then the era index for Heisei is
		/// returned. Likewise, if the given date is Showa 64 (1989)
		/// January 3, then the era index for Heisei is returned. If the
		/// given date is not in any transition month, then -1 is returned.
		/// </summary>
		private static int GetTransitionEraIndex(LocalGregorianCalendar.Date date)
		{
			int eraIndex = GetEraIndex(date);
			CalendarDate transitionDate = Eras[eraIndex].SinceDate;
			if (transitionDate.Year == date.NormalizedYear && transitionDate.Month == date.Month)
			{
				return eraIndex;
			}
			if (eraIndex < Eras.Length - 1)
			{
				transitionDate = Eras[++eraIndex].SinceDate;
				if (transitionDate.Year == date.NormalizedYear && transitionDate.Month == date.Month)
				{
					return eraIndex;
				}
			}
			return -1;
		}

		private bool IsTransitionYear(int normalizedYear)
		{
			for (int i = Eras.Length - 1; i > 0; i--)
			{
				int transitionYear = Eras[i].SinceDate.Year;
				if (normalizedYear == transitionYear)
				{
					return true;
				}
				if (normalizedYear > transitionYear)
				{
					break;
				}
			}
			return false;
		}

		private static int GetEraIndex(LocalGregorianCalendar.Date date)
		{
			Era era = date.Era;
			for (int i = Eras.Length - 1; i > 0; i--)
			{
				if (Eras[i] == era)
				{
					return i;
				}
			}
			return 0;
		}

		/// <summary>
		/// Returns this object if it's normalized (all fields and time are
		/// in sync). Otherwise, a cloned object is returned after calling
		/// complete() in lenient mode.
		/// </summary>
		private JapaneseImperialCalendar NormalizedCalendar
		{
			get
			{
				JapaneseImperialCalendar jc;
				if (FullyNormalized)
				{
					jc = this;
				}
				else
				{
					// Create a clone and normalize the calendar fields
					jc = (JapaneseImperialCalendar) this.Clone();
					jc.Lenient = true;
					jc.Complete();
				}
				return jc;
			}
		}

		/// <summary>
		/// After adjustments such as add(MONTH), add(YEAR), we don't want the
		/// month to jump around.  E.g., we don't want Jan 31 + 1 month to go to Mar
		/// 3, we want it to go to Feb 28.  Adjustments which might run into this
		/// problem call this method to retain the proper month.
		/// </summary>
		private void PinDayOfMonth(LocalGregorianCalendar.Date date)
		{
			int year = date.Year;
			int dom = date.DayOfMonth;
			if (year != GetMinimum(YEAR))
			{
				date.DayOfMonth = 1;
				Jcal.normalize(date);
				int monthLength = Jcal.getMonthLength(date);
				if (dom > monthLength)
				{
					date.DayOfMonth = monthLength;
				}
				else
				{
					date.DayOfMonth = dom;
				}
				Jcal.normalize(date);
			}
			else
			{
				LocalGregorianCalendar.Date d = Jcal.getCalendarDate(Long.MinValue, Zone);
				LocalGregorianCalendar.Date realDate = Jcal.getCalendarDate(Time_Renamed, Zone);
				long tod = realDate.TimeOfDay;
				// Use an equivalent year.
				realDate.addYear(+400);
				realDate.Month = date.Month;
				realDate.DayOfMonth = 1;
				Jcal.normalize(realDate);
				int monthLength = Jcal.getMonthLength(realDate);
				if (dom > monthLength)
				{
					realDate.DayOfMonth = monthLength;
				}
				else
				{
					if (dom < d.DayOfMonth)
					{
						realDate.DayOfMonth = d.DayOfMonth;
					}
					else
					{
						realDate.DayOfMonth = dom;
					}
				}
				if (realDate.DayOfMonth == d.DayOfMonth && tod < d.TimeOfDay)
				{
					realDate.DayOfMonth = System.Math.Min(dom + 1, monthLength);
				}
				// restore the year.
				date.setDate(year, realDate.Month, realDate.DayOfMonth);
				// Don't normalize date here so as not to cause underflow.
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
		/// default ERA is the current era, but a zero (unset) ERA means before Meiji.
		/// </summary>
		private int InternalGetEra()
		{
			return IsSet(ERA) ? InternalGet(ERA) : Eras.Length - 1;
		}

		/// <summary>
		/// Updates internal state.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			if (Jdate == null)
			{
				Jdate = Jcal.newCalendarDate(Zone);
				CachedFixedDate = Long.MinValue;
			}
		}
	}

}