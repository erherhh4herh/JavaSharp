using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// This class contains various methods for manipulating arrays (such as
	/// sorting and searching). This class also contains a static factory
	/// that allows arrays to be viewed as lists.
	/// 
	/// <para>The methods in this class all throw a {@code NullPointerException},
	/// if the specified array reference is null, except where noted.
	/// 
	/// </para>
	/// <para>The documentation for the methods contained in this class includes
	/// briefs description of the <i>implementations</i>. Such descriptions should
	/// be regarded as <i>implementation notes</i>, rather than parts of the
	/// <i>specification</i>. Implementors should feel free to substitute other
	/// algorithms, so long as the specification itself is adhered to. (For
	/// example, the algorithm used by {@code sort(Object[])} does not have to be
	/// a MergeSort, but it does have to be <i>stable</i>.)
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Josh Bloch
	/// @author Neal Gafter
	/// @author John Rose
	/// @since  1.2
	/// </para>
	/// </summary>
	public class Arrays
	{

		/// <summary>
		/// The minimum array length below which a parallel sorting
		/// algorithm will not further partition the sorting task. Using
		/// smaller sizes typically results in memory contention across
		/// tasks that makes parallel speedups unlikely.
		/// </summary>
		private static readonly int MIN_ARRAY_SORT_GRAN = 1 << 13;

		// Suppresses default constructor, ensuring non-instantiability.
		private Arrays()
		{
		}

		/// <summary>
		/// A comparator that implements the natural ordering of a group of
		/// mutually comparable elements. May be used when a supplied
		/// comparator is null. To simplify code-sharing within underlying
		/// implementations, the compare method only declares type Object
		/// for its second argument.
		/// 
		/// Arrays class implementor's note: It is an empirical matter
		/// whether ComparableTimSort offers any performance benefit over
		/// TimSort used with this comparator.  If not, you are better off
		/// deleting or bypassing ComparableTimSort.  There is currently no
		/// empirical case for separating them for parallel sorting, so all
		/// public Object parallelSort methods use the same comparator
		/// based implementation.
		/// </summary>
		internal sealed class NaturalOrder : Comparator<Object>
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public int compare(Object first, Object second)
			public int Compare(Object first, Object second)
			{
				return ((Comparable<Object>)first).CompareTo(second);
			}
			internal static readonly NaturalOrder INSTANCE = new NaturalOrder();
		}

		/// <summary>
		/// Checks that {@code fromIndex} and {@code toIndex} are in
		/// the range and throws an exception if they aren't.
		/// </summary>
		private static void RangeCheck(int arrayLength, int fromIndex, int toIndex)
		{
			if (fromIndex > toIndex)
			{
				throw new IllegalArgumentException("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
			}
			if (fromIndex < 0)
			{
				throw new ArrayIndexOutOfBoundsException(fromIndex);
			}
			if (toIndex > arrayLength)
			{
				throw new ArrayIndexOutOfBoundsException(toIndex);
			}
		}

		/*
		 * Sorting methods. Note that all public "sort" methods take the
		 * same form: Performing argument checks if necessary, and then
		 * expanding arguments into those required for the internal
		 * implementation methods residing in other package-private
		 * classes (except for legacyMergeSort, included in this class).
		 */

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(int[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(int[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(long[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(long[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(short[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(short[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(char[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(char[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(sbyte[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(sbyte[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all float
		/// values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		/// {@code 0.0f} and {@code Float.NaN} is considered greater than any
		/// other value and all {@code Float.NaN} values are considered equal.
		/// 
		/// </para>
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(float[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all float
		/// values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		/// {@code 0.0f} and {@code Float.NaN} is considered greater than any
		/// other value and all {@code Float.NaN} values are considered equal.
		/// 
		/// </para>
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(float[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all double
		/// values: {@code -0.0d == 0.0d} is {@code true} and a {@code Double.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		/// {@code 0.0d} and {@code Double.NaN} is considered greater than any
		/// other value and all {@code Double.NaN} values are considered equal.
		/// 
		/// </para>
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		public static void Sort(double[] a)
		{
			DualPivotQuicksort.Sort(a, 0, a.Length - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending order. The range
		/// to be sorted extends from the index {@code fromIndex}, inclusive, to
		/// the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		/// the range to be sorted is empty.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all double
		/// values: {@code -0.0d == 0.0d} is {@code true} and a {@code Double.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		/// {@code 0.0d} and {@code Double.NaN} is considered greater than any
		/// other value and all {@code Double.NaN} values are considered equal.
		/// 
		/// </para>
		/// <para>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		/// by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		/// offers O(n log(n)) performance on many data sets that cause other
		/// quicksorts to degrade to quadratic performance, and is typically
		/// faster than traditional (one-pivot) Quicksort implementations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		public static void Sort(double[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(byte[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(byte[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(sbyte[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJByte.Sorter(null, a, new sbyte[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(byte[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(byte[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(sbyte[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJByte.Sorter(null, a, new sbyte[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(char[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(char[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(char[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJChar.Sorter(null, a, new char[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		///  @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(char[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(char[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(char[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJChar.Sorter(null, a, new char[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(short[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(short[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(short[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJShort.Sorter(null, a, new short[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(short[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(short[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(short[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJShort.Sorter(null, a, new short[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(int[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(int[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(int[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJInt.Sorter(null, a, new int[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(int[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(int[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(int[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJInt.Sorter(null, a, new int[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(long[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(long[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(long[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJLong.Sorter(null, a, new long[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(long[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(long[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(long[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJLong.Sorter(null, a, new long[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all float
		/// values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		/// {@code 0.0f} and {@code Float.NaN} is considered greater than any
		/// other value and all {@code Float.NaN} values are considered equal.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(float[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(float[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(float[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJFloat.Sorter(null, a, new float[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all float
		/// values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		/// {@code 0.0f} and {@code Float.NaN} is considered greater than any
		/// other value and all {@code Float.NaN} values are considered equal.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(float[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(float[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(float[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJFloat.Sorter(null, a, new float[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array into ascending numerical order.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all double
		/// values: {@code -0.0d == 0.0d} is {@code true} and a {@code Double.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		/// {@code 0.0d} and {@code Double.NaN} is considered greater than any
		/// other value and all {@code Double.NaN} values are considered equal.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(double[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(double[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted
		/// 
		/// @since 1.8 </param>
		public static void ParallelSort(double[] a)
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, 0, n - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJDouble.Sorter(null, a, new double[n], 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the array into ascending numerical order.
		/// The range to be sorted extends from the index {@code fromIndex},
		/// inclusive, to the index {@code toIndex}, exclusive. If
		/// {@code fromIndex == toIndex}, the range to be sorted is empty.
		/// 
		/// <para>The {@code <} relation does not provide a total order on all double
		/// values: {@code -0.0d == 0.0d} is {@code true} and a {@code Double.NaN}
		/// value compares neither less than, greater than, nor equal to any value,
		/// even itself. This method uses the total order imposed by the method
		/// <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		/// {@code 0.0d} and {@code Double.NaN} is considered greater than any
		/// other value and all {@code Double.NaN} values are considered equal.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(double[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(double[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="toIndex"> the index of the last element, exclusive, to be sorted
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > a.length}
		/// 
		/// @since 1.8 </exception>
		public static void ParallelSort(double[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				DualPivotQuicksort.Sort(a, fromIndex, toIndex - 1, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJDouble.Sorter(null, a, new double[n], fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array of objects into ascending order, according
		/// to the <seealso cref="Comparable natural ordering"/> of its elements.
		/// All elements in the array must implement the <seealso cref="Comparable"/>
		/// interface.  Furthermore, all elements in the array must be
		/// <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)} must
		/// not throw a {@code ClassCastException} for any elements {@code e1}
		/// and {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted
		/// </param>
		/// <exception cref="ClassCastException"> if the array contains elements that are not
		///         <i>mutually comparable</i> (for example, strings and integers) </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the natural
		///         ordering of the array elements is found to violate the
		///         <seealso cref="Comparable"/> contract
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void parallelSort(T[] a)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void parallelSort(T[] a)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void parallelSort<T>(T[] a) where T : Comparable<? base T>
		{
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				TimSort.Sort(a, 0, n, NaturalOrder.INSTANCE, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJObject.Sorter<T> (null, a, (T[])Array.newInstance(a.GetType().GetElementType(), n), 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g, NaturalOrder.INSTANCE)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the specified array of objects into
		/// ascending order, according to the
		/// <seealso cref="Comparable natural ordering"/> of its
		/// elements.  The range to be sorted extends from index
		/// {@code fromIndex}, inclusive, to index {@code toIndex}, exclusive.
		/// (If {@code fromIndex==toIndex}, the range to be sorted is empty.)  All
		/// elements in this range must implement the <seealso cref="Comparable"/>
		/// interface.  Furthermore, all elements in this range must be <i>mutually
		/// comparable</i> (that is, {@code e1.compareTo(e2)} must not throw a
		/// {@code ClassCastException} for any elements {@code e1} and
		/// {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        sorted </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		///         (optional) if the natural ordering of the array elements is
		///         found to violate the <seealso cref="Comparable"/> contract </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		///         {@code toIndex > a.length} </exception>
		/// <exception cref="ClassCastException"> if the array contains elements that are
		///         not <i>mutually comparable</i> (for example, strings and
		///         integers).
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void parallelSort(T[] a, int fromIndex, int toIndex)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void parallelSort(T[] a, int fromIndex, int toIndex)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void parallelSort<T>(T[] a, int fromIndex, int toIndex) where T : Comparable<? base T>
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				TimSort.Sort(a, fromIndex, toIndex, NaturalOrder.INSTANCE, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJObject.Sorter<T> (null, a, (T[])Array.newInstance(a.GetType().GetElementType(), n), fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g, NaturalOrder.INSTANCE)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified array of objects according to the order induced by
		/// the specified comparator.  All elements in the array must be
		/// <i>mutually comparable</i> by the specified comparator (that is,
		/// {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		/// for any elements {@code e1} and {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a
		/// working space no greater than the size of the original array. The
		/// <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		/// execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="cmp"> the comparator to determine the order of the array.  A
		///        {@code null} value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <exception cref="ClassCastException"> if the array contains elements that are
		///         not <i>mutually comparable</i> using the specified comparator </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the comparator is
		///         found to violate the <seealso cref="java.util.Comparator"/> contract
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> void parallelSort(T[] a, Comparator<? base T> cmp)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public static void parallelSort<T, T1>(T[] a, Comparator<T1> cmp)
		{
			if (cmp == null)
			{
				cmp = NaturalOrder.INSTANCE;
			}
			int n = a.Length, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				TimSort.Sort(a, 0, n, cmp, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJObject.Sorter<T> (null, a, (T[])Array.newInstance(a.GetType().GetElementType(), n), 0, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g, cmp)).Invoke();
			}
		}

		/// <summary>
		/// Sorts the specified range of the specified array of objects according
		/// to the order induced by the specified comparator.  The range to be
		/// sorted extends from index {@code fromIndex}, inclusive, to index
		/// {@code toIndex}, exclusive.  (If {@code fromIndex==toIndex}, the
		/// range to be sorted is empty.)  All elements in the range must be
		/// <i>mutually comparable</i> by the specified comparator (that is,
		/// {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		/// for any elements {@code e1} and {@code e2} in the range).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// @implNote The sorting algorithm is a parallel sort-merge that breaks the
		/// array into sub-arrays that are themselves sorted and then merged. When
		/// the sub-array length reaches a minimum granularity, the sub-array is
		/// sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		/// method. If the length of the specified array is less than the minimum
		/// granularity, then it is sorted using the appropriate {@link
		/// Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a working
		/// space no greater than the size of the specified range of the original
		/// array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		/// used to execute any parallel tasks.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        sorted </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		/// <param name="cmp"> the comparator to determine the order of the array.  A
		///        {@code null} value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		///         (optional) if the natural ordering of the array elements is
		///         found to violate the <seealso cref="Comparable"/> contract </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		///         {@code toIndex > a.length} </exception>
		/// <exception cref="ClassCastException"> if the array contains elements that are
		///         not <i>mutually comparable</i> (for example, strings and
		///         integers).
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> void parallelSort(T[] a, int fromIndex, int toIndex, Comparator<? base T> cmp)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public static void parallelSort<T, T1>(T[] a, int fromIndex, int toIndex, Comparator<T1> cmp)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			if (cmp == null)
			{
				cmp = NaturalOrder.INSTANCE;
			}
			int n = toIndex - fromIndex, p , g ;
			if (n <= MIN_ARRAY_SORT_GRAN || (p = ForkJoinPool.CommonPoolParallelism) == 1)
			{
				TimSort.Sort(a, fromIndex, toIndex, cmp, null, 0, 0);
			}
			else
			{
				(new ArraysParallelSortHelpers.FJObject.Sorter<T> (null, a, (T[])Array.newInstance(a.GetType().GetElementType(), n), fromIndex, n, 0, ((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN) ? MIN_ARRAY_SORT_GRAN : g, cmp)).Invoke();
			}
		}

		/*
		 * Sorting of complex type arrays.
		 */

		/// <summary>
		/// Old merge sort implementation can be selected (for
		/// compatibility with broken comparators) using a system property.
		/// Cannot be a static boolean in the enclosing class due to
		/// circular dependencies. To be removed in a future release.
		/// </summary>
		internal sealed class LegacyMergeSort
		{
			internal static readonly bool UserRequested = (bool)java.security.AccessController.doPrivileged(new sun.security.action.GetBooleanAction("java.util.Arrays.useLegacyMergeSort"));
		}

		/// <summary>
		/// Sorts the specified array of objects into ascending order, according
		/// to the <seealso cref="Comparable natural ordering"/> of its elements.
		/// All elements in the array must implement the <seealso cref="Comparable"/>
		/// interface.  Furthermore, all elements in the array must be
		/// <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)} must
		/// not throw a {@code ClassCastException} for any elements {@code e1}
		/// and {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>Implementation note: This implementation is a stable, adaptive,
		/// iterative mergesort that requires far fewer than n lg(n) comparisons
		/// when the input array is partially sorted, while offering the
		/// performance of a traditional mergesort when the input array is
		/// randomly ordered.  If the input array is nearly sorted, the
		/// implementation requires approximately n comparisons.  Temporary
		/// storage requirements vary from a small constant for nearly sorted
		/// input arrays to n/2 object references for randomly ordered input
		/// arrays.
		/// 
		/// </para>
		/// <para>The implementation takes equal advantage of ascending and
		/// descending order in its input array, and can take advantage of
		/// ascending and descending order in different parts of the the same
		/// input array.  It is well-suited to merging two or more sorted arrays:
		/// simply concatenate the arrays and sort the resulting array.
		/// 
		/// </para>
		/// <para>The implementation was adapted from Tim Peters's list sort for Python
		/// (<a href="http://svn.python.org/projects/python/trunk/Objects/listsort.txt">
		/// TimSort</a>).  It uses techniques from Peter McIlroy's "Optimistic
		/// Sorting and Information Theoretic Complexity", in Proceedings of the
		/// Fourth Annual ACM-SIAM Symposium on Discrete Algorithms, pp 467-474,
		/// January 1993.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <exception cref="ClassCastException"> if the array contains elements that are not
		///         <i>mutually comparable</i> (for example, strings and integers) </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the natural
		///         ordering of the array elements is found to violate the
		///         <seealso cref="Comparable"/> contract </exception>
		public static void Sort(Object[] a)
		{
			if (LegacyMergeSort.UserRequested)
			{
				LegacyMergeSort(a);
			}
			else
			{
				ComparableTimSort.Sort(a, 0, a.Length, null, 0, 0);
			}
		}

		/// <summary>
		/// To be removed in a future release. </summary>
		private static void LegacyMergeSort(Object[] a)
		{
			Object[] aux = a.clone();
			MergeSort(aux, a, 0, a.Length, 0);
		}

		/// <summary>
		/// Sorts the specified range of the specified array of objects into
		/// ascending order, according to the
		/// <seealso cref="Comparable natural ordering"/> of its
		/// elements.  The range to be sorted extends from index
		/// {@code fromIndex}, inclusive, to index {@code toIndex}, exclusive.
		/// (If {@code fromIndex==toIndex}, the range to be sorted is empty.)  All
		/// elements in this range must implement the <seealso cref="Comparable"/>
		/// interface.  Furthermore, all elements in this range must be <i>mutually
		/// comparable</i> (that is, {@code e1.compareTo(e2)} must not throw a
		/// {@code ClassCastException} for any elements {@code e1} and
		/// {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>Implementation note: This implementation is a stable, adaptive,
		/// iterative mergesort that requires far fewer than n lg(n) comparisons
		/// when the input array is partially sorted, while offering the
		/// performance of a traditional mergesort when the input array is
		/// randomly ordered.  If the input array is nearly sorted, the
		/// implementation requires approximately n comparisons.  Temporary
		/// storage requirements vary from a small constant for nearly sorted
		/// input arrays to n/2 object references for randomly ordered input
		/// arrays.
		/// 
		/// </para>
		/// <para>The implementation takes equal advantage of ascending and
		/// descending order in its input array, and can take advantage of
		/// ascending and descending order in different parts of the the same
		/// input array.  It is well-suited to merging two or more sorted arrays:
		/// simply concatenate the arrays and sort the resulting array.
		/// 
		/// </para>
		/// <para>The implementation was adapted from Tim Peters's list sort for Python
		/// (<a href="http://svn.python.org/projects/python/trunk/Objects/listsort.txt">
		/// TimSort</a>).  It uses techniques from Peter McIlroy's "Optimistic
		/// Sorting and Information Theoretic Complexity", in Proceedings of the
		/// Fourth Annual ACM-SIAM Symposium on Discrete Algorithms, pp 467-474,
		/// January 1993.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        sorted </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		///         (optional) if the natural ordering of the array elements is
		///         found to violate the <seealso cref="Comparable"/> contract </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		///         {@code toIndex > a.length} </exception>
		/// <exception cref="ClassCastException"> if the array contains elements that are
		///         not <i>mutually comparable</i> (for example, strings and
		///         integers). </exception>
		public static void Sort(Object[] a, int fromIndex, int toIndex)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			if (LegacyMergeSort.UserRequested)
			{
				LegacyMergeSort(a, fromIndex, toIndex);
			}
			else
			{
				ComparableTimSort.Sort(a, fromIndex, toIndex, null, 0, 0);
			}
		}

		/// <summary>
		/// To be removed in a future release. </summary>
		private static void LegacyMergeSort(Object[] a, int fromIndex, int toIndex)
		{
			Object[] aux = CopyOfRange(a, fromIndex, toIndex);
			MergeSort(aux, a, fromIndex, toIndex, -fromIndex);
		}

		/// <summary>
		/// Tuning parameter: list size at or below which insertion sort will be
		/// used in preference to mergesort.
		/// To be removed in a future release.
		/// </summary>
		private const int INSERTIONSORT_THRESHOLD = 7;

		/// <summary>
		/// Src is the source array that starts at index 0
		/// Dest is the (possibly larger) array destination with a possible offset
		/// low is the index in dest to start sorting
		/// high is the end index in dest to end sorting
		/// off is the offset to generate corresponding low, high in src
		/// To be removed in a future release.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) private static void mergeSort(Object[] src, Object[] dest, int low, int high, int off)
		private static void MergeSort(Object[] src, Object[] dest, int low, int high, int off)
		{
			int length = high - low;

			// Insertion sort on smallest arrays
			if (length < INSERTIONSORT_THRESHOLD)
			{
				for (int i = low; i < high; i++)
				{
					for (int j = i; j > low && ((Comparable) dest[j - 1]).CompareTo(dest[j]) > 0; j--)
					{
						Swap(dest, j, j - 1);
					}
				}
				return;
			}

			// Recursively sort halves of dest into src
			int destLow = low;
			int destHigh = high;
			low += off;
			high += off;
			int mid = (int)((uint)(low + high) >> 1);
			MergeSort(dest, src, low, mid, -off);
			MergeSort(dest, src, mid, high, -off);

			// If list is already sorted, just copy from src to dest.  This is an
			// optimization that results in faster sorts for nearly ordered lists.
			if (((Comparable)src[mid - 1]).CompareTo(src[mid]) <= 0)
			{
				System.Array.Copy(src, low, dest, destLow, length);
				return;
			}

			// Merge sorted halves (now in src) into dest
			for (int i = destLow, p = low, q = mid; i < destHigh; i++)
			{
				if (q >= high || p < mid && ((Comparable)src[p]).CompareTo(src[q]) <= 0)
				{
					dest[i] = src[p++];
				}
				else
				{
					dest[i] = src[q++];
				}
			}
		}

		/// <summary>
		/// Swaps x[a] with x[b].
		/// </summary>
		private static void Swap(Object[] x, int a, int b)
		{
			Object t = x[a];
			x[a] = x[b];
			x[b] = t;
		}

		/// <summary>
		/// Sorts the specified array of objects according to the order induced by
		/// the specified comparator.  All elements in the array must be
		/// <i>mutually comparable</i> by the specified comparator (that is,
		/// {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		/// for any elements {@code e1} and {@code e2} in the array).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>Implementation note: This implementation is a stable, adaptive,
		/// iterative mergesort that requires far fewer than n lg(n) comparisons
		/// when the input array is partially sorted, while offering the
		/// performance of a traditional mergesort when the input array is
		/// randomly ordered.  If the input array is nearly sorted, the
		/// implementation requires approximately n comparisons.  Temporary
		/// storage requirements vary from a small constant for nearly sorted
		/// input arrays to n/2 object references for randomly ordered input
		/// arrays.
		/// 
		/// </para>
		/// <para>The implementation takes equal advantage of ascending and
		/// descending order in its input array, and can take advantage of
		/// ascending and descending order in different parts of the the same
		/// input array.  It is well-suited to merging two or more sorted arrays:
		/// simply concatenate the arrays and sort the resulting array.
		/// 
		/// </para>
		/// <para>The implementation was adapted from Tim Peters's list sort for Python
		/// (<a href="http://svn.python.org/projects/python/trunk/Objects/listsort.txt">
		/// TimSort</a>).  It uses techniques from Peter McIlroy's "Optimistic
		/// Sorting and Information Theoretic Complexity", in Proceedings of the
		/// Fourth Annual ACM-SIAM Symposium on Discrete Algorithms, pp 467-474,
		/// January 1993.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="c"> the comparator to determine the order of the array.  A
		///        {@code null} value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <exception cref="ClassCastException"> if the array contains elements that are
		///         not <i>mutually comparable</i> using the specified comparator </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the comparator is
		///         found to violate the <seealso cref="Comparator"/> contract </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> void sort(T[] a, Comparator<? base T> c)
		public static void sort<T, T1>(T[] a, Comparator<T1> c)
		{
			if (c == null)
			{
				Sort(a);
			}
			else
			{
				if (LegacyMergeSort.UserRequested)
				{
					LegacyMergeSort(a, c);
				}
				else
				{
					TimSort.Sort(a, 0, a.Length, c, null, 0, 0);
				}
			}
		}

		/// <summary>
		/// To be removed in a future release. </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> void legacyMergeSort(T[] a, Comparator<? base T> c)
		private static void legacyMergeSort<T, T1>(T[] a, Comparator<T1> c)
		{
			T[] aux = a.clone();
			if (c == null)
			{
				MergeSort(aux, a, 0, a.Length, 0);
			}
			else
			{
				MergeSort(aux, a, 0, a.Length, 0, c);
			}
		}

		/// <summary>
		/// Sorts the specified range of the specified array of objects according
		/// to the order induced by the specified comparator.  The range to be
		/// sorted extends from index {@code fromIndex}, inclusive, to index
		/// {@code toIndex}, exclusive.  (If {@code fromIndex==toIndex}, the
		/// range to be sorted is empty.)  All elements in the range must be
		/// <i>mutually comparable</i> by the specified comparator (that is,
		/// {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		/// for any elements {@code e1} and {@code e2} in the range).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>Implementation note: This implementation is a stable, adaptive,
		/// iterative mergesort that requires far fewer than n lg(n) comparisons
		/// when the input array is partially sorted, while offering the
		/// performance of a traditional mergesort when the input array is
		/// randomly ordered.  If the input array is nearly sorted, the
		/// implementation requires approximately n comparisons.  Temporary
		/// storage requirements vary from a small constant for nearly sorted
		/// input arrays to n/2 object references for randomly ordered input
		/// arrays.
		/// 
		/// </para>
		/// <para>The implementation takes equal advantage of ascending and
		/// descending order in its input array, and can take advantage of
		/// ascending and descending order in different parts of the the same
		/// input array.  It is well-suited to merging two or more sorted arrays:
		/// simply concatenate the arrays and sort the resulting array.
		/// 
		/// </para>
		/// <para>The implementation was adapted from Tim Peters's list sort for Python
		/// (<a href="http://svn.python.org/projects/python/trunk/Objects/listsort.txt">
		/// TimSort</a>).  It uses techniques from Peter McIlroy's "Optimistic
		/// Sorting and Information Theoretic Complexity", in Proceedings of the
		/// Fourth Annual ACM-SIAM Symposium on Discrete Algorithms, pp 467-474,
		/// January 1993.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects to be sorted </param>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        sorted </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		/// <param name="c"> the comparator to determine the order of the array.  A
		///        {@code null} value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <exception cref="ClassCastException"> if the array contains elements that are not
		///         <i>mutually comparable</i> using the specified comparator. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		///         (optional) if the comparator is found to violate the
		///         <seealso cref="Comparator"/> contract </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		///         {@code toIndex > a.length} </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> void sort(T[] a, int fromIndex, int toIndex, Comparator<? base T> c)
		public static void sort<T, T1>(T[] a, int fromIndex, int toIndex, Comparator<T1> c)
		{
			if (c == null)
			{
				Sort(a, fromIndex, toIndex);
			}
			else
			{
				RangeCheck(a.Length, fromIndex, toIndex);
				if (LegacyMergeSort.UserRequested)
				{
					LegacyMergeSort(a, fromIndex, toIndex, c);
				}
				else
				{
					TimSort.Sort(a, fromIndex, toIndex, c, null, 0, 0);
				}
			}
		}

		/// <summary>
		/// To be removed in a future release. </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> void legacyMergeSort(T[] a, int fromIndex, int toIndex, Comparator<? base T> c)
		private static void legacyMergeSort<T, T1>(T[] a, int fromIndex, int toIndex, Comparator<T1> c)
		{
			T[] aux = CopyOfRange(a, fromIndex, toIndex);
			if (c == null)
			{
				MergeSort(aux, a, fromIndex, toIndex, -fromIndex);
			}
			else
			{
				MergeSort(aux, a, fromIndex, toIndex, -fromIndex, c);
			}
		}

		/// <summary>
		/// Src is the source array that starts at index 0
		/// Dest is the (possibly larger) array destination with a possible offset
		/// low is the index in dest to start sorting
		/// high is the end index in dest to end sorting
		/// off is the offset into src corresponding to low in dest
		/// To be removed in a future release.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) private static void mergeSort(Object[] src, Object[] dest, int low, int high, int off, Comparator c)
		private static void MergeSort(Object[] src, Object[] dest, int low, int high, int off, Comparator c)
		{
			int length = high - low;

			// Insertion sort on smallest arrays
			if (length < INSERTIONSORT_THRESHOLD)
			{
				for (int i = low; i < high; i++)
				{
					for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
					{
						Swap(dest, j, j - 1);
					}
				}
				return;
			}

			// Recursively sort halves of dest into src
			int destLow = low;
			int destHigh = high;
			low += off;
			high += off;
			int mid = (int)((uint)(low + high) >> 1);
			MergeSort(dest, src, low, mid, -off, c);
			MergeSort(dest, src, mid, high, -off, c);

			// If list is already sorted, just copy from src to dest.  This is an
			// optimization that results in faster sorts for nearly ordered lists.
			if (c.Compare(src[mid - 1], src[mid]) <= 0)
			{
			   System.Array.Copy(src, low, dest, destLow, length);
			   return;
			}

			// Merge sorted halves (now in src) into dest
			for (int i = destLow, p = low, q = mid; i < destHigh; i++)
			{
				if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
				{
					dest[i] = src[p++];
				}
				else
				{
					dest[i] = src[q++];
				}
			}
		}

		// Parallel prefix

		/// <summary>
		/// Cumulates, in parallel, each element of the given array in place,
		/// using the supplied function. For example if the array initially
		/// holds {@code [2, 1, 0, 3]} and the operation performs addition,
		/// then upon return the array holds {@code [2, 3, 3, 6]}.
		/// Parallel prefix computation is usually more efficient than
		/// sequential loops for large arrays.
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="array"> the array, which is modified in-place by this method </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void parallelPrefix<T>(T[] array, BinaryOperator<T> op)
		{
			Objects.RequireNonNull(op);
			if (array.Length > 0)
			{
				(new ArrayPrefixHelpers.CumulateTask<> (null, op, array, 0, array.Length)).Invoke();
			}
		}

		/// <summary>
		/// Performs <seealso cref="#parallelPrefix(Object[], BinaryOperator)"/>
		/// for the given subrange of the array.
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="array"> the array </param>
		/// <param name="fromIndex"> the index of the first element, inclusive </param>
		/// <param name="toIndex"> the index of the last element, exclusive </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void parallelPrefix<T>(T[] array, int fromIndex, int toIndex, BinaryOperator<T> op)
		{
			Objects.RequireNonNull(op);
			RangeCheck(array.Length, fromIndex, toIndex);
			if (fromIndex < toIndex)
			{
				(new ArrayPrefixHelpers.CumulateTask<> (null, op, array, fromIndex, toIndex)).Invoke();
			}
		}

		/// <summary>
		/// Cumulates, in parallel, each element of the given array in place,
		/// using the supplied function. For example if the array initially
		/// holds {@code [2, 1, 0, 3]} and the operation performs addition,
		/// then upon return the array holds {@code [2, 3, 3, 6]}.
		/// Parallel prefix computation is usually more efficient than
		/// sequential loops for large arrays.
		/// </summary>
		/// <param name="array"> the array, which is modified in-place by this method </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(long[] array, LongBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			if (array.Length > 0)
			{
				(new ArrayPrefixHelpers.LongCumulateTask(null, op, array, 0, array.Length)).Invoke();
			}
		}

		/// <summary>
		/// Performs <seealso cref="#parallelPrefix(long[], LongBinaryOperator)"/>
		/// for the given subrange of the array.
		/// </summary>
		/// <param name="array"> the array </param>
		/// <param name="fromIndex"> the index of the first element, inclusive </param>
		/// <param name="toIndex"> the index of the last element, exclusive </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(long[] array, int fromIndex, int toIndex, LongBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			RangeCheck(array.Length, fromIndex, toIndex);
			if (fromIndex < toIndex)
			{
				(new ArrayPrefixHelpers.LongCumulateTask(null, op, array, fromIndex, toIndex)).Invoke();
			}
		}

		/// <summary>
		/// Cumulates, in parallel, each element of the given array in place,
		/// using the supplied function. For example if the array initially
		/// holds {@code [2.0, 1.0, 0.0, 3.0]} and the operation performs addition,
		/// then upon return the array holds {@code [2.0, 3.0, 3.0, 6.0]}.
		/// Parallel prefix computation is usually more efficient than
		/// sequential loops for large arrays.
		/// 
		/// <para> Because floating-point operations may not be strictly associative,
		/// the returned result may not be identical to the value that would be
		/// obtained if the operation was performed sequentially.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, which is modified in-place by this method </param>
		/// <param name="op"> a side-effect-free function to perform the cumulation </param>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(double[] array, DoubleBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			if (array.Length > 0)
			{
				(new ArrayPrefixHelpers.DoubleCumulateTask(null, op, array, 0, array.Length)).Invoke();
			}
		}

		/// <summary>
		/// Performs <seealso cref="#parallelPrefix(double[], DoubleBinaryOperator)"/>
		/// for the given subrange of the array.
		/// </summary>
		/// <param name="array"> the array </param>
		/// <param name="fromIndex"> the index of the first element, inclusive </param>
		/// <param name="toIndex"> the index of the last element, exclusive </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(double[] array, int fromIndex, int toIndex, DoubleBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			RangeCheck(array.Length, fromIndex, toIndex);
			if (fromIndex < toIndex)
			{
				(new ArrayPrefixHelpers.DoubleCumulateTask(null, op, array, fromIndex, toIndex)).Invoke();
			}
		}

		/// <summary>
		/// Cumulates, in parallel, each element of the given array in place,
		/// using the supplied function. For example if the array initially
		/// holds {@code [2, 1, 0, 3]} and the operation performs addition,
		/// then upon return the array holds {@code [2, 3, 3, 6]}.
		/// Parallel prefix computation is usually more efficient than
		/// sequential loops for large arrays.
		/// </summary>
		/// <param name="array"> the array, which is modified in-place by this method </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(int[] array, IntBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			if (array.Length > 0)
			{
				(new ArrayPrefixHelpers.IntCumulateTask(null, op, array, 0, array.Length)).Invoke();
			}
		}

		/// <summary>
		/// Performs <seealso cref="#parallelPrefix(int[], IntBinaryOperator)"/>
		/// for the given subrange of the array.
		/// </summary>
		/// <param name="array"> the array </param>
		/// <param name="fromIndex"> the index of the first element, inclusive </param>
		/// <param name="toIndex"> the index of the last element, exclusive </param>
		/// <param name="op"> a side-effect-free, associative function to perform the
		/// cumulation </param>
		/// <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		/// <exception cref="NullPointerException"> if the specified array or function is null
		/// @since 1.8 </exception>
		public static void ParallelPrefix(int[] array, int fromIndex, int toIndex, IntBinaryOperator op)
		{
			Objects.RequireNonNull(op);
			RangeCheck(array.Length, fromIndex, toIndex);
			if (fromIndex < toIndex)
			{
				(new ArrayPrefixHelpers.IntCumulateTask(null, op, array, fromIndex, toIndex)).Invoke();
			}
		}

		// Searching

		/// <summary>
		/// Searches the specified array of longs for the specified value using the
		/// binary search algorithm.  The array must be sorted (as
		/// by the <seealso cref="#sort(long[])"/> method) prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(long[] a, long key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of longs for the specified value using the
		/// binary search algorithm.
		/// The range must be sorted (as
		/// by the <seealso cref="#sort(long[], int, int)"/> method)
		/// prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(long[] a, int fromIndex, int toIndex, long key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(long[] a, int fromIndex, int toIndex, long key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				long midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1;
				}
				else if (midVal > key)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of ints for the specified value using the
		/// binary search algorithm.  The array must be sorted (as
		/// by the <seealso cref="#sort(int[])"/> method) prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(int[] a, int key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of ints for the specified value using the
		/// binary search algorithm.
		/// The range must be sorted (as
		/// by the <seealso cref="#sort(int[], int, int)"/> method)
		/// prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(int[] a, int fromIndex, int toIndex, int key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(int[] a, int fromIndex, int toIndex, int key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				int midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1;
				}
				else if (midVal > key)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of shorts for the specified value using
		/// the binary search algorithm.  The array must be sorted
		/// (as by the <seealso cref="#sort(short[])"/> method) prior to making this call.  If
		/// it is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(short[] a, short key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of shorts for the specified value using
		/// the binary search algorithm.
		/// The range must be sorted
		/// (as by the <seealso cref="#sort(short[], int, int)"/> method)
		/// prior to making this call.  If
		/// it is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(short[] a, int fromIndex, int toIndex, short key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(short[] a, int fromIndex, int toIndex, short key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				short midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1;
				}
				else if (midVal > key)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of chars for the specified value using the
		/// binary search algorithm.  The array must be sorted (as
		/// by the <seealso cref="#sort(char[])"/> method) prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(char[] a, char key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of chars for the specified value using the
		/// binary search algorithm.
		/// The range must be sorted (as
		/// by the <seealso cref="#sort(char[], int, int)"/> method)
		/// prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(char[] a, int fromIndex, int toIndex, char key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(char[] a, int fromIndex, int toIndex, char key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				char midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1;
				}
				else if (midVal > key)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of bytes for the specified value using the
		/// binary search algorithm.  The array must be sorted (as
		/// by the <seealso cref="#sort(byte[])"/> method) prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(sbyte[] a, sbyte key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of bytes for the specified value using the
		/// binary search algorithm.
		/// The range must be sorted (as
		/// by the <seealso cref="#sort(byte[], int, int)"/> method)
		/// prior to making this call.  If it
		/// is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(sbyte[] a, int fromIndex, int toIndex, sbyte key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(sbyte[] a, int fromIndex, int toIndex, sbyte key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				sbyte midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1;
				}
				else if (midVal > key)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of doubles for the specified value using
		/// the binary search algorithm.  The array must be sorted
		/// (as by the <seealso cref="#sort(double[])"/> method) prior to making this call.
		/// If it is not sorted, the results are undefined.  If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.  This method considers all NaN values to be
		/// equivalent and equal.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(double[] a, double key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of doubles for the specified value using
		/// the binary search algorithm.
		/// The range must be sorted
		/// (as by the <seealso cref="#sort(double[], int, int)"/> method)
		/// prior to making this call.
		/// If it is not sorted, the results are undefined.  If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found.  This method considers all NaN values to be
		/// equivalent and equal.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(double[] a, int fromIndex, int toIndex, double key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(double[] a, int fromIndex, int toIndex, double key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				double midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1; // Neither val is NaN, thisVal is smaller
				}
				else if (midVal > key)
				{
					high = mid - 1; // Neither val is NaN, thisVal is larger
				}
				else
				{
					long midBits = Double.DoubleToLongBits(midVal);
					long keyBits = Double.DoubleToLongBits(key);
					if (midBits == keyBits) // Values are equal
					{
						return mid; // Key found
					}
					else if (midBits < keyBits) // (-0.0, 0.0) or (!NaN, NaN)
					{
						low = mid + 1;
					}
					else // (0.0, -0.0) or (NaN, !NaN)
					{
						high = mid - 1;
					}
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array of floats for the specified value using
		/// the binary search algorithm. The array must be sorted
		/// (as by the <seealso cref="#sort(float[])"/> method) prior to making this call. If
		/// it is not sorted, the results are undefined. If the array contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found. This method considers all NaN values to be
		/// equivalent and equal.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>. The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key. Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		public static int BinarySearch(float[] a, float key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array of floats for the specified value using
		/// the binary search algorithm.
		/// The range must be sorted
		/// (as by the <seealso cref="#sort(float[], int, int)"/> method)
		/// prior to making this call. If
		/// it is not sorted, the results are undefined. If the range contains
		/// multiple elements with the specified value, there is no guarantee which
		/// one will be found. This method considers all NaN values to be
		/// equivalent and equal.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>. The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key. Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(float[] a, int fromIndex, int toIndex, float key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(float[] a, int fromIndex, int toIndex, float key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				float midVal = a[mid];

				if (midVal < key)
				{
					low = mid + 1; // Neither val is NaN, thisVal is smaller
				}
				else if (midVal > key)
				{
					high = mid - 1; // Neither val is NaN, thisVal is larger
				}
				else
				{
					int midBits = Float.FloatToIntBits(midVal);
					int keyBits = Float.FloatToIntBits(key);
					if (midBits == keyBits) // Values are equal
					{
						return mid; // Key found
					}
					else if (midBits < keyBits) // (-0.0, 0.0) or (!NaN, NaN)
					{
						low = mid + 1;
					}
					else // (0.0, -0.0) or (NaN, !NaN)
					{
						high = mid - 1;
					}
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array for the specified object using the binary
		/// search algorithm. The array must be sorted into ascending order
		/// according to the
		/// <seealso cref="Comparable natural ordering"/>
		/// of its elements (as by the
		/// <seealso cref="#sort(Object[])"/> method) prior to making this call.
		/// If it is not sorted, the results are undefined.
		/// (If the array contains elements that are not mutually comparable (for
		/// example, strings and integers), it <i>cannot</i> be sorted according
		/// to the natural ordering of its elements, hence results are undefined.)
		/// If the array contains multiple
		/// elements equal to the specified object, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the search key is not comparable to the
		///         elements of the array. </exception>
		public static int BinarySearch(Object[] a, Object key)
		{
			return BinarySearch0(a, 0, a.Length, key);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array for the specified object using the binary
		/// search algorithm.
		/// The range must be sorted into ascending order
		/// according to the
		/// <seealso cref="Comparable natural ordering"/>
		/// of its elements (as by the
		/// <seealso cref="#sort(Object[], int, int)"/> method) prior to making this
		/// call.  If it is not sorted, the results are undefined.
		/// (If the range contains elements that are not mutually comparable (for
		/// example, strings and integers), it <i>cannot</i> be sorted according
		/// to the natural ordering of its elements, hence results are undefined.)
		/// If the range contains multiple
		/// elements equal to the specified object, there is no guarantee which
		/// one will be found.
		/// </summary>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the search key is not comparable to the
		///         elements of the array within the specified range. </exception>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
		public static int BinarySearch(Object[] a, int fromIndex, int toIndex, Object key)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key);
		}

		// Like public version, but without range checks.
		private static int BinarySearch0(Object[] a, int fromIndex, int toIndex, Object key)
		{
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") Comparable midVal = (Comparable)a[mid];
				Comparable midVal = (Comparable)a[mid];
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") int cmp = midVal.compareTo(key);
				int cmp = midVal.CompareTo(key);

				if (cmp < 0)
				{
					low = mid + 1;
				}
				else if (cmp > 0)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		/// <summary>
		/// Searches the specified array for the specified object using the binary
		/// search algorithm.  The array must be sorted into ascending order
		/// according to the specified comparator (as by the
		/// <seealso cref="#sort(Object[], Comparator) sort(T[], Comparator)"/>
		/// method) prior to making this call.  If it is
		/// not sorted, the results are undefined.
		/// If the array contains multiple
		/// elements equal to the specified object, there is no guarantee which one
		/// will be found.
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="a"> the array to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <param name="c"> the comparator by which the array is ordered.  A
		///        <tt>null</tt> value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <returns> index of the search key, if it is contained in the array;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element greater than the key, or <tt>a.length</tt> if all
		///         elements in the array are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the array contains elements that are not
		///         <i>mutually comparable</i> using the specified comparator,
		///         or the search key is not comparable to the
		///         elements of the array using this comparator. </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> int binarySearch(T[] a, T key, Comparator<? base T> c)
		public static int binarySearch<T, T1>(T[] a, T key, Comparator<T1> c)
		{
			return BinarySearch0(a, 0, a.Length, key, c);
		}

		/// <summary>
		/// Searches a range of
		/// the specified array for the specified object using the binary
		/// search algorithm.
		/// The range must be sorted into ascending order
		/// according to the specified comparator (as by the
		/// {@link #sort(Object[], int, int, Comparator)
		/// sort(T[], int, int, Comparator)}
		/// method) prior to making this call.
		/// If it is not sorted, the results are undefined.
		/// If the range contains multiple elements equal to the specified object,
		/// there is no guarantee which one will be found.
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="a"> the array to be searched </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///          searched </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		/// <param name="key"> the value to be searched for </param>
		/// <param name="c"> the comparator by which the array is ordered.  A
		///        <tt>null</tt> value indicates that the elements'
		///        <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <returns> index of the search key, if it is contained in the array
		///         within the specified range;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the array: the index of the first
		///         element in the range greater than the key,
		///         or <tt>toIndex</tt> if all
		///         elements in the range are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the range contains elements that are not
		///         <i>mutually comparable</i> using the specified comparator,
		///         or the search key is not comparable to the
		///         elements in the range using this comparator. </exception>
		/// <exception cref="IllegalArgumentException">
		///         if {@code fromIndex > toIndex} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///         if {@code fromIndex < 0 or toIndex > a.length}
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> int binarySearch(T[] a, int fromIndex, int toIndex, T key, Comparator<? base T> c)
		public static int binarySearch<T, T1>(T[] a, int fromIndex, int toIndex, T key, Comparator<T1> c)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			return BinarySearch0(a, fromIndex, toIndex, key, c);
		}

		// Like public version, but without range checks.
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> int binarySearch0(T[] a, int fromIndex, int toIndex, T key, Comparator<? base T> c)
		private static int binarySearch0<T, T1>(T[] a, int fromIndex, int toIndex, T key, Comparator<T1> c)
		{
			if (c == null)
			{
				return BinarySearch0(a, fromIndex, toIndex, key);
			}
			int low = fromIndex;
			int high = toIndex - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				T midVal = a[mid];
				int cmp = c.Compare(midVal, key);
				if (cmp < 0)
				{
					low = mid + 1;
				}
				else if (cmp > 0)
				{
					high = mid - 1;
				}
				else
				{
					return mid; // key found
				}
			}
			return -(low + 1); // key not found.
		}

		// Equality Testing

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of longs are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(long[] a, long[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of ints are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(int[] a, int[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of shorts are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(short[] a, short[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of chars are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(char[] a, char[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of bytes are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(sbyte[] a, sbyte[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of booleans are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(bool[] a, bool[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (a[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of doubles are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// Two doubles <tt>d1</tt> and <tt>d2</tt> are considered equal if:
		/// <pre>    <tt>new Double(d1).equals(new Double(d2))</tt></pre>
		/// (Unlike the <tt>==</tt> operator, this method considers
		/// <tt>NaN</tt> equals to itself, and 0.0d unequal to -0.0d.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		/// <seealso cref= Double#equals(Object) </seealso>
		public static bool Equals(double[] a, double[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (Double.DoubleToLongBits(a[i]) != Double.DoubleToLongBits(a2[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of floats are
		/// <i>equal</i> to one another.  Two arrays are considered equal if both
		/// arrays contain the same number of elements, and all corresponding pairs
		/// of elements in the two arrays are equal.  In other words, two arrays
		/// are equal if they contain the same elements in the same order.  Also,
		/// two array references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// Two floats <tt>f1</tt> and <tt>f2</tt> are considered equal if:
		/// <pre>    <tt>new Float(f1).equals(new Float(f2))</tt></pre>
		/// (Unlike the <tt>==</tt> operator, this method considers
		/// <tt>NaN</tt> equals to itself, and 0.0f unequal to -0.0f.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		/// <seealso cref= Float#equals(Object) </seealso>
		public static bool Equals(float[] a, float[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				if (Float.FloatToIntBits(a[i]) != Float.FloatToIntBits(a2[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays of Objects are
		/// <i>equal</i> to one another.  The two arrays are considered equal if
		/// both arrays contain the same number of elements, and all corresponding
		/// pairs of elements in the two arrays are equal.  Two objects <tt>e1</tt>
		/// and <tt>e2</tt> are considered <i>equal</i> if <tt>(e1==null ? e2==null
		/// : e1.equals(e2))</tt>.  In other words, the two arrays are equal if
		/// they contain the same elements in the same order.  Also, two array
		/// references are considered equal if both are <tt>null</tt>.<para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		public static bool Equals(Object[] a, Object[] a2)
		{
			if (a == a2)
			{
				return true;
			}
			if (a == null || a2 == null)
			{
				return false;
			}

			int length = a.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				Object o1 = a[i];
				Object o2 = a2[i];
				if (!(o1 == null ? o2 == null : o1.Equals(o2)))
				{
					return false;
				}
			}

			return true;
		}

		// Filling

		/// <summary>
		/// Assigns the specified long value to each element of the specified array
		/// of longs.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(long[] a, long val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified long value to each element of the specified
		/// range of the specified array of longs.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(long[] a, int fromIndex, int toIndex, long val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified int value to each element of the specified array
		/// of ints.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(int[] a, int val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified int value to each element of the specified
		/// range of the specified array of ints.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(int[] a, int fromIndex, int toIndex, int val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified short value to each element of the specified array
		/// of shorts.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(short[] a, short val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified short value to each element of the specified
		/// range of the specified array of shorts.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(short[] a, int fromIndex, int toIndex, short val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified char value to each element of the specified array
		/// of chars.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(char[] a, char val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified char value to each element of the specified
		/// range of the specified array of chars.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(char[] a, int fromIndex, int toIndex, char val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified byte value to each element of the specified array
		/// of bytes.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(sbyte[] a, sbyte val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified byte value to each element of the specified
		/// range of the specified array of bytes.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(sbyte[] a, int fromIndex, int toIndex, sbyte val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified boolean value to each element of the specified
		/// array of booleans.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(bool[] a, bool val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified boolean value to each element of the specified
		/// range of the specified array of booleans.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(bool[] a, int fromIndex, int toIndex, bool val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified double value to each element of the specified
		/// array of doubles.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(double[] a, double val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified double value to each element of the specified
		/// range of the specified array of doubles.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(double[] a, int fromIndex, int toIndex, double val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified float value to each element of the specified array
		/// of floats.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		public static void Fill(float[] a, float val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified float value to each element of the specified
		/// range of the specified array of floats.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		public static void Fill(float[] a, int fromIndex, int toIndex, float val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified Object reference to each element of the specified
		/// array of Objects.
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="ArrayStoreException"> if the specified value is not of a
		///         runtime type that can be stored in the specified array </exception>
		public static void Fill(Object[] a, Object val)
		{
			for (int i = 0, len = a.Length; i < len; i++)
			{
				a[i] = val;
			}
		}

		/// <summary>
		/// Assigns the specified Object reference to each element of the specified
		/// range of the specified array of Objects.  The range to be filled
		/// extends from index <tt>fromIndex</tt>, inclusive, to index
		/// <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		/// range to be filled is empty.)
		/// </summary>
		/// <param name="a"> the array to be filled </param>
		/// <param name="fromIndex"> the index of the first element (inclusive) to be
		///        filled with the specified value </param>
		/// <param name="toIndex"> the index of the last element (exclusive) to be
		///        filled with the specified value </param>
		/// <param name="val"> the value to be stored in all elements of the array </param>
		/// <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		///         <tt>toIndex &gt; a.length</tt> </exception>
		/// <exception cref="ArrayStoreException"> if the specified value is not of a
		///         runtime type that can be stored in the specified array </exception>
		public static void Fill(Object[] a, int fromIndex, int toIndex, Object val)
		{
			RangeCheck(a.Length, fromIndex, toIndex);
			for (int i = fromIndex; i < toIndex; i++)
			{
				a[i] = val;
			}
		}

		// Cloning

		/// <summary>
		/// Copies the specified array, truncating or padding with nulls (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>null</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// The resulting array is of exactly the same class as the original array.
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with nulls
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T[] copyOf(T[] original, int newLength)
		public static T[] copyOf<T>(T[] original, int newLength)
		{
			return (T[]) CopyOf(original, newLength, original.GetType());
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with nulls (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>null</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// The resulting array is of the class <tt>newType</tt>.
		/// </summary>
		/// @param <U> the class of the objects in the original array </param>
		/// @param <T> the class of the objects in the returned array </param>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <param name="newType"> the class of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with nulls
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null </exception>
		/// <exception cref="ArrayStoreException"> if an element copied from
		///     <tt>original</tt> is not of a runtime type that can be stored in
		///     an array of class <tt>newType</tt>
		/// @since 1.6 </exception>
		public static T[] copyOf<T, U>(U[] original, int newLength, Class newType)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T[] copy = ((Object)newType == (Object)Object[].class) ? (T[]) new Object[newLength] : (T[]) Array.newInstance(newType.getComponentType(), newLength);
			T[] copy = ((Object)newType == typeof((Object)Object[])) ? (T[]) new Object[newLength] : (T[]) Array.newInstance(newType.ComponentType, newLength);
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>(byte)0</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static sbyte[] CopyOf(sbyte[] original, int newLength)
		{
			sbyte[] copy = new sbyte[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>(short)0</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static short[] CopyOf(short[] original, int newLength)
		{
			short[] copy = new short[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>0</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static int[] CopyOf(int[] original, int newLength)
		{
			int[] copy = new int[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>0L</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static long[] CopyOf(long[] original, int newLength)
		{
			long[] copy = new long[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with null characters (if necessary)
		/// so the copy has the specified length.  For all indices that are valid
		/// in both the original array and the copy, the two arrays will contain
		/// identical values.  For any indices that are valid in the copy but not
		/// the original, the copy will contain <tt>'\\u000'</tt>.  Such indices
		/// will exist if and only if the specified length is greater than that of
		/// the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with null characters
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static char[] CopyOf(char[] original, int newLength)
		{
			char[] copy = new char[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>0f</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static float[] CopyOf(float[] original, int newLength)
		{
			float[] copy = new float[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with zeros (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>0d</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with zeros
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static double[] CopyOf(double[] original, int newLength)
		{
			double[] copy = new double[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified array, truncating or padding with <tt>false</tt> (if necessary)
		/// so the copy has the specified length.  For all indices that are
		/// valid in both the original array and the copy, the two arrays will
		/// contain identical values.  For any indices that are valid in the
		/// copy but not the original, the copy will contain <tt>false</tt>.
		/// Such indices will exist if and only if the specified length
		/// is greater than that of the original array.
		/// </summary>
		/// <param name="original"> the array to be copied </param>
		/// <param name="newLength"> the length of the copy to be returned </param>
		/// <returns> a copy of the original array, truncated or padded with false elements
		///     to obtain the specified length </returns>
		/// <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static bool[] CopyOf(bool[] original, int newLength)
		{
			bool[] copy = new bool[newLength];
			System.Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>null</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// <para>
		/// The resulting array is of exactly the same class as the original array.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with nulls to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T[] copyOfRange(T[] original, int from, int to)
		public static T[] copyOfRange<T>(T[] original, int from, int to)
		{
			return CopyOfRange(original, from, to, (Class) original.GetType());
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>null</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// The resulting array is of the class <tt>newType</tt>.
		/// </summary>
		/// @param <U> the class of the objects in the original array </param>
		/// @param <T> the class of the objects in the returned array </param>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <param name="newType"> the class of the copy to be returned </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with nulls to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null </exception>
		/// <exception cref="ArrayStoreException"> if an element copied from
		///     <tt>original</tt> is not of a runtime type that can be stored in
		///     an array of class <tt>newType</tt>.
		/// @since 1.6 </exception>
		public static T[] copyOfRange<T, U>(U[] original, int from, int to, Class newType)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T[] copy = ((Object)newType == (Object)Object[].class) ? (T[]) new Object[newLength] : (T[]) Array.newInstance(newType.getComponentType(), newLength);
			T[] copy = ((Object)newType == typeof((Object)Object[])) ? (T[]) new Object[newLength] : (T[]) Array.newInstance(newType.ComponentType, newLength);
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>(byte)0</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static sbyte[] CopyOfRange(sbyte[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			sbyte[] copy = new sbyte[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>(short)0</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static short[] CopyOfRange(short[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			short[] copy = new short[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>0</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static int[] CopyOfRange(int[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			int[] copy = new int[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>0L</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static long[] CopyOfRange(long[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			long[] copy = new long[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>'\\u000'</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with null characters to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static char[] CopyOfRange(char[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			char[] copy = new char[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>0f</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static float[] CopyOfRange(float[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			float[] copy = new float[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>0d</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with zeros to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static double[] CopyOfRange(double[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			double[] copy = new double[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		/// <summary>
		/// Copies the specified range of the specified array into a new array.
		/// The initial index of the range (<tt>from</tt>) must lie between zero
		/// and <tt>original.length</tt>, inclusive.  The value at
		/// <tt>original[from]</tt> is placed into the initial element of the copy
		/// (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		/// Values from subsequent elements in the original array are placed into
		/// subsequent elements in the copy.  The final index of the range
		/// (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		/// may be greater than <tt>original.length</tt>, in which case
		/// <tt>false</tt> is placed in all elements of the copy whose index is
		/// greater than or equal to <tt>original.length - from</tt>.  The length
		/// of the returned array will be <tt>to - from</tt>.
		/// </summary>
		/// <param name="original"> the array from which a range is to be copied </param>
		/// <param name="from"> the initial index of the range to be copied, inclusive </param>
		/// <param name="to"> the final index of the range to be copied, exclusive.
		///     (This index may lie outside the array.) </param>
		/// <returns> a new array containing the specified range from the original array,
		///     truncated or padded with false elements to obtain the required length </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		///     or {@code from > original.length} </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		/// <exception cref="NullPointerException"> if <tt>original</tt> is null
		/// @since 1.6 </exception>
		public static bool[] CopyOfRange(bool[] original, int from, int to)
		{
			int newLength = to - from;
			if (newLength < 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			bool[] copy = new bool[newLength];
			System.Array.Copy(original, from, copy, 0, System.Math.Min(original.Length - from, newLength));
			return copy;
		}

		// Misc

		/// <summary>
		/// Returns a fixed-size list backed by the specified array.  (Changes to
		/// the returned list "write through" to the array.)  This method acts
		/// as bridge between array-based and collection-based APIs, in
		/// combination with <seealso cref="Collection#toArray"/>.  The returned list is
		/// serializable and implements <seealso cref="RandomAccess"/>.
		/// 
		/// <para>This method also provides a convenient way to create a fixed-size
		/// list initialized to contain several elements:
		/// <pre>
		///     List&lt;String&gt; stooges = Arrays.asList("Larry", "Moe", "Curly");
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects in the array </param>
		/// <param name="a"> the array by which the list will be backed </param>
		/// <returns> a list view of the specified array </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SafeVarargs @SuppressWarnings("varargs") public static <T> List<T> asList(T... a)
		public static List<T> asList<T>(params T[] a)
		{
			return new List<>(a);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		[Serializable]
		private class List<E> : AbstractList<E>, RandomAccess
		{
			internal const long SerialVersionUID = -2764017481108945198L;
			internal readonly E[] List_Fields;

			internal ArrayList(E[] array)
			{
				List_Fields.a = Objects.RequireNonNull(array);
			}

			public override int Size()
			{
				return List_Fields.a.Length;
			}

			public override Object[] ToArray()
			{
				return List_Fields.a.clone();
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <T> T[] toArray(T[] List_Fields.a)
			public override T[] toArray<T>(T[] List_Fields)
			{
				int size = Size();
				if (List_Fields.a.Length < size)
				{
					return Arrays.CopyOf(this.a, size, (Class) List_Fields.a.GetType());
				}
				System.Array.Copy(this.a, 0, List_Fields.a, 0, size);
				if (List_Fields.a.Length > size)
				{
					List_Fields.a[size] = null;
				}
				return List_Fields.a;
			}

			public override E Get(int index)
			{
				return List_Fields.a[index];
			}

			public override E Set(int index, E element)
			{
				E oldValue = List_Fields.a[index];
				List_Fields.a[index] = element;
				return oldValue;
			}

			public override int IndexOf(Object o)
			{
				E[] List_Fields.a = this.a;
				if (o == null)
				{
					for (int List_Fields.i = 0; List_Fields.i < List_Fields.a.Length; List_Fields.i++)
					{
						if (List_Fields.a[List_Fields.i] == null)
						{
							return List_Fields.i;
						}
					}
				}
				else
				{
					for (int List_Fields.i = 0; List_Fields.i < List_Fields.a.Length; List_Fields.i++)
					{
						if (o.Equals(List_Fields.a[List_Fields.i]))
						{
							return List_Fields.i;
						}
					}
				}
				return -1;
			}

			public override bool Contains(Object o)
			{
				return IndexOf(o) != -1;
			}

			public override Spliterator<E> Spliterator()
			{
				return Spliterators.Spliterator(List_Fields.a, Spliterator_Fields.ORDERED);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
			public override void forEach<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);
				foreach (E e in List_Fields.a)
				{
					action.Accept(e);
				}
			}

			public override void ReplaceAll(UnaryOperator<E> @operator)
			{
				Objects.RequireNonNull(@operator);
				E[] List_Fields.a = this.a;
				for (int List_Fields.i = 0; List_Fields.i < List_Fields.a.Length; List_Fields.i++)
				{
					List_Fields.a[List_Fields.i] = @operator.Apply(List_Fields.a[List_Fields.i]);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
			public override void sort<T1>(Comparator<T1> c)
			{
				System.Array.Sort(List_Fields.a, c);
			}
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>long</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Long"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(long[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (long element in a)
			{
				int elementHash = (int)(element ^ ((long)((ulong)element >> 32)));
				result = 31 * result + elementHash;
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two non-null <tt>int</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Integer"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(int[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (int element in a)
			{
				result = 31 * result + element;
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>short</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Short"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(short[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (short element in a)
			{
				result = 31 * result + element;
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>char</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Character"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(char[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (char element in a)
			{
				result = 31 * result + element;
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>byte</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Byte"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(sbyte[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (sbyte element in a)
			{
				result = 31 * result + element;
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>boolean</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Boolean"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(bool[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (bool element in a)
			{
				result = 31 * result + (element ? 1231 : 1237);
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>float</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Float"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(float[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (float element in a)
			{
				result = 31 * result + Float.FloatToIntBits(element);
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.
		/// For any two <tt>double</tt> arrays <tt>a</tt> and <tt>b</tt>
		/// such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// <para>The value returned by this method is the same value that would be
		/// obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		/// method on a <seealso cref="List"/> containing a sequence of <seealso cref="Double"/>
		/// instances representing the elements of <tt>a</tt> in the same order.
		/// If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose hash value to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt>
		/// @since 1.5 </returns>
		public static int HashCode(double[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;
			foreach (double element in a)
			{
				long bits = Double.DoubleToLongBits(element);
				result = 31 * result + (int)(bits ^ ((long)((ulong)bits >> 32)));
			}
			return result;
		}

		/// <summary>
		/// Returns a hash code based on the contents of the specified array.  If
		/// the array contains other arrays as elements, the hash code is based on
		/// their identities rather than their contents.  It is therefore
		/// acceptable to invoke this method on an array that contains itself as an
		/// element,  either directly or indirectly through one or more levels of
		/// arrays.
		/// 
		/// <para>For any two arrays <tt>a</tt> and <tt>b</tt> such that
		/// <tt>Arrays.equals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		/// 
		/// </para>
		/// <para>The value returned by this method is equal to the value that would
		/// be returned by <tt>Arrays.asList(a).hashCode()</tt>, unless <tt>a</tt>
		/// is <tt>null</tt>, in which case <tt>0</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose content-based hash code to compute </param>
		/// <returns> a content-based hash code for <tt>a</tt> </returns>
		/// <seealso cref= #deepHashCode(Object[])
		/// @since 1.5 </seealso>
		public static int HashCode(Object[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;

			foreach (Object element in a)
			{
				result = 31 * result + (element == null ? 0 : element.HashCode());
			}

			return result;
		}

		/// <summary>
		/// Returns a hash code based on the "deep contents" of the specified
		/// array.  If the array contains other arrays as elements, the
		/// hash code is based on their contents and so on, ad infinitum.
		/// It is therefore unacceptable to invoke this method on an array that
		/// contains itself as an element, either directly or indirectly through
		/// one or more levels of arrays.  The behavior of such an invocation is
		/// undefined.
		/// 
		/// <para>For any two arrays <tt>a</tt> and <tt>b</tt> such that
		/// <tt>Arrays.deepEquals(a, b)</tt>, it is also the case that
		/// <tt>Arrays.deepHashCode(a) == Arrays.deepHashCode(b)</tt>.
		/// 
		/// </para>
		/// <para>The computation of the value returned by this method is similar to
		/// that of the value returned by <seealso cref="List#hashCode()"/> on a list
		/// containing the same elements as <tt>a</tt> in the same order, with one
		/// difference: If an element <tt>e</tt> of <tt>a</tt> is itself an array,
		/// its hash code is computed not by calling <tt>e.hashCode()</tt>, but as
		/// by calling the appropriate overloading of <tt>Arrays.hashCode(e)</tt>
		/// if <tt>e</tt> is an array of a primitive type, or as by calling
		/// <tt>Arrays.deepHashCode(e)</tt> recursively if <tt>e</tt> is an array
		/// of a reference type.  If <tt>a</tt> is <tt>null</tt>, this method
		/// returns 0.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose deep-content-based hash code to compute </param>
		/// <returns> a deep-content-based hash code for <tt>a</tt> </returns>
		/// <seealso cref= #hashCode(Object[])
		/// @since 1.5 </seealso>
		public static int DeepHashCode(Object[] a)
		{
			if (a == null)
			{
				return 0;
			}

			int result = 1;

			foreach (Object element in a)
			{
				int elementHash = 0;
				if (element is Object[])
				{
					elementHash = DeepHashCode((Object[]) element);
				}
				else if (element is sbyte[])
				{
					elementHash = HashCode((sbyte[]) element);
				}
				else if (element is short[])
				{
					elementHash = HashCode((short[]) element);
				}
				else if (element is int[])
				{
					elementHash = HashCode((int[]) element);
				}
				else if (element is long[])
				{
					elementHash = HashCode((long[]) element);
				}
				else if (element is char[])
				{
					elementHash = HashCode((char[]) element);
				}
				else if (element is float[])
				{
					elementHash = HashCode((float[]) element);
				}
				else if (element is double[])
				{
					elementHash = HashCode((double[]) element);
				}
				else if (element is bool[])
				{
					elementHash = HashCode((bool[]) element);
				}
				else if (element != null)
				{
					elementHash = element.HashCode();
				}

				result = 31 * result + elementHash;
			}

			return result;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the two specified arrays are <i>deeply
		/// equal</i> to one another.  Unlike the <seealso cref="#equals(Object[],Object[])"/>
		/// method, this method is appropriate for use with nested arrays of
		/// arbitrary depth.
		/// 
		/// <para>Two array references are considered deeply equal if both
		/// are <tt>null</tt>, or if they refer to arrays that contain the same
		/// number of elements and all corresponding pairs of elements in the two
		/// arrays are deeply equal.
		/// 
		/// </para>
		/// <para>Two possibly <tt>null</tt> elements <tt>e1</tt> and <tt>e2</tt> are
		/// deeply equal if any of the following conditions hold:
		/// <ul>
		///    <li> <tt>e1</tt> and <tt>e2</tt> are both arrays of object reference
		///         types, and <tt>Arrays.deepEquals(e1, e2) would return true</tt>
		///    <li> <tt>e1</tt> and <tt>e2</tt> are arrays of the same primitive
		///         type, and the appropriate overloading of
		///         <tt>Arrays.equals(e1, e2)</tt> would return true.
		///    <li> <tt>e1 == e2</tt>
		///    <li> <tt>e1.equals(e2)</tt> would return true.
		/// </ul>
		/// Note that this definition permits <tt>null</tt> elements at any depth.
		/// 
		/// </para>
		/// <para>If either of the specified arrays contain themselves as elements
		/// either directly or indirectly through one or more levels of arrays,
		/// the behavior of this method is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a1"> one array to be tested for equality </param>
		/// <param name="a2"> the other array to be tested for equality </param>
		/// <returns> <tt>true</tt> if the two arrays are equal </returns>
		/// <seealso cref= #equals(Object[],Object[]) </seealso>
		/// <seealso cref= Objects#deepEquals(Object, Object)
		/// @since 1.5 </seealso>
		public static bool DeepEquals(Object[] a1, Object[] a2)
		{
			if (a1 == a2)
			{
				return true;
			}
			if (a1 == null || a2 == null)
			{
				return false;
			}
			int length = a1.Length;
			if (a2.Length != length)
			{
				return false;
			}

			for (int i = 0; i < length; i++)
			{
				Object e1 = a1[i];
				Object e2 = a2[i];

				if (e1 == e2)
				{
					continue;
				}
				if (e1 == null)
				{
					return false;
				}

				// Figure out whether the two elements are equal
				bool eq = DeepEquals0(e1, e2);

				if (!eq)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool DeepEquals0(Object e1, Object e2)
		{
			Debug.Assert(e1 != null);
			bool eq;
			if (e1 is Object[] && e2 is Object[])
			{
				eq = DeepEquals((Object[]) e1, (Object[]) e2);
			}
			else if (e1 is sbyte[] && e2 is sbyte[])
			{
				eq = Equals((sbyte[]) e1, (sbyte[]) e2);
			}
			else if (e1 is short[] && e2 is short[])
			{
				eq = Equals((short[]) e1, (short[]) e2);
			}
			else if (e1 is int[] && e2 is int[])
			{
				eq = Equals((int[]) e1, (int[]) e2);
			}
			else if (e1 is long[] && e2 is long[])
			{
				eq = Equals((long[]) e1, (long[]) e2);
			}
			else if (e1 is char[] && e2 is char[])
			{
				eq = Equals((char[]) e1, (char[]) e2);
			}
			else if (e1 is float[] && e2 is float[])
			{
				eq = Equals((float[]) e1, (float[]) e2);
			}
			else if (e1 is double[] && e2 is double[])
			{
				eq = Equals((double[]) e1, (double[]) e2);
			}
			else if (e1 is bool[] && e2 is bool[])
			{
				eq = Equals((bool[]) e1, (bool[]) e2);
			}
			else
			{
				eq = e1.Equals(e2);
			}
			return eq;
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(long)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		/// is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(long[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(int)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt> is
		/// <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(int[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(short)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		/// is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(short[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(char)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		/// is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(char[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements
		/// are separated by the characters <tt>", "</tt> (a comma followed
		/// by a space).  Elements are converted to strings as by
		/// <tt>String.valueOf(byte)</tt>.  Returns <tt>"null"</tt> if
		/// <tt>a</tt> is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(sbyte[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(boolean)</tt>.  Returns <tt>"null"</tt> if
		/// <tt>a</tt> is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(bool[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(float)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		/// is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(float[] a)
		{
			if (a == null)
			{
				return "null";
			}

			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// The string representation consists of a list of the array's elements,
		/// enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		/// separated by the characters <tt>", "</tt> (a comma followed by a
		/// space).  Elements are converted to strings as by
		/// <tt>String.valueOf(double)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		/// is <tt>null</tt>.
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt>
		/// @since 1.5 </returns>
		public static String ToString(double[] a)
		{
			if (a == null)
			{
				return "null";
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(a[i]);
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the contents of the specified array.
		/// If the array contains other arrays as elements, they are converted to
		/// strings by the <seealso cref="Object#toString"/> method inherited from
		/// <tt>Object</tt>, which describes their <i>identities</i> rather than
		/// their contents.
		/// 
		/// <para>The value returned by this method is equal to the value that would
		/// be returned by <tt>Arrays.asList(a).toString()</tt>, unless <tt>a</tt>
		/// is <tt>null</tt>, in which case <tt>"null"</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt> </returns>
		/// <seealso cref= #deepToString(Object[])
		/// @since 1.5 </seealso>
		public static String ToString(Object[] a)
		{
			if (a == null)
			{
				return "null";
			}

			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(Convert.ToString(a[i]));
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(", ");
			}
		}

		/// <summary>
		/// Returns a string representation of the "deep contents" of the specified
		/// array.  If the array contains other arrays as elements, the string
		/// representation contains their contents and so on.  This method is
		/// designed for converting multidimensional arrays to strings.
		/// 
		/// <para>The string representation consists of a list of the array's
		/// elements, enclosed in square brackets (<tt>"[]"</tt>).  Adjacent
		/// elements are separated by the characters <tt>", "</tt> (a comma
		/// followed by a space).  Elements are converted to strings as by
		/// <tt>String.valueOf(Object)</tt>, unless they are themselves
		/// arrays.
		/// 
		/// </para>
		/// <para>If an element <tt>e</tt> is an array of a primitive type, it is
		/// converted to a string as by invoking the appropriate overloading of
		/// <tt>Arrays.toString(e)</tt>.  If an element <tt>e</tt> is an array of a
		/// reference type, it is converted to a string as by invoking
		/// this method recursively.
		/// 
		/// </para>
		/// <para>To avoid infinite recursion, if the specified array contains itself
		/// as an element, or contains an indirect reference to itself through one
		/// or more levels of arrays, the self-reference is converted to the string
		/// <tt>"[...]"</tt>.  For example, an array containing only a reference
		/// to itself would be rendered as <tt>"[[...]]"</tt>.
		/// 
		/// </para>
		/// <para>This method returns <tt>"null"</tt> if the specified array
		/// is <tt>null</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array whose string representation to return </param>
		/// <returns> a string representation of <tt>a</tt> </returns>
		/// <seealso cref= #toString(Object[])
		/// @since 1.5 </seealso>
		public static String DeepToString(Object[] a)
		{
			if (a == null)
			{
				return "null";
			}

			int bufLen = 20 * a.Length;
			if (a.Length != 0 && bufLen <= 0)
			{
				bufLen = Integer.MaxValue;
			}
			StringBuilder buf = new StringBuilder(bufLen);
			DeepToString(a, buf, new HashSet<Object[]>());
			return buf.ToString();
		}

		private static void DeepToString(Object[] a, StringBuilder buf, Set<Object[]> dejaVu)
		{
			if (a == null)
			{
				buf.Append("null");
				return;
			}
			int iMax = a.Length - 1;
			if (iMax == -1)
			{
				buf.Append("[]");
				return;
			}

			dejaVu.Add(a);
			buf.Append('[');
			for (int i = 0; ; i++)
			{

				Object element = a[i];
				if (element == null)
				{
					buf.Append("null");
				}
				else
				{
					Class eClass = element.GetType();

					if (eClass.Array)
					{
						if (eClass == typeof(sbyte[]))
						{
							buf.Append(ToString((sbyte[]) element));
						}
						else if (eClass == typeof(short[]))
						{
							buf.Append(ToString((short[]) element));
						}
						else if (eClass == typeof(int[]))
						{
							buf.Append(ToString((int[]) element));
						}
						else if (eClass == typeof(long[]))
						{
							buf.Append(ToString((long[]) element));
						}
						else if (eClass == typeof(char[]))
						{
							buf.Append(ToString((char[]) element));
						}
						else if (eClass == typeof(float[]))
						{
							buf.Append(ToString((float[]) element));
						}
						else if (eClass == typeof(double[]))
						{
							buf.Append(ToString((double[]) element));
						}
						else if (eClass == typeof(bool[]))
						{
							buf.Append(ToString((bool[]) element));
						}
						else // element is an array of object references
						{
							if (dejaVu.Contains(element))
							{
								buf.Append("[...]");
							}
							else
							{
								DeepToString((Object[])element, buf, dejaVu);
							}
						}
					} // element is non-null and not an array
					else
					{
						buf.Append(element.ToString());
					}
				}
				if (i == iMax)
				{
					break;
				}
				buf.Append(", ");
			}
			buf.Append(']');
			dejaVu.Remove(a);
		}


		/// <summary>
		/// Set all elements of the specified array, using the provided
		/// generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, it is relayed to
		/// the caller and the array is left in an indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements of the array </param>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void setAll<T, T1>(T[] array, IntFunction<T1> generator) where T1 : T
		{
			Objects.RequireNonNull(generator);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = generator.Apply(i);
			}
		}

		/// <summary>
		/// Set all elements of the specified array, in parallel, using the
		/// provided generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, an unchecked exception
		/// is thrown from {@code parallelSetAll} and the array is left in an
		/// indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements of the array </param>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void parallelSetAll<T, T1>(T[] array, IntFunction<T1> generator) where T1 : T
		{
			Objects.RequireNonNull(generator);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			IntStream.range(0, array.Length).parallel().forEach(i =>
			{
				array[i] = generator.Apply(i);
			});
		}

		/// <summary>
		/// Set all elements of the specified array, using the provided
		/// generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, it is relayed to
		/// the caller and the array is left in an indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void SetAll(int[] array, IntUnaryOperator generator)
		{
			Objects.RequireNonNull(generator);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = generator.ApplyAsInt(i);
			}
		}

		/// <summary>
		/// Set all elements of the specified array, in parallel, using the
		/// provided generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, an unchecked exception
		/// is thrown from {@code parallelSetAll} and the array is left in an
		/// indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		/// value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void ParallelSetAll(int[] array, IntUnaryOperator generator)
		{
			Objects.RequireNonNull(generator);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			IntStream.range(0, array.Length).parallel().forEach(i =>
			{
				array[i] = generator.ApplyAsInt(i);
			});
		}

		/// <summary>
		/// Set all elements of the specified array, using the provided
		/// generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, it is relayed to
		/// the caller and the array is left in an indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void SetAll(long[] array, IntToLongFunction generator)
		{
			Objects.RequireNonNull(generator);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = generator.ApplyAsLong(i);
			}
		}

		/// <summary>
		/// Set all elements of the specified array, in parallel, using the
		/// provided generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, an unchecked exception
		/// is thrown from {@code parallelSetAll} and the array is left in an
		/// indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void ParallelSetAll(long[] array, IntToLongFunction generator)
		{
			Objects.RequireNonNull(generator);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			IntStream.range(0, array.Length).parallel().forEach(i =>
			{
				array[i] = generator.ApplyAsLong(i);
			});
		}

		/// <summary>
		/// Set all elements of the specified array, using the provided
		/// generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, it is relayed to
		/// the caller and the array is left in an indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void SetAll(double[] array, IntToDoubleFunction generator)
		{
			Objects.RequireNonNull(generator);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = generator.ApplyAsDouble(i);
			}
		}

		/// <summary>
		/// Set all elements of the specified array, in parallel, using the
		/// provided generator function to compute each element.
		/// 
		/// <para>If the generator function throws an exception, an unchecked exception
		/// is thrown from {@code parallelSetAll} and the array is left in an
		/// indeterminate state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> array to be initialized </param>
		/// <param name="generator"> a function accepting an index and producing the desired
		///        value for that position </param>
		/// <exception cref="NullPointerException"> if the generator is null
		/// @since 1.8 </exception>
		public static void ParallelSetAll(double[] array, IntToDoubleFunction generator)
		{
			Objects.RequireNonNull(generator);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			IntStream.range(0, array.Length).parallel().forEach(i =>
			{
				array[i] = generator.ApplyAsDouble(i);
			});
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> covering all of the specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements </param>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> a spliterator for the array elements
		/// @since 1.8 </returns>
		public static Spliterator<T> spliterator<T>(T[] array)
		{
			return Spliterators.Spliterator(array, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> covering the specified range of the
		/// specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements </param>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a spliterator for the array elements </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static Spliterator<T> spliterator<T>(T[] array, int startInclusive, int endExclusive)
		{
			return Spliterators.Spliterator(array, startInclusive, endExclusive, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfInt"/> covering all of the specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> a spliterator for the array elements
		/// @since 1.8 </returns>
		public static Spliterator_OfInt Spliterator(int[] array)
		{
			return Spliterators.Spliterator(array, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfInt"/> covering the specified range of the
		/// specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a spliterator for the array elements </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static Spliterator_OfInt Spliterator(int[] array, int startInclusive, int endExclusive)
		{
			return Spliterators.Spliterator(array, startInclusive, endExclusive, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfLong"/> covering all of the specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> the spliterator for the array elements
		/// @since 1.8 </returns>
		public static Spliterator_OfLong Spliterator(long[] array)
		{
			return Spliterators.Spliterator(array, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfLong"/> covering the specified range of the
		/// specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a spliterator for the array elements </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static Spliterator_OfLong Spliterator(long[] array, int startInclusive, int endExclusive)
		{
			return Spliterators.Spliterator(array, startInclusive, endExclusive, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfDouble"/> covering all of the specified
		/// array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> a spliterator for the array elements
		/// @since 1.8 </returns>
		public static Spliterator_OfDouble Spliterator(double[] array)
		{
			return Spliterators.Spliterator(array, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator.OfDouble"/> covering the specified range of
		/// the specified array.
		/// 
		/// <para>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		/// <seealso cref="Spliterator#IMMUTABLE"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a spliterator for the array elements </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static Spliterator_OfDouble Spliterator(double[] array, int startInclusive, int endExclusive)
		{
			return Spliterators.Spliterator(array, startInclusive, endExclusive, Spliterator_Fields.ORDERED | Spliterator_Fields.IMMUTABLE);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="Stream"/> with the specified array as its
		/// source.
		/// </summary>
		/// @param <T> The type of the array elements </param>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <returns> a {@code Stream} for the array
		/// @since 1.8 </returns>
		public static Stream<T> stream<T>(T[] array)
		{
			return Stream(array, 0, array.Length);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="Stream"/> with the specified range of the
		/// specified array as its source.
		/// </summary>
		/// @param <T> the type of the array elements </param>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a {@code Stream} for the array range </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static Stream<T> stream<T>(T[] array, int startInclusive, int endExclusive)
		{
			return StreamSupport.Stream(Spliterator(array, startInclusive, endExclusive), false);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="IntStream"/> with the specified array as its
		/// source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> an {@code IntStream} for the array
		/// @since 1.8 </returns>
		public static IntStream Stream(int[] array)
		{
			return Stream(array, 0, array.Length);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="IntStream"/> with the specified range of the
		/// specified array as its source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> an {@code IntStream} for the array range </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static IntStream Stream(int[] array, int startInclusive, int endExclusive)
		{
			return StreamSupport.IntStream(Spliterator(array, startInclusive, endExclusive), false);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="LongStream"/> with the specified array as its
		/// source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> a {@code LongStream} for the array
		/// @since 1.8 </returns>
		public static LongStream Stream(long[] array)
		{
			return Stream(array, 0, array.Length);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="LongStream"/> with the specified range of the
		/// specified array as its source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a {@code LongStream} for the array range </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static LongStream Stream(long[] array, int startInclusive, int endExclusive)
		{
			return StreamSupport.LongStream(Spliterator(array, startInclusive, endExclusive), false);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="DoubleStream"/> with the specified array as its
		/// source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <returns> a {@code DoubleStream} for the array
		/// @since 1.8 </returns>
		public static DoubleStream Stream(double[] array)
		{
			return Stream(array, 0, array.Length);
		}

		/// <summary>
		/// Returns a sequential <seealso cref="DoubleStream"/> with the specified range of the
		/// specified array as its source.
		/// </summary>
		/// <param name="array"> the array, assumed to be unmodified during use </param>
		/// <param name="startInclusive"> the first index to cover, inclusive </param>
		/// <param name="endExclusive"> index immediately past the last index to cover </param>
		/// <returns> a {@code DoubleStream} for the array range </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		///         negative, {@code endExclusive} is less than
		///         {@code startInclusive}, or {@code endExclusive} is greater than
		///         the array size
		/// @since 1.8 </exception>
		public static DoubleStream Stream(double[] array, int startInclusive, int endExclusive)
		{
			return StreamSupport.DoubleStream(Spliterator(array, startInclusive, endExclusive), false);
		}
	}

}