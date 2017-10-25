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
	/// Represents a predicate (boolean-valued function) of one {@code double}-valued
	/// argument. This is the {@code double}-consuming primitive type specialization
	/// of <seealso cref="Predicate"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#test(double)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Predicate
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface DoublePredicate
	public interface DoublePredicate
	{

		/// <summary>
		/// Evaluates this predicate on the given argument.
		/// </summary>
		/// <param name="value"> the input argument </param>
		/// <returns> {@code true} if the input argument matches the predicate,
		/// otherwise {@code false} </returns>
		bool Test(double value);

		/// <summary>
		/// Returns a composed predicate that represents a short-circuiting logical
		/// AND of this predicate and another.  When evaluating the composed
		/// predicate, if this predicate is {@code false}, then the {@code other}
		/// predicate is not evaluated.
		/// 
		/// <para>Any exceptions thrown during evaluation of either predicate are relayed
		/// to the caller; if evaluation of this predicate throws an exception, the
		/// {@code other} predicate will not be evaluated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other"> a predicate that will be logically-ANDed with this
		///              predicate </param>
		/// <returns> a composed predicate that represents the short-circuiting logical
		/// AND of this predicate and the {@code other} predicate </returns>
		/// <exception cref="NullPointerException"> if other is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default DoublePredicate and(DoublePredicate other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (value) -> test(value) && other.test(value);
	//	}

		/// <summary>
		/// Returns a predicate that represents the logical negation of this
		/// predicate.
		/// </summary>
		/// <returns> a predicate that represents the logical negation of this
		/// predicate </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default DoublePredicate negate()
	//	{
	//		return (value) -> !test(value);
	//	}

		/// <summary>
		/// Returns a composed predicate that represents a short-circuiting logical
		/// OR of this predicate and another.  When evaluating the composed
		/// predicate, if this predicate is {@code true}, then the {@code other}
		/// predicate is not evaluated.
		/// 
		/// <para>Any exceptions thrown during evaluation of either predicate are relayed
		/// to the caller; if evaluation of this predicate throws an exception, the
		/// {@code other} predicate will not be evaluated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other"> a predicate that will be logically-ORed with this
		///              predicate </param>
		/// <returns> a composed predicate that represents the short-circuiting logical
		/// OR of this predicate and the {@code other} predicate </returns>
		/// <exception cref="NullPointerException"> if other is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default DoublePredicate or(DoublePredicate other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (value) -> test(value) || other.test(value);
	//	}
	}

}