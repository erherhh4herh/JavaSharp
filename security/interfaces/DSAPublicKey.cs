/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The interface to a DSA public key. DSA (Digital Signature Algorithm)
	/// is defined in NIST's FIPS-186.
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.Signature </seealso>
	/// <seealso cref= DSAKey </seealso>
	/// <seealso cref= DSAPrivateKey
	/// 
	/// @author Benjamin Renaud </seealso>
	public interface DSAPublicKey : DSAKey, java.security.PublicKey
	{

		// Declare serialVersionUID to be compatible with JDK1.1

	   /// <summary>
	   /// The class fingerprint that is set to indicate
	   /// serialization compatibility with a previous
	   /// version of the class.
	   /// </summary>

		/// <summary>
		/// Returns the value of the public key, {@code y}.
		/// </summary>
		/// <returns> the value of the public key, {@code y}. </returns>
		System.Numerics.BigInteger Y {get;}
	}

	public static class DSAPublicKey_Fields
	{
		public const long SerialVersionUID = 1234526332779022332L;
	}

}