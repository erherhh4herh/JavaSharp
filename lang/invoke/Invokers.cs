using System;

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
	/// Construction and caching of often-used invokers.
	/// @author jrose
	/// </summary>
	internal class Invokers
	{
		// exact type (sans leading taget MH) for the outgoing call
		private readonly MethodType TargetType;

		// Cached adapter information:
		private readonly  Stable;
		// Indexes into invokers:
		internal const int INV_EXACT = 0, INV_GENERIC = 1, INV_BASIC = 2, INV_LIMIT = 3; // MethodHandles.basicInvoker -  MethodHandles.invoker (generic invocation) -  MethodHandles.exactInvoker

		/// <summary>
		/// Compute and cache information common to all collecting adapters
		///  that implement members of the erasure-family of the given erased type.
		/// </summary>
		/*non-public*/	 internal Invokers(MethodType targetType)
	 {
			this.TargetType = targetType;
	 }

		/*non-public*/	 internal virtual MethodHandle ExactInvoker()
	 {
			MethodHandle invoker = CachedInvoker(INV_EXACT);
			if (invoker != null)
			{
				return invoker;
			}
			invoker = MakeExactOrGeneralInvoker(true);
			return SetCachedInvoker(INV_EXACT, invoker);
	 }

		/*non-public*/	 internal virtual MethodHandle GenericInvoker()
	 {
			MethodHandle invoker = CachedInvoker(INV_GENERIC);
			if (invoker != null)
			{
				return invoker;
			}
			invoker = MakeExactOrGeneralInvoker(false);
			return SetCachedInvoker(INV_GENERIC, invoker);
	 }

		/*non-public*/	 internal virtual MethodHandle BasicInvoker()
	 {
			MethodHandle invoker = CachedInvoker(INV_BASIC);
			if (invoker != null)
			{
				return invoker;
			}
			MethodType basicType = TargetType.BasicType();
			if (basicType != TargetType)
			{
				// double cache; not used significantly
				return SetCachedInvoker(INV_BASIC, basicType.Invokers().BasicInvoker());
			}
			invoker = basicType.Form().CachedMethodHandle(MethodTypeForm.MH_BASIC_INV);
			if (invoker == null)
			{
				MemberName method = InvokeBasicMethod(basicType);
				invoker = DirectMethodHandle.Make(method);
				assert(CheckInvoker(invoker));
				invoker = basicType.Form().SetCachedMethodHandle(MethodTypeForm.MH_BASIC_INV, invoker);
			}
			return SetCachedInvoker(INV_BASIC, invoker);
	 }

		private MethodHandle CachedInvoker(int idx)
		{
			return invokers[idx];
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private synchronized MethodHandle setCachedInvoker(int idx, final MethodHandle invoker)
		private MethodHandle SetCachedInvoker(int idx, MethodHandle invoker)
		{
			lock (this)
			{
				// Simulate a CAS, to avoid racy duplication of results.
				MethodHandle prev = invokers[idx];
				if (prev != null)
				{
					return prev;
				}
				return invokers[idx] = invoker;
			}
		}

		private MethodHandle MakeExactOrGeneralInvoker(bool isExact)
		{
			MethodType mtype = TargetType;
			MethodType invokerType = mtype.InvokerType();
			int which = (isExact ? MethodTypeForm.LF_EX_INVOKER : MethodTypeForm.LF_GEN_INVOKER);
			LambdaForm lform = InvokeHandleForm(mtype, false, which);
			MethodHandle invoker = BoundMethodHandle.BindSingle(invokerType, lform, mtype);
			String whichName = (isExact ? "invokeExact" : "invoke");
			invoker = invoker.WithInternalMemberName(MemberName.MakeMethodHandleInvoke(whichName, mtype), false);
			assert(CheckInvoker(invoker));
			MaybeCompileToBytecode(invoker);
			return invoker;
		}

		/// <summary>
		/// If the target type seems to be common enough, eagerly compile the invoker to bytecodes. </summary>
		private void MaybeCompileToBytecode(MethodHandle invoker)
		{
			const int EAGER_COMPILE_ARITY_LIMIT = 10;
			if (TargetType == TargetType.Erase() && TargetType.ParameterCount() < EAGER_COMPILE_ARITY_LIMIT)
			{
				invoker.Form.CompileToBytecode();
			}
		}

		// This next one is called from LambdaForm.NamedFunction.<init>.
		/*non-public*/	 internal static MemberName InvokeBasicMethod(MethodType basicType)
	 {
			assert(basicType == basicType.BasicType());
			try
			{
				//Lookup.findVirtual(MethodHandle.class, name, type);
				return IMPL_LOOKUP.resolveOrFail(REF_invokeVirtual, typeof(MethodHandle), "invokeBasic", basicType);
			}
			catch (ReflectiveOperationException ex)
			{
				throw newInternalError("JVM cannot find invoker for " + basicType, ex);
			}
	 }

		private bool CheckInvoker(MethodHandle invoker)
		{
			assert(TargetType.InvokerType().Equals(invoker.Type())) : Arrays.asList(TargetType, TargetType.InvokerType(), invoker);
			assert(invoker.InternalMemberName() == null || invoker.InternalMemberName().MethodType.Equals(TargetType));
			assert(!invoker.VarargsCollector);
			return true;
		}

		/// <summary>
		/// Find or create an invoker which passes unchanged a given number of arguments
		/// and spreads the rest from a trailing array argument.
		/// The invoker target type is the post-spread type {@code (TYPEOF(uarg*), TYPEOF(sarg*))=>RT}.
		/// All the {@code sarg}s must have a common type {@code C}.  (If there are none, {@code Object} is assumed.} </summary>
		/// <param name="leadingArgCount"> the number of unchanged (non-spread) arguments </param>
		/// <returns> {@code invoker.invokeExact(mh, uarg*, C[]{sarg*}) := (RT)mh.invoke(uarg*, sarg*)} </returns>
		/*non-public*/	 internal virtual MethodHandle SpreadInvoker(int leadingArgCount)
	 {
			int spreadArgCount = TargetType.ParameterCount() - leadingArgCount;
			MethodType postSpreadType = TargetType;
			Class argArrayType = ImpliedRestargType(postSpreadType, leadingArgCount);
			if (postSpreadType.ParameterSlotCount() <= MethodType.MAX_MH_INVOKER_ARITY)
			{
				return GenericInvoker().AsSpreader(argArrayType, spreadArgCount);
			}
			// Cannot build a generic invoker here of type ginvoker.invoke(mh, a*[254]).
			// Instead, factor sinvoker.invoke(mh, a) into ainvoker.invoke(filter(mh), a)
			// where filter(mh) == mh.asSpreader(Object[], spreadArgCount)
			MethodType preSpreadType = postSpreadType.ReplaceParameterTypes(leadingArgCount, postSpreadType.ParameterCount(), argArrayType);
			MethodHandle arrayInvoker = MethodHandles.Invoker(preSpreadType);
			MethodHandle makeSpreader = MethodHandles.insertArguments(Lazy.MH_asSpreader, 1, argArrayType, spreadArgCount);
			return MethodHandles.FilterArgument(arrayInvoker, 0, makeSpreader);
	 }

		private static Class ImpliedRestargType(MethodType restargType, int fromPos)
		{
			if (restargType.Generic) // can be nothing else
			{
				return typeof(Object[]);
			}
			int maxPos = restargType.ParameterCount();
			if (fromPos >= maxPos) // reasonable default
			{
				return typeof(Object[]);
			}
			Class argType = restargType.ParameterType(fromPos);
			for (int i = fromPos + 1; i < maxPos; i++)
			{
				if (argType != restargType.ParameterType(i))
				{
					throw newIllegalArgumentException("need homogeneous rest arguments", restargType);
				}
			}
			if (argType == typeof(Object))
			{
				return typeof(Object[]);
			}
			return Array.newInstance(argType, 0).GetType();
		}

		public override String ToString()
		{
			return "Invokers" + TargetType;
		}

		internal static MemberName MethodHandleInvokeLinkerMethod(String name, MethodType mtype, Object[] appendixResult)
		{
			int which;
			switch (name)
			{
			case "invokeExact":
				which = MethodTypeForm.LF_EX_LINKER;
				break;
			case "invoke":
				which = MethodTypeForm.LF_GEN_LINKER;
				break;
			default:
				throw new InternalError("not invoker: " + name);
			}
			LambdaForm lform;
			if (mtype.ParameterSlotCount() <= MethodType.MAX_MH_ARITY - MH_LINKER_ARG_APPENDED)
			{
				lform = InvokeHandleForm(mtype, false, which);
				appendixResult[0] = mtype;
			}
			else
			{
				lform = InvokeHandleForm(mtype, true, which);
			}
			return lform.Vmentry;
		}

		// argument count to account for trailing "appendix value" (typically the mtype)
		private const int MH_LINKER_ARG_APPENDED = 1;

		/// <summary>
		/// Returns an adapter for invokeExact or generic invoke, as a MH or constant pool linker.
		/// If !customized, caller is responsible for supplying, during adapter execution,
		/// a copy of the exact mtype.  This is because the adapter might be generalized to
		/// a basic type. </summary>
		/// <param name="mtype"> the caller's method type (either basic or full-custom) </param>
		/// <param name="customized"> whether to use a trailing appendix argument (to carry the mtype) </param>
		/// <param name="which"> bit-encoded 0x01 whether it is a CP adapter ("linker") or MHs.invoker value ("invoker");
		///                          0x02 whether it is for invokeExact or generic invoke </param>
		private static LambdaForm InvokeHandleForm(MethodType mtype, bool customized, int which)
		{
			bool isCached;
			if (!customized)
			{
				mtype = mtype.BasicType(); // normalize Z to I, String to Object, etc.
				isCached = true;
			}
			else
			{
				isCached = false; // maybe cache if mtype == mtype.basicType()
			}
			bool isLinker, isGeneric;
			String debugName;
			switch (which)
			{
			case MethodTypeForm.LF_EX_LINKER:
				isLinker = true;
				isGeneric = false;
				debugName = "invokeExact_MT";
				break;
			case MethodTypeForm.LF_EX_INVOKER:
				isLinker = false;
				isGeneric = false;
				debugName = "exactInvoker";
				break;
			case MethodTypeForm.LF_GEN_LINKER:
				isLinker = true;
				isGeneric = true;
				debugName = "invoke_MT";
				break;
			case MethodTypeForm.LF_GEN_INVOKER:
				isLinker = false;
				isGeneric = true;
				debugName = "invoker";
				break;
			default:
				throw new InternalError();
			}
			LambdaForm lform;
			if (isCached)
			{
				lform = mtype.Form().CachedLambdaForm(which);
				if (lform != null)
				{
					return lform;
				}
			}
			// exactInvokerForm (Object,Object)Object
			//   link with java.lang.invoke.MethodHandle.invokeBasic(MethodHandle,Object,Object)Object/invokeSpecial
			const int THIS_MH = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CALL_MH = THIS_MH + (isLinker ? 0 : 1);
			int CALL_MH = THIS_MH + (isLinker ? 0 : 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_BASE = CALL_MH + 1;
			int ARG_BASE = CALL_MH + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int OUTARG_LIMIT = ARG_BASE + mtype.parameterCount();
			int OUTARG_LIMIT = ARG_BASE + mtype.ParameterCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INARG_LIMIT = OUTARG_LIMIT + (isLinker && !customized ? 1 : 0);
			int INARG_LIMIT = OUTARG_LIMIT + (isLinker && !customized ? 1 : 0);
			int nameCursor = OUTARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int MTYPE_ARG = customized ? -1 : nameCursor++;
			int MTYPE_ARG = customized ? - 1 : nameCursor++; // might be last in-argument
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CHECK_TYPE = nameCursor++;
			int CHECK_TYPE = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CHECK_CUSTOM = (CUSTOMIZE_THRESHOLD >= 0) ? nameCursor++ : -1;
			int CHECK_CUSTOM = (CUSTOMIZE_THRESHOLD >= 0) ? nameCursor++: -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int LINKER_CALL = nameCursor++;
			int LINKER_CALL = nameCursor++;
			MethodType invokerFormType = mtype.InvokerType();
			if (isLinker)
			{
				if (!customized)
				{
					invokerFormType = invokerFormType.AppendParameterTypes(typeof(MemberName));
				}
			}
			else
			{
				invokerFormType = invokerFormType.InvokerType();
			}
			Name[] names = arguments(nameCursor - INARG_LIMIT, invokerFormType);
			assert(names.Length == nameCursor) : Arrays.asList(mtype, customized, which, nameCursor, names.Length);
			if (MTYPE_ARG >= INARG_LIMIT)
			{
				assert(names[MTYPE_ARG] == null);
				BoundMethodHandle.SpeciesData speciesData = BoundMethodHandle.SpeciesData_L();
				names[THIS_MH] = names[THIS_MH].withConstraint(speciesData);
				NamedFunction getter = speciesData.GetterFunction(0);
				names[MTYPE_ARG] = new Name(getter, names[THIS_MH]);
				// else if isLinker, then MTYPE is passed in from the caller (e.g., the JVM)
			}

			// Make the final call.  If isGeneric, then prepend the result of type checking.
			MethodType outCallType = mtype.BasicType();
			Object[] outArgs = Arrays.CopyOfRange(names, CALL_MH, OUTARG_LIMIT, typeof(Object[]));
			Object mtypeArg = (customized ? mtype : names[MTYPE_ARG]);
			if (!isGeneric)
			{
				names[CHECK_TYPE] = new Name(NF_checkExactType, names[CALL_MH], mtypeArg);
				// mh.invokeExact(a*):R => checkExactType(mh, TYPEOF(a*:R)); mh.invokeBasic(a*)
			}
			else
			{
				names[CHECK_TYPE] = new Name(NF_checkGenericType, names[CALL_MH], mtypeArg);
				// mh.invokeGeneric(a*):R => checkGenericType(mh, TYPEOF(a*:R)).invokeBasic(a*)
				outArgs[0] = names[CHECK_TYPE];
			}
			if (CHECK_CUSTOM != -1)
			{
				names[CHECK_CUSTOM] = new Name(NF_checkCustomized, outArgs[0]);
			}
			names[LINKER_CALL] = new Name(outCallType, outArgs);
			lform = new LambdaForm(debugName, INARG_LIMIT, names);
			if (isLinker)
			{
				lform.CompileToBytecode(); // JVM needs a real methodOop
			}
			if (isCached)
			{
				lform = mtype.Form().SetCachedLambdaForm(which, lform);
			}
			return lform;
		}

		/*non-public*/	 internal static WrongMethodTypeException NewWrongMethodTypeException(MethodType actual, MethodType expected)
	 {
			// FIXME: merge with JVM logic for throwing WMTE
			return new WrongMethodTypeException("expected " + expected + " but found " + actual);
	 }

		/// <summary>
		/// Static definition of MethodHandle.invokeExact checking code. </summary>
		/*non-public*/	 internal static @ForceInline void CheckExactType(Object mhObj, Object expectedObj)
	 {
			MethodHandle mh = (MethodHandle) mhObj;
			MethodType expected = (MethodType) expectedObj;
			MethodType actual = mh.Type();
			if (actual != expected)
			{
				throw NewWrongMethodTypeException(expected, actual);
			}
	 }

		/// <summary>
		/// Static definition of MethodHandle.invokeGeneric checking code.
		/// Directly returns the type-adjusted MH to invoke, as follows:
		/// {@code (R)MH.invoke(a*) => MH.asType(TYPEOF(a*:R)).invokeBasic(a*)}
		/// </summary>
		/*non-public*/	 internal static @ForceInline Object CheckGenericType(Object mhObj, Object expectedObj)
	 {
			MethodHandle mh = (MethodHandle) mhObj;
			MethodType expected = (MethodType) expectedObj;
			return mh.AsType(expected);
			/* Maybe add more paths here.  Possible optimizations:
			 * for (R)MH.invoke(a*),
			 * let MT0 = TYPEOF(a*:R), MT1 = MH.type
			 *
			 * if MT0==MT1 or MT1 can be safely called by MT0
			 *  => MH.invokeBasic(a*)
			 * if MT1 can be safely called by MT0[R := Object]
			 *  => MH.invokeBasic(a*) & checkcast(R)
			 * if MT1 can be safely called by MT0[* := Object]
			 *  => checkcast(A)* & MH.invokeBasic(a*) & checkcast(R)
			 * if a big adapter BA can be pulled out of (MT0,MT1)
			 *  => BA.invokeBasic(MT0,MH,a*)
			 * if a local adapter LA can cached on static CS0 = new GICS(MT0)
			 *  => CS0.LA.invokeBasic(MH,a*)
			 * else
			 *  => MH.asType(MT0).invokeBasic(A*)
			 */
	 }

		internal static MemberName LinkToCallSiteMethod(MethodType mtype)
		{
			LambdaForm lform = CallSiteForm(mtype, false);
			return lform.Vmentry;
		}

		internal static MemberName LinkToTargetMethod(MethodType mtype)
		{
			LambdaForm lform = CallSiteForm(mtype, true);
			return lform.Vmentry;
		}

		// skipCallSite is true if we are optimizing a ConstantCallSite
		private static LambdaForm CallSiteForm(MethodType mtype, bool skipCallSite)
		{
			mtype = mtype.BasicType(); // normalize Z to I, String to Object, etc.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int which = (skipCallSite ? MethodTypeForm.LF_MH_LINKER : MethodTypeForm.LF_CS_LINKER);
			int which = (skipCallSite ? MethodTypeForm.LF_MH_LINKER : MethodTypeForm.LF_CS_LINKER);
			LambdaForm lform = mtype.Form().CachedLambdaForm(which);
			if (lform != null)
			{
				return lform;
			}
			// exactInvokerForm (Object,Object)Object
			//   link with java.lang.invoke.MethodHandle.invokeBasic(MethodHandle,Object,Object)Object/invokeSpecial
			const int ARG_BASE = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int OUTARG_LIMIT = ARG_BASE + mtype.parameterCount();
			int OUTARG_LIMIT = ARG_BASE + mtype.ParameterCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INARG_LIMIT = OUTARG_LIMIT + 1;
			int INARG_LIMIT = OUTARG_LIMIT + 1;
			int nameCursor = OUTARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int APPENDIX_ARG = nameCursor++;
			int APPENDIX_ARG = nameCursor++; // the last in-argument
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CSITE_ARG = skipCallSite ? -1 : APPENDIX_ARG;
			int CSITE_ARG = skipCallSite ? - 1 : APPENDIX_ARG;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CALL_MH = skipCallSite ? APPENDIX_ARG : nameCursor++;
			int CALL_MH = skipCallSite ? APPENDIX_ARG : nameCursor++; // result of getTarget
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int LINKER_CALL = nameCursor++;
			int LINKER_CALL = nameCursor++;
			MethodType invokerFormType = mtype.AppendParameterTypes(skipCallSite ? typeof(MethodHandle) : typeof(CallSite));
			Name[] names = arguments(nameCursor - INARG_LIMIT, invokerFormType);
			assert(names.Length == nameCursor);
			assert(names[APPENDIX_ARG] != null);
			if (!skipCallSite)
			{
				names[CALL_MH] = new Name(NF_getCallSiteTarget, names[CSITE_ARG]);
			}
			// (site.)invokedynamic(a*):R => mh = site.getTarget(); mh.invokeBasic(a*)
			const int PREPEND_MH = 0, PREPEND_COUNT = 1;
			Object[] outArgs = Arrays.CopyOfRange(names, ARG_BASE, OUTARG_LIMIT + PREPEND_COUNT, typeof(Object[]));
			// prepend MH argument:
			System.Array.Copy(outArgs, 0, outArgs, PREPEND_COUNT, outArgs.Length - PREPEND_COUNT);
			outArgs[PREPEND_MH] = names[CALL_MH];
			names[LINKER_CALL] = new Name(mtype, outArgs);
			lform = new LambdaForm((skipCallSite ? "linkToTargetMethod" : "linkToCallSite"), INARG_LIMIT, names);
			lform.CompileToBytecode(); // JVM needs a real methodOop
			lform = mtype.Form().SetCachedLambdaForm(which, lform);
			return lform;
		}

		/// <summary>
		/// Static definition of MethodHandle.invokeGeneric checking code. </summary>
		/*non-public*/	 internal static @ForceInline Object GetCallSiteTarget(Object site)
	 {
			return ((CallSite)site).Target;
	 }

		/*non-public*/	 internal static @ForceInline void CheckCustomized(Object o)
	 {
			MethodHandle mh = (MethodHandle)o;
			if (mh.Form.Customized == null)
			{
				MaybeCustomize(mh);
			}
	 }

		/*non-public*/	 internal static @DontInline void MaybeCustomize(MethodHandle mh)
	 {
			sbyte count = mh.CustomizationCount;
			if (count >= CUSTOMIZE_THRESHOLD)
			{
				mh.Customize();
			}
			else
			{
				mh.CustomizationCount = (sbyte)(count + 1);
			}
	 }

		// Local constant functions:
		private static readonly NamedFunction NF_checkExactType, NF_checkGenericType, NF_getCallSiteTarget, NF_checkCustomized;
		static Invokers()
		{
			try
			{
				NamedFunction[] nfs = new NamedFunction[] {NF_checkExactType = new NamedFunction(typeof(Invokers).getDeclaredMethod("checkExactType", typeof(Object), typeof(Object))), NF_checkGenericType = new NamedFunction(typeof(Invokers).getDeclaredMethod("checkGenericType", typeof(Object), typeof(Object))), NF_getCallSiteTarget = new NamedFunction(typeof(Invokers).getDeclaredMethod("getCallSiteTarget", typeof(Object))), NF_checkCustomized = new NamedFunction(typeof(Invokers).getDeclaredMethod("checkCustomized", typeof(Object)))};
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
				try
				{
					MH_asSpreader = IMPL_LOOKUP.findVirtual(typeof(MethodHandle), "asSpreader", MethodType.MethodType(typeof(MethodHandle), typeof(Class), typeof(int)));
				}
				catch (ReflectiveOperationException ex)
				{
					throw newInternalError(ex);
				}
		}

		private class Lazy
		{
			internal static readonly MethodHandle MH_asSpreader;

		}
	}

}