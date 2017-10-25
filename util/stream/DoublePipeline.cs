using System;
using System.Diagnostics;

/*
 * Copyright (c) 2013, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// stage implementing whose elements are of type {@code double}.
	/// </summary>
	/// @param <E_IN> type of elements in the upstream source
	/// 
	/// @since 1.8 </param>
	internal abstract class DoublePipeline<E_IN> : AbstractPipeline<E_IN, Double, DoubleStream>, DoubleStream
	{

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		/// <seealso cref="StreamOpFlag"/> </param>
		internal DoublePipeline<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Double> : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Spliterator} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		/// <seealso cref="StreamOpFlag"/> </param>
		internal DoublePipeline(Spliterator<Double> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for appending an intermediate operation onto an existing
		/// pipeline.
		/// </summary>
		/// <param name="upstream"> the upstream element source. </param>
		/// <param name="opFlags"> the operation flags </param>
		internal DoublePipeline<T1>(AbstractPipeline<T1> upstream, int opFlags) : base(upstream, opFlags)
		{
		}

		/// <summary>
		/// Adapt a {@code Sink<Double> to a {@code DoubleConsumer}, ideally simply
		/// by casting.
		/// </summary>
		private static DoubleConsumer Adapt(Sink<Double> sink)
		{
			if (sink is DoubleConsumer)
			{
				return (DoubleConsumer) sink;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using DoubleStream.adapt(Sink<Double> s)");
				}
				return sink::accept;
			}
		}

		/// <summary>
		/// Adapt a {@code Spliterator<Double>} to a {@code Spliterator.OfDouble}.
		/// 
		/// @implNote
		/// The implementation attempts to cast to a Spliterator.OfDouble, and throws
		/// an exception if this cast is not possible.
		/// </summary>
		private static java.util.Spliterator_OfDouble Adapt(Spliterator<Double> s)
		{
			if (s is java.util.Spliterator_OfDouble)
			{
				return (java.util.Spliterator_OfDouble) s;
			}
			else
			{
				if (Tripwire.ENABLED)
				{
					Tripwire.Trip(typeof(AbstractPipeline), "using DoubleStream.adapt(Spliterator<Double> s)");
				}
				throw new UnsupportedOperationException("DoubleStream.adapt(Spliterator<Double> s)");
			}
		}


		// Shape-specific methods

		internal override StreamShape OutputShape
		{
			get
			{
				return StreamShape.DOUBLE_VALUE;
			}
		}

		internal override Node<Double> evaluateToNode<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<Double[]> generator)
		{
			return Nodes.CollectDouble(helper, spliterator, flattenTree);
		}

		internal override Spliterator<Double> wrap<P_IN>(PipelineHelper<Double> ph, Supplier<Spliterator<P_IN>> supplier, bool isParallel)
		{
			return new StreamSpliterators.DoubleWrappingSpliterator<>(ph, supplier, isParallel);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfDouble lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Double>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final java.util.Spliterator_OfDouble lazySpliterator(java.util.function.Supplier<? extends java.util.Spliterator<Double>> supplier)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal override java.util.Spliterator_OfDouble lazySpliterator<T1>(Supplier<T1> supplier) where ? : java.util.Spliterator<Double>
		{
			return new StreamSpliterators.DelegatingSpliterator.OfDouble((Supplier<java.util.Spliterator_OfDouble>) supplier);
		}

		internal override void ForEachWithCancel(Spliterator<Double> spliterator, Sink<Double> sink)
		{
			java.util.Spliterator_OfDouble spl = Adapt(spliterator);
			DoubleConsumer adaptedSink = Adapt(sink);
			do
			{
			} while (!sink.cancellationRequested() && spl.TryAdvance(adaptedSink));
		}

		internal override Node_Builder<Double> MakeNodeBuilder(long exactSizeIfKnown, IntFunction<Double[]> generator)
		{
			return Nodes.DoubleBuilder(exactSizeIfKnown);
		}


		// DoubleStream

		public java.util.PrimitiveIterator_OfDouble Iterator()
		{
			return Spliterators.Iterator(Spliterator());
		}

		public java.util.Spliterator_OfDouble Spliterator()
		{
			return Adapt(base.Spliterator());
		}

		// Stateless intermediate ops from DoubleStream

		public Stream<Double> Boxed()
		{
			return MapToObj(Double::valueOf);
		}

		public DoubleStream Map(DoubleUnaryOperator mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleUnaryOperator Mapper;

			public StatelessOpAnonymousInnerClassHelper(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, int NOT_DISTINCT, DoubleUnaryOperator mapper) : base(this, StreamShape.DOUBLE_VALUE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper : Sink_ChainedDouble<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(double DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsDouble(DoubleStream_Fields.t));
				}
			}
		}

		public Stream<U> mapToObj<U, T1>(DoubleFunction<T1> mapper) where T1 : U
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : ReferencePipeline.StatelessOp<Double, U>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, int NOT_DISTINCT, DoubleFunction<T1> mapper) : base(this, StreamShape.DOUBLE_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<U> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper2(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper2 : Sink_ChainedDouble<U>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper2(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<U> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(double DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.apply(DoubleStream_Fields.t));
				}
			}
		}

		public IntStream MapToInt(DoubleToIntFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : IntPipeline.StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleToIntFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, int NOT_DISTINCT, DoubleToIntFunction mapper) : base(this, StreamShape.DOUBLE_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper3(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper3 : Sink_ChainedDouble<Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper3(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(double DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsInt(DoubleStream_Fields.t));
				}
			}
		}

		public LongStream MapToLong(DoubleToLongFunction mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : LongPipeline.StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleToLongFunction Mapper;

			public StatelessOpAnonymousInnerClassHelper(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, int NOT_DISTINCT, DoubleToLongFunction mapper) : base(this, StreamShape.DOUBLE_VALUE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper4(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper4 : Sink_ChainedDouble<Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper4(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(double DoubleStream_Fields)
				{
					downstream.accept(OuterInstance.Mapper.applyAsLong(DoubleStream_Fields.t));
				}
			}
		}

		public DoubleStream flatMap<T1>(DoubleFunction<T1> mapper) where T1 : DoubleStream
		{
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, int NOT_SIZED, DoubleFunction<T1> mapper) : base(this, StreamShape.DOUBLE_VALUE, NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper5(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper5 : Sink_ChainedDouble<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper5(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(double DoubleStream_Fields)
				{
					using (DoubleStream result = OuterInstance.Mapper.apply(DoubleStream_Fields.t))
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

		public virtual DoubleStream Unordered()
		{
			if (!Ordered)
			{
				return this;
			}
			return new StatelessOpAnonymousInnerClassHelper3(this, this);
		}

		private class StatelessOpAnonymousInnerClassHelper3 : StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper3(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this) : base(this, StreamShape.DOUBLE_VALUE, StreamOpFlag.NOT_ORDERED)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return sink;
			}
		}

		public DoubleStream Filter(DoublePredicate predicate)
		{
			Objects.RequireNonNull(predicate);
			return new StatelessOpAnonymousInnerClassHelper4(this, this, predicate);
		}

		private class StatelessOpAnonymousInnerClassHelper4 : StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoublePredicate Predicate;

			public StatelessOpAnonymousInnerClassHelper4(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, DoublePredicate predicate) : base(this, StreamShape.DOUBLE_VALUE, StreamOpFlag.NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Predicate = predicate;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper6(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper6 : Sink_ChainedDouble<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper4 OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper6(StatelessOpAnonymousInnerClassHelper4 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(double DoubleStream_Fields)
				{
					if (OuterInstance.Predicate.test(DoubleStream_Fields.t))
					{
						downstream.accept(DoubleStream_Fields.t);
					}
				}
			}
		}

		public DoubleStream Peek(DoubleConsumer action)
		{
			Objects.RequireNonNull(action);
			return new StatelessOpAnonymousInnerClassHelper5(this, this, action);
		}

		private class StatelessOpAnonymousInnerClassHelper5 : StatelessOp<Double>
		{
			private readonly DoublePipeline<E_IN> OuterInstance;

			private DoubleConsumer Action;

			public StatelessOpAnonymousInnerClassHelper5(DoublePipeline<E_IN> outerInstance, java.util.stream.DoublePipeline this, DoubleConsumer action) : base(this, StreamShape.DOUBLE_VALUE, 0)
			{
				this.outerInstance = outerInstance;
				this.Action = action;
			}

			internal override Sink<Double> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedDoubleAnonymousInnerClassHelper7(this, sink);
			}

			private class Sink_ChainedDoubleAnonymousInnerClassHelper7 : Sink_ChainedDouble<Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper5 OuterInstance;

				public Sink_ChainedDoubleAnonymousInnerClassHelper7(StatelessOpAnonymousInnerClassHelper5 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(double DoubleStream_Fields)
				{
					OuterInstance.Action.accept(DoubleStream_Fields.t);
					downstream.accept(DoubleStream_Fields.t);
				}
			}
		}

		// Stateful intermediate ops from DoubleStream

		public DoubleStream Limit(long maxSize)
		{
			if (maxSize < 0)
			{
				throw new IllegalArgumentException(Convert.ToString(maxSize));
			}
			return SliceOps.MakeDouble(this, (long) 0, maxSize);
		}

		public DoubleStream Skip(long n)
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
				long limit = -1;
				return SliceOps.MakeDouble(this, n, limit);
			}
		}

		public DoubleStream Sorted()
		{
			return SortedOps.MakeDouble(this);
		}

		public DoubleStream Distinct()
		{
			// While functional and quick to implement, this approach is not very efficient.
			// An efficient version requires a double-specific map/set implementation.
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return Boxed().Distinct().MapToDouble(i => (double) i);
		}

		// Terminal ops from DoubleStream

		public virtual void ForEach(DoubleConsumer consumer)
		{
			Evaluate(ForEachOps.MakeDouble(consumer, false));
		}

		public virtual void ForEachOrdered(DoubleConsumer consumer)
		{
			Evaluate(ForEachOps.MakeDouble(consumer, DoubleStream_Fields.True));
		}

		public double Sum()
		{
			/*
			 * In the arrays allocated for the collect operation, index 0
			 * holds the high-order bits of the running sum, index 1 holds
			 * the low-order bits of the sum computed via compensated
			 * summation, and index 2 holds the simple sum used to compute
			 * the proper result if the stream contains infinite values of
			 * the same sign.
			 */
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			double[] summation = Collect(() => new double[3], (ll, d) => {Collectors.SumWithCompensation(ll, d); ll[2] += d;}, (ll, rr) => {Collectors.SumWithCompensation(ll, rr[0]); Collectors.SumWithCompensation(ll, rr[1]); ll[2] += rr[2];});

			return Collectors.ComputeFinalSum(summation);
		}

		public OptionalDouble Min()
		{
			return Reduce(Math::min);
		}

		public OptionalDouble Max()
		{
			return Reduce(Math::max);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implNote The {@code double} format can represent all
		/// consecutive integers in the range -2<sup>53</sup> to
		/// 2<sup>53</sup>. If the pipeline has more than 2<sup>53</sup>
		/// values, the divisor in the average computation will saturate at
		/// 2<sup>53</sup>, leading to additional numerical errors.
		/// </summary>
		public OptionalDouble Average()
		{
			/*
			 * In the arrays allocated for the collect operation, index 0
			 * holds the high-order bits of the running sum, index 1 holds
			 * the low-order bits of the sum computed via compensated
			 * summation, index 2 holds the number of values seen, index 3
			 * holds the simple sum.
			 */
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			double[] avg = Collect(() => new double[4], (ll, d) => {ll[2]++; Collectors.SumWithCompensation(ll, d); ll[3] += d;}, (ll, rr) => {Collectors.SumWithCompensation(ll, rr[0]); Collectors.SumWithCompensation(ll, rr[1]); ll[2] += rr[2]; ll[3] += rr[3];});
			return avg[2] > 0 ? OptionalDouble.Of(Collectors.ComputeFinalSum(avg) / avg[2]) : OptionalDouble.Empty();
		}

		public long Count()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return MapToLong(e => 1L).Sum();
		}

		public DoubleSummaryStatistics SummaryStatistics()
		{
			return Collect(DoubleSummaryStatistics::new, DoubleSummaryStatistics::accept, DoubleSummaryStatistics::combine);
		}

		public double Reduce(double identity, DoubleBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeDouble(identity, op));
		}

		public OptionalDouble Reduce(DoubleBinaryOperator op)
		{
			return Evaluate(ReduceOps.MakeDouble(op));
		}

		public R collect<R>(Supplier<R> supplier, ObjDoubleConsumer<R> accumulator, BiConsumer<R, R> combiner)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			BinaryOperator<R> @operator = (left, right) =>
			{
				combiner.Accept(left, right);
				return left;
			};
			return Evaluate(ReduceOps.MakeDouble(supplier, accumulator, @operator));
		}

		public bool AnyMatch(DoublePredicate predicate)
		{
			return Evaluate(MatchOps.MakeDouble(predicate, MatchOps.MatchKind.ANY));
		}

		public bool AllMatch(DoublePredicate predicate)
		{
			return Evaluate(MatchOps.MakeDouble(predicate, MatchOps.MatchKind.ALL));
		}

		public bool NoneMatch(DoublePredicate predicate)
		{
			return Evaluate(MatchOps.MakeDouble(predicate, MatchOps.MatchKind.NONE));
		}

		public OptionalDouble FindFirst()
		{
			return Evaluate(FindOps.MakeDouble(DoubleStream_Fields.True));
		}

		public OptionalDouble FindAny()
		{
			return Evaluate(FindOps.MakeDouble(false));
		}

		public double[] ToArray()
		{
			return Nodes.FlattenDouble((Node_OfDouble) EvaluateToArrayNode(Double[] ::new)).AsPrimitiveArray();
		}

		//

		/// <summary>
		/// Source stage of a DoubleStream
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source </param>
		internal class Head<E_IN> : DoublePipeline<E_IN>
		{
			/// <summary>
			/// Constructor for the source stage of a DoubleStream.
			/// </summary>
			/// <param name="source"> {@code Supplier<Spliterator>} describing the stream
			///               source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<Double> : base(source, sourceFlags, parallel)
			{
			}

			/// <summary>
			/// Constructor for the source stage of a DoubleStream.
			/// </summary>
			/// <param name="source"> {@code Spliterator} describing the stream source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
			internal Head(Spliterator<Double> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
			{
			}

			internal override bool OpIsStateful()
			{
				throw new UnsupportedOperationException();
			}

			internal override Sink<E_IN> OpWrapSink(int flags, Sink<Double> sink)
			{
				throw new UnsupportedOperationException();
			}

			// Optimized sequential terminal operations for the head of the pipeline

			public override void ForEach(DoubleConsumer consumer)
			{
				if (!Parallel)
				{
					Adapt(SourceStageSpliterator()).forEachRemaining(consumer);
				}
				else
				{
					base.ForEach(consumer);
				}
			}

			public override void ForEachOrdered(DoubleConsumer consumer)
			{
				if (!Parallel)
				{
					Adapt(SourceStageSpliterator()).forEachRemaining(consumer);
				}
				else
				{
					base.ForEachOrdered(consumer);
				}
			}

		}

		/// <summary>
		/// Base class for a stateless intermediate stage of a DoubleStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatelessOp<E_IN> : DoublePipeline<E_IN>
		{
			/// <summary>
			/// Construct a new DoubleStream by appending a stateless intermediate
			/// operation to an existing stream.
			/// </summary>
			/// <param name="upstream"> the upstream pipeline stage </param>
			/// <param name="inputShape"> the stream shape for the upstream pipeline stage </param>
			/// <param name="opFlags"> operation flags for the new stage </param>
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
		/// Base class for a stateful intermediate stage of a DoubleStream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source
		/// @since 1.8 </param>
		internal abstract class StatefulOp<E_IN> : DoublePipeline<E_IN>
		{
			/// <summary>
			/// Construct a new DoubleStream by appending a stateful intermediate
			/// operation to an existing stream.
			/// </summary>
			/// <param name="upstream"> the upstream pipeline stage </param>
			/// <param name="inputShape"> the stream shape for the upstream pipeline stage </param>
			/// <param name="opFlags"> operation flags for the new stage </param>
			internal StatefulOp<T1>(AbstractPipeline<T1> upstream, StreamShape inputShape, int opFlags) : base(upstream, opFlags)
			{
				Debug.Assert(upstream.OutputShape == inputShape);
			}

			internal override bool OpIsStateful()
			{
				return DoubleStream_Fields.True;
			}

			internal override abstract Node<Double> opEvaluateParallel<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator, IntFunction<Double[]> generator);
		}
	}

}