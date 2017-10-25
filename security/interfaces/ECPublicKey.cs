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
	/// The interface to an elliptic curve (EC) public key.
	/// 
	/// @author Valerie Peng
	/// 
	/// </summary>
	/// <seealso cref= PublicKey </seealso>
	/// <seealso cref= ECKey </seealso>
	/// <seealso cref= java.security.spec.ECPoint
	/// 
	/// @since 1.5 </seealso>
	public interface ECPublicKey : PublicKey, ECKey
	{

	   /// <summary>
	   /// The class fingerprint that is set to indicate
	   /// serialization compatibility.
	   /// </summary>

		/// <summary>
		/// Returns the public point W. </summary>
		/// <returns> the public point W. </returns>
		ECPoint W {get;}
	}

	public static class ECPublicKey_Fields
	{
		public const long SerialVersionUID = -3314988629879632826L;
	}

}