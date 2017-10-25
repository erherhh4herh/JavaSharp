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
	/// Represents a supplier of {@code long}-valued results.  This is the
	/// {@code long}-producing primitive specialization of <seealso cref="Supplier"/>.
	/// 
	/// <para>There is no requirement that a distinct result be returned each
	/// time the supplier is invoked.
	/// 
	/// </para>
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#getAsLong()"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Supplier
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface LongSupplier
	public interface LongSupplier
	{

		/// <summary>
		/// Gets a result.
		/// </summary>
		/// <returns> a result </returns>
		long AsLong {get;}
	}

}