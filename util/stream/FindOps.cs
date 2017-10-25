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
	/// Factory for instances of a short-circuiting {@code TerminalOp} that searches
	/// for an element in a stream pipeline, and terminates when it finds one.
	/// Supported variants include find-first (find the first element in the
	/// encounter order) and find-any (find any element, may not be the first in
	/// encounter order.)
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class FindOps
	{

		private FindOps()
		{
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} for streams of objects.
		/// </summary>
		/// @param <T> the type of elements of the stream </param>
		/// <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		///        first element in the encounter order </param>
		/// <returns> a {@code TerminalOp} implementing the find operation </returns>
		public static TerminalOp<T, Optional<T>> makeRef<T>(bool mustFindFirst)
		{
			return new FindOp<>(mustFindFirst, StreamShape.REFERENCE, Optional.Empty(), Optional::isPresent, FindSink.OfRef::new);
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} for streams of ints.
		/// </summary>
		/// <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		///        first element in the encounter order </param>
		/// <returns> a {@code TerminalOp} implementing the find operation </returns>
		public static TerminalOp<Integer, OptionalInt> MakeInt(bool mustFindFirst)
		{
			return new FindOp<>(mustFindFirst, StreamShape.INT_VALUE, OptionalInt.Empty(), OptionalInt::isPresent, FindSink.OfInt::new);
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} for streams of longs.
		/// </summary>
		/// <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		///        first element in the encounter order </param>
		/// <returns> a {@code TerminalOp} implementing the find operation </returns>
		public static TerminalOp<Long, OptionalLong> MakeLong(bool mustFindFirst)
		{
			return new FindOp<>(mustFindFirst, StreamShape.LONG_VALUE, OptionalLong.Empty(), OptionalLong::isPresent, FindSink.OfLong::new);
		}

		/// <summary>
		/// Constructs a {@code FindOp} for streams of doubles.
		/// </summary>
		/// <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		///        first element in the encounter order </param>
		/// <returns> a {@code TerminalOp} implementing the find operation </returns>
		public static TerminalOp<Double, OptionalDouble> MakeDouble(bool mustFindFirst)
		{
			return new FindOp<>(mustFindFirst, StreamShape.DOUBLE_VALUE, OptionalDouble.Empty(), OptionalDouble::isPresent, FindSink.OfDouble::new);
		}

		/// <summary>
		/// A short-circuiting {@code TerminalOp} that searches for an element in a
		/// stream pipeline, and terminates when it finds one.  Implements both
		/// find-first (find the first element in the encounter order) and find-any
		/// (find any element, may not be the first in encounter order.)
		/// </summary>
		/// @param <T> the output type of the stream pipeline </param>
		/// @param <O> the result type of the find operation, typically an optional
		///        type </param>
		private sealed class FindOp<T, O> : TerminalOp<T, O>
		{
			internal readonly StreamShape Shape;
			internal readonly bool MustFindFirst;
			internal readonly O EmptyValue;
			internal readonly Predicate<O> PresentPredicate;
			internal readonly Supplier<TerminalSink<T, O>> SinkSupplier;

			/// <summary>
			/// Constructs a {@code FindOp}.
			/// </summary>
			/// <param name="mustFindFirst"> if true, must find the first element in
			///        encounter order, otherwise can find any element </param>
			/// <param name="shape"> stream shape of elements to search </param>
			/// <param name="emptyValue"> result value corresponding to "found nothing" </param>
			/// <param name="presentPredicate"> {@code Predicate} on result value
			///        corresponding to "found something" </param>
			/// <param name="sinkSupplier"> supplier for a {@code TerminalSink} implementing
			///        the matching functionality </param>
			internal FindOp(bool mustFindFirst, StreamShape shape, O emptyValue, Predicate<O> presentPredicate, Supplier<TerminalSink<T, O>> sinkSupplier)
			{
				this.MustFindFirst = mustFindFirst;
				this.Shape = shape;
				this.EmptyValue = emptyValue;
				this.PresentPredicate = presentPredicate;
				this.SinkSupplier = sinkSupplier;
			}

			public override int OpFlags
			{
				get
				{
					return StreamOpFlag.IS_SHORT_CIRCUIT | (MustFindFirst ? TerminalOp_Fields.0 : StreamOpFlag.NOT_ORDERED);
				}
			}

			public override StreamShape InputShape()
			{
				return Shape;
			}

			public override O evaluateSequential<S>(PipelineHelper<T> helper, Spliterator<S> spliterator)
			{
				O result = helper.WrapAndCopyInto(SinkSupplier.Get(), spliterator).get();
				return result != null ? result : EmptyValue;
			}

			public override O evaluateParallel<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				return (new FindTask<>(this, helper, spliterator)).invoke();
			}
		}

		/// <summary>
		/// Implementation of @{code TerminalSink} that implements the find
		/// functionality, requesting cancellation when something has been found
		/// </summary>
		/// @param <T> The type of input element </param>
		/// @param <O> The result type, typically an optional type </param>
		private abstract class FindSink<T, O> : TerminalSink<T, O>
		{
			internal bool HasValue;
			internal T Value;

			internal FindSink() // Avoid creation of special accessor
			{
			}

			public override void Accept(T value)
			{
				if (!HasValue)
				{
					HasValue = true;
					this.Value = value;
				}
			}

			public override bool CancellationRequested()
			{
				return HasValue;
			}

			/// <summary>
			/// Specialization of {@code FindSink} for reference streams </summary>
			internal sealed class OfRef<T> : FindSink<T, Optional<T>>
			{
				public override Optional<T> Get()
				{
					return HasValue ? Optional.Of(Value) : null;
				}
			}

			/// <summary>
			/// Specialization of {@code FindSink} for int streams </summary>
			internal sealed class OfInt : FindSink<Integer, OptionalInt>, Sink_OfInt
			{
				public void Accept(int value)
				{
					// Boxing is OK here, since few values will actually flow into the sink
					Accept((Integer) value);
				}

				public override OptionalInt Get()
				{
					return HasValue ? OptionalInt.Of(Value) : null;
				}
			}

			/// <summary>
			/// Specialization of {@code FindSink} for long streams </summary>
			internal sealed class OfLong : FindSink<Long, OptionalLong>, Sink_OfLong
			{
				public void Accept(long value)
				{
					// Boxing is OK here, since few values will actually flow into the sink
					Accept((Long) value);
				}

				public override OptionalLong Get()
				{
					return HasValue ? OptionalLong.Of(Value) : null;
				}
			}

			/// <summary>
			/// Specialization of {@code FindSink} for double streams </summary>
			internal sealed class OfDouble : FindSink<Double, OptionalDouble>, Sink_OfDouble
			{
				public void Accept(double value)
				{
					// Boxing is OK here, since few values will actually flow into the sink
					Accept((Double) value);
				}

				public override OptionalDouble Get()
				{
					return HasValue ? OptionalDouble.Of(Value) : null;
				}
			}
		}

		/// <summary>
		/// {@code ForkJoinTask} implementing parallel short-circuiting search </summary>
		/// @param <P_IN> Input element type to the stream pipeline </param>
		/// @param <P_OUT> Output element type from the stream pipeline </param>
		/// @param <O> Result type from the find operation </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class FindTask<P_IN, P_OUT, O> extends AbstractShortCircuitTask<P_IN, P_OUT, O, FindTask<P_IN, P_OUT, O>>
		private sealed class FindTask<P_IN, P_OUT, O> : AbstractShortCircuitTask<P_IN, P_OUT, O, FindTask<P_IN, P_OUT, O>>
		{
			internal readonly FindOp<P_OUT, O> Op;

			internal FindTask(FindOp<P_OUT, O> op, PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator)
			{
				this.Op = op;
			}

			internal FindTask(FindTask<P_IN, P_OUT, O> parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
			{
				this.Op = parent.Op;
			}

			protected internal override FindTask<P_IN, P_OUT, O> MakeChild(Spliterator<P_IN> spliterator)
			{
				return new FindTask<>(this, spliterator);
			}

			protected internal override O EmptyResult
			{
				get
				{
					return Op.EmptyValue;
				}
			}

			internal void FoundResult(O answer)
			{
				if (LeftmostNode)
				{
					ShortCircuit(answer);
				}
				else
				{
					CancelLaterNodes();
				}
			}

			protected internal override O DoLeaf()
			{
				O result = helper.wrapAndCopyInto(Op.SinkSupplier.Get(), spliterator).get();
				if (!Op.MustFindFirst)
				{
					if (result != null)
					{
						ShortCircuit(result);
					}
					return null;
				}
				else
				{
					if (result != null)
					{
						FoundResult(result);
						return result;
					}
					else
					{
						return null;
					}
				}
			}

			public override void onCompletion<T1>(CountedCompleter<T1> caller)
			{
				if (Op.MustFindFirst)
				{
						for (FindTask<P_IN, P_OUT, O> child = leftChild, p = null; child != p; p = child, child = rightChild)
						{
						O result = child.LocalResult;
						if (result != null && Op.PresentPredicate.Test(result))
						{
							LocalResult = result;
							FoundResult(result);
							break;
						}
						}
				}
				base.OnCompletion(caller);
			}
		}
	}


}