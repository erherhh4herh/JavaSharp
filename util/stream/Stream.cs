using System.Collections.Generic;

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
namespace java.util.stream
{


	/// <summary>
	/// A sequence of elements supporting sequential and parallel aggregate
	/// operations.  The following example illustrates an aggregate operation using
	/// <seealso cref="Stream"/> and <seealso cref="IntStream"/>:
	/// 
	/// <pre>{@code
	///     int sum = widgets.stream()
	///                      .filter(w -> w.getColor() == RED)
	///                      .mapToInt(w -> w.getWeight())
	///                      .sum();
	/// }</pre>
	/// 
	/// In this example, {@code widgets} is a {@code Collection<Widget>}.  We create
	/// a stream of {@code Widget} objects via <seealso cref="Collection#stream Collection.stream()"/>,
	/// filter it to produce a stream containing only the red widgets, and then
	/// transform it into a stream of {@code int} values representing the weight of
	/// each red widget. Then this stream is summed to produce a total weight.
	/// 
	/// <para>In addition to {@code Stream}, which is a stream of object references,
	/// there are primitive specializations for <seealso cref="IntStream"/>, <seealso cref="LongStream"/>,
	/// and <seealso cref="DoubleStream"/>, all of which are referred to as "streams" and
	/// conform to the characteristics and restrictions described here.
	/// 
	/// </para>
	/// <para>To perform a computation, stream
	/// <a href="package-summary.html#StreamOps">operations</a> are composed into a
	/// <em>stream pipeline</em>.  A stream pipeline consists of a source (which
	/// might be an array, a collection, a generator function, an I/O channel,
	/// etc), zero or more <em>intermediate operations</em> (which transform a
	/// stream into another stream, such as <seealso cref="Stream#filter(Predicate)"/>), and a
	/// <em>terminal operation</em> (which produces a result or side-effect, such
	/// as <seealso cref="Stream#count()"/> or <seealso cref="Stream#forEach(Consumer)"/>).
	/// Streams are lazy; computation on the source data is only performed when the
	/// terminal operation is initiated, and source elements are consumed only
	/// as needed.
	/// 
	/// </para>
	/// <para>Collections and streams, while bearing some superficial similarities,
	/// have different goals.  Collections are primarily concerned with the efficient
	/// management of, and access to, their elements.  By contrast, streams do not
	/// provide a means to directly access or manipulate their elements, and are
	/// instead concerned with declaratively describing their source and the
	/// computational operations which will be performed in aggregate on that source.
	/// However, if the provided stream operations do not offer the desired
	/// functionality, the <seealso cref="#iterator()"/> and <seealso cref="#spliterator()"/> operations
	/// can be used to perform a controlled traversal.
	/// 
	/// </para>
	/// <para>A stream pipeline, like the "widgets" example above, can be viewed as
	/// a <em>query</em> on the stream source.  Unless the source was explicitly
	/// designed for concurrent modification (such as a <seealso cref="ConcurrentHashMap"/>),
	/// unpredictable or erroneous behavior may result from modifying the stream
	/// source while it is being queried.
	/// 
	/// </para>
	/// <para>Most stream operations accept parameters that describe user-specified
	/// behavior, such as the lambda expression {@code w -> w.getWeight()} passed to
	/// {@code mapToInt} in the example above.  To preserve correct behavior,
	/// these <em>behavioral parameters</em>:
	/// <ul>
	/// <li>must be <a href="package-summary.html#NonInterference">non-interfering</a>
	/// (they do not modify the stream source); and</li>
	/// <li>in most cases must be <a href="package-summary.html#Statelessness">stateless</a>
	/// (their result should not depend on any state that might change during execution
	/// of the stream pipeline).</li>
	/// </ul>
	/// 
	/// </para>
	/// <para>Such parameters are always instances of a
	/// <a href="../function/package-summary.html">functional interface</a> such
	/// as <seealso cref="java.util.function.Function"/>, and are often lambda expressions or
	/// method references.  Unless otherwise specified these parameters must be
	/// <em>non-null</em>.
	/// 
	/// </para>
	/// <para>A stream should be operated on (invoking an intermediate or terminal stream
	/// operation) only once.  This rules out, for example, "forked" streams, where
	/// the same source feeds two or more pipelines, or multiple traversals of the
	/// same stream.  A stream implementation may throw <seealso cref="IllegalStateException"/>
	/// if it detects that the stream is being reused. However, since some stream
	/// operations may return their receiver rather than a new stream object, it may
	/// not be possible to detect reuse in all cases.
	/// 
	/// </para>
	/// <para>Streams have a <seealso cref="#close()"/> method and implement <seealso cref="AutoCloseable"/>,
	/// but nearly all stream instances do not actually need to be closed after use.
	/// Generally, only streams whose source is an IO channel (such as those returned
	/// by <seealso cref="Files#lines(Path, Charset)"/>) will require closing.  Most streams
	/// are backed by collections, arrays, or generating functions, which require no
	/// special resource management.  (If a stream does require closing, it can be
	/// declared as a resource in a {@code try}-with-resources statement.)
	/// 
	/// </para>
	/// <para>Stream pipelines may execute either sequentially or in
	/// <a href="package-summary.html#Parallelism">parallel</a>.  This
	/// execution mode is a property of the stream.  Streams are created
	/// with an initial choice of sequential or parallel execution.  (For example,
	/// <seealso cref="Collection#stream() Collection.stream()"/> creates a sequential stream,
	/// and <seealso cref="Collection#parallelStream() Collection.parallelStream()"/> creates
	/// a parallel one.)  This choice of execution mode may be modified by the
	/// <seealso cref="#sequential()"/> or <seealso cref="#parallel()"/> methods, and may be queried with
	/// the <seealso cref="#isParallel()"/> method.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the stream elements
	/// @since 1.8 </param>
	/// <seealso cref= IntStream </seealso>
	/// <seealso cref= LongStream </seealso>
	/// <seealso cref= DoubleStream </seealso>
	/// <seealso cref= <a href="package-summary.html">java.util.stream</a> </seealso>
	public interface Stream<T> : BaseStream<T, Stream<T>>
	{

		/// <summary>
		/// Returns a stream consisting of the elements of this stream that match
		/// the given predicate.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                  <a href="package-summary.html#Statelessness">stateless</a>
		///                  predicate to apply to each element to determine if it
		///                  should be included </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Stream<T> filter(java.util.function.Predicate<? base T> predicate);
		Stream<T> Filter(Predicate<T1> predicate);

		/// <summary>
		/// Returns a stream consisting of the results of applying the given
		/// function to the elements of this stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// @param <R> The element type of the new stream </param>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <R> Stream<R> map(java.util.function.Function<? base T, ? extends R> mapper);
		Stream<R> map<R, T1>(Function<T1> mapper) where T1 : R;

		/// <summary>
		/// Returns an {@code IntStream} consisting of the results of applying the
		/// given function to the elements of this stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">
		///     intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: IntStream mapToInt(java.util.function.ToIntFunction<? base T> mapper);
		IntStream mapToInt<T1>(ToIntFunction<T1> mapper);

		/// <summary>
		/// Returns a {@code LongStream} consisting of the results of applying the
		/// given function to the elements of this stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: LongStream mapToLong(java.util.function.ToLongFunction<? base T> mapper);
		LongStream mapToLong<T1>(ToLongFunction<T1> mapper);

		/// <summary>
		/// Returns a {@code DoubleStream} consisting of the results of applying the
		/// given function to the elements of this stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: DoubleStream mapToDouble(java.util.function.ToDoubleFunction<? base T> mapper);
		DoubleStream mapToDouble<T1>(ToDoubleFunction<T1> mapper);

		/// <summary>
		/// Returns a stream consisting of the results of replacing each element of
		/// this stream with the contents of a mapped stream produced by applying
		/// the provided mapping function to each element.  Each mapped stream is
		/// <seealso cref="java.util.stream.BaseStream#close() closed"/> after its contents
		/// have been placed into this stream.  (If a mapped stream is {@code null}
		/// an empty stream is used, instead.)
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// @apiNote
		/// The {@code flatMap()} operation has the effect of applying a one-to-many
		/// transformation to the elements of the stream, and then flattening the
		/// resulting elements into a new stream.
		/// 
		/// </para>
		/// <para><b>Examples.</b>
		/// 
		/// </para>
		/// <para>If {@code orders} is a stream of purchase orders, and each purchase
		/// order contains a collection of line items, then the following produces a
		/// stream containing all the line items in all the orders:
		/// <pre>{@code
		///     orders.flatMap(order -> order.getLineItems().stream())...
		/// }</pre>
		/// 
		/// </para>
		/// <para>If {@code path} is the path to a file, then the following produces a
		/// stream of the {@code words} contained in that file:
		/// <pre>{@code
		///     Stream<String> lines = Files.lines(path, StandardCharsets.UTF_8);
		///     Stream<String> words = lines.flatMap(line -> Stream.of(line.split(" +")));
		/// }</pre>
		/// The {@code mapper} function passed to {@code flatMap} splits a line,
		/// using a simple regular expression, into an array of words, and then
		/// creates a stream of words from that array.
		/// 
		/// </para>
		/// </summary>
		/// @param <R> The element type of the new stream </param>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element which produces a stream
		///               of new values </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <R> Stream<R> flatMap(java.util.function.Function<? base T, ? extends Stream<? extends R>> mapper);
		Stream<R> flatMap<R, T1>(Function<T1> mapper) where T1 : Stream<T1 extends R>;

		/// <summary>
		/// Returns an {@code IntStream} consisting of the results of replacing each
		/// element of this stream with the contents of a mapped stream produced by
		/// applying the provided mapping function to each element.  Each mapped
		/// stream is <seealso cref="java.util.stream.BaseStream#close() closed"/> after its
		/// contents have been placed into this stream.  (If a mapped stream is
		/// {@code null} an empty stream is used, instead.)
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element which produces a stream
		///               of new values </param>
		/// <returns> the new stream </returns>
		/// <seealso cref= #flatMap(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: IntStream flatMapToInt(java.util.function.Function<? base T, ? extends IntStream> mapper);
		IntStream flatMapToInt<T1>(Function<T1> mapper) where T1 : IntStream;

		/// <summary>
		/// Returns an {@code LongStream} consisting of the results of replacing each
		/// element of this stream with the contents of a mapped stream produced by
		/// applying the provided mapping function to each element.  Each mapped
		/// stream is <seealso cref="java.util.stream.BaseStream#close() closed"/> after its
		/// contents have been placed into this stream.  (If a mapped stream is
		/// {@code null} an empty stream is used, instead.)
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element which produces a stream
		///               of new values </param>
		/// <returns> the new stream </returns>
		/// <seealso cref= #flatMap(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: LongStream flatMapToLong(java.util.function.Function<? base T, ? extends LongStream> mapper);
		LongStream flatMapToLong<T1>(Function<T1> mapper) where T1 : LongStream;

		/// <summary>
		/// Returns an {@code DoubleStream} consisting of the results of replacing
		/// each element of this stream with the contents of a mapped stream produced
		/// by applying the provided mapping function to each element.  Each mapped
		/// stream is <seealso cref="java.util.stream.BaseStream#close() closed"/> after its
		/// contents have placed been into this stream.  (If a mapped stream is
		/// {@code null} an empty stream is used, instead.)
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element which produces a stream
		///               of new values </param>
		/// <returns> the new stream </returns>
		/// <seealso cref= #flatMap(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: DoubleStream flatMapToDouble(java.util.function.Function<? base T, ? extends DoubleStream> mapper);
		DoubleStream flatMapToDouble<T1>(Function<T1> mapper) where T1 : DoubleStream;

		/// <summary>
		/// Returns a stream consisting of the distinct elements (according to
		/// <seealso cref="Object#equals(Object)"/>) of this stream.
		/// 
		/// <para>For ordered streams, the selection of distinct elements is stable
		/// (for duplicated elements, the element appearing first in the encounter
		/// order is preserved.)  For unordered streams, no stability guarantees
		/// are made.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// @apiNote
		/// Preserving stability for {@code distinct()} in parallel pipelines is
		/// relatively expensive (requires that the operation act as a full barrier,
		/// with substantial buffering overhead), and stability is often not needed.
		/// Using an unordered stream source (such as <seealso cref="#generate(Supplier)"/>)
		/// or removing the ordering constraint with <seealso cref="#unordered()"/> may result
		/// in significantly more efficient execution for {@code distinct()} in parallel
		/// pipelines, if the semantics of your situation permit.  If consistency
		/// with encounter order is required, and you are experiencing poor performance
		/// or memory utilization with {@code distinct()} in parallel pipelines,
		/// switching to sequential execution with <seealso cref="#sequential()"/> may improve
		/// performance.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the new stream </returns>
		Stream<T> Distinct();

		/// <summary>
		/// Returns a stream consisting of the elements of this stream, sorted
		/// according to natural order.  If the elements of this stream are not
		/// {@code Comparable}, a {@code java.lang.ClassCastException} may be thrown
		/// when the terminal operation is executed.
		/// 
		/// <para>For ordered streams, the sort is stable.  For unordered streams, no
		/// stability guarantees are made.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the new stream </returns>
		Stream<T> Sorted();

		/// <summary>
		/// Returns a stream consisting of the elements of this stream, sorted
		/// according to the provided {@code Comparator}.
		/// 
		/// <para>For ordered streams, the sort is stable.  For unordered streams, no
		/// stability guarantees are made.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comparator"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                   <a href="package-summary.html#Statelessness">stateless</a>
		///                   {@code Comparator} to be used to compare stream elements </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Stream<T> sorted(java.util.Comparator<? base T> comparator);
		Stream<T> Sorted(IComparer<T1> comparator);

		/// <summary>
		/// Returns a stream consisting of the elements of this stream, additionally
		/// performing the provided action on each element as elements are consumed
		/// from the resulting stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// <para>For parallel stream pipelines, the action may be called at
		/// whatever time and in whatever thread the element is made available by the
		/// upstream operation.  If the action modifies shared state,
		/// it is responsible for providing the required synchronization.
		/// 
		/// @apiNote This method exists mainly to support debugging, where you want
		/// to see the elements as they flow past a certain point in a pipeline:
		/// <pre>{@code
		///     Stream.of("one", "two", "three", "four")
		///         .filter(e -> e.length() > 3)
		///         .peek(e -> System.out.println("Filtered value: " + e))
		///         .map(String::toUpperCase)
		///         .peek(e -> System.out.println("Mapped value: " + e))
		///         .collect(Collectors.toList());
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> a <a href="package-summary.html#NonInterference">
		///                 non-interfering</a> action to perform on the elements as
		///                 they are consumed from the stream </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Stream<T> peek(java.util.function.Consumer<? base T> action);
		Stream<T> Peek(Consumer<T1> action);

		/// <summary>
		/// Returns a stream consisting of the elements of this stream, truncated
		/// to be no longer than {@code maxSize} in length.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// stateful intermediate operation</a>.
		/// 
		/// @apiNote
		/// While {@code limit()} is generally a cheap operation on sequential
		/// stream pipelines, it can be quite expensive on ordered parallel pipelines,
		/// especially for large values of {@code maxSize}, since {@code limit(n)}
		/// is constrained to return not just any <em>n</em> elements, but the
		/// <em>first n</em> elements in the encounter order.  Using an unordered
		/// stream source (such as <seealso cref="#generate(Supplier)"/>) or removing the
		/// ordering constraint with <seealso cref="#unordered()"/> may result in significant
		/// speedups of {@code limit()} in parallel pipelines, if the semantics of
		/// your situation permit.  If consistency with encounter order is required,
		/// and you are experiencing poor performance or memory utilization with
		/// {@code limit()} in parallel pipelines, switching to sequential execution
		/// with <seealso cref="#sequential()"/> may improve performance.
		/// 
		/// </para>
		/// </summary>
		/// <param name="maxSize"> the number of elements the stream should be limited to </param>
		/// <returns> the new stream </returns>
		/// <exception cref="IllegalArgumentException"> if {@code maxSize} is negative </exception>
		Stream<T> Limit(long maxSize);

		/// <summary>
		/// Returns a stream consisting of the remaining elements of this stream
		/// after discarding the first {@code n} elements of the stream.
		/// If this stream contains fewer than {@code n} elements then an
		/// empty stream will be returned.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// @apiNote
		/// While {@code skip()} is generally a cheap operation on sequential
		/// stream pipelines, it can be quite expensive on ordered parallel pipelines,
		/// especially for large values of {@code n}, since {@code skip(n)}
		/// is constrained to skip not just any <em>n</em> elements, but the
		/// <em>first n</em> elements in the encounter order.  Using an unordered
		/// stream source (such as <seealso cref="#generate(Supplier)"/>) or removing the
		/// ordering constraint with <seealso cref="#unordered()"/> may result in significant
		/// speedups of {@code skip()} in parallel pipelines, if the semantics of
		/// your situation permit.  If consistency with encounter order is required,
		/// and you are experiencing poor performance or memory utilization with
		/// {@code skip()} in parallel pipelines, switching to sequential execution
		/// with <seealso cref="#sequential()"/> may improve performance.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n"> the number of leading elements to skip </param>
		/// <returns> the new stream </returns>
		/// <exception cref="IllegalArgumentException"> if {@code n} is negative </exception>
		Stream<T> Skip(long n);

		/// <summary>
		/// Performs an action for each element of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// <para>The behavior of this operation is explicitly nondeterministic.
		/// For parallel stream pipelines, this operation does <em>not</em>
		/// guarantee to respect the encounter order of the stream, as doing so
		/// would sacrifice the benefit of parallelism.  For any given element, the
		/// action may be performed at whatever time and in whatever thread the
		/// library chooses.  If the action accesses shared state, it is
		/// responsible for providing the required synchronization.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> a <a href="package-summary.html#NonInterference">
		///               non-interfering</a> action to perform on the elements </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: void forEach(java.util.function.Consumer<? base T> action);
		void forEach<T1>(Consumer<T1> action);

		/// <summary>
		/// Performs an action for each element of this stream, in the encounter
		/// order of the stream if the stream has a defined encounter order.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// <para>This operation processes the elements one at a time, in encounter
		/// order if one exists.  Performing the action for one element
		/// <a href="../concurrent/package-summary.html#MemoryVisibility"><i>happens-before</i></a>
		/// performing the action for subsequent elements, but for any given element,
		/// the action may be performed in whatever thread the library chooses.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> a <a href="package-summary.html#NonInterference">
		///               non-interfering</a> action to perform on the elements </param>
		/// <seealso cref= #forEach(Consumer) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: void forEachOrdered(java.util.function.Consumer<? base T> action);
		void forEachOrdered<T1>(Consumer<T1> action);

		/// <summary>
		/// Returns an array containing the elements of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing the elements of this stream </returns>
		Object[] ToArray();

		/// <summary>
		/// Returns an array containing the elements of this stream, using the
		/// provided {@code generator} function to allocate the returned array, as
		/// well as any additional arrays that might be required for a partitioned
		/// execution or for resizing.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// @apiNote
		/// The generator function takes an integer, which is the size of the
		/// desired array, and produces an array of the desired size.  This can be
		/// concisely expressed with an array constructor reference:
		/// <pre>{@code
		///     Person[] men = people.stream()
		///                          .filter(p -> p.getGender() == MALE)
		///                          .toArray(Person[]::new);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <A> the element type of the resulting array </param>
		/// <param name="generator"> a function which produces a new array of the desired
		///                  type and the provided length </param>
		/// <returns> an array containing the elements in this stream </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the array returned
		/// from the array generator is not a supertype of the runtime type of every
		/// element in this stream </exception>
		A[] toArray<A>(IntFunction<A[]> generator);

		/// <summary>
		/// Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		/// elements of this stream, using the provided identity value and an
		/// <a href="package-summary.html#Associativity">associative</a>
		/// accumulation function, and returns the reduced value.  This is equivalent
		/// to:
		/// <pre>{@code
		///     T result = identity;
		///     for (T element : this stream)
		///         result = accumulator.apply(result, element)
		///     return result;
		/// }</pre>
		/// 
		/// but is not constrained to execute sequentially.
		/// 
		/// <para>The {@code identity} value must be an identity for the accumulator
		/// function. This means that for all {@code t},
		/// {@code accumulator.apply(identity, t)} is equal to {@code t}.
		/// The {@code accumulator} function must be an
		/// <a href="package-summary.html#Associativity">associative</a> function.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// @apiNote Sum, min, max, average, and string concatenation are all special
		/// cases of reduction. Summing a stream of numbers can be expressed as:
		/// 
		/// <pre>{@code
		///     Integer sum = integers.reduce(0, (a, b) -> a+b);
		/// }</pre>
		/// 
		/// or:
		/// 
		/// <pre>{@code
		///     Integer sum = integers.reduce(0, Integer::sum);
		/// }</pre>
		/// 
		/// </para>
		/// <para>While this may seem a more roundabout way to perform an aggregation
		/// compared to simply mutating a running total in a loop, reduction
		/// operations parallelize more gracefully, without needing additional
		/// synchronization and with greatly reduced risk of data races.
		/// 
		/// </para>
		/// </summary>
		/// <param name="identity"> the identity value for the accumulating function </param>
		/// <param name="accumulator"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for combining two values </param>
		/// <returns> the result of the reduction </returns>
		T Reduce(T identity, BinaryOperator<T> accumulator);

		/// <summary>
		/// Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		/// elements of this stream, using an
		/// <a href="package-summary.html#Associativity">associative</a> accumulation
		/// function, and returns an {@code Optional} describing the reduced value,
		/// if any. This is equivalent to:
		/// <pre>{@code
		///     boolean foundAny = false;
		///     T result = null;
		///     for (T element : this stream) {
		///         if (!foundAny) {
		///             foundAny = true;
		///             result = element;
		///         }
		///         else
		///             result = accumulator.apply(result, element);
		///     }
		///     return foundAny ? Optional.of(result) : Optional.empty();
		/// }</pre>
		/// 
		/// but is not constrained to execute sequentially.
		/// 
		/// <para>The {@code accumulator} function must be an
		/// <a href="package-summary.html#Associativity">associative</a> function.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="accumulator"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for combining two values </param>
		/// <returns> an <seealso cref="Optional"/> describing the result of the reduction </returns>
		/// <exception cref="NullPointerException"> if the result of the reduction is null </exception>
		/// <seealso cref= #reduce(Object, BinaryOperator) </seealso>
		/// <seealso cref= #min(Comparator) </seealso>
		/// <seealso cref= #max(Comparator) </seealso>
		Optional<T> Reduce(BinaryOperator<T> accumulator);

		/// <summary>
		/// Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		/// elements of this stream, using the provided identity, accumulation and
		/// combining functions.  This is equivalent to:
		/// <pre>{@code
		///     U result = identity;
		///     for (T element : this stream)
		///         result = accumulator.apply(result, element)
		///     return result;
		/// }</pre>
		/// 
		/// but is not constrained to execute sequentially.
		/// 
		/// <para>The {@code identity} value must be an identity for the combiner
		/// function.  This means that for all {@code u}, {@code combiner(identity, u)}
		/// is equal to {@code u}.  Additionally, the {@code combiner} function
		/// must be compatible with the {@code accumulator} function; for all
		/// {@code u} and {@code t}, the following must hold:
		/// <pre>{@code
		///     combiner.apply(u, accumulator.apply(identity, t)) == accumulator.apply(u, t)
		/// }</pre>
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// @apiNote Many reductions using this form can be represented more simply
		/// by an explicit combination of {@code map} and {@code reduce} operations.
		/// The {@code accumulator} function acts as a fused mapper and accumulator,
		/// which can sometimes be more efficient than separate mapping and reduction,
		/// such as when knowing the previously reduced value allows you to avoid
		/// some computation.
		/// 
		/// </para>
		/// </summary>
		/// @param <U> The type of the result </param>
		/// <param name="identity"> the identity value for the combiner function </param>
		/// <param name="accumulator"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for incorporating an additional element into a result </param>
		/// <param name="combiner"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for combining two values, which must be
		///                    compatible with the accumulator function </param>
		/// <returns> the result of the reduction </returns>
		/// <seealso cref= #reduce(BinaryOperator) </seealso>
		/// <seealso cref= #reduce(Object, BinaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <U> U reduce(U identity, java.util.function.BiFunction<U, ? base T, U> accumulator, java.util.function.BinaryOperator<U> combiner);
		U reduce<U, T1>(U identity, BiFunction<T1> accumulator, BinaryOperator<U> combiner);

		/// <summary>
		/// Performs a <a href="package-summary.html#MutableReduction">mutable
		/// reduction</a> operation on the elements of this stream.  A mutable
		/// reduction is one in which the reduced value is a mutable result container,
		/// such as an {@code ArrayList}, and elements are incorporated by updating
		/// the state of the result rather than by replacing the result.  This
		/// produces a result equivalent to:
		/// <pre>{@code
		///     R result = supplier.get();
		///     for (T element : this stream)
		///         accumulator.accept(result, element);
		///     return result;
		/// }</pre>
		/// 
		/// <para>Like <seealso cref="#reduce(Object, BinaryOperator)"/>, {@code collect} operations
		/// can be parallelized without requiring additional synchronization.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// @apiNote There are many existing classes in the JDK whose signatures are
		/// well-suited for use with method references as arguments to {@code collect()}.
		/// For example, the following will accumulate strings into an {@code ArrayList}:
		/// <pre>{@code
		///     List<String> asList = stringStream.collect(ArrayList::new, ArrayList::add,
		///                                                ArrayList::addAll);
		/// }</pre>
		/// 
		/// </para>
		/// <para>The following will take a stream of strings and concatenates them into a
		/// single string:
		/// <pre>{@code
		///     String concat = stringStream.collect(StringBuilder::new, StringBuilder::append,
		///                                          StringBuilder::append)
		///                                 .toString();
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <R> type of the result </param>
		/// <param name="supplier"> a function that creates a new result container. For a
		///                 parallel execution, this function may be called
		///                 multiple times and must return a fresh value each time. </param>
		/// <param name="accumulator"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for incorporating an additional element into a result </param>
		/// <param name="combiner"> an <a href="package-summary.html#Associativity">associative</a>,
		///                    <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                    <a href="package-summary.html#Statelessness">stateless</a>
		///                    function for combining two values, which must be
		///                    compatible with the accumulator function </param>
		/// <returns> the result of the reduction </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <R> R collect(java.util.function.Supplier<R> supplier, java.util.function.BiConsumer<R, ? base T> accumulator, java.util.function.BiConsumer<R, R> combiner);
		R collect<R, T1>(Supplier<R> supplier, BiConsumer<T1> accumulator, BiConsumer<R, R> combiner);

		/// <summary>
		/// Performs a <a href="package-summary.html#MutableReduction">mutable
		/// reduction</a> operation on the elements of this stream using a
		/// {@code Collector}.  A {@code Collector}
		/// encapsulates the functions used as arguments to
		/// <seealso cref="#collect(Supplier, BiConsumer, BiConsumer)"/>, allowing for reuse of
		/// collection strategies and composition of collect operations such as
		/// multiple-level grouping or partitioning.
		/// 
		/// <para>If the stream is parallel, and the {@code Collector}
		/// is <seealso cref="Collector.Characteristics#CONCURRENT concurrent"/>, and
		/// either the stream is unordered or the collector is
		/// <seealso cref="Collector.Characteristics#UNORDERED unordered"/>,
		/// then a concurrent reduction will be performed (see <seealso cref="Collector"/> for
		/// details on concurrent reduction.)
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// <para>When executed in parallel, multiple intermediate results may be
		/// instantiated, populated, and merged so as to maintain isolation of
		/// mutable data structures.  Therefore, even when executed in parallel
		/// with non-thread-safe data structures (such as {@code ArrayList}), no
		/// additional synchronization is needed for a parallel reduction.
		/// 
		/// @apiNote
		/// The following will accumulate strings into an ArrayList:
		/// <pre>{@code
		///     List<String> asList = stringStream.collect(Collectors.toList());
		/// }</pre>
		/// 
		/// </para>
		/// <para>The following will classify {@code Person} objects by city:
		/// <pre>{@code
		///     Map<String, List<Person>> peopleByCity
		///         = personStream.collect(Collectors.groupingBy(Person::getCity));
		/// }</pre>
		/// 
		/// </para>
		/// <para>The following will classify {@code Person} objects by state and city,
		/// cascading two {@code Collector}s together:
		/// <pre>{@code
		///     Map<String, Map<String, List<Person>>> peopleByStateAndCity
		///         = personStream.collect(Collectors.groupingBy(Person::getState,
		///                                                      Collectors.groupingBy(Person::getCity)));
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param <R> the type of the result </param>
		/// @param <A> the intermediate accumulation type of the {@code Collector} </param>
		/// <param name="collector"> the {@code Collector} describing the reduction </param>
		/// <returns> the result of the reduction </returns>
		/// <seealso cref= #collect(Supplier, BiConsumer, BiConsumer) </seealso>
		/// <seealso cref= Collectors </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: <R, A> R collect(Collector<? base T, A, R> collector);
		R collect<R, A, T1>(Collector<T1> collector);

		/// <summary>
		/// Returns the minimum element of this stream according to the provided
		/// {@code Comparator}.  This is a special case of a
		/// <a href="package-summary.html#Reduction">reduction</a>.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comparator"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                   <a href="package-summary.html#Statelessness">stateless</a>
		///                   {@code Comparator} to compare elements of this stream </param>
		/// <returns> an {@code Optional} describing the minimum element of this stream,
		/// or an empty {@code Optional} if the stream is empty </returns>
		/// <exception cref="NullPointerException"> if the minimum element is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Optional<T> min(java.util.Comparator<? base T> comparator);
		Optional<T> Min(IComparer<T1> comparator);

		/// <summary>
		/// Returns the maximum element of this stream according to the provided
		/// {@code Comparator}.  This is a special case of a
		/// <a href="package-summary.html#Reduction">reduction</a>.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comparator"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                   <a href="package-summary.html#Statelessness">stateless</a>
		///                   {@code Comparator} to compare elements of this stream </param>
		/// <returns> an {@code Optional} describing the maximum element of this stream,
		/// or an empty {@code Optional} if the stream is empty </returns>
		/// <exception cref="NullPointerException"> if the maximum element is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Optional<T> max(java.util.Comparator<? base T> comparator);
		Optional<T> Max(IComparer<T1> comparator);

		/// <summary>
		/// Returns the count of elements in this stream.  This is a special case of
		/// a <a href="package-summary.html#Reduction">reduction</a> and is
		/// equivalent to:
		/// <pre>{@code
		///     return mapToLong(e -> 1L).sum();
		/// }</pre>
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the count of elements in this stream </returns>
		long Count();

		/// <summary>
		/// Returns whether any elements of this stream match the provided
		/// predicate.  May not evaluate the predicate on all elements if not
		/// necessary for determining the result.  If the stream is empty then
		/// {@code false} is returned and the predicate is not evaluated.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// @apiNote
		/// This method evaluates the <em>existential quantification</em> of the
		/// predicate over the elements of the stream (for some x P(x)).
		/// 
		/// </para>
		/// </summary>
		/// <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                  <a href="package-summary.html#Statelessness">stateless</a>
		///                  predicate to apply to elements of this stream </param>
		/// <returns> {@code true} if any elements of the stream match the provided
		/// predicate, otherwise {@code false} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean anyMatch(java.util.function.Predicate<? base T> predicate);
		bool anyMatch<T1>(Predicate<T1> predicate);

		/// <summary>
		/// Returns whether all elements of this stream match the provided predicate.
		/// May not evaluate the predicate on all elements if not necessary for
		/// determining the result.  If the stream is empty then {@code true} is
		/// returned and the predicate is not evaluated.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// @apiNote
		/// This method evaluates the <em>universal quantification</em> of the
		/// predicate over the elements of the stream (for all x P(x)).  If the
		/// stream is empty, the quantification is said to be <em>vacuously
		/// satisfied</em> and is always {@code true} (regardless of P(x)).
		/// 
		/// </para>
		/// </summary>
		/// <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                  <a href="package-summary.html#Statelessness">stateless</a>
		///                  predicate to apply to elements of this stream </param>
		/// <returns> {@code true} if either all elements of the stream match the
		/// provided predicate or the stream is empty, otherwise {@code false} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean allMatch(java.util.function.Predicate<? base T> predicate);
		bool allMatch<T1>(Predicate<T1> predicate);

		/// <summary>
		/// Returns whether no elements of this stream match the provided predicate.
		/// May not evaluate the predicate on all elements if not necessary for
		/// determining the result.  If the stream is empty then {@code true} is
		/// returned and the predicate is not evaluated.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// @apiNote
		/// This method evaluates the <em>universal quantification</em> of the
		/// negated predicate over the elements of the stream (for all x ~P(x)).  If
		/// the stream is empty, the quantification is said to be vacuously satisfied
		/// and is always {@code true}, regardless of P(x).
		/// 
		/// </para>
		/// </summary>
		/// <param name="predicate"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///                  <a href="package-summary.html#Statelessness">stateless</a>
		///                  predicate to apply to elements of this stream </param>
		/// <returns> {@code true} if either no elements of the stream match the
		/// provided predicate or the stream is empty, otherwise {@code false} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean noneMatch(java.util.function.Predicate<? base T> predicate);
		bool noneMatch<T1>(Predicate<T1> predicate);

		/// <summary>
		/// Returns an <seealso cref="Optional"/> describing the first element of this stream,
		/// or an empty {@code Optional} if the stream is empty.  If the stream has
		/// no encounter order, then any element may be returned.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code Optional} describing the first element of this stream,
		/// or an empty {@code Optional} if the stream is empty </returns>
		/// <exception cref="NullPointerException"> if the element selected is null </exception>
		Optional<T> FindFirst();

		/// <summary>
		/// Returns an <seealso cref="Optional"/> describing some element of the stream, or an
		/// empty {@code Optional} if the stream is empty.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// </para>
		/// <para>The behavior of this operation is explicitly nondeterministic; it is
		/// free to select any element in the stream.  This is to allow for maximal
		/// performance in parallel operations; the cost is that multiple invocations
		/// on the same source may not return the same result.  (If a stable result
		/// is desired, use <seealso cref="#findFirst()"/> instead.)
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code Optional} describing some element of this stream, or an
		/// empty {@code Optional} if the stream is empty </returns>
		/// <exception cref="NullPointerException"> if the element selected is null </exception>
		/// <seealso cref= #findFirst() </seealso>
		Optional<T> FindAny();

		// Static factories

		/// <summary>
		/// Returns a builder for a {@code Stream}.
		/// </summary>
		/// @param <T> type of elements </param>
		/// <returns> a stream builder </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream_Builder<T> builder()
	//	{
	//		return new Streams.StreamBuilderImpl<>();
	//	}

		/// <summary>
		/// Returns an empty sequential {@code Stream}.
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <returns> an empty sequential stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream<T> empty()
	//	{
	//		return StreamSupport.stream(Spliterators.emptySpliterator<T>(), false);
	//	}

		/// <summary>
		/// Returns a sequential {@code Stream} containing a single element.
		/// </summary>
		/// <param name="t"> the single element </param>
		/// @param <T> the type of stream elements </param>
		/// <returns> a singleton sequential stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream<T> of(T Stream_Fields.t)
	//	{
	//		return StreamSupport.stream(new Streams.StreamBuilderImpl<>(t), false);
	//	}

		/// <summary>
		/// Returns a sequential ordered stream whose elements are the specified values.
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="values"> the elements of the new stream </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SafeVarargs @SuppressWarnings("varargs") public static<T> Stream<T> of(T... values)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream<T> of(T... values) // Creating a stream from an array is safe
	//	{
	//		return Arrays.stream(values);
	//	}

		/// <summary>
		/// Returns an infinite sequential ordered {@code Stream} produced by iterative
		/// application of a function {@code f} to an initial element {@code seed},
		/// producing a {@code Stream} consisting of {@code seed}, {@code f(seed)},
		/// {@code f(f(seed))}, etc.
		/// 
		/// <para>The first element (position {@code 0}) in the {@code Stream} will be
		/// the provided {@code seed}.  For {@code n > 0}, the element at position
		/// {@code n}, will be the result of applying the function {@code f} to the
		/// element at position {@code n - 1}.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="seed"> the initial element </param>
		/// <param name="f"> a function to be applied to to the previous element to produce
		///          a new element </param>
		/// <returns> a new sequential {@code Stream} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream<T> iterate(final T seed, final java.util.function.UnaryOperator<T> f)
	//	{
	//		Objects.requireNonNull(f);
	//		final Iterator<T> iterator = new Iterator<T>()
	//		{
	//			@@SuppressWarnings("unchecked") T t = (T) Streams.NONE;
	//
	//			@@Override public boolean hasNext()
	//			{
	//			}
	//
	//			@@Override public T next()
	//			{
	//			}
	//		};
	//		return StreamSupport.stream(Spliterators.spliteratorUnknownSize(iterator, Spliterator.ORDERED | Spliterator.IMMUTABLE), false);
	//	}

		/// <summary>
		/// Returns an infinite sequential unordered stream where each element is
		/// generated by the provided {@code Supplier}.  This is suitable for
		/// generating constant streams, streams of random elements, etc.
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="s"> the {@code Supplier} of generated elements </param>
		/// <returns> a new infinite sequential unordered {@code Stream} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Stream<T> generate(java.util.function.Supplier<T> s)
	//	{
	//		Objects.requireNonNull(s);
	//		return StreamSupport.stream(new StreamSpliterators.InfiniteSupplyingSpliterator.OfRef<>(Long.MAX_VALUE, s), false);
	//	}

		/// <summary>
		/// Creates a lazily concatenated stream whose elements are all the
		/// elements of the first stream followed by all the elements of the
		/// second stream.  The resulting stream is ordered if both
		/// of the input streams are ordered, and parallel if either of the input
		/// streams is parallel.  When the resulting stream is closed, the close
		/// handlers for both input streams are invoked.
		/// 
		/// @implNote
		/// Use caution when constructing streams from repeated concatenation.
		/// Accessing an element of a deeply concatenated stream can result in deep
		/// call chains, or even {@code StackOverflowException}.
		/// </summary>
		/// @param <T> The type of stream elements </param>
		/// <param name="a"> the first stream </param>
		/// <param name="b"> the second stream </param>
		/// <returns> the concatenation of the two input streams </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T> Stream<T> concat(Stream<? extends T> a, Stream<? extends T> b)
//		public static <T> Stream<T> concat(Stream<JavaToDotNetGenericWildcard> a, Stream<JavaToDotNetGenericWildcard> b) where ? : T where ? : T
	//	{
	//		Objects.requireNonNull(a);
	//		Objects.requireNonNull(b);
	//
	//		@@SuppressWarnings("unchecked") Spliterator<T> split = new Streams.ConcatSpliterator.OfRef<>((Spliterator<T>) a.spliterator(), (Spliterator<T>) b.spliterator());
	//		return stream.onClose(Streams.composedClose(a, b));
	//	}

		/// <summary>
		/// A mutable builder for a {@code Stream}.  This allows the creation of a
		/// {@code Stream} by generating elements individually and adding them to the
		/// {@code Builder} (without the copying overhead that comes from using
		/// an {@code ArrayList} as a temporary buffer.)
		/// 
		/// <para>A stream builder has a lifecycle, which starts in a building
		/// phase, during which elements can be added, and then transitions to a built
		/// phase, after which elements may not be added.  The built phase begins
		/// when the <seealso cref="#build()"/> method is called, which creates an ordered
		/// {@code Stream} whose elements are the elements that were added to the stream
		/// builder, in the order they were added.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <seealso cref= Stream#builder()
		/// @since 1.8 </seealso>
	}

	public static class Stream_Fields
	{
					public static readonly return True;
					public static readonly return t = (t == Streams.NONE) ? seed : f.apply(t);
			public static readonly Stream<T> Stream = StreamSupport.Stream(split, a.Parallel || b.Parallel);
	}

	public interface Stream_Builder<T> : Consumer<T>
	{

		/// <summary>
		/// Adds an element to the stream being built.
		/// </summary>
		/// <exception cref="IllegalStateException"> if the builder has already transitioned to
		/// the built state </exception>
		void Accept(T Stream_Fields);

		/// <summary>
		/// Adds an element to the stream being built.
		/// 
		/// @implSpec
		/// The default implementation behaves as if:
		/// <pre>{@code
		///     accept(t)
		///     return this;
		/// }</pre>
		/// </summary>
		/// <param name="t"> the element to add </param>
		/// <returns> {@code this} builder </returns>
		/// <exception cref="IllegalStateException"> if the builder has already transitioned to
		/// the built state </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Stream_Builder<T> add(T Stream_Fields.t)
	//	{
	//		accept(Stream_Fields.t);
	//	}

		/// <summary>
		/// Builds the stream, transitioning this builder to the built state.
		/// An {@code IllegalStateException} is thrown if there are further attempts
		/// to operate on the builder after it has entered the built state.
		/// </summary>
		/// <returns> the built stream </returns>
		/// <exception cref="IllegalStateException"> if the builder has already transitioned to
		/// the built state </exception>
		Stream<T> Build();

	}

	public static class Stream_Builder_Fields
	{
			public static readonly return this;
	}

}