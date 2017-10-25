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
	/// This immutable class defines an elliptic curve (EC) prime
	/// finite field.
	/// </summary>
	/// <seealso cref= ECField
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECFieldFp : ECField
	{

		private System.Numerics.BigInteger p;

		/// <summary>
		/// Creates an elliptic curve prime finite field
		/// with the specified prime {@code p}. </summary>
		/// <param name="p"> the prime. </param>
		/// <exception cref="NullPointerException"> if {@code p} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code p}
		/// is not positive. </exception>
		public ECFieldFp(System.Numerics.BigInteger p)
		{
			if (p.signum() != 1)
			{
				throw new IllegalArgumentException("p is not positive");
			}
			this.p = p;
		}

		/// <summary>
		/// Returns the field size in bits which is size of prime p
		/// for this prime finite field. </summary>
		/// <returns> the field size in bits. </returns>
		public virtual int FieldSize
		{
			get
			{
				return p.bitLength();
			}
		};

		/// <summary>
		/// Returns the prime {@code p} of this prime finite field. </summary>
		/// <returns> the prime. </returns>
		public virtual System.Numerics.BigInteger P
		{
			get
			{
				return p;
			}
		}

		/// <summary>
		/// Compares this prime finite field for equality with the
		/// specified object. </summary>
		/// <param name="obj"> the object to be compared. </param>
		/// <returns> true if {@code obj} is an instance
		/// of ECFieldFp and the prime value match, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is ECFieldFp)
			{
				return (p.Equals(((ECFieldFp)obj).p));
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code value for this prime finite field. </summary>
		/// <returns> a hash code value. </returns>
		public override int HashCode()
		{
			return p.HashCode();
		}
	}

}