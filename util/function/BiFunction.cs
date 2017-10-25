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
	/// Represents a function that accepts two arguments and produces a result.
	/// This is the two-arity specialization of <seealso cref="Function"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#apply(Object, Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the first argument to the function </param>
	/// @param <U> the type of the second argument to the function </param>
	/// @param <R> the type of the result of the function
	/// </param>
	/// <seealso cref= Function
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface BiFunction<T, U, R>
	public interface BiFunction<T, U, R>
	{

		/// <summary>
		/// Applies this function to the given arguments.
		/// </summary>
		/// <param name="t"> the first function argument </param>
		/// <param name="u"> the second function argument </param>
		/// <returns> the function result </returns>
		R Apply(T t, U u);

		/// <summary>
		/// Returns a composed function that first applies this function to
		/// its input, and then applies the {@code after} function to the result.
		/// If evaluation of either function throws an exception, it is relayed to
		/// the caller of the composed function.
		/// </summary>
		/// @param <V> the type of output of the {@code after} function, and of the
		///           composed function </param>
		/// <param name="after"> the function to apply after this function is applied </param>
		/// <returns> a composed function that first applies this function and then
		/// applies the {@code after} function </returns>
		/// <exception cref="NullPointerException"> if after is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> BiFunction<T, U, V> andThen(Function<? base R, ? extends V> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> BiFunction<T, U, V> andThen(Function<? base R, ? extends V> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default <V> BiFunction<T, U, V> andThen(Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> after) where ? : V
	//	{
	//		Objects.requireNonNull(after);
	//		return (T t, U u) -> after.apply(apply(t, u));
	//	}
	}

}