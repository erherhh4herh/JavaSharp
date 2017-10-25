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
	/// Low-level utility methods for creating and manipulating streams.
	/// 
	/// <para>This class is mostly for library writers presenting stream views
	/// of data structures; most static stream methods intended for end users are in
	/// the various {@code Stream} classes.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class StreamSupport
	{

		// Suppresses default constructor, ensuring non-instantiability.
		private StreamSupport()
		{
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code Stream} from a
		/// {@code Spliterator}.
		/// 
		/// <para>The spliterator is only traversed, split, or queried for estimated
		/// size after the terminal operation of the stream pipeline commences.
		/// 
		/// </para>
		/// <para>It is strongly recommended the spliterator report a characteristic of
		/// {@code IMMUTABLE} or {@code CONCURRENT}, or be
		/// <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		/// <seealso cref="#stream(java.util.function.Supplier, int, boolean)"/> should be used
		/// to reduce the scope of potential interference with the source.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="spliterator"> a {@code Spliterator} describing the stream elements </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code Stream} </returns>
		public static Stream<T> stream<T>(Spliterator<T> spliterator, bool parallel)
		{
			Objects.RequireNonNull(spliterator);
			return new ReferencePipeline.Head<>(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code Stream} from a
		/// {@code Supplier} of {@code Spliterator}.
		/// 
		/// <para>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		/// more than once, and only after the terminal operation of the stream pipeline
		/// commences.
		/// 
		/// </para>
		/// <para>For spliterators that report a characteristic of {@code IMMUTABLE}
		/// or {@code CONCURRENT}, or that are
		/// <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		/// more efficient to use <seealso cref="#stream(java.util.Spliterator, boolean)"/>
		/// instead.
		/// </para>
		/// <para>The use of a {@code Supplier} in this form provides a level of
		/// indirection that reduces the scope of potential interference with the
		/// source.  Since the supplier is only invoked after the terminal operation
		/// commences, any modifications to the source up to the start of the
		/// terminal operation are reflected in the stream result.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="supplier"> a {@code Supplier} of a {@code Spliterator} </param>
		/// <param name="characteristics"> Spliterator characteristics of the supplied
		///        {@code Spliterator}.  The characteristics must be equal to
		///        {@code supplier.get().characteristics()}, otherwise undefined
		///        behavior may occur when terminal operation commences. </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code Stream} </returns>
		/// <seealso cref= #stream(java.util.Spliterator, boolean) </seealso>
		public static Stream<T> stream<T, T1>(Supplier<T1> supplier, int characteristics, bool parallel) where T1 : java.util.Spliterator<T>
		{
			Objects.RequireNonNull(supplier);
			return new ReferencePipeline.Head<>(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code IntStream} from a
		/// {@code Spliterator.OfInt}.
		/// 
		/// <para>The spliterator is only traversed, split, or queried for estimated size
		/// after the terminal operation of the stream pipeline commences.
		/// 
		/// </para>
		/// <para>It is strongly recommended the spliterator report a characteristic of
		/// {@code IMMUTABLE} or {@code CONCURRENT}, or be
		/// <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		/// <seealso cref="#intStream(java.util.function.Supplier, int, boolean)"/> should be
		/// used to reduce the scope of potential interference with the source.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> a {@code Spliterator.OfInt} describing the stream elements </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code IntStream} </returns>
		public static IntStream IntStream(java.util.Spliterator_OfInt spliterator, bool parallel)
		{
			return new IntPipeline.Head<>(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code IntStream} from a
		/// {@code Supplier} of {@code Spliterator.OfInt}.
		/// 
		/// <para>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		/// more than once, and only after the terminal operation of the stream pipeline
		/// commences.
		/// 
		/// </para>
		/// <para>For spliterators that report a characteristic of {@code IMMUTABLE}
		/// or {@code CONCURRENT}, or that are
		/// <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		/// more efficient to use <seealso cref="#intStream(java.util.Spliterator.OfInt, boolean)"/>
		/// instead.
		/// </para>
		/// <para>The use of a {@code Supplier} in this form provides a level of
		/// indirection that reduces the scope of potential interference with the
		/// source.  Since the supplier is only invoked after the terminal operation
		/// commences, any modifications to the source up to the start of the
		/// terminal operation are reflected in the stream result.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="supplier"> a {@code Supplier} of a {@code Spliterator.OfInt} </param>
		/// <param name="characteristics"> Spliterator characteristics of the supplied
		///        {@code Spliterator.OfInt}.  The characteristics must be equal to
		///        {@code supplier.get().characteristics()}, otherwise undefined
		///        behavior may occur when terminal operation commences. </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code IntStream} </returns>
		/// <seealso cref= #intStream(java.util.Spliterator.OfInt, boolean) </seealso>
		public static IntStream intStream<T1>(Supplier<T1> supplier, int characteristics, bool parallel) where T1 : java.util.Spliterator_OfInt
		{
			return new IntPipeline.Head<>(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code LongStream} from a
		/// {@code Spliterator.OfLong}.
		/// 
		/// <para>The spliterator is only traversed, split, or queried for estimated
		/// size after the terminal operation of the stream pipeline commences.
		/// 
		/// </para>
		/// <para>It is strongly recommended the spliterator report a characteristic of
		/// {@code IMMUTABLE} or {@code CONCURRENT}, or be
		/// <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		/// <seealso cref="#longStream(java.util.function.Supplier, int, boolean)"/> should be
		/// used to reduce the scope of potential interference with the source.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> a {@code Spliterator.OfLong} describing the stream elements </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code LongStream} </returns>
		public static LongStream LongStream(java.util.Spliterator_OfLong spliterator, bool parallel)
		{
			return new LongPipeline.Head<>(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code LongStream} from a
		/// {@code Supplier} of {@code Spliterator.OfLong}.
		/// 
		/// <para>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		/// more than once, and only after the terminal operation of the stream pipeline
		/// commences.
		/// 
		/// </para>
		/// <para>For spliterators that report a characteristic of {@code IMMUTABLE}
		/// or {@code CONCURRENT}, or that are
		/// <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		/// more efficient to use <seealso cref="#longStream(java.util.Spliterator.OfLong, boolean)"/>
		/// instead.
		/// </para>
		/// <para>The use of a {@code Supplier} in this form provides a level of
		/// indirection that reduces the scope of potential interference with the
		/// source.  Since the supplier is only invoked after the terminal operation
		/// commences, any modifications to the source up to the start of the
		/// terminal operation are reflected in the stream result.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="supplier"> a {@code Supplier} of a {@code Spliterator.OfLong} </param>
		/// <param name="characteristics"> Spliterator characteristics of the supplied
		///        {@code Spliterator.OfLong}.  The characteristics must be equal to
		///        {@code supplier.get().characteristics()}, otherwise undefined
		///        behavior may occur when terminal operation commences. </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code LongStream} </returns>
		/// <seealso cref= #longStream(java.util.Spliterator.OfLong, boolean) </seealso>
		public static LongStream longStream<T1>(Supplier<T1> supplier, int characteristics, bool parallel) where T1 : java.util.Spliterator_OfLong
		{
			return new LongPipeline.Head<>(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code DoubleStream} from a
		/// {@code Spliterator.OfDouble}.
		/// 
		/// <para>The spliterator is only traversed, split, or queried for estimated size
		/// after the terminal operation of the stream pipeline commences.
		/// 
		/// </para>
		/// <para>It is strongly recommended the spliterator report a characteristic of
		/// {@code IMMUTABLE} or {@code CONCURRENT}, or be
		/// <a href="../Spliterator.html#binding">late-binding</a>.  Otherwise,
		/// <seealso cref="#doubleStream(java.util.function.Supplier, int, boolean)"/> should
		/// be used to reduce the scope of potential interference with the source.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> A {@code Spliterator.OfDouble} describing the stream elements </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code DoubleStream} </returns>
		public static DoubleStream DoubleStream(java.util.Spliterator_OfDouble spliterator, bool parallel)
		{
			return new DoublePipeline.Head<>(spliterator, StreamOpFlag.fromCharacteristics(spliterator), parallel);
		}

		/// <summary>
		/// Creates a new sequential or parallel {@code DoubleStream} from a
		/// {@code Supplier} of {@code Spliterator.OfDouble}.
		/// 
		/// <para>The <seealso cref="Supplier#get()"/> method will be invoked on the supplier no
		/// more than once, and only after the terminal operation of the stream pipeline
		/// commences.
		/// 
		/// </para>
		/// <para>For spliterators that report a characteristic of {@code IMMUTABLE}
		/// or {@code CONCURRENT}, or that are
		/// <a href="../Spliterator.html#binding">late-binding</a>, it is likely
		/// more efficient to use <seealso cref="#doubleStream(java.util.Spliterator.OfDouble, boolean)"/>
		/// instead.
		/// </para>
		/// <para>The use of a {@code Supplier} in this form provides a level of
		/// indirection that reduces the scope of potential interference with the
		/// source.  Since the supplier is only invoked after the terminal operation
		/// commences, any modifications to the source up to the start of the
		/// terminal operation are reflected in the stream result.  See
		/// <a href="package-summary.html#NonInterference">Non-Interference</a> for
		/// more details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="supplier"> A {@code Supplier} of a {@code Spliterator.OfDouble} </param>
		/// <param name="characteristics"> Spliterator characteristics of the supplied
		///        {@code Spliterator.OfDouble}.  The characteristics must be equal to
		///        {@code supplier.get().characteristics()}, otherwise undefined
		///        behavior may occur when terminal operation commences. </param>
		/// <param name="parallel"> if {@code true} then the returned stream is a parallel
		///        stream; if {@code false} the returned stream is a sequential
		///        stream. </param>
		/// <returns> a new sequential or parallel {@code DoubleStream} </returns>
		/// <seealso cref= #doubleStream(java.util.Spliterator.OfDouble, boolean) </seealso>
		public static DoubleStream doubleStream<T1>(Supplier<T1> supplier, int characteristics, bool parallel) where T1 : java.util.Spliterator_OfDouble
		{
			return new DoublePipeline.Head<>(supplier, StreamOpFlag.fromCharacteristics(characteristics), parallel);
		}
	}

}