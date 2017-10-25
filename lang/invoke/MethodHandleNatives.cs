using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// The JVM interface for the method handles package is all here.
	/// This is an interface internal and private to an implementation of JSR 292.
	/// <em>This class is not part of the JSR 292 standard.</em>
	/// @author jrose
	/// </summary>
	internal class MethodHandleNatives
	{

		private MethodHandleNatives() // static only
		{
		}

		/// MemberName support

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void init(MemberName self, Object @ref);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void expand(MemberName self);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern MemberName resolve(MemberName self, Class caller);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int getMembers(Class defc, String matchName, String matchSig, int matchFlags, Class caller, int skip, MemberName[] results);

		/// Field layout queries parallel to sun.misc.Unsafe:
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern long objectFieldOffset(MemberName self);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern long staticFieldOffset(MemberName self);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern Object staticFieldBase(MemberName self);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern Object getMemberVMInfo(MemberName self);

		/// MethodHandle support

		/// <summary>
		/// Fetch MH-related JVM parameter.
		///  which=0 retrieves MethodHandlePushLimit
		///  which=1 retrieves stack slot push size (in address units)
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern int getConstant(int which);

		internal static readonly bool COUNT_GWT;

		/// CallSite support

		/// <summary>
		/// Tell the JVM that we need to change the target of a CallSite. </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void setCallSiteTargetNormal(CallSite site, MethodHandle target);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void setCallSiteTargetVolatile(CallSite site, MethodHandle target);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void registerNatives();
		static MethodHandleNatives()
		{
			registerNatives();
			COUNT_GWT = getConstant(Constants.GC_COUNT_GWT) != 0;

			// The JVM calls MethodHandleNatives.<clinit>.  Cascade the <clinit> calls as needed:
			MethodHandleImpl.InitStatics();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int HR_MASK = ((1 << REF_getField) | (1 << REF_putField) | (1 << REF_invokeVirtual) | (1 << REF_invokeSpecial) | (1 << REF_invokeInterface));
			int HR_MASK = ((1 << REF_getField) | (1 << REF_putField) | (1 << REF_invokeVirtual) | (1 << REF_invokeSpecial) | (1 << REF_invokeInterface));
			for (sbyte refKind = REF_NONE+1; refKind < REF_LIMIT; refKind++)
			{
				assert(RefKindHasReceiver(refKind) == (((1 << refKind) & HR_MASK) != 0)) : refKind;
			}
			assert(VerifyConstants());
		}

		// All compile-time constants go here.
		// There is an opportunity to check them against the JVM's idea of them.
		internal class Constants
		{
			internal Constants() // static only
			{
			}
			// MethodHandleImpl
			internal const int GC_COUNT_GWT = 4, GC_LAMBDA_SUPPORT = 5; // for getConstant

			// MemberName
			// The JVM uses values of -2 and above for vtable indexes.
			// Field values are simple positive offsets.
			// Ref: src/share/vm/oops/methodOop.hpp
			// This value is negative enough to avoid such numbers,
			// but not too negative.
			internal const int MN_IS_METHOD = 0x00010000, MN_IS_CONSTRUCTOR = 0x00020000, MN_IS_FIELD = 0x00040000, MN_IS_TYPE = 0x00080000, MN_CALLER_SENSITIVE = 0x00100000, MN_REFERENCE_KIND_SHIFT = 24, MN_REFERENCE_KIND_MASK = 0x0F000000 >> MN_REFERENCE_KIND_SHIFT, MN_SEARCH_SUPERCLASSES = 0x00100000, MN_SEARCH_INTERFACES = 0x00200000; // refKind -  @CallerSensitive annotation detected -  nested type -  field -  constructor -  method (not constructor)
					// The SEARCH_* bits are not for MN.flags but for the matchFlags argument of MHN.getMembers:

			/// <summary>
			/// Basic types as encoded in the JVM.  These code values are not
			/// intended for use outside this class.  They are used as part of
			/// a private interface between the JVM and this class.
			/// </summary>
			internal const int T_BOOLEAN = 4, T_CHAR = 5, T_FLOAT = 6, T_DOUBLE = 7, T_BYTE = 8, T_SHORT = 9, T_INT = 10, T_LONG = 11, T_OBJECT = 12, T_VOID = 14, T_ILLEGAL = 99;
				//T_ARRAY    = 13
				//T_ADDRESS  = 15

			/// <summary>
			/// Constant pool entry types.
			/// </summary>
			internal const sbyte CONSTANT_Utf8 = 1, CONSTANT_Integer = 3, CONSTANT_Float = 4, CONSTANT_Long = 5, CONSTANT_Double = 6, CONSTANT_Class = 7, CONSTANT_String = 8, CONSTANT_Fieldref = 9, CONSTANT_Methodref = 10, CONSTANT_InterfaceMethodref = 11, CONSTANT_NameAndType = 12, CONSTANT_MethodHandle = 15, CONSTANT_MethodType = 16, CONSTANT_InvokeDynamic = 18, CONSTANT_LIMIT = 19; // Limit to tags found in classfiles -  JSR 292 -  JSR 292

			/// <summary>
			/// Access modifier flags.
			/// </summary>
			internal const char ACC_PUBLIC = (char)0x0001, ACC_PRIVATE = (char)0x0002, ACC_PROTECTED = (char)0x0004, ACC_STATIC = (char)0x0008, ACC_FINAL = (char)0x0010, ACC_SYNCHRONIZED = (char)0x0020, ACC_VOLATILE = (char)0x0040, ACC_TRANSIENT = (char)0x0080, ACC_NATIVE = (char)0x0100, ACC_INTERFACE = (char)0x0200, ACC_ABSTRACT = (char)0x0400, ACC_STRICT = (char)0x0800, ACC_SYNTHETIC = (char)0x1000, ACC_ANNOTATION = (char)0x2000, ACC_ENUM = (char)0x4000, ACC_SUPER = ACC_SYNCHRONIZED, ACC_BRIDGE = ACC_VOLATILE, ACC_VARARGS = ACC_TRANSIENT;
				// aliases:

			/// <summary>
			/// Constant pool reference-kind codes, as used by CONSTANT_MethodHandle CP entries.
			/// </summary>
			internal const sbyte REF_NONE = 0, REF_getField = 1, REF_getStatic = 2, REF_putField = 3, REF_putStatic = 4, REF_invokeVirtual = 5, REF_invokeStatic = 6, REF_invokeSpecial = 7, REF_newInvokeSpecial = 8, REF_invokeInterface = 9, REF_LIMIT = 10; // null value
		}

		internal static bool RefKindIsValid(int refKind)
		{
			return (refKind > REF_NONE && refKind < REF_LIMIT);
		}
		internal static bool RefKindIsField(sbyte refKind)
		{
			assert(RefKindIsValid(refKind));
			return (refKind <= REF_putStatic);
		}
		internal static bool RefKindIsGetter(sbyte refKind)
		{
			assert(RefKindIsValid(refKind));
			return (refKind <= REF_getStatic);
		}
		internal static bool RefKindIsSetter(sbyte refKind)
		{
			return RefKindIsField(refKind) && !RefKindIsGetter(refKind);
		}
		internal static bool RefKindIsMethod(sbyte refKind)
		{
			return !RefKindIsField(refKind) && (refKind != REF_newInvokeSpecial);
		}
		internal static bool RefKindIsConstructor(sbyte refKind)
		{
			return (refKind == REF_newInvokeSpecial);
		}
		internal static bool RefKindHasReceiver(sbyte refKind)
		{
			assert(RefKindIsValid(refKind));
			return (refKind & 1) != 0;
		}
		internal static bool RefKindIsStatic(sbyte refKind)
		{
			return !RefKindHasReceiver(refKind) && (refKind != REF_newInvokeSpecial);
		}
		internal static bool RefKindDoesDispatch(sbyte refKind)
		{
			assert(RefKindIsValid(refKind));
			return (refKind == REF_invokeVirtual || refKind == REF_invokeInterface);
		}
		internal static String RefKindName(sbyte refKind)
		{
			assert(RefKindIsValid(refKind));
			switch (refKind)
			{
			case REF_getField:
				return "getField";
			case REF_getStatic:
				return "getStatic";
			case REF_putField:
				return "putField";
			case REF_putStatic:
				return "putStatic";
			case REF_invokeVirtual:
				return "invokeVirtual";
			case REF_invokeStatic:
				return "invokeStatic";
			case REF_invokeSpecial:
				return "invokeSpecial";
			case REF_newInvokeSpecial:
				return "newInvokeSpecial";
			case REF_invokeInterface:
				return "invokeInterface";
			default:
				return "REF_???";
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getNamedCon(int which, Object[] name);
		internal static bool VerifyConstants()
		{
			Object[] box = new Object[] {null};
			for (int i = 0; ; i++)
			{
				box[0] = null;
				int vmval = getNamedCon(i, box);
				if (box[0] == null)
				{
					break;
				}
				String name = (String) box[0];
				try
				{
					Field con = typeof(Constants).getDeclaredField(name);
					int jval = con.getInt(null);
					if (jval == vmval)
					{
						continue;
					}
					String err = (name + ": JVM has " + vmval + " while Java has " + jval);
					if (name.Equals("CONV_OP_LIMIT"))
					{
						System.Console.Error.WriteLine("warning: " + err);
						continue;
					}
					throw new InternalError(err);
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (NoSuchFieldException | IllegalAccessException ex)
				{
					String err = (name + ": JVM has " + vmval + " which Java does not define");
					// ignore exotic ops the JVM cares about; we just wont issue them
					//System.err.println("warning: "+err);
					continue;
				}
			}
			return true;
		}

		// Up-calls from the JVM.
		// These must NOT be public.

		/// <summary>
		/// The JVM is linking an invokedynamic instruction.  Create a reified call site for it.
		/// </summary>
		internal static MemberName LinkCallSite(Object callerObj, Object bootstrapMethodObj, Object nameObj, Object typeObj, Object staticArguments, Object[] appendixResult)
		{
			MethodHandle bootstrapMethod = (MethodHandle)bootstrapMethodObj;
			Class caller = (Class)callerObj;
			String name = nameObj.ToString().intern();
			MethodType type = (MethodType)typeObj;
			if (!TRACE_METHOD_LINKAGE)
			{
				return LinkCallSiteImpl(caller, bootstrapMethod, name, type, staticArguments, appendixResult);
			}
			return LinkCallSiteTracing(caller, bootstrapMethod, name, type, staticArguments, appendixResult);
		}
		internal static MemberName LinkCallSiteImpl(Class caller, MethodHandle bootstrapMethod, String name, MethodType type, Object staticArguments, Object[] appendixResult)
		{
			CallSite callSite = CallSite.MakeSite(bootstrapMethod, name, type, staticArguments, caller);
			if (callSite is ConstantCallSite)
			{
				appendixResult[0] = callSite.DynamicInvoker();
				return Invokers.LinkToTargetMethod(type);
			}
			else
			{
				appendixResult[0] = callSite;
				return Invokers.LinkToCallSiteMethod(type);
			}
		}
		// Tracing logic:
		internal static MemberName LinkCallSiteTracing(Class caller, MethodHandle bootstrapMethod, String name, MethodType type, Object staticArguments, Object[] appendixResult)
		{
			Object bsmReference = bootstrapMethod.InternalMemberName();
			if (bsmReference == null)
			{
				bsmReference = bootstrapMethod;
			}
			Object staticArglist = (staticArguments is Object[] ? (Object[]) staticArguments : staticArguments);
			System.Console.WriteLine("linkCallSite " + caller.Name + " " + bsmReference + " " + name + type + "/" + staticArglist);
			try
			{
				MemberName res = LinkCallSiteImpl(caller, bootstrapMethod, name, type, staticArguments, appendixResult);
				System.Console.WriteLine("linkCallSite => " + res + " + " + appendixResult[0]);
				return res;
			}
			catch (Throwable ex)
			{
				System.Console.WriteLine("linkCallSite => throw " + ex);
				throw ex;
			}
		}

		/// <summary>
		/// The JVM wants a pointer to a MethodType.  Oblige it by finding or creating one.
		/// </summary>
		internal static MethodType FindMethodHandleType(Class rtype, Class[] ptypes)
		{
			return MethodType.MakeImpl(rtype, ptypes, true);
		}

		/// <summary>
		/// The JVM wants to link a call site that requires a dynamic type check.
		/// Name is a type-checking invoker, invokeExact or invoke.
		/// Return a JVM method (MemberName) to handle the invoking.
		/// The method assumes the following arguments on the stack:
		/// 0: the method handle being invoked
		/// 1-N: the arguments to the method handle invocation
		/// N+1: an optional, implicitly added argument (typically the given MethodType)
		/// <para>
		/// The nominal method at such a call site is an instance of
		/// a signature-polymorphic method (see @PolymorphicSignature).
		/// Such method instances are user-visible entities which are
		/// "split" from the generic placeholder method in {@code MethodHandle}.
		/// (Note that the placeholder method is not identical with any of
		/// its instances.  If invoked reflectively, is guaranteed to throw an
		/// {@code UnsupportedOperationException}.)
		/// If the signature-polymorphic method instance is ever reified,
		/// it appears as a "copy" of the original placeholder
		/// (a native final member of {@code MethodHandle}) except
		/// that its type descriptor has shape required by the instance,
		/// and the method instance is <em>not</em> varargs.
		/// The method instance is also marked synthetic, since the
		/// method (by definition) does not appear in Java source code.
		/// </para>
		/// <para>
		/// The JVM is allowed to reify this method as instance metadata.
		/// For example, {@code invokeBasic} is always reified.
		/// But the JVM may instead call {@code linkMethod}.
		/// If the result is an * ordered pair of a {@code (method, appendix)},
		/// the method gets all the arguments (0..N inclusive)
		/// plus the appendix (N+1), and uses the appendix to complete the call.
		/// In this way, one reusable method (called a "linker method")
		/// can perform the function of any number of polymorphic instance
		/// methods.
		/// </para>
		/// <para>
		/// Linker methods are allowed to be weakly typed, with any or
		/// all references rewritten to {@code Object} and any primitives
		/// (except {@code long}/{@code float}/{@code double})
		/// rewritten to {@code int}.
		/// A linker method is trusted to return a strongly typed result,
		/// according to the specific method type descriptor of the
		/// signature-polymorphic instance it is emulating.
		/// This can involve (as necessary) a dynamic check using
		/// data extracted from the appendix argument.
		/// </para>
		/// <para>
		/// The JVM does not inspect the appendix, other than to pass
		/// it verbatim to the linker method at every call.
		/// This means that the JDK runtime has wide latitude
		/// for choosing the shape of each linker method and its
		/// corresponding appendix.
		/// Linker methods should be generated from {@code LambdaForm}s
		/// so that they do not become visible on stack traces.
		/// </para>
		/// <para>
		/// The {@code linkMethod} call is free to omit the appendix
		/// (returning null) and instead emulate the required function
		/// completely in the linker method.
		/// As a corner case, if N==255, no appendix is possible.
		/// In this case, the method returned must be custom-generated to
		/// to perform any needed type checking.
		/// </para>
		/// <para>
		/// If the JVM does not reify a method at a call site, but instead
		/// calls {@code linkMethod}, the corresponding call represented
		/// in the bytecodes may mention a valid method which is not
		/// representable with a {@code MemberName}.
		/// Therefore, use cases for {@code linkMethod} tend to correspond to
		/// special cases in reflective code such as {@code findVirtual}
		/// or {@code revealDirect}.
		/// </para>
		/// </summary>
		internal static MemberName LinkMethod(Class callerClass, int refKind, Class defc, String name, Object type, Object[] appendixResult)
		{
			if (!TRACE_METHOD_LINKAGE)
			{
				return LinkMethodImpl(callerClass, refKind, defc, name, type, appendixResult);
			}
			return LinkMethodTracing(callerClass, refKind, defc, name, type, appendixResult);
		}
		internal static MemberName LinkMethodImpl(Class callerClass, int refKind, Class defc, String name, Object type, Object[] appendixResult)
		{
			try
			{
				if (defc == typeof(MethodHandle) && refKind == REF_invokeVirtual)
				{
					return Invokers.MethodHandleInvokeLinkerMethod(name, FixMethodType(callerClass, type), appendixResult);
				}
			}
			catch (Throwable ex)
			{
				if (ex is LinkageError)
				{
					throw (LinkageError) ex;
				}
				else
				{
					throw new LinkageError(ex.Message, ex);
				}
			}
			throw new LinkageError("no such method " + defc.Name + "." + name + type);
		}
		private static MethodType FixMethodType(Class callerClass, Object type)
		{
			if (type is MethodType)
			{
				return (MethodType) type;
			}
			else
			{
				return MethodType.FromMethodDescriptorString((String)type, callerClass.ClassLoader);
			}
		}
		// Tracing logic:
		internal static MemberName LinkMethodTracing(Class callerClass, int refKind, Class defc, String name, Object type, Object[] appendixResult)
		{
			System.Console.WriteLine("linkMethod " + defc.Name + "." + name + type + "/" + refKind.ToString("x"));
			try
			{
				MemberName res = LinkMethodImpl(callerClass, refKind, defc, name, type, appendixResult);
				System.Console.WriteLine("linkMethod => " + res + " + " + appendixResult[0]);
				return res;
			}
			catch (Throwable ex)
			{
				System.Console.WriteLine("linkMethod => throw " + ex);
				throw ex;
			}
		}


		/// <summary>
		/// The JVM is resolving a CONSTANT_MethodHandle CP entry.  And it wants our help.
		/// It will make an up-call to this method.  (Do not change the name or signature.)
		/// The type argument is a Class for field requests and a MethodType for non-fields.
		/// <para>
		/// Recent versions of the JVM may also pass a resolved MemberName for the type.
		/// In that case, the name is ignored and may be null.
		/// </para>
		/// </summary>
		internal static MethodHandle LinkMethodHandleConstant(Class callerClass, int refKind, Class defc, String name, Object type)
		{
			try
			{
				Lookup lookup = IMPL_LOOKUP.@in(callerClass);
				assert(RefKindIsValid(refKind));
				return lookup.linkMethodHandleConstant((sbyte) refKind, defc, name, type);
			}
			catch (IllegalAccessException ex)
			{
				Throwable cause = ex.InnerException;
				if (cause is AbstractMethodError)
				{
					throw (AbstractMethodError) cause;
				}
				else
				{
					Error err = new IllegalAccessError(ex.Message);
					throw InitCauseFrom(err, ex);
				}
			}
			catch (NoSuchMethodException ex)
			{
				Error err = new NoSuchMethodError(ex.Message);
				throw InitCauseFrom(err, ex);
			}
			catch (NoSuchFieldException ex)
			{
				Error err = new NoSuchFieldError(ex.Message);
				throw InitCauseFrom(err, ex);
			}
			catch (ReflectiveOperationException ex)
			{
				Error err = new IncompatibleClassChangeError();
				throw InitCauseFrom(err, ex);
			}
		}

		/// <summary>
		/// Use best possible cause for err.initCause(), substituting the
		/// cause for err itself if the cause has the same (or better) type.
		/// </summary>
		private static Error InitCauseFrom(Error err, Exception ex)
		{
			Throwable th = ex.Cause;
			if (err.GetType().IsInstanceOfType(th))
			{
			   return (Error) th;
			}
			err.InitCause(th == null ? ex : th);
			return err;
		}

		/// <summary>
		/// Is this method a caller-sensitive method?
		/// I.e., does it call Reflection.getCallerClass or a similer method
		/// to ask about the identity of its caller?
		/// </summary>
		internal static bool IsCallerSensitive(MemberName mem)
		{
			if (!mem.Invocable) // fields are not caller sensitive
			{
				return false;
			}

			return mem.CallerSensitive || CanBeCalledVirtual(mem);
		}

		internal static bool CanBeCalledVirtual(MemberName mem)
		{
			assert(mem.Invocable);
			Class defc = mem.DeclaringClass;
			switch (mem.Name)
			{
			case "checkMemberAccess":
				return CanBeCalledVirtual(mem, typeof(java.lang.SecurityManager));
			case "getContextClassLoader":
				return CanBeCalledVirtual(mem, typeof(Thread));
			}
			return false;
		}

		internal static bool CanBeCalledVirtual(MemberName symbolicRef, Class definingClass)
		{
			Class symbolicRefClass = symbolicRef.DeclaringClass;
			if (symbolicRefClass == definingClass)
			{
				return true;
			}
			if (symbolicRef.Static || symbolicRef.Private)
			{
				return false;
			}
			return (symbolicRefClass.IsSubclassOf(definingClass) || symbolicRefClass.Interface); // Mdef implements Msym -  Msym overrides Mdef
		}
	}

}