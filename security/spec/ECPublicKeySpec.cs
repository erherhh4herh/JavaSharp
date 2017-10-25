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
	/// This immutable class specifies an elliptic curve public key with
	/// its associated parameters.
	/// </summary>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= ECPoint </seealso>
	/// <seealso cref= ECParameterSpec
	/// 
	/// @author Valerie Peng
	/// 
	/// @since 1.5 </seealso>
	public class ECPublicKeySpec : KeySpec
	{

		private ECPoint w;
		private ECParameterSpec @params;

		/// <summary>
		/// Creates a new ECPublicKeySpec with the specified
		/// parameter values. </summary>
		/// <param name="w"> the public point. </param>
		/// <param name="params"> the associated elliptic curve domain
		/// parameters. </param>
		/// <exception cref="NullPointerException"> if {@code w}
		/// or {@code params} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code w}
		/// is point at infinity, i.e. ECPoint.POINT_INFINITY </exception>
		public ECPublicKeySpec(ECPoint w, ECParameterSpec @params)
		{
			if (w == null)
			{
				throw new NullPointerException("w is null");
			}
			if (@params == null)
			{
				throw new NullPointerException("params is null");
			}
			if (w == ECPoint.POINT_INFINITY)
			{
				throw new IllegalArgumentException("w is ECPoint.POINT_INFINITY");
			}
			this.w = w;
			this.@params = @params;
		}

		/// <summary>
		/// Returns the public point W. </summary>
		/// <returns> the public point W. </returns>
		public virtual ECPoint W
		{
			get
			{
				return w;
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