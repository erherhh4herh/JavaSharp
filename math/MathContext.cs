using System;

/*
 * Copyright (c) 2003, 2007, Oracle and/or its affiliates. All rights reserved.
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

/*
 * Portions Copyright IBM Corporation, 1997, 2001. All Rights Reserved.
 */

namespace java.math
{

	/// <summary>
	/// Immutable objects which encapsulate the context settings which
	/// describe certain rules for numerical operators, such as those
	/// implemented by the <seealso cref="BigDecimal"/> class.
	/// 
	/// <para>The base-independent settings are:
	/// <ol>
	/// <li>{@code precision}:
	/// the number of digits to be used for an operation; results are
	/// rounded to this precision
	/// 
	/// <li>{@code roundingMode}:
	/// a <seealso cref="RoundingMode"/> object which specifies the algorithm to be
	/// used for rounding.
	/// </ol>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     BigDecimal </seealso>
	/// <seealso cref=     RoundingMode
	/// @author  Mike Cowlishaw
	/// @author  Joseph D. Darcy
	/// @since 1.5 </seealso>

	[Serializable]
	public sealed class MathContext
	{

		/* ----- Constants ----- */

		// defaults for constructors
		private const int DEFAULT_DIGITS = 9;
		private const RoundingMode DEFAULT_ROUNDINGMODE = RoundingMode.HALF_UP;
		// Smallest values for digits (Maximum is Integer.MAX_VALUE)
		private const int MIN_DIGITS = 0;

		// Serialization version
		private const long SerialVersionUID = 5579720004786848255L;

		/* ----- Public Properties ----- */
		/// <summary>
		///  A {@code MathContext} object whose settings have the values
		///  required for unlimited precision arithmetic.
		///  The values of the settings are:
		///  <code>
		///  precision=0 roundingMode=HALF_UP
		///  </code>
		/// </summary>
		public static readonly MathContext UNLIMITED = new MathContext(0, RoundingMode.HALF_UP);

		/// <summary>
		///  A {@code MathContext} object with a precision setting
		///  matching the IEEE 754R Decimal32 format, 7 digits, and a
		///  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
		///  IEEE 754R default.
		/// </summary>
		public static readonly MathContext DECIMAL32 = new MathContext(7, RoundingMode.HALF_EVEN);

		/// <summary>
		///  A {@code MathContext} object with a precision setting
		///  matching the IEEE 754R Decimal64 format, 16 digits, and a
		///  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
		///  IEEE 754R default.
		/// </summary>
		public static readonly MathContext DECIMAL64 = new MathContext(16, RoundingMode.HALF_EVEN);

		/// <summary>
		///  A {@code MathContext} object with a precision setting
		///  matching the IEEE 754R Decimal128 format, 34 digits, and a
		///  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
		///  IEEE 754R default.
		/// </summary>
		public static readonly MathContext DECIMAL128 = new MathContext(34, RoundingMode.HALF_EVEN);

		/* ----- Shared Properties ----- */
		/// <summary>
		/// The number of digits to be used for an operation.  A value of 0
		/// indicates that unlimited precision (as many digits as are
		/// required) will be used.  Note that leading zeros (in the
		/// coefficient of a number) are never significant.
		/// 
		/// <para>{@code precision} will always be non-negative.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		internal readonly int Precision_Renamed;

		/// <summary>
		/// The rounding algorithm to be used for an operation.
		/// </summary>
		/// <seealso cref= RoundingMode
		/// @serial </seealso>
		internal readonly RoundingMode RoundingMode_Renamed;

		/* ----- Constructors ----- */

		/// <summary>
		/// Constructs a new {@code MathContext} with the specified
		/// precision and the <seealso cref="RoundingMode#HALF_UP HALF_UP"/> rounding
		/// mode.
		/// </summary>
		/// <param name="setPrecision"> The non-negative {@code int} precision setting. </param>
		/// <exception cref="IllegalArgumentException"> if the {@code setPrecision} parameter is less
		///         than zero. </exception>
		public MathContext(int setPrecision) : this(setPrecision, DEFAULT_ROUNDINGMODE)
		{
			return;
		}

		/// <summary>
		/// Constructs a new {@code MathContext} with a specified
		/// precision and rounding mode.
		/// </summary>
		/// <param name="setPrecision"> The non-negative {@code int} precision setting. </param>
		/// <param name="setRoundingMode"> The rounding mode to use. </param>
		/// <exception cref="IllegalArgumentException"> if the {@code setPrecision} parameter is less
		///         than zero. </exception>
		/// <exception cref="NullPointerException"> if the rounding mode argument is {@code null} </exception>
		public MathContext(int setPrecision, RoundingMode setRoundingMode)
		{
			if (setPrecision < MIN_DIGITS)
			{
				throw new IllegalArgumentException("Digits < 0");
			}
			if (setRoundingMode == null)
			{
				throw new NullPointerException("null RoundingMode");
			}

			Precision_Renamed = setPrecision;
			RoundingMode_Renamed = setRoundingMode;
			return;
		}

		/// <summary>
		/// Constructs a new {@code MathContext} from a string.
		/// 
		/// The string must be in the same format as that produced by the
		/// <seealso cref="#toString"/> method.
		/// 
		/// <para>An {@code IllegalArgumentException} is thrown if the precision
		/// section of the string is out of range ({@code < 0}) or the string is
		/// not in the format created by the <seealso cref="#toString"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> The string to be parsed </param>
		/// <exception cref="IllegalArgumentException"> if the precision section is out of range
		/// or of incorrect format </exception>
		/// <exception cref="NullPointerException"> if the argument is {@code null} </exception>
		public MathContext(String val)
		{
			bool bad = false;
			int setPrecision;
			if (val == null)
			{
				throw new NullPointerException("null String");
			}
			try // any error here is a string format problem
			{
				if (!val.StartsWith("precision="))
				{
					throw new RuntimeException();
				}
				int fence = val.IndexOf(' '); // could be -1
				int off = 10; // where value starts
				setPrecision = Convert.ToInt32(val.Substring(10, fence - 10));

				if (!val.StartsWith("roundingMode=", fence+1))
				{
					throw new RuntimeException();
				}
				off = fence + 1 + 13;
				String str = val.Substring(off, val.Length() - off);
				RoundingMode_Renamed = RoundingMode.ValueOf(str);
			}
			catch (RuntimeException)
			{
				throw new IllegalArgumentException("bad string format");
			}

			if (setPrecision < MIN_DIGITS)
			{
				throw new IllegalArgumentException("Digits < 0");
			}
			// the other parameters cannot be invalid if we got here
			Precision_Renamed = setPrecision;
		}

		/// <summary>
		/// Returns the {@code precision} setting.
		/// This value is always non-negative.
		/// </summary>
		/// <returns> an {@code int} which is the value of the {@code precision}
		///         setting </returns>
		public int Precision
		{
			get
			{
				return Precision_Renamed;
			}
		}

		/// <summary>
		/// Returns the roundingMode setting.
		/// This will be one of
		/// <seealso cref=" RoundingMode#CEILING"/>,
		/// <seealso cref=" RoundingMode#DOWN"/>,
		/// <seealso cref=" RoundingMode#FLOOR"/>,
		/// <seealso cref=" RoundingMode#HALF_DOWN"/>,
		/// <seealso cref=" RoundingMode#HALF_EVEN"/>,
		/// <seealso cref=" RoundingMode#HALF_UP"/>,
		/// <seealso cref=" RoundingMode#UNNECESSARY"/>, or
		/// <seealso cref=" RoundingMode#UP"/>.
		/// </summary>
		/// <returns> a {@code RoundingMode} object which is the value of the
		///         {@code roundingMode} setting </returns>

		public RoundingMode RoundingMode
		{
			get
			{
				return RoundingMode_Renamed;
			}
		}

		/// <summary>
		/// Compares this {@code MathContext} with the specified
		/// {@code Object} for equality.
		/// </summary>
		/// <param name="x"> {@code Object} to which this {@code MathContext} is to
		///         be compared. </param>
		/// <returns> {@code true} if and only if the specified {@code Object} is
		///         a {@code MathContext} object which has exactly the same
		///         settings as this object </returns>
		public override bool Equals(Object x)
		{
			MathContext mc;
			if (!(x is MathContext))
			{
				return false;
			}
			mc = (MathContext) x;
			return mc.Precision_Renamed == this.Precision_Renamed && mc.RoundingMode_Renamed == this.RoundingMode_Renamed; // no need for .equals()
		}

		/// <summary>
		/// Returns the hash code for this {@code MathContext}.
		/// </summary>
		/// <returns> hash code for this {@code MathContext} </returns>
		public override int HashCode()
		{
			return this.Precision_Renamed + RoundingMode_Renamed.HashCode() * 59;
		}

		/// <summary>
		/// Returns the string representation of this {@code MathContext}.
		/// The {@code String} returned represents the settings of the
		/// {@code MathContext} object as two space-delimited words
		/// (separated by a single space character, <tt>'&#92;u0020'</tt>,
		/// and with no leading or trailing white space), as follows:
		/// <ol>
		/// <li>
		/// The string {@code "precision="}, immediately followed
		/// by the value of the precision setting as a numeric string as if
		/// generated by the <seealso cref="Integer#toString(int) Integer.toString"/>
		/// method.
		/// 
		/// <li>
		/// The string {@code "roundingMode="}, immediately
		/// followed by the value of the {@code roundingMode} setting as a
		/// word.  This word will be the same as the name of the
		/// corresponding public constant in the <seealso cref="RoundingMode"/>
		/// enum.
		/// </ol>
		/// <para>
		/// For example:
		/// <pre>
		/// precision=9 roundingMode=HALF_UP
		/// </pre>
		/// 
		/// Additional words may be appended to the result of
		/// {@code toString} in the future if more properties are added to
		/// this class.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code String} representing the context settings </returns>
		public override string ToString()
		{
			return "precision=" + Precision_Renamed + " " + "roundingMode=" + RoundingMode_Renamed.ToString();
		}

		// Private methods

		/// <summary>
		/// Reconstitute the {@code MathContext} instance from a stream (that is,
		/// deserialize it).
		/// </summary>
		/// <param name="s"> the stream being read. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject(); // read in all fields
			// validate possibly bad fields
			if (Precision_Renamed < MIN_DIGITS)
			{
				String message = "MathContext: invalid digits in stream";
				throw new java.io.StreamCorruptedException(message);
			}
			if (RoundingMode_Renamed == null)
			{
				String message = "MathContext: null roundingMode in stream";
				throw new java.io.StreamCorruptedException(message);
			}
		}

	}

}