using System;

/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// A <code>SkeletonNotFoundException</code> is thrown if the
	/// <code>Skeleton</code> corresponding to the remote object being
	/// exported is not found.  Skeletons are no longer required, so this
	/// exception is never thrown.
	/// 
	/// @since   JDK1.1 </summary>
	/// @deprecated no replacement.  Skeletons are no longer required for remote
	/// method calls in the Java 2 platform v1.2 and greater. 
	[Obsolete("no replacement.  Skeletons are no longer required for remote")]
	public class SkeletonNotFoundException : RemoteException
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = -7860299673822761231L;

		/// <summary>
		/// Constructs a <code>SkeletonNotFoundException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message.
		/// @since JDK1.1 </param>
		public SkeletonNotFoundException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a <code>SkeletonNotFoundException</code> with the specified
		/// detail message and nested exception.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		/// <param name="ex"> the nested exception
		/// @since JDK1.1 </param>
		public SkeletonNotFoundException(String s, Exception ex) : base(s, ex)
		{
		}
	}

}