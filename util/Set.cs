/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A collection that contains no duplicate elements.  More formally, sets
	/// contain no pair of elements <code>e1</code> and <code>e2</code> such that
	/// <code>e1.equals(e2)</code>, and at most one null element.  As implied by
	/// its name, this interface models the mathematical <i>set</i> abstraction.
	/// 
	/// <para>The <tt>Set</tt> interface places additional stipulations, beyond those
	/// inherited from the <tt>Collection</tt> interface, on the contracts of all
	/// constructors and on the contracts of the <tt>add</tt>, <tt>equals</tt> and
	/// <tt>hashCode</tt> methods.  Declarations for other inherited methods are
	/// also included here for convenience.  (The specifications accompanying these
	/// declarations have been tailored to the <tt>Set</tt> interface, but they do
	/// not contain any additional stipulations.)
	/// 
	/// </para>
	/// <para>The additional stipulation on constructors is, not surprisingly,
	/// that all constructors must create a set that contains no duplicate elements
	/// (as defined above).
	/// 
	/// </para>
	/// <para>Note: Great care must be exercised if mutable objects are used as set
	/// elements.  The behavior of a set is not specified if the value of an object
	/// is changed in a manner that affects <tt>equals</tt> comparisons while the
	/// object is an element in the set.  A special case of this prohibition is
	/// that it is not permissible for a set to contain itself as an element.
	/// 
	/// </para>
	/// <para>Some set implementations have restrictions on the elements that
	/// they may contain.  For example, some implementations prohibit null elements,
	/// and some have restrictions on the types of their elements.  Attempting to
	/// add an ineligible element throws an unchecked exception, typically
	/// <tt>NullPointerException</tt> or <tt>ClassCastException</tt>.  Attempting
	/// to query the presence of an ineligible element may throw an exception,
	/// or it may simply return false; some implementations will exhibit the former
	/// behavior and some will exhibit the latter.  More generally, attempting an
	/// operation on an ineligible element whose completion would not result in
	/// the insertion of an ineligible element into the set may throw an
	/// exception or it may succeed, at the option of the implementation.
	/// Such exceptions are marked as "optional" in the specification for this
	/// interface.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements maintained by this set
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= List </seealso>
	/// <seealso cref= SortedSet </seealso>
	/// <seealso cref= HashSet </seealso>
	/// <seealso cref= TreeSet </seealso>
	/// <seealso cref= AbstractSet </seealso>
	/// <seealso cref= Collections#singleton(java.lang.Object) </seealso>
	/// <seealso cref= Collections#EMPTY_SET
	/// @since 1.2 </seealso>

	public interface Set<E> : Collection<E>
	{
		// Query Operations

		/// <summary>
		/// Returns the number of elements in this set (its cardinality).  If this
		/// set contains more than <tt>Integer.MAX_VALUE</tt> elements, returns
		/// <tt>Integer.MAX_VALUE</tt>.
		/// </summary>
		/// <returns> the number of elements in this set (its cardinality) </returns>
		int Size();

		/// <summary>
		/// Returns <tt>true</tt> if this set contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this set contains no elements </returns>
		bool Empty {get;}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains the specified element.
		/// More formally, returns <tt>true</tt> if and only if this set
		/// contains an element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this set is to be tested </param>
		/// <returns> <tt>true</tt> if this set contains the specified element </returns>
		/// <exception cref="ClassCastException"> if the type of the specified element
		///         is incompatible with this set
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         set does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		bool Contains(Object o);

		/// <summary>
		/// Returns an iterator over the elements in this set.  The elements are
		/// returned in no particular order (unless this set is an instance of some
		/// class that provides a guarantee).
		/// </summary>
		/// <returns> an iterator over the elements in this set </returns>
		Iterator<E> Iterator();

		/// <summary>
		/// Returns an array containing all of the elements in this set.
		/// If this set makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the
		/// elements in the same order.
		/// 
		/// <para>The returned array will be "safe" in that no references to it
		/// are maintained by this set.  (In other words, this method must
		/// allocate a new array even if this set is backed by an array).
		/// The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all the elements in this set </returns>
		Object[] ToArray();

		/// <summary>
		/// Returns an array containing all of the elements in this set; the
		/// runtime type of the returned array is that of the specified array.
		/// If the set fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of the
		/// specified array and the size of this set.
		/// 
		/// <para>If this set fits in the specified array with room to spare
		/// (i.e., the array has more elements than this set), the element in
		/// the array immediately following the end of the set is set to
		/// <tt>null</tt>.  (This is useful in determining the length of this
		/// set <i>only</i> if the caller knows that this set does not contain
		/// any null elements.)
		/// 
		/// </para>
		/// <para>If this set makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the elements
		/// in the same order.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose <tt>x</tt> is a set known to contain only strings.
		/// The following code can be used to dump the set into a newly allocated
		/// array of <tt>String</tt>:
		/// 
		/// <pre>
		///     String[] y = x.toArray(new String[0]);</pre>
		/// 
		/// Note that <tt>toArray(new Object[0])</tt> is identical in function to
		/// <tt>toArray()</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of this set are to be
		///        stored, if it is big enough; otherwise, a new array of the same
		///        runtime type is allocated for this purpose. </param>
		/// <returns> an array containing all the elements in this set </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in this
		///         set </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		T[] toArray<T>(T[] a);


		// Modification Operations

		/// <summary>
		/// Adds the specified element to this set if it is not already present
		/// (optional operation).  More formally, adds the specified element
		/// <tt>e</tt> to this set if the set contains no element <tt>e2</tt>
		/// such that
		/// <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		/// If this set already contains the element, the call leaves the set
		/// unchanged and returns <tt>false</tt>.  In combination with the
		/// restriction on constructors, this ensures that sets never contain
		/// duplicate elements.
		/// 
		/// <para>The stipulation above does not imply that sets must accept all
		/// elements; sets may refuse to add any particular element, including
		/// <tt>null</tt>, and throw an exception, as described in the
		/// specification for <seealso cref="Collection#add Collection.add"/>.
		/// Individual set implementations should clearly document any
		/// restrictions on the elements that they may contain.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> <tt>true</tt> if this set did not already contain the specified
		///         element </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>add</tt> operation
		///         is not supported by this set </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         set does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified element
		///         prevents it from being added to this set </exception>
		bool Add(E e);


		/// <summary>
		/// Removes the specified element from this set if it is present
		/// (optional operation).  More formally, removes an element <tt>e</tt>
		/// such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>, if
		/// this set contains such an element.  Returns <tt>true</tt> if this set
		/// contained the element (or equivalently, if this set changed as a
		/// result of the call).  (This set will not contain the element once the
		/// call returns.)
		/// </summary>
		/// <param name="o"> object to be removed from this set, if present </param>
		/// <returns> <tt>true</tt> if this set contained the specified element </returns>
		/// <exception cref="ClassCastException"> if the type of the specified element
		///         is incompatible with this set
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         set does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
		///         is not supported by this set </exception>
		bool Remove(Object o);


		// Bulk Operations

		/// <summary>
		/// Returns <tt>true</tt> if this set contains all of the elements of the
		/// specified collection.  If the specified collection is also a set, this
		/// method returns <tt>true</tt> if it is a <i>subset</i> of this set.
		/// </summary>
		/// <param name="c"> collection to be checked for containment in this set </param>
		/// <returns> <tt>true</tt> if this set contains all of the elements of the
		///         specified collection </returns>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in the specified collection are incompatible with this
		///         set
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified collection contains one
		///         or more null elements and this set does not permit null
		///         elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref=    #contains(Object) </seealso>
		bool containsAll<T1>(Collection<T1> c);

		/// <summary>
		/// Adds all of the elements in the specified collection to this set if
		/// they're not already present (optional operation).  If the specified
		/// collection is also a set, the <tt>addAll</tt> operation effectively
		/// modifies this set so that its value is the <i>union</i> of the two
		/// sets.  The behavior of this operation is undefined if the specified
		/// collection is modified while the operation is in progress.
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>addAll</tt> operation
		///         is not supported by this set </exception>
		/// <exception cref="ClassCastException"> if the class of an element of the
		///         specified collection prevents it from being added to this set </exception>
		/// <exception cref="NullPointerException"> if the specified collection contains one
		///         or more null elements and this set does not permit null
		///         elements, or if the specified collection is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of an element of the
		///         specified collection prevents it from being added to this set </exception>
		/// <seealso cref= #add(Object) </seealso>
		bool addAll<T1>(Collection<T1> c) where T1 : E;

		/// <summary>
		/// Retains only the elements in this set that are contained in the
		/// specified collection (optional operation).  In other words, removes
		/// from this set all of its elements that are not contained in the
		/// specified collection.  If the specified collection is also a set, this
		/// operation effectively modifies this set so that its value is the
		/// <i>intersection</i> of the two sets.
		/// </summary>
		/// <param name="c"> collection containing elements to be retained in this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>retainAll</tt> operation
		///         is not supported by this set </exception>
		/// <exception cref="ClassCastException"> if the class of an element of this set
		///         is incompatible with the specified collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this set contains a null element and the
		///         specified collection does not permit null elements
		///         (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		bool retainAll<T1>(Collection<T1> c);

		/// <summary>
		/// Removes from this set all of its elements that are contained in the
		/// specified collection (optional operation).  If the specified
		/// collection is also a set, this operation effectively modifies this
		/// set so that its value is the <i>asymmetric set difference</i> of
		/// the two sets.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>removeAll</tt> operation
		///         is not supported by this set </exception>
		/// <exception cref="ClassCastException"> if the class of an element of this set
		///         is incompatible with the specified collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this set contains a null element and the
		///         specified collection does not permit null elements
		///         (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		bool removeAll<T1>(Collection<T1> c);

		/// <summary>
		/// Removes all of the elements from this set (optional operation).
		/// The set will be empty after this call returns.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the <tt>clear</tt> method
		///         is not supported by this set </exception>
		void Clear();


		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this set for equality.  Returns
		/// <tt>true</tt> if the specified object is also a set, the two sets
		/// have the same size, and every member of the specified set is
		/// contained in this set (or equivalently, every member of this set is
		/// contained in the specified set).  This definition ensures that the
		/// equals method works properly across different implementations of the
		/// set interface.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this set </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this set </returns>
		bool Equals(Object o);

		/// <summary>
		/// Returns the hash code value for this set.  The hash code of a set is
		/// defined to be the sum of the hash codes of the elements in the set,
		/// where the hash code of a <tt>null</tt> element is defined to be zero.
		/// This ensures that <tt>s1.equals(s2)</tt> implies that
		/// <tt>s1.hashCode()==s2.hashCode()</tt> for any two sets <tt>s1</tt>
		/// and <tt>s2</tt>, as required by the general contract of
		/// <seealso cref="Object#hashCode"/>.
		/// </summary>
		/// <returns> the hash code value for this set </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= Set#equals(Object) </seealso>
		int HashCode();

		/// <summary>
		/// Creates a {@code Spliterator} over the elements in this set.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#DISTINCT"/>.
		/// Implementations should document the reporting of additional
		/// characteristic values.
		/// 
		/// @implSpec
		/// The default implementation creates a
		/// <em><a href="Spliterator.html#binding">late-binding</a></em> spliterator
		/// from the set's {@code Iterator}.  The spliterator inherits the
		/// <em>fail-fast</em> properties of the set's iterator.
		/// </para>
		/// <para>
		/// The created {@code Spliterator} additionally reports
		/// <seealso cref="Spliterator#SIZED"/>.
		/// 
		/// @implNote
		/// The created {@code Spliterator} additionally reports
		/// <seealso cref="Spliterator#SUBSIZED"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this set
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Spliterator<E> spliterator()
	//	{
	//		return Spliterators.spliterator(this, Spliterator.DISTINCT);
	//	}
	}

}