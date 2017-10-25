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
	/// This immutable class holds the necessary values needed to represent
	/// an elliptic curve.
	/// </summary>
	/// <seealso cref= ECField </seealso>
	/// <seealso cref= ECFieldFp </seealso>
	/// <seealso cref= ECFieldF2m
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class EllipticCurve
	{

		private readonly ECField Field_Renamed;
		private readonly System.Numerics.BigInteger a;
		private readonly System.Numerics.BigInteger b;
		private readonly sbyte[] Seed_Renamed;

		// Check coefficient c is a valid element in ECField field.
		private static void CheckValidity(ECField field, System.Numerics.BigInteger c, String cName)
		{
			// can only perform check if field is ECFieldFp or ECFieldF2m.
			if (field is ECFieldFp)
			{
				System.Numerics.BigInteger p = ((ECFieldFp)field).P;
				if (p.CompareTo(c) != 1)
				{
					throw new IllegalArgumentException(cName + " is too large");
				}
				else if (c.signum() < 0)
				{
					throw new IllegalArgumentException(cName + " is negative");
				}
			}
			else if (field is ECFieldF2m)
			{
				int m = ((ECFieldF2m)field).M;
				if (c.bitLength() > m)
				{
					throw new IllegalArgumentException(cName + " is too large");
				}
			}
		}

		/// <summary>
		/// Creates an elliptic curve with the specified elliptic field
		/// {@code field} and the coefficients {@code a} and
		/// {@code b}. </summary>
		/// <param name="field"> the finite field that this elliptic curve is over. </param>
		/// <param name="a"> the first coefficient of this elliptic curve. </param>
		/// <param name="b"> the second coefficient of this elliptic curve. </param>
		/// <exception cref="NullPointerException"> if {@code field},
		/// {@code a}, or {@code b} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code a}
		/// or {@code b} is not null and not in {@code field}. </exception>
		public EllipticCurve(ECField field, System.Numerics.BigInteger a, System.Numerics.BigInteger b) : this(field, a, b, null)
		{
		}

		/// <summary>
		/// Creates an elliptic curve with the specified elliptic field
		/// {@code field}, the coefficients {@code a} and
		/// {@code b}, and the {@code seed} used for curve generation. </summary>
		/// <param name="field"> the finite field that this elliptic curve is over. </param>
		/// <param name="a"> the first coefficient of this elliptic curve. </param>
		/// <param name="b"> the second coefficient of this elliptic curve. </param>
		/// <param name="seed"> the bytes used during curve generation for later
		/// validation. Contents of this array are copied to protect against
		/// subsequent modification. </param>
		/// <exception cref="NullPointerException"> if {@code field},
		/// {@code a}, or {@code b} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code a}
		/// or {@code b} is not null and not in {@code field}. </exception>
		public EllipticCurve(ECField field, System.Numerics.BigInteger a, System.Numerics.BigInteger b, sbyte[] seed)
		{
			if (field == null)
			{
				throw new NullPointerException("field is null");
			}
			if (a == null)
			{
				throw new NullPointerException("first coefficient is null");
			}
			if (b == null)
			{
				throw new NullPointerException("second coefficient is null");
			}
			CheckValidity(field, a, "first coefficient");
			CheckValidity(field, b, "second coefficient");
			this.Field_Renamed = field;
			this.a = a;
			this.b = b;
			if (seed != null)
			{
				this.Seed_Renamed = seed.clone();
			}
			else
			{
				this.Seed_Renamed = null;
			}
		}

		/// <summary>
		/// Returns the finite field {@code field} that this
		/// elliptic curve is over. </summary>
		/// <returns> the field {@code field} that this curve
		/// is over. </returns>
		public virtual ECField Field
		{
			get
			{
				return Field_Renamed;
			}
		}

		/// <summary>
		/// Returns the first coefficient {@code a} of the
		/// elliptic curve. </summary>
		/// <returns> the first coefficient {@code a}. </returns>
		public virtual System.Numerics.BigInteger A
		{
			get
			{
				return a;
			}
		}

		/// <summary>
		/// Returns the second coefficient {@code b} of the
		/// elliptic curve. </summary>
		/// <returns> the second coefficient {@code b}. </returns>
		public virtual System.Numerics.BigInteger B
		{
			get
			{
				return b;
			}
		}

		/// <summary>
		/// Returns the seeding bytes {@code seed} used
		/// during curve generation. May be null if not specified. </summary>
		/// <returns> the seeding bytes {@code seed}. A new
		/// array is returned each time this method is called. </returns>
		public virtual sbyte[] Seed
		{
			get
			{
				if (Seed_Renamed == null)
				{
					return null;
				}
				else
				{
					return Seed_Renamed.clone();
				}
			}
		}

		/// <summary>
		/// Compares this elliptic curve for equality with the
		/// specified object. </summary>
		/// <param name="obj"> the object to be compared. </param>
		/// <returns> true if {@code obj} is an instance of
		/// EllipticCurve and the field, A, and B match, false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is EllipticCurve)
			{
				EllipticCurve curve = (EllipticCurve) obj;
				if ((Field_Renamed.Equals(curve.Field_Renamed)) && (a.Equals(curve.a)) && (b.Equals(curve.b)))
				{
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code value for this elliptic curve. </summary>
		/// <returns> a hash code value computed from the hash codes of the field, A,
		/// and B, as follows:
		/// <pre>{@code
		///     (field.hashCode() << 6) + (a.hashCode() << 4) + (b.hashCode() << 2)
		/// }</pre> </returns>
		public override int HashCode()
		{
			return (Field_Renamed.HashCode() << 6 + (a.HashCode() << 4) + (b.HashCode() << 2));
		}
	}

}