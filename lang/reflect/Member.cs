/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	/// <summary>
	/// Member is an interface that reflects identifying information about
	/// a single member (a field or a method) or a constructor.
	/// </summary>
	/// <seealso cref= java.lang.Class </seealso>
	/// <seealso cref= Field </seealso>
	/// <seealso cref= Method </seealso>
	/// <seealso cref= Constructor
	/// 
	/// @author Nakul Saraiya </seealso>
	public interface Member
	{

		/// <summary>
		/// Identifies the set of all public members of a class or interface,
		/// including inherited members.
		/// </summary>

		/// <summary>
		/// Identifies the set of declared members of a class or interface.
		/// Inherited members are not included.
		/// </summary>

		/// <summary>
		/// Returns the Class object representing the class or interface
		/// that declares the member or constructor represented by this Member.
		/// </summary>
		/// <returns> an object representing the declaring class of the
		/// underlying member </returns>
		Class DeclaringClass {get;}

		/// <summary>
		/// Returns the simple name of the underlying member or constructor
		/// represented by this Member.
		/// </summary>
		/// <returns> the simple name of the underlying member </returns>
		String Name {get;}

		/// <summary>
		/// Returns the Java language modifiers for the member or
		/// constructor represented by this Member, as an integer.  The
		/// Modifier class should be used to decode the modifiers in
		/// the integer.
		/// </summary>
		/// <returns> the Java language modifiers for the underlying member </returns>
		/// <seealso cref= Modifier </seealso>
		int Modifiers {get;}

		/// <summary>
		/// Returns {@code true} if this member was introduced by
		/// the compiler; returns {@code false} otherwise.
		/// </summary>
		/// <returns> true if and only if this member was introduced by
		/// the compiler.
		/// @jls 13.1 The Form of a Binary
		/// @since 1.5 </returns>
		bool Synthetic {get;}
	}

	public static class Member_Fields
	{
		public const int PUBLIC = 0;
		public const int DECLARED = 1;
	}

}