/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
namespace java.security.interfaces
{


	/// <summary>
	/// The interface to an elliptic curve (EC) private key.
	/// 
	/// @author Valerie Peng
	/// 
	/// </summary>
	/// <seealso cref= PrivateKey </seealso>
	/// <seealso cref= ECKey
	/// 
	/// @since 1.5 </seealso>
	public interface ECPrivateKey : PrivateKey, ECKey
	{
	   /// <summary>
	   /// The class fingerprint that is set to indicate
	   /// serialization compatibility.
	   /// </summary>

		/// <summary>
		/// Returns the private value S. </summary>
		/// <returns> the private value S. </returns>
		System.Numerics.BigInteger S {get;}
	}

	public static class ECPrivateKey_Fields
	{
		public const long SerialVersionUID = -7896394956925609184L;
	}

}