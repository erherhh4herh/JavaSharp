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
namespace java.time.chrono
{



	/// <summary>
	/// A date-time with a time-zone in an arbitrary chronology,
	/// intended for advanced globalization use cases.
	/// <para>
	/// <b>Most applications should declare method signatures, fields and variables
	/// as <seealso cref="ZonedDateTime"/>, not this interface.</b>
	/// </para>
	/// <para>
	/// A {@code ChronoZonedDateTime} is the abstract representation of an offset date-time
	/// where the {@code Chronology chronology}, or calendar system, is pluggable.
	/// The date-time is defined in terms of fields expressed by <seealso cref="TemporalField"/>,
	/// where most common implementations are defined in <seealso cref="ChronoField"/>.
	/// The chronology defines how the calendar system operates and the meaning of
	/// the standard fields.
	/// 
	/// <h3>When to use this interface</h3>
	/// The design of the API encourages the use of {@code ZonedDateTime} rather than this
	/// interface, even in the case where the application needs to deal with multiple
	/// calendar systems. The rationale for this is explored in detail in <seealso cref="ChronoLocalDate"/>.
	/// </para>
	/// <para>
	/// Ensure that the discussion in {@code ChronoLocalDate} has been read and understood
	/// before using this interface.
	/// 
	/// @implSpec
	/// This interface must be implemented with care to ensure other classes operate correctly.
	/// All implementations that can be instantiated must be final, immutable and thread-safe.
	/// Subclasses should be Serializable wherever possible.
	/// 
	/// </para>
	/// </summary>
	/// @param <D> the concrete type for the date of this date-time
	/// @since 1.8 </param>
	public interface ChronoZonedDateTime<D> : Temporal, Comparable<ChronoZonedDateTime<JavaToDotNetGenericWildcard>> where D : ChronoLocalDate
	{

		/// <summary>
		/// Gets a comparator that compares {@code ChronoZonedDateTime} in
		/// time-line order ignoring the chronology.
		/// <para>
		/// This comparator differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying instant and not the chronology.
		/// This allows dates in different calendar systems to be compared based
		/// on the position of the date-time on the instant time-line.
		/// The underlying comparison is equivalent to comparing the epoch-second and nano-of-second.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a comparator that compares in time-line order ignoring the chronology </returns>
		/// <seealso cref= #isAfter </seealso>
		/// <seealso cref= #isBefore </seealso>
		/// <seealso cref= #isEqual </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static java.util.Comparator<ChronoZonedDateTime<JavaToDotNetGenericWildcard>> timeLineOrder()
	//	{
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ChronoZonedDateTime} from a temporal object.
		/// <para>
		/// This creates a zoned date-time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code ChronoZonedDateTime}.
		/// </para>
		/// <para>
		/// The conversion extracts and combines the chronology, date, time and zone
		/// from the temporal object. The behavior is equivalent to using
		/// <seealso cref="Chronology#zonedDateTime(TemporalAccessor)"/> with the extracted chronology.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code ChronoZonedDateTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code ChronoZonedDateTime} </exception>
		/// <seealso cref= Chronology#zonedDateTime(TemporalAccessor) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static ChronoZonedDateTime<JavaToDotNetGenericWildcard> from(java.time.temporal.TemporalAccessor temporal)
	//	{
	//		if (temporal instanceof ChronoZonedDateTime)
	//		{
	//			return (ChronoZonedDateTime<?>) temporal;
	//		}
	//		Objects.requireNonNull(temporal, "temporal");
	//		if (chrono == null)
	//		{
	//			throw new DateTimeException("Unable to obtain ChronoZonedDateTime from TemporalAccessor: " + temporal.getClass());
	//		}
	//		return chrono.zonedDateTime(temporal);
	//	}

		//-----------------------------------------------------------------------
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.time.temporal.ValueRange range(java.time.temporal.TemporalField field)
	//	{
	//		if (field instanceof ChronoField)
	//		{
	//			if (field == INSTANT_SECONDS || field == OFFSET_SECONDS)
	//			{
	//				return field.range();
	//			}
	//			return toLocalDateTime().range(field);
	//		}
	//		return field.rangeRefinedBy(this);
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default int get(java.time.temporal.TemporalField field)
	//	{
	//		if (field instanceof ChronoField)
	//		{
	//			switch ((ChronoField) field)
	//			{
	//				case INSTANT_SECONDS:
	//					throw new UnsupportedTemporalTypeException("Invalid field 'InstantSeconds' for get() method, use getLong() instead");
	//				case OFFSET_SECONDS:
	//					return getOffset().getTotalSeconds();
	//			}
	//			return toLocalDateTime().get(field);
	//		}
	//		return Temporal.this.get(field);
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default long getLong(java.time.temporal.TemporalField field)
	//	{
	//		if (field instanceof ChronoField)
	//		{
	//			switch ((ChronoField) field)
	//			{
	//				case INSTANT_SECONDS:
	//					return toEpochSecond();
	//				case OFFSET_SECONDS:
	//					return getOffset().getTotalSeconds();
	//			}
	//			return toLocalDateTime().getLong(field);
	//		}
	//		return field.getFrom(this);
	//	}

		/// <summary>
		/// Gets the local date part of this date-time.
		/// <para>
		/// This returns a local date with the same year, month and day
		/// as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the date part of this date-time, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default D toLocalDate()
	//	{
	//		return toLocalDateTime().toLocalDate();
	//	}

		/// <summary>
		/// Gets the local time part of this date-time.
		/// <para>
		/// This returns a local time with the same hour, minute, second and
		/// nanosecond as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time part of this date-time, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.time.LocalTime toLocalTime()
	//	{
	//		return toLocalDateTime().toLocalTime();
	//	}

		/// <summary>
		/// Gets the local date-time part of this date-time.
		/// <para>
		/// This returns a local date with the same year, month and day
		/// as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the local date-time part of this date-time, not null </returns>
		ChronoLocalDateTime<D> ToLocalDateTime();

		/// <summary>
		/// Gets the chronology of this date-time.
		/// <para>
		/// The {@code Chronology} represents the calendar system in use.
		/// The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Chronology getChronology()
	//	{
	//		return toLocalDate().getChronology();
	//	}

		/// <summary>
		/// Gets the zone offset, such as '+01:00'.
		/// <para>
		/// This is the offset of the local date-time from UTC/Greenwich.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the zone offset, not null </returns>
		ZoneOffset Offset {get;}

		/// <summary>
		/// Gets the zone ID, such as 'Europe/Paris'.
		/// <para>
		/// This returns the stored time-zone id used to determine the time-zone rules.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the zone ID, not null </returns>
		ZoneId Zone {get;}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date-time changing the zone offset to the
		/// earlier of the two valid offsets at a local time-line overlap.
		/// <para>
		/// This method only has any effect when the local time-line overlaps, such as
		/// at an autumn daylight savings cutover. In this scenario, there are two
		/// valid offsets for the local date-time. Calling this method will return
		/// a zoned date-time with the earlier of the two selected.
		/// </para>
		/// <para>
		/// If this method is called when it is not an overlap, {@code this}
		/// is returned.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ChronoZonedDateTime} based on this date-time with the earlier offset, not null </returns>
		/// <exception cref="DateTimeException"> if no rules can be found for the zone </exception>
		/// <exception cref="DateTimeException"> if no rules are valid for this date-time </exception>
		ChronoZonedDateTime<D> WithEarlierOffsetAtOverlap();

		/// <summary>
		/// Returns a copy of this date-time changing the zone offset to the
		/// later of the two valid offsets at a local time-line overlap.
		/// <para>
		/// This method only has any effect when the local time-line overlaps, such as
		/// at an autumn daylight savings cutover. In this scenario, there are two
		/// valid offsets for the local date-time. Calling this method will return
		/// a zoned date-time with the later of the two selected.
		/// </para>
		/// <para>
		/// If this method is called when it is not an overlap, {@code this}
		/// is returned.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ChronoZonedDateTime} based on this date-time with the later offset, not null </returns>
		/// <exception cref="DateTimeException"> if no rules can be found for the zone </exception>
		/// <exception cref="DateTimeException"> if no rules are valid for this date-time </exception>
		ChronoZonedDateTime<D> WithLaterOffsetAtOverlap();

		/// <summary>
		/// Returns a copy of this date-time with a different time-zone,
		/// retaining the local date-time if possible.
		/// <para>
		/// This method changes the time-zone and retains the local date-time.
		/// The local date-time is only changed if it is invalid for the new zone.
		/// </para>
		/// <para>
		/// To change the zone and adjust the local date-time,
		/// use <seealso cref="#withZoneSameInstant(ZoneId)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to change to, not null </param>
		/// <returns> a {@code ChronoZonedDateTime} based on this date-time with the requested zone, not null </returns>
		ChronoZonedDateTime<D> WithZoneSameLocal(ZoneId zone);

		/// <summary>
		/// Returns a copy of this date-time with a different time-zone,
		/// retaining the instant.
		/// <para>
		/// This method changes the time-zone and retains the instant.
		/// This normally results in a change to the local date-time.
		/// </para>
		/// <para>
		/// This method is based on retaining the same instant, thus gaps and overlaps
		/// in the local time-line have no effect on the result.
		/// </para>
		/// <para>
		/// To change the offset while keeping the local time,
		/// use <seealso cref="#withZoneSameLocal(ZoneId)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to change to, not null </param>
		/// <returns> a {@code ChronoZonedDateTime} based on this date-time with the requested zone, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		ChronoZonedDateTime<D> WithZoneSameInstant(ZoneId zone);

		/// <summary>
		/// Checks if the specified field is supported.
		/// <para>
		/// This checks if the specified field can be queried on this date-time.
		/// If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		/// <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		/// methods will throw an exception.
		/// </para>
		/// <para>
		/// The set of supported fields is defined by the chronology and normally includes
		/// all {@code ChronoField} fields.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the field is supported is determined by the field.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to check, null returns false </param>
		/// <returns> true if the field can be queried, false if not </returns>
		bool IsSupported(TemporalField field);

		/// <summary>
		/// Checks if the specified unit is supported.
		/// <para>
		/// This checks if the specified unit can be added to or subtracted from this date-time.
		/// If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		/// <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		/// </para>
		/// <para>
		/// The set of supported units is defined by the chronology and normally includes
		/// all {@code ChronoUnit} units except {@code FOREVER}.
		/// </para>
		/// <para>
		/// If the unit is not a {@code ChronoUnit}, then the result of this method
		/// is obtained by invoking {@code TemporalUnit.isSupportedBy(Temporal)}
		/// passing {@code this} as the argument.
		/// Whether the unit is supported is determined by the unit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">  the unit to check, null returns false </param>
		/// <returns> true if the unit can be added/subtracted, false if not </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isSupported(java.time.temporal.TemporalUnit unit)
	//	{
	//		if (unit instanceof ChronoUnit)
	//		{
	//			return unit != FOREVER;
	//		}
	//		return unit != null && unit.isSupportedBy(this);
	//	}

		//-----------------------------------------------------------------------
		// override for covariant return type
		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoZonedDateTime<D> with(java.time.temporal.TemporalAdjuster adjuster)
	//	{
	//		return ChronoZonedDateTimeImpl.ensureValid(getChronology(), Temporal.this.with(adjuster));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		ChronoZonedDateTime<D> With(TemporalField field, long newValue);

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoZonedDateTime<D> plus(java.time.temporal.TemporalAmount amount)
	//	{
	//		return ChronoZonedDateTimeImpl.ensureValid(getChronology(), Temporal.this.plus(amount));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		ChronoZonedDateTime<D> Plus(long amountToAdd, TemporalUnit unit);

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoZonedDateTime<D> minus(java.time.temporal.TemporalAmount amount)
	//	{
	//		return ChronoZonedDateTimeImpl.ensureValid(getChronology(), Temporal.this.minus(amount));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoZonedDateTime<D> minus(long amountToSubtract, java.time.temporal.TemporalUnit unit)
	//	{
	//		return ChronoZonedDateTimeImpl.ensureValid(getChronology(), Temporal.this.minus(amountToSubtract, unit));
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Queries this date-time using the specified query.
		/// <para>
		/// This queries this date-time using the specified query strategy object.
		/// The {@code TemporalQuery} object defines the logic to be used to
		/// obtain the result. Read the documentation of the query to understand
		/// what the result of this method will be.
		/// </para>
		/// <para>
		/// The result of this method is obtained by invoking the
		/// <seealso cref="TemporalQuery#queryFrom(TemporalAccessor)"/> method on the
		/// specified query passing {@code this} as the argument.
		/// 
		/// </para>
		/// </summary>
		/// @param <R> the type of the result </param>
		/// <param name="query">  the query to invoke, not null </param>
		/// <returns> the query result, null may be returned (defined by the query) </returns>
		/// <exception cref="DateTimeException"> if unable to query (defined by the query) </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs (defined by the query) </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override default <R> R query(java.time.temporal.TemporalQuery<R> query)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default <R> R query(java.time.temporal.TemporalQuery<R> query)
	//	{
	//		if (query == TemporalQueries.zone() || query == TemporalQueries.zoneId())
	//		{
	//			return (R) getZone();
	//		}
	//		else if (query == TemporalQueries.offset())
	//		{
	//			return (R) getOffset();
	//		}
	//		else if (query == TemporalQueries.localTime())
	//		{
	//			return (R) toLocalTime();
	//		}
	//		else if (query == TemporalQueries.chronology())
	//		{
	//			return (R) getChronology();
	//		}
	//		else if (query == TemporalQueries.precision())
	//		{
	//			return (R) NANOS;
	//		}
	//		// inline TemporalAccessor.super.query(query) as an optimization
	//		// non-JDK classes are not permitted to make this optimization
	//		return query.queryFrom(this);
	//	}

		/// <summary>
		/// Formats this date-time using the specified formatter.
		/// <para>
		/// This date-time will be passed to the formatter to produce a string.
		/// </para>
		/// <para>
		/// The default implementation must behave as follows:
		/// <pre>
		///  return formatter.format(this);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatter">  the formatter to use, not null </param>
		/// <returns> the formatted date-time string, not null </returns>
		/// <exception cref="DateTimeException"> if an error occurs during printing </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default String format(java.time.format.DateTimeFormatter formatter)
	//	{
	//		Objects.requireNonNull(formatter, "formatter");
	//		return formatter.format(this);
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Converts this date-time to an {@code Instant}.
		/// <para>
		/// This returns an {@code Instant} representing the same point on the
		/// time-line as this date-time. The calculation combines the
		/// <seealso cref="#toLocalDateTime() local date-time"/> and
		/// <seealso cref="#getOffset() offset"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code Instant} representing the same instant, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.time.Instant toInstant()
	//	{
	//		return Instant.ofEpochSecond(toEpochSecond(), toLocalTime().getNano());
	//	}

		/// <summary>
		/// Converts this date-time to the number of seconds from the epoch
		/// of 1970-01-01T00:00:00Z.
		/// <para>
		/// This uses the <seealso cref="#toLocalDateTime() local date-time"/> and
		/// <seealso cref="#getOffset() offset"/> to calculate the epoch-second value,
		/// which is the number of elapsed seconds from 1970-01-01T00:00:00Z.
		/// Instants on the time-line after the epoch are positive, earlier are negative.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default long toEpochSecond()
	//	{
	//		secs -= getOffset().getTotalSeconds();
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this date-time to another date-time, including the chronology.
		/// <para>
		/// The comparison is based first on the instant, then on the local date-time,
		/// then on the zone ID, then on the chronology.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// If all the date-time objects being compared are in the same chronology, then the
		/// additional chronology stage is not required.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison defined above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default int compareTo(ChronoZonedDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		if (cmp == 0)
	//		{
	//			cmp = toLocalTime().getNano() - other.toLocalTime().getNano();
	//			if (cmp == 0)
	//			{
	//				cmp = toLocalDateTime().compareTo(other.toLocalDateTime());
	//				if (cmp == 0)
	//				{
	//					cmp = getZone().getId().compareTo(other.getZone().getId());
	//					if (cmp == 0)
	//					{
	//						cmp = getChronology().compareTo(other.getChronology());
	//					}
	//				}
	//			}
	//		}
	//	}

		/// <summary>
		/// Checks if the instant of this date-time is before that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().isBefore(dateTime2.toInstant());}.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-second
		/// and nano-of-second.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this point is before the specified date-time </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isBefore(ChronoZonedDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		return thisEpochSec < otherEpochSec || (thisEpochSec == otherEpochSec && toLocalTime().getNano() < other.toLocalTime().getNano());
	//	}

		/// <summary>
		/// Checks if the instant of this date-time is after that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().isAfter(dateTime2.toInstant());}.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-second
		/// and nano-of-second.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this is after the specified date-time </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isAfter(ChronoZonedDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		return thisEpochSec > otherEpochSec || (thisEpochSec == otherEpochSec && toLocalTime().getNano() > other.toLocalTime().getNano());
	//	}

		/// <summary>
		/// Checks if the instant of this date-time is equal to that of the specified date-time.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		/// in that it only compares the instant of the date-time. This is equivalent to using
		/// {@code dateTime1.toInstant().equals(dateTime2.toInstant());}.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-second
		/// and nano-of-second.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if the instant equals the instant of the specified date-time </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isEqual(ChronoZonedDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		return toEpochSecond() == other.toEpochSecond() && toLocalTime().getNano() == other.toLocalTime().getNano();
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this date-time is equal to another date-time.
		/// <para>
		/// The comparison is based on the offset date-time and the zone.
		/// To compare for the same instant on the time-line, use <seealso cref="#compareTo"/>.
		/// Only objects of type {@code ChronoZonedDateTime} are compared, other types return false.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date-time </returns>
		bool Equals(Object obj);

		/// <summary>
		/// A hash code for this date-time.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		int HashCode();

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this date-time as a {@code String}.
		/// <para>
		/// The output will include the full zoned date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date-time, not null </returns>
		String ToString();

	}

	public static class ChronoZonedDateTime_Fields
	{
			public static readonly return AbstractChronology;
			public static readonly Chronology Chrono = temporal.query(TemporalQueries.Chronology());
			public static readonly long EpochDay = toLocalDate().toEpochDay();
			public static readonly long Secs = EpochDay * 86400 + toLocalTime().toSecondOfDay();
			public static readonly return Secs;
			public static readonly int Cmp = Long.Compare(toEpochSecond(), other.toEpochSecond());
			public static readonly return Cmp;
			public static readonly long ThisEpochSec = toEpochSecond();
			public static readonly long OtherEpochSec = other.toEpochSecond();
			public static readonly long ThisEpochSec = toEpochSecond();
			public static readonly long OtherEpochSec = other.toEpochSecond();
	}

}