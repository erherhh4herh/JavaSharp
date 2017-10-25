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
	/// Represents a predicate (boolean-valued function) of one argument.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#test(Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the input to the predicate
	/// 
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Predicate<T>
	public interface Predicate<T>
	{

		/// <summary>
		/// Evaluates this predicate on the given argument.
		/// </summary>
		/// <param name="t"> the input argument </param>
		/// <returns> {@code true} if the input argument matches the predicate,
		/// otherwise {@code false} </returns>
		bool Test(T t);

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
//ORIGINAL LINE: default Predicate<T> and(Predicate<? base T> other)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Predicate<T> and(Predicate<JavaToDotNetGenericWildcard> other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (t) -> test(t) && other.test(t);
	//	}

		/// <summary>
		/// Returns a predicate that represents the logical negation of this
		/// predicate.
		/// </summary>
		/// <returns> a predicate that represents the logical negation of this
		/// predicate </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Predicate<T> negate()
	//	{
	//		return (t) -> !test(t);
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
//ORIGINAL LINE: default Predicate<T> or(Predicate<? base T> other)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Predicate<T> or(Predicate<JavaToDotNetGenericWildcard> other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (t) -> test(t) || other.test(t);
	//	}

		/// <summary>
		/// Returns a predicate that tests if two arguments are equal according
		/// to <seealso cref="Objects#equals(Object, Object)"/>.
		/// </summary>
		/// @param <T> the type of arguments to the predicate </param>
		/// <param name="targetRef"> the object reference with which to compare for equality,
		///               which may be {@code null} </param>
		/// <returns> a predicate that tests if two arguments are equal according
		/// to <seealso cref="Objects#equals(Object, Object)"/> </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static <T> Predicate<T> isEqual(Object targetRef)
	//	{
	//		return (null == targetRef) ? Objects::isNull : @object -> targetRef.equals(@object);
	//	}
	}

}