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
namespace java.util.function
{

	/// <summary>
	/// Represents an operation on a single {@code int}-valued operand that produces
	/// an {@code int}-valued result.  This is the primitive type specialization of
	/// <seealso cref="UnaryOperator"/> for {@code int}.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsInt(int)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= UnaryOperator
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface IntUnaryOperator
	public interface IntUnaryOperator
	{

		/// <summary>
		/// Applies this operator to the given operand.
		/// </summary>
		/// <param name="operand"> the operand </param>
		/// <returns> the operator result </returns>
		int ApplyAsInt(int operand);

		/// <summary>
		/// Returns a composed operator that first applies the {@code before}
		/// operator to its input, and then applies this operator to the result.
		/// If evaluation of either operator throws an exception, it is relayed to
		/// the caller of the composed operator.
		/// </summary>
		/// <param name="before"> the operator to apply before this operator is applied </param>
		/// <returns> a composed operator that first applies the {@code before}
		/// operator and then applies this operator </returns>
		/// <exception cref="NullPointerException"> if before is null
		/// </exception>
		/// <seealso cref= #andThen(IntUnaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default IntUnaryOperator compose(IntUnaryOperator before)
	//	{
	//		Objects.requireNonNull(before);
	//		return (int v) -> applyAsInt(before.applyAsInt(v));
	//	}

		/// <summary>
		/// Returns a composed operator that first applies this operator to
		/// its input, and then applies the {@code after} operator to the result.
		/// If evaluation of either operator throws an exception, it is relayed to
		/// the caller of the composed operator.
		/// </summary>
		/// <param name="after"> the operator to apply after this operator is applied </param>
		/// <returns> a composed operator that first applies this operator and then
		/// applies the {@code after} operator </returns>
		/// <exception cref="NullPointerException"> if after is null
		/// </exception>
		/// <seealso cref= #compose(IntUnaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default IntUnaryOperator andThen(IntUnaryOperator after)
	//	{
	//		Objects.requireNonNull(after);
	//		return (int t) -> after.applyAsInt(applyAsInt(t));
	//	}

		/// <summary>
		/// Returns a unary operator that always returns its input argument.
		/// </summary>
		/// <returns> a unary operator that always returns its input argument </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static IntUnaryOperator identity()
	//	{
	//	}
	}

	public static class IntUnaryOperator_Fields
	{
			public static readonly return t;
	}

}