/*
 * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.zip
{

	/// <summary>
	/// A reference to the native zlib's z_stream structure.
	/// </summary>

	internal class ZStreamRef
	{

		private long Address_Renamed;
		internal ZStreamRef(long address)
		{
			this.Address_Renamed = address;
		}

		internal virtual long Address()
		{
			return Address_Renamed;
		}

		internal virtual void Clear()
		{
			Address_Renamed = 0;
		}
	}

}