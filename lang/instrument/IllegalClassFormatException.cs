using System;

/*
 * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.instrument
{

	/*
	 * Copyright 2003 Wily Technology, Inc.
	 */

	/// <summary>
	/// Thrown by an implementation of
	/// <seealso cref="java.lang.instrument.ClassFileTransformer#transform ClassFileTransformer.transform"/>
	/// when its input parameters are invalid.
	/// This may occur either because the initial class file bytes were
	/// invalid or a previously applied transform corrupted the bytes.
	/// </summary>
	/// <seealso cref=     java.lang.instrument.ClassFileTransformer#transform
	/// @since   1.5 </seealso>
	public class IllegalClassFormatException : Exception
	{
		private new const long SerialVersionUID = -3841736710924794009L;

		/// <summary>
		/// Constructs an <code>IllegalClassFormatException</code> with no
		/// detail message.
		/// </summary>
		public IllegalClassFormatException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalClassFormatException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IllegalClassFormatException(String s) : base(s)
		{
		}
	}

}