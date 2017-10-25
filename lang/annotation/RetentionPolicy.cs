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
	/// Annotation retention policy.  The constants of this enumerated type
	/// describe the various policies for retaining annotations.  They are used
	/// in conjunction with the <seealso cref="Retention"/> meta-annotation type to specify
	/// how long annotations are to be retained.
	/// 
	/// @author  Joshua Bloch
	/// @since 1.5
	/// </summary>
	public enum RetentionPolicy
	{
		/// <summary>
		/// Annotations are to be discarded by the compiler.
		/// </summary>
		SOURCE,

		/// <summary>
		/// Annotations are to be recorded in the class file by the compiler
		/// but need not be retained by the VM at run time.  This is the default
		/// behavior.
		/// </summary>
		CLASS,

		/// <summary>
		/// Annotations are to be recorded in the class file by the compiler and
		/// retained by the VM at run time, so they may be read reflectively.
		/// </summary>
		/// <seealso cref= java.lang.reflect.AnnotatedElement </seealso>
		RUNTIME
	}

}