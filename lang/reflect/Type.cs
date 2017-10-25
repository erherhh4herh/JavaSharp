using System;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	/// <summary>
	/// Type is the common superinterface for all types in the Java
	/// programming language. These include raw types, parameterized types,
	/// array types, type variables and primitive types.
	/// 
	/// @since 1.5
	/// </summary>
	public interface Type
	{
		/// <summary>
		/// Returns a string describing this type, including information
		/// about any type parameters.
		/// 
		/// @implSpec The default implementation calls {@code toString}.
		/// </summary>
		/// <returns> a string describing this type
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default String getTypeName()
	//	{
	//		return toString();
	//	}
	}

}