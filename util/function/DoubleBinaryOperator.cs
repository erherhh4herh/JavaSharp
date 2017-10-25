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
	/// Represents an operation upon two {@code double}-valued operands and producing a
	/// {@code double}-valued result.   This is the primitive type specialization of
	/// <seealso cref="BinaryOperator"/> for {@code double}.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsDouble(double, double)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= BinaryOperator </seealso>
	/// <seealso cref= DoubleUnaryOperator
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface DoubleBinaryOperator
	public interface DoubleBinaryOperator
	{
		/// <summary>
		/// Applies this operator to the given operands.
		/// </summary>
		/// <param name="left"> the first operand </param>
		/// <param name="right"> the second operand </param>
		/// <returns> the operator result </returns>
		double ApplyAsDouble(double left, double right);
	}

}