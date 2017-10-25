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
	/// A class for building certification paths (also known as certificate chains).
	/// <para>
	/// This class uses a provider-based architecture.
	/// To create a {@code CertPathBuilder}, call
	/// one of the static {@code getInstance} methods, passing in the
	/// algorithm name of the {@code CertPathBuilder} desired and optionally
	/// the name of the provider desired.
	/// 
	/// </para>
	/// <para>Once a {@code CertPathBuilder} object has been created, certification
	/// paths can be constructed by calling the <seealso cref="#build build"/> method and
	/// passing it an algorithm-specific set of parameters. If successful, the
	/// result (including the {@code CertPath} that was built) is returned
	/// in an object that implements the {@code CertPathBuilderResult}
	/// interface.
	/// 
	/// </para>
	/// <para>The <seealso cref="#getRevocationChecker"/> method allows an application to specify
	/// additional algorithm-specific parameters and options used by the
	/// {@code CertPathBuilder} when checking the revocation status of certificates.
	/// Here is an example demonstrating how it is used with the PKIX algorithm:
	/// 
	/// <pre>
	/// CertPathBuilder cpb = CertPathBuilder.getInstance("PKIX");
	/// PKIXRevocationChecker rc = (PKIXRevocationChecker)cpb.getRevocationChecker();
	/// rc.setOptions(EnumSet.of(Option.PREFER_CRLS));
	/// params.addCertPathChecker(rc);
	/// CertPathBuilderResult cpbr = cpb.build(params);
	/// </pre>
	/// 
	/// </para>
	/// <para>Every implementation of the Java platform is required to support the
	/// following standard {@code CertPathBuilder} algorithm:
	/// <ul>
	/// <li>{@code PKIX}</li>
	/// </ul>
	/// This algorithm is described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
	/// CertPathBuilder section</a> of the
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
	/// access a single {@code CertPathBuilder} instance concurrently should
	/// synchronize amongst themselves and provide the necessary locking. Multiple
	/// threads each manipulating a different {@code CertPathBuilder} instance
	/// need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPath
	/// 
	/// @since       1.4
	/// @author      Sean Mullan
	/// @author      Yassir Elley </seealso>
	public class CertPathBuilder
	{

		/*
		 * Constant to lookup in the Security properties file to determine
		 * the default certpathbuilder type. In the Security properties file,
		 * the default certpathbuilder type is given as:
		 * <pre>
		 * certpathbuilder.type=PKIX
		 * </pre>
		 */
		private const String CPB_TYPE = "certpathbuilder.type";
		private readonly CertPathBuilderSpi BuilderSpi;
		private readonly Provider Provider_Renamed;
		private readonly String Algorithm_Renamed;

		/// <summary>
		/// Creates a {@code CertPathBuilder} object of the given algorithm,
		/// and encapsulates the given provider implementation (SPI object) in it.
		/// </summary>
		/// <param name="builderSpi"> the provider implementation </param>
		/// <param name="provider"> the provider </param>
		/// <param name="algorithm"> the algorithm name </param>
		protected internal CertPathBuilder(CertPathBuilderSpi builderSpi, Provider provider, String algorithm)
		{
			this.BuilderSpi = builderSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns a {@code CertPathBuilder} object that implements the
		/// specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new CertPathBuilder object encapsulating the
		/// CertPathBuilderSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		///  algorithm.  See the CertPathBuilder section in the <a href=
		///  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> a {@code CertPathBuilder} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          CertPathBuilderSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CertPathBuilder getInstance(String algorithm) throws java.security.NoSuchAlgorithmException
		public static CertPathBuilder GetInstance(String algorithm)
		{
			Instance instance = GetInstance.getInstance("CertPathBuilder", typeof(CertPathBuilderSpi), algorithm);
			return new CertPathBuilder((CertPathBuilderSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a {@code CertPathBuilder} object that implements the
		/// specified algorithm.
		/// 
		/// <para> A new CertPathBuilder object encapsulating the
		/// CertPathBuilderSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		///  algorithm.  See the CertPathBuilder section in the <a href=
		///  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> a {@code CertPathBuilder} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a CertPathBuilderSpi
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
//ORIGINAL LINE: public static CertPathBuilder getInstance(String algorithm, String provider) throws java.security.NoSuchAlgorithmException, java.security.NoSuchProviderException
		public static CertPathBuilder GetInstance(String algorithm, String provider)
		{
			Instance instance = GetInstance.getInstance("CertPathBuilder", typeof(CertPathBuilderSpi), algorithm, provider);
			return new CertPathBuilder((CertPathBuilderSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a {@code CertPathBuilder} object that implements the
		/// specified algorithm.
		/// 
		/// <para> A new CertPathBuilder object encapsulating the
		/// CertPathBuilderSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		///  algorithm.  See the CertPathBuilder section in the <a href=
		///  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> a {@code CertPathBuilder} object that implements the
		///          specified algorithm.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a CertPathBuilderSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code provider} is
		///          null.
		/// </exception>
		/// <seealso cref= java.security.Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CertPathBuilder getInstance(String algorithm, java.security.Provider provider) throws java.security.NoSuchAlgorithmException
		public static CertPathBuilder GetInstance(String algorithm, Provider provider)
		{
			Instance instance = GetInstance.getInstance("CertPathBuilder", typeof(CertPathBuilderSpi), algorithm, provider);
			return new CertPathBuilder((CertPathBuilderSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns the provider of this {@code CertPathBuilder}.
		/// </summary>
		/// <returns> the provider of this {@code CertPathBuilder} </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the algorithm of this {@code CertPathBuilder}.
		/// </summary>
		/// <returns> the name of the algorithm of this {@code CertPathBuilder} </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Attempts to build a certification path using the specified algorithm
		/// parameter set.
		/// </summary>
		/// <param name="params"> the algorithm parameters </param>
		/// <returns> the result of the build algorithm </returns>
		/// <exception cref="CertPathBuilderException"> if the builder is unable to construct
		///  a certification path that satisfies the specified parameters </exception>
		/// <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		/// are inappropriate for this {@code CertPathBuilder} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final CertPathBuilderResult build(CertPathParameters params) throws CertPathBuilderException, java.security.InvalidAlgorithmParameterException
		public CertPathBuilderResult Build(CertPathParameters @params)
		{
			return BuilderSpi.EngineBuild(@params);
		}

		/// <summary>
		/// Returns the default {@code CertPathBuilder} type as specified by
		/// the {@code certpathbuilder.type} security property, or the string
		/// {@literal "PKIX"} if no such property exists.
		/// 
		/// <para>The default {@code CertPathBuilder} type can be used by
		/// applications that do not want to use a hard-coded type when calling one
		/// of the {@code getInstance} methods, and want to provide a default
		/// type in case a user does not specify its own.
		/// 
		/// </para>
		/// <para>The default {@code CertPathBuilder} type can be changed by
		/// setting the value of the {@code certpathbuilder.type} security property
		/// to the desired type.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.security.Security security properties </seealso>
		/// <returns> the default {@code CertPathBuilder} type as specified
		/// by the {@code certpathbuilder.type} security property, or the string
		/// {@literal "PKIX"} if no such property exists. </returns>
		public static String DefaultType
		{
			get
			{
				String cpbtype = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
				return (cpbtype == null) ? "PKIX" : cpbtype;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty(CPB_TYPE);
			}
		}

		/// <summary>
		/// Returns a {@code CertPathChecker} that the encapsulated
		/// {@code CertPathBuilderSpi} implementation uses to check the revocation
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
				return BuilderSpi.EngineGetRevocationChecker();
			}
		}
	}

}