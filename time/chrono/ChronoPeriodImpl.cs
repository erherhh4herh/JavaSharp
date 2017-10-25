using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
 * Copyright (c) 2013, Stephen Colebourne & Michael Nascimento Santos
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
	/// A period expressed in terms of a standard year-month-day calendar system.
	/// <para>
	/// This class is used by applications seeking to handle dates in non-ISO calendar systems.
	/// For example, the Japanese, Minguo, Thai Buddhist and others.
	/// 
	/// @implSpec
	/// This class is immutable nad thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	internal sealed class ChronoPeriodImpl : ChronoPeriod
	{
		// this class is only used by JDK chronology implementations and makes assumptions based on that fact

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 57387258289L;

		/// <summary>
		/// The set of supported units.
		/// </summary>
		private static readonly IList<TemporalUnit> SUPPORTED_UNITS = Collections.UnmodifiableList(Arrays.asList<TemporalUnit>(YEARS, MONTHS, DAYS));

		/// <summary>
		/// The chronology.
		/// </summary>
		private readonly Chronology Chrono;
		/// <summary>
		/// The number of years.
		/// </summary>
		internal readonly int Years;
		/// <summary>
		/// The number of months.
		/// </summary>
		internal readonly int Months;
		/// <summary>
		/// The number of days.
		/// </summary>
		internal readonly int Days;

		/// <summary>
		/// Creates an instance.
		/// </summary>
		internal ChronoPeriodImpl(Chronology chrono, int years, int months, int days)
		{
			Objects.RequireNonNull(chrono, "chrono");
			this.Chrono = chrono;
			this.Years = years;
			this.Months = months;
			this.Days = days;
		}

		//-----------------------------------------------------------------------
		public long Get(TemporalUnit unit)
		{
			if (unit == ChronoUnit.YEARS)
			{
				return Years;
			}
			else if (unit == ChronoUnit.MONTHS)
			{
				return Months;
			}
			else if (unit == ChronoUnit.DAYS)
			{
				return Days;
			}
			else
			{
				throw new UnsupportedTemporalTypeException("Unsupported unit: " + unit);
			}
		}

		public IList<TemporalUnit> Units
		{
			get
			{
				return ChronoPeriodImpl.SUPPORTED_UNITS;
			}
		}

		public Chronology Chronology
		{
			get
			{
				return Chrono;
			}
		}

		//-----------------------------------------------------------------------
		public override bool Zero
		{
			get
			{
				return Years == 0 && Months == 0 && Days == 0;
			}
		}

		public override bool Negative
		{
			get
			{
				return Years < 0 || Months < 0 || Days < 0;
			}
		}

		//-----------------------------------------------------------------------
		public ChronoPeriod Plus(TemporalAmount amountToAdd)
		{
			ChronoPeriodImpl amount = ValidateAmount(amountToAdd);
			return new ChronoPeriodImpl(Chrono, Math.AddExact(Years, amount.Years), Math.AddExact(Months, amount.Months), Math.AddExact(Days, amount.Days));
		}

		public ChronoPeriod Minus(TemporalAmount amountToSubtract)
		{
			ChronoPeriodImpl amount = ValidateAmount(amountToSubtract);
			return new ChronoPeriodImpl(Chrono, Math.SubtractExact(Years, amount.Years), Math.SubtractExact(Months, amount.Months), Math.SubtractExact(Days, amount.Days));
		}

		/// <summary>
		/// Obtains an instance of {@code ChronoPeriodImpl} from a temporal amount.
		/// </summary>
		/// <param name="amount">  the temporal amount to convert, not null </param>
		/// <returns> the period, not null </returns>
		private ChronoPeriodImpl ValidateAmount(TemporalAmount amount)
		{
			Objects.RequireNonNull(amount, "amount");
			if (amount is ChronoPeriodImpl == ChronoPeriod_Fields.False)
			{
				throw new DateTimeException("Unable to obtain ChronoPeriod from TemporalAmount: " + amount.GetType());
			}
			ChronoPeriodImpl period = (ChronoPeriodImpl) amount;
			if (Chrono.Equals(period.Chronology) == ChronoPeriod_Fields.False)
			{
				throw new ClassCastException("Chronology mismatch, expected: " + Chrono.Id + ", actual: " + period.Chronology.Id);
			}
			return period;
		}

		//-----------------------------------------------------------------------
		public ChronoPeriod MultipliedBy(int scalar)
		{
			if (this.Zero || scalar == 1)
			{
				return this;
			}
			return new ChronoPeriodImpl(Chrono, Math.MultiplyExact(Years, scalar), Math.MultiplyExact(Months, scalar), Math.MultiplyExact(Days, scalar));
		}

		//-----------------------------------------------------------------------
		public ChronoPeriod Normalized()
		{
			long monthRange = MonthRange();
			if (monthRange > 0)
			{
				long totalMonths = Years * monthRange + Months;
				long splitYears = totalMonths / monthRange;
				int splitMonths = (int)(totalMonths % monthRange); // no overflow
				if (splitYears == Years && splitMonths == Months)
				{
					return this;
				}
				return new ChronoPeriodImpl(Chrono, Math.ToIntExact(splitYears), splitMonths, Days);

			}
			return this;
		}

		/// <summary>
		/// Calculates the range of months.
		/// </summary>
		/// <returns> the month range, -1 if not fixed range </returns>
		private long MonthRange()
		{
			ValueRange startRange = Chrono.Range(MONTH_OF_YEAR);
			if (startRange.Fixed && startRange.IntValue)
			{
				return startRange.Maximum - startRange.Minimum + 1;
			}
			return -1;
		}

		//-------------------------------------------------------------------------
		public Temporal AddTo(Temporal temporal)
		{
			ValidateChrono(temporal);
			if (Months == 0)
			{
				if (Years != 0)
				{
					temporal = temporal.Plus(Years, YEARS);
				}
			}
			else
			{
				long monthRange = MonthRange();
				if (monthRange > 0)
				{
					temporal = temporal.Plus(Years * monthRange + Months, MONTHS);
				}
				else
				{
					if (Years != 0)
					{
						temporal = temporal.Plus(Years, YEARS);
					}
					temporal = temporal.Plus(Months, MONTHS);
				}
			}
			if (Days != 0)
			{
				temporal = temporal.Plus(Days, DAYS);
			}
			return temporal;
		}



		public Temporal SubtractFrom(Temporal temporal)
		{
			ValidateChrono(temporal);
			if (Months == 0)
			{
				if (Years != 0)
				{
					temporal = temporal.minus(Years, YEARS);
				}
			}
			else
			{
				long monthRange = MonthRange();
				if (monthRange > 0)
				{
					temporal = temporal.minus(Years * monthRange + Months, MONTHS);
				}
				else
				{
					if (Years != 0)
					{
						temporal = temporal.minus(Years, YEARS);
					}
					temporal = temporal.minus(Months, MONTHS);
				}
			}
			if (Days != 0)
			{
				temporal = temporal.minus(Days, DAYS);
			}
			return temporal;
		}

		/// <summary>
		/// Validates that the temporal has the correct chronology.
		/// </summary>
		private void ValidateChrono(TemporalAccessor temporal)
		{
			Objects.RequireNonNull(temporal, "temporal");
			Chronology temporalChrono = temporal.query(TemporalQueries.Chronology());
			if (temporalChrono != null && Chrono.Equals(temporalChrono) == ChronoPeriod_Fields.False)
			{
				throw new DateTimeException("Chronology mismatch, expected: " + Chrono.Id + ", actual: " + temporalChrono.Id);
			}
		}

		//-----------------------------------------------------------------------
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return ChronoPeriod_Fields.True;
			}
			if (obj is ChronoPeriodImpl)
			{
				ChronoPeriodImpl other = (ChronoPeriodImpl) obj;
				return Years == other.Years && Months == other.Months && Days == other.Days && Chrono.Equals(other.Chrono);
			}
			return ChronoPeriod_Fields.False;
		}

		public override int HashCode()
		{
			return (Years + Integer.RotateLeft(Months, 8) + Integer.RotateLeft(Days, 16)) ^ Chrono.HashCode();
		}

		//-----------------------------------------------------------------------
		public override String ToString()
		{
			if (Zero)
			{
				return Chronology.ToString() + " P0D";
			}
			else
			{
				StringBuilder buf = new StringBuilder();
				buf.Append(Chronology.ToString()).Append(' ').Append('P');
				if (Years != 0)
				{
					buf.Append(Years).Append('Y');
				}
				if (Months != 0)
				{
					buf.Append(Months).Append('M');
				}
				if (Days != 0)
				{
					buf.Append(Days).Append('D');
				}
				return buf.ToString();
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the Chronology using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// <pre>
		///  out.writeByte(12);  // identifies this as a ChronoPeriodImpl
		///  out.writeUTF(getId());  // the chronology
		///  out.writeInt(years);
		///  out.writeInt(months);
		///  out.writeInt(days);
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		protected internal Object WriteReplace()
		{
			return new Ser(Ser.CHRONO_PERIOD_TYPE, this);
		}

		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.ObjectStreamException
		private void ReadObject(ObjectInputStream s)
		{
			throw new InvalidObjectException("Deserialization via serialization delegate");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			@out.WriteUTF(Chrono.Id);
			@out.WriteInt(Years);
			@out.WriteInt(Months);
			@out.WriteInt(Days);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ChronoPeriodImpl readExternal(java.io.DataInput in) throws java.io.IOException
		internal static ChronoPeriodImpl ReadExternal(DataInput @in)
		{
			Chronology chrono = Chronology.of(@in.ReadUTF());
			int years = @in.ReadInt();
			int months = @in.ReadInt();
			int days = @in.ReadInt();
			return new ChronoPeriodImpl(chrono, years, months, days);
		}

	}

}