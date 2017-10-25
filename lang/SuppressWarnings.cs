﻿/*
 * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Indicates that the named compiler warnings should be suppressed in the
	/// annotated element (and in all program elements contained in the annotated
	/// element).  Note that the set of warnings suppressed in a given element is
	/// a superset of the warnings suppressed in all containing elements.  For
	/// example, if you annotate a class to suppress one warning and annotate a
	/// method to suppress another, both warnings will be suppressed in the method.
	/// 
	/// <para>As a matter of style, programmers should always use this annotation
	/// on the most deeply nested element where it is effective.  If you want to
	/// suppress a warning in a particular method, you should annotate that
	/// method rather than its class.
	/// 
	/// @author Josh Bloch
	/// @since 1.5
	/// @jls 4.8 Raw Types
	/// @jls 4.12.2 Variables of Reference Type
	/// @jls 5.1.9 Unchecked Conversion
	/// @jls 5.5.2 Checked Casts and Unchecked Casts
	/// @jls 9.6.3.5 @SuppressWarnings
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
//ORIGINAL LINE: @Target({TYPE, FIELD, METHOD, PARAMETER, CONSTRUCTOR, LOCAL_VARIABLE}) @Retention(RetentionPolicy.SOURCE) public class SuppressWarnings extends System.Attribute
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to CONSTRUCTOR:
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to LOCAL_VARIABLE:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	[AttributeUsage(<missing> | <missing> | <missing> | <missing> | <missing> | <missing>, AllowMultiple = false, Inherited = false]
	public class SuppressWarnings : System.Attribute
	{
		/// <summary>
		/// The set of warnings that are to be suppressed by the compiler in the
		/// annotated element.  Duplicate names are permitted.  The second and
		/// successive occurrences of a name are ignored.  The presence of
		/// unrecognized warning names is <i>not</i> an error: Compilers must
		/// ignore any warning names they do not recognize.  They are, however,
		/// free to emit a warning if an annotation contains an unrecognized
		/// warning name.
		/// 
		/// <para> The string {@code "unchecked"} is used to suppress
		/// unchecked warnings. Compiler vendors should document the
		/// additional warning names they support in conjunction with this
		/// annotation type. They are encouraged to cooperate to ensure
		/// that the same names work across multiple compilers.
		/// </para>
		/// </summary>
		/// <returns> the set of warnings to be suppressed </returns>
		String[] value();
	}

}