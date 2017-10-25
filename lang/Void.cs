/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code Void} class is an uninstantiable placeholder class to hold a
	/// reference to the {@code Class} object representing the Java keyword
	/// void.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public sealed class Void
	{

		/// <summary>
		/// The {@code Class} object representing the pseudo-type corresponding to
		/// the keyword {@code void}.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("void");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("void");

		/*
		 * The Void class cannot be instantiated.
		 */
		private Void()
		{
		}
	}

}