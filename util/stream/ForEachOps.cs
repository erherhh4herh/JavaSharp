using System;
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
	/// Factory for creating instances of {@code TerminalOp} that perform an
	/// action for every element of a stream.  Supported variants include unordered
	/// traversal (elements are provided to the {@code Consumer} as soon as they are
	/// available), and ordered traversal (elements are provided to the
	/// {@code Consumer} in encounter order.)
	/// 
	/// <para>Elements are provided to the {@code Consumer} on whatever thread and
	/// whatever order they become available.  For ordered traversals, it is
	/// guaranteed that processing an element <em>happens-before</em> processing
	/// subsequent elements in the encounter order.
	/// 
	/// </para>
	/// <para>Exceptions occurring as a result of sending an element to the
	/// {@code Consumer} will be relayed to the caller and traversal will be
	/// prematurely terminated.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	internal sealed class ForEachOps
	{

		private ForEachOps()
		{
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that perform an action for every element
		/// of a stream.
		/// </summary>
		/// <param name="action"> the {@code Consumer} that receives all elements of a
		///        stream </param>
		/// <param name="ordered"> whether an ordered traversal is requested </param>
		/// @param <T> the type of the stream elements </param>
		/// <returns> the {@code TerminalOp} instance </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> TerminalOp<T, Void> makeRef(java.util.function.Consumer<? base T> action, boolean ordered)
		public static TerminalOp<T, Void> makeRef<T, T1>(Consumer<T1> action, bool ordered)
		{
			Objects.RequireNonNull(action);
			return new ForEachOp.OfRef<>(action, ordered);
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that perform an action for every element
		/// of an {@code IntStream}.
		/// </summary>
		/// <param name="action"> the {@code IntConsumer} that receives all elements of a
		///        stream </param>
		/// <param name="ordered"> whether an ordered traversal is requested </param>
		/// <returns> the {@code TerminalOp} instance </returns>
		public static TerminalOp<Integer, Void> MakeInt(IntConsumer action, bool ordered)
		{
			Objects.RequireNonNull(action);
			return new ForEachOp.OfInt(action, ordered);
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that perform an action for every element
		/// of a {@code LongStream}.
		/// </summary>
		/// <param name="action"> the {@code LongConsumer} that receives all elements of a
		///        stream </param>
		/// <param name="ordered"> whether an ordered traversal is requested </param>
		/// <returns> the {@code TerminalOp} instance </returns>
		public static TerminalOp<Long, Void> MakeLong(LongConsumer action, bool ordered)
		{
			Objects.RequireNonNull(action);
			return new ForEachOp.OfLong(action, ordered);
		}

		/// <summary>
		/// Constructs a {@code TerminalOp} that perform an action for every element
		/// of a {@code DoubleStream}.
		/// </summary>
		/// <param name="action"> the {@code DoubleConsumer} that receives all elements of
		///        a stream </param>
		/// <param name="ordered"> whether an ordered traversal is requested </param>
		/// <returns> the {@code TerminalOp} instance </returns>
		public static TerminalOp<Double, Void> MakeDouble(DoubleConsumer action, bool ordered)
		{
			Objects.RequireNonNull(action);
			return new ForEachOp.OfDouble(action, ordered);
		}

		/// <summary>
		/// A {@code TerminalOp} that evaluates a stream pipeline and sends the
		/// output to itself as a {@code TerminalSink}.  Elements will be sent in
		/// whatever thread they become available.  If the traversal is unordered,
		/// they will be sent independent of the stream's encounter order.
		/// 
		/// <para>This terminal operation is stateless.  For parallel evaluation, each
		/// leaf instance of a {@code ForEachTask} will send elements to the same
		/// {@code TerminalSink} reference that is an instance of this class.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the output type of the stream pipeline </param>
		internal abstract class ForEachOp<T> : TerminalOp<T, Void>, TerminalSink<T, Void>
		{
			internal readonly bool Ordered;

			protected internal ForEachOp(bool ordered)
			{
				this.Ordered = ordered;
			}

			// TerminalOp

			public override int OpFlags
			{
				get
				{
					return Ordered ? TerminalOp_Fields.0 : StreamOpFlag.NOT_ORDERED;
				}
			}

			public override Void evaluateSequential<S>(PipelineHelper<T> helper, Spliterator<S> spliterator)
			{
				return helper.WrapAndCopyInto(this, spliterator).get();
			}

			public override Void evaluateParallel<S>(PipelineHelper<T> helper, Spliterator<S> spliterator)
			{
				if (Ordered)
				{
					(new ForEachOrderedTask<>(helper, spliterator, this)).Invoke();
				}
				else
				{
					(new ForEachTask<>(helper, spliterator, helper.WrapSink(this))).Invoke();
				}
				return null;
			}

			// TerminalSink

			public override Void Get()
			{
				return null;
			}

			// Implementations

			/// <summary>
			/// Implementation class for reference streams </summary>
			internal sealed class OfRef<T> : ForEachOp<T>
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base T> consumer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				internal readonly Consumer<?> Consumer;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: OfRef(java.util.function.Consumer<? base T> consumer, boolean ordered)
				internal OfRef<T1>(Consumer<T1> consumer, bool ordered) : base(ordered)
				{
					this.Consumer = consumer;
				}

				public override void Accept(T t)
				{
					Consumer.Accept(t);
				}
			}

			/// <summary>
			/// Implementation class for {@code IntStream} </summary>
			internal sealed class OfInt : ForEachOp<Integer>, Sink_OfInt
			{
				internal readonly IntConsumer Consumer;

				internal OfInt(IntConsumer consumer, bool ordered) : base(ordered)
				{
					this.Consumer = consumer;
				}

				public override StreamShape InputShape()
				{
					return StreamShape.INT_VALUE;
				}

				public void Accept(int t)
				{
					Consumer.Accept(t);
				}
			}

			/// <summary>
			/// Implementation class for {@code LongStream} </summary>
			internal sealed class OfLong : ForEachOp<Long>, Sink_OfLong
			{
				internal readonly LongConsumer Consumer;

				internal OfLong(LongConsumer consumer, bool ordered) : base(ordered)
				{
					this.Consumer = consumer;
				}

				public override StreamShape InputShape()
				{
					return StreamShape.LONG_VALUE;
				}

				public void Accept(long t)
				{
					Consumer.Accept(t);
				}
			}

			/// <summary>
			/// Implementation class for {@code DoubleStream} </summary>
			internal sealed class OfDouble : ForEachOp<Double>, Sink_OfDouble
			{
				internal readonly DoubleConsumer Consumer;

				internal OfDouble(DoubleConsumer consumer, bool ordered) : base(ordered)
				{
					this.Consumer = consumer;
				}

				public override StreamShape InputShape()
				{
					return StreamShape.DOUBLE_VALUE;
				}

				public void Accept(double t)
				{
					Consumer.Accept(t);
				}
			}
		}

		/// <summary>
		/// A {@code ForkJoinTask} for performing a parallel for-each operation </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachTask<S, T> extends java.util.concurrent.CountedCompleter<Void>
		internal sealed class ForEachTask<S, T> : CountedCompleter<Void>
		{
			internal Spliterator<S> Spliterator;
			internal readonly Sink<S> Sink;
			internal readonly PipelineHelper<T> Helper;
			internal long TargetSize;

			internal ForEachTask(PipelineHelper<T> helper, Spliterator<S> spliterator, Sink<S> sink) : base(null)
			{
				this.Sink = sink;
				this.Helper = helper;
				this.Spliterator = spliterator;
				this.TargetSize = 0L;
			}

			internal ForEachTask(ForEachTask<S, T> parent, Spliterator<S> spliterator) : base(parent)
			{
				this.Spliterator = spliterator;
				this.Sink = parent.Sink;
				this.TargetSize = parent.TargetSize;
				this.Helper = parent.Helper;
			}

			// Similar to AbstractTask but doesn't need to track child tasks
			public void Compute()
			{
				Spliterator<S> rightSplit = Spliterator, leftSplit ;
				long sizeEstimate = rightSplit.EstimateSize(), sizeThreshold ;
				if ((sizeThreshold = TargetSize) == 0L)
				{
					TargetSize = sizeThreshold = AbstractTask.SuggestTargetSize(sizeEstimate);
				}
				bool isShortCircuit = StreamOpFlag.SHORT_CIRCUIT.isKnown(Helper.StreamAndOpFlags);
				bool forkRight = false;
				Sink<S> taskSink = Sink;
				ForEachTask<S, T> task = this;
				while (!isShortCircuit || !taskSink.cancellationRequested())
				{
					if (sizeEstimate <= sizeThreshold || (leftSplit = rightSplit.TrySplit()) == null)
					{
						task.Helper.CopyInto(taskSink, rightSplit);
						break;
					}
					ForEachTask<S, T> leftTask = new ForEachTask<S, T>(task, leftSplit);
					task.AddToPendingCount(1);
					ForEachTask<S, T> taskToFork;
					if (forkRight)
					{
						forkRight = false;
						rightSplit = leftSplit;
						taskToFork = task;
						task = leftTask;
					}
					else
					{
						forkRight = true;
						taskToFork = leftTask;
					}
					taskToFork.Fork();
					sizeEstimate = rightSplit.EstimateSize();
				}
				task.Spliterator = null;
				task.PropagateCompletion();
			}
		}

		/// <summary>
		/// A {@code ForkJoinTask} for performing a parallel for-each operation
		/// which visits the elements in encounter order
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachOrderedTask<S, T> extends java.util.concurrent.CountedCompleter<Void>
		internal sealed class ForEachOrderedTask<S, T> : CountedCompleter<Void>
		{
			/*
			 * Our goal is to ensure that the elements associated with a task are
			 * processed according to an in-order traversal of the computation tree.
			 * We use completion counts for representing these dependencies, so that
			 * a task does not complete until all the tasks preceding it in this
			 * order complete.  We use the "completion map" to associate the next
			 * task in this order for any left child.  We increase the pending count
			 * of any node on the right side of such a mapping by one to indicate
			 * its dependency, and when a node on the left side of such a mapping
			 * completes, it decrements the pending count of its corresponding right
			 * side.  As the computation tree is expanded by splitting, we must
			 * atomically update the mappings to maintain the invariant that the
			 * completion map maps left children to the next node in the in-order
			 * traversal.
			 *
			 * Take, for example, the following computation tree of tasks:
			 *
			 *       a
			 *      / \
			 *     b   c
			 *    / \ / \
			 *   d  e f  g
			 *
			 * The complete map will contain (not necessarily all at the same time)
			 * the following associations:
			 *
			 *   d -> e
			 *   b -> f
			 *   f -> g
			 *
			 * Tasks e, f, g will have their pending counts increased by 1.
			 *
			 * The following relationships hold:
			 *
			 *   - completion of d "happens-before" e;
			 *   - completion of d and e "happens-before b;
			 *   - completion of b "happens-before" f; and
			 *   - completion of f "happens-before" g
			 *
			 * Thus overall the "happens-before" relationship holds for the
			 * reporting of elements, covered by tasks d, e, f and g, as specified
			 * by the forEachOrdered operation.
			 */

			internal readonly PipelineHelper<T> Helper;
			internal Spliterator<S> Spliterator;
			internal readonly long TargetSize;
			internal readonly ConcurrentDictionary<ForEachOrderedTask<S, T>, ForEachOrderedTask<S, T>> CompletionMap;
			internal readonly Sink<T> Action;
			internal readonly ForEachOrderedTask<S, T> LeftPredecessor;
			internal Node<T> Node;

			protected internal ForEachOrderedTask(PipelineHelper<T> helper, Spliterator<S> spliterator, Sink<T> action) : base(null)
			{
				this.Helper = helper;
				this.Spliterator = spliterator;
				this.TargetSize = AbstractTask.SuggestTargetSize(spliterator.EstimateSize());
				// Size map to avoid concurrent re-sizes
				this.CompletionMap = new ConcurrentDictionary<>(System.Math.Max(16, AbstractTask.LEAF_TARGET << 1));
				this.Action = action;
				this.LeftPredecessor = null;
			}

			internal ForEachOrderedTask(ForEachOrderedTask<S, T> parent, Spliterator<S> spliterator, ForEachOrderedTask<S, T> leftPredecessor) : base(parent)
			{
				this.Helper = parent.Helper;
				this.Spliterator = spliterator;
				this.TargetSize = parent.TargetSize;
				this.CompletionMap = parent.CompletionMap;
				this.Action = parent.Action;
				this.LeftPredecessor = leftPredecessor;
			}

			public override void Compute()
			{
				DoCompute(this);
			}

			internal static void doCompute<S, T>(ForEachOrderedTask<S, T> task)
			{
				Spliterator<S> rightSplit = task.Spliterator, leftSplit ;
				long sizeThreshold = task.TargetSize;
				bool forkRight = false;
				while (rightSplit.EstimateSize() > sizeThreshold && (leftSplit = rightSplit.TrySplit()) != null)
				{
					ForEachOrderedTask<S, T> leftChild = new ForEachOrderedTask<S, T>(task, leftSplit, task.LeftPredecessor);
					ForEachOrderedTask<S, T> rightChild = new ForEachOrderedTask<S, T>(task, rightSplit, leftChild);

					// Fork the parent task
					// Completion of the left and right children "happens-before"
					// completion of the parent
					task.AddToPendingCount(1);
					// Completion of the left child "happens-before" completion of
					// the right child
					rightChild.AddToPendingCount(1);
					task.CompletionMap[leftChild] = rightChild;

					// If task is not on the left spine
					if (task.LeftPredecessor != null)
					{
						/*
						 * Completion of left-predecessor, or left subtree,
						 * "happens-before" completion of left-most leaf node of
						 * right subtree.
						 * The left child's pending count needs to be updated before
						 * it is associated in the completion map, otherwise the
						 * left child can complete prematurely and violate the
						 * "happens-before" constraint.
						 */
						leftChild.AddToPendingCount(1);
						// Update association of left-predecessor to left-most
						// leaf node of right subtree
						if (task.CompletionMap.replace(task.LeftPredecessor, task, leftChild))
						{
							// If replaced, adjust the pending count of the parent
							// to complete when its children complete
							task.AddToPendingCount(-1);
						}
						else
						{
							// Left-predecessor has already completed, parent's
							// pending count is adjusted by left-predecessor;
							// left child is ready to complete
							leftChild.AddToPendingCount(-1);
						}
					}

					ForEachOrderedTask<S, T> taskToFork;
					if (forkRight)
					{
						forkRight = false;
						rightSplit = leftSplit;
						task = leftChild;
						taskToFork = rightChild;
					}
					else
					{
						forkRight = true;
						task = rightChild;
						taskToFork = leftChild;
					}
					taskToFork.Fork();
				}

				/*
				 * Task's pending count is either 0 or 1.  If 1 then the completion
				 * map will contain a value that is task, and two calls to
				 * tryComplete are required for completion, one below and one
				 * triggered by the completion of task's left-predecessor in
				 * onCompletion.  Therefore there is no data race within the if
				 * block.
				 */
				if (task.PendingCount > 0)
				{
					// Cannot complete just yet so buffer elements into a Node
					// for use when completion occurs
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.function.IntFunction<T[]> generator = size -> (T[]) new Object[size];
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					IntFunction<T[]> generator = size => (T[]) new Object[size];
					Node_Builder<T> nb = task.Helper.MakeNodeBuilder(task.Helper.ExactOutputSizeIfKnown(rightSplit), generator);
					task.Node = task.Helper.WrapAndCopyInto(nb, rightSplit).build();
					task.Spliterator = null;
				}
				task.TryComplete();
			}

			public override void onCompletion<T1>(CountedCompleter<T1> caller)
			{
				if (Node != null)
				{
					// Dump buffered elements from this leaf into the sink
					Node.ForEach(Action);
					Node = null;
				}
				else if (Spliterator != null)
				{
					// Dump elements output from this leaf's pipeline into the sink
					Helper.WrapAndCopyInto(Action, Spliterator);
					Spliterator = null;
				}

				// The completion of this task *and* the dumping of elements
				// "happens-before" completion of the associated left-most leaf task
				// of right subtree (if any, which can be this task's right sibling)
				//
				ForEachOrderedTask<S, T> leftDescendant = CompletionMap.Remove(this);
				if (leftDescendant != null)
				{
					leftDescendant.TryComplete();
				}
			}
		}
	}

}