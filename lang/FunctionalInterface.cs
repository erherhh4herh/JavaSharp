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

namespace java.lang
{

	/// <summary>
	/// An informative annotation type used to indicate that an interface
	/// type declaration is intended to be a <i>functional interface</i> as
	/// defined by the Java Language Specification.
	/// 
	/// Conceptually, a functional interface has exactly one abstract
	/// method.  Since {@link java.lang.reflect.Method#isDefault()
	/// default methods} have an implementation, they are not abstract.  If
	/// an interface declares an abstract method overriding one of the
	/// public methods of {@code java.lang.Object}, that also does
	/// <em>not</em> count toward the interface's abstract method count
	/// since any implementation of the interface will have an
	/// implementation from {@code java.lang.Object} or elsewhere.
	/// 
	/// <para>Note that instances of functional interfaces can be created with
	/// lambda expressions, method references, or constructor references.
	/// 
	/// </para>
	/// <para>If a type is annotated with this annotation type, compilers are
	/// required to generate an error message unless:
	/// 
	/// <ul>
	/// <li> The type is an interface type and not an annotation type, enum, or class.
	/// <li> The annotated type satisfies the requirements of a functional interface.
	/// </ul>
	/// 
	/// </para>
	/// <para>However, the compiler will treat any interface meeting the
	/// definition of a functional interface as a functional interface
	/// regardless of whether or not a {@code FunctionalInterface}
	/// annotation is present on the interface declaration.
	/// 
	/// @jls 4.3.2. The Class Object
	/// @jls 9.8 Functional Interfaces
	/// @jls 9.4.3 Interface Method Body
	/// @since 1.8
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Documented @Retention(RetentionPolicy.RUNTIME) @Target(ElementType.TYPE) public class FunctionalInterface extends System.Attribute
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false]
	public class FunctionalInterface : System.Attribute
	{
	}

}