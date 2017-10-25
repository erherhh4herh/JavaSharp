/*
 * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The interface to an RSA public or private key.
	/// 
	/// @author Jan Luehe
	/// </summary>
	/// <seealso cref= RSAPublicKey </seealso>
	/// <seealso cref= RSAPrivateKey
	/// 
	/// @since 1.3 </seealso>

	public interface RSAKey
	{

		/// <summary>
		/// Returns the modulus.
		/// </summary>
		/// <returns> the modulus </returns>
		System.Numerics.BigInteger Modulus {get;}
	}

}