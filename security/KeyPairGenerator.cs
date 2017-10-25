using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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


	using sun.security.jca;
	using Instance = sun.security.jca.GetInstance.Instance;
	using Debug = sun.security.util.Debug;

	/// <summary>
	/// The KeyPairGenerator class is used to generate pairs of
	/// public and private keys. Key pair generators are constructed using the
	/// {@code getInstance} factory methods (static methods that
	/// return instances of a given class).
	/// 
	/// <para>A Key pair generator for a particular algorithm creates a public/private
	/// key pair that can be used with this algorithm. It also associates
	/// algorithm-specific parameters with each of the generated keys.
	/// 
	/// </para>
	/// <para>There are two ways to generate a key pair: in an algorithm-independent
	/// manner, and in an algorithm-specific manner.
	/// The only difference between the two is the initialization of the object:
	/// 
	/// <ul>
	/// <li><b>Algorithm-Independent Initialization</b>
	/// </para>
	/// <para>All key pair generators share the concepts of a keysize and a
	/// source of randomness. The keysize is interpreted differently for different
	/// algorithms (e.g., in the case of the <i>DSA</i> algorithm, the keysize
	/// corresponds to the length of the modulus).
	/// There is an
	/// <seealso cref="#initialize(int, java.security.SecureRandom) initialize"/>
	/// method in this KeyPairGenerator class that takes these two universally
	/// shared types of arguments. There is also one that takes just a
	/// {@code keysize} argument, and uses the {@code SecureRandom}
	/// implementation of the highest-priority installed provider as the source
	/// of randomness. (If none of the installed providers supply an implementation
	/// of {@code SecureRandom}, a system-provided source of randomness is
	/// used.)
	/// 
	/// </para>
	/// <para>Since no other parameters are specified when you call the above
	/// algorithm-independent {@code initialize} methods, it is up to the
	/// provider what to do about the algorithm-specific parameters (if any) to be
	/// associated with each of the keys.
	/// 
	/// </para>
	/// <para>If the algorithm is the <i>DSA</i> algorithm, and the keysize (modulus
	/// size) is 512, 768, or 1024, then the <i>Sun</i> provider uses a set of
	/// precomputed values for the {@code p}, {@code q}, and
	/// {@code g} parameters. If the modulus size is not one of the above
	/// values, the <i>Sun</i> provider creates a new set of parameters. Other
	/// providers might have precomputed parameter sets for more than just the
	/// three modulus sizes mentioned above. Still others might not have a list of
	/// precomputed parameters at all and instead always create new parameter sets.
	/// 
	/// <li><b>Algorithm-Specific Initialization</b>
	/// </para>
	/// <para>For situations where a set of algorithm-specific parameters already
	/// exists (e.g., so-called <i>community parameters</i> in DSA), there are two
	/// {@link #initialize(java.security.spec.AlgorithmParameterSpec)
	/// initialize} methods that have an {@code AlgorithmParameterSpec}
	/// argument. One also has a {@code SecureRandom} argument, while the
	/// the other uses the {@code SecureRandom}
	/// implementation of the highest-priority installed provider as the source
	/// of randomness. (If none of the installed providers supply an implementation
	/// of {@code SecureRandom}, a system-provided source of randomness is
	/// used.)
	/// </ul>
	/// 
	/// </para>
	/// <para>In case the client does not explicitly initialize the KeyPairGenerator
	/// (via a call to an {@code initialize} method), each provider must
	/// supply (and document) a default initialization.
	/// For example, the <i>Sun</i> provider uses a default modulus size (keysize)
	/// of 1024 bits.
	/// 
	/// </para>
	/// <para>Note that this class is abstract and extends from
	/// {@code KeyPairGeneratorSpi} for historical reasons.
	/// Application developers should only take notice of the methods defined in
	/// this {@code KeyPairGenerator} class; all the methods in
	/// the superclass are intended for cryptographic service providers who wish to
	/// supply their own implementations of key pair generators.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code KeyPairGenerator} algorithms and keysizes in
	/// parentheses:
	/// <ul>
	/// <li>{@code DiffieHellman} (1024)</li>
	/// <li>{@code DSA} (1024)</li>
	/// <li>{@code RSA} (1024, 2048)</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
	/// KeyPairGenerator section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>

	public abstract class KeyPairGenerator : KeyPairGeneratorSpi
	{

		private static readonly Debug Pdebug = Debug.getInstance("provider", "Provider");
		private static readonly bool SkipDebug = Debug.isOn("engine=") && !Debug.isOn("keypairgenerator");

		private readonly String Algorithm_Renamed;

		// The provider
		internal Provider Provider_Renamed;

		/// <summary>
		/// Creates a KeyPairGenerator object for the specified algorithm.
		/// </summary>
		/// <param name="algorithm"> the standard string name of the algorithm.
		/// See the KeyPairGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names. </param>
		protected internal KeyPairGenerator(String algorithm)
		{
			this.Algorithm_Renamed = algorithm;
		}

		/// <summary>
		/// Returns the standard name of the algorithm for this key pair generator.
		/// See the KeyPairGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </summary>
		/// <returns> the standard string name of the algorithm. </returns>
		public virtual String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		private static KeyPairGenerator GetInstance(Instance instance, String algorithm)
		{
			KeyPairGenerator kpg;
			if (instance.impl is KeyPairGenerator)
			{
				kpg = (KeyPairGenerator)instance.impl;
			}
			else
			{
				KeyPairGeneratorSpi spi = (KeyPairGeneratorSpi)instance.impl;
				kpg = new Delegate(spi, algorithm);
			}
			kpg.Provider_Renamed = instance.provider;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("KeyPairGenerator." + algorithm + " algorithm from: " + kpg.Provider_Renamed.Name);
			}

			return kpg;
		}

		/// <summary>
		/// Returns a KeyPairGenerator object that generates public/private
		/// key pairs for the specified algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new KeyPairGenerator object encapsulating the
		/// KeyPairGeneratorSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the standard string name of the algorithm.
		/// See the KeyPairGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> the new KeyPairGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          KeyPairGeneratorSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyPairGenerator getInstance(String algorithm) throws NoSuchAlgorithmException
		public static KeyPairGenerator GetInstance(String algorithm)
		{
			List<Service> list = GetInstance.getServices("KeyPairGenerator", algorithm);
			Iterator<Service> t = list.Iterator();
			if (t.HasNext() == false)
			{
				throw new NoSuchAlgorithmException(algorithm + " KeyPairGenerator not available");
			}
			// find a working Spi or KeyPairGenerator subclass
			NoSuchAlgorithmException failure = null;
			do
			{
				Service s = t.Next();
				try
				{
					Instance instance = GetInstance.getInstance(s, typeof(KeyPairGeneratorSpi));
					if (instance.impl is KeyPairGenerator)
					{
						return GetInstance(instance, algorithm);
					}
					else
					{
						return new Delegate(instance, t, algorithm);
					}
				}
				catch (NoSuchAlgorithmException e)
				{
					if (failure == null)
					{
						failure = e;
					}
				}
			} while (t.HasNext());
			throw failure;
		}

		/// <summary>
		/// Returns a KeyPairGenerator object that generates public/private
		/// key pairs for the specified algorithm.
		/// 
		/// <para> A new KeyPairGenerator object encapsulating the
		/// KeyPairGeneratorSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the standard string name of the algorithm.
		/// See the KeyPairGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the string name of the provider.
		/// </param>
		/// <returns> the new KeyPairGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a KeyPairGeneratorSpi
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
//ORIGINAL LINE: public static KeyPairGenerator getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static KeyPairGenerator GetInstance(String algorithm, String provider)
		{
			Instance instance = GetInstance.getInstance("KeyPairGenerator", typeof(KeyPairGeneratorSpi), algorithm, provider);
			return GetInstance(instance, algorithm);
		}

		/// <summary>
		/// Returns a KeyPairGenerator object that generates public/private
		/// key pairs for the specified algorithm.
		/// 
		/// <para> A new KeyPairGenerator object encapsulating the
		/// KeyPairGeneratorSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the standard string name of the algorithm.
		/// See the KeyPairGenerator section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> the new KeyPairGenerator object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a KeyPairGeneratorSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyPairGenerator getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static KeyPairGenerator GetInstance(String algorithm, Provider provider)
		{
			Instance instance = GetInstance.getInstance("KeyPairGenerator", typeof(KeyPairGeneratorSpi), algorithm, provider);
			return GetInstance(instance, algorithm);
		}

		/// <summary>
		/// Returns the provider of this key pair generator object.
		/// </summary>
		/// <returns> the provider of this key pair generator object </returns>
		public Provider Provider
		{
			get
			{
				DisableFailover();
				return this.Provider_Renamed;
			}
		}

		internal virtual void DisableFailover()
		{
			// empty, overridden in Delegate
		}

		/// <summary>
		/// Initializes the key pair generator for a certain keysize using
		/// a default parameter set and the {@code SecureRandom}
		/// implementation of the highest-priority installed provider as the source
		/// of randomness.
		/// (If none of the installed providers supply an implementation of
		/// {@code SecureRandom}, a system-provided source of randomness is
		/// used.)
		/// </summary>
		/// <param name="keysize"> the keysize. This is an
		/// algorithm-specific metric, such as modulus length, specified in
		/// number of bits.
		/// </param>
		/// <exception cref="InvalidParameterException"> if the {@code keysize} is not
		/// supported by this KeyPairGenerator object. </exception>
		public virtual void Initialize(int keysize)
		{
			Initialize(keysize, JCAUtil.SecureRandom);
		}

		/// <summary>
		/// Initializes the key pair generator for a certain keysize with
		/// the given source of randomness (and a default parameter set).
		/// </summary>
		/// <param name="keysize"> the keysize. This is an
		/// algorithm-specific metric, such as modulus length, specified in
		/// number of bits. </param>
		/// <param name="random"> the source of randomness.
		/// </param>
		/// <exception cref="InvalidParameterException"> if the {@code keysize} is not
		/// supported by this KeyPairGenerator object.
		/// 
		/// @since 1.2 </exception>
		public override void Initialize(int keysize, SecureRandom random)
		{
			// This does nothing, because either
			// 1. the implementation object returned by getInstance() is an
			//    instance of KeyPairGenerator which has its own
			//    initialize(keysize, random) method, so the application would
			//    be calling that method directly, or
			// 2. the implementation returned by getInstance() is an instance
			//    of Delegate, in which case initialize(keysize, random) is
			//    overridden to call the corresponding SPI method.
			// (This is a special case, because the API and SPI method have the
			// same name.)
		}

		/// <summary>
		/// Initializes the key pair generator using the specified parameter
		/// set and the {@code SecureRandom}
		/// implementation of the highest-priority installed provider as the source
		/// of randomness.
		/// (If none of the installed providers supply an implementation of
		/// {@code SecureRandom}, a system-provided source of randomness is
		/// used.).
		/// 
		/// <para>This concrete method has been added to this previously-defined
		/// abstract class.
		/// This method calls the KeyPairGeneratorSpi
		/// {@link KeyPairGeneratorSpi#initialize(
		/// java.security.spec.AlgorithmParameterSpec,
		/// java.security.SecureRandom) initialize} method,
		/// passing it {@code params} and a source of randomness (obtained
		/// from the highest-priority installed provider or system-provided if none
		/// of the installed providers supply one).
		/// That {@code initialize} method always throws an
		/// UnsupportedOperationException if it is not overridden by the provider.
		/// 
		/// </para>
		/// </summary>
		/// <param name="params"> the parameter set used to generate the keys.
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		/// are inappropriate for this key pair generator.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(java.security.spec.AlgorithmParameterSpec params) throws InvalidAlgorithmParameterException
		public virtual void Initialize(AlgorithmParameterSpec @params)
		{
			Initialize(@params, JCAUtil.SecureRandom);
		}

		/// <summary>
		/// Initializes the key pair generator with the given parameter
		/// set and source of randomness.
		/// 
		/// <para>This concrete method has been added to this previously-defined
		/// abstract class.
		/// This method calls the KeyPairGeneratorSpi {@link
		/// KeyPairGeneratorSpi#initialize(
		/// java.security.spec.AlgorithmParameterSpec,
		/// java.security.SecureRandom) initialize} method,
		/// passing it {@code params} and {@code random}.
		/// That {@code initialize}
		/// method always throws an
		/// UnsupportedOperationException if it is not overridden by the provider.
		/// 
		/// </para>
		/// </summary>
		/// <param name="params"> the parameter set used to generate the keys. </param>
		/// <param name="random"> the source of randomness.
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		/// are inappropriate for this key pair generator.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(java.security.spec.AlgorithmParameterSpec params, SecureRandom random) throws InvalidAlgorithmParameterException
		public override void Initialize(AlgorithmParameterSpec @params, SecureRandom random)
		{
			// This does nothing, because either
			// 1. the implementation object returned by getInstance() is an
			//    instance of KeyPairGenerator which has its own
			//    initialize(params, random) method, so the application would
			//    be calling that method directly, or
			// 2. the implementation returned by getInstance() is an instance
			//    of Delegate, in which case initialize(params, random) is
			//    overridden to call the corresponding SPI method.
			// (This is a special case, because the API and SPI method have the
			// same name.)
		}

		/// <summary>
		/// Generates a key pair.
		/// 
		/// <para>If this KeyPairGenerator has not been initialized explicitly,
		/// provider-specific defaults will be used for the size and other
		/// (algorithm-specific) values of the generated keys.
		/// 
		/// </para>
		/// <para>This will generate a new key pair every time it is called.
		/// 
		/// </para>
		/// <para>This method is functionally equivalent to
		/// <seealso cref="#generateKeyPair() generateKeyPair"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the generated key pair
		/// 
		/// @since 1.2 </returns>
		public KeyPair GenKeyPair()
		{
			return GenerateKeyPair();
		}

		/// <summary>
		/// Generates a key pair.
		/// 
		/// <para>If this KeyPairGenerator has not been initialized explicitly,
		/// provider-specific defaults will be used for the size and other
		/// (algorithm-specific) values of the generated keys.
		/// 
		/// </para>
		/// <para>This will generate a new key pair every time it is called.
		/// 
		/// </para>
		/// <para>This method is functionally equivalent to
		/// <seealso cref="#genKeyPair() genKeyPair"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the generated key pair </returns>
		public override KeyPair GenerateKeyPair()
		{
			// This does nothing (except returning null), because either:
			//
			// 1. the implementation object returned by getInstance() is an
			//    instance of KeyPairGenerator which has its own implementation
			//    of generateKeyPair (overriding this one), so the application
			//    would be calling that method directly, or
			//
			// 2. the implementation returned by getInstance() is an instance
			//    of Delegate, in which case generateKeyPair is
			//    overridden to invoke the corresponding SPI method.
			//
			// (This is a special case, because in JDK 1.1.x the generateKeyPair
			// method was used both as an API and a SPI method.)
			return null;
		}


		/*
		 * The following class allows providers to extend from KeyPairGeneratorSpi
		 * rather than from KeyPairGenerator. It represents a KeyPairGenerator
		 * with an encapsulated, provider-supplied SPI object (of type
		 * KeyPairGeneratorSpi).
		 * If the provider implementation is an instance of KeyPairGeneratorSpi,
		 * the getInstance() methods above return an instance of this class, with
		 * the SPI object encapsulated.
		 *
		 * Note: All SPI methods from the original KeyPairGenerator class have been
		 * moved up the hierarchy into a new class (KeyPairGeneratorSpi), which has
		 * been interposed in the hierarchy between the API (KeyPairGenerator)
		 * and its original parent (Object).
		 */

		//
		// error failover notes:
		//
		//  . we failover if the implementation throws an error during init
		//    by retrying the init on other providers
		//
		//  . we also failover if the init succeeded but the subsequent call
		//    to generateKeyPair() fails. In order for this to work, we need
		//    to remember the parameters to the last successful call to init
		//    and initialize() the next spi using them.
		//
		//  . although not specified, KeyPairGenerators could be thread safe,
		//    so we make sure we do not interfere with that
		//
		//  . failover is not available, if:
		//    . getInstance(algorithm, provider) was used
		//    . a provider extends KeyPairGenerator rather than
		//      KeyPairGeneratorSpi (JDK 1.1 style)
		//    . once getProvider() is called
		//

		private sealed class Delegate : KeyPairGenerator
		{

			// The provider implementation (delegate)
			internal volatile KeyPairGeneratorSpi Spi;

			internal readonly Object @lock = new Object();

			internal Iterator<Service> ServiceIterator;

			internal const int I_NONE = 1;
			internal const int I_SIZE = 2;
			internal const int I_PARAMS = 3;

			internal int InitType;
			internal int InitKeySize;
			internal AlgorithmParameterSpec InitParams;
			internal SecureRandom InitRandom;

			// constructor
			internal Delegate(KeyPairGeneratorSpi spi, String algorithm) : base(algorithm)
			{
				this.Spi = spi;
			}

			internal Delegate(Instance instance, Iterator<Service> serviceIterator, String algorithm) : base(algorithm)
			{
				Spi = (KeyPairGeneratorSpi)instance.impl;
				Provider_Renamed = instance.provider;
				this.ServiceIterator = serviceIterator;
				InitType = I_NONE;

				if (!SkipDebug && Pdebug != null)
				{
					Pdebug.println("KeyPairGenerator." + algorithm + " algorithm from: " + Provider_Renamed.Name);
				}
			}

			/// <summary>
			/// Update the active spi of this class and return the next
			/// implementation for failover. If no more implemenations are
			/// available, this method returns null. However, the active spi of
			/// this class is never set to null.
			/// </summary>
			internal KeyPairGeneratorSpi NextSpi(KeyPairGeneratorSpi oldSpi, bool reinit)
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
							Object inst = s.NewInstance(null);
							// ignore non-spis
							if (inst is KeyPairGeneratorSpi == false)
							{
								continue;
							}
							if (inst is KeyPairGenerator)
							{
								continue;
							}
							KeyPairGeneratorSpi spi = (KeyPairGeneratorSpi)inst;
							if (reinit)
							{
								if (InitType == I_SIZE)
								{
									spi.Initialize(InitKeySize, InitRandom);
								}
								else if (InitType == I_PARAMS)
								{
									spi.Initialize(InitParams, InitRandom);
								}
								else if (InitType != I_NONE)
								{
									throw new AssertionError("KeyPairGenerator initType: " + InitType);
								}
							}
							Provider_Renamed = s.Provider;
							this.Spi = spi;
							return spi;
						}
						catch (Exception)
						{
							// ignore
						}
					}
					DisableFailover();
					return null;
				}
			}

			internal override void DisableFailover()
			{
				ServiceIterator = null;
				InitType = 0;
				InitParams = null;
				InitRandom = null;
			}

			// engine method
			public override void Initialize(int keysize, SecureRandom random)
			{
				if (ServiceIterator == null)
				{
					Spi.Initialize(keysize, random);
					return;
				}
				RuntimeException failure = null;
				KeyPairGeneratorSpi mySpi = Spi;
				do
				{
					try
					{
						mySpi.Initialize(keysize, random);
						InitType = I_SIZE;
						InitKeySize = keysize;
						InitParams = null;
						InitRandom = random;
						return;
					}
					catch (RuntimeException e)
					{
						if (failure == null)
						{
							failure = e;
						}
						mySpi = NextSpi(mySpi, false);
					}
				} while (mySpi != null);
				throw failure;
			}

			// engine method
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(java.security.spec.AlgorithmParameterSpec params, SecureRandom random) throws InvalidAlgorithmParameterException
			public override void Initialize(AlgorithmParameterSpec @params, SecureRandom random)
			{
				if (ServiceIterator == null)
				{
					Spi.Initialize(@params, random);
					return;
				}
				Exception failure = null;
				KeyPairGeneratorSpi mySpi = Spi;
				do
				{
					try
					{
						mySpi.Initialize(@params, random);
						InitType = I_PARAMS;
						InitKeySize = 0;
						InitParams = @params;
						InitRandom = random;
						return;
					}
					catch (Exception e)
					{
						if (failure == null)
						{
							failure = e;
						}
						mySpi = NextSpi(mySpi, false);
					}
				} while (mySpi != null);
				if (failure is RuntimeException)
				{
					throw (RuntimeException)failure;
				}
				// must be an InvalidAlgorithmParameterException
				throw (InvalidAlgorithmParameterException)failure;
			}

			// engine method
			public override KeyPair GenerateKeyPair()
			{
				if (ServiceIterator == null)
				{
					return Spi.GenerateKeyPair();
				}
				RuntimeException failure = null;
				KeyPairGeneratorSpi mySpi = Spi;
				do
				{
					try
					{
						return mySpi.GenerateKeyPair();
					}
					catch (RuntimeException e)
					{
						if (failure == null)
						{
							failure = e;
						}
						mySpi = NextSpi(mySpi, true);
					}
				} while (mySpi != null);
				throw failure;
			}
		}

	}

}