/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// An enumeration of cryptographic primitives.
	/// 
	/// @since 1.7
	/// </summary>
	public enum CryptoPrimitive
	{
		/// <summary>
		/// Hash function
		/// </summary>
		MESSAGE_DIGEST,

		/// <summary>
		/// Cryptographic random number generator
		/// </summary>
		SECURE_RANDOM,

		/// <summary>
		/// Symmetric primitive: block cipher
		/// </summary>
		BLOCK_CIPHER,

		/// <summary>
		/// Symmetric primitive: stream cipher
		/// </summary>
		STREAM_CIPHER,

		/// <summary>
		/// Symmetric primitive: message authentication code
		/// </summary>
		MAC,

		/// <summary>
		/// Symmetric primitive: key wrap
		/// </summary>
		KEY_WRAP,

		/// <summary>
		/// Asymmetric primitive: public key encryption
		/// </summary>
		PUBLIC_KEY_ENCRYPTION,

		/// <summary>
		/// Asymmetric primitive: signature scheme
		/// </summary>
		SIGNATURE,

		/// <summary>
		/// Asymmetric primitive: key encapsulation mechanism
		/// </summary>
		KEY_ENCAPSULATION,

		/// <summary>
		/// Asymmetric primitive: key agreement and key distribution
		/// </summary>
		KEY_AGREEMENT
	}

}