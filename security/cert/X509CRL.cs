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



	using X509CRLImpl = sun.security.x509.X509CRLImpl;

	/// <summary>
	/// <para>
	/// Abstract class for an X.509 Certificate Revocation List (CRL).
	/// A CRL is a time-stamped list identifying revoked certificates.
	/// It is signed by a Certificate Authority (CA) and made freely
	/// available in a public repository.
	/// 
	/// </para>
	/// <para>Each revoked certificate is
	/// identified in a CRL by its certificate serial number. When a
	/// certificate-using system uses a certificate (e.g., for verifying a
	/// remote user's digital signature), that system not only checks the
	/// certificate signature and validity but also acquires a suitably-
	/// recent CRL and checks that the certificate serial number is not on
	/// that CRL.  The meaning of "suitably-recent" may vary with local
	/// policy, but it usually means the most recently-issued CRL.  A CA
	/// issues a new CRL on a regular periodic basis (e.g., hourly, daily, or
	/// weekly).  Entries are added to CRLs as revocations occur, and an
	/// entry may be removed when the certificate expiration date is reached.
	/// </para>
	/// <para>
	/// The X.509 v2 CRL format is described below in ASN.1:
	/// <pre>
	/// CertificateList  ::=  SEQUENCE  {
	///     tbsCertList          TBSCertList,
	///     signatureAlgorithm   AlgorithmIdentifier,
	///     signature            BIT STRING  }
	/// </pre>
	/// </para>
	/// <para>
	/// More information can be found in
	/// <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280: Internet X.509
	/// Public Key Infrastructure Certificate and CRL Profile</a>.
	/// </para>
	/// <para>
	/// The ASN.1 definition of {@code tbsCertList} is:
	/// <pre>
	/// TBSCertList  ::=  SEQUENCE  {
	///     version                 Version OPTIONAL,
	///                             -- if present, must be v2
	///     signature               AlgorithmIdentifier,
	///     issuer                  Name,
	///     thisUpdate              ChoiceOfTime,
	///     nextUpdate              ChoiceOfTime OPTIONAL,
	///     revokedCertificates     SEQUENCE OF SEQUENCE  {
	///         userCertificate         CertificateSerialNumber,
	///         revocationDate          ChoiceOfTime,
	///         crlEntryExtensions      Extensions OPTIONAL
	///                                 -- if present, must be v2
	///         }  OPTIONAL,
	///     crlExtensions           [0]  EXPLICIT Extensions OPTIONAL
	///                                  -- if present, must be v2
	///     }
	/// </pre>
	/// </para>
	/// <para>
	/// CRLs are instantiated using a certificate factory. The following is an
	/// example of how to instantiate an X.509 CRL:
	/// <pre>{@code
	/// try (InputStream inStream = new FileInputStream("fileName-of-crl")) {
	///     CertificateFactory cf = CertificateFactory.getInstance("X.509");
	///     X509CRL crl = (X509CRL)cf.generateCRL(inStream);
	/// }
	/// }</pre>
	/// 
	/// @author Hemma Prafullchandra
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CRL </seealso>
	/// <seealso cref= CertificateFactory </seealso>
	/// <seealso cref= X509Extension </seealso>

	public abstract class X509CRL : CRL, X509Extension
	{
		public abstract sbyte[] GetExtensionValue(string oid);
		public abstract Set<String> NonCriticalExtensionOIDs {get;}
		public abstract Set<String> CriticalExtensionOIDs {get;}
		public abstract bool HasUnsupportedCriticalExtension();

		[NonSerialized]
		private X500Principal IssuerPrincipal;

		/// <summary>
		/// Constructor for X.509 CRLs.
		/// </summary>
		protected internal X509CRL() : base("X.509")
		{
		}

		/// <summary>
		/// Compares this CRL for equality with the given
		/// object. If the {@code other} object is an
		/// {@code instanceof} {@code X509CRL}, then
		/// its encoded form is retrieved and compared with the
		/// encoded form of this CRL.
		/// </summary>
		/// <param name="other"> the object to test for equality with this CRL.
		/// </param>
		/// <returns> true iff the encoded forms of the two CRLs
		/// match, false otherwise. </returns>
		public override bool Equals(Object other)
		{
			if (this == other)
			{
				return true;
			}
			if (!(other is X509CRL))
			{
				return false;
			}
			try
			{
				sbyte[] thisCRL = X509CRLImpl.getEncodedInternal(this);
				sbyte[] otherCRL = X509CRLImpl.getEncodedInternal((X509CRL)other);

				return Arrays.Equals(thisCRL, otherCRL);
			}
			catch (CRLException)
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a hashcode value for this CRL from its
		/// encoded form.
		/// </summary>
		/// <returns> the hashcode value. </returns>
		public override int HashCode()
		{
			int retval = 0;
			try
			{
				sbyte[] crlData = X509CRLImpl.getEncodedInternal(this);
				for (int i = 1; i < crlData.Length; i++)
				{
					 retval += crlData[i] * i;
				}
				return retval;
			}
			catch (CRLException)
			{
				return retval;
			}
		}

		/// <summary>
		/// Returns the ASN.1 DER-encoded form of this CRL.
		/// </summary>
		/// <returns> the encoded form of this certificate </returns>
		/// <exception cref="CRLException"> if an encoding error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getEncoded() throws CRLException;
		public abstract sbyte[] Encoded {get;}

		/// <summary>
		/// Verifies that this CRL was signed using the
		/// private key that corresponds to the given public key.
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="NoSuchProviderException"> if there's no default provider. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CRLException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void verify(java.security.PublicKey key) throws CRLException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.NoSuchProviderException, java.security.SignatureException;
		public abstract void Verify(PublicKey key);

		/// <summary>
		/// Verifies that this CRL was signed using the
		/// private key that corresponds to the given public key.
		/// This method uses the signature verification engine
		/// supplied by the given provider.
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification. </param>
		/// <param name="sigProvider"> the name of the signature provider.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="NoSuchProviderException"> on incorrect provider. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CRLException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void verify(java.security.PublicKey key, String sigProvider) throws CRLException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.NoSuchProviderException, java.security.SignatureException;
		public abstract void Verify(PublicKey key, String sigProvider);

		/// <summary>
		/// Verifies that this CRL was signed using the
		/// private key that corresponds to the given public key.
		/// This method uses the signature verification engine
		/// supplied by the given provider. Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// This method was added to version 1.8 of the Java Platform Standard
		/// Edition. In order to maintain backwards compatibility with existing
		/// service providers, this method is not {@code abstract}
		/// and it provides a default implementation.
		/// </summary>
		/// <param name="key"> the PublicKey used to carry out the verification. </param>
		/// <param name="sigProvider"> the signature provider.
		/// </param>
		/// <exception cref="NoSuchAlgorithmException"> on unsupported signature
		/// algorithms. </exception>
		/// <exception cref="InvalidKeyException"> on incorrect key. </exception>
		/// <exception cref="SignatureException"> on signature errors. </exception>
		/// <exception cref="CRLException"> on encoding errors.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void verify(java.security.PublicKey key, java.security.Provider sigProvider) throws CRLException, java.security.NoSuchAlgorithmException, java.security.InvalidKeyException, java.security.SignatureException
		public virtual void Verify(PublicKey key, Provider sigProvider)
		{
			X509CRLImpl.verify(this, key, sigProvider);
		}

		/// <summary>
		/// Gets the {@code version} (version number) value from the CRL.
		/// The ASN.1 definition for this is:
		/// <pre>
		/// version    Version OPTIONAL,
		///             -- if present, must be v2
		/// 
		/// Version  ::=  INTEGER  {  v1(0), v2(1), v3(2)  }
		///             -- v3 does not apply to CRLs but appears for consistency
		///             -- with definition of Version for certs
		/// </pre>
		/// </summary>
		/// <returns> the version number, i.e. 1 or 2. </returns>
		public abstract int Version {get;}

		/// <summary>
		/// <strong>Denigrated</strong>, replaced by {@linkplain
		/// #getIssuerX500Principal()}. This method returns the {@code issuer}
		/// as an implementation specific Principal object, which should not be
		/// relied upon by portable code.
		/// 
		/// <para>
		/// Gets the {@code issuer} (issuer distinguished name) value from
		/// the CRL. The issuer name identifies the entity that signed (and
		/// issued) the CRL.
		/// 
		/// </para>
		/// <para>The issuer name field contains an
		/// X.500 distinguished name (DN).
		/// The ASN.1 definition for this is:
		/// <pre>
		/// issuer    Name
		/// 
		/// Name ::= CHOICE { RDNSequence }
		/// RDNSequence ::= SEQUENCE OF RelativeDistinguishedName
		/// RelativeDistinguishedName ::=
		///     SET OF AttributeValueAssertion
		/// 
		/// AttributeValueAssertion ::= SEQUENCE {
		///                               AttributeType,
		///                               AttributeValue }
		/// AttributeType ::= OBJECT IDENTIFIER
		/// AttributeValue ::= ANY
		/// </pre>
		/// The {@code Name} describes a hierarchical name composed of
		/// attributes,
		/// such as country name, and corresponding values, such as US.
		/// The type of the {@code AttributeValue} component is determined by
		/// the {@code AttributeType}; in general it will be a
		/// {@code directoryString}. A {@code directoryString} is usually
		/// one of {@code PrintableString},
		/// {@code TeletexString} or {@code UniversalString}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a Principal whose name is the issuer distinguished name. </returns>
		public abstract Principal IssuerDN {get;}

		/// <summary>
		/// Returns the issuer (issuer distinguished name) value from the
		/// CRL as an {@code X500Principal}.
		/// <para>
		/// It is recommended that subclasses override this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code X500Principal} representing the issuer
		///          distinguished name
		/// @since 1.4 </returns>
		public virtual X500Principal IssuerX500Principal
		{
			get
			{
				if (IssuerPrincipal == null)
				{
					IssuerPrincipal = X509CRLImpl.getIssuerX500Principal(this);
				}
				return IssuerPrincipal;
			}
		}

		/// <summary>
		/// Gets the {@code thisUpdate} date from the CRL.
		/// The ASN.1 definition for this is:
		/// <pre>
		/// thisUpdate   ChoiceOfTime
		/// ChoiceOfTime ::= CHOICE {
		///     utcTime        UTCTime,
		///     generalTime    GeneralizedTime }
		/// </pre>
		/// </summary>
		/// <returns> the {@code thisUpdate} date from the CRL. </returns>
		public abstract DateTime ThisUpdate {get;}

		/// <summary>
		/// Gets the {@code nextUpdate} date from the CRL.
		/// </summary>
		/// <returns> the {@code nextUpdate} date from the CRL, or null if
		/// not present. </returns>
		public abstract DateTime NextUpdate {get;}

		/// <summary>
		/// Gets the CRL entry, if any, with the given certificate serialNumber.
		/// </summary>
		/// <param name="serialNumber"> the serial number of the certificate for which a CRL entry
		/// is to be looked up </param>
		/// <returns> the entry with the given serial number, or null if no such entry
		/// exists in this CRL. </returns>
		/// <seealso cref= X509CRLEntry </seealso>
		public abstract X509CRLEntry GetRevokedCertificate(System.Numerics.BigInteger serialNumber);

		/// <summary>
		/// Get the CRL entry, if any, for the given certificate.
		/// 
		/// <para>This method can be used to lookup CRL entries in indirect CRLs,
		/// that means CRLs that contain entries from issuers other than the CRL
		/// issuer. The default implementation will only return entries for
		/// certificates issued by the CRL issuer. Subclasses that wish to
		/// support indirect CRLs should override this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certificate"> the certificate for which a CRL entry is to be looked
		///   up </param>
		/// <returns> the entry for the given certificate, or null if no such entry
		///   exists in this CRL. </returns>
		/// <exception cref="NullPointerException"> if certificate is null
		/// 
		/// @since 1.5 </exception>
		public virtual X509CRLEntry GetRevokedCertificate(X509Certificate certificate)
		{
			X500Principal certIssuer = certificate.IssuerX500Principal;
			X500Principal crlIssuer = IssuerX500Principal;
			if (certIssuer.Equals(crlIssuer) == false)
			{
				return null;
			}
			return GetRevokedCertificate(certificate.SerialNumber);
		}

		/// <summary>
		/// Gets all the entries from this CRL.
		/// This returns a Set of X509CRLEntry objects.
		/// </summary>
		/// <returns> all the entries or null if there are none present. </returns>
		/// <seealso cref= X509CRLEntry </seealso>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.Set<? extends X509CRLEntry> getRevokedCertificates();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract java.util.Set<? extends X509CRLEntry> getRevokedCertificates();
		public abstract Set<?> RevokedCertificates where ? : X509CRLEntry {get;}

		/// <summary>
		/// Gets the DER-encoded CRL information, the
		/// {@code tbsCertList} from this CRL.
		/// This can be used to verify the signature independently.
		/// </summary>
		/// <returns> the DER-encoded CRL information. </returns>
		/// <exception cref="CRLException"> if an encoding error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getTBSCertList() throws CRLException;
		public abstract sbyte[] TBSCertList {get;}

		/// <summary>
		/// Gets the {@code signature} value (the raw signature bits) from
		/// the CRL.
		/// The ASN.1 definition for this is:
		/// <pre>
		/// signature     BIT STRING
		/// </pre>
		/// </summary>
		/// <returns> the signature. </returns>
		public abstract sbyte[] Signature {get;}

		/// <summary>
		/// Gets the signature algorithm name for the CRL
		/// signature algorithm. An example is the string "SHA256withRSA".
		/// The ASN.1 definition for this is:
		/// <pre>
		/// signatureAlgorithm   AlgorithmIdentifier
		/// 
		/// AlgorithmIdentifier  ::=  SEQUENCE  {
		///     algorithm               OBJECT IDENTIFIER,
		///     parameters              ANY DEFINED BY algorithm OPTIONAL  }
		///                             -- contains a value of the type
		///                             -- registered for use with the
		///                             -- algorithm object identifier value
		/// </pre>
		/// 
		/// <para>The algorithm name is determined from the {@code algorithm}
		/// OID string.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the signature algorithm name. </returns>
		public abstract String SigAlgName {get;}

		/// <summary>
		/// Gets the signature algorithm OID string from the CRL.
		/// An OID is represented by a set of nonnegative whole numbers separated
		/// by periods.
		/// For example, the string "1.2.840.10040.4.3" identifies the SHA-1
		/// with DSA signature algorithm defined in
		/// <a href="http://www.ietf.org/rfc/rfc3279.txt">RFC 3279: Algorithms and
		/// Identifiers for the Internet X.509 Public Key Infrastructure Certificate
		/// and CRL Profile</a>.
		/// 
		/// <para>See <seealso cref="#getSigAlgName() getSigAlgName"/> for
		/// relevant ASN.1 definitions.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the signature algorithm OID string. </returns>
		public abstract String SigAlgOID {get;}

		/// <summary>
		/// Gets the DER-encoded signature algorithm parameters from this
		/// CRL's signature algorithm. In most cases, the signature
		/// algorithm parameters are null; the parameters are usually
		/// supplied with the public key.
		/// If access to individual parameter values is needed then use
		/// <seealso cref="java.security.AlgorithmParameters AlgorithmParameters"/>
		/// and instantiate with the name returned by
		/// <seealso cref="#getSigAlgName() getSigAlgName"/>.
		/// 
		/// <para>See <seealso cref="#getSigAlgName() getSigAlgName"/> for
		/// relevant ASN.1 definitions.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the DER-encoded signature algorithm parameters, or
		///         null if no parameters are present. </returns>
		public abstract sbyte[] SigAlgParams {get;}
	}

}