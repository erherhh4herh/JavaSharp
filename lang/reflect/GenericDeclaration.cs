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
	/// A common interface for all entities that declare type variables.
	/// 
	/// @since 1.5
	/// </summary>
	public interface GenericDeclaration : AnnotatedElement
	{
		/// <summary>
		/// Returns an array of {@code TypeVariable} objects that
		/// represent the type variables declared by the generic
		/// declaration represented by this {@code GenericDeclaration}
		/// object, in declaration order.  Returns an array of length 0 if
		/// the underlying generic declaration declares no type variables.
		/// </summary>
		/// <returns> an array of {@code TypeVariable} objects that represent
		///     the type variables declared by this generic declaration </returns>
		/// <exception cref="GenericSignatureFormatError"> if the generic
		///     signature of this generic declaration does not conform to
		///     the format specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public TypeVariable<?>[] getTypeParameters();
		TypeVariable<?>[] TypeParameters {get;}
	}

}