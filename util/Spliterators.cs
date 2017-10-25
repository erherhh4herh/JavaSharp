using System;

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
	/// Static classes and methods for operating on or creating instances of
	/// <seealso cref="Spliterator"/> and its primitive specializations
	/// <seealso cref="Spliterator.OfInt"/>, <seealso cref="Spliterator.OfLong"/>, and
	/// <seealso cref="Spliterator.OfDouble"/>.
	/// </summary>
	/// <seealso cref= Spliterator
	/// @since 1.8 </seealso>
	public sealed class Spliterators
	{

		// Suppresses default constructor, ensuring non-instantiability.
		private Spliterators()
		{
		}

		// Empty spliterators

		/// <summary>
		/// Creates an empty {@code Spliterator}
		/// 
		/// <para>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		/// <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <returns> An empty spliterator </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> Spliterator<T> emptySpliterator()
		public static Spliterator<T> emptySpliterator<T>()
		{
			return (Spliterator<T>) EMPTY_SPLITERATOR;
		}

		private static readonly Spliterator<Object> EMPTY_SPLITERATOR = new EmptySpliterator.OfRef<Object>();

		/// <summary>
		/// Creates an empty {@code Spliterator.OfInt}
		/// 
		/// <para>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		/// <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> An empty spliterator </returns>
		public static Spliterator_OfInt EmptyIntSpliterator()
		{
			return EMPTY_INT_SPLITERATOR;
		}

		private static readonly Spliterator_OfInt EMPTY_INT_SPLITERATOR = new EmptySpliterator.OfInt();

		/// <summary>
		/// Creates an empty {@code Spliterator.OfLong}
		/// 
		/// <para>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		/// <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> An empty spliterator </returns>
		public static Spliterator_OfLong EmptyLongSpliterator()
		{
			return EMPTY_LONG_SPLITERATOR;
		}

		private static readonly Spliterator_OfLong EMPTY_LONG_SPLITERATOR = new EmptySpliterator.OfLong();

		/// <summary>
		/// Creates an empty {@code Spliterator.OfDouble}
		/// 
		/// <para>The empty spliterator reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#SUBSIZED"/>.  Calls to
		/// <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> An empty spliterator </returns>
		public static Spliterator_OfDouble EmptyDoubleSpliterator()
		{
			return EMPTY_DOUBLE_SPLITERATOR;
		}

		private static readonly Spliterator_OfDouble EMPTY_DOUBLE_SPLITERATOR = new EmptySpliterator.OfDouble();

		// Array-based spliterators

		/// <summary>
		/// Creates a {@code Spliterator} covering the elements of a given array,
		/// using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(Object[])"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <seealso cref= Arrays#spliterator(Object[]) </seealso>
		public static Spliterator<T> spliterator<T>(Object[] array, int additionalCharacteristics)
		{
			return new ArraySpliterator<>(Objects.RequireNonNull(array), additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator} covering a range of elements of a given
		/// array, using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(Object[])"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="fromIndex"> The least index (inclusive) to cover </param>
		/// <param name="toIndex"> One past the greatest index to cover </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         {@code toIndex} is less than {@code fromIndex}, or
		///         {@code toIndex} is greater than the array size </exception>
		/// <seealso cref= Arrays#spliterator(Object[], int, int) </seealso>
		public static Spliterator<T> spliterator<T>(Object[] array, int fromIndex, int toIndex, int additionalCharacteristics)
		{
			CheckFromToBounds(Objects.RequireNonNull(array).length, fromIndex, toIndex);
			return new ArraySpliterator<>(array, fromIndex, toIndex, additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfInt} covering the elements of a given array,
		/// using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(int[])"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <seealso cref= Arrays#spliterator(int[]) </seealso>
		public static Spliterator_OfInt Spliterator(int[] array, int additionalCharacteristics)
		{
			return new IntArraySpliterator(Objects.RequireNonNull(array), additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfInt} covering a range of elements of a
		/// given array, using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(int[], int, int)"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="fromIndex"> The least index (inclusive) to cover </param>
		/// <param name="toIndex"> One past the greatest index to cover </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         {@code toIndex} is less than {@code fromIndex}, or
		///         {@code toIndex} is greater than the array size </exception>
		/// <seealso cref= Arrays#spliterator(int[], int, int) </seealso>
		public static Spliterator_OfInt Spliterator(int[] array, int fromIndex, int toIndex, int additionalCharacteristics)
		{
			CheckFromToBounds(Objects.RequireNonNull(array).length, fromIndex, toIndex);
			return new IntArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfLong} covering the elements of a given array,
		/// using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(long[])"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <seealso cref= Arrays#spliterator(long[]) </seealso>
		public static Spliterator_OfLong Spliterator(long[] array, int additionalCharacteristics)
		{
			return new LongArraySpliterator(Objects.RequireNonNull(array), additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfLong} covering a range of elements of a
		/// given array, using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(long[], int, int)"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report.  (For example, if it is
		/// known the array will not be further modified, specify {@code IMMUTABLE};
		/// if the array data is considered to have an an encounter order, specify
		/// {@code ORDERED}).  The method <seealso cref="Arrays#spliterator(long[], int, int)"/> can
		/// often be used instead, which returns a spliterator that reports
		/// {@code SIZED}, {@code SUBSIZED}, {@code IMMUTABLE}, and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="fromIndex"> The least index (inclusive) to cover </param>
		/// <param name="toIndex"> One past the greatest index to cover </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         {@code toIndex} is less than {@code fromIndex}, or
		///         {@code toIndex} is greater than the array size </exception>
		/// <seealso cref= Arrays#spliterator(long[], int, int) </seealso>
		public static Spliterator_OfLong Spliterator(long[] array, int fromIndex, int toIndex, int additionalCharacteristics)
		{
			CheckFromToBounds(Objects.RequireNonNull(array).length, fromIndex, toIndex);
			return new LongArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfDouble} covering the elements of a given array,
		/// using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(double[])"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report; it is common to
		/// additionally specify {@code IMMUTABLE} and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <seealso cref= Arrays#spliterator(double[]) </seealso>
		public static Spliterator_OfDouble Spliterator(double[] array, int additionalCharacteristics)
		{
			return new DoubleArraySpliterator(Objects.RequireNonNull(array), additionalCharacteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfDouble} covering a range of elements of a
		/// given array, using a customized set of spliterator characteristics.
		/// 
		/// <para>This method is provided as an implementation convenience for
		/// Spliterators which store portions of their elements in arrays, and need
		/// fine control over Spliterator characteristics.  Most other situations in
		/// which a Spliterator for an array is needed should use
		/// <seealso cref="Arrays#spliterator(double[], int, int)"/>.
		/// 
		/// </para>
		/// <para>The returned spliterator always reports the characteristics
		/// {@code SIZED} and {@code SUBSIZED}.  The caller may provide additional
		/// characteristics for the spliterator to report.  (For example, if it is
		/// known the array will not be further modified, specify {@code IMMUTABLE};
		/// if the array data is considered to have an an encounter order, specify
		/// {@code ORDERED}).  The method <seealso cref="Arrays#spliterator(long[], int, int)"/> can
		/// often be used instead, which returns a spliterator that reports
		/// {@code SIZED}, {@code SUBSIZED}, {@code IMMUTABLE}, and {@code ORDERED}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> The array, assumed to be unmodified during use </param>
		/// <param name="fromIndex"> The least index (inclusive) to cover </param>
		/// <param name="toIndex"> One past the greatest index to cover </param>
		/// <param name="additionalCharacteristics"> Additional spliterator characteristics
		///        of this spliterator's source or elements beyond {@code SIZED} and
		///        {@code SUBSIZED} which are are always reported </param>
		/// <returns> A spliterator for an array </returns>
		/// <exception cref="NullPointerException"> if the given array is {@code null} </exception>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         {@code toIndex} is less than {@code fromIndex}, or
		///         {@code toIndex} is greater than the array size </exception>
		/// <seealso cref= Arrays#spliterator(double[], int, int) </seealso>
		public static Spliterator_OfDouble Spliterator(double[] array, int fromIndex, int toIndex, int additionalCharacteristics)
		{
			CheckFromToBounds(Objects.RequireNonNull(array).length, fromIndex, toIndex);
			return new DoubleArraySpliterator(array, fromIndex, toIndex, additionalCharacteristics);
		}

		/// <summary>
		/// Validate inclusive start index and exclusive end index against the length
		/// of an array. </summary>
		/// <param name="arrayLength"> The length of the array </param>
		/// <param name="origin"> The inclusive start index </param>
		/// <param name="fence"> The exclusive end index </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if the start index is greater than
		/// the end index, if the start index is negative, or the end index is
		/// greater than the array length </exception>
		private static void CheckFromToBounds(int arrayLength, int origin, int fence)
		{
			if (origin > fence)
			{
				throw new ArrayIndexOutOfBoundsException("origin(" + origin + ") > fence(" + fence + ")");
			}
			if (origin < 0)
			{
				throw new ArrayIndexOutOfBoundsException(origin);
			}
			if (fence > arrayLength)
			{
				throw new ArrayIndexOutOfBoundsException(fence);
			}
		}

		// Iterator-based spliterators

		/// <summary>
		/// Creates a {@code Spliterator} using the given collection's
		/// <seealso cref="java.util.Collection#iterator()"/> as the source of elements, and
		/// reporting its <seealso cref="java.util.Collection#size()"/> as its initial size.
		/// 
		/// <para>The spliterator is
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the collection's iterator, and
		/// implements {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="c"> The collection </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source or
		///        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		///        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given collection is {@code null} </exception>
		public static Spliterator<T> spliterator<T, T1>(Collection<T1> c, int characteristics) where T1 : T
		{
			return new IteratorSpliterator<>(Objects.RequireNonNull(c), characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator} using a given {@code Iterator}
		/// as the source of elements, and with a given initially reported size.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned, or the initially reported
		/// size is not equal to the actual number of elements in the source.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="size"> The number of elements in the source, to be reported as
		///        initial {@code estimateSize} </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source or
		///        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		///        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator<T> spliterator<T, T1>(Iterator<T1> iterator, long size, int characteristics) where T1 : T
		{
			return new IteratorSpliterator<>(Objects.RequireNonNull(iterator), size, characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator} using a given {@code Iterator}
		/// as the source of elements, with no initial size estimate.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source
		///        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		///        ignored and are not reported.) </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator<T> spliteratorUnknownSize<T, T1>(Iterator<T1> iterator, int characteristics) where T1 : T
		{
			return new IteratorSpliterator<>(Objects.RequireNonNull(iterator), characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfInt} using a given
		/// {@code IntStream.IntIterator} as the source of elements, and with a given
		/// initially reported size.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned, or the initially reported
		/// size is not equal to the actual number of elements in the source.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="size"> The number of elements in the source, to be reported as
		///        initial {@code estimateSize}. </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source or
		///        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		///        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfInt Spliterator(PrimitiveIterator_OfInt iterator, long size, int characteristics)
		{
			return new IntIteratorSpliterator(Objects.RequireNonNull(iterator), size, characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfInt} using a given
		/// {@code IntStream.IntIterator} as the source of elements, with no initial
		/// size estimate.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source
		///        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		///        ignored and are not reported.) </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfInt SpliteratorUnknownSize(PrimitiveIterator_OfInt iterator, int characteristics)
		{
			return new IntIteratorSpliterator(Objects.RequireNonNull(iterator), characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfLong} using a given
		/// {@code LongStream.LongIterator} as the source of elements, and with a
		/// given initially reported size.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned, or the initially reported
		/// size is not equal to the actual number of elements in the source.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="size"> The number of elements in the source, to be reported as
		///        initial {@code estimateSize}. </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source or
		///        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		///        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfLong Spliterator(PrimitiveIterator_OfLong iterator, long size, int characteristics)
		{
			return new LongIteratorSpliterator(Objects.RequireNonNull(iterator), size, characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfLong} using a given
		/// {@code LongStream.LongIterator} as the source of elements, with no
		/// initial size estimate.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source
		///        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		///        ignored and are not reported.) </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfLong SpliteratorUnknownSize(PrimitiveIterator_OfLong iterator, int characteristics)
		{
			return new LongIteratorSpliterator(Objects.RequireNonNull(iterator), characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfDouble} using a given
		/// {@code DoubleStream.DoubleIterator} as the source of elements, and with a
		/// given initially reported size.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned, or the initially reported
		/// size is not equal to the actual number of elements in the source.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="size"> The number of elements in the source, to be reported as
		///        initial {@code estimateSize} </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source or
		///        elements.  The characteristics {@code SIZED} and {@code SUBSIZED}
		///        are additionally reported unless {@code CONCURRENT} is supplied. </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfDouble Spliterator(PrimitiveIterator_OfDouble iterator, long size, int characteristics)
		{
			return new DoubleIteratorSpliterator(Objects.RequireNonNull(iterator), size, characteristics);
		}

		/// <summary>
		/// Creates a {@code Spliterator.OfDouble} using a given
		/// {@code DoubleStream.DoubleIterator} as the source of elements, with no
		/// initial size estimate.
		/// 
		/// <para>The spliterator is not
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>, inherits
		/// the <em>fail-fast</em> properties of the iterator, and implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// </para>
		/// <para>Traversal of elements should be accomplished through the spliterator.
		/// The behaviour of splitting and traversal is undefined if the iterator is
		/// operated on after the spliterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="iterator"> The iterator for the source </param>
		/// <param name="characteristics"> Characteristics of this spliterator's source
		///        or elements ({@code SIZED} and {@code SUBSIZED}, if supplied, are
		///        ignored and are not reported.) </param>
		/// <returns> A spliterator from an iterator </returns>
		/// <exception cref="NullPointerException"> if the given iterator is {@code null} </exception>
		public static Spliterator_OfDouble SpliteratorUnknownSize(PrimitiveIterator_OfDouble iterator, int characteristics)
		{
			return new DoubleIteratorSpliterator(Objects.RequireNonNull(iterator), characteristics);
		}

		// Iterators from Spliterators

		/// <summary>
		/// Creates an {@code Iterator} from a {@code Spliterator}.
		/// 
		/// <para>Traversal of elements should be accomplished through the iterator.
		/// The behaviour of traversal is undefined if the spliterator is operated
		/// after the iterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> Type of elements </param>
		/// <param name="spliterator"> The spliterator </param>
		/// <returns> An iterator </returns>
		/// <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		public static Iterator<T> iterator<T, T1>(Spliterator<T1> spliterator) where T1 : T
		{
			Objects.RequireNonNull(spliterator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class Adapter implements Iterator<T>, java.util.function.Consumer<T>
	//		{
	//			boolean valueReady = false;
	//			T nextElement;
	//
	//			@@Override public void accept(T t)
	//			{
	//				valueReady = true;
	//				nextElement = t;
	//			}
	//
	//			@@Override public boolean hasNext()
	//			{
	//				if (!valueReady)
	//					spliterator.tryAdvance(this);
	//				return valueReady;
	//			}
	//
	//			@@Override public T next()
	//			{
	//				if (!valueReady && !hasNext())
	//					throw new NoSuchElementException();
	//				else
	//				{
	//					valueReady = false;
	//					return nextElement;
	//				}
	//			}
	//		}

			return new Adapter();
		}

		/// <summary>
		/// Creates an {@code PrimitiveIterator.OfInt} from a
		/// {@code Spliterator.OfInt}.
		/// 
		/// <para>Traversal of elements should be accomplished through the iterator.
		/// The behaviour of traversal is undefined if the spliterator is operated
		/// after the iterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> The spliterator </param>
		/// <returns> An iterator </returns>
		/// <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		public static PrimitiveIterator_OfInt Iterator(Spliterator_OfInt spliterator)
		{
			Objects.RequireNonNull(spliterator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class Adapter implements PrimitiveIterator_OfInt, java.util.function.IntConsumer
	//		{
	//			boolean valueReady = false;
	//			int nextElement;
	//
	//			@@Override public void accept(int t)
	//			{
	//				valueReady = true;
	//				nextElement = t;
	//			}
	//
	//			@@Override public boolean hasNext()
	//			{
	//				if (!valueReady)
	//					spliterator.tryAdvance(this);
	//				return valueReady;
	//			}
	//
	//			@@Override public int nextInt()
	//			{
	//				if (!valueReady && !hasNext())
	//					throw new NoSuchElementException();
	//				else
	//				{
	//					valueReady = false;
	//					return nextElement;
	//				}
	//			}
	//		}

			return new Adapter();
		}

		/// <summary>
		/// Creates an {@code PrimitiveIterator.OfLong} from a
		/// {@code Spliterator.OfLong}.
		/// 
		/// <para>Traversal of elements should be accomplished through the iterator.
		/// The behaviour of traversal is undefined if the spliterator is operated
		/// after the iterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> The spliterator </param>
		/// <returns> An iterator </returns>
		/// <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		public static PrimitiveIterator_OfLong Iterator(Spliterator_OfLong spliterator)
		{
			Objects.RequireNonNull(spliterator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class Adapter implements PrimitiveIterator_OfLong, java.util.function.LongConsumer
	//		{
	//			boolean valueReady = false;
	//			long nextElement;
	//
	//			@@Override public void accept(long t)
	//			{
	//				valueReady = true;
	//				nextElement = t;
	//			}
	//
	//			@@Override public boolean hasNext()
	//			{
	//				if (!valueReady)
	//					spliterator.tryAdvance(this);
	//				return valueReady;
	//			}
	//
	//			@@Override public long nextLong()
	//			{
	//				if (!valueReady && !hasNext())
	//					throw new NoSuchElementException();
	//				else
	//				{
	//					valueReady = false;
	//					return nextElement;
	//				}
	//			}
	//		}

			return new Adapter();
		}

		/// <summary>
		/// Creates an {@code PrimitiveIterator.OfDouble} from a
		/// {@code Spliterator.OfDouble}.
		/// 
		/// <para>Traversal of elements should be accomplished through the iterator.
		/// The behaviour of traversal is undefined if the spliterator is operated
		/// after the iterator is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="spliterator"> The spliterator </param>
		/// <returns> An iterator </returns>
		/// <exception cref="NullPointerException"> if the given spliterator is {@code null} </exception>
		public static PrimitiveIterator_OfDouble Iterator(Spliterator_OfDouble spliterator)
		{
			Objects.RequireNonNull(spliterator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class Adapter implements PrimitiveIterator_OfDouble, java.util.function.DoubleConsumer
	//		{
	//			boolean valueReady = false;
	//			double nextElement;
	//
	//			@@Override public void accept(double t)
	//			{
	//				valueReady = true;
	//				nextElement = t;
	//			}
	//
	//			@@Override public boolean hasNext()
	//			{
	//				if (!valueReady)
	//					spliterator.tryAdvance(this);
	//				return valueReady;
	//			}
	//
	//			@@Override public double nextDouble()
	//			{
	//				if (!valueReady && !hasNext())
	//					throw new NoSuchElementException();
	//				else
	//				{
	//					valueReady = false;
	//					return nextElement;
	//				}
	//			}
	//		}

			return new Adapter();
		}

		// Implementations

		private abstract class EmptySpliterator<T, S, C> where S : Spliterator<T>
		{

			internal EmptySpliterator()
			{
			}

			public virtual S TrySplit()
			{
				return null;
			}

			public virtual bool TryAdvance(C consumer)
			{
				Objects.RequireNonNull(consumer);
				return false;
			}

			public virtual void ForEachRemaining(C consumer)
			{
				Objects.RequireNonNull(consumer);
			}

			public virtual long EstimateSize()
			{
				return 0;
			}

			public virtual int Characteristics()
			{
				return Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static final class OfRef<T> extends EmptySpliterator<T, Spliterator<T>, java.util.function.Consumer<? base T>> implements Spliterator<T>
			private sealed class OfRef<T> : EmptySpliterator<T, Spliterator<T>, Consumer<JavaToDotNetGenericWildcard>>, Spliterator<T>
			{
				internal OfRef()
				{
				}
			}

			private sealed class OfInt : EmptySpliterator<Integer, Spliterator_OfInt, IntConsumer>, Spliterator_OfInt
			{
				internal OfInt()
				{
				}
			}

			private sealed class OfLong : EmptySpliterator<Long, Spliterator_OfLong, LongConsumer>, Spliterator_OfLong
			{
				internal OfLong()
				{
				}
			}

			private sealed class OfDouble : EmptySpliterator<Double, Spliterator_OfDouble, DoubleConsumer>, Spliterator_OfDouble
			{
				internal OfDouble()
				{
				}
			}
		}

		// Array-based spliterators

		/// <summary>
		/// A Spliterator designed for use by sources that traverse and split
		/// elements maintained in an unmodifiable {@code Object[]} array.
		/// </summary>
		internal sealed class ArraySpliterator<T> : Spliterator<T>
		{
			/// <summary>
			/// The array, explicitly typed as Object[]. Unlike in some other
			/// classes (see for example CR 6260652), we do not need to
			/// screen arguments to ensure they are exactly of type Object[]
			/// so long as no methods write into the array or serialize it,
			/// which we ensure here by defining this class as final.
			/// </summary>
			internal readonly Object[] Array;
			internal int Index; // current index, modified on advance/split
			internal readonly int Fence; // one past last index
			internal readonly int Characteristics_Renamed;

			/// <summary>
			/// Creates a spliterator covering all of the given array. </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			/// of this spliterator's source or elements beyond {@code SIZED} and
			/// {@code SUBSIZED} which are are always reported </param>
			public ArraySpliterator(Object[] array, int additionalCharacteristics) : this(array, 0, array.Length, additionalCharacteristics)
			{
			}

			/// <summary>
			/// Creates a spliterator covering the given array and range </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="origin"> the least index (inclusive) to cover </param>
			/// <param name="fence"> one past the greatest index to cover </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			/// of this spliterator's source or elements beyond {@code SIZED} and
			/// {@code SUBSIZED} which are are always reported </param>
			public ArraySpliterator(Object[] array, int origin, int fence, int additionalCharacteristics)
			{
				this.Array = array;
				this.Index = origin;
				this.Fence = fence;
				this.Characteristics_Renamed = additionalCharacteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}

			public override Spliterator<T> TrySplit()
			{
				int lo = Index, mid = (int)((uint)(lo + Fence) >> 1);
				return (lo >= mid) ? null : new ArraySpliterator<>(Array, lo, Index = mid, Characteristics_Renamed);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				Object[] a; // hoist accesses and checks from loop
				int i, hi;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((a = Array).Length >= (hi = Fence) && (i = Index) >= 0 && i < (Index = hi))
				{
					do
					{
						action.Accept((T)a[i]);
					} while (++i < hi);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
			public override bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Index >= 0 && Index < Fence)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T e = (T) array[index++];
					T e = (T) Array[Index++];
					action.Accept(e);
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return (long)(Fence - Index);
			}
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		/// <summary>
		/// A Spliterator.OfInt designed for use by sources that traverse and split
		/// elements maintained in an unmodifiable {@code int[]} array.
		/// </summary>
		internal sealed class IntArraySpliterator : Spliterator_OfInt
		{
			internal readonly int[] Array;
			internal int Index; // current index, modified on advance/split
			internal readonly int Fence; // one past last index
			internal readonly int Characteristics_Renamed;

			/// <summary>
			/// Creates a spliterator covering all of the given array. </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public IntArraySpliterator(int[] array, int additionalCharacteristics) : this(array, 0, array.Length, additionalCharacteristics)
			{
			}

			/// <summary>
			/// Creates a spliterator covering the given array and range </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="origin"> the least index (inclusive) to cover </param>
			/// <param name="fence"> one past the greatest index to cover </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public IntArraySpliterator(int[] array, int origin, int fence, int additionalCharacteristics)
			{
				this.Array = array;
				this.Index = origin;
				this.Fence = fence;
				this.Characteristics_Renamed = additionalCharacteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}

			public OfInt TrySplit()
			{
				int lo = Index, mid = (int)((uint)(lo + Fence) >> 1);
				return (lo >= mid) ? null : new IntArraySpliterator(Array, lo, Index = mid, Characteristics_Renamed);
			}

			public override void ForEachRemaining(IntConsumer action)
			{
				int[] a; // hoist accesses and checks from loop
				int i, hi;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((a = Array).Length >= (hi = Fence) && (i = Index) >= 0 && i < (Index = hi))
				{
					do
					{
						action.Accept(a[i]);
					} while (++i < hi);
				}
			}

			public bool TryAdvance(IntConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Index >= 0 && Index < Fence)
				{
					action.Accept(Array[Index++]);
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return (long)(Fence - Index);
			}
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Integer> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		/// <summary>
		/// A Spliterator.OfLong designed for use by sources that traverse and split
		/// elements maintained in an unmodifiable {@code int[]} array.
		/// </summary>
		internal sealed class LongArraySpliterator : Spliterator_OfLong
		{
			internal readonly long[] Array;
			internal int Index; // current index, modified on advance/split
			internal readonly int Fence; // one past last index
			internal readonly int Characteristics_Renamed;

			/// <summary>
			/// Creates a spliterator covering all of the given array. </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public LongArraySpliterator(long[] array, int additionalCharacteristics) : this(array, 0, array.Length, additionalCharacteristics)
			{
			}

			/// <summary>
			/// Creates a spliterator covering the given array and range </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="origin"> the least index (inclusive) to cover </param>
			/// <param name="fence"> one past the greatest index to cover </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public LongArraySpliterator(long[] array, int origin, int fence, int additionalCharacteristics)
			{
				this.Array = array;
				this.Index = origin;
				this.Fence = fence;
				this.Characteristics_Renamed = additionalCharacteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}

			public OfLong TrySplit()
			{
				int lo = Index, mid = (int)((uint)(lo + Fence) >> 1);
				return (lo >= mid) ? null : new LongArraySpliterator(Array, lo, Index = mid, Characteristics_Renamed);
			}

			public override void ForEachRemaining(LongConsumer action)
			{
				long[] a; // hoist accesses and checks from loop
				int i, hi;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((a = Array).Length >= (hi = Fence) && (i = Index) >= 0 && i < (Index = hi))
				{
					do
					{
						action.Accept(a[i]);
					} while (++i < hi);
				}
			}

			public bool TryAdvance(LongConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Index >= 0 && Index < Fence)
				{
					action.Accept(Array[Index++]);
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return (long)(Fence - Index);
			}
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Long> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		/// <summary>
		/// A Spliterator.OfDouble designed for use by sources that traverse and split
		/// elements maintained in an unmodifiable {@code int[]} array.
		/// </summary>
		internal sealed class DoubleArraySpliterator : Spliterator_OfDouble
		{
			internal readonly double[] Array;
			internal int Index; // current index, modified on advance/split
			internal readonly int Fence; // one past last index
			internal readonly int Characteristics_Renamed;

			/// <summary>
			/// Creates a spliterator covering all of the given array. </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public DoubleArraySpliterator(double[] array, int additionalCharacteristics) : this(array, 0, array.Length, additionalCharacteristics)
			{
			}

			/// <summary>
			/// Creates a spliterator covering the given array and range </summary>
			/// <param name="array"> the array, assumed to be unmodified during use </param>
			/// <param name="origin"> the least index (inclusive) to cover </param>
			/// <param name="fence"> one past the greatest index to cover </param>
			/// <param name="additionalCharacteristics"> Additional spliterator characteristics
			///        of this spliterator's source or elements beyond {@code SIZED} and
			///        {@code SUBSIZED} which are are always reported </param>
			public DoubleArraySpliterator(double[] array, int origin, int fence, int additionalCharacteristics)
			{
				this.Array = array;
				this.Index = origin;
				this.Fence = fence;
				this.Characteristics_Renamed = additionalCharacteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED;
			}

			public OfDouble TrySplit()
			{
				int lo = Index, mid = (int)((uint)(lo + Fence) >> 1);
				return (lo >= mid) ? null : new DoubleArraySpliterator(Array, lo, Index = mid, Characteristics_Renamed);
			}

			public override void ForEachRemaining(DoubleConsumer action)
			{
				double[] a; // hoist accesses and checks from loop
				int i, hi;
				if (action == null)
				{
					throw new NullPointerException();
				}
				if ((a = Array).Length >= (hi = Fence) && (i = Index) >= 0 && i < (Index = hi))
				{
					do
					{
						action.Accept(a[i]);
					} while (++i < hi);
				}
			}

			public bool TryAdvance(DoubleConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (Index >= 0 && Index < Fence)
				{
					action.Accept(Array[Index++]);
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return (long)(Fence - Index);
			}
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Double> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		//

		/// <summary>
		/// An abstract {@code Spliterator} that implements {@code trySplit} to
		/// permit limited parallelism.
		/// 
		/// <para>An extending class need only
		/// implement <seealso cref="#tryAdvance(java.util.function.Consumer) tryAdvance"/>.
		/// The extending class should override
		/// <seealso cref="#forEachRemaining(java.util.function.Consumer) forEach"/> if it can
		/// provide a more performant implementation.
		/// 
		/// @apiNote
		/// This class is a useful aid for creating a spliterator when it is not
		/// possible or difficult to efficiently partition elements in a manner
		/// allowing balanced parallel computation.
		/// 
		/// </para>
		/// <para>An alternative to using this class, that also permits limited
		/// parallelism, is to create a spliterator from an iterator
		/// (see <seealso cref="#spliterator(Iterator, long, int)"/>.  Depending on the
		/// circumstances using an iterator may be easier or more convenient than
		/// extending this class, such as when there is already an iterator
		/// available to use.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #spliterator(Iterator, long, int)
		/// @since 1.8 </seealso>
		public abstract class AbstractSpliterator<T> : Spliterator<T>
		{
			internal static readonly int BATCH_UNIT = 1 << 10; // batch array size increment
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator reporting the given estimated size and
			/// additionalCharacteristics.
			/// </summary>
			/// <param name="est"> the estimated size of this spliterator if known, otherwise
			///        {@code Long.MAX_VALUE}. </param>
			/// <param name="additionalCharacteristics"> properties of this spliterator's
			///        source or elements.  If {@code SIZED} is reported then this
			///        spliterator will additionally report {@code SUBSIZED}. </param>
			protected internal AbstractSpliterator(long est, int additionalCharacteristics)
			{
				this.Est = est;
				this.Characteristics_Renamed = ((additionalCharacteristics & Spliterator_Fields.SIZED) != 0) ? additionalCharacteristics | Spliterator_Fields.SUBSIZED : additionalCharacteristics;
			}

			internal sealed class HoldingConsumer<T> : Consumer<T>
			{
				internal Object Value;

				public override void Accept(T value)
				{
					this.Value = value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// This implementation permits limited parallelism.
			/// </summary>
			public override Spliterator<T> TrySplit()
			{
				/*
				 * Split into arrays of arithmetically increasing batch
				 * sizes.  This will only improve parallel performance if
				 * per-element Consumer actions are more costly than
				 * transferring them into an array.  The use of an
				 * arithmetic progression in split sizes provides overhead
				 * vs parallelism bounds that do not particularly favor or
				 * penalize cases of lightweight vs heavyweight element
				 * operations, across combinations of #elements vs #cores,
				 * whether or not either are known.  We generate
				 * O(sqrt(#elements)) splits, allowing O(sqrt(#cores))
				 * potential speedup.
				 */
				HoldingConsumer<T> holder = new HoldingConsumer<T>();
				long s = Est;
				if (s > 1 && tryAdvance(holder))
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					Object[] a = new Object[n];
					int j = 0;
					do
					{
						a[j] = holder.Value;
					} while (++j < n && tryAdvance(holder));
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new ArraySpliterator<>(a, 0, j, Characteristics());
				}
				return null;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the estimated size as reported when
			/// created and, if the estimate size is known, decreases in size when
			/// split.
			/// </summary>
			public override long EstimateSize()
			{
				return Est;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the characteristics as reported when
			/// created.
			/// </summary>
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
		}

		/// <summary>
		/// An abstract {@code Spliterator.OfInt} that implements {@code trySplit} to
		/// permit limited parallelism.
		/// 
		/// <para>To implement a spliterator an extending class need only
		/// implement <seealso cref="#tryAdvance(java.util.function.IntConsumer)"/>
		/// tryAdvance}.  The extending class should override
		/// <seealso cref="#forEachRemaining(java.util.function.IntConsumer)"/> forEach} if it
		/// can provide a more performant implementation.
		/// 
		/// @apiNote
		/// This class is a useful aid for creating a spliterator when it is not
		/// possible or difficult to efficiently partition elements in a manner
		/// allowing balanced parallel computation.
		/// 
		/// </para>
		/// <para>An alternative to using this class, that also permits limited
		/// parallelism, is to create a spliterator from an iterator
		/// (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfInt, long, int)"/>.
		/// Depending on the circumstances using an iterator may be easier or more
		/// convenient than extending this class. For example, if there is already an
		/// iterator available to use then there is no need to extend this class.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #spliterator(java.util.PrimitiveIterator.OfInt, long, int)
		/// @since 1.8 </seealso>
		public abstract class AbstractIntSpliterator : Spliterator_OfInt
		{
			public abstract bool TryAdvance(function.IntConsumer action);
			internal static readonly int MAX_BATCH = AbstractSpliterator.MAX_BATCH;
			internal static readonly int BATCH_UNIT = AbstractSpliterator.BATCH_UNIT;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator reporting the given estimated size and
			/// characteristics.
			/// </summary>
			/// <param name="est"> the estimated size of this spliterator if known, otherwise
			///        {@code Long.MAX_VALUE}. </param>
			/// <param name="additionalCharacteristics"> properties of this spliterator's
			///        source or elements.  If {@code SIZED} is reported then this
			///        spliterator will additionally report {@code SUBSIZED}. </param>
			protected internal AbstractIntSpliterator(long est, int additionalCharacteristics)
			{
				this.Est = est;
				this.Characteristics_Renamed = ((additionalCharacteristics & Spliterator_Fields.SIZED) != 0) ? additionalCharacteristics | Spliterator_Fields.SUBSIZED : additionalCharacteristics;
			}

			internal sealed class HoldingIntConsumer : IntConsumer
			{
				internal int Value;

				public void Accept(int value)
				{
					this.Value = value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// This implementation permits limited parallelism.
			/// </summary>
			public virtual Spliterator_OfInt TrySplit()
			{
				HoldingIntConsumer holder = new HoldingIntConsumer();
				long s = Est;
				if (s > 1 && TryAdvance(holder))
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					int[] a = new int[n];
					int j = 0;
					do
					{
						a[j] = holder.Value;
					} while (++j < n && TryAdvance(holder));
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new IntArraySpliterator(a, 0, j, Characteristics());
				}
				return null;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the estimated size as reported when
			/// created and, if the estimate size is known, decreases in size when
			/// split.
			/// </summary>
			public override long EstimateSize()
			{
				return Est;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the characteristics as reported when
			/// created.
			/// </summary>
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
		}

		/// <summary>
		/// An abstract {@code Spliterator.OfLong} that implements {@code trySplit}
		/// to permit limited parallelism.
		/// 
		/// <para>To implement a spliterator an extending class need only
		/// implement <seealso cref="#tryAdvance(java.util.function.LongConsumer)"/>
		/// tryAdvance}.  The extending class should override
		/// <seealso cref="#forEachRemaining(java.util.function.LongConsumer)"/> forEach} if it
		/// can provide a more performant implementation.
		/// 
		/// @apiNote
		/// This class is a useful aid for creating a spliterator when it is not
		/// possible or difficult to efficiently partition elements in a manner
		/// allowing balanced parallel computation.
		/// 
		/// </para>
		/// <para>An alternative to using this class, that also permits limited
		/// parallelism, is to create a spliterator from an iterator
		/// (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfLong, long, int)"/>.
		/// Depending on the circumstances using an iterator may be easier or more
		/// convenient than extending this class. For example, if there is already an
		/// iterator available to use then there is no need to extend this class.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #spliterator(java.util.PrimitiveIterator.OfLong, long, int)
		/// @since 1.8 </seealso>
		public abstract class AbstractLongSpliterator : Spliterator_OfLong
		{
			public abstract bool TryAdvance(function.LongConsumer action);
			internal static readonly int MAX_BATCH = AbstractSpliterator.MAX_BATCH;
			internal static readonly int BATCH_UNIT = AbstractSpliterator.BATCH_UNIT;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator reporting the given estimated size and
			/// characteristics.
			/// </summary>
			/// <param name="est"> the estimated size of this spliterator if known, otherwise
			///        {@code Long.MAX_VALUE}. </param>
			/// <param name="additionalCharacteristics"> properties of this spliterator's
			///        source or elements.  If {@code SIZED} is reported then this
			///        spliterator will additionally report {@code SUBSIZED}. </param>
			protected internal AbstractLongSpliterator(long est, int additionalCharacteristics)
			{
				this.Est = est;
				this.Characteristics_Renamed = ((additionalCharacteristics & Spliterator_Fields.SIZED) != 0) ? additionalCharacteristics | Spliterator_Fields.SUBSIZED : additionalCharacteristics;
			}

			internal sealed class HoldingLongConsumer : LongConsumer
			{
				internal long Value;

				public void Accept(long value)
				{
					this.Value = value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// This implementation permits limited parallelism.
			/// </summary>
			public virtual Spliterator_OfLong TrySplit()
			{
				HoldingLongConsumer holder = new HoldingLongConsumer();
				long s = Est;
				if (s > 1 && TryAdvance(holder))
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					long[] a = new long[n];
					int j = 0;
					do
					{
						a[j] = holder.Value;
					} while (++j < n && TryAdvance(holder));
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new LongArraySpliterator(a, 0, j, Characteristics());
				}
				return null;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the estimated size as reported when
			/// created and, if the estimate size is known, decreases in size when
			/// split.
			/// </summary>
			public override long EstimateSize()
			{
				return Est;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the characteristics as reported when
			/// created.
			/// </summary>
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
		}

		/// <summary>
		/// An abstract {@code Spliterator.OfDouble} that implements
		/// {@code trySplit} to permit limited parallelism.
		/// 
		/// <para>To implement a spliterator an extending class need only
		/// implement <seealso cref="#tryAdvance(java.util.function.DoubleConsumer)"/>
		/// tryAdvance}.  The extending class should override
		/// <seealso cref="#forEachRemaining(java.util.function.DoubleConsumer)"/> forEach} if
		/// it can provide a more performant implementation.
		/// 
		/// @apiNote
		/// This class is a useful aid for creating a spliterator when it is not
		/// possible or difficult to efficiently partition elements in a manner
		/// allowing balanced parallel computation.
		/// 
		/// </para>
		/// <para>An alternative to using this class, that also permits limited
		/// parallelism, is to create a spliterator from an iterator
		/// (see <seealso cref="#spliterator(java.util.PrimitiveIterator.OfDouble, long, int)"/>.
		/// Depending on the circumstances using an iterator may be easier or more
		/// convenient than extending this class. For example, if there is already an
		/// iterator available to use then there is no need to extend this class.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #spliterator(java.util.PrimitiveIterator.OfDouble, long, int)
		/// @since 1.8 </seealso>
		public abstract class AbstractDoubleSpliterator : Spliterator_OfDouble
		{
			public abstract bool TryAdvance(function.DoubleConsumer action);
			internal static readonly int MAX_BATCH = AbstractSpliterator.MAX_BATCH;
			internal static readonly int BATCH_UNIT = AbstractSpliterator.BATCH_UNIT;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator reporting the given estimated size and
			/// characteristics.
			/// </summary>
			/// <param name="est"> the estimated size of this spliterator if known, otherwise
			///        {@code Long.MAX_VALUE}. </param>
			/// <param name="additionalCharacteristics"> properties of this spliterator's
			///        source or elements.  If {@code SIZED} is reported then this
			///        spliterator will additionally report {@code SUBSIZED}. </param>
			protected internal AbstractDoubleSpliterator(long est, int additionalCharacteristics)
			{
				this.Est = est;
				this.Characteristics_Renamed = ((additionalCharacteristics & Spliterator_Fields.SIZED) != 0) ? additionalCharacteristics | Spliterator_Fields.SUBSIZED : additionalCharacteristics;
			}

			internal sealed class HoldingDoubleConsumer : DoubleConsumer
			{
				internal double Value;

				public void Accept(double value)
				{
					this.Value = value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// This implementation permits limited parallelism.
			/// </summary>
			public virtual Spliterator_OfDouble TrySplit()
			{
				HoldingDoubleConsumer holder = new HoldingDoubleConsumer();
				long s = Est;
				if (s > 1 && TryAdvance(holder))
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					double[] a = new double[n];
					int j = 0;
					do
					{
						a[j] = holder.Value;
					} while (++j < n && TryAdvance(holder));
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new DoubleArraySpliterator(a, 0, j, Characteristics());
				}
				return null;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the estimated size as reported when
			/// created and, if the estimate size is known, decreases in size when
			/// split.
			/// </summary>
			public override long EstimateSize()
			{
				return Est;
			}

			/// <summary>
			/// {@inheritDoc}
			/// 
			/// @implSpec
			/// This implementation returns the characteristics as reported when
			/// created.
			/// </summary>
			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
		}

		// Iterator-based Spliterators

		/// <summary>
		/// A Spliterator using a given Iterator for element
		/// operations. The spliterator implements {@code trySplit} to
		/// permit limited parallelism.
		/// </summary>
		internal class IteratorSpliterator<T> : Spliterator<T>
		{
			internal static readonly int BATCH_UNIT = 1 << 10; // batch array size increment
			internal static readonly int MAX_BATCH = 1 << 25; // max batch array size;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Collection<? extends T> collection;
			internal readonly Collection<?> Collection; // null OK
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Iterator<? extends T> it;
			internal Iterator<?> It;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator using the given given
			/// collection's {@link java.util.Collection#iterator()) for traversal,
			/// and reporting its {@link java.util.Collection#size()) as its initial
			/// size.
			/// </summary>
			/// <param name="c"> the collection </param>
			/// <param name="characteristics"> properties of this spliterator's
			///        source or elements. </param>
			public IteratorSpliterator<T1>(Collection<T1> collection, int characteristics) where T1 : T
			{
				this.Collection = collection;
				this.It = null;
				this.Characteristics_Renamed = (characteristics & Spliterator_Fields.CONCURRENT) == 0 ? characteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED : characteristics;
			}

			/// <summary>
			/// Creates a spliterator using the given iterator
			/// for traversal, and reporting the given initial size
			/// and characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="size"> the number of elements in the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public IteratorSpliterator<T1>(Iterator<T1> iterator, long size, int characteristics) where T1 : T
			{
				this.Collection = null;
				this.It = iterator;
				this.Est = size;
				this.Characteristics_Renamed = (characteristics & Spliterator_Fields.CONCURRENT) == 0 ? characteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED : characteristics;
			}

			/// <summary>
			/// Creates a spliterator using the given iterator
			/// for traversal, and reporting the given initial size
			/// and characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public IteratorSpliterator<T1>(Iterator<T1> iterator, int characteristics) where T1 : T
			{
				this.Collection = null;
				this.It = iterator;
				this.Est = Long.MaxValue;
				this.Characteristics_Renamed = characteristics & ~(Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED);
			}

			public override Spliterator<T> TrySplit()
			{
				/*
				 * Split into arrays of arithmetically increasing batch
				 * sizes.  This will only improve parallel performance if
				 * per-element Consumer actions are more costly than
				 * transferring them into an array.  The use of an
				 * arithmetic progression in split sizes provides overhead
				 * vs parallelism bounds that do not particularly favor or
				 * penalize cases of lightweight vs heavyweight element
				 * operations, across combinations of #elements vs #cores,
				 * whether or not either are known.  We generate
				 * O(sqrt(#elements)) splits, allowing O(sqrt(#cores))
				 * potential speedup.
				 */
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i;
				Iterator<?> i;
				long s;
				if ((i = It) == null)
				{
					i = It = Collection.Iterator();
					s = Est = (long) Collection.Size();
				}
				else
				{
					s = Est;
				}
				if (s > 1 && i.HasNext())
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					Object[] a = new Object[n];
					int j = 0;
					do
					{
						a[j] = i.Next();
					} while (++j < n && i.HasNext());
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new ArraySpliterator<>(a, 0, j, Characteristics_Renamed);
				}
				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends T> i;
				Iterator<?> i;
				if ((i = It) == null)
				{
					i = It = Collection.Iterator();
					Est = (long)Collection.Size();
				}
				i.forEachRemaining(action);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
			public override bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (It == null)
				{
					It = Collection.Iterator();
					Est = (long) Collection.Size();
				}
				if (It.HasNext())
				{
					action.Accept(It.Next());
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				if (It == null)
				{
					It = Collection.Iterator();
					return Est = (long)Collection.Size();
				}
				return Est;
			}

			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		/// <summary>
		/// A Spliterator.OfInt using a given IntStream.IntIterator for element
		/// operations. The spliterator implements {@code trySplit} to
		/// permit limited parallelism.
		/// </summary>
		internal sealed class IntIteratorSpliterator : Spliterator_OfInt
		{
			internal static readonly int BATCH_UNIT = IteratorSpliterator.BATCH_UNIT;
			internal static readonly int MAX_BATCH = IteratorSpliterator.MAX_BATCH;
			internal PrimitiveIterator_OfInt It;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator using the given iterator
			/// for traversal, and reporting the given initial size
			/// and characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="size"> the number of elements in the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public IntIteratorSpliterator(PrimitiveIterator_OfInt iterator, long size, int characteristics)
			{
				this.It = iterator;
				this.Est = size;
				this.Characteristics_Renamed = (characteristics & Spliterator_Fields.CONCURRENT) == 0 ? characteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED : characteristics;
			}

			/// <summary>
			/// Creates a spliterator using the given iterator for a
			/// source of unknown size, reporting the given
			/// characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public IntIteratorSpliterator(PrimitiveIterator_OfInt iterator, int characteristics)
			{
				this.It = iterator;
				this.Est = Long.MaxValue;
				this.Characteristics_Renamed = characteristics & ~(Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED);
			}

			public OfInt TrySplit()
			{
				PrimitiveIterator_OfInt i = It;
				long s = Est;
				if (s > 1 && i.HasNext())
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					int[] a = new int[n];
					int j = 0;
					do
					{
						a[j] = i.NextInt();
					} while (++j < n && i.HasNext());
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new IntArraySpliterator(a, 0, j, Characteristics_Renamed);
				}
				return null;
			}

			public override void ForEachRemaining(IntConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				It.ForEachRemaining(action);
			}

			public bool TryAdvance(IntConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (It.HasNext())
				{
					action.Accept(It.NextInt());
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return Est;
			}

			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Integer> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		internal sealed class LongIteratorSpliterator : Spliterator_OfLong
		{
			internal static readonly int BATCH_UNIT = IteratorSpliterator.BATCH_UNIT;
			internal static readonly int MAX_BATCH = IteratorSpliterator.MAX_BATCH;
			internal PrimitiveIterator_OfLong It;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator using the given iterator
			/// for traversal, and reporting the given initial size
			/// and characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="size"> the number of elements in the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public LongIteratorSpliterator(PrimitiveIterator_OfLong iterator, long size, int characteristics)
			{
				this.It = iterator;
				this.Est = size;
				this.Characteristics_Renamed = (characteristics & Spliterator_Fields.CONCURRENT) == 0 ? characteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED : characteristics;
			}

			/// <summary>
			/// Creates a spliterator using the given iterator for a
			/// source of unknown size, reporting the given
			/// characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public LongIteratorSpliterator(PrimitiveIterator_OfLong iterator, int characteristics)
			{
				this.It = iterator;
				this.Est = Long.MaxValue;
				this.Characteristics_Renamed = characteristics & ~(Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED);
			}

			public OfLong TrySplit()
			{
				PrimitiveIterator_OfLong i = It;
				long s = Est;
				if (s > 1 && i.HasNext())
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					long[] a = new long[n];
					int j = 0;
					do
					{
						a[j] = i.NextLong();
					} while (++j < n && i.HasNext());
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new LongArraySpliterator(a, 0, j, Characteristics_Renamed);
				}
				return null;
			}

			public override void ForEachRemaining(LongConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				It.ForEachRemaining(action);
			}

			public bool TryAdvance(LongConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (It.HasNext())
				{
					action.Accept(It.NextLong());
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return Est;
			}

			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Long> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}

		internal sealed class DoubleIteratorSpliterator : Spliterator_OfDouble
		{
			internal static readonly int BATCH_UNIT = IteratorSpliterator.BATCH_UNIT;
			internal static readonly int MAX_BATCH = IteratorSpliterator.MAX_BATCH;
			internal PrimitiveIterator_OfDouble It;
			internal readonly int Characteristics_Renamed;
			internal long Est; // size estimate
			internal int Batch; // batch size for splits

			/// <summary>
			/// Creates a spliterator using the given iterator
			/// for traversal, and reporting the given initial size
			/// and characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="size"> the number of elements in the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public DoubleIteratorSpliterator(PrimitiveIterator_OfDouble iterator, long size, int characteristics)
			{
				this.It = iterator;
				this.Est = size;
				this.Characteristics_Renamed = (characteristics & Spliterator_Fields.CONCURRENT) == 0 ? characteristics | Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED : characteristics;
			}

			/// <summary>
			/// Creates a spliterator using the given iterator for a
			/// source of unknown size, reporting the given
			/// characteristics.
			/// </summary>
			/// <param name="iterator"> the iterator for the source </param>
			/// <param name="characteristics"> properties of this spliterator's
			/// source or elements. </param>
			public DoubleIteratorSpliterator(PrimitiveIterator_OfDouble iterator, int characteristics)
			{
				this.It = iterator;
				this.Est = Long.MaxValue;
				this.Characteristics_Renamed = characteristics & ~(Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED);
			}

			public OfDouble TrySplit()
			{
				PrimitiveIterator_OfDouble i = It;
				long s = Est;
				if (s > 1 && i.HasNext())
				{
					int n = Batch + BATCH_UNIT;
					if (n > s)
					{
						n = (int) s;
					}
					if (n > MAX_BATCH)
					{
						n = MAX_BATCH;
					}
					double[] a = new double[n];
					int j = 0;
					do
					{
						a[j] = i.NextDouble();
					} while (++j < n && i.HasNext());
					Batch = j;
					if (Est != Long.MaxValue)
					{
						Est -= j;
					}
					return new DoubleArraySpliterator(a, 0, j, Characteristics_Renamed);
				}
				return null;
			}

			public override void ForEachRemaining(DoubleConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				It.ForEachRemaining(action);
			}

			public bool TryAdvance(DoubleConsumer action)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}
				if (It.HasNext())
				{
					action.Accept(It.NextDouble());
					return true;
				}
				return false;
			}

			public override long EstimateSize()
			{
				return Est;
			}

			public override int Characteristics()
			{
				return Characteristics_Renamed;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public Comparator<? base Double> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override Comparator<?> Comparator
			{
				get
				{
					if (hasCharacteristics(Spliterator_Fields.SORTED))
					{
						return null;
					}
					throw new IllegalStateException();
				}
			}
		}
	}

}