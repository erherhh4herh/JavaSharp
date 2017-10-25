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
	/// This immutable class specifies an elliptic curve private key with
	/// its associated parameters.
	/// </summary>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= ECParameterSpec
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECPrivateKeySpec : KeySpec
	{

		private System.Numerics.BigInteger s;
		private ECParameterSpec @params;

		/// <summary>
		/// Creates a new ECPrivateKeySpec with the specified
		/// parameter values. </summary>
		/// <param name="s"> the private value. </param>
		/// <param name="params"> the associated elliptic curve domain
		/// parameters. </param>
		/// <exception cref="NullPointerException"> if {@code s}
		/// or {@code params} is null. </exception>
		public ECPrivateKeySpec(System.Numerics.BigInteger s, ECParameterSpec @params)
		{
			if (s == null)
			{
				throw new NullPointerException("s is null");
			}
			if (@params == null)
			{
				throw new NullPointerException("params is null");
			}
			this.s = s;
			this.@params = @params;
		}

		/// <summary>
		/// Returns the private value S. </summary>
		/// <returns> the private value S. </returns>
		public virtual System.Numerics.BigInteger S
		{
			get
			{
				return s;
			}
		}

		/// <summary>
		/// Returns the associated elliptic curve domain
		/// parameters. </summary>
		/// <returns> the EC domain parameters. </returns>
		public virtual ECParameterSpec Params
		{
			get
			{
				return @params;
			}
		}
	}

}