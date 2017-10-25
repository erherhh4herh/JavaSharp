using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

/*
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
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.format
{



	using CalendarDataUtility = sun.util.locale.provider.CalendarDataUtility;
	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleResources = sun.util.locale.provider.LocaleResources;

	/// <summary>
	/// A provider to obtain the textual form of a date-time field.
	/// 
	/// @implSpec
	/// Implementations must be thread-safe.
	/// Implementations should cache the textual information.
	/// 
	/// @since 1.8
	/// </summary>
	internal class DateTimeTextProvider
	{

		/// <summary>
		/// Cache. </summary>
		private static readonly ConcurrentMap<Map_Entry<TemporalField, Locale>, Object> CACHE = new ConcurrentDictionary<Map_Entry<TemporalField, Locale>, Object>(16, 0.75f, 2);
		/// <summary>
		/// Comparator. </summary>
		private static readonly IComparer<Map_Entry<String, Long>> COMPARATOR = new ComparatorAnonymousInnerClassHelper();

		private class ComparatorAnonymousInnerClassHelper : Comparator<Map_Entry<String, Long>>
		{
			public ComparatorAnonymousInnerClassHelper()
			{
			}

			public virtual int Compare(Map_Entry<String, Long> obj1, Map_Entry<String, Long> obj2)
			{
				return obj2.Key.Length() - obj1.Key.Length(); // longest to shortest
			}
		}

		internal DateTimeTextProvider()
		{
		}

		/// <summary>
		/// Gets the provider of text.
		/// </summary>
		/// <returns> the provider, not null </returns>
		internal static DateTimeTextProvider Instance
		{
			get
			{
				return new DateTimeTextProvider();
			}
		}

		/// <summary>
		/// Gets the text for the specified field, locale and style
		/// for the purpose of formatting.
		/// <para>
		/// The text associated with the value is returned.
		/// The null return value should be used if there is no applicable text, or
		/// if the text would be a numeric representation of the value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get text for, not null </param>
		/// <param name="value">  the field value to get text for, not null </param>
		/// <param name="style">  the style to get text for, not null </param>
		/// <param name="locale">  the locale to get text for, not null </param>
		/// <returns> the text for the field value, null if no text found </returns>
		public virtual String GetText(TemporalField field, long value, TextStyle style, Locale locale)
		{
			Object store = FindStore(field, locale);
			if (store is LocaleStore)
			{
				return ((LocaleStore) store).GetText(value, style);
			}
			return null;
		}

		/// <summary>
		/// Gets the text for the specified chrono, field, locale and style
		/// for the purpose of formatting.
		/// <para>
		/// The text associated with the value is returned.
		/// The null return value should be used if there is no applicable text, or
		/// if the text would be a numeric representation of the value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chrono">  the Chronology to get text for, not null </param>
		/// <param name="field">  the field to get text for, not null </param>
		/// <param name="value">  the field value to get text for, not null </param>
		/// <param name="style">  the style to get text for, not null </param>
		/// <param name="locale">  the locale to get text for, not null </param>
		/// <returns> the text for the field value, null if no text found </returns>
		public virtual String GetText(Chronology chrono, TemporalField field, long value, TextStyle style, Locale locale)
		{
			if (chrono == IsoChronology.INSTANCE || !(field is ChronoField))
			{
				return GetText(field, value, style, locale);
			}

			int fieldIndex;
			int fieldValue;
			if (field == ERA)
			{
				fieldIndex = DateTime.ERA;
				if (chrono == JapaneseChronology.INSTANCE)
				{
					if (value == -999)
					{
						fieldValue = 0;
					}
					else
					{
						fieldValue = (int) value + 2;
					}
				}
				else
				{
					fieldValue = (int) value;
				}
			}
			else if (field == MONTH_OF_YEAR)
			{
				fieldIndex = DateTime.MONTH;
				fieldValue = (int) value - 1;
			}
			else if (field == DAY_OF_WEEK)
			{
				fieldIndex = DateTime.DAY_OF_WEEK;
				fieldValue = (int) value + 1;
				if (fieldValue > 7)
				{
					fieldValue = DayOfWeek.Sunday;
				}
			}
			else if (field == AMPM_OF_DAY)
			{
				fieldIndex = DateTime.AM_PM;
				fieldValue = (int) value;
			}
			else
			{
				return null;
			}
			return CalendarDataUtility.retrieveJavaTimeFieldValueName(chrono.CalendarType, fieldIndex, fieldValue, style.toCalendarStyle(), locale);
		}

		/// <summary>
		/// Gets an iterator of text to field for the specified field, locale and style
		/// for the purpose of parsing.
		/// <para>
		/// The iterator must be returned in order from the longest text to the shortest.
		/// </para>
		/// <para>
		/// The null return value should be used if there is no applicable parsable text, or
		/// if the text would be a numeric representation of the value.
		/// Text can only be parsed if all the values for that field-style-locale combination are unique.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to get text for, not null </param>
		/// <param name="style">  the style to get text for, null for all parsable text </param>
		/// <param name="locale">  the locale to get text for, not null </param>
		/// <returns> the iterator of text to field pairs, in order from longest text to shortest text,
		///  null if the field or style is not parsable </returns>
		public virtual IEnumerator<Map_Entry<String, Long>> GetTextIterator(TemporalField field, TextStyle style, Locale locale)
		{
			Object store = FindStore(field, locale);
			if (store is LocaleStore)
			{
				return ((LocaleStore) store).GetTextIterator(style);
			}
			return null;
		}

		/// <summary>
		/// Gets an iterator of text to field for the specified chrono, field, locale and style
		/// for the purpose of parsing.
		/// <para>
		/// The iterator must be returned in order from the longest text to the shortest.
		/// </para>
		/// <para>
		/// The null return value should be used if there is no applicable parsable text, or
		/// if the text would be a numeric representation of the value.
		/// Text can only be parsed if all the values for that field-style-locale combination are unique.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chrono">  the Chronology to get text for, not null </param>
		/// <param name="field">  the field to get text for, not null </param>
		/// <param name="style">  the style to get text for, null for all parsable text </param>
		/// <param name="locale">  the locale to get text for, not null </param>
		/// <returns> the iterator of text to field pairs, in order from longest text to shortest text,
		///  null if the field or style is not parsable </returns>
		public virtual IEnumerator<Map_Entry<String, Long>> GetTextIterator(Chronology chrono, TemporalField field, TextStyle style, Locale locale)
		{
			if (chrono == IsoChronology.INSTANCE || !(field is ChronoField))
			{
				return GetTextIterator(field, style, locale);
			}

			int fieldIndex;
			switch ((ChronoField)field)
			{
			case ERA:
				fieldIndex = DateTime.ERA;
				break;
			case MONTH_OF_YEAR:
				fieldIndex = DateTime.MONTH;
				break;
			case DAY_OF_WEEK:
				fieldIndex = DateTime.DAY_OF_WEEK;
				break;
			case AMPM_OF_DAY:
				fieldIndex = DateTime.AM_PM;
				break;
			default:
				return null;
			}

			int calendarStyle = (style == null) ? DateTime.ALL_STYLES : style.toCalendarStyle();
			IDictionary<String, Integer> map = CalendarDataUtility.retrieveJavaTimeFieldValueNames(chrono.CalendarType, fieldIndex, calendarStyle, locale);
			if (map == null)
			{
				return null;
			}
			IList<Map_Entry<String, Long>> list = new List<Map_Entry<String, Long>>(map.Count);
			switch (fieldIndex)
			{
			case DateTime.ERA:
				foreach (Map_Entry<String, Integer> entry in map)
				{
					int era = entry.Value;
					if (chrono == JapaneseChronology.INSTANCE)
					{
						if (era == 0)
						{
							era = -999;
						}
						else
						{
							era -= 2;
						}
					}
					list.Add(CreateEntry(entry.Key, (long)era));
				}
				break;
			case DateTime.MONTH:
				foreach (Map_Entry<String, Integer> entry in map)
				{
					list.Add(CreateEntry(entry.Key, (long)(entry.Value + 1)));
				}
				break;
			case DateTime.DAY_OF_WEEK:
				foreach (Map_Entry<String, Integer> entry in map)
				{
					list.Add(CreateEntry(entry.Key, (long)ToWeekDay(entry.Value)));
				}
				break;
			default:
				foreach (Map_Entry<String, Integer> entry in map)
				{
					list.Add(CreateEntry(entry.Key, (long)entry.Value));
				}
				break;
			}
			return list.GetEnumerator();
		}

		private Object FindStore(TemporalField field, Locale locale)
		{
			Map_Entry<TemporalField, Locale> key = CreateEntry(field, locale);
			Object store = CACHE[key];
			if (store == null)
			{
				store = CreateStore(field, locale);
				CACHE.PutIfAbsent(key, store);
				store = CACHE[key];
			}
			return store;
		}

		private static int ToWeekDay(int calWeekDay)
		{
			if (calWeekDay == DayOfWeek.Sunday)
			{
				return 7;
			}
			else
			{
				return calWeekDay - 1;
			}
		}

		private Object CreateStore(TemporalField field, Locale locale)
		{
			IDictionary<TextStyle, IDictionary<Long, String>> styleMap = new Dictionary<TextStyle, IDictionary<Long, String>>();
			if (field == ERA)
			{
				foreach (TextStyle textStyle in TextStyle.values())
				{
					if (textStyle.Standalone)
					{
						// Stand-alone isn't applicable to era names.
						continue;
					}
					IDictionary<String, Integer> displayNames = CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.ERA, textStyle.toCalendarStyle(), locale);
					if (displayNames != null)
					{
						IDictionary<Long, String> map = new Dictionary<Long, String>();
						foreach (Map_Entry<String, Integer> entry in displayNames)
						{
							map[(long) entry.Value] = entry.Key;
						}
						if (map.Count > 0)
						{
							styleMap[textStyle] = map;
						}
					}
				}
				return new LocaleStore(styleMap);
			}

			if (field == MONTH_OF_YEAR)
			{
				foreach (TextStyle textStyle in TextStyle.values())
				{
					IDictionary<String, Integer> displayNames = CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.MONTH, textStyle.toCalendarStyle(), locale);
					IDictionary<Long, String> map = new Dictionary<Long, String>();
					if (displayNames != null)
					{
						foreach (Map_Entry<String, Integer> entry in displayNames)
						{
							map[(long)(entry.Value + 1)] = entry.Key;
						}

					}
					else
					{
						// Narrow names may have duplicated names, such as "J" for January, Jun, July.
						// Get names one by one in that case.
						for (int month = 1; month <= 12; month++)
						{
							String name;
							name = CalendarDataUtility.retrieveJavaTimeFieldValueName("gregory", DateTime.MONTH, month, textStyle.toCalendarStyle(), locale);
							if (name == null)
							{
								break;
							}
							map[(long)(month + 1)] = name;
						}
					}
					if (map.Count > 0)
					{
						styleMap[textStyle] = map;
					}
				}
				return new LocaleStore(styleMap);
			}

			if (field == DAY_OF_WEEK)
			{
				foreach (TextStyle textStyle in TextStyle.values())
				{
					IDictionary<String, Integer> displayNames = CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.DAY_OF_WEEK, textStyle.toCalendarStyle(), locale);
					IDictionary<Long, String> map = new Dictionary<Long, String>();
					if (displayNames != null)
					{
						foreach (Map_Entry<String, Integer> entry in displayNames)
						{
							map[(long)ToWeekDay(entry.Value)] = entry.Key;
						}

					}
					else
					{
						// Narrow names may have duplicated names, such as "S" for Sunday and Saturday.
						// Get names one by one in that case.
						for (int wday = DayOfWeek.Sunday; wday <= DayOfWeek.Saturday; wday++)
						{
							String name;
							name = CalendarDataUtility.retrieveJavaTimeFieldValueName("gregory", DateTime.DAY_OF_WEEK, wday, textStyle.toCalendarStyle(), locale);
							if (name == null)
							{
								break;
							}
							map[(long)ToWeekDay(wday)] = name;
						}
					}
					if (map.Count > 0)
					{
						styleMap[textStyle] = map;
					}
				}
				return new LocaleStore(styleMap);
			}

			if (field == AMPM_OF_DAY)
			{
				foreach (TextStyle textStyle in TextStyle.values())
				{
					if (textStyle.Standalone)
					{
						// Stand-alone isn't applicable to AM/PM.
						continue;
					}
					IDictionary<String, Integer> displayNames = CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.AM_PM, textStyle.toCalendarStyle(), locale);
					if (displayNames != null)
					{
						IDictionary<Long, String> map = new Dictionary<Long, String>();
						foreach (Map_Entry<String, Integer> entry in displayNames)
						{
							map[(long) entry.Value] = entry.Key;
						}
						if (map.Count > 0)
						{
							styleMap[textStyle] = map;
						}
					}
				}
				return new LocaleStore(styleMap);
			}

			if (field == IsoFields.QUARTER_OF_YEAR)
			{
				// The order of keys must correspond to the TextStyle.values() order.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] keys = { "QuarterNames", "standalone.QuarterNames", "QuarterAbbreviations", "standalone.QuarterAbbreviations", "QuarterNarrows", "standalone.QuarterNarrows"};
				String[] keys = new String[] {"QuarterNames", "standalone.QuarterNames", "QuarterAbbreviations", "standalone.QuarterAbbreviations", "QuarterNarrows", "standalone.QuarterNarrows"};
				for (int i = 0; i < keys.Length; i++)
				{
					String[] names = GetLocalizedResource(keys[i], locale);
					if (names != null)
					{
						IDictionary<Long, String> map = new Dictionary<Long, String>();
						for (int q = 0; q < names.Length; q++)
						{
							map[(long)(q + 1)] = names[q];
						}
						styleMap[TextStyle.values()[i]] = map;
					}
				}
				return new LocaleStore(styleMap);
			}

			return ""; // null marker for map
		}

		/// <summary>
		/// Helper method to create an immutable entry.
		/// </summary>
		/// <param name="text">  the text, not null </param>
		/// <param name="field">  the field, not null </param>
		/// <returns> the entry, not null </returns>
		private static Map_Entry<A, B> createEntry<A, B>(A text, B field)
		{
			return new SimpleImmutableEntry<>(text, field);
		}

		/// <summary>
		/// Returns the localized resource of the given key and locale, or null
		/// if no localized resource is available.
		/// </summary>
		/// <param name="key">  the key of the localized resource, not null </param>
		/// <param name="locale">  the locale, not null </param>
		/// <returns> the localized resource, or null if not available </returns>
		/// <exception cref="NullPointerException"> if key or locale is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <T> T getLocalizedResource(String key, java.util.Locale locale)
		internal static T getLocalizedResource<T>(String key, Locale locale)
		{
			LocaleResources lr = LocaleProviderAdapter.ResourceBundleBased.getLocaleResources(locale);
			ResourceBundle rb = lr.JavaTimeFormatData;
			return rb.ContainsKey(key) ? (T) rb.GetObject(key) : null;
		}

		/// <summary>
		/// Stores the text for a single locale.
		/// <para>
		/// Some fields have a textual representation, such as day-of-week or month-of-year.
		/// These textual representations can be captured in this class for printing
		/// and parsing.
		/// </para>
		/// <para>
		/// This class is immutable and thread-safe.
		/// </para>
		/// </summary>
		internal sealed class LocaleStore
		{
			/// <summary>
			/// Map of value to text.
			/// </summary>
			internal readonly IDictionary<TextStyle, IDictionary<Long, String>> ValueTextMap;
			/// <summary>
			/// Parsable data.
			/// </summary>
			internal readonly IDictionary<TextStyle, IList<Map_Entry<String, Long>>> Parsable;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="valueTextMap">  the map of values to text to store, assigned and not altered, not null </param>
			internal LocaleStore(IDictionary<TextStyle, IDictionary<Long, String>> valueTextMap)
			{
				this.ValueTextMap = valueTextMap;
				IDictionary<TextStyle, IList<Map_Entry<String, Long>>> map = new Dictionary<TextStyle, IList<Map_Entry<String, Long>>>();
				IList<Map_Entry<String, Long>> allList = new List<Map_Entry<String, Long>>();
				foreach (Map_Entry<TextStyle, IDictionary<Long, String>> vtmEntry in valueTextMap)
				{
					IDictionary<String, Map_Entry<String, Long>> reverse = new Dictionary<String, Map_Entry<String, Long>>();
					foreach (Map_Entry<Long, String> entry in vtmEntry.Value)
					{
						if (reverse.put(entry.Value, CreateEntry(entry.Value, entry.Key)) != null)
						{
							// TODO: BUG: this has no effect
							continue; // not parsable, try next style
						}
					}
					IList<Map_Entry<String, Long>> list = new List<Map_Entry<String, Long>>(reverse.Values);
					list.Sort(COMPARATOR);
					map[vtmEntry.Key] = list;
					allList.AddRange(list);
					map[null] = allList;
				}
				allList.Sort(COMPARATOR);
				this.Parsable = map;
			}

			/// <summary>
			/// Gets the text for the specified field value, locale and style
			/// for the purpose of printing.
			/// </summary>
			/// <param name="value">  the value to get text for, not null </param>
			/// <param name="style">  the style to get text for, not null </param>
			/// <returns> the text for the field value, null if no text found </returns>
			internal String GetText(long value, TextStyle style)
			{
				IDictionary<Long, String> map = ValueTextMap[style];
				return map != null ? map[value] : null;
			}

			/// <summary>
			/// Gets an iterator of text to field for the specified style for the purpose of parsing.
			/// <para>
			/// The iterator must be returned in order from the longest text to the shortest.
			/// 
			/// </para>
			/// </summary>
			/// <param name="style">  the style to get text for, null for all parsable text </param>
			/// <returns> the iterator of text to field pairs, in order from longest text to shortest text,
			///  null if the style is not parsable </returns>
			internal IEnumerator<Map_Entry<String, Long>> GetTextIterator(TextStyle style)
			{
				IList<Map_Entry<String, Long>> list = Parsable[style];
				return list != null ? list.GetEnumerator() : null;
			}
		}
	}

}