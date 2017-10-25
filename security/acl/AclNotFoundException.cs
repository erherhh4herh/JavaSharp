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
	/// This is an exception that is thrown whenever a reference is made to a
	/// non-existent ACL (Access Control List).
	/// 
	/// @author      Satish Dharmaraj
	/// </summary>
	public class AclNotFoundException : Exception
	{

		private new const long SerialVersionUID = 5684295034092681791L;

		/// <summary>
		/// Constructs an AclNotFoundException.
		/// </summary>
		public AclNotFoundException()
		{
		}

	}

}