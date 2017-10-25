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
	/// summary statistics on a stream of ints with:
	/// <pre> {@code
	/// IntSummaryStatistics stats = intStream.collect(IntSummaryStatistics::new,
	///                                                IntSummaryStatistics::accept,
	///                                                IntSummaryStatistics::combine);
	/// }</pre>
	/// 
	/// </para>
	/// <para>{@code IntSummaryStatistics} can be used as a
	/// <seealso cref="java.util.stream.Stream#collect(Collector) reduction"/>
	/// target for a <seealso cref="java.util.stream.Stream stream"/>. For example:
	/// 
	/// <pre> {@code
	/// IntSummaryStatistics stats = people.stream()
	///                                    .collect(Collectors.summarizingInt(Person::getDependents));
	/// }</pre>
	/// 
	/// This computes, in a single pass, the count of people, as well as the minimum,
	/// maximum, sum, and average of their number of dependents.
	/// 
	/// @implNote This implementation is not thread safe. However, it is safe to use
	/// {@link java.util.stream.Collectors#summarizingInt(java.util.function.ToIntFunction)
	/// Collectors.toIntStatistics()} on a parallel stream, because the parallel
	/// implementation of <seealso cref="java.util.stream.Stream#collect Stream.collect()"/>
	/// provides the necessary partitioning, isolation, and merging of results for
	/// safe and efficient parallel execution.
	/// 
	/// </para>
	/// <para>This implementation does not check for overflow of the sum.
	/// @since 1.8
	/// </para>
	/// </summary>
	public class IntSummaryStatistics : IntConsumer
	{
		private long Count_Renamed;
		private long Sum_Renamed;
		private int Min_Renamed = Integer.MaxValue;
		private int Max_Renamed = Integer.MinValue;

		/// <summary>
		/// Construct an empty instance with zero count, zero sum,
		/// {@code Integer.MAX_VALUE} min, {@code Integer.MIN_VALUE} max and zero
		/// average.
		/// </summary>
		public IntSummaryStatistics()
		{
		}

		/// <summary>
		/// Records a new value into the summary information
		/// </summary>
		/// <param name="value"> the input value </param>
		public virtual void Accept(int value)
		{
			++Count_Renamed;
			Sum_Renamed += value;
			Min_Renamed = System.Math.Min(Min_Renamed, value);
			Max_Renamed = System.Math.Max(Max_Renamed, value);
		}

		/// <summary>
		/// Combines the state of another {@code IntSummaryStatistics} into this one.
		/// </summary>
		/// <param name="other"> another {@code IntSummaryStatistics} </param>
		/// <exception cref="NullPointerException"> if {@code other} is null </exception>
		public virtual void Combine(IntSummaryStatistics other)
		{
			Count_Renamed += other.Count_Renamed;
			Sum_Renamed += other.Sum_Renamed;
			Min_Renamed = System.Math.Min(Min_Renamed, other.Min_Renamed);
			Max_Renamed = System.Math.Max(Max_Renamed, other.Max_Renamed);
		}

		/// <summary>
		/// Returns the count of values recorded.
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
		/// </summary>
		/// <returns> the sum of values, or zero if none </returns>
		public long Sum
		{
			get
			{
				return Sum_Renamed;
			}
		}

		/// <summary>
		/// Returns the minimum value recorded, or {@code Integer.MAX_VALUE} if no
		/// values have been recorded.
		/// </summary>
		/// <returns> the minimum value, or {@code Integer.MAX_VALUE} if none </returns>
		public int Min
		{
			get
			{
				return Min_Renamed;
			}
		}

		/// <summary>
		/// Returns the maximum value recorded, or {@code Integer.MIN_VALUE} if no
		/// values have been recorded.
		/// </summary>
		/// <returns> the maximum value, or {@code Integer.MIN_VALUE} if none </returns>
		public int Max
		{
			get
			{
				return Max_Renamed;
			}
		}

		/// <summary>
		/// Returns the arithmetic mean of values recorded, or zero if no values have been
		/// recorded.
		/// </summary>
		/// <returns> the arithmetic mean of values, or zero if none </returns>
		public double Average
		{
			get
			{
				return Count > 0 ? (double) Sum / Count : 0.0d;
			}
		}

		public override String ToString()
		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns a non-empty string representation of this object suitable for
		/// debugging. The exact presentation format is unspecified and may vary
		/// between implementations and versions.
		/// </summary>
		{
			return string.Format("{0}{{count={1:D}, sum={2:D}, min={3:D}, average={4:F}, max={5:D}}}", this.GetType().Name, Count, Sum, Min, Average, Max);
		}
	}

}