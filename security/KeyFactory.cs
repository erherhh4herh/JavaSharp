using System;
using System.Collections.Generic;

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


	using Debug = sun.security.util.Debug;
	using sun.security.jca;
	using Instance = sun.security.jca.GetInstance.Instance;

	/// <summary>
	/// Key factories are used to convert <I>keys</I> (opaque
	/// cryptographic keys of type {@code Key}) into <I>key specifications</I>
	/// (transparent representations of the underlying key material), and vice
	/// versa.
	/// 
	/// <P> Key factories are bi-directional. That is, they allow you to build an
	/// opaque key object from a given key specification (key material), or to
	/// retrieve the underlying key material of a key object in a suitable format.
	/// 
	/// <P> Multiple compatible key specifications may exist for the same key.
	/// For example, a DSA public key may be specified using
	/// {@code DSAPublicKeySpec} or
	/// {@code X509EncodedKeySpec}. A key factory can be used to translate
	/// between compatible key specifications.
	/// 
	/// <P> The following is an example of how to use a key factory in order to
	/// instantiate a DSA public key from its encoding.
	/// Assume Alice has received a digital signature from Bob.
	/// Bob also sent her his public key (in encoded format) to verify
	/// his signature. Alice then performs the following actions:
	/// 
	/// <pre>
	/// X509EncodedKeySpec bobPubKeySpec = new X509EncodedKeySpec(bobEncodedPubKey);
	/// KeyFactory keyFactory = KeyFactory.getInstance("DSA");
	/// PublicKey bobPubKey = keyFactory.generatePublic(bobPubKeySpec);
	/// Signature sig = Signature.getInstance("DSA");
	/// sig.initVerify(bobPubKey);
	/// sig.update(data);
	/// sig.verify(signature);
	/// </pre>
	/// 
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code KeyFactory} algorithms:
	/// <ul>
	/// <li>{@code DiffieHellman}</li>
	/// <li>{@code DSA}</li>
	/// <li>{@code RSA}</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
	/// KeyFactory section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Jan Luehe
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Key </seealso>
	/// <seealso cref= PublicKey </seealso>
	/// <seealso cref= PrivateKey </seealso>
	/// <seealso cref= java.security.spec.KeySpec </seealso>
	/// <seealso cref= java.security.spec.DSAPublicKeySpec </seealso>
	/// <seealso cref= java.security.spec.X509EncodedKeySpec
	/// 
	/// @since 1.2 </seealso>

	public class KeyFactory
	{

		private static readonly Debug Debug = Debug.getInstance("jca", "KeyFactory");

		// The algorithm associated with this key factory
		private readonly String Algorithm_Renamed;

		// The provider
		private Provider Provider_Renamed;

		// The provider implementation (delegate)
		private volatile KeyFactorySpi Spi;

		// lock for mutex during provider selection
		private readonly Object @lock = new Object();

		// remaining services to try in provider selection
		// null once provider is selected
		private Iterator<Service> ServiceIterator;

		/// <summary>
		/// Creates a KeyFactory object.
		/// </summary>
		/// <param name="keyFacSpi"> the delegate </param>
		/// <param name="provider"> the provider </param>
		/// <param name="algorithm"> the name of the algorithm
		/// to associate with this {@code KeyFactory} </param>
		protected internal KeyFactory(KeyFactorySpi keyFacSpi, Provider provider, String algorithm)
		{
			this.Spi = keyFacSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private KeyFactory(String algorithm) throws NoSuchAlgorithmException
		private KeyFactory(String algorithm)
		{
			this.Algorithm_Renamed = algorithm;
			List<Service> list = GetInstance.getServices("KeyFactory", algorithm);
			ServiceIterator = list.Iterator();
			// fetch and instantiate initial spi
			if (NextSpi(null) == null)
			{
				throw new NoSuchAlgorithmException(algorithm + " KeyFactory not available");
			}
		}

		/// <summary>
		/// Returns a KeyFactory object that converts
		/// public/private keys of the specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new KeyFactory object encapsulating the
		/// KeyFactorySpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested key algorithm.
		/// See the KeyFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> the new KeyFactory object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          KeyFactorySpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyFactory getInstance(String algorithm) throws NoSuchAlgorithmException
		public static KeyFactory GetInstance(String algorithm)
		{
			return new KeyFactory(algorithm);
		}

		/// <summary>
		/// Returns a KeyFactory object that converts
		/// public/private keys of the specified algorithm.
		/// 
		/// <para> A new KeyFactory object encapsulating the
		/// KeyFactorySpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested key algorithm.
		/// See the KeyFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> the new KeyFactory object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a KeyFactorySpi
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
//ORIGINAL LINE: public static KeyFactory getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static KeyFactory GetInstance(String algorithm, String provider)
		{
			Instance instance = GetInstance.getInstance("KeyFactory", typeof(KeyFactorySpi), algorithm, provider);
			return new KeyFactory((KeyFactorySpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a KeyFactory object that converts
		/// public/private keys of the specified algorithm.
		/// 
		/// <para> A new KeyFactory object encapsulating the
		/// KeyFactorySpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the requested key algorithm.
		/// See the KeyFactory section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> the new KeyFactory object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a KeyFactorySpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyFactory getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static KeyFactory GetInstance(String algorithm, Provider provider)
		{
			Instance instance = GetInstance.getInstance("KeyFactory", typeof(KeyFactorySpi), algorithm, provider);
			return new KeyFactory((KeyFactorySpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns the provider of this key factory object.
		/// </summary>
		/// <returns> the provider of this key factory object </returns>
		public Provider Provider
		{
			get
			{
				lock (@lock)
				{
					// disable further failover after this call
					ServiceIterator = null;
					return Provider_Renamed;
				}
			}
		}

		/// <summary>
		/// Gets the name of the algorithm
		/// associated with this {@code KeyFactory}.
		/// </summary>
		/// <returns> the name of the algorithm associated with this
		/// {@code KeyFactory} </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Update the active KeyFactorySpi of this class and return the next
		/// implementation for failover. If no more implemenations are
		/// available, this method returns null. However, the active spi of
		/// this class is never set to null.
		/// </summary>
		private KeyFactorySpi NextSpi(KeyFactorySpi oldSpi)
		{
			lock (@lock)
			{
				// somebody else did a failover concurrently
				// try that spi now
				if ((oldSpi != null) && (oldSpi != Spi))
				{
					return Spi;
				}
				if (ServiceIterator == null)
				{
					return null;
				}
				while (ServiceIterator.HasNext())
				{
					Service s = ServiceIterator.Next();
					try
					{
						Object obj = s.NewInstance(null);
						if (obj is KeyFactorySpi == false)
						{
							continue;
						}
						KeyFactorySpi spi = (KeyFactorySpi)obj;
						Provider_Renamed = s.Provider;
						this.Spi = spi;
						return spi;
					}
					catch (NoSuchAlgorithmException)
					{
						// ignore
					}
				}
				ServiceIterator = null;
				return null;
			}
		}

		/// <summary>
		/// Generates a public key object from the provided key specification
		/// (key material).
		/// </summary>
		/// <param name="keySpec"> the specification (key material) of the public key.
		/// </param>
		/// <returns> the public key.
		/// </returns>
		/// <exception cref="InvalidKeySpecException"> if the given key specification
		/// is inappropriate for this key factory to produce a public key. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final PublicKey generatePublic(java.security.spec.KeySpec keySpec) throws java.security.spec.InvalidKeySpecException
		public PublicKey GeneratePublic(KeySpec keySpec)
		{
			if (ServiceIterator == null)
			{
				return Spi.EngineGeneratePublic(keySpec);
			}
			Exception failure = null;
			KeyFactorySpi mySpi = Spi;
			do
			{
				try
				{
					return mySpi.EngineGeneratePublic(keySpec);
				}
				catch (Exception e)
				{
					if (failure == null)
					{
						failure = e;
					}
					mySpi = NextSpi(mySpi);
				}
			} while (mySpi != null);
			if (failure is RuntimeException)
			{
				throw (RuntimeException)failure;
			}
			if (failure is InvalidKeySpecException)
			{
				throw (InvalidKeySpecException)failure;
			}
			throw new InvalidKeySpecException("Could not generate public key", failure);
		}

		/// <summary>
		/// Generates a private key object from the provided key specification
		/// (key material).
		/// </summary>
		/// <param name="keySpec"> the specification (key material) of the private key.
		/// </param>
		/// <returns> the private key.
		/// </returns>
		/// <exception cref="InvalidKeySpecException"> if the given key specification
		/// is inappropriate for this key factory to produce a private key. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final PrivateKey generatePrivate(java.security.spec.KeySpec keySpec) throws java.security.spec.InvalidKeySpecException
		public PrivateKey GeneratePrivate(KeySpec keySpec)
		{
			if (ServiceIterator == null)
			{
				return Spi.EngineGeneratePrivate(keySpec);
			}
			Exception failure = null;
			KeyFactorySpi mySpi = Spi;
			do
			{
				try
				{
					return mySpi.EngineGeneratePrivate(keySpec);
				}
				catch (Exception e)
				{
					if (failure == null)
					{
						failure = e;
					}
					mySpi = NextSpi(mySpi);
				}
			} while (mySpi != null);
			if (failure is RuntimeException)
			{
				throw (RuntimeException)failure;
			}
			if (failure is InvalidKeySpecException)
			{
				throw (InvalidKeySpecException)failure;
			}
			throw new InvalidKeySpecException("Could not generate private key", failure);
		}

		/// <summary>
		/// Returns a specification (key material) of the given key object.
		/// {@code keySpec} identifies the specification class in which
		/// the key material should be returned. It could, for example, be
		/// {@code DSAPublicKeySpec.class}, to indicate that the
		/// key material should be returned in an instance of the
		/// {@code DSAPublicKeySpec} class.
		/// </summary>
		/// @param <T> the type of the key specification to be returned
		/// </param>
		/// <param name="key"> the key.
		/// </param>
		/// <param name="keySpec"> the specification class in which
		/// the key material should be returned.
		/// </param>
		/// <returns> the underlying key specification (key material) in an instance
		/// of the requested specification class.
		/// </returns>
		/// <exception cref="InvalidKeySpecException"> if the requested key specification is
		/// inappropriate for the given key, or the given key cannot be processed
		/// (e.g., the given key has an unrecognized algorithm or format). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final <T extends java.security.spec.KeySpec> T getKeySpec(Key key, Class keySpec) throws java.security.spec.InvalidKeySpecException
		public T getKeySpec<T>(Key key, Class keySpec) where T : java.security.spec.KeySpec
		{
			if (ServiceIterator == null)
			{
				return Spi.EngineGetKeySpec(key, keySpec);
			}
			Exception failure = null;
			KeyFactorySpi mySpi = Spi;
			do
			{
				try
				{
					return mySpi.EngineGetKeySpec(key, keySpec);
				}
				catch (Exception e)
				{
					if (failure == null)
					{
						failure = e;
					}
					mySpi = NextSpi(mySpi);
				}
			} while (mySpi != null);
			if (failure is RuntimeException)
			{
				throw (RuntimeException)failure;
			}
			if (failure is InvalidKeySpecException)
			{
				throw (InvalidKeySpecException)failure;
			}
			throw new InvalidKeySpecException("Could not get key spec", failure);
		}

		/// <summary>
		/// Translates a key object, whose provider may be unknown or potentially
		/// untrusted, into a corresponding key object of this key factory.
		/// </summary>
		/// <param name="key"> the key whose provider is unknown or untrusted.
		/// </param>
		/// <returns> the translated key.
		/// </returns>
		/// <exception cref="InvalidKeyException"> if the given key cannot be processed
		/// by this key factory. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Key translateKey(Key key) throws InvalidKeyException
		public Key TranslateKey(Key key)
		{
			if (ServiceIterator == null)
			{
				return Spi.EngineTranslateKey(key);
			}
			Exception failure = null;
			KeyFactorySpi mySpi = Spi;
			do
			{
				try
				{
					return mySpi.EngineTranslateKey(key);
				}
				catch (Exception e)
				{
					if (failure == null)
					{
						failure = e;
					}
					mySpi = NextSpi(mySpi);
				}
			} while (mySpi != null);
			if (failure is RuntimeException)
			{
				throw (RuntimeException)failure;
			}
			if (failure is InvalidKeyException)
			{
				throw (InvalidKeyException)failure;
			}
			throw new InvalidKeyException("Could not translate key", failure);
		}

	}

}