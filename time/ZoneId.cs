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
	/// A time-zone ID, such as {@code Europe/Paris}.
	/// <para>
	/// A {@code ZoneId} is used to identify the rules used to convert between
	/// an <seealso cref="Instant"/> and a <seealso cref="LocalDateTime"/>.
	/// There are two distinct types of ID:
	/// <ul>
	/// <li>Fixed offsets - a fully resolved offset from UTC/Greenwich, that uses
	///  the same offset for all local date-times
	/// <li>Geographical regions - an area where a specific set of rules for finding
	///  the offset from UTC/Greenwich apply
	/// </ul>
	/// Most fixed offsets are represented by <seealso cref="ZoneOffset"/>.
	/// Calling <seealso cref="#normalized()"/> on any {@code ZoneId} will ensure that a
	/// fixed offset ID will be represented as a {@code ZoneOffset}.
	/// </para>
	/// <para>
	/// The actual rules, describing when and how the offset changes, are defined by <seealso cref="ZoneRules"/>.
	/// This class is simply an ID used to obtain the underlying rules.
	/// This approach is taken because rules are defined by governments and change
	/// frequently, whereas the ID is stable.
	/// </para>
	/// <para>
	/// The distinction has other effects. Serializing the {@code ZoneId} will only send
	/// the ID, whereas serializing the rules sends the entire data set.
	/// Similarly, a comparison of two IDs only examines the ID, whereas
	/// a comparison of two rules examines the entire data set.
	/// 
	/// <h3>Time-zone IDs</h3>
	/// The ID is unique within the system.
	/// There are three types of ID.
	/// </para>
	/// <para>
	/// The simplest type of ID is that from {@code ZoneOffset}.
	/// This consists of 'Z' and IDs starting with '+' or '-'.
	/// </para>
	/// <para>
	/// The next type of ID are offset-style IDs with some form of prefix,
	/// such as 'GMT+2' or 'UTC+01:00'.
	/// The recognised prefixes are 'UTC', 'GMT' and 'UT'.
	/// The offset is the suffix and will be normalized during creation.
	/// These IDs can be normalized to a {@code ZoneOffset} using {@code normalized()}.
	/// </para>
	/// <para>
	/// The third type of ID are region-based IDs. A region-based ID must be of
	/// two or more characters, and not start with 'UTC', 'GMT', 'UT' '+' or '-'.
	/// Region-based IDs are defined by configuration, see <seealso cref="ZoneRulesProvider"/>.
	/// The configuration focuses on providing the lookup from the ID to the
	/// underlying {@code ZoneRules}.
	/// </para>
	/// <para>
	/// Time-zone rules are defined by governments and change frequently.
	/// There are a number of organizations, known here as groups, that monitor
	/// time-zone changes and collate them.
	/// The default group is the IANA Time Zone Database (TZDB).
	/// Other organizations include IATA (the airline industry body) and Microsoft.
	/// </para>
	/// <para>
	/// Each group defines its own format for the region ID it provides.
	/// The TZDB group defines IDs such as 'Europe/London' or 'America/New_York'.
	/// TZDB IDs take precedence over other groups.
	/// </para>
	/// <para>
	/// It is strongly recommended that the group name is included in all IDs supplied by
	/// groups other than TZDB to avoid conflicts. For example, IATA airline time-zone
	/// region IDs are typically the same as the three letter airport code.
	/// However, the airport of Utrecht has the code 'UTC', which is obviously a conflict.
	/// The recommended format for region IDs from groups other than TZDB is 'group~region'.
	/// Thus if IATA data were defined, Utrecht airport would be 'IATA~UTC'.
	/// 
	/// <h3>Serialization</h3>
	/// This class can be serialized and stores the string zone ID in the external form.
	/// The {@code ZoneOffset} subclass uses a dedicated format that only stores the
	/// offset from UTC/Greenwich.
	/// </para>
	/// <para>
	/// A {@code ZoneId} can be deserialized in a Java Runtime where the ID is unknown.
	/// For example, if a server-side Java Runtime has been updated with a new zone ID, but
	/// the client-side Java Runtime has not been updated. In this case, the {@code ZoneId}
	/// object will exist, and can be queried using {@code getId}, {@code equals},
	/// {@code hashCode}, {@code toString}, {@code getDisplayName} and {@code normalized}.
	/// However, any call to {@code getRules} will fail with {@code ZoneRulesException}.
	/// This approach is designed to allow a <seealso cref="ZonedDateTime"/> to be loaded and
	/// queried, but not modified, on a Java Runtime with incomplete time-zone information.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code ZoneId} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This abstract class has two implementations, both of which are immutable and thread-safe.
	/// One implementation models region-based IDs, the other is {@code ZoneOffset} modelling
	/// offset-based IDs. This difference is visible in serialization.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public abstract class ZoneId
	{

		/// <summary>
		/// A map of zone overrides to enable the short time-zone names to be used.
		/// <para>
		/// Use of short zone IDs has been deprecated in {@code java.util.TimeZone}.
		/// This map allows the IDs to continue to be used via the
		/// <seealso cref="#of(String, Map)"/> factory method.
		/// </para>
		/// <para>
		/// This map contains a mapping of the IDs that is in line with TZDB 2005r and
		/// later, where 'EST', 'MST' and 'HST' map to IDs which do not include daylight
		/// savings.
		/// </para>
		/// <para>
		/// This maps as follows:
		/// <ul>
		/// <li>EST - -05:00</li>
		/// <li>HST - -10:00</li>
		/// <li>MST - -07:00</li>
		/// <li>ACT - Australia/Darwin</li>
		/// <li>AET - Australia/Sydney</li>
		/// <li>AGT - America/Argentina/Buenos_Aires</li>
		/// <li>ART - Africa/Cairo</li>
		/// <li>AST - America/Anchorage</li>
		/// <li>BET - America/Sao_Paulo</li>
		/// <li>BST - Asia/Dhaka</li>
		/// <li>CAT - Africa/Harare</li>
		/// <li>CNT - America/St_Johns</li>
		/// <li>CST - America/Chicago</li>
		/// <li>CTT - Asia/Shanghai</li>
		/// <li>EAT - Africa/Addis_Ababa</li>
		/// <li>ECT - Europe/Paris</li>
		/// <li>IET - America/Indiana/Indianapolis</li>
		/// <li>IST - Asia/Kolkata</li>
		/// <li>JST - Asia/Tokyo</li>
		/// <li>MIT - Pacific/Apia</li>
		/// <li>NET - Asia/Yerevan</li>
		/// <li>NST - Pacific/Auckland</li>
		/// <li>PLT - Asia/Karachi</li>
		/// <li>PNT - America/Phoenix</li>
		/// <li>PRT - America/Puerto_Rico</li>
		/// <li>PST - America/Los_Angeles</li>
		/// <li>SST - Pacific/Guadalcanal</li>
		/// <li>VST - Asia/Ho_Chi_Minh</li>
		/// </ul>
		/// The map is unmodifiable.
		/// </para>
		/// </summary>
		public static readonly IDictionary<String, String> SHORT_IDS;
		static ZoneId()
		{
			IDictionary<String, String> map = new Dictionary<String, String>(64);
			map["ACT"] = "Australia/Darwin";
			map["AET"] = "Australia/Sydney";
			map["AGT"] = "America/Argentina/Buenos_Aires";
			map["ART"] = "Africa/Cairo";
			map["AST"] = "America/Anchorage";
			map["BET"] = "America/Sao_Paulo";
			map["BST"] = "Asia/Dhaka";
			map["CAT"] = "Africa/Harare";
			map["CNT"] = "America/St_Johns";
			map["CST"] = "America/Chicago";
			map["CTT"] = "Asia/Shanghai";
			map["EAT"] = "Africa/Addis_Ababa";
			map["ECT"] = "Europe/Paris";
			map["IET"] = "America/Indiana/Indianapolis";
			map["IST"] = "Asia/Kolkata";
			map["JST"] = "Asia/Tokyo";
			map["MIT"] = "Pacific/Apia";
			map["NET"] = "Asia/Yerevan";
			map["NST"] = "Pacific/Auckland";
			map["PLT"] = "Asia/Karachi";
			map["PNT"] = "America/Phoenix";
			map["PRT"] = "America/Puerto_Rico";
			map["PST"] = "America/Los_Angeles";
			map["SST"] = "Pacific/Guadalcanal";
			map["VST"] = "Asia/Ho_Chi_Minh";
			map["EST"] = "-05:00";
			map["MST"] = "-07:00";
			map["HST"] = "-10:00";
			SHORT_IDS = Collections.UnmodifiableMap(map);
		}
		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 8352817235686L;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the system default time-zone.
		/// <para>
		/// This queries <seealso cref="TimeZone#getDefault()"/> to find the default time-zone
		/// and converts it to a {@code ZoneId}. If the system default time-zone is changed,
		/// then the result of this method will also change.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the converted zone ID has an invalid format </exception>
		/// <exception cref="ZoneRulesException"> if the converted zone region ID cannot be found </exception>
		public static ZoneId SystemDefault()
		{
			return TimeZone.Default.ToZoneId();
		}

		/// <summary>
		/// Gets the set of available zone IDs.
		/// <para>
		/// This set includes the string form of all available region-based IDs.
		/// Offset-based zone IDs are not included in the returned set.
		/// The ID can be passed to <seealso cref="#of(String)"/> to create a {@code ZoneId}.
		/// </para>
		/// <para>
		/// The set of zone IDs can increase over time, although in a typical application
		/// the set of IDs is fixed. Each call to this method is thread-safe.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a modifiable copy of the set of zone IDs, not null </returns>
		public static Set<String> AvailableZoneIds
		{
			get
			{
				return ZoneRulesProvider.AvailableZoneIds;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneId} using its ID using a map
		/// of aliases to supplement the standard zone IDs.
		/// <para>
		/// Many users of time-zones use short abbreviations, such as PST for
		/// 'Pacific Standard Time' and PDT for 'Pacific Daylight Time'.
		/// These abbreviations are not unique, and so cannot be used as IDs.
		/// This method allows a map of string to time-zone to be setup and reused
		/// within an application.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <param name="aliasMap">  a map of alias zone IDs (typically abbreviations) to real zone IDs, not null </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		/// <exception cref="ZoneRulesException"> if the zone ID is a region ID that cannot be found </exception>
		public static ZoneId Of(String zoneId, IDictionary<String, String> aliasMap)
		{
			Objects.RequireNonNull(zoneId, "zoneId");
			Objects.RequireNonNull(aliasMap, "aliasMap");
			String id = aliasMap[zoneId];
			id = (id != null ? id : zoneId);
			return Of(id);
		}

		/// <summary>
		/// Obtains an instance of {@code ZoneId} from an ID ensuring that the
		/// ID is valid and available for use.
		/// <para>
		/// This method parses the ID producing a {@code ZoneId} or {@code ZoneOffset}.
		/// A {@code ZoneOffset} is returned if the ID is 'Z', or starts with '+' or '-'.
		/// The result will always be a valid ID for which <seealso cref="ZoneRules"/> can be obtained.
		/// </para>
		/// <para>
		/// Parsing matches the zone ID step by step as follows.
		/// <ul>
		/// <li>If the zone ID equals 'Z', the result is {@code ZoneOffset.UTC}.
		/// <li>If the zone ID consists of a single letter, the zone ID is invalid
		///  and {@code DateTimeException} is thrown.
		/// <li>If the zone ID starts with '+' or '-', the ID is parsed as a
		///  {@code ZoneOffset} using <seealso cref="ZoneOffset#of(String)"/>.
		/// <li>If the zone ID equals 'GMT', 'UTC' or 'UT' then the result is a {@code ZoneId}
		///  with the same ID and rules equivalent to {@code ZoneOffset.UTC}.
		/// <li>If the zone ID starts with 'UTC+', 'UTC-', 'GMT+', 'GMT-', 'UT+' or 'UT-'
		///  then the ID is a prefixed offset-based ID. The ID is split in two, with
		///  a two or three letter prefix and a suffix starting with the sign.
		///  The suffix is parsed as a <seealso cref="ZoneOffset#of(String) ZoneOffset"/>.
		///  The result will be a {@code ZoneId} with the specified UTC/GMT/UT prefix
		///  and the normalized offset ID as per <seealso cref="ZoneOffset#getId()"/>.
		///  The rules of the returned {@code ZoneId} will be equivalent to the
		///  parsed {@code ZoneOffset}.
		/// <li>All other IDs are parsed as region-based zone IDs. Region IDs must
		///  match the regular expression <code>[A-Za-z][A-Za-z0-9~/._+-]+</code>
		///  otherwise a {@code DateTimeException} is thrown. If the zone ID is not
		///  in the configured set of IDs, {@code ZoneRulesException} is thrown.
		///  The detailed format of the region ID depends on the group supplying the data.
		///  The default set of data is supplied by the IANA Time Zone Database (TZDB).
		///  This has region IDs of the form '{area}/{city}', such as 'Europe/Paris' or 'America/New_York'.
		///  This is compatible with most IDs from <seealso cref="java.util.TimeZone"/>.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		/// <exception cref="ZoneRulesException"> if the zone ID is a region ID that cannot be found </exception>
		public static ZoneId Of(String zoneId)
		{
			return Of(zoneId, true);
		}

		/// <summary>
		/// Obtains an instance of {@code ZoneId} wrapping an offset.
		/// <para>
		/// If the prefix is "GMT", "UTC", or "UT" a {@code ZoneId}
		/// with the prefix and the non-zero offset is returned.
		/// If the prefix is empty {@code ""} the {@code ZoneOffset} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prefix">  the time-zone ID, not null </param>
		/// <param name="offset">  the offset, not null </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the prefix is not one of
		///     "GMT", "UTC", or "UT", or "" </exception>
		public static ZoneId OfOffset(String prefix, ZoneOffset offset)
		{
			Objects.RequireNonNull(prefix, "prefix");
			Objects.RequireNonNull(offset, "offset");
			if (prefix.Length() == 0)
			{
				return offset;
			}

			if (!prefix.Equals("GMT") && !prefix.Equals("UTC") && !prefix.Equals("UT"))
			{
				 throw new IllegalArgumentException("prefix should be GMT, UTC or UT, is: " + prefix);
			}

			if (offset.TotalSeconds != 0)
			{
				prefix = prefix.Concat(offset.Id);
			}
			return new ZoneRegion(prefix, offset.Rules);
		}

		/// <summary>
		/// Parses the ID, taking a flag to indicate whether {@code ZoneRulesException}
		/// should be thrown or not, used in deserialization.
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <param name="checkAvailable">  whether to check if the zone ID is available </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the ID format is invalid </exception>
		/// <exception cref="ZoneRulesException"> if checking availability and the ID cannot be found </exception>
		internal static ZoneId Of(String zoneId, bool checkAvailable)
		{
			Objects.RequireNonNull(zoneId, "zoneId");
			if (zoneId.Length() <= 1 || zoneId.StartsWith("+") || zoneId.StartsWith("-"))
			{
				return ZoneOffset.Of(zoneId);
			}
			else if (zoneId.StartsWith("UTC") || zoneId.StartsWith("GMT"))
			{
				return OfWithPrefix(zoneId, 3, checkAvailable);
			}
			else if (zoneId.StartsWith("UT"))
			{
				return OfWithPrefix(zoneId, 2, checkAvailable);
			}
			return ZoneRegion.OfId(zoneId, checkAvailable);
		}

		/// <summary>
		/// Parse once a prefix is established.
		/// </summary>
		/// <param name="zoneId">  the time-zone ID, not null </param>
		/// <param name="prefixLength">  the length of the prefix, 2 or 3 </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		private static ZoneId OfWithPrefix(String zoneId, int prefixLength, bool checkAvailable)
		{
			String prefix = zoneId.Substring(0, prefixLength);
			if (zoneId.Length() == prefixLength)
			{
				return OfOffset(prefix, ZoneOffset.UTC);
			}
			if (zoneId.CharAt(prefixLength) != '+' && zoneId.CharAt(prefixLength) != '-')
			{
				return ZoneRegion.OfId(zoneId, checkAvailable); // drop through to ZoneRulesProvider
			}
			try
			{
				ZoneOffset offset = ZoneOffset.Of(zoneId.Substring(prefixLength));
				if (offset == ZoneOffset.UTC)
				{
					return OfOffset(prefix, offset);
				}
				return OfOffset(prefix, offset);
			}
			catch (DateTimeException ex)
			{
				throw new DateTimeException("Invalid ID for offset-based ZoneId: " + zoneId, ex);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ZoneId} from a temporal object.
		/// <para>
		/// This obtains a zone based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code ZoneId}.
		/// </para>
		/// <para>
		/// A {@code TemporalAccessor} represents some form of date and time information.
		/// This factory converts the arbitrary temporal object to an instance of {@code ZoneId}.
		/// </para>
		/// <para>
		/// The conversion will try to obtain the zone in a way that favours region-based
		/// zones over offset-based zones using <seealso cref="TemporalQueries#zone()"/>.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code ZoneId::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the zone ID, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code ZoneId} </exception>
		public static ZoneId From(TemporalAccessor temporal)
		{
			ZoneId obj = temporal.query(TemporalQueries.Zone());
			if (obj == null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DateTimeException("Unable to obtain ZoneId from TemporalAccessor: " + temporal + " of type " + temporal.GetType().FullName);
			}
			return obj;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor only accessible within the package.
		/// </summary>
		internal ZoneId()
		{
			if (this.GetType() != typeof(ZoneOffset) && this.GetType() != typeof(ZoneRegion))
			{
				throw new AssertionError("Invalid subclass");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the unique time-zone ID.
		/// <para>
		/// This ID uniquely defines this object.
		/// The format of an offset based ID is defined by <seealso cref="ZoneOffset#getId()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time-zone unique ID, not null </returns>
		public abstract String Id {get;}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the textual representation of the zone, such as 'British Time' or
		/// '+02:00'.
		/// <para>
		/// This returns the textual name used to identify the time-zone ID,
		/// suitable for presentation to the user.
		/// The parameters control the style of the returned text and the locale.
		/// </para>
		/// <para>
		/// If no textual mapping is found then the <seealso cref="#getId() full ID"/> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="style">  the length of the text required, not null </param>
		/// <param name="locale">  the locale to use, not null </param>
		/// <returns> the text value of the zone, not null </returns>
		public virtual String GetDisplayName(TextStyle style, Locale locale)
		{
			return (new DateTimeFormatterBuilder()).AppendZoneText(style).ToFormatter(locale).Format(ToTemporal());
		}

		/// <summary>
		/// Converts this zone to a {@code TemporalAccessor}.
		/// <para>
		/// A {@code ZoneId} can be fully represented as a {@code TemporalAccessor}.
		/// However, the interface is not implemented by this class as most of the
		/// methods on the interface have no meaning to {@code ZoneId}.
		/// </para>
		/// <para>
		/// The returned temporal has no supported fields, with the query method
		/// supporting the return of the zone using <seealso cref="TemporalQueries#zoneId()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a temporal equivalent to this zone, not null </returns>
		private TemporalAccessor ToTemporal()
		{
			return new TemporalAccessorAnonymousInnerClassHelper(this);
		}

		private class TemporalAccessorAnonymousInnerClassHelper : TemporalAccessor
		{
			private readonly ZoneId OuterInstance;

			public TemporalAccessorAnonymousInnerClassHelper(ZoneId outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual bool IsSupported(TemporalField field)
			{
				return false;
			}
			public virtual long GetLong(TemporalField field)
			{
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R> R query(java.time.temporal.TemporalQuery<R> query)
			public override R query<R>(TemporalQuery<R> query)
			{
				if (query == TemporalQueries.ZoneId())
				{
					return (R) ZoneId.this;
				}
				return TemporalAccessor.this.query(query);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the time-zone rules for this ID allowing calculations to be performed.
		/// <para>
		/// The rules provide the functionality associated with a time-zone,
		/// such as finding the offset for a given instant or local date-time.
		/// </para>
		/// <para>
		/// A time-zone can be invalid if it is deserialized in a Java Runtime which
		/// does not have the same rules loaded as the Java Runtime that stored it.
		/// In this case, calling this method will throw a {@code ZoneRulesException}.
		/// </para>
		/// <para>
		/// The rules are supplied by <seealso cref="ZoneRulesProvider"/>. An advanced provider may
		/// support dynamic updates to the rules without restarting the Java Runtime.
		/// If so, then the result of this method may change over time.
		/// Each individual call will be still remain thread-safe.
		/// </para>
		/// <para>
		/// <seealso cref="ZoneOffset"/> will always return a set of rules where the offset never changes.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the rules, not null </returns>
		/// <exception cref="ZoneRulesException"> if no rules are available for this ID </exception>
		public abstract ZoneRules Rules {get;}

		/// <summary>
		/// Normalizes the time-zone ID, returning a {@code ZoneOffset} where possible.
		/// <para>
		/// The returns a normalized {@code ZoneId} that can be used in place of this ID.
		/// The result will have {@code ZoneRules} equivalent to those returned by this object,
		/// however the ID returned by {@code getId()} may be different.
		/// </para>
		/// <para>
		/// The normalization checks if the rules of this {@code ZoneId} have a fixed offset.
		/// If they do, then the {@code ZoneOffset} equal to that offset is returned.
		/// Otherwise {@code this} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time-zone unique ID, not null </returns>
		public virtual ZoneId Normalized()
		{
			try
			{
				ZoneRules rules = Rules;
				if (rules.FixedOffset)
				{
					return rules.GetOffset(Instant.EPOCH);
				}
			}
			catch (ZoneRulesException)
			{
				// invalid ZoneRegion is not important to this method
			}
			return this;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this time-zone ID is equal to another time-zone ID.
		/// <para>
		/// The comparison is based on the ID.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other time-zone ID </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
			   return true;
			}
			if (obj is ZoneId)
			{
				ZoneId other = (ZoneId) obj;
				return Id.Equals(other.Id);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this time-zone ID.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return Id.HashCode();
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
		/// Outputs this zone as a {@code String}, using the ID.
		/// </summary>
		/// <returns> a string representation of this time-zone ID, not null </returns>
		public override String ToString()
		{
			return Id;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(7);  // identifies a ZoneId (not ZoneOffset)
		///  out.writeUTF(getId());
		/// </pre>
		/// <para>
		/// When read back in, the {@code ZoneId} will be created as though using
		/// <seealso cref="#of(String)"/>, but without any exception in the case where the
		/// ID has a valid format, but is not in the known set of region-based IDs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		// this is here for serialization Javadoc
		private Object WriteReplace()
		{
			return new Ser(Ser.ZONE_REGION_TYPE, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract void write(java.io.DataOutput out) throws java.io.IOException;
		internal abstract void Write(DataOutput @out);

	}

}