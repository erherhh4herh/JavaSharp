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

	using Wrapper = sun.invoke.util.Wrapper;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.invoke.util.Wrapper.forPrimitiveType;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.invoke.util.Wrapper.forWrapperType;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.invoke.util.Wrapper.isWrapperType;

	/// <summary>
	/// Abstract implementation of a lambda metafactory which provides parameter
	/// unrolling and input validation.
	/// </summary>
	/// <seealso cref= LambdaMetafactory </seealso>
	/* package */	 internal abstract class AbstractValidatingLambdaMetafactory
	 {

		/*
		 * For context, the comments for the following fields are marked in quotes
		 * with their values, given this program:
		 * interface II<T> {  Object foo(T x); }
		 * interface JJ<R extends Number> extends II<R> { }
		 * class CC {  String impl(int i) { return "impl:"+i; }}
		 * class X {
		 *     public static void main(String[] args) {
		 *         JJ<Integer> iii = (new CC())::impl;
		 *         System.out.printf(">>> %s\n", iii.foo(44));
		 * }}
		 */
		internal readonly Class TargetClass; // The class calling the meta-factory via invokedynamic "class X"
		internal readonly MethodType InvokedType; // The type of the invoked method "(CC)II"
		internal readonly Class SamBase; // The type of the returned instance "interface JJ"
		internal readonly String SamMethodName; // Name of the SAM method "foo"
		internal readonly MethodType SamMethodType; // Type of the SAM method "(Object)Object"
		internal readonly MethodHandle ImplMethod; // Raw method handle for the implementation method
		internal readonly MethodHandleInfo ImplInfo; // Info about the implementation method handle "MethodHandleInfo[5 CC.impl(int)String]"
		internal readonly int ImplKind; // Invocation kind for implementation "5"=invokevirtual
		internal readonly bool ImplIsInstanceMethod; // Is the implementation an instance method "true"
		internal readonly Class ImplDefiningClass; // Type defining the implementation "class CC"
		internal readonly MethodType ImplMethodType; // Type of the implementation method "(int)String"
		internal readonly MethodType InstantiatedMethodType; // Instantiated erased functional interface method type "(Integer)Object"
		internal readonly bool IsSerializable; // Should the returned instance be serializable
		internal readonly Class[] MarkerInterfaces; // Additional marker interfaces to be implemented
		internal readonly MethodType[] AdditionalBridges; // Signatures of additional methods to bridge


		/// <summary>
		/// Meta-factory constructor.
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
		/// <param name="implMethod"> The implementation method which should be called
		///                   (with suitable adaptation of argument types, return
		///                   types, and adjustment for captured arguments) when
		///                   methods of the resulting functional interface instance
		///                   are invoked. </param>
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
//ORIGINAL LINE: AbstractValidatingLambdaMetafactory(MethodHandles.Lookup caller, MethodType invokedType, String samMethodName, MethodType samMethodType, MethodHandle implMethod, MethodType instantiatedMethodType, boolean isSerializable, Class[] markerInterfaces, MethodType[] additionalBridges) throws LambdaConversionException
		internal AbstractValidatingLambdaMetafactory(MethodHandles.Lookup caller, MethodType invokedType, String samMethodName, MethodType samMethodType, MethodHandle implMethod, MethodType instantiatedMethodType, bool isSerializable, Class[] markerInterfaces, MethodType[] additionalBridges)
		{
			if ((caller.LookupModes() & MethodHandles.Lookup.PRIVATE) == 0)
			{
				throw new LambdaConversionException(string.Format("Invalid caller: {0}", caller.LookupClass().Name));
			}
			this.TargetClass = caller.LookupClass();
			this.InvokedType = invokedType;

			this.SamBase = invokedType.ReturnType();

			this.SamMethodName = samMethodName;
			this.SamMethodType = samMethodType;

			this.ImplMethod = implMethod;
			this.ImplInfo = caller.RevealDirect(implMethod);
			this.ImplKind = ImplInfo.ReferenceKind;
			this.ImplIsInstanceMethod = ImplKind == MethodHandleInfo.REF_invokeVirtual || ImplKind == MethodHandleInfo.REF_invokeSpecial || ImplKind == MethodHandleInfo.REF_invokeInterface;
			this.ImplDefiningClass = ImplInfo.DeclaringClass;
			this.ImplMethodType = ImplInfo.MethodType;
			this.InstantiatedMethodType = instantiatedMethodType;
			this.IsSerializable = isSerializable;
			this.MarkerInterfaces = markerInterfaces;
			this.AdditionalBridges = additionalBridges;

			if (!SamBase.Interface)
			{
				throw new LambdaConversionException(string.Format("Functional interface {0} is not an interface", SamBase.Name));
			}

			foreach (Class c in markerInterfaces)
			{
				if (!c.Interface)
				{
					throw new LambdaConversionException(string.Format("Marker interface {0} is not an interface", c.Name));
				}
			}
		}

		/// <summary>
		/// Build the CallSite.
		/// </summary>
		/// <returns> a CallSite, which, when invoked, will return an instance of the
		/// functional interface </returns>
		/// <exception cref="ReflectiveOperationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: abstract CallSite buildCallSite() throws LambdaConversionException;
		internal abstract CallSite BuildCallSite();

		/// <summary>
		/// Check the meta-factory arguments for errors </summary>
		/// <exception cref="LambdaConversionException"> if there are improper conversions </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void validateMetafactoryArgs() throws LambdaConversionException
		internal virtual void ValidateMetafactoryArgs()
		{
			switch (ImplKind)
			{
				case MethodHandleInfo.REF_invokeInterface:
				case MethodHandleInfo.REF_invokeVirtual:
				case MethodHandleInfo.REF_invokeStatic:
				case MethodHandleInfo.REF_newInvokeSpecial:
				case MethodHandleInfo.REF_invokeSpecial:
					break;
				default:
					throw new LambdaConversionException(string.Format("Unsupported MethodHandle kind: {0}", ImplInfo));
			}

			// Check arity: optional-receiver + captured + SAM == impl
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int implArity = implMethodType.parameterCount();
			int implArity = ImplMethodType.ParameterCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int receiverArity = implIsInstanceMethod ? 1 : 0;
			int receiverArity = ImplIsInstanceMethod ? 1 : 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int capturedArity = invokedType.parameterCount();
			int capturedArity = InvokedType.ParameterCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int samArity = samMethodType.parameterCount();
			int samArity = SamMethodType.ParameterCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int instantiatedArity = instantiatedMethodType.parameterCount();
			int instantiatedArity = InstantiatedMethodType.ParameterCount();
			if (implArity + receiverArity != capturedArity + samArity)
			{
				throw new LambdaConversionException(string.Format("Incorrect number of parameters for {0} method {1}; {2:D} captured parameters, {3:D} functional interface method parameters, {4:D} implementation parameters", ImplIsInstanceMethod ? "instance" : "static", ImplInfo, capturedArity, samArity, implArity));
			}
			if (instantiatedArity != samArity)
			{
				throw new LambdaConversionException(string.Format("Incorrect number of parameters for {0} method {1}; {2:D} instantiated parameters, {3:D} functional interface method parameters", ImplIsInstanceMethod ? "instance" : "static", ImplInfo, instantiatedArity, samArity));
			}
			foreach (MethodType bridgeMT in AdditionalBridges)
			{
				if (bridgeMT.ParameterCount() != samArity)
				{
					throw new LambdaConversionException(string.Format("Incorrect number of parameters for bridge signature {0}; incompatible with {1}", bridgeMT, SamMethodType));
				}
			}

			// If instance: first captured arg (receiver) must be subtype of class where impl method is defined
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int capturedStart;
			int capturedStart;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int samStart;
			int samStart;
			if (ImplIsInstanceMethod)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class receiverClass;
				Class receiverClass;

				// implementation is an instance method, adjust for receiver in captured variables / SAM arguments
				if (capturedArity == 0)
				{
					// receiver is function parameter
					capturedStart = 0;
					samStart = 1;
					receiverClass = InstantiatedMethodType.ParameterType(0);
				}
				else
				{
					// receiver is a captured variable
					capturedStart = 1;
					samStart = 0;
					receiverClass = InvokedType.ParameterType(0);
				}

				// check receiver type
				if (!receiverClass.IsSubclassOf(ImplDefiningClass))
				{
					throw new LambdaConversionException(string.Format("Invalid receiver type {0}; not a subtype of implementation type {1}", receiverClass, ImplDefiningClass));
				}

			   Class implReceiverClass = ImplMethod.Type().ParameterType(0);
			   if (implReceiverClass != ImplDefiningClass && !receiverClass.IsSubclassOf(implReceiverClass))
			   {
				   throw new LambdaConversionException(string.Format("Invalid receiver type {0}; not a subtype of implementation receiver type {1}", receiverClass, implReceiverClass));
			   }
			}
			else
			{
				// no receiver
				capturedStart = 0;
				samStart = 0;
			}

			// Check for exact match on non-receiver captured arguments
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int implFromCaptured = capturedArity - capturedStart;
			int implFromCaptured = capturedArity - capturedStart;
			for (int i = 0; i < implFromCaptured; i++)
			{
				Class implParamType = ImplMethodType.ParameterType(i);
				Class capturedParamType = InvokedType.ParameterType(i + capturedStart);
				if (!capturedParamType.Equals(implParamType))
				{
					throw new LambdaConversionException(string.Format("Type mismatch in captured lambda parameter {0:D}: expecting {1}, found {2}", i, capturedParamType, implParamType));
				}
			}
			// Check for adaptation match on SAM arguments
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int samOffset = samStart - implFromCaptured;
			int samOffset = samStart - implFromCaptured;
			for (int i = implFromCaptured; i < implArity; i++)
			{
				Class implParamType = ImplMethodType.ParameterType(i);
				Class instantiatedParamType = InstantiatedMethodType.ParameterType(i + samOffset);
				if (!IsAdaptableTo(instantiatedParamType, implParamType, true))
				{
					throw new LambdaConversionException(string.Format("Type mismatch for lambda argument {0:D}: {1} is not convertible to {2}", i, instantiatedParamType, implParamType));
				}
			}

			// Adaptation match: return type
			Class expectedType = InstantiatedMethodType.ReturnType();
			Class actualReturnType = (ImplKind == MethodHandleInfo.REF_newInvokeSpecial) ? ImplDefiningClass : ImplMethodType.ReturnType();
			Class samReturnType = SamMethodType.ReturnType();
			if (!IsAdaptableToAsReturn(actualReturnType, expectedType))
			{
				throw new LambdaConversionException(string.Format("Type mismatch for lambda return: {0} is not convertible to {1}", actualReturnType, expectedType));
			}
			if (!IsAdaptableToAsReturnStrict(expectedType, samReturnType))
			{
				throw new LambdaConversionException(string.Format("Type mismatch for lambda expected return: {0} is not convertible to {1}", expectedType, samReturnType));
			}
			foreach (MethodType bridgeMT in AdditionalBridges)
			{
				if (!IsAdaptableToAsReturnStrict(expectedType, bridgeMT.ReturnType()))
				{
					throw new LambdaConversionException(string.Format("Type mismatch for lambda expected return: {0} is not convertible to {1}", expectedType, bridgeMT.ReturnType()));
				}
			}
		}

		/// <summary>
		/// Check type adaptability for parameter types. </summary>
		/// <param name="fromType"> Type to convert from </param>
		/// <param name="toType"> Type to convert to </param>
		/// <param name="strict"> If true, do strict checks, else allow that fromType may be parameterized </param>
		/// <returns> True if 'fromType' can be passed to an argument of 'toType' </returns>
		private bool IsAdaptableTo(Class fromType, Class toType, bool strict)
		{
			if (fromType.Equals(toType))
			{
				return true;
			}
			if (fromType.Primitive)
			{
				Wrapper wfrom = forPrimitiveType(fromType);
				if (toType.Primitive)
				{
					// both are primitive: widening
					Wrapper wto = forPrimitiveType(toType);
					return wto.isConvertibleFrom(wfrom);
				}
				else
				{
					// from primitive to reference: boxing
					return wfrom.wrapperType().IsSubclassOf(toType);
				}
			}
			else
			{
				if (toType.Primitive)
				{
					// from reference to primitive: unboxing
					Wrapper wfrom;
					if (isWrapperType(fromType) && (wfrom = forWrapperType(fromType)).primitiveType().Primitive)
					{
						// fromType is a primitive wrapper; unbox+widen
						Wrapper wto = forPrimitiveType(toType);
						return wto.isConvertibleFrom(wfrom);
					}
					else
					{
						// must be convertible to primitive
						return !strict;
					}
				}
				else
				{
					// both are reference types: fromType should be a superclass of toType.
					return !strict || fromType.IsSubclassOf(toType);
				}
			}
		}

		/// <summary>
		/// Check type adaptability for return types --
		/// special handling of void type) and parameterized fromType </summary>
		/// <returns> True if 'fromType' can be converted to 'toType' </returns>
		private bool IsAdaptableToAsReturn(Class fromType, Class toType)
		{
			return toType.Equals(typeof(void)) || !fromType.Equals(typeof(void)) && IsAdaptableTo(fromType, toType, false);
		}
		private bool IsAdaptableToAsReturnStrict(Class fromType, Class toType)
		{
			if (fromType.Equals(typeof(void)))
			{
				return toType.Equals(typeof(void));
			}
			return IsAdaptableTo(fromType, toType, true);
		}


		/// <summary>
		///********* Logging support -- for debugging only, uncomment as needed
		/// static final Executor logPool = Executors.newSingleThreadExecutor();
		/// protected static void log(final String s) {
		///    MethodHandleProxyLambdaMetafactory.logPool.execute(new Runnable() {
		///        @Override
		///        public void run() {
		///            System.out.println(s);
		///        }
		///    });
		/// }
		/// 
		/// protected static void log(final String s, final Throwable e) {
		///    MethodHandleProxyLambdaMetafactory.logPool.execute(new Runnable() {
		///        @Override
		///        public void run() {
		///            System.out.println(s);
		///            e.printStackTrace(System.out);
		///        }
		///    });
		/// }
		/// **********************
		/// </summary>

	 }

}