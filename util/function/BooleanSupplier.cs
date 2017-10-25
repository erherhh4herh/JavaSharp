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
	/// Represents a supplier of {@code boolean}-valued results.  This is the
	/// {@code boolean}-producing primitive specialization of <seealso cref="Supplier"/>.
	/// 
	/// <para>There is no requirement that a new or distinct result be returned each
	/// time the supplier is invoked.
	/// 
	/// </para>
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#getAsBoolean()"/>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Supplier
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface BooleanSupplier
	public interface BooleanSupplier
	{

		/// <summary>
		/// Gets a result.
		/// </summary>
		/// <returns> a result </returns>
		bool AsBoolean {get;}
	}

}