using System;
using System.Collections.Generic;

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


	using Empty = sun.invoke.empty.Empty;
	using ValueConversions = sun.invoke.util.ValueConversions;
	using VerifyType = sun.invoke.util.VerifyType;
	using Wrapper = sun.invoke.util.Wrapper;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;

	/// <summary>
	/// Trusted implementation code for MethodHandle.
	/// @author jrose
	/// </summary>
	/*non-public*/	 internal abstract class MethodHandleImpl
	 {
		// Do not adjust this except for special platforms:
		private static readonly int MAX_ARITY;
		static MethodHandleImpl()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] values = { 255 };
			Object[] values = new Object[] {255};
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(values));
			MAX_ARITY = (Integer) values[0];
				MethodHandle[] cache = TYPED_ACCESSORS.get(typeof(Object[]));
				cache[GETTER_INDEX] = OBJECT_ARRAY_GETTER = MakeIntrinsic(getAccessor(typeof(Object[]), false), Intrinsic.ARRAY_LOAD);
				cache[SETTER_INDEX] = OBJECT_ARRAY_SETTER = MakeIntrinsic(getAccessor(typeof(Object[]), true), Intrinsic.ARRAY_STORE);

				assert(InvokerBytecodeGenerator.IsStaticallyInvocable(ArrayAccessor.OBJECT_ARRAY_GETTER.InternalMemberName()));
				assert(InvokerBytecodeGenerator.IsStaticallyInvocable(ArrayAccessor.OBJECT_ARRAY_SETTER.InternalMemberName()));
				ARRAYS = MakeArrays();
				FILL_ARRAYS = MakeFillArrays();

				try
				{
					NF_checkSpreadArgument = new NamedFunction(MHI.getDeclaredMethod("checkSpreadArgument", typeof(Object), typeof(int)));
					NF_guardWithCatch = new NamedFunction(MHI.getDeclaredMethod("guardWithCatch", typeof(MethodHandle), typeof(Class), typeof(MethodHandle), typeof(Object[])));
					NF_throwException = new NamedFunction(MHI.getDeclaredMethod("throwException", typeof(Throwable)));
					NF_profileBoolean = new NamedFunction(MHI.getDeclaredMethod("profileBoolean", typeof(bool), typeof(int[])));

					NF_checkSpreadArgument.resolve();
					NF_guardWithCatch.resolve();
					NF_throwException.resolve();
					NF_profileBoolean.resolve();

					MH_castReference = IMPL_LOOKUP.findStatic(MHI, "castReference", MethodType.MethodType(typeof(Object), typeof(Class), typeof(Object)));
					MH_copyAsPrimitiveArray = IMPL_LOOKUP.findStatic(MHI, "copyAsPrimitiveArray", MethodType.MethodType(typeof(Object), typeof(Wrapper), typeof(Object[])));
					MH_arrayIdentity = IMPL_LOOKUP.findStatic(MHI, "identity", MethodType.MethodType(typeof(Object[]), typeof(Object[])));
					MH_fillNewArray = IMPL_LOOKUP.findStatic(MHI, "fillNewArray", MethodType.MethodType(typeof(Object[]), typeof(Integer), typeof(Object[])));
					MH_fillNewTypedArray = IMPL_LOOKUP.findStatic(MHI, "fillNewTypedArray", MethodType.methodType(typeof(Object[]), typeof(Object[]), typeof(Integer), typeof(Object[])));

					MH_selectAlternative = MakeIntrinsic(IMPL_LOOKUP.findStatic(MHI, "selectAlternative", MethodType.methodType(typeof(MethodHandle), typeof(bool), typeof(MethodHandle), typeof(MethodHandle))), Intrinsic.SELECT_ALTERNATIVE);
				}
				catch (ReflectiveOperationException ex)
				{
					throw newInternalError(ex);
				}
				Class THIS_CLASS = typeof(CountingWrapper);
				try
				{
					NF_maybeStopCounting = new NamedFunction(THIS_CLASS.GetDeclaredMethod("maybeStopCounting", typeof(Object)));
				}
				catch (ReflectiveOperationException ex)
				{
					throw newInternalError(ex);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class THIS_CLASS = BindCaller.class;
				Class THIS_CLASS = typeof(BindCaller);
				assert(checkCallerClass(THIS_CLASS, THIS_CLASS));
				try
				{
					MH_checkCallerClass = IMPL_LOOKUP.findStatic(THIS_CLASS, "checkCallerClass", MethodType.MethodType(typeof(bool), typeof(Class), typeof(Class)));
					assert((bool) MH_checkCallerClass.invokeExact(THIS_CLASS, THIS_CLASS));
				}
				catch (Throwable ex)
				{
					throw new InternalError(ex);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] values = {null};
				Object[] values = new Object[] {null};
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(values, ex));
				T_BYTES = (sbyte[]) values[0];
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
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Values[0] = Integer.GetInteger(typeof(MethodHandleImpl).FullName + ".MAX_ARITY", 255);
				return null;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private object[] Values;
			private java.lang.ReflectiveOperationException Ex;

			public PrivilegedActionAnonymousInnerClassHelper2(object[] values, java.lang.ReflectiveOperationException ex)
			{
				this.Values = values;
				this.Ex = ex;
			}

			public virtual Void Run()
			{
				try
				{
					Class tClass = typeof(T);
					String tName = tClass.Name;
					String tResource = tName.Substring(tName.LastIndexOf('.') + 1) + ".class";
					java.net.URLConnection uconn = tClass.GetResource(tResource).OpenConnection();
					int len = uconn.ContentLength;
					sbyte[] bytes = new sbyte[len];
					using (java.io.InputStream str = uconn.InputStream)
					{
						int nr = str.read(bytes);
						if (nr != len)
						{
							throw new java.io.IOException(tResource);
						}
					}
					Values[0] = bytes;
				}
				catch (java.io.IOException ex)
				{
					throw new InternalError(ex);
				}
				return null;
			}
		}

		/// Factory methods to create method handles:

		internal static void InitStatics()
		{
			// Trigger selected static initializations.
			MemberName.Factory.INSTANCE.GetType();
		}

		internal static MethodHandle MakeArrayElementAccessor(Class arrayClass, bool isSetter)
		{
			if (arrayClass == typeof(Object[]))
			{
				return (isSetter ? ArrayAccessor.OBJECT_ARRAY_SETTER : ArrayAccessor.OBJECT_ARRAY_GETTER);
			}
			if (!arrayClass.Array)
			{
				throw newIllegalArgumentException("not an array: " + arrayClass);
			}
			MethodHandle[] cache = ArrayAccessor.TYPED_ACCESSORS.get(arrayClass);
			int cacheIndex = (isSetter ? ArrayAccessor.SETTER_INDEX : ArrayAccessor.GETTER_INDEX);
			MethodHandle mh = cache[cacheIndex];
			if (mh != null)
			{
				return mh;
			}
			mh = ArrayAccessor.GetAccessor(arrayClass, isSetter);
			MethodType correctType = ArrayAccessor.CorrectType(arrayClass, isSetter);
			if (mh.Type() != correctType)
			{
				assert(mh.Type().ParameterType(0) == typeof(Object[]));
				assert((isSetter ? mh.Type().ParameterType(2) : mh.Type().ReturnType()) == typeof(Object));
				assert(isSetter || correctType.ParameterType(0).ComponentType == correctType.ReturnType());
				// safe to view non-strictly, because element type follows from array type
				mh = mh.ViewAsType(correctType, false);
			}
			mh = MakeIntrinsic(mh, (isSetter ? Intrinsic.ARRAY_STORE : Intrinsic.ARRAY_LOAD));
			// Atomically update accessor cache.
			lock (cache)
			{
				if (cache[cacheIndex] == null)
				{
					cache[cacheIndex] = mh;
				}
				else
				{
					// Throw away newly constructed accessor and use cached version.
					mh = cache[cacheIndex];
				}
			}
			return mh;
		}

		internal sealed class ArrayAccessor
		{
			/// Support for array element access
			internal const int GETTER_INDEX = 0, SETTER_INDEX = 1, INDEX_LIMIT = 2;
			internal static readonly ClassValue<MethodHandle[]> TYPED_ACCESSORS = new ClassValueAnonymousInnerClassHelper();

			private class ClassValueAnonymousInnerClassHelper : ClassValue<MethodHandle[]>
			{
				public ClassValueAnonymousInnerClassHelper()
				{
				}

				protected internal override MethodHandle[] ComputeValue(Class type)
				{
					return new MethodHandle[INDEX_LIMIT];
				}
			}
			internal static readonly MethodHandle OBJECT_ARRAY_GETTER, OBJECT_ARRAY_SETTER;

			internal static int GetElementI(int[] a, int i)
			{
				return a[i];
			}
			internal static long GetElementJ(long[] a, int i)
			{
				return a[i];
			}
			internal static float GetElementF(float[] a, int i)
			{
				return a[i];
			}
			internal static double GetElementD(double[] a, int i)
			{
				return a[i];
			}
			internal static bool GetElementZ(bool[] a, int i)
			{
				return a[i];
			}
			internal static sbyte GetElementB(sbyte[] a, int i)
			{
				return a[i];
			}
			internal static short GetElementS(short[] a, int i)
			{
				return a[i];
			}
			internal static char GetElementC(char[] a, int i)
			{
				return a[i];
			}
			internal static Object GetElementL(Object[] a, int i)
			{
				return a[i];
			}

			internal static void SetElementI(int[] a, int i, int x)
			{
				a[i] = x;
			}
			internal static void SetElementJ(long[] a, int i, long x)
			{
				a[i] = x;
			}
			internal static void SetElementF(float[] a, int i, float x)
			{
				a[i] = x;
			}
			internal static void SetElementD(double[] a, int i, double x)
			{
				a[i] = x;
			}
			internal static void SetElementZ(bool[] a, int i, bool x)
			{
				a[i] = x;
			}
			internal static void SetElementB(sbyte[] a, int i, sbyte x)
			{
				a[i] = x;
			}
			internal static void SetElementS(short[] a, int i, short x)
			{
				a[i] = x;
			}
			internal static void SetElementC(char[] a, int i, char x)
			{
				a[i] = x;
			}
			internal static void SetElementL(Object[] a, int i, Object x)
			{
				a[i] = x;
			}

			internal static String Name(Class arrayClass, bool isSetter)
			{
				Class elemClass = arrayClass.ComponentType;
				if (elemClass == null)
				{
					throw newIllegalArgumentException("not an array", arrayClass);
				}
				return (!isSetter ? "getElement" : "setElement") + Wrapper.basicTypeChar(elemClass);
			}
			internal static MethodType Type(Class arrayClass, bool isSetter)
			{
				Class elemClass = arrayClass.ComponentType;
				Class arrayArgClass = arrayClass;
				if (!elemClass.Primitive)
				{
					arrayArgClass = typeof(Object[]);
					elemClass = typeof(Object);
				}
				return !isSetter ? MethodType.MethodType(elemClass, arrayArgClass, typeof(int)) : MethodType.methodType(typeof(void), arrayArgClass, typeof(int), elemClass);
			}
			internal static MethodType CorrectType(Class arrayClass, bool isSetter)
			{
				Class elemClass = arrayClass.ComponentType;
				return !isSetter ? MethodType.MethodType(elemClass, arrayClass, typeof(int)) : MethodType.methodType(typeof(void), arrayClass, typeof(int), elemClass);
			}
			internal static MethodHandle GetAccessor(Class arrayClass, bool isSetter)
			{
				String name = Name(arrayClass, isSetter);
				MethodType type = Type(arrayClass, isSetter);
				try
				{
					return IMPL_LOOKUP.findStatic(typeof(ArrayAccessor), name, type);
				}
				catch (ReflectiveOperationException ex)
				{
					throw uncaughtException(ex);
				}
			}
		}

		/// <summary>
		/// Create a JVM-level adapter method handle to conform the given method
		/// handle to the similar newType, using only pairwise argument conversions.
		/// For each argument, convert incoming argument to the exact type needed.
		/// The argument conversions allowed are casting, boxing and unboxing,
		/// integral widening or narrowing, and floating point widening or narrowing. </summary>
		/// <param name="srcType"> required call type </param>
		/// <param name="target"> original method handle </param>
		/// <param name="strict"> if true, only asType conversions are allowed; if false, explicitCastArguments conversions allowed </param>
		/// <param name="monobox"> if true, unboxing conversions are assumed to be exactly typed (Integer to int only, not long or double) </param>
		/// <returns> an adapter to the original handle with the desired new type,
		///          or the original target if the types are already identical
		///          or null if the adaptation cannot be made </returns>
		internal static MethodHandle MakePairwiseConvert(MethodHandle target, MethodType srcType, bool strict, bool monobox)
		{
			MethodType dstType = target.Type();
			if (srcType == dstType)
			{
				return target;
			}
			return MakePairwiseConvertByEditor(target, srcType, strict, monobox);
		}

		private static int CountNonNull(Object[] array)
		{
			int count = 0;
			foreach (Object x in array)
			{
				if (x != null)
				{
					++count;
				}
			}
			return count;
		}

		internal static MethodHandle MakePairwiseConvertByEditor(MethodHandle target, MethodType srcType, bool strict, bool monobox)
		{
			Object[] convSpecs = ComputeValueConversions(srcType, target.Type(), strict, monobox);
			int convCount = CountNonNull(convSpecs);
			if (convCount == 0)
			{
				return target.ViewAsType(srcType, strict);
			}
			MethodType basicSrcType = srcType.BasicType();
			MethodType midType = target.Type().BasicType();
			BoundMethodHandle mh = target.Rebind();
			// FIXME: Reduce number of bindings when there is more than one Class conversion.
			// FIXME: Reduce number of bindings when there are repeated conversions.
			for (int i = 0; i < convSpecs.Length - 1; i++)
			{
				Object convSpec = convSpecs[i];
				if (convSpec == null)
				{
					continue;
				}
				MethodHandle fn;
				if (convSpec is Class)
				{
					fn = Lazy.MH_castReference.BindTo(convSpec);
				}
				else
				{
					fn = (MethodHandle) convSpec;
				}
				Class newType = basicSrcType.ParameterType(i);
				if (--convCount == 0)
				{
					midType = srcType;
				}
				else
				{
					midType = midType.ChangeParameterType(i, newType);
				}
				LambdaForm form2 = mh.Editor().FilterArgumentForm(1 + i, BasicType.basicType(newType));
				mh = mh.CopyWithExtendL(midType, form2, fn);
				mh = mh.Rebind();
			}
			Object convSpec = convSpecs[convSpecs.Length - 1];
			if (convSpec != null)
			{
				MethodHandle fn;
				if (convSpec is Class)
				{
					if (convSpec == typeof(void))
					{
						fn = null;
					}
					else
					{
						fn = Lazy.MH_castReference.BindTo(convSpec);
					}
				}
				else
				{
					fn = (MethodHandle) convSpec;
				}
				Class newType = basicSrcType.ReturnType();
				assert(--convCount == 0);
				midType = srcType;
				if (fn != null)
				{
					mh = mh.Rebind(); // rebind if too complex
					LambdaForm form2 = mh.Editor().FilterReturnForm(BasicType.basicType(newType), false);
					mh = mh.CopyWithExtendL(midType, form2, fn);
				}
				else
				{
					LambdaForm form2 = mh.Editor().FilterReturnForm(BasicType.basicType(newType), true);
					mh = mh.CopyWith(midType, form2);
				}
			}
			assert(convCount == 0);
			assert(mh.Type().Equals(srcType));
			return mh;
		}

		internal static MethodHandle MakePairwiseConvertIndirect(MethodHandle target, MethodType srcType, bool strict, bool monobox)
		{
			assert(target.Type().ParameterCount() == srcType.ParameterCount());
			// Calculate extra arguments (temporaries) required in the names array.
			Object[] convSpecs = ComputeValueConversions(srcType, target.Type(), strict, monobox);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INARG_COUNT = srcType.parameterCount();
			int INARG_COUNT = srcType.ParameterCount();
			int convCount = CountNonNull(convSpecs);
			bool retConv = (convSpecs[INARG_COUNT] != null);
			bool retVoid = srcType.ReturnType() == typeof(void);
			if (retConv && retVoid)
			{
				convCount -= 1;
				retConv = false;
			}

			const int IN_MH = 0;
			const int INARG_BASE = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INARG_LIMIT = INARG_BASE + INARG_COUNT;
			int INARG_LIMIT = INARG_BASE + INARG_COUNT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int NAME_LIMIT = INARG_LIMIT + convCount + 1;
			int NAME_LIMIT = INARG_LIMIT + convCount + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int RETURN_CONV = (!retConv ? -1 : NAME_LIMIT - 1);
			int RETURN_CONV = (!retConv ? - 1 : NAME_LIMIT - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int OUT_CALL = (!retConv ? NAME_LIMIT : RETURN_CONV) - 1;
			int OUT_CALL = (!retConv ? NAME_LIMIT : RETURN_CONV) - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int RESULT = (retVoid ? -1 : NAME_LIMIT - 1);
			int RESULT = (retVoid ? - 1 : NAME_LIMIT - 1);

			// Now build a LambdaForm.
			MethodType lambdaType = srcType.BasicType().InvokerType();
			Name[] names = arguments(NAME_LIMIT - INARG_LIMIT, lambdaType);

			// Collect the arguments to the outgoing call, maybe with conversions:
			const int OUTARG_BASE = 0; // target MH is Name.function, name Name.arguments[0]
			Object[] outArgs = new Object[OUTARG_BASE + INARG_COUNT];

			int nameCursor = INARG_LIMIT;
			for (int i = 0; i < INARG_COUNT; i++)
			{
				Object convSpec = convSpecs[i];
				if (convSpec == null)
				{
					// do nothing: difference is trivial
					outArgs[OUTARG_BASE + i] = names[INARG_BASE + i];
					continue;
				}

				Name conv;
				if (convSpec is Class)
				{
					Class convClass = (Class) convSpec;
					conv = new Name(Lazy.MH_castReference, convClass, names[INARG_BASE + i]);
				}
				else
				{
					MethodHandle fn = (MethodHandle) convSpec;
					conv = new Name(fn, names[INARG_BASE + i]);
				}
				assert(names[nameCursor] == null);
				names[nameCursor++] = conv;
				assert(outArgs[OUTARG_BASE + i] == null);
				outArgs[OUTARG_BASE + i] = conv;
			}

			// Build argument array for the call.
			assert(nameCursor == OUT_CALL);
			names[OUT_CALL] = new Name(target, outArgs);

			Object convSpec = convSpecs[INARG_COUNT];
			if (!retConv)
			{
				assert(OUT_CALL == names.Length - 1);
			}
			else
			{
				Name conv;
				if (convSpec == typeof(void))
				{
					conv = new Name(LambdaForm.ConstantZero(BasicType.basicType(srcType.ReturnType())));
				}
				else if (convSpec is Class)
				{
					Class convClass = (Class) convSpec;
					conv = new Name(Lazy.MH_castReference, convClass, names[OUT_CALL]);
				}
				else
				{
					MethodHandle fn = (MethodHandle) convSpec;
					if (fn.Type().ParameterCount() == 0)
					{
						conv = new Name(fn); // don't pass retval to void conversion
					}
					else
					{
						conv = new Name(fn, names[OUT_CALL]);
					}
				}
				assert(names[RETURN_CONV] == null);
				names[RETURN_CONV] = conv;
				assert(RETURN_CONV == names.Length - 1);
			}

			LambdaForm form = new LambdaForm("convert", lambdaType.ParameterCount(), names, RESULT);
			return SimpleMethodHandle.Make(srcType, form);
		}

		/// <summary>
		/// Identity function, with reference cast. </summary>
		/// <param name="t"> an arbitrary reference type </param>
		/// <param name="x"> an arbitrary reference value </param>
		/// <returns> the same value x </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ForceInline @SuppressWarnings("unchecked") static <T,U> T castReference(Class t, U x)
		internal static T castReference<T, U>(Class t, U x)
		{
			// inlined Class.cast because we can't ForceInline it
			if (x != null && !t.isInstance(x))
			{
				throw NewClassCastException(t, x);
			}
			return (T) x;
		}

		private static ClassCastException NewClassCastException(Class t, Object obj)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return new ClassCastException("Cannot cast " + obj.GetType().FullName + " to " + t.Name);
		}

		internal static Object[] ComputeValueConversions(MethodType srcType, MethodType dstType, bool strict, bool monobox)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int INARG_COUNT = srcType.parameterCount();
			int INARG_COUNT = srcType.ParameterCount();
			Object[] convSpecs = new Object[INARG_COUNT + 1];
			for (int i = 0; i <= INARG_COUNT; i++)
			{
				bool isRet = (i == INARG_COUNT);
				Class src = isRet ? dstType.ReturnType() : srcType.ParameterType(i);
				Class dst = isRet ? srcType.ReturnType() : dstType.ParameterType(i);
				if (!VerifyType.isNullConversion(src, dst, strict)) //keepInterfaces=
				{
					convSpecs[i] = ValueConversion(src, dst, strict, monobox);
				}
			}
			return convSpecs;
		}
		internal static MethodHandle MakePairwiseConvert(MethodHandle target, MethodType srcType, bool strict)
		{
			return MakePairwiseConvert(target, srcType, strict, false); //monobox=
		}

		/// <summary>
		/// Find a conversion function from the given source to the given destination.
		/// This conversion function will be used as a LF NamedFunction.
		/// Return a Class object if a simple cast is needed.
		/// Return void.class if void is involved.
		/// </summary>
		internal static Object ValueConversion(Class src, Class dst, bool strict, bool monobox)
		{
			assert(!VerifyType.isNullConversion(src, dst, strict)); // caller responsibility - keepInterfaces=
			if (dst == typeof(void))
			{
				return dst;
			}
			MethodHandle fn;
			if (src.Primitive)
			{
				if (src == typeof(void))
				{
					return typeof(void); // caller must recognize this specially
				}
				else if (dst.Primitive)
				{
					// Examples: int->byte, byte->int, boolean->int (!strict)
					fn = ValueConversions.convertPrimitive(src, dst);
				}
				else
				{
					// Examples: int->Integer, boolean->Object, float->Number
					Wrapper wsrc = Wrapper.forPrimitiveType(src);
					fn = ValueConversions.boxExact(wsrc);
					assert(fn.Type().ParameterType(0) == wsrc.primitiveType());
					assert(fn.Type().ReturnType() == wsrc.wrapperType());
					if (!VerifyType.isNullConversion(wsrc.wrapperType(), dst, strict))
					{
						// Corner case, such as int->Long, which will probably fail.
						MethodType mt = MethodType.MethodType(dst, src);
						if (strict)
						{
							fn = fn.AsType(mt);
						}
						else
						{
							fn = MethodHandleImpl.MakePairwiseConvert(fn, mt, false); //strict=
						}
					}
				}
			}
			else if (dst.Primitive)
			{
				Wrapper wdst = Wrapper.forPrimitiveType(dst);
				if (monobox || src == wdst.wrapperType())
				{
					// Use a strongly-typed unboxer, if possible.
					fn = ValueConversions.unboxExact(wdst, strict);
				}
				else
				{
					// Examples:  Object->int, Number->int, Comparable->int, Byte->int
					// must include additional conversions
					// src must be examined at runtime, to detect Byte, Character, etc.
					fn = (strict ? ValueConversions.unboxWiden(wdst) : ValueConversions.unboxCast(wdst));
				}
			}
			else
			{
				// Simple reference conversion.
				// Note:  Do not check for a class hierarchy relation
				// between src and dst.  In all cases a 'null' argument
				// will pass the cast conversion.
				return dst;
			}
			assert(fn.Type().ParameterCount() <= 1) : "pc" + Arrays.asList(src.SimpleName, dst.SimpleName, fn);
			return fn;
		}

		internal static MethodHandle MakeVarargsCollector(MethodHandle target, Class arrayType)
		{
			MethodType type = target.Type();
			int last = type.ParameterCount() - 1;
			if (type.ParameterType(last) != arrayType)
			{
				target = target.AsType(type.ChangeParameterType(last, arrayType));
			}
			target = target.AsFixedArity(); // make sure this attribute is turned off
			return new AsVarargsCollector(target, arrayType);
		}

		private sealed class AsVarargsCollector : DelegatingMethodHandle
		{
			internal readonly MethodHandle Target_Renamed;
			internal readonly Class ArrayType;
			internal  Stable;

			internal AsVarargsCollector(MethodHandle target, Class arrayType) : this(target.Type(), target, arrayType)
			{
			}
			internal AsVarargsCollector(MethodType type, MethodHandle target, Class arrayType) : base(type, target)
			{
				this.Target_Renamed = target;
				this.ArrayType = arrayType;
				this.asCollectorCache = target.AsCollector(arrayType, 0);
			}

			public override bool VarargsCollector
			{
				get
				{
					return true;
				}
			}

			protected internal override MethodHandle Target
			{
				get
				{
					return Target_Renamed;
				}
			}

			public override MethodHandle AsFixedArity()
			{
				return Target_Renamed;
			}

			internal override MethodHandle SetVarargs(MemberName member)
			{
				if (member.Varargs)
				{
					return this;
				}
				return AsFixedArity();
			}

			public override MethodHandle AsTypeUncached(MethodType newType)
			{
				MethodType type = this.Type();
				int collectArg = type.ParameterCount() - 1;
				int newArity = newType.ParameterCount();
				if (newArity == collectArg + 1 && newType.ParameterType(collectArg).IsSubclassOf(type.ParameterType(collectArg)))
				{
					// if arity and trailing parameter are compatible, do normal thing
					return AsTypeCache = AsFixedArity().AsType(newType);
				}
				// check cache
				MethodHandle acc = asCollectorCache;
				if (acc != null && acc.Type().ParameterCount() == newArity)
				{
					return AsTypeCache = acc.AsType(newType);
				}
				// build and cache a collector
				int arrayLength = newArity - collectArg;
				MethodHandle collector;
				try
				{
					collector = AsFixedArity().AsCollector(ArrayType, arrayLength);
					assert(collector.Type().ParameterCount() == newArity) : "newArity=" + newArity + " but collector=" + collector;
				}
				catch (IllegalArgumentException ex)
				{
					throw new WrongMethodTypeException("cannot build collector", ex);
				}
				asCollectorCache = collector;
				return AsTypeCache = collector.AsType(newType);
			}

			internal override bool ViewAsTypeChecks(MethodType newType, bool strict)
			{
				base.ViewAsTypeChecks(newType, true);
				if (strict)
				{
					return true;
				}
				// extra assertion for non-strict checks:
				assert(newType.LastParameterType().ComponentType.IsSubclassOf(Type().LastParameterType().ComponentType)) : Arrays.asList(this, newType);
				return true;
			}
		}

		/// <summary>
		/// Factory method:  Spread selected argument. </summary>
		internal static MethodHandle MakeSpreadArguments(MethodHandle target, Class spreadArgType, int spreadArgPos, int spreadArgCount)
		{
			MethodType targetType = target.Type();

			for (int i = 0; i < spreadArgCount; i++)
			{
				Class arg = VerifyType.spreadArgElementType(spreadArgType, i);
				if (arg == null)
				{
					arg = typeof(Object);
				}
				targetType = targetType.ChangeParameterType(spreadArgPos + i, arg);
			}
			target = target.AsType(targetType);

			MethodType srcType = targetType.ReplaceParameterTypes(spreadArgPos, spreadArgPos + spreadArgCount, spreadArgType);
			// Now build a LambdaForm.
			MethodType lambdaType = srcType.InvokerType();
			Name[] names = arguments(spreadArgCount + 2, lambdaType);
			int nameCursor = lambdaType.ParameterCount();
			int[] indexes = new int[targetType.ParameterCount()];

			for (int i = 0, argIndex = 1; i < targetType.ParameterCount() + 1; i++, argIndex++)
			{
				Class src = lambdaType.ParameterType(i);
				if (i == spreadArgPos)
				{
					// Spread the array.
					MethodHandle aload = MethodHandles.ArrayElementGetter(spreadArgType);
					Name array = names[argIndex];
					names[nameCursor++] = new Name(Lazy.NF_checkSpreadArgument, array, spreadArgCount);
					for (int j = 0; j < spreadArgCount; i++, j++)
					{
						indexes[i] = nameCursor;
						names[nameCursor++] = new Name(aload, array, j);
					}
				}
				else if (i < indexes.Length)
				{
					indexes[i] = argIndex;
				}
			}
			assert(nameCursor == names.Length - 1); // leave room for the final call

			// Build argument array for the call.
			Name[] targetArgs = new Name[targetType.ParameterCount()];
			for (int i = 0; i < targetType.ParameterCount(); i++)
			{
				int idx = indexes[i];
				targetArgs[i] = names[idx];
			}
			names[names.Length - 1] = new Name(target, (Object[]) targetArgs);

			LambdaForm form = new LambdaForm("spread", lambdaType.ParameterCount(), names);
			return SimpleMethodHandle.Make(srcType, form);
		}

		internal static void CheckSpreadArgument(Object av, int n)
		{
			if (av == null)
			{
				if (n == 0)
				{
					return;
				}
			}
			else if (av is Object[])
			{
				int len = ((Object[])av).Length;
				if (len == n)
				{
					return;
				}
			}
			else
			{
				int len = java.lang.reflect.Array.getLength(av);
				if (len == n)
				{
					return;
				}
			}
			// fall through to error:
			throw newIllegalArgumentException("array is not of length " + n);
		}

		/// <summary>
		/// Pre-initialized NamedFunctions for bootstrapping purposes.
		/// Factored in an inner class to delay initialization until first usage.
		/// </summary>
		internal class Lazy
		{
			internal static readonly Class MHI = typeof(MethodHandleImpl);

			internal static readonly MethodHandle[] ARRAYS;
			internal static readonly MethodHandle[] FILL_ARRAYS;

			internal static readonly NamedFunction NF_checkSpreadArgument;
			internal static readonly NamedFunction NF_guardWithCatch;
			internal static readonly NamedFunction NF_throwException;
			internal static readonly NamedFunction NF_profileBoolean;

			internal static readonly MethodHandle MH_castReference;
			internal static readonly MethodHandle MH_selectAlternative;
			internal static readonly MethodHandle MH_copyAsPrimitiveArray;
			internal static readonly MethodHandle MH_fillNewTypedArray;
			internal static readonly MethodHandle MH_fillNewArray;
			internal static readonly MethodHandle MH_arrayIdentity;

		}

		/// <summary>
		/// Factory method:  Collect or filter selected argument(s). </summary>
		internal static MethodHandle MakeCollectArguments(MethodHandle target, MethodHandle collector, int collectArgPos, bool retainOriginalArgs)
		{
			MethodType targetType = target.Type(); // (a..., c, [b...])=>r
			MethodType collectorType = collector.Type(); // (b...)=>c
			int collectArgCount = collectorType.ParameterCount();
			Class collectValType = collectorType.ReturnType();
			int collectValCount = (collectValType == typeof(void) ? 0 : 1);
			MethodType srcType = targetType.DropParameterTypes(collectArgPos, collectArgPos + collectValCount); // (a..., [b...])=>r
			if (!retainOriginalArgs) // (a..., b...)=>r
			{
				srcType = srcType.InsertParameterTypes(collectArgPos, collectorType.ParameterList());
			}
			// in  arglist: [0: ...keep1 | cpos: collect...  | cpos+cacount: keep2... ]
			// out arglist: [0: ...keep1 | cpos: collectVal? | cpos+cvcount: keep2... ]
			// out(retain): [0: ...keep1 | cpos: cV? coll... | cpos+cvc+cac: keep2... ]

			// Now build a LambdaForm.
			MethodType lambdaType = srcType.InvokerType();
			Name[] names = arguments(2, lambdaType);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int collectNamePos = names.length - 2;
			int collectNamePos = names.Length - 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int targetNamePos = names.length - 1;
			int targetNamePos = names.Length - 1;

			Name[] collectorArgs = Arrays.CopyOfRange(names, 1 + collectArgPos, 1 + collectArgPos + collectArgCount);
			names[collectNamePos] = new Name(collector, (Object[]) collectorArgs);

			// Build argument array for the target.
			// Incoming LF args to copy are: [ (mh) headArgs collectArgs tailArgs ].
			// Output argument array is [ headArgs (collectVal)? (collectArgs)? tailArgs ].
			Name[] targetArgs = new Name[targetType.ParameterCount()];
			int inputArgPos = 1; // incoming LF args to copy to target
			int targetArgPos = 0; // fill pointer for targetArgs
			int chunk = collectArgPos; // |headArgs|
			System.Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk);
			inputArgPos += chunk;
			targetArgPos += chunk;
			if (collectValType != typeof(void))
			{
				targetArgs[targetArgPos++] = names[collectNamePos];
			}
			chunk = collectArgCount;
			if (retainOriginalArgs)
			{
				System.Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk);
				targetArgPos += chunk; // optionally pass on the collected chunk
			}
			inputArgPos += chunk;
			chunk = targetArgs.Length - targetArgPos; // all the rest
			System.Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk);
			assert(inputArgPos + chunk == collectNamePos); // use of rest of input args also
			names[targetNamePos] = new Name(target, (Object[]) targetArgs);

			LambdaForm form = new LambdaForm("collect", lambdaType.ParameterCount(), names);
			return SimpleMethodHandle.Make(srcType, form);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @LambdaForm.Hidden static MethodHandle selectAlternative(boolean testResult, MethodHandle target, MethodHandle fallback)
		internal static MethodHandle SelectAlternative(bool testResult, MethodHandle target, MethodHandle fallback)
		{
			if (testResult)
			{
				return target;
			}
			else
			{
				return fallback;
			}
		}

		// Intrinsified by C2. Counters are used during parsing to calculate branch frequencies.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @LambdaForm.Hidden static boolean profileBoolean(boolean result, int[] counters)
		internal static bool ProfileBoolean(bool result, int[] counters)
		{
			// Profile is int[2] where [0] and [1] correspond to false and true occurrences respectively.
			int idx = result ? 1 : 0;
			try
			{
				counters[idx] = Math.AddExact(counters[idx], 1);
			}
			catch (ArithmeticException)
			{
				// Avoid continuous overflow by halving the problematic count.
				counters[idx] = counters[idx] / 2;
			}
			return result;
		}

		internal static MethodHandle MakeGuardWithTest(MethodHandle test, MethodHandle target, MethodHandle fallback)
		{
			MethodType type = target.Type();
			assert(test.Type().Equals(type.ChangeReturnType(typeof(bool))) && fallback.Type().Equals(type));
			MethodType basicType = type.BasicType();
			LambdaForm form = MakeGuardWithTestForm(basicType);
			BoundMethodHandle mh;
			try
			{
				if (PROFILE_GWT)
				{
					int[] counts = new int[2];
					mh = (BoundMethodHandle) BoundMethodHandle.SpeciesData_LLLL().Constructor().invokeBasic(type, form, (Object) test, (Object) Profile(target), (Object) Profile(fallback), counts);
				}
				else
				{
					mh = (BoundMethodHandle) BoundMethodHandle.SpeciesData_LLL().Constructor().invokeBasic(type, form, (Object) test, (Object) Profile(target), (Object) Profile(fallback));
				}
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
			assert(mh.Type() == type);
			return mh;
		}


		internal static MethodHandle Profile(MethodHandle target)
		{
			if (DONT_INLINE_THRESHOLD >= 0)
			{
				return MakeBlockInlningWrapper(target);
			}
			else
			{
				return target;
			}
		}

		/// <summary>
		/// Block inlining during JIT-compilation of a target method handle if it hasn't been invoked enough times.
		/// Corresponding LambdaForm has @DontInline when compiled into bytecode.
		/// </summary>
		internal static MethodHandle MakeBlockInlningWrapper(MethodHandle target)
		{
			LambdaForm lform = PRODUCE_BLOCK_INLINING_FORM.apply(target);
			return new CountingWrapper(target, lform, PRODUCE_BLOCK_INLINING_FORM, PRODUCE_REINVOKER_FORM, DONT_INLINE_THRESHOLD);
		}

		/// <summary>
		/// Constructs reinvoker lambda form which block inlining during JIT-compilation for a particular method handle </summary>
		private static readonly Function<MethodHandle, LambdaForm> PRODUCE_BLOCK_INLINING_FORM = new FunctionAnonymousInnerClassHelper();

		private class FunctionAnonymousInnerClassHelper : Function<MethodHandle, LambdaForm>
		{
			public FunctionAnonymousInnerClassHelper()
			{
			}

			public virtual LambdaForm Apply(MethodHandle target)
			{
				return DelegatingMethodHandle.MakeReinvokerForm(target, MethodTypeForm.LF_DELEGATE_BLOCK_INLINING, typeof(CountingWrapper), "reinvoker.dontInline", false, DelegatingMethodHandle.NF_getTarget, CountingWrapper.NF_maybeStopCounting);
			}
		}

		/// <summary>
		/// Constructs simple reinvoker lambda form for a particular method handle </summary>
		private static readonly Function<MethodHandle, LambdaForm> PRODUCE_REINVOKER_FORM = new FunctionAnonymousInnerClassHelper2();

		private class FunctionAnonymousInnerClassHelper2 : Function<MethodHandle, LambdaForm>
		{
			public FunctionAnonymousInnerClassHelper2()
			{
			}

			public virtual LambdaForm Apply(MethodHandle target)
			{
				return DelegatingMethodHandle.MakeReinvokerForm(target, MethodTypeForm.LF_DELEGATE, typeof(DelegatingMethodHandle), DelegatingMethodHandle.NF_getTarget);
			}
		}

		/// <summary>
		/// Counting method handle. It has 2 states: counting and non-counting.
		/// It is in counting state for the first n invocations and then transitions to non-counting state.
		/// Behavior in counting and non-counting states is determined by lambda forms produced by
		/// countingFormProducer & nonCountingFormProducer respectively.
		/// </summary>
		internal class CountingWrapper : DelegatingMethodHandle
		{
			internal readonly MethodHandle Target_Renamed;
			internal int Count;
			internal Function<MethodHandle, LambdaForm> CountingFormProducer;
			internal Function<MethodHandle, LambdaForm> NonCountingFormProducer;
			internal volatile bool IsCounting;

			internal CountingWrapper(MethodHandle target, LambdaForm lform, Function<MethodHandle, LambdaForm> countingFromProducer, Function<MethodHandle, LambdaForm> nonCountingFormProducer, int count) : base(target.Type(), lform)
			{
				this.Target_Renamed = target;
				this.Count = count;
				this.CountingFormProducer = countingFromProducer;
				this.NonCountingFormProducer = nonCountingFormProducer;
				this.IsCounting = (count > 0);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden @Override protected MethodHandle getTarget()
			protected internal override MethodHandle Target
			{
				get
				{
					return Target_Renamed;
				}
			}

			public override MethodHandle AsTypeUncached(MethodType newType)
			{
				MethodHandle newTarget = Target_Renamed.AsType(newType);
				MethodHandle wrapper;
				if (IsCounting)
				{
					LambdaForm lform;
					lform = CountingFormProducer.Apply(newTarget);
					wrapper = new CountingWrapper(newTarget, lform, CountingFormProducer, NonCountingFormProducer, DONT_INLINE_THRESHOLD);
				}
				else
				{
					wrapper = newTarget; // no need for a counting wrapper anymore
				}
				return (AsTypeCache = wrapper);
			}

			internal virtual bool CountDown()
			{
				if (Count <= 0)
				{
					// Try to limit number of updates. MethodHandle.updateForm() doesn't guarantee LF update visibility.
					if (IsCounting)
					{
						IsCounting = false;
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					--Count;
					return false;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static void maybeStopCounting(Object o1)
			internal static void MaybeStopCounting(Object o1)
			{
				 CountingWrapper wrapper = (CountingWrapper) o1;
				 if (wrapper.CountDown())
				 {
					 // Reached invocation threshold. Replace counting behavior with a non-counting one.
					 LambdaForm lform = wrapper.NonCountingFormProducer.Apply(wrapper.Target_Renamed);
					 lform.CompileToBytecode(); // speed up warmup by avoiding LF interpretation again after transition
					 wrapper.UpdateForm(lform);
				 }
			}

			internal static readonly NamedFunction NF_maybeStopCounting;
		}

		internal static LambdaForm MakeGuardWithTestForm(MethodType basicType)
		{
			LambdaForm lform = basicType.Form().CachedLambdaForm(MethodTypeForm.LF_GWT);
			if (lform != null)
			{
				return lform;
			}
			const int THIS_MH = 0; // the BMH_LLL
			const int ARG_BASE = 1; // start of incoming arguments
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_LIMIT = ARG_BASE + basicType.parameterCount();
			int ARG_LIMIT = ARG_BASE + basicType.ParameterCount();
			int nameCursor = ARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_TEST = nameCursor++;
			int GET_TEST = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_TARGET = nameCursor++;
			int GET_TARGET = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_FALLBACK = nameCursor++;
			int GET_FALLBACK = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_COUNTERS = PROFILE_GWT ? nameCursor++ : -1;
			int GET_COUNTERS = PROFILE_GWT ? nameCursor++: -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CALL_TEST = nameCursor++;
			int CALL_TEST = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int PROFILE = (GET_COUNTERS != -1) ? nameCursor++ : -1;
			int PROFILE = (GET_COUNTERS != -1) ? nameCursor++: -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int TEST = nameCursor-1;
			int TEST = nameCursor - 1; // previous statement: either PROFILE or CALL_TEST
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int SELECT_ALT = nameCursor++;
			int SELECT_ALT = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CALL_TARGET = nameCursor++;
			int CALL_TARGET = nameCursor++;
			assert(CALL_TARGET == SELECT_ALT + 1); // must be true to trigger IBG.emitSelectAlternative

			MethodType lambdaType = basicType.InvokerType();
			Name[] names = arguments(nameCursor - ARG_LIMIT, lambdaType);

			BoundMethodHandle.SpeciesData data = (GET_COUNTERS != -1) ? BoundMethodHandle.SpeciesData_LLLL() : BoundMethodHandle.SpeciesData_LLL();
			names[THIS_MH] = names[THIS_MH].withConstraint(data);
			names[GET_TEST] = new Name(data.GetterFunction(0), names[THIS_MH]);
			names[GET_TARGET] = new Name(data.GetterFunction(1), names[THIS_MH]);
			names[GET_FALLBACK] = new Name(data.GetterFunction(2), names[THIS_MH]);
			if (GET_COUNTERS != -1)
			{
				names[GET_COUNTERS] = new Name(data.GetterFunction(3), names[THIS_MH]);
			}
			Object[] invokeArgs = Arrays.CopyOfRange(names, 0, ARG_LIMIT, typeof(Object[]));

			// call test
			MethodType testType = basicType.ChangeReturnType(typeof(bool)).BasicType();
			invokeArgs[0] = names[GET_TEST];
			names[CALL_TEST] = new Name(testType, invokeArgs);

			// profile branch
			if (PROFILE != -1)
			{
				names[PROFILE] = new Name(Lazy.NF_profileBoolean, names[CALL_TEST], names[GET_COUNTERS]);
			}
			// call selectAlternative
			names[SELECT_ALT] = new Name(Lazy.MH_selectAlternative, names[TEST], names[GET_TARGET], names[GET_FALLBACK]);

			// call target or fallback
			invokeArgs[0] = names[SELECT_ALT];
			names[CALL_TARGET] = new Name(basicType, invokeArgs);

			lform = new LambdaForm("guard", lambdaType.ParameterCount(), names, true); //forceInline=

			return basicType.Form().SetCachedLambdaForm(MethodTypeForm.LF_GWT, lform);
		}

		/// <summary>
		/// The LambaForm shape for catchException combinator is the following:
		/// <blockquote><pre>{@code
		///  guardWithCatch=Lambda(a0:L,a1:L,a2:L)=>{
		///    t3:L=BoundMethodHandle$Species_LLLLL.argL0(a0:L);
		///    t4:L=BoundMethodHandle$Species_LLLLL.argL1(a0:L);
		///    t5:L=BoundMethodHandle$Species_LLLLL.argL2(a0:L);
		///    t6:L=BoundMethodHandle$Species_LLLLL.argL3(a0:L);
		///    t7:L=BoundMethodHandle$Species_LLLLL.argL4(a0:L);
		///    t8:L=MethodHandle.invokeBasic(t6:L,a1:L,a2:L);
		///    t9:L=MethodHandleImpl.guardWithCatch(t3:L,t4:L,t5:L,t8:L);
		///   t10:I=MethodHandle.invokeBasic(t7:L,t9:L);t10:I}
		/// }</pre></blockquote>
		/// 
		/// argL0 and argL2 are target and catcher method handles. argL1 is exception class.
		/// argL3 and argL4 are auxiliary method handles: argL3 boxes arguments and wraps them into Object[]
		/// (ValueConversions.array()) and argL4 unboxes result if necessary (ValueConversions.unbox()).
		/// 
		/// Having t8 and t10 passed outside and not hardcoded into a lambda form allows to share lambda forms
		/// among catchException combinators with the same basic type.
		/// </summary>
		private static LambdaForm MakeGuardWithCatchForm(MethodType basicType)
		{
			MethodType lambdaType = basicType.InvokerType();

			LambdaForm lform = basicType.Form().CachedLambdaForm(MethodTypeForm.LF_GWC);
			if (lform != null)
			{
				return lform;
			}
			const int THIS_MH = 0; // the BMH_LLLLL
			const int ARG_BASE = 1; // start of incoming arguments
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ARG_LIMIT = ARG_BASE + basicType.parameterCount();
			int ARG_LIMIT = ARG_BASE + basicType.ParameterCount();

			int nameCursor = ARG_LIMIT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_TARGET = nameCursor++;
			int GET_TARGET = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_CLASS = nameCursor++;
			int GET_CLASS = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_CATCHER = nameCursor++;
			int GET_CATCHER = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_COLLECT_ARGS = nameCursor++;
			int GET_COLLECT_ARGS = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int GET_UNBOX_RESULT = nameCursor++;
			int GET_UNBOX_RESULT = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int BOXED_ARGS = nameCursor++;
			int BOXED_ARGS = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int TRY_CATCH = nameCursor++;
			int TRY_CATCH = nameCursor++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int UNBOX_RESULT = nameCursor++;
			int UNBOX_RESULT = nameCursor++;

			Name[] names = arguments(nameCursor - ARG_LIMIT, lambdaType);

			BoundMethodHandle.SpeciesData data = BoundMethodHandle.SpeciesData_LLLLL();
			names[THIS_MH] = names[THIS_MH].withConstraint(data);
			names[GET_TARGET] = new Name(data.GetterFunction(0), names[THIS_MH]);
			names[GET_CLASS] = new Name(data.GetterFunction(1), names[THIS_MH]);
			names[GET_CATCHER] = new Name(data.GetterFunction(2), names[THIS_MH]);
			names[GET_COLLECT_ARGS] = new Name(data.GetterFunction(3), names[THIS_MH]);
			names[GET_UNBOX_RESULT] = new Name(data.GetterFunction(4), names[THIS_MH]);

			// FIXME: rework argument boxing/result unboxing logic for LF interpretation

			// t_{i}:L=MethodHandle.invokeBasic(collectArgs:L,a1:L,...);
			MethodType collectArgsType = basicType.ChangeReturnType(typeof(Object));
			MethodHandle invokeBasic = MethodHandles.BasicInvoker(collectArgsType);
			Object[] args = new Object[invokeBasic.Type().ParameterCount()];
			args[0] = names[GET_COLLECT_ARGS];
			System.Array.Copy(names, ARG_BASE, args, 1, ARG_LIMIT - ARG_BASE);
			names[BOXED_ARGS] = new Name(MakeIntrinsic(invokeBasic, Intrinsic.GUARD_WITH_CATCH), args);

			// t_{i+1}:L=MethodHandleImpl.guardWithCatch(target:L,exType:L,catcher:L,t_{i}:L);
			Object[] gwcArgs = new Object[] {names[GET_TARGET], names[GET_CLASS], names[GET_CATCHER], names[BOXED_ARGS]};
			names[TRY_CATCH] = new Name(Lazy.NF_guardWithCatch, gwcArgs);

			// t_{i+2}:I=MethodHandle.invokeBasic(unbox:L,t_{i+1}:L);
			MethodHandle invokeBasicUnbox = MethodHandles.BasicInvoker(MethodType.MethodType(basicType.Rtype(), typeof(Object)));
			Object[] unboxArgs = new Object[] {names[GET_UNBOX_RESULT], names[TRY_CATCH]};
			names[UNBOX_RESULT] = new Name(invokeBasicUnbox, unboxArgs);

			lform = new LambdaForm("guardWithCatch", lambdaType.ParameterCount(), names);

			return basicType.Form().SetCachedLambdaForm(MethodTypeForm.LF_GWC, lform);
		}

		internal static MethodHandle MakeGuardWithCatch(MethodHandle target, Class exType, MethodHandle catcher)
		{
			MethodType type = target.Type();
			LambdaForm form = MakeGuardWithCatchForm(type.BasicType());

			// Prepare auxiliary method handles used during LambdaForm interpreation.
			// Box arguments and wrap them into Object[]: ValueConversions.array().
			MethodType varargsType = type.ChangeReturnType(typeof(Object[]));
			MethodHandle collectArgs = VarargsArray(type.ParameterCount()).AsType(varargsType);
			// Result unboxing: ValueConversions.unbox() OR ValueConversions.identity() OR ValueConversions.ignore().
			MethodHandle unboxResult;
			Class rtype = type.ReturnType();
			if (rtype.Primitive)
			{
				if (rtype == typeof(void))
				{
					unboxResult = ValueConversions.ignore();
				}
				else
				{
					Wrapper w = Wrapper.forPrimitiveType(type.ReturnType());
					unboxResult = ValueConversions.unboxExact(w);
				}
			}
			else
			{
				unboxResult = MethodHandles.Identity(typeof(Object));
			}

			BoundMethodHandle.SpeciesData data = BoundMethodHandle.SpeciesData_LLLLL();
			BoundMethodHandle mh;
			try
			{
				mh = (BoundMethodHandle) data.Constructor().invokeBasic(type, form, (Object) target, (Object) exType, (Object) catcher, (Object) collectArgs, (Object) unboxResult);
			}
			catch (Throwable ex)
			{
				throw uncaughtException(ex);
			}
			assert(mh.Type() == type);
			return mh;
		}

		/// <summary>
		/// Intrinsified during LambdaForm compilation
		/// (see <seealso cref="InvokerBytecodeGenerator#emitGuardWithCatch emitGuardWithCatch"/>).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @LambdaForm.Hidden static Object guardWithCatch(MethodHandle target, Class exType, MethodHandle catcher, Object... av) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal static Object GuardWithCatch(MethodHandle target, Class exType, MethodHandle catcher, params Object[] av)
		{
			// Use asFixedArity() to avoid unnecessary boxing of last argument for VarargsCollector case.
			try
			{
				return target.AsFixedArity().InvokeWithArguments(av);
			}
			catch (Throwable t)
			{
				if (!exType.isInstance(t))
				{
					throw t;
				}
				return catcher.AsFixedArity().InvokeWithArguments(Prepend(t, av));
			}
		}

		/// <summary>
		/// Prepend an element {@code elem} to an {@code array}. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @LambdaForm.Hidden private static Object[] prepend(Object elem, Object[] array)
		private static Object[] Prepend(Object elem, Object[] array)
		{
			Object[] newArray = new Object[array.Length + 1];
			newArray[0] = elem;
			System.Array.Copy(array, 0, newArray, 1, array.Length);
			return newArray;
		}

		internal static MethodHandle ThrowException(MethodType type)
		{
			assert(type.ParameterType(0).IsSubclassOf(typeof(Throwable)));
			int arity = type.ParameterCount();
			if (arity > 1)
			{
				MethodHandle mh = ThrowException(type.DropParameterTypes(1, arity));
				mh = MethodHandles.DropArguments(mh, 1, type.ParameterList().subList(1, arity));
				return mh;
			}
			return MakePairwiseConvert(Lazy.NF_throwException.resolvedHandle(), type, false, true);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static <T extends Throwable> sun.invoke.empty.Empty throwException(T t) throws T
		internal static Empty throwException<T>(T t) where T : Throwable
		{
			throw t;
		}

		internal static MethodHandle[] FAKE_METHOD_HANDLE_INVOKE = new MethodHandle[2];
		internal static MethodHandle FakeMethodHandleInvoke(MemberName method)
		{
			int idx;
			assert(method.MethodHandleInvoke);
			switch (method.Name)
			{
			case "invoke":
				idx = 0;
				break;
			case "invokeExact":
				idx = 1;
				break;
			default:
				throw new InternalError(method.Name);
			}
			MethodHandle mh = FAKE_METHOD_HANDLE_INVOKE[idx];
			if (mh != null)
			{
				return mh;
			}
			MethodType type = MethodType.methodType(typeof(Object), typeof(UnsupportedOperationException), typeof(MethodHandle), typeof(Object[]));
			mh = ThrowException(type);
			mh = mh.BindTo(new UnsupportedOperationException("cannot reflectively invoke MethodHandle"));
			if (!method.InvocationType.Equals(mh.Type()))
			{
				throw new InternalError(method.ToString());
			}
			mh = mh.WithInternalMemberName(method, false);
			mh = mh.AsVarargsCollector(typeof(Object[]));
			assert(method.Varargs);
			FAKE_METHOD_HANDLE_INVOKE[idx] = mh;
			return mh;
		}

		/// <summary>
		/// Create an alias for the method handle which, when called,
		/// appears to be called from the same class loader and protection domain
		/// as hostClass.
		/// This is an expensive no-op unless the method which is called
		/// is sensitive to its caller.  A small number of system methods
		/// are in this category, including Class.forName and Method.invoke.
		/// </summary>
		internal static MethodHandle BindCaller(MethodHandle mh, Class hostClass)
		{
			return BindCaller.BindCaller(mh, hostClass);
		}

		// Put the whole mess into its own nested class.
		// That way we can lazily load the code and set up the constants.
		private class BindCaller
		{
			internal static MethodHandle BindCaller(MethodHandle mh, Class hostClass)
			{
				// Do not use this function to inject calls into system classes.
				if (hostClass == null || (hostClass.Array || hostClass.Primitive || hostClass.Name.StartsWith("java.") || hostClass.Name.StartsWith("sun.")))
				{
					throw new InternalError(); // does not happen, and should not anyway
				}
				// For simplicity, convert mh to a varargs-like method.
				MethodHandle vamh = PrepareForInvoker(mh);
				// Cache the result of makeInjectedInvoker once per argument class.
				MethodHandle bccInvoker = CV_makeInjectedInvoker.get(hostClass);
				return RestoreToType(bccInvoker.BindTo(vamh), mh, hostClass);
			}

			internal static MethodHandle MakeInjectedInvoker(Class hostClass)
			{
				Class bcc = UNSAFE.defineAnonymousClass(hostClass, T_BYTES, null);
				if (hostClass.ClassLoader != bcc.ClassLoader)
				{
					throw new InternalError(hostClass.Name + " (CL)");
				}
				try
				{
					if (hostClass.ProtectionDomain != bcc.ProtectionDomain)
					{
						throw new InternalError(hostClass.Name + " (PD)");
					}
				}
				catch (SecurityException)
				{
					// Self-check was blocked by security manager.  This is OK.
					// In fact the whole try body could be turned into an assertion.
				}
				try
				{
					MethodHandle init = IMPL_LOOKUP.findStatic(bcc, "init", MethodType.MethodType(typeof(void)));
					init.invokeExact(); // force initialization of the class
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
				MethodHandle bccInvoker;
				try
				{
					MethodType invokerMT = MethodType.MethodType(typeof(Object), typeof(MethodHandle), typeof(Object[]));
					bccInvoker = IMPL_LOOKUP.findStatic(bcc, "invoke_V", invokerMT);
				}
				catch (ReflectiveOperationException ex)
				{
					throw uncaughtException(ex);
				}
				// Test the invoker, to ensure that it really injects into the right place.
				try
				{
					MethodHandle vamh = PrepareForInvoker(MH_checkCallerClass);
					Object ok = bccInvoker.invokeExact(vamh, new Object[]{hostClass, bcc});
				}
				catch (Throwable ex)
				{
					throw new InternalError(ex);
				}
				return bccInvoker;
			}
			internal static ClassValue<MethodHandle> CV_makeInjectedInvoker = new ClassValueAnonymousInnerClassHelper();

			private class ClassValueAnonymousInnerClassHelper : ClassValue<MethodHandle>
			{
				public ClassValueAnonymousInnerClassHelper()
				{
				}

				protected internal override MethodHandle ComputeValue(Class hostClass)
				{
					return MakeInjectedInvoker(hostClass);
				}
			}

			// Adapt mh so that it can be called directly from an injected invoker:
			internal static MethodHandle PrepareForInvoker(MethodHandle mh)
			{
				mh = mh.AsFixedArity();
				MethodType mt = mh.Type();
				int arity = mt.ParameterCount();
				MethodHandle vamh = mh.AsType(mt.Generic());
				vamh.InternalForm().CompileToBytecode(); // eliminate LFI stack frames
				vamh = vamh.AsSpreader(typeof(Object[]), arity);
				vamh.InternalForm().CompileToBytecode(); // eliminate LFI stack frames
				return vamh;
			}

			// Undo the adapter effect of prepareForInvoker:
			internal static MethodHandle RestoreToType(MethodHandle vamh, MethodHandle original, Class hostClass)
			{
				MethodType type = original.Type();
				MethodHandle mh = vamh.AsCollector(typeof(Object[]), type.ParameterCount());
				MemberName member = original.InternalMemberName();
				mh = mh.AsType(type);
				mh = new WrappedMember(mh, type, member, original.InvokeSpecial, hostClass);
				return mh;
			}

			internal static readonly MethodHandle MH_checkCallerClass;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive private static boolean checkCallerClass(Class expected, Class expected2)
			internal static bool CheckCallerClass(Class expected, Class expected2)
			{
				// This method is called via MH_checkCallerClass and so it's
				// correct to ask for the immediate caller here.
				Class actual = Reflection.CallerClass;
				if (actual != expected && actual != expected2)
				{
					throw new InternalError("found " + actual.Name + ", expected " + expected.Name + (expected == expected2 ? "" : ", or else " + expected2.Name));
				}
				return true;
			}

			internal static readonly sbyte[] T_BYTES;

			// The following class is used as a template for Unsafe.defineAnonymousClass:
			private class T
			{
				internal static void Init() // side effect: initializes this class
				{
				}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object invoke_V(MethodHandle vamh, Object[] args) throws Throwable
				internal static Object Invoke_V(MethodHandle vamh, Object[] args)
				{
					return vamh.invokeExact(args);
				}
			}
		}


		/// <summary>
		/// This subclass allows a wrapped method handle to be re-associated with an arbitrary member name. </summary>
		private sealed class WrappedMember : DelegatingMethodHandle
		{
			internal readonly MethodHandle Target_Renamed;
			internal readonly MemberName Member;
			internal readonly Class CallerClass;
			internal readonly bool IsInvokeSpecial;

			internal WrappedMember(MethodHandle target, MethodType type, MemberName member, bool isInvokeSpecial, Class callerClass) : base(type, target)
			{
				this.Target_Renamed = target;
				this.Member = member;
				this.CallerClass = callerClass;
				this.IsInvokeSpecial = isInvokeSpecial;
			}

			internal override MemberName InternalMemberName()
			{
				return Member;
			}
			internal override Class InternalCallerClass()
			{
				return CallerClass;
			}
			internal override bool InvokeSpecial
			{
				get
				{
					return IsInvokeSpecial;
				}
			}
			protected internal override MethodHandle Target
			{
				get
				{
					return Target_Renamed;
				}
			}
			public override MethodHandle AsTypeUncached(MethodType newType)
			{
				// This MH is an alias for target, except for the MemberName
				// Drop the MemberName if there is any conversion.
				return AsTypeCache = Target_Renamed.AsType(newType);
			}
		}

		internal static MethodHandle MakeWrappedMember(MethodHandle target, MemberName member, bool isInvokeSpecial)
		{
			if (member.Equals(target.InternalMemberName()) && isInvokeSpecial == target.InvokeSpecial)
			{
				return target;
			}
			return new WrappedMember(target, target.Type(), member, isInvokeSpecial, null);
		}

		/// <summary>
		/// Intrinsic IDs </summary>
		/*non-public*/
		internal enum Intrinsic
		{
			SELECT_ALTERNATIVE,
			GUARD_WITH_CATCH,
			NEW_ARRAY,
			ARRAY_LOAD,
			ARRAY_STORE,
			IDENTITY,
			ZERO,
			NONE // no intrinsic associated
		}

		/// <summary>
		/// Mark arbitrary method handle as intrinsic.
		/// InvokerBytecodeGenerator uses this info to produce more efficient bytecode shape. 
		/// </summary>
		private sealed class IntrinsicMethodHandle : DelegatingMethodHandle
		{
			internal readonly MethodHandle Target_Renamed;
			internal readonly Intrinsic IntrinsicName_Renamed;

			internal IntrinsicMethodHandle(MethodHandle target, Intrinsic intrinsicName) : base(target.Type(), target)
			{
				this.Target_Renamed = target;
				this.IntrinsicName_Renamed = intrinsicName;
			}

			protected internal override MethodHandle Target
			{
				get
				{
					return Target_Renamed;
				}
			}

			internal override Intrinsic IntrinsicName()
			{
				return IntrinsicName_Renamed;
			}

			public override MethodHandle AsTypeUncached(MethodType newType)
			{
				// This MH is an alias for target, except for the intrinsic name
				// Drop the name if there is any conversion.
				return AsTypeCache = Target_Renamed.AsType(newType);
			}

			internal override String InternalProperties()
			{
				return base.InternalProperties() + "\n& Intrinsic=" + IntrinsicName_Renamed;
			}

			public override MethodHandle AsCollector(Class arrayType, int arrayLength)
			{
				if (IntrinsicName_Renamed == Intrinsic.IDENTITY)
				{
					MethodType resultType = Type().AsCollectorType(arrayType, arrayLength);
					MethodHandle newArray = MethodHandleImpl.VarargsArray(arrayType, arrayLength);
					return newArray.AsType(resultType);
				}
				return base.AsCollector(arrayType, arrayLength);
			}
		}

		internal static MethodHandle MakeIntrinsic(MethodHandle target, Intrinsic intrinsicName)
		{
			if (intrinsicName == target.IntrinsicName())
			{
				return target;
			}
			return new IntrinsicMethodHandle(target, intrinsicName);
		}

		internal static MethodHandle MakeIntrinsic(MethodType type, LambdaForm form, Intrinsic intrinsicName)
		{
			return new IntrinsicMethodHandle(SimpleMethodHandle.Make(type, form), intrinsicName);
		}

		/// Collection of multiple arguments.

		private static MethodHandle FindCollector(String name, int nargs, Class rtype, params Class[] ptypes)
		{
			MethodType type = MethodType.GenericMethodType(nargs).ChangeReturnType(rtype).InsertParameterTypes(0, ptypes);
			try
			{
				return IMPL_LOOKUP.findStatic(typeof(MethodHandleImpl), name, type);
			}
			catch (ReflectiveOperationException)
			{
				return null;
			}
		}

		private static readonly Object[] NO_ARGS_ARRAY = new Object[] {};
		private static Object[] MakeArray(params Object[] args)
		{
			return args;
		}
		private static Object[] Array()
		{
			return NO_ARGS_ARRAY;
		}
		private static Object[] Array(Object a0)
		{
						return MakeArray(a0);
		}
		private static Object[] Array(Object a0, Object a1)
		{
						return makeArray(a0, a1);
		}
		private static Object[] Array(Object a0, Object a1, Object a2)
		{
						return makeArray(a0, a1, a2);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3)
		{
						return makeArray(a0, a1, a2, a3);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4)
		{
						return makeArray(a0, a1, a2, a3, a4);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4, Object a5)
		{
						return makeArray(a0, a1, a2, a3, a4, a5);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6)
		{
						return makeArray(a0, a1, a2, a3, a4, a5, a6);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7)
		{
						return makeArray(a0, a1, a2, a3, a4, a5, a6, a7);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7, Object a8)
		{
						return makeArray(a0, a1, a2, a3, a4, a5, a6, a7, a8);
		}
		private static Object[] Array(Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7, Object a8, Object a9)
		{
						return makeArray(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}
		private static MethodHandle[] MakeArrays()
		{
			List<MethodHandle> mhs = new List<MethodHandle>();
			for (;;)
			{
				MethodHandle mh = findCollector("array", mhs.Count, typeof(Object[]));
				if (mh == null)
				{
					break;
				}
				mh = MakeIntrinsic(mh, Intrinsic.NEW_ARRAY);
				mhs.Add(mh);
			}
			assert(mhs.Count == 11); // current number of methods
			return mhs.ToArray();
		}

		// filling versions of the above:
		// using Integer len instead of int len and no varargs to avoid bootstrapping problems
		private static Object[] FillNewArray(Integer len, Object[] args) //not ...
		{
			Object[] a = new Object[len];
			FillWithArguments(a, 0, args);
			return a;
		}
		private static Object[] FillNewTypedArray(Object[] example, Integer len, Object[] args) //not ...
		{
			Object[] a = Arrays.CopyOf(example, len);
			assert(a.GetType() != typeof(Object[]));
			FillWithArguments(a, 0, args);
			return a;
		}
		private static void FillWithArguments(Object[] a, int pos, params Object[] args)
		{
			System.Array.Copy(args, 0, a, pos, args.Length);
		}
		// using Integer pos instead of int pos to avoid bootstrapping problems
		private static Object[] FillArray(Integer pos, Object[] a, Object a0)
		{
						FillWithArguments(a, pos, a0);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1)
		{
						fillWithArguments(a, pos, a0, a1);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2)
		{
						fillWithArguments(a, pos, a0, a1, a2);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4, Object a5)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7, Object a8)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7, a8);
						return a;
		}
		private static Object[] FillArray(Integer pos, Object[] a, Object a0, Object a1, Object a2, Object a3, Object a4, Object a5, Object a6, Object a7, Object a8, Object a9)
		{
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
						return a;
		}

		private const int FILL_ARRAYS_COUNT = 11; // current number of fillArray methods

		private static MethodHandle[] MakeFillArrays()
		{
			List<MethodHandle> mhs = new List<MethodHandle>();
			mhs.Add(null); // there is no empty fill; at least a0 is required
			for (;;)
			{
				MethodHandle mh = findCollector("fillArray", mhs.Count, typeof(Object[]), typeof(Integer), typeof(Object[]));
				if (mh == null)
				{
					break;
				}
				mhs.Add(mh);
			}
			assert(mhs.Count == FILL_ARRAYS_COUNT);
			return mhs.ToArray();
		}

		private static Object CopyAsPrimitiveArray(Wrapper w, params Object[] boxes)
		{
			Object a = w.makeArray(boxes.Length);
			w.copyArrayUnboxing(boxes, 0, a, 0, boxes.Length);
			return a;
		}

		/// <summary>
		/// Return a method handle that takes the indicated number of Object
		///  arguments and returns an Object array of them, as if for varargs.
		/// </summary>
		internal static MethodHandle VarargsArray(int nargs)
		{
			MethodHandle mh = Lazy.ARRAYS[nargs];
			if (mh != null)
			{
				return mh;
			}
			mh = findCollector("array", nargs, typeof(Object[]));
			if (mh != null)
			{
				mh = MakeIntrinsic(mh, Intrinsic.NEW_ARRAY);
			}
			if (mh != null)
			{
				return Lazy.ARRAYS[nargs] = mh;
			}
			mh = BuildVarargsArray(Lazy.MH_fillNewArray, Lazy.MH_arrayIdentity, nargs);
			assert(AssertCorrectArity(mh, nargs));
			mh = MakeIntrinsic(mh, Intrinsic.NEW_ARRAY);
			return Lazy.ARRAYS[nargs] = mh;
		}

		private static bool AssertCorrectArity(MethodHandle mh, int arity)
		{
			assert(mh.Type().ParameterCount() == arity) : "arity != " + arity + ": " + mh;
			return true;
		}

		// Array identity function (used as Lazy.MH_arrayIdentity).
		internal static T[] identity<T>(T[] x)
		{
			return x;
		}

		private static MethodHandle BuildVarargsArray(MethodHandle newArray, MethodHandle finisher, int nargs)
		{
			// Build up the result mh as a sequence of fills like this:
			//   finisher(fill(fill(newArrayWA(23,x1..x10),10,x11..x20),20,x21..x23))
			// The various fill(_,10*I,___*[J]) are reusable.
			int leftLen = System.Math.Min(nargs, LEFT_ARGS); // absorb some arguments immediately
			int rightLen = nargs - leftLen;
			MethodHandle leftCollector = newArray.BindTo(nargs);
			leftCollector = leftCollector.AsCollector(typeof(Object[]), leftLen);
			MethodHandle mh = finisher;
			if (rightLen > 0)
			{
				MethodHandle rightFiller = FillToRight(LEFT_ARGS + rightLen);
				if (mh == Lazy.MH_arrayIdentity)
				{
					mh = rightFiller;
				}
				else
				{
					mh = MethodHandles.CollectArguments(mh, 0, rightFiller);
				}
			}
			if (mh == Lazy.MH_arrayIdentity)
			{
				mh = leftCollector;
			}
			else
			{
				mh = MethodHandles.CollectArguments(mh, 0, leftCollector);
			}
			return mh;
		}

		private static readonly int LEFT_ARGS = FILL_ARRAYS_COUNT - 1;
		private static readonly MethodHandle[] FILL_ARRAY_TO_RIGHT = new MethodHandle[MAX_ARITY + 1];
		/// <summary>
		/// fill_array_to_right(N).invoke(a, argL..arg[N-1])
		///  fills a[L]..a[N-1] with corresponding arguments,
		///  and then returns a.  The value L is a global constant (LEFT_ARGS).
		/// </summary>
		private static MethodHandle FillToRight(int nargs)
		{
			MethodHandle filler = FILL_ARRAY_TO_RIGHT[nargs];
			if (filler != null)
			{
				return filler;
			}
			filler = BuildFiller(nargs);
			assert(AssertCorrectArity(filler, nargs - LEFT_ARGS + 1));
			return FILL_ARRAY_TO_RIGHT[nargs] = filler;
		}
		private static MethodHandle BuildFiller(int nargs)
		{
			if (nargs <= LEFT_ARGS)
			{
				return Lazy.MH_arrayIdentity; // no args to fill; return the array unchanged
			}
			// we need room for both mh and a in mh.invoke(a, arg*[nargs])
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int CHUNK = LEFT_ARGS;
			int CHUNK = LEFT_ARGS;
			int rightLen = nargs % CHUNK;
			int midLen = nargs - rightLen;
			if (rightLen == 0)
			{
				midLen = nargs - (rightLen = CHUNK);
				if (FILL_ARRAY_TO_RIGHT[midLen] == null)
				{
					// build some precursors from left to right
					for (int j = LEFT_ARGS % CHUNK; j < midLen; j += CHUNK)
					{
						if (j > LEFT_ARGS)
						{
							FillToRight(j);
						}
					}
				}
			}
			if (midLen < LEFT_ARGS)
			{
				rightLen = nargs - (midLen = LEFT_ARGS);
			}
			assert(rightLen > 0);
			MethodHandle midFill = FillToRight(midLen); // recursive fill
			MethodHandle rightFill = Lazy.FILL_ARRAYS[rightLen].BindTo(midLen); // [midLen..nargs-1]
			assert(midFill.Type().ParameterCount() == 1 + midLen - LEFT_ARGS);
			assert(rightFill.Type().ParameterCount() == 1 + rightLen);

			// Combine the two fills:
			//   right(mid(a, x10..x19), x20..x23)
			// The final product will look like this:
			//   right(mid(newArrayLeft(24, x0..x9), x10..x19), x20..x23)
			if (midLen == LEFT_ARGS)
			{
				return rightFill;
			}
			else
			{
				return MethodHandles.CollectArguments(rightFill, 0, midFill);
			}
		}

		// Type-polymorphic version of varargs maker.
		private static readonly ClassValue<MethodHandle[]> TYPED_COLLECTORS = new ClassValueAnonymousInnerClassHelper();

		private class ClassValueAnonymousInnerClassHelper : ClassValue<MethodHandle[]>
		{
			public ClassValueAnonymousInnerClassHelper()
			{
			}

			protected internal override MethodHandle[] ComputeValue(Class type)
			{
				return new MethodHandle[256];
			}
		}

		internal const int MAX_JVM_ARITY = 255; // limit imposed by the JVM

		/// <summary>
		/// Return a method handle that takes the indicated number of
		///  typed arguments and returns an array of them.
		///  The type argument is the array type.
		/// </summary>
		internal static MethodHandle VarargsArray(Class arrayType, int nargs)
		{
			Class elemType = arrayType.ComponentType;
			if (elemType == null)
			{
				throw new IllegalArgumentException("not an array: " + arrayType);
			}
			// FIXME: Need more special casing and caching here.
			if (nargs >= MAX_JVM_ARITY / 2 - 1)
			{
				int slots = nargs;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int MAX_ARRAY_SLOTS = MAX_JVM_ARITY - 1;
				int MAX_ARRAY_SLOTS = MAX_JVM_ARITY - 1; // 1 for receiver MH
				if (slots <= MAX_ARRAY_SLOTS && elemType.Primitive)
				{
					slots *= Wrapper.forPrimitiveType(elemType).stackSlots();
				}
				if (slots > MAX_ARRAY_SLOTS)
				{
					throw new IllegalArgumentException("too many arguments: " + arrayType.SimpleName + ", length " + nargs);
				}
			}
			if (elemType == typeof(Object))
			{
				return VarargsArray(nargs);
			}
			// other cases:  primitive arrays, subtypes of Object[]
			MethodHandle[] cache = TYPED_COLLECTORS.get(elemType);
			MethodHandle mh = nargs < cache.Length ? cache[nargs] : null;
			if (mh != null)
			{
				return mh;
			}
			if (nargs == 0)
			{
				Object example = java.lang.reflect.Array.NewInstance(arrayType.ComponentType, 0);
				mh = MethodHandles.Constant(arrayType, example);
			}
			else if (elemType.Primitive)
			{
				MethodHandle builder = Lazy.MH_fillNewArray;
				MethodHandle producer = BuildArrayProducer(arrayType);
				mh = BuildVarargsArray(builder, producer, nargs);
			}
			else
			{
				Class objArrayType = arrayType.AsSubclass(typeof(Object[]));
				Object[] example = Arrays.CopyOf(NO_ARGS_ARRAY, 0, objArrayType);
				MethodHandle builder = Lazy.MH_fillNewTypedArray.BindTo(example);
				MethodHandle producer = Lazy.MH_arrayIdentity; // must be weakly typed
				mh = BuildVarargsArray(builder, producer, nargs);
			}
			mh = mh.AsType(MethodType.MethodType(arrayType, Collections.NCopies<Class>(nargs, elemType)));
			mh = MakeIntrinsic(mh, Intrinsic.NEW_ARRAY);
			assert(AssertCorrectArity(mh, nargs));
			if (nargs < cache.Length)
			{
				cache[nargs] = mh;
			}
			return mh;
		}

		private static MethodHandle BuildArrayProducer(Class arrayType)
		{
			Class elemType = arrayType.ComponentType;
			assert(elemType.Primitive);
			return Lazy.MH_copyAsPrimitiveArray.BindTo(Wrapper.forPrimitiveType(elemType));
		}

		/*non-public*/	 internal static void AssertSame(Object mh1, Object mh2)
	 {
			if (mh1 != mh2)
			{
				String msg = string.Format("mh1 != mh2: mh1 = {0} (form: {1}); mh2 = {2} (form: {3})", mh1, ((MethodHandle)mh1).Form, mh2, ((MethodHandle)mh2).Form);
				throw newInternalError(msg);
			}
	 }
	 }

}