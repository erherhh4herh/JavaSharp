/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.interfaces
{

	/// <summary>
	/// The interface to a DSA public or private key. DSA (Digital Signature
	/// Algorithm) is defined in NIST's FIPS-186.
	/// </summary>
	/// <seealso cref= DSAParams </seealso>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.Signature
	/// 
	/// @author Benjamin Renaud
	/// @author Josh Bloch </seealso>
	public interface DSAKey
	{

		/// <summary>
		/// Returns the DSA-specific key parameters. These parameters are
		/// never secret.
		/// </summary>
		/// <returns> the DSA-specific key parameters.
		/// </returns>
		/// <seealso cref= DSAParams </seealso>
		DSAParams Params {get;}
	}

}