/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// Enumeration for RowId life-time values.
	/// 
	/// @since 1.6
	/// </summary>

	public enum RowIdLifetime
	{

		/// <summary>
		/// Indicates that this data source does not support the ROWID type.
		/// </summary>
		ROWID_UNSUPPORTED,

		/// <summary>
		/// Indicates that the lifetime of a RowId from this data source is indeterminate;
		/// but not one of ROWID_VALID_TRANSACTION, ROWID_VALID_SESSION, or,
		/// ROWID_VALID_FOREVER.
		/// </summary>
		ROWID_VALID_OTHER,

		/// <summary>
		/// Indicates that the lifetime of a RowId from this data source is at least the
		/// containing session.
		/// </summary>
		ROWID_VALID_SESSION,

		/// <summary>
		/// Indicates that the lifetime of a RowId from this data source is at least the
		/// containing transaction.
		/// </summary>
		ROWID_VALID_TRANSACTION,

		/// <summary>
		/// Indicates that the lifetime of a RowId from this data source is, effectively,
		/// unlimited.
		/// </summary>
		ROWID_VALID_FOREVER
	}

}