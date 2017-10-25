/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.attribute
{

	/// <summary>
	/// A typesafe enumeration of the access control entry types.
	/// 
	/// @since 1.7
	/// </summary>

	public enum AclEntryType
	{
		/// <summary>
		/// Explicitly grants access to a file or directory.
		/// </summary>
		ALLOW,

		/// <summary>
		/// Explicitly denies access to a file or directory.
		/// </summary>
		DENY,

		/// <summary>
		/// Log, in a system dependent way, the access specified in the
		/// permissions component of the ACL entry.
		/// </summary>
		AUDIT,

		/// <summary>
		/// Generate an alarm, in a system dependent way, the access specified in the
		/// permissions component of the ACL entry.
		/// </summary>
		ALARM
	}

}