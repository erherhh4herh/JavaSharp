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

namespace java.lang
{

	/// <summary>
	/// Indicates that a method declaration is intended to override a
	/// method declaration in a supertype. If a method is annotated with
	/// this annotation type compilers are required to generate an error
	/// message unless at least one of the following conditions hold:
	/// 
	/// <ul><li>
	/// The method does override or implement a method declared in a
	/// supertype.
	/// </li><li>
	/// The method has a signature that is override-equivalent to that of
	/// any public method declared in <seealso cref="Object"/>.
	/// </li></ul>
	/// 
	/// @author  Peter von der Ah&eacute;
	/// @author  Joshua Bloch
	/// @jls 9.6.1.4 @Override
	/// @since 1.5
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Target(ElementType.METHOD) @Retention(RetentionPolicy.SOURCE) public class Override extends System.Attribute
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false]
	public class Override : System.Attribute
	{
	}

}