using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2013, 2015, Oracle and/or its affiliates. All rights reserved.
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
namespace java.lang
{

	using CompoundEnumeration = sun.misc.CompoundEnumeration;
	using Resource = sun.misc.Resource;
	using URLClassPath = sun.misc.URLClassPath;
	using VM = sun.misc.VM;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// A class loader is an object that is responsible for loading classes. The
	/// class <tt>ClassLoader</tt> is an abstract class.  Given the <a
	/// href="#name">binary name</a> of a class, a class loader should attempt to
	/// locate or generate data that constitutes a definition for the class.  A
	/// typical strategy is to transform the name into a file name and then read a
	/// "class file" of that name from a file system.
	/// 
	/// <para> Every <seealso cref="Class <tt>Class</tt>"/> object contains a {@link
	/// Class#getClassLoader() reference} to the <tt>ClassLoader</tt> that defined
	/// it.
	/// 
	/// </para>
	/// <para> <tt>Class</tt> objects for array classes are not created by class
	/// loaders, but are created automatically as required by the Java runtime.
	/// The class loader for an array class, as returned by {@link
	/// Class#getClassLoader()} is the same as the class loader for its element
	/// type; if the element type is a primitive type, then the array class has no
	/// class loader.
	/// 
	/// </para>
	/// <para> Applications implement subclasses of <tt>ClassLoader</tt> in order to
	/// extend the manner in which the Java virtual machine dynamically loads
	/// classes.
	/// 
	/// </para>
	/// <para> Class loaders may typically be used by security managers to indicate
	/// security domains.
	/// 
	/// </para>
	/// <para> The <tt>ClassLoader</tt> class uses a delegation model to search for
	/// classes and resources.  Each instance of <tt>ClassLoader</tt> has an
	/// associated parent class loader.  When requested to find a class or
	/// resource, a <tt>ClassLoader</tt> instance will delegate the search for the
	/// class or resource to its parent class loader before attempting to find the
	/// class or resource itself.  The virtual machine's built-in class loader,
	/// called the "bootstrap class loader", does not itself have a parent but may
	/// serve as the parent of a <tt>ClassLoader</tt> instance.
	/// 
	/// </para>
	/// <para> Class loaders that support concurrent loading of classes are known as
	/// <em>parallel capable</em> class loaders and are required to register
	/// themselves at their class initialization time by invoking the
	/// {@link
	/// #registerAsParallelCapable <tt>ClassLoader.registerAsParallelCapable</tt>}
	/// method. Note that the <tt>ClassLoader</tt> class is registered as parallel
	/// capable by default. However, its subclasses still need to register themselves
	/// if they are parallel capable. <br>
	/// In environments in which the delegation model is not strictly
	/// hierarchical, class loaders need to be parallel capable, otherwise class
	/// loading can lead to deadlocks because the loader lock is held for the
	/// duration of the class loading process (see {@link #loadClass
	/// <tt>loadClass</tt>} methods).
	/// 
	/// </para>
	/// <para> Normally, the Java virtual machine loads classes from the local file
	/// system in a platform-dependent manner.  For example, on UNIX systems, the
	/// virtual machine loads classes from the directory defined by the
	/// <tt>CLASSPATH</tt> environment variable.
	/// 
	/// </para>
	/// <para> However, some classes may not originate from a file; they may originate
	/// from other sources, such as the network, or they could be constructed by an
	/// application.  The method {@link #defineClass(String, byte[], int, int)
	/// <tt>defineClass</tt>} converts an array of bytes into an instance of class
	/// <tt>Class</tt>. Instances of this newly defined class can be created using
	/// <seealso cref="Class#newInstance <tt>Class.newInstance</tt>"/>.
	/// 
	/// </para>
	/// <para> The methods and constructors of objects created by a class loader may
	/// reference other classes.  To determine the class(es) referred to, the Java
	/// virtual machine invokes the <seealso cref="#loadClass <tt>loadClass</tt>"/> method of
	/// the class loader that originally created the class.
	/// 
	/// </para>
	/// <para> For example, an application could create a network class loader to
	/// download class files from a server.  Sample code might look like:
	/// 
	/// <blockquote><pre>
	///   ClassLoader loader&nbsp;= new NetworkClassLoader(host,&nbsp;port);
	///   Object main&nbsp;= loader.loadClass("Main", true).newInstance();
	///       &nbsp;.&nbsp;.&nbsp;.
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> The network class loader subclass must define the methods {@link
	/// #findClass <tt>findClass</tt>} and <tt>loadClassData</tt> to load a class
	/// from the network.  Once it has downloaded the bytes that make up the class,
	/// it should use the method <seealso cref="#defineClass <tt>defineClass</tt>"/> to
	/// create a class instance.  A sample implementation is:
	/// 
	/// <blockquote><pre>
	///     class NetworkClassLoader extends ClassLoader {
	///         String host;
	///         int port;
	/// 
	///         public Class findClass(String name) {
	///             byte[] b = loadClassData(name);
	///             return defineClass(name, b, 0, b.length);
	///         }
	/// 
	///         private byte[] loadClassData(String name) {
	///             // load the class data from the connection
	///             &nbsp;.&nbsp;.&nbsp;.
	///         }
	///     }
	/// </pre></blockquote>
	/// 
	/// <h3> <a name="name">Binary names</a> </h3>
	/// 
	/// </para>
	/// <para> Any class name provided as a <seealso cref="String"/> parameter to methods in
	/// <tt>ClassLoader</tt> must be a binary name as defined by
	/// <cite>The Java&trade; Language Specification</cite>.
	/// 
	/// </para>
	/// <para> Examples of valid class names include:
	/// <blockquote><pre>
	///   "java.lang.String"
	///   "javax.swing.JSpinner$DefaultEditor"
	///   "java.security.KeyStore$Builder$FileBuilder$1"
	///   "java.net.URLClassLoader$3$1"
	/// </pre></blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=      #resolveClass(Class)
	/// @since 1.0 </seealso>
	public abstract class ClassLoader
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			DefaultDomain = new ProtectionDomain(new CodeSource(null, (Certificate[]) null), null, this, null);
		}


//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void registerNatives();
		static ClassLoader()
		{
			registerNatives();
				lock (loaderTypes)
				{
					loaderTypes.add(typeof(ClassLoader));
				}
		}

		// The parent class loader for delegation
		// Note: VM hardcoded the offset of this field, thus all new fields
		// must be added *after* it.
		private readonly ClassLoader Parent_Renamed;

		/// <summary>
		/// Encapsulates the set of parallel capable loader types.
		/// </summary>
		private class ParallelLoaders
		{
			internal ParallelLoaders()
			{
			}

			// the set of parallel capable loader types
			internal static readonly Set<Class> LoaderTypes = Collections.NewSetFromMap(new WeakHashMap<Class, Boolean>());

			/// <summary>
			/// Registers the given class loader type as parallel capabale.
			/// Returns {@code true} is successfully registered; {@code false} if
			/// loader's super class is not registered.
			/// </summary>
			internal static bool Register(Class c)
			{
				lock (LoaderTypes)
				{
					if (LoaderTypes.Contains(c.BaseType))
					{
						// register the class loader as parallel capable
						// if and only if all of its super classes are.
						// Note: given current classloading sequence, if
						// the immediate super class is parallel capable,
						// all the super classes higher up must be too.
						LoaderTypes.Add(c);
						return true;
					}
					else
					{
						return false;
					}
				}
			}

			/// <summary>
			/// Returns {@code true} if the given class loader type is
			/// registered as parallel capable.
			/// </summary>
			internal static bool IsRegistered(Class c)
			{
				lock (LoaderTypes)
				{
					return LoaderTypes.Contains(c);
				}
			}
		}

		// Maps class name to the corresponding lock object when the current
		// class loader is parallel capable.
		// Note: VM also uses this field to decide if the current class loader
		// is parallel capable and the appropriate lock object for class loading.
		private readonly ConcurrentDictionary<String, Object> ParallelLockMap;

		// Hashtable that maps packages to certs
		private readonly IDictionary<String, Certificate[]> Package2certs;

		// Shared among all packages with unsigned classes
		private static readonly Certificate[] Nocerts = new Certificate[0];

		// The classes loaded by this class loader. The only purpose of this table
		// is to keep the classes from being GC'ed until the loader is GC'ed.
		private readonly List<Class> Classes = new List<Class>();

		// The "default" domain. Set as the default ProtectionDomain on newly
		// created classes.
		private ProtectionDomain DefaultDomain;

		// The initiating protection domains for all classes loaded by this loader
		private readonly Set<ProtectionDomain> Domains;

		// Invoked by the VM to record every loaded class with this loader.
		internal virtual void AddClass(Class c)
		{
			Classes.Add(c);
		}

		// The packages defined in this class loader.  Each package name is mapped
		// to its corresponding Package object.
		// @GuardedBy("itself")
		private readonly Dictionary<String, Package> Packages_Renamed = new Dictionary<String, Package>();

		private static Void CheckCreateClassLoader()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckCreateClassLoader();
			}
			return null;
		}

		private ClassLoader(Void unused, ClassLoader parent)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Parent_Renamed = parent;
			if (ParallelLoaders.IsRegistered(this.GetType()))
			{
				ParallelLockMap = new ConcurrentDictionary<>();
			Package2certs = new ConcurrentDictionary<>();
				Domains = Collections.SynchronizedSet(new HashSet<ProtectionDomain>());
				AssertionLock = new Object();
			}
			else
			{
				// no finer-grained lock; lock on the classloader instance
				ParallelLockMap = null;
			Package2certs = new Dictionary<>();
				Domains = new HashSet<>();
				AssertionLock = this;
			}
		}

		/// <summary>
		/// Creates a new class loader using the specified parent class loader for
		/// delegation.
		/// 
		/// <para> If there is a security manager, its {@link
		/// SecurityManager#checkCreateClassLoader()
		/// <tt>checkCreateClassLoader</tt>} method is invoked.  This may result in
		/// a security exception.  </para>
		/// </summary>
		/// <param name="parent">
		///         The parent class loader
		/// </param>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its
		///          <tt>checkCreateClassLoader</tt> method doesn't allow creation
		///          of a new class loader.
		/// 
		/// @since  1.2 </exception>
		protected internal ClassLoader(ClassLoader parent) : this(CheckCreateClassLoader(), parent)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a new class loader using the <tt>ClassLoader</tt> returned by
		/// the method {@link #getSystemClassLoader()
		/// <tt>getSystemClassLoader()</tt>} as the parent class loader.
		/// 
		/// <para> If there is a security manager, its {@link
		/// SecurityManager#checkCreateClassLoader()
		/// <tt>checkCreateClassLoader</tt>} method is invoked.  This may result in
		/// a security exception.  </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its
		///          <tt>checkCreateClassLoader</tt> method doesn't allow creation
		///          of a new class loader. </exception>
		protected internal ClassLoader() : this(CheckCreateClassLoader(), SystemClassLoader)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		// -- Class --

		/// <summary>
		/// Loads the class with the specified <a href="#name">binary name</a>.
		/// This method searches for classes in the same manner as the {@link
		/// #loadClass(String, boolean)} method.  It is invoked by the Java virtual
		/// machine to resolve class references.  Invoking this method is equivalent
		/// to invoking {@link #loadClass(String, boolean) <tt>loadClass(name,
		/// false)</tt>}.
		/// </summary>
		/// <param name="name">
		///         The <a href="#name">binary name</a> of the class
		/// </param>
		/// <returns>  The resulting <tt>Class</tt> object
		/// </returns>
		/// <exception cref="ClassNotFoundException">
		///          If the class was not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Class loadClass(String name) throws ClassNotFoundException
		public virtual Class LoadClass(String name)
		{
			return LoadClass(name, false);
		}

		/// <summary>
		/// Loads the class with the specified <a href="#name">binary name</a>.  The
		/// default implementation of this method searches for classes in the
		/// following order:
		/// 
		/// <ol>
		/// 
		///   <li><para> Invoke <seealso cref="#findLoadedClass(String)"/> to check if the class
		///   has already been loaded.  </para></li>
		/// 
		///   <li><para> Invoke the <seealso cref="#loadClass(String) <tt>loadClass</tt>"/> method
		///   on the parent class loader.  If the parent is <tt>null</tt> the class
		///   loader built-in to the virtual machine is used, instead.  </para></li>
		/// 
		///   <li><para> Invoke the <seealso cref="#findClass(String)"/> method to find the
		///   class.  </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> If the class was found using the above steps, and the
		/// <tt>resolve</tt> flag is true, this method will then invoke the {@link
		/// #resolveClass(Class)} method on the resulting <tt>Class</tt> object.
		/// 
		/// </para>
		/// <para> Subclasses of <tt>ClassLoader</tt> are encouraged to override {@link
		/// #findClass(String)}, rather than this method.  </para>
		/// 
		/// <para> Unless overridden, this method synchronizes on the result of
		/// <seealso cref="#getClassLoadingLock <tt>getClassLoadingLock</tt>"/> method
		/// during the entire class loading process.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">
		///         The <a href="#name">binary name</a> of the class
		/// </param>
		/// <param name="resolve">
		///         If <tt>true</tt> then resolve the class
		/// </param>
		/// <returns>  The resulting <tt>Class</tt> object
		/// </returns>
		/// <exception cref="ClassNotFoundException">
		///          If the class could not be found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class loadClass(String name, boolean resolve) throws ClassNotFoundException
		protected internal virtual Class LoadClass(String name, bool resolve)
		{
			lock (GetClassLoadingLock(name))
			{
				// First, check if the class has already been loaded
				Class c = FindLoadedClass(name);
				if (c == null)
				{
					long t0 = System.nanoTime();
					try
					{
						if (Parent_Renamed != null)
						{
							c = Parent_Renamed.LoadClass(name, false);
						}
						else
						{
							c = FindBootstrapClassOrNull(name);
						}
					}
					catch (ClassNotFoundException)
					{
						// ClassNotFoundException thrown if class not found
						// from the non-null parent class loader
					}

					if (c == null)
					{
						// If still not found, then invoke findClass in order
						// to find the class.
						long t1 = System.nanoTime();
						c = FindClass(name);

						// this is the defining class loader; record the stats
						sun.misc.PerfCounter.ParentDelegationTime.addTime(t1 - t0);
						sun.misc.PerfCounter.FindClassTime.addElapsedTimeFrom(t1);
						sun.misc.PerfCounter.FindClasses.increment();
					}
				}
				if (resolve)
				{
					ResolveClass(c);
				}
				return c;
			}
		}

		/// <summary>
		/// Returns the lock object for class loading operations.
		/// For backward compatibility, the default implementation of this method
		/// behaves as follows. If this ClassLoader object is registered as
		/// parallel capable, the method returns a dedicated object associated
		/// with the specified class name. Otherwise, the method returns this
		/// ClassLoader object.
		/// </summary>
		/// <param name="className">
		///         The name of the to-be-loaded class
		/// </param>
		/// <returns> the lock for class loading operations
		/// </returns>
		/// <exception cref="NullPointerException">
		///         If registered as parallel capable and <tt>className</tt> is null
		/// </exception>
		/// <seealso cref= #loadClass(String, boolean)
		/// 
		/// @since  1.7 </seealso>
		protected internal virtual Object GetClassLoadingLock(String className)
		{
			Object @lock = this;
			if (ParallelLockMap != null)
			{
				Object newLock = new Object();
				@lock = ParallelLockMap.GetOrAdd(className, newLock);
				if (@lock == null)
				{
					@lock = newLock;
				}
			}
			return @lock;
		}

		// This method is invoked by the virtual machine to load a class.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class loadClassInternal(String name) throws ClassNotFoundException
		private Class LoadClassInternal(String name)
		{
			// For backward compatibility, explicitly lock on 'this' when
			// the current class loader is not parallel capable.
			if (ParallelLockMap == null)
			{
				lock (this)
				{
					 return LoadClass(name);
				}
			}
			else
			{
				return LoadClass(name);
			}
		}

		// Invoked by the VM after loading class with this loader.
		private void CheckPackageAccess(Class cls, ProtectionDomain pd)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecurityManager sm = System.getSecurityManager();
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				if (ReflectUtil.isNonPublicProxyClass(cls))
				{
					foreach (Class intf in cls.Interfaces)
					{
						CheckPackageAccess(intf, pd);
					}
					return;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = cls.getName();
				String name = cls.Name;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = name.lastIndexOf('.');
				int i = name.LastIndexOf('.');
				if (i != -1)
				{
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, sm, name, i), new AccessControlContext(new ProtectionDomain[] {pd}));
				}
			}
			Domains.Add(pd);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly ClassLoader OuterInstance;

			private java.lang.SecurityManager Sm;
			private string Name;
			private int i;

			public PrivilegedActionAnonymousInnerClassHelper(ClassLoader outerInstance, java.lang.SecurityManager sm, string name, int i)
			{
				this.OuterInstance = outerInstance;
				this.Sm = sm;
				this.Name = name;
				this.i = i;
			}

			public virtual Void Run()
			{
				Sm.CheckPackageAccess(Name.Substring(0, i));
				return null;
			}
		}

		/// <summary>
		/// Finds the class with the specified <a href="#name">binary name</a>.
		/// This method should be overridden by class loader implementations that
		/// follow the delegation model for loading classes, and will be invoked by
		/// the <seealso cref="#loadClass <tt>loadClass</tt>"/> method after checking the
		/// parent class loader for the requested class.  The default implementation
		/// throws a <tt>ClassNotFoundException</tt>.
		/// </summary>
		/// <param name="name">
		///         The <a href="#name">binary name</a> of the class
		/// </param>
		/// <returns>  The resulting <tt>Class</tt> object
		/// </returns>
		/// <exception cref="ClassNotFoundException">
		///          If the class could not be found
		/// 
		/// @since  1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class findClass(String name) throws ClassNotFoundException
		protected internal virtual Class FindClass(String name)
		{
			throw new ClassNotFoundException(name);
		}

		/// <summary>
		/// Converts an array of bytes into an instance of class <tt>Class</tt>.
		/// Before the <tt>Class</tt> can be used it must be resolved.  This method
		/// is deprecated in favor of the version that takes a <a
		/// href="#name">binary name</a> as its first argument, and is more secure.
		/// </summary>
		/// <param name="b">
		///         The bytes that make up the class data.  The bytes in positions
		///         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		///         of a valid class file as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <param name="off">
		///         The start offset in <tt>b</tt> of the class data
		/// </param>
		/// <param name="len">
		///         The length of the class data
		/// </param>
		/// <returns>  The <tt>Class</tt> object that was created from the specified
		///          class data
		/// </returns>
		/// <exception cref="ClassFormatError">
		///          If the data did not contain a valid class
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		///          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		/// </exception>
		/// <exception cref="SecurityException">
		///          If an attempt is made to add this class to a package that
		///          contains classes that were signed by a different set of
		///          certificates than this class, or if an attempt is made
		///          to define a class in a package with a fully-qualified name
		///          that starts with "{@code java.}".
		/// </exception>
		/// <seealso cref=  #loadClass(String, boolean) </seealso>
		/// <seealso cref=  #resolveClass(Class)
		/// </seealso>
		/// @deprecated  Replaced by {@link #defineClass(String, byte[], int, int)
		/// defineClass(String, byte[], int, int)} 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated(" Replaced by {@link #defineClass(String, byte[], int, int)") protected final Class defineClass(byte[] b, int off, int len) throws ClassFormatError
		[Obsolete(" Replaced by {@link #defineClass(String, byte[], int, int)")]
		protected internal Class DefineClass(sbyte[] b, int off, int len)
		{
			return DefineClass(null, b, off, len, null);
		}

		/// <summary>
		/// Converts an array of bytes into an instance of class <tt>Class</tt>.
		/// Before the <tt>Class</tt> can be used it must be resolved.
		/// 
		/// <para> This method assigns a default {@link java.security.ProtectionDomain
		/// <tt>ProtectionDomain</tt>} to the newly defined class.  The
		/// <tt>ProtectionDomain</tt> is effectively granted the same set of
		/// permissions returned when {@link
		/// java.security.Policy#getPermissions(java.security.CodeSource)
		/// <tt>Policy.getPolicy().getPermissions(new CodeSource(null, null))</tt>}
		/// is invoked.  The default domain is created on the first invocation of
		/// <seealso cref="#defineClass(String, byte[], int, int) <tt>defineClass</tt>"/>,
		/// and re-used on subsequent invocations.
		/// 
		/// </para>
		/// <para> To assign a specific <tt>ProtectionDomain</tt> to the class, use
		/// the {@link #defineClass(String, byte[], int, int,
		/// java.security.ProtectionDomain) <tt>defineClass</tt>} method that takes a
		/// <tt>ProtectionDomain</tt> as one of its arguments.  </para>
		/// </summary>
		/// <param name="name">
		///         The expected <a href="#name">binary name</a> of the class, or
		///         <tt>null</tt> if not known
		/// </param>
		/// <param name="b">
		///         The bytes that make up the class data.  The bytes in positions
		///         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		///         of a valid class file as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <param name="off">
		///         The start offset in <tt>b</tt> of the class data
		/// </param>
		/// <param name="len">
		///         The length of the class data
		/// </param>
		/// <returns>  The <tt>Class</tt> object that was created from the specified
		///          class data.
		/// </returns>
		/// <exception cref="ClassFormatError">
		///          If the data did not contain a valid class
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		///          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		/// </exception>
		/// <exception cref="SecurityException">
		///          If an attempt is made to add this class to a package that
		///          contains classes that were signed by a different set of
		///          certificates than this class (which is unsigned), or if
		///          <tt>name</tt> begins with "<tt>java.</tt>".
		/// </exception>
		/// <seealso cref=  #loadClass(String, boolean) </seealso>
		/// <seealso cref=  #resolveClass(Class) </seealso>
		/// <seealso cref=  java.security.CodeSource </seealso>
		/// <seealso cref=  java.security.SecureClassLoader
		/// 
		/// @since  1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final Class defineClass(String name, byte[] b, int off, int len) throws ClassFormatError
		protected internal Class DefineClass(String name, sbyte[] b, int off, int len)
		{
			return DefineClass(name, b, off, len, null);
		}

		/* Determine protection domain, and check that:
		    - not define java.* class,
		    - signer of this class matches signers for the rest of the classes in
		      package.
		*/
		private ProtectionDomain PreDefineClass(String name, ProtectionDomain pd)
		{
			if (!CheckName(name))
			{
				throw new NoClassDefFoundError("IllegalName: " + name);
			}

			if ((name != null) && name.StartsWith("java."))
			{
				throw new SecurityException("Prohibited package name: " + name.Substring(0, name.LastIndexOf('.')));
			}
			if (pd == null)
			{
				pd = DefaultDomain;
			}

			if (name != null)
			{
				CheckCerts(name, pd.CodeSource);
			}

			return pd;
		}

		private String DefineClassSourceLocation(ProtectionDomain pd)
		{
			CodeSource cs = pd.CodeSource;
			String source = null;
			if (cs != null && cs.Location != null)
			{
				source = cs.Location.ToString();
			}
			return source;
		}

		private void PostDefineClass(Class c, ProtectionDomain pd)
		{
			if (pd.CodeSource != null)
			{
				Certificate[] certs = pd.CodeSource.Certificates;
				if (certs != null)
				{
					SetSigners(c, certs);
				}
			}
		}

		/// <summary>
		/// Converts an array of bytes into an instance of class <tt>Class</tt>,
		/// with an optional <tt>ProtectionDomain</tt>.  If the domain is
		/// <tt>null</tt>, then a default domain will be assigned to the class as
		/// specified in the documentation for {@link #defineClass(String, byte[],
		/// int, int)}.  Before the class can be used it must be resolved.
		/// 
		/// <para> The first class defined in a package determines the exact set of
		/// certificates that all subsequent classes defined in that package must
		/// contain.  The set of certificates for a class is obtained from the
		/// <seealso cref="java.security.CodeSource <tt>CodeSource</tt>"/> within the
		/// <tt>ProtectionDomain</tt> of the class.  Any classes added to that
		/// package must contain the same set of certificates or a
		/// <tt>SecurityException</tt> will be thrown.  Note that if
		/// <tt>name</tt> is <tt>null</tt>, this check is not performed.
		/// You should always pass in the <a href="#name">binary name</a> of the
		/// class you are defining as well as the bytes.  This ensures that the
		/// class you are defining is indeed the class you think it is.
		/// 
		/// </para>
		/// <para> The specified <tt>name</tt> cannot begin with "<tt>java.</tt>", since
		/// all classes in the "<tt>java.*</tt> packages can only be defined by the
		/// bootstrap class loader.  If <tt>name</tt> is not <tt>null</tt>, it
		/// must be equal to the <a href="#name">binary name</a> of the class
		/// specified by the byte array "<tt>b</tt>", otherwise a {@link
		/// NoClassDefFoundError <tt>NoClassDefFoundError</tt>} will be thrown. </para>
		/// </summary>
		/// <param name="name">
		///         The expected <a href="#name">binary name</a> of the class, or
		///         <tt>null</tt> if not known
		/// </param>
		/// <param name="b">
		///         The bytes that make up the class data. The bytes in positions
		///         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		///         of a valid class file as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <param name="off">
		///         The start offset in <tt>b</tt> of the class data
		/// </param>
		/// <param name="len">
		///         The length of the class data
		/// </param>
		/// <param name="protectionDomain">
		///         The ProtectionDomain of the class
		/// </param>
		/// <returns>  The <tt>Class</tt> object created from the data,
		///          and optional <tt>ProtectionDomain</tt>.
		/// </returns>
		/// <exception cref="ClassFormatError">
		///          If the data did not contain a valid class
		/// </exception>
		/// <exception cref="NoClassDefFoundError">
		///          If <tt>name</tt> is not equal to the <a href="#name">binary
		///          name</a> of the class specified by <tt>b</tt>
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		///          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		/// </exception>
		/// <exception cref="SecurityException">
		///          If an attempt is made to add this class to a package that
		///          contains classes that were signed by a different set of
		///          certificates than this class, or if <tt>name</tt> begins with
		///          "<tt>java.</tt>". </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final Class defineClass(String name, byte[] b, int off, int len, java.security.ProtectionDomain protectionDomain) throws ClassFormatError
		protected internal Class DefineClass(String name, sbyte[] b, int off, int len, ProtectionDomain protectionDomain)
		{
			protectionDomain = PreDefineClass(name, protectionDomain);
			String source = DefineClassSourceLocation(protectionDomain);
			Class c = defineClass1(name, b, off, len, protectionDomain, source);
			PostDefineClass(c, protectionDomain);
			return c;
		}

		/// <summary>
		/// Converts a <seealso cref="java.nio.ByteBuffer <tt>ByteBuffer</tt>"/>
		/// into an instance of class <tt>Class</tt>,
		/// with an optional <tt>ProtectionDomain</tt>.  If the domain is
		/// <tt>null</tt>, then a default domain will be assigned to the class as
		/// specified in the documentation for {@link #defineClass(String, byte[],
		/// int, int)}.  Before the class can be used it must be resolved.
		/// 
		/// <para>The rules about the first class defined in a package determining the
		/// set of certificates for the package, and the restrictions on class names
		/// are identical to those specified in the documentation for {@link
		/// #defineClass(String, byte[], int, int, ProtectionDomain)}.
		/// 
		/// </para>
		/// <para> An invocation of this method of the form
		/// <i>cl</i><tt>.defineClass(</tt><i>name</i><tt>,</tt>
		/// <i>bBuffer</i><tt>,</tt> <i>pd</i><tt>)</tt> yields exactly the same
		/// result as the statements
		/// 
		/// </para>
		/// <para> <tt>
		/// ...<br>
		/// byte[] temp = new byte[bBuffer.{@link
		/// java.nio.ByteBuffer#remaining remaining}()];<br>
		///     bBuffer.{@link java.nio.ByteBuffer#get(byte[])
		/// get}(temp);<br>
		///     return {@link #defineClass(String, byte[], int, int, ProtectionDomain)
		/// cl.defineClass}(name, temp, 0,
		/// temp.length, pd);<br>
		/// </tt></para>
		/// </summary>
		/// <param name="name">
		///         The expected <a href="#name">binary name</a>. of the class, or
		///         <tt>null</tt> if not known
		/// </param>
		/// <param name="b">
		///         The bytes that make up the class data. The bytes from positions
		///         <tt>b.position()</tt> through <tt>b.position() + b.limit() -1
		///         </tt> should have the format of a valid class file as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		/// </param>
		/// <param name="protectionDomain">
		///         The ProtectionDomain of the class, or <tt>null</tt>.
		/// </param>
		/// <returns>  The <tt>Class</tt> object created from the data,
		///          and optional <tt>ProtectionDomain</tt>.
		/// </returns>
		/// <exception cref="ClassFormatError">
		///          If the data did not contain a valid class.
		/// </exception>
		/// <exception cref="NoClassDefFoundError">
		///          If <tt>name</tt> is not equal to the <a href="#name">binary
		///          name</a> of the class specified by <tt>b</tt>
		/// </exception>
		/// <exception cref="SecurityException">
		///          If an attempt is made to add this class to a package that
		///          contains classes that were signed by a different set of
		///          certificates than this class, or if <tt>name</tt> begins with
		///          "<tt>java.</tt>".
		/// </exception>
		/// <seealso cref=      #defineClass(String, byte[], int, int, ProtectionDomain)
		/// 
		/// @since  1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final Class defineClass(String name, java.nio.ByteBuffer b, java.security.ProtectionDomain protectionDomain) throws ClassFormatError
		protected internal Class DefineClass(String name, java.nio.ByteBuffer b, ProtectionDomain protectionDomain)
		{
			int len = b.Remaining();

			// Use byte[] if not a direct ByteBufer:
			if (!b.Direct)
			{
				if (b.HasArray())
				{
					return DefineClass(name, b.Array(), b.Position() + b.ArrayOffset(), len, protectionDomain);
				}
				else
				{
					// no array, or read-only array
					sbyte[] tb = new sbyte[len];
					b.Get(tb); // get bytes out of byte buffer.
					return DefineClass(name, tb, 0, len, protectionDomain);
				}
			}

			protectionDomain = PreDefineClass(name, protectionDomain);
			String source = DefineClassSourceLocation(protectionDomain);
			Class c = defineClass2(name, b, b.Position(), len, protectionDomain, source);
			PostDefineClass(c, protectionDomain);
			return c;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class defineClass0(String name, sbyte[] b, int off, int len, java.security.ProtectionDomain pd);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class defineClass1(String name, sbyte[] b, int off, int len, java.security.ProtectionDomain pd, String source);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class defineClass2(String name, java.nio.ByteBuffer b, int off, int len, java.security.ProtectionDomain pd, String source);

		// true if the name is null or has the potential to be a valid binary name
		private bool CheckName(String name)
		{
			if ((name == null) || (name.Length() == 0))
			{
				return true;
			}
			if ((name.IndexOf('/') != -1) || (!VM.allowArraySyntax() && (name.CharAt(0) == '[')))
			{
				return false;
			}
			return true;
		}

		private void CheckCerts(String name, CodeSource cs)
		{
			int i = name.LastIndexOf('.');
			String pname = (i == -1) ? "" : name.Substring(0, i);

			Certificate[] certs = null;
			if (cs != null)
			{
				certs = cs.Certificates;
			}
			Certificate[] pcerts = null;
			if (ParallelLockMap == null)
			{
				lock (this)
				{
					pcerts = Package2certs[pname];
					if (pcerts == null)
					{
					Package2certs[pname] = (certs == null? Nocerts:certs);
					}
				}
			}
			else
			{
				pcerts = ((ConcurrentDictionary<String, Certificate[]>)Package2certs).GetOrAdd(pname, (certs == null? Nocerts:certs));
			}
			if (pcerts != null && !CompareCerts(pcerts, certs))
			{
				throw new SecurityException("class \"" + name + "\"'s signer information does not match signer information of other classes in the same package");
			}
		}

		/// <summary>
		/// check to make sure the certs for the new class (certs) are the same as
		/// the certs for the first class inserted in the package (pcerts)
		/// </summary>
		private bool CompareCerts(Certificate[] pcerts, Certificate[] certs)
		{
			// certs can be null, indicating no certs.
			if ((certs == null) || (certs.Length == 0))
			{
				return pcerts.Length == 0;
			}

			// the length must be the same at this point
			if (certs.Length != pcerts.Length)
			{
				return false;
			}

			// go through and make sure all the certs in one array
			// are in the other and vice-versa.
			bool match;
			for (int i = 0; i < certs.Length; i++)
			{
				match = false;
				for (int j = 0; j < pcerts.Length; j++)
				{
					if (certs[i].Equals(pcerts[j]))
					{
						match = true;
						break;
					}
				}
				if (!match)
				{
					return false;
				}
			}

			// now do the same for pcerts
			for (int i = 0; i < pcerts.Length; i++)
			{
				match = false;
				for (int j = 0; j < certs.Length; j++)
				{
					if (pcerts[i].Equals(certs[j]))
					{
						match = true;
						break;
					}
				}
				if (!match)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Links the specified class.  This (misleadingly named) method may be
		/// used by a class loader to link a class.  If the class <tt>c</tt> has
		/// already been linked, then this method simply returns. Otherwise, the
		/// class is linked as described in the "Execution" chapter of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="c">
		///         The class to link
		/// </param>
		/// <exception cref="NullPointerException">
		///          If <tt>c</tt> is <tt>null</tt>.
		/// </exception>
		/// <seealso cref=  #defineClass(String, byte[], int, int) </seealso>
		protected internal void ResolveClass(Class c)
		{
			resolveClass0(c);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void resolveClass0(Class c);

		/// <summary>
		/// Finds a class with the specified <a href="#name">binary name</a>,
		/// loading it if necessary.
		/// 
		/// <para> This method loads the class through the system class loader (see
		/// <seealso cref="#getSystemClassLoader()"/>).  The <tt>Class</tt> object returned
		/// might have more than one <tt>ClassLoader</tt> associated with it.
		/// Subclasses of <tt>ClassLoader</tt> need not usually invoke this method,
		/// because most class loaders need to override just {@link
		/// #findClass(String)}.  </para>
		/// </summary>
		/// <param name="name">
		///         The <a href="#name">binary name</a> of the class
		/// </param>
		/// <returns>  The <tt>Class</tt> object for the specified <tt>name</tt>
		/// </returns>
		/// <exception cref="ClassNotFoundException">
		///          If the class could not be found
		/// </exception>
		/// <seealso cref=  #ClassLoader(ClassLoader) </seealso>
		/// <seealso cref=  #getParent() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final Class findSystemClass(String name) throws ClassNotFoundException
		protected internal Class FindSystemClass(String name)
		{
			ClassLoader system = SystemClassLoader;
			if (system == null)
			{
				if (!CheckName(name))
				{
					throw new ClassNotFoundException(name);
				}
				Class cls = findBootstrapClass(name);
				if (cls == null)
				{
					throw new ClassNotFoundException(name);
				}
				return cls;
			}
			return system.LoadClass(name);
		}

		/// <summary>
		/// Returns a class loaded by the bootstrap class loader;
		/// or return null if not found.
		/// </summary>
		private Class FindBootstrapClassOrNull(String name)
		{
			if (!CheckName(name))
			{
				return null;
			}

			return findBootstrapClass(name);
		}

		// return null if not found
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class findBootstrapClass(String name);

		/// <summary>
		/// Returns the class with the given <a href="#name">binary name</a> if this
		/// loader has been recorded by the Java virtual machine as an initiating
		/// loader of a class with that <a href="#name">binary name</a>.  Otherwise
		/// <tt>null</tt> is returned.
		/// </summary>
		/// <param name="name">
		///         The <a href="#name">binary name</a> of the class
		/// </param>
		/// <returns>  The <tt>Class</tt> object, or <tt>null</tt> if the class has
		///          not been loaded
		/// 
		/// @since  1.1 </returns>
		protected internal Class FindLoadedClass(String name)
		{
			if (!CheckName(name))
			{
				return null;
			}
			return findLoadedClass0(name);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern final Class findLoadedClass0(String name);

		/// <summary>
		/// Sets the signers of a class.  This should be invoked after defining a
		/// class.
		/// </summary>
		/// <param name="c">
		///         The <tt>Class</tt> object
		/// </param>
		/// <param name="signers">
		///         The signers for the class
		/// 
		/// @since  1.1 </param>
		protected internal void SetSigners(Class c, Object[] signers)
		{
			c.Signers = signers;
		}


		// -- Resource --

		/// <summary>
		/// Finds the resource with the given name.  A resource is some data
		/// (images, audio, text, etc) that can be accessed by class code in a way
		/// that is independent of the location of the code.
		/// 
		/// <para> The name of a resource is a '<tt>/</tt>'-separated path name that
		/// identifies the resource.
		/// 
		/// </para>
		/// <para> This method will first search the parent class loader for the
		/// resource; if the parent is <tt>null</tt> the path of the class loader
		/// built-in to the virtual machine is searched.  That failing, this method
		/// will invoke <seealso cref="#findResource(String)"/> to find the resource.  </para>
		/// 
		/// @apiNote When overriding this method it is recommended that an
		/// implementation ensures that any delegation is consistent with the {@link
		/// #getResources(java.lang.String) getResources(String)} method.
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  A <tt>URL</tt> object for reading the resource, or
		///          <tt>null</tt> if the resource could not be found or the invoker
		///          doesn't have adequate  privileges to get the resource.
		/// 
		/// @since  1.1 </returns>
		public virtual URL GetResource(String name)
		{
			URL url;
			if (Parent_Renamed != null)
			{
				url = Parent_Renamed.GetResource(name);
			}
			else
			{
				url = GetBootstrapResource(name);
			}
			if (url == null)
			{
				url = FindResource(name);
			}
			return url;
		}

		/// <summary>
		/// Finds all the resources with the given name. A resource is some data
		/// (images, audio, text, etc) that can be accessed by class code in a way
		/// that is independent of the location of the code.
		/// 
		/// <para>The name of a resource is a <tt>/</tt>-separated path name that
		/// identifies the resource.
		/// 
		/// </para>
		/// <para> The search order is described in the documentation for {@link
		/// #getResource(String)}.  </para>
		/// 
		/// @apiNote When overriding this method it is recommended that an
		/// implementation ensures that any delegation is consistent with the {@link
		/// #getResource(java.lang.String) getResource(String)} method. This should
		/// ensure that the first element returned by the Enumeration's
		/// {@code nextElement} method is the same resource that the
		/// {@code getResource(String)} method would return.
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects for
		///          the resource.  If no resources could  be found, the enumeration
		///          will be empty.  Resources that the class loader doesn't have
		///          access to will not be in the enumeration.
		/// </returns>
		/// <exception cref="IOException">
		///          If I/O errors occur
		/// </exception>
		/// <seealso cref=  #findResources(String)
		/// 
		/// @since  1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Iterator<java.net.URL> getResources(String name) throws java.io.IOException
		public virtual IEnumerator<URL> GetResources(String name)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Iterator<java.net.URL>[] tmp = (java.util.Iterator<java.net.URL>[]) new java.util.Iterator<?>[2];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IEnumerator<URL>[] tmp = (IEnumerator<URL>[]) new IEnumerator<?>[2];
			if (Parent_Renamed != null)
			{
				tmp[0] = Parent_Renamed.GetResources(name);
			}
			else
			{
				tmp[0] = GetBootstrapResources(name);
			}
			tmp[1] = FindResources(name);

			return new CompoundEnumeration<>(tmp);
		}

		/// <summary>
		/// Finds the resource with the given name. Class loader implementations
		/// should override this method to specify where to find resources.
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  A <tt>URL</tt> object for reading the resource, or
		///          <tt>null</tt> if the resource could not be found
		/// 
		/// @since  1.2 </returns>
		protected internal virtual URL FindResource(String name)
		{
			return null;
		}

		/// <summary>
		/// Returns an enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects
		/// representing all the resources with the given name. Class loader
		/// implementations should override this method to specify where to load
		/// resources from.
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects for
		///          the resources
		/// </returns>
		/// <exception cref="IOException">
		///          If I/O errors occur
		/// 
		/// @since  1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.Iterator<java.net.URL> findResources(String name) throws java.io.IOException
		protected internal virtual IEnumerator<URL> FindResources(String name)
		{
			return Collections.EmptyEnumeration();
		}

		/// <summary>
		/// Registers the caller as parallel capable.
		/// The registration succeeds if and only if all of the following
		/// conditions are met:
		/// <ol>
		/// <li> no instance of the caller has been created</li>
		/// <li> all of the super classes (except class Object) of the caller are
		/// registered as parallel capable</li>
		/// </ol>
		/// <para>Note that once a class loader is registered as parallel capable, there
		/// is no way to change it back.</para>
		/// </summary>
		/// <returns>  true if the caller is successfully registered as
		///          parallel capable and false if otherwise.
		/// 
		/// @since   1.7 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive protected static boolean registerAsParallelCapable()
		protected internal static bool RegisterAsParallelCapable()
		{
			Class callerClass = Reflection.CallerClass.asSubclass(typeof(ClassLoader));
			return ParallelLoaders.Register(callerClass);
		}

		/// <summary>
		/// Find a resource of the specified name from the search path used to load
		/// classes.  This method locates the resource through the system class
		/// loader (see <seealso cref="#getSystemClassLoader()"/>).
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  A <seealso cref="java.net.URL <tt>URL</tt>"/> object for reading the
		///          resource, or <tt>null</tt> if the resource could not be found
		/// 
		/// @since  1.1 </returns>
		public static URL GetSystemResource(String name)
		{
			ClassLoader system = SystemClassLoader;
			if (system == null)
			{
				return GetBootstrapResource(name);
			}
			return system.GetResource(name);
		}

		/// <summary>
		/// Finds all resources of the specified name from the search path used to
		/// load classes.  The resources thus found are returned as an
		/// <seealso cref="java.util.Enumeration <tt>Enumeration</tt>"/> of {@link
		/// java.net.URL <tt>URL</tt>} objects.
		/// 
		/// <para> The search order is described in the documentation for {@link
		/// #getSystemResource(String)}.  </para>
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An enumeration of resource <seealso cref="java.net.URL <tt>URL</tt>"/>
		///          objects
		/// </returns>
		/// <exception cref="IOException">
		///          If I/O errors occur
		/// 
		/// @since  1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.Iterator<java.net.URL> getSystemResources(String name) throws java.io.IOException
		public static IEnumerator<URL> GetSystemResources(String name)
		{
			ClassLoader system = SystemClassLoader;
			if (system == null)
			{
				return GetBootstrapResources(name);
			}
			return system.GetResources(name);
		}

		/// <summary>
		/// Find resources from the VM's built-in classloader.
		/// </summary>
		private static URL GetBootstrapResource(String name)
		{
			URLClassPath ucp = BootstrapClassPath;
			Resource res = ucp.getResource(name);
			return res != null ? res.URL : null;
		}

		/// <summary>
		/// Find resources from the VM's built-in classloader.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.util.Iterator<java.net.URL> getBootstrapResources(String name) throws java.io.IOException
		private static IEnumerator<URL> GetBootstrapResources(String name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<sun.misc.Resource> e = getBootstrapClassPath().getResources(name);
			IEnumerator<Resource> e = BootstrapClassPath.getResources(name);
			return new IteratorAnonymousInnerClassHelper(e);
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<URL>
		{
			private IEnumerator<Resource> e;

			public IteratorAnonymousInnerClassHelper(IEnumerator<Resource> e)
			{
				this.e = e;
			}

			public virtual URL NextElement()
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return e.nextElement().URL;
			}
			public virtual bool HasMoreElements()
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				return e.hasMoreElements();
			}
		}

		// Returns the URLClassPath that is used for finding system resources.
		internal static URLClassPath BootstrapClassPath
		{
			get
			{
				return sun.misc.Launcher.BootstrapClassPath;
			}
		}


		/// <summary>
		/// Returns an input stream for reading the specified resource.
		/// 
		/// <para> The search order is described in the documentation for {@link
		/// #getResource(String)}.  </para>
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An input stream for reading the resource, or <tt>null</tt>
		///          if the resource could not be found
		/// 
		/// @since  1.1 </returns>
		public virtual InputStream GetResourceAsStream(String name)
		{
			URL url = GetResource(name);
			try
			{
				return url != null ? url.OpenStream() : null;
			}
			catch (IOException)
			{
				return null;
			}
		}

		/// <summary>
		/// Open for reading, a resource of the specified name from the search path
		/// used to load classes.  This method locates the resource through the
		/// system class loader (see <seealso cref="#getSystemClassLoader()"/>).
		/// </summary>
		/// <param name="name">
		///         The resource name
		/// </param>
		/// <returns>  An input stream for reading the resource, or <tt>null</tt>
		///          if the resource could not be found
		/// 
		/// @since  1.1 </returns>
		public static InputStream GetSystemResourceAsStream(String name)
		{
			URL url = GetSystemResource(name);
			try
			{
				return url != null ? url.OpenStream() : null;
			}
			catch (IOException)
			{
				return null;
			}
		}


		// -- Hierarchy --

		/// <summary>
		/// Returns the parent class loader for delegation. Some implementations may
		/// use <tt>null</tt> to represent the bootstrap class loader. This method
		/// will return <tt>null</tt> in such implementations if this class loader's
		/// parent is the bootstrap class loader.
		/// 
		/// <para> If a security manager is present, and the invoker's class loader is
		/// not <tt>null</tt> and is not an ancestor of this class loader, then this
		/// method invokes the security manager's {@link
		/// SecurityManager#checkPermission(java.security.Permission)
		/// <tt>checkPermission</tt>} method with a {@link
		/// RuntimePermission#RuntimePermission(String)
		/// <tt>RuntimePermission("getClassLoader")</tt>} permission to verify
		/// access to the parent class loader is permitted.  If not, a
		/// <tt>SecurityException</tt> will be thrown.  </para>
		/// </summary>
		/// <returns>  The parent <tt>ClassLoader</tt>
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <tt>checkPermission</tt>
		///          method doesn't allow access to this class loader's parent class
		///          loader.
		/// 
		/// @since  1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public final ClassLoader getParent()
		public ClassLoader Parent
		{
			get
			{
				if (Parent_Renamed == null)
				{
					return null;
				}
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					// Check access to the parent class loader
					// If the caller's class loader is same as this class loader,
					// permission check is performed.
					CheckClassLoaderPermission(Parent_Renamed, Reflection.CallerClass);
				}
				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Returns the system class loader for delegation.  This is the default
		/// delegation parent for new <tt>ClassLoader</tt> instances, and is
		/// typically the class loader used to start the application.
		/// 
		/// <para> This method is first invoked early in the runtime's startup
		/// sequence, at which point it creates the system class loader and sets it
		/// as the context class loader of the invoking <tt>Thread</tt>.
		/// 
		/// </para>
		/// <para> The default system class loader is an implementation-dependent
		/// instance of this class.
		/// 
		/// </para>
		/// <para> If the system property "<tt>java.system.class.loader</tt>" is defined
		/// when this method is first invoked then the value of that property is
		/// taken to be the name of a class that will be returned as the system
		/// class loader.  The class is loaded using the default system class loader
		/// and must define a public constructor that takes a single parameter of
		/// type <tt>ClassLoader</tt> which is used as the delegation parent.  An
		/// instance is then created using this constructor with the default system
		/// class loader as the parameter.  The resulting class loader is defined
		/// to be the system class loader.
		/// 
		/// </para>
		/// <para> If a security manager is present, and the invoker's class loader is
		/// not <tt>null</tt> and the invoker's class loader is not the same as or
		/// an ancestor of the system class loader, then this method invokes the
		/// security manager's {@link
		/// SecurityManager#checkPermission(java.security.Permission)
		/// <tt>checkPermission</tt>} method with a {@link
		/// RuntimePermission#RuntimePermission(String)
		/// <tt>RuntimePermission("getClassLoader")</tt>} permission to verify
		/// access to the system class loader.  If not, a
		/// <tt>SecurityException</tt> will be thrown.  </para>
		/// </summary>
		/// <returns>  The system <tt>ClassLoader</tt> for delegation, or
		///          <tt>null</tt> if none
		/// </returns>
		/// <exception cref="SecurityException">
		///          If a security manager exists and its <tt>checkPermission</tt>
		///          method doesn't allow access to the system class loader.
		/// </exception>
		/// <exception cref="IllegalStateException">
		///          If invoked recursively during the construction of the class
		///          loader specified by the "<tt>java.system.class.loader</tt>"
		///          property.
		/// </exception>
		/// <exception cref="Error">
		///          If the system property "<tt>java.system.class.loader</tt>"
		///          is defined but the named class could not be loaded, the
		///          provider class does not define the required constructor, or an
		///          exception is thrown by that constructor when it is invoked. The
		///          underlying cause of the error can be retrieved via the
		///          <seealso cref="Throwable#getCause()"/> method.
		/// 
		/// @revised  1.4 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static ClassLoader getSystemClassLoader()
		public static ClassLoader SystemClassLoader
		{
			get
			{
				InitSystemClassLoader();
				if (Scl == null)
				{
					return null;
				}
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					CheckClassLoaderPermission(Scl, Reflection.CallerClass);
				}
				return Scl;
			}
		}

		private static void InitSystemClassLoader()
		{
			lock (typeof(ClassLoader))
			{
				if (!SclSet)
				{
					if (Scl != null)
					{
						throw new IllegalStateException("recursive invocation");
					}
					sun.misc.Launcher l = sun.misc.Launcher.Launcher;
					if (l != null)
					{
						Throwable oops = null;
						Scl = l.ClassLoader;
						try
						{
							Scl = AccessController.doPrivileged(new SystemClassLoaderAction(Scl));
						}
						catch (PrivilegedActionException pae)
						{
							oops = pae.InnerException;
							if (oops is InvocationTargetException)
							{
								oops = oops.Cause;
							}
						}
						if (oops != null)
						{
							if (oops is Error)
							{
								throw (Error) oops;
							}
							else
							{
								// wrap the exception
								throw new Error(oops);
							}
						}
					}
					SclSet = true;
				}
			}
		}

		// Returns true if the specified class loader can be found in this class
		// loader's delegation chain.
		internal virtual bool IsAncestor(ClassLoader cl)
		{
			ClassLoader acl = this;
			do
			{
				acl = acl.Parent_Renamed;
				if (cl == acl)
				{
					return true;
				}
			} while (acl != null);
			return false;
		}

		// Tests if class loader access requires "getClassLoader" permission
		// check.  A class loader 'from' can access class loader 'to' if
		// class loader 'from' is same as class loader 'to' or an ancestor
		// of 'to'.  The class loader in a system domain can access
		// any class loader.
		private static bool NeedsClassLoaderPermissionCheck(ClassLoader from, ClassLoader to)
		{
			if (from == to)
			{
				return false;
			}

			if (from == null)
			{
				return false;
			}

			return !to.IsAncestor(from);
		}

		// Returns the class's class loader, or null if none.
		internal static ClassLoader GetClassLoader(Class caller)
		{
			// This can be null if the VM is requesting it
			if (caller == null)
			{
				return null;
			}
			// Circumvent security check since this is package-private
			return caller.ClassLoader0;
		}

		/*
		 * Checks RuntimePermission("getClassLoader") permission
		 * if caller's class loader is not null and caller's class loader
		 * is not the same as or an ancestor of the given cl argument.
		 */
		internal static void CheckClassLoaderPermission(ClassLoader cl, Class caller)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				// caller can be null if the VM is requesting it
				ClassLoader ccl = GetClassLoader(caller);
				if (NeedsClassLoaderPermissionCheck(ccl, cl))
				{
					sm.CheckPermission(SecurityConstants.GET_CLASSLOADER_PERMISSION);
				}
			}
		}

		// The class loader for the system
		// @GuardedBy("ClassLoader.class")
		private static ClassLoader Scl;

		// Set to true once the system class loader has been set
		// @GuardedBy("ClassLoader.class")
		private static bool SclSet;


		// -- Package --

		/// <summary>
		/// Defines a package by name in this <tt>ClassLoader</tt>.  This allows
		/// class loaders to define the packages for their classes. Packages must
		/// be created before the class is defined, and package names must be
		/// unique within a class loader and cannot be redefined or changed once
		/// created.
		/// </summary>
		/// <param name="name">
		///         The package name
		/// </param>
		/// <param name="specTitle">
		///         The specification title
		/// </param>
		/// <param name="specVersion">
		///         The specification version
		/// </param>
		/// <param name="specVendor">
		///         The specification vendor
		/// </param>
		/// <param name="implTitle">
		///         The implementation title
		/// </param>
		/// <param name="implVersion">
		///         The implementation version
		/// </param>
		/// <param name="implVendor">
		///         The implementation vendor
		/// </param>
		/// <param name="sealBase">
		///         If not <tt>null</tt>, then this package is sealed with
		///         respect to the given code source {@link java.net.URL
		///         <tt>URL</tt>}  object.  Otherwise, the package is not sealed.
		/// </param>
		/// <returns>  The newly defined <tt>Package</tt> object
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If package name duplicates an existing package either in this
		///          class loader or one of its ancestors
		/// 
		/// @since  1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Package definePackage(String name, String specTitle, String specVersion, String specVendor, String implTitle, String implVersion, String implVendor, java.net.URL sealBase) throws IllegalArgumentException
		protected internal virtual Package DefinePackage(String name, String specTitle, String specVersion, String specVendor, String implTitle, String implVersion, String implVendor, URL sealBase)
		{
			lock (Packages_Renamed)
			{
				Package pkg = GetPackage(name);
				if (pkg != null)
				{
					throw new IllegalArgumentException(name);
				}
				pkg = new Package(name, specTitle, specVersion, specVendor, implTitle, implVersion, implVendor, sealBase, this);
			Packages_Renamed[name] = pkg;
				return pkg;
			}
		}

		/// <summary>
		/// Returns a <tt>Package</tt> that has been defined by this class loader
		/// or any of its ancestors.
		/// </summary>
		/// <param name="name">
		///         The package name
		/// </param>
		/// <returns>  The <tt>Package</tt> corresponding to the given name, or
		///          <tt>null</tt> if not found
		/// 
		/// @since  1.2 </returns>
		protected internal virtual Package GetPackage(String name)
		{
			Package pkg;
			lock (Packages_Renamed)
			{
				pkg = Packages_Renamed[name];
			}
			if (pkg == null)
			{
				if (Parent_Renamed != null)
				{
					pkg = Parent_Renamed.GetPackage(name);
				}
				else
				{
					pkg = Package.GetSystemPackage(name);
				}
				if (pkg != null)
				{
					lock (Packages_Renamed)
					{
						Package pkg2 = Packages_Renamed[name];
						if (pkg2 == null)
						{
						Packages_Renamed[name] = pkg;
						}
						else
						{
							pkg = pkg2;
						}
					}
				}
			}
			return pkg;
		}

		/// <summary>
		/// Returns all of the <tt>Packages</tt> defined by this class loader and
		/// its ancestors.
		/// </summary>
		/// <returns>  The array of <tt>Package</tt> objects defined by this
		///          <tt>ClassLoader</tt>
		/// 
		/// @since  1.2 </returns>
		protected internal virtual Package[] Packages
		{
			get
			{
				IDictionary<String, Package> map;
				lock (Packages_Renamed)
				{
					map = new Dictionary<>(Packages_Renamed);
				}
				Package[] pkgs;
				if (Parent_Renamed != null)
				{
					pkgs = Parent_Renamed.Packages;
				}
				else
				{
					pkgs = Package.SystemPackages;
				}
				if (pkgs != null)
				{
					for (int i = 0; i < pkgs.Length; i++)
					{
						String pkgName = pkgs[i].Name;
						if (map[pkgName] == null)
						{
							map[pkgName] = pkgs[i];
						}
					}
				}
				return map.Values.toArray(new Package[map.Count]);
			}
		}


		// -- Native library access --

		/// <summary>
		/// Returns the absolute path name of a native library.  The VM invokes this
		/// method to locate the native libraries that belong to classes loaded with
		/// this class loader. If this method returns <tt>null</tt>, the VM
		/// searches the library along the path specified as the
		/// "<tt>java.library.path</tt>" property.
		/// </summary>
		/// <param name="libname">
		///         The library name
		/// </param>
		/// <returns>  The absolute path of the native library
		/// </returns>
		/// <seealso cref=  System#loadLibrary(String) </seealso>
		/// <seealso cref=  System#mapLibraryName(String)
		/// 
		/// @since  1.2 </seealso>
		protected internal virtual String FindLibrary(String libname)
		{
			return null;
		}

		/// <summary>
		/// The inner class NativeLibrary denotes a loaded native library instance.
		/// Every classloader contains a vector of loaded native libraries in the
		/// private field <tt>nativeLibraries</tt>.  The native libraries loaded
		/// into the system are entered into the <tt>systemNativeLibraries</tt>
		/// vector.
		/// 
		/// <para> Every native library requires a particular version of JNI. This is
		/// denoted by the private <tt>jniVersion</tt> field.  This field is set by
		/// the VM when it loads the library, and used by the VM to pass the correct
		/// version of JNI to the native methods.  </para>
		/// </summary>
		/// <seealso cref=      ClassLoader
		/// @since    1.2 </seealso>
		internal class NativeLibrary
		{
			// opaque handle to native library, used in native code.
			internal long Handle;
			// the version of JNI environment the native library requires.
			internal int JniVersion;
			// the class from which the library is loaded, also indicates
			// the loader this native library belongs.
			internal readonly Class FromClass_Renamed;
			// the canonicalized name of the native library.
			// or static library name
			internal String Name;
			// Indicates if the native library is linked into the VM
			internal bool IsBuiltin;
			// Indicates if the native library is loaded
			internal bool Loaded;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			[DllImport("unknown")]
			internal extern void load(String name, bool isBuiltin);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			[DllImport("unknown")]
			internal extern long find(String name);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			[DllImport("unknown")]
			internal extern void unload(String name, bool isBuiltin);

			public NativeLibrary(Class fromClass, String name, bool isBuiltin)
			{
				this.Name = name;
				this.FromClass_Renamed = fromClass;
				this.IsBuiltin = isBuiltin;
			}

			~NativeLibrary()
			{
				lock (LoadedLibraryNames)
				{
					if (FromClass_Renamed.ClassLoader != null && Loaded)
					{
						/* remove the native library name */
						int size = LoadedLibraryNames.Count;
						for (int i = 0; i < size; i++)
						{
							if (Name.Equals(LoadedLibraryNames[i]))
							{
								LoadedLibraryNames.RemoveAt(i);
								break;
							}
						}
						/* unload the library. */
						ClassLoader.NativeLibraryContext.Push(this);
						try
						{
							unload(Name, IsBuiltin);
						}
						finally
						{
							ClassLoader.NativeLibraryContext.Pop();
						}
					}
				}
			}
			// Invoked in the VM to determine the context class in
			// JNI_Load/JNI_Unload
			internal static Class FromClass
			{
				get
				{
					return ClassLoader.NativeLibraryContext.Peek().fromClass;
				}
			}
		}

		// All native library names we've loaded.
		private static List<String> LoadedLibraryNames = new List<String>();

		// Native libraries belonging to system classes.
		private static List<NativeLibrary> SystemNativeLibraries = new List<NativeLibrary>();

		// Native libraries associated with the class loader.
		private List<NativeLibrary> NativeLibraries = new List<NativeLibrary>();

		// native libraries being loaded/unloaded.
		private static Stack<NativeLibrary> NativeLibraryContext = new Stack<NativeLibrary>();

		// The paths searched for libraries
		private static String[] Usr_paths;
		private static String[] Sys_paths;

		private static String[] InitializePath(String propname)
		{
			String ldpath = System.getProperty(propname, "");
			String ps = File.pathSeparator;
			int ldlen = ldpath.Length();
			int i, j, n;
			// Count the separators in the path
			i = ldpath.IndexOf(ps);
			n = 0;
			while (i >= 0)
			{
				n++;
				i = ldpath.IndexOf(ps, i + 1);
			}

			// allocate the array of paths - n :'s = n + 1 path elements
			String[] paths = new String[n + 1];

			// Fill the array with paths from the ldpath
			n = i = 0;
			j = ldpath.IndexOf(ps);
			while (j >= 0)
			{
				if (j - i > 0)
				{
					paths[n++] = ldpath.Substring(i, j - i);
				}
				else if (j - i == 0)
				{
					paths[n++] = ".";
				}
				i = j + 1;
				j = ldpath.IndexOf(ps, i);
			}
			paths[n] = ldpath.Substring(i, ldlen - i);
			return paths;
		}

		// Invoked in the java.lang.Runtime class to implement load and loadLibrary.
		internal static void LoadLibrary(Class fromClass, String name, bool isAbsolute)
		{
			ClassLoader loader = (fromClass == null) ? null : fromClass.ClassLoader;
			if (Sys_paths == null)
			{
				Usr_paths = InitializePath("java.library.path");
				Sys_paths = InitializePath("sun.boot.library.path");
			}
			if (isAbsolute)
			{
				if (LoadLibrary0(fromClass, new File(name)))
				{
					return;
				}
				throw new UnsatisfiedLinkError("Can't load library: " + name);
			}
			if (loader != null)
			{
				String libfilename = loader.FindLibrary(name);
				if (libfilename != null)
				{
					File libfile = new File(libfilename);
					if (!libfile.Absolute)
					{
						throw new UnsatisfiedLinkError("ClassLoader.findLibrary failed to return an absolute path: " + libfilename);
					}
					if (LoadLibrary0(fromClass, libfile))
					{
						return;
					}
					throw new UnsatisfiedLinkError("Can't load " + libfilename);
				}
			}
			for (int i = 0 ; i < Sys_paths.Length ; i++)
			{
				File libfile = new File(Sys_paths[i], System.mapLibraryName(name));
				if (LoadLibrary0(fromClass, libfile))
				{
					return;
				}
				libfile = ClassLoaderHelper.MapAlternativeName(libfile);
				if (libfile != null && LoadLibrary0(fromClass, libfile))
				{
					return;
				}
			}
			if (loader != null)
			{
				for (int i = 0 ; i < Usr_paths.Length ; i++)
				{
					File libfile = new File(Usr_paths[i], System.mapLibraryName(name));
					if (LoadLibrary0(fromClass, libfile))
					{
						return;
					}
					libfile = ClassLoaderHelper.MapAlternativeName(libfile);
					if (libfile != null && LoadLibrary0(fromClass, libfile))
					{
						return;
					}
				}
			}
			// Oops, it failed
			throw new UnsatisfiedLinkError("no " + name + " in java.library.path");
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String findBuiltinLib(String name);

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean loadLibrary0(Class fromClass, final java.io.File file)
		private static bool LoadLibrary0(Class fromClass, File file)
		{
			// Check to see if we're attempting to access a static library
			String name = findBuiltinLib(file.Name);
			bool isBuiltin = (name != null);
			if (!isBuiltin)
			{
				bool exists = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(file))
					!= null;
				if (!exists)
				{
					return false;
				}
				try
				{
					name = file.CanonicalPath;
				}
				catch (IOException)
				{
					return false;
				}
			}
			ClassLoader loader = (fromClass == null) ? null : fromClass.ClassLoader;
			List<NativeLibrary> libs = loader != null ? loader.NativeLibraries : SystemNativeLibraries;
			lock (libs)
			{
				int size = libs.Count;
				for (int i = 0; i < size; i++)
				{
					NativeLibrary lib = libs[i];
					if (name.Equals(lib.Name))
					{
						return true;
					}
				}

				lock (LoadedLibraryNames)
				{
					if (LoadedLibraryNames.Contains(name))
					{
						throw new UnsatisfiedLinkError("Native Library " + name + " already loaded in another classloader");
					}
					/* If the library is being loaded (must be by the same thread,
					 * because Runtime.load and Runtime.loadLibrary are
					 * synchronous). The reason is can occur is that the JNI_OnLoad
					 * function can cause another loadLibrary invocation.
					 *
					 * Thus we can use a static stack to hold the list of libraries
					 * we are loading.
					 *
					 * If there is a pending load operation for the library, we
					 * immediately return success; otherwise, we raise
					 * UnsatisfiedLinkError.
					 */
					int n = NativeLibraryContext.Count;
					for (int i = 0; i < n; i++)
					{
						NativeLibrary lib = NativeLibraryContext.elementAt(i);
						if (name.Equals(lib.Name))
						{
							if (loader == lib.FromClass_Renamed.ClassLoader)
							{
								return true;
							}
							else
							{
								throw new UnsatisfiedLinkError("Native Library " + name + " is being loaded in another classloader");
							}
						}
					}
					NativeLibrary lib = new NativeLibrary(fromClass, name, isBuiltin);
					NativeLibraryContext.Push(lib);
					try
					{
						lib.load(name, isBuiltin);
					}
					finally
					{
						NativeLibraryContext.Pop();
					}
					if (lib.Loaded)
					{
						LoadedLibraryNames.Add(name);
						libs.Add(lib);
						return true;
					}
					return false;
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private File File;

			public PrivilegedActionAnonymousInnerClassHelper(File file)
			{
				this.File = file;
			}

			public virtual Object Run()
			{
				return File.Exists() ? true : null;
			}
		}

		// Invoked in the VM class linking code.
		internal static long FindNative(ClassLoader loader, String name)
		{
			List<NativeLibrary> libs = loader != null ? loader.NativeLibraries : SystemNativeLibraries;
			lock (libs)
			{
				int size = libs.Count;
				for (int i = 0; i < size; i++)
				{
					NativeLibrary lib = libs[i];
					long entry = lib.find(name);
					if (entry != 0)
					{
						return entry;
					}
				}
			}
			return 0;
		}


		// -- Assertion management --

		internal readonly Object AssertionLock;

		// The default toggle for assertion checking.
		// @GuardedBy("assertionLock")
		private bool DefaultAssertionStatus_Renamed = false;

		// Maps String packageName to Boolean package default assertion status Note
		// that the default package is placed under a null map key.  If this field
		// is null then we are delegating assertion status queries to the VM, i.e.,
		// none of this ClassLoader's assertion status modification methods have
		// been invoked.
		// @GuardedBy("assertionLock")
		private IDictionary<String, Boolean> PackageAssertionStatus = null;

		// Maps String fullyQualifiedClassName to Boolean assertionStatus If this
		// field is null then we are delegating assertion status queries to the VM,
		// i.e., none of this ClassLoader's assertion status modification methods
		// have been invoked.
		// @GuardedBy("assertionLock")
		internal IDictionary<String, Boolean> ClassAssertionStatus = null;

		/// <summary>
		/// Sets the default assertion status for this class loader.  This setting
		/// determines whether classes loaded by this class loader and initialized
		/// in the future will have assertions enabled or disabled by default.
		/// This setting may be overridden on a per-package or per-class basis by
		/// invoking <seealso cref="#setPackageAssertionStatus(String, boolean)"/> or {@link
		/// #setClassAssertionStatus(String, boolean)}.
		/// </summary>
		/// <param name="enabled">
		///         <tt>true</tt> if classes loaded by this class loader will
		///         henceforth have assertions enabled by default, <tt>false</tt>
		///         if they will have assertions disabled by default.
		/// 
		/// @since  1.4 </param>
		public virtual bool DefaultAssertionStatus
		{
			set
			{
				lock (AssertionLock)
				{
					if (ClassAssertionStatus == null)
					{
						InitializeJavaAssertionMaps();
					}
    
					DefaultAssertionStatus_Renamed = value;
				}
			}
		}

		/// <summary>
		/// Sets the package default assertion status for the named package.  The
		/// package default assertion status determines the assertion status for
		/// classes initialized in the future that belong to the named package or
		/// any of its "subpackages".
		/// 
		/// <para> A subpackage of a package named p is any package whose name begins
		/// with "<tt>p.</tt>".  For example, <tt>javax.swing.text</tt> is a
		/// subpackage of <tt>javax.swing</tt>, and both <tt>java.util</tt> and
		/// <tt>java.lang.reflect</tt> are subpackages of <tt>java</tt>.
		/// 
		/// </para>
		/// <para> In the event that multiple package defaults apply to a given class,
		/// the package default pertaining to the most specific package takes
		/// precedence over the others.  For example, if <tt>javax.lang</tt> and
		/// <tt>javax.lang.reflect</tt> both have package defaults associated with
		/// them, the latter package default applies to classes in
		/// <tt>javax.lang.reflect</tt>.
		/// 
		/// </para>
		/// <para> Package defaults take precedence over the class loader's default
		/// assertion status, and may be overridden on a per-class basis by invoking
		/// <seealso cref="#setClassAssertionStatus(String, boolean)"/>.  </para>
		/// </summary>
		/// <param name="packageName">
		///         The name of the package whose package default assertion status
		///         is to be set. A <tt>null</tt> value indicates the unnamed
		///         package that is "current"
		///         (see section 7.4.2 of
		///         <cite>The Java&trade; Language Specification</cite>.)
		/// </param>
		/// <param name="enabled">
		///         <tt>true</tt> if classes loaded by this classloader and
		///         belonging to the named package or any of its subpackages will
		///         have assertions enabled by default, <tt>false</tt> if they will
		///         have assertions disabled by default.
		/// 
		/// @since  1.4 </param>
		public virtual void SetPackageAssertionStatus(String packageName, bool enabled)
		{
			lock (AssertionLock)
			{
				if (PackageAssertionStatus == null)
				{
					InitializeJavaAssertionMaps();
				}

			PackageAssertionStatus[packageName] = enabled;
			}
		}

		/// <summary>
		/// Sets the desired assertion status for the named top-level class in this
		/// class loader and any nested classes contained therein.  This setting
		/// takes precedence over the class loader's default assertion status, and
		/// over any applicable per-package default.  This method has no effect if
		/// the named class has already been initialized.  (Once a class is
		/// initialized, its assertion status cannot change.)
		/// 
		/// <para> If the named class is not a top-level class, this invocation will
		/// have no effect on the actual assertion status of any class. </para>
		/// </summary>
		/// <param name="className">
		///         The fully qualified class name of the top-level class whose
		///         assertion status is to be set.
		/// </param>
		/// <param name="enabled">
		///         <tt>true</tt> if the named class is to have assertions
		///         enabled when (and if) it is initialized, <tt>false</tt> if the
		///         class is to have assertions disabled.
		/// 
		/// @since  1.4 </param>
		public virtual void SetClassAssertionStatus(String className, bool enabled)
		{
			lock (AssertionLock)
			{
				if (ClassAssertionStatus == null)
				{
					InitializeJavaAssertionMaps();
				}

				ClassAssertionStatus[className] = enabled;
			}
		}

		/// <summary>
		/// Sets the default assertion status for this class loader to
		/// <tt>false</tt> and discards any package defaults or class assertion
		/// status settings associated with the class loader.  This method is
		/// provided so that class loaders can be made to ignore any command line or
		/// persistent assertion status settings and "start with a clean slate."
		/// 
		/// @since  1.4
		/// </summary>
		public virtual void ClearAssertionStatus()
		{
			/*
			 * Whether or not "Java assertion maps" are initialized, set
			 * them to empty maps, effectively ignoring any present settings.
			 */
			lock (AssertionLock)
			{
				ClassAssertionStatus = new Dictionary<>();
			PackageAssertionStatus = new Dictionary<>();
				DefaultAssertionStatus_Renamed = false;
			}
		}

		/// <summary>
		/// Returns the assertion status that would be assigned to the specified
		/// class if it were to be initialized at the time this method is invoked.
		/// If the named class has had its assertion status set, the most recent
		/// setting will be returned; otherwise, if any package default assertion
		/// status pertains to this class, the most recent setting for the most
		/// specific pertinent package default assertion status is returned;
		/// otherwise, this class loader's default assertion status is returned.
		/// </p>
		/// </summary>
		/// <param name="className">
		///         The fully qualified class name of the class whose desired
		///         assertion status is being queried.
		/// </param>
		/// <returns>  The desired assertion status of the specified class.
		/// </returns>
		/// <seealso cref=  #setClassAssertionStatus(String, boolean) </seealso>
		/// <seealso cref=  #setPackageAssertionStatus(String, boolean) </seealso>
		/// <seealso cref=  #setDefaultAssertionStatus(boolean)
		/// 
		/// @since  1.4 </seealso>
		internal virtual bool DesiredAssertionStatus(String className)
		{
			lock (AssertionLock)
			{
				// assert classAssertionStatus   != null;
				// assert packageAssertionStatus != null;

				// Check for a class entry
				Boolean result = ClassAssertionStatus[className];
				if (result != null)
				{
					return result.BooleanValue();
				}

				// Check for most specific package entry
				int dotIndex = className.LastIndexOf(".");
				if (dotIndex < 0) // default package
				{
					result = PackageAssertionStatus[null];
					if (result != null)
					{
						return result.BooleanValue();
					}
				}
				while (dotIndex > 0)
				{
					className = className.Substring(0, dotIndex);
					result = PackageAssertionStatus[className];
					if (result != null)
					{
						return result.BooleanValue();
					}
					dotIndex = className.LastIndexOf(".", dotIndex - 1);
				}

				// Return the classloader default
				return DefaultAssertionStatus_Renamed;
			}
		}

		// Set up the assertions with information provided by the VM.
		// Note: Should only be called inside a synchronized block
		private void InitializeJavaAssertionMaps()
		{
			// assert Thread.holdsLock(assertionLock);

			ClassAssertionStatus = new Dictionary<>();
		PackageAssertionStatus = new Dictionary<>();
			AssertionStatusDirectives directives = retrieveDirectives();

			for (int i = 0; i < directives.Classes.Length; i++)
			{
				ClassAssertionStatus[directives.Classes[i]] = directives.ClassEnabled[i];
			}

			for (int i = 0; i < directives.Packages.Length; i++)
			{
			PackageAssertionStatus[directives.Packages[i]] = directives.PackageEnabled[i];
			}

			DefaultAssertionStatus_Renamed = directives.Deflt;
		}

		// Retrieves the assertion directives from the VM.
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern AssertionStatusDirectives retrieveDirectives();
	}


	internal class SystemClassLoaderAction : PrivilegedExceptionAction<ClassLoader>
	{
		private ClassLoader Parent;

		internal SystemClassLoaderAction(ClassLoader parent)
		{
			this.Parent = parent;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ClassLoader run() throws Exception
		public virtual ClassLoader Run()
		{
			String cls = System.getProperty("java.system.class.loader");
			if (cls == null)
			{
				return Parent;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> ctor = Class.forName(cls, true, parent).getDeclaredConstructor(new Class[] { ClassLoader.class });
			Constructor<?> ctor = Class.ForName(cls, true, Parent).GetDeclaredConstructor(new Class[] {typeof(ClassLoader)});
			ClassLoader sys = (ClassLoader) ctor.newInstance(new Object[] {Parent});
			Thread.CurrentThread.ContextClassLoader = sys;
			return sys;
		}
	}

}