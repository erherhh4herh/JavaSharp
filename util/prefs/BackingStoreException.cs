using System;

/*
 * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	/// <summary>
	/// Thrown to indicate that a preferences operation could not complete because
	/// of a failure in the backing store, or a failure to contact the backing
	/// store.
	/// 
	/// @author  Josh Bloch
	/// @since   1.4
	/// </summary>
	public class BackingStoreException : Exception
	{
		/// <summary>
		/// Constructs a BackingStoreException with the specified detail message.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		public BackingStoreException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a BackingStoreException with the specified cause.
		/// </summary>
		/// <param name="cause"> the cause </param>
		public BackingStoreException(Throwable cause) : base(cause)
		{
		}

		private new const long SerialVersionUID = 859796500401108469L;
	}

}