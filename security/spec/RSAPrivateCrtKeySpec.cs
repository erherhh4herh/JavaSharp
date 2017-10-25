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

namespace java.security.spec
{

	/// <summary>
	/// This class specifies an RSA private key, as defined in the PKCS#1
	/// standard, using the Chinese Remainder Theorem (CRT) information values for
	/// efficiency.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec </seealso>
	/// <seealso cref= RSAPrivateKeySpec </seealso>
	/// <seealso cref= RSAPublicKeySpec </seealso>

	public class RSAPrivateCrtKeySpec : RSAPrivateKeySpec
	{

		private readonly System.Numerics.BigInteger PublicExponent_Renamed;
		private readonly System.Numerics.BigInteger PrimeP_Renamed;
		private readonly System.Numerics.BigInteger PrimeQ_Renamed;
		private readonly System.Numerics.BigInteger PrimeExponentP_Renamed;
		private readonly System.Numerics.BigInteger PrimeExponentQ_Renamed;
		private readonly System.Numerics.BigInteger CrtCoefficient_Renamed;



	   /// <summary>
	   /// Creates a new {@code RSAPrivateCrtKeySpec}
	   /// given the modulus, publicExponent, privateExponent,
	   /// primeP, primeQ, primeExponentP, primeExponentQ, and
	   /// crtCoefficient as defined in PKCS#1.
	   /// </summary>
	   /// <param name="modulus"> the modulus n </param>
	   /// <param name="publicExponent"> the public exponent e </param>
	   /// <param name="privateExponent"> the private exponent d </param>
	   /// <param name="primeP"> the prime factor p of n </param>
	   /// <param name="primeQ"> the prime factor q of n </param>
	   /// <param name="primeExponentP"> this is d mod (p-1) </param>
	   /// <param name="primeExponentQ"> this is d mod (q-1) </param>
	   /// <param name="crtCoefficient"> the Chinese Remainder Theorem
	   /// coefficient q-1 mod p </param>
		public RSAPrivateCrtKeySpec(System.Numerics.BigInteger modulus, System.Numerics.BigInteger publicExponent, System.Numerics.BigInteger privateExponent, System.Numerics.BigInteger primeP, System.Numerics.BigInteger primeQ, System.Numerics.BigInteger primeExponentP, System.Numerics.BigInteger primeExponentQ, System.Numerics.BigInteger crtCoefficient) : base(modulus, privateExponent)
		{
			this.PublicExponent_Renamed = publicExponent;
			this.PrimeP_Renamed = primeP;
			this.PrimeQ_Renamed = primeQ;
			this.PrimeExponentP_Renamed = primeExponentP;
			this.PrimeExponentQ_Renamed = primeExponentQ;
			this.CrtCoefficient_Renamed = crtCoefficient;
		}

		/// <summary>
		/// Returns the public exponent.
		/// </summary>
		/// <returns> the public exponent </returns>
		public virtual System.Numerics.BigInteger PublicExponent
		{
			get
			{
				return this.PublicExponent_Renamed;
			}
		}

		/// <summary>
		/// Returns the primeP.
		/// </summary>
		/// <returns> the primeP </returns>
		public virtual System.Numerics.BigInteger PrimeP
		{
			get
			{
				return this.PrimeP_Renamed;
			}
		}

		/// <summary>
		/// Returns the primeQ.
		/// </summary>
		/// <returns> the primeQ </returns>
		public virtual System.Numerics.BigInteger PrimeQ
		{
			get
			{
				return this.PrimeQ_Renamed;
			}
		}

		/// <summary>
		/// Returns the primeExponentP.
		/// </summary>
		/// <returns> the primeExponentP </returns>
		public virtual System.Numerics.BigInteger PrimeExponentP
		{
			get
			{
				return this.PrimeExponentP_Renamed;
			}
		}

		/// <summary>
		/// Returns the primeExponentQ.
		/// </summary>
		/// <returns> the primeExponentQ </returns>
		public virtual System.Numerics.BigInteger PrimeExponentQ
		{
			get
			{
				return this.PrimeExponentQ_Renamed;
			}
		}

		/// <summary>
		/// Returns the crtCoefficient.
		/// </summary>
		/// <returns> the crtCoefficient </returns>
		public virtual System.Numerics.BigInteger CrtCoefficient
		{
			get
			{
				return this.CrtCoefficient_Renamed;
			}
		}
	}

}