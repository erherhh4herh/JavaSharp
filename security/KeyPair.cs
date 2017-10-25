using System;

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
	/// This class is a simple holder for a key pair (a public key and a
	/// private key). It does not enforce any security, and, when initialized,
	/// should be treated like a PrivateKey.
	/// </summary>
	/// <seealso cref= PublicKey </seealso>
	/// <seealso cref= PrivateKey
	/// 
	/// @author Benjamin Renaud </seealso>

	[Serializable]
	public sealed class KeyPair
	{

		private const long SerialVersionUID = -7565189502268009837L;

		private PrivateKey PrivateKey;
		private PublicKey PublicKey;

		/// <summary>
		/// Constructs a key pair from the given public key and private key.
		/// 
		/// <para>Note that this constructor only stores references to the public
		/// and private key components in the generated key pair. This is safe,
		/// because {@code Key} objects are immutable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="publicKey"> the public key.
		/// </param>
		/// <param name="privateKey"> the private key. </param>
		public KeyPair(PublicKey publicKey, PrivateKey privateKey)
		{
			this.PublicKey = publicKey;
			this.PrivateKey = privateKey;
		}

		/// <summary>
		/// Returns a reference to the public key component of this key pair.
		/// </summary>
		/// <returns> a reference to the public key. </returns>
		public PublicKey Public
		{
			get
			{
				return PublicKey;
			}
		}

		 /// <summary>
		 /// Returns a reference to the private key component of this key pair.
		 /// </summary>
		 /// <returns> a reference to the private key. </returns>
	   public PrivateKey Private
	   {
		   get
		   {
				return PrivateKey;
		   }
	   }
	}

}