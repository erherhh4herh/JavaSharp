using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

namespace java.lang
{




	using ParseUtil = sun.net.www.ParseUtil;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;

	/// <summary>
	/// {@code Package} objects contain version information
	/// about the implementation and specification of a Java package.
	/// This versioning information is retrieved and made available
	/// by the <seealso cref="ClassLoader"/> instance that
	/// loaded the class(es).  Typically, it is stored in the manifest that is
	/// distributed with the classes.
	/// 
	/// <para>The set of classes that make up the package may implement a
	/// particular specification and if so the specification title, version number,
	/// and vendor strings identify that specification.
	/// An application can ask if the package is
	/// compatible with a particular version, see the {@link
	/// #isCompatibleWith isCompatibleWith}
	/// method for details.
	/// 
	/// </para>
	/// <para>Specification version numbers use a syntax that consists of nonnegative
	/// decimal integers separated by periods ".", for example "2.0" or
	/// "1.2.3.4.5.6.7".  This allows an extensible number to be used to represent
	/// major, minor, micro, etc. versions.  The version specification is described
	/// by the following formal grammar:
	/// <blockquote>
	/// <dl>
	/// <dt><i>SpecificationVersion:</i>
	/// <dd><i>Digits RefinedVersion<sub>opt</sub></i>
	/// 
	/// <dt><i>RefinedVersion:</i>
	/// <dd>{@code .} <i>Digits</i>
	/// <dd>{@code .} <i>Digits RefinedVersion</i>
	/// 
	/// <dt><i>Digits:</i>
	/// <dd><i>Digit</i>
	/// <dd><i>Digits</i>
	/// 
	/// <dt><i>Digit:</i>
	/// <dd>any character for which <seealso cref="Character#isDigit"/> returns {@code true},
	/// e.g. 0, 1, 2, ...
	/// </dl>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>The implementation title, version, and vendor strings identify an
	/// implementation and are made available conveniently to enable accurate
	/// reporting of the packages involved when a problem occurs. The contents
	/// all three implementation strings are vendor specific. The
	/// implementation version strings have no specified syntax and should
	/// only be compared for equality with desired version identifiers.
	/// 
	/// </para>
	/// <para>Within each {@code ClassLoader} instance all classes from the same
	/// java package have the same Package object.  The static methods allow a package
	/// to be found by name or the set of all packages known to the current class
	/// loader to be found.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ClassLoader#definePackage </seealso>
	public class Package : java.lang.reflect.AnnotatedElement
	{
		/// <summary>
		/// Return the name of this package.
		/// </summary>
		/// <returns>  The fully-qualified name of this package as defined in section 6.5.3 of
		///          <cite>The Java&trade; Language Specification</cite>,
		///          for example, {@code java.lang} </returns>
		public virtual String Name
		{
			get
			{
				return PkgName;
			}
		}


		/// <summary>
		/// Return the title of the specification that this package implements. </summary>
		/// <returns> the specification title, null is returned if it is not known. </returns>
		public virtual String SpecificationTitle
		{
			get
			{
				return SpecTitle;
			}
		}

		/// <summary>
		/// Returns the version number of the specification
		/// that this package implements.
		/// This version string must be a sequence of nonnegative decimal
		/// integers separated by "."'s and may have leading zeros.
		/// When version strings are compared the most significant
		/// numbers are compared. </summary>
		/// <returns> the specification version, null is returned if it is not known. </returns>
		public virtual String SpecificationVersion
		{
			get
			{
				return SpecVersion;
			}
		}

		/// <summary>
		/// Return the name of the organization, vendor,
		/// or company that owns and maintains the specification
		/// of the classes that implement this package. </summary>
		/// <returns> the specification vendor, null is returned if it is not known. </returns>
		public virtual String SpecificationVendor
		{
			get
			{
				return SpecVendor;
			}
		}

		/// <summary>
		/// Return the title of this package. </summary>
		/// <returns> the title of the implementation, null is returned if it is not known. </returns>
		public virtual String ImplementationTitle
		{
			get
			{
				return ImplTitle;
			}
		}

		/// <summary>
		/// Return the version of this implementation. It consists of any string
		/// assigned by the vendor of this implementation and does
		/// not have any particular syntax specified or expected by the Java
		/// runtime. It may be compared for equality with other
		/// package version strings used for this implementation
		/// by this vendor for this package. </summary>
		/// <returns> the version of the implementation, null is returned if it is not known. </returns>
		public virtual String ImplementationVersion
		{
			get
			{
				return ImplVersion;
			}
		}

		/// <summary>
		/// Returns the name of the organization,
		/// vendor or company that provided this implementation. </summary>
		/// <returns> the vendor that implemented this package.. </returns>
		public virtual String ImplementationVendor
		{
			get
			{
				return ImplVendor;
			}
		}

		/// <summary>
		/// Returns true if this package is sealed.
		/// </summary>
		/// <returns> true if the package is sealed, false otherwise </returns>
		public virtual bool Sealed
		{
			get
			{
				return SealBase != reflect.AnnotatedElement_Fields.Null;
			}
		}

		/// <summary>
		/// Returns true if this package is sealed with respect to the specified
		/// code source url.
		/// </summary>
		/// <param name="url"> the code source url </param>
		/// <returns> true if this package is sealed with respect to url </returns>
		public virtual bool IsSealed(URL url)
		{
			return url.Equals(SealBase);
		}

		/// <summary>
		/// Compare this package's specification version with a
		/// desired version. It returns true if
		/// this packages specification version number is greater than or equal
		/// to the desired version number. <para>
		/// 
		/// Version numbers are compared by sequentially comparing corresponding
		/// components of the desired and specification strings.
		/// Each component is converted as a decimal integer and the values
		/// compared.
		/// If the specification value is greater than the desired
		/// value true is returned. If the value is less false is returned.
		/// If the values are equal the period is skipped and the next pair of
		/// components is compared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="desired"> the version string of the desired version. </param>
		/// <returns> true if this package's version number is greater
		///          than or equal to the desired version number
		/// </returns>
		/// <exception cref="NumberFormatException"> if the desired or current version
		///          is not of the correct dotted form. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isCompatibleWith(String desired) throws NumberFormatException
		public virtual bool IsCompatibleWith(String desired)
		{
			if (SpecVersion == reflect.AnnotatedElement_Fields.Null || SpecVersion.Length() < 1)
			{
				throw new NumberFormatException("Empty version string");
			}

			String[] sa = SpecVersion.Split("\\.", -1);
			int[] si = new int[sa.Length];
			for (int i = 0; i < sa.Length; i++)
			{
				si[i] = Convert.ToInt32(sa[i]);
				if (si[i] < 0)
				{
					throw NumberFormatException.ForInputString("" + si[i]);
				}
			}

			String[] da = desired.Split("\\.", -1);
			int[] di = new int[da.Length];
			for (int i = 0; i < da.Length; i++)
			{
				di[i] = Convert.ToInt32(da[i]);
				if (di[i] < 0)
				{
					throw NumberFormatException.ForInputString("" + di[i]);
				}
			}

			int len = System.Math.Max(di.Length, si.Length);
			for (int i = 0; i < len; i++)
			{
				int d = (i < di.Length ? di[i] : 0);
				int s = (i < si.Length ? si[i] : 0);
				if (s < d)
				{
					return false;
				}
				if (s > d)
				{
					return true;
				}
			}
			return true;
		}

		/// <summary>
		/// Find a package by name in the callers {@code ClassLoader} instance.
		/// The callers {@code ClassLoader} instance is used to find the package
		/// instance corresponding to the named class. If the callers
		/// {@code ClassLoader} instance is null then the set of packages loaded
		/// by the system {@code ClassLoader} instance is searched to find the
		/// named package. <para>
		/// 
		/// Packages have attributes for versions and specifications only if the class
		/// loader created the package instance with the appropriate attributes. Typically,
		/// those attributes are defined in the manifests that accompany the classes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> a package name, for example, java.lang. </param>
		/// <returns> the package of the requested name. It may be null if no package
		///          information is available from the archive or codebase. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Package getPackage(String name)
		public static Package GetPackage(String name)
		{
			ClassLoader l = ClassLoader.GetClassLoader(Reflection.CallerClass);
			if (l != reflect.AnnotatedElement_Fields.Null)
			{
				return l.GetPackage(name);
			}
			else
			{
				return GetSystemPackage(name);
			}
		}

		/// <summary>
		/// Get all the packages currently known for the caller's {@code ClassLoader}
		/// instance.  Those packages correspond to classes loaded via or accessible by
		/// name to that {@code ClassLoader} instance.  If the caller's
		/// {@code ClassLoader} instance is the bootstrap {@code ClassLoader}
		/// instance, which may be represented by {@code null} in some implementations,
		/// only packages corresponding to classes loaded by the bootstrap
		/// {@code ClassLoader} instance will be returned.
		/// </summary>
		/// <returns> a new array of packages known to the callers {@code ClassLoader}
		/// instance.  An zero length array is returned if none are known. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Package[] getPackages()
		public static Package[] Packages
		{
			get
			{
				ClassLoader l = ClassLoader.GetClassLoader(Reflection.CallerClass);
				if (l != reflect.AnnotatedElement_Fields.Null)
				{
					return l.Packages;
				}
				else
				{
					return SystemPackages;
				}
			}
		}

		/// <summary>
		/// Get the package for the specified class.
		/// The class's class loader is used to find the package instance
		/// corresponding to the specified class. If the class loader
		/// is the bootstrap class loader, which may be represented by
		/// {@code null} in some implementations, then the set of packages
		/// loaded by the bootstrap class loader is searched to find the package.
		/// <para>
		/// Packages have attributes for versions and specifications only
		/// if the class loader created the package
		/// instance with the appropriate attributes. Typically those
		/// attributes are defined in the manifests that accompany
		/// the classes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c"> the class to get the package of. </param>
		/// <returns> the package of the class. It may be null if no package
		///          information is available from the archive or codebase.   </returns>
		internal static Package GetPackage(Class c)
		{
			String name = c.Name;
			int i = name.LastIndexOf('.');
			if (i != -1)
			{
				name = name.Substring(0, i);
				ClassLoader cl = c.ClassLoader;
				if (cl != reflect.AnnotatedElement_Fields.Null)
				{
					return cl.GetPackage(name);
				}
				else
				{
					return GetSystemPackage(name);
				}
			}
			else
			{
				return reflect.AnnotatedElement_Fields.Null;
			}
		}

		/// <summary>
		/// Return the hash code computed from the package name. </summary>
		/// <returns> the hash code computed from the package name. </returns>
		public override int HashCode()
		{
			return PkgName.HashCode();
		}

		/// <summary>
		/// Returns the string representation of this Package.
		/// Its value is the string "package " and the package name.
		/// If the package title is defined it is appended.
		/// If the package version is defined it is appended. </summary>
		/// <returns> the string representation of the package. </returns>
		public override String ToString()
		{
			String spec = SpecTitle;
			String ver = SpecVersion;
			if (spec != reflect.AnnotatedElement_Fields.Null && spec.Length() > 0)
			{
				spec = ", " + spec;
			}
			else
			{
				spec = "";
			}
			if (ver != reflect.AnnotatedElement_Fields.Null && ver.Length() > 0)
			{
				ver = ", version " + ver;
			}
			else
			{
				ver = "";
			}
			return "package " + PkgName + spec + ver;
		}

		private Class PackageInfo
		{
			get
			{
				if (PackageInfo_Renamed == reflect.AnnotatedElement_Fields.Null)
				{
					try
					{
					PackageInfo_Renamed = Class.ForName(PkgName + ".package-info", false, Loader);
					}
					catch (ClassNotFoundException)
					{
						// store a proxy for the package info that has no annotations
	//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
	//					class PackageInfoProxy
		//				{
		//				}
					PackageInfo_Renamed = typeof(PackageInfoProxy);
					}
				}
				return PackageInfo_Renamed;
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public virtual A getAnnotation<A>(Class annotationClass) where A : Annotation
		{
			return PackageInfo.GetAnnotation(annotationClass);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public override bool IsAnnotationPresent(Class annotationClass)
		{
			return AnnotatedElement.this.isAnnotationPresent(annotationClass);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override A[] getAnnotationsByType<A>(Class annotationClass) where A : Annotation
		{
			return PackageInfo.GetAnnotationsByType(annotationClass);
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public virtual Annotation[] Annotations
		{
			get
			{
				return PackageInfo.GetCustomAttributes(true);
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override A getDeclaredAnnotation<A>(Class annotationClass) where A : Annotation
		{
			return PackageInfo.GetDeclaredAnnotation(annotationClass);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override A[] getDeclaredAnnotationsByType<A>(Class annotationClass) where A : Annotation
		{
			return PackageInfo.GetDeclaredAnnotationsByType(annotationClass);
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public virtual Annotation[] DeclaredAnnotations
		{
			get
			{
				return PackageInfo.GetCustomAttributes(false);
			}
		}

		/// <summary>
		/// Construct a package instance with the specified version
		/// information. </summary>
		/// <param name="name"> the name of the package </param>
		/// <param name="spectitle"> the title of the specification </param>
		/// <param name="specversion"> the version of the specification </param>
		/// <param name="specvendor"> the organization that maintains the specification </param>
		/// <param name="impltitle"> the title of the implementation </param>
		/// <param name="implversion"> the version of the implementation </param>
		/// <param name="implvendor"> the organization that maintains the implementation </param>
		internal Package(String name, String spectitle, String specversion, String specvendor, String impltitle, String implversion, String implvendor, URL sealbase, ClassLoader loader)
		{
			PkgName = name;
			ImplTitle = impltitle;
			ImplVersion = implversion;
			ImplVendor = implvendor;
			SpecTitle = spectitle;
			SpecVersion = specversion;
			SpecVendor = specvendor;
			SealBase = sealbase;
			this.Loader = loader;
		}

		/*
		 * Construct a package using the attributes from the specified manifest.
		 *
		 * @param name the package name
		 * @param man the optional manifest for the package
		 * @param url the optional code source url for the package
		 */
		private Package(String name, Manifest man, URL url, ClassLoader loader)
		{
			String path = name.Replace('.', '/') + "/";
			String @sealed = reflect.AnnotatedElement_Fields.Null;
			String specTitle = reflect.AnnotatedElement_Fields.Null;
			String specVersion = reflect.AnnotatedElement_Fields.Null;
			String specVendor = reflect.AnnotatedElement_Fields.Null;
			String implTitle = reflect.AnnotatedElement_Fields.Null;
			String implVersion = reflect.AnnotatedElement_Fields.Null;
			String implVendor = reflect.AnnotatedElement_Fields.Null;
			URL sealBase = reflect.AnnotatedElement_Fields.Null;
			Attributes attr = man.GetAttributes(path);
			if (attr != reflect.AnnotatedElement_Fields.Null)
			{
				specTitle = attr.GetValue(Attributes.Name.SPECIFICATION_TITLE);
				specVersion = attr.GetValue(Attributes.Name.SPECIFICATION_VERSION);
				specVendor = attr.GetValue(Attributes.Name.SPECIFICATION_VENDOR);
				implTitle = attr.GetValue(Attributes.Name.IMPLEMENTATION_TITLE);
				implVersion = attr.GetValue(Attributes.Name.IMPLEMENTATION_VERSION);
				implVendor = attr.GetValue(Attributes.Name.IMPLEMENTATION_VENDOR);
				@sealed = attr.GetValue(Attributes.Name.SEALED);
			}
			attr = man.MainAttributes;
			if (attr != reflect.AnnotatedElement_Fields.Null)
			{
				if (specTitle == reflect.AnnotatedElement_Fields.Null)
				{
					specTitle = attr.GetValue(Attributes.Name.SPECIFICATION_TITLE);
				}
				if (specVersion == reflect.AnnotatedElement_Fields.Null)
				{
					specVersion = attr.GetValue(Attributes.Name.SPECIFICATION_VERSION);
				}
				if (specVendor == reflect.AnnotatedElement_Fields.Null)
				{
					specVendor = attr.GetValue(Attributes.Name.SPECIFICATION_VENDOR);
				}
				if (implTitle == reflect.AnnotatedElement_Fields.Null)
				{
					implTitle = attr.GetValue(Attributes.Name.IMPLEMENTATION_TITLE);
				}
				if (implVersion == reflect.AnnotatedElement_Fields.Null)
				{
					implVersion = attr.GetValue(Attributes.Name.IMPLEMENTATION_VERSION);
				}
				if (implVendor == reflect.AnnotatedElement_Fields.Null)
				{
					implVendor = attr.GetValue(Attributes.Name.IMPLEMENTATION_VENDOR);
				}
				if (@sealed == reflect.AnnotatedElement_Fields.Null)
				{
					@sealed = attr.GetValue(Attributes.Name.SEALED);
				}
			}
			if ("true".Equals(@sealed, StringComparison.CurrentCultureIgnoreCase))
			{
				sealBase = url;
			}
			PkgName = name;
			this.SpecTitle = specTitle;
			this.SpecVersion = specVersion;
			this.SpecVendor = specVendor;
			this.ImplTitle = implTitle;
			this.ImplVersion = implVersion;
			this.ImplVendor = implVendor;
			this.SealBase = sealBase;
			this.Loader = loader;
		}

		/*
		 * Returns the loaded system package for the specified name.
		 */
		internal static Package GetSystemPackage(String name)
		{
			lock (Pkgs)
			{
				Package pkg = Pkgs[name];
				if (pkg == reflect.AnnotatedElement_Fields.Null)
				{
					name = name.Replace('.', '/') + "/";
					String fn = getSystemPackage0(name);
					if (fn != reflect.AnnotatedElement_Fields.Null)
					{
						pkg = DefineSystemPackage(name, fn);
					}
				}
				return pkg;
			}
		}

		/*
		 * Return an array of loaded system packages.
		 */
		internal static Package[] SystemPackages
		{
			get
			{
				// First, update the system package map with new package names
				String[] names = SystemPackages0;
				lock (Pkgs)
				{
					for (int i = 0; i < names.Length; i++)
					{
						DefineSystemPackage(names[i], getSystemPackage0(names[i]));
					}
					return Pkgs.Values.toArray(new Package[Pkgs.Count]);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Package defineSystemPackage(final String iname, final String fn)
		private static Package DefineSystemPackage(String iname, String fn)
		{
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(iname, fn));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Package>
		{
			private string Iname;
			private string Fn;

			public PrivilegedActionAnonymousInnerClassHelper(string iname, string fn)
			{
				this.Iname = iname;
				this.Fn = fn;
			}

			public virtual Package Run()
			{
				String name = Iname;
				// Get the cached code source url for the file name
				URL url = Urls[Fn];
				if (url == reflect.AnnotatedElement_Fields.Null)
				{
					// URL not found, so create one
					File file = new File(Fn);
					try
					{
						url = ParseUtil.fileToEncodedURL(file);
					}
					catch (MalformedURLException)
					{
					}
					if (url != reflect.AnnotatedElement_Fields.Null)
					{
						Urls[Fn] = url;
						// If loading a JAR file, then also cache the manifest
						if (file.File)
						{
							Mans[Fn] = LoadManifest(Fn);
						}
					}
				}
				// Convert to "."-separated package name
				name = name.Substring(0, name.Length() - 1).Replace('/', '.');
				Package pkg;
				Manifest man = Mans[Fn];
				if (man != reflect.AnnotatedElement_Fields.Null)
				{
					pkg = new Package(name, man, url, reflect.AnnotatedElement_Fields.Null);
				}
				else
				{
					pkg = new Package(name, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null, reflect.AnnotatedElement_Fields.Null);
				}
				Pkgs[name] = pkg;
				return pkg;
			}
		}

		/*
		 * Returns the Manifest for the specified JAR file name.
		 */
		private static Manifest LoadManifest(String fn)
		{
			try
			{
					using (FileInputStream fis = new FileInputStream(fn), JarInputStream jis = new JarInputStream(fis, false))
					{
					return jis.Manifest;
					}
			}
			catch (IOException)
			{
				return reflect.AnnotatedElement_Fields.Null;
			}
		}

		// The map of loaded system packages
		private static IDictionary<String, Package> Pkgs = new Dictionary<String, Package>(31);

		// Maps each directory or zip file name to its corresponding url
		private static IDictionary<String, URL> Urls = new Dictionary<String, URL>(10);

		// Maps each code source url for a jar file to its manifest
		private static IDictionary<String, Manifest> Mans = new Dictionary<String, Manifest>(10);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String getSystemPackage0(String name);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String[] getSystemPackages0();

		/*
		 * Private storage for the package name and attributes.
		 */
		private readonly String PkgName;
		private readonly String SpecTitle;
		private readonly String SpecVersion;
		private readonly String SpecVendor;
		private readonly String ImplTitle;
		private readonly String ImplVersion;
		private readonly String ImplVendor;
		private readonly URL SealBase;
		[NonSerialized]
		private readonly ClassLoader Loader;
		[NonSerialized]
		private Class PackageInfo_Renamed;
	}

}