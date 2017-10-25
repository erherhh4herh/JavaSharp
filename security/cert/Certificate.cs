using System;

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

namespace java.security.cert
{


	using X509CertImpl = sun.security.x509.X509CertImpl;

	/// <summary>
	/// <para>Abstract class for managing a variety of identity certificates.
	/// An identity certificate is a binding of a principal to a public key which
	/// is vouched for by another principal.  (A principal represents
	/// an entity such as an individual user, a group, or a corporation.)
	/// </para>
	/// <para>
	/// This class is an abstraction for certificates that have different
	/// formats but important common uses.  For example, different types of
	/// certificates, such as X.509 and PGP, share general certificate
	/// functionality (like encoding and verifying) and
	/// some types of information (like a public key).
	/// </para>
	/// <para>
	/// X.509, PGP, and SDSI certificates can all be implemented by
	/// subclassing the Certificate class, even though they contain different
	/// sets of information, and they store and retrieve the information in
	/// different ways.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= X509Certificate </seealso>
	/// <seealso cref= CertificateFactory
	/// 
	/// @author Hemma Prafullchandra </seealso>

	[Serializable]
	public abstract class Certificate
	{

		private const long SerialVersionUID = -3585440601605666277L;

		// the certificate type
		private readonly String Type_Renamed;

		/// <summary>
		/// Cache the hash code for the certiticate </summary>
		private int Hash = -1; // Default to -1

		/// <summary>
		/// Creates a certificate of the specified type.
		/// </summary>
		/// <param name="type"> the standard name of the certificate type.
		/// See the CertificateFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard certificate types. </param>
		protected internal Certificate(String type)
		{
			this.Type_Renamed = type;
		}

		/// <summary>
		/// Returns the type of this certificate.
		/// </summary>
		/// <returns> the type of this certificate. </returns>
		public String Type
		{
			get
			{
				return this.Type_Renamed;
			}
		}

		/// <summary>
		/// Compares this certificate for equality with the specified
		/// object. If the {@code other} object is an
		/// {@code instanceof} {@code Certificate}, then
		/// its encoded form is retrieved and compared with the
		/// encoded form of this certificate.
		/// </summary>
		/// <param name="other"> the object to test for equality with this certificate. </param>
		/// <returns> true iff the encoded forms of the two certificates
		/// match, false otherwise. </returns>
		public override bool Equals(Object other)
		{
			if (this == other)
			{
				return true;
			}
			if (!(other is Certificate))
			{
				return false;
			}
			try
			{
				sbyte[] thisCert = X509CertImpl.getEncodedInternal(this);
				sbyte[] otherCert = X509CertImpl.getEncodedInternal((Certificate)other);

				return Arrays.Equals(thisCert, otherCert);
			}
			catch (CertificateException)
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a hashcode value for this certificate from its
		/// encoded form.
		/// </summary>
		/// <returns> the hashcode value. </returns>
		public override int HashCode()
		{
			int h = Hash;
			if (h == -1)
			{
				try
				{
					h = Arrays.HashCode(X509CertImpl.getEncodedInternal(this));
				}
				catch (CertificateException)
				{
					h = 0;
				}
				Hash = h;
			}
			return h;
		}

		/// <summary>
		/// Returns the encoded form of this certificate. It is
		/// assumed that each certificate type would have only a single
		/// form of encoding; for example, X.509 certificates would
		/// be encoded as ASN.1 DER.
		/// </summary>
		/// <returns> the encoded form of this certificate
		/// </returns>
		/// <exception cref="CertificateEncodingException"> if an encoding error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getEncoded() throws CertificateEncodingException;
		public abstract sbyte[] Encoded {get;}

		/// <summary>
		/// Verifies that this certificate was signed using the
		/// private key that corresponds to the specified public key.
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="NoSuchProviderException"> if there's no default provider. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CertificateException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void verify(java.security.PublicKey key) throws CertificateException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.NoSuchProviderException, java.security.SignatureException;
		public abstract void Verify(PublicKey key);

		/// <summary>
		/// Verifies that this certificate was signed using the
		/// private key that corresponds to the specified public key.
		/// This method uses the signature verification engine
		/// supplied by the specified provider.
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification. </param>
		/// <param name="sigProvider"> the name of the signature provider.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="NoSuchProviderException"> on incorrect provider. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CertificateException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void verify(java.security.PublicKey key, String sigProvider) throws CertificateException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.NoSuchProviderException, java.security.SignatureException;
		public abstract void Verify(PublicKey key, String sigProvider);

		/// <summary>
		/// Verifies that this certificate was signed using the
		/// private key that corresponds to the specified public key.
		/// This method uses the signature verification engine
		/// supplied by the specified provider. Note that the specified
		/// Provider object does not have to be registered in the provider list.
		/// 
		/// <para> This method was added to version 1.8 of the Java Platform
		/// Standard Edition. In order to maintain backwards compatibility with
		/// existing service providers, this method cannot be {@code abstract}
		/// and by default throws an {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification. </param>
		/// <param name="sigProvider"> the signature provider.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CertificateException"> on encoding errors. </exception>
		/// <exception cref="UnsupportedOperationException"> if the method is not supported
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void verify(java.security.PublicKey key, java.security.Provider sigProvider) throws CertificateException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.SignatureException
		public virtual void Verify(PublicKey key, Provider sigProvider)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Returns a string representation of this certificate.
		/// </summary>
		/// <returns> a string representation of this certificate. </returns>
		public override abstract String ToString();

		/// <summary>
		/// Gets the public key from this certificate.
		/// </summary>
		/// <returns> the public key. </returns>
		public abstract PublicKey PublicKey {get;}

		/// <summary>
		/// Alternate Certificate class for serialization.
		/// @since 1.3
		/// </summary>
		[Serializable]
		protected internal class CertificateRep
		{

			internal const long SerialVersionUID = -8563758940495660020L;

			internal String Type;
			internal sbyte[] Data;

			/// <summary>
			/// Construct the alternate Certificate class with the Certificate
			/// type and Certificate encoding bytes.
			/// 
			/// <para>
			/// 
			/// </para>
			/// </para>
			/// </summary>
			/// <param name="type"> the standard name of the Certificate type. <para>
			/// </param>
			/// <param name="data"> the Certificate data. </param>
			protected internal CertificateRep(String type, sbyte[] data)
			{
				this.Type = type;
				this.Data = data;
			}

			/// <summary>
			/// Resolve the Certificate Object.
			/// 
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <returns> the resolved Certificate Object
			/// </returns>
			/// <exception cref="java.io.ObjectStreamException"> if the Certificate
			///      could not be resolved </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.ObjectStreamException
			protected internal virtual Object ReadResolve()
			{
				try
				{
					CertificateFactory cf = CertificateFactory.GetInstance(Type);
					return cf.GenerateCertificate(new java.io.ByteArrayInputStream(Data));
				}
				catch (CertificateException e)
				{
					throw new java.io.NotSerializableException("java.security.cert.Certificate: " + Type + ": " + e.Message);
				}
			}
		}

		/// <summary>
		/// Replace the Certificate to be serialized.
		/// </summary>
		/// <returns> the alternate Certificate object to be serialized
		/// </returns>
		/// <exception cref="java.io.ObjectStreamException"> if a new object representing
		/// this Certificate could not be created
		/// @since 1.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object writeReplace() throws java.io.ObjectStreamException
		protected internal virtual Object WriteReplace()
		{
			try
			{
				return new CertificateRep(Type_Renamed, Encoded);
			}
			catch (CertificateException e)
			{
				throw new java.io.NotSerializableException("java.security.cert.Certificate: " + Type_Renamed + ": " + e.Message);
			}
		}
	}

}