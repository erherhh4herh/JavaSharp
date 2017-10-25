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
	/// A program element annotated &#64;Deprecated is one that programmers
	/// are discouraged from using, typically because it is dangerous,
	/// or because a better alternative exists.  Compilers warn when a
	/// deprecated program element is used or overridden in non-deprecated code.
	/// 
	/// @author  Neal Gafter
	/// @since 1.5
	/// @jls 9.6.3.6 @Deprecated
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to value={CONSTRUCTOR, FIELD, LOCAL_VARIABLE, METHOD, PACKAGE, PARAMETER, TYPE}:
//ORIGINAL LINE: @Documented @Retention(RetentionPolicy.RUNTIME) @Target(value={CONSTRUCTOR, FIELD, LOCAL_VARIABLE, METHOD, PACKAGE, PARAMETER, TYPE}) public class Deprecated extends System.Attribute
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	[AttributeUsage(<missing>, AllowMultiple = false, Inherited = false]
	public class Deprecated : System.Attribute
	{
	}

}