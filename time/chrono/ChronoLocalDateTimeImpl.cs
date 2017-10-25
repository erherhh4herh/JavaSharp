using System;

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
 * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.chrono
{


	/// <summary>
	/// A date-time without a time-zone for the calendar neutral API.
	/// <para>
	/// {@code ChronoLocalDateTime} is an immutable date-time object that represents a date-time, often
	/// viewed as year-month-day-hour-minute-second. This object can also access other
	/// fields such as day-of-year, day-of-week and week-of-year.
	/// </para>
	/// <para>
	/// This class stores all date and time fields, to a precision of nanoseconds.
	/// It does not store or represent a time-zone. For example, the value
	/// "2nd October 2007 at 13:45.30.123456789" can be stored in an {@code ChronoLocalDateTime}.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// @serial
	/// </para>
	/// </summary>
	/// @param <D> the concrete type for the date of this date-time
	/// @since 1.8 </param>
	[Serializable]
	internal sealed class ChronoLocalDateTimeImpl<D> : ChronoLocalDateTime<D>, Temporal, TemporalAdjuster where D : ChronoLocalDate
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 4556003607393004514L;
		/// <summary>
		/// Hours per day.
		/// </summary>
		internal const int HOURS_PER_DAY = 24;
		/// <summary>
		/// Minutes per hour.
		/// </summary>
		internal const int MINUTES_PER_HOUR = 60;
		/// <summary>
		/// Minutes per day.
		/// </summary>
		internal static readonly int MINUTES_PER_DAY = MINUTES_PER_HOUR * HOURS_PER_DAY;
		/// <summary>
		/// Seconds per minute.
		/// </summary>
		internal const int SECONDS_PER_MINUTE = 60;
		/// <summary>
		/// Seconds per hour.
		/// </summary>
		internal static readonly int SECONDS_PER_HOUR = SECONDS_PER_MINUTE * MINUTES_PER_HOUR;
		/// <summary>
		/// Seconds per day.
		/// </summary>
		internal static readonly int SECONDS_PER_DAY = SECONDS_PER_HOUR * HOURS_PER_DAY;
		/// <summary>
		/// Milliseconds per day.
		/// </summary>
		internal static readonly long MILLIS_PER_DAY = SECONDS_PER_DAY * 1000L;
		/// <summary>
		/// Microseconds per day.
		/// </summary>
		internal static readonly long MICROS_PER_DAY = SECONDS_PER_DAY * 1000_000L;
		/// <summary>
		/// Nanos per second.
		/// </summary>
		internal static readonly long NANOS_PER_SECOND = 1000_000_000L;
		/// <summary>
		/// Nanos per minute.
		/// </summary>
		internal static readonly long NANOS_PER_MINUTE = NANOS_PER_SECOND * SECONDS_PER_MINUTE;
		/// <summary>
		/// Nanos per hour.
		/// </summary>
		internal static readonly long NANOS_PER_HOUR = NANOS_PER_MINUTE * MINUTES_PER_HOUR;
		/// <summary>
		/// Nanos per day.
		/// </summary>
		internal static readonly long NANOS_PER_DAY = NANOS_PER_HOUR * HOURS_PER_DAY;

		/// <summary>
		/// The date part.
		/// </summary>
		[NonSerialized]
		private readonly D Date;
		/// <summary>
		/// The time part.
		/// </summary>
		[NonSerialized]
		private readonly LocalTime Time;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ChronoLocalDateTime} from a date and time.
		/// </summary>
		/// <param name="date">  the local date, not null </param>
		/// <param name="time">  the local time, not null </param>
		/// <returns> the local date-time, not null </returns>
		internal static ChronoLocalDateTimeImpl<R> of<R>(R date, LocalTime time) where R : ChronoLocalDate
		{
			return new ChronoLocalDateTimeImpl<>(date, time);
		}

		/// <summary>
		/// Casts the {@code Temporal} to {@code ChronoLocalDateTime} ensuring it bas the specified chronology.
		/// </summary>
		/// <param name="chrono">  the chronology to check for, not null </param>
		/// <param name="temporal">   a date-time to cast, not null </param>
		/// <returns> the date-time checked and cast to {@code ChronoLocalDateTime}, not null </returns>
		/// <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoLocalDateTimeImpl
		///  or the chronology is not equal this Chronology </exception>
		internal static ChronoLocalDateTimeImpl<R> ensureValid<R>(Chronology ChronoLocalDateTime_Fields, Temporal temporal) where R : ChronoLocalDate
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ChronoLocalDateTimeImpl<R> other = (ChronoLocalDateTimeImpl<R>) temporal;
			ChronoLocalDateTimeImpl<R> other = (ChronoLocalDateTimeImpl<R>) temporal;
			if (ChronoLocalDateTime_Fields.Chrono.Equals(other.Chronology) == false)
			{
				throw new ClassCastException("Chronology mismatch, required: " + ChronoLocalDateTime_Fields.Chrono.Id + ", actual: " + other.Chronology.Id);
			}
			return other;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="date">  the date part of the date-time, not null </param>
		/// <param name="time">  the time part of the date-time, not null </param>
		private ChronoLocalDateTimeImpl(D date, LocalTime time)
		{
			Objects.RequireNonNull(date, "date");
			Objects.RequireNonNull(time, "time");
			this.Date = date;
			this.Time = time;
		}

		/// <summary>
		/// Returns a copy of this date-time with the new date and time, checking
		/// to see if a new object is in fact required.
		/// </summary>
		/// <param name="newDate">  the date of the new date-time, not null </param>
		/// <param name="newTime">  the time of the new date-time, not null </param>
		/// <returns> the date-time, not null </returns>
		private ChronoLocalDateTimeImpl<D> With(Temporal newDate, LocalTime newTime)
		{
			if (Date == newDate && Time == newTime)
			{
				return this;
			}
			// Validate that the new Temporal is a ChronoLocalDate (and not something else)
			D cd = ChronoLocalDateImpl.EnsureValid(Date.Chronology, newDate);
			return new ChronoLocalDateTimeImpl<>(cd, newTime);
		}

		//-----------------------------------------------------------------------
		public override D ToLocalDate()
		{
			return Date;
		}

		public override LocalTime ToLocalTime()
		{
			return Time;
		}

		//-----------------------------------------------------------------------
		public bool IsSupported(TemporalField field)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				return f.DateBased || f.TimeBased;
			}
			return field != ChronoLocalDateTime_Fields.Null && field.IsSupportedBy(this);
		}

		public override ValueRange java.time.temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.Range(field) : Date.range(field));
			}
			return field.RangeRefinedBy(this);
		}

		public override int Get(TemporalField field)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.Get(field) : Date.get(field));
			}
			return java.time.temporal.TemporalAccessor_Fields.range(field).checkValidIntValue(GetLong(field), field);
		}

		public long GetLong(TemporalField field)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				return (f.TimeBased ? Time.GetLong(field) : Date.GetLong(field));
			}
			return field.GetFrom(this);
		}

		//-----------------------------------------------------------------------
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public ChronoLocalDateTimeImpl<D> with(java.time.temporal.TemporalAdjuster adjuster)
		public override ChronoLocalDateTimeImpl<D> With(TemporalAdjuster adjuster)
		{
			if (adjuster is ChronoLocalDate)
			{
				// The Chronology is checked in with(date,time)
				return With((ChronoLocalDate) adjuster, Time);
			}
			else if (adjuster is LocalTime)
			{
				return With(Date, (LocalTime) adjuster);
			}
			else if (adjuster is ChronoLocalDateTimeImpl)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ChronoLocalDateTimeImpl.ensureValid(date.getChronology(), (ChronoLocalDateTimeImpl<?>) adjuster);
				return ChronoLocalDateTimeImpl.EnsureValid(Date.Chronology, (ChronoLocalDateTimeImpl<?>) adjuster);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ChronoLocalDateTimeImpl.ensureValid(date.getChronology(), (ChronoLocalDateTimeImpl<?>) adjuster.adjustInto(this));
			return ChronoLocalDateTimeImpl.EnsureValid(Date.Chronology, (ChronoLocalDateTimeImpl<?>) adjuster.AdjustInto(this));
		}

		public ChronoLocalDateTimeImpl<D> With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				if (f.TimeBased)
				{
					return With(Date, Time.With(field, newValue));
				}
				else
				{
					return With(Date.With(field, newValue), Time);
				}
			}
			return ChronoLocalDateTimeImpl.EnsureValid(Date.Chronology, field.AdjustInto(this, newValue));
		}

		//-----------------------------------------------------------------------
		public ChronoLocalDateTimeImpl<D> Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				ChronoUnit f = (ChronoUnit) unit;
				switch (f)
				{
					case NANOS:
						return PlusNanos(amountToAdd);
					case MICROS:
						return PlusDays(amountToAdd / MICROS_PER_DAY).PlusNanos((amountToAdd % MICROS_PER_DAY) * 1000);
					case MILLIS:
						return PlusDays(amountToAdd / MILLIS_PER_DAY).PlusNanos((amountToAdd % MILLIS_PER_DAY) * 1000000);
					case SECONDS:
						return PlusSeconds(amountToAdd);
					case MINUTES:
						return PlusMinutes(amountToAdd);
					case HOURS:
						return PlusHours(amountToAdd);
					case HALF_DAYS: // no overflow (256 is multiple of 2)
						return PlusDays(amountToAdd / 256).PlusHours((amountToAdd % 256) * 12);
				}
				return With(Date.Plus(amountToAdd, unit), Time);
			}
			return ChronoLocalDateTimeImpl.EnsureValid(Date.Chronology, unit.AddTo(this, amountToAdd));
		}

		private ChronoLocalDateTimeImpl<D> PlusDays(long days)
		{
			return With(Date.Plus(days, ChronoUnit.DAYS), Time);
		}

		private ChronoLocalDateTimeImpl<D> PlusHours(long hours)
		{
			return PlusWithOverflow(Date, hours, 0, 0, 0);
		}

		private ChronoLocalDateTimeImpl<D> PlusMinutes(long minutes)
		{
			return PlusWithOverflow(Date, 0, minutes, 0, 0);
		}

		internal ChronoLocalDateTimeImpl<D> PlusSeconds(long seconds)
		{
			return PlusWithOverflow(Date, 0, 0, seconds, 0);
		}

		private ChronoLocalDateTimeImpl<D> PlusNanos(long nanos)
		{
			return PlusWithOverflow(Date, 0, 0, 0, nanos);
		}

		//-----------------------------------------------------------------------
		private ChronoLocalDateTimeImpl<D> PlusWithOverflow(D newDate, long hours, long minutes, long seconds, long nanos)
		{
			// 9223372036854775808 long, 2147483648 int
			if ((hours | minutes | seconds | nanos) == 0)
			{
				return With(newDate, Time);
			}
			long totDays = nanos / NANOS_PER_DAY + seconds / SECONDS_PER_DAY + minutes / MINUTES_PER_DAY + hours / HOURS_PER_DAY; //   max/24 -    max/24*60 -    max/24*60*60 -    max/24*60*60*1B
			long totNanos = nanos % NANOS_PER_DAY + (seconds % SECONDS_PER_DAY) * NANOS_PER_SECOND + (minutes % MINUTES_PER_DAY) * NANOS_PER_MINUTE + (hours % HOURS_PER_DAY) * NANOS_PER_HOUR; //   max  86400000000000 -    max  86400000000000 -    max  86400000000000 -    max  86400000000000
			long curNoD = Time.ToNanoOfDay(); //   max  86400000000000
			totNanos = totNanos + curNoD; // total 432000000000000
			totDays += Math.FloorDiv(totNanos, NANOS_PER_DAY);
			long newNoD = Math.FloorMod(totNanos, NANOS_PER_DAY);
			LocalTime newTime = (newNoD == curNoD ? Time : LocalTime.OfNanoOfDay(newNoD));
			return With(newDate.Plus(totDays, ChronoUnit.DAYS), newTime);
		}

		//-----------------------------------------------------------------------
		public override ChronoZonedDateTime<D> AtZone(ZoneId zone)
		{
			return ChronoZonedDateTimeImpl.OfBest(this, zone, ChronoLocalDateTime_Fields.Null);
		}

		//-----------------------------------------------------------------------
		public long Until(Temporal endExclusive, TemporalUnit unit)
		{
			Objects.RequireNonNull(endExclusive, "endExclusive");
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ChronoLocalDateTime<D> end = (ChronoLocalDateTime<D>) getChronology().localDateTime(endExclusive);
			ChronoLocalDateTime<D> end = (ChronoLocalDateTime<D>) Chronology.localDateTime(endExclusive);
			if (unit is ChronoUnit)
			{
				if (unit.TimeBased)
				{
					long amount = end.getLong(EPOCH_DAY) - Date.GetLong(EPOCH_DAY);
					switch ((ChronoUnit) unit)
					{
						case NANOS:
							amount = Math.MultiplyExact(amount, NANOS_PER_DAY);
							break;
						case MICROS:
							amount = Math.MultiplyExact(amount, MICROS_PER_DAY);
							break;
						case MILLIS:
							amount = Math.MultiplyExact(amount, MILLIS_PER_DAY);
							break;
						case SECONDS:
							amount = Math.MultiplyExact(amount, SECONDS_PER_DAY);
							break;
						case MINUTES:
							amount = Math.MultiplyExact(amount, MINUTES_PER_DAY);
							break;
						case HOURS:
							amount = Math.MultiplyExact(amount, HOURS_PER_DAY);
							break;
						case HALF_DAYS:
							amount = Math.MultiplyExact(amount, 2);
							break;
					}
					return Math.AddExact(amount, Time.Until(end.ToLocalTime(), unit));
				}
				ChronoLocalDate endDate = end.ToLocalDate();
				if (end.ToLocalTime().IsBefore(Time))
				{
					endDate = endDate.minus(1, ChronoUnit.DAYS);
				}
				return Date.Until(endDate, unit);
			}
			Objects.RequireNonNull(unit, "unit");
			return unit.Between(this, end);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the ChronoLocalDateTime using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(2);              // identifies a ChronoLocalDateTime
		///  out.writeObject(toLocalDate());
		///  out.witeObject(toLocalTime());
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.CHRONO_LOCAL_DATE_TIME_TYPE, this);
		}

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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		internal void WriteExternal(ObjectOutput @out)
		{
			@out.WriteObject(Date);
			@out.WriteObject(Time);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ChronoLocalDateTime<?> readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal static ChronoLocalDateTime<?> ReadExternal(ObjectInput @in)
		{
			ChronoLocalDate date = (ChronoLocalDate) @in.ReadObject();
			LocalTime time = (LocalTime) @in.ReadObject();
			return date.atTime(time);
		}

		//-----------------------------------------------------------------------
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is ChronoLocalDateTime)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return compareTo((ChronoLocalDateTime<?>) obj) == 0;
				return compareTo((ChronoLocalDateTime<?>) obj) == 0;
			}
			return false;
		}

		public override int HashCode()
		{
			return ToLocalDate().HashCode() ^ ToLocalTime().HashCode();
		}

		public override String ToString()
		{
			return ToLocalDate().ToString() + 'T' + ToLocalTime().ToString();
		}

	}

}