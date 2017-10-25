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


	/// <summary>
	/// Serialized form of a lambda expression.  The properties of this class
	/// represent the information that is present at the lambda factory site, including
	/// static metafactory arguments such as the identity of the primary functional
	/// interface method and the identity of the implementation method, as well as
	/// dynamic metafactory arguments such as values captured from the lexical scope
	/// at the time of lambda capture.
	/// 
	/// <para>Implementors of serializable lambdas, such as compilers or language
	/// runtime libraries, are expected to ensure that instances deserialize properly.
	/// One means to do so is to ensure that the {@code writeReplace} method returns
	/// an instance of {@code SerializedLambda}, rather than allowing default
	/// serialization to proceed.
	/// 
	/// </para>
	/// <para>{@code SerializedLambda} has a {@code readResolve} method that looks for
	/// a (possibly private) static method called
	/// {@code $deserializeLambda$(SerializedLambda)} in the capturing class, invokes
	/// that with itself as the first argument, and returns the result.  Lambda classes
	/// implementing {@code $deserializeLambda$} are responsible for validating
	/// that the properties of the {@code SerializedLambda} are consistent with a
	/// lambda actually captured by that class.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= LambdaMetafactory </seealso>
	[Serializable]
	public sealed class SerializedLambda
	{
		private const long SerialVersionUID = 8025925345765570181L;
		private readonly Class CapturingClass_Renamed;
		private readonly String FunctionalInterfaceClass_Renamed;
		private readonly String FunctionalInterfaceMethodName_Renamed;
		private readonly String FunctionalInterfaceMethodSignature_Renamed;
		private readonly String ImplClass_Renamed;
		private readonly String ImplMethodName_Renamed;
		private readonly String ImplMethodSignature_Renamed;
		private readonly int ImplMethodKind_Renamed;
		private readonly String InstantiatedMethodType_Renamed;
		private readonly Object[] CapturedArgs;

		/// <summary>
		/// Create a {@code SerializedLambda} from the low-level information present
		/// at the lambda factory site.
		/// </summary>
		/// <param name="capturingClass"> The class in which the lambda expression appears </param>
		/// <param name="functionalInterfaceClass"> Name, in slash-delimited form, of static
		///                                 type of the returned lambda object </param>
		/// <param name="functionalInterfaceMethodName"> Name of the functional interface
		///                                      method for the present at the
		///                                      lambda factory site </param>
		/// <param name="functionalInterfaceMethodSignature"> Signature of the functional
		///                                           interface method present at
		///                                           the lambda factory site </param>
		/// <param name="implMethodKind"> Method handle kind for the implementation method </param>
		/// <param name="implClass"> Name, in slash-delimited form, for the class holding
		///                  the implementation method </param>
		/// <param name="implMethodName"> Name of the implementation method </param>
		/// <param name="implMethodSignature"> Signature of the implementation method </param>
		/// <param name="instantiatedMethodType"> The signature of the primary functional
		///                               interface method after type variables
		///                               are substituted with their instantiation
		///                               from the capture site </param>
		/// <param name="capturedArgs"> The dynamic arguments to the lambda factory site,
		///                     which represent variables captured by
		///                     the lambda </param>
		public SerializedLambda(Class capturingClass, String functionalInterfaceClass, String functionalInterfaceMethodName, String functionalInterfaceMethodSignature, int implMethodKind, String implClass, String implMethodName, String implMethodSignature, String instantiatedMethodType, Object[] capturedArgs)
		{
			this.CapturingClass_Renamed = capturingClass;
			this.FunctionalInterfaceClass_Renamed = functionalInterfaceClass;
			this.FunctionalInterfaceMethodName_Renamed = functionalInterfaceMethodName;
			this.FunctionalInterfaceMethodSignature_Renamed = functionalInterfaceMethodSignature;
			this.ImplMethodKind_Renamed = implMethodKind;
			this.ImplClass_Renamed = implClass;
			this.ImplMethodName_Renamed = implMethodName;
			this.ImplMethodSignature_Renamed = implMethodSignature;
			this.InstantiatedMethodType_Renamed = instantiatedMethodType;
			this.CapturedArgs = Objects.RequireNonNull(capturedArgs).clone();
		}

		/// <summary>
		/// Get the name of the class that captured this lambda. </summary>
		/// <returns> the name of the class that captured this lambda </returns>
		public String CapturingClass
		{
			get
			{
				return CapturingClass_Renamed.Name.Replace('.', '/');
			}
		}

		/// <summary>
		/// Get the name of the invoked type to which this
		/// lambda has been converted </summary>
		/// <returns> the name of the functional interface class to which
		/// this lambda has been converted </returns>
		public String FunctionalInterfaceClass
		{
			get
			{
				return FunctionalInterfaceClass_Renamed;
			}
		}

		/// <summary>
		/// Get the name of the primary method for the functional interface
		/// to which this lambda has been converted. </summary>
		/// <returns> the name of the primary methods of the functional interface </returns>
		public String FunctionalInterfaceMethodName
		{
			get
			{
				return FunctionalInterfaceMethodName_Renamed;
			}
		}

		/// <summary>
		/// Get the signature of the primary method for the functional
		/// interface to which this lambda has been converted. </summary>
		/// <returns> the signature of the primary method of the functional
		/// interface </returns>
		public String FunctionalInterfaceMethodSignature
		{
			get
			{
				return FunctionalInterfaceMethodSignature_Renamed;
			}
		}

		/// <summary>
		/// Get the name of the class containing the implementation
		/// method. </summary>
		/// <returns> the name of the class containing the implementation
		/// method </returns>
		public String ImplClass
		{
			get
			{
				return ImplClass_Renamed;
			}
		}

		/// <summary>
		/// Get the name of the implementation method. </summary>
		/// <returns> the name of the implementation method </returns>
		public String ImplMethodName
		{
			get
			{
				return ImplMethodName_Renamed;
			}
		}

		/// <summary>
		/// Get the signature of the implementation method. </summary>
		/// <returns> the signature of the implementation method </returns>
		public String ImplMethodSignature
		{
			get
			{
				return ImplMethodSignature_Renamed;
			}
		}

		/// <summary>
		/// Get the method handle kind (see <seealso cref="MethodHandleInfo"/>) of
		/// the implementation method. </summary>
		/// <returns> the method handle kind of the implementation method </returns>
		public int ImplMethodKind
		{
			get
			{
				return ImplMethodKind_Renamed;
			}
		}

		/// <summary>
		/// Get the signature of the primary functional interface method
		/// after type variables are substituted with their instantiation
		/// from the capture site. </summary>
		/// <returns> the signature of the primary functional interface method
		/// after type variable processing </returns>
		public String InstantiatedMethodType
		{
			get
			{
				return InstantiatedMethodType_Renamed;
			}
		}

		/// <summary>
		/// Get the count of dynamic arguments to the lambda capture site. </summary>
		/// <returns> the count of dynamic arguments to the lambda capture site </returns>
		public int CapturedArgCount
		{
			get
			{
				return CapturedArgs.Length;
			}
		}

		/// <summary>
		/// Get a dynamic argument to the lambda capture site. </summary>
		/// <param name="i"> the argument to capture </param>
		/// <returns> a dynamic argument to the lambda capture site </returns>
		public Object GetCapturedArg(int i)
		{
			return CapturedArgs[i];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws ReflectiveOperationException
		private Object ReadResolve()
		{
			try
			{
				Method deserialize = AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this));

				return deserialize.invoke(null, this);
			}
			catch (PrivilegedActionException e)
			{
				Exception cause = e.Exception;
				if (cause is ReflectiveOperationException)
				{
					throw (ReflectiveOperationException) cause;
				}
				else if (cause is RuntimeException)
				{
					throw (RuntimeException) cause;
				}
				else
				{
					throw new RuntimeException("Exception in SerializedLambda.readResolve", e);
				}
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Method>
		{
			private readonly SerializedLambda OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(SerializedLambda outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Method run() throws Exception
			public virtual Method Run()
			{
				Method m = OuterInstance.CapturingClass_Renamed.GetDeclaredMethod("$deserializeLambda$", typeof(SerializedLambda));
				m.Accessible = true;
				return m;
			}
		}

		public override String ToString()
		{
			String implKind = MethodHandleInfo.referenceKindToString(ImplMethodKind_Renamed);
			return string.Format("SerializedLambda[{0}={1}, {2}={3}.{4}:{5}, " + "{6}={7} {8}.{9}:{10}, {11}={12}, {13}={14:D}]", "capturingClass", CapturingClass_Renamed, "functionalInterfaceMethod", FunctionalInterfaceClass_Renamed, FunctionalInterfaceMethodName_Renamed, FunctionalInterfaceMethodSignature_Renamed, "implementation", implKind, ImplClass_Renamed, ImplMethodName_Renamed, ImplMethodSignature_Renamed, "instantiatedMethodType", InstantiatedMethodType_Renamed, "numCaptured", CapturedArgs.Length);
		}
	}

}