using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The symbolic, non-executable form of a method handle's invocation semantics.
	/// It consists of a series of names.
	/// The first N (N=arity) names are parameters,
	/// while any remaining names are temporary values.
	/// Each temporary specifies the application of a function to some arguments.
	/// The functions are method handles, while the arguments are mixes of
	/// constant values and local names.
	/// The result of the lambda is defined as one of the names, often the last one.
	/// <para>
	/// Here is an approximate grammar:
	/// <blockquote><pre>{@code
	/// LambdaForm = "(" ArgName* ")=>{" TempName* Result "}"
	/// ArgName = "a" N ":" T
	/// TempName = "t" N ":" T "=" Function "(" Argument* ");"
	/// Function = ConstantValue
	/// Argument = NameRef | ConstantValue
	/// Result = NameRef | "void"
	/// NameRef = "a" N | "t" N
	/// N = (any whole number)
	/// T = "L" | "I" | "J" | "F" | "D" | "V"
	/// }</pre></blockquote>
	/// Names are numbered consecutively from left to right starting at zero.
	/// (The letters are merely a taste of syntax sugar.)
	/// Thus, the first temporary (if any) is always numbered N (where N=arity).
	/// Every occurrence of a name reference in an argument list must refer to
	/// a name previously defined within the same lambda.
	/// A lambda has a void result if and only if its result index is -1.
	/// If a temporary has the type "V", it cannot be the subject of a NameRef,
	/// even though possesses a number.
	/// Note that all reference types are erased to "L", which stands for {@code Object}.
	/// All subword types (boolean, byte, short, char) are erased to "I" which is {@code int}.
	/// The other types stand for the usual primitive types.
	/// </para>
	/// <para>
	/// Function invocation closely follows the static rules of the Java verifier.
	/// Arguments and return values must exactly match when their "Name" types are
	/// considered.
	/// Conversions are allowed only if they do not change the erased type.
	/// <ul>
	/// <li>L = Object: casts are used freely to convert into and out of reference types
	/// <li>I = int: subword types are forcibly narrowed when passed as arguments (see {@code explicitCastArguments})
	/// <li>J = long: no implicit conversions
	/// <li>F = float: no implicit conversions
	/// <li>D = double: no implicit conversions
	/// <li>V = void: a function result may be void if and only if its Name is of type "V"
	/// </ul>
	/// Although implicit conversions are not allowed, explicit ones can easily be
	/// encoded by using temporary expressions which call type-transformed identity functions.
	/// </para>
	/// <para>
	/// Examples:
	/// <blockquote><pre>{@code
	/// (a0:J)=>{ a0 }
	///     == identity(long)
	/// (a0:I)=>{ t1:V = System.out#println(a0); void }
	///     == System.out#println(int)
	/// (a0:L)=>{ t1:V = System.out#println(a0); a0 }
	///     == identity, with printing side-effect
	/// (a0:L, a1:L)=>{ t2:L = BoundMethodHandle#argument(a0);
	///                 t3:L = BoundMethodHandle#target(a0);
	///                 t4:L = MethodHandle#invoke(t3, t2, a1); t4 }
	///     == general invoker for unary insertArgument combination
	/// (a0:L, a1:L)=>{ t2:L = FilterMethodHandle#filter(a0);
	///                 t3:L = MethodHandle#invoke(t2, a1);
	///                 t4:L = FilterMethodHandle#target(a0);
	///                 t5:L = MethodHandle#invoke(t4, t3); t5 }
	///     == general invoker for unary filterArgument combination
	/// (a0:L, a1:L)=>{ ...(same as previous example)...
	///                 t5:L = MethodHandle#invoke(t4, t3, a1); t5 }
	///     == general invoker for unary/unary foldArgument combination
	/// (a0:L, a1:I)=>{ t2:I = identity(long).asType((int)->long)(a1); t2 }
	///     == invoker for identity method handle which performs i2l
	/// (a0:L, a1:L)=>{ t2:L = BoundMethodHandle#argument(a0);
	///                 t3:L = Class#cast(t2,a1); t3 }
	///     == invoker for identity method handle which performs cast
	/// }</pre></blockquote>
	/// </para>
	/// <para>
	/// @author John Rose, JSR 292 EG
	/// </para>
	/// </summary>
	internal class LambdaForm
	{
		internal readonly int Arity_Renamed;
		internal readonly int Result;
		internal readonly bool ForceInline;
		internal readonly MethodHandle Customized;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable final Name[] names;
		internal readonly Name[] Names;
		internal readonly String DebugName;
		internal MemberName Vmentry; // low-level behavior, or null if not yet prepared
		private bool IsCompiled;

		// Either a LambdaForm cache (managed by LambdaFormEditor) or a link to uncustomized version (for customized LF)
		internal volatile Object TransformCache;

		public const int VOID_RESULT = -1, LAST_RESULT = -2;

		internal sealed class BasicType
		{
			public static readonly BasicType L_TYPE = new BasicType("L_TYPE", InnerEnum.L_TYPE, 'L', Object.class, sun.invoke.util.Wrapper.OBJECT);
			public static readonly BasicType I_TYPE = new BasicType("I_TYPE", InnerEnum.I_TYPE, 'I', int.class, sun.invoke.util.Wrapper.INT);
			public static readonly BasicType J_TYPE = new BasicType("J_TYPE", InnerEnum.J_TYPE, 'J', long.class, sun.invoke.util.Wrapper.LONG);
			public static readonly BasicType F_TYPE = new BasicType("F_TYPE", InnerEnum.F_TYPE, 'F', float.class, sun.invoke.util.Wrapper.FLOAT);
			public static readonly BasicType D_TYPE = new BasicType("D_TYPE", InnerEnum.D_TYPE, 'D', double.class, sun.invoke.util.Wrapper.DOUBLE);
			public static readonly BasicType V_TYPE = new BasicType("V_TYPE", InnerEnum.V_TYPE, 'V', void.class, sun.invoke.util.Wrapper.VOID);

			private static readonly IList<BasicType> valueList = new List<BasicType>();

			static BasicType()
			{
				valueList.Add(L_TYPE);
				valueList.Add(I_TYPE);
				valueList.Add(J_TYPE);
				valueList.Add(F_TYPE);
				valueList.Add(D_TYPE);
				valueList.Add(V_TYPE);
			}

			public enum InnerEnum
			{
				L_TYPE,
				I_TYPE,
				J_TYPE,
				F_TYPE,
				D_TYPE,
				V_TYPE
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			internal static readonly BasicType[] ALL_TYPES = BasicType.values();
			public static readonly BasicType static final BasicType[] ARG_TYPES = java.util.Arrays.copyOf = new BasicType("static final BasicType[] ARG_TYPES = java.util.Arrays.copyOf", InnerEnum.static final BasicType[] ARG_TYPES = java.util.Arrays.copyOf, ALL_TYPES, ALL_TYPES.length - 1);

			private static readonly IList<BasicType> valueList = new List<BasicType>();

			static BasicType()
			{
				valueList.Add(L_TYPE);
				valueList.Add(I_TYPE);
				valueList.Add(J_TYPE);
				valueList.Add(F_TYPE);
				valueList.Add(D_TYPE);
				valueList.Add(V_TYPE);
				valueList.Add(static final BasicType[] ARG_TYPES = java.util.Arrays.copyOf);
			}

			public enum InnerEnum
			{
				L_TYPE,
				I_TYPE,
				J_TYPE,
				F_TYPE,
				D_TYPE,
				V_TYPE,
				static final BasicType[] ARG_TYPES = java.util.Arrays.copyOf
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			internal static readonly int ARG_TYPE_LIMIT = ARG_TYPES.length;
			internal static readonly int TYPE_LIMIT = ALL_TYPES.length;

			internal readonly char btChar;
			internal readonly Class btClass;
			internal readonly sun.invoke.util.Wrapper btWrapper;

			internal BasicType(string name, InnerEnum innerEnum, LambdaForm outerInstance, char btChar, Class btClass, sun.invoke.util.Wrapper wrapper)
			{
				this.outerInstance = outerInstance;
				this.btChar = btChar;
				this.btClass = btClass;
				this.btWrapper = wrapper;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			internal char BasicTypeChar()
			{
				return btChar;
			}
			internal Class BasicTypeClass()
			{
				return btClass;
			}
			internal sun.invoke.util.Wrapper BasicTypeWrapper()
			{
				return btWrapper;
			}
			internal int BasicTypeSlots()
			{
				return btWrapper.stackSlots();
			}

			internal static BasicType BasicType(sbyte type)
			{
				return ALL_TYPES[type];
			}
			internal static BasicType BasicType(char type)
			{
				switch (type)
				{
					case 'L':
						return L_TYPE;
					case 'I':
						return I_TYPE;
					case 'J':
						return J_TYPE;
					case 'F':
						return F_TYPE;
					case 'D':
						return D_TYPE;
					case 'V':
						return V_TYPE;
					// all subword types are represented as ints
					case 'Z':
					case 'B':
					case 'S':
					case 'C':
						return I_TYPE;
					default:
						throw newInternalError("Unknown type char: '" + type + "'");
				}
			}
			internal static BasicType BasicType(sun.invoke.util.Wrapper type)
			{
				char c = type.basicTypeChar();
				return basicType(c);
			}
			internal static BasicType BasicType(Class type)
			{
				if (!type.Primitive)
				{
					return L_TYPE;
				}
				return basicType(Wrapper.forPrimitiveType(type));
			}

			internal static char BasicTypeChar(Class type)
			{
				return basicType(type).btChar;
			}
			internal static BasicType[] BasicTypes(IList<Class> types)
			{
				BasicType[] btypes = new BasicType[types.Count];
				for (int i = 0; i < btypes.Length; i++)
				{
					btypes[i] = basicType(types[i]);
				}
				return btypes;
			}
			internal static BasicType[] BasicTypes(String types)
			{
				BasicType[] btypes = new BasicType[types.Length()];
				for (int i = 0; i < btypes.Length; i++)
				{
					btypes[i] = basicType(types.CharAt(i));
				}
				return btypes;
			}
			internal static byte[] BasicTypesOrd(BasicType[] btypes)
			{
				sbyte[] ords = new sbyte[btypes.Length];
				for (int i = 0; i < btypes.Length; i++)
				{
					ords[i] = (sbyte)btypes[i].ordinal();
				}
				return ords;
			}
			internal static boolean IsBasicTypeChar(char c)
			{
				return "LIJFDV".IndexOf(c) >= 0;
			}
			internal static boolean IsArgBasicTypeChar(char c)
			{
				return "LIJFD".IndexOf(c) >= 0;
			}

			static BasicType()
			{
				assert(checkBasicType());

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}
			internal static boolean CheckBasicType()
			{
				for (int i = 0; i < ARG_TYPE_LIMIT; i++)
				{
					Debug.Assert(ARG_TYPES[i].ordinal() == i);
					Debug.Assert(ARG_TYPES[i] == ALL_TYPES[i]);
				}
				for (int i = 0; i < TYPE_LIMIT; i++)
				{
					Debug.Assert(ALL_TYPES[i].ordinal() == i);
				}
				Debug.Assert(ALL_TYPES[TYPE_LIMIT - 1] == V_TYPE);
				Debug.Assert(!Arrays.AsList(ARG_TYPES).Contains(V_TYPE));
				return true;
			}

			public static IList<BasicType> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static BasicType valueOf(string name)
			{
				foreach (BasicType enumInstance in BasicType.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		internal LambdaForm(String debugName, int arity, Name[] names, int result) : this(debugName, arity, names, result, true, null); / /customized = - forceInline=
		{
		}
		internal LambdaForm(String debugName, int arity, Name[] names, int result, bool forceInline, MethodHandle customized)
		{
			assert(NamesOK(arity, names));
			this.Arity_Renamed = arity;
			this.Result = FixResult(result, names);
			this.Names = names.clone();
			this.DebugName = FixDebugName(debugName);
			this.ForceInline = forceInline;
			this.Customized = customized;
			int maxOutArity = Normalize();
			if (maxOutArity > MethodType.MAX_MH_INVOKER_ARITY)
			{
				// Cannot use LF interpreter on very high arity expressions.
				assert(maxOutArity <= MethodType.MAX_JVM_ARITY);
				CompileToBytecode();
			}
		}
		internal LambdaForm(String debugName, int arity, Name[] names) : this(debugName, arity, names, LAST_RESULT, true, null); / /customized = - forceInline=
		{
		}
		internal LambdaForm(String debugName, int arity, Name[] names, bool forceInline) : this(debugName, arity, names, LAST_RESULT, forceInline, null); / /customized=
		{
		}
		internal LambdaForm(String debugName, Name[] formals, Name[] temps, Name result) : this(debugName, formals.Length, BuildNames(formals, temps, result), LAST_RESULT, true, null); / /customized = - forceInline=
		{
		}
		internal LambdaForm(String debugName, Name[] formals, Name[] temps, Name result, bool forceInline) : this(debugName, formals.Length, BuildNames(formals, temps, result), LAST_RESULT, forceInline, null); / /customized=
		{
		}

		private static Name[] BuildNames(Name[] formals, Name[] temps, Name result)
		{
			int arity = formals.Length;
			int length = arity + temps.Length + (result == null ? 0 : 1);
			Name[] names = Arrays.CopyOf(formals, length);
			System.Array.Copy(temps, 0, names, arity, temps.Length);
			if (result != null)
			{
				names[length - 1] = result;
			}
			return names;
		}

		private LambdaForm(String sig)
		{
			// Make a blank lambda form, which returns a constant zero or null.
			// It is used as a template for managing the invocation of similar forms that are non-empty.
			// Called only from getPreparedForm.
			assert(IsValidSignature(sig));
			this.Arity_Renamed = SignatureArity(sig);
			this.Result = (SignatureReturn(sig) == V_TYPE ? - 1 : Arity_Renamed);
			this.Names = BuildEmptyNames(Arity_Renamed, sig);
			this.DebugName = "LF.zero";
			this.ForceInline = true;
			this.Customized = null;
			assert(NameRefsAreLegal());
			assert(Empty);
			assert(sig.Equals(BasicTypeSignature())) : sig + " != " + BasicTypeSignature();
		}

		private static Name[] BuildEmptyNames(int arity, String basicTypeSignature)
		{
			assert(IsValidSignature(basicTypeSignature));
			int resultPos = arity + 1; // skip '_'
			if (arity < 0 || basicTypeSignature.Length() != resultPos + 1)
			{
				throw new IllegalArgumentException("bad arity for " + basicTypeSignature);
			}
			int numRes = (basicType(basicTypeSignature.CharAt(resultPos)) == V_TYPE ? 0 : 1);
			Name[] names = Arguments(numRes, basicTypeSignature.Substring(0, arity));
			for (int i = 0; i < numRes; i++)
			{
				Name zero = new Name(ConstantZero(basicType(basicTypeSignature.CharAt(resultPos + i))));
				names[arity + i] = zero.NewIndex(arity + i);
			}
			return names;
		}

		private static int FixResult(int result, Name[] names)
		{
			if (result == LAST_RESULT)
			{
				result = names.Length - 1; // might still be void
			}
			if (result >= 0 && names[result].Type_Renamed == V_TYPE)
			{
				result = VOID_RESULT;
			}
			return result;
		}

		private static String FixDebugName(String debugName)
		{
			if (DEBUG_NAME_COUNTERS != null)
			{
				int under = debugName.IndexOf('_');
				int length = debugName.Length();
				if (under < 0)
				{
					under = length;
				}
				String debugNameStem = debugName.Substring(0, under);
				Integer ctr;
				lock (DEBUG_NAME_COUNTERS)
				{
					ctr = DEBUG_NAME_COUNTERS[debugNameStem];
					if (ctr == null)
					{
						ctr = 0;
					}
					DEBUG_NAME_COUNTERS[debugNameStem] = ctr + 1;
				}
				StringBuilder buf = new StringBuilder(debugNameStem);
				buf.Append('_');
				int leadingZero = buf.Length();
				buf.Append((int) ctr);
				for (int i = buf.Length() - leadingZero; i < 3; i++)
				{
					buf.Insert(leadingZero, '0');
				}
				if (under < length)
				{
					++under; // skip "_"
					while (under < length && char.IsDigit(debugName.CharAt(under)))
					{
						++under;
					}
					if (under < length && debugName.CharAt(under) == '_')
					{
						++under;
					}
					if (under < length)
					{
						buf.Append('_').Append(debugName, under, length);
					}
				}
				return buf.ToString();
			}
			return debugName;
		}

		private static bool NamesOK(int arity, Name[] names)
		{
			for (int i = 0; i < names.Length; i++)
			{
				Name n = names[i];
				assert(n != null) : "n is null";
				if (i < arity)
				{
					assert(n.Param) : n + " is not param at " + i;
				}
				else
				{
					assert(!n.Param) : n + " is param at " + i;
				}
			}
			return true;
		}

		/// <summary>
		/// Customize LambdaForm for a particular MethodHandle </summary>
		internal virtual LambdaForm Customize(MethodHandle mh)
		{
			LambdaForm customForm = new LambdaForm(DebugName, Arity_Renamed, Names, Result, ForceInline, mh);
			if (COMPILE_THRESHOLD > 0 && IsCompiled)
			{
				// If shared LambdaForm has been compiled, compile customized version as well.
				customForm.CompileToBytecode();
			}
			customForm.TransformCache = this; // LambdaFormEditor should always use uncustomized form.
			return customForm;
		}

		/// <summary>
		/// Get uncustomized flavor of the LambdaForm </summary>
		internal virtual LambdaForm Uncustomize()
		{
			if (Customized == null)
			{
				return this;
			}
			assert(TransformCache != null); // Customized LambdaForm should always has a link to uncustomized version.
			LambdaForm uncustomizedForm = (LambdaForm)TransformCache;
			if (COMPILE_THRESHOLD > 0 && IsCompiled)
			{
				// If customized LambdaForm has been compiled, compile uncustomized version as well.
				uncustomizedForm.CompileToBytecode();
			}
			return uncustomizedForm;
		}

		/// <summary>
		/// Renumber and/or replace params so that they are interned and canonically numbered. </summary>
		///  <returns> maximum argument list length among the names (since we have to pass over them anyway) </returns>
		private int Normalize()
		{
			Name[] oldNames = null;
			int maxOutArity = 0;
			int changesStart = 0;
			for (int i = 0; i < Names.Length; i++)
			{
				Name n = Names[i];
				if (!n.InitIndex(i))
				{
					if (oldNames == null)
					{
						oldNames = Names.clone();
						changesStart = i;
					}
					Names[i] = n.CloneWithIndex(i);
				}
				if (n.Arguments != null && maxOutArity < n.Arguments.Length)
				{
					maxOutArity = n.Arguments.Length;
				}
			}
			if (oldNames != null)
			{
				int startFixing = Arity_Renamed;
				if (startFixing <= changesStart)
				{
					startFixing = changesStart + 1;
				}
				for (int i = startFixing; i < Names.Length; i++)
				{
					Name @fixed = Names[i].ReplaceNames(oldNames, Names, changesStart, i);
					Names[i] = @fixed.NewIndex(i);
				}
			}
			assert(NameRefsAreLegal());
			int maxInterned = System.Math.Min(Arity_Renamed, INTERNED_ARGUMENT_LIMIT);
			bool needIntern = false;
			for (int i = 0; i < maxInterned; i++)
			{
				Name n = Names[i], n2 = InternArgument(n);
				if (n != n2)
				{
					Names[i] = n2;
					needIntern = true;
				}
			}
			if (needIntern)
			{
				for (int i = Arity_Renamed; i < Names.Length; i++)
				{
					Names[i].InternArguments();
				}
			}
			assert(NameRefsAreLegal());
			return maxOutArity;
		}

		/// <summary>
		/// Check that all embedded Name references are localizable to this lambda,
		/// and are properly ordered after their corresponding definitions.
		/// <para>
		/// Note that a Name can be local to multiple lambdas, as long as
		/// it possesses the same index in each use site.
		/// This allows Name references to be freely reused to construct
		/// fresh lambdas, without confusion.
		/// </para>
		/// </summary>
		internal virtual bool NameRefsAreLegal()
		{
			assert(Arity_Renamed >= 0 && Arity_Renamed <= Names.Length);
			assert(Result >= -1 && Result < Names.Length);
			// Do all names possess an index consistent with their local definition order?
			for (int i = 0; i < Arity_Renamed; i++)
			{
				Name n = Names[i];
				assert(n.Index() == i) : Arrays.asList(n.Index(), i);
				assert(n.Param);
			}
			// Also, do all local name references
			for (int i = Arity_Renamed; i < Names.Length; i++)
			{
				Name n = Names[i];
				assert(n.Index() == i);
				foreach (Object arg in n.Arguments)
				{
					if (arg is Name)
					{
						Name n2 = (Name) arg;
						int i2 = n2.Index_Renamed;
						assert(0 <= i2 && i2 < Names.Length) : n.DebugString() + ": 0 <= i2 && i2 < names.length: 0 <= " + i2 + " < " + Names.Length;
						assert(Names[i2] == n2) : Arrays.asList("-1-", i, "-2-", n.DebugString(), "-3-", i2, "-4-", n2.DebugString(), "-5-", Names[i2].DebugString(), "-6-", this);
						assert(i2 < i); // ref must come after def!
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Invoke this form on the given arguments. </summary>
		// final Object invoke(Object... args) throws Throwable {
		//     // NYI: fit this into the fast path?
		//     return interpretWithArguments(args);
		// }

		/// <summary>
		/// Report the return type. </summary>
		internal virtual BasicType ReturnType()
		{
			if (Result < 0)
			{
				return V_TYPE;
			}
			Name n = Names[Result];
			return n.Type_Renamed;
		}

		/// <summary>
		/// Report the N-th argument type. </summary>
		internal virtual BasicType ParameterType(int n)
		{
			return Parameter(n).Type_Renamed;
		}

		/// <summary>
		/// Report the N-th argument name. </summary>
		internal virtual Name Parameter(int n)
		{
			assert(n < Arity_Renamed);
			Name param = Names[n];
			assert(param.Param);
			return param;
		}

		/// <summary>
		/// Report the N-th argument type constraint. </summary>
		internal virtual Object ParameterConstraint(int n)
		{
			return Parameter(n).Constraint;
		}

		/// <summary>
		/// Report the arity. </summary>
		internal virtual int Arity()
		{
			return Arity_Renamed;
		}

		/// <summary>
		/// Report the number of expressions (non-parameter names). </summary>
		internal virtual int ExpressionCount()
		{
			return Names.Length - Arity_Renamed;
		}

		/// <summary>
		/// Return the method type corresponding to my basic type signature. </summary>
		internal virtual MethodType MethodType()
		{
			return SignatureType(BasicTypeSignature());
		}
		/// <summary>
		/// Return ABC_Z, where the ABC are parameter type characters, and Z is the return type character. </summary>
		internal String BasicTypeSignature()
		{
			StringBuilder buf = new StringBuilder(Arity() + 3);
			for (int i = 0, a = Arity(); i < a; i++)
			{
				buf.Append(ParameterType(i).basicTypeChar());
			}
			return buf.Append('_').Append(ReturnType().basicTypeChar()).ToString();
		}
		internal static int SignatureArity(String sig)
		{
			assert(IsValidSignature(sig));
			return sig.IndexOf('_');
		}
		internal static BasicType SignatureReturn(String sig)
		{
			return basicType(sig.CharAt(SignatureArity(sig) + 1));
		}
		internal static bool IsValidSignature(String sig)
		{
			int arity = sig.IndexOf('_');
			if (arity < 0) // must be of the form *_*
			{
				return false;
			}
			int siglen = sig.Length();
			if (siglen != arity + 2) // *_X
			{
				return false;
			}
			for (int i = 0; i < siglen; i++)
			{
				if (i == arity) // skip '_'
				{
					continue;
				}
				char c = sig.CharAt(i);
				if (c == 'V')
				{
					return (i == siglen - 1 && arity == siglen - 2);
				}
				if (!isArgBasicTypeChar(c)) // must be [LIJFD]
				{
					return false;
				}
			}
			return true; // [LIJFD]*_[LIJFDV]
		}
		internal static MethodType SignatureType(String sig)
		{
			Class[] ptypes = new Class[SignatureArity(sig)];
			for (int i = 0; i < ptypes.Length; i++)
			{
				ptypes[i] = basicType(sig.CharAt(i)).btClass;
			}
			Class rtype = SignatureReturn(sig).btClass;
			return MethodType.MethodType(rtype, ptypes);
		}

		/*
		 * Code generation issues:
		 *
		 * Compiled LFs should be reusable in general.
		 * The biggest issue is how to decide when to pull a name into
		 * the bytecode, versus loading a reified form from the MH data.
		 *
		 * For example, an asType wrapper may require execution of a cast
		 * after a call to a MH.  The target type of the cast can be placed
		 * as a constant in the LF itself.  This will force the cast type
		 * to be compiled into the bytecodes and native code for the MH.
		 * Or, the target type of the cast can be erased in the LF, and
		 * loaded from the MH data.  (Later on, if the MH as a whole is
		 * inlined, the data will flow into the inlined instance of the LF,
		 * as a constant, and the end result will be an optimal cast.)
		 *
		 * This erasure of cast types can be done with any use of
		 * reference types.  It can also be done with whole method
		 * handles.  Erasing a method handle might leave behind
		 * LF code that executes correctly for any MH of a given
		 * type, and load the required MH from the enclosing MH's data.
		 * Or, the erasure might even erase the expected MT.
		 *
		 * Also, for direct MHs, the MemberName of the target
		 * could be erased, and loaded from the containing direct MH.
		 * As a simple case, a LF for all int-valued non-static
		 * field getters would perform a cast on its input argument
		 * (to non-constant base type derived from the MemberName)
		 * and load an integer value from the input object
		 * (at a non-constant offset also derived from the MemberName).
		 * Such MN-erased LFs would be inlinable back to optimized
		 * code, whenever a constant enclosing DMH is available
		 * to supply a constant MN from its data.
		 *
		 * The main problem here is to keep LFs reasonably generic,
		 * while ensuring that hot spots will inline good instances.
		 * "Reasonably generic" means that we don't end up with
		 * repeated versions of bytecode or machine code that do
		 * not differ in their optimized form.  Repeated versions
		 * of machine would have the undesirable overheads of
		 * (a) redundant compilation work and (b) extra I$ pressure.
		 * To control repeated versions, we need to be ready to
		 * erase details from LFs and move them into MH data,
		 * whevener those details are not relevant to significant
		 * optimization.  "Significant" means optimization of
		 * code that is actually hot.
		 *
		 * Achieving this may require dynamic splitting of MHs, by replacing
		 * a generic LF with a more specialized one, on the same MH,
		 * if (a) the MH is frequently executed and (b) the MH cannot
		 * be inlined into a containing caller, such as an invokedynamic.
		 *
		 * Compiled LFs that are no longer used should be GC-able.
		 * If they contain non-BCP references, they should be properly
		 * interlinked with the class loader(s) that their embedded types
		 * depend on.  This probably means that reusable compiled LFs
		 * will be tabulated (indexed) on relevant class loaders,
		 * or else that the tables that cache them will have weak links.
		 */

		/// <summary>
		/// Make this LF directly executable, as part of a MethodHandle.
		/// Invariant:  Every MH which is invoked must prepare its LF
		/// before invocation.
		/// (In principle, the JVM could do this very lazily,
		/// as a sort of pre-invocation linkage step.)
		/// </summary>
		public virtual void Prepare()
		{
			if (COMPILE_THRESHOLD == 0 && !IsCompiled)
			{
				CompileToBytecode();
			}
			if (this.Vmentry != null)
			{
				// already prepared (e.g., a primitive DMH invoker form)
				return;
			}
			LambdaForm prep = GetPreparedForm(BasicTypeSignature());
			this.Vmentry = prep.Vmentry;
			// TO DO: Maybe add invokeGeneric, invokeWithArguments
		}

		/// <summary>
		/// Generate optimizable bytecode for this form. </summary>
		internal virtual MemberName CompileToBytecode()
		{
			if (Vmentry != null && IsCompiled)
			{
				return Vmentry; // already compiled somehow
			}
			MethodType invokerType = MethodType();
			assert(Vmentry == null || Vmentry.MethodType.BasicType().Equals(invokerType));
			try
			{
				Vmentry = InvokerBytecodeGenerator.GenerateCustomizedCode(this, invokerType);
				if (TRACE_INTERPRETER)
				{
					TraceInterpreter("compileToBytecode", this);
				}
				IsCompiled = true;
				return Vmentry;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (Error | Exception ex)
			{
				throw newInternalError(this.ToString(), ex);
			}
		}

		private static void ComputeInitialPreparedForms()
		{
			// Find all predefined invokers and associate them with canonical empty lambda forms.
			foreach (MemberName m in MemberName.Factory.GetMethods(typeof(LambdaForm), false, null, null, null))
			{
				if (!m.Static || !m.Package)
				{
					continue;
				}
				MethodType mt = m.MethodType;
				if (mt.ParameterCount() > 0 && mt.ParameterType(0) == typeof(MethodHandle) && m.Name.StartsWith("interpret_"))
				{
					String sig = BasicTypeSignature(mt);
					assert(m.Name.Equals("interpret" + sig.Substring(sig.IndexOf('_'))));
					LambdaForm form = new LambdaForm(sig);
					form.Vmentry = m;
					form = mt.Form().SetCachedLambdaForm(MethodTypeForm.LF_INTERPRET, form);
				}
			}
		}

		// Set this false to disable use of the interpret_L methods defined in this file.
		private const bool USE_PREDEFINED_INTERPRET_METHODS = true;

		// The following are predefined exact invokers.  The system must build
		// a separate invoker for each distinct signature.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object interpret_L(MethodHandle mh) throws Throwable
		internal static Object Interpret_L(MethodHandle mh)
		{
			Object[] av = new Object[] {mh};
			String sig = null;
			assert(ArgumentTypesMatch(sig = "L_L", av));
			Object res = mh.Form.InterpretWithArguments(av);
			assert(ReturnTypesMatch(sig, av, res));
			return res;
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object interpret_L(MethodHandle mh, Object x1) throws Throwable
		internal static Object Interpret_L(MethodHandle mh, Object x1)
		{
			Object[] av = new Object[] {mh, x1};
			String sig = null;
			assert(ArgumentTypesMatch(sig = "LL_L", av));
			Object res = mh.Form.InterpretWithArguments(av);
			assert(ReturnTypesMatch(sig, av, res));
			return res;
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object interpret_L(MethodHandle mh, Object x1, Object x2) throws Throwable
		internal static Object Interpret_L(MethodHandle mh, Object x1, Object x2)
		{
			Object[] av = new Object[] {mh, x1, x2};
			String sig = null;
			assert(ArgumentTypesMatch(sig = "LLL_L", av));
			Object res = mh.Form.InterpretWithArguments(av);
			assert(ReturnTypesMatch(sig, av, res));
			return res;
		}
		private static LambdaForm GetPreparedForm(String sig)
		{
			MethodType mtype = SignatureType(sig);
			LambdaForm prep = mtype.Form().CachedLambdaForm(MethodTypeForm.LF_INTERPRET);
			if (prep != null)
			{
				return prep;
			}
			assert(IsValidSignature(sig));
			prep = new LambdaForm(sig);
			prep.Vmentry = InvokerBytecodeGenerator.GenerateLambdaFormInterpreterEntryPoint(sig);
			return mtype.Form().SetCachedLambdaForm(MethodTypeForm.LF_INTERPRET, prep);
		}

		// The next few routines are called only from assert expressions
		// They verify that the built-in invokers process the correct raw data types.
		private static bool ArgumentTypesMatch(String sig, Object[] av)
		{
			int arity = SignatureArity(sig);
			assert(av.Length == arity) : "av.length == arity: av.length=" + av.Length + ", arity=" + arity;
			assert(av[0] is MethodHandle) : "av[0] not instace of MethodHandle: " + av[0];
			MethodHandle mh = (MethodHandle) av[0];
			MethodType mt = mh.Type();
			assert(mt.ParameterCount() == arity - 1);
			for (int i = 0; i < av.Length; i++)
			{
				Class pt = (i == 0 ? typeof(MethodHandle) : mt.ParameterType(i - 1));
				assert(ValueMatches(basicType(sig.CharAt(i)), pt, av[i]));
			}
			return true;
		}
		private static bool ValueMatches(BasicType tc, Class type, Object x)
		{
			// The following line is needed because (...)void method handles can use non-void invokers
			if (type == typeof(void)) // can drop any kind of value
			{
				tc = V_TYPE;
			}
			Debug.Assert(tc == basicType(type), tc + " == basicType(" + type + ")=" + basicType(type));
			switch (tc.InnerEnumValue())
			{
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.I_TYPE:
				Debug.Assert(CheckInt(type, x), "checkInt(" + type + "," + x + ")");
				break;
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.J_TYPE:
				Debug.Assert(x is Long, "instanceof Long: " + x);
				break;
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.F_TYPE:
				Debug.Assert(x is Float, "instanceof Float: " + x);
				break;
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.D_TYPE:
				Debug.Assert(x is Double, "instanceof Double: " + x);
				break;
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.L_TYPE:
				Debug.Assert(CheckRef(type, x), "checkRef(" + type + "," + x + ")");
				break;
			case java.lang.invoke.LambdaForm.BasicType.InnerEnum.V_TYPE: // allow anything here; will be dropped
				break;
			default:
				assert(false);
			break;
			}
			return true;
		}
		private static bool ReturnTypesMatch(String sig, Object[] av, Object res)
		{
			MethodHandle mh = (MethodHandle) av[0];
			return ValueMatches(SignatureReturn(sig), mh.Type().ReturnType(), res);
		}
		private static bool CheckInt(Class type, Object x)
		{
			assert(x is Integer);
			if (type == typeof(int))
			{
				return true;
			}
			Wrapper w = Wrapper.forBasicType(type);
			assert(w.SubwordOrInt);
			Object x1 = Wrapper.INT.wrap(w.wrap(x));
			return x.Equals(x1);
		}
		private static bool CheckRef(Class type, Object x)
		{
			assert(!type.Primitive);
			if (x == null)
			{
				return true;
			}
			if (type.Interface)
			{
				return true;
			}
			return type.isInstance(x);
		}

		/// <summary>
		/// If the invocation count hits the threshold we spin bytecodes and call that subsequently. </summary>
		private static readonly int COMPILE_THRESHOLD;
		static LambdaForm()
		{
			COMPILE_THRESHOLD = System.Math.Max(-1, MethodHandleStatics.COMPILE_THRESHOLD);
			foreach (BasicType type in BasicType.ARG_TYPES)
			{
				int ord = type.ordinal();
				for (int i = 0; i < INTERNED_ARGUMENTS[ord].Length; i++)
				{
					INTERNED_ARGUMENTS[ord][i] = new Name(i, type);
				}
			}
			if (debugEnabled())
			{
				DEBUG_NAME_COUNTERS = new Dictionary<>();
			}
			else
			{
				DEBUG_NAME_COUNTERS = null;
			}
			CreateIdentityForms();
			if (USE_PREDEFINED_INTERPRET_METHODS)
			{
				ComputeInitialPreparedForms();
			}
			NamedFunction.InitializeInvokers();
		}
		private int InvocationCounter = 0;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden @DontInline Object interpretWithArguments(Object... argumentValues) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal virtual Object InterpretWithArguments(params Object[] argumentValues)
		/// <summary>
		/// Interpretively invoke this form on the given arguments. </summary>
		{
			if (TRACE_INTERPRETER)
			{
				return InterpretWithArgumentsTracing(argumentValues);
			}
			CheckInvocationCounter();
			assert(ArityCheck(argumentValues));
			Object[] values = Arrays.CopyOf(argumentValues, Names.Length);
			for (int i = argumentValues.Length; i < values.Length; i++)
			{
				values[i] = InterpretName(Names[i], values);
			}
			Object rv = (Result < 0) ? null : values[Result];
			assert(ResultCheck(argumentValues, rv));
			return rv;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden @DontInline Object interpretName(Name name, Object[] values) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal virtual Object InterpretName(Name name, Object[] values)
		/// <summary>
		/// Evaluate a single Name within this form, applying its function to its arguments. </summary>
		{
			if (TRACE_INTERPRETER)
			{
				TraceInterpreter("| interpretName", name.DebugString(), (Object[]) null);
			}
			Object[] arguments = Arrays.CopyOf(name.Arguments, name.Arguments.Length, typeof(Object[]));
			for (int i = 0; i < arguments.Length; i++)
			{
				Object a = arguments[i];
				if (a is Name)
				{
					int i2 = ((Name)a).Index();
					assert(Names[i2] == a);
					a = values[i2];
					arguments[i] = a;
				}
			}
			return name.Function.InvokeWithArguments(arguments);
		}

		private void CheckInvocationCounter()
		{
			if (COMPILE_THRESHOLD != 0 && InvocationCounter < COMPILE_THRESHOLD)
			{
				InvocationCounter++; // benign race
				if (InvocationCounter >= COMPILE_THRESHOLD)
				{
					// Replace vmentry with a bytecode version of this LF.
					CompileToBytecode();
				}
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object interpretWithArgumentsTracing(Object... argumentValues) throws Throwable
		internal virtual Object InterpretWithArgumentsTracing(params Object[] argumentValues)
		{
			TraceInterpreter("[ interpretWithArguments", this, argumentValues);
			if (InvocationCounter < COMPILE_THRESHOLD)
			{
				int ctr = InvocationCounter++; // benign race
				TraceInterpreter("| invocationCounter", ctr);
				if (InvocationCounter >= COMPILE_THRESHOLD)
				{
					CompileToBytecode();
				}
			}
			Object rval;
			try
			{
				assert(ArityCheck(argumentValues));
				Object[] values = Arrays.CopyOf(argumentValues, Names.Length);
				for (int i = argumentValues.Length; i < values.Length; i++)
				{
					values[i] = InterpretName(Names[i], values);
				}
				rval = (Result < 0) ? null : values[Result];
			}
			catch (Throwable ex)
			{
				TraceInterpreter("] throw =>", ex);
				throw ex;
			}
			TraceInterpreter("] return =>", rval);
			return rval;
		}

		internal static void TraceInterpreter(String @event, Object obj, params Object[] args)
		{
			if (TRACE_INTERPRETER)
			{
				System.Console.WriteLine("LFI: " + @event + " " + (obj != null ? obj : "") + (args != null && args.Length != 0 ? Arrays.AsList(args) : ""));
			}
		}
		internal static void TraceInterpreter(String @event, Object obj)
		{
			TraceInterpreter(@event, obj, (Object[])null);
		}
		private bool ArityCheck(Object[] argumentValues)
		{
			assert(argumentValues.Length == Arity_Renamed) : Arity_Renamed + "!=" + Arrays.AsList(argumentValues) + ".length";
			// also check that the leading (receiver) argument is somehow bound to this LF:
			assert(argumentValues[0] is MethodHandle) : "not MH: " + argumentValues[0];
			MethodHandle mh = (MethodHandle) argumentValues[0];
			assert(mh.InternalForm() == this);
			// note:  argument #0 could also be an interface wrapper, in the future
			ArgumentTypesMatch(BasicTypeSignature(), argumentValues);
			return true;
		}
		private bool ResultCheck(Object[] argumentValues, Object result)
		{
			MethodHandle mh = (MethodHandle) argumentValues[0];
			MethodType mt = mh.Type();
			assert(ValueMatches(ReturnType(), mt.ReturnType(), result));
			return true;
		}

		private bool Empty
		{
			get
			{
				if (Result < 0)
				{
					return (Names.Length == Arity_Renamed);
				}
				else if (Result == Arity_Renamed && Names.Length == Arity_Renamed + 1)
				{
					return Names[Arity_Renamed].ConstantZero;
				}
				else
				{
					return false;
				}
			}
		}

		public override String ToString()
		{
			StringBuilder buf = new StringBuilder(DebugName + "=Lambda(");
			for (int i = 0; i < Names.Length; i++)
			{
				if (i == Arity_Renamed)
				{
					buf.Append(")=>{");
				}
				Name n = Names[i];
				if (i >= Arity_Renamed)
				{
					buf.Append("\n    ");
				}
				buf.Append(n.ParamString());
				if (i < Arity_Renamed)
				{
					if (i + 1 < Arity_Renamed)
					{
						buf.Append(",");
					}
					continue;
				}
				buf.Append("=").Append(n.ExprString());
				buf.Append(";");
			}
			if (Arity_Renamed == Names.Length)
			{
				buf.Append(")=>{");
			}
			buf.Append(Result < 0 ? "void" : Names[Result]).Append("}");
			if (TRACE_INTERPRETER)
			{
				// Extra verbosity:
				buf.Append(":").Append(BasicTypeSignature());
				buf.Append("/").Append(Vmentry);
			}
			return buf.ToString();
		}

		public override bool Equals(Object obj)
		{
			return obj is LambdaForm && Equals((LambdaForm)obj);
		}
		public virtual bool Equals(LambdaForm that)
		{
			if (this.Result != that.Result)
			{
				return false;
			}
			return Arrays.Equals(this.Names, that.Names);
		}
		public override int HashCode()
		{
			return Result + 31 * Arrays.HashCode(Names);
		}
		internal virtual LambdaFormEditor Editor()
		{
			return LambdaFormEditor.LambdaFormEditor(this);
		}

		internal virtual bool Contains(Name name)
		{
			int pos = name.Index();
			if (pos >= 0)
			{
				return pos < Names.Length && name.Equals(Names[pos]);
			}
			for (int i = Arity_Renamed; i < Names.Length; i++)
			{
				if (name.Equals(Names[i]))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual LambdaForm AddArguments(int pos, params BasicType[] types)
		{
			// names array has MH in slot 0; skip it.
			int argpos = pos + 1;
			assert(argpos <= Arity_Renamed);
			int length = Names.Length;
			int inTypes = types.Length;
			Name[] names2 = Arrays.CopyOf(Names, length + inTypes);
			int arity2 = Arity_Renamed + inTypes;
			int result2 = Result;
			if (result2 >= argpos)
			{
				result2 += inTypes;
			}
			// Note:  The LF constructor will rename names2[argpos...].
			// Make space for new arguments (shift temporaries).
			System.Array.Copy(Names, argpos, names2, argpos + inTypes, length - argpos);
			for (int i = 0; i < inTypes; i++)
			{
				names2[argpos + i] = new Name(types[i]);
			}
			return new LambdaForm(DebugName, arity2, names2, result2);
		}

		internal virtual LambdaForm AddArguments(int pos, IList<Class> types)
		{
			return AddArguments(pos, basicTypes(types));
		}

		internal virtual LambdaForm PermuteArguments(int skip, int[] reorder, BasicType[] types)
		{
			// Note:  When inArg = reorder[outArg], outArg is fed by a copy of inArg.
			// The types are the types of the new (incoming) arguments.
			int length = Names.Length;
			int inTypes = types.Length;
			int outArgs = reorder.Length;
			assert(skip + outArgs == Arity_Renamed);
			assert(PermutedTypesMatch(reorder, types, Names, skip));
			int pos = 0;
			// skip trivial first part of reordering:
			while (pos < outArgs && reorder[pos] == pos)
			{
				pos += 1;
			}
			Name[] names2 = new Name[length - outArgs + inTypes];
			System.Array.Copy(Names, 0, names2, 0, skip + pos);
			// copy the body:
			int bodyLength = length - Arity_Renamed;
			System.Array.Copy(Names, skip + outArgs, names2, skip + inTypes, bodyLength);
			int arity2 = names2.Length - bodyLength;
			int result2 = Result;
			if (result2 >= 0)
			{
				if (result2 < skip + outArgs)
				{
					// return the corresponding inArg
					result2 = reorder[result2 - skip];
				}
				else
				{
					result2 = result2 - outArgs + inTypes;
				}
			}
			// rework names in the body:
			for (int j = pos; j < outArgs; j++)
			{
				Name n = Names[skip + j];
				int i = reorder[j];
				// replace names[skip+j] by names2[skip+i]
				Name n2 = names2[skip + i];
				if (n2 == null)
				{
					names2[skip + i] = n2 = new Name(types[i]);
				}
				else
				{
					assert(n2.Type_Renamed == types[i]);
				}
				for (int k = arity2; k < names2.Length; k++)
				{
					names2[k] = names2[k].ReplaceName(n, n2);
				}
			}
			// some names are unused, but must be filled in
			for (int i = skip + pos; i < arity2; i++)
			{
				if (names2[i] == null)
				{
					names2[i] = Argument(i, types[i - skip]);
				}
			}
			for (int j = Arity_Renamed; j < Names.Length; j++)
			{
				int i = j - Arity_Renamed + arity2;
				// replace names2[i] by names[j]
				Name n = Names[j];
				Name n2 = names2[i];
				if (n != n2)
				{
					for (int k = i + 1; k < names2.Length; k++)
					{
						names2[k] = names2[k].ReplaceName(n, n2);
					}
				}
			}
			return new LambdaForm(DebugName, arity2, names2, result2);
		}

		internal static bool PermutedTypesMatch(int[] reorder, BasicType[] types, Name[] names, int skip)
		{
			int inTypes = types.Length;
			int outArgs = reorder.Length;
			for (int i = 0; i < outArgs; i++)
			{
				assert(names[skip + i].Param);
				assert(names[skip + i].Type_Renamed == types[reorder[i]]);
			}
			return true;
		}

		internal class NamedFunction
		{
			internal readonly MemberName Member_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable MethodHandle resolvedHandle;
			internal MethodHandle ResolvedHandle_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable MethodHandle invoker;
			internal MethodHandle Invoker_Renamed;

			internal NamedFunction(MethodHandle resolvedHandle) : this(resolvedHandle.InternalMemberName(), resolvedHandle)
			{
			}
			internal NamedFunction(MemberName member, MethodHandle resolvedHandle)
			{
				this.Member_Renamed = member;
				this.ResolvedHandle_Renamed = resolvedHandle;
				 // The following assert is almost always correct, but will fail for corner cases, such as PrivateInvokeTest.
				 //assert(!isInvokeBasic());
			}
			internal NamedFunction(MethodType basicInvokerType)
			{
				assert(basicInvokerType == basicInvokerType.BasicType()) : basicInvokerType;
				if (basicInvokerType.ParameterSlotCount() < MethodType.MAX_MH_INVOKER_ARITY)
				{
					this.ResolvedHandle_Renamed = basicInvokerType.Invokers().BasicInvoker();
					this.Member_Renamed = ResolvedHandle_Renamed.InternalMemberName();
				}
				else
				{
					// necessary to pass BigArityTest
					this.Member_Renamed = Invokers.InvokeBasicMethod(basicInvokerType);
				}
				assert(InvokeBasic);
			}

			internal virtual bool InvokeBasic
			{
				get
				{
					return Member_Renamed != null && Member_Renamed.MethodHandleInvoke && "invokeBasic".Equals(Member_Renamed.Name);
				}
			}

			// The next 3 constructors are used to break circular dependencies on MH.invokeStatic, etc.
			// Any LambdaForm containing such a member is not interpretable.
			// This is OK, since all such LFs are prepared with special primitive vmentry points.
			// And even without the resolvedHandle, the name can still be compiled and optimized.
			internal NamedFunction(Method method) : this(new MemberName(method))
			{
			}
			internal NamedFunction(Field field) : this(new MemberName(field))
			{
			}
			internal NamedFunction(MemberName member)
			{
				this.Member_Renamed = member;
				this.ResolvedHandle_Renamed = null;
			}

			internal virtual MethodHandle ResolvedHandle()
			{
				if (ResolvedHandle_Renamed == null)
				{
					Resolve();
				}
				return ResolvedHandle_Renamed;
			}

			internal virtual void Resolve()
			{
				ResolvedHandle_Renamed = DirectMethodHandle.Make(Member_Renamed);
			}

			public override bool Equals(Object other)
			{
				if (this == other)
				{
					return true;
				}
				if (other == null)
				{
					return false;
				}
				if (!(other is NamedFunction))
				{
					return false;
				}
				NamedFunction that = (NamedFunction) other;
				return this.Member_Renamed != null && this.Member_Renamed.Equals(that.Member_Renamed);
			}

			public override int HashCode()
			{
				if (Member_Renamed != null)
				{
					return Member_Renamed.HashCode();
				}
				return base.HashCode();
			}

			// Put the predefined NamedFunction invokers into the table.
			internal static void InitializeInvokers()
			{
				foreach (MemberName m in MemberName.Factory.GetMethods(typeof(NamedFunction), false, null, null, null))
				{
					if (!m.Static || !m.Package)
					{
						continue;
					}
					MethodType type = m.MethodType;
					if (type.Equals(INVOKER_METHOD_TYPE) && m.Name.StartsWith("invoke_"))
					{
						String sig = m.Name.Substring("invoke_".Length);
						int arity = LambdaForm.SignatureArity(sig);
						MethodType srcType = MethodType.GenericMethodType(arity);
						if (LambdaForm.SignatureReturn(sig) == V_TYPE)
						{
							srcType = srcType.ChangeReturnType(typeof(void));
						}
						MethodTypeForm typeForm = srcType.Form();
						typeForm.SetCachedMethodHandle(MethodTypeForm.MH_NF_INV, DirectMethodHandle.Make(m));
					}
				}
			}

			// The following are predefined NamedFunction invokers.  The system must build
			// a separate invoker for each distinct signature.
			/// <summary>
			/// void return type invokers. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke__V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke__V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(0, typeof(void), mh, a));
				mh.invokeBasic();
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_L_V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_L_V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(1, typeof(void), mh, a));
				mh.invokeBasic(a[0]);
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LL_V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LL_V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(2, typeof(void), mh, a));
				mh.invokeBasic(a[0], a[1]);
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLL_V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLL_V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(3, typeof(void), mh, a));
				mh.invokeBasic(a[0], a[1], a[2]);
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLLL_V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLLL_V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(4, typeof(void), mh, a));
				mh.invokeBasic(a[0], a[1], a[2], a[3]);
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLLLL_V(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLLLL_V(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(5, typeof(void), mh, a));
				mh.invokeBasic(a[0], a[1], a[2], a[3], a[4]);
				return null;
			}
			/// <summary>
			/// Object return type invokers. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke__L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke__L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(0, mh, a));
				return mh.invokeBasic();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_L_L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_L_L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(1, mh, a));
				return mh.invokeBasic(a[0]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LL_L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LL_L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(2, mh, a));
				return mh.invokeBasic(a[0], a[1]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLL_L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLL_L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(3, mh, a));
				return mh.invokeBasic(a[0], a[1], a[2]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLLL_L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLLL_L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(4, mh, a));
				return mh.invokeBasic(a[0], a[1], a[2], a[3]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden static Object invoke_LLLLL_L(MethodHandle mh, Object[] a) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal static Object Invoke_LLLLL_L(MethodHandle mh, Object[] a)
			{
				assert(ArityCheck(5, mh, a));
				return mh.invokeBasic(a[0], a[1], a[2], a[3], a[4]);
			}
			internal static bool ArityCheck(int arity, MethodHandle mh, Object[] a)
			{
				return ArityCheck(arity, typeof(Object), mh, a);
			}
			internal static bool ArityCheck(int arity, Class rtype, MethodHandle mh, Object[] a)
			{
				assert(a.Length == arity) : Arrays.asList(a.Length, arity);
				assert(mh.Type().BasicType() == MethodType.GenericMethodType(arity).ChangeReturnType(rtype)) : Arrays.asList(mh, rtype, arity);
				MemberName member = mh.InternalMemberName();
				if (member != null && member.Name.Equals("invokeBasic") && member.MethodHandleInvoke)
				{
					assert(arity > 0);
					assert(a[0] is MethodHandle);
					MethodHandle mh2 = (MethodHandle) a[0];
					assert(mh2.Type().BasicType() == MethodType.GenericMethodType(arity - 1).ChangeReturnType(rtype)) : Arrays.asList(member, mh2, rtype, arity);
				}
				return true;
			}

			internal static readonly MethodType INVOKER_METHOD_TYPE = MethodType.MethodType(typeof(Object), typeof(MethodHandle), typeof(Object[]));

			internal static MethodHandle ComputeInvoker(MethodTypeForm typeForm)
			{
				typeForm = typeForm.BasicType().Form(); // normalize to basic type
				MethodHandle mh = typeForm.CachedMethodHandle(MethodTypeForm.MH_NF_INV);
				if (mh != null)
				{
					return mh;
				}
				MemberName invoker = InvokerBytecodeGenerator.GenerateNamedFunctionInvoker(typeForm); // this could take a while
				mh = DirectMethodHandle.Make(invoker);
				MethodHandle mh2 = typeForm.CachedMethodHandle(MethodTypeForm.MH_NF_INV);
				if (mh2 != null) // benign race
				{
					return mh2;
				}
				if (!mh.Type().Equals(INVOKER_METHOD_TYPE))
				{
					throw newInternalError(mh.DebugString());
				}
				return typeForm.SetCachedMethodHandle(MethodTypeForm.MH_NF_INV, mh);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden Object invokeWithArguments(Object... arguments) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal virtual Object InvokeWithArguments(params Object[] arguments)
			{
				// If we have a cached invoker, call it right away.
				// NOTE: The invoker always returns a reference value.
				if (TRACE_INTERPRETER)
				{
					return InvokeWithArgumentsTracing(arguments);
				}
				assert(CheckArgumentTypes(arguments, MethodType()));
				return Invoker().invokeBasic(ResolvedHandle(), arguments);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Hidden Object invokeWithArgumentsTracing(Object[] arguments) throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			internal virtual Object InvokeWithArgumentsTracing(Object[] arguments)
			{
				Object rval;
				try
				{
					TraceInterpreter("[ call", this, arguments);
					if (Invoker_Renamed == null)
					{
						TraceInterpreter("| getInvoker", this);
						Invoker();
					}
					if (ResolvedHandle_Renamed == null)
					{
						TraceInterpreter("| resolve", this);
						ResolvedHandle();
					}
					assert(CheckArgumentTypes(arguments, MethodType()));
					rval = Invoker().invokeBasic(ResolvedHandle(), arguments);
				}
				catch (Throwable ex)
				{
					TraceInterpreter("] throw =>", ex);
					throw ex;
				}
				TraceInterpreter("] return =>", rval);
				return rval;
			}

			internal virtual MethodHandle Invoker()
			{
				if (Invoker_Renamed != null)
				{
					return Invoker_Renamed;
				}
				// Get an invoker and cache it.
				return Invoker_Renamed = ComputeInvoker(MethodType().Form());
			}

			internal static bool CheckArgumentTypes(Object[] arguments, MethodType methodType)
			{
				if (true) // FIXME
				{
					return true;
				}
				MethodType dstType = methodType.Form().ErasedType();
				MethodType srcType = dstType.BasicType().Wrap();
				Class[] ptypes = new Class[arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
				{
					Object arg = arguments[i];
					Class ptype = arg == null ? typeof(Object) : arg.GetType();
					// If the dest. type is a primitive we keep the
					// argument type.
					ptypes[i] = dstType.ParameterType(i).Primitive ? ptype : typeof(Object);
				}
				MethodType argType = MethodType.MethodType(srcType.ReturnType(), ptypes).Wrap();
				assert(argType.IsConvertibleTo(srcType)) : "wrong argument types: cannot convert " + argType + " to " + srcType;
				return true;
			}

			internal virtual MethodType MethodType()
			{
				if (ResolvedHandle_Renamed != null)
				{
					return ResolvedHandle_Renamed.Type();
				}
				else
				{
					// only for certain internal LFs during bootstrapping
					return Member_Renamed.InvocationType;
				}
			}

			internal virtual MemberName Member()
			{
				assert(AssertMemberIsConsistent());
				return Member_Renamed;
			}

			// Called only from assert.
			internal virtual bool AssertMemberIsConsistent()
			{
				if (ResolvedHandle_Renamed is DirectMethodHandle)
				{
					MemberName m = ResolvedHandle_Renamed.InternalMemberName();
					assert(m.Equals(Member_Renamed));
				}
				return true;
			}

			internal virtual Class MemberDeclaringClassOrNull()
			{
				return (Member_Renamed == null) ? null : Member_Renamed.DeclaringClass;
			}

			internal virtual BasicType ReturnType()
			{
				return basicType(MethodType().ReturnType());
			}

			internal virtual BasicType ParameterType(int n)
			{
				return basicType(MethodType().ParameterType(n));
			}

			internal virtual int Arity()
			{
				return MethodType().ParameterCount();
			}

			public override String ToString()
			{
				if (Member_Renamed == null)
				{
					return Convert.ToString(ResolvedHandle_Renamed);
				}
				return Member_Renamed.DeclaringClass.SimpleName + "." + Member_Renamed.Name;
			}

			public virtual bool Identity
			{
				get
				{
					return this.Equals(Identity(ReturnType()));
				}
			}

			public virtual bool ConstantZero
			{
				get
				{
					return this.Equals(ConstantZero(ReturnType()));
				}
			}

			public virtual MethodHandleImpl.Intrinsic IntrinsicName()
			{
				return ResolvedHandle_Renamed == null ? MethodHandleImpl.Intrinsic.NONE : ResolvedHandle_Renamed.IntrinsicName();
			}
		}

		public static String BasicTypeSignature(MethodType type)
		{
			char[] sig = new char[type.ParameterCount() + 2];
			int sigp = 0;
			foreach (Class pt in type.ParameterList())
			{
				sig[sigp++] = basicTypeChar(pt);
			}
			sig[sigp++] = '_';
			sig[sigp++] = basicTypeChar(type.ReturnType());
			assert(sigp == sig.Length);
			return Convert.ToString(sig);
		}
		public static String ShortenSignature(String signature)
		{
			// Hack to make signatures more readable when they show up in method names.
			const int NO_CHAR = -1, MIN_RUN = 3;
			int c0 , c1 = NO_CHAR, c1reps = 0;
			StringBuilder buf = null;
			int len = signature.Length();
			if (len < MIN_RUN)
			{
				return signature;
			}
			for (int i = 0; i <= len; i++)
			{
				// shift in the next char:
				c0 = c1;
				c1 = (i == len ? NO_CHAR : signature.CharAt(i));
				if (c1 == c0)
				{
					++c1reps;
					continue;
				}
				// shift in the next count:
				int c0reps = c1reps;
				c1reps = 1;
				// end of a  character run
				if (c0reps < MIN_RUN)
				{
					if (buf != null)
					{
						while (--c0reps >= 0)
						{
							buf.Append((char)c0);
						}
					}
					continue;
				}
				// found three or more in a row
				if (buf == null)
				{
					buf = (new StringBuilder()).Append(signature, 0, i - c0reps);
				}
				buf.Append((char)c0).Append(c0reps);
			}
			return (buf == null) ? signature : buf.ToString();
		}

		internal sealed class Name
		{
			internal readonly BasicType Type_Renamed;
			internal short Index_Renamed;
			internal readonly NamedFunction Function;
			internal readonly Object Constraint; // additional type information, if not null
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable final Object[] arguments;
			internal readonly Object[] Arguments;

			internal Name(int index, BasicType type, NamedFunction function, Object[] arguments)
			{
				this.Index_Renamed = (short)index;
				this.Type_Renamed = type;
				this.Function = function;
				this.Arguments = arguments;
				this.Constraint = null;
				assert(this.Index_Renamed == index);
			}
			internal Name(Name that, Object constraint)
			{
				this.Index_Renamed = that.Index_Renamed;
				this.Type_Renamed = that.Type_Renamed;
				this.Function = that.Function;
				this.Arguments = that.Arguments;
				this.Constraint = constraint;
				assert(constraint == null || Param); // only params have constraints
				assert(constraint == null || constraint is BoundMethodHandle.SpeciesData || constraint is Class);
			}
			internal Name(MethodHandle function, params Object[] arguments) : this(new NamedFunction(function), arguments)
			{
			}
			internal Name(MethodType functionType, params Object[] arguments) : this(new NamedFunction(functionType), arguments)
			{
				assert(arguments[0] is Name && ((Name)arguments[0]).Type_Renamed == L_TYPE);
			}
			internal Name(MemberName function, params Object[] arguments) : this(new NamedFunction(function), arguments)
			{
			}
			internal Name(NamedFunction function, params Object[] arguments) : this(-1, function.ReturnType(), function, arguments = Arrays.CopyOf(arguments, arguments.Length, typeof(Object[])))
			{
				assert(arguments.Length == function.Arity()) : "arity mismatch: arguments.length=" + arguments.Length + " == function.arity()=" + function.Arity() + " in " + DebugString();
				for (int i = 0; i < arguments.Length; i++)
				{
					assert(TypesMatch(function.ParameterType(i), arguments[i])) : "types don't match: function.parameterType(" + i + ")=" + function.ParameterType(i) + ", arguments[" + i + "]=" + arguments[i] + " in " + DebugString();
				}
			}
			/// <summary>
			/// Create a raw parameter of the given type, with an expected index. </summary>
			internal Name(int index, BasicType type) : this(index, type, null, null)
			{
			}
			/// <summary>
			/// Create a raw parameter of the given type. </summary>
			internal Name(BasicType type) : this(-1, type)
			{
			}

			internal BasicType Type()
			{
				return Type_Renamed;
			}
			internal int Index()
			{
				return Index_Renamed;
			}
			internal bool InitIndex(int i)
			{
				if (Index_Renamed != i)
				{
					if (Index_Renamed != -1)
					{
						return false;
					}
					Index_Renamed = (short)i;
				}
				return true;
			}
			internal char TypeChar()
			{
				return Type_Renamed.btChar;
			}

			internal void Resolve()
			{
				if (Function != null)
				{
					Function.Resolve();
				}
			}

			internal Name NewIndex(int i)
			{
				if (InitIndex(i))
				{
					return this;
				}
				return CloneWithIndex(i);
			}
			internal Name CloneWithIndex(int i)
			{
				Object[] newArguments = (Arguments == null) ? null : Arguments.clone();
				return (new Name(i, Type_Renamed, Function, newArguments)).WithConstraint(Constraint);
			}
			internal Name WithConstraint(Object constraint)
			{
				if (constraint == this.Constraint)
				{
					return this;
				}
				return new Name(this, constraint);
			}
			internal Name ReplaceName(Name oldName, Name newName) // FIXME: use replaceNames uniformly
			{
				if (oldName == newName)
				{
					return this;
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LocalVariableHidesMemberVariable") Object[] arguments = this.arguments;
				Object[] arguments = this.Arguments;
				if (arguments == null)
				{
					return this;
				}
				bool replaced = false;
				for (int j = 0; j < arguments.Length; j++)
				{
					if (arguments[j] == oldName)
					{
						if (!replaced)
						{
							replaced = true;
							arguments = arguments.clone();
						}
						arguments[j] = newName;
					}
				}
				if (!replaced)
				{
					return this;
				}
				return new Name(Function, arguments);
			}
			/// <summary>
			/// In the arguments of this Name, replace oldNames[i] pairwise by newNames[i].
			///  Limit such replacements to {@code start<=i<end}.  Return possibly changed self.
			/// </summary>
			internal Name ReplaceNames(Name[] oldNames, Name[] newNames, int start, int end)
			{
				if (start >= end)
				{
					return this;
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LocalVariableHidesMemberVariable") Object[] arguments = this.arguments;
				Object[] arguments = this.Arguments;
				bool replaced = false;
				for (int j = 0; j < arguments.Length; j++)
				{
					if (arguments[j] is Name)
					{
						Name n = (Name) arguments[j];
						int check = n.Index_Renamed;
						// harmless check to see if the thing is already in newNames:
						if (check >= 0 && check < newNames.Length && n == newNames[check])
						{
							goto eachArgContinue;
						}
						// n might not have the correct index: n != oldNames[n.index].
						for (int i = start; i < end; i++)
						{
							if (n == oldNames[i])
							{
								if (n == newNames[i])
								{
									goto eachArgContinue;
								}
								if (!replaced)
								{
									replaced = true;
									arguments = arguments.clone();
								}
								arguments[j] = newNames[i];
								goto eachArgContinue;
							}
						}
					}
				eachArgContinue:;
				}
			eachArgBreak:
				if (!replaced)
				{
					return this;
				}
				return new Name(Function, arguments);
			}
			internal void InternArguments()
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("LocalVariableHidesMemberVariable") Object[] arguments = this.arguments;
				Object[] arguments = this.Arguments;
				for (int j = 0; j < arguments.Length; j++)
				{
					if (arguments[j] is Name)
					{
						Name n = (Name) arguments[j];
						if (n.Param && n.Index_Renamed < INTERNED_ARGUMENT_LIMIT)
						{
							arguments[j] = InternArgument(n);
						}
					}
				}
			}
			internal bool Param
			{
				get
				{
					return Function == null;
				}
			}
			internal bool ConstantZero
			{
				get
				{
					return !Param && Arguments.Length == 0 && Function.ConstantZero;
				}
			}

			public override String ToString()
			{
				return (Param?"a":"t") + (Index_Renamed >= 0 ? Index_Renamed : System.identityHashCode(this)) + ":" + TypeChar();
			}
			public String DebugString()
			{
				String s = ParamString();
				return (Function == null) ? s : s + "=" + ExprString();
			}
			public String ParamString()
			{
				String s = ToString();
				Object c = Constraint;
				if (c == null)
				{
					return s;
				}
				if (c is Class)
				{
					c = ((Class)c).SimpleName;
				}
				return s + "/" + c;
			}
			public String ExprString()
			{
				if (Function == null)
				{
					return ToString();
				}
				StringBuilder buf = new StringBuilder(Function.ToString());
				buf.Append("(");
				String cma = "";
				foreach (Object a in Arguments)
				{
					buf.Append(cma);
					cma = ",";
					if (a is Name || a is Integer)
					{
						buf.Append(a);
					}
					else
					{
						buf.Append("(").Append(a).Append(")");
					}
				}
				buf.Append(")");
				return buf.ToString();
			}

			internal static bool TypesMatch(BasicType parameterType, Object @object)
			{
				if (@object is Name)
				{
					return ((Name)@object).Type_Renamed == parameterType;
				}
				switch (parameterType.InnerEnumValue())
				{
					case java.lang.invoke.LambdaForm.BasicType.InnerEnum.I_TYPE:
						return @object is Integer;
					case java.lang.invoke.LambdaForm.BasicType.InnerEnum.J_TYPE:
						return @object is Long;
					case java.lang.invoke.LambdaForm.BasicType.InnerEnum.F_TYPE:
						return @object is Float;
					case java.lang.invoke.LambdaForm.BasicType.InnerEnum.D_TYPE:
						return @object is Double;
				}
				assert(parameterType == L_TYPE);
				return true;
			}

			/// <summary>
			/// Return the index of the last occurrence of n in the argument array.
			///  Return -1 if the name is not used.
			/// </summary>
			internal int LastUseIndex(Name n)
			{
				if (Arguments == null)
				{
					return -1;
				}
				for (int i = Arguments.Length; --i >= 0;)
				{
					if (Arguments[i] == n)
					{
						return i;
					}
				}
				return -1;
			}

			/// <summary>
			/// Return the number of occurrences of n in the argument array.
			///  Return 0 if the name is not used.
			/// </summary>
			internal int UseCount(Name n)
			{
				if (Arguments == null)
				{
					return 0;
				}
				int count = 0;
				for (int i = Arguments.Length; --i >= 0;)
				{
					if (Arguments[i] == n)
					{
						++count;
					}
				}
				return count;
			}

			internal bool Contains(Name n)
			{
				return this == n || LastUseIndex(n) >= 0;
			}

			public bool Equals(Name that)
			{
				if (this == that)
				{
					return true;
				}
				if (Param)
				{
					// each parameter is a unique atom
					return false; // this != that
				}
				return this.Type_Renamed == that.Type_Renamed && this.Function.Equals(that.Function) && Arrays.Equals(this.Arguments, that.Arguments);
					//this.index == that.index &&
			}
			public override bool Equals(Object x)
			{
				return x is Name && Equals((Name)x);
			}
			public override int HashCode()
			{
				if (Param)
				{
					return Index_Renamed | (Type_Renamed.ordinal() << 8);
				}
				return Function.HashCode() ^ Arrays.HashCode(Arguments);
			}
		}

		/// <summary>
		/// Return the index of the last name which contains n as an argument.
		///  Return -1 if the name is not used.  Return names.length if it is the return value.
		/// </summary>
		internal virtual int LastUseIndex(Name n)
		{
			int ni = n.Index_Renamed, nmax = Names.Length;
			assert(Names[ni] == n);
			if (Result == ni) // live all the way beyond the end
			{
				return nmax;
			}
			for (int i = nmax; --i > ni;)
			{
				if (Names[i].LastUseIndex(n) >= 0)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Return the number of times n is used as an argument or return value. </summary>
		internal virtual int UseCount(Name n)
		{
			int ni = n.Index_Renamed, nmax = Names.Length;
			int end = LastUseIndex(n);
			if (end < 0)
			{
				return 0;
			}
			int count = 0;
			if (end == nmax)
			{
				count++;
				end--;
			}
			int beg = n.Index() + 1;
			if (beg < Arity_Renamed)
			{
				beg = Arity_Renamed;
			}
			for (int i = beg; i <= end; i++)
			{
				count += Names[i].UseCount(n);
			}
			return count;
		}

		internal static Name Argument(int which, char type)
		{
			return Argument(which, basicType(type));
		}
		internal static Name Argument(int which, BasicType type)
		{
			if (which >= INTERNED_ARGUMENT_LIMIT)
			{
				return new Name(which, type);
			}
			return INTERNED_ARGUMENTS[type.ordinal()][which];
		}
		internal static Name InternArgument(Name n)
		{
			assert(n.Param) : "not param: " + n;
			assert(n.Index_Renamed < INTERNED_ARGUMENT_LIMIT);
			if (n.Constraint != null)
			{
				return n;
			}
			return Argument(n.Index_Renamed, n.Type_Renamed);
		}
		internal static Name[] Arguments(int extra, String types)
		{
			int length = types.Length();
			Name[] names = new Name[length + extra];
			for (int i = 0; i < length; i++)
			{
				names[i] = Argument(i, types.CharAt(i));
			}
			return names;
		}
		internal static Name[] Arguments(int extra, params char[] types)
		{
			int length = types.Length;
			Name[] names = new Name[length + extra];
			for (int i = 0; i < length; i++)
			{
				names[i] = Argument(i, types[i]);
			}
			return names;
		}
		internal static Name[] Arguments(int extra, IList<Class> types)
		{
			int length = types.Count;
			Name[] names = new Name[length + extra];
			for (int i = 0; i < length; i++)
			{
				names[i] = Argument(i, basicType(types[i]));
			}
			return names;
		}
		internal static Name[] Arguments(int extra, params Class[] types)
		{
			int length = types.Length;
			Name[] names = new Name[length + extra];
			for (int i = 0; i < length; i++)
			{
				names[i] = Argument(i, basicType(types[i]));
			}
			return names;
		}
		internal static Name[] Arguments(int extra, MethodType types)
		{
			int length = types.ParameterCount();
			Name[] names = new Name[length + extra];
			for (int i = 0; i < length; i++)
			{
				names[i] = Argument(i, basicType(types.ParameterType(i)));
			}
			return names;
		}
		internal const int INTERNED_ARGUMENT_LIMIT = 10;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: private static readonly Name[][] INTERNED_ARGUMENTS = new Name[ARG_TYPE_LIMIT][INTERNED_ARGUMENT_LIMIT];
		private static readonly Name[][] INTERNED_ARGUMENTS = RectangularArrays.ReturnRectangularNameArray(ARG_TYPE_LIMIT, INTERNED_ARGUMENT_LIMIT);

		private static readonly MemberName.Factory IMPL_NAMES = MemberName.Factory;

		internal static LambdaForm IdentityForm(BasicType type)
		{
			return LF_identityForm[type.ordinal()];
		}
		internal static LambdaForm ZeroForm(BasicType type)
		{
			return LF_zeroForm[type.ordinal()];
		}
		internal static NamedFunction Identity(BasicType type)
		{
			return NF_identity[type.ordinal()];
		}
		internal static NamedFunction ConstantZero(BasicType type)
		{
			return NF_zero[type.ordinal()];
		}
		private static readonly LambdaForm[] LF_identityForm = new LambdaForm[TYPE_LIMIT];
		private static readonly LambdaForm[] LF_zeroForm = new LambdaForm[TYPE_LIMIT];
		private static readonly NamedFunction[] NF_identity = new NamedFunction[TYPE_LIMIT];
		private static readonly NamedFunction[] NF_zero = new NamedFunction[TYPE_LIMIT];
		private static void CreateIdentityForms()
		{
			foreach (BasicType type in BasicType.ALL_TYPES)
			{
				int ord = type.ordinal();
				char btChar = type.basicTypeChar();
				bool isVoid = (type == V_TYPE);
				Class btClass = type.btClass;
				MethodType zeType = MethodType.MethodType(btClass);
				MethodType idType = isVoid ? zeType : zeType.AppendParameterTypes(btClass);

				// Look up some symbolic names.  It might not be necessary to have these,
				// but if we need to emit direct references to bytecodes, it helps.
				// Zero is built from a call to an identity function with a constant zero input.
				MemberName idMem = new MemberName(typeof(LambdaForm), "identity_" + btChar, idType, REF_invokeStatic);
				MemberName zeMem = new MemberName(typeof(LambdaForm), "zero_" + btChar, zeType, REF_invokeStatic);
				try
				{
					zeMem = IMPL_NAMES.ResolveOrFail(REF_invokeStatic, zeMem, null, typeof(NoSuchMethodException));
					idMem = IMPL_NAMES.ResolveOrFail(REF_invokeStatic, idMem, null, typeof(NoSuchMethodException));
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (IllegalAccessException | NoSuchMethodException ex)
				{
					throw newInternalError(ex);
				}

				NamedFunction idFun = new NamedFunction(idMem);
				LambdaForm idForm;
				if (isVoid)
				{
					Name[] idNames = new Name[] {Argument(0, L_TYPE)};
					idForm = new LambdaForm(idMem.Name, 1, idNames, VOID_RESULT);
				}
				else
				{
					Name[] idNames = new Name[] {Argument(0, L_TYPE), Argument(1, type)};
					idForm = new LambdaForm(idMem.Name, 2, idNames, 1);
				}
				LF_identityForm[ord] = idForm;
				NF_identity[ord] = idFun;

				NamedFunction zeFun = new NamedFunction(zeMem);
				LambdaForm zeForm;
				if (isVoid)
				{
					zeForm = idForm;
				}
				else
				{
					Object zeValue = Wrapper.forBasicType(btChar).zero();
					Name[] zeNames = new Name[] {Argument(0, L_TYPE), new Name(idFun, zeValue)};
					zeForm = new LambdaForm(zeMem.Name, 1, zeNames, 1);
				}
				LF_zeroForm[ord] = zeForm;
				NF_zero[ord] = zeFun;

				assert(idFun.Identity);
				assert(zeFun.ConstantZero);
				assert((new Name(zeFun)).ConstantZero);
			}

			// Do this in a separate pass, so that SimpleMethodHandle.make can see the tables.
			foreach (BasicType type in BasicType.ALL_TYPES)
			{
				int ord = type.ordinal();
				NamedFunction idFun = NF_identity[ord];
				LambdaForm idForm = LF_identityForm[ord];
				MemberName idMem = idFun.Member_Renamed;
				idFun.ResolvedHandle_Renamed = SimpleMethodHandle.Make(idMem.InvocationType, idForm);

				NamedFunction zeFun = NF_zero[ord];
				LambdaForm zeForm = LF_zeroForm[ord];
				MemberName zeMem = zeFun.Member_Renamed;
				zeFun.ResolvedHandle_Renamed = SimpleMethodHandle.Make(zeMem.InvocationType, zeForm);

				assert(idFun.Identity);
				assert(zeFun.ConstantZero);
				assert((new Name(zeFun)).ConstantZero);
			}
		}

		// Avoid appealing to ValueConversions at bootstrap time:
		private static int Identity_I(int x)
		{
			return x;
		}
		private static long Identity_J(long x)
		{
			return x;
		}
		private static float Identity_F(float x)
		{
			return x;
		}
		private static double Identity_D(double x)
		{
			return x;
		}
		private static Object Identity_L(Object x)
		{
			return x;
		}
		private static void Identity_V() // same as zeroV, but that's OK
		{
			return;
		}
		private static int Zero_I()
		{
			return 0;
		}
		private static long Zero_J()
		{
			return 0;
		}
		private static float Zero_F()
		{
			return 0;
		}
		private static double Zero_D()
		{
			return 0;
		}
		private static Object Zero_L()
		{
			return null;
		}
		private static void Zero_V()
		{
			return;
		}

		/// <summary>
		/// Internal marker for byte-compiled LambdaForms.
		/// </summary>
		/*non-public*/
		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false]
		internal class Compiled : System.Attribute
		{
			private readonly LambdaForm OuterInstance;

			public Compiled(LambdaForm outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

		}

		/// <summary>
		/// Internal marker for LambdaForm interpreter frames.
		/// </summary>
		/*non-public*/
		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false]
		internal class Hidden : System.Attribute
		{
			private readonly LambdaForm OuterInstance;

			public Hidden(LambdaForm outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

		}

		private static readonly Dictionary<String, Integer> DEBUG_NAME_COUNTERS;

		// Put this last, so that previous static inits can run before.

		// The following hack is necessary in order to suppress TRACE_INTERPRETER
		// during execution of the static initializes of this class.
		// Turning on TRACE_INTERPRETER too early will cause
		// stack overflows and other misbehavior during attempts to trace events
		// that occur during LambdaForm.<clinit>.
		// Therefore, do not move this line higher in this file, and do not remove.
		private static readonly bool TRACE_INTERPRETER = MethodHandleStatics.TRACE_INTERPRETER;
	}

}