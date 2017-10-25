using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
	/// A transition between two offsets caused by a discontinuity in the local time-line.
	/// <para>
	/// A transition between two offsets is normally the result of a daylight savings cutover.
	/// The discontinuity is normally a gap in spring and an overlap in autumn.
	/// {@code ZoneOffsetTransition} models the transition between the two offsets.
	/// </para>
	/// <para>
	/// Gaps occur where there are local date-times that simply do not exist.
	/// An example would be when the offset changes from {@code +03:00} to {@code +04:00}.
	/// This might be described as 'the clocks will move forward one hour tonight at 1am'.
	/// </para>
	/// <para>
	/// Overlaps occur where there are local date-times that exist twice.
	/// An example would be when the offset changes from {@code +04:00} to {@code +03:00}.
	/// This might be described as 'the clocks will move back one hour tonight at 2am'.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ZoneOffsetTransition : Comparable<ZoneOffsetTransition>
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -6946044323557704546L;
		/// <summary>
		/// The local transition date-time at the transition.
		/// </summary>
		private readonly LocalDateTime Transition;
		/// <summary>
		/// The offset before transition.
		/// </summary>
		private readonly ZoneOffset OffsetBefore_Renamed;
		/// <summary>
		/// The offset after transition.
		/// </summary>
		private readonly ZoneOffset OffsetAfter_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance defining a transition between two offsets.
		/// <para>
		/// Applications should normally obtain an instance from <seealso cref="ZoneRules"/>.
		/// This factory is only intended for use when creating <seealso cref="ZoneRules"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="transition">  the transition date-time at the transition, which never
		///  actually occurs, expressed local to the before offset, not null </param>
		/// <param name="offsetBefore">  the offset before the transition, not null </param>
		/// <param name="offsetAfter">  the offset at and after the transition, not null </param>
		/// <returns> the transition, not null </returns>
		/// <exception cref="IllegalArgumentException"> if {@code offsetBefore} and {@code offsetAfter}
		///         are equal, or {@code transition.getNano()} returns non-zero value </exception>
		public static ZoneOffsetTransition Of(LocalDateTime transition, ZoneOffset offsetBefore, ZoneOffset offsetAfter)
		{
			Objects.RequireNonNull(transition, "transition");
			Objects.RequireNonNull(offsetBefore, "offsetBefore");
			Objects.RequireNonNull(offsetAfter, "offsetAfter");
			if (offsetBefore.Equals(offsetAfter))
			{
				throw new IllegalArgumentException("Offsets must not be equal");
			}
			if (transition.Nano != 0)
			{
				throw new IllegalArgumentException("Nano-of-second must be zero");
			}
			return new ZoneOffsetTransition(transition, offsetBefore, offsetAfter);
		}

		/// <summary>
		/// Creates an instance defining a transition between two offsets.
		/// </summary>
		/// <param name="transition">  the transition date-time with the offset before the transition, not null </param>
		/// <param name="offsetBefore">  the offset before the transition, not null </param>
		/// <param name="offsetAfter">  the offset at and after the transition, not null </param>
		internal ZoneOffsetTransition(LocalDateTime transition, ZoneOffset offsetBefore, ZoneOffset offsetAfter)
		{
			this.Transition = transition;
			this.OffsetBefore_Renamed = offsetBefore;
			this.OffsetAfter_Renamed = offsetAfter;
		}

		/// <summary>
		/// Creates an instance from epoch-second and offsets.
		/// </summary>
		/// <param name="epochSecond">  the transition epoch-second </param>
		/// <param name="offsetBefore">  the offset before the transition, not null </param>
		/// <param name="offsetAfter">  the offset at and after the transition, not null </param>
		internal ZoneOffsetTransition(long epochSecond, ZoneOffset offsetBefore, ZoneOffset offsetAfter)
		{
			this.Transition = LocalDateTime.OfEpochSecond(epochSecond, 0, offsetBefore);
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
		///   out.writeByte(2);                // identifies a ZoneOffsetTransition
		///   out.writeEpochSec(toEpochSecond);
		///   out.writeOffset(offsetBefore);
		///   out.writeOffset(offsetAfter);
		/// }
		/// </pre> </summary>
		/// <returns> the replacing object, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZOT, this);
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
			Ser.WriteEpochSec(ToEpochSecond(), @out);
			Ser.WriteOffset(OffsetBefore_Renamed, @out);
			Ser.WriteOffset(OffsetAfter_Renamed, @out);
		}

		/// <summary>
		/// Reads the state from the stream.
		/// </summary>
		/// <param name="in">  the input stream, not null </param>
		/// <returns> the created object, not null </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZoneOffsetTransition readExternal(java.io.DataInput in) throws java.io.IOException
		internal static ZoneOffsetTransition ReadExternal(DataInput @in)
		{
			long epochSecond = Ser.ReadEpochSec(@in);
			ZoneOffset before = Ser.ReadOffset(@in);
			ZoneOffset after = Ser.ReadOffset(@in);
			if (before.Equals(after))
			{
				throw new IllegalArgumentException("Offsets must not be equal");
			}
			return new ZoneOffsetTransition(epochSecond, before, after);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the transition instant.
		/// <para>
		/// This is the instant of the discontinuity, which is defined as the first
		/// instant that the 'after' offset applies.
		/// </para>
		/// <para>
		/// The methods <seealso cref="#getInstant()"/>, <seealso cref="#getDateTimeBefore()"/> and <seealso cref="#getDateTimeAfter()"/>
		/// all represent the same instant.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the transition instant, not null </returns>
		public Instant Instant
		{
			get
			{
				return Transition.toInstant(OffsetBefore_Renamed);
			}
		}

		/// <summary>
		/// Gets the transition instant as an epoch second.
		/// </summary>
		/// <returns> the transition epoch second </returns>
		public long ToEpochSecond()
		{
			return Transition.toEpochSecond(OffsetBefore_Renamed);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Gets the local transition date-time, as would be expressed with the 'before' offset.
		/// <para>
		/// This is the date-time where the discontinuity begins expressed with the 'before' offset.
		/// At this instant, the 'after' offset is actually used, therefore the combination of this
		/// date-time and the 'before' offset will never occur.
		/// </para>
		/// <para>
		/// The combination of the 'before' date-time and offset represents the same instant
		/// as the 'after' date-time and offset.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the transition date-time expressed with the before offset, not null </returns>
		public LocalDateTime DateTimeBefore
		{
			get
			{
				return Transition;
			}
		}

		/// <summary>
		/// Gets the local transition date-time, as would be expressed with the 'after' offset.
		/// <para>
		/// This is the first date-time after the discontinuity, when the new offset applies.
		/// </para>
		/// <para>
		/// The combination of the 'before' date-time and offset represents the same instant
		/// as the 'after' date-time and offset.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the transition date-time expressed with the after offset, not null </returns>
		public LocalDateTime DateTimeAfter
		{
			get
			{
				return Transition.PlusSeconds(DurationSeconds);
			}
		}

		/// <summary>
		/// Gets the offset before the transition.
		/// <para>
		/// This is the offset in use before the instant of the transition.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the offset before the transition, not null </returns>
		public ZoneOffset OffsetBefore
		{
			get
			{
				return OffsetBefore_Renamed;
			}
		}

		/// <summary>
		/// Gets the offset after the transition.
		/// <para>
		/// This is the offset in use on and after the instant of the transition.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the offset after the transition, not null </returns>
		public ZoneOffset OffsetAfter
		{
			get
			{
				return OffsetAfter_Renamed;
			}
		}

		/// <summary>
		/// Gets the duration of the transition.
		/// <para>
		/// In most cases, the transition duration is one hour, however this is not always the case.
		/// The duration will be positive for a gap and negative for an overlap.
		/// Time-zones are second-based, so the nanosecond part of the duration will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the duration of the transition, positive for gaps, negative for overlaps </returns>
		public Duration Duration
		{
			get
			{
				return Duration.OfSeconds(DurationSeconds);
			}
		}

		/// <summary>
		/// Gets the duration of the transition in seconds.
		/// </summary>
		/// <returns> the duration in seconds </returns>
		private int DurationSeconds
		{
			get
			{
				return OffsetAfter.TotalSeconds - OffsetBefore.TotalSeconds;
			}
		}

		/// <summary>
		/// Does this transition represent a gap in the local time-line.
		/// <para>
		/// Gaps occur where there are local date-times that simply do not exist.
		/// An example would be when the offset changes from {@code +01:00} to {@code +02:00}.
		/// This might be described as 'the clocks will move forward one hour tonight at 1am'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this transition is a gap, false if it is an overlap </returns>
		public bool Gap
		{
			get
			{
				return OffsetAfter.TotalSeconds > OffsetBefore.TotalSeconds;
			}
		}

		/// <summary>
		/// Does this transition represent an overlap in the local time-line.
		/// <para>
		/// Overlaps occur where there are local date-times that exist twice.
		/// An example would be when the offset changes from {@code +02:00} to {@code +01:00}.
		/// This might be described as 'the clocks will move back one hour tonight at 2am'.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this transition is an overlap, false if it is a gap </returns>
		public bool Overlap
		{
			get
			{
				return OffsetAfter.TotalSeconds < OffsetBefore.TotalSeconds;
			}
		}

		/// <summary>
		/// Checks if the specified offset is valid during this transition.
		/// <para>
		/// This checks to see if the given offset will be valid at some point in the transition.
		/// A gap will always return false.
		/// An overlap will return true if the offset is either the before or after offset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to check, null returns false </param>
		/// <returns> true if the offset is valid during the transition </returns>
		public bool IsValidOffset(ZoneOffset offset)
		{
			return Gap ? false : (OffsetBefore.Equals(offset) || OffsetAfter.Equals(offset));
		}

		/// <summary>
		/// Gets the valid offsets during this transition.
		/// <para>
		/// A gap will return an empty list, while an overlap will return both offsets.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the list of valid offsets </returns>
		internal IList<ZoneOffset> ValidOffsets
		{
			get
			{
				if (Gap)
				{
					return Collections.EmptyList();
				}
				return Arrays.asList(OffsetBefore, OffsetAfter);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this transition to another based on the transition instant.
		/// <para>
		/// This compares the instants of each transition.
		/// The offsets are ignored, making this order inconsistent with equals.
		/// 
		/// </para>
		/// </summary>
		/// <param name="transition">  the transition to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public int CompareTo(ZoneOffsetTransition transition)
		{
			return this.Instant.CompareTo(transition.Instant);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this object equals another.
		/// <para>
		/// The entire state of the object is compared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other object to compare to, null returns false </param>
		/// <returns> true if equal </returns>
		public override bool Equals(Object other)
		{
			if (other == this)
			{
				return true;
			}
			if (other is ZoneOffsetTransition)
			{
				ZoneOffsetTransition d = (ZoneOffsetTransition) other;
				return Transition.Equals(d.Transition) && OffsetBefore_Renamed.Equals(d.OffsetBefore_Renamed) && OffsetAfter_Renamed.Equals(d.OffsetAfter_Renamed);
			}
			return false;
		}

		/// <summary>
		/// Returns a suitable hash code.
		/// </summary>
		/// <returns> the hash code </returns>
		public override int HashCode()
		{
			return Transition.HashCode() ^ OffsetBefore_Renamed.HashCode() ^ Integer.RotateLeft(OffsetAfter_Renamed.HashCode(), 16);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a string describing this object.
		/// </summary>
		/// <returns> a string for debugging, not null </returns>
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("Transition[").Append(Gap ? "Gap" : "Overlap").Append(" at ").Append(Transition).Append(OffsetBefore_Renamed).Append(" to ").Append(OffsetAfter_Renamed).Append(']');
			return buf.ToString();
		}

	}

}