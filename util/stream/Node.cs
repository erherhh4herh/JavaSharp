using System;

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
	/// An immutable container for describing an ordered sequence of elements of some
	/// type {@code T}.
	/// 
	/// <para>A {@code Node} contains a fixed number of elements, which can be accessed
	/// via the <seealso cref="#count"/>, <seealso cref="#spliterator"/>, <seealso cref="#forEach"/>,
	/// <seealso cref="#asArray"/>, or <seealso cref="#copyInto"/> methods.  A {@code Node} may have zero
	/// or more child {@code Node}s; if it has no children (accessed via
	/// <seealso cref="#getChildCount"/> and <seealso cref="#getChild(int)"/>, it is considered <em>flat
	/// </em> or a <em>leaf</em>; if it has children, it is considered an
	/// <em>internal</em> node.  The size of an internal node is the sum of sizes of
	/// its children.
	/// 
	/// @apiNote
	/// </para>
	/// <para>A {@code Node} typically does not store the elements directly, but instead
	/// mediates access to one or more existing (effectively immutable) data
	/// structures such as a {@code Collection}, array, or a set of other
	/// {@code Node}s.  Commonly {@code Node}s are formed into a tree whose shape
	/// corresponds to the computation tree that produced the elements that are
	/// contained in the leaf nodes.  The use of {@code Node} within the stream
	/// framework is largely to avoid copying data unnecessarily during parallel
	/// operations.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of elements.
	/// @since 1.8 </param>
	internal interface Node<T>
	{

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> describing the elements contained in this
		/// {@code Node}.
		/// </summary>
		/// <returns> a {@code Spliterator} describing the elements contained in this
		///         {@code Node} </returns>
		Spliterator<T> Node_Fields.spliterator();

		/// <summary>
		/// Traverses the elements of this node, and invoke the provided
		/// {@code Consumer} with each element.  Elements are provided in encounter
		/// order if the source for the {@code Node} has a defined encounter order.
		/// </summary>
		/// <param name="consumer"> a {@code Consumer} that is to be invoked with each
		///        element in this {@code Node} </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: void forEach(java.util.function.Consumer<? base T> consumer);
		void forEach<T1>(Consumer<T1> consumer);

		/// <summary>
		/// Returns the number of child nodes of this node.
		/// 
		/// @implSpec The default implementation returns zero.
		/// </summary>
		/// <returns> the number of child nodes </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default int getChildCount()
	//	{
	//	}

		/// <summary>
		/// Retrieves the child {@code Node} at a given index.
		/// 
		/// @implSpec The default implementation always throws
		/// {@code IndexOutOfBoundsException}.
		/// </summary>
		/// <param name="i"> the index to the child node </param>
		/// <returns> the child node </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the index is less than 0 or greater
		///         than or equal to the number of child nodes </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node<T> getChild(int i)
	//	{
	//		throw new IndexOutOfBoundsException();
	//	}

		/// <summary>
		/// Return a node describing a subsequence of the elements of this node,
		/// starting at the given inclusive start offset and ending at the given
		/// exclusive end offset.
		/// </summary>
		/// <param name="from"> The (inclusive) starting offset of elements to include, must
		///             be in range 0..count(). </param>
		/// <param name="to"> The (exclusive) end offset of elements to include, must be
		///           in range 0..count(). </param>
		/// <param name="generator"> A function to be used to create a new array, if needed,
		///                  for reference nodes. </param>
		/// <returns> the truncated node </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node<T> truncate(long from, long to, java.util.function.IntFunction<T[]> generator)
	//	{
	//		if (from == 0 && to == count())
	//		nodeBuilder.begin(size);
	//		for (int i = 0; i < from && spliterator.tryAdvance(e -> { }); i++)
	//		{
	//		}
	//		for (int i = 0; (i < size) && spliterator.tryAdvance(nodeBuilder); i++)
	//		{
	//		}
	//		nodeBuilder.end();
	//		return nodeBuilder.build();
	//	}

		/// <summary>
		/// Provides an array view of the contents of this node.
		/// 
		/// <para>Depending on the underlying implementation, this may return a
		/// reference to an internal array rather than a copy.  Since the returned
		/// array may be shared, the returned array should not be modified.  The
		/// {@code generator} function may be consulted to create the array if a new
		/// array needs to be created.
		/// 
		/// </para>
		/// </summary>
		/// <param name="generator"> a factory function which takes an integer parameter and
		///        returns a new, empty array of that size and of the appropriate
		///        array type </param>
		/// <returns> an array containing the contents of this {@code Node} </returns>
		T[] AsArray(IntFunction<T[]> generator);

		/// <summary>
		/// Copies the content of this {@code Node} into an array, starting at a
		/// given offset into the array.  It is the caller's responsibility to ensure
		/// there is sufficient room in the array, otherwise unspecified behaviour
		/// will occur if the array length is less than the number of elements
		/// contained in this node.
		/// </summary>
		/// <param name="array"> the array into which to copy the contents of this
		///       {@code Node} </param>
		/// <param name="offset"> the starting offset within the array </param>
		/// <exception cref="IndexOutOfBoundsException"> if copying would cause access of data
		///         outside array bounds </exception>
		/// <exception cref="NullPointerException"> if {@code array} is {@code null} </exception>
		void CopyInto(T[] array, int offset);

		/// <summary>
		/// Gets the {@code StreamShape} associated with this {@code Node}.
		/// 
		/// @implSpec The default in {@code Node} returns
		/// {@code StreamShape.REFERENCE}
		/// </summary>
		/// <returns> the stream shape associated with this node </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default StreamShape getShape()
	//	{
	//	}

		/// <summary>
		/// Returns the number of elements contained in this node.
		/// </summary>
		/// <returns> the number of elements contained in this node </returns>
		long Count();

		/// <summary>
		/// A mutable builder for a {@code Node} that implements <seealso cref="Sink"/>, which
		/// builds a flat node containing the elements that have been pushed to it.
		/// </summary>

		/// <summary>
		/// Specialized {@code Node} for int elements
		/// </summary>

		/// <summary>
		/// Specialized {@code Node} for long elements
		/// </summary>

		/// <summary>
		/// Specialized {@code Node} for double elements
		/// </summary>
	}

	public static class Node_Fields
	{
			public static readonly return 0;
				public static readonly return this;
			public static readonly Spliterator<T> Spliterator = spliterator();
			public static readonly long Size = to - from;
			public static readonly Node_Builder<T> NodeBuilder = Nodes.Builder(Size, generator);
			public static readonly return StreamShape;
	}

	internal interface Node_Builder<T> : Sink<T>
	{

		/// <summary>
		/// Builds the node.  Should be called after all elements have been
		/// pushed and signalled with an invocation of <seealso cref="Sink#end()"/>.
		/// </summary>
		/// <returns> the resulting {@code Node} </returns>
		Node<T> Build();

		/// <summary>
		/// Specialized @{code Node.Builder} for int elements
		/// </summary>

		/// <summary>
		/// Specialized @{code Node.Builder} for long elements
		/// </summary>

		/// <summary>
		/// Specialized @{code Node.Builder} for double elements
		/// </summary>
	}

	internal interface Node_Builder_OfInt : Node_Builder<Integer>, Sink_OfInt
	{
		Node_OfInt Build();
	}

	internal interface Node_Builder_OfLong : Node_Builder<Long>, Sink_OfLong
	{
		Node_OfLong Build();
	}

	internal interface Node_Builder_OfDouble : Node_Builder<Double>, Sink_OfDouble
	{
		Node_OfDouble Build();
	}

	public interface Node_OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE> : Node<T> where T_SPLITR : java.util.Spliterator_OfPrimitive<T, T_CONS, T_SPLITR> where T_NODE : Node_OfPrimitive<T, T_CONS, T_ARR, T_SPLITR, T_NODE>
	{

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> a <seealso cref="Spliterator.OfPrimitive"/> describing the elements of
		///         this node </returns>
		T_SPLITR Node_Fields.spliterator();

		/// <summary>
		/// Traverses the elements of this node, and invoke the provided
		/// {@code action} with each element.
		/// </summary>
		/// <param name="action"> a consumer that is to be invoked with each
		///        element in this {@code Node.OfPrimitive} </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("overloads") void forEach(T_CONS action);
		void ForEach(T_CONS action);

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default T_NODE getChild(int i)
	//	{
	//		throw new IndexOutOfBoundsException();
	//	}

		T_NODE Truncate(long from, long to, IntFunction<T[]> generator);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec the default implementation invokes the generator to create
		/// an instance of a boxed primitive array with a length of
		/// <seealso cref="#count()"/> and then invokes <seealso cref="#copyInto(T[], int)"/> with
		/// that array at an offset of 0.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default T[] asArray(java.util.function.IntFunction<T[]> generator)
	//	{
	//		if (java.util.stream.Tripwire.ENABLED)
	//			java.util.stream.Tripwire.trip(getClass(), "{0} calling Node.OfPrimitive.asArray");
	//		if (Node_Fields.size >= Nodes.MAX_ARRAY_SIZE)
	//			throw new IllegalArgumentException(Nodes.BAD_SIZE);
	//		copyInto(boxed, Node_Fields.0);
	//	}

		/// <summary>
		/// Views this node as a primitive array.
		/// 
		/// <para>Depending on the underlying implementation this may return a
		/// reference to an internal array rather than a copy.  It is the callers
		/// responsibility to decide if either this node or the array is utilized
		/// as the primary reference for the data.</para>
		/// </summary>
		/// <returns> an array containing the contents of this {@code Node} </returns>
		T_ARR AsPrimitiveArray();

		/// <summary>
		/// Creates a new primitive array.
		/// </summary>
		/// <param name="count"> the length of the primitive array. </param>
		/// <returns> the new primitive array. </returns>
		T_ARR NewArray(int count);

		/// <summary>
		/// Copies the content of this {@code Node} into a primitive array,
		/// starting at a given offset into the array.  It is the caller's
		/// responsibility to ensure there is sufficient room in the array.
		/// </summary>
		/// <param name="array"> the array into which to copy the contents of this
		///              {@code Node} </param>
		/// <param name="offset"> the starting offset within the array </param>
		/// <exception cref="IndexOutOfBoundsException"> if copying would cause access of
		///         data outside array bounds </exception>
		/// <exception cref="NullPointerException"> if {@code array} is {@code null} </exception>
		void CopyInto(T_ARR array, int offset);
	}

	public static class Node_OfPrimitive_Fields
	{
			public const long Node_Fields;
			public static readonly T[] Boxed = generator.apply((int) count());
			public static readonly return Boxed;
	}

	internal interface Node_OfInt : Node_OfPrimitive<Integer, IntConsumer, int[], java.util.Spliterator_OfInt, Node_OfInt>
	{

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="consumer"> a {@code Consumer} that is to be invoked with each
		///        element in this {@code Node}.  If this is an
		///        {@code IntConsumer}, it is cast to {@code IntConsumer} so the
		///        elements may be processed without boxing. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEach(java.util.function.Consumer<? base Integer> consumer)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEach(java.util.function.Consumer<JavaToDotNetGenericWildcard> consumer)
	//	{
	//		if (consumer instanceof IntConsumer)
	//		{
	//			forEach((IntConsumer) consumer);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Node.OfInt.forEachRemaining(Consumer)");
	//			Node_Fields.spliterator().forEachRemaining(consumer);
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/> to
		/// obtain an int[] array then and copies the elements from that int[]
		/// array into the boxed Integer[] array.  This is not efficient and it
		/// is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void copyInto(Integer[] Node_OfInt_Fields.boxed, Node_OfInt_Fields.int offset)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Node.OfInt.copyInto(Integer[], int)");
	//		for (int i = Node_Fields.0; i < array.length; i++)
	//		{
	//		}
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfInt truncate(long from, long to, java.util.function.IntFunction<Integer[]> generator)
	//	{
	//		if (from == Node_Fields.0 && to == count())
	//		Node_Fields.nodeBuilder.begin(Node_Fields.size);
	//		for (int i = Node_Fields.0; i < from && Node_Fields.spliterator.tryAdvance((IntConsumer) e -> { }); i++)
	//		{
	//		}
	//		for (int i = Node_Fields.0; (i < Node_Fields.size) && Node_Fields.spliterator.tryAdvance((IntConsumer) Node_Fields.nodeBuilder); i++)
	//		{
	//		}
	//		Node_Fields.nodeBuilder.end();
	//		return Node_Fields.nodeBuilder.build();
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfInt_Fields.int[] newArray(Node_OfInt_Fields.int count)
	//	{
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec The default in {@code Node.OfInt} returns
		/// {@code StreamShape.INT_VALUE}
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default StreamShape getShape()
	//	{
	//	}
	}

	public static class Node_OfInt_Fields
	{
			public static readonly int[] Array = asPrimitiveArray();
				public const boxed[offset + i] = Array[i];
				public static readonly return Node_Fields;
			public const long Node_Fields;
			public const java.util.Spliterator_OfInt Node_Fields;
			public const Node_Builder_OfInt Node_Fields;
			public static readonly return New;
			public static readonly return StreamShape;
	}

	internal interface Node_OfLong : Node_OfPrimitive<Long, LongConsumer, long[], java.util.Spliterator_OfLong, Node_OfLong>
	{

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="consumer"> A {@code Consumer} that is to be invoked with each
		///        element in this {@code Node}.  If this is an
		///        {@code LongConsumer}, it is cast to {@code LongConsumer} so
		///        the elements may be processed without boxing. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEach(java.util.function.Consumer<? base Long> consumer)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEach(java.util.function.Consumer<JavaToDotNetGenericWildcard> consumer)
	//	{
	//		if (consumer instanceof LongConsumer)
	//		{
	//			forEach((LongConsumer) consumer);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Node.OfLong.forEachRemaining(Consumer)");
	//			Node_Fields.spliterator().forEachRemaining(consumer);
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/>
		/// to obtain a long[] array then and copies the elements from that
		/// long[] array into the boxed Long[] array.  This is not efficient and
		/// it is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void copyInto(Long[] Node_OfLong_Fields.boxed, int offset)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Node.OfInt.copyInto(Long[], int)");
	//		for (int i = Node_Fields.0; i < array.length; i++)
	//		{
	//		}
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfLong truncate(Node_OfLong_Fields.long from, Node_OfLong_Fields.long to, java.util.function.IntFunction<Long[]> generator)
	//	{
	//		if (from == Node_Fields.0 && to == count())
	//		Node_Fields.nodeBuilder.begin(Node_Fields.size);
	//		for (int i = Node_Fields.0; i < from && Node_Fields.spliterator.tryAdvance((LongConsumer) e -> { }); i++)
	//		{
	//		}
	//		for (int i = Node_Fields.0; (i < Node_Fields.size) && Node_Fields.spliterator.tryAdvance((LongConsumer) Node_Fields.nodeBuilder); i++)
	//		{
	//		}
	//		Node_Fields.nodeBuilder.end();
	//		return Node_Fields.nodeBuilder.build();
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfLong_Fields.long[] newArray(int count)
	//	{
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// @implSpec The default in {@code Node.OfLong} returns
		/// {@code StreamShape.LONG_VALUE}
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default StreamShape getShape()
	//	{
	//	}
	}

	public static class Node_OfLong_Fields
	{
			public static readonly long[] Array = asPrimitiveArray();
				public const boxed[offset + i] = Array[i];
				public static readonly return Node_Fields;
			public const long Node_Fields;
			public const java.util.Spliterator_OfLong Node_Fields;
			public const Node_Builder_OfLong Node_Fields;
			public static readonly return New;
			public static readonly return StreamShape;
	}

	internal interface Node_OfDouble : Node_OfPrimitive<Double, DoubleConsumer, double[], java.util.Spliterator_OfDouble, Node_OfDouble>
	{

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="consumer"> A {@code Consumer} that is to be invoked with each
		///        element in this {@code Node}.  If this is an
		///        {@code DoubleConsumer}, it is cast to {@code DoubleConsumer}
		///        so the elements may be processed without boxing. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEach(java.util.function.Consumer<? base Double> consumer)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEach(java.util.function.Consumer<JavaToDotNetGenericWildcard> consumer)
	//	{
	//		if (consumer instanceof DoubleConsumer)
	//		{
	//			forEach((DoubleConsumer) consumer);
	//		}
	//		else
	//		{
	//			if (Tripwire.ENABLED)
	//				Tripwire.trip(getClass(), "{0} calling Node.OfLong.forEachRemaining(Consumer)");
	//			Node_Fields.spliterator().forEachRemaining(consumer);
	//		}
	//	}

		//

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec the default implementation invokes <seealso cref="#asPrimitiveArray()"/>
		/// to obtain a double[] array then and copies the elements from that
		/// double[] array into the boxed Double[] array.  This is not efficient
		/// and it is recommended to invoke <seealso cref="#copyInto(Object, int)"/>.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void copyInto(Double[] Node_OfDouble_Fields.boxed, int offset)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} calling Node.OfDouble.copyInto(Double[], int)");
	//		for (int i = Node_Fields.0; i < array.length; i++)
	//		{
	//		}
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfDouble truncate(long from, long to, java.util.function.IntFunction<Double[]> generator)
	//	{
	//		if (from == Node_Fields.0 && to == count())
	//		Node_Fields.nodeBuilder.begin(Node_Fields.size);
	//		for (int i = Node_Fields.0; i < from && Node_Fields.spliterator.tryAdvance((DoubleConsumer) e -> { }); i++)
	//		{
	//		}
	//		for (int i = Node_Fields.0; (i < Node_Fields.size) && Node_Fields.spliterator.tryAdvance((DoubleConsumer) Node_Fields.nodeBuilder); i++)
	//		{
	//		}
	//		Node_Fields.nodeBuilder.end();
	//		return Node_Fields.nodeBuilder.build();
	//	}

//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Node_OfDouble_Fields.double[] newArray(int count)
	//	{
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec The default in {@code Node.OfDouble} returns
		/// {@code StreamShape.DOUBLE_VALUE}
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default StreamShape getShape()
	//	{
	//	}
	}

	public static class Node_OfDouble_Fields
	{
			public static readonly double[] Array = asPrimitiveArray();
				public const boxed[offset + i] = Array[i];
				public static readonly return Node_Fields;
			public const long Node_Fields;
			public const java.util.Spliterator_OfDouble Node_Fields;
			public const Node_Builder_OfDouble Node_Fields;
			public static readonly return New;
			public static readonly return StreamShape;
	}

}