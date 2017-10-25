using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	using CallerSensitive = sun.reflect.CallerSensitive;
	using MethodAccessor = sun.reflect.MethodAccessor;
	using Reflection = sun.reflect.Reflection;
	using MethodRepository = sun.reflect.generics.repository.MethodRepository;
	using CoreReflectionFactory = sun.reflect.generics.factory.CoreReflectionFactory;
	using GenericsFactory = sun.reflect.generics.factory.GenericsFactory;
	using MethodScope = sun.reflect.generics.scope.MethodScope;
	using AnnotationType = sun.reflect.annotation.AnnotationType;
	using AnnotationParser = sun.reflect.annotation.AnnotationParser;

	/// <summary>
	/// A {@code Method} provides information about, and access to, a single method
	/// on a class or interface.  The reflected method may be a class method
	/// or an instance method (including an abstract method).
	/// 
	/// <para>A {@code Method} permits widening conversions to occur when matching the
	/// actual parameters to invoke with the underlying method's formal
	/// parameters, but it throws an {@code IllegalArgumentException} if a
	/// narrowing conversion would occur.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Member </seealso>
	/// <seealso cref= java.lang.Class </seealso>
	/// <seealso cref= java.lang.Class#getMethods() </seealso>
	/// <seealso cref= java.lang.Class#getMethod(String, Class[]) </seealso>
	/// <seealso cref= java.lang.Class#getDeclaredMethods() </seealso>
	/// <seealso cref= java.lang.Class#getDeclaredMethod(String, Class[])
	/// 
	/// @author Kenneth Russell
	/// @author Nakul Saraiya </seealso>
	public sealed class Method : Executable
	{
		private Class Clazz;
		private int Slot;
		// This is guaranteed to be interned by the VM in the 1.4
		// reflection implementation
		private String Name_Renamed;
		private Class ReturnType_Renamed;
		private Class[] ParameterTypes_Renamed;
		private Class[] ExceptionTypes_Renamed;
		private int Modifiers_Renamed;
		// Generics and annotations support
		[NonSerialized]
		private String Signature;
		// generic info repository; lazily initialized
		[NonSerialized]
		private MethodRepository GenericInfo_Renamed;
		private sbyte[] Annotations;
		private sbyte[] ParameterAnnotations_Renamed;
		private sbyte[] AnnotationDefault;
		private volatile MethodAccessor MethodAccessor_Renamed;
		// For sharing of MethodAccessors. This branching structure is
		// currently only two levels deep (i.e., one root Method and
		// potentially many Method objects pointing to it.)
		//
		// If this branching structure would ever contain cycles, deadlocks can
		// occur in annotation code.
		private Method Root_Renamed;

		// Generics infrastructure
		private String GenericSignature
		{
			get
			{
				return Signature;
			}
		}

		// Accessor for factory
		private GenericsFactory Factory
		{
			get
			{
				// create scope and factory
				return CoreReflectionFactory.make(this, MethodScope.make(this));
			}
		}

		// Accessor for generic info repository
		internal override MethodRepository GenericInfo
		{
			get
			{
				// lazily initialize repository if necessary
				if (GenericInfo_Renamed == AnnotatedElement_Fields.Null)
				{
					// create and cache generic info repository
					GenericInfo_Renamed = MethodRepository.make(GenericSignature, Factory);
				}
				return GenericInfo_Renamed; //return cached repository
			}
		}

		/// <summary>
		/// Package-private constructor used by ReflectAccess to enable
		/// instantiation of these objects in Java code from the java.lang
		/// package via sun.reflect.LangReflectAccess.
		/// </summary>
		internal Method(Class declaringClass, String name, Class[] parameterTypes, Class returnType, Class[] checkedExceptions, int modifiers, int slot, String signature, sbyte[] annotations, sbyte[] parameterAnnotations, sbyte[] annotationDefault)
		{
			this.Clazz = declaringClass;
			this.Name_Renamed = name;
			this.ParameterTypes_Renamed = parameterTypes;
			this.ReturnType_Renamed = returnType;
			this.ExceptionTypes_Renamed = checkedExceptions;
			this.Modifiers_Renamed = modifiers;
			this.Slot = slot;
			this.Signature = signature;
			this.Annotations = annotations;
			this.ParameterAnnotations_Renamed = parameterAnnotations;
			this.AnnotationDefault = annotationDefault;
		}

		/// <summary>
		/// Package-private routine (exposed to java.lang.Class via
		/// ReflectAccess) which returns a copy of this Method. The copy's
		/// "root" field points to this Method.
		/// </summary>
		internal Method Copy()
		{
			// This routine enables sharing of MethodAccessor objects
			// among Method objects which refer to the same underlying
			// method in the VM. (All of this contortion is only necessary
			// because of the "accessibility" bit in AccessibleObject,
			// which implicitly requires that new java.lang.reflect
			// objects be fabricated for each reflective call on Class
			// objects.)
			if (this.Root_Renamed != AnnotatedElement_Fields.Null)
			{
				throw new IllegalArgumentException("Can not copy a non-root Method");
			}

			Method res = new Method(Clazz, Name_Renamed, ParameterTypes_Renamed, ReturnType_Renamed, ExceptionTypes_Renamed, Modifiers_Renamed, Slot, Signature, Annotations, ParameterAnnotations_Renamed, AnnotationDefault);
			res.Root_Renamed = this;
			// Might as well eagerly propagate this if already present
			res.MethodAccessor_Renamed = MethodAccessor_Renamed;
			return res;
		}

		/// <summary>
		/// Used by Excecutable for annotation sharing.
		/// </summary>
		internal override Executable Root
		{
			get
			{
				return Root_Renamed;
			}
		}

		internal override bool HasGenericInformation()
		{
			return (GenericSignature != AnnotatedElement_Fields.Null);
		}

		internal override sbyte[] AnnotationBytes
		{
			get
			{
				return Annotations;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Class DeclaringClass
		{
			get
			{
				return Clazz;
			}
		}

		/// <summary>
		/// Returns the name of the method represented by this {@code Method}
		/// object, as a {@code String}.
		/// </summary>
		public override String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int Modifiers
		{
			get
			{
				return Modifiers_Renamed;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="GenericSignatureFormatError"> {@inheritDoc}
		/// @since 1.5 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({"rawtypes", "unchecked"}) public TypeVariable<Method>[] getTypeParameters()
		public override TypeVariable<Method>[] TypeParameters
		{
			get
			{
				if (GenericSignature != AnnotatedElement_Fields.Null)
				{
					return (TypeVariable<Method>[])GenericInfo.TypeParameters;
				}
				else
				{
					return (TypeVariable<Method>[])new TypeVariable[0];
				}
			}
		}

		/// <summary>
		/// Returns a {@code Class} object that represents the formal return type
		/// of the method represented by this {@code Method} object.
		/// </summary>
		/// <returns> the return type for the method this object represents </returns>
		public Class ReturnType
		{
			get
			{
				return ReturnType_Renamed;
			}
		}

		/// <summary>
		/// Returns a {@code Type} object that represents the formal return
		/// type of the method represented by this {@code Method} object.
		/// 
		/// <para>If the return type is a parameterized type,
		/// the {@code Type} object returned must accurately reflect
		/// the actual type parameters used in the source code.
		/// 
		/// </para>
		/// <para>If the return type is a type variable or a parameterized type, it
		/// is created. Otherwise, it is resolved.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a {@code Type} object that represents the formal return
		///     type of the underlying  method </returns>
		/// <exception cref="GenericSignatureFormatError">
		///     if the generic method signature does not conform to the format
		///     specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if the underlying method's
		///     return type refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if the
		///     underlying method's return typed refers to a parameterized
		///     type that cannot be instantiated for any reason
		/// @since 1.5 </exception>
		public Type GenericReturnType
		{
			get
			{
			  if (GenericSignature != AnnotatedElement_Fields.Null)
			  {
				return GenericInfo.ReturnType;
			  }
			  else
			  {
				  return ReturnType;
			  }
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Class[] ParameterTypes
		{
			get
			{
				return ParameterTypes_Renamed.clone();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int ParameterCount
		{
			get
			{
				return ParameterTypes_Renamed.Length;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="GenericSignatureFormatError"> {@inheritDoc} </exception>
		/// <exception cref="TypeNotPresentException"> {@inheritDoc} </exception>
		/// <exception cref="MalformedParameterizedTypeException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public override Type[] GenericParameterTypes
		{
			get
			{
				return base.GenericParameterTypes;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Class[] ExceptionTypes
		{
			get
			{
				return ExceptionTypes_Renamed.clone();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="GenericSignatureFormatError"> {@inheritDoc} </exception>
		/// <exception cref="TypeNotPresentException"> {@inheritDoc} </exception>
		/// <exception cref="MalformedParameterizedTypeException"> {@inheritDoc}
		/// @since 1.5 </exception>
		public override Type[] GenericExceptionTypes
		{
			get
			{
				return base.GenericExceptionTypes;
			}
		}

		/// <summary>
		/// Compares this {@code Method} against the specified object.  Returns
		/// true if the objects are the same.  Two {@code Methods} are the same if
		/// they were declared by the same class and have the same name
		/// and formal parameter types and return type.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj != AnnotatedElement_Fields.Null && obj is Method)
			{
				Method other = (Method)obj;
				if ((DeclaringClass == other.DeclaringClass) && (Name == other.Name))
				{
					if (!ReturnType_Renamed.Equals(other.ReturnType))
					{
						return false;
					}
					return EqualParamTypes(ParameterTypes_Renamed, other.ParameterTypes_Renamed);
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a hashcode for this {@code Method}.  The hashcode is computed
		/// as the exclusive-or of the hashcodes for the underlying
		/// method's declaring class name and the method's name.
		/// </summary>
		public override int HashCode()
		{
			return DeclaringClass.Name.HashCode() ^ Name.HashCode();
		}

		/// <summary>
		/// Returns a string describing this {@code Method}.  The string is
		/// formatted as the method access modifiers, if any, followed by
		/// the method return type, followed by a space, followed by the
		/// class declaring the method, followed by a period, followed by
		/// the method name, followed by a parenthesized, comma-separated
		/// list of the method's formal parameter types. If the method
		/// throws checked exceptions, the parameter list is followed by a
		/// space, followed by the word throws followed by a
		/// comma-separated list of the thrown exception types.
		/// For example:
		/// <pre>
		///    public boolean java.lang.Object.equals(java.lang.Object)
		/// </pre>
		/// 
		/// <para>The access modifiers are placed in canonical order as
		/// specified by "The Java Language Specification".  This is
		/// {@code public}, {@code protected} or {@code private} first,
		/// and then other modifiers in the following order:
		/// {@code abstract}, {@code default}, {@code static}, {@code final},
		/// {@code synchronized}, {@code native}, {@code strictfp}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Method}
		/// 
		/// @jls 8.4.3 Method Modifiers </returns>
		public override String ToString()
		{
			return SharedToString(Modifier.MethodModifiers(), Default, ParameterTypes_Renamed, ExceptionTypes_Renamed);
		}

		internal override void SpecificToStringHeader(StringBuilder sb)
		{
			sb.Append(ReturnType.TypeName).Append(' ');
			sb.Append(DeclaringClass.TypeName).Append('.');
			sb.Append(Name);
		}

		/// <summary>
		/// Returns a string describing this {@code Method}, including
		/// type parameters.  The string is formatted as the method access
		/// modifiers, if any, followed by an angle-bracketed
		/// comma-separated list of the method's type parameters, if any,
		/// followed by the method's generic return type, followed by a
		/// space, followed by the class declaring the method, followed by
		/// a period, followed by the method name, followed by a
		/// parenthesized, comma-separated list of the method's generic
		/// formal parameter types.
		/// 
		/// If this method was declared to take a variable number of
		/// arguments, instead of denoting the last parameter as
		/// "<tt><i>Type</i>[]</tt>", it is denoted as
		/// "<tt><i>Type</i>...</tt>".
		/// 
		/// A space is used to separate access modifiers from one another
		/// and from the type parameters or return type.  If there are no
		/// type parameters, the type parameter list is elided; if the type
		/// parameter list is present, a space separates the list from the
		/// class name.  If the method is declared to throw exceptions, the
		/// parameter list is followed by a space, followed by the word
		/// throws followed by a comma-separated list of the generic thrown
		/// exception types.
		/// 
		/// <para>The access modifiers are placed in canonical order as
		/// specified by "The Java Language Specification".  This is
		/// {@code public}, {@code protected} or {@code private} first,
		/// and then other modifiers in the following order:
		/// {@code abstract}, {@code default}, {@code static}, {@code final},
		/// {@code synchronized}, {@code native}, {@code strictfp}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Method},
		/// include type parameters
		/// 
		/// @since 1.5
		/// 
		/// @jls 8.4.3 Method Modifiers </returns>
		public override String ToGenericString()
		{
			return SharedToGenericString(Modifier.MethodModifiers(), Default);
		}

		internal override void SpecificToGenericStringHeader(StringBuilder sb)
		{
			Type genRetType = GenericReturnType;
			sb.Append(genRetType.TypeName).Append(' ');
			sb.Append(DeclaringClass.TypeName).Append('.');
			sb.Append(Name);
		}

		/// <summary>
		/// Invokes the underlying method represented by this {@code Method}
		/// object, on the specified object with the specified parameters.
		/// Individual parameters are automatically unwrapped to match
		/// primitive formal parameters, and both primitive and reference
		/// parameters are subject to method invocation conversions as
		/// necessary.
		/// 
		/// <para>If the underlying method is static, then the specified {@code obj}
		/// argument is ignored. It may be null.
		/// 
		/// </para>
		/// <para>If the number of formal parameters required by the underlying method is
		/// 0, the supplied {@code args} array may be of length 0 or null.
		/// 
		/// </para>
		/// <para>If the underlying method is an instance method, it is invoked
		/// using dynamic method lookup as documented in The Java Language
		/// Specification, Second Edition, section 15.12.4.4; in particular,
		/// overriding based on the runtime type of the target object will occur.
		/// 
		/// </para>
		/// <para>If the underlying method is static, the class that declared
		/// the method is initialized if it has not already been initialized.
		/// 
		/// </para>
		/// <para>If the method completes normally, the value it returns is
		/// returned to the caller of invoke; if the value has a primitive
		/// type, it is first appropriately wrapped in an object. However,
		/// if the value has the type of an array of a primitive type, the
		/// elements of the array are <i>not</i> wrapped in objects; in
		/// other words, an array of primitive type is returned.  If the
		/// underlying method return type is void, the invocation returns
		/// null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object the underlying method is invoked from </param>
		/// <param name="args"> the arguments used for the method call </param>
		/// <returns> the result of dispatching the method represented by
		/// this object on {@code obj} with parameters
		/// {@code args}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Method} object
		///              is enforcing Java language access control and the underlying
		///              method is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the method is an
		///              instance method and the specified object argument
		///              is not an instance of the class or interface
		///              declaring the underlying method (or of a subclass
		///              or implementor thereof); if the number of actual
		///              and formal parameters differ; if an unwrapping
		///              conversion for primitive arguments fails; or if,
		///              after possible unwrapping, a parameter value
		///              cannot be converted to the corresponding formal
		///              parameter type by a method invocation conversion. </exception>
		/// <exception cref="InvocationTargetException"> if the underlying method
		///              throws an exception. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the method is an instance method. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization
		/// provoked by this method fails. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Object invoke(Object obj, Object... args) throws IllegalAccessException, IllegalArgumentException, InvocationTargetException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Object Invoke(Object obj, params Object[] args)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			MethodAccessor ma = MethodAccessor_Renamed; // read volatile
			if (ma == AnnotatedElement_Fields.Null)
			{
				ma = AcquireMethodAccessor();
			}
			return ma.invoke(obj, args);
		}

		/// <summary>
		/// Returns {@code true} if this method is a bridge
		/// method; returns {@code false} otherwise.
		/// </summary>
		/// <returns> true if and only if this method is a bridge
		/// method as defined by the Java Language Specification.
		/// @since 1.5 </returns>
		public bool Bridge
		{
			get
			{
				return (Modifiers & Modifier.BRIDGE) != 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.5
		/// </summary>
		public override bool VarArgs
		{
			get
			{
				return base.VarArgs;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @jls 13.1 The Form of a Binary
		/// @since 1.5
		/// </summary>
		public override bool Synthetic
		{
			get
			{
				return base.Synthetic;
			}
		}

		/// <summary>
		/// Returns {@code true} if this method is a default
		/// method; returns {@code false} otherwise.
		/// 
		/// A default method is a public non-abstract instance method, that
		/// is, a non-static method with a body, declared in an interface
		/// type.
		/// </summary>
		/// <returns> true if and only if this method is a default
		/// method as defined by the Java Language Specification.
		/// @since 1.8 </returns>
		public bool Default
		{
			get
			{
				// Default methods are public non-abstract instance methods
				// declared in an interface.
				return ((Modifiers & (Modifier.ABSTRACT | Modifier.PUBLIC | Modifier.STATIC)) == Modifier.PUBLIC) && DeclaringClass.Interface;
			}
		}

		// NOTE that there is no synchronization used here. It is correct
		// (though not efficient) to generate more than one MethodAccessor
		// for a given Method. However, avoiding synchronization will
		// probably make the implementation more scalable.
		private MethodAccessor AcquireMethodAccessor()
		{
			// First check to see if one has been created yet, and take it
			// if so
			MethodAccessor tmp = AnnotatedElement_Fields.Null;
			if (Root_Renamed != AnnotatedElement_Fields.Null)
			{
				tmp = Root_Renamed.MethodAccessor;
			}
			if (tmp != AnnotatedElement_Fields.Null)
			{
				MethodAccessor_Renamed = tmp;
			}
			else
			{
				// Otherwise fabricate one and propagate it up to the root
				tmp = ReflectionFactory.newMethodAccessor(this);
				MethodAccessor = tmp;
			}

			return tmp;
		}

		// Returns MethodAccessor for this Method object, not looking up
		// the chain to the root
		internal MethodAccessor MethodAccessor
		{
			get
			{
				return MethodAccessor_Renamed;
			}
			set
			{
				MethodAccessor_Renamed = value;
				// Propagate up
				if (Root_Renamed != AnnotatedElement_Fields.Null)
				{
					Root_Renamed.MethodAccessor = value;
				}
			}
		}


		/// <summary>
		/// Returns the default value for the annotation member represented by
		/// this {@code Method} instance.  If the member is of a primitive type,
		/// an instance of the corresponding wrapper type is returned. Returns
		/// null if no default is associated with the member, or if the method
		/// instance does not represent a declared member of an annotation type.
		/// </summary>
		/// <returns> the default value for the annotation member represented
		///     by this {@code Method} instance. </returns>
		/// <exception cref="TypeNotPresentException"> if the annotation is of type
		///     <seealso cref="Class"/> and no definition can be found for the
		///     default class value.
		/// @since  1.5 </exception>
		public Object DefaultValue
		{
			get
			{
				if (AnnotationDefault == AnnotatedElement_Fields.Null)
				{
					return AnnotatedElement_Fields.Null;
				}
				Class memberType = AnnotationType.invocationHandlerReturnType(ReturnType);
				Object AnnotatedElement_Fields.Result = AnnotationParser.parseMemberValue(memberType, ByteBuffer.Wrap(AnnotationDefault), sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), DeclaringClass);
				if (AnnotatedElement_Fields.Result is sun.reflect.annotation.ExceptionProxy)
				{
					throw new AnnotationFormatError("Invalid default: " + this);
				}
				return AnnotatedElement_Fields.Result;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException">  {@inheritDoc}
		/// @since 1.5 </exception>
		public override T getAnnotation<T>(Class annotationClass) where T : Annotation
		{
			return base.GetAnnotation(annotationClass);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.5
		/// </summary>
		public override Annotation[] DeclaredAnnotations
		{
			get
			{
				return base.GetCustomAttributes(false);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.5
		/// </summary>
		public override Annotation[][] ParameterAnnotations
		{
			get
			{
				return SharedGetParameterAnnotations(ParameterTypes_Renamed, ParameterAnnotations_Renamed);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.8
		/// </summary>
		public override AnnotatedType AnnotatedReturnType
		{
			get
			{
				return GetAnnotatedReturnType0(GenericReturnType);
			}
		}

		internal override void HandleParameterNumberMismatch(int resultLength, int numParameters)
		{
			throw new AnnotationFormatError("Parameter annotations don't match number of parameters");
		}
	}

}