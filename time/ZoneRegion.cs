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
namespace java.time
{


	/// <summary>
	/// A geographical region where the same time-zone rules apply.
	/// <para>
	/// Time-zone information is categorized as a set of rules defining when and
	/// how the offset from UTC/Greenwich changes. These rules are accessed using
	/// identifiers based on geographical regions, such as countries or states.
	/// The most common region classification is the Time Zone Database (TZDB),
	/// which defines regions such as 'Europe/Paris' and 'Asia/Tokyo'.
	/// </para>
	/// <para>
	/// The region identifier, modeled by this class, is distinct from the
	/// underlying rules, modeled by <seealso cref="ZoneRules"/>.
	/// The rules are defined by governments and change frequently.
	/// By contrast, the region identifier is well-defined and long-lived.
	/// This separation also allows rules to be shared between regions if appropriate.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	internal sealed class ZoneRegion : ZoneId
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 8386373296231747096L;
		/// <summary>
		/// The time-zone ID, not null.
		/// </summary>
		private readonly String Id_Renamed;
		/// <summary>
		/// The time-zone rules, null if zone ID was loaded leniently.
		/// </summary>
		[NonSerialized]
		private readonly ZoneRules Rules_Renamed;

		/// <summary>
		/// Obtains an instance of {@code ZoneId} from an identifier.
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <param name="checkAvailable">  whether to check if the zone ID is available </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the ID format is invalid </exception>
		/// <exception cref="ZoneRulesException"> if checking availability and the ID cannot be found </exception>
		internal static ZoneRegion OfId(String zoneId, bool checkAvailable)
		{
			Objects.RequireNonNull(zoneId, "zoneId");
			CheckName(zoneId);
			ZoneRules rules = null;
			try
			{
				// always attempt load for better behavior after deserialization
				rules = ZoneRulesProvider.GetRules(zoneId, true);
			}
			catch (ZoneRulesException ex)
			{
				if (checkAvailable)
				{
					throw ex;
				}
			}
			return new ZoneRegion(zoneId, rules);
		}

		/// <summary>
		/// Checks that the given string is a legal ZondId name.
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <exception cref="DateTimeException"> if the ID format is invalid </exception>
		private static void CheckName(String zoneId)
		{
			int n = zoneId.Length();
			if (n < 2)
			{
			   throw new DateTimeException("Invalid ID for region-based ZoneId, invalid format: " + zoneId);
			}
			for (int i = 0; i < n; i++)
			{
				char c = zoneId.CharAt(i);
				if (c >= 'a' && c <= 'z')
				{
					continue;
				}
				if (c >= 'A' && c <= 'Z')
				{
					continue;
				}
				if (c == '/' && i != 0)
				{
					continue;
				}
				if (c >= '0' && c <= '9' && i != 0)
				{
					continue;
				}
				if (c == '~' && i != 0)
				{
					continue;
				}
				if (c == '.' && i != 0)
				{
					continue;
				}
				if (c == '_' && i != 0)
				{
					continue;
				}
				if (c == '+' && i != 0)
				{
					continue;
				}
				if (c == '-' && i != 0)
				{
					continue;
				}
				throw new DateTimeException("Invalid ID for region-based ZoneId, invalid format: " + zoneId);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="id">  the time-zone ID, not null </param>
		/// <param name="rules">  the rules, null for lazy lookup </param>
		internal ZoneRegion(String id, ZoneRules rules)
		{
			this.Id_Renamed = id;
			this.Rules_Renamed = rules;
		}

		//-----------------------------------------------------------------------
		public override String Id
		{
			get
			{
				return Id_Renamed;
			}
		}

		public override ZoneRules Rules
		{
			get
			{
				// additional query for group provider when null allows for possibility
				// that the provider was updated after the ZoneId was created
				return (Rules_Renamed != null ? Rules_Renamed : ZoneRulesProvider.GetRules(Id_Renamed, false));
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(7);  // identifies a ZoneId (not ZoneOffset)
		///  out.writeUTF(zoneId);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.ZONE_REGION_TYPE, this);
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
//ORIGINAL LINE: @Override void write(java.io.DataOutput out) throws java.io.IOException
		internal override void Write(DataOutput @out)
		{
			@out.WriteByte(Ser.ZONE_REGION_TYPE);
			WriteExternal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			@out.WriteUTF(Id_Renamed);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ZoneId readExternal(java.io.DataInput in) throws java.io.IOException
		internal static ZoneId ReadExternal(DataInput @in)
		{
			String id = @in.ReadUTF();
			return ZoneId.Of(id, false);
		}

	}

}