using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using HexDumpEncoder = sun.misc.HexDumpEncoder;
	using Debug = sun.security.util.Debug;
	using DerInputStream = sun.security.util.DerInputStream;
	using DerValue = sun.security.util.DerValue;
	using ObjectIdentifier = sun.security.util.ObjectIdentifier;
	using  = sun.security.x509.0 * ;

	/// <summary>
	/// A {@code CertSelector} that selects {@code X509Certificates} that
	/// match all specified criteria. This class is particularly useful when
	/// selecting certificates from a {@code CertStore} to build a
	/// PKIX-compliant certification path.
	/// <para>
	/// When first constructed, an {@code X509CertSelector} has no criteria
	/// enabled and each of the {@code get} methods return a default value
	/// ({@code null}, or {@code -1} for the {@link #getBasicConstraints
	/// getBasicConstraints} method). Therefore, the <seealso cref="#match match"/>
	/// method would return {@code true} for any {@code X509Certificate}.
	/// Typically, several criteria are enabled (by calling
	/// <seealso cref="#setIssuer setIssuer"/> or
	/// <seealso cref="#setKeyUsage setKeyUsage"/>, for instance) and then the
	/// {@code X509CertSelector} is passed to
	/// <seealso cref="CertStore#getCertificates CertStore.getCertificates"/> or some similar
	/// method.
	/// </para>
	/// <para>
	/// Several criteria can be enabled (by calling <seealso cref="#setIssuer setIssuer"/>
	/// and <seealso cref="#setSerialNumber setSerialNumber"/>,
	/// for example) such that the {@code match} method
	/// usually uniquely matches a single {@code X509Certificate}. We say
	/// usually, since it is possible for two issuing CAs to have the same
	/// distinguished name and each issue a certificate with the same serial
	/// number. Other unique combinations include the issuer, subject,
	/// subjectKeyIdentifier and/or the subjectPublicKey criteria.
	/// </para>
	/// <para>
	/// Please refer to <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280:
	/// Internet X.509 Public Key Infrastructure Certificate and CRL Profile</a> for
	/// definitions of the X.509 certificate extensions mentioned below.
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// Unless otherwise specified, the methods defined in this class are not
	/// thread-safe. Multiple threads that need to access a single
	/// object concurrently should synchronize amongst themselves and
	/// provide the necessary locking. Multiple threads each manipulating
	/// separate objects need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertSelector </seealso>
	/// <seealso cref= X509Certificate
	/// 
	/// @since       1.4
	/// @author      Steve Hanna </seealso>
	public class X509CertSelector : CertSelector
	{

		private static readonly Debug Debug = Debug.getInstance("certpath");

		private static readonly ObjectIdentifier ANY_EXTENDED_KEY_USAGE = ObjectIdentifier.newInternal(new int[] {2, 5, 29, 37, 0});

		static X509CertSelector()
		{
			CertPathHelperImpl.Initialize();
			EXTENSION_OIDS[PRIVATE_KEY_USAGE_ID] = "2.5.29.16";
			EXTENSION_OIDS[SUBJECT_ALT_NAME_ID] = "2.5.29.17";
			EXTENSION_OIDS[NAME_CONSTRAINTS_ID] = "2.5.29.30";
			EXTENSION_OIDS[CERT_POLICIES_ID] = "2.5.29.32";
			EXTENSION_OIDS[EXTENDED_KEY_USAGE_ID] = "2.5.29.37";
		}

		private System.Numerics.BigInteger SerialNumber_Renamed;
		private X500Principal Issuer_Renamed;
		private X500Principal Subject_Renamed;
		private sbyte[] SubjectKeyID;
		private sbyte[] AuthorityKeyID;
		private Date CertificateValid_Renamed;
		private Date PrivateKeyValid_Renamed;
		private ObjectIdentifier SubjectPublicKeyAlgID_Renamed;
		private PublicKey SubjectPublicKey_Renamed;
		private sbyte[] SubjectPublicKeyBytes;
		private bool[] KeyUsage_Renamed;
		private Set<String> KeyPurposeSet;
		private Set<ObjectIdentifier> KeyPurposeOIDSet;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Set<List<?>> subjectAlternativeNames;
		private Set<List<?>> SubjectAlternativeNames_Renamed;
		private Set<GeneralNameInterface> SubjectAlternativeGeneralNames;
		private CertificatePolicySet Policy_Renamed;
		private Set<String> PolicySet;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Set<List<?>> pathToNames;
		private Set<List<?>> PathToNames_Renamed;
		private Set<GeneralNameInterface> PathToGeneralNames;
		private NameConstraintsExtension Nc;
		private sbyte[] NcBytes;
		private int BasicConstraints_Renamed = -1;
		private X509Certificate X509Cert;
		private bool MatchAllSubjectAltNames_Renamed = true;

		private static readonly Boolean FALSE = false;

		private const int PRIVATE_KEY_USAGE_ID = 0;
		private const int SUBJECT_ALT_NAME_ID = 1;
		private const int NAME_CONSTRAINTS_ID = 2;
		private const int CERT_POLICIES_ID = 3;
		private const int EXTENDED_KEY_USAGE_ID = 4;
		private const int NUM_OF_EXTENSIONS = 5;
		private static readonly String[] EXTENSION_OIDS = new String[NUM_OF_EXTENSIONS];


		/* Constants representing the GeneralName types */
		internal const int NAME_ANY = 0;
		internal const int NAME_RFC822 = 1;
		internal const int NAME_DNS = 2;
		internal const int NAME_X400 = 3;
		internal const int NAME_DIRECTORY = 4;
		internal const int NAME_EDI = 5;
		internal const int NAME_URI = 6;
		internal const int NAME_IP = 7;
		internal const int NAME_OID = 8;

		/// <summary>
		/// Creates an {@code X509CertSelector}. Initially, no criteria are set
		/// so any {@code X509Certificate} will match.
		/// </summary>
		public X509CertSelector()
		{
			// empty
		}

		/// <summary>
		/// Sets the certificateEquals criterion. The specified
		/// {@code X509Certificate} must be equal to the
		/// {@code X509Certificate} passed to the {@code match} method.
		/// If {@code null}, then this check is not applied.
		/// 
		/// <para>This method is particularly useful when it is necessary to
		/// match a single certificate. Although other criteria can be specified
		/// in conjunction with the certificateEquals criterion, it is usually not
		/// practical or necessary.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cert"> the {@code X509Certificate} to match (or
		/// {@code null}) </param>
		/// <seealso cref= #getCertificate </seealso>
		public virtual X509Certificate Certificate
		{
			set
			{
				X509Cert = value;
			}
			get
			{
				return X509Cert;
			}
		}

		/// <summary>
		/// Sets the serialNumber criterion. The specified serial number
		/// must match the certificate serial number in the
		/// {@code X509Certificate}. If {@code null}, any certificate
		/// serial number will do.
		/// </summary>
		/// <param name="serial"> the certificate serial number to match
		///        (or {@code null}) </param>
		/// <seealso cref= #getSerialNumber </seealso>
		public virtual System.Numerics.BigInteger SerialNumber
		{
			set
			{
				SerialNumber_Renamed = value;
			}
			get
			{
				return SerialNumber_Renamed;
			}
		}

		/// <summary>
		/// Sets the issuer criterion. The specified distinguished name
		/// must match the issuer distinguished name in the
		/// {@code X509Certificate}. If {@code null}, any issuer
		/// distinguished name will do.
		/// </summary>
		/// <param name="issuer"> a distinguished name as X500Principal
		///                 (or {@code null})
		/// @since 1.5 </param>
		public virtual X500Principal Issuer
		{
			set
			{
				this.Issuer_Renamed = value;
			}
			get
			{
				return Issuer_Renamed;
			}
		}

		/// <summary>
		/// <strong>Denigrated</strong>, use <seealso cref="#setIssuer(X500Principal)"/>
		/// or <seealso cref="#setIssuer(byte[])"/> instead. This method should not be
		/// relied on as it can fail to match some certificates because of a loss of
		/// encoding information in the
		/// <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a> String form
		/// of some distinguished names.
		/// <para>
		/// Sets the issuer criterion. The specified distinguished name
		/// must match the issuer distinguished name in the
		/// {@code X509Certificate}. If {@code null}, any issuer
		/// distinguished name will do.
		/// </para>
		/// <para>
		/// If {@code issuerDN} is not {@code null}, it should contain a
		/// distinguished name, in RFC 2253 format.
		/// 
		/// </para>
		/// </summary>
		/// <param name="issuerDN"> a distinguished name in RFC 2253 format
		///                 (or {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs (incorrect form for DN) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setIssuer(String issuerDN) throws java.io.IOException
		public virtual String Issuer
		{
			set
			{
				if (value == null)
				{
					Issuer_Renamed = null;
				}
				else
				{
					Issuer_Renamed = (new X500Name(value)).asX500Principal();
				}
			}
		}

		/// <summary>
		/// Sets the issuer criterion. The specified distinguished name
		/// must match the issuer distinguished name in the
		/// {@code X509Certificate}. If {@code null} is specified,
		/// the issuer criterion is disabled and any issuer distinguished name will
		/// do.
		/// <para>
		/// If {@code issuerDN} is not {@code null}, it should contain a
		/// single DER encoded distinguished name, as defined in X.501. The ASN.1
		/// notation for this structure is as follows.
		/// <pre>{@code
		/// Name ::= CHOICE {
		///   RDNSequence }
		/// 
		/// RDNSequence ::= SEQUENCE OF RelativeDistinguishedName
		/// 
		/// RelativeDistinguishedName ::=
		///   SET SIZE (1 .. MAX) OF AttributeTypeAndValue
		/// 
		/// AttributeTypeAndValue ::= SEQUENCE {
		///   type     AttributeType,
		///   value    AttributeValue }
		/// 
		/// AttributeType ::= OBJECT IDENTIFIER
		/// 
		/// AttributeValue ::= ANY DEFINED BY AttributeType
		/// ....
		/// DirectoryString ::= CHOICE {
		///       teletexString           TeletexString (SIZE (1..MAX)),
		///       printableString         PrintableString (SIZE (1..MAX)),
		///       universalString         UniversalString (SIZE (1..MAX)),
		///       utf8String              UTF8String (SIZE (1.. MAX)),
		///       bmpString               BMPString (SIZE (1..MAX)) }
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that the byte array specified here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="issuerDN"> a byte array containing the distinguished name
		///                 in ASN.1 DER encoded form (or {@code null}) </param>
		/// <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setIssuer(byte[] issuerDN) throws java.io.IOException
		public virtual sbyte[] Issuer
		{
			set
			{
				try
				{
					Issuer_Renamed = (value == null ? null : new X500Principal(value));
				}
				catch (IllegalArgumentException e)
				{
					throw new IOException("Invalid name", e);
				}
			}
		}

		/// <summary>
		/// Sets the subject criterion. The specified distinguished name
		/// must match the subject distinguished name in the
		/// {@code X509Certificate}. If {@code null}, any subject
		/// distinguished name will do.
		/// </summary>
		/// <param name="subject"> a distinguished name as X500Principal
		///                  (or {@code null})
		/// @since 1.5 </param>
		public virtual X500Principal Subject
		{
			set
			{
				this.Subject_Renamed = value;
			}
			get
			{
				return Subject_Renamed;
			}
		}

		/// <summary>
		/// <strong>Denigrated</strong>, use <seealso cref="#setSubject(X500Principal)"/>
		/// or <seealso cref="#setSubject(byte[])"/> instead. This method should not be
		/// relied on as it can fail to match some certificates because of a loss of
		/// encoding information in the RFC 2253 String form of some distinguished
		/// names.
		/// <para>
		/// Sets the subject criterion. The specified distinguished name
		/// must match the subject distinguished name in the
		/// {@code X509Certificate}. If {@code null}, any subject
		/// distinguished name will do.
		/// </para>
		/// <para>
		/// If {@code subjectDN} is not {@code null}, it should contain a
		/// distinguished name, in RFC 2253 format.
		/// 
		/// </para>
		/// </summary>
		/// <param name="subjectDN"> a distinguished name in RFC 2253 format
		///                  (or {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs (incorrect form for DN) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubject(String subjectDN) throws java.io.IOException
		public virtual String Subject
		{
			set
			{
				if (value == null)
				{
					Subject_Renamed = null;
				}
				else
				{
					Subject_Renamed = (new X500Name(value)).asX500Principal();
				}
			}
		}

		/// <summary>
		/// Sets the subject criterion. The specified distinguished name
		/// must match the subject distinguished name in the
		/// {@code X509Certificate}. If {@code null}, any subject
		/// distinguished name will do.
		/// <para>
		/// If {@code subjectDN} is not {@code null}, it should contain a
		/// single DER encoded distinguished name, as defined in X.501. For the ASN.1
		/// notation for this structure, see
		/// <seealso cref="#setIssuer(byte [] issuerDN) setIssuer(byte [] issuerDN)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="subjectDN"> a byte array containing the distinguished name in
		///                  ASN.1 DER format (or {@code null}) </param>
		/// <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubject(byte[] subjectDN) throws java.io.IOException
		public virtual sbyte[] Subject
		{
			set
			{
				try
				{
					Subject_Renamed = (value == null ? null : new X500Principal(value));
				}
				catch (IllegalArgumentException e)
				{
					throw new IOException("Invalid name", e);
				}
			}
		}

		/// <summary>
		/// Sets the subjectKeyIdentifier criterion. The
		/// {@code X509Certificate} must contain a SubjectKeyIdentifier
		/// extension for which the contents of the extension
		/// matches the specified criterion value.
		/// If the criterion value is {@code null}, no
		/// subjectKeyIdentifier check will be done.
		/// <para>
		/// If {@code subjectKeyID} is not {@code null}, it
		/// should contain a single DER encoded value corresponding to the contents
		/// of the extension value (not including the object identifier,
		/// criticality setting, and encapsulating OCTET STRING)
		/// for a SubjectKeyIdentifier extension.
		/// The ASN.1 notation for this structure follows.
		/// 
		/// <pre>{@code
		/// SubjectKeyIdentifier ::= KeyIdentifier
		/// 
		/// KeyIdentifier ::= OCTET STRING
		/// }</pre>
		/// </para>
		/// <para>
		/// Since the format of subject key identifiers is not mandated by
		/// any standard, subject key identifiers are not parsed by the
		/// {@code X509CertSelector}. Instead, the values are compared using
		/// a byte-by-byte comparison.
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="subjectKeyID"> the subject key identifier (or {@code null}) </param>
		/// <seealso cref= #getSubjectKeyIdentifier </seealso>
		public virtual sbyte[] SubjectKeyIdentifier
		{
			set
			{
				if (value == null)
				{
					this.SubjectKeyID = null;
				}
				else
				{
					this.SubjectKeyID = value.clone();
				}
			}
			get
			{
				if (SubjectKeyID == null)
				{
					return null;
				}
				return SubjectKeyID.clone();
			}
		}

		/// <summary>
		/// Sets the authorityKeyIdentifier criterion. The
		/// {@code X509Certificate} must contain an
		/// AuthorityKeyIdentifier extension for which the contents of the
		/// extension value matches the specified criterion value.
		/// If the criterion value is {@code null}, no
		/// authorityKeyIdentifier check will be done.
		/// <para>
		/// If {@code authorityKeyID} is not {@code null}, it
		/// should contain a single DER encoded value corresponding to the contents
		/// of the extension value (not including the object identifier,
		/// criticality setting, and encapsulating OCTET STRING)
		/// for an AuthorityKeyIdentifier extension.
		/// The ASN.1 notation for this structure follows.
		/// 
		/// <pre>{@code
		/// AuthorityKeyIdentifier ::= SEQUENCE {
		///    keyIdentifier             [0] KeyIdentifier           OPTIONAL,
		///    authorityCertIssuer       [1] GeneralNames            OPTIONAL,
		///    authorityCertSerialNumber [2] CertificateSerialNumber OPTIONAL  }
		/// 
		/// KeyIdentifier ::= OCTET STRING
		/// }</pre>
		/// </para>
		/// <para>
		/// Authority key identifiers are not parsed by the
		/// {@code X509CertSelector}.  Instead, the values are
		/// compared using a byte-by-byte comparison.
		/// </para>
		/// <para>
		/// When the {@code keyIdentifier} field of
		/// {@code AuthorityKeyIdentifier} is populated, the value is
		/// usually taken from the {@code SubjectKeyIdentifier} extension
		/// in the issuer's certificate.  Note, however, that the result of
		/// {@code X509Certificate.getExtensionValue(<SubjectKeyIdentifier Object
		/// Identifier>)} on the issuer's certificate may NOT be used
		/// directly as the input to {@code setAuthorityKeyIdentifier}.
		/// This is because the SubjectKeyIdentifier contains
		/// only a KeyIdentifier OCTET STRING, and not a SEQUENCE of
		/// KeyIdentifier, GeneralNames, and CertificateSerialNumber.
		/// In order to use the extension value of the issuer certificate's
		/// {@code SubjectKeyIdentifier}
		/// extension, it will be necessary to extract the value of the embedded
		/// {@code KeyIdentifier} OCTET STRING, then DER encode this OCTET
		/// STRING inside a SEQUENCE.
		/// For more details on SubjectKeyIdentifier, see
		/// <seealso cref="#setSubjectKeyIdentifier(byte[] subjectKeyID)"/>.
		/// </para>
		/// <para>
		/// Note also that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="authorityKeyID"> the authority key identifier
		///        (or {@code null}) </param>
		/// <seealso cref= #getAuthorityKeyIdentifier </seealso>
		public virtual sbyte[] AuthorityKeyIdentifier
		{
			set
			{
				if (value == null)
				{
					this.AuthorityKeyID = null;
				}
				else
				{
					this.AuthorityKeyID = value.clone();
				}
			}
			get
			{
				if (AuthorityKeyID == null)
				{
				  return null;
				}
				return AuthorityKeyID.clone();
			}
		}

		/// <summary>
		/// Sets the certificateValid criterion. The specified date must fall
		/// within the certificate validity period for the
		/// {@code X509Certificate}. If {@code null}, no certificateValid
		/// check will be done.
		/// <para>
		/// Note that the {@code Date} supplied here is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certValid"> the {@code Date} to check (or {@code null}) </param>
		/// <seealso cref= #getCertificateValid </seealso>
		public virtual Date CertificateValid
		{
			set
			{
				if (value == null)
				{
					CertificateValid_Renamed = null;
				}
				else
				{
					CertificateValid_Renamed = (Date)value.Clone();
				}
			}
			get
			{
				if (CertificateValid_Renamed == null)
				{
					return null;
				}
				return (Date)CertificateValid_Renamed.Clone();
			}
		}

		/// <summary>
		/// Sets the privateKeyValid criterion. The specified date must fall
		/// within the private key validity period for the
		/// {@code X509Certificate}. If {@code null}, no privateKeyValid
		/// check will be done.
		/// <para>
		/// Note that the {@code Date} supplied here is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="privateKeyValid"> the {@code Date} to check (or
		///                        {@code null}) </param>
		/// <seealso cref= #getPrivateKeyValid </seealso>
		public virtual Date PrivateKeyValid
		{
			set
			{
				if (value == null)
				{
					this.PrivateKeyValid_Renamed = null;
				}
				else
				{
					this.PrivateKeyValid_Renamed = (Date)value.Clone();
				}
			}
			get
			{
				if (PrivateKeyValid_Renamed == null)
				{
					return null;
				}
				return (Date)PrivateKeyValid_Renamed.Clone();
			}
		}

		/// <summary>
		/// Sets the subjectPublicKeyAlgID criterion. The
		/// {@code X509Certificate} must contain a subject public key
		/// with the specified algorithm. If {@code null}, no
		/// subjectPublicKeyAlgID check will be done.
		/// </summary>
		/// <param name="oid"> The object identifier (OID) of the algorithm to check
		///            for (or {@code null}). An OID is represented by a
		///            set of nonnegative integers separated by periods. </param>
		/// <exception cref="IOException"> if the OID is invalid, such as
		/// the first component being not 0, 1 or 2 or the second component
		/// being greater than 39.
		/// </exception>
		/// <seealso cref= #getSubjectPublicKeyAlgID </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubjectPublicKeyAlgID(String oid) throws java.io.IOException
		public virtual String SubjectPublicKeyAlgID
		{
			set
			{
				if (value == null)
				{
					SubjectPublicKeyAlgID_Renamed = null;
				}
				else
				{
					SubjectPublicKeyAlgID_Renamed = new ObjectIdentifier(value);
				}
			}
			get
			{
				if (SubjectPublicKeyAlgID_Renamed == null)
				{
					return null;
				}
				return SubjectPublicKeyAlgID_Renamed.ToString();
			}
		}

		/// <summary>
		/// Sets the subjectPublicKey criterion. The
		/// {@code X509Certificate} must contain the specified subject public
		/// key. If {@code null}, no subjectPublicKey check will be done.
		/// </summary>
		/// <param name="key"> the subject public key to check for (or {@code null}) </param>
		/// <seealso cref= #getSubjectPublicKey </seealso>
		public virtual PublicKey SubjectPublicKey
		{
			set
			{
				if (value == null)
				{
					SubjectPublicKey_Renamed = null;
					SubjectPublicKeyBytes = null;
				}
				else
				{
					SubjectPublicKey_Renamed = value;
					SubjectPublicKeyBytes = value.Encoded;
				}
			}
			get
			{
				return SubjectPublicKey_Renamed;
			}
		}

		/// <summary>
		/// Sets the subjectPublicKey criterion. The {@code X509Certificate}
		/// must contain the specified subject public key. If {@code null},
		/// no subjectPublicKey check will be done.
		/// <para>
		/// Because this method allows the public key to be specified as a byte
		/// array, it may be used for unknown key types.
		/// </para>
		/// <para>
		/// If {@code key} is not {@code null}, it should contain a
		/// single DER encoded SubjectPublicKeyInfo structure, as defined in X.509.
		/// The ASN.1 notation for this structure is as follows.
		/// <pre>{@code
		/// SubjectPublicKeyInfo  ::=  SEQUENCE  {
		///   algorithm            AlgorithmIdentifier,
		///   subjectPublicKey     BIT STRING  }
		/// 
		/// AlgorithmIdentifier  ::=  SEQUENCE  {
		///   algorithm               OBJECT IDENTIFIER,
		///   parameters              ANY DEFINED BY algorithm OPTIONAL  }
		///                              -- contains a value of the type
		///                              -- registered for use with the
		///                              -- algorithm object identifier value
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> a byte array containing the subject public key in ASN.1 DER
		///            form (or {@code null}) </param>
		/// <exception cref="IOException"> if an encoding error occurs (incorrect form for
		/// subject public key) </exception>
		/// <seealso cref= #getSubjectPublicKey </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubjectPublicKey(byte[] key) throws java.io.IOException
		public virtual sbyte[] SubjectPublicKey
		{
			set
			{
				if (value == null)
				{
					SubjectPublicKey_Renamed = null;
					SubjectPublicKeyBytes = null;
				}
				else
				{
					SubjectPublicKeyBytes = value.clone();
					SubjectPublicKey_Renamed = X509Key.parse(new DerValue(SubjectPublicKeyBytes));
				}
			}
		}

		/// <summary>
		/// Sets the keyUsage criterion. The {@code X509Certificate}
		/// must allow the specified keyUsage values. If {@code null}, no
		/// keyUsage check will be done. Note that an {@code X509Certificate}
		/// that has no keyUsage extension implicitly allows all keyUsage values.
		/// <para>
		/// Note that the boolean array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keyUsage"> a boolean array in the same format as the boolean
		///                 array returned by
		/// <seealso cref="X509Certificate#getKeyUsage() X509Certificate.getKeyUsage()"/>.
		///                 Or {@code null}. </param>
		/// <seealso cref= #getKeyUsage </seealso>
		public virtual bool[] KeyUsage
		{
			set
			{
				if (value == null)
				{
					this.KeyUsage_Renamed = null;
				}
				else
				{
					this.KeyUsage_Renamed = value.clone();
				}
			}
			get
			{
				if (KeyUsage_Renamed == null)
				{
					return null;
				}
				return KeyUsage_Renamed.clone();
			}
		}

		/// <summary>
		/// Sets the extendedKeyUsage criterion. The {@code X509Certificate}
		/// must allow the specified key purposes in its extended key usage
		/// extension. If {@code keyPurposeSet} is empty or {@code null},
		/// no extendedKeyUsage check will be done. Note that an
		/// {@code X509Certificate} that has no extendedKeyUsage extension
		/// implicitly allows all key purposes.
		/// <para>
		/// Note that the {@code Set} is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keyPurposeSet"> a {@code Set} of key purpose OIDs in string
		/// format (or {@code null}). Each OID is represented by a set of
		/// nonnegative integers separated by periods. </param>
		/// <exception cref="IOException"> if the OID is invalid, such as
		/// the first component being not 0, 1 or 2 or the second component
		/// being greater than 39. </exception>
		/// <seealso cref= #getExtendedKeyUsage </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setExtendedKeyUsage(Set<String> keyPurposeSet) throws java.io.IOException
		public virtual Set<String> ExtendedKeyUsage
		{
			set
			{
				if ((value == null) || value.Count == 0)
				{
					this.KeyPurposeSet = null;
					KeyPurposeOIDSet = null;
				}
				else
				{
					this.KeyPurposeSet = Collections.UnmodifiableSet(new HashSet<String>(value));
					KeyPurposeOIDSet = new HashSet<ObjectIdentifier>();
					foreach (String s in this.KeyPurposeSet)
					{
						KeyPurposeOIDSet.Add(new ObjectIdentifier(s));
					}
				}
			}
			get
			{
				return KeyPurposeSet;
			}
		}

		/// <summary>
		/// Enables/disables matching all of the subjectAlternativeNames
		/// specified in the {@link #setSubjectAlternativeNames
		/// setSubjectAlternativeNames} or {@link #addSubjectAlternativeName
		/// addSubjectAlternativeName} methods. If enabled,
		/// the {@code X509Certificate} must contain all of the
		/// specified subject alternative names. If disabled, the
		/// {@code X509Certificate} must contain at least one of the
		/// specified subject alternative names.
		/// 
		/// <para>The matchAllNames flag is {@code true} by default.
		/// 
		/// </para>
		/// </summary>
		/// <param name="matchAllNames"> if {@code true}, the flag is enabled;
		/// if {@code false}, the flag is disabled. </param>
		/// <seealso cref= #getMatchAllSubjectAltNames </seealso>
		public virtual bool MatchAllSubjectAltNames
		{
			set
			{
				this.MatchAllSubjectAltNames_Renamed = value;
			}
			get
			{
				return MatchAllSubjectAltNames_Renamed;
			}
		}

		/// <summary>
		/// Sets the subjectAlternativeNames criterion. The
		/// {@code X509Certificate} must contain all or at least one of the
		/// specified subjectAlternativeNames, depending on the value of
		/// the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		/// setMatchAllSubjectAltNames}).
		/// <para>
		/// This method allows the caller to specify, with a single method call,
		/// the complete set of subject alternative names for the
		/// subjectAlternativeNames criterion. The specified value replaces
		/// the previous value for the subjectAlternativeNames criterion.
		/// </para>
		/// <para>
		/// The {@code names} parameter (if not {@code null}) is a
		/// {@code Collection} with one
		/// entry for each name to be included in the subject alternative name
		/// criterion. Each entry is a {@code List} whose first entry is an
		/// {@code Integer} (the name type, 0-8) and whose second
		/// entry is a {@code String} or a byte array (the name, in
		/// string or ASN.1 DER encoded form, respectively).
		/// There can be multiple names of the same type. If {@code null}
		/// is supplied as the value for this argument, no
		/// subjectAlternativeNames check will be performed.
		/// </para>
		/// <para>
		/// Each subject alternative name in the {@code Collection}
		/// may be specified either as a {@code String} or as an ASN.1 encoded
		/// byte array. For more details about the formats used, see
		/// {@link #addSubjectAlternativeName(int type, String name)
		/// addSubjectAlternativeName(int type, String name)} and
		/// {@link #addSubjectAlternativeName(int type, byte [] name)
		/// addSubjectAlternativeName(int type, byte [] name)}.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> for distinguished names, specify the byte
		/// array form instead of the String form. See the note in
		/// <seealso cref="#addSubjectAlternativeName(int, String)"/> for more information.
		/// </para>
		/// <para>
		/// Note that the {@code names} parameter can contain duplicate
		/// names (same name and name type), but they may be removed from the
		/// {@code Collection} of names returned by the
		/// <seealso cref="#getSubjectAlternativeNames getSubjectAlternativeNames"/> method.
		/// </para>
		/// <para>
		/// Note that a deep copy is performed on the {@code Collection} to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="names"> a {@code Collection} of names (or {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
		/// <seealso cref= #getSubjectAlternativeNames </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubjectAlternativeNames(Collection<List<?>> names) throws java.io.IOException
		public virtual Collection<T1> SubjectAlternativeNames<T1>
		{
			set
			{
				if (value == null)
				{
					SubjectAlternativeNames_Renamed = null;
					SubjectAlternativeGeneralNames = null;
				}
				else
				{
					if (value.Empty)
					{
						SubjectAlternativeNames_Renamed = null;
						SubjectAlternativeGeneralNames = null;
						return;
					}
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Set<List<?>> tempNames = cloneAndCheckNames(value);
					Set<List<?>> tempNames = CloneAndCheckNames(value);
					// Ensure that we either set both of these or neither
					SubjectAlternativeGeneralNames = ParseNames(tempNames);
					SubjectAlternativeNames_Renamed = tempNames;
				}
			}
			get
			{
				if (SubjectAlternativeNames_Renamed == null)
				{
					return null;
				}
				return CloneNames(SubjectAlternativeNames_Renamed);
			}
		}

		/// <summary>
		/// Adds a name to the subjectAlternativeNames criterion. The
		/// {@code X509Certificate} must contain all or at least one
		/// of the specified subjectAlternativeNames, depending on the value of
		/// the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		/// setMatchAllSubjectAltNames}).
		/// <para>
		/// This method allows the caller to add a name to the set of subject
		/// alternative names.
		/// The specified name is added to any previous value for the
		/// subjectAlternativeNames criterion. If the specified name is a
		/// duplicate, it may be ignored.
		/// </para>
		/// <para>
		/// The name is provided in string format.
		/// <a href="http://www.ietf.org/rfc/rfc822.txt">RFC 822</a>, DNS, and URI
		/// names use the well-established string formats for those types (subject to
		/// the restrictions included in RFC 3280). IPv4 address names are
		/// supplied using dotted quad notation. OID address names are represented
		/// as a series of nonnegative integers separated by periods. And
		/// directory names (distinguished names) are supplied in RFC 2253 format.
		/// No standard string format is defined for otherNames, X.400 names,
		/// EDI party names, IPv6 address names, or any other type of names. They
		/// should be specified using the
		/// {@link #addSubjectAlternativeName(int type, byte [] name)
		/// addSubjectAlternativeName(int type, byte [] name)}
		/// method.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> for distinguished names, use
		/// <seealso cref="#addSubjectAlternativeName(int, byte[])"/> instead.
		/// This method should not be relied on as it can fail to match some
		/// certificates because of a loss of encoding information in the RFC 2253
		/// String form of some distinguished names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the name type (0-8, as specified in
		///             RFC 3280, section 4.2.1.7) </param>
		/// <param name="name"> the name in string form (not {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addSubjectAlternativeName(int type, String name) throws java.io.IOException
		public virtual void AddSubjectAlternativeName(int type, String name)
		{
			AddSubjectAlternativeNameInternal(type, name);
		}

		/// <summary>
		/// Adds a name to the subjectAlternativeNames criterion. The
		/// {@code X509Certificate} must contain all or at least one
		/// of the specified subjectAlternativeNames, depending on the value of
		/// the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		/// setMatchAllSubjectAltNames}).
		/// <para>
		/// This method allows the caller to add a name to the set of subject
		/// alternative names.
		/// The specified name is added to any previous value for the
		/// subjectAlternativeNames criterion. If the specified name is a
		/// duplicate, it may be ignored.
		/// </para>
		/// <para>
		/// The name is provided as a byte array. This byte array should contain
		/// the DER encoded name, as it would appear in the GeneralName structure
		/// defined in RFC 3280 and X.509. The encoded byte array should only contain
		/// the encoded value of the name, and should not include the tag associated
		/// with the name in the GeneralName structure. The ASN.1 definition of this
		/// structure appears below.
		/// <pre>{@code
		///  GeneralName ::= CHOICE {
		///       otherName                       [0]     OtherName,
		///       rfc822Name                      [1]     IA5String,
		///       dNSName                         [2]     IA5String,
		///       x400Address                     [3]     ORAddress,
		///       directoryName                   [4]     Name,
		///       ediPartyName                    [5]     EDIPartyName,
		///       uniformResourceIdentifier       [6]     IA5String,
		///       iPAddress                       [7]     OCTET STRING,
		///       registeredID                    [8]     OBJECT IDENTIFIER}
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the name type (0-8, as listed above) </param>
		/// <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addSubjectAlternativeName(int type, byte[] name) throws java.io.IOException
		public virtual void AddSubjectAlternativeName(int type, sbyte[] name)
		{
			// clone because byte arrays are modifiable
			AddSubjectAlternativeNameInternal(type, name.clone());
		}

		/// <summary>
		/// A private method that adds a name (String or byte array) to the
		/// subjectAlternativeNames criterion. The {@code X509Certificate}
		/// must contain the specified subjectAlternativeName.
		/// </summary>
		/// <param name="type"> the name type (0-8, as specified in
		///             RFC 3280, section 4.2.1.7) </param>
		/// <param name="name"> the name in string or byte array form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void addSubjectAlternativeNameInternal(int type, Object name) throws java.io.IOException
		private void AddSubjectAlternativeNameInternal(int type, Object name)
		{
			// First, ensure that the name parses
			GeneralNameInterface tempName = MakeGeneralNameInterface(type, name);
			if (SubjectAlternativeNames_Renamed == null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: subjectAlternativeNames = new HashSet<List<?>>();
				SubjectAlternativeNames_Renamed = new HashSet<List<?>>();
			}
			if (SubjectAlternativeGeneralNames == null)
			{
				SubjectAlternativeGeneralNames = new HashSet<GeneralNameInterface>();
			}
			List<Object> list = new List<Object>(2);
			list.Add(Convert.ToInt32(type));
			list.Add(name);
			SubjectAlternativeNames_Renamed.Add(list);
			SubjectAlternativeGeneralNames.Add(tempName);
		}

		/// <summary>
		/// Parse an argument of the form passed to setSubjectAlternativeNames,
		/// returning a {@code Collection} of
		/// {@code GeneralNameInterface}s.
		/// Throw an IllegalArgumentException or a ClassCastException
		/// if the argument is malformed.
		/// </summary>
		/// <param name="names"> a Collection with one entry per name.
		///              Each entry is a {@code List} whose first entry
		///              is an Integer (the name type, 0-8) and whose second
		///              entry is a String or a byte array (the name, in
		///              string or ASN.1 DER encoded form, respectively).
		///              There can be multiple names of the same type. Null is
		///              not an acceptable value. </param>
		/// <returns> a Set of {@code GeneralNameInterface}s </returns>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Set<GeneralNameInterface> parseNames(Collection<List<?>> names) throws java.io.IOException
		private static Set<GeneralNameInterface> ParseNames(Collection<T1> names)
		{
			Set<GeneralNameInterface> genNames = new HashSet<GeneralNameInterface>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (List<?> nameList : names)
			foreach (List<?> nameList in names)
			{
				if (nameList.Count != 2)
				{
					throw new IOException("name list size not 2");
				}
				Object o = nameList.Get(0);
				if (!(o is Integer))
				{
					throw new IOException("expected an Integer");
				}
				int nameType = ((Integer)o).IntValue();
				o = nameList.Get(1);
				genNames.Add(MakeGeneralNameInterface(nameType, o));
			}

			return genNames;
		}

		/// <summary>
		/// Compare for equality two objects of the form passed to
		/// setSubjectAlternativeNames (or X509CRLSelector.setIssuerNames).
		/// Throw an {@code IllegalArgumentException} or a
		/// {@code ClassCastException} if one of the objects is malformed.
		/// </summary>
		/// <param name="object1"> a Collection containing the first object to compare </param>
		/// <param name="object2"> a Collection containing the second object to compare </param>
		/// <returns> true if the objects are equal, false otherwise </returns>
		internal static bool equalNames<T1, T2>(Collection<T1> object1, Collection<T2> object2)
		{
			if ((object1 == null) || (object2 == null))
			{
				return object1 == object2;
			}
			return object1.Equals(object2);
		}

		/// <summary>
		/// Make a {@code GeneralNameInterface} out of a name type (0-8) and an
		/// Object that may be a byte array holding the ASN.1 DER encoded
		/// name or a String form of the name.  Except for X.509
		/// Distinguished Names, the String form of the name must not be the
		/// result from calling toString on an existing GeneralNameInterface
		/// implementing class.  The output of toString is not compatible
		/// with the String constructors for names other than Distinguished
		/// Names.
		/// </summary>
		/// <param name="type"> name type (0-8) </param>
		/// <param name="name"> name as ASN.1 Der-encoded byte array or String </param>
		/// <returns> a GeneralNameInterface name </returns>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static GeneralNameInterface makeGeneralNameInterface(int type, Object name) throws java.io.IOException
		internal static GeneralNameInterface MakeGeneralNameInterface(int type, Object name)
		{
			GeneralNameInterface result;
			if (Debug != null)
			{
				Debug.println("X509CertSelector.makeGeneralNameInterface(" + type + ")...");
			}

			if (name is String)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.makeGeneralNameInterface() " + "name is String: " + name);
				}
				switch (type)
				{
				case NAME_RFC822:
					result = new RFC822Name((String)name);
					break;
				case NAME_DNS:
					result = new DNSName((String)name);
					break;
				case NAME_DIRECTORY:
					result = new X500Name((String)name);
					break;
				case NAME_URI:
					result = new URIName((String)name);
					break;
				case NAME_IP:
					result = new IPAddressName((String)name);
					break;
				case NAME_OID:
					result = new OIDName((String)name);
					break;
				default:
					throw new IOException("unable to parse String names of type " + type);
				}
				if (Debug != null)
				{
					Debug.println("X509CertSelector.makeGeneralNameInterface() " + "result: " + result.ToString());
				}
			}
			else if (name is sbyte[])
			{
				DerValue val = new DerValue((sbyte[]) name);
				if (Debug != null)
				{
					Debug.println("X509CertSelector.makeGeneralNameInterface() is byte[]");
				}

				switch (type)
				{
				case NAME_ANY:
					result = new OtherName(val);
					break;
				case NAME_RFC822:
					result = new RFC822Name(val);
					break;
				case NAME_DNS:
					result = new DNSName(val);
					break;
				case NAME_X400:
					result = new X400Address(val);
					break;
				case NAME_DIRECTORY:
					result = new X500Name(val);
					break;
				case NAME_EDI:
					result = new EDIPartyName(val);
					break;
				case NAME_URI:
					result = new URIName(val);
					break;
				case NAME_IP:
					result = new IPAddressName(val);
					break;
				case NAME_OID:
					result = new OIDName(val);
					break;
				default:
					throw new IOException("unable to parse byte array names of " + "type " + type);
				}
				if (Debug != null)
				{
					Debug.println("X509CertSelector.makeGeneralNameInterface() result: " + result.ToString());
				}
			}
			else
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.makeGeneralName() input name " + "not String or byte array");
				}
				throw new IOException("name not String or byte array");
			}
			return result;
		}


		/// <summary>
		/// Sets the name constraints criterion. The {@code X509Certificate}
		/// must have subject and subject alternative names that
		/// meet the specified name constraints.
		/// <para>
		/// The name constraints are specified as a byte array. This byte array
		/// should contain the DER encoded form of the name constraints, as they
		/// would appear in the NameConstraints structure defined in RFC 3280
		/// and X.509. The ASN.1 definition of this structure appears below.
		/// 
		/// <pre>{@code
		///  NameConstraints ::= SEQUENCE {
		///       permittedSubtrees       [0]     GeneralSubtrees OPTIONAL,
		///       excludedSubtrees        [1]     GeneralSubtrees OPTIONAL }
		/// 
		///  GeneralSubtrees ::= SEQUENCE SIZE (1..MAX) OF GeneralSubtree
		/// 
		///  GeneralSubtree ::= SEQUENCE {
		///       base                    GeneralName,
		///       minimum         [0]     BaseDistance DEFAULT 0,
		///       maximum         [1]     BaseDistance OPTIONAL }
		/// 
		///  BaseDistance ::= INTEGER (0..MAX)
		/// 
		///  GeneralName ::= CHOICE {
		///       otherName                       [0]     OtherName,
		///       rfc822Name                      [1]     IA5String,
		///       dNSName                         [2]     IA5String,
		///       x400Address                     [3]     ORAddress,
		///       directoryName                   [4]     Name,
		///       ediPartyName                    [5]     EDIPartyName,
		///       uniformResourceIdentifier       [6]     IA5String,
		///       iPAddress                       [7]     OCTET STRING,
		///       registeredID                    [8]     OBJECT IDENTIFIER}
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes"> a byte array containing the ASN.1 DER encoding of
		///              a NameConstraints extension to be used for checking
		///              name constraints. Only the value of the extension is
		///              included, not the OID or criticality flag. Can be
		///              {@code null},
		///              in which case no name constraints check will be performed. </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
		/// <seealso cref= #getNameConstraints </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setNameConstraints(byte[] bytes) throws java.io.IOException
		public virtual sbyte[] NameConstraints
		{
			set
			{
				if (value == null)
				{
					NcBytes = null;
					Nc = null;
				}
				else
				{
					NcBytes = value.clone();
					Nc = new NameConstraintsExtension(FALSE, value);
				}
			}
			get
			{
				if (NcBytes == null)
				{
					return null;
				}
				else
				{
					return NcBytes.clone();
				}
			}
		}

		/// <summary>
		/// Sets the basic constraints constraint. If the value is greater than or
		/// equal to zero, {@code X509Certificates} must include a
		/// basicConstraints extension with
		/// a pathLen of at least this value. If the value is -2, only end-entity
		/// certificates are accepted. If the value is -1, no check is done.
		/// <para>
		/// This constraint is useful when building a certification path forward
		/// (from the target toward the trust anchor. If a partial path has been
		/// built, any candidate certificate must have a maxPathLen value greater
		/// than or equal to the number of certificates in the partial path.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minMaxPathLen"> the value for the basic constraints constraint </param>
		/// <exception cref="IllegalArgumentException"> if the value is less than -2 </exception>
		/// <seealso cref= #getBasicConstraints </seealso>
		public virtual int BasicConstraints
		{
			set
			{
				if (value < -2)
				{
					throw new IllegalArgumentException("basic constraints less than -2");
				}
				BasicConstraints_Renamed = value;
			}
			get
			{
				return BasicConstraints_Renamed;
			}
		}

		/// <summary>
		/// Sets the policy constraint. The {@code X509Certificate} must
		/// include at least one of the specified policies in its certificate
		/// policies extension. If {@code certPolicySet} is empty, then the
		/// {@code X509Certificate} must include at least some specified policy
		/// in its certificate policies extension. If {@code certPolicySet} is
		/// {@code null}, no policy check will be performed.
		/// <para>
		/// Note that the {@code Set} is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certPolicySet"> a {@code Set} of certificate policy OIDs in
		///                      string format (or {@code null}). Each OID is
		///                      represented by a set of nonnegative integers
		///                    separated by periods. </param>
		/// <exception cref="IOException"> if a parsing error occurs on the OID such as
		/// the first component is not 0, 1 or 2 or the second component is
		/// greater than 39. </exception>
		/// <seealso cref= #getPolicy </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPolicy(Set<String> certPolicySet) throws java.io.IOException
		public virtual Set<String> Policy
		{
			set
			{
				if (value == null)
				{
					PolicySet = null;
					Policy_Renamed = null;
				}
				else
				{
					// Snapshot set and parse it
					Set<String> tempSet = Collections.UnmodifiableSet(new HashSet<String>(value));
					/* Convert to Vector of ObjectIdentifiers */
					Iterator<String> i = tempSet.Iterator();
					Vector<CertificatePolicyId> polIdVector = new Vector<CertificatePolicyId>();
					while (i.HasNext())
					{
						Object o = i.Next();
						if (!(o is String))
						{
							throw new IOException("non String in certPolicySet");
						}
						polIdVector.Add(new CertificatePolicyId(new ObjectIdentifier((String)o)));
					}
					// If everything went OK, make the changes
					PolicySet = tempSet;
					Policy_Renamed = new CertificatePolicySet(polIdVector);
				}
			}
			get
			{
				return PolicySet;
			}
		}

		/// <summary>
		/// Sets the pathToNames criterion. The {@code X509Certificate} must
		/// not include name constraints that would prohibit building a
		/// path to the specified names.
		/// <para>
		/// This method allows the caller to specify, with a single method call,
		/// the complete set of names which the {@code X509Certificates}'s
		/// name constraints must permit. The specified value replaces
		/// the previous value for the pathToNames criterion.
		/// </para>
		/// <para>
		/// This constraint is useful when building a certification path forward
		/// (from the target toward the trust anchor. If a partial path has been
		/// built, any candidate certificate must not include name constraints that
		/// would prohibit building a path to any of the names in the partial path.
		/// </para>
		/// <para>
		/// The {@code names} parameter (if not {@code null}) is a
		/// {@code Collection} with one
		/// entry for each name to be included in the pathToNames
		/// criterion. Each entry is a {@code List} whose first entry is an
		/// {@code Integer} (the name type, 0-8) and whose second
		/// entry is a {@code String} or a byte array (the name, in
		/// string or ASN.1 DER encoded form, respectively).
		/// There can be multiple names of the same type. If {@code null}
		/// is supplied as the value for this argument, no
		/// pathToNames check will be performed.
		/// </para>
		/// <para>
		/// Each name in the {@code Collection}
		/// may be specified either as a {@code String} or as an ASN.1 encoded
		/// byte array. For more details about the formats used, see
		/// {@link #addPathToName(int type, String name)
		/// addPathToName(int type, String name)} and
		/// {@link #addPathToName(int type, byte [] name)
		/// addPathToName(int type, byte [] name)}.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> for distinguished names, specify the byte
		/// array form instead of the String form. See the note in
		/// <seealso cref="#addPathToName(int, String)"/> for more information.
		/// </para>
		/// <para>
		/// Note that the {@code names} parameter can contain duplicate
		/// names (same name and name type), but they may be removed from the
		/// {@code Collection} of names returned by the
		/// <seealso cref="#getPathToNames getPathToNames"/> method.
		/// </para>
		/// <para>
		/// Note that a deep copy is performed on the {@code Collection} to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="names"> a {@code Collection} with one entry per name
		///              (or {@code null}) </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
		/// <seealso cref= #getPathToNames </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPathToNames(Collection<List<?>> names) throws java.io.IOException
		public virtual Collection<T1> PathToNames<T1>
		{
			set
			{
				if ((value == null) || value.Empty)
				{
					PathToNames_Renamed = null;
					PathToGeneralNames = null;
				}
				else
				{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Set<List<?>> tempNames = cloneAndCheckNames(value);
					Set<List<?>> tempNames = CloneAndCheckNames(value);
					PathToGeneralNames = ParseNames(tempNames);
					// Ensure that we either set both of these or neither
					PathToNames_Renamed = tempNames;
				}
			}
			get
			{
				if (PathToNames_Renamed == null)
				{
					return null;
				}
				return CloneNames(PathToNames_Renamed);
			}
		}

		// called from CertPathHelper
		internal virtual Set<GeneralNameInterface> PathToNamesInternal
		{
			set
			{
				// set value to non-null dummy value
				// this breaks getPathToNames()
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: pathToNames = Collections.emptySet<List<?>>();
				PathToNames_Renamed = System.Linq.Enumerable.Empty<List<?>>();
				PathToGeneralNames = value;
			}
		}

		/// <summary>
		/// Adds a name to the pathToNames criterion. The {@code X509Certificate}
		/// must not include name constraints that would prohibit building a
		/// path to the specified name.
		/// <para>
		/// This method allows the caller to add a name to the set of names which
		/// the {@code X509Certificates}'s name constraints must permit.
		/// The specified name is added to any previous value for the
		/// pathToNames criterion.  If the name is a duplicate, it may be ignored.
		/// </para>
		/// <para>
		/// The name is provided in string format. RFC 822, DNS, and URI names
		/// use the well-established string formats for those types (subject to
		/// the restrictions included in RFC 3280). IPv4 address names are
		/// supplied using dotted quad notation. OID address names are represented
		/// as a series of nonnegative integers separated by periods. And
		/// directory names (distinguished names) are supplied in RFC 2253 format.
		/// No standard string format is defined for otherNames, X.400 names,
		/// EDI party names, IPv6 address names, or any other type of names. They
		/// should be specified using the
		/// {@link #addPathToName(int type, byte [] name)
		/// addPathToName(int type, byte [] name)} method.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> for distinguished names, use
		/// <seealso cref="#addPathToName(int, byte[])"/> instead.
		/// This method should not be relied on as it can fail to match some
		/// certificates because of a loss of encoding information in the RFC 2253
		/// String form of some distinguished names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the name type (0-8, as specified in
		///             RFC 3280, section 4.2.1.7) </param>
		/// <param name="name"> the name in string form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addPathToName(int type, String name) throws java.io.IOException
		public virtual void AddPathToName(int type, String name)
		{
			AddPathToNameInternal(type, name);
		}

		/// <summary>
		/// Adds a name to the pathToNames criterion. The {@code X509Certificate}
		/// must not include name constraints that would prohibit building a
		/// path to the specified name.
		/// <para>
		/// This method allows the caller to add a name to the set of names which
		/// the {@code X509Certificates}'s name constraints must permit.
		/// The specified name is added to any previous value for the
		/// pathToNames criterion. If the name is a duplicate, it may be ignored.
		/// </para>
		/// <para>
		/// The name is provided as a byte array. This byte array should contain
		/// the DER encoded name, as it would appear in the GeneralName structure
		/// defined in RFC 3280 and X.509. The ASN.1 definition of this structure
		/// appears in the documentation for
		/// {@link #addSubjectAlternativeName(int type, byte [] name)
		/// addSubjectAlternativeName(int type, byte [] name)}.
		/// </para>
		/// <para>
		/// Note that the byte array supplied here is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the name type (0-8, as specified in
		///             RFC 3280, section 4.2.1.7) </param>
		/// <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addPathToName(int type, byte [] name) throws java.io.IOException
		public virtual void AddPathToName(int type, sbyte[] name)
		{
			// clone because byte arrays are modifiable
			AddPathToNameInternal(type, name.clone());
		}

		/// <summary>
		/// A private method that adds a name (String or byte array) to the
		/// pathToNames criterion. The {@code X509Certificate} must contain
		/// the specified pathToName.
		/// </summary>
		/// <param name="type"> the name type (0-8, as specified in
		///             RFC 3280, section 4.2.1.7) </param>
		/// <param name="name"> the name in string or byte array form </param>
		/// <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void addPathToNameInternal(int type, Object name) throws java.io.IOException
		private void AddPathToNameInternal(int type, Object name)
		{
			// First, ensure that the name parses
			GeneralNameInterface tempName = MakeGeneralNameInterface(type, name);
			if (PathToGeneralNames == null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: pathToNames = new HashSet<List<?>>();
				PathToNames_Renamed = new HashSet<List<?>>();
				PathToGeneralNames = new HashSet<GeneralNameInterface>();
			}
			List<Object> list = new List<Object>(2);
			list.Add(Convert.ToInt32(type));
			list.Add(name);
			PathToNames_Renamed.Add(list);
			PathToGeneralNames.Add(tempName);
		}




		/// <summary>
		/// <strong>Denigrated</strong>, use <seealso cref="#getIssuer()"/> or
		/// <seealso cref="#getIssuerAsBytes()"/> instead. This method should not be
		/// relied on as it can fail to match some certificates because of a loss of
		/// encoding information in the RFC 2253 String form of some distinguished
		/// names.
		/// <para>
		/// Returns the issuer criterion as a {@code String}. This
		/// distinguished name must match the issuer distinguished name in the
		/// {@code X509Certificate}. If {@code null}, the issuer criterion
		/// is disabled and any issuer distinguished name will do.
		/// </para>
		/// <para>
		/// If the value returned is not {@code null}, it is a
		/// distinguished name, in RFC 2253 format.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the required issuer distinguished name in RFC 2253 format
		///         (or {@code null}) </returns>
		public virtual String IssuerAsString
		{
			get
			{
				return (Issuer_Renamed == null ? null : Issuer_Renamed.Name);
			}
		}

		/// <summary>
		/// Returns the issuer criterion as a byte array. This distinguished name
		/// must match the issuer distinguished name in the
		/// {@code X509Certificate}. If {@code null}, the issuer criterion
		/// is disabled and any issuer distinguished name will do.
		/// <para>
		/// If the value returned is not {@code null}, it is a byte
		/// array containing a single DER encoded distinguished name, as defined in
		/// X.501. The ASN.1 notation for this structure is supplied in the
		/// documentation for
		/// <seealso cref="#setIssuer(byte [] issuerDN) setIssuer(byte [] issuerDN)"/>.
		/// </para>
		/// <para>
		/// Note that the byte array returned is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a byte array containing the required issuer distinguished name
		///         in ASN.1 DER format (or {@code null}) </returns>
		/// <exception cref="IOException"> if an encoding error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getIssuerAsBytes() throws java.io.IOException
		public virtual sbyte[] IssuerAsBytes
		{
			get
			{
				return (Issuer_Renamed == null ? null: Issuer_Renamed.Encoded);
			}
		}


		/// <summary>
		/// <strong>Denigrated</strong>, use <seealso cref="#getSubject()"/> or
		/// <seealso cref="#getSubjectAsBytes()"/> instead. This method should not be
		/// relied on as it can fail to match some certificates because of a loss of
		/// encoding information in the RFC 2253 String form of some distinguished
		/// names.
		/// <para>
		/// Returns the subject criterion as a {@code String}. This
		/// distinguished name must match the subject distinguished name in the
		/// {@code X509Certificate}. If {@code null}, the subject criterion
		/// is disabled and any subject distinguished name will do.
		/// </para>
		/// <para>
		/// If the value returned is not {@code null}, it is a
		/// distinguished name, in RFC 2253 format.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the required subject distinguished name in RFC 2253 format
		///         (or {@code null}) </returns>
		public virtual String SubjectAsString
		{
			get
			{
				return (Subject_Renamed == null ? null : Subject_Renamed.Name);
			}
		}

		/// <summary>
		/// Returns the subject criterion as a byte array. This distinguished name
		/// must match the subject distinguished name in the
		/// {@code X509Certificate}. If {@code null}, the subject criterion
		/// is disabled and any subject distinguished name will do.
		/// <para>
		/// If the value returned is not {@code null}, it is a byte
		/// array containing a single DER encoded distinguished name, as defined in
		/// X.501. The ASN.1 notation for this structure is supplied in the
		/// documentation for
		/// <seealso cref="#setSubject(byte [] subjectDN) setSubject(byte [] subjectDN)"/>.
		/// </para>
		/// <para>
		/// Note that the byte array returned is cloned to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a byte array containing the required subject distinguished name
		///         in ASN.1 DER format (or {@code null}) </returns>
		/// <exception cref="IOException"> if an encoding error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getSubjectAsBytes() throws java.io.IOException
		public virtual sbyte[] SubjectAsBytes
		{
			get
			{
				return (Subject_Renamed == null ? null : Subject_Renamed.Encoded);
			}
		}











		/// <summary>
		/// Clone an object of the form passed to
		/// setSubjectAlternativeNames and setPathToNames.
		/// Throw a {@code RuntimeException} if the argument is malformed.
		/// <para>
		/// This method wraps cloneAndCheckNames, changing any
		/// {@code IOException} into a {@code RuntimeException}. This
		/// method should be used when the object being
		/// cloned has already been checked, so there should never be any exceptions.
		/// 
		/// </para>
		/// </summary>
		/// <param name="names"> a {@code Collection} with one entry per name.
		///              Each entry is a {@code List} whose first entry
		///              is an Integer (the name type, 0-8) and whose second
		///              entry is a String or a byte array (the name, in
		///              string or ASN.1 DER encoded form, respectively).
		///              There can be multiple names of the same type. Null
		///              is not an acceptable value. </param>
		/// <returns> a deep copy of the specified {@code Collection} </returns>
		/// <exception cref="RuntimeException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Set<List<?>> cloneNames(Collection<List<?>> names)
		private static Set<List<?>> CloneNames(Collection<T1> names)
		{
			try
			{
				return CloneAndCheckNames(names);
			}
			catch (IOException e)
			{
				throw new RuntimeException("cloneNames encountered IOException: " + e.Message);
			}
		}

		/// <summary>
		/// Clone and check an argument of the form passed to
		/// setSubjectAlternativeNames and setPathToNames.
		/// Throw an {@code IOException} if the argument is malformed.
		/// </summary>
		/// <param name="names"> a {@code Collection} with one entry per name.
		///              Each entry is a {@code List} whose first entry
		///              is an Integer (the name type, 0-8) and whose second
		///              entry is a String or a byte array (the name, in
		///              string or ASN.1 DER encoded form, respectively).
		///              There can be multiple names of the same type.
		///              {@code null} is not an acceptable value. </param>
		/// <returns> a deep copy of the specified {@code Collection} </returns>
		/// <exception cref="IOException"> if a parsing error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Set<List<?>> cloneAndCheckNames(Collection<List<?>> names) throws java.io.IOException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static Set<List<?>> CloneAndCheckNames(Collection<T1> names)
		{
			// Copy the Lists and Collection
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<List<?>> namesCopy = new HashSet<List<?>>();
			Set<List<?>> namesCopy = new HashSet<List<?>>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (List<?> o : names)
			foreach (List<?> o in names)
			{
				namesCopy.Add(new List<Object>(o));
			}

			// Check the contents of the Lists and clone any byte arrays
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (List<?> list : namesCopy)
			foreach (List<?> list in namesCopy)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") List<Object> nameList = (List<Object>)list;
				List<Object> nameList = (List<Object>)list; // See javadoc for parameter "names".
				if (nameList.Count != 2)
				{
					throw new IOException("name list size not 2");
				}
				Object o = nameList.Get(0);
				if (!(o is Integer))
				{
					throw new IOException("expected an Integer");
				}
				int nameType = ((Integer)o).IntValue();
				if ((nameType < 0) || (nameType > 8))
				{
					throw new IOException("name type not 0-8");
				}
				Object nameObject = nameList.Get(1);
				if (!(nameObject is sbyte[]) && !(nameObject is String))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.cloneAndCheckNames() " + "name not byte array");
					}
					throw new IOException("name not byte array or String");
				}
				if (nameObject is sbyte[])
				{
					nameList.Set(1, ((sbyte[]) nameObject).clone());
				}
			}
			return namesCopy;
		}





		/// <summary>
		/// Return a printable representation of the {@code CertSelector}.
		/// </summary>
		/// <returns> a {@code String} describing the contents of the
		///         {@code CertSelector} </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("X509CertSelector: [\n");
			if (X509Cert != null)
			{
				sb.Append("  Certificate: " + X509Cert.ToString() + "\n");
			}
			if (SerialNumber_Renamed != null)
			{
				sb.Append("  Serial Number: " + SerialNumber_Renamed.ToString() + "\n");
			}
			if (Issuer_Renamed != null)
			{
				sb.Append("  Issuer: " + IssuerAsString + "\n");
			}
			if (Subject_Renamed != null)
			{
				sb.Append("  Subject: " + SubjectAsString + "\n");
			}
			sb.Append("  matchAllSubjectAltNames flag: " + Convert.ToString(MatchAllSubjectAltNames_Renamed) + "\n");
			if (SubjectAlternativeNames_Renamed != null)
			{
				sb.Append("  SubjectAlternativeNames:\n");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<List<?>> i = subjectAlternativeNames.iterator();
				Iterator<List<?>> i = SubjectAlternativeNames_Renamed.Iterator();
				while (i.HasNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = i.next();
					List<?> list = i.Next();
					sb.Append("    type " + list.Get(0) + ", name " + list.Get(1) + "\n");
				}
			}
			if (SubjectKeyID != null)
			{
				HexDumpEncoder enc = new HexDumpEncoder();
				sb.Append("  Subject Key Identifier: " + enc.encodeBuffer(SubjectKeyID) + "\n");
			}
			if (AuthorityKeyID != null)
			{
				HexDumpEncoder enc = new HexDumpEncoder();
				sb.Append("  Authority Key Identifier: " + enc.encodeBuffer(AuthorityKeyID) + "\n");
			}
			if (CertificateValid_Renamed != null)
			{
				sb.Append("  Certificate Valid: " + CertificateValid_Renamed.ToString() + "\n");
			}
			if (PrivateKeyValid_Renamed != null)
			{
				sb.Append("  Private Key Valid: " + PrivateKeyValid_Renamed.ToString() + "\n");
			}
			if (SubjectPublicKeyAlgID_Renamed != null)
			{
				sb.Append("  Subject Public Key AlgID: " + SubjectPublicKeyAlgID_Renamed.ToString() + "\n");
			}
			if (SubjectPublicKey_Renamed != null)
			{
				sb.Append("  Subject Public Key: " + SubjectPublicKey_Renamed.ToString() + "\n");
			}
			if (KeyUsage_Renamed != null)
			{
				sb.Append("  Key Usage: " + KeyUsageToString(KeyUsage_Renamed) + "\n");
			}
			if (KeyPurposeSet != null)
			{
				sb.Append("  Extended Key Usage: " + KeyPurposeSet.ToString() + "\n");
			}
			if (Policy_Renamed != null)
			{
				sb.Append("  Policy: " + Policy_Renamed.ToString() + "\n");
			}
			if (PathToGeneralNames != null)
			{
				sb.Append("  Path to names:\n");
				Iterator<GeneralNameInterface> i = PathToGeneralNames.Iterator();
				while (i.HasNext())
				{
					sb.Append("    " + i.Next() + "\n");
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		// Copied from sun.security.x509.KeyUsageExtension
		// (without calling the superclass)
		/// <summary>
		/// Returns a printable representation of the KeyUsage.
		/// </summary>
		private static String KeyUsageToString(bool[] k)
		{
			String s = "KeyUsage [\n";
			try
			{
				if (k[0])
				{
					s += "  DigitalSignature\n";
				}
				if (k[1])
				{
					s += "  Non_repudiation\n";
				}
				if (k[2])
				{
					s += "  Key_Encipherment\n";
				}
				if (k[3])
				{
					s += "  Data_Encipherment\n";
				}
				if (k[4])
				{
					s += "  Key_Agreement\n";
				}
				if (k[5])
				{
					s += "  Key_CertSign\n";
				}
				if (k[6])
				{
					s += "  Crl_Sign\n";
				}
				if (k[7])
				{
					s += "  Encipher_Only\n";
				}
				if (k[8])
				{
					s += "  Decipher_Only\n";
				}
			}
			catch (ArrayIndexOutOfBoundsException)
			{
			}

			s += "]\n";

			return (s);
		}

		/// <summary>
		/// Returns an Extension object given any X509Certificate and extension oid.
		/// Throw an {@code IOException} if the extension byte value is
		/// malformed.
		/// </summary>
		/// <param name="cert"> a {@code X509Certificate} </param>
		/// <param name="extId"> an {@code integer} which specifies the extension index.
		/// Currently, the supported extensions are as follows:
		/// index 0 - PrivateKeyUsageExtension
		/// index 1 - SubjectAlternativeNameExtension
		/// index 2 - NameConstraintsExtension
		/// index 3 - CertificatePoliciesExtension
		/// index 4 - ExtendedKeyUsageExtension </param>
		/// <returns> an {@code Extension} object whose real type is as specified
		/// by the extension oid. </returns>
		/// <exception cref="IOException"> if cannot construct the {@code Extension}
		/// object with the extension encoding retrieved from the passed in
		/// {@code X509Certificate}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Extension getExtensionObject(X509Certificate cert, int extId) throws java.io.IOException
		private static Extension GetExtensionObject(X509Certificate cert, int extId)
		{
			if (cert is X509CertImpl)
			{
				X509CertImpl impl = (X509CertImpl)cert;
				switch (extId)
				{
				case PRIVATE_KEY_USAGE_ID:
					return impl.PrivateKeyUsageExtension;
				case SUBJECT_ALT_NAME_ID:
					return impl.SubjectAlternativeNameExtension;
				case NAME_CONSTRAINTS_ID:
					return impl.NameConstraintsExtension;
				case CERT_POLICIES_ID:
					return impl.CertificatePoliciesExtension;
				case EXTENDED_KEY_USAGE_ID:
					return impl.ExtendedKeyUsageExtension;
				default:
					return null;
				}
			}
			sbyte[] rawExtVal = cert.GetExtensionValue(EXTENSION_OIDS[extId]);
			if (rawExtVal == null)
			{
				return null;
			}
			DerInputStream @in = new DerInputStream(rawExtVal);
			sbyte[] encoded = @in.OctetString;
			switch (extId)
			{
			case PRIVATE_KEY_USAGE_ID:
				try
				{
					return new PrivateKeyUsageExtension(FALSE, encoded);
				}
				catch (CertificateException ex)
				{
					throw new IOException(ex.Message);
				}
			case SUBJECT_ALT_NAME_ID:
				return new SubjectAlternativeNameExtension(FALSE, encoded);
			case NAME_CONSTRAINTS_ID:
				return new NameConstraintsExtension(FALSE, encoded);
			case CERT_POLICIES_ID:
				return new CertificatePoliciesExtension(FALSE, encoded);
			case EXTENDED_KEY_USAGE_ID:
				return new ExtendedKeyUsageExtension(FALSE, encoded);
			default:
				return null;
			}
		}

		/// <summary>
		/// Decides whether a {@code Certificate} should be selected.
		/// </summary>
		/// <param name="cert"> the {@code Certificate} to be checked </param>
		/// <returns> {@code true} if the {@code Certificate} should be
		///         selected, {@code false} otherwise </returns>
		public virtual bool Match(Certificate cert)
		{
			if (!(cert is X509Certificate))
			{
				return false;
			}
			X509Certificate xcert = (X509Certificate)cert;

			if (Debug != null)
			{
				Debug.println("X509CertSelector.match(SN: " + (xcert.SerialNumber).ToString(16) + "\n  Issuer: " + xcert.IssuerDN + "\n  Subject: " + xcert.SubjectDN + ")");
			}

			/* match on X509Certificate */
			if (X509Cert != null)
			{
				if (!X509Cert.Equals(xcert))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "certs don't match");
					}
					return false;
				}
			}

			/* match on serial number */
			if (SerialNumber_Renamed != null)
			{
				if (!SerialNumber_Renamed.Equals(xcert.SerialNumber))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "serial numbers don't match");
					}
					return false;
				}
			}

			/* match on issuer name */
			if (Issuer_Renamed != null)
			{
				if (!Issuer_Renamed.Equals(xcert.IssuerX500Principal))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "issuer DNs don't match");
					}
					return false;
				}
			}

			/* match on subject name */
			if (Subject_Renamed != null)
			{
				if (!Subject_Renamed.Equals(xcert.SubjectX500Principal))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "subject DNs don't match");
					}
					return false;
				}
			}

			/* match on certificate validity range */
			if (CertificateValid_Renamed != null)
			{
				try
				{
					xcert.CheckValidity(CertificateValid_Renamed);
				}
				catch (CertificateException)
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "certificate not within validity period");
					}
					return false;
				}
			}

			/* match on subject public key */
			if (SubjectPublicKeyBytes != null)
			{
				sbyte[] certKey = xcert.PublicKey.Encoded;
				if (!System.Array.Equals(SubjectPublicKeyBytes, certKey))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "subject public keys don't match");
					}
					return false;
				}
			}

			bool result = MatchBasicConstraints(xcert) && MatchKeyUsage(xcert) && MatchExtendedKeyUsage(xcert) && MatchSubjectKeyID(xcert) && MatchAuthorityKeyID(xcert) && MatchPrivateKeyValid(xcert) && MatchSubjectPublicKeyAlgID(xcert) && MatchPolicy(xcert) && MatchSubjectAlternativeNames(xcert) && MatchPathToNames(xcert) && MatchNameConstraints(xcert);

			if (result && (Debug != null))
			{
				Debug.println("X509CertSelector.match returning: true");
			}
			return result;
		}

		/* match on subject key identifier extension value */
		private bool MatchSubjectKeyID(X509Certificate xcert)
		{
			if (SubjectKeyID == null)
			{
				return true;
			}
			try
			{
				sbyte[] extVal = xcert.GetExtensionValue("2.5.29.14");
				if (extVal == null)
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "no subject key ID extension");
					}
					return false;
				}
				DerInputStream @in = new DerInputStream(extVal);
				sbyte[] certSubjectKeyID = @in.OctetString;
				if (certSubjectKeyID == null || !System.Array.Equals(SubjectKeyID, certSubjectKeyID))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "subject key IDs don't match");
					}
					return false;
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "exception in subject key ID check");
				}
				return false;
			}
			return true;
		}

		/* match on authority key identifier extension value */
		private bool MatchAuthorityKeyID(X509Certificate xcert)
		{
			if (AuthorityKeyID == null)
			{
				return true;
			}
			try
			{
				sbyte[] extVal = xcert.GetExtensionValue("2.5.29.35");
				if (extVal == null)
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "no authority key ID extension");
					}
					return false;
				}
				DerInputStream @in = new DerInputStream(extVal);
				sbyte[] certAuthKeyID = @in.OctetString;
				if (certAuthKeyID == null || !System.Array.Equals(AuthorityKeyID, certAuthKeyID))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "authority key IDs don't match");
					}
					return false;
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "exception in authority key ID check");
				}
				return false;
			}
			return true;
		}

		/* match on private key usage range */
		private bool MatchPrivateKeyValid(X509Certificate xcert)
		{
			if (PrivateKeyValid_Renamed == null)
			{
				return true;
			}
			PrivateKeyUsageExtension ext = null;
			try
			{
				ext = (PrivateKeyUsageExtension) GetExtensionObject(xcert, PRIVATE_KEY_USAGE_ID);
				if (ext != null)
				{
					ext.valid(PrivateKeyValid_Renamed);
				}
			}
			catch (CertificateExpiredException e1)
			{
				if (Debug != null)
				{
					String time = "n/a";
					try
					{
						Date notAfter = ext.get(PrivateKeyUsageExtension.NOT_AFTER);
						time = notAfter.ToString();
					}
					catch (CertificateException)
					{
						// not able to retrieve notAfter value
					}
					Debug.println("X509CertSelector.match: private key usage not " + "within validity date; ext.NOT_After: " + time + "; X509CertSelector: " + this.ToString());
					Console.WriteLine(e1.ToString());
					Console.Write(e1.StackTrace);
				}
				return false;
			}
			catch (CertificateNotYetValidException e2)
			{
				if (Debug != null)
				{
					String time = "n/a";
					try
					{
						Date notBefore = ext.get(PrivateKeyUsageExtension.NOT_BEFORE);
						time = notBefore.ToString();
					}
					catch (CertificateException)
					{
						// not able to retrieve notBefore value
					}
					Debug.println("X509CertSelector.match: private key usage not " + "within validity date; ext.NOT_BEFORE: " + time + "; X509CertSelector: " + this.ToString());
					Console.WriteLine(e2.ToString());
					Console.Write(e2.StackTrace);
				}
				return false;
			}
			catch (IOException e4)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: IOException in " + "private key usage check; X509CertSelector: " + this.ToString());
					Console.WriteLine(e4.ToString());
					Console.Write(e4.StackTrace);
				}
				return false;
			}
			return true;
		}

		/* match on subject public key algorithm OID */
		private bool MatchSubjectPublicKeyAlgID(X509Certificate xcert)
		{
			if (SubjectPublicKeyAlgID_Renamed == null)
			{
				return true;
			}
			try
			{
				sbyte[] encodedKey = xcert.PublicKey.Encoded;
				DerValue val = new DerValue(encodedKey);
				if (val.tag != DerValue.tag_Sequence)
				{
					throw new IOException("invalid key format");
				}

				AlgorithmId algID = AlgorithmId.parse(val.data.DerValue);
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: subjectPublicKeyAlgID = " + SubjectPublicKeyAlgID_Renamed + ", xcert subjectPublicKeyAlgID = " + algID.OID);
				}
				if (!SubjectPublicKeyAlgID_Renamed.Equals((Object)algID.OID))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "subject public key alg IDs don't match");
					}
					return false;
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: IOException in subject " + "public key algorithm OID check");
				}
				return false;
			}
			return true;
		}

		/* match on key usage extension value */
		private bool MatchKeyUsage(X509Certificate xcert)
		{
			if (KeyUsage_Renamed == null)
			{
				return true;
			}
			bool[] certKeyUsage = xcert.KeyUsage;
			if (certKeyUsage != null)
			{
				for (int keyBit = 0; keyBit < KeyUsage_Renamed.Length; keyBit++)
				{
					if (KeyUsage_Renamed[keyBit] && ((keyBit >= certKeyUsage.Length) || !certKeyUsage[keyBit]))
					{
						if (Debug != null)
						{
							Debug.println("X509CertSelector.match: " + "key usage bits don't match");
						}
						return false;
					}
				}
			}
			return true;
		}

		/* match on extended key usage purpose OIDs */
		private bool MatchExtendedKeyUsage(X509Certificate xcert)
		{
			if ((KeyPurposeSet == null) || KeyPurposeSet.Count == 0)
			{
				return true;
			}
			try
			{
				ExtendedKeyUsageExtension ext = (ExtendedKeyUsageExtension)GetExtensionObject(xcert, EXTENDED_KEY_USAGE_ID);
				if (ext != null)
				{
					Vector<ObjectIdentifier> certKeyPurposeVector = ext.get(ExtendedKeyUsageExtension.USAGES);
					if (!certKeyPurposeVector.Contains(ANY_EXTENDED_KEY_USAGE) && !certKeyPurposeVector.ContainsAll(KeyPurposeOIDSet))
					{
						if (Debug != null)
						{
							Debug.println("X509CertSelector.match: cert failed " + "extendedKeyUsage criterion");
						}
						return false;
					}
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "IOException in extended key usage check");
				}
				return false;
			}
			return true;
		}

		/* match on subject alternative name extension names */
		private bool MatchSubjectAlternativeNames(X509Certificate xcert)
		{
			if ((SubjectAlternativeNames_Renamed == null) || SubjectAlternativeNames_Renamed.Count == 0)
			{
				return true;
			}
			try
			{
				SubjectAlternativeNameExtension sanExt = (SubjectAlternativeNameExtension) GetExtensionObject(xcert, SUBJECT_ALT_NAME_ID);
				if (sanExt == null)
				{
					if (Debug != null)
					{
					  Debug.println("X509CertSelector.match: " + "no subject alternative name extension");
					}
					return false;
				}
				GeneralNames certNames = sanExt.get(SubjectAlternativeNameExtension.SUBJECT_NAME);
				Iterator<GeneralNameInterface> i = SubjectAlternativeGeneralNames.Iterator();
				while (i.HasNext())
				{
					GeneralNameInterface matchName = i.Next();
					bool found = false;
					for (Iterator<GeneralName> t = certNames.GetEnumerator(); t.HasNext() && !found;)
					{
						GeneralNameInterface certName = (t.Next()).Name;
						found = certName.Equals(matchName);
					}
					if (!found && (MatchAllSubjectAltNames_Renamed || !i.HasNext()))
					{
						if (Debug != null)
						{
						  Debug.println("X509CertSelector.match: subject alternative " + "name " + matchName + " not found");
						}
						return false;
					}
					else if (found && !MatchAllSubjectAltNames_Renamed)
					{
						break;
					}
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: IOException in subject " + "alternative name check");
				}
				return false;
			}
			return true;
		}

		/* match on name constraints */
		private bool MatchNameConstraints(X509Certificate xcert)
		{
			if (Nc == null)
			{
				return true;
			}
			try
			{
				if (!Nc.verify(xcert))
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: " + "name constraints not satisfied");
					}
					return false;
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "IOException in name constraints check");
				}
				return false;
			}
			return true;
		}

		/* match on policy OIDs */
		private bool MatchPolicy(X509Certificate xcert)
		{
			if (Policy_Renamed == null)
			{
				return true;
			}
			try
			{
				CertificatePoliciesExtension ext = (CertificatePoliciesExtension) GetExtensionObject(xcert, CERT_POLICIES_ID);
				if (ext == null)
				{
					if (Debug != null)
					{
					  Debug.println("X509CertSelector.match: " + "no certificate policy extension");
					}
					return false;
				}
				List<PolicyInformation> policies = ext.get(CertificatePoliciesExtension.POLICIES);
				/*
				 * Convert the Vector of PolicyInformation to a Vector
				 * of CertificatePolicyIds for easier comparison.
				 */
				List<CertificatePolicyId> policyIDs = new List<CertificatePolicyId>(policies.Count);
				foreach (PolicyInformation info in policies)
				{
					policyIDs.Add(info.PolicyIdentifier);
				}
				if (Policy_Renamed != null)
				{
					bool foundOne = false;
					/*
					 * if the user passes in an empty policy Set, then
					 * we just want to make sure that the candidate certificate
					 * has some policy OID in its CertPoliciesExtension
					 */
					if (Policy_Renamed.CertPolicyIds.Empty)
					{
						if (policyIDs.Count == 0)
						{
							if (Debug != null)
							{
								Debug.println("X509CertSelector.match: " + "cert failed policyAny criterion");
							}
							return false;
						}
					}
					else
					{
						foreach (CertificatePolicyId id in Policy_Renamed.CertPolicyIds)
						{
							if (policyIDs.Contains(id))
							{
								foundOne = true;
								break;
							}
						}
						if (!foundOne)
						{
							if (Debug != null)
							{
								Debug.println("X509CertSelector.match: " + "cert failed policyAny criterion");
							}
							return false;
						}
					}
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "IOException in certificate policy ID check");
				}
				return false;
			}
			return true;
		}

		/* match on pathToNames */
		private bool MatchPathToNames(X509Certificate xcert)
		{
			if (PathToGeneralNames == null)
			{
				return true;
			}
			try
			{
				NameConstraintsExtension ext = (NameConstraintsExtension) GetExtensionObject(xcert, NAME_CONSTRAINTS_ID);
				if (ext == null)
				{
					return true;
				}
				if ((Debug != null) && Debug.isOn("certpath"))
				{
					Debug.println("X509CertSelector.match pathToNames:\n");
					Iterator<GeneralNameInterface> i = PathToGeneralNames.Iterator();
					while (i.HasNext())
					{
						Debug.println("    " + i.Next() + "\n");
					}
				}

				GeneralSubtrees permitted = ext.get(NameConstraintsExtension.PERMITTED_SUBTREES);
				GeneralSubtrees excluded = ext.get(NameConstraintsExtension.EXCLUDED_SUBTREES);
				if (excluded != null)
				{
					if (MatchExcluded(excluded) == false)
					{
						return false;
					}
				}
				if (permitted != null)
				{
					if (MatchPermitted(permitted) == false)
					{
						return false;
					}
				}
			}
			catch (IOException)
			{
				if (Debug != null)
				{
					Debug.println("X509CertSelector.match: " + "IOException in name constraints check");
				}
				return false;
			}
			return true;
		}

		private bool MatchExcluded(GeneralSubtrees excluded)
		{
			/*
			 * Enumerate through excluded and compare each entry
			 * to all pathToNames. If any pathToName is within any of the
			 * subtrees listed in excluded, return false.
			 */
			for (Iterator<GeneralSubtree> t = excluded.GetEnumerator(); t.HasNext();)
			{
				GeneralSubtree tree = t.Next();
				GeneralNameInterface excludedName = tree.Name.Name;
				Iterator<GeneralNameInterface> i = PathToGeneralNames.Iterator();
				while (i.HasNext())
				{
					GeneralNameInterface pathToName = i.Next();
					if (excludedName.Type == pathToName.Type)
					{
						switch (pathToName.constrains(excludedName))
						{
						case GeneralNameInterface.NAME_WIDENS:
						case GeneralNameInterface.NAME_MATCH:
							if (Debug != null)
							{
								Debug.println("X509CertSelector.match: name constraints " + "inhibit path to specified name");
								Debug.println("X509CertSelector.match: excluded name: " + pathToName);
							}
							return false;
						default:
					break;
						}
					}
				}
			}
			return true;
		}

		private bool MatchPermitted(GeneralSubtrees permitted)
		{
			/*
			 * Enumerate through pathToNames, checking that each pathToName
			 * is in at least one of the subtrees listed in permitted.
			 * If not, return false. However, if no subtrees of a given type
			 * are listed, all names of that type are permitted.
			 */
			Iterator<GeneralNameInterface> i = PathToGeneralNames.Iterator();
			while (i.HasNext())
			{
				GeneralNameInterface pathToName = i.Next();
				Iterator<GeneralSubtree> t = permitted.GetEnumerator();
				bool permittedNameFound = false;
				bool nameTypeFound = false;
				String names = "";
				while (t.HasNext() && !permittedNameFound)
				{
					GeneralSubtree tree = t.Next();
					GeneralNameInterface permittedName = tree.Name.Name;
					if (permittedName.Type == pathToName.Type)
					{
						nameTypeFound = true;
						names = names + "  " + permittedName;
						switch (pathToName.constrains(permittedName))
						{
						case GeneralNameInterface.NAME_WIDENS:
						case GeneralNameInterface.NAME_MATCH:
							permittedNameFound = true;
							break;
						default:
					break;
						}
					}
				}
				if (!permittedNameFound && nameTypeFound)
				{
					if (Debug != null)
					{
					  Debug.println("X509CertSelector.match: " + "name constraints inhibit path to specified name; " + "permitted names of type " + pathToName.Type + ": " + names);
					}
					return false;
				}
			}
			return true;
		}

		/* match on basic constraints */
		private bool MatchBasicConstraints(X509Certificate xcert)
		{
			if (BasicConstraints_Renamed == -1)
			{
				return true;
			}
			int maxPathLen = xcert.BasicConstraints;
			if (BasicConstraints_Renamed == -2)
			{
				if (maxPathLen != -1)
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: not an EE cert");
					}
					return false;
				}
			}
			else
			{
				if (maxPathLen < BasicConstraints_Renamed)
				{
					if (Debug != null)
					{
						Debug.println("X509CertSelector.match: cert's maxPathLen " + "is less than the min maxPathLen set by " + "basicConstraints. " + "(" + maxPathLen + " < " + BasicConstraints_Renamed + ")");
					}
					return false;
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> Set<T> cloneSet(Set<T> set)
		private static Set<T> cloneSet<T>(Set<T> set) // Safe casts assuming clone() works correctly
		{
			if (set is HashSet)
			{
				Object clone = ((HashSet<T>)set).Clone();
				return (Set<T>)clone;
			}
			else
			{
				return new HashSet<T>(set);
			}
		}

		/// <summary>
		/// Returns a copy of this object.
		/// </summary>
		/// <returns> the copy </returns>
		public virtual Object Clone()
		{
			try
			{
				X509CertSelector copy = (X509CertSelector)base.Clone();
				// Must clone these because addPathToName et al. modify them
				if (SubjectAlternativeNames_Renamed != null)
				{
					copy.SubjectAlternativeNames_Renamed = CloneSet(SubjectAlternativeNames_Renamed);
					copy.SubjectAlternativeGeneralNames = CloneSet(SubjectAlternativeGeneralNames);
				}
				if (PathToGeneralNames != null)
				{
					copy.PathToNames_Renamed = CloneSet(PathToNames_Renamed);
					copy.PathToGeneralNames = CloneSet(PathToGeneralNames);
				}
				return copy;
			}
			catch (CloneNotSupportedException e)
			{
				/* Cannot happen */
				throw new InternalError(e.ToString(), e);
			}
		}
	}

}