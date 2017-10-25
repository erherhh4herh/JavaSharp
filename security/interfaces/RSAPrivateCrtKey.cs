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
	/// The interface to an RSA private key, as defined in the PKCS#1 standard,
	/// using the <i>Chinese Remainder Theorem</i> (CRT) information values.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= RSAPrivateKey </seealso>

	public interface RSAPrivateCrtKey : RSAPrivateKey
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

		/// <summary>
		/// Returns the primeP.
		/// </summary>
		/// <returns> the primeP </returns>
		System.Numerics.BigInteger PrimeP {get;}

		/// <summary>
		/// Returns the primeQ.
		/// </summary>
		/// <returns> the primeQ </returns>
		System.Numerics.BigInteger PrimeQ {get;}

		/// <summary>
		/// Returns the primeExponentP.
		/// </summary>
		/// <returns> the primeExponentP </returns>
		System.Numerics.BigInteger PrimeExponentP {get;}

		/// <summary>
		/// Returns the primeExponentQ.
		/// </summary>
		/// <returns> the primeExponentQ </returns>
		System.Numerics.BigInteger PrimeExponentQ {get;}

		/// <summary>
		/// Returns the crtCoefficient.
		/// </summary>
		/// <returns> the crtCoefficient </returns>
		System.Numerics.BigInteger CrtCoefficient {get;}
	}

	public static class RSAPrivateCrtKey_Fields
	{
		public const long SerialVersionUID = -5682214253527700368L;
	}

}