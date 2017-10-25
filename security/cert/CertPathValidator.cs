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

	using Debug = sun.security.util.Debug;

	using sun.security.jca;
	using Instance = sun.security.jca.GetInstance.Instance;

	/// <summary>
	/// A class for validating certification paths (also known as certificate
	/// chains).
	/// <para>
	/// This class uses a provider-based architecture.
	/// To create a {@code CertPathValidator},
	/// call one of the static {@code getInstance} methods, passing in the
	/// algorithm name of the {@code CertPathValidator} desired and
	/// optionally the name of the provider desired.
	/// 
	/// </para>
	/// <para>Once a {@code CertPathValidator} object has been created, it can
	/// be used to validate certification paths by calling the {@link #validate
	/// validate} method and passing it the {@code CertPath} to be validated
	/// and an algorithm-specific set of parameters. If successful, the result is
	/// returned in an object that implements the
	/// {@code CertPathValidatorResult} interface.
	/// 
	/// </para>
	/// <para>The <seealso cref="#getRevocationChecker"/> method allows an application to specify
	/// additional algorithm-specific parameters and options used by the
	/// {@code CertPathValidator} when checking the revocation status of
	/// certificates. Here is an example demonstrating how it is used with the PKIX
	/// algorithm:
	/// 
	/// <pre>
	/// CertPathValidator cpv = CertPathValidator.getInstance("PKIX");
	/// PKIXRevocationChecker rc = (PKIXRevocationChecker)cpv.getRevocationChecker();
	/// rc.setOptions(EnumSet.of(Option.SOFT_FAIL));
	/// params.addCertPathChecker(rc);
	/// CertPathValidatorResult cpvr = cpv.validate(path, params);
	/// </pre>
	/// 
	/// </para>
	/// <para>Every implementation of the Java platform is required to support the
	/// following standard {@code CertPathValidator} algorithm:
	/// <ul>
	/// <li>{@code PKIX}</li>
	/// </ul>
	/// This algorithm is described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
	/// CertPathValidator section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// The static methods of this class are guaranteed to be thread-safe.
	/// Multiple threads may concurrently invoke the static methods defined in
	/// this class with no ill effects.
	/// </para>
	/// <para>
	/// However, this is not true for the non-static methods defined by this class.
	/// Unless otherwise documented by a specific provider, threads that need to
	/// access a single {@code CertPathValidator} instance concurrently should
	/// synchronize amongst themselves and provide the necessary locking. Multiple
	/// threads each manipulating a different {@code CertPathValidator}
	/// instance need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPath
	/// 
	/// @since       1.4
	/// @author      Yassir Elley </seealso>
	public class CertPathValidator
	{

		/*
		 * Constant to lookup in the Security properties file to determine
		 * the default certpathvalidator type. In the Security properties file,
		 * the default certpathvalidator type is given as:
		 * <pre>
		 * certpathvalidator.type=PKIX
		 * </pre>
		 */
		private const String CPV_TYPE = "certpathvalidator.type";
		private readonly CertPathValidatorSpi ValidatorSpi;
		private readonly Provider Provider_Renamed;
		private readonly String Algorithm_Renamed;

		/// <summary>
		/// Creates a {@code CertPathValidator} object of the given algorithm,
		/// and encapsulates the given provider implementation (SPI object) in it.
		/// </summary>
		/// <param name="validatorSpi"> the provider implementation </param>
		/// <param name="provider"> the provider </param>
		/// <param name="algorithm"> the algorithm name </param>
		protected internal CertPathValidator(CertPathValidatorSpi validatorSpi, Provider provider, String algorithm)
		{
			this.ValidatorSpi = validatorSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns a {@code CertPathValidator} object that implements the
		/// specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new CertPathValidator object encapsulating the
		/// CertPathValidatorSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathValidator}
		///  algorithm. See the CertPathValidator section in the <a href=
		///  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> a {@code CertPathValidator} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          CertPathValidatorSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CertPathValidator getInstance(String algorithm) throws java.security.NoSuchAlgorithmException
		public static CertPathValidator GetInstance(String algorithm)
		{
			Instance instance = GetInstance.getInstance("CertPathValidator", typeof(CertPathValidatorSpi), algorithm);
			return new CertPathValidator((CertPathValidatorSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a {@code CertPathValidator} object that implements the
		/// specified algorithm.
		/// 
		/// <para> A new CertPathValidator object encapsulating the
		/// CertPathValidatorSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathValidator}
		///  algorithm. See the CertPathValidator section in the <a href=
		///  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> a {@code CertPathValidator} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a CertPathValidatorSpi
		///          implementation for the specified algorithm is not
		///          available from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code provider} is
		///          null or empty.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CertPathValidator getInstance(String algorithm, String provider) throws java.security.NoSuchAlgorithmException, java.security.NoSuchProviderException
		public static CertPathValidator GetInstance(String algorithm, String provider)
		{
			Instance instance = GetInstance.getInstance("CertPathValidator", typeof(CertPathValidatorSpi), algorithm, provider);
			return new CertPathValidator((CertPathValidatorSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a {@code CertPathValidator} object that implements the
		/// specified algorithm.
		/// 
		/// <para> A new CertPathValidator object encapsulating the
		/// CertPathValidatorSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathValidator}
		/// algorithm. See the CertPathValidator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> a {@code CertPathValidator} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a CertPathValidatorSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code provider} is
		///          null.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CertPathValidator getInstance(String algorithm, java.security.Provider provider) throws java.security.NoSuchAlgorithmException
		public static CertPathValidator GetInstance(String algorithm, Provider provider)
		{
			Instance instance = GetInstance.getInstance("CertPathValidator", typeof(CertPathValidatorSpi), algorithm, provider);
			return new CertPathValidator((CertPathValidatorSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns the {@code Provider} of this
		/// {@code CertPathValidator}.
		/// </summary>
		/// <returns> the {@code Provider} of this {@code CertPathValidator} </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Returns the algorithm name of this {@code CertPathValidator}.
		/// </summary>
		/// <returns> the algorithm name of this {@code CertPathValidator} </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Validates the specified certification path using the specified
		/// algorithm parameter set.
		/// <para>
		/// The {@code CertPath} specified must be of a type that is
		/// supported by the validation algorithm, otherwise an
		/// {@code InvalidAlgorithmParameterException} will be thrown. For
		/// example, a {@code CertPathValidator} that implements the PKIX
		/// algorithm validates {@code CertPath} objects of type X.509.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certPath"> the {@code CertPath} to be validated </param>
		/// <param name="params"> the algorithm parameters </param>
		/// <returns> the result of the validation algorithm </returns>
		/// <exception cref="CertPathValidatorException"> if the {@code CertPath}
		/// does not validate </exception>
		/// <exception cref="InvalidAlgorithmParameterException"> if the specified
		/// parameters or the type of the specified {@code CertPath} are
		/// inappropriate for this {@code CertPathValidator} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CertPathValidatorResult validate(CertPath certPath, CertPathParameters params) throws CertPathValidatorException, java.security.InvalidAlgorithmParameterException
		public CertPathValidatorResult Validate(CertPath certPath, CertPathParameters @params)
		{
			return ValidatorSpi.EngineValidate(certPath, @params);
		}

		/// <summary>
		/// Returns the default {@code CertPathValidator} type as specified by
		/// the {@code certpathvalidator.type} security property, or the string
		/// {@literal "PKIX"} if no such property exists.
		/// 
		/// <para>The default {@code CertPathValidator} type can be used by
		/// applications that do not want to use a hard-coded type when calling one
		/// of the {@code getInstance} methods, and want to provide a default
		/// type in case a user does not specify its own.
		/// 
		/// </para>
		/// <para>The default {@code CertPathValidator} type can be changed by
		/// setting the value of the {@code certpathvalidator.type} security
		/// property to the desired type.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.security.Security security properties </seealso>
		/// <returns> the default {@code CertPathValidator} type as specified
		/// by the {@code certpathvalidator.type} security property, or the string
		/// {@literal "PKIX"} if no such property exists. </returns>
		public static String DefaultType
		{
			get
			{
				String cpvtype = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
				return (cpvtype == null) ? "PKIX" : cpvtype;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty(CPV_TYPE);
			}
		}

		/// <summary>
		/// Returns a {@code CertPathChecker} that the encapsulated
		/// {@code CertPathValidatorSpi} implementation uses to check the revocation
		/// status of certificates. A PKIX implementation returns objects of
		/// type {@code PKIXRevocationChecker}. Each invocation of this method
		/// returns a new instance of {@code CertPathChecker}.
		/// 
		/// <para>The primary purpose of this method is to allow callers to specify
		/// additional input parameters and options specific to revocation checking.
		/// See the class description for an example.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code CertPathChecker} </returns>
		/// <exception cref="UnsupportedOperationException"> if the service provider does not
		///         support this method
		/// @since 1.8 </exception>
		public CertPathChecker RevocationChecker
		{
			get
			{
				return ValidatorSpi.EngineGetRevocationChecker();
			}
		}
	}

}