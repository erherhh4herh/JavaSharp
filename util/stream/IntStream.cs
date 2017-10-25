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
	/// A sequence of primitive int-valued elements supporting sequential and parallel
	/// aggregate operations.  This is the {@code int} primitive specialization of
	/// <seealso cref="Stream"/>.
	/// 
	/// <para>The following example illustrates an aggregate operation using
	/// <seealso cref="Stream"/> and <seealso cref="IntStream"/>, computing the sum of the weights of the
	/// red widgets:
	/// 
	/// <pre>{@code
	///     int sum = widgets.stream()
	///                      .filter(w -> w.getColor() == RED)
	///                      .mapToInt(w -> w.getWeight())
	///                      .sum();
	/// }</pre>
	/// 
	/// See the class documentation for <seealso cref="Stream"/> and the package documentation
	/// for <a href="package-summary.html">java.util.stream</a> for additional
	/// specification of streams, stream operations, stream pipelines, and
	/// parallelism.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	/// <seealso cref= Stream </seealso>
	/// <seealso cref= <a href="package-summary.html">java.util.stream</a> </seealso>
	public interface IntStream : BaseStream<Integer, IntStream>
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
		IntStream Filter(IntPredicate predicate);

		/// <summary>
		/// Returns a stream consisting of the results of applying the given
		/// function to the elements of this stream.
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
		IntStream Map(IntUnaryOperator mapper);

		/// <summary>
		/// Returns an object-valued {@code Stream} consisting of the results of
		/// applying the given function to the elements of this stream.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">
		///     intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// @param <U> the element type of the new stream </param>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element </param>
		/// <returns> the new stream </returns>
		Stream<U> mapToObj<U, T1>(IntFunction<T1> mapper) where T1 : U;

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
		LongStream MapToLong(IntToLongFunction mapper);

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
		DoubleStream MapToDouble(IntToDoubleFunction mapper);

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
		/// </para>
		/// </summary>
		/// <param name="mapper"> a <a href="package-summary.html#NonInterference">non-interfering</a>,
		///               <a href="package-summary.html#Statelessness">stateless</a>
		///               function to apply to each element which produces an
		///               {@code IntStream} of new values </param>
		/// <returns> the new stream </returns>
		/// <seealso cref= Stream#flatMap(Function) </seealso>
		IntStream flatMap<T1>(IntFunction<T1> mapper) where T1 : IntStream;

		/// <summary>
		/// Returns a stream consisting of the distinct elements of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the new stream </returns>
		IntStream Distinct();

		/// <summary>
		/// Returns a stream consisting of the elements of this stream in sorted
		/// order.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">stateful
		/// intermediate operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the new stream </returns>
		IntStream Sorted();

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
		///     IntStream.of(1, 2, 3, 4)
		///         .filter(e -> e > 2)
		///         .peek(e -> System.out.println("Filtered value: " + e))
		///         .map(e -> e * e)
		///         .peek(e -> System.out.println("Mapped value: " + e))
		///         .sum();
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> a <a href="package-summary.html#NonInterference">
		///               non-interfering</a> action to perform on the elements as
		///               they are consumed from the stream </param>
		/// <returns> the new stream </returns>
		IntStream Peek(IntConsumer action);

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
		/// stream source (such as <seealso cref="#generate(IntSupplier)"/>) or removing the
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
		IntStream Limit(long maxSize);

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
		/// stream source (such as <seealso cref="#generate(IntSupplier)"/>) or removing the
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
		IntStream Skip(long n);

		/// <summary>
		/// Performs an action for each element of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// <para>For parallel stream pipelines, this operation does <em>not</em>
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
		void ForEach(IntConsumer action);

		/// <summary>
		/// Performs an action for each element of this stream, guaranteeing that
		/// each element is processed in encounter order for streams that have a
		/// defined encounter order.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> a <a href="package-summary.html#NonInterference">
		///               non-interfering</a> action to perform on the elements </param>
		/// <seealso cref= #forEach(IntConsumer) </seealso>
		void ForEachOrdered(IntConsumer action);

		/// <summary>
		/// Returns an array containing the elements of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing the elements of this stream </returns>
		int[] ToArray();

		/// <summary>
		/// Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		/// elements of this stream, using the provided identity value and an
		/// <a href="package-summary.html#Associativity">associative</a>
		/// accumulation function, and returns the reduced value.  This is equivalent
		/// to:
		/// <pre>{@code
		///     int result = identity;
		///     for (int element : this stream)
		///         result = accumulator.applyAsInt(result, element)
		///     return result;
		/// }</pre>
		/// 
		/// but is not constrained to execute sequentially.
		/// 
		/// <para>The {@code identity} value must be an identity for the accumulator
		/// function. This means that for all {@code x},
		/// {@code accumulator.apply(identity, x)} is equal to {@code x}.
		/// The {@code accumulator} function must be an
		/// <a href="package-summary.html#Associativity">associative</a> function.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// @apiNote Sum, min, max, and average are all special cases of reduction.
		/// Summing a stream of numbers can be expressed as:
		/// 
		/// <pre>{@code
		///     int sum = integers.reduce(0, (a, b) -> a+b);
		/// }</pre>
		/// 
		/// or more compactly:
		/// 
		/// <pre>{@code
		///     int sum = integers.reduce(0, Integer::sum);
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
		/// <param name="op"> an <a href="package-summary.html#Associativity">associative</a>,
		///           <a href="package-summary.html#NonInterference">non-interfering</a>,
		///           <a href="package-summary.html#Statelessness">stateless</a>
		///           function for combining two values </param>
		/// <returns> the result of the reduction </returns>
		/// <seealso cref= #sum() </seealso>
		/// <seealso cref= #min() </seealso>
		/// <seealso cref= #max() </seealso>
		/// <seealso cref= #average() </seealso>
		int Reduce(int identity, IntBinaryOperator op);

		/// <summary>
		/// Performs a <a href="package-summary.html#Reduction">reduction</a> on the
		/// elements of this stream, using an
		/// <a href="package-summary.html#Associativity">associative</a> accumulation
		/// function, and returns an {@code OptionalInt} describing the reduced value,
		/// if any. This is equivalent to:
		/// <pre>{@code
		///     boolean foundAny = false;
		///     int result = null;
		///     for (int element : this stream) {
		///         if (!foundAny) {
		///             foundAny = true;
		///             result = element;
		///         }
		///         else
		///             result = accumulator.applyAsInt(result, element);
		///     }
		///     return foundAny ? OptionalInt.of(result) : OptionalInt.empty();
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
		/// <param name="op"> an <a href="package-summary.html#Associativity">associative</a>,
		///           <a href="package-summary.html#NonInterference">non-interfering</a>,
		///           <a href="package-summary.html#Statelessness">stateless</a>
		///           function for combining two values </param>
		/// <returns> the result of the reduction </returns>
		/// <seealso cref= #reduce(int, IntBinaryOperator) </seealso>
		OptionalInt Reduce(IntBinaryOperator op);

		/// <summary>
		/// Performs a <a href="package-summary.html#MutableReduction">mutable
		/// reduction</a> operation on the elements of this stream.  A mutable
		/// reduction is one in which the reduced value is a mutable result container,
		/// such as an {@code ArrayList}, and elements are incorporated by updating
		/// the state of the result rather than by replacing the result.  This
		/// produces a result equivalent to:
		/// <pre>{@code
		///     R result = supplier.get();
		///     for (int element : this stream)
		///         accumulator.accept(result, element);
		///     return result;
		/// }</pre>
		/// 
		/// <para>Like <seealso cref="#reduce(int, IntBinaryOperator)"/>, {@code collect} operations
		/// can be parallelized without requiring additional synchronization.
		/// 
		/// </para>
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
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
		/// <seealso cref= Stream#collect(Supplier, BiConsumer, BiConsumer) </seealso>
		R collect<R>(Supplier<R> supplier, ObjIntConsumer<R> accumulator, BiConsumer<R, R> combiner);

		/// <summary>
		/// Returns the sum of elements in this stream.  This is a special case
		/// of a <a href="package-summary.html#Reduction">reduction</a>
		/// and is equivalent to:
		/// <pre>{@code
		///     return reduce(0, Integer::sum);
		/// }</pre>
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the sum of elements in this stream </returns>
		int Sum();

		/// <summary>
		/// Returns an {@code OptionalInt} describing the minimum element of this
		/// stream, or an empty optional if this stream is empty.  This is a special
		/// case of a <a href="package-summary.html#Reduction">reduction</a>
		/// and is equivalent to:
		/// <pre>{@code
		///     return reduce(Integer::min);
		/// }</pre>
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code OptionalInt} containing the minimum element of this
		/// stream, or an empty {@code OptionalInt} if the stream is empty </returns>
		OptionalInt Min();

		/// <summary>
		/// Returns an {@code OptionalInt} describing the maximum element of this
		/// stream, or an empty optional if this stream is empty.  This is a special
		/// case of a <a href="package-summary.html#Reduction">reduction</a>
		/// and is equivalent to:
		/// <pre>{@code
		///     return reduce(Integer::max);
		/// }</pre>
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code OptionalInt} containing the maximum element of this
		/// stream, or an empty {@code OptionalInt} if the stream is empty </returns>
		OptionalInt Max();

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
		/// Returns an {@code OptionalDouble} describing the arithmetic mean of elements of
		/// this stream, or an empty optional if this stream is empty.  This is a
		/// special case of a
		/// <a href="package-summary.html#Reduction">reduction</a>.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code OptionalDouble} containing the average element of this
		/// stream, or an empty optional if the stream is empty </returns>
		OptionalDouble Average();

		/// <summary>
		/// Returns an {@code IntSummaryStatistics} describing various
		/// summary data about the elements of this stream.  This is a special
		/// case of a <a href="package-summary.html#Reduction">reduction</a>.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code IntSummaryStatistics} describing various summary data
		/// about the elements of this stream </returns>
		IntSummaryStatistics SummaryStatistics();

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
		bool AnyMatch(IntPredicate predicate);

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
		bool AllMatch(IntPredicate predicate);

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
		bool NoneMatch(IntPredicate predicate);

		/// <summary>
		/// Returns an <seealso cref="OptionalInt"/> describing the first element of this
		/// stream, or an empty {@code OptionalInt} if the stream is empty.  If the
		/// stream has no encounter order, then any element may be returned.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">short-circuiting
		/// terminal operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code OptionalInt} describing the first element of this stream,
		/// or an empty {@code OptionalInt} if the stream is empty </returns>
		OptionalInt FindFirst();

		/// <summary>
		/// Returns an <seealso cref="OptionalInt"/> describing some element of the stream, or
		/// an empty {@code OptionalInt} if the stream is empty.
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
		/// <returns> an {@code OptionalInt} describing some element of this stream, or
		/// an empty {@code OptionalInt} if the stream is empty </returns>
		/// <seealso cref= #findFirst() </seealso>
		OptionalInt FindAny();

		/// <summary>
		/// Returns a {@code LongStream} consisting of the elements of this stream,
		/// converted to {@code long}.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code LongStream} consisting of the elements of this stream,
		/// converted to {@code long} </returns>
		LongStream AsLongStream();

		/// <summary>
		/// Returns a {@code DoubleStream} consisting of the elements of this stream,
		/// converted to {@code double}.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code DoubleStream} consisting of the elements of this stream,
		/// converted to {@code double} </returns>
		DoubleStream AsDoubleStream();

		/// <summary>
		/// Returns a {@code Stream} consisting of the elements of this stream,
		/// each boxed to an {@code Integer}.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Stream} consistent of the elements of this stream,
		/// each boxed to an {@code Integer} </returns>
		Stream<Integer> Boxed();

		IntStream Sequential();

		IntStream Parallel();

		java.util.PrimitiveIterator_OfInt Iterator();

		java.util.Spliterator_OfInt Spliterator();

		// Static factories

		/// <summary>
		/// Returns a builder for an {@code IntStream}.
		/// </summary>
		/// <returns> a stream builder </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream_Builder builder()
	//	{
	//		return new Streams.IntStreamBuilderImpl();
	//	}

		/// <summary>
		/// Returns an empty sequential {@code IntStream}.
		/// </summary>
		/// <returns> an empty sequential stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream empty()
	//	{
	//		return StreamSupport.intStream(Spliterators.emptyIntSpliterator(), false);
	//	}

		/// <summary>
		/// Returns a sequential {@code IntStream} containing a single element.
		/// </summary>
		/// <param name="t"> the single element </param>
		/// <returns> a singleton sequential stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream of(int IntStream_Fields.t)
	//	{
	//		return StreamSupport.intStream(new Streams.IntStreamBuilderImpl(t), false);
	//	}

		/// <summary>
		/// Returns a sequential ordered stream whose elements are the specified values.
		/// </summary>
		/// <param name="values"> the elements of the new stream </param>
		/// <returns> the new stream </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream of(int... values)
	//	{
	//		return Arrays.stream(values);
	//	}

		/// <summary>
		/// Returns an infinite sequential ordered {@code IntStream} produced by iterative
		/// application of a function {@code f} to an initial element {@code seed},
		/// producing a {@code Stream} consisting of {@code seed}, {@code f(seed)},
		/// {@code f(f(seed))}, etc.
		/// 
		/// <para>The first element (position {@code 0}) in the {@code IntStream} will be
		/// the provided {@code seed}.  For {@code n > 0}, the element at position
		/// {@code n}, will be the result of applying the function {@code f} to the
		/// element at position {@code n - 1}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seed"> the initial element </param>
		/// <param name="f"> a function to be applied to to the previous element to produce
		///          a new element </param>
		/// <returns> A new sequential {@code IntStream} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream iterate(final int seed, final java.util.function.IntUnaryOperator f)
	//	{
	//		Objects.requireNonNull(f);
	//		final PrimitiveIterator.OfInt iterator = new PrimitiveIterator.OfInt()
	//		{
	//
	//			@@Override public boolean hasNext()
	//			{
	//			}
	//
	//			@@Override public int nextInt()
	//			{
	//				t = f.applyAsInt(t);
	//			}
	//		};
	//		return StreamSupport.intStream(Spliterators.spliteratorUnknownSize(iterator, Spliterator.ORDERED | Spliterator.IMMUTABLE | Spliterator.NONNULL), false);
	//	}

		/// <summary>
		/// Returns an infinite sequential unordered stream where each element is
		/// generated by the provided {@code IntSupplier}.  This is suitable for
		/// generating constant streams, streams of random elements, etc.
		/// </summary>
		/// <param name="s"> the {@code IntSupplier} for generated elements </param>
		/// <returns> a new infinite sequential unordered {@code IntStream} </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream generate(java.util.function.IntSupplier s)
	//	{
	//		Objects.requireNonNull(s);
	//		return StreamSupport.intStream(new StreamSpliterators.InfiniteSupplyingSpliterator.OfInt(Long.MAX_VALUE, s), false);
	//	}

		/// <summary>
		/// Returns a sequential ordered {@code IntStream} from {@code startInclusive}
		/// (inclusive) to {@code endExclusive} (exclusive) by an incremental step of
		/// {@code 1}.
		/// 
		/// @apiNote
		/// <para>An equivalent sequence of increasing values can be produced
		/// sequentially using a {@code for} loop as follows:
		/// <pre>{@code
		///     for (int i = startInclusive; i < endExclusive ; i++) { ... }
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="startInclusive"> the (inclusive) initial value </param>
		/// <param name="endExclusive"> the exclusive upper bound </param>
		/// <returns> a sequential {@code IntStream} for the range of {@code int}
		///         elements </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream range(int startInclusive, int endExclusive)
	//	{
	//		if (startInclusive >= endExclusive)
	//		{
	//			return empty();
	//		}
	//		else
	//		{
	//			return StreamSupport.intStream(new Streams.RangeIntSpliterator(startInclusive, endExclusive, false), false);
	//		}
	//	}

		/// <summary>
		/// Returns a sequential ordered {@code IntStream} from {@code startInclusive}
		/// (inclusive) to {@code endInclusive} (inclusive) by an incremental step of
		/// {@code 1}.
		/// 
		/// @apiNote
		/// <para>An equivalent sequence of increasing values can be produced
		/// sequentially using a {@code for} loop as follows:
		/// <pre>{@code
		///     for (int i = startInclusive; i <= endInclusive ; i++) { ... }
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="startInclusive"> the (inclusive) initial value </param>
		/// <param name="endInclusive"> the inclusive upper bound </param>
		/// <returns> a sequential {@code IntStream} for the range of {@code int}
		///         elements </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream rangeClosed(int startInclusive, int endInclusive)
	//	{
	//		if (startInclusive > endInclusive)
	//		{
	//			return empty();
	//		}
	//		else
	//		{
	//			return StreamSupport.intStream(new Streams.RangeIntSpliterator(startInclusive, endInclusive, true), false);
	//		}
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
		/// <param name="a"> the first stream </param>
		/// <param name="b"> the second stream </param>
		/// <returns> the concatenation of the two input streams </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static IntStream concat(IntStream a, IntStream b)
	//	{
	//		Objects.requireNonNull(a);
	//		Objects.requireNonNull(b);
	//		return stream.onClose(Streams.composedClose(a, b));
	//	}

		/// <summary>
		/// A mutable builder for an {@code IntStream}.
		/// 
		/// <para>A stream builder has a lifecycle, which starts in a building
		/// phase, during which elements can be added, and then transitions to a built
		/// phase, after which elements may not be added.  The built phase
		/// begins when the <seealso cref="#build()"/> method is called, which creates an
		/// ordered stream whose elements are the elements that were added to the
		/// stream builder, in the order they were added.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= IntStream#builder()
		/// @since 1.8 </seealso>
	}

	public static class IntStream_Fields
	{
				public static readonly int t = seed;
					public static readonly return True;
					public static readonly int v = t;
					public static readonly return v;
			public static readonly java.util.Spliterator_OfInt Split = new Streams.ConcatSpliterator.OfInt(a.spliterator(), b.spliterator());
			public static readonly IntStream Stream = StreamSupport.IntStream(Split, a.Parallel || b.Parallel);
	}

	public interface IntStream_Builder : IntConsumer
	{

		/// <summary>
		/// Adds an element to the stream being built.
		/// </summary>
		/// <exception cref="IllegalStateException"> if the builder has already transitioned
		/// to the built state </exception>
		void Accept(int IntStream_Fields);

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
		/// <exception cref="IllegalStateException"> if the builder has already transitioned
		/// to the built state </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default IntStream_Builder add(int IntStream_Fields.t)
	//	{
	//		accept(IntStream_Fields.t);
	//	}

		/// <summary>
		/// Builds the stream, transitioning this builder to the built state.
		/// An {@code IllegalStateException} is thrown if there are further
		/// attempts to operate on the builder after it has entered the built
		/// state.
		/// </summary>
		/// <returns> the built stream </returns>
		/// <exception cref="IllegalStateException"> if the builder has already transitioned to
		/// the built state </exception>
		IntStream Build();
	}

	public static class IntStream_Builder_Fields
	{
			public static readonly return this;
	}

}