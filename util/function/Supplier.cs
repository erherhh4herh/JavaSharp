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
	/// Represents a supplier of results.
	/// 
	/// <para>There is no requirement that a new or distinct result be returned each
	/// time the supplier is invoked.
	/// 
	/// </para>
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#get()"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of results supplied by this supplier
	/// 
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Supplier<T>
	public interface Supplier<T>
	{

		/// <summary>
		/// Gets a result.
		/// </summary>
		/// <returns> a result </returns>
		T Get();
	}

}