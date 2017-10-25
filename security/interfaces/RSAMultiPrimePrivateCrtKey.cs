/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The interface to an RSA multi-prime private key, as defined in the
	/// PKCS#1 v2.1, using the <i>Chinese Remainder Theorem</i>
	/// (CRT) information values.
	/// 
	/// @author Valerie Peng
	/// 
	/// </summary>
	/// <seealso cref= java.security.spec.RSAPrivateKeySpec </seealso>
	/// <seealso cref= java.security.spec.RSAMultiPrimePrivateCrtKeySpec </seealso>
	/// <seealso cref= RSAPrivateKey </seealso>
	/// <seealso cref= RSAPrivateCrtKey
	/// 
	/// @since 1.4 </seealso>

	public interface RSAMultiPrimePrivateCrtKey : RSAPrivateKey
	{

		/// <summary>
		/// The type fingerprint that is set to indicate
		/// serialization compatibility with a previous
		/// version of the type.
		/// </summary>

		/// <summary>
		/// Returns the public exponent.
		/// </summary>
		/// <returns> the public exponent. </returns>
		System.Numerics.BigInteger PublicExponent {get;}

		/// <summary>
		/// Returns the primeP.
		/// </summary>
		/// <returns> the primeP. </returns>
		System.Numerics.BigInteger PrimeP {get;}

		/// <summary>
		/// Returns the primeQ.
		/// </summary>
		/// <returns> the primeQ. </returns>
		System.Numerics.BigInteger PrimeQ {get;}

		/// <summary>
		/// Returns the primeExponentP.
		/// </summary>
		/// <returns> the primeExponentP. </returns>
		System.Numerics.BigInteger PrimeExponentP {get;}

		/// <summary>
		/// Returns the primeExponentQ.
		/// </summary>
		/// <returns> the primeExponentQ. </returns>
		System.Numerics.BigInteger PrimeExponentQ {get;}

		/// <summary>
		/// Returns the crtCoefficient.
		/// </summary>
		/// <returns> the crtCoefficient. </returns>
		System.Numerics.BigInteger CrtCoefficient {get;}

		/// <summary>
		/// Returns the otherPrimeInfo or null if there are only
		/// two prime factors (p and q).
		/// </summary>
		/// <returns> the otherPrimeInfo. </returns>
		RSAOtherPrimeInfo[] OtherPrimeInfo {get;}
	}

	public static class RSAMultiPrimePrivateCrtKey_Fields
	{
		public const long SerialVersionUID = 618058533534628008L;
	}

}