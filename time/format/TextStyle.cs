using System.Collections.Generic;

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
 * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
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

	/// <summary>
	/// Enumeration of the style of text formatting and parsing.
	/// <para>
	/// Text styles define three sizes for the formatted text - 'full', 'short' and 'narrow'.
	/// Each of these three sizes is available in both 'standard' and 'stand-alone' variations.
	/// </para>
	/// <para>
	/// The difference between the three sizes is obvious in most languages.
	/// For example, in English the 'full' month is 'January', the 'short' month is 'Jan'
	/// and the 'narrow' month is 'J'. Note that the narrow size is often not unique.
	/// For example, 'January', 'June' and 'July' all have the 'narrow' text 'J'.
	/// </para>
	/// <para>
	/// The difference between the 'standard' and 'stand-alone' forms is trickier to describe
	/// as there is no difference in English. However, in other languages there is a difference
	/// in the word used when the text is used alone, as opposed to in a complete date.
	/// For example, the word used for a month when used alone in a date picker is different
	/// to the word used for month in association with a day and year in a date.
	/// 
	/// @implSpec
	/// This is immutable and thread-safe enum.
	/// </para>
	/// </summary>
	public sealed class TextStyle
	{
		// ordered from large to small
		// ordered so that bit 0 of the ordinal indicates stand-alone.

		/// <summary>
		/// Full text, typically the full description.
		/// For example, day-of-week Monday might output "Monday".
		/// </summary>
		public static readonly TextStyle FULL = new TextStyle("FULL", InnerEnum.FULL, java.util.Calendar.LONG_FORMAT, 0);
		/// <summary>
		/// Full text for stand-alone use, typically the full description.
		/// For example, day-of-week Monday might output "Monday".
		/// </summary>
		public static readonly TextStyle FULL_STANDALONE = new TextStyle("FULL_STANDALONE", InnerEnum.FULL_STANDALONE, java.util.Calendar.LONG_STANDALONE, 0);
		/// <summary>
		/// Short text, typically an abbreviation.
		/// For example, day-of-week Monday might output "Mon".
		/// </summary>
		public static readonly TextStyle SHORT = new TextStyle("SHORT", InnerEnum.SHORT, java.util.Calendar.SHORT_FORMAT, 1);
		/// <summary>
		/// Short text for stand-alone use, typically an abbreviation.
		/// For example, day-of-week Monday might output "Mon".
		/// </summary>
		public static readonly TextStyle SHORT_STANDALONE = new TextStyle("SHORT_STANDALONE", InnerEnum.SHORT_STANDALONE, java.util.Calendar.SHORT_STANDALONE, 1);
		/// <summary>
		/// Narrow text, typically a single letter.
		/// For example, day-of-week Monday might output "M".
		/// </summary>
		public static readonly TextStyle NARROW = new TextStyle("NARROW", InnerEnum.NARROW, java.util.Calendar.NARROW_FORMAT, 1);
		/// <summary>
		/// Narrow text for stand-alone use, typically a single letter.
		/// For example, day-of-week Monday might output "M".
		/// </summary>
		public static readonly TextStyle NARROW_STANDALONE = new TextStyle("NARROW_STANDALONE", InnerEnum.NARROW_STANDALONE, java.util.Calendar.NARROW_STANDALONE, 1);

		private static readonly IList<TextStyle> valueList = new List<TextStyle>();

		static TextStyle()
		{
			valueList.Add(FULL);
			valueList.Add(FULL_STANDALONE);
			valueList.Add(SHORT);
			valueList.Add(SHORT_STANDALONE);
			valueList.Add(NARROW);
			valueList.Add(NARROW_STANDALONE);
		}

		public enum InnerEnum
		{
			FULL,
			FULL_STANDALONE,
			SHORT,
			SHORT_STANDALONE,
			NARROW,
			NARROW_STANDALONE
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		private readonly int calendarStyle;
		private readonly int zoneNameStyleIndex;

		private TextStyle(string name, InnerEnum innerEnum, int calendarStyle, int zoneNameStyleIndex)
		{
			this.calendarStyle = calendarStyle;
			this.zoneNameStyleIndex = zoneNameStyleIndex;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Returns true if the Style is a stand-alone style. </summary>
		/// <returns> true if the style is a stand-alone style. </returns>
		public boolean Standalone
		{
			get
			{
				return (ordinal() & 1) == 1;
			}
		}

		/// <summary>
		/// Returns the stand-alone style with the same size. </summary>
		/// <returns> the stand-alone style with the same size </returns>
		public TextStyle AsStandalone()
		{
			return TextStyle.values()[ordinal() | 1];
		}

		/// <summary>
		/// Returns the normal style with the same size.
		/// </summary>
		/// <returns> the normal style with the same size </returns>
		public TextStyle AsNormal()
		{
			return TextStyle.values()[ordinal() & ~1];
		}

		/// <summary>
		/// Returns the {@code Calendar} style corresponding to this {@code TextStyle}.
		/// </summary>
		/// <returns> the corresponding {@code Calendar} style </returns>
		internal int ToCalendarStyle()
		{
			return calendarStyle;
		}

		/// <summary>
		/// Returns the relative index value to an element of the {@link
		/// java.text.DateFormatSymbols#getZoneStrings() DateFormatSymbols.getZoneStrings()}
		/// value, 0 for long names and 1 for short names (abbreviations). Note that these values
		/// do <em>not</em> correspond to the <seealso cref="java.util.TimeZone#LONG"/> and {@link
		/// java.util.TimeZone#SHORT} values.
		/// </summary>
		/// <returns> the relative index value to time zone names array </returns>
		internal int ZoneNameStyleIndex()
		{
			return zoneNameStyleIndex;
		}

		public static IList<TextStyle> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TextStyle valueOf(string name)
		{
			foreach (TextStyle enumInstance in TextStyle.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}