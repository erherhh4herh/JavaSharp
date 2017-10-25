/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.cert
{


	/// <summary>
	/// This interface represents an X.509 extension.
	/// 
	/// <para>
	/// Extensions provide a means of associating additional attributes with users
	/// or public keys and for managing a certification hierarchy.  The extension
	/// format also allows communities to define private extensions to carry
	/// information unique to those communities.
	/// 
	/// </para>
	/// <para>
	/// Each extension contains an object identifier, a criticality setting
	/// indicating whether it is a critical or a non-critical extension, and
	/// and an ASN.1 DER-encoded value. Its ASN.1 definition is:
	/// 
	/// <pre>
	/// 
	///     Extension ::= SEQUENCE {
	///         extnId        OBJECT IDENTIFIER,
	///         critical      BOOLEAN DEFAULT FALSE,
	///         extnValue     OCTET STRING
	///                 -- contains a DER encoding of a value
	///                 -- of the type registered for use with
	///                 -- the extnId object identifier value
	///     }
	/// 
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// This interface is designed to provide access to a single extension,
	/// unlike <seealso cref="java.security.cert.X509Extension"/> which is more suitable
	/// for accessing a set of extensions.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>
	public interface Extension
	{

		/// <summary>
		/// Gets the extensions's object identifier.
		/// </summary>
		/// <returns> the object identifier as a String </returns>
		String Id {get;}

		/// <summary>
		/// Gets the extension's criticality setting.
		/// </summary>
		/// <returns> true if this is a critical extension. </returns>
		bool Critical {get;}

		/// <summary>
		/// Gets the extensions's DER-encoded value. Note, this is the bytes
		/// that are encoded as an OCTET STRING. It does not include the OCTET
		/// STRING tag and length.
		/// </summary>
		/// <returns> a copy of the extension's value, or {@code null} if no
		///    extension value is present. </returns>
		sbyte[] Value {get;}

		/// <summary>
		/// Generates the extension's DER encoding and writes it to the output
		/// stream.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <exception cref="IOException"> on encoding or output error. </exception>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void encode(java.io.OutputStream out) throws java.io.IOException;
		void Encode(OutputStream @out);
	}

}