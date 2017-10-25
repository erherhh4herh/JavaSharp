using System.Collections.Generic;

/*
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
 * Written by Doug Lea and Josh Bloch with assistance from members of JCP
 * JSR-166 Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util
{

	/// <summary>
	/// A <seealso cref="SortedSet"/> extended with navigation methods reporting
	/// closest matches for given search targets. Methods {@code lower},
	/// {@code floor}, {@code ceiling}, and {@code higher} return elements
	/// respectively less than, less than or equal, greater than or equal,
	/// and greater than a given element, returning {@code null} if there
	/// is no such element.  A {@code NavigableSet} may be accessed and
	/// traversed in either ascending or descending order.  The {@code
	/// descendingSet} method returns a view of the set with the senses of
	/// all relational and directional methods inverted. The performance of
	/// ascending operations and views is likely to be faster than that of
	/// descending ones.  This interface additionally defines methods
	/// {@code pollFirst} and {@code pollLast} that return and remove the
	/// lowest and highest element, if one exists, else returning {@code
	/// null}.  Methods {@code subSet}, {@code headSet},
	/// and {@code tailSet} differ from the like-named {@code
	/// SortedSet} methods in accepting additional arguments describing
	/// whether lower and upper bounds are inclusive versus exclusive.
	/// Subsets of any {@code NavigableSet} must implement the {@code
	/// NavigableSet} interface.
	/// 
	/// <para> The return values of navigation methods may be ambiguous in
	/// implementations that permit {@code null} elements. However, even
	/// in this case the result can be disambiguated by checking
	/// {@code contains(null)}. To avoid such issues, implementations of
	/// this interface are encouraged to <em>not</em> permit insertion of
	/// {@code null} elements. (Note that sorted sets of {@link
	/// Comparable} elements intrinsically do not permit {@code null}.)
	/// 
	/// </para>
	/// <para>Methods
	/// <seealso cref="#subSet(Object, Object) subSet(E, E)"/>,
	/// <seealso cref="#headSet(Object) headSet(E)"/>, and
	/// <seealso cref="#tailSet(Object) tailSet(E)"/>
	/// are specified to return {@code SortedSet} to allow existing
	/// implementations of {@code SortedSet} to be compatibly retrofitted to
	/// implement {@code NavigableSet}, but extensions and implementations
	/// of this interface are encouraged to override these methods to return
	/// {@code NavigableSet}.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Doug Lea
	/// @author Josh Bloch
	/// </para>
	/// </summary>
	/// @param <E> the type of elements maintained by this set
	/// @since 1.6 </param>
	public interface NavigableSet<E> : SortedSet<E>
	{
		/// <summary>
		/// Returns the greatest element in this set strictly less than the
		/// given element, or {@code null} if there is no such element.
		/// </summary>
		/// <param name="e"> the value to match </param>
		/// <returns> the greatest element less than {@code e},
		///         or {@code null} if there is no such element </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set does not permit null elements </exception>
		E Lower(E e);

		/// <summary>
		/// Returns the greatest element in this set less than or equal to
		/// the given element, or {@code null} if there is no such element.
		/// </summary>
		/// <param name="e"> the value to match </param>
		/// <returns> the greatest element less than or equal to {@code e},
		///         or {@code null} if there is no such element </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set does not permit null elements </exception>
		E Floor(E e);

		/// <summary>
		/// Returns the least element in this set greater than or equal to
		/// the given element, or {@code null} if there is no such element.
		/// </summary>
		/// <param name="e"> the value to match </param>
		/// <returns> the least element greater than or equal to {@code e},
		///         or {@code null} if there is no such element </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set does not permit null elements </exception>
		E Ceiling(E e);

		/// <summary>
		/// Returns the least element in this set strictly greater than the
		/// given element, or {@code null} if there is no such element.
		/// </summary>
		/// <param name="e"> the value to match </param>
		/// <returns> the least element greater than {@code e},
		///         or {@code null} if there is no such element </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set does not permit null elements </exception>
		E Higher(E e);

		/// <summary>
		/// Retrieves and removes the first (lowest) element,
		/// or returns {@code null} if this set is empty.
		/// </summary>
		/// <returns> the first element, or {@code null} if this set is empty </returns>
		E PollFirst();

		/// <summary>
		/// Retrieves and removes the last (highest) element,
		/// or returns {@code null} if this set is empty.
		/// </summary>
		/// <returns> the last element, or {@code null} if this set is empty </returns>
		E PollLast();

		/// <summary>
		/// Returns an iterator over the elements in this set, in ascending order.
		/// </summary>
		/// <returns> an iterator over the elements in this set, in ascending order </returns>
		Iterator<E> Iterator();

		/// <summary>
		/// Returns a reverse order view of the elements contained in this set.
		/// The descending set is backed by this set, so changes to the set are
		/// reflected in the descending set, and vice-versa.  If either set is
		/// modified while an iteration over either set is in progress (except
		/// through the iterator's own {@code remove} operation), the results of
		/// the iteration are undefined.
		/// 
		/// <para>The returned set has an ordering equivalent to
		/// <tt><seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>(comparator())</tt>.
		/// The expression {@code s.descendingSet().descendingSet()} returns a
		/// view of {@code s} essentially equivalent to {@code s}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a reverse order view of this set </returns>
		NavigableSet<E> DescendingSet();

		/// <summary>
		/// Returns an iterator over the elements in this set, in descending order.
		/// Equivalent in effect to {@code descendingSet().iterator()}.
		/// </summary>
		/// <returns> an iterator over the elements in this set, in descending order </returns>
		Iterator<E> DescendingIterator();

		/// <summary>
		/// Returns a view of the portion of this set whose elements range from
		/// {@code fromElement} to {@code toElement}.  If {@code fromElement} and
		/// {@code toElement} are equal, the returned set is empty unless {@code
		/// fromInclusive} and {@code toInclusive} are both true.  The returned set
		/// is backed by this set, so changes in the returned set are reflected in
		/// this set, and vice-versa.  The returned set supports all optional set
		/// operations that this set supports.
		/// 
		/// <para>The returned set will throw an {@code IllegalArgumentException}
		/// on an attempt to insert an element outside its range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromElement"> low endpoint of the returned set </param>
		/// <param name="fromInclusive"> {@code true} if the low endpoint
		///        is to be included in the returned view </param>
		/// <param name="toElement"> high endpoint of the returned set </param>
		/// <param name="toInclusive"> {@code true} if the high endpoint
		///        is to be included in the returned view </param>
		/// <returns> a view of the portion of this set whose elements range from
		///         {@code fromElement}, inclusive, to {@code toElement}, exclusive </returns>
		/// <exception cref="ClassCastException"> if {@code fromElement} and
		///         {@code toElement} cannot be compared to one another using this
		///         set's comparator (or, if the set has no comparator, using
		///         natural ordering).  Implementations may, but are not required
		///         to, throw this exception if {@code fromElement} or
		///         {@code toElement} cannot be compared to elements currently in
		///         the set. </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} or
		///         {@code toElement} is null and this set does
		///         not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if {@code fromElement} is
		///         greater than {@code toElement}; or if this set itself
		///         has a restricted range, and {@code fromElement} or
		///         {@code toElement} lies outside the bounds of the range. </exception>
		NavigableSet<E> SubSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive);

		/// <summary>
		/// Returns a view of the portion of this set whose elements are less than
		/// (or equal to, if {@code inclusive} is true) {@code toElement}.  The
		/// returned set is backed by this set, so changes in the returned set are
		/// reflected in this set, and vice-versa.  The returned set supports all
		/// optional set operations that this set supports.
		/// 
		/// <para>The returned set will throw an {@code IllegalArgumentException}
		/// on an attempt to insert an element outside its range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="toElement"> high endpoint of the returned set </param>
		/// <param name="inclusive"> {@code true} if the high endpoint
		///        is to be included in the returned view </param>
		/// <returns> a view of the portion of this set whose elements are less than
		///         (or equal to, if {@code inclusive} is true) {@code toElement} </returns>
		/// <exception cref="ClassCastException"> if {@code toElement} is not compatible
		///         with this set's comparator (or, if the set has no comparator,
		///         if {@code toElement} does not implement <seealso cref="Comparable"/>).
		///         Implementations may, but are not required to, throw this
		///         exception if {@code toElement} cannot be compared to elements
		///         currently in the set. </exception>
		/// <exception cref="NullPointerException"> if {@code toElement} is null and
		///         this set does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if this set itself has a
		///         restricted range, and {@code toElement} lies outside the
		///         bounds of the range </exception>
		NavigableSet<E> HeadSet(E toElement, bool inclusive);

		/// <summary>
		/// Returns a view of the portion of this set whose elements are greater
		/// than (or equal to, if {@code inclusive} is true) {@code fromElement}.
		/// The returned set is backed by this set, so changes in the returned set
		/// are reflected in this set, and vice-versa.  The returned set supports
		/// all optional set operations that this set supports.
		/// 
		/// <para>The returned set will throw an {@code IllegalArgumentException}
		/// on an attempt to insert an element outside its range.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromElement"> low endpoint of the returned set </param>
		/// <param name="inclusive"> {@code true} if the low endpoint
		///        is to be included in the returned view </param>
		/// <returns> a view of the portion of this set whose elements are greater
		///         than or equal to {@code fromElement} </returns>
		/// <exception cref="ClassCastException"> if {@code fromElement} is not compatible
		///         with this set's comparator (or, if the set has no comparator,
		///         if {@code fromElement} does not implement <seealso cref="Comparable"/>).
		///         Implementations may, but are not required to, throw this
		///         exception if {@code fromElement} cannot be compared to elements
		///         currently in the set. </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} is null
		///         and this set does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if this set itself has a
		///         restricted range, and {@code fromElement} lies outside the
		///         bounds of the range </exception>
		NavigableSet<E> TailSet(E fromElement, bool inclusive);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>Equivalent to {@code subSet(fromElement, true, toElement, false)}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		SortedSet<E> SubSet(E fromElement, E toElement);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>Equivalent to {@code headSet(toElement, false)}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		SortedSet<E> HeadSet(E toElement);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <para>Equivalent to {@code tailSet(fromElement, true)}.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		SortedSet<E> TailSet(E fromElement);
	}

}