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
	/// A date-time without a time-zone in an arbitrary chronology, intended
	/// for advanced globalization use cases.
	/// <para>
	/// <b>Most applications should declare method signatures, fields and variables
	/// as <seealso cref="LocalDateTime"/>, not this interface.</b>
	/// </para>
	/// <para>
	/// A {@code ChronoLocalDateTime} is the abstract representation of a local date-time
	/// where the {@code Chronology chronology}, or calendar system, is pluggable.
	/// The date-time is defined in terms of fields expressed by <seealso cref="TemporalField"/>,
	/// where most common implementations are defined in <seealso cref="ChronoField"/>.
	/// The chronology defines how the calendar system operates and the meaning of
	/// the standard fields.
	/// 
	/// <h3>When to use this interface</h3>
	/// The design of the API encourages the use of {@code LocalDateTime} rather than this
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
	public interface ChronoLocalDateTime<D> : Temporal, TemporalAdjuster, Comparable<ChronoLocalDateTime<JavaToDotNetGenericWildcard>> where D : ChronoLocalDate
	{

		/// <summary>
		/// Gets a comparator that compares {@code ChronoLocalDateTime} in
		/// time-line order ignoring the chronology.
		/// <para>
		/// This comparator differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying date-time and not the chronology.
		/// This allows dates in different calendar systems to be compared based
		/// on the position of the date-time on the local time-line.
		/// The underlying comparison is equivalent to comparing the epoch-day and nano-of-day.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a comparator that compares in time-line order ignoring the chronology </returns>
		/// <seealso cref= #isAfter </seealso>
		/// <seealso cref= #isBefore </seealso>
		/// <seealso cref= #isEqual </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static java.util.Comparator<ChronoLocalDateTime<JavaToDotNetGenericWildcard>> timeLineOrder()
	//	{
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ChronoLocalDateTime} from a temporal object.
		/// <para>
		/// This obtains a local date-time based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code ChronoLocalDateTime}.
		/// </para>
		/// <para>
		/// The conversion extracts and combines the chronology and the date-time
		/// from the temporal object. The behavior is equivalent to using
		/// <seealso cref="Chronology#localDateTime(TemporalAccessor)"/> with the extracted chronology.
		/// Implementations are permitted to perform optimizations such as accessing
		/// those fields that are equivalent to the relevant objects.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code ChronoLocalDateTime::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the date-time, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code ChronoLocalDateTime} </exception>
		/// <seealso cref= Chronology#localDateTime(TemporalAccessor) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static ChronoLocalDateTime<JavaToDotNetGenericWildcard> from(java.time.temporal.TemporalAccessor temporal)
	//	{
	//		if (temporal instanceof ChronoLocalDateTime)
	//		{
	//			return (ChronoLocalDateTime<?>) temporal;
	//		}
	//		Objects.requireNonNull(temporal, "temporal");
	//		if (chrono == null)
	//		{
	//			throw new DateTimeException("Unable to obtain ChronoLocalDateTime from TemporalAccessor: " + temporal.getClass());
	//		}
	//		return chrono.localDateTime(temporal);
	//	}

		//-----------------------------------------------------------------------
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
		/// Gets the local date part of this date-time.
		/// <para>
		/// This returns a local date with the same year, month and day
		/// as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the date part of this date-time, not null </returns>
		D ToLocalDate();

		/// <summary>
		/// Gets the local time part of this date-time.
		/// <para>
		/// This returns a local time with the same hour, minute, second and
		/// nanosecond as this date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time part of this date-time, not null </returns>
		LocalTime ToLocalTime();

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
		/// all {@code ChronoField} date and time fields.
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
//		default ChronoLocalDateTime<D> with(java.time.temporal.TemporalAdjuster adjuster)
	//	{
	//		return ChronoLocalDateTimeImpl.ensureValid(getChronology(), Temporal.this.with(adjuster));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		ChronoLocalDateTime<D> With(TemporalField field, long newValue);

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoLocalDateTime<D> plus(java.time.temporal.TemporalAmount amount)
	//	{
	//		return ChronoLocalDateTimeImpl.ensureValid(getChronology(), Temporal.this.plus(amount));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		ChronoLocalDateTime<D> Plus(long amountToAdd, TemporalUnit unit);

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoLocalDateTime<D> minus(java.time.temporal.TemporalAmount amount)
	//	{
	//		return ChronoLocalDateTimeImpl.ensureValid(getChronology(), Temporal.this.minus(amount));
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoLocalDateTime<D> minus(long amountToSubtract, java.time.temporal.TemporalUnit unit)
	//	{
	//		return ChronoLocalDateTimeImpl.ensureValid(getChronology(), Temporal.this.minus(amountToSubtract, unit));
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
	//		if (query == TemporalQueries.zoneId() || query == TemporalQueries.zone() || query == TemporalQueries.offset())
	//		{
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
		/// Adjusts the specified temporal object to have the same date and time as this object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with the date and time changed to be the same as this.
		/// </para>
		/// <para>
		/// The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		/// twice, passing <seealso cref="ChronoField#EPOCH_DAY"/> and
		/// <seealso cref="ChronoField#NANO_OF_DAY"/> as the fields.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   temporal = thisLocalDateTime.adjustInto(temporal);
		///   temporal = temporal.with(thisLocalDateTime);
		/// </pre>
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the target object to be adjusted, not null </param>
		/// <returns> the adjusted object, not null </returns>
		/// <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.time.temporal.Temporal adjustInto(java.time.temporal.Temporal temporal)
	//	{
	//		return temporal.with(EPOCH_DAY, toLocalDate().toEpochDay()).with(NANO_OF_DAY, toLocalTime().toNanoOfDay());
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
		/// Combines this time with a time-zone to create a {@code ChronoZonedDateTime}.
		/// <para>
		/// This returns a {@code ChronoZonedDateTime} formed from this date-time at the
		/// specified time-zone. The result will match this date-time as closely as possible.
		/// Time-zone rules, such as daylight savings, mean that not every local date-time
		/// is valid for the specified zone, thus the local date-time may be adjusted.
		/// </para>
		/// <para>
		/// The local date-time is resolved to a single instant on the time-line.
		/// This is achieved by finding a valid offset from UTC/Greenwich for the local
		/// date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		/// </para>
		/// <para>
		/// In most cases, there is only one valid offset for a local date-time.
		/// In the case of an overlap, where clocks are set back, there are two valid offsets.
		/// This method uses the earlier offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// In the case of a gap, where clocks jump forward, there is no valid offset.
		/// Instead, the local date-time is adjusted to be later by the length of the gap.
		/// For a typical one hour daylight savings change, the local date-time will be
		/// moved one hour later into the offset typically corresponding to "summer".
		/// </para>
		/// <para>
		/// To obtain the later offset during an overlap, call
		/// <seealso cref="ChronoZonedDateTime#withLaterOffsetAtOverlap()"/> on the result of this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use, not null </param>
		/// <returns> the zoned date-time formed from this date-time, not null </returns>
		ChronoZonedDateTime<D> AtZone(ZoneId zone);

		//-----------------------------------------------------------------------
		/// <summary>
		/// Converts this date-time to an {@code Instant}.
		/// <para>
		/// This combines this local date-time and the specified offset to form
		/// an {@code Instant}.
		/// </para>
		/// <para>
		/// This default implementation calculates from the epoch-day of the date and the
		/// second-of-day of the time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to use for the conversion, not null </param>
		/// <returns> an {@code Instant} representing the same instant, not null </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.time.Instant toInstant(java.time.ZoneOffset offset)
	//	{
	//		return Instant.ofEpochSecond(toEpochSecond(offset), toLocalTime().getNano());
	//	}

		/// <summary>
		/// Converts this date-time to the number of seconds from the epoch
		/// of 1970-01-01T00:00:00Z.
		/// <para>
		/// This combines this local date-time and the specified offset to calculate the
		/// epoch-second value, which is the number of elapsed seconds from 1970-01-01T00:00:00Z.
		/// Instants on the time-line after the epoch are positive, earlier are negative.
		/// </para>
		/// <para>
		/// This default implementation calculates from the epoch-day of the date and the
		/// second-of-day of the time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">  the offset to use for the conversion, not null </param>
		/// <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default long toEpochSecond(java.time.ZoneOffset offset)
	//	{
	//		Objects.requireNonNull(offset, "offset");
	//		secs -= offset.getTotalSeconds();
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Compares this date-time to another date-time, including the chronology.
		/// <para>
		/// The comparison is based first on the underlying time-line date-time, then
		/// on the chronology.
		/// It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		/// </para>
		/// <para>
		/// For example, the following is the comparator order:
		/// <ol>
		/// <li>{@code 2012-12-03T12:00 (ISO)}</li>
		/// <li>{@code 2012-12-04T12:00 (ISO)}</li>
		/// <li>{@code 2555-12-04T12:00 (ThaiBuddhist)}</li>
		/// <li>{@code 2012-12-05T12:00 (ISO)}</li>
		/// </ol>
		/// Values #2 and #3 represent the same date-time on the time-line.
		/// When two values represent the same date-time, the chronology ID is compared to distinguish them.
		/// This step is needed to make the ordering "consistent with equals".
		/// </para>
		/// <para>
		/// If all the date-time objects being compared are in the same chronology, then the
		/// additional chronology stage is not required and only the local date-time is used.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison defined above.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> the comparator value, negative if less, positive if greater </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default int compareTo(ChronoLocalDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		if (cmp == 0)
	//		{
	//			cmp = toLocalTime().compareTo(other.toLocalTime());
	//			if (cmp == 0)
	//			{
	//				cmp = getChronology().compareTo(other.getChronology());
	//			}
	//		}
	//	}

		/// <summary>
		/// Checks if this date-time is after the specified date-time ignoring the chronology.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying date-time and not the chronology.
		/// This allows dates in different calendar systems to be compared based
		/// on the time-line position.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-day
		/// and nano-of-day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this is after the specified date-time </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isAfter(ChronoLocalDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		return thisEpDay > otherEpDay || (thisEpDay == otherEpDay && this.toLocalTime().toNanoOfDay() > other.toLocalTime().toNanoOfDay());
	//	}

		/// <summary>
		/// Checks if this date-time is before the specified date-time ignoring the chronology.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying date-time and not the chronology.
		/// This allows dates in different calendar systems to be compared based
		/// on the time-line position.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-day
		/// and nano-of-day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if this is before the specified date-time </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isBefore(ChronoLocalDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		return thisEpDay < otherEpDay || (thisEpDay == otherEpDay && this.toLocalTime().toNanoOfDay() < other.toLocalTime().toNanoOfDay());
	//	}

		/// <summary>
		/// Checks if this date-time is equal to the specified date-time ignoring the chronology.
		/// <para>
		/// This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		/// only compares the underlying date and time and not the chronology.
		/// This allows date-times in different calendar systems to be compared based
		/// on the time-line position.
		/// </para>
		/// <para>
		/// This default implementation performs the comparison based on the epoch-day
		/// and nano-of-day.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other">  the other date-time to compare to, not null </param>
		/// <returns> true if the underlying date-time is equal to the specified date-time on the timeline </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isEqual(ChronoLocalDateTime<JavaToDotNetGenericWildcard> other)
	//	{
	//		// Do the time check first, it is cheaper than computing EPOCH day.
	//		return this.toLocalTime().toNanoOfDay() == other.toLocalTime().toNanoOfDay() && this.toLocalDate().toEpochDay() == other.toLocalDate().toEpochDay();
	//	}

		/// <summary>
		/// Checks if this date-time is equal to another date-time, including the chronology.
		/// <para>
		/// Compares this date-time with another ensuring that the date-time and chronology are the same.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date </returns>
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
		/// The output will include the full local date-time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this date-time, not null </returns>
		String ToString();

	}

	public static class ChronoLocalDateTime_Fields
	{
			public static readonly return AbstractChronology;
			public static readonly Chronology Chrono = temporal.query(TemporalQueries.Chronology());
				public static readonly return Null;
			public static readonly long EpochDay = toLocalDate().toEpochDay();
			public static readonly long Secs = EpochDay * 86400 + toLocalTime().toSecondOfDay();
			public static readonly return Secs;
			public static readonly int Cmp = toLocalDate().CompareTo(other.toLocalDate());
			public static readonly return Cmp;
			public static readonly long ThisEpDay = this.toLocalDate().toEpochDay();
			public static readonly long OtherEpDay = other.toLocalDate().toEpochDay();
			public static readonly long ThisEpDay = this.toLocalDate().toEpochDay();
			public static readonly long OtherEpDay = other.toLocalDate().toEpochDay();
	}

}