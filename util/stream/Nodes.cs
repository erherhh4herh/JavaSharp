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
	/// Factory methods for constructing implementations of <seealso cref="Node"/> and
	/// <seealso cref="Node.Builder"/> and their primitive specializations.  Fork/Join tasks
	/// for collecting output from a <seealso cref="PipelineHelper"/> to a <seealso cref="Node"/> and
	/// flattening <seealso cref="Node"/>s.
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class Nodes
	{

		private Nodes()
		{
			throw new Error("no instances");
		}

		/// <summary>
		/// The maximum size of an array that can be allocated.
		/// </summary>
		internal static readonly long MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		// IllegalArgumentException messages
		internal const String BAD_SIZE = "Stream size exceeds max array size";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private static final Node EMPTY_NODE = new EmptyNode.OfRef();
		private static readonly Node EMPTY_NODE = new EmptyNode.OfRef();
		private static readonly Node_OfInt EMPTY_INT_NODE = new EmptyNode.OfInt();
		private static readonly Node_OfLong EMPTY_LONG_NODE = new EmptyNode.OfLong();
		private static readonly Node_OfDouble EMPTY_DOUBLE_NODE = new EmptyNode.OfDouble();

		// General shape-based node creation methods

		/// <summary>
		/// Produces an empty node whose count is zero, has no children and no content.
		/// </summary>
		/// @param <T> the type of elements of the created node </param>
		/// <param name="shape"> the shape of the node to be created </param>
		/// <returns> an empty node. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <T> Node<T> emptyNode(StreamShape shape)
		internal static Node<T> emptyNode<T>(StreamShape shape)
		{
			switch (shape)
			{
				case java.util.stream.StreamShape.REFERENCE:
					return (Node<T>) EMPTY_NODE;
				case java.util.stream.StreamShape.INT_VALUE:
					return (Node<T>) EMPTY_INT_NODE;
				case java.util.stream.StreamShape.LONG_VALUE:
					return (Node<T>) EMPTY_LONG_NODE;
				case java.util.stream.StreamShape.DOUBLE_VALUE:
					return (Node<T>) EMPTY_DOUBLE_NODE;
				default:
					throw new IllegalStateException("Unknown shape " + shape);
			}
		}

		/// <summary>
		/// Produces a concatenated <seealso cref="Node"/> that has two or more children.
		/// <para>The count of the concatenated node is equal to the sum of the count
		/// of each child. Traversal of the concatenated node traverses the content
		/// of each child in encounter order of the list of children. Splitting a
		/// spliterator obtained from the concatenated node preserves the encounter
		/// order of the list of children.
		/// 
		/// </para>
		/// <para>The result may be a concatenated node, the input sole node if the size
		/// of the list is 1, or an empty node.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of elements of the concatenated node </param>
		/// <param name="shape"> the shape of the concatenated node to be created </param>
		/// <param name="left"> the left input node </param>
		/// <param name="right"> the right input node </param>
		/// <returns> a {@code Node} covering the elements of the input nodes </returns>
		/// <exception cref="IllegalStateException"> if all <seealso cref="Node"/> elements of the list
		/// are an not instance of type supported by this factory. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <T> Node<T> conc(StreamShape shape, Node<T> left, Node<T> right)
		internal static Node<T> conc<T>(StreamShape shape, Node<T> left, Node<T> right)
		{
			switch (shape)
			{
				case java.util.stream.StreamShape.REFERENCE:
					return new ConcNode<>(left, right);
				case java.util.stream.StreamShape.INT_VALUE:
					return (Node<T>) new ConcNode.OfInt((Node_OfInt) left, (Node_OfInt) right);
				case java.util.stream.StreamShape.LONG_VALUE:
					return (Node<T>) new ConcNode.OfLong((Node_OfLong) left, (Node_OfLong) right);
				case java.util.stream.StreamShape.DOUBLE_VALUE:
					return (Node<T>) new ConcNode.OfDouble((Node_OfDouble) left, (Node_OfDouble) right);
				default:
					throw new IllegalStateException("Unknown shape " + shape);
			}
		}

		// Reference-based node methods

		/// <summary>
		/// Produces a <seealso cref="Node"/> describing an array.
		/// 
		/// <para>The node will hold a reference to the array and will not make a copy.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of elements held by the node </param>
		/// <param name="array"> the array </param>
		/// <returns> a node holding an array </returns>
		internal static Node<T> node<T>(T[] array)
		{
			return new ArrayNode<>(array);
		}

		/// <summary>
		/// Produces a <seealso cref="Node"/> describing a <seealso cref="Collection"/>.
		/// <para>
		/// The node will hold a reference to the collection and will not make a copy.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of elements held by the node </param>
		/// <param name="c"> the collection </param>
		/// <returns> a node holding a collection </returns>
		internal static Node<T> node<T>(ICollection<T> c)
		{
			return new CollectionNode<>(c);
		}

		/// <summary>
		/// Produces a <seealso cref="Node.Builder"/>.
		/// </summary>
		/// <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		/// otherwise the exact capacity desired.  A fixed capacity builder will
		/// fail if the wrong number of elements are added to the builder. </param>
		/// <param name="generator"> the array factory </param>
		/// @param <T> the type of elements of the node builder </param>
		/// <returns> a {@code Node.Builder} </returns>
		internal static Node_Builder<T> builder<T>(long exactSizeIfKnown, IntFunction<T[]> generator)
		{
			return (exactSizeIfKnown >= 0 && exactSizeIfKnown < MAX_ARRAY_SIZE) ? new FixedNodeBuilder<>(exactSizeIfKnown, generator) : Builder();
		}

		/// <summary>
		/// Produces a variable size @{link Node.Builder}.
		/// </summary>
		/// @param <T> the type of elements of the node builder </param>
		/// <returns> a {@code Node.Builder} </returns>
		internal static Node_Builder<T> builder<T>()
		{
			return new SpinedNodeBuilder<>();
		}

		// Int nodes

		/// <summary>
		/// Produces a <seealso cref="Node.OfInt"/> describing an int[] array.
		/// 
		/// <para>The node will hold a reference to the array and will not make a copy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array </param>
		/// <returns> a node holding an array </returns>
		internal static Node_OfInt Node(int[] array)
		{
			return new IntArrayNode(array);
		}

		/// <summary>
		/// Produces a <seealso cref="Node.Builder.OfInt"/>.
		/// </summary>
		/// <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		/// otherwise the exact capacity desired.  A fixed capacity builder will
		/// fail if the wrong number of elements are added to the builder. </param>
		/// <returns> a {@code Node.Builder.OfInt} </returns>
		internal static Node_Builder_OfInt IntBuilder(long exactSizeIfKnown)
		{
			return (exactSizeIfKnown >= 0 && exactSizeIfKnown < MAX_ARRAY_SIZE) ? new IntFixedNodeBuilder(exactSizeIfKnown) : IntBuilder();
		}

		/// <summary>
		/// Produces a variable size @{link Node.Builder.OfInt}.
		/// </summary>
		/// <returns> a {@code Node.Builder.OfInt} </returns>
		internal static Node_Builder_OfInt IntBuilder()
		{
			return new IntSpinedNodeBuilder();
		}

		// Long nodes

		/// <summary>
		/// Produces a <seealso cref="Node.OfLong"/> describing a long[] array.
		/// <para>
		/// The node will hold a reference to the array and will not make a copy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array </param>
		/// <returns> a node holding an array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static Node_OfLong node(final long[] array)
		internal static Node_OfLong Node(long[] array)
		{
			return new LongArrayNode(array);
		}

		/// <summary>
		/// Produces a <seealso cref="Node.Builder.OfLong"/>.
		/// </summary>
		/// <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		/// otherwise the exact capacity desired.  A fixed capacity builder will
		/// fail if the wrong number of elements are added to the builder. </param>
		/// <returns> a {@code Node.Builder.OfLong} </returns>
		internal static Node_Builder_OfLong LongBuilder(long exactSizeIfKnown)
		{
			return (exactSizeIfKnown >= 0 && exactSizeIfKnown < MAX_ARRAY_SIZE) ? new LongFixedNodeBuilder(exactSizeIfKnown) : LongBuilder();
		}

		/// <summary>
		/// Produces a variable size @{link Node.Builder.OfLong}.
		/// </summary>
		/// <returns> a {@code Node.Builder.OfLong} </returns>
		internal static Node_Builder_OfLong LongBuilder()
		{
			return new LongSpinedNodeBuilder();
		}

		// Double nodes

		/// <summary>
		/// Produces a <seealso cref="Node.OfDouble"/> describing a double[] array.
		/// 
		/// <para>The node will hold a reference to the array and will not make a copy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="array"> the array </param>
		/// <returns> a node holding an array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static Node_OfDouble node(final double[] array)
		internal static Node_OfDouble Node(double[] array)
		{
			return new DoubleArrayNode(array);
		}

		/// <summary>
		/// Produces a <seealso cref="Node.Builder.OfDouble"/>.
		/// </summary>
		/// <param name="exactSizeIfKnown"> -1 if a variable size builder is requested,
		/// otherwise the exact capacity desired.  A fixed capacity builder will
		/// fail if the wrong number of elements are added to the builder. </param>
		/// <returns> a {@code Node.Builder.OfDouble} </returns>
		internal static Node_Builder_OfDouble DoubleBuilder(long exactSizeIfKnown)
		{
			return (exactSizeIfKnown >= 0 && exactSizeIfKnown < MAX_ARRAY_SIZE) ? new DoubleFixedNodeBuilder(exactSizeIfKnown) : DoubleBuilder();
		}

		/// <summary>
		/// Produces a variable size @{link Node.Builder.OfDouble}.
		/// </summary>
		/// <returns> a {@code Node.Builder.OfDouble} </returns>
		internal static Node_Builder_OfDouble DoubleBuilder()
		{
			return new DoubleSpinedNodeBuilder();
		}

		// Parallel evaluation of pipelines to nodes

		/// <summary>
		/// Collect, in parallel, elements output from a pipeline and describe those
		/// elements with a <seealso cref="Node"/>.
		/// 
		/// @implSpec
		/// If the exact size of the output from the pipeline is known and the source
		/// <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		/// then a flat <seealso cref="Node"/> will be returned whose content is an array,
		/// since the size is known the array can be constructed in advance and
		/// output elements can be placed into the array concurrently by leaf
		/// tasks at the correct offsets.  If the exact size is not known, output
		/// elements are collected into a conc-node whose shape mirrors that
		/// of the computation. This conc-node can then be flattened in
		/// parallel to produce a flat {@code Node} if desired.
		/// </summary>
		/// <param name="helper"> the pipeline helper describing the pipeline </param>
		/// <param name="flattenTree"> whether a conc node should be flattened into a node
		///                    describing an array before returning </param>
		/// <param name="generator"> the array generator </param>
		/// <returns> a <seealso cref="Node"/> describing the output elements </returns>
		public static Node<P_OUT> collect<P_IN, P_OUT>(PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator, bool flattenTree, IntFunction<P_OUT[]> generator)
		{
			long size = helper.ExactOutputSizeIfKnown(spliterator);
			if (size >= 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				P_OUT[] array = generator.Apply((int) size);
				(new SizedCollectorTask.OfRef<>(spliterator, helper, array)).invoke();
				return Node(array);
			}
			else
			{
				Node<P_OUT> node = (new CollectorTask.OfRef<P_OUT>(helper, generator, spliterator)).invoke();
				return flattenTree ? Flatten(node, generator) : node;
			}
		}

		/// <summary>
		/// Collect, in parallel, elements output from an int-valued pipeline and
		/// describe those elements with a <seealso cref="Node.OfInt"/>.
		/// 
		/// @implSpec
		/// If the exact size of the output from the pipeline is known and the source
		/// <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		/// then a flat <seealso cref="Node"/> will be returned whose content is an array,
		/// since the size is known the array can be constructed in advance and
		/// output elements can be placed into the array concurrently by leaf
		/// tasks at the correct offsets.  If the exact size is not known, output
		/// elements are collected into a conc-node whose shape mirrors that
		/// of the computation. This conc-node can then be flattened in
		/// parallel to produce a flat {@code Node.OfInt} if desired.
		/// </summary>
		/// @param <P_IN> the type of elements from the source Spliterator </param>
		/// <param name="helper"> the pipeline helper describing the pipeline </param>
		/// <param name="flattenTree"> whether a conc node should be flattened into a node
		///                    describing an array before returning </param>
		/// <returns> a <seealso cref="Node.OfInt"/> describing the output elements </returns>
		public static Node_OfInt collectInt<P_IN>(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator, bool flattenTree)
		{
			long size = helper.ExactOutputSizeIfKnown(spliterator);
			if (size >= 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				int[] array = new int[(int) size];
				(new SizedCollectorTask.OfInt<>(spliterator, helper, array)).invoke();
				return Node(array);
			}
			else
			{
				Node_OfInt node = (new CollectorTask.OfInt<>(helper, spliterator)).invoke();
				return flattenTree ? FlattenInt(node) : node;
			}
		}

		/// <summary>
		/// Collect, in parallel, elements output from a long-valued pipeline and
		/// describe those elements with a <seealso cref="Node.OfLong"/>.
		/// 
		/// @implSpec
		/// If the exact size of the output from the pipeline is known and the source
		/// <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		/// then a flat <seealso cref="Node"/> will be returned whose content is an array,
		/// since the size is known the array can be constructed in advance and
		/// output elements can be placed into the array concurrently by leaf
		/// tasks at the correct offsets.  If the exact size is not known, output
		/// elements are collected into a conc-node whose shape mirrors that
		/// of the computation. This conc-node can then be flattened in
		/// parallel to produce a flat {@code Node.OfLong} if desired.
		/// </summary>
		/// @param <P_IN> the type of elements from the source Spliterator </param>
		/// <param name="helper"> the pipeline helper describing the pipeline </param>
		/// <param name="flattenTree"> whether a conc node should be flattened into a node
		///                    describing an array before returning </param>
		/// <returns> a <seealso cref="Node.OfLong"/> describing the output elements </returns>
		public static Node_OfLong collectLong<P_IN>(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator, bool flattenTree)
		{
			long size = helper.ExactOutputSizeIfKnown(spliterator);
			if (size >= 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				long[] array = new long[(int) size];
				(new SizedCollectorTask.OfLong<>(spliterator, helper, array)).invoke();
				return Node(array);
			}
			else
			{
				Node_OfLong node = (new CollectorTask.OfLong<>(helper, spliterator)).invoke();
				return flattenTree ? FlattenLong(node) : node;
			}
		}

		/// <summary>
		/// Collect, in parallel, elements output from n double-valued pipeline and
		/// describe those elements with a <seealso cref="Node.OfDouble"/>.
		/// 
		/// @implSpec
		/// If the exact size of the output from the pipeline is known and the source
		/// <seealso cref="Spliterator"/> has the <seealso cref="Spliterator#SUBSIZED"/> characteristic,
		/// then a flat <seealso cref="Node"/> will be returned whose content is an array,
		/// since the size is known the array can be constructed in advance and
		/// output elements can be placed into the array concurrently by leaf
		/// tasks at the correct offsets.  If the exact size is not known, output
		/// elements are collected into a conc-node whose shape mirrors that
		/// of the computation. This conc-node can then be flattened in
		/// parallel to produce a flat {@code Node.OfDouble} if desired.
		/// </summary>
		/// @param <P_IN> the type of elements from the source Spliterator </param>
		/// <param name="helper"> the pipeline helper describing the pipeline </param>
		/// <param name="flattenTree"> whether a conc node should be flattened into a node
		///                    describing an array before returning </param>
		/// <returns> a <seealso cref="Node.OfDouble"/> describing the output elements </returns>
		public static Node_OfDouble collectDouble<P_IN>(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator, bool flattenTree)
		{
			long size = helper.ExactOutputSizeIfKnown(spliterator);
			if (size >= 0 && spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED))
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				double[] array = new double[(int) size];
				(new SizedCollectorTask.OfDouble<>(spliterator, helper, array)).invoke();
				return Node(array);
			}
			else
			{
				Node_OfDouble node = (new CollectorTask.OfDouble<>(helper, spliterator)).invoke();
				return flattenTree ? FlattenDouble(node) : node;
			}
		}

		// Parallel flattening of nodes

		/// <summary>
		/// Flatten, in parallel, a <seealso cref="Node"/>.  A flattened node is one that has
		/// no children.  If the node is already flat, it is simply returned.
		/// 
		/// @implSpec
		/// If a new node is to be created, the generator is used to create an array
		/// whose length is <seealso cref="Node#count()"/>.  Then the node tree is traversed
		/// and leaf node elements are placed in the array concurrently by leaf tasks
		/// at the correct offsets.
		/// </summary>
		/// @param <T> type of elements contained by the node </param>
		/// <param name="node"> the node to flatten </param>
		/// <param name="generator"> the array factory used to create array instances </param>
		/// <returns> a flat {@code Node} </returns>
		public static Node<T> flatten<T>(Node<T> node, IntFunction<T[]> generator)
		{
			if (node.ChildCount > 0)
			{
				long size = node.Count();
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				T[] array = generator.Apply((int) size);
				(new ToArrayTask.OfRef<>(node, array, 0)).invoke();
				return Node(array);
			}
			else
			{
				return node;
			}
		}

		/// <summary>
		/// Flatten, in parallel, a <seealso cref="Node.OfInt"/>.  A flattened node is one that
		/// has no children.  If the node is already flat, it is simply returned.
		/// 
		/// @implSpec
		/// If a new node is to be created, a new int[] array is created whose length
		/// is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		/// elements are placed in the array concurrently by leaf tasks at the
		/// correct offsets.
		/// </summary>
		/// <param name="node"> the node to flatten </param>
		/// <returns> a flat {@code Node.OfInt} </returns>
		public static Node_OfInt FlattenInt(Node_OfInt node)
		{
			if (node.ChildCount > 0)
			{
				long size = node.count();
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				int[] array = new int[(int) size];
				(new ToArrayTask.OfInt(node, array, 0)).invoke();
				return Node(array);
			}
			else
			{
				return node;
			}
		}

		/// <summary>
		/// Flatten, in parallel, a <seealso cref="Node.OfLong"/>.  A flattened node is one that
		/// has no children.  If the node is already flat, it is simply returned.
		/// 
		/// @implSpec
		/// If a new node is to be created, a new long[] array is created whose length
		/// is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		/// elements are placed in the array concurrently by leaf tasks at the
		/// correct offsets.
		/// </summary>
		/// <param name="node"> the node to flatten </param>
		/// <returns> a flat {@code Node.OfLong} </returns>
		public static Node_OfLong FlattenLong(Node_OfLong node)
		{
			if (node.ChildCount > 0)
			{
				long size = node.count();
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				long[] array = new long[(int) size];
				(new ToArrayTask.OfLong(node, array, 0)).invoke();
				return Node(array);
			}
			else
			{
				return node;
			}
		}

		/// <summary>
		/// Flatten, in parallel, a <seealso cref="Node.OfDouble"/>.  A flattened node is one that
		/// has no children.  If the node is already flat, it is simply returned.
		/// 
		/// @implSpec
		/// If a new node is to be created, a new double[] array is created whose length
		/// is <seealso cref="Node#count()"/>.  Then the node tree is traversed and leaf node
		/// elements are placed in the array concurrently by leaf tasks at the
		/// correct offsets.
		/// </summary>
		/// <param name="node"> the node to flatten </param>
		/// <returns> a flat {@code Node.OfDouble} </returns>
		public static Node_OfDouble FlattenDouble(Node_OfDouble node)
		{
			if (node.ChildCount > 0)
			{
				long size = node.count();
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				double[] array = new double[(int) size];
				(new ToArrayTask.OfDouble(node, array, 0)).invoke();
				return Node(array);
			}
			else
			{
				return node;
			}
		}

		// Implementations

		private abstract class EmptyNode<T, T_ARR, T_CONS> : Node<T>
		{
			internal EmptyNode()
			{
			}

			public override T[] AsArray(IntFunction<T[]> generator)
			{
				return generator.Apply(Node_Fields.0);
			}

			public virtual void CopyInto(T_ARR array, int offset)
			{
			}

			public override long Count()
			{
				return Node_Fields.0;
			}

			public virtual void ForEach(T_CONS consumer)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private static class OfRef<T> extends EmptyNode<T, T[] , java.util.function.Consumer<? base T>>
			private class OfRef<T> : EmptyNode<T, T[], Consumer<JavaToDotNetGenericWildcard>>
			{
				internal OfRef() : base()
				{
				}

				public override Spliterator<T> Node_Fields.spliterator()
				{
					return Spliterators.EmptySpliterator();
				}
			}

			private sealed class OfInt : EmptyNode<Integer, int[], IntConsumer>, Node_OfInt
			{

				internal OfInt() // Avoid creation of special accessor
				{
				}

				public java.util.Spliterator_OfInt Node_OfInt_Fields.Node_Fields.spliterator()
				{
					return Spliterators.EmptyIntSpliterator();
				}

				public Node_OfInt_Fields.int[] AsPrimitiveArray()
				{
					return EMPTY_INT_ARRAY;
				}
			}

			private sealed class OfLong : EmptyNode<Long, long[], LongConsumer>, Node_OfLong
			{

				internal OfLong() // Avoid creation of special accessor
				{
				}

				public java.util.Spliterator_OfLong Node_OfLong_Fields.Node_Fields.spliterator()
				{
					return Spliterators.EmptyLongSpliterator();
				}

				public Node_OfLong_Fields.long[] AsPrimitiveArray()
				{
					return EMPTY_LONG_ARRAY;
				}
			}

			private sealed class OfDouble : EmptyNode<Double, double[], DoubleConsumer>, Node_OfDouble
			{

				internal OfDouble() // Avoid creation of special accessor
				{
				}

				public java.util.Spliterator_OfDouble Node_OfDouble_Fields.Node_Fields.spliterator()
				{
					return Spliterators.EmptyDoubleSpliterator();
				}

				public Node_OfDouble_Fields.double[] AsPrimitiveArray()
				{
					return EMPTY_DOUBLE_ARRAY;
				}
			}
		}

		/// <summary>
		/// Node class for a reference array </summary>
		private class ArrayNode<T> : Node<T>
		{
			internal readonly T[] Array;
			internal int CurSize;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ArrayNode(long Node_Fields.size, java.util.function.IntFunction<T[]> generator)
			internal ArrayNode(long Node_Fields, IntFunction<T[]> generator)
			{
				if (Node_Fields.Size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				Node_Fields.this.array = generator.Apply((int) Node_Fields.Size);
				Node_Fields.this.curSize = Node_Fields.0;
			}

			internal ArrayNode(T[] array)
			{
				Node_Fields.this.array = array;
				Node_Fields.this.curSize = array.Length;
			}

			// Node

			public override Spliterator<T> Node_Fields.spliterator()
			{
				return Arrays.Spliterator(Array, Node_Fields.0, CurSize);
			}

			public override void CopyInto(T[] dest, int destOffset)
			{
				System.Array.Copy(Array, Node_Fields.0, dest, destOffset, CurSize);
			}

			public override T[] AsArray(IntFunction<T[]> generator)
			{
				if (Array.Length == CurSize)
				{
					return Array;
				}
				else
				{
					throw new IllegalStateException();
				}
			}

			public override long Count()
			{
				return CurSize;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base T> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				for (int i = Node_Fields.0; i < CurSize; i++)
				{
					consumer.Accept(Array[i]);
				}
			}

			//

			public override String ToString()
			{
				return string.Format("ArrayNode[{0:D}][{1}]", Array.Length - CurSize, Arrays.ToString(Array));
			}
		}

		/// <summary>
		/// Node class for a Collection </summary>
		private sealed class CollectionNode<T> : Node<T>
		{
			internal readonly ICollection<T> c;

			internal CollectionNode(ICollection<T> c)
			{
				Node_Fields.this.c = c;
			}

			// Node

			public override Spliterator<T> Node_Fields.spliterator()
			{
				return c.stream().spliterator();
			}

			public override void CopyInto(T[] array, int offset)
			{
				foreach (T t in c)
				{
					array[offset++] = t;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public T[] asArray(java.util.function.IntFunction<T[]> generator)
			public override T[] AsArray(IntFunction<T[]> generator)
			{
				return c.toArray(generator.Apply(c.Count));
			}

			public override long Count()
			{
				return c.Count;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base T> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				c.forEach(consumer);
			}

			//

			public override String ToString()
			{
				return string.Format("CollectionNode[{0:D}][{1}]", c.Count, c);
			}
		}

		/// <summary>
		/// Node class for an internal node with two or more children
		/// </summary>
		private abstract class AbstractConcNode<T, T_NODE> : Node<T> where T_NODE : Node<T>
		{
			protected internal readonly T_NODE Left;
			protected internal readonly T_NODE Right;
			internal readonly long Node_Fields;

			internal AbstractConcNode(T_NODE left, T_NODE right)
			{
				Node_Fields.this.left = left;
				Node_Fields.this.right = right;
				// The Node count will be required when the Node spliterator is
				// obtained and it is cheaper to aggressively calculate bottom up
				// as the tree is built rather than later on from the top down
				// traversing the tree
				Node_Fields.this.size = left.count() + right.count();
			}

			public override int ChildCount
			{
				get
				{
					return 2;
				}
			}

			public override T_NODE GetChild(int i)
			{
				if (i == Node_Fields.0)
				{
					return Left;
				}
				if (i == 1)
				{
					return Right;
				}
				throw new IndexOutOfBoundsException();
			}

			public override long Count()
			{
				return Node_Fields.Size;
			}
		}

		internal sealed class ConcNode<T> : AbstractConcNode<T, Node<T>>, Node<T>
		{

			internal ConcNode(Node<T> left, Node<T> right) : base(left, right)
			{
			}

			public override Spliterator<T> Node_Fields.spliterator()
			{
				return new Nodes.InternalNodeSpliterator.OfRef<>(Node_Fields.this);
			}

			public override void CopyInto(T[] array, int offset)
			{
				Objects.RequireNonNull(array);
				Left.CopyInto(array, offset);
				// Cast to int is safe since it is the callers responsibility to
				// ensure that there is sufficient room in the array
				Right.CopyInto(array, offset + (int) Left.Count());
			}

			public override T[] AsArray(IntFunction<T[]> generator)
			{
				long Node_Fields.Size = Count();
				if (Node_Fields.Size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				T[] array = generator.Apply((int) Node_Fields.Size);
				CopyInto(array, Node_Fields.0);
				return array;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base T> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				Left.ForEach(consumer);
				Right.ForEach(consumer);
			}

			public override Node<T> Truncate(long from, long to, IntFunction<T[]> generator)
			{
				if (from == Node_Fields.0 && to == Count())
				{
					return Node_Fields.this;
				}
				long leftCount = Left.Count();
				if (from >= leftCount)
				{
					return Right.truncate(from - leftCount, to - leftCount, generator);
				}
				else if (to <= leftCount)
				{
					return Left.truncate(from, to, generator);
				}
				else
				{
					return Nodes.Conc(Shape, Left.truncate(from, leftCount, generator), Right.truncate(Node_Fields.0, to - leftCount, generator));
				}
			}

			public override String ToString()
			{
				if (Count() < 32)
				{
					return string.Format("ConcNode[{0}.{1}]", Left, Right);
				}
				else
				{
					return string.Format("ConcNode[size={0:D}]", Count());
				}
			}

			private abstract class OfPrimitive<E, T_CONS, T_ARR, T_SPLITR, T_NODE> : AbstractConcNode<E, T_NODE>, Node_OfPrimitive<E, T_CONS, T_ARR, T_SPLITR, T_NODE> where T_SPLITR : java.util.Spliterator_OfPrimitive<E, T_CONS, T_SPLITR> where T_NODE : Node_OfPrimitive<E, T_CONS, T_ARR, T_SPLITR, T_NODE>
			{

				internal OfPrimitive(T_NODE left, T_NODE right) : base(left, right)
				{
				}

				public override void ForEach(T_CONS consumer)
				{
					outerInstance.Left.forEach(consumer);
					outerInstance.Right.forEach(consumer);
				}

				public override void CopyInto(T_ARR array, int offset)
				{
					outerInstance.Left.copyInto(array, offset);
					// Cast to int is safe since it is the callers responsibility to
					// ensure that there is sufficient room in the array
					outerInstance.Right.copyInto(array, offset + (int) outerInstance.Left.count());
				}

				public override T_ARR AsPrimitiveArray()
				{
					long Node_OfPrimitive_Fields.Node_Fields.size = outerInstance.Count();
					if (Node_OfPrimitive_Fields.Node_Fields.size >= MAX_ARRAY_SIZE)
					{
						throw new IllegalArgumentException(BAD_SIZE);
					}
					T_ARR array = newArray((int) Node_OfPrimitive_Fields.Node_Fields.size);
					CopyInto(array, Node_Fields.0);
					return array;
				}

				public override String ToString()
				{
					if (outerInstance.Count() < 32)
					{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						return string.Format("{0}[{1}.{2}]", Node_Fields.this.GetType().FullName, outerInstance.Left, outerInstance.Right);
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						return string.Format("{0}[size={1:D}]", Node_Fields.this.GetType().FullName, outerInstance.Count());
					}
				}
			}

			internal sealed class OfInt : ConcNode.OfPrimitive<Integer, IntConsumer, int[], java.util.Spliterator_OfInt, Node_OfInt>, Node_OfInt
			{

				internal OfInt(Node_OfInt left, Node_OfInt right) : base(left, right)
				{
				}

				public java.util.Spliterator_OfInt Node_OfInt_Fields.Node_Fields.spliterator()
				{
					return new InternalNodeSpliterator.OfInt(Node_OfInt_Fields.Node_Fields.this);
				}
			}

			internal sealed class OfLong : ConcNode.OfPrimitive<Long, LongConsumer, long[], java.util.Spliterator_OfLong, Node_OfLong>, Node_OfLong
			{

				internal OfLong(Node_OfLong left, Node_OfLong right) : base(left, right)
				{
				}

				public java.util.Spliterator_OfLong Node_OfLong_Fields.Node_Fields.spliterator()
				{
					return new InternalNodeSpliterator.OfLong(Node_OfLong_Fields.Node_Fields.this);
				}
			}

			internal sealed class OfDouble : ConcNode.OfPrimitive<Double, DoubleConsumer, double[], java.util.Spliterator_OfDouble, Node_OfDouble>, Node_OfDouble
			{

				internal OfDouble(Node_OfDouble left, Node_OfDouble right) : base(left, right)
				{
				}

				public java.util.Spliterator_OfDouble Node_OfDouble_Fields.Node_Fields.spliterator()
				{
					return new InternalNodeSpliterator.OfDouble(Node_OfDouble_Fields.Node_Fields.this);
				}
			}
		}

		/// <summary>
		/// Abstract class for spliterator for all internal node classes </summary>
		private abstract class InternalNodeSpliterator<T, S, N> : Spliterator<T> where S : java.util.Spliterator<T> where N : Node<T>
		{
			// Node we are pointing to
			// null if full traversal has occurred
			internal N CurNode;

			// next child of curNode to consume
			internal int CurChildIndex;

			// The spliterator of the curNode if that node is last and has no children.
			// This spliterator will be delegated to for splitting and traversing.
			// null if curNode has children
			internal S LastNodeSpliterator;

			// spliterator used while traversing with tryAdvance
			// null if no partial traversal has occurred
			internal S TryAdvanceSpliterator;

			// node stack used when traversing to search and find leaf nodes
			// null if no partial traversal has occurred
			internal Deque<N> TryAdvanceStack;

			internal InternalNodeSpliterator(N curNode)
			{
				this.CurNode = curNode;
			}

			/// <summary>
			/// Initiate a stack containing, in left-to-right order, the child nodes
			/// covered by this spliterator
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected final java.util.Deque<N> initStack()
			protected internal Deque<N> InitStack()
			{
				// Bias size to the case where leaf nodes are close to this node
				// 8 is the minimum initial capacity for the ArrayDeque implementation
				Deque<N> stack = new ArrayDeque<N>(8);
				for (int i = CurNode.ChildCount - 1; i >= CurChildIndex; i--)
				{
					stack.AddFirst((N) CurNode.getChild(i));
				}
				return stack;
			}

			/// <summary>
			/// Depth first search, in left-to-right order, of the node tree, using
			/// an explicit stack, to find the next non-empty leaf node.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected final N findNextLeafNode(java.util.Deque<N> stack)
			protected internal N FindNextLeafNode(Deque<N> stack)
			{
				N n = null;
				while ((n = stack.PollFirst()) != null)
				{
					if (n.ChildCount == 0)
					{
						if (n.count() > 0)
						{
							return n;
						}
					}
					else
					{
						for (int i = n.ChildCount - 1; i >= 0; i--)
						{
							stack.AddFirst((N) n.getChild(i));
						}
					}
				}

				return null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected final boolean initTryAdvance()
			protected internal bool InitTryAdvance()
			{
				if (CurNode == null)
				{
					return false;
				}

				if (TryAdvanceSpliterator == null)
				{
					if (LastNodeSpliterator == null)
					{
						// Initiate the node stack
						TryAdvanceStack = InitStack();
						N leaf = FindNextLeafNode(TryAdvanceStack);
						if (leaf != null)
						{
							TryAdvanceSpliterator = (S) leaf.spliterator();
						}
						else
						{
							// A non-empty leaf node was not found
							// No elements to traverse
							CurNode = null;
							return false;
						}
					}
					else
					{
						TryAdvanceSpliterator = LastNodeSpliterator;
					}
				}
				return true;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public final S trySplit()
			public override S TrySplit()
			{
				if (CurNode == null || TryAdvanceSpliterator != null)
				{
					return null; // Cannot split if fully or partially traversed
				}
				else if (LastNodeSpliterator != null)
				{
					return (S) LastNodeSpliterator.trySplit();
				}
				else if (CurChildIndex < CurNode.ChildCount - 1)
				{
					return (S) CurNode.getChild(CurChildIndex++).spliterator();
				}
				else
				{
					CurNode = (N) CurNode.getChild(CurChildIndex);
					if (CurNode.ChildCount == 0)
					{
						LastNodeSpliterator = (S) CurNode.spliterator();
						return (S) LastNodeSpliterator.trySplit();
					}
					else
					{
						CurChildIndex = 0;
						return (S) CurNode.getChild(CurChildIndex++).spliterator();
					}
				}
			}

			public override long EstimateSize()
			{
				if (CurNode == null)
				{
					return 0;
				}

				// Will not reflect the effects of partial traversal.
				// This is compliant with the specification
				if (LastNodeSpliterator != null)
				{
					return LastNodeSpliterator.estimateSize();
				}
				else
				{
					long size = 0;
					for (int i = CurChildIndex; i < CurNode.ChildCount; i++)
					{
						size += CurNode.getChild(i).count();
					}
					return size;
				}
			}

			public override int Characteristics()
			{
				return java.util.Spliterator_Fields.SIZED;
			}

			private sealed class OfRef<T> : InternalNodeSpliterator<T, Spliterator<T>, Node<T>>
			{

				internal OfRef(Node<T> curNode) : base(curNode)
				{
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public boolean tryAdvance(java.util.function.Consumer<? base T> consumer)
				public override bool tryAdvance<T1>(Consumer<T1> consumer)
				{
					if (!InitTryAdvance())
					{
						return false;
					}

					bool hasNext = TryAdvanceSpliterator.TryAdvance(consumer);
					if (!hasNext)
					{
						if (LastNodeSpliterator == null)
						{
							// Advance to the spliterator of the next non-empty leaf node
							Node<T> leaf = FindNextLeafNode(TryAdvanceStack);
							if (leaf != null)
							{
								TryAdvanceSpliterator = leaf.Spliterator();
								// Since the node is not-empty the spliterator can be advanced
								return TryAdvanceSpliterator.TryAdvance(consumer);
							}
						}
						// No more elements to traverse
						CurNode = null;
					}
					return hasNext;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? base T> consumer)
				public override void forEachRemaining<T1>(Consumer<T1> consumer)
				{
					if (CurNode == null)
					{
						return;
					}

					if (TryAdvanceSpliterator == null)
					{
						if (LastNodeSpliterator == null)
						{
							Deque<Node<T>> stack = InitStack();
							Node<T> leaf;
							while ((leaf = FindNextLeafNode(stack)) != null)
							{
								leaf.ForEach(consumer);
							}
							CurNode = null;
						}
						else
						{
							LastNodeSpliterator.forEachRemaining(consumer);
						}
					}
					else
					{
						while (TryAdvance(consumer))
						{
						}
					}
				}
			}

			private abstract class OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, N> : InternalNodeSpliterator<T, T_SPLITR, N>, java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where N : Node_OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, N>
			{

				internal OfPrimitive(N cur) : base(cur)
				{
				}

				public override bool TryAdvance(T_CONS consumer)
				{
					if (!outerInstance.InitTryAdvance())
					{
						return false;
					}

					bool hasNext = outerInstance.TryAdvanceSpliterator.tryAdvance(consumer);
					if (!hasNext)
					{
						if (outerInstance.LastNodeSpliterator == null)
						{
							// Advance to the spliterator of the next non-empty leaf node
							N leaf = outerInstance.FindNextLeafNode(outerInstance.TryAdvanceStack);
							if (leaf != null)
							{
								outerInstance.TryAdvanceSpliterator = leaf.spliterator();
								// Since the node is not-empty the spliterator can be advanced
								return outerInstance.TryAdvanceSpliterator.tryAdvance(consumer);
							}
						}
						// No more elements to traverse
						outerInstance.CurNode = null;
					}
					return hasNext;
				}

				public override void ForEachRemaining(T_CONS consumer)
				{
					if (outerInstance.CurNode == null)
					{
						return;
					}

					if (outerInstance.TryAdvanceSpliterator == null)
					{
						if (outerInstance.LastNodeSpliterator == null)
						{
							Deque<N> stack = outerInstance.InitStack();
							N leaf;
							while ((leaf = outerInstance.FindNextLeafNode(stack)) != null)
							{
								leaf.forEach(consumer);
							}
							outerInstance.CurNode = null;
						}
						else
						{
							outerInstance.LastNodeSpliterator.forEachRemaining(consumer);
						}
					}
					else
					{
						while (TryAdvance(consumer))
						{
						}
					}
				}
			}

			private sealed class OfInt : OfPrimitive<Integer, IntConsumer, int[], java.util.Spliterator_OfInt, Node_OfInt>, java.util.Spliterator_OfInt
			{

				internal OfInt(Node_OfInt cur) : base(cur)
				{
				}
			}

			private sealed class OfLong : OfPrimitive<Long, LongConsumer, long[], java.util.Spliterator_OfLong, Node_OfLong>, java.util.Spliterator_OfLong
			{

				internal OfLong(Node_OfLong cur) : base(cur)
				{
				}
			}

			private sealed class OfDouble : OfPrimitive<Double, DoubleConsumer, double[], java.util.Spliterator_OfDouble, Node_OfDouble>, java.util.Spliterator_OfDouble
			{

				internal OfDouble(Node_OfDouble cur) : base(cur)
				{
				}
			}
		}

		/// <summary>
		/// Fixed-sized builder class for reference nodes
		/// </summary>
		private sealed class FixedNodeBuilder<T> : ArrayNode<T>, Node_Builder<T>
		{

			internal FixedNodeBuilder(long Node_Fields, IntFunction<T[]> generator) : base(Node_Fields.Size, generator)
			{
				Debug.Assert(Node_Fields.Size < MAX_ARRAY_SIZE);
			}

			public override Node<T> Build()
			{
				if (CurSize < Array.length)
				{
					throw new IllegalStateException(string.Format("Current size {0:D} is less than fixed size {1:D}", CurSize, Array.length));
				}
				return Node_Fields.this;
			}

			public override void Begin(long Node_Fields)
			{
				if (Node_Fields.Size != Array.length)
				{
					throw new IllegalStateException(string.Format("Begin size {0:D} is not equal to fixed size {1:D}", Node_Fields.Size, Array.length));
				}
				CurSize = Node_Fields.0;
			}

			public override void Accept(T t)
			{
				if (CurSize < Array.length)
				{
					Array[CurSize++] = t;
				}
				else
				{
					throw new IllegalStateException(string.Format("Accept exceeded fixed size of {0:D}", Array.length));
				}
			}

			public override void End()
			{
				if (CurSize < Array.length)
				{
					throw new IllegalStateException(string.Format("End size {0:D} is less than fixed size {1:D}", CurSize, Array.length));
				}
			}

			public override String ToString()
			{
				return string.Format("FixedNodeBuilder[{0:D}][{1}]", Array.length - CurSize, Arrays.ToString(Array));
			}
		}

		/// <summary>
		/// Variable-sized builder class for reference nodes
		/// </summary>
		private sealed class SpinedNodeBuilder<T> : SpinedBuffer<T>, Node<T>, Node_Builder<T>
		{
			internal bool Building = false;

			internal SpinedNodeBuilder() // Avoid creation of special accessor
			{
			}

			public override Spliterator<T> Node_Fields.spliterator()
			{
				Debug.Assert(!Building, "during building");
				return base.Spliterator();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base T> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				Debug.Assert(!Building, "during building");
				base.ForEach(consumer);
			}

			//
			public override void Begin(long Node_Fields)
			{
				Debug.Assert(!Building, "was already building");
				Building = true;
				Clear();
				EnsureCapacity(Node_Fields.Size);
			}

			public override void Accept(T t)
			{
				Debug.Assert(Building, "not building");
				base.Accept(t);
			}

			public override void End()
			{
				Debug.Assert(Building, "was not building");
				Building = false;
				// @@@ check begin(size) and size
			}

			public override void CopyInto(T[] array, int offset)
			{
				Debug.Assert(!Building, "during building");
				base.CopyInto(array, offset);
			}

			public override T[] AsArray(IntFunction<T[]> arrayFactory)
			{
				Debug.Assert(!Building, "during building");
				return base.AsArray(arrayFactory);
			}

			public override Node<T> Build()
			{
				Debug.Assert(!Building, "during building");
				return Node_Fields.this;
			}
		}

		//

		private static readonly int[] EMPTY_INT_ARRAY = new int[0];
		private static readonly long[] EMPTY_LONG_ARRAY = new long[0];
		private static readonly double[] EMPTY_DOUBLE_ARRAY = new double[0];

		private class IntArrayNode : Node_OfInt
		{
			internal readonly Node_OfInt_Fields.int[] Node_OfInt_Fields;
			internal Node_OfInt_Fields.int CurSize;

			internal IntArrayNode(long size)
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				this.Array = new Node_OfInt_Fields.int[(Node_OfInt_Fields.int) size];
				this.CurSize = 0;
			}

			internal IntArrayNode(Node_OfInt_Fields.int[] Node_OfInt_Fields)
			{
				this.Array = Node_OfInt_Fields.Array;
				this.CurSize = Node_OfInt_Fields.Array.Length;
			}

			// Node

			public virtual java.util.Spliterator_OfInt Spliterator()
			{
				return Arrays.Spliterator(Node_OfInt_Fields.Array, 0, CurSize);
			}

			public virtual Node_OfInt_Fields.int[] AsPrimitiveArray()
			{
				if (Node_OfInt_Fields.Array.Length == CurSize)
				{
					return Node_OfInt_Fields.Array;
				}
				else
				{
					return Arrays.CopyOf(Node_OfInt_Fields.Array, CurSize);
				}
			}

			public virtual void CopyInto(Node_OfInt_Fields.int[] dest, Node_OfInt_Fields.int destOffset)
			{
				System.Array.Copy(Node_OfInt_Fields.Array, 0, dest, destOffset, CurSize);
			}

			public override long Count()
			{
				return CurSize;
			}

			public virtual void ForEach(IntConsumer consumer)
			{
				for (Node_OfInt_Fields.int i = 0; i < CurSize; i++)
				{
					consumer.Accept(Node_OfInt_Fields.Array[i]);
				}
			}

			public override String ToString()
			{
				return string.Format("IntArrayNode[{0:D}][{1}]", Node_OfInt_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfInt_Fields.Array));
			}
		}

		private class LongArrayNode : Node_OfLong
		{
			internal readonly Node_OfLong_Fields.long[] Node_OfLong_Fields;
			internal int CurSize;

			internal LongArrayNode(Node_OfLong_Fields.long size)
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				this.Array = new Node_OfLong_Fields.long[(int) size];
				this.CurSize = 0;
			}

			internal LongArrayNode(Node_OfLong_Fields.long[] Node_OfLong_Fields)
			{
				this.Array = Node_OfLong_Fields.Array;
				this.CurSize = Node_OfLong_Fields.Array.Length;
			}

			public virtual java.util.Spliterator_OfLong Spliterator()
			{
				return Arrays.Spliterator(Node_OfLong_Fields.Array, 0, CurSize);
			}

			public virtual Node_OfLong_Fields.long[] AsPrimitiveArray()
			{
				if (Node_OfLong_Fields.Array.Length == CurSize)
				{
					return Node_OfLong_Fields.Array;
				}
				else
				{
					return Arrays.CopyOf(Node_OfLong_Fields.Array, CurSize);
				}
			}

			public virtual void CopyInto(Node_OfLong_Fields.long[] dest, int destOffset)
			{
				System.Array.Copy(Node_OfLong_Fields.Array, 0, dest, destOffset, CurSize);
			}

			public override Node_OfLong_Fields.long Count()
			{
				return CurSize;
			}

			public virtual void ForEach(LongConsumer consumer)
			{
				for (int i = 0; i < CurSize; i++)
				{
					consumer.Accept(Node_OfLong_Fields.Array[i]);
				}
			}

			public override String ToString()
			{
				return string.Format("LongArrayNode[{0:D}][{1}]", Node_OfLong_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfLong_Fields.Array));
			}
		}

		private class DoubleArrayNode : Node_OfDouble
		{
			internal readonly Node_OfDouble_Fields.double[] Node_OfDouble_Fields;
			internal int CurSize;

			internal DoubleArrayNode(long size)
			{
				if (size >= MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(BAD_SIZE);
				}
				this.Array = new Node_OfDouble_Fields.double[(int) size];
				this.CurSize = 0;
			}

			internal DoubleArrayNode(Node_OfDouble_Fields.double[] Node_OfDouble_Fields)
			{
				this.Array = Node_OfDouble_Fields.Array;
				this.CurSize = Node_OfDouble_Fields.Array.Length;
			}

			public virtual java.util.Spliterator_OfDouble Spliterator()
			{
				return Arrays.Spliterator(Node_OfDouble_Fields.Array, 0, CurSize);
			}

			public virtual Node_OfDouble_Fields.double[] AsPrimitiveArray()
			{
				if (Node_OfDouble_Fields.Array.Length == CurSize)
				{
					return Node_OfDouble_Fields.Array;
				}
				else
				{
					return Arrays.CopyOf(Node_OfDouble_Fields.Array, CurSize);
				}
			}

			public virtual void CopyInto(Node_OfDouble_Fields.double[] dest, int destOffset)
			{
				System.Array.Copy(Node_OfDouble_Fields.Array, 0, dest, destOffset, CurSize);
			}

			public override long Count()
			{
				return CurSize;
			}

			public virtual void ForEach(DoubleConsumer consumer)
			{
				for (int i = 0; i < CurSize; i++)
				{
					consumer.Accept(Node_OfDouble_Fields.Array[i]);
				}
			}

			public override String ToString()
			{
				return string.Format("DoubleArrayNode[{0:D}][{1}]", Node_OfDouble_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfDouble_Fields.Array));
			}
		}

		private sealed class IntFixedNodeBuilder : IntArrayNode, Node_Builder_OfInt
		{

			internal IntFixedNodeBuilder(long size) : base(size)
			{
				Debug.Assert(size < MAX_ARRAY_SIZE);
			}

			public Node_OfInt Build()
			{
				if (CurSize < Node_OfInt_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Current size {0:D} is less than fixed size {1:D}", CurSize, Node_OfInt_Fields.Array.Length));
				}

				return this;
			}

			public override void Begin(long size)
			{
				if (size != Node_OfInt_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, Node_OfInt_Fields.Array.Length));
				}

				CurSize = 0;
			}

			public void Accept(Node_OfInt_Fields.int i)
			{
				if (CurSize < Node_OfInt_Fields.Array.Length)
				{
					Node_OfInt_Fields.Array[CurSize++] = i;
				}
				else
				{
					throw new IllegalStateException(string.Format("Accept exceeded fixed size of {0:D}", Node_OfInt_Fields.Array.Length));
				}
			}

			public override void End()
			{
				if (CurSize < Node_OfInt_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("End size {0:D} is less than fixed size {1:D}", CurSize, Node_OfInt_Fields.Array.Length));
				}
			}

			public override String ToString()
			{
				return string.Format("IntFixedNodeBuilder[{0:D}][{1}]", Node_OfInt_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfInt_Fields.Array));
			}
		}

		private sealed class LongFixedNodeBuilder : LongArrayNode, Node_Builder_OfLong
		{

			internal LongFixedNodeBuilder(Node_OfLong_Fields.long size) : base(size)
			{
				Debug.Assert(size < MAX_ARRAY_SIZE);
			}

			public Node_OfLong Build()
			{
				if (CurSize < Node_OfLong_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Current size {0:D} is less than fixed size {1:D}", CurSize, Node_OfLong_Fields.Array.Length));
				}

				return this;
			}

			public override void Begin(Node_OfLong_Fields.long size)
			{
				if (size != Node_OfLong_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, Node_OfLong_Fields.Array.Length));
				}

				CurSize = 0;
			}

			public void Accept(Node_OfLong_Fields.long i)
			{
				if (CurSize < Node_OfLong_Fields.Array.Length)
				{
					Node_OfLong_Fields.Array[CurSize++] = i;
				}
				else
				{
					throw new IllegalStateException(string.Format("Accept exceeded fixed size of {0:D}", Node_OfLong_Fields.Array.Length));
				}
			}

			public override void End()
			{
				if (CurSize < Node_OfLong_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("End size {0:D} is less than fixed size {1:D}", CurSize, Node_OfLong_Fields.Array.Length));
				}
			}

			public override String ToString()
			{
				return string.Format("LongFixedNodeBuilder[{0:D}][{1}]", Node_OfLong_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfLong_Fields.Array));
			}
		}

		private sealed class DoubleFixedNodeBuilder : DoubleArrayNode, Node_Builder_OfDouble
		{

			internal DoubleFixedNodeBuilder(long size) : base(size)
			{
				Debug.Assert(size < MAX_ARRAY_SIZE);
			}

			public Node_OfDouble Build()
			{
				if (CurSize < Node_OfDouble_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Current size {0:D} is less than fixed size {1:D}", CurSize, Node_OfDouble_Fields.Array.Length));
				}

				return this;
			}

			public override void Begin(long size)
			{
				if (size != Node_OfDouble_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("Begin size {0:D} is not equal to fixed size {1:D}", size, Node_OfDouble_Fields.Array.Length));
				}

				CurSize = 0;
			}

			public void Accept(Node_OfDouble_Fields.double i)
			{
				if (CurSize < Node_OfDouble_Fields.Array.Length)
				{
					Node_OfDouble_Fields.Array[CurSize++] = i;
				}
				else
				{
					throw new IllegalStateException(string.Format("Accept exceeded fixed size of {0:D}", Node_OfDouble_Fields.Array.Length));
				}
			}

			public override void End()
			{
				if (CurSize < Node_OfDouble_Fields.Array.Length)
				{
					throw new IllegalStateException(string.Format("End size {0:D} is less than fixed size {1:D}", CurSize, Node_OfDouble_Fields.Array.Length));
				}
			}

			public override String ToString()
			{
				return string.Format("DoubleFixedNodeBuilder[{0:D}][{1}]", Node_OfDouble_Fields.Array.Length - CurSize, Arrays.ToString(Node_OfDouble_Fields.Array));
			}
		}

		private sealed class IntSpinedNodeBuilder : SpinedBuffer.OfInt, Node_OfInt, Node_Builder_OfInt
		{
			internal bool Building = false;

			internal IntSpinedNodeBuilder() // Avoid creation of special accessor
			{
			}

			public override java.util.Spliterator_OfInt Spliterator()
			{
				Debug.Assert(!Building, "during building");
				return base.Spliterator();
			}

			public void ForEach(IntConsumer consumer)
			{
				Debug.Assert(!Building, "during building");
				base.ForEach(consumer);
			}

			//
			public override void Begin(long size)
			{
				Debug.Assert(!Building, "was already building");
				Building = true;
				Clear();
				EnsureCapacity(size);
			}

			public void Accept(Node_OfInt_Fields.int i)
			{
				Debug.Assert(Building, "not building");
				base.Accept(i);
			}

			public override void End()
			{
				Debug.Assert(Building, "was not building");
				Building = false;
				// @@@ check begin(size) and size
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void copyInto(Node_OfInt_Fields.int[] Node_OfInt_Fields.array, Node_OfInt_Fields.int offset) throws IndexOutOfBoundsException
			public void CopyInto(Node_OfInt_Fields.int[] Node_OfInt_Fields, Node_OfInt_Fields.int offset)
			{
				Debug.Assert(!Building, "during building");
				base.CopyInto(Node_OfInt_Fields.Array, offset);
			}

			public override Node_OfInt_Fields.int[] AsPrimitiveArray()
			{
				Debug.Assert(!Building, "during building");
				return base.AsPrimitiveArray();
			}

			public Node_OfInt Build()
			{
				Debug.Assert(!Building, "during building");
				return this;
			}
		}

		private sealed class LongSpinedNodeBuilder : SpinedBuffer.OfLong, Node_OfLong, Node_Builder_OfLong
		{
			internal bool Building = false;

			internal LongSpinedNodeBuilder() // Avoid creation of special accessor
			{
			}

			public override java.util.Spliterator_OfLong Spliterator()
			{
				Debug.Assert(!Building, "during building");
				return base.Spliterator();
			}

			public void ForEach(LongConsumer consumer)
			{
				Debug.Assert(!Building, "during building");
				base.ForEach(consumer);
			}

			//
			public override void Begin(Node_OfLong_Fields.long size)
			{
				Debug.Assert(!Building, "was already building");
				Building = true;
				Clear();
				EnsureCapacity(size);
			}

			public void Accept(Node_OfLong_Fields.long i)
			{
				Debug.Assert(Building, "not building");
				base.Accept(i);
			}

			public override void End()
			{
				Debug.Assert(Building, "was not building");
				Building = false;
				// @@@ check begin(size) and size
			}

			public void CopyInto(Node_OfLong_Fields.long[] Node_OfLong_Fields, int offset)
			{
				Debug.Assert(!Building, "during building");
				base.CopyInto(Node_OfLong_Fields.Array, offset);
			}

			public override Node_OfLong_Fields.long[] AsPrimitiveArray()
			{
				Debug.Assert(!Building, "during building");
				return base.AsPrimitiveArray();
			}

			public Node_OfLong Build()
			{
				Debug.Assert(!Building, "during building");
				return this;
			}
		}

		private sealed class DoubleSpinedNodeBuilder : SpinedBuffer.OfDouble, Node_OfDouble, Node_Builder_OfDouble
		{
			internal bool Building = false;

			internal DoubleSpinedNodeBuilder() // Avoid creation of special accessor
			{
			}

			public override java.util.Spliterator_OfDouble Spliterator()
			{
				Debug.Assert(!Building, "during building");
				return base.Spliterator();
			}

			public void ForEach(DoubleConsumer consumer)
			{
				Debug.Assert(!Building, "during building");
				base.ForEach(consumer);
			}

			//
			public override void Begin(long size)
			{
				Debug.Assert(!Building, "was already building");
				Building = true;
				Clear();
				EnsureCapacity(size);
			}

			public void Accept(Node_OfDouble_Fields.double i)
			{
				Debug.Assert(Building, "not building");
				base.Accept(i);
			}

			public override void End()
			{
				Debug.Assert(Building, "was not building");
				Building = false;
				// @@@ check begin(size) and size
			}

			public void CopyInto(Node_OfDouble_Fields.double[] Node_OfDouble_Fields, int offset)
			{
				Debug.Assert(!Building, "during building");
				base.CopyInto(Node_OfDouble_Fields.Array, offset);
			}

			public override Node_OfDouble_Fields.double[] AsPrimitiveArray()
			{
				Debug.Assert(!Building, "during building");
				return base.AsPrimitiveArray();
			}

			public Node_OfDouble Build()
			{
				Debug.Assert(!Building, "during building");
				return this;
			}
		}

		/*
		 * This and subclasses are not intended to be serializable
		 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static abstract class SizedCollectorTask<P_IN, P_OUT, T_SINK extends Sink<P_OUT>, K extends SizedCollectorTask<P_IN, P_OUT, T_SINK, K>> extends java.util.concurrent.CountedCompleter<Void> implements Sink<P_OUT>
		private abstract class SizedCollectorTask<P_IN, P_OUT, T_SINK, K> : CountedCompleter<Void>, Sink<P_OUT> where T_SINK : Sink<P_OUT> where K : SizedCollectorTask<P_IN, P_OUT, T_SINK, K>
		{
			protected internal readonly Spliterator<P_IN> Spliterator;
			protected internal readonly PipelineHelper<P_OUT> Helper;
			protected internal readonly long TargetSize;
			protected internal long Offset;
			protected internal long Length;
			// For Sink implementation
			protected internal int Index, Fence;

			internal SizedCollectorTask(Spliterator<P_IN> spliterator, PipelineHelper<P_OUT> helper, int arrayLength)
			{
				Debug.Assert(spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED));
				this.Spliterator = spliterator;
				this.Helper = helper;
				this.TargetSize = AbstractTask.SuggestTargetSize(spliterator.EstimateSize());
				this.Offset = 0;
				this.Length = arrayLength;
			}

			internal SizedCollectorTask(K parent, Spliterator<P_IN> spliterator, long offset, long length, int arrayLength) : base(parent)
			{
				Debug.Assert(spliterator.hasCharacteristics(java.util.Spliterator_Fields.SUBSIZED));
				this.Spliterator = spliterator;
				this.Helper = parent.helper;
				this.TargetSize = parent.targetSize;
				this.Offset = offset;
				this.Length = length;

				if (offset < 0 || length < 0 || (offset + length - 1 >= arrayLength))
				{
					throw new IllegalArgumentException(string.Format("offset and length interval [{0:D}, {1:D} + {2:D}) is not within array size interval [0, {3:D})", offset, offset, length, arrayLength));
				}
			}

			public override void Compute()
			{
				SizedCollectorTask<P_IN, P_OUT, T_SINK, K> task = this;
				Spliterator<P_IN> rightSplit = Spliterator, leftSplit ;
				while (rightSplit.EstimateSize() > task.TargetSize && (leftSplit = rightSplit.TrySplit()) != null)
				{
					task.PendingCount = 1;
					long leftSplitSize = leftSplit.EstimateSize();
					task.MakeChild(leftSplit, task.Offset, leftSplitSize).fork();
					task = task.MakeChild(rightSplit, task.Offset + leftSplitSize, task.Length - leftSplitSize);
				}

				Debug.Assert(task.Offset + task.Length < MAX_ARRAY_SIZE);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T_SINK sink = (T_SINK) task;
				T_SINK sink = (T_SINK) task;
				task.Helper.WrapAndCopyInto(sink, rightSplit);
				task.propagateCompletion();
			}

			internal abstract K MakeChild(Spliterator<P_IN> spliterator, long offset, long size);

			public override void Begin(long size)
			{
				if (size > Length)
				{
					throw new IllegalStateException("size passed to Sink.begin exceeds array length");
				}
				// Casts to int are safe since absolute size is verified to be within
				// bounds when the root concrete SizedCollectorTask is constructed
				// with the shared array
				Index = (int) Offset;
				Fence = Index + (int) Length;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OfRef<P_IN, P_OUT> extends SizedCollectorTask<P_IN, P_OUT, Sink<P_OUT>, OfRef<P_IN, P_OUT>> implements Sink<P_OUT>
			internal sealed class OfRef<P_IN, P_OUT> : SizedCollectorTask<P_IN, P_OUT, Sink<P_OUT>, OfRef<P_IN, P_OUT>>, Sink<P_OUT>
			{
				internal readonly P_OUT[] Array;

				internal OfRef(Spliterator<P_IN> spliterator, PipelineHelper<P_OUT> helper, P_OUT[] array) : base(spliterator, helper, array.Length)
				{
					this.Array = array;
				}

				internal OfRef(OfRef<P_IN, P_OUT> parent, Spliterator<P_IN> spliterator, long offset, long length) : base(parent, spliterator, offset, length, parent.Array.Length)
				{
					this.Array = parent.Array;
				}

				internal override OfRef<P_IN, P_OUT> MakeChild(Spliterator<P_IN> spliterator, long offset, long size)
				{
					return new OfRef<>(this, spliterator, offset, size);
				}

				public override void Accept(P_OUT value)
				{
					if (Index >= Fence)
					{
						throw new IndexOutOfBoundsException(Convert.ToString(Index));
					}
					Array[Index++] = value;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OfInt<P_IN> extends SizedCollectorTask<P_IN, Integer, Sink_OfInt, OfInt<P_IN>> implements Sink_OfInt
			internal sealed class OfInt<P_IN> : SizedCollectorTask<P_IN, Integer, Sink_OfInt, OfInt<P_IN>>, Sink_OfInt
			{
				internal readonly int[] Array;

				internal OfInt(Spliterator<P_IN> spliterator, PipelineHelper<Integer> helper, int[] array) : base(spliterator, helper, array.Length)
				{
					this.Array = array;
				}

				internal OfInt(SizedCollectorTask.OfInt<P_IN> parent, Spliterator<P_IN> spliterator, long offset, long length) : base(parent, spliterator, offset, length, parent.Array.Length)
				{
					this.Array = parent.Array;
				}

				internal override SizedCollectorTask.OfInt<P_IN> MakeChild(Spliterator<P_IN> spliterator, long offset, long size)
				{
					return new SizedCollectorTask.OfInt<>(this, spliterator, offset, size);
				}

				public void Accept(int value)
				{
					if (Index >= Fence)
					{
						throw new IndexOutOfBoundsException(Convert.ToString(Index));
					}
					Array[Index++] = value;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OfLong<P_IN> extends SizedCollectorTask<P_IN, Long, Sink_OfLong, OfLong<P_IN>> implements Sink_OfLong
			internal sealed class OfLong<P_IN> : SizedCollectorTask<P_IN, Long, Sink_OfLong, OfLong<P_IN>>, Sink_OfLong
			{
				internal readonly long[] Array;

				internal OfLong(Spliterator<P_IN> spliterator, PipelineHelper<Long> helper, long[] array) : base(spliterator, helper, array.Length)
				{
					this.Array = array;
				}

				internal OfLong(SizedCollectorTask.OfLong<P_IN> parent, Spliterator<P_IN> spliterator, long offset, long length) : base(parent, spliterator, offset, length, parent.Array.Length)
				{
					this.Array = parent.Array;
				}

				internal override SizedCollectorTask.OfLong<P_IN> MakeChild(Spliterator<P_IN> spliterator, long offset, long size)
				{
					return new SizedCollectorTask.OfLong<>(this, spliterator, offset, size);
				}

				public void Accept(long value)
				{
					if (Index >= Fence)
					{
						throw new IndexOutOfBoundsException(Convert.ToString(Index));
					}
					Array[Index++] = value;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OfDouble<P_IN> extends SizedCollectorTask<P_IN, Double, Sink_OfDouble, OfDouble<P_IN>> implements Sink_OfDouble
			internal sealed class OfDouble<P_IN> : SizedCollectorTask<P_IN, Double, Sink_OfDouble, OfDouble<P_IN>>, Sink_OfDouble
			{
				internal readonly double[] Array;

				internal OfDouble(Spliterator<P_IN> spliterator, PipelineHelper<Double> helper, double[] array) : base(spliterator, helper, array.Length)
				{
					this.Array = array;
				}

				internal OfDouble(SizedCollectorTask.OfDouble<P_IN> parent, Spliterator<P_IN> spliterator, long offset, long length) : base(parent, spliterator, offset, length, parent.Array.Length)
				{
					this.Array = parent.Array;
				}

				internal override SizedCollectorTask.OfDouble<P_IN> MakeChild(Spliterator<P_IN> spliterator, long offset, long size)
				{
					return new SizedCollectorTask.OfDouble<>(this, spliterator, offset, size);
				}

				public void Accept(double value)
				{
					if (Index >= Fence)
					{
						throw new IndexOutOfBoundsException(Convert.ToString(Index));
					}
					Array[Index++] = value;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static abstract class ToArrayTask<T, T_NODE extends Node<T>, K extends ToArrayTask<T, T_NODE, K>> extends java.util.concurrent.CountedCompleter<Void>
		private abstract class ToArrayTask<T, T_NODE, K> : CountedCompleter<Void> where T_NODE : Node<T> where K : ToArrayTask<T, T_NODE, K>
		{
			protected internal readonly T_NODE Node;
			protected internal readonly int Offset;

			internal ToArrayTask(T_NODE node, int offset)
			{
				this.Node = node;
				this.Offset = offset;
			}

			internal ToArrayTask(K parent, T_NODE node, int offset) : base(parent)
			{
				this.Node = node;
				this.Offset = offset;
			}

			internal abstract void CopyNodeToArray();

			internal abstract K MakeChild(int childIndex, int offset);

			public override void Compute()
			{
				ToArrayTask<T, T_NODE, K> task = this;
				while (true)
				{
					if (task.Node.ChildCount == 0)
					{
						task.CopyNodeToArray();
						task.propagateCompletion();
						return;
					}
					else
					{
						task.PendingCount = task.Node.ChildCount - 1;

						int size = 0;
						int i = 0;
						for (;i < task.Node.ChildCount - 1; i++)
						{
							K leftTask = task.MakeChild(i, task.Offset + size);
							size += leftTask.node.count();
							leftTask.fork();
						}
						task = task.MakeChild(i, task.Offset + size);
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfRef<T> extends ToArrayTask<T, Node<T>, OfRef<T>>
			private sealed class OfRef<T> : ToArrayTask<T, Node<T>, OfRef<T>>
			{
				internal readonly T[] Array;

				internal OfRef(Node<T> node, T[] array, int offset) : base(node, offset)
				{
					this.Array = array;
				}

				internal OfRef(OfRef<T> parent, Node<T> node, int offset) : base(parent, node, offset)
				{
					this.Array = parent.Array;
				}

				internal override OfRef<T> MakeChild(int childIndex, int offset)
				{
					return new OfRef<>(this, Node.getChild(childIndex), offset);
				}

				internal override void CopyNodeToArray()
				{
					Node.CopyInto(Array, Offset);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static class OfPrimitive<T, T_CONS, T_ARR, T_SPLITR extends java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR>, T_NODE extends Node_OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE>> extends ToArrayTask<T, T_NODE, OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE>>
			private class OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE> : ToArrayTask<T, T_NODE, OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE>> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_NODE : Node_OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE>
			{
				internal readonly T_ARR Array;

				internal OfPrimitive(T_NODE node, T_ARR array, int offset) : base(node, offset)
				{
					this.Array = array;
				}

				internal OfPrimitive(OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE> parent, T_NODE node, int offset) : base(parent, node, offset)
				{
					this.Array = parent.Array;
				}

				internal override OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE> MakeChild(int childIndex, int offset)
				{
					return new OfPrimitive<>(this, outerInstance.Node.getChild(childIndex), offset);
				}

				internal override void CopyNodeToArray()
				{
					outerInstance.Node.copyInto(Array, outerInstance.Offset);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfInt extends OfPrimitive<Integer, java.util.function.IntConsumer, int[] , java.util.Spliterator_OfInt, Node_OfInt>
			private sealed class OfInt : OfPrimitive<Integer, IntConsumer, int[], java.util.Spliterator_OfInt, Node_OfInt>
			{
				internal OfInt(Node_OfInt node, int[] array, int offset) : base(node, array, offset)
				{
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfLong extends OfPrimitive<Long, java.util.function.LongConsumer, long[] , java.util.Spliterator_OfLong, Node_OfLong>
			private sealed class OfLong : OfPrimitive<Long, LongConsumer, long[], java.util.Spliterator_OfLong, Node_OfLong>
			{
				internal OfLong(Node_OfLong node, long[] array, int offset) : base(node, array, offset)
				{
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfDouble extends OfPrimitive<Double, java.util.function.DoubleConsumer, double[] , java.util.Spliterator_OfDouble, Node_OfDouble>
			private sealed class OfDouble : OfPrimitive<Double, DoubleConsumer, double[], java.util.Spliterator_OfDouble, Node_OfDouble>
			{
				internal OfDouble(Node_OfDouble node, double[] array, int offset) : base(node, array, offset)
				{
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static class CollectorTask<P_IN, P_OUT, T_NODE extends Node<P_OUT>, T_BUILDER extends Node_Builder<P_OUT>> extends AbstractTask<P_IN, P_OUT, T_NODE, CollectorTask<P_IN, P_OUT, T_NODE, T_BUILDER>>
		private class CollectorTask<P_IN, P_OUT, T_NODE, T_BUILDER> : AbstractTask<P_IN, P_OUT, T_NODE, CollectorTask<P_IN, P_OUT, T_NODE, T_BUILDER>> where T_NODE : Node<P_OUT> where T_BUILDER : Node_Builder<P_OUT>
		{
			protected internal readonly PipelineHelper<P_OUT> Helper;
			protected internal readonly LongFunction<T_BUILDER> BuilderFactory;
			protected internal readonly BinaryOperator<T_NODE> ConcFactory;

			internal CollectorTask(PipelineHelper<P_OUT> helper, Spliterator<P_IN> spliterator, LongFunction<T_BUILDER> builderFactory, BinaryOperator<T_NODE> concFactory) : base(helper, spliterator)
			{
				this.Helper = helper;
				this.BuilderFactory = builderFactory;
				this.ConcFactory = concFactory;
			}

			internal CollectorTask(CollectorTask<P_IN, P_OUT, T_NODE, T_BUILDER> parent, Spliterator<P_IN> spliterator) : base(parent, spliterator)
			{
				Helper = parent.Helper;
				BuilderFactory = parent.BuilderFactory;
				ConcFactory = parent.ConcFactory;
			}

			protected internal override CollectorTask<P_IN, P_OUT, T_NODE, T_BUILDER> MakeChild(Spliterator<P_IN> spliterator)
			{
				return new CollectorTask<>(this, spliterator);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") protected T_NODE doLeaf()
			protected internal override T_NODE DoLeaf()
			{
				T_BUILDER builder = BuilderFactory.Apply(Helper.ExactOutputSizeIfKnown(spliterator));
				return (T_NODE) Helper.WrapAndCopyInto(builder, spliterator).build();
			}

			public override void onCompletion<T1>(CountedCompleter<T1> caller)
			{
				if (!Leaf)
				{
					LocalResult = ConcFactory.Apply(leftChild.LocalResult, rightChild.LocalResult);
				}
				base.OnCompletion(caller);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfRef<P_IN, P_OUT> extends CollectorTask<P_IN, P_OUT, Node<P_OUT>, Node_Builder<P_OUT>>
			private sealed class OfRef<P_IN, P_OUT> : CollectorTask<P_IN, P_OUT, Node<P_OUT>, Node_Builder<P_OUT>>
			{
				internal OfRef(PipelineHelper<P_OUT> helper, IntFunction<P_OUT[]> generator, Spliterator<P_IN> spliterator) : base(helper, spliterator, s -> Builder(s, generator), ConcNode::new)
				{
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfInt<P_IN> extends CollectorTask<P_IN, Integer, Node_OfInt, Node_Builder_OfInt>
			private sealed class OfInt<P_IN> : CollectorTask<P_IN, Integer, Node_OfInt, Node_Builder_OfInt>
			{
				internal OfInt(PipelineHelper<Integer> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator, Nodes::intBuilder, ConcNode.OfInt::new)
				{
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfLong<P_IN> extends CollectorTask<P_IN, Long, Node_OfLong, Node_Builder_OfLong>
			private sealed class OfLong<P_IN> : CollectorTask<P_IN, Long, Node_OfLong, Node_Builder_OfLong>
			{
				internal OfLong(PipelineHelper<Long> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator, Nodes::longBuilder, ConcNode.OfLong::new)
				{
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static final class OfDouble<P_IN> extends CollectorTask<P_IN, Double, Node_OfDouble, Node_Builder_OfDouble>
			private sealed class OfDouble<P_IN> : CollectorTask<P_IN, Double, Node_OfDouble, Node_Builder_OfDouble>
			{
				internal OfDouble(PipelineHelper<Double> helper, Spliterator<P_IN> spliterator) : base(helper, spliterator, Nodes::doubleBuilder, ConcNode.OfDouble::new)
				{
				}
			}
		}
	}

}