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
	/// Represents a function that accepts two arguments and produces a double-valued
	/// result.  This is the {@code double}-producing primitive specialization for
	/// <seealso cref="BiFunction"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsDouble(Object, Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the first argument to the function </param>
	/// @param <U> the type of the second argument to the function
	/// </param>
	/// <seealso cref= BiFunction
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface ToDoubleBiFunction<T, U>
	public interface ToDoubleBiFunction<T, U>
	{

		/// <summary>
		/// Applies this function to the given arguments.
		/// </summary>
		/// <param name="t"> the first function argument </param>
		/// <param name="u"> the second function argument </param>
		/// <returns> the function result </returns>
		double ApplyAsDouble(T t, U u);
	}

}