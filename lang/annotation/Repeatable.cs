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

namespace java.lang.annotation
{

	/// <summary>
	/// The annotation type {@code java.lang.annotation.Repeatable} is
	/// used to indicate that the annotation type whose declaration it
	/// (meta-)annotates is <em>repeatable</em>. The value of
	/// {@code @Repeatable} indicates the <em>containing annotation
	/// type</em> for the repeatable annotation type.
	/// 
	/// @since 1.8
	/// @jls 9.6 Annotation Types
	/// @jls 9.7 Annotations
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Documented @Retention(RetentionPolicy.RUNTIME) @Target(ElementType.ANNOTATION_TYPE) public class Repeatable extends System.Attribute
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false]
	public class Repeatable : System.Attribute
	{
		/// <summary>
		/// Indicates the <em>containing annotation type</em> for the
		/// repeatable annotation type. </summary>
		/// <returns> the containing annotation type </returns>
		Class value();
	}

}