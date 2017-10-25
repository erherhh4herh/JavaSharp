using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// The {@code Compiler} class is provided to support Java-to-native-code
	/// compilers and related services. By design, the {@code Compiler} class does
	/// nothing; it serves as a placeholder for a JIT compiler implementation.
	/// 
	/// <para> When the Java Virtual Machine first starts, it determines if the system
	/// property {@code java.compiler} exists. (System properties are accessible
	/// through <seealso cref="System#getProperty(String)"/> and {@link
	/// System#getProperty(String, String)}.  If so, it is assumed to be the name of
	/// a library (with a platform-dependent exact location and type); {@link
	/// System#loadLibrary} is called to load that library. If this loading
	/// succeeds, the function named {@code java_lang_Compiler_start()} in that
	/// library is called.
	/// 
	/// </para>
	/// <para> If no compiler is available, these methods do nothing.
	/// 
	/// @author  Frank Yellin
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public sealed class Compiler
	{
		private Compiler() // don't make instances
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initialize();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void registerNatives();

		static Compiler()
		{
			registerNatives();
			java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
				bool loaded = false;
				String jit = System.getProperty("java.compiler");
				if ((jit != null) && (!jit.Equals("NONE")) && (!jit.Equals("")))
				{
					try
					{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//						System.loadLibrary(jit);
						initialize();
						loaded = true;
					}
					catch (UnsatisfiedLinkError)
					{
						System.Console.Error.WriteLine("Warning: JIT compiler \"" + jit + "\" not found. Will use interpreter.");
					}
				}
				String info = System.getProperty("java.vm.info");
				if (loaded)
				{
					System.setProperty("java.vm.info", info + ", " + jit);
				}
				else
				{
					System.setProperty("java.vm.info", info + ", nojit");
				}
				return null;
			}
		}

		/// <summary>
		/// Compiles the specified class.
		/// </summary>
		/// <param name="clazz">
		///         A class
		/// </param>
		/// <returns>  {@code true} if the compilation succeeded; {@code false} if the
		///          compilation failed or no compiler is available
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code clazz} is {@code null} </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern boolean compileClass(Class clazz);

		/// <summary>
		/// Compiles all classes whose name matches the specified string.
		/// </summary>
		/// <param name="string">
		///         The name of the classes to compile
		/// </param>
		/// <returns>  {@code true} if the compilation succeeded; {@code false} if the
		///          compilation failed or no compiler is available
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code string} is {@code null} </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern boolean compileClasses(String @string);

		/// <summary>
		/// Examines the argument type and its fields and perform some documented
		/// operation.  No specific operations are required.
		/// </summary>
		/// <param name="any">
		///         An argument
		/// </param>
		/// <returns>  A compiler-specific value, or {@code null} if no compiler is
		///          available
		/// </returns>
		/// <exception cref="NullPointerException">
		///          If {@code any} is {@code null} </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern Object command(Object any);

		/// <summary>
		/// Cause the Compiler to resume operation.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern void enable();

		/// <summary>
		/// Cause the Compiler to cease operation.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern void disable();
	}

}