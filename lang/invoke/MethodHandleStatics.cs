using System;

/*
 * Copyright (c) 2011, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	using Unsafe = sun.misc.Unsafe;

	/// <summary>
	/// This class consists exclusively of static names internal to the
	/// method handle implementation.
	/// Usage:  {@code import static java.lang.invoke.MethodHandleStatics.*}
	/// @author John Rose, JSR 292 EG
	/// </summary>
	/*non-public*/	 internal class MethodHandleStatics
	 {

		private MethodHandleStatics() // do not instantiate
		{
		}

		internal static readonly Unsafe UNSAFE = Unsafe.Unsafe;

		internal static readonly bool DEBUG_METHOD_HANDLE_NAMES;
		internal static readonly bool DUMP_CLASS_FILES;
		internal static readonly bool TRACE_INTERPRETER;
		internal static readonly bool TRACE_METHOD_LINKAGE;
		internal static readonly int COMPILE_THRESHOLD;
		internal static readonly int DONT_INLINE_THRESHOLD;
		internal static readonly int PROFILE_LEVEL;
		internal static readonly bool PROFILE_GWT;
		internal static readonly int CUSTOMIZE_THRESHOLD;

		static MethodHandleStatics()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] values = new Object[9];
			Object[] values = new Object[9];
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(values));
			DEBUG_METHOD_HANDLE_NAMES = (Boolean) values[0];
			DUMP_CLASS_FILES = (Boolean) values[1];
			TRACE_INTERPRETER = (Boolean) values[2];
			TRACE_METHOD_LINKAGE = (Boolean) values[3];
			COMPILE_THRESHOLD = (Integer) values[4];
			DONT_INLINE_THRESHOLD = (Integer) values[5];
			PROFILE_LEVEL = (Integer) values[6];
			PROFILE_GWT = (Boolean) values[7];
			CUSTOMIZE_THRESHOLD = (Integer) values[8];

			if (CUSTOMIZE_THRESHOLD < -1 || CUSTOMIZE_THRESHOLD > 127)
			{
				throw NewInternalError("CUSTOMIZE_THRESHOLD should be in [-1...127] range");
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private object[] Values;

			public PrivilegedActionAnonymousInnerClassHelper(object[] values)
			{
				this.Values = values;
			}

			public virtual Void Run()
			{
				Values[0] = Boolean.GetBoolean("java.lang.invoke.MethodHandle.DEBUG_NAMES");
				Values[1] = Boolean.GetBoolean("java.lang.invoke.MethodHandle.DUMP_CLASS_FILES");
				Values[2] = Boolean.GetBoolean("java.lang.invoke.MethodHandle.TRACE_INTERPRETER");
				Values[3] = Boolean.GetBoolean("java.lang.invoke.MethodHandle.TRACE_METHOD_LINKAGE");
				Values[4] = Integer.GetInteger("java.lang.invoke.MethodHandle.COMPILE_THRESHOLD", 0);
				Values[5] = Integer.GetInteger("java.lang.invoke.MethodHandle.DONT_INLINE_THRESHOLD", 30);
				Values[6] = Integer.GetInteger("java.lang.invoke.MethodHandle.PROFILE_LEVEL", 0);
				Values[7] = Convert.ToBoolean(System.getProperty("java.lang.invoke.MethodHandle.PROFILE_GWT", "true"));
				Values[8] = Integer.GetInteger("java.lang.invoke.MethodHandle.CUSTOMIZE_THRESHOLD", 127);
				return null;
			}
		}

		/// <summary>
		/// Tell if any of the debugging switches are turned on.
		///  If this is the case, it is reasonable to perform extra checks or save extra information.
		/// </summary>
		/*non-public*/	 internal static bool DebugEnabled()
	 {
			return (DEBUG_METHOD_HANDLE_NAMES | DUMP_CLASS_FILES | TRACE_INTERPRETER | TRACE_METHOD_LINKAGE);
	 }

		/*non-public*/	 internal static String GetNameString(MethodHandle target, MethodType type)
	 {
			if (type == null)
			{
				type = target.Type();
			}
			MemberName name = null;
			if (target != null)
			{
				name = target.InternalMemberName();
			}
			if (name == null)
			{
				return "invoke" + type;
			}
			return name.Name + type;
	 }

		/*non-public*/	 internal static String GetNameString(MethodHandle target, MethodHandle typeHolder)
	 {
			return GetNameString(target, typeHolder == null ? (MethodType) null : typeHolder.Type());
	 }

		/*non-public*/	 internal static String GetNameString(MethodHandle target)
	 {
			return GetNameString(target, (MethodType) null);
	 }

		/*non-public*/	 internal static String AddTypeString(Object obj, MethodHandle target)
	 {
			String str = Convert.ToString(obj);
			if (target == null)
			{
				return str;
			}
			int paren = str.IndexOf('(');
			if (paren >= 0)
			{
				str = str.Substring(0, paren);
			}
			return str + target.Type();
	 }

		// handy shared exception makers (they simplify the common case code)
		/*non-public*/	 internal static InternalError NewInternalError(String message)
	 {
			return new InternalError(message);
	 }
		/*non-public*/	 internal static InternalError NewInternalError(String message, Throwable cause)
	 {
			return new InternalError(message, cause);
	 }
		/*non-public*/	 internal static InternalError NewInternalError(Throwable cause)
	 {
			return new InternalError(cause);
	 }
		/*non-public*/	 internal static RuntimeException NewIllegalStateException(String message)
	 {
			return new IllegalStateException(message);
	 }
		/*non-public*/	 internal static RuntimeException NewIllegalStateException(String message, Object obj)
	 {
			return new IllegalStateException(Message(message, obj));
	 }
		/*non-public*/	 internal static RuntimeException NewIllegalArgumentException(String message)
	 {
			return new IllegalArgumentException(message);
	 }
		/*non-public*/	 internal static RuntimeException NewIllegalArgumentException(String message, Object obj)
	 {
			return new IllegalArgumentException(Message(message, obj));
	 }
		/*non-public*/	 internal static RuntimeException NewIllegalArgumentException(String message, Object obj, Object obj2)
	 {
			return new IllegalArgumentException(Message(message, obj, obj2));
	 }
		/// <summary>
		/// Propagate unchecked exceptions and errors, but wrap anything checked and throw that instead. </summary>
		/*non-public*/	 internal static Error UncaughtException(Throwable ex)
	 {
			if (ex is Error)
			{
				throw (Error) ex;
			}
			if (ex is RuntimeException)
			{
				throw (RuntimeException) ex;
			}
			throw NewInternalError("uncaught exception", ex);
	 }
		internal static Error NYI()
		{
			throw new AssertionError("NYI");
		}
		private static String Message(String message, Object obj)
		{
			if (obj != null)
			{
				message = message + ": " + obj;
			}
			return message;
		}
		private static String Message(String message, Object obj, Object obj2)
		{
			if (obj != null || obj2 != null)
			{
				message = message + ": " + obj + ", " + obj2;
			}
			return message;
		}
	 }

}