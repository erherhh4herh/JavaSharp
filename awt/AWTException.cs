using System;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that an Abstract Window Toolkit exception has occurred.
	/// 
	/// @author      Arthur van Hoff
	/// </summary>
	public class AWTException : Exception
	{

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private new const long SerialVersionUID = -1900414231151323879L;

		/// <summary>
		/// Constructs an instance of <code>AWTException</code> with the
		/// specified detail message. A detail message is an
		/// instance of <code>String</code> that describes this particular
		/// exception. </summary>
		/// <param name="msg">     the detail message
		/// @since   JDK1.0 </param>
		public AWTException(String msg) : base(msg)
		{
		}
	}

}