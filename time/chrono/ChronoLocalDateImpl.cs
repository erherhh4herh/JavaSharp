using System;

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



	/// <summary>
	/// A date expressed in terms of a standard year-month-day calendar system.
	/// <para>
	/// This class is used by applications seeking to handle dates in non-ISO calendar systems.
	/// For example, the Japanese, Minguo, Thai Buddhist and others.
	/// </para>
	/// <para>
	/// {@code ChronoLocalDate} is built on the generic concepts of year, month and day.
	/// The calendar system, represented by a <seealso cref="java.time.chrono.Chronology"/>, expresses the relationship between
	/// the fields and this class allows the resulting date to be manipulated.
	/// </para>
	/// <para>
	/// Note that not all calendar systems are suitable for use with this class.
	/// For example, the Mayan calendar uses a system that bears no relation to years, months and days.
	/// </para>
	/// <para>
	/// The API design encourages the use of {@code LocalDate} for the majority of the application.
	/// This includes code to read and write from a persistent data store, such as a database,
	/// and to send dates and times across a network. The {@code ChronoLocalDate} instance is then used
	/// at the user interface level to deal with localized input/output.
	/// 
	/// <P>Example: </para>
	/// <pre>
	///        System.out.printf("Example()%n");
	///        // Enumerate the list of available calendars and print today for each
	///        Set&lt;Chronology&gt; chronos = Chronology.getAvailableChronologies();
	///        for (Chronology chrono : chronos) {
	///            ChronoLocalDate date = chrono.dateNow();
	///            System.out.printf("   %20s: %s%n", chrono.getID(), date.toString());
	///        }
	/// 
	///        // Print the Hijrah date and calendar
	///        ChronoLocalDate date = Chronology.of("Hijrah").dateNow();
	///        int day = date.get(ChronoField.DAY_OF_MONTH);
	///        int dow = date.get(ChronoField.DAY_OF_WEEK);
	///        int month = date.get(ChronoField.MONTH_OF_YEAR);
	///        int year = date.get(ChronoField.YEAR);
	///        System.out.printf("  Today is %s %s %d-%s-%d%n", date.getChronology().getID(),
	///                dow, day, month, year);
	/// 
	///        // Print today's date and the last day of the year
	///        ChronoLocalDate now1 = Chronology.of("Hijrah").dateNow();
	///        ChronoLocalDate first = now1.with(ChronoField.DAY_OF_MONTH, 1)
	///                .with(ChronoField.MONTH_OF_YEAR, 1);
	///        ChronoLocalDate last = first.plus(1, ChronoUnit.YEARS)
	///                .minus(1, ChronoUnit.DAYS);
	///        System.out.printf("  Today is %s: start: %s; end: %s%n", last.getChronology().getID(),
	///                first, last);
	/// </pre>
	/// 
	/// <h3>Adding Calendars</h3>
	/// <para> The set of calendars is extensible by defining a subclass of <seealso cref="ChronoLocalDate"/>
	/// to represent a date instance and an implementation of {@code Chronology}
	/// to be the factory for the ChronoLocalDate subclass.
	/// </para>
	/// <para> To permit the discovery of the additional calendar types the implementation of
	/// {@code Chronology} must be registered as a Service implementing the {@code Chronology} interface
	/// in the {@code META-INF/Services} file as per the specification of <seealso cref="java.util.ServiceLoader"/>.
	/// The subclass must function according to the {@code Chronology} class description and must provide its
	/// <seealso cref="java.time.chrono.Chronology#getId() chronlogy ID"/> and <seealso cref="Chronology#getCalendarType() calendar type"/>. </para>
	/// 
	/// @implSpec
	/// This abstract class must be implemented with care to ensure other classes operate correctly.
	/// All implementations that can be instantiated must be final, immutable and thread-safe.
	/// Subclasses should be Serializable wherever possible.
	/// </summary>
	/// @param <D> the ChronoLocalDate of this date-time
	/// @since 1.8 </param>
	[Serializable]
	internal abstract class ChronoLocalDateImpl<D> : ChronoLocalDate, Temporal, TemporalAdjuster where D : ChronoLocalDate
	{
		public abstract int CompareTo(T o);
		public abstract Temporal AdjustInto(Temporal temporal);
		public abstract long GetLong(TemporalField field);
		public abstract bool IsSupported(TemporalUnit unit);
		public abstract ChronoPeriod Until(ChronoLocalDate endDateExclusive);
		public abstract int LengthOfMonth();
		public abstract Chronology Chronology {get;}

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 6282433883239719096L;

		/// <summary>
		/// Casts the {@code Temporal} to {@code ChronoLocalDate} ensuring it bas the specified chronology.
		/// </summary>
		/// <param name="chrono">  the chronology to check for, not null </param>
		/// <param name="temporal">  a date-time to cast, not null </param>
		/// <returns> the date-time checked and cast to {@code ChronoLocalDate}, not null </returns>
		/// <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoLocalDate
		///  or the chronology is not equal this Chronology </exception>
		internal static D ensureValid<D>(Chronology ChronoLocalDate_Fields, Temporal temporal) where D : ChronoLocalDate
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") D other = (D) temporal;
			D other = (D) temporal;
			if (ChronoLocalDate_Fields.Chrono.Equals(other.Chronology) == false)
			{
				throw new ClassCastException("Chronology mismatch, expected: " + ChronoLocalDate_Fields.Chrono.Id + ", actual: " + other.Chronology.Id);
			}
			return other;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance.
		/// </summary>
		internal ChronoLocalDateImpl()
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D with(java.time.temporal.TemporalAdjuster adjuster)
		public override D With(TemporalAdjuster adjuster)
		{
			return (D) ChronoLocalDate.this.with(adjuster);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D with(java.time.temporal.TemporalField field, long java.time.temporal.TemporalAccessor_Fields.value)
		public virtual D With(TemporalField field, long java)
		{
			return (D) ChronoLocalDate.this.with(field, java.time.temporal.TemporalAccessor_Fields.Value);
		}

		//-----------------------------------------------------------------------
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D plus(java.time.temporal.TemporalAmount amount)
		public override D Plus(TemporalAmount amount)
		{
			return (D) ChronoLocalDate.this.plus(amount);
		}

		//-----------------------------------------------------------------------
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D plus(long amountToAdd, java.time.temporal.TemporalUnit unit)
		public virtual D Plus(long amountToAdd, TemporalUnit unit)
		{
			if (unit is ChronoUnit)
			{
				ChronoUnit f = (ChronoUnit) unit;
				switch (f)
				{
					case DAYS:
						return PlusDays(amountToAdd);
					case WEEKS:
						return PlusDays(Math.MultiplyExact(amountToAdd, 7));
					case MONTHS:
						return PlusMonths(amountToAdd);
					case YEARS:
						return PlusYears(amountToAdd);
					case DECADES:
						return PlusYears(Math.MultiplyExact(amountToAdd, 10));
					case CENTURIES:
						return PlusYears(Math.MultiplyExact(amountToAdd, 100));
					case MILLENNIA:
						return PlusYears(Math.MultiplyExact(amountToAdd, 1000));
					case ERAS:
						return With(ERA, Math.AddExact(GetLong(ERA), amountToAdd));
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			return (D) ChronoLocalDate.this.plus(amountToAdd, unit);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D minus(java.time.temporal.TemporalAmount amount)
		public override D Minus(TemporalAmount amount)
		{
			return (D) ChronoLocalDate.this.minus(amount);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public D minus(long amountToSubtract, java.time.temporal.TemporalUnit unit)
		public override D Minus(long amountToSubtract, TemporalUnit unit)
		{
			return (D) ChronoLocalDate.this.minus(amountToSubtract, unit);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date with the specified number of years added.
		/// <para>
		/// This adds the specified period in years to the date.
		/// In some cases, adding years can cause the resulting date to become invalid.
		/// If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		/// that the result is valid. Typically this will select the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToAdd">  the years to add, may be negative </param>
		/// <returns> a date based on this one with the years added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		internal abstract D PlusYears(long yearsToAdd);

		/// <summary>
		/// Returns a copy of this date with the specified number of months added.
		/// <para>
		/// This adds the specified period in months to the date.
		/// In some cases, adding months can cause the resulting date to become invalid.
		/// If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		/// that the result is valid. Typically this will select the last valid day of the month.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToAdd">  the months to add, may be negative </param>
		/// <returns> a date based on this one with the months added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		internal abstract D PlusMonths(long monthsToAdd);

		/// <summary>
		/// Returns a copy of this date with the specified number of weeks added.
		/// <para>
		/// This adds the specified period in weeks to the date.
		/// In some cases, adding weeks can cause the resulting date to become invalid.
		/// If this occurs, then other fields will be adjusted to ensure that the result is valid.
		/// </para>
		/// <para>
		/// The default implementation uses <seealso cref="#plusDays(long)"/> using a 7 day week.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeksToAdd">  the weeks to add, may be negative </param>
		/// <returns> a date based on this one with the weeks added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		internal virtual D PlusWeeks(long weeksToAdd)
		{
			return PlusDays(Math.MultiplyExact(weeksToAdd, 7));
		}

		/// <summary>
		/// Returns a copy of this date with the specified number of days added.
		/// <para>
		/// This adds the specified period in days to the date.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToAdd">  the days to add, may be negative </param>
		/// <returns> a date based on this one with the days added, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		internal abstract D PlusDays(long daysToAdd);

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this date with the specified number of years subtracted.
		/// <para>
		/// This subtracts the specified period in years to the date.
		/// In some cases, subtracting years can cause the resulting date to become invalid.
		/// If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		/// that the result is valid. Typically this will select the last valid day of the month.
		/// </para>
		/// <para>
		/// The default implementation uses <seealso cref="#plusYears(long)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		/// <returns> a date based on this one with the years subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") D minusYears(long yearsToSubtract)
		internal virtual D MinusYears(long yearsToSubtract)
		{
			return (yearsToSubtract == Long.MinValue ? ((ChronoLocalDateImpl<D>)PlusYears(Long.MaxValue)).PlusYears(1) : PlusYears(-yearsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this date with the specified number of months subtracted.
		/// <para>
		/// This subtracts the specified period in months to the date.
		/// In some cases, subtracting months can cause the resulting date to become invalid.
		/// If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		/// that the result is valid. Typically this will select the last valid day of the month.
		/// </para>
		/// <para>
		/// The default implementation uses <seealso cref="#plusMonths(long)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		/// <returns> a date based on this one with the months subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") D minusMonths(long monthsToSubtract)
		internal virtual D MinusMonths(long monthsToSubtract)
		{
			return (monthsToSubtract == Long.MinValue ? ((ChronoLocalDateImpl<D>)PlusMonths(Long.MaxValue)).PlusMonths(1) : PlusMonths(-monthsToSubtract));
		}

		/// <summary>
		/// Returns a copy of this date with the specified number of weeks subtracted.
		/// <para>
		/// This subtracts the specified period in weeks to the date.
		/// In some cases, subtracting weeks can cause the resulting date to become invalid.
		/// If this occurs, then other fields will be adjusted to ensure that the result is valid.
		/// </para>
		/// <para>
		/// The default implementation uses <seealso cref="#plusWeeks(long)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="weeksToSubtract">  the weeks to subtract, may be negative </param>
		/// <returns> a date based on this one with the weeks subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") D minusWeeks(long weeksToSubtract)
		internal virtual D MinusWeeks(long weeksToSubtract)
		{
			return (weeksToSubtract == Long.MinValue ? ((ChronoLocalDateImpl<D>)PlusWeeks(Long.MaxValue)).PlusWeeks(1) : PlusWeeks(-weeksToSubtract));
		}

		/// <summary>
		/// Returns a copy of this date with the specified number of days subtracted.
		/// <para>
		/// This subtracts the specified period in days to the date.
		/// </para>
		/// <para>
		/// The default implementation uses <seealso cref="#plusDays(long)"/>.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="daysToSubtract">  the days to subtract, may be negative </param>
		/// <returns> a date based on this one with the days subtracted, not null </returns>
		/// <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") D minusDays(long daysToSubtract)
		internal virtual D MinusDays(long daysToSubtract)
		{
			return (daysToSubtract == Long.MinValue ? ((ChronoLocalDateImpl<D>)PlusDays(Long.MaxValue)).PlusDays(1) : PlusDays(-daysToSubtract));
		}

		//-----------------------------------------------------------------------
		public virtual long Until(Temporal endExclusive, TemporalUnit unit)
		{
			Objects.RequireNonNull(endExclusive, "endExclusive");
			ChronoLocalDate end = Chronology.Date(endExclusive);
			if (unit is ChronoUnit)
			{
				switch ((ChronoUnit) unit)
				{
					case DAYS:
						return DaysUntil(end);
					case WEEKS:
						return DaysUntil(end) / 7;
					case MONTHS:
						return MonthsUntil(end);
					case YEARS:
						return MonthsUntil(end) / 12;
					case DECADES:
						return MonthsUntil(end) / 120;
					case CENTURIES:
						return MonthsUntil(end) / 1200;
					case MILLENNIA:
						return MonthsUntil(end) / 12000;
					case ERAS:
						return end.GetLong(ERA) - GetLong(ERA);
				}
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
			Objects.RequireNonNull(unit, "unit");
			return unit.Between(this, end);
		}

		private long DaysUntil(ChronoLocalDate end)
		{
			return end.toEpochDay() - toEpochDay(); // no overflow
		}

		private long MonthsUntil(ChronoLocalDate end)
		{
			ValueRange java.time.temporal.TemporalAccessor_Fields.Range = Chronology.Range(MONTH_OF_YEAR);
			if (java.time.temporal.TemporalAccessor_Fields.Range.Maximum != 12)
			{
				throw new IllegalStateException("ChronoLocalDateImpl only supports Chronologies with 12 months per year");
			}
			long packed1 = GetLong(PROLEPTIC_MONTH) * 32L + get(DAY_OF_MONTH); // no overflow
			long packed2 = end.GetLong(PROLEPTIC_MONTH) * 32L + end.get(DAY_OF_MONTH); // no overflow
			return (packed2 - packed1) / 32;
		}

		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is ChronoLocalDate)
			{
				return CompareTo((ChronoLocalDate) obj) == 0;
			}
			return false;
		}

		public override int HashCode()
		{
			long epDay = toEpochDay();
			return Chronology.HashCode() ^ ((int)(epDay ^ ((long)((ulong)epDay >> 32))));
		}

		public override String ToString()
		{
			// getLong() reduces chances of exceptions in toString()
			long yoe = GetLong(YEAR_OF_ERA);
			long moy = GetLong(MONTH_OF_YEAR);
			long dom = GetLong(DAY_OF_MONTH);
			StringBuilder buf = new StringBuilder(30);
			buf.Append(Chronology.ToString()).Append(" ").Append(Era).Append(" ").Append(yoe).Append(moy < 10 ? "-0" : "-").Append(moy).Append(dom < 10 ? "-0" : "-").Append(dom);
			return buf.ToString();
		}

	}

}