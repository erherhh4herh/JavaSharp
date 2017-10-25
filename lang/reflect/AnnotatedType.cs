using System;

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
	/// {@code AnnotatedType} represents the potentially annotated use of a type in
	/// the program currently running in this VM. The use may be of any type in the
	/// Java programming language, including an array type, a parameterized type, a
	/// type variable, or a wildcard type.
	/// 
	/// @since 1.8
	/// </summary>
	public interface AnnotatedType : AnnotatedElement
	{

		/// <summary>
		/// Returns the underlying type that this annotated type represents.
		/// </summary>
		/// <returns> the type this annotated type represents </returns>
		Type Type {get;}
	}

}