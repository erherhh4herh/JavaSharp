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
	/// Represents an operation on a single {@code long}-valued operand that produces
	/// a {@code long}-valued result.  This is the primitive type specialization of
	/// <seealso cref="UnaryOperator"/> for {@code long}.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsLong(long)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= UnaryOperator
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface LongUnaryOperator
	public interface LongUnaryOperator
	{

		/// <summary>
		/// Applies this operator to the given operand.
		/// </summary>
		/// <param name="operand"> the operand </param>
		/// <returns> the operator result </returns>
		long ApplyAsLong(long operand);

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
		/// <seealso cref= #andThen(LongUnaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default LongUnaryOperator compose(LongUnaryOperator before)
	//	{
	//		Objects.requireNonNull(before);
	//		return (long v) -> applyAsLong(before.applyAsLong(v));
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
		/// <seealso cref= #compose(LongUnaryOperator) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default LongUnaryOperator andThen(LongUnaryOperator after)
	//	{
	//		Objects.requireNonNull(after);
	//		return (long t) -> after.applyAsLong(applyAsLong(t));
	//	}

		/// <summary>
		/// Returns a unary operator that always returns its input argument.
		/// </summary>
		/// <returns> a unary operator that always returns its input argument </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static LongUnaryOperator identity()
	//	{
	//	}
	}

	public static class LongUnaryOperator_Fields
	{
			public static readonly return t;
	}

}