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
	/// A date in the Minguo calendar system.
	/// <para>
	/// This date operates using the <seealso cref="MinguoChronology Minguo calendar"/>.
	/// This calendar system is primarily used in the Republic of China, often known as Taiwan.
	/// Dates are aligned such that {@code 0001-01-01 (Minguo)} is {@code 1912-01-01 (ISO)}.
	/// 
	/// </para>
	/// <para>
	/// This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	/// class; use of identity-sensitive operations (including reference equality
	/// ({@code ==}), identity hash code, or synchronization) on instances of
	/// {@code MinguoDate} may have unpredictable results and should be avoided.
	/// The {@code equals} method should be used for comparisons.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class MinguoDate : ChronoLocalDateImpl<MinguoDate>, ChronoLocalDate
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 1300372329181994526L;

		/// <summary>
		/// The underlying date.
		/// </summary>
		[NonSerialized]
		private readonly LocalDate IsoDate;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains the current {@code MinguoDate} from the system clock in the default time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		/// time-zone to obtain the current date.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current date using the system clock and default time-zone, not null </returns>
		public static MinguoDate Now()
		{
			return Now(Clock.SystemDefaultZone());
		}

		/// <summary>
		/// Obtains the current {@code MinguoDate} from the system clock in the specified time-zone.
		/// <para>
		/// This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		/// Specifying the time-zone avoids dependence on the default time-zone.
		/// </para>
		/// <para>
		/// Using this method will prevent the ability to use an alternate clock for testing
		/// because the clock is hard-coded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the zone ID to use, not null </param>
		/// <returns> the current date using the system clock, not null </returns>
		public static MinguoDate Now(ZoneId zone)
		{
			return Now(Clock.System(zone));
		}

		/// <summary>
		/// Obtains the current {@code MinguoDate} from the specified clock.
		/// <para>
		/// This will query the specified clock to obtain the current date - today.
		/// Using this method allows the use of an alternate clock for testing.
		/// The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="clock">  the clock to use, not null </param>
		/// <returns> the current date, not null </returns>
		/// <exception cref="DateTimeException"> if the current date cannot be obtained </exception>
		public static MinguoDate Now(Clock clock)
		{
			return new MinguoDate(LocalDate.Now(clock));
		}

		/// <summary>
		/// Obtains a {@code MinguoDate} representing a date in the Minguo calendar
		/// system from the proleptic-year, month-of-year and day-of-month fields.
		/// <para>
		/// This returns a {@code MinguoDate} with the specified fields.
		/// The day must be valid for the year and month, otherwise an exception will be thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="prolepticYear">  the Minguo proleptic-year </param>
		/// <param name="month">  the Minguo month-of-year, from 1 to 12 </param>
		/// <param name="dayOfMonth">  the Minguo day-of-month, from 1 to 31 </param>
		/// <returns> the date in Minguo calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if the value of any field is out of range,
		///  or if the day-of-month is invalid for the month-year </exception>
		public static MinguoDate Of(int prolepticYear, int month, int dayOfMonth)
		{
			return new MinguoDate(LocalDate.Of(prolepticYear + YEARS_DIFFERENCE, month, dayOfMonth));
		}

		/// <summary>
		/// Obtains a {@code MinguoDate} from a temporal object.
		/// <para>
		/// This obtains a date in the Minguo calendar system based on the specified temporal.
		/// A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		/// which this factory converts to an instance of {@code MinguoDate}.
		/// </para>
		/// <para>
		/// The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		/// field, which is standardized across calendar systems.
		/// </para>
		/// <para>
		/// This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		/// allowing it to be used as a query via method reference, {@code MinguoDate::from}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to convert, not null </param>
		/// <returns> the date in Minguo calendar system, not null </returns>
		/// <exception cref="DateTimeException"> if unable to convert to a {@code MinguoDate} </exception>
		public static MinguoDate From(TemporalAccessor temporal)
		{
			return MinguoChronology.INSTANCE.Date(temporal);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Creates an instance from an ISO date.
		/// </summary>
		/// <param name="isoDate">  the standard local date, validated not null </param>
		internal MinguoDate(LocalDate isoDate)
		{
			Objects.RequireNonNull(isoDate, "isoDate");
			this.IsoDate = isoDate;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the chronology of this date, which is the Minguo calendar system.
		/// <para>
		/// The {@code Chronology} represents the calendar system in use.
		/// The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the Minguo chronology, not null </returns>
		public MinguoChronology Chronology
		{
			get
			{
				return MinguoChronology.INSTANCE;
			}
		}

		/// <summary>
		/// Gets the era applicable at this date.
		/// <para>
		/// The Minguo calendar system has two eras, 'ROC' and 'BEFORE_ROC',
		/// defined by <seealso cref="MinguoEra"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era applicable at this date, not null </returns>
		public override MinguoEra Era
		{
			get
			{
				return (ProlepticYear >= 1 ? MinguoEra.ROC : MinguoEra.BEFORE_ROC);
			}
		}

		/// <summary>
		/// Returns the length of the month represented by this date.
		/// <para>
		/// This returns the length of the month in days.
		/// Month lengths match those of the ISO calendar system.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the length of the month in days </returns>
		public int LengthOfMonth()
		{
			return IsoDate.LengthOfMonth();
		}

		//-----------------------------------------------------------------------
		public override ValueRange java.time.temporal.TemporalAccessor_Fields.range(TemporalField field)
		{
			if (field is ChronoField)
			{
				if (IsSupported(field))
				{
					ChronoField f = (ChronoField) field;
					switch (f)
					{
						case DAY_OF_MONTH:
						case DAY_OF_YEAR:
						case ALIGNED_WEEK_OF_MONTH:
							return IsoDate.Range(field);
						case YEAR_OF_ERA:
						{
							ValueRange java.time.temporal.TemporalAccessor_Fields.Range = YEAR.range();
							long max = (ProlepticYear <= 0 ? - java.time.temporal.TemporalAccessor_Fields.Range.Minimum + 1 + YEARS_DIFFERENCE : java.time.temporal.TemporalAccessor_Fields.Range.Maximum - YEARS_DIFFERENCE);
							return ValueRange.Of(1, max);
						}
					}
					return Chronology.Range(f);
				}
				throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
			}
			return field.RangeRefinedBy(this);
		}

		public long GetLong(TemporalField field)
		{
			if (field is ChronoField)
			{
				switch ((ChronoField) field)
				{
					case PROLEPTIC_MONTH:
						return ProlepticMonth;
					case YEAR_OF_ERA:
					{
						int prolepticYear = ProlepticYear;
						return (prolepticYear >= 1 ? prolepticYear : 1 - prolepticYear);
					}
					case YEAR:
						return ProlepticYear;
					case ERA:
						return (ProlepticYear >= 1 ? 1 : 0);
				}
				return IsoDate.GetLong(field);
			}
			return field.GetFrom(this);
		}

		private long ProlepticMonth
		{
			get
			{
				return ProlepticYear * 12L + IsoDate.MonthValue - 1;
			}
		}

		private int ProlepticYear
		{
			get
			{
				return IsoDate.Year - YEARS_DIFFERENCE;
			}
		}

		//-----------------------------------------------------------------------
		public MinguoDate With(TemporalField field, long newValue)
		{
			if (field is ChronoField)
			{
				ChronoField f = (ChronoField) field;
				if (GetLong(f) == newValue)
				{
					return this;
				}
				switch (f)
				{
					case PROLEPTIC_MONTH:
						Chronology.Range(f).CheckValidValue(newValue, f);
						return PlusMonths(newValue - ProlepticMonth);
					case YEAR_OF_ERA:
					case YEAR:
					case ERA:
					{
						int nvalue = Chronology.Range(f).CheckValidIntValue(newValue, f);
						switch (f)
						{
							case YEAR_OF_ERA:
								return With(IsoDate.WithYear(ProlepticYear >= 1 ? nvalue + YEARS_DIFFERENCE : (1 - nvalue) + YEARS_DIFFERENCE));
							case YEAR:
								return With(IsoDate.WithYear(nvalue + YEARS_DIFFERENCE));
							case ERA:
								return With(IsoDate.WithYear((1 - ProlepticYear) + YEARS_DIFFERENCE));
						}
					}
				break;
				}
				return With(IsoDate.With(field, newValue));
			}
			return base.With(field, newValue);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override MinguoDate With(TemporalAdjuster adjuster)
		{
			return base.With(adjuster);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override MinguoDate Plus(TemporalAmount amount)
		{
			return base.Plus(amount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DateTimeException"> {@inheritDoc} </exception>
		/// <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		public override MinguoDate Minus(TemporalAmount amount)
		{
			return base.Minus(amount);
		}

		//-----------------------------------------------------------------------
		internal override MinguoDate PlusYears(long years)
		{
			return With(IsoDate.PlusYears(years));
		}

		internal override MinguoDate PlusMonths(long months)
		{
			return With(IsoDate.PlusMonths(months));
		}

		internal override MinguoDate PlusWeeks(long weeksToAdd)
		{
			return base.PlusWeeks(weeksToAdd);
		}

		internal override MinguoDate PlusDays(long days)
		{
			return With(IsoDate.PlusDays(days));
		}

		public MinguoDate Plus(long amountToAdd, TemporalUnit unit)
		{
			return base.Plus(amountToAdd, unit);
		}

		public override MinguoDate Minus(long amountToAdd, TemporalUnit unit)
		{
			return base.Minus(amountToAdd, unit);
		}

		internal override MinguoDate MinusYears(long yearsToSubtract)
		{
			return base.MinusYears(yearsToSubtract);
		}

		internal override MinguoDate MinusMonths(long monthsToSubtract)
		{
			return base.MinusMonths(monthsToSubtract);
		}

		internal override MinguoDate MinusWeeks(long weeksToSubtract)
		{
			return base.MinusWeeks(weeksToSubtract);
		}

		internal override MinguoDate MinusDays(long daysToSubtract)
		{
			return base.MinusDays(daysToSubtract);
		}

		private MinguoDate With(LocalDate newDate)
		{
			return (newDate.Equals(IsoDate) ? this : new MinguoDate(newDate));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final ChronoLocalDateTime<MinguoDate> atTime(java.time.LocalTime localTime)
		public override ChronoLocalDateTime<MinguoDate> AtTime(LocalTime localTime) // for javadoc and covariant return type
		{
			return (ChronoLocalDateTime<MinguoDate>)base.AtTime(localTime);
		}

		public ChronoPeriod Until(ChronoLocalDate endDate)
		{
			Period period = IsoDate.Until(endDate);
			return Chronology.period(period.Years, period.Months, period.Days);
		}

		public override long ToEpochDay() // override for performance
		{
			return IsoDate.ToEpochDay();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Compares this date to another date, including the chronology.
		/// <para>
		/// Compares this {@code MinguoDate} with another ensuring that the date is the same.
		/// </para>
		/// <para>
		/// Only objects of type {@code MinguoDate} are compared, other types return false.
		/// To compare the dates of two {@code TemporalAccessor} instances, including dates
		/// in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date </returns>
		public override bool Equals(Object obj) // override for performance
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is MinguoDate)
			{
				MinguoDate otherDate = (MinguoDate) obj;
				return this.IsoDate.Equals(otherDate.IsoDate);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this date.
		/// </summary>
		/// <returns> a suitable hash code based only on the Chronology and the date </returns>
		public override int HashCode() // override for performance
		{
			return Chronology.Id.HashCode() ^ IsoDate.HashCode();
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
		/// Writes the object using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(8);                 // identifies a MinguoDate
		///  out.writeInt(get(YEAR));
		///  out.writeByte(get(MONTH_OF_YEAR));
		///  out.writeByte(get(DAY_OF_MONTH));
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.MINGUO_DATE_TYPE, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			// MinguoChronology is implicit in the MINGUO_DATE_TYPE
			@out.WriteInt(get(YEAR));
			@out.WriteByte(get(MONTH_OF_YEAR));
			@out.WriteByte(get(DAY_OF_MONTH));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static MinguoDate readExternal(java.io.DataInput in) throws java.io.IOException
		internal static MinguoDate ReadExternal(DataInput @in)
		{
			int year = @in.ReadInt();
			int month = @in.ReadByte();
			int dayOfMonth = @in.ReadByte();
			return MinguoChronology.INSTANCE.Date(year, month, dayOfMonth);
		}

	}

}