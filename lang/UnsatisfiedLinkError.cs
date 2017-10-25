/*
 * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown if the Java Virtual Machine cannot find an appropriate
	/// native-language definition of a method declared <code>native</code>.
	/// 
	/// @author unascribed </summary>
	/// <seealso cref=     java.lang.Runtime
	/// @since   JDK1.0 </seealso>
	public class UnsatisfiedLinkError : LinkageError
	{
		private new const long SerialVersionUID = -4019343241616879428L;

		/// <summary>
		/// Constructs an <code>UnsatisfiedLinkError</code> with no detail message.
		/// </summary>
		public UnsatisfiedLinkError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>UnsatisfiedLinkError</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public UnsatisfiedLinkError(String s) : base(s)
		{
		}
	}

}