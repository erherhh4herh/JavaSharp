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

namespace java.security
{

	/// <summary>
	/// The {@code AlgorithmParameterGenerator} class is used to generate a
	/// set of
	/// parameters to be used with a certain algorithm. Parameter generators
	/// are constructed using the {@code getInstance} factory methods
	/// (static methods that return instances of a given class).
	/// 
	/// <P>The object that will generate the parameters can be initialized
	/// in two different ways: in an algorithm-independent manner, or in an
	/// algorithm-specific manner:
	/// 
	/// <ul>
	/// <li>The algorithm-independent approach uses the fact that all parameter
	/// generators share the concept of a "size" and a
	/// source of randomness. The measure of size is universally shared
	/// by all algorithm parameters, though it is interpreted differently
	/// for different algorithms. For example, in the case of parameters for
	/// the <i>DSA</i> algorithm, "size" corresponds to the size
	/// of the prime modulus (in bits).
	/// When using this approach, algorithm-specific parameter generation
	/// values - if any - default to some standard values, unless they can be
	/// derived from the specified size.
	/// 
	/// <li>The other approach initializes a parameter generator object
	/// using algorithm-specific semantics, which are represented by a set of
	/// algorithm-specific parameter generation values. To generate
	/// Diffie-Hellman system parameters, for example, the parameter generation
	/// values usually
	/// consist of the size of the prime modulus and the size of the
	/// random exponent, both specified in number of bits.
	/// </ul>
	/// 
	/// <P>In case the client does not explicitly initialize the
	/// AlgorithmParameterGenerator
	/// (via a call to an {@code init} method), each provider must supply (and
	/// document) a default initialization. For example, the Sun provider uses a
	/// default modulus prime size of 1024 bits for the generation of DSA
	/// parameters.
	/// 
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code AlgorithmParameterGenerator} algorithms and
	/// keysizes in parentheses:
	/// <ul>
	/// <li>{@code DiffieHellman} (1024)</li>
	/// <li>{@code DSA} (1024)</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
	/// AlgorithmParameterGenerator section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Jan Luehe
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AlgorithmParameters </seealso>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec
	/// 
	/// @since 1.2 </seealso>

	public class AlgorithmParameterGenerator
	{

		// The provider
		private Provider Provider_Renamed;

		// The provider implementation (delegate)
		private AlgorithmParameterGeneratorSpi ParamGenSpi;

		// The algorithm
		private String Algorithm_Renamed;

		/// <summary>
		/// Creates an AlgorithmParameterGenerator object.
		/// </summary>
		/// <param name="paramGenSpi"> the delegate </param>
		/// <param name="provider"> the provider </param>
		/// <param name="algorithm"> the algorithm </param>
		protected internal AlgorithmParameterGenerator(AlgorithmParameterGeneratorSpi paramGenSpi, Provider provider, String algorithm)
		{
			this.ParamGenSpi = paramGenSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns the standard name of the algorithm this parameter
		/// generator is associated with.
		/// </summary>
		/// <returns> the string name of the algorithm. </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Returns an AlgorithmParameterGenerator object for generating
		/// a set of parameters to be used with the specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new AlgorithmParameterGenerator object encapsulating the
		/// AlgorithmParameterGeneratorSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm this
		/// parameter generator is associated with.
		/// See the AlgorithmParameterGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> the new AlgorithmParameterGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports an
		///          AlgorithmParameterGeneratorSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AlgorithmParameterGenerator getInstance(String algorithm) throws NoSuchAlgorithmException
		public static AlgorithmParameterGenerator GetInstance(String algorithm)
		{
				try
				{
					Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameterGenerator", (String)null);
					return new AlgorithmParameterGenerator((AlgorithmParameterGeneratorSpi)objs[0], (Provider)objs[1], algorithm);
				}
				catch (NoSuchProviderException)
				{
					throw new NoSuchAlgorithmException(algorithm + " not found");
				}
		}

		/// <summary>
		/// Returns an AlgorithmParameterGenerator object for generating
		/// a set of parameters to be used with the specified algorithm.
		/// 
		/// <para> A new AlgorithmParameterGenerator object encapsulating the
		/// AlgorithmParameterGeneratorSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm this
		/// parameter generator is associated with.
		/// See the AlgorithmParameterGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the string name of the Provider.
		/// </param>
		/// <returns> the new AlgorithmParameterGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
		///          implementation for the specified algorithm is not
		///          available from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider name is null
		///          or empty.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AlgorithmParameterGenerator getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static AlgorithmParameterGenerator GetInstance(String algorithm, String provider)
		{
			if (provider == null || provider.Length() == 0)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameterGenerator", provider);
			return new AlgorithmParameterGenerator((AlgorithmParameterGeneratorSpi)objs[0], (Provider)objs[1], algorithm);
		}

		/// <summary>
		/// Returns an AlgorithmParameterGenerator object for generating
		/// a set of parameters to be used with the specified algorithm.
		/// 
		/// <para> A new AlgorithmParameterGenerator object encapsulating the
		/// AlgorithmParameterGeneratorSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the string name of the algorithm this
		/// parameter generator is associated with.
		/// See the AlgorithmParameterGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the Provider object.
		/// </param>
		/// <returns> the new AlgorithmParameterGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AlgorithmParameterGenerator getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static AlgorithmParameterGenerator GetInstance(String algorithm, Provider provider)
		{
			if (provider == null)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameterGenerator", provider);
			return new AlgorithmParameterGenerator((AlgorithmParameterGeneratorSpi)objs[0], (Provider)objs[1], algorithm);
		}

		/// <summary>
		/// Returns the provider of this algorithm parameter generator object.
		/// </summary>
		/// <returns> the provider of this algorithm parameter generator object </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Initializes this parameter generator for a certain size.
		/// To create the parameters, the {@code SecureRandom}
		/// implementation of the highest-priority installed provider is used as
		/// the source of randomness.
		/// (If none of the installed providers supply an implementation of
		/// {@code SecureRandom}, a system-provided source of randomness is
		/// used.)
		/// </summary>
		/// <param name="size"> the size (number of bits). </param>
		public void Init(int size)
		{
			ParamGenSpi.EngineInit(size, new SecureRandom());
		}

		/// <summary>
		/// Initializes this parameter generator for a certain size and source
		/// of randomness.
		/// </summary>
		/// <param name="size"> the size (number of bits). </param>
		/// <param name="random"> the source of randomness. </param>
		public void Init(int size, SecureRandom random)
		{
			ParamGenSpi.EngineInit(size, random);
		}

		/// <summary>
		/// Initializes this parameter generator with a set of algorithm-specific
		/// parameter generation values.
		/// To generate the parameters, the {@code SecureRandom}
		/// implementation of the highest-priority installed provider is used as
		/// the source of randomness.
		/// (If none of the installed providers supply an implementation of
		/// {@code SecureRandom}, a system-provided source of randomness is
		/// used.)
		/// </summary>
		/// <param name="genParamSpec"> the set of algorithm-specific parameter generation values.
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameter
		/// generation values are inappropriate for this parameter generator. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void init(java.security.spec.AlgorithmParameterSpec genParamSpec) throws InvalidAlgorithmParameterException
		public void Init(AlgorithmParameterSpec genParamSpec)
		{
				ParamGenSpi.EngineInit(genParamSpec, new SecureRandom());
		}

		/// <summary>
		/// Initializes this parameter generator with a set of algorithm-specific
		/// parameter generation values.
		/// </summary>
		/// <param name="genParamSpec"> the set of algorithm-specific parameter generation values. </param>
		/// <param name="random"> the source of randomness.
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameter
		/// generation values are inappropriate for this parameter generator. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void init(java.security.spec.AlgorithmParameterSpec genParamSpec, SecureRandom random) throws InvalidAlgorithmParameterException
		public void Init(AlgorithmParameterSpec genParamSpec, SecureRandom random)
		{
				ParamGenSpi.EngineInit(genParamSpec, random);
		}

		/// <summary>
		/// Generates the parameters.
		/// </summary>
		/// <returns> the new AlgorithmParameters object. </returns>
		public AlgorithmParameters GenerateParameters()
		{
			return ParamGenSpi.EngineGenerateParameters();
		}
	}

}