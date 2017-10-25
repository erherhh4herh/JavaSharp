using System;

/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.zip
{

	/// <summary>
	/// Signals that a data format error has occurred.
	/// 
	/// @author      David Connelly
	/// </summary>
	public class DataFormatException : Exception
	{
		private new const long SerialVersionUID = 2219632870893641452L;

		/// <summary>
		/// Constructs a DataFormatException with no detail message.
		/// </summary>
		public DataFormatException() : base()
		{
		}

		/// <summary>
		/// Constructs a DataFormatException with the specified detail message.
		/// A detail message is a String that describes this particular exception. </summary>
		/// <param name="s"> the String containing a detail message </param>
		public DataFormatException(String s) : base(s)
		{
		}
	}

}