/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// This exception, designed for use by the JCA/JCE engine classes,
	/// is thrown when an invalid parameter is passed
	/// to a method.
	/// 
	/// @author Benjamin Renaud
	/// </summary>

	public class InvalidParameterException : IllegalArgumentException
	{

		private new const long SerialVersionUID = -857968536935667808L;

		/// <summary>
		/// Constructs an InvalidParameterException with no detail message.
		/// A detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public InvalidParameterException() : base()
		{
		}

		/// <summary>
		/// Constructs an InvalidParameterException with the specified
		/// detail message.  A detail message is a String that describes
		/// this particular exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public InvalidParameterException(String msg) : base(msg)
		{
		}
	}

}