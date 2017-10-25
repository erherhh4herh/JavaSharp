using System;

/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that the <code>clone</code> method in class
	/// <code>Object</code> has been called to clone an object, but that
	/// the object's class does not implement the <code>Cloneable</code>
	/// interface.
	/// <para>
	/// Applications that override the <code>clone</code> method can also
	/// throw this exception to indicate that an object could not or
	/// should not be cloned.
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.Cloneable </seealso>
	/// <seealso cref=     java.lang.Object#clone()
	/// @since   JDK1.0 </seealso>

	public class CloneNotSupportedException : Exception
	{
		private new const long SerialVersionUID = 5195511250079656443L;

		/// <summary>
		/// Constructs a <code>CloneNotSupportedException</code> with no
		/// detail message.
		/// </summary>
		public CloneNotSupportedException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>CloneNotSupportedException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public CloneNotSupportedException(String s) : base(s)
		{
		}
	}

}