/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// <para>A public key. This interface contains no methods or constants.
	/// It merely serves to group (and provide type safety for) all public key
	/// interfaces.
	/// 
	/// Note: The specialized public key interfaces extend this interface.
	/// See, for example, the DSAPublicKey interface in
	/// {@code java.security.interfaces}.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Key </seealso>
	/// <seealso cref= PrivateKey </seealso>
	/// <seealso cref= Certificate </seealso>
	/// <seealso cref= Signature#initVerify </seealso>
	/// <seealso cref= java.security.interfaces.DSAPublicKey </seealso>
	/// <seealso cref= java.security.interfaces.RSAPublicKey
	///  </seealso>

	public interface PublicKey : Key
	{
		// Declare serialVersionUID to be compatible with JDK1.1
		/// <summary>
		/// The class fingerprint that is set to indicate serialization
		/// compatibility with a previous version of the class.
		/// </summary>
	}

	public static class PublicKey_Fields
	{
		public const long SerialVersionUID = 7187392471159151072L;
	}

}