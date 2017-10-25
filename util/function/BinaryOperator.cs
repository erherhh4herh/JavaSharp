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
	/// Represents an operation upon two operands of the same type, producing a result
	/// of the same type as the operands.  This is a specialization of
	/// <seealso cref="BiFunction"/> for the case where the operands and the result are all of
	/// the same type.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#apply(Object, Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the operands and result of the operator
	/// </param>
	/// <seealso cref= BiFunction </seealso>
	/// <seealso cref= UnaryOperator
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface BinaryOperator<T> extends BiFunction<T,T,T>
	public interface BinaryOperator<T> : BiFunction<T, T, T>
	{
		/// <summary>
		/// Returns a <seealso cref="BinaryOperator"/> which returns the lesser of two elements
		/// according to the specified {@code Comparator}.
		/// </summary>
		/// @param <T> the type of the input arguments of the comparator </param>
		/// <param name="comparator"> a {@code Comparator} for comparing the two values </param>
		/// <returns> a {@code BinaryOperator} which returns the lesser of its operands,
		///         according to the supplied {@code Comparator} </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> BinaryOperator<T> minBy(java.util.Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> BinaryOperator<T> minBy(java.util.Comparator<JavaToDotNetGenericWildcard> comparator)
	//	{
	//		Objects.requireNonNull(comparator);
	//		return (a, b) -> comparator.compare(a, b) <= 0 ? a : b;
	//	}

		/// <summary>
		/// Returns a <seealso cref="BinaryOperator"/> which returns the greater of two elements
		/// according to the specified {@code Comparator}.
		/// </summary>
		/// @param <T> the type of the input arguments of the comparator </param>
		/// <param name="comparator"> a {@code Comparator} for comparing the two values </param>
		/// <returns> a {@code BinaryOperator} which returns the greater of its operands,
		///         according to the supplied {@code Comparator} </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> BinaryOperator<T> maxBy(java.util.Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> BinaryOperator<T> maxBy(java.util.Comparator<JavaToDotNetGenericWildcard> comparator)
	//	{
	//		Objects.requireNonNull(comparator);
	//		return (a, b) -> comparator.compare(a, b) >= 0 ? a : b;
	//	}
	}

}