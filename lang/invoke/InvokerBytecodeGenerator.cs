using System;
using System.Diagnostics;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using jdk.@internal.org.objectweb.asm;


	using VerifyAccess = sun.invoke.util.VerifyAccess;
	using VerifyType = sun.invoke.util.VerifyType;
	using Wrapper = sun.invoke.util.Wrapper;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// Code generation backend for LambdaForm.
	/// <para>
	/// @author John Rose, JSR 292 EG
	/// </para>
	/// </summary>
	internal class InvokerBytecodeGenerator
	{
		/// <summary>
		/// Define class names for convenience. </summary>
		private const String MH = "java/lang/invoke/MethodHandle";
		private const String MHI = "java/lang/invoke/MethodHandleImpl";
		private const String LF = "java/lang/invoke/LambdaForm";
		private const String LFN = "java/lang/invoke/LambdaForm$Name";
		private const String CLS = "java/lang/Class";
		private const String OBJ = "java/lang/Object";
		private const String OBJARY = "[Ljava/lang/Object;";

		private static readonly String MH_SIG = "L" + MH + ";";
		private static readonly String LF_SIG = "L" + LF + ";";
		private static readonly String LFN_SIG = "L" + LFN + ";";
		private static readonly String LL_SIG = "(L" + OBJ + ";)L" + OBJ + ";";
		private static readonly String LLV_SIG = "(L" + OBJ + ";L" + OBJ + ";)V";
		private static readonly String CLL_SIG = "(L" + CLS + ";L" + OBJ + ";)L" + OBJ + ";";

		/// <summary>
		/// Name of its super class </summary>
		private const String SuperName = OBJ;

		/// <summary>
		/// Name of new class </summary>
		private readonly String ClassName;

		/// <summary>
		/// Name of the source file (for stack trace printing). </summary>
		private readonly String SourceFile;

		private readonly LambdaForm LambdaForm;
		private readonly String InvokerName;
		private readonly MethodType InvokerType;

		/// <summary>
		/// Info about local variables in compiled lambda form </summary>
		private readonly int[] LocalsMap; // index
		private readonly BasicType[] LocalTypes; // basic type
		private readonly Class[] LocalClasses; // type

		/// <summary>
		/// ASM bytecode generation. </summary>
		private ClassWriter Cw;
		private MethodVisitor Mv;

		private static readonly MemberName.Factory MEMBERNAME_FACTORY = MemberName.Factory;
		private static readonly Class HOST_CLASS = typeof(LambdaForm);

		/// <summary>
		/// Main constructor; other constructors delegate to this one. </summary>
		private InvokerBytecodeGenerator(LambdaForm lambdaForm, int localsMapSize, String className, String invokerName, MethodType invokerType)
		{
			if (invokerName.Contains("."))
			{
				int p = invokerName.IndexOf(".");
				className = invokerName.Substring(0, p);
				invokerName = invokerName.Substring(p + 1);
			}
			if (DUMP_CLASS_FILES)
			{
				className = MakeDumpableClassName(className);
			}
			this.ClassName = LF + "$" + className;
			this.SourceFile = "LambdaForm$" + className;
			this.LambdaForm = lambdaForm;
			this.InvokerName = invokerName;
			this.InvokerType = invokerType;
			this.LocalsMap = new int[localsMapSize+1];
			// last entry of localsMap is count of allocated local slots
			this.LocalTypes = new BasicType[localsMapSize+1];
			this.LocalClasses = new Class[localsMapSize+1];
		}

		/// <summary>
		/// For generating LambdaForm interpreter entry points. </summary>
		private InvokerBytecodeGenerator(String className, String invokerName, MethodType invokerType) : this(null, invokerType.ParameterCount(), className, invokerName, invokerType)
		{
			// Create an array to map name indexes to locals indexes.
			LocalTypes[LocalTypes.Length - 1] = V_TYPE;
			for (int i = 0; i < LocalsMap.Length; i++)
			{
				LocalsMap[i] = invokerType.ParameterSlotCount() - invokerType.ParameterSlotDepth(i);
				if (i < invokerType.ParameterCount())
				{
					LocalTypes[i] = basicType(invokerType.ParameterType(i));
				}
			}
		}

		/// <summary>
		/// For generating customized code for a single LambdaForm. </summary>
		private InvokerBytecodeGenerator(String className, LambdaForm form, MethodType invokerType) : this(form, form.Names.Length, className, form.DebugName, invokerType)
		{
			// Create an array to map name indexes to locals indexes.
			Name[] names = form.Names;
			for (int i = 0, index = 0; i < LocalsMap.Length; i++)
			{
				LocalsMap[i] = index;
				if (i < names.Length)
				{
					BasicType type = names[i].type();
					index += type.basicTypeSlots();
					LocalTypes[i] = type;
				}
			}
		}


		/// <summary>
		/// instance counters for dumped classes </summary>
		private static readonly HashMap<String, Integer> DUMP_CLASS_FILES_COUNTERS;
		/// <summary>
		/// debugging flag for saving generated class files </summary>
		private static readonly File DUMP_CLASS_FILES_DIR;

		static InvokerBytecodeGenerator()
		{
			if (DUMP_CLASS_FILES)
			{
				DUMP_CLASS_FILES_COUNTERS = new HashMap<>();
				try
				{
					File dumpDir = new File("DUMP_CLASS_FILES");
					if (!dumpDir.Exists())
					{
						dumpDir.Mkdirs();
					}
					DUMP_CLASS_FILES_DIR = dumpDir;
					System.Console.WriteLine("Dumping class files to " + DUMP_CLASS_FILES_DIR + "/...");
				}
				catch (Exception e)
				{
					throw newInternalError(e);
				}
			}
			else
			{
				DUMP_CLASS_FILES_COUNTERS = null;
				DUMP_CLASS_FILES_DIR = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static void maybeDump(final String className, final byte[] classFile)
		internal static void MaybeDump(String className, sbyte[] classFile)
		{
			if (DUMP_CLASS_FILES)
			{
				java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(className, classFile));
			}

		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			private string ClassName;
			private sbyte[] ClassFile;

			public PrivilegedActionAnonymousInnerClassHelper(string className, sbyte[] classFile)
			{
				this.ClassName = className;
				this.ClassFile = classFile;
			}

			public virtual Void Run()
			{
				try
				{
					String dumpName = ClassName;
					//dumpName = dumpName.replace('/', '-');
					File dumpFile = new File(DUMP_CLASS_FILES_DIR, dumpName + ".class");
					System.Console.WriteLine("dump: " + dumpFile);
					dumpFile.ParentFile.Mkdirs();
					FileOutputStream file = new FileOutputStream(dumpFile);
					file.Write(ClassFile);
					file.Close();
					return null;
				}
				catch (IOException ex)
				{
					throw newInternalError(ex);
				}
			}
		}

		private static String MakeDumpableClassName(String className)
		{
			Integer ctr;
			lock (DUMP_CLASS_FILES_COUNTERS)
			{
				ctr = DUMP_CLASS_FILES_COUNTERS.Get(className);
				if (ctr == null)
				{
					ctr = 0;
				}
				DUMP_CLASS_FILES_COUNTERS.Put(className, ctr + 1);
			}
			String sfx = ctr.ToString();
			while (sfx.Length() < 3)
			{
				sfx = "0" + sfx;
			}
			className += sfx;
			return className;
		}

		internal class CpPatch
		{
			private readonly InvokerBytecodeGenerator OuterInstance;

			internal readonly int Index;
			internal readonly String Placeholder;
			internal readonly Object Value;
			internal CpPatch(InvokerBytecodeGenerator outerInstance, int index, String placeholder, Object value)
			{
				this.OuterInstance = outerInstance;
				this.Index = index;
				this.Placeholder = placeholder;
				this.Value = value;
			}
			public override String ToString()
			{
				return "CpPatch/index=" + Index + ",placeholder=" + Placeholder + ",value=" + Value;
			}
		}

		internal Map<Object, CpPatch> CpPatches_Renamed = new HashMap<Object, CpPatch>();

		internal int Cph = 0; // for counting constant placeholders

		internal virtual String ConstantPlaceholder(Object arg)
		{
			String cpPlaceholder = "CONSTANT_PLACEHOLDER_" + Cph++;
			if (DUMP_CLASS_FILES) // debugging aid
			{
				cpPlaceholder += " <<" + DebugString(arg) + ">>";
			}
			if (CpPatches_Renamed.ContainsKey(cpPlaceholder))
			{
				throw new InternalError("observed CP placeholder twice: " + cpPlaceholder);
			}
			// insert placeholder in CP and remember the patch
			int index = Cw.newConst((Object) cpPlaceholder); // TODO check if aready in the constant pool
			CpPatches_Renamed.Put(cpPlaceholder, new CpPatch(this, index, cpPlaceholder, arg));
			return cpPlaceholder;
		}

		internal virtual Object[] CpPatches(sbyte[] classFile)
		{
			int size = GetConstantPoolSize(classFile);
			Object[] res = new Object[size];
			foreach (CpPatch p in CpPatches_Renamed.Values())
			{
				if (p.Index >= size)
				{
					throw new InternalError("in cpool[" + size + "]: " + p + "\n" + Arrays.ToString(Arrays.CopyOf(classFile, 20)));
				}
				res[p.Index] = p.Value;
			}
			return res;
		}

		private static String DebugString(Object arg)
		{
			if (arg is MethodHandle)
			{
				MethodHandle mh = (MethodHandle) arg;
				MemberName member = mh.InternalMemberName();
				if (member != null)
				{
					return member.ToString();
				}
				return mh.DebugString();
			}
			return arg.ToString();
		}

		/// <summary>
		/// Extract the number of constant pool entries from a given class file.
		/// </summary>
		/// <param name="classFile"> the bytes of the class file in question. </param>
		/// <returns> the number of entries in the constant pool. </returns>
		private static int GetConstantPoolSize(sbyte[] classFile)
		{
			// The first few bytes:
			// u4 magic;
			// u2 minor_version;
			// u2 major_version;
			// u2 constant_pool_count;
			return ((classFile[8] & 0xFF) << 8) | (classFile[9] & 0xFF);
		}

		/// <summary>
		/// Extract the MemberName of a newly-defined method.
		/// </summary>
		private MemberName LoadMethod(sbyte[] classFile)
		{
			Class invokerClass = LoadAndInitializeInvokerClass(classFile, CpPatches(classFile));
			return ResolveInvokerMember(invokerClass, InvokerName, InvokerType);
		}

		/// <summary>
		/// Define a given class as anonymous class in the runtime system.
		/// </summary>
		private static Class LoadAndInitializeInvokerClass(sbyte[] classBytes, Object[] patches)
		{
			Class invokerClass = UNSAFE.defineAnonymousClass(HOST_CLASS, classBytes, patches);
			UNSAFE.ensureClassInitialized(invokerClass); // Make sure the class is initialized; VM might complain.
			return invokerClass;
		}

		private static MemberName ResolveInvokerMember(Class invokerClass, String name, MethodType type)
		{
			MemberName member = new MemberName(invokerClass, name, type, REF_invokeStatic);
			//System.out.println("resolveInvokerMember => "+member);
			//for (Method m : invokerClass.getDeclaredMethods())  System.out.println("  "+m);
			try
			{
				member = MEMBERNAME_FACTORY.ResolveOrFail(REF_invokeStatic, member, HOST_CLASS, typeof(ReflectiveOperationException));
			}
			catch (ReflectiveOperationException e)
			{
				throw newInternalError(e);
			}
			//System.out.println("resolveInvokerMember => "+member);
			return member;
		}

		/// <summary>
		/// Set up class file generation.
		/// </summary>
		private void ClassFilePrologue()
		{
			const int NOT_ACC_PUBLIC = 0; // not ACC_PUBLIC
			Cw = new ClassWriter(ClassWriter.COMPUTE_MAXS + ClassWriter.COMPUTE_FRAMES);
			Cw.visit(Opcodes.V1_8, NOT_ACC_PUBLIC + Opcodes.ACC_FINAL + Opcodes.ACC_SUPER, ClassName, null, SuperName, null);
			Cw.visitSource(SourceFile, null);

			String invokerDesc = InvokerType.ToMethodDescriptorString();
			Mv = Cw.visitMethod(Opcodes.ACC_STATIC, InvokerName, invokerDesc, null, null);
		}

		/// <summary>
		/// Tear down class file generation.
		/// </summary>
		private void ClassFileEpilogue()
		{
			Mv.visitMaxs(0, 0);
			Mv.visitEnd();
		}

		/*
		 * Low-level emit helpers.
		 */
		private void EmitConst(Object con)
		{
			if (con == null)
			{
				Mv.visitInsn(Opcodes.ACONST_NULL);
				return;
			}
			if (con is Integer)
			{
				EmitIconstInsn((int) con);
				return;
			}
			if (con is Long)
			{
				long x = (long) con;
				if (x == (short) x)
				{
					EmitIconstInsn((int) x);
					Mv.visitInsn(Opcodes.I2L);
					return;
				}
			}
			if (con is Float)
			{
				float x = (float) con;
				if (x == (short) x)
				{
					EmitIconstInsn((int) x);
					Mv.visitInsn(Opcodes.I2F);
					return;
				}
			}
			if (con is Double)
			{
				double x = (double) con;
				if (x == (short) x)
				{
					EmitIconstInsn((int) x);
					Mv.visitInsn(Opcodes.I2D);
					return;
				}
			}
			if (con is Boolean)
			{
				EmitIconstInsn((bool) con ? 1 : 0);
				return;
			}
			// fall through:
			Mv.visitLdcInsn(con);
		}

		private void EmitIconstInsn(int i)
		{
			int opcode;
			switch (i)
			{
			case 0:
				opcode = Opcodes.ICONST_0;
				break;
			case 1:
				opcode = Opcodes.ICONST_1;
				break;
			case 2:
				opcode = Opcodes.ICONST_2;
				break;
			case 3:
				opcode = Opcodes.ICONST_3;
				break;
			case 4:
				opcode = Opcodes.ICONST_4;
				break;
			case 5:
				opcode = Opcodes.ICONST_5;
				break;
			default:
				if (i == (sbyte) i)
				{
					Mv.visitIntInsn(Opcodes.BIPUSH, i & 0xFF);
				}
				else if (i == (short) i)
				{
					Mv.visitIntInsn(Opcodes.SIPUSH, (char) i);
				}
				else
				{
					Mv.visitLdcInsn(i);
				}
				return;
			}
			Mv.visitInsn(opcode);
		}

		/*
		 * NOTE: These load/store methods use the localsMap to find the correct index!
		 */
		private void EmitLoadInsn(BasicType type, int index)
		{
			int opcode = LoadInsnOpcode(type);
			Mv.visitVarInsn(opcode, LocalsMap[index]);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int loadInsnOpcode(BasicType type) throws InternalError
		private int LoadInsnOpcode(BasicType type)
		{
			switch (type)
			{
				case I_TYPE:
					return Opcodes.ILOAD;
				case J_TYPE:
					return Opcodes.LLOAD;
				case F_TYPE:
					return Opcodes.FLOAD;
				case D_TYPE:
					return Opcodes.DLOAD;
				case L_TYPE:
					return Opcodes.ALOAD;
				default:
					throw new InternalError("unknown type: " + type);
			}
		}
		private void EmitAloadInsn(int index)
		{
			EmitLoadInsn(L_TYPE, index);
		}

		private void EmitStoreInsn(BasicType type, int index)
		{
			int opcode = StoreInsnOpcode(type);
			Mv.visitVarInsn(opcode, LocalsMap[index]);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int storeInsnOpcode(BasicType type) throws InternalError
		private int StoreInsnOpcode(BasicType type)
		{
			switch (type)
			{
				case I_TYPE:
					return Opcodes.ISTORE;
				case J_TYPE:
					return Opcodes.LSTORE;
				case F_TYPE:
					return Opcodes.FSTORE;
				case D_TYPE:
					return Opcodes.DSTORE;
				case L_TYPE:
					return Opcodes.ASTORE;
				default:
					throw new InternalError("unknown type: " + type);
			}
		}
		private void EmitAstoreInsn(int index)
		{
			EmitStoreInsn(L_TYPE, index);
		}

		private sbyte ArrayTypeCode(Wrapper elementType)
		{
			switch (elementType)
			{
				case BOOLEAN:
					return Opcodes.T_BOOLEAN;
				case BYTE:
					return Opcodes.T_BYTE;
				case CHAR:
					return Opcodes.T_CHAR;
				case SHORT:
					return Opcodes.T_SHORT;
				case INT:
					return Opcodes.T_INT;
				case LONG:
					return Opcodes.T_LONG;
				case FLOAT:
					return Opcodes.T_FLOAT;
				case DOUBLE:
					return Opcodes.T_DOUBLE;
				case OBJECT: // in place of Opcodes.T_OBJECT
					return 0;
				default:
					throw new InternalError();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int arrayInsnOpcode(byte tcode, int aaop) throws InternalError
		private int ArrayInsnOpcode(sbyte tcode, int aaop)
		{
			assert(aaop == Opcodes.AASTORE || aaop == Opcodes.AALOAD);
			int xas;
			switch (tcode)
			{
				case Opcodes.T_BOOLEAN:
					xas = Opcodes.BASTORE;
					break;
				case Opcodes.T_BYTE:
					xas = Opcodes.BASTORE;
					break;
				case Opcodes.T_CHAR:
					xas = Opcodes.CASTORE;
					break;
				case Opcodes.T_SHORT:
					xas = Opcodes.SASTORE;
					break;
				case Opcodes.T_INT:
					xas = Opcodes.IASTORE;
					break;
				case Opcodes.T_LONG:
					xas = Opcodes.LASTORE;
					break;
				case Opcodes.T_FLOAT:
					xas = Opcodes.FASTORE;
					break;
				case Opcodes.T_DOUBLE:
					xas = Opcodes.DASTORE;
					break;
				case 0:
					xas = Opcodes.AASTORE;
					break;
				default:
					throw new InternalError();
			}
			return xas - Opcodes.AASTORE + aaop;
		}


		private void FreeFrameLocal(int oldFrameLocal)
		{
			int i = IndexForFrameLocal(oldFrameLocal);
			if (i < 0)
			{
				return;
			}
			BasicType type = LocalTypes[i];
			int newFrameLocal = MakeLocalTemp(type);
			Mv.visitVarInsn(LoadInsnOpcode(type), oldFrameLocal);
			Mv.visitVarInsn(StoreInsnOpcode(type), newFrameLocal);
			assert(LocalsMap[i] == oldFrameLocal);
			LocalsMap[i] = newFrameLocal;
			assert(IndexForFrameLocal(oldFrameLocal) < 0);
		}
		private int IndexForFrameLocal(int frameLocal)
		{
			for (int i = 0; i < LocalsMap.Length; i++)
			{
				if (LocalsMap[i] == frameLocal && LocalTypes[i] != V_TYPE)
				{
					return i;
				}
			}
			return -1;
		}
		private int MakeLocalTemp(BasicType type)
		{
			int frameLocal = LocalsMap[LocalsMap.Length - 1];
			LocalsMap[LocalsMap.Length - 1] = frameLocal + type.basicTypeSlots();
			return frameLocal;
		}

		/// <summary>
		/// Emit a boxing call.
		/// </summary>
		/// <param name="wrapper"> primitive type class to box. </param>
		private void EmitBoxing(Wrapper wrapper)
		{
			String owner = "java/lang/" + wrapper.wrapperType().SimpleName;
			String name = "valueOf";
			String desc = "(" + wrapper.basicTypeChar() + ")L" + owner + ";";
			Mv.visitMethodInsn(Opcodes.INVOKESTATIC, owner, name, desc, false);
		}

		/// <summary>
		/// Emit an unboxing call (plus preceding checkcast).
		/// </summary>
		/// <param name="wrapper"> wrapper type class to unbox. </param>
		private void EmitUnboxing(Wrapper wrapper)
		{
			String owner = "java/lang/" + wrapper.wrapperType().SimpleName;
			String name = wrapper.primitiveSimpleName() + "Value";
			String desc = "()" + wrapper.basicTypeChar();
			EmitReferenceCast(wrapper.wrapperType(), null);
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, owner, name, desc, false);
		}

		/// <summary>
		/// Emit an implicit conversion for an argument which must be of the given pclass.
		/// This is usually a no-op, except when pclass is a subword type or a reference other than Object or an interface.
		/// </summary>
		/// <param name="ptype"> type of value present on stack </param>
		/// <param name="pclass"> type of value required on stack </param>
		/// <param name="arg"> compile-time representation of value on stack (Node, constant) or null if none </param>
		private void EmitImplicitConversion(BasicType ptype, Class pclass, Object arg)
		{
			assert(basicType(pclass) == ptype); // boxing/unboxing handled by caller
			if (pclass == ptype.basicTypeClass() && ptype != L_TYPE)
			{
				return; // nothing to do
			}
			switch (ptype)
			{
				case L_TYPE:
					if (VerifyType.isNullConversion(typeof(Object), pclass, false))
					{
						if (PROFILE_LEVEL > 0)
						{
							EmitReferenceCast(typeof(Object), arg);
						}
						return;
					}
					EmitReferenceCast(pclass, arg);
					return;
				case I_TYPE:
					if (!VerifyType.isNullConversion(typeof(int), pclass, false))
					{
						EmitPrimCast(ptype.basicTypeWrapper(), Wrapper.forPrimitiveType(pclass));
					}
					return;
			}
			throw newInternalError("bad implicit conversion: tc=" + ptype + ": " + pclass);
		}

		/// <summary>
		/// Update localClasses type map.  Return true if the information is already present. </summary>
		private bool AssertStaticType(Class cls, Name n)
		{
			int local = n.index();
			Class aclass = LocalClasses[local];
			if (aclass != null && (aclass == cls || aclass.IsSubclassOf(cls)))
			{
				return true; // type info is already present
			}
			else if (aclass == null || cls.IsSubclassOf(aclass))
			{
				LocalClasses[local] = cls; // type info can be improved
			}
			return false;
		}

		private void EmitReferenceCast(Class cls, Object arg)
		{
			Name writeBack = null; // local to write back result
			if (arg is Name)
			{
				Name n = (Name) arg;
				if (AssertStaticType(cls, n))
				{
					return; // this cast was already performed
				}
				if (LambdaForm.UseCount(n) > 1)
				{
					// This guy gets used more than once.
					writeBack = n;
				}
			}
			if (IsStaticallyNameable(cls))
			{
				String sig = GetInternalName(cls);
				Mv.visitTypeInsn(Opcodes.CHECKCAST, sig);
			}
			else
			{
				Mv.visitLdcInsn(ConstantPlaceholder(cls));
				Mv.visitTypeInsn(Opcodes.CHECKCAST, CLS);
				Mv.visitInsn(Opcodes.SWAP);
				Mv.visitMethodInsn(Opcodes.INVOKESTATIC, MHI, "castReference", CLL_SIG, false);
				if (cls.IsSubclassOf(typeof(Object[])))
				{
					Mv.visitTypeInsn(Opcodes.CHECKCAST, OBJARY);
				}
				else if (PROFILE_LEVEL > 0)
				{
					Mv.visitTypeInsn(Opcodes.CHECKCAST, OBJ);
				}
			}
			if (writeBack != null)
			{
				Mv.visitInsn(Opcodes.DUP);
				EmitAstoreInsn(writeBack.index());
			}
		}

		/// <summary>
		/// Emits an actual return instruction conforming to the given return type.
		/// </summary>
		private void EmitReturnInsn(BasicType type)
		{
			int opcode;
			switch (type)
			{
			case I_TYPE:
				opcode = Opcodes.IRETURN;
				break;
			case J_TYPE:
				opcode = Opcodes.LRETURN;
				break;
			case F_TYPE:
				opcode = Opcodes.FRETURN;
				break;
			case D_TYPE:
				opcode = Opcodes.DRETURN;
				break;
			case L_TYPE:
				opcode = Opcodes.ARETURN;
				break;
			case V_TYPE:
				opcode = Opcodes.RETURN;
				break;
			default:
				throw new InternalError("unknown return type: " + type);
			}
			Mv.visitInsn(opcode);
		}

		private static String GetInternalName(Class c)
		{
			if (c == typeof(Object))
			{
				return OBJ;
			}
			else if (c == typeof(Object[]))
			{
				return OBJARY;
			}
			else if (c == typeof(Class))
			{
				return CLS;
			}
			else if (c == typeof(MethodHandle))
			{
				return MH;
			}
			assert(VerifyAccess.isTypeVisible(c, typeof(Object))) : c.Name;
			return c.Name.Replace('.', '/');
		}

		/// <summary>
		/// Generate customized bytecode for a given LambdaForm.
		/// </summary>
		internal static MemberName GenerateCustomizedCode(LambdaForm form, MethodType invokerType)
		{
			InvokerBytecodeGenerator g = new InvokerBytecodeGenerator("MH", form, invokerType);
			return g.LoadMethod(g.GenerateCustomizedCodeBytes());
		}

		/// <summary>
		/// Generates code to check that actual receiver and LambdaForm matches </summary>
		private bool CheckActualReceiver()
		{
			// Expects MethodHandle on the stack and actual receiver MethodHandle in slot #0
			Mv.visitInsn(Opcodes.DUP);
			Mv.visitVarInsn(Opcodes.ALOAD, LocalsMap[0]);
			Mv.visitMethodInsn(Opcodes.INVOKESTATIC, MHI, "assertSame", LLV_SIG, false);
			return true;
		}

		/// <summary>
		/// Generate an invoker method for the passed <seealso cref="LambdaForm"/>.
		/// </summary>
		private sbyte[] GenerateCustomizedCodeBytes()
		{
			ClassFilePrologue();

			// Suppress this method in backtraces displayed to the user.
			Mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", true);

			// Mark this method as a compiled LambdaForm
			Mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Compiled;", true);

			if (LambdaForm.ForceInline)
			{
				// Force inlining of this invoker method.
				Mv.visitAnnotation("Ljava/lang/invoke/ForceInline;", true);
			}
			else
			{
				Mv.visitAnnotation("Ljava/lang/invoke/DontInline;", true);
			}

			if (LambdaForm.Customized != null)
			{
				// Since LambdaForm is customized for a particular MethodHandle, it's safe to substitute
				// receiver MethodHandle (at slot #0) with an embedded constant and use it instead.
				// It enables more efficient code generation in some situations, since embedded constants
				// are compile-time constants for JIT compiler.
				Mv.visitLdcInsn(ConstantPlaceholder(LambdaForm.Customized));
				Mv.visitTypeInsn(Opcodes.CHECKCAST, MH);
				assert(CheckActualReceiver()); // expects MethodHandle on top of the stack
				Mv.visitVarInsn(Opcodes.ASTORE, LocalsMap[0]);
			}

			// iterate over the form's names, generating bytecode instructions for each
			// start iterating at the first name following the arguments
			Name onStack = null;
			for (int i = LambdaForm.Arity_Renamed; i < LambdaForm.Names.Length; i++)
			{
				Name name = LambdaForm.Names[i];

				EmitStoreResult(onStack);
				onStack = name; // unless otherwise modified below
				MethodHandleImpl.Intrinsic intr = name.function.intrinsicName();
				switch (intr)
				{
					case java.lang.invoke.MethodHandleImpl.Intrinsic.SELECT_ALTERNATIVE:
						Debug.Assert(IsSelectAlternative(i));
						if (PROFILE_GWT)
						{
							assert(name.arguments[0] is Name && NameRefersTo((Name)name.arguments[0], typeof(MethodHandleImpl), "profileBoolean"));
							Mv.visitAnnotation("Ljava/lang/invoke/InjectedProfile;", true);
						}
						onStack = EmitSelectAlternative(name, LambdaForm.Names[i + 1]);
						i++; // skip MH.invokeBasic of the selectAlternative result
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.GUARD_WITH_CATCH:
						Debug.Assert(IsGuardWithCatch(i));
						onStack = EmitGuardWithCatch(i);
						i = i + 2; // Jump to the end of GWC idiom
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.NEW_ARRAY:
						Class rtype = name.function.methodType().returnType();
						if (IsStaticallyNameable(rtype))
						{
							EmitNewArray(name);
							continue;
						}
						break;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.ARRAY_LOAD:
						EmitArrayLoad(name);
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.ARRAY_STORE:
						EmitArrayStore(name);
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.IDENTITY:
						assert(name.arguments.length == 1);
						EmitPushArguments(name);
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.ZERO:
						assert(name.arguments.length == 0);
						EmitConst(name.type.basicTypeWrapper().zero());
						continue;
					case java.lang.invoke.MethodHandleImpl.Intrinsic.NONE:
						// no intrinsic associated
						break;
					default:
						throw newInternalError("Unknown intrinsic: " + intr);
				}

				MemberName member = name.function.member();
				if (IsStaticallyInvocable(member))
				{
					EmitStaticInvoke(member, name);
				}
				else
				{
					EmitInvoke(name);
				}
			}

			// return statement
			EmitReturn(onStack);

			ClassFileEpilogue();
			BogusMethod(LambdaForm);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] classFile = cw.toByteArray();
			sbyte[] classFile = Cw.toByteArray();
			MaybeDump(ClassName, classFile);
			return classFile;
		}

		internal virtual void EmitArrayLoad(Name name)
		{
			EmitArrayOp(name, Opcodes.AALOAD);
		}
		internal virtual void EmitArrayStore(Name name)
		{
			EmitArrayOp(name, Opcodes.AASTORE);
		}

		internal virtual void EmitArrayOp(Name name, int arrayOpcode)
		{
			Debug.Assert(arrayOpcode == Opcodes.AALOAD || arrayOpcode == Opcodes.AASTORE);
			Class elementType = name.function.methodType().parameterType(0).ComponentType;
			Debug.Assert(elementType != null);
			EmitPushArguments(name);
			if (elementType.Primitive)
			{
				Wrapper w = Wrapper.forPrimitiveType(elementType);
				arrayOpcode = ArrayInsnOpcode(ArrayTypeCode(w), arrayOpcode);
			}
			Mv.visitInsn(arrayOpcode);
		}

		/// <summary>
		/// Emit an invoke for the given name.
		/// </summary>
		internal virtual void EmitInvoke(Name name)
		{
			assert(!IsLinkerMethodInvoke(name)); // should use the static path for these
			if (true)
			{
				// push receiver
				MethodHandle target = name.function.resolvedHandle;
				assert(target != null) : name.exprString();
				Mv.visitLdcInsn(ConstantPlaceholder(target));
				EmitReferenceCast(typeof(MethodHandle), target);
			}
			else
			{
				// load receiver
				EmitAloadInsn(0);
				EmitReferenceCast(typeof(MethodHandle), null);
				Mv.visitFieldInsn(Opcodes.GETFIELD, MH, "form", LF_SIG);
				Mv.visitFieldInsn(Opcodes.GETFIELD, LF, "names", LFN_SIG);
				// TODO more to come
			}

			// push arguments
			EmitPushArguments(name);

			// invocation
			MethodType type = name.function.methodType();
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", type.BasicType().ToMethodDescriptorString(), false);
		}

		private static Class[] STATICALLY_INVOCABLE_PACKAGES = new Class[] {typeof(object), typeof(Arrays), typeof(sun.misc.Unsafe)};

		internal static bool IsStaticallyInvocable(Name name)
		{
			return IsStaticallyInvocable(name.function.member());
		}

		internal static bool IsStaticallyInvocable(MemberName member)
		{
			if (member == null)
			{
				return false;
			}
			if (member.Constructor)
			{
				return false;
			}
			Class cls = member.DeclaringClass;
			if (cls.Array || cls.Primitive)
			{
				return false; // FIXME
			}
			if (cls.AnonymousClass || cls.LocalClass)
			{
				return false; // inner class of some sort
			}
			if (cls.ClassLoader != typeof(MethodHandle).ClassLoader)
			{
				return false; // not on BCP
			}
			if (ReflectUtil.isVMAnonymousClass(cls)) // FIXME: switch to supported API once it is added
			{
				return false;
			}
			MethodType mtype = member.MethodOrFieldType;
			if (!IsStaticallyNameable(mtype.ReturnType()))
			{
				return false;
			}
			foreach (Class ptype in mtype.ParameterArray())
			{
				if (!IsStaticallyNameable(ptype))
				{
					return false;
				}
			}
			if (!member.Private && VerifyAccess.isSamePackage(typeof(MethodHandle), cls))
			{
				return true; // in java.lang.invoke package
			}
			if (member.Public && IsStaticallyNameable(cls))
			{
				return true;
			}
			return false;
		}

		internal static bool IsStaticallyNameable(Class cls)
		{
			if (cls == typeof(Object))
			{
				return true;
			}
			while (cls.Array)
			{
				cls = cls.ComponentType;
			}
			if (cls.Primitive)
			{
				return true; // int[].class, for example
			}
			if (ReflectUtil.isVMAnonymousClass(cls)) // FIXME: switch to supported API once it is added
			{
				return false;
			}
			// could use VerifyAccess.isClassAccessible but the following is a safe approximation
			if (cls.ClassLoader != typeof(Object).ClassLoader)
			{
				return false;
			}
			if (VerifyAccess.isSamePackage(typeof(MethodHandle), cls))
			{
				return true;
			}
			if (!Modifier.isPublic(cls.Modifiers))
			{
				return false;
			}
			foreach (Class pkgcls in STATICALLY_INVOCABLE_PACKAGES)
			{
				if (VerifyAccess.isSamePackage(pkgcls, cls))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual void EmitStaticInvoke(Name name)
		{
			EmitStaticInvoke(name.function.member(), name);
		}

		/// <summary>
		/// Emit an invoke for the given name, using the MemberName directly.
		/// </summary>
		internal virtual void EmitStaticInvoke(MemberName member, Name name)
		{
			assert(member.Equals(name.function.member()));
			Class defc = member.DeclaringClass;
			String cname = GetInternalName(defc);
			String mname = member.Name;
			String mtype;
			sbyte refKind = member.ReferenceKind;
			if (refKind == REF_invokeSpecial)
			{
				// in order to pass the verifier, we need to convert this to invokevirtual in all cases
				assert(member.CanBeStaticallyBound()) : member;
				refKind = REF_invokeVirtual;
			}

			if (member.DeclaringClass.Interface && refKind == REF_invokeVirtual)
			{
				// Methods from Object declared in an interface can be resolved by JVM to invokevirtual kind.
				// Need to convert it back to invokeinterface to pass verification and make the invocation works as expected.
				refKind = REF_invokeInterface;
			}

			// push arguments
			EmitPushArguments(name);

			// invocation
			if (member.Method)
			{
				mtype = member.MethodType.ToMethodDescriptorString();
				Mv.visitMethodInsn(RefKindOpcode(refKind), cname, mname, mtype, member.DeclaringClass.Interface);
			}
			else
			{
				mtype = MethodType.ToFieldDescriptorString(member.FieldType);
				Mv.visitFieldInsn(RefKindOpcode(refKind), cname, mname, mtype);
			}
			// Issue a type assertion for the result, so we can avoid casts later.
			if (name.type == L_TYPE)
			{
				Class rtype = member.InvocationType.ReturnType();
				assert(!rtype.Primitive);
				if (rtype != typeof(Object) && !rtype.Interface)
				{
					AssertStaticType(rtype, name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void emitNewArray(Name name) throws InternalError
		internal virtual void EmitNewArray(Name name)
		{
			Class rtype = name.function.methodType().returnType();
			if (name.arguments.length == 0)
			{
				// The array will be a constant.
				Object emptyArray;
				try
				{
					emptyArray = name.function.resolvedHandle.invoke();
				}
				catch (Throwable ex)
				{
					throw newInternalError(ex);
				}
				assert(java.lang.reflect.Array.getLength(emptyArray) == 0);
				assert(emptyArray.GetType() == rtype); // exact typing
				Mv.visitLdcInsn(ConstantPlaceholder(emptyArray));
				EmitReferenceCast(rtype, emptyArray);
				return;
			}
			Class arrayElementType = rtype.ComponentType;
			assert(arrayElementType != null);
			EmitIconstInsn(name.arguments.length);
			int xas = Opcodes.AASTORE;
			if (!arrayElementType.Primitive)
			{
				Mv.visitTypeInsn(Opcodes.ANEWARRAY, GetInternalName(arrayElementType));
			}
			else
			{
				sbyte tc = ArrayTypeCode(Wrapper.forPrimitiveType(arrayElementType));
				xas = ArrayInsnOpcode(tc, xas);
				Mv.visitIntInsn(Opcodes.NEWARRAY, tc);
			}
			// store arguments
			for (int i = 0; i < name.arguments.length; i++)
			{
				Mv.visitInsn(Opcodes.DUP);
				EmitIconstInsn(i);
				EmitPushArgument(name, i);
				Mv.visitInsn(xas);
			}
			// the array is left on the stack
			AssertStaticType(rtype, name);
		}
		internal virtual int RefKindOpcode(sbyte refKind)
		{
			switch (refKind)
			{
			case REF_invokeVirtual:
				return Opcodes.INVOKEVIRTUAL;
			case REF_invokeStatic:
				return Opcodes.INVOKESTATIC;
			case REF_invokeSpecial:
				return Opcodes.INVOKESPECIAL;
			case REF_invokeInterface:
				return Opcodes.INVOKEINTERFACE;
			case REF_getField:
				return Opcodes.GETFIELD;
			case REF_putField:
				return Opcodes.PUTFIELD;
			case REF_getStatic:
				return Opcodes.GETSTATIC;
			case REF_putStatic:
				return Opcodes.PUTSTATIC;
			}
			throw new InternalError("refKind=" + refKind);
		}

		/// <summary>
		/// Check if MemberName is a call to a method named {@code name} in class {@code declaredClass}.
		/// </summary>
		private bool MemberRefersTo(MemberName member, Class declaringClass, String name)
		{
			return member != null && member.DeclaringClass == declaringClass && member.Name.Equals(name);
		}
		private bool NameRefersTo(Name name, Class declaringClass, String methodName)
		{
			return name.function != null && MemberRefersTo(name.function.member(), declaringClass, methodName);
		}

		/// <summary>
		/// Check if MemberName is a call to MethodHandle.invokeBasic.
		/// </summary>
		private bool IsInvokeBasic(Name name)
		{
			if (name.function == null)
			{
				return false;
			}
			if (name.arguments.length < 1)
			{
				return false; // must have MH argument
			}
			MemberName member = name.function.member();
			return MemberRefersTo(member, typeof(MethodHandle), "invokeBasic") && !member.Public && !member.Static;
		}

		/// <summary>
		/// Check if MemberName is a call to MethodHandle.linkToStatic, etc.
		/// </summary>
		private bool IsLinkerMethodInvoke(Name name)
		{
			if (name.function == null)
			{
				return false;
			}
			if (name.arguments.length < 1)
			{
				return false; // must have MH argument
			}
			MemberName member = name.function.member();
			return member != null && member.DeclaringClass == typeof(MethodHandle) && !member.Public && member.Static && member.Name.StartsWith("linkTo");
		}

		/// <summary>
		/// Check if i-th name is a call to MethodHandleImpl.selectAlternative.
		/// </summary>
		private bool IsSelectAlternative(int pos)
		{
			// selectAlternative idiom:
			//   t_{n}:L=MethodHandleImpl.selectAlternative(...)
			//   t_{n+1}:?=MethodHandle.invokeBasic(t_{n}, ...)
			if (pos + 1 >= LambdaForm.Names.Length)
			{
				return false;
			}
			Name name0 = LambdaForm.Names[pos];
			Name name1 = LambdaForm.Names[pos + 1];
			return NameRefersTo(name0, typeof(MethodHandleImpl), "selectAlternative") && IsInvokeBasic(name1) && name1.lastUseIndex(name0) == 0 && LambdaForm.LastUseIndex(name0) == pos + 1; // t_{n} is local: used only in t_{n+1} -  t_{n+1}:?=MethodHandle.invokeBasic(t_{n}, ...)
		}

		/// <summary>
		/// Check if i-th name is a start of GuardWithCatch idiom.
		/// </summary>
		private bool IsGuardWithCatch(int pos)
		{
			// GuardWithCatch idiom:
			//   t_{n}:L=MethodHandle.invokeBasic(...)
			//   t_{n+1}:L=MethodHandleImpl.guardWithCatch(*, *, *, t_{n});
			//   t_{n+2}:?=MethodHandle.invokeBasic(t_{n+1})
			if (pos + 2 >= LambdaForm.Names.Length)
			{
				return false;
			}
			Name name0 = LambdaForm.Names[pos];
			Name name1 = LambdaForm.Names[pos + 1];
			Name name2 = LambdaForm.Names[pos + 2];
			return NameRefersTo(name1, typeof(MethodHandleImpl), "guardWithCatch") && IsInvokeBasic(name0) && IsInvokeBasic(name2) && name1.lastUseIndex(name0) == 3 && LambdaForm.LastUseIndex(name0) == pos + 1 && name2.lastUseIndex(name1) == 1 && LambdaForm.LastUseIndex(name1) == pos + 2; // t_{n+1} is local: used only in t_{n+2} -  t_{n+2}:?=MethodHandle.invokeBasic(t_{n+1}) -  t_{n} is local: used only in t_{n+1} -  t_{n+1}:L=MethodHandleImpl.guardWithCatch(*, *, *, t_{n});
		}

		/// <summary>
		/// Emit bytecode for the selectAlternative idiom.
		/// 
		/// The pattern looks like (Cf. MethodHandleImpl.makeGuardWithTest):
		/// <blockquote><pre>{@code
		///   Lambda(a0:L,a1:I)=>{
		///     t2:I=foo.test(a1:I);
		///     t3:L=MethodHandleImpl.selectAlternative(t2:I,(MethodHandle(int)int),(MethodHandle(int)int));
		///     t4:I=MethodHandle.invokeBasic(t3:L,a1:I);t4:I}
		/// }</pre></blockquote>
		/// </summary>
		private Name EmitSelectAlternative(Name selectAlternativeName, Name invokeBasicName)
		{
			Debug.Assert(IsStaticallyInvocable(invokeBasicName));

			Name receiver = (Name) invokeBasicName.arguments[0];

			Label L_fallback = new Label();
			Label L_done = new Label();

			// load test result
			EmitPushArgument(selectAlternativeName, 0);

			// if_icmpne L_fallback
			Mv.visitJumpInsn(Opcodes.IFEQ, L_fallback);

			// invoke selectAlternativeName.arguments[1]
			Class[] preForkClasses = LocalClasses.clone();
			EmitPushArgument(selectAlternativeName, 1); // get 2nd argument of selectAlternative
			EmitAstoreInsn(receiver.index()); // store the MH in the receiver slot
			EmitStaticInvoke(invokeBasicName);

			// goto L_done
			Mv.visitJumpInsn(Opcodes.GOTO, L_done);

			// L_fallback:
			Mv.visitLabel(L_fallback);

			// invoke selectAlternativeName.arguments[2]
			System.Array.Copy(preForkClasses, 0, LocalClasses, 0, preForkClasses.Length);
			EmitPushArgument(selectAlternativeName, 2); // get 3rd argument of selectAlternative
			EmitAstoreInsn(receiver.index()); // store the MH in the receiver slot
			EmitStaticInvoke(invokeBasicName);

			// L_done:
			Mv.visitLabel(L_done);
			// for now do not bother to merge typestate; just reset to the dominator state
			System.Array.Copy(preForkClasses, 0, LocalClasses, 0, preForkClasses.Length);

			return invokeBasicName; // return what's on stack
		}

		/// <summary>
		/// Emit bytecode for the guardWithCatch idiom.
		///  
		/// The pattern looks like (Cf. MethodHandleImpl.makeGuardWithCatch):
		/// <blockquote><pre>{@code
		///  guardWithCatch=Lambda(a0:L,a1:L,a2:L,a3:L,a4:L,a5:L,a6:L,a7:L)=>{
		///    t8:L=MethodHandle.invokeBasic(a4:L,a6:L,a7:L);
		///    t9:L=MethodHandleImpl.guardWithCatch(a1:L,a2:L,a3:L,t8:L);
		///   t10:I=MethodHandle.invokeBasic(a5:L,t9:L);t10:I}
		/// }</pre></blockquote>
		///  
		/// It is compiled into bytecode equivalent of the following code:
		/// <blockquote><pre>{@code
		///  try {
		///      return a1.invokeBasic(a6, a7);
		///  } catch (Throwable e) {
		///      if (!a2.isInstance(e)) throw e;
		///      return a3.invokeBasic(ex, a6, a7);
		///  }}
		/// </summary>
		private Name EmitGuardWithCatch(int pos)
		{
			Name args = LambdaForm.Names[pos];
			Name invoker = LambdaForm.Names[pos + 1];
			Name result = LambdaForm.Names[pos + 2];

			Label L_startBlock = new Label();
			Label L_endBlock = new Label();
			Label L_handler = new Label();
			Label L_done = new Label();

			Class returnType = result.function.resolvedHandle.type().returnType();
			MethodType type = args.function.resolvedHandle.type().dropParameterTypes(0,1).changeReturnType(returnType);

			Mv.visitTryCatchBlock(L_startBlock, L_endBlock, L_handler, "java/lang/Throwable");

			// Normal case
			Mv.visitLabel(L_startBlock);
			// load target
			EmitPushArgument(invoker, 0);
			EmitPushArguments(args, 1); // skip 1st argument: method handle
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", type.BasicType().ToMethodDescriptorString(), false);
			Mv.visitLabel(L_endBlock);
			Mv.visitJumpInsn(Opcodes.GOTO, L_done);

			// Exceptional case
			Mv.visitLabel(L_handler);

			// Check exception's type
			Mv.visitInsn(Opcodes.DUP);
			// load exception class
			EmitPushArgument(invoker, 1);
			Mv.visitInsn(Opcodes.SWAP);
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, "java/lang/Class", "isInstance", "(Ljava/lang/Object;)Z", false);
			Label L_rethrow = new Label();
			Mv.visitJumpInsn(Opcodes.IFEQ, L_rethrow);

			// Invoke catcher
			// load catcher
			EmitPushArgument(invoker, 2);
			Mv.visitInsn(Opcodes.SWAP);
			EmitPushArguments(args, 1); // skip 1st argument: method handle
			MethodType catcherType = type.InsertParameterTypes(0, typeof(Throwable));
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", catcherType.BasicType().ToMethodDescriptorString(), false);
			Mv.visitJumpInsn(Opcodes.GOTO, L_done);

			Mv.visitLabel(L_rethrow);
			Mv.visitInsn(Opcodes.ATHROW);

			Mv.visitLabel(L_done);

			return result;
		}

		private void EmitPushArguments(Name args)
		{
			EmitPushArguments(args, 0);
		}

		private void EmitPushArguments(Name args, int start)
		{
			for (int i = start; i < args.arguments.length; i++)
			{
				EmitPushArgument(args, i);
			}
		}

		private void EmitPushArgument(Name name, int paramIndex)
		{
			Object arg = name.arguments[paramIndex];
			Class ptype = name.function.methodType().parameterType(paramIndex);
			EmitPushArgument(ptype, arg);
		}

		private void EmitPushArgument(Class ptype, Object arg)
		{
			BasicType bptype = basicType(ptype);
			if (arg is Name)
			{
				Name n = (Name) arg;
				EmitLoadInsn(n.type, n.index());
				EmitImplicitConversion(n.type, ptype, n);
			}
			else if ((arg == null || arg is String) && bptype == L_TYPE)
			{
				EmitConst(arg);
			}
			else
			{
				if (Wrapper.isWrapperType(arg.GetType()) && bptype != L_TYPE)
				{
					EmitConst(arg);
				}
				else
				{
					Mv.visitLdcInsn(ConstantPlaceholder(arg));
					EmitImplicitConversion(L_TYPE, ptype, arg);
				}
			}
		}

		/// <summary>
		/// Store the name to its local, if necessary.
		/// </summary>
		private void EmitStoreResult(Name name)
		{
			if (name != null && name.type != V_TYPE)
			{
				// non-void: actually assign
				EmitStoreInsn(name.type, name.index());
			}
		}

		/// <summary>
		/// Emits a return statement from a LF invoker. If required, the result type is cast to the correct return type.
		/// </summary>
		private void EmitReturn(Name onStack)
		{
			// return statement
			Class rclass = InvokerType.ReturnType();
			BasicType rtype = LambdaForm.ReturnType();
			assert(rtype == basicType(rclass)); // must agree
			if (rtype == V_TYPE)
			{
				// void
				Mv.visitInsn(Opcodes.RETURN);
				// it doesn't matter what rclass is; the JVM will discard any value
			}
			else
			{
				LambdaForm.Name rn = LambdaForm.Names[LambdaForm.Result];

				// put return value on the stack if it is not already there
				if (rn != onStack)
				{
					EmitLoadInsn(rtype, LambdaForm.Result);
				}

				EmitImplicitConversion(rtype, rclass, rn);

				// generate actual return statement
				EmitReturnInsn(rtype);
			}
		}

		/// <summary>
		/// Emit a type conversion bytecode casting from "from" to "to".
		/// </summary>
		private void EmitPrimCast(Wrapper from, Wrapper to)
		{
			// Here's how.
			// -   indicates forbidden
			// <-> indicates implicit
			//      to ----> boolean  byte     short    char     int      long     float    double
			// from boolean    <->        -        -        -        -        -        -        -
			//      byte        -       <->       i2s      i2c      <->      i2l      i2f      i2d
			//      short       -       i2b       <->      i2c      <->      i2l      i2f      i2d
			//      char        -       i2b       i2s      <->      <->      i2l      i2f      i2d
			//      int         -       i2b       i2s      i2c      <->      i2l      i2f      i2d
			//      long        -     l2i,i2b   l2i,i2s  l2i,i2c    l2i      <->      l2f      l2d
			//      float       -     f2i,i2b   f2i,i2s  f2i,i2c    f2i      f2l      <->      f2d
			//      double      -     d2i,i2b   d2i,i2s  d2i,i2c    d2i      d2l      d2f      <->
			if (from == to)
			{
				// no cast required, should be dead code anyway
				return;
			}
			if (from.SubwordOrInt)
			{
				// cast from {byte,short,char,int} to anything
				EmitI2X(to);
			}
			else
			{
				// cast from {long,float,double} to anything
				if (to.SubwordOrInt)
				{
					// cast to {byte,short,char,int}
					EmitX2I(from);
					if (to.bitWidth() < 32)
					{
						// targets other than int require another conversion
						EmitI2X(to);
					}
				}
				else
				{
					// cast to {long,float,double} - this is verbose
					bool error = false;
					switch (from)
					{
					case LONG:
						switch (to)
						{
						case FLOAT:
							Mv.visitInsn(Opcodes.L2F);
							break;
						case DOUBLE:
							Mv.visitInsn(Opcodes.L2D);
							break;
						default:
							error = true;
							break;
						}
						break;
					case FLOAT:
						switch (to)
						{
						case LONG :
							Mv.visitInsn(Opcodes.F2L);
							break;
						case DOUBLE:
							Mv.visitInsn(Opcodes.F2D);
							break;
						default:
							error = true;
							break;
						}
						break;
					case DOUBLE:
						switch (to)
						{
						case LONG :
							Mv.visitInsn(Opcodes.D2L);
							break;
						case FLOAT:
							Mv.visitInsn(Opcodes.D2F);
							break;
						default:
							error = true;
							break;
						}
						break;
					default:
						error = true;
						break;
					}
					if (error)
					{
						throw new IllegalStateException("unhandled prim cast: " + from + "2" + to);
					}
				}
			}
		}

		private void EmitI2X(Wrapper type)
		{
			switch (type)
			{
			case BYTE:
				Mv.visitInsn(Opcodes.I2B);
				break;
			case SHORT:
				Mv.visitInsn(Opcodes.I2S);
				break;
			case CHAR:
				Mv.visitInsn(Opcodes.I2C);
				break;
			case INT: // naught
					break;
			case LONG:
				Mv.visitInsn(Opcodes.I2L);
				break;
			case FLOAT:
				Mv.visitInsn(Opcodes.I2F);
				break;
			case DOUBLE:
				Mv.visitInsn(Opcodes.I2D);
				break;
			case BOOLEAN:
				// For compatibility with ValueConversions and explicitCastArguments:
				Mv.visitInsn(Opcodes.ICONST_1);
				Mv.visitInsn(Opcodes.IAND);
				break;
			default:
				throw new InternalError("unknown type: " + type);
			}
		}

		private void EmitX2I(Wrapper type)
		{
			switch (type)
			{
			case LONG:
				Mv.visitInsn(Opcodes.L2I);
				break;
			case FLOAT:
				Mv.visitInsn(Opcodes.F2I);
				break;
			case DOUBLE:
				Mv.visitInsn(Opcodes.D2I);
				break;
			default:
				throw new InternalError("unknown type: " + type);
			}
		}

		/// <summary>
		/// Generate bytecode for a LambdaForm.vmentry which calls interpretWithArguments.
		/// </summary>
		internal static MemberName GenerateLambdaFormInterpreterEntryPoint(String sig)
		{
			assert(isValidSignature(sig));
			String name = "interpret_" + signatureReturn(sig).basicTypeChar();
			MethodType type = signatureType(sig); // sig includes leading argument
			type = type.ChangeParameterType(0, typeof(MethodHandle));
			InvokerBytecodeGenerator g = new InvokerBytecodeGenerator("LFI", name, type);
			return g.LoadMethod(g.GenerateLambdaFormInterpreterEntryPointBytes());
		}

		private sbyte[] GenerateLambdaFormInterpreterEntryPointBytes()
		{
			ClassFilePrologue();

			// Suppress this method in backtraces displayed to the user.
			Mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", true);

			// Don't inline the interpreter entry.
			Mv.visitAnnotation("Ljava/lang/invoke/DontInline;", true);

			// create parameter array
			EmitIconstInsn(InvokerType.ParameterCount());
			Mv.visitTypeInsn(Opcodes.ANEWARRAY, "java/lang/Object");

			// fill parameter array
			for (int i = 0; i < InvokerType.ParameterCount(); i++)
			{
				Class ptype = InvokerType.ParameterType(i);
				Mv.visitInsn(Opcodes.DUP);
				EmitIconstInsn(i);
				EmitLoadInsn(basicType(ptype), i);
				// box if primitive type
				if (ptype.Primitive)
				{
					EmitBoxing(Wrapper.forPrimitiveType(ptype));
				}
				Mv.visitInsn(Opcodes.AASTORE);
			}
			// invoke
			EmitAloadInsn(0);
			Mv.visitFieldInsn(Opcodes.GETFIELD, MH, "form", "Ljava/lang/invoke/LambdaForm;");
			Mv.visitInsn(Opcodes.SWAP); // swap form and array; avoid local variable
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, LF, "interpretWithArguments", "([Ljava/lang/Object;)Ljava/lang/Object;", false);

			// maybe unbox
			Class rtype = InvokerType.ReturnType();
			if (rtype.Primitive && rtype != typeof(void))
			{
				EmitUnboxing(Wrapper.forPrimitiveType(rtype));
			}

			// return statement
			EmitReturnInsn(basicType(rtype));

			ClassFileEpilogue();
			BogusMethod(InvokerType);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] classFile = cw.toByteArray();
			sbyte[] classFile = Cw.toByteArray();
			MaybeDump(ClassName, classFile);
			return classFile;
		}

		/// <summary>
		/// Generate bytecode for a NamedFunction invoker.
		/// </summary>
		internal static MemberName GenerateNamedFunctionInvoker(MethodTypeForm typeForm)
		{
			MethodType invokerType = NamedFunction.INVOKER_METHOD_TYPE;
			String invokerName = "invoke_" + shortenSignature(basicTypeSignature(typeForm.ErasedType()));
			InvokerBytecodeGenerator g = new InvokerBytecodeGenerator("NFI", invokerName, invokerType);
			return g.LoadMethod(g.GenerateNamedFunctionInvokerImpl(typeForm));
		}

		private sbyte[] GenerateNamedFunctionInvokerImpl(MethodTypeForm typeForm)
		{
			MethodType dstType = typeForm.ErasedType();
			ClassFilePrologue();

			// Suppress this method in backtraces displayed to the user.
			Mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", true);

			// Force inlining of this invoker method.
			Mv.visitAnnotation("Ljava/lang/invoke/ForceInline;", true);

			// Load receiver
			EmitAloadInsn(0);

			// Load arguments from array
			for (int i = 0; i < dstType.ParameterCount(); i++)
			{
				EmitAloadInsn(1);
				EmitIconstInsn(i);
				Mv.visitInsn(Opcodes.AALOAD);

				// Maybe unbox
				Class dptype = dstType.ParameterType(i);
				if (dptype.Primitive)
				{
					Class sptype = dstType.BasicType().Wrap().ParameterType(i);
					Wrapper dstWrapper = Wrapper.forBasicType(dptype);
					Wrapper srcWrapper = dstWrapper.SubwordOrInt ? Wrapper.INT : dstWrapper; // narrow subword from int
					EmitUnboxing(srcWrapper);
					EmitPrimCast(srcWrapper, dstWrapper);
				}
			}

			// Invoke
			String targetDesc = dstType.BasicType().ToMethodDescriptorString();
			Mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", targetDesc, false);

			// Box primitive types
			Class rtype = dstType.ReturnType();
			if (rtype != typeof(void) && rtype.Primitive)
			{
				Wrapper srcWrapper = Wrapper.forBasicType(rtype);
				Wrapper dstWrapper = srcWrapper.SubwordOrInt ? Wrapper.INT : srcWrapper; // widen subword to int
				// boolean casts not allowed
				EmitPrimCast(srcWrapper, dstWrapper);
				EmitBoxing(dstWrapper);
			}

			// If the return type is void we return a null reference.
			if (rtype == typeof(void))
			{
				Mv.visitInsn(Opcodes.ACONST_NULL);
			}
			EmitReturnInsn(L_TYPE); // NOTE: NamedFunction invokers always return a reference value.

			ClassFileEpilogue();
			BogusMethod(dstType);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] classFile = cw.toByteArray();
			sbyte[] classFile = Cw.toByteArray();
			MaybeDump(ClassName, classFile);
			return classFile;
		}

		/// <summary>
		/// Emit a bogus method that just loads some string constants. This is to get the constants into the constant pool
		/// for debugging purposes.
		/// </summary>
		private void BogusMethod(params Object[] os)
		{
			if (DUMP_CLASS_FILES)
			{
				Mv = Cw.visitMethod(Opcodes.ACC_STATIC, "dummy", "()V", null, null);
				foreach (Object o in os)
				{
					Mv.visitLdcInsn(o.ToString());
					Mv.visitInsn(Opcodes.POP);
				}
				Mv.visitInsn(Opcodes.RETURN);
				Mv.visitMaxs(0, 0);
				Mv.visitEnd();
			}
		}
	}

}