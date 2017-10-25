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

namespace java.lang.reflect
{

	/// <summary>
	/// {@code AnnotatedParameterizedType} represents the potentially annotated use
	/// of a parameterized type, whose type arguments may themselves represent
	/// annotated uses of types.
	/// 
	/// @since 1.8
	/// </summary>
	public interface AnnotatedParameterizedType : AnnotatedType
	{

		/// <summary>
		/// Returns the potentially annotated actual type arguments of this parameterized type.
		/// </summary>
		/// <returns> the potentially annotated actual type arguments of this parameterized type </returns>
		AnnotatedType[] AnnotatedActualTypeArguments {get;}
	}

}