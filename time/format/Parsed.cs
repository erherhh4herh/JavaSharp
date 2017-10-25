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
 * Copyright (c) 2008-2013, Stephen Colebourne & Michael Nascimento Santos
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
	/// A store of parsed data.
	/// <para>
	/// This class is used during parsing to collect the data. Part of the parsing process
	/// involves handling optional blocks and multiple copies of the data get created to
	/// support the necessary backtracking.
	/// </para>
	/// <para>
	/// Once parsing is completed, this class can be used as the resultant {@code TemporalAccessor}.
	/// In most cases, it is only exposed once the fields have been resolved.
	/// 
	/// @implSpec
	/// This class is a mutable context intended for use from a single thread.
	/// Usage of the class is thread-safe within standard parsing as a new instance of this class
	/// is automatically created for each parse and parsing is single-threaded
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	internal sealed class Parsed : TemporalAccessor
	{
		// some fields are accessed using package scope from DateTimeParseContext

		/// <summary>
		/// The parsed fields.
		/// </summary>
		internal readonly IDictionary<TemporalField, Long> FieldValues = new Dictionary<TemporalField, Long>();
		/// <summary>
		/// The parsed zone.
		/// </summary>
		internal ZoneId Zone;
		/// <summary>
		/// The parsed chronology.
		/// </summary>
		internal Chronology Chrono;
		/// <summary>
		/// Whether a leap-second is parsed.
		/// </summary>
		internal bool LeapSecond;
		/// <summary>
		/// The resolver style to use.
		/// </summary>
		private ResolverStyle ResolverStyle;
		/// <summary>
		/// The resolved date.
		/// </summary>
		private ChronoLocalDate Date;
		/// <summary>
		/// The resolved time.
		/// </summary>
		private LocalTime Time;
		/// <summary>
		/// The excess period from time-only parsing.
		/// </summary>
		internal Period ExcessDays = Period.ZERO;

		/// <summary>
		/// Creates an instance.
		/// </summary>
		internal Parsed()
		{
		}

		/// <summary>
		/// Creates a copy.
		/// </summary>
		internal Parsed Copy()
		{
			// only copy fields used in parsing stage
			Parsed cloned = new Parsed();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			cloned.FieldValues.putAll(this.FieldValues);
			cloned.Zone = this.Zone;
			cloned.Chrono = this.Chrono;
			cloned.LeapSecond = this.LeapSecond;
			return cloned;
		}

		//-----------------------------------------------------------------------
		public bool IsSupported(TemporalField field)
		{
			if (FieldValues.ContainsKey(field) || (Date != java.time.temporal.TemporalAccessor_Fields.Null && Date.IsSupported(field)) || (Time != java.time.temporal.TemporalAccessor_Fields.Null && Time.IsSupported(field)))
			{
				return true;
			}
			return field != java.time.temporal.TemporalAccessor_Fields.Null && (field is ChronoField == false) && field.IsSupportedBy(this);
		}

		public long GetLong(TemporalField field)
		{
			Objects.RequireNonNull(field, "field");
			Long java.time.temporal.TemporalAccessor_Fields.Value = FieldValues[field];
			if (java.time.temporal.TemporalAccessor_Fields.Value != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				return java.time.temporal.TemporalAccessor_Fields.Value;
			}
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null && Date.IsSupported(field))
			{
				return Date.GetLong(field);
			}
			if (Time != java.time.temporal.TemporalAccessor_Fields.Null && Time.IsSupported(field))
			{
				return Time.GetLong(field);
			}
			if (field is ChronoField)
			{
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.GetFrom(this);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <R> R query(java.time.temporal.TemporalQuery<R> query)
		public override R query<R>(TemporalQuery<R> query)
		{
			if (query == TemporalQueries.ZoneId())
			{
				return (R) Zone;
			}
			else if (query == TemporalQueries.Chronology())
			{
				return (R) Chrono;
			}
			else if (query == TemporalQueries.LocalDate())
			{
				return (R)(Date != java.time.temporal.TemporalAccessor_Fields.Null ? LocalDate.From(Date) : java.time.temporal.TemporalAccessor_Fields.Null);
			}
			else if (query == TemporalQueries.LocalTime())
			{
				return (R) Time;
			}
			else if (query == TemporalQueries.Zone() || query == TemporalQueries.Offset())
			{
				return query.QueryFrom(this);
			}
			else if (query == TemporalQueries.Precision())
			{
				return java.time.temporal.TemporalAccessor_Fields.Null; // not a complete date/time
			}
			// inline TemporalAccessor.super.query(query) as an optimization
			// non-JDK classes are not permitted to make this optimization
			return query.QueryFrom(this);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Resolves the fields in this context.
		/// </summary>
		/// <param name="resolverStyle">  the resolver style, not null </param>
		/// <param name="resolverFields">  the fields to use for resolving, null for all fields </param>
		/// <returns> this, for method chaining </returns>
		/// <exception cref="DateTimeException"> if resolving one field results in a value for
		///  another field that is in conflict </exception>
		internal TemporalAccessor Resolve(ResolverStyle resolverStyle, Set<TemporalField> resolverFields)
		{
			if (resolverFields != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				FieldValues.Keys.retainAll(resolverFields);
			}
			this.ResolverStyle = resolverStyle;
			ResolveFields();
			ResolveTimeLenient();
			CrossCheck();
			ResolvePeriod();
			ResolveFractional();
			ResolveInstant();
			return this;
		}

		//-----------------------------------------------------------------------
		private void ResolveFields()
		{
			// resolve ChronoField
			ResolveInstantFields();
			ResolveDateFields();
			ResolveTimeFields();

			// if any other fields, handle them
			// any lenient date resolution should return epoch-day
			if (FieldValues.Count > 0)
			{
				int changedCount = 0;
				while (changedCount < 50)
				{
					foreach (Map_Entry<TemporalField, Long> entry in FieldValues)
					{
						TemporalField targetField = entry.Key;
						TemporalAccessor resolvedObject = targetField.resolve(FieldValues, this, ResolverStyle);
						if (resolvedObject != java.time.temporal.TemporalAccessor_Fields.Null)
						{
							if (resolvedObject is ChronoZonedDateTime)
							{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.time.chrono.ChronoZonedDateTime<?> czdt = (java.time.chrono.ChronoZonedDateTime<?>) resolvedObject;
								ChronoZonedDateTime<?> czdt = (ChronoZonedDateTime<?>) resolvedObject;
								if (Zone == java.time.temporal.TemporalAccessor_Fields.Null)
								{
									Zone = czdt.Zone;
								}
								else if (Zone.Equals(czdt.Zone) == false)
								{
									throw new DateTimeException("ChronoZonedDateTime must use the effective parsed zone: " + Zone);
								}
								resolvedObject = czdt.ToLocalDateTime();
							}
							if (resolvedObject is ChronoLocalDateTime)
							{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.time.chrono.ChronoLocalDateTime<?> cldt = (java.time.chrono.ChronoLocalDateTime<?>) resolvedObject;
								ChronoLocalDateTime<?> cldt = (ChronoLocalDateTime<?>) resolvedObject;
								UpdateCheckConflict(cldt.ToLocalTime(), Period.ZERO);
								UpdateCheckConflict(cldt.ToLocalDate());
								changedCount++;
								goto outerContinue; // have to restart to avoid concurrent modification
							}
							if (resolvedObject is ChronoLocalDate)
							{
								UpdateCheckConflict((ChronoLocalDate) resolvedObject);
								changedCount++;
								goto outerContinue; // have to restart to avoid concurrent modification
							}
							if (resolvedObject is LocalTime)
							{
								UpdateCheckConflict((LocalTime) resolvedObject, Period.ZERO);
								changedCount++;
								goto outerContinue; // have to restart to avoid concurrent modification
							}
							throw new DateTimeException("Method resolve() can only return ChronoZonedDateTime, " + "ChronoLocalDateTime, ChronoLocalDate or LocalTime");
						}
						else if (FieldValues.ContainsKey(targetField) == false)
						{
							changedCount++;
							goto outerContinue; // have to restart to avoid concurrent modification
						}
					}
					break;
					outerContinue:;
				}
				outerBreak:
				if (changedCount == 50) // catch infinite loops
				{
					throw new DateTimeException("One of the parsed fields has an incorrectly implemented resolve method");
				}
				// if something changed then have to redo ChronoField resolve
				if (changedCount > 0)
				{
					ResolveInstantFields();
					ResolveDateFields();
					ResolveTimeFields();
				}
			}
		}

		private void UpdateCheckConflict(TemporalField targetField, TemporalField changeField, Long changeValue)
		{
			Long old = FieldValues[changeField] = changeValue;
			if (old != java.time.temporal.TemporalAccessor_Fields.Null && old.LongValue() != changeValue.LongValue())
			{
				throw new DateTimeException("Conflict found: " + changeField + " " + old + " differs from " + changeField + " " + changeValue + " while resolving  " + targetField);
			}
		}

		//-----------------------------------------------------------------------
		private void ResolveInstantFields()
		{
			// resolve parsed instant seconds to date and time if zone available
			if (FieldValues.ContainsKey(INSTANT_SECONDS))
			{
				if (Zone != java.time.temporal.TemporalAccessor_Fields.Null)
				{
					ResolveInstantFields0(Zone);
				}
				else
				{
					Long offsetSecs = FieldValues[OFFSET_SECONDS];
					if (offsetSecs != java.time.temporal.TemporalAccessor_Fields.Null)
					{
						ZoneOffset offset = ZoneOffset.OfTotalSeconds(offsetSecs.IntValue());
						ResolveInstantFields0(offset);
					}
				}
			}
		}

		private void ResolveInstantFields0(ZoneId selectedZone)
		{
			Instant instant = Instant.OfEpochSecond(FieldValues.Remove(INSTANT_SECONDS));
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.time.chrono.ChronoZonedDateTime<?> zdt = chrono.zonedDateTime(instant, selectedZone);
			ChronoZonedDateTime<?> zdt = Chrono.zonedDateTime(instant, selectedZone);
			UpdateCheckConflict(zdt.toLocalDate());
			UpdateCheckConflict(INSTANT_SECONDS, SECOND_OF_DAY, (long) zdt.toLocalTime().toSecondOfDay());
		}

		//-----------------------------------------------------------------------
		private void ResolveDateFields()
		{
			UpdateCheckConflict(Chrono.ResolveDate(FieldValues, ResolverStyle));
		}

		private void UpdateCheckConflict(ChronoLocalDate cld)
		{
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				if (cld != java.time.temporal.TemporalAccessor_Fields.Null && Date.Equals(cld) == false)
				{
					throw new DateTimeException("Conflict found: Fields resolved to two different dates: " + Date + " " + cld);
				}
			}
			else if (cld != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				if (Chrono.Equals(cld.Chronology) == false)
				{
					throw new DateTimeException("ChronoLocalDate must use the effective parsed chronology: " + Chrono);
				}
				Date = cld;
			}
		}

		//-----------------------------------------------------------------------
		private void ResolveTimeFields()
		{
			// simplify fields
			if (FieldValues.ContainsKey(CLOCK_HOUR_OF_DAY))
			{
				// lenient allows anything, smart allows 0-24, strict allows 1-24
				long ch = FieldValues.Remove(CLOCK_HOUR_OF_DAY);
				if (ResolverStyle == ResolverStyle.STRICT || (ResolverStyle == ResolverStyle.SMART && ch != 0))
				{
					CLOCK_HOUR_OF_DAY.checkValidValue(ch);
				}
				UpdateCheckConflict(CLOCK_HOUR_OF_DAY, HOUR_OF_DAY, ch == 24 ? 0 : ch);
			}
			if (FieldValues.ContainsKey(CLOCK_HOUR_OF_AMPM))
			{
				// lenient allows anything, smart allows 0-12, strict allows 1-12
				long ch = FieldValues.Remove(CLOCK_HOUR_OF_AMPM);
				if (ResolverStyle == ResolverStyle.STRICT || (ResolverStyle == ResolverStyle.SMART && ch != 0))
				{
					CLOCK_HOUR_OF_AMPM.checkValidValue(ch);
				}
				UpdateCheckConflict(CLOCK_HOUR_OF_AMPM, HOUR_OF_AMPM, ch == 12 ? 0 : ch);
			}
			if (FieldValues.ContainsKey(AMPM_OF_DAY) && FieldValues.ContainsKey(HOUR_OF_AMPM))
			{
				long ap = FieldValues.Remove(AMPM_OF_DAY);
				long hap = FieldValues.Remove(HOUR_OF_AMPM);
				if (ResolverStyle == ResolverStyle.LENIENT)
				{
					UpdateCheckConflict(AMPM_OF_DAY, HOUR_OF_DAY, Math.AddExact(Math.MultiplyExact(ap, 12), hap));
				} // STRICT or SMART
				else
				{
					AMPM_OF_DAY.checkValidValue(ap);
					HOUR_OF_AMPM.checkValidValue(ap);
					UpdateCheckConflict(AMPM_OF_DAY, HOUR_OF_DAY, ap * 12 + hap);
				}
			}
			if (FieldValues.ContainsKey(NANO_OF_DAY))
			{
				long nod = FieldValues.Remove(NANO_OF_DAY);
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					NANO_OF_DAY.checkValidValue(nod);
				}
				UpdateCheckConflict(NANO_OF_DAY, HOUR_OF_DAY, nod / 3600_000_000_000L);
				UpdateCheckConflict(NANO_OF_DAY, MINUTE_OF_HOUR, (nod / 60_000_000_000L) % 60);
				UpdateCheckConflict(NANO_OF_DAY, SECOND_OF_MINUTE, (nod / 1_000_000_000L) % 60);
				UpdateCheckConflict(NANO_OF_DAY, NANO_OF_SECOND, nod % 1_000_000_000L);
			}
			if (FieldValues.ContainsKey(MICRO_OF_DAY))
			{
				long cod = FieldValues.Remove(MICRO_OF_DAY);
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					MICRO_OF_DAY.checkValidValue(cod);
				}
				UpdateCheckConflict(MICRO_OF_DAY, SECOND_OF_DAY, cod / 1_000_000L);
				UpdateCheckConflict(MICRO_OF_DAY, MICRO_OF_SECOND, cod % 1_000_000L);
			}
			if (FieldValues.ContainsKey(MILLI_OF_DAY))
			{
				long lod = FieldValues.Remove(MILLI_OF_DAY);
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					MILLI_OF_DAY.checkValidValue(lod);
				}
				UpdateCheckConflict(MILLI_OF_DAY, SECOND_OF_DAY, lod / 1000);
				UpdateCheckConflict(MILLI_OF_DAY, MILLI_OF_SECOND, lod % 1000);
			}
			if (FieldValues.ContainsKey(SECOND_OF_DAY))
			{
				long sod = FieldValues.Remove(SECOND_OF_DAY);
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					SECOND_OF_DAY.checkValidValue(sod);
				}
				UpdateCheckConflict(SECOND_OF_DAY, HOUR_OF_DAY, sod / 3600);
				UpdateCheckConflict(SECOND_OF_DAY, MINUTE_OF_HOUR, (sod / 60) % 60);
				UpdateCheckConflict(SECOND_OF_DAY, SECOND_OF_MINUTE, sod % 60);
			}
			if (FieldValues.ContainsKey(MINUTE_OF_DAY))
			{
				long mod = FieldValues.Remove(MINUTE_OF_DAY);
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					MINUTE_OF_DAY.checkValidValue(mod);
				}
				UpdateCheckConflict(MINUTE_OF_DAY, HOUR_OF_DAY, mod / 60);
				UpdateCheckConflict(MINUTE_OF_DAY, MINUTE_OF_HOUR, mod % 60);
			}

			// combine partial second fields strictly, leaving lenient expansion to later
			if (FieldValues.ContainsKey(NANO_OF_SECOND))
			{
				long nos = FieldValues[NANO_OF_SECOND];
				if (ResolverStyle != ResolverStyle.LENIENT)
				{
					NANO_OF_SECOND.checkValidValue(nos);
				}
				if (FieldValues.ContainsKey(MICRO_OF_SECOND))
				{
					long cos = FieldValues.Remove(MICRO_OF_SECOND);
					if (ResolverStyle != ResolverStyle.LENIENT)
					{
						MICRO_OF_SECOND.checkValidValue(cos);
					}
					nos = cos * 1000 + (nos % 1000);
					UpdateCheckConflict(MICRO_OF_SECOND, NANO_OF_SECOND, nos);
				}
				if (FieldValues.ContainsKey(MILLI_OF_SECOND))
				{
					long los = FieldValues.Remove(MILLI_OF_SECOND);
					if (ResolverStyle != ResolverStyle.LENIENT)
					{
						MILLI_OF_SECOND.checkValidValue(los);
					}
					UpdateCheckConflict(MILLI_OF_SECOND, NANO_OF_SECOND, los * 1_000_000L + (nos % 1_000_000L));
				}
			}

			// convert to time if all four fields available (optimization)
			if (FieldValues.ContainsKey(HOUR_OF_DAY) && FieldValues.ContainsKey(MINUTE_OF_HOUR) && FieldValues.ContainsKey(SECOND_OF_MINUTE) && FieldValues.ContainsKey(NANO_OF_SECOND))
			{
				long hod = FieldValues.Remove(HOUR_OF_DAY);
				long moh = FieldValues.Remove(MINUTE_OF_HOUR);
				long som = FieldValues.Remove(SECOND_OF_MINUTE);
				long nos = FieldValues.Remove(NANO_OF_SECOND);
				ResolveTime(hod, moh, som, nos);
			}
		}

		private void ResolveTimeLenient()
		{
			// leniently create a time from incomplete information
			// done after everything else as it creates information from nothing
			// which would break updateCheckConflict(field)

			if (Time == java.time.temporal.TemporalAccessor_Fields.Null)
			{
				// NANO_OF_SECOND merged with MILLI/MICRO above
				if (FieldValues.ContainsKey(MILLI_OF_SECOND))
				{
					long los = FieldValues.Remove(MILLI_OF_SECOND);
					if (FieldValues.ContainsKey(MICRO_OF_SECOND))
					{
						// merge milli-of-second and micro-of-second for better error message
						long cos = los * 1000 + (FieldValues[MICRO_OF_SECOND] % 1000);
						UpdateCheckConflict(MILLI_OF_SECOND, MICRO_OF_SECOND, cos);
						FieldValues.Remove(MICRO_OF_SECOND);
						FieldValues[NANO_OF_SECOND] = cos * 1_000L;
					}
					else
					{
						// convert milli-of-second to nano-of-second
						FieldValues[NANO_OF_SECOND] = los * 1_000_000L;
					}
				}
				else if (FieldValues.ContainsKey(MICRO_OF_SECOND))
				{
					// convert micro-of-second to nano-of-second
					long cos = FieldValues.Remove(MICRO_OF_SECOND);
					FieldValues[NANO_OF_SECOND] = cos * 1_000L;
				}

				// merge hour/minute/second/nano leniently
				Long hod = FieldValues[HOUR_OF_DAY];
				if (hod != java.time.temporal.TemporalAccessor_Fields.Null)
				{
					Long moh = FieldValues[MINUTE_OF_HOUR];
					Long som = FieldValues[SECOND_OF_MINUTE];
					Long nos = FieldValues[NANO_OF_SECOND];

					// check for invalid combinations that cannot be defaulted
					if ((moh == java.time.temporal.TemporalAccessor_Fields.Null && (som != java.time.temporal.TemporalAccessor_Fields.Null || nos != java.time.temporal.TemporalAccessor_Fields.Null)) || (moh != java.time.temporal.TemporalAccessor_Fields.Null && som == java.time.temporal.TemporalAccessor_Fields.Null && nos != java.time.temporal.TemporalAccessor_Fields.Null))
					{
						return;
					}

					// default as necessary and build time
					long mohVal = (moh != java.time.temporal.TemporalAccessor_Fields.Null ? moh : 0);
					long somVal = (som != java.time.temporal.TemporalAccessor_Fields.Null ? som : 0);
					long nosVal = (nos != java.time.temporal.TemporalAccessor_Fields.Null ? nos : 0);
					ResolveTime(hod, mohVal, somVal, nosVal);
					FieldValues.Remove(HOUR_OF_DAY);
					FieldValues.Remove(MINUTE_OF_HOUR);
					FieldValues.Remove(SECOND_OF_MINUTE);
					FieldValues.Remove(NANO_OF_SECOND);
				}
			}

			// validate remaining
			if (ResolverStyle != ResolverStyle.LENIENT && FieldValues.Count > 0)
			{
				foreach (Map_Entry<TemporalField, Long> entry in FieldValues)
				{
					TemporalField field = entry.Key;
					if (field is ChronoField && field.TimeBased)
					{
						((ChronoField) field).checkValidValue(entry.Value);
					}
				}
			}
		}

		private void ResolveTime(long hod, long moh, long som, long nos)
		{
			if (ResolverStyle == ResolverStyle.LENIENT)
			{
				long totalNanos = Math.MultiplyExact(hod, 3600_000_000_000L);
				totalNanos = Math.AddExact(totalNanos, Math.MultiplyExact(moh, 60_000_000_000L));
				totalNanos = Math.AddExact(totalNanos, Math.MultiplyExact(som, 1_000_000_000L));
				totalNanos = Math.AddExact(totalNanos, nos);
				int excessDays = (int) Math.FloorDiv(totalNanos, 86400_000_000_000L); // safe int cast
				long nod = Math.FloorMod(totalNanos, 86400_000_000_000L);
				UpdateCheckConflict(LocalTime.OfNanoOfDay(nod), Period.OfDays(excessDays));
			} // STRICT or SMART
			else
			{
				int mohVal = MINUTE_OF_HOUR.checkValidIntValue(moh);
				int nosVal = NANO_OF_SECOND.checkValidIntValue(nos);
				// handle 24:00 end of day
				if (ResolverStyle == ResolverStyle.SMART && hod == 24 && mohVal == 0 && som == 0 && nosVal == 0)
				{
					UpdateCheckConflict(LocalTime.MIDNIGHT, Period.OfDays(1));
				}
				else
				{
					int hodVal = HOUR_OF_DAY.checkValidIntValue(hod);
					int somVal = SECOND_OF_MINUTE.checkValidIntValue(som);
					UpdateCheckConflict(LocalTime.Of(hodVal, mohVal, somVal, nosVal), Period.ZERO);
				}
			}
		}

		private void ResolvePeriod()
		{
			// add whole days if we have both date and time
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null && Time != java.time.temporal.TemporalAccessor_Fields.Null && ExcessDays.Zero == false)
			{
				Date = Date.plus(ExcessDays);
				ExcessDays = Period.ZERO;
			}
		}

		private void ResolveFractional()
		{
			// ensure fractional seconds available as ChronoField requires
			// resolveTimeLenient() will have merged MICRO_OF_SECOND/MILLI_OF_SECOND to NANO_OF_SECOND
			if (Time == java.time.temporal.TemporalAccessor_Fields.Null && (FieldValues.ContainsKey(INSTANT_SECONDS) || FieldValues.ContainsKey(SECOND_OF_DAY) || FieldValues.ContainsKey(SECOND_OF_MINUTE)))
			{
				if (FieldValues.ContainsKey(NANO_OF_SECOND))
				{
					long nos = FieldValues[NANO_OF_SECOND];
					FieldValues[MICRO_OF_SECOND] = nos / 1000;
					FieldValues[MILLI_OF_SECOND] = nos / 1000000;
				}
				else
				{
					FieldValues[NANO_OF_SECOND] = 0L;
					FieldValues[MICRO_OF_SECOND] = 0L;
					FieldValues[MILLI_OF_SECOND] = 0L;
				}
			}
		}

		private void ResolveInstant()
		{
			// add instant seconds if we have date, time and zone
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null && Time != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				if (Zone != java.time.temporal.TemporalAccessor_Fields.Null)
				{
					long instant = Date.atTime(Time).atZone(Zone).getLong(ChronoField.INSTANT_SECONDS);
					FieldValues[INSTANT_SECONDS] = instant;
				}
				else
				{
					Long offsetSecs = FieldValues[OFFSET_SECONDS];
					if (offsetSecs != java.time.temporal.TemporalAccessor_Fields.Null)
					{
						ZoneOffset offset = ZoneOffset.OfTotalSeconds(offsetSecs.IntValue());
						long instant = Date.atTime(Time).atZone(offset).getLong(ChronoField.INSTANT_SECONDS);
						FieldValues[INSTANT_SECONDS] = instant;
					}
				}
			}
		}

		private void UpdateCheckConflict(LocalTime timeToSet, Period periodToSet)
		{
			if (Time != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				if (Time.Equals(timeToSet) == false)
				{
					throw new DateTimeException("Conflict found: Fields resolved to different times: " + Time + " " + timeToSet);
				}
				if (ExcessDays.Zero == false && periodToSet.Zero == false && ExcessDays.Equals(periodToSet) == false)
				{
					throw new DateTimeException("Conflict found: Fields resolved to different excess periods: " + ExcessDays + " " + periodToSet);
				}
				else
				{
					ExcessDays = periodToSet;
				}
			}
			else
			{
				Time = timeToSet;
				ExcessDays = periodToSet;
			}
		}

		//-----------------------------------------------------------------------
		private void CrossCheck()
		{
			// only cross-check date, time and date-time
			// avoid object creation if possible
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				CrossCheck(Date);
			}
			if (Time != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				CrossCheck(Time);
				if (Date != java.time.temporal.TemporalAccessor_Fields.Null && FieldValues.Count > 0)
				{
					CrossCheck(Date.atTime(Time));
				}
			}
		}

		private void CrossCheck(TemporalAccessor target)
		{
			for (IEnumerator<Map_Entry<TemporalField, Long>> it = FieldValues.GetEnumerator(); it.MoveNext();)
			{
				Map_Entry<TemporalField, Long> entry = it.Current;
				TemporalField field = entry.Key;
				if (target.IsSupported(field))
				{
					long val1;
					try
					{
						val1 = target.GetLong(field);
					}
					catch (RuntimeException)
					{
						continue;
					}
					long val2 = entry.Value;
					if (val1 != val2)
					{
						throw new DateTimeException("Conflict found: Field " + field + " " + val1 + " differs from " + field + " " + val2 + " derived from " + target);
					}
					it.remove();
				}
			}
		}

		//-----------------------------------------------------------------------
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder(64);
			buf.Append(FieldValues).Append(',').Append(Chrono);
			if (Zone != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				buf.Append(',').Append(Zone);
			}
			if (Date != java.time.temporal.TemporalAccessor_Fields.Null || Time != java.time.temporal.TemporalAccessor_Fields.Null)
			{
				buf.Append(" resolved to ");
				if (Date != java.time.temporal.TemporalAccessor_Fields.Null)
				{
					buf.Append(Date);
					if (Time != java.time.temporal.TemporalAccessor_Fields.Null)
					{
						buf.Append('T').Append(Time);
					}
				}
				else
				{
					buf.Append(Time);
				}
			}
			return buf.ToString();
		}

	}

}