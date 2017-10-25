/*
 * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// This class specifies an RSA public key.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= X509EncodedKeySpec </seealso>
	/// <seealso cref= RSAPrivateKeySpec </seealso>
	/// <seealso cref= RSAPrivateCrtKeySpec </seealso>

	public class RSAPublicKeySpec : KeySpec
	{

		private System.Numerics.BigInteger Modulus_Renamed;
		private System.Numerics.BigInteger PublicExponent_Renamed;

		/// <summary>
		/// Creates a new RSAPublicKeySpec.
		/// </summary>
		/// <param name="modulus"> the modulus </param>
		/// <param name="publicExponent"> the public exponent </param>
		public RSAPublicKeySpec(System.Numerics.BigInteger modulus, System.Numerics.BigInteger publicExponent)
		{
			this.Modulus_Renamed = modulus;
			this.PublicExponent_Renamed = publicExponent;
		}

		/// <summary>
		/// Returns the modulus.
		/// </summary>
		/// <returns> the modulus </returns>
		public virtual System.Numerics.BigInteger Modulus
		{
			get
			{
				return this.Modulus_Renamed;
			}
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
	}

}