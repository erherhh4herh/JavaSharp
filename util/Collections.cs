using System;
using System.Diagnostics;
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
	/// This class consists exclusively of static methods that operate on or return
	/// collections.  It contains polymorphic algorithms that operate on
	/// collections, "wrappers", which return a new collection backed by a
	/// specified collection, and a few other odds and ends.
	/// 
	/// <para>The methods of this class all throw a <tt>NullPointerException</tt>
	/// if the collections or class objects provided to them are null.
	/// 
	/// </para>
	/// <para>The documentation for the polymorphic algorithms contained in this class
	/// generally includes a brief description of the <i>implementation</i>.  Such
	/// descriptions should be regarded as <i>implementation notes</i>, rather than
	/// parts of the <i>specification</i>.  Implementors should feel free to
	/// substitute other algorithms, so long as the specification itself is adhered
	/// to.  (For example, the algorithm used by <tt>sort</tt> does not have to be
	/// a mergesort, but it does have to be <i>stable</i>.)
	/// 
	/// </para>
	/// <para>The "destructive" algorithms contained in this class, that is, the
	/// algorithms that modify the collection on which they operate, are specified
	/// to throw <tt>UnsupportedOperationException</tt> if the collection does not
	/// support the appropriate mutation primitive(s), such as the <tt>set</tt>
	/// method.  These algorithms may, but are not required to, throw this
	/// exception if an invocation would have no effect on the collection.  For
	/// example, invoking the <tt>sort</tt> method on an unmodifiable list that is
	/// already sorted may or may not throw <tt>UnsupportedOperationException</tt>.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// </para>
	/// </summary>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Set </seealso>
	/// <seealso cref=     List </seealso>
	/// <seealso cref=     Map
	/// @since   1.2 </seealso>

	public class Collections
	{
		// Suppresses default constructor, ensuring non-instantiability.
		private Collections()
		{
		}

		// Algorithms

		/*
		 * Tuning parameters for algorithms - Many of the List algorithms have
		 * two implementations, one of which is appropriate for RandomAccess
		 * lists, the other for "sequential."  Often, the random access variant
		 * yields better performance on small sequential access lists.  The
		 * tuning parameters below determine the cutoff point for what constitutes
		 * a "small" sequential access list for each algorithm.  The values below
		 * were empirically determined to work well for LinkedList. Hopefully
		 * they should be reasonable for other sequential access List
		 * implementations.  Those doing performance work on this code would
		 * do well to validate the values of these parameters from time to time.
		 * (The first word of each tuning parameter name is the algorithm to which
		 * it applies.)
		 */
		private const int BINARYSEARCH_THRESHOLD = 5000;
		private const int REVERSE_THRESHOLD = 18;
		private const int SHUFFLE_THRESHOLD = 5;
		private const int FILL_THRESHOLD = 25;
		private const int ROTATE_THRESHOLD = 100;
		private const int COPY_THRESHOLD = 10;
		private const int REPLACEALL_THRESHOLD = 11;
		private const int INDEXOFSUBLIST_THRESHOLD = 35;

		/// <summary>
		/// Sorts the specified list into ascending order, according to the
		/// <seealso cref="Comparable natural ordering"/> of its elements.
		/// All elements in the list must implement the <seealso cref="Comparable"/>
		/// interface.  Furthermore, all elements in the list must be
		/// <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)}
		/// must not throw a {@code ClassCastException} for any elements
		/// {@code e1} and {@code e2} in the list).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>The specified list must be modifiable, but need not be resizable.
		/// 
		/// @implNote
		/// This implementation defers to the <seealso cref="List#sort(Comparator)"/>
		/// method using the specified list and a {@code null} comparator.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be sorted. </param>
		/// <exception cref="ClassCastException"> if the list contains elements that are not
		///         <i>mutually comparable</i> (for example, strings and integers). </exception>
		/// <exception cref="UnsupportedOperationException"> if the specified list's
		///         list-iterator does not support the {@code set} operation. </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the implementation
		///         detects that the natural ordering of the list elements is
		///         found to violate the <seealso cref="Comparable"/> contract </exception>
		/// <seealso cref= List#sort(Comparator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void sort(List<T> list)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> void sort(List<T> list)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void sort<T>(List<T> list) where T : Comparable<? base T>
		{
			list.sort(null);
		}

		/// <summary>
		/// Sorts the specified list according to the order induced by the
		/// specified comparator.  All elements in the list must be <i>mutually
		/// comparable</i> using the specified comparator (that is,
		/// {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		/// for any elements {@code e1} and {@code e2} in the list).
		/// 
		/// <para>This sort is guaranteed to be <i>stable</i>:  equal elements will
		/// not be reordered as a result of the sort.
		/// 
		/// </para>
		/// <para>The specified list must be modifiable, but need not be resizable.
		/// 
		/// @implNote
		/// This implementation defers to the <seealso cref="List#sort(Comparator)"/>
		/// method using the specified list and comparator.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be sorted. </param>
		/// <param name="c"> the comparator to determine the order of the list.  A
		///        {@code null} value indicates that the elements' <i>natural
		///        ordering</i> should be used. </param>
		/// <exception cref="ClassCastException"> if the list contains elements that are not
		///         <i>mutually comparable</i> using the specified comparator. </exception>
		/// <exception cref="UnsupportedOperationException"> if the specified list's
		///         list-iterator does not support the {@code set} operation. </exception>
		/// <exception cref="IllegalArgumentException"> (optional) if the comparator is
		///         found to violate the <seealso cref="Comparator"/> contract </exception>
		/// <seealso cref= List#sort(Comparator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public static <T> void sort(List<T> list, Comparator<? base T> c)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public static void sort<T, T1>(List<T> list, Comparator<T1> c)
		{
			list.sort(c);
		}


		/// <summary>
		/// Searches the specified list for the specified object using the binary
		/// search algorithm.  The list must be sorted into ascending order
		/// according to the <seealso cref="Comparable natural ordering"/> of its
		/// elements (as by the <seealso cref="#sort(List)"/> method) prior to making this
		/// call.  If it is not sorted, the results are undefined.  If the list
		/// contains multiple elements equal to the specified object, there is no
		/// guarantee which one will be found.
		/// 
		/// <para>This method runs in log(n) time for a "random access" list (which
		/// provides near-constant-time positional access).  If the specified list
		/// does not implement the <seealso cref="RandomAccess"/> interface and is large,
		/// this method will do an iterator-based binary search that performs
		/// O(n) link traversals and O(log n) element comparisons.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be searched. </param>
		/// <param name="key"> the key to be searched for. </param>
		/// <returns> the index of the search key, if it is contained in the list;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the list: the index of the first
		///         element greater than the key, or <tt>list.size()</tt> if all
		///         elements in the list are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the list contains elements that are not
		///         <i>mutually comparable</i> (for example, strings and
		///         integers), or the search key is not mutually comparable
		///         with the elements of the list. </exception>
		public static int binarySearch<T, T1>(List<T1> list, T key) where T1 : Comparable<T1 base T>
		{
			if (list is RandomAccess || list.Count < BINARYSEARCH_THRESHOLD)
			{
				return Collections.IndexedBinarySearch(list, key);
			}
			else
			{
				return Collections.IteratorBinarySearch(list, key);
			}
		}

		private static int indexedBinarySearch<T, T1>(List<T1> list, T key) where T1 : Comparable<T1 base T>
		{
			int low = 0;
			int high = list.Count - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base T> midVal = list.get(mid);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Comparable<?> midVal = list.Get(mid);
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
			return -(low + 1); // key not found
		}

		private static int iteratorBinarySearch<T, T1>(List<T1> list, T key) where T1 : Comparable<T1 base T>
		{
			int low = 0;
			int high = list.Count - 1;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<? extends Comparable<? base T>> i = list.listIterator();
			ListIterator<?> i = list.ListIterator();

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparable<? base T> midVal = get(i, mid);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Comparable<?> midVal = Get(i, mid);
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
			return -(low + 1); // key not found
		}

		/// <summary>
		/// Gets the ith element from the given list by repositioning the specified
		/// list listIterator.
		/// </summary>
		private static T get<T, T1>(ListIterator<T1> i, int index) where T1 : T
		{
			T obj = null;
			int pos = i.NextIndex();
			if (pos <= index)
			{
				do
				{
					obj = i.Next();
				} while (pos++ < index);
			}
			else
			{
				do
				{
					obj = i.Previous();
				} while (--pos > index);
			}
			return obj;
		}

		/// <summary>
		/// Searches the specified list for the specified object using the binary
		/// search algorithm.  The list must be sorted into ascending order
		/// according to the specified comparator (as by the
		/// <seealso cref="#sort(List, Comparator) sort(List, Comparator)"/>
		/// method), prior to making this call.  If it is
		/// not sorted, the results are undefined.  If the list contains multiple
		/// elements equal to the specified object, there is no guarantee which one
		/// will be found.
		/// 
		/// <para>This method runs in log(n) time for a "random access" list (which
		/// provides near-constant-time positional access).  If the specified list
		/// does not implement the <seealso cref="RandomAccess"/> interface and is large,
		/// this method will do an iterator-based binary search that performs
		/// O(n) link traversals and O(log n) element comparisons.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be searched. </param>
		/// <param name="key"> the key to be searched for. </param>
		/// <param name="c"> the comparator by which the list is ordered.
		///         A <tt>null</tt> value indicates that the elements'
		///         <seealso cref="Comparable natural ordering"/> should be used. </param>
		/// <returns> the index of the search key, if it is contained in the list;
		///         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		///         <i>insertion point</i> is defined as the point at which the
		///         key would be inserted into the list: the index of the first
		///         element greater than the key, or <tt>list.size()</tt> if all
		///         elements in the list are less than the specified key.  Note
		///         that this guarantees that the return value will be &gt;= 0 if
		///         and only if the key is found. </returns>
		/// <exception cref="ClassCastException"> if the list contains elements that are not
		///         <i>mutually comparable</i> using the specified comparator,
		///         or the search key is not mutually comparable with the
		///         elements of the list using this comparator. </exception>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		SuppressWarnings("unchecked") public static <T> int binarySearch(List<? extends T> list, T key, Comparator<? base T> c)
		{
			if (c == null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return binarySearch((List<? extends Comparable<? base T>>) list, key);
				return BinarySearch((List<?>) list, key);
			}

			if (list is RandomAccess || list.Count < BINARYSEARCH_THRESHOLD)
			{
				return Collections.IndexedBinarySearch(list, key, c);
			}
			else
			{
				return Collections.IteratorBinarySearch(list, key, c);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> int indexedBinarySearch(List<? extends T> l, T key, Comparator<? base T> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static <T> int IndexedBinarySearch(List<?> l, T key, Comparator<?> c)
		{
			int low = 0;
			int high = l.size() - 1;

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				T midVal = l.get(mid);
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
			return -(low + 1); // key not found
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static <T> int iteratorBinarySearch(List<? extends T> l, T key, Comparator<? base T> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static <T> int IteratorBinarySearch(List<?> l, T key, Comparator<?> c)
		{
			int low = 0;
			int high = l.size() - 1;
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<? extends T> i = l.listIterator();
			ListIterator<?> i = l.GetEnumerator();

			while (low <= high)
			{
				int mid = (int)((uint)(low + high) >> 1);
				T midVal = Get(i, mid);
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
			return -(low + 1); // key not found
		}

		/// <summary>
		/// Reverses the order of the elements in the specified list.<para>
		/// 
		/// This method runs in linear time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="list"> the list whose elements are to be reversed. </param>
		/// <exception cref="UnsupportedOperationException"> if the specified list or
		///         its list-iterator does not support the <tt>set</tt> operation. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) public static void reverse(List<?> list)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void reverse(List<?> list)
		{
			int size = list.Count;
			if (size < REVERSE_THRESHOLD || list is RandomAccess)
			{
				for (int i = 0, mid = size >> 1, j = size-1; i < mid; i++, j--)
				{
					Swap(list, i, j);
				}
			}
			else
			{
				// instead of using a raw type here, it's possible to capture
				// the wildcard but it will require a call to a supplementary
				// private method
				ListIterator fwd = list.ListIterator();
				ListIterator rev = list.ListIterator(size);
				for (int i = 0, mid = list.Count >> 1; i < mid; i++)
				{
					Object tmp = fwd.next();
					fwd.set(rev.previous());
					rev.set(tmp);
				}
			}
		}

		/// <summary>
		/// Randomly permutes the specified list using a default source of
		/// randomness.  All permutations occur with approximately equal
		/// likelihood.
		/// 
		/// <para>The hedge "approximately" is used in the foregoing description because
		/// default source of randomness is only approximately an unbiased source
		/// of independently chosen bits. If it were a perfect source of randomly
		/// chosen bits, then the algorithm would choose permutations with perfect
		/// uniformity.
		/// 
		/// </para>
		/// <para>This implementation traverses the list backwards, from the last
		/// element up to the second, repeatedly swapping a randomly selected element
		/// into the "current position".  Elements are randomly selected from the
		/// portion of the list that runs from the first element to the current
		/// position, inclusive.
		/// 
		/// </para>
		/// <para>This method runs in linear time.  If the specified list does not
		/// implement the <seealso cref="RandomAccess"/> interface and is large, this
		/// implementation dumps the specified list into an array before shuffling
		/// it, and dumps the shuffled array back into the list.  This avoids the
		/// quadratic behavior that would result from shuffling a "sequential
		/// access" list in place.
		/// 
		/// </para>
		/// </summary>
		/// <param name="list"> the list to be shuffled. </param>
		/// <exception cref="UnsupportedOperationException"> if the specified list or
		///         its list-iterator does not support the <tt>set</tt> operation. </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static void shuffle(List<?> list)
		public static void shuffle(List<?> list)
		{
			Random rnd = r;
			if (rnd == null)
			{
				r = rnd = new Random(); // harmless race.
			}
			Shuffle(list, rnd);
		}

		private static Random r;

		/// <summary>
		/// Randomly permute the specified list using the specified source of
		/// randomness.  All permutations occur with equal likelihood
		/// assuming that the source of randomness is fair.<para>
		/// 
		/// This implementation traverses the list backwards, from the last element
		/// up to the second, repeatedly swapping a randomly selected element into
		/// the "current position".  Elements are randomly selected from the
		/// portion of the list that runs from the first element to the current
		/// </para>
		/// position, inclusive.<para>
		/// 
		/// This method runs in linear time.  If the specified list does not
		/// implement the <seealso cref="RandomAccess"/> interface and is large, this
		/// implementation dumps the specified list into an array before shuffling
		/// it, and dumps the shuffled array back into the list.  This avoids the
		/// quadratic behavior that would result from shuffling a "sequential
		/// access" list in place.
		/// 
		/// </para>
		/// </summary>
		/// <param name="list"> the list to be shuffled. </param>
		/// <param name="rnd"> the source of randomness to use to shuffle the list. </param>
		/// <exception cref="UnsupportedOperationException"> if the specified list or its
		///         list-iterator does not support the <tt>set</tt> operation. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) public static void shuffle(List<?> list, Random rnd)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void shuffle(List<?> list, Random rnd)
		{
			int size = list.Count;
			if (size < SHUFFLE_THRESHOLD || list is RandomAccess)
			{
				for (int i = size; i > 1; i--)
				{
					Swap(list, i - 1, rnd.Next(i));
				}
			}
			else
			{
				Object[] arr = list.ToArray();

				// Shuffle array
				for (int i = size; i > 1; i--)
				{
					Swap(arr, i - 1, rnd.Next(i));
				}

				// Dump array back into list
				// instead of using a raw type here, it's possible to capture
				// the wildcard but it will require a call to a supplementary
				// private method
				ListIterator it = list.ListIterator();
				for (int i = 0; i < arr.Length; i++)
				{
					it.next();
					it.set(arr[i]);
				}
			}
		}

		/// <summary>
		/// Swaps the elements at the specified positions in the specified list.
		/// (If the specified positions are equal, invoking this method leaves
		/// the list unchanged.)
		/// </summary>
		/// <param name="list"> The list in which to swap elements. </param>
		/// <param name="i"> the index of one element to be swapped. </param>
		/// <param name="j"> the index of the other element to be swapped. </param>
		/// <exception cref="IndexOutOfBoundsException"> if either <tt>i</tt> or <tt>j</tt>
		///         is out of range (i &lt; 0 || i &gt;= list.size()
		///         || j &lt; 0 || j &gt;= list.size()).
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) public static void swap(List<?> list, int i, int j)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static void swap(List<?> list, int i, int j)
		{
			// instead of using a raw type here, it's possible to capture
			// the wildcard but it will require a call to a supplementary
			// private method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List l = list;
			List l = list;
			l.set(i, l.set(j, l.get(i)));
		}

		/// <summary>
		/// Swaps the two specified elements in the specified array.
		/// </summary>
		private static void swap(Object[] arr, int i, int j)
		{
			Object tmp = arr[i];
			arr[i] = arr[j];
			arr[j] = tmp;
		}

		/// <summary>
		/// Replaces all of the elements of the specified list with the specified
		/// element. <para>
		/// 
		/// This method runs in linear time.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be filled with the specified element. </param>
		/// <param name="obj"> The element with which to fill the specified list. </param>
		/// <exception cref="UnsupportedOperationException"> if the specified list or its
		///         list-iterator does not support the <tt>set</tt> operation. </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> void fill(List<? base T> list, T obj)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static <T> void fill(List<?> list, T obj)
		{
			int size = list.Count;

			if (size < FILL_THRESHOLD || list is RandomAccess)
			{
				for (int i = 0; i < size; i++)
				{
					list.Set(i, obj);
				}
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ListIterator<? base T> itr = list.listIterator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ListIterator<?> itr = list.ListIterator();
				for (int i = 0; i < size; i++)
				{
					itr.Next();
					itr.Set(obj);
				}
			}
		}

		/// <summary>
		/// Copies all of the elements from one list into another.  After the
		/// operation, the index of each copied element in the destination list
		/// will be identical to its index in the source list.  The destination
		/// list must be at least as long as the source list.  If it is longer, the
		/// remaining elements in the destination list are unaffected. <para>
		/// 
		/// This method runs in linear time.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the lists </param>
		/// <param name="dest"> The destination list. </param>
		/// <param name="src"> The source list. </param>
		/// <exception cref="IndexOutOfBoundsException"> if the destination list is too small
		///         to contain the entire source List. </exception>
		/// <exception cref="UnsupportedOperationException"> if the destination list's
		///         list-iterator does not support the <tt>set</tt> operation. </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> void copy(List<? base T> dest, List<? extends T> src)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static <T> void copy(List<?> dest, List<?> src)
		{
			int srcSize = src.size();
			if (srcSize > dest.size())
			{
				throw new IndexOutOfBoundsException("Source does not fit in dest");
			}

			if (srcSize < COPY_THRESHOLD || (src is RandomAccess && dest is RandomAccess))
			{
				for (int i = 0; i < srcSize; i++)
				{
					dest.set(i, src.get(i));
				}
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ListIterator<? base T> di=dest.listIterator();
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ListIterator<?> di = dest.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<? extends T> si=src.listIterator();
				ListIterator<?> si = src.GetEnumerator();
				for (int i = 0; i < srcSize; i++)
				{
					di.Next();
					di.Set(si.Next());
				}
			}
		}

		/// <summary>
		/// Returns the minimum element of the given collection, according to the
		/// <i>natural ordering</i> of its elements.  All elements in the
		/// collection must implement the <tt>Comparable</tt> interface.
		/// Furthermore, all elements in the collection must be <i>mutually
		/// comparable</i> (that is, <tt>e1.compareTo(e2)</tt> must not throw a
		/// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		/// <tt>e2</tt> in the collection).<para>
		/// 
		/// This method iterates over the entire collection, hence it requires
		/// time proportional to the size of the collection.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="coll"> the collection whose minimum element is to be determined. </param>
		/// <returns> the minimum element of the given collection, according
		///         to the <i>natural ordering</i> of its elements. </returns>
		/// <exception cref="ClassCastException"> if the collection contains elements that are
		///         not <i>mutually comparable</i> (for example, strings and
		///         integers). </exception>
		/// <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T extends Object & Comparable<? base T>> T min(Collection<? extends T> coll)
		public static <T> T Min(Collection<?> coll)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i = coll.iterator();
			Iterator<?> i = coll.GetEnumerator();
			T candidate = i.Next();

			while (i.HasNext())
			{
				T next = i.Next();
				if (next.CompareTo(candidate) < 0)
				{
					candidate = next;
				}
			}
			return candidate;
		}

		/// <summary>
		/// Returns the minimum element of the given collection, according to the
		/// order induced by the specified comparator.  All elements in the
		/// collection must be <i>mutually comparable</i> by the specified
		/// comparator (that is, <tt>comp.compare(e1, e2)</tt> must not throw a
		/// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		/// <tt>e2</tt> in the collection).<para>
		/// 
		/// This method iterates over the entire collection, hence it requires
		/// time proportional to the size of the collection.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="coll"> the collection whose minimum element is to be determined. </param>
		/// <param name="comp"> the comparator with which to determine the minimum element.
		///         A <tt>null</tt> value indicates that the elements' <i>natural
		///         ordering</i> should be used. </param>
		/// <returns> the minimum element of the given collection, according
		///         to the specified comparator. </returns>
		/// <exception cref="ClassCastException"> if the collection contains elements that are
		///         not <i>mutually comparable</i> using the specified comparator. </exception>
		/// <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public static <T> T min(Collection<? extends T> coll, Comparator<? base T> comp)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static <T> T Min(Collection<?> coll, Comparator<?> comp)
		{
			if (comp == null)
			{
				return (T)Min((Collection) coll);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i = coll.iterator();
			Iterator<?> i = coll.GetEnumerator();
			T candidate = i.Next();

			while (i.HasNext())
			{
				T next = i.Next();
				if (comp.Compare(next, candidate) < 0)
				{
					candidate = next;
				}
			}
			return candidate;
		}

		/// <summary>
		/// Returns the maximum element of the given collection, according to the
		/// <i>natural ordering</i> of its elements.  All elements in the
		/// collection must implement the <tt>Comparable</tt> interface.
		/// Furthermore, all elements in the collection must be <i>mutually
		/// comparable</i> (that is, <tt>e1.compareTo(e2)</tt> must not throw a
		/// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		/// <tt>e2</tt> in the collection).<para>
		/// 
		/// This method iterates over the entire collection, hence it requires
		/// time proportional to the size of the collection.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="coll"> the collection whose maximum element is to be determined. </param>
		/// <returns> the maximum element of the given collection, according
		///         to the <i>natural ordering</i> of its elements. </returns>
		/// <exception cref="ClassCastException"> if the collection contains elements that are
		///         not <i>mutually comparable</i> (for example, strings and
		///         integers). </exception>
		/// <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T extends Object & Comparable<? base T>> T max(Collection<? extends T> coll)
		public static <T> T Max(Collection<?> coll)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i = coll.iterator();
			Iterator<?> i = coll.GetEnumerator();
			T candidate = i.Next();

			while (i.HasNext())
			{
				T next = i.Next();
				if (next.CompareTo(candidate) > 0)
				{
					candidate = next;
				}
			}
			return candidate;
		}

		/// <summary>
		/// Returns the maximum element of the given collection, according to the
		/// order induced by the specified comparator.  All elements in the
		/// collection must be <i>mutually comparable</i> by the specified
		/// comparator (that is, <tt>comp.compare(e1, e2)</tt> must not throw a
		/// <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		/// <tt>e2</tt> in the collection).<para>
		/// 
		/// This method iterates over the entire collection, hence it requires
		/// time proportional to the size of the collection.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="coll"> the collection whose maximum element is to be determined. </param>
		/// <param name="comp"> the comparator with which to determine the maximum element.
		///         A <tt>null</tt> value indicates that the elements' <i>natural
		///        ordering</i> should be used. </param>
		/// <returns> the maximum element of the given collection, according
		///         to the specified comparator. </returns>
		/// <exception cref="ClassCastException"> if the collection contains elements that are
		///         not <i>mutually comparable</i> using the specified comparator. </exception>
		/// <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public static <T> T max(Collection<? extends T> coll, Comparator<? base T> comp)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static <T> T Max(Collection<?> coll, Comparator<?> comp)
		{
			if (comp == null)
			{
				return (T)Max((Collection) coll);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i = coll.iterator();
			Iterator<?> i = coll.GetEnumerator();
			T candidate = i.Next();

			while (i.HasNext())
			{
				T next = i.Next();
				if (comp.Compare(next, candidate) > 0)
				{
					candidate = next;
				}
			}
			return candidate;
		}

		/// <summary>
		/// Rotates the elements in the specified list by the specified distance.
		/// After calling this method, the element at index <tt>i</tt> will be
		/// the element previously at index <tt>(i - distance)</tt> mod
		/// <tt>list.size()</tt>, for all values of <tt>i</tt> between <tt>0</tt>
		/// and <tt>list.size()-1</tt>, inclusive.  (This method has no effect on
		/// the size of the list.)
		/// 
		/// <para>For example, suppose <tt>list</tt> comprises<tt> [t, a, n, k, s]</tt>.
		/// After invoking <tt>Collections.rotate(list, 1)</tt> (or
		/// <tt>Collections.rotate(list, -4)</tt>), <tt>list</tt> will comprise
		/// <tt>[s, t, a, n, k]</tt>.
		/// 
		/// </para>
		/// <para>Note that this method can usefully be applied to sublists to
		/// move one or more elements within a list while preserving the
		/// order of the remaining elements.  For example, the following idiom
		/// moves the element at index <tt>j</tt> forward to position
		/// <tt>k</tt> (which must be greater than or equal to <tt>j</tt>):
		/// <pre>
		///     Collections.rotate(list.subList(j, k+1), -1);
		/// </pre>
		/// To make this concrete, suppose <tt>list</tt> comprises
		/// <tt>[a, b, c, d, e]</tt>.  To move the element at index <tt>1</tt>
		/// (<tt>b</tt>) forward two positions, perform the following invocation:
		/// <pre>
		///     Collections.rotate(l.subList(1, 4), -1);
		/// </pre>
		/// The resulting list is <tt>[a, c, d, b, e]</tt>.
		/// 
		/// </para>
		/// <para>To move more than one element forward, increase the absolute value
		/// of the rotation distance.  To move elements backward, use a positive
		/// shift distance.
		/// 
		/// </para>
		/// <para>If the specified list is small or implements the {@link
		/// RandomAccess} interface, this implementation exchanges the first
		/// element into the location it should go, and then repeatedly exchanges
		/// the displaced element into the location it should go until a displaced
		/// element is swapped into the first element.  If necessary, the process
		/// is repeated on the second and successive elements, until the rotation
		/// is complete.  If the specified list is large and doesn't implement the
		/// <tt>RandomAccess</tt> interface, this implementation breaks the
		/// list into two sublist views around index <tt>-distance mod size</tt>.
		/// Then the <seealso cref="#reverse(List)"/> method is invoked on each sublist view,
		/// and finally it is invoked on the entire list.  For a more complete
		/// description of both algorithms, see Section 2.3 of Jon Bentley's
		/// <i>Programming Pearls</i> (Addison-Wesley, 1986).
		/// 
		/// </para>
		/// </summary>
		/// <param name="list"> the list to be rotated. </param>
		/// <param name="distance"> the distance to rotate the list.  There are no
		///        constraints on this value; it may be zero, negative, or
		///        greater than <tt>list.size()</tt>. </param>
		/// <exception cref="UnsupportedOperationException"> if the specified list or
		///         its list-iterator does not support the <tt>set</tt> operation.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static void rotate(List<?> list, int distance)
		public static void rotate(List<?> list, int distance)
		{
			if (list is RandomAccess || list.Count < ROTATE_THRESHOLD)
			{
				Rotate1(list, distance);
			}
			else
			{
				Rotate2(list, distance);
			}
		}

		private static <T> void rotate1(List<T> list, int distance)
		{
			int size = list.Count;
			if (size == 0)
			{
				return;
			}
			distance = distance % size;
			if (distance < 0)
			{
				distance += size;
			}
			if (distance == 0)
			{
				return;
			}

			for (int cycleStart = 0, nMoved = 0; nMoved != size; cycleStart++)
			{
				T displaced = list.Get(cycleStart);
				int i = cycleStart;
				do
				{
					i += distance;
					if (i >= size)
					{
						i -= size;
					}
					displaced = list.Set(i, displaced);
					nMoved++;
				} while (i != cycleStart);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static void rotate2(List<?> list, int distance)
		private static void rotate2(List<?> list, int distance)
		{
			int size = list.Count;
			if (size == 0)
			{
				return;
			}
			int mid = -distance % size;
			if (mid < 0)
			{
				mid += size;
			}
			if (mid == 0)
			{
				return;
			}

			Reverse(list.SubList(0, mid));
			Reverse(list.SubList(mid, size));
			Reverse(list);
		}

		/// <summary>
		/// Replaces all occurrences of one specified value in a list with another.
		/// More formally, replaces with <tt>newVal</tt> each element <tt>e</tt>
		/// in <tt>list</tt> such that
		/// <tt>(oldVal==null ? e==null : oldVal.equals(e))</tt>.
		/// (This method has no effect on the size of the list.)
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list in which replacement is to occur. </param>
		/// <param name="oldVal"> the old value to be replaced. </param>
		/// <param name="newVal"> the new value with which <tt>oldVal</tt> is to be
		///        replaced. </param>
		/// <returns> <tt>true</tt> if <tt>list</tt> contained one or more elements
		///         <tt>e</tt> such that
		///         <tt>(oldVal==null ?  e==null : oldVal.equals(e))</tt>. </returns>
		/// <exception cref="UnsupportedOperationException"> if the specified list or
		///         its list-iterator does not support the <tt>set</tt> operation.
		/// @since  1.4 </exception>
		public static <T> bool ReplaceAll(List<T> list, T oldVal, T newVal)
		{
			bool result = false;
			int size = list.Count;
			if (size < REPLACEALL_THRESHOLD || list is RandomAccess)
			{
				if (oldVal == null)
				{
					for (int i = 0; i < size; i++)
					{
						if (list.Get(i) == null)
						{
							list.Set(i, newVal);
							result = true;
						}
					}
				}
				else
				{
					for (int i = 0; i < size; i++)
					{
						if (oldVal.Equals(list.Get(i)))
						{
							list.Set(i, newVal);
							result = true;
						}
					}
				}
			}
			else
			{
				ListIterator<T> itr = list.ListIterator();
				if (oldVal == null)
				{
					for (int i = 0; i < size; i++)
					{
						if (itr.Next() == null)
						{
							itr.Set(newVal);
							result = true;
						}
					}
				}
				else
				{
					for (int i = 0; i < size; i++)
					{
						if (oldVal.Equals(itr.Next()))
						{
							itr.Set(newVal);
							result = true;
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the starting position of the first occurrence of the specified
		/// target list within the specified source list, or -1 if there is no
		/// such occurrence.  More formally, returns the lowest index <tt>i</tt>
		/// such that {@code source.subList(i, i+target.size()).equals(target)},
		/// or -1 if there is no such index.  (Returns -1 if
		/// {@code target.size() > source.size()})
		/// 
		/// <para>This implementation uses the "brute force" technique of scanning
		/// over the source list, looking for a match with the target at each
		/// location in turn.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> the list in which to search for the first occurrence
		///        of <tt>target</tt>. </param>
		/// <param name="target"> the list to search for as a subList of <tt>source</tt>. </param>
		/// <returns> the starting position of the first occurrence of the specified
		///         target list within the specified source list, or -1 if there
		///         is no such occurrence.
		/// @since  1.4 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static int indexOfSubList(List<?> source, List<?> target)
		public static int IndexOfSubList(List<?> source, List<?> target)
		{
			int sourceSize = source.size();
			int targetSize = target.size();
			int maxCandidate = sourceSize - targetSize;

			if (sourceSize < INDEXOFSUBLIST_THRESHOLD || (source is RandomAccess && target is RandomAccess))
			{
				for (int candidate = 0; candidate <= maxCandidate; candidate++)
				{
					for (int i = 0, j = candidate; i < targetSize; i++, j++)
					{
						if (!Eq(target.get(i), source.get(j)))
						{
							goto nextCandContinue; // Element mismatch, try next cand
						}
					}
					return candidate; // All elements of candidate matched target
				nextCandContinue:;
				}
			nextCandBreak:;
			} // Iterator version of above algorithm
			else
			{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<?> si = source.listIterator();
				ListIterator<?> si = source.GetEnumerator();
				for (int candidate = 0; candidate <= maxCandidate; candidate++)
				{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<?> ti = target.listIterator();
					ListIterator<?> ti = target.GetEnumerator();
					for (int i = 0; i < targetSize; i++)
					{
						if (!Eq(ti.Next(), si.Next()))
						{
							// Back up source iterator to next candidate
							for (int j = 0; j < i; j++)
							{
								si.Previous();
							}
							goto nextCandContinue;
						}
					}
					return candidate;
				nextCandContinue:;
				}
			nextCandBreak:;
			}
			return -1; // No candidate matched the target
		}

		/// <summary>
		/// Returns the starting position of the last occurrence of the specified
		/// target list within the specified source list, or -1 if there is no such
		/// occurrence.  More formally, returns the highest index <tt>i</tt>
		/// such that {@code source.subList(i, i+target.size()).equals(target)},
		/// or -1 if there is no such index.  (Returns -1 if
		/// {@code target.size() > source.size()})
		/// 
		/// <para>This implementation uses the "brute force" technique of iterating
		/// over the source list, looking for a match with the target at each
		/// location in turn.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> the list in which to search for the last occurrence
		///        of <tt>target</tt>. </param>
		/// <param name="target"> the list to search for as a subList of <tt>source</tt>. </param>
		/// <returns> the starting position of the last occurrence of the specified
		///         target list within the specified source list, or -1 if there
		///         is no such occurrence.
		/// @since  1.4 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static int lastIndexOfSubList(List<?> source, List<?> target)
		public static int LastIndexOfSubList(List<?> source, List<?> target)
		{
			int sourceSize = source.size();
			int targetSize = target.size();
			int maxCandidate = sourceSize - targetSize;

			if (sourceSize < INDEXOFSUBLIST_THRESHOLD || source is RandomAccess) // Index access version
			{
				for (int candidate = maxCandidate; candidate >= 0; candidate--)
				{
					for (int i = 0, j = candidate; i < targetSize; i++, j++)
					{
						if (!Eq(target.get(i), source.get(j)))
						{
							goto nextCandContinue; // Element mismatch, try next cand
						}
					}
					return candidate; // All elements of candidate matched target
				nextCandContinue:;
				}
			nextCandBreak:;
			} // Iterator version of above algorithm
			else
			{
				if (maxCandidate < 0)
				{
					return -1;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<?> si = source.listIterator(maxCandidate);
				ListIterator<?> si = source.listIterator(maxCandidate);
				for (int candidate = maxCandidate; candidate >= 0; candidate--)
				{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ListIterator<?> ti = target.listIterator();
					ListIterator<?> ti = target.GetEnumerator();
					for (int i = 0; i < targetSize; i++)
					{
						if (!Eq(ti.Next(), si.Next()))
						{
							if (candidate != 0)
							{
								// Back up source iterator to next candidate
								for (int j = 0; j <= i + 1; j++)
								{
									si.Previous();
								}
							}
							goto nextCandContinue;
						}
					}
					return candidate;
				nextCandContinue:;
				}
			nextCandBreak:;
			}
			return -1; // No candidate matched the target
		}


		// Unmodifiable Wrappers

		/// <summary>
		/// Returns an unmodifiable view of the specified collection.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// collections.  Query operations on the returned collection "read through"
		/// to the specified collection, and attempts to modify the returned
		/// collection, whether direct or via its iterator, result in an
		/// <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned collection does <i>not</i> pass the hashCode and equals
		/// operations through to the backing collection, but relies on
		/// <tt>Object</tt>'s <tt>equals</tt> and <tt>hashCode</tt> methods.  This
		/// is necessary to preserve the contracts of these operations in the case
		/// </para>
		/// that the backing collection is a set or a list.<para>
		/// 
		/// The returned collection will be serializable if the specified collection
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="c"> the collection for which an unmodifiable view is to be
		///         returned. </param>
		/// <returns> an unmodifiable view of the specified collection. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Collection<T> unmodifiableCollection(Collection<? extends T> c)
		public static <T> Collection<T> UnmodifiableCollection(Collection<?> c)
		{
			return new UnmodifiableCollection<>(c);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableCollection<E> implements Collection<E>, Serializable
		{
			private static final long serialVersionUID = 1820017752578914078L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Collection<? extends E> c;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Collection<?> c;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableCollection(Collection<? extends E> c)
			UnmodifiableCollection(Collection<?> c)
			{
				if (c == null)
				{
					throw new NullPointerException();
				}
				this.c = c;
			}

			public int size()
			{
				return c.Size();
			}
			public bool Empty
			{
				return c.Empty;
			}
			public bool contains(Object o)
			{
				return c.Contains(o);
			}
			public Object[] toArray()
			{
				return c.ToArray();
			}
			public <T> T[] toArray(T[] a)
			{
				return c.ToArray(a);
			}
			public String ToString()
			{
				return c.ToString();
			}

			public Iterator<E> iterator()
			{
				return new IteratorAnonymousInnerClassHelper(c);
			}

			public bool add(E e)
			{
				throw new UnsupportedOperationException();
			}
			public bool remove(Object o)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> coll)
			public bool containsAll(Collection<?> coll)
			{
				return c.ContainsAll(coll);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(Collection<? extends E> coll)
			public bool addAll(Collection<?> coll)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> coll)
			public bool removeAll(Collection<?> coll)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> coll)
			public bool retainAll(Collection<?> coll)
			{
				throw new UnsupportedOperationException();
			}
			public void clear()
			{
				throw new UnsupportedOperationException();
			}

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				c.forEach(action);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public Spliterator<E> spliterator()
			public Spliterator<E> spliterator()
			{
				return (Spliterator<E>)c.spliterator();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.stream.Stream<E> stream()
			public Stream<E> stream()
			{
				return (Stream<E>)c.stream();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.stream.Stream<E> parallelStream()
			public Stream<E> parallelStream()
			{
				return (Stream<E>)c.parallelStream();
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified set.  This method allows
		/// modules to provide users with "read-only" access to internal sets.
		/// Query operations on the returned set "read through" to the specified
		/// set, and attempts to modify the returned set, whether direct or via its
		/// iterator, result in an <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned set will be serializable if the specified set
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the set for which an unmodifiable view is to be returned. </param>
		/// <returns> an unmodifiable view of the specified set. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Set<T> unmodifiableSet(Set<? extends T> s)
		public static <T> Set<T> UnmodifiableSet(Set<?> s)
		{
			return new UnmodifiableSet<>(s);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableSet<E> extends UnmodifiableCollection<E> implements Set<E>, Serializable
		{
			private static final long serialVersionUID = -9215047833775013803L;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableSet(Set<? extends E> s)
			UnmodifiableSet(Set<?> s)
			{
				base(s);
			}
			public bool Equals(Object o)
			{
				return o == this || c.Equals(o);
			}
			public int GetHashCode()
			{
				return c.HashCode();
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified sorted set.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// sorted sets.  Query operations on the returned sorted set "read
		/// through" to the specified sorted set.  Attempts to modify the returned
		/// sorted set, whether direct, via its iterator, or via its
		/// <tt>subSet</tt>, <tt>headSet</tt>, or <tt>tailSet</tt> views, result in
		/// an <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned sorted set will be serializable if the specified sorted set
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the sorted set for which an unmodifiable view is to be
		///        returned. </param>
		/// <returns> an unmodifiable view of the specified sorted set. </returns>
		public static <T> SortedSet<T> UnmodifiableSortedSet(SortedSet<T> s)
		{
			return new UnmodifiableSortedSet<>(s);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableSortedSet<E> extends UnmodifiableSet<E> implements SortedSet<E>, Serializable
		{
			private static final long serialVersionUID = -4929149591599911165L;
			private final SortedSet<E> ss;

			UnmodifiableSortedSet(SortedSet<E> s)
			{
				base(s);
				ss = s;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				return ss.comparator();
			}

			public SortedSet<E> subSet(E fromElement, E toElement)
			{
				return new UnmodifiableSortedSet<>(ss.subSet(fromElement,toElement));
			}
			public SortedSet<E> headSet(E toElement)
			{
				return new UnmodifiableSortedSet<>(ss.headSet(toElement));
			}
			public SortedSet<E> tailSet(E fromElement)
			{
				return new UnmodifiableSortedSet<>(ss.tailSet(fromElement));
			}

			public E first()
			{
				return ss.first();
			}
			public E last()
			{
				return ss.last();
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified navigable set.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// navigable sets.  Query operations on the returned navigable set "read
		/// through" to the specified navigable set.  Attempts to modify the returned
		/// navigable set, whether direct, via its iterator, or via its
		/// {@code subSet}, {@code headSet}, or {@code tailSet} views, result in
		/// an {@code UnsupportedOperationException}.<para>
		/// 
		/// The returned navigable set will be serializable if the specified
		/// navigable set is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the navigable set for which an unmodifiable view is to be
		///        returned </param>
		/// <returns> an unmodifiable view of the specified navigable set
		/// @since 1.8 </returns>
		public static <T> NavigableSet<T> UnmodifiableNavigableSet(NavigableSet<T> s)
		{
			return new UnmodifiableNavigableSet<>(s);
		}

		/// <summary>
		/// Wraps a navigable set and disables all of the mutative operations.
		/// </summary>
		/// @param <E> type of elements
		/// @serial include </param>
		static class UnmodifiableNavigableSet<E> extends UnmodifiableSortedSet<E> implements NavigableSet<E>, Serializable
		{

			private static final long serialVersionUID = -6027448201786391929L;

			/// <summary>
			/// A singleton empty unmodifiable navigable set used for
			/// <seealso cref="#emptyNavigableSet()"/>.
			/// </summary>
			/// @param <E> type of elements, if there were any, and bounds </param>
			private static class EmptyNavigableSet<E> extends UnmodifiableNavigableSet<E> implements Serializable
			{
				private static final long serialVersionUID = -6291252904449939134L;

				public EmptyNavigableSet()
				{
					base(new TreeSet<E>());
				}

				private Object readResolve()
				{
					return EMPTY_NAVIGABLE_SET;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private static final NavigableSet<?> EMPTY_NAVIGABLE_SET = new EmptyNavigableSet<>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			private static final NavigableSet<?> EMPTY_NAVIGABLE_SET = new EmptyNavigableSet<>();

			/// <summary>
			/// The instance we are protecting.
			/// </summary>
			private final NavigableSet<E> ns;

			UnmodifiableNavigableSet(NavigableSet<E> s)
			{
				base(s);
				ns = s;
			}

			public E lower(E e)
			{
				return ns.lower(e);
			}
			public E floor(E e)
			{
				return ns.floor(e);
			}
			public E ceiling(E e)
			{
				return ns.ceiling(e);
			}
			public E higher(E e)
			{
				return ns.higher(e);
			}
			public E pollFirst()
			{
				throw new UnsupportedOperationException();
			}
			public E pollLast()
			{
				throw new UnsupportedOperationException();
			}
			public NavigableSet<E> descendingSet()
			{
						 return new UnmodifiableNavigableSet<>(ns.descendingSet());
			}
			public Iterator<E> descendingIterator()
			{
												 return descendingSet().GetEnumerator();
			}

			public NavigableSet<E> subSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
			{
				return new UnmodifiableNavigableSet<>(ns.subSet(fromElement, fromInclusive, toElement, toInclusive));
			}

			public NavigableSet<E> headSet(E toElement, bool inclusive)
			{
				return new UnmodifiableNavigableSet<>(ns.headSet(toElement, inclusive));
			}

			public NavigableSet<E> tailSet(E fromElement, bool inclusive)
			{
				return new UnmodifiableNavigableSet<>(ns.tailSet(fromElement, inclusive));
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified list.  This method allows
		/// modules to provide users with "read-only" access to internal
		/// lists.  Query operations on the returned list "read through" to the
		/// specified list, and attempts to modify the returned list, whether
		/// direct or via its iterator, result in an
		/// <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned list will be serializable if the specified list
		/// is serializable. Similarly, the returned list will implement
		/// <seealso cref="RandomAccess"/> if the specified list does.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list for which an unmodifiable view is to be returned. </param>
		/// <returns> an unmodifiable view of the specified list. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> List<T> unmodifiableList(List<? extends T> list)
		public static <T> List<T> UnmodifiableList(List<?> list)
		{
			return (list is RandomAccess ? new UnmodifiableRandomAccessList<>(list) : new UnmodifiableList<>(list));
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableList<E> extends UnmodifiableCollection<E> implements List<E>
		{
			private static final long serialVersionUID = -283967356065247728L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<? extends E> list;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			List<?> list;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableList(List<? extends E> list)
			UnmodifiableList(List<?> list)
			{
				base(list);
				this.list = list;
			}

			public bool Equals(Object o)
			{
				return o == this || list.Equals(o);
			}
			public int GetHashCode()
			{
				return list.HashCode();
			}

			public E get(int index)
			{
				return list.Get(index);
			}
			public E set(int index, E element)
			{
				throw new UnsupportedOperationException();
			}
			public void add(int index, E element)
			{
				throw new UnsupportedOperationException();
			}
			public E remove(int index)
			{
				throw new UnsupportedOperationException();
			}
			public int indexOf(Object o)
			{
				return list.IndexOf(o);
			}
			public int lastIndexOf(Object o)
			{
				return list.LastIndexOf(o);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(int index, Collection<? extends E> c)
			public bool AddAll(int index, Collection<?> c)
			{
				throw new UnsupportedOperationException();
			}

			public void replaceAll(UnaryOperator<E> @operator)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void sort(Comparator<?> c)
			{
				throw new UnsupportedOperationException();
			}

			public ListIterator<E> listIterator()
			{
				return listIterator(0);
			}

			public ListIterator<E> listIterator(final int index)
			{
				return new ListIteratorAnonymousInnerClassHelper(list);
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				return new UnmodifiableList<>(list.SubList(fromIndex, toIndex));
			}

			/// <summary>
			/// UnmodifiableRandomAccessList instances are serialized as
			/// UnmodifiableList instances to allow them to be deserialized
			/// in pre-1.4 JREs (which do not have UnmodifiableRandomAccessList).
			/// This method inverts the transformation.  As a beneficial
			/// side-effect, it also grafts the RandomAccess marker onto
			/// UnmodifiableList instances that were serialized in pre-1.4 JREs.
			/// 
			/// Note: Unfortunately, UnmodifiableRandomAccessList instances
			/// serialized in 1.4.1 and deserialized in 1.4 will become
			/// UnmodifiableList instances, as this method was missing in 1.4.
			/// </summary>
			private Object readResolve()
			{
				return (list is RandomAccess ? new UnmodifiableRandomAccessList<>(list)
						: this);
			}
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableRandomAccessList<E> extends UnmodifiableList<E> implements RandomAccess
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableRandomAccessList(List<? extends E> list)
			UnmodifiableRandomAccessList(List<?> list)
			{
				base(list);
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				return new UnmodifiableRandomAccessList<>(list.SubList(fromIndex, toIndex));
			}

			private static final long serialVersionUID = -2542308836966382001L;

			/// <summary>
			/// Allows instances to be deserialized in pre-1.4 JREs (which do
			/// not have UnmodifiableRandomAccessList).  UnmodifiableList has
			/// a readResolve method that inverts this transformation upon
			/// deserialization.
			/// </summary>
			private Object writeReplace()
			{
				return new UnmodifiableList<>(list);
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified map.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// maps.  Query operations on the returned map "read through"
		/// to the specified map, and attempts to modify the returned
		/// map, whether direct or via its collection views, result in an
		/// <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned map will be serializable if the specified map
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the map for which an unmodifiable view is to be returned. </param>
		/// <returns> an unmodifiable view of the specified map. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <K,V> Map<K,V> unmodifiableMap(Map<? extends K, ? extends V> m)
		public static <K, V> Map<K, V> UnmodifiableMap(Map<?, ?> m)
		{
			return new UnmodifiableMap<>(m);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class UnmodifiableMap<K, V> implements Map<K, V>, Serializable
		{
			private static final long serialVersionUID = -1034234728574286014L;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Map<? extends K, ? extends V> m;
			private final Map<?, ?> m;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableMap(Map<? extends K, ? extends V> m)
			UnmodifiableMap(Map<?, ?> m)
			{
				if (m == null)
				{
					throw new NullPointerException();
				}
				this.m = m;
			}

			public int size()
			{
				return m.size();
			}
			public bool Empty
			{
				return m.Empty;
			}
			public bool containsKey(Object key)
			{
				return m.containsKey(key);
			}
			public bool containsValue(Object val)
			{
				return m.containsValue(val);
			}
			public V get(Object key)
			{
				return m.get(key);
			}

			public V put(K key, V value)
			{
				throw new UnsupportedOperationException();
			}
			public V remove(Object key)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> m)
			public void putAll(Map<?, ?> m)
			{
				throw new UnsupportedOperationException();
			}
			public void clear()
			{
				throw new UnsupportedOperationException();
			}

			private transient Set<K> keySet;
			private transient Set<Map_Entry<K, V>> entrySet;
			private transient Collection<V> values;

			public Set<K> keySet()
			{
				if (keySet == null)
				{
					keySet = UnmodifiableSet(m.Keys);
				}
				return keySet;
			}

			public Set<Map_Entry<K, V>> entrySet()
			{
				if (entrySet == null)
				{
					entrySet = new UnmodifiableEntrySet<>(m.entrySet());
				}
				return entrySet;
			}

			public Collection<V> values()
			{
				if (values == null)
				{
					values = UnmodifiableCollection(m.values());
				}
				return values;
			}

			public bool Equals(Object o)
			{
				return o == this || m.Equals(o);
			}
			public int GetHashCode()
			{
				return m.HashCode();
			}
			public String ToString()
			{
				return m.ToString();
			}

			// Override default methods in Map
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public V getOrDefault(Object k, V defaultValue)
			public V getOrDefault(Object k, V defaultValue)
			{
				// Safe cast as we don't change the value
				return ((Map<K, V>)m).getOrDefault(k, defaultValue);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(BiConsumer<?, ?> action)
			{
				m.forEach(action);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void replaceAll(BiFunction<?, ?, ?> function)
			{
				throw new UnsupportedOperationException();
			}

			public V putIfAbsent(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

			public bool remove(Object key, Object value)
			{
				throw new UnsupportedOperationException();
			}

			public bool replace(K key, V oldValue, V newValue)
			{
				throw new UnsupportedOperationException();
			}

			public V replace(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfAbsent(K key, Function<?, ?> mappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfPresent(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V compute(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V merge(K key, V value, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// We need this class in addition to UnmodifiableSet as
			/// Map.Entries themselves permit modification of the backing Map
			/// via their setValue operation.  This class is subtle: there are
			/// many possible attacks that must be thwarted.
			/// 
			/// @serial include
			/// </summary>
			static class UnmodifiableEntrySet<K, V> extends UnmodifiableSet<Map_Entry<K, V>>
			{
				private static final long serialVersionUID = 7854390611657943733L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) UnmodifiableEntrySet(Set<? extends Map_Entry<? extends K, ? extends V>> s)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				UnmodifiableEntrySet(Set<?> s)
				{
					// Need to cast to raw in order to work around a limitation in the type system
					base((Set)s);
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: static <K, V> java.util.function.Consumer<Map_Entry<K, V>> entryConsumer(java.util.function.Consumer<? base Entry<K, V>> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				static <K, V> Consumer<Map_Entry<K, V>> entryConsumer(Consumer<?> action)
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					return e => action.accept(new UnmodifiableEntry<>(e));
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base Entry<K, V>> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				public void forEach(Consumer<?> action)
				{
					Objects.RequireNonNull(action);
					c.forEach(entryConsumer(action));
				}

				static final class UnmodifiableEntrySetSpliterator<K, V> implements Spliterator<Entry<K, V>>
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Spliterator<Map_Entry<K, V>> s;
					Spliterator<Map_Entry<K, V>> s;

					UnmodifiableEntrySetSpliterator(Spliterator<Entry<K, V>> s)
					{
						this.s = s;
					}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base Entry<K, V>> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					public bool tryAdvance(Consumer<?> action)
					{
						Objects.RequireNonNull(action);
						return s.TryAdvance(entryConsumer(action));
					}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base Entry<K, V>> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					public void forEachRemaining(Consumer<?> action)
					{
						Objects.RequireNonNull(action);
						s.forEachRemaining(entryConsumer(action));
					}

					public Spliterator<Entry<K, V>> trySplit()
					{
						Spliterator<Entry<K, V>> split = s.TrySplit();
						return split == null ? null : new UnmodifiableEntrySetSpliterator<>(split);
					}

					public long estimateSize()
					{
						return s.EstimateSize();
					}

					public long ExactSizeIfKnown
					{
						return s.ExactSizeIfKnown;
					}

					public int characteristics()
					{
						return s.Characteristics();
					}

					public bool hasCharacteristics(int characteristics)
					{
						return s.hasCharacteristics(characteristics);
					}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Entry<K, V>> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					public Comparator<?> Comparator
					{
						return s.Comparator;
					}
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Spliterator<Entry<K,V>> spliterator()
				public Spliterator<Entry<K, V>> spliterator()
				{
					return new UnmodifiableEntrySetSpliterator<>((Spliterator<Map_Entry<K, V>>) c.spliterator());
				}

				public Stream<Entry<K, V>> stream()
				{
					return StreamSupport.Stream(spliterator(), false);
				}

				public Stream<Entry<K, V>> parallelStream()
				{
					return StreamSupport.Stream(spliterator(), true);
				}

				public Iterator<Map_Entry<K, V>> iterator()
				{
					return new IteratorAnonymousInnerClassHelper2();
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object[] toArray()
				public Object[] toArray()
				{
					Object[] a = c.toArray();
					for (int i = 0; i < a.Length; i++)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: a[i] = new UnmodifiableEntry<>((Map_Entry<? extends K, ? extends V>)a[i]);
						a[i] = new UnmodifiableEntry<>((Map_Entry<?, ?>)a[i]);
					}
					return a;
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
				public <T> T[] toArray(T[] a)
				{
					// We don't pass a to c.toArray, to avoid window of
					// vulnerability wherein an unscrupulous multithreaded client
					// could get his hands on raw (unwrapped) Entries from c.
					Object[] arr = c.toArray(a.length == 0 ? a : Arrays.CopyOf(a, 0));

					for (int i = 0; i < arr.Length; i++)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: arr[i] = new UnmodifiableEntry<>((Map_Entry<? extends K, ? extends V>)arr[i]);
						arr[i] = new UnmodifiableEntry<>((Map_Entry<?, ?>)arr[i]);
					}

					if (arr.Length > a.length)
					{
						return (T[])arr;
					}

					System.Array.Copy(arr, 0, a, 0, arr.Length);
					if (a.length > arr.Length)
					{
						a[arr.Length] = null;
					}
					return a;
				}

				/// <summary>
				/// This method is overridden to protect the backing set against
				/// an object with a nefarious equals function that senses
				/// that the equality-candidate is Map.Entry and calls its
				/// setValue method.
				/// </summary>
				public bool contains(Object o)
				{
					if (!(o is Map_Entry))
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return c.contains(new UnmodifiableEntry<>((Map_Entry<?,?>) o));
					return c.contains(new UnmodifiableEntry<>((Map_Entry<?, ?>) o));
				}

				/// <summary>
				/// The next two methods are overridden to protect against
				/// an unscrupulous List whose contains(Object o) method senses
				/// when o is a Map.Entry, and calls o.setValue.
				/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> coll)
				public bool containsAll(Collection<?> coll)
				{
					foreach (Object e in coll)
					{
						if (!contains(e)) // Invokes safe contains() above
						{
							return false;
						}
					}
					return true;
				}
				public bool Equals(Object o)
				{
					if (o == this)
					{
						return true;
					}

					if (!(o is Set))
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> s = (Set<?>) o;
					Set<?> s = (Set<?>) o;
					if (s.Count != c.size())
					{
						return false;
					}
					return containsAll(s); // Invokes safe containsAll() above
				}

				/// <summary>
				/// This "wrapper class" serves two purposes: it prevents
				/// the client from modifying the backing Map, by short-circuiting
				/// the setValue method, and it protects the backing Map against
				/// an ill-behaved Map.Entry that attempts to modify another
				/// Map Entry when asked to perform an equality check.
				/// </summary>
				private static class UnmodifiableEntry<K, V> implements Map_Entry<K, V>
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Map_Entry<? extends K, ? extends V> e;
					private Map_Entry<?, ?> e;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableEntry(Map_Entry<? extends K, ? extends V> e)
					UnmodifiableEntry(Map_Entry<?, ?> e)
					{
								this.e = Objects.RequireNonNull(e);
					}

					public K Key
					{
						return e.Key;
					}
					public V Value
					{
						return e.Value;
					}
					public V setValue(V value)
					{
						throw new UnsupportedOperationException();
					}
					public int GetHashCode()
					{
						return e.HashCode();
					}
					public bool Equals(Object o)
					{
						if (this == o)
						{
							return true;
						}
						if (!(o is Map_Entry))
						{
							return false;
						}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> t = (Map_Entry<?,?>)o;
						Map_Entry<?, ?> t = (Map_Entry<?, ?>)o;
						return Eq(e.Key, t.Key) && Eq(e.Value, t.Value);
					}
					public String ToString()
					{
						return e.ToString();
					}
				}
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified sorted map.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// sorted maps.  Query operations on the returned sorted map "read through"
		/// to the specified sorted map.  Attempts to modify the returned
		/// sorted map, whether direct, via its collection views, or via its
		/// <tt>subMap</tt>, <tt>headMap</tt>, or <tt>tailMap</tt> views, result in
		/// an <tt>UnsupportedOperationException</tt>.<para>
		/// 
		/// The returned sorted map will be serializable if the specified sorted map
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the sorted map for which an unmodifiable view is to be
		///        returned. </param>
		/// <returns> an unmodifiable view of the specified sorted map. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <K,V> SortedMap<K,V> unmodifiableSortedMap(SortedMap<K, ? extends V> m)
		public static <K, V> SortedMap<K, V> UnmodifiableSortedMap(SortedMap<K, ?> m)
		{
			return new UnmodifiableSortedMap<>(m);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableSortedMap<K, V> extends UnmodifiableMap<K, V> implements SortedMap<K, V>, Serializable
		{
			private static final long serialVersionUID = -8806743815996713206L;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final SortedMap<K, ? extends V> sm;
			private final SortedMap<K, ?> sm;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableSortedMap(SortedMap<K, ? extends V> m)
			UnmodifiableSortedMap(SortedMap<K, ?> m)
			{
				base(m);
				sm = m;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				return sm.comparator();
			}
			public SortedMap<K, V> subMap(K fromKey, K toKey)
			{
					 return new UnmodifiableSortedMap<>(sm.subMap(fromKey, toKey));
			}
			public SortedMap<K, V> headMap(K toKey)
			{
							 return new UnmodifiableSortedMap<>(sm.headMap(toKey));
			}
			public SortedMap<K, V> tailMap(K fromKey)
			{
						   return new UnmodifiableSortedMap<>(sm.tailMap(fromKey));
			}
			public K firstKey()
			{
				return sm.firstKey();
			}
			public K lastKey()
			{
				return sm.lastKey();
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified navigable map.  This method
		/// allows modules to provide users with "read-only" access to internal
		/// navigable maps.  Query operations on the returned navigable map "read
		/// through" to the specified navigable map.  Attempts to modify the returned
		/// navigable map, whether direct, via its collection views, or via its
		/// {@code subMap}, {@code headMap}, or {@code tailMap} views, result in
		/// an {@code UnsupportedOperationException}.<para>
		/// 
		/// The returned navigable map will be serializable if the specified
		/// navigable map is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the navigable map for which an unmodifiable view is to be
		///        returned </param>
		/// <returns> an unmodifiable view of the specified navigable map
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <K,V> NavigableMap<K,V> unmodifiableNavigableMap(NavigableMap<K, ? extends V> m)
		public static <K, V> NavigableMap<K, V> UnmodifiableNavigableMap(NavigableMap<K, ?> m)
		{
			return new UnmodifiableNavigableMap<>(m);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class UnmodifiableNavigableMap<K, V> extends UnmodifiableSortedMap<K, V> implements NavigableMap<K, V>, Serializable
		{
			private static final long serialVersionUID = -4858195264774772197L;

			/// <summary>
			/// A class for the <seealso cref="EMPTY_NAVIGABLE_MAP"/> which needs readResolve
			/// to preserve singleton property.
			/// </summary>
			/// @param <K> type of keys, if there were any, and of bounds </param>
			/// @param <V> type of values, if there were any </param>
			private static class EmptyNavigableMap<K, V> extends UnmodifiableNavigableMap<K, V> implements Serializable
			{

				private static final long serialVersionUID = -2239321462712562324L;

				EmptyNavigableMap()
				{
					base(new TreeMap<K, V>());
				}

				public NavigableSet<K> navigableKeySet()
				{
														return EmptyNavigableSet();
				}

				private Object readResolve()
				{
					return EMPTY_NAVIGABLE_MAP;
				}
			}

			/// <summary>
			/// Singleton for <seealso cref="emptyNavigableMap()"/> which is also immutable.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final EmptyNavigableMap<?,?> EMPTY_NAVIGABLE_MAP = new EmptyNavigableMap<>();
			private static final EmptyNavigableMap<?, ?> EMPTY_NAVIGABLE_MAP = new EmptyNavigableMap<>();

			/// <summary>
			/// The instance we wrap and protect.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final NavigableMap<K, ? extends V> nm;
			private final NavigableMap<K, ?> nm;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: UnmodifiableNavigableMap(NavigableMap<K, ? extends V> m)
			UnmodifiableNavigableMap(NavigableMap<K, ?> m)
			{
																	base(m);
																	nm = m;
			}

			public K lowerKey(K key)
			{
				return nm.lowerKey(key);
			}
			public K floorKey(K key)
			{
				return nm.floorKey(key);
			}
			public K ceilingKey(K key)
			{
				return nm.ceilingKey(key);
			}
			public K higherKey(K key)
			{
				return nm.higherKey(key);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> lowerEntry(K key)
			public Entry<K, V> lowerEntry(K key)
			{
				Entry<K, V> lower = (Entry<K, V>) nm.lowerEntry(key);
				return (null != lower) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(lower) : null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> floorEntry(K key)
			public Entry<K, V> floorEntry(K key)
			{
				Entry<K, V> floor = (Entry<K, V>) nm.floorEntry(key);
				return (null != floor) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(floor) : null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> ceilingEntry(K key)
			public Entry<K, V> ceilingEntry(K key)
			{
				Entry<K, V> ceiling = (Entry<K, V>) nm.ceilingEntry(key);
				return (null != ceiling) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(ceiling) : null;
			}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> higherEntry(K key)
			public Entry<K, V> higherEntry(K key)
			{
				Entry<K, V> higher = (Entry<K, V>) nm.higherEntry(key);
				return (null != higher) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(higher) : null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> firstEntry()
			public Entry<K, V> firstEntry()
			{
				Entry<K, V> first = (Entry<K, V>) nm.firstEntry();
				return (null != first) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(first) : null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Entry<K, V> lastEntry()
			public Entry<K, V> lastEntry()
			{
				Entry<K, V> last = (Entry<K, V>) nm.lastEntry();
				return (null != last) ? new UnmodifiableEntrySet.UnmodifiableEntry<>(last) : null;
			}

			public Entry<K, V> pollFirstEntry()
			{
										 throw new UnsupportedOperationException();
			}
			public Entry<K, V> pollLastEntry()
			{
										 throw new UnsupportedOperationException();
			}
			public NavigableMap<K, V> descendingMap()
			{
							   return UnmodifiableNavigableMap(nm.descendingMap());
			}
			public NavigableSet<K> navigableKeySet()
			{
							 return UnmodifiableNavigableSet(nm.navigableKeySet());
			}
			public NavigableSet<K> descendingKeySet()
			{
							return UnmodifiableNavigableSet(nm.descendingKeySet());
			}

			public NavigableMap<K, V> subMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				return UnmodifiableNavigableMap(nm.subMap(fromKey, fromInclusive, toKey, toInclusive));
			}

			public NavigableMap<K, V> headMap(K toKey, bool inclusive)
			{
					 return UnmodifiableNavigableMap(nm.headMap(toKey, inclusive));
			}
			public NavigableMap<K, V> tailMap(K fromKey, bool inclusive)
			{
				   return UnmodifiableNavigableMap(nm.tailMap(fromKey, inclusive));
			}
		}

		// Synch Wrappers

		/// <summary>
		/// Returns a synchronized (thread-safe) collection backed by the specified
		/// collection.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing collection is accomplished
		/// through the returned collection.<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// collection when traversing it via <seealso cref="Iterator"/>, <seealso cref="Spliterator"/>
		/// or <seealso cref="Stream"/>:
		/// <pre>
		///  Collection c = Collections.synchronizedCollection(myCollection);
		///     ...
		///  synchronized (c) {
		///      Iterator i = c.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///         foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned collection does <i>not</i> pass the {@code hashCode}
		/// and {@code equals} operations through to the backing collection, but
		/// relies on {@code Object}'s equals and hashCode methods.  This is
		/// necessary to preserve the contracts of these operations in the case
		/// </para>
		/// that the backing collection is a set or a list.<para>
		/// 
		/// The returned collection will be serializable if the specified collection
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="c"> the collection to be "wrapped" in a synchronized collection. </param>
		/// <returns> a synchronized view of the specified collection. </returns>
		public static <T> Collection<T> SynchronizedCollection(Collection<T> c)
		{
			return new SynchronizedCollection<>(c);
		}

		static <T> Collection<T> SynchronizedCollection(Collection<T> c, Object mutex)
		{
			return new SynchronizedCollection<>(c, mutex);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedCollection<E> implements Collection<E>, Serializable
		{
			private static final long serialVersionUID = 3053995032091335093L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Collection<E> c;
			Collection<E> c; // Backing Collection
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object mutex;
			Object mutex; // Object on which to synchronize

			SynchronizedCollection(Collection<E> c)
			{
				this.c = Objects.RequireNonNull(c);
				mutex = this;
			}

			SynchronizedCollection(Collection<E> c, Object mutex)
			{
				this.c = Objects.RequireNonNull(c);
				this.mutex = Objects.RequireNonNull(mutex);
			}

			public int size()
			{
				lock (mutex)
				{
					return c.Size();
				}
			}
			public bool Empty
			{
				lock (mutex)
				{
					return c.Empty;
				}
			}
			public bool contains(Object o)
			{
				lock (mutex)
				{
					return c.Contains(o);
				}
			}
			public Object[] toArray()
			{
				lock (mutex)
				{
					return c.ToArray();
				}
			}
			public <T> T[] toArray(T[] a)
			{
				lock (mutex)
				{
					return c.ToArray(a);
				}
			}

			public Iterator<E> iterator()
			{
				return c.Iterator(); // Must be manually synched by user!
			}

			public bool add(E e)
			{
				lock (mutex)
				{
					return c.Add(e);
				}
			}
			public bool remove(Object o)
			{
				lock (mutex)
				{
					return c.Remove(o);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> coll)
			public bool containsAll(Collection<?> coll)
			{
				lock (mutex)
				{
					return c.ContainsAll(coll);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(Collection<? extends E> coll)
			public bool addAll(Collection<?> coll)
			{
				lock (mutex)
				{
					return c.AddAll(coll);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> coll)
			public bool removeAll(Collection<?> coll)
			{
				lock (mutex)
				{
					return c.RemoveAll(coll);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> coll)
			public bool retainAll(Collection<?> coll)
			{
				lock (mutex)
				{
					return c.RetainAll(coll);
				}
			}
			public void clear()
			{
				lock (mutex)
				{
					c.Clear();
				}
			}
			public String ToString()
			{
				lock (mutex)
				{
					return c.ToString();
				}
			}
			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> consumer)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> consumer)
			{
				lock (mutex)
				{
					c.forEach(consumer);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				lock (mutex)
				{
					return c.removeIf(filter);
				}
			}
			public Spliterator<E> spliterator()
			{
				return c.spliterator(); // Must be manually synched by user!
			}
			public Stream<E> stream()
			{
				return c.stream(); // Must be manually synched by user!
			}
			public Stream<E> parallelStream()
			{
				return c.parallelStream(); // Must be manually synched by user!
			}
			private void writeObject(ObjectOutputStream s) throws IOException
			{
				lock (mutex)
				{
					s.defaultWriteObject();
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) set backed by the specified
		/// set.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing set is accomplished
		/// through the returned set.<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// set when iterating over it:
		/// <pre>
		///  Set s = Collections.synchronizedSet(new HashSet());
		///      ...
		///  synchronized (s) {
		///      Iterator i = s.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned set will be serializable if the specified set is
		/// serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the set to be "wrapped" in a synchronized set. </param>
		/// <returns> a synchronized view of the specified set. </returns>
		public static <T> Set<T> SynchronizedSet(Set<T> s)
		{
			return new SynchronizedSet<>(s);
		}

		static <T> Set<T> SynchronizedSet(Set<T> s, Object mutex)
		{
			return new SynchronizedSet<>(s, mutex);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedSet<E> extends SynchronizedCollection<E> implements Set<E>
		{
			private static final long serialVersionUID = 487447009682186044L;

			SynchronizedSet(Set<E> s)
			{
				base(s);
			}
			SynchronizedSet(Set<E> s, Object mutex)
			{
				base(s, mutex);
			}

			public bool Equals(Object o)
			{
				if (this == o)
				{
					return true;
				}
				lock (mutex)
				{
					return c.Equals(o);
				}
			}
			public int GetHashCode()
			{
				lock (mutex)
				{
					return c.HashCode();
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) sorted set backed by the specified
		/// sorted set.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing sorted set is accomplished
		/// through the returned sorted set (or its views).<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// sorted set when iterating over it or any of its <tt>subSet</tt>,
		/// <tt>headSet</tt>, or <tt>tailSet</tt> views.
		/// <pre>
		///  SortedSet s = Collections.synchronizedSortedSet(new TreeSet());
		///      ...
		///  synchronized (s) {
		///      Iterator i = s.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// or:
		/// <pre>
		///  SortedSet s = Collections.synchronizedSortedSet(new TreeSet());
		///  SortedSet s2 = s.headSet(foo);
		///      ...
		///  synchronized (s) {  // Note: s, not s2!!!
		///      Iterator i = s2.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned sorted set will be serializable if the specified
		/// sorted set is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the sorted set to be "wrapped" in a synchronized sorted set. </param>
		/// <returns> a synchronized view of the specified sorted set. </returns>
		public static <T> SortedSet<T> SynchronizedSortedSet(SortedSet<T> s)
		{
			return new SynchronizedSortedSet<>(s);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedSortedSet<E> extends SynchronizedSet<E> implements SortedSet<E>
		{
			private static final long serialVersionUID = 8695801310862127406L;

			private final SortedSet<E> ss;

			SynchronizedSortedSet(SortedSet<E> s)
			{
				base(s);
				ss = s;
			}
			SynchronizedSortedSet(SortedSet<E> s, Object mutex)
			{
				base(s, mutex);
				ss = s;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				lock (mutex)
				{
					return ss.comparator();
				}
			}

			public SortedSet<E> subSet(E fromElement, E toElement)
			{
				lock (mutex)
				{
					return new SynchronizedSortedSet<>(ss.subSet(fromElement, toElement), mutex);
				}
			}
			public SortedSet<E> headSet(E toElement)
			{
				lock (mutex)
				{
					return new SynchronizedSortedSet<>(ss.headSet(toElement), mutex);
				}
			}
			public SortedSet<E> tailSet(E fromElement)
			{
				lock (mutex)
				{
				   return new SynchronizedSortedSet<>(ss.tailSet(fromElement),mutex);
				}
			}

			public E first()
			{
				lock (mutex)
				{
					return ss.first();
				}
			}
			public E last()
			{
				lock (mutex)
				{
					return ss.last();
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) navigable set backed by the
		/// specified navigable set.  In order to guarantee serial access, it is
		/// critical that <strong>all</strong> access to the backing navigable set is
		/// accomplished through the returned navigable set (or its views).<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// navigable set when iterating over it or any of its {@code subSet},
		/// {@code headSet}, or {@code tailSet} views.
		/// <pre>
		///  NavigableSet s = Collections.synchronizedNavigableSet(new TreeSet());
		///      ...
		///  synchronized (s) {
		///      Iterator i = s.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// or:
		/// <pre>
		///  NavigableSet s = Collections.synchronizedNavigableSet(new TreeSet());
		///  NavigableSet s2 = s.headSet(foo, true);
		///      ...
		///  synchronized (s) {  // Note: s, not s2!!!
		///      Iterator i = s2.iterator(); // Must be in the synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned navigable set will be serializable if the specified
		/// navigable set is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="s"> the navigable set to be "wrapped" in a synchronized navigable
		/// set </param>
		/// <returns> a synchronized view of the specified navigable set
		/// @since 1.8 </returns>
		public static <T> NavigableSet<T> SynchronizedNavigableSet(NavigableSet<T> s)
		{
			return new SynchronizedNavigableSet<>(s);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedNavigableSet<E> extends SynchronizedSortedSet<E> implements NavigableSet<E>
		{
			private static final long serialVersionUID = -5505529816273629798L;

			private final NavigableSet<E> ns;

			SynchronizedNavigableSet(NavigableSet<E> s)
			{
				base(s);
				ns = s;
			}

			SynchronizedNavigableSet(NavigableSet<E> s, Object mutex)
			{
				base(s, mutex);
				ns = s;
			}
			public E lower(E e)
			{
				lock (mutex)
				{
					return ns.lower(e);
				}
			}
			public E floor(E e)
			{
				lock (mutex)
				{
					return ns.floor(e);
				}
			}
			public E ceiling(E e)
			{
				lock (mutex)
				{
					return ns.ceiling(e);
				}
			}
			public E higher(E e)
			{
				lock (mutex)
				{
					return ns.higher(e);
				}
			}
			public E pollFirst()
			{
				lock (mutex)
				{
					return ns.pollFirst();
				}
			}
			public E pollLast()
			{
				lock (mutex)
				{
					return ns.pollLast();
				}
			}

			public NavigableSet<E> descendingSet()
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.descendingSet(), mutex);
				}
			}

			public Iterator<E> descendingIterator()
			{
						 lock (mutex)
						 {
							 return descendingSet().GetEnumerator();
						 }
			}

			public NavigableSet<E> subSet(E fromElement, E toElement)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.subSet(fromElement, true, toElement, false), mutex);
				}
			}
			public NavigableSet<E> headSet(E toElement)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.headSet(toElement, false), mutex);
				}
			}
			public NavigableSet<E> tailSet(E fromElement)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.tailSet(fromElement, true), mutex);
				}
			}

			public NavigableSet<E> subSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.subSet(fromElement, fromInclusive, toElement, toInclusive), mutex);
				}
			}

			public NavigableSet<E> headSet(E toElement, bool inclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.headSet(toElement, inclusive), mutex);
				}
			}

			public NavigableSet<E> tailSet(E fromElement, bool inclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(ns.tailSet(fromElement, inclusive), mutex);
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) list backed by the specified
		/// list.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing list is accomplished
		/// through the returned list.<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// list when iterating over it:
		/// <pre>
		///  List list = Collections.synchronizedList(new ArrayList());
		///      ...
		///  synchronized (list) {
		///      Iterator i = list.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned list will be serializable if the specified list is
		/// serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="list"> the list to be "wrapped" in a synchronized list. </param>
		/// <returns> a synchronized view of the specified list. </returns>
		public static <T> List<T> SynchronizedList(List<T> list)
		{
			return (list is RandomAccess ? new SynchronizedRandomAccessList<>(list) : new SynchronizedList<>(list));
		}

		static <T> List<T> SynchronizedList(List<T> list, Object mutex)
		{
			return (list is RandomAccess ? new SynchronizedRandomAccessList<>(list, mutex) : new SynchronizedList<>(list, mutex));
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedList<E> extends SynchronizedCollection<E> implements List<E>
		{
			private static final long serialVersionUID = -7754090372962971524L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<E> list;
			List<E> list;

			SynchronizedList(List<E> list)
			{
				base(list);
				this.list = list;
			}
			SynchronizedList(List<E> list, Object mutex)
			{
				base(list, mutex);
				this.list = list;
			}

			public bool Equals(Object o)
			{
				if (this == o)
				{
					return true;
				}
				lock (mutex)
				{
					return list.Equals(o);
				}
			}
			public int GetHashCode()
			{
				lock (mutex)
				{
					return list.HashCode();
				}
			}

			public E get(int index)
			{
				lock (mutex)
				{
					return list.Get(index);
				}
			}
			public E set(int index, E element)
			{
				lock (mutex)
				{
					return list.Set(index, element);
				}
			}
			public void add(int index, E element)
			{
				lock (mutex)
				{
					list.Add(index, element);
				}
			}
			public E remove(int index)
			{
				lock (mutex)
				{
					return list.Remove(index);
				}
			}

			public int indexOf(Object o)
			{
				lock (mutex)
				{
					return list.IndexOf(o);
				}
			}
			public int lastIndexOf(Object o)
			{
				lock (mutex)
				{
					return list.LastIndexOf(o);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(int index, Collection<? extends E> c)
			public bool AddAll(int index, Collection<?> c)
			{
				lock (mutex)
				{
					return list.AddAll(index, c);
				}
			}

			public ListIterator<E> listIterator()
			{
				return list.ListIterator(); // Must be manually synched by user
			}

			public ListIterator<E> listIterator(int index)
			{
				return list.ListIterator(index); // Must be manually synched by user
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				lock (mutex)
				{
					return new SynchronizedList<>(list.SubList(fromIndex, toIndex), mutex);
				}
			}

			public void replaceAll(UnaryOperator<E> @operator)
			{
				lock (mutex)
				{
					list.replaceAll(@operator);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void sort(Comparator<?> c)
			{
				lock (mutex)
				{
					list.sort(c);
				}
			}

			/// <summary>
			/// SynchronizedRandomAccessList instances are serialized as
			/// SynchronizedList instances to allow them to be deserialized
			/// in pre-1.4 JREs (which do not have SynchronizedRandomAccessList).
			/// This method inverts the transformation.  As a beneficial
			/// side-effect, it also grafts the RandomAccess marker onto
			/// SynchronizedList instances that were serialized in pre-1.4 JREs.
			/// 
			/// Note: Unfortunately, SynchronizedRandomAccessList instances
			/// serialized in 1.4.1 and deserialized in 1.4 will become
			/// SynchronizedList instances, as this method was missing in 1.4.
			/// </summary>
			private Object readResolve()
			{
				return (list is RandomAccess ? new SynchronizedRandomAccessList<>(list)
						: this);
			}
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedRandomAccessList<E> extends SynchronizedList<E> implements RandomAccess
		{

			SynchronizedRandomAccessList(List<E> list)
			{
				base(list);
			}

			SynchronizedRandomAccessList(List<E> list, Object mutex)
			{
				base(list, mutex);
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				lock (mutex)
				{
					return new SynchronizedRandomAccessList<>(list.SubList(fromIndex, toIndex), mutex);
				}
			}

			private static final long serialVersionUID = 1530674583602358482L;

			/// <summary>
			/// Allows instances to be deserialized in pre-1.4 JREs (which do
			/// not have SynchronizedRandomAccessList).  SynchronizedList has
			/// a readResolve method that inverts this transformation upon
			/// deserialization.
			/// </summary>
			private Object writeReplace()
			{
				return new SynchronizedList<>(list);
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) map backed by the specified
		/// map.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing map is accomplished
		/// through the returned map.<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// map when iterating over any of its collection views:
		/// <pre>
		///  Map m = Collections.synchronizedMap(new HashMap());
		///      ...
		///  Set s = m.keySet();  // Needn't be in synchronized block
		///      ...
		///  synchronized (m) {  // Synchronizing on m, not s!
		///      Iterator i = s.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned map will be serializable if the specified map is
		/// serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the map to be "wrapped" in a synchronized map. </param>
		/// <returns> a synchronized view of the specified map. </returns>
		public static <K, V> Map<K, V> SynchronizedMap(Map<K, V> m)
		{
			return new SynchronizedMap<>(m);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class SynchronizedMap<K, V> implements Map<K, V>, Serializable
		{
			private static final long serialVersionUID = 1978198479659022715L;

			private final Map<K, V> m; // Backing Map
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object mutex;
			Object mutex; // Object on which to synchronize

			SynchronizedMap(Map<K, V> m)
			{
				this.m = Objects.RequireNonNull(m);
				mutex = this;
			}

			SynchronizedMap(Map<K, V> m, Object mutex)
			{
				this.m = m;
				this.mutex = mutex;
			}

			public int size()
			{
				lock (mutex)
				{
					return m.size();
				}
			}
			public bool Empty
			{
				lock (mutex)
				{
					return m.Empty;
				}
			}
			public bool containsKey(Object key)
			{
				lock (mutex)
				{
					return m.containsKey(key);
				}
			}
			public bool containsValue(Object value)
			{
				lock (mutex)
				{
					return m.containsValue(value);
				}
			}
			public V get(Object key)
			{
				lock (mutex)
				{
					return m.get(key);
				}
			}

			public V put(K key, V value)
			{
				lock (mutex)
				{
					return m.put(key, value);
				}
			}
			public V remove(Object key)
			{
				lock (mutex)
				{
					return m.remove(key);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> map)
			public void putAll(Map<?, ?> map)
			{
				lock (mutex)
				{
					m.putAll(map);
				}
			}
			public void clear()
			{
				lock (mutex)
				{
					m.clear();
				}
			}

			private transient Set<K> keySet;
			private transient Set<Map_Entry<K, V>> entrySet;
			private transient Collection<V> values;

			public Set<K> keySet()
			{
				lock (mutex)
				{
					if (keySet == null)
					{
						keySet = new SynchronizedSet<>(m.Keys, mutex);
					}
					return keySet;
				}
			}

			public Set<Map_Entry<K, V>> entrySet()
			{
				lock (mutex)
				{
					if (entrySet == null)
					{
						entrySet = new SynchronizedSet<>(m.entrySet(), mutex);
					}
					return entrySet;
				}
			}

			public Collection<V> values()
			{
				lock (mutex)
				{
					if (values == null)
					{
						values = new SynchronizedCollection<>(m.values(), mutex);
					}
					return values;
				}
			}

			public bool Equals(Object o)
			{
				if (this == o)
				{
					return true;
				}
				lock (mutex)
				{
					return m.Equals(o);
				}
			}
			public int GetHashCode()
			{
				lock (mutex)
				{
					return m.HashCode();
				}
			}
			public String ToString()
			{
				lock (mutex)
				{
					return m.ToString();
				}
			}

			// Override default methods in Map
			public V getOrDefault(Object k, V defaultValue)
			{
				lock (mutex)
				{
					return m.getOrDefault(k, defaultValue);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(BiConsumer<?, ?> action)
			{
				lock (mutex)
				{
					m.forEach(action);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void replaceAll(BiFunction<?, ?, ?> function)
			{
				lock (mutex)
				{
					m.replaceAll(function);
				}
			}
			public V putIfAbsent(K key, V value)
			{
				lock (mutex)
				{
					return m.putIfAbsent(key, value);
				}
			}
			public bool remove(Object key, Object value)
			{
				lock (mutex)
				{
					return m.remove(key, value);
				}
			}
			public bool replace(K key, V oldValue, V newValue)
			{
				lock (mutex)
				{
					return m.replace(key, oldValue, newValue);
				}
			}
			public V replace(K key, V value)
			{
				lock (mutex)
				{
					return m.replace(key, value);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfAbsent(K key, Function<?, ?> mappingFunction)
			{
				lock (mutex)
				{
					return m.computeIfAbsent(key, mappingFunction);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfPresent(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				lock (mutex)
				{
					return m.computeIfPresent(key, remappingFunction);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V compute(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				lock (mutex)
				{
					return m.compute(key, remappingFunction);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V merge(K key, V value, BiFunction<?, ?, ?> remappingFunction)
			{
				lock (mutex)
				{
					return m.merge(key, value, remappingFunction);
				}
			}

			private void writeObject(ObjectOutputStream s) throws IOException
			{
				lock (mutex)
				{
					s.defaultWriteObject();
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) sorted map backed by the specified
		/// sorted map.  In order to guarantee serial access, it is critical that
		/// <strong>all</strong> access to the backing sorted map is accomplished
		/// through the returned sorted map (or its views).<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// sorted map when iterating over any of its collection views, or the
		/// collections views of any of its <tt>subMap</tt>, <tt>headMap</tt> or
		/// <tt>tailMap</tt> views.
		/// <pre>
		///  SortedMap m = Collections.synchronizedSortedMap(new TreeMap());
		///      ...
		///  Set s = m.keySet();  // Needn't be in synchronized block
		///      ...
		///  synchronized (m) {  // Synchronizing on m, not s!
		///      Iterator i = s.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// or:
		/// <pre>
		///  SortedMap m = Collections.synchronizedSortedMap(new TreeMap());
		///  SortedMap m2 = m.subMap(foo, bar);
		///      ...
		///  Set s2 = m2.keySet();  // Needn't be in synchronized block
		///      ...
		///  synchronized (m) {  // Synchronizing on m, not m2 or s2!
		///      Iterator i = s.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned sorted map will be serializable if the specified
		/// sorted map is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the sorted map to be "wrapped" in a synchronized sorted map. </param>
		/// <returns> a synchronized view of the specified sorted map. </returns>
		public static <K, V> SortedMap<K, V> SynchronizedSortedMap(SortedMap<K, V> m)
		{
			return new SynchronizedSortedMap<>(m);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class SynchronizedSortedMap<K, V> extends SynchronizedMap<K, V> implements SortedMap<K, V>
		{
			private static final long serialVersionUID = -8798146769416483793L;

			private final SortedMap<K, V> sm;

			SynchronizedSortedMap(SortedMap<K, V> m)
			{
				base(m);
				sm = m;
			}
			SynchronizedSortedMap(SortedMap<K, V> m, Object mutex)
			{
				base(m, mutex);
				sm = m;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				lock (mutex)
				{
					return sm.comparator();
				}
			}

			public SortedMap<K, V> subMap(K fromKey, K toKey)
			{
				lock (mutex)
				{
					return new SynchronizedSortedMap<>(sm.subMap(fromKey, toKey), mutex);
				}
			}
			public SortedMap<K, V> headMap(K toKey)
			{
				lock (mutex)
				{
					return new SynchronizedSortedMap<>(sm.headMap(toKey), mutex);
				}
			}
			public SortedMap<K, V> tailMap(K fromKey)
			{
				lock (mutex)
				{
				   return new SynchronizedSortedMap<>(sm.tailMap(fromKey),mutex);
				}
			}

			public K firstKey()
			{
				lock (mutex)
				{
					return sm.firstKey();
				}
			}
			public K lastKey()
			{
				lock (mutex)
				{
					return sm.lastKey();
				}
			}
		}

		/// <summary>
		/// Returns a synchronized (thread-safe) navigable map backed by the
		/// specified navigable map.  In order to guarantee serial access, it is
		/// critical that <strong>all</strong> access to the backing navigable map is
		/// accomplished through the returned navigable map (or its views).<para>
		/// 
		/// It is imperative that the user manually synchronize on the returned
		/// navigable map when iterating over any of its collection views, or the
		/// collections views of any of its {@code subMap}, {@code headMap} or
		/// {@code tailMap} views.
		/// <pre>
		///  NavigableMap m = Collections.synchronizedNavigableMap(new TreeMap());
		///      ...
		///  Set s = m.keySet();  // Needn't be in synchronized block
		///      ...
		///  synchronized (m) {  // Synchronizing on m, not s!
		///      Iterator i = s.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// or:
		/// <pre>
		///  NavigableMap m = Collections.synchronizedNavigableMap(new TreeMap());
		///  NavigableMap m2 = m.subMap(foo, true, bar, false);
		///      ...
		///  Set s2 = m2.keySet();  // Needn't be in synchronized block
		///      ...
		///  synchronized (m) {  // Synchronizing on m, not m2 or s2!
		///      Iterator i = s.iterator(); // Must be in synchronized block
		///      while (i.hasNext())
		///          foo(i.next());
		///  }
		/// </pre>
		/// Failure to follow this advice may result in non-deterministic behavior.
		/// 
		/// </para>
		/// <para>The returned navigable map will be serializable if the specified
		/// navigable map is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the navigable map to be "wrapped" in a synchronized navigable
		///              map </param>
		/// <returns> a synchronized view of the specified navigable map.
		/// @since 1.8 </returns>
		public static <K, V> NavigableMap<K, V> SynchronizedNavigableMap(NavigableMap<K, V> m)
		{
			return new SynchronizedNavigableMap<>(m);
		}

		/// <summary>
		/// A synchronized NavigableMap.
		/// 
		/// @serial include
		/// </summary>
		static class SynchronizedNavigableMap<K, V> extends SynchronizedSortedMap<K, V> implements NavigableMap<K, V>
		{
			private static final long serialVersionUID = 699392247599746807L;

			private final NavigableMap<K, V> nm;

			SynchronizedNavigableMap(NavigableMap<K, V> m)
			{
				base(m);
				nm = m;
			}
			SynchronizedNavigableMap(NavigableMap<K, V> m, Object mutex)
			{
				base(m, mutex);
				nm = m;
			}

			public Entry<K, V> lowerEntry(K key)
			{
								lock (mutex)
								{
									return nm.lowerEntry(key);
								}
			}
			public K lowerKey(K key)
			{
								  lock (mutex)
								  {
									  return nm.lowerKey(key);
								  }
			}
			public Entry<K, V> floorEntry(K key)
			{
								lock (mutex)
								{
									return nm.floorEntry(key);
								}
			}
			public K floorKey(K key)
			{
								  lock (mutex)
								  {
									  return nm.floorKey(key);
								  }
			}
			public Entry<K, V> ceilingEntry(K key)
			{
							  lock (mutex)
							  {
								  return nm.ceilingEntry(key);
							  }
			}
			public K ceilingKey(K key)
			{
								lock (mutex)
								{
									return nm.ceilingKey(key);
								}
			}
			public Entry<K, V> higherEntry(K key)
			{
							   lock (mutex)
							   {
								   return nm.higherEntry(key);
							   }
			}
			public K higherKey(K key)
			{
								 lock (mutex)
								 {
									 return nm.higherKey(key);
								 }
			}
			public Entry<K, V> firstEntry()
			{
								   lock (mutex)
								   {
									   return nm.firstEntry();
								   }
			}
			public Entry<K, V> lastEntry()
			{
									lock (mutex)
									{
										return nm.lastEntry();
									}
			}
			public Entry<K, V> pollFirstEntry()
			{
							   lock (mutex)
							   {
								   return nm.pollFirstEntry();
							   }
			}
			public Entry<K, V> pollLastEntry()
			{
								lock (mutex)
								{
									return nm.pollLastEntry();
								}
			}

			public NavigableMap<K, V> descendingMap()
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.descendingMap(), mutex);
				}
			}

			public NavigableSet<K> keySet()
			{
				return navigableKeySet();
			}

			public NavigableSet<K> navigableKeySet()
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(nm.navigableKeySet(), mutex);
				}
			}

			public NavigableSet<K> descendingKeySet()
			{
				lock (mutex)
				{
					return new SynchronizedNavigableSet<>(nm.descendingKeySet(), mutex);
				}
			}


			public SortedMap<K, V> subMap(K fromKey, K toKey)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.subMap(fromKey, true, toKey, false), mutex);
				}
			}
			public SortedMap<K, V> headMap(K toKey)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.headMap(toKey, false), mutex);
				}
			}
			public SortedMap<K, V> tailMap(K fromKey)
			{
				lock (mutex)
				{
			return new SynchronizedNavigableMap<>(nm.tailMap(fromKey, true),mutex);
				}
			}

			public NavigableMap<K, V> subMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.subMap(fromKey, fromInclusive, toKey, toInclusive), mutex);
				}
			}

			public NavigableMap<K, V> headMap(K toKey, bool inclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.headMap(toKey, inclusive), mutex);
				}
			}

			public NavigableMap<K, V> tailMap(K fromKey, bool inclusive)
			{
				lock (mutex)
				{
					return new SynchronizedNavigableMap<>(nm.tailMap(fromKey, inclusive), mutex);
				}
			}
		}

		// Dynamically typesafe collection wrappers

		/// <summary>
		/// Returns a dynamically typesafe view of the specified collection.
		/// Any attempt to insert an element of the wrong type will result in an
		/// immediate <seealso cref="ClassCastException"/>.  Assuming a collection
		/// contains no incorrectly typed elements prior to the time a
		/// dynamically typesafe view is generated, and that all subsequent
		/// access to the collection takes place through the view, it is
		/// <i>guaranteed</i> that the collection cannot contain an incorrectly
		/// typed element.
		/// 
		/// <para>The generics mechanism in the language provides compile-time
		/// (static) type checking, but it is possible to defeat this mechanism
		/// with unchecked casts.  Usually this is not a problem, as the compiler
		/// issues warnings on all such unchecked operations.  There are, however,
		/// times when static type checking alone is not sufficient.  For example,
		/// suppose a collection is passed to a third-party library and it is
		/// imperative that the library code not corrupt the collection by
		/// inserting an element of the wrong type.
		/// 
		/// </para>
		/// <para>Another use of dynamically typesafe views is debugging.  Suppose a
		/// program fails with a {@code ClassCastException}, indicating that an
		/// incorrectly typed element was put into a parameterized collection.
		/// Unfortunately, the exception can occur at any time after the erroneous
		/// element is inserted, so it typically provides little or no information
		/// as to the real source of the problem.  If the problem is reproducible,
		/// one can quickly determine its source by temporarily modifying the
		/// program to wrap the collection with a dynamically typesafe view.
		/// For example, this declaration:
		///  <pre> {@code
		///     Collection<String> c = new HashSet<>();
		/// }</pre>
		/// may be replaced temporarily by this one:
		///  <pre> {@code
		///     Collection<String> c = Collections.checkedCollection(
		///         new HashSet<>(), String.class);
		/// }</pre>
		/// Running the program again will cause it to fail at the point where
		/// an incorrectly typed element is inserted into the collection, clearly
		/// identifying the source of the problem.  Once the problem is fixed, the
		/// modified declaration may be reverted back to the original.
		/// 
		/// </para>
		/// <para>The returned collection does <i>not</i> pass the hashCode and equals
		/// operations through to the backing collection, but relies on
		/// {@code Object}'s {@code equals} and {@code hashCode} methods.  This
		/// is necessary to preserve the contracts of these operations in the case
		/// that the backing collection is a set or a list.
		/// 
		/// </para>
		/// <para>The returned collection will be serializable if the specified
		/// collection is serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned collection permits insertion of null elements
		/// whenever the backing collection does.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the objects in the collection </param>
		/// <param name="c"> the collection for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="type"> the type of element that {@code c} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified collection
		/// @since 1.5 </returns>
		public static <E> Collection<E> CheckedCollection(Collection<E> c, Class type)
		{
			return new CheckedCollection<>(c, type);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <T> T[] zeroLengthArray(Class type)
		static <T> T[] ZeroLengthArray(Class type)
		{
			return (T[]) Array.newInstance(type, 0);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedCollection<E> implements Collection<E>, Serializable
		{
			private static final long serialVersionUID = 1578914078182001775L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Collection<E> c;
			Collection<E> c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class type;
			Class type;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E typeCheck(Object o)
			E typeCheck(Object o)
			{
				if (o != null && !type.isInstance(o))
				{
					throw new ClassCastException(badElementMsg(o));
				}
				return (E) o;
			}

			private String badElementMsg(Object o)
			{
				return "Attempt to insert " + o.GetType() + " element into collection with element type " + type;
			}

			CheckedCollection(Collection<E> c, Class type)
			{
				this.c = Objects.RequireNonNull(c, "c");
				this.type = Objects.RequireNonNull(type, "type");
			}

			public int size()
			{
				return c.Size();
			}
			public bool Empty
			{
				return c.Empty;
			}
			public bool contains(Object o)
			{
				return c.Contains(o);
			}
			public Object[] toArray()
			{
				return c.ToArray();
			}
			public <T> T[] toArray(T[] a)
			{
				return c.ToArray(a);
			}
			public String ToString()
			{
				return c.ToString();
			}
			public bool remove(Object o)
			{
				return c.Remove(o);
			}
			public void clear()
			{
				c.Clear();
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> coll)
			public bool containsAll(Collection<?> coll)
			{
				return c.ContainsAll(coll);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> coll)
			public bool removeAll(Collection<?> coll)
			{
				return c.RemoveAll(coll);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> coll)
			public bool retainAll(Collection<?> coll)
			{
				return c.RetainAll(coll);
			}

			public Iterator<E> iterator()
			{
				// JDK-6363904 - unwrapped iterator could be typecast to
				// ListIterator with unsafe set()
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<E> it = c.iterator();
				Iterator<E> it = c.Iterator();
				return new IteratorAnonymousInnerClassHelper3(it);
			}

			public bool add(E e)
			{
				return c.Add(typeCheck(e));
			}

			private E[] zeroLengthElementArray; // Lazily initialized

			private E[] zeroLengthElementArray()
			{
				return zeroLengthElementArray != null ? zeroLengthElementArray : (zeroLengthElementArray = ZeroLengthArray(type));
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Collection<E> checkedCopyOf(Collection<? extends E> coll)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Collection<E> checkedCopyOf(Collection<?> coll)
			{
				Object[] a;
				try
				{
					E[] z = zeroLengthElementArray();
					a = coll.toArray(z);
					// Defend against coll violating the toArray contract
					if (a.GetType() != z.GetType())
					{
						a = Arrays.CopyOf(a, a.Length, z.GetType());
					}
				}
				catch (ArrayStoreException)
				{
					// To get better and consistent diagnostics,
					// we call typeCheck explicitly on each element.
					// We call clone() to defend against coll retaining a
					// reference to the returned array and storing a bad
					// element into it after it has been type checked.
					a = coll.toArray().clone();
					foreach (Object o in a)
					{
						typeCheck(o);
					}
				}
				// A slight abuse of the type system, but safe here.
				return (Collection<E>) a;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(Collection<? extends E> coll)
			public bool addAll(Collection<?> coll)
			{
				// Doing things this way insulates us from concurrent changes
				// in the contents of coll and provides all-or-nothing
				// semantics (which we wouldn't get if we type-checked each
				// element as we added it)
				return c.AddAll(checkedCopyOf(coll));
			}

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				c.forEach(action);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				return c.removeIf(filter);
			}
			public Spliterator<E> spliterator()
			{
				return c.spliterator();
			}
			public Stream<E> stream()
			{
				return c.stream();
			}
			public Stream<E> parallelStream()
			{
				return c.parallelStream();
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified queue.
		/// Any attempt to insert an element of the wrong type will result in
		/// an immediate <seealso cref="ClassCastException"/>.  Assuming a queue contains
		/// no incorrectly typed elements prior to the time a dynamically typesafe
		/// view is generated, and that all subsequent access to the queue
		/// takes place through the view, it is <i>guaranteed</i> that the
		/// queue cannot contain an incorrectly typed element.
		/// 
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned queue will be serializable if the specified queue
		/// is serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned queue permits insertion of {@code null} elements
		/// whenever the backing queue does.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the objects in the queue </param>
		/// <param name="queue"> the queue for which a dynamically typesafe view is to be
		///             returned </param>
		/// <param name="type"> the type of element that {@code queue} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified queue
		/// @since 1.8 </returns>
		public static <E> Queue<E> CheckedQueue(Queue<E> queue, Class type)
		{
			return new CheckedQueue<>(queue, type);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedQueue<E> extends CheckedCollection<E> implements Queue<E>, Serializable
		{
			private static final long serialVersionUID = 1433151992604707767L;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Queue<E> queue;
			Queue<E> queue;

			CheckedQueue(Queue<E> queue, Class elementType)
			{
				base(queue, elementType);
				this.queue = queue;
			}

			public E element()
			{
				return queue.Element();
			}
			public bool Equals(Object o)
			{
				return o == this || c.Equals(o);
			}
			public int GetHashCode()
			{
				return c.HashCode();
			}
			public E peek()
			{
				return queue.Peek();
			}
			public E poll()
			{
				return queue.Poll();
			}
			public E remove()
			{
				return queue.Remove();
			}
			public bool offer(E e)
			{
				return queue.Offer(typeCheck(e));
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified set.
		/// Any attempt to insert an element of the wrong type will result in
		/// an immediate <seealso cref="ClassCastException"/>.  Assuming a set contains
		/// no incorrectly typed elements prior to the time a dynamically typesafe
		/// view is generated, and that all subsequent access to the set
		/// takes place through the view, it is <i>guaranteed</i> that the
		/// set cannot contain an incorrectly typed element.
		/// 
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned set will be serializable if the specified set is
		/// serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned set permits insertion of null elements whenever
		/// the backing set does.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the objects in the set </param>
		/// <param name="s"> the set for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="type"> the type of element that {@code s} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified set
		/// @since 1.5 </returns>
		public static <E> Set<E> CheckedSet(Set<E> s, Class type)
		{
			return new CheckedSet<>(s, type);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedSet<E> extends CheckedCollection<E> implements Set<E>, Serializable
		{
			private static final long serialVersionUID = 4694047833775013803L;

			CheckedSet(Set<E> s, Class elementType)
			{
				base(s, elementType);
			}

			public bool Equals(Object o)
			{
				return o == this || c.Equals(o);
			}
			public int GetHashCode()
			{
				return c.HashCode();
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified sorted set.
		/// Any attempt to insert an element of the wrong type will result in an
		/// immediate <seealso cref="ClassCastException"/>.  Assuming a sorted set
		/// contains no incorrectly typed elements prior to the time a
		/// dynamically typesafe view is generated, and that all subsequent
		/// access to the sorted set takes place through the view, it is
		/// <i>guaranteed</i> that the sorted set cannot contain an incorrectly
		/// typed element.
		/// 
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned sorted set will be serializable if the specified sorted
		/// set is serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned sorted set permits insertion of null elements
		/// whenever the backing sorted set does.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the objects in the set </param>
		/// <param name="s"> the sorted set for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="type"> the type of element that {@code s} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified sorted set
		/// @since 1.5 </returns>
		public static <E> SortedSet<E> CheckedSortedSet(SortedSet<E> s, Class type)
		{
			return new CheckedSortedSet<>(s, type);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedSortedSet<E> extends CheckedSet<E> implements SortedSet<E>, Serializable
		{
			private static final long serialVersionUID = 1599911165492914959L;

			private final SortedSet<E> ss;

			CheckedSortedSet(SortedSet<E> s, Class type)
			{
				base(s, type);
				ss = s;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				return ss.comparator();
			}
			public E first()
			{
				return ss.first();
			}
			public E last()
			{
				return ss.last();
			}

			public SortedSet<E> subSet(E fromElement, E toElement)
			{
				return CheckedSortedSet(ss.subSet(fromElement,toElement), type);
			}
			public SortedSet<E> headSet(E toElement)
			{
				return CheckedSortedSet(ss.headSet(toElement), type);
			}
			public SortedSet<E> tailSet(E fromElement)
			{
				return CheckedSortedSet(ss.tailSet(fromElement), type);
			}
		}

	/// <summary>
	/// Returns a dynamically typesafe view of the specified navigable set.
	/// Any attempt to insert an element of the wrong type will result in an
	/// immediate <seealso cref="ClassCastException"/>.  Assuming a navigable set
	/// contains no incorrectly typed elements prior to the time a
	/// dynamically typesafe view is generated, and that all subsequent
	/// access to the navigable set takes place through the view, it is
	/// <em>guaranteed</em> that the navigable set cannot contain an incorrectly
	/// typed element.
	///     
	/// <para>A discussion of the use of dynamically typesafe views may be
	/// found in the documentation for the {@link #checkedCollection
	/// checkedCollection} method.
	///     
	/// </para>
	/// <para>The returned navigable set will be serializable if the specified
	/// navigable set is serializable.
	///     
	/// </para>
	/// <para>Since {@code null} is considered to be a value of any reference
	/// type, the returned navigable set permits insertion of null elements
	/// whenever the backing sorted set does.
	///     
	/// </para>
	/// </summary>
	/// @param <E> the class of the objects in the set </param>
	/// <param name="s"> the navigable set for which a dynamically typesafe view is to be
	///          returned </param>
	/// <param name="type"> the type of element that {@code s} is permitted to hold </param>
	/// <returns> a dynamically typesafe view of the specified navigable set
	/// @since 1.8 </returns>
		public static <E> NavigableSet<E> CheckedNavigableSet(NavigableSet<E> s, Class type)
		{
			return new CheckedNavigableSet<>(s, type);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedNavigableSet<E> extends CheckedSortedSet<E> implements NavigableSet<E>, Serializable
		{
			private static final long serialVersionUID = -5429120189805438922L;

			private final NavigableSet<E> ns;

			CheckedNavigableSet(NavigableSet<E> s, Class type)
			{
				base(s, type);
				ns = s;
			}

			public E lower(E e)
			{
				return ns.lower(e);
			}
			public E floor(E e)
			{
				return ns.floor(e);
			}
			public E ceiling(E e)
			{
				return ns.ceiling(e);
			}
			public E higher(E e)
			{
				return ns.higher(e);
			}
			public E pollFirst()
			{
				return ns.pollFirst();
			}
			public E pollLast()
			{
				return ns.pollLast();
			}
			public NavigableSet<E> descendingSet()
			{
							  return CheckedNavigableSet(ns.descendingSet(), type);
			}
			public Iterator<E> descendingIterator()
			{
					return CheckedNavigableSet(ns.descendingSet(), type).Iterator();
			}

			public NavigableSet<E> subSet(E fromElement, E toElement)
			{
				return CheckedNavigableSet(ns.subSet(fromElement, true, toElement, false), type);
			}
			public NavigableSet<E> headSet(E toElement)
			{
				return CheckedNavigableSet(ns.headSet(toElement, false), type);
			}
			public NavigableSet<E> tailSet(E fromElement)
			{
				return CheckedNavigableSet(ns.tailSet(fromElement, true), type);
			}

			public NavigableSet<E> subSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
			{
				return CheckedNavigableSet(ns.subSet(fromElement, fromInclusive, toElement, toInclusive), type);
			}

			public NavigableSet<E> headSet(E toElement, bool inclusive)
			{
				return CheckedNavigableSet(ns.headSet(toElement, inclusive), type);
			}

			public NavigableSet<E> tailSet(E fromElement, bool inclusive)
			{
				return CheckedNavigableSet(ns.tailSet(fromElement, inclusive), type);
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified list.
		/// Any attempt to insert an element of the wrong type will result in
		/// an immediate <seealso cref="ClassCastException"/>.  Assuming a list contains
		/// no incorrectly typed elements prior to the time a dynamically typesafe
		/// view is generated, and that all subsequent access to the list
		/// takes place through the view, it is <i>guaranteed</i> that the
		/// list cannot contain an incorrectly typed element.
		/// 
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned list will be serializable if the specified list
		/// is serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned list permits insertion of null elements whenever
		/// the backing list does.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the objects in the list </param>
		/// <param name="list"> the list for which a dynamically typesafe view is to be
		///             returned </param>
		/// <param name="type"> the type of element that {@code list} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified list
		/// @since 1.5 </returns>
		public static <E> List<E> CheckedList(List<E> list, Class type)
		{
			return (list is RandomAccess ? new CheckedRandomAccessList<>(list, type) : new CheckedList<>(list, type));
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedList<E> extends CheckedCollection<E> implements List<E>
		{
			private static final long serialVersionUID = 65247728283967356L;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<E> list;
			List<E> list;

			CheckedList(List<E> list, Class type)
			{
				base(list, type);
				this.list = list;
			}

			public bool Equals(Object o)
			{
				return o == this || list.Equals(o);
			}
			public int GetHashCode()
			{
				return list.HashCode();
			}
			public E get(int index)
			{
				return list.Get(index);
			}
			public E remove(int index)
			{
				return list.Remove(index);
			}
			public int indexOf(Object o)
			{
				return list.IndexOf(o);
			}
			public int lastIndexOf(Object o)
			{
				return list.LastIndexOf(o);
			}

			public E set(int index, E element)
			{
				return list.Set(index, typeCheck(element));
			}

			public void add(int index, E element)
			{
				list.Add(index, typeCheck(element));
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(int index, Collection<? extends E> c)
			public bool AddAll(int index, Collection<?> c)
			{
				return list.AddAll(index, checkedCopyOf(c));
			}
			public ListIterator<E> listIterator()
			{
				return listIterator(0);
			}

			public ListIterator<E> listIterator(final int index)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ListIterator<E> i = list.listIterator(index);
				ListIterator<E> i = list.ListIterator(index);

				return new ListIteratorAnonymousInnerClassHelper2(i);
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				return new CheckedList<>(list.SubList(fromIndex, toIndex), type);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="ClassCastException"> if the class of an element returned by the
			///         operator prevents it from being added to this collection. The
			///         exception may be thrown after some elements of the list have
			///         already been replaced. </exception>
			public void replaceAll(UnaryOperator<E> @operator)
			{
				Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				list.replaceAll(e => typeCheck(@operator.apply(e)));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void sort(Comparator<?> c)
			{
				list.sort(c);
			}
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedRandomAccessList<E> extends CheckedList<E> implements RandomAccess
		{
			private static final long serialVersionUID = 1638200125423088369L;

			CheckedRandomAccessList(List<E> list, Class type)
			{
				base(list, type);
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				return new CheckedRandomAccessList<>(list.SubList(fromIndex, toIndex), type);
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified map.
		/// Any attempt to insert a mapping whose key or value have the wrong
		/// type will result in an immediate <seealso cref="ClassCastException"/>.
		/// Similarly, any attempt to modify the value currently associated with
		/// a key will result in an immediate <seealso cref="ClassCastException"/>,
		/// whether the modification is attempted directly through the map
		/// itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		/// map's <seealso cref="Map#entrySet() entry set"/> view.
		/// 
		/// <para>Assuming a map contains no incorrectly typed keys or values
		/// prior to the time a dynamically typesafe view is generated, and
		/// that all subsequent access to the map takes place through the view
		/// (or one of its collection views), it is <i>guaranteed</i> that the
		/// map cannot contain an incorrectly typed key or value.
		/// 
		/// </para>
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned map will be serializable if the specified map is
		/// serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned map permits insertion of null keys or values
		/// whenever the backing map does.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the map for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		/// <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified map
		/// @since 1.5 </returns>
		public static <K, V> Map<K, V> CheckedMap(Map<K, V> m, Class keyType, Class valueType)
		{
			return new CheckedMap<>(m, keyType, valueType);
		}


		/// <summary>
		/// @serial include
		/// </summary>
		private static class CheckedMap<K, V> implements Map<K, V>, Serializable
		{
			private static final long serialVersionUID = 5742860141034234728L;

			private final Map<K, V> m;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class keyType;
			Class keyType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class valueType;
			Class valueType;

			private void typeCheck(Object key, Object value)
			{
				if (key != null && !keyType.isInstance(key))
				{
					throw new ClassCastException(badKeyMsg(key));
				}

				if (value != null && !valueType.isInstance(value))
				{
					throw new ClassCastException(badValueMsg(value));
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private java.util.function.BiFunction<? base K, ? base V, ? extends V> typeCheck(java.util.function.BiFunction<? base K, ? base V, ? extends V> func)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			private BiFunction<?, ?, ?> typeCheck(BiFunction<?, ?, ?> func)
			{
				Objects.RequireNonNull(func);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return (k, v) =>
				{
					V newValue = func.apply(k, v);
					typeCheck(k, newValue);
					return newValue;
				};
			}

			private String badKeyMsg(Object key)
			{
				return "Attempt to insert " + key.GetType() + " key into map with key type " + keyType;
			}

			private String badValueMsg(Object value)
			{
				return "Attempt to insert " + value.GetType() + " value into map with value type " + valueType;
			}

			CheckedMap(Map<K, V> m, Class keyType, Class valueType)
			{
				this.m = Objects.RequireNonNull(m);
				this.keyType = Objects.RequireNonNull(keyType);
				this.valueType = Objects.RequireNonNull(valueType);
			}

			public int size()
			{
				return m.size();
			}
			public bool Empty
			{
				return m.Empty;
			}
			public bool containsKey(Object key)
			{
				return m.containsKey(key);
			}
			public bool containsValue(Object v)
			{
				return m.containsValue(v);
			}
			public V get(Object key)
			{
				return m.get(key);
			}
			public V remove(Object key)
			{
				return m.remove(key);
			}
			public void clear()
			{
				m.clear();
			}
			public Set<K> keySet()
			{
				return m.Keys;
			}
			public Collection<V> values()
			{
				return m.values();
			}
			public bool Equals(Object o)
			{
				return o == this || m.Equals(o);
			}
			public int GetHashCode()
			{
				return m.HashCode();
			}
			public String ToString()
			{
				return m.ToString();
			}

			public V put(K key, V value)
			{
				typeCheck(key, value);
				return m.put(key, value);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void putAll(Map<? extends K, ? extends V> t)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void putAll(Map<?, ?> t)
			{
				// Satisfy the following goals:
				// - good diagnostics in case of type mismatch
				// - all-or-nothing semantics
				// - protection from malicious t
				// - correct behavior if t is a concurrent map
				Object[] entries = t.entrySet().toArray();
				List<Map_Entry<K, V>> @checked = new List<Map_Entry<K, V>>(entries.Length);
				foreach (Object o in entries)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>) o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>) o;
					Object k = e.Key;
					Object v = e.Value;
					typeCheck(k, v);
					@checked.Add(new AbstractMap.SimpleImmutableEntry<>((K)k, (V)v));
				}
				foreach (Map_Entry<K, V> e in @checked)
				{
					m.put(e.Key, e.Value);
				}
			}

			private transient Set<Map_Entry<K, V>> entrySet;

			public Set<Map_Entry<K, V>> entrySet()
			{
				if (entrySet == null)
				{
					entrySet = new CheckedEntrySet<>(m.entrySet(), valueType);
				}
				return entrySet;
			}

			// Override default methods in Map
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(BiConsumer<?, ?> action)
			{
				m.forEach(action);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void replaceAll(BiFunction<?, ?, ?> function)
			{
				m.replaceAll(typeCheck(function));
			}

			public V putIfAbsent(K key, V value)
			{
				typeCheck(key, value);
				return m.putIfAbsent(key, value);
			}

			public bool remove(Object key, Object value)
			{
				return m.remove(key, value);
			}

			public bool replace(K key, V oldValue, V newValue)
			{
				typeCheck(key, newValue);
				return m.replace(key, oldValue, newValue);
			}

			public V replace(K key, V value)
			{
				typeCheck(key, value);
				return m.replace(key, value);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfAbsent(K key, Function<?, ?> mappingFunction)
			{
				Objects.RequireNonNull(mappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return m.computeIfAbsent(key, k =>
				{
					V value = mappingFunction.apply(k);
					typeCheck(k, value);
					return value;
				});
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfPresent(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				return m.computeIfPresent(key, typeCheck(remappingFunction));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V compute(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				return m.compute(key, typeCheck(remappingFunction));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V merge(K key, V value, BiFunction<?, ?, ?> remappingFunction)
			{
				Objects.RequireNonNull(remappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return m.merge(key, value, (v1, v2) =>
				{
					V newValue = remappingFunction.apply(v1, v2);
					typeCheck(null, newValue);
					return newValue;
				});
			}

			/// <summary>
			/// We need this class in addition to CheckedSet as Map.Entry permits
			/// modification of the backing Map via the setValue operation.  This
			/// class is subtle: there are many possible attacks that must be
			/// thwarted.
			/// 
			/// @serial exclude
			/// </summary>
			static class CheckedEntrySet<K, V> implements Set<Map_Entry<K, V>>
			{
				private final Set<Map_Entry<K, V>> s;
				private final Class valueType;

				CheckedEntrySet(Set<Map_Entry<K, V>> s, Class valueType)
				{
					this.s = s;
					this.valueType = valueType;
				}

				public int size()
				{
					return s.size();
				}
				public bool Empty
				{
					return s.Empty;
				}
				public String ToString()
				{
					return s.ToString();
				}
				public int GetHashCode()
				{
					return s.HashCode();
				}
				public void clear()
				{
					s.clear();
				}

				public bool add(Map_Entry<K, V> e)
				{
					throw new UnsupportedOperationException();
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean addAll(Collection<? extends Map_Entry<K, V>> coll)
				public bool addAll(Collection<?> coll)
				{
					throw new UnsupportedOperationException();
				}

				public Iterator<Map_Entry<K, V>> iterator()
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<Map_Entry<K, V>> i = s.iterator();
					Iterator<Map_Entry<K, V>> i = s.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class valueType = this.valueType;
					Class valueType = this.valueType;

					return new IteratorAnonymousInnerClassHelper4(valueType, i);
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object[] toArray()
				public Object[] toArray()
				{
					Object[] source = s.toArray();

					/*
					 * Ensure that we don't get an ArrayStoreException even if
					 * s.toArray returns an array of something other than Object
					 */
					Object[] dest = (typeof(CheckedEntry).IsInstanceOfType(source.GetType().GetElementType()) ? source : new Object[source.Length]);

					for (int i = 0; i < source.Length; i++)
					{
						dest[i] = checkedEntry((Map_Entry<K, V>)source[i], valueType);
					}
					return dest;
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
				public <T> T[] toArray(T[] a)
				{
					// We don't pass a to s.toArray, to avoid window of
					// vulnerability wherein an unscrupulous multithreaded client
					// could get his hands on raw (unwrapped) Entries from s.
					T[] arr = s.toArray(a.length == 0 ? a : Arrays.CopyOf(a, 0));

					for (int i = 0; i < arr.Length; i++)
					{
						arr[i] = (T) checkedEntry((Map_Entry<K, V>)arr[i], valueType);
					}
					if (arr.Length > a.length)
					{
						return arr;
					}

					System.Array.Copy(arr, 0, a, 0, arr.Length);
					if (a.length > arr.Length)
					{
						a[arr.Length] = null;
					}
					return a;
				}

				/// <summary>
				/// This method is overridden to protect the backing set against
				/// an object with a nefarious equals function that senses
				/// that the equality-candidate is Map.Entry and calls its
				/// setValue method.
				/// </summary>
				public bool contains(Object o)
				{
					if (!(o is Map_Entry))
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>) o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>) o;
					return s.contains((e is CheckedEntry) ? e : checkedEntry(e, valueType));
				}

				/// <summary>
				/// The bulk collection methods are overridden to protect
				/// against an unscrupulous collection whose contains(Object o)
				/// method senses when o is a Map.Entry, and calls o.setValue.
				/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> c)
				public bool containsAll(Collection<?> c)
				{
					foreach (Object o in c)
					{
						if (!contains(o)) // Invokes safe contains() above
						{
							return false;
						}
					}
					return true;
				}

				public bool remove(Object o)
				{
					if (!(o is Map_Entry))
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return s.remove(new AbstractMap.SimpleImmutableEntry <>((Map_Entry<?,?>)o));
					return s.remove(new AbstractMap.SimpleImmutableEntry <>((Map_Entry<?, ?>)o));
				}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> c)
				public bool removeAll(Collection<?> c)
				{
					return batchRemove(c, false);
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> c)
				public bool retainAll(Collection<?> c)
				{
					return batchRemove(c, true);
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private boolean batchRemove(Collection<?> c, boolean complement)
				private bool batchRemove(Collection<?> c, bool complement)
				{
					Objects.RequireNonNull(c);
					bool modified = false;
					Iterator<Map_Entry<K, V>> it = iterator();
					while (it.HasNext())
					{
						if (c.contains(it.Next()) != complement)
						{
							it.remove();
							modified = true;
						}
					}
					return modified;
				}

				public bool Equals(Object o)
				{
					if (o == this)
					{
						return true;
					}
					if (!(o is Set))
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> that = (Set<?>) o;
					Set<?> that = (Set<?>) o;
					return that.Count == s.size() && containsAll(that); // Invokes safe containsAll() above
				}

				static <K, V, T> CheckedEntry<K, V, T> checkedEntry(Map_Entry<K, V> e, Class valueType)
				{
					return new CheckedEntry<>(e, valueType);
				}

				/// <summary>
				/// This "wrapper class" serves two purposes: it prevents
				/// the client from modifying the backing Map, by short-circuiting
				/// the setValue method, and it protects the backing Map against
				/// an ill-behaved Map.Entry that attempts to modify another
				/// Map.Entry when asked to perform an equality check.
				/// </summary>
				private static class CheckedEntry<K, V, T> implements Map_Entry<K, V>
				{
					private final Map_Entry<K, V> e;
					private final Class valueType;

					CheckedEntry(Map_Entry<K, V> e, Class valueType)
					{
						this.e = Objects.RequireNonNull(e);
						this.valueType = Objects.RequireNonNull(valueType);
					}

					public K Key
					{
						return e.Key;
					}
					public V Value
					{
						return e.Value;
					}
					public int GetHashCode()
					{
						return e.HashCode();
					}
					public String ToString()
					{
						return e.ToString();
					}

					public V setValue(V value)
					{
						if (value != null && !valueType.isInstance(value))
						{
							throw new ClassCastException(badValueMsg(value));
						}
						return e.setValue(value);
					}

					private String badValueMsg(Object value)
					{
						return "Attempt to insert " + value.GetType() + " value into map with value type " + valueType;
					}

					public bool Equals(Object o)
					{
						if (o == this)
						{
							return true;
						}
						if (!(o is Map_Entry))
						{
							return false;
						}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return e.equals(new AbstractMap.SimpleImmutableEntry <>((Map_Entry<?,?>)o));
						return e.Equals(new AbstractMap.SimpleImmutableEntry <>((Map_Entry<?, ?>)o));
					}
				}
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified sorted map.
		/// Any attempt to insert a mapping whose key or value have the wrong
		/// type will result in an immediate <seealso cref="ClassCastException"/>.
		/// Similarly, any attempt to modify the value currently associated with
		/// a key will result in an immediate <seealso cref="ClassCastException"/>,
		/// whether the modification is attempted directly through the map
		/// itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		/// map's <seealso cref="Map#entrySet() entry set"/> view.
		/// 
		/// <para>Assuming a map contains no incorrectly typed keys or values
		/// prior to the time a dynamically typesafe view is generated, and
		/// that all subsequent access to the map takes place through the view
		/// (or one of its collection views), it is <i>guaranteed</i> that the
		/// map cannot contain an incorrectly typed key or value.
		/// 
		/// </para>
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned map will be serializable if the specified map is
		/// serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned map permits insertion of null keys or values
		/// whenever the backing map does.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="m"> the map for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		/// <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified map
		/// @since 1.5 </returns>
		public static <K, V> SortedMap<K, V> CheckedSortedMap(SortedMap<K, V> m, Class keyType, Class valueType)
		{
			return new CheckedSortedMap<>(m, keyType, valueType);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedSortedMap<K, V> extends CheckedMap<K, V> implements SortedMap<K, V>, Serializable
		{
			private static final long serialVersionUID = 1599671320688067438L;

			private final SortedMap<K, V> sm;

			CheckedSortedMap(SortedMap<K, V> m, Class keyType, Class valueType)
			{
				base(m, keyType, valueType);
				sm = m;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				return sm.comparator();
			}
			public K firstKey()
			{
				return sm.firstKey();
			}
			public K lastKey()
			{
				return sm.lastKey();
			}

			public SortedMap<K, V> subMap(K fromKey, K toKey)
			{
				return CheckedSortedMap(sm.subMap(fromKey, toKey), keyType, valueType);
			}
			public SortedMap<K, V> headMap(K toKey)
			{
				return CheckedSortedMap(sm.headMap(toKey), keyType, valueType);
			}
			public SortedMap<K, V> tailMap(K fromKey)
			{
				return CheckedSortedMap(sm.tailMap(fromKey), keyType, valueType);
			}
		}

		/// <summary>
		/// Returns a dynamically typesafe view of the specified navigable map.
		/// Any attempt to insert a mapping whose key or value have the wrong
		/// type will result in an immediate <seealso cref="ClassCastException"/>.
		/// Similarly, any attempt to modify the value currently associated with
		/// a key will result in an immediate <seealso cref="ClassCastException"/>,
		/// whether the modification is attempted directly through the map
		/// itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		/// map's <seealso cref="Map#entrySet() entry set"/> view.
		/// 
		/// <para>Assuming a map contains no incorrectly typed keys or values
		/// prior to the time a dynamically typesafe view is generated, and
		/// that all subsequent access to the map takes place through the view
		/// (or one of its collection views), it is <em>guaranteed</em> that the
		/// map cannot contain an incorrectly typed key or value.
		/// 
		/// </para>
		/// <para>A discussion of the use of dynamically typesafe views may be
		/// found in the documentation for the {@link #checkedCollection
		/// checkedCollection} method.
		/// 
		/// </para>
		/// <para>The returned map will be serializable if the specified map is
		/// serializable.
		/// 
		/// </para>
		/// <para>Since {@code null} is considered to be a value of any reference
		/// type, the returned map permits insertion of null keys or values
		/// whenever the backing map does.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> type of map keys </param>
		/// @param <V> type of map values </param>
		/// <param name="m"> the map for which a dynamically typesafe view is to be
		///          returned </param>
		/// <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		/// <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		/// <returns> a dynamically typesafe view of the specified map
		/// @since 1.8 </returns>
		public static <K, V> NavigableMap<K, V> CheckedNavigableMap(NavigableMap<K, V> m, Class keyType, Class valueType)
		{
			return new CheckedNavigableMap<>(m, keyType, valueType);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class CheckedNavigableMap<K, V> extends CheckedSortedMap<K, V> implements NavigableMap<K, V>, Serializable
		{
			private static final long serialVersionUID = -4852462692372534096L;

			private final NavigableMap<K, V> nm;

			CheckedNavigableMap(NavigableMap<K, V> m, Class keyType, Class valueType)
			{
				base(m, keyType, valueType);
				nm = m;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> comparator()
			{
				return nm.comparator();
			}
			public K firstKey()
			{
				return nm.firstKey();
			}
			public K lastKey()
			{
				return nm.lastKey();
			}

			public Entry<K, V> lowerEntry(K key)
			{
				Entry<K, V> lower = nm.lowerEntry(key);
				return (null != lower) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(lower, valueType) : null;
			}

			public K lowerKey(K key)
			{
				return nm.lowerKey(key);
			}

			public Entry<K, V> floorEntry(K key)
			{
				Entry<K, V> floor = nm.floorEntry(key);
				return (null != floor) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(floor, valueType) : null;
			}

			public K floorKey(K key)
			{
				return nm.floorKey(key);
			}

			public Entry<K, V> ceilingEntry(K key)
			{
				Entry<K, V> ceiling = nm.ceilingEntry(key);
				return (null != ceiling) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(ceiling, valueType) : null;
			}

			public K ceilingKey(K key)
			{
				return nm.ceilingKey(key);
			}

			public Entry<K, V> higherEntry(K key)
			{
				Entry<K, V> higher = nm.higherEntry(key);
				return (null != higher) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(higher, valueType) : null;
			}

			public K higherKey(K key)
			{
				return nm.higherKey(key);
			}

			public Entry<K, V> firstEntry()
			{
				Entry<K, V> first = nm.firstEntry();
				return (null != first) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(first, valueType) : null;
			}

			public Entry<K, V> lastEntry()
			{
				Entry<K, V> last = nm.lastEntry();
				return (null != last) ? new CheckedMap.CheckedEntrySet.CheckedEntry<>(last, valueType) : null;
			}

			public Entry<K, V> pollFirstEntry()
			{
				Entry<K, V> entry = nm.pollFirstEntry();
				return (null == entry) ? null : new CheckedMap.CheckedEntrySet.CheckedEntry<>(entry, valueType);
			}

			public Entry<K, V> pollLastEntry()
			{
				Entry<K, V> entry = nm.pollLastEntry();
				return (null == entry) ? null : new CheckedMap.CheckedEntrySet.CheckedEntry<>(entry, valueType);
			}

			public NavigableMap<K, V> descendingMap()
			{
				return CheckedNavigableMap(nm.descendingMap(), keyType, valueType);
			}

			public NavigableSet<K> keySet()
			{
				return navigableKeySet();
			}

			public NavigableSet<K> navigableKeySet()
			{
				return CheckedNavigableSet(nm.navigableKeySet(), keyType);
			}

			public NavigableSet<K> descendingKeySet()
			{
				return CheckedNavigableSet(nm.descendingKeySet(), keyType);
			}

			public NavigableMap<K, V> subMap(K fromKey, K toKey)
			{
				return CheckedNavigableMap(nm.subMap(fromKey, true, toKey, false), keyType, valueType);
			}

			public NavigableMap<K, V> headMap(K toKey)
			{
				return CheckedNavigableMap(nm.headMap(toKey, false), keyType, valueType);
			}

			public NavigableMap<K, V> tailMap(K fromKey)
			{
				return CheckedNavigableMap(nm.tailMap(fromKey, true), keyType, valueType);
			}

			public NavigableMap<K, V> subMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				return CheckedNavigableMap(nm.subMap(fromKey, fromInclusive, toKey, toInclusive), keyType, valueType);
			}

			public NavigableMap<K, V> headMap(K toKey, bool inclusive)
			{
				return CheckedNavigableMap(nm.headMap(toKey, inclusive), keyType, valueType);
			}

			public NavigableMap<K, V> tailMap(K fromKey, bool inclusive)
			{
				return CheckedNavigableMap(nm.tailMap(fromKey, inclusive), keyType, valueType);
			}
		}

		// Empty collections

		/// <summary>
		/// Returns an iterator that has no elements.  More precisely,
		/// 
		/// <ul>
		/// <li><seealso cref="Iterator#hasNext hasNext"/> always returns {@code
		/// false}.</li>
		/// <li><seealso cref="Iterator#next next"/> always throws {@link
		/// NoSuchElementException}.</li>
		/// <li><seealso cref="Iterator#remove remove"/> always throws {@link
		/// IllegalStateException}.</li>
		/// </ul>
		/// 
		/// <para>Implementations of this method are permitted, but not
		/// required, to return the same object from multiple invocations.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements, if there were any, in the iterator </param>
		/// <returns> an empty iterator
		/// @since 1.7 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> Iterator<T> emptyIterator()
		public static <T> Iterator<T> EmptyIterator()
		{
			return (Iterator<T>) EmptyIterator.EMPTY_ITERATOR;
		}

		private static class EmptyIterator<E> implements Iterator<E>
		{
			static final EmptyIterator<Object> EMPTY_ITERATOR = new EmptyIterator<>();

			public bool hasNext()
			{
				return false;
			}
			public E next()
			{
				throw new NoSuchElementException();
			}
			public void remove()
			{
				throw new IllegalStateException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEachRemaining(Consumer<?> action)
			{
				Objects.RequireNonNull(action);
			}
		}

		/// <summary>
		/// Returns a list iterator that has no elements.  More precisely,
		/// 
		/// <ul>
		/// <li><seealso cref="Iterator#hasNext hasNext"/> and {@link
		/// ListIterator#hasPrevious hasPrevious} always return {@code
		/// false}.</li>
		/// <li><seealso cref="Iterator#next next"/> and {@link ListIterator#previous
		/// previous} always throw <seealso cref="NoSuchElementException"/>.</li>
		/// <li><seealso cref="Iterator#remove remove"/> and {@link ListIterator#set
		/// set} always throw <seealso cref="IllegalStateException"/>.</li>
		/// <li><seealso cref="ListIterator#add add"/> always throws {@link
		/// UnsupportedOperationException}.</li>
		/// <li><seealso cref="ListIterator#nextIndex nextIndex"/> always returns
		/// {@code 0}.</li>
		/// <li><seealso cref="ListIterator#previousIndex previousIndex"/> always
		/// returns {@code -1}.</li>
		/// </ul>
		/// 
		/// <para>Implementations of this method are permitted, but not
		/// required, to return the same object from multiple invocations.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements, if there were any, in the iterator </param>
		/// <returns> an empty list iterator
		/// @since 1.7 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> ListIterator<T> emptyListIterator()
		public static <T> ListIterator<T> EmptyListIterator()
		{
			return (ListIterator<T>) EmptyListIterator.EMPTY_ITERATOR;
		}

		private static class EmptyListIterator<E> extends EmptyIterator<E> implements ListIterator<E>
		{
			static final EmptyListIterator<Object> EMPTY_ITERATOR = new EmptyListIterator<>();

			public bool hasPrevious()
			{
				return false;
			}
			public E previous()
			{
				throw new NoSuchElementException();
			}
			public int nextIndex()
			{
				return 0;
			}
			public int previousIndex()
			{
				return -1;
			}
			public void set(E e)
			{
				throw new IllegalStateException();
			}
			public void add(E e)
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Returns an enumeration that has no elements.  More precisely,
		/// 
		/// <ul>
		/// <li><seealso cref="Enumeration#hasMoreElements hasMoreElements"/> always
		/// returns {@code false}.</li>
		/// <li> <seealso cref="Enumeration#nextElement nextElement"/> always throws
		/// <seealso cref="NoSuchElementException"/>.</li>
		/// </ul>
		/// 
		/// <para>Implementations of this method are permitted, but not
		/// required, to return the same object from multiple invocations.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the enumeration </param>
		/// <returns> an empty enumeration
		/// @since 1.7 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> java.util.Iterator<T> emptyEnumeration()
		public static <T> IEnumerator<T> EmptyEnumeration()
		{
			return (IEnumerator<T>) EmptyEnumeration.EMPTY_ENUMERATION;
		}

		private static class EmptyEnumeration<E> implements IEnumerator<E>
		{
			static final EmptyEnumeration<Object> EMPTY_ENUMERATION = new EmptyEnumeration<>();

			public bool hasMoreElements()
			{
				return false;
			}
			public E nextElement()
			{
				throw new NoSuchElementException();
			}
		}

		/// <summary>
		/// The empty set (immutable).  This set is serializable.
		/// </summary>
		/// <seealso cref= #emptySet() </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static final Set EMPTY_SET = new EmptySet<>();
		public static final Set EMPTY_SET = new EmptySet<>();

		/// <summary>
		/// Returns an empty set (immutable).  This set is serializable.
		/// Unlike the like-named field, this method is parameterized.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty set:
		/// <pre>
		///     Set&lt;String&gt; s = Collections.emptySet();
		/// </pre>
		/// @implNote Implementations of this method need not create a separate
		/// {@code Set} object for each call.  Using this method is likely to have
		/// comparable cost to using the like-named field.  (Unlike this method, the
		/// field does not provide type safety.)
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <returns> the empty set
		/// </returns>
		/// <seealso cref= #EMPTY_SET
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final <T> Set<T> emptySet()
		public static final <T> Set<T> EmptySet()
		{
			return (Set<T>) EMPTY_SET;
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class EmptySet<E> extends AbstractSet<E> implements Serializable
		{
			private static final long serialVersionUID = 1582296315990362920L;

			public Iterator<E> iterator()
			{
				return EmptyIterator();
			}

			public int size()
			{
				return 0;
			}
			public bool Empty
			{
				return true;
			}

			public bool contains(Object obj)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> c)
			public bool containsAll(Collection<?> c)
			{
				return c.Empty;
			}

			public Object[] toArray()
			{
				return new Object[0];
			}

			public <T> T[] toArray(T[] a)
			{
				if (a.length > 0)
				{
					a[0] = null;
				}
				return a;
			}

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				Objects.RequireNonNull(action);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				Objects.RequireNonNull(filter);
				return false;
			}
			public Spliterator<E> spliterator()
			{
				return Spliterators.EmptySpliterator();
			}

			// Preserves singleton property
			private Object readResolve()
			{
				return EMPTY_SET;
			}
		}

		/// <summary>
		/// Returns an empty sorted set (immutable).  This set is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty
		/// sorted set:
		/// <pre> {@code
		///     SortedSet<String> s = Collections.emptySortedSet();
		/// }</pre>
		/// 
		/// @implNote Implementations of this method need not create a separate
		/// {@code SortedSet} object for each call.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> type of elements, if there were any, in the set </param>
		/// <returns> the empty sorted set
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <E> SortedSet<E> emptySortedSet()
		public static <E> SortedSet<E> EmptySortedSet()
		{
			return (SortedSet<E>) UnmodifiableNavigableSet.EMPTY_NAVIGABLE_SET;
		}

		/// <summary>
		/// Returns an empty navigable set (immutable).  This set is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty
		/// navigable set:
		/// <pre> {@code
		///     NavigableSet<String> s = Collections.emptyNavigableSet();
		/// }</pre>
		/// 
		/// @implNote Implementations of this method need not
		/// create a separate {@code NavigableSet} object for each call.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> type of elements, if there were any, in the set </param>
		/// <returns> the empty navigable set
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <E> NavigableSet<E> emptyNavigableSet()
		public static <E> NavigableSet<E> EmptyNavigableSet()
		{
			return (NavigableSet<E>) UnmodifiableNavigableSet.EMPTY_NAVIGABLE_SET;
		}

		/// <summary>
		/// The empty list (immutable).  This list is serializable.
		/// </summary>
		/// <seealso cref= #emptyList() </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static final List EMPTY_LIST = new EmptyList<>();
		public static final List EMPTY_LIST = new EmptyList<>();

		/// <summary>
		/// Returns an empty list (immutable).  This list is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty list:
		/// <pre>
		///     List&lt;String&gt; s = Collections.emptyList();
		/// </pre>
		/// 
		/// @implNote
		/// Implementations of this method need not create a separate <tt>List</tt>
		/// object for each call.   Using this method is likely to have comparable
		/// cost to using the like-named field.  (Unlike this method, the field does
		/// not provide type safety.)
		/// 
		/// </para>
		/// </summary>
		/// @param <T> type of elements, if there were any, in the list </param>
		/// <returns> an empty immutable list
		/// </returns>
		/// <seealso cref= #EMPTY_LIST
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final <T> List<T> emptyList()
		public static final <T> List<T> EmptyList()
		{
			return (List<T>) EMPTY_LIST;
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class EmptyList<E> extends AbstractList<E> implements RandomAccess, Serializable
		{
			private static final long serialVersionUID = 8842843931221139166L;

			public Iterator<E> iterator()
			{
				return EmptyIterator();
			}
			public ListIterator<E> listIterator()
			{
				return EmptyListIterator();
			}

			public int size()
			{
				return 0;
			}
			public bool Empty
			{
				return true;
			}

			public bool contains(Object obj)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> c)
			public bool containsAll(Collection<?> c)
			{
				return c.Empty;
			}

			public Object[] toArray()
			{
				return new Object[0];
			}

			public <T> T[] toArray(T[] a)
			{
				if (a.length > 0)
				{
					a[0] = null;
				}
				return a;
			}

			public E get(int index)
			{
				throw new IndexOutOfBoundsException("Index: " + index);
			}

			public bool Equals(Object o)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (o instanceof List) && ((List<?>)o).isEmpty();
				return (o is List) && ((List<?>)o).Count == 0;
			}

			public int GetHashCode()
			{
				return 1;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				Objects.RequireNonNull(filter);
				return false;
			}
			public void replaceAll(UnaryOperator<E> @operator)
			{
				Objects.RequireNonNull(@operator);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void sort(Comparator<?> c)
			{
			}

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				Objects.RequireNonNull(action);
			}

			public Spliterator<E> spliterator()
			{
				return Spliterators.EmptySpliterator();
			}

			// Preserves singleton property
			private Object readResolve()
			{
				return EMPTY_LIST;
			}
		}

		/// <summary>
		/// The empty map (immutable).  This map is serializable.
		/// </summary>
		/// <seealso cref= #emptyMap()
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static final Map EMPTY_MAP = new EmptyMap<>();
		public static final Map EMPTY_MAP = new EmptyMap<>();

		/// <summary>
		/// Returns an empty map (immutable).  This map is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty map:
		/// <pre>
		///     Map&lt;String, Date&gt; s = Collections.emptyMap();
		/// </pre>
		/// @implNote Implementations of this method need not create a separate
		/// {@code Map} object for each call.  Using this method is likely to have
		/// comparable cost to using the like-named field.  (Unlike this method, the
		/// field does not provide type safety.)
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <returns> an empty map </returns>
		/// <seealso cref= #EMPTY_MAP
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final <K,V> Map<K,V> emptyMap()
		public static final <K, V> Map<K, V> EmptyMap()
		{
			return (Map<K, V>) EMPTY_MAP;
		}

		/// <summary>
		/// Returns an empty sorted map (immutable).  This map is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty map:
		/// <pre> {@code
		///     SortedMap<String, Date> s = Collections.emptySortedMap();
		/// }</pre>
		/// 
		/// @implNote Implementations of this method need not create a separate
		/// {@code SortedMap} object for each call.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <returns> an empty sorted map
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final <K,V> SortedMap<K,V> emptySortedMap()
		public static final <K, V> SortedMap<K, V> EmptySortedMap()
		{
			return (SortedMap<K, V>) UnmodifiableNavigableMap.EMPTY_NAVIGABLE_MAP;
		}

		/// <summary>
		/// Returns an empty navigable map (immutable).  This map is serializable.
		/// 
		/// <para>This example illustrates the type-safe way to obtain an empty map:
		/// <pre> {@code
		///     NavigableMap<String, Date> s = Collections.emptyNavigableMap();
		/// }</pre>
		/// 
		/// @implNote Implementations of this method need not create a separate
		/// {@code NavigableMap} object for each call.
		/// 
		/// </para>
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <returns> an empty navigable map
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final <K,V> NavigableMap<K,V> emptyNavigableMap()
		public static final <K, V> NavigableMap<K, V> EmptyNavigableMap()
		{
			return (NavigableMap<K, V>) UnmodifiableNavigableMap.EMPTY_NAVIGABLE_MAP;
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class EmptyMap<K, V> extends AbstractMap<K, V> implements Serializable
		{
			private static final long serialVersionUID = 6428348081105594320L;

			public int size()
			{
				return 0;
			}
			public bool Empty
			{
				return true;
			}
			public bool containsKey(Object key)
			{
				return false;
			}
			public bool containsValue(Object value)
			{
				return false;
			}
			public V get(Object key)
			{
				return null;
			}
			public Set<K> keySet()
			{
				return EmptySet();
			}
			public Collection<V> values()
			{
				return EmptySet();
			}
			public Set<Map_Entry<K, V>> entrySet()
			{
				return EmptySet();
			}

			public bool Equals(Object o)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (o instanceof Map) && ((Map<?,?>)o).isEmpty();
				return (o is Map) && ((Map<?, ?>)o).Empty;
			}

			public int GetHashCode()
			{
				return 0;
			}

			// Override default methods in Map
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public V getOrDefault(Object k, V defaultValue)
			public V getOrDefault(Object k, V defaultValue)
			{
				return defaultValue;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(BiConsumer<?, ?> action)
			{
				Objects.RequireNonNull(action);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void replaceAll(BiFunction<?, ?, ?> function)
			{
				Objects.RequireNonNull(function);
			}

			public V putIfAbsent(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

			public bool remove(Object key, Object value)
			{
				throw new UnsupportedOperationException();
			}

			public bool replace(K key, V oldValue, V newValue)
			{
				throw new UnsupportedOperationException();
			}

			public V replace(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfAbsent(K key, Function<?, ?> mappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfPresent(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V compute(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V merge(K key, V value, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

			// Preserves singleton property
			private Object readResolve()
			{
				return EMPTY_MAP;
			}
		}

		// Singleton collections

		/// <summary>
		/// Returns an immutable set containing only the specified object.
		/// The returned set is serializable.
		/// </summary>
		/// @param  <T> the class of the objects in the set </param>
		/// <param name="o"> the sole object to be stored in the returned set. </param>
		/// <returns> an immutable set containing only the specified object. </returns>
		public static <T> Set<T> Singleton(T o)
		{
			return new SingletonSet<>(o);
		}

		static <E> Iterator<E> SingletonIterator(final E e)
		{
			return new IteratorAnonymousInnerClassHelper5();
		}

		/// <summary>
		/// Creates a {@code Spliterator} with only the specified element
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <returns> A singleton {@code Spliterator} </returns>
		static <T> Spliterator<T> SingletonSpliterator(final T element)
		{
			return new SpliteratorAnonymousInnerClassHelper();
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class SingletonSet<E> extends AbstractSet<E> implements Serializable
		{
			private static final long serialVersionUID = 3193687207550431679L;

			private final E element;

			SingletonSet(E e)
			{
				element = e;
			}

			public Iterator<E> iterator()
			{
				return SingletonIterator(element);
			}

			public int size()
			{
				return 1;
			}

			public bool contains(Object o)
			{
				return Eq(o, element);
			}

			// Override default methods for Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				action.accept(element);
			}
			public Spliterator<E> spliterator()
			{
				return SingletonSpliterator(element);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Returns an immutable list containing only the specified object.
		/// The returned list is serializable.
		/// </summary>
		/// @param  <T> the class of the objects in the list </param>
		/// <param name="o"> the sole object to be stored in the returned list. </param>
		/// <returns> an immutable list containing only the specified object.
		/// @since 1.3 </returns>
		public static <T> List<T> SingletonList(T o)
		{
			return new SingletonList<>(o);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class SingletonList<E> extends AbstractList<E> implements RandomAccess, Serializable
		{

			private static final long serialVersionUID = 3093736618740652951L;

			private final E element;

			SingletonList(E obj)
			{
				element = obj;
			}

			public Iterator<E> iterator()
			{
				return SingletonIterator(element);
			}

			public int size()
			{
				return 1;
			}

			public bool contains(Object obj)
			{
				return Eq(obj, element);
			}

			public E get(int index)
			{
				if (index != 0)
				{
				  throw new IndexOutOfBoundsException("Index: " + index + ", Size: 1");
				}
				return element;
			}

			// Override default methods for Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				action.accept(element);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				throw new UnsupportedOperationException();
			}
			public void replaceAll(UnaryOperator<E> @operator)
			{
				throw new UnsupportedOperationException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void sort(Comparator<? base E> c)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void sort(Comparator<?> c)
			{
			}
			public Spliterator<E> spliterator()
			{
				return SingletonSpliterator(element);
			}
		}

		/// <summary>
		/// Returns an immutable map, mapping only the specified key to the
		/// specified value.  The returned map is serializable.
		/// </summary>
		/// @param <K> the class of the map keys </param>
		/// @param <V> the class of the map values </param>
		/// <param name="key"> the sole key to be stored in the returned map. </param>
		/// <param name="value"> the value to which the returned map maps <tt>key</tt>. </param>
		/// <returns> an immutable map containing only the specified key-value
		///         mapping.
		/// @since 1.3 </returns>
		public static <K, V> Map<K, V> SingletonMap(K key, V value)
		{
			return new SingletonMap<>(key, value);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class SingletonMap<K, V> extends AbstractMap<K, V> implements Serializable
		{
			private static final long serialVersionUID = -6979724477215052911L;

			private final K k;
			private final V v;

			SingletonMap(K key, V value)
			{
				k = key;
				v = value;
			}

			public int size()
			{
				return 1;
			}
			public bool Empty
			{
				return false;
			}
			public bool containsKey(Object key)
			{
				return Eq(key, k);
			}
			public bool containsValue(Object value)
			{
				return Eq(value, v);
			}
			public V get(Object key)
			{
				return (Eq(key, k) ? v : null);
			}

			private transient Set<K> keySet;
			private transient Set<Map_Entry<K, V>> entrySet;
			private transient Collection<V> values;

			public Set<K> keySet()
			{
				if (keySet == null)
				{
					keySet = Singleton(k);
				}
				return keySet;
			}

			public Set<Map_Entry<K, V>> entrySet()
			{
				if (entrySet == null)
				{
					entrySet = Collections.Singleton<Map_Entry<K, V>>(new SimpleImmutableEntry<>(k, v));
				}
				return entrySet;
			}

			public Collection<V> values()
			{
				if (values == null)
				{
					values = Singleton(v);
				}
				return values;
			}

			// Override default methods in Map
			public V getOrDefault(Object key, V defaultValue)
			{
				return Eq(key, k) ? v : defaultValue;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(BiConsumer<?, ?> action)
			{
				action.accept(k, v);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void replaceAll(BiFunction<?, ?, ?> function)
			{
				throw new UnsupportedOperationException();
			}

			public V putIfAbsent(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

			public bool remove(Object key, Object value)
			{
				throw new UnsupportedOperationException();
			}

			public bool replace(K key, V oldValue, V newValue)
			{
				throw new UnsupportedOperationException();
			}

			public V replace(K key, V value)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfAbsent(K key, Function<?, ?> mappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V computeIfPresent(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V compute(K key, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public V merge(K key, V value, BiFunction<?, ?, ?> remappingFunction)
			{
				throw new UnsupportedOperationException();
			}
		}

		// Miscellaneous

		/// <summary>
		/// Returns an immutable list consisting of <tt>n</tt> copies of the
		/// specified object.  The newly allocated data object is tiny (it contains
		/// a single reference to the data object).  This method is useful in
		/// combination with the <tt>List.addAll</tt> method to grow lists.
		/// The returned list is serializable.
		/// </summary>
		/// @param  <T> the class of the object to copy and of the objects
		///         in the returned list. </param>
		/// <param name="n"> the number of elements in the returned list. </param>
		/// <param name="o"> the element to appear repeatedly in the returned list. </param>
		/// <returns> an immutable list consisting of <tt>n</tt> copies of the
		///         specified object. </returns>
		/// <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
		/// <seealso cref=    List#addAll(Collection) </seealso>
		/// <seealso cref=    List#addAll(int, Collection) </seealso>
		public static <T> List<T> NCopies(int n, T o)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException("List length = " + n);
			}
			return new CopiesList<>(n, o);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class CopiesList<E> extends AbstractList<E> implements RandomAccess, Serializable
		{
			private static final long serialVersionUID = 2739099268398711800L;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n;
			int n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final E element;
			E element;

			CopiesList(int n, E e)
			{
				Debug.Assert(n >= 0);
				this.n = n;
				element = e;
			}

			public int size()
			{
				return n;
			}

			public bool contains(Object obj)
			{
				return n != 0 && Eq(obj, element);
			}

			public int indexOf(Object o)
			{
				return contains(o) ? 0 : -1;
			}

			public int lastIndexOf(Object o)
			{
				return contains(o) ? n - 1 : -1;
			}

			public E get(int index)
			{
				if (index < 0 || index >= n)
				{
					throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + n);
				}
				return element;
			}

			public Object[] toArray()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] a = new Object[n];
				Object[] a = new Object[n];
				if (element != null)
				{
					Arrays.Fill(a, 0, n, element);
				}
				return a;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public <T> T[] toArray(T[] a)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = this.n;
				int n = this.n;
				if (a.length < n)
				{
					a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), n);
					if (element != null)
					{
						Arrays.Fill(a, 0, n, element);
					}
				}
				else
				{
					Arrays.Fill(a, 0, n, element);
					if (a.length > n)
					{
						a[n] = null;
					}
				}
				return a;
			}

			public List<E> subList(int fromIndex, int toIndex)
			{
				if (fromIndex < 0)
				{
					throw new IndexOutOfBoundsException("fromIndex = " + fromIndex);
				}
				if (toIndex > n)
				{
					throw new IndexOutOfBoundsException("toIndex = " + toIndex);
				}
				if (fromIndex > toIndex)
				{
					throw new IllegalArgumentException("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
				}
				return new CopiesList<>(toIndex - fromIndex, element);
			}

			// Override default methods in Collection
			public Stream<E> stream()
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return IntStream.range(0, n).mapToObj(i => element);
			}

			public Stream<E> parallelStream()
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return IntStream.range(0, n).parallel().mapToObj(i => element);
			}

			public Spliterator<E> spliterator()
			{
				return stream().spliterator();
			}
		}

		/// <summary>
		/// Returns a comparator that imposes the reverse of the <em>natural
		/// ordering</em> on a collection of objects that implement the
		/// {@code Comparable} interface.  (The natural ordering is the ordering
		/// imposed by the objects' own {@code compareTo} method.)  This enables a
		/// simple idiom for sorting (or maintaining) collections (or arrays) of
		/// objects that implement the {@code Comparable} interface in
		/// reverse-natural-order.  For example, suppose {@code a} is an array of
		/// strings. Then: <pre>
		///          Arrays.sort(a, Collections.reverseOrder());
		/// </pre> sorts the array in reverse-lexicographic (alphabetical) order.<para>
		/// 
		/// The returned comparator is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects compared by the comparator </param>
		/// <returns> A comparator that imposes the reverse of the <i>natural
		///         ordering</i> on a collection of objects that implement
		///         the <tt>Comparable</tt> interface. </returns>
		/// <seealso cref= Comparable </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> Comparator<T> reverseOrder()
		public static <T> Comparator<T> ReverseOrder()
		{
			return (Comparator<T>) ReverseComparator.REVERSE_ORDER;
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class ReverseComparator implements Comparator<Comparable<Object>>, Serializable
		{

			private static final long serialVersionUID = 7207038068494060240L;

			static final ReverseComparator REVERSE_ORDER = new ReverseComparator();

			public int compare(Comparable<Object> c1, Comparable<Object> c2)
			{
				return c2.CompareTo(c1);
			}

			private Object readResolve()
			{
				return Collections.ReverseOrder();
			}

			public Comparator<Comparable<Object>> reversed()
			{
				return Comparator.naturalOrder();
			}
		}

		/// <summary>
		/// Returns a comparator that imposes the reverse ordering of the specified
		/// comparator.  If the specified comparator is {@code null}, this method is
		/// equivalent to <seealso cref="#reverseOrder()"/> (in other words, it returns a
		/// comparator that imposes the reverse of the <em>natural ordering</em> on
		/// a collection of objects that implement the Comparable interface).
		/// 
		/// <para>The returned comparator is serializable (assuming the specified
		/// comparator is also serializable or {@code null}).
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the class of the objects compared by the comparator </param>
		/// <param name="cmp"> a comparator who's ordering is to be reversed by the returned
		/// comparator or {@code null} </param>
		/// <returns> A comparator that imposes the reverse ordering of the
		///         specified comparator.
		/// @since 1.5 </returns>
		public static <T> Comparator<T> ReverseOrder(Comparator<T> cmp)
		{
			if (cmp == null)
			{
				return ReverseOrder();
			}

			if (cmp is ReverseComparator2)
			{
				return ((ReverseComparator2<T>)cmp).Cmp;
			}

			return new ReverseComparator2<>(cmp);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class ReverseComparator2<T> implements Comparator<T>, Serializable
		{
			private static final long serialVersionUID = 4374092139857L;

			/// <summary>
			/// The comparator specified in the static factory.  This will never
			/// be null, as the static factory returns a ReverseComparator
			/// instance if its argument is null.
			/// 
			/// @serial
			/// </summary>
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Comparator<T> cmp;
			Comparator<T> cmp;

			ReverseComparator2(Comparator<T> cmp)
			{
				Debug.Assert(cmp != null);
				this.cmp = cmp;
			}

			public int compare(T t1, T t2)
			{
				return cmp.Compare(t2, t1);
			}

			public bool Equals(Object o)
			{
				return (o == this) || (o is ReverseComparator2 && cmp.Equals(((ReverseComparator2)o).cmp));
			}

			public int GetHashCode()
			{
				return cmp.HashCode() ^ Integer.MinValue;
			}

			public Comparator<T> reversed()
			{
				return cmp;
			}
		}

		/// <summary>
		/// Returns an enumeration over the specified collection.  This provides
		/// interoperability with legacy APIs that require an enumeration
		/// as input.
		/// </summary>
		/// @param  <T> the class of the objects in the collection </param>
		/// <param name="c"> the collection for which an enumeration is to be returned. </param>
		/// <returns> an enumeration over the specified collection. </returns>
		/// <seealso cref= Enumeration </seealso>
		public static <T> IEnumerator<T> Enumeration(final Collection<T> c)
		{
			return new IteratorAnonymousInnerClassHelper();
		}

		/// <summary>
		/// Returns an array list containing the elements returned by the
		/// specified enumeration in the order they are returned by the
		/// enumeration.  This method provides interoperability between
		/// legacy APIs that return enumerations and new APIs that require
		/// collections.
		/// </summary>
		/// @param <T> the class of the objects returned by the enumeration </param>
		/// <param name="e"> enumeration providing elements for the returned
		///          array list </param>
		/// <returns> an array list containing the elements returned
		///         by the specified enumeration.
		/// @since 1.4 </returns>
		/// <seealso cref= Enumeration </seealso>
		/// <seealso cref= ArrayList </seealso>
		public static <T> List<T> List(IEnumerator<T> e)
		{
			List<T> l = new List<T>();
			while (e.hasMoreElements())
			{
				l.Add(e.nextElement());
			}
			return l;
		}

		/// <summary>
		/// Returns true if the specified arguments are equal, or both null.
		/// 
		/// NB: Do not replace with Object.equals until JDK-8015417 is resolved.
		/// </summary>
		static bool Eq(Object o1, Object o2)
		{
			return o1 == null ? o2 == null : o1.Equals(o2);
		}

		/// <summary>
		/// Returns the number of elements in the specified collection equal to the
		/// specified object.  More formally, returns the number of elements
		/// <tt>e</tt> in the collection such that
		/// <tt>(o == null ? e == null : o.equals(e))</tt>.
		/// </summary>
		/// <param name="c"> the collection in which to determine the frequency
		///     of <tt>o</tt> </param>
		/// <param name="o"> the object whose frequency is to be determined </param>
		/// <returns> the number of elements in {@code c} equal to {@code o} </returns>
		/// <exception cref="NullPointerException"> if <tt>c</tt> is null
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static int frequency(Collection<?> c, Object o)
		public static int Frequency(Collection<?> c, Object o)
		{
			int result = 0;
			if (o == null)
			{
				foreach (Object e in c)
				{
					if (e == null)
					{
						result++;
					}
				}
			}
			else
			{
				foreach (Object e in c)
				{
					if (o.Equals(e))
					{
						result++;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Returns {@code true} if the two specified collections have no
		/// elements in common.
		/// 
		/// <para>Care must be exercised if this method is used on collections that
		/// do not comply with the general contract for {@code Collection}.
		/// Implementations may elect to iterate over either collection and test
		/// for containment in the other collection (or to perform any equivalent
		/// computation).  If either collection uses a nonstandard equality test
		/// (as does a <seealso cref="SortedSet"/> whose ordering is not <em>compatible with
		/// equals</em>, or the key set of an <seealso cref="IdentityHashMap"/>), both
		/// collections must use the same nonstandard equality test, or the
		/// result of this method is undefined.
		/// 
		/// </para>
		/// <para>Care must also be exercised when using collections that have
		/// restrictions on the elements that they may contain. Collection
		/// implementations are allowed to throw exceptions for any operation
		/// involving elements they deem ineligible. For absolute safety the
		/// specified collections should contain only elements which are
		/// eligible elements for both collections.
		/// 
		/// </para>
		/// <para>Note that it is permissible to pass the same collection in both
		/// parameters, in which case the method will return {@code true} if and
		/// only if the collection is empty.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c1"> a collection </param>
		/// <param name="c2"> a collection </param>
		/// <returns> {@code true} if the two specified collections have no
		/// elements in common. </returns>
		/// <exception cref="NullPointerException"> if either collection is {@code null}. </exception>
		/// <exception cref="NullPointerException"> if one collection contains a {@code null}
		/// element and {@code null} is not an eligible element for the other collection.
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="ClassCastException"> if one collection contains an element that is
		/// of a type which is ineligible for the other collection.
		/// (<a href="Collection.html#optional-restrictions">optional</a>)
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static boolean disjoint(Collection<?> c1, Collection<?> c2)
		public static bool Disjoint(Collection<?> c1, Collection<?> c2)
		{
			// The collection to be used for contains(). Preference is given to
			// the collection who's contains() has lower O() complexity.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Collection<?> contains = c2;
			Collection<?> contains = c2;
			// The collection to be iterated. If the collections' contains() impl
			// are of different O() complexity, the collection with slower
			// contains() will be used for iteration. For collections who's
			// contains() are of the same complexity then best performance is
			// achieved by iterating the smaller collection.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Collection<?> iterate = c1;
			Collection<?> iterate = c1;

			// Performance optimization cases. The heuristics:
			//   1. Generally iterate over c1.
			//   2. If c1 is a Set then iterate over c2.
			//   3. If either collection is empty then result is always true.
			//   4. Iterate over the smaller Collection.
			if (c1 is Set)
			{
				// Use c1 for contains as a Set's contains() is expected to perform
				// better than O(N/2)
				iterate = c2;
				contains = c1;
			}
			else if (!(c2 is Set))
			{
				// Both are mere Collections. Iterate over smaller collection.
				// Example: If c1 contains 3 elements and c2 contains 50 elements and
				// assuming contains() requires ceiling(N/2) comparisons then
				// checking for all c1 elements in c2 would require 75 comparisons
				// (3 * ceiling(50/2)) vs. checking all c2 elements in c1 requiring
				// 100 comparisons (50 * ceiling(3/2)).
				int c1size = c1.size();
				int c2size = c2.size();
				if (c1size == 0 || c2size == 0)
				{
					// At least one collection is empty. Nothing will match.
					return true;
				}

				if (c1size > c2size)
				{
					iterate = c2;
					contains = c1;
				}
			}

			foreach (Object e in iterate)
			{
				if (contains.Contains(e))
				{
				   // Found a common element. Collections are not disjoint.
					return false;
				}
			}

			// No common elements were found.
			return true;
		}

		/// <summary>
		/// Adds all of the specified elements to the specified collection.
		/// Elements to be added may be specified individually or as an array.
		/// The behavior of this convenience method is identical to that of
		/// <tt>c.addAll(Arrays.asList(elements))</tt>, but this method is likely
		/// to run significantly faster under most implementations.
		/// 
		/// <para>When elements are specified individually, this method provides a
		/// convenient way to add a few elements to an existing collection:
		/// <pre>
		///     Collections.addAll(flavors, "Peaches 'n Plutonium", "Rocky Racoon");
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the elements to add and of the collection </param>
		/// <param name="c"> the collection into which <tt>elements</tt> are to be inserted </param>
		/// <param name="elements"> the elements to insert into <tt>c</tt> </param>
		/// <returns> <tt>true</tt> if the collection changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if <tt>c</tt> does not support
		///         the <tt>add</tt> operation </exception>
		/// <exception cref="NullPointerException"> if <tt>elements</tt> contains one or more
		///         null values and <tt>c</tt> does not permit null elements, or
		///         if <tt>c</tt> or <tt>elements</tt> are <tt>null</tt> </exception>
		/// <exception cref="IllegalArgumentException"> if some property of a value in
		///         <tt>elements</tt> prevents it from being added to <tt>c</tt> </exception>
		/// <seealso cref= Collection#addAll(Collection)
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SafeVarargs public static <T> boolean addAll(Collection<? base T> c, T... elements)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public static <T> bool AddAll(Collection<?> c, T... elements)
		{
			bool result = false;
			foreach (T element in elements)
			{
				result |= c.add(element);
			}
			return result;
		}

		/// <summary>
		/// Returns a set backed by the specified map.  The resulting set displays
		/// the same ordering, concurrency, and performance characteristics as the
		/// backing map.  In essence, this factory method provides a <seealso cref="Set"/>
		/// implementation corresponding to any <seealso cref="Map"/> implementation.  There
		/// is no need to use this method on a <seealso cref="Map"/> implementation that
		/// already has a corresponding <seealso cref="Set"/> implementation (such as {@link
		/// HashMap} or <seealso cref="TreeMap"/>).
		/// 
		/// <para>Each method invocation on the set returned by this method results in
		/// exactly one method invocation on the backing map or its <tt>keySet</tt>
		/// view, with one exception.  The <tt>addAll</tt> method is implemented
		/// as a sequence of <tt>put</tt> invocations on the backing map.
		/// 
		/// </para>
		/// <para>The specified map must be empty at the time this method is invoked,
		/// and should not be accessed directly after this method returns.  These
		/// conditions are ensured if the map is created empty, passed directly
		/// to this method, and no reference to the map is retained, as illustrated
		/// in the following code fragment:
		/// <pre>
		///    Set&lt;Object&gt; weakHashSet = Collections.newSetFromMap(
		///        new WeakHashMap&lt;Object, Boolean&gt;());
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the class of the map keys and of the objects in the
		///        returned set </param>
		/// <param name="map"> the backing map </param>
		/// <returns> the set backed by the map </returns>
		/// <exception cref="IllegalArgumentException"> if <tt>map</tt> is not empty
		/// @since 1.6 </exception>
		public static <E> Set<E> NewSetFromMap(Map<E, Boolean> map)
		{
			return new SetFromMap<>(map);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		private static class SetFromMap<E> extends AbstractSet<E> implements Set<E>, Serializable
		{
			private final Map<E, Boolean> m; // The backing map
			private transient Set<E> s; // Its keySet

			SetFromMap(Map<E, Boolean> map)
			{
				if (!map.Empty)
				{
					throw new IllegalArgumentException("Map is non-empty");
				}
				m = map;
				s = map.Keys;
			}

			public void clear()
			{
				m.clear();
			}
			public int size()
			{
				return m.size();
			}
			public bool Empty
			{
				return m.Empty;
			}
			public bool contains(Object o)
			{
				return m.containsKey(o);
			}
			public bool remove(Object o)
			{
				return m.remove(o) != null;
			}
			public bool add(E e)
			{
				return m.put(e, true) == null;
			}
			public Iterator<E> iterator()
			{
				return s.GetEnumerator();
			}
			public Object[] toArray()
			{
				return s.toArray();
			}
			public <T> T[] toArray(T[] a)
			{
				return s.toArray(a);
			}
			public String ToString()
			{
				return s.ToString();
			}
			public int GetHashCode()
			{
				return s.HashCode();
			}
			public bool Equals(Object o)
			{
				return o == this || s.Equals(o);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> c)
			public bool containsAll(Collection<?> c)
			{
				return s.containsAll(c);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> c)
			public bool removeAll(Collection<?> c)
			{
				return s.removeAll(c);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> c)
			public bool retainAll(Collection<?> c)
			{
				return s.retainAll(c);
			}
			// addAll is the only inherited implementation

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				s.forEach(action);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				return s.removeIf(filter);
			}

			public Spliterator<E> spliterator()
			{
				return s.spliterator();
			}
			public Stream<E> stream()
			{
				return s.stream();
			}
			public Stream<E> parallelStream()
			{
				return s.parallelStream();
			}

			private static final long serialVersionUID = 2454657854757543876L;

			private void readObject(java.io.ObjectInputStream stream) throws IOException, ClassNotFoundException
			{
				stream.defaultReadObject();
				s = m.Keys;
			}
		}

		/// <summary>
		/// Returns a view of a <seealso cref="Deque"/> as a Last-in-first-out (Lifo)
		/// <seealso cref="Queue"/>. Method <tt>add</tt> is mapped to <tt>push</tt>,
		/// <tt>remove</tt> is mapped to <tt>pop</tt> and so on. This
		/// view can be useful when you would like to use a method
		/// requiring a <tt>Queue</tt> but you need Lifo ordering.
		/// 
		/// <para>Each method invocation on the queue returned by this method
		/// results in exactly one method invocation on the backing deque, with
		/// one exception.  The <seealso cref="Queue#addAll addAll"/> method is
		/// implemented as a sequence of <seealso cref="Deque#addFirst addFirst"/>
		/// invocations on the backing deque.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the class of the objects in the deque </param>
		/// <param name="deque"> the deque </param>
		/// <returns> the queue
		/// @since  1.6 </returns>
		public static <T> Queue<T> AsLifoQueue(Deque<T> deque)
		{
			return new AsLIFOQueue<>(deque);
		}

		/// <summary>
		/// @serial include
		/// </summary>
		static class AsLIFOQueue<E> extends AbstractQueue<E> implements Queue<E>, Serializable
		{
			private static final long serialVersionUID = 1802017725587941708L;
			private final Deque<E> q;
			AsLIFOQueue(Deque<E> q)
			{
				this.q = q;
			}
			public bool add(E e)
			{
				q.addFirst(e);
				return true;
			}
			public bool offer(E e)
			{
				return q.offerFirst(e);
			}
			public E poll()
			{
				return q.pollFirst();
			}
			public E remove()
			{
				return q.removeFirst();
			}
			public E peek()
			{
				return q.peekFirst();
			}
			public E element()
			{
				return q.First;
			}
			public void clear()
			{
				q.clear();
			}
			public int size()
			{
				return q.size();
			}
			public bool Empty
			{
				return q.Empty;
			}
			public bool contains(Object o)
			{
				return q.contains(o);
			}
			public bool remove(Object o)
			{
				return q.remove(o);
			}
			public Iterator<E> iterator()
			{
				return q.GetEnumerator();
			}
			public Object[] toArray()
			{
				return q.toArray();
			}
			public <T> T[] toArray(T[] a)
			{
				return q.toArray(a);
			}
			public String ToString()
			{
				return q.ToString();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean containsAll(Collection<?> c)
			public bool containsAll(Collection<?> c)
			{
				return q.containsAll(c);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean removeAll(Collection<?> c)
			public bool removeAll(Collection<?> c)
			{
				return q.removeAll(c);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public boolean retainAll(Collection<?> c)
			public bool retainAll(Collection<?> c)
			{
				return q.retainAll(c);
			}
			// We use inherited addAll; forwarding addAll would be wrong

			// Override default methods in Collection
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public void forEach(Consumer<?> action)
			{
				q.forEach(action);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public bool removeIf(Predicate<?> filter)
			{
				return q.removeIf(filter);
			}
			public Spliterator<E> spliterator()
			{
				return q.spliterator();
			}
			public Stream<E> stream()
			{
				return q.stream();
			}
			public Stream<E> parallelStream()
			{
				return q.parallelStream();
			}
		}
	}


	private class IteratorAnonymousInnerClassHelper : Iterator<E>
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.util.Collection<JavaToDotNetGenericWildcard> c;
		private ICollection<?> c;

		public IteratorAnonymousInnerClassHelper<T1>(ICollection<T1> c)
		{
			this.c = c;
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Iterator<? extends E> i = c.iterator();
		private readonly Iterator<?> i = c.GetEnumerator();

		public virtual bool HasNext()
		{
			return i.hasNext();
		}
		public virtual E Next()
		{
			return i.next();
		}
		public virtual void Remove()
		{
			throw new UnsupportedOperationException();
		}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
		public virtual void forEachRemaining<T1>(Consumer<T1> action)
		{
			// Use backing collection version
			i.forEachRemaining(action);
		}
	}

	private class ListIteratorAnonymousInnerClassHelper : ListIterator<E>
	{
		private IList<T1> list;

		public ListIteratorAnonymousInnerClassHelper(IList<T1> list)
		{
			this.list = list;
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final ListIterator<? extends E> i = list.listIterator(index);
		private readonly ListIterator<?> i = list.listIterator(index);

		public virtual bool HasNext()
		{
			return i.hasNext();
		}
		public virtual E Next()
		{
			return i.next();
		}
		public virtual bool HasPrevious()
		{
			return i.hasPrevious();
		}
		public virtual E Previous()
		{
			return i.previous();
		}
		public virtual int NextIndex()
		{
			return i.nextIndex();
		}
		public virtual int PreviousIndex()
		{
			return i.previousIndex();
		}

		public virtual void Remove()
		{
			throw new UnsupportedOperationException();
		}
		public virtual void Set(E e)
		{
			throw new UnsupportedOperationException();
		}
		public virtual void Add(E e)
		{
			throw new UnsupportedOperationException();
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
		public virtual void forEachRemaining<T1>(Consumer<T1> action)
		{
			i.forEachRemaining(action);
		}
	}

	private class IteratorAnonymousInnerClassHelper2 : Iterator<Map_Entry<K, V>>
	{
		public IteratorAnonymousInnerClassHelper2()
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Iterator<? extends Map_Entry<? extends K, ? extends V>> i = c.iterator();
		private readonly Iterator<?> i = c.GetEnumerator();

		public virtual bool HasNext()
		{
			return i.hasNext();
		}
		public virtual Map_Entry<K, V> Next()
		{
			return new UnmodifiableEntry<>(i.next());
		}
		public virtual void Remove()
		{
			throw new UnsupportedOperationException();
		}
	}

	private class IteratorAnonymousInnerClassHelper3 : Iterator<E>
	{
		private IEnumerator<E> it;

		public IteratorAnonymousInnerClassHelper3(IEnumerator<E> it)
		{
			this.it = it;
		}

		public virtual bool HasNext()
		{
			return it.hasNext();
		}
		public virtual E Next()
		{
			return it.next();
		}
		public virtual void Remove()
		{
			it.remove();
		}
	}

	private class ListIteratorAnonymousInnerClassHelper2 : ListIterator<E>
	{
		private IEnumerator<E> i;

		public ListIteratorAnonymousInnerClassHelper2(IEnumerator<E> i)
		{
			this.i = i;
		}

		public virtual bool HasNext()
		{
			return i.hasNext();
		}
		public virtual E Next()
		{
			return i.next();
		}
		public virtual bool HasPrevious()
		{
			return i.hasPrevious();
		}
		public virtual E Previous()
		{
			return i.previous();
		}
		public virtual int NextIndex()
		{
			return i.nextIndex();
		}
		public virtual int PreviousIndex()
		{
			return i.previousIndex();
		}
		public virtual void Remove()
		{
			i.remove();
		}

		public virtual void Set(E e)
		{
			i.set(typeCheck(e));
		}

		public virtual void Add(E e)
		{
			i.add(typeCheck(e));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
		public virtual void forEachRemaining<T1>(Consumer<T1> action)
		{
			i.forEachRemaining(action);
		}
	}

	private class IteratorAnonymousInnerClassHelper4 : Iterator<Map_Entry<K, V>>
	{
		private Type valueType;
		private IEnumerator<Map_Entry<K, V>> i;

		public IteratorAnonymousInnerClassHelper4(Type valueType, IEnumerator<Map_Entry<K, V>> i)
		{
			this.valueType = valueType;
			this.i = i;
		}

		public virtual bool HasNext()
		{
			return i.hasNext();
		}
		public virtual void Remove()
		{
			i.remove();
		}

		public virtual Map_Entry<K, V> Next()
		{
			return checkedEntry(i.next(), valueType);
		}
	}

	private class IteratorAnonymousInnerClassHelper5 : Iterator<E>
	{
		public IteratorAnonymousInnerClassHelper5()
		{
		}

		private bool hasNext = true;
		public virtual bool HasNext()
		{
			return hasNext;
		}
		public virtual E Next()
		{
			if (hasNext)
			{
				hasNext = false;
				return e;
			}
			throw new NoSuchElementException();
		}
		public virtual void Remove()
		{
			throw new UnsupportedOperationException();
		}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base E> action)
		public virtual void forEachRemaining<T1>(Consumer<T1> action)
		{
			Objects.RequireNonNull(action);
			if (hasNext)
			{
				action.Accept(e);
				hasNext = false;
			}
		}
	}

	private class SpliteratorAnonymousInnerClassHelper : Spliterator<T>
	{
		public SpliteratorAnonymousInnerClassHelper()
		{
		}

		internal long est = 1;

		public virtual Spliterator<T> TrySplit()
		{
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base T> consumer)
		public virtual bool tryAdvance<T1>(Consumer<T1> consumer)
		{
			Objects.RequireNonNull(consumer);
			if (est > 0)
			{
				est--;
				consumer.Accept(element);
				return true;
			}
			return false;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base T> consumer)
		public virtual void forEachRemaining<T1>(Consumer<T1> consumer)
		{
			tryAdvance(consumer);
		}

		public virtual long EstimateSize()
		{
			return est;
		}

		public virtual int Characteristics()
		{
			int value = (element != null) ? Spliterator_Fields.NONNULL : 0;

			return value | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.IMMUTABLE | Spliterator_Fields.DISTINCT | Spliterator_Fields.ORDERED;
		}
	}

	private class IteratorAnonymousInnerClassHelper : IEnumerator<T>
	{
		public IteratorAnonymousInnerClassHelper()
		{
		}

		private readonly Iterator<T> i = c.GetEnumerator();

		public virtual bool HasMoreElements()
		{
			return i.hasNext();
		}

		public virtual T NextElement()
		{
			return i.next();
		}
	}
}