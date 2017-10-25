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
	/// An operation in a stream pipeline that takes a stream as input and produces
	/// a result or side-effect.  A {@code TerminalOp} has an input type and stream
	/// shape, and a result type.  A {@code TerminalOp} also has a set of
	/// <em>operation flags</em> that describes how the operation processes elements
	/// of the stream (such as short-circuiting or respecting encounter order; see
	/// <seealso cref="StreamOpFlag"/>).
	/// 
	/// <para>A {@code TerminalOp} must provide a sequential and parallel implementation
	/// of the operation relative to a given stream source and set of intermediate
	/// operations.
	/// 
	/// </para>
	/// </summary>
	/// @param <E_IN> the type of input elements </param>
	/// @param <R>    the type of the result
	/// @since 1.8 </param>
	internal interface TerminalOp<E_IN, R>
	{
		/// <summary>
		/// Gets the shape of the input type of this operation.
		/// 
		/// @implSpec The default returns {@code StreamShape.REFERENCE}.
		/// </summary>
		/// <returns> StreamShape of the input type of this operation </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default StreamShape inputShape()
	//	{
	//	}

		/// <summary>
		/// Gets the stream flags of the operation.  Terminal operations may set a
		/// limited subset of the stream flags defined in <seealso cref="StreamOpFlag"/>, and
		/// these flags are combined with the previously combined stream and
		/// intermediate operation flags for the pipeline.
		/// 
		/// @implSpec The default implementation returns zero.
		/// </summary>
		/// <returns> the stream flags for this operation </returns>
		/// <seealso cref= StreamOpFlag </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default int getOpFlags()
	//	{
	//	}

		/// <summary>
		/// Performs a parallel evaluation of the operation using the specified
		/// {@code PipelineHelper}, which describes the upstream intermediate
		/// operations.
		/// 
		/// @implSpec The default performs a sequential evaluation of the operation
		/// using the specified {@code PipelineHelper}.
		/// </summary>
		/// <param name="helper"> the pipeline helper </param>
		/// <param name="spliterator"> the source spliterator </param>
		/// <returns> the result of the evaluation </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default <P_IN> R evaluateParallel(PipelineHelper<E_IN> helper, java.util.Spliterator<P_IN> spliterator)
	//	{
	//		if (Tripwire.ENABLED)
	//			Tripwire.trip(getClass(), "{0} triggering TerminalOp.evaluateParallel serial default");
	//		return evaluateSequential(helper, spliterator);
	//	}

		/// <summary>
		/// Performs a sequential evaluation of the operation using the specified
		/// {@code PipelineHelper}, which describes the upstream intermediate
		/// operations.
		/// </summary>
		/// <param name="helper"> the pipeline helper </param>
		/// <param name="spliterator"> the source spliterator </param>
		/// <returns> the result of the evaluation </returns>
		R evaluateSequential<P_IN>(PipelineHelper<E_IN> helper, Spliterator<P_IN> spliterator);
	}

	public static class TerminalOp_Fields
	{
			public static readonly return StreamShape;
			public static readonly return 0;
	}

}