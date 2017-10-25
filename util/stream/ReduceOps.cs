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
	/// Factory for creating instances of {@code TerminalOp} that implement
	/// reductions.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class ReduceOps
	{

		private ReduceOps()
		{
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// reference values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <U> the type of the result </param>
		/// <param name="seed"> the identity element for the reduction </param>
		/// <param name="reducer"> the accumulating function that incorporates an additional
		///        input element into the result </param>
		/// <param name="combiner"> the combining function that combines two intermediate
		///        results </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U> TerminalOp<T, U> makeRef(U seed, java.util.function.BiFunction<U, ? base T, U> reducer, java.util.function.BinaryOperator<U> combiner)
		public static TerminalOp<T, U> makeRef<T, U, T1>(U seed, BiFunction<T1> reducer, BinaryOperator<U> combiner)
		{
			Objects.RequireNonNull(reducer);
			Objects.RequireNonNull(combiner);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<U> implements AccumulatingSink<T, U, ReducingSink>
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = seed;
	//			}
	//
	//			@@Override public void accept(T t)
	//			{
	//				state = reducer.apply(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				state = combiner.apply(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper();
		}

		private class ReduceOpAnonymousInnerClassHelper : ReduceOp<T, U, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper() : base(TerminalOp_Fields.StreamShape.REFERENCE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// reference values producing an optional reference result.
		/// </summary>
		/// @param <T> The type of the input elements, and the type of the result </param>
		/// <param name="operator"> The reducing function </param>
		/// <returns> A {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<T, Optional<T>> makeRef<T>(BinaryOperator<T> @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<T, java.util.Optional<T>, ReducingSink>
	//		{
	//			private boolean empty;
	//			private T state;
	//
	//			public void begin(long size)
	//			{
	//				empty = true;
	//				state = null;
	//			}
	//
	//			@@Override public void accept(T t)
	//			{
	//				if (empty)
	//				{
	//					empty = false;
	//					state = t;
	//				}
	//				else
	//				{
	//					state = @operator.apply(state, t);
	//				}
	//			}
	//
	//			@@Override public Optional<T> get()
	//			{
	//				return empty ? Optional.empty() : Optional.of(state);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				if (!other.empty)
	//					accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper2();
		}

		private class ReduceOpAnonymousInnerClassHelper2 : ReduceOp<T, Optional<T>, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper2() : base(TerminalOp_Fields.StreamShape.REFERENCE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a mutable reduce on
		/// reference values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <I> the type of the intermediate reduction result </param>
		/// <param name="collector"> a {@code Collector} defining the reduction </param>
		/// <returns> a {@code ReduceOp} implementing the reduction </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, I> TerminalOp<T, I> makeRef(Collector<? base T, I, ?> collector)
		public static TerminalOp<T, I> makeRef<T, I, T1>(Collector<T1> collector)
		{
			Supplier<I> supplier = Objects.RequireNonNull(collector).supplier();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<I, ? base T> accumulator = collector.accumulator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			BiConsumer<I, ?> accumulator = collector.Accumulator();
			BinaryOperator<I> combiner = collector.Combiner();
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<I> implements AccumulatingSink<T, I, ReducingSink>
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = supplier.get();
	//			}
	//
	//			@@Override public void accept(T t)
	//			{
	//				accumulator.accept(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				state = combiner.apply(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper3(collector);
		}

		private class ReduceOpAnonymousInnerClassHelper3 : ReduceOp<T, I, ReducingSink>
		{
			private java.util.stream.Collector<T1> Collector;

			public ReduceOpAnonymousInnerClassHelper3(java.util.stream.Collector<T1> collector) : base(TerminalOp_Fields.StreamShape.REFERENCE)
			{
				this.Collector = collector;
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}

			public override int OpFlags
			{
				get
				{
					return Collector.Characteristics().Contains(Collector_Characteristics.UNORDERED) ? StreamOpFlag.NOT_ORDERED : TerminalOp_Fields.0;
				}
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a mutable reduce on
		/// reference values.
		/// </summary>
		/// @param <T> the type of the input elements </param>
		/// @param <R> the type of the result </param>
		/// <param name="seedFactory"> a factory to produce a new base accumulator </param>
		/// <param name="accumulator"> a function to incorporate an element into an
		///        accumulator </param>
		/// <param name="reducer"> a function to combine an accumulator into another </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, R> TerminalOp<T, R> makeRef(java.util.function.Supplier<R> seedFactory, java.util.function.BiConsumer<R, ? base T> accumulator, java.util.function.BiConsumer<R,R> reducer)
		public static TerminalOp<T, R> makeRef<T, R, T1>(Supplier<R> seedFactory, BiConsumer<T1> accumulator, BiConsumer<R, R> reducer)
		{
			Objects.RequireNonNull(seedFactory);
			Objects.RequireNonNull(accumulator);
			Objects.RequireNonNull(reducer);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<R> implements AccumulatingSink<T, R, ReducingSink>
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = seedFactory.get();
	//			}
	//
	//			@@Override public void accept(T t)
	//			{
	//				accumulator.accept(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				reducer.accept(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper4();
		}

		private class ReduceOpAnonymousInnerClassHelper4 : ReduceOp<T, R, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper4() : base(TerminalOp_Fields.StreamShape.REFERENCE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code int} values.
		/// </summary>
		/// <param name="identity"> the identity for the combining function </param>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Integer, Integer> MakeInt(int identity, IntBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Integer, Integer, ReducingSink>, Sink_OfInt
	//		{
	//			private int state;
	//
	//			@@Override public void begin(long size)
	//			{
	//				state = identity;
	//			}
	//
	//			@@Override public void accept(int t)
	//			{
	//				state = @operator.applyAsInt(state, t);
	//			}
	//
	//			@@Override public Integer get()
	//			{
	//				return state;
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper5();
		}

		private class ReduceOpAnonymousInnerClassHelper5 : ReduceOp<Integer, Integer, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper5() : base(StreamShape.INT_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code int} values, producing an optional integer result.
		/// </summary>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Integer, OptionalInt> MakeInt(IntBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Integer, java.util.OptionalInt, ReducingSink>, Sink_OfInt
	//		{
	//			private boolean empty;
	//			private int state;
	//
	//			public void begin(long size)
	//			{
	//				empty = true;
	//				state = 0;
	//			}
	//
	//			@@Override public void accept(int t)
	//			{
	//				if (empty)
	//				{
	//					empty = false;
	//					state = t;
	//				}
	//				else
	//				{
	//					state = @operator.applyAsInt(state, t);
	//				}
	//			}
	//
	//			@@Override public OptionalInt get()
	//			{
	//				return empty ? OptionalInt.empty() : OptionalInt.of(state);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				if (!other.empty)
	//					accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper6();
		}

		private class ReduceOpAnonymousInnerClassHelper6 : ReduceOp<Integer, OptionalInt, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper6() : base(StreamShape.INT_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a mutable reduce on
		/// {@code int} values.
		/// </summary>
		/// @param <R> The type of the result </param>
		/// <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		/// <param name="accumulator"> a function to incorporate an int into an
		///        accumulator </param>
		/// <param name="combiner"> a function to combine an accumulator into another </param>
		/// <returns> A {@code ReduceOp} implementing the reduction </returns>
		public static TerminalOp<Integer, R> makeInt<R>(Supplier<R> supplier, ObjIntConsumer<R> accumulator, BinaryOperator<R> combiner)
		{
			Objects.RequireNonNull(supplier);
			Objects.RequireNonNull(accumulator);
			Objects.RequireNonNull(combiner);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<R> implements AccumulatingSink<Integer, R, ReducingSink>, Sink_OfInt
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = supplier.get();
	//			}
	//
	//			@@Override public void accept(int t)
	//			{
	//				accumulator.accept(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				state = combiner.apply(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper7();
		}

		private class ReduceOpAnonymousInnerClassHelper7 : ReduceOp<Integer, R, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper7() : base(StreamShape.INT_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code long} values.
		/// </summary>
		/// <param name="identity"> the identity for the combining function </param>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Long, Long> MakeLong(long identity, LongBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Long, Long, ReducingSink>, Sink_OfLong
	//		{
	//			private long state;
	//
	//			@@Override public void begin(long size)
	//			{
	//				state = identity;
	//			}
	//
	//			@@Override public void accept(long t)
	//			{
	//				state = @operator.applyAsLong(state, t);
	//			}
	//
	//			@@Override public Long get()
	//			{
	//				return state;
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper8();
		}

		private class ReduceOpAnonymousInnerClassHelper8 : ReduceOp<Long, Long, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper8() : base(StreamShape.LONG_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code long} values, producing an optional long result.
		/// </summary>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Long, OptionalLong> MakeLong(LongBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Long, java.util.OptionalLong, ReducingSink>, Sink_OfLong
	//		{
	//			private boolean empty;
	//			private long state;
	//
	//			public void begin(long size)
	//			{
	//				empty = true;
	//				state = 0;
	//			}
	//
	//			@@Override public void accept(long t)
	//			{
	//				if (empty)
	//				{
	//					empty = false;
	//					state = t;
	//				}
	//				else
	//				{
	//					state = @operator.applyAsLong(state, t);
	//				}
	//			}
	//
	//			@@Override public OptionalLong get()
	//			{
	//				return empty ? OptionalLong.empty() : OptionalLong.of(state);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				if (!other.empty)
	//					accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper9();
		}

		private class ReduceOpAnonymousInnerClassHelper9 : ReduceOp<Long, OptionalLong, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper9() : base(StreamShape.LONG_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a mutable reduce on
		/// {@code long} values.
		/// </summary>
		/// @param <R> the type of the result </param>
		/// <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		/// <param name="accumulator"> a function to incorporate an int into an
		///        accumulator </param>
		/// <param name="combiner"> a function to combine an accumulator into another </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Long, R> makeLong<R>(Supplier<R> supplier, ObjLongConsumer<R> accumulator, BinaryOperator<R> combiner)
		{
			Objects.RequireNonNull(supplier);
			Objects.RequireNonNull(accumulator);
			Objects.RequireNonNull(combiner);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<R> implements AccumulatingSink<Long, R, ReducingSink>, Sink_OfLong
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = supplier.get();
	//			}
	//
	//			@@Override public void accept(long t)
	//			{
	//				accumulator.accept(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				state = combiner.apply(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper10();
		}

		private class ReduceOpAnonymousInnerClassHelper10 : ReduceOp<Long, R, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper10() : base(StreamShape.LONG_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code double} values.
		/// </summary>
		/// <param name="identity"> the identity for the combining function </param>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Double, Double> MakeDouble(double identity, DoubleBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Double, Double, ReducingSink>, Sink_OfDouble
	//		{
	//			private double state;
	//
	//			@@Override public void begin(long size)
	//			{
	//				state = identity;
	//			}
	//
	//			@@Override public void accept(double t)
	//			{
	//				state = @operator.applyAsDouble(state, t);
	//			}
	//
	//			@@Override public Double get()
	//			{
	//				return state;
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper11();
		}

		private class ReduceOpAnonymousInnerClassHelper11 : ReduceOp<Double, Double, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper11() : base(StreamShape.DOUBLE_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a functional reduce on
		/// {@code double} values, producing an optional double result.
		/// </summary>
		/// <param name="operator"> the combining function </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Double, OptionalDouble> MakeDouble(DoubleBinaryOperator @operator)
		{
			Objects.RequireNonNull(@operator);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink implements AccumulatingSink<Double, java.util.OptionalDouble, ReducingSink>, Sink_OfDouble
	//		{
	//			private boolean empty;
	//			private double state;
	//
	//			public void begin(long size)
	//			{
	//				empty = true;
	//				state = 0;
	//			}
	//
	//			@@Override public void accept(double t)
	//			{
	//				if (empty)
	//				{
	//					empty = false;
	//					state = t;
	//				}
	//				else
	//				{
	//					state = @operator.applyAsDouble(state, t);
	//				}
	//			}
	//
	//			@@Override public OptionalDouble get()
	//			{
	//				return empty ? OptionalDouble.empty() : OptionalDouble.of(state);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				if (!other.empty)
	//					accept(other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper12();
		}

		private class ReduceOpAnonymousInnerClassHelper12 : ReduceOp<Double, OptionalDouble, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper12() : base(StreamShape.DOUBLE_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that implements a mutable reduce on
		/// {@code double} values.
		/// </summary>
		/// @param <R> the type of the result </param>
		/// <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		/// <param name="accumulator"> a function to incorporate an int into an
		///        accumulator </param>
		/// <param name="combiner"> a function to combine an accumulator into another </param>
		/// <returns> a {@code TerminalOp} implementing the reduction </returns>
		public static TerminalOp<Double, R> makeDouble<R>(Supplier<R> supplier, ObjDoubleConsumer<R> accumulator, BinaryOperator<R> combiner)
		{
			Objects.RequireNonNull(supplier);
			Objects.RequireNonNull(accumulator);
			Objects.RequireNonNull(combiner);
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class ReducingSink extends Box<R> implements AccumulatingSink<Double, R, ReducingSink>, Sink_OfDouble
	//		{
	//			@@Override public void begin(long size)
	//			{
	//				state = supplier.get();
	//			}
	//
	//			@@Override public void accept(double t)
	//			{
	//				accumulator.accept(state, t);
	//			}
	//
	//			@@Override public void combine(ReducingSink other)
	//			{
	//				state = combiner.apply(state, other.state);
	//			}
	//		}
			return new ReduceOpAnonymousInnerClassHelper13();
		}

		private class ReduceOpAnonymousInnerClassHelper13 : ReduceOp<Double, R, ReducingSink>
		{
			public ReduceOpAnonymousInnerClassHelper13() : base(StreamShape.DOUBLE_VALUE)
			{
			}

			public override ReducingSink MakeSink()
			{
				return new ReducingSink();
			}
		}

		/// <summary>
		/// A type of {@code TerminalSink} that implements an associative reducing
		/// operation on elements of type {@code T} and producing a result of type
		/// {@code R}.
		/// </summary>
		/// @param <T> the type of input element to the combining operation </param>
		/// @param <R> the result type </param>
		/// @param <K> the type of the {@code AccumulatingSink}. </param>
		private interface AccumulatingSink<T, R, K> : TerminalSink<T, R> where K : AccumulatingSink<T, R, K>
		{
			void Combine(K other);
		}

		/// <summary>
		/// State box for a single state element, used as a base class for
		/// {@code AccumulatingSink} instances
		/// </summary>
		/// @param <U> The type of the state element </param>
		private abstract class Box<U>
		{
			internal U State;

			internal Box() // Avoid creation of special accessor
			{
			}

			public virtual U Get()
			{
				return State;
			}
		}

		/// <summary>
		/// A {@code TerminalOp} that evaluates a stream pipeline and sends the
		/// output into an {@code AccumulatingSink}, which performs a reduce
		/// operation. The {@code AccumulatingSink} must represent an associative
		/// reducing operation.
		/// </summary>
		/// @param <T> the output type of the stream pipeline </param>
		/// @param <R> the result type of the reducing operation </param>
		/// @param <S> the type of the {@code AccumulatingSink} </param>
		private abstract class ReduceOp<T, R, S> : TerminalOp<T, R> where S : AccumulatingSink<T, R, S>
		{
			internal readonly StreamShape InputShape_Renamed;

			/// <summary>
			/// Create a {@code ReduceOp} of the specified stream shape which uses
			/// the specified {@code Supplier} to create accumulating sinks.
			/// </summary>
			/// <param name="shape"> The shape of the stream pipeline </param>
			internal ReduceOp(StreamShape shape)
			{
				InputShape_Renamed = shape;
			}

			public abstract S MakeSink();

			public override StreamShape InputShape()
			{
				return InputShape_Renamed;
			}

			public override R evaluateSequential<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				return helper.WrapAndCopyInto(MakeSink(), spliterator).get();
			}

			public override R evaluateParallel<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				return (new ReduceTask<>(this, helper, spliterator)).invoke().get();
			}
		}

		/// <summary>
		/// A {@code ForkJoinTask} for performing a parallel reduce operation.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class ReduceTask<P_IN, P_OUT, R, S extends AccumulatingSink<P_OUT, R, S>> extends AbstractTask<P_IN, P_OUT, S, ReduceTask<P_IN, P_OUT, R, S>>
		private sealed class ReduceTask<P_IN, P_OUT, R, S> : AbstractTask<P_IN, P_OUT, S, ReduceTask<P_IN, P_OUT, R, S>> where S : AccumulatingSink<P_OUT, R, S>
		{
			internal readonly ReduceOp<P_OUT, R, S> Op;

			internal ReduceTask(ReduceOp<P_OUT, R, S> op, PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator)
			{
				this.Op = op;
			}

			internal ReduceTask(ReduceTask<P_IN, P_OUT, R, S> parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
			{
				this.Op = parent.Op;
			}

			protected internal override ReduceTask<P_IN, P_OUT, R, S> MakeChild(Spliterator<P_IN> spliterator)
			{
				return new ReduceTask<>(this, spliterator);
			}

			protected internal override S DoLeaf()
			{
				return helper.wrapAndCopyInto(Op.MakeSink(), spliterator);
			}

			public override void onCompletion<T1>(CountedCompleter<T1> caller)
			{
				if (!Leaf)
				{
					S leftResult = leftChild.LocalResult;
					leftResult.combine(rightChild.LocalResult);
					LocalResult = leftResult;
				}
				// GC spliterator, left and right child
				base.OnCompletion(caller);
			}
		}
	}

}