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
	/// {@code AnnotatedWildcardType} represents the potentially annotated use of a
	/// wildcard type argument, whose upper or lower bounds may themselves represent
	/// annotated uses of types.
	/// 
	/// @since 1.8
	/// </summary>
	public interface AnnotatedWildcardType : AnnotatedType
	{

		/// <summary>
		/// Returns the potentially annotated lower bounds of this wildcard type.
		/// </summary>
		/// <returns> the potentially annotated lower bounds of this wildcard type </returns>
		AnnotatedType[] AnnotatedLowerBounds {get;}

		/// <summary>
		/// Returns the potentially annotated upper bounds of this wildcard type.
		/// </summary>
		/// <returns> the potentially annotated upper bounds of this wildcard type </returns>
		AnnotatedType[] AnnotatedUpperBounds {get;}
	}

}