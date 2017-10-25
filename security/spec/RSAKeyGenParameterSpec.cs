/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class specifies the set of parameters used to generate an RSA
	/// key pair.
	/// 
	/// @author Jan Luehe
	/// </summary>
	/// <seealso cref= java.security.KeyPairGenerator#initialize(java.security.spec.AlgorithmParameterSpec)
	/// 
	/// @since 1.3 </seealso>

	public class RSAKeyGenParameterSpec : AlgorithmParameterSpec
	{

		private int Keysize_Renamed;
		private System.Numerics.BigInteger PublicExponent_Renamed;

		/// <summary>
		/// The public-exponent value F0 = 3.
		/// </summary>
		public static readonly System.Numerics.BigInteger F0 = System.Numerics.BigInteger.ValueOf(3);

		/// <summary>
		/// The public exponent-value F4 = 65537.
		/// </summary>
		public static readonly System.Numerics.BigInteger F4 = System.Numerics.BigInteger.ValueOf(65537);

		/// <summary>
		/// Constructs a new {@code RSAParameterSpec} object from the
		/// given keysize and public-exponent value.
		/// </summary>
		/// <param name="keysize"> the modulus size (specified in number of bits) </param>
		/// <param name="publicExponent"> the public exponent </param>
		public RSAKeyGenParameterSpec(int keysize, System.Numerics.BigInteger publicExponent)
		{
			this.Keysize_Renamed = keysize;
			this.PublicExponent_Renamed = publicExponent;
		}

		/// <summary>
		/// Returns the keysize.
		/// </summary>
		/// <returns> the keysize. </returns>
		public virtual int Keysize
		{
			get
			{
				return Keysize_Renamed;
			}
		}

		/// <summary>
		/// Returns the public-exponent value.
		/// </summary>
		/// <returns> the public-exponent value. </returns>
		public virtual System.Numerics.BigInteger PublicExponent
		{
			get
			{
				return PublicExponent_Renamed;
			}
		}
	}

}