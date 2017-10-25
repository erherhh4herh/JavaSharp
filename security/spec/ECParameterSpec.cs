/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This immutable class specifies the set of domain parameters
	/// used with elliptic curve cryptography (ECC).
	/// </summary>
	/// <seealso cref= AlgorithmParameterSpec
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECParameterSpec : AlgorithmParameterSpec
	{

		private readonly EllipticCurve Curve_Renamed;
		private readonly ECPoint g;
		private readonly System.Numerics.BigInteger n;
		private readonly int h;

		/// <summary>
		/// Creates elliptic curve domain parameters based on the
		/// specified values. </summary>
		/// <param name="curve"> the elliptic curve which this parameter
		/// defines. </param>
		/// <param name="g"> the generator which is also known as the base point. </param>
		/// <param name="n"> the order of the generator {@code g}. </param>
		/// <param name="h"> the cofactor. </param>
		/// <exception cref="NullPointerException"> if {@code curve},
		/// {@code g}, or {@code n} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code n}
		/// or {@code h} is not positive. </exception>
		public ECParameterSpec(EllipticCurve curve, ECPoint g, System.Numerics.BigInteger n, int h)
		{
			if (curve == null)
			{
				throw new NullPointerException("curve is null");
			}
			if (g == null)
			{
				throw new NullPointerException("g is null");
			}
			if (n == null)
			{
				throw new NullPointerException("n is null");
			}
			if (n.signum() != 1)
			{
				throw new IllegalArgumentException("n is not positive");
			}
			if (h <= 0)
			{
				throw new IllegalArgumentException("h is not positive");
			}
			this.Curve_Renamed = curve;
			this.g = g;
			this.n = n;
			this.h = h;
		}

		/// <summary>
		/// Returns the elliptic curve that this parameter defines. </summary>
		/// <returns> the elliptic curve that this parameter defines. </returns>
		public virtual EllipticCurve Curve
		{
			get
			{
				return Curve_Renamed;
			}
		}

		/// <summary>
		/// Returns the generator which is also known as the base point. </summary>
		/// <returns> the generator which is also known as the base point. </returns>
		public virtual ECPoint Generator
		{
			get
			{
				return g;
			}
		}

		/// <summary>
		/// Returns the order of the generator. </summary>
		/// <returns> the order of the generator. </returns>
		public virtual System.Numerics.BigInteger Order
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Returns the cofactor. </summary>
		/// <returns> the cofactor. </returns>
		public virtual int Cofactor
		{
			get
			{
				return h;
			}
		}
	}

}