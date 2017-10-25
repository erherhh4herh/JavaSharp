using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class provides a cryptographically strong random number
	/// generator (RNG).
	/// 
	/// <para>A cryptographically strong random number
	/// minimally complies with the statistical random number generator tests
	/// specified in <a href="http://csrc.nist.gov/cryptval/140-2.htm">
	/// <i>FIPS 140-2, Security Requirements for Cryptographic Modules</i></a>,
	/// section 4.9.1.
	/// Additionally, SecureRandom must produce non-deterministic output.
	/// Therefore any seed material passed to a SecureRandom object must be
	/// unpredictable, and all SecureRandom output sequences must be
	/// cryptographically strong, as described in
	/// <a href="http://www.ietf.org/rfc/rfc1750.txt">
	/// <i>RFC 1750: Randomness Recommendations for Security</i></a>.
	/// 
	/// </para>
	/// <para>A caller obtains a SecureRandom instance via the
	/// no-argument constructor or one of the {@code getInstance} methods:
	/// 
	/// <pre>
	///      SecureRandom random = new SecureRandom();
	/// </pre>
	/// 
	/// </para>
	/// <para> Many SecureRandom implementations are in the form of a pseudo-random
	/// number generator (PRNG), which means they use a deterministic algorithm
	/// to produce a pseudo-random sequence from a true random seed.
	/// Other implementations may produce true random numbers,
	/// and yet others may use a combination of both techniques.
	/// 
	/// </para>
	/// <para> Typical callers of SecureRandom invoke the following methods
	/// to retrieve random bytes:
	/// 
	/// <pre>
	///      SecureRandom random = new SecureRandom();
	///      byte bytes[] = new byte[20];
	///      random.nextBytes(bytes);
	/// </pre>
	/// 
	/// </para>
	/// <para> Callers may also invoke the {@code generateSeed} method
	/// to generate a given number of seed bytes (to seed other random number
	/// generators, for example):
	/// <pre>
	///      byte seed[] = random.generateSeed(20);
	/// </pre>
	/// 
	/// Note: Depending on the implementation, the {@code generateSeed} and
	/// {@code nextBytes} methods may block as entropy is being gathered,
	/// for example, if they need to read from /dev/random on various Unix-like
	/// operating systems.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.SecureRandomSpi </seealso>
	/// <seealso cref= java.util.Random
	/// 
	/// @author Benjamin Renaud
	/// @author Josh Bloch </seealso>

	public class SecureRandom : Random
	{

		private static readonly Debug Pdebug = Debug.getInstance("provider", "Provider");
		private static readonly bool SkipDebug = Debug.isOn("engine=") && !Debug.isOn("securerandom");

		/// <summary>
		/// The provider.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private Provider Provider_Renamed = null;

		/// <summary>
		/// The provider implementation.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private SecureRandomSpi SecureRandomSpi_Renamed = null;

		/*
		 * The algorithm name of null if unknown.
		 *
		 * @serial
		 * @since 1.5
		 */
		private String Algorithm_Renamed;

		// Seed Generator
		private static volatile SecureRandom SeedGenerator = null;

		/// <summary>
		/// Constructs a secure random number generator (RNG) implementing the
		/// default random number algorithm.
		/// 
		/// <para> This constructor traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new SecureRandom object encapsulating the
		/// SecureRandomSpi implementation from the first
		/// Provider that supports a SecureRandom (RNG) algorithm is returned.
		/// If none of the Providers support a RNG algorithm,
		/// then an implementation-specific default is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para> See the SecureRandom section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard RNG algorithm names.
		/// 
		/// </para>
		/// <para> The returned SecureRandom object has not been seeded.  To seed the
		/// returned object, call the {@code setSeed} method.
		/// If {@code setSeed} is not called, the first call to
		/// {@code nextBytes} will force the SecureRandom object to seed itself.
		/// This self-seeding will not occur if {@code setSeed} was
		/// previously called.
		/// </para>
		/// </summary>
		public SecureRandom() : base(0)
		{
			/*
			 * This call to our superclass constructor will result in a call
			 * to our own {@code setSeed} method, which will return
			 * immediately when it is passed zero.
			 */
			GetDefaultPRNG(false, null);
		}

		/// <summary>
		/// Constructs a secure random number generator (RNG) implementing the
		/// default random number algorithm.
		/// The SecureRandom instance is seeded with the specified seed bytes.
		/// 
		/// <para> This constructor traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new SecureRandom object encapsulating the
		/// SecureRandomSpi implementation from the first
		/// Provider that supports a SecureRandom (RNG) algorithm is returned.
		/// If none of the Providers support a RNG algorithm,
		/// then an implementation-specific default is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para> See the SecureRandom section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard RNG algorithm names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seed"> the seed. </param>
		public SecureRandom(sbyte[] seed) : base(0)
		{
			GetDefaultPRNG(true, seed);
		}

		private void GetDefaultPRNG(bool setSeed, sbyte[] seed)
		{
			String prng = PrngAlgorithm;
			if (prng == null)
			{
				// bummer, get the SUN implementation
				prng = "SHA1PRNG";
				this.SecureRandomSpi_Renamed = new sun.security.provider.SecureRandom();
				this.Provider_Renamed = Providers.SunProvider;
				if (setSeed)
				{
					this.SecureRandomSpi_Renamed.EngineSetSeed(seed);
				}
			}
			else
			{
				try
				{
					SecureRandom random = SecureRandom.GetInstance(prng);
					this.SecureRandomSpi_Renamed = random.SecureRandomSpi;
					this.Provider_Renamed = random.Provider;
					if (setSeed)
					{
						this.SecureRandomSpi_Renamed.EngineSetSeed(seed);
					}
				}
				catch (NoSuchAlgorithmException nsae)
				{
					// never happens, because we made sure the algorithm exists
					throw new RuntimeException(nsae);
				}
			}
			// JDK 1.1 based implementations subclass SecureRandom instead of
			// SecureRandomSpi. They will also go through this code path because
			// they must call a SecureRandom constructor as it is their superclass.
			// If we are dealing with such an implementation, do not set the
			// algorithm value as it would be inaccurate.
			if (this.GetType() == typeof(SecureRandom))
			{
				this.Algorithm_Renamed = prng;
			}
		}

		/// <summary>
		/// Creates a SecureRandom object.
		/// </summary>
		/// <param name="secureRandomSpi"> the SecureRandom implementation. </param>
		/// <param name="provider"> the provider. </param>
		protected internal SecureRandom(SecureRandomSpi secureRandomSpi, Provider provider) : this(secureRandomSpi, provider, null)
		{
		}

		private SecureRandom(SecureRandomSpi secureRandomSpi, Provider provider, String algorithm) : base(0)
		{
			this.SecureRandomSpi_Renamed = secureRandomSpi;
			this.Provider_Renamed = provider;
			this.Algorithm_Renamed = algorithm;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("SecureRandom." + algorithm + " algorithm from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Returns a SecureRandom object that implements the specified
		/// Random Number Generator (RNG) algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new SecureRandom object encapsulating the
		/// SecureRandomSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para> The returned SecureRandom object has not been seeded.  To seed the
		/// returned object, call the {@code setSeed} method.
		/// If {@code setSeed} is not called, the first call to
		/// {@code nextBytes} will force the SecureRandom object to seed itself.
		/// This self-seeding will not occur if {@code setSeed} was
		/// previously called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the RNG algorithm.
		/// See the SecureRandom section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard RNG algorithm names.
		/// </param>
		/// <returns> the new SecureRandom object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          SecureRandomSpi implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SecureRandom getInstance(String algorithm) throws NoSuchAlgorithmException
		public static SecureRandom GetInstance(String algorithm)
		{
			Instance instance = GetInstance.getInstance("SecureRandom", typeof(SecureRandomSpi), algorithm);
			return new SecureRandom((SecureRandomSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a SecureRandom object that implements the specified
		/// Random Number Generator (RNG) algorithm.
		/// 
		/// <para> A new SecureRandom object encapsulating the
		/// SecureRandomSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// <para> The returned SecureRandom object has not been seeded.  To seed the
		/// returned object, call the {@code setSeed} method.
		/// If {@code setSeed} is not called, the first call to
		/// {@code nextBytes} will force the SecureRandom object to seed itself.
		/// This self-seeding will not occur if {@code setSeed} was
		/// previously called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the RNG algorithm.
		/// See the SecureRandom section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard RNG algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> the new SecureRandom object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a SecureRandomSpi
		///          implementation for the specified algorithm is not
		///          available from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider name is null
		///          or empty.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SecureRandom getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static SecureRandom GetInstance(String algorithm, String provider)
		{
			Instance instance = GetInstance.getInstance("SecureRandom", typeof(SecureRandomSpi), algorithm, provider);
			return new SecureRandom((SecureRandomSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns a SecureRandom object that implements the specified
		/// Random Number Generator (RNG) algorithm.
		/// 
		/// <para> A new SecureRandom object encapsulating the
		/// SecureRandomSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// <para> The returned SecureRandom object has not been seeded.  To seed the
		/// returned object, call the {@code setSeed} method.
		/// If {@code setSeed} is not called, the first call to
		/// {@code nextBytes} will force the SecureRandom object to seed itself.
		/// This self-seeding will not occur if {@code setSeed} was
		/// previously called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the RNG algorithm.
		/// See the SecureRandom section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard RNG algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> the new SecureRandom object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a SecureRandomSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SecureRandom getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static SecureRandom GetInstance(String algorithm, Provider provider)
		{
			Instance instance = GetInstance.getInstance("SecureRandom", typeof(SecureRandomSpi), algorithm, provider);
			return new SecureRandom((SecureRandomSpi)instance.impl, instance.provider, algorithm);
		}

		/// <summary>
		/// Returns the SecureRandomSpi of this SecureRandom object.
		/// </summary>
		internal virtual SecureRandomSpi SecureRandomSpi
		{
			get
			{
				return SecureRandomSpi_Renamed;
			}
		}

		/// <summary>
		/// Returns the provider of this SecureRandom object.
		/// </summary>
		/// <returns> the provider of this SecureRandom object. </returns>
		public Provider Provider
		{
			get
			{
				return Provider_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the algorithm implemented by this SecureRandom
		/// object.
		/// </summary>
		/// <returns> the name of the algorithm or {@code unknown}
		///          if the algorithm name cannot be determined.
		/// @since 1.5 </returns>
		public virtual String Algorithm
		{
			get
			{
				return (Algorithm_Renamed != null) ? Algorithm_Renamed : "unknown";
			}
		}

		/// <summary>
		/// Reseeds this random object. The given seed supplements, rather than
		/// replaces, the existing seed. Thus, repeated calls are guaranteed
		/// never to reduce randomness.
		/// </summary>
		/// <param name="seed"> the seed.
		/// </param>
		/// <seealso cref= #getSeed </seealso>
		public virtual sbyte[] Seed
		{
			set
			{
				lock (this)
				{
					SecureRandomSpi_Renamed.EngineSetSeed(value);
				}
			}
		}

		/// <summary>
		/// Reseeds this random object, using the eight bytes contained
		/// in the given {@code long seed}. The given seed supplements,
		/// rather than replaces, the existing seed. Thus, repeated calls
		/// are guaranteed never to reduce randomness.
		/// 
		/// <para>This method is defined for compatibility with
		/// {@code java.util.Random}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seed"> the seed.
		/// </param>
		/// <seealso cref= #getSeed </seealso>
		public override long Seed
		{
			set
			{
				/*
				 * Ignore call from super constructor (as well as any other calls
				 * unfortunate enough to be passing 0).  It's critical that we
				 * ignore call from superclass constructor, as digest has not
				 * yet been initialized at that point.
				 */
				if (value != 0)
				{
					SecureRandomSpi_Renamed.EngineSetSeed(LongToByteArray(value));
				}
			}
		}

		/// <summary>
		/// Generates a user-specified number of random bytes.
		/// 
		/// <para> If a call to {@code setSeed} had not occurred previously,
		/// the first call to this method forces this SecureRandom object
		/// to seed itself.  This self-seeding will not occur if
		/// {@code setSeed} was previously called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes"> the array to be filled in with random bytes. </param>
		public override void NextBytes(sbyte[] bytes)
		{
			lock (this)
			{
				SecureRandomSpi_Renamed.EngineNextBytes(bytes);
			}
		}

		/// <summary>
		/// Generates an integer containing the user-specified number of
		/// pseudo-random bits (right justified, with leading zeros).  This
		/// method overrides a {@code java.util.Random} method, and serves
		/// to provide a source of random bits to all of the methods inherited
		/// from that class (for example, {@code nextInt},
		/// {@code nextLong}, and {@code nextFloat}).
		/// </summary>
		/// <param name="numBits"> number of pseudo-random bits to be generated, where
		/// {@code 0 <= numBits <= 32}.
		/// </param>
		/// <returns> an {@code int} containing the user-specified number
		/// of pseudo-random bits (right justified, with leading zeros). </returns>
		protected internal override sealed int Next(int numBits)
		{
			int numBytes = (numBits + 7) / 8;
			sbyte[] b = new sbyte[numBytes];
			int next = 0;

			NextBytes(b);
			for (int i = 0; i < numBytes; i++)
			{
				next = (next << 8) + (b[i] & 0xFF);
			}

			return (int)((uint)next >> (numBytes * 8 - numBits));
		}

		/// <summary>
		/// Returns the given number of seed bytes, computed using the seed
		/// generation algorithm that this class uses to seed itself.  This
		/// call may be used to seed other random number generators.
		/// 
		/// <para>This method is only included for backwards compatibility.
		/// The caller is encouraged to use one of the alternative
		/// {@code getInstance} methods to obtain a SecureRandom object, and
		/// then call the {@code generateSeed} method to obtain seed bytes
		/// from that object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="numBytes"> the number of seed bytes to generate.
		/// </param>
		/// <returns> the seed bytes.
		/// </returns>
		/// <seealso cref= #setSeed </seealso>
		public static sbyte[] GetSeed(int numBytes)
		{
			if (SeedGenerator == null)
			{
				SeedGenerator = new SecureRandom();
			}
			return SeedGenerator.GenerateSeed(numBytes);
		}

		/// <summary>
		/// Returns the given number of seed bytes, computed using the seed
		/// generation algorithm that this class uses to seed itself.  This
		/// call may be used to seed other random number generators.
		/// </summary>
		/// <param name="numBytes"> the number of seed bytes to generate.
		/// </param>
		/// <returns> the seed bytes. </returns>
		public virtual sbyte[] GenerateSeed(int numBytes)
		{
			return SecureRandomSpi_Renamed.EngineGenerateSeed(numBytes);
		}

		/// <summary>
		/// Helper function to convert a long into a byte array (least significant
		/// byte first).
		/// </summary>
		private static sbyte[] LongToByteArray(long l)
		{
			sbyte[] retVal = new sbyte[8];

			for (int i = 0; i < 8; i++)
			{
				retVal[i] = (sbyte) l;
				l >>= 8;
			}

			return retVal;
		}

		/// <summary>
		/// Gets a default PRNG algorithm by looking through all registered
		/// providers. Returns the first PRNG algorithm of the first provider that
		/// has registered a SecureRandom implementation, or null if none of the
		/// registered providers supplies a SecureRandom implementation.
		/// </summary>
		private static String PrngAlgorithm
		{
			get
			{
				foreach (Provider p in Providers.ProviderList.providers())
				{
					foreach (Service s in p.Services)
					{
						if (s.Type.Equals("SecureRandom"))
						{
							return s.Algorithm;
						}
					}
				}
				return null;
			}
		}

		/*
		 * Lazily initialize since Pattern.compile() is heavy.
		 * Effective Java (2nd Edition), Item 71.
		 */
		private sealed class StrongPatternHolder
		{
			/*
			 * Entries are alg:prov separated by ,
			 * Allow for prepended/appended whitespace between entries.
			 *
			 * Capture groups:
			 *     1 - alg
			 *     2 - :prov (optional)
			 *     3 - prov (optional)
			 *     4 - ,nextEntry (optional)
			 *     5 - nextEntry (optional)
			 */
			internal static Pattern Pattern = Pattern.Compile("\\s*([\\S&&[^:,]]*)(\\:([\\S&&[^,]]*))?\\s*(\\,(.*))?");
		}

		/// <summary>
		/// Returns a {@code SecureRandom} object that was selected by using
		/// the algorithms/providers specified in the {@code
		/// securerandom.strongAlgorithms} <seealso cref="Security"/> property.
		/// <para>
		/// Some situations require strong random values, such as when
		/// creating high-value/long-lived secrets like RSA public/private
		/// keys.  To help guide applications in selecting a suitable strong
		/// {@code SecureRandom} implementation, Java distributions
		/// include a list of known strong {@code SecureRandom}
		/// implementations in the {@code securerandom.strongAlgorithms}
		/// Security property.
		/// </para>
		/// <para>
		/// Every implementation of the Java platform is required to
		/// support at least one strong {@code SecureRandom} implementation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a strong {@code SecureRandom} implementation as indicated
		/// by the {@code securerandom.strongAlgorithms} Security property
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no algorithm is available
		/// </exception>
		/// <seealso cref= Security#getProperty(String)
		/// 
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SecureRandom getInstanceStrong() throws NoSuchAlgorithmException
		public static SecureRandom InstanceStrong
		{
			get
			{
    
				String property = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
    
				if ((property == null) || (property.Length() == 0))
				{
					throw new NoSuchAlgorithmException("Null/empty securerandom.strongAlgorithms Security Property");
				}
    
				String remainder = property;
				while (remainder != null)
				{
					Matcher m;
					if ((m = StrongPatternHolder.Pattern.Matcher(remainder)).matches())
					{
    
						String alg = m.Group(1);
						String prov = m.Group(3);
    
						try
						{
							if (prov == null)
							{
								return SecureRandom.GetInstance(alg);
							}
							else
							{
								return SecureRandom.GetInstance(alg, prov);
							}
						}
	//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
						catch (NoSuchAlgorithmException | NoSuchProviderException e)
						{
						}
						remainder = m.Group(5);
					}
					else
					{
						remainder = null;
					}
				}
    
				throw new NoSuchAlgorithmException("No strong SecureRandom impls available: " + property);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty("securerandom.strongAlgorithms");
			}
		}

		// Declare serialVersionUID to be compatible with JDK1.1
		internal new const long SerialVersionUID = 4940670005562187L;

		// Retain unused values serialized from JDK1.1
		/// <summary>
		/// @serial
		/// </summary>
		private sbyte[] State;
		/// <summary>
		/// @serial
		/// </summary>
		private MessageDigest Digest = null;
		/// <summary>
		/// @serial
		/// 
		/// We know that the MessageDigest class does not implement
		/// java.io.Serializable.  However, since this field is no longer
		/// used, it will always be NULL and won't affect the serialization
		/// of the SecureRandom class itself.
		/// </summary>
		private sbyte[] RandomBytes;
		/// <summary>
		/// @serial
		/// </summary>
		private int RandomBytesUsed;
		/// <summary>
		/// @serial
		/// </summary>
		private long Counter;
	}

}