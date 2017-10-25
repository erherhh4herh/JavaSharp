using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1994, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using Unsafe = sun.misc.Unsafe;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using ConstantPool = sun.reflect.ConstantPool;
	using Reflection = sun.reflect.Reflection;
	using ReflectionFactory = sun.reflect.ReflectionFactory;
	using CoreReflectionFactory = sun.reflect.generics.factory.CoreReflectionFactory;
	using GenericsFactory = sun.reflect.generics.factory.GenericsFactory;
	using ClassRepository = sun.reflect.generics.repository.ClassRepository;
	using MethodRepository = sun.reflect.generics.repository.MethodRepository;
	using ConstructorRepository = sun.reflect.generics.repository.ConstructorRepository;
	using ClassScope = sun.reflect.generics.scope.ClassScope;
	using SecurityConstants = sun.security.util.SecurityConstants;
	using sun.reflect.annotation;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// Instances of the class {@code Class} represent classes and
	/// interfaces in a running Java application.  An enum is a kind of
	/// class and an annotation is a kind of interface.  Every array also
	/// belongs to a class that is reflected as a {@code Class} object
	/// that is shared by all arrays with the same element type and number
	/// of dimensions.  The primitive Java types ({@code boolean},
	/// {@code byte}, {@code char}, {@code short},
	/// {@code int}, {@code long}, {@code float}, and
	/// {@code double}), and the keyword {@code void} are also
	/// represented as {@code Class} objects.
	/// 
	/// <para> {@code Class} has no public constructor. Instead {@code Class}
	/// objects are constructed automatically by the Java Virtual Machine as classes
	/// are loaded and by calls to the {@code defineClass} method in the class
	/// loader.
	/// 
	/// </para>
	/// <para> The following example uses a {@code Class} object to print the
	/// class name of an object:
	/// 
	/// <blockquote><pre>
	///     void printClassName(Object obj) {
	///         System.out.println("The class of " + obj +
	///                            " is " + obj.getClass().getName());
	///     }
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para> It is also possible to get the {@code Class} object for a named
	/// type (or for void) using a class literal.  See Section 15.8.2 of
	/// <cite>The Java&trade; Language Specification</cite>.
	/// For example:
	/// 
	/// <blockquote>
	///     {@code System.out.println("The name of class Foo is: "+Foo.class.getName());}
	/// </blockquote>
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the class modeled by this {@code Class}
	/// object.  For example, the type of {@code String.class} is {@code
	/// Class<String>}.  Use {@code Class<?>} if the class being modeled is
	/// unknown.
	/// 
	/// @author  unascribed </param>
	/// <seealso cref=     java.lang.ClassLoader#defineClass(byte[], int, int)
	/// @since   JDK1.0 </seealso>
	[Serializable]
	public sealed class Class : GenericDeclaration, Type, AnnotatedElement
	{
		private const int ANNOTATION = 0x00002000;
		private const int ENUM = 0x00004000;
		private const int SYNTHETIC = 0x00001000;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void registerNatives();
		static Class()
		{
			registerNatives();
				Field[] fields = typeof(Class).getDeclaredFields0(false); // bypass caches
				reflectionDataOffset = objectFieldOffset(fields, "reflectionData");
				annotationTypeOffset = objectFieldOffset(fields, "annotationType");
				annotationDataOffset = objectFieldOffset(fields, "annotationData");
		}

		/*
		 * Private constructor. Only the Java Virtual Machine creates Class objects.
		 * This constructor is not used and prevents the default constructor being
		 * generated.
		 */
		private Class(ClassLoader loader)
		{
			// Initialize final field for classLoader.  The initialization value of non-null
			// prevents future JIT optimizations from assuming this final field is null.
			ClassLoader_Renamed = loader;
		}

		/// <summary>
		/// Converts the object to a string. The string representation is the
		/// string "class" or "interface", followed by a space, and then by the
		/// fully qualified name of the class in the format returned by
		/// {@code getName}.  If this {@code Class} object represents a
		/// primitive type, this method returns the name of the primitive type.  If
		/// this {@code Class} object represents void this method returns
		/// "void".
		/// </summary>
		/// <returns> a string representation of this class object. </returns>
		public override String ToString()
		{
			return (Interface ? "interface " : (Primitive ? "" : "class ")) + Name;
		}

		/// <summary>
		/// Returns a string describing this {@code Class}, including
		/// information about modifiers and type parameters.
		/// 
		/// The string is formatted as a list of type modifiers, if any,
		/// followed by the kind of type (empty string for primitive types
		/// and {@code class}, {@code enum}, {@code interface}, or
		/// <code>&#64;</code>{@code interface}, as appropriate), followed
		/// by the type's name, followed by an angle-bracketed
		/// comma-separated list of the type's type parameters, if any.
		/// 
		/// A space is used to separate modifiers from one another and to
		/// separate any modifiers from the kind of type. The modifiers
		/// occur in canonical order. If there are no type parameters, the
		/// type parameter list is elided.
		/// 
		/// <para>Note that since information about the runtime representation
		/// of a type is being generated, modifiers not present on the
		/// originating source code or illegal on the originating source
		/// code may be present.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Class}, including
		/// information about modifiers and type parameters
		/// 
		/// @since 1.8 </returns>
		public String ToGenericString()
		{
			if (Primitive)
			{
				return ToString();
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				// Class modifiers are a superset of interface modifiers
				int modifiers = Modifiers & Modifier.classModifiers();
				if (modifiers != 0)
				{
					sb.Append(Modifier.ToString(modifiers));
					sb.Append(' ');
				}

				if (Annotation)
				{
					sb.Append('@');
				}
				if (Interface) // Note: all annotation types are interfaces
				{
					sb.Append("interface");
				}
				else
				{
					if (Enum)
					{
						sb.Append("enum");
					}
					else
					{
						sb.Append("class");
					}
				}
				sb.Append(' ');
				sb.Append(Name);

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: TypeVariable<?>[] typeparms = getTypeParameters();
				TypeVariable<?>[] typeparms = TypeParameters;
				if (typeparms.Length > 0)
				{
					bool first = true;
					sb.Append('<');
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for(TypeVariable<?> typeparm: typeparms)
					foreach (TypeVariable<?> typeparm in typeparms)
					{
						if (!first)
						{
							sb.Append(',');
						}
						sb.Append(typeparm.TypeName);
						first = false;
					}
					sb.Append('>');
				}

				return sb.ToString();
			}
		}

		/// <summary>
		/// Returns the {@code Class} object associated with the class or
		/// interface with the given string name.  Invoking this method is
		/// equivalent to:
		/// 
		/// <blockquote>
		///  {@code Class.forName(className, true, currentLoader)}
		/// </blockquote>
		/// 
		/// where {@code currentLoader} denotes the defining class loader of
		/// the current class.
		/// 
		/// <para> For example, the following code fragment returns the
		/// runtime {@code Class} descriptor for the class named
		/// {@code java.lang.Thread}:
		/// 
		/// <blockquote>
		///   {@code Class t = Class.forName("java.lang.Thread")}
		/// </blockquote>
		/// </para>
		/// <para>
		/// A call to {@code forName("X")} causes the class named
		/// {@code X} to be initialized.
		/// 
		/// </para>
		/// </summary>
		/// <param name="className">   the fully qualified name of the desired class. </param>
		/// <returns>     the {@code Class} object for the class with the
		///             specified name. </returns>
		/// <exception cref="LinkageError"> if the linkage fails </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///            by this method fails </exception>
		/// <exception cref="ClassNotFoundException"> if the class cannot be located </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Class forName(String className) throws ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Class ForName(String className)
		{
			Class caller = Reflection.CallerClass;
			return forName0(className, true, ClassLoader.GetClassLoader(caller), caller);
		}


		/// <summary>
		/// Returns the {@code Class} object associated with the class or
		/// interface with the given string name, using the given class loader.
		/// Given the fully qualified name for a class or interface (in the same
		/// format returned by {@code getName}) this method attempts to
		/// locate, load, and link the class or interface.  The specified class
		/// loader is used to load the class or interface.  If the parameter
		/// {@code loader} is null, the class is loaded through the bootstrap
		/// class loader.  The class is initialized only if the
		/// {@code initialize} parameter is {@code true} and if it has
		/// not been initialized earlier.
		/// 
		/// <para> If {@code name} denotes a primitive type or void, an attempt
		/// will be made to locate a user-defined class in the unnamed package whose
		/// name is {@code name}. Therefore, this method cannot be used to
		/// obtain any of the {@code Class} objects representing primitive
		/// types or void.
		/// 
		/// </para>
		/// <para> If {@code name} denotes an array class, the component type of
		/// the array class is loaded but not initialized.
		/// 
		/// </para>
		/// <para> For example, in an instance method the expression:
		/// 
		/// <blockquote>
		///  {@code Class.forName("Foo")}
		/// </blockquote>
		/// 
		/// is equivalent to:
		/// 
		/// <blockquote>
		///  {@code Class.forName("Foo", true, this.getClass().getClassLoader())}
		/// </blockquote>
		/// 
		/// Note that this method throws errors related to loading, linking or
		/// initializing as specified in Sections 12.2, 12.3 and 12.4 of <em>The
		/// Java Language Specification</em>.
		/// Note that this method does not check whether the requested class
		/// is accessible to its caller.
		/// 
		/// </para>
		/// <para> If the {@code loader} is {@code null}, and a security
		/// manager is present, and the caller's class loader is not null, then this
		/// method calls the security manager's {@code checkPermission} method
		/// with a {@code RuntimePermission("getClassLoader")} permission to
		/// ensure it's ok to access the bootstrap class loader.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">       fully qualified name of the desired class </param>
		/// <param name="initialize"> if {@code true} the class will be initialized.
		///                   See Section 12.4 of <em>The Java Language Specification</em>. </param>
		/// <param name="loader">     class loader from which the class must be loaded </param>
		/// <returns>           class object representing the desired class
		/// </returns>
		/// <exception cref="LinkageError"> if the linkage fails </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///            by this method fails </exception>
		/// <exception cref="ClassNotFoundException"> if the class cannot be located by
		///            the specified class loader
		/// </exception>
		/// <seealso cref=       java.lang.Class#forName(String) </seealso>
		/// <seealso cref=       java.lang.ClassLoader
		/// @since     1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Class forName(String name, boolean initialize, ClassLoader loader) throws ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Class ForName(String name, bool initialize, ClassLoader loader)
		{
			Class caller = null;
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				// Reflective call to get caller class is only needed if a security manager
				// is present.  Avoid the overhead of making this call otherwise.
				caller = Reflection.CallerClass;
				if (sun.misc.VM.isSystemDomainLoader(loader))
				{
					ClassLoader ccl = ClassLoader.GetClassLoader(caller);
					if (!sun.misc.VM.isSystemDomainLoader(ccl))
					{
						sm.CheckPermission(SecurityConstants.GET_CLASSLOADER_PERMISSION);
					}
				}
			}
			return forName0(name, initialize, loader, caller);
		}

		/// <summary>
		/// Called after security check for system loader access checks have been made. </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern Class forName0(String name, bool initialize, ClassLoader loader, Class caller);

		/// <summary>
		/// Creates a new instance of the class represented by this {@code Class}
		/// object.  The class is instantiated as if by a {@code new}
		/// expression with an empty argument list.  The class is initialized if it
		/// has not already been initialized.
		/// 
		/// <para>Note that this method propagates any exception thrown by the
		/// nullary constructor, including a checked exception.  Use of
		/// this method effectively bypasses the compile-time exception
		/// checking that would otherwise be performed by the compiler.
		/// The {@link
		/// java.lang.reflect.Constructor#newInstance(java.lang.Object...)
		/// Constructor.newInstance} method avoids this problem by wrapping
		/// any exception thrown by the constructor in a (checked) {@link
		/// java.lang.reflect.InvocationTargetException}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a newly allocated instance of the class represented by this
		///          object. </returns>
		/// <exception cref="IllegalAccessException">  if the class or its nullary
		///          constructor is not accessible. </exception>
		/// <exception cref="InstantiationException">
		///          if this {@code Class} represents an abstract class,
		///          an interface, an array class, a primitive type, or void;
		///          or if the class has no nullary constructor;
		///          or if the instantiation fails for some other reason. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization
		///          provoked by this method fails. </exception>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and
		///          the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public T newInstance() throws InstantiationException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public T NewInstance()
		{
			if (System.SecurityManager != null)
			{
				CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, false);
			}

			// NOTE: the following code may not be strictly correct under
			// the current Java memory model.

			// Constructor lookup
			if (CachedConstructor == null)
			{
				if (this == typeof(Class))
				{
					throw new IllegalAccessException("Can not call newInstance() on the Class for java.lang.Class");
				}
				try
				{
					Class[] empty = new Class[] {};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Constructor<T> c = getConstructor0(empty, Member.DECLARED);
					Constructor<T> c = GetConstructor0(empty, Member.DECLARED);
					// Disable accessibility checks on the constructor
					// since we have to do the security check here anyway
					// (the stack depth is wrong for the Constructor's
					// security check to work)
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, c));
					CachedConstructor = c;
				}
				catch (NoSuchMethodException e)
				{
					throw (InstantiationException) (new InstantiationException(Name)).InitCause(e);
				}
			}
			Constructor<T> tmpConstructor = CachedConstructor;
			// Security check (same as in java.lang.reflect.Constructor)
			int modifiers = tmpConstructor.Modifiers;
			if (!Reflection.quickCheckMemberAccess(this, modifiers))
			{
				Class caller = Reflection.CallerClass;
				if (NewInstanceCallerCache != caller)
				{
					Reflection.ensureMemberAccess(caller, this, null, modifiers);
					NewInstanceCallerCache = caller;
				}
			}
			// Run constructor
			try
			{
				return tmpConstructor.newInstance((Object[])null);
			}
			catch (InvocationTargetException e)
			{
				Unsafe.Unsafe.throwException(e.TargetException);
				// Not reached
				return null;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly Class OuterInstance;

			private Constructor<T> c;

			public PrivilegedActionAnonymousInnerClassHelper(Class outerInstance, Constructor<T> c)
			{
				this.OuterInstance = outerInstance;
				this.c = c;
			}

			public virtual Void Run()
			{
					c.Accessible = true;
					return null;
			}
		}
		[NonSerialized]
		private volatile Constructor<T> CachedConstructor;
		[NonSerialized]
		private volatile Class NewInstanceCallerCache;


		/// <summary>
		/// Determines if the specified {@code Object} is assignment-compatible
		/// with the object represented by this {@code Class}.  This method is
		/// the dynamic equivalent of the Java language {@code instanceof}
		/// operator. The method returns {@code true} if the specified
		/// {@code Object} argument is non-null and can be cast to the
		/// reference type represented by this {@code Class} object without
		/// raising a {@code ClassCastException.} It returns {@code false}
		/// otherwise.
		/// 
		/// <para> Specifically, if this {@code Class} object represents a
		/// declared class, this method returns {@code true} if the specified
		/// {@code Object} argument is an instance of the represented class (or
		/// of any of its subclasses); it returns {@code false} otherwise. If
		/// this {@code Class} object represents an array class, this method
		/// returns {@code true} if the specified {@code Object} argument
		/// can be converted to an object of the array class by an identity
		/// conversion or by a widening reference conversion; it returns
		/// {@code false} otherwise. If this {@code Class} object
		/// represents an interface, this method returns {@code true} if the
		/// class or any superclass of the specified {@code Object} argument
		/// implements this interface; it returns {@code false} otherwise. If
		/// this {@code Class} object represents a primitive type, this method
		/// returns {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object to check </param>
		/// <returns>  true if {@code obj} is an instance of this class
		/// 
		/// @since JDK1.1 </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern boolean isInstance(Object obj);


		/// <summary>
		/// Determines if the class or interface represented by this
		/// {@code Class} object is either the same as, or is a superclass or
		/// superinterface of, the class or interface represented by the specified
		/// {@code Class} parameter. It returns {@code true} if so;
		/// otherwise it returns {@code false}. If this {@code Class}
		/// object represents a primitive type, this method returns
		/// {@code true} if the specified {@code Class} parameter is
		/// exactly this {@code Class} object; otherwise it returns
		/// {@code false}.
		/// 
		/// <para> Specifically, this method tests whether the type represented by the
		/// specified {@code Class} parameter can be converted to the type
		/// represented by this {@code Class} object via an identity conversion
		/// or via a widening reference conversion. See <em>The Java Language
		/// Specification</em>, sections 5.1.1 and 5.1.4 , for details.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cls"> the {@code Class} object to be checked </param>
		/// <returns> the {@code boolean} value indicating whether objects of the
		/// type {@code cls} can be assigned to objects of this class </returns>
		/// <exception cref="NullPointerException"> if the specified Class parameter is
		///            null.
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern boolean isAssignableFrom(Class cls);


		/// <summary>
		/// Determines if the specified {@code Class} object represents an
		/// interface type.
		/// </summary>
		/// <returns>  {@code true} if this object represents an interface;
		///          {@code false} otherwise. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern boolean isInterface();


		/// <summary>
		/// Determines if this {@code Class} object represents an array class.
		/// </summary>
		/// <returns>  {@code true} if this object represents an array class;
		///          {@code false} otherwise.
		/// @since   JDK1.1 </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern boolean isArray();


		/// <summary>
		/// Determines if the specified {@code Class} object represents a
		/// primitive type.
		/// 
		/// <para> There are nine predefined {@code Class} objects to represent
		/// the eight primitive types and void.  These are created by the Java
		/// Virtual Machine, and have the same names as the primitive types that
		/// they represent, namely {@code boolean}, {@code byte},
		/// {@code char}, {@code short}, {@code int},
		/// {@code long}, {@code float}, and {@code double}.
		/// 
		/// </para>
		/// <para> These objects may only be accessed via the following public static
		/// final variables, and are the only {@code Class} objects for which
		/// this method returns {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if and only if this class represents a primitive type
		/// </returns>
		/// <seealso cref=     java.lang.Boolean#TYPE </seealso>
		/// <seealso cref=     java.lang.Character#TYPE </seealso>
		/// <seealso cref=     java.lang.Byte#TYPE </seealso>
		/// <seealso cref=     java.lang.Short#TYPE </seealso>
		/// <seealso cref=     java.lang.Integer#TYPE </seealso>
		/// <seealso cref=     java.lang.Long#TYPE </seealso>
		/// <seealso cref=     java.lang.Float#TYPE </seealso>
		/// <seealso cref=     java.lang.Double#TYPE </seealso>
		/// <seealso cref=     java.lang.Void#TYPE
		/// @since JDK1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern boolean isPrimitive();

		/// <summary>
		/// Returns true if this {@code Class} object represents an annotation
		/// type.  Note that if this method returns true, <seealso cref="#isInterface()"/>
		/// would also return true, as all annotation types are also interfaces.
		/// </summary>
		/// <returns> {@code true} if this class object represents an annotation
		///      type; {@code false} otherwise
		/// @since 1.5 </returns>
		public bool Annotation
		{
			get
			{
				return (Modifiers & ANNOTATION) != 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if this class is a synthetic class;
		/// returns {@code false} otherwise. </summary>
		/// <returns> {@code true} if and only if this class is a synthetic class as
		///         defined by the Java Language Specification.
		/// @jls 13.1 The Form of a Binary
		/// @since 1.5 </returns>
		public bool Synthetic
		{
			get
			{
				return (Modifiers & SYNTHETIC) != 0;
			}
		}

		/// <summary>
		/// Returns the  name of the entity (class, interface, array class,
		/// primitive type, or void) represented by this {@code Class} object,
		/// as a {@code String}.
		/// 
		/// <para> If this class object represents a reference type that is not an
		/// array type then the binary name of the class is returned, as specified
		/// by
		/// <cite>The Java&trade; Language Specification</cite>.
		/// 
		/// </para>
		/// <para> If this class object represents a primitive type or void, then the
		/// name returned is a {@code String} equal to the Java language
		/// keyword corresponding to the primitive type or void.
		/// 
		/// </para>
		/// <para> If this class object represents a class of arrays, then the internal
		/// form of the name consists of the name of the element type preceded by
		/// one or more '{@code [}' characters representing the depth of the array
		/// nesting.  The encoding of element type names is as follows:
		/// 
		/// <blockquote><table summary="Element types and encodings">
		/// <tr><th> Element Type <th> &nbsp;&nbsp;&nbsp; <th> Encoding
		/// <tr><td> boolean      <td> &nbsp;&nbsp;&nbsp; <td align=center> Z
		/// <tr><td> byte         <td> &nbsp;&nbsp;&nbsp; <td align=center> B
		/// <tr><td> char         <td> &nbsp;&nbsp;&nbsp; <td align=center> C
		/// <tr><td> class or interface
		///                       <td> &nbsp;&nbsp;&nbsp; <td align=center> L<i>classname</i>;
		/// <tr><td> double       <td> &nbsp;&nbsp;&nbsp; <td align=center> D
		/// <tr><td> float        <td> &nbsp;&nbsp;&nbsp; <td align=center> F
		/// <tr><td> int          <td> &nbsp;&nbsp;&nbsp; <td align=center> I
		/// <tr><td> long         <td> &nbsp;&nbsp;&nbsp; <td align=center> J
		/// <tr><td> short        <td> &nbsp;&nbsp;&nbsp; <td align=center> S
		/// </table></blockquote>
		/// 
		/// </para>
		/// <para> The class or interface name <i>classname</i> is the binary name of
		/// the class specified above.
		/// 
		/// </para>
		/// <para> Examples:
		/// <blockquote><pre>
		/// String.class.getName()
		///     returns "java.lang.String"
		/// byte.class.getName()
		///     returns "byte"
		/// (new Object[3]).getClass().getName()
		///     returns "[Ljava.lang.Object;"
		/// (new int[3][4][5][6][7][8][9]).getClass().getName()
		///     returns "[[[[[[[I"
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the name of the class or interface
		///          represented by this object. </returns>
		public String Name
		{
			get
			{
				String name = this.Name_Renamed;
				if (name == null)
				{
					this.Name_Renamed = name = Name0;
				}
				return name;
			}
		}

		// cache the name to reduce the number of calls into the VM
		[NonSerialized]
		private String Name_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String getName0();

		/// <summary>
		/// Returns the class loader for the class.  Some implementations may use
		/// null to represent the bootstrap class loader. This method will return
		/// null in such implementations if this class was loaded by the bootstrap
		/// class loader.
		/// 
		/// <para> If a security manager is present, and the caller's class loader is
		/// not null and the caller's class loader is not the same as or an ancestor of
		/// the class loader for the class whose class loader is requested, then
		/// this method calls the security manager's {@code checkPermission}
		/// method with a {@code RuntimePermission("getClassLoader")}
		/// permission to ensure it's ok to access the class loader for the class.
		/// 
		/// </para>
		/// <para>If this object
		/// represents a primitive type or void, null is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the class loader that loaded the class or interface
		///          represented by this object. </returns>
		/// <exception cref="SecurityException">
		///    if a security manager exists and its
		///    {@code checkPermission} method denies
		///    access to the class loader for the class. </exception>
		/// <seealso cref= java.lang.ClassLoader </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.lang.RuntimePermission </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public ClassLoader getClassLoader()
		public ClassLoader ClassLoader
		{
			get
			{
				ClassLoader cl = ClassLoader0;
				if (cl == null)
				{
					return null;
				}
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					ClassLoader.CheckClassLoaderPermission(cl, Reflection.CallerClass);
				}
				return cl;
			}
		}

		// Package-private to allow ClassLoader access
		internal ClassLoader ClassLoader0
		{
			get
			{
				return ClassLoader_Renamed;
			}
		}

		// Initialized in JVM not by private constructor
		// This field is filtered from reflection access, i.e. getDeclaredField
		// will throw NoSuchFieldException
		private readonly ClassLoader ClassLoader_Renamed;

		/// <summary>
		/// Returns an array of {@code TypeVariable} objects that represent the
		/// type variables declared by the generic declaration represented by this
		/// {@code GenericDeclaration} object, in declaration order.  Returns an
		/// array of length 0 if the underlying generic declaration declares no type
		/// variables.
		/// </summary>
		/// <returns> an array of {@code TypeVariable} objects that represent
		///     the type variables declared by this generic declaration </returns>
		/// <exception cref="java.lang.reflect.GenericSignatureFormatError"> if the generic
		///     signature of this generic declaration does not conform to
		///     the format specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite>
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public TypeVariable<Class>[] getTypeParameters()
		public TypeVariable<Class>[] TypeParameters
		{
			get
			{
				ClassRepository info = GenericInfo;
				if (info != null)
				{
					return (TypeVariable<Class>[])info.TypeParameters;
				}
				else
				{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: return (TypeVariable<Class>[])new TypeVariable<?>[0];
					return (TypeVariable<Class>[])new TypeVariable<?>[0];
				}
			}
		}


		/// <summary>
		/// Returns the {@code Class} representing the superclass of the entity
		/// (class, interface, primitive type or void) represented by this
		/// {@code Class}.  If this {@code Class} represents either the
		/// {@code Object} class, an interface, a primitive type, or void, then
		/// null is returned.  If this object represents an array class then the
		/// {@code Class} object representing the {@code Object} class is
		/// returned.
		/// </summary>
		/// <returns> the superclass of the class represented by this object. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern Class getSuperclass();


		/// <summary>
		/// Returns the {@code Type} representing the direct superclass of
		/// the entity (class, interface, primitive type or void) represented by
		/// this {@code Class}.
		/// 
		/// <para>If the superclass is a parameterized type, the {@code Type}
		/// object returned must accurately reflect the actual type
		/// parameters used in the source code. The parameterized type
		/// representing the superclass is created if it had not been
		/// created before. See the declaration of {@link
		/// java.lang.reflect.ParameterizedType ParameterizedType} for the
		/// semantics of the creation process for parameterized types.  If
		/// this {@code Class} represents either the {@code Object}
		/// class, an interface, a primitive type, or void, then null is
		/// returned.  If this object represents an array class then the
		/// {@code Class} object representing the {@code Object} class is
		/// returned.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="java.lang.reflect.GenericSignatureFormatError"> if the generic
		///     class signature does not conform to the format specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if the generic superclass
		///     refers to a non-existent type declaration </exception>
		/// <exception cref="java.lang.reflect.MalformedParameterizedTypeException"> if the
		///     generic superclass refers to a parameterized type that cannot be
		///     instantiated  for any reason </exception>
		/// <returns> the superclass of the class represented by this object
		/// @since 1.5 </returns>
		public Type GenericSuperclass
		{
			get
			{
				ClassRepository info = GenericInfo;
				if (info == null)
				{
					return Superclass;
				}
    
				// Historical irregularity:
				// Generic signature marks interfaces with superclass = Object
				// but this API returns null for interfaces
				if (Interface)
				{
					return null;
				}
    
				return info.BaseType;
			}
		}

		/// <summary>
		/// Gets the package for this class.  The class loader of this class is used
		/// to find the package.  If the class was loaded by the bootstrap class
		/// loader the set of packages loaded from CLASSPATH is searched to find the
		/// package of the class. Null is returned if no package object was created
		/// by the class loader of this class.
		/// 
		/// <para> Packages have attributes for versions and specifications only if the
		/// information was defined in the manifests that accompany the classes, and
		/// if the class loader created the package instance with the attributes
		/// from the manifest.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the package of the class, or null if no package
		///         information is available from the archive or codebase. </returns>
		public Package Package
		{
			get
			{
				return Package.GetPackage(this);
			}
		}


		/// <summary>
		/// Determines the interfaces implemented by the class or interface
		/// represented by this object.
		/// 
		/// <para> If this object represents a class, the return value is an array
		/// containing objects representing all interfaces implemented by the
		/// class. The order of the interface objects in the array corresponds to
		/// the order of the interface names in the {@code implements} clause
		/// of the declaration of the class represented by this object. For
		/// example, given the declaration:
		/// <blockquote>
		/// {@code class Shimmer implements FloorWax, DessertTopping { ... }}
		/// </blockquote>
		/// suppose the value of {@code s} is an instance of
		/// {@code Shimmer}; the value of the expression:
		/// <blockquote>
		/// {@code s.getClass().getInterfaces()[0]}
		/// </blockquote>
		/// is the {@code Class} object that represents interface
		/// {@code FloorWax}; and the value of:
		/// <blockquote>
		/// {@code s.getClass().getInterfaces()[1]}
		/// </blockquote>
		/// is the {@code Class} object that represents interface
		/// {@code DessertTopping}.
		/// 
		/// </para>
		/// <para> If this object represents an interface, the array contains objects
		/// representing all interfaces extended by the interface. The order of the
		/// interface objects in the array corresponds to the order of the interface
		/// names in the {@code extends} clause of the declaration of the
		/// interface represented by this object.
		/// 
		/// </para>
		/// <para> If this object represents a class or interface that implements no
		/// interfaces, the method returns an array of length 0.
		/// 
		/// </para>
		/// <para> If this object represents a primitive type or void, the method
		/// returns an array of length 0.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, the
		/// interfaces {@code Cloneable} and {@code java.io.Serializable} are
		/// returned in that order.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of interfaces implemented by this class. </returns>
		public Class[] Interfaces
		{
			get
			{
				ReflectionData<T> rd = ReflectionData();
				if (rd == null)
				{
					// no cloning required
					return Interfaces0;
				}
				else
				{
					Class[] interfaces = rd.Interfaces;
					if (interfaces == null)
					{
						interfaces = Interfaces0;
						rd.Interfaces = interfaces;
					}
					// defensively copy before handing over to user code
					return interfaces.clone();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class[] getInterfaces0();

		/// <summary>
		/// Returns the {@code Type}s representing the interfaces
		/// directly implemented by the class or interface represented by
		/// this object.
		/// 
		/// <para>If a superinterface is a parameterized type, the
		/// {@code Type} object returned for it must accurately reflect
		/// the actual type parameters used in the source code. The
		/// parameterized type representing each superinterface is created
		/// if it had not been created before. See the declaration of
		/// <seealso cref="java.lang.reflect.ParameterizedType ParameterizedType"/>
		/// for the semantics of the creation process for parameterized
		/// types.
		/// 
		/// </para>
		/// <para> If this object represents a class, the return value is an
		/// array containing objects representing all interfaces
		/// implemented by the class. The order of the interface objects in
		/// the array corresponds to the order of the interface names in
		/// the {@code implements} clause of the declaration of the class
		/// represented by this object.  In the case of an array class, the
		/// interfaces {@code Cloneable} and {@code Serializable} are
		/// returned in that order.
		/// 
		/// </para>
		/// <para>If this object represents an interface, the array contains
		/// objects representing all interfaces directly extended by the
		/// interface.  The order of the interface objects in the array
		/// corresponds to the order of the interface names in the
		/// {@code extends} clause of the declaration of the interface
		/// represented by this object.
		/// 
		/// </para>
		/// <para>If this object represents a class or interface that
		/// implements no interfaces, the method returns an array of length
		/// 0.
		/// 
		/// </para>
		/// <para>If this object represents a primitive type or void, the
		/// method returns an array of length 0.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="java.lang.reflect.GenericSignatureFormatError">
		///     if the generic class signature does not conform to the format
		///     specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if any of the generic
		///     superinterfaces refers to a non-existent type declaration </exception>
		/// <exception cref="java.lang.reflect.MalformedParameterizedTypeException">
		///     if any of the generic superinterfaces refer to a parameterized
		///     type that cannot be instantiated for any reason </exception>
		/// <returns> an array of interfaces implemented by this class
		/// @since 1.5 </returns>
		public Type[] GenericInterfaces
		{
			get
			{
				ClassRepository info = GenericInfo;
				return (info == null) ? Interfaces : info.SuperInterfaces;
			}
		}


		/// <summary>
		/// Returns the {@code Class} representing the component type of an
		/// array.  If this class does not represent an array class this method
		/// returns null.
		/// </summary>
		/// <returns> the {@code Class} representing the component type of this
		/// class if this class is an array </returns>
		/// <seealso cref=     java.lang.reflect.Array
		/// @since JDK1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern Class getComponentType();


		/// <summary>
		/// Returns the Java language modifiers for this class or interface, encoded
		/// in an integer. The modifiers consist of the Java Virtual Machine's
		/// constants for {@code public}, {@code protected},
		/// {@code private}, {@code final}, {@code static},
		/// {@code abstract} and {@code interface}; they should be decoded
		/// using the methods of class {@code Modifier}.
		/// 
		/// <para> If the underlying class is an array class, then its
		/// {@code public}, {@code private} and {@code protected}
		/// modifiers are the same as those of its component type.  If this
		/// {@code Class} represents a primitive type or void, its
		/// {@code public} modifier is always {@code true}, and its
		/// {@code protected} and {@code private} modifiers are always
		/// {@code false}. If this object represents an array class, a
		/// primitive type or void, then its {@code final} modifier is always
		/// {@code true} and its interface modifier is always
		/// {@code false}. The values of its other modifiers are not determined
		/// by this specification.
		/// 
		/// </para>
		/// <para> The modifier encodings are defined in <em>The Java Virtual Machine
		/// Specification</em>, table 4.1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code int} representing the modifiers for this class </returns>
		/// <seealso cref=     java.lang.reflect.Modifier
		/// @since JDK1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern int getModifiers();


		/// <summary>
		/// Gets the signers of this class.
		/// </summary>
		/// <returns>  the signers of this class, or null if there are no signers.  In
		///          particular, this method returns null if this object represents
		///          a primitive type or void.
		/// @since   JDK1.1 </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern Object[] getSigners();


		/// <summary>
		/// Set the signers of this class.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern void setSigners(Object[] signers);


		/// <summary>
		/// If this {@code Class} object represents a local or anonymous
		/// class within a method, returns a {@link
		/// java.lang.reflect.Method Method} object representing the
		/// immediately enclosing method of the underlying class. Returns
		/// {@code null} otherwise.
		/// 
		/// In particular, this method returns {@code null} if the underlying
		/// class is a local or anonymous class immediately enclosed by a type
		/// declaration, instance initializer or static initializer.
		/// </summary>
		/// <returns> the immediately enclosing method of the underlying class, if
		///     that class is a local or anonymous class; otherwise {@code null}.
		/// </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and any of the
		///         following conditions is met:
		/// 
		///         <ul>
		/// 
		///         <li> the caller's class loader is not the same as the
		///         class loader of the enclosing class and invocation of
		///         {@link SecurityManager#checkPermission
		///         s.checkPermission} method with
		///         {@code RuntimePermission("accessDeclaredMembers")}
		///         denies access to the methods within the enclosing class
		/// 
		///         <li> the caller's class loader is not the same as or an
		///         ancestor of the class loader for the enclosing class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of the enclosing class
		/// 
		///         </ul>
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Method getEnclosingMethod() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Method EnclosingMethod
		{
			get
			{
				EnclosingMethodInfo enclosingInfo = EnclosingMethodInfo;
    
				if (enclosingInfo == null)
				{
					return null;
				}
				else
				{
					if (!enclosingInfo.Method)
					{
						return null;
					}
    
					MethodRepository typeInfo = MethodRepository.make(enclosingInfo.Descriptor, Factory);
					Class returnType = ToClass(typeInfo.ReturnType);
					Type[] parameterTypes = typeInfo.ParameterTypes;
					Class[] parameterClasses = new Class[parameterTypes.Length];
    
					// Convert Types to Classes; returned types *should*
					// be class objects since the methodDescriptor's used
					// don't have generics information
					for (int i = 0; i < parameterClasses.Length; i++)
					{
						parameterClasses[i] = ToClass(parameterTypes[i]);
					}
    
					// Perform access check
					Class enclosingCandidate = enclosingInfo.EnclosingClass;
					enclosingCandidate.CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
					/*
					 * Loop over all declared methods; match method name,
					 * number of and type of parameters, *and* return
					 * type.  Matching return type is also necessary
					 * because of covariant returns, etc.
					 */
					foreach (Method m in enclosingCandidate.DeclaredMethods)
					{
						if (m.Name.Equals(enclosingInfo.Name))
						{
							Class[] candidateParamClasses = m.ParameterTypes;
							if (candidateParamClasses.Length == parameterClasses.Length)
							{
								bool matches = true;
								for (int i = 0; i < candidateParamClasses.Length; i++)
								{
									if (!candidateParamClasses[i].Equals(parameterClasses[i]))
									{
										matches = false;
										break;
									}
								}
    
								if (matches) // finally, check return type
								{
									if (m.ReturnType.Equals(returnType))
									{
										return m;
									}
								}
							}
						}
					}
    
					throw new InternalError("Enclosing method not found");
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Object[] getEnclosingMethod0();

		private EnclosingMethodInfo EnclosingMethodInfo
		{
			get
			{
				Object[] enclosingInfo = EnclosingMethod0;
				if (enclosingInfo == null)
				{
					return null;
				}
				else
				{
					return new EnclosingMethodInfo(enclosingInfo);
				}
			}
		}

		private sealed class EnclosingMethodInfo
		{
			internal Class EnclosingClass_Renamed;
			internal String Name_Renamed;
			internal String Descriptor_Renamed;

			internal EnclosingMethodInfo(Object[] enclosingInfo)
			{
				if (enclosingInfo.Length != 3)
				{
					throw new InternalError("Malformed enclosing method information");
				}
				try
				{
					// The array is expected to have three elements:

					// the immediately enclosing class
					EnclosingClass_Renamed = (Class) enclosingInfo[0];
					assert(EnclosingClass_Renamed != null);

					// the immediately enclosing method or constructor's
					// name (can be null).
					Name_Renamed = (String) enclosingInfo[1];

					// the immediately enclosing method or constructor's
					// descriptor (null iff name is).
					Descriptor_Renamed = (String) enclosingInfo[2];
					assert((Name_Renamed != null && Descriptor_Renamed != null) || Name_Renamed == Descriptor_Renamed);
				}
				catch (ClassCastException cce)
				{
					throw new InternalError("Invalid type in enclosing method information", cce);
				}
			}

			internal bool Partial
			{
				get
				{
					return EnclosingClass_Renamed == null || Name_Renamed == null || Descriptor_Renamed == null;
				}
			}

			internal bool Constructor
			{
				get
				{
					return !Partial && "<init>".Equals(Name_Renamed);
				}
			}

			internal bool Method
			{
				get
				{
					return !Partial && !Constructor && !"<clinit>".Equals(Name_Renamed);
				}
			}

			internal Class EnclosingClass
			{
				get
				{
					return EnclosingClass_Renamed;
				}
			}

			internal String Name
			{
				get
				{
					return Name_Renamed;
				}
			}

			internal String Descriptor
			{
				get
				{
					return Descriptor_Renamed;
				}
			}

		}

		private static Class ToClass(Type o)
		{
			if (o is GenericArrayType)
			{
				return Array.newInstance(ToClass(((GenericArrayType)o).GenericComponentType), 0).GetType();
			}
			return (Class)o;
		}

		/// <summary>
		/// If this {@code Class} object represents a local or anonymous
		/// class within a constructor, returns a {@link
		/// java.lang.reflect.Constructor Constructor} object representing
		/// the immediately enclosing constructor of the underlying
		/// class. Returns {@code null} otherwise.  In particular, this
		/// method returns {@code null} if the underlying class is a local
		/// or anonymous class immediately enclosed by a type declaration,
		/// instance initializer or static initializer.
		/// </summary>
		/// <returns> the immediately enclosing constructor of the underlying class, if
		///     that class is a local or anonymous class; otherwise {@code null}. </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and any of the
		///         following conditions is met:
		/// 
		///         <ul>
		/// 
		///         <li> the caller's class loader is not the same as the
		///         class loader of the enclosing class and invocation of
		///         {@link SecurityManager#checkPermission
		///         s.checkPermission} method with
		///         {@code RuntimePermission("accessDeclaredMembers")}
		///         denies access to the constructors within the enclosing class
		/// 
		///         <li> the caller's class loader is not the same as or an
		///         ancestor of the class loader for the enclosing class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of the enclosing class
		/// 
		///         </ul>
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Constructor<?> getEnclosingConstructor() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Constructor<?> EnclosingConstructor
		{
			get
			{
				EnclosingMethodInfo enclosingInfo = EnclosingMethodInfo;
    
				if (enclosingInfo == null)
				{
					return null;
				}
				else
				{
					if (!enclosingInfo.Constructor)
					{
						return null;
					}
    
					ConstructorRepository typeInfo = ConstructorRepository.make(enclosingInfo.Descriptor, Factory);
					Type[] parameterTypes = typeInfo.ParameterTypes;
					Class[] parameterClasses = new Class[parameterTypes.Length];
    
					// Convert Types to Classes; returned types *should*
					// be class objects since the methodDescriptor's used
					// don't have generics information
					for (int i = 0; i < parameterClasses.Length; i++)
					{
						parameterClasses[i] = ToClass(parameterTypes[i]);
					}
    
					// Perform access check
					Class enclosingCandidate = enclosingInfo.EnclosingClass;
					enclosingCandidate.CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
					/*
					 * Loop over all declared constructors; match number
					 * of and type of parameters.
					 */
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: for(Constructor<?> c: enclosingCandidate.getDeclaredConstructors())
					foreach (Constructor<?> c in enclosingCandidate.DeclaredConstructors)
					{
						Class[] candidateParamClasses = c.ParameterTypes;
						if (candidateParamClasses.Length == parameterClasses.Length)
						{
							bool matches = true;
							for (int i = 0; i < candidateParamClasses.Length; i++)
							{
								if (!candidateParamClasses[i].Equals(parameterClasses[i]))
								{
									matches = false;
									break;
								}
							}
    
							if (matches)
							{
								return c;
							}
						}
					}
    
					throw new InternalError("Enclosing constructor not found");
				}
			}
		}


		/// <summary>
		/// If the class or interface represented by this {@code Class} object
		/// is a member of another class, returns the {@code Class} object
		/// representing the class in which it was declared.  This method returns
		/// null if this class or interface is not a member of any other class.  If
		/// this {@code Class} object represents an array class, a primitive
		/// type, or void,then this method returns null.
		/// </summary>
		/// <returns> the declaring class for this class </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and the caller's
		///         class loader is not the same as or an ancestor of the class
		///         loader for the declaring class and invocation of {@link
		///         SecurityManager#checkPackageAccess s.checkPackageAccess()}
		///         denies access to the package of the declaring class
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class getDeclaringClass() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Class DeclaringClass
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Class candidate = getDeclaringClass0();
				Class candidate = DeclaringClass0;
    
				if (candidate != null)
				{
					candidate.CheckPackageAccess(ClassLoader.GetClassLoader(Reflection.CallerClass), true);
				}
				return candidate;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class getDeclaringClass0();


		/// <summary>
		/// Returns the immediately enclosing class of the underlying
		/// class.  If the underlying class is a top level class this
		/// method returns {@code null}. </summary>
		/// <returns> the immediately enclosing class of the underlying class </returns>
		/// <exception cref="SecurityException">
		///             If a security manager, <i>s</i>, is present and the caller's
		///             class loader is not the same as or an ancestor of the class
		///             loader for the enclosing class and invocation of {@link
		///             SecurityManager#checkPackageAccess s.checkPackageAccess()}
		///             denies access to the package of the enclosing class
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class getEnclosingClass() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Class EnclosingClass
		{
			get
			{
				// There are five kinds of classes (or interfaces):
				// a) Top level classes
				// b) Nested classes (static member classes)
				// c) Inner classes (non-static member classes)
				// d) Local classes (named classes declared within a method)
				// e) Anonymous classes
    
    
				// JVM Spec 4.8.6: A class must have an EnclosingMethod
				// attribute if and only if it is a local class or an
				// anonymous class.
				EnclosingMethodInfo enclosingInfo = EnclosingMethodInfo;
				Class enclosingCandidate;
    
				if (enclosingInfo == null)
				{
					// This is a top level or a nested class or an inner class (a, b, or c)
					enclosingCandidate = DeclaringClass;
				}
				else
				{
					Class enclosingClass = enclosingInfo.EnclosingClass;
					// This is a local class or an anonymous class (d or e)
					if (enclosingClass == this || enclosingClass == null)
					{
						throw new InternalError("Malformed enclosing method information");
					}
					else
					{
						enclosingCandidate = enclosingClass;
					}
				}
    
				if (enclosingCandidate != null)
				{
					enclosingCandidate.CheckPackageAccess(ClassLoader.GetClassLoader(Reflection.CallerClass), true);
				}
				return enclosingCandidate;
			}
		}

		/// <summary>
		/// Returns the simple name of the underlying class as given in the
		/// source code. Returns an empty string if the underlying class is
		/// anonymous.
		/// 
		/// <para>The simple name of an array is the simple name of the
		/// component type with "[]" appended.  In particular the simple
		/// name of an array whose component type is anonymous is "[]".
		/// 
		/// </para>
		/// </summary>
		/// <returns> the simple name of the underlying class
		/// @since 1.5 </returns>
		public String SimpleName
		{
			get
			{
				if (Array)
				{
					return ComponentType.SimpleName + "[]";
				}
    
				String simpleName = SimpleBinaryName;
				if (simpleName == null) // top level class
				{
					simpleName = Name;
					return simpleName.Substring(simpleName.LastIndexOf(".") + 1); // strip the package name
				}
				// According to JLS3 "Binary Compatibility" (13.1) the binary
				// name of non-package classes (not top level) is the binary
				// name of the immediately enclosing class followed by a '$' followed by:
				// (for nested and inner classes): the simple name.
				// (for local classes): 1 or more digits followed by the simple name.
				// (for anonymous classes): 1 or more digits.
    
				// Since getSimpleBinaryName() will strip the binary name of
				// the immediatly enclosing class, we are now looking at a
				// string that matches the regular expression "\$[0-9]*"
				// followed by a simple name (considering the simple of an
				// anonymous class to be the empty string).
    
				// Remove leading "\$[0-9]*" from the name
				int length = simpleName.Length();
				if (length < 1 || simpleName.CharAt(0) != '$')
				{
					throw new InternalError("Malformed class name");
				}
				int index = 1;
				while (index < length && IsAsciiDigit(simpleName.CharAt(index)))
				{
					index++;
				}
				// Eventually, this is the empty string iff this is an anonymous class
				return simpleName.Substring(index);
			}
		}

		/// <summary>
		/// Return an informative string for the name of this type.
		/// </summary>
		/// <returns> an informative string for the name of this type
		/// @since 1.8 </returns>
		public String TypeName
		{
			get
			{
				if (Array)
				{
					try
					{
						Class cl = this;
						int dimensions = 0;
						while (cl.Array)
						{
							dimensions++;
							cl = cl.ComponentType;
						}
						StringBuilder sb = new StringBuilder();
						sb.Append(cl.Name);
						for (int i = 0; i < dimensions; i++)
						{
							sb.Append("[]");
						}
						return sb.ToString();
					} //FALLTHRU
					catch (Throwable)
					{
					}
				}
				return Name;
			}
		}

		/// <summary>
		/// Character.isDigit answers {@code true} to some non-ascii
		/// digits.  This one does not.
		/// </summary>
		private static bool IsAsciiDigit(char c)
		{
			return '0' <= c && c <= '9';
		}

		/// <summary>
		/// Returns the canonical name of the underlying class as
		/// defined by the Java Language Specification.  Returns null if
		/// the underlying class does not have a canonical name (i.e., if
		/// it is a local or anonymous class or an array whose component
		/// type does not have a canonical name). </summary>
		/// <returns> the canonical name of the underlying class if it exists, and
		/// {@code null} otherwise.
		/// @since 1.5 </returns>
		public String CanonicalName
		{
			get
			{
				if (Array)
				{
					String canonicalName = ComponentType.CanonicalName;
					if (canonicalName != null)
					{
						return canonicalName + "[]";
					}
					else
					{
						return null;
					}
				}
				if (LocalOrAnonymousClass)
				{
					return null;
				}
				Class enclosingClass = EnclosingClass;
				if (enclosingClass == null) // top level class
				{
					return Name;
				}
				else
				{
					String enclosingName = enclosingClass.CanonicalName;
					if (enclosingName == null)
					{
						return null;
					}
					return enclosingName + "." + SimpleName;
				}
			}
		}

		/// <summary>
		/// Returns {@code true} if and only if the underlying class
		/// is an anonymous class.
		/// </summary>
		/// <returns> {@code true} if and only if this class is an anonymous class.
		/// @since 1.5 </returns>
		public bool AnonymousClass
		{
			get
			{
				return "".Equals(SimpleName);
			}
		}

		/// <summary>
		/// Returns {@code true} if and only if the underlying class
		/// is a local class.
		/// </summary>
		/// <returns> {@code true} if and only if this class is a local class.
		/// @since 1.5 </returns>
		public bool LocalClass
		{
			get
			{
				return LocalOrAnonymousClass && !AnonymousClass;
			}
		}

		/// <summary>
		/// Returns {@code true} if and only if the underlying class
		/// is a member class.
		/// </summary>
		/// <returns> {@code true} if and only if this class is a member class.
		/// @since 1.5 </returns>
		public bool MemberClass
		{
			get
			{
				return SimpleBinaryName != null && !LocalOrAnonymousClass;
			}
		}

		/// <summary>
		/// Returns the "simple binary name" of the underlying class, i.e.,
		/// the binary name without the leading enclosing class name.
		/// Returns {@code null} if the underlying class is a top level
		/// class.
		/// </summary>
		private String SimpleBinaryName
		{
			get
			{
				Class enclosingClass = EnclosingClass;
				if (enclosingClass == null) // top level class
				{
					return null;
				}
				// Otherwise, strip the enclosing class' name
				try
				{
					return Name.Substring(enclosingClass.Name.Length());
				}
				catch (IndexOutOfBoundsException ex)
				{
					throw new InternalError("Malformed class name", ex);
				}
			}
		}

		/// <summary>
		/// Returns {@code true} if this is a local class or an anonymous
		/// class.  Returns {@code false} otherwise.
		/// </summary>
		private bool LocalOrAnonymousClass
		{
			get
			{
				// JVM Spec 4.8.6: A class must have an EnclosingMethod
				// attribute if and only if it is a local class or an
				// anonymous class.
				return EnclosingMethodInfo != null;
			}
		}

		/// <summary>
		/// Returns an array containing {@code Class} objects representing all
		/// the public classes and interfaces that are members of the class
		/// represented by this {@code Class} object.  This includes public
		/// class and interface members inherited from superclasses and public class
		/// and interface members declared by the class.  This method returns an
		/// array of length 0 if this {@code Class} object has no public member
		/// classes or interfaces.  This method also returns an array of length 0 if
		/// this {@code Class} object represents a primitive type, an array
		/// class, or void.
		/// </summary>
		/// <returns> the array of {@code Class} objects representing the public
		///         members of this class </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class[] getClasses()
		public Class[] Classes
		{
			get
			{
				CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, false);
    
				// Privileged so this implementation can look at DECLARED classes,
				// something the caller might not have privilege to do.  The code here
				// is allowed to look at DECLARED classes because (1) it does not hand
				// out anything other than public members and (2) public member access
				// has already been ok'd by the SecurityManager.
    
				return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Class[]>
		{
			private readonly Class OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(Class outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Class[] Run()
			{
				IList<Class> list = new List<Class>();
				Class currentClass = OuterInstance;
				while (currentClass != null)
				{
					Class[] members = currentClass.DeclaredClasses;
					for (int i = 0; i < members.Length; i++)
					{
						if (Modifier.isPublic(members[i].Modifiers))
						{
							list.Add(members[i]);
						}
					}
					currentClass = currentClass.BaseType;
				}
				return list.ToArray();
			}
		}


		/// <summary>
		/// Returns an array containing {@code Field} objects reflecting all
		/// the accessible public fields of the class or interface represented by
		/// this {@code Class} object.
		/// 
		/// <para> If this {@code Class} object represents a class or interface with no
		/// no accessible public fields, then this method returns an array of length
		/// 0.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a class, then this method
		/// returns the public fields of the class and of all its superclasses.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an interface, then this
		/// method returns the fields of the interface and of all its
		/// superinterfaces.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, a primitive
		/// type, or void, then this method returns an array of length 0.
		/// 
		/// </para>
		/// <para> The elements in the returned array are not sorted and are not in any
		/// particular order.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the array of {@code Field} objects representing the
		///         public fields </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @since JDK1.1
		/// @jls 8.2 Class Members
		/// @jls 8.3 Field Declarations </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Field[] getFields() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Field[] Fields
		{
			get
			{
				CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
				return CopyFields(PrivateGetPublicFields(null));
			}
		}


		/// <summary>
		/// Returns an array containing {@code Method} objects reflecting all the
		/// public methods of the class or interface represented by this {@code
		/// Class} object, including those declared by the class or interface and
		/// those inherited from superclasses and superinterfaces.
		/// 
		/// <para> If this {@code Class} object represents a type that has multiple
		/// public methods with the same name and parameter types, but different
		/// return types, then the returned array has a {@code Method} object for
		/// each such method.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a type with a class
		/// initialization method {@code <clinit>}, then the returned array does
		/// <em>not</em> have a corresponding {@code Method} object.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, then the
		/// returned array has a {@code Method} object for each of the public
		/// methods inherited by the array type from {@code Object}. It does not
		/// contain a {@code Method} object for {@code clone()}.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an interface then the
		/// returned array does not contain any implicitly declared methods from
		/// {@code Object}. Therefore, if no methods are explicitly declared in
		/// this interface or any of its superinterfaces then the returned array
		/// has length 0. (Note that a {@code Class} object which represents a class
		/// always has public methods, inherited from {@code Object}.)
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a primitive type or void,
		/// then the returned array has length 0.
		/// 
		/// </para>
		/// <para> Static methods declared in superinterfaces of the class or interface
		/// represented by this {@code Class} object are not considered members of
		/// the class or interface.
		/// 
		/// </para>
		/// <para> The elements in the returned array are not sorted and are not in any
		/// particular order.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the array of {@code Method} objects representing the
		///         public methods of this class </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @jls 8.2 Class Members
		/// @jls 8.4 Method Declarations
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Method[] getMethods() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Method[] Methods
		{
			get
			{
				CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
				return CopyMethods(PrivateGetPublicMethods());
			}
		}


		/// <summary>
		/// Returns an array containing {@code Constructor} objects reflecting
		/// all the public constructors of the class represented by this
		/// {@code Class} object.  An array of length 0 is returned if the
		/// class has no public constructors, or if the class is an array class, or
		/// if the class reflects a primitive type or void.
		/// 
		/// Note that while this method returns an array of {@code
		/// Constructor<T>} objects (that is an array of constructors from
		/// this class), the return type of this method is {@code
		/// Constructor<?>[]} and <em>not</em> {@code Constructor<T>[]} as
		/// might be expected.  This less informative return type is
		/// necessary since after being returned from this method, the
		/// array could be modified to hold {@code Constructor} objects for
		/// different classes, which would violate the type guarantees of
		/// {@code Constructor<T>[]}.
		/// </summary>
		/// <returns> the array of {@code Constructor} objects representing the
		///         public constructors of this class </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Constructor<?>[] getConstructors() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Constructor<?>[] Constructors
		{
			get
			{
				CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
				return CopyConstructors(PrivateGetDeclaredConstructors(true));
			}
		}


		/// <summary>
		/// Returns a {@code Field} object that reflects the specified public member
		/// field of the class or interface represented by this {@code Class}
		/// object. The {@code name} parameter is a {@code String} specifying the
		/// simple name of the desired field.
		/// 
		/// <para> The field to be reflected is determined by the algorithm that
		/// follows.  Let C be the class or interface represented by this object:
		/// 
		/// <OL>
		/// <LI> If C declares a public field with the name specified, that is the
		///      field to be reflected.</LI>
		/// <LI> If no field was found in step 1 above, this algorithm is applied
		///      recursively to each direct superinterface of C. The direct
		///      superinterfaces are searched in the order they were declared.</LI>
		/// <LI> If no field was found in steps 1 and 2 above, and C has a
		///      superclass S, then this algorithm is invoked recursively upon S.
		///      If C has no superclass, then a {@code NoSuchFieldException}
		///      is thrown.</LI>
		/// </OL>
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, then this
		/// method does not find the {@code length} field of the array type.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the field name </param>
		/// <returns> the {@code Field} object of this class specified by
		///         {@code name} </returns>
		/// <exception cref="NoSuchFieldException"> if a field with the specified name is
		///         not found. </exception>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @since JDK1.1
		/// @jls 8.2 Class Members
		/// @jls 8.3 Field Declarations </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Field getField(String name) throws NoSuchFieldException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Field GetField(String name)
		{
			CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
			Field field = GetField0(name);
			if (field == null)
			{
				throw new NoSuchFieldException(name);
			}
			return field;
		}


		/// <summary>
		/// Returns a {@code Method} object that reflects the specified public
		/// member method of the class or interface represented by this
		/// {@code Class} object. The {@code name} parameter is a
		/// {@code String} specifying the simple name of the desired method. The
		/// {@code parameterTypes} parameter is an array of {@code Class}
		/// objects that identify the method's formal parameter types, in declared
		/// order. If {@code parameterTypes} is {@code null}, it is
		/// treated as if it were an empty array.
		/// 
		/// <para> If the {@code name} is "{@code <init>}" or "{@code <clinit>}" a
		/// {@code NoSuchMethodException} is raised. Otherwise, the method to
		/// be reflected is determined by the algorithm that follows.  Let C be the
		/// class or interface represented by this object:
		/// <OL>
		/// <LI> C is searched for a <I>matching method</I>, as defined below. If a
		///      matching method is found, it is reflected.</LI>
		/// <LI> If no matching method is found by step 1 then:
		///   <OL TYPE="a">
		///   <LI> If C is a class other than {@code Object}, then this algorithm is
		///        invoked recursively on the superclass of C.</LI>
		///   <LI> If C is the class {@code Object}, or if C is an interface, then
		///        the superinterfaces of C (if any) are searched for a matching
		///        method. If any such method is found, it is reflected.</LI>
		///   </OL></LI>
		/// </OL>
		/// 
		/// </para>
		/// <para> To find a matching method in a class or interface C:&nbsp; If C
		/// declares exactly one public method with the specified name and exactly
		/// the same formal parameter types, that is the method reflected. If more
		/// than one such method is found in C, and one of these methods has a
		/// return type that is more specific than any of the others, that method is
		/// reflected; otherwise one of the methods is chosen arbitrarily.
		/// 
		/// </para>
		/// <para>Note that there may be more than one matching method in a
		/// class because while the Java language forbids a class to
		/// declare multiple methods with the same signature but different
		/// return types, the Java virtual machine does not.  This
		/// increased flexibility in the virtual machine can be used to
		/// implement various language features.  For example, covariant
		/// returns can be implemented with {@linkplain
		/// java.lang.reflect.Method#isBridge bridge methods}; the bridge
		/// method and the method being overridden would have the same
		/// signature but different return types.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, then this
		/// method does not find the {@code clone()} method.
		/// 
		/// </para>
		/// <para> Static methods declared in superinterfaces of the class or interface
		/// represented by this {@code Class} object are not considered members of
		/// the class or interface.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the method </param>
		/// <param name="parameterTypes"> the list of parameters </param>
		/// <returns> the {@code Method} object that matches the specified
		///         {@code name} and {@code parameterTypes} </returns>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found
		///         or if the name is "&lt;init&gt;"or "&lt;clinit&gt;". </exception>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @jls 8.2 Class Members
		/// @jls 8.4 Method Declarations
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Method getMethod(String name, Class... parameterTypes) throws NoSuchMethodException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Method GetMethod(String name, params Class[] parameterTypes)
		{
			CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
			Method method = GetMethod0(name, parameterTypes, true);
			if (method == null)
			{
				throw new NoSuchMethodException(Name + "." + name + ArgumentTypesToString(parameterTypes));
			}
			return method;
		}


		/// <summary>
		/// Returns a {@code Constructor} object that reflects the specified
		/// public constructor of the class represented by this {@code Class}
		/// object. The {@code parameterTypes} parameter is an array of
		/// {@code Class} objects that identify the constructor's formal
		/// parameter types, in declared order.
		/// 
		/// If this {@code Class} object represents an inner class
		/// declared in a non-static context, the formal parameter types
		/// include the explicit enclosing instance as the first parameter.
		/// 
		/// <para> The constructor to reflect is the public constructor of the class
		/// represented by this {@code Class} object whose formal parameter
		/// types match those specified by {@code parameterTypes}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterTypes"> the parameter array </param>
		/// <returns> the {@code Constructor} object of the public constructor that
		///         matches the specified {@code parameterTypes} </returns>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and
		///         the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class.
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Constructor<T> getConstructor(Class... parameterTypes) throws NoSuchMethodException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Constructor<T> GetConstructor(params Class[] parameterTypes)
		{
			CheckMemberAccess(Member.PUBLIC, Reflection.CallerClass, true);
			return GetConstructor0(parameterTypes, Member.PUBLIC);
		}


		/// <summary>
		/// Returns an array of {@code Class} objects reflecting all the
		/// classes and interfaces declared as members of the class represented by
		/// this {@code Class} object. This includes public, protected, default
		/// (package) access, and private classes and interfaces declared by the
		/// class, but excludes inherited classes and interfaces.  This method
		/// returns an array of length 0 if the class declares no classes or
		/// interfaces as members, or if this {@code Class} object represents a
		/// primitive type, an array class, or void.
		/// </summary>
		/// <returns> the array of {@code Class} objects representing all the
		///         declared members of this class </returns>
		/// <exception cref="SecurityException">
		///         If a security manager, <i>s</i>, is present and any of the
		///         following conditions is met:
		/// 
		///         <ul>
		/// 
		///         <li> the caller's class loader is not the same as the
		///         class loader of this class and invocation of
		///         {@link SecurityManager#checkPermission
		///         s.checkPermission} method with
		///         {@code RuntimePermission("accessDeclaredMembers")}
		///         denies access to the declared classes within this class
		/// 
		///         <li> the caller's class loader is not the same as or an
		///         ancestor of the class loader for the current class and
		///         invocation of {@link SecurityManager#checkPackageAccess
		///         s.checkPackageAccess()} denies access to the package
		///         of this class
		/// 
		///         </ul>
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class[] getDeclaredClasses() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Class[] DeclaredClasses
		{
			get
			{
				CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, false);
				return DeclaredClasses0;
			}
		}


		/// <summary>
		/// Returns an array of {@code Field} objects reflecting all the fields
		/// declared by the class or interface represented by this
		/// {@code Class} object. This includes public, protected, default
		/// (package) access, and private fields, but excludes inherited fields.
		/// 
		/// <para> If this {@code Class} object represents a class or interface with no
		/// declared fields, then this method returns an array of length 0.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, a primitive
		/// type, or void, then this method returns an array of length 0.
		/// 
		/// </para>
		/// <para> The elements in the returned array are not sorted and are not in any
		/// particular order.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the array of {@code Field} objects representing all the
		///          declared fields of this class </returns>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared fields within this class
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @since JDK1.1
		/// @jls 8.2 Class Members
		/// @jls 8.3 Field Declarations </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Field[] getDeclaredFields() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Field[] DeclaredFields
		{
			get
			{
				CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
				return CopyFields(PrivateGetDeclaredFields(false));
			}
		}


		/// 
		/// <summary>
		/// Returns an array containing {@code Method} objects reflecting all the
		/// declared methods of the class or interface represented by this {@code
		/// Class} object, including public, protected, default (package)
		/// access, and private methods, but excluding inherited methods.
		/// 
		/// <para> If this {@code Class} object represents a type that has multiple
		/// declared methods with the same name and parameter types, but different
		/// return types, then the returned array has a {@code Method} object for
		/// each such method.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a type that has a class
		/// initialization method {@code <clinit>}, then the returned array does
		/// <em>not</em> have a corresponding {@code Method} object.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a class or interface with no
		/// declared methods, then the returned array has length 0.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an array type, a primitive
		/// type, or void, then the returned array has length 0.
		/// 
		/// </para>
		/// <para> The elements in the returned array are not sorted and are not in any
		/// particular order.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the array of {@code Method} objects representing all the
		///          declared methods of this class </returns>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared methods within this class
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @jls 8.2 Class Members
		/// @jls 8.4 Method Declarations
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Method[] getDeclaredMethods() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Method[] DeclaredMethods
		{
			get
			{
				CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
				return CopyMethods(PrivateGetDeclaredMethods(false));
			}
		}


		/// <summary>
		/// Returns an array of {@code Constructor} objects reflecting all the
		/// constructors declared by the class represented by this
		/// {@code Class} object. These are public, protected, default
		/// (package) access, and private constructors.  The elements in the array
		/// returned are not sorted and are not in any particular order.  If the
		/// class has a default constructor, it is included in the returned array.
		/// This method returns an array of length 0 if this {@code Class}
		/// object represents an interface, a primitive type, an array class, or
		/// void.
		/// 
		/// <para> See <em>The Java Language Specification</em>, section 8.2.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the array of {@code Constructor} objects representing all the
		///          declared constructors of this class </returns>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared constructors within this class
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Constructor<?>[] getDeclaredConstructors() throws SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Constructor<?>[] DeclaredConstructors
		{
			get
			{
				CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
				return CopyConstructors(PrivateGetDeclaredConstructors(false));
			}
		}


		/// <summary>
		/// Returns a {@code Field} object that reflects the specified declared
		/// field of the class or interface represented by this {@code Class}
		/// object. The {@code name} parameter is a {@code String} that specifies
		/// the simple name of the desired field.
		/// 
		/// <para> If this {@code Class} object represents an array type, then this
		/// method does not find the {@code length} field of the array type.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the field </param>
		/// <returns>  the {@code Field} object for the specified field in this
		///          class </returns>
		/// <exception cref="NoSuchFieldException"> if a field with the specified name is
		///          not found. </exception>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared field
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @since JDK1.1
		/// @jls 8.2 Class Members
		/// @jls 8.3 Field Declarations </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Field getDeclaredField(String name) throws NoSuchFieldException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Field GetDeclaredField(String name)
		{
			CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
			Field field = SearchFields(PrivateGetDeclaredFields(false), name);
			if (field == null)
			{
				throw new NoSuchFieldException(name);
			}
			return field;
		}


		/// <summary>
		/// Returns a {@code Method} object that reflects the specified
		/// declared method of the class or interface represented by this
		/// {@code Class} object. The {@code name} parameter is a
		/// {@code String} that specifies the simple name of the desired
		/// method, and the {@code parameterTypes} parameter is an array of
		/// {@code Class} objects that identify the method's formal parameter
		/// types, in declared order.  If more than one method with the same
		/// parameter types is declared in a class, and one of these methods has a
		/// return type that is more specific than any of the others, that method is
		/// returned; otherwise one of the methods is chosen arbitrarily.  If the
		/// name is "&lt;init&gt;"or "&lt;clinit&gt;" a {@code NoSuchMethodException}
		/// is raised.
		/// 
		/// <para> If this {@code Class} object represents an array type, then this
		/// method does not find the {@code clone()} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the method </param>
		/// <param name="parameterTypes"> the parameter array </param>
		/// <returns>  the {@code Method} object for the method of this class
		///          matching the specified name and parameters </returns>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
		/// <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared method
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @jls 8.2 Class Members
		/// @jls 8.4 Method Declarations
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Method getDeclaredMethod(String name, Class... parameterTypes) throws NoSuchMethodException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Method GetDeclaredMethod(String name, params Class[] parameterTypes)
		{
			CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
			Method method = SearchMethods(PrivateGetDeclaredMethods(false), name, parameterTypes);
			if (method == null)
			{
				throw new NoSuchMethodException(Name + "." + name + ArgumentTypesToString(parameterTypes));
			}
			return method;
		}


		/// <summary>
		/// Returns a {@code Constructor} object that reflects the specified
		/// constructor of the class or interface represented by this
		/// {@code Class} object.  The {@code parameterTypes} parameter is
		/// an array of {@code Class} objects that identify the constructor's
		/// formal parameter types, in declared order.
		/// 
		/// If this {@code Class} object represents an inner class
		/// declared in a non-static context, the formal parameter types
		/// include the explicit enclosing instance as the first parameter.
		/// </summary>
		/// <param name="parameterTypes"> the parameter array </param>
		/// <returns>  The {@code Constructor} object for the constructor with the
		///          specified parameter list </returns>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
		/// <exception cref="SecurityException">
		///          If a security manager, <i>s</i>, is present and any of the
		///          following conditions is met:
		/// 
		///          <ul>
		/// 
		///          <li> the caller's class loader is not the same as the
		///          class loader of this class and invocation of
		///          {@link SecurityManager#checkPermission
		///          s.checkPermission} method with
		///          {@code RuntimePermission("accessDeclaredMembers")}
		///          denies access to the declared constructor
		/// 
		///          <li> the caller's class loader is not the same as or an
		///          ancestor of the class loader for the current class and
		///          invocation of {@link SecurityManager#checkPackageAccess
		///          s.checkPackageAccess()} denies access to the package
		///          of this class
		/// 
		///          </ul>
		/// 
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Constructor<T> getDeclaredConstructor(Class... parameterTypes) throws NoSuchMethodException, SecurityException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Constructor<T> GetDeclaredConstructor(params Class[] parameterTypes)
		{
			CheckMemberAccess(Member.DECLARED, Reflection.CallerClass, true);
			return GetConstructor0(parameterTypes, Member.DECLARED);
		}

		/// <summary>
		/// Finds a resource with a given name.  The rules for searching resources
		/// associated with a given class are implemented by the defining
		/// <seealso cref="ClassLoader class loader"/> of the class.  This method
		/// delegates to this object's class loader.  If this object was loaded by
		/// the bootstrap class loader, the method delegates to {@link
		/// ClassLoader#getSystemResourceAsStream}.
		/// 
		/// <para> Before delegation, an absolute resource name is constructed from the
		/// given resource name using this algorithm:
		/// 
		/// <ul>
		/// 
		/// <li> If the {@code name} begins with a {@code '/'}
		/// (<tt>'&#92;u002f'</tt>), then the absolute name of the resource is the
		/// portion of the {@code name} following the {@code '/'}.
		/// 
		/// <li> Otherwise, the absolute name is of the following form:
		/// 
		/// <blockquote>
		///   {@code modified_package_name/name}
		/// </blockquote>
		/// 
		/// </para>
		/// <para> Where the {@code modified_package_name} is the package name of this
		/// object with {@code '/'} substituted for {@code '.'}
		/// (<tt>'&#92;u002e'</tt>).
		/// 
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> name of the desired resource </param>
		/// <returns>      A <seealso cref="java.io.InputStream"/> object or {@code null} if
		///              no resource with this name is found </returns>
		/// <exception cref="NullPointerException"> If {@code name} is {@code null}
		/// @since  JDK1.1 </exception>
		 public InputStream GetResourceAsStream(String name)
		 {
			name = ResolveName(name);
			ClassLoader cl = ClassLoader0;
			if (cl == null)
			{
				// A system class.
				return ClassLoader.GetSystemResourceAsStream(name);
			}
			return cl.GetResourceAsStream(name);
		 }

		/// <summary>
		/// Finds a resource with a given name.  The rules for searching resources
		/// associated with a given class are implemented by the defining
		/// <seealso cref="ClassLoader class loader"/> of the class.  This method
		/// delegates to this object's class loader.  If this object was loaded by
		/// the bootstrap class loader, the method delegates to {@link
		/// ClassLoader#getSystemResource}.
		/// 
		/// <para> Before delegation, an absolute resource name is constructed from the
		/// given resource name using this algorithm:
		/// 
		/// <ul>
		/// 
		/// <li> If the {@code name} begins with a {@code '/'}
		/// (<tt>'&#92;u002f'</tt>), then the absolute name of the resource is the
		/// portion of the {@code name} following the {@code '/'}.
		/// 
		/// <li> Otherwise, the absolute name is of the following form:
		/// 
		/// <blockquote>
		///   {@code modified_package_name/name}
		/// </blockquote>
		/// 
		/// </para>
		/// <para> Where the {@code modified_package_name} is the package name of this
		/// object with {@code '/'} substituted for {@code '.'}
		/// (<tt>'&#92;u002e'</tt>).
		/// 
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> name of the desired resource </param>
		/// <returns>      A  <seealso cref="java.net.URL"/> object or {@code null} if no
		///              resource with this name is found
		/// @since  JDK1.1 </returns>
		public java.net.URL GetResource(String name)
		{
			name = ResolveName(name);
			ClassLoader cl = ClassLoader0;
			if (cl == null)
			{
				// A system class.
				return ClassLoader.GetSystemResource(name);
			}
			return cl.GetResource(name);
		}



		/// <summary>
		/// protection domain returned when the internal domain is null </summary>
		private static java.security.ProtectionDomain AllPermDomain;


		/// <summary>
		/// Returns the {@code ProtectionDomain} of this class.  If there is a
		/// security manager installed, this method first calls the security
		/// manager's {@code checkPermission} method with a
		/// {@code RuntimePermission("getProtectionDomain")} permission to
		/// ensure it's ok to get the
		/// {@code ProtectionDomain}.
		/// </summary>
		/// <returns> the ProtectionDomain of this class
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        getting the ProtectionDomain.
		/// </exception>
		/// <seealso cref= java.security.ProtectionDomain </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.lang.RuntimePermission
		/// @since 1.2 </seealso>
		public java.security.ProtectionDomain ProtectionDomain
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(SecurityConstants.GET_PD_PERMISSION);
				}
				java.security.ProtectionDomain pd = ProtectionDomain0;
				if (pd == null)
				{
					if (AllPermDomain == null)
					{
						java.security.Permissions perms = new java.security.Permissions();
						perms.Add(SecurityConstants.ALL_PERMISSION);
						AllPermDomain = new java.security.ProtectionDomain(null, perms);
					}
					pd = AllPermDomain;
				}
				return pd;
			}
		}


		/// <summary>
		/// Returns the ProtectionDomain of this class.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern java.security.ProtectionDomain getProtectionDomain0();

		/*
		 * Return the Virtual Machine's Class object for the named
		 * primitive type.
		 */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern Class getPrimitiveClass(String name);

		/*
		 * Check if client is allowed to access members.  If access is denied,
		 * throw a SecurityException.
		 *
		 * This method also enforces package access.
		 *
		 * <p> Default policy: allow all clients access with normal Java access
		 * control.
		 */
		private void CheckMemberAccess(int which, Class caller, bool checkProxyInterfaces)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecurityManager s = System.getSecurityManager();
			SecurityManager s = System.SecurityManager;
			if (s != null)
			{
				/* Default policy allows access to all {@link Member#PUBLIC} members,
				 * as well as access to classes that have the same class loader as the caller.
				 * In all other cases, it requires RuntimePermission("accessDeclaredMembers")
				 * permission.
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader ccl = ClassLoader.getClassLoader(caller);
				ClassLoader ccl = ClassLoader.GetClassLoader(caller);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader cl = getClassLoader0();
				ClassLoader cl = ClassLoader0;
				if (which != Member.PUBLIC)
				{
					if (ccl != cl)
					{
						s.CheckPermission(SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION);
					}
				}
				this.CheckPackageAccess(ccl, checkProxyInterfaces);
			}
		}

		/*
		 * Checks if a client loaded in ClassLoader ccl is allowed to access this
		 * class under the current package access policy. If access is denied,
		 * throw a SecurityException.
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void checkPackageAccess(final ClassLoader ccl, boolean checkProxyInterfaces)
		private void CheckPackageAccess(ClassLoader ccl, bool checkProxyInterfaces)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecurityManager s = System.getSecurityManager();
			SecurityManager s = System.SecurityManager;
			if (s != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader cl = getClassLoader0();
				ClassLoader cl = ClassLoader0;

				if (ReflectUtil.needsPackageAccessCheck(ccl, cl))
				{
					String name = this.Name;
					int i = name.LastIndexOf('.');
					if (i != -1)
					{
						// skip the package access check on a proxy class in default proxy package
						String pkg = name.Substring(0, i);
						if (!Proxy.isProxyClass(this) || ReflectUtil.isNonPublicProxyClass(this))
						{
							s.CheckPackageAccess(pkg);
						}
					}
				}
				// check package access on the proxy interfaces
				if (checkProxyInterfaces && Proxy.isProxyClass(this))
				{
					ReflectUtil.checkProxyPackageAccess(ccl, this.Interfaces);
				}
			}
		}

		/// <summary>
		/// Add a package name prefix if the name is not absolute Remove leading "/"
		/// if name is absolute
		/// </summary>
		private String ResolveName(String name)
		{
			if (name == null)
			{
				return name;
			}
			if (!name.StartsWith("/"))
			{
				Class c = this;
				while (c.Array)
				{
					c = c.ComponentType;
				}
				String baseName = c.Name;
				int index = baseName.LastIndexOf('.');
				if (index != -1)
				{
					name = baseName.Substring(0, index).Replace('.', '/') + "/" + name;
				}
			}
			else
			{
				name = name.Substring(1);
			}
			return name;
		}

		/// <summary>
		/// Atomic operations support.
		/// </summary>
		private class Atomic
		{
			// initialize Unsafe machinery here, since we need to call Class.class instance method
			// and have to avoid calling it in the static initializer of the Class class...
			internal static readonly Unsafe @unsafe = Unsafe.Unsafe;
			// offset of Class.reflectionData instance field
			internal static readonly long ReflectionDataOffset;
			// offset of Class.annotationType instance field
			internal static readonly long AnnotationTypeOffset;
			// offset of Class.annotationData instance field
			internal static readonly long AnnotationDataOffset;


			internal static long ObjectFieldOffset(Field[] fields, String fieldName)
			{
				Field field = SearchFields(fields, fieldName);
				if (field == null)
				{
					throw new Error("No " + fieldName + " field found in java.lang.Class");
				}
				return @unsafe.objectFieldOffset(field);
			}

			internal static bool casReflectionData<T>(Class clazz, SoftReference<ReflectionData<T>> oldData, SoftReference<ReflectionData<T>> newData)
			{
				return @unsafe.compareAndSwapObject(clazz, ReflectionDataOffset, oldData, newData);
			}

			internal static bool casAnnotationType<T>(Class clazz, AnnotationType oldType, AnnotationType newType)
			{
				return @unsafe.compareAndSwapObject(clazz, AnnotationTypeOffset, oldType, newType);
			}

			internal static bool casAnnotationData<T>(Class clazz, AnnotationData oldData, AnnotationData newData)
			{
				return @unsafe.compareAndSwapObject(clazz, AnnotationDataOffset, oldData, newData);
			}
		}

		/// <summary>
		/// Reflection support.
		/// </summary>

		// Caches for certain reflective results
		private static bool UseCaches = true;

		// reflection data that might get invalidated when JVM TI RedefineClasses() is called
		private class ReflectionData<T>
		{
			internal volatile Field[] DeclaredFields;
			internal volatile Field[] PublicFields;
			internal volatile Method[] DeclaredMethods;
			internal volatile Method[] PublicMethods;
			internal volatile Constructor<T>[] DeclaredConstructors;
			internal volatile Constructor<T>[] PublicConstructors;
			// Intermediate results for getFields and getMethods
			internal volatile Field[] DeclaredPublicFields;
			internal volatile Method[] DeclaredPublicMethods;
			internal volatile Class[] Interfaces;

			// Value of classRedefinedCount when we created this ReflectionData instance
			internal readonly int RedefinedCount;

			internal ReflectionData(int redefinedCount)
			{
				this.RedefinedCount = redefinedCount;
			}
		}

		[NonSerialized]
		private volatile SoftReference<ReflectionData<T>> ReflectionData_Renamed;

		// Incremented by the VM on each call to JVM TI RedefineClasses()
		// that redefines this class or a superclass.
		[NonSerialized]
		private volatile int ClassRedefinedCount = 0;

		// Lazily create and cache ReflectionData
		private ReflectionData<T> ReflectionData()
		{
			SoftReference<ReflectionData<T>> reflectionData = this.ReflectionData_Renamed;
			int classRedefinedCount = this.ClassRedefinedCount;
			ReflectionData<T> rd;
			if (UseCaches && reflectionData != null && (rd = reflectionData.get()) != null && rd.RedefinedCount == classRedefinedCount)
			{
				return rd;
			}
			// else no SoftReference or cleared SoftReference or stale ReflectionData
			// -> create and replace new instance
			return NewReflectionData(reflectionData, classRedefinedCount);
		}

		private ReflectionData<T> NewReflectionData(SoftReference<ReflectionData<T>> oldReflectionData, int classRedefinedCount)
		{
			if (!UseCaches)
			{
				return null;
			}

			while (true)
			{
				ReflectionData<T> rd = new ReflectionData<T>(classRedefinedCount);
				// try to CAS it...
				if (Atomic.CasReflectionData(this, oldReflectionData, new SoftReference<>(rd)))
				{
					return rd;
				}
				// else retry
				oldReflectionData = this.ReflectionData_Renamed;
				classRedefinedCount = this.ClassRedefinedCount;
				if (oldReflectionData != null && (rd = oldReflectionData.get()) != null && rd.RedefinedCount == classRedefinedCount)
				{
					return rd;
				}
			}
		}

		// Generic signature handling
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern String getGenericSignature0();

		// Generic info repository; lazily initialized
		[NonSerialized]
		private volatile ClassRepository GenericInfo_Renamed;

		// accessor for factory
		private GenericsFactory Factory
		{
			get
			{
				// create scope and factory
				return CoreReflectionFactory.make(this, ClassScope.make(this));
			}
		}

		// accessor for generic info repository;
		// generic info is lazily initialized
		private ClassRepository GenericInfo
		{
			get
			{
				ClassRepository genericInfo = this.GenericInfo_Renamed;
				if (genericInfo == null)
				{
					String signature = GenericSignature0;
					if (signature == null)
					{
						genericInfo = ClassRepository.NONE;
					}
					else
					{
						genericInfo = ClassRepository.make(signature, Factory);
					}
					this.GenericInfo_Renamed = genericInfo;
				}
				return (genericInfo != ClassRepository.NONE) ? genericInfo : null;
			}
		}

		// Annotations handling
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern byte[] getRawAnnotations();
		// Since 1.8
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern byte[] getRawTypeAnnotations();
		internal static sbyte[] GetExecutableTypeAnnotationBytes(Executable ex)
		{
			return ReflectionFactory.getExecutableTypeAnnotationBytes(ex);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern sun.reflect.ConstantPool getConstantPool();

		//
		//
		// java.lang.reflect.Field handling
		//
		//

		// Returns an array of "root" fields. These Field objects must NOT
		// be propagated to the outside world, but must instead be copied
		// via ReflectionFactory.copyField.
		private Field[] PrivateGetDeclaredFields(bool publicOnly)
		{
			CheckInitted();
			Field[] res;
			ReflectionData<T> rd = ReflectionData();
			if (rd != null)
			{
				res = publicOnly ? rd.DeclaredPublicFields : rd.DeclaredFields;
				if (res != null)
				{
					return res;
				}
			}
			// No cached value available; request value from VM
			res = Reflection.filterFields(this, getDeclaredFields0(publicOnly));
			if (rd != null)
			{
				if (publicOnly)
				{
					rd.DeclaredPublicFields = res;
				}
				else
				{
					rd.DeclaredFields = res;
				}
			}
			return res;
		}

		// Returns an array of "root" fields. These Field objects must NOT
		// be propagated to the outside world, but must instead be copied
		// via ReflectionFactory.copyField.
		private Field[] PrivateGetPublicFields(Set<Class> traversedInterfaces)
		{
			CheckInitted();
			Field[] res;
			ReflectionData<T> rd = ReflectionData();
			if (rd != null)
			{
				res = rd.PublicFields;
				if (res != null)
				{
					return res;
				}
			}

			// No cached value available; compute value recursively.
			// Traverse in correct order for getField().
			IList<Field> fields = new List<Field>();
			if (traversedInterfaces == null)
			{
				traversedInterfaces = new HashSet<>();
			}

			// Local fields
			Field[] tmp = PrivateGetDeclaredFields(true);
			AddAll(fields, tmp);

			// Direct superinterfaces, recursively
			foreach (Class c in Interfaces)
			{
				if (!traversedInterfaces.Contains(c))
				{
					traversedInterfaces.Add(c);
					AddAll(fields, c.PrivateGetPublicFields(traversedInterfaces));
				}
			}

			// Direct superclass, recursively
			if (!Interface)
			{
				Class c = Superclass;
				if (c != null)
				{
					AddAll(fields, c.PrivateGetPublicFields(traversedInterfaces));
				}
			}

			res = new Field[fields.Count];
			fields.toArray(res);
			if (rd != null)
			{
				rd.PublicFields = res;
			}
			return res;
		}

		private static void AddAll(ICollection<Field> c, Field[] o)
		{
			for (int i = 0; i < o.Length; i++)
			{
				c.Add(o[i]);
			}
		}


		//
		//
		// java.lang.reflect.Constructor handling
		//
		//

		// Returns an array of "root" constructors. These Constructor
		// objects must NOT be propagated to the outside world, but must
		// instead be copied via ReflectionFactory.copyConstructor.
		private Constructor<T>[] PrivateGetDeclaredConstructors(bool publicOnly)
		{
			CheckInitted();
			Constructor<T>[] res;
			ReflectionData<T> rd = ReflectionData();
			if (rd != null)
			{
				res = publicOnly ? rd.PublicConstructors : rd.DeclaredConstructors;
				if (res != null)
				{
					return res;
				}
			}
			// No cached value available; request value from VM
			if (Interface)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Constructor<T>[] temporaryRes = (Constructor<T>[]) new Constructor<?>[0];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Constructor<T>[] temporaryRes = (Constructor<T>[]) new Constructor<?>[0];
				res = temporaryRes;
			}
			else
			{
				res = getDeclaredConstructors0(publicOnly);
			}
			if (rd != null)
			{
				if (publicOnly)
				{
					rd.PublicConstructors = res;
				}
				else
				{
					rd.DeclaredConstructors = res;
				}
			}
			return res;
		}

		//
		//
		// java.lang.reflect.Method handling
		//
		//

		// Returns an array of "root" methods. These Method objects must NOT
		// be propagated to the outside world, but must instead be copied
		// via ReflectionFactory.copyMethod.
		private Method[] PrivateGetDeclaredMethods(bool publicOnly)
		{
			CheckInitted();
			Method[] res;
			ReflectionData<T> rd = ReflectionData();
			if (rd != null)
			{
				res = publicOnly ? rd.DeclaredPublicMethods : rd.DeclaredMethods;
				if (res != null)
				{
					return res;
				}
			}
			// No cached value available; request value from VM
			res = Reflection.filterMethods(this, getDeclaredMethods0(publicOnly));
			if (rd != null)
			{
				if (publicOnly)
				{
					rd.DeclaredPublicMethods = res;
				}
				else
				{
					rd.DeclaredMethods = res;
				}
			}
			return res;
		}

		internal class MethodArray
		{
			// Don't add or remove methods except by add() or remove() calls.
			internal Method[] Methods;
			internal int Length_Renamed;
			internal int Defaults;

			internal MethodArray() : this(20)
			{
			}

			internal MethodArray(int initialSize)
			{
				if (initialSize < 2)
				{
					throw new IllegalArgumentException("Size should be 2 or more");
				}

				Methods = new Method[initialSize];
				Length_Renamed = 0;
				Defaults = 0;
			}

			internal virtual bool HasDefaults()
			{
				return Defaults != 0;
			}

			internal virtual void Add(Method m)
			{
				if (Length_Renamed == Methods.Length)
				{
					Methods = Arrays.CopyOf(Methods, 2 * Methods.Length);
				}
				Methods[Length_Renamed++] = m;

				if (m != null && m.Default)
				{
					Defaults++;
				}
			}

			internal virtual void AddAll(Method[] ma)
			{
				for (int i = 0; i < ma.Length; i++)
				{
					Add(ma[i]);
				}
			}

			internal virtual void AddAll(MethodArray ma)
			{
				for (int i = 0; i < ma.Length(); i++)
				{
					Add(ma.Get(i));
				}
			}

			internal virtual void AddIfNotPresent(Method newMethod)
			{
				for (int i = 0; i < Length_Renamed; i++)
				{
					Method m = Methods[i];
					if (m == newMethod || (m != null && m.Equals(newMethod)))
					{
						return;
					}
				}
				Add(newMethod);
			}

			internal virtual void AddAllIfNotPresent(MethodArray newMethods)
			{
				for (int i = 0; i < newMethods.Length(); i++)
				{
					Method m = newMethods.Get(i);
					if (m != null)
					{
						AddIfNotPresent(m);
					}
				}
			}

			/* Add Methods declared in an interface to this MethodArray.
			 * Static methods declared in interfaces are not inherited.
			 */
			internal virtual void AddInterfaceMethods(Method[] methods)
			{
				foreach (Method candidate in methods)
				{
					if (!Modifier.isStatic(candidate.Modifiers))
					{
						Add(candidate);
					}
				}
			}

			internal virtual int Length()
			{
				return Length_Renamed;
			}

			internal virtual Method Get(int i)
			{
				return Methods[i];
			}

			internal virtual Method First
			{
				get
				{
					foreach (Method m in Methods)
					{
						if (m != null)
						{
							return m;
						}
					}
					return null;
				}
			}

			internal virtual void RemoveByNameAndDescriptor(Method toRemove)
			{
				for (int i = 0; i < Length_Renamed; i++)
				{
					Method m = Methods[i];
					if (m != null && MatchesNameAndDescriptor(m, toRemove))
					{
						Remove(i);
					}
				}
			}

			internal virtual void Remove(int i)
			{
				if (Methods[i] != null && Methods[i].Default)
				{
					Defaults--;
				}
				Methods[i] = null;
			}

			internal virtual bool MatchesNameAndDescriptor(Method m1, Method m2)
			{
				return m1.ReturnType == m2.ReturnType && m1.Name == m2.Name && ArrayContentsEq(m1.ParameterTypes, m2.ParameterTypes); // name is guaranteed to be interned
			}

			internal virtual void CompactAndTrim()
			{
				int newPos = 0;
				// Get rid of null slots
				for (int pos = 0; pos < Length_Renamed; pos++)
				{
					Method m = Methods[pos];
					if (m != null)
					{
						if (pos != newPos)
						{
							Methods[newPos] = m;
						}
						newPos++;
					}
				}
				if (newPos != Methods.Length)
				{
					Methods = Arrays.CopyOf(Methods, newPos);
				}
			}

			/* Removes all Methods from this MethodArray that have a more specific
			 * default Method in this MethodArray.
			 *
			 * Users of MethodArray are responsible for pruning Methods that have
			 * a more specific <em>concrete</em> Method.
			 */
			internal virtual void RemoveLessSpecifics()
			{
				if (!HasDefaults())
				{
					return;
				}

				for (int i = 0; i < Length_Renamed; i++)
				{
					Method m = Get(i);
					if (m == null || !m.Default)
					{
						continue;
					}

					for (int j = 0; j < Length_Renamed; j++)
					{
						if (i == j)
						{
							continue;
						}

						Method candidate = Get(j);
						if (candidate == null)
						{
							continue;
						}

						if (!MatchesNameAndDescriptor(m, candidate))
						{
							continue;
						}

						if (HasMoreSpecificClass(m, candidate))
						{
							Remove(j);
						}
					}
				}
			}

			internal virtual Method[] Array
			{
				get
				{
					return Methods;
				}
			}

			// Returns true if m1 is more specific than m2
			internal static bool HasMoreSpecificClass(Method m1, Method m2)
			{
				Class m1Class = m1.DeclaringClass;
				Class m2Class = m2.DeclaringClass;
				return m1Class != m2Class && m1Class.IsSubclassOf(m2Class);
			}
		}


		// Returns an array of "root" methods. These Method objects must NOT
		// be propagated to the outside world, but must instead be copied
		// via ReflectionFactory.copyMethod.
		private Method[] PrivateGetPublicMethods()
		{
			CheckInitted();
			Method[] res;
			ReflectionData<T> rd = ReflectionData();
			if (rd != null)
			{
				res = rd.PublicMethods;
				if (res != null)
				{
					return res;
				}
			}

			// No cached value available; compute value recursively.
			// Start by fetching public declared methods
			MethodArray methods = new MethodArray();
			{
				Method[] tmp = PrivateGetDeclaredMethods(true);
				methods.AddAll(tmp);
			}
			// Now recur over superclass and direct superinterfaces.
			// Go over superinterfaces first so we can more easily filter
			// out concrete implementations inherited from superclasses at
			// the end.
			MethodArray inheritedMethods = new MethodArray();
			foreach (Class i in Interfaces)
			{
				inheritedMethods.AddInterfaceMethods(i.PrivateGetPublicMethods());
			}
			if (!Interface)
			{
				Class c = Superclass;
				if (c != null)
				{
					MethodArray supers = new MethodArray();
					supers.AddAll(c.PrivateGetPublicMethods());
					// Filter out concrete implementations of any
					// interface methods
					for (int i = 0; i < supers.Length(); i++)
					{
						Method m = supers.Get(i);
						if (m != null && !Modifier.isAbstract(m.Modifiers) && !m.Default)
						{
							inheritedMethods.RemoveByNameAndDescriptor(m);
						}
					}
					// Insert superclass's inherited methods before
					// superinterfaces' to satisfy getMethod's search
					// order
					supers.AddAll(inheritedMethods);
					inheritedMethods = supers;
				}
			}
			// Filter out all local methods from inherited ones
			for (int i = 0; i < methods.Length(); i++)
			{
				Method m = methods.Get(i);
				inheritedMethods.RemoveByNameAndDescriptor(m);
			}
			methods.AddAllIfNotPresent(inheritedMethods);
			methods.RemoveLessSpecifics();
			methods.CompactAndTrim();
			res = methods.Array;
			if (rd != null)
			{
				rd.PublicMethods = res;
			}
			return res;
		}


		//
		// Helpers for fetchers of one field, method, or constructor
		//

		private static Field SearchFields(Field[] fields, String name)
		{
			String internedName = name.intern();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].Name == internedName)
				{
					return ReflectionFactory.copyField(fields[i]);
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Field getField0(String name) throws NoSuchFieldException
		private Field GetField0(String name)
		{
			// Note: the intent is that the search algorithm this routine
			// uses be equivalent to the ordering imposed by
			// privateGetPublicFields(). It fetches only the declared
			// public fields for each class, however, to reduce the number
			// of Field objects which have to be created for the common
			// case where the field being requested is declared in the
			// class which is being queried.
			Field res;
			// Search declared public fields
			if ((res = SearchFields(PrivateGetDeclaredFields(true), name)) != null)
			{
				return res;
			}
			// Direct superinterfaces, recursively
			Class[] interfaces = Interfaces;
			for (int i = 0; i < interfaces.Length; i++)
			{
				Class c = interfaces[i];
				if ((res = c.GetField0(name)) != null)
				{
					return res;
				}
			}
			// Direct superclass, recursively
			if (!Interface)
			{
				Class c = Superclass;
				if (c != null)
				{
					if ((res = c.GetField0(name)) != null)
					{
						return res;
					}
				}
			}
			return null;
		}

		private static Method SearchMethods(Method[] methods, String name, Class[] parameterTypes)
		{
			Method res = null;
			String internedName = name.intern();
			for (int i = 0; i < methods.Length; i++)
			{
				Method m = methods[i];
				if (m.Name == internedName && ArrayContentsEq(parameterTypes, m.ParameterTypes) && (res == null || m.ReturnType.IsSubclassOf(res.ReturnType)))
				{
					res = m;
				}
			}

			return (res == null ? res : ReflectionFactory.copyMethod(res));
		}

		private Method GetMethod0(String name, Class[] parameterTypes, bool includeStaticMethods)
		{
			MethodArray interfaceCandidates = new MethodArray(2);
			Method res = PrivateGetMethodRecursive(name, parameterTypes, includeStaticMethods, interfaceCandidates);
			if (res != null)
			{
				return res;
			}

			// Not found on class or superclass directly
			interfaceCandidates.RemoveLessSpecifics();
			return interfaceCandidates.First; // may be null
		}

		private Method PrivateGetMethodRecursive(String name, Class[] parameterTypes, bool includeStaticMethods, MethodArray allInterfaceCandidates)
		{
			// Note: the intent is that the search algorithm this routine
			// uses be equivalent to the ordering imposed by
			// privateGetPublicMethods(). It fetches only the declared
			// public methods for each class, however, to reduce the
			// number of Method objects which have to be created for the
			// common case where the method being requested is declared in
			// the class which is being queried.
			//
			// Due to default methods, unless a method is found on a superclass,
			// methods declared in any superinterface needs to be considered.
			// Collect all candidates declared in superinterfaces in {@code
			// allInterfaceCandidates} and select the most specific if no match on
			// a superclass is found.

			// Must _not_ return root methods
			Method res;
			// Search declared public methods
			if ((res = SearchMethods(PrivateGetDeclaredMethods(true), name, parameterTypes)) != null)
			{
				if (includeStaticMethods || !Modifier.isStatic(res.Modifiers))
				{
					return res;
				}
			}
			// Search superclass's methods
			if (!Interface)
			{
				Class c = Superclass;
				if (c != null)
				{
					if ((res = c.GetMethod0(name, parameterTypes, true)) != null)
					{
						return res;
					}
				}
			}
			// Search superinterfaces' methods
			Class[] interfaces = Interfaces;
			foreach (Class c in interfaces)
			{
				if ((res = c.GetMethod0(name, parameterTypes, false)) != null)
				{
					allInterfaceCandidates.Add(res);
				}
			}
			// Not found
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Constructor<T> getConstructor0(Class[] parameterTypes, int which) throws NoSuchMethodException
		private Constructor<T> GetConstructor0(Class[] parameterTypes, int which)
		{
			Constructor<T>[] constructors = PrivateGetDeclaredConstructors((which == Member.PUBLIC));
			foreach (Constructor<T> constructor in constructors)
			{
				if (ArrayContentsEq(parameterTypes, constructor.ParameterTypes))
				{
					return ReflectionFactory.copyConstructor(constructor);
				}
			}
			throw new NoSuchMethodException(Name + ".<init>" + ArgumentTypesToString(parameterTypes));
		}

		//
		// Other helpers and base implementation
		//

		private static bool ArrayContentsEq(Object[] a1, Object[] a2)
		{
			if (a1 == null)
			{
				return a2 == null || a2.Length == 0;
			}

			if (a2 == null)
			{
				return a1.Length == 0;
			}

			if (a1.Length != a2.Length)
			{
				return false;
			}

			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}

			return true;
		}

		private static Field[] CopyFields(Field[] arg)
		{
			Field[] @out = new Field[arg.Length];
			ReflectionFactory fact = ReflectionFactory;
			for (int i = 0; i < arg.Length; i++)
			{
				@out[i] = fact.copyField(arg[i]);
			}
			return @out;
		}

		private static Method[] CopyMethods(Method[] arg)
		{
			Method[] @out = new Method[arg.Length];
			ReflectionFactory fact = ReflectionFactory;
			for (int i = 0; i < arg.Length; i++)
			{
				@out[i] = fact.copyMethod(arg[i]);
			}
			return @out;
		}

		private static Constructor<U>[] copyConstructors<U>(Constructor<U>[] arg)
		{
			Constructor<U>[] @out = arg.clone();
			ReflectionFactory fact = ReflectionFactory;
			for (int i = 0; i < @out.Length; i++)
			{
				@out[i] = fact.copyConstructor(@out[i]);
			}
			return @out;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Field[] getDeclaredFields0(bool publicOnly);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Method[] getDeclaredMethods0(bool publicOnly);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Constructor<T>[] getDeclaredConstructors0(bool publicOnly);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Class[] getDeclaredClasses0();

		private static String ArgumentTypesToString(Class[] argTypes)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("(");
			if (argTypes != null)
			{
				for (int i = 0; i < argTypes.Length; i++)
				{
					if (i > 0)
					{
						buf.Append(", ");
					}
					Class c = argTypes[i];
					buf.Append((c == null) ? "null" : c.Name);
				}
			}
			buf.Append(")");
			return buf.ToString();
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.1 for interoperability </summary>
		private const long SerialVersionUID = 3206093459760846163L;


		/// <summary>
		/// Class Class is special cased within the Serialization Stream Protocol.
		/// 
		/// A Class instance is written initially into an ObjectOutputStream in the
		/// following format:
		/// <pre>
		///      {@code TC_CLASS} ClassDescriptor
		///      A ClassDescriptor is a special cased serialization of
		///      a {@code java.io.ObjectStreamClass} instance.
		/// </pre>
		/// A new handle is generated for the initial time the class descriptor
		/// is written into the stream. Future references to the class descriptor
		/// are written as references to the initial class descriptor instance.
		/// </summary>
		/// <seealso cref= java.io.ObjectStreamClass </seealso>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[0];


		/// <summary>
		/// Returns the assertion status that would be assigned to this
		/// class if it were to be initialized at the time this method is invoked.
		/// If this class has had its assertion status set, the most recent
		/// setting will be returned; otherwise, if any package default assertion
		/// status pertains to this class, the most recent setting for the most
		/// specific pertinent package default assertion status is returned;
		/// otherwise, if this class is not a system class (i.e., it has a
		/// class loader) its class loader's default assertion status is returned;
		/// otherwise, the system class default assertion status is returned.
		/// <para>
		/// Few programmers will have any need for this method; it is provided
		/// for the benefit of the JRE itself.  (It allows a class to determine at
		/// the time that it is initialized whether assertions should be enabled.)
		/// Note that this method is not guaranteed to return the actual
		/// assertion status that was (or will be) associated with the specified
		/// class when it was (or will be) initialized.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the desired assertion status of the specified class. </returns>
		/// <seealso cref=    java.lang.ClassLoader#setClassAssertionStatus </seealso>
		/// <seealso cref=    java.lang.ClassLoader#setPackageAssertionStatus </seealso>
		/// <seealso cref=    java.lang.ClassLoader#setDefaultAssertionStatus
		/// @since  1.4 </seealso>
		public bool DesiredAssertionStatus()
		{
			ClassLoader loader = ClassLoader;
			// If the loader is null this is a system class, so ask the VM
			if (loader == null)
			{
				return desiredAssertionStatus0(this);
			}

			// If the classloader has been initialized with the assertion
			// directives, ask it. Otherwise, ask the VM.
			lock (loader.AssertionLock)
			{
				if (loader.ClassAssertionStatus != null)
				{
					return loader.DesiredAssertionStatus(Name);
				}
			}
			return desiredAssertionStatus0(this);
		}

		// Retrieves the desired assertion status of this class from the VM
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean desiredAssertionStatus0(Class clazz);

		/// <summary>
		/// Returns true if and only if this class was declared as an enum in the
		/// source code.
		/// </summary>
		/// <returns> true if and only if this class was declared as an enum in the
		///     source code
		/// @since 1.5 </returns>
		public bool Enum
		{
			get
			{
				// An enum must both directly extend java.lang.Enum and have
				// the ENUM bit set; classes for specialized enum constants
				// don't do the former.
				return (this.Modifiers & ENUM) != 0 && this.BaseType == typeof(java.lang.Enum);
			}
		}

		// Fetches the factory for reflective objects
		private static ReflectionFactory ReflectionFactory
		{
			get
			{
				if (ReflectionFactory_Renamed == null)
				{
					ReflectionFactory_Renamed = AccessController.doPrivileged(new ReflectionFactory.GetReflectionFactoryAction());
				}
				return ReflectionFactory_Renamed;
			}
		}
		private static ReflectionFactory ReflectionFactory_Renamed;

		// To be able to query system properties as soon as they're available
		private static bool Initted = false;
		private static void CheckInitted()
		{
			if (Initted)
			{
				return;
			}
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
				// Tests to ensure the system properties table is fully
				// initialized. This is needed because reflection code is
				// called very early in the initialization process (before
				// command-line arguments have been parsed and therefore
				// these user-settable properties installed.) We assume that
				// if System.out is non-null then the System class has been
				// fully initialized and that the bulk of the startup code
				// has been run.

				if (System.out == null)
				{
					// java.lang.System not yet fully initialized
					return null;
				}

				// Doesn't use Boolean.getBoolean to avoid class init.
				String val = System.getProperty("sun.reflect.noCaches");
				if (val != null && val.Equals("true"))
				{
					UseCaches = false;
				}

				Initted = true;
				return null;
			}
		}

		/// <summary>
		/// Returns the elements of this enum class or null if this
		/// Class object does not represent an enum type.
		/// </summary>
		/// <returns> an array containing the values comprising the enum class
		///     represented by this Class object in the order they're
		///     declared, or null if this Class object does not
		///     represent an enum type
		/// @since 1.5 </returns>
		public T[] EnumConstants
		{
			get
			{
				T[] values = EnumConstantsShared;
				return (values != null) ? values.clone() : null;
			}
		}

		/// <summary>
		/// Returns the elements of this enum class or null if this
		/// Class object does not represent an enum type;
		/// identical to getEnumConstants except that the result is
		/// uncloned, cached, and shared by all callers.
		/// </summary>
		internal T[] EnumConstantsShared
		{
			get
			{
				if (EnumConstants_Renamed == null)
				{
					if (!Enum)
					{
						return null;
					}
					try
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Method values = getMethod("values");
						Method values = getMethod("values");
						AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this, values));
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") T[] temporaryConstants = (T[])values.invoke(null);
						T[] temporaryConstants = (T[])values.invoke(null);
						EnumConstants_Renamed = temporaryConstants;
					}
					// These can happen when users concoct enum-like classes
					// that don't comply with the enum spec.
	//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
					catch (InvocationTargetException | NoSuchMethodException | IllegalAccessException ex)
					{
					return null;
					}
				}
				return EnumConstants_Renamed;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private readonly Class OuterInstance;

			private Method Values;

			public PrivilegedActionAnonymousInnerClassHelper2(Class outerInstance, Method values)
			{
				this.OuterInstance = outerInstance;
				this.Values = values;
			}

			public virtual Void Run()
			{
					Values.Accessible = true;
					return null;
			}
		}
		[NonSerialized]
		private volatile T[] EnumConstants_Renamed = null;

		/// <summary>
		/// Returns a map from simple name to enum constant.  This package-private
		/// method is used internally by Enum to implement
		/// {@code public static <T extends Enum<T>> T valueOf(Class<T>, String)}
		/// efficiently.  Note that the map is returned by this method is
		/// created lazily on first use.  Typically it won't ever get created.
		/// </summary>
		internal IDictionary<String, T> EnumConstantDirectory()
		{
			if (EnumConstantDirectory_Renamed == null)
			{
				T[] universe = EnumConstantsShared;
				if (universe == null)
				{
					throw new IllegalArgumentException(Name + " is not an enum type");
				}
				IDictionary<String, T> m = new Dictionary<String, T>(2 * universe.Length);
				foreach (T constant in universe)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: m.put(((Enum<?>)constant).name(), constant);
					m[((Enum<?>)constant).Name()] = constant;
				}
				EnumConstantDirectory_Renamed = m;
			}
			return EnumConstantDirectory_Renamed;
		}
		[NonSerialized]
		private volatile IDictionary<String, T> EnumConstantDirectory_Renamed = null;

		/// <summary>
		/// Casts an object to the class or interface represented
		/// by this {@code Class} object.
		/// </summary>
		/// <param name="obj"> the object to be cast </param>
		/// <returns> the object after casting, or null if obj is null
		/// </returns>
		/// <exception cref="ClassCastException"> if the object is not
		/// null and is not assignable to the type T.
		/// 
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T cast(Object obj)
		public T Cast(Object obj)
		{
			if (obj != null && !isInstance(obj))
			{
				throw new ClassCastException(CannotCastMsg(obj));
			}
			return (T) obj;
		}

		private String CannotCastMsg(Object obj)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return "Cannot cast " + obj.GetType().FullName + " to " + Name;
		}

		/// <summary>
		/// Casts this {@code Class} object to represent a subclass of the class
		/// represented by the specified class object.  Checks that the cast
		/// is valid, and throws a {@code ClassCastException} if it is not.  If
		/// this method succeeds, it always returns a reference to this class object.
		/// 
		/// <para>This method is useful when a client needs to "narrow" the type of
		/// a {@code Class} object to pass it to an API that restricts the
		/// {@code Class} objects that it is willing to accept.  A cast would
		/// generate a compile-time warning, as the correctness of the cast
		/// could not be checked at runtime (because generic types are implemented
		/// by erasure).
		/// 
		/// </para>
		/// </summary>
		/// @param <U> the type to cast this class object to </param>
		/// <param name="clazz"> the class of the type to cast this class object to </param>
		/// <returns> this {@code Class} object, cast to represent a subclass of
		///    the specified class object. </returns>
		/// <exception cref="ClassCastException"> if this {@code Class} object does not
		///    represent a subclass of the specified class (here "subclass" includes
		///    the class itself).
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <U> Class asSubclass(Class clazz)
		public Class asSubclass<U>(Class clazz)
		{
			if (this.IsSubclassOf(clazz))
			{
				return (Class) this;
			}
			else
			{
				throw new ClassCastException(this.ToString());
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <A extends Annotation> A getAnnotation(Class annotationClass)
		public A getAnnotation<A>(Class annotationClass) where A : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			return (A) AnnotationData().Annotations[annotationClass];
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public override bool IsAnnotationPresent(Class annotationClass)
		{
			return GenericDeclaration.this.isAnnotationPresent(annotationClass);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override A[] getAnnotationsByType<A>(Class annotationClass) where A : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			AnnotationData annotationData = AnnotationData();
			return AnnotationSupport.getAssociatedAnnotations(annotationData.DeclaredAnnotations, this, annotationClass);
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public Annotation[] Annotations
		{
			get
			{
				return AnnotationParser.toArray(AnnotationData().Annotations);
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <A extends Annotation> A getDeclaredAnnotation(Class annotationClass)
		public override A getDeclaredAnnotation<A>(Class annotationClass) where A : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			return (A) AnnotationData().DeclaredAnnotations[annotationClass];
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override A[] getDeclaredAnnotationsByType<A>(Class annotationClass) where A : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			return AnnotationSupport.getDirectlyAndIndirectlyPresent(AnnotationData().DeclaredAnnotations, annotationClass);
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public Annotation[] DeclaredAnnotations
		{
			get
			{
				return AnnotationParser.toArray(AnnotationData().DeclaredAnnotations);
			}
		}

		// annotation data that might get invalidated when JVM TI RedefineClasses() is called
		private class AnnotationData
		{
			internal readonly IDictionary<Class, Annotation> Annotations;
			internal readonly IDictionary<Class, Annotation> DeclaredAnnotations;

			// Value of classRedefinedCount when we created this AnnotationData instance
			internal readonly int RedefinedCount;

			internal AnnotationData(IDictionary<Class, Annotation> annotations, IDictionary<Class, Annotation> declaredAnnotations, int redefinedCount)
			{
				this.Annotations = annotations;
				this.DeclaredAnnotations = declaredAnnotations;
				this.RedefinedCount = redefinedCount;
			}
		}

		// Annotations cache
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("UnusedDeclaration") private volatile transient AnnotationData annotationData;
		[NonSerialized]
		private volatile AnnotationData AnnotationData_Renamed;

		private AnnotationData AnnotationData()
		{
			while (true) // retry loop
			{
				AnnotationData annotationData = this.AnnotationData_Renamed;
				int classRedefinedCount = this.ClassRedefinedCount;
				if (annotationData != null && annotationData.RedefinedCount == classRedefinedCount)
				{
					return annotationData;
				}
				// null or stale annotationData -> optimistically create new instance
				AnnotationData newAnnotationData = CreateAnnotationData(classRedefinedCount);
				// try to install it
				if (Atomic.CasAnnotationData(this, annotationData, newAnnotationData))
				{
					// successfully installed new AnnotationData
					return newAnnotationData;
				}
			}
		}

		private AnnotationData CreateAnnotationData(int classRedefinedCount)
		{
			IDictionary<Class, Annotation> declaredAnnotations = AnnotationParser.parseAnnotations(RawAnnotations, ConstantPool, this);
			Class superClass = Superclass;
			IDictionary<Class, Annotation> annotations = null;
			if (superClass != null)
			{
				IDictionary<Class, Annotation> superAnnotations = superClass.AnnotationData().Annotations;
				foreach (java.util.Map_Entry<Class, Annotation> e in superAnnotations)
				{
					Class annotationClass = e.Key;
					if (AnnotationType.getInstance(annotationClass).Inherited)
					{
						if (annotations == null) // lazy construction
						{
							annotations = new LinkedHashMap<>((System.Math.Max(declaredAnnotations.Count, System.Math.Min(12, declaredAnnotations.Count + superAnnotations.Count)) * 4 + 2) / 3);
						}
						annotations[annotationClass] = e.Value;
					}
				}
			}
			if (annotations == null)
			{
				// no inherited annotations -> share the Map with declaredAnnotations
				annotations = declaredAnnotations;
			}
			else
			{
				// at least one inherited annotation -> declared may override inherited
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				annotations.putAll(declaredAnnotations);
			}
			return new AnnotationData(annotations, declaredAnnotations, classRedefinedCount);
		}

		// Annotation types cache their internal (AnnotationType) form

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("UnusedDeclaration") private volatile transient AnnotationType annotationType;
		[NonSerialized]
		private volatile AnnotationType AnnotationType_Renamed;

		internal bool CasAnnotationType(AnnotationType oldType, AnnotationType newType)
		{
			return Atomic.CasAnnotationType(this, oldType, newType);
		}

		internal AnnotationType AnnotationType
		{
			get
			{
				return AnnotationType_Renamed;
			}
		}

		internal IDictionary<Class, Annotation> DeclaredAnnotationMap
		{
			get
			{
				return AnnotationData().DeclaredAnnotations;
			}
		}

		/* Backing store of user-defined values pertaining to this class.
		 * Maintained by the ClassValue class.
		 */
		[NonSerialized]
		internal ClassValue.ClassValueMap ClassValueMap;

		/// <summary>
		/// Returns an {@code AnnotatedType} object that represents the use of a
		/// type to specify the superclass of the entity represented by this {@code
		/// Class} object. (The <em>use</em> of type Foo to specify the superclass
		/// in '...  extends Foo' is distinct from the <em>declaration</em> of type
		/// Foo.)
		/// 
		/// <para> If this {@code Class} object represents a type whose declaration
		/// does not explicitly indicate an annotated superclass, then the return
		/// value is an {@code AnnotatedType} object representing an element with no
		/// annotations.
		/// 
		/// </para>
		/// <para> If this {@code Class} represents either the {@code Object} class, an
		/// interface type, an array type, a primitive type, or void, the return
		/// value is {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an object representing the superclass
		/// @since 1.8 </returns>
		public AnnotatedType AnnotatedSuperclass
		{
			get
			{
				if (this == typeof(Object) || Interface || Array || Primitive || this == Void.TYPE)
				{
					return null;
				}
    
				return TypeAnnotationParser.buildAnnotatedSuperclass(RawTypeAnnotations, ConstantPool, this);
			}
		}

		/// <summary>
		/// Returns an array of {@code AnnotatedType} objects that represent the use
		/// of types to specify superinterfaces of the entity represented by this
		/// {@code Class} object. (The <em>use</em> of type Foo to specify a
		/// superinterface in '... implements Foo' is distinct from the
		/// <em>declaration</em> of type Foo.)
		/// 
		/// <para> If this {@code Class} object represents a class, the return value is
		/// an array containing objects representing the uses of interface types to
		/// specify interfaces implemented by the class. The order of the objects in
		/// the array corresponds to the order of the interface types used in the
		/// 'implements' clause of the declaration of this {@code Class} object.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents an interface, the return
		/// value is an array containing objects representing the uses of interface
		/// types to specify interfaces directly extended by the interface. The
		/// order of the objects in the array corresponds to the order of the
		/// interface types used in the 'extends' clause of the declaration of this
		/// {@code Class} object.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents a class or interface whose
		/// declaration does not explicitly indicate any annotated superinterfaces,
		/// the return value is an array of length 0.
		/// 
		/// </para>
		/// <para> If this {@code Class} object represents either the {@code Object}
		/// class, an array type, a primitive type, or void, the return value is an
		/// array of length 0.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array representing the superinterfaces
		/// @since 1.8 </returns>
		public AnnotatedType[] AnnotatedInterfaces
		{
			get
			{
				 return TypeAnnotationParser.buildAnnotatedInterfaces(RawTypeAnnotations, ConstantPool, this);
			}
		}
	}

}