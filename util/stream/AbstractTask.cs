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
	/// Abstract base class for most fork-join tasks used to implement stream ops.
	/// Manages splitting logic, tracking of child tasks, and intermediate results.
	/// Each task is associated with a <seealso cref="Spliterator"/> that describes the portion
	/// of the input associated with the subtree rooted at this task.
	/// Tasks may be leaf nodes (which will traverse the elements of
	/// the {@code Spliterator}) or internal nodes (which split the
	/// {@code Spliterator} into multiple child tasks).
	/// 
	/// @implNote
	/// <para>This class is based on <seealso cref="CountedCompleter"/>, a form of fork-join task
	/// where each task has a semaphore-like count of uncompleted children, and the
	/// task is implicitly completed and notified when its last child completes.
	/// Internal node tasks will likely override the {@code onCompletion} method from
	/// {@code CountedCompleter} to merge the results from child tasks into the
	/// current task's result.
	/// 
	/// </para>
	/// <para>Splitting and setting up the child task links is done by {@code compute()}
	/// for internal nodes.  At {@code compute()} time for leaf nodes, it is
	/// guaranteed that the parent's child-related fields (including sibling links
	/// for the parent's children) will be set up for all children.
	/// 
	/// </para>
	/// <para>For example, a task that performs a reduce would override {@code doLeaf()}
	/// to perform a reduction on that leaf node's chunk using the
	/// {@code Spliterator}, and override {@code onCompletion()} to merge the results
	/// of the child tasks for internal nodes:
	/// 
	/// <pre>{@code
	///     protected S doLeaf() {
	///         spliterator.forEach(...);
	///         return localReductionResult;
	///     }
	/// 
	///     public void onCompletion(CountedCompleter caller) {
	///         if (!isLeaf()) {
	///             ReduceTask<P_IN, P_OUT, T, R> child = children;
	///             R result = child.getLocalResult();
	///             child = child.nextSibling;
	///             for (; child != null; child = child.nextSibling)
	///                 result = combine(result, child.getLocalResult());
	///             setLocalResult(result);
	///         }
	///     }
	/// }</pre>
	/// 
	/// </para>
	/// <para>Serialization is not supported as there is no intention to serialize
	/// tasks managed by stream ops.
	/// 
	/// </para>
	/// </summary>
	/// @param <P_IN> Type of elements input to the pipeline </param>
	/// @param <P_OUT> Type of elements output from the pipeline </param>
	/// @param <R> Type of intermediate result, which may be different from operation
	///        result type </param>
	/// @param <K> Type of parent, child and sibling tasks
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract class AbstractTask<P_IN, P_OUT, R, K extends AbstractTask<P_IN, P_OUT, R, K>> extends java.util.concurrent.CountedCompleter<R>
	internal abstract class AbstractTask<P_IN, P_OUT, R, K> : CountedCompleter<R> where K : AbstractTask<P_IN, P_OUT, R, K>
	{

		/// <summary>
		/// Default target factor of leaf tasks for parallel decomposition.
		/// To allow load balancing, we over-partition, currently to approximately
		/// four tasks per processor, which enables others to help out
		/// if leaf tasks are uneven or some processors are otherwise busy.
		/// </summary>
		internal static readonly int LEAF_TARGET = ForkJoinPool.CommonPoolParallelism << 2;

		/// <summary>
		/// The pipeline helper, common to all tasks in a computation </summary>
		protected internal readonly PipelineHelper<P_OUT> Helper;

		/// <summary>
		/// The spliterator for the portion of the input associated with the subtree
		/// rooted at this task
		/// </summary>
		protected internal Spliterator<P_IN> Spliterator;

		/// <summary>
		/// Target leaf size, common to all tasks in a computation </summary>
		protected internal long TargetSize; // may be laziliy initialized

		/// <summary>
		/// The left child.
		/// null if no children
		/// if non-null rightChild is non-null
		/// </summary>
		protected internal K LeftChild;

		/// <summary>
		/// The right child.
		/// null if no children
		/// if non-null leftChild is non-null
		/// </summary>
		protected internal K RightChild;

		/// <summary>
		/// The result of this node, if completed </summary>
		private R LocalResult_Renamed;

		/// <summary>
		/// Constructor for root nodes.
		/// </summary>
		/// <param name="helper"> The {@code PipelineHelper} describing the stream pipeline
		///               up to this operation </param>
		/// <param name="spliterator"> The {@code Spliterator} describing the source for this
		///                    pipeline </param>
		protected internal AbstractTask(PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator) : base(null)
		{
			this.Helper = helper;
			this.Spliterator = spliterator;
			this.TargetSize = 0L;
		}

		/// <summary>
		/// Constructor for non-root nodes.
		/// </summary>
		/// <param name="parent"> this node's parent task </param>
		/// <param name="spliterator"> {@code Spliterator} describing the subtree rooted at
		///        this node, obtained by splitting the parent {@code Spliterator} </param>
		protected internal AbstractTask(K parent, Spliterator<P_IN> spliterator) : base(parent)
		{
			this.Spliterator = spliterator;
			this.Helper = parent.helper;
			this.TargetSize = parent.targetSize;
		}

		/// <summary>
		/// Constructs a new node of type T whose parent is the receiver; must call
		/// the AbstractTask(T, Spliterator) constructor with the receiver and the
		/// provided Spliterator.
		/// </summary>
		/// <param name="spliterator"> {@code Spliterator} describing the subtree rooted at
		///        this node, obtained by splitting the parent {@code Spliterator} </param>
		/// <returns> newly constructed child node </returns>
		protected internal abstract K MakeChild(Spliterator<P_IN> spliterator);

		/// <summary>
		/// Computes the result associated with a leaf node.  Will be called by
		/// {@code compute()} and the result passed to @{code setLocalResult()}
		/// </summary>
		/// <returns> the computed result of a leaf node </returns>
		protected internal abstract R DoLeaf();

		/// <summary>
		/// Returns a suggested target leaf size based on the initial size estimate.
		/// </summary>
		/// <returns> suggested target leaf size </returns>
		public static long SuggestTargetSize(long sizeEstimate)
		{
			long est = sizeEstimate / LEAF_TARGET;
			return est > 0L ? est : 1L;
		}

		/// <summary>
		/// Returns the targetSize, initializing it via the supplied
		/// size estimate if not already initialized.
		/// </summary>
		protected internal long GetTargetSize(long sizeEstimate)
		{
			long s;
			return ((s = TargetSize) != 0 ? s : (TargetSize = SuggestTargetSize(sizeEstimate)));
		}

		/// <summary>
		/// Returns the local result, if any. Subclasses should use
		/// <seealso cref="#setLocalResult(Object)"/> and <seealso cref="#getLocalResult()"/> to manage
		/// results.  This returns the local result so that calls from within the
		/// fork-join framework will return the correct result.
		/// </summary>
		/// <returns> local result for this node previously stored with
		/// <seealso cref="#setLocalResult"/> </returns>
		public override R RawResult
		{
			get
			{
				return LocalResult_Renamed;
			}
			set
			{
				if (value != null)
				{
					throw new IllegalStateException();
				}
			}
		}


		/// <summary>
		/// Retrieves a result previously stored with <seealso cref="#setLocalResult"/>
		/// </summary>
		/// <returns> local result for this node previously stored with
		/// <seealso cref="#setLocalResult"/> </returns>
		protected internal virtual R LocalResult
		{
			get
			{
				return LocalResult_Renamed;
			}
			set
			{
				this.LocalResult_Renamed = value;
			}
		}


		/// <summary>
		/// Indicates whether this task is a leaf node.  (Only valid after
		/// <seealso cref="#compute"/> has been called on this node).  If the node is not a
		/// leaf node, then children will be non-null and numChildren will be
		/// positive.
		/// </summary>
		/// <returns> {@code true} if this task is a leaf node </returns>
		protected internal virtual bool Leaf
		{
			get
			{
				return LeftChild == null;
			}
		}

		/// <summary>
		/// Indicates whether this task is the root node
		/// </summary>
		/// <returns> {@code true} if this task is the root node. </returns>
		protected internal virtual bool Root
		{
			get
			{
				return Parent == null;
			}
		}

		/// <summary>
		/// Returns the parent of this task, or null if this task is the root
		/// </summary>
		/// <returns> the parent of this task, or null if this task is the root </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected K getParent()
		protected internal virtual K Parent
		{
			get
			{
				return (K) Completer;
			}
		}

		/// <summary>
		/// Decides whether or not to split a task further or compute it
		/// directly. If computing directly, calls {@code doLeaf} and pass
		/// the result to {@code setRawResult}. Otherwise splits off
		/// subtasks, forking one and continuing as the other.
		/// 
		/// <para> The method is structured to conserve resources across a
		/// range of uses.  The loop continues with one of the child tasks
		/// when split, to avoid deep recursion. To cope with spliterators
		/// that may be systematically biased toward left-heavy or
		/// right-heavy splits, we alternate which child is forked versus
		/// continued in the loop.
		/// </para>
		/// </summary>
		public override void Compute()
		{
			Spliterator<P_IN> rs = Spliterator, ls ; // right, left spliterators
			long sizeEstimate = rs.EstimateSize();
			long sizeThreshold = GetTargetSize(sizeEstimate);
			bool forkRight = false;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K task = (K) this;
			K task = (K) this;
			while (sizeEstimate > sizeThreshold && (ls = rs.TrySplit()) != null)
			{
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
			task.LocalResult = task.doLeaf();
			task.tryComplete();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implNote
		/// Clears spliterator and children fields.  Overriders MUST call
		/// {@code super.onCompletion} as the last thing they do if they want these
		/// cleared.
		/// </summary>
		public override void onCompletion<T1>(CountedCompleter<T1> caller)
		{
			Spliterator = null;
			LeftChild = RightChild = null;
		}

		/// <summary>
		/// Returns whether this node is a "leftmost" node -- whether the path from
		/// the root to this node involves only traversing leftmost child links.  For
		/// a leaf node, this means it is the first leaf node in the encounter order.
		/// </summary>
		/// <returns> {@code true} if this node is a "leftmost" node </returns>
		protected internal virtual bool LeftmostNode
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") K node = (K) this;
				K node = (K) this;
				while (node != null)
				{
					K parent = node.Parent;
					if (parent != null && parent.leftChild != node)
					{
						return false;
					}
					node = parent;
				}
				return true;
			}
		}
	}

}