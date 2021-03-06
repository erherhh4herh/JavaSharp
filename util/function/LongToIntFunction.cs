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
	/// Represents a function that accepts a long-valued argument and produces an
	/// int-valued result.  This is the {@code long}-to-{@code int} primitive
	/// specialization for <seealso cref="Function"/>.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#applyAsInt(long)"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Function
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface LongToIntFunction
	public interface LongToIntFunction
	{

		/// <summary>
		/// Applies this function to the given argument.
		/// </summary>
		/// <param name="value"> the function argument </param>
		/// <returns> the function result </returns>
		int ApplyAsInt(long value);
	}

}