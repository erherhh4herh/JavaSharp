/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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

namespace java.sql
{


	/// <summary>
	/// Enumeration for pseudo/hidden column usage.
	/// 
	/// @since 1.7 </summary>
	/// <seealso cref= DatabaseMetaData#getPseudoColumns </seealso>
	public enum PseudoColumnUsage
	{

		/// <summary>
		/// The pseudo/hidden column may only be used in a SELECT list.
		/// </summary>
		SELECT_LIST_ONLY,

		/// <summary>
		/// The pseudo/hidden column may only be used in a WHERE clause.
		/// </summary>
		WHERE_CLAUSE_ONLY,

		/// <summary>
		/// There are no restrictions on the usage of the pseudo/hidden columns.
		/// </summary>
		NO_USAGE_RESTRICTIONS,

		/// <summary>
		/// The usage of the pseudo/hidden column cannot be determined.
		/// </summary>
		USAGE_UNKNOWN

	}

}