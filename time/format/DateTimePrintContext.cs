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
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.format
{



	/// <summary>
	/// Context object used during date and time printing.
	/// <para>
	/// This class provides a single wrapper to items used in the format.
	/// 
	/// @implSpec
	/// This class is a mutable context intended for use from a single thread.
	/// Usage of the class is thread-safe within standard printing as the framework creates
	/// a new instance of the class for each format and printing is single-threaded.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	internal sealed class DateTimePrintContext
	{

		/// <summary>
		/// The temporal being output.
		/// </summary>
		private TemporalAccessor Temporal_Renamed;
		/// <summary>
		/// The formatter, not null.
		/// </summary>
		private DateTimeFormatter Formatter;
		/// <summary>
		/// Whether the current formatter is optional.
		/// </summary>
		private int Optional;

		/// <summary>
		/// Creates a new instance of the context.
		/// </summary>
		/// <param name="temporal">  the temporal object being output, not null </param>
		/// <param name="formatter">  the formatter controlling the format, not null </param>
		internal DateTimePrintContext(TemporalAccessor temporal, DateTimeFormatter formatter) : base()
		{
			this.Temporal_Renamed = Adjust(temporal, formatter);
			this.Formatter = formatter;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static java.time.temporal.TemporalAccessor adjust(final java.time.temporal.TemporalAccessor temporal, DateTimeFormatter formatter)
		private static TemporalAccessor Adjust(TemporalAccessor temporal, DateTimeFormatter formatter)
		{
			// normal case first (early return is an optimization)
			Chronology overrideChrono = formatter.Chronology;
			ZoneId overrideZone = formatter.Zone;
			if (overrideChrono == null && overrideZone == null)
			{
				return temporal;
			}

			// ensure minimal change (early return is an optimization)
			Chronology temporalChrono = temporal.query(TemporalQueries.Chronology());
			ZoneId temporalZone = temporal.query(TemporalQueries.ZoneId());
			if (Objects.Equals(overrideChrono, temporalChrono))
			{
				overrideChrono = null;
			}
			if (Objects.Equals(overrideZone, temporalZone))
			{
				overrideZone = null;
			}
			if (overrideChrono == null && overrideZone == null)
			{
				return temporal;
			}

			// make adjustment
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.time.chrono.Chronology effectiveChrono = (overrideChrono != null ? overrideChrono : temporalChrono);
			Chronology effectiveChrono = (overrideChrono != null ? overrideChrono : temporalChrono);
			if (overrideZone != null)
			{
				// if have zone and instant, calculation is simple, defaulting chrono if necessary
				if (temporal.IsSupported(INSTANT_SECONDS))
				{
					Chronology chrono = (effectiveChrono != null ? effectiveChrono : IsoChronology.INSTANCE);
					return chrono.zonedDateTime(Instant.From(temporal), overrideZone);
				}
				// block changing zone on OffsetTime, and similar problem cases
				if (overrideZone.Normalized() is ZoneOffset && temporal.IsSupported(OFFSET_SECONDS) && temporal.get(OFFSET_SECONDS) != overrideZone.Rules.GetOffset(Instant.EPOCH).TotalSeconds)
				{
					throw new DateTimeException("Unable to apply override zone '" + overrideZone + "' because the temporal object being formatted has a different offset but" + " does not represent an instant: " + temporal);
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.time.ZoneId effectiveZone = (overrideZone != null ? overrideZone : temporalZone);
			ZoneId effectiveZone = (overrideZone != null ? overrideZone : temporalZone);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.time.chrono.ChronoLocalDate effectiveDate;
			ChronoLocalDate effectiveDate;
			if (overrideChrono != null)
			{
				if (temporal.IsSupported(EPOCH_DAY))
				{
					effectiveDate = effectiveChrono.Date(temporal);
				}
				else
				{
					// check for date fields other than epoch-day, ignoring case of converting null to ISO
					if (!(overrideChrono == IsoChronology.INSTANCE && temporalChrono == null))
					{
						foreach (ChronoField f in ChronoField.values())
						{
							if (f.DateBased && temporal.IsSupported(f))
							{
								throw new DateTimeException("Unable to apply override chronology '" + overrideChrono + "' because the temporal object being formatted contains date fields but" + " does not represent a whole date: " + temporal);
							}
						}
					}
					effectiveDate = null;
				}
			}
			else
			{
				effectiveDate = null;
			}

			// combine available data
			// this is a non-standard temporal that is almost a pure delegate
			// this better handles map-like underlying temporal instances
			return new TemporalAccessorAnonymousInnerClassHelper(temporal, effectiveChrono, effectiveZone, effectiveDate);
		}

		private class TemporalAccessorAnonymousInnerClassHelper : TemporalAccessor
		{
			private TemporalAccessor Temporal;
			private Chronology EffectiveChrono;
			private ZoneId EffectiveZone;
			private ChronoLocalDate EffectiveDate;

			public TemporalAccessorAnonymousInnerClassHelper(TemporalAccessor temporal, Chronology effectiveChrono, ZoneId effectiveZone, ChronoLocalDate effectiveDate)
			{
				this.Temporal = temporal;
				this.EffectiveChrono = effectiveChrono;
				this.EffectiveZone = effectiveZone;
				this.EffectiveDate = effectiveDate;
			}

			public virtual bool IsSupported(TemporalField field)
			{
				if (EffectiveDate != java.time.temporal.TemporalAccessor_Fields.Null && field.DateBased)
				{
					return EffectiveDate.IsSupported(field);
				}
				return Temporal.IsSupported(field);
			}
			public virtual ValueRange java.time.temporal.TemporalAccessor_Fields.range(TemporalField field)
			{
				if (EffectiveDate != java.time.temporal.TemporalAccessor_Fields.Null && field.DateBased)
				{
					return EffectiveDate.range(field);
				}
				return Temporal.range(field);
			}
			public virtual long GetLong(TemporalField field)
			{
				if (EffectiveDate != java.time.temporal.TemporalAccessor_Fields.Null && field.DateBased)
				{
					return EffectiveDate.GetLong(field);
				}
				return Temporal.GetLong(field);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R> R query(java.time.temporal.TemporalQuery<R> query)
			public override R query<R>(TemporalQuery<R> query)
			{
				if (query == TemporalQueries.Chronology())
				{
					return (R) EffectiveChrono;
				}
				if (query == TemporalQueries.ZoneId())
				{
					return (R) EffectiveZone;
				}
				if (query == TemporalQueries.Precision())
				{
					return Temporal.query(query);
				}
				return query.QueryFrom(this);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the temporal object being output.
		/// </summary>
		/// <returns> the temporal object, not null </returns>
		internal TemporalAccessor Temporal
		{
			get
			{
				return Temporal_Renamed;
			}
		}

		/// <summary>
		/// Gets the locale.
		/// <para>
		/// This locale is used to control localization in the format output except
		/// where localization is controlled by the DecimalStyle.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the locale, not null </returns>
		internal Locale Locale
		{
			get
			{
				return Formatter.Locale;
			}
		}

		/// <summary>
		/// Gets the DecimalStyle.
		/// <para>
		/// The DecimalStyle controls the localization of numeric output.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the DecimalStyle, not null </returns>
		internal DecimalStyle DecimalStyle
		{
			get
			{
				return Formatter.DecimalStyle;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Starts the printing of an optional segment of the input.
		/// </summary>
		internal void StartOptional()
		{
			this.Optional++;
		}

		/// <summary>
		/// Ends the printing of an optional segment of the input.
		/// </summary>
		internal void EndOptional()
		{
			this.Optional--;
		}

		/// <summary>
		/// Gets a value using a query.
		/// </summary>
		/// <param name="query">  the query to use, not null </param>
		/// <returns> the result, null if not found and optional is true </returns>
		/// <exception cref="DateTimeException"> if the type is not available and the section is not optional </exception>
		internal R getValue<R>(TemporalQuery<R> query)
		{
			R result = Temporal_Renamed.query(query);
			if (result == null && Optional == 0)
			{
				throw new DateTimeException("Unable to extract value: " + Temporal_Renamed.GetType());
			}
			return result;
		}

		/// <summary>
		/// Gets the value of the specified field.
		/// <para>
		/// This will return the value for the specified field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to find, not null </param>
		/// <returns> the value, null if not found and optional is true </returns>
		/// <exception cref="DateTimeException"> if the field is not available and the section is not optional </exception>
		internal Long GetValue(TemporalField field)
		{
			try
			{
				return Temporal_Renamed.GetLong(field);
			}
			catch (DateTimeException ex)
			{
				if (Optional > 0)
				{
					return null;
				}
				throw ex;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a string version of the context for debugging.
		/// </summary>
		/// <returns> a string representation of the context, not null </returns>
		public override String ToString()
		{
			return Temporal_Renamed.ToString();
		}

	}

}