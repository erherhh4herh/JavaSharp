using System;

/*
 * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// {@code GenericArrayType} represents an array type whose component
	/// type is either a parameterized type or a type variable.
	/// @since 1.5
	/// </summary>
	public interface GenericArrayType : Type
	{
		/// <summary>
		/// Returns a {@code Type} object representing the component type
		/// of this array. This method creates the component type of the
		/// array.  See the declaration of {@link
		/// java.lang.reflect.ParameterizedType ParameterizedType} for the
		/// semantics of the creation process for parameterized types and
		/// see <seealso cref="java.lang.reflect.TypeVariable TypeVariable"/> for the
		/// creation process for type variables.
		/// </summary>
		/// <returns>  a {@code Type} object representing the component type
		///     of this array </returns>
		/// <exception cref="TypeNotPresentException"> if the underlying array type's
		///     component type refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if  the
		///     underlying array type's component type refers to a
		///     parameterized type that cannot be instantiated for any reason </exception>
		Type GenericComponentType {get;}
	}

}