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
	/// This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the {@code AlgorithmParameters} class, which is used to manage
	/// algorithm parameters.
	/// 
	/// <para> All the abstract methods in this class must be implemented by each
	/// cryptographic service provider who wishes to supply parameter management
	/// for a particular algorithm.
	/// 
	/// @author Jan Luehe
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AlgorithmParameters </seealso>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>
	/// <seealso cref= java.security.spec.DSAParameterSpec
	/// 
	/// @since 1.2 </seealso>

	public abstract class AlgorithmParametersSpi
	{

		/// <summary>
		/// Initializes this parameters object using the parameters
		/// specified in {@code paramSpec}.
		/// </summary>
		/// <param name="paramSpec"> the parameter specification.
		/// </param>
		/// <exception cref="InvalidParameterSpecException"> if the given parameter
		/// specification is inappropriate for the initialization of this parameter
		/// object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineInit(java.security.spec.AlgorithmParameterSpec paramSpec) throws java.security.spec.InvalidParameterSpecException;
		protected internal abstract void EngineInit(AlgorithmParameterSpec paramSpec);

		/// <summary>
		/// Imports the specified parameters and decodes them
		/// according to the primary decoding format for parameters.
		/// The primary decoding format for parameters is ASN.1, if an ASN.1
		/// specification for this type of parameters exists.
		/// </summary>
		/// <param name="params"> the encoded parameters.
		/// </param>
		/// <exception cref="IOException"> on decoding errors </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineInit(byte[] params) throws IOException;
		protected internal abstract void EngineInit(sbyte[] @params);

		/// <summary>
		/// Imports the parameters from {@code params} and
		/// decodes them according to the specified decoding format.
		/// If {@code format} is null, the
		/// primary decoding format for parameters is used. The primary decoding
		/// format is ASN.1, if an ASN.1 specification for these parameters
		/// exists.
		/// </summary>
		/// <param name="params"> the encoded parameters.
		/// </param>
		/// <param name="format"> the name of the decoding format.
		/// </param>
		/// <exception cref="IOException"> on decoding errors </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void engineInit(byte[] params, String format) throws IOException;
		protected internal abstract void EngineInit(sbyte[] @params, String format);

		/// <summary>
		/// Returns a (transparent) specification of this parameters
		/// object.
		/// {@code paramSpec} identifies the specification class in which
		/// the parameters should be returned. It could, for example, be
		/// {@code DSAParameterSpec.class}, to indicate that the
		/// parameters should be returned in an instance of the
		/// {@code DSAParameterSpec} class.
		/// </summary>
		/// @param <T> the type of the parameter specification to be returned
		/// </param>
		/// <param name="paramSpec"> the specification class in which
		/// the parameters should be returned.
		/// </param>
		/// <returns> the parameter specification.
		/// </returns>
		/// <exception cref="InvalidParameterSpecException"> if the requested parameter
		/// specification is inappropriate for this parameter object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract <T extends java.security.spec.AlgorithmParameterSpec> T engineGetParameterSpec(Class paramSpec) throws java.security.spec.InvalidParameterSpecException;
		protected internal abstract T engineGetParameterSpec<T>(Class paramSpec) where T : java.security.spec.AlgorithmParameterSpec;

		/// <summary>
		/// Returns the parameters in their primary encoding format.
		/// The primary encoding format for parameters is ASN.1, if an ASN.1
		/// specification for this type of parameters exists.
		/// </summary>
		/// <returns> the parameters encoded using their primary encoding format.
		/// </returns>
		/// <exception cref="IOException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract byte[] engineGetEncoded() throws IOException;
		protected internal abstract sbyte[] EngineGetEncoded();

		/// <summary>
		/// Returns the parameters encoded in the specified format.
		/// If {@code format} is null, the
		/// primary encoding format for parameters is used. The primary encoding
		/// format is ASN.1, if an ASN.1 specification for these parameters
		/// exists.
		/// </summary>
		/// <param name="format"> the name of the encoding format.
		/// </param>
		/// <returns> the parameters encoded using the specified encoding scheme.
		/// </returns>
		/// <exception cref="IOException"> on encoding errors. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract byte[] engineGetEncoded(String format) throws IOException;
		protected internal abstract sbyte[] EngineGetEncoded(String format);

		/// <summary>
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters. </returns>
		protected internal abstract String EngineToString();
	}

}