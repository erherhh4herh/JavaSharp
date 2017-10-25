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
	/// Indicates the contexts in which an annotation type is applicable. The
	/// declaration contexts and type contexts in which an annotation type may be
	/// applicable are specified in JLS 9.6.4.1, and denoted in source code by enum
	/// constants of <seealso cref="ElementType java.lang.annotation.ElementType"/>.
	/// 
	/// <para>If an {@code @Target} meta-annotation is not present on an annotation type
	/// {@code T} , then an annotation of type {@code T} may be written as a
	/// modifier for any declaration except a type parameter declaration.
	/// 
	/// </para>
	/// <para>If an {@code @Target} meta-annotation is present, the compiler will enforce
	/// the usage restrictions indicated by {@code ElementType}
	/// enum constants, in line with JLS 9.7.4.
	/// 
	/// </para>
	/// <para>For example, this {@code @Target} meta-annotation indicates that the
	/// declared type is itself a meta-annotation type.  It can only be used on
	/// annotation type declarations:
	/// <pre>
	///    &#064;Target(ElementType.ANNOTATION_TYPE)
	///    public &#064;interface MetaAnnotationType {
	///        ...
	///    }
	/// </pre>
	/// 
	/// </para>
	/// <para>This {@code @Target} meta-annotation indicates that the declared type is
	/// intended solely for use as a member type in complex annotation type
	/// declarations.  It cannot be used to annotate anything directly:
	/// <pre>
	///    &#064;Target({})
	///    public &#064;interface MemberType {
	///        ...
	///    }
	/// </pre>
	/// 
	/// </para>
	/// <para>It is a compile-time error for a single {@code ElementType} constant to
	/// appear more than once in an {@code @Target} annotation.  For example, the
	/// following {@code @Target} meta-annotation is illegal:
	/// <pre>
	///    &#064;Target({ElementType.FIELD, ElementType.METHOD, ElementType.FIELD})
	///    public &#064;interface Bogus {
	///        ...
	///    }
	/// </pre>
	/// 
	/// @since 1.5
	/// @jls 9.6.4.1 @Target
	/// @jls 9.7.4 Where Annotations May Appear
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Documented @Retention(RetentionPolicy.RUNTIME) @Target(ElementType.ANNOTATION_TYPE) public class Target extends System.Attribute
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false]
	public class Target : System.Attribute
	{
		/// <summary>
		/// Returns an array of the kinds of elements an annotation type
		/// can be applied to. </summary>
		/// <returns> an array of the kinds of elements an annotation type
		/// can be applied to </returns>
		ElementType[] value();
	}

}