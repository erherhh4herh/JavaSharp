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

	using Wrapper = sun.invoke.util.Wrapper;

	/// <summary>
	/// Shared information for a group of method types, which differ
	/// only by reference types, and therefore share a common erasure
	/// and wrapping.
	/// <para>
	/// For an empirical discussion of the structure of method types,
	/// see <a href="http://groups.google.com/group/jvm-languages/browse_thread/thread/ac9308ae74da9b7e/">
	/// the thread "Avoiding Boxing" on jvm-languages</a>.
	/// There are approximately 2000 distinct erased method types in the JDK.
	/// There are a little over 10 times that number of unerased types.
	/// No more than half of these are likely to be loaded at once.
	/// @author John Rose
	/// </para>
	/// </summary>
	internal sealed class MethodTypeForm
	{
		internal readonly int[] ArgToSlotTable, SlotToArgTable;
		internal readonly long ArgCounts; // packed slot & value counts
		internal readonly long PrimCounts; // packed prim & double counts
		internal readonly MethodType ErasedType_Renamed; // the canonical erasure
		internal readonly MethodType BasicType_Renamed; // the canonical erasure, with primitives simplified

		// Cached adapter information:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable final SoftReference<MethodHandle>[] methodHandles;
		internal readonly SoftReference<MethodHandle>[] MethodHandles;
		// Indexes into methodHandles:
		internal const int MH_BASIC_INV = 0, MH_NF_INV = 1, MH_UNINIT_CS = 2, MH_LIMIT = 3; // uninitialized call site -  cached helper for LF.NamedFunction -  cached instance of MH.invokeBasic

		// Cached lambda form information, for basic types only:
		internal readonly  Stable;
		// Indexes into lambdaForms:
		internal const int LF_INVVIRTUAL = 0, LF_INVSTATIC = 1, LF_INVSPECIAL = 2, LF_NEWINVSPECIAL = 3, LF_INVINTERFACE = 4, LF_INVSTATIC_INIT = 5, LF_INTERPRET = 6, LF_REBIND = 7, LF_DELEGATE = 8, LF_DELEGATE_BLOCK_INLINING = 9, LF_EX_LINKER = 10, LF_EX_INVOKER = 11, LF_GEN_LINKER = 12, LF_GEN_INVOKER = 13, LF_CS_LINKER = 14, LF_MH_LINKER = 15, LF_GWC = 16, LF_GWT = 17, LF_LIMIT = 18; // guardWithTest -  guardWithCatch (catchException) -  linkToCallSite_MH -  linkToCallSite_CS -  generic MHs.invoke -  generic invoke_MT (for invokehandle) -  MHs.invokeExact -  invokeExact_MT (for invokehandle) -  Counting DelegatingMethodHandle w/ @DontInline -  DelegatingMethodHandle -  BoundMethodHandle -  LF interpreter -  DMH invokeStatic with <clinit> barrier -  DMH invokeVirtual

		/// <summary>
		/// Return the type corresponding uniquely (1-1) to this MT-form.
		///  It might have any primitive returns or arguments, but will have no references except Object.
		/// </summary>
		public MethodType ErasedType()
		{
			return ErasedType_Renamed;
		}

		/// <summary>
		/// Return the basic type derived from the erased type of this MT-form.
		///  A basic type is erased (all references Object) and also has all primitive
		///  types (except int, long, float, double, void) normalized to int.
		///  Such basic types correspond to low-level JVM calling sequences.
		/// </summary>
		public MethodType BasicType()
		{
			return BasicType_Renamed;
		}

		private bool AssertIsBasicType()
		{
			// primitives must be flattened also
			assert(ErasedType_Renamed == BasicType_Renamed) : "erasedType: " + ErasedType_Renamed + " != basicType: " + BasicType_Renamed;
			return true;
		}

		public MethodHandle CachedMethodHandle(int which)
		{
			assert(AssertIsBasicType());
			SoftReference<MethodHandle> entry = MethodHandles[which];
			return (entry != null) ? entry.get() : null;
		}

		public MethodHandle SetCachedMethodHandle(int which, MethodHandle mh)
		{
			lock (this)
			{
				// Simulate a CAS, to avoid racy duplication of results.
				SoftReference<MethodHandle> entry = MethodHandles[which];
				if (entry != null)
				{
					MethodHandle prev = entry.get();
					if (prev != null)
					{
						return prev;
					}
				}
				MethodHandles[which] = new SoftReference<>(mh);
				return mh;
			}
		}

		public LambdaForm CachedLambdaForm(int which)
		{
			assert(AssertIsBasicType());
			SoftReference<LambdaForm> entry = lambdaForms[which];
			return (entry != null) ? entry.get() : null;
		}

		public LambdaForm SetCachedLambdaForm(int which, LambdaForm form)
		{
			lock (this)
			{
				// Simulate a CAS, to avoid racy duplication of results.
				SoftReference<LambdaForm> entry = lambdaForms[which];
				if (entry != null)
				{
					LambdaForm prev = entry.get();
					if (prev != null)
					{
						return prev;
					}
				}
				lambdaForms[which] = new SoftReference<>(form);
				return form;
			}
		}

		/// <summary>
		/// Build an MTF for a given type, which must have all references erased to Object.
		/// This MTF will stand for that type and all un-erased variations.
		/// Eagerly compute some basic properties of the type, common to all variations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) protected MethodTypeForm(MethodType erasedType)
		protected internal MethodTypeForm(MethodType erasedType)
		{
			this.ErasedType_Renamed = erasedType;

			Class[] ptypes = erasedType.Ptypes();
			int ptypeCount = ptypes.Length;
			int pslotCount = ptypeCount; // temp. estimate
			int rtypeCount = 1; // temp. estimate
			int rslotCount = 1; // temp. estimate

			int[] argToSlotTab = null, slotToArgTab = null;

			// Walk the argument types, looking for primitives.
			int pac = 0, lac = 0, prc = 0, lrc = 0;
			Class[] epts = ptypes;
			Class[] bpts = epts;
			for (int i = 0; i < epts.Length; i++)
			{
				Class pt = epts[i];
				if (pt != typeof(Object))
				{
					++pac;
					Wrapper w = Wrapper.forPrimitiveType(pt);
					if (w.DoubleWord)
					{
						++lac;
					}
					if (w.SubwordOrInt && pt != typeof(int))
					{
						if (bpts == epts)
						{
							bpts = bpts.clone();
						}
						bpts[i] = typeof(int);
					}
				}
			}
			pslotCount += lac; // #slots = #args + #longs
			Class rt = erasedType.ReturnType();
			Class bt = rt;
			if (rt != typeof(Object))
			{
				++prc; // even void.class counts as a prim here
				Wrapper w = Wrapper.forPrimitiveType(rt);
				if (w.DoubleWord)
				{
					++lrc;
				}
				if (w.SubwordOrInt && rt != typeof(int))
				{
					bt = typeof(int);
				}
				// adjust #slots, #args
				if (rt == typeof(void))
				{
					rtypeCount = rslotCount = 0;
				}
				else
				{
					rslotCount += lrc;
				}
			}
			if (epts == bpts && bt == rt)
			{
				this.BasicType_Renamed = erasedType;
			}
			else
			{
				this.BasicType_Renamed = MethodType.MakeImpl(bt, bpts, true);
				// fill in rest of data from the basic type:
				MethodTypeForm that = this.BasicType_Renamed.Form();
				assert(this != that);
				this.PrimCounts = that.PrimCounts;
				this.ArgCounts = that.ArgCounts;
				this.ArgToSlotTable = that.ArgToSlotTable;
				this.SlotToArgTable = that.SlotToArgTable;
				this.MethodHandles = null;
				this.lambdaForms = null;
				return;
			}
			if (lac != 0)
			{
				int slot = ptypeCount + lac;
				slotToArgTab = new int[slot + 1];
				argToSlotTab = new int[1 + ptypeCount];
				argToSlotTab[0] = slot; // argument "-1" is past end of slots
				for (int i = 0; i < epts.Length; i++)
				{
					Class pt = epts[i];
					Wrapper w = Wrapper.forBasicType(pt);
					if (w.DoubleWord)
					{
						--slot;
					}
					--slot;
					slotToArgTab[slot] = i + 1; // "+1" see argSlotToParameter note
					argToSlotTab[1 + i] = slot;
				}
				assert(slot == 0); // filled the table
			}
			else if (pac != 0)
			{
				// have primitives but no long primitives; share slot counts with generic
				assert(ptypeCount == pslotCount);
				MethodTypeForm that = MethodType.GenericMethodType(ptypeCount).Form();
				assert(this != that);
				slotToArgTab = that.SlotToArgTable;
				argToSlotTab = that.ArgToSlotTable;
			}
			else
			{
				int slot = ptypeCount; // first arg is deepest in stack
				slotToArgTab = new int[slot + 1];
				argToSlotTab = new int[1 + ptypeCount];
				argToSlotTab[0] = slot; // argument "-1" is past end of slots
				for (int i = 0; i < ptypeCount; i++)
				{
					--slot;
					slotToArgTab[slot] = i + 1; // "+1" see argSlotToParameter note
					argToSlotTab[1 + i] = slot;
				}
			}
			this.PrimCounts = Pack(lrc, prc, lac, pac);
			this.ArgCounts = Pack(rslotCount, rtypeCount, pslotCount, ptypeCount);
			this.ArgToSlotTable = argToSlotTab;
			this.SlotToArgTable = slotToArgTab;

			if (pslotCount >= 256)
			{
				throw newIllegalArgumentException("too many arguments");
			}

			// Initialize caches, but only for basic types
			assert(BasicType_Renamed == erasedType);
			this.lambdaForms = new SoftReference[LF_LIMIT];
			this.MethodHandles = new SoftReference[MH_LIMIT];
		}

		private static long Pack(int a, int b, int c, int d)
		{
			assert(((a | b | c | d) & ~0xFFFF) == 0);
			long hw = ((a << 16) | b), lw = ((c << 16) | d);
			return (hw << 32) | lw;
		}
		private static char Unpack(long packed, int word) // word==0 => return a, ==3 => return d
		{
			assert(word <= 3);
			return (char)(packed >> ((3 - word) * 16));
		}

		public int ParameterCount() // # outgoing values
		{
			return Unpack(ArgCounts, 3);
		}
		public int ParameterSlotCount() // # outgoing interpreter slots
		{
			return Unpack(ArgCounts, 2);
		}
		public int ReturnCount() // = 0 (V), or 1
		{
			return Unpack(ArgCounts, 1);
		}
		public int ReturnSlotCount() // = 0 (V), 2 (J/D), or 1
		{
			return Unpack(ArgCounts, 0);
		}
		public int PrimitiveParameterCount()
		{
			return Unpack(PrimCounts, 3);
		}
		public int LongPrimitiveParameterCount()
		{
			return Unpack(PrimCounts, 2);
		}
		public int PrimitiveReturnCount() // = 0 (obj), or 1
		{
			return Unpack(PrimCounts, 1);
		}
		public int LongPrimitiveReturnCount() // = 1 (J/D), or 0
		{
			return Unpack(PrimCounts, 0);
		}
		public bool HasPrimitives()
		{
			return PrimCounts != 0;
		}
		public bool HasNonVoidPrimitives()
		{
			if (PrimCounts == 0)
			{
				return false;
			}
			if (PrimitiveParameterCount() != 0)
			{
				return true;
			}
			return (PrimitiveReturnCount() != 0 && ReturnCount() != 0);
		}
		public bool HasLongPrimitives()
		{
			return (LongPrimitiveParameterCount() | LongPrimitiveReturnCount()) != 0;
		}
		public int ParameterToArgSlot(int i)
		{
			return ArgToSlotTable[1 + i];
		}
		public int ArgSlotToParameter(int argSlot)
		{
			// Note:  Empty slots are represented by zero in this table.
			// Valid arguments slots contain incremented entries, so as to be non-zero.
			// We return -1 the caller to mean an empty slot.
			return SlotToArgTable[argSlot] - 1;
		}

		internal static MethodTypeForm FindForm(MethodType mt)
		{
			MethodType erased = Canonicalize(mt, ERASE, ERASE);
			if (erased == null)
			{
				// It is already erased.  Make a new MethodTypeForm.
				return new MethodTypeForm(mt);
			}
			else
			{
				// Share the MethodTypeForm with the erased version.
				return erased.Form();
			}
		}

		/// <summary>
		/// Codes for <seealso cref="#canonicalize(java.lang.Class, int)"/>.
		/// ERASE means change every reference to {@code Object}.
		/// WRAP means convert primitives (including {@code void} to their
		/// corresponding wrapper types.  UNWRAP means the reverse of WRAP.
		/// INTS means convert all non-void primitive types to int or long,
		/// according to size.  LONGS means convert all non-void primitives
		/// to long, regardless of size.  RAW_RETURN means convert a type
		/// (assumed to be a return type) to int if it is smaller than an int,
		/// or if it is void.
		/// </summary>
		public const int NO_CHANGE = 0, ERASE = 1, WRAP = 2, UNWRAP = 3, INTS = 4, LONGS = 5, RAW_RETURN = 6;

		/// <summary>
		/// Canonicalize the types in the given method type.
		/// If any types change, intern the new type, and return it.
		/// Otherwise return null.
		/// </summary>
		public static MethodType Canonicalize(MethodType mt, int howRet, int howArgs)
		{
			Class[] ptypes = mt.Ptypes();
			Class[] ptc = MethodTypeForm.CanonicalizeAll(ptypes, howArgs);
			Class rtype = mt.ReturnType();
			Class rtc = MethodTypeForm.Canonicalize(rtype, howRet);
			if (ptc == null && rtc == null)
			{
				// It is already canonical.
				return null;
			}
			// Find the erased version of the method type:
			if (rtc == null)
			{
				rtc = rtype;
			}
			if (ptc == null)
			{
				ptc = ptypes;
			}
			return MethodType.MakeImpl(rtc, ptc, true);
		}

		/// <summary>
		/// Canonicalize the given return or param type.
		///  Return null if the type is already canonicalized.
		/// </summary>
		internal static Class Canonicalize(Class t, int how)
		{
			Class ct;
			if (t == typeof(Object))
			{
				// no change, ever
			}
			else if (!t.Primitive)
			{
				switch (how)
				{
					case UNWRAP:
						ct = Wrapper.asPrimitiveType(t);
						if (ct != t)
						{
							return ct;
						}
						break;
					case RAW_RETURN:
					case ERASE:
						return typeof(Object);
				}
			}
			else if (t == typeof(void))
			{
				// no change, usually
				switch (how)
				{
					case RAW_RETURN:
						return typeof(int);
					case WRAP:
						return typeof(Void);
				}
			}
			else
			{
				// non-void primitive
				switch (how)
				{
					case WRAP:
						return Wrapper.asWrapperType(t);
					case INTS:
						if (t == typeof(int) || t == typeof(long))
						{
							return null; // no change
						}
						if (t == typeof(double))
						{
							return typeof(long);
						}
						return typeof(int);
					case LONGS:
						if (t == typeof(long))
						{
							return null; // no change
						}
						return typeof(long);
					case RAW_RETURN:
						if (t == typeof(int) || t == typeof(long) || t == typeof(float) || t == typeof(double))
						{
							return null; // no change
						}
						// everything else returns as an int
						return typeof(int);
				}
			}
			// no change; return null to signify
			return null;
		}

		/// <summary>
		/// Canonicalize each param type in the given array.
		///  Return null if all types are already canonicalized.
		/// </summary>
		internal static Class[] CanonicalizeAll(Class[] ts, int how)
		{
			Class[] cs = null;
			for (int imax = ts.Length, i = 0; i < imax; i++)
			{
				Class c = Canonicalize(ts[i], how);
				if (c == typeof(void))
				{
					c = null; // a Void parameter was unwrapped to void; ignore
				}
				if (c != null)
				{
					if (cs == null)
					{
						cs = ts.clone();
					}
					cs[i] = c;
				}
			}
			return cs;
		}

		public override String ToString()
		{
			return "Form" + ErasedType_Renamed;
		}
	}

}