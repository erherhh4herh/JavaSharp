/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The interface to an RSA public key.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>

	public interface RSAPublicKey : java.security.PublicKey, RSAKey
	{
		/// <summary>
		/// The type fingerprint that is set to indicate
		/// serialization compatibility with a previous
		/// version of the type.
		/// </summary>

		/// <summary>
		/// Returns the public exponent.
		/// </summary>
		/// <returns> the public exponent </returns>
		System.Numerics.BigInteger PublicExponent {get;}
	}

	public static class RSAPublicKey_Fields
	{
		public const long SerialVersionUID = -8727434096241101194L;
	}

}