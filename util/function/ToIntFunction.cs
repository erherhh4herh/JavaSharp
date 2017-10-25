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
	/// Represents a function that produces an int-valued result.  This is the
	/// {@code int}-producing primitive specialization for <seealso cref="Function"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsInt(Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the input to the function
	/// </param>
	/// <seealso cref= Function
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface ToIntFunction<T>
	public interface ToIntFunction<T>
	{

		/// <summary>
		/// Applies this function to the given argument.
		/// </summary>
		/// <param name="value"> the function argument </param>
		/// <returns> the function result </returns>
		int ApplyAsInt(T value);
	}

}