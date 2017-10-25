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
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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



	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// An abstract implementation of a calendar system, used to organize and identify dates.
	/// <para>
	/// The main date and time API is built on the ISO calendar system.
	/// The chronology operates behind the scenes to represent the general concept of a calendar system.
	/// </para>
	/// <para>
	/// See <seealso cref="Chronology"/> for more details.
	/// 
	/// @implSpec
	/// This class is separated from the {@code Chronology} interface so that the static methods
	/// are not inherited. While {@code Chronology} can be implemented directly, it is strongly
	/// recommended to extend this abstract class instead.
	/// </para>
	/// <para>
	/// This class must be implemented with care to ensure other classes operate correctly.
	/// All implementations that can be instantiated must be final, immutable and thread-safe.
	/// Subclasses should be Serializable wherever possible.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public abstract class AbstractChronology : Chronology
	{
		public abstract ValueRange Range(ChronoField field);
		public abstract IList<Era> Eras();
		public abstract Era EraOf(int eraValue);
		public abstract int ProlepticYear(Era era, int yearOfEra);
		public abstract bool IsLeapYear(long prolepticYear);
		public abstract ChronoLocalDate Date(java.time.temporal.TemporalAccessor temporal);
		public abstract ChronoLocalDate DateEpochDay(long epochDay);
		public abstract ChronoLocalDate DateYearDay(int prolepticYear, int dayOfYear);
		public abstract ChronoLocalDate Date(int prolepticYear, int month, int dayOfMonth);
		public abstract String CalendarType {get;}
		public abstract String Id {get;}

		/// <summary>
		/// ChronoLocalDate order constant.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		internal static readonly IComparer<ChronoLocalDate> DATE_ORDER = (IComparer<ChronoLocalDate> & Serializable)(date1, date2) =>
		{
				return Long.Compare(date1.toEpochDay(), date2.toEpochDay());
			};
		/// <summary>
		/// ChronoLocalDateTime order constant.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static final java.util.Comparator<ChronoLocalDateTime<? extends ChronoLocalDate>> DATE_TIME_ORDER = (java.util.Comparator<ChronoLocalDateTime<? extends ChronoLocalDate>> & java.io.Serializable)(dateTime1, dateTime2) ->
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		internal static readonly IComparer<ChronoLocalDateTime<?>> DATE_TIME_ORDER = (IComparer<ChronoLocalDateTime<?>> & Serializable)(dateTime1, dateTime2) =>
		{
				int cmp = Long.Compare(dateTime1.toLocalDate().toEpochDay(), dateTime2.toLocalDate().toEpochDay());
				if (cmp == 0)
				{
					cmp = Long.Compare(dateTime1.toLocalTime().toNanoOfDay(), dateTime2.toLocalTime().toNanoOfDay());
				}
				return cmp;
			};
		/// <summary>
		/// ChronoZonedDateTime order constant.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static final java.util.Comparator<ChronoZonedDateTime<?>> INSTANT_ORDER = (java.util.Comparator<ChronoZonedDateTime<?>> & java.io.Serializable)(dateTime1, dateTime2) ->
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		internal static readonly IComparer<ChronoZonedDateTime<?>> INSTANT_ORDER = (IComparer<ChronoZonedDateTime<?>> & Serializable)(dateTime1, dateTime2) =>
		{
					int cmp = Long.Compare(dateTime1.toEpochSecond(), dateTime2.toEpochSecond());
					if (cmp == 0)
					{
						cmp = Long.Compare(dateTime1.toLocalTime().Nano, dateTime2.toLocalTime().Nano);
					}
					return cmp;
				};

		/// <summary>
		/// Map of available calendars by ID.
		/// </summary>
		private static readonly ConcurrentDictionary<String, Chronology> CHRONOS_BY_ID = new ConcurrentDictionary<String, Chronology>();
		/// <summary>
		/// Map of available calendars by calendar type.
		/// </summary>
		private static readonly ConcurrentDictionary<String, Chronology> CHRONOS_BY_TYPE = new ConcurrentDictionary<String, Chronology>();

		/// <summary>
		/// Register a Chronology by its ID and type for lookup by <seealso cref="#of(String)"/>.
		/// Chronologies must not be registered until they are completely constructed.
		/// Specifically, not in the constructor of Chronology.
		/// </summary>
		/// <param name="chrono"> the chronology to register; not null </param>
		/// <returns> the already registered Chronology if any, may be null </returns>
		internal static Chronology RegisterChrono(Chronology chrono)
		{
			return RegisterChrono(chrono, chrono.Id);
		}

		/// <summary>
		/// Register a Chronology by ID and type for lookup by <seealso cref="#of(String)"/>.
		/// Chronos must not be registered until they are completely constructed.
		/// Specifically, not in the constructor of Chronology.
		/// </summary>
		/// <param name="chrono"> the chronology to register; not null </param>
		/// <param name="id"> the ID to register the chronology; not null </param>
		/// <returns> the already registered Chronology if any, may be null </returns>
		internal static Chronology RegisterChrono(Chronology chrono, String id)
		{
			Chronology prev = CHRONOS_BY_ID.GetOrAdd(id, chrono);
			if (prev == null)
			{
				String type = chrono.CalendarType;
				if (type != null)
				{
					CHRONOS_BY_TYPE.GetOrAdd(type, chrono);
				}
			}
			return prev;
		}

		/// <summary>
		/// Initialization of the maps from id and type to Chronology.
		/// The ServiceLoader is used to find and register any implementations
		/// of <seealso cref="java.time.chrono.AbstractChronology"/> found in the bootclass loader.
		/// The built-in chronologies are registered explicitly.
		/// Calendars configured via the Thread's context classloader are local
		/// to that thread and are ignored.
		/// <para>
		/// The initialization is done only once using the registration
		/// of the IsoChronology as the test and the final step.
		/// Multiple threads may perform the initialization concurrently.
		/// Only the first registration of each Chronology is retained by the
		/// ConcurrentHashMap.
		/// </para>
		/// </summary>
		/// <returns> true if the cache was initialized </returns>
		private static bool InitCache()
		{
			if (CHRONOS_BY_ID["ISO"] == null)
			{
				// Initialization is incomplete

				// Register built-in Chronologies
				RegisterChrono(HijrahChronology.INSTANCE);
				RegisterChrono(JapaneseChronology.INSTANCE);
				RegisterChrono(MinguoChronology.INSTANCE);
				RegisterChrono(ThaiBuddhistChronology.INSTANCE);

				// Register Chronologies from the ServiceLoader
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.ServiceLoader<AbstractChronology> loader = java.util.ServiceLoader.load(AbstractChronology.class, null);
				ServiceLoader<AbstractChronology> loader = ServiceLoader.Load(typeof(AbstractChronology), null);
				foreach (AbstractChronology chrono in loader)
				{
					String id = chrono.Id;
					if (id.Equals("ISO") || RegisterChrono(chrono) != null)
					{
						// Log the attempt to replace an existing Chronology
						PlatformLogger logger = PlatformLogger.getLogger("java.time.chrono");
						logger.warning("Ignoring duplicate Chronology, from ServiceLoader configuration " + id);
					}
				}

				// finally, register IsoChronology to mark initialization is complete
				RegisterChrono(IsoChronology.INSTANCE);
				return true;
			}
			return Chronology_Fields.False;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Chronology} from a locale.
		/// <para>
		/// See <seealso cref="Chronology#ofLocale(Locale)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale">  the locale to use to obtain the calendar system, not null </param>
		/// <returns> the calendar system associated with the locale, not null </returns>
		/// <exception cref="java.time.DateTimeException"> if the locale-specified calendar cannot be found </exception>
		internal static Chronology OfLocale(Locale locale)
		{
			Objects.RequireNonNull(locale, "locale");
			String type = locale.GetUnicodeLocaleType("ca");
			if (type == null || "iso".Equals(type) || "iso8601".Equals(type))
			{
				return IsoChronology.INSTANCE;
			}
			// Not pre-defined; lookup by the type
			do
			{
				Chronology chrono = CHRONOS_BY_TYPE[type];
				if (chrono != null)
				{
					return chrono;
				}
				// If not found, do the initialization (once) and repeat the lookup
			} while (InitCache());

			// Look for a Chronology using ServiceLoader of the Thread's ContextClassLoader
			// Application provided Chronologies must not be cached
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.ServiceLoader<Chronology> loader = java.util.ServiceLoader.load(Chronology.class);
			ServiceLoader<Chronology> loader = ServiceLoader.Load(typeof(Chronology));
			foreach (Chronology chrono in loader)
			{
				if (type.Equals(chrono.CalendarType))
				{
					return chrono;
				}
			}
			throw new DateTimeException("Unknown calendar system: " + type);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code Chronology} from a chronology ID or
		/// calendar system type.
		/// <para>
		/// See <seealso cref="Chronology#of(String)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id">  the chronology ID or calendar system type, not null </param>
		/// <returns> the chronology with the identifier requested, not null </returns>
		/// <exception cref="java.time.DateTimeException"> if the chronology cannot be found </exception>
		internal static Chronology Of(String id)
		{
			Objects.RequireNonNull(id, "id");
			do
			{
				Chronology chrono = Of0(id);
				if (chrono != null)
				{
					return chrono;
				}
				// If not found, do the initialization (once) and repeat the lookup
			} while (InitCache());

			// Look for a Chronology using ServiceLoader of the Thread's ContextClassLoader
			// Application provided Chronologies must not be cached
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.ServiceLoader<Chronology> loader = java.util.ServiceLoader.load(Chronology.class);
			ServiceLoader<Chronology> loader = ServiceLoader.Load(typeof(Chronology));
			foreach (Chronology chrono in loader)
			{
				if (id.Equals(chrono.Id) || id.Equals(chrono.CalendarType))
				{
					return chrono;
				}
			}
			throw new DateTimeException("Unknown chronology: " + id);
		}

		/// <summary>
		/// Obtains an instance of {@code Chronology} from a chronology ID or
		/// calendar system type.
		/// </summary>
		/// <param name="id">  the chronology ID or calendar system type, not null </param>
		/// <returns> the chronology with the identifier requested, or {@code null} if not found </returns>
		private static Chronology Of0(String id)
		{
			Chronology chrono = CHRONOS_BY_ID[id];
			if (chrono == null)
			{
				chrono = CHRONOS_BY_TYPE[id];
			}
			return chrono;
		}

		/// <summary>
		/// Returns the available chronologies.
		/// <para>
		/// Each returned {@code Chronology} is available for use in the system.
		/// The set of chronologies includes the system chronologies and
		/// any chronologies provided by the application via ServiceLoader
		/// configuration.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the independent, modifiable set of the available chronology IDs, not null </returns>
		internal static Set<Chronology> AvailableChronologies
		{
			get
			{
				InitCache(); // force initialization
				HashSet<Chronology> chronos = new HashSet<Chronology>(CHRONOS_BY_ID.Values);
    
				/// Add in Chronologies from the ServiceLoader configuration
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.ServiceLoader<Chronology> loader = java.util.ServiceLoader.load(Chronology.class);
				ServiceLoader<Chronology> loader = ServiceLoader.Load(typeof(Chronology));
				foreach (Chronology chrono in loader)
				{
					chronos.Add(chrono);
				}
				return chronos;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance.
		/// </summary>
		protected internal AbstractChronology()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Resolves parsed {@code ChronoField} values into a date during parsing.
		/// <para>
		/// Most {@code TemporalField} implementations are resolved using the
		/// resolve method on the field. By contrast, the {@code ChronoField} class
		/// defines fields that only have meaning relative to the chronology.
		/// As such, {@code ChronoField} date fields are resolved here in the
		/// context of a specific chronology.
		/// </para>
		/// <para>
		/// {@code ChronoField} instances are resolved by this method, which may
		/// be overridden in subclasses.
		/// <ul>
		/// <li>{@code EPOCH_DAY} - If present, this is converted to a date and
		///  all other date fields are then cross-checked against the date.
		/// <li>{@code PROLEPTIC_MONTH} - If present, then it is split into the
		///  {@code YEAR} and {@code MONTH_OF_YEAR}. If the mode is strict or smart
		///  then the field is validated.
		/// <li>{@code YEAR_OF_ERA} and {@code ERA} - If both are present, then they
		///  are combined to form a {@code YEAR}. In lenient mode, the {@code YEAR_OF_ERA}
		///  range is not validated, in smart and strict mode it is. The {@code ERA} is
		///  validated for range in all three modes. If only the {@code YEAR_OF_ERA} is
		///  present, and the mode is smart or lenient, then the last available era
		///  is assumed. In strict mode, no era is assumed and the {@code YEAR_OF_ERA} is
		///  left untouched. If only the {@code ERA} is present, then it is left untouched.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH} -
		///  If all three are present, then they are combined to form a date.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is smart or strict, then the month and day are validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first day of the first month in the requested year,
		///  then adding the difference in months, then the difference in days.
		///  If the mode is smart, and the day-of-month is greater than the maximum for
		///  the year-month, then the day-of-month is adjusted to the last day-of-month.
		///  If the mode is strict, then the three fields must form a valid date.
		/// <li>{@code YEAR} and {@code DAY_OF_YEAR} -
		///  If both are present, then they are combined to form a date.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first day of the requested year, then adding
		///  the difference in days.
		///  If the mode is smart or strict, then the two fields must form a valid date.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		///  {@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		///  If all four are present, then they are combined to form a date.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first day of the first month in the requested year, then adding
		///  the difference in months, then the difference in weeks, then in days.
		///  If the mode is smart or strict, then the all four fields are validated to
		///  their outer ranges. The date is then combined in a manner equivalent to
		///  creating a date on the first day of the requested year and month, then adding
		///  the amount in weeks and days to reach their values. If the mode is strict,
		///  the date is additionally validated to check that the day and week adjustment
		///  did not change the month.
		/// <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		///  {@code DAY_OF_WEEK} - If all four are present, then they are combined to
		///  form a date. The approach is the same as described above for
		///  years, months and weeks in {@code ALIGNED_DAY_OF_WEEK_IN_MONTH}.
		///  The day-of-week is adjusted as the next or same matching day-of-week once
		///  the years, months and weeks have been handled.
		/// <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		///  If all three are present, then they are combined to form a date.
		///  In all three modes, the {@code YEAR} is validated.
		///  If the mode is lenient, then the date is combined in a manner equivalent to
		///  creating a date on the first day of the requested year, then adding
		///  the difference in weeks, then in days.
		///  If the mode is smart or strict, then the all three fields are validated to
		///  their outer ranges. The date is then combined in a manner equivalent to
		///  creating a date on the first day of the requested year, then adding
		///  the amount in weeks and days to reach their values. If the mode is strict,
		///  the date is additionally validated to check that the day and week adjustment
		///  did not change the year.
		/// <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code DAY_OF_WEEK} -
		///  If all three are present, then they are combined to form a date.
		///  The approach is the same as described above for years and weeks in
		///  {@code ALIGNED_DAY_OF_WEEK_IN_YEAR}. The day-of-week is adjusted as the
		///  next or same matching day-of-week once the years and weeks have been handled.
		/// </ul>
		/// </para>
		/// <para>
		/// The default implementation is suitable for most calendar systems.
		/// If <seealso cref="java.time.temporal.ChronoField#YEAR_OF_ERA"/> is found without an <seealso cref="java.time.temporal.ChronoField#ERA"/>
		/// then the last era in <seealso cref="#eras()"/> is used.
		/// The implementation assumes a 7 day week, that the first day-of-month
		/// has the value 1, that first day-of-year has the value 1, and that the
		/// first of the month and year always exists.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		/// <param name="resolverStyle">  the requested type of resolve, not null </param>
		/// <returns> the resolved date, null if insufficient information to create a date </returns>
		/// <exception cref="java.time.DateTimeException"> if the date cannot be resolved, typically
		///  because of a conflict in the input data </exception>
		public virtual ChronoLocalDate ResolveDate(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			// check epoch-day before inventing era
			if (fieldValues.ContainsKey(EPOCH_DAY))
			{
				return DateEpochDay(fieldValues.Remove(EPOCH_DAY));
			}

			// fix proleptic month before inventing era
			ResolveProlepticMonth(fieldValues, resolverStyle);

			// invent era if necessary to resolve year-of-era
			ChronoLocalDate resolved = ResolveYearOfEra(fieldValues, resolverStyle);
			if (resolved != null)
			{
				return resolved;
			}

			// build date
			if (fieldValues.ContainsKey(YEAR))
			{
				if (fieldValues.ContainsKey(MONTH_OF_YEAR))
				{
					if (fieldValues.ContainsKey(DAY_OF_MONTH))
					{
						return ResolveYMD(fieldValues, resolverStyle);
					}
					if (fieldValues.ContainsKey(ALIGNED_WEEK_OF_MONTH))
					{
						if (fieldValues.ContainsKey(ALIGNED_DAY_OF_WEEK_IN_MONTH))
						{
							return ResolveYMAA(fieldValues, resolverStyle);
						}
						if (fieldValues.ContainsKey(DAY_OF_WEEK))
						{
							return ResolveYMAD(fieldValues, resolverStyle);
						}
					}
				}
				if (fieldValues.ContainsKey(DAY_OF_YEAR))
				{
					return ResolveYD(fieldValues, resolverStyle);
				}
				if (fieldValues.ContainsKey(ALIGNED_WEEK_OF_YEAR))
				{
					if (fieldValues.ContainsKey(ALIGNED_DAY_OF_WEEK_IN_YEAR))
					{
						return ResolveYAA(fieldValues, resolverStyle);
					}
					if (fieldValues.ContainsKey(DAY_OF_WEEK))
					{
						return ResolveYAD(fieldValues, resolverStyle);
					}
				}
			}
			return null;
		}

		internal virtual void ResolveProlepticMonth(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			Long pMonth = fieldValues.Remove(PROLEPTIC_MONTH);
			if (pMonth != null)
			{
				if (resolverStyle != ResolverStyle.LENIENT)
				{
					PROLEPTIC_MONTH.checkValidValue(pMonth);
				}
				// first day-of-month is likely to be safest for setting proleptic-month
				// cannot add to year zero, as not all chronologies have a year zero
				ChronoLocalDate chronoDate = dateNow().with(DAY_OF_MONTH, 1).with(PROLEPTIC_MONTH, pMonth);
				AddFieldValue(fieldValues, MONTH_OF_YEAR, chronoDate.get(MONTH_OF_YEAR));
				AddFieldValue(fieldValues, YEAR, chronoDate.get(YEAR));
			}
		}

		internal virtual ChronoLocalDate ResolveYearOfEra(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			Long yoeLong = fieldValues.Remove(YEAR_OF_ERA);
			if (yoeLong != null)
			{
				Long eraLong = fieldValues.Remove(ERA);
				int yoe;
				if (resolverStyle != ResolverStyle.LENIENT)
				{
					yoe = Range(YEAR_OF_ERA).CheckValidIntValue(yoeLong, YEAR_OF_ERA);
				}
				else
				{
					yoe = Math.ToIntExact(yoeLong);
				}
				if (eraLong != null)
				{
					Era eraObj = EraOf(Range(ERA).CheckValidIntValue(eraLong, ERA));
					AddFieldValue(fieldValues, YEAR, ProlepticYear(eraObj, yoe));
				}
				else
				{
					if (fieldValues.ContainsKey(YEAR))
					{
						int year = Range(YEAR).CheckValidIntValue(fieldValues[YEAR], YEAR);
						ChronoLocalDate chronoDate = DateYearDay(year, 1);
						AddFieldValue(fieldValues, YEAR, ProlepticYear(chronoDate.Era, yoe));
					}
					else if (resolverStyle == ResolverStyle.STRICT)
					{
						// do not invent era if strict
						// reinstate the field removed earlier, no cross-check issues
						fieldValues[YEAR_OF_ERA] = yoeLong;
					}
					else
					{
						IList<Era> eras = Eras();
						if (eras.Count == 0)
						{
							AddFieldValue(fieldValues, YEAR, yoe);
						}
						else
						{
							Era eraObj = eras[eras.Count - 1];
							AddFieldValue(fieldValues, YEAR, ProlepticYear(eraObj, yoe));
						}
					}
				}
			}
			else if (fieldValues.ContainsKey(ERA))
			{
				Range(ERA).CheckValidValue(fieldValues[ERA], ERA); // always validated
			}
			return null;
		}

		internal virtual ChronoLocalDate ResolveYMD(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long months = Math.SubtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1);
				long days = Math.SubtractExact(fieldValues.Remove(DAY_OF_MONTH), 1);
				return Date(y, 1, 1).Plus(months, MONTHS).Plus(days, DAYS);
			}
			int moy = Range(MONTH_OF_YEAR).CheckValidIntValue(fieldValues.Remove(MONTH_OF_YEAR), MONTH_OF_YEAR);
			ValueRange domRange = Range(DAY_OF_MONTH);
			int dom = domRange.CheckValidIntValue(fieldValues.Remove(DAY_OF_MONTH), DAY_OF_MONTH);
			if (resolverStyle == ResolverStyle.SMART) // previous valid
			{
				try
				{
					return Date(y, moy, dom);
				}
				catch (DateTimeException)
				{
					return Date(y, moy, 1).with(TemporalAdjusters.LastDayOfMonth());
				}
			}
			return Date(y, moy, dom);
		}

		internal virtual ChronoLocalDate ResolveYD(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long days = Math.SubtractExact(fieldValues.Remove(DAY_OF_YEAR), 1);
				return DateYearDay(y, 1).Plus(days, DAYS);
			}
			int doy = Range(DAY_OF_YEAR).CheckValidIntValue(fieldValues.Remove(DAY_OF_YEAR), DAY_OF_YEAR);
			return DateYearDay(y, doy); // smart is same as strict
		}

		internal virtual ChronoLocalDate ResolveYMAA(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long months = Math.SubtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1);
				long weeks = Math.SubtractExact(fieldValues.Remove(ALIGNED_WEEK_OF_MONTH), 1);
				long days = Math.SubtractExact(fieldValues.Remove(ALIGNED_DAY_OF_WEEK_IN_MONTH), 1);
				return Date(y, 1, 1).Plus(months, MONTHS).Plus(weeks, WEEKS).Plus(days, DAYS);
			}
			int moy = Range(MONTH_OF_YEAR).CheckValidIntValue(fieldValues.Remove(MONTH_OF_YEAR), MONTH_OF_YEAR);
			int aw = Range(ALIGNED_WEEK_OF_MONTH).CheckValidIntValue(fieldValues.Remove(ALIGNED_WEEK_OF_MONTH), ALIGNED_WEEK_OF_MONTH);
			int ad = Range(ALIGNED_DAY_OF_WEEK_IN_MONTH).CheckValidIntValue(fieldValues.Remove(ALIGNED_DAY_OF_WEEK_IN_MONTH), ALIGNED_DAY_OF_WEEK_IN_MONTH);
			ChronoLocalDate date = Date(y, moy, 1).Plus((aw - 1) * 7 + (ad - 1), DAYS);
			if (resolverStyle == ResolverStyle.STRICT && date.get(MONTH_OF_YEAR) != moy)
			{
				throw new DateTimeException("Strict mode rejected resolved date as it is in a different month");
			}
			return date;
		}

		internal virtual ChronoLocalDate ResolveYMAD(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long months = Math.SubtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1);
				long weeks = Math.SubtractExact(fieldValues.Remove(ALIGNED_WEEK_OF_MONTH), 1);
				long dow = Math.SubtractExact(fieldValues.Remove(DAY_OF_WEEK), 1);
				return ResolveAligned(Date(y, 1, 1), months, weeks, dow);
			}
			int moy = Range(MONTH_OF_YEAR).CheckValidIntValue(fieldValues.Remove(MONTH_OF_YEAR), MONTH_OF_YEAR);
			int aw = Range(ALIGNED_WEEK_OF_MONTH).CheckValidIntValue(fieldValues.Remove(ALIGNED_WEEK_OF_MONTH), ALIGNED_WEEK_OF_MONTH);
			int dow = Range(DAY_OF_WEEK).CheckValidIntValue(fieldValues.Remove(DAY_OF_WEEK), DAY_OF_WEEK);
			ChronoLocalDate date = Date(y, moy, 1).Plus((aw - 1) * 7, DAYS).with(nextOrSame(DayOfWeek.of(dow)));
			if (resolverStyle == ResolverStyle.STRICT && date.get(MONTH_OF_YEAR) != moy)
			{
				throw new DateTimeException("Strict mode rejected resolved date as it is in a different month");
			}
			return date;
		}

		internal virtual ChronoLocalDate ResolveYAA(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long weeks = Math.SubtractExact(fieldValues.Remove(ALIGNED_WEEK_OF_YEAR), 1);
				long days = Math.SubtractExact(fieldValues.Remove(ALIGNED_DAY_OF_WEEK_IN_YEAR), 1);
				return DateYearDay(y, 1).Plus(weeks, WEEKS).Plus(days, DAYS);
			}
			int aw = Range(ALIGNED_WEEK_OF_YEAR).CheckValidIntValue(fieldValues.Remove(ALIGNED_WEEK_OF_YEAR), ALIGNED_WEEK_OF_YEAR);
			int ad = Range(ALIGNED_DAY_OF_WEEK_IN_YEAR).CheckValidIntValue(fieldValues.Remove(ALIGNED_DAY_OF_WEEK_IN_YEAR), ALIGNED_DAY_OF_WEEK_IN_YEAR);
			ChronoLocalDate date = DateYearDay(y, 1).Plus((aw - 1) * 7 + (ad - 1), DAYS);
			if (resolverStyle == ResolverStyle.STRICT && date.get(YEAR) != y)
			{
				throw new DateTimeException("Strict mode rejected resolved date as it is in a different year");
			}
			return date;
		}

		internal virtual ChronoLocalDate ResolveYAD(IDictionary<TemporalField, Long> fieldValues, ResolverStyle resolverStyle)
		{
			int y = Range(YEAR).CheckValidIntValue(fieldValues.Remove(YEAR), YEAR);
			if (resolverStyle == ResolverStyle.LENIENT)
			{
				long weeks = Math.SubtractExact(fieldValues.Remove(ALIGNED_WEEK_OF_YEAR), 1);
				long dow = Math.SubtractExact(fieldValues.Remove(DAY_OF_WEEK), 1);
				return ResolveAligned(DateYearDay(y, 1), 0, weeks, dow);
			}
			int aw = Range(ALIGNED_WEEK_OF_YEAR).CheckValidIntValue(fieldValues.Remove(ALIGNED_WEEK_OF_YEAR), ALIGNED_WEEK_OF_YEAR);
			int dow = Range(DAY_OF_WEEK).CheckValidIntValue(fieldValues.Remove(DAY_OF_WEEK), DAY_OF_WEEK);
			ChronoLocalDate date = DateYearDay(y, 1).Plus((aw - 1) * 7, DAYS).with(nextOrSame(DayOfWeek.of(dow)));
			if (resolverStyle == ResolverStyle.STRICT && date.get(YEAR) != y)
			{
				throw new DateTimeException("Strict mode rejected resolved date as it is in a different year");
			}
			return date;
		}

		internal virtual ChronoLocalDate ResolveAligned(ChronoLocalDate @base, long months, long weeks, long dow)
		{
			ChronoLocalDate date = @base.Plus(months, MONTHS).Plus(weeks, WEEKS);
			if (dow > 7)
			{
				date = date.Plus((dow - 1) / 7, WEEKS);
				dow = ((dow - 1) % 7) + 1;
			}
			else if (dow < 1)
			{
				date = date.Plus(Math.SubtractExact(dow, 7) / 7, WEEKS);
				dow = ((dow + 6) % 7) + 1;
			}
			return date.with(nextOrSame(DayOfWeek.of((int) dow)));
		}

		/// <summary>
		/// Adds a field-value pair to the map, checking for conflicts.
		/// <para>
		/// If the field is not already present, then the field-value pair is added to the map.
		/// If the field is already present and it has the same value as that specified, no action occurs.
		/// If the field is already present and it has a different value to that specified, then
		/// an exception is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to add, not null </param>
		/// <param name="value">  the value to add, not null </param>
		/// <exception cref="java.time.DateTimeException"> if the field is already present with a different value </exception>
		internal virtual void AddFieldValue(IDictionary<TemporalField, Long> fieldValues, ChronoField field, long value)
		{
			Long old = fieldValues[field]; // check first for better error message
			if (old != null && old.LongValue() != value)
			{
				throw new DateTimeException("Conflict found: " + field + " " + old + " differs from " + field + " " + value);
			}
			fieldValues[field] = value;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this chronology to another chronology.
		/// <para>
		/// The comparison order first by the chronology ID string, then by any
		/// additional information specific to the subclass.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// 
		/// @implSpec
		/// This implementation compares the chronology ID.
		/// Subclasses must compare any additional state that they store.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other chronology to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
		public virtual int CompareTo(Chronology other)
		{
			return Id.CompareTo(other.Id);
		}

		/// <summary>
		/// Checks if this chronology is equal to another chronology.
		/// <para>
		/// The comparison is based on the entire state of the object.
		/// 
		/// @implSpec
		/// This implementation checks the type and calls
		/// <seealso cref="#compareTo(java.time.chrono.Chronology)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other chronology </returns>
		public override bool Equals(Object Chronology_Fields)
		{
			if (this == Chronology_Fields.Obj)
			{
			   return true;
			}
			if (Chronology_Fields.Obj is AbstractChronology)
			{
				return CompareTo((AbstractChronology) Chronology_Fields.Obj) == 0;
			}
			return Chronology_Fields.False;
		}

		/// <summary>
		/// A hash code for this chronology.
		/// <para>
		/// The hash code should be based on the entire state of the object.
		/// 
		/// @implSpec
		/// This implementation is based on the chronology ID and class.
		/// Subclasses should add any additional state that they store.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return this.GetType().HashCode() ^ Id.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this chronology as a {@code String}, using the chronology ID.
		/// </summary>
		/// <returns> a string representation of this chronology, not null </returns>
		public override String ToString()
		{
			return Id;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the Chronology using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// <pre>
		///  out.writeByte(1);  // identifies this as a Chronology
		///  out.writeUTF(getId());
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		internal virtual Object WriteReplace()
		{
			return new Ser(Ser.CHRONO_TYPE, this);
		}

		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="java.io.InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.ObjectStreamException
		private void ReadObject(ObjectInputStream s)
		{
			throw new InvalidObjectException("Deserialization via serialization delegate");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal virtual void WriteExternal(DataOutput @out)
		{
			@out.WriteUTF(Id);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Chronology readExternal(java.io.DataInput in) throws java.io.IOException
		internal static Chronology ReadExternal(DataInput @in)
		{
			String id = @in.ReadUTF();
			return Chronology.of(id);
		}

	}

}