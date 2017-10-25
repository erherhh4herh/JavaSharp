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
	/// ParameterizedType represents a parameterized type such as
	/// Collection&lt;String&gt;.
	/// 
	/// <para>A parameterized type is created the first time it is needed by a
	/// reflective method, as specified in this package. When a
	/// parameterized type p is created, the generic type declaration that
	/// p instantiates is resolved, and all type arguments of p are created
	/// recursively. See {@link java.lang.reflect.TypeVariable
	/// TypeVariable} for details on the creation process for type
	/// variables. Repeated creation of a parameterized type has no effect.
	/// 
	/// </para>
	/// <para>Instances of classes that implement this interface must implement
	/// an equals() method that equates any two instances that share the
	/// same generic type declaration and have equal type parameters.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public interface ParameterizedType : Type
	{
		/// <summary>
		/// Returns an array of {@code Type} objects representing the actual type
		/// arguments to this type.
		/// 
		/// <para>Note that in some cases, the returned array be empty. This can occur
		/// if this type represents a non-parameterized type nested within
		/// a parameterized type.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of {@code Type} objects representing the actual type
		///     arguments to this type </returns>
		/// <exception cref="TypeNotPresentException"> if any of the
		///     actual type arguments refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if any of the
		///     actual type parameters refer to a parameterized type that cannot
		///     be instantiated for any reason
		/// @since 1.5 </exception>
		Type[] ActualTypeArguments {get;}

		/// <summary>
		/// Returns the {@code Type} object representing the class or interface
		/// that declared this type.
		/// </summary>
		/// <returns> the {@code Type} object representing the class or interface
		///     that declared this type
		/// @since 1.5 </returns>
		Type RawType {get;}

		/// <summary>
		/// Returns a {@code Type} object representing the type that this type
		/// is a member of.  For example, if this type is {@code O<T>.I<S>},
		/// return a representation of {@code O<T>}.
		/// 
		/// <para>If this type is a top-level type, {@code null} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Type} object representing the type that
		///     this type is a member of. If this type is a top-level type,
		///     {@code null} is returned </returns>
		/// <exception cref="TypeNotPresentException"> if the owner type
		///     refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if the owner type
		///     refers to a parameterized type that cannot be instantiated
		///     for any reason
		/// @since 1.5 </exception>
		Type OwnerType {get;}
	}

}