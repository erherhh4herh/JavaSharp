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
	/// The interface to an RSA private key.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= RSAPrivateCrtKey </seealso>

	public interface RSAPrivateKey : java.security.PrivateKey, RSAKey
	{

		/// <summary>
		/// The type fingerprint that is set to indicate
		/// serialization compatibility with a previous
		/// version of the type.
		/// </summary>

		/// <summary>
		/// Returns the private exponent.
		/// </summary>
		/// <returns> the private exponent </returns>
		System.Numerics.BigInteger PrivateExponent {get;}
	}

	public static class RSAPrivateKey_Fields
	{
		public const long SerialVersionUID = 5187144804936595022L;
	}

}