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
 *
 *
 *
 *
 *
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
	/// A date-based amount of time, such as '3 years, 4 months and 5 days' in an
	/// arbitrary chronology, intended for advanced globalization use cases.
	/// <para>
	/// This interface models a date-based amount of time in a calendar system.
	/// While most calendar systems use years, months and days, some do not.
	/// Therefore, this interface operates solely in terms of a set of supported
	/// units that are defined by the {@code Chronology}.
	/// The set of supported units is fixed for a given chronology.
	/// The amount of a supported unit may be set to zero.
	/// </para>
	/// <para>
	/// The period is modeled as a directed amount of time, meaning that individual
	/// parts of the period may be negative.
	/// 
	/// @implSpec
	/// This interface must be implemented with care to ensure other classes operate correctly.
	/// All implementations that can be instantiated must be final, immutable and thread-safe.
	/// Subclasses should be Serializable wherever possible.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public interface ChronoPeriod : TemporalAmount
	{

		/// <summary>
		/// Obtains a {@code ChronoPeriod} consisting of amount of time between two dates.
		/// <para>
		/// The start date is included, but the end date is not.
		/// The period is calculated using <seealso cref="ChronoLocalDate#until(ChronoLocalDate)"/>.
		/// As such, the calculation is chronology specific.
		/// </para>
		/// <para>
		/// The chronology of the first date is used.
		/// The chronology of the second date is ignored, with the date being converted
		/// to the target chronology system before the calculation starts.
		/// </para>
		/// <para>
		/// The result of this method can be a negative period if the end is before the start.
		/// In most cases, the positive/negative sign will be the same in each of the supported fields.
		/// 
		/// </para>
		/// </summary>
		/// <param name="startDateInclusive">  the start date, inclusive, specifying the chronology of the calculation, not null </param>
		/// <param name="endDateExclusive">  the end date, exclusive, in any chronology, not null </param>
		/// <returns> the period between this date and the end date, not null </returns>
		/// <seealso cref= ChronoLocalDate#until(ChronoLocalDate) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static ChronoPeriod between(ChronoLocalDate startDateInclusive, ChronoLocalDate endDateExclusive)
	//	{
	//		Objects.requireNonNull(startDateInclusive, "startDateInclusive");
	//		Objects.requireNonNull(endDateExclusive, "endDateExclusive");
	//		return startDateInclusive.until(endDateExclusive);
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the value of the requested unit.
		/// <para>
		/// The supported units are chronology specific.
		/// They will typically be <seealso cref="ChronoUnit#YEARS YEARS"/>,
		/// <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
		/// Requesting an unsupported unit will throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit"> the {@code TemporalUnit} for which to return the value </param>
		/// <returns> the long value of the unit </returns>
		/// <exception cref="DateTimeException"> if the unit is not supported </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		long Get(TemporalUnit unit);

		/// <summary>
		/// Gets the set of units supported by this period.
		/// <para>
		/// The supported units are chronology specific.
		/// They will typically be <seealso cref="ChronoUnit#YEARS YEARS"/>,
		/// <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
		/// They are returned in order from largest to smallest.
		/// </para>
		/// <para>
		/// This set can be used in conjunction with <seealso cref="#get(TemporalUnit)"/>
		/// to access the entire state of the period.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a list containing the supported units, not null </returns>
		IList<TemporalUnit> Units {get;}

		/// <summary>
		/// Gets the chronology that defines the meaning of the supported units.
		/// <para>
		/// The period is defined by the chronology.
		/// It controls the supported units and restricts addition/subtraction
		/// to {@code ChronoLocalDate} instances of the same chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the chronology defining the period, not null </returns>
		Chronology Chronology {get;}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if all the supported units of this period are zero.
		/// </summary>
		/// <returns> true if this period is zero-length </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isZero()
	//	{
	//		for (TemporalUnit unit : getUnits())
	//		{
	//			if (get(unit) != 0)
	//			{
	//			}
	//		}
	//	}

		/// <summary>
		/// Checks if any of the supported units of this period are negative.
		/// </summary>
		/// <returns> true if any unit of this period is negative </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean isNegative()
	//	{
	//		for (TemporalUnit unit : getUnits())
	//		{
	//			if (get(unit) < 0)
	//			{
	//			}
	//		}
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the specified period added.
		/// <para>
		/// If the specified amount is a {@code ChronoPeriod} then it must have
		/// the same chronology as this period. Implementations may choose to
		/// accept or reject other {@code TemporalAmount} implementations.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToAdd">  the period to add, not null </param>
		/// <returns> a {@code ChronoPeriod} based on this period with the requested period added, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		ChronoPeriod Plus(TemporalAmount amountToAdd);

		/// <summary>
		/// Returns a copy of this period with the specified period subtracted.
		/// <para>
		/// If the specified amount is a {@code ChronoPeriod} then it must have
		/// the same chronology as this period. Implementations may choose to
		/// accept or reject other {@code TemporalAmount} implementations.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="amountToSubtract">  the period to subtract, not null </param>
		/// <returns> a {@code ChronoPeriod} based on this period with the requested period subtracted, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		ChronoPeriod Minus(TemporalAmount amountToSubtract);

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a new instance with each amount in this period in this period
		/// multiplied by the specified scalar.
		/// <para>
		/// This returns a period with each supported unit individually multiplied.
		/// For example, a period of "2 years, -3 months and 4 days" multiplied by
		/// 3 will return "6 years, -9 months and 12 days".
		/// No normalization is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="scalar">  the scalar to multiply by, not null </param>
		/// <returns> a {@code ChronoPeriod} based on this period with the amounts multiplied
		///  by the scalar, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		ChronoPeriod MultipliedBy(int scalar);

		/// <summary>
		/// Returns a new instance with each amount in this period negated.
		/// <para>
		/// This returns a period with each supported unit individually negated.
		/// For example, a period of "2 years, -3 months and 4 days" will be
		/// negated to "-2 years, 3 months and -4 days".
		/// No normalization is performed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ChronoPeriod} based on this period with the amounts negated, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs, which only happens if
		///  one of the units has the value {@code Long.MIN_VALUE} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default ChronoPeriod negated()
	//	{
	//		return multipliedBy(-1);
	//	}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a copy of this period with the amounts of each unit normalized.
		/// <para>
		/// The process of normalization is specific to each calendar system.
		/// For example, in the ISO calendar system, the years and months are
		/// normalized but the days are not, such that "15 months" would be
		/// normalized to "1 year and 3 months".
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code ChronoPeriod} based on this period with the amounts of each
		///  unit normalized, not null </returns>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		ChronoPeriod Normalized();

		//-------------------------------------------------------------------------
		/// <summary>
		/// Adds this period to the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this period added.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#plus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisPeriod.addTo(dateTime);
		///   dateTime = dateTime.plus(thisPeriod);
		/// </pre>
		/// </para>
		/// <para>
		/// The specified temporal must have the same chronology as this period.
		/// This returns a temporal with the non-zero supported units added.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to add </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Temporal AddTo(Temporal temporal);

		/// <summary>
		/// Subtracts this period from the specified temporal object.
		/// <para>
		/// This returns a temporal object of the same observable type as the input
		/// with this period subtracted.
		/// </para>
		/// <para>
		/// In most cases, it is clearer to reverse the calling pattern by using
		/// <seealso cref="Temporal#minus(TemporalAmount)"/>.
		/// <pre>
		///   // these two lines are equivalent, but the second approach is recommended
		///   dateTime = thisPeriod.subtractFrom(dateTime);
		///   dateTime = dateTime.minus(thisPeriod);
		/// </pre>
		/// </para>
		/// <para>
		/// The specified temporal must have the same chronology as this period.
		/// This returns a temporal with the non-zero supported units subtracted.
		/// </para>
		/// <para>
		/// This instance is immutable and unaffected by this method call.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporal">  the temporal object to adjust, not null </param>
		/// <returns> an object of the same type with the adjustment made, not null </returns>
		/// <exception cref="DateTimeException"> if unable to subtract </exception>
		/// <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Temporal SubtractFrom(Temporal temporal);

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this period is equal to another period, including the chronology.
		/// <para>
		/// Compares this period with another ensuring that the type, each amount and
		/// the chronology are the same.
		/// Note that this means that a period of "15 Months" is not equal to a period
		/// of "1 Year and 3 Months".
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other period </returns>
		bool Equals(Object obj);

		/// <summary>
		/// A hash code for this period.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		int HashCode();

		//-----------------------------------------------------------------------
		/// <summary>
		/// Outputs this period as a {@code String}.
		/// <para>
		/// The output will include the period amounts and chronology.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this period, not null </returns>
		String ToString();

	}

	public static class ChronoPeriod_Fields
	{
					public static readonly return False;
			public static readonly return True;
					public static readonly return True;
			public static readonly return False;
	}

}