using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// for the {@code KeyStore} class.
	/// All the abstract methods in this class must be implemented by each
	/// cryptographic service provider who wishes to supply the implementation
	/// of a keystore for a particular keystore type.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= KeyStore
	/// 
	/// @since 1.2 </seealso>

	public abstract class KeyStoreSpi
	{

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
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		/// key cannot be found </exception>
		/// <exception cref="UnrecoverableKeyException"> if the key cannot be recovered
		/// (e.g., the given password is wrong). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Key engineGetKey(String alias, char[] password) throws NoSuchAlgorithmException, UnrecoverableKeyException;
		public abstract Key EngineGetKey(String alias, char[] password);

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
		/// and the root certificate authority last), or null if the given alias
		/// does not exist or does not contain a certificate chain </returns>
		public abstract Certificate[] EngineGetCertificateChain(String alias);

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
		/// (if a chain exists) is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> the certificate, or null if the given alias does not exist or
		/// does not contain a certificate. </returns>
		public abstract Certificate EngineGetCertificate(String alias);

		/// <summary>
		/// Returns the creation date of the entry identified by the given alias.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> the creation date of this entry, or null if the given alias does
		/// not exist </returns>
		public abstract Date EngineGetCreationDate(String alias);

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
		/// <exception cref="KeyStoreException"> if the given key cannot be protected, or
		/// this operation fails for some other reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void engineSetKeyEntry(String alias, Key key, char[] password, java.security.cert.Certificate[] chain) throws KeyStoreException;
		public abstract void EngineSetKeyEntry(String alias, Key key, char[] password, Certificate[] chain);

		/// <summary>
		/// Assigns the given key (that has already been protected) to the given
		/// alias.
		/// 
		/// <para>If the protected key is of type
		/// {@code java.security.PrivateKey},
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
		/// <param name="key"> the key (in protected format) to be associated with the alias </param>
		/// <param name="chain"> the certificate chain for the corresponding public
		/// key (only useful if the protected key is of type
		/// {@code java.security.PrivateKey}).
		/// </param>
		/// <exception cref="KeyStoreException"> if this operation fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void engineSetKeyEntry(String alias, byte[] key, java.security.cert.Certificate[] chain) throws KeyStoreException;
		public abstract void EngineSetKeyEntry(String alias, sbyte[] key, Certificate[] chain);

		/// <summary>
		/// Assigns the given certificate to the given alias.
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
		/// <exception cref="KeyStoreException"> if the given alias already exists and does
		/// not identify an entry containing a trusted certificate,
		/// or this operation fails for some other reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void engineSetCertificateEntry(String alias, java.security.cert.Certificate cert) throws KeyStoreException;
		public abstract void EngineSetCertificateEntry(String alias, Certificate cert);

		/// <summary>
		/// Deletes the entry identified by the given alias from this keystore.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <exception cref="KeyStoreException"> if the entry cannot be removed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void engineDeleteEntry(String alias) throws KeyStoreException;
		public abstract void EngineDeleteEntry(String alias);

		/// <summary>
		/// Lists all the alias names of this keystore.
		/// </summary>
		/// <returns> enumeration of the alias names </returns>
		public abstract IEnumerator<String> EngineAliases();

		/// <summary>
		/// Checks if the given alias exists in this keystore.
		/// </summary>
		/// <param name="alias"> the alias name
		/// </param>
		/// <returns> true if the alias exists, false otherwise </returns>
		public abstract bool EngineContainsAlias(String alias);

		/// <summary>
		/// Retrieves the number of entries in this keystore.
		/// </summary>
		/// <returns> the number of entries in this keystore </returns>
		public abstract int EngineSize();

		/// <summary>
		/// Returns true if the entry identified by the given alias
		/// was created by a call to {@code setKeyEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code PrivateKeyEntry} or a {@code SecretKeyEntry}.
		/// </summary>
		/// <param name="alias"> the alias for the keystore entry to be checked
		/// </param>
		/// <returns> true if the entry identified by the given alias is a
		/// key-related, false otherwise. </returns>
		public abstract bool EngineIsKeyEntry(String alias);

		/// <summary>
		/// Returns true if the entry identified by the given alias
		/// was created by a call to {@code setCertificateEntry},
		/// or created by a call to {@code setEntry} with a
		/// {@code TrustedCertificateEntry}.
		/// </summary>
		/// <param name="alias"> the alias for the keystore entry to be checked
		/// </param>
		/// <returns> true if the entry identified by the given alias contains a
		/// trusted certificate, false otherwise. </returns>
		public abstract bool EngineIsCertificateEntry(String alias);

		/// <summary>
		/// Returns the (alias) name of the first keystore entry whose certificate
		/// matches the given certificate.
		/// 
		/// <para>This method attempts to match the given certificate with each
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
		/// <returns> the alias name of the first entry with matching certificate,
		/// or null if no such entry exists in this keystore. </returns>
		public abstract String EngineGetCertificateAlias(Certificate cert);

		/// <summary>
		/// Stores this keystore to the given output stream, and protects its
		/// integrity with the given password.
		/// </summary>
		/// <param name="stream"> the output stream to which this keystore is written. </param>
		/// <param name="password"> the password to generate the keystore integrity check
		/// </param>
		/// <exception cref="IOException"> if there was an I/O problem with data </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		/// algorithm could not be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates included in
		/// the keystore data could not be stored </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void engineStore(OutputStream stream, char[] password) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException;
		public abstract void EngineStore(OutputStream stream, char[] password);

		/// <summary>
		/// Stores this keystore using the given
		/// {@code KeyStore.LoadStoreParmeter}.
		/// </summary>
		/// <param name="param"> the {@code KeyStore.LoadStoreParmeter}
		///          that specifies how to store the keystore,
		///          which may be {@code null}
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the given
		///          {@code KeyStore.LoadStoreParmeter}
		///          input is not recognized </exception>
		/// <exception cref="IOException"> if there was an I/O problem with data </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		///          algorithm could not be found </exception>
		/// <exception cref="CertificateException"> if any of the certificates included in
		///          the keystore data could not be stored
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void engineStore(KeyStore.LoadStoreParameter param) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public virtual void EngineStore(KeyStore.LoadStoreParameter param)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Loads the keystore from the given input stream.
		/// 
		/// <para>A password may be given to unlock the keystore
		/// (e.g. the keystore resides on a hardware token device),
		/// or to check the integrity of the keystore data.
		/// If a password is not given for integrity checking,
		/// then integrity checking is not performed.
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
//ORIGINAL LINE: public abstract void engineLoad(InputStream stream, char[] password) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException;
		public abstract void EngineLoad(InputStream stream, char[] password);

		/// <summary>
		/// Loads the keystore using the given
		/// {@code KeyStore.LoadStoreParameter}.
		/// 
		/// <para> Note that if this KeyStore has already been loaded, it is
		/// reinitialized and loaded again from the given parameter.
		/// 
		/// </para>
		/// </summary>
		/// <param name="param"> the {@code KeyStore.LoadStoreParameter}
		///          that specifies how to load the keystore,
		///          which may be {@code null}
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the given
		///          {@code KeyStore.LoadStoreParameter}
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
//ORIGINAL LINE: public void engineLoad(KeyStore.LoadStoreParameter param) throws IOException, NoSuchAlgorithmException, java.security.cert.CertificateException
		public virtual void EngineLoad(KeyStore.LoadStoreParameter param)
		{

			if (param == null)
			{
				EngineLoad((InputStream)null, (char[])null);
				return;
			}

			if (param is KeyStore.SimpleLoadStoreParameter)
			{
				ProtectionParameter protection = param.ProtectionParameter;
				char[] password;
				if (protection is PasswordProtection)
				{
					password = ((PasswordProtection)protection).Password;
				}
				else if (protection is CallbackHandlerProtection)
				{
					CallbackHandler handler = ((CallbackHandlerProtection)protection).CallbackHandler;
					PasswordCallback callback = new PasswordCallback("Password: ", false);
					try
					{
						handler.handle(new Callback[] {callback});
					}
					catch (UnsupportedCallbackException e)
					{
						throw new NoSuchAlgorithmException("Could not obtain password", e);
					}
					password = callback.Password;
					callback.clearPassword();
					if (password == null)
					{
						throw new NoSuchAlgorithmException("No password provided");
					}
				}
				else
				{
					throw new NoSuchAlgorithmException("ProtectionParameter must" + " be PasswordProtection or CallbackHandlerProtection");
				}
				EngineLoad(null, password);
				return;
			}

			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Gets a {@code KeyStore.Entry} for the specified alias
		/// with the specified protection parameter.
		/// </summary>
		/// <param name="alias"> get the {@code KeyStore.Entry} for this alias </param>
		/// <param name="protParam"> the {@code ProtectionParameter}
		///          used to protect the {@code Entry},
		///          which may be {@code null}
		/// </param>
		/// <returns> the {@code KeyStore.Entry} for the specified alias,
		///          or {@code null} if there is no such entry
		/// </returns>
		/// <exception cref="KeyStoreException"> if the operation failed </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		///          entry cannot be found </exception>
		/// <exception cref="UnrecoverableEntryException"> if the specified
		///          {@code protParam} were insufficient or invalid </exception>
		/// <exception cref="UnrecoverableKeyException"> if the entry is a
		///          {@code PrivateKeyEntry} or {@code SecretKeyEntry}
		///          and the specified {@code protParam} does not contain
		///          the information needed to recover the key (e.g. wrong password)
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KeyStore.Entry engineGetEntry(String alias, KeyStore.ProtectionParameter protParam) throws KeyStoreException, NoSuchAlgorithmException, UnrecoverableEntryException
		public virtual KeyStore.Entry EngineGetEntry(String alias, KeyStore.ProtectionParameter protParam)
		{

			if (!EngineContainsAlias(alias))
			{
				return null;
			}

			if (protParam == null)
			{
				if (EngineIsCertificateEntry(alias))
				{
					return new KeyStore.TrustedCertificateEntry(EngineGetCertificate(alias));
				}
				else
				{
					throw new UnrecoverableKeyException("requested entry requires a password");
				}
			}

			if (protParam is KeyStore.PasswordProtection)
			{
				if (EngineIsCertificateEntry(alias))
				{
					throw new UnsupportedOperationException("trusted certificate entries are not password-protected");
				}
				else if (EngineIsKeyEntry(alias))
				{
					KeyStore.PasswordProtection pp = (KeyStore.PasswordProtection)protParam;
					char[] password = pp.Password;

					Key key = EngineGetKey(alias, password);
					if (key is PrivateKey)
					{
						Certificate[] chain = EngineGetCertificateChain(alias);
						return new KeyStore.PrivateKeyEntry((PrivateKey)key, chain);
					}
					else if (key is SecretKey)
					{
						return new KeyStore.SecretKeyEntry((SecretKey)key);
					}
				}
			}

			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Saves a {@code KeyStore.Entry} under the specified alias.
		/// The specified protection parameter is used to protect the
		/// {@code Entry}.
		/// 
		/// <para> If an entry already exists for the specified alias,
		/// it is overridden.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alias"> save the {@code KeyStore.Entry} under this alias </param>
		/// <param name="entry"> the {@code Entry} to save </param>
		/// <param name="protParam"> the {@code ProtectionParameter}
		///          used to protect the {@code Entry},
		///          which may be {@code null}
		/// </param>
		/// <exception cref="KeyStoreException"> if this operation fails
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void engineSetEntry(String alias, KeyStore.Entry entry, KeyStore.ProtectionParameter protParam) throws KeyStoreException
		public virtual void EngineSetEntry(String alias, KeyStore.Entry entry, KeyStore.ProtectionParameter protParam)
		{

			// get password
			if (protParam != null && !(protParam is KeyStore.PasswordProtection))
			{
				throw new KeyStoreException("unsupported protection parameter");
			}
			KeyStore.PasswordProtection pProtect = null;
			if (protParam != null)
			{
				pProtect = (KeyStore.PasswordProtection)protParam;
			}

			// set entry
			if (entry is KeyStore.TrustedCertificateEntry)
			{
				if (protParam != null && pProtect.Password != null)
				{
					// pre-1.5 style setCertificateEntry did not allow password
					throw new KeyStoreException("trusted certificate entries are not password-protected");
				}
				else
				{
					KeyStore.TrustedCertificateEntry tce = (KeyStore.TrustedCertificateEntry)entry;
					EngineSetCertificateEntry(alias, tce.TrustedCertificate);
					return;
				}
			}
			else if (entry is KeyStore.PrivateKeyEntry)
			{
				if (pProtect == null || pProtect.Password == null)
				{
					// pre-1.5 style setKeyEntry required password
					throw new KeyStoreException("non-null password required to create PrivateKeyEntry");
				}
				else
				{
					EngineSetKeyEntry(alias, ((KeyStore.PrivateKeyEntry)entry).PrivateKey, pProtect.Password, ((KeyStore.PrivateKeyEntry)entry).CertificateChain);
					return;
				}
			}
			else if (entry is KeyStore.SecretKeyEntry)
			{
				if (pProtect == null || pProtect.Password == null)
				{
					// pre-1.5 style setKeyEntry required password
					throw new KeyStoreException("non-null password required to create SecretKeyEntry");
				}
				else
				{
					EngineSetKeyEntry(alias, ((KeyStore.SecretKeyEntry)entry).SecretKey, pProtect.Password, (Certificate[])null);
					return;
				}
			}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new KeyStoreException("unsupported entry type: " + entry.GetType().FullName);
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
		/// 
		/// @since 1.5 </returns>
		public virtual bool EngineEntryInstanceOf(String alias, Class entryClass)
		{
			if (entryClass == typeof(KeyStore.TrustedCertificateEntry))
			{
				return EngineIsCertificateEntry(alias);
			}
			if (entryClass == typeof(KeyStore.PrivateKeyEntry))
			{
				return EngineIsKeyEntry(alias) && EngineGetCertificate(alias) != null;
			}
			if (entryClass == typeof(KeyStore.SecretKeyEntry))
			{
				return EngineIsKeyEntry(alias) && EngineGetCertificate(alias) == null;
			}
			return false;
		}
	}

}