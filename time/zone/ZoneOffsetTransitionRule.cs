using System;
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
 * Copyright (c) 2009-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.zone
{



	/// <summary>
	/// A rule expressing how to create a transition.
	/// <para>
	/// This class allows rules for identifying future transitions to be expressed.
	/// A rule might be written in many forms:
	/// <ul>
	/// <li>the 16th March
	/// <li>the Sunday on or after the 16th March
	/// <li>the Sunday on or before the 16th March
	/// <li>the last Sunday in February
	/// </ul>
	/// These different rule types can be expressed and queried.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ZoneOffsetTransitionRule
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 6889046316657758795L;

		/// <summary>
		/// The month of the month-day of the first day of the cutover week.
		/// The actual date will be adjusted by the dowChange field.
		/// </summary>
		private readonly Month Month_Renamed;
		/// <summary>
		/// The day-of-month of the month-day of the cutover week.
		/// If positive, it is the start of the week where the cutover can occur.
		/// If negative, it represents the end of the week where cutover can occur.
		/// The value is the number of days from the end of the month, such that
		/// {@code -1} is the last day of the month, {@code -2} is the second
		/// to last day, and so on.
		/// </summary>
		private readonly sbyte Dom;
		/// <summary>
		/// The cutover day-of-week, null to retain the day-of-month.
		/// </summary>
		private readonly DayOfWeek Dow;
		/// <summary>
		/// The cutover time in the 'before' offset.
		/// </summary>
		private readonly LocalTime Time;
		/// <summary>
		/// Whether the cutover time is midnight at the end of day.
		/// </summary>
		private readonly bool TimeEndOfDay;
		/// <summary>
		/// The definition of how the local time should be interpreted.
		/// </summary>
		private readonly TimeDefinition TimeDefinition_Renamed;
		/// <summary>
		/// The standard offset at the cutover.
		/// </summary>
		private readonly ZoneOffset StandardOffset_Renamed;
		/// <summary>
		/// The offset before the cutover.
		/// </summary>
		private readonly ZoneOffset OffsetBefore_Renamed;
		/// <summary>
		/// The offset after the cutover.
		/// </summary>
		private readonly ZoneOffset OffsetAfter_Renamed;

		/// <summary>
		/// Obtains an instance defining the yearly rule to create transitions between two offsets.
		/// <para>
		/// Applications should normally obtain an instance from <seealso cref="ZoneRules"/>.
		/// This factory is only intended for use when creating <seealso cref="ZoneRules"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="month">  the month of the month-day of the first day of the cutover week, not null </param>
		/// <param name="dayOfMonthIndicator">  the day of the month-day of the cutover week, positive if the week is that
		///  day or later, negative if the week is that day or earlier, counting from the last day of the month,
		///  from -28 to 31 excluding 0 </param>
		/// <param name="dayOfWeek">  the required day-of-week, null if the month-day should not be changed </param>
		/// <param name="time">  the cutover time in the 'before' offset, not null </param>
		/// <param name="timeEndOfDay">  whether the time is midnight at the end of day </param>
		/// <param name="timeDefnition">  how to interpret the cutover </param>
		/// <param name="standardOffset">  the standard offset in force at the cutover, not null </param>
		/// <param name="offsetBefore">  the offset before the cutover, not null </param>
		/// <param name="offsetAfter">  the offset after the cutover, not null </param>
		/// <returns> the rule, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the day of month indicator is invalid </exception>
		/// <exception cref="IllegalArgumentException"> if the end of day flag is true when the time is not midnight </exception>
		public static ZoneOffsetTransitionRule Of(Month month, int dayOfMonthIndicator, DayOfWeek dayOfWeek, LocalTime time, bool timeEndOfDay, TimeDefinition timeDefnition, ZoneOffset standardOffset, ZoneOffset offsetBefore, ZoneOffset offsetAfter)
		{
			Objects.RequireNonNull(month, "month");
			Objects.RequireNonNull(time, "time");
			Objects.RequireNonNull(timeDefnition, "timeDefnition");
			Objects.RequireNonNull(standardOffset, "standardOffset");
			Objects.RequireNonNull(offsetBefore, "offsetBefore");
			Objects.RequireNonNull(offsetAfter, "offsetAfter");
			if (dayOfMonthIndicator < -28 || dayOfMonthIndicator > 31 || dayOfMonthIndicator == 0)
			{
				throw new IllegalArgumentException("Day of month indicator must be between -28 and 31 inclusive excluding zero");
			}
			if (timeEndOfDay && time.Equals(LocalTime.MIDNIGHT) == false)
			{
				throw new IllegalArgumentException("Time must be midnight when end of day flag is true");
			}
			return new ZoneOffsetTransitionRule(month, dayOfMonthIndicator, dayOfWeek, time, timeEndOfDay, timeDefnition, standardOffset, offsetBefore, offsetAfter);
		}

		/// <summary>
		/// Creates an instance defining the yearly rule to create transitions between two offsets.
		/// </summary>
		/// <param name="month">  the month of the month-day of the first day of the cutover week, not null </param>
		/// <param name="dayOfMonthIndicator">  the day of the month-day of the cutover week, positive if the week is that
		///  day or later, negative if the week is that day or earlier, counting from the last day of the month,
		///  from -28 to 31 excluding 0 </param>
		/// <param name="dayOfWeek">  the required day-of-week, null if the month-day should not be changed </param>
		/// <param name="time">  the cutover time in the 'before' offset, not null </param>
		/// <param name="timeEndOfDay">  whether the time is midnight at the end of day </param>
		/// <param name="timeDefnition">  how to interpret the cutover </param>
		/// <param name="standardOffset">  the standard offset in force at the cutover, not null </param>
		/// <param name="offsetBefore">  the offset before the cutover, not null </param>
		/// <param name="offsetAfter">  the offset after the cutover, not null </param>
		/// <exception cref="IllegalArgumentException"> if the day of month indicator is invalid </exception>
		/// <exception cref="IllegalArgumentException"> if the end of day flag is true when the time is not midnight </exception>
		internal ZoneOffsetTransitionRule(Month month, int dayOfMonthIndicator, DayOfWeek dayOfWeek, LocalTime time, bool timeEndOfDay, TimeDefinition timeDefnition, ZoneOffset standardOffset, ZoneOffset offsetBefore, ZoneOffset offsetAfter)
		{
			this.Month_Renamed = month;
			this.Dom = (sbyte) dayOfMonthIndicator;
			this.Dow = dayOfWeek;
			this.Time = time;
			this.TimeEndOfDay = timeEndOfDay;
			this.TimeDefinition_Renamed = timeDefnition;
			this.StandardOffset_Renamed = standardOffset;
			this.OffsetBefore_Renamed = offsetBefore;
			this.OffsetAfter_Renamed = offsetAfter;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.InvalidObjectException
		private void ReadObject(ObjectInputStream s)
		{
			throw new InvalidObjectException("Deserialization via serialization delegate");
		}

		/// <summary>
		/// Writes the object using a
		/// <a href="../../../serialized-form.html#java.time.zone.Ser">dedicated serialized form</a>.
		/// @serialData
		/// Refer to the serialized form of
		/// <a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules.writeReplace</a>
		/// for the encoding of epoch seconds and offsets.
		/// <pre style="font-size:1.0em">{@code
		/// 
		///      out.writeByte(3);                // identifies a ZoneOffsetTransition
		///      final int timeSecs = (timeEndOfDay ? 86400 : time.toSecondOfDay());
		///      final int stdOffset = standardOffset.getTotalSeconds();
		///      final int beforeDiff = offsetBefore.getTotalSeconds() - stdOffset;
		///      final int afterDiff = offsetAfter.getTotalSeconds() - stdOffset;
		///      final int timeByte = (timeSecs % 3600 == 0 ? (timeEndOfDay ? 24 : time.getHour()) : 31);
		///      final int stdOffsetByte = (stdOffset % 900 == 0 ? stdOffset / 900 + 128 : 255);
		///      final int beforeByte = (beforeDiff == 0 || beforeDiff == 1800 || beforeDiff == 3600 ? beforeDiff / 1800 : 3);
		///      final int afterByte = (afterDiff == 0 || afterDiff == 1800 || afterDiff == 3600 ? afterDiff / 1800 : 3);
		///      final int dowByte = (dow == null ? 0 : dow.getValue());
		///      int b = (month.getValue() << 28) +          // 4 bits
		///              ((dom + 32) << 22) +                // 6 bits
		///              (dowByte << 19) +                   // 3 bits
		///              (timeByte << 14) +                  // 5 bits
		///              (timeDefinition.ordinal() << 12) +  // 2 bits
		///              (stdOffsetByte << 4) +              // 8 bits
		///              (beforeByte << 2) +                 // 2 bits
		///              afterByte;                          // 2 bits
		///      out.writeInt(b);
		///      if (timeByte == 31) {
		///          out.writeInt(timeSecs);
		///      }
		///      if (stdOffsetByte == 255) {
		///          out.writeInt(stdOffset);
		///      }
		///      if (beforeByte == 3) {
		///          out.writeInt(offsetBefore.getTotalSeconds());
		///      }
		///      if (afterByte == 3) {
		///          out.writeInt(offsetAfter.getTotalSeconds());
		///      }
		/// }
		/// </pre>
		/// </summary>
		/// <returns> the replacing object, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZOTRULE, this);
		}

		/// <summary>
		/// Writes the state to the stream.
		/// </summary>
		/// <param name="out">  the output stream, not null </param>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int timeSecs = (timeEndOfDay ? 86400 : time.toSecondOfDay());
			int timeSecs = (TimeEndOfDay ? 86400 : Time.ToSecondOfDay());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stdOffset = standardOffset.getTotalSeconds();
			int stdOffset = StandardOffset_Renamed.TotalSeconds;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int beforeDiff = offsetBefore.getTotalSeconds() - stdOffset;
			int beforeDiff = OffsetBefore_Renamed.TotalSeconds - stdOffset;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int afterDiff = offsetAfter.getTotalSeconds() - stdOffset;
			int afterDiff = OffsetAfter_Renamed.TotalSeconds - stdOffset;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int timeByte = (timeSecs % 3600 == 0 ? (timeEndOfDay ? 24 : time.getHour()) : 31);
			int timeByte = (timeSecs % 3600 == 0 ? (TimeEndOfDay ? 24 : Time.Hour) : 31);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stdOffsetByte = (stdOffset % 900 == 0 ? stdOffset / 900 + 128 : 255);
			int stdOffsetByte = (stdOffset % 900 == 0 ? stdOffset / 900 + 128 : 255);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int beforeByte = (beforeDiff == 0 || beforeDiff == 1800 || beforeDiff == 3600 ? beforeDiff / 1800 : 3);
			int beforeByte = (beforeDiff == 0 || beforeDiff == 1800 || beforeDiff == 3600 ? beforeDiff / 1800 : 3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int afterByte = (afterDiff == 0 || afterDiff == 1800 || afterDiff == 3600 ? afterDiff / 1800 : 3);
			int afterByte = (afterDiff == 0 || afterDiff == 1800 || afterDiff == 3600 ? afterDiff / 1800 : 3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dowByte = (dow == null ? 0 : dow.getValue());
			int dowByte = (Dow == null ? 0 : Dow.Value);
			int b = (Month_Renamed.Value << 28) + ((Dom + 32) << 22) + (dowByte << 19) + (timeByte << 14) + (TimeDefinition_Renamed.ordinal() << 12) + (stdOffsetByte << 4) + (beforeByte << 2) + afterByte; // 2 bits -  2 bits -  8 bits -  2 bits -  5 bits -  3 bits -  6 bits -  4 bits
			@out.WriteInt(b);
			if (timeByte == 31)
			{
				@out.WriteInt(timeSecs);
			}
			if (stdOffsetByte == 255)
			{
				@out.WriteInt(stdOffset);
			}
			if (beforeByte == 3)
			{
				@out.WriteInt(OffsetBefore_Renamed.TotalSeconds);
			}
			if (afterByte == 3)
			{
				@out.WriteInt(OffsetAfter_Renamed.TotalSeconds);
			}
		}

		/// <summary>
		/// Reads the state from the stream.
		/// </summary>
		/// <param name="in">  the input stream, not null </param>
		/// <returns> the created object, not null </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZoneOffsetTransitionRule readExternal(java.io.DataInput in) throws java.io.IOException
		internal static ZoneOffsetTransitionRule ReadExternal(DataInput @in)
		{
			int data = @in.ReadInt();
			Month month = Month.of((int)((uint)data >> 28));
			int dom = ((int)((uint)(data & (63 << 22)) >> 22)) - 32;
			int dowByte = (int)((uint)(data & (7 << 19)) >> 19);
			DayOfWeek dow = dowByte == 0 ? null : DayOfWeek.of(dowByte);
			int timeByte = (int)((uint)(data & (31 << 14)) >> 14);
			TimeDefinition defn = TimeDefinition.values()[(int)((uint)(data & (3 << 12)) >> 12)];
			int stdByte = (int)((uint)(data & (255 << 4)) >> 4);
			int beforeByte = (int)((uint)(data & (3 << 2)) >> 2);
			int afterByte = (data & 3);
			LocalTime time = (timeByte == 31 ? LocalTime.OfSecondOfDay(@in.ReadInt()) : LocalTime.Of(timeByte % 24, 0));
			ZoneOffset std = (stdByte == 255 ? ZoneOffset.OfTotalSeconds(@in.ReadInt()) : ZoneOffset.OfTotalSeconds((stdByte - 128) * 900));
			ZoneOffset before = (beforeByte == 3 ? ZoneOffset.OfTotalSeconds(@in.ReadInt()) : ZoneOffset.OfTotalSeconds(std.TotalSeconds + beforeByte * 1800));
			ZoneOffset after = (afterByte == 3 ? ZoneOffset.OfTotalSeconds(@in.ReadInt()) : ZoneOffset.OfTotalSeconds(std.TotalSeconds + afterByte * 1800));
			return ZoneOffsetTransitionRule.Of(month, dom, dow, time, timeByte == 24, defn, std, before, after);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the month of the transition.
		/// <para>
		/// If the rule defines an exact date then the month is the month of that date.
		/// </para>
		/// <para>
		/// If the rule defines a week where the transition might occur, then the month
		/// if the month of either the earliest or latest possible date of the cutover.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the month of the transition, not null </returns>
		public Month Month
		{
			get
			{
				return Month_Renamed;
			}
		}

		/// <summary>
		/// Gets the indicator of the day-of-month of the transition.
		/// <para>
		/// If the rule defines an exact date then the day is the month of that date.
		/// </para>
		/// <para>
		/// If the rule defines a week where the transition might occur, then the day
		/// defines either the start of the end of the transition week.
		/// </para>
		/// <para>
		/// If the value is positive, then it represents a normal day-of-month, and is the
		/// earliest possible date that the transition can be.
		/// The date may refer to 29th February which should be treated as 1st March in non-leap years.
		/// </para>
		/// <para>
		/// If the value is negative, then it represents the number of days back from the
		/// end of the month where {@code -1} is the last day of the month.
		/// In this case, the day identified is the latest possible date that the transition can be.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-month indicator, from -28 to 31 excluding 0 </returns>
		public int DayOfMonthIndicator
		{
			get
			{
				return Dom;
			}
		}

		/// <summary>
		/// Gets the day-of-week of the transition.
		/// <para>
		/// If the rule defines an exact date then this returns null.
		/// </para>
		/// <para>
		/// If the rule defines a week where the cutover might occur, then this method
		/// returns the day-of-week that the month-day will be adjusted to.
		/// If the day is positive then the adjustment is later.
		/// If the day is negative then the adjustment is earlier.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the day-of-week that the transition occurs, null if the rule defines an exact date </returns>
		public DayOfWeek DayOfWeek
		{
			get
			{
				return Dow;
			}
		}

		/// <summary>
		/// Gets the local time of day of the transition which must be checked with
		/// <seealso cref="#isMidnightEndOfDay()"/>.
		/// <para>
		/// The time is converted into an instant using the time definition.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the local time of day of the transition, not null </returns>
		public LocalTime LocalTime
		{
			get
			{
				return Time;
			}
		}

		/// <summary>
		/// Is the transition local time midnight at the end of day.
		/// <para>
		/// The transition may be represented as occurring at 24:00.
		/// 
		/// </para>
		/// </summary>
		/// <returns> whether a local time of midnight is at the start or end of the day </returns>
		public bool MidnightEndOfDay
		{
			get
			{
				return TimeEndOfDay;
			}
		}

		/// <summary>
		/// Gets the time definition, specifying how to convert the time to an instant.
		/// <para>
		/// The local time can be converted to an instant using the standard offset,
		/// the wall offset or UTC.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time definition, not null </returns>
		public TimeDefinition TimeDefinition
		{
			get
			{
				return TimeDefinition_Renamed;
			}
		}

		/// <summary>
		/// Gets the standard offset in force at the transition.
		/// </summary>
		/// <returns> the standard offset, not null </returns>
		public ZoneOffset StandardOffset
		{
			get
			{
				return StandardOffset_Renamed;
			}
		}

		/// <summary>
		/// Gets the offset before the transition.
		/// </summary>
		/// <returns> the offset before, not null </returns>
		public ZoneOffset OffsetBefore
		{
			get
			{
				return OffsetBefore_Renamed;
			}
		}

		/// <summary>
		/// Gets the offset after the transition.
		/// </summary>
		/// <returns> the offset after, not null </returns>
		public ZoneOffset OffsetAfter
		{
			get
			{
				return OffsetAfter_Renamed;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates a transition instance for the specified year.
		/// <para>
		/// Calculations are performed using the ISO-8601 chronology.
		/// 
		/// </para>
		/// </summary>
		/// <param name="year">  the year to create a transition for, not null </param>
		/// <returns> the transition instance, not null </returns>
		public ZoneOffsetTransition CreateTransition(int year)
		{
			LocalDate date;
			if (Dom < 0)
			{
				date = LocalDate.Of(year, Month_Renamed, Month_Renamed.length(IsoChronology.INSTANCE.IsLeapYear(year)) + 1 + Dom);
				if (Dow != null)
				{
					date = date.With(previousOrSame(Dow));
				}
			}
			else
			{
				date = LocalDate.Of(year, Month_Renamed, Dom);
				if (Dow != null)
				{
					date = date.With(nextOrSame(Dow));
				}
			}
			if (TimeEndOfDay)
			{
				date = date.PlusDays(1);
			}
			LocalDateTime localDT = LocalDateTime.Of(date, Time);
			LocalDateTime transition = TimeDefinition_Renamed.createDateTime(localDT, StandardOffset_Renamed, OffsetBefore_Renamed);
			return new ZoneOffsetTransition(transition, OffsetBefore_Renamed, OffsetAfter_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this object equals another.
		/// <para>
		/// The entire state of the object is compared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherRule">  the other object to compare to, null returns false </param>
		/// <returns> true if equal </returns>
		public override bool Equals(Object otherRule)
		{
			if (otherRule == this)
			{
				return true;
			}
			if (otherRule is ZoneOffsetTransitionRule)
			{
				ZoneOffsetTransitionRule other = (ZoneOffsetTransitionRule) otherRule;
				return Month_Renamed == other.Month_Renamed && Dom == other.Dom && Dow == other.Dow && TimeDefinition_Renamed == other.TimeDefinition_Renamed && Time.Equals(other.Time) && TimeEndOfDay == other.TimeEndOfDay && StandardOffset_Renamed.Equals(other.StandardOffset_Renamed) && OffsetBefore_Renamed.Equals(other.OffsetBefore_Renamed) && OffsetAfter_Renamed.Equals(other.OffsetAfter_Renamed);
			}
			return false;
		}

		/// <summary>
		/// Returns a suitable hash code.
		/// </summary>
		/// <returns> the hash code </returns>
		public override int HashCode()
		{
			int hash = ((Time.ToSecondOfDay() + (TimeEndOfDay ? 1 : 0)) << 15) + (Month_Renamed.ordinal() << 11) + ((Dom + 32) << 5) + ((Dow == null ? 7 : Dow.ordinal()) << 2) + (TimeDefinition_Renamed.ordinal());
			return hash ^ StandardOffset_Renamed.HashCode() ^ OffsetBefore_Renamed.HashCode() ^ OffsetAfter_Renamed.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a string describing this object.
		/// </summary>
		/// <returns> a string for debugging, not null </returns>
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("TransitionRule[").Append(OffsetBefore_Renamed.CompareTo(OffsetAfter_Renamed) > 0 ? "Gap " : "Overlap ").Append(OffsetBefore_Renamed).Append(" to ").Append(OffsetAfter_Renamed).Append(", ");
			if (Dow != null)
			{
				if (Dom == -1)
				{
					buf.Append(Dow.name()).Append(" on or before last day of ").Append(Month_Renamed.name());
				}
				else if (Dom < 0)
				{
					buf.Append(Dow.name()).Append(" on or before last day minus ").Append(-Dom - 1).Append(" of ").Append(Month_Renamed.name());
				}
				else
				{
					buf.Append(Dow.name()).Append(" on or after ").Append(Month_Renamed.name()).Append(' ').Append(Dom);
				}
			}
			else
			{
				buf.Append(Month_Renamed.name()).Append(' ').Append(Dom);
			}
			buf.Append(" at ").Append(TimeEndOfDay ? "24:00" : Time.ToString()).Append(" ").Append(TimeDefinition_Renamed).Append(", standard offset ").Append(StandardOffset_Renamed).Append(']');
			return buf.ToString();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// A definition of the way a local time can be converted to the actual
		/// transition date-time.
		/// <para>
		/// Time zone rules are expressed in one of three ways:
		/// <ul>
		/// <li>Relative to UTC</li>
		/// <li>Relative to the standard offset in force</li>
		/// <li>Relative to the wall offset (what you would see on a clock on the wall)</li>
		/// </ul>
		/// </para>
		/// </summary>
		public sealed class TimeDefinition
		{
			/// <summary>
			/// The local date-time is expressed in terms of the UTC offset. </summary>
			UTC,
			public static readonly TimeDefinition UTC = new TimeDefinition("UTC", InnerEnum.UTC);
			/// <summary>
			/// The local date-time is expressed in terms of the wall offset. </summary>
			WALL,
			public static readonly TimeDefinition WALL = new TimeDefinition("WALL", InnerEnum.WALL);
			/// <summary>
			/// The local date-time is expressed in terms of the standard offset. </summary>
			STANDARD
			public static readonly TimeDefinition STANDARD = new TimeDefinition("STANDARD", InnerEnum.STANDARD);

			private static readonly IList<TimeDefinition> valueList = new List<TimeDefinition>();

			static TimeDefinition()
			{
				valueList.Add(UTC);
				valueList.Add(WALL);
				valueList.Add(STANDARD);
			}

			public enum InnerEnum
			{
				UTC,
				WALL,
				STANDARD
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			/// <summary>
			/// Converts the specified local date-time to the local date-time actually
			/// seen on a wall clock.
			/// <para>
			/// This method converts using the type of this enum.
			/// The output is defined relative to the 'before' offset of the transition.
			/// </para>
			/// <para>
			/// The UTC type uses the UTC offset.
			/// The STANDARD type uses the standard offset.
			/// The WALL type returns the input date-time.
			/// The result is intended for use with the wall-offset.
			/// 
			/// </para>
			/// </summary>
			/// <param name="dateTime">  the local date-time, not null </param>
			/// <param name="standardOffset">  the standard offset, not null </param>
			/// <param name="wallOffset">  the wall offset, not null </param>
			/// <returns> the date-time relative to the wall/before offset, not null </returns>
			public java.time.LocalDateTime CreateDateTime(java.time.LocalDateTime dateTime, java.time.ZoneOffset standardOffset, java.time.ZoneOffset wallOffset)
			{
				switch (this)
				{
					case UTC:
					{
						int difference = wallOffset.TotalSeconds - ZoneOffset.UTC.TotalSeconds;
						return dateTime.PlusSeconds(difference);
					}
					case STANDARD:
					{
						int difference = wallOffset.TotalSeconds - standardOffset.TotalSeconds;
						return dateTime.PlusSeconds(difference);
					}
					default: // WALL
						return dateTime;
				}
			}

			public static IList<TimeDefinition> values()
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

			public static TimeDefinition valueOf(string name)
			{
				foreach (TimeDefinition enumInstance in TimeDefinition.values())
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

}