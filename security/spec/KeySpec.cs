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
	/// A (transparent) specification of the key material
	/// that constitutes a cryptographic key.
	/// 
	/// <para>If the key is stored on a hardware device, its
	/// specification may contain information that helps identify the key on the
	/// device.
	/// 
	/// <P> A key may be specified in an algorithm-specific way, or in an
	/// algorithm-independent encoding format (such as ASN.1).
	/// For example, a DSA private key may be specified by its components
	/// {@code x}, {@code p}, {@code q}, and {@code g}
	/// (see <seealso cref="DSAPrivateKeySpec"/>), or it may be
	/// specified using its DER encoding
	/// (see <seealso cref="PKCS8EncodedKeySpec"/>).
	/// 
	/// <P> This interface contains no methods or constants. Its only purpose
	/// is to group (and provide type safety for) all key specifications.
	/// All key specifications must implement this interface.
	/// 
	/// @author Jan Luehe
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= EncodedKeySpec </seealso>
	/// <seealso cref= X509EncodedKeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec </seealso>
	/// <seealso cref= DSAPrivateKeySpec </seealso>
	/// <seealso cref= DSAPublicKeySpec
	/// 
	/// @since 1.2 </seealso>

	public interface KeySpec
	{
	}

}