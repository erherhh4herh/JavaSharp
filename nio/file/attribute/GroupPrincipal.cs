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
	/// A {@code UserPrincipal} representing a <em>group identity</em>, used to
	/// determine access rights to objects in a file system. The exact definition of
	/// a group is implementation specific, but typically, it represents an identity
	/// created for administrative purposes so as to determine the access rights for
	/// the members of the group. Whether an entity can be a member of multiple
	/// groups, and whether groups can be nested, are implementation specified and
	/// therefore not specified.
	/// 
	/// @since 1.7
	/// </summary>
	/// <seealso cref= UserPrincipalLookupService#lookupPrincipalByGroupName </seealso>

	public interface GroupPrincipal : UserPrincipal
	{
	}

}