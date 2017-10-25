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
	/// This class represents the ASN.1 encoding of a public key,
	/// encoded according to the ASN.1 type {@code SubjectPublicKeyInfo}.
	/// The {@code SubjectPublicKeyInfo} syntax is defined in the X.509
	/// standard as follows:
	/// 
	/// <pre>
	/// SubjectPublicKeyInfo ::= SEQUENCE {
	///   algorithm AlgorithmIdentifier,
	///   subjectPublicKey BIT STRING }
	/// </pre>
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= java.security.Key </seealso>
	/// <seealso cref= java.security.KeyFactory </seealso>
	/// <seealso cref= KeySpec </seealso>
	/// <seealso cref= EncodedKeySpec </seealso>
	/// <seealso cref= PKCS8EncodedKeySpec
	/// 
	/// @since 1.2 </seealso>

	public class X509EncodedKeySpec : EncodedKeySpec
	{

		/// <summary>
		/// Creates a new X509EncodedKeySpec with the given encoded key.
		/// </summary>
		/// <param name="encodedKey"> the key, which is assumed to be
		/// encoded according to the X.509 standard. The contents of the
		/// array are copied to protect against subsequent modification. </param>
		/// <exception cref="NullPointerException"> if {@code encodedKey}
		/// is null. </exception>
		public X509EncodedKeySpec(sbyte[] encodedKey) : base(encodedKey)
		{
		}

		/// <summary>
		/// Returns the key bytes, encoded according to the X.509 standard.
		/// </summary>
		/// <returns> the X.509 encoding of the key. Returns a new array
		/// each time this method is called. </returns>
		public override sbyte[] Encoded
		{
			get
			{
				return base.Encoded;
			}
		}

		/// <summary>
		/// Returns the name of the encoding format associated with this
		/// key specification.
		/// </summary>
		/// <returns> the string {@code "X.509"}. </returns>
		public sealed override String Format
		{
			get
			{
				return "X.509";
			}
		}
	}

}