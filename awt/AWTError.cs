/*
 * Copyright (c) 1995, 1997, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when a serious Abstract Window Toolkit error has occurred.
	/// 
	/// @author      Arthur van Hoff
	/// </summary>
	public class AWTError : Error
	{

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private new const long SerialVersionUID = -1819846354050686206L;

		/// <summary>
		/// Constructs an instance of <code>AWTError</code> with the specified
		/// detail message. </summary>
		/// <param name="msg">   the detail message.
		/// @since   JDK1.0 </param>
		public AWTError(String msg) : base(msg)
		{
		}
	}

}