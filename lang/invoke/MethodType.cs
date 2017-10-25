using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
	using BytecodeDescriptor = sun.invoke.util.BytecodeDescriptor;
	using VerifyType = sun.invoke.util.VerifyType;

	/// <summary>
	/// A method type represents the arguments and return type accepted and
	/// returned by a method handle, or the arguments and return type passed
	/// and expected  by a method handle caller.  Method types must be properly
	/// matched between a method handle and all its callers,
	/// and the JVM's operations enforce this matching at, specifically
	/// during calls to <seealso cref="MethodHandle#invokeExact MethodHandle.invokeExact"/>
	/// and <seealso cref="MethodHandle#invoke MethodHandle.invoke"/>, and during execution
	/// of {@code invokedynamic} instructions.
	/// <para>
	/// The structure is a return type accompanied by any number of parameter types.
	/// The types (primitive, {@code void}, and reference) are represented by <seealso cref="Class"/> objects.
	/// (For ease of exposition, we treat {@code void} as if it were a type.
	/// In fact, it denotes the absence of a return type.)
	/// </para>
	/// <para>
	/// All instances of {@code MethodType} are immutable.
	/// Two instances are completely interchangeable if they compare equal.
	/// Equality depends on pairwise correspondence of the return and parameter types and on nothing else.
	/// </para>
	/// <para>
	/// This type can be created only by factory methods.
	/// All factory methods may cache values, though caching is not guaranteed.
	/// Some factory methods are static, while others are virtual methods which
	/// modify precursor method types, e.g., by changing a selected parameter.
	/// </para>
	/// <para>
	/// Factory methods which operate on groups of parameter types
	/// are systematically presented in two versions, so that both Java arrays and
	/// Java lists can be used to work with groups of parameter types.
	/// The query methods {@code parameterArray} and {@code parameterList}
	/// also provide a choice between arrays and lists.
	/// </para>
	/// <para>
	/// {@code MethodType} objects are sometimes derived from bytecode instructions
	/// such as {@code invokedynamic}, specifically from the type descriptor strings associated
	/// with the instructions in a class file's constant pool.
	/// </para>
	/// <para>
	/// Like classes and strings, method types can also be represented directly
	/// in a class file's constant pool as constants.
	/// A method type may be loaded by an {@code ldc} instruction which refers
	/// to a suitable {@code CONSTANT_MethodType} constant pool entry.
	/// The entry refers to a {@code CONSTANT_Utf8} spelling for the descriptor string.
	/// (For full details on method type constants,
	/// see sections 4.4.8 and 5.4.3.5 of the Java Virtual Machine Specification.)
	/// </para>
	/// <para>
	/// When the JVM materializes a {@code MethodType} from a descriptor string,
	/// all classes named in the descriptor must be accessible, and will be loaded.
	/// (But the classes need not be initialized, as is the case with a {@code CONSTANT_Class}.)
	/// This loading may occur at any time before the {@code MethodType} object is first derived.
	/// @author John Rose, JSR 292 EG
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class MethodType
	{
		private const long SerialVersionUID = 292L; // {rtype, {ptype...}}

		// The rtype and ptypes fields define the structural identity of the method type:
		private readonly Class Rtype_Renamed;
		private readonly Class[] Ptypes_Renamed;

		// The remaining fields are caches of various sorts:
		private  Stable; // erased form, plus cached data about primitives
		private  Stable; // alternative wrapped/unwrapped version
		private  Stable; // cache of handy higher-order adapters
		private  Stable; // cache for toMethodDescriptorString

		/// <summary>
		/// Check the given parameters for validity and store them into the final fields.
		/// </summary>
		private MethodType(Class rtype, Class[] ptypes, bool trusted)
		{
			CheckRtype(rtype);
			CheckPtypes(ptypes);
			this.Rtype_Renamed = rtype;
			// defensively copy the array passed in by the user
			this.Ptypes_Renamed = trusted ? ptypes : Arrays.CopyOf(ptypes, ptypes.Length);
		}

		/// <summary>
		/// Construct a temporary unchecked instance of MethodType for use only as a key to the intern table.
		/// Does not check the given parameters for validity, and must be discarded after it is used as a searching key.
		/// The parameters are reversed for this constructor, so that is is not accidentally used.
		/// </summary>
		private MethodType(Class[] ptypes, Class rtype)
		{
			this.Rtype_Renamed = rtype;
			this.Ptypes_Renamed = ptypes;
		}

		/*trusted*/	 internal MethodTypeForm Form()
	 {
		 return form;
	 }
		/*trusted*/	 internal Class Rtype()
	 {
		 return Rtype_Renamed;
	 }
		/*trusted*/	 internal Class[] Ptypes()
	 {
		 return Ptypes_Renamed;
	 }

		internal MethodTypeForm Form
		{
			set
			{
				form = value;
			}
		}

		/// <summary>
		/// This number, mandated by the JVM spec as 255,
		///  is the maximum number of <em>slots</em>
		///  that any Java method can receive in its argument list.
		///  It limits both JVM signatures and method type objects.
		///  The longest possible invocation will look like
		///  {@code staticMethod(arg1, arg2, ..., arg255)} or
		///  {@code x.virtualMethod(arg1, arg2, ..., arg254)}.
		/// </summary>
		/*non-public*/	 internal const int MAX_JVM_ARITY = 255; // this is mandated by the JVM spec.

		/// <summary>
		/// This number is the maximum arity of a method handle, 254.
		///  It is derived from the absolute JVM-imposed arity by subtracting one,
		///  which is the slot occupied by the method handle itself at the
		///  beginning of the argument list used to invoke the method handle.
		///  The longest possible invocation will look like
		///  {@code mh.invoke(arg1, arg2, ..., arg254)}.
		/// </summary>
		// Issue:  Should we allow MH.invokeWithArguments to go to the full 255?
		/*non-public*/	 internal static readonly int MAX_MH_ARITY = MAX_JVM_ARITY - 1; // deduct one for mh receiver

		/// <summary>
		/// This number is the maximum arity of a method handle invoker, 253.
		///  It is derived from the absolute JVM-imposed arity by subtracting two,
		///  which are the slots occupied by invoke method handle, and the
		///  target method handle, which are both at the beginning of the argument
		///  list used to invoke the target method handle.
		///  The longest possible invocation will look like
		///  {@code invokermh.invoke(targetmh, arg1, arg2, ..., arg253)}.
		/// </summary>
		/*non-public*/	 internal static readonly int MAX_MH_INVOKER_ARITY = MAX_MH_ARITY - 1; // deduct one more for invoker

		private static void CheckRtype(Class rtype)
		{
			Objects.RequireNonNull(rtype);
		}
		private static void CheckPtype(Class ptype)
		{
			Objects.RequireNonNull(ptype);
			if (ptype == typeof(void))
			{
				throw newIllegalArgumentException("parameter type cannot be void");
			}
		}
		/// <summary>
		/// Return number of extra slots (count of long/double args). </summary>
		private static int CheckPtypes(Class[] ptypes)
		{
			int slots = 0;
			foreach (Class ptype in ptypes)
			{
				CheckPtype(ptype);
				if (ptype == typeof(double) || ptype == typeof(long))
				{
					slots++;
				}
			}
			CheckSlotCount(ptypes.Length + slots);
			return slots;
		}
		internal static void CheckSlotCount(int count)
		{
			assert((MAX_JVM_ARITY & (MAX_JVM_ARITY + 1)) == 0);
			// MAX_JVM_ARITY must be power of 2 minus 1 for following code trick to work:
			if ((count & MAX_JVM_ARITY) != count)
			{
				throw newIllegalArgumentException("bad parameter count " + count);
			}
		}
		private static IndexOutOfBoundsException NewIndexOutOfBoundsException(Object num)
		{
			if (num is Integer)
			{
				num = "bad index: " + num;
			}
			return new IndexOutOfBoundsException(num.ToString());
		}

		internal static readonly ConcurrentWeakInternSet<MethodType> InternTable = new ConcurrentWeakInternSet<MethodType>();

		internal static readonly Class[] NO_PTYPES = new Class[] {};

		/// <summary>
		/// Finds or creates an instance of the given method type. </summary>
		/// <param name="rtype">  the return type </param>
		/// <param name="ptypes"> the parameter types </param>
		/// <returns> a method type with the given components </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypes} is {@code void.class} </exception>
		public static MethodType MethodType(Class rtype, Class[] ptypes)
		{
			return MakeImpl(rtype, ptypes, false);
		}

		/// <summary>
		/// Finds or creates a method type with the given components.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="rtype">  the return type </param>
		/// <param name="ptypes"> the parameter types </param>
		/// <returns> a method type with the given components </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypes} is {@code void.class} </exception>
		public static MethodType MethodType(Class rtype, IList<Class> ptypes)
		{
			bool notrust = false; // random List impl. could return evil ptypes array
			return MakeImpl(rtype, ListToArray(ptypes), notrust);
		}

		private static Class[] ListToArray(IList<Class> ptypes)
		{
			// sanity check the size before the toArray call, since size might be huge
			CheckSlotCount(ptypes.Count);
			return ptypes.toArray(NO_PTYPES);
		}

		/// <summary>
		/// Finds or creates a method type with the given components.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// The leading parameter type is prepended to the remaining array. </summary>
		/// <param name="rtype">  the return type </param>
		/// <param name="ptype0"> the first parameter type </param>
		/// <param name="ptypes"> the remaining parameter types </param>
		/// <returns> a method type with the given components </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} or {@code ptype0} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code ptype0} or {@code ptypes} or any element of {@code ptypes} is {@code void.class} </exception>
		public static MethodType MethodType(Class rtype, Class ptype0, params Class[] ptypes)
		{
			Class[] ptypes1 = new Class[1 + ptypes.Length];
			ptypes1[0] = ptype0;
			System.Array.Copy(ptypes, 0, ptypes1, 1, ptypes.Length);
			return MakeImpl(rtype, ptypes1, true);
		}

		/// <summary>
		/// Finds or creates a method type with the given components.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// The resulting method has no parameter types. </summary>
		/// <param name="rtype">  the return type </param>
		/// <returns> a method type with the given return value </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} is null </exception>
		public static MethodType MethodType(Class rtype)
		{
			return MakeImpl(rtype, NO_PTYPES, true);
		}

		/// <summary>
		/// Finds or creates a method type with the given components.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// The resulting method has the single given parameter type. </summary>
		/// <param name="rtype">  the return type </param>
		/// <param name="ptype0"> the parameter type </param>
		/// <returns> a method type with the given return value and parameter type </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} or {@code ptype0} is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code ptype0} is {@code void.class} </exception>
		public static MethodType MethodType(Class rtype, Class ptype0)
		{
			return MakeImpl(rtype, new Class[]{ptype0}, true);
		}

		/// <summary>
		/// Finds or creates a method type with the given components.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// The resulting method has the same parameter types as {@code ptypes},
		/// and the specified return type. </summary>
		/// <param name="rtype">  the return type </param>
		/// <param name="ptypes"> the method type which supplies the parameter types </param>
		/// <returns> a method type with the given components </returns>
		/// <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} is null </exception>
		public static MethodType MethodType(Class rtype, MethodType ptypes)
		{
			return MakeImpl(rtype, ptypes.Ptypes_Renamed, true);
		}

		/// <summary>
		/// Sole factory method to find or create an interned method type. </summary>
		/// <param name="rtype"> desired return type </param>
		/// <param name="ptypes"> desired parameter types </param>
		/// <param name="trusted"> whether the ptypes can be used without cloning </param>
		/// <returns> the unique method type of the desired structure </returns>
		/*trusted*/	 internal static MethodType MakeImpl(Class rtype, Class[] ptypes, bool trusted)
	 {
			MethodType mt = InternTable.Get(new MethodType(ptypes, rtype));
			if (mt != null)
			{
				return mt;
			}
			if (ptypes.Length == 0)
			{
				ptypes = NO_PTYPES;
				trusted = true;
			}
			mt = new MethodType(rtype, ptypes, trusted);
			// promote the object to the Real Thing, and reprobe
			mt.form = MethodTypeForm.FindForm(mt);
			return InternTable.Add(mt);
	 }
		private static readonly MethodType[] ObjectOnlyTypes = new MethodType[20];

		/// <summary>
		/// Finds or creates a method type whose components are {@code Object} with an optional trailing {@code Object[]} array.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// All parameters and the return type will be {@code Object},
		/// except the final array parameter if any, which will be {@code Object[]}. </summary>
		/// <param name="objectArgCount"> number of parameters (excluding the final array parameter if any) </param>
		/// <param name="finalArray"> whether there will be a trailing array parameter, of type {@code Object[]} </param>
		/// <returns> a generally applicable method type, for all calls of the given fixed argument count and a collected array of further arguments </returns>
		/// <exception cref="IllegalArgumentException"> if {@code objectArgCount} is negative or greater than 255 (or 254, if {@code finalArray} is true) </exception>
		/// <seealso cref= #genericMethodType(int) </seealso>
		public static MethodType GenericMethodType(int objectArgCount, bool finalArray)
		{
			MethodType mt;
			CheckSlotCount(objectArgCount);
			int ivarargs = (!finalArray ? 0 : 1);
			int ootIndex = objectArgCount * 2 + ivarargs;
			if (ootIndex < ObjectOnlyTypes.Length)
			{
				mt = ObjectOnlyTypes[ootIndex];
				if (mt != null)
				{
					return mt;
				}
			}
			Class[] ptypes = new Class[objectArgCount + ivarargs];
			Arrays.Fill(ptypes, typeof(Object));
			if (ivarargs != 0)
			{
				ptypes[objectArgCount] = typeof(Object[]);
			}
			mt = MakeImpl(typeof(Object), ptypes, true);
			if (ootIndex < ObjectOnlyTypes.Length)
			{
				ObjectOnlyTypes[ootIndex] = mt; // cache it here also!
			}
			return mt;
		}

		/// <summary>
		/// Finds or creates a method type whose components are all {@code Object}.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// All parameters and the return type will be Object. </summary>
		/// <param name="objectArgCount"> number of parameters </param>
		/// <returns> a generally applicable method type, for all calls of the given argument count </returns>
		/// <exception cref="IllegalArgumentException"> if {@code objectArgCount} is negative or greater than 255 </exception>
		/// <seealso cref= #genericMethodType(int, boolean) </seealso>
		public static MethodType GenericMethodType(int objectArgCount)
		{
			return GenericMethodType(objectArgCount, false);
		}

		/// <summary>
		/// Finds or creates a method type with a single different parameter type.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="num">    the index (zero-based) of the parameter type to change </param>
		/// <param name="nptype"> a new parameter type to replace the old one with </param>
		/// <returns> the same type, except with the selected parameter changed </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code num} is not a valid index into {@code parameterArray()} </exception>
		/// <exception cref="IllegalArgumentException"> if {@code nptype} is {@code void.class} </exception>
		/// <exception cref="NullPointerException"> if {@code nptype} is null </exception>
		public MethodType ChangeParameterType(int num, Class nptype)
		{
			if (ParameterType(num) == nptype)
			{
				return this;
			}
			CheckPtype(nptype);
			Class[] nptypes = Ptypes_Renamed.clone();
			nptypes[num] = nptype;
			return MakeImpl(Rtype_Renamed, nptypes, true);
		}

		/// <summary>
		/// Finds or creates a method type with additional parameter types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="num">    the position (zero-based) of the inserted parameter type(s) </param>
		/// <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		/// <returns> the same type, except with the selected parameter(s) inserted </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		///                                  or if the resulting method type would have more than 255 parameter slots </exception>
		/// <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		public MethodType InsertParameterTypes(int num, params Class[] ptypesToInsert)
		{
			int len = Ptypes_Renamed.Length;
			if (num < 0 || num > len)
			{
				throw NewIndexOutOfBoundsException(num);
			}
			int ins = CheckPtypes(ptypesToInsert);
			CheckSlotCount(ParameterSlotCount() + ptypesToInsert.Length + ins);
			int ilen = ptypesToInsert.Length;
			if (ilen == 0)
			{
				return this;
			}
			Class[] nptypes = Arrays.CopyOfRange(Ptypes_Renamed, 0, len + ilen);
			System.Array.Copy(nptypes, num, nptypes, num + ilen, len - num);
			System.Array.Copy(ptypesToInsert, 0, nptypes, num, ilen);
			return MakeImpl(Rtype_Renamed, nptypes, true);
		}

		/// <summary>
		/// Finds or creates a method type with additional parameter types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="ptypesToInsert"> zero or more new parameter types to insert after the end of the parameter list </param>
		/// <returns> the same type, except with the selected parameter(s) appended </returns>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		///                                  or if the resulting method type would have more than 255 parameter slots </exception>
		/// <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		public MethodType AppendParameterTypes(params Class[] ptypesToInsert)
		{
			return InsertParameterTypes(ParameterCount(), ptypesToInsert);
		}

		/// <summary>
		/// Finds or creates a method type with additional parameter types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="num">    the position (zero-based) of the inserted parameter type(s) </param>
		/// <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		/// <returns> the same type, except with the selected parameter(s) inserted </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		///                                  or if the resulting method type would have more than 255 parameter slots </exception>
		/// <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		public MethodType InsertParameterTypes(int num, IList<Class> ptypesToInsert)
		{
			return InsertParameterTypes(num, ListToArray(ptypesToInsert));
		}

		/// <summary>
		/// Finds or creates a method type with additional parameter types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="ptypesToInsert"> zero or more new parameter types to insert after the end of the parameter list </param>
		/// <returns> the same type, except with the selected parameter(s) appended </returns>
		/// <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		///                                  or if the resulting method type would have more than 255 parameter slots </exception>
		/// <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		public MethodType AppendParameterTypes(IList<Class> ptypesToInsert)
		{
			return InsertParameterTypes(ParameterCount(), ptypesToInsert);
		}

		 /// <summary>
		 /// Finds or creates a method type with modified parameter types.
		 /// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		 /// <param name="start">  the position (zero-based) of the first replaced parameter type(s) </param>
		 /// <param name="end">    the position (zero-based) after the last replaced parameter type(s) </param>
		 /// <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		 /// <returns> the same type, except with the selected parameter(s) replaced </returns>
		 /// <exception cref="IndexOutOfBoundsException"> if {@code start} is negative or greater than {@code parameterCount()}
		 ///                                  or if {@code end} is negative or greater than {@code parameterCount()}
		 ///                                  or if {@code start} is greater than {@code end} </exception>
		 /// <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		 ///                                  or if the resulting method type would have more than 255 parameter slots </exception>
		 /// <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		/*non-public*/	 internal MethodType ReplaceParameterTypes(int start, int end, params Class[] ptypesToInsert)
	 {
			if (start == end)
			{
				return InsertParameterTypes(start, ptypesToInsert);
			}
			int len = Ptypes_Renamed.Length;
			if (!(0 <= start && start <= end && end <= len))
			{
				throw NewIndexOutOfBoundsException("start=" + start + " end=" + end);
			}
			int ilen = ptypesToInsert.Length;
			if (ilen == 0)
			{
				return DropParameterTypes(start, end);
			}
			return DropParameterTypes(start, end).InsertParameterTypes(start, ptypesToInsert);
	 }

		/// <summary>
		/// Replace the last arrayLength parameter types with the component type of arrayType. </summary>
		/// <param name="arrayType"> any array type </param>
		/// <param name="arrayLength"> the number of parameter types to change </param>
		/// <returns> the resulting type </returns>
		/*non-public*/	 internal MethodType AsSpreaderType(Class arrayType, int arrayLength)
	 {
			assert(ParameterCount() >= arrayLength);
			int spreadPos = Ptypes_Renamed.Length - arrayLength;
			if (arrayLength == 0) // nothing to change
			{
				return this;
			}
			if (arrayType == typeof(Object[]))
			{
				if (Generic) // nothing to change
				{
					return this;
				}
				if (spreadPos == 0)
				{
					// no leading arguments to preserve; go generic
					MethodType res = GenericMethodType(arrayLength);
					if (Rtype_Renamed != typeof(Object))
					{
						res = res.ChangeReturnType(Rtype_Renamed);
					}
					return res;
				}
			}
			Class elemType = arrayType.ComponentType;
			assert(elemType != null);
			for (int i = spreadPos; i < Ptypes_Renamed.Length; i++)
			{
				if (Ptypes_Renamed[i] != elemType)
				{
					Class[] fixedPtypes = Ptypes_Renamed.clone();
					Arrays.Fill(fixedPtypes, i, Ptypes_Renamed.Length, elemType);
					return MethodType(Rtype_Renamed, fixedPtypes);
				}
			}
			return this; // arguments check out; no change
	 }

		/// <summary>
		/// Return the leading parameter type, which must exist and be a reference. </summary>
		///  <returns> the leading parameter type, after error checks </returns>
		/*non-public*/	 internal Class LeadingReferenceParameter()
	 {
			Class ptype;
			if (Ptypes_Renamed.Length == 0 || (ptype = Ptypes_Renamed[0]).Primitive)
			{
				throw newIllegalArgumentException("no leading reference parameter");
			}
			return ptype;
	 }

		/// <summary>
		/// Delete the last parameter type and replace it with arrayLength copies of the component type of arrayType. </summary>
		/// <param name="arrayType"> any array type </param>
		/// <param name="arrayLength"> the number of parameter types to insert </param>
		/// <returns> the resulting type </returns>
		/*non-public*/	 internal MethodType AsCollectorType(Class arrayType, int arrayLength)
	 {
			assert(ParameterCount() >= 1);
			assert(arrayType.IsSubclassOf(LastParameterType()));
			MethodType res;
			if (arrayType == typeof(Object[]))
			{
				res = GenericMethodType(arrayLength);
				if (Rtype_Renamed != typeof(Object))
				{
					res = res.ChangeReturnType(Rtype_Renamed);
				}
			}
			else
			{
				Class elemType = arrayType.ComponentType;
				assert(elemType != null);
				res = MethodType(Rtype_Renamed, Collections.NCopies(arrayLength, elemType));
			}
			if (Ptypes_Renamed.Length == 1)
			{
				return res;
			}
			else
			{
				return res.InsertParameterTypes(0, ParameterList().subList(0, Ptypes_Renamed.Length - 1));
			}
	 }

		/// <summary>
		/// Finds or creates a method type with some parameter types omitted.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="start">  the index (zero-based) of the first parameter type to remove </param>
		/// <param name="end">    the index (greater than {@code start}) of the first parameter type after not to remove </param>
		/// <returns> the same type, except with the selected parameter(s) removed </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code start} is negative or greater than {@code parameterCount()}
		///                                  or if {@code end} is negative or greater than {@code parameterCount()}
		///                                  or if {@code start} is greater than {@code end} </exception>
		public MethodType DropParameterTypes(int start, int end)
		{
			int len = Ptypes_Renamed.Length;
			if (!(0 <= start && start <= end && end <= len))
			{
				throw NewIndexOutOfBoundsException("start=" + start + " end=" + end);
			}
			if (start == end)
			{
				return this;
			}
			Class[] nptypes;
			if (start == 0)
			{
				if (end == len)
				{
					// drop all parameters
					nptypes = NO_PTYPES;
				}
				else
				{
					// drop initial parameter(s)
					nptypes = Arrays.CopyOfRange(Ptypes_Renamed, end, len);
				}
			}
			else
			{
				if (end == len)
				{
					// drop trailing parameter(s)
					nptypes = Arrays.CopyOfRange(Ptypes_Renamed, 0, start);
				}
				else
				{
					int tail = len - end;
					nptypes = Arrays.CopyOfRange(Ptypes_Renamed, 0, start + tail);
					System.Array.Copy(Ptypes_Renamed, end, nptypes, start, tail);
				}
			}
			return MakeImpl(Rtype_Renamed, nptypes, true);
		}

		/// <summary>
		/// Finds or creates a method type with a different return type.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		/// <param name="nrtype"> a return parameter type to replace the old one with </param>
		/// <returns> the same type, except with the return type change </returns>
		/// <exception cref="NullPointerException"> if {@code nrtype} is null </exception>
		public MethodType ChangeReturnType(Class nrtype)
		{
			if (ReturnType() == nrtype)
			{
				return this;
			}
			return MakeImpl(nrtype, Ptypes_Renamed, true);
		}

		/// <summary>
		/// Reports if this type contains a primitive argument or return value.
		/// The return type {@code void} counts as a primitive. </summary>
		/// <returns> true if any of the types are primitives </returns>
		public bool HasPrimitives()
		{
			return form.hasPrimitives();
		}

		/// <summary>
		/// Reports if this type contains a wrapper argument or return value.
		/// Wrappers are types which box primitive values, such as <seealso cref="Integer"/>.
		/// The reference type {@code java.lang.Void} counts as a wrapper,
		/// if it occurs as a return type. </summary>
		/// <returns> true if any of the types are wrappers </returns>
		public bool HasWrappers()
		{
			return Unwrap() != this;
		}

		/// <summary>
		/// Erases all reference types to {@code Object}.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// All primitive types (including {@code void}) will remain unchanged. </summary>
		/// <returns> a version of the original type with all reference types replaced </returns>
		public MethodType Erase()
		{
			return form.erasedType();
		}

		/// <summary>
		/// Erases all reference types to {@code Object}, and all subword types to {@code int}.
		/// This is the reduced type polymorphism used by private methods
		/// such as <seealso cref="MethodHandle#invokeBasic invokeBasic"/>. </summary>
		/// <returns> a version of the original type with all reference and subword types replaced </returns>
		/*non-public*/	 internal MethodType BasicType()
	 {
			return form.basicType();
	 }

		/// <returns> a version of the original type with MethodHandle prepended as the first argument </returns>
		/*non-public*/	 internal MethodType InvokerType()
	 {
			return InsertParameterTypes(0, typeof(MethodHandle));
	 }

		/// <summary>
		/// Converts all types, both reference and primitive, to {@code Object}.
		/// Convenience method for <seealso cref="#genericMethodType(int) genericMethodType"/>.
		/// The expression {@code type.wrap().erase()} produces the same value
		/// as {@code type.generic()}. </summary>
		/// <returns> a version of the original type with all types replaced </returns>
		public MethodType Generic()
		{
			return GenericMethodType(ParameterCount());
		}

		/*non-public*/
	 internal bool Generic
	 {
		 get
		 {
				return this == Erase() && !HasPrimitives();
		 }
	 }

		/// <summary>
		/// Converts all primitive types to their corresponding wrapper types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// All reference types (including wrapper types) will remain unchanged.
		/// A {@code void} return type is changed to the type {@code java.lang.Void}.
		/// The expression {@code type.wrap().erase()} produces the same value
		/// as {@code type.generic()}. </summary>
		/// <returns> a version of the original type with all primitive types replaced </returns>
		public MethodType Wrap()
		{
			return HasPrimitives() ? WrapWithPrims(this) : this;
		}

		/// <summary>
		/// Converts all wrapper types to their corresponding primitive types.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// All primitive types (including {@code void}) will remain unchanged.
		/// A return type of {@code java.lang.Void} is changed to {@code void}. </summary>
		/// <returns> a version of the original type with all wrapper types replaced </returns>
		public MethodType Unwrap()
		{
			MethodType noprims = !HasPrimitives() ? this : WrapWithPrims(this);
			return UnwrapWithNoPrims(noprims);
		}

		private static MethodType WrapWithPrims(MethodType pt)
		{
			assert(pt.HasPrimitives());
			MethodType wt = pt.wrapAlt;
			if (wt == null)
			{
				// fill in lazily
				wt = MethodTypeForm.Canonicalize(pt, MethodTypeForm.WRAP, MethodTypeForm.WRAP);
				assert(wt != null);
				pt.wrapAlt = wt;
			}
			return wt;
		}

		private static MethodType UnwrapWithNoPrims(MethodType wt)
		{
			assert(!wt.HasPrimitives());
			MethodType uwt = wt.wrapAlt;
			if (uwt == null)
			{
				// fill in lazily
				uwt = MethodTypeForm.Canonicalize(wt, MethodTypeForm.UNWRAP, MethodTypeForm.UNWRAP);
				if (uwt == null)
				{
					uwt = wt; // type has no wrappers or prims at all
				}
				wt.wrapAlt = uwt;
			}
			return uwt;
		}

		/// <summary>
		/// Returns the parameter type at the specified index, within this method type. </summary>
		/// <param name="num"> the index (zero-based) of the desired parameter type </param>
		/// <returns> the selected parameter type </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code num} is not a valid index into {@code parameterArray()} </exception>
		public Class ParameterType(int num)
		{
			return Ptypes_Renamed[num];
		}
		/// <summary>
		/// Returns the number of parameter types in this method type. </summary>
		/// <returns> the number of parameter types </returns>
		public int ParameterCount()
		{
			return Ptypes_Renamed.Length;
		}
		/// <summary>
		/// Returns the return type of this method type. </summary>
		/// <returns> the return type </returns>
		public Class ReturnType()
		{
			return Rtype_Renamed;
		}

		/// <summary>
		/// Presents the parameter types as a list (a convenience method).
		/// The list will be immutable. </summary>
		/// <returns> the parameter types (as an immutable list) </returns>
		public IList<Class> ParameterList()
		{
			return Collections.UnmodifiableList(Arrays.AsList(Ptypes_Renamed.clone()));
		}

		/*non-public*/	 internal Class LastParameterType()
	 {
			int len = Ptypes_Renamed.Length;
			return len == 0 ? typeof(void) : Ptypes_Renamed[len - 1];
	 }

		/// <summary>
		/// Presents the parameter types as an array (a convenience method).
		/// Changes to the array will not result in changes to the type. </summary>
		/// <returns> the parameter types (as a fresh copy if necessary) </returns>
		public Class[] ParameterArray()
		{
			return Ptypes_Renamed.clone();
		}

		/// <summary>
		/// Compares the specified object with this type for equality.
		/// That is, it returns <tt>true</tt> if and only if the specified object
		/// is also a method type with exactly the same parameters and return type. </summary>
		/// <param name="x"> object to compare </param>
		/// <seealso cref= Object#equals(Object) </seealso>
		public override bool Equals(Object x)
		{
			return this == x || x is MethodType && Equals((MethodType)x);
		}

		private bool Equals(MethodType that)
		{
			return this.Rtype_Renamed == that.Rtype_Renamed && Arrays.Equals(this.Ptypes_Renamed, that.Ptypes_Renamed);
		}

		/// <summary>
		/// Returns the hash code value for this method type.
		/// It is defined to be the same as the hashcode of a List
		/// whose elements are the return type followed by the
		/// parameter types. </summary>
		/// <returns> the hash code value for this method type </returns>
		/// <seealso cref= Object#hashCode() </seealso>
		/// <seealso cref= #equals(Object) </seealso>
		/// <seealso cref= List#hashCode() </seealso>
		public override int HashCode()
		{
		  int hashCode = 31 + Rtype_Renamed.HashCode();
		  foreach (Class ptype in Ptypes_Renamed)
		  {
			  hashCode = 31 * hashCode + ptype.HashCode();
		  }
		  return hashCode;
		}

		/// <summary>
		/// Returns a string representation of the method type,
		/// of the form {@code "(PT0,PT1...)RT"}.
		/// The string representation of a method type is a
		/// parenthesis enclosed, comma separated list of type names,
		/// followed immediately by the return type.
		/// <para>
		/// Each type is represented by its
		/// <seealso cref="java.lang.Class#getSimpleName simple name"/>.
		/// </para>
		/// </summary>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("(");
			for (int i = 0; i < Ptypes_Renamed.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(",");
				}
				sb.Append(Ptypes_Renamed[i].SimpleName);
			}
			sb.Append(")");
			sb.Append(Rtype_Renamed.SimpleName);
			return sb.ToString();
		}

		/// <summary>
		/// True if the old return type can always be viewed (w/o casting) under new return type,
		///  and the new parameters can be viewed (w/o casting) under the old parameter types.
		/// </summary>
		/*non-public*/
		internal bool IsViewableAs(MethodType newType, bool keepInterfaces)
		{
			if (!VerifyType.isNullConversion(ReturnType(), newType.ReturnType(), keepInterfaces))
			{
				return false;
			}
			return ParametersAreViewableAs(newType, keepInterfaces);
		}
		/// <summary>
		/// True if the new parameters can be viewed (w/o casting) under the old parameter types. </summary>
		/*non-public*/
		internal bool ParametersAreViewableAs(MethodType newType, bool keepInterfaces)
		{
			if (form == newType.form && form.erasedType == this)
			{
				return true; // my reference parameters are all Object
			}
			if (Ptypes_Renamed == newType.Ptypes_Renamed)
			{
				return true;
			}
			int argc = ParameterCount();
			if (argc != newType.ParameterCount())
			{
				return false;
			}
			for (int i = 0; i < argc; i++)
			{
				if (!VerifyType.isNullConversion(newType.ParameterType(i), ParameterType(i), keepInterfaces))
				{
					return false;
				}
			}
			return true;
		}
		/*non-public*/
		internal bool IsConvertibleTo(MethodType newType)
		{
			MethodTypeForm oldForm = this.Form();
			MethodTypeForm newForm = newType.Form();
			if (oldForm == newForm)
			{
				// same parameter count, same primitive/object mix
				return true;
			}
			if (!CanConvert(ReturnType(), newType.ReturnType()))
			{
				return false;
			}
			Class[] srcTypes = newType.Ptypes_Renamed;
			Class[] dstTypes = Ptypes_Renamed;
			if (srcTypes == dstTypes)
			{
				return true;
			}
			int argc;
			if ((argc = srcTypes.Length) != dstTypes.Length)
			{
				return false;
			}
			if (argc <= 1)
			{
				if (argc == 1 && !CanConvert(srcTypes[0], dstTypes[0]))
				{
					return false;
				}
				return true;
			}
			if ((oldForm.PrimitiveParameterCount() == 0 && oldForm.ErasedType_Renamed == this) || (newForm.PrimitiveParameterCount() == 0 && newForm.ErasedType_Renamed == newType))
			{
				// Somewhat complicated test to avoid a loop of 2 or more trips.
				// If either type has only Object parameters, we know we can convert.
				assert(CanConvertParameters(srcTypes, dstTypes));
				return true;
			}
			return CanConvertParameters(srcTypes, dstTypes);
		}

		/// <summary>
		/// Returns true if MHs.explicitCastArguments produces the same result as MH.asType.
		///  If the type conversion is impossible for either, the result should be false.
		/// </summary>
		/*non-public*/
		internal bool ExplicitCastEquivalentToAsType(MethodType newType)
		{
			if (this == newType)
			{
				return true;
			}
			if (!ExplicitCastEquivalentToAsType(Rtype_Renamed, newType.Rtype_Renamed))
			{
				return false;
			}
			Class[] srcTypes = newType.Ptypes_Renamed;
			Class[] dstTypes = Ptypes_Renamed;
			if (dstTypes == srcTypes)
			{
				return true;
			}
			assert(dstTypes.Length == srcTypes.Length);
			for (int i = 0; i < dstTypes.Length; i++)
			{
				if (!ExplicitCastEquivalentToAsType(srcTypes[i], dstTypes[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Reports true if the src can be converted to the dst, by both asType and MHs.eCE,
		///  and with the same effect.
		///  MHs.eCA has the following "upgrades" to MH.asType:
		///  1. interfaces are unchecked (that is, treated as if aliased to Object)
		///     Therefore, {@code Object->CharSequence} is possible in both cases but has different semantics
		///  2. the full matrix of primitive-to-primitive conversions is supported
		///     Narrowing like {@code long->byte} and basic-typing like {@code boolean->int}
		///     are not supported by asType, but anything supported by asType is equivalent
		///     with MHs.eCE.
		///  3a. unboxing conversions can be followed by the full matrix of primitive conversions
		///  3b. unboxing of null is permitted (creates a zero primitive value)
		/// Other than interfaces, reference-to-reference conversions are the same.
		/// Boxing primitives to references is the same for both operators.
		/// </summary>
		private static bool ExplicitCastEquivalentToAsType(Class src, Class dst)
		{
			if (src == dst || dst == typeof(Object) || dst == typeof(void))
			{
				return true;
			}
			if (src.Primitive)
			{
				// Could be a prim/prim conversion, where casting is a strict superset.
				// Or a boxing conversion, which is always to an exact wrapper class.
				return CanConvert(src, dst);
			}
			else if (dst.Primitive)
			{
				// Unboxing behavior is different between MHs.eCA & MH.asType (see 3b).
				return false;
			}
			else
			{
				// R->R always works, but we have to avoid a check-cast to an interface.
				return !dst.Interface || src.IsSubclassOf(dst);
			}
		}

		private bool CanConvertParameters(Class[] srcTypes, Class[] dstTypes)
		{
			for (int i = 0; i < srcTypes.Length; i++)
			{
				if (!CanConvert(srcTypes[i], dstTypes[i]))
				{
					return false;
				}
			}
			return true;
		}

		/*non-public*/
		internal static bool CanConvert(Class src, Class dst)
		{
			// short-circuit a few cases:
			if (src == dst || src == typeof(Object) || dst == typeof(Object))
			{
				return true;
			}
			// the remainder of this logic is documented in MethodHandle.asType
			if (src.Primitive)
			{
				// can force void to an explicit null, a la reflect.Method.invoke
				// can also force void to a primitive zero, by analogy
				if (src == typeof(void)) //or !dst.isPrimitive()?
				{
					return true;
				}
				Wrapper sw = Wrapper.forPrimitiveType(src);
				if (dst.Primitive)
				{
					// P->P must widen
					return Wrapper.forPrimitiveType(dst).isConvertibleFrom(sw);
				}
				else
				{
					// P->R must box and widen
					return sw.wrapperType().IsSubclassOf(dst);
				}
			}
			else if (dst.Primitive)
			{
				// any value can be dropped
				if (dst == typeof(void))
				{
					return true;
				}
				Wrapper dw = Wrapper.forPrimitiveType(dst);
				// R->P must be able to unbox (from a dynamically chosen type) and widen
				// For example:
				//   Byte/Number/Comparable/Object -> dw:Byte -> byte.
				//   Character/Comparable/Object -> dw:Character -> char
				//   Boolean/Comparable/Object -> dw:Boolean -> boolean
				// This means that dw must be cast-compatible with src.
				if (dw.wrapperType().IsSubclassOf(src))
				{
					return true;
				}
				// The above does not work if the source reference is strongly typed
				// to a wrapper whose primitive must be widened.  For example:
				//   Byte -> unbox:byte -> short/int/long/float/double
				//   Character -> unbox:char -> int/long/float/double
				if (Wrapper.isWrapperType(src) && dw.isConvertibleFrom(Wrapper.forWrapperType(src)))
				{
					// can unbox from src and then widen to dst
					return true;
				}
				// We have already covered cases which arise due to runtime unboxing
				// of a reference type which covers several wrapper types:
				//   Object -> cast:Integer -> unbox:int -> long/float/double
				//   Serializable -> cast:Byte -> unbox:byte -> byte/short/int/long/float/double
				// An marginal case is Number -> dw:Character -> char, which would be OK if there were a
				// subclass of Number which wraps a value that can convert to char.
				// Since there is none, we don't need an extra check here to cover char or boolean.
				return false;
			}
			else
			{
				// R->R always works, since null is always valid dynamically
				return true;
			}
		}

		/// Queries which have to do with the bytecode architecture

		/// <summary>
		/// Reports the number of JVM stack slots required to invoke a method
		/// of this type.  Note that (for historical reasons) the JVM requires
		/// a second stack slot to pass long and double arguments.
		/// So this method returns <seealso cref="#parameterCount() parameterCount"/> plus the
		/// number of long and double parameters (if any).
		/// <para>
		/// This method is included for the benefit of applications that must
		/// generate bytecodes that process method handles and invokedynamic.
		/// </para>
		/// </summary>
		/// <returns> the number of JVM stack slots for this type's parameters </returns>
		/*non-public*/	 internal int ParameterSlotCount()
	 {
			return form.parameterSlotCount();
	 }

		/*non-public*/	 internal Invokers Invokers()
	 {
			Invokers inv = invokers;
			if (inv != null)
			{
				return inv;
			}
			invokers = inv = new Invokers(this);
			return inv;
	 }

		/// <summary>
		/// Reports the number of JVM stack slots which carry all parameters including and after
		/// the given position, which must be in the range of 0 to
		/// {@code parameterCount} inclusive.  Successive parameters are
		/// more shallowly stacked, and parameters are indexed in the bytecodes
		/// according to their trailing edge.  Thus, to obtain the depth
		/// in the outgoing call stack of parameter {@code N}, obtain
		/// the {@code parameterSlotDepth} of its trailing edge
		/// at position {@code N+1}.
		/// <para>
		/// Parameters of type {@code long} and {@code double} occupy
		/// two stack slots (for historical reasons) and all others occupy one.
		/// Therefore, the number returned is the number of arguments
		/// <em>including</em> and <em>after</em> the given parameter,
		/// <em>plus</em> the number of long or double arguments
		/// at or after after the argument for the given parameter.
		/// </para>
		/// <para>
		/// This method is included for the benefit of applications that must
		/// generate bytecodes that process method handles and invokedynamic.
		/// </para>
		/// </summary>
		/// <param name="num"> an index (zero-based, inclusive) within the parameter types </param>
		/// <returns> the index of the (shallowest) JVM stack slot transmitting the
		///         given parameter </returns>
		/// <exception cref="IllegalArgumentException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		/*non-public*/	 internal int ParameterSlotDepth(int num)
	 {
			if (num < 0 || num > Ptypes_Renamed.Length)
			{
				ParameterType(num); // force a range check
			}
			return form.parameterToArgSlot(num - 1);
	 }

		/// <summary>
		/// Reports the number of JVM stack slots required to receive a return value
		/// from a method of this type.
		/// If the <seealso cref="#returnType() return type"/> is void, it will be zero,
		/// else if the return type is long or double, it will be two, else one.
		/// <para>
		/// This method is included for the benefit of applications that must
		/// generate bytecodes that process method handles and invokedynamic.
		/// </para>
		/// </summary>
		/// <returns> the number of JVM stack slots (0, 1, or 2) for this type's return value
		/// Will be removed for PFD. </returns>
		/*non-public*/	 internal int ReturnSlotCount()
	 {
			return form.returnSlotCount();
	 }

		/// <summary>
		/// Finds or creates an instance of a method type, given the spelling of its bytecode descriptor.
		/// Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		/// Any class or interface name embedded in the descriptor string
		/// will be resolved by calling <seealso cref="ClassLoader#loadClass(java.lang.String)"/>
		/// on the given loader (or if it is null, on the system class loader).
		/// <para>
		/// Note that it is possible to encounter method types which cannot be
		/// constructed by this method, because their component types are
		/// not all reachable from a common class loader.
		/// </para>
		/// <para>
		/// This method is included for the benefit of applications that must
		/// generate bytecodes that process method handles and {@code invokedynamic}.
		/// </para>
		/// </summary>
		/// <param name="descriptor"> a bytecode-level type descriptor string "(T...)T" </param>
		/// <param name="loader"> the class loader in which to look up the types </param>
		/// <returns> a method type matching the bytecode-level type descriptor </returns>
		/// <exception cref="NullPointerException"> if the string is null </exception>
		/// <exception cref="IllegalArgumentException"> if the string is not well-formed </exception>
		/// <exception cref="TypeNotPresentException"> if a named type cannot be found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MethodType fromMethodDescriptorString(String descriptor, ClassLoader loader) throws IllegalArgumentException, TypeNotPresentException
		public static MethodType FromMethodDescriptorString(String descriptor, ClassLoader loader)
		{
			if (!descriptor.StartsWith("(") || descriptor.IndexOf(')') < 0 || descriptor.IndexOf('.') >= 0) // also generates NPE if needed
			{
				throw newIllegalArgumentException("not a method descriptor: " + descriptor);
			}
			IList<Class> types = BytecodeDescriptor.parseMethod(descriptor, loader);
			Class rtype = types.Remove(types.Count - 1);
			CheckSlotCount(types.Count);
			Class[] ptypes = ListToArray(types);
			return MakeImpl(rtype, ptypes, true);
		}

		/// <summary>
		/// Produces a bytecode descriptor representation of the method type.
		/// <para>
		/// Note that this is not a strict inverse of <seealso cref="#fromMethodDescriptorString fromMethodDescriptorString"/>.
		/// Two distinct classes which share a common name but have different class loaders
		/// will appear identical when viewed within descriptor strings.
		/// </para>
		/// <para>
		/// This method is included for the benefit of applications that must
		/// generate bytecodes that process method handles and {@code invokedynamic}.
		/// <seealso cref="#fromMethodDescriptorString(java.lang.String, java.lang.ClassLoader) fromMethodDescriptorString"/>,
		/// because the latter requires a suitable class loader argument.
		/// </para>
		/// </summary>
		/// <returns> the bytecode type descriptor representation </returns>
		public String ToMethodDescriptorString()
		{
			String desc = methodDescriptor;
			if (desc == null)
			{
				desc = BytecodeDescriptor.unparse(this);
				methodDescriptor = desc;
			}
			return desc;
		}

		/*non-public*/	 internal static String ToFieldDescriptorString(Class cls)
	 {
			return BytecodeDescriptor.unparse(cls);
	 }

		/// Serialization.

		/// <summary>
		/// There are no serializable fields for {@code MethodType}.
		/// </summary>
		private static readonly java.io.ObjectStreamField[] SerialPersistentFields = new java.io.ObjectStreamField[] { };

		/// <summary>
		/// Save the {@code MethodType} instance to a stream.
		/// 
		/// @serialData
		/// For portability, the serialized format does not refer to named fields.
		/// Instead, the return type and parameter type arrays are written directly
		/// from the {@code writeObject} method, using two calls to {@code s.writeObject}
		/// as follows:
		/// <blockquote><pre>{@code
		/// s.writeObject(this.returnType());
		/// s.writeObject(this.parameterArray());
		/// }</pre></blockquote>
		/// <para>
		/// The deserialized field values are checked as if they were
		/// provided to the factory method <seealso cref="#methodType(Class,Class[]) methodType"/>.
		/// For example, null values, or {@code void} parameter types,
		/// will lead to exceptions during deserialization.
		/// </para>
		/// </summary>
		/// <param name="s"> the stream to write the object to </param>
		/// <exception cref="java.io.IOException"> if there is a problem writing the object </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject(); // requires serialPersistentFields to be an empty array
			s.WriteObject(ReturnType());
			s.WriteObject(ParameterArray());
		}

		/// <summary>
		/// Reconstitute the {@code MethodType} instance from a stream (that is,
		/// deserialize it).
		/// This instance is a scratch object with bogus final fields.
		/// It provides the parameters to the factory method called by
		/// <seealso cref="#readResolve readResolve"/>.
		/// After that call it is discarded. </summary>
		/// <param name="s"> the stream to read the object from </param>
		/// <exception cref="java.io.IOException"> if there is a problem reading the object </exception>
		/// <exception cref="ClassNotFoundException"> if one of the component classes cannot be resolved </exception>
		/// <seealso cref= #MethodType() </seealso>
		/// <seealso cref= #readResolve </seealso>
		/// <seealso cref= #writeObject </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject(); // requires serialPersistentFields to be an empty array

			Class returnType = (Class) s.ReadObject();
			Class[] parameterArray = (Class[]) s.ReadObject();

			// Probably this object will never escape, but let's check
			// the field values now, just to be sure.
			CheckRtype(returnType);
			CheckPtypes(parameterArray);

			parameterArray = parameterArray.clone(); // make sure it is unshared
			MethodType_init(returnType, parameterArray);
		}

		/// <summary>
		/// For serialization only.
		/// Sets the final fields to null, pending {@code Unsafe.putObject}.
		/// </summary>
		private MethodType()
		{
			this.Rtype_Renamed = null;
			this.Ptypes_Renamed = null;
		}
		private void MethodType_init(Class rtype, Class[] ptypes)
		{
			// In order to communicate these values to readResolve, we must
			// store them into the implementation-specific final fields.
			CheckRtype(rtype);
			CheckPtypes(ptypes);
			UNSAFE.putObject(this, RtypeOffset, rtype);
			UNSAFE.putObject(this, PtypesOffset, ptypes);
		}

		// Support for resetting final fields while deserializing
		private static readonly long RtypeOffset, PtypesOffset;
		static MethodType()
		{
			try
			{
				RtypeOffset = UNSAFE.objectFieldOffset(typeof(MethodType).getDeclaredField("rtype"));
				PtypesOffset = UNSAFE.objectFieldOffset(typeof(MethodType).getDeclaredField("ptypes"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		/// <summary>
		/// Resolves and initializes a {@code MethodType} object
		/// after serialization. </summary>
		/// <returns> the fully initialized {@code MethodType} object </returns>
		private Object ReadResolve()
		{
			// Do not use a trusted path for deserialization:
			//return makeImpl(rtype, ptypes, true);
			// Verify all operands, and make sure ptypes is unshared:
			return MethodType(Rtype_Renamed, Ptypes_Renamed);
		}

		/// <summary>
		/// Simple implementation of weak concurrent intern set.
		/// </summary>
		/// @param <T> interned type </param>
		private class ConcurrentWeakInternSet<T>
		{

			internal readonly ConcurrentMap<WeakEntry<T>, WeakEntry<T>> Map;
			internal readonly ReferenceQueue<T> Stale;

			public ConcurrentWeakInternSet()
			{
				this.Map = new ConcurrentDictionary<>();
				this.Stale = new ReferenceQueue<>();
			}

			/// <summary>
			/// Get the existing interned element.
			/// This method returns null if no element is interned.
			/// </summary>
			/// <param name="elem"> element to look up </param>
			/// <returns> the interned element </returns>
			public virtual T Get(T elem)
			{
				if (elem == null)
				{
					throw new NullPointerException();
				}
				ExpungeStaleElements();

				WeakEntry<T> value = Map[new WeakEntry<T>(elem)];
				if (value != null)
				{
					T res = value.get();
					if (res != null)
					{
						return res;
					}
				}
				return null;
			}

			/// <summary>
			/// Interns the element.
			/// Always returns non-null element, matching the one in the intern set.
			/// Under the race against another add(), it can return <i>different</i>
			/// element, if another thread beats us to interning it.
			/// </summary>
			/// <param name="elem"> element to add </param>
			/// <returns> element that was actually added </returns>
			public virtual T Add(T elem)
			{
				if (elem == null)
				{
					throw new NullPointerException();
				}

				// Playing double race here, and so spinloop is required.
				// First race is with two concurrent updaters.
				// Second race is with GC purging weak ref under our feet.
				// Hopefully, we almost always end up with a single pass.
				T interned;
				WeakEntry<T> e = new WeakEntry<T>(elem, Stale);
				do
				{
					ExpungeStaleElements();
					WeakEntry<T> exist = Map.PutIfAbsent(e, e);
					interned = (exist == null) ? elem : exist.get();
				} while (interned == null);
				return interned;
			}

			internal virtual void ExpungeStaleElements()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<? extends T> reference;
				Reference<?> reference;
				while ((reference = Stale.poll()) != null)
				{
					Map.Remove(reference);
				}
			}

			private class WeakEntry<T> : WeakReference<T>
			{

				public readonly int Hashcode;

				public WeakEntry(T key, ReferenceQueue<T> queue) : base(key, queue)
				{
					Hashcode = key.HashCode();
				}

				public WeakEntry(T key) : base(key)
				{
					Hashcode = key.HashCode();
				}

				public override bool Equals(Object obj)
				{
					if (obj is WeakEntry)
					{
						Object that = ((WeakEntry) obj).get();
						Object mine = get();
						return (that == null || mine == null) ? (this == obj) : mine.Equals(that);
					}
					return false;
				}

				public override int HashCode()
				{
					return Hashcode;
				}

			}
		}

	}

}