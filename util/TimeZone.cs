using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using GetPropertyAction = sun.security.action.GetPropertyAction;
	using ZoneInfo = sun.util.calendar.ZoneInfo;
	using ZoneInfoFile = sun.util.calendar.ZoneInfoFile;
	using TimeZoneNameUtility = sun.util.locale.provider.TimeZoneNameUtility;

	/// <summary>
	/// <code>TimeZone</code> represents a time zone offset, and also figures out daylight
	/// savings.
	/// 
	/// <para>
	/// Typically, you get a <code>TimeZone</code> using <code>getDefault</code>
	/// which creates a <code>TimeZone</code> based on the time zone where the program
	/// is running. For example, for a program running in Japan, <code>getDefault</code>
	/// creates a <code>TimeZone</code> object based on Japanese Standard Time.
	/// 
	/// </para>
	/// <para>
	/// You can also get a <code>TimeZone</code> using <code>getTimeZone</code>
	/// along with a time zone ID. For instance, the time zone ID for the
	/// U.S. Pacific Time zone is "America/Los_Angeles". So, you can get a
	/// U.S. Pacific Time <code>TimeZone</code> object with:
	/// <blockquote><pre>
	/// TimeZone tz = TimeZone.getTimeZone("America/Los_Angeles");
	/// </pre></blockquote>
	/// You can use the <code>getAvailableIDs</code> method to iterate through
	/// all the supported time zone IDs. You can then choose a
	/// supported ID to get a <code>TimeZone</code>.
	/// If the time zone you want is not represented by one of the
	/// supported IDs, then a custom time zone ID can be specified to
	/// produce a TimeZone. The syntax of a custom time zone ID is:
	/// 
	/// <blockquote><pre>
	/// <a name="CustomID"><i>CustomID:</i></a>
	///         <code>GMT</code> <i>Sign</i> <i>Hours</i> <code>:</code> <i>Minutes</i>
	///         <code>GMT</code> <i>Sign</i> <i>Hours</i> <i>Minutes</i>
	///         <code>GMT</code> <i>Sign</i> <i>Hours</i>
	/// <i>Sign:</i> one of
	///         <code>+ -</code>
	/// <i>Hours:</i>
	///         <i>Digit</i>
	///         <i>Digit</i> <i>Digit</i>
	/// <i>Minutes:</i>
	///         <i>Digit</i> <i>Digit</i>
	/// <i>Digit:</i> one of
	///         <code>0 1 2 3 4 5 6 7 8 9</code>
	/// </pre></blockquote>
	/// 
	/// <i>Hours</i> must be between 0 to 23 and <i>Minutes</i> must be
	/// between 00 to 59.  For example, "GMT+10" and "GMT+0010" mean ten
	/// hours and ten minutes ahead of GMT, respectively.
	/// </para>
	/// <para>
	/// The format is locale independent and digits must be taken from the
	/// Basic Latin block of the Unicode standard. No daylight saving time
	/// transition schedule can be specified with a custom time zone ID. If
	/// the specified string doesn't match the syntax, <code>"GMT"</code>
	/// is used.
	/// </para>
	/// <para>
	/// When creating a <code>TimeZone</code>, the specified custom time
	/// zone ID is normalized in the following syntax:
	/// <blockquote><pre>
	/// <a name="NormalizedCustomID"><i>NormalizedCustomID:</i></a>
	///         <code>GMT</code> <i>Sign</i> <i>TwoDigitHours</i> <code>:</code> <i>Minutes</i>
	/// <i>Sign:</i> one of
	///         <code>+ -</code>
	/// <i>TwoDigitHours:</i>
	///         <i>Digit</i> <i>Digit</i>
	/// <i>Minutes:</i>
	///         <i>Digit</i> <i>Digit</i>
	/// <i>Digit:</i> one of
	///         <code>0 1 2 3 4 5 6 7 8 9</code>
	/// </pre></blockquote>
	/// For example, TimeZone.getTimeZone("GMT-8").getID() returns "GMT-08:00".
	/// 
	/// <h3>Three-letter time zone IDs</h3>
	/// 
	/// For compatibility with JDK 1.1.x, some other three-letter time zone IDs
	/// (such as "PST", "CTT", "AST") are also supported. However, <strong>their
	/// use is deprecated</strong> because the same abbreviation is often used
	/// for multiple time zones (for example, "CST" could be U.S. "Central Standard
	/// Time" and "China Standard Time"), and the Java platform can then only
	/// recognize one of them.
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          Calendar </seealso>
	/// <seealso cref=          GregorianCalendar </seealso>
	/// <seealso cref=          SimpleTimeZone
	/// @author       Mark Davis, David Goldsmith, Chen-Lieh Huang, Alan Liu
	/// @since        JDK1.1 </seealso>
	[Serializable]
	public abstract class TimeZone : Cloneable
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		public TimeZone()
		{
		}

		/// <summary>
		/// A style specifier for <code>getDisplayName()</code> indicating
		/// a short name, such as "PST." </summary>
		/// <seealso cref= #LONG
		/// @since 1.2 </seealso>
		public const int SHORT = 0;

		/// <summary>
		/// A style specifier for <code>getDisplayName()</code> indicating
		/// a long name, such as "Pacific Standard Time." </summary>
		/// <seealso cref= #SHORT
		/// @since 1.2 </seealso>
		public const int LONG = 1;

		// Constants used internally; unit is milliseconds
		private const int ONE_MINUTE = 60 * 1000;
		private static readonly int ONE_HOUR = 60 * ONE_MINUTE;
		private static readonly int ONE_DAY = 24 * ONE_HOUR;

		// Proclaim serialization compatibility with JDK 1.1
		internal const long SerialVersionUID = 3581463369166924961L;

		/// <summary>
		/// Gets the time zone offset, for current date, modified in case of
		/// daylight savings. This is the offset to add to UTC to get local time.
		/// <para>
		/// This method returns a historically correct offset if an
		/// underlying <code>TimeZone</code> implementation subclass
		/// supports historical Daylight Saving Time schedule and GMT
		/// offset changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="era"> the era of the given date. </param>
		/// <param name="year"> the year in the given date. </param>
		/// <param name="month"> the month in the given date.
		/// Month is 0-based. e.g., 0 for January. </param>
		/// <param name="day"> the day-in-month of the given date. </param>
		/// <param name="dayOfWeek"> the day-of-week of the given date. </param>
		/// <param name="milliseconds"> the milliseconds in day in <em>standard</em>
		/// local time.
		/// </param>
		/// <returns> the offset in milliseconds to add to GMT to get local time.
		/// </returns>
		/// <seealso cref= Calendar#ZONE_OFFSET </seealso>
		/// <seealso cref= Calendar#DST_OFFSET </seealso>
		public abstract int GetOffset(int era, int year, int month, int day, int dayOfWeek, int milliseconds);

		/// <summary>
		/// Returns the offset of this time zone from UTC at the specified
		/// date. If Daylight Saving Time is in effect at the specified
		/// date, the offset value is adjusted with the amount of daylight
		/// saving.
		/// <para>
		/// This method returns a historically correct offset value if an
		/// underlying TimeZone implementation subclass supports historical
		/// Daylight Saving Time schedule and GMT offset changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date"> the date represented in milliseconds since January 1, 1970 00:00:00 GMT </param>
		/// <returns> the amount of time in milliseconds to add to UTC to get local time.
		/// </returns>
		/// <seealso cref= Calendar#ZONE_OFFSET </seealso>
		/// <seealso cref= Calendar#DST_OFFSET
		/// @since 1.4 </seealso>
		public virtual int GetOffset(long date)
		{
			if (InDaylightTime(new Date(date)))
			{
				return RawOffset + DSTSavings;
			}
			return RawOffset;
		}

		/// <summary>
		/// Gets the raw GMT offset and the amount of daylight saving of this
		/// time zone at the given time. </summary>
		/// <param name="date"> the milliseconds (since January 1, 1970,
		/// 00:00:00.000 GMT) at which the time zone offset and daylight
		/// saving amount are found </param>
		/// <param name="offsets"> an array of int where the raw GMT offset
		/// (offset[0]) and daylight saving amount (offset[1]) are stored,
		/// or null if those values are not needed. The method assumes that
		/// the length of the given array is two or larger. </param>
		/// <returns> the total amount of the raw GMT offset and daylight
		/// saving at the specified date.
		/// </returns>
		/// <seealso cref= Calendar#ZONE_OFFSET </seealso>
		/// <seealso cref= Calendar#DST_OFFSET </seealso>
		internal virtual int GetOffsets(long date, int[] offsets)
		{
			int rawoffset = RawOffset;
			int dstoffset = 0;
			if (InDaylightTime(new Date(date)))
			{
				dstoffset = DSTSavings;
			}
			if (offsets != null)
			{
				offsets[0] = rawoffset;
				offsets[1] = dstoffset;
			}
			return rawoffset + dstoffset;
		}

		/// <summary>
		/// Sets the base time zone offset to GMT.
		/// This is the offset to add to UTC to get local time.
		/// <para>
		/// If an underlying <code>TimeZone</code> implementation subclass
		/// supports historical GMT offset changes, the specified GMT
		/// offset is set as the latest GMT offset and the difference from
		/// the known latest GMT offset value is used to adjust all
		/// historical GMT offset values.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offsetMillis"> the given base time zone offset to GMT. </param>
		public abstract int RawOffset {set;get;}


		/// <summary>
		/// Gets the ID of this time zone. </summary>
		/// <returns> the ID of this time zone. </returns>
		public virtual String ID
		{
			get
			{
				return ID_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
				this.ID_Renamed = value;
			}
		}


		/// <summary>
		/// Returns a long standard time name of this {@code TimeZone} suitable for
		/// presentation to the user in the default locale.
		/// 
		/// <para>This method is equivalent to:
		/// <blockquote><pre>
		/// getDisplayName(false, <seealso cref="#LONG"/>,
		///                Locale.getDefault(<seealso cref="Locale.Category#DISPLAY"/>))
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the human-readable name of this time zone in the default locale.
		/// @since 1.2 </returns>
		/// <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		/// <seealso cref= Locale#getDefault(Locale.Category) </seealso>
		/// <seealso cref= Locale.Category </seealso>
		public String DisplayName
		{
			get
			{
				return GetDisplayName(false, LONG, Locale.GetDefault(Locale.Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a long standard time name of this {@code TimeZone} suitable for
		/// presentation to the user in the specified {@code locale}.
		/// 
		/// <para>This method is equivalent to:
		/// <blockquote><pre>
		/// getDisplayName(false, <seealso cref="#LONG"/>, locale)
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale"> the locale in which to supply the display name. </param>
		/// <returns> the human-readable name of this time zone in the given locale. </returns>
		/// <exception cref="NullPointerException"> if {@code locale} is {@code null}.
		/// @since 1.2 </exception>
		/// <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		public String GetDisplayName(Locale locale)
		{
			return GetDisplayName(false, LONG, locale);
		}

		/// <summary>
		/// Returns a name in the specified {@code style} of this {@code TimeZone}
		/// suitable for presentation to the user in the default locale. If the
		/// specified {@code daylight} is {@code true}, a Daylight Saving Time name
		/// is returned (even if this {@code TimeZone} doesn't observe Daylight Saving
		/// Time). Otherwise, a Standard Time name is returned.
		/// 
		/// <para>This method is equivalent to:
		/// <blockquote><pre>
		/// getDisplayName(daylight, style,
		///                Locale.getDefault(<seealso cref="Locale.Category#DISPLAY"/>))
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="daylight"> {@code true} specifying a Daylight Saving Time name, or
		///                 {@code false} specifying a Standard Time name </param>
		/// <param name="style"> either <seealso cref="#LONG"/> or <seealso cref="#SHORT"/> </param>
		/// <returns> the human-readable name of this time zone in the default locale. </returns>
		/// <exception cref="IllegalArgumentException"> if {@code style} is invalid.
		/// @since 1.2 </exception>
		/// <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		/// <seealso cref= Locale#getDefault(Locale.Category) </seealso>
		/// <seealso cref= Locale.Category </seealso>
		/// <seealso cref= java.text.DateFormatSymbols#getZoneStrings() </seealso>
		public String GetDisplayName(bool daylight, int style)
		{
			return GetDisplayName(daylight, style, Locale.GetDefault(Locale.Category.DISPLAY));
		}

		/// <summary>
		/// Returns a name in the specified {@code style} of this {@code TimeZone}
		/// suitable for presentation to the user in the specified {@code
		/// locale}. If the specified {@code daylight} is {@code true}, a Daylight
		/// Saving Time name is returned (even if this {@code TimeZone} doesn't
		/// observe Daylight Saving Time). Otherwise, a Standard Time name is
		/// returned.
		/// 
		/// <para>When looking up a time zone name, the {@linkplain
		/// ResourceBundle.Control#getCandidateLocales(String,Locale) default
		/// <code>Locale</code> search path of <code>ResourceBundle</code>} derived
		/// from the specified {@code locale} is used. (No {@linkplain
		/// ResourceBundle.Control#getFallbackLocale(String,Locale) fallback
		/// <code>Locale</code>} search is performed.) If a time zone name in any
		/// {@code Locale} of the search path, including <seealso cref="Locale#ROOT"/>, is
		/// found, the name is returned. Otherwise, a string in the
		/// <a href="#NormalizedCustomID">normalized custom ID format</a> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daylight"> {@code true} specifying a Daylight Saving Time name, or
		///                 {@code false} specifying a Standard Time name </param>
		/// <param name="style"> either <seealso cref="#LONG"/> or <seealso cref="#SHORT"/> </param>
		/// <param name="locale">   the locale in which to supply the display name. </param>
		/// <returns> the human-readable name of this time zone in the given locale. </returns>
		/// <exception cref="IllegalArgumentException"> if {@code style} is invalid. </exception>
		/// <exception cref="NullPointerException"> if {@code locale} is {@code null}.
		/// @since 1.2 </exception>
		/// <seealso cref= java.text.DateFormatSymbols#getZoneStrings() </seealso>
		public virtual String GetDisplayName(bool daylight, int style, Locale locale)
		{
			if (style != SHORT && style != LONG)
			{
				throw new IllegalArgumentException("Illegal style: " + style);
			}
			String id = ID;
			String name = TimeZoneNameUtility.retrieveDisplayName(id, daylight, style, locale);
			if (name != null)
			{
				return name;
			}

			if (id.StartsWith("GMT") && id.Length() > 3)
			{
				char sign = id.CharAt(3);
				if (sign == '+' || sign == '-')
				{
					return id;
				}
			}
			int offset = RawOffset;
			if (daylight)
			{
				offset += DSTSavings;
			}
			return ZoneInfoFile.toCustomID(offset);
		}

		private static String[] GetDisplayNames(String id, Locale locale)
		{
			return TimeZoneNameUtility.retrieveDisplayNames(id, locale);
		}

		/// <summary>
		/// Returns the amount of time to be added to local standard time
		/// to get local wall clock time.
		/// 
		/// <para>The default implementation returns 3600000 milliseconds
		/// (i.e., one hour) if a call to <seealso cref="#useDaylightTime()"/>
		/// returns {@code true}. Otherwise, 0 (zero) is returned.
		/// 
		/// </para>
		/// <para>If an underlying {@code TimeZone} implementation subclass
		/// supports historical and future Daylight Saving Time schedule
		/// changes, this method returns the amount of saving time of the
		/// last known Daylight Saving Time rule that can be a future
		/// prediction.
		/// 
		/// </para>
		/// <para>If the amount of saving time at any given time stamp is
		/// required, construct a <seealso cref="Calendar"/> with this {@code
		/// TimeZone} and the time stamp, and call {@link Calendar#get(int)
		/// Calendar.get}{@code (}<seealso cref="Calendar#DST_OFFSET"/>{@code )}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the amount of saving time in milliseconds
		/// @since 1.4 </returns>
		/// <seealso cref= #inDaylightTime(Date) </seealso>
		/// <seealso cref= #getOffset(long) </seealso>
		/// <seealso cref= #getOffset(int,int,int,int,int,int) </seealso>
		/// <seealso cref= Calendar#ZONE_OFFSET </seealso>
		public virtual int DSTSavings
		{
			get
			{
				if (UseDaylightTime())
				{
					return 3600000;
				}
				return 0;
			}
		}

		/// <summary>
		/// Queries if this {@code TimeZone} uses Daylight Saving Time.
		/// 
		/// <para>If an underlying {@code TimeZone} implementation subclass
		/// supports historical and future Daylight Saving Time schedule
		/// changes, this method refers to the last known Daylight Saving Time
		/// rule that can be a future prediction and may not be the same as
		/// the current rule. Consider calling <seealso cref="#observesDaylightTime()"/>
		/// if the current rule should also be taken into account.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if this {@code TimeZone} uses Daylight Saving Time,
		///         {@code false}, otherwise. </returns>
		/// <seealso cref= #inDaylightTime(Date) </seealso>
		/// <seealso cref= Calendar#DST_OFFSET </seealso>
		public abstract bool UseDaylightTime();

		/// <summary>
		/// Returns {@code true} if this {@code TimeZone} is currently in
		/// Daylight Saving Time, or if a transition from Standard Time to
		/// Daylight Saving Time occurs at any future time.
		/// 
		/// <para>The default implementation returns {@code true} if
		/// {@code useDaylightTime()} or {@code inDaylightTime(new Date())}
		/// returns {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if this {@code TimeZone} is currently in
		/// Daylight Saving Time, or if a transition from Standard Time to
		/// Daylight Saving Time occurs at any future time; {@code false}
		/// otherwise.
		/// @since 1.7 </returns>
		/// <seealso cref= #useDaylightTime() </seealso>
		/// <seealso cref= #inDaylightTime(Date) </seealso>
		/// <seealso cref= Calendar#DST_OFFSET </seealso>
		public virtual bool ObservesDaylightTime()
		{
			return UseDaylightTime() || InDaylightTime(DateTime.Now);
		}

		/// <summary>
		/// Queries if the given {@code date} is in Daylight Saving Time in
		/// this time zone.
		/// </summary>
		/// <param name="date"> the given Date. </param>
		/// <returns> {@code true} if the given date is in Daylight Saving Time,
		///         {@code false}, otherwise. </returns>
		public abstract bool InDaylightTime(Date date);

		/// <summary>
		/// Gets the <code>TimeZone</code> for the given ID.
		/// </summary>
		/// <param name="ID"> the ID for a <code>TimeZone</code>, either an abbreviation
		/// such as "PST", a full name such as "America/Los_Angeles", or a custom
		/// ID such as "GMT-8:00". Note that the support of abbreviations is
		/// for JDK 1.1.x compatibility only and full names should be used.
		/// </param>
		/// <returns> the specified <code>TimeZone</code>, or the GMT zone if the given ID
		/// cannot be understood. </returns>
		public static TimeZone GetTimeZone(String ID)
		{
			lock (typeof(TimeZone))
			{
				return GetTimeZone(ID, true);
			}
		}

		/// <summary>
		/// Gets the {@code TimeZone} for the given {@code zoneId}.
		/// </summary>
		/// <param name="zoneId"> a <seealso cref="ZoneId"/> from which the time zone ID is obtained </param>
		/// <returns> the specified {@code TimeZone}, or the GMT zone if the given ID
		///         cannot be understood. </returns>
		/// <exception cref="NullPointerException"> if {@code zoneId} is {@code null}
		/// @since 1.8 </exception>
		public static TimeZone GetTimeZone(ZoneId zoneId)
		{
			String tzid = zoneId.Id; // throws an NPE if null
			char c = tzid.CharAt(0);
			if (c == '+' || c == '-')
			{
				tzid = "GMT" + tzid;
			}
			else if (c == 'Z' && tzid.Length() == 1)
			{
				tzid = "UTC";
			}
			return GetTimeZone(tzid, true);
		}

		/// <summary>
		/// Converts this {@code TimeZone} object to a {@code ZoneId}.
		/// </summary>
		/// <returns> a {@code ZoneId} representing the same time zone as this
		///         {@code TimeZone}
		/// @since 1.8 </returns>
		public virtual ZoneId ToZoneId()
		{
			String id = ID;
			if (ZoneInfoFile.useOldMapping() && id.Length() == 3)
			{
				if ("EST".Equals(id))
				{
					return ZoneId.Of("America/New_York");
				}
				if ("MST".Equals(id))
				{
					return ZoneId.Of("America/Denver");
				}
				if ("HST".Equals(id))
				{
					return ZoneId.Of("America/Honolulu");
				}
			}
			return ZoneId.Of(id, ZoneId.SHORT_IDS);
		}

		private static TimeZone GetTimeZone(String ID, bool fallback)
		{
			TimeZone tz = ZoneInfo.getTimeZone(ID);
			if (tz == null)
			{
				tz = ParseCustomTimeZone(ID);
				if (tz == null && fallback)
				{
					tz = new ZoneInfo(GMT_ID, 0);
				}
			}
			return tz;
		}

		/// <summary>
		/// Gets the available IDs according to the given time zone offset in milliseconds.
		/// </summary>
		/// <param name="rawOffset"> the given time zone GMT offset in milliseconds. </param>
		/// <returns> an array of IDs, where the time zone for that ID has
		/// the specified GMT offset. For example, "America/Phoenix" and "America/Denver"
		/// both have GMT-07:00, but differ in daylight saving behavior. </returns>
		/// <seealso cref= #getRawOffset() </seealso>
		public static String[] GetAvailableIDs(int rawOffset)
		{
			lock (typeof(TimeZone))
			{
				return ZoneInfo.getAvailableIDs(rawOffset);
			}
		}

		/// <summary>
		/// Gets all the available IDs supported. </summary>
		/// <returns> an array of IDs. </returns>
		public static String[] AvailableIDs
		{
			get
			{
				lock (typeof(TimeZone))
				{
					return ZoneInfo.AvailableIDs;
				}
			}
		}

		/// <summary>
		/// Gets the platform defined TimeZone ID.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String getSystemTimeZoneID(String javaHome);

		/// <summary>
		/// Gets the custom time zone ID based on the GMT offset of the
		/// platform. (e.g., "GMT+08:00")
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String getSystemGMTOffsetID();

		/// <summary>
		/// Gets the default {@code TimeZone} of the Java virtual machine. If the
		/// cached default {@code TimeZone} is available, its clone is returned.
		/// Otherwise, the method takes the following steps to determine the default
		/// time zone.
		/// 
		/// <ul>
		/// <li>Use the {@code user.timezone} property value as the default
		/// time zone ID if it's available.</li>
		/// <li>Detect the platform time zone ID. The source of the
		/// platform time zone and ID mapping may vary with implementation.</li>
		/// <li>Use {@code GMT} as the last resort if the given or detected
		/// time zone ID is unknown.</li>
		/// </ul>
		/// 
		/// <para>The default {@code TimeZone} created from the ID is cached,
		/// and its clone is returned. The {@code user.timezone} property
		/// value is set to the ID upon return.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the default {@code TimeZone} </returns>
		/// <seealso cref= #setDefault(TimeZone) </seealso>
		public static TimeZone Default
		{
			get
			{
				return (TimeZone) DefaultRef.Clone();
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new PropertyPermission("user.timezone", "write"));
				}
				DefaultTimeZone = value;
			}
		}

		/// <summary>
		/// Returns the reference to the default TimeZone object. This
		/// method doesn't create a clone.
		/// </summary>
		internal static TimeZone DefaultRef
		{
			get
			{
				TimeZone defaultZone = DefaultTimeZone;
				if (defaultZone == null)
				{
					// Need to initialize the default time zone.
					defaultZone = SetDefaultZone();
					Debug.Assert(defaultZone != null);
				}
				// Don't clone here.
				return defaultZone;
			}
		}

		private static TimeZone SetDefaultZone()
		{
			lock (typeof(TimeZone))
			{
				TimeZone tz;
				// get the time zone ID from the system properties
				String zoneID = AccessController.doPrivileged(new GetPropertyAction("user.timezone"));
        
				// if the time zone ID is not set (yet), perform the
				// platform to Java time zone ID mapping.
				if (zoneID == null || zoneID.Empty)
				{
					String javaHome = AccessController.doPrivileged(new GetPropertyAction("java.home"));
					try
					{
						zoneID = getSystemTimeZoneID(javaHome);
						if (zoneID == null)
						{
							zoneID = GMT_ID;
						}
					}
					catch (NullPointerException)
					{
						zoneID = GMT_ID;
					}
				}
        
				// Get the time zone for zoneID. But not fall back to
				// "GMT" here.
				tz = GetTimeZone(zoneID, false);
        
				if (tz == null)
				{
					// If the given zone ID is unknown in Java, try to
					// get the GMT-offset-based time zone ID,
					// a.k.a. custom time zone ID (e.g., "GMT-08:00").
					String gmtOffsetID = SystemGMTOffsetID;
					if (gmtOffsetID != null)
					{
						zoneID = gmtOffsetID;
					}
					tz = GetTimeZone(zoneID, true);
				}
				Debug.Assert(tz != null);
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String id = zoneID;
				String id = zoneID;
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(id));
        
				DefaultTimeZone = tz;
				return tz;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private string Id;

			public PrivilegedActionAnonymousInnerClassHelper(string id)
			{
				this.Id = id;
			}

			public virtual Void Run()
			{
					System.setProperty("user.timezone", Id);
					return null;
			}
		}


		/// <summary>
		/// Returns true if this zone has the same rule and offset as another zone.
		/// That is, if this zone differs only in ID, if at all.  Returns false
		/// if the other zone is null. </summary>
		/// <param name="other"> the <code>TimeZone</code> object to be compared with </param>
		/// <returns> true if the other zone is not null and is the same as this one,
		/// with the possible exception of the ID
		/// @since 1.2 </returns>
		public virtual bool HasSameRules(TimeZone other)
		{
			return other != null && RawOffset == other.RawOffset && UseDaylightTime() == other.UseDaylightTime();
		}

		/// <summary>
		/// Creates a copy of this <code>TimeZone</code>.
		/// </summary>
		/// <returns> a clone of this <code>TimeZone</code> </returns>
		public virtual Object Clone()
		{
			try
			{
				TimeZone other = (TimeZone) base.Clone();
				other.ID_Renamed = ID_Renamed;
				return other;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// The null constant as a TimeZone.
		/// </summary>
		internal const TimeZone NO_TIMEZONE = null;

		// =======================privates===============================

		/// <summary>
		/// The string identifier of this <code>TimeZone</code>.  This is a
		/// programmatic identifier used internally to look up <code>TimeZone</code>
		/// objects from the system table and also to map them to their localized
		/// display names.  <code>ID</code> values are unique in the system
		/// table but may not be for dynamically created zones.
		/// @serial
		/// </summary>
		private String ID_Renamed;
		private static volatile TimeZone DefaultTimeZone;

		internal const String GMT_ID = "GMT";
		private const int GMT_ID_LENGTH = 3;

		// a static TimeZone we can reference if no AppContext is in place
		private static volatile TimeZone MainAppContextDefault;

		/// <summary>
		/// Parses a custom time zone identifier and returns a corresponding zone.
		/// This method doesn't support the RFC 822 time zone format. (e.g., +hhmm)
		/// </summary>
		/// <param name="id"> a string of the <a href="#CustomID">custom ID form</a>. </param>
		/// <returns> a newly created TimeZone with the given offset and
		/// no daylight saving time, or null if the id cannot be parsed. </returns>
		private static TimeZone ParseCustomTimeZone(String id)
		{
			int length;

			// Error if the length of id isn't long enough or id doesn't
			// start with "GMT".
			if ((length = id.Length()) < (GMT_ID_LENGTH + 2) || id.IndexOf(GMT_ID) != 0)
			{
				return null;
			}

			ZoneInfo zi;

			// First, we try to find it in the cache with the given
			// id. Even the id is not normalized, the returned ZoneInfo
			// should have its normalized id.
			zi = ZoneInfoFile.getZoneInfo(id);
			if (zi != null)
			{
				return zi;
			}

			int index = GMT_ID_LENGTH;
			bool negative = false;
			char c = id.CharAt(index++);
			if (c == '-')
			{
				negative = true;
			}
			else if (c != '+')
			{
				return null;
			}

			int hours = 0;
			int num = 0;
			int countDelim = 0;
			int len = 0;
			while (index < length)
			{
				c = id.CharAt(index++);
				if (c == ':')
				{
					if (countDelim > 0)
					{
						return null;
					}
					if (len > 2)
					{
						return null;
					}
					hours = num;
					countDelim++;
					num = 0;
					len = 0;
					continue;
				}
				if (c < '0' || c > '9')
				{
					return null;
				}
				num = num * 10 + (c - '0');
				len++;
			}
			if (index != length)
			{
				return null;
			}
			if (countDelim == 0)
			{
				if (len <= 2)
				{
					hours = num;
					num = 0;
				}
				else
				{
					hours = num / 100;
					num %= 100;
				}
			}
			else
			{
				if (len != 2)
				{
					return null;
				}
			}
			if (hours > 23 || num > 59)
			{
				return null;
			}
			int gmtOffset = (hours * 60 + num) * 60 * 1000;

			if (gmtOffset == 0)
			{
				zi = ZoneInfoFile.getZoneInfo(GMT_ID);
				if (negative)
				{
					zi.ID = "GMT-00:00";
				}
				else
				{
					zi.ID = "GMT+00:00";
				}
			}
			else
			{
				zi = ZoneInfoFile.getCustomTimeZone(id, negative ? - gmtOffset : gmtOffset);
			}
			return zi;
		}
	}

}