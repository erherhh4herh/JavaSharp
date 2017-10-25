using System;
using System.Diagnostics;

/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
 * Copyright 2009 Google Inc.  All Rights Reserved.
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
	/// This is a near duplicate of <seealso cref="TimSort"/>, modified for use with
	/// arrays of objects that implement <seealso cref="Comparable"/>, instead of using
	/// explicit comparators.
	/// 
	/// <para>If you are using an optimizing VM, you may find that ComparableTimSort
	/// offers no performance benefit over TimSort in conjunction with a
	/// comparator that simply returns {@code ((Comparable)first).compareTo(Second)}.
	/// If this is the case, you are better off deleting ComparableTimSort to
	/// eliminate the code duplication.  (See Arrays.java for details.)
	/// 
	/// @author Josh Bloch
	/// </para>
	/// </summary>
	internal class ComparableTimSort
	{
		/// <summary>
		/// This is the minimum sized sequence that will be merged.  Shorter
		/// sequences will be lengthened by calling binarySort.  If the entire
		/// array is less than this length, no merges will be performed.
		/// 
		/// This constant should be a power of two.  It was 64 in Tim Peter's C
		/// implementation, but 32 was empirically determined to work better in
		/// this implementation.  In the unlikely event that you set this constant
		/// to be a number that's not a power of two, you'll need to change the
		/// <seealso cref="#minRunLength"/> computation.
		/// 
		/// If you decrease this constant, you must change the stackLen
		/// computation in the TimSort constructor, or you risk an
		/// ArrayOutOfBounds exception.  See listsort.txt for a discussion
		/// of the minimum stack length required as a function of the length
		/// of the array being sorted and the minimum merge sequence length.
		/// </summary>
		private const int MIN_MERGE = 32;

		/// <summary>
		/// The array being sorted.
		/// </summary>
		private readonly Object[] a;

		/// <summary>
		/// When we get into galloping mode, we stay there until both runs win less
		/// often than MIN_GALLOP consecutive times.
		/// </summary>
		private const int MIN_GALLOP = 7;

		/// <summary>
		/// This controls when we get *into* galloping mode.  It is initialized
		/// to MIN_GALLOP.  The mergeLo and mergeHi methods nudge it higher for
		/// random data, and lower for highly structured data.
		/// </summary>
		private int MinGallop = MIN_GALLOP;

		/// <summary>
		/// Maximum initial size of tmp array, which is used for merging.  The array
		/// can grow to accommodate demand.
		/// 
		/// Unlike Tim's original C version, we do not allocate this much storage
		/// when sorting smaller arrays.  This change was required for performance.
		/// </summary>
		private const int INITIAL_TMP_STORAGE_LENGTH = 256;

		/// <summary>
		/// Temp storage for merges. A workspace array may optionally be
		/// provided in constructor, and if so will be used as long as it
		/// is big enough.
		/// </summary>
		private Object[] Tmp;
		private int TmpBase; // base of tmp array slice
		private int TmpLen; // length of tmp array slice

		/// <summary>
		/// A stack of pending runs yet to be merged.  Run i starts at
		/// address base[i] and extends for len[i] elements.  It's always
		/// true (so long as the indices are in bounds) that:
		/// 
		///     runBase[i] + runLen[i] == runBase[i + 1]
		/// 
		/// so we could cut the storage for this, but it's a minor amount,
		/// and keeping all the info explicit simplifies the code.
		/// </summary>
		private int StackSize = 0; // Number of pending runs on stack
		private readonly int[] RunBase;
		private readonly int[] RunLen;

		/// <summary>
		/// Creates a TimSort instance to maintain the state of an ongoing sort.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array </param>
		private ComparableTimSort(Object[] a, Object[] work, int workBase, int workLen)
		{
			this.a = a;

			// Allocate temp storage (which may be increased later if necessary)
			int len = a.Length;
			int tlen = (len < 2 * INITIAL_TMP_STORAGE_LENGTH) ? (int)((uint)len >> 1) : INITIAL_TMP_STORAGE_LENGTH;
			if (work == null || workLen < tlen || workBase + tlen > work.Length)
			{
				Tmp = new Object[tlen];
				TmpBase = 0;
				TmpLen = tlen;
			}
			else
			{
				Tmp = work;
				TmpBase = workBase;
				TmpLen = workLen;
			}

			/*
			 * Allocate runs-to-be-merged stack (which cannot be expanded).  The
			 * stack length requirements are described in listsort.txt.  The C
			 * version always uses the same stack length (85), but this was
			 * measured to be too expensive when sorting "mid-sized" arrays (e.g.,
			 * 100 elements) in Java.  Therefore, we use smaller (but sufficiently
			 * large) stack lengths for smaller arrays.  The "magic numbers" in the
			 * computation below must be changed if MIN_MERGE is decreased.  See
			 * the MIN_MERGE declaration above for more information.
			 * The maximum value of 49 allows for an array up to length
			 * Integer.MAX_VALUE-4, if array is filled by the worst case stack size
			 * increasing scenario. More explanations are given in section 4 of:
			 * http://envisage-project.eu/wp-content/uploads/2015/02/sorting.pdf
			 */
			int stackLen = (len < 120 ? 5 : len < 1542 ? 10 : len < 119151 ? 24 : 49);
			RunBase = new int[stackLen];
			RunLen = new int[stackLen];
		}

		/*
		 * The next method (package private and static) constitutes the
		 * entire API of this class.
		 */

		/// <summary>
		/// Sorts the given range, using the given workspace array slice
		/// for temp storage when possible. This method is designed to be
		/// invoked from public methods (in class Arrays) after performing
		/// any necessary array bounds checks and expanding parameters into
		/// the required forms.
		/// </summary>
		/// <param name="a"> the array to be sorted </param>
		/// <param name="lo"> the index of the first element, inclusive, to be sorted </param>
		/// <param name="hi"> the index of the last element, exclusive, to be sorted </param>
		/// <param name="work"> a workspace array (slice) </param>
		/// <param name="workBase"> origin of usable space in work array </param>
		/// <param name="workLen"> usable size of work array
		/// @since 1.8 </param>
		internal static void Sort(Object[] a, int lo, int hi, Object[] work, int workBase, int workLen)
		{
			Debug.Assert(a != null && lo >= 0 && lo <= hi && hi <= a.Length);

			int nRemaining = hi - lo;
			if (nRemaining < 2)
			{
				return; // Arrays of size 0 and 1 are always sorted
			}

			// If array is small, do a "mini-TimSort" with no merges
			if (nRemaining < MIN_MERGE)
			{
				int initRunLen = CountRunAndMakeAscending(a, lo, hi);
				BinarySort(a, lo, hi, lo + initRunLen);
				return;
			}

			/// <summary>
			/// March over the array once, left to right, finding natural runs,
			/// extending short natural runs to minRun elements, and merging runs
			/// to maintain stack invariant.
			/// </summary>
			ComparableTimSort ts = new ComparableTimSort(a, work, workBase, workLen);
			int minRun = MinRunLength(nRemaining);
			do
			{
				// Identify next run
				int runLen = CountRunAndMakeAscending(a, lo, hi);

				// If run is short, extend to min(minRun, nRemaining)
				if (runLen < minRun)
				{
					int force = nRemaining <= minRun ? nRemaining : minRun;
					BinarySort(a, lo, lo + force, lo + runLen);
					runLen = force;
				}

				// Push run onto pending-run stack, and maybe merge
				ts.PushRun(lo, runLen);
				ts.MergeCollapse();

				// Advance to find next run
				lo += runLen;
				nRemaining -= runLen;
			} while (nRemaining != 0);

			// Merge all remaining runs to complete sort
			Debug.Assert(lo == hi);
			ts.MergeForceCollapse();
			Debug.Assert(ts.StackSize == 1);
		}

		/// <summary>
		/// Sorts the specified portion of the specified array using a binary
		/// insertion sort.  This is the best method for sorting small numbers
		/// of elements.  It requires O(n log n) compares, but O(n^2) data
		/// movement (worst case).
		/// 
		/// If the initial part of the specified range is already sorted,
		/// this method can take advantage of it: the method assumes that the
		/// elements from index {@code lo}, inclusive, to {@code start},
		/// exclusive are already sorted.
		/// </summary>
		/// <param name="a"> the array in which a range is to be sorted </param>
		/// <param name="lo"> the index of the first element in the range to be sorted </param>
		/// <param name="hi"> the index after the last element in the range to be sorted </param>
		/// <param name="start"> the index of the first element in the range that is
		///        not already known to be sorted ({@code lo <= start <= hi}) </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"fallthrough", "rawtypes", "unchecked"}) private static void binarySort(Object[] a, int lo, int hi, int start)
		private static void BinarySort(Object[] a, int lo, int hi, int start)
		{
			Debug.Assert(lo <= start && start <= hi);
			if (start == lo)
			{
				start++;
			}
			for (; start < hi; start++)
			{
				Comparable pivot = (Comparable) a[start];

				// Set left (and right) to the index where a[start] (pivot) belongs
				int left = lo;
				int right = start;
				Debug.Assert(left <= right);
				/*
				 * Invariants:
				 *   pivot >= all in [lo, left).
				 *   pivot <  all in [right, start).
				 */
				while (left < right)
				{
					int mid = (int)((uint)(left + right) >> 1);
					if (pivot.CompareTo(a[mid]) < 0)
					{
						right = mid;
					}
					else
					{
						left = mid + 1;
					}
				}
				Debug.Assert(left == right);

				/*
				 * The invariants still hold: pivot >= all in [lo, left) and
				 * pivot < all in [left, start), so pivot belongs at left.  Note
				 * that if there are elements equal to pivot, left points to the
				 * first slot after them -- that's why this sort is stable.
				 * Slide elements over to make room for pivot.
				 */
				int n = start - left; // The number of elements to move
				// Switch is just an optimization for arraycopy in default case
				switch (n)
				{
					case 2:
						a[left + 2] = a[left + 1];
						goto case 1;
					case 1:
						a[left + 1] = a[left];
							 break;
					default:
						System.Array.Copy(a, left, a, left + 1, n);
					break;
				}
				a[left] = pivot;
			}
		}

		/// <summary>
		/// Returns the length of the run beginning at the specified position in
		/// the specified array and reverses the run if it is descending (ensuring
		/// that the run will always be ascending when the method returns).
		/// 
		/// A run is the longest ascending sequence with:
		/// 
		///    a[lo] <= a[lo + 1] <= a[lo + 2] <= ...
		/// 
		/// or the longest descending sequence with:
		/// 
		///    a[lo] >  a[lo + 1] >  a[lo + 2] >  ...
		/// 
		/// For its intended use in a stable mergesort, the strictness of the
		/// definition of "descending" is needed so that the call can safely
		/// reverse a descending sequence without violating stability.
		/// </summary>
		/// <param name="a"> the array in which a run is to be counted and possibly reversed </param>
		/// <param name="lo"> index of the first element in the run </param>
		/// <param name="hi"> index after the last element that may be contained in the run.
		///          It is required that {@code lo < hi}. </param>
		/// <returns>  the length of the run beginning at the specified position in
		///          the specified array </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) private static int countRunAndMakeAscending(Object[] a, int lo, int hi)
		private static int CountRunAndMakeAscending(Object[] a, int lo, int hi)
		{
			Debug.Assert(lo < hi);
			int runHi = lo + 1;
			if (runHi == hi)
			{
				return 1;
			}

			// Find end of run, and reverse range if descending
			if (((Comparable) a[runHi++]).CompareTo(a[lo]) < 0) // Descending
			{
				while (runHi < hi && ((Comparable) a[runHi]).CompareTo(a[runHi - 1]) < 0)
				{
					runHi++;
				}
				ReverseRange(a, lo, runHi);
			} // Ascending
			else
			{
				while (runHi < hi && ((Comparable) a[runHi]).CompareTo(a[runHi - 1]) >= 0)
				{
					runHi++;
				}
			}

			return runHi - lo;
		}

		/// <summary>
		/// Reverse the specified range of the specified array.
		/// </summary>
		/// <param name="a"> the array in which a range is to be reversed </param>
		/// <param name="lo"> the index of the first element in the range to be reversed </param>
		/// <param name="hi"> the index after the last element in the range to be reversed </param>
		private static void ReverseRange(Object[] a, int lo, int hi)
		{
			hi--;
			while (lo < hi)
			{
				Object t = a[lo];
				a[lo++] = a[hi];
				a[hi--] = t;
			}
		}

		/// <summary>
		/// Returns the minimum acceptable run length for an array of the specified
		/// length. Natural runs shorter than this will be extended with
		/// <seealso cref="#binarySort"/>.
		/// 
		/// Roughly speaking, the computation is:
		/// 
		///  If n < MIN_MERGE, return n (it's too small to bother with fancy stuff).
		///  Else if n is an exact power of 2, return MIN_MERGE/2.
		///  Else return an int k, MIN_MERGE/2 <= k <= MIN_MERGE, such that n/k
		///   is close to, but strictly less than, an exact power of 2.
		/// 
		/// For the rationale, see listsort.txt.
		/// </summary>
		/// <param name="n"> the length of the array to be sorted </param>
		/// <returns> the length of the minimum run to be merged </returns>
		private static int MinRunLength(int n)
		{
			Debug.Assert(n >= 0);
			int r = 0; // Becomes 1 if any 1 bits are shifted off
			while (n >= MIN_MERGE)
			{
				r |= (n & 1);
				n >>= 1;
			}
			return n + r;
		}

		/// <summary>
		/// Pushes the specified run onto the pending-run stack.
		/// </summary>
		/// <param name="runBase"> index of the first element in the run </param>
		/// <param name="runLen">  the number of elements in the run </param>
		private void PushRun(int runBase, int runLen)
		{
			this.RunBase[StackSize] = runBase;
			this.RunLen[StackSize] = runLen;
			StackSize++;
		}

		/// <summary>
		/// Examines the stack of runs waiting to be merged and merges adjacent runs
		/// until the stack invariants are reestablished:
		/// 
		///     1. runLen[i - 3] > runLen[i - 2] + runLen[i - 1]
		///     2. runLen[i - 2] > runLen[i - 1]
		/// 
		/// This method is called each time a new run is pushed onto the stack,
		/// so the invariants are guaranteed to hold for i < stackSize upon
		/// entry to the method.
		/// </summary>
		private void MergeCollapse()
		{
			while (StackSize > 1)
			{
				int n = StackSize - 2;
				if (n > 0 && RunLen[n - 1] <= RunLen[n] + RunLen[n + 1])
				{
					if (RunLen[n - 1] < RunLen[n + 1])
					{
						n--;
					}
					MergeAt(n);
				}
				else if (RunLen[n] <= RunLen[n + 1])
				{
					MergeAt(n);
				}
				else
				{
					break; // Invariant is established
				}
			}
		}

		/// <summary>
		/// Merges all runs on the stack until only one remains.  This method is
		/// called once, to complete the sort.
		/// </summary>
		private void MergeForceCollapse()
		{
			while (StackSize > 1)
			{
				int n = StackSize - 2;
				if (n > 0 && RunLen[n - 1] < RunLen[n + 1])
				{
					n--;
				}
				MergeAt(n);
			}
		}

		/// <summary>
		/// Merges the two runs at stack indices i and i+1.  Run i must be
		/// the penultimate or antepenultimate run on the stack.  In other words,
		/// i must be equal to stackSize-2 or stackSize-3.
		/// </summary>
		/// <param name="i"> stack index of the first of the two runs to merge </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void mergeAt(int i)
		private void MergeAt(int i)
		{
			Debug.Assert(StackSize >= 2);
			Debug.Assert(i >= 0);
			Debug.Assert(i == StackSize - 2 || i == StackSize - 3);

			int base1 = RunBase[i];
			int len1 = RunLen[i];
			int base2 = RunBase[i + 1];
			int len2 = RunLen[i + 1];
			Debug.Assert(len1 > 0 && len2 > 0);
			Debug.Assert(base1 + len1 == base2);

			/*
			 * Record the length of the combined runs; if i is the 3rd-last
			 * run now, also slide over the last run (which isn't involved
			 * in this merge).  The current run (i+1) goes away in any case.
			 */
			RunLen[i] = len1 + len2;
			if (i == StackSize - 3)
			{
				RunBase[i + 1] = RunBase[i + 2];
				RunLen[i + 1] = RunLen[i + 2];
			}
			StackSize--;

			/*
			 * Find where the first element of run2 goes in run1. Prior elements
			 * in run1 can be ignored (because they're already in place).
			 */
			int k = GallopRight((Comparable<Object>) a[base2], a, base1, len1, 0);
			Debug.Assert(k >= 0);
			base1 += k;
			len1 -= k;
			if (len1 == 0)
			{
				return;
			}

			/*
			 * Find where the last element of run1 goes in run2. Subsequent elements
			 * in run2 can be ignored (because they're already in place).
			 */
			len2 = GallopLeft((Comparable<Object>) a[base1 + len1 - 1], a, base2, len2, len2 - 1);
			Debug.Assert(len2 >= 0);
			if (len2 == 0)
			{
				return;
			}

			// Merge remaining runs, using tmp array with min(len1, len2) elements
			if (len1 <= len2)
			{
				MergeLo(base1, len1, base2, len2);
			}
			else
			{
				MergeHi(base1, len1, base2, len2);
			}
		}

		/// <summary>
		/// Locates the position at which to insert the specified key into the
		/// specified sorted range; if the range contains an element equal to key,
		/// returns the index of the leftmost equal element.
		/// </summary>
		/// <param name="key"> the key whose insertion point to search for </param>
		/// <param name="a"> the array in which to search </param>
		/// <param name="base"> the index of the first element in the range </param>
		/// <param name="len"> the length of the range; must be > 0 </param>
		/// <param name="hint"> the index at which to begin the search, 0 <= hint < n.
		///     The closer hint is to the result, the faster this method will run. </param>
		/// <returns> the int k,  0 <= k <= n such that a[b + k - 1] < key <= a[b + k],
		///    pretending that a[b - 1] is minus infinity and a[b + n] is infinity.
		///    In other words, key belongs at index b + k; or in other words,
		///    the first k elements of a should precede key, and the last n - k
		///    should follow it. </returns>
		private static int GallopLeft(Comparable<Object> key, Object[] a, int @base, int len, int hint)
		{
			Debug.Assert(len > 0 && hint >= 0 && hint < len);

			int lastOfs = 0;
			int ofs = 1;
			if (key.CompareTo(a[@base + hint]) > 0)
			{
				// Gallop right until a[base+hint+lastOfs] < key <= a[base+hint+ofs]
				int maxOfs = len - hint;
				while (ofs < maxOfs && key.CompareTo(a[@base + hint + ofs]) > 0)
				{
					lastOfs = ofs;
					ofs = (ofs << 1) + 1;
					if (ofs <= 0) // int overflow
					{
						ofs = maxOfs;
					}
				}
				if (ofs > maxOfs)
				{
					ofs = maxOfs;
				}

				// Make offsets relative to base
				lastOfs += hint;
				ofs += hint;
			} // key <= a[base + hint]
			else
			{
				// Gallop left until a[base+hint-ofs] < key <= a[base+hint-lastOfs]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxOfs = hint + 1;
				int maxOfs = hint + 1;
				while (ofs < maxOfs && key.CompareTo(a[@base + hint - ofs]) <= 0)
				{
					lastOfs = ofs;
					ofs = (ofs << 1) + 1;
					if (ofs <= 0) // int overflow
					{
						ofs = maxOfs;
					}
				}
				if (ofs > maxOfs)
				{
					ofs = maxOfs;
				}

				// Make offsets relative to base
				int tmp = lastOfs;
				lastOfs = hint - ofs;
				ofs = hint - tmp;
			}
			Debug.Assert(-1 <= lastOfs && lastOfs < ofs && ofs <= len);

			/*
			 * Now a[base+lastOfs] < key <= a[base+ofs], so key belongs somewhere
			 * to the right of lastOfs but no farther right than ofs.  Do a binary
			 * search, with invariant a[base + lastOfs - 1] < key <= a[base + ofs].
			 */
			lastOfs++;
			while (lastOfs < ofs)
			{
				int m = lastOfs + ((int)((uint)(ofs - lastOfs) >> 1));

				if (key.CompareTo(a[@base + m]) > 0)
				{
					lastOfs = m + 1; // a[base + m] < key
				}
				else
				{
					ofs = m; // key <= a[base + m]
				}
			}
			Debug.Assert(lastOfs == ofs); // so a[base + ofs - 1] < key <= a[base + ofs]
			return ofs;
		}

		/// <summary>
		/// Like gallopLeft, except that if the range contains an element equal to
		/// key, gallopRight returns the index after the rightmost equal element.
		/// </summary>
		/// <param name="key"> the key whose insertion point to search for </param>
		/// <param name="a"> the array in which to search </param>
		/// <param name="base"> the index of the first element in the range </param>
		/// <param name="len"> the length of the range; must be > 0 </param>
		/// <param name="hint"> the index at which to begin the search, 0 <= hint < n.
		///     The closer hint is to the result, the faster this method will run. </param>
		/// <returns> the int k,  0 <= k <= n such that a[b + k - 1] <= key < a[b + k] </returns>
		private static int GallopRight(Comparable<Object> key, Object[] a, int @base, int len, int hint)
		{
			Debug.Assert(len > 0 && hint >= 0 && hint < len);

			int ofs = 1;
			int lastOfs = 0;
			if (key.CompareTo(a[@base + hint]) < 0)
			{
				// Gallop left until a[b+hint - ofs] <= key < a[b+hint - lastOfs]
				int maxOfs = hint + 1;
				while (ofs < maxOfs && key.CompareTo(a[@base + hint - ofs]) < 0)
				{
					lastOfs = ofs;
					ofs = (ofs << 1) + 1;
					if (ofs <= 0) // int overflow
					{
						ofs = maxOfs;
					}
				}
				if (ofs > maxOfs)
				{
					ofs = maxOfs;
				}

				// Make offsets relative to b
				int tmp = lastOfs;
				lastOfs = hint - ofs;
				ofs = hint - tmp;
			} // a[b + hint] <= key
			else
			{
				// Gallop right until a[b+hint + lastOfs] <= key < a[b+hint + ofs]
				int maxOfs = len - hint;
				while (ofs < maxOfs && key.CompareTo(a[@base + hint + ofs]) >= 0)
				{
					lastOfs = ofs;
					ofs = (ofs << 1) + 1;
					if (ofs <= 0) // int overflow
					{
						ofs = maxOfs;
					}
				}
				if (ofs > maxOfs)
				{
					ofs = maxOfs;
				}

				// Make offsets relative to b
				lastOfs += hint;
				ofs += hint;
			}
			Debug.Assert(-1 <= lastOfs && lastOfs < ofs && ofs <= len);

			/*
			 * Now a[b + lastOfs] <= key < a[b + ofs], so key belongs somewhere to
			 * the right of lastOfs but no farther right than ofs.  Do a binary
			 * search, with invariant a[b + lastOfs - 1] <= key < a[b + ofs].
			 */
			lastOfs++;
			while (lastOfs < ofs)
			{
				int m = lastOfs + ((int)((uint)(ofs - lastOfs) >> 1));

				if (key.CompareTo(a[@base + m]) < 0)
				{
					ofs = m; // key < a[b + m]
				}
				else
				{
					lastOfs = m + 1; // a[b + m] <= key
				}
			}
			Debug.Assert(lastOfs == ofs); // so a[b + ofs - 1] <= key < a[b + ofs]
			return ofs;
		}

		/// <summary>
		/// Merges two adjacent runs in place, in a stable fashion.  The first
		/// element of the first run must be greater than the first element of the
		/// second run (a[base1] > a[base2]), and the last element of the first run
		/// (a[base1 + len1-1]) must be greater than all elements of the second run.
		/// 
		/// For performance, this method should be called only when len1 <= len2;
		/// its twin, mergeHi should be called if len1 >= len2.  (Either method
		/// may be called if len1 == len2.)
		/// </summary>
		/// <param name="base1"> index of first element in first run to be merged </param>
		/// <param name="len1">  length of first run to be merged (must be > 0) </param>
		/// <param name="base2"> index of first element in second run to be merged
		///        (must be aBase + aLen) </param>
		/// <param name="len2">  length of second run to be merged (must be > 0) </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) private void mergeLo(int base1, int len1, int base2, int len2)
		private void MergeLo(int base1, int len1, int base2, int len2)
		{
			Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

			// Copy first run into temp array
			Object[] a = this.a; // For performance
			Object[] tmp = EnsureCapacity(len1);

			int cursor1 = TmpBase; // Indexes into tmp array
			int cursor2 = base2; // Indexes int a
			int dest = base1; // Indexes int a
			System.Array.Copy(a, base1, tmp, cursor1, len1);

			// Move first element of second run and deal with degenerate cases
			a[dest++] = a[cursor2++];
			if (--len2 == 0)
			{
				System.Array.Copy(tmp, cursor1, a, dest, len1);
				return;
			}
			if (len1 == 1)
			{
				System.Array.Copy(a, cursor2, a, dest, len2);
				a[dest + len2] = tmp[cursor1]; // Last elt of run 1 to end of merge
				return;
			}

			int minGallop = this.MinGallop; // Use local variable for performance
			while (true)
			{
				int count1 = 0; // Number of times in a row that first run won
				int count2 = 0; // Number of times in a row that second run won

				/*
				 * Do the straightforward thing until (if ever) one run starts
				 * winning consistently.
				 */
				do
				{
					Debug.Assert(len1 > 1 && len2 > 0);
					if (((Comparable) a[cursor2]).CompareTo(tmp[cursor1]) < 0)
					{
						a[dest++] = a[cursor2++];
						count2++;
						count1 = 0;
						if (--len2 == 0)
						{
							goto outerBreak;
						}
					}
					else
					{
						a[dest++] = tmp[cursor1++];
						count1++;
						count2 = 0;
						if (--len1 == 1)
						{
							goto outerBreak;
						}
					}
				} while ((count1 | count2) < minGallop);

				/*
				 * One run is winning so consistently that galloping may be a
				 * huge win. So try that, and continue galloping until (if ever)
				 * neither run appears to be winning consistently anymore.
				 */
				do
				{
					Debug.Assert(len1 > 1 && len2 > 0);
					count1 = GallopRight((Comparable) a[cursor2], tmp, cursor1, len1, 0);
					if (count1 != 0)
					{
						System.Array.Copy(tmp, cursor1, a, dest, count1);
						dest += count1;
						cursor1 += count1;
						len1 -= count1;
						if (len1 <= 1) // len1 == 1 || len1 == 0
						{
							goto outerBreak;
						}
					}
					a[dest++] = a[cursor2++];
					if (--len2 == 0)
					{
						goto outerBreak;
					}

					count2 = GallopLeft((Comparable) tmp[cursor1], a, cursor2, len2, 0);
					if (count2 != 0)
					{
						System.Array.Copy(a, cursor2, a, dest, count2);
						dest += count2;
						cursor2 += count2;
						len2 -= count2;
						if (len2 == 0)
						{
							goto outerBreak;
						}
					}
					a[dest++] = tmp[cursor1++];
					if (--len1 == 1)
					{
						goto outerBreak;
					}
					minGallop--;
				} while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
				if (minGallop < 0)
				{
					minGallop = 0;
				}
				minGallop += 2; // Penalize for leaving gallop mode
			outerContinue:;
			} // End of "outer" loop
		outerBreak:
			this.MinGallop = minGallop < 1 ? 1 : minGallop; // Write back to field

			if (len1 == 1)
			{
				Debug.Assert(len2 > 0);
				System.Array.Copy(a, cursor2, a, dest, len2);
				a[dest + len2] = tmp[cursor1]; //  Last elt of run 1 to end of merge
			}
			else if (len1 == 0)
			{
				throw new IllegalArgumentException("Comparison method violates its general contract!");
			}
			else
			{
				Debug.Assert(len2 == 0);
				Debug.Assert(len1 > 1);
				System.Array.Copy(tmp, cursor1, a, dest, len1);
			}
		}

		/// <summary>
		/// Like mergeLo, except that this method should be called only if
		/// len1 >= len2; mergeLo should be called if len1 <= len2.  (Either method
		/// may be called if len1 == len2.)
		/// </summary>
		/// <param name="base1"> index of first element in first run to be merged </param>
		/// <param name="len1">  length of first run to be merged (must be > 0) </param>
		/// <param name="base2"> index of first element in second run to be merged
		///        (must be aBase + aLen) </param>
		/// <param name="len2">  length of second run to be merged (must be > 0) </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) private void mergeHi(int base1, int len1, int base2, int len2)
		private void MergeHi(int base1, int len1, int base2, int len2)
		{
			Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

			// Copy second run into temp array
			Object[] a = this.a; // For performance
			Object[] tmp = EnsureCapacity(len2);
			int tmpBase = this.TmpBase;
			System.Array.Copy(a, base2, tmp, tmpBase, len2);

			int cursor1 = base1 + len1 - 1; // Indexes into a
			int cursor2 = tmpBase + len2 - 1; // Indexes into tmp array
			int dest = base2 + len2 - 1; // Indexes into a

			// Move last element of first run and deal with degenerate cases
			a[dest--] = a[cursor1--];
			if (--len1 == 0)
			{
				System.Array.Copy(tmp, tmpBase, a, dest - (len2 - 1), len2);
				return;
			}
			if (len2 == 1)
			{
				dest -= len1;
				cursor1 -= len1;
				System.Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
				a[dest] = tmp[cursor2];
				return;
			}

			int minGallop = this.MinGallop; // Use local variable for performance
			while (true)
			{
				int count1 = 0; // Number of times in a row that first run won
				int count2 = 0; // Number of times in a row that second run won

				/*
				 * Do the straightforward thing until (if ever) one run
				 * appears to win consistently.
				 */
				do
				{
					Debug.Assert(len1 > 0 && len2 > 1);
					if (((Comparable) tmp[cursor2]).CompareTo(a[cursor1]) < 0)
					{
						a[dest--] = a[cursor1--];
						count1++;
						count2 = 0;
						if (--len1 == 0)
						{
							goto outerBreak;
						}
					}
					else
					{
						a[dest--] = tmp[cursor2--];
						count2++;
						count1 = 0;
						if (--len2 == 1)
						{
							goto outerBreak;
						}
					}
				} while ((count1 | count2) < minGallop);

				/*
				 * One run is winning so consistently that galloping may be a
				 * huge win. So try that, and continue galloping until (if ever)
				 * neither run appears to be winning consistently anymore.
				 */
				do
				{
					Debug.Assert(len1 > 0 && len2 > 1);
					count1 = len1 - GallopRight((Comparable) tmp[cursor2], a, base1, len1, len1 - 1);
					if (count1 != 0)
					{
						dest -= count1;
						cursor1 -= count1;
						len1 -= count1;
						System.Array.Copy(a, cursor1 + 1, a, dest + 1, count1);
						if (len1 == 0)
						{
							goto outerBreak;
						}
					}
					a[dest--] = tmp[cursor2--];
					if (--len2 == 1)
					{
						goto outerBreak;
					}

					count2 = len2 - GallopLeft((Comparable) a[cursor1], tmp, tmpBase, len2, len2 - 1);
					if (count2 != 0)
					{
						dest -= count2;
						cursor2 -= count2;
						len2 -= count2;
						System.Array.Copy(tmp, cursor2 + 1, a, dest + 1, count2);
						if (len2 <= 1)
						{
							goto outerBreak; // len2 == 1 || len2 == 0
						}
					}
					a[dest--] = a[cursor1--];
					if (--len1 == 0)
					{
						goto outerBreak;
					}
					minGallop--;
				} while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
				if (minGallop < 0)
				{
					minGallop = 0;
				}
				minGallop += 2; // Penalize for leaving gallop mode
			outerContinue:;
			} // End of "outer" loop
		outerBreak:
			this.MinGallop = minGallop < 1 ? 1 : minGallop; // Write back to field

			if (len2 == 1)
			{
				Debug.Assert(len1 > 0);
				dest -= len1;
				cursor1 -= len1;
				System.Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
				a[dest] = tmp[cursor2]; // Move first elt of run2 to front of merge
			}
			else if (len2 == 0)
			{
				throw new IllegalArgumentException("Comparison method violates its general contract!");
			}
			else
			{
				Debug.Assert(len1 == 0);
				Debug.Assert(len2 > 0);
				System.Array.Copy(tmp, tmpBase, a, dest - (len2 - 1), len2);
			}
		}

		/// <summary>
		/// Ensures that the external array tmp has at least the specified
		/// number of elements, increasing its size if necessary.  The size
		/// increases exponentially to ensure amortized linear time complexity.
		/// </summary>
		/// <param name="minCapacity"> the minimum required capacity of the tmp array </param>
		/// <returns> tmp, whether or not it grew </returns>
		private Object[] EnsureCapacity(int minCapacity)
		{
			if (TmpLen < minCapacity)
			{
				// Compute smallest power of 2 > minCapacity
				int newSize = minCapacity;
				newSize |= newSize >> 1;
				newSize |= newSize >> 2;
				newSize |= newSize >> 4;
				newSize |= newSize >> 8;
				newSize |= newSize >> 16;
				newSize++;

				if (newSize < 0) // Not bloody likely!
				{
					newSize = minCapacity;
				}
				else
				{
					newSize = System.Math.Min(newSize, (int)((uint)a.Length >> 1));
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "UnnecessaryLocalVariable"}) Object[] newArray = new Object[newSize];
				Object[] newArray = new Object[newSize];
				Tmp = newArray;
				TmpLen = newSize;
				TmpBase = 0;
			}
			return Tmp;
		}

	}

}