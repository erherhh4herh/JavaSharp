/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// This exception is thrown if an entry in the keystore cannot be recovered.
	/// 
	/// 
	/// @since 1.5
	/// </summary>

	public class UnrecoverableEntryException : GeneralSecurityException
	{

		private new const long SerialVersionUID = -4527142945246286535L;

		/// <summary>
		/// Constructs an UnrecoverableEntryException with no detail message.
		/// </summary>
		public UnrecoverableEntryException() : base()
		{
		}

		/// <summary>
		/// Constructs an UnrecoverableEntryException with the specified detail
		/// message, which provides more information about why this exception
		/// has been thrown.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
	   public UnrecoverableEntryException(String msg) : base(msg)
	   {
	   }
	}

}