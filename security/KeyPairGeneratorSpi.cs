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
	/// <para> This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the {@code KeyPairGenerator} class, which is used to generate
	/// pairs of public and private keys.
	/// 
	/// </para>
	/// <para> All the abstract methods in this class must be implemented by each
	/// cryptographic service provider who wishes to supply the implementation
	/// of a key pair generator for a particular algorithm.
	/// 
	/// </para>
	/// <para> In case the client does not explicitly initialize the KeyPairGenerator
	/// (via a call to an {@code initialize} method), each provider must
	/// supply (and document) a default initialization.
	/// For example, the <i>Sun</i> provider uses a default modulus size (keysize)
	/// of 1024 bits.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= KeyPairGenerator </seealso>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>

	public abstract class KeyPairGeneratorSpi
	{

		/// <summary>
		/// Initializes the key pair generator for a certain keysize, using
		/// the default parameter set.
		/// </summary>
		/// <param name="keysize"> the keysize. This is an
		/// algorithm-specific metric, such as modulus length, specified in
		/// number of bits.
		/// </param>
		/// <param name="random"> the source of randomness for this generator.
		/// </param>
		/// <exception cref="InvalidParameterException"> if the {@code keysize} is not
		/// supported by this KeyPairGeneratorSpi object. </exception>
		public abstract void Initialize(int keysize, SecureRandom random);

		/// <summary>
		/// Initializes the key pair generator using the specified parameter
		/// set and user-provided source of randomness.
		/// 
		/// <para>This concrete method has been added to this previously-defined
		/// abstract class. (For backwards compatibility, it cannot be abstract.)
		/// It may be overridden by a provider to initialize the key pair
		/// generator. Such an override
		/// is expected to throw an InvalidAlgorithmParameterException if
		/// a parameter is inappropriate for this key pair generator.
		/// If this method is not overridden, it always throws an
		/// UnsupportedOperationException.
		/// 
		/// </para>
		/// </summary>
		/// <param name="params"> the parameter set used to generate the keys.
		/// </param>
		/// <param name="random"> the source of randomness for this generator.
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		/// are inappropriate for this key pair generator.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(java.security.spec.AlgorithmParameterSpec params, SecureRandom random) throws InvalidAlgorithmParameterException
		public virtual void Initialize(AlgorithmParameterSpec @params, SecureRandom random)
		{
				throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Generates a key pair. Unless an initialization method is called
		/// using a KeyPairGenerator interface, algorithm-specific defaults
		/// will be used. This will generate a new key pair every time it
		/// is called.
		/// </summary>
		/// <returns> the newly generated {@code KeyPair} </returns>
		public abstract KeyPair GenerateKeyPair();
	}

}