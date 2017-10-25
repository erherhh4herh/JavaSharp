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
	/// The root interface in the <i>collection hierarchy</i>.  A collection
	/// represents a group of objects, known as its <i>elements</i>.  Some
	/// collections allow duplicate elements and others do not.  Some are ordered
	/// and others unordered.  The JDK does not provide any <i>direct</i>
	/// implementations of this interface: it provides implementations of more
	/// specific subinterfaces like <tt>Set</tt> and <tt>List</tt>.  This interface
	/// is typically used to pass collections around and manipulate them where
	/// maximum generality is desired.
	/// 
	/// <para><i>Bags</i> or <i>multisets</i> (unordered collections that may contain
	/// duplicate elements) should implement this interface directly.
	/// 
	/// </para>
	/// <para>All general-purpose <tt>Collection</tt> implementation classes (which
	/// typically implement <tt>Collection</tt> indirectly through one of its
	/// subinterfaces) should provide two "standard" constructors: a void (no
	/// arguments) constructor, which creates an empty collection, and a
	/// constructor with a single argument of type <tt>Collection</tt>, which
	/// creates a new collection with the same elements as its argument.  In
	/// effect, the latter constructor allows the user to copy any collection,
	/// producing an equivalent collection of the desired implementation type.
	/// There is no way to enforce this convention (as interfaces cannot contain
	/// constructors) but all of the general-purpose <tt>Collection</tt>
	/// implementations in the Java platform libraries comply.
	/// 
	/// </para>
	/// <para>The "destructive" methods contained in this interface, that is, the
	/// methods that modify the collection on which they operate, are specified to
	/// throw <tt>UnsupportedOperationException</tt> if this collection does not
	/// support the operation.  If this is the case, these methods may, but are not
	/// required to, throw an <tt>UnsupportedOperationException</tt> if the
	/// invocation would have no effect on the collection.  For example, invoking
	/// the <seealso cref="#addAll(Collection)"/> method on an unmodifiable collection may,
	/// but is not required to, throw the exception if the collection to be added
	/// is empty.
	/// 
	/// </para>
	/// <para><a name="optional-restrictions">
	/// Some collection implementations have restrictions on the elements that
	/// they may contain.</a>  For example, some implementations prohibit null elements,
	/// and some have restrictions on the types of their elements.  Attempting to
	/// add an ineligible element throws an unchecked exception, typically
	/// <tt>NullPointerException</tt> or <tt>ClassCastException</tt>.  Attempting
	/// to query the presence of an ineligible element may throw an exception,
	/// or it may simply return false; some implementations will exhibit the former
	/// behavior and some will exhibit the latter.  More generally, attempting an
	/// operation on an ineligible element whose completion would not result in
	/// the insertion of an ineligible element into the collection may throw an
	/// exception or it may succeed, at the option of the implementation.
	/// Such exceptions are marked as "optional" in the specification for this
	/// interface.
	/// 
	/// </para>
	/// <para>It is up to each collection to determine its own synchronization
	/// policy.  In the absence of a stronger guarantee by the
	/// implementation, undefined behavior may result from the invocation
	/// of any method on a collection that is being mutated by another
	/// thread; this includes direct invocations, passing the collection to
	/// a method that might perform invocations, and using an existing
	/// iterator to examine the collection.
	/// 
	/// </para>
	/// <para>Many methods in Collections Framework interfaces are defined in
	/// terms of the <seealso cref="Object#equals(Object) equals"/> method.  For example,
	/// the specification for the <seealso cref="#contains(Object) contains(Object o)"/>
	/// method says: "returns <tt>true</tt> if and only if this collection
	/// contains at least one element <tt>e</tt> such that
	/// <tt>(o==null ? e==null : o.equals(e))</tt>."  This specification should
	/// <i>not</i> be construed to imply that invoking <tt>Collection.contains</tt>
	/// with a non-null argument <tt>o</tt> will cause <tt>o.equals(e)</tt> to be
	/// invoked for any element <tt>e</tt>.  Implementations are free to implement
	/// optimizations whereby the <tt>equals</tt> invocation is avoided, for
	/// example, by first comparing the hash codes of the two elements.  (The
	/// <seealso cref="Object#hashCode()"/> specification guarantees that two objects with
	/// unequal hash codes cannot be equal.)  More generally, implementations of
	/// the various Collections Framework interfaces are free to take advantage of
	/// the specified behavior of underlying <seealso cref="Object"/> methods wherever the
	/// implementor deems it appropriate.
	/// 
	/// </para>
	/// <para>Some collection operations which perform recursive traversal of the
	/// collection may fail with an exception for self-referential instances where
	/// the collection directly or indirectly contains itself. This includes the
	/// {@code clone()}, {@code equals()}, {@code hashCode()} and {@code toString()}
	/// methods. Implementations may optionally handle the self-referential scenario,
	/// however most current implementations do not do so.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @implSpec
	/// The default method implementations (inherited or otherwise) do not apply any
	/// synchronization protocol.  If a {@code Collection} implementation has a
	/// specific synchronization protocol, then it must override default
	/// implementations to apply that protocol.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements in this collection
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref=     Set </seealso>
	/// <seealso cref=     List </seealso>
	/// <seealso cref=     Map </seealso>
	/// <seealso cref=     SortedSet </seealso>
	/// <seealso cref=     SortedMap </seealso>
	/// <seealso cref=     HashSet </seealso>
	/// <seealso cref=     TreeSet </seealso>
	/// <seealso cref=     ArrayList </seealso>
	/// <seealso cref=     LinkedList </seealso>
	/// <seealso cref=     Vector </seealso>
	/// <seealso cref=     Collections </seealso>
	/// <seealso cref=     Arrays </seealso>
	/// <seealso cref=     AbstractCollection
	/// @since 1.2 </seealso>

	public interface Collection<E> : Iterable<E>
	{
		// Query Operations

		/// <summary>
		/// Returns the number of elements in this collection.  If this collection
		/// contains more than <tt>Integer.MAX_VALUE</tt> elements, returns
		/// <tt>Integer.MAX_VALUE</tt>.
		/// </summary>
		/// <returns> the number of elements in this collection </returns>
		int Size();

		/// <summary>
		/// Returns <tt>true</tt> if this collection contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this collection contains no elements </returns>
		bool Empty {get;}

		/// <summary>
		/// Returns <tt>true</tt> if this collection contains the specified element.
		/// More formally, returns <tt>true</tt> if and only if this collection
		/// contains at least one element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this collection is to be tested </param>
		/// <returns> <tt>true</tt> if this collection contains the specified
		///         element </returns>
		/// <exception cref="ClassCastException"> if the type of the specified element
		///         is incompatible with this collection
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         collection does not permit null elements
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		bool Contains(Object o);

		/// <summary>
		/// Returns an iterator over the elements in this collection.  There are no
		/// guarantees concerning the order in which the elements are returned
		/// (unless this collection is an instance of some class that provides a
		/// guarantee).
		/// </summary>
		/// <returns> an <tt>Iterator</tt> over the elements in this collection </returns>
		Iterator<E> Iterator();

		/// <summary>
		/// Returns an array containing all of the elements in this collection.
		/// If this collection makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the elements in
		/// the same order.
		/// 
		/// <para>The returned array will be "safe" in that no references to it are
		/// maintained by this collection.  (In other words, this method must
		/// allocate a new array even if this collection is backed by an array).
		/// The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all of the elements in this collection </returns>
		Object[] ToArray();

		/// <summary>
		/// Returns an array containing all of the elements in this collection;
		/// the runtime type of the returned array is that of the specified array.
		/// If the collection fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of the
		/// specified array and the size of this collection.
		/// 
		/// <para>If this collection fits in the specified array with room to spare
		/// (i.e., the array has more elements than this collection), the element
		/// in the array immediately following the end of the collection is set to
		/// <tt>null</tt>.  (This is useful in determining the length of this
		/// collection <i>only</i> if the caller knows that this collection does
		/// not contain any <tt>null</tt> elements.)
		/// 
		/// </para>
		/// <para>If this collection makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the elements in
		/// the same order.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose <tt>x</tt> is a collection known to contain only strings.
		/// The following code can be used to dump the collection into a newly
		/// allocated array of <tt>String</tt>:
		/// 
		/// <pre>
		///     String[] y = x.toArray(new String[0]);</pre>
		/// 
		/// Note that <tt>toArray(new Object[0])</tt> is identical in function to
		/// <tt>toArray()</tt>.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the runtime type of the array to contain the collection </param>
		/// <param name="a"> the array into which the elements of this collection are to be
		///        stored, if it is big enough; otherwise, a new array of the same
		///        runtime type is allocated for this purpose. </param>
		/// <returns> an array containing all of the elements in this collection </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in
		///         this collection </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		T[] toArray<T>(T[] a);

		// Modification Operations

		/// <summary>
		/// Ensures that this collection contains the specified element (optional
		/// operation).  Returns <tt>true</tt> if this collection changed as a
		/// result of the call.  (Returns <tt>false</tt> if this collection does
		/// not permit duplicates and already contains the specified element.)<para>
		/// 
		/// Collections that support this operation may place limitations on what
		/// elements may be added to this collection.  In particular, some
		/// collections will refuse to add <tt>null</tt> elements, and others will
		/// impose restrictions on the type of elements that may be added.
		/// Collection classes should clearly specify in their documentation any
		/// </para>
		/// restrictions on what elements may be added.<para>
		/// 
		/// If a collection refuses to add a particular element for any reason
		/// other than that it already contains the element, it <i>must</i> throw
		/// an exception (rather than returning <tt>false</tt>).  This preserves
		/// the invariant that a collection always contains the specified element
		/// after this call returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> element whose presence in this collection is to be ensured </param>
		/// <returns> <tt>true</tt> if this collection changed as a result of the
		///         call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>add</tt> operation
		///         is not supported by this collection </exception>
		/// <exception cref="ClassCastException"> if the class of the specified element
		///         prevents it from being added to this collection </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         collection does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the element
		///         prevents it from being added to this collection </exception>
		/// <exception cref="IllegalStateException"> if the element cannot be added at this
		///         time due to insertion restrictions </exception>
		bool Add(E e);

		/// <summary>
		/// Removes a single instance of the specified element from this
		/// collection, if it is present (optional operation).  More formally,
		/// removes an element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>, if
		/// this collection contains one or more such elements.  Returns
		/// <tt>true</tt> if this collection contained the specified element (or
		/// equivalently, if this collection changed as a result of the call).
		/// </summary>
		/// <param name="o"> element to be removed from this collection, if present </param>
		/// <returns> <tt>true</tt> if an element was removed as a result of this call </returns>
		/// <exception cref="ClassCastException"> if the type of the specified element
		///         is incompatible with this collection
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified element is null and this
		///         collection does not permit null elements
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
		///         is not supported by this collection </exception>
		bool Remove(Object o);


		// Bulk Operations

		/// <summary>
		/// Returns <tt>true</tt> if this collection contains all of the elements
		/// in the specified collection.
		/// </summary>
		/// <param name="c"> collection to be checked for containment in this collection </param>
		/// <returns> <tt>true</tt> if this collection contains all of the elements
		///         in the specified collection </returns>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in the specified collection are incompatible with this
		///         collection
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified collection contains one
		///         or more null elements and this collection does not permit null
		///         elements
		///         (<a href="#optional-restrictions">optional</a>),
		///         or if the specified collection is null. </exception>
		/// <seealso cref=    #contains(Object) </seealso>
		bool containsAll<T1>(Collection<T1> c);

		/// <summary>
		/// Adds all of the elements in the specified collection to this collection
		/// (optional operation).  The behavior of this operation is undefined if
		/// the specified collection is modified while the operation is in progress.
		/// (This implies that the behavior of this call is undefined if the
		/// specified collection is this collection, and this collection is
		/// nonempty.)
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this collection </param>
		/// <returns> <tt>true</tt> if this collection changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>addAll</tt> operation
		///         is not supported by this collection </exception>
		/// <exception cref="ClassCastException"> if the class of an element of the specified
		///         collection prevents it from being added to this collection </exception>
		/// <exception cref="NullPointerException"> if the specified collection contains a
		///         null element and this collection does not permit null elements,
		///         or if the specified collection is null </exception>
		/// <exception cref="IllegalArgumentException"> if some property of an element of the
		///         specified collection prevents it from being added to this
		///         collection </exception>
		/// <exception cref="IllegalStateException"> if not all the elements can be added at
		///         this time due to insertion restrictions </exception>
		/// <seealso cref= #add(Object) </seealso>
		bool addAll<T1>(Collection<T1> c) where T1 : E;

		/// <summary>
		/// Removes all of this collection's elements that are also contained in the
		/// specified collection (optional operation).  After this call returns,
		/// this collection will contain no elements in common with the specified
		/// collection.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this collection </param>
		/// <returns> <tt>true</tt> if this collection changed as a result of the
		///         call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>removeAll</tt> method
		///         is not supported by this collection </exception>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in this collection are incompatible with the specified
		///         collection
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this collection contains one or more
		///         null elements and the specified collection does not support
		///         null elements
		///         (<a href="#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		bool removeAll<T1>(Collection<T1> c);

		/// <summary>
		/// Removes all of the elements of this collection that satisfy the given
		/// predicate.  Errors or runtime exceptions thrown during iteration or by
		/// the predicate are relayed to the caller.
		/// 
		/// @implSpec
		/// The default implementation traverses all elements of the collection using
		/// its <seealso cref="#iterator"/>.  Each matching element is removed using
		/// <seealso cref="Iterator#remove()"/>.  If the collection's iterator does not
		/// support removal then an {@code UnsupportedOperationException} will be
		/// thrown on the first matching element.
		/// </summary>
		/// <param name="filter"> a predicate which returns {@code true} for elements to be
		///        removed </param>
		/// <returns> {@code true} if any elements were removed </returns>
		/// <exception cref="NullPointerException"> if the specified filter is null </exception>
		/// <exception cref="UnsupportedOperationException"> if elements cannot be removed
		///         from this collection.  Implementations may throw this exception if a
		///         matching element cannot be removed or if, in general, removal is not
		///         supported.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default boolean removeIf(java.util.function.Predicate<? base E> filter)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean removeIf(java.util.function.Predicate<JavaToDotNetGenericWildcard> filter)
	//	{
	//		Objects.requireNonNull(filter);
	//		while (each.hasNext())
	//		{
	//			if (filter.test(each.next()))
	//			{
	//				each.remove();
	//				removed = true;
	//			}
	//		}
	//	}

		/// <summary>
		/// Retains only the elements in this collection that are contained in the
		/// specified collection (optional operation).  In other words, removes from
		/// this collection all of its elements that are not contained in the
		/// specified collection.
		/// </summary>
		/// <param name="c"> collection containing elements to be retained in this collection </param>
		/// <returns> <tt>true</tt> if this collection changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>retainAll</tt> operation
		///         is not supported by this collection </exception>
		/// <exception cref="ClassCastException"> if the types of one or more elements
		///         in this collection are incompatible with the specified
		///         collection
		///         (<a href="#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this collection contains one or more
		///         null elements and the specified collection does not permit null
		///         elements
		///         (<a href="#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		bool retainAll<T1>(Collection<T1> c);

		/// <summary>
		/// Removes all of the elements from this collection (optional operation).
		/// The collection will be empty after this method returns.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the <tt>clear</tt> operation
		///         is not supported by this collection </exception>
		void Clear();


		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this collection for equality. <para>
		/// 
		/// While the <tt>Collection</tt> interface adds no stipulations to the
		/// general contract for the <tt>Object.equals</tt>, programmers who
		/// implement the <tt>Collection</tt> interface "directly" (in other words,
		/// create a class that is a <tt>Collection</tt> but is not a <tt>Set</tt>
		/// or a <tt>List</tt>) must exercise care if they choose to override the
		/// <tt>Object.equals</tt>.  It is not necessary to do so, and the simplest
		/// course of action is to rely on <tt>Object</tt>'s implementation, but
		/// the implementor may wish to implement a "value comparison" in place of
		/// the default "reference comparison."  (The <tt>List</tt> and
		/// </para>
		/// <tt>Set</tt> interfaces mandate such value comparisons.)<para>
		/// 
		/// The general contract for the <tt>Object.equals</tt> method states that
		/// equals must be symmetric (in other words, <tt>a.equals(b)</tt> if and
		/// only if <tt>b.equals(a)</tt>).  The contracts for <tt>List.equals</tt>
		/// and <tt>Set.equals</tt> state that lists are only equal to other lists,
		/// and sets to other sets.  Thus, a custom <tt>equals</tt> method for a
		/// collection class that implements neither the <tt>List</tt> nor
		/// <tt>Set</tt> interface must return <tt>false</tt> when this collection
		/// is compared to any list or set.  (By the same logic, it is not possible
		/// to write a class that correctly implements both the <tt>Set</tt> and
		/// <tt>List</tt> interfaces.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> object to be compared for equality with this collection </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this
		/// collection
		/// </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= Set#equals(Object) </seealso>
		/// <seealso cref= List#equals(Object) </seealso>
		bool Equals(Object o);

		/// <summary>
		/// Returns the hash code value for this collection.  While the
		/// <tt>Collection</tt> interface adds no stipulations to the general
		/// contract for the <tt>Object.hashCode</tt> method, programmers should
		/// take note that any class that overrides the <tt>Object.equals</tt>
		/// method must also override the <tt>Object.hashCode</tt> method in order
		/// to satisfy the general contract for the <tt>Object.hashCode</tt> method.
		/// In particular, <tt>c1.equals(c2)</tt> implies that
		/// <tt>c1.hashCode()==c2.hashCode()</tt>.
		/// </summary>
		/// <returns> the hash code value for this collection
		/// </returns>
		/// <seealso cref= Object#hashCode() </seealso>
		/// <seealso cref= Object#equals(Object) </seealso>
		int HashCode();

		/// <summary>
		/// Creates a <seealso cref="Spliterator"/> over the elements in this collection.
		/// 
		/// Implementations should document characteristic values reported by the
		/// spliterator.  Such characteristic values are not required to be reported
		/// if the spliterator reports <seealso cref="Spliterator#SIZED"/> and this collection
		/// contains no elements.
		/// 
		/// <para>The default implementation should be overridden by subclasses that
		/// can return a more efficient spliterator.  In order to
		/// preserve expected laziness behavior for the <seealso cref="#stream()"/> and
		/// <seealso cref="#parallelStream()"/>} methods, spliterators should either have the
		/// characteristic of {@code IMMUTABLE} or {@code CONCURRENT}, or be
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>.
		/// If none of these is practical, the overriding class should describe the
		/// spliterator's documented policy of binding and structural interference,
		/// and should override the <seealso cref="#stream()"/> and <seealso cref="#parallelStream()"/>
		/// methods to create streams using a {@code Supplier} of the spliterator,
		/// as in:
		/// <pre>{@code
		///     Stream<E> s = StreamSupport.stream(() -> spliterator(), spliteratorCharacteristics)
		/// }</pre>
		/// </para>
		/// <para>These requirements ensure that streams produced by the
		/// <seealso cref="#stream()"/> and <seealso cref="#parallelStream()"/> methods will reflect the
		/// contents of the collection as of initiation of the terminal stream
		/// operation.
		/// 
		/// @implSpec
		/// The default implementation creates a
		/// <em><a href="Spliterator.html#binding">late-binding</a></em> spliterator
		/// from the collections's {@code Iterator}.  The spliterator inherits the
		/// <em>fail-fast</em> properties of the collection's iterator.
		/// </para>
		/// <para>
		/// The created {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>.
		/// 
		/// @implNote
		/// The created {@code Spliterator} additionally reports
		/// <seealso cref="Spliterator#SUBSIZED"/>.
		/// 
		/// </para>
		/// <para>If a spliterator covers no elements then the reporting of additional
		/// characteristic values, beyond that of {@code SIZED} and {@code SUBSIZED},
		/// does not aid clients to control, specialize or simplify computation.
		/// However, this does enable shared use of an immutable and empty
		/// spliterator instance (see <seealso cref="Spliterators#emptySpliterator()"/>) for
		/// empty collections, and enables clients to determine if such a spliterator
		/// covers no elements.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this collection
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Spliterator<E> spliterator()
	//	{
	//		return Spliterators.spliterator(this, 0);
	//	}

		/// <summary>
		/// Returns a sequential {@code Stream} with this collection as its source.
		/// 
		/// <para>This method should be overridden when the <seealso cref="#spliterator()"/>
		/// method cannot return a spliterator that is {@code IMMUTABLE},
		/// {@code CONCURRENT}, or <em>late-binding</em>. (See <seealso cref="#spliterator()"/>
		/// for details.)
		/// 
		/// @implSpec
		/// The default implementation creates a sequential {@code Stream} from the
		/// collection's {@code Spliterator}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a sequential {@code Stream} over the elements in this collection
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.util.stream.Stream<E> stream()
	//	{
	//		return StreamSupport.stream(spliterator(), false);
	//	}

		/// <summary>
		/// Returns a possibly parallel {@code Stream} with this collection as its
		/// source.  It is allowable for this method to return a sequential stream.
		/// 
		/// <para>This method should be overridden when the <seealso cref="#spliterator()"/>
		/// method cannot return a spliterator that is {@code IMMUTABLE},
		/// {@code CONCURRENT}, or <em>late-binding</em>. (See <seealso cref="#spliterator()"/>
		/// for details.)
		/// 
		/// @implSpec
		/// The default implementation creates a parallel {@code Stream} from the
		/// collection's {@code Spliterator}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a possibly parallel {@code Stream} over the elements in this
		/// collection
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default java.util.stream.Stream<E> parallelStream()
	//	{
	//		return StreamSupport.stream(spliterator(), true);
	//	}
	}

	public static class Collection_Fields
	{
			public const bool Removed = false;
			public static readonly Iterator<E> Each = iterator();
			public static readonly return Removed;
	}

}