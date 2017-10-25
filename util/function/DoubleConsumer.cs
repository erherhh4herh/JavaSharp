/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.util.function
{

	/// <summary>
	/// Represents an operation that accepts a single {@code double}-valued argument and
	/// returns no result.  This is the primitive type specialization of
	/// <seealso cref="Consumer"/> for {@code double}.  Unlike most other functional interfaces,
	/// {@code DoubleConsumer} is expected to operate via side-effects.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#accept(double)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Consumer
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface DoubleConsumer
	public interface DoubleConsumer
	{

		/// <summary>
		/// Performs this operation on the given argument.
		/// </summary>
		/// <param name="value"> the input argument </param>
		void Accept(double value);

		/// <summary>
		/// Returns a composed {@code DoubleConsumer} that performs, in sequence, this
		/// operation followed by the {@code after} operation. If performing either
		/// operation throws an exception, it is relayed to the caller of the
		/// composed operation.  If performing this operation throws an exception,
		/// the {@code after} operation will not be performed.
		/// </summary>
		/// <param name="after"> the operation to perform after this operation </param>
		/// <returns> a composed {@code DoubleConsumer} that performs in sequence this
		/// operation followed by the {@code after} operation </returns>
		/// <exception cref="NullPointerException"> if {@code after} is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default DoubleConsumer andThen(DoubleConsumer after)
	//	{
	//		Objects.requireNonNull(after);
	//		return (double t) ->
	//		{
	//			accept(t);
	//			after.accept(t);
	//		};
	//	}
	}

}