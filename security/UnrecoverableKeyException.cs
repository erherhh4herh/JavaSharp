/*
 * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// This exception is thrown if a key in the keystore cannot be recovered.
	/// 
	/// 
	/// @since 1.2
	/// </summary>

	public class UnrecoverableKeyException : UnrecoverableEntryException
	{

		private new const long SerialVersionUID = 7275063078190151277L;

		/// <summary>
		/// Constructs an UnrecoverableKeyException with no detail message.
		/// </summary>
		public UnrecoverableKeyException() : base()
		{
		}

		/// <summary>
		/// Constructs an UnrecoverableKeyException with the specified detail
		/// message, which provides more information about why this exception
		/// has been thrown.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
	   public UnrecoverableKeyException(String msg) : base(msg)
	   {
	   }
	}

}