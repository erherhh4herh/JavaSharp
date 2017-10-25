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
	/// Represents a function that accepts one argument and produces a result.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#apply(Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the input to the function </param>
	/// @param <R> the type of the result of the function
	/// 
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Function<T, R>
	public interface Function<T, R>
	{

		/// <summary>
		/// Applies this function to the given argument.
		/// </summary>
		/// <param name="t"> the function argument </param>
		/// <returns> the function result </returns>
		R Apply(T Function_Fields);

		/// <summary>
		/// Returns a composed function that first applies the {@code before}
		/// function to its input, and then applies this function to the result.
		/// If evaluation of either function throws an exception, it is relayed to
		/// the caller of the composed function.
		/// </summary>
		/// @param <V> the type of input to the {@code before} function, and to the
		///           composed function </param>
		/// <param name="before"> the function to apply before this function is applied </param>
		/// <returns> a composed function that first applies the {@code before}
		/// function and then applies this function </returns>
		/// <exception cref="NullPointerException"> if before is null
		/// </exception>
		/// <seealso cref= #andThen(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> Function<V, R> compose(Function<? base V, ? extends T> before)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> Function<V, R> compose(Function<? base V, ? extends T> before)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default <V> Function<V, R> compose(Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> before) where ? : T
	//	{
	//		Objects.requireNonNull(before);
	//		return (V v) -> apply(before.apply(v));
	//	}

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
		/// <exception cref="NullPointerException"> if after is null
		/// </exception>
		/// <seealso cref= #compose(Function) </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> Function<T, V> andThen(Function<? base R, ? extends V> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <V> Function<T, V> andThen(Function<? base R, ? extends V> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default <V> Function<T, V> andThen(Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> after) where ? : V
	//	{
	//		Objects.requireNonNull(after);
	//		return (T t) -> after.apply(apply(t));
	//	}

		/// <summary>
		/// Returns a function that always returns its input argument.
		/// </summary>
		/// @param <T> the type of the input and output objects to the function </param>
		/// <returns> a function that always returns its input argument </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		static <T> Function<T, T> identity()
	//	{
	//	}
	}

	public static class Function_Fields
	{
			public static readonly return t;
	}

}