using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

	using Debug = sun.security.util.Debug;
	using PropertyExpander = sun.security.util.PropertyExpander;

	using sun.security.jca;

	/// <summary>
	/// <para>This class centralizes all security properties and common security
	/// methods. One of its primary uses is to manage providers.
	/// 
	/// </para>
	/// <para>The default values of security properties are read from an
	/// implementation-specific location, which is typically the properties file
	/// {@code lib/security/java.security} in the Java installation directory.
	/// 
	/// @author Benjamin Renaud
	/// </para>
	/// </summary>

	public sealed class Security
	{

		/* Are we debugging? -- for developers */
		private static readonly Debug Sdebug = Debug.getInstance("properties");

		/* The java.security properties */
		private static Properties Props;

		// An element in the cache
		private class ProviderProperty
		{
			internal String ClassName;
			internal Provider Provider;
		}

		static Security()
		{
			// doPrivileged here because there are multiple
			// things in initialize that might require privs.
			// (the FileInputStream call and the File.exists call,
			// the securityPropFile call, etc)
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
				Initialize();
				return null;
			}
		}

		private static void Initialize()
		{
			Props = new Properties();
			bool loadedProps = false;
			bool overrideAll = false;

			// first load the system properties file
			// to determine the value of security.overridePropertiesFile
			File propFile = SecurityPropFile("java.security");
			if (propFile.Exists())
			{
				InputStream @is = null;
				try
				{
					FileInputStream fis = new FileInputStream(propFile);
					@is = new BufferedInputStream(fis);
					Props.Load(@is);
					loadedProps = true;

					if (Sdebug != null)
					{
						Sdebug.println("reading security properties file: " + propFile);
					}
				}
				catch (IOException e)
				{
					if (Sdebug != null)
					{
						Sdebug.println("unable to load security properties from " + propFile);
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				finally
				{
					if (@is != null)
					{
						try
						{
							@is.Close();
						}
						catch (IOException)
						{
							if (Sdebug != null)
							{
								Sdebug.println("unable to close input stream");
							}
						}
					}
				}
			}

			if ("true".Equals(Props.GetProperty("security.overridePropertiesFile"), StringComparison.CurrentCultureIgnoreCase))
			{

				String extraPropFile = System.getProperty("java.security.properties");
				if (extraPropFile != null && extraPropFile.StartsWith("="))
				{
					overrideAll = true;
					extraPropFile = extraPropFile.Substring(1);
				}

				if (overrideAll)
				{
					Props = new Properties();
					if (Sdebug != null)
					{
						Sdebug.println("overriding other security properties files!");
					}
				}

				// now load the user-specified file so its values
				// will win if they conflict with the earlier values
				if (extraPropFile != null)
				{
					BufferedInputStream bis = null;
					try
					{
						URL propURL;

						extraPropFile = PropertyExpander.expand(extraPropFile);
						propFile = new File(extraPropFile);
						if (propFile.Exists())
						{
							propURL = new URL("file:" + propFile.CanonicalPath);
						}
						else
						{
							propURL = new URL(extraPropFile);
						}
						bis = new BufferedInputStream(propURL.OpenStream());
						Props.Load(bis);
						loadedProps = true;

						if (Sdebug != null)
						{
							Sdebug.println("reading security properties file: " + propURL);
							if (overrideAll)
							{
								Sdebug.println("overriding other security properties files!");
							}
						}
					}
					catch (Exception e)
					{
						if (Sdebug != null)
						{
							Sdebug.println("unable to load security properties from " + extraPropFile);
							e.PrintStackTrace();
						}
					}
					finally
					{
						if (bis != null)
						{
							try
							{
								bis.Close();
							}
							catch (IOException)
							{
								if (Sdebug != null)
								{
									Sdebug.println("unable to close input stream");
								}
							}
						}
					}
				}
			}

			if (!loadedProps)
			{
				InitializeStatic();
				if (Sdebug != null)
				{
					Sdebug.println("unable to load security properties " + "-- using defaults");
				}
			}

		}

		/*
		 * Initialize to default values, if <java.home>/lib/java.security
		 * is not found.
		 */
		private static void InitializeStatic()
		{
			Props["security.provider.1"] = "sun.security.provider.Sun";
			Props["security.provider.2"] = "sun.security.rsa.SunRsaSign";
			Props["security.provider.3"] = "com.sun.net.ssl.internal.ssl.Provider";
			Props["security.provider.4"] = "com.sun.crypto.provider.SunJCE";
			Props["security.provider.5"] = "sun.security.jgss.SunProvider";
			Props["security.provider.6"] = "com.sun.security.sasl.Provider";
		}

		/// <summary>
		/// Don't let anyone instantiate this.
		/// </summary>
		private Security()
		{
		}

		private static File SecurityPropFile(String filename)
		{
			// maybe check for a system property which will specify where to
			// look. Someday.
			String sep = File.Separator;
			return new File(System.getProperty("java.home") + sep + "lib" + sep + "security" + sep + filename);
		}

		/// <summary>
		/// Looks up providers, and returns the property (and its associated
		/// provider) mapping the key, if any.
		/// The order in which the providers are looked up is the
		/// provider-preference order, as specificed in the security
		/// properties file.
		/// </summary>
		private static ProviderProperty GetProviderProperty(String key)
		{
			ProviderProperty entry = null;

			List<Provider> providers = Providers.ProviderList.providers();
			for (int i = 0; i < providers.Count; i++)
			{

				String matchKey = null;
				Provider prov = providers.Get(i);
				String prop = prov.GetProperty(key);

				if (prop == null)
				{
					// Is there a match if we do a case-insensitive property name
					// comparison? Let's try ...
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					for (IEnumerator<Object> e = prov.Keys(); e.hasMoreElements() && prop == null;)
					{
						matchKey = (String)e.Current;
						if (key.EqualsIgnoreCase(matchKey))
						{
							prop = prov.GetProperty(matchKey);
							break;
						}
					}
				}

				if (prop != null)
				{
					ProviderProperty newEntry = new ProviderProperty();
					newEntry.ClassName = prop;
					newEntry.Provider = prov;
					return newEntry;
				}
			}

			return entry;
		}

		/// <summary>
		/// Returns the property (if any) mapping the key for the given provider.
		/// </summary>
		private static String GetProviderProperty(String key, Provider provider)
		{
			String prop = provider.GetProperty(key);
			if (prop == null)
			{
				// Is there a match if we do a case-insensitive property name
				// comparison? Let's try ...
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				for (IEnumerator<Object> e = provider.Keys(); e.hasMoreElements() && prop == null;)
				{
					String matchKey = (String)e.Current;
					if (key.EqualsIgnoreCase(matchKey))
					{
						prop = provider.GetProperty(matchKey);
						break;
					}
				}
			}
			return prop;
		}

		/// <summary>
		/// Gets a specified property for an algorithm. The algorithm name
		/// should be a standard name. See the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard algorithm names.
		/// 
		/// One possible use is by specialized algorithm parsers, which may map
		/// classes to algorithms which they understand (much like Key parsers
		/// do).
		/// </summary>
		/// <param name="algName"> the algorithm name.
		/// </param>
		/// <param name="propName"> the name of the property to get.
		/// </param>
		/// <returns> the value of the specified property.
		/// </returns>
		/// @deprecated This method used to return the value of a proprietary
		/// property in the master file of the "SUN" Cryptographic Service
		/// Provider in order to determine how to parse algorithm-specific
		/// parameters. Use the new provider-based and algorithm-independent
		/// {@code AlgorithmParameters} and {@code KeyFactory} engine
		/// classes (introduced in the J2SE version 1.2 platform) instead. 
		[Obsolete("This method used to return the value of a proprietary")]
		public static String GetAlgorithmProperty(String algName, String propName)
		{
			ProviderProperty entry = GetProviderProperty("Alg." + propName + "." + algName);
			if (entry != null)
			{
				return entry.ClassName;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Adds a new provider, at a specified position. The position is
		/// the preference order in which providers are searched for
		/// requested algorithms.  The position is 1-based, that is,
		/// 1 is most preferred, followed by 2, and so on.
		/// 
		/// <para>If the given provider is installed at the requested position,
		/// the provider that used to be at that position, and all providers
		/// with a position greater than {@code position}, are shifted up
		/// one position (towards the end of the list of installed providers).
		/// 
		/// </para>
		/// <para>A provider cannot be added if it is already installed.
		/// 
		/// </para>
		/// <para>If there is a security manager, the
		/// <seealso cref="java.lang.SecurityManager#checkSecurityAccess"/> method is called
		/// with the {@code "insertProvider"} permission target name to see if
		/// it's ok to add a new provider. If this permission check is denied,
		/// {@code checkSecurityAccess} is called again with the
		/// {@code "insertProvider."+provider.getName()} permission target name. If
		/// both checks are denied, a {@code SecurityException} is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="provider"> the provider to be added.
		/// </param>
		/// <param name="position"> the preference position that the caller would
		/// like for this provider.
		/// </param>
		/// <returns> the actual preference position in which the provider was
		/// added, or -1 if the provider was not added because it is
		/// already installed.
		/// </returns>
		/// <exception cref="NullPointerException"> if provider is null </exception>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to add a new provider
		/// </exception>
		/// <seealso cref= #getProvider </seealso>
		/// <seealso cref= #removeProvider </seealso>
		/// <seealso cref= java.security.SecurityPermission </seealso>
		public static int InsertProviderAt(Provider provider, int position)
		{
			lock (typeof(Security))
			{
				String providerName = provider.Name;
				CheckInsertProvider(providerName);
				ProviderList list = Providers.FullProviderList;
				ProviderList newList = ProviderList.insertAt(list, provider, position - 1);
				if (list == newList)
				{
					return -1;
				}
				Providers.ProviderList = newList;
				return newList.getIndex(providerName) + 1;
			}
		}

		/// <summary>
		/// Adds a provider to the next position available.
		/// 
		/// <para>If there is a security manager, the
		/// <seealso cref="java.lang.SecurityManager#checkSecurityAccess"/> method is called
		/// with the {@code "insertProvider"} permission target name to see if
		/// it's ok to add a new provider. If this permission check is denied,
		/// {@code checkSecurityAccess} is called again with the
		/// {@code "insertProvider."+provider.getName()} permission target name. If
		/// both checks are denied, a {@code SecurityException} is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="provider"> the provider to be added.
		/// </param>
		/// <returns> the preference position in which the provider was
		/// added, or -1 if the provider was not added because it is
		/// already installed.
		/// </returns>
		/// <exception cref="NullPointerException"> if provider is null </exception>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to add a new provider
		/// </exception>
		/// <seealso cref= #getProvider </seealso>
		/// <seealso cref= #removeProvider </seealso>
		/// <seealso cref= java.security.SecurityPermission </seealso>
		public static int AddProvider(Provider provider)
		{
			/*
			 * We can't assign a position here because the statically
			 * registered providers may not have been installed yet.
			 * insertProviderAt() will fix that value after it has
			 * loaded the static providers.
			 */
			return InsertProviderAt(provider, 0);
		}

		/// <summary>
		/// Removes the provider with the specified name.
		/// 
		/// <para>When the specified provider is removed, all providers located
		/// at a position greater than where the specified provider was are shifted
		/// down one position (towards the head of the list of installed
		/// providers).
		/// 
		/// </para>
		/// <para>This method returns silently if the provider is not installed or
		/// if name is null.
		/// 
		/// </para>
		/// <para>First, if there is a security manager, its
		/// {@code checkSecurityAccess}
		/// method is called with the string {@code "removeProvider."+name}
		/// to see if it's ok to remove the provider.
		/// If the default implementation of {@code checkSecurityAccess}
		/// is used (i.e., that method is not overriden), then this will result in
		/// a call to the security manager's {@code checkPermission} method
		/// with a {@code SecurityPermission("removeProvider."+name)}
		/// permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the provider to remove.
		/// </param>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies
		///          access to remove the provider
		/// </exception>
		/// <seealso cref= #getProvider </seealso>
		/// <seealso cref= #addProvider </seealso>
		public static void RemoveProvider(String name)
		{
			lock (typeof(Security))
			{
				Check("removeProvider." + name);
				ProviderList list = Providers.FullProviderList;
				ProviderList newList = ProviderList.remove(list, name);
				Providers.ProviderList = newList;
			}
		}

		/// <summary>
		/// Returns an array containing all the installed providers. The order of
		/// the providers in the array is their preference order.
		/// </summary>
		/// <returns> an array of all the installed providers. </returns>
		public static Provider[] Providers
		{
			get
			{
				return Providers.FullProviderList.toArray();
			}
		}

		/// <summary>
		/// Returns the provider installed with the specified name, if
		/// any. Returns null if no provider with the specified name is
		/// installed or if name is null.
		/// </summary>
		/// <param name="name"> the name of the provider to get.
		/// </param>
		/// <returns> the provider of the specified name.
		/// </returns>
		/// <seealso cref= #removeProvider </seealso>
		/// <seealso cref= #addProvider </seealso>
		public static Provider GetProvider(String name)
		{
			return Providers.ProviderList.getProvider(name);
		}

		/// <summary>
		/// Returns an array containing all installed providers that satisfy the
		/// specified selection criterion, or null if no such providers have been
		/// installed. The returned providers are ordered
		/// according to their
		/// <seealso cref="#insertProviderAt(java.security.Provider, int) preference order"/>.
		/// 
		/// <para> A cryptographic service is always associated with a particular
		/// algorithm or type. For example, a digital signature service is
		/// always associated with a particular algorithm (e.g., DSA),
		/// and a CertificateFactory service is always associated with
		/// a particular certificate type (e.g., X.509).
		/// 
		/// </para>
		/// <para>The selection criterion must be specified in one of the following two
		/// formats:
		/// <ul>
		/// <li> <i>{@literal <crypto_service>.<algorithm_or_type>}</i>
		/// </para>
		/// <para> The cryptographic service name must not contain any dots.
		/// </para>
		/// <para> A
		/// provider satisfies the specified selection criterion iff the provider
		/// implements the
		/// specified algorithm or type for the specified cryptographic service.
		/// </para>
		/// <para> For example, "CertificateFactory.X.509"
		/// would be satisfied by any provider that supplied
		/// a CertificateFactory implementation for X.509 certificates.
		/// <li> <i>{@literal <crypto_service>.<algorithm_or_type>
		/// <attribute_name>:<attribute_value>}</i>
		/// </para>
		/// <para> The cryptographic service name must not contain any dots. There
		/// must be one or more space characters between the
		/// <i>{@literal <algorithm_or_type>}</i> and the
		/// <i>{@literal <attribute_name>}</i>.
		/// </para>
		///  <para> A provider satisfies this selection criterion iff the
		/// provider implements the specified algorithm or type for the specified
		/// cryptographic service and its implementation meets the
		/// constraint expressed by the specified attribute name/value pair.
		/// </para>
		/// <para> For example, "Signature.SHA1withDSA KeySize:1024" would be
		/// satisfied by any provider that implemented
		/// the SHA1withDSA signature algorithm with a keysize of 1024 (or larger).
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para> See the <a href=
		/// "{@docRoot}/../technotes/guides/security/StandardNames.html">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard cryptographic service names, standard
		/// algorithm names and standard attribute names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="filter"> the criterion for selecting
		/// providers. The filter is case-insensitive.
		/// </param>
		/// <returns> all the installed providers that satisfy the selection
		/// criterion, or null if no such providers have been installed.
		/// </returns>
		/// <exception cref="InvalidParameterException">
		///         if the filter is not in the required format </exception>
		/// <exception cref="NullPointerException"> if filter is null
		/// </exception>
		/// <seealso cref= #getProviders(java.util.Map)
		/// @since 1.3 </seealso>
		public static Provider[] GetProviders(String filter)
		{
			String key = null;
			String value = null;
			int index = filter.IndexOf(':');

			if (index == -1)
			{
				key = filter;
				value = "";
			}
			else
			{
				key = filter.Substring(0, index);
				value = filter.Substring(index + 1);
			}

			Dictionary<String, String> hashtableFilter = new Dictionary<String, String>(1);
			hashtableFilter.Put(key, value);

			return (GetProviders(hashtableFilter));
		}

		/// <summary>
		/// Returns an array containing all installed providers that satisfy the
		/// specified* selection criteria, or null if no such providers have been
		/// installed. The returned providers are ordered
		/// according to their
		/// {@link #insertProviderAt(java.security.Provider, int)
		/// preference order}.
		/// 
		/// <para>The selection criteria are represented by a map.
		/// Each map entry represents a selection criterion.
		/// A provider is selected iff it satisfies all selection
		/// criteria. The key for any entry in such a map must be in one of the
		/// following two formats:
		/// <ul>
		/// <li> <i>{@literal <crypto_service>.<algorithm_or_type>}</i>
		/// </para>
		/// <para> The cryptographic service name must not contain any dots.
		/// </para>
		/// <para> The value associated with the key must be an empty string.
		/// </para>
		/// <para> A provider
		/// satisfies this selection criterion iff the provider implements the
		/// specified algorithm or type for the specified cryptographic service.
		/// <li>  <i>{@literal <crypto_service>}.
		/// {@literal <algorithm_or_type> <attribute_name>}</i>
		/// </para>
		/// <para> The cryptographic service name must not contain any dots. There
		/// must be one or more space characters between the
		/// <i>{@literal <algorithm_or_type>}</i>
		/// and the <i>{@literal <attribute_name>}</i>.
		/// </para>
		/// <para> The value associated with the key must be a non-empty string.
		/// A provider satisfies this selection criterion iff the
		/// provider implements the specified algorithm or type for the specified
		/// cryptographic service and its implementation meets the
		/// constraint expressed by the specified attribute name/value pair.
		/// </ul>
		/// 
		/// </para>
		/// <para> See the <a href=
		/// "../../../technotes/guides/security/StandardNames.html">
		/// Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		/// for information about standard cryptographic service names, standard
		/// algorithm names and standard attribute names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="filter"> the criteria for selecting
		/// providers. The filter is case-insensitive.
		/// </param>
		/// <returns> all the installed providers that satisfy the selection
		/// criteria, or null if no such providers have been installed.
		/// </returns>
		/// <exception cref="InvalidParameterException">
		///         if the filter is not in the required format </exception>
		/// <exception cref="NullPointerException"> if filter is null
		/// </exception>
		/// <seealso cref= #getProviders(java.lang.String)
		/// @since 1.3 </seealso>
		public static Provider[] GetProviders(Map<String, String> filter)
		{
			// Get all installed providers first.
			// Then only return those providers who satisfy the selection criteria.
			Provider[] allProviders = Security.Providers;
			IDictionary<String, String>.KeyCollection keySet = filter.KeySet();
			LinkedHashSet<Provider> candidates = new LinkedHashSet<Provider>(5);

			// Returns all installed providers
			// if the selection criteria is null.
			if ((keySet == null) || (allProviders == null))
			{
				return allProviders;
			}

			bool firstSearch = true;

			// For each selection criterion, remove providers
			// which don't satisfy the criterion from the candidate set.
			for (Iterator<String> ite = keySet.GetEnumerator(); ite.HasNext();)
			{
				String key = ite.Next();
				String value = filter.Get(key);

				LinkedHashSet<Provider> newCandidates = GetAllQualifyingCandidates(key, value, allProviders);
				if (firstSearch)
				{
					candidates = newCandidates;
					firstSearch = false;
				}

				if ((newCandidates != null) && newCandidates.Count > 0)
				{
					// For each provider in the candidates set, if it
					// isn't in the newCandidate set, we should remove
					// it from the candidate set.
					for (Iterator<Provider> cansIte = candidates.Iterator(); cansIte.HasNext();)
					{
						Provider prov = cansIte.Next();
						if (!newCandidates.Contains(prov))
						{
							cansIte.remove();
						}
					}
				}
				else
				{
					candidates = null;
					break;
				}
			}

			if ((candidates == null) || (candidates.Count == 0))
			{
				return null;
			}

			Object[] candidatesArray = candidates.ToArray();
			Provider[] result = new Provider[candidatesArray.Length];

			for (int i = 0; i < result.Length; i++)
			{
				result[i] = (Provider)candidatesArray[i];
			}

			return result;
		}

		// Map containing cached Spi Class objects of the specified type
		private static readonly Map<String, Class> SpiMap = new ConcurrentDictionary<String, Class>();

		/// <summary>
		/// Return the Class object for the given engine type
		/// (e.g. "MessageDigest"). Works for Spis in the java.security package
		/// only.
		/// </summary>
		private static Class GetSpiClass(String type)
		{
			Class clazz = SpiMap.Get(type);
			if (clazz != null)
			{
				return clazz;
			}
			try
			{
				clazz = Class.ForName("java.security." + type + "Spi");
				SpiMap.Put(type, clazz);
				return clazz;
			}
			catch (ClassNotFoundException e)
			{
				throw new AssertionError("Spi class not found", e);
			}
		}

		/*
		 * Returns an array of objects: the first object in the array is
		 * an instance of an implementation of the requested algorithm
		 * and type, and the second object in the array identifies the provider
		 * of that implementation.
		 * The {@code provider} argument can be null, in which case all
		 * configured providers will be searched in order of preference.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object[] getImpl(String algorithm, String type, String provider) throws NoSuchAlgorithmException, NoSuchProviderException
		internal static Object[] GetImpl(String algorithm, String type, String provider)
		{
			if (provider == null)
			{
				return GetInstance.getInstance(type, GetSpiClass(type), algorithm).toArray();
			}
			else
			{
				return GetInstance.getInstance(type, GetSpiClass(type), algorithm, provider).toArray();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object[] getImpl(String algorithm, String type, String provider, Object params) throws NoSuchAlgorithmException, NoSuchProviderException, InvalidAlgorithmParameterException
		internal static Object[] GetImpl(String algorithm, String type, String provider, Object @params)
		{
			if (provider == null)
			{
				return GetInstance.getInstance(type, GetSpiClass(type), algorithm, @params).toArray();
			}
			else
			{
				return GetInstance.getInstance(type, GetSpiClass(type), algorithm, @params, provider).toArray();
			}
		}

		/*
		 * Returns an array of objects: the first object in the array is
		 * an instance of an implementation of the requested algorithm
		 * and type, and the second object in the array identifies the provider
		 * of that implementation.
		 * The {@code provider} argument cannot be null.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object[] getImpl(String algorithm, String type, Provider provider) throws NoSuchAlgorithmException
		internal static Object[] GetImpl(String algorithm, String type, Provider provider)
		{
			return GetInstance.getInstance(type, GetSpiClass(type), algorithm, provider).toArray();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object[] getImpl(String algorithm, String type, Provider provider, Object params) throws NoSuchAlgorithmException, InvalidAlgorithmParameterException
		internal static Object[] GetImpl(String algorithm, String type, Provider provider, Object @params)
		{
			return GetInstance.getInstance(type, GetSpiClass(type), algorithm, @params, provider).toArray();
		}

		/// <summary>
		/// Gets a security property value.
		/// 
		/// <para>First, if there is a security manager, its
		/// {@code checkPermission}  method is called with a
		/// {@code java.security.SecurityPermission("getProperty."+key)}
		/// permission to see if it's ok to retrieve the specified
		/// security property value..
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> the key of the property being retrieved.
		/// </param>
		/// <returns> the value of the security property corresponding to key.
		/// </returns>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkPermission} method
		///          denies
		///          access to retrieve the specified security property value </exception>
		/// <exception cref="NullPointerException"> is key is null
		/// </exception>
		/// <seealso cref= #setProperty </seealso>
		/// <seealso cref= java.security.SecurityPermission </seealso>
		public static String GetProperty(String key)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new SecurityPermission("getProperty." + key));
			}
			String name = Props.GetProperty(key);
			if (name != null)
			{
				name = name.Trim(); // could be a class name with trailing ws
			}
			return name;
		}

		/// <summary>
		/// Sets a security property value.
		/// 
		/// <para>First, if there is a security manager, its
		/// {@code checkPermission} method is called with a
		/// {@code java.security.SecurityPermission("setProperty."+key)}
		/// permission to see if it's ok to set the specified
		/// security property value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> the name of the property to be set.
		/// </param>
		/// <param name="datum"> the value of the property to be set.
		/// </param>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkPermission} method
		///          denies access to set the specified security property value </exception>
		/// <exception cref="NullPointerException"> if key or datum is null
		/// </exception>
		/// <seealso cref= #getProperty </seealso>
		/// <seealso cref= java.security.SecurityPermission </seealso>
		public static void SetProperty(String key, String datum)
		{
			Check("setProperty." + key);
			Props[key] = datum;
			InvalidateSMCache(key); // See below.
		}

		/*
		 * Implementation detail:  If the property we just set in
		 * setProperty() was either "package.access" or
		 * "package.definition", we need to signal to the SecurityManager
		 * class that the value has just changed, and that it should
		 * invalidate it's local cache values.
		 *
		 * Rather than create a new API entry for this function,
		 * we use reflection to set a private variable.
		 */
		private static void InvalidateSMCache(String key)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean pa = key.equals("package.access");
			bool pa = key.Equals("package.access");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean pd = key.equals("package.definition");
			bool pd = key.Equals("package.definition");

			if (pa || pd)
			{
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(pa)); // PrivilegedAction
			} // if
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private bool Pa;

			public PrivilegedActionAnonymousInnerClassHelper2(bool pa)
			{
				this.Pa = pa;
			}

			public virtual Void Run()
			{
				try
				{
					/* Get the class via the bootstrap class loader. */
					Class cl = Class.ForName("java.lang.SecurityManager", false, null);
					Field f = null;
					bool accessible = false;

					if (Pa)
					{
						f = cl.GetDeclaredField("packageAccessValid");
						accessible = f.Accessible;
						f.Accessible = true;
					}
					else
					{
						f = cl.GetDeclaredField("packageDefinitionValid");
						accessible = f.Accessible;
						f.Accessible = true;
					}
					f.SetBoolean(f, false);
					f.Accessible = accessible;
				}
				catch (Exception)
				{
					/* If we couldn't get the class, it hasn't
					 * been loaded yet.  If there is no such
					 * field, we shouldn't try to set it.  There
					 * shouldn't be a security execption, as we
					 * are loaded by boot class loader, and we
					 * are inside a doPrivileged() here.
					 *
					 * NOOP: don't do anything...
					 */
				}
				return null;
			} // run
		}

		private static void Check(String directive)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckSecurityAccess(directive);
			}
		}

		private static void CheckInsertProvider(String name)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				try
				{
					security.CheckSecurityAccess("insertProvider");
				}
				catch (SecurityException se1)
				{
					try
					{
						security.CheckSecurityAccess("insertProvider." + name);
					}
					catch (SecurityException se2)
					{
						// throw first exception, but add second to suppressed
						se1.AddSuppressed(se2);
						throw se1;
					}
				}
			}
		}

		/*
		* Returns all providers who satisfy the specified
		* criterion.
		*/
		private static LinkedHashSet<Provider> GetAllQualifyingCandidates(String filterKey, String filterValue, Provider[] allProviders)
		{
			String[] filterComponents = GetFilterComponents(filterKey, filterValue);

			// The first component is the service name.
			// The second is the algorithm name.
			// If the third isn't null, that is the attrinute name.
			String serviceName = filterComponents[0];
			String algName = filterComponents[1];
			String attrName = filterComponents[2];

			return GetProvidersNotUsingCache(serviceName, algName, attrName, filterValue, allProviders);
		}

		private static LinkedHashSet<Provider> GetProvidersNotUsingCache(String serviceName, String algName, String attrName, String filterValue, Provider[] allProviders)
		{
			LinkedHashSet<Provider> candidates = new LinkedHashSet<Provider>(5);
			for (int i = 0; i < allProviders.Length; i++)
			{
				if (IsCriterionSatisfied(allProviders[i], serviceName, algName, attrName, filterValue))
				{
					candidates.Add(allProviders[i]);
				}
			}
			return candidates;
		}

		/*
		 * Returns true if the given provider satisfies
		 * the selection criterion key:value.
		 */
		private static bool IsCriterionSatisfied(Provider prov, String serviceName, String algName, String attrName, String filterValue)
		{
			String key = serviceName + '.' + algName;

			if (attrName != null)
			{
				key += ' ' + attrName;
			}
			// Check whether the provider has a property
			// whose key is the same as the given key.
			String propValue = GetProviderProperty(key, prov);

			if (propValue == null)
			{
				// Check whether we have an alias instead
				// of a standard name in the key.
				String standardName = GetProviderProperty("Alg.Alias." + serviceName + "." + algName, prov);
				if (standardName != null)
				{
					key = serviceName + "." + standardName;

					if (attrName != null)
					{
						key += ' ' + attrName;
					}

					propValue = GetProviderProperty(key, prov);
				}

				if (propValue == null)
				{
					// The provider doesn't have the given
					// key in its property list.
					return false;
				}
			}

			// If the key is in the format of:
			// <crypto_service>.<algorithm_or_type>,
			// there is no need to check the value.

			if (attrName == null)
			{
				return true;
			}

			// If we get here, the key must be in the
			// format of <crypto_service>.<algorithm_or_provider> <attribute_name>.
			if (IsStandardAttr(attrName))
			{
				return IsConstraintSatisfied(attrName, filterValue, propValue);
			}
			else
			{
				return filterValue.EqualsIgnoreCase(propValue);
			}
		}

		/*
		 * Returns true if the attribute is a standard attribute;
		 * otherwise, returns false.
		 */
		private static bool IsStandardAttr(String attribute)
		{
			// For now, we just have two standard attributes:
			// KeySize and ImplementedIn.
			if (attribute.EqualsIgnoreCase("KeySize"))
			{
				return true;
			}

			if (attribute.EqualsIgnoreCase("ImplementedIn"))
			{
				return true;
			}

			return false;
		}

		/*
		 * Returns true if the requested attribute value is supported;
		 * otherwise, returns false.
		 */
		private static bool IsConstraintSatisfied(String attribute, String value, String prop)
		{
			// For KeySize, prop is the max key size the
			// provider supports for a specific <crypto_service>.<algorithm>.
			if (attribute.EqualsIgnoreCase("KeySize"))
			{
				int requestedSize = Convert.ToInt32(value);
				int maxSize = Convert.ToInt32(prop);
				if (requestedSize <= maxSize)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			// For Type, prop is the type of the implementation
			// for a specific <crypto service>.<algorithm>.
			if (attribute.EqualsIgnoreCase("ImplementedIn"))
			{
				return value.EqualsIgnoreCase(prop);
			}

			return false;
		}

		internal static String[] GetFilterComponents(String filterKey, String filterValue)
		{
			int algIndex = filterKey.IndexOf('.');

			if (algIndex < 0)
			{
				// There must be a dot in the filter, and the dot
				// shouldn't be at the beginning of this string.
				throw new InvalidParameterException("Invalid filter");
			}

			String serviceName = filterKey.Substring(0, algIndex);
			String algName = null;
			String attrName = null;

			if (filterValue.Length() == 0)
			{
				// The filterValue is an empty string. So the filterKey
				// should be in the format of <crypto_service>.<algorithm_or_type>.
				algName = filterKey.Substring(algIndex + 1).Trim();
				if (algName.Length() == 0)
				{
					// There must be a algorithm or type name.
					throw new InvalidParameterException("Invalid filter");
				}
			}
			else
			{
				// The filterValue is a non-empty string. So the filterKey must be
				// in the format of
				// <crypto_service>.<algorithm_or_type> <attribute_name>
				int attrIndex = filterKey.IndexOf(' ');

				if (attrIndex == -1)
				{
					// There is no attribute name in the filter.
					throw new InvalidParameterException("Invalid filter");
				}
				else
				{
					attrName = filterKey.Substring(attrIndex + 1).Trim();
					if (attrName.Length() == 0)
					{
						// There is no attribute name in the filter.
						throw new InvalidParameterException("Invalid filter");
					}
				}

				// There must be an algorithm name in the filter.
				if ((attrIndex < algIndex) || (algIndex == attrIndex - 1))
				{
					throw new InvalidParameterException("Invalid filter");
				}
				else
				{
					algName = StringHelperClass.SubstringSpecial(filterKey, algIndex + 1, attrIndex);
				}
			}

			String[] result = new String[3];
			result[0] = serviceName;
			result[1] = algName;
			result[2] = attrName;

			return result;
		}

		/// <summary>
		/// Returns a Set of Strings containing the names of all available
		/// algorithms or types for the specified Java cryptographic service
		/// (e.g., Signature, MessageDigest, Cipher, Mac, KeyStore). Returns
		/// an empty Set if there is no provider that supports the
		/// specified service or if serviceName is null. For a complete list
		/// of Java cryptographic services, please see the
		/// <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">Java
		/// Cryptography Architecture API Specification &amp; Reference</a>.
		/// Note: the returned set is immutable.
		/// </summary>
		/// <param name="serviceName"> the name of the Java cryptographic
		/// service (e.g., Signature, MessageDigest, Cipher, Mac, KeyStore).
		/// Note: this parameter is case-insensitive.
		/// </param>
		/// <returns> a Set of Strings containing the names of all available
		/// algorithms or types for the specified Java cryptographic service
		/// or an empty set if no provider supports the specified service.
		/// 
		/// @since 1.4
		///  </returns>
		public static Set<String> GetAlgorithms(String serviceName)
		{

			if ((serviceName == null) || (serviceName.Length() == 0) || (serviceName.EndsWith(".")))
			{
				return java.util.Collections.EmptySet();
			}

			HashSet<String> result = new HashSet<String>();
			Provider[] providers = Security.Providers;

			for (int i = 0; i < providers.Length; i++)
			{
				// Check the keys for each provider.
				for (IEnumerator<Object> e = providers[i].Keys(); e.MoveNext();)
				{
					String currentKey = ((String)e.Current).ToUpperCase(Locale.ENGLISH);
					if (currentKey.StartsWith(serviceName.ToUpperCase(Locale.ENGLISH)))
					{
						// We should skip the currentKey if it contains a
						// whitespace. The reason is: such an entry in the
						// provider property contains attributes for the
						// implementation of an algorithm. We are only interested
						// in entries which lead to the implementation
						// classes.
						if (currentKey.IndexOf(" ") < 0)
						{
							result.Add(currentKey.Substring(serviceName.Length() + 1));
						}
					}
				}
			}
			return Collections.UnmodifiableSet(result);
		}
	}

}