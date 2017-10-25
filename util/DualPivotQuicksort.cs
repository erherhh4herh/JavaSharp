/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements the Dual-Pivot Quicksort algorithm by
	/// Vladimir Yaroslavskiy, Jon Bentley, and Josh Bloch. The algorithm
	/// offers O(n log(n)) performance on many data sets that cause other
	/// quicksorts to degrade to quadratic performance, and is typically
	/// faster than traditional (one-pivot) Quicksort implementations.
	/// 
	/// All exposed methods are package-private, designed to be invoked
	/// from public methods (in class Arrays) after performing any
	/// necessary array bounds checks and expanding parameters into the
	/// required forms.
	/// 
	/// @author Vladimir Yaroslavskiy
	/// @author Jon Bentley
	/// @author Josh Bloch
	/// 
	/// @version 2011.02.11 m765.827.12i:5\7pm
	/// @since 1.7
	/// </summary>
	internal sealed class DualPivotQuicksort
	{

		/// <summary>
		/// Prevents instantiation.
		/// </summary>
		private DualPivotQuicksort()
		{
		}

		/*
		 * Tuning parameters.
		 */

		/// <summary>
		/// The maximum number of runs in merge sort.
		/// </summary>
		private const int MAX_RUN_COUNT = 67;

		/// <summary>
		/// The maximum length of run in merge sort.
		/// </summary>
		private const int MAX_RUN_LENGTH = 33;

		/// <summary>
		/// If the length of an array to be sorted is less than this
		/// constant, Quicksort is used in preference to merge sort.
		/// </summary>
		private const int QUICKSORT_THRESHOLD = 286;

		/// <summary>
		/// If the length of an array to be sorted is less than this
		/// constant, insertion sort is used in preference to Quicksort.
		/// </summary>
		private const int INSERTION_SORT_THRESHOLD = 47;

		/// <summary>
		/// If the length of a byte array to be sorted is greater than this
		/// constant, counting sort is used in preference to insertion sort.
		/// </summary>
		private const int COUNTING_SORT_THRESHOLD_FOR_BYTE = 29;

		/// <summary>
		/// If the length of a short or char array to be sorted is greater
		/// than this constant, counting sort is used in preference to Quicksort.
		/// </summary>
		private const int COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR = 3200;

		/*
		 * Sorting methods for seven primitive types.
		 */

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(int[] a, int left, int right, int[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						int t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			int[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new int[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				int[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(int[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						int ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						int a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					int last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				int t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				int t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				int t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				int t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				int pivot1 = a[e2];
				int pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					int ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						int ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = pivot1;
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				int pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					int ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = pivot;
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(long[] a, int left, int right, long[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						long t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			long[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new long[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				long[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(long[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						long ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						long a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					long last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				long t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				long t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				long t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				long t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				long pivot1 = a[e2];
				long pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					long ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						long ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = pivot1;
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				long pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					long ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = pivot;
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(short[] a, int left, int right, short[] work, int workBase, int workLen)
		{
			// Use counting sort on large arrays
			if (right - left > COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR)
			{
				int[] count = new int[NUM_SHORT_VALUES];

				for (int i = left - 1; ++i <= right; count[a[i] - Short.MinValue]++)
				{
					;
				}
				for (int i = NUM_SHORT_VALUES, k = right + 1; k > left;)
				{
					while (count[--i] == 0);
					short value = (short)(i + Short.MinValue);
					int s = count[i];

					do
					{
						a[--k] = value;
					} while (--s > 0);
				}
			} // Use Dual-Pivot Quicksort on small arrays
			else
			{
				DoSort(a, left, right, work, workBase, workLen);
			}
		}

		/// <summary>
		/// The number of distinct short values. </summary>
		private static readonly int NUM_SHORT_VALUES = 1 << 16;

		/// <summary>
		/// Sorts the specified range of the array.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		private static void DoSort(short[] a, int left, int right, short[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						short t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			short[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new short[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				short[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(short[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						short ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						short a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					short last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				short t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				short t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				short t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				short t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				short pivot1 = a[e2];
				short pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					short ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						short ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = pivot1;
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				short pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					short ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = pivot;
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(char[] a, int left, int right, char[] work, int workBase, int workLen)
		{
			// Use counting sort on large arrays
			if (right - left > COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR)
			{
				int[] count = new int[NUM_CHAR_VALUES];

				for (int i = left - 1; ++i <= right; count[a[i]]++)
				{
					;
				}
				for (int i = NUM_CHAR_VALUES, k = right + 1; k > left;)
				{
					while (count[--i] == 0);
					char value = (char) i;
					int s = count[i];

					do
					{
						a[--k] = value;
					} while (--s > 0);
				}
			} // Use Dual-Pivot Quicksort on small arrays
			else
			{
				DoSort(a, left, right, work, workBase, workLen);
			}
		}

		/// <summary>
		/// The number of distinct char values. </summary>
		private static readonly int NUM_CHAR_VALUES = 1 << 16;

		/// <summary>
		/// Sorts the specified range of the array.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		private static void DoSort(char[] a, int left, int right, char[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						char t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			char[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new char[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				char[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(char[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						char ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						char a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					char last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				char t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				char t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				char t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				char t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				char pivot1 = a[e2];
				char pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					char ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						char ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = pivot1;
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				char pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					char ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = pivot;
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}

		/// <summary>
		/// The number of distinct byte values. </summary>
		private static readonly int NUM_BYTE_VALUES = 1 << 8;

		/// <summary>
		/// Sorts the specified range of the array.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		internal static void Sort(sbyte[] a, int left, int right)
		{
			// Use counting sort on large arrays
			if (right - left > COUNTING_SORT_THRESHOLD_FOR_BYTE)
			{
				int[] count = new int[NUM_BYTE_VALUES];

				for (int i = left - 1; ++i <= right; count[a[i] - Byte.MinValue]++)
				{
					;
				}
				for (int i = NUM_BYTE_VALUES, k = right + 1; k > left;)
				{
					while (count[--i] == 0);
					sbyte value = (sbyte)(i + Byte.MinValue);
					int s = count[i];

					do
					{
						a[--k] = value;
					} while (--s > 0);
				}
			} // Use insertion sort on small arrays
			else
			{
				for (int i = left, j = i; i < right; j = ++i)
				{
					sbyte ai = a[i + 1];
					while (ai < a[j])
					{
						a[j + 1] = a[j];
						if (j-- == left)
						{
							break;
						}
					}
					a[j + 1] = ai;
				}
			}
		}

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(float[] a, int left, int right, float[] work, int workBase, int workLen)
		{
			/*
			 * Phase 1: Move NaNs to the end of the array.
			 */
			while (left <= right && Float.IsNaN(a[right]))
			{
				--right;
			}
			for (int k = right; --k >= left;)
			{
				float ak = a[k];
				if (ak != ak) // a[k] is NaN
				{
					a[k] = a[right];
					a[right] = ak;
					--right;
				}
			}

			/*
			 * Phase 2: Sort everything except NaNs (which are already in place).
			 */
			DoSort(a, left, right, work, workBase, workLen);

			/*
			 * Phase 3: Place negative zeros before positive zeros.
			 */
			int hi = right;

			/*
			 * Find the first zero, or first positive, or last negative element.
			 */
			while (left < hi)
			{
				int middle = (int)((uint)(left + hi) >> 1);
				float middleValue = a[middle];

				if (middleValue < 0.0f)
				{
					left = middle + 1;
				}
				else
				{
					hi = middle;
				}
			}

			/*
			 * Skip the last negative value (if any) or all leading negative zeros.
			 */
			while (left <= right && Float.floatToRawIntBits(a[left]) < 0)
			{
				++left;
			}

			/*
			 * Move negative zeros to the beginning of the sub-range.
			 *
			 * Partitioning:
			 *
			 * +----------------------------------------------------+
			 * |   < 0.0   |   -0.0   |   0.0   |   ?  ( >= 0.0 )   |
			 * +----------------------------------------------------+
			 *              ^          ^         ^
			 *              |          |         |
			 *             left        p         k
			 *
			 * Invariants:
			 *
			 *   all in (*,  left)  <  0.0
			 *   all in [left,  p) == -0.0
			 *   all in [p,     k) ==  0.0
			 *   all in [k, right] >=  0.0
			 *
			 * Pointer k is the first index of ?-part.
			 */
			for (int k = left, p = left - 1; ++k <= right;)
			{
				float ak = a[k];
				if (ak != 0.0f)
				{
					break;
				}
				if (Float.floatToRawIntBits(ak) < 0) // ak is -0.0f
				{
					a[k] = 0.0f;
					a[++p] = -0.0f;
				}
			}
		}

		/// <summary>
		/// Sorts the specified range of the array.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		private static void DoSort(float[] a, int left, int right, float[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						float t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			float[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new float[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				float[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(float[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						float ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						float a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					float last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				float t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				float t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				float t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				float t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				float pivot1 = a[e2];
				float pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					float ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						float ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = a[great];
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				float pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					float ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = a[great];
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}

		/// <summary>
		/// Sorts the specified range of the array using the given
		/// workspace array slice if possible for merging
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		internal static void Sort(double[] a, int left, int right, double[] work, int workBase, int workLen)
		{
			/*
			 * Phase 1: Move NaNs to the end of the array.
			 */
			while (left <= right && Double.IsNaN(a[right]))
			{
				--right;
			}
			for (int k = right; --k >= left;)
			{
				double ak = a[k];
				if (ak != ak) // a[k] is NaN
				{
					a[k] = a[right];
					a[right] = ak;
					--right;
				}
			}

			/*
			 * Phase 2: Sort everything except NaNs (which are already in place).
			 */
			DoSort(a, left, right, work, workBase, workLen);

			/*
			 * Phase 3: Place negative zeros before positive zeros.
			 */
			int hi = right;

			/*
			 * Find the first zero, or first positive, or last negative element.
			 */
			while (left < hi)
			{
				int middle = (int)((uint)(left + hi) >> 1);
				double middleValue = a[middle];

				if (middleValue < 0.0d)
				{
					left = middle + 1;
				}
				else
				{
					hi = middle;
				}
			}

			/*
			 * Skip the last negative value (if any) or all leading negative zeros.
			 */
			while (left <= right && Double.doubleToRawLongBits(a[left]) < 0)
			{
				++left;
			}

			/*
			 * Move negative zeros to the beginning of the sub-range.
			 *
			 * Partitioning:
			 *
			 * +----------------------------------------------------+
			 * |   < 0.0   |   -0.0   |   0.0   |   ?  ( >= 0.0 )   |
			 * +----------------------------------------------------+
			 *              ^          ^         ^
			 *              |          |         |
			 *             left        p         k
			 *
			 * Invariants:
			 *
			 *   all in (*,  left)  <  0.0
			 *   all in [left,  p) == -0.0
			 *   all in [p,     k) ==  0.0
			 *   all in [k, right] >=  0.0
			 *
			 * Pointer k is the first index of ?-part.
			 */
			for (int k = left, p = left - 1; ++k <= right;)
			{
				double ak = a[k];
				if (ak != 0.0d)
				{
					break;
				}
				if (Double.doubleToRawLongBits(ak) < 0) // ak is -0.0d
				{
					a[k] = 0.0d;
					a[++p] = -0.0d;
				}
			}
		}

		/// <summary>
		/// Sorts the specified range of the array.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		private static void DoSort(double[] a, int left, int right, double[] work, int workBase, int workLen)
		{
			// Use Quicksort on small arrays
			if (right - left < QUICKSORT_THRESHOLD)
			{
				Sort(a, left, right, true);
				return;
			}

			/*
			 * Index run[i] is the start of i-th run
			 * (ascending or descending sequence).
			 */
			int[] run = new int[MAX_RUN_COUNT + 1];
			int count = 0;
			run[0] = left;

			// Check if the array is nearly sorted
			for (int k = left; k < right; run[count] = k)
			{
				if (a[k] < a[k + 1]) // ascending
				{
					while (++k <= right && a[k - 1] <= a[k]);
				} // descending
			else if (a[k] > a[k + 1])
			{
					while (++k <= right && a[k - 1] >= a[k]);
					for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
					{
						double t = a[lo];
						a[lo] = a[hi];
						a[hi] = t;
					}
			} // equal
				else
				{
					for (int m = MAX_RUN_LENGTH; ++k <= right && a[k - 1] == a[k];)
					{
						if (--m == 0)
						{
							Sort(a, left, right, true);
							return;
						}
					}
				}

				/*
				 * The array is not highly structured,
				 * use Quicksort instead of merge sort.
				 */
				if (++count == MAX_RUN_COUNT)
				{
					Sort(a, left, right, true);
					return;
				}
			}

			// Check special cases
			// Implementation note: variable "right" is increased by 1.
			if (run[count] == right++) // The last run contains one element
			{
				run[++count] = right;
			} // The array is already sorted
			else if (count == 1)
			{
				return;
			}

			// Determine alternation base for merge
			sbyte odd = 0;
			for (int n = 1; (n <<= 1) < count; odd ^= 1)
			{
				;
			}

			// Use or create temporary array b for merging
			double[] b; // temp array; alternates with a
			int ao, bo; // array offsets from 'left'
			int blen = right - left; // space needed for b
			if (work == null || workLen < blen || workBase + blen > work.Length)
			{
				work = new double[blen];
				workBase = 0;
			}
			if (odd == 0)
			{
				System.Array.Copy(a, left, work, workBase, blen);
				b = a;
				bo = 0;
				a = work;
				ao = workBase - left;
			}
			else
			{
				b = work;
				ao = 0;
				bo = workBase - left;
			}

			// Merging
			for (int last; count > 1; count = last)
			{
				for (int k = (last = 0) + 2; k <= count; k += 2)
				{
					int hi = run[k], mi = run[k - 1];
					for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
					{
						if (q >= hi || p < mi && a[p + ao] <= a[q + ao])
						{
							b[i + bo] = a[p++ + ao];
						}
						else
						{
							b[i + bo] = a[q++ + ao];
						}
					}
					run[++last] = hi;
				}
				if ((count & 1) != 0)
				{
					for (int i = right, lo = run[count - 1]; --i >= lo; b[i + bo] = a[i + ao])
					{
						;
					}
					run[++last] = right;
				}
				double[] t = a;
				a = b;
				b = t;
				int o = ao;
				ao = bo;
				bo = o;
			}
		}

		/// <summary>
		/// Sorts the specified range of the array by Dual-Pivot Quicksort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="left"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="right"> the index of the last element, inclusive, to be sorted </param>
		/// <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		private static void Sort(double[] a, int left, int right, bool leftmost)
		{
			int length = right - left + 1;

			// Use insertion sort on tiny arrays
			if (length < INSERTION_SORT_THRESHOLD)
			{
				if (leftmost)
				{
					/*
					 * Traditional (without sentinel) insertion sort,
					 * optimized for server VM, is used in case of
					 * the leftmost part.
					 */
					for (int i = left, j = i; i < right; j = ++i)
					{
						double ai = a[i + 1];
						while (ai < a[j])
						{
							a[j + 1] = a[j];
							if (j-- == left)
							{
								break;
							}
						}
						a[j + 1] = ai;
					}
				}
				else
				{
					/*
					 * Skip the longest ascending sequence.
					 */
					do
					{
						if (left >= right)
						{
							return;
						}
					} while (a[++left] >= a[left - 1]);

					/*
					 * Every element from adjoining part plays the role
					 * of sentinel, therefore this allows us to avoid the
					 * left range check on each iteration. Moreover, we use
					 * the more optimized algorithm, so called pair insertion
					 * sort, which is faster (in the context of Quicksort)
					 * than traditional implementation of insertion sort.
					 */
					for (int k = left; ++left <= right; k = ++left)
					{
						double a1 = a[k], a2 = a[left];

						if (a1 < a2)
						{
							a2 = a1;
							a1 = a[left];
						}
						while (a1 < a[--k])
						{
							a[k + 2] = a[k];
						}
						a[++k + 1] = a1;

						while (a2 < a[--k])
						{
							a[k + 1] = a[k];
						}
						a[k + 1] = a2;
					}
					double last = a[right];

					while (last < a[--right])
					{
						a[right + 1] = a[right];
					}
					a[right + 1] = last;
				}
				return;
			}

			// Inexpensive approximation of length / 7
			int seventh = (length >> 3) + (length >> 6) + 1;

			/*
			 * Sort five evenly spaced elements around (and including) the
			 * center element in the range. These elements will be used for
			 * pivot selection as described below. The choice for spacing
			 * these elements was empirically determined to work well on
			 * a wide variety of inputs.
			 */
			int e3 = (int)((uint)(left + right) >> 1); // The midpoint
			int e2 = e3 - seventh;
			int e1 = e2 - seventh;
			int e4 = e3 + seventh;
			int e5 = e4 + seventh;

			// Sort these elements using insertion sort
			if (a[e2] < a[e1])
			{
				double t = a[e2];
				a[e2] = a[e1];
				a[e1] = t;
			}

			if (a[e3] < a[e2])
			{
				double t = a[e3];
				a[e3] = a[e2];
				a[e2] = t;
				if (t < a[e1])
				{
					a[e2] = a[e1];
					a[e1] = t;
				}
			}
			if (a[e4] < a[e3])
			{
				double t = a[e4];
				a[e4] = a[e3];
				a[e3] = t;
				if (t < a[e2])
				{
					a[e3] = a[e2];
					a[e2] = t;
					if (t < a[e1])
					{
						a[e2] = a[e1];
						a[e1] = t;
					}
				}
			}
			if (a[e5] < a[e4])
			{
				double t = a[e5];
				a[e5] = a[e4];
				a[e4] = t;
				if (t < a[e3])
				{
					a[e4] = a[e3];
					a[e3] = t;
					if (t < a[e2])
					{
						a[e3] = a[e2];
						a[e2] = t;
						if (t < a[e1])
						{
							a[e2] = a[e1];
							a[e1] = t;
						}
					}
				}
			}

			// Pointers
			int less = left; // The index of the first element of center part
			int great = right; // The index before the first element of right part

			if (a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5])
			{
				/*
				 * Use the second and fourth of the five sorted elements as pivots.
				 * These values are inexpensive approximations of the first and
				 * second terciles of the array. Note that pivot1 <= pivot2.
				 */
				double pivot1 = a[e2];
				double pivot2 = a[e4];

				/*
				 * The first and the last elements to be sorted are moved to the
				 * locations formerly occupied by the pivots. When partitioning
				 * is complete, the pivots are swapped back into their final
				 * positions, and excluded from subsequent sorting.
				 */
				a[e2] = a[left];
				a[e4] = a[right];

				/*
				 * Skip elements, which are less or greater than pivot values.
				 */
				while (a[++less] < pivot1);
				while (a[--great] > pivot2);

				/*
				 * Partitioning:
				 *
				 *   left part           center part                   right part
				 * +--------------------------------------------------------------+
				 * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
				 * +--------------------------------------------------------------+
				 *               ^                          ^       ^
				 *               |                          |       |
				 *              less                        k     great
				 *
				 * Invariants:
				 *
				 *              all in (left, less)   < pivot1
				 *    pivot1 <= all in [less, k)     <= pivot2
				 *              all in (great, right) > pivot2
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less - 1; ++k <= great;)
				{
					double ak = a[k];
					if (ak < pivot1) // Move a[k] to left part
					{
						a[k] = a[less];
						/*
						 * Here and below we use "a[i] = b; i++;" instead
						 * of "a[i++] = b;" due to performance issue.
						 */
						a[less] = ak;
						++less;
					} // Move a[k] to right part
					else if (ak > pivot2)
					{
						while (a[great] > pivot2)
						{
							if (great-- == k)
							{
								goto outerBreak;
							}
						}
						if (a[great] < pivot1) // a[great] <= pivot2
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // pivot1 <= a[great] <= pivot2
						else
						{
							a[k] = a[great];
						}
						/*
						 * Here and below we use "a[i] = b; i--;" instead
						 * of "a[i--] = b;" due to performance issue.
						 */
						a[great] = ak;
						--great;
					}
					outerContinue:;
				}
				outerBreak:

				// Swap pivots into their final positions
				a[left] = a[less - 1];
				a[less - 1] = pivot1;
				a[right] = a[great + 1];
				a[great + 1] = pivot2;

				// Sort left and right parts recursively, excluding known pivots
				Sort(a, left, less - 2, leftmost);
				Sort(a, great + 2, right, false);

				/*
				 * If center part is too large (comprises > 4/7 of the array),
				 * swap internal pivot values to ends.
				 */
				if (less < e1 && e5 < great)
				{
					/*
					 * Skip elements, which are equal to pivot values.
					 */
					while (a[less] == pivot1)
					{
						++less;
					}

					while (a[great] == pivot2)
					{
						--great;
					}

					/*
					 * Partitioning:
					 *
					 *   left part         center part                  right part
					 * +----------------------------------------------------------+
					 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
					 * +----------------------------------------------------------+
					 *              ^                        ^       ^
					 *              |                        |       |
					 *             less                      k     great
					 *
					 * Invariants:
					 *
					 *              all in (*,  less) == pivot1
					 *     pivot1 < all in [less,  k)  < pivot2
					 *              all in (great, *) == pivot2
					 *
					 * Pointer k is the first index of ?-part.
					 */
					for (int k = less - 1; ++k <= great;)
					{
						double ak = a[k];
						if (ak == pivot1) // Move a[k] to left part
						{
							a[k] = a[less];
							a[less] = ak;
							++less;
						} // Move a[k] to right part
						else if (ak == pivot2)
						{
							while (a[great] == pivot2)
							{
								if (great-- == k)
								{
									goto outerBreak;
								}
							}
							if (a[great] == pivot1) // a[great] < pivot2
							{
								a[k] = a[less];
								/*
								 * Even though a[great] equals to pivot1, the
								 * assignment a[less] = pivot1 may be incorrect,
								 * if a[great] and pivot1 are floating-point zeros
								 * of different signs. Therefore in float and
								 * double sorting methods we have to use more
								 * accurate assignment a[less] = a[great].
								 */
								a[less] = a[great];
								++less;
							} // pivot1 < a[great] < pivot2
							else
							{
								a[k] = a[great];
							}
							a[great] = ak;
							--great;
						}
						outerContinue:;
					}
					outerBreak:;
				}

				// Sort center part recursively
				Sort(a, less, great, false);

			} // Partitioning with one pivot
			else
			{
				/*
				 * Use the third of the five sorted elements as pivot.
				 * This value is inexpensive approximation of the median.
				 */
				double pivot = a[e3];

				/*
				 * Partitioning degenerates to the traditional 3-way
				 * (or "Dutch National Flag") schema:
				 *
				 *   left part    center part              right part
				 * +-------------------------------------------------+
				 * |  < pivot  |   == pivot   |     ?    |  > pivot  |
				 * +-------------------------------------------------+
				 *              ^              ^        ^
				 *              |              |        |
				 *             less            k      great
				 *
				 * Invariants:
				 *
				 *   all in (left, less)   < pivot
				 *   all in [less, k)     == pivot
				 *   all in (great, right) > pivot
				 *
				 * Pointer k is the first index of ?-part.
				 */
				for (int k = less; k <= great; ++k)
				{
					if (a[k] == pivot)
					{
						continue;
					}
					double ak = a[k];
					if (ak < pivot) // Move a[k] to left part
					{
						a[k] = a[less];
						a[less] = ak;
						++less;
					} // a[k] > pivot - Move a[k] to right part
					else
					{
						while (a[great] > pivot)
						{
							--great;
						}
						if (a[great] < pivot) // a[great] <= pivot
						{
							a[k] = a[less];
							a[less] = a[great];
							++less;
						} // a[great] == pivot
						else
						{
							/*
							 * Even though a[great] equals to pivot, the
							 * assignment a[k] = pivot may be incorrect,
							 * if a[great] and pivot are floating-point
							 * zeros of different signs. Therefore in float
							 * and double sorting methods we have to use
							 * more accurate assignment a[k] = a[great].
							 */
							a[k] = a[great];
						}
						a[great] = ak;
						--great;
					}
				}

				/*
				 * Sort left and right parts recursively.
				 * All elements from center part are equal
				 * and, therefore, already sorted.
				 */
				Sort(a, left, less - 1, leftmost);
				Sort(a, great + 1, right, false);
			}
		}
	}

}