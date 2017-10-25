using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
	/// Spliterator implementations for wrapping and delegating spliterators, used
	/// in the implementation of the <seealso cref="Stream#spliterator()"/> method.
	/// 
	/// @since 1.8
	/// </summary>
	internal class StreamSpliterators
	{

		/// <summary>
		/// Abstract wrapping spliterator that binds to the spliterator of a
		/// pipeline helper on first operation.
		/// 
		/// <para>This spliterator is not late-binding and will bind to the source
		/// spliterator when first operated on.
		/// 
		/// </para>
		/// <para>A wrapping spliterator produced from a sequential stream
		/// cannot be split if there are stateful operations present.
		/// </para>
		/// </summary>
		private abstract class AbstractWrappingSpliterator<P_IN, P_OUT, T_BUFFER> : Spliterator<P_OUT> where T_BUFFER : AbstractSpinedBuffer
		{

			// @@@ Detect if stateful operations are present or not
			//     If not then can split otherwise cannot

			/// <summary>
			/// True if this spliterator supports splitting
			/// </summary>
			internal readonly bool IsParallel;

			internal readonly PipelineHelper<P_OUT> Ph;

			/// <summary>
			/// Supplier for the source spliterator.  Client provides either a
			/// spliterator or a supplier.
			/// </summary>
			internal Supplier<Spliterator<P_IN>> SpliteratorSupplier;

			/// <summary>
			/// Source spliterator.  Either provided from client or obtained from
			/// supplier.
			/// </summary>
			internal Spliterator<P_IN> Spliterator;

			/// <summary>
			/// Sink chain for the downstream stages of the pipeline, ultimately
			/// leading to the buffer. Used during partial traversal.
			/// </summary>
			internal Sink<P_IN> BufferSink;

			/// <summary>
			/// A function that advances one element of the spliterator, pushing
			/// it to bufferSink.  Returns whether any elements were processed.
			/// Used during partial traversal.
			/// </summary>
			internal BooleanSupplier Pusher;

			/// <summary>
			/// Next element to consume from the buffer, used during partial traversal </summary>
			internal long NextToConsume;

			/// <summary>
			/// Buffer into which elements are pushed.  Used during partial traversal. </summary>
			internal T_BUFFER Buffer;

			/// <summary>
			/// True if full traversal has occurred (with possible cancelation).
			/// If doing a partial traversal, there may be still elements in buffer.
			/// </summary>
			internal bool Finished;

			/// <summary>
			/// Construct an AbstractWrappingSpliterator from a
			/// {@code Supplier<Spliterator>}.
			/// </summary>
			internal AbstractWrappingSpliterator(PipelineHelper<P_OUT> ph, Supplier<Spliterator<P_IN>> spliteratorSupplier, bool parallel)
			{
				this.Ph = ph;
				this.SpliteratorSupplier = spliteratorSupplier;
				this.Spliterator = null;
				this.IsParallel = parallel;
			}

			/// <summary>
			/// Construct an AbstractWrappingSpliterator from a
			/// {@code Spliterator}.
			/// </summary>
			internal AbstractWrappingSpliterator(PipelineHelper<P_OUT> ph, Spliterator<P_IN> spliterator, bool parallel)
			{
				this.Ph = ph;
				this.SpliteratorSupplier = null;
				this.Spliterator = spliterator;
				this.IsParallel = parallel;
			}

			/// <summary>
			/// Called before advancing to set up spliterator, if needed.
			/// </summary>
			internal void Init()
			{
				if (Spliterator == null)
				{
					Spliterator = SpliteratorSupplier.Get();
					SpliteratorSupplier = null;
				}
			}

			/// <summary>
			/// Get an element from the source, pushing it into the sink chain,
			/// setting up the buffer if needed </summary>
			/// <returns> whether there are elements to consume from the buffer </returns>
			internal bool DoAdvance()
			{
				if (Buffer == null)
				{
					if (Finished)
					{
						return false;
					}

					Init();
					InitPartialTraversalState();
					NextToConsume = 0;
					BufferSink.begin(Spliterator.ExactSizeIfKnown);
					return FillBuffer();
				}
				else
				{
					++NextToConsume;
					bool hasNext = NextToConsume < Buffer.Count();
					if (!hasNext)
					{
						NextToConsume = 0;
						Buffer.Clear();
						hasNext = FillBuffer();
					}
					return hasNext;
				}
			}

			/// <summary>
			/// Invokes the shape-specific constructor with the provided arguments
			/// and returns the result.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: abstract AbstractWrappingSpliterator<P_IN, P_OUT, ?> wrap(java.util.Spliterator<P_IN> s);
			internal abstract AbstractWrappingSpliterator<P_IN, P_OUT, ?> Wrap(Spliterator<P_IN> s);

			/// <summary>
			/// Initializes buffer, sink chain, and pusher for a shape-specific
			/// implementation.
			/// </summary>
			internal abstract void InitPartialTraversalState();

			public override Spliterator<P_OUT> TrySplit()
			{
				if (IsParallel && !Finished)
				{
					Init();

					Spliterator<P_IN> split = Spliterator.TrySplit();
					return (split == null) ? null : Wrap(split);
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// If the buffer is empty, push elements into the sink chain until
			/// the source is empty or cancellation is requested. </summary>
			/// <returns> whether there are elements to consume from the buffer </returns>
			internal virtual bool FillBuffer()
			{
				while (Buffer.Count() == 0)
				{
					if (BufferSink.cancellationRequested() || !Pusher.AsBoolean)
					{
						if (Finished)
						{
							return false;
						}
						else
						{
							BufferSink.end(); // might trigger more elements
							Finished = true;
						}
					}
				}
				return true;
			}

			public override long EstimateSize()
			{
				Init();
				// Use the estimate of the wrapped spliterator
				// Note this may not be accurate if there are filter/flatMap
				// operations filtering or adding elements to the stream
				return Spliterator.EstimateSize();
			}

			public override long ExactSizeIfKnown
			{
				get
				{
					Init();
					return StreamOpFlag.SIZED.isKnown(Ph.StreamAndOpFlags) ? Spliterator.ExactSizeIfKnown : -1;
				}
			}

			public override int Characteristics()
			{
				Init();

				// Get the characteristics from the pipeline
				int c = StreamOpFlag.toCharacteristics(StreamOpFlag.toStreamFlags(Ph.StreamAndOpFlags));

				// Mask off the size and uniform characteristics and replace with
				// those of the spliterator
				// Note that a non-uniform spliterator can change from something
				// with an exact size to an estimate for a sub-split, for example
				// with HashSet where the size is known at the top level spliterator
				// but for sub-splits only an estimate is known
				if ((c & java.util.Spliterator_Fields.SIZED) != 0)
				{
					c &= ~(java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED);
					c |= (Spliterator.Characteristics() & (java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED));
				}

				return c;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base P_OUT> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					if (!hasCharacteristics(java.util.Spliterator_Fields.SORTED))
					{
						throw new IllegalStateException();
					}
					return null;
				}
			}

			public override sealed String ToString()
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return string.Format("{0}[{1}]", this.GetType().FullName, Spliterator);
			}
		}

		internal sealed class WrappingSpliterator<P_IN, P_OUT> : AbstractWrappingSpliterator<P_IN, P_OUT, SpinedBuffer<P_OUT>>
		{

			internal WrappingSpliterator(PipelineHelper<P_OUT> ph, Supplier<Spliterator<P_IN>> supplier, bool parallel) : base(ph, supplier, parallel)
			{
			}

			internal WrappingSpliterator(PipelineHelper<P_OUT> ph, Spliterator<P_IN> spliterator, bool parallel) : base(ph, spliterator, parallel)
			{
			}

			internal override WrappingSpliterator<P_IN, P_OUT> Wrap(Spliterator<P_IN> s)
			{
				return new WrappingSpliterator<>(Ph, s, IsParallel);
			}

			internal override void InitPartialTraversalState()
			{
				SpinedBuffer<P_OUT> b = new SpinedBuffer<P_OUT>();
				Buffer = b;
				BufferSink = Ph.WrapSink(b::accept);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Pusher = () => Spliterator.TryAdvance(BufferSink);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base P_OUT> consumer)
			public override bool tryAdvance<T1>(Consumer<T1> consumer)
			{
				Objects.RequireNonNull(consumer);
				bool hasNext = DoAdvance();
				if (hasNext)
				{
					consumer.Accept(Buffer.Get(NextToConsume));
				}
				return hasNext;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base P_OUT> consumer)
			public override void forEachRemaining<T1>(Consumer<T1> consumer)
			{
				if (Buffer == null && !Finished)
				{
					Objects.RequireNonNull(consumer);
					Init();

					Ph.WrapAndCopyInto((Sink<P_OUT>) consumer::accept, Spliterator);
					Finished = true;
				}
				else
				{
					do
					{
					} while (TryAdvance(consumer));
				}
			}
		}

		internal sealed class IntWrappingSpliterator<P_IN> : AbstractWrappingSpliterator<P_IN, Integer, SpinedBuffer.OfInt>, java.util.Spliterator_OfInt
		{

			internal IntWrappingSpliterator(PipelineHelper<Integer> ph, Supplier<Spliterator<P_IN>> supplier, bool parallel) : base(ph, supplier, parallel)
			{
			}

			internal IntWrappingSpliterator(PipelineHelper<Integer> ph, Spliterator<P_IN> spliterator, bool parallel) : base(ph, spliterator, parallel)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: @Override AbstractWrappingSpliterator<P_IN, Integer, ?> wrap(java.util.Spliterator<P_IN> s)
			internal override AbstractWrappingSpliterator<P_IN, Integer, ?> Wrap(Spliterator<P_IN> s)
			{
				return new IntWrappingSpliterator<>(Ph, s, IsParallel);
			}

			internal override void InitPartialTraversalState()
			{
				SpinedBuffer.OfInt b = new SpinedBuffer.OfInt();
				Buffer = b;
				BufferSink = Ph.WrapSink((Sink_OfInt) b::accept);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Pusher = () => Spliterator.TryAdvance(BufferSink);
			}

			public java.util.Spliterator_OfInt TrySplit()
			{
				return (java.util.Spliterator_OfInt) base.TrySplit();
			}

			public bool TryAdvance(IntConsumer consumer)
			{
				Objects.RequireNonNull(consumer);
				bool hasNext = DoAdvance();
				if (hasNext)
				{
					consumer.Accept(Buffer.Get(NextToConsume));
				}
				return hasNext;
			}

			public override void ForEachRemaining(IntConsumer consumer)
			{
				if (Buffer == null && !Finished)
				{
					Objects.RequireNonNull(consumer);
					Init();

					Ph.WrapAndCopyInto((Sink_OfInt) consumer::accept, Spliterator);
					Finished = true;
				}
				else
				{
					do
					{
					} while (TryAdvance(consumer));
				}
			}
		}

		internal sealed class LongWrappingSpliterator<P_IN> : AbstractWrappingSpliterator<P_IN, Long, SpinedBuffer.OfLong>, java.util.Spliterator_OfLong
		{

			internal LongWrappingSpliterator(PipelineHelper<Long> ph, Supplier<Spliterator<P_IN>> supplier, bool parallel) : base(ph, supplier, parallel)
			{
			}

			internal LongWrappingSpliterator(PipelineHelper<Long> ph, Spliterator<P_IN> spliterator, bool parallel) : base(ph, spliterator, parallel)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: @Override AbstractWrappingSpliterator<P_IN, Long, ?> wrap(java.util.Spliterator<P_IN> s)
			internal override AbstractWrappingSpliterator<P_IN, Long, ?> Wrap(Spliterator<P_IN> s)
			{
				return new LongWrappingSpliterator<>(Ph, s, IsParallel);
			}

			internal override void InitPartialTraversalState()
			{
				SpinedBuffer.OfLong b = new SpinedBuffer.OfLong();
				Buffer = b;
				BufferSink = Ph.WrapSink((Sink_OfLong) b::accept);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Pusher = () => Spliterator.TryAdvance(BufferSink);
			}

			public java.util.Spliterator_OfLong TrySplit()
			{
				return (java.util.Spliterator_OfLong) base.TrySplit();
			}

			public bool TryAdvance(LongConsumer consumer)
			{
				Objects.RequireNonNull(consumer);
				bool hasNext = DoAdvance();
				if (hasNext)
				{
					consumer.Accept(Buffer.Get(NextToConsume));
				}
				return hasNext;
			}

			public override void ForEachRemaining(LongConsumer consumer)
			{
				if (Buffer == null && !Finished)
				{
					Objects.RequireNonNull(consumer);
					Init();

					Ph.WrapAndCopyInto((Sink_OfLong) consumer::accept, Spliterator);
					Finished = true;
				}
				else
				{
					do
					{
					} while (TryAdvance(consumer));
				}
			}
		}

		internal sealed class DoubleWrappingSpliterator<P_IN> : AbstractWrappingSpliterator<P_IN, Double, SpinedBuffer.OfDouble>, java.util.Spliterator_OfDouble
		{

			internal DoubleWrappingSpliterator(PipelineHelper<Double> ph, Supplier<Spliterator<P_IN>> supplier, bool parallel) : base(ph, supplier, parallel)
			{
			}

			internal DoubleWrappingSpliterator(PipelineHelper<Double> ph, Spliterator<P_IN> spliterator, bool parallel) : base(ph, spliterator, parallel)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: @Override AbstractWrappingSpliterator<P_IN, Double, ?> wrap(java.util.Spliterator<P_IN> s)
			internal override AbstractWrappingSpliterator<P_IN, Double, ?> Wrap(Spliterator<P_IN> s)
			{
				return new DoubleWrappingSpliterator<>(Ph, s, IsParallel);
			}

			internal override void InitPartialTraversalState()
			{
				SpinedBuffer.OfDouble b = new SpinedBuffer.OfDouble();
				Buffer = b;
				BufferSink = Ph.WrapSink((Sink_OfDouble) b::accept);
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				Pusher = () => Spliterator.TryAdvance(BufferSink);
			}

			public java.util.Spliterator_OfDouble TrySplit()
			{
				return (java.util.Spliterator_OfDouble) base.TrySplit();
			}

			public bool TryAdvance(DoubleConsumer consumer)
			{
				Objects.RequireNonNull(consumer);
				bool hasNext = DoAdvance();
				if (hasNext)
				{
					consumer.Accept(Buffer.Get(NextToConsume));
				}
				return hasNext;
			}

			public override void ForEachRemaining(DoubleConsumer consumer)
			{
				if (Buffer == null && !Finished)
				{
					Objects.RequireNonNull(consumer);
					Init();

					Ph.WrapAndCopyInto((Sink_OfDouble) consumer::accept, Spliterator);
					Finished = true;
				}
				else
				{
					do
					{
					} while (TryAdvance(consumer));
				}
			}
		}

		/// <summary>
		/// Spliterator implementation that delegates to an underlying spliterator,
		/// acquiring the spliterator from a {@code Supplier<Spliterator>} on the
		/// first call to any spliterator method. </summary>
		/// @param <T> </param>
		internal class DelegatingSpliterator<T, T_SPLITR> : Spliterator<T> where T_SPLITR : java.util.Spliterator<T>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final java.util.function.Supplier<? extends T_SPLITR> supplier;
			internal readonly Supplier<?> Supplier;

			internal T_SPLITR s;

			internal DelegatingSpliterator<T1>(Supplier<T1> supplier) where T1 : T_SPLITR
			{
				this.Supplier = supplier;
			}

			internal virtual T_SPLITR Get()
			{
				if (s == null)
				{
					s = Supplier.Get();
				}
				return s;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public T_SPLITR trySplit()
			public override T_SPLITR TrySplit()
			{
				return (T_SPLITR) Get().trySplit();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> consumer)
			public override bool tryAdvance<T1>(Consumer<T1> consumer)
			{
				return Get().tryAdvance(consumer);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> consumer)
			public override void forEachRemaining<T1>(Consumer<T1> consumer)
			{
				Get().forEachRemaining(consumer);
			}

			public override long EstimateSize()
			{
				return Get().estimateSize();
			}

			public override int Characteristics()
			{
				return Get().characteristics();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					return Get().Comparator;
				}
			}

			public override long ExactSizeIfKnown
			{
				get
				{
					return Get().ExactSizeIfKnown;
				}
			}

			public override String ToString()
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName + "[" + Get() + "]";
			}

			internal class OfPrimitive<T, T_CONS, T_SPLITR> : DelegatingSpliterator<T, T_SPLITR>, java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>
			{
				internal OfPrimitive<T1>(Supplier<T1> supplier) where T1 : T_SPLITR : base(supplier)
				{
				}

				public override bool TryAdvance(T_CONS consumer)
				{
					return outerInstance.Get().tryAdvance(consumer);
				}

				public override void ForEachRemaining(T_CONS consumer)
				{
					outerInstance.Get().forEachRemaining(consumer);
				}
			}

			internal sealed class OfInt : OfPrimitive<Integer, IntConsumer, java.util.Spliterator_OfInt>, java.util.Spliterator_OfInt
			{

				internal OfInt(Supplier<java.util.Spliterator_OfInt> supplier) : base(supplier)
				{
				}
			}

			internal sealed class OfLong : OfPrimitive<Long, LongConsumer, java.util.Spliterator_OfLong>, java.util.Spliterator_OfLong
			{

				internal OfLong(Supplier<java.util.Spliterator_OfLong> supplier) : base(supplier)
				{
				}
			}

			internal sealed class OfDouble : OfPrimitive<Double, DoubleConsumer, java.util.Spliterator_OfDouble>, java.util.Spliterator_OfDouble
			{

				internal OfDouble(Supplier<java.util.Spliterator_OfDouble> supplier) : base(supplier)
				{
				}
			}
		}

		/// <summary>
		/// A slice Spliterator from a source Spliterator that reports
		/// {@code SUBSIZED}.
		/// 
		/// </summary>
		internal abstract class SliceSpliterator<T, T_SPLITR> where T_SPLITR : java.util.Spliterator<T>
		{
			// The start index of the slice
			internal readonly long SliceOrigin;
			// One past the last index of the slice
			internal readonly long SliceFence;

			// The spliterator to slice
			internal T_SPLITR s;
			// current (absolute) index, modified on advance/split
			internal long Index;
			// one past last (absolute) index or sliceFence, which ever is smaller
			internal long Fence;

			internal SliceSpliterator(T_SPLITR s, long sliceOrigin, long sliceFence, long origin, long fence)
			{
				Debug.Assert(s.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED));
				this.s = s;
				this.SliceOrigin = sliceOrigin;
				this.SliceFence = sliceFence;
				this.Index = origin;
				this.Fence = fence;
			}

			protected internal abstract T_SPLITR MakeSpliterator(T_SPLITR s, long sliceOrigin, long sliceFence, long origin, long fence);

			public virtual T_SPLITR TrySplit()
			{
				if (SliceOrigin >= Fence)
				{
					return null;
				}

				if (Index >= Fence)
				{
					return null;
				}

				// Keep splitting until the left and right splits intersect with the slice
				// thereby ensuring the size estimate decreases.
				// This also avoids creating empty spliterators which can result in
				// existing and additionally created F/J tasks that perform
				// redundant work on no elements.
				while (true)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_SPLITR leftSplit = (T_SPLITR) s.trySplit();
					T_SPLITR leftSplit = (T_SPLITR) s.trySplit();
					if (leftSplit == null)
					{
						return null;
					}

					long leftSplitFenceUnbounded = Index + leftSplit.estimateSize();
					long leftSplitFence = System.Math.Min(leftSplitFenceUnbounded, SliceFence);
					if (SliceOrigin >= leftSplitFence)
					{
						// The left split does not intersect with, and is to the left of, the slice
						// The right split does intersect
						// Discard the left split and split further with the right split
						Index = leftSplitFence;
					}
					else if (leftSplitFence >= SliceFence)
					{
						// The right split does not intersect with, and is to the right of, the slice
						// The left split does intersect
						// Discard the right split and split further with the left split
						s = leftSplit;
						Fence = leftSplitFence;
					}
					else if (Index >= SliceOrigin && leftSplitFenceUnbounded <= SliceFence)
					{
						// The left split is contained within the slice, return the underlying left split
						// Right split is contained within or intersects with the slice
						Index = leftSplitFence;
						return leftSplit;
					}
					else
					{
						// The left split intersects with the slice
						// Right split is contained within or intersects with the slice
						return MakeSpliterator(leftSplit, SliceOrigin, SliceFence, Index, Index = leftSplitFence);
					}
				}
			}

			public virtual long EstimateSize()
			{
				return (SliceOrigin < Fence) ? Fence - System.Math.Max(SliceOrigin, Index) : 0;
			}

			public virtual int Characteristics()
			{
				return s.characteristics();
			}

			internal sealed class OfRef<T> : SliceSpliterator<T, Spliterator<T>>, Spliterator<T>
			{

				internal OfRef(Spliterator<T> s, long sliceOrigin, long sliceFence) : this(s, sliceOrigin, sliceFence, 0, System.Math.Min(s.EstimateSize(), sliceFence))
				{
				}

				internal OfRef(Spliterator<T> s, long sliceOrigin, long sliceFence, long origin, long fence) : base(s, sliceOrigin, sliceFence, origin, fence)
				{
				}

				protected internal override Spliterator<T> MakeSpliterator(Spliterator<T> s, long sliceOrigin, long sliceFence, long origin, long fence)
				{
					return new OfRef<>(s, sliceOrigin, sliceFence, origin, fence);
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
				public override bool tryAdvance<T1>(Consumer<T1> action)
				{
					Objects.RequireNonNull(action);

					if (SliceOrigin >= Fence)
					{
						return false;
					}

					while (SliceOrigin > Index)
					{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
						s.tryAdvance(e =>
						{
						});
						Index++;
					}

					if (Index >= Fence)
					{
						return false;
					}

					Index++;
					return s.TryAdvance(action);
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
				public override void forEachRemaining<T1>(Consumer<T1> action)
				{
					Objects.RequireNonNull(action);

					if (SliceOrigin >= Fence)
					{
						return;
					}

					if (Index >= Fence)
					{
						return;
					}

					if (Index >= SliceOrigin && (Index + s.EstimateSize()) <= SliceFence)
					{
						// The spliterator is contained within the slice
						s.forEachRemaining(action);
						Index = Fence;
					}
					else
					{
						// The spliterator intersects with the slice
						while (SliceOrigin > Index)
						{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
							s.tryAdvance(e =>
							{
							});
							Index++;
						}
						// Traverse elements up to the fence
						for (;Index < Fence; Index++)
						{
							s.TryAdvance(action);
						}
					}
				}
			}

			internal abstract class OfPrimitive<T, T_SPLITR, T_CONS> : SliceSpliterator<T, T_SPLITR>, java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>
			{

				internal OfPrimitive(T_SPLITR s, long sliceOrigin, long sliceFence) : this(s, sliceOrigin, sliceFence, 0, System.Math.Min(s.estimateSize(), sliceFence))
				{
				}

				internal OfPrimitive(T_SPLITR s, long sliceOrigin, long sliceFence, long origin, long fence) : base(s, sliceOrigin, sliceFence, origin, fence)
				{
				}

				public override bool TryAdvance(T_CONS action)
				{
					Objects.RequireNonNull(action);

					if (outerInstance.SliceOrigin >= outerInstance.Fence)
					{
						return false;
					}

					while (outerInstance.SliceOrigin > outerInstance.Index)
					{
						outerInstance.s.tryAdvance(EmptyConsumer());
						outerInstance.Index++;
					}

					if (outerInstance.Index >= outerInstance.Fence)
					{
						return false;
					}

					outerInstance.Index++;
					return outerInstance.s.tryAdvance(action);
				}

				public override void ForEachRemaining(T_CONS action)
				{
					Objects.RequireNonNull(action);

					if (outerInstance.SliceOrigin >= outerInstance.Fence)
					{
						return;
					}

					if (outerInstance.Index >= outerInstance.Fence)
					{
						return;
					}

					if (outerInstance.Index >= outerInstance.SliceOrigin && (outerInstance.Index + outerInstance.s.estimateSize()) <= outerInstance.SliceFence)
					{
						// The spliterator is contained within the slice
						outerInstance.s.forEachRemaining(action);
						outerInstance.Index = outerInstance.Fence;
					}
					else
					{
						// The spliterator intersects with the slice
						while (outerInstance.SliceOrigin > outerInstance.Index)
						{
							outerInstance.s.tryAdvance(EmptyConsumer());
							outerInstance.Index++;
						}
						// Traverse elements up to the fence
						for (;outerInstance.Index < outerInstance.Fence; outerInstance.Index++)
						{
							outerInstance.s.tryAdvance(action);
						}
					}
				}

				protected internal abstract T_CONS EmptyConsumer();
			}

			internal sealed class OfInt : OfPrimitive<Integer, java.util.Spliterator_OfInt, IntConsumer>, java.util.Spliterator_OfInt
			{
				internal OfInt(java.util.Spliterator_OfInt s, long sliceOrigin, long sliceFence) : base(s, sliceOrigin, sliceFence)
				{
				}

				internal OfInt(java.util.Spliterator_OfInt s, long sliceOrigin, long sliceFence, long origin, long fence) : base(s, sliceOrigin, sliceFence, origin, fence)
				{
				}

				protected internal override java.util.Spliterator_OfInt MakeSpliterator(java.util.Spliterator_OfInt s, long sliceOrigin, long sliceFence, long origin, long fence)
				{
					return new SliceSpliterator.OfInt(s, sliceOrigin, sliceFence, origin, fence);
				}

				protected internal override IntConsumer EmptyConsumer()
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					return e =>
					{
					};
				}
			}

			internal sealed class OfLong : OfPrimitive<Long, java.util.Spliterator_OfLong, LongConsumer>, java.util.Spliterator_OfLong
			{
				internal OfLong(java.util.Spliterator_OfLong s, long sliceOrigin, long sliceFence) : base(s, sliceOrigin, sliceFence)
				{
				}

				internal OfLong(java.util.Spliterator_OfLong s, long sliceOrigin, long sliceFence, long origin, long fence) : base(s, sliceOrigin, sliceFence, origin, fence)
				{
				}

				protected internal override java.util.Spliterator_OfLong MakeSpliterator(java.util.Spliterator_OfLong s, long sliceOrigin, long sliceFence, long origin, long fence)
				{
					return new SliceSpliterator.OfLong(s, sliceOrigin, sliceFence, origin, fence);
				}

				protected internal override LongConsumer EmptyConsumer()
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					return e =>
					{
					};
				}
			}

			internal sealed class OfDouble : OfPrimitive<Double, java.util.Spliterator_OfDouble, DoubleConsumer>, java.util.Spliterator_OfDouble
			{
				internal OfDouble(java.util.Spliterator_OfDouble s, long sliceOrigin, long sliceFence) : base(s, sliceOrigin, sliceFence)
				{
				}

				internal OfDouble(java.util.Spliterator_OfDouble s, long sliceOrigin, long sliceFence, long origin, long fence) : base(s, sliceOrigin, sliceFence, origin, fence)
				{
				}

				protected internal override java.util.Spliterator_OfDouble MakeSpliterator(java.util.Spliterator_OfDouble s, long sliceOrigin, long sliceFence, long origin, long fence)
				{
					return new SliceSpliterator.OfDouble(s, sliceOrigin, sliceFence, origin, fence);
				}

				protected internal override DoubleConsumer EmptyConsumer()
				{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					return e =>
					{
					};
				}
			}
		}

		/// <summary>
		/// A slice Spliterator that does not preserve order, if any, of a source
		/// Spliterator.
		/// 
		/// Note: The source spliterator may report {@code ORDERED} since that
		/// spliterator be the result of a previous pipeline stage that was
		/// collected to a {@code Node}. It is the order of the pipeline stage
		/// that governs whether the this slice spliterator is to be used or not.
		/// </summary>
		internal abstract class UnorderedSliceSpliterator<T, T_SPLITR> where T_SPLITR : java.util.Spliterator<T>
		{
			internal static readonly int CHUNK_SIZE = 1 << 7;

			// The spliterator to slice
			protected internal readonly T_SPLITR s;
			protected internal readonly bool Unlimited;
			internal readonly long SkipThreshold;
			internal readonly AtomicLong Permits;

			internal UnorderedSliceSpliterator(T_SPLITR s, long skip, long limit)
			{
				this.s = s;
				this.Unlimited = limit < 0;
				this.SkipThreshold = limit >= 0 ? limit : 0;
				this.Permits = new AtomicLong(limit >= 0 ? skip + limit : skip);
			}

			internal UnorderedSliceSpliterator(T_SPLITR s, UnorderedSliceSpliterator<T, T_SPLITR> parent)
			{
				this.s = s;
				this.Unlimited = parent.Unlimited;
				this.Permits = parent.Permits;
				this.SkipThreshold = parent.SkipThreshold;
			}

			/// <summary>
			/// Acquire permission to skip or process elements.  The caller must
			/// first acquire the elements, then consult this method for guidance
			/// as to what to do with the data.
			/// 
			/// <para>We use an {@code AtomicLong} to atomically maintain a counter,
			/// which is initialized as skip+limit if we are limiting, or skip only
			/// if we are not limiting.  The user should consult the method
			/// {@code checkPermits()} before acquiring data elements.
			/// 
			/// </para>
			/// </summary>
			/// <param name="numElements"> the number of elements the caller has in hand </param>
			/// <returns> the number of elements that should be processed; any
			/// remaining elements should be discarded. </returns>
			protected internal long AcquirePermits(long numElements)
			{
				long remainingPermits;
				long grabbing;
				// permits never increase, and don't decrease below zero
				Debug.Assert(numElements > 0);
				do
				{
					remainingPermits = Permits.Get();
					if (remainingPermits == 0)
					{
						return Unlimited ? numElements : 0;
					}
					grabbing = System.Math.Min(remainingPermits, numElements);
				} while (grabbing > 0 && !Permits.CompareAndSet(remainingPermits, remainingPermits - grabbing));

				if (Unlimited)
				{
					return System.Math.Max(numElements - grabbing, 0);
				}
				else if (remainingPermits > SkipThreshold)
				{
					return System.Math.Max(grabbing - (remainingPermits - SkipThreshold), 0);
				}
				else
				{
					return grabbing;
				}
			}

			internal enum PermitStatus
			{
				NO_MORE,
				MAYBE_MORE,
				UNLIMITED
			}

			/// <summary>
			/// Call to check if permits might be available before acquiring data </summary>
			protected internal PermitStatus PermitStatus()
			{
				if (Permits.Get() > 0)
				{
					return PermitStatus.MAYBE_MORE;
				}
				else
				{
					return Unlimited ? PermitStatus.UNLIMITED : PermitStatus.NO_MORE;
				}
			}

			public T_SPLITR TrySplit()
			{
				// Stop splitting when there are no more limit permits
				if (Permits.Get() == 0)
				{
					return null;
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_SPLITR split = (T_SPLITR) s.trySplit();
				T_SPLITR split = (T_SPLITR) s.trySplit();
				return split == null ? null : MakeSpliterator(split);
			}

			protected internal abstract T_SPLITR MakeSpliterator(T_SPLITR s);

			public long EstimateSize()
			{
				return s.estimateSize();
			}

			public int Characteristics()
			{
				return s.characteristics() & ~(java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.ORDERED);
			}

			internal sealed class OfRef<T> : UnorderedSliceSpliterator<T, Spliterator<T>>, Spliterator<T>, Consumer<T>
			{
				internal T TmpSlot;

				internal OfRef(Spliterator<T> s, long skip, long limit) : base(s, skip, limit)
				{
				}

				internal OfRef(Spliterator<T> s, OfRef<T> parent) : base(s, parent)
				{
				}

				public override void Accept(T t)
				{
					TmpSlot = t;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
				public override bool tryAdvance<T1>(Consumer<T1> action)
				{
					Objects.RequireNonNull(action);

					while (PermitStatus() != PermitStatus.NO_MORE)
					{
						if (!s.TryAdvance(this))
						{
							return false;
						}
						else if (AcquirePermits(1) == 1)
						{
							action.Accept(TmpSlot);
							TmpSlot = null;
							return true;
						}
					}
					return false;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
				public override void forEachRemaining<T1>(Consumer<T1> action)
				{
					Objects.RequireNonNull(action);

					ArrayBuffer.OfRef<T> sb = null;
					PermitStatus permitStatus;
					while ((permitStatus = PermitStatus()) != PermitStatus.NO_MORE)
					{
						if (permitStatus == PermitStatus.MAYBE_MORE)
						{
							// Optimistically traverse elements up to a threshold of CHUNK_SIZE
							if (sb == null)
							{
								sb = new ArrayBuffer.OfRef<>(CHUNK_SIZE);
							}
							else
							{
								sb.Reset();
							}
							long permitsRequested = 0;
							do
							{
							} while (s.TryAdvance(sb) && ++permitsRequested < CHUNK_SIZE);
							if (permitsRequested == 0)
							{
								return;
							}
							sb.ForEach(action, AcquirePermits(permitsRequested));
						}
						else
						{
							// Must be UNLIMITED; let 'er rip
							s.forEachRemaining(action);
							return;
						}
					}
				}

				protected internal override Spliterator<T> MakeSpliterator(Spliterator<T> s)
				{
					return new UnorderedSliceSpliterator.OfRef<>(s, this);
				}
			}

			/// <summary>
			/// Concrete sub-types must also be an instance of type {@code T_CONS}.
			/// </summary>
			/// @param <T_BUFF> the type of the spined buffer. Must also be a type of
			///        {@code T_CONS}. </param>
			internal abstract class OfPrimitive<T, T_CONS, T_BUFF, T_SPLITR> : UnorderedSliceSpliterator<T, T_SPLITR>, java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_BUFF : ArrayBuffer.OfPrimitive<T_CONS> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>
			{
				internal OfPrimitive(T_SPLITR s, long skip, long limit) : base(s, skip, limit)
				{
				}

				internal OfPrimitive(T_SPLITR s, UnorderedSliceSpliterator.OfPrimitive<T, T_CONS, T_BUFF, T_SPLITR> parent) : base(s, parent)
				{
				}

				public override bool TryAdvance(T_CONS action)
				{
					Objects.RequireNonNull(action);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_CONS consumer = (T_CONS) this;
					T_CONS consumer = (T_CONS) this;

					while (outerInstance.PermitStatus() != PermitStatus.NO_MORE)
					{
						if (!outerInstance.s.tryAdvance(consumer))
						{
							return false;
						}
						else if (outerInstance.AcquirePermits(1) == 1)
						{
							AcceptConsumed(action);
							return true;
						}
					}
					return false;
				}

				protected internal abstract void AcceptConsumed(T_CONS action);

				public override void ForEachRemaining(T_CONS action)
				{
					Objects.RequireNonNull(action);

					T_BUFF sb = null;
					PermitStatus permitStatus;
					while ((permitStatus = outerInstance.PermitStatus()) != PermitStatus.NO_MORE)
					{
						if (permitStatus == PermitStatus.MAYBE_MORE)
						{
							// Optimistically traverse elements up to a threshold of CHUNK_SIZE
							if (sb == null)
							{
								sb = BufferCreate(CHUNK_SIZE);
							}
							else
							{
								sb.reset();
							}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_CONS sbc = (T_CONS) sb;
							T_CONS sbc = (T_CONS) sb;
							long permitsRequested = 0;
							do
							{
							} while (outerInstance.s.tryAdvance(sbc) && ++permitsRequested < CHUNK_SIZE);
							if (permitsRequested == 0)
							{
								return;
							}
							sb.forEach(action, outerInstance.AcquirePermits(permitsRequested));
						}
						else
						{
							// Must be UNLIMITED; let 'er rip
							outerInstance.s.forEachRemaining(action);
							return;
						}
					}
				}

				protected internal abstract T_BUFF BufferCreate(int initialCapacity);
			}

			internal sealed class OfInt : OfPrimitive<Integer, IntConsumer, ArrayBuffer.OfInt, java.util.Spliterator_OfInt>, java.util.Spliterator_OfInt, IntConsumer
			{

				internal int TmpValue;

				internal OfInt(java.util.Spliterator_OfInt s, long skip, long limit) : base(s, skip, limit)
				{
				}

				internal OfInt(java.util.Spliterator_OfInt s, UnorderedSliceSpliterator.OfInt parent) : base(s, parent)
				{
				}

				public void Accept(int value)
				{
					TmpValue = value;
				}

				protected internal override void AcceptConsumed(IntConsumer action)
				{
					action.Accept(TmpValue);
				}

				protected internal override ArrayBuffer.OfInt BufferCreate(int initialCapacity)
				{
					return new ArrayBuffer.OfInt(initialCapacity);
				}

				protected internal override java.util.Spliterator_OfInt MakeSpliterator(java.util.Spliterator_OfInt s)
				{
					return new UnorderedSliceSpliterator.OfInt(s, this);
				}
			}

			internal sealed class OfLong : OfPrimitive<Long, LongConsumer, ArrayBuffer.OfLong, java.util.Spliterator_OfLong>, java.util.Spliterator_OfLong, LongConsumer
			{

				internal long TmpValue;

				internal OfLong(java.util.Spliterator_OfLong s, long skip, long limit) : base(s, skip, limit)
				{
				}

				internal OfLong(java.util.Spliterator_OfLong s, UnorderedSliceSpliterator.OfLong parent) : base(s, parent)
				{
				}

				public void Accept(long value)
				{
					TmpValue = value;
				}

				protected internal override void AcceptConsumed(LongConsumer action)
				{
					action.Accept(TmpValue);
				}

				protected internal override ArrayBuffer.OfLong BufferCreate(int initialCapacity)
				{
					return new ArrayBuffer.OfLong(initialCapacity);
				}

				protected internal override java.util.Spliterator_OfLong MakeSpliterator(java.util.Spliterator_OfLong s)
				{
					return new UnorderedSliceSpliterator.OfLong(s, this);
				}
			}

			internal sealed class OfDouble : OfPrimitive<Double, DoubleConsumer, ArrayBuffer.OfDouble, java.util.Spliterator_OfDouble>, java.util.Spliterator_OfDouble, DoubleConsumer
			{

				internal double TmpValue;

				internal OfDouble(java.util.Spliterator_OfDouble s, long skip, long limit) : base(s, skip, limit)
				{
				}

				internal OfDouble(java.util.Spliterator_OfDouble s, UnorderedSliceSpliterator.OfDouble parent) : base(s, parent)
				{
				}

				public void Accept(double value)
				{
					TmpValue = value;
				}

				protected internal override void AcceptConsumed(DoubleConsumer action)
				{
					action.Accept(TmpValue);
				}

				protected internal override ArrayBuffer.OfDouble BufferCreate(int initialCapacity)
				{
					return new ArrayBuffer.OfDouble(initialCapacity);
				}

				protected internal override java.util.Spliterator_OfDouble MakeSpliterator(java.util.Spliterator_OfDouble s)
				{
					return new UnorderedSliceSpliterator.OfDouble(s, this);
				}
			}
		}

		/// <summary>
		/// A wrapping spliterator that only reports distinct elements of the
		/// underlying spliterator. Does not preserve size and encounter order.
		/// </summary>
		internal sealed class DistinctSpliterator<T> : Spliterator<T>, Consumer<T>
		{

			// The value to represent null in the ConcurrentHashMap
			internal static readonly Object NULL_VALUE = new Object();

			// The underlying spliterator
			internal readonly Spliterator<T> s;

			// ConcurrentHashMap holding distinct elements as keys
			internal readonly ConcurrentDictionary<T, Boolean> Seen;

			// Temporary element, only used with tryAdvance
			internal T TmpSlot;

			internal DistinctSpliterator(Spliterator<T> s) : this(s, new ConcurrentDictionary<>())
			{
			}

			internal DistinctSpliterator(Spliterator<T> s, ConcurrentDictionary<T, Boolean> seen)
			{
				this.s = s;
				this.Seen = seen;
			}

			public override void Accept(T t)
			{
				this.TmpSlot = t;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private T mapNull(T t)
			internal T MapNull(T t)
			{
				return t != null ? t : (T) NULL_VALUE;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
			public override bool tryAdvance<T1>(Consumer<T1> action)
			{
				while (s.TryAdvance(this))
				{
					if (Seen.GetOrAdd(MapNull(TmpSlot), true) == null)
					{
						action.Accept(TmpSlot);
						TmpSlot = null;
						return true;
					}
				}
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> action)
			public override void forEachRemaining<T1>(Consumer<T1> action)
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				s.forEachRemaining(t =>
				{
					if (Seen.GetOrAdd(MapNull(t), true) == null)
					{
						action.Accept(t);
					}
				});
			}

			public override Spliterator<T> TrySplit()
			{
				Spliterator<T> split = s.TrySplit();
				return (split != null) ? new DistinctSpliterator<>(split, Seen) : null;
			}

			public override long EstimateSize()
			{
				return s.EstimateSize();
			}

			public override int Characteristics()
			{
				return (s.Characteristics() & ~(java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.SUBSIZED | java.util.Spliterator_Fields.SORTED | java.util.Spliterator_Fields.ORDERED)) | java.util.Spliterator_Fields.DISTINCT;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public java.util.Comparator<? base T> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public override IComparer<?> Comparator
			{
				get
				{
					return s.Comparator;
				}
			}
		}

		/// <summary>
		/// A Spliterator that infinitely supplies elements in no particular order.
		/// 
		/// <para>Splitting divides the estimated size in two and stops when the
		/// estimate size is 0.
		/// 
		/// </para>
		/// <para>The {@code forEachRemaining} method if invoked will never terminate.
		/// The {@code tryAdvance} method always returns true.
		/// 
		/// </para>
		/// </summary>
		internal abstract class InfiniteSupplyingSpliterator<T> : Spliterator<T>
		{
			internal long Estimate;

			protected internal InfiniteSupplyingSpliterator(long estimate)
			{
				this.Estimate = estimate;
			}

			public override long EstimateSize()
			{
				return Estimate;
			}

			public override int Characteristics()
			{
				return java.util.Spliterator_Fields.IMMUTABLE;
			}

			internal sealed class OfRef<T> : InfiniteSupplyingSpliterator<T>
			{
				internal readonly Supplier<T> s;

				internal OfRef(long size, Supplier<T> s) : base(size)
				{
					this.s = s;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> action)
				public override bool tryAdvance<T1>(Consumer<T1> action)
				{
					Objects.RequireNonNull(action);

					action.Accept(s.Get());
					return true;
				}

				public override Spliterator<T> TrySplit()
				{
					if (Estimate == 0)
					{
						return null;
					}
					return new InfiniteSupplyingSpliterator.OfRef<>(Estimate = (long)((ulong)Estimate >> 1), s);
				}
			}

			internal sealed class OfInt : InfiniteSupplyingSpliterator<Integer>, java.util.Spliterator_OfInt
			{
				internal readonly IntSupplier s;

				internal OfInt(long size, IntSupplier s) : base(size)
				{
					this.s = s;
				}

				public bool TryAdvance(IntConsumer action)
				{
					Objects.RequireNonNull(action);

					action.Accept(s.AsInt);
					return true;
				}

				public java.util.Spliterator_OfInt TrySplit()
				{
					if (Estimate == 0)
					{
						return null;
					}
					return new InfiniteSupplyingSpliterator.OfInt(Estimate = (long)((ulong)Estimate >> 1), s);
				}
			}

			internal sealed class OfLong : InfiniteSupplyingSpliterator<Long>, java.util.Spliterator_OfLong
			{
				internal readonly LongSupplier s;

				internal OfLong(long size, LongSupplier s) : base(size)
				{
					this.s = s;
				}

				public bool TryAdvance(LongConsumer action)
				{
					Objects.RequireNonNull(action);

					action.Accept(s.AsLong);
					return true;
				}

				public java.util.Spliterator_OfLong TrySplit()
				{
					if (Estimate == 0)
					{
						return null;
					}
					return new InfiniteSupplyingSpliterator.OfLong(Estimate = (long)((ulong)Estimate >> 1), s);
				}
			}

			internal sealed class OfDouble : InfiniteSupplyingSpliterator<Double>, java.util.Spliterator_OfDouble
			{
				internal readonly DoubleSupplier s;

				internal OfDouble(long size, DoubleSupplier s) : base(size)
				{
					this.s = s;
				}

				public bool TryAdvance(DoubleConsumer action)
				{
					Objects.RequireNonNull(action);

					action.Accept(s.AsDouble);
					return true;
				}

				public java.util.Spliterator_OfDouble TrySplit()
				{
					if (Estimate == 0)
					{
						return null;
					}
					return new InfiniteSupplyingSpliterator.OfDouble(Estimate = (long)((ulong)Estimate >> 1), s);
				}
			}
		}

		// @@@ Consolidate with Node.Builder
		internal abstract class ArrayBuffer
		{
			internal int Index;

			internal virtual void Reset()
			{
				Index = 0;
			}

			internal sealed class OfRef<T> : ArrayBuffer, Consumer<T>
			{
				internal readonly Object[] Array;

				internal OfRef(int size)
				{
					this.Array = new Object[size];
				}

				public override void Accept(T t)
				{
					Array[Index++] = t;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base T> action, long fence)
				public void forEach<T1>(Consumer<T1> action, long fence)
				{
					for (int i = 0; i < fence; i++)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T t = (T) array[i];
						T t = (T) Array[i];
						action.Accept(t);
					}
				}
			}

			internal abstract class OfPrimitive<T_CONS> : ArrayBuffer
			{
				internal new int Index;

				internal override void Reset()
				{
					Index = 0;
				}

				internal abstract void ForEach(T_CONS action, long fence);
			}

			internal sealed class OfInt : OfPrimitive<IntConsumer>, IntConsumer
			{
				internal readonly int[] Array;

				internal OfInt(int size)
				{
					this.Array = new int[size];
				}

				public void Accept(int t)
				{
					Array[Index++] = t;
				}

				public override void ForEach(IntConsumer action, long fence)
				{
					for (int i = 0; i < fence; i++)
					{
						action.Accept(Array[i]);
					}
				}
			}

			internal sealed class OfLong : OfPrimitive<LongConsumer>, LongConsumer
			{
				internal readonly long[] Array;

				internal OfLong(int size)
				{
					this.Array = new long[size];
				}

				public void Accept(long t)
				{
					Array[Index++] = t;
				}

				public override void ForEach(LongConsumer action, long fence)
				{
					for (int i = 0; i < fence; i++)
					{
						action.Accept(Array[i]);
					}
				}
			}

			internal sealed class OfDouble : OfPrimitive<DoubleConsumer>, DoubleConsumer
			{
				internal readonly double[] Array;

				internal OfDouble(int size)
				{
					this.Array = new double[size];
				}

				public void Accept(double t)
				{
					Array[Index++] = t;
				}

				internal override void ForEach(DoubleConsumer action, long fence)
				{
					for (int i = 0; i < fence; i++)
					{
						action.Accept(Array[i]);
					}
				}
			}
		}
	}


}