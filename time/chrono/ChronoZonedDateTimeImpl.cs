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
	/// A date-time with a time-zone in the calendar neutral API.
	/// <para>
	/// {@code ZoneChronoDateTime} is an immutable representation of a date-time with a time-zone.
	/// This class stores all date and time fields, to a precision of nanoseconds,
	/// as well as a time-zone and zone offset.
	/// </para>
	/// <para>
	/// The purpose of storing the time-zone is to distinguish the ambiguous case where
	/// the local time-line overlaps, typically as a result of the end of daylight time.
	/// Information about the local-time can be obtained using methods on the time-zone.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @serial Document the delegation of this class in the serialized-form specification.
	/// </para>
	/// </summary>
	/// @param <D> the concrete type for the date of this date-time
	/// @since 1.8 </param>
	[Serializable]
	internal sealed class ChronoZonedDateTimeImpl<D> : ChronoZonedDateTime<D> where D : ChronoLocalDate
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -5261813987200935591L;

		/// <summary>
		/// The local date-time.
		/// </summary>
		[NonSerialized]
		private readonly ChronoLocalDateTimeImpl<D> DateTime;
		/// <summary>
		/// The zone offset.
		/// </summary>
		[NonSerialized]
		private readonly ZoneOffset Offset_Renamed;
		/// <summary>
		/// The zone ID.
		/// </summary>
		[NonSerialized]
		private readonly ZoneId Zone_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance from a local date-time using the preferred offset if possible.
		/// </summary>
		/// <param name="localDateTime">  the local date-time, not null </param>
		/// <param name="zone">  the zone identifier, not null </param>
		/// <param name="preferredOffset">  the zone offset, null if no preference </param>
		/// <returns> the zoned date-time, not null </returns>
		internal static ChronoZonedDateTime<R> ofBest<R>(ChronoLocalDateTimeImpl<R> localDateTime, ZoneId zone, ZoneOffset preferredOffset) where R : ChronoLocalDate
		{
			Objects.RequireNonNull(localDateTime, "localDateTime");
			Objects.RequireNonNull(zone, "zone");
			if (zone is ZoneOffset)
			{
				return new ChronoZonedDateTimeImpl<>(localDateTime, (ZoneOffset) zone, zone);
			}
			ZoneRules rules = zone.Rules;
			LocalDateTime isoLDT = LocalDateTime.From(localDateTime);
			IList<ZoneOffset> validOffsets = rules.GetValidOffsets(isoLDT);
			ZoneOffset offset;
			if (validOffsets.Count == 1)
			{
				offset = validOffsets[0];
			}
			else if (validOffsets.Count == 0)
			{
				ZoneOffsetTransition trans = rules.GetTransition(isoLDT);
				localDateTime = localDateTime.PlusSeconds(trans.Duration.Seconds);
				offset = trans.OffsetAfter;
			}
			else
			{
				if (preferredOffset != null && validOffsets.Contains(preferredOffset))
				{
					offset = preferredOffset;
				}
				else
				{
					offset = validOffsets[0];
				}
			}
			Objects.RequireNonNull(offset, "offset"); // protect against bad ZoneRules
			return new ChronoZonedDateTimeImpl<>(localDateTime, offset, zone);
		}

		/// <summary>
		/// Obtains an instance from an instant using the specified time-zone.
		/// </summary>
		/// <param name="chrono">  the chronology, not null </param>
		/// <param name="instant">  the instant, not null </param>
		/// <param name="zone">  the zone identifier, not null </param>
		/// <returns> the zoned date-time, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static ChronoZonedDateTimeImpl<?> ofInstant(Chronology ChronoZonedDateTime_Fields.chrono, java.time.Instant instant, java.time.ZoneId zone)
		internal static ChronoZonedDateTimeImpl<?> OfInstant(Chronology ChronoZonedDateTime_Fields, Instant instant, ZoneId zone)
		{
			ZoneRules rules = zone.Rules;
			ZoneOffset offset = rules.GetOffset(instant);
			Objects.RequireNonNull(offset, "offset"); // protect against bad ZoneRules
			LocalDateTime ldt = LocalDateTime.OfEpochSecond(instant.EpochSecond, instant.Nano, offset);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ChronoLocalDateTimeImpl<?> cldt = (ChronoLocalDateTimeImpl<?>)ChronoZonedDateTime_Fields.chrono.localDateTime(ldt);
			ChronoLocalDateTimeImpl<?> cldt = (ChronoLocalDateTimeImpl<?>)ChronoZonedDateTime_Fields.Chrono.localDateTime(ldt);
			return new ChronoZonedDateTimeImpl<>(cldt, offset, zone);
		}

		/// <summary>
		/// Obtains an instance from an {@code Instant}.
		/// </summary>
		/// <param name="instant">  the instant to create the date-time from, not null </param>
		/// <param name="zone">  the time-zone to use, validated not null </param>
		/// <returns> the zoned date-time, validated not null </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private ChronoZonedDateTimeImpl<D> create(java.time.Instant instant, java.time.ZoneId zone)
		private ChronoZonedDateTimeImpl<D> Create(Instant instant, ZoneId zone)
		{
			return (ChronoZonedDateTimeImpl<D>)OfInstant(Chronology, instant, zone);
		}

		/// <summary>
		/// Casts the {@code Temporal} to {@code ChronoZonedDateTimeImpl} ensuring it bas the specified chronology.
		/// </summary>
		/// <param name="chrono">  the chronology to check for, not null </param>
		/// <param name="temporal">  a date-time to cast, not null </param>
		/// <returns> the date-time checked and cast to {@code ChronoZonedDateTimeImpl}, not null </returns>
		/// <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoZonedDateTimeImpl
		///  or the chronology is not equal this Chronology </exception>
		internal static ChronoZonedDateTimeImpl<R> ensureValid<R>(Chronology ChronoZonedDateTime_Fields, Temporal temporal) where R : ChronoLocalDate
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ChronoZonedDateTimeImpl<R> other = (ChronoZonedDateTimeImpl<R>) temporal;
			ChronoZonedDateTimeImpl<R> other = (ChronoZonedDateTimeImpl<R>) temporal;
			if (ChronoZonedDateTime_Fields.Chrono.Equals(other.Chronology) == false)
			{
				throw new ClassCastException("Chronology mismatch, required: " + ChronoZonedDateTime_Fields.Chrono.Id + ", actual: " + other.Chronology.Id);
			}
			return other;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dateTime">  the date-time, not null </param>
		/// <param name="offset">  the zone offset, not null </param>
		/// <param name="zone">  the zone ID, not null </param>
		private ChronoZonedDateTimeImpl(ChronoLocalDateTimeImpl<D> dateTime, ZoneOffset offset, ZoneId zone)
		{
			this.DateTime = Objects.RequireNonNull(dateTime, "dateTime");
			this.Offset_Renamed = Objects.RequireNonNull(offset, "offset");
			this.Zone_Renamed = Objects.RequireNonNull(zone, "zone");
		}

		//-----------------------------------------------------------------------
		public override ZoneOffset Offset
		{
			get
			{
				return Offset_Renamed;
			}
		}

		public override ChronoZonedDateTime<D> WithEarlierOffsetAtOverlap()
		{
			ZoneOffsetTransition trans = Zone.Rules.GetTransition(LocalDateTime.From(this));
			if (trans != null && trans.Overlap)
			{
				ZoneOffset earlierOffset = trans.OffsetBefore;
				if (earlierOffset.Equals(Offset_Renamed) == false)
				{
					return new ChronoZonedDateTimeImpl<>(DateTime, earlierOffset, Zone_Renamed);
				}
			}
			return this;
		}

		public override ChronoZonedDateTime<D> WithLaterOffsetAtOverlap()
		{
			ZoneOffsetTransition trans = Zone.Rules.GetTransition(LocalDateTime.From(this));
			if (trans != null)
			{
				ZoneOffset offset = trans.OffsetAfter;
				if (offset.Equals(Offset) == false)
				{
					return new ChronoZonedDateTimeImpl<>(DateTime, offset, Zone_Renamed);
				}
			}
			return this;
		}

		//-----------------------------------------------------------------------
		public override ChronoLocalDateTime<D> ToLocalDateTime()
		{
			return DateTime;
		}

		public override ZoneId Zone
		{
			get
			{
				return Zone_Renamed;
			}
		}

		public override ChronoZonedDateTime<D> WithZoneSameLocal(ZoneId zone)
		{
			return OfBest(DateTime, zone, Offset_Renamed);
		}

		public override ChronoZonedDateTime<D> WithZoneSameInstant(ZoneId zone)
		{
			Objects.RequireNonNull(zone, "zone");
			return this.Zone_Renamed.Equals(zone) ? this : Create(DateTime.toInstant(Offset_Renamed), zone);
		}

		//-----------------------------------------------------------------------
		public override bool IsSupported(TemporalField field)
		{
			return field is ChronoField || (field != null && field.IsSupportedBy(this));
		}

		//-----------------------------------------------------------------------
		public override ChronoZonedDateTime<D> With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				switch (f)
				{
					case INSTANT_SECONDS:
						return Plus(newValue - toEpochSecond(), SECONDS);
					case OFFSET_SECONDS:
					{
						ZoneOffset offset = ZoneOffset.OfTotalSeconds(f.checkValidIntValue(newValue));
						return Create(DateTime.toInstant(offset), Zone_Renamed);
					}
				}
				return OfBest(DateTime.With(field, newValue), Zone_Renamed, Offset_Renamed);
			}
			return ChronoZonedDateTimeImpl.EnsureValid(Chronology, field.AdjustInto(this, newValue));
		}

		//-----------------------------------------------------------------------
		public override ChronoZonedDateTime<D> Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				return with(DateTime.Plus(amountToAdd, unit));
			}
			return ChronoZonedDateTimeImpl.EnsureValid(Chronology, unit.AddTo(this, amountToAdd)); /// TODO: Generics replacement Risk!
		}

		//-----------------------------------------------------------------------
		public override long Until(Temporal endExclusive, TemporalUnit unit)
		{
			Objects.RequireNonNull(endExclusive, "endExclusive");
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ChronoZonedDateTime<D> end = (ChronoZonedDateTime<D>) getChronology().zonedDateTime(endExclusive);
			ChronoZonedDateTime<D> end = (ChronoZonedDateTime<D>) Chronology.zonedDateTime(endExclusive);
			if (unit is ChronoUnit)
			{
				end = end.WithZoneSameInstant(Offset_Renamed);
				return DateTime.Until(end.ToLocalDateTime(), unit);
			}
			Objects.RequireNonNull(unit, "unit");
			return unit.Between(this, end);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the ChronoZonedDateTime using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(3);                  // identifies a ChronoZonedDateTime
		///  out.writeObject(toLocalDateTime());
		///  out.writeObject(getOffset());
		///  out.writeObject(getZone());
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.CHRONO_ZONE_DATE_TIME_TYPE, this);
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
			@out.WriteObject(DateTime);
			@out.WriteObject(Offset_Renamed);
			@out.WriteObject(Zone_Renamed);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ChronoZonedDateTime<?> readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal static ChronoZonedDateTime<?> ReadExternal(ObjectInput @in)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ChronoLocalDateTime<?> dateTime = (ChronoLocalDateTime<?>) in.readObject();
			ChronoLocalDateTime<?> dateTime = (ChronoLocalDateTime<?>) @in.ReadObject();
			ZoneOffset offset = (ZoneOffset) @in.ReadObject();
			ZoneId zone = (ZoneId) @in.ReadObject();
			return dateTime.AtZone(offset).WithZoneSameLocal(zone);
			// TODO: ZDT uses ofLenient()
		}

		//-------------------------------------------------------------------------
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is ChronoZonedDateTime)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return compareTo((ChronoZonedDateTime<?>) obj) == 0;
				return compareTo((ChronoZonedDateTime<?>) obj) == 0;
			}
			return false;
		}

		public override int HashCode()
		{
			return ToLocalDateTime().HashCode() ^ Offset.HashCode() ^ Integer.RotateLeft(Zone.HashCode(), 3);
		}

		public override String ToString()
		{
			String str = ToLocalDateTime().ToString() + Offset.ToString();
			if (Offset != Zone)
			{
				str += '[' + Zone.ToString() + ']';
			}
			return str;
		}


	}

}