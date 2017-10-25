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
	/// {@code AnnotatedArrayType} represents the potentially annotated use of an
	/// array type, whose component type may itself represent the annotated use of a
	/// type.
	/// 
	/// @since 1.8
	/// </summary>
	public interface AnnotatedArrayType : AnnotatedType
	{

		/// <summary>
		/// Returns the potentially annotated generic component type of this array type.
		/// </summary>
		/// <returns> the potentially annotated generic component type of this array type </returns>
		AnnotatedType AnnotatedGenericComponentType {get;}
	}

}