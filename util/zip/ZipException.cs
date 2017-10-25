/*
 * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that a Zip exception of some sort has occurred.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.io.IOException
	/// @since   JDK1.0 </seealso>

	public class ZipException : IOException
	{
		private new const long SerialVersionUID = 8000196834066748623L;

		/// <summary>
		/// Constructs a <code>ZipException</code> with <code>null</code>
		/// as its error detail message.
		/// </summary>
		public ZipException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>ZipException</code> with the specified detail
		/// message.
		/// </summary>
		/// <param name="s">   the detail message. </param>

		public ZipException(String s) : base(s)
		{
		}
	}

}