using System;
using System.Diagnostics;

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
	/// Factory for instances of a short-circuiting stateful intermediate operations
	/// that produce subsequences of their input stream.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class SliceOps
	{

		// No instances
		private SliceOps()
		{
		}

		/// <summary>
		/// Calculates the sliced size given the current size, number of elements
		/// skip, and the number of elements to limit.
		/// </summary>
		/// <param name="size"> the current size </param>
		/// <param name="skip"> the number of elements to skip, assumed to be >= 0 </param>
		/// <param name="limit"> the number of elements to limit, assumed to be >= 0, with
		///        a value of {@code Long.MAX_VALUE} if there is no limit </param>
		/// <returns> the sliced size </returns>
		private static long CalcSize(long size, long skip, long limit)
		{
			return size >= 0 ? System.Math.Max(-1, System.Math.Min(size - skip, limit)) : -1;
		}

		/// <summary>
		/// Calculates the slice fence, which is one past the index of the slice
		/// range </summary>
		/// <param name="skip"> the number of elements to skip, assumed to be >= 0 </param>
		/// <param name="limit"> the number of elements to limit, assumed to be >= 0, with
		///        a value of {@code Long.MAX_VALUE} if there is no limit </param>
		/// <returns> the slice fence. </returns>
		private static long CalcSliceFence(long skip, long limit)
		{
			long sliceFence = limit >= 0 ? skip + limit : Long.MaxValue;
			// Check for overflow
			return (sliceFence >= 0) ? sliceFence : Long.MaxValue;
		}

		/// <summary>
		/// Creates a slice spliterator given a stream shape governing the
		/// spliterator type.  Requires that the underlying Spliterator
		/// be SUBSIZED.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <P_IN> java.util.Spliterator<P_IN> sliceSpliterator(StreamShape shape, java.util.Spliterator<P_IN> s, long skip, long limit)
		private static Spliterator<P_IN> sliceSpliterator<P_IN>(StreamShape shape, Spliterator<P_IN> s, long skip, long limit)
		{
			Debug.Assert(s.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED));
			long sliceFence = CalcSliceFence(skip, limit);
			switch (shape)
			{
				case java.util.stream.StreamShape.REFERENCE:
					return new StreamSpliterators.SliceSpliterator.OfRef<>(s, skip, sliceFence);
				case java.util.stream.StreamShape.INT_VALUE:
					return (Spliterator<P_IN>) new StreamSpliterators.SliceSpliterator.OfInt((java.util.Spliterator_OfInt) s, skip, sliceFence);
				case java.util.stream.StreamShape.LONG_VALUE:
					return (Spliterator<P_IN>) new StreamSpliterators.SliceSpliterator.OfLong((java.util.Spliterator_OfLong) s, skip, sliceFence);
				case java.util.stream.StreamShape.DOUBLE_VALUE:
					return (Spliterator<P_IN>) new StreamSpliterators.SliceSpliterator.OfDouble((java.util.Spliterator_OfDouble) s, skip, sliceFence);
				default:
					throw new IllegalStateException("Unknown shape " + shape);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> java.util.function.IntFunction<T[]> castingArray()
		private static IntFunction<T[]> castingArray<T>()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return size => (T[]) new Object[size];
		}

		/// <summary>
		/// Appends a "slice" operation to the provided stream.  The slice operation
		/// may be may be skip-only, limit-only, or skip-and-limit.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		/// <param name="skip"> the number of elements to skip.  Must be >= 0. </param>
		/// <param name="limit"> the maximum size of the resulting stream, or -1 if no limit
		///        is to be imposed </param>
		public static Stream<T> makeRef<T, T1>(AbstractPipeline<T1> upstream, long skip, long limit)
		{
			if (skip < 0)
			{
				throw new IllegalArgumentException("Skip must be non-negative: " + skip);
			}

			return new StatefulOpAnonymousInnerClassHelper(upstream, Flags(limit), skip, limit);
		}

		private class StatefulOpAnonymousInnerClassHelper : ReferencePipeline.StatefulOp<T, T>
		{
			private long Skip;
			private long Limit;

			public StatefulOpAnonymousInnerClassHelper(java.util.stream.AbstractPipeline<T1> upstream, int flags, long skip, long limit) : base(upstream, StreamShape.REFERENCE, flags)
			{
				this.Skip = skip;
				this.Limit = limit;
			}

			internal virtual Spliterator<T> UnorderedSkipLimitSpliterator(Spliterator<T> s, long skip, long limit, long sizeIfKnown)
			{
				if (skip <= sizeIfKnown)
				{
					// Use just the limit if the number of elements
					// to skip is <= the known pipeline size
					limit = limit >= 0 ? System.Math.Min(limit, sizeIfKnown - skip) : sizeIfKnown - skip;
					skip = 0;
				}
				return new StreamSpliterators.UnorderedSliceSpliterator.OfRef<>(s, skip, limit);
			}

			internal override Spliterator<T> opEvaluateParallelLazy<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					return new StreamSpliterators.SliceSpliterator.OfRef<>(helper.WrapSpliterator(spliterator), Skip, CalcSliceFence(Skip, Limit));
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					return unorderedSkipLimitSpliterator(helper.WrapSpliterator(spliterator), Skip, Limit, size);
				}
				else
				{
					// @@@ OOMEs will occur for LongStream.longs().filter(i -> true).limit(n)
					//     regardless of the value of n
					//     Need to adjust the target size of splitting for the
					//     SliceTask from say (size / k) to say min(size / k, 1 << 14)
					//     This will limit the size of the buffers created at the leaf nodes
					//     cancellation will be more aggressive cancelling later tasks
					//     if the target slice size has been reached from a given task,
					//     cancellation should also clear local results if any
					return (new SliceTask<>(this, helper, spliterator, CastingArray(), Skip, Limit)).invoke().spliterator();
				}
			}

			internal override Node<T> opEvaluateParallel<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator, IntFunction<T[]> generator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					// Because the pipeline is SIZED the slice spliterator
					// can be created from the source, this requires matching
					// to shape of the source, and is potentially more efficient
					// than creating the slice spliterator from the pipeline
					// wrapping spliterator
					Spliterator<P_IN> s = SliceSpliterator(helper.SourceShape, spliterator, Skip, Limit);
					return Nodes.Collect(helper, s, Stream_Fields.True, generator);
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					Spliterator<T> s = unorderedSkipLimitSpliterator(helper.WrapSpliterator(spliterator), Skip, Limit, size);
					// Collect using this pipeline, which is empty and therefore
					// can be used with the pipeline wrapping spliterator
					// Note that we cannot create a slice spliterator from
					// the source spliterator if the pipeline is not SIZED
					return Nodes.Collect(this, s, Stream_Fields.True, generator);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, generator, Skip, Limit)).invoke();
				}
			}

			internal override Sink<T> OpWrapSink(int flags, Sink<T> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper : Sink_ChainedReference<T, T>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<T> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					n = outerInstance.Skip;
					m = outerInstance.Limit >= 0 ? outerInstance.Limit : Long.MaxValue;
				}

				internal long n;
				internal long m;

				public override void Begin(long size)
				{
					downstream.begin(CalcSize(size, OuterInstance.Skip, m));
				}

				public override void Accept(T Stream_Fields)
				{
					if (n == 0)
					{
						if (m > 0)
						{
							m--;
							downstream.accept(Stream_Fields.t);
						}
					}
					else
					{
						n--;
					}
				}

				public override bool CancellationRequested()
				{
					return m == 0 || downstream.cancellationRequested();
				}
			}
		}

		/// <summary>
		/// Appends a "slice" operation to the provided IntStream.  The slice
		/// operation may be may be skip-only, limit-only, or skip-and-limit.
		/// </summary>
		/// <param name="upstream"> An IntStream </param>
		/// <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		/// <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		///        is to be imposed </param>
		public static IntStream makeInt<T1>(AbstractPipeline<T1> upstream, long skip, long limit)
		{
			if (skip < 0)
			{
				throw new IllegalArgumentException("Skip must be non-negative: " + skip);
			}

			return new StatefulOpAnonymousInnerClassHelper(upstream, Flags(limit), skip, limit);
		}

		private class StatefulOpAnonymousInnerClassHelper : IntPipeline.StatefulOp<Integer>
		{
			private long Skip;
			private long Limit;

			public StatefulOpAnonymousInnerClassHelper(java.util.stream.AbstractPipeline<T1> upstream, int flags, long skip, long limit) : base(upstream, StreamShape.INT_VALUE, flags)
			{
				this.Skip = skip;
				this.Limit = limit;
			}

			internal virtual java.util.Spliterator_OfInt UnorderedSkipLimitSpliterator(java.util.Spliterator_OfInt s, long skip, long limit, long sizeIfKnown)
			{
				if (skip <= sizeIfKnown)
				{
					// Use just the limit if the number of elements
					// to skip is <= the known pipeline size
					limit = limit >= 0 ? System.Math.Min(limit, sizeIfKnown - skip) : sizeIfKnown - skip;
					skip = 0;
				}
				return new StreamSpliterators.UnorderedSliceSpliterator.OfInt(s, skip, limit);
			}

			internal override Spliterator<Integer> opEvaluateParallelLazy<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					return new StreamSpliterators.SliceSpliterator.OfInt((java.util.Spliterator_OfInt) helper.WrapSpliterator(spliterator), Skip, CalcSliceFence(Skip, Limit));
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					return unorderedSkipLimitSpliterator((java.util.Spliterator_OfInt) helper.WrapSpliterator(spliterator), Skip, Limit, size);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, Integer[] ::new, Skip, Limit)).invoke().spliterator();
				}
			}

			internal override Node<Integer> opEvaluateParallel<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator, IntFunction<Integer[]> generator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					// Because the pipeline is SIZED the slice spliterator
					// can be created from the source, this requires matching
					// to shape of the source, and is potentially more efficient
					// than creating the slice spliterator from the pipeline
					// wrapping spliterator
					Spliterator<P_IN> s = SliceSpliterator(helper.SourceShape, spliterator, Skip, Limit);
					return Nodes.CollectInt(helper, s, IntStream_Fields.True);
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					java.util.Spliterator_OfInt s = unorderedSkipLimitSpliterator((java.util.Spliterator_OfInt) helper.WrapSpliterator(spliterator), Skip, Limit, size);
					// Collect using this pipeline, which is empty and therefore
					// can be used with the pipeline wrapping spliterator
					// Note that we cannot create a slice spliterator from
					// the source spliterator if the pipeline is not SIZED
					return Nodes.CollectInt(this, s, IntStream_Fields.True);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, generator, Skip, Limit)).invoke();
				}
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper : Sink_ChainedInt<Integer>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					n = outerInstance.Skip;
					m = outerInstance.Limit >= 0 ? outerInstance.Limit : Long.MaxValue;
				}

				internal long n;
				internal long m;

				public override void Begin(long size)
				{
					downstream.begin(CalcSize(size, OuterInstance.Skip, m));
				}

				public override void Accept(int IntStream_Fields)
				{
					if (n == 0)
					{
						if (m > 0)
						{
							m--;
							downstream.accept(IntStream_Fields.t);
						}
					}
					else
					{
						n--;
					}
				}

				public override bool CancellationRequested()
				{
					return m == 0 || downstream.cancellationRequested();
				}
			}
		}

		/// <summary>
		/// Appends a "slice" operation to the provided LongStream.  The slice
		/// operation may be may be skip-only, limit-only, or skip-and-limit.
		/// </summary>
		/// <param name="upstream"> A LongStream </param>
		/// <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		/// <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		///        is to be imposed </param>
		public static LongStream makeLong<T1>(AbstractPipeline<T1> upstream, long skip, long limit)
		{
			if (skip < 0)
			{
				throw new IllegalArgumentException("Skip must be non-negative: " + skip);
			}

			return new StatefulOpAnonymousInnerClassHelper(upstream, Flags(limit), skip, limit);
		}

		private class StatefulOpAnonymousInnerClassHelper : LongPipeline.StatefulOp<Long>
		{
			private long Skip;
			private long Limit;

			public StatefulOpAnonymousInnerClassHelper(java.util.stream.AbstractPipeline<T1> upstream, int flags, long skip, long limit) : base(upstream, StreamShape.LONG_VALUE, flags)
			{
				this.Skip = skip;
				this.Limit = limit;
			}

			internal virtual java.util.Spliterator_OfLong UnorderedSkipLimitSpliterator(java.util.Spliterator_OfLong s, long skip, long limit, long sizeIfKnown)
			{
				if (skip <= sizeIfKnown)
				{
					// Use just the limit if the number of elements
					// to skip is <= the known pipeline size
					limit = limit >= 0 ? System.Math.Min(limit, sizeIfKnown - skip) : sizeIfKnown - skip;
					skip = 0;
				}
				return new StreamSpliterators.UnorderedSliceSpliterator.OfLong(s, skip, limit);
			}

			internal override Spliterator<Long> opEvaluateParallelLazy<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					return new StreamSpliterators.SliceSpliterator.OfLong((java.util.Spliterator_OfLong) helper.WrapSpliterator(spliterator), Skip, CalcSliceFence(Skip, Limit));
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					return unorderedSkipLimitSpliterator((java.util.Spliterator_OfLong) helper.WrapSpliterator(spliterator), Skip, Limit, size);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, Long[] ::new, Skip, Limit)).invoke().spliterator();
				}
			}

			internal override Node<Long> opEvaluateParallel<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator, IntFunction<Long[]> generator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					// Because the pipeline is SIZED the slice spliterator
					// can be created from the source, this requires matching
					// to shape of the source, and is potentially more efficient
					// than creating the slice spliterator from the pipeline
					// wrapping spliterator
					Spliterator<P_IN> s = SliceSpliterator(helper.SourceShape, spliterator, Skip, Limit);
					return Nodes.CollectLong(helper, s, LongStream_Fields.True);
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					java.util.Spliterator_OfLong s = unorderedSkipLimitSpliterator((java.util.Spliterator_OfLong) helper.WrapSpliterator(spliterator), Skip, Limit, size);
					// Collect using this pipeline, which is empty and therefore
					// can be used with the pipeline wrapping spliterator
					// Note that we cannot create a slice spliterator from
					// the source spliterator if the pipeline is not SIZED
					return Nodes.CollectLong(this, s, LongStream_Fields.True);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, generator, Skip, Limit)).invoke();
				}
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper : Sink_ChainedLong<Long>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					n = outerInstance.Skip;
					LongStream_Fields.m = outerInstance.Limit >= 0 ? outerInstance.Limit : Long.MaxValue;
				}

				internal long n;
				internal long LongStream_Fields;

				public override void Begin(long size)
				{
					downstream.begin(CalcSize(size, OuterInstance.Skip, LongStream_Fields.m));
				}

				public override void Accept(long LongStream_Fields)
				{
					if (n == 0)
					{
						if (LongStream_Fields.m > 0)
						{
							LongStream_Fields.m--;
							downstream.accept(LongStream_Fields.t);
						}
					}
					else
					{
						n--;
					}
				}

				public override bool CancellationRequested()
				{
					return LongStream_Fields.m == 0 || downstream.cancellationRequested();
				}
			}
		}

		/// <summary>
		/// Appends a "slice" operation to the provided DoubleStream.  The slice
		/// operation may be may be skip-only, limit-only, or skip-and-limit.
		/// </summary>
		/// <param name="upstream"> A DoubleStream </param>
		/// <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		/// <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		///        is to be imposed </param>
		public static DoubleStream makeDouble<T1>(AbstractPipeline<T1> upstream, long skip, long limit)
		{
			if (skip < 0)
			{
				throw new IllegalArgumentException("Skip must be non-negative: " + skip);
			}

			return new StatefulOpAnonymousInnerClassHelper(upstream, Flags(limit), skip, limit);
		}

		private class StatefulOpAnonymousInnerClassHelper : DoublePipeline.StatefulOp<Double>
		{
			private long Skip;
			private long Limit;

			public StatefulOpAnonymousInnerClassHelper(java.util.stream.AbstractPipeline<T1> upstream, int flags, long skip, long limit) : base(upstream, StreamShape.DOUBLE_VALUE, flags)
			{
				this.Skip = skip;
				this.Limit = limit;
			}

			internal virtual java.util.Spliterator_OfDouble UnorderedSkipLimitSpliterator(java.util.Spliterator_OfDouble s, long skip, long limit, long sizeIfKnown)
			{
				if (skip <= sizeIfKnown)
				{
					// Use just the limit if the number of elements
					// to skip is <= the known pipeline size
					limit = limit >= 0 ? System.Math.Min(limit, sizeIfKnown - skip) : sizeIfKnown - skip;
					skip = 0;
				}
				return new StreamSpliterators.UnorderedSliceSpliterator.OfDouble(s, skip, limit);
			}

			internal override Spliterator<Double> opEvaluateParallelLazy<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					return new StreamSpliterators.SliceSpliterator.OfDouble((java.util.Spliterator_OfDouble) helper.WrapSpliterator(spliterator), Skip, CalcSliceFence(Skip, Limit));
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					return unorderedSkipLimitSpliterator((java.util.Spliterator_OfDouble) helper.WrapSpliterator(spliterator), Skip, Limit, size);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, Double[] ::new, Skip, Limit)).invoke().spliterator();
				}
			}

			internal override Node<Double> opEvaluateParallel<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator, IntFunction<Double[]> generator)
			{
				long size = helper.ExactOutputSizeIfKnown(spliterator);
				if (size > 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
				{
					// Because the pipeline is SIZED the slice spliterator
					// can be created from the source, this requires matching
					// to shape of the source, and is potentially more efficient
					// than creating the slice spliterator from the pipeline
					// wrapping spliterator
					Spliterator<P_IN> s = SliceSpliterator(helper.SourceShape, spliterator, Skip, Limit);
					return Nodes.CollectDouble(helper, s, DoubleStream_Fields.True);
				}
				else if (!StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					java.util.Spliterator_OfDouble s = unorderedSkipLimitSpliterator((java.util.Spliterator_OfDouble) helper.WrapSpliterator(spliterator), Skip, Limit, size);
					// Collect using this pipeline, which is empty and therefore
					// can be used with the pipeline wrapping spliterator
					// Note that we cannot create a slice spliterator from
					// the source spliterator if the pipeline is not SIZED
					return Nodes.CollectDouble(this, s, DoubleStream_Fields.True);
				}
				else
				{
					return (new SliceTask<>(this, helper, spliterator, generator, Skip, Limit)).invoke();
				}
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper : Sink_ChainedDouble<Double>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					n = outerInstance.Skip;
					m = outerInstance.Limit >= 0 ? outerInstance.Limit : Long.MaxValue;
				}

				internal long n;
				internal long m;

				public override void Begin(long size)
				{
					downstream.begin(CalcSize(size, OuterInstance.Skip, m));
				}

				public override void Accept(double DoubleStream_Fields)
				{
					if (n == 0)
					{
						if (m > 0)
						{
							m--;
							downstream.accept(DoubleStream_Fields.t);
						}
					}
					else
					{
						n--;
					}
				}

				public override bool CancellationRequested()
				{
					return m == 0 || downstream.cancellationRequested();
				}
			}
		}

		private static int Flags(long limit)
		{
			return StreamOpFlag.NOT_SIZED | ((limit != -1) ? StreamOpFlag.IS_SHORT_CIRCUIT : 0);
		}

		/// <summary>
		/// {@code ForkJoinTask} implementing slice computation.
		/// </summary>
		/// @param <P_IN> Input element type to the stream pipeline </param>
		/// @param <P_OUT> Output element type from the stream pipeline </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class SliceTask<P_IN, P_OUT> extends AbstractShortCircuitTask<P_IN, P_OUT, Node<P_OUT>, SliceTask<P_IN, P_OUT>>
		private sealed class SliceTask<P_IN, P_OUT> : AbstractShortCircuitTask<P_IN, P_OUT, Node<P_OUT>, SliceTask<P_IN, P_OUT>>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final AbstractPipeline<P_OUT, P_OUT, ?> op;
			internal readonly AbstractPipeline<P_OUT, P_OUT, ?> Op;
			internal readonly IntFunction<P_OUT[]> Generator;
			internal readonly long TargetOffset, TargetSize;
			internal long ThisNodeSize;

			internal volatile bool Completed;

			internal SliceTask<T1>(AbstractPipeline<T1> op, PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator, IntFunction<P_OUT[]> generator, long offset, long size) : base(helper, spliterator)
			{
				this.Op = op;
				this.Generator = generator;
				this.TargetOffset = offset;
				this.TargetSize = size;
			}

			internal SliceTask(SliceTask<P_IN, P_OUT> parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
			{
				this.Op = parent.Op;
				this.Generator = parent.Generator;
				this.TargetOffset = parent.TargetOffset;
				this.TargetSize = parent.TargetSize;
			}

			protected internal override SliceTask<P_IN, P_OUT> MakeChild(Spliterator<P_IN> spliterator)
			{
				return new SliceTask<>(this, spliterator);
			}

			protected internal override Node<P_OUT> EmptyResult
			{
				get
				{
					return Nodes.EmptyNode(Op.OutputShape);
				}
			}

			protected internal override Node<P_OUT> DoLeaf()
			{
				if (Root)
				{
					long sizeIfKnown = StreamOpFlag.SIZED.isPreserved(Op.SourceOrOpFlags) ? Op.ExactOutputSizeIfKnown(spliterator) : -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node_Builder<P_OUT> nb = op.makeNodeBuilder(sizeIfKnown, generator);
					Node_Builder<P_OUT> nb = Op.MakeNodeBuilder(sizeIfKnown, Generator);
					Sink<P_OUT> opSink = Op.OpWrapSink(helper.StreamAndOpFlags, nb);
					helper.copyIntoWithCancel(helper.wrapSink(opSink), spliterator);
					// There is no need to truncate since the op performs the
					// skipping and limiting of elements
					return nb.Build();
				}
				else
				{
					Node<P_OUT> node = helper.wrapAndCopyInto(helper.makeNodeBuilder(-1, Generator), spliterator).build();
					ThisNodeSize = node.Count();
					Completed = true;
					spliterator = null;
					return node;
				}
			}

			public override void onCompletion<T1>(CountedCompleter<T1> caller)
			{
				if (!Leaf)
				{
					Node<P_OUT> result;
					ThisNodeSize = leftChild.thisNodeSize + rightChild.thisNodeSize;
					if (Canceled)
					{
						ThisNodeSize = 0;
						result = EmptyResult;
					}
					else if (ThisNodeSize == 0)
					{
						result = EmptyResult;
					}
					else if (leftChild.thisNodeSize == 0)
					{
						result = rightChild.LocalResult;
					}
					else
					{
						result = Nodes.Conc(Op.OutputShape, leftChild.LocalResult, rightChild.LocalResult);
					}
					LocalResult = Root ? DoTruncate(result) : result;
					Completed = true;
				}
				if (TargetSize >= 0 && !Root && IsLeftCompleted(TargetOffset + TargetSize))
				{
						CancelLaterNodes();
				}

				base.OnCompletion(caller);
			}

			protected internal override void Cancel()
			{
				base.Cancel();
				if (Completed)
				{
					LocalResult = EmptyResult;
				}
			}

			internal Node<P_OUT> DoTruncate(Node<P_OUT> input)
			{
				long to = TargetSize >= 0 ? System.Math.Min(input.Count(), TargetOffset + TargetSize) : ThisNodeSize;
				return input.truncate(TargetOffset, to, Generator);
			}

			/// <summary>
			/// Determine if the number of completed elements in this node and nodes
			/// to the left of this node is greater than or equal to the target size.
			/// </summary>
			/// <param name="target"> the target size </param>
			/// <returns> true if the number of elements is greater than or equal to
			///         the target size, otherwise false. </returns>
			internal bool IsLeftCompleted(long target)
			{
				long size = Completed ? ThisNodeSize : CompletedSize(target);
				if (size >= target)
				{
					return true;
				}
				for (SliceTask<P_IN, P_OUT> parent = Parent, node = this; parent != null; node = parent, parent = parent.Parent)
				{
					if (node == parent.rightChild)
					{
						SliceTask<P_IN, P_OUT> left = parent.leftChild;
						if (left != null)
						{
							size += left.CompletedSize(target);
							if (size >= target)
							{
								return true;
							}
						}
					}
				}
				return size >= target;
			}

			/// <summary>
			/// Compute the number of completed elements in this node.
			/// <para>
			/// Computation terminates if all nodes have been processed or the
			/// number of completed elements is greater than or equal to the target
			/// size.
			/// 
			/// </para>
			/// </summary>
			/// <param name="target"> the target size </param>
			/// <returns> return the number of completed elements </returns>
			internal long CompletedSize(long target)
			{
				if (Completed)
				{
					return ThisNodeSize;
				}
				else
				{
					SliceTask<P_IN, P_OUT> left = leftChild;
					SliceTask<P_IN, P_OUT> right = rightChild;
					if (left == null || right == null)
					{
						// must be completed
						return ThisNodeSize;
					}
					else
					{
						long leftSize = left.CompletedSize(target);
						return (leftSize >= target) ? leftSize : leftSize + right.CompletedSize(target);
					}
				}
			}
		}
	}

}