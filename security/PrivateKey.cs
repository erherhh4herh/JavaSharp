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
	/// A private key.
	/// The purpose of this interface is to group (and provide type safety
	/// for) all private key interfaces.
	/// <para>
	/// Note: The specialized private key interfaces extend this interface.
	/// See, for example, the {@code DSAPrivateKey} interface in
	/// <seealso cref="java.security.interfaces"/>.
	/// </para>
	/// <para>
	/// Implementations should override the default {@code destroy} and
	/// {@code isDestroyed} methods from the
	/// <seealso cref="javax.security.auth.Destroyable"/> interface to enable
	/// sensitive key information to be destroyed, cleared, or in the case
	/// where such information is immutable, unreferenced.
	/// Finally, since {@code PrivateKey} is {@code Serializable}, implementations
	/// should also override
	/// <seealso cref="java.io.ObjectOutputStream#writeObject(java.lang.Object)"/>
	/// to prevent keys that have been destroyed from being serialized.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Key </seealso>
	/// <seealso cref= PublicKey </seealso>
	/// <seealso cref= Certificate </seealso>
	/// <seealso cref= Signature#initVerify </seealso>
	/// <seealso cref= java.security.interfaces.DSAPrivateKey </seealso>
	/// <seealso cref= java.security.interfaces.RSAPrivateKey </seealso>
	/// <seealso cref= java.security.interfaces.RSAPrivateCrtKey
	/// 
	/// @author Benjamin Renaud
	/// @author Josh Bloch </seealso>

	public interface PrivateKey : Key, javax.security.auth.Destroyable
	{

		// Declare serialVersionUID to be compatible with JDK1.1
		/// <summary>
		/// The class fingerprint that is set to indicate serialization
		/// compatibility with a previous version of the class.
		/// </summary>
	}

	public static class PrivateKey_Fields
	{
		public const long SerialVersionUID = 6034044314589513430L;
	}

}