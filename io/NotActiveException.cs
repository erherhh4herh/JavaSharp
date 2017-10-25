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
	/// Thrown when serialization or deserialization is not active.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public class NotActiveException : ObjectStreamException
	{

		private new const long SerialVersionUID = -3893467273049808895L;

		/// <summary>
		/// Constructor to create a new NotActiveException with the reason given.
		/// </summary>
		/// <param name="reason">  a String describing the reason for the exception. </param>
		public NotActiveException(String reason) : base(reason)
		{
		}

		/// <summary>
		/// Constructor to create a new NotActiveException without a reason.
		/// </summary>
		public NotActiveException() : base()
		{
		}
	}

}