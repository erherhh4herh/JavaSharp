/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// Thrown when control information that was read from an object stream
	/// violates internal consistency checks.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public class StreamCorruptedException : ObjectStreamException
	{

		private new const long SerialVersionUID = 8983558202217591746L;

		/// <summary>
		/// Create a StreamCorruptedException and list a reason why thrown.
		/// </summary>
		/// <param name="reason">  String describing the reason for the exception. </param>
		public StreamCorruptedException(String reason) : base(reason)
		{
		}

		/// <summary>
		/// Create a StreamCorruptedException and list no reason why thrown.
		/// </summary>
		public StreamCorruptedException() : base()
		{
		}
	}

}