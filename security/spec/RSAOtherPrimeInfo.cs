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
	/// This class represents the triplet (prime, exponent, and coefficient)
	/// inside RSA's OtherPrimeInfo structure, as defined in the PKCS#1 v2.1.
	/// The ASN.1 syntax of RSA's OtherPrimeInfo is as follows:
	/// 
	/// <pre>
	/// OtherPrimeInfo ::= SEQUENCE {
	///   prime INTEGER,
	///   exponent INTEGER,
	///   coefficient INTEGER
	///   }
	/// 
	/// </pre>
	/// 
	/// @author Valerie Peng
	/// 
	/// </summary>
	/// <seealso cref= RSAPrivateCrtKeySpec </seealso>
	/// <seealso cref= java.security.interfaces.RSAMultiPrimePrivateCrtKey
	/// 
	/// @since 1.4 </seealso>

	public class RSAOtherPrimeInfo
	{

		private System.Numerics.BigInteger Prime_Renamed;
		private System.Numerics.BigInteger PrimeExponent;
		private System.Numerics.BigInteger CrtCoefficient_Renamed;


	   /// <summary>
	   /// Creates a new {@code RSAOtherPrimeInfo}
	   /// given the prime, primeExponent, and
	   /// crtCoefficient as defined in PKCS#1.
	   /// </summary>
	   /// <param name="prime"> the prime factor of n. </param>
	   /// <param name="primeExponent"> the exponent. </param>
	   /// <param name="crtCoefficient"> the Chinese Remainder Theorem
	   /// coefficient. </param>
	   /// <exception cref="NullPointerException"> if any of the parameters, i.e.
	   /// {@code prime}, {@code primeExponent},
	   /// {@code crtCoefficient}, is null.
	   ///  </exception>
		public RSAOtherPrimeInfo(System.Numerics.BigInteger prime, System.Numerics.BigInteger primeExponent, System.Numerics.BigInteger crtCoefficient)
		{
			if (prime == null)
			{
				throw new NullPointerException("the prime parameter must be " + "non-null");
			}
			if (primeExponent == null)
			{
				throw new NullPointerException("the primeExponent parameter " + "must be non-null");
			}
			if (crtCoefficient == null)
			{
				throw new NullPointerException("the crtCoefficient parameter " + "must be non-null");
			}
			this.Prime_Renamed = prime;
			this.PrimeExponent = primeExponent;
			this.CrtCoefficient_Renamed = crtCoefficient;
		}

		/// <summary>
		/// Returns the prime.
		/// </summary>
		/// <returns> the prime. </returns>
		public System.Numerics.BigInteger Prime
		{
			get
			{
				return this.Prime_Renamed;
			}
		}

		/// <summary>
		/// Returns the prime's exponent.
		/// </summary>
		/// <returns> the primeExponent. </returns>
		public System.Numerics.BigInteger Exponent
		{
			get
			{
				return this.PrimeExponent;
			}
		}

		/// <summary>
		/// Returns the prime's crtCoefficient.
		/// </summary>
		/// <returns> the crtCoefficient. </returns>
		public System.Numerics.BigInteger CrtCoefficient
		{
			get
			{
				return this.CrtCoefficient_Renamed;
			}
		}
	}

}