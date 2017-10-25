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

	/// <summary>
	/// This class represents a storage facility for cryptographic
	/// keys and certificates.
	/// 
	/// <para> A {@code KeyStore} manages different types of entries.
	/// Each type of entry implements the {@code KeyStore.Entry} interface.
	/// Three basic {@code KeyStore.Entry} implementations are provided:
	/// 
	/// <ul>
	/// <li><b>KeyStore.PrivateKeyEntry</b>
	/// </para>
	/// <para> This type of entry holds a cryptographic {@code PrivateKey},
	/// which is optionally stored in a protected format to prevent
	/// unauthorized access.  It is also accompanied by a certificate chain
	/// for the corresponding public key.
	/// 
	/// </para>
	/// <para> Private keys and certificate chains are used by a given entity for
	/// self-authentication. Applications for this authentication include software
	/// distribution organizations which sign JAR files as part of releasing
	/// and/or licensing software.
	/// 
	/// <li><b>KeyStore.SecretKeyEntry</b>
	/// </para>
	/// <para> This type of entry holds a cryptographic {@code SecretKey},
	/// which is optionally stored in a protected format to prevent
	/// unauthorized access.
	/// 
	/// <li><b>KeyStore.TrustedCertificateEntry</b>
	/// </para>
	/// <para> This type of entry contains a single public key {@code Certificate}
	/// belonging to another party. It is called a <i>trusted certificate</i>
	/// because the keystore owner trusts that the public key in the certificate
	/// indeed belongs to the identity identified by the <i>subject</i> (owner)
	/// of the certificate.
	/// 
	/// </para>
	/// <para>This type of entry can be used to authenticate other parties.
	/// </ul>
	/// 
	/// </para>
	/// <para> Each entry in a keystore is identified by an "alias" string. In the
	/// case of private keys and their associated certificate chains, these strings
	/// distinguish among the different ways in which the entity may authenticate
	/// itself. For example, the entity may authenticate itself using different
	/// certificate authorities, or using different public key algorithms.
	/// 
	/// </para>
	/// <para> Whether aliases are case sensitive is implementation dependent. In order
	/// to avoid problems, it is recommended not to use aliases in a KeyStore that
	/// only differ in case.
	/// 
	/// </para>
	/// <para> Whether keystores are persistent, and the mechanisms used by the
	/// keystore if it is persistent, are not specified here. This allows
	/// use of a variety of techniques for protecting sensitive (e.g., private or
	/// secret) keys. Smart cards or other integrated cryptographic engines
	/// (SafeKeyper) are one option, and simpler mechanisms such as files may also
	/// be used (in a variety of formats).
	/// 
	/// </para>
	/// <para> Typical ways to request a KeyStore object include
	/// relying on the default type and providing a specific keystore type.
	/// 
	/// <ul>
	/// <li>To rely on the default type:
	/// <pre>
	///    KeyStore ks = KeyStore.getInstance(KeyStore.getDefaultType());
	/// </pre>
	/// The system will return a keystore implementation for the default type.
	/// 
	/// <li>To provide a specific keystore type:
	/// <pre>
	///      KeyStore ks = KeyStore.getInstance("JKS");
	/// </pre>
	/// The system will return the most preferred implementation of the
	/// </para>
	/// specified keystore type available in the environment. <para>
	/// </ul>
	/// 
	/// </para>
	/// <para> Before a keystore can be accessed, it must be
	/// <seealso cref="#load(java.io.InputStream, char[]) loaded"/>.
	/// <pre>
	///    KeyStore ks = KeyStore.getInstance(KeyStore.getDefaultType());
	/// 
	///    // get user password and file input stream
	///    char[] password = getPassword();
	/// 
	///    try (FileInputStream fis = new FileInputStream("keyStoreName")) {
	///        ks.load(fis, password);
	///    }
	/// </pre>
	/// 
	/// To create an empty keystore using the above {@code load} method,
	/// pass {@code null} as the {@code InputStream} argument.
	/// 
	/// </para>
	/// <para> Once the keystore has been loaded, it is possible
	/// to read existing entries from the keystore, or to write new entries
	/// into the keystore:
	/// <pre>
	///    KeyStore.ProtectionParameter protParam =
	///        new KeyStore.PasswordProtection(password);
	/// 
	///    // get my private key
	///    KeyStore.PrivateKeyEntry pkEntry = (KeyStore.PrivateKeyEntry)
	///        ks.getEntry("privateKeyAlias", protParam);
	///    PrivateKey myPrivateKey = pkEntry.getPrivateKey();
	/// 
	///    // save my secret key
	///    javax.crypto.SecretKey mySecretKey;
	///    KeyStore.SecretKeyEntry skEntry =
	///        new KeyStore.SecretKeyEntry(mySecretKey);
	///    ks.setEntry("secretKeyAlias", skEntry, protParam);
	/// 
	///    // store away the keystore
	///    try (FileOutputStream fos = new FileOutputStream("newKeyStoreName")) {
	///        ks.store(fos, password);
	///    }
	/// </pre>
	/// 
	/// Note that although the same password may be used to
	/// load the keystore, to protect the private key entry,
	/// to protect the secret key entry, and to store the keystore
	/// (as is shown in the sample code above),
	/// different passwords or other protection parameters
	/// may also be used.
	/// 
	/// </para>
	/// <para> Every implementation of the Java platform is required to support
	/// the following standard {@code KeyStore} type:
	/// <ul>
	/// <li>{@code PKCS12}</li>
	/// </ul>
	/// This type is described in the <a href=
	/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
	/// KeyStore section</a> of the
	/// Java Cryptography Architecture Standard Algorithm Name Documentation.
	/// Consult the release documentation for your implementation to see if any
	/// other types are supported.
	/// 
	/// @author Jan Luehe
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.PrivateKey </seealso>
	/// <seealso cref= javax.crypto.SecretKey </seealso>
	/// <seealso cref= java.security.cert.Certificate
	/// 
	/// @since 1.2 </seealso>

	public class KeyStore
	{

		private static readonly Debug Pdebug = Debug.getInstance("provider", "Provider");
		private static readonly bool SkipDebug = Debug.isOn("engine=") && !Debug.isOn("keystore");

		/*
		 * Constant to lookup in the Security properties file to determine
		 * the default keystore type.
		 * In the Security properties file, the default keystore type is given as:
		 * <pre>
		 * keystore.type=jks
		 * </pre>
		 */
		private const String KEYSTORE_TYPE = "keystore.type";

		// The keystore type
		private String Type_Renamed;

		// The provider
		private Provider Provider_Renamed;

		// The provider implementation
		private KeyStoreSpi KeyStoreSpi;

		// Has this keystore been initialized (loaded)?
		private bool Initialized = false;

		/// <summary>
		/// A marker interface for {@code KeyStore}
		/// <seealso cref="#load(KeyStore.LoadStoreParameter) load"/>
		/// and
		/// <seealso cref="#store(KeyStore.LoadStoreParameter) store"/>
		/// parameters.
		/// 
		/// @since 1.5
		/// </summary>
		public interface LoadStoreParameter
		{
			/// <summary>
			/// Gets the parameter used to protect keystore data.
			/// </summary>
			/// <returns> the parameter used to protect keystore data, or null </returns>
			ProtectionParameter ProtectionParameter {get;}
		}

		/// <summary>
		/// A marker interface for keystore protection parameters.
		/// 
		/// <para> The information stored in a {@code ProtectionParameter}
		/// object protects the contents of a keystore.
		/// For example, protection parameters may be used to check
		/// the integrity of keystore data, or to protect the
		/// confidentiality of sensitive keystore data
		/// (such as a {@code PrivateKey}).
		/// 
		/// @since 1.5
		/// </para>
		/// </summary>
		public interface ProtectionParameter
		{
		}

		/// <summary>
		/// A password-based implementation of {@code ProtectionParameter}.
		/// 
		/// @since 1.5
		/// </summary>
		public class PasswordProtection : ProtectionParameter, javax.security.auth.Destroyable
		{

			internal readonly char[] Password_Renamed;
			internal readonly String ProtectionAlgorithm_Renamed;
			internal readonly AlgorithmParameterSpec ProtectionParameters_Renamed;
			internal volatile bool Destroyed_Renamed = false;

			/// <summary>
			/// Creates a password parameter.
			/// 
			/// <para> The specified {@code password} is cloned before it is stored
			/// in the new {@code PasswordProtection} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="password"> the password, which may be {@code null} </param>
			public PasswordProtection(char[] password)
			{
				this.Password_Renamed = (password == null) ? null : password.clone();
				this.ProtectionAlgorithm_Renamed = null;
				this.ProtectionParameters_Renamed = null;
			}

			/// <summary>
			/// Creates a password parameter and specifies the protection algorithm
			/// and associated parameters to use when encrypting a keystore entry.
			/// <para>
			/// The specified {@code password} is cloned before it is stored in the
			/// new {@code PasswordProtection} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="password"> the password, which may be {@code null} </param>
			/// <param name="protectionAlgorithm"> the encryption algorithm name, for
			///     example, {@code PBEWithHmacSHA256AndAES_256}.
			///     See the Cipher section in the <a href=
			/// "{@docRoot}/../technotes/guides/security/StandardNames.html#Cipher">
			/// Java Cryptography Architecture Standard Algorithm Name
			/// Documentation</a>
			///     for information about standard encryption algorithm names. </param>
			/// <param name="protectionParameters"> the encryption algorithm parameter
			///     specification, which may be {@code null} </param>
			/// <exception cref="NullPointerException"> if {@code protectionAlgorithm} is
			///     {@code null}
			/// 
			/// @since 1.8 </exception>
			public PasswordProtection(char[] password, String protectionAlgorithm, AlgorithmParameterSpec protectionParameters)
			{
				if (protectionAlgorithm == null)
				{
					throw new NullPointerException("invalid null input");
				}
				this.Password_Renamed = (password == null) ? null : password.clone();
				this.ProtectionAlgorithm_Renamed = protectionAlgorithm;
				this.ProtectionParameters_Renamed = protectionParameters;
			}

			/// <summary>
			/// Gets the name of the protection algorithm.
			/// If none was set then the keystore provider will use its default
			/// protection algorithm. The name of the default protection algorithm
			/// for a given keystore type is set using the
			/// {@code 'keystore.<type>.keyProtectionAlgorithm'} security property.
			/// For example, the
			/// {@code keystore.PKCS12.keyProtectionAlgorithm} property stores the
			/// name of the default key protection algorithm used for PKCS12
			/// keystores. If the security property is not set, an
			/// implementation-specific algorithm will be used.
			/// </summary>
			/// <returns> the algorithm name, or {@code null} if none was set
			/// 
			/// @since 1.8 </returns>
			public virtual String ProtectionAlgorithm
			{
				get
				{
					return ProtectionAlgorithm_Renamed;
				}
			}

			/// <summary>
			/// Gets the parameters supplied for the protection algorithm.
			/// </summary>
			/// <returns> the algorithm parameter specification, or {@code  null},
			///     if none was set
			/// 
			/// @since 1.8 </returns>
			public virtual AlgorithmParameterSpec ProtectionParameters
			{
				get
				{
					return ProtectionParameters_Renamed;
				}
			}

			/// <summary>
			/// Gets the password.
			/// 
			/// <para>Note that this method returns a reference to the password.
			/// If a clone of the array is created it is the caller's
			/// responsibility to zero out the password information
			/// after it is no longer needed.
			/// 
			/// </para>
			/// </summary>
			/// <seealso cref= #destroy() </seealso>
			/// <returns> the password, which may be {@code null} </returns>
			/// <exception cref="IllegalStateException"> if the password has
			///              been cleared (destroyed) </exception>
			public virtual char[] Password
			{
				get
				{
					lock (this)
					{
						if (Destroyed_Renamed)
						{
							throw new IllegalStateException("password has been cleared");
						}
						return Password_Renamed;
					}
				}
			}

			/// <summary>
			/// Clears the password.
			/// </summary>
			/// <exception cref="DestroyFailedException"> if this method was unable
			///      to clear the password </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void destroy() throws javax.security.auth.DestroyFailedException
			public virtual void Destroy()
			{
				lock (this)
				{
					Destroyed_Renamed = true;
					if (Password_Renamed != null)
					{
						Arrays.Fill(Password_Renamed, ' ');
					}
				}
			}

			/// <summary>
			/// Determines if password has been cleared.
			/// </summary>
			/// <returns> true if the password has been cleared, false otherwise </returns>
			public virtual bool Destroyed
			{
				get
				{
					lock (this)
					{
						return Destroyed_Renamed;
					}
				}
			}
		}

		/// <summary>
		/// A ProtectionParameter encapsulating a CallbackHandler.
		/// 
		/// @since 1.5
		/// </summary>
		public class CallbackHandlerProtection : ProtectionParameter
		{

			internal readonly CallbackHandler Handler;

			/// <summary>
			/// Constructs a new CallbackHandlerProtection from a
			/// CallbackHandler.
			/// </summary>
			/// <param name="handler"> the CallbackHandler </param>
			/// <exception cref="NullPointerException"> if handler is null </exception>
			public CallbackHandlerProtection(CallbackHandler handler)
			{
				if (handler == null)
				{
					throw new NullPointerException("handler must not be null");
				}
				this.Handler = handler;
			}

			/// <summary>
			/// Returns the CallbackHandler.
			/// </summary>
			/// <returns> the CallbackHandler. </returns>
			public virtual CallbackHandler CallbackHandler
			{
				get
				{
					return Handler;
				}
			}

		}

		/// <summary>
		/// A marker interface for {@code KeyStore} entry types.
		/// 
		/// @since 1.5
		/// </summary>
		public interface Entry
		{

			/// <summary>
			/// Retrieves the attributes associated with an entry.
			/// <para>
			/// The default implementation returns an empty {@code Set}.
			/// 
			/// </para>
			/// </summary>
			/// <returns> an unmodifiable {@code Set} of attributes, possibly empty
			/// 
			/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//			public default Set<Entry_Attribute> getAttributes()
	//		{
	//			return Collections.emptySet<Attribute>();
	//		}

			/// <summary>
			/// An attribute associated with a keystore entry.
			/// It comprises a name and one or more values.
			/// 
			/// @since 1.8
			/// </summary>
		}

		public interface Entry_Attribute
		{
			/// <summary>
			/// Returns the attribute's name.
			/// </summary>
			/// <returns> the attribute name </returns>
			String Name {get;}

			/// <summary>
			/// Returns the attribute's value.
			/// Multi-valued attributes encode their values as a single string.
			/// </summary>
			/// <returns> the attribute value </returns>
			String Value {get;}
		}

		/// <summary>
		/// A {@code KeyStore} entry that holds a {@code PrivateKey}
		/// and corresponding certificate chain.
		/// 
		/// @since 1.5
		/// </summary>
		public sealed class PrivateKeyEntry : Entry
		{

			internal readonly PrivateKey PrivKey;
			internal readonly Certificate[] Chain;
			internal readonly Set<KeyStore.Entry_Attribute> Attributes_Renamed;

			/// <summary>
			/// Constructs a {@code PrivateKeyEntry} with a
			/// {@code PrivateKey} and corresponding certificate chain.
			/// 
			/// <para> The specified {@code chain} is cloned before it is stored
			/// in the new {@code PrivateKeyEntry} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="privateKey"> the {@code PrivateKey} </param>
			/// <param name="chain"> an array of {@code Certificate}s
			///      representing the certificate chain.
			///      The chain must be ordered and contain a
			///      {@code Certificate} at index 0
			///      corresponding to the private key.
			/// </param>
			/// <exception cref="NullPointerException"> if
			///      {@code privateKey} or {@code chain}
			///      is {@code null} </exception>
			/// <exception cref="IllegalArgumentException"> if the specified chain has a
			///      length of 0, if the specified chain does not contain
			///      {@code Certificate}s of the same type,
			///      or if the {@code PrivateKey} algorithm
			///      does not match the algorithm of the {@code PublicKey}
			///      in the end entity {@code Certificate} (at index 0) </exception>
			public PrivateKeyEntry(PrivateKey privateKey, Certificate[] chain) : this(privateKey, chain, System.Linq.Enumerable.Empty<KeyStore.Entry_Attribute>())
			{
			}

			/// <summary>
			/// Constructs a {@code PrivateKeyEntry} with a {@code PrivateKey} and
			/// corresponding certificate chain and associated entry attributes.
			/// 
			/// <para> The specified {@code chain} and {@code attributes} are cloned
			/// before they are stored in the new {@code PrivateKeyEntry} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="privateKey"> the {@code PrivateKey} </param>
			/// <param name="chain"> an array of {@code Certificate}s
			///      representing the certificate chain.
			///      The chain must be ordered and contain a
			///      {@code Certificate} at index 0
			///      corresponding to the private key. </param>
			/// <param name="attributes"> the attributes
			/// </param>
			/// <exception cref="NullPointerException"> if {@code privateKey}, {@code chain}
			///      or {@code attributes} is {@code null} </exception>
			/// <exception cref="IllegalArgumentException"> if the specified chain has a
			///      length of 0, if the specified chain does not contain
			///      {@code Certificate}s of the same type,
			///      or if the {@code PrivateKey} algorithm
			///      does not match the algorithm of the {@code PublicKey}
			///      in the end entity {@code Certificate} (at index 0)
			/// 
			/// @since 1.8 </exception>
			public PrivateKeyEntry(PrivateKey privateKey, Certificate[] chain, Set<KeyStore.Entry_Attribute> attributes)
			{

				if (privateKey == null || chain == null || attributes == null)
				{
					throw new NullPointerException("invalid null input");
				}
				if (chain.Length == 0)
				{
					throw new IllegalArgumentException("invalid zero-length input chain");
				}

				Certificate[] clonedChain = chain.clone();
				String certType = clonedChain[0].Type;
				for (int i = 1; i < clonedChain.Length; i++)
				{
					if (!certType.Equals(clonedChain[i].Type))
					{
						throw new IllegalArgumentException("chain does not contain certificates " + "of the same type");
					}
				}
				if (!privateKey.Algorithm.Equals(clonedChain[0].PublicKey.Algorithm))
				{
					throw new IllegalArgumentException("private key algorithm does not match " + "algorithm of public key in end entity " + "certificate (at index 0)");
				}
				this.PrivKey = privateKey;

				if (clonedChain[0] is X509Certificate && !(clonedChain is X509Certificate[]))
				{

					this.Chain = new X509Certificate[clonedChain.Length];
					System.Array.Copy(clonedChain, 0, this.Chain, 0, clonedChain.Length);
				}
				else
				{
					this.Chain = clonedChain;
				}

				this.Attributes_Renamed = Collections.UnmodifiableSet(new HashSet<>(attributes));
			}

			/// <summary>
			/// Gets the {@code PrivateKey} from this entry.
			/// </summary>
			/// <returns> the {@code PrivateKey} from this entry </returns>
			public PrivateKey PrivateKey
			{
				get
				{
					return PrivKey;
				}
			}

			/// <summary>
			/// Gets the {@code Certificate} chain from this entry.
			/// 
			/// <para> The stored chain is cloned before being returned.
			/// 
			/// </para>
			/// </summary>
			/// <returns> an array of {@code Certificate}s corresponding
			///      to the certificate chain for the public key.
			///      If the certificates are of type X.509,
			///      the runtime type of the returned array is
			///      {@code X509Certificate[]}. </returns>
			public Certificate[] CertificateChain
			{
				get
				{
					return Chain.clone();
				}
			}

			/// <summary>
			/// Gets the end entity {@code Certificate}
			/// from the certificate chain in this entry.
			/// </summary>
			/// <returns> the end entity {@code Certificate} (at index 0)
			///      from the certificate chain in this entry.
			///      If the certificate is of type X.509,
			///      the runtime type of the returned certificate is
			///      {@code X509Certificate}. </returns>
			public Certificate Certificate
			{
				get
				{
					return Chain[0];
				}
			}

			/// <summary>
			/// Retrieves the attributes associated with an entry.
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <returns> an unmodifiable {@code Set} of attributes, possibly empty
			/// 
			/// @since 1.8 </returns>
			public override Set<KeyStore.Entry_Attribute> Attributes
			{
				get
				{
					return Attributes_Renamed;
				}
			}

			/// <summary>
			/// Returns a string representation of this PrivateKeyEntry. </summary>
			/// <returns> a string representation of this PrivateKeyEntry. </returns>
			public override String ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Private key entry and certificate chain with " + Chain.Length + " elements:\r\n");
				foreach (Certificate cert in Chain)
				{
					sb.Append(cert);
					sb.Append("\r\n");
				}
				return sb.ToString();
			}

		}

		/// <summary>
		/// A {@code KeyStore} entry that holds a {@code SecretKey}.
		/// 
		/// @since 1.5
		/// </summary>
		public sealed class SecretKeyEntry : Entry
		{

			internal readonly SecretKey SKey;
			internal readonly Set<KeyStore.Entry_Attribute> Attributes_Renamed;

			/// <summary>
			/// Constructs a {@code SecretKeyEntry} with a
			/// {@code SecretKey}.
			/// </summary>
			/// <param name="secretKey"> the {@code SecretKey}
			/// </param>
			/// <exception cref="NullPointerException"> if {@code secretKey}
			///      is {@code null} </exception>
			public SecretKeyEntry(SecretKey secretKey)
			{
				if (secretKey == null)
				{
					throw new NullPointerException("invalid null input");
				}
				this.SKey = secretKey;
				this.Attributes_Renamed = System.Linq.Enumerable.Empty<KeyStore.Entry_Attribute>();
			}

			/// <summary>
			/// Constructs a {@code SecretKeyEntry} with a {@code SecretKey} and
			/// associated entry attributes.
			/// 
			/// <para> The specified {@code attributes} is cloned before it is stored
			/// in the new {@code SecretKeyEntry} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="secretKey"> the {@code SecretKey} </param>
			/// <param name="attributes"> the attributes
			/// </param>
			/// <exception cref="NullPointerException"> if {@code secretKey} or
			///     {@code attributes} is {@code null}
			/// 
			/// @since 1.8 </exception>
			public SecretKeyEntry(SecretKey secretKey, Set<KeyStore.Entry_Attribute> attributes)
			{

				if (secretKey == null || attributes == null)
				{
					throw new NullPointerException("invalid null input");
				}
				this.SKey = secretKey;
				this.Attributes_Renamed = Collections.UnmodifiableSet(new HashSet<>(attributes));
			}

			/// <summary>
			/// Gets the {@code SecretKey} from this entry.
			/// </summary>
			/// <returns> the {@code SecretKey} from this entry </returns>
			public SecretKey SecretKey
			{
				get
				{
					return SKey;
				}
			}

			/// <summary>
			/// Retrieves the attributes associated with an entry.
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <returns> an unmodifiable {@code Set} of attributes, possibly empty
			/// 
			/// @since 1.8 </returns>
			public override Set<KeyStore.Entry_Attribute> Attributes
			{
				get
				{
					return Attributes_Renamed;
				}
			}

			/// <summary>
			/// Returns a string representation of this SecretKeyEntry. </summary>
			/// <returns> a string representation of this SecretKeyEntry. </returns>
			public override String ToString()
			{
				return "Secret key entry with algorithm " + SKey.Algorithm;
			}
		}

		/// <summary>
		/// A {@code KeyStore} entry that holds a trusted
		/// {@code Certificate}.
		/// 
		/// @since 1.5
		/// </summary>
		public sealed class TrustedCertificateEntry : Entry
		{

			internal readonly Certificate Cert;
			internal readonly Set<KeyStore.Entry_Attribute> Attributes_Renamed;

			/// <summary>
			/// Constructs a {@code TrustedCertificateEntry} with a
			/// trusted {@code Certificate}.
			/// </summary>
			/// <param name="trustedCert"> the trusted {@code Certificate}
			/// </param>
			/// <exception cref="NullPointerException"> if
			///      {@code trustedCert} is {@code null} </exception>
			public TrustedCertificateEntry(Certificate trustedCert)
			{
				if (trustedCert == null)
				{
					throw new NullPointerException("invalid null input");
				}
				this.Cert = trustedCert;
				this.Attributes_Renamed = System.Linq.Enumerable.Empty<KeyStore.Entry_Attribute>();
			}

			/// <summary>
			/// Constructs a {@code TrustedCertificateEntry} with a
			/// trusted {@code Certificate} and associated entry attributes.
			/// 
			/// <para> The specified {@code attributes} is cloned before it is stored
			/// in the new {@code TrustedCertificateEntry} object.
			/// 
			/// </para>
			/// </summary>
			/// <param name="trustedCert"> the trusted {@code Certificate} </param>
			/// <param name="attributes"> the attributes
			/// </param>
			/// <exception cref="NullPointerException"> if {@code trustedCert} or
			///     {@code attributes} is {@code null}
			/// 
			/// @since 1.8 </exception>
			public TrustedCertificateEntry(Certificate trustedCert, Set<KeyStore.Entry_Attribute> attributes)
			{
				if (trustedCert == null || attributes == null)
				{
					throw new NullPointerException("invalid null input");
				}
				this.Cert = trustedCert;
				this.Attributes_Renamed = Collections.UnmodifiableSet(new HashSet<>(attributes));
			}

			/// <summary>
			/// Gets the trusted {@code Certficate} from this entry.
			/// </summary>
			/// <returns> the trusted {@code Certificate} from this entry </returns>
			public Certificate TrustedCertificate
			{
				get
				{
					return Cert;
				}
			}

			/// <summary>
			/// Retrieves the attributes associated with an entry.
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <returns> an unmodifiable {@code Set} of attributes, possibly empty
			/// 
			/// @since 1.8 </returns>
			public override Set<KeyStore.Entry_Attribute> Attributes
			{
				get
				{
					return Attributes_Renamed;
				}
			}

			/// <summary>
			/// Returns a string representation of this TrustedCertificateEntry. </summary>
			/// <returns> a string representation of this TrustedCertificateEntry. </returns>
			public override String ToString()
			{
				return "Trusted certificate entry:\r\n" + Cert.ToString();
			}
		}

		/// <summary>
		/// Creates a KeyStore object of the given type, and encapsulates the given
		/// provider implementation (SPI object) in it.
		/// </summary>
		/// <param name="keyStoreSpi"> the provider implementation. </param>
		/// <param name="provider"> the provider. </param>
		/// <param name="type"> the keystore type. </param>
		protected internal KeyStore(KeyStoreSpi keyStoreSpi, Provider provider, String type)
		{
			this.KeyStoreSpi = keyStoreSpi;
			this.Provider_Renamed = provider;
			this.Type_Renamed = type;

			if (!SkipDebug && Pdebug != null)
			{
				Pdebug.println("KeyStore." + type.ToUpperCase() + " type from: " + this.Provider_Renamed.Name);
			}
		}

		/// <summary>
		/// Returns a keystore object of the specified type.
		/// 
		/// <para> This method traverses the list of registered security Providers,
		/// starting with the most preferred Provider.
		/// A new KeyStore object encapsulating the
		/// KeyStoreSpi implementation from the first
		/// Provider that supports the specified type is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type of keystore.
		/// See the KeyStore section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard keystore types.
		/// </param>
		/// <returns> a keystore object of the specified type.
		/// </returns>
		/// <exception cref="KeyStoreException"> if no Provider supports a
		///          KeyStoreSpi implementation for the
		///          specified type.
		/// </exception>
		/// <seealso cref= Provider </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyStore getInstance(String type) throws KeyStoreException
		public static KeyStore GetInstance(String type)
		{
			try
			{
				Object[] objs = Security.GetImpl(type, "KeyStore", (String)null);
				return new KeyStore((KeyStoreSpi)objs[0], (Provider)objs[1], type);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				throw new KeyStoreException(type + " not found", nsae);
			}
			catch (NoSuchProviderException nspe)
			{
				throw new KeyStoreException(type + " not found", nspe);
			}
		}

		/// <summary>
		/// Returns a keystore object of the specified type.
		/// 
		/// <para> A new KeyStore object encapsulating the
		/// KeyStoreSpi implementation from the specified provider
		/// is returned.  The specified provider must be registered
		/// in the security provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type of keystore.
		/// See the KeyStore section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard keystore types.
		/// </param>
		/// <param name="provider"> the name of the provider.
		/// </param>
		/// <returns> a keystore object of the specified type.
		/// </returns>
		/// <exception cref="KeyStoreException"> if a KeyStoreSpi
		///          implementation for the specified type is not
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
//ORIGINAL LINE: public static KeyStore getInstance(String type, String provider) throws KeyStoreException, NoSuchProviderException
		public static KeyStore GetInstance(String type, String provider)
		{
			if (provider == null || provider.Length() == 0)
			{
				throw new IllegalArgumentException("missing provider");
			}
			try
			{
				Object[] objs = Security.GetImpl(type, "KeyStore", provider);
				return new KeyStore((KeyStoreSpi)objs[0], (Provider)objs[1], type);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				throw new KeyStoreException(type + " not found", nsae);
			}
		}

		/// <summary>
		/// Returns a keystore object of the specified type.
		/// 
		/// <para> A new KeyStore object encapsulating the
		/// KeyStoreSpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type of keystore.
		/// See the KeyStore section in the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard keystore types.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> a keystore object of the specified type.
		/// </returns>
		/// <exception cref="KeyStoreException"> if KeyStoreSpi
		///          implementation for the specified type is not available
		///          from the specified Provider object.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider is null.
		/// </exception>
		/// <seealso cref= Provider
		/// 
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static KeyStore getInstance(String type, Provider provider) throws KeyStoreException
		public static KeyStore GetInstance(String type, Provider provider)
		{
			if (provider == null)
			{
				throw new IllegalArgumentException("missing provider");
			}
			try
			{
				Object[] objs = Security.GetImpl(type, "KeyStore", provider);
				return new KeyStore((KeyStoreSpi)objs[0], (Provider)objs[1], type);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				throw new KeyStoreException(type + " not found", nsae);
			}
		}

		/// <summary>
		/// Returns the default keystore type as specified by the
		/// {@code keystore.type} security property, or the string
		/// {@literal "jks"} (acronym for {@literal "Java keystore"})
		/// if no such property exists.
		/// 
		/// <para>The default keystore type can be used by applications that do not
		/// want to use a hard-coded keystore type when calling one of the
		/// {@code getInstance} methods, and want to provide a default keystore
		/// type in case a user does not specify its own.
		/// 
		/// </para>
		/// <para>The default keystore type can be changed by setting the value of the
		/// {@code keystore.type} security property to the desired keystore type.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the default keystore type as specified by the
		/// {@code keystore.type} security property, or the string {@literal "jks"}
		/// if no such property exists. </returns>
		/// <seealso cref= java.security.Security security properties </seealso>
		public static String DefaultType
		{
			get
			{
				String kstype;
				kstype = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
				if (kstype == null)
				{
					kstype = "jks";
				}
				return kstype;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty(KEYSTORE_TYPE);
			}
		}

		/// <summary>
		/// Returns the provider of this keystore.
		/// </summary>
		/// <returns> the provider of this keystore. </returns>
		public Provider Provider
		{
			get
			{
				return this.Provider_Renamed;
			}
		}

		/// <summary>
		/// Returns the type of this keystore.
		/// </summary>
		/// <returns> the type of this keystore. </returns>
		public String Type
		{
			get
			{
				return this.Type_Renamed;
			}
		}

		/// <summary>
		/// Returns the key associated with the given alias, using the given
		/// password to recover it.  The key must have been associated with
		/// the alias by a call to {@code setKeyEntry},
		/// or by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry} or {@code SecretKeyEntry}.
		/// </summary>
		/// <param name="alias"> the alias name </param>
		/// <param name="password"> the password for recovering the key
		/// </param>
		/// <returns> the requested key, or null if the given alias does not exist
		/// or does not identify a key-related entry.
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		/// key cannot be found </exception>
		/// <exception cref="UnrecoverableKeyException"> if the key cannot be recovered
		/// (e.g., the given password is wrong). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Key getKey(String alias, char[] password) throws KeyStoreException, NoSuchAlgorithmException, UnrecoverableKeyException
		public Key GetKey(String alias, char[] password)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetKey(alias, password);
		}

		/// <summary>
		/// Returns the certificate chain associated with the given alias.
		/// The certificate chain must have been associated with the alias
		/// by a call to {@code setKeyEntry},
		/// or by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry}.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> the certificate chain (ordered with the user's certificate first
		/// followed by zero or more certificate authorities), or null if the given alias
		/// does not exist or does not contain a certificate chain
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.security.cert.Certificate[] getCertificateChain(String alias) throws KeyStoreException
		public Certificate[] GetCertificateChain(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetCertificateChain(alias);
		}

		/// <summary>
		/// Returns the certificate associated with the given alias.
		/// 
		/// <para> If the given alias name identifies an entry
		/// created by a call to {@code setCertificateEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code TrustedCertificateEntry},
		/// then the trusted certificate contained in that entry is returned.
		/// 
		/// </para>
		/// <para> If the given alias name identifies an entry
		/// created by a call to {@code setKeyEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry},
		/// then the first element of the certificate chain in that entry
		/// is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> the certificate, or null if the given alias does not exist or
		/// does not contain a certificate.
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.security.cert.Certificate getCertificate(String alias) throws KeyStoreException
		public Certificate GetCertificate(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetCertificate(alias);
		}

		/// <summary>
		/// Returns the creation date of the entry identified by the given alias.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> the creation date of this entry, or null if the given alias does
		/// not exist
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Date getCreationDate(String alias) throws KeyStoreException
		public Date GetCreationDate(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetCreationDate(alias);
		}

		/// <summary>
		/// Assigns the given key to the given alias, protecting it with the given
		/// password.
		/// 
		/// <para>If the given key is of type {@code java.security.PrivateKey},
		/// it must be accompanied by a certificate chain certifying the
		/// corresponding public key.
		/// 
		/// </para>
		/// <para>If the given alias already exists, the keystore information
		/// associated with it is overridden by the given key (and possibly
		/// certificate chain).
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> the alias name </param>
		/// <param name="key"> the key to be associated with the alias </param>
		/// <param name="password"> the password to protect the key </param>
		/// <param name="chain"> the certificate chain for the corresponding public
		/// key (only required if the given key is of type
		/// {@code java.security.PrivateKey}).
		/// </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded), the given key cannot be protected, or this operation fails
		/// for some other reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setKeyEntry(String alias, Key key, char[] password, java.security.cert.Certificate[] chain) throws KeyStoreException
		public void SetKeyEntry(String alias, Key key, char[] password, Certificate[] chain)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			if ((key is PrivateKey) && (chain == null || chain.Length == 0))
			{
				throw new IllegalArgumentException("Private key must be " + "accompanied by certificate " + "chain");
			}
			KeyStoreSpi.EngineSetKeyEntry(alias, key, password, chain);
		}

		/// <summary>
		/// Assigns the given key (that has already been protected) to the given
		/// alias.
		/// 
		/// <para>If the protected key is of type
		/// {@code java.security.PrivateKey}, it must be accompanied by a
		/// certificate chain certifying the corresponding public key. If the
		/// underlying keystore implementation is of type {@code jks},
		/// {@code key} must be encoded as an
		/// {@code EncryptedPrivateKeyInfo} as defined in the PKCS #8 standard.
		/// 
		/// </para>
		/// <para>If the given alias already exists, the keystore information
		/// associated with it is overridden by the given key (and possibly
		/// certificate chain).
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> the alias name </param>
		/// <param name="key"> the key (in protected format) to be associated with the alias </param>
		/// <param name="chain"> the certificate chain for the corresponding public
		///          key (only useful if the protected key is of type
		///          {@code java.security.PrivateKey}).
		/// </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded), or if this operation fails for some other reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setKeyEntry(String alias, byte[] key, java.security.cert.Certificate[] chain) throws KeyStoreException
		public void SetKeyEntry(String alias, sbyte[] key, Certificate[] chain)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineSetKeyEntry(alias, key, chain);
		}

		/// <summary>
		/// Assigns the given trusted certificate to the given alias.
		/// 
		/// <para> If the given alias identifies an existing entry
		/// created by a call to {@code setCertificateEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code TrustedCertificateEntry},
		/// the trusted certificate in the existing entry
		/// is overridden by the given certificate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> the alias name </param>
		/// <param name="cert"> the certificate
		/// </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized,
		/// or the given alias already exists and does not identify an
		/// entry containing a trusted certificate,
		/// or this operation fails for some other reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setCertificateEntry(String alias, java.security.cert.Certificate cert) throws KeyStoreException
		public void SetCertificateEntry(String alias, Certificate cert)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineSetCertificateEntry(alias, cert);
		}

		/// <summary>
		/// Deletes the entry identified by the given alias from this keystore.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized,
		/// or if the entry cannot be removed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void deleteEntry(String alias) throws KeyStoreException
		public void DeleteEntry(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineDeleteEntry(alias);
		}

		/// <summary>
		/// Lists all the alias names of this keystore.
		/// </summary>
		/// <returns> enumeration of the alias names
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final java.util.Iterator<String> aliases() throws KeyStoreException
		public IEnumerator<String> Aliases()
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineAliases();
		}

		/// <summary>
		/// Checks if the given alias exists in this keystore.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> true if the alias exists, false otherwise
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean containsAlias(String alias) throws KeyStoreException
		public bool ContainsAlias(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineContainsAlias(alias);
		}

		/// <summary>
		/// Retrieves the number of entries in this keystore.
		/// </summary>
		/// <returns> the number of entries in this keystore
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final int size() throws KeyStoreException
		public int Size()
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineSize();
		}

		/// <summary>
		/// Returns true if the entry identified by the given alias
		/// was created by a call to {@code setKeyEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry} or a {@code SecretKeyEntry}.
		/// </summary>
		/// <param name="alias"> the alias for the keystore entry to be checked
		/// </param>
		/// <returns> true if the entry identified by the given alias is a
		/// key-related entry, false otherwise.
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean isKeyEntry(String alias) throws KeyStoreException
		public bool IsKeyEntry(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineIsKeyEntry(alias);
		}

		/// <summary>
		/// Returns true if the entry identified by the given alias
		/// was created by a call to {@code setCertificateEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code TrustedCertificateEntry}.
		/// </summary>
		/// <param name="alias"> the alias for the keystore entry to be checked
		/// </param>
		/// <returns> true if the entry identified by the given alias contains a
		/// trusted certificate, false otherwise.
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean isCertificateEntry(String alias) throws KeyStoreException
		public bool IsCertificateEntry(String alias)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineIsCertificateEntry(alias);
		}

		/// <summary>
		/// Returns the (alias) name of the first keystore entry whose certificate
		/// matches the given certificate.
		/// 
		/// <para> This method attempts to match the given certificate with each
		/// keystore entry. If the entry being considered was
		/// created by a call to {@code setCertificateEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code TrustedCertificateEntry},
		/// then the given certificate is compared to that entry's certificate.
		/// 
		/// </para>
		/// <para> If the entry being considered was
		/// created by a call to {@code setKeyEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry},
		/// then the given certificate is compared to the first
		/// element of that entry's certificate chain.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cert"> the certificate to match with.
		/// </param>
		/// <returns> the alias name of the first entry with a matching certificate,
		/// or null if no such entry exists in this keystore.
		/// </returns>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String getCertificateAlias(java.security.cert.Certificate cert) throws KeyStoreException
		public String GetCertificateAlias(Certificate cert)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetCertificateAlias(cert);
		}

		/// <summary>
		/// Stores this keystore to the given output stream, and protects its
		/// integrity with the given password.
		/// </summary>
		/// <param name="stream"> the output stream to which this keystore is written. </param>
		/// <param name="password"> the password to generate the keystore integrity check
		/// </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		/// (loaded). </exception>
		/// <exception cref="IOException"> if there was an I/O problem with data </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		/// algorithm could not be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates included in
		/// the keystore data could not be stored </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void store(OutputStream stream, char[] password) throws KeyStoreException, IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public void Store(OutputStream stream, char[] password)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineStore(stream, password);
		}

		/// <summary>
		/// Stores this keystore using the given {@code LoadStoreParameter}.
		/// </summary>
		/// <param name="param"> the {@code LoadStoreParameter}
		///          that specifies how to store the keystore,
		///          which may be {@code null}
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the given
		///          {@code LoadStoreParameter}
		///          input is not recognized </exception>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		///          (loaded) </exception>
		/// <exception cref="IOException"> if there was an I/O problem with data </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		///          algorithm could not be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates included in
		///          the keystore data could not be stored
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void store(LoadStoreParameter param) throws KeyStoreException, IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public void Store(LoadStoreParameter param)
		{
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineStore(param);
		}

		/// <summary>
		/// Loads this KeyStore from the given input stream.
		/// 
		/// <para>A password may be given to unlock the keystore
		/// (e.g. the keystore resides on a hardware token device),
		/// or to check the integrity of the keystore data.
		/// If a password is not given for integrity checking,
		/// then integrity checking is not performed.
		/// 
		/// </para>
		/// <para>In order to create an empty keystore, or if the keystore cannot
		/// be initialized from a stream, pass {@code null}
		/// as the {@code stream} argument.
		/// 
		/// </para>
		/// <para> Note that if this keystore has already been loaded, it is
		/// reinitialized and loaded again from the given input stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="stream"> the input stream from which the keystore is loaded,
		/// or {@code null} </param>
		/// <param name="password"> the password used to check the integrity of
		/// the keystore, the password used to unlock the keystore,
		/// or {@code null}
		/// </param>
		/// <exception cref="IOException"> if there is an I/O or format problem with the
		/// keystore data, if a password is required but not given,
		/// or if the given password was incorrect. If the error is due to a
		/// wrong password, the <seealso cref="Throwable#getCause cause"/> of the
		/// {@code IOException} should be an
		/// {@code UnrecoverableKeyException} </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		/// the integrity of the keystore cannot be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates in the
		/// keystore could not be loaded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void load(InputStream stream, char[] password) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public void Load(InputStream stream, char[] password)
		{
			KeyStoreSpi.EngineLoad(stream, password);
			Initialized = true;
		}

		/// <summary>
		/// Loads this keystore using the given {@code LoadStoreParameter}.
		/// 
		/// <para> Note that if this KeyStore has already been loaded, it is
		/// reinitialized and loaded again from the given parameter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="param"> the {@code LoadStoreParameter}
		///          that specifies how to load the keystore,
		///          which may be {@code null}
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the given
		///          {@code LoadStoreParameter}
		///          input is not recognized </exception>
		/// <exception cref="IOException"> if there is an I/O or format problem with the
		///          keystore data. If the error is due to an incorrect
		///         {@code ProtectionParameter} (e.g. wrong password)
		///         the <seealso cref="Throwable#getCause cause"/> of the
		///         {@code IOException} should be an
		///         {@code UnrecoverableKeyException} </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		///          the integrity of the keystore cannot be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates in the
		///          keystore could not be loaded
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void load(LoadStoreParameter param) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public void Load(LoadStoreParameter param)
		{

			KeyStoreSpi.EngineLoad(param);
			Initialized = true;
		}

		/// <summary>
		/// Gets a keystore {@code Entry} for the specified alias
		/// with the specified protection parameter.
		/// </summary>
		/// <param name="alias"> get the keystore {@code Entry} for this alias </param>
		/// <param name="protParam"> the {@code ProtectionParameter}
		///          used to protect the {@code Entry},
		///          which may be {@code null}
		/// </param>
		/// <returns> the keystore {@code Entry} for the specified alias,
		///          or {@code null} if there is no such entry
		/// </returns>
		/// <exception cref="NullPointerException"> if
		///          {@code alias} is {@code null} </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		///          entry cannot be found </exception>
		/// <exception cref="UnrecoverableEntryException"> if the specified
		///          {@code protParam} were insufficient or invalid </exception>
		/// <exception cref="UnrecoverableKeyException"> if the entry is a
		///          {@code PrivateKeyEntry} or {@code SecretKeyEntry}
		///          and the specified {@code protParam} does not contain
		///          the information needed to recover the key (e.g. wrong password) </exception>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		///          (loaded). </exception>
		/// <seealso cref= #setEntry(String, KeyStore.Entry, KeyStore.ProtectionParameter)
		/// 
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final Entry getEntry(String alias, ProtectionParameter protParam) throws NoSuchAlgorithmException, UnrecoverableEntryException, KeyStoreException
		public Entry GetEntry(String alias, ProtectionParameter protParam)
		{

			if (alias == null)
			{
				throw new NullPointerException("invalid null input");
			}
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineGetEntry(alias, protParam);
		}

		/// <summary>
		/// Saves a keystore {@code Entry} under the specified alias.
		/// The protection parameter is used to protect the
		/// {@code Entry}.
		/// 
		/// <para> If an entry already exists for the specified alias,
		/// it is overridden.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> save the keystore {@code Entry} under this alias </param>
		/// <param name="entry"> the {@code Entry} to save </param>
		/// <param name="protParam"> the {@code ProtectionParameter}
		///          used to protect the {@code Entry},
		///          which may be {@code null}
		/// </param>
		/// <exception cref="NullPointerException"> if
		///          {@code alias} or {@code entry}
		///          is {@code null} </exception>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized
		///          (loaded), or if this operation fails for some other reason
		/// </exception>
		/// <seealso cref= #getEntry(String, KeyStore.ProtectionParameter)
		/// 
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setEntry(String alias, Entry entry, ProtectionParameter protParam) throws KeyStoreException
		public void SetEntry(String alias, Entry entry, ProtectionParameter protParam)
		{
			if (alias == null || entry == null)
			{
				throw new NullPointerException("invalid null input");
			}
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			KeyStoreSpi.EngineSetEntry(alias, entry, protParam);
		}

		/// <summary>
		/// Determines if the keystore {@code Entry} for the specified
		/// {@code alias} is an instance or subclass of the specified
		/// {@code entryClass}.
		/// </summary>
		/// <param name="alias"> the alias name </param>
		/// <param name="entryClass"> the entry class
		/// </param>
		/// <returns> true if the keystore {@code Entry} for the specified
		///          {@code alias} is an instance or subclass of the
		///          specified {@code entryClass}, false otherwise
		/// </returns>
		/// <exception cref="NullPointerException"> if
		///          {@code alias} or {@code entryClass}
		///          is {@code null} </exception>
		/// <exception cref="KeyStoreException"> if the keystore has not been
		///          initialized (loaded)
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final boolean entryInstanceOf(String alias, Class entryClass) throws KeyStoreException
		public bool EntryInstanceOf(String alias, Class entryClass)
		{

			if (alias == null || entryClass == null)
			{
				throw new NullPointerException("invalid null input");
			}
			if (!Initialized)
			{
				throw new KeyStoreException("Uninitialized keystore");
			}
			return KeyStoreSpi.EngineEntryInstanceOf(alias, entryClass);
		}

		/// <summary>
		/// A description of a to-be-instantiated KeyStore object.
		/// 
		/// <para>An instance of this class encapsulates the information needed to
		/// instantiate and initialize a KeyStore object. That process is
		/// triggered when the <seealso cref="#getKeyStore"/> method is called.
		/// 
		/// </para>
		/// <para>This makes it possible to decouple configuration from KeyStore
		/// object creation and e.g. delay a password prompt until it is
		/// needed.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= KeyStore </seealso>
		/// <seealso cref= javax.net.ssl.KeyStoreBuilderParameters
		/// @since 1.5 </seealso>
		public abstract class Builder
		{

			// maximum times to try the callbackhandler if the password is wrong
			internal const int MAX_CALLBACK_TRIES = 3;

			/// <summary>
			/// Construct a new Builder.
			/// </summary>
			protected internal Builder()
			{
				// empty
			}

			/// <summary>
			/// Returns the KeyStore described by this object.
			/// </summary>
			/// <returns> the {@code KeyStore} described by this object </returns>
			/// <exception cref="KeyStoreException"> if an error occurred during the
			///   operation, for example if the KeyStore could not be
			///   instantiated or loaded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract KeyStore getKeyStore() throws KeyStoreException;
			public abstract KeyStore KeyStore {get;}

			/// <summary>
			/// Returns the ProtectionParameters that should be used to obtain
			/// the <seealso cref="KeyStore.Entry Entry"/> with the given alias.
			/// The {@code getKeyStore} method must be invoked before this
			/// method may be called.
			/// </summary>
			/// <returns> the ProtectionParameters that should be used to obtain
			///   the <seealso cref="KeyStore.Entry Entry"/> with the given alias. </returns>
			/// <param name="alias"> the alias of the KeyStore entry </param>
			/// <exception cref="NullPointerException"> if alias is null </exception>
			/// <exception cref="KeyStoreException"> if an error occurred during the
			///   operation </exception>
			/// <exception cref="IllegalStateException"> if the getKeyStore method has
			///   not been invoked prior to calling this method </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract ProtectionParameter getProtectionParameter(String alias) throws KeyStoreException;
			public abstract ProtectionParameter GetProtectionParameter(String alias);

			/// <summary>
			/// Returns a new Builder that encapsulates the given KeyStore.
			/// The <seealso cref="#getKeyStore"/> method of the returned object
			/// will return {@code keyStore}, the {@linkplain
			/// #getProtectionParameter getProtectionParameter()} method will
			/// return {@code protectionParameters}.
			/// 
			/// <para> This is useful if an existing KeyStore object needs to be
			/// used with Builder-based APIs.
			/// 
			/// </para>
			/// </summary>
			/// <returns> a new Builder object </returns>
			/// <param name="keyStore"> the KeyStore to be encapsulated </param>
			/// <param name="protectionParameter"> the ProtectionParameter used to
			///   protect the KeyStore entries </param>
			/// <exception cref="NullPointerException"> if keyStore or
			///   protectionParameters is null </exception>
			/// <exception cref="IllegalArgumentException"> if the keyStore has not been
			///   initialized </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Builder newInstance(final KeyStore keyStore, final ProtectionParameter protectionParameter)
			public static Builder NewInstance(KeyStore keyStore, ProtectionParameter protectionParameter)
			{
				if ((keyStore == null) || (protectionParameter == null))
				{
					throw new NullPointerException();
				}
				if (keyStore.Initialized == false)
				{
					throw new IllegalArgumentException("KeyStore not initialized");
				}
				return new BuilderAnonymousInnerClassHelper(keyStore, protectionParameter);
			}

			private class BuilderAnonymousInnerClassHelper : Builder
			{
				private java.security.KeyStore KeyStore;
				private java.security.KeyStore.ProtectionParameter ProtectionParameter;

				public BuilderAnonymousInnerClassHelper(java.security.KeyStore keyStore, java.security.KeyStore.ProtectionParameter protectionParameter)
				{
					this.KeyStore = keyStore;
					this.ProtectionParameter = protectionParameter;
				}

				private volatile bool getCalled;

				public override KeyStore KeyStore
				{
					get
					{
						getCalled = true;
						return KeyStore;
					}
				}

				public override ProtectionParameter GetProtectionParameter(String alias)
				{
					if (alias == null)
					{
						throw new NullPointerException();
					}
					if (getCalled == false)
					{
						throw new IllegalStateException("getKeyStore() must be called first");
					}
					return ProtectionParameter;
				}
			}

			/// <summary>
			/// Returns a new Builder object.
			/// 
			/// <para>The first call to the <seealso cref="#getKeyStore"/> method on the returned
			/// builder will create a KeyStore of type {@code type} and call
			/// its <seealso cref="KeyStore#load load()"/> method.
			/// The {@code inputStream} argument is constructed from
			/// {@code file}.
			/// If {@code protection} is a
			/// {@code PasswordProtection}, the password is obtained by
			/// calling the {@code getPassword} method.
			/// Otherwise, if {@code protection} is a
			/// {@code CallbackHandlerProtection}, the password is obtained
			/// by invoking the CallbackHandler.
			/// 
			/// </para>
			/// <para>Subsequent calls to <seealso cref="#getKeyStore"/> return the same object
			/// as the initial call. If the initial call to failed with a
			/// KeyStoreException, subsequent calls also throw a
			/// KeyStoreException.
			/// 
			/// </para>
			/// <para>The KeyStore is instantiated from {@code provider} if
			/// non-null. Otherwise, all installed providers are searched.
			/// 
			/// </para>
			/// <para>Calls to <seealso cref="#getProtectionParameter getProtectionParameter()"/>
			/// will return a <seealso cref="KeyStore.PasswordProtection PasswordProtection"/>
			/// object encapsulating the password that was used to invoke the
			/// {@code load} method.
			/// 
			/// </para>
			/// <para><em>Note</em> that the <seealso cref="#getKeyStore"/> method is executed
			/// within the <seealso cref="AccessControlContext"/> of the code invoking this
			/// method.
			/// 
			/// </para>
			/// </summary>
			/// <returns> a new Builder object </returns>
			/// <param name="type"> the type of KeyStore to be constructed </param>
			/// <param name="provider"> the provider from which the KeyStore is to
			///   be instantiated (or null) </param>
			/// <param name="file"> the File that contains the KeyStore data </param>
			/// <param name="protection"> the ProtectionParameter securing the KeyStore data </param>
			/// <exception cref="NullPointerException"> if type, file or protection is null </exception>
			/// <exception cref="IllegalArgumentException"> if protection is not an instance
			///   of either PasswordProtection or CallbackHandlerProtection; or
			///   if file does not exist or does not refer to a normal file </exception>
			public static Builder NewInstance(String type, Provider provider, File file, ProtectionParameter protection)
			{
				if ((type == null) || (file == null) || (protection == null))
				{
					throw new NullPointerException();
				}
				if ((protection is PasswordProtection == false) && (protection is CallbackHandlerProtection == false))
				{
					throw new IllegalArgumentException("Protection must be PasswordProtection or " + "CallbackHandlerProtection");
				}
				if (file.File == false)
				{
					throw new IllegalArgumentException("File does not exist or it does not refer " + "to a normal file: " + file);
				}
				return new FileBuilder(type, provider, file, protection, AccessController.Context);
			}

			private sealed class FileBuilder : Builder
			{

				internal readonly String Type;
				internal readonly Provider Provider;
				internal readonly File File;
				internal ProtectionParameter Protection;
				internal ProtectionParameter KeyProtection;
				internal readonly AccessControlContext Context;

				internal KeyStore KeyStore_Renamed;

				internal Throwable OldException;

				internal FileBuilder(String type, Provider provider, File file, ProtectionParameter protection, AccessControlContext context)
				{
					this.Type = type;
					this.Provider = provider;
					this.File = file;
					this.Protection = protection;
					this.Context = context;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized KeyStore getKeyStore() throws KeyStoreException
				public override KeyStore KeyStore
				{
					get
					{
						lock (this)
						{
							if (KeyStore_Renamed != null)
							{
								return KeyStore_Renamed;
							}
							if (OldException != null)
							{
								throw new KeyStoreException("Previous KeyStore instantiation failed", OldException);
							}
							PrivilegedExceptionAction<KeyStore> action = new PrivilegedExceptionActionAnonymousInnerClassHelper(this);
							try
							{
								KeyStore_Renamed = AccessController.doPrivileged(action, Context);
								return KeyStore_Renamed;
							}
							catch (PrivilegedActionException e)
							{
								OldException = e.InnerException;
								throw new KeyStoreException("KeyStore instantiation failed", OldException);
							}
						}
					}
				}

				private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<KeyStore>
				{
					private readonly FileBuilder OuterInstance;

					public PrivilegedExceptionActionAnonymousInnerClassHelper(FileBuilder outerInstance)
					{
						this.OuterInstance = outerInstance;
					}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KeyStore run() throws Exception
					public virtual KeyStore Run()
					{
						if (OuterInstance.Protection is CallbackHandlerProtection == false)
						{
							return run0();
						}
						// when using a CallbackHandler,
						// reprompt if the password is wrong
						int tries = 0;
						while (true)
						{
							tries++;
							try
							{
								return run0();
							}
							catch (IOException e)
							{
								if ((tries < MAX_CALLBACK_TRIES) && (e.InnerException is UnrecoverableKeyException))
								{
									continue;
								}
								throw e;
							}
						}
					}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KeyStore run0() throws Exception
					public virtual KeyStore Run0()
					{
						KeyStore ks;
						if (OuterInstance.Provider == null)
						{
							ks = KeyStore.GetInstance(OuterInstance.Type);
						}
						else
						{
							ks = KeyStore.GetInstance(OuterInstance.Type, OuterInstance.Provider);
						}
						InputStream @in = null;
						char[] password = null;
						try
						{
							@in = new FileInputStream(OuterInstance.File);
							if (OuterInstance.Protection is PasswordProtection)
							{
								password = ((PasswordProtection)OuterInstance.Protection).Password;
								OuterInstance.KeyProtection = OuterInstance.Protection;
							}
							else
							{
								CallbackHandler handler = ((CallbackHandlerProtection)OuterInstance.Protection).CallbackHandler;
								PasswordCallback callback = new PasswordCallback("Password for keystore " + OuterInstance.File.Name, false);
								handler.handle(new Callback[] {callback});
								password = callback.Password;
								if (password == null)
								{
									throw new KeyStoreException("No password" + " provided");
								}
								callback.clearPassword();
								OuterInstance.KeyProtection = new PasswordProtection(password);
							}
							ks.Load(@in, password);
							return ks;
						}
						finally
						{
							if (@in != null)
							{
								@in.Close();
							}
						}
					}
				}

				public override ProtectionParameter GetProtectionParameter(String alias)
				{
					lock (this)
					{
						if (alias == null)
						{
							throw new NullPointerException();
						}
						if (KeyStore_Renamed == null)
						{
							throw new IllegalStateException("getKeyStore() must be called first");
						}
						return KeyProtection;
					}
				}
			}

			/// <summary>
			/// Returns a new Builder object.
			/// 
			/// <para>Each call to the <seealso cref="#getKeyStore"/> method on the returned
			/// builder will return a new KeyStore object of type {@code type}.
			/// Its <seealso cref="KeyStore#load(KeyStore.LoadStoreParameter) load()"/>
			/// method is invoked using a
			/// {@code LoadStoreParameter} that encapsulates
			/// {@code protection}.
			/// 
			/// </para>
			/// <para>The KeyStore is instantiated from {@code provider} if
			/// non-null. Otherwise, all installed providers are searched.
			/// 
			/// </para>
			/// <para>Calls to <seealso cref="#getProtectionParameter getProtectionParameter()"/>
			/// will return {@code protection}.
			/// 
			/// </para>
			/// <para><em>Note</em> that the <seealso cref="#getKeyStore"/> method is executed
			/// within the <seealso cref="AccessControlContext"/> of the code invoking this
			/// method.
			/// 
			/// </para>
			/// </summary>
			/// <returns> a new Builder object </returns>
			/// <param name="type"> the type of KeyStore to be constructed </param>
			/// <param name="provider"> the provider from which the KeyStore is to
			///   be instantiated (or null) </param>
			/// <param name="protection"> the ProtectionParameter securing the Keystore </param>
			/// <exception cref="NullPointerException"> if type or protection is null </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Builder newInstance(final String type, final Provider provider, final ProtectionParameter protection)
			public static Builder NewInstance(String type, Provider provider, ProtectionParameter protection)
			{
				if ((type == null) || (protection == null))
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AccessControlContext context = AccessController.getContext();
				AccessControlContext context = AccessController.Context;
				return new BuilderAnonymousInnerClassHelper(type, provider, protection, context);
			}

			private class BuilderAnonymousInnerClassHelper : Builder
			{
				private string Type;
				private java.security.Provider Provider;
				private java.security.KeyStore.ProtectionParameter Protection;
				private java.security.AccessControlContext Context;

				public BuilderAnonymousInnerClassHelper(string type, java.security.Provider provider, java.security.KeyStore.ProtectionParameter protection, java.security.AccessControlContext context)
				{
					this.Type = type;
					this.Provider = provider;
					this.Protection = protection;
					this.Context = context;
				}

				private volatile bool getCalled;
				private IOException oldException;

				private readonly PrivilegedExceptionAction<KeyStore> action = new PrivilegedExceptionActionAnonymousInnerClassHelper();

				private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<KeyStore>
				{
					public PrivilegedExceptionActionAnonymousInnerClassHelper()
					{
					}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KeyStore run() throws Exception
					public virtual KeyStore Run()
					{
						KeyStore ks;
						if (outerInstance.Provider == null)
						{
							ks = KeyStore.GetInstance(outerInstance.Type);
						}
						else
						{
							ks = KeyStore.GetInstance(outerInstance.Type, outerInstance.Provider);
						}
						LoadStoreParameter param = new SimpleLoadStoreParameter(outerInstance.Protection);
						if (outerInstance.Protection is CallbackHandlerProtection == false)
						{
							ks.Load(param);
						}
						else
						{
							// when using a CallbackHandler,
							// reprompt if the password is wrong
							int tries = 0;
							while (true)
							{
								tries++;
								try
								{
									ks.Load(param);
									break;
								}
								catch (IOException e)
								{
									if (e.InnerException is UnrecoverableKeyException)
									{
										if (tries < MAX_CALLBACK_TRIES)
										{
											continue;
										}
										else
										{
											oldException = e;
										}
									}
									throw e;
								}
							}
						}
						getCalled = true;
						return ks;
					}
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized KeyStore getKeyStore() throws KeyStoreException
				public override KeyStore KeyStore
				{
					get
					{
						lock (this)
						{
							if (oldException != null)
							{
								throw new KeyStoreException("Previous KeyStore instantiation failed", oldException);
							}
							try
							{
								return AccessController.doPrivileged(action, Context);
							}
							catch (PrivilegedActionException e)
							{
								Throwable cause = e.InnerException;
								throw new KeyStoreException("KeyStore instantiation failed", cause);
							}
						}
					}
				}

				public override ProtectionParameter GetProtectionParameter(String alias)
				{
					if (alias == null)
					{
						throw new NullPointerException();
					}
					if (getCalled == false)
					{
						throw new IllegalStateException("getKeyStore() must be called first");
					}
					return Protection;
				}
			}

		}

		internal class SimpleLoadStoreParameter : LoadStoreParameter
		{

			internal readonly ProtectionParameter Protection;

			internal SimpleLoadStoreParameter(ProtectionParameter protection)
			{
				this.Protection = protection;
			}

			public virtual ProtectionParameter ProtectionParameter
			{
				get
				{
					return Protection;
				}
			}
		}

	}

}