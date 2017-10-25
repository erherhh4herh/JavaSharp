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
namespace java.util
{


	/// <summary>
	/// A state object for collecting statistics such as count, min, max, sum, and
	/// average.
	/// 
	/// <para>This class is designed to work with (though does not require)
	/// <seealso cref="java.util.stream streams"/>. For example, you can compute
	/// summary statistics on a stream of doubles with:
	/// <pre> {@code
	/// DoubleSummaryStatistics stats = doubleStream.collect(DoubleSummaryStatistics::new,
	///                                                      DoubleSummaryStatistics::accept,
	///                                                      DoubleSummaryStatistics::combine);
	/// }</pre>
	/// 
	/// </para>
	/// <para>{@code DoubleSummaryStatistics} can be used as a
	/// <seealso cref="java.util.stream.Stream#collect(Collector) reduction"/>
	/// target for a <seealso cref="java.util.stream.Stream stream"/>. For example:
	/// 
	/// <pre> {@code
	/// DoubleSummaryStatistics stats = people.stream()
	///     .collect(Collectors.summarizingDouble(Person::getWeight));
	/// }</pre>
	/// 
	/// This computes, in a single pass, the count of people, as well as the minimum,
	/// maximum, sum, and average of their weights.
	/// 
	/// @implNote This implementation is not thread safe. However, it is safe to use
	/// {@link java.util.stream.Collectors#summarizingDouble(java.util.function.ToDoubleFunction)
	/// Collectors.toDoubleStatistics()} on a parallel stream, because the parallel
	/// implementation of <seealso cref="java.util.stream.Stream#collect Stream.collect()"/>
	/// provides the necessary partitioning, isolation, and merging of results for
	/// safe and efficient parallel execution.
	/// @since 1.8
	/// </para>
	/// </summary>
	public class DoubleSummaryStatistics : DoubleConsumer
	{
		private long Count_Renamed;
		private double Sum_Renamed;
		private double SumCompensation; // Low order bits of sum
		private double SimpleSum; // Used to compute right sum for non-finite inputs
		private double Min_Renamed = Double.PositiveInfinity;
		private double Max_Renamed = Double.NegativeInfinity;

		/// <summary>
		/// Construct an empty instance with zero count, zero sum,
		/// {@code Double.POSITIVE_INFINITY} min, {@code Double.NEGATIVE_INFINITY}
		/// max and zero average.
		/// </summary>
		public DoubleSummaryStatistics()
		{
		}

		/// <summary>
		/// Records another value into the summary information.
		/// </summary>
		/// <param name="value"> the input value </param>
		public virtual void Accept(double value)
		{
			++Count_Renamed;
			SimpleSum += value;
			SumWithCompensation(value);
			Min_Renamed = System.Math.Min(Min_Renamed, value);
			Max_Renamed = System.Math.Max(Max_Renamed, value);
		}

		/// <summary>
		/// Combines the state of another {@code DoubleSummaryStatistics} into this
		/// one.
		/// </summary>
		/// <param name="other"> another {@code DoubleSummaryStatistics} </param>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public virtual void Combine(DoubleSummaryStatistics other)
		{
			Count_Renamed += other.Count_Renamed;
			SimpleSum += other.SimpleSum;
			SumWithCompensation(other.Sum_Renamed);
			SumWithCompensation(other.SumCompensation);
			Min_Renamed = System.Math.Min(Min_Renamed, other.Min_Renamed);
			Max_Renamed = System.Math.Max(Max_Renamed, other.Max_Renamed);
		}

		/// <summary>
		/// Incorporate a new double value using Kahan summation /
		/// compensated summation.
		/// </summary>
		private void SumWithCompensation(double value)
		{
			double tmp = value - SumCompensation;
			double velvel = Sum_Renamed + tmp; // Little wolf of rounding error
			SumCompensation = (velvel - Sum_Renamed) - tmp;
			Sum_Renamed = velvel;
		}

		/// <summary>
		/// Return the count of values recorded.
		/// </summary>
		/// <returns> the count of values </returns>
		public long Count
		{
			get
			{
				return Count_Renamed;
			}
		}

		/// <summary>
		/// Returns the sum of values recorded, or zero if no values have been
		/// recorded.
		/// 
		/// If any recorded value is a NaN or the sum is at any point a NaN
		/// then the sum will be NaN.
		/// 
		/// <para> The value of a floating-point sum is a function both of the
		/// input values as well as the order of addition operations. The
		/// order of addition operations of this method is intentionally
		/// not defined to allow for implementation flexibility to improve
		/// the speed and accuracy of the computed result.
		/// 
		/// In particular, this method may be implemented using compensated
		/// summation or other technique to reduce the error bound in the
		/// numerical sum compared to a simple summation of {@code double}
		/// values.
		/// 
		/// @apiNote Values sorted by increasing absolute magnitude tend to yield
		/// more accurate results.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the sum of values, or zero if none </returns>
		public double Sum
		{
			get
			{
				// Better error bounds to add both terms as the final sum
				double tmp = Sum_Renamed + SumCompensation;
				if (Double.IsNaN(tmp) && Double.IsInfinity(SimpleSum))
					// If the compensated sum is spuriously NaN from
					// accumulating one or more same-signed infinite values,
					// return the correctly-signed infinity stored in
					// simpleSum.
				{
					return SimpleSum;
				}
				else
				{
					return tmp;
				}
			}
		}

		/// <summary>
		/// Returns the minimum recorded value, {@code Double.NaN} if any recorded
		/// value was NaN or {@code Double.POSITIVE_INFINITY} if no values were
		/// recorded. Unlike the numerical comparison operators, this method
		/// considers negative zero to be strictly smaller than positive zero.
		/// </summary>
		/// <returns> the minimum recorded value, {@code Double.NaN} if any recorded
		/// value was NaN or {@code Double.POSITIVE_INFINITY} if no values were
		/// recorded </returns>
		public double Min
		{
			get
			{
				return Min_Renamed;
			}
		}

		/// <summary>
		/// Returns the maximum recorded value, {@code Double.NaN} if any recorded
		/// value was NaN or {@code Double.NEGATIVE_INFINITY} if no values were
		/// recorded. Unlike the numerical comparison operators, this method
		/// considers negative zero to be strictly smaller than positive zero.
		/// </summary>
		/// <returns> the maximum recorded value, {@code Double.NaN} if any recorded
		/// value was NaN or {@code Double.NEGATIVE_INFINITY} if no values were
		/// recorded </returns>
		public double Max
		{
			get
			{
				return Max_Renamed;
			}
		}

		/// <summary>
		/// Returns the arithmetic mean of values recorded, or zero if no
		/// values have been recorded.
		/// 
		/// If any recorded value is a NaN or the sum is at any point a NaN
		/// then the average will be code NaN.
		/// 
		/// <para>The average returned can vary depending upon the order in
		/// which values are recorded.
		/// 
		/// This method may be implemented using compensated summation or
		/// other technique to reduce the error bound in the {@link #getSum
		/// numerical sum} used to compute the average.
		/// 
		/// @apiNote Values sorted by increasing absolute magnitude tend to yield
		/// more accurate results.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the arithmetic mean of values, or zero if none </returns>
		public double Average
		{
			get
			{
				return Count > 0 ? Sum / Count : 0.0d;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns a non-empty string representation of this object suitable for
		/// debugging. The exact presentation format is unspecified and may vary
		/// between implementations and versions.
		/// </summary>
		public override String ToString()
		{
			return string.Format("{0}{{count={1:D}, sum={2:F}, min={3:F}, average={4:F}, max={5:F}}}", this.GetType().Name, Count, Sum, Min, Average, Max);
		}
	}

}