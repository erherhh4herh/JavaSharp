using System;

/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.acl
{

	/// <summary>
	/// This is an exception that is thrown whenever an attempt is made to delete
	/// the last owner of an Access Control List.
	/// </summary>
	/// <seealso cref= java.security.acl.Owner#deleteOwner
	/// 
	/// @author Satish Dharmaraj </seealso>
	public class LastOwnerException : Exception
	{

		private new const long SerialVersionUID = -5141997548211140359L;

		/// <summary>
		/// Constructs a LastOwnerException.
		/// </summary>
		public LastOwnerException()
		{
		}
	}

}