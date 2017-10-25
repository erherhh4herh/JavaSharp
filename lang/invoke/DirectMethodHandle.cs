using System.Threading;

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

	using Unsafe = sun.misc.Unsafe;
	using VerifyAccess = sun.invoke.util.VerifyAccess;
	using ValueConversions = sun.invoke.util.ValueConversions;
	using VerifyType = sun.invoke.util.VerifyType;
	using Wrapper = sun.invoke.util.Wrapper;

	/// <summary>
	/// The flavor of method handle which implements a constant reference
	/// to a class member.
	/// @author jrose
	/// </summary>
	internal class DirectMethodHandle : MethodHandle
	{
		internal readonly MemberName Member;

		// Constructors and factory methods in this class *must* be package scoped or private.
		private DirectMethodHandle(MethodType mtype, LambdaForm form, MemberName member) : base(mtype, form)
		{
			if (!member.Resolved)
			{
				throw new InternalError();
			}

			if (member.DeclaringClass.Interface && member.Method && !member.Abstract)
			{
				// Check for corner case: invokeinterface of Object method
				MemberName m = new MemberName(typeof(Object), member.Name, member.MethodType, member.ReferenceKind);
				m = MemberName.Factory.ResolveOrNull(m.ReferenceKind, m, null);
				if (m != null && m.Public)
				{
					assert(member.ReferenceKind == m.ReferenceKind); // else this.form is wrong
					member = m;
				}
			}

			this.Member = member;
		}

		// Factory methods:
		internal static DirectMethodHandle Make(sbyte refKind, Class receiver, MemberName member)
		{
			MethodType mtype = member.MethodOrFieldType;
			if (!member.Static)
			{
				if (!receiver.IsSubclassOf(member.DeclaringClass) || member.Constructor)
				{
					throw new InternalError(member.ToString());
				}
				mtype = mtype.InsertParameterTypes(0, receiver);
			}
			if (!member.Field)
			{
				if (refKind == REF_invokeSpecial)
				{
					member = member.AsSpecial();
					LambdaForm lform = PreparedLambdaForm(member);
					return new Special(mtype, lform, member);
				}
				else
				{
					LambdaForm lform = PreparedLambdaForm(member);
					return new DirectMethodHandle(mtype, lform, member);
				}
			}
			else
			{
				LambdaForm lform = PreparedFieldLambdaForm(member);
				if (member.Static)
				{
					long offset = MethodHandleNatives.staticFieldOffset(member);
					Object @base = MethodHandleNatives.staticFieldBase(member);
					return new StaticAccessor(mtype, lform, member, @base, offset);
				}
				else
				{
					long offset = MethodHandleNatives.objectFieldOffset(member);
					assert(offset == (int)offset);
					return new Accessor(mtype, lform, member, (int)offset);
				}
			}
		}
		internal static DirectMethodHandle Make(Class receiver, MemberName member)
		{
			sbyte refKind = member.ReferenceKind;
			if (refKind == REF_invokeSpecial)
			{
				refKind = REF_invokeVirtual;
			}
			return Make(refKind, receiver, member);
		}
		internal static DirectMethodHandle Make(MemberName member)
		{
			if (member.Constructor)
			{
				return MakeAllocator(member);
			}
			return Make(member.DeclaringClass, member);
		}
		internal static DirectMethodHandle Make(Method method)
		{
			return Make(method.DeclaringClass, new MemberName(method));
		}
		internal static DirectMethodHandle Make(Field field)
		{
			return Make(field.DeclaringClass, new MemberName(field));
		}
		private static DirectMethodHandle MakeAllocator(MemberName ctor)
		{
			assert(ctor.Constructor && ctor.Name.Equals("<init>"));
			Class instanceClass = ctor.DeclaringClass;
			ctor = ctor.AsConstructor();
			assert(ctor.Constructor && ctor.ReferenceKind == REF_newInvokeSpecial) : ctor;
			MethodType mtype = ctor.MethodType.ChangeReturnType(instanceClass);
			LambdaForm lform = PreparedLambdaForm(ctor);
			MemberName init = ctor.AsSpecial();
			assert(init.MethodType.ReturnType() == typeof(void));
			return new Constructor(mtype, lform, ctor, init, instanceClass);
		}

		internal override BoundMethodHandle Rebind()
		{
			return BoundMethodHandle.MakeReinvoker(this);
		}

		internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
		{
			assert(this.GetType() == typeof(DirectMethodHandle)); // must override in subclasses
			return new DirectMethodHandle(mt, lf, Member);
		}

		internal override String InternalProperties()
		{
			return "\n& DMH.MN=" + InternalMemberName();
		}

		//// Implementation methods.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @ForceInline MemberName internalMemberName()
		internal override MemberName InternalMemberName()
		{
			return Member;
		}

		private static readonly MemberName.Factory IMPL_NAMES = MemberName.Factory;

		/// <summary>
		/// Create a LF which can invoke the given method.
		/// Cache and share this structure among all methods with
		/// the same basicType and refKind.
		/// </summary>
		private static LambdaForm PreparedLambdaForm(MemberName m)
		{
			assert(m.Invocable) : m; // call preparedFieldLambdaForm instead
			MethodType mtype = m.InvocationType.BasicType();
			assert(!m.MethodHandleInvoke || "invokeBasic".Equals(m.Name)) : m;
			int which;
			switch (m.ReferenceKind)
			{
			case REF_invokeVirtual:
				which = LF_INVVIRTUAL;
				break;
			case REF_invokeStatic:
				which = LF_INVSTATIC;
				break;
			case REF_invokeSpecial:
				which = LF_INVSPECIAL;
				break;
			case REF_invokeInterface:
				which = LF_INVINTERFACE;
				break;
			case REF_newInvokeSpecial:
				which = LF_NEWINVSPECIAL;
				break;
			default:
				throw new InternalError(m.ToString());
			}
			if (which == LF_INVSTATIC && ShouldBeInitialized(m))
			{
				// precompute the barrier-free version:
				PreparedLambdaForm(mtype, which);
				which = LF_INVSTATIC_INIT;
			}
			LambdaForm lform = PreparedLambdaForm(mtype, which);
			MaybeCompile(lform, m);
			assert(lform.MethodType().DropParameterTypes(0, 1).Equals(m.InvocationType.BasicType())) : Arrays.asList(m, m.InvocationType.BasicType(), lform, lform.MethodType());
			return lform;
		}

		private static LambdaForm PreparedLambdaForm(MethodType mtype, int which)
		{
			LambdaForm lform = mtype.Form().CachedLambdaForm(which);
			if (lform != null)
			{
				return lform;
			}
			lform = MakePreparedLambdaForm(mtype, which);
			return mtype.Form().SetCachedLambdaForm(which, lform);
		}

		private static LambdaForm MakePreparedLambdaForm(MethodType mtype, int which)
		{
			bool needsInit = (which == LF_INVSTATIC_INIT);
			bool doesAlloc = (which == LF_NEWINVSPECIAL);
			String linkerName, lambdaName;
			switch (which)
			{
			case LF_INVVIRTUAL:
				linkerName = "linkToVirtual";
				lambdaName = "DMH.invokeVirtual";
				break;
			case LF_INVSTATIC:
				linkerName = "linkToStatic";
				lambdaName = "DMH.invokeStatic";
				break;
			case LF_INVSTATIC_INIT:
				linkerName = "linkToStatic";
				lambdaName = "DMH.invokeStaticInit";
				break;
			case LF_INVSPECIAL:
				linkerName = "linkToSpecial";
				lambdaName = "DMH.invokeSpecial";
				break;
			case LF_INVINTERFACE:
				linkerName = "linkToInterface";
				lambdaName = "DMH.invokeInterface";
				break;
			case LF_NEWINVSPECIAL:
				linkerName = "linkToSpecial";
				lambdaName = "DMH.newInvokeSpecial";
				break;
			default:
				throw new InternalError("which=" + which);
			}
			MethodType mtypeWithArg = mtype.AppendParameterTypes(typeof(MemberName));
			if (doesAlloc)
			{
				mtypeWithArg = mtypeWithArg.InsertParameterTypes(0, typeof(Object)).ChangeReturnType(typeof(void)); // <init> returns void -  insert newly allocated obj
			}
			MemberName linker = new MemberName(typeof(MethodHandle), linkerName, mtypeWithArg, REF_invokeStatic);
			try
			{
				linker = IMPL_NAMES.ResolveOrFail(REF_invokeStatic, linker, null, typeof(NoSuchMethodException));
			}
			catch (ReflectiveOperationException ex)
			{
				throw newInternalError(ex);
			}
			const int DMH_THIS = 0;
			const int ARG_BASE = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_LIMIT = ARG_BASE + mtype.parameterCount();
			int ARG_LIMIT = ARG_BASE + mtype.ParameterCount();
			int nameCursor = ARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int NEW_OBJ = (doesAlloc ? nameCursor++ : -1);
			int NEW_OBJ = (doesAlloc ? nameCursor++: -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_MEMBER = nameCursor++;
			int GET_MEMBER = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int LINKER_CALL = nameCursor++;
			int LINKER_CALL = nameCursor++;
			Name[] names = arguments(nameCursor - ARG_LIMIT, mtype.InvokerType());
			assert(names.Length == nameCursor);
			if (doesAlloc)
			{
				// names = { argx,y,z,... new C, init method }
				names[NEW_OBJ] = new Name(Lazy.NF_allocateInstance, names[DMH_THIS]);
				names[GET_MEMBER] = new Name(Lazy.NF_constructorMethod, names[DMH_THIS]);
			}
			else if (needsInit)
			{
				names[GET_MEMBER] = new Name(Lazy.NF_internalMemberNameEnsureInit, names[DMH_THIS]);
			}
			else
			{
				names[GET_MEMBER] = new Name(Lazy.NF_internalMemberName, names[DMH_THIS]);
			}
			assert(FindDirectMethodHandle(names[GET_MEMBER]) == names[DMH_THIS]);
			Object[] outArgs = Arrays.CopyOfRange(names, ARG_BASE, GET_MEMBER + 1, typeof(Object[]));
			assert(outArgs[outArgs.Length - 1] == names[GET_MEMBER]); // look, shifted args!
			int result = LAST_RESULT;
			if (doesAlloc)
			{
				assert(outArgs[outArgs.Length - 2] == names[NEW_OBJ]); // got to move this one
				System.Array.Copy(outArgs, 0, outArgs, 1, outArgs.Length - 2);
				outArgs[0] = names[NEW_OBJ];
				result = NEW_OBJ;
			}
			names[LINKER_CALL] = new Name(linker, outArgs);
			lambdaName += "_" + shortenSignature(basicTypeSignature(mtype));
			LambdaForm lform = new LambdaForm(lambdaName, ARG_LIMIT, names, result);
			// This is a tricky bit of code.  Don't send it through the LF interpreter.
			lform.CompileToBytecode();
			return lform;
		}

		internal static Object FindDirectMethodHandle(Name name)
		{
			if (name.function == Lazy.NF_internalMemberName || name.function == Lazy.NF_internalMemberNameEnsureInit || name.function == Lazy.NF_constructorMethod)
			{
				assert(name.arguments.length == 1);
				return name.arguments[0];
			}
			return null;
		}

		private static void MaybeCompile(LambdaForm lform, MemberName m)
		{
			if (VerifyAccess.isSamePackage(m.DeclaringClass, typeof(MethodHandle)))
			{
				// Help along bootstrapping...
				lform.CompileToBytecode();
			}
		}

		/// <summary>
		/// Static wrapper for DirectMethodHandle.internalMemberName. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static Object internalMemberName(Object mh)
		internal static Object InternalMemberName(Object mh)
		/*non-public*/
		{
			return ((DirectMethodHandle)mh).Member;
		}

		/// <summary>
		/// Static wrapper for DirectMethodHandle.internalMemberName.
		/// This one also forces initialization.
		/// </summary>
		/*non-public*/	 internal static Object InternalMemberNameEnsureInit(Object mh)
	 {
			DirectMethodHandle dmh = (DirectMethodHandle)mh;
			dmh.EnsureInitialized();
			return dmh.Member;
	 }

		/*non-public*/	 internal static bool ShouldBeInitialized(MemberName member)
	 {
			switch (member.ReferenceKind)
			{
			case REF_invokeStatic:
			case REF_getStatic:
			case REF_putStatic:
			case REF_newInvokeSpecial:
				break;
			default:
				// No need to initialize the class on this kind of member.
				return false;
			}
			Class cls = member.DeclaringClass;
			if (cls == typeof(ValueConversions) || cls == typeof(MethodHandleImpl) || cls == typeof(Invokers))
			{
				// These guys have lots of <clinit> DMH creation but we know
				// the MHs will not be used until the system is booted.
				return false;
			}
			if (VerifyAccess.isSamePackage(typeof(MethodHandle), cls) || VerifyAccess.isSamePackage(typeof(ValueConversions), cls))
			{
				// It is a system class.  It is probably in the process of
				// being initialized, but we will help it along just to be safe.
				if (UNSAFE.shouldBeInitialized(cls))
				{
					UNSAFE.ensureClassInitialized(cls);
				}
				return false;
			}
			return UNSAFE.shouldBeInitialized(cls);
	 }

		private class EnsureInitialized : ClassValue<WeakReference<Thread>>
		{
			protected internal override WeakReference<Thread> ComputeValue(Class type)
			{
				UNSAFE.ensureClassInitialized(type);
				if (UNSAFE.shouldBeInitialized(type))
					// If the previous call didn't block, this can happen.
					// We are executing inside <clinit>.
				{
					return new WeakReference<>(Thread.CurrentThread);
				}
				return null;
			}
			internal static readonly EnsureInitialized INSTANCE = new EnsureInitialized();
		}

		private void EnsureInitialized()
		{
			if (CheckInitialized(Member))
			{
				// The coast is clear.  Delete the <clinit> barrier.
				if (Member.Field)
				{
					UpdateForm(PreparedFieldLambdaForm(Member));
				}
				else
				{
					UpdateForm(PreparedLambdaForm(Member));
				}
			}
		}
		private static bool CheckInitialized(MemberName member)
		{
			Class defc = member.DeclaringClass;
			WeakReference<Thread> @ref = EnsureInitialized.INSTANCE.Get(defc);
			if (@ref == null)
			{
				return true; // the final state
			}
			Thread clinitThread = @ref.get();
			// Somebody may still be running defc.<clinit>.
			if (clinitThread == Thread.CurrentThread)
			{
				// If anybody is running defc.<clinit>, it is this thread.
				if (UNSAFE.shouldBeInitialized(defc))
				{
					// Yes, we are running it; keep the barrier for now.
					return false;
				}
			}
			else
			{
				// We are in a random thread.  Block.
				UNSAFE.ensureClassInitialized(defc);
			}
			assert(!UNSAFE.shouldBeInitialized(defc));
			// put it into the final state
			EnsureInitialized.INSTANCE.Remove(defc);
			return true;
		}

		/*non-public*/	 internal static void EnsureInitialized(Object mh)
	 {
			((DirectMethodHandle)mh).EnsureInitialized();
	 }

		/// <summary>
		/// This subclass represents invokespecial instructions. </summary>
		internal class Special : DirectMethodHandle
		{
			internal Special(MethodType mtype, LambdaForm form, MemberName member) : base(mtype, form, member)
			{
			}
			internal override bool InvokeSpecial
			{
				get
				{
					return true;
				}
			}
			internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
			{
				return new Special(mt, lf, Member);
			}
		}

		/// <summary>
		/// This subclass handles constructor references. </summary>
		internal class Constructor : DirectMethodHandle
		{
			internal readonly MemberName InitMethod;
			internal readonly Class InstanceClass;

			internal Constructor(MethodType mtype, LambdaForm form, MemberName constructor, MemberName initMethod, Class instanceClass) : base(mtype, form, constructor)
			{
				this.InitMethod = initMethod;
				this.InstanceClass = instanceClass;
				assert(initMethod.Resolved);
			}
			internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
			{
				return new Constructor(mt, lf, Member, InitMethod, InstanceClass);
			}
		}

		/*non-public*/	 internal static Object ConstructorMethod(Object mh)
	 {
			Constructor dmh = (Constructor)mh;
			return dmh.InitMethod;
	 }

		/*non-public*///JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object allocateInstance(Object mh) throws InstantiationException
	 internal static Object AllocateInstance(Object mh)
	 {
			Constructor dmh = (Constructor)mh;
			return UNSAFE.allocateInstance(dmh.InstanceClass);
	 }

		/// <summary>
		/// This subclass handles non-static field references. </summary>
		internal class Accessor : DirectMethodHandle
		{
			internal readonly Class FieldType;
			internal readonly int FieldOffset;
			internal Accessor(MethodType mtype, LambdaForm form, MemberName member, int fieldOffset) : base(mtype, form, member)
			{
				this.FieldType = member.FieldType;
				this.FieldOffset = fieldOffset;
			}

			internal override Object CheckCast(Object obj)
			{
				return FieldType.Cast(obj);
			}
			internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
			{
				return new Accessor(mt, lf, Member, FieldOffset);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static long fieldOffset(Object accessorObj)
		internal static long FieldOffset(Object accessorObj)
		/*non-public*/
		{
			// Note: We return a long because that is what Unsafe.getObject likes.
			// We store a plain int because it is more compact.
			return ((Accessor)accessorObj).FieldOffset;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static Object checkBase(Object obj)
		internal static Object CheckBase(Object obj)
		/*non-public*/
		{
			// Note that the object's class has already been verified,
			// since the parameter type of the Accessor method handle
			// is either member.getDeclaringClass or a subclass.
			// This was verified in DirectMethodHandle.make.
			// Therefore, the only remaining check is for null.
			// Since this check is *not* guaranteed by Unsafe.getInt
			// and its siblings, we need to make an explicit one here.
			obj.GetType(); // maybe throw NPE
			return obj;
		}

		/// <summary>
		/// This subclass handles static field references. </summary>
		internal class StaticAccessor : DirectMethodHandle
		{
			internal readonly Class FieldType;
			internal readonly Object StaticBase;
			internal readonly long StaticOffset;

			internal StaticAccessor(MethodType mtype, LambdaForm form, MemberName member, Object staticBase, long staticOffset) : base(mtype, form, member)
			{
				this.FieldType = member.FieldType;
				this.StaticBase = staticBase;
				this.StaticOffset = staticOffset;
			}

			internal override Object CheckCast(Object obj)
			{
				return FieldType.Cast(obj);
			}
			internal override MethodHandle CopyWith(MethodType mt, LambdaForm lf)
			{
				return new StaticAccessor(mt, lf, Member, StaticBase, StaticOffset);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static Object nullCheck(Object obj)
		internal static Object NullCheck(Object obj)
		/*non-public*/
		{
			obj.GetType();
			return obj;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static Object staticBase(Object accessorObj)
		internal static Object StaticBase(Object accessorObj)
		/*non-public*/
		{
			return ((StaticAccessor)accessorObj).StaticBase;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static long staticOffset(Object accessorObj)
		internal static long StaticOffset(Object accessorObj)
		/*non-public*/
		{
			return ((StaticAccessor)accessorObj).StaticOffset;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline static Object checkCast(Object mh, Object obj)
		internal static Object CheckCast(Object mh, Object obj)
		/*non-public*/
		{
			return ((DirectMethodHandle) mh).CheckCast(obj);
		}

		internal virtual Object CheckCast(Object obj)
		{
			return Member.ReturnType.Cast(obj);
		}

		// Caching machinery for field accessors:
		private static sbyte AF_GETFIELD = 0, AF_PUTFIELD = 1, AF_GETSTATIC = 2, AF_PUTSTATIC = 3, AF_GETSTATIC_INIT = 4, AF_PUTSTATIC_INIT = 5, AF_LIMIT = 6;
		// Enumerate the different field kinds using Wrapper,
		// with an extra case added for checked references.
		private static int FT_LAST_WRAPPER = Wrapper.values().length - 1, FT_UNCHECKED_REF = Wrapper.OBJECT.ordinal(), FT_CHECKED_REF = FT_LAST_WRAPPER + 1, FT_LIMIT = FT_LAST_WRAPPER + 2;
		private static int AfIndex(sbyte formOp, bool isVolatile, int ftypeKind)
		{
			return ((formOp * FT_LIMIT * 2) + (isVolatile ? FT_LIMIT : 0) + ftypeKind);
		}
		private static readonly LambdaForm[] ACCESSOR_FORMS = new LambdaForm[AfIndex(AF_LIMIT, false, 0)];
		private static int FtypeKind(Class ftype)
		{
			if (ftype.Primitive)
			{
				return Wrapper.forPrimitiveType(ftype).ordinal();
			}
			else if (VerifyType.isNullReferenceConversion(typeof(Object), ftype))
			{
				return FT_UNCHECKED_REF;
			}
			else
			{
				return FT_CHECKED_REF;
			}
		}

		/// <summary>
		/// Create a LF which can access the given field.
		/// Cache and share this structure among all fields with
		/// the same basicType and refKind.
		/// </summary>
		private static LambdaForm PreparedFieldLambdaForm(MemberName m)
		{
			Class ftype = m.FieldType;
			bool isVolatile = m.Volatile;
			sbyte formOp;
			switch (m.ReferenceKind)
			{
			case REF_getField:
				formOp = AF_GETFIELD;
				break;
			case REF_putField:
				formOp = AF_PUTFIELD;
				break;
			case REF_getStatic:
				formOp = AF_GETSTATIC;
				break;
			case REF_putStatic:
				formOp = AF_PUTSTATIC;
				break;
			default:
				throw new InternalError(m.ToString());
			}
			if (ShouldBeInitialized(m))
			{
				// precompute the barrier-free version:
				PreparedFieldLambdaForm(formOp, isVolatile, ftype);
				assert((AF_GETSTATIC_INIT - AF_GETSTATIC) == (AF_PUTSTATIC_INIT - AF_PUTSTATIC));
				formOp += (sbyte)(AF_GETSTATIC_INIT - AF_GETSTATIC);
			}
			LambdaForm lform = PreparedFieldLambdaForm(formOp, isVolatile, ftype);
			MaybeCompile(lform, m);
			assert(lform.MethodType().DropParameterTypes(0, 1).Equals(m.InvocationType.BasicType())) : Arrays.asList(m, m.InvocationType.BasicType(), lform, lform.MethodType());
			return lform;
		}
		private static LambdaForm PreparedFieldLambdaForm(sbyte formOp, bool isVolatile, Class ftype)
		{
			int afIndex = AfIndex(formOp, isVolatile, FtypeKind(ftype));
			LambdaForm lform = ACCESSOR_FORMS[afIndex];
			if (lform != null)
			{
				return lform;
			}
			lform = MakePreparedFieldLambdaForm(formOp, isVolatile, FtypeKind(ftype));
			ACCESSOR_FORMS[afIndex] = lform; // don't bother with a CAS
			return lform;
		}

		private static LambdaForm MakePreparedFieldLambdaForm(sbyte formOp, bool isVolatile, int ftypeKind)
		{
			bool isGetter = (formOp & 1) == (AF_GETFIELD & 1);
			bool isStatic = (formOp >= AF_GETSTATIC);
			bool needsInit = (formOp >= AF_GETSTATIC_INIT);
			bool needsCast = (ftypeKind == FT_CHECKED_REF);
			Wrapper fw = (needsCast ? Wrapper.OBJECT : Wrapper.values()[ftypeKind]);
			Class ft = fw.primitiveType();
			assert(FtypeKind(needsCast ? typeof(String) : ft) == ftypeKind);
			String tname = fw.primitiveSimpleName();
			String ctname = char.ToUpper(tname.CharAt(0)) + tname.Substring(1);
			if (isVolatile)
			{
				ctname += "Volatile";
			}
			String getOrPut = (isGetter ? "get" : "put");
			String linkerName = (getOrPut + ctname); // getObject, putIntVolatile, etc.
			MethodType linkerType;
			if (isGetter)
			{
				linkerType = MethodType.MethodType(ft, typeof(Object), typeof(long));
			}
			else
			{
				linkerType = MethodType.methodType(typeof(void), typeof(Object), typeof(long), ft);
			}
			MemberName linker = new MemberName(typeof(Unsafe), linkerName, linkerType, REF_invokeVirtual);
			try
			{
				linker = IMPL_NAMES.ResolveOrFail(REF_invokeVirtual, linker, null, typeof(NoSuchMethodException));
			}
			catch (ReflectiveOperationException ex)
			{
				throw newInternalError(ex);
			}

			// What is the external type of the lambda form?
			MethodType mtype;
			if (isGetter)
			{
				mtype = MethodType.MethodType(ft);
			}
			else
			{
				mtype = MethodType.MethodType(typeof(void), ft);
			}
			mtype = mtype.BasicType(); // erase short to int, etc.
			if (!isStatic)
			{
				mtype = mtype.InsertParameterTypes(0, typeof(Object));
			}
			const int DMH_THIS = 0;
			const int ARG_BASE = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_LIMIT = ARG_BASE + mtype.parameterCount();
			int ARG_LIMIT = ARG_BASE + mtype.ParameterCount();
			// if this is for non-static access, the base pointer is stored at this index:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int OBJ_BASE = isStatic ? -1 : ARG_BASE;
			int OBJ_BASE = isStatic ? - 1 : ARG_BASE;
			// if this is for write access, the value to be written is stored at this index:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int SET_VALUE = isGetter ? -1 : ARG_LIMIT - 1;
			int SET_VALUE = isGetter ? - 1 : ARG_LIMIT - 1;
			int nameCursor = ARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int F_HOLDER = (isStatic ? nameCursor++ : -1);
			int F_HOLDER = (isStatic ? nameCursor++: -1); // static base if any
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int F_OFFSET = nameCursor++;
			int F_OFFSET = nameCursor++; // Either static offset or field offset.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int OBJ_CHECK = (OBJ_BASE >= 0 ? nameCursor++ : -1);
			int OBJ_CHECK = (OBJ_BASE >= 0 ? nameCursor++: -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INIT_BAR = (needsInit ? nameCursor++ : -1);
			int INIT_BAR = (needsInit ? nameCursor++: -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int PRE_CAST = (needsCast && !isGetter ? nameCursor++ : -1);
			int PRE_CAST = (needsCast && !isGetter ? nameCursor++: -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int LINKER_CALL = nameCursor++;
			int LINKER_CALL = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int POST_CAST = (needsCast && isGetter ? nameCursor++ : -1);
			int POST_CAST = (needsCast && isGetter ? nameCursor++: -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int RESULT = nameCursor-1;
			int RESULT = nameCursor - 1; // either the call or the cast
			Name[] names = arguments(nameCursor - ARG_LIMIT, mtype.InvokerType());
			if (needsInit)
			{
				names[INIT_BAR] = new Name(Lazy.NF_ensureInitialized, names[DMH_THIS]);
			}
			if (needsCast && !isGetter)
			{
				names[PRE_CAST] = new Name(Lazy.NF_checkCast, names[DMH_THIS], names[SET_VALUE]);
			}
			Object[] outArgs = new Object[1 + linkerType.ParameterCount()];
			assert(outArgs.Length == (isGetter ? 3 : 4));
			outArgs[0] = UNSAFE;
			if (isStatic)
			{
				outArgs[1] = names[F_HOLDER] = new Name(Lazy.NF_staticBase, names[DMH_THIS]);
				outArgs[2] = names[F_OFFSET] = new Name(Lazy.NF_staticOffset, names[DMH_THIS]);
			}
			else
			{
				outArgs[1] = names[OBJ_CHECK] = new Name(Lazy.NF_checkBase, names[OBJ_BASE]);
				outArgs[2] = names[F_OFFSET] = new Name(Lazy.NF_fieldOffset, names[DMH_THIS]);
			}
			if (!isGetter)
			{
				outArgs[3] = (needsCast ? names[PRE_CAST] : names[SET_VALUE]);
			}
			foreach (Object a in outArgs)
			{
				assert(a != null);
			}
			names[LINKER_CALL] = new Name(linker, outArgs);
			if (needsCast && isGetter)
			{
				names[POST_CAST] = new Name(Lazy.NF_checkCast, names[DMH_THIS], names[LINKER_CALL]);
			}
			foreach (Name n in names)
			{
				assert(n != null);
			}
			String fieldOrStatic = (isStatic ? "Static" : "Field");
			String lambdaName = (linkerName + fieldOrStatic); // significant only for debugging
			if (needsCast)
			{
				lambdaName += "Cast";
			}
			if (needsInit)
			{
				lambdaName += "Init";
			}
			return new LambdaForm(lambdaName, ARG_LIMIT, names, RESULT);
		}

		/// <summary>
		/// Pre-initialized NamedFunctions for bootstrapping purposes.
		/// Factored in an inner class to delay initialization until first usage.
		/// </summary>
		private class Lazy
		{
			internal static readonly NamedFunction NF_internalMemberName, NF_internalMemberNameEnsureInit, NF_ensureInitialized, NF_fieldOffset, NF_checkBase, NF_staticBase, NF_staticOffset, NF_checkCast, NF_allocateInstance, NF_constructorMethod;
			static Lazy()
			{
				try
				{
					NamedFunction[] nfs = new NamedFunction[] {NF_internalMemberName = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("internalMemberName", typeof(Object))), NF_internalMemberNameEnsureInit = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("internalMemberNameEnsureInit", typeof(Object))), NF_ensureInitialized = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("ensureInitialized", typeof(Object))), NF_fieldOffset = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("fieldOffset", typeof(Object))), NF_checkBase = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("checkBase", typeof(Object))), NF_staticBase = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("staticBase", typeof(Object))), NF_staticOffset = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("staticOffset", typeof(Object))), NF_checkCast = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("checkCast", typeof(Object), typeof(Object))), NF_allocateInstance = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("allocateInstance", typeof(Object))), NF_constructorMethod = new NamedFunction(typeof(DirectMethodHandle).getDeclaredMethod("constructorMethod", typeof(Object)))};
					foreach (NamedFunction nf in nfs)
					{
						// Each nf must be statically invocable or we get tied up in our bootstraps.
						assert(InvokerBytecodeGenerator.IsStaticallyInvocable(nf.member)) : nf;
						nf.resolve();
					}
				}
				catch (ReflectiveOperationException ex)
				{
					throw newInternalError(ex);
				}
			}
		}
	}

}