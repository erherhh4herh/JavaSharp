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
	/// The rules defining how the zone offset varies for a single time-zone.
	/// <para>
	/// The rules model all the historic and future transitions for a time-zone.
	/// <seealso cref="ZoneOffsetTransition"/> is used for known transitions, typically historic.
	/// <seealso cref="ZoneOffsetTransitionRule"/> is used for future transitions that are based
	/// on the result of an algorithm.
	/// </para>
	/// <para>
	/// The rules are loaded via <seealso cref="ZoneRulesProvider"/> using a <seealso cref="ZoneId"/>.
	/// The same rules may be shared internally between multiple zone IDs.
	/// </para>
	/// <para>
	/// Serializing an instance of {@code ZoneRules} will store the entire set of rules.
	/// It does not store the zone ID as it is not part of the state of this object.
	/// </para>
	/// <para>
	/// A rule implementation may or may not store full information about historic
	/// and future transitions, and the information stored is only as accurate as
	/// that supplied to the implementation by the rules provider.
	/// Applications should treat the data provided as representing the best information
	/// available to the implementation of this rule.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class ZoneRules
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 3044319355680032515L;
		/// <summary>
		/// The last year to have its transitions cached.
		/// </summary>
		private const int LAST_CACHED_YEAR = 2100;

		/// <summary>
		/// The transitions between standard offsets (epoch seconds), sorted.
		/// </summary>
		private readonly long[] StandardTransitions;
		/// <summary>
		/// The standard offsets.
		/// </summary>
		private readonly ZoneOffset[] StandardOffsets;
		/// <summary>
		/// The transitions between instants (epoch seconds), sorted.
		/// </summary>
		private readonly long[] SavingsInstantTransitions;
		/// <summary>
		/// The transitions between local date-times, sorted.
		/// This is a paired array, where the first entry is the start of the transition
		/// and the second entry is the end of the transition.
		/// </summary>
		private readonly LocalDateTime[] SavingsLocalTransitions;
		/// <summary>
		/// The wall offsets.
		/// </summary>
		private readonly ZoneOffset[] WallOffsets;
		/// <summary>
		/// The last rule.
		/// </summary>
		private readonly ZoneOffsetTransitionRule[] LastRules;
		/// <summary>
		/// The map of recent transitions.
		/// </summary>
		[NonSerialized]
		private readonly ConcurrentMap<Integer, ZoneOffsetTransition[]> LastRulesCache = new ConcurrentDictionary<Integer, ZoneOffsetTransition[]>();
		/// <summary>
		/// The zero-length long array.
		/// </summary>
		private static readonly long[] EMPTY_LONG_ARRAY = new long[0];
		/// <summary>
		/// The zero-length lastrules array.
		/// </summary>
		private static readonly ZoneOffsetTransitionRule[] EMPTY_LASTRULES = new ZoneOffsetTransitionRule[0];
		/// <summary>
		/// The zero-length ldt array.
		/// </summary>
		private static readonly LocalDateTime[] EMPTY_LDT_ARRAY = new LocalDateTime[0];

		/// <summary>
		/// Obtains an instance of a ZoneRules.
		/// </summary>
		/// <param name="baseStandardOffset">  the standard offset to use before legal rules were set, not null </param>
		/// <param name="baseWallOffset">  the wall offset to use before legal rules were set, not null </param>
		/// <param name="standardOffsetTransitionList">  the list of changes to the standard offset, not null </param>
		/// <param name="transitionList">  the list of transitions, not null </param>
		/// <param name="lastRules">  the recurring last rules, size 16 or less, not null </param>
		/// <returns> the zone rules, not null </returns>
		public static ZoneRules Of(ZoneOffset baseStandardOffset, ZoneOffset baseWallOffset, IList<ZoneOffsetTransition> standardOffsetTransitionList, IList<ZoneOffsetTransition> transitionList, IList<ZoneOffsetTransitionRule> lastRules)
		{
			Objects.RequireNonNull(baseStandardOffset, "baseStandardOffset");
			Objects.RequireNonNull(baseWallOffset, "baseWallOffset");
			Objects.RequireNonNull(standardOffsetTransitionList, "standardOffsetTransitionList");
			Objects.RequireNonNull(transitionList, "transitionList");
			Objects.RequireNonNull(lastRules, "lastRules");
			return new ZoneRules(baseStandardOffset, baseWallOffset, standardOffsetTransitionList, transitionList, lastRules);
		}

		/// <summary>
		/// Obtains an instance of ZoneRules that has fixed zone rules.
		/// </summary>
		/// <param name="offset">  the offset this fixed zone rules is based on, not null </param>
		/// <returns> the zone rules, not null </returns>
		/// <seealso cref= #isFixedOffset() </seealso>
		public static ZoneRules Of(ZoneOffset offset)
		{
			Objects.RequireNonNull(offset, "offset");
			return new ZoneRules(offset);
		}

		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="baseStandardOffset">  the standard offset to use before legal rules were set, not null </param>
		/// <param name="baseWallOffset">  the wall offset to use before legal rules were set, not null </param>
		/// <param name="standardOffsetTransitionList">  the list of changes to the standard offset, not null </param>
		/// <param name="transitionList">  the list of transitions, not null </param>
		/// <param name="lastRules">  the recurring last rules, size 16 or less, not null </param>
		internal ZoneRules(ZoneOffset baseStandardOffset, ZoneOffset baseWallOffset, IList<ZoneOffsetTransition> standardOffsetTransitionList, IList<ZoneOffsetTransition> transitionList, IList<ZoneOffsetTransitionRule> lastRules) : base()
		{

			// convert standard transitions

			this.StandardTransitions = new long[standardOffsetTransitionList.Count];

			this.StandardOffsets = new ZoneOffset[standardOffsetTransitionList.Count + 1];
			this.StandardOffsets[0] = baseStandardOffset;
			for (int i = 0; i < standardOffsetTransitionList.Count; i++)
			{
				this.StandardTransitions[i] = standardOffsetTransitionList[i].ToEpochSecond();
				this.StandardOffsets[i + 1] = standardOffsetTransitionList[i].OffsetAfter;
			}

			// convert savings transitions to locals
			IList<LocalDateTime> localTransitionList = new List<LocalDateTime>();
			IList<ZoneOffset> localTransitionOffsetList = new List<ZoneOffset>();
			localTransitionOffsetList.Add(baseWallOffset);
			foreach (ZoneOffsetTransition trans in transitionList)
			{
				if (trans.Gap)
				{
					localTransitionList.Add(trans.DateTimeBefore);
					localTransitionList.Add(trans.DateTimeAfter);
				}
				else
				{
					localTransitionList.Add(trans.DateTimeAfter);
					localTransitionList.Add(trans.DateTimeBefore);
				}
				localTransitionOffsetList.Add(trans.OffsetAfter);
			}
			this.SavingsLocalTransitions = localTransitionList.ToArray();
			this.WallOffsets = localTransitionOffsetList.ToArray();

			// convert savings transitions to instants
			this.SavingsInstantTransitions = new long[transitionList.Count];
			for (int i = 0; i < transitionList.Count; i++)
			{
				this.SavingsInstantTransitions[i] = transitionList[i].ToEpochSecond();
			}

			// last rules
			if (lastRules.Count > 16)
			{
				throw new IllegalArgumentException("Too many transition rules");
			}
			this.LastRules = lastRules.ToArray();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="standardTransitions">  the standard transitions, not null </param>
		/// <param name="standardOffsets">  the standard offsets, not null </param>
		/// <param name="savingsInstantTransitions">  the standard transitions, not null </param>
		/// <param name="wallOffsets">  the wall offsets, not null </param>
		/// <param name="lastRules">  the recurring last rules, size 15 or less, not null </param>
		private ZoneRules(long[] standardTransitions, ZoneOffset[] standardOffsets, long[] savingsInstantTransitions, ZoneOffset[] wallOffsets, ZoneOffsetTransitionRule[] lastRules) : base()
		{

			this.StandardTransitions = standardTransitions;
			this.StandardOffsets = standardOffsets;
			this.SavingsInstantTransitions = savingsInstantTransitions;
			this.WallOffsets = wallOffsets;
			this.LastRules = lastRules;

			if (savingsInstantTransitions.Length == 0)
			{
				this.SavingsLocalTransitions = EMPTY_LDT_ARRAY;
			}
			else
			{
				// convert savings transitions to locals
				IList<LocalDateTime> localTransitionList = new List<LocalDateTime>();
				for (int i = 0; i < savingsInstantTransitions.Length; i++)
				{
					ZoneOffset before = wallOffsets[i];
					ZoneOffset after = wallOffsets[i + 1];
					ZoneOffsetTransition trans = new ZoneOffsetTransition(savingsInstantTransitions[i], before, after);
					if (trans.Gap)
					{
						localTransitionList.Add(trans.DateTimeBefore);
						localTransitionList.Add(trans.DateTimeAfter);
					}
					else
					{
						localTransitionList.Add(trans.DateTimeAfter);
						localTransitionList.Add(trans.DateTimeBefore);
					}
				}
				this.SavingsLocalTransitions = localTransitionList.ToArray();
			}
		}

		/// <summary>
		/// Creates an instance of ZoneRules that has fixed zone rules.
		/// </summary>
		/// <param name="offset">  the offset this fixed zone rules is based on, not null </param>
		/// <returns> the zone rules, not null </returns>
		/// <seealso cref= #isFixedOffset() </seealso>
		private ZoneRules(ZoneOffset offset)
		{
			this.StandardOffsets = new ZoneOffset[1];
			this.StandardOffsets[0] = offset;
			this.StandardTransitions = EMPTY_LONG_ARRAY;
			this.SavingsInstantTransitions = EMPTY_LONG_ARRAY;
			this.SavingsLocalTransitions = EMPTY_LDT_ARRAY;
			this.WallOffsets = StandardOffsets;
			this.LastRules = EMPTY_LASTRULES;
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

		/// <summary>
		/// Writes the object using a
		/// <a href="../../../serialized-form.html#java.time.zone.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre style="font-size:1.0em">{@code
		/// 
		///   out.writeByte(1);  // identifies a ZoneRules
		///   out.writeInt(standardTransitions.length);
		///   for (long trans : standardTransitions) {
		///       Ser.writeEpochSec(trans, out);
		///   }
		///   for (ZoneOffset offset : standardOffsets) {
		///       Ser.writeOffset(offset, out);
		///   }
		///   out.writeInt(savingsInstantTransitions.length);
		///   for (long trans : savingsInstantTransitions) {
		///       Ser.writeEpochSec(trans, out);
		///   }
		///   for (ZoneOffset offset : wallOffsets) {
		///       Ser.writeOffset(offset, out);
		///   }
		///   out.writeByte(lastRules.length);
		///   for (ZoneOffsetTransitionRule rule : lastRules) {
		///       rule.writeExternal(out);
		///   }
		/// }
		/// </pre>
		/// <para>
		/// Epoch second values used for offsets are encoded in a variable
		/// length form to make the common cases put fewer bytes in the stream.
		/// <pre style="font-size:1.0em">{@code
		/// 
		///  static void writeEpochSec(long epochSec, DataOutput out) throws IOException {
		///     if (epochSec >= -4575744000L && epochSec < 10413792000L && epochSec % 900 == 0) {  // quarter hours between 1825 and 2300
		///         int store = (int) ((epochSec + 4575744000L) / 900);
		///         out.writeByte((store >>> 16) & 255);
		///         out.writeByte((store >>> 8) & 255);
		///         out.writeByte(store & 255);
		///      } else {
		///          out.writeByte(255);
		///          out.writeLong(epochSec);
		///      }
		///  }
		/// }
		/// </pre>
		/// </para>
		/// <para>
		/// ZoneOffset values are encoded in a variable length form so the
		/// common cases put fewer bytes in the stream.
		/// <pre style="font-size:1.0em">{@code
		/// 
		///  static void writeOffset(ZoneOffset offset, DataOutput out) throws IOException {
		///     final int offsetSecs = offset.getTotalSeconds();
		///     int offsetByte = offsetSecs % 900 == 0 ? offsetSecs / 900 : 127;  // compress to -72 to +72
		///     out.writeByte(offsetByte);
		///     if (offsetByte == 127) {
		///         out.writeInt(offsetSecs);
		///     }
		/// }
		/// }
		/// </pre>
		/// </para>
		/// </summary>
		/// <returns> the replacing object, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZRULES, this);
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
			@out.WriteInt(StandardTransitions.Length);
			foreach (long trans in StandardTransitions)
			{
				Ser.WriteEpochSec(trans, @out);
			}
			foreach (ZoneOffset offset in StandardOffsets)
			{
				Ser.WriteOffset(offset, @out);
			}
			@out.WriteInt(SavingsInstantTransitions.Length);
			foreach (long trans in SavingsInstantTransitions)
			{
				Ser.WriteEpochSec(trans, @out);
			}
			foreach (ZoneOffset offset in WallOffsets)
			{
				Ser.WriteOffset(offset, @out);
			}
			@out.WriteByte(LastRules.Length);
			foreach (ZoneOffsetTransitionRule rule in LastRules)
			{
				rule.WriteExternal(@out);
			}
		}

		/// <summary>
		/// Reads the state from the stream.
		/// </summary>
		/// <param name="in">  the input stream, not null </param>
		/// <returns> the created object, not null </returns>
		/// <exception cref="IOException"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZoneRules readExternal(java.io.DataInput in) throws java.io.IOException, ClassNotFoundException
		internal static ZoneRules ReadExternal(DataInput @in)
		{
			int stdSize = @in.ReadInt();
			long[] stdTrans = (stdSize == 0) ? EMPTY_LONG_ARRAY : new long[stdSize];
			for (int i = 0; i < stdSize; i++)
			{
				stdTrans[i] = Ser.ReadEpochSec(@in);
			}
			ZoneOffset[] stdOffsets = new ZoneOffset[stdSize + 1];
			for (int i = 0; i < stdOffsets.Length; i++)
			{
				stdOffsets[i] = Ser.ReadOffset(@in);
			}
			int savSize = @in.ReadInt();
			long[] savTrans = (savSize == 0) ? EMPTY_LONG_ARRAY : new long[savSize];
			for (int i = 0; i < savSize; i++)
			{
				savTrans[i] = Ser.ReadEpochSec(@in);
			}
			ZoneOffset[] savOffsets = new ZoneOffset[savSize + 1];
			for (int i = 0; i < savOffsets.Length; i++)
			{
				savOffsets[i] = Ser.ReadOffset(@in);
			}
			int ruleSize = @in.ReadByte();
			ZoneOffsetTransitionRule[] rules = (ruleSize == 0) ? EMPTY_LASTRULES : new ZoneOffsetTransitionRule[ruleSize];
			for (int i = 0; i < ruleSize; i++)
			{
				rules[i] = ZoneOffsetTransitionRule.ReadExternal(@in);
			}
			return new ZoneRules(stdTrans, stdOffsets, savTrans, savOffsets, rules);
		}

		/// <summary>
		/// Checks of the zone rules are fixed, such that the offset never varies.
		/// </summary>
		/// <returns> true if the time-zone is fixed and the offset never changes </returns>
		public bool FixedOffset
		{
			get
			{
				return SavingsInstantTransitions.Length == 0;
			}
		}

		/// <summary>
		/// Gets the offset applicable at the specified instant in these rules.
		/// <para>
		/// The mapping from an instant to an offset is simple, there is only
		/// one valid offset for each instant.
		/// This method returns that offset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to find the offset for, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the offset, not null </returns>
		public ZoneOffset GetOffset(Instant instant)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return StandardOffsets[0];
			}
			long epochSec = instant.EpochSecond;
			// check if using last rules
			if (LastRules.Length > 0 && epochSec > SavingsInstantTransitions[SavingsInstantTransitions.Length - 1])
			{
				int year = FindYear(epochSec, WallOffsets[WallOffsets.Length - 1]);
				ZoneOffsetTransition[] transArray = FindTransitionArray(year);
				ZoneOffsetTransition trans = null;
				for (int i = 0; i < transArray.Length; i++)
				{
					trans = transArray[i];
					if (epochSec < trans.ToEpochSecond())
					{
						return trans.OffsetBefore;
					}
				}
				return trans.OffsetAfter;
			}

			// using historic rules
			int index = Arrays.BinarySearch(SavingsInstantTransitions, epochSec);
			if (index < 0)
			{
				// switch negative insert position to start of matched range
				index = -index - 2;
			}
			return WallOffsets[index + 1];
		}

		/// <summary>
		/// Gets a suitable offset for the specified local date-time in these rules.
		/// <para>
		/// The mapping from a local date-time to an offset is not straightforward.
		/// There are three cases:
		/// <ul>
		/// <li>Normal, with one valid offset. For the vast majority of the year, the normal
		///  case applies, where there is a single valid offset for the local date-time.</li>
		/// <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		///  due to the spring daylight savings change from "winter" to "summer".
		///  In a gap there are local date-time values with no valid offset.</li>
		/// <li>Overlap, with two valid offsets. This is when clocks are set back typically
		///  due to the autumn daylight savings change from "summer" to "winter".
		///  In an overlap there are local date-time values with two valid offsets.</li>
		/// </ul>
		/// Thus, for any given local date-time there can be zero, one or two valid offsets.
		/// This method returns the single offset in the Normal case, and in the Gap or Overlap
		/// case it returns the offset before the transition.
		/// </para>
		/// <para>
		/// Since, in the case of Gap and Overlap, the offset returned is a "best" value, rather
		/// than the "correct" value, it should be treated with care. Applications that care
		/// about the correct offset should use a combination of this method,
		/// <seealso cref="#getValidOffsets(LocalDateTime)"/> and <seealso cref="#getTransition(LocalDateTime)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time to query, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the best available offset for the local date-time, not null </returns>
		public ZoneOffset GetOffset(LocalDateTime localDateTime)
		{
			Object info = GetOffsetInfo(localDateTime);
			if (info is ZoneOffsetTransition)
			{
				return ((ZoneOffsetTransition) info).OffsetBefore;
			}
			return (ZoneOffset) info;
		}

		/// <summary>
		/// Gets the offset applicable at the specified local date-time in these rules.
		/// <para>
		/// The mapping from a local date-time to an offset is not straightforward.
		/// There are three cases:
		/// <ul>
		/// <li>Normal, with one valid offset. For the vast majority of the year, the normal
		///  case applies, where there is a single valid offset for the local date-time.</li>
		/// <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		///  due to the spring daylight savings change from "winter" to "summer".
		///  In a gap there are local date-time values with no valid offset.</li>
		/// <li>Overlap, with two valid offsets. This is when clocks are set back typically
		///  due to the autumn daylight savings change from "summer" to "winter".
		///  In an overlap there are local date-time values with two valid offsets.</li>
		/// </ul>
		/// Thus, for any given local date-time there can be zero, one or two valid offsets.
		/// This method returns that list of valid offsets, which is a list of size 0, 1 or 2.
		/// In the case where there are two offsets, the earlier offset is returned at index 0
		/// and the later offset at index 1.
		/// </para>
		/// <para>
		/// There are various ways to handle the conversion from a {@code LocalDateTime}.
		/// One technique, using this method, would be:
		/// <pre>
		///  List&lt;ZoneOffset&gt; validOffsets = rules.getOffset(localDT);
		///  if (validOffsets.size() == 1) {
		///    // Normal case: only one valid offset
		///    zoneOffset = validOffsets.get(0);
		///  } else {
		///    // Gap or Overlap: determine what to do from transition (which will be non-null)
		///    ZoneOffsetTransition trans = rules.getTransition(localDT);
		///  }
		/// </pre>
		/// </para>
		/// <para>
		/// In theory, it is possible for there to be more than two valid offsets.
		/// This would happen if clocks to be put back more than once in quick succession.
		/// This has never happened in the history of time-zones and thus has no special handling.
		/// However, if it were to happen, then the list would return more than 2 entries.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time to query for valid offsets, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the list of valid offsets, may be immutable, not null </returns>
		public IList<ZoneOffset> GetValidOffsets(LocalDateTime localDateTime)
		{
			// should probably be optimized
			Object info = GetOffsetInfo(localDateTime);
			if (info is ZoneOffsetTransition)
			{
				return ((ZoneOffsetTransition) info).ValidOffsets;
			}
			return Collections.SingletonList((ZoneOffset) info);
		}

		/// <summary>
		/// Gets the offset transition applicable at the specified local date-time in these rules.
		/// <para>
		/// The mapping from a local date-time to an offset is not straightforward.
		/// There are three cases:
		/// <ul>
		/// <li>Normal, with one valid offset. For the vast majority of the year, the normal
		///  case applies, where there is a single valid offset for the local date-time.</li>
		/// <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		///  due to the spring daylight savings change from "winter" to "summer".
		///  In a gap there are local date-time values with no valid offset.</li>
		/// <li>Overlap, with two valid offsets. This is when clocks are set back typically
		///  due to the autumn daylight savings change from "summer" to "winter".
		///  In an overlap there are local date-time values with two valid offsets.</li>
		/// </ul>
		/// A transition is used to model the cases of a Gap or Overlap.
		/// The Normal case will return null.
		/// </para>
		/// <para>
		/// There are various ways to handle the conversion from a {@code LocalDateTime}.
		/// One technique, using this method, would be:
		/// <pre>
		///  ZoneOffsetTransition trans = rules.getTransition(localDT);
		///  if (trans == null) {
		///    // Gap or Overlap: determine what to do from transition
		///  } else {
		///    // Normal case: only one valid offset
		///    zoneOffset = rule.getOffset(localDT);
		///  }
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the local date-time to query for offset transition, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the offset transition, null if the local date-time is not in transition </returns>
		public ZoneOffsetTransition GetTransition(LocalDateTime localDateTime)
		{
			Object info = GetOffsetInfo(localDateTime);
			return (info is ZoneOffsetTransition ? (ZoneOffsetTransition) info : null);
		}

		private Object GetOffsetInfo(LocalDateTime dt)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return StandardOffsets[0];
			}
			// check if using last rules
			if (LastRules.Length > 0 && dt.IsAfter(SavingsLocalTransitions[SavingsLocalTransitions.Length - 1]))
			{
				ZoneOffsetTransition[] transArray = FindTransitionArray(dt.Year);
				Object info = null;
				foreach (ZoneOffsetTransition trans in transArray)
				{
					info = FindOffsetInfo(dt, trans);
					if (info is ZoneOffsetTransition || info.Equals(trans.OffsetBefore))
					{
						return info;
					}
				}
				return info;
			}

			// using historic rules
			int index = Arrays.BinarySearch(SavingsLocalTransitions, dt);
			if (index == -1)
			{
				// before first transition
				return WallOffsets[0];
			}
			if (index < 0)
			{
				// switch negative insert position to start of matched range
				index = -index - 2;
			}
			else if (index < SavingsLocalTransitions.Length - 1 && SavingsLocalTransitions[index].Equals(SavingsLocalTransitions[index + 1]))
			{
				// handle overlap immediately following gap
				index++;
			}
			if ((index & 1) == 0)
			{
				// gap or overlap
				LocalDateTime dtBefore = SavingsLocalTransitions[index];
				LocalDateTime dtAfter = SavingsLocalTransitions[index + 1];
				ZoneOffset offsetBefore = WallOffsets[index / 2];
				ZoneOffset offsetAfter = WallOffsets[index / 2 + 1];
				if (offsetAfter.TotalSeconds > offsetBefore.TotalSeconds)
				{
					// gap
					return new ZoneOffsetTransition(dtBefore, offsetBefore, offsetAfter);
				}
				else
				{
					// overlap
					return new ZoneOffsetTransition(dtAfter, offsetBefore, offsetAfter);
				}
			}
			else
			{
				// normal (neither gap or overlap)
				return WallOffsets[index / 2 + 1];
			}
		}

		/// <summary>
		/// Finds the offset info for a local date-time and transition.
		/// </summary>
		/// <param name="dt">  the date-time, not null </param>
		/// <param name="trans">  the transition, not null </param>
		/// <returns> the offset info, not null </returns>
		private Object FindOffsetInfo(LocalDateTime dt, ZoneOffsetTransition trans)
		{
			LocalDateTime localTransition = trans.DateTimeBefore;
			if (trans.Gap)
			{
				if (dt.IsBefore(localTransition))
				{
					return trans.OffsetBefore;
				}
				if (dt.IsBefore(trans.DateTimeAfter))
				{
					return trans;
				}
				else
				{
					return trans.OffsetAfter;
				}
			}
			else
			{
				if (dt.IsBefore(localTransition) == false)
				{
					return trans.OffsetAfter;
				}
				if (dt.IsBefore(trans.DateTimeAfter))
				{
					return trans.OffsetBefore;
				}
				else
				{
					return trans;
				}
			}
		}

		/// <summary>
		/// Finds the appropriate transition array for the given year.
		/// </summary>
		/// <param name="year">  the year, not null </param>
		/// <returns> the transition array, not null </returns>
		private ZoneOffsetTransition[] FindTransitionArray(int year)
		{
			Integer yearObj = year; // should use Year class, but this saves a class load
			ZoneOffsetTransition[] transArray = LastRulesCache[yearObj];
			if (transArray != null)
			{
				return transArray;
			}
			ZoneOffsetTransitionRule[] ruleArray = LastRules;
			transArray = new ZoneOffsetTransition[ruleArray.Length];
			for (int i = 0; i < ruleArray.Length; i++)
			{
				transArray[i] = ruleArray[i].CreateTransition(year);
			}
			if (year < LAST_CACHED_YEAR)
			{
				LastRulesCache.PutIfAbsent(yearObj, transArray);
			}
			return transArray;
		}

		/// <summary>
		/// Gets the standard offset for the specified instant in this zone.
		/// <para>
		/// This provides access to historic information on how the standard offset
		/// has changed over time.
		/// The standard offset is the offset before any daylight saving time is applied.
		/// This is typically the offset applicable during winter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to find the offset information for, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the standard offset, not null </returns>
		public ZoneOffset GetStandardOffset(Instant instant)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return StandardOffsets[0];
			}
			long epochSec = instant.EpochSecond;
			int index = Arrays.BinarySearch(StandardTransitions, epochSec);
			if (index < 0)
			{
				// switch negative insert position to start of matched range
				index = -index - 2;
			}
			return StandardOffsets[index + 1];
		}

		/// <summary>
		/// Gets the amount of daylight savings in use for the specified instant in this zone.
		/// <para>
		/// This provides access to historic information on how the amount of daylight
		/// savings has changed over time.
		/// This is the difference between the standard offset and the actual offset.
		/// Typically the amount is zero during winter and one hour during summer.
		/// Time-zones are second-based, so the nanosecond part of the duration will be zero.
		/// </para>
		/// <para>
		/// This default implementation calculates the duration from the
		/// <seealso cref="#getOffset(java.time.Instant) actual"/> and
		/// <seealso cref="#getStandardOffset(java.time.Instant) standard"/> offsets.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to find the daylight savings for, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the difference between the standard and actual offset, not null </returns>
		public Duration GetDaylightSavings(Instant instant)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return Duration.ZERO;
			}
			ZoneOffset standardOffset = GetStandardOffset(instant);
			ZoneOffset actualOffset = GetOffset(instant);
			return Duration.OfSeconds(actualOffset.TotalSeconds - standardOffset.TotalSeconds);
		}

		/// <summary>
		/// Checks if the specified instant is in daylight savings.
		/// <para>
		/// This checks if the standard offset and the actual offset are the same
		/// for the specified instant.
		/// If they are not, it is assumed that daylight savings is in operation.
		/// </para>
		/// <para>
		/// This default implementation compares the <seealso cref="#getOffset(java.time.Instant) actual"/>
		/// and <seealso cref="#getStandardOffset(java.time.Instant) standard"/> offsets.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to find the offset information for, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the standard offset, not null </returns>
		public bool IsDaylightSavings(Instant instant)
		{
			return (GetStandardOffset(instant).Equals(GetOffset(instant)) == false);
		}

		/// <summary>
		/// Checks if the offset date-time is valid for these rules.
		/// <para>
		/// To be valid, the local date-time must not be in a gap and the offset
		/// must match one of the valid offsets.
		/// </para>
		/// <para>
		/// This default implementation checks if <seealso cref="#getValidOffsets(java.time.LocalDateTime)"/>
		/// contains the specified offset.
		/// 
		/// </para>
		/// </summary>
		/// <param name="localDateTime">  the date-time to check, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <param name="offset">  the offset to check, null returns false </param>
		/// <returns> true if the offset date-time is valid for these rules </returns>
		public bool IsValidOffset(LocalDateTime localDateTime, ZoneOffset offset)
		{
			return GetValidOffsets(localDateTime).Contains(offset);
		}

		/// <summary>
		/// Gets the next transition after the specified instant.
		/// <para>
		/// This returns details of the next transition after the specified instant.
		/// For example, if the instant represents a point where "Summer" daylight savings time
		/// applies, then the method will return the transition to the next "Winter" time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to get the next transition after, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the next transition after the specified instant, null if this is after the last transition </returns>
		public ZoneOffsetTransition NextTransition(Instant instant)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return null;
			}
			long epochSec = instant.EpochSecond;
			// check if using last rules
			if (epochSec >= SavingsInstantTransitions[SavingsInstantTransitions.Length - 1])
			{
				if (LastRules.Length == 0)
				{
					return null;
				}
				// search year the instant is in
				int year = FindYear(epochSec, WallOffsets[WallOffsets.Length - 1]);
				ZoneOffsetTransition[] transArray = FindTransitionArray(year);
				foreach (ZoneOffsetTransition trans in transArray)
				{
					if (epochSec < trans.ToEpochSecond())
					{
						return trans;
					}
				}
				// use first from following year
				if (year < Year.MAX_VALUE)
				{
					transArray = FindTransitionArray(year + 1);
					return transArray[0];
				}
				return null;
			}

			// using historic rules
			int index = Arrays.BinarySearch(SavingsInstantTransitions, epochSec);
			if (index < 0)
			{
				index = -index - 1; // switched value is the next transition
			}
			else
			{
				index += 1; // exact match, so need to add one to get the next
			}
			return new ZoneOffsetTransition(SavingsInstantTransitions[index], WallOffsets[index], WallOffsets[index + 1]);
		}

		/// <summary>
		/// Gets the previous transition before the specified instant.
		/// <para>
		/// This returns details of the previous transition after the specified instant.
		/// For example, if the instant represents a point where "summer" daylight saving time
		/// applies, then the method will return the transition from the previous "winter" time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to get the previous transition after, not null, but null
		///  may be ignored if the rules have a single offset for all instants </param>
		/// <returns> the previous transition after the specified instant, null if this is before the first transition </returns>
		public ZoneOffsetTransition PreviousTransition(Instant instant)
		{
			if (SavingsInstantTransitions.Length == 0)
			{
				return null;
			}
			long epochSec = instant.EpochSecond;
			if (instant.Nano > 0 && epochSec < Long.MaxValue)
			{
				epochSec += 1; // allow rest of method to only use seconds
			}

			// check if using last rules
			long lastHistoric = SavingsInstantTransitions[SavingsInstantTransitions.Length - 1];
			if (LastRules.Length > 0 && epochSec > lastHistoric)
			{
				// search year the instant is in
				ZoneOffset lastHistoricOffset = WallOffsets[WallOffsets.Length - 1];
				int year = FindYear(epochSec, lastHistoricOffset);
				ZoneOffsetTransition[] transArray = FindTransitionArray(year);
				for (int i = transArray.Length - 1; i >= 0; i--)
				{
					if (epochSec > transArray[i].ToEpochSecond())
					{
						return transArray[i];
					}
				}
				// use last from preceding year
				int lastHistoricYear = FindYear(lastHistoric, lastHistoricOffset);
				if (--year > lastHistoricYear)
				{
					transArray = FindTransitionArray(year);
					return transArray[transArray.Length - 1];
				}
				// drop through
			}

			// using historic rules
			int index = Arrays.BinarySearch(SavingsInstantTransitions, epochSec);
			if (index < 0)
			{
				index = -index - 1;
			}
			if (index <= 0)
			{
				return null;
			}
			return new ZoneOffsetTransition(SavingsInstantTransitions[index - 1], WallOffsets[index - 1], WallOffsets[index]);
		}

		private int FindYear(long epochSecond, ZoneOffset offset)
		{
			// inline for performance
			long localSecond = epochSecond + offset.TotalSeconds;
			long localEpochDay = Math.FloorDiv(localSecond, 86400);
			return LocalDate.OfEpochDay(localEpochDay).Year;
		}

		/// <summary>
		/// Gets the complete list of fully defined transitions.
		/// <para>
		/// The complete set of transitions for this rules instance is defined by this method
		/// and <seealso cref="#getTransitionRules()"/>. This method returns those transitions that have
		/// been fully defined. These are typically historical, but may be in the future.
		/// </para>
		/// <para>
		/// The list will be empty for fixed offset rules and for any time-zone where there has
		/// only ever been a single offset. The list will also be empty if the transition rules are unknown.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an immutable list of fully defined transitions, not null </returns>
		public IList<ZoneOffsetTransition> Transitions
		{
			get
			{
				IList<ZoneOffsetTransition> list = new List<ZoneOffsetTransition>();
				for (int i = 0; i < SavingsInstantTransitions.Length; i++)
				{
					list.Add(new ZoneOffsetTransition(SavingsInstantTransitions[i], WallOffsets[i], WallOffsets[i + 1]));
				}
				return Collections.UnmodifiableList(list);
			}
		}

		/// <summary>
		/// Gets the list of transition rules for years beyond those defined in the transition list.
		/// <para>
		/// The complete set of transitions for this rules instance is defined by this method
		/// and <seealso cref="#getTransitions()"/>. This method returns instances of <seealso cref="ZoneOffsetTransitionRule"/>
		/// that define an algorithm for when transitions will occur.
		/// </para>
		/// <para>
		/// For any given {@code ZoneRules}, this list contains the transition rules for years
		/// beyond those years that have been fully defined. These rules typically refer to future
		/// daylight saving time rule changes.
		/// </para>
		/// <para>
		/// If the zone defines daylight savings into the future, then the list will normally
		/// be of size two and hold information about entering and exiting daylight savings.
		/// If the zone does not have daylight savings, or information about future changes
		/// is uncertain, then the list will be empty.
		/// </para>
		/// <para>
		/// The list will be empty for fixed offset rules and for any time-zone where there is no
		/// daylight saving time. The list will also be empty if the transition rules are unknown.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an immutable list of transition rules, not null </returns>
		public IList<ZoneOffsetTransitionRule> TransitionRules
		{
			get
			{
				return Collections.UnmodifiableList(Arrays.AsList(LastRules));
			}
		}

		/// <summary>
		/// Checks if this set of rules equals another.
		/// <para>
		/// Two rule sets are equal if they will always result in the same output
		/// for any given input instant or local date-time.
		/// Rules from two different groups may return false even if they are in fact the same.
		/// </para>
		/// <para>
		/// This definition should result in implementations comparing their entire state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="otherRules">  the other rules, null returns false </param>
		/// <returns> true if this rules is the same as that specified </returns>
		public override bool Equals(Object otherRules)
		{
			if (this == otherRules)
			{
			   return true;
			}
			if (otherRules is ZoneRules)
			{
				ZoneRules other = (ZoneRules) otherRules;
				return Arrays.Equals(StandardTransitions, other.StandardTransitions) && Arrays.Equals(StandardOffsets, other.StandardOffsets) && Arrays.Equals(SavingsInstantTransitions, other.SavingsInstantTransitions) && Arrays.Equals(WallOffsets, other.WallOffsets) && Arrays.Equals(LastRules, other.LastRules);
			}
			return false;
		}

		/// <summary>
		/// Returns a suitable hash code given the definition of {@code #equals}.
		/// </summary>
		/// <returns> the hash code </returns>
		public override int HashCode()
		{
			return Arrays.HashCode(StandardTransitions) ^ Arrays.HashCode(StandardOffsets) ^ Arrays.HashCode(SavingsInstantTransitions) ^ Arrays.HashCode(WallOffsets) ^ Arrays.HashCode(LastRules);
		}

		/// <summary>
		/// Returns a string describing this object.
		/// </summary>
		/// <returns> a string for debugging, not null </returns>
		public override String ToString()
		{
			return "ZoneRules[currentStandardOffset=" + StandardOffsets[StandardOffsets.Length - 1] + "]";
		}

	}

}