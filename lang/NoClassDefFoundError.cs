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
	/// Thrown if the Java Virtual Machine or a <code>ClassLoader</code> instance
	/// tries to load in the definition of a class (as part of a normal method call
	/// or as part of creating a new instance using the <code>new</code> expression)
	/// and no definition of the class could be found.
	/// <para>
	/// The searched-for class definition existed when the currently
	/// executing class was compiled, but the definition can no longer be
	/// found.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class NoClassDefFoundError : LinkageError
	{
		private new const long SerialVersionUID = 9095859863287012458L;

		/// <summary>
		/// Constructs a <code>NoClassDefFoundError</code> with no detail message.
		/// </summary>
		public NoClassDefFoundError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NoClassDefFoundError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NoClassDefFoundError(String s) : base(s)
		{
		}
	}

}