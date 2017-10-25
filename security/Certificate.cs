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
	/// <para>This is an interface of abstract methods for managing a
	/// variety of identity certificates.
	/// An identity certificate is a guarantee by a principal that
	/// a public key is that of another principal.  (A principal represents
	/// an entity such as an individual user, a group, or a corporation.)
	/// 
	/// </para>
	/// <para>In particular, this interface is intended to be a common
	/// abstraction for constructs that have different formats but
	/// important common uses.  For example, different types of
	/// certificates, such as X.509 certificates and PGP certificates,
	/// share general certificate functionality (the need to encode and
	/// decode certificates) and some types of information, such as a
	/// public key, the principal whose key it is, and the guarantor
	/// guaranteeing that the public key is that of the specified
	/// principal. So an implementation of X.509 certificates and an
	/// implementation of PGP certificates can both utilize the Certificate
	/// interface, even though their formats and additional types and
	/// amounts of information stored are different.
	/// 
	/// </para>
	/// <para><b>Important</b>: This interface is useful for cataloging and
	/// grouping objects sharing certain common uses. It does not have any
	/// semantics of its own. In particular, a Certificate object does not
	/// make any statement as to the <i>validity</i> of the binding. It is
	/// the duty of the application implementing this interface to verify
	/// the certificate and satisfy itself of its validity.
	/// 
	/// @author Benjamin Renaud
	/// </para>
	/// </summary>
	/// @deprecated A new certificate handling package is created in the Java platform.
	///             This Certificate interface is entirely deprecated and
	///             is here to allow for a smooth transition to the new
	///             package. 
	/// <seealso cref= java.security.cert.Certificate </seealso>
	[Obsolete("A new certificate handling package is created in the Java platform.")]
	public interface Certificate
	{

		/// <summary>
		/// Returns the guarantor of the certificate, that is, the principal
		/// guaranteeing that the public key associated with this certificate
		/// is that of the principal associated with this certificate. For X.509
		/// certificates, the guarantor will typically be a Certificate Authority
		/// (such as the United States Postal Service or Verisign, Inc.).
		/// </summary>
		/// <returns> the guarantor which guaranteed the principal-key
		/// binding. </returns>
		Principal Guarantor {get;}

		/// <summary>
		/// Returns the principal of the principal-key pair being guaranteed by
		/// the guarantor.
		/// </summary>
		/// <returns> the principal to which this certificate is bound. </returns>
		Principal Principal {get;}

		/// <summary>
		/// Returns the key of the principal-key pair being guaranteed by
		/// the guarantor.
		/// </summary>
		/// <returns> the public key that this certificate certifies belongs
		/// to a particular principal. </returns>
		PublicKey PublicKey {get;}

		/// <summary>
		/// Encodes the certificate to an output stream in a format that can
		/// be decoded by the {@code decode} method.
		/// </summary>
		/// <param name="stream"> the output stream to which to encode the
		/// certificate.
		/// </param>
		/// <exception cref="KeyException"> if the certificate is not
		/// properly initialized, or data is missing, etc.
		/// </exception>
		/// <exception cref="IOException"> if a stream exception occurs while
		/// trying to output the encoded certificate to the output stream.
		/// </exception>
		/// <seealso cref= #decode </seealso>
		/// <seealso cref= #getFormat </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void encode(OutputStream stream) throws KeyException, IOException;
		void Encode(OutputStream stream);

		/// <summary>
		/// Decodes a certificate from an input stream. The format should be
		/// that returned by {@code getFormat} and produced by
		/// {@code encode}.
		/// </summary>
		/// <param name="stream"> the input stream from which to fetch the data
		/// being decoded.
		/// </param>
		/// <exception cref="KeyException"> if the certificate is not properly initialized,
		/// or data is missing, etc.
		/// </exception>
		/// <exception cref="IOException"> if an exception occurs while trying to input
		/// the encoded certificate from the input stream.
		/// </exception>
		/// <seealso cref= #encode </seealso>
		/// <seealso cref= #getFormat </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void decode(InputStream stream) throws KeyException, IOException;
		void Decode(InputStream stream);


		/// <summary>
		/// Returns the name of the coding format. This is used as a hint to find
		/// an appropriate parser. It could be "X.509", "PGP", etc. This is
		/// the format produced and understood by the {@code encode}
		/// and {@code decode} methods.
		/// </summary>
		/// <returns> the name of the coding format. </returns>
		String Format {get;}

		/// <summary>
		/// Returns a string that represents the contents of the certificate.
		/// </summary>
		/// <param name="detailed"> whether or not to give detailed information
		/// about the certificate
		/// </param>
		/// <returns> a string representing the contents of the certificate </returns>
		String ToString(bool detailed);
	}

}