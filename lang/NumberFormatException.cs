/*
 * Copyright (c) 1994, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that the application has attempted to convert
	/// a string to one of the numeric types, but that the string does not
	/// have the appropriate format.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.lang.Integer#parseInt(String)
	/// @since   JDK1.0 </seealso>
	public class NumberFormatException : IllegalArgumentException
	{
		internal new const long SerialVersionUID = -2848938806368998894L;

		/// <summary>
		/// Constructs a <code>NumberFormatException</code> with no detail message.
		/// </summary>
		public NumberFormatException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NumberFormatException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NumberFormatException(String s) : base(s)
		{
		}

		/// <summary>
		/// Factory method for making a <code>NumberFormatException</code>
		/// given the specified input which caused the error.
		/// </summary>
		/// <param name="s">   the input causing the error </param>
		internal static NumberFormatException ForInputString(String s)
		{
			return new NumberFormatException("For input string: \"" + s + "\"");
		}
	}

}