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
	/// Provider of time-zone rules to the system.
	/// <para>
	/// This class manages the configuration of time-zone rules.
	/// The static methods provide the public API that can be used to manage the providers.
	/// The abstract methods provide the SPI that allows rules to be provided.
	/// </para>
	/// <para>
	/// ZoneRulesProvider may be installed in an instance of the Java Platform as
	/// extension classes, that is, jar files placed into any of the usual extension
	/// directories. Installed providers are loaded using the service-provider loading
	/// facility defined by the <seealso cref="ServiceLoader"/> class. A ZoneRulesProvider
	/// identifies itself with a provider configuration file named
	/// {@code java.time.zone.ZoneRulesProvider} in the resource directory
	/// {@code META-INF/services}. The file should contain a line that specifies the
	/// fully qualified concrete zonerules-provider class name.
	/// Providers may also be made available by adding them to the class path or by
	/// registering themselves via <seealso cref="#registerProvider"/> method.
	/// </para>
	/// <para>
	/// The Java virtual machine has a default provider that provides zone rules
	/// for the time-zones defined by IANA Time Zone Database (TZDB). If the system
	/// property {@code java.time.zone.DefaultZoneRulesProvider} is defined then
	/// it is taken to be the fully-qualified name of a concrete ZoneRulesProvider
	/// class to be loaded as the default provider, using the system class loader.
	/// If this system property is not defined, a system-default provider will be
	/// loaded to serve as the default provider.
	/// </para>
	/// <para>
	/// Rules are looked up primarily by zone ID, as used by <seealso cref="ZoneId"/>.
	/// Only zone region IDs may be used, zone offset IDs are not used here.
	/// </para>
	/// <para>
	/// Time-zone rules are political, thus the data can change at any time.
	/// Each provider will provide the latest rules for each zone ID, but they
	/// may also provide the history of how the rules changed.
	/// 
	/// @implSpec
	/// This interface is a service provider that can be called by multiple threads.
	/// Implementations must be immutable and thread-safe.
	/// </para>
	/// <para>
	/// Providers must ensure that once a rule has been seen by the application, the
	/// rule must continue to be available.
	/// </para>
	/// <para>
	///  Providers are encouraged to implement a meaningful {@code toString} method.
	/// </para>
	/// <para>
	/// Many systems would like to update time-zone rules dynamically without stopping the JVM.
	/// When examined in detail, this is a complex problem.
	/// Providers may choose to handle dynamic updates, however the default provider does not.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public abstract class ZoneRulesProvider
	{

		/// <summary>
		/// The set of loaded providers.
		/// </summary>
		private static readonly CopyOnWriteArrayList<ZoneRulesProvider> PROVIDERS = new CopyOnWriteArrayList<ZoneRulesProvider>();
		/// <summary>
		/// The lookup from zone ID to provider.
		/// </summary>
		private static readonly ConcurrentMap<String, ZoneRulesProvider> ZONES = new ConcurrentDictionary<String, ZoneRulesProvider>(512, 0.75f, 2);

		static ZoneRulesProvider()
		{
			// if the property java.time.zone.DefaultZoneRulesProvider is
			// set then its value is the class name of the default provider
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<ZoneRulesProvider> loaded = new java.util.ArrayList<>();
			IList<ZoneRulesProvider> loaded = new List<ZoneRulesProvider>();
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(loaded));

			ServiceLoader<ZoneRulesProvider> sl = ServiceLoader.Load(typeof(ZoneRulesProvider), ClassLoader.SystemClassLoader);
			IEnumerator<ZoneRulesProvider> it = sl.Iterator();
			while (it.MoveNext())
			{
				ZoneRulesProvider provider;
				try
				{
					provider = it.Current;
				}
				catch (ServiceConfigurationError ex)
				{
					if (ex.Cause is SecurityException)
					{
						continue; // ignore the security exception, try the next provider
					}
					throw ex;
				}
				bool found = false;
				foreach (ZoneRulesProvider p in loaded)
				{
					if (p.GetType() == provider.GetType())
					{
						found = true;
					}
				}
				if (!found)
				{
					RegisterProvider0(provider);
					loaded.Add(provider);
				}
			}
			// CopyOnWriteList could be slow if lots of providers and each added individually
			PROVIDERS.AddRange(loaded);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private IList<ZoneRulesProvider> Loaded;

			public PrivilegedActionAnonymousInnerClassHelper(IList<ZoneRulesProvider> loaded)
			{
				this.Loaded = loaded;
			}

			public virtual Object Run()
			{
				String prop = System.getProperty("java.time.zone.DefaultZoneRulesProvider");
				if (prop != null)
				{
					try
					{
						Class c = Class.ForName(prop, true, ClassLoader.SystemClassLoader);
						ZoneRulesProvider provider = typeof(ZoneRulesProvider).cast(c.NewInstance());
						RegisterProvider(provider);
						Loaded.Add(provider);
					}
					catch (Exception x)
					{
						throw new Error(x);
					}
				}
				else
				{
					RegisterProvider(new TzdbZoneRulesProvider());
				}
				return null;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Gets the set of available zone IDs.
		/// <para>
		/// These IDs are the string form of a <seealso cref="ZoneId"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a modifiable copy of the set of zone IDs, not null </returns>
		public static Set<String> AvailableZoneIds
		{
			get
			{
				return new HashSet<>(ZONES.KeySet());
			}
		}

		/// <summary>
		/// Gets the rules for the zone ID.
		/// <para>
		/// This returns the latest available rules for the zone ID.
		/// </para>
		/// <para>
		/// This method relies on time-zone data provider files that are configured.
		/// These are loaded using a {@code ServiceLoader}.
		/// </para>
		/// <para>
		/// The caching flag is designed to allow provider implementations to
		/// prevent the rules being cached in {@code ZoneId}.
		/// Under normal circumstances, the caching of zone rules is highly desirable
		/// as it will provide greater performance. However, there is a use case where
		/// the caching would not be desirable, see <seealso cref="#provideRules"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId"> the zone ID as defined by {@code ZoneId}, not null </param>
		/// <param name="forCaching"> whether the rules are being queried for caching,
		/// true if the returned rules will be cached by {@code ZoneId},
		/// false if they will be returned to the user without being cached in {@code ZoneId} </param>
		/// <returns> the rules, null if {@code forCaching} is true and this
		/// is a dynamic provider that wants to prevent caching in {@code ZoneId},
		/// otherwise not null </returns>
		/// <exception cref="ZoneRulesException"> if rules cannot be obtained for the zone ID </exception>
		public static ZoneRules GetRules(String zoneId, bool forCaching)
		{
			Objects.RequireNonNull(zoneId, "zoneId");
			return GetProvider(zoneId).ProvideRules(zoneId, forCaching);
		}

		/// <summary>
		/// Gets the history of rules for the zone ID.
		/// <para>
		/// Time-zones are defined by governments and change frequently.
		/// This method allows applications to find the history of changes to the
		/// rules for a single zone ID. The map is keyed by a string, which is the
		/// version string associated with the rules.
		/// </para>
		/// <para>
		/// The exact meaning and format of the version is provider specific.
		/// The version must follow lexicographical order, thus the returned map will
		/// be order from the oldest known rules to the newest available rules.
		/// The default 'TZDB' group uses version numbering consisting of the year
		/// followed by a letter, such as '2009e' or '2012f'.
		/// </para>
		/// <para>
		/// Implementations must provide a result for each valid zone ID, however
		/// they do not have to provide a history of rules.
		/// Thus the map will always contain one element, and will only contain more
		/// than one element if historical rule information is available.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		/// <returns> a modifiable copy of the history of the rules for the ID, sorted
		///  from oldest to newest, not null </returns>
		/// <exception cref="ZoneRulesException"> if history cannot be obtained for the zone ID </exception>
		public static NavigableMap<String, ZoneRules> GetVersions(String zoneId)
		{
			Objects.RequireNonNull(zoneId, "zoneId");
			return GetProvider(zoneId).ProvideVersions(zoneId);
		}

		/// <summary>
		/// Gets the provider for the zone ID.
		/// </summary>
		/// <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		/// <returns> the provider, not null </returns>
		/// <exception cref="ZoneRulesException"> if the zone ID is unknown </exception>
		private static ZoneRulesProvider GetProvider(String zoneId)
		{
			ZoneRulesProvider provider = ZONES[zoneId];
			if (provider == null)
			{
				if (ZONES.Count == 0)
				{
					throw new ZoneRulesException("No time-zone data files registered");
				}
				throw new ZoneRulesException("Unknown time-zone ID: " + zoneId);
			}
			return provider;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Registers a zone rules provider.
		/// <para>
		/// This adds a new provider to those currently available.
		/// A provider supplies rules for one or more zone IDs.
		/// A provider cannot be registered if it supplies a zone ID that has already been
		/// registered. See the notes on time-zone IDs in <seealso cref="ZoneId"/>, especially
		/// the section on using the concept of a "group" to make IDs unique.
		/// </para>
		/// <para>
		/// To ensure the integrity of time-zones already created, there is no way
		/// to deregister providers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="provider">  the provider to register, not null </param>
		/// <exception cref="ZoneRulesException"> if a zone ID is already registered </exception>
		public static void RegisterProvider(ZoneRulesProvider provider)
		{
			Objects.RequireNonNull(provider, "provider");
			RegisterProvider0(provider);
			PROVIDERS.Add(provider);
		}

		/// <summary>
		/// Registers the provider.
		/// </summary>
		/// <param name="provider">  the provider to register, not null </param>
		/// <exception cref="ZoneRulesException"> if unable to complete the registration </exception>
		private static void RegisterProvider0(ZoneRulesProvider provider)
		{
			foreach (String zoneId in provider.ProvideZoneIds())
			{
				Objects.RequireNonNull(zoneId, "zoneId");
				ZoneRulesProvider old = ZONES.PutIfAbsent(zoneId, provider);
				if (old != null)
				{
					throw new ZoneRulesException("Unable to register zone as one already registered with that ID: " + zoneId + ", currently loading from provider: " + provider);
				}
			}
		}

		/// <summary>
		/// Refreshes the rules from the underlying data provider.
		/// <para>
		/// This method allows an application to request that the providers check
		/// for any updates to the provided rules.
		/// After calling this method, the offset stored in any <seealso cref="ZonedDateTime"/>
		/// may be invalid for the zone ID.
		/// </para>
		/// <para>
		/// Dynamic update of rules is a complex problem and most applications
		/// should not use this method or dynamic rules.
		/// To achieve dynamic rules, a provider implementation will have to be written
		/// as per the specification of this class.
		/// In addition, instances of {@code ZoneRules} must not be cached in the
		/// application as they will become stale. However, the boolean flag on
		/// <seealso cref="#provideRules(String, boolean)"/> allows provider implementations
		/// to control the caching of {@code ZoneId}, potentially ensuring that
		/// all objects in the system see the new rules.
		/// Note that there is likely to be a cost in performance of a dynamic rules
		/// provider. Note also that no dynamic rules provider is in this specification.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the rules were updated </returns>
		/// <exception cref="ZoneRulesException"> if an error occurs during the refresh </exception>
		public static bool Refresh()
		{
			bool changed = false;
			foreach (ZoneRulesProvider provider in PROVIDERS)
			{
				changed |= provider.ProvideRefresh();
			}
			return changed;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected internal ZoneRulesProvider()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// SPI method to get the available zone IDs.
		/// <para>
		/// This obtains the IDs that this {@code ZoneRulesProvider} provides.
		/// A provider should provide data for at least one zone ID.
		/// </para>
		/// <para>
		/// The returned zone IDs remain available and valid for the lifetime of the application.
		/// A dynamic provider may increase the set of IDs as more data becomes available.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the set of zone IDs being provided, not null </returns>
		/// <exception cref="ZoneRulesException"> if a problem occurs while providing the IDs </exception>
		protected internal abstract Set<String> ProvideZoneIds();

		/// <summary>
		/// SPI method to get the rules for the zone ID.
		/// <para>
		/// This loads the rules for the specified zone ID.
		/// The provider implementation must validate that the zone ID is valid and
		/// available, throwing a {@code ZoneRulesException} if it is not.
		/// The result of the method in the valid case depends on the caching flag.
		/// </para>
		/// <para>
		/// If the provider implementation is not dynamic, then the result of the
		/// method must be the non-null set of rules selected by the ID.
		/// </para>
		/// <para>
		/// If the provider implementation is dynamic, then the flag gives the option
		/// of preventing the returned rules from being cached in <seealso cref="ZoneId"/>.
		/// When the flag is true, the provider is permitted to return null, where
		/// null will prevent the rules from being cached in {@code ZoneId}.
		/// When the flag is false, the provider must return non-null rules.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId"> the zone ID as defined by {@code ZoneId}, not null </param>
		/// <param name="forCaching"> whether the rules are being queried for caching,
		/// true if the returned rules will be cached by {@code ZoneId},
		/// false if they will be returned to the user without being cached in {@code ZoneId} </param>
		/// <returns> the rules, null if {@code forCaching} is true and this
		/// is a dynamic provider that wants to prevent caching in {@code ZoneId},
		/// otherwise not null </returns>
		/// <exception cref="ZoneRulesException"> if rules cannot be obtained for the zone ID </exception>
		protected internal abstract ZoneRules ProvideRules(String zoneId, bool forCaching);

		/// <summary>
		/// SPI method to get the history of rules for the zone ID.
		/// <para>
		/// This returns a map of historical rules keyed by a version string.
		/// The exact meaning and format of the version is provider specific.
		/// The version must follow lexicographical order, thus the returned map will
		/// be order from the oldest known rules to the newest available rules.
		/// The default 'TZDB' group uses version numbering consisting of the year
		/// followed by a letter, such as '2009e' or '2012f'.
		/// </para>
		/// <para>
		/// Implementations must provide a result for each valid zone ID, however
		/// they do not have to provide a history of rules.
		/// Thus the map will contain at least one element, and will only contain
		/// more than one element if historical rule information is available.
		/// </para>
		/// <para>
		/// The returned versions remain available and valid for the lifetime of the application.
		/// A dynamic provider may increase the set of versions as more data becomes available.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		/// <returns> a modifiable copy of the history of the rules for the ID, sorted
		///  from oldest to newest, not null </returns>
		/// <exception cref="ZoneRulesException"> if history cannot be obtained for the zone ID </exception>
		protected internal abstract NavigableMap<String, ZoneRules> ProvideVersions(String zoneId);

		/// <summary>
		/// SPI method to refresh the rules from the underlying data provider.
		/// <para>
		/// This method provides the opportunity for a provider to dynamically
		/// recheck the underlying data provider to find the latest rules.
		/// This could be used to load new rules without stopping the JVM.
		/// Dynamic behavior is entirely optional and most providers do not support it.
		/// </para>
		/// <para>
		/// This implementation returns false.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if the rules were updated </returns>
		/// <exception cref="ZoneRulesException"> if an error occurs during the refresh </exception>
		protected internal virtual bool ProvideRefresh()
		{
			return false;
		}

	}

}