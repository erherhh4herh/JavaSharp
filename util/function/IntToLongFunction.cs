﻿/*
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
	/// Represents a function that accepts an int-valued argument and produces a
	/// long-valued result.  This is the {@code int}-to-{@code long} primitive
	/// specialization for <seealso cref="Function"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsLong(int)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Function
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface IntToLongFunction
	public interface IntToLongFunction
	{

		/// <summary>
		/// Applies this function to the given argument.
		/// </summary>
		/// <param name="value"> the function argument </param>
		/// <returns> the function result </returns>
		long ApplyAsLong(int value);
	}

}