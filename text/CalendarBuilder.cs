using System;

/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.text
{


	/// <summary>
	/// {@code CalendarBuilder} keeps field-value pairs for setting
	/// the calendar fields of the given {@code Calendar}. It has the
	/// <seealso cref="Calendar#FIELD_COUNT FIELD_COUNT"/>-th field for the week year
	/// support. Also {@code ISO_DAY_OF_WEEK} is used to specify
	/// {@code DAY_OF_WEEK} in the ISO day of week numbering.
	/// 
	/// <para>{@code CalendarBuilder} retains the semantic of the pseudo
	/// timestamp for fields. {@code CalendarBuilder} uses a single
	/// int array combining fields[] and stamp[] of {@code Calendar}.
	/// 
	/// @author Masayoshi Okutsu
	/// </para>
	/// </summary>
	internal class CalendarBuilder
	{
		/*
		 * Pseudo time stamp constants used in java.util.Calendar
		 */
		private const int UNSET = 0;
		private const int COMPUTED = 1;
		private const int MINIMUM_USER_STAMP = 2;

		private static readonly int MAX_FIELD = FIELD_COUNT + 1;

		public static readonly int WEEK_YEAR = FIELD_COUNT;
		public const int ISO_DAY_OF_WEEK = 1000; // pseudo field index

		// stamp[] (lower half) and field[] (upper half) combined
		private readonly int[] Field;
		private int NextStamp;
		private int MaxFieldIndex;

		internal CalendarBuilder()
		{
			Field = new int[MAX_FIELD * 2];
			NextStamp = MINIMUM_USER_STAMP;
			MaxFieldIndex = -1;
		}

		internal virtual CalendarBuilder Set(int index, int value)
		{
			if (index == ISO_DAY_OF_WEEK)
			{
				index = DAY_OF_WEEK;
				value = ToCalendarDayOfWeek(value);
			}
			Field[index] = NextStamp++;
			Field[MAX_FIELD + index] = value;
			if (index > MaxFieldIndex && index < FIELD_COUNT)
			{
				MaxFieldIndex = index;
			}
			return this;
		}

		internal virtual CalendarBuilder AddYear(int value)
		{
			Field[MAX_FIELD + YEAR] += value;
			Field[MAX_FIELD + WEEK_YEAR] += value;
			return this;
		}

		internal virtual bool IsSet(int index)
		{
			if (index == ISO_DAY_OF_WEEK)
			{
				index = DAY_OF_WEEK;
			}
			return Field[index] > UNSET;
		}

		internal virtual CalendarBuilder Clear(int index)
		{
			if (index == ISO_DAY_OF_WEEK)
			{
				index = DAY_OF_WEEK;
			}
			Field[index] = UNSET;
			Field[MAX_FIELD + index] = 0;
			return this;
		}

		internal virtual DateTime Establish(DateTime cal)
		{
			bool weekDate = IsSet(WEEK_YEAR) && Field[WEEK_YEAR] > Field[YEAR];
			if (weekDate && !cal.WeekDateSupported)
			{
				// Use YEAR instead
				if (!IsSet(YEAR))
				{
					Set(YEAR, Field[MAX_FIELD + WEEK_YEAR]);
				}
				weekDate = false;
			}

			cal.clear();
			// Set the fields from the min stamp to the max stamp so that
			// the field resolution works in the Calendar.
			for (int stamp = MINIMUM_USER_STAMP; stamp < NextStamp; stamp++)
			{
				for (int index = 0; index <= MaxFieldIndex; index++)
				{
					if (Field[index] == stamp)
					{
						cal.set(index, Field[MAX_FIELD + index]);
						break;
					}
				}
			}

			if (weekDate)
			{
				int weekOfYear = IsSet(WEEK_OF_YEAR) ? Field[MAX_FIELD + WEEK_OF_YEAR] : 1;
				int dayOfWeek = IsSet(DAY_OF_WEEK) ? Field[MAX_FIELD + DAY_OF_WEEK] : cal.FirstDayOfWeek;
				if (!IsValidDayOfWeek(dayOfWeek) && cal.Lenient)
				{
					if (dayOfWeek >= 8)
					{
						dayOfWeek--;
						weekOfYear += dayOfWeek / 7;
						dayOfWeek = (dayOfWeek % 7) + 1;
					}
					else
					{
						while (dayOfWeek <= 0)
						{
							dayOfWeek += 7;
							weekOfYear--;
						}
					}
					dayOfWeek = ToCalendarDayOfWeek(dayOfWeek);
				}
				cal.setWeekDate(Field[MAX_FIELD + WEEK_YEAR], weekOfYear, dayOfWeek);
			}
			return cal;
		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CalendarBuilder:[");
			for (int i = 0; i < Field.Length; i++)
			{
				if (IsSet(i))
				{
					sb.Append(i).Append('=').Append(Field[MAX_FIELD + i]).Append(',');
				}
			}
			int lastIndex = sb.Length() - 1;
			if (sb.CharAt(lastIndex) == ',')
			{
				sb.Length = lastIndex;
			}
			sb.Append(']');
			return sb.ToString();
		}

		internal static int ToISODayOfWeek(int calendarDayOfWeek)
		{
			return calendarDayOfWeek == SUNDAY ? 7 : calendarDayOfWeek - 1;
		}

		internal static int ToCalendarDayOfWeek(int isoDayOfWeek)
		{
			if (!IsValidDayOfWeek(isoDayOfWeek))
			{
				// adjust later for lenient mode
				return isoDayOfWeek;
			}
			return isoDayOfWeek == 7 ? SUNDAY : isoDayOfWeek + 1;
		}

		internal static bool IsValidDayOfWeek(int dayOfWeek)
		{
			return dayOfWeek > 0 && dayOfWeek <= 7;
		}
	}

}