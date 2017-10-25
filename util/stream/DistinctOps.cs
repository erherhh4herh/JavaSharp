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
	/// Factory methods for transforming streams into duplicate-free streams, using
	/// <seealso cref="Object#equals(Object)"/> to determine equality.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class DistinctOps
	{

		private DistinctOps()
		{
		}

		/// <summary>
		/// Appends a "distinct" operation to the provided stream, and returns the
		/// new stream.
		/// </summary>
		/// @param <T> the type of both input and output elements </param>
		/// <param name="upstream"> a reference stream with element type T </param>
		/// <returns> the new stream </returns>
		internal static ReferencePipeline<T, T> makeRef<T, T1>(AbstractPipeline<T1> upstream)
		{
			return new StatefulOpAnonymousInnerClassHelper(upstream, StreamOpFlag.IS_DISTINCT | StreamOpFlag.NOT_SIZED);
		}

		private class StatefulOpAnonymousInnerClassHelper : ReferencePipeline.StatefulOp<T, T>
		{
			public StatefulOpAnonymousInnerClassHelper(java.util.stream.AbstractPipeline<T1> upstream, int NOT_SIZED) : base(upstream, StreamShape.REFERENCE, NOT_SIZED)
			{
			}


			internal virtual Node<T> reduce<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				// If the stream is SORTED then it should also be ORDERED so the following will also
				// preserve the sort order
				TerminalOp<T, LinkedHashSet<T>> reduceOp = ReduceOps.MakeRef<T, LinkedHashSet<T>>(LinkedHashSet::new, LinkedHashSet::add, LinkedHashSet::addAll);
				return Nodes.Node(reduceOp.evaluateParallel(helper, spliterator));
			}

			internal override Node<T> opEvaluateParallel<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator, IntFunction<T[]> generator)
			{
				if (StreamOpFlag.DISTINCT.isKnown(helper.StreamAndOpFlags))
				{
					// No-op
					return helper.Evaluate(spliterator, false, generator);
				}
				else if (StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					return reduce(helper, spliterator);
				}
				else
				{
					// Holder of null state since ConcurrentHashMap does not support null values
					AtomicBoolean seenNull = new AtomicBoolean(false);
					ConcurrentDictionary<T, Boolean> map = new ConcurrentDictionary<T, Boolean>();
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
					TerminalOp<T, Void> forEachOp = ForEachOps.makeRef(Stream_Fields.t =>
					{
						if (Stream_Fields.t == null)
						{
							seenNull.Set(Stream_Fields.True);
						}
						else
						{
							map.GetOrAdd(Stream_Fields.t, true);
						}
					}, false);
					forEachOp.evaluateParallel(helper, spliterator);

					// If null has been seen then copy the key set into a HashSet that supports null values
					// and add null
					ConcurrentDictionary<T, Boolean>.KeyCollection keys = map.Keys;
					if (seenNull.Get())
					{
						// TODO Implement a more efficient set-union view, rather than copying
						keys = new HashSet<>(keys);
						keys.add(null);
					}
					return Nodes.Node(keys);
				}
			}

			internal override Spliterator<T> opEvaluateParallelLazy<P_IN>(PipelineHelper<T> helper, Spliterator<P_IN> spliterator)
			{
				if (StreamOpFlag.DISTINCT.isKnown(helper.StreamAndOpFlags))
				{
					// No-op
					return helper.WrapSpliterator(spliterator);
				}
				else if (StreamOpFlag.ORDERED.isKnown(helper.StreamAndOpFlags))
				{
					// Not lazy, barrier required to preserve order
					return reduce(helper, spliterator).spliterator();
				}
				else
				{
					// Lazy
					return new StreamSpliterators.DistinctSpliterator<>(helper.WrapSpliterator(spliterator));
				}
			}

			internal override Sink<T> OpWrapSink(int flags, Sink<T> sink)
			{
				Objects.RequireNonNull(sink);

				if (StreamOpFlag.DISTINCT.isKnown(flags))
				{
					return sink;
				}
				else if (StreamOpFlag.SORTED.isKnown(flags))
				{
					return new Sink_ChainedReferenceAnonymousInnerClassHelper(this, sink);
				}
				else
				{
					return new Sink_ChainedReferenceAnonymousInnerClassHelper2(this, sink);
				}
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper : Sink_ChainedReference<T, T>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<T> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				internal bool seenNull;
				internal T lastSeen;

				public override void Begin(long size)
				{
					seenNull = Sink_Fields.False;
					lastSeen = null;
					downstream.begin(-1);
				}

				public override void End()
				{
					seenNull = Sink_Fields.False;
					lastSeen = null;
					downstream.end();
				}

				public override void Accept(T Stream_Fields)
				{
					if (Stream_Fields.t == null)
					{
						if (!seenNull)
						{
							seenNull = Stream_Fields.True;
							downstream.accept(lastSeen = null);
						}
					}
					else if (lastSeen == null || !Stream_Fields.t.Equals(lastSeen))
					{
						downstream.accept(lastSeen = Stream_Fields.t);
					}
				}
			}

			private class Sink_ChainedReferenceAnonymousInnerClassHelper2 : Sink_ChainedReference<T, T>
			{
				private readonly StatefulOpAnonymousInnerClassHelper OuterInstance;

				public Sink_ChainedReferenceAnonymousInnerClassHelper2(StatefulOpAnonymousInnerClassHelper outerInstance, java.util.stream.Sink<T> sink) : base(sink)
				{
					this.outerInstance = outerInstance;
				}

				internal Set<T> seen;

				public override void Begin(long size)
				{
					seen = new HashSet<>();
					downstream.begin(-1);
				}

				public override void End()
				{
					seen = null;
					downstream.end();
				}

				public override void Accept(T Stream_Fields)
				{
					if (!seen.contains(Stream_Fields.t))
					{
						seen.add(Stream_Fields.t);
						downstream.accept(Stream_Fields.t);
					}
				}
			}
		}
	}

}