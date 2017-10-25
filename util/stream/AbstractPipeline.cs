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
	/// Abstract base class for "pipeline" classes, which are the core
	/// implementations of the Stream interface and its primitive specializations.
	/// Manages construction and evaluation of stream pipelines.
	/// 
	/// <para>An {@code AbstractPipeline} represents an initial portion of a stream
	/// pipeline, encapsulating a stream source and zero or more intermediate
	/// operations.  The individual {@code AbstractPipeline} objects are often
	/// referred to as <em>stages</em>, where each stage describes either the stream
	/// source or an intermediate operation.
	/// 
	/// </para>
	/// <para>A concrete intermediate stage is generally built from an
	/// {@code AbstractPipeline}, a shape-specific pipeline class which extends it
	/// (e.g., {@code IntPipeline}) which is also abstract, and an operation-specific
	/// concrete class which extends that.  {@code AbstractPipeline} contains most of
	/// the mechanics of evaluating the pipeline, and implements methods that will be
	/// used by the operation; the shape-specific classes add helper methods for
	/// dealing with collection of results into the appropriate shape-specific
	/// containers.
	/// 
	/// </para>
	/// <para>After chaining a new intermediate operation, or executing a terminal
	/// operation, the stream is considered to be consumed, and no more intermediate
	/// or terminal operations are permitted on this stream instance.
	/// 
	/// @implNote
	/// </para>
	/// <para>For sequential streams, and parallel streams without
	/// <a href="package-summary.html#StreamOps">stateful intermediate
	/// operations</a>, parallel streams, pipeline evaluation is done in a single
	/// pass that "jams" all the operations together.  For parallel streams with
	/// stateful operations, execution is divided into segments, where each
	/// stateful operations marks the end of a segment, and each segment is
	/// evaluated separately and the result used as the input to the next
	/// segment.  In all cases, the source data is not consumed until a terminal
	/// operation begins.
	/// 
	/// </para>
	/// </summary>
	/// @param <E_IN>  type of input elements </param>
	/// @param <E_OUT> type of output elements </param>
	/// @param <S> type of the subclass implementing {@code BaseStream}
	/// @since 1.8 </param>
	internal abstract class AbstractPipeline<E_IN, E_OUT, S> : PipelineHelper<E_OUT>, BaseStream<E_OUT, S> where S : BaseStream<E_OUT, S>
	{
		private const String MSG_STREAM_LINKED = "stream has already been operated upon or closed";
		private const String MSG_CONSUMED = "source already consumed or closed";

		/// <summary>
		/// Backlink to the head of the pipeline chain (self if this is the source
		/// stage).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private final AbstractPipeline sourceStage;
		private readonly AbstractPipeline SourceStage;

		/// <summary>
		/// The "upstream" pipeline, or null if this is the source stage.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private final AbstractPipeline previousStage;
		private readonly AbstractPipeline PreviousStage;

		/// <summary>
		/// The operation flags for the intermediate operation represented by this
		/// pipeline object.
		/// </summary>
		protected internal readonly int SourceOrOpFlags;

		/// <summary>
		/// The next stage in the pipeline, or null if this is the last stage.
		/// Effectively final at the point of linking to the next pipeline.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private AbstractPipeline nextStage;
		private AbstractPipeline NextStage;

		/// <summary>
		/// The number of intermediate operations between this pipeline object
		/// and the stream source if sequential, or the previous stateful if parallel.
		/// Valid at the point of pipeline preparation for evaluation.
		/// </summary>
		private int Depth;

		/// <summary>
		/// The combined source and operation flags for the source and all operations
		/// up to and including the operation represented by this pipeline object.
		/// Valid at the point of pipeline preparation for evaluation.
		/// </summary>
		private int CombinedFlags;

		/// <summary>
		/// The source spliterator. Only valid for the head pipeline.
		/// Before the pipeline is consumed if non-null then {@code sourceSupplier}
		/// must be null. After the pipeline is consumed if non-null then is set to
		/// null.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.util.Spliterator<?> sourceSpliterator;
		private Spliterator<?> SourceSpliterator_Renamed;

		/// <summary>
		/// The source supplier. Only valid for the head pipeline. Before the
		/// pipeline is consumed if non-null then {@code sourceSpliterator} must be
		/// null. After the pipeline is consumed if non-null then is set to null.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.util.function.Supplier<? extends java.util.Spliterator<?>> sourceSupplier;
		private Supplier<?> SourceSupplier;

		/// <summary>
		/// True if this pipeline has been linked or consumed
		/// </summary>
		private bool LinkedOrConsumed;

		/// <summary>
		/// True if there are any stateful ops in the pipeline; only valid for the
		/// source stage.
		/// </summary>
		private bool SourceAnyStateful;

		private Runnable SourceCloseAction;

		/// <summary>
		/// True if pipeline is parallel, otherwise the pipeline is sequential; only
		/// valid for the source stage.
		/// </summary>
		private bool Parallel_Renamed;

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		/// <param name="sourceFlags"> The source flags for the stream source, described in
		/// <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> True if the pipeline is parallel </param>
		internal AbstractPipeline<T1>(Supplier<T1> source, int sourceFlags, bool parallel) where T1 : java.util.Spliterator<T1>
		{
			this.PreviousStage = null;
			this.SourceSupplier = source;
			this.SourceStage = this;
			this.SourceOrOpFlags = sourceFlags & StreamOpFlag.STREAM_MASK;
			// The following is an optimization of:
			// StreamOpFlag.combineOpFlags(sourceOrOpFlags, StreamOpFlag.INITIAL_OPS_VALUE);
			this.CombinedFlags = (~(SourceOrOpFlags << 1)) & StreamOpFlag.INITIAL_OPS_VALUE;
			this.Depth = 0;
			this.Parallel_Renamed = parallel;
		}

		/// <summary>
		/// Constructor for the head of a stream pipeline.
		/// </summary>
		/// <param name="source"> {@code Spliterator} describing the stream source </param>
		/// <param name="sourceFlags"> the source flags for the stream source, described in
		/// <seealso cref="StreamOpFlag"/> </param>
		/// <param name="parallel"> {@code true} if the pipeline is parallel </param>
		internal AbstractPipeline<T1>(Spliterator<T1> source, int sourceFlags, bool parallel)
		{
			this.PreviousStage = null;
			this.SourceSpliterator_Renamed = source;
			this.SourceStage = this;
			this.SourceOrOpFlags = sourceFlags & StreamOpFlag.STREAM_MASK;
			// The following is an optimization of:
			// StreamOpFlag.combineOpFlags(sourceOrOpFlags, StreamOpFlag.INITIAL_OPS_VALUE);
			this.CombinedFlags = (~(SourceOrOpFlags << 1)) & StreamOpFlag.INITIAL_OPS_VALUE;
			this.Depth = 0;
			this.Parallel_Renamed = parallel;
		}

		/// <summary>
		/// Constructor for appending an intermediate operation stage onto an
		/// existing pipeline.
		/// </summary>
		/// <param name="previousStage"> the upstream pipeline stage </param>
		/// <param name="opFlags"> the operation flags for the new stage, described in
		/// <seealso cref="StreamOpFlag"/> </param>
		internal AbstractPipeline<T1>(AbstractPipeline<T1> previousStage, int opFlags)
		{
			if (previousStage.LinkedOrConsumed)
			{
				throw new IllegalStateException(MSG_STREAM_LINKED);
			}
			previousStage.LinkedOrConsumed = true;
			previousStage.NextStage = this;

			this.PreviousStage = previousStage;
			this.SourceOrOpFlags = opFlags & StreamOpFlag.OP_MASK;
			this.CombinedFlags = StreamOpFlag.combineOpFlags(opFlags, previousStage.CombinedFlags);
			this.SourceStage = previousStage.SourceStage;
			if (OpIsStateful())
			{
				SourceStage.SourceAnyStateful = true;
			}
			this.Depth = previousStage.Depth + 1;
		}


		// Terminal evaluation methods

		/// <summary>
		/// Evaluate the pipeline with a terminal operation to produce a result.
		/// </summary>
		/// @param <R> the type of result </param>
		/// <param name="terminalOp"> the terminal operation to be applied to the pipeline. </param>
		/// <returns> the result </returns>
		internal R evaluate<R>(TerminalOp<E_OUT, R> terminalOp)
		{
			Debug.Assert(OutputShape == terminalOp.inputShape());
			if (LinkedOrConsumed)
			{
				throw new IllegalStateException(MSG_STREAM_LINKED);
			}
			LinkedOrConsumed = true;

			return Parallel ? terminalOp.evaluateParallel(this, SourceSpliterator(terminalOp.OpFlags)) : terminalOp.EvaluateSequential(this, SourceSpliterator(terminalOp.OpFlags));
		}

		/// <summary>
		/// Collect the elements output from the pipeline stage.
		/// </summary>
		/// <param name="generator"> the array generator to be used to create array instances </param>
		/// <returns> a flat array-backed Node that holds the collected output elements </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final Node<E_OUT> evaluateToArrayNode(java.util.function.IntFunction<E_OUT[]> generator)
		internal Node<E_OUT> EvaluateToArrayNode(IntFunction<E_OUT[]> generator)
		{
			if (LinkedOrConsumed)
			{
				throw new IllegalStateException(MSG_STREAM_LINKED);
			}
			LinkedOrConsumed = true;

			// If the last intermediate operation is stateful then
			// evaluate directly to avoid an extra collection step
			if (Parallel && PreviousStage != null && OpIsStateful())
			{
				// Set the depth of this, last, pipeline stage to zero to slice the
				// pipeline such that this operation will not be included in the
				// upstream slice and upstream operations will not be included
				// in this slice
				Depth = 0;
				return OpEvaluateParallel(PreviousStage, PreviousStage.SourceSpliterator(0), generator);
			}
			else
			{
				return Evaluate(SourceSpliterator(0), true, generator);
			}
		}

		/// <summary>
		/// Gets the source stage spliterator if this pipeline stage is the source
		/// stage.  The pipeline is consumed after this method is called and
		/// returns successfully.
		/// </summary>
		/// <returns> the source stage spliterator </returns>
		/// <exception cref="IllegalStateException"> if this pipeline stage is not the source
		///         stage. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final java.util.Spliterator<E_OUT> sourceStageSpliterator()
		internal Spliterator<E_OUT> SourceStageSpliterator()
		{
			if (this != SourceStage)
			{
				throw new IllegalStateException();
			}

			if (LinkedOrConsumed)
			{
				throw new IllegalStateException(MSG_STREAM_LINKED);
			}
			LinkedOrConsumed = true;

			if (SourceStage.SourceSpliterator_Renamed != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Spliterator<E_OUT> s = sourceStage.sourceSpliterator;
				Spliterator<E_OUT> s = SourceStage.SourceSpliterator_Renamed;
				SourceStage.SourceSpliterator_Renamed = null;
				return s;
			}
			else if (SourceStage.SourceSupplier != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Spliterator<E_OUT> s = (java.util.Spliterator<E_OUT>) sourceStage.sourceSupplier.get();
				Spliterator<E_OUT> s = (Spliterator<E_OUT>) SourceStage.SourceSupplier.Get();
				SourceStage.SourceSupplier = null;
				return s;
			}
			else
			{
				throw new IllegalStateException(MSG_CONSUMED);
			}
		}

		// BaseStream

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final S sequential()
		public override S Sequential()
		{
			SourceStage.Parallel_Renamed = false;
			return (S) this;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final S parallel()
		public override S Parallel()
		{
			SourceStage.Parallel_Renamed = true;
			return (S) this;
		}

		public override void Close()
		{
			LinkedOrConsumed = true;
			SourceSupplier = null;
			SourceSpliterator_Renamed = null;
			if (SourceStage.SourceCloseAction != null)
			{
				Runnable closeAction = SourceStage.SourceCloseAction;
				SourceStage.SourceCloseAction = null;
				closeAction.Run();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public S onClose(Runnable closeHandler)
		public override S OnClose(Runnable closeHandler)
		{
			Runnable existingHandler = SourceStage.SourceCloseAction;
			SourceStage.SourceCloseAction = (existingHandler == null) ? closeHandler : Streams.ComposeWithExceptions(existingHandler, closeHandler);
			return (S) this;
		}

		// Primitive specialization use co-variant overrides, hence is not final
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public java.util.Spliterator<E_OUT> spliterator()
		public override Spliterator<E_OUT> Spliterator()
		{
			if (LinkedOrConsumed)
			{
				throw new IllegalStateException(MSG_STREAM_LINKED);
			}
			LinkedOrConsumed = true;

			if (this == SourceStage)
			{
				if (SourceStage.SourceSpliterator_Renamed != null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Spliterator<E_OUT> s = (java.util.Spliterator<E_OUT>) sourceStage.sourceSpliterator;
					Spliterator<E_OUT> s = (Spliterator<E_OUT>) SourceStage.SourceSpliterator_Renamed;
					SourceStage.SourceSpliterator_Renamed = null;
					return s;
				}
				else if (SourceStage.SourceSupplier != null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.Supplier<java.util.Spliterator<E_OUT>> s = (java.util.function.Supplier<java.util.Spliterator<E_OUT>>) sourceStage.sourceSupplier;
					Supplier<Spliterator<E_OUT>> s = (Supplier<Spliterator<E_OUT>>) SourceStage.SourceSupplier;
					SourceStage.SourceSupplier = null;
					return LazySpliterator(s);
				}
				else
				{
					throw new IllegalStateException(MSG_CONSUMED);
				}
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return Wrap(this, () => SourceSpliterator(0), Parallel);
			}
		}

		public override bool Parallel
		{
			get
			{
				return SourceStage.Parallel_Renamed;
			}
		}


		/// <summary>
		/// Returns the composition of stream flags of the stream source and all
		/// intermediate operations.
		/// </summary>
		/// <returns> the composition of stream flags of the stream source and all
		///         intermediate operations </returns>
		/// <seealso cref= StreamOpFlag </seealso>
		internal int StreamFlags
		{
			get
			{
				return StreamOpFlag.toStreamFlags(CombinedFlags);
			}
		}

		/// <summary>
		/// Get the source spliterator for this pipeline stage.  For a sequential or
		/// stateless parallel pipeline, this is the source spliterator.  For a
		/// stateful parallel pipeline, this is a spliterator describing the results
		/// of all computations up to and including the most recent stateful
		/// operation.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private java.util.Spliterator<?> sourceSpliterator(int terminalFlags)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private Spliterator<?> SourceSpliterator(int terminalFlags)
		{
			// Get the source spliterator of the pipeline
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Spliterator<?> spliterator = null;
			Spliterator<?> spliterator = null;
			if (SourceStage.SourceSpliterator_Renamed != null)
			{
				spliterator = SourceStage.SourceSpliterator_Renamed;
				SourceStage.SourceSpliterator_Renamed = null;
			}
			else if (SourceStage.SourceSupplier != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: spliterator = (java.util.Spliterator<?>) sourceStage.sourceSupplier.get();
				spliterator = (Spliterator<?>) SourceStage.SourceSupplier.Get();
				SourceStage.SourceSupplier = null;
			}
			else
			{
				throw new IllegalStateException(MSG_CONSUMED);
			}

			if (Parallel && SourceStage.SourceAnyStateful)
			{
				// Adapt the source spliterator, evaluating each stateful op
				// in the pipeline up to and including this pipeline stage.
				// The depth and flags of each pipeline stage are adjusted accordingly.
				int depth = 1;
				for (@SuppressWarnings("rawtypes") AbstractPipeline u = SourceStage, p = SourceStage.NextStage, e = this; u != e; u = p, p = p.nextStage)
				{

					int thisOpFlags = p.sourceOrOpFlags;
					if (p.opIsStateful())
					{
						depth = 0;

						if (StreamOpFlag.SHORT_CIRCUIT.isKnown(thisOpFlags))
						{
							// Clear the short circuit flag for next pipeline stage
							// This stage encapsulates short-circuiting, the next
							// stage may not have any short-circuit operations, and
							// if so spliterator.forEachRemaining should be used
							// for traversal
							thisOpFlags = thisOpFlags & ~StreamOpFlag.IS_SHORT_CIRCUIT;
						}

						spliterator = p.opEvaluateParallelLazy(u, spliterator);

						// Inject or clear SIZED on the source pipeline stage
						// based on the stage's spliterator
						thisOpFlags = spliterator.hasCharacteristics(java.util.Spliterator_Fields.SIZED) ? (thisOpFlags & ~StreamOpFlag.NOT_SIZED) | StreamOpFlag.IS_SIZED : (thisOpFlags & ~StreamOpFlag.IS_SIZED) | StreamOpFlag.NOT_SIZED;
					}
					p.depth = depth++;
					p.combinedFlags = StreamOpFlag.combineOpFlags(thisOpFlags, u.combinedFlags);
				}
			}

			if (terminalFlags != 0)
			{
				// Apply flags from the terminal operation to last pipeline stage
				CombinedFlags = StreamOpFlag.combineOpFlags(terminalFlags, CombinedFlags);
			}

			return spliterator;
		}

		// PipelineHelper

		internal override StreamShape SourceShape
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("rawtypes") AbstractPipeline p = AbstractPipeline.this;
				AbstractPipeline p = AbstractPipeline.this;
				while (p.Depth > 0)
				{
					p = p.PreviousStage;
				}
				return p.OutputShape;
			}
		}

		internal override long exactOutputSizeIfKnown<P_IN>(Spliterator<P_IN> spliterator)
		{
			return StreamOpFlag.SIZED.isKnown(StreamAndOpFlags) ? spliterator.ExactSizeIfKnown : -1;
		}

		internal override S wrapAndCopyInto<P_IN, S>(S sink, Spliterator<P_IN> spliterator) where S : Sink<E_OUT>
		{
			CopyInto(WrapSink(Objects.RequireNonNull(sink)), spliterator);
			return sink;
		}

		internal override void copyInto<P_IN>(Sink<P_IN> wrappedSink, Spliterator<P_IN> spliterator)
		{
			Objects.RequireNonNull(wrappedSink);

			if (!StreamOpFlag.SHORT_CIRCUIT.isKnown(StreamAndOpFlags))
			{
				wrappedSink.begin(spliterator.ExactSizeIfKnown);
				spliterator.forEachRemaining(wrappedSink);
				wrappedSink.end();
			}
			else
			{
				CopyIntoWithCancel(wrappedSink, spliterator);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final <P_IN> void copyIntoWithCancel(Sink<P_IN> wrappedSink, java.util.Spliterator<P_IN> spliterator)
		internal override void copyIntoWithCancel<P_IN>(Sink<P_IN> wrappedSink, Spliterator<P_IN> spliterator)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes","unchecked"}) AbstractPipeline p = AbstractPipeline.this;
			AbstractPipeline p = AbstractPipeline.this;
			while (p.Depth > 0)
			{
				p = p.PreviousStage;
			}
			wrappedSink.begin(spliterator.ExactSizeIfKnown);
			p.ForEachWithCancel(spliterator, wrappedSink);
			wrappedSink.end();
		}

		internal override int StreamAndOpFlags
		{
			get
			{
				return CombinedFlags;
			}
		}

		internal bool Ordered
		{
			get
			{
				return StreamOpFlag.ORDERED.isKnown(CombinedFlags);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final <P_IN> Sink<P_IN> wrapSink(Sink<E_OUT> sink)
		internal override Sink<P_IN> wrapSink<P_IN>(Sink<E_OUT> sink)
		{
			Objects.RequireNonNull(sink);

			for (@SuppressWarnings("rawtypes") AbstractPipeline p = AbstractPipeline.this; p.depth > 0; p = p.previousStage)
			{
				sink = p.opWrapSink(p.previousStage.combinedFlags, sink);
			}
			return (Sink<P_IN>) sink;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final <P_IN> java.util.Spliterator<E_OUT> wrapSpliterator(java.util.Spliterator<P_IN> sourceSpliterator)
		internal override Spliterator<E_OUT> wrapSpliterator<P_IN>(Spliterator<P_IN> sourceSpliterator)
		{
			if (Depth == 0)
			{
				return (Spliterator<E_OUT>) sourceSpliterator;
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
				return Wrap(this, () => sourceSpliterator, Parallel);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") final <P_IN> Node<E_OUT> evaluate(java.util.Spliterator<P_IN> spliterator, boolean flatten, java.util.function.IntFunction<E_OUT[]> generator)
		internal override Node<E_OUT> evaluate<P_IN>(Spliterator<P_IN> spliterator, bool flatten, IntFunction<E_OUT[]> generator)
		{
			if (Parallel)
			{
				// @@@ Optimize if op of this pipeline stage is a stateful op
				return EvaluateToNode(this, spliterator, flatten, generator);
			}
			else
			{
				Node_Builder<E_OUT> nb = MakeNodeBuilder(ExactOutputSizeIfKnown(spliterator), generator);
				return WrapAndCopyInto(nb, spliterator).build();
			}
		}


		// Shape-specific abstract methods, implemented by XxxPipeline classes

		/// <summary>
		/// Get the output shape of the pipeline.  If the pipeline is the head,
		/// then it's output shape corresponds to the shape of the source.
		/// Otherwise, it's output shape corresponds to the output shape of the
		/// associated operation.
		/// </summary>
		/// <returns> the output shape </returns>
		internal abstract StreamShape OutputShape {get;}

		/// <summary>
		/// Collect elements output from a pipeline into a Node that holds elements
		/// of this shape.
		/// </summary>
		/// <param name="helper"> the pipeline helper describing the pipeline stages </param>
		/// <param name="spliterator"> the source spliterator </param>
		/// <param name="flattenTree"> true if the returned node should be flattened </param>
		/// <param name="generator"> the array generator </param>
		/// <returns> a Node holding the output of the pipeline </returns>
		internal abstract Node<E_OUT> evaluateToNode<P_IN>(PipelineHelper<E_OUT> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<E_OUT[]> generator);

		/// <summary>
		/// Create a spliterator that wraps a source spliterator, compatible with
		/// this stream shape, and operations associated with a {@link
		/// PipelineHelper}.
		/// </summary>
		/// <param name="ph"> the pipeline helper describing the pipeline stages </param>
		/// <param name="supplier"> the supplier of a spliterator </param>
		/// <returns> a wrapping spliterator compatible with this shape </returns>
		internal abstract Spliterator<E_OUT> wrap<P_IN>(PipelineHelper<E_OUT> ph, Supplier<Spliterator<P_IN>> supplier, bool isParallel);

		/// <summary>
		/// Create a lazy spliterator that wraps and obtains the supplied the
		/// spliterator when a method is invoked on the lazy spliterator. </summary>
		/// <param name="supplier"> the supplier of a spliterator </param>
		internal abstract Spliterator<E_OUT> LazySpliterator(Supplier<T1> supplier) where T1 : java.util.Spliterator<E_OUT>;

		/// <summary>
		/// Traverse the elements of a spliterator compatible with this stream shape,
		/// pushing those elements into a sink.   If the sink requests cancellation,
		/// no further elements will be pulled or pushed.
		/// </summary>
		/// <param name="spliterator"> the spliterator to pull elements from </param>
		/// <param name="sink"> the sink to push elements to </param>
		internal abstract void ForEachWithCancel(Spliterator<E_OUT> spliterator, Sink<E_OUT> sink);

		/// <summary>
		/// Make a node builder compatible with this stream shape.
		/// </summary>
		/// <param name="exactSizeIfKnown"> if {@literal >=0}, then a node builder will be
		/// created that has a fixed capacity of at most sizeIfKnown elements. If
		/// {@literal < 0}, then the node builder has an unfixed capacity. A fixed
		/// capacity node builder will throw exceptions if an element is added after
		/// builder has reached capacity, or is built before the builder has reached
		/// capacity.
		/// </param>
		/// <param name="generator"> the array generator to be used to create instances of a
		/// T[] array. For implementations supporting primitive nodes, this parameter
		/// may be ignored. </param>
		/// <returns> a node builder </returns>
		internal override abstract Node_Builder<E_OUT> MakeNodeBuilder(long exactSizeIfKnown, IntFunction<E_OUT[]> generator);


		// Op-specific abstract methods, implemented by the operation class

		/// <summary>
		/// Returns whether this operation is stateful or not.  If it is stateful,
		/// then the method
		/// <seealso cref="#opEvaluateParallel(PipelineHelper, java.util.Spliterator, java.util.function.IntFunction)"/>
		/// must be overridden.
		/// </summary>
		/// <returns> {@code true} if this operation is stateful </returns>
		internal abstract bool OpIsStateful();

		/// <summary>
		/// Accepts a {@code Sink} which will receive the results of this operation,
		/// and return a {@code Sink} which accepts elements of the input type of
		/// this operation and which performs the operation, passing the results to
		/// the provided {@code Sink}.
		/// 
		/// @apiNote
		/// The implementation may use the {@code flags} parameter to optimize the
		/// sink wrapping.  For example, if the input is already {@code DISTINCT},
		/// the implementation for the {@code Stream#distinct()} method could just
		/// return the sink it was passed.
		/// </summary>
		/// <param name="flags"> The combined stream and operation flags up to, but not
		///        including, this operation </param>
		/// <param name="sink"> sink to which elements should be sent after processing </param>
		/// <returns> a sink which accepts elements, perform the operation upon
		///         each element, and passes the results (if any) to the provided
		///         {@code Sink}. </returns>
		internal abstract Sink<E_IN> OpWrapSink(int flags, Sink<E_OUT> sink);

		/// <summary>
		/// Performs a parallel evaluation of the operation using the specified
		/// {@code PipelineHelper} which describes the upstream intermediate
		/// operations.  Only called on stateful operations.  If {@link
		/// #opIsStateful()} returns true then implementations must override the
		/// default implementation.
		/// 
		/// @implSpec The default implementation always throw
		/// {@code UnsupportedOperationException}.
		/// </summary>
		/// <param name="helper"> the pipeline helper describing the pipeline stages </param>
		/// <param name="spliterator"> the source {@code Spliterator} </param>
		/// <param name="generator"> the array generator </param>
		/// <returns> a {@code Node} describing the result of the evaluation </returns>
		internal virtual Node<E_OUT> opEvaluateParallel<P_IN>(PipelineHelper<E_OUT> helper, Spliterator<P_IN> spliterator, IntFunction<E_OUT[]> generator)
		{
			throw new UnsupportedOperationException("Parallel evaluation is not supported");
		}

		/// <summary>
		/// Returns a {@code Spliterator} describing a parallel evaluation of the
		/// operation, using the specified {@code PipelineHelper} which describes the
		/// upstream intermediate operations.  Only called on stateful operations.
		/// It is not necessary (though acceptable) to do a full computation of the
		/// result here; it is preferable, if possible, to describe the result via a
		/// lazily evaluated spliterator.
		/// 
		/// @implSpec The default implementation behaves as if:
		/// <pre>{@code
		///     return evaluateParallel(helper, i -> (E_OUT[]) new
		/// Object[i]).spliterator();
		/// }</pre>
		/// and is suitable for implementations that cannot do better than a full
		/// synchronous evaluation.
		/// </summary>
		/// <param name="helper"> the pipeline helper </param>
		/// <param name="spliterator"> the source {@code Spliterator} </param>
		/// <returns> a {@code Spliterator} describing the result of the evaluation </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") <P_IN> java.util.Spliterator<E_OUT> opEvaluateParallelLazy(PipelineHelper<E_OUT> helper, java.util.Spliterator<P_IN> spliterator)
		internal virtual Spliterator<E_OUT> opEvaluateParallelLazy<P_IN>(PipelineHelper<E_OUT> helper, Spliterator<P_IN> spliterator)
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return OpEvaluateParallel(helper, spliterator, i => (E_OUT[]) new Object[i]).Spliterator();
		}
	}

}