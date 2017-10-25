using System;
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
	/// Factory methods for transforming streams into sorted streams.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class SortedOps
	{

		private SortedOps()
		{
		}

		/// <summary>
		/// Appends a "sorted" operation to the provided stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		internal static Stream<T> makeRef<T, T1>(AbstractPipeline<T1> upstream)
		{
			return new OfRef<>(upstream);
		}

		/// <summary>
		/// Appends a "sorted" operation to the provided stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		/// <param name="comparator"> the comparator to order elements by </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: static <T> Stream<T> makeRef(AbstractPipeline<?, T, ?> upstream, java.util.Comparator<? base T> comparator)
		internal static Stream<T> makeRef<T, T1, T2>(AbstractPipeline<T1> upstream, IComparer<T2> comparator)
		{
			return new OfRef<>(upstream, comparator);
		}

		/// <summary>
		/// Appends a "sorted" operation to the provided stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		internal static IntStream makeInt<T, T1>(AbstractPipeline<T1> upstream)
		{
			return new OfInt(upstream);
		}

		/// <summary>
		/// Appends a "sorted" operation to the provided stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		internal static LongStream makeLong<T, T1>(AbstractPipeline<T1> upstream)
		{
			return new OfLong(upstream);
		}

		/// <summary>
		/// Appends a "sorted" operation to the provided stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		internal static DoubleStream makeDouble<T, T1>(AbstractPipeline<T1> upstream)
		{
			return new OfDouble(upstream);
		}

		/// <summary>
		/// Specialized subtype for sorting reference streams
		/// </summary>
		private sealed class OfRef<T> : ReferencePipeline.StatefulOp<T, T>
		{
			/// <summary>
			/// Comparator used for sorting
			/// </summary>
			internal readonly bool IsNaturalSort;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private final java.util.Comparator<? base T> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly IComparer<?> Comparator;

			/// <summary>
			/// Sort using natural order of {@literal <T>} which must be
			/// {@code Comparable}.
			/// </summary>
			internal OfRef<T1>(AbstractPipeline<T1> upstream) : base(upstream, StreamShape.REFERENCE, StreamOpFlag.IS_ORDERED | StreamOpFlag.IS_SORTED)
			{
				this.IsNaturalSort = Stream_Fields.True;
				// Will throw CCE when we try to sort if T is not Comparable
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Comparator<? base T> comp = (java.util.Comparator<? base T>) java.util.Comparator.naturalOrder();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> comp = (IComparer<?>) IComparer.naturalOrder();
				this.Comparator = comp;
			}

			/// <summary>
			/// Sort using the provided comparator.
			/// </summary>
			/// <param name="comparator"> The comparator to be used to evaluate ordering. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: OfRef(AbstractPipeline<?, T, ?> upstream, java.util.Comparator<? base T> comparator)
			internal OfRef<T1, T2>(AbstractPipeline<T1> upstream, IComparer<T2> comparator) : base(upstream, StreamShape.REFERENCE, StreamOpFlag.IS_ORDERED | StreamOpFlag.NOT_SORTED)
			{
				this.IsNaturalSort = false;
				this.Comparator = Objects.RequireNonNull(comparator);
			}

			public override Sink<T> OpWrapSink(int flags, Sink<T> sink)
			{
				Objects.RequireNonNull(sink);

				// If the input is already naturally sorted and this operation
				// also naturally sorted then this is a no-op
				if (StreamOpFlag.SORTED.isKnown(flags) && IsNaturalSort)
				{
					return sink;
				}
				else if (StreamOpFlag.SIZED.isKnown(flags))
				{
					return new SizedRefSortingSink<>(sink, Comparator);
				}
				else
				{
					return new RefSortingSink<>(sink, Comparator);
				}
			}

			public override Node<T> opEvaluateParallel<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator, IntFunction<T[]> generator)
			{
				// If the input is already naturally sorted and this operation
				// naturally sorts then collect the output
				if (StreamOpFlag.SORTED.isKnown(helper.StreamAndOpFlags) && IsNaturalSort)
				{
					return helper.Evaluate(spliterator, false, generator);
				}
				else
				{
					// @@@ Weak two-pass parallel implementation; parallel collect, parallel sort
					T[] flattenedData = helper.Evaluate(spliterator, Stream_Fields.True, generator).AsArray(generator);
					Arrays.ParallelSort(flattenedData, Comparator);
					return Nodes.Node(flattenedData);
				}
			}
		}

		/// <summary>
		/// Specialized subtype for sorting int streams.
		/// </summary>
		private sealed class OfInt : IntPipeline.StatefulOp<Integer>
		{
			internal OfInt<T1>(AbstractPipeline<T1> upstream) : base(upstream, StreamShape.INT_VALUE, StreamOpFlag.IS_ORDERED | StreamOpFlag.IS_SORTED)
			{
			}

			public override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				Objects.RequireNonNull(sink);

				if (StreamOpFlag.SORTED.isKnown(flags))
				{
					return sink;
				}
				else if (StreamOpFlag.SIZED.isKnown(flags))
				{
					return new SizedIntSortingSink(sink);
				}
				else
				{
					return new IntSortingSink(sink);
				}
			}

			public override Node<Integer> opEvaluateParallel<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator, IntFunction<Integer[]> generator)
			{
				if (StreamOpFlag.SORTED.isKnown(helper.StreamAndOpFlags))
				{
					return helper.Evaluate(spliterator, false, generator);
				}
				else
				{
					Node_OfInt n = (Node_OfInt) helper.Evaluate(spliterator, IntStream_Fields.True, generator);

					int[] content = n.AsPrimitiveArray();
					Arrays.ParallelSort(content);

					return Nodes.Node(content);
				}
			}
		}

		/// <summary>
		/// Specialized subtype for sorting long streams.
		/// </summary>
		private sealed class OfLong : LongPipeline.StatefulOp<Long>
		{
			internal OfLong<T1>(AbstractPipeline<T1> upstream) : base(upstream, StreamShape.LONG_VALUE, StreamOpFlag.IS_ORDERED | StreamOpFlag.IS_SORTED)
			{
			}

			public override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				Objects.RequireNonNull(sink);

				if (StreamOpFlag.SORTED.isKnown(flags))
				{
					return sink;
				}
				else if (StreamOpFlag.SIZED.isKnown(flags))
				{
					return new SizedLongSortingSink(sink);
				}
				else
				{
					return new LongSortingSink(sink);
				}
			}

			public override Node<Long> opEvaluateParallel<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator, IntFunction<Long[]> generator)
			{
				if (StreamOpFlag.SORTED.isKnown(helper.StreamAndOpFlags))
				{
					return helper.Evaluate(spliterator, false, generator);
				}
				else
				{
					Node_OfLong n = (Node_OfLong) helper.Evaluate(spliterator, LongStream_Fields.True, generator);

					long[] content = n.AsPrimitiveArray();
					Arrays.ParallelSort(content);

					return Nodes.Node(content);
				}
			}
		}

		/// <summary>
		/// Specialized subtype for sorting double streams.
		/// </summary>
		private sealed class OfDouble : DoublePipeline.StatefulOp<Double>
		{
			internal OfDouble<T1>(AbstractPipeline<T1> upstream) : base(upstream, StreamShape.DOUBLE_VALUE, StreamOpFlag.IS_ORDERED | StreamOpFlag.IS_SORTED)
			{
			}

			public override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				Objects.RequireNonNull(sink);

				if (StreamOpFlag.SORTED.isKnown(flags))
				{
					return sink;
				}
				else if (StreamOpFlag.SIZED.isKnown(flags))
				{
					return new SizedDoubleSortingSink(sink);
				}
				else
				{
					return new DoubleSortingSink(sink);
				}
			}

			public override Node<Double> opEvaluateParallel<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator, IntFunction<Double[]> generator)
			{
				if (StreamOpFlag.SORTED.isKnown(helper.StreamAndOpFlags))
				{
					return helper.Evaluate(spliterator, false, generator);
				}
				else
				{
					Node_OfDouble n = (Node_OfDouble) helper.Evaluate(spliterator, DoubleStream_Fields.True, generator);

					double[] content = n.AsPrimitiveArray();
					Arrays.ParallelSort(content);

					return Nodes.Node(content);
				}
			}
		}

		/// <summary>
		/// Abstract <seealso cref="Sink"/> for implementing sort on reference streams.
		/// 
		/// <para>
		/// Note: documentation below applies to reference and all primitive sinks.
		/// </para>
		/// <para>
		/// Sorting sinks first accept all elements, buffering then into an array
		/// or a re-sizable data structure, if the size of the pipeline is known or
		/// unknown respectively.  At the end of the sink protocol those elements are
		/// sorted and then pushed downstream.
		/// This class records if <seealso cref="#cancellationRequested"/> is called.  If so it
		/// can be inferred that the source pushing source elements into the pipeline
		/// knows that the pipeline is short-circuiting.  In such cases sub-classes
		/// pushing elements downstream will preserve the short-circuiting protocol
		/// by calling {@code downstream.cancellationRequested()} and checking the
		/// result is {@code false} before an element is pushed.
		/// </para>
		/// <para>
		/// Note that the above behaviour is an optimization for sorting with
		/// sequential streams.  It is not an error that more elements, than strictly
		/// required to produce a result, may flow through the pipeline.  This can
		/// occur, in general (not restricted to just sorting), for short-circuiting
		/// parallel pipelines.
		/// </para>
		/// </summary>
		private abstract class AbstractRefSortingSink<T> : Sink_ChainedReference<T, T>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: protected final java.util.Comparator<? base T> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			protected internal readonly IComparer<?> Comparator;
			// @@@ could be a lazy final value, if/when support is added
			protected internal bool CancellationWasRequested;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: AbstractRefSortingSink(Sink<? base T> downstream, java.util.Comparator<? base T> comparator)
			internal AbstractRefSortingSink<T1, T2>(Sink<T1> downstream, IComparer<T2> comparator) : base(downstream)
			{
				this.Comparator = comparator;
			}

			/// <summary>
			/// Records is cancellation is requested so short-circuiting behaviour
			/// can be preserved when the sorted elements are pushed downstream.
			/// </summary>
			/// <returns> false, as this sink never short-circuits. </returns>
			public override bool CancellationRequested()
			{
				CancellationWasRequested = true;
				return Sink_Fields.False;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on SIZED reference streams.
		/// </summary>
		private sealed class SizedRefSortingSink<T> : AbstractRefSortingSink<T>
		{
			internal T[] Array;
			internal int Offset;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SizedRefSortingSink(Sink<? base T> sink, java.util.Comparator<? base T> comparator)
			internal SizedRefSortingSink<T1, T2>(Sink<T1> sink, IComparer<T2> comparator) : base(sink, comparator)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void begin(long size)
			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				Array = (T[]) new Object[(int) size];
			}

			public override void End()
			{
				Arrays.Sort(Array, 0, Offset, Comparator);
				Downstream.begin(Offset);
				if (!CancellationWasRequested)
				{
					for (int i = 0; i < Offset; i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				else
				{
					for (int i = 0; i < Offset && !Downstream.cancellationRequested(); i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				Downstream.end();
				Array = null;
			}

			public override void Accept(T t)
			{
				Array[Offset++] = t;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on reference streams.
		/// </summary>
		private sealed class RefSortingSink<T> : AbstractRefSortingSink<T>
		{
			internal List<T> List;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: RefSortingSink(Sink<? base T> sink, java.util.Comparator<? base T> comparator)
			internal RefSortingSink<T1, T2>(Sink<T1> sink, IComparer<T2> comparator) : base(sink, comparator)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				List = (size >= 0) ? new List<T>((int) size) : new List<T>();
			}

			public override void End()
			{
				List.Sort(Comparator);
				Downstream.begin(List.Size());
				if (!CancellationWasRequested)
				{
					List.ForEach(Downstream::accept);
				}
				else
				{
					foreach (T t in List)
					{
						if (Downstream.cancellationRequested())
						{
							break;
						}
						Downstream.Accept(t);
					}
				}
				Downstream.end();
				List = null;
			}

			public override void Accept(T t)
			{
				List.Add(t);
			}
		}

		/// <summary>
		/// Abstract <seealso cref="Sink"/> for implementing sort on int streams.
		/// </summary>
		private abstract class AbstractIntSortingSink : Sink_ChainedInt<Integer>
		{
			protected internal bool CancellationWasRequested;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: AbstractIntSortingSink(Sink<? base Integer> downstream)
			internal AbstractIntSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override bool CancellationRequested()
			{
				CancellationWasRequested = true;
				return false;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on SIZED int streams.
		/// </summary>
		private sealed class SizedIntSortingSink : AbstractIntSortingSink
		{
			internal int[] Array;
			internal int Offset;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SizedIntSortingSink(Sink<? base Integer> downstream)
			internal SizedIntSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				Array = new int[(int) size];
			}

			public override void End()
			{
				Arrays.Sort(Array, 0, Offset);
				Downstream.begin(Offset);
				if (!CancellationWasRequested)
				{
					for (int i = 0; i < Offset; i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				else
				{
					for (int i = 0; i < Offset && !Downstream.cancellationRequested(); i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				Downstream.end();
				Array = null;
			}

			public override void Accept(int t)
			{
				Array[Offset++] = t;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on int streams.
		/// </summary>
		private sealed class IntSortingSink : AbstractIntSortingSink
		{
			internal SpinedBuffer.OfInt b;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: IntSortingSink(Sink<? base Integer> sink)
			internal IntSortingSink<T1>(Sink<T1> sink) : base(sink)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				b = (size > 0) ? new SpinedBuffer.OfInt((int) size) : new SpinedBuffer.OfInt();
			}

			public override void End()
			{
				int[] ints = b.AsPrimitiveArray();
				Arrays.Sort(ints);
				Downstream.begin(ints.Length);
				if (!CancellationWasRequested)
				{
					foreach (int anInt in ints)
					{
						Downstream.Accept(anInt);
					}
				}
				else
				{
					foreach (int anInt in ints)
					{
						if (Downstream.cancellationRequested())
						{
							break;
						}
						Downstream.Accept(anInt);
					}
				}
				Downstream.end();
			}

			public override void Accept(int t)
			{
				b.Accept(t);
			}
		}

		/// <summary>
		/// Abstract <seealso cref="Sink"/> for implementing sort on long streams.
		/// </summary>
		private abstract class AbstractLongSortingSink : Sink_ChainedLong<Long>
		{
			protected internal bool CancellationWasRequested;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: AbstractLongSortingSink(Sink<? base Long> downstream)
			internal AbstractLongSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override bool CancellationRequested()
			{
				CancellationWasRequested = true;
				return false;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on SIZED long streams.
		/// </summary>
		private sealed class SizedLongSortingSink : AbstractLongSortingSink
		{
			internal long[] Array;
			internal int Offset;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SizedLongSortingSink(Sink<? base Long> downstream)
			internal SizedLongSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				Array = new long[(int) size];
			}

			public override void End()
			{
				Arrays.Sort(Array, 0, Offset);
				Downstream.begin(Offset);
				if (!CancellationWasRequested)
				{
					for (int i = 0; i < Offset; i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				else
				{
					for (int i = 0; i < Offset && !Downstream.cancellationRequested(); i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				Downstream.end();
				Array = null;
			}

			public override void Accept(long t)
			{
				Array[Offset++] = t;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on long streams.
		/// </summary>
		private sealed class LongSortingSink : AbstractLongSortingSink
		{
			internal SpinedBuffer.OfLong b;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: LongSortingSink(Sink<? base Long> sink)
			internal LongSortingSink<T1>(Sink<T1> sink) : base(sink)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				b = (size > 0) ? new SpinedBuffer.OfLong((int) size) : new SpinedBuffer.OfLong();
			}

			public override void End()
			{
				long[] longs = b.AsPrimitiveArray();
				Arrays.Sort(longs);
				Downstream.begin(longs.Length);
				if (!CancellationWasRequested)
				{
					foreach (long aLong in longs)
					{
						Downstream.Accept(aLong);
					}
				}
				else
				{
					foreach (long aLong in longs)
					{
						if (Downstream.cancellationRequested())
						{
							break;
						}
						Downstream.Accept(aLong);
					}
				}
				Downstream.end();
			}

			public override void Accept(long t)
			{
				b.Accept(t);
			}
		}

		/// <summary>
		/// Abstract <seealso cref="Sink"/> for implementing sort on long streams.
		/// </summary>
		private abstract class AbstractDoubleSortingSink : Sink_ChainedDouble<Double>
		{
			protected internal bool CancellationWasRequested;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: AbstractDoubleSortingSink(Sink<? base Double> downstream)
			internal AbstractDoubleSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override bool CancellationRequested()
			{
				CancellationWasRequested = true;
				return false;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on SIZED double streams.
		/// </summary>
		private sealed class SizedDoubleSortingSink : AbstractDoubleSortingSink
		{
			internal double[] Array;
			internal int Offset;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SizedDoubleSortingSink(Sink<? base Double> downstream)
			internal SizedDoubleSortingSink<T1>(Sink<T1> downstream) : base(downstream)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				Array = new double[(int) size];
			}

			public override void End()
			{
				Arrays.Sort(Array, 0, Offset);
				Downstream.begin(Offset);
				if (!CancellationWasRequested)
				{
					for (int i = 0; i < Offset; i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				else
				{
					for (int i = 0; i < Offset && !Downstream.cancellationRequested(); i++)
					{
						Downstream.Accept(Array[i]);
					}
				}
				Downstream.end();
				Array = null;
			}

			public override void Accept(double t)
			{
				Array[Offset++] = t;
			}
		}

		/// <summary>
		/// <seealso cref="Sink"/> for implementing sort on double streams.
		/// </summary>
		private sealed class DoubleSortingSink : AbstractDoubleSortingSink
		{
			internal SpinedBuffer.OfDouble b;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: DoubleSortingSink(Sink<? base Double> sink)
			internal DoubleSortingSink<T1>(Sink<T1> sink) : base(sink)
			{
			}

			public override void Begin(long size)
			{
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				b = (size > 0) ? new SpinedBuffer.OfDouble((int) size) : new SpinedBuffer.OfDouble();
			}

			public override void End()
			{
				double[] doubles = b.AsPrimitiveArray();
				Arrays.Sort(doubles);
				Downstream.begin(doubles.Length);
				if (!CancellationWasRequested)
				{
					foreach (double aDouble in doubles)
					{
						Downstream.Accept(aDouble);
					}
				}
				else
				{
					foreach (double aDouble in doubles)
					{
						if (Downstream.cancellationRequested())
						{
							break;
						}
						Downstream.Accept(aDouble);
					}
				}
				Downstream.end();
			}

			public override void Accept(double t)
			{
				b.Accept(t);
			}
		}
	}

}