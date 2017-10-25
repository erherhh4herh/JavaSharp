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
namespace java.util
{


	/// <summary>
	/// An object for traversing and partitioning elements of a source.  The source
	/// of elements covered by a Spliterator could be, for example, an array, a
	/// <seealso cref="Collection"/>, an IO channel, or a generator function.
	/// 
	/// <para>A Spliterator may traverse elements individually ({@link
	/// #tryAdvance tryAdvance()}) or sequentially in bulk
	/// (<seealso cref="#forEachRemaining forEachRemaining()"/>).
	/// 
	/// </para>
	/// <para>A Spliterator may also partition off some of its elements (using
	/// <seealso cref="#trySplit"/>) as another Spliterator, to be used in
	/// possibly-parallel operations.  Operations using a Spliterator that
	/// cannot split, or does so in a highly imbalanced or inefficient
	/// manner, are unlikely to benefit from parallelism.  Traversal
	/// and splitting exhaust elements; each Spliterator is useful for only a single
	/// bulk computation.
	/// 
	/// </para>
	/// <para>A Spliterator also reports a set of <seealso cref="#characteristics()"/> of its
	/// structure, source, and elements from among <seealso cref="#ORDERED"/>,
	/// <seealso cref="#DISTINCT"/>, <seealso cref="#SORTED"/>, <seealso cref="#SIZED"/>, <seealso cref="#NONNULL"/>,
	/// <seealso cref="#IMMUTABLE"/>, <seealso cref="#CONCURRENT"/>, and <seealso cref="#SUBSIZED"/>. These may
	/// be employed by Spliterator clients to control, specialize or simplify
	/// computation.  For example, a Spliterator for a <seealso cref="Collection"/> would
	/// report {@code SIZED}, a Spliterator for a <seealso cref="Set"/> would report
	/// {@code DISTINCT}, and a Spliterator for a <seealso cref="SortedSet"/> would also
	/// report {@code SORTED}.  Characteristics are reported as a simple unioned bit
	/// set.
	/// 
	/// Some characteristics additionally constrain method behavior; for example if
	/// {@code ORDERED}, traversal methods must conform to their documented ordering.
	/// New characteristics may be defined in the future, so implementors should not
	/// assign meanings to unlisted values.
	/// 
	/// </para>
	/// <para><a name="binding">A Spliterator that does not report {@code IMMUTABLE} or
	/// {@code CONCURRENT} is expected to have a documented policy concerning:
	/// when the spliterator <em>binds</em> to the element source; and detection of
	/// structural interference of the element source detected after binding.</a>  A
	/// <em>late-binding</em> Spliterator binds to the source of elements at the
	/// point of first traversal, first split, or first query for estimated size,
	/// rather than at the time the Spliterator is created.  A Spliterator that is
	/// not <em>late-binding</em> binds to the source of elements at the point of
	/// construction or first invocation of any method.  Modifications made to the
	/// source prior to binding are reflected when the Spliterator is traversed.
	/// After binding a Spliterator should, on a best-effort basis, throw
	/// <seealso cref="ConcurrentModificationException"/> if structural interference is
	/// detected.  Spliterators that do this are called <em>fail-fast</em>.  The
	/// bulk traversal method (<seealso cref="#forEachRemaining forEachRemaining()"/>) of a
	/// Spliterator may optimize traversal and check for structural interference
	/// after all elements have been traversed, rather than checking per-element and
	/// failing immediately.
	/// 
	/// </para>
	/// <para>Spliterators can provide an estimate of the number of remaining elements
	/// via the <seealso cref="#estimateSize"/> method.  Ideally, as reflected in characteristic
	/// <seealso cref="#SIZED"/>, this value corresponds exactly to the number of elements
	/// that would be encountered in a successful traversal.  However, even when not
	/// exactly known, an estimated value value may still be useful to operations
	/// being performed on the source, such as helping to determine whether it is
	/// preferable to split further or traverse the remaining elements sequentially.
	/// 
	/// </para>
	/// <para>Despite their obvious utility in parallel algorithms, spliterators are not
	/// expected to be thread-safe; instead, implementations of parallel algorithms
	/// using spliterators should ensure that the spliterator is only used by one
	/// thread at a time.  This is generally easy to attain via <em>serial
	/// thread-confinement</em>, which often is a natural consequence of typical
	/// parallel algorithms that work by recursive decomposition.  A thread calling
	/// <seealso cref="#trySplit()"/> may hand over the returned Spliterator to another thread,
	/// which in turn may traverse or further split that Spliterator.  The behaviour
	/// of splitting and traversal is undefined if two or more threads operate
	/// concurrently on the same spliterator.  If the original thread hands a
	/// spliterator off to another thread for processing, it is best if that handoff
	/// occurs before any elements are consumed with {@link #tryAdvance(Consumer)
	/// tryAdvance()}, as certain guarantees (such as the accuracy of
	/// <seealso cref="#estimateSize()"/> for {@code SIZED} spliterators) are only valid before
	/// traversal has begun.
	/// 
	/// </para>
	/// <para>Primitive subtype specializations of {@code Spliterator} are provided for
	/// <seealso cref="OfInt int"/>, <seealso cref="OfLong long"/>, and <seealso cref="OfDouble double"/> values.
	/// The subtype default implementations of
	/// <seealso cref="Spliterator#tryAdvance(java.util.function.Consumer)"/>
	/// and <seealso cref="Spliterator#forEachRemaining(java.util.function.Consumer)"/> box
	/// primitive values to instances of their corresponding wrapper class.  Such
	/// boxing may undermine any performance advantages gained by using the primitive
	/// specializations.  To avoid boxing, the corresponding primitive-based methods
	/// should be used.  For example,
	/// <seealso cref="Spliterator.OfInt#tryAdvance(java.util.function.IntConsumer)"/>
	/// and <seealso cref="Spliterator.OfInt#forEachRemaining(java.util.function.IntConsumer)"/>
	/// should be used in preference to
	/// <seealso cref="Spliterator.OfInt#tryAdvance(java.util.function.Consumer)"/> and
	/// <seealso cref="Spliterator.OfInt#forEachRemaining(java.util.function.Consumer)"/>.
	/// Traversal of primitive values using boxing-based methods
	/// <seealso cref="#tryAdvance tryAdvance()"/> and
	/// <seealso cref="#forEachRemaining(java.util.function.Consumer) forEachRemaining()"/>
	/// does not affect the order in which the values, transformed to boxed values,
	/// are encountered.
	/// 
	/// @apiNote
	/// </para>
	/// <para>Spliterators, like {@code Iterators}s, are for traversing the elements of
	/// a source.  The {@code Spliterator} API was designed to support efficient
	/// parallel traversal in addition to sequential traversal, by supporting
	/// decomposition as well as single-element iteration.  In addition, the
	/// protocol for accessing elements via a Spliterator is designed to impose
	/// smaller per-element overhead than {@code Iterator}, and to avoid the inherent
	/// race involved in having separate methods for {@code hasNext()} and
	/// {@code next()}.
	/// 
	/// </para>
	/// <para>For mutable sources, arbitrary and non-deterministic behavior may occur if
	/// the source is structurally interfered with (elements added, replaced, or
	/// removed) between the time that the Spliterator binds to its data source and
	/// the end of traversal.  For example, such interference will produce arbitrary,
	/// non-deterministic results when using the {@code java.util.stream} framework.
	/// 
	/// </para>
	/// <para>Structural interference of a source can be managed in the following ways
	/// (in approximate order of decreasing desirability):
	/// <ul>
	/// <li>The source cannot be structurally interfered with.
	/// <br>For example, an instance of
	/// <seealso cref="java.util.concurrent.CopyOnWriteArrayList"/> is an immutable source.
	/// A Spliterator created from the source reports a characteristic of
	/// {@code IMMUTABLE}.</li>
	/// <li>The source manages concurrent modifications.
	/// <br>For example, a key set of a <seealso cref="java.util.concurrent.ConcurrentHashMap"/>
	/// is a concurrent source.  A Spliterator created from the source reports a
	/// characteristic of {@code CONCURRENT}.</li>
	/// <li>The mutable source provides a late-binding and fail-fast Spliterator.
	/// <br>Late binding narrows the window during which interference can affect
	/// the calculation; fail-fast detects, on a best-effort basis, that structural
	/// interference has occurred after traversal has commenced and throws
	/// <seealso cref="ConcurrentModificationException"/>.  For example, <seealso cref="ArrayList"/>,
	/// and many other non-concurrent {@code Collection} classes in the JDK, provide
	/// a late-binding, fail-fast spliterator.</li>
	/// <li>The mutable source provides a non-late-binding but fail-fast Spliterator.
	/// <br>The source increases the likelihood of throwing
	/// {@code ConcurrentModificationException} since the window of potential
	/// interference is larger.</li>
	/// <li>The mutable source provides a late-binding and non-fail-fast Spliterator.
	/// <br>The source risks arbitrary, non-deterministic behavior after traversal
	/// has commenced since interference is not detected.
	/// </li>
	/// <li>The mutable source provides a non-late-binding and non-fail-fast
	/// Spliterator.
	/// <br>The source increases the risk of arbitrary, non-deterministic behavior
	/// since non-detected interference may occur after construction.
	/// </li>
	/// </ul>
	/// 
	/// </para>
	/// <para><b>Example.</b> Here is a class (not a very useful one, except
	/// for illustration) that maintains an array in which the actual data
	/// are held in even locations, and unrelated tag data are held in odd
	/// locations. Its Spliterator ignores the tags.
	/// 
	/// <pre> {@code
	/// class TaggedArray<T> {
	///   private final Object[] elements; // immutable after construction
	///   TaggedArray(T[] data, Object[] tags) {
	///     int size = data.length;
	///     if (tags.length != size) throw new IllegalArgumentException();
	///     this.elements = new Object[2 * size];
	///     for (int i = 0, j = 0; i < size; ++i) {
	///       elements[j++] = data[i];
	///       elements[j++] = tags[i];
	///     }
	///   }
	/// 
	///   public Spliterator<T> spliterator() {
	///     return new TaggedArraySpliterator<>(elements, 0, elements.length);
	///   }
	/// 
	///   static class TaggedArraySpliterator<T> implements Spliterator<T> {
	///     private final Object[] array;
	///     private int origin; // current index, advanced on split or traversal
	///     private final int fence; // one past the greatest index
	/// 
	///     TaggedArraySpliterator(Object[] array, int origin, int fence) {
	///       this.array = array; this.origin = origin; this.fence = fence;
	///     }
	/// 
	///     public void forEachRemaining(Consumer<? super T> action) {
	///       for (; origin < fence; origin += 2)
	///         action.accept((T) array[origin]);
	///     }
	/// 
	///     public boolean tryAdvance(Consumer<? super T> action) {
	///       if (origin < fence) {
	///         action.accept((T) array[origin]);
	///         origin += 2;
	///         return true;
	///       }
	///       else // cannot advance
	///         return false;
	///     }
	/// 
	///     public Spliterator<T> trySplit() {
	///       int lo = origin; // divide range in half
	///       int mid = ((lo + fence) >>> 1) & ~1; // force midpoint to be even
	///       if (lo < mid) { // split out left half
	///         origin = mid; // reset this Spliterator's origin
	///         return new TaggedArraySpliterator<>(array, lo, mid);
	///       }
	///       else       // too small to split
	///         return null;
	///     }
	/// 
	///     public long estimateSize() {
	///       return (long)((fence - origin) / 2);
	///     }
	/// 
	///     public int characteristics() {
	///       return ORDERED | SIZED | IMMUTABLE | SUBSIZED;
	///     }
	///   }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>As an example how a parallel computation framework, such as the
	/// {@code java.util.stream} package, would use Spliterator in a parallel
	/// computation, here is one way to implement an associated parallel forEach,
	/// that illustrates the primary usage idiom of splitting off subtasks until
	/// the estimated amount of work is small enough to perform
	/// sequentially. Here we assume that the order of processing across
	/// subtasks doesn't matter; different (forked) tasks may further split
	/// and process elements concurrently in undetermined order.  This
	/// example uses a <seealso cref="java.util.concurrent.CountedCompleter"/>;
	/// similar usages apply to other parallel task constructions.
	/// 
	/// <pre>{@code
	/// static <T> void parEach(TaggedArray<T> a, Consumer<T> action) {
	///   Spliterator<T> s = a.spliterator();
	///   long targetBatchSize = s.estimateSize() / (ForkJoinPool.getCommonPoolParallelism() * 8);
	///   new ParEach(null, s, action, targetBatchSize).invoke();
	/// }
	/// 
	/// static class ParEach<T> extends CountedCompleter<Void> {
	///   final Spliterator<T> spliterator;
	///   final Consumer<T> action;
	///   final long targetBatchSize;
	/// 
	///   ParEach(ParEach<T> parent, Spliterator<T> spliterator,
	///           Consumer<T> action, long targetBatchSize) {
	///     super(parent);
	///     this.spliterator = spliterator; this.action = action;
	///     this.targetBatchSize = targetBatchSize;
	///   }
	/// 
	///   public void compute() {
	///     Spliterator<T> sub;
	///     while (spliterator.estimateSize() > targetBatchSize &&
	///            (sub = spliterator.trySplit()) != null) {
	///       addToPendingCount(1);
	///       new ParEach<>(this, sub, action, targetBatchSize).fork();
	///     }
	///     spliterator.forEachRemaining(action);
	///     propagateCompletion();
	///   }
	/// }}</pre>
	/// 
	/// @implNote
	/// If the boolean system property {@code org.openjdk.java.util.stream.tripwire}
	/// is set to {@code true} then diagnostic warnings are reported if boxing of
	/// primitive values occur when operating on primitive subtype specializations.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of elements returned by this Spliterator
	/// </param>
	/// <seealso cref= Collection
	/// @since 1.8 </seealso>
	public interface Spliterator<T>
	{
		/// <summary>
		/// If a remaining element exists, performs the given action on it,
		/// returning {@code true}; else returns {@code false}.  If this
		/// Spliterator is <seealso cref="#ORDERED"/> the action is performed on the
		/// next element in encounter order.  Exceptions thrown by the
		/// action are relayed to the caller.
		/// </summary>
		/// <param name="action"> The action </param>
		/// <returns> {@code false} if no remaining elements existed
		/// upon entry to this method, else {@code true}. </returns>
		/// <exception cref="NullPointerException"> if the specified action is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean tryAdvance(java.util.function.Consumer<? base T> action);
		bool tryAdvance<T1>(Consumer<T1> action);

		/// <summary>
		/// Performs the given action for each remaining element, sequentially in
		/// the current thread, until all elements have been processed or the action
		/// throws an exception.  If this Spliterator is <seealso cref="#ORDERED"/>, actions
		/// are performed in encounter order.  Exceptions thrown by the action
		/// are relayed to the caller.
		/// 
		/// @implSpec
		/// The default implementation repeatedly invokes <seealso cref="#tryAdvance"/> until
		/// it returns {@code false}.  It should be overridden whenever possible.
		/// </summary>
		/// <param name="action"> The action </param>
		/// <exception cref="NullPointerException"> if the specified action is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default void forEachRemaining(java.util.function.Consumer<? base T> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		do
	//		{
	//		} while (tryAdvance(action));
	//	}

		/// <summary>
		/// If this spliterator can be partitioned, returns a Spliterator
		/// covering elements, that will, upon return from this method, not
		/// be covered by this Spliterator.
		/// 
		/// <para>If this Spliterator is <seealso cref="#ORDERED"/>, the returned Spliterator
		/// must cover a strict prefix of the elements.
		/// 
		/// </para>
		/// <para>Unless this Spliterator covers an infinite number of elements,
		/// repeated calls to {@code trySplit()} must eventually return {@code null}.
		/// Upon non-null return:
		/// <ul>
		/// <li>the value reported for {@code estimateSize()} before splitting,
		/// must, after splitting, be greater than or equal to {@code estimateSize()}
		/// for this and the returned Spliterator; and</li>
		/// <li>if this Spliterator is {@code SUBSIZED}, then {@code estimateSize()}
		/// for this spliterator before splitting must be equal to the sum of
		/// {@code estimateSize()} for this and the returned Spliterator after
		/// splitting.</li>
		/// </ul>
		/// 
		/// </para>
		/// <para>This method may return {@code null} for any reason,
		/// including emptiness, inability to split after traversal has
		/// commenced, data structure constraints, and efficiency
		/// considerations.
		/// 
		/// @apiNote
		/// An ideal {@code trySplit} method efficiently (without
		/// traversal) divides its elements exactly in half, allowing
		/// balanced parallel computation.  Many departures from this ideal
		/// remain highly effective; for example, only approximately
		/// splitting an approximately balanced tree, or for a tree in
		/// which leaf nodes may contain either one or two elements,
		/// failing to further split these nodes.  However, large
		/// deviations in balance and/or overly inefficient {@code
		/// trySplit} mechanics typically result in poor parallel
		/// performance.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} covering some portion of the
		/// elements, or {@code null} if this spliterator cannot be split </returns>
		Spliterator<T> TrySplit();

		/// <summary>
		/// Returns an estimate of the number of elements that would be
		/// encountered by a <seealso cref="#forEachRemaining"/> traversal, or returns {@link
		/// Long#MAX_VALUE} if infinite, unknown, or too expensive to compute.
		/// 
		/// <para>If this Spliterator is <seealso cref="#SIZED"/> and has not yet been partially
		/// traversed or split, or this Spliterator is <seealso cref="#SUBSIZED"/> and has
		/// not yet been partially traversed, this estimate must be an accurate
		/// count of elements that would be encountered by a complete traversal.
		/// Otherwise, this estimate may be arbitrarily inaccurate, but must decrease
		/// as specified across invocations of <seealso cref="#trySplit"/>.
		/// 
		/// @apiNote
		/// Even an inexact estimate is often useful and inexpensive to compute.
		/// For example, a sub-spliterator of an approximately balanced binary tree
		/// may return a value that estimates the number of elements to be half of
		/// that of its parent; if the root Spliterator does not maintain an
		/// accurate count, it could estimate size to be the power of two
		/// corresponding to its maximum depth.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the estimated size, or {@code Long.MAX_VALUE} if infinite,
		///         unknown, or too expensive to compute. </returns>
		long EstimateSize();

		/// <summary>
		/// Convenience method that returns <seealso cref="#estimateSize()"/> if this
		/// Spliterator is <seealso cref="#SIZED"/>, else {@code -1}.
		/// @implSpec
		/// The default implementation returns the result of {@code estimateSize()}
		/// if the Spliterator reports a characteristic of {@code SIZED}, and
		/// {@code -1} otherwise.
		/// </summary>
		/// <returns> the exact size, if known, else {@code -1}. </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default long getExactSizeIfKnown()
	//	{
	//		return (characteristics() & SIZED) == 0 ? -1L : estimateSize();
	//	}

		/// <summary>
		/// Returns a set of characteristics of this Spliterator and its
		/// elements. The result is represented as ORed values from {@link
		/// #ORDERED}, <seealso cref="#DISTINCT"/>, <seealso cref="#SORTED"/>, <seealso cref="#SIZED"/>,
		/// <seealso cref="#NONNULL"/>, <seealso cref="#IMMUTABLE"/>, <seealso cref="#CONCURRENT"/>,
		/// <seealso cref="#SUBSIZED"/>.  Repeated calls to {@code characteristics()} on
		/// a given spliterator, prior to or in-between calls to {@code trySplit},
		/// should always return the same result.
		/// 
		/// <para>If a Spliterator reports an inconsistent set of
		/// characteristics (either those returned from a single invocation
		/// or across multiple invocations), no guarantees can be made
		/// about any computation using this Spliterator.
		/// 
		/// @apiNote The characteristics of a given spliterator before splitting
		/// may differ from the characteristics after splitting.  For specific
		/// examples see the characteristic values <seealso cref="#SIZED"/>, <seealso cref="#SUBSIZED"/>
		/// and <seealso cref="#CONCURRENT"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a representation of characteristics </returns>
		int Characteristics();

		/// <summary>
		/// Returns {@code true} if this Spliterator's {@link
		/// #characteristics} contain all of the given characteristics.
		/// 
		/// @implSpec
		/// The default implementation returns true if the corresponding bits
		/// of the given characteristics are set.
		/// </summary>
		/// <param name="characteristics"> the characteristics to check for </param>
		/// <returns> {@code true} if all the specified characteristics are present,
		/// else {@code false} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean hasCharacteristics(int characteristics)
	//	{
	//		return (characteristics() & characteristics) == characteristics;
	//	}

		/// <summary>
		/// If this Spliterator's source is <seealso cref="#SORTED"/> by a <seealso cref="Comparator"/>,
		/// returns that {@code Comparator}. If the source is {@code SORTED} in
		/// <seealso cref="Comparable natural order"/>, returns {@code null}.  Otherwise,
		/// if the source is not {@code SORTED}, throws <seealso cref="IllegalStateException"/>.
		/// 
		/// @implSpec
		/// The default implementation always throws <seealso cref="IllegalStateException"/>.
		/// </summary>
		/// <returns> a Comparator, or {@code null} if the elements are sorted in the
		/// natural order. </returns>
		/// <exception cref="IllegalStateException"> if the spliterator does not report
		///         a characteristic of {@code SORTED}. </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<JavaToDotNetGenericWildcard> getComparator()
	//	{
	//		throw new IllegalStateException();
	//	}

		/// <summary>
		/// Characteristic value signifying that an encounter order is defined for
		/// elements. If so, this Spliterator guarantees that method
		/// <seealso cref="#trySplit"/> splits a strict prefix of elements, that method
		/// <seealso cref="#tryAdvance"/> steps by one element in prefix order, and that
		/// <seealso cref="#forEachRemaining"/> performs actions in encounter order.
		/// 
		/// <para>A <seealso cref="Collection"/> has an encounter order if the corresponding
		/// <seealso cref="Collection#iterator"/> documents an order. If so, the encounter
		/// order is the same as the documented order. Otherwise, a collection does
		/// not have an encounter order.
		/// 
		/// @apiNote Encounter order is guaranteed to be ascending index order for
		/// any <seealso cref="List"/>. But no order is guaranteed for hash-based collections
		/// such as <seealso cref="HashSet"/>. Clients of a Spliterator that reports
		/// {@code ORDERED} are expected to preserve ordering constraints in
		/// non-commutative parallel computations.
		/// </para>
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that, for each pair of
		/// encountered elements {@code x, y}, {@code !x.equals(y)}. This
		/// applies for example, to a Spliterator based on a <seealso cref="Set"/>.
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that encounter order follows a defined
		/// sort order. If so, method <seealso cref="#getComparator()"/> returns the associated
		/// Comparator, or {@code null} if all elements are <seealso cref="Comparable"/> and
		/// are sorted by their natural ordering.
		/// 
		/// <para>A Spliterator that reports {@code SORTED} must also report
		/// {@code ORDERED}.
		/// 
		/// @apiNote The spliterators for {@code Collection} classes in the JDK that
		/// implement <seealso cref="NavigableSet"/> or <seealso cref="SortedSet"/> report {@code SORTED}.
		/// </para>
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that the value returned from
		/// {@code estimateSize()} prior to traversal or splitting represents a
		/// finite size that, in the absence of structural source modification,
		/// represents an exact count of the number of elements that would be
		/// encountered by a complete traversal.
		/// 
		/// @apiNote Most Spliterators for Collections, that cover all elements of a
		/// {@code Collection} report this characteristic. Sub-spliterators, such as
		/// those for <seealso cref="HashSet"/>, that cover a sub-set of elements and
		/// approximate their reported size do not.
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that the source guarantees that
		/// encountered elements will not be {@code null}. (This applies,
		/// for example, to most concurrent collections, queues, and maps.)
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that the element source cannot be
		/// structurally modified; that is, elements cannot be added, replaced, or
		/// removed, so such changes cannot occur during traversal. A Spliterator
		/// that does not report {@code IMMUTABLE} or {@code CONCURRENT} is expected
		/// to have a documented policy (for example throwing
		/// <seealso cref="ConcurrentModificationException"/>) concerning structural
		/// interference detected during traversal.
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that the element source may be safely
		/// concurrently modified (allowing additions, replacements, and/or removals)
		/// by multiple threads without external synchronization. If so, the
		/// Spliterator is expected to have a documented policy concerning the impact
		/// of modifications during traversal.
		/// 
		/// <para>A top-level Spliterator should not report both {@code CONCURRENT} and
		/// {@code SIZED}, since the finite size, if known, may change if the source
		/// is concurrently modified during traversal. Such a Spliterator is
		/// inconsistent and no guarantees can be made about any computation using
		/// that Spliterator. Sub-spliterators may report {@code SIZED} if the
		/// sub-split size is known and additions or removals to the source are not
		/// reflected when traversing.
		/// 
		/// @apiNote Most concurrent collections maintain a consistency policy
		/// guaranteeing accuracy with respect to elements present at the point of
		/// Spliterator construction, but possibly not reflecting subsequent
		/// additions or removals.
		/// </para>
		/// </summary>

		/// <summary>
		/// Characteristic value signifying that all Spliterators resulting from
		/// {@code trySplit()} will be both <seealso cref="#SIZED"/> and <seealso cref="#SUBSIZED"/>.
		/// (This means that all child Spliterators, whether direct or indirect, will
		/// be {@code SIZED}.)
		/// 
		/// <para>A Spliterator that does not report {@code SIZED} as required by
		/// {@code SUBSIZED} is inconsistent and no guarantees can be made about any
		/// computation using that Spliterator.
		/// 
		/// @apiNote Some spliterators, such as the top-level spliterator for an
		/// approximately balanced binary tree, will report {@code SIZED} but not
		/// {@code SUBSIZED}, since it is common to know the size of the entire tree
		/// but not the exact sizes of subtrees.
		/// </para>
		/// </summary>

		/// <summary>
		/// A Spliterator specialized for primitive values.
		/// </summary>
		/// @param <T> the type of elements returned by this Spliterator.  The
		/// type must be a wrapper type for a primitive type, such as {@code Integer}
		/// for the primitive {@code int} type. </param>
		/// @param <T_CONS> the type of primitive consumer.  The type must be a
		/// primitive specialization of <seealso cref="java.util.function.Consumer"/> for
		/// {@code T}, such as <seealso cref="java.util.function.IntConsumer"/> for
		/// {@code Integer}. </param>
		/// @param <T_SPLITR> the type of primitive Spliterator.  The type must be
		/// a primitive specialization of Spliterator for {@code T}, such as
		/// <seealso cref="Spliterator.OfInt"/> for {@code Integer}.
		/// </param>
		/// <seealso cref= Spliterator.OfInt </seealso>
		/// <seealso cref= Spliterator.OfLong </seealso>
		/// <seealso cref= Spliterator.OfDouble
		/// @since 1.8 </seealso>

		/// <summary>
		/// A Spliterator specialized for {@code int} values.
		/// @since 1.8
		/// </summary>

		/// <summary>
		/// A Spliterator specialized for {@code long} values.
		/// @since 1.8
		/// </summary>

		/// <summary>
		/// A Spliterator specialized for {@code double} values.
		/// @since 1.8
		/// </summary>
	}

	public static class Spliterator_Fields
	{
		public const int ORDERED = 0x00000010;
		public const int DISTINCT = 0x00000001;
		public const int SORTED = 0x00000004;
		public const int SIZED = 0x00000040;
		public const int NONNULL = 0x00000100;
		public const int IMMUTABLE = 0x00000400;
		public const int CONCURRENT = 0x00001000;
		public const int SUBSIZED = 0x00004000;
	}

	public interface Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> : Spliterator<T> where T_SPLITR : Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>
	{
		T_SPLITR TrySplit();

		/// <summary>
		/// If a remaining element exists, performs the given action on it,
		/// returning {@code true}; else returns {@code false}.  If this
		/// Spliterator is <seealso cref="#ORDERED"/> the action is performed on the
		/// next element in encounter order.  Exceptions thrown by the
		/// action are relayed to the caller.
		/// </summary>
		/// <param name="action"> The action </param>
		/// <returns> {@code false} if no remaining elements existed
		/// upon entry to this method, else {@code true}. </returns>
		/// <exception cref="NullPointerException"> if the specified action is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("overloads") boolean tryAdvance(T_CONS action);
		bool TryAdvance(T_CONS action);

		/// <summary>
		/// Performs the given action for each remaining element, sequentially in
		/// the current thread, until all elements have been processed or the
		/// action throws an exception.  If this Spliterator is <seealso cref="#ORDERED"/>,
		/// actions are performed in encounter order.  Exceptions thrown by the
		/// action are relayed to the caller.
		/// 
		/// @implSpec
		/// The default implementation repeatedly invokes <seealso cref="#tryAdvance"/>
		/// until it returns {@code false}.  It should be overridden whenever
		/// possible.
		/// </summary>
		/// <param name="action"> The action </param>
		/// <exception cref="NullPointerException"> if the specified action is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("overloads") default void forEachRemaining(T_CONS action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(T_CONS action)
	//	{
	//		do
	//		{
	//		} while (tryAdvance(action));
	//	}
	}

	public interface Spliterator_OfInt : Spliterator_OfPrimitive<Integer, IntConsumer, Spliterator_OfInt>
	{

		Spliterator_OfInt TrySplit();

		bool TryAdvance(IntConsumer action);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.IntConsumer action)
	//	{
	//		do
	//		{
	//		} while (tryAdvance(action));
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code IntConsumer} then it is cast
		/// to {@code IntConsumer} and passed to
		/// <seealso cref="#tryAdvance(java.util.function.IntConsumer)"/>; otherwise
		/// the action is adapted to an instance of {@code IntConsumer}, by
		/// boxing the argument of {@code IntConsumer}, and then passed to
		/// <seealso cref="#tryAdvance(java.util.function.IntConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default boolean tryAdvance(java.util.function.Consumer<? base Integer> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean tryAdvance(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof IntConsumer)
	//		{
	//			return tryAdvance((IntConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfInt.tryAdvance((IntConsumer) action::accept)");
	//			return tryAdvance((IntConsumer) action::accept);
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code IntConsumer} then it is cast
		/// to {@code IntConsumer} and passed to
		/// <seealso cref="#forEachRemaining(java.util.function.IntConsumer)"/>; otherwise
		/// the action is adapted to an instance of {@code IntConsumer}, by
		/// boxing the argument of {@code IntConsumer}, and then passed to
		/// <seealso cref="#forEachRemaining(java.util.function.IntConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEachRemaining(java.util.function.Consumer<? base Integer> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof IntConsumer)
	//		{
	//			forEachRemaining((IntConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfInt.forEachRemaining((IntConsumer) action::accept)");
	//			forEachRemaining((IntConsumer) action::accept);
	//		}
	//	}
	}

	public interface Spliterator_OfLong : Spliterator_OfPrimitive<Long, LongConsumer, Spliterator_OfLong>
	{

		Spliterator_OfLong TrySplit();

		bool TryAdvance(LongConsumer action);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.LongConsumer action)
	//	{
	//		do
	//		{
	//		} while (tryAdvance(action));
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code LongConsumer} then it is cast
		/// to {@code LongConsumer} and passed to
		/// <seealso cref="#tryAdvance(java.util.function.LongConsumer)"/>; otherwise
		/// the action is adapted to an instance of {@code LongConsumer}, by
		/// boxing the argument of {@code LongConsumer}, and then passed to
		/// <seealso cref="#tryAdvance(java.util.function.LongConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default boolean tryAdvance(java.util.function.Consumer<? base Long> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean tryAdvance(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof LongConsumer)
	//		{
	//			return tryAdvance((LongConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfLong.tryAdvance((LongConsumer) action::accept)");
	//			return tryAdvance((LongConsumer) action::accept);
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code LongConsumer} then it is cast
		/// to {@code LongConsumer} and passed to
		/// <seealso cref="#forEachRemaining(java.util.function.LongConsumer)"/>; otherwise
		/// the action is adapted to an instance of {@code LongConsumer}, by
		/// boxing the argument of {@code LongConsumer}, and then passed to
		/// <seealso cref="#forEachRemaining(java.util.function.LongConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEachRemaining(java.util.function.Consumer<? base Long> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof LongConsumer)
	//		{
	//			forEachRemaining((LongConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfLong.forEachRemaining((LongConsumer) action::accept)");
	//			forEachRemaining((LongConsumer) action::accept);
	//		}
	//	}
	}

	public interface Spliterator_OfDouble : Spliterator_OfPrimitive<Double, DoubleConsumer, Spliterator_OfDouble>
	{

		Spliterator_OfDouble TrySplit();

		bool TryAdvance(DoubleConsumer action);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.DoubleConsumer action)
	//	{
	//		do
	//		{
	//		} while (tryAdvance(action));
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code DoubleConsumer} then it is
		/// cast to {@code DoubleConsumer} and passed to
		/// <seealso cref="#tryAdvance(java.util.function.DoubleConsumer)"/>; otherwise
		/// the action is adapted to an instance of {@code DoubleConsumer}, by
		/// boxing the argument of {@code DoubleConsumer}, and then passed to
		/// <seealso cref="#tryAdvance(java.util.function.DoubleConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default boolean tryAdvance(java.util.function.Consumer<? base Double> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean tryAdvance(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof DoubleConsumer)
	//		{
	//			return tryAdvance((DoubleConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfDouble.tryAdvance((DoubleConsumer) action::accept)");
	//			return tryAdvance((DoubleConsumer) action::accept);
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec
		/// If the action is an instance of {@code DoubleConsumer} then it is
		/// cast to {@code DoubleConsumer} and passed to
		/// <seealso cref="#forEachRemaining(java.util.function.DoubleConsumer)"/>;
		/// otherwise the action is adapted to an instance of
		/// {@code DoubleConsumer}, by boxing the argument of
		/// {@code DoubleConsumer}, and then passed to
		/// <seealso cref="#forEachRemaining(java.util.function.DoubleConsumer)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEachRemaining(java.util.function.Consumer<? base Double> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		if (action instanceof DoubleConsumer)
	//		{
	//			forEachRemaining((DoubleConsumer) action);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Spliterator.OfDouble.forEachRemaining((DoubleConsumer) action::accept)");
	//			forEachRemaining((DoubleConsumer) action::accept);
	//		}
	//	}
	}

}