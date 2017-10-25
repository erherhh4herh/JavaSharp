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
	using ConstructorAccessor = sun.reflect.ConstructorAccessor;
	using Reflection = sun.reflect.Reflection;
	using TypeAnnotation = sun.reflect.annotation.TypeAnnotation;
	using TypeAnnotationParser = sun.reflect.annotation.TypeAnnotationParser;
	using ConstructorRepository = sun.reflect.generics.repository.ConstructorRepository;
	using CoreReflectionFactory = sun.reflect.generics.factory.CoreReflectionFactory;
	using GenericsFactory = sun.reflect.generics.factory.GenericsFactory;
	using ConstructorScope = sun.reflect.generics.scope.ConstructorScope;

	/// <summary>
	/// {@code Constructor} provides information about, and access to, a single
	/// constructor for a class.
	/// 
	/// <para>{@code Constructor} permits widening conversions to occur when matching the
	/// actual parameters to newInstance() with the underlying
	/// constructor's formal parameters, but throws an
	/// {@code IllegalArgumentException} if a narrowing conversion would occur.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the class in which the constructor is declared
	/// </param>
	/// <seealso cref= Member </seealso>
	/// <seealso cref= java.lang.Class </seealso>
	/// <seealso cref= java.lang.Class#getConstructors() </seealso>
	/// <seealso cref= java.lang.Class#getConstructor(Class[]) </seealso>
	/// <seealso cref= java.lang.Class#getDeclaredConstructors()
	/// 
	/// @author      Kenneth Russell
	/// @author      Nakul Saraiya </seealso>
	public sealed class Constructor<T> : Executable
	{
		private Class Clazz;
		private int Slot_Renamed;
		private Class[] ParameterTypes_Renamed;
		private Class[] ExceptionTypes_Renamed;
		private int Modifiers_Renamed;
		// Generics and annotations support
		[NonSerialized]
		private String Signature_Renamed;
		// generic info repository; lazily initialized
		[NonSerialized]
		private ConstructorRepository GenericInfo_Renamed;
		private sbyte[] Annotations;
		private sbyte[] ParameterAnnotations_Renamed;

		// Generics infrastructure
		// Accessor for factory
		private GenericsFactory Factory
		{
			get
			{
				// create scope and factory
				return CoreReflectionFactory.make(this, ConstructorScope.make(this));
			}
		}

		// Accessor for generic info repository
		internal override ConstructorRepository GenericInfo
		{
			get
			{
				// lazily initialize repository if necessary
				if (GenericInfo_Renamed == AnnotatedElement_Fields.Null)
				{
					// create and cache generic info repository
					GenericInfo_Renamed = ConstructorRepository.make(Signature, Factory);
				}
				return GenericInfo_Renamed; //return cached repository
			}
		}

		private volatile ConstructorAccessor ConstructorAccessor_Renamed;
		// For sharing of ConstructorAccessors. This branching structure
		// is currently only two levels deep (i.e., one root Constructor
		// and potentially many Constructor objects pointing to it.)
		//
		// If this branching structure would ever contain cycles, deadlocks can
		// occur in annotation code.
		private Constructor<T> Root_Renamed;

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

		/// <summary>
		/// Package-private constructor used by ReflectAccess to enable
		/// instantiation of these objects in Java code from the java.lang
		/// package via sun.reflect.LangReflectAccess.
		/// </summary>
		internal Constructor(Class declaringClass, Class[] parameterTypes, Class[] checkedExceptions, int modifiers, int slot, String signature, sbyte[] annotations, sbyte[] parameterAnnotations)
		{
			this.Clazz = declaringClass;
			this.ParameterTypes_Renamed = parameterTypes;
			this.ExceptionTypes_Renamed = checkedExceptions;
			this.Modifiers_Renamed = modifiers;
			this.Slot_Renamed = slot;
			this.Signature_Renamed = signature;
			this.Annotations = annotations;
			this.ParameterAnnotations_Renamed = parameterAnnotations;
		}

		/// <summary>
		/// Package-private routine (exposed to java.lang.Class via
		/// ReflectAccess) which returns a copy of this Constructor. The copy's
		/// "root" field points to this Constructor.
		/// </summary>
		internal Constructor<T> Copy()
		{
			// This routine enables sharing of ConstructorAccessor objects
			// among Constructor objects which refer to the same underlying
			// method in the VM. (All of this contortion is only necessary
			// because of the "accessibility" bit in AccessibleObject,
			// which implicitly requires that new java.lang.reflect
			// objects be fabricated for each reflective call on Class
			// objects.)
			if (this.Root_Renamed != AnnotatedElement_Fields.Null)
			{
				throw new IllegalArgumentException("Can not copy a non-root Constructor");
			}

			Constructor<T> res = new Constructor<T>(Clazz, ParameterTypes_Renamed, ExceptionTypes_Renamed, Modifiers_Renamed, Slot_Renamed, Signature_Renamed, Annotations, ParameterAnnotations_Renamed);
			res.Root_Renamed = this;
			// Might as well eagerly propagate this if already present
			res.ConstructorAccessor_Renamed = ConstructorAccessor_Renamed;
			return res;
		}

		internal override bool HasGenericInformation()
		{
			return (Signature != AnnotatedElement_Fields.Null);
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
		/// Returns the name of this constructor, as a string.  This is
		/// the binary name of the constructor's declaring class.
		/// </summary>
		public override String Name
		{
			get
			{
				return DeclaringClass.Name;
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
//ORIGINAL LINE: @Override @SuppressWarnings({"rawtypes", "unchecked"}) public TypeVariable<Constructor<T>>[] getTypeParameters()
		public override TypeVariable<Constructor<T>>[] TypeParameters
		{
			get
			{
			  if (Signature != AnnotatedElement_Fields.Null)
			  {
				return (TypeVariable<Constructor<T>>[])GenericInfo.TypeParameters;
			  }
			  else
			  {
				  return (TypeVariable<Constructor<T>>[])new TypeVariable[0];
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
		/// Compares this {@code Constructor} against the specified object.
		/// Returns true if the objects are the same.  Two {@code Constructor} objects are
		/// the same if they were declared by the same class and have the
		/// same formal parameter types.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj != AnnotatedElement_Fields.Null && obj is Constructor)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> other = (Constructor<?>)obj;
				Constructor<?> other = (Constructor<?>)obj;
				if (DeclaringClass == other.DeclaringClass)
				{
					return EqualParamTypes(ParameterTypes_Renamed, other.ParameterTypes_Renamed);
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a hashcode for this {@code Constructor}. The hashcode is
		/// the same as the hashcode for the underlying constructor's
		/// declaring class name.
		/// </summary>
		public override int HashCode()
		{
			return DeclaringClass.Name.HashCode();
		}

		/// <summary>
		/// Returns a string describing this {@code Constructor}.  The string is
		/// formatted as the constructor access modifiers, if any,
		/// followed by the fully-qualified name of the declaring class,
		/// followed by a parenthesized, comma-separated list of the
		/// constructor's formal parameter types.  For example:
		/// <pre>
		///    public java.util.Hashtable(int,float)
		/// </pre>
		/// 
		/// <para>The only possible modifiers for constructors are the access
		/// modifiers {@code public}, {@code protected} or
		/// {@code private}.  Only one of these may appear, or none if the
		/// constructor has default (package) access.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Constructor}
		/// @jls 8.8.3. Constructor Modifiers </returns>
		public override String ToString()
		{
			return SharedToString(Modifier.ConstructorModifiers(), false, ParameterTypes_Renamed, ExceptionTypes_Renamed);
		}

		internal override void SpecificToStringHeader(StringBuilder sb)
		{
			sb.Append(DeclaringClass.TypeName);
		}

		/// <summary>
		/// Returns a string describing this {@code Constructor},
		/// including type parameters.  The string is formatted as the
		/// constructor access modifiers, if any, followed by an
		/// angle-bracketed comma separated list of the constructor's type
		/// parameters, if any, followed by the fully-qualified name of the
		/// declaring class, followed by a parenthesized, comma-separated
		/// list of the constructor's generic formal parameter types.
		/// 
		/// If this constructor was declared to take a variable number of
		/// arguments, instead of denoting the last parameter as
		/// "<tt><i>Type</i>[]</tt>", it is denoted as
		/// "<tt><i>Type</i>...</tt>".
		/// 
		/// A space is used to separate access modifiers from one another
		/// and from the type parameters or return type.  If there are no
		/// type parameters, the type parameter list is elided; if the type
		/// parameter list is present, a space separates the list from the
		/// class name.  If the constructor is declared to throw
		/// exceptions, the parameter list is followed by a space, followed
		/// by the word "{@code throws}" followed by a
		/// comma-separated list of the thrown exception types.
		/// 
		/// <para>The only possible modifiers for constructors are the access
		/// modifiers {@code public}, {@code protected} or
		/// {@code private}.  Only one of these may appear, or none if the
		/// constructor has default (package) access.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Constructor},
		/// include type parameters
		/// 
		/// @since 1.5
		/// @jls 8.8.3. Constructor Modifiers </returns>
		public override String ToGenericString()
		{
			return SharedToGenericString(Modifier.ConstructorModifiers(), false);
		}

		internal override void SpecificToGenericStringHeader(StringBuilder sb)
		{
			SpecificToStringHeader(sb);
		}

		/// <summary>
		/// Uses the constructor represented by this {@code Constructor} object to
		/// create and initialize a new instance of the constructor's
		/// declaring class, with the specified initialization parameters.
		/// Individual parameters are automatically unwrapped to match
		/// primitive formal parameters, and both primitive and reference
		/// parameters are subject to method invocation conversions as necessary.
		/// 
		/// <para>If the number of formal parameters required by the underlying constructor
		/// is 0, the supplied {@code initargs} array may be of length 0 or null.
		/// 
		/// </para>
		/// <para>If the constructor's declaring class is an inner class in a
		/// non-static context, the first argument to the constructor needs
		/// to be the enclosing instance; see section 15.9.3 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// 
		/// </para>
		/// <para>If the required access and argument checks succeed and the
		/// instantiation will proceed, the constructor's declaring class
		/// is initialized if it has not already been initialized.
		/// 
		/// </para>
		/// <para>If the constructor completes normally, returns the newly
		/// created and initialized instance.
		/// 
		/// </para>
		/// </summary>
		/// <param name="initargs"> array of objects to be passed as arguments to
		/// the constructor call; values of primitive types are wrapped in
		/// a wrapper object of the appropriate type (e.g. a {@code float}
		/// in a <seealso cref="java.lang.Float Float"/>)
		/// </param>
		/// <returns> a new object created by calling the constructor
		/// this object represents
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Constructor} object
		///              is enforcing Java language access control and the underlying
		///              constructor is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the number of actual
		///              and formal parameters differ; if an unwrapping
		///              conversion for primitive arguments fails; or if,
		///              after possible unwrapping, a parameter value
		///              cannot be converted to the corresponding formal
		///              parameter type by a method invocation conversion; if
		///              this constructor pertains to an enum type. </exception>
		/// <exception cref="InstantiationException">    if the class that declares the
		///              underlying constructor represents an abstract class. </exception>
		/// <exception cref="InvocationTargetException"> if the underlying constructor
		///              throws an exception. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public T newInstance(Object... initargs) throws InstantiationException, IllegalAccessException, IllegalArgumentException, InvocationTargetException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public T NewInstance(params Object[] initargs)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, AnnotatedElement_Fields.Null, Modifiers_Renamed);
				}
			}
			if ((Clazz.Modifiers & Modifier.ENUM) != 0)
			{
				throw new IllegalArgumentException("Cannot reflectively create enum objects");
			}
			ConstructorAccessor ca = ConstructorAccessor_Renamed; // read volatile
			if (ca == AnnotatedElement_Fields.Null)
			{
				ca = AcquireConstructorAccessor();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T inst = (T) ca.newInstance(initargs);
			T inst = (T) ca.newInstance(initargs);
			return inst;
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

		// NOTE that there is no synchronization used here. It is correct
		// (though not efficient) to generate more than one
		// ConstructorAccessor for a given Constructor. However, avoiding
		// synchronization will probably make the implementation more
		// scalable.
		private ConstructorAccessor AcquireConstructorAccessor()
		{
			// First check to see if one has been created yet, and take it
			// if so.
			ConstructorAccessor tmp = AnnotatedElement_Fields.Null;
			if (Root_Renamed != AnnotatedElement_Fields.Null)
			{
				tmp = Root_Renamed.ConstructorAccessor;
			}
			if (tmp != AnnotatedElement_Fields.Null)
			{
				ConstructorAccessor_Renamed = tmp;
			}
			else
			{
				// Otherwise fabricate one and propagate it up to the root
				tmp = ReflectionFactory.newConstructorAccessor(this);
				ConstructorAccessor = tmp;
			}

			return tmp;
		}

		// Returns ConstructorAccessor for this Constructor object, not
		// looking up the chain to the root
		internal ConstructorAccessor ConstructorAccessor
		{
			get
			{
				return ConstructorAccessor_Renamed;
			}
			set
			{
				ConstructorAccessor_Renamed = value;
				// Propagate up
				if (Root_Renamed != AnnotatedElement_Fields.Null)
				{
					Root_Renamed.ConstructorAccessor = value;
				}
			}
		}


		internal int Slot
		{
			get
			{
				return Slot_Renamed;
			}
		}

		internal String Signature
		{
			get
			{
				return Signature_Renamed;
			}
		}

		internal sbyte[] RawAnnotations
		{
			get
			{
				return Annotations;
			}
		}

		internal sbyte[] RawParameterAnnotations
		{
			get
			{
				return ParameterAnnotations_Renamed;
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

		internal override void HandleParameterNumberMismatch(int resultLength, int numParameters)
		{
			Class declaringClass = DeclaringClass;
			if (declaringClass.Enum || declaringClass.AnonymousClass || declaringClass.LocalClass)
			{
				return; // Can't do reliable parameter counting
			}
			else
			{
				if (!declaringClass.MemberClass || (declaringClass.MemberClass && ((declaringClass.Modifiers & Modifier.STATIC) == 0) && resultLength + 1 != numParameters)) // top-level
					// Check for the enclosing instance parameter for
					// non-static member classes
				{
					throw new AnnotationFormatError("Parameter annotations don't match number of parameters");
				}
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
				return GetAnnotatedReturnType0(DeclaringClass);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.8
		/// </summary>
		public override AnnotatedType AnnotatedReceiverType
		{
			get
			{
				if (DeclaringClass.EnclosingClass == AnnotatedElement_Fields.Null)
				{
					return base.AnnotatedReceiverType;
				}
    
				return TypeAnnotationParser.buildAnnotatedType(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, DeclaringClass.EnclosingClass, TypeAnnotation.TypeAnnotationTarget.METHOD_RECEIVER);
			}
		}
	}

}