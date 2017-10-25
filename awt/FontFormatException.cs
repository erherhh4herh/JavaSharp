using System;

/*
 * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown by method createFont in the <code>Font</code> class to indicate
	/// that the specified font is bad.
	/// 
	/// @author  Parry Kejriwal </summary>
	/// <seealso cref=     java.awt.Font
	/// @since   1.3 </seealso>
	public class FontFormatException : Exception
	{
		/*
		 * serialVersionUID
		 */
		private new const long SerialVersionUID = -4481290147811361272L;

		/// <summary>
		/// Report a FontFormatException for the reason specified. </summary>
		/// <param name="reason"> a <code>String</code> message indicating why
		///        the font is not accepted. </param>
		public FontFormatException(String reason) : base(reason)
		{
		}
	}

}