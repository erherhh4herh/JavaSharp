/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This immutable class specifies the set of parameters used for
	/// generating DSA parameters as specified in
	/// <a href="http://csrc.nist.gov/publications/fips/fips186-3/fips_186-3.pdf">FIPS 186-3 Digital Signature Standard (DSS)</a>.
	/// </summary>
	/// <seealso cref= AlgorithmParameterSpec
	/// 
	/// @since 8 </seealso>
	public sealed class DSAGenParameterSpec : AlgorithmParameterSpec
	{

		private readonly int PLen;
		private readonly int QLen;
		private readonly int SeedLen;

		/// <summary>
		/// Creates a domain parameter specification for DSA parameter
		/// generation using {@code primePLen} and {@code subprimeQLen}.
		/// The value of {@code subprimeQLen} is also used as the default
		/// length of the domain parameter seed in bits. </summary>
		/// <param name="primePLen"> the desired length of the prime P in bits. </param>
		/// <param name="subprimeQLen"> the desired length of the sub-prime Q in bits. </param>
		/// <exception cref="IllegalArgumentException"> if {@code primePLen}
		/// or {@code subprimeQLen} is illegal per the specification of
		/// FIPS 186-3. </exception>
		public DSAGenParameterSpec(int primePLen, int subprimeQLen) : this(primePLen, subprimeQLen, subprimeQLen)
		{
		}

		/// <summary>
		/// Creates a domain parameter specification for DSA parameter
		/// generation using {@code primePLen}, {@code subprimeQLen},
		/// and {@code seedLen}. </summary>
		/// <param name="primePLen"> the desired length of the prime P in bits. </param>
		/// <param name="subprimeQLen"> the desired length of the sub-prime Q in bits. </param>
		/// <param name="seedLen"> the desired length of the domain parameter seed in bits,
		/// shall be equal to or greater than {@code subprimeQLen}. </param>
		/// <exception cref="IllegalArgumentException"> if {@code primePLenLen},
		/// {@code subprimeQLen}, or {@code seedLen} is illegal per the
		/// specification of FIPS 186-3. </exception>
		public DSAGenParameterSpec(int primePLen, int subprimeQLen, int seedLen)
		{
			switch (primePLen)
			{
			case 1024:
				if (subprimeQLen != 160)
				{
					throw new IllegalArgumentException("subprimeQLen must be 160 when primePLen=1024");
				}
				break;
			case 2048:
				if (subprimeQLen != 224 && subprimeQLen != 256)
				{
				   throw new IllegalArgumentException("subprimeQLen must be 224 or 256 when primePLen=2048");
				}
				break;
			case 3072:
				if (subprimeQLen != 256)
				{
					throw new IllegalArgumentException("subprimeQLen must be 256 when primePLen=3072");
				}
				break;
			default:
				throw new IllegalArgumentException("primePLen must be 1024, 2048, or 3072");
			}
			if (seedLen < subprimeQLen)
			{
				throw new IllegalArgumentException("seedLen must be equal to or greater than subprimeQLen");
			}
			this.PLen = primePLen;
			this.QLen = subprimeQLen;
			this.SeedLen = seedLen;
		}

		/// <summary>
		/// Returns the desired length of the prime P of the
		/// to-be-generated DSA domain parameters in bits. </summary>
		/// <returns> the length of the prime P. </returns>
		public int PrimePLength
		{
			get
			{
				return PLen;
			}
		}

		/// <summary>
		/// Returns the desired length of the sub-prime Q of the
		/// to-be-generated DSA domain parameters in bits. </summary>
		/// <returns> the length of the sub-prime Q. </returns>
		public int SubprimeQLength
		{
			get
			{
				return QLen;
			}
		}

		/// <summary>
		/// Returns the desired length of the domain parameter seed in bits. </summary>
		/// <returns> the length of the domain parameter seed. </returns>
		public int SeedLength
		{
			get
			{
				return SeedLen;
			}
		}
	}

}