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

namespace java.lang.annotation
{

	/// <summary>
	/// Indicates that annotations with a type are to be documented by javadoc
	/// and similar tools by default.  This type should be used to annotate the
	/// declarations of types whose annotations affect the use of annotated
	/// elements by their clients.  If a type declaration is annotated with
	/// Documented, its annotations become part of the public API
	/// of the annotated elements.
	/// 
	/// @author  Joshua Bloch
	/// @since 1.5
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Documented @Retention(RetentionPolicy.RUNTIME) @Target(ElementType.ANNOTATION_TYPE) public class Documented extends System.Attribute
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false]
	public class Documented : System.Attribute
	{
	}

}