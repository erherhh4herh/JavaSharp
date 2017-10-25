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

namespace java.security.interfaces
{

	/// <summary>
	/// An interface to an object capable of generating DSA key pairs.
	/// 
	/// <para>The {@code initialize} methods may each be called any number
	/// of times. If no {@code initialize} method is called on a
	/// DSAKeyPairGenerator, the default is to generate 1024-bit keys, using
	/// precomputed p, q and g parameters and an instance of SecureRandom as
	/// the random bit source.
	/// 
	/// </para>
	/// <para>Users wishing to indicate DSA-specific parameters, and to generate a key
	/// pair suitable for use with the DSA algorithm typically
	/// 
	/// <ol>
	/// 
	/// <li>Get a key pair generator for the DSA algorithm by calling the
	/// KeyPairGenerator {@code getInstance} method with "DSA"
	/// as its argument.
	/// 
	/// <li>Initialize the generator by casting the result to a DSAKeyPairGenerator
	/// and calling one of the
	/// {@code initialize} methods from this DSAKeyPairGenerator interface.
	/// 
	/// <li>Generate a key pair by calling the {@code generateKeyPair}
	/// method from the KeyPairGenerator class.
	/// 
	/// </ol>
	/// 
	/// </para>
	/// <para>Note: it is not always necessary to do do algorithm-specific
	/// initialization for a DSA key pair generator. That is, it is not always
	/// necessary to call an {@code initialize} method in this interface.
	/// Algorithm-independent initialization using the {@code initialize} method
	/// in the KeyPairGenerator
	/// interface is all that is needed when you accept defaults for algorithm-specific
	/// parameters.
	/// 
	/// </para>
	/// <para>Note: Some earlier implementations of this interface may not support
	/// larger sizes of DSA parameters such as 2048 and 3072-bit.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.KeyPairGenerator </seealso>
	public interface DSAKeyPairGenerator
	{

		/// <summary>
		/// Initializes the key pair generator using the DSA family parameters
		/// (p,q and g) and an optional SecureRandom bit source. If a
		/// SecureRandom bit source is needed but not supplied, i.e. null, a
		/// default SecureRandom instance will be used.
		/// </summary>
		/// <param name="params"> the parameters to use to generate the keys.
		/// </param>
		/// <param name="random"> the random bit source to use to generate key bits;
		/// can be null.
		/// </param>
		/// <exception cref="InvalidParameterException"> if the {@code params}
		/// value is invalid, null, or unsupported. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(DSAParams params, SecureRandom random) throws InvalidParameterException;
	   void Initialize(DSAParams @params, SecureRandom random);

		/// <summary>
		/// Initializes the key pair generator for a given modulus length
		/// (instead of parameters), and an optional SecureRandom bit source.
		/// If a SecureRandom bit source is needed but not supplied, i.e.
		/// null, a default SecureRandom instance will be used.
		/// 
		/// <para>If {@code genParams} is true, this method generates new
		/// p, q and g parameters. If it is false, the method uses precomputed
		/// parameters for the modulus length requested. If there are no
		/// precomputed parameters for that modulus length, an exception will be
		/// thrown. It is guaranteed that there will always be
		/// default parameters for modulus lengths of 512 and 1024 bits.
		/// 
		/// </para>
		/// </summary>
		/// <param name="modlen"> the modulus length in bits. Valid values are any
		/// multiple of 64 between 512 and 1024, inclusive, 2048, and 3072.
		/// </param>
		/// <param name="random"> the random bit source to use to generate key bits;
		/// can be null.
		/// </param>
		/// <param name="genParams"> whether or not to generate new parameters for
		/// the modulus length requested.
		/// </param>
		/// <exception cref="InvalidParameterException"> if {@code modlen} is
		/// invalid, or unsupported, or if {@code genParams} is false and there
		/// are no precomputed parameters for the requested modulus length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(int modlen, boolean genParams, SecureRandom random) throws InvalidParameterException;
		void Initialize(int modlen, bool genParams, SecureRandom random);
	}

}