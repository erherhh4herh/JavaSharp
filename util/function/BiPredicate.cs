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
	/// Represents a predicate (boolean-valued function) of two arguments.  This is
	/// the two-arity specialization of <seealso cref="Predicate"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#test(Object, Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the first argument to the predicate </param>
	/// @param <U> the type of the second argument the predicate
	/// </param>
	/// <seealso cref= Predicate
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface BiPredicate<T, U>
	public interface BiPredicate<T, U>
	{

		/// <summary>
		/// Evaluates this predicate on the given arguments.
		/// </summary>
		/// <param name="t"> the first input argument </param>
		/// <param name="u"> the second input argument </param>
		/// <returns> {@code true} if the input arguments match the predicate,
		/// otherwise {@code false} </returns>
		bool Test(T t, U u);

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
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default BiPredicate<T, U> and(BiPredicate<? base T, ? base U> other)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default BiPredicate<T, U> and(BiPredicate<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (T t, U u) -> test(t, u) && other.test(t, u);
	//	}

		/// <summary>
		/// Returns a predicate that represents the logical negation of this
		/// predicate.
		/// </summary>
		/// <returns> a predicate that represents the logical negation of this
		/// predicate </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default BiPredicate<T, U> negate()
	//	{
	//		return (T t, U u) -> !test(t, u);
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
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default BiPredicate<T, U> or(BiPredicate<? base T, ? base U> other)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default BiPredicate<T, U> or(BiPredicate<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (T t, U u) -> test(t, u) || other.test(t, u);
	//	}
	}

}