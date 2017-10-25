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
	/// Loads time-zone rules for 'TZDB'.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class TzdbZoneRulesProvider : ZoneRulesProvider
	{

		/// <summary>
		/// All the regions that are available.
		/// </summary>
		private IList<String> RegionIds;
		/// <summary>
		/// Version Id of this tzdb rules
		/// </summary>
		private String VersionId;
		/// <summary>
		/// Region to rules mapping
		/// </summary>
		private readonly IDictionary<String, Object> RegionToRules = new ConcurrentDictionary<String, Object>();

		/// <summary>
		/// Creates an instance.
		/// Created by the {@code ServiceLoader}.
		/// </summary>
		/// <exception cref="ZoneRulesException"> if unable to load </exception>
		public TzdbZoneRulesProvider()
		{
			try
			{
				String libDir = System.getProperty("java.home") + File.separator + "lib";
				using (DataInputStream dis = new DataInputStream(new BufferedInputStream(new FileInputStream(new File(libDir, "tzdb.dat")))))
				{
					Load(dis);
				}
			}
			catch (Exception ex)
			{
				throw new ZoneRulesException("Unable to load TZDB time-zone rules", ex);
			}
		}

		protected internal override Set<String> ProvideZoneIds()
		{
			return new HashSet<>(RegionIds);
		}

		protected internal override ZoneRules ProvideRules(String zoneId, bool forCaching)
		{
			// forCaching flag is ignored because this is not a dynamic provider
			Object obj = RegionToRules[zoneId];
			if (obj == null)
			{
				throw new ZoneRulesException("Unknown time-zone ID: " + zoneId);
			}
			try
			{
				if (obj is sbyte[])
				{
					sbyte[] bytes = (sbyte[]) obj;
					DataInputStream dis = new DataInputStream(new ByteArrayInputStream(bytes));
					obj = Ser.Read(dis);
					RegionToRules[zoneId] = obj;
				}
				return (ZoneRules) obj;
			}
			catch (Exception ex)
			{
				throw new ZoneRulesException("Invalid binary time-zone data: TZDB:" + zoneId + ", version: " + VersionId, ex);
			}
		}

		protected internal override NavigableMap<String, ZoneRules> ProvideVersions(String zoneId)
		{
			SortedDictionary<String, ZoneRules> map = new SortedDictionary<String, ZoneRules>();
			ZoneRules rules = GetRules(zoneId, false);
			if (rules != null)
			{
				map[VersionId] = rules;
			}
			return map;
		}

		/// <summary>
		/// Loads the rules from a DateInputStream, often in a jar file.
		/// </summary>
		/// <param name="dis">  the DateInputStream to load, not null </param>
		/// <exception cref="Exception"> if an error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void load(java.io.DataInputStream dis) throws Exception
		private void Load(DataInputStream dis)
		{
			if (dis.ReadByte() != 1)
			{
				throw new StreamCorruptedException("File format not recognised");
			}
			// group
			String groupId = dis.ReadUTF();
			if ("TZDB".Equals(groupId) == false)
			{
				throw new StreamCorruptedException("File format not recognised");
			}
			// versions
			int versionCount = dis.ReadShort();
			for (int i = 0; i < versionCount; i++)
			{
				VersionId = dis.ReadUTF();
			}
			// regions
			int regionCount = dis.ReadShort();
			String[] regionArray = new String[regionCount];
			for (int i = 0; i < regionCount; i++)
			{
				regionArray[i] = dis.ReadUTF();
			}
			RegionIds = Arrays.AsList(regionArray);
			// rules
			int ruleCount = dis.ReadShort();
			Object[] ruleArray = new Object[ruleCount];
			for (int i = 0; i < ruleCount; i++)
			{
				sbyte[] bytes = new sbyte[dis.ReadShort()];
				dis.ReadFully(bytes);
				ruleArray[i] = bytes;
			}
			// link version-region-rules
			for (int i = 0; i < versionCount; i++)
			{
				int versionRegionCount = dis.ReadShort();
				RegionToRules.Clear();
				for (int j = 0; j < versionRegionCount; j++)
				{
					String region = regionArray[dis.ReadShort()];
					Object rule = ruleArray[dis.ReadShort() & 0xffff];
					RegionToRules[region] = rule;
				}
			}
		}

		public override String ToString()
		{
			return "TZDB[" + VersionId + "]";
		}
	}

}