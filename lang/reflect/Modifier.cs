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

namespace java.lang.reflect
{

	using LangReflectAccess = sun.reflect.LangReflectAccess;
	using ReflectionFactory = sun.reflect.ReflectionFactory;

	/// <summary>
	/// The Modifier class provides {@code static} methods and
	/// constants to decode class and member access modifiers.  The sets of
	/// modifiers are represented as integers with distinct bit positions
	/// representing different modifiers.  The values for the constants
	/// representing the modifiers are taken from the tables in sections 4.1, 4.4, 4.5, and 4.7 of
	/// <cite>The Java&trade; Virtual Machine Specification</cite>.
	/// </summary>
	/// <seealso cref= Class#getModifiers() </seealso>
	/// <seealso cref= Member#getModifiers()
	/// 
	/// @author Nakul Saraiya
	/// @author Kenneth Russell </seealso>
	public class Modifier
	{

		/*
		 * Bootstrapping protocol between java.lang and java.lang.reflect
		 *  packages
		 */
		static Modifier()
		{
			ReflectionFactory factory = AccessController.doPrivileged(new ReflectionFactory.GetReflectionFactoryAction());
			factory.LangReflectAccess = new java.lang.reflect.ReflectAccess();
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code public} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code public} modifier; {@code false} otherwise. </returns>
		public static bool IsPublic(int mod)
		{
			return (mod & PUBLIC) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code private} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code private} modifier; {@code false} otherwise. </returns>
		public static bool IsPrivate(int mod)
		{
			return (mod & PRIVATE) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code protected} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code protected} modifier; {@code false} otherwise. </returns>
		public static bool IsProtected(int mod)
		{
			return (mod & PROTECTED) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code static} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code static} modifier; {@code false} otherwise. </returns>
		public static bool IsStatic(int mod)
		{
			return (mod & STATIC) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code final} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code final} modifier; {@code false} otherwise. </returns>
		public static bool IsFinal(int mod)
		{
			return (mod & FINAL) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code synchronized} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code synchronized} modifier; {@code false} otherwise. </returns>
		public static bool IsSynchronized(int mod)
		{
			return (mod & SYNCHRONIZED) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code volatile} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code volatile} modifier; {@code false} otherwise. </returns>
		public static bool IsVolatile(int mod)
		{
			return (mod & VOLATILE) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code transient} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code transient} modifier; {@code false} otherwise. </returns>
		public static bool IsTransient(int mod)
		{
			return (mod & TRANSIENT) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code native} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code native} modifier; {@code false} otherwise. </returns>
		public static bool IsNative(int mod)
		{
			return (mod & NATIVE) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code interface} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code interface} modifier; {@code false} otherwise. </returns>
		public static bool IsInterface(int mod)
		{
			return (mod & INTERFACE) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code abstract} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code abstract} modifier; {@code false} otherwise. </returns>
		public static bool IsAbstract(int mod)
		{
			return (mod & ABSTRACT) != 0;
		}

		/// <summary>
		/// Return {@code true} if the integer argument includes the
		/// {@code strictfp} modifier, {@code false} otherwise.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns> {@code true} if {@code mod} includes the
		/// {@code strictfp} modifier; {@code false} otherwise. </returns>
		public static bool IsStrict(int mod)
		{
			return (mod & STRICT) != 0;
		}

		/// <summary>
		/// Return a string describing the access modifier flags in
		/// the specified modifier. For example:
		/// <blockquote><pre>
		///    public final synchronized strictfp
		/// </pre></blockquote>
		/// The modifier names are returned in an order consistent with the
		/// suggested modifier orderings given in sections 8.1.1, 8.3.1, 8.4.3, 8.8.3, and 9.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// The full modifier ordering used by this method is:
		/// <blockquote> {@code
		/// public protected private abstract static final transient
		/// volatile synchronized native strictfp
		/// interface } </blockquote>
		/// The {@code interface} modifier discussed in this class is
		/// not a true modifier in the Java language and it appears after
		/// all other modifiers listed by this method.  This method may
		/// return a string of modifiers that are not valid modifiers of a
		/// Java entity; in other words, no checking is done on the
		/// possible validity of the combination of modifiers represented
		/// by the input.
		/// 
		/// Note that to perform such checking for a known kind of entity,
		/// such as a constructor or method, first AND the argument of
		/// {@code toString} with the appropriate mask from a method like
		/// <seealso cref="#constructorModifiers"/> or <seealso cref="#methodModifiers"/>.
		/// </summary>
		/// <param name="mod"> a set of modifiers </param>
		/// <returns>  a string representation of the set of modifiers
		/// represented by {@code mod} </returns>
		public static String ToString(int mod)
		{
			StringBuilder sb = new StringBuilder();
			int len;

			if ((mod & PUBLIC) != 0)
			{
				sb.Append("public ");
			}
			if ((mod & PROTECTED) != 0)
			{
				sb.Append("protected ");
			}
			if ((mod & PRIVATE) != 0)
			{
				sb.Append("private ");
			}

			/* Canonical order */
			if ((mod & ABSTRACT) != 0)
			{
				sb.Append("abstract ");
			}
			if ((mod & STATIC) != 0)
			{
				sb.Append("static ");
			}
			if ((mod & FINAL) != 0)
			{
				sb.Append("final ");
			}
			if ((mod & TRANSIENT) != 0)
			{
				sb.Append("transient ");
			}
			if ((mod & VOLATILE) != 0)
			{
				sb.Append("volatile ");
			}
			if ((mod & SYNCHRONIZED) != 0)
			{
				sb.Append("synchronized ");
			}
			if ((mod & NATIVE) != 0)
			{
				sb.Append("native ");
			}
			if ((mod & STRICT) != 0)
			{
				sb.Append("strictfp ");
			}
			if ((mod & INTERFACE) != 0)
			{
				sb.Append("interface ");
			}

			if ((len = sb.Length()) > 0) // trim trailing space
			{
				return sb.ToString().Substring(0, len - 1);
			}
			return "";
		}

		/*
		 * Access modifier flag constants from tables 4.1, 4.4, 4.5, and 4.7 of
		 * <cite>The Java&trade; Virtual Machine Specification</cite>
		 */

		/// <summary>
		/// The {@code int} value representing the {@code public}
		/// modifier.
		/// </summary>
		public const int PUBLIC = 0x00000001;

		/// <summary>
		/// The {@code int} value representing the {@code private}
		/// modifier.
		/// </summary>
		public const int PRIVATE = 0x00000002;

		/// <summary>
		/// The {@code int} value representing the {@code protected}
		/// modifier.
		/// </summary>
		public const int PROTECTED = 0x00000004;

		/// <summary>
		/// The {@code int} value representing the {@code static}
		/// modifier.
		/// </summary>
		public const int STATIC = 0x00000008;

		/// <summary>
		/// The {@code int} value representing the {@code final}
		/// modifier.
		/// </summary>
		public const int FINAL = 0x00000010;

		/// <summary>
		/// The {@code int} value representing the {@code synchronized}
		/// modifier.
		/// </summary>
		public const int SYNCHRONIZED = 0x00000020;

		/// <summary>
		/// The {@code int} value representing the {@code volatile}
		/// modifier.
		/// </summary>
		public const int VOLATILE = 0x00000040;

		/// <summary>
		/// The {@code int} value representing the {@code transient}
		/// modifier.
		/// </summary>
		public const int TRANSIENT = 0x00000080;

		/// <summary>
		/// The {@code int} value representing the {@code native}
		/// modifier.
		/// </summary>
		public const int NATIVE = 0x00000100;

		/// <summary>
		/// The {@code int} value representing the {@code interface}
		/// modifier.
		/// </summary>
		public const int INTERFACE = 0x00000200;

		/// <summary>
		/// The {@code int} value representing the {@code abstract}
		/// modifier.
		/// </summary>
		public const int ABSTRACT = 0x00000400;

		/// <summary>
		/// The {@code int} value representing the {@code strictfp}
		/// modifier.
		/// </summary>
		public const int STRICT = 0x00000800;

		// Bits not (yet) exposed in the public API either because they
		// have different meanings for fields and methods and there is no
		// way to distinguish between the two in this class, or because
		// they are not Java programming language keywords
		internal const int BRIDGE = 0x00000040;
		internal const int VARARGS = 0x00000080;
		internal const int SYNTHETIC = 0x00001000;
		internal const int ANNOTATION = 0x00002000;
		internal const int ENUM = 0x00004000;
		internal const int MANDATED = 0x00008000;
		internal static bool IsSynthetic(int mod)
		{
		  return (mod & SYNTHETIC) != 0;
		}

		internal static bool IsMandated(int mod)
		{
		  return (mod & MANDATED) != 0;
		}

		// Note on the FOO_MODIFIERS fields and fooModifiers() methods:
		// the sets of modifiers are not guaranteed to be constants
		// across time and Java SE releases. Therefore, it would not be
		// appropriate to expose an external interface to this information
		// that would allow the values to be treated as Java-level
		// constants since the values could be constant folded and updates
		// to the sets of modifiers missed. Thus, the fooModifiers()
		// methods return an unchanging values for a given release, but a
		// value that can potentially change over time.

		/// <summary>
		/// The Java source modifiers that can be applied to a class.
		/// @jls 8.1.1 Class Modifiers
		/// </summary>
		private static readonly int CLASS_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE | Modifier.ABSTRACT | Modifier.STATIC | Modifier.FINAL | Modifier.STRICT;

		/// <summary>
		/// The Java source modifiers that can be applied to an interface.
		/// @jls 9.1.1 Interface Modifiers
		/// </summary>
		private static readonly int INTERFACE_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE | Modifier.ABSTRACT | Modifier.STATIC | Modifier.STRICT;


		/// <summary>
		/// The Java source modifiers that can be applied to a constructor.
		/// @jls 8.8.3 Constructor Modifiers
		/// </summary>
		private static readonly int CONSTRUCTOR_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE;

		/// <summary>
		/// The Java source modifiers that can be applied to a method.
		/// @jls8.4.3  Method Modifiers
		/// </summary>
		private static readonly int METHOD_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE | Modifier.ABSTRACT | Modifier.STATIC | Modifier.FINAL | Modifier.SYNCHRONIZED | Modifier.NATIVE | Modifier.STRICT;

		/// <summary>
		/// The Java source modifiers that can be applied to a field.
		/// @jls 8.3.1  Field Modifiers
		/// </summary>
		private static readonly int FIELD_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE | Modifier.STATIC | Modifier.FINAL | Modifier.TRANSIENT | Modifier.VOLATILE;

		/// <summary>
		/// The Java source modifiers that can be applied to a method or constructor parameter.
		/// @jls 8.4.1 Formal Parameters
		/// </summary>
		private const int PARAMETER_MODIFIERS = Modifier.FINAL;

		/// 
		internal static readonly int ACCESS_MODIFIERS = Modifier.PUBLIC | Modifier.PROTECTED | Modifier.PRIVATE;

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a class. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a class.
		/// 
		/// @jls 8.1.1 Class Modifiers
		/// @since 1.7 </returns>
		public static int ClassModifiers()
		{
			return CLASS_MODIFIERS;
		}

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to an interface. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to an interface.
		/// 
		/// @jls 9.1.1 Interface Modifiers
		/// @since 1.7 </returns>
		public static int InterfaceModifiers()
		{
			return INTERFACE_MODIFIERS;
		}

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a constructor. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a constructor.
		/// 
		/// @jls 8.8.3 Constructor Modifiers
		/// @since 1.7 </returns>
		public static int ConstructorModifiers()
		{
			return CONSTRUCTOR_MODIFIERS;
		}

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a method. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a method.
		/// 
		/// @jls 8.4.3 Method Modifiers
		/// @since 1.7 </returns>
		public static int MethodModifiers()
		{
			return METHOD_MODIFIERS;
		}

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a field. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a field.
		/// 
		/// @jls 8.3.1 Field Modifiers
		/// @since 1.7 </returns>
		public static int FieldModifiers()
		{
			return FIELD_MODIFIERS;
		}

		/// <summary>
		/// Return an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a parameter. </summary>
		/// <returns> an {@code int} value OR-ing together the source language
		/// modifiers that can be applied to a parameter.
		/// 
		/// @jls 8.4.1 Formal Parameters
		/// @since 1.8 </returns>
		public static int ParameterModifiers()
		{
			return PARAMETER_MODIFIERS;
		}
	}

}