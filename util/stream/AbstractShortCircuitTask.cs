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
	/// Abstract class for fork-join tasks used to implement short-circuiting
	/// stream ops, which can produce a result without processing all elements of the
	/// stream.
	/// </summary>
	/// @param <P_IN> type of input elements to the pipeline </param>
	/// @param <P_OUT> type of output elements from the pipeline </param>
	/// @param <R> type of intermediate result, may be different from operation
	///        result type </param>
	/// @param <K> type of child and sibling tasks
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract class AbstractShortCircuitTask<P_IN, P_OUT, R, K extends AbstractShortCircuitTask<P_IN, P_OUT, R, K>> extends AbstractTask<P_IN, P_OUT, R, K>
	internal abstract class AbstractShortCircuitTask<P_IN, P_OUT, R, K> : AbstractTask<P_IN, P_OUT, R, K> where K : AbstractShortCircuitTask<P_IN, P_OUT, R, K>
	{
		/// <summary>
		/// The result for this computation; this is shared among all tasks and set
		/// exactly once
		/// </summary>
		protected internal readonly AtomicReference<R> SharedResult;

		/// <summary>
		/// Indicates whether this task has been canceled.  Tasks may cancel other
		/// tasks in the computation under various conditions, such as in a
		/// find-first operation, a task that finds a value will cancel all tasks
		/// that are later in the encounter order.
		/// </summary>
		protected internal volatile bool Canceled;

		/// <summary>
		/// Constructor for root tasks.
		/// </summary>
		/// <param name="helper"> the {@code PipelineHelper} describing the stream pipeline
		///               up to this operation </param>
		/// <param name="spliterator"> the {@code Spliterator} describing the source for this
		///                    pipeline </param>
		protected internal AbstractShortCircuitTask(PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator)
		{
			SharedResult = new AtomicReference<>(null);
		}

		/// <summary>
		/// Constructor for non-root nodes.
		/// </summary>
		/// <param name="parent"> parent task in the computation tree </param>
		/// <param name="spliterator"> the {@code Spliterator} for the portion of the
		///                    computation tree described by this task </param>
		protected internal AbstractShortCircuitTask(K parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
		{
			SharedResult = parent.sharedResult;
		}

		/// <summary>
		/// Returns the value indicating the computation completed with no task
		/// finding a short-circuitable result.  For example, for a "find" operation,
		/// this might be null or an empty {@code Optional}.
		/// </summary>
		/// <returns> the result to return when no task finds a result </returns>
		protected internal abstract R EmptyResult {get;}

		/// <summary>
		/// Overrides AbstractTask version to include checks for early
		/// exits while splitting or computing.
		/// </summary>
		public override void Compute()
		{
			Spliterator<P_IN> rs = spliterator, ls ;
			long sizeEstimate = rs.EstimateSize();
			long sizeThreshold = getTargetSize(sizeEstimate);
			bool forkRight = false;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K task = (K) this;
			K task = (K) this;
			AtomicReference<R> sr = SharedResult;
			R result;
			while ((result = sr.Get()) == null)
			{
				if (task.taskCanceled())
				{
					result = task.EmptyResult;
					break;
				}
				if (sizeEstimate <= sizeThreshold || (ls = rs.TrySplit()) == null)
				{
					result = task.doLeaf();
					break;
				}
				K leftChild, rightChild, taskToFork;
				task.leftChild = leftChild = task.makeChild(ls);
				task.rightChild = rightChild = task.makeChild(rs);
				task.PendingCount = 1;
				if (forkRight)
				{
					forkRight = false;
					rs = ls;
					task = leftChild;
					taskToFork = rightChild;
				}
				else
				{
					forkRight = true;
					task = rightChild;
					taskToFork = leftChild;
				}
				taskToFork.fork();
				sizeEstimate = rs.EstimateSize();
			}
			task.LocalResult = result;
			task.tryComplete();
		}


		/// <summary>
		/// Declares that a globally valid result has been found.  If another task has
		/// not already found the answer, the result is installed in
		/// {@code sharedResult}.  The {@code compute()} method will check
		/// {@code sharedResult} before proceeding with computation, so this causes
		/// the computation to terminate early.
		/// </summary>
		/// <param name="result"> the result found </param>
		protected internal virtual void ShortCircuit(R result)
		{
			if (result != null)
			{
				SharedResult.CompareAndSet(null, result);
			}
		}

		/// <summary>
		/// Sets a local result for this task.  If this task is the root, set the
		/// shared result instead (if not already set).
		/// </summary>
		/// <param name="localResult"> The result to set for this task </param>
		protected internal override R LocalResult
		{
			set
			{
				if (Root)
				{
					if (value != null)
					{
						SharedResult.CompareAndSet(null, value);
					}
				}
				else
				{
					base.LocalResult = value;
				}
			}
			get
			{
				if (Root)
				{
					R answer = SharedResult.Get();
					return (answer == null) ? EmptyResult : answer;
				}
				else
				{
					return base.LocalResult;
				}
			}
		}

		/// <summary>
		/// Retrieves the local result for this task
		/// </summary>
		public override R RawResult
		{
			get
			{
				return LocalResult;
			}
		}


		/// <summary>
		/// Mark this task as canceled
		/// </summary>
		protected internal virtual void Cancel()
		{
			Canceled = true;
		}

		/// <summary>
		/// Queries whether this task is canceled.  A task is considered canceled if
		/// it or any of its parents have been canceled.
		/// </summary>
		/// <returns> {@code true} if this task or any parent is canceled. </returns>
		protected internal virtual bool TaskCanceled()
		{
			bool cancel = Canceled;
			if (!cancel)
			{
				for (K parent = Parent; !cancel && parent != null; parent = parent.Parent)
				{
					cancel = parent.canceled;
				}
			}

			return cancel;
		}

		/// <summary>
		/// Cancels all tasks which succeed this one in the encounter order.  This
		/// includes canceling all the current task's right sibling, as well as the
		/// later right siblings of all its parents.
		/// </summary>
		protected internal virtual void CancelLaterNodes()
		{
			// Go up the tree, cancel right siblings of this node and all parents
			for (@SuppressWarnings("unchecked") K parent = Parent, node = (K) this; parent != null; node = parent, parent = parent.Parent)
			{
				// If node is a left child of parent, then has a right sibling
				if (parent.leftChild == node)
				{
					K rightSibling = parent.rightChild;
					if (!rightSibling.canceled)
					{
						rightSibling.cancel();
					}
				}
			}
		}
	}

}