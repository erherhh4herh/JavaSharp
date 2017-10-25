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
	/// Represents an operation on a single operand that produces a result of the
	/// same type as its operand.  This is a specialization of {@code Function} for
	/// the case where the operand and result are of the same type.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#apply(Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the operand and result of the operator
	/// </param>
	/// <seealso cref= Function
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface UnaryOperator<T> extends Function<T, T>
	public interface UnaryOperator<T> : Function<T, T>
	{

		/// <summary>
		/// Returns a unary operator that always returns its input argument.
		/// </summary>
		/// @param <T> the type of the input and output of the operator </param>
		/// <returns> a unary operator that always returns its input argument </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static <T> UnaryOperator<T> identity()
	//	{
	//	}
	}

	public static class UnaryOperator_Fields
	{
			public static readonly return t;
	}

}