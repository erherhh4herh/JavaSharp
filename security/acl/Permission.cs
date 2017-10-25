/*
 * Copyright (c) 1996, Oracle and/or its affiliates. All rights reserved.
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
	/// This interface represents a permission, such as that used to grant
	/// a particular type of access to a resource.
	/// 
	/// @author Satish Dharmaraj
	/// </summary>
	public interface Permission
	{

		/// <summary>
		/// Returns true if the object passed matches the permission represented
		/// in this interface.
		/// </summary>
		/// <param name="another"> the Permission object to compare with.
		/// </param>
		/// <returns> true if the Permission objects are equal, false otherwise </returns>
		bool Equals(Object another);

		/// <summary>
		/// Prints a string representation of this permission.
		/// </summary>
		/// <returns> the string representation of the permission. </returns>
		String ToString();

	}

}