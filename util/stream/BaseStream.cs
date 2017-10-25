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
	/// Base interface for streams, which are sequences of elements supporting
	/// sequential and parallel aggregate operations.  The following example
	/// illustrates an aggregate operation using the stream types <seealso cref="Stream"/>
	/// and <seealso cref="IntStream"/>, computing the sum of the weights of the red widgets:
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
	/// parallelism, which governs the behavior of all stream types.
	/// </summary>
	/// @param <T> the type of the stream elements </param>
	/// @param <S> the type of of the stream implementing {@code BaseStream}
	/// @since 1.8 </param>
	/// <seealso cref= Stream </seealso>
	/// <seealso cref= IntStream </seealso>
	/// <seealso cref= LongStream </seealso>
	/// <seealso cref= DoubleStream </seealso>
	/// <seealso cref= <a href="package-summary.html">java.util.stream</a> </seealso>
	public interface BaseStream<T, S> : AutoCloseable where S : BaseStream<T, S>
	{
		/// <summary>
		/// Returns an iterator for the elements of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the element iterator for this stream </returns>
		IEnumerator<T> Iterator();

		/// <summary>
		/// Returns a spliterator for the elements of this stream.
		/// 
		/// <para>This is a <a href="package-summary.html#StreamOps">terminal
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the element spliterator for this stream </returns>
		Spliterator<T> Spliterator();

		/// <summary>
		/// Returns whether this stream, if a terminal operation were to be executed,
		/// would execute in parallel.  Calling this method after invoking an
		/// terminal stream operation method may yield unpredictable results.
		/// </summary>
		/// <returns> {@code true} if this stream would execute in parallel if executed </returns>
		bool Parallel {get;}

		/// <summary>
		/// Returns an equivalent stream that is sequential.  May return
		/// itself, either because the stream was already sequential, or because
		/// the underlying stream state was modified to be sequential.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a sequential stream </returns>
		S Sequential();

		/// <summary>
		/// Returns an equivalent stream that is parallel.  May return
		/// itself, either because the stream was already parallel, or because
		/// the underlying stream state was modified to be parallel.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a parallel stream </returns>
		S Parallel();

		/// <summary>
		/// Returns an equivalent stream that is
		/// <a href="package-summary.html#Ordering">unordered</a>.  May return
		/// itself, either because the stream was already unordered, or because
		/// the underlying stream state was modified to be unordered.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an unordered stream </returns>
		S Unordered();

		/// <summary>
		/// Returns an equivalent stream with an additional close handler.  Close
		/// handlers are run when the <seealso cref="#close()"/> method
		/// is called on the stream, and are executed in the order they were
		/// added.  All close handlers are run, even if earlier close handlers throw
		/// exceptions.  If any close handler throws an exception, the first
		/// exception thrown will be relayed to the caller of {@code close()}, with
		/// any remaining exceptions added to that exception as suppressed exceptions
		/// (unless one of the remaining exceptions is the same exception as the
		/// first exception, since an exception cannot suppress itself.)  May
		/// return itself.
		/// 
		/// <para>This is an <a href="package-summary.html#StreamOps">intermediate
		/// operation</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="closeHandler"> A task to execute when the stream is closed </param>
		/// <returns> a stream with a handler that is run if the stream is closed </returns>
		S OnClose(Runnable closeHandler);

		/// <summary>
		/// Closes this stream, causing all close handlers for this stream pipeline
		/// to be called.
		/// </summary>
		/// <seealso cref= AutoCloseable#close() </seealso>
		void Close();
	}

}