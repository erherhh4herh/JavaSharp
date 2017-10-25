using System;
using System.Diagnostics;

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
namespace java.util.stream
{


	/// <summary>
	/// Abstract base class for an intermediate pipeline stage or pipeline source
	/// stage implementing whose elements are of type {@code long}.
	/// </summary>
	/// @param <E_IN> type of elements in the upstream source
	/// @since 1.8 </param>
	internal abstract class LongPipeline<E_IN> : AbstractPipeline<E_IN, Long, LongStream>, LongStream
	{

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal LongPipeline<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Long> : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Spliterator} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal LongPipeline(Spliterator<Long> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for appending an intermediate operation onto an existing pipeline.
		/// </summary>
		/// <param name="upstream"> the upstream element source. </param>
		/// <param name="opFlags"> the operation flags </param>
		internal LongPipeline<T1>(AbstractPipeline<T1> upstream, int opFlags) : base(upstream, opFlags)
		{
		}

		/// <summary>
		/// Adapt a {@code Sink<Long> to an {@code LongConsumer}, ideally simply
		/// by casting.
		/// </summary>
		private static LongConsumer Adapt(Sink<Long> sink)
		{
			if (sink is LongConsumer)
			{
				return (LongConsumer) sink;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using LongStream.adapt(Sink<Long> s)");
				}
				return sink::accept;
			}
		}

		/// <summary>
		/// Adapt a {@code Spliterator<Long>} to a {@code Spliterator.OfLong}.
		/// 
		/// @implNote
		/// The implementation attempts to cast to a Spliterator.OfLong, and throws
		/// an exception if this cast is not possible.
		/// </summary>
		private static java.util.Spliterator_OfLong Adapt(Spliterator<Long> s)
		{
			if (s is java.util.Spliterator_OfLong)
			{
				return (java.util.Spliterator_OfLong) s;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using LongStream.adapt(Spliterator<Long> s)");
				}
				throw new UnsupportedOperationException("LongStream.adapt(Spliterator<Long> s)");
			}
		}


		// Shape-specific methods

		internal override StreamShape OutputShape
		{
			get
			{
				return StreamShape.LONG_VALUE;
			}
		}

		internal override Node<Long> evaluateToNode<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<Long[]> generator)
		{
			return Nodes.CollectLong(helper, spliterator, flattenTree);
		}

		internal override Spliterator<Long> wrap<P_IN>(PipelineHelper<Long> ph, Supplier<Spliterator<P_IN>> supplier, bool isParallel)
		{
			return new StreamSpliterators.LongWrappingSpliterator<>(ph, supplier, isParallel);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfLong lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Long>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfLong lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Long>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal override java.util.Spliterator_OfLong lazySpliterator<T1>(Supplier<T1> supplier) where ? : java.util.Spliterator<Long>
		{
			return new StreamSpliterators.DelegatingSpliterator.OfLong((Supplier<java.util.Spliterator_OfLong>) supplier);
		}

		internal override void ForEachWithCancel(Spliterator<Long> spliterator, Sink<Long> sink)
		{
			java.util.Spliterator_OfLong spl = Adapt(spliterator);
			LongConsumer adaptedSink = Adapt(sink);
			do
			{
			} while (!sink.cancellationRequested() && spl.TryAdvance(adaptedSink));
		}

		internal override Node_Builder<Long> MakeNodeBuilder(long exactSizeIfKnown, IntFunction<Long[]> generator)
		{
			return Nodes.LongBuilder(exactSizeIfKnown);
		}


		// LongStream

		public java.util.PrimitiveIterator_OfLong Iterator()
		{
			return Spliterators.Iterator(Spliterator());
		}

		public java.util.Spliterator_OfLong Spliterator()
		{
			return Adapt(base.Spliterator());
		}

		// Stateless intermediate ops from LongStream

		public DoubleStream AsDoubleStream()
		{
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT);
		}

		private class StatelessOpAnonymousInnerClassHelper : DoublePipeline.StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_DISTINCT) : base(this, StreamShape.LONG_VALUE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper : Sink_ChainedLong<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long DoubleStream_Fields)
				{
					downstream.accept((double) DoubleStream_Fields.t);
				}
			}
		}

		public Stream<Long> Boxed()
		{
			return MapToObj(Long::valueOf);
		}

		public LongStream Map(LongUnaryOperator mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongUnaryOperator Mapper;

			public StatelessOpAnonymousInnerClassHelper(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_DISTINCT, LongUnaryOperator mapper) : base(this, StreamShape.LONG_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper2(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper2 : Sink_ChainedLong<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper2(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long LongStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsLong(LongStream_Fields.t));
				}
			}
		}

		public Stream<U> mapToObj<U, T1>(LongFunction<T1> mapper) where T1 : U
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : ReferencePipeline.StatelessOp<Long, U>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_DISTINCT, LongFunction<T1> mapper) : base(this, StreamShape.LONG_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<U> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper3(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper3 : Sink_ChainedLong<U>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper3(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<U> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long LongStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.apply(LongStream_Fields.t));
				}
			}
		}

		public IntStream MapToInt(LongToIntFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : IntPipeline.StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongToIntFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_DISTINCT, LongToIntFunction mapper) : base(this, StreamShape.LONG_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper4(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper4 : Sink_ChainedLong<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper4(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long IntStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsInt(IntStream_Fields.t));
				}
			}
		}

		public DoubleStream MapToDouble(LongToDoubleFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : DoublePipeline.StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongToDoubleFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper2(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_DISTINCT, LongToDoubleFunction mapper) : base(this, StreamShape.LONG_VALUE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper5(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper5 : Sink_ChainedLong<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper5(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsDouble(DoubleStream_Fields.t));
				}
			}
		}

		public LongStream flatMap<T1>(LongFunction<T1> mapper) where T1 : LongStream
		{
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, int NOT_SIZED, LongFunction<T1> mapper) : base(this, StreamShape.LONG_VALUE, NOT_SIZED)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper6(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper6 : Sink_ChainedLong<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper6(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(long LongStream_Fields)
				{
					using (LongStream result = OuterInstance.Mapper.apply(LongStream_Fields.t))
					{
						// We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
						if (result != null)
						{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
							result.sequential().forEach(i => downstream.accept(i));
						}
					}
				}
			}
		}

		public virtual LongStream Unordered()
		{
			if (!Ordered)
			{
				return this;
			}
			return new StatelessOpAnonymousInnerClassHelper3(this, this);
		}

		private class StatelessOpAnonymousInnerClassHelper3 : StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper3(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this) : base(this, StreamShape.LONG_VALUE, StreamOpFlag.NOT_ORDERED)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return sink;
			}
		}

		public LongStream Filter(LongPredicate predicate)
		{
			Objects.RequireNonNull(predicate);
			return new StatelessOpAnonymousInnerClassHelper4(this, this, predicate);
		}

		private class StatelessOpAnonymousInnerClassHelper4 : StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongPredicate Predicate;

			public StatelessOpAnonymousInnerClassHelper4(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, LongPredicate predicate) : base(this, StreamShape.LONG_VALUE, StreamOpFlag.NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Predicate = predicate;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper7(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper7 : Sink_ChainedLong<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper4 OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper7(StatelessOpAnonymousInnerClassHelper4 outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(long LongStream_Fields)
				{
					if (OuterInstance.Predicate.test(LongStream_Fields.t))
					{
						downstream.accept(LongStream_Fields.t);
					}
				}
			}
		}

		public LongStream Peek(LongConsumer action)
		{
			Objects.RequireNonNull(action);
			return new StatelessOpAnonymousInnerClassHelper5(this, this, action);
		}

		private class StatelessOpAnonymousInnerClassHelper5 : StatelessOp<Long>
		{
			private readonly LongPipeline<E_IN> OuterInstance;

			private LongConsumer Action;

			public StatelessOpAnonymousInnerClassHelper5(LongPipeline<E_IN> outerInstance, java.util.stream.LongPipeline this, LongConsumer action) : base(this, StreamShape.LONG_VALUE, 0)
			{
				this.outerInstance = outerInstance;
				this.Action = action;
			}

			internal override Sink<Long> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedLongAnonymousInnerClassHelper8(this, sink);
			}

			private class Sink_ChainedLongAnonymousInnerClassHelper8 : Sink_ChainedLong<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper5 OuterInstance;

				public Sink_ChainedLongAnonymousInnerClassHelper8(StatelessOpAnonymousInnerClassHelper5 outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(long LongStream_Fields)
				{
					OuterInstance.Action.accept(LongStream_Fields.t);
					downstream.accept(LongStream_Fields.t);
				}
			}
		}

		// Stateful intermediate ops from LongStream

		public LongStream Limit(long maxSize)
		{
			if (maxSize < 0)
			{
				throw new IllegalArgumentException(Convert.ToString(maxSize));
			}
			return SliceOps.MakeLong(this, 0, maxSize);
		}

		public LongStream Skip(long n)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException(Convert.ToString(n));
			}
			if (n == 0)
			{
				return this;
			}
			else
			{
				return SliceOps.MakeLong(this, n, -1);
			}
		}

		public LongStream Sorted()
		{
			return SortedOps.MakeLong(this);
		}

		public LongStream Distinct()
		{
			// While functional and quick to implement, this approach is not very efficient.
			// An efficient version requires a long-specific map/set implementation.
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return Boxed().Distinct().MapToLong(i => (long) i);
		}

		// Terminal ops from LongStream

		public virtual void ForEach(LongConsumer action)
		{
			Evaluate(ForEachOps.MakeLong(action, false));
		}

		public virtual void ForEachOrdered(LongConsumer action)
		{
			Evaluate(ForEachOps.MakeLong(action, LongStream_Fields.True));
		}

		public long Sum()
		{
			// use better algorithm to compensate for intermediate overflow?
			return Reduce(0, Long::sum);
		}

		public OptionalLong Min()
		{
			return Reduce(Math::min);
		}

		public OptionalLong Max()
		{
			return Reduce(Math::max);
		}

		public OptionalDouble Average()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			long[] avg = Collect(() => new long[2], (ll, i) => {ll[0]++; ll[1] += i;}, (ll, rr) => {ll[0] += rr[0]; ll[1] += rr[1];});
			return avg[0] > 0 ? OptionalDouble.Of((double) avg[1] / avg[0]) : OptionalDouble.Empty();
		}

		public long Count()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return Map(e => 1L).Sum();
		}

		public LongSummaryStatistics SummaryStatistics()
		{
			return Collect(LongSummaryStatistics::new, LongSummaryStatistics::accept, LongSummaryStatistics::combine);
		}

		public long Reduce(long identity, LongBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeLong(identity, op));
		}

		public OptionalLong Reduce(LongBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeLong(op));
		}

		public R collect<R>(Supplier<R> supplier, ObjLongConsumer<R> accumulator, BiConsumer<R, R> combiner)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BinaryOperator<R> @operator = (left, right) =>
			{
				combiner.Accept(left, right);
				return left;
			};
			return Evaluate(ReduceOps.MakeLong(supplier, accumulator, @operator));
		}

		public bool AnyMatch(LongPredicate predicate)
		{
			return Evaluate(MatchOps.MakeLong(predicate, MatchOps.MatchKind.ANY));
		}

		public bool AllMatch(LongPredicate predicate)
		{
			return Evaluate(MatchOps.MakeLong(predicate, MatchOps.MatchKind.ALL));
		}

		public bool NoneMatch(LongPredicate predicate)
		{
			return Evaluate(MatchOps.MakeLong(predicate, MatchOps.MatchKind.NONE));
		}

		public OptionalLong FindFirst()
		{
			return Evaluate(FindOps.MakeLong(LongStream_Fields.True));
		}

		public OptionalLong FindAny()
		{
			return Evaluate(FindOps.MakeLong(false));
		}

		public long[] ToArray()
		{
			return Nodes.FlattenLong((Node_OfLong) EvaluateToArrayNode(Long[] ::new)).AsPrimitiveArray();
		}


		//

		/// <summary>
		/// Source stage of a LongPipeline.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal class Head<E_IN> : LongPipeline<E_IN>
		{
			/// <summary>
			/// Constructor for the source stage of a LongStream.
			/// </summary>
			/// <param name="source"> {@code Supplier<Spliterator>} describing the stream
			///               source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Long> : base(source, sourceFlags, parallel)
			{
			}

			/// <summary>
			/// Constructor for the source stage of a LongStream.
			/// </summary>
			/// <param name="source"> {@code Spliterator} describing the stream source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head(Spliterator<Long> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
			{
			}

			internal override bool OpIsStateful()
			{
				throw new UnsupportedOperationException();
			}

			internal override Sink<E_IN> OpWrapSink(int flags, Sink<Long> sink)
			{
				throw new UnsupportedOperationException();
			}

			// Optimized sequential terminal operations for the head of the pipeline

			public override void ForEach(LongConsumer action)
			{
				if (!Parallel)
				{
					Adapt(SourceStageSpliterator()).forEachRemaining(action);
				}
				else
				{
					base.ForEach(action);
				}
			}

			public override void ForEachOrdered(LongConsumer action)
			{
				if (!Parallel)
				{
					Adapt(SourceStageSpliterator()).forEachRemaining(action);
				}
				else
				{
					base.ForEachOrdered(action);
				}
			}
		}

		/// <summary>
		/// Base class for a stateless intermediate stage of a LongStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatelessOp<E_IN> : LongPipeline<E_IN>
		{
			/// <summary>
			/// Construct a new LongStream by appending a stateless intermediate
			/// operation to an existing stream. </summary>
			/// <param name="upstream"> The upstream pipeline stage </param>
			/// <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			/// <param name="opFlags"> Operation flags for the new stage </param>
			internal StatelessOp<T1>(AbstractPipeline<T1> upstream, StreamShape inputShape, int opFlags) : base(upstream, opFlags)
			{
				Debug.Assert(upstream.OutputShape == inputShape);
			}

			internal override bool OpIsStateful()
			{
				return false;
			}
		}

		/// <summary>
		/// Base class for a stateful intermediate stage of a LongStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatefulOp<E_IN> : LongPipeline<E_IN>
		{
			/// <summary>
			/// Construct a new LongStream by appending a stateful intermediate
			/// operation to an existing stream. </summary>
			/// <param name="upstream"> The upstream pipeline stage </param>
			/// <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			/// <param name="opFlags"> Operation flags for the new stage </param>
			internal StatefulOp<T1>(AbstractPipeline<T1> upstream, StreamShape inputShape, int opFlags) : base(upstream, opFlags)
			{
				Debug.Assert(upstream.OutputShape == inputShape);
			}

			internal override bool OpIsStateful()
			{
				return LongStream_Fields.True;
			}

			internal override abstract Node<Long> opEvaluateParallel<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator, IntFunction<Long[]> generator);
		}
	}

}