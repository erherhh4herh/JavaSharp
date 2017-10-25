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
	/// This immutable class represents a point on an elliptic curve (EC)
	/// in affine coordinates. Other coordinate systems can
	/// extend this class to represent this point in other
	/// coordinates.
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5
	/// </summary>
	public class ECPoint
	{

		private readonly System.Numerics.BigInteger x;
		private readonly System.Numerics.BigInteger y;

		/// <summary>
		/// This defines the point at infinity.
		/// </summary>
		public static readonly ECPoint POINT_INFINITY = new ECPoint();

		// private constructor for constructing point at infinity
		private ECPoint()
		{
			this.x = null;
			this.y = null;
		}

		/// <summary>
		/// Creates an ECPoint from the specified affine x-coordinate
		/// {@code x} and affine y-coordinate {@code y}. </summary>
		/// <param name="x"> the affine x-coordinate. </param>
		/// <param name="y"> the affine y-coordinate. </param>
		/// <exception cref="NullPointerException"> if {@code x} or
		/// {@code y} is null. </exception>
		public ECPoint(System.Numerics.BigInteger x, System.Numerics.BigInteger y)
		{
			if ((x == null) || (y == null))
			{
				throw new NullPointerException("affine coordinate x or y is null");
			}
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Returns the affine x-coordinate {@code x}.
		/// Note: POINT_INFINITY has a null affine x-coordinate. </summary>
		/// <returns> the affine x-coordinate. </returns>
		public virtual System.Numerics.BigInteger AffineX
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Returns the affine y-coordinate {@code y}.
		/// Note: POINT_INFINITY has a null affine y-coordinate. </summary>
		/// <returns> the affine y-coordinate. </returns>
		public virtual System.Numerics.BigInteger AffineY
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Compares this elliptic curve point for equality with
		/// the specified object. </summary>
		/// <param name="obj"> the object to be compared. </param>
		/// <returns> true if {@code obj} is an instance of
		/// ECPoint and the affine coordinates match, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (this == POINT_INFINITY)
			{
				return false;
			}
			if (obj is ECPoint)
			{
				return ((x.Equals(((ECPoint)obj).x)) && (y.Equals(((ECPoint)obj).y)));
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code value for this elliptic curve point. </summary>
		/// <returns> a hash code value. </returns>
		public override int HashCode()
		{
			if (this == POINT_INFINITY)
			{
				return 0;
			}
			return x.HashCode() << 5 + y.HashCode();
		}
	}

}