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
	/// This class specifies an RSA private key.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec </seealso>
	/// <seealso cref= RSAPublicKeySpec </seealso>
	/// <seealso cref= RSAPrivateCrtKeySpec </seealso>

	public class RSAPrivateKeySpec : KeySpec
	{

		private System.Numerics.BigInteger Modulus_Renamed;
		private System.Numerics.BigInteger PrivateExponent_Renamed;

		/// <summary>
		/// Creates a new RSAPrivateKeySpec.
		/// </summary>
		/// <param name="modulus"> the modulus </param>
		/// <param name="privateExponent"> the private exponent </param>
		public RSAPrivateKeySpec(System.Numerics.BigInteger modulus, System.Numerics.BigInteger privateExponent)
		{
			this.Modulus_Renamed = modulus;
			this.PrivateExponent_Renamed = privateExponent;
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
		/// Returns the private exponent.
		/// </summary>
		/// <returns> the private exponent </returns>
		public virtual System.Numerics.BigInteger PrivateExponent
		{
			get
			{
				return this.PrivateExponent_Renamed;
			}
		}
	}

}