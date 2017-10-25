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

namespace java.security.spec
{

	/// <summary>
	/// This class specifies an RSA multi-prime private key, as defined in the
	/// PKCS#1 v2.1, using the Chinese Remainder Theorem (CRT) information
	/// values for efficiency.
	/// 
	/// @author Valerie Peng
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec </seealso>
	/// <seealso cref= RSAPrivateKeySpec </seealso>
	/// <seealso cref= RSAPublicKeySpec </seealso>
	/// <seealso cref= RSAOtherPrimeInfo
	/// 
	/// @since 1.4 </seealso>

	public class RSAMultiPrimePrivateCrtKeySpec : RSAPrivateKeySpec
	{

		private readonly System.Numerics.BigInteger PublicExponent_Renamed;
		private readonly System.Numerics.BigInteger PrimeP_Renamed;
		private readonly System.Numerics.BigInteger PrimeQ_Renamed;
		private readonly System.Numerics.BigInteger PrimeExponentP_Renamed;
		private readonly System.Numerics.BigInteger PrimeExponentQ_Renamed;
		private readonly System.Numerics.BigInteger CrtCoefficient_Renamed;
		private readonly RSAOtherPrimeInfo[] OtherPrimeInfo_Renamed;

	   /// <summary>
	   /// Creates a new {@code RSAMultiPrimePrivateCrtKeySpec}
	   /// given the modulus, publicExponent, privateExponent,
	   /// primeP, primeQ, primeExponentP, primeExponentQ,
	   /// crtCoefficient, and otherPrimeInfo as defined in PKCS#1 v2.1.
	   /// 
	   /// <para>Note that the contents of {@code otherPrimeInfo}
	   /// are copied to protect against subsequent modification when
	   /// constructing this object.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <param name="modulus"> the modulus n. </param>
	   /// <param name="publicExponent"> the public exponent e. </param>
	   /// <param name="privateExponent"> the private exponent d. </param>
	   /// <param name="primeP"> the prime factor p of n. </param>
	   /// <param name="primeQ"> the prime factor q of n. </param>
	   /// <param name="primeExponentP"> this is d mod (p-1). </param>
	   /// <param name="primeExponentQ"> this is d mod (q-1). </param>
	   /// <param name="crtCoefficient"> the Chinese Remainder Theorem
	   /// coefficient q-1 mod p. </param>
	   /// <param name="otherPrimeInfo"> triplets of the rest of primes, null can be
	   /// specified if there are only two prime factors (p and q). </param>
	   /// <exception cref="NullPointerException"> if any of the parameters, i.e.
	   /// {@code modulus},
	   /// {@code publicExponent}, {@code privateExponent},
	   /// {@code primeP}, {@code primeQ},
	   /// {@code primeExponentP}, {@code primeExponentQ},
	   /// {@code crtCoefficient}, is null. </exception>
	   /// <exception cref="IllegalArgumentException"> if an empty, i.e. 0-length,
	   /// {@code otherPrimeInfo} is specified. </exception>
		public RSAMultiPrimePrivateCrtKeySpec(System.Numerics.BigInteger modulus, System.Numerics.BigInteger publicExponent, System.Numerics.BigInteger privateExponent, System.Numerics.BigInteger primeP, System.Numerics.BigInteger primeQ, System.Numerics.BigInteger primeExponentP, System.Numerics.BigInteger primeExponentQ, System.Numerics.BigInteger crtCoefficient, RSAOtherPrimeInfo[] otherPrimeInfo) : base(modulus, privateExponent)
		{
			if (modulus == null)
			{
				throw new NullPointerException("the modulus parameter must be " + "non-null");
			}
			if (publicExponent == null)
			{
				throw new NullPointerException("the publicExponent parameter " + "must be non-null");
			}
			if (privateExponent == null)
			{
				throw new NullPointerException("the privateExponent parameter " + "must be non-null");
			}
			if (primeP == null)
			{
				throw new NullPointerException("the primeP parameter " + "must be non-null");
			}
			if (primeQ == null)
			{
				throw new NullPointerException("the primeQ parameter " + "must be non-null");
			}
			if (primeExponentP == null)
			{
				throw new NullPointerException("the primeExponentP parameter " + "must be non-null");
			}
			if (primeExponentQ == null)
			{
				throw new NullPointerException("the primeExponentQ parameter " + "must be non-null");
			}
			if (crtCoefficient == null)
			{
				throw new NullPointerException("the crtCoefficient parameter " + "must be non-null");
			}
			this.PublicExponent_Renamed = publicExponent;
			this.PrimeP_Renamed = primeP;
			this.PrimeQ_Renamed = primeQ;
			this.PrimeExponentP_Renamed = primeExponentP;
			this.PrimeExponentQ_Renamed = primeExponentQ;
			this.CrtCoefficient_Renamed = crtCoefficient;
			if (otherPrimeInfo == null)
			{
				this.OtherPrimeInfo_Renamed = null;
			}
			else if (otherPrimeInfo.Length == 0)
			{
				throw new IllegalArgumentException("the otherPrimeInfo " + "parameter must not be empty");
			}
			else
			{
				this.OtherPrimeInfo_Renamed = otherPrimeInfo.clone();
			}
		}

		/// <summary>
		/// Returns the public exponent.
		/// </summary>
		/// <returns> the public exponent. </returns>
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
		/// <returns> the primeP. </returns>
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
		/// <returns> the primeQ. </returns>
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
		/// <returns> the primeExponentP. </returns>
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
		/// <returns> the primeExponentQ. </returns>
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
		/// <returns> the crtCoefficient. </returns>
		public virtual System.Numerics.BigInteger CrtCoefficient
		{
			get
			{
				return this.CrtCoefficient_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the otherPrimeInfo or null if there are
		/// only two prime factors (p and q).
		/// </summary>
		/// <returns> the otherPrimeInfo. Returns a new array each
		/// time this method is called. </returns>
		public virtual RSAOtherPrimeInfo[] OtherPrimeInfo
		{
			get
			{
				if (OtherPrimeInfo_Renamed == null)
				{
					return null;
				}
				return OtherPrimeInfo_Renamed.clone();
			}
		}
	}

}