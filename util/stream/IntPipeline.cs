using System;
using System.Diagnostics;

/*
 * Copyright (c) 2012, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// stage implementing whose elements are of type {@code int}.
	/// </summary>
	/// @param <E_IN> type of elements in the upstream source
	/// @since 1.8 </param>
	internal abstract class IntPipeline<E_IN> : AbstractPipeline<E_IN, Integer, IntStream>, IntStream
	{

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		/// <param name="sourceFlags"> The source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal IntPipeline<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Integer> : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Spliterator} describing the stream source </param>
		/// <param name="sourceFlags"> The source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal IntPipeline(Spliterator<Integer> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for appending an intermediate operation onto an existing
		/// pipeline.
		/// </summary>
		/// <param name="upstream"> the upstream element source </param>
		/// <param name="opFlags"> the operation flags for the new operation </param>
		internal IntPipeline<T1>(AbstractPipeline<T1> upstream, int opFlags) : base(upstream, opFlags)
		{
		}

		/// <summary>
		/// Adapt a {@code Sink<Integer> to an {@code IntConsumer}, ideally simply
		/// by casting.
		/// </summary>
		private static IntConsumer Adapt(Sink<Integer> sink)
		{
			if (sink is IntConsumer)
			{
				return (IntConsumer) sink;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using IntStream.adapt(Sink<Integer> s)");
				}
				return sink::accept;
			}
		}

		/// <summary>
		/// Adapt a {@code Spliterator<Integer>} to a {@code Spliterator.OfInt}.
		/// 
		/// @implNote
		/// The implementation attempts to cast to a Spliterator.OfInt, and throws an
		/// exception if this cast is not possible.
		/// </summary>
		private static java.util.Spliterator_OfInt Adapt(Spliterator<Integer> s)
		{
			if (s is java.util.Spliterator_OfInt)
			{
				return (java.util.Spliterator_OfInt) s;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using IntStream.adapt(Spliterator<Integer> s)");
				}
				throw new UnsupportedOperationException("IntStream.adapt(Spliterator<Integer> s)");
			}
		}


		// Shape-specific methods

		internal override StreamShape OutputShape
		{
			get
			{
				return StreamShape.INT_VALUE;
			}
		}

		internal override Node<Integer> evaluateToNode<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<Integer[]> generator)
		{
			return Nodes.CollectInt(helper, spliterator, flattenTree);
		}

		internal override Spliterator<Integer> wrap<P_IN>(PipelineHelper<Integer> ph, Supplier<Spliterator<P_IN>> supplier, bool isParallel)
		{
			return new StreamSpliterators.IntWrappingSpliterator<>(ph, supplier, isParallel);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfInt lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Integer>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfInt lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Integer>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal override java.util.Spliterator_OfInt lazySpliterator<T1>(Supplier<T1> supplier) where ? : java.util.Spliterator<Integer>
		{
			return new StreamSpliterators.DelegatingSpliterator.OfInt((Supplier<java.util.Spliterator_OfInt>) supplier);
		}

		internal override void ForEachWithCancel(Spliterator<Integer> spliterator, Sink<Integer> sink)
		{
			java.util.Spliterator_OfInt spl = Adapt(spliterator);
			IntConsumer adaptedSink = Adapt(sink);
			do
			{
			} while (!sink.cancellationRequested() && spl.TryAdvance(adaptedSink));
		}

		internal override Node_Builder<Integer> MakeNodeBuilder(long exactSizeIfKnown, IntFunction<Integer[]> generator)
		{
			return Nodes.IntBuilder(exactSizeIfKnown);
		}


		// IntStream

		public java.util.PrimitiveIterator_OfInt Iterator()
		{
			return Spliterators.Iterator(Spliterator());
		}

		public java.util.Spliterator_OfInt Spliterator()
		{
			return Adapt(base.Spliterator());
		}

		// Stateless intermediate ops from IntStream

		public LongStream AsLongStream()
		{
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT);
		}

		private class StatelessOpAnonymousInnerClassHelper : LongPipeline.StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper : Sink_ChainedInt<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int IntStream_Fields)
				{
					downstream.accept((long) IntStream_Fields.t);
				}
			}
		}

		public DoubleStream AsDoubleStream()
		{
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT);
		}

		private class StatelessOpAnonymousInnerClassHelper : DoublePipeline.StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper2(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper2 : Sink_ChainedInt<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper2(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int DoubleStream_Fields)
				{
					downstream.accept((double) DoubleStream_Fields.t);
				}
			}
		}

		public Stream<Integer> Boxed()
		{
			return MapToObj(Integer::valueOf);
		}

		public IntStream Map(IntUnaryOperator mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntUnaryOperator Mapper;

			public StatelessOpAnonymousInnerClassHelper(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT, IntUnaryOperator mapper) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper3(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper3 : Sink_ChainedInt<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper3(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int IntStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsInt(IntStream_Fields.t));
				}
			}
		}

		public Stream<U> mapToObj<U, T1>(IntFunction<T1> mapper) where T1 : U
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : ReferencePipeline.StatelessOp<Integer, U>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT, IntFunction<T1> mapper) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<U> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper4(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper4 : Sink_ChainedInt<U>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper4(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<U> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int IntStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.apply(IntStream_Fields.t));
				}
			}
		}

		public LongStream MapToLong(IntToLongFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : LongPipeline.StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntToLongFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper2(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT, IntToLongFunction mapper) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper5(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper5 : Sink_ChainedInt<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper5(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int IntStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsLong(IntStream_Fields.t));
				}
			}
		}

		public DoubleStream MapToDouble(IntToDoubleFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : DoublePipeline.StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntToDoubleFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper2(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_DISTINCT, IntToDoubleFunction mapper) : base(this, StreamShape.INT_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper6(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper6 : Sink_ChainedInt<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper6(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsDouble(DoubleStream_Fields.t));
				}
			}
		}

		public IntStream flatMap<T1>(IntFunction<T1> mapper) where T1 : IntStream
		{
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, int NOT_SIZED, IntFunction<T1> mapper) : base(this, StreamShape.INT_VALUE, NOT_SIZED)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper7(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper7 : Sink_ChainedInt<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper7(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(int IntStream_Fields)
				{
					using (IntStream result = OuterInstance.Mapper.apply(IntStream_Fields.t))
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

		public virtual IntStream Unordered()
		{
			if (!Ordered)
			{
				return this;
			}
			return new StatelessOpAnonymousInnerClassHelper3(this, this);
		}

		private class StatelessOpAnonymousInnerClassHelper3 : StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper3(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this) : base(this, StreamShape.INT_VALUE, StreamOpFlag.NOT_ORDERED)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return sink;
			}
		}

		public IntStream Filter(IntPredicate predicate)
		{
			Objects.RequireNonNull(predicate);
			return new StatelessOpAnonymousInnerClassHelper4(this, this, predicate);
		}

		private class StatelessOpAnonymousInnerClassHelper4 : StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntPredicate Predicate;

			public StatelessOpAnonymousInnerClassHelper4(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, IntPredicate predicate) : base(this, StreamShape.INT_VALUE, StreamOpFlag.NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Predicate = predicate;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper8(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper8 : Sink_ChainedInt<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper4 OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper8(StatelessOpAnonymousInnerClassHelper4 outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(int IntStream_Fields)
				{
					if (OuterInstance.Predicate.test(IntStream_Fields.t))
					{
						downstream.accept(IntStream_Fields.t);
					}
				}
			}
		}

		public IntStream Peek(IntConsumer action)
		{
			Objects.RequireNonNull(action);
			return new StatelessOpAnonymousInnerClassHelper5(this, this, action);
		}

		private class StatelessOpAnonymousInnerClassHelper5 : StatelessOp<Integer>
		{
			private readonly IntPipeline<E_IN> OuterInstance;

			private IntConsumer Action;

			public StatelessOpAnonymousInnerClassHelper5(IntPipeline<E_IN> outerInstance, java.util.stream.IntPipeline this, IntConsumer action) : base(this, StreamShape.INT_VALUE, 0)
			{
				this.outerInstance = outerInstance;
				this.Action = action;
			}

			internal override Sink<Integer> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedIntAnonymousInnerClassHelper9(this, sink);
			}

			private class Sink_ChainedIntAnonymousInnerClassHelper9 : Sink_ChainedInt<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper5 OuterInstance;

				public Sink_ChainedIntAnonymousInnerClassHelper9(StatelessOpAnonymousInnerClassHelper5 outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(int IntStream_Fields)
				{
					OuterInstance.Action.accept(IntStream_Fields.t);
					downstream.accept(IntStream_Fields.t);
				}
			}
		}

		// Stateful intermediate ops from IntStream

		public IntStream Limit(long maxSize)
		{
			if (maxSize < 0)
			{
				throw new IllegalArgumentException(Convert.ToString(maxSize));
			}
			return SliceOps.MakeInt(this, 0, maxSize);
		}

		public IntStream Skip(long n)
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
				return SliceOps.MakeInt(this, n, -1);
			}
		}

		public IntStream Sorted()
		{
			return SortedOps.MakeInt(this);
		}

		public IntStream Distinct()
		{
			// While functional and quick to implement, this approach is not very efficient.
			// An efficient version requires an int-specific map/set implementation.
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return Boxed().Distinct().MapToInt(i => i);
		}

		// Terminal ops from IntStream

		public virtual void ForEach(IntConsumer action)
		{
			Evaluate(ForEachOps.MakeInt(action, false));
		}

		public virtual void ForEachOrdered(IntConsumer action)
		{
			Evaluate(ForEachOps.MakeInt(action, IntStream_Fields.True));
		}

		public int Sum()
		{
			return Reduce(0, Integer::sum);
		}

		public OptionalInt Min()
		{
			return Reduce(Math::min);
		}

		public OptionalInt Max()
		{
			return Reduce(Math::max);
		}

		public long Count()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return MapToLong(e => 1L).Sum();
		}

		public OptionalDouble Average()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			long[] avg = Collect(() => new long[2], (ll, i) => {ll[0]++; ll[1] += i;}, (ll, rr) => {ll[0] += rr[0]; ll[1] += rr[1];});
			return avg[0] > 0 ? OptionalDouble.Of((double) avg[1] / avg[0]) : OptionalDouble.Empty();
		}

		public IntSummaryStatistics SummaryStatistics()
		{
			return Collect(IntSummaryStatistics::new, IntSummaryStatistics::accept, IntSummaryStatistics::combine);
		}

		public int Reduce(int identity, IntBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeInt(identity, op));
		}

		public OptionalInt Reduce(IntBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeInt(op));
		}

		public R collect<R>(Supplier<R> supplier, ObjIntConsumer<R> accumulator, BiConsumer<R, R> combiner)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BinaryOperator<R> @operator = (left, right) =>
			{
				combiner.Accept(left, right);
				return left;
			};
			return Evaluate(ReduceOps.MakeInt(supplier, accumulator, @operator));
		}

		public bool AnyMatch(IntPredicate predicate)
		{
			return Evaluate(MatchOps.MakeInt(predicate, MatchOps.MatchKind.ANY));
		}

		public bool AllMatch(IntPredicate predicate)
		{
			return Evaluate(MatchOps.MakeInt(predicate, MatchOps.MatchKind.ALL));
		}

		public bool NoneMatch(IntPredicate predicate)
		{
			return Evaluate(MatchOps.MakeInt(predicate, MatchOps.MatchKind.NONE));
		}

		public OptionalInt FindFirst()
		{
			return Evaluate(FindOps.MakeInt(IntStream_Fields.True));
		}

		public OptionalInt FindAny()
		{
			return Evaluate(FindOps.MakeInt(false));
		}

		public int[] ToArray()
		{
			return Nodes.FlattenInt((Node_OfInt) EvaluateToArrayNode(Integer[] ::new)).AsPrimitiveArray();
		}

		//

		/// <summary>
		/// Source stage of an IntStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal class Head<E_IN> : IntPipeline<E_IN>
		{
			/// <summary>
			/// Constructor for the source stage of an IntStream.
			/// </summary>
			/// <param name="source"> {@code Supplier<Spliterator>} describing the stream
			///               source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Integer> : base(source, sourceFlags, parallel)
			{
			}

			/// <summary>
			/// Constructor for the source stage of an IntStream.
			/// </summary>
			/// <param name="source"> {@code Spliterator} describing the stream source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head(Spliterator<Integer> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
			{
			}

			internal override bool OpIsStateful()
			{
				throw new UnsupportedOperationException();
			}

			internal override Sink<E_IN> OpWrapSink(int flags, Sink<Integer> sink)
			{
				throw new UnsupportedOperationException();
			}

			// Optimized sequential terminal operations for the head of the pipeline

			public override void ForEach(IntConsumer action)
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

			public override void ForEachOrdered(IntConsumer action)
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
		/// Base class for a stateless intermediate stage of an IntStream
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatelessOp<E_IN> : IntPipeline<E_IN>
		{
			/// <summary>
			/// Construct a new IntStream by appending a stateless intermediate
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
		/// Base class for a stateful intermediate stage of an IntStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatefulOp<E_IN> : IntPipeline<E_IN>
		{
			/// <summary>
			/// Construct a new IntStream by appending a stateful intermediate
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
				return IntStream_Fields.True;
			}

			internal override abstract Node<Integer> opEvaluateParallel<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator, IntFunction<Integer[]> generator);
		}
	}

}