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
	/// This class is used as an opaque representation of cryptographic parameters.
	/// 
	/// <para>An {@code AlgorithmParameters} object for managing the parameters
	/// for a particular algorithm can be obtained by
	/// calling one of the {@code getInstance} factory methods
	/// (static methods that return instances of a given class).
	/// 
	/// </para>
	/// <para>Once an {@code AlgorithmParameters} object is obtained, it must be
	/// initialized via a call to {@code init}, using an appropriate parameter
	/// specification or parameter encoding.
	/// 
	/// </para>
	/// <para>A transparent parameter specification is obtained from an
	/// {@code AlgorithmParameters} object via a call to
	/// {@code getParameterSpec}, and a byte encoding of the parameters is
	/// obtained via a call to {@code getEncoded}.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code AlgorithmParameters} algorithms:
	/// <ul>
	/// <li>{@code AES}</li>
	/// <li>{@code DES}</li>
	/// <li>{@code DESede}</li>
	/// <li>{@code DiffieHellman}</li>
	/// <li>{@code DSA}</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
	/// AlgorithmParameters section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Jan Luehe
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>
	/// <seealso cref= java.security.spec.DSAParameterSpec </seealso>
	/// <seealso cref= KeyPairGenerator
	/// 
	/// @since 1.2 </seealso>

	public class AlgorithmParameters
	{

		// The provider
		private Provider Provider_Renamed;

		// The provider implementation (delegate)
		private AlgorithmParametersSpi ParamSpi;

		// The algorithm
		private String Algorithm_Renamed;

		// Has this object been initialized?
		private bool Initialized = false;

		/// <summary>
		/// Creates an AlgorithmParameters object.
		/// </summary>
		/// <param name="paramSpi"> the delegate </param>
		/// <param name="provider"> the provider </param>
		/// <param name="algorithm"> the algorithm </param>
		protected internal AlgorithmParameters(AlgorithmParametersSpi paramSpi, Provider provider, String algorithm)
		{
			this.ParamSpi = paramSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns the name of the algorithm associated with this parameter object.
		/// </summary>
		/// <returns> the algorithm name. </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Returns a parameter object for the specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new AlgorithmParameters object encapsulating the
		/// AlgorithmParametersSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para> The returned parameter object must be initialized via a call to
		/// {@code init}, using an appropriate parameter specification or
		/// parameter encoding.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the AlgorithmParameters section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> the new parameter object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports an
		///          AlgorithmParametersSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AlgorithmParameters getInstance(String algorithm) throws NoSuchAlgorithmException
		public static AlgorithmParameters GetInstance(String algorithm)
		{
			try
			{
				Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameters", (String)null);
				return new AlgorithmParameters((AlgorithmParametersSpi)objs[0], (Provider)objs[1], algorithm);
			}
			catch (NoSuchProviderException)
			{
				throw new NoSuchAlgorithmException(algorithm + " not found");
			}
		}

		/// <summary>
		/// Returns a parameter object for the specified algorithm.
		/// 
		/// <para> A new AlgorithmParameters object encapsulating the
		/// AlgorithmParametersSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para>The returned parameter object must be initialized via a call to
		/// {@code init}, using an appropriate parameter specification or
		/// parameter encoding.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the AlgorithmParameters section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> the new parameter object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if an AlgorithmParametersSpi
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
//ORIGINAL LINE: public static AlgorithmParameters getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static AlgorithmParameters GetInstance(String algorithm, String provider)
		{
			if (provider == null || provider.Length() == 0)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameters", provider);
			return new AlgorithmParameters((AlgorithmParametersSpi)objs[0], (Provider)objs[1], algorithm);
		}

		/// <summary>
		/// Returns a parameter object for the specified algorithm.
		/// 
		/// <para> A new AlgorithmParameters object encapsulating the
		/// AlgorithmParametersSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// <para>The returned parameter object must be initialized via a call to
		/// {@code init}, using an appropriate parameter specification or
		/// parameter encoding.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the AlgorithmParameters section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> the new parameter object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AlgorithmParameters getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static AlgorithmParameters GetInstance(String algorithm, Provider provider)
		{
			if (provider == null)
			{
				throw new IllegalArgumentException("missing provider");
			}
			Object[] objs = Security.GetImpl(algorithm, "AlgorithmParameters", provider);
			return new AlgorithmParameters((AlgorithmParametersSpi)objs[0], (Provider)objs[1], algorithm);
		}

		/// <summary>
		/// Returns the provider of this parameter object.
		/// </summary>
		/// <returns> the provider of this parameter object </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Initializes this parameter object using the parameters
		/// specified in {@code paramSpec}.
		/// </summary>
		/// <param name="paramSpec"> the parameter specification.
		/// </param>
		/// <exception cref="InvalidParameterSpecException"> if the given parameter
		/// specification is inappropriate for the initialization of this parameter
		/// object, or if this parameter object has already been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void init(java.security.spec.AlgorithmParameterSpec paramSpec) throws java.security.spec.InvalidParameterSpecException
		public void Init(AlgorithmParameterSpec paramSpec)
		{
			if (this.Initialized)
			{
				throw new InvalidParameterSpecException("already initialized");
			}
			ParamSpi.EngineInit(paramSpec);
			this.Initialized = true;
		}

		/// <summary>
		/// Imports the specified parameters and decodes them according to the
		/// primary decoding format for parameters. The primary decoding
		/// format for parameters is ASN.1, if an ASN.1 specification for this type
		/// of parameters exists.
		/// </summary>
		/// <param name="params"> the encoded parameters.
		/// </param>
		/// <exception cref="IOException"> on decoding errors, or if this parameter object
		/// has already been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void init(byte[] params) throws IOException
		public void Init(sbyte[] @params)
		{
			if (this.Initialized)
			{
				throw new IOException("already initialized");
			}
			ParamSpi.EngineInit(@params);
			this.Initialized = true;
		}

		/// <summary>
		/// Imports the parameters from {@code params} and decodes them
		/// according to the specified decoding scheme.
		/// If {@code format} is null, the
		/// primary decoding format for parameters is used. The primary decoding
		/// format is ASN.1, if an ASN.1 specification for these parameters
		/// exists.
		/// </summary>
		/// <param name="params"> the encoded parameters.
		/// </param>
		/// <param name="format"> the name of the decoding scheme.
		/// </param>
		/// <exception cref="IOException"> on decoding errors, or if this parameter object
		/// has already been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void init(byte[] params, String format) throws IOException
		public void Init(sbyte[] @params, String format)
		{
			if (this.Initialized)
			{
				throw new IOException("already initialized");
			}
			ParamSpi.EngineInit(@params, format);
			this.Initialized = true;
		}

		/// <summary>
		/// Returns a (transparent) specification of this parameter object.
		/// {@code paramSpec} identifies the specification class in which
		/// the parameters should be returned. It could, for example, be
		/// {@code DSAParameterSpec.class}, to indicate that the
		/// parameters should be returned in an instance of the
		/// {@code DSAParameterSpec} class.
		/// </summary>
		/// @param <T> the type of the parameter specification to be returrned </param>
		/// <param name="paramSpec"> the specification class in which
		/// the parameters should be returned.
		/// </param>
		/// <returns> the parameter specification.
		/// </returns>
		/// <exception cref="InvalidParameterSpecException"> if the requested parameter
		/// specification is inappropriate for this parameter object, or if this
		/// parameter object has not been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final <T extends java.security.spec.AlgorithmParameterSpec> T getParameterSpec(Class paramSpec) throws java.security.spec.InvalidParameterSpecException
		public T getParameterSpec<T>(Class paramSpec) where T : java.security.spec.AlgorithmParameterSpec
		{
			if (this.Initialized == false)
			{
				throw new InvalidParameterSpecException("not initialized");
			}
			return ParamSpi.EngineGetParameterSpec(paramSpec);
		}

		/// <summary>
		/// Returns the parameters in their primary encoding format.
		/// The primary encoding format for parameters is ASN.1, if an ASN.1
		/// specification for this type of parameters exists.
		/// </summary>
		/// <returns> the parameters encoded using their primary encoding format.
		/// </returns>
		/// <exception cref="IOException"> on encoding errors, or if this parameter object
		/// has not been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final byte[] getEncoded() throws IOException
		public sbyte[] Encoded
		{
			get
			{
				if (this.Initialized == false)
				{
					throw new IOException("not initialized");
				}
				return ParamSpi.EngineGetEncoded();
			}
		}

		/// <summary>
		/// Returns the parameters encoded in the specified scheme.
		/// If {@code format} is null, the
		/// primary encoding format for parameters is used. The primary encoding
		/// format is ASN.1, if an ASN.1 specification for these parameters
		/// exists.
		/// </summary>
		/// <param name="format"> the name of the encoding format.
		/// </param>
		/// <returns> the parameters encoded using the specified encoding scheme.
		/// </returns>
		/// <exception cref="IOException"> on encoding errors, or if this parameter object
		/// has not been initialized. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final byte[] getEncoded(String format) throws IOException
		public sbyte[] GetEncoded(String format)
		{
			if (this.Initialized == false)
			{
				throw new IOException("not initialized");
			}
			return ParamSpi.EngineGetEncoded(format);
		}

		/// <summary>
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters, or null if this
		/// parameter object has not been initialized. </returns>
		public sealed override String ToString()
		{
			if (this.Initialized == false)
			{
				return null;
			}
			return ParamSpi.EngineToString();
		}
	}

}