using System;
using System.Diagnostics;
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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static jdk.@internal.org.objectweb.asm.Opcodes.*;


	using ValueConversions = sun.invoke.util.ValueConversions;
	using Wrapper = sun.invoke.util.Wrapper;

	using ClassWriter = jdk.@internal.org.objectweb.asm.ClassWriter;
	using MethodVisitor = jdk.@internal.org.objectweb.asm.MethodVisitor;
	using Type = jdk.@internal.org.objectweb.asm.Type;

	/// <summary>
	/// The flavor of method handle which emulates an invoke instruction
	/// on a predetermined argument.  The JVM dispatches to the correct method
	/// when the handle is created, not when it is invoked.
	/// 
	/// All bound arguments are encapsulated in dedicated species.
	/// </summary>
	/*non-public*/	 internal abstract class BoundMethodHandle : MethodHandle
	 {

		/*non-public*/	 internal BoundMethodHandle(MethodType type, LambdaForm form) : base(type, form)
	 {
			assert(SpeciesData() == SpeciesData(form));
	 }

		//
		// BMH API and internals
		//

		internal static BoundMethodHandle BindSingle(MethodType type, LambdaForm form, BasicType xtype, Object x)
		{
			// for some type signatures, there exist pre-defined concrete BMH classes
			try
			{
				switch (xtype)
				{
				case L_TYPE:
					return BindSingle(type, form, x); // Use known fast path.
				case I_TYPE:
					return (BoundMethodHandle) SpeciesData.EMPTY.ExtendWith(I_TYPE).Constructor().invokeBasic(type, form, ValueConversions.widenSubword(x));
				case J_TYPE:
					return (BoundMethodHandle) SpeciesData.EMPTY.ExtendWith(J_TYPE).Constructor().invokeBasic(type, form, (long) x);
				case F_TYPE:
					return (BoundMethodHandle) SpeciesData.EMPTY.ExtendWith(F_TYPE).Constructor().invokeBasic(type, form, (float) x);
				case D_TYPE:
					return (BoundMethodHandle) SpeciesData.EMPTY.ExtendWith(D_TYPE).Constructor().invokeBasic(type, form, (double) x);
				default :
					throw newInternalError("unexpected xtype: " + xtype);
				}
			}
			catch (Throwable t)
			{
				throw newInternalError(t);
			}
		}

		/*non-public*/
		internal virtual LambdaFormEditor Editor()
		{
			return Form.Editor();
		}

		internal static BoundMethodHandle BindSingle(MethodType type, LambdaForm form, Object x)
		{
			return Species_L.Make(type, form, x);
		}

		internal override BoundMethodHandle BindArgumentL(int pos, Object value) // there is a default binder in the super class, for 'L' types only
		/*non-public*/
		{
			return Editor().BindArgumentL(this, pos, value);
		}
		/*non-public*/
		internal virtual BoundMethodHandle BindArgumentI(int pos, int value)
		{
			return Editor().BindArgumentI(this, pos, value);
		}
		/*non-public*/
		internal virtual BoundMethodHandle BindArgumentJ(int pos, long value)
		{
			return Editor().BindArgumentJ(this, pos, value);
		}
		/*non-public*/
		internal virtual BoundMethodHandle BindArgumentF(int pos, float value)
		{
			return Editor().BindArgumentF(this, pos, value);
		}
		/*non-public*/
		internal virtual BoundMethodHandle BindArgumentD(int pos, double value)
		{
			return Editor().BindArgumentD(this, pos, value);
		}

		internal override BoundMethodHandle Rebind()
		{
			if (!TooComplex())
			{
				return this;
			}
			return MakeReinvoker(this);
		}

		private bool TooComplex()
		{
			return (FieldCount() > FIELD_COUNT_THRESHOLD || Form.ExpressionCount() > FORM_EXPRESSION_THRESHOLD);
		}
		private const int FIELD_COUNT_THRESHOLD = 12; // largest convenient BMH field count
		private const int FORM_EXPRESSION_THRESHOLD = 24; // largest convenient BMH expression count

		/// <summary>
		/// A reinvoker MH has this form:
		/// {@code lambda (bmh, arg*) { thismh = bmh[0]; invokeBasic(thismh, arg*) }}
		/// </summary>
		internal static BoundMethodHandle MakeReinvoker(MethodHandle target)
		{
			LambdaForm form = DelegatingMethodHandle.MakeReinvokerForm(target, MethodTypeForm.LF_REBIND, Species_L.SPECIES_DATA, Species_L.SPECIES_DATA.GetterFunction(0));
			return Species_L.Make(target.Type(), form, target);
		}

		/// <summary>
		/// Return the <seealso cref="SpeciesData"/> instance representing this BMH species. All subclasses must provide a
		/// static field containing this value, and they must accordingly implement this method.
		/// </summary>
		/*non-public*/	 internal abstract SpeciesData SpeciesData();

		/*non-public*/	 internal static SpeciesData SpeciesData(LambdaForm form)
	 {
			Object c = form.Names[0].Constraint;
			if (c is SpeciesData)
			{
				return (SpeciesData) c;
			}
			// if there is no BMH constraint, then use the null constraint
			return SpeciesData.EMPTY;
	 }

		/// <summary>
		/// Return the number of fields in this BMH.  Equivalent to speciesData().fieldCount().
		/// </summary>
		/*non-public*/	 internal abstract int FieldCount();

		internal override Object InternalProperties()
		{
			return "\n& BMH=" + InternalValues();
		}

		internal override sealed Object InternalValues()
		{
			Object[] boundValues = new Object[SpeciesData().FieldCount()];
			for (int i = 0; i < boundValues.Length; ++i)
			{
				boundValues[i] = Arg(i);
			}
			return Arrays.AsList(boundValues);
		}

		/*non-public*/	 internal Object Arg(int i)
	 {
			try
			{
				switch (SpeciesData().FieldType(i))
				{
				case L_TYPE:
					return SpeciesData().Getters[i].invokeBasic(this);
				case I_TYPE:
					return (int) SpeciesData().Getters[i].invokeBasic(this);
				case J_TYPE:
					return (long) SpeciesData().Getters[i].invokeBasic(this);
				case F_TYPE:
					return (float) SpeciesData().Getters[i].invokeBasic(this);
				case D_TYPE:
					return (double) SpeciesData().Getters[i].invokeBasic(this);
				}
			}
			catch (Throwable ex)
			{
				throw newInternalError(ex);
			}
			throw new InternalError("unexpected type: " + SpeciesData().TypeChars + "." + i);
	 }

		//
		// cloning API
		//

		/*non-public*/	 internal override abstract BoundMethodHandle CopyWith(MethodType mt, LambdaForm lf);
		/*non-public*/	 internal abstract BoundMethodHandle CopyWithExtendL(MethodType mt, LambdaForm lf, Object narg);
		/*non-public*/	 internal abstract BoundMethodHandle CopyWithExtendI(MethodType mt, LambdaForm lf, int narg);
		/*non-public*/	 internal abstract BoundMethodHandle CopyWithExtendJ(MethodType mt, LambdaForm lf, long narg);
		/*non-public*/	 internal abstract BoundMethodHandle CopyWithExtendF(MethodType mt, LambdaForm lf, float narg);
		/*non-public*/	 internal abstract BoundMethodHandle CopyWithExtendD(MethodType mt, LambdaForm lf, double narg);

		//
		// concrete BMH classes required to close bootstrap loops
		//

		private sealed class Species_L : BoundMethodHandle // make it private to force users to access the enclosing class first
		{
			internal readonly Object ArgL0;
			internal Species_L(MethodType mt, LambdaForm lf, Object argL0) : base(mt, lf)
			{
				this.ArgL0 = argL0;
			}
			internal override SpeciesData SpeciesData()
			/*non-public*/
			{
				return SPECIES_DATA;
			}
			internal override int FieldCount()
			/*non-public*/
			{
				return 1;
			}
			/*non-public*/	 internal new static readonly SpeciesData SPECIES_DATA = SpeciesData.GetForClass("L", typeof(Species_L));
			/*non-public*/	 internal static BoundMethodHandle Make(MethodType mt, LambdaForm lf, Object argL0)
	 {
				return new Species_L(mt, lf, argL0);
	 }
			internal override BoundMethodHandle CopyWith(MethodType mt, LambdaForm lf)
			/*non-public*/
			{
				return new Species_L(mt, lf, ArgL0);
			}
			internal override BoundMethodHandle CopyWithExtendL(MethodType mt, LambdaForm lf, Object narg)
			/*non-public*/
			{
				try
				{
					return (BoundMethodHandle) SPECIES_DATA.ExtendWith(L_TYPE).Constructor().invokeBasic(mt, lf, ArgL0, narg);
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
			}
			internal override BoundMethodHandle CopyWithExtendI(MethodType mt, LambdaForm lf, int narg)
			/*non-public*/
			{
				try
				{
					return (BoundMethodHandle) SPECIES_DATA.ExtendWith(I_TYPE).Constructor().invokeBasic(mt, lf, ArgL0, narg);
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
			}
			internal override BoundMethodHandle CopyWithExtendJ(MethodType mt, LambdaForm lf, long narg)
			/*non-public*/
			{
				try
				{
					return (BoundMethodHandle) SPECIES_DATA.ExtendWith(J_TYPE).Constructor().invokeBasic(mt, lf, ArgL0, narg);
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
			}
			internal override BoundMethodHandle CopyWithExtendF(MethodType mt, LambdaForm lf, float narg)
			/*non-public*/
			{
				try
				{
					return (BoundMethodHandle) SPECIES_DATA.ExtendWith(F_TYPE).Constructor().invokeBasic(mt, lf, ArgL0, narg);
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
			}
			internal override BoundMethodHandle CopyWithExtendD(MethodType mt, LambdaForm lf, double narg)
			/*non-public*/
			{
				try
				{
					return (BoundMethodHandle) SPECIES_DATA.ExtendWith(D_TYPE).Constructor().invokeBasic(mt, lf, ArgL0, narg);
				}
				catch (Throwable ex)
				{
					throw uncaughtException(ex);
				}
			}
		}

		//
		// BMH species meta-data
		//

		/// <summary>
		/// Meta-data wrapper for concrete BMH types.
		/// Each BMH type corresponds to a given sequence of basic field types (LIJFD).
		/// The fields are immutable; their values are fully specified at object construction.
		/// Each BMH type supplies an array of getter functions which may be used in lambda forms.
		/// A BMH is constructed by cloning a shorter BMH and adding one or more new field values.
		/// The shortest possible BMH has zero fields; its class is SimpleMethodHandle.
		/// BMH species are not interrelated by subtyping, even though it would appear that
		/// a shorter BMH could serve as a supertype of a longer one which extends it.
		/// </summary>
		internal class SpeciesData
		{
			internal readonly String TypeChars;
			internal readonly BasicType[] TypeCodes;
			internal readonly Class Clazz;
			// Bootstrapping requires circular relations MH -> BMH -> SpeciesData -> MH
			// Therefore, we need a non-final link in the chain.  Use array elements.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable private final MethodHandle[] constructor;
			internal readonly MethodHandle[] Constructor_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable private final MethodHandle[] getters;
			internal readonly MethodHandle[] Getters;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable private final NamedFunction[] nominalGetters;
			internal readonly NamedFunction[] NominalGetters;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stable private final SpeciesData[] extensions;
			internal readonly SpeciesData[] Extensions;

			/*non-public*/	 internal virtual int FieldCount()
	 {
				return TypeCodes.Length;
	 }
			/*non-public*/	 internal virtual BasicType FieldType(int i)
	 {
				return TypeCodes[i];
	 }
			/*non-public*/	 internal virtual char FieldTypeChar(int i)
	 {
				return TypeChars.CharAt(i);
	 }
			internal virtual Object FieldSignature()
			{
				return TypeChars;
			}
			public virtual Class FieldHolder()
			{
				return Clazz;
			}
			public override String ToString()
			{
				return "SpeciesData<" + FieldSignature() + ">";
			}

			/// <summary>
			/// Return a <seealso cref="LambdaForm.Name"/> containing a <seealso cref="LambdaForm.NamedFunction"/> that
			/// represents a MH bound to a generic invoker, which in turn forwards to the corresponding
			/// getter.
			/// </summary>
			internal virtual NamedFunction GetterFunction(int i)
			{
				NamedFunction nf = NominalGetters[i];
				assert(nf.memberDeclaringClassOrNull() == FieldHolder());
				assert(nf.returnType() == FieldType(i));
				return nf;
			}

			internal virtual NamedFunction[] GetterFunctions()
			{
				return NominalGetters;
			}

			internal virtual MethodHandle[] GetterHandles()
			{
				return Getters;
			}

			internal virtual MethodHandle Constructor()
			{
				return Constructor_Renamed[0];
			}

			internal static readonly SpeciesData EMPTY = new SpeciesData("", typeof(BoundMethodHandle));

			internal SpeciesData(String types, Class clazz)
			{
				this.TypeChars = types;
				this.TypeCodes = basicTypes(types);
				this.Clazz = clazz;
				if (!INIT_DONE)
				{
					this.Constructor_Renamed = new MethodHandle[1]; // only one ctor
					this.Getters = new MethodHandle[types.Length()];
					this.NominalGetters = new NamedFunction[types.Length()];
				}
				else
				{
					this.Constructor_Renamed = Factory.MakeCtors(clazz, types, null);
					this.Getters = Factory.MakeGetters(clazz, types, null);
					this.NominalGetters = Factory.MakeNominalGetters(types, null, this.Getters);
				}
				this.Extensions = new SpeciesData[ARG_TYPE_LIMIT];
			}

			internal virtual void InitForBootstrap()
			{
				assert(!INIT_DONE);
				if (Constructor() == null)
				{
					String types = TypeChars;
					Factory.MakeCtors(Clazz, types, this.Constructor_Renamed);
					Factory.MakeGetters(Clazz, types, this.Getters);
					Factory.MakeNominalGetters(types, this.NominalGetters, this.Getters);
				}
			}

			internal SpeciesData(String typeChars)
			{
				// Placeholder only.
				this.TypeChars = typeChars;
				this.TypeCodes = basicTypes(typeChars);
				this.Clazz = null;
				this.Constructor_Renamed = null;
				this.Getters = null;
				this.NominalGetters = null;
				this.Extensions = null;
			}
			internal virtual bool Placeholder
			{
				get
				{
					return Clazz == null;
				}
			}

			internal static readonly Dictionary<String, SpeciesData> CACHE = new Dictionary<String, SpeciesData>();
			static SpeciesData() // make bootstrap predictable
			{
				CACHE[""] = EMPTY;
				// pre-fill the BMH speciesdata cache with BMH's inner classes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class rootCls = BoundMethodHandle.class;
				Class rootCls = typeof(BoundMethodHandle);
				try
				{
					foreach (Class c in rootCls.DeclaredClasses)
					{
						if (c.IsSubclassOf(rootCls))
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class cbmh = c.asSubclass(BoundMethodHandle.class);
							Class cbmh = c.AsSubclass(typeof(BoundMethodHandle));
							SpeciesData d = Factory.SpeciesDataFromConcreteBMHClass(cbmh);
							assert(d != null) : cbmh.Name;
							assert(d.Clazz == cbmh);
							assert(d == LookupCache(d.TypeChars));
						}
					}
				}
				catch (Throwable e)
				{
					throw newInternalError(e);
				}

				foreach (SpeciesData d in CACHE.Values)
				{
					d.InitForBootstrap();
				}
				// Note:  Do not simplify this, because INIT_DONE must not be
				// a compile-time constant during bootstrapping.
				INIT_DONE = true;
			}
			internal static readonly bool INIT_DONE; // set after <clinit> finishes...

			internal virtual SpeciesData ExtendWith(sbyte type)
			{
				return ExtendWith(BasicType.basicType(type));
			}

			internal virtual SpeciesData ExtendWith(BasicType type)
			{
				int ord = type.ordinal();
				SpeciesData d = Extensions[ord];
				if (d != null)
				{
					return d;
				}
				Extensions[ord] = d = Get(TypeChars + type.basicTypeChar());
				return d;
			}

			internal static SpeciesData Get(String types)
			{
				// Acquire cache lock for query.
				SpeciesData d = LookupCache(types);
				if (!d.Placeholder)
				{
					return d;
				}
				lock (d)
				{
					// Use synch. on the placeholder to prevent multiple instantiation of one species.
					// Creating this class forces a recursive call to getForClass.
					if (LookupCache(types).Placeholder)
					{
						Factory.GenerateConcreteBMHClass(types);
					}
				}
				// Reacquire cache lock.
				d = LookupCache(types);
				// Class loading must have upgraded the cache.
				assert(d != null && !d.Placeholder);
				return d;
			}
			internal static SpeciesData GetForClass(String types, Class clazz)
			{
				// clazz is a new class which is initializing its SPECIES_DATA field
				return UpdateCache(types, new SpeciesData(types, clazz));
			}
			internal static SpeciesData LookupCache(String types)
			{
				lock (typeof(SpeciesData))
				{
					SpeciesData d = CACHE[types];
					if (d != null)
					{
						return d;
					}
					d = new SpeciesData(types);
					assert(d.Placeholder);
					CACHE[types] = d;
					return d;
				}
			}
			internal static SpeciesData UpdateCache(String types, SpeciesData d)
			{
				lock (typeof(SpeciesData))
				{
					SpeciesData d2;
					assert((d2 = CACHE[types]) == null || d2.Placeholder);
					assert(!d.Placeholder);
					CACHE[types] = d;
					return d;
				}
			}

		}

		internal static SpeciesData GetSpeciesData(String types)
		{
			return SpeciesData.Get(types);
		}

		/// <summary>
		/// Generation of concrete BMH classes.
		/// 
		/// A concrete BMH species is fit for binding a number of values adhering to a
		/// given type pattern. Reference types are erased.
		/// 
		/// BMH species are cached by type pattern.
		/// 
		/// A BMH species has a number of fields with the concrete (possibly erased) types of
		/// bound values. Setters are provided as an API in BMH. Getters are exposed as MHs,
		/// which can be included as names in lambda forms.
		/// </summary>
		internal class Factory
		{

			internal const String JLO_SIG = "Ljava/lang/Object;";
			internal const String JLS_SIG = "Ljava/lang/String;";
			internal const String JLC_SIG = "Ljava/lang/Class;";
			internal const String MH = "java/lang/invoke/MethodHandle";
			internal static readonly String MH_SIG = "L" + MH + ";";
			internal const String BMH = "java/lang/invoke/BoundMethodHandle";
			internal static readonly String BMH_SIG = "L" + BMH + ";";
			internal const String SPECIES_DATA = "java/lang/invoke/BoundMethodHandle$SpeciesData";
			internal static readonly String SPECIES_DATA_SIG = "L" + SPECIES_DATA + ";";

			internal const String SPECIES_PREFIX_NAME = "Species_";
			internal static readonly String SPECIES_PREFIX_PATH = BMH + "$" + SPECIES_PREFIX_NAME;

			internal static readonly String BMHSPECIES_DATA_EWI_SIG = "(B)" + SPECIES_DATA_SIG;
			internal static readonly String BMHSPECIES_DATA_GFC_SIG = "(" + JLS_SIG + JLC_SIG + ")" + SPECIES_DATA_SIG;
			internal static readonly String MYSPECIES_DATA_SIG = "()" + SPECIES_DATA_SIG;
			internal const String VOID_SIG = "()V";
			internal const String INT_SIG = "()I";

			internal const String SIG_INCIPIT = "(Ljava/lang/invoke/MethodType;Ljava/lang/invoke/LambdaForm;";

			internal static readonly String[] E_THROWABLE = new String[] {"java/lang/Throwable"};

			/// <summary>
			/// Generate a concrete subclass of BMH for a given combination of bound types.
			/// 
			/// A concrete BMH species adheres to the following schema:
			/// 
			/// <pre>
			/// class Species_[[types]] extends BoundMethodHandle {
			///     [[fields]]
			///     final SpeciesData speciesData() { return SpeciesData.get("[[types]]"); }
			/// }
			/// </pre>
			/// 
			/// The {@code [[types]]} signature is precisely the string that is passed to this
			/// method.
			/// 
			/// The {@code [[fields]]} section consists of one field definition per character in
			/// the type signature, adhering to the naming schema described in the definition of
			/// <seealso cref="#makeFieldName"/>.
			/// 
			/// For example, a concrete BMH species for two reference and one integral bound values
			/// would have the following shape:
			/// 
			/// <pre>
			/// class BoundMethodHandle { ... private static
			/// final class Species_LLI extends BoundMethodHandle {
			///     final Object argL0;
			///     final Object argL1;
			///     final int argI2;
			///     private Species_LLI(MethodType mt, LambdaForm lf, Object argL0, Object argL1, int argI2) {
			///         super(mt, lf);
			///         this.argL0 = argL0;
			///         this.argL1 = argL1;
			///         this.argI2 = argI2;
			///     }
			///     final SpeciesData speciesData() { return SPECIES_DATA; }
			///     final int fieldCount() { return 3; }
			///     static final SpeciesData SPECIES_DATA = SpeciesData.getForClass("LLI", Species_LLI.class);
			///     static BoundMethodHandle make(MethodType mt, LambdaForm lf, Object argL0, Object argL1, int argI2) {
			///         return new Species_LLI(mt, lf, argL0, argL1, argI2);
			///     }
			///     final BoundMethodHandle copyWith(MethodType mt, LambdaForm lf) {
			///         return new Species_LLI(mt, lf, argL0, argL1, argI2);
			///     }
			///     final BoundMethodHandle copyWithExtendL(MethodType mt, LambdaForm lf, Object narg) {
			///         return SPECIES_DATA.extendWith(L_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			///     }
			///     final BoundMethodHandle copyWithExtendI(MethodType mt, LambdaForm lf, int narg) {
			///         return SPECIES_DATA.extendWith(I_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			///     }
			///     final BoundMethodHandle copyWithExtendJ(MethodType mt, LambdaForm lf, long narg) {
			///         return SPECIES_DATA.extendWith(J_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			///     }
			///     final BoundMethodHandle copyWithExtendF(MethodType mt, LambdaForm lf, float narg) {
			///         return SPECIES_DATA.extendWith(F_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			///     }
			///     public final BoundMethodHandle copyWithExtendD(MethodType mt, LambdaForm lf, double narg) {
			///         return SPECIES_DATA.extendWith(D_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			///     }
			/// }
			/// </pre>
			/// </summary>
			/// <param name="types"> the type signature, wherein reference types are erased to 'L' </param>
			/// <returns> the generated concrete BMH class </returns>
			internal static Class GenerateConcreteBMHClass(String types)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final jdk.internal.org.objectweb.asm.ClassWriter cw = new jdk.internal.org.objectweb.asm.ClassWriter(jdk.internal.org.objectweb.asm.ClassWriter.COMPUTE_MAXS + jdk.internal.org.objectweb.asm.ClassWriter.COMPUTE_FRAMES);
				ClassWriter cw = new ClassWriter(ClassWriter.COMPUTE_MAXS + ClassWriter.COMPUTE_FRAMES);

				String shortTypes = LambdaForm.ShortenSignature(types);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = SPECIES_PREFIX_PATH + shortTypes;
				String className = SPECIES_PREFIX_PATH + shortTypes;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String sourceFile = SPECIES_PREFIX_NAME + shortTypes;
				String sourceFile = SPECIES_PREFIX_NAME + shortTypes;
				const int NOT_ACC_PUBLIC = 0; // not ACC_PUBLIC
				cw.visit(V1_6, NOT_ACC_PUBLIC + ACC_FINAL + ACC_SUPER, className, null, BMH, null);
				cw.visitSource(sourceFile, null);

				// emit static types and SPECIES_DATA fields
				cw.visitField(NOT_ACC_PUBLIC + ACC_STATIC, "SPECIES_DATA", SPECIES_DATA_SIG, null, null).visitEnd();

				// emit bound argument fields
				for (int i = 0; i < types.Length(); ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char t = types.charAt(i);
					char t = types.CharAt(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fieldName = makeFieldName(types, i);
					String fieldName = MakeFieldName(types, i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fieldDesc = t == 'L' ? JLO_SIG : String.valueOf(t);
					String fieldDesc = t == 'L' ? JLO_SIG : Convert.ToString(t);
					cw.visitField(ACC_FINAL, fieldName, fieldDesc, null, null).visitEnd();
				}

				MethodVisitor mv;

				// emit constructor
				mv = cw.visitMethod(ACC_PRIVATE, "<init>", MakeSignature(types, true), null, null);
				mv.visitCode();
				mv.visitVarInsn(ALOAD, 0); // this
				mv.visitVarInsn(ALOAD, 1); // type
				mv.visitVarInsn(ALOAD, 2); // form

				mv.visitMethodInsn(INVOKESPECIAL, BMH, "<init>", MakeSignature("", true), false);

				for (int i = 0, j = 0; i < types.Length(); ++i, ++j)
				{
					// i counts the arguments, j counts corresponding argument slots
					char t = types.CharAt(i);
					mv.visitVarInsn(ALOAD, 0);
					mv.visitVarInsn(TypeLoadOp(t), j + 3); // parameters start at 3
					mv.visitFieldInsn(PUTFIELD, className, MakeFieldName(types, i), TypeSig(t));
					if (t == 'J' || t == 'D')
					{
						++j; // adjust argument register access
					}
				}

				mv.visitInsn(RETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();

				// emit implementation of speciesData()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "speciesData", MYSPECIES_DATA_SIG, null, null);
				mv.visitCode();
				mv.visitFieldInsn(GETSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG);
				mv.visitInsn(ARETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();

				// emit implementation of fieldCount()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "fieldCount", INT_SIG, null, null);
				mv.visitCode();
				int fc = types.Length();
				if (fc <= (ICONST_5 - ICONST_0))
				{
					mv.visitInsn(ICONST_0 + fc);
				}
				else
				{
					mv.visitIntInsn(SIPUSH, fc);
				}
				mv.visitInsn(IRETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();
				// emit make()  ...factory method wrapping constructor
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_STATIC, "make", MakeSignature(types, false), null, null);
				mv.visitCode();
				// make instance
				mv.visitTypeInsn(NEW, className);
				mv.visitInsn(DUP);
				// load mt, lf
				mv.visitVarInsn(ALOAD, 0); // type
				mv.visitVarInsn(ALOAD, 1); // form
				// load factory method arguments
				for (int i = 0, j = 0; i < types.Length(); ++i, ++j)
				{
					// i counts the arguments, j counts corresponding argument slots
					char t = types.CharAt(i);
					mv.visitVarInsn(TypeLoadOp(t), j + 2); // parameters start at 3
					if (t == 'J' || t == 'D')
					{
						++j; // adjust argument register access
					}
				}

				// finally, invoke the constructor and return
				mv.visitMethodInsn(INVOKESPECIAL, className, "<init>", MakeSignature(types, true), false);
				mv.visitInsn(ARETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();

				// emit copyWith()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "copyWith", MakeSignature("", false), null, null);
				mv.visitCode();
				// make instance
				mv.visitTypeInsn(NEW, className);
				mv.visitInsn(DUP);
				// load mt, lf
				mv.visitVarInsn(ALOAD, 1);
				mv.visitVarInsn(ALOAD, 2);
				// put fields on the stack
				EmitPushFields(types, className, mv);
				// finally, invoke the constructor and return
				mv.visitMethodInsn(INVOKESPECIAL, className, "<init>", MakeSignature(types, true), false);
				mv.visitInsn(ARETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();

				// for each type, emit copyWithExtendT()
				foreach (BasicType type in BasicType.ARG_TYPES)
				{
					int ord = type.ordinal();
					char btChar = type.basicTypeChar();
					mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "copyWithExtend" + btChar, MakeSignature(Convert.ToString(btChar), false), null, E_THROWABLE);
					mv.visitCode();
					// return SPECIES_DATA.extendWith(t).constructor().invokeBasic(mt, lf, argL0, ..., narg)
					// obtain constructor
					mv.visitFieldInsn(GETSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG);
					int iconstInsn = ICONST_0 + ord;
					assert(iconstInsn <= ICONST_5);
					mv.visitInsn(iconstInsn);
					mv.visitMethodInsn(INVOKEVIRTUAL, SPECIES_DATA, "extendWith", BMHSPECIES_DATA_EWI_SIG, false);
					mv.visitMethodInsn(INVOKEVIRTUAL, SPECIES_DATA, "constructor", "()" + MH_SIG, false);
					// load mt, lf
					mv.visitVarInsn(ALOAD, 1);
					mv.visitVarInsn(ALOAD, 2);
					// put fields on the stack
					EmitPushFields(types, className, mv);
					// put narg on stack
					mv.visitVarInsn(TypeLoadOp(btChar), 3);
					// finally, invoke the constructor and return
					mv.visitMethodInsn(INVOKEVIRTUAL, MH, "invokeBasic", MakeSignature(types + btChar, false), false);
					mv.visitInsn(ARETURN);
					mv.visitMaxs(0, 0);
					mv.visitEnd();
				}

				// emit class initializer
				mv = cw.visitMethod(NOT_ACC_PUBLIC | ACC_STATIC, "<clinit>", VOID_SIG, null, null);
				mv.visitCode();
				mv.visitLdcInsn(types);
				mv.visitLdcInsn(Type.getObjectType(className));
				mv.visitMethodInsn(INVOKESTATIC, SPECIES_DATA, "getForClass", BMHSPECIES_DATA_GFC_SIG, false);
				mv.visitFieldInsn(PUTSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG);
				mv.visitInsn(RETURN);
				mv.visitMaxs(0, 0);
				mv.visitEnd();

				cw.visitEnd();

				// load class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] classFile = cw.toByteArray();
				sbyte[] classFile = cw.toByteArray();
				InvokerBytecodeGenerator.MaybeDump(className, classFile);
				Class bmhClass = UNSAFE.defineClass(className, classFile, 0, classFile.Length, typeof(BoundMethodHandle).ClassLoader, null).asSubclass(typeof(BoundMethodHandle));
					//UNSAFE.defineAnonymousClass(BoundMethodHandle.class, classFile, null).asSubclass(BoundMethodHandle.class);
				UNSAFE.ensureClassInitialized(bmhClass);

				return bmhClass;
			}

			internal static int TypeLoadOp(char t)
			{
				switch (t)
				{
				case 'L':
					return ALOAD;
				case 'I':
					return ILOAD;
				case 'J':
					return LLOAD;
				case 'F':
					return FLOAD;
				case 'D':
					return DLOAD;
				default :
					throw newInternalError("unrecognized type " + t);
				}
			}

			internal static void EmitPushFields(String types, String className, MethodVisitor mv)
			{
				for (int i = 0; i < types.Length(); ++i)
				{
					char tc = types.CharAt(i);
					mv.visitVarInsn(ALOAD, 0);
					mv.visitFieldInsn(GETFIELD, className, MakeFieldName(types, i), TypeSig(tc));
				}
			}

			internal static String TypeSig(char t)
			{
				return t == 'L' ? JLO_SIG : Convert.ToString(t);
			}

			//
			// Getter MH generation.
			//

			internal static MethodHandle MakeGetter(Class cbmhClass, String types, int index)
			{
				String fieldName = MakeFieldName(types, index);
				Class fieldType = Wrapper.forBasicType(types.CharAt(index)).primitiveType();
				try
				{
					return LOOKUP.findGetter(cbmhClass, fieldName, fieldType);
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (NoSuchFieldException | IllegalAccessException e)
				{
					throw newInternalError(e);
				}
			}

			internal static MethodHandle[] MakeGetters(Class cbmhClass, String types, MethodHandle[] mhs)
			{
				if (mhs == null)
				{
					mhs = new MethodHandle[types.Length()];
				}
				for (int i = 0; i < mhs.Length; ++i)
				{
					mhs[i] = MakeGetter(cbmhClass, types, i);
					assert(mhs[i].InternalMemberName().DeclaringClass == cbmhClass);
				}
				return mhs;
			}

			internal static MethodHandle[] MakeCtors(Class cbmh, String types, MethodHandle[] mhs)
			{
				if (mhs == null)
				{
					mhs = new MethodHandle[1];
				}
				if (types.Equals("")) // hack for empty BMH species
				{
					return mhs;
				}
				mhs[0] = MakeCbmhCtor(cbmh, types);
				return mhs;
			}

			internal static NamedFunction[] MakeNominalGetters(String types, NamedFunction[] nfs, MethodHandle[] getters)
			{
				if (nfs == null)
				{
					nfs = new NamedFunction[types.Length()];
				}
				for (int i = 0; i < nfs.Length; ++i)
				{
					nfs[i] = new NamedFunction(getters[i]);
				}
				return nfs;
			}

			//
			// Auxiliary methods.
			//

			internal static SpeciesData SpeciesDataFromConcreteBMHClass(Class cbmh)
			{
				try
				{
					Field F_SPECIES_DATA = cbmh.GetDeclaredField("SPECIES_DATA");
					return (SpeciesData) F_SPECIES_DATA.get(null);
				}
				catch (ReflectiveOperationException ex)
				{
					throw newInternalError(ex);
				}
			}

			/// <summary>
			/// Field names in concrete BMHs adhere to this pattern:
			/// arg + type + index
			/// where type is a single character (L, I, J, F, D).
			/// </summary>
			internal static String MakeFieldName(String types, int index)
			{
				Debug.Assert(index >= 0 && index < types.Length());
				return "arg" + types.CharAt(index) + index;
			}

			internal static String MakeSignature(String types, bool ctor)
			{
				StringBuilder buf = new StringBuilder(SIG_INCIPIT);
				foreach (char c in types.ToCharArray())
				{
					buf.Append(TypeSig(c));
				}
				return buf.Append(')').Append(ctor ? "V" : BMH_SIG).ToString();
			}

			internal static MethodHandle MakeCbmhCtor(Class cbmh, String types)
			{
				try
				{
					return LOOKUP.findStatic(cbmh, "make", MethodType.FromMethodDescriptorString(MakeSignature(types, false), null));
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (NoSuchMethodException | IllegalAccessException | IllegalArgumentException | TypeNotPresentException e)
				{
					throw newInternalError(e);
				}
			}
		}

		private static readonly Lookup LOOKUP = Lookup.IMPL_LOOKUP;

		/// <summary>
		/// All subclasses must provide such a value describing their type signature.
		/// </summary>
		internal static readonly SpeciesData SPECIES_DATA = SpeciesData.EMPTY;

		private static readonly SpeciesData[] SPECIES_DATA_CACHE = new SpeciesData[5];
		private static SpeciesData CheckCache(int size, String types)
		{
			int idx = size - 1;
			SpeciesData data = SPECIES_DATA_CACHE[idx];
			if (data != null)
			{
				return data;
			}
			SPECIES_DATA_CACHE[idx] = data = GetSpeciesData(types);
			return data;
		}
		internal static SpeciesData SpeciesData_L()
		{
			return CheckCache(1, "L");
		}
		internal static SpeciesData SpeciesData_LL()
		{
			return CheckCache(2, "LL");
		}
		internal static SpeciesData SpeciesData_LLL()
		{
			return CheckCache(3, "LLL");
		}
		internal static SpeciesData SpeciesData_LLLL()
		{
			return CheckCache(4, "LLLL");
		}
		internal static SpeciesData SpeciesData_LLLLL()
		{
			return CheckCache(5, "LLLLL");
		}
	 }

}