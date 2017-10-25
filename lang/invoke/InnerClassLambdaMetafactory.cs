using System;

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
	using BytecodeDescriptor = sun.invoke.util.BytecodeDescriptor;
	using Unsafe = sun.misc.Unsafe;
	using GetPropertyAction = sun.security.action.GetPropertyAction;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static jdk.@internal.org.objectweb.asm.Opcodes.*;

	/// <summary>
	/// Lambda metafactory implementation which dynamically creates an
	/// inner-class-like class per lambda callsite.
	/// </summary>
	/// <seealso cref= LambdaMetafactory </seealso>
	/* package */	 internal sealed class InnerClassLambdaMetafactory : AbstractValidatingLambdaMetafactory
	 {
		private static readonly Unsafe UNSAFE = Unsafe.Unsafe;

		private const int CLASSFILE_VERSION = 52;
		private static readonly String METHOD_DESCRIPTOR_VOID = Type.getMethodDescriptor(Type.VOID_TYPE);
		private const String JAVA_LANG_OBJECT = "java/lang/Object";
		private const String NAME_CTOR = "<init>";
		private const String NAME_FACTORY = "get$Lambda";

		//Serialization support
		private const String NAME_SERIALIZED_LAMBDA = "java/lang/invoke/SerializedLambda";
		private const String NAME_NOT_SERIALIZABLE_EXCEPTION = "java/io/NotSerializableException";
		private const String DESCR_METHOD_WRITE_REPLACE = "()Ljava/lang/Object;";
		private const String DESCR_METHOD_WRITE_OBJECT = "(Ljava/io/ObjectOutputStream;)V";
		private const String DESCR_METHOD_READ_OBJECT = "(Ljava/io/ObjectInputStream;)V";
		private const String NAME_METHOD_WRITE_REPLACE = "writeReplace";
		private const String NAME_METHOD_READ_OBJECT = "readObject";
		private const String NAME_METHOD_WRITE_OBJECT = "writeObject";
		private static readonly String DESCR_CTOR_SERIALIZED_LAMBDA = MethodType.methodType(typeof(void), typeof(Class), typeof(String), typeof(String), typeof(String), typeof(int), typeof(String), typeof(String), typeof(String), typeof(String), typeof(Object[])).toMethodDescriptorString();
		private static readonly String DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION = MethodType.MethodType(typeof(void), typeof(String)).ToMethodDescriptorString();
		private static readonly String[] SER_HOSTILE_EXCEPTIONS = new String[] {NAME_NOT_SERIALIZABLE_EXCEPTION};


		private static readonly String[] EMPTY_STRING_ARRAY = new String[0];

		// Used to ensure that each spun class name is unique
		private static readonly AtomicInteger Counter = new AtomicInteger(0);

		// For dumping generated classes to disk, for debugging purposes
		private static readonly ProxyClassesDumper Dumper;

		static InnerClassLambdaMetafactory()
		{
			const String key = "jdk.internal.lambda.dumpProxyClasses";
			String path = AccessController.DoPrivileged(new GetPropertyAction(key), null, new PropertyPermission(key, "read"));
			Dumper = (null == path) ? null : ProxyClassesDumper.GetInstance(path);
		}

		// See context values in AbstractValidatingLambdaMetafactory
		private readonly String ImplMethodClassName; // Name of type containing implementation "CC"
		private readonly String ImplMethodName; // Name of implementation method "impl"
		private readonly String ImplMethodDesc; // Type descriptor for implementation methods "(I)Ljava/lang/String;"
		private readonly Class ImplMethodReturnClass; // class for implementaion method return type "Ljava/lang/String;"
		private readonly MethodType ConstructorType; // Generated class constructor type "(CC)void"
		private readonly ClassWriter Cw; // ASM class writer
		private readonly String[] ArgNames; // Generated names for the constructor arguments
		private readonly String[] ArgDescs; // Type descriptors for the constructor arguments
		private readonly String LambdaClassName; // Generated name for the generated class "X$$Lambda$1"

		/// <summary>
		/// General meta-factory constructor, supporting both standard cases and
		/// allowing for uncommon options such as serialization or bridging.
		/// </summary>
		/// <param name="caller"> Stacked automatically by VM; represents a lookup context
		///               with the accessibility privileges of the caller. </param>
		/// <param name="invokedType"> Stacked automatically by VM; the signature of the
		///                    invoked method, which includes the expected static
		///                    type of the returned lambda object, and the static
		///                    types of the captured arguments for the lambda.  In
		///                    the event that the implementation method is an
		///                    instance method, the first argument in the invocation
		///                    signature will correspond to the receiver. </param>
		/// <param name="samMethodName"> Name of the method in the functional interface to
		///                      which the lambda or method reference is being
		///                      converted, represented as a String. </param>
		/// <param name="samMethodType"> Type of the method in the functional interface to
		///                      which the lambda or method reference is being
		///                      converted, represented as a MethodType. </param>
		/// <param name="implMethod"> The implementation method which should be called (with
		///                   suitable adaptation of argument types, return types,
		///                   and adjustment for captured arguments) when methods of
		///                   the resulting functional interface instance are invoked. </param>
		/// <param name="instantiatedMethodType"> The signature of the primary functional
		///                               interface method after type variables are
		///                               substituted with their instantiation from
		///                               the capture site </param>
		/// <param name="isSerializable"> Should the lambda be made serializable?  If set,
		///                       either the target type or one of the additional SAM
		///                       types must extend {@code Serializable}. </param>
		/// <param name="markerInterfaces"> Additional interfaces which the lambda object
		///                       should implement. </param>
		/// <param name="additionalBridges"> Method types for additional signatures to be
		///                          bridged to the implementation method </param>
		/// <exception cref="LambdaConversionException"> If any of the meta-factory protocol
		/// invariants are violated </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InnerClassLambdaMetafactory(MethodHandles.Lookup caller, MethodType invokedType, String samMethodName, MethodType samMethodType, MethodHandle implMethod, MethodType instantiatedMethodType, boolean isSerializable, Class[] markerInterfaces, MethodType[] additionalBridges) throws LambdaConversionException
		public InnerClassLambdaMetafactory(MethodHandles.Lookup caller, MethodType invokedType, String samMethodName, MethodType samMethodType, MethodHandle implMethod, MethodType instantiatedMethodType, bool isSerializable, Class[] markerInterfaces, MethodType[] additionalBridges) : base(caller, invokedType, samMethodName, samMethodType, implMethod, instantiatedMethodType, isSerializable, markerInterfaces, additionalBridges)
		{
			ImplMethodClassName = ImplDefiningClass.Name.Replace('.', '/');
			ImplMethodName = ImplInfo.Name;
			ImplMethodDesc = ImplMethodType.ToMethodDescriptorString();
			ImplMethodReturnClass = (ImplKind == MethodHandleInfo.REF_newInvokeSpecial) ? ImplDefiningClass : ImplMethodType.ReturnType();
			ConstructorType = invokedType.ChangeReturnType(Void.TYPE);
			LambdaClassName = TargetClass.Name.Replace('.', '/') + "$$Lambda$" + Counter.IncrementAndGet();
			Cw = new ClassWriter(ClassWriter.COMPUTE_MAXS);
			int parameterCount = invokedType.ParameterCount();
			if (parameterCount > 0)
			{
				ArgNames = new String[parameterCount];
				ArgDescs = new String[parameterCount];
				for (int i = 0; i < parameterCount; i++)
				{
					ArgNames[i] = "arg$" + (i + 1);
					ArgDescs[i] = BytecodeDescriptor.unparse(invokedType.ParameterType(i));
				}
			}
			else
			{
				ArgNames = ArgDescs = EMPTY_STRING_ARRAY;
			}
		}

		/// <summary>
		/// Build the CallSite. Generate a class file which implements the functional
		/// interface, define the class, if there are no parameters create an instance
		/// of the class which the CallSite will return, otherwise, generate handles
		/// which will call the class' constructor.
		/// </summary>
		/// <returns> a CallSite, which, when invoked, will return an instance of the
		/// functional interface </returns>
		/// <exception cref="ReflectiveOperationException"> </exception>
		/// <exception cref="LambdaConversionException"> If properly formed functional interface
		/// is not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override CallSite buildCallSite() throws LambdaConversionException
		internal override CallSite BuildCallSite()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class innerClass = spinInnerClass();
			Class innerClass = SpinInnerClass();
			if (InvokedType.ParameterCount() == 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Constructor<?>[] ctrs = java.security.AccessController.doPrivileged(new java.security.PrivilegedAction<Constructor<?>[]>()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Constructor<?>[] ctrs = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, innerClass));
				if (ctrs.Length != 1)
				{
					throw new LambdaConversionException("Expected one lambda constructor for " + innerClass.CanonicalName + ", got " + ctrs.Length);
				}

				try
				{
					Object inst = ctrs[0].newInstance();
					return new ConstantCallSite(MethodHandles.Constant(SamBase, inst));
				}
				catch (ReflectiveOperationException e)
				{
					throw new LambdaConversionException("Exception instantiating lambda object", e);
				}
			}
			else
			{
				try
				{
					UNSAFE.ensureClassInitialized(innerClass);
					return new ConstantCallSite(MethodHandles.Lookup.IMPL_LOOKUP.FindStatic(innerClass, NAME_FACTORY, InvokedType));
				}
				catch (ReflectiveOperationException e)
				{
					throw new LambdaConversionException("Exception finding constructor", e);
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Constructor<JavaToDotNetGenericWildcard>[]>
		{
			private readonly InnerClassLambdaMetafactory OuterInstance;

			private Type InnerClass;

			public PrivilegedActionAnonymousInnerClassHelper(InnerClassLambdaMetafactory outerInstance, Type innerClass)
			{
				this.OuterInstance = outerInstance;
				this.InnerClass = innerClass;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Constructor<?>[] run()
			public virtual Constructor<?>[] Run()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?>[] ctrs = innerClass.getDeclaredConstructors();
				Constructor<?>[] ctrs = InnerClass.DeclaredConstructors;
				if (ctrs.Length == 1)
				{
					// The lambda implementing inner class constructor is private, set
					// it accessible (by us) before creating the constant sole instance
					ctrs[0].Accessible = true;
				}
				return ctrs;
			}
		}

		/// <summary>
		/// Generate a class file which implements the functional
		/// interface, define and return the class.
		/// 
		/// @implNote The class that is generated does not include signature
		/// information for exceptions that may be present on the SAM method.
		/// This is to reduce classfile size, and is harmless as checked exceptions
		/// are erased anyway, no one will ever compile against this classfile,
		/// and we make no guarantees about the reflective properties of lambda
		/// objects.
		/// </summary>
		/// <returns> a Class which implements the functional interface </returns>
		/// <exception cref="LambdaConversionException"> If properly formed functional interface
		/// is not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class spinInnerClass() throws LambdaConversionException
		private Class SpinInnerClass()
		{
			String[] interfaces;
			String samIntf = SamBase.Name.Replace('.', '/');
			bool accidentallySerializable = !IsSerializable && SamBase.IsSubclassOf(typeof(Serializable));
			if (MarkerInterfaces.Length == 0)
			{
				interfaces = new String[]{samIntf};
			}
			else
			{
				// Assure no duplicate interfaces (ClassFormatError)
				Set<String> itfs = new LinkedHashSet<String>(MarkerInterfaces.Length + 1);
				itfs.Add(samIntf);
				foreach (Class markerInterface in MarkerInterfaces)
				{
					itfs.Add(markerInterface.Name.Replace('.', '/'));
					accidentallySerializable |= !IsSerializable && markerInterface.IsSubclassOf(typeof(Serializable));
				}
				interfaces = itfs.ToArray(new String[itfs.Count]);
			}

			Cw.visit(CLASSFILE_VERSION, ACC_SUPER + ACC_FINAL + ACC_SYNTHETIC, LambdaClassName, null, JAVA_LANG_OBJECT, interfaces);

			// Generate final fields to be filled in by constructor
			for (int i = 0; i < ArgDescs.Length; i++)
			{
				FieldVisitor fv = Cw.visitField(ACC_PRIVATE + ACC_FINAL, ArgNames[i], ArgDescs[i], null, null);
				fv.visitEnd();
			}

			GenerateConstructor();

			if (InvokedType.ParameterCount() != 0)
			{
				GenerateFactory();
			}

			// Forward the SAM method
			MethodVisitor mv = Cw.visitMethod(ACC_PUBLIC, SamMethodName, SamMethodType.ToMethodDescriptorString(), null, null);
			mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", true);
			(new ForwardingMethodGenerator(this, mv)).Generate(SamMethodType);

			// Forward the bridges
			if (AdditionalBridges != null)
			{
				foreach (MethodType mt in AdditionalBridges)
				{
					mv = Cw.visitMethod(ACC_PUBLIC | ACC_BRIDGE, SamMethodName, mt.ToMethodDescriptorString(), null, null);
					mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", true);
					(new ForwardingMethodGenerator(this, mv)).Generate(mt);
				}
			}

			if (IsSerializable)
			{
				GenerateSerializationFriendlyMethods();
			}
			else if (accidentallySerializable)
			{
				GenerateSerializationHostileMethods();
			}

			Cw.visitEnd();

			// Define the generated class in this VM.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] classBytes = cw.toByteArray();
			sbyte[] classBytes = Cw.toByteArray();

			// If requested, dump out to a file for debugging purposes
			if (Dumper != null)
			{
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this, classBytes), null,
				new FilePermission("<<ALL FILES>>", "read, write"), new PropertyPermission("user.dir", "read"));
				// createDirectories may need it
			}

			return UNSAFE.defineAnonymousClass(TargetClass, classBytes, null);
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private readonly InnerClassLambdaMetafactory OuterInstance;

			private sbyte[] ClassBytes;

			public PrivilegedActionAnonymousInnerClassHelper2(InnerClassLambdaMetafactory outerInstance, sbyte[] classBytes)
			{
				this.OuterInstance = outerInstance;
				this.ClassBytes = classBytes;
			}

			public virtual Void Run()
			{
				Dumper.DumpClass(OuterInstance.LambdaClassName, ClassBytes);
				return null;
			}
		}

		/// <summary>
		/// Generate the factory method for the class
		/// </summary>
		private void GenerateFactory()
		{
			MethodVisitor m = Cw.visitMethod(ACC_PRIVATE | ACC_STATIC, NAME_FACTORY, InvokedType.ToMethodDescriptorString(), null, null);
			m.visitCode();
			m.visitTypeInsn(NEW, LambdaClassName);
			m.visitInsn(Opcodes.DUP);
			int parameterCount = InvokedType.ParameterCount();
			for (int typeIndex = 0, varIndex = 0; typeIndex < parameterCount; typeIndex++)
			{
				Class argType = InvokedType.ParameterType(typeIndex);
				m.visitVarInsn(GetLoadOpcode(argType), varIndex);
				varIndex += GetParameterSize(argType);
			}
			m.visitMethodInsn(INVOKESPECIAL, LambdaClassName, NAME_CTOR, ConstructorType.ToMethodDescriptorString(), false);
			m.visitInsn(ARETURN);
			m.visitMaxs(-1, -1);
			m.visitEnd();
		}

		/// <summary>
		/// Generate the constructor for the class
		/// </summary>
		private void GenerateConstructor()
		{
			// Generate constructor
			MethodVisitor ctor = Cw.visitMethod(ACC_PRIVATE, NAME_CTOR, ConstructorType.ToMethodDescriptorString(), null, null);
			ctor.visitCode();
			ctor.visitVarInsn(ALOAD, 0);
			ctor.visitMethodInsn(INVOKESPECIAL, JAVA_LANG_OBJECT, NAME_CTOR, METHOD_DESCRIPTOR_VOID, false);
			int parameterCount = InvokedType.ParameterCount();
			for (int i = 0, lvIndex = 0; i < parameterCount; i++)
			{
				ctor.visitVarInsn(ALOAD, 0);
				Class argType = InvokedType.ParameterType(i);
				ctor.visitVarInsn(GetLoadOpcode(argType), lvIndex + 1);
				lvIndex += GetParameterSize(argType);
				ctor.visitFieldInsn(PUTFIELD, LambdaClassName, ArgNames[i], ArgDescs[i]);
			}
			ctor.visitInsn(RETURN);
			// Maxs computed by ClassWriter.COMPUTE_MAXS, these arguments ignored
			ctor.visitMaxs(-1, -1);
			ctor.visitEnd();
		}

		/// <summary>
		/// Generate a writeReplace method that supports serialization
		/// </summary>
		private void GenerateSerializationFriendlyMethods()
		{
			TypeConvertingMethodAdapter mv = new TypeConvertingMethodAdapter(Cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_WRITE_REPLACE, DESCR_METHOD_WRITE_REPLACE, null, null));

			mv.visitCode();
			mv.visitTypeInsn(NEW, NAME_SERIALIZED_LAMBDA);
			mv.visitInsn(DUP);
			mv.visitLdcInsn(Type.GetType(TargetClass));
			mv.visitLdcInsn(InvokedType.ReturnType().Name.Replace('.', '/'));
			mv.visitLdcInsn(SamMethodName);
			mv.visitLdcInsn(SamMethodType.ToMethodDescriptorString());
			mv.visitLdcInsn(ImplInfo.ReferenceKind);
			mv.visitLdcInsn(ImplInfo.DeclaringClass.Name.Replace('.', '/'));
			mv.visitLdcInsn(ImplInfo.Name);
			mv.visitLdcInsn(ImplInfo.MethodType.ToMethodDescriptorString());
			mv.visitLdcInsn(InstantiatedMethodType.ToMethodDescriptorString());
			mv.Iconst(ArgDescs.Length);
			mv.visitTypeInsn(ANEWARRAY, JAVA_LANG_OBJECT);
			for (int i = 0; i < ArgDescs.Length; i++)
			{
				mv.visitInsn(DUP);
				mv.Iconst(i);
				mv.visitVarInsn(ALOAD, 0);
				mv.visitFieldInsn(GETFIELD, LambdaClassName, ArgNames[i], ArgDescs[i]);
				mv.BoxIfTypePrimitive(Type.GetType(ArgDescs[i]));
				mv.visitInsn(AASTORE);
			}
			mv.visitMethodInsn(INVOKESPECIAL, NAME_SERIALIZED_LAMBDA, NAME_CTOR, DESCR_CTOR_SERIALIZED_LAMBDA, false);
			mv.visitInsn(ARETURN);
			// Maxs computed by ClassWriter.COMPUTE_MAXS, these arguments ignored
			mv.visitMaxs(-1, -1);
			mv.visitEnd();
		}

		/// <summary>
		/// Generate a readObject/writeObject method that is hostile to serialization
		/// </summary>
		private void GenerateSerializationHostileMethods()
		{
			MethodVisitor mv = Cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_WRITE_OBJECT, DESCR_METHOD_WRITE_OBJECT, null, SER_HOSTILE_EXCEPTIONS);
			mv.visitCode();
			mv.visitTypeInsn(NEW, NAME_NOT_SERIALIZABLE_EXCEPTION);
			mv.visitInsn(DUP);
			mv.visitLdcInsn("Non-serializable lambda");
			mv.visitMethodInsn(INVOKESPECIAL, NAME_NOT_SERIALIZABLE_EXCEPTION, NAME_CTOR, DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION, false);
			mv.visitInsn(ATHROW);
			mv.visitMaxs(-1, -1);
			mv.visitEnd();

			mv = Cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_READ_OBJECT, DESCR_METHOD_READ_OBJECT, null, SER_HOSTILE_EXCEPTIONS);
			mv.visitCode();
			mv.visitTypeInsn(NEW, NAME_NOT_SERIALIZABLE_EXCEPTION);
			mv.visitInsn(DUP);
			mv.visitLdcInsn("Non-serializable lambda");
			mv.visitMethodInsn(INVOKESPECIAL, NAME_NOT_SERIALIZABLE_EXCEPTION, NAME_CTOR, DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION, false);
			mv.visitInsn(ATHROW);
			mv.visitMaxs(-1, -1);
			mv.visitEnd();
		}

		/// <summary>
		/// This class generates a method body which calls the lambda implementation
		/// method, converting arguments, as needed.
		/// </summary>
		private class ForwardingMethodGenerator : TypeConvertingMethodAdapter
		{
			private readonly InnerClassLambdaMetafactory OuterInstance;


			internal ForwardingMethodGenerator(InnerClassLambdaMetafactory outerInstance, MethodVisitor mv) : base(mv)
			{
				this.OuterInstance = outerInstance;
			}

			internal virtual void Generate(MethodType methodType)
			{
				visitCode();

				if (outerInstance.ImplKind == MethodHandleInfo.REF_newInvokeSpecial)
				{
					visitTypeInsn(NEW, outerInstance.ImplMethodClassName);
					visitInsn(DUP);
				}
				for (int i = 0; i < outerInstance.ArgNames.Length; i++)
				{
					visitVarInsn(ALOAD, 0);
					visitFieldInsn(GETFIELD, outerInstance.LambdaClassName, outerInstance.ArgNames[i], outerInstance.ArgDescs[i]);
				}

				ConvertArgumentTypes(methodType);

				// Invoke the method we want to forward to
				visitMethodInsn(InvocationOpcode(), outerInstance.ImplMethodClassName, outerInstance.ImplMethodName, outerInstance.ImplMethodDesc, outerInstance.ImplDefiningClass.Interface);

				// Convert the return value (if any) and return it
				// Note: if adapting from non-void to void, the 'return'
				// instruction will pop the unneeded result
				Class samReturnClass = methodType.ReturnType();
				ConvertType(outerInstance.ImplMethodReturnClass, samReturnClass, samReturnClass);
				visitInsn(GetReturnOpcode(samReturnClass));
				// Maxs computed by ClassWriter.COMPUTE_MAXS,these arguments ignored
				visitMaxs(-1, -1);
				visitEnd();
			}

			internal virtual void ConvertArgumentTypes(MethodType samType)
			{
				int lvIndex = 0;
				bool samIncludesReceiver = outerInstance.ImplIsInstanceMethod && outerInstance.InvokedType.ParameterCount() == 0;
				int samReceiverLength = samIncludesReceiver ? 1 : 0;
				if (samIncludesReceiver)
				{
					// push receiver
					Class rcvrType = samType.ParameterType(0);
					visitVarInsn(GetLoadOpcode(rcvrType), lvIndex + 1);
					lvIndex += GetParameterSize(rcvrType);
					ConvertType(rcvrType, outerInstance.ImplDefiningClass, outerInstance.InstantiatedMethodType.ParameterType(0));
				}
				int samParametersLength = samType.ParameterCount();
				int argOffset = outerInstance.ImplMethodType.ParameterCount() - samParametersLength;
				for (int i = samReceiverLength; i < samParametersLength; i++)
				{
					Class argType = samType.ParameterType(i);
					visitVarInsn(GetLoadOpcode(argType), lvIndex + 1);
					lvIndex += GetParameterSize(argType);
					ConvertType(argType, outerInstance.ImplMethodType.ParameterType(argOffset + i), outerInstance.InstantiatedMethodType.ParameterType(i));
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int invocationOpcode() throws InternalError
			internal virtual int InvocationOpcode()
			{
				switch (outerInstance.ImplKind)
				{
					case MethodHandleInfo.REF_invokeStatic:
						return INVOKESTATIC;
					case MethodHandleInfo.REF_newInvokeSpecial:
						return INVOKESPECIAL;
					 case MethodHandleInfo.REF_invokeVirtual:
						return INVOKEVIRTUAL;
					case MethodHandleInfo.REF_invokeInterface:
						return INVOKEINTERFACE;
					case MethodHandleInfo.REF_invokeSpecial:
						return INVOKESPECIAL;
					default:
						throw new InternalError("Unexpected invocation kind: " + outerInstance.ImplKind);
				}
			}
		}

		internal static int GetParameterSize(Class c)
		{
			if (c == Void.TYPE)
			{
				return 0;
			}
			else if (c == Long.TYPE || c == Double.TYPE)
			{
				return 2;
			}
			return 1;
		}

		internal static int GetLoadOpcode(Class c)
		{
			if (c == Void.TYPE)
			{
				throw new InternalError("Unexpected void type of load opcode");
			}
			return ILOAD + GetOpcodeOffset(c);
		}

		internal static int GetReturnOpcode(Class c)
		{
			if (c == Void.TYPE)
			{
				return RETURN;
			}
			return IRETURN + GetOpcodeOffset(c);
		}

		private static int GetOpcodeOffset(Class c)
		{
			if (c.Primitive)
			{
				if (c == Long.TYPE)
				{
					return 1;
				}
				else if (c == Float.TYPE)
				{
					return 2;
				}
				else if (c == Double.TYPE)
				{
					return 3;
				}
				return 0;
			}
			else
			{
				return 4;
			}
		}

	 }

}