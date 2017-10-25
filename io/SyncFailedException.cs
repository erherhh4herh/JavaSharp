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

namespace java.io
{

	/// <summary>
	/// Signals that a sync operation has failed.
	/// 
	/// @author  Ken Arnold </summary>
	/// <seealso cref=     java.io.FileDescriptor#sync </seealso>
	/// <seealso cref=     java.io.IOException
	/// @since   JDK1.1 </seealso>
	public class SyncFailedException : IOException
	{
		private new const long SerialVersionUID = -2353342684412443330L;

		/// <summary>
		/// Constructs an SyncFailedException with a detail message.
		/// A detail message is a String that describes this particular exception.
		/// </summary>
		/// <param name="desc">  a String describing the exception. </param>
		public SyncFailedException(String desc) : base(desc)
		{
		}
	}

}