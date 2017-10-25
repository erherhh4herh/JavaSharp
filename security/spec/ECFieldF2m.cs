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
	/// This immutable class defines an elliptic curve (EC)
	/// characteristic 2 finite field.
	/// </summary>
	/// <seealso cref= ECField
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECFieldF2m : ECField
	{

		private int m;
		private int[] Ks;
		private System.Numerics.BigInteger Rp;

		/// <summary>
		/// Creates an elliptic curve characteristic 2 finite
		/// field which has 2^{@code m} elements with normal basis. </summary>
		/// <param name="m"> with 2^{@code m} being the number of elements. </param>
		/// <exception cref="IllegalArgumentException"> if {@code m}
		/// is not positive. </exception>
		public ECFieldF2m(int m)
		{
			if (m <= 0)
			{
				throw new IllegalArgumentException("m is not positive");
			}
			this.m = m;
			this.Ks = null;
			this.Rp = null;
		}

		/// <summary>
		/// Creates an elliptic curve characteristic 2 finite
		/// field which has 2^{@code m} elements with
		/// polynomial basis.
		/// The reduction polynomial for this field is based
		/// on {@code rp} whose i-th bit corresponds to
		/// the i-th coefficient of the reduction polynomial.<para>
		/// Note: A valid reduction polynomial is either a
		/// trinomial (X^{@code m} + X^{@code k} + 1
		/// with {@code m} &gt; {@code k} &gt;= 1) or a
		/// pentanomial (X^{@code m} + X^{@code k3}
		/// + X^{@code k2} + X^{@code k1} + 1 with
		/// {@code m} &gt; {@code k3} &gt; {@code k2}
		/// &gt; {@code k1} &gt;= 1).
		/// </para>
		/// </summary>
		/// <param name="m"> with 2^{@code m} being the number of elements. </param>
		/// <param name="rp"> the BigInteger whose i-th bit corresponds to
		/// the i-th coefficient of the reduction polynomial. </param>
		/// <exception cref="NullPointerException"> if {@code rp} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code m}
		/// is not positive, or {@code rp} does not represent
		/// a valid reduction polynomial. </exception>
		public ECFieldF2m(int m, System.Numerics.BigInteger rp)
		{
			// check m and rp
			this.m = m;
			this.Rp = rp;
			if (m <= 0)
			{
				throw new IllegalArgumentException("m is not positive");
			}
			int bitCount = this.Rp.bitCount();
			if (!this.Rp.testBit(0) || !this.Rp.testBit(m) || ((bitCount != 3) && (bitCount != 5)))
			{
				throw new IllegalArgumentException("rp does not represent a valid reduction polynomial");
			}
			// convert rp into ks
			System.Numerics.BigInteger temp = this.Rp.clearBit(0).clearBit(m);
			this.Ks = new int[bitCount - 2];
			for (int i = this.Ks.Length - 1; i >= 0; i--)
			{
				int index = temp.LowestSetBit;
				this.Ks[i] = index;
				temp = temp.clearBit(index);
			}
		}

		/// <summary>
		/// Creates an elliptic curve characteristic 2 finite
		/// field which has 2^{@code m} elements with
		/// polynomial basis. The reduction polynomial for this
		/// field is based on {@code ks} whose content
		/// contains the order of the middle term(s) of the
		/// reduction polynomial.
		/// Note: A valid reduction polynomial is either a
		/// trinomial (X^{@code m} + X^{@code k} + 1
		/// with {@code m} &gt; {@code k} &gt;= 1) or a
		/// pentanomial (X^{@code m} + X^{@code k3}
		/// + X^{@code k2} + X^{@code k1} + 1 with
		/// {@code m} &gt; {@code k3} &gt; {@code k2}
		/// &gt; {@code k1} &gt;= 1), so {@code ks} should
		/// have length 1 or 3. </summary>
		/// <param name="m"> with 2^{@code m} being the number of elements. </param>
		/// <param name="ks"> the order of the middle term(s) of the
		/// reduction polynomial. Contents of this array are copied
		/// to protect against subsequent modification. </param>
		/// <exception cref="NullPointerException"> if {@code ks} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if{@code m}
		/// is not positive, or the length of {@code ks}
		/// is neither 1 nor 3, or values in {@code ks}
		/// are not between {@code m}-1 and 1 (inclusive)
		/// and in descending order. </exception>
		public ECFieldF2m(int m, int[] ks)
		{
			// check m and ks
			this.m = m;
			this.Ks = ks.clone();
			if (m <= 0)
			{
				throw new IllegalArgumentException("m is not positive");
			}
			if ((this.Ks.Length != 1) && (this.Ks.Length != 3))
			{
				throw new IllegalArgumentException("length of ks is neither 1 nor 3");
			}
			for (int i = 0; i < this.Ks.Length; i++)
			{
				if ((this.Ks[i] < 1) || (this.Ks[i] > m - 1))
				{
					throw new IllegalArgumentException("ks[" + i + "] is out of range");
				}
				if ((i != 0) && (this.Ks[i] >= this.Ks[i - 1]))
				{
					throw new IllegalArgumentException("values in ks are not in descending order");
				}
			}
			// convert ks into rp
			this.Rp = System.Numerics.BigInteger.ONE;
			this.Rp = Rp.setBit(m);
			for (int j = 0; j < this.Ks.Length; j++)
			{
				Rp = Rp.setBit(this.Ks[j]);
			}
		}

		/// <summary>
		/// Returns the field size in bits which is {@code m}
		/// for this characteristic 2 finite field. </summary>
		/// <returns> the field size in bits. </returns>
		public virtual int FieldSize
		{
			get
			{
				return m;
			}
		}

		/// <summary>
		/// Returns the value {@code m} of this characteristic
		/// 2 finite field. </summary>
		/// <returns> {@code m} with 2^{@code m} being the
		/// number of elements. </returns>
		public virtual int M
		{
			get
			{
				return m;
			}
		}

		/// <summary>
		/// Returns a BigInteger whose i-th bit corresponds to the
		/// i-th coefficient of the reduction polynomial for polynomial
		/// basis or null for normal basis. </summary>
		/// <returns> a BigInteger whose i-th bit corresponds to the
		/// i-th coefficient of the reduction polynomial for polynomial
		/// basis or null for normal basis. </returns>
		public virtual System.Numerics.BigInteger ReductionPolynomial
		{
			get
			{
				return Rp;
			}
		}

		/// <summary>
		/// Returns an integer array which contains the order of the
		/// middle term(s) of the reduction polynomial for polynomial
		/// basis or null for normal basis. </summary>
		/// <returns> an integer array which contains the order of the
		/// middle term(s) of the reduction polynomial for polynomial
		/// basis or null for normal basis. A new array is returned
		/// each time this method is called. </returns>
		public virtual int[] MidTermsOfReductionPolynomial
		{
			get
			{
				if (Ks == null)
				{
					return null;
				}
				else
				{
					return Ks.clone();
				}
			}
		}

		/// <summary>
		/// Compares this finite field for equality with the
		/// specified object. </summary>
		/// <param name="obj"> the object to be compared. </param>
		/// <returns> true if {@code obj} is an instance
		/// of ECFieldF2m and both {@code m} and the reduction
		/// polynomial match, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is ECFieldF2m)
			{
				// no need to compare rp here since ks and rp
				// should be equivalent
				return ((m == ((ECFieldF2m)obj).m) && (Arrays.Equals(Ks, ((ECFieldF2m) obj).Ks)));
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code value for this characteristic 2
		/// finite field. </summary>
		/// <returns> a hash code value. </returns>
		public override int HashCode()
		{
			int value = m << 5;
			value += (Rp == null? 0:Rp.HashCode());
			// no need to involve ks here since ks and rp
			// should be equivalent.
			return value;
		}
	}

}