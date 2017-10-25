/*
 * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// Enumeration for status of the reason that a property could not be set
	/// via a call to <code>Connection.setClientInfo</code>
	/// @since 1.6
	/// </summary>

	public enum ClientInfoStatus
	{

		/// <summary>
		/// The client info property could not be set for some unknown reason
		/// @since 1.6
		/// </summary>
		REASON_UNKNOWN,

		/// <summary>
		/// The client info property name specified was not a recognized property
		/// name.
		/// @since 1.6
		/// </summary>
		REASON_UNKNOWN_PROPERTY,

		/// <summary>
		/// The value specified for the client info property was not valid.
		/// @since 1.6
		/// </summary>
		REASON_VALUE_INVALID,

		/// <summary>
		/// The value specified for the client info property was too large.
		/// @since 1.6
		/// </summary>
		REASON_VALUE_TRUNCATED
	}

}