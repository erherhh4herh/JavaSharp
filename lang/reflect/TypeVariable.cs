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
	/// TypeVariable is the common superinterface for type variables of kinds.
	/// A type variable is created the first time it is needed by a reflective
	/// method, as specified in this package.  If a type variable t is referenced
	/// by a type (i.e, class, interface or annotation type) T, and T is declared
	/// by the nth enclosing class of T (see JLS 8.1.2), then the creation of t
	/// requires the resolution (see JVMS 5) of the ith enclosing class of T,
	/// for i = 0 to n, inclusive. Creating a type variable must not cause the
	/// creation of its bounds. Repeated creation of a type variable has no effect.
	/// 
	/// <para>Multiple objects may be instantiated at run-time to
	/// represent a given type variable. Even though a type variable is
	/// created only once, this does not imply any requirement to cache
	/// instances representing the type variable. However, all instances
	/// representing a type variable must be equal() to each other.
	/// As a consequence, users of type variables must not rely on the identity
	/// of instances of classes implementing this interface.
	/// 
	/// </para>
	/// </summary>
	/// @param <D> the type of generic declaration that declared the
	/// underlying type variable.
	/// 
	/// @since 1.5 </param>
	public interface TypeVariable<D> : Type, AnnotatedElement where D : GenericDeclaration
	{
		/// <summary>
		/// Returns an array of {@code Type} objects representing the
		/// upper bound(s) of this type variable.  Note that if no upper bound is
		/// explicitly declared, the upper bound is {@code Object}.
		/// 
		/// <para>For each upper bound B: <ul> <li>if B is a parameterized
		/// type or a type variable, it is created, (see {@link
		/// java.lang.reflect.ParameterizedType ParameterizedType} for the
		/// details of the creation process for parameterized types).
		/// <li>Otherwise, B is resolved.  </ul>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="TypeNotPresentException">  if any of the
		///     bounds refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if any of the
		///     bounds refer to a parameterized type that cannot be instantiated
		///     for any reason </exception>
		/// <returns> an array of {@code Type}s representing the upper
		///     bound(s) of this type variable </returns>
		Type[] Bounds {get;}

		/// <summary>
		/// Returns the {@code GenericDeclaration} object representing the
		/// generic declaration declared this type variable.
		/// </summary>
		/// <returns> the generic declaration declared for this type variable.
		/// 
		/// @since 1.5 </returns>
		D GenericDeclaration {get;}

		/// <summary>
		/// Returns the name of this type variable, as it occurs in the source code.
		/// </summary>
		/// <returns> the name of this type variable, as it appears in the source code </returns>
		String Name {get;}

		/// <summary>
		/// Returns an array of AnnotatedType objects that represent the use of
		/// types to denote the upper bounds of the type parameter represented by
		/// this TypeVariable. The order of the objects in the array corresponds to
		/// the order of the bounds in the declaration of the type parameter.
		/// 
		/// Returns an array of length 0 if the type parameter declares no bounds.
		/// </summary>
		/// <returns> an array of objects representing the upper bounds of the type variable
		/// @since 1.8 </returns>
		 AnnotatedType[] AnnotatedBounds {get;}
	}

}