/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.net
{

	/// <summary>
	/// Represents a family of communication protocols.
	/// 
	/// @since 1.7
	/// </summary>

	public interface ProtocolFamily
	{
		/// <summary>
		/// Returns the name of the protocol family.
		/// </summary>
		/// <returns> the name of the protocol family </returns>
		String Name();
	}

}