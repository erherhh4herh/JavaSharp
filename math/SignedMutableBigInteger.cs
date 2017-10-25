/*
 * Copyright (c) 1999, 2007, Oracle and/or its affiliates. All rights reserved.
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

namespace java.math
{

	/// <summary>
	/// A class used to represent multiprecision integers that makes efficient
	/// use of allocated space by allowing a number to occupy only part of
	/// an array so that the arrays do not have to be reallocated as often.
	/// When performing an operation with many iterations the array used to
	/// hold a number is only increased when necessary and does not have to
	/// be the same size as the number it represents. A mutable number allows
	/// calculations to occur on the same number without having to create
	/// a new number for every step of the calculation as occurs with
	/// BigIntegers.
	/// 
	/// Note that SignedMutableBigIntegers only support signed addition and
	/// subtraction. All other operations occur as with MutableBigIntegers.
	/// </summary>
	/// <seealso cref=     BigInteger
	/// @author  Michael McCloskey
	/// @since   1.3 </seealso>

	internal class SignedMutableBigInteger : MutableBigInteger
	{

	   /// <summary>
	   /// The sign of this MutableBigInteger.
	   /// </summary>
		internal int Sign = 1;

		// Constructors

		/// <summary>
		/// The default constructor. An empty MutableBigInteger is created with
		/// a one word capacity.
		/// </summary>
		internal SignedMutableBigInteger() : base()
		{
		}

		/// <summary>
		/// Construct a new MutableBigInteger with a magnitude specified by
		/// the int val.
		/// </summary>
		internal SignedMutableBigInteger(int val) : base(val)
		{
		}

		/// <summary>
		/// Construct a new MutableBigInteger with a magnitude equal to the
		/// specified MutableBigInteger.
		/// </summary>
		internal SignedMutableBigInteger(MutableBigInteger val) : base(val)
		{
		}

	   // Arithmetic Operations

	   /// <summary>
	   /// Signed addition built upon unsigned add and subtract.
	   /// </summary>
		internal virtual void SignedAdd(SignedMutableBigInteger addend)
		{
			if (Sign == addend.Sign)
			{
				Add(addend);
			}
			else
			{
				Sign = Sign * Subtract(addend);
			}

		}

	   /// <summary>
	   /// Signed addition built upon unsigned add and subtract.
	   /// </summary>
		internal virtual void SignedAdd(MutableBigInteger addend)
		{
			if (Sign == 1)
			{
				Add(addend);
			}
			else
			{
				Sign = Sign * Subtract(addend);
			}

		}

	   /// <summary>
	   /// Signed subtraction built upon unsigned add and subtract.
	   /// </summary>
		internal virtual void SignedSubtract(SignedMutableBigInteger addend)
		{
			if (Sign == addend.Sign)
			{
				Sign = Sign * Subtract(addend);
			}
			else
			{
				Add(addend);
			}

		}

	   /// <summary>
	   /// Signed subtraction built upon unsigned add and subtract.
	   /// </summary>
		internal virtual void SignedSubtract(MutableBigInteger addend)
		{
			if (Sign == 1)
			{
				Sign = Sign * Subtract(addend);
			}
			else
			{
				Add(addend);
			}
			if (IntLen == 0)
			{
				 Sign = 1;
			}
		}

		/// <summary>
		/// Print out the first intLen ints of this MutableBigInteger's value
		/// array starting at offset.
		/// </summary>
		public override String ToString()
		{
			return this.ToBigInteger(Sign).ToString();
		}

	}

}