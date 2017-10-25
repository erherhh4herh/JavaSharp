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
	/// This class specifies the set of parameters used with the DSA algorithm.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= AlgorithmParameterSpec
	/// 
	/// @since 1.2 </seealso>

	public class DSAParameterSpec : AlgorithmParameterSpec, java.security.interfaces.DSAParams
	{

		internal System.Numerics.BigInteger p;
		internal System.Numerics.BigInteger q;
		internal System.Numerics.BigInteger g;

		/// <summary>
		/// Creates a new DSAParameterSpec with the specified parameter values.
		/// </summary>
		/// <param name="p"> the prime.
		/// </param>
		/// <param name="q"> the sub-prime.
		/// </param>
		/// <param name="g"> the base. </param>
		public DSAParameterSpec(System.Numerics.BigInteger p, System.Numerics.BigInteger q, System.Numerics.BigInteger g)
		{
			this.p = p;
			this.q = q;
			this.g = g;
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