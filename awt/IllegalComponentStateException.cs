/*
 * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{

	/// <summary>
	/// Signals that an AWT component is not in an appropriate state for
	/// the requested operation.
	/// 
	/// @author      Jonni Kanerva
	/// </summary>
	public class IllegalComponentStateException : IllegalStateException
	{
		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private new const long SerialVersionUID = -1889339587208144238L;

		/// <summary>
		/// Constructs an IllegalComponentStateException with no detail message.
		/// A detail message is a String that describes this particular exception.
		/// </summary>
		public IllegalComponentStateException() : base()
		{
		}

		/// <summary>
		/// Constructs an IllegalComponentStateException with the specified detail
		/// message.  A detail message is a String that describes this particular
		/// exception. </summary>
		/// <param name="s"> the String that contains a detailed message </param>
		public IllegalComponentStateException(String s) : base(s)
		{
		}
	}

}