using System;
using System.Collections.Concurrent;

/*
 * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// Transforms on LFs.
	///  A lambda-form editor can derive new LFs from its base LF.
	///  The editor can cache derived LFs, which simplifies the reuse of their underlying bytecodes.
	///  To support this caching, a LF has an optional pointer to its editor.
	/// </summary>
	internal class LambdaFormEditor
	{
		internal readonly LambdaForm LambdaForm;

		private LambdaFormEditor(LambdaForm lambdaForm)
		{
			this.LambdaForm = lambdaForm;
		}

		// Factory method.
		internal static LambdaFormEditor LambdaFormEditor(LambdaForm lambdaForm)
		{
			// TO DO:  Consider placing intern logic here, to cut down on duplication.
			// lambdaForm = findPreexistingEquivalent(lambdaForm)

			// Always use uncustomized version for editing.
			// It helps caching and customized LambdaForms reuse transformCache field to keep a link to uncustomized version.
			return new LambdaFormEditor(lambdaForm.Uncustomize());
		}

		/// <summary>
		/// A description of a cached transform, possibly associated with the result of the transform.
		///  The logical content is a sequence of byte values, starting with a Kind.ordinal value.
		///  The sequence is unterminated, ending with an indefinite number of zero bytes.
		///  Sequences that are simple (short enough and with small enough values) pack into a 64-bit long.
		/// </summary>
		private sealed class Transform : SoftReference<LambdaForm>
		{
			internal readonly long PackedBytes_Renamed;
			internal readonly sbyte[] FullBytes_Renamed;

			private enum Kind
			{
				NO_KIND, // necessary because ordinal must be greater than zero
				BIND_ARG,
				ADD_ARG,
				DUP_ARG,
				SPREAD_ARGS,
				FILTER_ARG,
				FILTER_RETURN,
				FILTER_RETURN_TO_ZERO,
				COLLECT_ARGS,
				COLLECT_ARGS_TO_VOID,
				COLLECT_ARGS_TO_ARRAY,
				FOLD_ARGS,
				FOLD_ARGS_TO_VOID,
				PERMUTE_ARGS
				//maybe add more for guard with test, catch exception, pointwise type conversions
			}

			internal const bool STRESS_TEST = false; // turn on to disable most packing
			internal static readonly int PACKED_BYTE_SIZE = (STRESS_TEST ? 2 : 4), PACKED_BYTE_MASK = (1 << PACKED_BYTE_SIZE) - 1, PACKED_BYTE_MAX_LENGTH = (STRESS_TEST ? 3 : 64 / PACKED_BYTE_SIZE);

			internal static long PackedBytes(sbyte[] bytes)
			{
				if (bytes.Length > PACKED_BYTE_MAX_LENGTH)
				{
					return 0;
				}
				long pb = 0;
				int bitset = 0;
				for (int i = 0; i < bytes.Length; i++)
				{
					int b = bytes[i] & 0xFF;
					bitset |= b;
					pb |= (long)b << (i * PACKED_BYTE_SIZE);
				}
				if (!InRange(bitset))
				{
					return 0;
				}
				return pb;
			}
			internal static long PackedBytes(int b0, int b1)
			{
				assert(InRange(b0 | b1));
				return ((b0 << 0 * PACKED_BYTE_SIZE) | (b1 << 1 * PACKED_BYTE_SIZE));
			}
			internal static long PackedBytes(int b0, int b1, int b2)
			{
				assert(InRange(b0 | b1 | b2));
				return ((b0 << 0 * PACKED_BYTE_SIZE) | (b1 << 1 * PACKED_BYTE_SIZE) | (b2 << 2 * PACKED_BYTE_SIZE));
			}
			internal static long PackedBytes(int b0, int b1, int b2, int b3)
			{
				assert(InRange(b0 | b1 | b2 | b3));
				return ((b0 << 0 * PACKED_BYTE_SIZE) | (b1 << 1 * PACKED_BYTE_SIZE) | (b2 << 2 * PACKED_BYTE_SIZE) | (b3 << 3 * PACKED_BYTE_SIZE));
			}
			internal static bool InRange(int bitset)
			{
				assert((bitset & 0xFF) == bitset); // incoming values must fit in *unsigned* byte
				return ((bitset & ~PACKED_BYTE_MASK) == 0);
			}
			internal static sbyte[] FullBytes(params int[] byteValues)
			{
				sbyte[] bytes = new sbyte[byteValues.Length];
				int i = 0;
				foreach (int bv in byteValues)
				{
					bytes[i++] = Bval(bv);
				}
				assert(PackedBytes(bytes) == 0);
				return bytes;
			}

			internal sbyte ByteAt(int i)
			{
				long pb = PackedBytes_Renamed;
				if (pb == 0)
				{
					if (i >= FullBytes_Renamed.Length)
					{
						return 0;
					}
					return FullBytes_Renamed[i];
				}
				assert(FullBytes_Renamed == null);
				if (i > PACKED_BYTE_MAX_LENGTH)
				{
					return 0;
				}
				int pos = (i * PACKED_BYTE_SIZE);
				return (sbyte)(((long)((ulong)pb >> pos)) & PACKED_BYTE_MASK);
			}

			internal Kind Kind()
			{
				return Enum.GetValues(typeof(Kind))[ByteAt(0)];
			}

			internal Transform(long packedBytes, sbyte[] fullBytes, LambdaForm result) : base(result)
			{
				this.PackedBytes_Renamed = packedBytes;
				this.FullBytes_Renamed = fullBytes;
			}
			internal Transform(long packedBytes) : this(packedBytes, null, null)
			{
				assert(packedBytes != 0);
			}
			internal Transform(sbyte[] fullBytes) : this(0, fullBytes, null)
			{
			}

			internal static sbyte Bval(int b)
			{
				assert((b & 0xFF) == b); // incoming value must fit in *unsigned* byte
				return (sbyte)b;
			}
			internal static sbyte Bval(Kind k)
			{
				return Bval(k.ordinal());
			}
			internal static Transform Of(Kind k, int b1)
			{
				sbyte b0 = Bval(k);
				if (InRange(b0 | b1))
				{
					return new Transform(PackedBytes(b0, b1));
				}
				else
				{
					return new Transform(fullBytes(b0, b1));
				}
			}
			internal static Transform Of(Kind k, int b1, int b2)
			{
				sbyte b0 = (sbyte) k.ordinal();
				if (InRange(b0 | b1 | b2))
				{
					return new Transform(PackedBytes(b0, b1, b2));
				}
				else
				{
					return new Transform(fullBytes(b0, b1, b2));
				}
			}
			internal static Transform Of(Kind k, int b1, int b2, int b3)
			{
				sbyte b0 = (sbyte) k.ordinal();
				if (InRange(b0 | b1 | b2 | b3))
				{
					return new Transform(PackedBytes(b0, b1, b2, b3));
				}
				else
				{
					return new Transform(fullBytes(b0, b1, b2, b3));
				}
			}
			internal static readonly sbyte[] NO_BYTES = new sbyte[] {};
			internal static Transform Of(Kind k, params int[] b123)
			{
				return OfBothArrays(k, b123, NO_BYTES);
			}
			internal static Transform Of(Kind k, int b1, sbyte[] b234)
			{
				return OfBothArrays(k, new int[]{b1}, b234);
			}
			internal static Transform Of(Kind k, int b1, int b2, sbyte[] b345)
			{
				return OfBothArrays(k, new int[]{b1, b2}, b345);
			}
			internal static Transform OfBothArrays(Kind k, int[] b123, sbyte[] b456)
			{
				sbyte[] fullBytes = new sbyte[1 + b123.Length + b456.Length];
				int i = 0;
				fullBytes[i++] = Bval(k);
				foreach (int bv in b123)
				{
					fullBytes[i++] = Bval(bv);
				}
				foreach (sbyte bv in b456)
				{
					fullBytes[i++] = bv;
				}
				long packedBytes = PackedBytes(fullBytes);
				if (packedBytes != 0)
				{
					return new Transform(packedBytes);
				}
				else
				{
					return new Transform(fullBytes);
				}
			}

			internal Transform WithResult(LambdaForm result)
			{
				return new Transform(this.PackedBytes_Renamed, this.FullBytes_Renamed, result);
			}

			public override bool Equals(Object obj)
			{
				return obj is Transform && Equals((Transform)obj);
			}
			public bool Equals(Transform that)
			{
				return this.PackedBytes_Renamed == that.PackedBytes_Renamed && Arrays.Equals(this.FullBytes_Renamed, that.FullBytes_Renamed);
			}
			public override int HashCode()
			{
				if (PackedBytes_Renamed != 0)
				{
					assert(FullBytes_Renamed == null);
					return Long.HashCode(PackedBytes_Renamed);
				}
				return Arrays.HashCode(FullBytes_Renamed);
			}
			public override String ToString()
			{
				StringBuilder buf = new StringBuilder();
				long bits = PackedBytes_Renamed;
				if (bits != 0)
				{
					buf.Append("(");
					while (bits != 0)
					{
						buf.Append(bits & PACKED_BYTE_MASK);
						bits = (long)((ulong)bits >> PACKED_BYTE_SIZE);
						if (bits != 0)
						{
							buf.Append(",");
						}
					}
					buf.Append(")");
				}
				if (FullBytes_Renamed != null)
				{
					buf.Append("unpacked");
					buf.Append(Arrays.ToString(FullBytes_Renamed));
				}
				LambdaForm result = get();
				if (result != null)
				{
					buf.Append(" result=");
					buf.Append(result);
				}
				return buf.ToString();
			}
		}

		/// <summary>
		/// Find a previously cached transform equivalent to the given one, and return its result. </summary>
		private LambdaForm GetInCache(Transform key)
		{
			assert(key.get() == null);
			// The transformCache is one of null, Transform, Transform[], or ConcurrentHashMap.
			Object c = LambdaForm.TransformCache;
			Transform k = null;
			if (c is ConcurrentDictionary)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.concurrent.ConcurrentHashMap<Transform,Transform> m = (java.util.concurrent.ConcurrentHashMap<Transform,Transform>) c;
				ConcurrentDictionary<Transform, Transform> m = (ConcurrentDictionary<Transform, Transform>) c;
				k = m[key];
			}
			else if (c == null)
			{
				return null;
			}
			else if (c is Transform)
			{
				// one-element cache avoids overhead of an array
				Transform t = (Transform)c;
				if (t.Equals(key))
				{
					k = t;
				}
			}
			else
			{
				Transform[] ta = (Transform[])c;
				for (int i = 0; i < ta.Length; i++)
				{
					Transform t = ta[i];
					if (t == null)
					{
						break;
					}
					if (t.Equals(key))
					{
						k = t;
						break;
					}
				}
			}
			assert(k == null || key.Equals(k));
			return (k != null) ? k.get() : null;
		}

		/// <summary>
		/// Arbitrary but reasonable limits on Transform[] size for cache. </summary>
		private const int MIN_CACHE_ARRAY_SIZE = 4, MAX_CACHE_ARRAY_SIZE = 16;

		/// <summary>
		/// Cache a transform with its result, and return that result.
		///  But if an equivalent transform has already been cached, return its result instead.
		/// </summary>
		private LambdaForm PutInCache(Transform key, LambdaForm form)
		{
			key = key.WithResult(form);
			for (int pass = 0; ; pass++)
			{
				Object c = LambdaForm.TransformCache;
				if (c is ConcurrentDictionary)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.concurrent.ConcurrentHashMap<Transform,Transform> m = (java.util.concurrent.ConcurrentHashMap<Transform,Transform>) c;
					ConcurrentDictionary<Transform, Transform> m = (ConcurrentDictionary<Transform, Transform>) c;
					Transform k = m.GetOrAdd(key, key);
					if (k == null)
					{
						return form;
					}
					LambdaForm result = k.get();
					if (result != null)
					{
						return result;
					}
					else
					{
						if (m.replace(key, k, key))
						{
							return form;
						}
						else
						{
							continue;
						}
					}
				}
				assert(pass == 0);
				lock (LambdaForm)
				{
					c = LambdaForm.TransformCache;
					if (c is ConcurrentDictionary)
					{
						continue;
					}
					if (c == null)
					{
						LambdaForm.TransformCache = key;
						return form;
					}
					Transform[] ta;
					if (c is Transform)
					{
						Transform k = (Transform)c;
						if (k.Equals(key))
						{
							LambdaForm result = k.get();
							if (result == null)
							{
								LambdaForm.TransformCache = key;
								return form;
							}
							else
							{
								return result;
							}
						} // overwrite stale entry
						else if (k.get() == null)
						{
							LambdaForm.TransformCache = key;
							return form;
						}
						// expand one-element cache to small array
						ta = new Transform[MIN_CACHE_ARRAY_SIZE];
						ta[0] = k;
						LambdaForm.TransformCache = ta;
					}
					else
					{
						// it is already expanded
						ta = (Transform[])c;
					}
					int len = ta.Length;
					int stale = -1;
					int i;
					for (i = 0; i < len; i++)
					{
						Transform k = ta[i];
						if (k == null)
						{
							break;
						}
						if (k.Equals(key))
						{
							LambdaForm result = k.get();
							if (result == null)
							{
								ta[i] = key;
								return form;
							}
							else
							{
								return result;
							}
						}
						else if (stale < 0 && k.get() == null)
						{
							stale = i; // remember 1st stale entry index
						}
					}
					if (i < len || stale >= 0)
					{
						// just fall through to cache update
					}
					else if (len < MAX_CACHE_ARRAY_SIZE)
					{
						len = System.Math.Min(len * 2, MAX_CACHE_ARRAY_SIZE);
						ta = Arrays.CopyOf(ta, len);
						LambdaForm.TransformCache = ta;
					}
					else
					{
						ConcurrentDictionary<Transform, Transform> m = new ConcurrentDictionary<Transform, Transform>(MAX_CACHE_ARRAY_SIZE * 2);
						foreach (Transform k in ta)
						{
							m[k] = k;
						}
						LambdaForm.TransformCache = m;
						// The second iteration will update for this query, concurrently.
						continue;
					}
					int idx = (stale >= 0) ? stale : i;
					ta[idx] = key;
					return form;
				}
			}
		}

		private LambdaFormBuffer Buffer()
		{
			return new LambdaFormBuffer(LambdaForm);
		}

		/// Editing methods for method handles.  These need to have fast paths.

		private BoundMethodHandle.SpeciesData OldSpeciesData()
		{
			return BoundMethodHandle.SpeciesData(LambdaForm);
		}
		private BoundMethodHandle.SpeciesData NewSpeciesData(BasicType type)
		{
			return OldSpeciesData().ExtendWith(type);
		}

		internal virtual BoundMethodHandle BindArgumentL(BoundMethodHandle mh, int pos, Object value)
		{
			assert(mh.SpeciesData() == OldSpeciesData());
			BasicType bt = L_TYPE;
			MethodType type2 = BindArgumentType(mh, pos, bt);
			LambdaForm form2 = BindArgumentForm(1 + pos);
			return mh.CopyWithExtendL(type2, form2, value);
		}
		internal virtual BoundMethodHandle BindArgumentI(BoundMethodHandle mh, int pos, int value)
		{
			assert(mh.SpeciesData() == OldSpeciesData());
			BasicType bt = I_TYPE;
			MethodType type2 = BindArgumentType(mh, pos, bt);
			LambdaForm form2 = BindArgumentForm(1 + pos);
			return mh.CopyWithExtendI(type2, form2, value);
		}

		internal virtual BoundMethodHandle BindArgumentJ(BoundMethodHandle mh, int pos, long value)
		{
			assert(mh.SpeciesData() == OldSpeciesData());
			BasicType bt = J_TYPE;
			MethodType type2 = BindArgumentType(mh, pos, bt);
			LambdaForm form2 = BindArgumentForm(1 + pos);
			return mh.CopyWithExtendJ(type2, form2, value);
		}

		internal virtual BoundMethodHandle BindArgumentF(BoundMethodHandle mh, int pos, float value)
		{
			assert(mh.SpeciesData() == OldSpeciesData());
			BasicType bt = F_TYPE;
			MethodType type2 = BindArgumentType(mh, pos, bt);
			LambdaForm form2 = BindArgumentForm(1 + pos);
			return mh.CopyWithExtendF(type2, form2, value);
		}

		internal virtual BoundMethodHandle BindArgumentD(BoundMethodHandle mh, int pos, double value)
		{
			assert(mh.SpeciesData() == OldSpeciesData());
			BasicType bt = D_TYPE;
			MethodType type2 = BindArgumentType(mh, pos, bt);
			LambdaForm form2 = BindArgumentForm(1 + pos);
			return mh.CopyWithExtendD(type2, form2, value);
		}

		private MethodType BindArgumentType(BoundMethodHandle mh, int pos, BasicType bt)
		{
			assert(mh.Form == LambdaForm);
			assert(mh.Form.Names[1 + pos].Type_Renamed == bt);
			assert(BasicType.basicType(mh.Type().ParameterType(pos)) == bt);
			return mh.Type().DropParameterTypes(pos, pos + 1);
		}

		/// Editing methods for lambda forms.
		// Each editing method can (potentially) cache the edited LF so that it can be reused later.

		internal virtual LambdaForm BindArgumentForm(int pos)
		{
			Transform key = Transform.Of(Transform.Kind.BIND_ARG, pos);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.ParameterConstraint(0) == NewSpeciesData(LambdaForm.ParameterType(pos)));
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			BoundMethodHandle.SpeciesData oldData = OldSpeciesData();
			BoundMethodHandle.SpeciesData newData = NewSpeciesData(LambdaForm.ParameterType(pos));
			Name oldBaseAddress = LambdaForm.Parameter(0); // BMH holding the values
			Name newBaseAddress;
			NamedFunction getter = newData.GetterFunction(oldData.FieldCount());

			if (pos != 0)
			{
				// The newly created LF will run with a different BMH.
				// Switch over any pre-existing BMH field references to the new BMH class.
				buf.ReplaceFunctions(oldData.GetterFunctions(), newData.GetterFunctions(), oldBaseAddress);
				newBaseAddress = oldBaseAddress.withConstraint(newData);
				buf.RenameParameter(0, newBaseAddress);
				buf.ReplaceParameterByNewExpression(pos, new Name(getter, newBaseAddress));
			}
			else
			{
				// cannot bind the MH arg itself, unless oldData is empty
				assert(oldData == BoundMethodHandle.SpeciesData.EMPTY);
				newBaseAddress = (new Name(L_TYPE)).withConstraint(newData);
				buf.ReplaceParameterByNewExpression(0, new Name(getter, newBaseAddress));
				buf.InsertParameter(0, newBaseAddress);
			}

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm AddArgumentForm(int pos, BasicType type)
		{
			Transform key = Transform.Of(Transform.Kind.ADD_ARG, pos, type.ordinal());
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed + 1);
				assert(form.ParameterType(pos) == type);
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			buf.InsertParameter(pos, new Name(type));

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm DupArgumentForm(int srcPos, int dstPos)
		{
			Transform key = Transform.Of(Transform.Kind.DUP_ARG, srcPos, dstPos);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed - 1);
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			assert(LambdaForm.Parameter(srcPos).Constraint == null);
			assert(LambdaForm.Parameter(dstPos).Constraint == null);
			buf.ReplaceParameterByCopy(dstPos, srcPos);

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm SpreadArgumentsForm(int pos, Class arrayType, int arrayLength)
		{
			Class elementType = arrayType.ComponentType;
			Class erasedArrayType = arrayType;
			if (!elementType.Primitive)
			{
				erasedArrayType = typeof(Object[]);
			}
			BasicType bt = basicType(elementType);
			int elementTypeKey = bt.ordinal();
			if (bt.basicTypeClass() != elementType)
			{
				if (elementType.Primitive)
				{
					elementTypeKey = TYPE_LIMIT + Wrapper.forPrimitiveType(elementType).ordinal();
				}
			}
			Transform key = Transform.Of(Transform.Kind.SPREAD_ARGS, pos, elementTypeKey, arrayLength);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed - arrayLength + 1);
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			assert(pos <= MethodType.MAX_JVM_ARITY);
			assert(pos + arrayLength <= LambdaForm.Arity_Renamed);
			assert(pos > 0); // cannot spread the MH arg itself

			Name spreadParam = new Name(L_TYPE);
			Name checkSpread = new Name(MethodHandleImpl.Lazy.NF_checkSpreadArgument, spreadParam, arrayLength);

			// insert the new expressions
			int exprPos = LambdaForm.Arity();
			buf.InsertExpression(exprPos++, checkSpread);
			// adjust the arguments
			MethodHandle aload = MethodHandles.ArrayElementGetter(erasedArrayType);
			for (int i = 0; i < arrayLength; i++)
			{
				Name loadArgument = new Name(aload, spreadParam, i);
				buf.InsertExpression(exprPos + i, loadArgument);
				buf.ReplaceParameterByCopy(pos + i, exprPos + i);
			}
			buf.InsertParameter(pos, spreadParam);

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm CollectArgumentsForm(int pos, MethodType collectorType)
		{
			int collectorArity = collectorType.ParameterCount();
			bool dropResult = (collectorType.ReturnType() == typeof(void));
			if (collectorArity == 1 && !dropResult)
			{
				return FilterArgumentForm(pos, basicType(collectorType.ParameterType(0)));
			}
			BasicType[] newTypes = BasicType.basicTypes(collectorType.ParameterList());
			Transform.Kind kind = (dropResult ? Transform.Kind.COLLECT_ARGS_TO_VOID : Transform.Kind.COLLECT_ARGS);
			if (dropResult && collectorArity == 0) // pure side effect
			{
				pos = 1;
			}
			Transform key = Transform.Of(kind, pos, collectorArity, BasicType.basicTypesOrd(newTypes));
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed - (dropResult ? 0 : 1) + collectorArity);
				return form;
			}
			form = MakeArgumentCombinationForm(pos, collectorType, false, dropResult);
			return PutInCache(key, form);
		}

		internal virtual LambdaForm CollectArgumentArrayForm(int pos, MethodHandle arrayCollector)
		{
			MethodType collectorType = arrayCollector.Type();
			int collectorArity = collectorType.ParameterCount();
			assert(arrayCollector.IntrinsicName() == Intrinsic.NEW_ARRAY);
			Class arrayType = collectorType.ReturnType();
			Class elementType = arrayType.ComponentType;
			BasicType argType = basicType(elementType);
			int argTypeKey = argType.ordinal();
			if (argType.basicTypeClass() != elementType)
			{
				// return null if it requires more metadata (like String[].class)
				if (!elementType.Primitive)
				{
					return null;
				}
				argTypeKey = TYPE_LIMIT + Wrapper.forPrimitiveType(elementType).ordinal();
			}
			assert(collectorType.ParameterList().Equals(Collections.NCopies(collectorArity, elementType)));
			Transform.Kind kind = Transform.Kind.COLLECT_ARGS_TO_ARRAY;
			Transform key = Transform.Of(kind, pos, collectorArity, argTypeKey);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed - 1 + collectorArity);
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			assert(pos + 1 <= LambdaForm.Arity_Renamed);
			assert(pos > 0); // cannot filter the MH arg itself

			Name[] newParams = new Name[collectorArity];
			for (int i = 0; i < collectorArity; i++)
			{
				newParams[i] = new Name(pos + i, argType);
			}
			Name callCombiner = new Name(arrayCollector, (Object[]) newParams); //...

			// insert the new expression
			int exprPos = LambdaForm.Arity();
			buf.InsertExpression(exprPos, callCombiner);

			// insert new arguments
			int argPos = pos + 1; // skip result parameter
			foreach (Name newParam in newParams)
			{
				buf.InsertParameter(argPos++, newParam);
			}
			assert(buf.LastIndexOf(callCombiner) == exprPos + newParams.Length);
			buf.ReplaceParameterByCopy(pos, exprPos + newParams.Length);

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm FilterArgumentForm(int pos, BasicType newType)
		{
			Transform key = Transform.Of(Transform.Kind.FILTER_ARG, pos, newType.ordinal());
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed);
				assert(form.ParameterType(pos) == newType);
				return form;
			}

			BasicType oldType = LambdaForm.ParameterType(pos);
			MethodType filterType = MethodType.MethodType(oldType.basicTypeClass(), newType.basicTypeClass());
			form = MakeArgumentCombinationForm(pos, filterType, false, false);
			return PutInCache(key, form);
		}

		private LambdaForm MakeArgumentCombinationForm(int pos, MethodType combinerType, bool keepArguments, bool dropResult)
		{
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();
			int combinerArity = combinerType.ParameterCount();
			int resultArity = (dropResult ? 0 : 1);

			assert(pos <= MethodType.MAX_JVM_ARITY);
			assert(pos + resultArity + (keepArguments ? combinerArity : 0) <= LambdaForm.Arity_Renamed);
			assert(pos > 0); // cannot filter the MH arg itself
			assert(combinerType == combinerType.BasicType());
			assert(combinerType.ReturnType() != typeof(void) || dropResult);

			BoundMethodHandle.SpeciesData oldData = OldSpeciesData();
			BoundMethodHandle.SpeciesData newData = NewSpeciesData(L_TYPE);

			// The newly created LF will run with a different BMH.
			// Switch over any pre-existing BMH field references to the new BMH class.
			Name oldBaseAddress = LambdaForm.Parameter(0); // BMH holding the values
			buf.ReplaceFunctions(oldData.GetterFunctions(), newData.GetterFunctions(), oldBaseAddress);
			Name newBaseAddress = oldBaseAddress.withConstraint(newData);
			buf.RenameParameter(0, newBaseAddress);

			Name getCombiner = new Name(newData.GetterFunction(oldData.FieldCount()), newBaseAddress);
			Object[] combinerArgs = new Object[1 + combinerArity];
			combinerArgs[0] = getCombiner;
			Name[] newParams;
			if (keepArguments)
			{
				newParams = new Name[0];
				System.Array.Copy(LambdaForm.Names, pos + resultArity, combinerArgs, 1, combinerArity);
			}
			else
			{
				newParams = new Name[combinerArity];
				BasicType[] newTypes = basicTypes(combinerType.ParameterList());
				for (int i = 0; i < newTypes.Length; i++)
				{
					newParams[i] = new Name(pos + i, newTypes[i]);
				}
				System.Array.Copy(newParams, 0, combinerArgs, 1, combinerArity);
			}
			Name callCombiner = new Name(combinerType, combinerArgs);

			// insert the two new expressions
			int exprPos = LambdaForm.Arity();
			buf.InsertExpression(exprPos + 0, getCombiner);
			buf.InsertExpression(exprPos + 1, callCombiner);

			// insert new arguments, if needed
			int argPos = pos + resultArity; // skip result parameter
			foreach (Name newParam in newParams)
			{
				buf.InsertParameter(argPos++, newParam);
			}
			assert(buf.LastIndexOf(callCombiner) == exprPos + 1 + newParams.Length);
			if (!dropResult)
			{
				buf.ReplaceParameterByCopy(pos, exprPos + 1 + newParams.Length);
			}

			return buf.EndEdit();
		}

		internal virtual LambdaForm FilterReturnForm(BasicType newType, bool constantZero)
		{
			Transform.Kind kind = (constantZero ? Transform.Kind.FILTER_RETURN_TO_ZERO : Transform.Kind.FILTER_RETURN);
			Transform key = Transform.Of(kind, newType.ordinal());
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed);
				assert(form.ReturnType() == newType);
				return form;
			}
			LambdaFormBuffer buf = Buffer();
			buf.StartEdit();

			int insPos = LambdaForm.Names.Length;
			Name callFilter;
			if (constantZero)
			{
				// Synthesize a constant zero value for the given type.
				if (newType == V_TYPE)
				{
					callFilter = null;
				}
				else
				{
					callFilter = new Name(constantZero(newType));
				}
			}
			else
			{
				BoundMethodHandle.SpeciesData oldData = OldSpeciesData();
				BoundMethodHandle.SpeciesData newData = NewSpeciesData(L_TYPE);

				// The newly created LF will run with a different BMH.
				// Switch over any pre-existing BMH field references to the new BMH class.
				Name oldBaseAddress = LambdaForm.Parameter(0); // BMH holding the values
				buf.ReplaceFunctions(oldData.GetterFunctions(), newData.GetterFunctions(), oldBaseAddress);
				Name newBaseAddress = oldBaseAddress.withConstraint(newData);
				buf.RenameParameter(0, newBaseAddress);

				Name getFilter = new Name(newData.GetterFunction(oldData.FieldCount()), newBaseAddress);
				buf.InsertExpression(insPos++, getFilter);
				BasicType oldType = LambdaForm.ReturnType();
				if (oldType == V_TYPE)
				{
					MethodType filterType = MethodType.MethodType(newType.basicTypeClass());
					callFilter = new Name(filterType, getFilter);
				}
				else
				{
					MethodType filterType = MethodType.MethodType(newType.basicTypeClass(), oldType.basicTypeClass());
					callFilter = new Name(filterType, getFilter, LambdaForm.Names[LambdaForm.Result]);
				}
			}

			if (callFilter != null)
			{
				buf.InsertExpression(insPos++, callFilter);
			}
			buf.Result = callFilter;

			form = buf.EndEdit();
			return PutInCache(key, form);
		}

		internal virtual LambdaForm FoldArgumentsForm(int foldPos, bool dropResult, MethodType combinerType)
		{
			int combinerArity = combinerType.ParameterCount();
			Transform.Kind kind = (dropResult ? Transform.Kind.FOLD_ARGS_TO_VOID : Transform.Kind.FOLD_ARGS);
			Transform key = Transform.Of(kind, foldPos, combinerArity);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == LambdaForm.Arity_Renamed - (kind == Transform.Kind.FOLD_ARGS ? 1 : 0));
				return form;
			}
			form = MakeArgumentCombinationForm(foldPos, combinerType, true, dropResult);
			return PutInCache(key, form);
		}

		internal virtual LambdaForm PermuteArgumentsForm(int skip, int[] reorder)
		{
			assert(skip == 1); // skip only the leading MH argument, names[0]
			int length = LambdaForm.Names.Length;
			int outArgs = reorder.Length;
			int inTypes = 0;
			bool nullPerm = true;
			for (int i = 0; i < reorder.Length; i++)
			{
				int inArg = reorder[i];
				if (inArg != i)
				{
					nullPerm = false;
				}
				inTypes = System.Math.Max(inTypes, inArg + 1);
			}
			assert(skip + reorder.Length == LambdaForm.Arity_Renamed);
			if (nullPerm) // do not bother to cache
			{
				return LambdaForm;
			}
			Transform key = Transform.Of(Transform.Kind.PERMUTE_ARGS, reorder);
			LambdaForm form = GetInCache(key);
			if (form != null)
			{
				assert(form.Arity_Renamed == skip + inTypes) : form;
				return form;
			}

			BasicType[] types = new BasicType[inTypes];
			for (int i = 0; i < outArgs; i++)
			{
				int inArg = reorder[i];
				types[inArg] = LambdaForm.Names[skip + i].Type_Renamed;
			}
			assert(skip + outArgs == LambdaForm.Arity_Renamed);
			assert(PermutedTypesMatch(reorder, types, LambdaForm.Names, skip));
			int pos = 0;
			while (pos < outArgs && reorder[pos] == pos)
			{
				pos += 1;
			}
			Name[] names2 = new Name[length - outArgs + inTypes];
			System.Array.Copy(LambdaForm.Names, 0, names2, 0, skip + pos);
			int bodyLength = length - LambdaForm.Arity_Renamed;
			System.Array.Copy(LambdaForm.Names, skip + outArgs, names2, skip + inTypes, bodyLength);
			int arity2 = names2.Length - bodyLength;
			int result2 = LambdaForm.Result;
			if (result2 >= 0)
			{
				if (result2 < skip + outArgs)
				{
					result2 = reorder[result2 - skip];
				}
				else
				{
					result2 = result2 - outArgs + inTypes;
				}
			}
			for (int j = pos; j < outArgs; j++)
			{
				Name n = LambdaForm.Names[skip + j];
				int i = reorder[j];
				Name n2 = names2[skip + i];
				if (n2 == null)
				{
					names2[skip + i] = n2 = new Name(types[i]);
				}
				else
				{
					assert(n2.type == types[i]);
				}
				for (int k = arity2; k < names2.Length; k++)
				{
					names2[k] = names2[k].replaceName(n, n2);
				}
			}
			for (int i = skip + pos; i < arity2; i++)
			{
				if (names2[i] == null)
				{
					names2[i] = argument(i, types[i - skip]);
				}
			}
			for (int j = LambdaForm.Arity_Renamed; j < LambdaForm.Names.Length; j++)
			{
				int i = j - LambdaForm.Arity_Renamed + arity2;
				Name n = LambdaForm.Names[j];
				Name n2 = names2[i];
				if (n != n2)
				{
					for (int k = i + 1; k < names2.Length; k++)
					{
						names2[k] = names2[k].replaceName(n, n2);
					}
				}
			}

			form = new LambdaForm(LambdaForm.DebugName, arity2, names2, result2);
			return PutInCache(key, form);
		}

		internal static bool PermutedTypesMatch(int[] reorder, BasicType[] types, Name[] names, int skip)
		{
			for (int i = 0; i < reorder.Length; i++)
			{
				assert(names[skip + i].Param);
				assert(names[skip + i].type == types[reorder[i]]);
			}
			return true;
		}
	}

}