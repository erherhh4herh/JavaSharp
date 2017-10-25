using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

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

/*
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1999 - All Rights Reserved
 *
 * The original version of this source code and documentation
 * is copyrighted and owned by Taligent, Inc., a wholly-owned
 * subsidiary of IBM. These materials are provided under terms
 * of a License Agreement between Taligent and Sun. This technology
 * is protected by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.util
{


	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using BaseLocale = sun.util.locale.BaseLocale;
	using LocaleObjectCache = sun.util.locale.LocaleObjectCache;


	/// 
	/// <summary>
	/// Resource bundles contain locale-specific objects.  When your program needs a
	/// locale-specific resource, a <code>String</code> for example, your program can
	/// load it from the resource bundle that is appropriate for the current user's
	/// locale. In this way, you can write program code that is largely independent
	/// of the user's locale isolating most, if not all, of the locale-specific
	/// information in resource bundles.
	/// 
	/// <para>
	/// This allows you to write programs that can:
	/// <UL>
	/// <LI> be easily localized, or translated, into different languages
	/// <LI> handle multiple locales at once
	/// <LI> be easily modified later to support even more locales
	/// </UL>
	/// 
	/// <P>
	/// Resource bundles belong to families whose members share a common base
	/// name, but whose names also have additional components that identify
	/// their locales. For example, the base name of a family of resource
	/// bundles might be "MyResources". The family should have a default
	/// resource bundle which simply has the same name as its family -
	/// "MyResources" - and will be used as the bundle of last resort if a
	/// specific locale is not supported. The family can then provide as
	/// many locale-specific members as needed, for example a German one
	/// named "MyResources_de".
	/// 
	/// <P>
	/// Each resource bundle in a family contains the same items, but the items have
	/// been translated for the locale represented by that resource bundle.
	/// For example, both "MyResources" and "MyResources_de" may have a
	/// <code>String</code> that's used on a button for canceling operations.
	/// In "MyResources" the <code>String</code> may contain "Cancel" and in
	/// "MyResources_de" it may contain "Abbrechen".
	/// 
	/// <P>
	/// If there are different resources for different countries, you
	/// can make specializations: for example, "MyResources_de_CH" contains objects for
	/// the German language (de) in Switzerland (CH). If you want to only
	/// modify some of the resources
	/// in the specialization, you can do so.
	/// 
	/// <P>
	/// When your program needs a locale-specific object, it loads
	/// the <code>ResourceBundle</code> class using the
	/// <seealso cref="#getBundle(java.lang.String, java.util.Locale) getBundle"/>
	/// method:
	/// <blockquote>
	/// <pre>
	/// ResourceBundle myResources =
	///      ResourceBundle.getBundle("MyResources", currentLocale);
	/// </pre>
	/// </blockquote>
	/// 
	/// <P>
	/// Resource bundles contain key/value pairs. The keys uniquely
	/// identify a locale-specific object in the bundle. Here's an
	/// example of a <code>ListResourceBundle</code> that contains
	/// two key/value pairs:
	/// <blockquote>
	/// <pre>
	/// public class MyResources extends ListResourceBundle {
	///     protected Object[][] getContents() {
	///         return new Object[][] {
	///             // LOCALIZE THE SECOND STRING OF EACH ARRAY (e.g., "OK")
	///             {"OkKey", "OK"},
	///             {"CancelKey", "Cancel"},
	///             // END OF MATERIAL TO LOCALIZE
	///        };
	///     }
	/// }
	/// </pre>
	/// </blockquote>
	/// Keys are always <code>String</code>s.
	/// In this example, the keys are "OkKey" and "CancelKey".
	/// In the above example, the values
	/// are also <code>String</code>s--"OK" and "Cancel"--but
	/// they don't have to be. The values can be any type of object.
	/// 
	/// <P>
	/// You retrieve an object from resource bundle using the appropriate
	/// getter method. Because "OkKey" and "CancelKey"
	/// are both strings, you would use <code>getString</code> to retrieve them:
	/// <blockquote>
	/// <pre>
	/// button1 = new Button(myResources.getString("OkKey"));
	/// button2 = new Button(myResources.getString("CancelKey"));
	/// </pre>
	/// </blockquote>
	/// The getter methods all require the key as an argument and return
	/// the object if found. If the object is not found, the getter method
	/// throws a <code>MissingResourceException</code>.
	/// 
	/// <P>
	/// Besides <code>getString</code>, <code>ResourceBundle</code> also provides
	/// a method for getting string arrays, <code>getStringArray</code>,
	/// as well as a generic <code>getObject</code> method for any other
	/// type of object. When using <code>getObject</code>, you'll
	/// have to cast the result to the appropriate type. For example:
	/// <blockquote>
	/// <pre>
	/// int[] myIntegers = (int[]) myResources.getObject("intList");
	/// </pre>
	/// </blockquote>
	/// 
	/// <P>
	/// The Java Platform provides two subclasses of <code>ResourceBundle</code>,
	/// <code>ListResourceBundle</code> and <code>PropertyResourceBundle</code>,
	/// that provide a fairly simple way to create resources.
	/// As you saw briefly in a previous example, <code>ListResourceBundle</code>
	/// manages its resource as a list of key/value pairs.
	/// <code>PropertyResourceBundle</code> uses a properties file to manage
	/// its resources.
	/// 
	/// </para>
	/// <para>
	/// If <code>ListResourceBundle</code> or <code>PropertyResourceBundle</code>
	/// do not suit your needs, you can write your own <code>ResourceBundle</code>
	/// subclass.  Your subclasses must override two methods: <code>handleGetObject</code>
	/// and <code>getKeys()</code>.
	/// 
	/// </para>
	/// <para>
	/// The implementation of a {@code ResourceBundle} subclass must be thread-safe
	/// if it's simultaneously used by multiple threads. The default implementations
	/// of the non-abstract methods in this class, and the methods in the direct
	/// known concrete subclasses {@code ListResourceBundle} and
	/// {@code PropertyResourceBundle} are thread-safe.
	/// 
	/// <h3>ResourceBundle.Control</h3>
	/// 
	/// The <seealso cref="ResourceBundle.Control"/> class provides information necessary
	/// to perform the bundle loading process by the <code>getBundle</code>
	/// factory methods that take a <code>ResourceBundle.Control</code>
	/// instance. You can implement your own subclass in order to enable
	/// non-standard resource bundle formats, change the search strategy, or
	/// define caching parameters. Refer to the descriptions of the class and the
	/// <seealso cref="#getBundle(String, Locale, ClassLoader, Control) getBundle"/>
	/// factory method for details.
	/// 
	/// </para>
	/// <para><a name="modify_default_behavior">For the {@code getBundle} factory</a>
	/// methods that take no <seealso cref="Control"/> instance, their <a
	/// href="#default_behavior"> default behavior</a> of resource bundle loading
	/// can be modified with <em>installed</em> {@link
	/// ResourceBundleControlProvider} implementations. Any installed providers are
	/// detected at the {@code ResourceBundle} class loading time. If any of the
	/// providers provides a <seealso cref="Control"/> for the given base name, that {@link
	/// Control} will be used instead of the default <seealso cref="Control"/>. If there is
	/// more than one service provider installed for supporting the same base name,
	/// the first one returned from <seealso cref="ServiceLoader"/> will be used.
	/// 
	/// <h3>Cache Management</h3>
	/// 
	/// Resource bundle instances created by the <code>getBundle</code> factory
	/// methods are cached by default, and the factory methods return the same
	/// resource bundle instance multiple times if it has been
	/// cached. <code>getBundle</code> clients may clear the cache, manage the
	/// lifetime of cached resource bundle instances using time-to-live values,
	/// or specify not to cache resource bundle instances. Refer to the
	/// descriptions of the {@link #getBundle(String, Locale, ClassLoader,
	/// Control) <code>getBundle</code> factory method}, {@link
	/// #clearCache(ClassLoader) clearCache}, {@link
	/// Control#getTimeToLive(String, Locale)
	/// ResourceBundle.Control.getTimeToLive}, and {@link
	/// Control#needsReload(String, Locale, String, ClassLoader, ResourceBundle,
	/// long) ResourceBundle.Control.needsReload} for details.
	/// 
	/// <h3>Example</h3>
	/// 
	/// The following is a very simple example of a <code>ResourceBundle</code>
	/// subclass, <code>MyResources</code>, that manages two resources (for a larger number of
	/// resources you would probably use a <code>Map</code>).
	/// Notice that you don't need to supply a value if
	/// a "parent-level" <code>ResourceBundle</code> handles the same
	/// key with the same value (as for the okKey below).
	/// <blockquote>
	/// <pre>
	/// // default (English language, United States)
	/// public class MyResources extends ResourceBundle {
	///     public Object handleGetObject(String key) {
	///         if (key.equals("okKey")) return "Ok";
	///         if (key.equals("cancelKey")) return "Cancel";
	///         return null;
	///     }
	/// 
	///     public Enumeration&lt;String&gt; getKeys() {
	///         return Collections.enumeration(keySet());
	///     }
	/// 
	///     // Overrides handleKeySet() so that the getKeys() implementation
	///     // can rely on the keySet() value.
	///     protected Set&lt;String&gt; handleKeySet() {
	///         return new HashSet&lt;String&gt;(Arrays.asList("okKey", "cancelKey"));
	///     }
	/// }
	/// 
	/// // German language
	/// public class MyResources_de extends MyResources {
	///     public Object handleGetObject(String key) {
	///         // don't need okKey, since parent level handles it.
	///         if (key.equals("cancelKey")) return "Abbrechen";
	///         return null;
	///     }
	/// 
	///     protected Set&lt;String&gt; handleKeySet() {
	///         return new HashSet&lt;String&gt;(Arrays.asList("cancelKey"));
	///     }
	/// }
	/// </pre>
	/// </blockquote>
	/// You do not have to restrict yourself to using a single family of
	/// <code>ResourceBundle</code>s. For example, you could have a set of bundles for
	/// exception messages, <code>ExceptionResources</code>
	/// (<code>ExceptionResources_fr</code>, <code>ExceptionResources_de</code>, ...),
	/// and one for widgets, <code>WidgetResource</code> (<code>WidgetResources_fr</code>,
	/// <code>WidgetResources_de</code>, ...); breaking up the resources however you like.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ListResourceBundle </seealso>
	/// <seealso cref= PropertyResourceBundle </seealso>
	/// <seealso cref= MissingResourceException
	/// @since JDK1.1 </seealso>
	public abstract class ResourceBundle
	{

		/// <summary>
		/// initial size of the bundle cache </summary>
		private const int INITIAL_CACHE_SIZE = 32;

		/// <summary>
		/// constant indicating that no resource bundle exists </summary>
		private static readonly ResourceBundle NONEXISTENT_BUNDLE = new ResourceBundleAnonymousInnerClassHelper();

		private class ResourceBundleAnonymousInnerClassHelper : ResourceBundle
		{
			public ResourceBundleAnonymousInnerClassHelper()
			{
			}

			public override IEnumerator<String> Keys
			{
				get
				{
					return null;
				}
			}
			protected internal override Object HandleGetObject(String key)
			{
				return null;
			}
			public override String ToString()
			{
				return "NONEXISTENT_BUNDLE";
			}
		}


		/// <summary>
		/// The cache is a map from cache keys (with bundle base name, locale, and
		/// class loader) to either a resource bundle or NONEXISTENT_BUNDLE wrapped by a
		/// BundleReference.
		/// 
		/// The cache is a ConcurrentMap, allowing the cache to be searched
		/// concurrently by multiple threads.  This will also allow the cache keys
		/// to be reclaimed along with the ClassLoaders they reference.
		/// 
		/// This variable would be better named "cache", but we keep the old
		/// name for compatibility with some workarounds for bug 4212439.
		/// </summary>
		private static readonly ConcurrentMap<CacheKey, BundleReference> CacheList = new ConcurrentDictionary<CacheKey, BundleReference>(INITIAL_CACHE_SIZE);

		/// <summary>
		/// Queue for reference objects referring to class loaders or bundles.
		/// </summary>
		private static readonly ReferenceQueue<Object> ReferenceQueue = new ReferenceQueue<Object>();

		/// <summary>
		/// Returns the base name of this bundle, if known, or {@code null} if unknown.
		/// 
		/// If not null, then this is the value of the {@code baseName} parameter
		/// that was passed to the {@code ResourceBundle.getBundle(...)} method
		/// when the resource bundle was loaded.
		/// </summary>
		/// <returns> The base name of the resource bundle, as provided to and expected
		/// by the {@code ResourceBundle.getBundle(...)} methods.
		/// </returns>
		/// <seealso cref= #getBundle(java.lang.String, java.util.Locale, java.lang.ClassLoader)
		/// 
		/// @since 1.8 </seealso>
		public virtual String BaseBundleName
		{
			get
			{
				return Name;
			}
		}

		/// <summary>
		/// The parent bundle of this bundle.
		/// The parent bundle is searched by <seealso cref="#getObject getObject"/>
		/// when this bundle does not contain a particular resource.
		/// </summary>
		protected internal ResourceBundle Parent_Renamed = null;

		/// <summary>
		/// The locale for this bundle.
		/// </summary>
		private Locale Locale_Renamed = null;

		/// <summary>
		/// The base bundle name for this bundle.
		/// </summary>
		private String Name;

		/// <summary>
		/// The flag indicating this bundle has expired in the cache.
		/// </summary>
		private volatile bool Expired;

		/// <summary>
		/// The back link to the cache key. null if this bundle isn't in
		/// the cache (yet) or has expired.
		/// </summary>
		private volatile CacheKey CacheKey;

		/// <summary>
		/// A Set of the keys contained only in this ResourceBundle.
		/// </summary>
		private volatile Set<String> KeySet_Renamed;

		private static readonly List<ResourceBundleControlProvider> Providers;

		static ResourceBundle()
		{
			List<ResourceBundleControlProvider> list = null;
			ServiceLoader<ResourceBundleControlProvider> serviceLoaders = ServiceLoader.LoadInstalled(typeof(ResourceBundleControlProvider));
			foreach (ResourceBundleControlProvider provider in serviceLoaders)
			{
				if (list == null)
				{
					list = new List<>();
				}
				list.Add(provider);
			}
			Providers = list;
		}

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		public ResourceBundle()
		{
		}

		/// <summary>
		/// Gets a string for the given key from this resource bundle or one of its parents.
		/// Calling this method is equivalent to calling
		/// <blockquote>
		/// <code>(String) <seealso cref="#getObject(java.lang.String) getObject"/>(key)</code>.
		/// </blockquote>
		/// </summary>
		/// <param name="key"> the key for the desired string </param>
		/// <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException"> if no object for the given key can be found </exception>
		/// <exception cref="ClassCastException"> if the object found for the given key is not a string </exception>
		/// <returns> the string for the given key </returns>
		public String GetString(String key)
		{
			return (String) GetObject(key);
		}

		/// <summary>
		/// Gets a string array for the given key from this resource bundle or one of its parents.
		/// Calling this method is equivalent to calling
		/// <blockquote>
		/// <code>(String[]) <seealso cref="#getObject(java.lang.String) getObject"/>(key)</code>.
		/// </blockquote>
		/// </summary>
		/// <param name="key"> the key for the desired string array </param>
		/// <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException"> if no object for the given key can be found </exception>
		/// <exception cref="ClassCastException"> if the object found for the given key is not a string array </exception>
		/// <returns> the string array for the given key </returns>
		public String[] GetStringArray(String key)
		{
			return (String[]) GetObject(key);
		}

		/// <summary>
		/// Gets an object for the given key from this resource bundle or one of its parents.
		/// This method first tries to obtain the object from this resource bundle using
		/// <seealso cref="#handleGetObject(java.lang.String) handleGetObject"/>.
		/// If not successful, and the parent resource bundle is not null,
		/// it calls the parent's <code>getObject</code> method.
		/// If still not successful, it throws a MissingResourceException.
		/// </summary>
		/// <param name="key"> the key for the desired object </param>
		/// <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException"> if no object for the given key can be found </exception>
		/// <returns> the object for the given key </returns>
		public Object GetObject(String key)
		{
			Object obj = HandleGetObject(key);
			if (obj == null)
			{
				if (Parent_Renamed != null)
				{
					obj = Parent_Renamed.GetObject(key);
				}
				if (obj == null)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new MissingResourceException("Can't find resource for bundle " + this.GetType().FullName + ", key " + key, this.GetType().FullName, key);
				}
			}
			return obj;
		}

		/// <summary>
		/// Returns the locale of this resource bundle. This method can be used after a
		/// call to getBundle() to determine whether the resource bundle returned really
		/// corresponds to the requested locale or is a fallback.
		/// </summary>
		/// <returns> the locale of this resource bundle </returns>
		public virtual Locale Locale
		{
			get
			{
				return Locale_Renamed;
			}
		}

		/*
		 * Automatic determination of the ClassLoader to be used to load
		 * resources on behalf of the client.
		 */
		private static ClassLoader GetLoader(Class caller)
		{
			ClassLoader cl = caller == null ? null : caller.ClassLoader;
			if (cl == null)
			{
				// When the caller's loader is the boot class loader, cl is null
				// here. In that case, ClassLoader.getSystemClassLoader() may
				// return the same class loader that the application is
				// using. We therefore use a wrapper ClassLoader to create a
				// separate scope for bundles loaded on behalf of the Java
				// runtime so that these bundles cannot be returned from the
				// cache to the application (5048280).
				cl = RBClassLoader.INSTANCE;
			}
			return cl;
		}

		/// <summary>
		/// A wrapper of ClassLoader.getSystemClassLoader().
		/// </summary>
		private class RBClassLoader : ClassLoader
		{
			internal static readonly RBClassLoader INSTANCE = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<RBClassLoader>
			{
				public PrivilegedActionAnonymousInnerClassHelper()
				{
				}

				public virtual RBClassLoader Run()
				{
					return new RBClassLoader();
				}
			}
			internal static readonly ClassLoader Loader = ClassLoader.SystemClassLoader;

			internal RBClassLoader()
			{
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Class loadClass(String name) throws ClassNotFoundException
			public override Class LoadClass(String name)
			{
				if (Loader != null)
				{
					return Loader.LoadClass(name);
				}
				return Class.ForName(name);
			}
			public override URL GetResource(String name)
			{
				if (Loader != null)
				{
					return Loader.GetResource(name);
				}
				return ClassLoader.GetSystemResource(name);
			}
			public override InputStream GetResourceAsStream(String name)
			{
				if (Loader != null)
				{
					return Loader.GetResourceAsStream(name);
				}
				return ClassLoader.GetSystemResourceAsStream(name);
			}
		}

		/// <summary>
		/// Sets the parent bundle of this bundle.
		/// The parent bundle is searched by <seealso cref="#getObject getObject"/>
		/// when this bundle does not contain a particular resource.
		/// </summary>
		/// <param name="parent"> this bundle's parent bundle. </param>
		protected internal virtual ResourceBundle Parent
		{
			set
			{
				Debug.Assert(value != NONEXISTENT_BUNDLE);
				this.Parent_Renamed = value;
			}
		}

		/// <summary>
		/// Key used for cached resource bundles.  The key checks the base
		/// name, the locale, and the class loader to determine if the
		/// resource is a match to the requested one. The loader may be
		/// null, but the base name and the locale must have a non-null
		/// value.
		/// </summary>
		private class CacheKey : Cloneable
		{
			// These three are the actual keys for lookup in Map.
			internal String Name_Renamed;
			internal Locale Locale_Renamed;
			internal LoaderReference LoaderRef;

			// bundle format which is necessary for calling
			// Control.needsReload().
			internal String Format_Renamed;

			// These time values are in CacheKey so that NONEXISTENT_BUNDLE
			// doesn't need to be cloned for caching.

			// The time when the bundle has been loaded
			internal volatile long LoadTime;

			// The time when the bundle expires in the cache, or either
			// Control.TTL_DONT_CACHE or Control.TTL_NO_EXPIRATION_CONTROL.
			internal volatile long ExpirationTime;

			// Placeholder for an error report by a Throwable
			internal Throwable Cause_Renamed;

			// Hash code value cache to avoid recalculating the hash code
			// of this instance.
			internal int HashCodeCache;

			internal CacheKey(String baseName, Locale locale, ClassLoader loader)
			{
				this.Name_Renamed = baseName;
				this.Locale_Renamed = locale;
				if (loader == null)
				{
					this.LoaderRef = null;
				}
				else
				{
					LoaderRef = new LoaderReference(loader, ReferenceQueue, this);
				}
				CalculateHashCode();
			}

			internal virtual String Name
			{
				get
				{
					return Name_Renamed;
				}
			}

			internal virtual CacheKey SetName(String baseName)
			{
				if (!this.Name_Renamed.Equals(baseName))
				{
					this.Name_Renamed = baseName;
					CalculateHashCode();
				}
				return this;
			}

			internal virtual Locale Locale
			{
				get
				{
					return Locale_Renamed;
				}
			}

			internal virtual CacheKey SetLocale(Locale locale)
			{
				if (!this.Locale_Renamed.Equals(locale))
				{
					this.Locale_Renamed = locale;
					CalculateHashCode();
				}
				return this;
			}

			internal virtual ClassLoader Loader
			{
				get
				{
					return (LoaderRef != null) ? LoaderRef.get() : null;
				}
			}

			public override bool Equals(Object other)
			{
				if (this == other)
				{
					return true;
				}
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CacheKey otherEntry = (CacheKey)other;
					CacheKey otherEntry = (CacheKey)other;
					//quick check to see if they are not equal
					if (HashCodeCache != otherEntry.HashCodeCache)
					{
						return false;
					}
					//are the names the same?
					if (!Name_Renamed.Equals(otherEntry.Name_Renamed))
					{
						return false;
					}
					// are the locales the same?
					if (!Locale_Renamed.Equals(otherEntry.Locale_Renamed))
					{
						return false;
					}
					//are refs (both non-null) or (both null)?
					if (LoaderRef == null)
					{
						return otherEntry.LoaderRef == null;
					}
					ClassLoader loader = LoaderRef.get();
					return (otherEntry.LoaderRef != null) && (loader != null) && (loader == otherEntry.LoaderRef.get());
							// with a null reference we can no longer find
							// out which class loader was referenced; so
							// treat it as unequal
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (NullPointerException | ClassCastException e)
				{
				}
				return false;
			}

			public override int HashCode()
			{
				return HashCodeCache;
			}

			internal virtual void CalculateHashCode()
			{
				HashCodeCache = Name_Renamed.HashCode() << 3;
				HashCodeCache ^= Locale_Renamed.HashCode();
				ClassLoader loader = Loader;
				if (loader != null)
				{
					HashCodeCache ^= loader.HashCode();
				}
			}

			public virtual Object Clone()
			{
				try
				{
					CacheKey clone = (CacheKey) base.Clone();
					if (LoaderRef != null)
					{
						clone.LoaderRef = new LoaderReference(LoaderRef.get(), ReferenceQueue, clone);
					}
					// Clear the reference to a Throwable
					clone.Cause_Renamed = null;
					return clone;
				}
				catch (CloneNotSupportedException e)
				{
					//this should never happen
					throw new InternalError(e);
				}
			}

			internal virtual String Format
			{
				get
				{
					return Format_Renamed;
				}
				set
				{
					this.Format_Renamed = value;
				}
			}


			internal virtual Throwable Cause
			{
				set
				{
					if (this.Cause_Renamed == null)
					{
						this.Cause_Renamed = value;
					}
					else
					{
						// Override the value if the previous one is
						// ClassNotFoundException.
						if (this.Cause_Renamed is ClassNotFoundException)
						{
							this.Cause_Renamed = value;
						}
					}
				}
				get
				{
					return Cause_Renamed;
				}
			}


			public override String ToString()
			{
				String l = Locale_Renamed.ToString();
				if (l.Length() == 0)
				{
					if (Locale_Renamed.Variant.Length() != 0)
					{
						l = "__" + Locale_Renamed.Variant;
					}
					else
					{
						l = "\"\"";
					}
				}
				return "CacheKey[" + Name_Renamed + ", lc=" + l + ", ldr=" + Loader + "(format=" + Format_Renamed + ")]";
			}
		}

		/// <summary>
		/// The common interface to get a CacheKey in LoaderReference and
		/// BundleReference.
		/// </summary>
		private interface CacheKeyReference
		{
			CacheKey CacheKey {get;}
		}

		/// <summary>
		/// References to class loaders are weak references, so that they can be
		/// garbage collected when nobody else is using them. The ResourceBundle
		/// class has no reason to keep class loaders alive.
		/// </summary>
		private class LoaderReference : WeakReference<ClassLoader>, CacheKeyReference
		{
			internal CacheKey CacheKey_Renamed;

			internal LoaderReference(ClassLoader referent, ReferenceQueue<Object> q, CacheKey key) : base(referent, q)
			{
				CacheKey_Renamed = key;
			}

			public virtual CacheKey CacheKey
			{
				get
				{
					return CacheKey_Renamed;
				}
			}
		}

		/// <summary>
		/// References to bundles are soft references so that they can be garbage
		/// collected when they have no hard references.
		/// </summary>
		private class BundleReference : SoftReference<ResourceBundle>, CacheKeyReference
		{
			internal CacheKey CacheKey_Renamed;

			internal BundleReference(ResourceBundle referent, ReferenceQueue<Object> q, CacheKey key) : base(referent, q)
			{
				CacheKey_Renamed = key;
			}

			public virtual CacheKey CacheKey
			{
				get
				{
					return CacheKey_Renamed;
				}
			}
		}

		/// <summary>
		/// Gets a resource bundle using the specified base name, the default locale,
		/// and the caller's class loader. Calling this method is equivalent to calling
		/// <blockquote>
		/// <code>getBundle(baseName, Locale.getDefault(), this.getClass().getClassLoader())</code>,
		/// </blockquote>
		/// except that <code>getClassLoader()</code> is run with the security
		/// privileges of <code>ResourceBundle</code>.
		/// See <seealso cref="#getBundle(String, Locale, ClassLoader) getBundle"/>
		/// for a complete description of the search and instantiation strategy.
		/// </summary>
		/// <param name="baseName"> the base name of the resource bundle, a fully qualified class name </param>
		/// <exception cref="java.lang.NullPointerException">
		///     if <code>baseName</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///     if no resource bundle for the specified base name can be found </exception>
		/// <returns> a resource bundle for the given base name and the default locale </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static final ResourceBundle getBundle(String baseName)
		public static ResourceBundle GetBundle(String baseName)
		{
			return GetBundleImpl(baseName, Locale.Default, GetLoader(Reflection.CallerClass), GetDefaultControl(baseName));
		}

		/// <summary>
		/// Returns a resource bundle using the specified base name, the
		/// default locale and the specified control. Calling this method
		/// is equivalent to calling
		/// <pre>
		/// getBundle(baseName, Locale.getDefault(),
		///           this.getClass().getClassLoader(), control),
		/// </pre>
		/// except that <code>getClassLoader()</code> is run with the security
		/// privileges of <code>ResourceBundle</code>.  See {@link
		/// #getBundle(String, Locale, ClassLoader, Control) getBundle} for the
		/// complete description of the resource bundle loading process with a
		/// <code>ResourceBundle.Control</code>.
		/// </summary>
		/// <param name="baseName">
		///        the base name of the resource bundle, a fully qualified class
		///        name </param>
		/// <param name="control">
		///        the control which gives information for the resource bundle
		///        loading process </param>
		/// <returns> a resource bundle for the given base name and the default
		///        locale </returns>
		/// <exception cref="NullPointerException">
		///        if <code>baseName</code> or <code>control</code> is
		///        <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///        if no resource bundle for the specified base name can be found </exception>
		/// <exception cref="IllegalArgumentException">
		///        if the given <code>control</code> doesn't perform properly
		///        (e.g., <code>control.getCandidateLocales</code> returns null.)
		///        Note that validation of <code>control</code> is performed as
		///        needed.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static final ResourceBundle getBundle(String baseName, Control control)
		public static ResourceBundle GetBundle(String baseName, Control control)
		{
			return GetBundleImpl(baseName, Locale.Default, GetLoader(Reflection.CallerClass), control);
		}

		/// <summary>
		/// Gets a resource bundle using the specified base name and locale,
		/// and the caller's class loader. Calling this method is equivalent to calling
		/// <blockquote>
		/// <code>getBundle(baseName, locale, this.getClass().getClassLoader())</code>,
		/// </blockquote>
		/// except that <code>getClassLoader()</code> is run with the security
		/// privileges of <code>ResourceBundle</code>.
		/// See <seealso cref="#getBundle(String, Locale, ClassLoader) getBundle"/>
		/// for a complete description of the search and instantiation strategy.
		/// </summary>
		/// <param name="baseName">
		///        the base name of the resource bundle, a fully qualified class name </param>
		/// <param name="locale">
		///        the locale for which a resource bundle is desired </param>
		/// <exception cref="NullPointerException">
		///        if <code>baseName</code> or <code>locale</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///        if no resource bundle for the specified base name can be found </exception>
		/// <returns> a resource bundle for the given base name and locale </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static final ResourceBundle getBundle(String baseName, Locale locale)
		public static ResourceBundle GetBundle(String baseName, Locale locale)
		{
			return GetBundleImpl(baseName, locale, GetLoader(Reflection.CallerClass), GetDefaultControl(baseName));
		}

		/// <summary>
		/// Returns a resource bundle using the specified base name, target
		/// locale and control, and the caller's class loader. Calling this
		/// method is equivalent to calling
		/// <pre>
		/// getBundle(baseName, targetLocale, this.getClass().getClassLoader(),
		///           control),
		/// </pre>
		/// except that <code>getClassLoader()</code> is run with the security
		/// privileges of <code>ResourceBundle</code>.  See {@link
		/// #getBundle(String, Locale, ClassLoader, Control) getBundle} for the
		/// complete description of the resource bundle loading process with a
		/// <code>ResourceBundle.Control</code>.
		/// </summary>
		/// <param name="baseName">
		///        the base name of the resource bundle, a fully qualified
		///        class name </param>
		/// <param name="targetLocale">
		///        the locale for which a resource bundle is desired </param>
		/// <param name="control">
		///        the control which gives information for the resource
		///        bundle loading process </param>
		/// <returns> a resource bundle for the given base name and a
		///        <code>Locale</code> in <code>locales</code> </returns>
		/// <exception cref="NullPointerException">
		///        if <code>baseName</code>, <code>locales</code> or
		///        <code>control</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///        if no resource bundle for the specified base name in any
		///        of the <code>locales</code> can be found. </exception>
		/// <exception cref="IllegalArgumentException">
		///        if the given <code>control</code> doesn't perform properly
		///        (e.g., <code>control.getCandidateLocales</code> returns null.)
		///        Note that validation of <code>control</code> is performed as
		///        needed.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static final ResourceBundle getBundle(String baseName, Locale targetLocale, Control control)
		public static ResourceBundle GetBundle(String baseName, Locale targetLocale, Control control)
		{
			return GetBundleImpl(baseName, targetLocale, GetLoader(Reflection.CallerClass), control);
		}

		/// <summary>
		/// Gets a resource bundle using the specified base name, locale, and class
		/// loader.
		/// 
		/// <para>This method behaves the same as calling
		/// <seealso cref="#getBundle(String, Locale, ClassLoader, Control)"/> passing a
		/// default instance of <seealso cref="Control"/> unless another <seealso cref="Control"/> is
		/// provided with the <seealso cref="ResourceBundleControlProvider"/> SPI. Refer to the
		/// description of <a href="#modify_default_behavior">modifying the default
		/// behavior</a>.
		/// 
		/// </para>
		/// <para><a name="default_behavior">The following describes the default
		/// behavior</a>.
		/// 
		/// </para>
		/// <para><code>getBundle</code> uses the base name, the specified locale, and
		/// the default locale (obtained from {@link java.util.Locale#getDefault()
		/// Locale.getDefault}) to generate a sequence of <a
		/// name="candidates"><em>candidate bundle names</em></a>.  If the specified
		/// locale's language, script, country, and variant are all empty strings,
		/// then the base name is the only candidate bundle name.  Otherwise, a list
		/// of candidate locales is generated from the attribute values of the
		/// specified locale (language, script, country and variant) and appended to
		/// the base name.  Typically, this will look like the following:
		/// 
		/// <pre>
		///     baseName + "_" + language + "_" + script + "_" + country + "_" + variant
		///     baseName + "_" + language + "_" + script + "_" + country
		///     baseName + "_" + language + "_" + script
		///     baseName + "_" + language + "_" + country + "_" + variant
		///     baseName + "_" + language + "_" + country
		///     baseName + "_" + language
		/// </pre>
		/// 
		/// </para>
		/// <para>Candidate bundle names where the final component is an empty string
		/// are omitted, along with the underscore.  For example, if country is an
		/// empty string, the second and the fifth candidate bundle names above
		/// would be omitted.  Also, if script is an empty string, the candidate names
		/// including script are omitted.  For example, a locale with language "de"
		/// and variant "JAVA" will produce candidate names with base name
		/// "MyResource" below.
		/// 
		/// <pre>
		///     MyResource_de__JAVA
		///     MyResource_de
		/// </pre>
		/// 
		/// In the case that the variant contains one or more underscores ('_'), a
		/// sequence of bundle names generated by truncating the last underscore and
		/// the part following it is inserted after a candidate bundle name with the
		/// original variant.  For example, for a locale with language "en", script
		/// "Latn, country "US" and variant "WINDOWS_VISTA", and bundle base name
		/// "MyResource", the list of candidate bundle names below is generated:
		/// 
		/// <pre>
		/// MyResource_en_Latn_US_WINDOWS_VISTA
		/// MyResource_en_Latn_US_WINDOWS
		/// MyResource_en_Latn_US
		/// MyResource_en_Latn
		/// MyResource_en_US_WINDOWS_VISTA
		/// MyResource_en_US_WINDOWS
		/// MyResource_en_US
		/// MyResource_en
		/// </pre>
		/// 
		/// <blockquote><b>Note:</b> For some <code>Locale</code>s, the list of
		/// candidate bundle names contains extra names, or the order of bundle names
		/// is slightly modified.  See the description of the default implementation
		/// of {@link Control#getCandidateLocales(String, Locale)
		/// getCandidateLocales} for details.</blockquote>
		/// 
		/// </para>
		/// <para><code>getBundle</code> then iterates over the candidate bundle names
		/// to find the first one for which it can <em>instantiate</em> an actual
		/// resource bundle. It uses the default controls' {@link Control#getFormats
		/// getFormats} method, which generates two bundle names for each generated
		/// name, the first a class name and the second a properties file name. For
		/// each candidate bundle name, it attempts to create a resource bundle:
		/// 
		/// <ul><li>First, it attempts to load a class using the generated class name.
		/// If such a class can be found and loaded using the specified class
		/// loader, is assignment compatible with ResourceBundle, is accessible from
		/// ResourceBundle, and can be instantiated, <code>getBundle</code> creates a
		/// new instance of this class and uses it as the <em>result resource
		/// bundle</em>.
		/// 
		/// <li>Otherwise, <code>getBundle</code> attempts to locate a property
		/// resource file using the generated properties file name.  It generates a
		/// path name from the candidate bundle name by replacing all "." characters
		/// with "/" and appending the string ".properties".  It attempts to find a
		/// "resource" with this name using {@link
		/// java.lang.ClassLoader#getResource(java.lang.String)
		/// ClassLoader.getResource}.  (Note that a "resource" in the sense of
		/// <code>getResource</code> has nothing to do with the contents of a
		/// resource bundle, it is just a container of data, such as a file.)  If it
		/// finds a "resource", it attempts to create a new {@link
		/// PropertyResourceBundle} instance from its contents.  If successful, this
		/// instance becomes the <em>result resource bundle</em>.  </ul>
		/// 
		/// </para>
		/// <para>This continues until a result resource bundle is instantiated or the
		/// list of candidate bundle names is exhausted.  If no matching resource
		/// bundle is found, the default control's {@link Control#getFallbackLocale
		/// getFallbackLocale} method is called, which returns the current default
		/// locale.  A new sequence of candidate locale names is generated using this
		/// locale and and searched again, as above.
		/// 
		/// </para>
		/// <para>If still no result bundle is found, the base name alone is looked up. If
		/// this still fails, a <code>MissingResourceException</code> is thrown.
		/// 
		/// </para>
		/// <para><a name="parent_chain"> Once a result resource bundle has been found,
		/// its <em>parent chain</em> is instantiated</a>.  If the result bundle already
		/// has a parent (perhaps because it was returned from a cache) the chain is
		/// complete.
		/// 
		/// </para>
		/// <para>Otherwise, <code>getBundle</code> examines the remainder of the
		/// candidate locale list that was used during the pass that generated the
		/// result resource bundle.  (As before, candidate bundle names where the
		/// final component is an empty string are omitted.)  When it comes to the
		/// end of the candidate list, it tries the plain bundle name.  With each of the
		/// candidate bundle names it attempts to instantiate a resource bundle (first
		/// looking for a class and then a properties file, as described above).
		/// 
		/// </para>
		/// <para>Whenever it succeeds, it calls the previously instantiated resource
		/// bundle's <seealso cref="#setParent(java.util.ResourceBundle) setParent"/> method
		/// with the new resource bundle.  This continues until the list of names
		/// is exhausted or the current bundle already has a non-null parent.
		/// 
		/// </para>
		/// <para>Once the parent chain is complete, the bundle is returned.
		/// 
		/// </para>
		/// <para><b>Note:</b> <code>getBundle</code> caches instantiated resource
		/// bundles and might return the same resource bundle instance multiple times.
		/// 
		/// </para>
		/// <para><b>Note:</b>The <code>baseName</code> argument should be a fully
		/// qualified class name. However, for compatibility with earlier versions,
		/// Sun's Java SE Runtime Environments do not verify this, and so it is
		/// possible to access <code>PropertyResourceBundle</code>s by specifying a
		/// path name (using "/") instead of a fully qualified class name (using
		/// ".").
		/// 
		/// </para>
		/// <para><a name="default_behavior_example">
		/// <strong>Example:</strong></a>
		/// </para>
		/// <para>
		/// The following class and property files are provided:
		/// <pre>
		///     MyResources.class
		///     MyResources.properties
		///     MyResources_fr.properties
		///     MyResources_fr_CH.class
		///     MyResources_fr_CH.properties
		///     MyResources_en.properties
		///     MyResources_es_ES.class
		/// </pre>
		/// 
		/// The contents of all files are valid (that is, public non-abstract
		/// subclasses of <code>ResourceBundle</code> for the ".class" files,
		/// syntactically correct ".properties" files).  The default locale is
		/// <code>Locale("en", "GB")</code>.
		/// 
		/// </para>
		/// <para>Calling <code>getBundle</code> with the locale arguments below will
		/// instantiate resource bundles as follows:
		/// 
		/// <table summary="getBundle() locale to resource bundle mapping">
		/// <tr><td>Locale("fr", "CH")</td><td>MyResources_fr_CH.class, parent MyResources_fr.properties, parent MyResources.class</td></tr>
		/// <tr><td>Locale("fr", "FR")</td><td>MyResources_fr.properties, parent MyResources.class</td></tr>
		/// <tr><td>Locale("de", "DE")</td><td>MyResources_en.properties, parent MyResources.class</td></tr>
		/// <tr><td>Locale("en", "US")</td><td>MyResources_en.properties, parent MyResources.class</td></tr>
		/// <tr><td>Locale("es", "ES")</td><td>MyResources_es_ES.class, parent MyResources.class</td></tr>
		/// </table>
		/// 
		/// </para>
		/// <para>The file MyResources_fr_CH.properties is never used because it is
		/// hidden by the MyResources_fr_CH.class. Likewise, MyResources.properties
		/// is also hidden by MyResources.class.
		/// 
		/// </para>
		/// </summary>
		/// <param name="baseName"> the base name of the resource bundle, a fully qualified class name </param>
		/// <param name="locale"> the locale for which a resource bundle is desired </param>
		/// <param name="loader"> the class loader from which to load the resource bundle </param>
		/// <returns> a resource bundle for the given base name and locale </returns>
		/// <exception cref="java.lang.NullPointerException">
		///        if <code>baseName</code>, <code>locale</code>, or <code>loader</code> is <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///        if no resource bundle for the specified base name can be found
		/// @since 1.2 </exception>
		public static ResourceBundle GetBundle(String baseName, Locale locale, ClassLoader loader)
		{
			if (loader == null)
			{
				throw new NullPointerException();
			}
			return GetBundleImpl(baseName, locale, loader, GetDefaultControl(baseName));
		}

		/// <summary>
		/// Returns a resource bundle using the specified base name, target
		/// locale, class loader and control. Unlike the {@linkplain
		/// #getBundle(String, Locale, ClassLoader) <code>getBundle</code>
		/// factory methods with no <code>control</code> argument}, the given
		/// <code>control</code> specifies how to locate and instantiate resource
		/// bundles. Conceptually, the bundle loading process with the given
		/// <code>control</code> is performed in the following steps.
		/// 
		/// <ol>
		/// <li>This factory method looks up the resource bundle in the cache for
		/// the specified <code>baseName</code>, <code>targetLocale</code> and
		/// <code>loader</code>.  If the requested resource bundle instance is
		/// found in the cache and the time-to-live periods of the instance and
		/// all of its parent instances have not expired, the instance is returned
		/// to the caller. Otherwise, this factory method proceeds with the
		/// loading process below.</li>
		/// 
		/// <li>The {@link ResourceBundle.Control#getFormats(String)
		/// control.getFormats} method is called to get resource bundle formats
		/// to produce bundle or resource names. The strings
		/// <code>"java.class"</code> and <code>"java.properties"</code>
		/// designate class-based and {@link PropertyResourceBundle
		/// property}-based resource bundles, respectively. Other strings
		/// starting with <code>"java."</code> are reserved for future extensions
		/// and must not be used for application-defined formats. Other strings
		/// designate application-defined formats.</li>
		/// 
		/// <li>The {@link ResourceBundle.Control#getCandidateLocales(String,
		/// Locale) control.getCandidateLocales} method is called with the target
		/// locale to get a list of <em>candidate <code>Locale</code>s</em> for
		/// which resource bundles are searched.</li>
		/// 
		/// <li>The {@link ResourceBundle.Control#newBundle(String, Locale,
		/// String, ClassLoader, boolean) control.newBundle} method is called to
		/// instantiate a <code>ResourceBundle</code> for the base bundle name, a
		/// candidate locale, and a format. (Refer to the note on the cache
		/// lookup below.) This step is iterated over all combinations of the
		/// candidate locales and formats until the <code>newBundle</code> method
		/// returns a <code>ResourceBundle</code> instance or the iteration has
		/// used up all the combinations. For example, if the candidate locales
		/// are <code>Locale("de", "DE")</code>, <code>Locale("de")</code> and
		/// <code>Locale("")</code> and the formats are <code>"java.class"</code>
		/// and <code>"java.properties"</code>, then the following is the
		/// sequence of locale-format combinations to be used to call
		/// <code>control.newBundle</code>.
		/// 
		/// <table style="width: 50%; text-align: left; margin-left: 40px;"
		///  border="0" cellpadding="2" cellspacing="2" summary="locale-format combinations for newBundle">
		/// <tbody>
		/// <tr>
		/// <td
		/// style="vertical-align: top; text-align: left; font-weight: bold; width: 50%;"><code>Locale</code><br>
		/// </td>
		/// <td
		/// style="vertical-align: top; text-align: left; font-weight: bold; width: 50%;"><code>format</code><br>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("de", "DE")</code><br>
		/// </td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.class</code><br>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("de", "DE")</code></td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.properties</code><br>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("de")</code></td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.class</code></td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("de")</code></td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.properties</code></td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("")</code><br>
		/// </td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.class</code></td>
		/// </tr>
		/// <tr>
		/// <td style="vertical-align: top; width: 50%;"><code>Locale("")</code></td>
		/// <td style="vertical-align: top; width: 50%;"><code>java.properties</code></td>
		/// </tr>
		/// </tbody>
		/// </table>
		/// </li>
		/// 
		/// <li>If the previous step has found no resource bundle, proceed to
		/// Step 6. If a bundle has been found that is a base bundle (a bundle
		/// for <code>Locale("")</code>), and the candidate locale list only contained
		/// <code>Locale("")</code>, return the bundle to the caller. If a bundle
		/// has been found that is a base bundle, but the candidate locale list
		/// contained locales other than Locale(""), put the bundle on hold and
		/// proceed to Step 6. If a bundle has been found that is not a base
		/// bundle, proceed to Step 7.</li>
		/// 
		/// <li>The {@link ResourceBundle.Control#getFallbackLocale(String,
		/// Locale) control.getFallbackLocale} method is called to get a fallback
		/// locale (alternative to the current target locale) to try further
		/// finding a resource bundle. If the method returns a non-null locale,
		/// it becomes the next target locale and the loading process starts over
		/// from Step 3. Otherwise, if a base bundle was found and put on hold in
		/// a previous Step 5, it is returned to the caller now. Otherwise, a
		/// MissingResourceException is thrown.</li>
		/// 
		/// <li>At this point, we have found a resource bundle that's not the
		/// base bundle. If this bundle set its parent during its instantiation,
		/// it is returned to the caller. Otherwise, its <a
		/// href="./ResourceBundle.html#parent_chain">parent chain</a> is
		/// instantiated based on the list of candidate locales from which it was
		/// found. Finally, the bundle is returned to the caller.</li>
		/// </ol>
		/// 
		/// <para>During the resource bundle loading process above, this factory
		/// method looks up the cache before calling the {@link
		/// Control#newBundle(String, Locale, String, ClassLoader, boolean)
		/// control.newBundle} method.  If the time-to-live period of the
		/// resource bundle found in the cache has expired, the factory method
		/// calls the {@link ResourceBundle.Control#needsReload(String, Locale,
		/// String, ClassLoader, ResourceBundle, long) control.needsReload}
		/// method to determine whether the resource bundle needs to be reloaded.
		/// If reloading is required, the factory method calls
		/// <code>control.newBundle</code> to reload the resource bundle.  If
		/// <code>control.newBundle</code> returns <code>null</code>, the factory
		/// method puts a dummy resource bundle in the cache as a mark of
		/// nonexistent resource bundles in order to avoid lookup overhead for
		/// subsequent requests. Such dummy resource bundles are under the same
		/// expiration control as specified by <code>control</code>.
		/// 
		/// </para>
		/// <para>All resource bundles loaded are cached by default. Refer to
		/// {@link Control#getTimeToLive(String,Locale)
		/// control.getTimeToLive} for details.
		/// 
		/// </para>
		/// <para>The following is an example of the bundle loading process with the
		/// default <code>ResourceBundle.Control</code> implementation.
		/// 
		/// </para>
		/// <para>Conditions:
		/// <ul>
		/// <li>Base bundle name: <code>foo.bar.Messages</code>
		/// <li>Requested <code>Locale</code>: <seealso cref="Locale#ITALY"/></li>
		/// <li>Default <code>Locale</code>: <seealso cref="Locale#FRENCH"/></li>
		/// <li>Available resource bundles:
		/// <code>foo/bar/Messages_fr.properties</code> and
		/// <code>foo/bar/Messages.properties</code></li>
		/// </ul>
		/// 
		/// </para>
		/// <para>First, <code>getBundle</code> tries loading a resource bundle in
		/// the following sequence.
		/// 
		/// <ul>
		/// <li>class <code>foo.bar.Messages_it_IT</code>
		/// <li>file <code>foo/bar/Messages_it_IT.properties</code>
		/// <li>class <code>foo.bar.Messages_it</code></li>
		/// <li>file <code>foo/bar/Messages_it.properties</code></li>
		/// <li>class <code>foo.bar.Messages</code></li>
		/// <li>file <code>foo/bar/Messages.properties</code></li>
		/// </ul>
		/// 
		/// </para>
		/// <para>At this point, <code>getBundle</code> finds
		/// <code>foo/bar/Messages.properties</code>, which is put on hold
		/// because it's the base bundle.  <code>getBundle</code> calls {@link
		/// Control#getFallbackLocale(String, Locale)
		/// control.getFallbackLocale("foo.bar.Messages", Locale.ITALY)} which
		/// returns <code>Locale.FRENCH</code>. Next, <code>getBundle</code>
		/// tries loading a bundle in the following sequence.
		/// 
		/// <ul>
		/// <li>class <code>foo.bar.Messages_fr</code></li>
		/// <li>file <code>foo/bar/Messages_fr.properties</code></li>
		/// <li>class <code>foo.bar.Messages</code></li>
		/// <li>file <code>foo/bar/Messages.properties</code></li>
		/// </ul>
		/// 
		/// </para>
		/// <para><code>getBundle</code> finds
		/// <code>foo/bar/Messages_fr.properties</code> and creates a
		/// <code>ResourceBundle</code> instance. Then, <code>getBundle</code>
		/// sets up its parent chain from the list of the candidate locales.  Only
		/// <code>foo/bar/Messages.properties</code> is found in the list and
		/// <code>getBundle</code> creates a <code>ResourceBundle</code> instance
		/// that becomes the parent of the instance for
		/// <code>foo/bar/Messages_fr.properties</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="baseName">
		///        the base name of the resource bundle, a fully qualified
		///        class name </param>
		/// <param name="targetLocale">
		///        the locale for which a resource bundle is desired </param>
		/// <param name="loader">
		///        the class loader from which to load the resource bundle </param>
		/// <param name="control">
		///        the control which gives information for the resource
		///        bundle loading process </param>
		/// <returns> a resource bundle for the given base name and locale </returns>
		/// <exception cref="NullPointerException">
		///        if <code>baseName</code>, <code>targetLocale</code>,
		///        <code>loader</code>, or <code>control</code> is
		///        <code>null</code> </exception>
		/// <exception cref="MissingResourceException">
		///        if no resource bundle for the specified base name can be found </exception>
		/// <exception cref="IllegalArgumentException">
		///        if the given <code>control</code> doesn't perform properly
		///        (e.g., <code>control.getCandidateLocales</code> returns null.)
		///        Note that validation of <code>control</code> is performed as
		///        needed.
		/// @since 1.6 </exception>
		public static ResourceBundle GetBundle(String baseName, Locale targetLocale, ClassLoader loader, Control control)
		{
			if (loader == null || control == null)
			{
				throw new NullPointerException();
			}
			return GetBundleImpl(baseName, targetLocale, loader, control);
		}

		private static Control GetDefaultControl(String baseName)
		{
			if (Providers != null)
			{
				foreach (ResourceBundleControlProvider provider in Providers)
				{
					Control control = provider.GetControl(baseName);
					if (control != null)
					{
						return control;
					}
				}
			}
			return Control.INSTANCE;
		}

		private static ResourceBundle GetBundleImpl(String baseName, Locale locale, ClassLoader loader, Control control)
		{
			if (locale == null || control == null)
			{
				throw new NullPointerException();
			}

			// We create a CacheKey here for use by this call. The base
			// name and loader will never change during the bundle loading
			// process. We have to make sure that the locale is set before
			// using it as a cache key.
			CacheKey cacheKey = new CacheKey(baseName, locale, loader);
			ResourceBundle bundle = null;

			// Quick lookup of the cache.
			BundleReference bundleRef = CacheList[cacheKey];
			if (bundleRef != null)
			{
				bundle = bundleRef.get();
				bundleRef = null;
			}

			// If this bundle and all of its parents are valid (not expired),
			// then return this bundle. If any of the bundles is expired, we
			// don't call control.needsReload here but instead drop into the
			// complete loading process below.
			if (IsValidBundle(bundle) && HasValidParentChain(bundle))
			{
				return bundle;
			}

			// No valid bundle was found in the cache, so we need to load the
			// resource bundle and its parents.

			bool isKnownControl = (control == Control.INSTANCE) || (control is SingleFormatControl);
			List<String> formats = control.GetFormats(baseName);
			if (!isKnownControl && !CheckList(formats))
			{
				throw new IllegalArgumentException("Invalid Control: getFormats");
			}

			ResourceBundle baseBundle = null;
			for (Locale targetLocale = locale; targetLocale != null; targetLocale = control.GetFallbackLocale(baseName, targetLocale))
			{
				List<Locale> candidateLocales = control.GetCandidateLocales(baseName, targetLocale);
				if (!isKnownControl && !CheckList(candidateLocales))
				{
					throw new IllegalArgumentException("Invalid Control: getCandidateLocales");
				}

				bundle = FindBundle(cacheKey, candidateLocales, formats, 0, control, baseBundle);

				// If the loaded bundle is the base bundle and exactly for the
				// requested locale or the only candidate locale, then take the
				// bundle as the resulting one. If the loaded bundle is the base
				// bundle, it's put on hold until we finish processing all
				// fallback locales.
				if (IsValidBundle(bundle))
				{
					bool isBaseBundle = Locale.ROOT.Equals(bundle.Locale_Renamed);
					if (!isBaseBundle || bundle.Locale_Renamed.Equals(locale) || (candidateLocales.Count == 1 && bundle.Locale_Renamed.Equals(candidateLocales.Get(0))))
					{
						break;
					}

					// If the base bundle has been loaded, keep the reference in
					// baseBundle so that we can avoid any redundant loading in case
					// the control specify not to cache bundles.
					if (isBaseBundle && baseBundle == null)
					{
						baseBundle = bundle;
					}
				}
			}

			if (bundle == null)
			{
				if (baseBundle == null)
				{
					ThrowMissingResourceException(baseName, locale, cacheKey.Cause);
				}
				bundle = baseBundle;
			}

			return bundle;
		}

		/// <summary>
		/// Checks if the given <code>List</code> is not null, not empty,
		/// not having null in its elements.
		/// </summary>
		private static bool checkList<T1>(List<T1> a)
		{
			bool valid = (a != null && a.Count > 0);
			if (valid)
			{
				int size = a.Count;
				for (int i = 0; valid && i < size; i++)
				{
					valid = (a.Get(i) != null);
				}
			}
			return valid;
		}

		private static ResourceBundle FindBundle(CacheKey cacheKey, List<Locale> candidateLocales, List<String> formats, int index, Control control, ResourceBundle baseBundle)
		{
			Locale targetLocale = candidateLocales.Get(index);
			ResourceBundle parent = null;
			if (index != candidateLocales.Count - 1)
			{
				parent = FindBundle(cacheKey, candidateLocales, formats, index + 1, control, baseBundle);
			}
			else if (baseBundle != null && Locale.ROOT.Equals(targetLocale))
			{
				return baseBundle;
			}

			// Before we do the real loading work, see whether we need to
			// do some housekeeping: If references to class loaders or
			// resource bundles have been nulled out, remove all related
			// information from the cache.
			Object @ref;
			while ((@ref = ReferenceQueue.poll()) != null)
			{
				CacheList.Remove(((CacheKeyReference)@ref).CacheKey);
			}

			// flag indicating the resource bundle has expired in the cache
			bool expiredBundle = false;

			// First, look up the cache to see if it's in the cache, without
			// attempting to load bundle.
			cacheKey.Locale = targetLocale;
			ResourceBundle bundle = FindBundleInCache(cacheKey, control);
			if (IsValidBundle(bundle))
			{
				expiredBundle = bundle.Expired;
				if (!expiredBundle)
				{
					// If its parent is the one asked for by the candidate
					// locales (the runtime lookup path), we can take the cached
					// one. (If it's not identical, then we'd have to check the
					// parent's parents to be consistent with what's been
					// requested.)
					if (bundle.Parent_Renamed == parent)
					{
						return bundle;
					}
					// Otherwise, remove the cached one since we can't keep
					// the same bundles having different parents.
					BundleReference bundleRef = CacheList[cacheKey];
					if (bundleRef != null && bundleRef.get() == bundle)
					{
						CacheList.Remove(cacheKey, bundleRef);
					}
				}
			}

			if (bundle != NONEXISTENT_BUNDLE)
			{
				CacheKey constKey = (CacheKey) cacheKey.Clone();

				try
				{
					bundle = LoadBundle(cacheKey, formats, control, expiredBundle);
					if (bundle != null)
					{
						if (bundle.Parent_Renamed == null)
						{
							bundle.Parent = parent;
						}
						bundle.Locale_Renamed = targetLocale;
						bundle = PutBundleInCache(cacheKey, bundle, control);
						return bundle;
					}

					// Put NONEXISTENT_BUNDLE in the cache as a mark that there's no bundle
					// instance for the locale.
					PutBundleInCache(cacheKey, NONEXISTENT_BUNDLE, control);
				}
				finally
				{
					if (constKey.Cause is InterruptedException)
					{
						Thread.CurrentThread.Interrupt();
					}
				}
			}
			return parent;
		}

		private static ResourceBundle LoadBundle(CacheKey cacheKey, List<String> formats, Control control, bool reload)
		{

			// Here we actually load the bundle in the order of formats
			// specified by the getFormats() value.
			Locale targetLocale = cacheKey.Locale;

			ResourceBundle bundle = null;
			int size = formats.Count;
			for (int i = 0; i < size; i++)
			{
				String format = formats.Get(i);
				try
				{
					bundle = control.NewBundle(cacheKey.Name, targetLocale, format, cacheKey.Loader, reload);
				}
				catch (LinkageError error)
				{
					// We need to handle the LinkageError case due to
					// inconsistent case-sensitivity in ClassLoader.
					// See 6572242 for details.
					cacheKey.Cause = error;
				}
				catch (Exception cause)
				{
					cacheKey.Cause = cause;
				}
				if (bundle != null)
				{
					// Set the format in the cache key so that it can be
					// used when calling needsReload later.
					cacheKey.Format = format;
					bundle.Name = cacheKey.Name;
					bundle.Locale_Renamed = targetLocale;
					// Bundle provider might reuse instances. So we should make
					// sure to clear the expired flag here.
					bundle.Expired = false;
					break;
				}
			}

			return bundle;
		}

		private static bool IsValidBundle(ResourceBundle bundle)
		{
			return bundle != null && bundle != NONEXISTENT_BUNDLE;
		}

		/// <summary>
		/// Determines whether any of resource bundles in the parent chain,
		/// including the leaf, have expired.
		/// </summary>
		private static bool HasValidParentChain(ResourceBundle bundle)
		{
			long now = DateTimeHelperClass.CurrentUnixTimeMillis();
			while (bundle != null)
			{
				if (bundle.Expired)
				{
					return false;
				}
				CacheKey key = bundle.CacheKey;
				if (key != null)
				{
					long expirationTime = key.ExpirationTime;
					if (expirationTime >= 0 && expirationTime <= now)
					{
						return false;
					}
				}
				bundle = bundle.Parent_Renamed;
			}
			return true;
		}

		/// <summary>
		/// Throw a MissingResourceException with proper message
		/// </summary>
		private static void ThrowMissingResourceException(String baseName, Locale locale, Throwable cause)
		{
			// If the cause is a MissingResourceException, avoid creating
			// a long chain. (6355009)
			if (cause is MissingResourceException)
			{
				cause = null;
			}
			throw new MissingResourceException("Can't find bundle for base name " + baseName + ", locale " + locale, baseName + "_" + locale, "", cause); // key -  className
		}

		/// <summary>
		/// Finds a bundle in the cache. Any expired bundles are marked as
		/// `expired' and removed from the cache upon return.
		/// </summary>
		/// <param name="cacheKey"> the key to look up the cache </param>
		/// <param name="control"> the Control to be used for the expiration control </param>
		/// <returns> the cached bundle, or null if the bundle is not found in the
		/// cache or its parent has expired. <code>bundle.expire</code> is true
		/// upon return if the bundle in the cache has expired. </returns>
		private static ResourceBundle FindBundleInCache(CacheKey cacheKey, Control control)
		{
			BundleReference bundleRef = CacheList[cacheKey];
			if (bundleRef == null)
			{
				return null;
			}
			ResourceBundle bundle = bundleRef.get();
			if (bundle == null)
			{
				return null;
			}
			ResourceBundle p = bundle.Parent_Renamed;
			Debug.Assert(p != NONEXISTENT_BUNDLE);
			// If the parent has expired, then this one must also expire. We
			// check only the immediate parent because the actual loading is
			// done from the root (base) to leaf (child) and the purpose of
			// checking is to propagate expiration towards the leaf. For
			// example, if the requested locale is ja_JP_JP and there are
			// bundles for all of the candidates in the cache, we have a list,
			//
			// base <- ja <- ja_JP <- ja_JP_JP
			//
			// If ja has expired, then it will reload ja and the list becomes a
			// tree.
			//
			// base <- ja (new)
			//  "   <- ja (expired) <- ja_JP <- ja_JP_JP
			//
			// When looking up ja_JP in the cache, it finds ja_JP in the cache
			// which references to the expired ja. Then, ja_JP is marked as
			// expired and removed from the cache. This will be propagated to
			// ja_JP_JP.
			//
			// Now, it's possible, for example, that while loading new ja_JP,
			// someone else has started loading the same bundle and finds the
			// base bundle has expired. Then, what we get from the first
			// getBundle call includes the expired base bundle. However, if
			// someone else didn't start its loading, we wouldn't know if the
			// base bundle has expired at the end of the loading process. The
			// expiration control doesn't guarantee that the returned bundle and
			// its parents haven't expired.
			//
			// We could check the entire parent chain to see if there's any in
			// the chain that has expired. But this process may never end. An
			// extreme case would be that getTimeToLive returns 0 and
			// needsReload always returns true.
			if (p != null && p.Expired)
			{
				Debug.Assert(bundle != NONEXISTENT_BUNDLE);
				bundle.Expired = true;
				bundle.CacheKey = null;
				CacheList.Remove(cacheKey, bundleRef);
				bundle = null;
			}
			else
			{
				CacheKey key = bundleRef.CacheKey;
				long expirationTime = key.ExpirationTime;
				if (!bundle.Expired && expirationTime >= 0 && expirationTime <= DateTimeHelperClass.CurrentUnixTimeMillis())
				{
					// its TTL period has expired.
					if (bundle != NONEXISTENT_BUNDLE)
					{
						// Synchronize here to call needsReload to avoid
						// redundant concurrent calls for the same bundle.
						lock (bundle)
						{
							expirationTime = key.ExpirationTime;
							if (!bundle.Expired && expirationTime >= 0 && expirationTime <= DateTimeHelperClass.CurrentUnixTimeMillis())
							{
								try
								{
									bundle.Expired = control.NeedsReload(key.Name, key.Locale, key.Format, key.Loader, bundle, key.LoadTime);
								}
								catch (Exception e)
								{
									cacheKey.Cause = e;
								}
								if (bundle.Expired)
								{
									// If the bundle needs to be reloaded, then
									// remove the bundle from the cache, but
									// return the bundle with the expired flag
									// on.
									bundle.CacheKey = null;
									CacheList.Remove(cacheKey, bundleRef);
								}
								else
								{
									// Update the expiration control info. and reuse
									// the same bundle instance
									SetExpirationTime(key, control);
								}
							}
						}
					}
					else
					{
						// We just remove NONEXISTENT_BUNDLE from the cache.
						CacheList.Remove(cacheKey, bundleRef);
						bundle = null;
					}
				}
			}
			return bundle;
		}

		/// <summary>
		/// Put a new bundle in the cache.
		/// </summary>
		/// <param name="cacheKey"> the key for the resource bundle </param>
		/// <param name="bundle"> the resource bundle to be put in the cache </param>
		/// <returns> the ResourceBundle for the cacheKey; if someone has put
		/// the bundle before this call, the one found in the cache is
		/// returned. </returns>
		private static ResourceBundle PutBundleInCache(CacheKey cacheKey, ResourceBundle bundle, Control control)
		{
			SetExpirationTime(cacheKey, control);
			if (cacheKey.ExpirationTime != Control.TTL_DONT_CACHE)
			{
				CacheKey key = (CacheKey) cacheKey.Clone();
				BundleReference bundleRef = new BundleReference(bundle, ReferenceQueue, key);
				bundle.CacheKey = key;

				// Put the bundle in the cache if it's not been in the cache.
				BundleReference result = CacheList.PutIfAbsent(key, bundleRef);

				// If someone else has put the same bundle in the cache before
				// us and it has not expired, we should use the one in the cache.
				if (result != null)
				{
					ResourceBundle rb = result.get();
					if (rb != null && !rb.Expired)
					{
						// Clear the back link to the cache key
						bundle.CacheKey = null;
						bundle = rb;
						// Clear the reference in the BundleReference so that
						// it won't be enqueued.
						bundleRef.clear();
					}
					else
					{
						// Replace the invalid (garbage collected or expired)
						// instance with the valid one.
						CacheList[key] = bundleRef;
					}
				}
			}
			return bundle;
		}

		private static void SetExpirationTime(CacheKey cacheKey, Control control)
		{
			long ttl = control.GetTimeToLive(cacheKey.Name, cacheKey.Locale);
			if (ttl >= 0)
			{
				// If any expiration time is specified, set the time to be
				// expired in the cache.
				long now = DateTimeHelperClass.CurrentUnixTimeMillis();
				cacheKey.LoadTime = now;
				cacheKey.ExpirationTime = now + ttl;
			}
			else if (ttl >= Control.TTL_NO_EXPIRATION_CONTROL)
			{
				cacheKey.ExpirationTime = ttl;
			}
			else
			{
				throw new IllegalArgumentException("Invalid Control: TTL=" + ttl);
			}
		}

		/// <summary>
		/// Removes all resource bundles from the cache that have been loaded
		/// using the caller's class loader.
		/// 
		/// @since 1.6 </summary>
		/// <seealso cref= ResourceBundle.Control#getTimeToLive(String,Locale) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static final void clearCache()
		public static void ClearCache()
		{
			ClearCache(GetLoader(Reflection.CallerClass));
		}

		/// <summary>
		/// Removes all resource bundles from the cache that have been loaded
		/// using the given class loader.
		/// </summary>
		/// <param name="loader"> the class loader </param>
		/// <exception cref="NullPointerException"> if <code>loader</code> is null
		/// @since 1.6 </exception>
		/// <seealso cref= ResourceBundle.Control#getTimeToLive(String,Locale) </seealso>
		public static void ClearCache(ClassLoader loader)
		{
			if (loader == null)
			{
				throw new NullPointerException();
			}
			ConcurrentMap<CacheKey, BundleReference>.KeyCollection set = CacheList.KeySet();
			foreach (CacheKey key in set)
			{
				if (key.Loader == loader)
				{
					set.remove(key);
				}
			}
		}

		/// <summary>
		/// Gets an object for the given key from this resource bundle.
		/// Returns null if this resource bundle does not contain an
		/// object for the given key.
		/// </summary>
		/// <param name="key"> the key for the desired object </param>
		/// <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		/// <returns> the object for the given key, or null </returns>
		protected internal abstract Object HandleGetObject(String key);

		/// <summary>
		/// Returns an enumeration of the keys.
		/// </summary>
		/// <returns> an <code>Enumeration</code> of the keys contained in
		///         this <code>ResourceBundle</code> and its parent bundles. </returns>
		public abstract IEnumerator<String> Keys {get;}

		/// <summary>
		/// Determines whether the given <code>key</code> is contained in
		/// this <code>ResourceBundle</code> or its parent bundles.
		/// </summary>
		/// <param name="key">
		///        the resource <code>key</code> </param>
		/// <returns> <code>true</code> if the given <code>key</code> is
		///        contained in this <code>ResourceBundle</code> or its
		///        parent bundles; <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException">
		///         if <code>key</code> is <code>null</code>
		/// @since 1.6 </exception>
		public virtual bool ContainsKey(String key)
		{
			if (key == null)
			{
				throw new NullPointerException();
			}
			for (ResourceBundle rb = this; rb != null; rb = rb.Parent_Renamed)
			{
				if (rb.HandleKeySet().Contains(key))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a <code>Set</code> of all keys contained in this
		/// <code>ResourceBundle</code> and its parent bundles.
		/// </summary>
		/// <returns> a <code>Set</code> of all keys contained in this
		///         <code>ResourceBundle</code> and its parent bundles.
		/// @since 1.6 </returns>
		public virtual Set<String> KeySet()
		{
			Set<String> keys = new HashSet<String>();
			for (ResourceBundle rb = this; rb != null; rb = rb.Parent_Renamed)
			{
				keys.AddAll(rb.HandleKeySet());
			}
			return keys;
		}

		/// <summary>
		/// Returns a <code>Set</code> of the keys contained <em>only</em>
		/// in this <code>ResourceBundle</code>.
		/// 
		/// <para>The default implementation returns a <code>Set</code> of the
		/// keys returned by the <seealso cref="#getKeys() getKeys"/> method except
		/// for the ones for which the {@link #handleGetObject(String)
		/// handleGetObject} method returns <code>null</code>. Once the
		/// <code>Set</code> has been created, the value is kept in this
		/// <code>ResourceBundle</code> in order to avoid producing the
		/// same <code>Set</code> in subsequent calls. Subclasses can
		/// override this method for faster handling.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <code>Set</code> of the keys contained only in this
		///        <code>ResourceBundle</code>
		/// @since 1.6 </returns>
		protected internal virtual Set<String> HandleKeySet()
		{
			if (KeySet_Renamed == null)
			{
				lock (this)
				{
					if (KeySet_Renamed == null)
					{
						Set<String> keys = new HashSet<String>();
						IEnumerator<String> enumKeys = Keys;
						while (enumKeys.MoveNext())
						{
							String key = enumKeys.Current;
							if (HandleGetObject(key) != null)
							{
								keys.Add(key);
							}
						}
						KeySet_Renamed = keys;
					}
				}
			}
			return KeySet_Renamed;
		}



		/// <summary>
		/// <code>ResourceBundle.Control</code> defines a set of callback methods
		/// that are invoked by the {@link ResourceBundle#getBundle(String,
		/// Locale, ClassLoader, Control) ResourceBundle.getBundle} factory
		/// methods during the bundle loading process. In other words, a
		/// <code>ResourceBundle.Control</code> collaborates with the factory
		/// methods for loading resource bundles. The default implementation of
		/// the callback methods provides the information necessary for the
		/// factory methods to perform the <a
		/// href="./ResourceBundle.html#default_behavior">default behavior</a>.
		/// 
		/// <para>In addition to the callback methods, the {@link
		/// #toBundleName(String, Locale) toBundleName} and {@link
		/// #toResourceName(String, String) toResourceName} methods are defined
		/// primarily for convenience in implementing the callback
		/// methods. However, the <code>toBundleName</code> method could be
		/// overridden to provide different conventions in the organization and
		/// packaging of localized resources.  The <code>toResourceName</code>
		/// method is <code>final</code> to avoid use of wrong resource and class
		/// name separators.
		/// 
		/// </para>
		/// <para>Two factory methods, <seealso cref="#getControl(List)"/> and {@link
		/// #getNoFallbackControl(List)}, provide
		/// <code>ResourceBundle.Control</code> instances that implement common
		/// variations of the default bundle loading process.
		/// 
		/// </para>
		/// <para>The formats returned by the {@link Control#getFormats(String)
		/// getFormats} method and candidate locales returned by the {@link
		/// ResourceBundle.Control#getCandidateLocales(String, Locale)
		/// getCandidateLocales} method must be consistent in all
		/// <code>ResourceBundle.getBundle</code> invocations for the same base
		/// bundle. Otherwise, the <code>ResourceBundle.getBundle</code> methods
		/// may return unintended bundles. For example, if only
		/// <code>"java.class"</code> is returned by the <code>getFormats</code>
		/// method for the first call to <code>ResourceBundle.getBundle</code>
		/// and only <code>"java.properties"</code> for the second call, then the
		/// second call will return the class-based one that has been cached
		/// during the first call.
		/// 
		/// </para>
		/// <para>A <code>ResourceBundle.Control</code> instance must be thread-safe
		/// if it's simultaneously used by multiple threads.
		/// <code>ResourceBundle.getBundle</code> does not synchronize to call
		/// the <code>ResourceBundle.Control</code> methods. The default
		/// implementations of the methods are thread-safe.
		/// 
		/// </para>
		/// <para>Applications can specify <code>ResourceBundle.Control</code>
		/// instances returned by the <code>getControl</code> factory methods or
		/// created from a subclass of <code>ResourceBundle.Control</code> to
		/// customize the bundle loading process. The following are examples of
		/// changing the default bundle loading process.
		/// 
		/// </para>
		/// <para><b>Example 1</b>
		/// 
		/// </para>
		/// <para>The following code lets <code>ResourceBundle.getBundle</code> look
		/// up only properties-based resources.
		/// 
		/// <pre>
		/// import java.util.*;
		/// import static java.util.ResourceBundle.Control.*;
		/// ...
		/// ResourceBundle bundle =
		///   ResourceBundle.getBundle("MyResources", new Locale("fr", "CH"),
		///                            ResourceBundle.Control.getControl(FORMAT_PROPERTIES));
		/// </pre>
		/// 
		/// Given the resource bundles in the <a
		/// href="./ResourceBundle.html#default_behavior_example">example</a> in
		/// the <code>ResourceBundle.getBundle</code> description, this
		/// <code>ResourceBundle.getBundle</code> call loads
		/// <code>MyResources_fr_CH.properties</code> whose parent is
		/// <code>MyResources_fr.properties</code> whose parent is
		/// <code>MyResources.properties</code>. (<code>MyResources_fr_CH.properties</code>
		/// is not hidden, but <code>MyResources_fr_CH.class</code> is.)
		/// 
		/// </para>
		/// <para><b>Example 2</b>
		/// 
		/// </para>
		/// <para>The following is an example of loading XML-based bundles
		/// using {@link Properties#loadFromXML(java.io.InputStream)
		/// Properties.loadFromXML}.
		/// 
		/// <pre>
		/// ResourceBundle rb = ResourceBundle.getBundle("Messages",
		///     new ResourceBundle.Control() {
		///         public List&lt;String&gt; getFormats(String baseName) {
		///             if (baseName == null)
		///                 throw new NullPointerException();
		///             return Arrays.asList("xml");
		///         }
		///         public ResourceBundle newBundle(String baseName,
		///                                         Locale locale,
		///                                         String format,
		///                                         ClassLoader loader,
		///                                         boolean reload)
		///                          throws IllegalAccessException,
		///                                 InstantiationException,
		///                                 IOException {
		///             if (baseName == null || locale == null
		///                   || format == null || loader == null)
		///                 throw new NullPointerException();
		///             ResourceBundle bundle = null;
		///             if (format.equals("xml")) {
		///                 String bundleName = toBundleName(baseName, locale);
		///                 String resourceName = toResourceName(bundleName, format);
		///                 InputStream stream = null;
		///                 if (reload) {
		///                     URL url = loader.getResource(resourceName);
		///                     if (url != null) {
		///                         URLConnection connection = url.openConnection();
		///                         if (connection != null) {
		///                             // Disable caches to get fresh data for
		///                             // reloading.
		///                             connection.setUseCaches(false);
		///                             stream = connection.getInputStream();
		///                         }
		///                     }
		///                 } else {
		///                     stream = loader.getResourceAsStream(resourceName);
		///                 }
		///                 if (stream != null) {
		///                     BufferedInputStream bis = new BufferedInputStream(stream);
		///                     bundle = new XMLResourceBundle(bis);
		///                     bis.close();
		///                 }
		///             }
		///             return bundle;
		///         }
		///     });
		/// 
		/// ...
		/// 
		/// private static class XMLResourceBundle extends ResourceBundle {
		///     private Properties props;
		///     XMLResourceBundle(InputStream stream) throws IOException {
		///         props = new Properties();
		///         props.loadFromXML(stream);
		///     }
		///     protected Object handleGetObject(String key) {
		///         return props.getProperty(key);
		///     }
		///     public Enumeration&lt;String&gt; getKeys() {
		///         ...
		///     }
		/// }
		/// </pre>
		/// 
		/// @since 1.6
		/// </para>
		/// </summary>
		public class Control
		{
			/// <summary>
			/// The default format <code>List</code>, which contains the strings
			/// <code>"java.class"</code> and <code>"java.properties"</code>, in
			/// this order. This <code>List</code> is {@linkplain
			/// Collections#unmodifiableList(List) unmodifiable}.
			/// </summary>
			/// <seealso cref= #getFormats(String) </seealso>
			public static readonly List<String> FORMAT_DEFAULT = Collections.unmodifiableList("java.class", "java.properties");

			/// <summary>
			/// The class-only format <code>List</code> containing
			/// <code>"java.class"</code>. This <code>List</code> is {@linkplain
			/// Collections#unmodifiableList(List) unmodifiable}.
			/// </summary>
			/// <seealso cref= #getFormats(String) </seealso>
			public static readonly List<String> FORMAT_CLASS = Collections.UnmodifiableList("java.class");

			/// <summary>
			/// The properties-only format <code>List</code> containing
			/// <code>"java.properties"</code>. This <code>List</code> is
			/// <seealso cref="Collections#unmodifiableList(List) unmodifiable"/>.
			/// </summary>
			/// <seealso cref= #getFormats(String) </seealso>
			public static readonly List<String> FORMAT_PROPERTIES = Collections.UnmodifiableList("java.properties");

			/// <summary>
			/// The time-to-live constant for not caching loaded resource bundle
			/// instances.
			/// </summary>
			/// <seealso cref= #getTimeToLive(String, Locale) </seealso>
			public const long TTL_DONT_CACHE = -1;

			/// <summary>
			/// The time-to-live constant for disabling the expiration control
			/// for loaded resource bundle instances in the cache.
			/// </summary>
			/// <seealso cref= #getTimeToLive(String, Locale) </seealso>
			public const long TTL_NO_EXPIRATION_CONTROL = -2;

			internal static readonly Control INSTANCE = new Control();

			/// <summary>
			/// Sole constructor. (For invocation by subclass constructors,
			/// typically implicit.)
			/// </summary>
			protected internal Control()
			{
			}

			/// <summary>
			/// Returns a <code>ResourceBundle.Control</code> in which the {@link
			/// #getFormats(String) getFormats} method returns the specified
			/// <code>formats</code>. The <code>formats</code> must be equal to
			/// one of <seealso cref="Control#FORMAT_PROPERTIES"/>, {@link
			/// Control#FORMAT_CLASS} or {@link
			/// Control#FORMAT_DEFAULT}. <code>ResourceBundle.Control</code>
			/// instances returned by this method are singletons and thread-safe.
			/// 
			/// <para>Specifying <seealso cref="Control#FORMAT_DEFAULT"/> is equivalent to
			/// instantiating the <code>ResourceBundle.Control</code> class,
			/// except that this method returns a singleton.
			/// 
			/// </para>
			/// </summary>
			/// <param name="formats">
			///        the formats to be returned by the
			///        <code>ResourceBundle.Control.getFormats</code> method </param>
			/// <returns> a <code>ResourceBundle.Control</code> supporting the
			///        specified <code>formats</code> </returns>
			/// <exception cref="NullPointerException">
			///        if <code>formats</code> is <code>null</code> </exception>
			/// <exception cref="IllegalArgumentException">
			///        if <code>formats</code> is unknown </exception>
			public static Control GetControl(List<String> formats)
			{
				if (formats.Equals(Control.FORMAT_PROPERTIES))
				{
					return SingleFormatControl.PROPERTIES_ONLY;
				}
				if (formats.Equals(Control.FORMAT_CLASS))
				{
					return SingleFormatControl.CLASS_ONLY;
				}
				if (formats.Equals(Control.FORMAT_DEFAULT))
				{
					return Control.INSTANCE;
				}
				throw new IllegalArgumentException();
			}

			/// <summary>
			/// Returns a <code>ResourceBundle.Control</code> in which the {@link
			/// #getFormats(String) getFormats} method returns the specified
			/// <code>formats</code> and the {@link
			/// Control#getFallbackLocale(String, Locale) getFallbackLocale}
			/// method returns <code>null</code>. The <code>formats</code> must
			/// be equal to one of <seealso cref="Control#FORMAT_PROPERTIES"/>, {@link
			/// Control#FORMAT_CLASS} or <seealso cref="Control#FORMAT_DEFAULT"/>.
			/// <code>ResourceBundle.Control</code> instances returned by this
			/// method are singletons and thread-safe.
			/// </summary>
			/// <param name="formats">
			///        the formats to be returned by the
			///        <code>ResourceBundle.Control.getFormats</code> method </param>
			/// <returns> a <code>ResourceBundle.Control</code> supporting the
			///        specified <code>formats</code> with no fallback
			///        <code>Locale</code> support </returns>
			/// <exception cref="NullPointerException">
			///        if <code>formats</code> is <code>null</code> </exception>
			/// <exception cref="IllegalArgumentException">
			///        if <code>formats</code> is unknown </exception>
			public static Control GetNoFallbackControl(List<String> formats)
			{
				if (formats.Equals(Control.FORMAT_DEFAULT))
				{
					return NoFallbackControl.NO_FALLBACK;
				}
				if (formats.Equals(Control.FORMAT_PROPERTIES))
				{
					return NoFallbackControl.PROPERTIES_ONLY_NO_FALLBACK;
				}
				if (formats.Equals(Control.FORMAT_CLASS))
				{
					return NoFallbackControl.CLASS_ONLY_NO_FALLBACK;
				}
				throw new IllegalArgumentException();
			}

			/// <summary>
			/// Returns a <code>List</code> of <code>String</code>s containing
			/// formats to be used to load resource bundles for the given
			/// <code>baseName</code>. The <code>ResourceBundle.getBundle</code>
			/// factory method tries to load resource bundles with formats in the
			/// order specified by the list. The list returned by this method
			/// must have at least one <code>String</code>. The predefined
			/// formats are <code>"java.class"</code> for class-based resource
			/// bundles and <code>"java.properties"</code> for {@linkplain
			/// PropertyResourceBundle properties-based} ones. Strings starting
			/// with <code>"java."</code> are reserved for future extensions and
			/// must not be used by application-defined formats.
			/// 
			/// <para>It is not a requirement to return an immutable (unmodifiable)
			/// <code>List</code>.  However, the returned <code>List</code> must
			/// not be mutated after it has been returned by
			/// <code>getFormats</code>.
			/// 
			/// </para>
			/// <para>The default implementation returns <seealso cref="#FORMAT_DEFAULT"/> so
			/// that the <code>ResourceBundle.getBundle</code> factory method
			/// looks up first class-based resource bundles, then
			/// properties-based ones.
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base name of the resource bundle, a fully qualified class
			///        name </param>
			/// <returns> a <code>List</code> of <code>String</code>s containing
			///        formats for loading resource bundles. </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code> is null </exception>
			/// <seealso cref= #FORMAT_DEFAULT </seealso>
			/// <seealso cref= #FORMAT_CLASS </seealso>
			/// <seealso cref= #FORMAT_PROPERTIES </seealso>
			public virtual List<String> GetFormats(String baseName)
			{
				if (baseName == null)
				{
					throw new NullPointerException();
				}
				return FORMAT_DEFAULT;
			}

			/// <summary>
			/// Returns a <code>List</code> of <code>Locale</code>s as candidate
			/// locales for <code>baseName</code> and <code>locale</code>. This
			/// method is called by the <code>ResourceBundle.getBundle</code>
			/// factory method each time the factory method tries finding a
			/// resource bundle for a target <code>Locale</code>.
			/// 
			/// <para>The sequence of the candidate locales also corresponds to the
			/// runtime resource lookup path (also known as the <I>parent
			/// chain</I>), if the corresponding resource bundles for the
			/// candidate locales exist and their parents are not defined by
			/// loaded resource bundles themselves.  The last element of the list
			/// must be a <seealso cref="Locale#ROOT root locale"/> if it is desired to
			/// have the base bundle as the terminal of the parent chain.
			/// 
			/// </para>
			/// <para>If the given locale is equal to <code>Locale.ROOT</code> (the
			/// root locale), a <code>List</code> containing only the root
			/// <code>Locale</code> must be returned. In this case, the
			/// <code>ResourceBundle.getBundle</code> factory method loads only
			/// the base bundle as the resulting resource bundle.
			/// 
			/// </para>
			/// <para>It is not a requirement to return an immutable (unmodifiable)
			/// <code>List</code>. However, the returned <code>List</code> must not
			/// be mutated after it has been returned by
			/// <code>getCandidateLocales</code>.
			/// 
			/// </para>
			/// <para>The default implementation returns a <code>List</code> containing
			/// <code>Locale</code>s using the rules described below.  In the
			/// description below, <em>L</em>, <em>S</em>, <em>C</em> and <em>V</em>
			/// respectively represent non-empty language, script, country, and
			/// variant.  For example, [<em>L</em>, <em>C</em>] represents a
			/// <code>Locale</code> that has non-empty values only for language and
			/// country.  The form <em>L</em>("xx") represents the (non-empty)
			/// language value is "xx".  For all cases, <code>Locale</code>s whose
			/// final component values are empty strings are omitted.
			/// 
			/// <ol><li>For an input <code>Locale</code> with an empty script value,
			/// append candidate <code>Locale</code>s by omitting the final component
			/// one by one as below:
			/// 
			/// <ul>
			/// <li> [<em>L</em>, <em>C</em>, <em>V</em>] </li>
			/// <li> [<em>L</em>, <em>C</em>] </li>
			/// <li> [<em>L</em>] </li>
			/// <li> <code>Locale.ROOT</code> </li>
			/// </ul></li>
			/// 
			/// <li>For an input <code>Locale</code> with a non-empty script value,
			/// append candidate <code>Locale</code>s by omitting the final component
			/// up to language, then append candidates generated from the
			/// <code>Locale</code> with country and variant restored:
			/// 
			/// <ul>
			/// <li> [<em>L</em>, <em>S</em>, <em>C</em>, <em>V</em>]</li>
			/// <li> [<em>L</em>, <em>S</em>, <em>C</em>]</li>
			/// <li> [<em>L</em>, <em>S</em>]</li>
			/// <li> [<em>L</em>, <em>C</em>, <em>V</em>]</li>
			/// <li> [<em>L</em>, <em>C</em>]</li>
			/// <li> [<em>L</em>]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul></li>
			/// 
			/// <li>For an input <code>Locale</code> with a variant value consisting
			/// of multiple subtags separated by underscore, generate candidate
			/// <code>Locale</code>s by omitting the variant subtags one by one, then
			/// insert them after every occurrence of <code> Locale</code>s with the
			/// full variant value in the original list.  For example, if the
			/// the variant consists of two subtags <em>V1</em> and <em>V2</em>:
			/// 
			/// <ul>
			/// <li> [<em>L</em>, <em>S</em>, <em>C</em>, <em>V1</em>, <em>V2</em>]</li>
			/// <li> [<em>L</em>, <em>S</em>, <em>C</em>, <em>V1</em>]</li>
			/// <li> [<em>L</em>, <em>S</em>, <em>C</em>]</li>
			/// <li> [<em>L</em>, <em>S</em>]</li>
			/// <li> [<em>L</em>, <em>C</em>, <em>V1</em>, <em>V2</em>]</li>
			/// <li> [<em>L</em>, <em>C</em>, <em>V1</em>]</li>
			/// <li> [<em>L</em>, <em>C</em>]</li>
			/// <li> [<em>L</em>]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul></li>
			/// 
			/// <li>Special cases for Chinese.  When an input <code>Locale</code> has the
			/// language "zh" (Chinese) and an empty script value, either "Hans" (Simplified) or
			/// "Hant" (Traditional) might be supplied, depending on the country.
			/// When the country is "CN" (China) or "SG" (Singapore), "Hans" is supplied.
			/// When the country is "HK" (Hong Kong SAR China), "MO" (Macau SAR China),
			/// or "TW" (Taiwan), "Hant" is supplied.  For all other countries or when the country
			/// is empty, no script is supplied.  For example, for <code>Locale("zh", "CN")
			/// </code>, the candidate list will be:
			/// <ul>
			/// <li> [<em>L</em>("zh"), <em>S</em>("Hans"), <em>C</em>("CN")]</li>
			/// <li> [<em>L</em>("zh"), <em>S</em>("Hans")]</li>
			/// <li> [<em>L</em>("zh"), <em>C</em>("CN")]</li>
			/// <li> [<em>L</em>("zh")]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul>
			/// 
			/// For <code>Locale("zh", "TW")</code>, the candidate list will be:
			/// <ul>
			/// <li> [<em>L</em>("zh"), <em>S</em>("Hant"), <em>C</em>("TW")]</li>
			/// <li> [<em>L</em>("zh"), <em>S</em>("Hant")]</li>
			/// <li> [<em>L</em>("zh"), <em>C</em>("TW")]</li>
			/// <li> [<em>L</em>("zh")]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul></li>
			/// 
			/// <li>Special cases for Norwegian.  Both <code>Locale("no", "NO",
			/// "NY")</code> and <code>Locale("nn", "NO")</code> represent Norwegian
			/// Nynorsk.  When a locale's language is "nn", the standard candidate
			/// list is generated up to [<em>L</em>("nn")], and then the following
			/// candidates are added:
			/// 
			/// <ul><li> [<em>L</em>("no"), <em>C</em>("NO"), <em>V</em>("NY")]</li>
			/// <li> [<em>L</em>("no"), <em>C</em>("NO")]</li>
			/// <li> [<em>L</em>("no")]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul>
			/// 
			/// If the locale is exactly <code>Locale("no", "NO", "NY")</code>, it is first
			/// converted to <code>Locale("nn", "NO")</code> and then the above procedure is
			/// followed.
			/// 
			/// </para>
			/// <para>Also, Java treats the language "no" as a synonym of Norwegian
			/// Bokm&#xE5;l "nb".  Except for the single case <code>Locale("no",
			/// "NO", "NY")</code> (handled above), when an input <code>Locale</code>
			/// has language "no" or "nb", candidate <code>Locale</code>s with
			/// language code "no" and "nb" are interleaved, first using the
			/// requested language, then using its synonym. For example,
			/// <code>Locale("nb", "NO", "POSIX")</code> generates the following
			/// candidate list:
			/// 
			/// <ul>
			/// <li> [<em>L</em>("nb"), <em>C</em>("NO"), <em>V</em>("POSIX")]</li>
			/// <li> [<em>L</em>("no"), <em>C</em>("NO"), <em>V</em>("POSIX")]</li>
			/// <li> [<em>L</em>("nb"), <em>C</em>("NO")]</li>
			/// <li> [<em>L</em>("no"), <em>C</em>("NO")]</li>
			/// <li> [<em>L</em>("nb")]</li>
			/// <li> [<em>L</em>("no")]</li>
			/// <li> <code>Locale.ROOT</code></li>
			/// </ul>
			/// 
			/// <code>Locale("no", "NO", "POSIX")</code> would generate the same list
			/// except that locales with "no" would appear before the corresponding
			/// locales with "nb".</li>
			/// </ol>
			/// 
			/// </para>
			/// <para>The default implementation uses an <seealso cref="ArrayList"/> that
			/// overriding implementations may modify before returning it to the
			/// caller. However, a subclass must not modify it after it has
			/// been returned by <code>getCandidateLocales</code>.
			/// 
			/// </para>
			/// <para>For example, if the given <code>baseName</code> is "Messages"
			/// and the given <code>locale</code> is
			/// <code>Locale("ja",&nbsp;"",&nbsp;"XX")</code>, then a
			/// <code>List</code> of <code>Locale</code>s:
			/// <pre>
			///     Locale("ja", "", "XX")
			///     Locale("ja")
			///     Locale.ROOT
			/// </pre>
			/// is returned. And if the resource bundles for the "ja" and
			/// "" <code>Locale</code>s are found, then the runtime resource
			/// lookup path (parent chain) is:
			/// <pre>{@code
			///     Messages_ja -> Messages
			/// }</pre>
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base name of the resource bundle, a fully
			///        qualified class name </param>
			/// <param name="locale">
			///        the locale for which a resource bundle is desired </param>
			/// <returns> a <code>List</code> of candidate
			///        <code>Locale</code>s for the given <code>locale</code> </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code> or <code>locale</code> is
			///        <code>null</code> </exception>
			public virtual List<Locale> GetCandidateLocales(String baseName, Locale locale)
			{
				if (baseName == null)
				{
					throw new NullPointerException();
				}
				return new List<>(CANDIDATES_CACHE.get(locale.BaseLocale));
			}

			internal static readonly CandidateListCache CANDIDATES_CACHE = new CandidateListCache();

			private class CandidateListCache : LocaleObjectCache<BaseLocale, List<Locale>>
			{
				protected internal virtual List<Locale> CreateObject(BaseLocale @base)
				{
					String language = @base.Language;
					String script = @base.Script;
					String region = @base.Region;
					String variant = @base.Variant;

					// Special handling for Norwegian
					bool isNorwegianBokmal = false;
					bool isNorwegianNynorsk = false;
					if (language.Equals("no"))
					{
						if (region.Equals("NO") && variant.Equals("NY"))
						{
							variant = "";
							isNorwegianNynorsk = true;
						}
						else
						{
							isNorwegianBokmal = true;
						}
					}
					if (language.Equals("nb") || isNorwegianBokmal)
					{
						List<Locale> tmpList = GetDefaultList("nb", script, region, variant);
						// Insert a locale replacing "nb" with "no" for every list entry
						List<Locale> bokmalList = new LinkedList<Locale>();
						foreach (Locale l in tmpList)
						{
							bokmalList.Add(l);
							if (l.Language.Length() == 0)
							{
								break;
							}
							bokmalList.Add(Locale.GetInstance("no", l.Script, l.Country, l.Variant, null));
						}
						return bokmalList;
					}
					else if (language.Equals("nn") || isNorwegianNynorsk)
					{
						// Insert no_NO_NY, no_NO, no after nn
						List<Locale> nynorskList = GetDefaultList("nn", script, region, variant);
						int idx = nynorskList.Count - 1;
						nynorskList.Add(idx++, Locale.GetInstance("no", "NO", "NY"));
						nynorskList.Add(idx++, Locale.GetInstance("no", "NO", ""));
						nynorskList.Add(idx++, Locale.GetInstance("no", "", ""));
						return nynorskList;
					}
					// Special handling for Chinese
					else if (language.Equals("zh"))
					{
						if (script.Length() == 0 && region.Length() > 0)
						{
							// Supply script for users who want to use zh_Hans/zh_Hant
							// as bundle names (recommended for Java7+)
							switch (region)
							{
							case "TW":
							case "HK":
							case "MO":
								script = "Hant";
								break;
							case "CN":
							case "SG":
								script = "Hans";
								break;
							}
						}
						else if (script.Length() > 0 && region.Length() == 0)
						{
							// Supply region(country) for users who still package Chinese
							// bundles using old convension.
							switch (script)
							{
							case "Hans":
								region = "CN";
								break;
							case "Hant":
								region = "TW";
								break;
							}
						}
					}

					return GetDefaultList(language, script, region, variant);
				}

				internal static List<Locale> GetDefaultList(String language, String script, String region, String variant)
				{
					List<String> variants = null;

					if (variant.Length() > 0)
					{
						variants = new LinkedList<>();
						int idx = variant.Length();
						while (idx != -1)
						{
							variants.Add(variant.Substring(0, idx));
							idx = variant.LastIndexOf('_', --idx);
						}
					}

					List<Locale> list = new LinkedList<Locale>();

					if (variants != null)
					{
						foreach (String v in variants)
						{
							list.Add(Locale.GetInstance(language, script, region, v, null));
						}
					}
					if (region.Length() > 0)
					{
						list.Add(Locale.GetInstance(language, script, region, "", null));
					}
					if (script.Length() > 0)
					{
						list.Add(Locale.GetInstance(language, script, "", "", null));

						// With script, after truncating variant, region and script,
						// start over without script.
						if (variants != null)
						{
							foreach (String v in variants)
							{
								list.Add(Locale.GetInstance(language, "", region, v, null));
							}
						}
						if (region.Length() > 0)
						{
							list.Add(Locale.GetInstance(language, "", region, "", null));
						}
					}
					if (language.Length() > 0)
					{
						list.Add(Locale.GetInstance(language, "", "", "", null));
					}
					// Add root locale at the end
					list.Add(Locale.ROOT);

					return list;
				}
			}

			/// <summary>
			/// Returns a <code>Locale</code> to be used as a fallback locale for
			/// further resource bundle searches by the
			/// <code>ResourceBundle.getBundle</code> factory method. This method
			/// is called from the factory method every time when no resulting
			/// resource bundle has been found for <code>baseName</code> and
			/// <code>locale</code>, where locale is either the parameter for
			/// <code>ResourceBundle.getBundle</code> or the previous fallback
			/// locale returned by this method.
			/// 
			/// <para>The method returns <code>null</code> if no further fallback
			/// search is desired.
			/// 
			/// </para>
			/// <para>The default implementation returns the {@linkplain
			/// Locale#getDefault() default <code>Locale</code>} if the given
			/// <code>locale</code> isn't the default one.  Otherwise,
			/// <code>null</code> is returned.
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base name of the resource bundle, a fully
			///        qualified class name for which
			///        <code>ResourceBundle.getBundle</code> has been
			///        unable to find any resource bundles (except for the
			///        base bundle) </param>
			/// <param name="locale">
			///        the <code>Locale</code> for which
			///        <code>ResourceBundle.getBundle</code> has been
			///        unable to find any resource bundles (except for the
			///        base bundle) </param>
			/// <returns> a <code>Locale</code> for the fallback search,
			///        or <code>null</code> if no further fallback search
			///        is desired. </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code> or <code>locale</code>
			///        is <code>null</code> </exception>
			public virtual Locale GetFallbackLocale(String baseName, Locale locale)
			{
				if (baseName == null)
				{
					throw new NullPointerException();
				}
				Locale defaultLocale = Locale.Default;
				return locale.Equals(defaultLocale) ? null : defaultLocale;
			}

			/// <summary>
			/// Instantiates a resource bundle for the given bundle name of the
			/// given format and locale, using the given class loader if
			/// necessary. This method returns <code>null</code> if there is no
			/// resource bundle available for the given parameters. If a resource
			/// bundle can't be instantiated due to an unexpected error, the
			/// error must be reported by throwing an <code>Error</code> or
			/// <code>Exception</code> rather than simply returning
			/// <code>null</code>.
			/// 
			/// <para>If the <code>reload</code> flag is <code>true</code>, it
			/// indicates that this method is being called because the previously
			/// loaded resource bundle has expired.
			/// 
			/// </para>
			/// <para>The default implementation instantiates a
			/// <code>ResourceBundle</code> as follows.
			/// 
			/// <ul>
			/// 
			/// <li>The bundle name is obtained by calling {@link
			/// #toBundleName(String, Locale) toBundleName(baseName,
			/// locale)}.</li>
			/// 
			/// <li>If <code>format</code> is <code>"java.class"</code>, the
			/// <seealso cref="Class"/> specified by the bundle name is loaded by calling
			/// <seealso cref="ClassLoader#loadClass(String)"/>. Then, a
			/// <code>ResourceBundle</code> is instantiated by calling {@link
			/// Class#newInstance()}.  Note that the <code>reload</code> flag is
			/// ignored for loading class-based resource bundles in this default
			/// implementation.</li>
			/// 
			/// <li>If <code>format</code> is <code>"java.properties"</code>,
			/// {@link #toResourceName(String, String) toResourceName(bundlename,
			/// "properties")} is called to get the resource name.
			/// If <code>reload</code> is <code>true</code>, {@link
			/// ClassLoader#getResource(String) load.getResource} is called
			/// to get a <seealso cref="URL"/> for creating a {@link
			/// URLConnection}. This <code>URLConnection</code> is used to
			/// {@link URLConnection#setUseCaches(boolean) disable the
			/// caches} of the underlying resource loading layers,
			/// and to {@link URLConnection#getInputStream() get an
			/// <code>InputStream</code>}.
			/// Otherwise, {@link ClassLoader#getResourceAsStream(String)
			/// loader.getResourceAsStream} is called to get an {@link
			/// InputStream}. Then, a {@link
			/// PropertyResourceBundle} is constructed with the
			/// <code>InputStream</code>.</li>
			/// 
			/// <li>If <code>format</code> is neither <code>"java.class"</code>
			/// nor <code>"java.properties"</code>, an
			/// <code>IllegalArgumentException</code> is thrown.</li>
			/// 
			/// </ul>
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base bundle name of the resource bundle, a fully
			///        qualified class name </param>
			/// <param name="locale">
			///        the locale for which the resource bundle should be
			///        instantiated </param>
			/// <param name="format">
			///        the resource bundle format to be loaded </param>
			/// <param name="loader">
			///        the <code>ClassLoader</code> to use to load the bundle </param>
			/// <param name="reload">
			///        the flag to indicate bundle reloading; <code>true</code>
			///        if reloading an expired resource bundle,
			///        <code>false</code> otherwise </param>
			/// <returns> the resource bundle instance,
			///        or <code>null</code> if none could be found. </returns>
			/// <exception cref="NullPointerException">
			///        if <code>bundleName</code>, <code>locale</code>,
			///        <code>format</code>, or <code>loader</code> is
			///        <code>null</code>, or if <code>null</code> is returned by
			///        <seealso cref="#toBundleName(String, Locale) toBundleName"/> </exception>
			/// <exception cref="IllegalArgumentException">
			///        if <code>format</code> is unknown, or if the resource
			///        found for the given parameters contains malformed data. </exception>
			/// <exception cref="ClassCastException">
			///        if the loaded class cannot be cast to <code>ResourceBundle</code> </exception>
			/// <exception cref="IllegalAccessException">
			///        if the class or its nullary constructor is not
			///        accessible. </exception>
			/// <exception cref="InstantiationException">
			///        if the instantiation of a class fails for some other
			///        reason. </exception>
			/// <exception cref="ExceptionInInitializerError">
			///        if the initialization provoked by this method fails. </exception>
			/// <exception cref="SecurityException">
			///        If a security manager is present and creation of new
			///        instances is denied. See <seealso cref="Class#newInstance()"/>
			///        for details. </exception>
			/// <exception cref="IOException">
			///        if an error occurred when reading resources using
			///        any I/O operations </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResourceBundle newBundle(String baseName, Locale locale, String format, ClassLoader loader, boolean reload) throws IllegalAccessException, InstantiationException, java.io.IOException
			public virtual ResourceBundle NewBundle(String baseName, Locale locale, String format, ClassLoader loader, bool reload)
			{
				String bundleName = ToBundleName(baseName, locale);
				ResourceBundle bundle = null;
				if (format.Equals("java.class"))
				{
					try
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Class bundleClass = (Class)loader.loadClass(bundleName);
						Class bundleClass = (Class)loader.LoadClass(bundleName);

						// If the class isn't a ResourceBundle subclass, throw a
						// ClassCastException.
						if (bundleClass.IsSubclassOf(typeof(ResourceBundle)))
						{
							bundle = bundleClass.NewInstance();
						}
						else
						{
							throw new ClassCastException(bundleClass.Name + " cannot be cast to ResourceBundle");
						}
					}
					catch (ClassNotFoundException)
					{
					}
				}
				else if (format.Equals("java.properties"))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName = toResourceName0(bundleName, "properties");
					String resourceName = ToResourceName0(bundleName, "properties");
					if (resourceName == null)
					{
						return bundle;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader classLoader = loader;
					ClassLoader classLoader = loader;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean reloadFlag = reload;
					bool reloadFlag = reload;
					InputStream stream = null;
					try
					{
						stream = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, resourceName, classLoader, reloadFlag));
					}
					catch (PrivilegedActionException e)
					{
						throw (IOException) e.Exception;
					}
					if (stream != null)
					{
						try
						{
							bundle = new PropertyResourceBundle(stream);
						}
						finally
						{
							stream.Close();
						}
					}
				}
				else
				{
					throw new IllegalArgumentException("unknown format: " + format);
				}
				return bundle;
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<InputStream>
			{
				private readonly Control OuterInstance;

				private string ResourceName;
				private java.lang.ClassLoader ClassLoader;
				private bool ReloadFlag;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(Control outerInstance, string resourceName, java.lang.ClassLoader classLoader, bool reloadFlag)
				{
					this.OuterInstance = outerInstance;
					this.ResourceName = resourceName;
					this.ClassLoader = classLoader;
					this.ReloadFlag = reloadFlag;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream run() throws java.io.IOException
				public virtual InputStream Run()
				{
					InputStream @is = null;
					if (ReloadFlag)
					{
						URL url = ClassLoader.GetResource(ResourceName);
						if (url != null)
						{
							URLConnection connection = url.OpenConnection();
							if (connection != null)
							{
								// Disable caches to get fresh data for
								// reloading.
								connection.UseCaches = false;
								@is = connection.InputStream;
							}
						}
					}
					else
					{
						@is = ClassLoader.GetResourceAsStream(ResourceName);
					}
					return @is;
				}
			}

			/// <summary>
			/// Returns the time-to-live (TTL) value for resource bundles that
			/// are loaded under this
			/// <code>ResourceBundle.Control</code>. Positive time-to-live values
			/// specify the number of milliseconds a bundle can remain in the
			/// cache without being validated against the source data from which
			/// it was constructed. The value 0 indicates that a bundle must be
			/// validated each time it is retrieved from the cache. {@link
			/// #TTL_DONT_CACHE} specifies that loaded resource bundles are not
			/// put in the cache. <seealso cref="#TTL_NO_EXPIRATION_CONTROL"/> specifies
			/// that loaded resource bundles are put in the cache with no
			/// expiration control.
			/// 
			/// <para>The expiration affects only the bundle loading process by the
			/// <code>ResourceBundle.getBundle</code> factory method.  That is,
			/// if the factory method finds a resource bundle in the cache that
			/// has expired, the factory method calls the {@link
			/// #needsReload(String, Locale, String, ClassLoader, ResourceBundle,
			/// long) needsReload} method to determine whether the resource
			/// bundle needs to be reloaded. If <code>needsReload</code> returns
			/// <code>true</code>, the cached resource bundle instance is removed
			/// from the cache. Otherwise, the instance stays in the cache,
			/// updated with the new TTL value returned by this method.
			/// 
			/// </para>
			/// <para>All cached resource bundles are subject to removal from the
			/// cache due to memory constraints of the runtime environment.
			/// Returning a large positive value doesn't mean to lock loaded
			/// resource bundles in the cache.
			/// 
			/// </para>
			/// <para>The default implementation returns <seealso cref="#TTL_NO_EXPIRATION_CONTROL"/>.
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base name of the resource bundle for which the
			///        expiration value is specified. </param>
			/// <param name="locale">
			///        the locale of the resource bundle for which the
			///        expiration value is specified. </param>
			/// <returns> the time (0 or a positive millisecond offset from the
			///        cached time) to get loaded bundles expired in the cache,
			///        <seealso cref="#TTL_NO_EXPIRATION_CONTROL"/> to disable the
			///        expiration control, or <seealso cref="#TTL_DONT_CACHE"/> to disable
			///        caching. </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code> or <code>locale</code> is
			///        <code>null</code> </exception>
			public virtual long GetTimeToLive(String baseName, Locale locale)
			{
				if (baseName == null || locale == null)
				{
					throw new NullPointerException();
				}
				return TTL_NO_EXPIRATION_CONTROL;
			}

			/// <summary>
			/// Determines if the expired <code>bundle</code> in the cache needs
			/// to be reloaded based on the loading time given by
			/// <code>loadTime</code> or some other criteria. The method returns
			/// <code>true</code> if reloading is required; <code>false</code>
			/// otherwise. <code>loadTime</code> is a millisecond offset since
			/// the <a href="Calendar.html#Epoch"> <code>Calendar</code>
			/// Epoch</a>.
			/// 
			/// The calling <code>ResourceBundle.getBundle</code> factory method
			/// calls this method on the <code>ResourceBundle.Control</code>
			/// instance used for its current invocation, not on the instance
			/// used in the invocation that originally loaded the resource
			/// bundle.
			/// 
			/// <para>The default implementation compares <code>loadTime</code> and
			/// the last modified time of the source data of the resource
			/// bundle. If it's determined that the source data has been modified
			/// since <code>loadTime</code>, <code>true</code> is
			/// returned. Otherwise, <code>false</code> is returned. This
			/// implementation assumes that the given <code>format</code> is the
			/// same string as its file suffix if it's not one of the default
			/// formats, <code>"java.class"</code> or
			/// <code>"java.properties"</code>.
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base bundle name of the resource bundle, a
			///        fully qualified class name </param>
			/// <param name="locale">
			///        the locale for which the resource bundle
			///        should be instantiated </param>
			/// <param name="format">
			///        the resource bundle format to be loaded </param>
			/// <param name="loader">
			///        the <code>ClassLoader</code> to use to load the bundle </param>
			/// <param name="bundle">
			///        the resource bundle instance that has been expired
			///        in the cache </param>
			/// <param name="loadTime">
			///        the time when <code>bundle</code> was loaded and put
			///        in the cache </param>
			/// <returns> <code>true</code> if the expired bundle needs to be
			///        reloaded; <code>false</code> otherwise. </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code>, <code>locale</code>,
			///        <code>format</code>, <code>loader</code>, or
			///        <code>bundle</code> is <code>null</code> </exception>
			public virtual bool NeedsReload(String baseName, Locale locale, String format, ClassLoader loader, ResourceBundle bundle, long loadTime)
			{
				if (bundle == null)
				{
					throw new NullPointerException();
				}
				if (format.Equals("java.class") || format.Equals("java.properties"))
				{
					format = format.Substring(5);
				}
				bool result = false;
				try
				{
					String resourceName = ToResourceName0(ToBundleName(baseName, locale), format);
					if (resourceName == null)
					{
						return result;
					}
					URL url = loader.GetResource(resourceName);
					if (url != null)
					{
						long lastModified = 0;
						URLConnection connection = url.OpenConnection();
						if (connection != null)
						{
							// disable caches to get the correct data
							connection.UseCaches = false;
							if (connection is JarURLConnection)
							{
								JarEntry ent = ((JarURLConnection)connection).JarEntry;
								if (ent != null)
								{
									lastModified = ent.Time;
									if (lastModified == -1)
									{
										lastModified = 0;
									}
								}
							}
							else
							{
								lastModified = connection.LastModified;
							}
						}
						result = lastModified >= loadTime;
					}
				}
				catch (NullPointerException npe)
				{
					throw npe;
				}
				catch (Exception)
				{
					// ignore other exceptions
				}
				return result;
			}

			/// <summary>
			/// Converts the given <code>baseName</code> and <code>locale</code>
			/// to the bundle name. This method is called from the default
			/// implementation of the {@link #newBundle(String, Locale, String,
			/// ClassLoader, boolean) newBundle} and {@link #needsReload(String,
			/// Locale, String, ClassLoader, ResourceBundle, long) needsReload}
			/// methods.
			/// 
			/// <para>This implementation returns the following value:
			/// <pre>
			///     baseName + "_" + language + "_" + script + "_" + country + "_" + variant
			/// </pre>
			/// where <code>language</code>, <code>script</code>, <code>country</code>,
			/// and <code>variant</code> are the language, script, country, and variant
			/// values of <code>locale</code>, respectively. Final component values that
			/// are empty Strings are omitted along with the preceding '_'.  When the
			/// script is empty, the script value is omitted along with the preceding '_'.
			/// If all of the values are empty strings, then <code>baseName</code>
			/// is returned.
			/// 
			/// </para>
			/// <para>For example, if <code>baseName</code> is
			/// <code>"baseName"</code> and <code>locale</code> is
			/// <code>Locale("ja",&nbsp;"",&nbsp;"XX")</code>, then
			/// <code>"baseName_ja_&thinsp;_XX"</code> is returned. If the given
			/// locale is <code>Locale("en")</code>, then
			/// <code>"baseName_en"</code> is returned.
			/// 
			/// </para>
			/// <para>Overriding this method allows applications to use different
			/// conventions in the organization and packaging of localized
			/// resources.
			/// 
			/// </para>
			/// </summary>
			/// <param name="baseName">
			///        the base name of the resource bundle, a fully
			///        qualified class name </param>
			/// <param name="locale">
			///        the locale for which a resource bundle should be
			///        loaded </param>
			/// <returns> the bundle name for the resource bundle </returns>
			/// <exception cref="NullPointerException">
			///        if <code>baseName</code> or <code>locale</code>
			///        is <code>null</code> </exception>
			public virtual String ToBundleName(String baseName, Locale locale)
			{
				if (locale == Locale.ROOT)
				{
					return baseName;
				}

				String language = locale.Language;
				String script = locale.Script;
				String country = locale.Country;
				String variant = locale.Variant;

				if (language == "" && country == "" && variant == "")
				{
					return baseName;
				}

				StringBuilder sb = new StringBuilder(baseName);
				sb.Append('_');
				if (script != "")
				{
					if (variant != "")
					{
						sb.Append(language).Append('_').Append(script).Append('_').Append(country).Append('_').Append(variant);
					}
					else if (country != "")
					{
						sb.Append(language).Append('_').Append(script).Append('_').Append(country);
					}
					else
					{
						sb.Append(language).Append('_').Append(script);
					}
				}
				else
				{
					if (variant != "")
					{
						sb.Append(language).Append('_').Append(country).Append('_').Append(variant);
					}
					else if (country != "")
					{
						sb.Append(language).Append('_').Append(country);
					}
					else
					{
						sb.Append(language);
					}
				}
				return sb.ToString();

			}

			/// <summary>
			/// Converts the given <code>bundleName</code> to the form required
			/// by the <seealso cref="ClassLoader#getResource ClassLoader.getResource"/>
			/// method by replacing all occurrences of <code>'.'</code> in
			/// <code>bundleName</code> with <code>'/'</code> and appending a
			/// <code>'.'</code> and the given file <code>suffix</code>. For
			/// example, if <code>bundleName</code> is
			/// <code>"foo.bar.MyResources_ja_JP"</code> and <code>suffix</code>
			/// is <code>"properties"</code>, then
			/// <code>"foo/bar/MyResources_ja_JP.properties"</code> is returned.
			/// </summary>
			/// <param name="bundleName">
			///        the bundle name </param>
			/// <param name="suffix">
			///        the file type suffix </param>
			/// <returns> the converted resource name </returns>
			/// <exception cref="NullPointerException">
			///         if <code>bundleName</code> or <code>suffix</code>
			///         is <code>null</code> </exception>
			public String ToResourceName(String bundleName, String suffix)
			{
				StringBuilder sb = new StringBuilder(bundleName.Length() + 1 + suffix.Length());
				sb.Append(bundleName.Replace('.', '/')).Append('.').Append(suffix);
				return sb.ToString();
			}

			internal virtual String ToResourceName0(String bundleName, String suffix)
			{
				// application protocol check
				if (bundleName.Contains("://"))
				{
					return null;
				}
				else
				{
					return ToResourceName(bundleName, suffix);
				}
			}
		}

		private class SingleFormatControl : Control
		{
			internal static readonly Control PROPERTIES_ONLY = new SingleFormatControl(FORMAT_PROPERTIES);

			internal static readonly Control CLASS_ONLY = new SingleFormatControl(FORMAT_CLASS);

			internal readonly List<String> Formats;

			protected internal SingleFormatControl(List<String> formats)
			{
				this.Formats = formats;
			}

			public override List<String> GetFormats(String baseName)
			{
				if (baseName == null)
				{
					throw new NullPointerException();
				}
				return Formats;
			}
		}

		private sealed class NoFallbackControl : SingleFormatControl
		{
			internal static readonly Control NO_FALLBACK = new NoFallbackControl(FORMAT_DEFAULT);

			internal static readonly Control PROPERTIES_ONLY_NO_FALLBACK = new NoFallbackControl(FORMAT_PROPERTIES);

			internal static readonly Control CLASS_ONLY_NO_FALLBACK = new NoFallbackControl(FORMAT_CLASS);

			protected internal NoFallbackControl(List<String> formats) : base(formats)
			{
			}

			public override Locale GetFallbackLocale(String baseName, Locale locale)
			{
				if (baseName == null || locale == null)
				{
					throw new NullPointerException();
				}
				return null;
			}
		}
	}

}