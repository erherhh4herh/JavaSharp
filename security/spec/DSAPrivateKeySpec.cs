/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class specifies a DSA private key with its associated parameters.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= DSAPublicKeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec
	/// 
	/// @since 1.2 </seealso>

	public class DSAPrivateKeySpec : KeySpec
	{

		private System.Numerics.BigInteger x;
		private System.Numerics.BigInteger p;
		private System.Numerics.BigInteger q;
		private System.Numerics.BigInteger g;

		/// <summary>
		/// Creates a new DSAPrivateKeySpec with the specified parameter values.
		/// </summary>
		/// <param name="x"> the private key.
		/// </param>
		/// <param name="p"> the prime.
		/// </param>
		/// <param name="q"> the sub-prime.
		/// </param>
		/// <param name="g"> the base. </param>
		public DSAPrivateKeySpec(System.Numerics.BigInteger x, System.Numerics.BigInteger p, System.Numerics.BigInteger q, System.Numerics.BigInteger g)
		{
			this.x = x;
			this.p = p;
			this.q = q;
			this.g = g;
		}

		/// <summary>
		/// Returns the private key {@code x}.
		/// </summary>
		/// <returns> the private key {@code x}. </returns>
		public virtual System.Numerics.BigInteger X
		{
			get
			{
				return this.x;
			}
		}

		/// <summary>
		/// Returns the prime {@code p}.
		/// </summary>
		/// <returns> the prime {@code p}. </returns>
		public virtual System.Numerics.BigInteger P
		{
			get
			{
				return this.p;
			}
		}

		/// <summary>
		/// Returns the sub-prime {@code q}.
		/// </summary>
		/// <returns> the sub-prime {@code q}. </returns>
		public virtual System.Numerics.BigInteger Q
		{
			get
			{
				return this.q;
			}
		}

		/// <summary>
		/// Returns the base {@code g}.
		/// </summary>
		/// <returns> the base {@code g}. </returns>
		public virtual System.Numerics.BigInteger G
		{
			get
			{
				return this.g;
			}
		}
	}

}