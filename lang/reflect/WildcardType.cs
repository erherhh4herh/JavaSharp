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
	/// WildcardType represents a wildcard type expression, such as
	/// {@code ?}, {@code ? extends Number}, or {@code ? super Integer}.
	/// 
	/// @since 1.5
	/// </summary>
	public interface WildcardType : Type
	{
		/// <summary>
		/// Returns an array of {@code Type} objects representing the  upper
		/// bound(s) of this type variable.  Note that if no upper bound is
		/// explicitly declared, the upper bound is {@code Object}.
		/// 
		/// <para>For each upper bound B :
		/// <ul>
		///  <li>if B is a parameterized type or a type variable, it is created,
		///  (see <seealso cref="java.lang.reflect.ParameterizedType ParameterizedType"/>
		///  for the details of the creation process for parameterized types).
		///  <li>Otherwise, B is resolved.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of Types representing the upper bound(s) of this
		///     type variable </returns>
		/// <exception cref="TypeNotPresentException"> if any of the
		///     bounds refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if any of the
		///     bounds refer to a parameterized type that cannot be instantiated
		///     for any reason </exception>
		Type[] UpperBounds {get;}

		/// <summary>
		/// Returns an array of {@code Type} objects representing the
		/// lower bound(s) of this type variable.  Note that if no lower bound is
		/// explicitly declared, the lower bound is the type of {@code null}.
		/// In this case, a zero length array is returned.
		/// 
		/// <para>For each lower bound B :
		/// <ul>
		///   <li>if B is a parameterized type or a type variable, it is created,
		///  (see <seealso cref="java.lang.reflect.ParameterizedType ParameterizedType"/>
		///  for the details of the creation process for parameterized types).
		///   <li>Otherwise, B is resolved.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of Types representing the lower bound(s) of this
		///     type variable </returns>
		/// <exception cref="TypeNotPresentException"> if any of the
		///     bounds refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if any of the
		///     bounds refer to a parameterized type that cannot be instantiated
		///     for any reason </exception>
		Type[] LowerBounds {get;}
		// one or many? Up to language spec; currently only one, but this API
		// allows for generalization.
	}

}