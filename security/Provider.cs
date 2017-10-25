using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// This class represents a "provider" for the
	/// Java Security API, where a provider implements some or all parts of
	/// Java Security. Services that a provider may implement include:
	/// 
	/// <ul>
	/// 
	/// <li>Algorithms (such as DSA, RSA, MD5 or SHA-1).
	/// 
	/// <li>Key generation, conversion, and management facilities (such as for
	/// algorithm-specific keys).
	/// 
	/// </ul>
	/// 
	/// <para>Each provider has a name and a version number, and is configured
	/// in each runtime it is installed in.
	/// 
	/// </para>
	/// <para>See <a href =
	/// "../../../technotes/guides/security/crypto/CryptoSpec.html#Provider">The Provider Class</a>
	/// in the "Java Cryptography Architecture API Specification &amp; Reference"
	/// for information about how a particular type of provider, the
	/// cryptographic service provider, works and is installed. However,
	/// please note that a provider can be used to implement any security
	/// service in Java that uses a pluggable architecture with a choice
	/// of implementations that fit underneath.
	/// 
	/// </para>
	/// <para>Some provider implementations may encounter unrecoverable internal
	/// errors during their operation, for example a failure to communicate with a
	/// security token. A <seealso cref="ProviderException"/> should be used to indicate
	/// such errors.
	/// 
	/// </para>
	/// <para>The service type {@code Provider} is reserved for use by the
	/// security framework. Services of this type cannot be added, removed,
	/// or modified by applications.
	/// The following attributes are automatically placed in each Provider object:
	/// <table cellspacing=4>
	/// <caption><b>Attributes Automatically Placed in a Provider Object</b></caption>
	/// <tr><th>Name</th><th>Value</th>
	/// <tr><td>{@code Provider.id name}</td>
	///    <td>{@code String.valueOf(provider.getName())}</td>
	/// <tr><td>{@code Provider.id version}</td>
	///     <td>{@code String.valueOf(provider.getVersion())}</td>
	/// <tr><td>{@code Provider.id info}</td>
	///       <td>{@code String.valueOf(provider.getInfo())}</td>
	/// <tr><td>{@code Provider.id className}</td>
	///     <td>{@code provider.getClass().getName()}</td>
	/// </table>
	/// 
	/// @author Benjamin Renaud
	/// @author Andreas Sterbenz
	/// </para>
	/// </summary>
	public abstract class Provider : Properties
	{

		// Declare serialVersionUID to be compatible with JDK1.1
		internal const long SerialVersionUID = -4298000515446427739L;

		private static readonly sun.security.util.Debug Debug = sun.security.util.Debug.getInstance("provider", "Provider");

		/// <summary>
		/// The provider name.
		/// 
		/// @serial
		/// </summary>
		private String Name_Renamed;

		/// <summary>
		/// A description of the provider and its services.
		/// 
		/// @serial
		/// </summary>
		private String Info_Renamed;

		/// <summary>
		/// The provider version number.
		/// 
		/// @serial
		/// </summary>
		private double Version_Renamed;


		[NonSerialized]
		private Set<Map_Entry<Object, Object>> EntrySet_Renamed = java.util.Map_Fields.Null;
		[NonSerialized]
		private int EntrySetCallCount = 0;

		[NonSerialized]
		private bool Initialized;

		/// <summary>
		/// Constructs a provider with the specified name, version number,
		/// and information.
		/// </summary>
		/// <param name="name"> the provider name.
		/// </param>
		/// <param name="version"> the provider version number.
		/// </param>
		/// <param name="info"> a description of the provider and its services. </param>
		protected internal Provider(String name, double version, String info)
		{
			this.Name_Renamed = name;
			this.Version_Renamed = version;
			this.Info_Renamed = info;
			PutId();
			Initialized = java.util.Map_Fields.True;
		}

		/// <summary>
		/// Returns the name of this provider.
		/// </summary>
		/// <returns> the name of this provider. </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the version number for this provider.
		/// </summary>
		/// <returns> the version number for this provider. </returns>
		public virtual double Version
		{
			get
			{
				return Version_Renamed;
			}
		}

		/// <summary>
		/// Returns a human-readable description of the provider and its
		/// services.  This may return an HTML page, with relevant links.
		/// </summary>
		/// <returns> a description of the provider and its services. </returns>
		public virtual String Info
		{
			get
			{
				return Info_Renamed;
			}
		}

		/// <summary>
		/// Returns a string with the name and the version number
		/// of this provider.
		/// </summary>
		/// <returns> the string with the name and the version number
		/// for this provider. </returns>
		public override String ToString()
		{
			return Name_Renamed + " version " + Version_Renamed;
		}

		/*
		 * override the following methods to ensure that provider
		 * information can only be changed if the caller has the appropriate
		 * permissions.
		 */

		/// <summary>
		/// Clears this provider so that it no longer contains the properties
		/// used to look up facilities implemented by the provider.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "clearProviderProperties."+name}
		/// (where {@code name} is the provider name) to see if it's ok to clear
		/// this provider.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to clear this provider
		/// 
		/// @since 1.2 </exception>
		public override void Clear()
		{
			lock (this)
			{
				Check("clearProviderProperties." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Remove " + Name_Renamed + " provider properties");
				}
				ImplClear();
			}
		}

		/// <summary>
		/// Reads a property list (key and element pairs) from the input stream.
		/// </summary>
		/// <param name="inStream">   the input stream. </param>
		/// <exception cref="IOException">  if an error occurred when reading from the
		///               input stream. </exception>
		/// <seealso cref= java.util.Properties#load </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void load(InputStream inStream) throws IOException
		public override void Load(InputStream inStream)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Load " + Name_Renamed + " provider properties");
				}
				Properties tempProperties = new Properties();
				tempProperties.Load(inStream);
				ImplPutAll(tempProperties);
			}
		}

		/// <summary>
		/// Copies all of the mappings from the specified Map to this provider.
		/// These mappings will replace any properties that this provider had
		/// for any of the keys currently in the specified Map.
		/// 
		/// @since 1.2
		/// </summary>
		public override void putAll<T1>(Map<T1> t)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Put all " + Name_Renamed + " provider properties");
				}
				ImplPutAll(t);
			}
		}

		/// <summary>
		/// Returns an unmodifiable Set view of the property entries contained
		/// in this Provider.
		/// </summary>
		/// <seealso cref=   java.util.Map.Entry
		/// @since 1.2 </seealso>
		public override Set<Map_Entry<Object, Object>> EntrySet()
		{
			lock (this)
			{
				CheckInitialized();
				if (EntrySet_Renamed == java.util.Map_Fields.Null)
				{
					if (EntrySetCallCount++ == 0) // Initial call
					{
						EntrySet_Renamed = Collections.UnmodifiableMap(this).EntrySet();
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
						return base.EntrySet(); // Recursive call
					}
				}
        
				// This exception will be thrown if the implementation of
				// Collections.unmodifiableMap.entrySet() is changed such that it
				// no longer calls entrySet() on the backing Map.  (Provider's
				// entrySet implementation depends on this "implementation detail",
				// which is unlikely to change.
				if (EntrySetCallCount != 2)
				{
					throw new RuntimeException("Internal error.");
				}
        
				return EntrySet_Renamed;
			}
		}

		/// <summary>
		/// Returns an unmodifiable Set view of the property keys contained in
		/// this provider.
		/// 
		/// @since 1.2
		/// </summary>
		public override Set<Object> KeySet()
		{
			CheckInitialized();
			return Collections.UnmodifiableSet(base.KeySet());
		}

		/// <summary>
		/// Returns an unmodifiable Collection view of the property values
		/// contained in this provider.
		/// 
		/// @since 1.2
		/// </summary>
		public override Collection<Object> Values()
		{
			CheckInitialized();
			return Collections.UnmodifiableCollection(base.Values);
		}

		/// <summary>
		/// Sets the {@code key} property to have the specified
		/// {@code value}.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "putProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to set this
		/// provider's property values.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values.
		/// 
		/// @since 1.2 </exception>
		public override Object Put(Object key, Object value)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Set " + Name_Renamed + " provider property [" + key + "/" + value + "]");
				}
				return ImplPut(key, value);
			}
		}

		/// <summary>
		/// If the specified key is not already associated with a value (or is mapped
		/// to {@code null}) associates it with the given value and returns
		/// {@code null}, else returns the current value.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "putProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to set this
		/// provider's property values.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values.
		/// 
		/// @since 1.8 </exception>
		public override Object PutIfAbsent(Object key, Object value)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Set " + Name_Renamed + " provider property [" + key + "/" + value + "]");
				}
				return ImplPutIfAbsent(key, value);
			}
		}

		/// <summary>
		/// Removes the {@code key} property (and its corresponding
		/// {@code value}).
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "removeProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to remove this
		/// provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to remove this provider's properties.
		/// 
		/// @since 1.2 </exception>
		public override Object Remove(Object key)
		{
			lock (this)
			{
				Check("removeProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Remove " + Name_Renamed + " provider property " + key);
				}
				return ImplRemove(key);
			}
		}

		/// <summary>
		/// Removes the entry for the specified key only if it is currently
		/// mapped to the specified value.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "removeProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to remove this
		/// provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to remove this provider's properties.
		/// 
		/// @since 1.8 </exception>
		public override bool Remove(Object key, Object value)
		{
			lock (this)
			{
				Check("removeProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Remove " + Name_Renamed + " provider property " + key);
				}
				return ImplRemove(key, value);
			}
		}

		/// <summary>
		/// Replaces the entry for the specified key only if currently
		/// mapped to the specified value.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "putProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to set this
		/// provider's property values.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values.
		/// 
		/// @since 1.8 </exception>
		public override bool Replace(Object key, Object java, Object java)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Replace " + Name_Renamed + " provider property " + key);
				}
				return ImplReplace(key, java.util.Map_Fields.OldValue, java.util.Map_Fields.NewValue);
			}
		}

		/// <summary>
		/// Replaces the entry for the specified key only if it is
		/// currently mapped to some value.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "putProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to set this
		/// provider's property values.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values.
		/// 
		/// @since 1.8 </exception>
		public override Object Replace(Object key, Object value)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Replace " + Name_Renamed + " provider property " + key);
				}
				return ImplReplace(key, value);
			}
		}

		/// <summary>
		/// Replaces each entry's value with the result of invoking the given
		/// function on that entry, in the order entries are returned by an entry
		/// set iterator, until all entries have been processed or the function
		/// throws an exception.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the string {@code "putProviderProperty."+name},
		/// where {@code name} is the provider name, to see if it's ok to set this
		/// provider's property values.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized void replaceAll(java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> function)
		public override void replaceAll<T1>(BiFunction<T1> function) where T1 : Object
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("ReplaceAll " + Name_Renamed + " provider property ");
				}
				ImplReplaceAll(function);
			}
		}

		/// <summary>
		/// Attempts to compute a mapping for the specified key and its
		/// current mapped value (or {@code null} if there is no current
		/// mapping).
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the strings {@code "putProviderProperty."+name}
		/// and {@code "removeProviderProperty."+name}, where {@code name} is the
		/// provider name, to see if it's ok to set this provider's property values
		/// and remove this provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values or remove properties.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized Object compute(Object key, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		public override Object compute<T1>(Object key, BiFunction<T1> remappingFunction) where T1 : Object
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				Check("removeProviderProperty" + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Compute " + Name_Renamed + " provider property " + key);
				}
				return ImplCompute(key, remappingFunction);
			}
		}

		/// <summary>
		/// If the specified key is not already associated with a value (or
		/// is mapped to {@code null}), attempts to compute its value using
		/// the given mapping function and enters it into this map unless
		/// {@code null}.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the strings {@code "putProviderProperty."+name}
		/// and {@code "removeProviderProperty."+name}, where {@code name} is the
		/// provider name, to see if it's ok to set this provider's property values
		/// and remove this provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values and remove properties.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized Object computeIfAbsent(Object key, java.util.function.Function<? base Object, ? extends Object> mappingFunction)
		public override Object computeIfAbsent<T1>(Object key, Function<T1> mappingFunction) where T1 : Object
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				Check("removeProviderProperty" + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("ComputeIfAbsent " + Name_Renamed + " provider property " + key);
				}
				return ImplComputeIfAbsent(key, mappingFunction);
			}
		}

		/// <summary>
		/// If the value for the specified key is present and non-null, attempts to
		/// compute a new mapping given the key and its current mapped value.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the strings {@code "putProviderProperty."+name}
		/// and {@code "removeProviderProperty."+name}, where {@code name} is the
		/// provider name, to see if it's ok to set this provider's property values
		/// and remove this provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values or remove properties.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized Object computeIfPresent(Object key, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		public override Object computeIfPresent<T1>(Object key, BiFunction<T1> remappingFunction) where T1 : Object
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				Check("removeProviderProperty" + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("ComputeIfPresent " + Name_Renamed + " provider property " + key);
				}
				return ImplComputeIfPresent(key, remappingFunction);
			}
		}

		/// <summary>
		/// If the specified key is not already associated with a value or is
		/// associated with null, associates it with the given value. Otherwise,
		/// replaces the value with the results of the given remapping function,
		/// or removes if the result is null. This method may be of use when
		/// combining multiple mapped values for a key.
		/// 
		/// <para>If a security manager is enabled, its {@code checkSecurityAccess}
		/// method is called with the strings {@code "putProviderProperty."+name}
		/// and {@code "removeProviderProperty."+name}, where {@code name} is the
		/// provider name, to see if it's ok to set this provider's property values
		/// and remove this provider's properties.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method
		///          denies access to set property values or remove properties.
		/// 
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized Object merge(Object key, Object value, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		public override Object merge<T1>(Object key, Object value, BiFunction<T1> remappingFunction) where T1 : Object
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				Check("removeProviderProperty" + Name_Renamed);
        
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Merge " + Name_Renamed + " provider property " + key);
				}
				return ImplMerge(key, value, remappingFunction);
			}
		}

		// let javadoc show doc from superclass
		public override Object Get(Object key)
		{
			CheckInitialized();
			return base[key];
		}
		/// <summary>
		/// @since 1.8
		/// </summary>
		public override Object GetOrDefault(Object key, Object defaultValue)
		{
			lock (this)
			{
				CheckInitialized();
				return base.GetOrDefault(key, defaultValue);
			}
		}

		/// <summary>
		/// @since 1.8
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized void forEach(java.util.function.BiConsumer<? base Object, ? base Object> action)
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			lock (this)
			{
				CheckInitialized();
				base.ForEach(action);
			}
		}

		// let javadoc show doc from superclass
		public override IEnumerator<Object> Keys()
		{
			CheckInitialized();
			return base.Keys.GetEnumerator();
		}

		// let javadoc show doc from superclass
		public override IEnumerator<Object> Elements()
		{
			CheckInitialized();
			return base.Values.GetEnumerator();
		}

		// let javadoc show doc from superclass
		public override String GetProperty(String key)
		{
			CheckInitialized();
			return base.GetProperty(key);
		}

		private void CheckInitialized()
		{
			if (!Initialized)
			{
				throw new IllegalStateException();
			}
		}

		private void Check(String directive)
		{
			CheckInitialized();
			SecurityManager security = System.SecurityManager;
			if (security != java.util.Map_Fields.Null)
			{
				security.CheckSecurityAccess(directive);
			}
		}

		// legacy properties changed since last call to any services method?
		[NonSerialized]
		private bool LegacyChanged;
		// serviceMap changed since last call to getServices()
		[NonSerialized]
		private bool ServicesChanged;

		// Map<String,String>
		[NonSerialized]
		private Map<String, String> LegacyStrings;

		// Map<ServiceKey,Service>
		// used for services added via putService(), initialized on demand
		[NonSerialized]
		private Map<ServiceKey, Service> ServiceMap;

		// Map<ServiceKey,Service>
		// used for services added via legacy methods, init on demand
		[NonSerialized]
		private Map<ServiceKey, Service> LegacyMap;

		// Set<Service>
		// Unmodifiable set of all services. Initialized on demand.
		[NonSerialized]
		private Set<Service> ServiceSet;

		// register the id attributes for this provider
		// this is to ensure that equals() and hashCode() do not incorrectly
		// report to different provider objects as the same
		private void PutId()
		{
			// note: name and info may be null
			base["Provider.id name"] = Convert.ToString(Name_Renamed);
			base["Provider.id version"] = Convert.ToString(Version_Renamed);
			base["Provider.id info"] = Convert.ToString(Info_Renamed);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			base["Provider.id className"] = this.GetType().FullName;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream in) throws IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			Map<Object, Object> copy = new HashMap<Object, Object>();
			foreach (Map_Entry<Object, Object> entry in base)
			{
				copy.Put(entry.Key, entry.Value);
			}
			Defaults = java.util.Map_Fields.Null;
			@in.DefaultReadObject();
			ImplClear();
			Initialized = java.util.Map_Fields.True;
			PutAll(copy);
		}

		private bool CheckLegacy(Object key)
		{
			String keyString = (String)key;
			if (keyString.StartsWith("Provider."))
			{
				return java.util.Map_Fields.False;
			}

			LegacyChanged = java.util.Map_Fields.True;
			if (LegacyStrings == java.util.Map_Fields.Null)
			{
				LegacyStrings = new LinkedHashMap<String, String>();
			}
			return java.util.Map_Fields.True;
		}

		/// <summary>
		/// Copies all of the mappings from the specified Map to this provider.
		/// Internal method to be called AFTER the security check has been
		/// performed.
		/// </summary>
		private void implPutAll<T1>(Map<T1> t)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<?,?> e : t.entrySet())
			foreach (Map_Entry<?, ?> e in t.EntrySet())
			{
				ImplPut(e.Key, e.Value);
			}
		}

		private Object ImplRemove(Object key)
		{
			if (key is String)
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
				LegacyStrings.Remove((String)key);
			}
			return base.Remove(key);
		}

		private bool ImplRemove(Object key, Object value)
		{
			if (key is String && value is String)
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.False;
				}
				LegacyStrings.remove((String)key, value);
			}
			return base.Remove(key, value);
		}

		private bool ImplReplace(Object key, Object java, Object java)
		{
			if ((key is String) && (java.util.Map_Fields.OldValue is String) && (java.util.Map_Fields.NewValue is String))
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.False;
				}
				LegacyStrings.replace((String)key, (String)java.util.Map_Fields.OldValue, (String)java.util.Map_Fields.NewValue);
			}
			return base.Replace(key, java.util.Map_Fields.OldValue, java.util.Map_Fields.NewValue);
		}

		private Object ImplReplace(Object key, Object value)
		{
			if ((key is String) && (value is String))
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
				LegacyStrings.replace((String)key, (String)value);
			}
			return base.Replace(key, value);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private void implReplaceAll(java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> function)
		private void implReplaceAll<T1>(BiFunction<T1> function) where T1 : Object
		{
			LegacyChanged = java.util.Map_Fields.True;
			if (LegacyStrings == java.util.Map_Fields.Null)
			{
				LegacyStrings = new LinkedHashMap<String, String>();
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: legacyStrings.replaceAll((java.util.function.BiFunction<? base String, ? base String, ? extends String>) function);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				LegacyStrings.replaceAll((BiFunction<?, ?, ?>) function);
			}
			base.ReplaceAll(function);
		}


//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private Object implMerge(Object key, Object value, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		private Object implMerge<T1>(Object key, Object value, BiFunction<T1> remappingFunction) where T1 : Object
		{
			if ((key is String) && (value is String))
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: legacyStrings.merge((String)key, (String)value, (java.util.function.BiFunction<? base String, ? base String, ? extends String>) remappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				LegacyStrings.merge((String)key, (String)value, (BiFunction<?, ?, ?>) remappingFunction);
			}
			return base.Merge(key, value, remappingFunction);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private Object implCompute(Object key, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		private Object implCompute<T1>(Object key, BiFunction<T1> remappingFunction) where T1 : Object
		{
			if (key is String)
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: legacyStrings.computeIfAbsent((String) key, (java.util.function.Function<? base String, ? extends String>) remappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				LegacyStrings.computeIfAbsent((String) key, (Function<?, ?>) remappingFunction);
			}
			return base.Compute(key, remappingFunction);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private Object implComputeIfAbsent(Object key, java.util.function.Function<? base Object, ? extends Object> mappingFunction)
		private Object implComputeIfAbsent<T1>(Object key, Function<T1> mappingFunction) where T1 : Object
		{
			if (key is String)
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: legacyStrings.computeIfAbsent((String) key, (java.util.function.Function<? base String, ? extends String>) mappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				LegacyStrings.computeIfAbsent((String) key, (Function<?, ?>) mappingFunction);
			}
			return base.ComputeIfAbsent(key, mappingFunction);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private Object implComputeIfPresent(Object key, java.util.function.BiFunction<? base Object, ? base Object, ? extends Object> remappingFunction)
		private Object implComputeIfPresent<T1>(Object key, BiFunction<T1> remappingFunction) where T1 : Object
		{
			if (key is String)
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: legacyStrings.computeIfPresent((String) key, (java.util.function.BiFunction<? base String, ? base String, ? extends String>) remappingFunction);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				LegacyStrings.computeIfPresent((String) key, (BiFunction<?, ?, ?>) remappingFunction);
			}
			return base.ComputeIfPresent(key, remappingFunction);
		}

		private Object ImplPut(Object key, Object value)
		{
			if ((key is String) && (value is String))
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
				LegacyStrings.Put((String)key, (String)value);
			}
			return base[key] = value;
		}

		private Object ImplPutIfAbsent(Object key, Object value)
		{
			if ((key is String) && (value is String))
			{
				if (!CheckLegacy(key))
				{
					return java.util.Map_Fields.Null;
				}
				LegacyStrings.putIfAbsent((String)key, (String)value);
			}
			return base.PutIfAbsent(key, value);
		}

		private void ImplClear()
		{
			if (LegacyStrings != java.util.Map_Fields.Null)
			{
				LegacyStrings.Clear();
			}
			if (LegacyMap != java.util.Map_Fields.Null)
			{
				LegacyMap.Clear();
			}
			if (ServiceMap != java.util.Map_Fields.Null)
			{
				ServiceMap.Clear();
			}
			LegacyChanged = java.util.Map_Fields.False;
			ServicesChanged = java.util.Map_Fields.False;
			ServiceSet = java.util.Map_Fields.Null;
			base.Clear();
			PutId();
		}

		// used as key in the serviceMap and legacyMap HashMaps
		private class ServiceKey
		{
			internal readonly String Type;
			internal readonly String Algorithm;
			internal readonly String OriginalAlgorithm;
			internal ServiceKey(String type, String algorithm, bool intern)
			{
				this.Type = type;
				this.OriginalAlgorithm = algorithm;
				algorithm = algorithm.ToUpperCase(ENGLISH);
				this.Algorithm = intern ? algorithm.intern() : algorithm;
			}
			public override int HashCode()
			{
				return Type.HashCode() + Algorithm.HashCode();
			}
			public override bool Equals(Object obj)
			{
				if (this == obj)
				{
					return java.util.Map_Fields.True;
				}
				if (obj is ServiceKey == java.util.Map_Fields.False)
				{
					return java.util.Map_Fields.False;
				}
				ServiceKey other = (ServiceKey)obj;
				return this.Type.Equals(other.Type) && this.Algorithm.Equals(other.Algorithm);
			}
			internal virtual bool Matches(String type, String algorithm)
			{
				return (this.Type == type) && (this.OriginalAlgorithm == algorithm);
			}
		}

		/// <summary>
		/// Ensure all the legacy String properties are fully parsed into
		/// service objects.
		/// </summary>
		private void EnsureLegacyParsed()
		{
			if ((LegacyChanged == java.util.Map_Fields.False) || (LegacyStrings == java.util.Map_Fields.Null))
			{
				return;
			}
			ServiceSet = java.util.Map_Fields.Null;
			if (LegacyMap == java.util.Map_Fields.Null)
			{
				LegacyMap = new LinkedHashMap<ServiceKey, Service>();
			}
			else
			{
				LegacyMap.Clear();
			}
			foreach (Map_Entry<String, String> entry in LegacyStrings.EntrySet())
			{
				ParseLegacyPut(entry.Key, entry.Value);
			}
			RemoveInvalidServices(LegacyMap);
			LegacyChanged = java.util.Map_Fields.False;
		}

		/// <summary>
		/// Remove all invalid services from the Map. Invalid services can only
		/// occur if the legacy properties are inconsistent or incomplete.
		/// </summary>
		private void RemoveInvalidServices(Map<ServiceKey, Service> map)
		{
			for (Iterator<Map_Entry<ServiceKey, Service>> t = map.EntrySet().Iterator(); t.HasNext();)
			{
				Service s = t.Next().Value;
				if (s.Valid == java.util.Map_Fields.False)
				{
					t.remove();
				}
			}
		}

		private String[] GetTypeAndAlgorithm(String key)
		{
			int i = key.IndexOf(".");
			if (i < 1)
			{
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println("Ignoring invalid entry in provider " + Name_Renamed + ":" + key);
				}
				return java.util.Map_Fields.Null;
			}
			String type = key.Substring(0, i);
			String alg = key.Substring(i + 1);
			return new String[] {type, alg};
		}

		private const String ALIAS_PREFIX = "Alg.Alias.";
		private const String ALIAS_PREFIX_LOWER = "alg.alias.";
		private static readonly int ALIAS_LENGTH = ALIAS_PREFIX.Length();

		private void ParseLegacyPut(String name, String value)
		{
			if (name.ToLowerCase(ENGLISH).StartsWith(ALIAS_PREFIX_LOWER))
			{
				// e.g. put("Alg.Alias.MessageDigest.SHA", "SHA-1");
				// aliasKey ~ MessageDigest.SHA
				String stdAlg = value;
				String aliasKey = name.Substring(ALIAS_LENGTH);
				String[] typeAndAlg = GetTypeAndAlgorithm(aliasKey);
				if (typeAndAlg == java.util.Map_Fields.Null)
				{
					return;
				}
				String type = GetEngineName(typeAndAlg[0]);
				String aliasAlg = typeAndAlg[1].intern();
				ServiceKey key = new ServiceKey(type, stdAlg, java.util.Map_Fields.True);
				Service s = LegacyMap.Get(key);
				if (s == java.util.Map_Fields.Null)
				{
					s = new Service(this);
					s.Type_Renamed = type;
					s.Algorithm_Renamed = stdAlg;
					LegacyMap.Put(key, s);
				}
				LegacyMap.Put(new ServiceKey(type, aliasAlg, java.util.Map_Fields.True), s);
				s.AddAlias(aliasAlg);
			}
			else
			{
				String[] typeAndAlg = GetTypeAndAlgorithm(name);
				if (typeAndAlg == java.util.Map_Fields.Null)
				{
					return;
				}
				int i = typeAndAlg[1].IndexOf(' ');
				if (i == -1)
				{
					// e.g. put("MessageDigest.SHA-1", "sun.security.provider.SHA");
					String type = GetEngineName(typeAndAlg[0]);
					String stdAlg = typeAndAlg[1].intern();
					String className = value;
					ServiceKey key = new ServiceKey(type, stdAlg, java.util.Map_Fields.True);
					Service s = LegacyMap.Get(key);
					if (s == java.util.Map_Fields.Null)
					{
						s = new Service(this);
						s.Type_Renamed = type;
						s.Algorithm_Renamed = stdAlg;
						LegacyMap.Put(key, s);
					}
					s.ClassName_Renamed = className;
				} // attribute
				else
				{
					// e.g. put("MessageDigest.SHA-1 ImplementedIn", "Software");
					String attributeValue = value;
					String type = GetEngineName(typeAndAlg[0]);
					String attributeString = typeAndAlg[1];
					String stdAlg = attributeString.Substring(0, i).intern();
					String attributeName = attributeString.Substring(i + 1);
					// kill additional spaces
					while (attributeName.StartsWith(" "))
					{
						attributeName = attributeName.Substring(1);
					}
					attributeName = attributeName.intern();
					ServiceKey key = new ServiceKey(type, stdAlg, java.util.Map_Fields.True);
					Service s = LegacyMap.Get(key);
					if (s == java.util.Map_Fields.Null)
					{
						s = new Service(this);
						s.Type_Renamed = type;
						s.Algorithm_Renamed = stdAlg;
						LegacyMap.Put(key, s);
					}
					s.AddAttribute(attributeName, attributeValue);
				}
			}
		}

		/// <summary>
		/// Get the service describing this Provider's implementation of the
		/// specified type of this algorithm or alias. If no such
		/// implementation exists, this method returns null. If there are two
		/// matching services, one added to this provider using
		/// <seealso cref="#putService putService()"/> and one added via <seealso cref="#put put()"/>,
		/// the service added via <seealso cref="#putService putService()"/> is returned.
		/// </summary>
		/// <param name="type"> the type of <seealso cref="Service service"/> requested
		/// (for example, {@code MessageDigest}) </param>
		/// <param name="algorithm"> the case insensitive algorithm name (or alternate
		/// alias) of the service requested (for example, {@code SHA-1})
		/// </param>
		/// <returns> the service describing this Provider's matching service
		/// or null if no such service exists
		/// </returns>
		/// <exception cref="NullPointerException"> if type or algorithm is null
		/// 
		/// @since 1.5 </exception>
		public virtual Service GetService(String type, String algorithm)
		{
			lock (this)
			{
				CheckInitialized();
				// avoid allocating a new key object if possible
				ServiceKey key = PreviousKey;
				if (key.Matches(type, algorithm) == java.util.Map_Fields.False)
				{
					key = new ServiceKey(type, algorithm, java.util.Map_Fields.False);
					PreviousKey = key;
				}
				if (ServiceMap != java.util.Map_Fields.Null)
				{
					Service service = ServiceMap.Get(key);
					if (service != java.util.Map_Fields.Null)
					{
						return service;
					}
				}
				EnsureLegacyParsed();
				return (LegacyMap != java.util.Map_Fields.Null) ? LegacyMap.Get(key) : java.util.Map_Fields.Null;
			}
		}

		// ServiceKey from previous getService() call
		// by re-using it if possible we avoid allocating a new object
		// and the toUpperCase() call.
		// re-use will occur e.g. as the framework traverses the provider
		// list and queries each provider with the same values until it finds
		// a matching service
		private static volatile ServiceKey PreviousKey = new ServiceKey("", "", java.util.Map_Fields.False);

		/// <summary>
		/// Get an unmodifiable Set of all services supported by
		/// this Provider.
		/// </summary>
		/// <returns> an unmodifiable Set of all services supported by
		/// this Provider
		/// 
		/// @since 1.5 </returns>
		public virtual Set<Service> Services
		{
			get
			{
				lock (this)
				{
					CheckInitialized();
					if (LegacyChanged || ServicesChanged)
					{
						ServiceSet = java.util.Map_Fields.Null;
					}
					if (ServiceSet == java.util.Map_Fields.Null)
					{
						EnsureLegacyParsed();
						Set<Service> set = new LinkedHashSet<Service>();
						if (ServiceMap != java.util.Map_Fields.Null)
						{
							set.AddAll(ServiceMap.Values());
						}
						if (LegacyMap != java.util.Map_Fields.Null)
						{
							set.AddAll(LegacyMap.Values());
						}
						ServiceSet = Collections.UnmodifiableSet(set);
						ServicesChanged = java.util.Map_Fields.False;
					}
					return ServiceSet;
				}
			}
		}

		/// <summary>
		/// Add a service. If a service of the same type with the same algorithm
		/// name exists and it was added using <seealso cref="#putService putService()"/>,
		/// it is replaced by the new service.
		/// This method also places information about this service
		/// in the provider's Hashtable values in the format described in the
		/// <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
		/// Java Cryptography Architecture API Specification &amp; Reference </a>.
		/// 
		/// <para>Also, if there is a security manager, its
		/// {@code checkSecurityAccess} method is called with the string
		/// {@code "putProviderProperty."+name}, where {@code name} is
		/// the provider name, to see if it's ok to set this provider's property
		/// values. If the default implementation of {@code checkSecurityAccess}
		/// is used (that is, that method is not overriden), then this results in
		/// a call to the security manager's {@code checkPermission} method with
		/// a {@code SecurityPermission("putProviderProperty."+name)}
		/// permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the Service to add
		/// </param>
		/// <exception cref="SecurityException">
		///      if a security manager exists and its {@link
		///      java.lang.SecurityManager#checkSecurityAccess} method denies
		///      access to set property values. </exception>
		/// <exception cref="NullPointerException"> if s is null
		/// 
		/// @since 1.5 </exception>
		protected internal virtual void PutService(Service s)
		{
			lock (this)
			{
				Check("putProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println(Name_Renamed + ".putService(): " + s);
				}
				if (s == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (s.Provider != this)
				{
					throw new IllegalArgumentException("service.getProvider() must match this Provider object");
				}
				if (ServiceMap == java.util.Map_Fields.Null)
				{
					ServiceMap = new LinkedHashMap<ServiceKey, Service>();
				}
				ServicesChanged = java.util.Map_Fields.True;
				String type = s.Type;
				String algorithm = s.Algorithm;
				ServiceKey key = new ServiceKey(type, algorithm, java.util.Map_Fields.True);
				// remove existing service
				ImplRemoveService(ServiceMap.Get(key));
				ServiceMap.Put(key, s);
				foreach (String alias in s.Aliases)
				{
					ServiceMap.Put(new ServiceKey(type, alias, java.util.Map_Fields.True), s);
				}
				PutPropertyStrings(s);
			}
		}

		/// <summary>
		/// Put the string properties for this Service in this Provider's
		/// Hashtable.
		/// </summary>
		private void PutPropertyStrings(Service s)
		{
			String type = s.Type;
			String algorithm = s.Algorithm;
			// use super() to avoid permission check and other processing
			base[type + "." + algorithm] = s.ClassName;
			foreach (String alias in s.Aliases)
			{
				base[ALIAS_PREFIX + type + "." + alias] = algorithm;
			}
			foreach (Map_Entry<UString, String> entry in s.Attributes.EntrySet())
			{
				String key = type + "." + algorithm + " " + entry.Key;
				base[key] = entry.Value;
			}
		}

		/// <summary>
		/// Remove the string properties for this Service from this Provider's
		/// Hashtable.
		/// </summary>
		private void RemovePropertyStrings(Service s)
		{
			String type = s.Type;
			String algorithm = s.Algorithm;
			// use super() to avoid permission check and other processing
			base.Remove(type + "." + algorithm);
			foreach (String alias in s.Aliases)
			{
				base.Remove(ALIAS_PREFIX + type + "." + alias);
			}
			foreach (Map_Entry<UString, String> entry in s.Attributes.EntrySet())
			{
				String key = type + "." + algorithm + " " + entry.Key;
				base.Remove(key);
			}
		}

		/// <summary>
		/// Remove a service previously added using
		/// <seealso cref="#putService putService()"/>. The specified service is removed from
		/// this provider. It will no longer be returned by
		/// <seealso cref="#getService getService()"/> and its information will be removed
		/// from this provider's Hashtable.
		/// 
		/// <para>Also, if there is a security manager, its
		/// {@code checkSecurityAccess} method is called with the string
		/// {@code "removeProviderProperty."+name}, where {@code name} is
		/// the provider name, to see if it's ok to remove this provider's
		/// properties. If the default implementation of
		/// {@code checkSecurityAccess} is used (that is, that method is not
		/// overriden), then this results in a call to the security manager's
		/// {@code checkPermission} method with a
		/// {@code SecurityPermission("removeProviderProperty."+name)}
		/// permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the Service to be removed
		/// </param>
		/// <exception cref="SecurityException">
		///          if a security manager exists and its {@link
		///          java.lang.SecurityManager#checkSecurityAccess} method denies
		///          access to remove this provider's properties. </exception>
		/// <exception cref="NullPointerException"> if s is null
		/// 
		/// @since 1.5 </exception>
		protected internal virtual void RemoveService(Service s)
		{
			lock (this)
			{
				Check("removeProviderProperty." + Name_Renamed);
				if (Debug != java.util.Map_Fields.Null)
				{
					Debug.println(Name_Renamed + ".removeService(): " + s);
				}
				if (s == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				ImplRemoveService(s);
			}
		}

		private void ImplRemoveService(Service s)
		{
			if ((s == java.util.Map_Fields.Null) || (ServiceMap == java.util.Map_Fields.Null))
			{
				return;
			}
			String type = s.Type;
			String algorithm = s.Algorithm;
			ServiceKey key = new ServiceKey(type, algorithm, java.util.Map_Fields.False);
			Service oldService = ServiceMap.Get(key);
			if (s != oldService)
			{
				return;
			}
			ServicesChanged = java.util.Map_Fields.True;
			ServiceMap.Remove(key);
			foreach (String alias in s.Aliases)
			{
				ServiceMap.Remove(new ServiceKey(type, alias, java.util.Map_Fields.False));
			}
			RemovePropertyStrings(s);
		}

		// Wrapped String that behaves in a case insensitive way for equals/hashCode
		private class UString
		{
			internal readonly String @string;
			internal readonly String LowerString;

			internal UString(String s)
			{
				this.@string = s;
				this.LowerString = s.ToLowerCase(ENGLISH);
			}

			public override int HashCode()
			{
				return LowerString.HashCode();
			}

			public override bool Equals(Object obj)
			{
				if (this == obj)
				{
					return java.util.Map_Fields.True;
				}
				if (obj is UString == java.util.Map_Fields.False)
				{
					return java.util.Map_Fields.False;
				}
				UString other = (UString)obj;
				return LowerString.Equals(other.LowerString);
			}

			public override String ToString()
			{
				return @string;
			}
		}

		// describe relevant properties of a type of engine
		private class EngineDescription
		{
			internal readonly String Name;
			internal readonly bool SupportsParameter;
			internal readonly String ConstructorParameterClassName;
			internal volatile Class ConstructorParameterClass_Renamed;

			internal EngineDescription(String name, bool sp, String paramName)
			{
				this.Name = name;
				this.SupportsParameter = sp;
				this.ConstructorParameterClassName = paramName;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Class getConstructorParameterClass() throws ClassNotFoundException
			internal virtual Class ConstructorParameterClass
			{
				get
				{
					Class clazz = ConstructorParameterClass_Renamed;
					if (clazz == java.util.Map_Fields.Null)
					{
						clazz = Class.ForName(ConstructorParameterClassName);
						ConstructorParameterClass_Renamed = clazz;
					}
					return clazz;
				}
			}
		}

		// built in knowledge of the engine types shipped as part of the JDK
		private static readonly Map<String, EngineDescription> KnownEngines;

		private static void AddEngine(String name, bool sp, String paramName)
		{
			EngineDescription ed = new EngineDescription(name, sp, paramName);
			// also index by canonical name to avoid toLowerCase() for some lookups
			KnownEngines.Put(name.ToLowerCase(ENGLISH), ed);
			KnownEngines.Put(name, ed);
		}

		static Provider()
		{
			KnownEngines = new HashMap<String, EngineDescription>();
			// JCA
			AddEngine("AlgorithmParameterGenerator", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("AlgorithmParameters", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("KeyFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("KeyPairGenerator", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("KeyStore", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("MessageDigest", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("SecureRandom", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("Signature", java.util.Map_Fields.True, java.util.Map_Fields.Null);
			AddEngine("CertificateFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("CertPathBuilder", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("CertPathValidator", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("CertStore", java.util.Map_Fields.False, "java.security.cert.CertStoreParameters");
			// JCE
			AddEngine("Cipher", java.util.Map_Fields.True, java.util.Map_Fields.Null);
			AddEngine("ExemptionMechanism", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("Mac", java.util.Map_Fields.True, java.util.Map_Fields.Null);
			AddEngine("KeyAgreement", java.util.Map_Fields.True, java.util.Map_Fields.Null);
			AddEngine("KeyGenerator", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("SecretKeyFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			// JSSE
			AddEngine("KeyManagerFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("SSLContext", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("TrustManagerFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			// JGSS
			AddEngine("GssApiMechanism", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			// SASL
			AddEngine("SaslClientFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("SaslServerFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			// POLICY
			AddEngine("Policy", java.util.Map_Fields.False, "java.security.Policy$Parameters");
			// CONFIGURATION
			AddEngine("Configuration", java.util.Map_Fields.False, "javax.security.auth.login.Configuration$Parameters");
			// XML DSig
			AddEngine("XMLSignatureFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("KeyInfoFactory", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			AddEngine("TransformService", java.util.Map_Fields.False, java.util.Map_Fields.Null);
			// Smart Card I/O
			AddEngine("TerminalFactory", java.util.Map_Fields.False, "java.lang.Object");
		}

		// get the "standard" (mixed-case) engine name for arbitary case engine name
		// if there is no known engine by that name, return s
		private static String GetEngineName(String s)
		{
			// try original case first, usually correct
			EngineDescription e = KnownEngines.Get(s);
			if (e == java.util.Map_Fields.Null)
			{
				e = KnownEngines.Get(s.ToLowerCase(ENGLISH));
			}
			return (e == java.util.Map_Fields.Null) ? s : e.Name;
		}

		/// <summary>
		/// The description of a security service. It encapsulates the properties
		/// of a service and contains a factory method to obtain new implementation
		/// instances of this service.
		/// 
		/// <para>Each service has a provider that offers the service, a type,
		/// an algorithm name, and the name of the class that implements the
		/// service. Optionally, it also includes a list of alternate algorithm
		/// names for this service (aliases) and attributes, which are a map of
		/// (name, value) String pairs.
		/// 
		/// </para>
		/// <para>This class defines the methods {@link #supportsParameter
		/// supportsParameter()} and <seealso cref="#newInstance newInstance()"/>
		/// which are used by the Java security framework when it searches for
		/// suitable services and instantiates them. The valid arguments to those
		/// methods depend on the type of service. For the service types defined
		/// within Java SE, see the
		/// <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
		/// Java Cryptography Architecture API Specification &amp; Reference </a>
		/// for the valid values.
		/// Note that components outside of Java SE can define additional types of
		/// services and their behavior.
		/// 
		/// </para>
		/// <para>Instances of this class are immutable.
		/// 
		/// @since 1.5
		/// </para>
		/// </summary>
		public class Service
		{

			internal String Type_Renamed, Algorithm_Renamed, ClassName_Renamed;
			internal readonly Provider Provider_Renamed;
			internal List<String> Aliases_Renamed;
			internal Map<UString, String> Attributes;

			// Reference to the cached implementation Class object
			internal volatile Reference<Class> ClassRef;

			// flag indicating whether this service has its attributes for
			// supportedKeyFormats or supportedKeyClasses set
			// if null, the values have not been initialized
			// if TRUE, at least one of supportedFormats/Classes is non null
			internal volatile Boolean HasKeyAttributes_Renamed;

			// supported encoding formats
			internal String[] SupportedFormats;

			// names of the supported key (super) classes
			internal Class[] SupportedClasses;

			// whether this service has been registered with the Provider
			internal bool Registered;

			internal static readonly Class[] CLASS0 = new Class[0];

			// this constructor and these methods are used for parsing
			// the legacy string properties.

			internal Service(Provider provider)
			{
				this.Provider_Renamed = provider;
				Aliases_Renamed = System.Linq.Enumerable.Empty<String>();
				Attributes = System.Linq.Enumerable.Empty<UString, String>();
			}

			internal virtual bool Valid
			{
				get
				{
					return (Type_Renamed != java.util.Map_Fields.Null) && (Algorithm_Renamed != java.util.Map_Fields.Null) && (ClassName_Renamed != java.util.Map_Fields.Null);
				}
			}

			internal virtual void AddAlias(String alias)
			{
				if (Aliases_Renamed.Count == 0)
				{
					Aliases_Renamed = new List<String>(2);
				}
				Aliases_Renamed.Add(alias);
			}

			internal virtual void AddAttribute(String type, String value)
			{
				if (Attributes.Empty)
				{
					Attributes = new HashMap<UString, String>(8);
				}
				Attributes.Put(new UString(type), value);
			}

			/// <summary>
			/// Construct a new service.
			/// </summary>
			/// <param name="provider"> the provider that offers this service </param>
			/// <param name="type"> the type of this service </param>
			/// <param name="algorithm"> the algorithm name </param>
			/// <param name="className"> the name of the class implementing this service </param>
			/// <param name="aliases"> List of aliases or null if algorithm has no aliases </param>
			/// <param name="attributes"> Map of attributes or null if this implementation
			///                   has no attributes
			/// </param>
			/// <exception cref="NullPointerException"> if provider, type, algorithm, or
			/// className is null </exception>
			public Service(Provider provider, String type, String algorithm, String className, List<String> aliases, Map<String, String> attributes)
			{
				if ((provider == java.util.Map_Fields.Null) || (type == java.util.Map_Fields.Null) || (algorithm == java.util.Map_Fields.Null) || (className == java.util.Map_Fields.Null))
				{
					throw new NullPointerException();
				}
				this.Provider_Renamed = provider;
				this.Type_Renamed = GetEngineName(type);
				this.Algorithm_Renamed = algorithm;
				this.ClassName_Renamed = className;
				if (aliases == java.util.Map_Fields.Null)
				{
					this.Aliases_Renamed = System.Linq.Enumerable.Empty<String>();
				}
				else
				{
					this.Aliases_Renamed = new List<String>(aliases);
				}
				if (attributes == java.util.Map_Fields.Null)
				{
					this.Attributes = System.Linq.Enumerable.Empty<UString, String>();
				}
				else
				{
					this.Attributes = new HashMap<UString, String>();
					foreach (Map_Entry<String, String> entry in attributes.EntrySet())
					{
						this.Attributes.Put(new UString(entry.Key), entry.Value);
					}
				}
			}

			/// <summary>
			/// Get the type of this service. For example, {@code MessageDigest}.
			/// </summary>
			/// <returns> the type of this service </returns>
			public String Type
			{
				get
				{
					return Type_Renamed;
				}
			}

			/// <summary>
			/// Return the name of the algorithm of this service. For example,
			/// {@code SHA-1}.
			/// </summary>
			/// <returns> the algorithm of this service </returns>
			public String Algorithm
			{
				get
				{
					return Algorithm_Renamed;
				}
			}

			/// <summary>
			/// Return the Provider of this service.
			/// </summary>
			/// <returns> the Provider of this service </returns>
			public Provider Provider
			{
				get
				{
					return Provider_Renamed;
				}
			}

			/// <summary>
			/// Return the name of the class implementing this service.
			/// </summary>
			/// <returns> the name of the class implementing this service </returns>
			public String ClassName
			{
				get
				{
					return ClassName_Renamed;
				}
			}

			// internal only
			internal List<String> Aliases
			{
				get
				{
					return Aliases_Renamed;
				}
			}

			/// <summary>
			/// Return the value of the specified attribute or null if this
			/// attribute is not set for this Service.
			/// </summary>
			/// <param name="name"> the name of the requested attribute
			/// </param>
			/// <returns> the value of the specified attribute or null if the
			///         attribute is not present
			/// </returns>
			/// <exception cref="NullPointerException"> if name is null </exception>
			public String GetAttribute(String name)
			{
				if (name == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return Attributes.Get(new UString(name));
			}

			/// <summary>
			/// Return a new instance of the implementation described by this
			/// service. The security provider framework uses this method to
			/// construct implementations. Applications will typically not need
			/// to call it.
			/// 
			/// <para>The default implementation uses reflection to invoke the
			/// standard constructor for this type of service.
			/// Security providers can override this method to implement
			/// instantiation in a different way.
			/// For details and the values of constructorParameter that are
			/// valid for the various types of services see the
			/// <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
			/// Java Cryptography Architecture API Specification &amp;
			/// Reference</a>.
			/// 
			/// </para>
			/// </summary>
			/// <param name="constructorParameter"> the value to pass to the constructor,
			/// or null if this type of service does not use a constructorParameter.
			/// </param>
			/// <returns> a new implementation of this service
			/// </returns>
			/// <exception cref="InvalidParameterException"> if the value of
			/// constructorParameter is invalid for this type of service. </exception>
			/// <exception cref="NoSuchAlgorithmException"> if instantiation failed for
			/// any other reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object newInstance(Object constructorParameter) throws NoSuchAlgorithmException
			public virtual Object NewInstance(Object constructorParameter)
			{
				if (Registered == java.util.Map_Fields.False)
				{
					if (Provider_Renamed.GetService(Type_Renamed, Algorithm_Renamed) != this)
					{
						throw new NoSuchAlgorithmException("Service not registered with Provider " + Provider_Renamed.Name + ": " + this);
					}
					Registered = java.util.Map_Fields.True;
				}
				try
				{
					EngineDescription cap = KnownEngines.Get(Type_Renamed);
					if (cap == java.util.Map_Fields.Null)
					{
						// unknown engine type, use generic code
						// this is the code path future for non-core
						// optional packages
						return NewInstanceGeneric(constructorParameter);
					}
					if (cap.ConstructorParameterClassName == java.util.Map_Fields.Null)
					{
						if (constructorParameter != java.util.Map_Fields.Null)
						{
							throw new InvalidParameterException("constructorParameter not used with " + Type_Renamed + " engines");
						}
						Class clazz = ImplClass;
						Class[] empty = new Class[] {};
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> con = clazz.getConstructor(empty);
						Constructor<?> con = clazz.GetConstructor(empty);
						return con.newInstance();
					}
					else
					{
						Class paramClass = cap.ConstructorParameterClass;
						if (constructorParameter != java.util.Map_Fields.Null)
						{
							Class argClass = constructorParameter.GetType();
							if (argClass.IsSubclassOf(paramClass) == java.util.Map_Fields.False)
							{
								throw new InvalidParameterException("constructorParameter must be instanceof " + cap.ConstructorParameterClassName.Replace('$', '.') + " for engine type " + Type_Renamed);
							}
						}
						Class clazz = ImplClass;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> cons = clazz.getConstructor(paramClass);
						Constructor<?> cons = clazz.GetConstructor(paramClass);
						return cons.NewInstance(constructorParameter);
					}
				}
				catch (NoSuchAlgorithmException e)
				{
					throw e;
				}
				catch (InvocationTargetException e)
				{
					throw new NoSuchAlgorithmException("Error constructing implementation (algorithm: " + Algorithm_Renamed + ", provider: " + Provider_Renamed.Name + ", class: " + ClassName_Renamed + ")", e.InnerException);
				}
				catch (Exception e)
				{
					throw new NoSuchAlgorithmException("Error constructing implementation (algorithm: " + Algorithm_Renamed + ", provider: " + Provider_Renamed.Name + ", class: " + ClassName_Renamed + ")", e);
				}
			}

			// return the implementation Class object for this service
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class getImplClass() throws NoSuchAlgorithmException
			internal virtual Class ImplClass
			{
				get
				{
					try
					{
						Reference<Class> @ref = ClassRef;
						Class clazz = (@ref == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : @ref.Get();
						if (clazz == java.util.Map_Fields.Null)
						{
							ClassLoader cl = Provider_Renamed.GetType().ClassLoader;
							if (cl == java.util.Map_Fields.Null)
							{
								clazz = Class.ForName(ClassName_Renamed);
							}
							else
							{
								clazz = cl.LoadClass(ClassName_Renamed);
							}
							if (!Modifier.IsPublic(clazz.Modifiers))
							{
								throw new NoSuchAlgorithmException("class configured for " + Type_Renamed + " (provider: " + Provider_Renamed.Name + ") is not public.");
							}
							ClassRef = new WeakReference<Class>(clazz);
						}
						return clazz;
					}
					catch (ClassNotFoundException e)
					{
						throw new NoSuchAlgorithmException("class configured for " + Type_Renamed + " (provider: " + Provider_Renamed.Name + ") cannot be found.", e);
					}
				}
			}

			/// <summary>
			/// Generic code path for unknown engine types. Call the
			/// no-args constructor if constructorParameter is null, otherwise
			/// use the first matching constructor.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object newInstanceGeneric(Object constructorParameter) throws Exception
			internal virtual Object NewInstanceGeneric(Object constructorParameter)
			{
				Class clazz = ImplClass;
				if (constructorParameter == java.util.Map_Fields.Null)
				{
					// create instance with public no-arg constructor if it exists
					try
					{
						Class[] empty = new Class[] {};
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> con = clazz.getConstructor(empty);
						Constructor<?> con = clazz.GetConstructor(empty);
						return con.newInstance();
					}
					catch (NoSuchMethodException)
					{
						throw new NoSuchAlgorithmException("No public no-arg " + "constructor found in class " + ClassName_Renamed);
					}
				}
				Class argClass = constructorParameter.GetType();
				Constructor[] cons = clazz.Constructors;
				// find first public constructor that can take the
				// argument as parameter
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Constructor<?> con : cons)
				foreach (Constructor<?> con in cons)
				{
					Class[] paramTypes = con.ParameterTypes;
					if (paramTypes.Length != 1)
					{
						continue;
					}
					if (argClass.IsSubclassOf(paramTypes[0]) == java.util.Map_Fields.False)
					{
						continue;
					}
					return con.NewInstance(constructorParameter);
				}
				throw new NoSuchAlgorithmException("No public constructor matching " + argClass.Name + " found in class " + ClassName_Renamed);
			}

			/// <summary>
			/// Test whether this Service can use the specified parameter.
			/// Returns false if this service cannot use the parameter. Returns
			/// true if this service can use the parameter, if a fast test is
			/// infeasible, or if the status is unknown.
			/// 
			/// <para>The security provider framework uses this method with
			/// some types of services to quickly exclude non-matching
			/// implementations for consideration.
			/// Applications will typically not need to call it.
			/// 
			/// </para>
			/// <para>For details and the values of parameter that are valid for the
			/// various types of services see the top of this class and the
			/// <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
			/// Java Cryptography Architecture API Specification &amp;
			/// Reference</a>.
			/// Security providers can override it to implement their own test.
			/// 
			/// </para>
			/// </summary>
			/// <param name="parameter"> the parameter to test
			/// </param>
			/// <returns> false if this this service cannot use the specified
			/// parameter; true if it can possibly use the parameter
			/// </returns>
			/// <exception cref="InvalidParameterException"> if the value of parameter is
			/// invalid for this type of service or if this method cannot be
			/// used with this type of service </exception>
			public virtual bool SupportsParameter(Object parameter)
			{
				EngineDescription cap = KnownEngines.Get(Type_Renamed);
				if (cap == java.util.Map_Fields.Null)
				{
					// unknown engine type, return true by default
					return java.util.Map_Fields.True;
				}
				if (cap.SupportsParameter == java.util.Map_Fields.False)
				{
					throw new InvalidParameterException("supportsParameter() not " + "used with " + Type_Renamed + " engines");
				}
				// allow null for keys without attributes for compatibility
				if ((parameter != java.util.Map_Fields.Null) && (parameter is Key == java.util.Map_Fields.False))
				{
					throw new InvalidParameterException("Parameter must be instanceof Key for engine " + Type_Renamed);
				}
				if (HasKeyAttributes() == java.util.Map_Fields.False)
				{
					return java.util.Map_Fields.True;
				}
				if (parameter == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				Key key = (Key)parameter;
				if (SupportsKeyFormat(key))
				{
					return java.util.Map_Fields.True;
				}
				if (SupportsKeyClass(key))
				{
					return java.util.Map_Fields.True;
				}
				return java.util.Map_Fields.False;
			}

			/// <summary>
			/// Return whether this service has its Supported* properties for
			/// keys defined. Parses the attributes if not yet initialized.
			/// </summary>
			internal virtual bool HasKeyAttributes()
			{
				Boolean b = HasKeyAttributes_Renamed;
				if (b == java.util.Map_Fields.Null)
				{
					lock (this)
					{
						String s;
						s = GetAttribute("SupportedKeyFormats");
						if (s != java.util.Map_Fields.Null)
						{
							SupportedFormats = s.Split("\\|");
						}
						s = GetAttribute("SupportedKeyClasses");
						if (s != java.util.Map_Fields.Null)
						{
							String[] classNames = s.Split("\\|");
							List<Class> classList = new List<Class>(classNames.Length);
							foreach (String className in classNames)
							{
								Class clazz = GetKeyClass(className);
								if (clazz != java.util.Map_Fields.Null)
								{
									classList.Add(clazz);
								}
							}
							SupportedClasses = classList.ToArray(CLASS0);
						}
						bool @bool = (SupportedFormats != java.util.Map_Fields.Null) || (SupportedClasses != java.util.Map_Fields.Null);
						b = Convert.ToBoolean(@bool);
						HasKeyAttributes_Renamed = b;
					}
				}
				return b.BooleanValue();
			}

			// get the key class object of the specified name
			internal virtual Class GetKeyClass(String name)
			{
				try
				{
					return Class.ForName(name);
				}
				catch (ClassNotFoundException)
				{
					// ignore
				}
				try
				{
					ClassLoader cl = Provider_Renamed.GetType().ClassLoader;
					if (cl != java.util.Map_Fields.Null)
					{
						return cl.LoadClass(name);
					}
				}
				catch (ClassNotFoundException)
				{
					// ignore
				}
				return java.util.Map_Fields.Null;
			}

			internal virtual bool SupportsKeyFormat(Key key)
			{
				if (SupportedFormats == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				String format = key.Format;
				if (format == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				foreach (String supportedFormat in SupportedFormats)
				{
					if (supportedFormat.Equals(format))
					{
						return java.util.Map_Fields.True;
					}
				}
				return java.util.Map_Fields.False;
			}

			internal virtual bool SupportsKeyClass(Key key)
			{
				if (SupportedClasses == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				Class keyClass = key.GetType();
				foreach (Class clazz in SupportedClasses)
				{
					if (keyClass.IsSubclassOf(clazz))
					{
						return java.util.Map_Fields.True;
					}
				}
				return java.util.Map_Fields.False;
			}

			/// <summary>
			/// Return a String representation of this service.
			/// </summary>
			/// <returns> a String representation of this service. </returns>
			public override String ToString()
			{
				String aString = Aliases_Renamed.Count == 0 ? "" : "\r\n  aliases: " + Aliases_Renamed.ToString();
				String attrs = Attributes.Empty ? "" : "\r\n  attributes: " + Attributes.ToString();
				return Provider_Renamed.Name + ": " + Type_Renamed + "." + Algorithm_Renamed + " -> " + ClassName_Renamed + aString + attrs + "\r\n";
			}

		}

	}

}