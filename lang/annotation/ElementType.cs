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

namespace java.lang.annotation
{

	/// <summary>
	/// The constants of this enumerated type provide a simple classification of the
	/// syntactic locations where annotations may appear in a Java program. These
	/// constants are used in <seealso cref="Target java.lang.annotation.Target"/>
	/// meta-annotations to specify where it is legal to write annotations of a
	/// given type.
	/// 
	/// <para>The syntactic locations where annotations may appear are split into
	/// <em>declaration contexts</em> , where annotations apply to declarations, and
	/// <em>type contexts</em> , where annotations apply to types used in
	/// declarations and expressions.
	/// 
	/// </para>
	/// <para>The constants <seealso cref="#ANNOTATION_TYPE"/> , <seealso cref="#CONSTRUCTOR"/> , {@link
	/// #FIELD} , <seealso cref="#LOCAL_VARIABLE"/> , <seealso cref="#METHOD"/> , <seealso cref="#PACKAGE"/> ,
	/// <seealso cref="#PARAMETER"/> , <seealso cref="#TYPE"/> , and <seealso cref="#TYPE_PARAMETER"/> correspond
	/// to the declaration contexts in JLS 9.6.4.1.
	/// 
	/// </para>
	/// <para>For example, an annotation whose type is meta-annotated with
	/// {@code @Target(ElementType.FIELD)} may only be written as a modifier for a
	/// field declaration.
	/// 
	/// </para>
	/// <para>The constant <seealso cref="#TYPE_USE"/> corresponds to the 15 type contexts in JLS
	/// 4.11, as well as to two declaration contexts: type declarations (including
	/// annotation type declarations) and type parameter declarations.
	/// 
	/// </para>
	/// <para>For example, an annotation whose type is meta-annotated with
	/// {@code @Target(ElementType.TYPE_USE)} may be written on the type of a field
	/// (or within the type of the field, if it is a nested, parameterized, or array
	/// type), and may also appear as a modifier for, say, a class declaration.
	/// 
	/// </para>
	/// <para>The {@code TYPE_USE} constant includes type declarations and type
	/// parameter declarations as a convenience for designers of type checkers which
	/// give semantics to annotation types. For example, if the annotation type
	/// {@code NonNull} is meta-annotated with
	/// {@code @Target(ElementType.TYPE_USE)}, then {@code @NonNull}
	/// {@code class C {...}} could be treated by a type checker as indicating that
	/// all variables of class {@code C} are non-null, while still allowing
	/// variables of other classes to be non-null or not non-null based on whether
	/// {@code @NonNull} appears at the variable's declaration.
	/// 
	/// @author  Joshua Bloch
	/// @since 1.5
	/// @jls 9.6.4.1 @Target
	/// @jls 4.1 The Kinds of Types and Values
	/// </para>
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Class, interface (including annotation type), or enum declaration </summary>
		TYPE,

		/// <summary>
		/// Field declaration (includes enum constants) </summary>
		FIELD,

		/// <summary>
		/// Method declaration </summary>
		METHOD,

		/// <summary>
		/// Formal parameter declaration </summary>
		PARAMETER,

		/// <summary>
		/// Constructor declaration </summary>
		CONSTRUCTOR,

		/// <summary>
		/// Local variable declaration </summary>
		LOCAL_VARIABLE,

		/// <summary>
		/// Annotation type declaration </summary>
		ANNOTATION_TYPE,

		/// <summary>
		/// Package declaration </summary>
		PACKAGE,

		/// <summary>
		/// Type parameter declaration
		/// 
		/// @since 1.8
		/// </summary>
		TYPE_PARAMETER,

		/// <summary>
		/// Use of a type
		/// 
		/// @since 1.8
		/// </summary>
		TYPE_USE
	}

}