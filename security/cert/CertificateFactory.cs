using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using sun.security.jca;
	using Instance = sun.security.jca.GetInstance.Instance;

	/// <summary>
	/// This class defines the functionality of a certificate factory, which is
	/// used to generate certificate, certification path ({@code CertPath})
	/// and certificate revocation list (CRL) objects from their encodings.
	/// 
	/// <para>For encodings consisting of multiple certificates, use
	/// {@code generateCertificates} when you want to
	/// parse a collection of possibly unrelated certificates. Otherwise,
	/// use {@code generateCertPath} when you want to generate
	/// a {@code CertPath} (a certificate chain) and subsequently
	/// validate it with a {@code CertPathValidator}.
	/// 
	/// </para>
	/// <para>A certificate factory for X.509 must return certificates that are an
	/// instance of {@code java.security.cert.X509Certificate}, and CRLs
	/// that are an instance of {@code java.security.cert.X509CRL}.
	/// 
	/// </para>
	/// <para>The following example reads a file with Base64 encoded certificates,
	/// which are each bounded at the beginning by -----BEGIN CERTIFICATE-----, and
	/// bounded at the end by -----END CERTIFICATE-----. We convert the
	/// {@code FileInputStream} (which does not support {@code mark}
	/// and {@code reset}) to a {@code BufferedInputStream} (which
	/// supports those methods), so that each call to
	/// {@code generateCertificate} consumes only one certificate, and the
	/// read position of the input stream is positioned to the next certificate in
	/// the file:
	/// 
	/// <pre>{@code
	/// FileInputStream fis = new FileInputStream(filename);
	/// BufferedInputStream bis = new BufferedInputStream(fis);
	/// 
	/// CertificateFactory cf = CertificateFactory.getInstance("X.509");
	/// 
	/// while (bis.available() > 0) {
	///    Certificate cert = cf.generateCertificate(bis);
	///    System.out.println(cert.toString());
	/// }
	/// }</pre>
	/// 
	/// </para>
	/// <para>The following example parses a PKCS#7-formatted certificate reply stored
	/// in a file and extracts all the certificates from it:
	/// 
	/// <pre>
	/// FileInputStream fis = new FileInputStream(filename);
	/// CertificateFactory cf = CertificateFactory.getInstance("X.509");
	/// Collection c = cf.generateCertificates(fis);
	/// Iterator i = c.iterator();
	/// while (i.hasNext()) {
	///    Certificate cert = (Certificate)i.next();
	///    System.out.println(cert);
	/// }
	/// </pre>
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code CertificateFactory} type:
	/// <ul>
	/// <li>{@code X.509}</li>
	/// </ul>
	/// and the following standard {@code CertPath} encodings:
	/// <ul>
	/// <li>{@code PKCS7}</li>
	/// <li>{@code PkiPath}</li>
	/// </ul>
	/// The type and encodings are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
	/// CertificateFactory section</a> and the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
	/// CertPath Encodings section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other types or encodings are supported.
	/// 
	/// @author Hemma Prafullchandra
	/// @author Jan Luehe
	/// @author Sean Mullan
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Certificate </seealso>
	/// <seealso cref= X509Certificate </seealso>
	/// <seealso cref= CertPath </seealso>
	/// <seealso cref= CRL </seealso>
	/// <seealso cref= X509CRL
	/// 
	/// @since 1.2 </seealso>

	public class CertificateFactory
	{

		// The certificate type
		private String Type_Renamed;

		// The provider
		private Provider Provider_Renamed;

		// The provider implementation
		private CertificateFactorySpi CertFacSpi;

		/// <summary>
		/// Creates a CertificateFactory object of the given type, and encapsulates
		/// the given provider implementation (SPI object) in it.
		/// </summary>
		/// <param name="certFacSpi"> the provider implementation. </param>
		/// <param name="provider"> the provider. </param>
		/// <param name="type"> the certificate type. </param>
		protected internal CertificateFactory(CertificateFactorySpi certFacSpi, Provider provider, String type)
		{
			this.CertFacSpi = certFacSpi;
			this.Provider_Renamed = provider;
			this.Type_Renamed = type;
		}

		/// <summary>
		/// Returns a certificate factory object that implements the
		/// specified certificate type.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new CertificateFactory object encapsulating the
		/// CertificateFactorySpi implementation from the first
		/// Provider that supports the specified type is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the name of the requested certificate type.
		/// See the CertificateFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard certificate types.
		/// </param>
		/// <returns> a certificate factory object for the specified type.
		/// </returns>
		/// <exception cref="CertificateException"> if no Provider supports a
		///          CertificateFactorySpi implementation for the
		///          specified type.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final CertificateFactory getInstance(String type) throws CertificateException
		public static CertificateFactory GetInstance(String type)
		{
			try
			{
				Instance instance = GetInstance.getInstance("CertificateFactory", typeof(CertificateFactorySpi), type);
				return new CertificateFactory((CertificateFactorySpi)instance.impl, instance.provider, type);
			}
			catch (NoSuchAlgorithmException e)
			{
				throw new CertificateException(type + " not found", e);
			}
		}

		/// <summary>
		/// Returns a certificate factory object for the specified
		/// certificate type.
		/// 
		/// <para> A new CertificateFactory object encapsulating the
		/// CertificateFactorySpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the certificate type.
		/// See the CertificateFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard certificate types.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> a certificate factory object for the specified type.
		/// </returns>
		/// <exception cref="CertificateException"> if a CertificateFactorySpi
		///          implementation for the specified algorithm is not
		///          available from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider name is null
		///          or empty.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final CertificateFactory getInstance(String type, String provider) throws CertificateException, java.security.NoSuchProviderException
		public static CertificateFactory GetInstance(String type, String provider)
		{
			try
			{
				Instance instance = GetInstance.getInstance("CertificateFactory", typeof(CertificateFactorySpi), type, provider);
				return new CertificateFactory((CertificateFactorySpi)instance.impl, instance.provider, type);
			}
			catch (NoSuchAlgorithmException e)
			{
				throw new CertificateException(type + " not found", e);
			}
		}

		/// <summary>
		/// Returns a certificate factory object for the specified
		/// certificate type.
		/// 
		/// <para> A new CertificateFactory object encapsulating the
		/// CertificateFactorySpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the certificate type.
		/// See the CertificateFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard certificate types. </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> a certificate factory object for the specified type.
		/// </returns>
		/// <exception cref="CertificateException"> if a CertificateFactorySpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code provider} is
		///          null.
		/// </exception>
		/// <seealso cref= java.security.Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final CertificateFactory getInstance(String type, java.security.Provider provider) throws CertificateException
		public static CertificateFactory GetInstance(String type, Provider provider)
		{
			try
			{
				Instance instance = GetInstance.getInstance("CertificateFactory", typeof(CertificateFactorySpi), type, provider);
				return new CertificateFactory((CertificateFactorySpi)instance.impl, instance.provider, type);
			}
			catch (NoSuchAlgorithmException e)
			{
				throw new CertificateException(type + " not found", e);
			}
		}

		/// <summary>
		/// Returns the provider of this certificate factory.
		/// </summary>
		/// <returns> the provider of this certificate factory. </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the certificate type associated with this
		/// certificate factory.
		/// </summary>
		/// <returns> the name of the certificate type associated with this
		/// certificate factory. </returns>
		public String Type
		{
			get
			{
				return this.Type_Renamed;
			}
		}

		/// <summary>
		/// Generates a certificate object and initializes it with
		/// the data read from the input stream {@code inStream}.
		/// 
		/// <para>In order to take advantage of the specialized certificate format
		/// supported by this certificate factory,
		/// the returned certificate object can be typecast to the corresponding
		/// certificate class. For example, if this certificate
		/// factory implements X.509 certificates, the returned certificate object
		/// can be typecast to the {@code X509Certificate} class.
		/// 
		/// </para>
		/// <para>In the case of a certificate factory for X.509 certificates, the
		/// certificate provided in {@code inStream} must be DER-encoded and
		/// may be supplied in binary or printable (Base64) encoding. If the
		/// certificate is provided in Base64 encoding, it must be bounded at
		/// the beginning by -----BEGIN CERTIFICATE-----, and must be bounded at
		/// the end by -----END CERTIFICATE-----.
		/// 
		/// </para>
		/// <para>Note that if the given input stream does not support
		/// <seealso cref="java.io.InputStream#mark(int) mark"/> and
		/// <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		/// consume the entire input stream. Otherwise, each call to this
		/// method consumes one certificate and the read position of the
		/// input stream is positioned to the next available byte after
		/// the inherent end-of-certificate marker. If the data in the input stream
		/// does not contain an inherent end-of-certificate marker (other
		/// than EOF) and there is trailing data after the certificate is parsed, a
		/// {@code CertificateException} is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inStream"> an input stream with the certificate data.
		/// </param>
		/// <returns> a certificate object initialized with the data
		/// from the input stream.
		/// </returns>
		/// <exception cref="CertificateException"> on parsing errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Certificate generateCertificate(java.io.InputStream inStream) throws CertificateException
		public Certificate GenerateCertificate(InputStream inStream)
		{
			return CertFacSpi.EngineGenerateCertificate(inStream);
		}

		/// <summary>
		/// Returns an iteration of the {@code CertPath} encodings supported
		/// by this certificate factory, with the default encoding first. See
		/// the CertPath Encodings section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard encoding names and their formats.
		/// <para>
		/// Attempts to modify the returned {@code Iterator} via its
		/// {@code remove} method result in an
		/// {@code UnsupportedOperationException}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an {@code Iterator} over the names of the supported
		///         {@code CertPath} encodings (as {@code String}s)
		/// @since 1.4 </returns>
		public IEnumerator<String> CertPathEncodings
		{
			get
			{
				return (CertFacSpi.EngineGetCertPathEncodings());
			}
		}

		/// <summary>
		/// Generates a {@code CertPath} object and initializes it with
		/// the data read from the {@code InputStream} inStream. The data
		/// is assumed to be in the default encoding. The name of the default
		/// encoding is the first element of the {@code Iterator} returned by
		/// the <seealso cref="#getCertPathEncodings getCertPathEncodings"/> method.
		/// </summary>
		/// <param name="inStream"> an {@code InputStream} containing the data </param>
		/// <returns> a {@code CertPath} initialized with the data from the
		///   {@code InputStream} </returns>
		/// <exception cref="CertificateException"> if an exception occurs while decoding
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CertPath generateCertPath(java.io.InputStream inStream) throws CertificateException
		public CertPath GenerateCertPath(InputStream inStream)
		{
			return (CertFacSpi.EngineGenerateCertPath(inStream));
		}

		/// <summary>
		/// Generates a {@code CertPath} object and initializes it with
		/// the data read from the {@code InputStream} inStream. The data
		/// is assumed to be in the specified encoding. See
		/// the CertPath Encodings section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard encoding names and their formats.
		/// </summary>
		/// <param name="inStream"> an {@code InputStream} containing the data </param>
		/// <param name="encoding"> the encoding used for the data </param>
		/// <returns> a {@code CertPath} initialized with the data from the
		///   {@code InputStream} </returns>
		/// <exception cref="CertificateException"> if an exception occurs while decoding or
		///   the encoding requested is not supported
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CertPath generateCertPath(java.io.InputStream inStream, String encoding) throws CertificateException
		public CertPath GenerateCertPath(InputStream inStream, String encoding)
		{
			return (CertFacSpi.EngineGenerateCertPath(inStream, encoding));
		}

		/// <summary>
		/// Generates a {@code CertPath} object and initializes it with
		/// a {@code List} of {@code Certificate}s.
		/// <para>
		/// The certificates supplied must be of a type supported by the
		/// {@code CertificateFactory}. They will be copied out of the supplied
		/// {@code List} object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certificates"> a {@code List} of {@code Certificate}s </param>
		/// <returns> a {@code CertPath} initialized with the supplied list of
		///   certificates </returns>
		/// <exception cref="CertificateException"> if an exception occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CertPath generateCertPath(java.util.List<? extends Certificate> certificates) throws CertificateException
		public CertPath generateCertPath<T1>(IList<T1> certificates) where T1 : Certificate
		{
			return (CertFacSpi.EngineGenerateCertPath(certificates));
		}

		/// <summary>
		/// Returns a (possibly empty) collection view of the certificates read
		/// from the given input stream {@code inStream}.
		/// 
		/// <para>In order to take advantage of the specialized certificate format
		/// supported by this certificate factory, each element in
		/// the returned collection view can be typecast to the corresponding
		/// certificate class. For example, if this certificate
		/// factory implements X.509 certificates, the elements in the returned
		/// collection can be typecast to the {@code X509Certificate} class.
		/// 
		/// </para>
		/// <para>In the case of a certificate factory for X.509 certificates,
		/// {@code inStream} may contain a sequence of DER-encoded certificates
		/// in the formats described for
		/// <seealso cref="#generateCertificate(java.io.InputStream) generateCertificate"/>.
		/// In addition, {@code inStream} may contain a PKCS#7 certificate
		/// chain. This is a PKCS#7 <i>SignedData</i> object, with the only
		/// significant field being <i>certificates</i>. In particular, the
		/// signature and the contents are ignored. This format allows multiple
		/// certificates to be downloaded at once. If no certificates are present,
		/// an empty collection is returned.
		/// 
		/// </para>
		/// <para>Note that if the given input stream does not support
		/// <seealso cref="java.io.InputStream#mark(int) mark"/> and
		/// <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		/// consume the entire input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inStream"> the input stream with the certificates.
		/// </param>
		/// <returns> a (possibly empty) collection view of
		/// java.security.cert.Certificate objects
		/// initialized with the data from the input stream.
		/// </returns>
		/// <exception cref="CertificateException"> on parsing errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.util.Collection<? extends Certificate> generateCertificates(java.io.InputStream inStream) throws CertificateException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.util.Collection<? extends Certificate> generateCertificates(java.io.InputStream inStream) throws CertificateException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public ICollection<?> GenerateCertificates(InputStream inStream) where ? : Certificate
		{
			return CertFacSpi.EngineGenerateCertificates(inStream);
		}

		/// <summary>
		/// Generates a certificate revocation list (CRL) object and initializes it
		/// with the data read from the input stream {@code inStream}.
		/// 
		/// <para>In order to take advantage of the specialized CRL format
		/// supported by this certificate factory,
		/// the returned CRL object can be typecast to the corresponding
		/// CRL class. For example, if this certificate
		/// factory implements X.509 CRLs, the returned CRL object
		/// can be typecast to the {@code X509CRL} class.
		/// 
		/// </para>
		/// <para>Note that if the given input stream does not support
		/// <seealso cref="java.io.InputStream#mark(int) mark"/> and
		/// <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		/// consume the entire input stream. Otherwise, each call to this
		/// method consumes one CRL and the read position of the input stream
		/// is positioned to the next available byte after the inherent
		/// end-of-CRL marker. If the data in the
		/// input stream does not contain an inherent end-of-CRL marker (other
		/// than EOF) and there is trailing data after the CRL is parsed, a
		/// {@code CRLException} is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inStream"> an input stream with the CRL data.
		/// </param>
		/// <returns> a CRL object initialized with the data
		/// from the input stream.
		/// </returns>
		/// <exception cref="CRLException"> on parsing errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CRL generateCRL(java.io.InputStream inStream) throws CRLException
		public CRL GenerateCRL(InputStream inStream)
		{
			return CertFacSpi.EngineGenerateCRL(inStream);
		}

		/// <summary>
		/// Returns a (possibly empty) collection view of the CRLs read
		/// from the given input stream {@code inStream}.
		/// 
		/// <para>In order to take advantage of the specialized CRL format
		/// supported by this certificate factory, each element in
		/// the returned collection view can be typecast to the corresponding
		/// CRL class. For example, if this certificate
		/// factory implements X.509 CRLs, the elements in the returned
		/// collection can be typecast to the {@code X509CRL} class.
		/// 
		/// </para>
		/// <para>In the case of a certificate factory for X.509 CRLs,
		/// {@code inStream} may contain a sequence of DER-encoded CRLs.
		/// In addition, {@code inStream} may contain a PKCS#7 CRL
		/// set. This is a PKCS#7 <i>SignedData</i> object, with the only
		/// significant field being <i>crls</i>. In particular, the
		/// signature and the contents are ignored. This format allows multiple
		/// CRLs to be downloaded at once. If no CRLs are present,
		/// an empty collection is returned.
		/// 
		/// </para>
		/// <para>Note that if the given input stream does not support
		/// <seealso cref="java.io.InputStream#mark(int) mark"/> and
		/// <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		/// consume the entire input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="inStream"> the input stream with the CRLs.
		/// </param>
		/// <returns> a (possibly empty) collection view of
		/// java.security.cert.CRL objects initialized with the data from the input
		/// stream.
		/// </returns>
		/// <exception cref="CRLException"> on parsing errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.util.Collection<? extends CRL> generateCRLs(java.io.InputStream inStream) throws CRLException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.util.Collection<? extends CRL> generateCRLs(java.io.InputStream inStream) throws CRLException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public ICollection<?> GenerateCRLs(InputStream inStream) where ? : CRL
		{
			return CertFacSpi.EngineGenerateCRLs(inStream);
		}
	}

}