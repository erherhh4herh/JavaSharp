using System.Diagnostics;
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
	/// Utility methods for operating on and creating streams.
	/// 
	/// <para>Unless otherwise stated, streams are created as sequential streams.  A
	/// sequential stream can be transformed into a parallel stream by calling the
	/// {@code parallel()} method on the created stream.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	internal sealed class Streams
	{

		private Streams()
		{
			throw new Error("no instances");
		}

		/// <summary>
		/// An object instance representing no value, that cannot be an actual
		/// data element of a stream.  Used when processing streams that can contain
		/// {@code null} elements to distinguish between a {@code null} value and no
		/// value.
		/// </summary>
		internal static readonly Object NONE = new Object();

		/// <summary>
		/// An {@code int} range spliterator.
		/// </summary>
		internal sealed class RangeIntSpliterator : java.util.Spliterator_OfInt
		{
			// Can never be greater that upTo, this avoids overflow if upper bound
			// is Integer.MAX_VALUE
			// All elements are traversed if from == upTo & last == 0
			internal int From;
			internal readonly int UpTo;
			// 1 if the range is closed and the last element has not been traversed
			// Otherwise, 0 if the range is open, or is a closed range and all
			// elements have been traversed
			internal int Last;

			internal RangeIntSpliterator(int from, int upTo, bool closed) : this(from, upTo, closed ? 1 : 0)
			{
			}

			internal RangeIntSpliterator(int from, int upTo, int last)
			{
				this.From = from;
				this.UpTo = upTo;
				this.Last = last;
			}

			public bool TryAdvance(IntConsumer consumer)
			{
				Objects.RequireNonNull(consumer);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = from;
				int i = From;
				if (i < UpTo)
				{
					From++;
					consumer.Accept(i);
					return true;
				}
				else if (Last > 0)
				{
					Last = 0;
					consumer.Accept(i);
					return true;
				}
				return false;
			}

			public override void ForEachRemaining(IntConsumer consumer)
			{
				Objects.RequireNonNull(consumer);

				int i = From;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int hUpTo = upTo;
				int hUpTo = UpTo;
				int hLast = Last;
				From = UpTo;
				Last = 0;
				while (i < hUpTo)
				{
					consumer.Accept(i++);
				}
				if (hLast > 0)
				{
					// Last element of closed range
					consumer.Accept(i);
				}
			}

			public override long EstimateSize()
			{
				// Ensure ranges of size > Integer.MAX_VALUE report the correct size
				return ((long) UpTo) - From + Last;
			}

			public override int Characteristics()
			{
				return java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.SORTED;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base Integer> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					return null;
				}
			}

			public java.util.Spliterator_OfInt TrySplit()
			{
				long size = EstimateSize();
				return size <= 1 ? null : new RangeIntSpliterator(From, From = From + SplitPoint(size), 0);
					   // Left split always has a half-open range
			}

			/// <summary>
			/// The spliterator size below which the spliterator will be split
			/// at the mid-point to produce balanced splits. Above this size the
			/// spliterator will be split at a ratio of
			/// 1:(RIGHT_BALANCED_SPLIT_RATIO - 1)
			/// to produce right-balanced splits.
			/// 
			/// <para>Such splitting ensures that for very large ranges that the left
			/// side of the range will more likely be processed at a lower-depth
			/// than a balanced tree at the expense of a higher-depth for the right
			/// side of the range.
			/// 
			/// </para>
			/// <para>This is optimized for cases such as IntStream.ints() that is
			/// implemented as range of 0 to Integer.MAX_VALUE but is likely to be
			/// augmented with a limit operation that limits the number of elements
			/// to a count lower than this threshold.
			/// </para>
			/// </summary>
			internal static readonly int BALANCED_SPLIT_THRESHOLD = 1 << 24;

			/// <summary>
			/// The split ratio of the left and right split when the spliterator
			/// size is above BALANCED_SPLIT_THRESHOLD.
			/// </summary>
			internal static readonly int RIGHT_BALANCED_SPLIT_RATIO = 1 << 3;

			internal int SplitPoint(long size)
			{
				int d = (size < BALANCED_SPLIT_THRESHOLD) ? 2 : RIGHT_BALANCED_SPLIT_RATIO;
				// Cast to int is safe since:
				//   2 <= size < 2^32
				//   2 <= d <= 8
				return (int)(size / d);
			}
		}

		/// <summary>
		/// A {@code long} range spliterator.
		/// 
		/// This implementation cannot be used for ranges whose size is greater
		/// than Long.MAX_VALUE
		/// </summary>
		internal sealed class RangeLongSpliterator : java.util.Spliterator_OfLong
		{
			// Can never be greater that upTo, this avoids overflow if upper bound
			// is Long.MAX_VALUE
			// All elements are traversed if from == upTo & last == 0
			internal long From;
			internal readonly long UpTo;
			// 1 if the range is closed and the last element has not been traversed
			// Otherwise, 0 if the range is open, or is a closed range and all
			// elements have been traversed
			internal int Last;

			internal RangeLongSpliterator(long from, long upTo, bool closed) : this(from, upTo, closed ? 1 : 0)
			{
			}

			internal RangeLongSpliterator(long from, long upTo, int last)
			{
				Debug.Assert(upTo - from + last > 0);
				this.From = from;
				this.UpTo = upTo;
				this.Last = last;
			}

			public bool TryAdvance(LongConsumer consumer)
			{
				Objects.RequireNonNull(consumer);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long i = from;
				long i = From;
				if (i < UpTo)
				{
					From++;
					consumer.Accept(i);
					return true;
				}
				else if (Last > 0)
				{
					Last = 0;
					consumer.Accept(i);
					return true;
				}
				return false;
			}

			public override void ForEachRemaining(LongConsumer consumer)
			{
				Objects.RequireNonNull(consumer);

				long i = From;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long hUpTo = upTo;
				long hUpTo = UpTo;
				int hLast = Last;
				From = UpTo;
				Last = 0;
				while (i < hUpTo)
				{
					consumer.Accept(i++);
				}
				if (hLast > 0)
				{
					// Last element of closed range
					consumer.Accept(i);
				}
			}

			public override long EstimateSize()
			{
				return UpTo - From + Last;
			}

			public override int Characteristics()
			{
				return java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.NONNULL | java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.SORTED;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base Long> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					return null;
				}
			}

			public java.util.Spliterator_OfLong TrySplit()
			{
				long size = EstimateSize();
				return size <= 1 ? null : new RangeLongSpliterator(From, From = From + SplitPoint(size), 0);
					   // Left split always has a half-open range
			}

			/// <summary>
			/// The spliterator size below which the spliterator will be split
			/// at the mid-point to produce balanced splits. Above this size the
			/// spliterator will be split at a ratio of
			/// 1:(RIGHT_BALANCED_SPLIT_RATIO - 1)
			/// to produce right-balanced splits.
			/// 
			/// <para>Such splitting ensures that for very large ranges that the left
			/// side of the range will more likely be processed at a lower-depth
			/// than a balanced tree at the expense of a higher-depth for the right
			/// side of the range.
			/// 
			/// </para>
			/// <para>This is optimized for cases such as LongStream.longs() that is
			/// implemented as range of 0 to Long.MAX_VALUE but is likely to be
			/// augmented with a limit operation that limits the number of elements
			/// to a count lower than this threshold.
			/// </para>
			/// </summary>
			internal static readonly long BALANCED_SPLIT_THRESHOLD = 1 << 24;

			/// <summary>
			/// The split ratio of the left and right split when the spliterator
			/// size is above BALANCED_SPLIT_THRESHOLD.
			/// </summary>
			internal static readonly long RIGHT_BALANCED_SPLIT_RATIO = 1 << 3;

			internal long SplitPoint(long size)
			{
				long d = (size < BALANCED_SPLIT_THRESHOLD) ? 2 : RIGHT_BALANCED_SPLIT_RATIO;
				// 2 <= size <= Long.MAX_VALUE
				return size / d;
			}
		}

		private abstract class AbstractStreamBuilderImpl<T, S> : Spliterator<T> where S : java.util.Spliterator<T>
		{
			// >= 0 when building, < 0 when built
			// -1 == no elements
			// -2 == one element, held by first
			// -3 == two or more elements, held by buffer
			internal int Count;

			// Spliterator implementation for 0 or 1 element
			// count == -1 for no elements
			// count == -2 for one element held by first

			public override S TrySplit()
			{
				return null;
			}

			public override long EstimateSize()
			{
				return -Count - 1;
			}

			public override int Characteristics()
			{
				return java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.IMMUTABLE;
			}
		}

		internal sealed class StreamBuilderImpl<T> : AbstractStreamBuilderImpl<T, Spliterator<T>>, Stream_Builder<T>
		{
			// The first element in the stream
			// valid if count == 1
			internal T First;

			// The first and subsequent elements in the stream
			// non-null if count == 2
			internal SpinedBuffer<T> Buffer;

			/// <summary>
			/// Constructor for building a stream of 0 or more elements.
			/// </summary>
			internal StreamBuilderImpl()
			{
			}

			/// <summary>
			/// Constructor for a singleton stream.
			/// </summary>
			/// <param name="t"> the single element </param>
			internal StreamBuilderImpl(T t)
			{
				First = t;
				Count = -2;
			}

			// StreamBuilder implementation

			public override void Accept(T t)
			{
				if (Count == 0)
				{
					First = t;
					Count++;
				}
				else if (Count > 0)
				{
					if (Buffer == null)
					{
						Buffer = new SpinedBuffer<>();
						Buffer.Accept(First);
						Count++;
					}

					Buffer.Accept(t);
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			public Stream_Builder<T> Add(T t)
			{
				Accept(t);
				return Stream_Builder_Fields.this;
			}

			public override Stream<T> Build()
			{
				int c = Count;
				if (c >= 0)
				{
					// Switch count to negative value signalling the builder is built
					Count = -Count - 1;
					// Use this spliterator if 0 or 1 elements, otherwise use
					// the spliterator of the spined buffer
					return (c < 2) ? StreamSupport.Stream(Stream_Builder_Fields.this, false) : StreamSupport.Stream(Buffer.Spliterator(), false);
				}

				throw new IllegalStateException();
			}

			// Spliterator implementation for 0 or 1 element
			// count == -1 for no elements
			// count == -2 for one element held by first

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
			public override bool tryAdvance<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
					return true;
				}
				else
				{
					return false;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
				}
			}
		}

		internal sealed class IntStreamBuilderImpl : AbstractStreamBuilderImpl<Integer, java.util.Spliterator_OfInt>, IntStream_Builder, java.util.Spliterator_OfInt
		{
			// The first element in the stream
			// valid if count == 1
			internal int First;

			// The first and subsequent elements in the stream
			// non-null if count == 2
			internal SpinedBuffer.OfInt Buffer;

			/// <summary>
			/// Constructor for building a stream of 0 or more elements.
			/// </summary>
			internal IntStreamBuilderImpl()
			{
			}

			/// <summary>
			/// Constructor for a singleton stream.
			/// </summary>
			/// <param name="t"> the single element </param>
			internal IntStreamBuilderImpl(int t)
			{
				First = t;
				Count = -2;
			}

			// StreamBuilder implementation

			public void Accept(int t)
			{
				if (Count == 0)
				{
					First = t;
					Count++;
				}
				else if (Count > 0)
				{
					if (Buffer == null)
					{
						Buffer = new SpinedBuffer.OfInt();
						Buffer.Accept(First);
						Count++;
					}

					Buffer.Accept(t);
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			public IntStream Build()
			{
				int c = Count;
				if (c >= 0)
				{
					// Switch count to negative value signalling the builder is built
					Count = -Count - 1;
					// Use this spliterator if 0 or 1 elements, otherwise use
					// the spliterator of the spined buffer
					return (c < 2) ? StreamSupport.IntStream(IntStream_Builder_Fields.this, false) : StreamSupport.IntStream(Buffer.Spliterator(), false);
				}

				throw new IllegalStateException();
			}

			// Spliterator implementation for 0 or 1 element
			// count == -1 for no elements
			// count == -2 for one element held by first

			public bool TryAdvance(IntConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
					return true;
				}
				else
				{
					return false;
				}
			}

			public override void ForEachRemaining(IntConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
				}
			}
		}

		internal sealed class LongStreamBuilderImpl : AbstractStreamBuilderImpl<Long, java.util.Spliterator_OfLong>, LongStream_Builder, java.util.Spliterator_OfLong
		{
			// The first element in the stream
			// valid if count == 1
			internal long First;

			// The first and subsequent elements in the stream
			// non-null if count == 2
			internal SpinedBuffer.OfLong Buffer;

			/// <summary>
			/// Constructor for building a stream of 0 or more elements.
			/// </summary>
			internal LongStreamBuilderImpl()
			{
			}

			/// <summary>
			/// Constructor for a singleton stream.
			/// </summary>
			/// <param name="t"> the single element </param>
			internal LongStreamBuilderImpl(long t)
			{
				First = t;
				Count = -2;
			}

			// StreamBuilder implementation

			public void Accept(long t)
			{
				if (Count == 0)
				{
					First = t;
					Count++;
				}
				else if (Count > 0)
				{
					if (Buffer == null)
					{
						Buffer = new SpinedBuffer.OfLong();
						Buffer.Accept(First);
						Count++;
					}

					Buffer.Accept(t);
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			public LongStream Build()
			{
				int c = Count;
				if (c >= 0)
				{
					// Switch count to negative value signalling the builder is built
					Count = -Count - 1;
					// Use this spliterator if 0 or 1 elements, otherwise use
					// the spliterator of the spined buffer
					return (c < 2) ? StreamSupport.LongStream(LongStream_Builder_Fields.this, false) : StreamSupport.LongStream(Buffer.Spliterator(), false);
				}

				throw new IllegalStateException();
			}

			// Spliterator implementation for 0 or 1 element
			// count == -1 for no elements
			// count == -2 for one element held by first

			public bool TryAdvance(LongConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
					return true;
				}
				else
				{
					return false;
				}
			}

			public override void ForEachRemaining(LongConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
				}
			}
		}

		internal sealed class DoubleStreamBuilderImpl : AbstractStreamBuilderImpl<Double, java.util.Spliterator_OfDouble>, DoubleStream_Builder, java.util.Spliterator_OfDouble
		{
			// The first element in the stream
			// valid if count == 1
			internal double First;

			// The first and subsequent elements in the stream
			// non-null if count == 2
			internal SpinedBuffer.OfDouble Buffer;

			/// <summary>
			/// Constructor for building a stream of 0 or more elements.
			/// </summary>
			internal DoubleStreamBuilderImpl()
			{
			}

			/// <summary>
			/// Constructor for a singleton stream.
			/// </summary>
			/// <param name="t"> the single element </param>
			internal DoubleStreamBuilderImpl(double t)
			{
				First = t;
				Count = -2;
			}

			// StreamBuilder implementation

			public void Accept(double t)
			{
				if (Count == 0)
				{
					First = t;
					Count++;
				}
				else if (Count > 0)
				{
					if (Buffer == null)
					{
						Buffer = new SpinedBuffer.OfDouble();
						Buffer.Accept(First);
						Count++;
					}

					Buffer.Accept(t);
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			public DoubleStream Build()
			{
				int c = Count;
				if (c >= 0)
				{
					// Switch count to negative value signalling the builder is built
					Count = -Count - 1;
					// Use this spliterator if 0 or 1 elements, otherwise use
					// the spliterator of the spined buffer
					return (c < 2) ? StreamSupport.DoubleStream(DoubleStream_Builder_Fields.this, false) : StreamSupport.DoubleStream(Buffer.Spliterator(), false);
				}

				throw new IllegalStateException();
			}

			// Spliterator implementation for 0 or 1 element
			// count == -1 for no elements
			// count == -2 for one element held by first

			public bool TryAdvance(DoubleConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
					return true;
				}
				else
				{
					return false;
				}
			}

			public override void ForEachRemaining(DoubleConsumer action)
			{
				Objects.RequireNonNull(action);

				if (Count == -2)
				{
					action.Accept(First);
					Count = -1;
				}
			}
		}

		internal abstract class ConcatSpliterator<T, T_SPLITR> : Spliterator<T> where T_SPLITR : java.util.Spliterator<T>
		{
			protected internal readonly T_SPLITR ASpliterator;
			protected internal readonly T_SPLITR BSpliterator;
			// True when no split has occurred, otherwise false
			internal bool BeforeSplit;
			// Never read after splitting
			internal readonly bool Unsized;

			public ConcatSpliterator(T_SPLITR aSpliterator, T_SPLITR bSpliterator)
			{
				this.ASpliterator = aSpliterator;
				this.BSpliterator = bSpliterator;
				BeforeSplit = true;
				// The spliterator is known to be unsized before splitting if the
				// sum of the estimates overflows.
				Unsized = aSpliterator.estimateSize() + bSpliterator.estimateSize() < 0;
			}

			public override T_SPLITR TrySplit()
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_SPLITR ret = beforeSplit ? aSpliterator : (T_SPLITR) bSpliterator.trySplit();
				T_SPLITR ret = BeforeSplit ? ASpliterator : (T_SPLITR) BSpliterator.trySplit();
				BeforeSplit = false;
				return ret;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> consumer)
			public override bool tryAdvance<T1>(Consumer<T1> consumer)
			{
				bool hasNext;
				if (BeforeSplit)
				{
					hasNext = ASpliterator.tryAdvance(consumer);
					if (!hasNext)
					{
						BeforeSplit = false;
						hasNext = BSpliterator.tryAdvance(consumer);
					}
				}
				else
				{
					hasNext = BSpliterator.tryAdvance(consumer);
				}
				return hasNext;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> consumer)
			public override void forEachRemaining<T1>(Consumer<T1> consumer)
			{
				if (BeforeSplit)
				{
					ASpliterator.forEachRemaining(consumer);
				}
				BSpliterator.forEachRemaining(consumer);
			}

			public override long EstimateSize()
			{
				if (BeforeSplit)
				{
					// If one or both estimates are Long.MAX_VALUE then the sum
					// will either be Long.MAX_VALUE or overflow to a negative value
					long size = ASpliterator.estimateSize() + BSpliterator.estimateSize();
					return (size >= 0) ? size : Long.MaxValue;
				}
				else
				{
					return BSpliterator.estimateSize();
				}
			}

			public override int Characteristics()
			{
				if (BeforeSplit)
				{
					// Concatenation loses DISTINCT and SORTED characteristics
					return ASpliterator.characteristics() & BSpliterator.characteristics() & ~(java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.SORTED | (Unsized ? java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED : 0));
				}
				else
				{
					return BSpliterator.characteristics();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					if (BeforeSplit)
					{
						throw new IllegalStateException();
					}
					return BSpliterator.Comparator;
				}
			}

			internal class OfRef<T> : ConcatSpliterator<T, Spliterator<T>>
			{
				internal OfRef(Spliterator<T> aSpliterator, Spliterator<T> bSpliterator) : base(aSpliterator, bSpliterator)
				{
				}
			}

			private abstract class OfPrimitive<T, T_CONS, T_SPLITR> : ConcatSpliterator<T, T_SPLITR>, java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>
			{
				internal OfPrimitive(T_SPLITR aSpliterator, T_SPLITR bSpliterator) : base(aSpliterator, bSpliterator)
				{
				}

				public override bool TryAdvance(T_CONS action)
				{
					bool hasNext;
					if (outerInstance.BeforeSplit)
					{
						hasNext = outerInstance.ASpliterator.tryAdvance(action);
						if (!hasNext)
						{
							outerInstance.BeforeSplit = false;
							hasNext = outerInstance.BSpliterator.tryAdvance(action);
						}
					}
					else
					{
						hasNext = outerInstance.BSpliterator.tryAdvance(action);
					}
					return hasNext;
				}

				public override void ForEachRemaining(T_CONS action)
				{
					if (outerInstance.BeforeSplit)
					{
						outerInstance.ASpliterator.forEachRemaining(action);
					}
					outerInstance.BSpliterator.forEachRemaining(action);
				}
			}

			internal class OfInt : ConcatSpliterator.OfPrimitive<Integer, IntConsumer, java.util.Spliterator_OfInt>, java.util.Spliterator_OfInt
			{
				internal OfInt(java.util.Spliterator_OfInt aSpliterator, java.util.Spliterator_OfInt bSpliterator) : base(aSpliterator, bSpliterator)
				{
				}
			}

			internal class OfLong : ConcatSpliterator.OfPrimitive<Long, LongConsumer, java.util.Spliterator_OfLong>, java.util.Spliterator_OfLong
			{
				internal OfLong(java.util.Spliterator_OfLong aSpliterator, java.util.Spliterator_OfLong bSpliterator) : base(aSpliterator, bSpliterator)
				{
				}
			}

			internal class OfDouble : ConcatSpliterator.OfPrimitive<Double, DoubleConsumer, java.util.Spliterator_OfDouble>, java.util.Spliterator_OfDouble
			{
				internal OfDouble(java.util.Spliterator_OfDouble aSpliterator, java.util.Spliterator_OfDouble bSpliterator) : base(aSpliterator, bSpliterator)
				{
				}
			}
		}

		/// <summary>
		/// Given two Runnables, return a Runnable that executes both in sequence,
		/// even if the first throws an exception, and if both throw exceptions, add
		/// any exceptions thrown by the second as suppressed exceptions of the first.
		/// </summary>
		internal static Runnable ComposeWithExceptions(Runnable a, Runnable b)
		{
			return new RunnableAnonymousInnerClassHelper(a, b);
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private java.lang.Runnable a;
			private java.lang.Runnable b;

			public RunnableAnonymousInnerClassHelper(java.lang.Runnable a, java.lang.Runnable b)
			{
				this.a = a;
				this.b = b;
			}

			public virtual void Run()
			{
				try
				{
					a.Run();
				}
				catch (Throwable e1)
				{
					try
					{
						b.Run();
					}
					catch (Throwable e2)
					{
						try
						{
							e1.AddSuppressed(e2);
						}
						catch (Throwable)
						{
						}
					}
					throw e1;
				}
				b.Run();
			}
		}

		/// <summary>
		/// Given two streams, return a Runnable that
		/// executes both of their <seealso cref="BaseStream#close"/> methods in sequence,
		/// even if the first throws an exception, and if both throw exceptions, add
		/// any exceptions thrown by the second as suppressed exceptions of the first.
		/// </summary>
		internal static Runnable composedClose<T1, T2>(BaseStream<T1> a, BaseStream<T2> b)
		{
			return new RunnableAnonymousInnerClassHelper2(a, b);
		}

		private class RunnableAnonymousInnerClassHelper2 : Runnable
		{
			private java.util.stream.BaseStream<T1> a;
			private java.util.stream.BaseStream<T2> b;

			public RunnableAnonymousInnerClassHelper2(java.util.stream.BaseStream<T1> a, java.util.stream.BaseStream<T2> b)
			{
				this.a = a;
				this.b = b;
			}

			public virtual void Run()
			{
				try
				{
					a.Close();
				}
				catch (Throwable e1)
				{
					try
					{
						b.Close();
					}
					catch (Throwable e2)
					{
						try
						{
							e1.AddSuppressed(e2);
						}
						catch (Throwable)
						{
						}
					}
					throw e1;
				}
				b.Close();
			}
		}
	}

}