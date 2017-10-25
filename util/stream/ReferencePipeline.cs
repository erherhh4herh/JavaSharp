using System;
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
	/// Abstract base class for an intermediate pipeline stage or pipeline source
	/// stage implementing whose elements are of type {@code U}.
	/// </summary>
	/// @param <P_IN> type of elements in the upstream source </param>
	/// @param <P_OUT> type of elements in produced by this stage
	/// 
	/// @since 1.8 </param>
	internal abstract class ReferencePipeline<P_IN, P_OUT> : AbstractPipeline<P_IN, P_OUT, Stream<P_OUT>>, Stream<P_OUT>
	{

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal ReferencePipeline<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<T1> : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Spliterator} describing the stream source </param>
		/// <param name="sourceFlags"> The source flags for the stream source, described in
		///        <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal ReferencePipeline<T1>(Spliterator<T1> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
		{
		}

		/// <summary>
		/// Constructor for appending an intermediate operation onto an existing
		/// pipeline.
		/// </summary>
		/// <param name="upstream"> the upstream element source. </param>
		internal ReferencePipeline<T1>(AbstractPipeline<T1> upstream, int opFlags) : base(upstream, opFlags)
		{
		}

		// Shape-specific methods

		internal override StreamShape OutputShape
		{
			get
			{
				return StreamShape.REFERENCE;
			}
		}

		internal override Node<P_OUT> evaluateToNode<P_IN>(PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<P_OUT[]> generator)
		{
			return Nodes.Collect(helper, spliterator, flattenTree, generator);
		}

		internal override Spliterator<P_OUT> wrap<P_IN>(PipelineHelper<P_OUT> ph, Supplier<Spliterator<P_IN>> supplier, bool isParallel)
		{
			return new StreamSpliterators.WrappingSpliterator<>(ph, supplier, isParallel);
		}

		internal override Spliterator<P_OUT> LazySpliterator(Supplier<T1> supplier) where T1 : java.util.Spliterator<P_OUT>
		{
			return new StreamSpliterators.DelegatingSpliterator<>(supplier);
		}

		internal override void ForEachWithCancel(Spliterator<P_OUT> spliterator, Sink<P_OUT> sink)
		{
			do
			{
			} while (!sink.cancellationRequested() && spliterator.TryAdvance(sink));
		}

		internal override Node_Builder<P_OUT> MakeNodeBuilder(long exactSizeIfKnown, IntFunction<P_OUT[]> generator)
		{
			return Nodes.Builder(exactSizeIfKnown, generator);
		}


		// BaseStream

		public override IEnumerator<P_OUT> Iterator()
		{
			return Spliterators.Iterator(Spliterator());
		}


		// Stream

		// Stateless intermediate operations from Stream

		public override Stream<P_OUT> Unordered()
		{
			if (!Ordered)
			{
				return this;
			}
			return new StatelessOpAnonymousInnerClassHelper(this, this);
		}

		private class StatelessOpAnonymousInnerClassHelper : StatelessOp<P_OUT, P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			public StatelessOpAnonymousInnerClassHelper(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this) : base(this, StreamShape.REFERENCE, StreamOpFlag.NOT_ORDERED)
			{
				this.outerInstance = outerInstance;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<P_OUT> sink)
			{
				return sink;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final Stream<P_OUT> filter(java.util.function.Predicate<? base P_OUT> predicate)
		public override Stream<P_OUT> Filter(Predicate<T1> predicate)
		{
			Objects.RequireNonNull(predicate);
			return new StatelessOpAnonymousInnerClassHelper2(this, this, predicate);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : StatelessOp<P_OUT, P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Predicate<T1> Predicate;

			public StatelessOpAnonymousInnerClassHelper2(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, Predicate<T1> predicate) : base(this, StreamShape.REFERENCE, StreamOpFlag.NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Predicate = predicate;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<P_OUT> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper : Sink_ChainedReference<P_OUT, P_OUT>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<P_OUT> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(P_OUT u)
				{
					if (OuterInstance.Predicate.test(u))
					{
						downstream.accept(u);
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final <R> Stream<R> map(java.util.function.Function<? base P_OUT, ? extends R> mapper)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final <R> Stream<R> map(java.util.function.Function<? base P_OUT, ? extends R> mapper)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public override Stream<R> map<R, T1>(Function<T1> mapper) where ? : R
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper3(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper3 : StatelessOp<P_OUT, R>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Function<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper3(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_DISTINCT, Function<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<R> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper2(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper2 : Sink_ChainedReference<P_OUT, R>
			{
				private readonly StatelessOpAnonymousInnerClassHelper3 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper2(StatelessOpAnonymousInnerClassHelper3 outerInstance, java.util.stream.Sink<R> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(P_OUT u)
				{
					downstream.accept(OuterInstance.Mapper.apply(u));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final IntStream mapToInt(java.util.function.ToIntFunction<? base P_OUT> mapper)
		public override IntStream mapToInt<T1>(ToIntFunction<T1> mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : IntPipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private ToIntFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_DISTINCT, ToIntFunction<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_DISTINCT)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper3(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper3 : Sink_ChainedReference<P_OUT, Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper3(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(P_OUT u)
				{
					downstream.accept(OuterInstance.Mapper.applyAsInt(u));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final LongStream mapToLong(java.util.function.ToLongFunction<? base P_OUT> mapper)
		public override LongStream mapToLong<T1>(ToLongFunction<T1> mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : LongPipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private ToLongFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_DISTINCT, ToLongFunction<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper4(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper4 : Sink_ChainedReference<P_OUT, Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper4(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(P_OUT u)
				{
					downstream.accept(OuterInstance.Mapper.applyAsLong(u));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final DoubleStream mapToDouble(java.util.function.ToDoubleFunction<? base P_OUT> mapper)
		public override DoubleStream mapToDouble<T1>(ToDoubleFunction<T1> mapper)
		{
			Objects.RequireNonNull(mapper);
			return new StatelessOpAnonymousInnerClassHelper(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper : DoublePipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private ToDoubleFunction<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_DISTINCT, ToDoubleFunction<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_DISTINCT)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper5(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper5 : Sink_ChainedReference<P_OUT, Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper5(StatelessOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(P_OUT u)
				{
					downstream.accept(OuterInstance.Mapper.applyAsDouble(u));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final <R> Stream<R> flatMap(java.util.function.Function<? base P_OUT, ? extends Stream<? extends R>> mapper)
		public override Stream<R> flatMap<R, T1>(Function<T1> mapper) where T1 : Stream<T1 extends R>
		{
			Objects.RequireNonNull(mapper);
			// We can do better than this, by polling cancellationRequested when stream is infinite
			return new StatelessOpAnonymousInnerClassHelper4(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper4 : StatelessOp<P_OUT, R>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Function<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper4(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_SIZED, Function<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_SIZED)
			{
				this.outerInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<R> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper6(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper6 : Sink_ChainedReference<P_OUT, R>
			{
				private readonly StatelessOpAnonymousInnerClassHelper4 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper6(StatelessOpAnonymousInnerClassHelper4 outerInstance, java.util.stream.Sink<R> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(P_OUT u)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: try (Stream<? extends R> result = mapper.apply(u))
					using (Stream<?> result = OuterInstance.Mapper.apply(u))
					{
						// We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
						if (result != null)
						{
							result.sequential().forEach(downstream);
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final IntStream flatMapToInt(java.util.function.Function<? base P_OUT, ? extends IntStream> mapper)
		public override IntStream flatMapToInt<T1>(Function<T1> mapper) where T1 : IntStream
		{
			Objects.RequireNonNull(mapper);
			// We can do better than this, by polling cancellationRequested when stream is infinite
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : IntPipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Function<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_SIZED, Function<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_SIZED)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Integer> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper7(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper7 : Sink_ChainedReference<P_OUT, Integer>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper7(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Integer> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					downstreamAsInt = downstream::accept;
				}

				internal IntConsumer downstreamAsInt;
				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(P_OUT u)
				{
					using (IntStream result = OuterInstance.Mapper.apply(u))
					{
						// We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
						if (result != null)
						{
							result.sequential().forEach(downstreamAsInt);
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final DoubleStream flatMapToDouble(java.util.function.Function<? base P_OUT, ? extends DoubleStream> mapper)
		public override DoubleStream flatMapToDouble<T1>(Function<T1> mapper) where T1 : DoubleStream
		{
			Objects.RequireNonNull(mapper);
			// We can do better than this, by polling cancellationRequested when stream is infinite
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : DoublePipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Function<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_SIZED, Function<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_SIZED)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Double> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper8(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper8 : Sink_ChainedReference<P_OUT, Double>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper8(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Double> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					downstreamAsDouble = downstream::accept;
				}

				internal DoubleConsumer downstreamAsDouble;
				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(P_OUT u)
				{
					using (DoubleStream result = OuterInstance.Mapper.apply(u))
					{
						// We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
						if (result != null)
						{
							result.sequential().forEach(downstreamAsDouble);
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final LongStream flatMapToLong(java.util.function.Function<? base P_OUT, ? extends LongStream> mapper)
		public override LongStream flatMapToLong<T1>(Function<T1> mapper) where T1 : LongStream
		{
			Objects.RequireNonNull(mapper);
			// We can do better than this, by polling cancellationRequested when stream is infinite
			return new StatelessOpAnonymousInnerClassHelper2(this, this, StreamOpFlag.NOT_SORTED | StreamOpFlag.NOT_DISTINCT | StreamOpFlag.NOT_SIZED, mapper);
		}

		private class StatelessOpAnonymousInnerClassHelper2 : LongPipeline.StatelessOp<P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Function<T1> Mapper;

			public StatelessOpAnonymousInnerClassHelper2(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, int NOT_SIZED, Function<T1> mapper) : base(this, StreamShape.REFERENCE, NOT_SIZED)
			{
				this.OuterInstance = outerInstance;
				this.Mapper = mapper;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<Long> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper9(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper9 : Sink_ChainedReference<P_OUT, Long>
			{
				private readonly StatelessOpAnonymousInnerClassHelper2 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper9(StatelessOpAnonymousInnerClassHelper2 outerInstance, java.util.stream.Sink<Long> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
					downstreamAsLong = downstream::accept;
				}

				internal LongConsumer downstreamAsLong;
				public override void Begin(long size)
				{
					downstream.begin(-1);
				}

				public override void Accept(P_OUT u)
				{
					using (LongStream result = OuterInstance.Mapper.apply(u))
					{
						// We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
						if (result != null)
						{
							result.sequential().forEach(downstreamAsLong);
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final Stream<P_OUT> peek(java.util.function.Consumer<? base P_OUT> action)
		public override Stream<P_OUT> Peek(Consumer<T1> action)
		{
			Objects.RequireNonNull(action);
			return new StatelessOpAnonymousInnerClassHelper5(this, this, action);
		}

		private class StatelessOpAnonymousInnerClassHelper5 : StatelessOp<P_OUT, P_OUT>
		{
			private readonly ReferencePipeline<P_IN, P_OUT> OuterInstance;

			private Consumer<T1> Action;

			public StatelessOpAnonymousInnerClassHelper5(ReferencePipeline<P_IN, P_OUT> outerInstance, java.util.stream.ReferencePipeline this, Consumer<T1> action) : base(this, StreamShape.REFERENCE, 0)
			{
				this.outerInstance = outerInstance;
				this.Action = action;
			}

			internal override Sink<P_OUT> OpWrapSink(int flags, Sink<P_OUT> sink)
			{
				return new Sink_ChainedReferenceAnonymousInnerClassHelper10(this, sink);
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper10 : Sink_ChainedReference<P_OUT, P_OUT>
			{
				private readonly StatelessOpAnonymousInnerClassHelper5 OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper10(StatelessOpAnonymousInnerClassHelper5 outerInstance, java.util.stream.Sink<P_OUT> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				public override void Accept(P_OUT u)
				{
					OuterInstance.Action.accept(u);
					downstream.accept(u);
				}
			}
		}

		// Stateful intermediate operations from Stream

		public override Stream<P_OUT> Distinct()
		{
			return DistinctOps.MakeRef(this);
		}

		public override Stream<P_OUT> Sorted()
		{
			return SortedOps.MakeRef(this);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final Stream<P_OUT> sorted(java.util.Comparator<? base P_OUT> comparator)
		public override Stream<P_OUT> Sorted(IComparer<T1> comparator)
		{
			return SortedOps.MakeRef(this, comparator);
		}

		public override Stream<P_OUT> Limit(long maxSize)
		{
			if (maxSize < 0)
			{
				throw new IllegalArgumentException(Convert.ToString(maxSize));
			}
			return SliceOps.MakeRef(this, 0, maxSize);
		}

		public override Stream<P_OUT> Skip(long n)
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
				return SliceOps.MakeRef(this, n, -1);
			}
		}

		// Terminal operations from Stream

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base P_OUT> action)
		public override void forEach<T1>(Consumer<T1> action)
		{
			Evaluate(ForEachOps.MakeRef(action, false));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachOrdered(java.util.function.Consumer<? base P_OUT> action)
		public override void forEachOrdered<T1>(Consumer<T1> action)
		{
			Evaluate(ForEachOps.MakeRef(action, Stream_Fields.True));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final <A> A[] toArray(java.util.function.IntFunction<A[]> generator)
		public override A[] toArray<A>(IntFunction<A[]> generator)
			// Since A has no relation to U (not possible to declare that A is an upper bound of U)
			// there will be no static type checking.
			// Therefore use a raw type and assume A == U rather than propagating the separation of A and U
			// throughout the code-base.
			// The runtime type of U is never checked for equality with the component type of the runtime type of A[].
			// Runtime checking will be performed when an element is stored in A[], thus if A is not a
			// super type of U an ArrayStoreException will be thrown.
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.function.IntFunction rawGenerator = (java.util.function.IntFunction) generator;
			IntFunction rawGenerator = (IntFunction) generator;
			return (A[]) Nodes.Flatten(EvaluateToArrayNode(rawGenerator), rawGenerator).AsArray(rawGenerator);
		}

		public override Object[] ToArray()
		{
			return ToArray(Object[] ::new);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final boolean anyMatch(java.util.function.Predicate<? base P_OUT> predicate)
		public override bool anyMatch<T1>(Predicate<T1> predicate)
		{
			return Evaluate(MatchOps.MakeRef(predicate, MatchOps.MatchKind.ANY));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final boolean allMatch(java.util.function.Predicate<? base P_OUT> predicate)
		public override bool allMatch<T1>(Predicate<T1> predicate)
		{
			return Evaluate(MatchOps.MakeRef(predicate, MatchOps.MatchKind.ALL));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final boolean noneMatch(java.util.function.Predicate<? base P_OUT> predicate)
		public override bool noneMatch<T1>(Predicate<T1> predicate)
		{
			return Evaluate(MatchOps.MakeRef(predicate, MatchOps.MatchKind.NONE));
		}

		public override Optional<P_OUT> FindFirst()
		{
			return Evaluate(FindOps.MakeRef(Stream_Fields.True));
		}

		public override Optional<P_OUT> FindAny()
		{
			return Evaluate(FindOps.MakeRef(false));
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public final P_OUT reduce(final P_OUT identity, final java.util.function.BinaryOperator<P_OUT> accumulator)
		public override P_OUT Reduce(P_OUT identity, BinaryOperator<P_OUT> accumulator)
		{
			return Evaluate(ReduceOps.MakeRef(identity, accumulator, accumulator));
		}

		public override Optional<P_OUT> Reduce(BinaryOperator<P_OUT> accumulator)
		{
			return Evaluate(ReduceOps.MakeRef(accumulator));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final <R> R reduce(R identity, java.util.function.BiFunction<R, ? base P_OUT, R> accumulator, java.util.function.BinaryOperator<R> combiner)
		public override R reduce<R, T1>(R identity, BiFunction<T1> accumulator, BinaryOperator<R> combiner)
		{
			return Evaluate(ReduceOps.MakeRef(identity, accumulator, combiner));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final <R, A> R collect(Collector<? base P_OUT, A, R> collector)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override R collect<R, A, T1>(Collector<T1> collector)
		{
			A container;
			if (Parallel && (collector.Characteristics().Contains(Collector_Characteristics.CONCURRENT)) && (!Ordered || collector.Characteristics().Contains(Collector_Characteristics.UNORDERED)))
			{
				container = collector.Supplier().Get();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<A, ? base P_OUT> accumulator = collector.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiConsumer<A, ?> accumulator = collector.Accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				ForEach(u => accumulator.accept(container, u));
			}
			else
			{
				container = Evaluate(ReduceOps.MakeRef(collector));
			}
			return collector.Characteristics().Contains(Collector_Characteristics.IDENTITY_FINISH) ? (R) container : collector.Finisher().Apply(container);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final <R> R collect(java.util.function.Supplier<R> supplier, java.util.function.BiConsumer<R, ? base P_OUT> accumulator, java.util.function.BiConsumer<R, R> combiner)
		public override R collect<R, T1>(Supplier<R> supplier, BiConsumer<T1> accumulator, BiConsumer<R, R> combiner)
		{
			return Evaluate(ReduceOps.MakeRef(supplier, accumulator, combiner));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final java.util.Optional<P_OUT> max(java.util.Comparator<? base P_OUT> comparator)
		public override Optional<P_OUT> Max(IComparer<T1> comparator)
		{
			return Reduce(BinaryOperator.maxBy(comparator));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public final java.util.Optional<P_OUT> min(java.util.Comparator<? base P_OUT> comparator)
		public override Optional<P_OUT> Min(IComparer<T1> comparator)
		{
			return Reduce(BinaryOperator.minBy(comparator));

		}

		public override long Count()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return MapToLong(e => 1L).Sum();
		}


		//

		/// <summary>
		/// Source stage of a ReferencePipeline.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source </param>
		/// @param <E_OUT> type of elements in produced by this stage
		/// @since 1.8 </param>
		internal class Head<E_IN, E_OUT> : ReferencePipeline<E_IN, E_OUT>
		{
			/// <summary>
			/// Constructor for the source stage of a Stream.
			/// </summary>
			/// <param name="source"> {@code Supplier<Spliterator>} describing the stream
			///               source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			internal Head<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<T1> : base(source, sourceFlags, parallel)
			{
			}

			/// <summary>
			/// Constructor for the source stage of a Stream.
			/// </summary>
			/// <param name="source"> {@code Spliterator} describing the stream source </param>
			/// <param name="sourceFlags"> the source flags for the stream source, described
			///                    in <seealso cref="StreamOpFlag"/> </param>
			internal Head<T1>(Spliterator<T1> source, int sourceFlags, bool parallel) : base(source, sourceFlags, parallel)
			{
			}

			internal override bool OpIsStateful()
			{
				throw new UnsupportedOperationException();
			}

			internal override Sink<E_IN> OpWrapSink(int flags, Sink<E_OUT> sink)
			{
				throw new UnsupportedOperationException();
			}

			// Optimized sequential terminal operations for the head of the pipeline

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E_OUT> action)
			public override void forEach<T1>(Consumer<T1> action)
			{
				if (!Parallel)
				{
					SourceStageSpliterator().forEachRemaining(action);
				}
				else
				{
					base.ForEach(action);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachOrdered(java.util.function.Consumer<? base E_OUT> action)
			public override void forEachOrdered<T1>(Consumer<T1> action)
			{
				if (!Parallel)
				{
					SourceStageSpliterator().forEachRemaining(action);
				}
				else
				{
					base.ForEachOrdered(action);
				}
			}
		}

		/// <summary>
		/// Base class for a stateless intermediate stage of a Stream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source </param>
		/// @param <E_OUT> type of elements in produced by this stage
		/// @since 1.8 </param>
		internal abstract class StatelessOp<E_IN, E_OUT> : ReferencePipeline<E_IN, E_OUT>
		{
			/// <summary>
			/// Construct a new Stream by appending a stateless intermediate
			/// operation to an existing stream.
			/// </summary>
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
		/// Base class for a stateful intermediate stage of a Stream.
		/// </summary>
		/// @param <E_IN> type of elements in the upstream source </param>
		/// @param <E_OUT> type of elements in produced by this stage
		/// @since 1.8 </param>
		internal abstract class StatefulOp<E_IN, E_OUT> : ReferencePipeline<E_IN, E_OUT>
		{
			/// <summary>
			/// Construct a new Stream by appending a stateful intermediate operation
			/// to an existing stream. </summary>
			/// <param name="upstream"> The upstream pipeline stage </param>
			/// <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			/// <param name="opFlags"> Operation flags for the new stage </param>
			internal StatefulOp<T1>(AbstractPipeline<T1> upstream, StreamShape inputShape, int opFlags) : base(upstream, opFlags)
			{
				Debug.Assert(upstream.OutputShape == inputShape);
			}

			internal override bool OpIsStateful()
			{
				return Stream_Fields.True;
			}

			internal override abstract Node<E_OUT> opEvaluateParallel<P_IN>(PipelineHelper<E_OUT> helper, Spliterator<P_IN> spliterator, IntFunction<E_OUT[]> generator);
		}
	}

}