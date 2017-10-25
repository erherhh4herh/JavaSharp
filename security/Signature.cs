using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	/// The Signature class is used to provide applications the functionality
	/// of a digital signature algorithm. Digital signatures are used for
	/// authentication and integrity assurance of digital data.
	/// 
	/// <para> The signature algorithm can be, among others, the NIST standard
	/// DSA, using DSA and SHA-1. The DSA algorithm using the
	/// SHA-1 message digest algorithm can be specified as {@code SHA1withDSA}.
	/// In the case of RSA, there are multiple choices for the message digest
	/// algorithm, so the signing algorithm could be specified as, for example,
	/// {@code MD2withRSA}, {@code MD5withRSA}, or {@code SHA1withRSA}.
	/// The algorithm name must be specified, as there is no default.
	/// 
	/// </para>
	/// <para> A Signature object can be used to generate and verify digital
	/// signatures.
	/// 
	/// </para>
	/// <para> There are three phases to the use of a Signature object for
	/// either signing data or verifying a signature:<ol>
	/// 
	/// <li>Initialization, with either
	/// 
	///     <ul>
	/// 
	///     <li>a public key, which initializes the signature for
	///     verification (see <seealso cref="#initVerify(PublicKey) initVerify"/>), or
	/// 
	///     <li>a private key (and optionally a Secure Random Number Generator),
	///     which initializes the signature for signing
	///     (see <seealso cref="#initSign(PrivateKey)"/>
	///     and <seealso cref="#initSign(PrivateKey, SecureRandom)"/>).
	/// 
	///     </ul>
	/// 
	/// <li>Updating
	/// 
	/// </para>
	/// <para>Depending on the type of initialization, this will update the
	/// bytes to be signed or verified. See the
	/// <seealso cref="#update(byte) update"/> methods.
	/// 
	/// <li>Signing or Verifying a signature on all updated bytes. See the
	/// <seealso cref="#sign() sign"/> methods and the <seealso cref="#verify(byte[]) verify"/>
	/// method.
	/// 
	/// </ol>
	/// 
	/// </para>
	/// <para>Note that this class is abstract and extends from
	/// {@code SignatureSpi} for historical reasons.
	/// Application developers should only take notice of the methods defined in
	/// this {@code Signature} class; all the methods in
	/// the superclass are intended for cryptographic service providers who wish to
	/// supply their own implementations of digital signature algorithms.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support the
	/// following standard {@code Signature} algorithms:
	/// <ul>
	/// <li>{@code SHA1withDSA}</li>
	/// <li>{@code SHA1withRSA}</li>
	/// <li>{@code SHA256withRSA}</li>
	/// </ul>
	/// These algorithms are described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
	/// Signature section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other algorithms are supported.
	/// 
	/// @author Benjamin Renaud
	/// 
	/// </para>
	/// </summary>

	public abstract class Signature : SignatureSpi
	{

		private static readonly Debug Debug = Debug.getInstance("jca", "Signature");

		private static readonly Debug Pdebug = Debug.getInstance("provider", "Provider");
		private static readonly bool SkipDebug = Debug.isOn("engine=") && !Debug.isOn("signature");

		/*
		 * The algorithm for this signature object.
		 * This value is used to map an OID to the particular algorithm.
		 * The mapping is done in AlgorithmObject.algOID(String algorithm)
		 */
		private String Algorithm_Renamed;

		// The provider
		internal Provider Provider_Renamed;

		/// <summary>
		/// Possible <seealso cref="#state"/> value, signifying that
		/// this signature object has not yet been initialized.
		/// </summary>
		protected internal const int UNINITIALIZED = 0;

		/// <summary>
		/// Possible <seealso cref="#state"/> value, signifying that
		/// this signature object has been initialized for signing.
		/// </summary>
		protected internal const int SIGN = 2;

		/// <summary>
		/// Possible <seealso cref="#state"/> value, signifying that
		/// this signature object has been initialized for verification.
		/// </summary>
		protected internal const int VERIFY = 3;

		/// <summary>
		/// Current state of this signature object.
		/// </summary>
		protected internal int State = UNINITIALIZED;

		/// <summary>
		/// Creates a Signature object for the specified algorithm.
		/// </summary>
		/// <param name="algorithm"> the standard string name of the algorithm.
		/// See the Signature section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names. </param>
		protected internal Signature(String algorithm)
		{
			this.Algorithm_Renamed = algorithm;
		}

		// name of the special signature alg
		private const String RSA_SIGNATURE = "NONEwithRSA";

		// name of the equivalent cipher alg
		private const String RSA_CIPHER = "RSA/ECB/PKCS1Padding";

		// all the services we need to lookup for compatibility with Cipher
		private static readonly List<ServiceId> RsaIds = new ServiceId[] {new ServiceId("Signature", "NONEwithRSA"), new ServiceId("Cipher", "RSA/ECB/PKCS1Padding"), new ServiceId("Cipher", "RSA/ECB"), new ServiceId("Cipher", "RSA//PKCS1Padding"), new ServiceId("Cipher", "RSA")};

		/// <summary>
		/// Returns a Signature object that implements the specified signature
		/// algorithm.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new Signature object encapsulating the
		/// SignatureSpi implementation from the first
		/// Provider that supports the specified algorithm is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the standard name of the algorithm requested.
		/// See the Signature section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <returns> the new Signature object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		///          Signature implementation for the
		///          specified algorithm.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Signature getInstance(String algorithm) throws NoSuchAlgorithmException
		public static Signature GetInstance(String algorithm)
		{
			List<Service> list;
			if (algorithm.EqualsIgnoreCase(RSA_SIGNATURE))
			{
				list = GetInstance.getServices(RsaIds);
			}
			else
			{
				list = GetInstance.getServices("Signature", algorithm);
			}
			Iterator<Service> t = list.Iterator();
			if (t.HasNext() == false)
			{
				throw new NoSuchAlgorithmException(algorithm + " Signature not available");
			}
			// try services until we find an Spi or a working Signature subclass
			NoSuchAlgorithmException failure;
			do
			{
				Service s = t.Next();
				if (IsSpi(s))
				{
					return new Delegate(s, t, algorithm);
				}
				else
				{
					// must be a subclass of Signature, disable dynamic selection
					try
					{
						Instance instance = GetInstance.getInstance(s, typeof(SignatureSpi));
						return GetInstance(instance, algorithm);
					}
					catch (NoSuchAlgorithmException e)
					{
						failure = e;
					}
				}
			} while (t.HasNext());
			throw failure;
		}

		private static Signature GetInstance(Instance instance, String algorithm)
		{
			Signature sig;
			if (instance.impl is Signature)
			{
				sig = (Signature)instance.impl;
				sig.Algorithm_Renamed = algorithm;
			}
			else
			{
				SignatureSpi spi = (SignatureSpi)instance.impl;
				sig = new Delegate(spi, algorithm);
			}
			sig.Provider_Renamed = instance.provider;
			return sig;
		}

		private static readonly Map<String, Boolean> SignatureInfo;

		static Signature()
		{
			SignatureInfo = new ConcurrentDictionary<String, Boolean>();
			Boolean TRUE = true;
			// pre-initialize with values for our SignatureSpi implementations
			SignatureInfo.Put("sun.security.provider.DSA$RawDSA", TRUE);
			SignatureInfo.Put("sun.security.provider.DSA$SHA1withDSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$MD2withRSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$MD5withRSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$SHA1withRSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$SHA256withRSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$SHA384withRSA", TRUE);
			SignatureInfo.Put("sun.security.rsa.RSASignature$SHA512withRSA", TRUE);
			SignatureInfo.Put("com.sun.net.ssl.internal.ssl.RSASignature", TRUE);
			SignatureInfo.Put("sun.security.pkcs11.P11Signature", TRUE);
		}

		private static bool IsSpi(Service s)
		{
			if (s.Type.Equals("Cipher"))
			{
				// must be a CipherSpi, which we can wrap with the CipherAdapter
				return true;
			}
			String className = s.ClassName;
			Boolean result = SignatureInfo.Get(className);
			if (result == null)
			{
				try
				{
					Object instance = s.NewInstance(null);
					// Signature extends SignatureSpi
					// so it is a "real" Spi if it is an
					// instance of SignatureSpi but not Signature
					bool r = (instance is SignatureSpi) && (instance is Signature == false);
					if ((Debug != null) && (r == false))
					{
						Debug.println("Not a SignatureSpi " + className);
						Debug.println("Delayed provider selection may not be " + "available for algorithm " + s.Algorithm);
					}
					result = Convert.ToBoolean(r);
					SignatureInfo.Put(className, result);
				}
				catch (Exception)
				{
					// something is wrong, assume not an SPI
					return false;
				}
			}
			return result.BooleanValue();
		}

		/// <summary>
		/// Returns a Signature object that implements the specified signature
		/// algorithm.
		/// 
		/// <para> A new Signature object encapsulating the
		/// SignatureSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the Signature section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> the new Signature object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a SignatureSpi
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
//ORIGINAL LINE: public static Signature getInstance(String algorithm, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		public static Signature GetInstance(String algorithm, String provider)
		{
			if (algorithm.EqualsIgnoreCase(RSA_SIGNATURE))
			{
				// exception compatibility with existing code
				if ((provider == null) || (provider.Length() == 0))
				{
					throw new IllegalArgumentException("missing provider");
				}
				Provider p = Security.GetProvider(provider);
				if (p == null)
				{
					throw new NoSuchProviderException("no such provider: " + provider);
				}
				return GetInstanceRSA(p);
			}
			Instance instance = GetInstance.getInstance("Signature", typeof(SignatureSpi), algorithm, provider);
			return GetInstance(instance, algorithm);
		}

		/// <summary>
		/// Returns a Signature object that implements the specified
		/// signature algorithm.
		/// 
		/// <para> A new Signature object encapsulating the
		/// SignatureSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="algorithm"> the name of the algorithm requested.
		/// See the Signature section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> the new Signature object.
		/// </returns>
		/// <exception cref="NoSuchAlgorithmException"> if a SignatureSpi
		///          implementation for the specified algorithm is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Signature getInstance(String algorithm, Provider provider) throws NoSuchAlgorithmException
		public static Signature GetInstance(String algorithm, Provider provider)
		{
			if (algorithm.EqualsIgnoreCase(RSA_SIGNATURE))
			{
				// exception compatibility with existing code
				if (provider == null)
				{
					throw new IllegalArgumentException("missing provider");
				}
				return GetInstanceRSA(provider);
			}
			Instance instance = GetInstance.getInstance("Signature", typeof(SignatureSpi), algorithm, provider);
			return GetInstance(instance, algorithm);
		}

		// return an implementation for NONEwithRSA, which is a special case
		// because of the Cipher.RSA/ECB/PKCS1Padding compatibility wrapper
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Signature getInstanceRSA(Provider p) throws NoSuchAlgorithmException
		private static Signature GetInstanceRSA(Provider p)
		{
			// try Signature first
			Service s = p.GetService("Signature", RSA_SIGNATURE);
			if (s != null)
			{
				Instance instance = GetInstance.getInstance(s, typeof(SignatureSpi));
				return GetInstance(instance, RSA_SIGNATURE);
			}
			// check Cipher
			try
			{
				Cipher c = Cipher.getInstance(RSA_CIPHER, p);
				return new Delegate(new CipherAdapter(c), RSA_SIGNATURE);
			}
			catch (GeneralSecurityException e)
			{
				// throw Signature style exception message to avoid confusion,
				// but append Cipher exception as cause
				throw new NoSuchAlgorithmException("no such algorithm: " + RSA_SIGNATURE + " for provider " + p.Name, e);
			}
		}

		/// <summary>
		/// Returns the provider of this signature object.
		/// </summary>
		/// <returns> the provider of this signature object </returns>
		public Provider Provider
		{
			get
			{
				ChooseFirstProvider();
				return this.Provider_Renamed;
			}
		}

		internal virtual void ChooseFirstProvider()
		{
			// empty, overridden in Delegate
		}

		/// <summary>
		/// Initializes this object for verification. If this method is called
		/// again with a different argument, it negates the effect
		/// of this call.
		/// </summary>
		/// <param name="publicKey"> the public key of the identity whose signature is
		/// going to be verified.
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void initVerify(PublicKey publicKey) throws InvalidKeyException
		public void InitVerify(PublicKey publicKey)
		{
			EngineInitVerify(publicKey);
			State = VERIFY;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("Signature." + Algorithm_Renamed + " verification algorithm from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Initializes this object for verification, using the public key from
		/// the given certificate.
		/// <para>If the certificate is of type X.509 and has a <i>key usage</i>
		/// extension field marked as critical, and the value of the <i>key usage</i>
		/// extension field implies that the public key in
		/// the certificate and its corresponding private key are not
		/// supposed to be used for digital signatures, an
		/// {@code InvalidKeyException} is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certificate"> the certificate of the identity whose signature is
		/// going to be verified.
		/// </param>
		/// <exception cref="InvalidKeyException">  if the public key in the certificate
		/// is not encoded properly or does not include required  parameter
		/// information or cannot be used for digital signature purposes.
		/// @since 1.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void initVerify(java.security.cert.Certificate certificate) throws InvalidKeyException
		public void InitVerify(Certificate certificate)
		{
			// If the certificate is of type X509Certificate,
			// we should check whether it has a Key Usage
			// extension marked as critical.
			if (certificate is X509Certificate)
			{
				// Check whether the cert has a key usage extension
				// marked as a critical extension.
				// The OID for KeyUsage extension is 2.5.29.15.
				X509Certificate cert = (X509Certificate)certificate;
				Set<String> critSet = cert.CriticalExtensionOIDs;

				if (critSet != null && critSet.Count > 0 && critSet.Contains("2.5.29.15"))
				{
					bool[] keyUsageInfo = cert.KeyUsage;
					// keyUsageInfo[0] is for digitalSignature.
					if ((keyUsageInfo != null) && (keyUsageInfo[0] == false))
					{
						throw new InvalidKeyException("Wrong key usage");
					}
				}
			}

			PublicKey publicKey = certificate.PublicKey;
			EngineInitVerify(publicKey);
			State = VERIFY;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("Signature." + Algorithm_Renamed + " verification algorithm from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Initialize this object for signing. If this method is called
		/// again with a different argument, it negates the effect
		/// of this call.
		/// </summary>
		/// <param name="privateKey"> the private key of the identity whose signature
		/// is going to be generated.
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void initSign(PrivateKey privateKey) throws InvalidKeyException
		public void InitSign(PrivateKey privateKey)
		{
			EngineInitSign(privateKey);
			State = SIGN;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("Signature." + Algorithm_Renamed + " signing algorithm from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Initialize this object for signing. If this method is called
		/// again with a different argument, it negates the effect
		/// of this call.
		/// </summary>
		/// <param name="privateKey"> the private key of the identity whose signature
		/// is going to be generated.
		/// </param>
		/// <param name="random"> the source of randomness for this signature.
		/// </param>
		/// <exception cref="InvalidKeyException"> if the key is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void initSign(PrivateKey privateKey, SecureRandom random) throws InvalidKeyException
		public void InitSign(PrivateKey privateKey, SecureRandom random)
		{
			EngineInitSign(privateKey, random);
			State = SIGN;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("Signature." + Algorithm_Renamed + " signing algorithm from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Returns the signature bytes of all the data updated.
		/// The format of the signature depends on the underlying
		/// signature scheme.
		/// 
		/// <para>A call to this method resets this signature object to the state
		/// it was in when previously initialized for signing via a
		/// call to {@code initSign(PrivateKey)}. That is, the object is
		/// reset and available to generate another signature from the same
		/// signer, if desired, via new calls to {@code update} and
		/// {@code sign}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the signature bytes of the signing operation's result.
		/// </returns>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly or if this signature algorithm is unable to
		/// process the input data provided. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final byte[] sign() throws SignatureException
		public sbyte[] Sign()
		{
			if (State == SIGN)
			{
				return EngineSign();
			}
			throw new SignatureException("object not initialized for " + "signing");
		}

		/// <summary>
		/// Finishes the signature operation and stores the resulting signature
		/// bytes in the provided buffer {@code outbuf}, starting at
		/// {@code offset}.
		/// The format of the signature depends on the underlying
		/// signature scheme.
		/// 
		/// <para>This signature object is reset to its initial state (the state it
		/// was in after a call to one of the {@code initSign} methods) and
		/// can be reused to generate further signatures with the same private key.
		/// 
		/// </para>
		/// </summary>
		/// <param name="outbuf"> buffer for the signature result.
		/// </param>
		/// <param name="offset"> offset into {@code outbuf} where the signature is
		/// stored.
		/// </param>
		/// <param name="len"> number of bytes within {@code outbuf} allotted for the
		/// signature.
		/// </param>
		/// <returns> the number of bytes placed into {@code outbuf}.
		/// </returns>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly, if this signature algorithm is unable to
		/// process the input data provided, or if {@code len} is less
		/// than the actual signature length.
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int sign(byte[] outbuf, int offset, int len) throws SignatureException
		public int Sign(sbyte[] outbuf, int offset, int len)
		{
			if (outbuf == null)
			{
				throw new IllegalArgumentException("No output buffer given");
			}
			if (offset < 0 || len < 0)
			{
				throw new IllegalArgumentException("offset or len is less than 0");
			}
			if (outbuf.Length - offset < len)
			{
				throw new IllegalArgumentException("Output buffer too small for specified offset and length");
			}
			if (State != SIGN)
			{
				throw new SignatureException("object not initialized for " + "signing");
			}
			return EngineSign(outbuf, offset, len);
		}

		/// <summary>
		/// Verifies the passed-in signature.
		/// 
		/// <para>A call to this method resets this signature object to the state
		/// it was in when previously initialized for verification via a
		/// call to {@code initVerify(PublicKey)}. That is, the object is
		/// reset and available to verify another signature from the identity
		/// whose public key was specified in the call to {@code initVerify}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="signature"> the signature bytes to be verified.
		/// </param>
		/// <returns> true if the signature was verified, false if not.
		/// </returns>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly, the passed-in signature is improperly
		/// encoded or of the wrong type, if this signature algorithm is unable to
		/// process the input data provided, etc. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean verify(byte[] signature) throws SignatureException
		public bool Verify(sbyte[] signature)
		{
			if (State == VERIFY)
			{
				return EngineVerify(signature);
			}
			throw new SignatureException("object not initialized for " + "verification");
		}

		/// <summary>
		/// Verifies the passed-in signature in the specified array
		/// of bytes, starting at the specified offset.
		/// 
		/// <para>A call to this method resets this signature object to the state
		/// it was in when previously initialized for verification via a
		/// call to {@code initVerify(PublicKey)}. That is, the object is
		/// reset and available to verify another signature from the identity
		/// whose public key was specified in the call to {@code initVerify}.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="signature"> the signature bytes to be verified. </param>
		/// <param name="offset"> the offset to start from in the array of bytes. </param>
		/// <param name="length"> the number of bytes to use, starting at offset.
		/// </param>
		/// <returns> true if the signature was verified, false if not.
		/// </returns>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly, the passed-in signature is improperly
		/// encoded or of the wrong type, if this signature algorithm is unable to
		/// process the input data provided, etc. </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code signature}
		/// byte array is null, or the {@code offset} or {@code length}
		/// is less than 0, or the sum of the {@code offset} and
		/// {@code length} is greater than the length of the
		/// {@code signature} byte array.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean verify(byte[] signature, int offset, int length) throws SignatureException
		public bool Verify(sbyte[] signature, int offset, int length)
		{
			if (State == VERIFY)
			{
				if (signature == null)
				{
					throw new IllegalArgumentException("signature is null");
				}
				if (offset < 0 || length < 0)
				{
					throw new IllegalArgumentException("offset or length is less than 0");
				}
				if (signature.Length - offset < length)
				{
					throw new IllegalArgumentException("signature too small for specified offset and length");
				}

				return EngineVerify(signature, offset, length);
			}
			throw new SignatureException("object not initialized for " + "verification");
		}

		/// <summary>
		/// Updates the data to be signed or verified by a byte.
		/// </summary>
		/// <param name="b"> the byte to use for the update.
		/// </param>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void update(byte b) throws SignatureException
		public void Update(sbyte b)
		{
			if (State == VERIFY || State == SIGN)
			{
				EngineUpdate(b);
			}
			else
			{
				throw new SignatureException("object not initialized for " + "signature or verification");
			}
		}

		/// <summary>
		/// Updates the data to be signed or verified, using the specified
		/// array of bytes.
		/// </summary>
		/// <param name="data"> the byte array to use for the update.
		/// </param>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void update(byte[] data) throws SignatureException
		public void Update(sbyte[] data)
		{
			Update(data, 0, data.Length);
		}

		/// <summary>
		/// Updates the data to be signed or verified, using the specified
		/// array of bytes, starting at the specified offset.
		/// </summary>
		/// <param name="data"> the array of bytes. </param>
		/// <param name="off"> the offset to start from in the array of bytes. </param>
		/// <param name="len"> the number of bytes to use, starting at offset.
		/// </param>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void update(byte[] data, int off, int len) throws SignatureException
		public void Update(sbyte[] data, int off, int len)
		{
			if (State == SIGN || State == VERIFY)
			{
				if (data == null)
				{
					throw new IllegalArgumentException("data is null");
				}
				if (off < 0 || len < 0)
				{
					throw new IllegalArgumentException("off or len is less than 0");
				}
				if (data.Length - off < len)
				{
					throw new IllegalArgumentException("data too small for specified offset and length");
				}
				EngineUpdate(data, off, len);
			}
			else
			{
				throw new SignatureException("object not initialized for " + "signature or verification");
			}
		}

		/// <summary>
		/// Updates the data to be signed or verified using the specified
		/// ByteBuffer. Processes the {@code data.remaining()} bytes
		/// starting at at {@code data.position()}.
		/// Upon return, the buffer's position will be equal to its limit;
		/// its limit will not have changed.
		/// </summary>
		/// <param name="data"> the ByteBuffer
		/// </param>
		/// <exception cref="SignatureException"> if this signature object is not
		/// initialized properly.
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void update(java.nio.ByteBuffer data) throws SignatureException
		public void Update(ByteBuffer data)
		{
			if ((State != SIGN) && (State != VERIFY))
			{
				throw new SignatureException("object not initialized for " + "signature or verification");
			}
			if (data == null)
			{
				throw new NullPointerException();
			}
			EngineUpdate(data);
		}

		/// <summary>
		/// Returns the name of the algorithm for this signature object.
		/// </summary>
		/// <returns> the name of the algorithm for this signature object. </returns>
		public String Algorithm
		{
			get
			{
				return this.Algorithm_Renamed;
			}
		}

		/// <summary>
		/// Returns a string representation of this signature object,
		/// providing information that includes the state of the object
		/// and the name of the algorithm used.
		/// </summary>
		/// <returns> a string representation of this signature object. </returns>
		public override String ToString()
		{
			String initState = "";
			switch (State)
			{
			case UNINITIALIZED:
				initState = "<not initialized>";
				break;
			case VERIFY:
				initState = "<initialized for verifying>";
				break;
			case SIGN:
				initState = "<initialized for signing>";
				break;
			}
			return "Signature object: " + Algorithm + initState;
		}

		/// <summary>
		/// Sets the specified algorithm parameter to the specified value.
		/// This method supplies a general-purpose mechanism through
		/// which it is possible to set the various parameters of this object.
		/// A parameter may be any settable parameter for the algorithm, such as
		/// a parameter size, or a source of random bits for signature generation
		/// (if appropriate), or an indication of whether or not to perform
		/// a specific but optional computation. A uniform algorithm-specific
		/// naming scheme for each parameter is desirable but left unspecified
		/// at this time.
		/// </summary>
		/// <param name="param"> the string identifier of the parameter. </param>
		/// <param name="value"> the parameter value.
		/// </param>
		/// <exception cref="InvalidParameterException"> if {@code param} is an
		/// invalid parameter for this signature algorithm engine,
		/// the parameter is already set
		/// and cannot be set again, a security exception occurs, and so on.
		/// </exception>
		/// <seealso cref= #getParameter
		/// </seealso>
		/// @deprecated Use
		/// {@link #setParameter(java.security.spec.AlgorithmParameterSpec)
		/// setParameter}. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use") public final void setParameter(String param, Object value) throws InvalidParameterException
		[Obsolete("Use")]
		public void SetParameter(String param, Object value)
		{
			EngineSetParameter(param, value);
		}

		/// <summary>
		/// Initializes this signature engine with the specified parameter set.
		/// </summary>
		/// <param name="params"> the parameters
		/// </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		/// are inappropriate for this signature engine
		/// </exception>
		/// <seealso cref= #getParameters </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setParameter(java.security.spec.AlgorithmParameterSpec params) throws InvalidAlgorithmParameterException
		public AlgorithmParameterSpec Parameter
		{
			set
			{
				EngineSetParameter(value);
			}
		}

		/// <summary>
		/// Returns the parameters used with this signature object.
		/// 
		/// <para>The returned parameters may be the same that were used to initialize
		/// this signature, or may contain a combination of default and randomly
		/// generated parameter values used by the underlying signature
		/// implementation if this signature requires algorithm parameters but
		/// was not initialized with any.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the parameters used with this signature, or null if this
		/// signature does not use any parameters.
		/// </returns>
		/// <seealso cref= #setParameter(AlgorithmParameterSpec)
		/// @since 1.4 </seealso>
		public AlgorithmParameters Parameters
		{
			get
			{
				return EngineGetParameters();
			}
		}

		/// <summary>
		/// Gets the value of the specified algorithm parameter. This method
		/// supplies a general-purpose mechanism through which it is possible to
		/// get the various parameters of this object. A parameter may be any
		/// settable parameter for the algorithm, such as a parameter size, or
		/// a source of random bits for signature generation (if appropriate),
		/// or an indication of whether or not to perform a specific but optional
		/// computation. A uniform algorithm-specific naming scheme for each
		/// parameter is desirable but left unspecified at this time.
		/// </summary>
		/// <param name="param"> the string name of the parameter.
		/// </param>
		/// <returns> the object that represents the parameter value, or null if
		/// there is none.
		/// </returns>
		/// <exception cref="InvalidParameterException"> if {@code param} is an invalid
		/// parameter for this engine, or another exception occurs while
		/// trying to get this parameter.
		/// </exception>
		/// <seealso cref= #setParameter(String, Object)
		/// 
		/// @deprecated </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated public final Object getParameter(String param) throws InvalidParameterException
		[Obsolete]
		public Object GetParameter(String param)
		{
			return EngineGetParameter(param);
		}

		/// <summary>
		/// Returns a clone if the implementation is cloneable.
		/// </summary>
		/// <returns> a clone if the implementation is cloneable.
		/// </returns>
		/// <exception cref="CloneNotSupportedException"> if this is called
		/// on an implementation that does not support {@code Cloneable}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override Object Clone()
		{
			if (this is Cloneable)
			{
				return base.Clone();
			}
			else
			{
				throw new CloneNotSupportedException();
			}
		}

		/*
		 * The following class allows providers to extend from SignatureSpi
		 * rather than from Signature. It represents a Signature with an
		 * encapsulated, provider-supplied SPI object (of type SignatureSpi).
		 * If the provider implementation is an instance of SignatureSpi, the
		 * getInstance() methods above return an instance of this class, with
		 * the SPI object encapsulated.
		 *
		 * Note: All SPI methods from the original Signature class have been
		 * moved up the hierarchy into a new class (SignatureSpi), which has
		 * been interposed in the hierarchy between the API (Signature)
		 * and its original parent (Object).
		 */

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private static class Delegate extends Signature
		private class Delegate : Signature
		{

			// The provider implementation (delegate)
			// filled in once the provider is selected
			internal SignatureSpi SigSpi;

			// lock for mutex during provider selection
			internal readonly Object @lock;

			// next service to try in provider selection
			// null once provider is selected
			internal Service FirstService;

			// remaining services to try in provider selection
			// null once provider is selected
			internal Iterator<Service> ServiceIterator;

			// constructor
			internal Delegate(SignatureSpi sigSpi, String algorithm) : base(algorithm)
			{
				this.SigSpi = sigSpi;
				this.@lock = null; // no lock needed
			}

			// used with delayed provider selection
			internal Delegate(Service service, Iterator<Service> iterator, String algorithm) : base(algorithm)
			{
				this.FirstService = service;
				this.ServiceIterator = iterator;
				this.@lock = new Object();
			}

			/// <summary>
			/// Returns a clone if the delegate is cloneable.
			/// </summary>
			/// <returns> a clone if the delegate is cloneable.
			/// </returns>
			/// <exception cref="CloneNotSupportedException"> if this is called on a
			/// delegate that does not support {@code Cloneable}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
			public override Object Clone()
			{
				ChooseFirstProvider();
				if (SigSpi is Cloneable)
				{
					SignatureSpi sigSpiClone = (SignatureSpi)SigSpi.Clone();
					// Because 'algorithm' and 'provider' are private
					// members of our supertype, we must perform a cast to
					// access them.
					Signature that = new Delegate(sigSpiClone, ((Signature)this).Algorithm_Renamed);
					that.Provider_Renamed = ((Signature)this).Provider_Renamed;
					return that;
				}
				else
				{
					throw new CloneNotSupportedException();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static SignatureSpi newInstance(java.security.Provider.Service s) throws NoSuchAlgorithmException
			internal static SignatureSpi NewInstance(Service s)
			{
				if (s.Type.Equals("Cipher"))
				{
					// must be NONEwithRSA
					try
					{
						Cipher c = Cipher.getInstance(RSA_CIPHER, s.Provider);
						return new CipherAdapter(c);
					}
					catch (NoSuchPaddingException e)
					{
						throw new NoSuchAlgorithmException(e);
					}
				}
				else
				{
					Object o = s.NewInstance(null);
					if (o is SignatureSpi == false)
					{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						throw new NoSuchAlgorithmException("Not a SignatureSpi: " + o.GetType().FullName);
					}
					return (SignatureSpi)o;
				}
			}

			// max number of debug warnings to print from chooseFirstProvider()
			internal static int WarnCount = 10;

			/// <summary>
			/// Choose the Spi from the first provider available. Used if
			/// delayed provider selection is not possible because initSign()/
			/// initVerify() is not the first method called.
			/// </summary>
			internal override void ChooseFirstProvider()
			{
				if (SigSpi != null)
				{
					return;
				}
				lock (@lock)
				{
					if (SigSpi != null)
					{
						return;
					}
					if (Debug != null)
					{
						int w = --WarnCount;
						if (w >= 0)
						{
							Debug.println("Signature.init() not first method " + "called, disabling delayed provider selection");
							if (w == 0)
							{
								Debug.println("Further warnings of this type will " + "be suppressed");
							}
							(new Exception("Call trace")).PrintStackTrace();
						}
					}
					Exception lastException = null;
					while ((FirstService != null) || ServiceIterator.HasNext())
					{
						Service s;
						if (FirstService != null)
						{
							s = FirstService;
							FirstService = null;
						}
						else
						{
							s = ServiceIterator.Next();
						}
						if (IsSpi(s) == false)
						{
							continue;
						}
						try
						{
							SigSpi = NewInstance(s);
							Provider_Renamed = s.Provider;
							// not needed any more
							FirstService = null;
							ServiceIterator = null;
							return;
						}
						catch (NoSuchAlgorithmException e)
						{
							lastException = e;
						}
					}
					ProviderException e = new ProviderException("Could not construct SignatureSpi instance");
					if (lastException != null)
					{
						e.InitCause(lastException);
					}
					throw e;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void chooseProvider(int type, Key key, SecureRandom random) throws InvalidKeyException
			internal virtual void ChooseProvider(int type, Key key, SecureRandom random)
			{
				lock (@lock)
				{
					if (SigSpi != null)
					{
						Init(SigSpi, type, key, random);
						return;
					}
					Exception lastException = null;
					while ((FirstService != null) || ServiceIterator.HasNext())
					{
						Service s;
						if (FirstService != null)
						{
							s = FirstService;
							FirstService = null;
						}
						else
						{
							s = ServiceIterator.Next();
						}
						// if provider says it does not support this key, ignore it
						if (s.SupportsParameter(key) == false)
						{
							continue;
						}
						// if instance is not a SignatureSpi, ignore it
						if (IsSpi(s) == false)
						{
							continue;
						}
						try
						{
							SignatureSpi spi = NewInstance(s);
							Init(spi, type, key, random);
							Provider_Renamed = s.Provider;
							SigSpi = spi;
							FirstService = null;
							ServiceIterator = null;
							return;
						}
						catch (Exception e)
						{
							// NoSuchAlgorithmException from newInstance()
							// InvalidKeyException from init()
							// RuntimeException (ProviderException) from init()
							if (lastException == null)
							{
								lastException = e;
							}
						}
					}
					// no working provider found, fail
					if (lastException is InvalidKeyException)
					{
						throw (InvalidKeyException)lastException;
					}
					if (lastException is RuntimeException)
					{
						throw (RuntimeException)lastException;
					}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					String k = (key != null) ? key.GetType().FullName : "(null)";
					throw new InvalidKeyException("No installed provider supports this key: " + k, lastException);
				}
			}

			internal const int I_PUB = 1;
			internal const int I_PRIV = 2;
			internal const int I_PRIV_SR = 3;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void init(SignatureSpi spi, int type, Key key, SecureRandom random) throws InvalidKeyException
			internal virtual void Init(SignatureSpi spi, int type, Key key, SecureRandom random)
			{
				switch (type)
				{
				case I_PUB:
					spi.EngineInitVerify((PublicKey)key);
					break;
				case I_PRIV:
					spi.EngineInitSign((PrivateKey)key);
					break;
				case I_PRIV_SR:
					spi.EngineInitSign((PrivateKey)key, random);
					break;
				default:
					throw new AssertionError("Internal error: " + type);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitVerify(PublicKey publicKey) throws InvalidKeyException
			protected internal override void EngineInitVerify(PublicKey publicKey)
			{
				if (SigSpi != null)
				{
					SigSpi.EngineInitVerify(publicKey);
				}
				else
				{
					ChooseProvider(I_PUB, publicKey, null);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitSign(PrivateKey privateKey) throws InvalidKeyException
			protected internal override void EngineInitSign(PrivateKey privateKey)
			{
				if (SigSpi != null)
				{
					SigSpi.EngineInitSign(privateKey);
				}
				else
				{
					ChooseProvider(I_PRIV, privateKey, null);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitSign(PrivateKey privateKey, SecureRandom sr) throws InvalidKeyException
			protected internal override void EngineInitSign(PrivateKey privateKey, SecureRandom sr)
			{
				if (SigSpi != null)
				{
					SigSpi.EngineInitSign(privateKey, sr);
				}
				else
				{
					ChooseProvider(I_PRIV_SR, privateKey, sr);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineUpdate(byte b) throws SignatureException
			protected internal override void EngineUpdate(sbyte b)
			{
				ChooseFirstProvider();
				SigSpi.EngineUpdate(b);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineUpdate(byte[] b, int off, int len) throws SignatureException
			protected internal override void EngineUpdate(sbyte[] b, int off, int len)
			{
				ChooseFirstProvider();
				SigSpi.EngineUpdate(b, off, len);
			}

			protected internal override void EngineUpdate(ByteBuffer data)
			{
				ChooseFirstProvider();
				SigSpi.EngineUpdate(data);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected byte[] engineSign() throws SignatureException
			protected internal override sbyte[] EngineSign()
			{
				ChooseFirstProvider();
				return SigSpi.EngineSign();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int engineSign(byte[] outbuf, int offset, int len) throws SignatureException
			protected internal override int EngineSign(sbyte[] outbuf, int offset, int len)
			{
				ChooseFirstProvider();
				return SigSpi.EngineSign(outbuf, offset, len);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean engineVerify(byte[] sigBytes) throws SignatureException
			protected internal override bool EngineVerify(sbyte[] sigBytes)
			{
				ChooseFirstProvider();
				return SigSpi.EngineVerify(sigBytes);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean engineVerify(byte[] sigBytes, int offset, int length) throws SignatureException
			protected internal override bool EngineVerify(sbyte[] sigBytes, int offset, int length)
			{
				ChooseFirstProvider();
				return SigSpi.EngineVerify(sigBytes, offset, length);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineSetParameter(String param, Object value) throws InvalidParameterException
			protected internal override void EngineSetParameter(String param, Object value)
			{
				ChooseFirstProvider();
				SigSpi.EngineSetParameter(param, value);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineSetParameter(java.security.spec.AlgorithmParameterSpec params) throws InvalidAlgorithmParameterException
			protected internal override void EngineSetParameter(AlgorithmParameterSpec @params)
			{
				ChooseFirstProvider();
				SigSpi.EngineSetParameter(@params);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object engineGetParameter(String param) throws InvalidParameterException
			protected internal override Object EngineGetParameter(String param)
			{
				ChooseFirstProvider();
				return SigSpi.EngineGetParameter(param);
			}

			protected internal override AlgorithmParameters EngineGetParameters()
			{
				ChooseFirstProvider();
				return SigSpi.EngineGetParameters();
			}
		}

		// adapter for RSA/ECB/PKCS1Padding ciphers
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private static class CipherAdapter extends SignatureSpi
		private class CipherAdapter : SignatureSpi
		{

			internal readonly Cipher Cipher;

			internal ByteArrayOutputStream Data;

			internal CipherAdapter(Cipher cipher)
			{
				this.Cipher = cipher;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitVerify(PublicKey publicKey) throws InvalidKeyException
			protected internal override void EngineInitVerify(PublicKey publicKey)
			{
				Cipher.init(Cipher.DECRYPT_MODE, publicKey);
				if (Data == null)
				{
					Data = new ByteArrayOutputStream(128);
				}
				else
				{
					Data.Reset();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitSign(PrivateKey privateKey) throws InvalidKeyException
			protected internal override void EngineInitSign(PrivateKey privateKey)
			{
				Cipher.init(Cipher.ENCRYPT_MODE, privateKey);
				Data = null;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineInitSign(PrivateKey privateKey, SecureRandom random) throws InvalidKeyException
			protected internal override void EngineInitSign(PrivateKey privateKey, SecureRandom random)
			{
				Cipher.init(Cipher.ENCRYPT_MODE, privateKey, random);
				Data = null;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineUpdate(byte b) throws SignatureException
			protected internal override void EngineUpdate(sbyte b)
			{
				EngineUpdate(new sbyte[] {b}, 0, 1);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineUpdate(byte[] b, int off, int len) throws SignatureException
			protected internal override void EngineUpdate(sbyte[] b, int off, int len)
			{
				if (Data != null)
				{
					Data.Write(b, off, len);
					return;
				}
				sbyte[] @out = Cipher.update(b, off, len);
				if ((@out != null) && (@out.Length != 0))
				{
					throw new SignatureException("Cipher unexpectedly returned data");
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected byte[] engineSign() throws SignatureException
			protected internal override sbyte[] EngineSign()
			{
				try
				{
					return Cipher.doFinal();
				}
				catch (IllegalBlockSizeException e)
				{
					throw new SignatureException("doFinal() failed", e);
				}
				catch (BadPaddingException e)
				{
					throw new SignatureException("doFinal() failed", e);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean engineVerify(byte[] sigBytes) throws SignatureException
			protected internal override bool EngineVerify(sbyte[] sigBytes)
			{
				try
				{
					sbyte[] @out = Cipher.doFinal(sigBytes);
					sbyte[] dataBytes = Data.ToByteArray();
					Data.Reset();
					return MessageDigest.IsEqual(@out, dataBytes);
				}
				catch (BadPaddingException)
				{
					// e.g. wrong public key used
					// return false rather than throwing exception
					return false;
				}
				catch (IllegalBlockSizeException e)
				{
					throw new SignatureException("doFinal() failed", e);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void engineSetParameter(String param, Object value) throws InvalidParameterException
			protected internal override void EngineSetParameter(String param, Object value)
			{
				throw new InvalidParameterException("Parameters not supported");
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object engineGetParameter(String param) throws InvalidParameterException
			protected internal override Object EngineGetParameter(String param)
			{
				throw new InvalidParameterException("Parameters not supported");
			}

		}

	}

}