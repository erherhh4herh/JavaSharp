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
	/// This class specifies a DSA public key with its associated parameters.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= DSAPrivateKeySpec </seealso>
	/// <seealso cref= X509EncodedKeySpec
	/// 
	/// @since 1.2 </seealso>

	public class DSAPublicKeySpec : KeySpec
	{

		private System.Numerics.BigInteger y;
		private System.Numerics.BigInteger p;
		private System.Numerics.BigInteger q;
		private System.Numerics.BigInteger g;

		/// <summary>
		/// Creates a new DSAPublicKeySpec with the specified parameter values.
		/// </summary>
		/// <param name="y"> the public key.
		/// </param>
		/// <param name="p"> the prime.
		/// </param>
		/// <param name="q"> the sub-prime.
		/// </param>
		/// <param name="g"> the base. </param>
		public DSAPublicKeySpec(System.Numerics.BigInteger y, System.Numerics.BigInteger p, System.Numerics.BigInteger q, System.Numerics.BigInteger g)
		{
			this.y = y;
			this.p = p;
			this.q = q;
			this.g = g;
		}

		/// <summary>
		/// Returns the public key {@code y}.
		/// </summary>
		/// <returns> the public key {@code y}. </returns>
		public virtual System.Numerics.BigInteger Y
		{
			get
			{
				return this.y;
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