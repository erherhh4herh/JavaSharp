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
	/// Factory for instances of a short-circuiting {@code TerminalOp} that implement
	/// quantified predicate matching on the elements of a stream. Supported variants
	/// include match-all, match-any, and match-none.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class MatchOps
	{

		private MatchOps()
		{
		}

		/// <summary>
		/// Enum describing quantified match options -- all match, any match, none
		/// match.
		/// </summary>
		internal sealed class MatchKind
		{
			/// <summary>
			/// Do all elements match the predicate? </summary>
			public static readonly MatchKind ANY = new MatchKind("ANY", InnerEnum.ANY, true, true);

			/// <summary>
			/// Do any elements match the predicate? </summary>
			public static readonly MatchKind ALL = new MatchKind("ALL", InnerEnum.ALL, false, false);

			/// <summary>
			/// Do no elements match the predicate? </summary>
			public static readonly MatchKind NONE = new MatchKind("NONE", InnerEnum.NONE, true, false);

			private static readonly IList<MatchKind> valueList = new List<MatchKind>();

			static MatchKind()
			{
				valueList.Add(ANY);
				valueList.Add(ALL);
				valueList.Add(NONE);
			}

			public enum InnerEnum
			{
				ANY,
				ALL,
				NONE
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			internal readonly bool stopOnPredicateMatches;
			internal readonly bool shortCircuitResult;

			private MatchKind(boolean stopOnPredicateMatches,
			public static readonly MatchKind private MatchKind(boolean stopOnPredicateMatches = new MatchKind("private MatchKind(boolean stopOnPredicateMatches", InnerEnum.private MatchKind(boolean stopOnPredicateMatches);
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
							  boolean shortCircuitResult)
							  {
				this.stopOnPredicateMatches = stopOnPredicateMatches
				public static readonly MatchKind this.stopOnPredicateMatches = stopOnPredicateMatches = new MatchKind("this.stopOnPredicateMatches = stopOnPredicateMatches", InnerEnum.this.stopOnPredicateMatches = stopOnPredicateMatches);

				private static readonly IList<MatchKind> valueList = new List<MatchKind>();

				static MatchKind()
				{
					valueList.Add(ANY);
					valueList.Add(ALL);
					valueList.Add(NONE);
					valueList.Add(private MatchKind(boolean stopOnPredicateMatches);
					valueList.Add(this.stopOnPredicateMatches = stopOnPredicateMatches);
				}

				public enum InnerEnum
				{
					ANY,
					ALL,
					NONE,
					private MatchKind(boolean stopOnPredicateMatches,
					this.stopOnPredicateMatches = stopOnPredicateMatches
				}

				private readonly string nameValue;
				private readonly int ordinalValue;
				private readonly InnerEnum innerEnumValue;
				private static int nextOrdinal = 0;
				this.shortCircuitResult = shortCircuitResult
				public static readonly MatchKind this.shortCircuitResult = shortCircuitResult = new MatchKind("this.shortCircuitResult = shortCircuitResult", InnerEnum.this.shortCircuitResult = shortCircuitResult);

				private static readonly IList<MatchKind> valueList = new List<MatchKind>();

				static MatchKind()
				{
					valueList.Add(ANY);
					valueList.Add(ALL);
					valueList.Add(NONE);
					valueList.Add(private MatchKind(boolean stopOnPredicateMatches);
					valueList.Add(this.stopOnPredicateMatches = stopOnPredicateMatches);
					valueList.Add(this.shortCircuitResult = shortCircuitResult);
				}

				public enum InnerEnum
				{
					ANY,
					ALL,
					NONE,
					private MatchKind(boolean stopOnPredicateMatches,
					this.stopOnPredicateMatches = stopOnPredicateMatches,
					this.shortCircuitResult = shortCircuitResult
				}

				private readonly string nameValue;
				private readonly int ordinalValue;
				private readonly InnerEnum innerEnumValue;
				private static int nextOrdinal = 0;
							  }

			public static IList<MatchKind> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static MatchKind valueOf(string name)
			{
				foreach (MatchKind enumInstance in MatchKind.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// Constructs a quantified predicate matcher for a Stream.
		/// </summary>
		/// @param <T> the type of stream elements </param>
		/// <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		/// <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		/// <returns> a {@code TerminalOp} implementing the desired quantified match
		///         criteria </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> TerminalOp<T, Boolean> makeRef(java.util.function.Predicate<? base T> predicate, MatchKind matchKind)
		public static TerminalOp<T, Boolean> makeRef<T, T1>(Predicate<T1> predicate, MatchKind matchKind)
		{
			Objects.RequireNonNull(predicate);
			Objects.RequireNonNull(matchKind);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class MatchSink extends BooleanTerminalSink<T>
	//		{
	//			MatchSink()
	//			{
	//				base(matchKind);
	//			}
	//
	//			@@Override public void accept(T t)
	//			{
	//				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	//				{
	//					stop = true;
	//					value = matchKind.shortCircuitResult;
	//				}
	//			}
	//		}

			return new MatchOp<>(StreamShape.REFERENCE, matchKind, MatchSink::new);
		}

		/// <summary>
		/// Constructs a quantified predicate matcher for an {@code IntStream}.
		/// </summary>
		/// <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		/// <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		/// <returns> a {@code TerminalOp} implementing the desired quantified match
		///         criteria </returns>
		public static TerminalOp<Integer, Boolean> MakeInt(IntPredicate predicate, MatchKind matchKind)
		{
			Objects.RequireNonNull(predicate);
			Objects.RequireNonNull(matchKind);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class MatchSink extends BooleanTerminalSink<Integer> implements Sink_OfInt
	//		{
	//			MatchSink()
	//			{
	//				base(matchKind);
	//			}
	//
	//			@@Override public void accept(int t)
	//			{
	//				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	//				{
	//					stop = true;
	//					value = matchKind.shortCircuitResult;
	//				}
	//			}
	//		}

			return new MatchOp<>(StreamShape.INT_VALUE, matchKind, MatchSink::new);
		}

		/// <summary>
		/// Constructs a quantified predicate matcher for a {@code LongStream}.
		/// </summary>
		/// <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		/// <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		/// <returns> a {@code TerminalOp} implementing the desired quantified match
		///         criteria </returns>
		public static TerminalOp<Long, Boolean> MakeLong(LongPredicate predicate, MatchKind matchKind)
		{
			Objects.RequireNonNull(predicate);
			Objects.RequireNonNull(matchKind);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class MatchSink extends BooleanTerminalSink<Long> implements Sink_OfLong
	//		{
	//
	//			MatchSink()
	//			{
	//				base(matchKind);
	//			}
	//
	//			@@Override public void accept(long t)
	//			{
	//				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	//				{
	//					stop = true;
	//					value = matchKind.shortCircuitResult;
	//				}
	//			}
	//		}

			return new MatchOp<>(StreamShape.LONG_VALUE, matchKind, MatchSink::new);
		}

		/// <summary>
		/// Constructs a quantified predicate matcher for a {@code DoubleStream}.
		/// </summary>
		/// <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		/// <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		/// <returns> a {@code TerminalOp} implementing the desired quantified match
		///         criteria </returns>
		public static TerminalOp<Double, Boolean> MakeDouble(DoublePredicate predicate, MatchKind matchKind)
		{
			Objects.RequireNonNull(predicate);
			Objects.RequireNonNull(matchKind);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class MatchSink extends BooleanTerminalSink<Double> implements Sink_OfDouble
	//		{
	//
	//			MatchSink()
	//			{
	//				base(matchKind);
	//			}
	//
	//			@@Override public void accept(double t)
	//			{
	//				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	//				{
	//					stop = true;
	//					value = matchKind.shortCircuitResult;
	//				}
	//			}
	//		}

			return new MatchOp<>(StreamShape.DOUBLE_VALUE, matchKind, MatchSink::new);
		}

		/// <summary>
		/// A short-circuiting {@code TerminalOp} that evaluates a predicate on the
		/// elements of a stream and determines whether all, any or none of those
		/// elements match the predicate.
		/// </summary>
		/// @param <T> the output type of the stream pipeline </param>
		private sealed class MatchOp<T> : TerminalOp<T, Boolean>
		{
			internal readonly StreamShape InputShape_Renamed;
			internal readonly MatchKind MatchKind;
			internal readonly Supplier<BooleanTerminalSink<T>> SinkSupplier;

			/// <summary>
			/// Constructs a {@code MatchOp}.
			/// </summary>
			/// <param name="shape"> the output shape of the stream pipeline </param>
			/// <param name="matchKind"> the kind of quantified match (all, any, none) </param>
			/// <param name="sinkSupplier"> {@code Supplier} for a {@code Sink} of the
			///        appropriate shape which implements the matching operation </param>
			internal MatchOp(StreamShape shape, MatchKind matchKind, Supplier<BooleanTerminalSink<T>> sinkSupplier)
			{
				this.InputShape_Renamed = shape;
				this.MatchKind = matchKind;
				this.SinkSupplier = sinkSupplier;
			}

			public override int OpFlags
			{
				get
				{
					return StreamOpFlag.IS_SHORT_CIRCUIT | StreamOpFlag.NOT_ORDERED;
				}
			}

			public override StreamShape InputShape()
			{
				return InputShape_Renamed;
			}

			public override Boolean evaluateSequential<S>(PipelineHelper<T> helper, Spliterator<S> spliterator)
			{
				return helper.WrapAndCopyInto(SinkSupplier.Get(), spliterator).AndClearState;
			}

			public override Boolean evaluateParallel<S>(PipelineHelper<T> helper, Spliterator<S> spliterator)
			{
				// Approach for parallel implementation:
				// - Decompose as per usual
				// - run match on leaf chunks, call result "b"
				// - if b == matchKind.shortCircuitOn, complete early and return b
				// - else if we complete normally, return !shortCircuitOn

				return (new MatchTask<>(this, helper, spliterator)).invoke();
			}
		}

		/// <summary>
		/// Boolean specific terminal sink to avoid the boxing costs when returning
		/// results.  Subclasses implement the shape-specific functionality.
		/// </summary>
		/// @param <T> The output type of the stream pipeline </param>
		private abstract class BooleanTerminalSink<T> : Sink<T>
		{
			internal bool Stop;
			internal bool Value;

			internal BooleanTerminalSink(MatchKind matchKind)
			{
				Value = !matchKind.shortCircuitResult;
			}

			public virtual bool AndClearState
			{
				get
				{
					return Value;
				}
			}

			public override bool CancellationRequested()
			{
				return Stop;
			}
		}

		/// <summary>
		/// ForkJoinTask implementation to implement a parallel short-circuiting
		/// quantified match
		/// </summary>
		/// @param <P_IN> the type of source elements for the pipeline </param>
		/// @param <P_OUT> the type of output elements for the pipeline </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class MatchTask<P_IN, P_OUT> extends AbstractShortCircuitTask<P_IN, P_OUT, Boolean, MatchTask<P_IN, P_OUT>>
		private sealed class MatchTask<P_IN, P_OUT> : AbstractShortCircuitTask<P_IN, P_OUT, Boolean, MatchTask<P_IN, P_OUT>>
		{
			internal readonly MatchOp<P_OUT> Op;

			/// <summary>
			/// Constructor for root node
			/// </summary>
			internal MatchTask(MatchOp<P_OUT> op, PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator)
			{
				this.Op = op;
			}

			/// <summary>
			/// Constructor for non-root node
			/// </summary>
			internal MatchTask(MatchTask<P_IN, P_OUT> parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
			{
				this.Op = parent.Op;
			}

			protected internal override MatchTask<P_IN, P_OUT> MakeChild(Spliterator<P_IN> spliterator)
			{
				return new MatchTask<>(this, spliterator);
			}

			protected internal override Boolean DoLeaf()
			{
				bool b = helper.wrapAndCopyInto(Op.SinkSupplier.Get(), spliterator).AndClearState;
				if (b == Op.MatchKind.shortCircuitResult)
				{
					ShortCircuit(b);
				}
				return null;
			}

			protected internal override Boolean EmptyResult
			{
				get
				{
					return !Op.MatchKind.shortCircuitResult;
				}
			}
		}
	}


}