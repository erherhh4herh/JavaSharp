using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

namespace java.lang.reflect
{

	using AnnotationParser = sun.reflect.annotation.AnnotationParser;
	using AnnotationSupport = sun.reflect.annotation.AnnotationSupport;
	using TypeAnnotationParser = sun.reflect.annotation.TypeAnnotationParser;
	using TypeAnnotation = sun.reflect.annotation.TypeAnnotation;
	using ConstructorRepository = sun.reflect.generics.repository.ConstructorRepository;

	/// <summary>
	/// A shared superclass for the common functionality of <seealso cref="Method"/>
	/// and <seealso cref="Constructor"/>.
	/// 
	/// @since 1.8
	/// </summary>
	public abstract class Executable : AccessibleObject, Member, GenericDeclaration
	{
		/*
		 * Only grant package-visibility to the constructor.
		 */
		internal Executable()
		{
		}

		/// <summary>
		/// Accessor method to allow code sharing
		/// </summary>
		internal abstract sbyte[] AnnotationBytes {get;}

		/// <summary>
		/// Accessor method to allow code sharing
		/// </summary>
		internal abstract Executable Root {get;}

		/// <summary>
		/// Does the Executable have generic information.
		/// </summary>
		internal abstract bool HasGenericInformation();

		internal abstract ConstructorRepository GenericInfo {get;}

		internal virtual bool EqualParamTypes(Class[] params1, Class[] params2)
		{
			/* Avoid unnecessary cloning */
			if (params1.Length == params2.Length)
			{
				for (int i = 0; i < params1.Length; i++)
				{
					if (params1[i] != params2[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal virtual Annotation[][] ParseParameterAnnotations(sbyte[] parameterAnnotations)
		{
			return AnnotationParser.parseParameterAnnotations(parameterAnnotations, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), DeclaringClass);
		}

		internal virtual void SeparateWithCommas(Class[] types, StringBuilder sb)
		{
			for (int j = 0; j < types.Length; j++)
			{
				sb.Append(types[j].TypeName);
				if (j < (types.Length - 1))
				{
					sb.Append(",");
				}
			}

		}

		internal virtual void PrintModifiersIfNonzero(StringBuilder sb, int mask, bool isDefault)
		{
			int mod = Modifiers & mask;

			if (mod != 0 && !isDefault)
			{
				sb.Append(Modifier.ToString(mod)).Append(' ');
			}
			else
			{
				int access_mod = mod & Modifier.ACCESS_MODIFIERS;
				if (access_mod != 0)
				{
					sb.Append(Modifier.ToString(access_mod)).Append(' ');
				}
				if (isDefault)
				{
					sb.Append("default ");
				}
				mod = (mod & ~Modifier.ACCESS_MODIFIERS);
				if (mod != 0)
				{
					sb.Append(Modifier.ToString(mod)).Append(' ');
				}
			}
		}

		internal virtual String SharedToString(int modifierMask, bool isDefault, Class[] parameterTypes, Class[] exceptionTypes)
		{
			try
			{
				StringBuilder sb = new StringBuilder();

				PrintModifiersIfNonzero(sb, modifierMask, isDefault);
				SpecificToStringHeader(sb);

				sb.Append('(');
				SeparateWithCommas(parameterTypes, sb);
				sb.Append(')');
				if (exceptionTypes.Length > 0)
				{
					sb.Append(" throws ");
					SeparateWithCommas(exceptionTypes, sb);
				}
				return sb.ToString();
			}
			catch (Exception e)
			{
				return "<" + e + ">";
			}
		}

		/// <summary>
		/// Generate toString header information specific to a method or
		/// constructor.
		/// </summary>
		internal abstract void SpecificToStringHeader(StringBuilder sb);

		internal virtual String SharedToGenericString(int modifierMask, bool isDefault)
		{
			try
			{
				StringBuilder sb = new StringBuilder();

				PrintModifiersIfNonzero(sb, modifierMask, isDefault);

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: TypeVariable<?>[] typeparms = getTypeParameters();
				TypeVariable<?>[] typeparms = TypeParameters;
				if (typeparms.Length > 0)
				{
					bool first = true;
					sb.Append('<');
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for(TypeVariable<?> typeparm: typeparms)
					foreach (TypeVariable<?> typeparm in typeparms)
					{
						if (!first)
						{
							sb.Append(',');
						}
						// Class objects can't occur here; no need to test
						// and call Class.getName().
						sb.Append(typeparm.ToString());
						first = false;
					}
					sb.Append("> ");
				}

				SpecificToGenericStringHeader(sb);

				sb.Append('(');
				Type[] @params = GenericParameterTypes;
				for (int j = 0; j < @params.Length; j++)
				{
					String param = @params[j].TypeName;
					if (VarArgs && (j == @params.Length - 1)) // replace T[] with T...
					{
						param = param.ReplaceFirst("\\[\\]$", "...");
					}
					sb.Append(param);
					if (j < (@params.Length - 1))
					{
						sb.Append(',');
					}
				}
				sb.Append(')');
				Type[] exceptions = GenericExceptionTypes;
				if (exceptions.Length > 0)
				{
					sb.Append(" throws ");
					for (int k = 0; k < exceptions.Length; k++)
					{
						sb.Append((exceptions[k] is Class)? ((Class)exceptions[k]).Name: exceptions[k].ToString());
						if (k < (exceptions.Length - 1))
						{
							sb.Append(',');
						}
					}
				}
				return sb.ToString();
			}
			catch (Exception e)
			{
				return "<" + e + ">";
			}
		}

		/// <summary>
		/// Generate toGenericString header information specific to a
		/// method or constructor.
		/// </summary>
		internal abstract void SpecificToGenericStringHeader(StringBuilder sb);

		/// <summary>
		/// Returns the {@code Class} object representing the class or interface
		/// that declares the executable represented by this object.
		/// </summary>
		public abstract Class DeclaringClass {get;}

		/// <summary>
		/// Returns the name of the executable represented by this object.
		/// </summary>
		public abstract String Name {get;}

		/// <summary>
		/// Returns the Java language <seealso cref="Modifier modifiers"/> for
		/// the executable represented by this object.
		/// </summary>
		public abstract int Modifiers {get;}

		/// <summary>
		/// Returns an array of {@code TypeVariable} objects that represent the
		/// type variables declared by the generic declaration represented by this
		/// {@code GenericDeclaration} object, in declaration order.  Returns an
		/// array of length 0 if the underlying generic declaration declares no type
		/// variables.
		/// </summary>
		/// <returns> an array of {@code TypeVariable} objects that represent
		///     the type variables declared by this generic declaration </returns>
		/// <exception cref="GenericSignatureFormatError"> if the generic
		///     signature of this generic declaration does not conform to
		///     the format specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract TypeVariable<?>[] getTypeParameters();
		public abstract TypeVariable<?>[] TypeParameters {get;}

		/// <summary>
		/// Returns an array of {@code Class} objects that represent the formal
		/// parameter types, in declaration order, of the executable
		/// represented by this object.  Returns an array of length
		/// 0 if the underlying executable takes no parameters.
		/// </summary>
		/// <returns> the parameter types for the executable this object
		/// represents </returns>
		public abstract Class[] ParameterTypes {get;}

		/// <summary>
		/// Returns the number of formal parameters (whether explicitly
		/// declared or implicitly declared or neither) for the executable
		/// represented by this object.
		/// 
		/// @since 1.8 </summary>
		/// <returns> The number of formal parameters for the executable this
		/// object represents </returns>
		public virtual int ParameterCount
		{
			get
			{
				throw new AbstractMethodError();
			}
		}

		/// <summary>
		/// Returns an array of {@code Type} objects that represent the formal
		/// parameter types, in declaration order, of the executable represented by
		/// this object. Returns an array of length 0 if the
		/// underlying executable takes no parameters.
		/// 
		/// <para>If a formal parameter type is a parameterized type,
		/// the {@code Type} object returned for it must accurately reflect
		/// the actual type parameters used in the source code.
		/// 
		/// </para>
		/// <para>If a formal parameter type is a type variable or a parameterized
		/// type, it is created. Otherwise, it is resolved.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of {@code Type}s that represent the formal
		///     parameter types of the underlying executable, in declaration order </returns>
		/// <exception cref="GenericSignatureFormatError">
		///     if the generic method signature does not conform to the format
		///     specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if any of the parameter
		///     types of the underlying executable refers to a non-existent type
		///     declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if any of
		///     the underlying executable's parameter types refer to a parameterized
		///     type that cannot be instantiated for any reason </exception>
		public virtual Type[] GenericParameterTypes
		{
			get
			{
				if (HasGenericInformation())
				{
					return GenericInfo.ParameterTypes;
				}
				else
				{
					return ParameterTypes;
				}
			}
		}

		/// <summary>
		/// Behaves like {@code getGenericParameterTypes}, but returns type
		/// information for all parameters, including synthetic parameters.
		/// </summary>
		internal virtual Type[] AllGenericParameterTypes
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final boolean genericInfo = hasGenericInformation();
				bool genericInfo = HasGenericInformation();
    
				// Easy case: we don't have generic parameter information.  In
				// this case, we just return the result of
				// getParameterTypes().
				if (!genericInfo)
				{
					return ParameterTypes;
				}
				else
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final boolean realParamData = hasRealParameterData();
					bool realParamData = HasRealParameterData();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Type[] genericParamTypes = getGenericParameterTypes();
					Type[] genericParamTypes = GenericParameterTypes;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Type[] nonGenericParamTypes = getParameterTypes();
					Type[] nonGenericParamTypes = ParameterTypes;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Type[] out = new Type[nonGenericParamTypes.length];
					Type[] @out = new Type[nonGenericParamTypes.Length];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Parameter[] params = getParameters();
					Parameter[] @params = Parameters;
					int fromidx = 0;
					// If we have real parameter data, then we use the
					// synthetic and mandate flags to our advantage.
					if (realParamData)
					{
						for (int i = 0; i < @out.Length; i++)
						{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Parameter param = params[i];
							Parameter param = @params[i];
							if (param.Synthetic || param.Implicit)
							{
								// If we hit a synthetic or mandated parameter,
								// use the non generic parameter info.
								@out[i] = nonGenericParamTypes[i];
							}
							else
							{
								// Otherwise, use the generic parameter info.
								@out[i] = genericParamTypes[fromidx];
								fromidx++;
							}
						}
					}
					else
					{
						// Otherwise, use the non-generic parameter data.
						// Without method parameter reflection data, we have
						// no way to figure out which parameters are
						// synthetic/mandated, thus, no way to match up the
						// indexes.
						return genericParamTypes.Length == nonGenericParamTypes.Length ? genericParamTypes : nonGenericParamTypes;
					}
					return @out;
				}
			}
		}

		/// <summary>
		/// Returns an array of {@code Parameter} objects that represent
		/// all the parameters to the underlying executable represented by
		/// this object.  Returns an array of length 0 if the executable
		/// has no parameters.
		/// 
		/// <para>The parameters of the underlying executable do not necessarily
		/// have unique names, or names that are legal identifiers in the
		/// Java programming language (JLS 3.8).
		/// 
		/// @since 1.8
		/// </para>
		/// </summary>
		/// <exception cref="MalformedParametersException"> if the class file contains
		/// a MethodParameters attribute that is improperly formatted. </exception>
		/// <returns> an array of {@code Parameter} objects representing all
		/// the parameters to the executable this object represents. </returns>
		public virtual Parameter[] Parameters
		{
			get
			{
				// TODO: This may eventually need to be guarded by security
				// mechanisms similar to those in Field, Method, etc.
				//
				// Need to copy the cached array to prevent users from messing
				// with it.  Since parameters are immutable, we can
				// shallow-copy.
				return PrivateGetParameters().clone();
			}
		}

		private Parameter[] SynthesizeAllParams()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int realparams = getParameterCount();
			int realparams = ParameterCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parameter[] out = new Parameter[realparams];
			Parameter[] @out = new Parameter[realparams];
			for (int i = 0; i < realparams; i++)
				// TODO: is there a way to synthetically derive the
				// modifiers?  Probably not in the general case, since
				// we'd have no way of knowing about them, but there
				// may be specific cases.
			{
				@out[i] = new Parameter("arg" + i, 0, this, i);
			}
			return @out;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void verifyParameters(final Parameter[] parameters)
		private void VerifyParameters(Parameter[] parameters)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mask = Modifier.FINAL | Modifier.SYNTHETIC | Modifier.MANDATED;
			int mask = Modifier.FINAL | Modifier.SYNTHETIC | Modifier.MANDATED;

			if (ParameterTypes.Length != parameters.Length)
			{
				throw new MalformedParametersException("Wrong number of parameters in MethodParameters attribute");
			}

			foreach (Parameter parameter in parameters)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = parameter.getRealName();
				String name = parameter.RealName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mods = parameter.getModifiers();
				int mods = parameter.Modifiers;

				if (name != AnnotatedElement_Fields.Null)
				{
					if (name.Empty || name.IndexOf('.') != -1 || name.IndexOf(';') != -1 || name.IndexOf('[') != -1 || name.IndexOf('/') != -1)
					{
						throw new MalformedParametersException("Invalid parameter name \"" + name + "\"");
					}
				}

				if (mods != (mods & mask))
				{
					throw new MalformedParametersException("Invalid parameter modifiers");
				}
			}
		}

		private Parameter[] PrivateGetParameters()
		{
			// Use tmp to avoid multiple writes to a volatile.
			Parameter[] tmp = Parameters_Renamed;

			if (tmp == AnnotatedElement_Fields.Null)
			{

				// Otherwise, go to the JVM to get them
				try
				{
					tmp = Parameters0;
				}
				catch (IllegalArgumentException)
				{
					// Rethrow ClassFormatErrors
					throw new MalformedParametersException("Invalid constant pool index");
				}

				// If we get back nothing, then synthesize parameters
				if (tmp == AnnotatedElement_Fields.Null)
				{
					HasRealParameterData_Renamed = false;
					tmp = SynthesizeAllParams();
				}
				else
				{
					HasRealParameterData_Renamed = true;
					VerifyParameters(tmp);
				}

				Parameters_Renamed = tmp;
			}

			return tmp;
		}

		internal virtual bool HasRealParameterData()
		{
			// If this somehow gets called before parameters gets
			// initialized, force it into existence.
			if (Parameters_Renamed == AnnotatedElement_Fields.Null)
			{
				PrivateGetParameters();
			}
			return HasRealParameterData_Renamed;
		}

		[NonSerialized]
		private volatile bool HasRealParameterData_Renamed;
		[NonSerialized]
		private volatile Parameter[] Parameters_Renamed;

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern Parameter[] getParameters0();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal extern byte[] getTypeAnnotationBytes0();

		// Needed by reflectaccess
		internal virtual sbyte[] TypeAnnotationBytes
		{
			get
			{
				return TypeAnnotationBytes0;
			}
		}

		/// <summary>
		/// Returns an array of {@code Class} objects that represent the
		/// types of exceptions declared to be thrown by the underlying
		/// executable represented by this object.  Returns an array of
		/// length 0 if the executable declares no exceptions in its {@code
		/// throws} clause.
		/// </summary>
		/// <returns> the exception types declared as being thrown by the
		/// executable this object represents </returns>
		public abstract Class[] ExceptionTypes {get;}

		/// <summary>
		/// Returns an array of {@code Type} objects that represent the
		/// exceptions declared to be thrown by this executable object.
		/// Returns an array of length 0 if the underlying executable declares
		/// no exceptions in its {@code throws} clause.
		/// 
		/// <para>If an exception type is a type variable or a parameterized
		/// type, it is created. Otherwise, it is resolved.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of Types that represent the exception types
		///     thrown by the underlying executable </returns>
		/// <exception cref="GenericSignatureFormatError">
		///     if the generic method signature does not conform to the format
		///     specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if the underlying executable's
		///     {@code throws} clause refers to a non-existent type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if
		///     the underlying executable's {@code throws} clause refers to a
		///     parameterized type that cannot be instantiated for any reason </exception>
		public virtual Type[] GenericExceptionTypes
		{
			get
			{
				Type[] AnnotatedElement_Fields.Result;
				if (HasGenericInformation() && ((AnnotatedElement_Fields.Result = GenericInfo.ExceptionTypes).Length > 0))
				{
					return AnnotatedElement_Fields.Result;
				}
				else
				{
					return ExceptionTypes;
				}
			}
		}

		/// <summary>
		/// Returns a string describing this {@code Executable}, including
		/// any type parameters. </summary>
		/// <returns> a string describing this {@code Executable}, including
		/// any type parameters </returns>
		public abstract String ToGenericString();

		/// <summary>
		/// Returns {@code true} if this executable was declared to take a
		/// variable number of arguments; returns {@code false} otherwise.
		/// </summary>
		/// <returns> {@code true} if an only if this executable was declared
		/// to take a variable number of arguments. </returns>
		public virtual bool VarArgs
		{
			get
			{
				return (Modifiers & Modifier.VARARGS) != 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if this executable is a synthetic
		/// construct; returns {@code false} otherwise.
		/// </summary>
		/// <returns> true if and only if this executable is a synthetic
		/// construct as defined by
		/// <cite>The Java&trade; Language Specification</cite>.
		/// @jls 13.1 The Form of a Binary </returns>
		public virtual bool Synthetic
		{
			get
			{
				return Modifier.IsSynthetic(Modifiers);
			}
		}

		/// <summary>
		/// Returns an array of arrays of {@code Annotation}s that
		/// represent the annotations on the formal parameters, in
		/// declaration order, of the {@code Executable} represented by
		/// this object.  Synthetic and mandated parameters (see
		/// explanation below), such as the outer "this" parameter to an
		/// inner class constructor will be represented in the returned
		/// array.  If the executable has no parameters (meaning no formal,
		/// no synthetic, and no mandated parameters), a zero-length array
		/// will be returned.  If the {@code Executable} has one or more
		/// parameters, a nested array of length zero is returned for each
		/// parameter with no annotations. The annotation objects contained
		/// in the returned arrays are serializable.  The caller of this
		/// method is free to modify the returned arrays; it will have no
		/// effect on the arrays returned to other callers.
		/// 
		/// A compiler may add extra parameters that are implicitly
		/// declared in source ("mandated"), as well as parameters that
		/// are neither implicitly nor explicitly declared in source
		/// ("synthetic") to the parameter list for a method.  See {@link
		/// java.lang.reflect.Parameter} for more information.
		/// </summary>
		/// <seealso cref= java.lang.reflect.Parameter </seealso>
		/// <seealso cref= java.lang.reflect.Parameter#getAnnotations </seealso>
		/// <returns> an array of arrays that represent the annotations on
		///    the formal and implicit parameters, in declaration order, of
		///    the executable represented by this object </returns>
		public abstract Annotation[][] ParameterAnnotations {get;}

		internal virtual Annotation[][] SharedGetParameterAnnotations(Class[] parameterTypes, sbyte[] parameterAnnotations)
		{
			int numParameters = parameterTypes.Length;
			if (parameterAnnotations == AnnotatedElement_Fields.Null)
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: return new Annotation[numParameters][0];
				return RectangularArrays.ReturnRectangularAnnotationArray(numParameters, 0);
			}

			Annotation[][] AnnotatedElement_Fields.Result = ParseParameterAnnotations(parameterAnnotations);

			if (AnnotatedElement_Fields.Result.Length != numParameters)
			{
				HandleParameterNumberMismatch(AnnotatedElement_Fields.Result.Length, numParameters);
			}
			return AnnotatedElement_Fields.Result;
		}

		internal abstract void HandleParameterNumberMismatch(int resultLength, int numParameters);

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException">  {@inheritDoc} </exception>
		public override T getAnnotation<T>(Class annotationClass) where T : Annotation
		{
			Objects.RequireNonNull(annotationClass);
			return annotationClass.Cast(DeclaredAnnotations()[annotationClass]);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
		public override T[] getAnnotationsByType<T>(Class annotationClass) where T : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			return AnnotationSupport.getDirectlyAndIndirectlyPresent(DeclaredAnnotations(), annotationClass);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Annotation[] DeclaredAnnotations
		{
			get
			{
				return AnnotationParser.toArray(DeclaredAnnotations());
			}
		}

		[NonSerialized]
		private IDictionary<Class, Annotation> DeclaredAnnotations_Renamed;

		private IDictionary<Class, Annotation> DeclaredAnnotations()
		{
			lock (this)
			{
				if (DeclaredAnnotations_Renamed == AnnotatedElement_Fields.Null)
				{
					Executable root = Root;
					if (root != AnnotatedElement_Fields.Null)
					{
						DeclaredAnnotations_Renamed = root.DeclaredAnnotations();
					}
					else
					{
						DeclaredAnnotations_Renamed = AnnotationParser.parseAnnotations(AnnotationBytes, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), DeclaringClass);
					}
				}
				return DeclaredAnnotations_Renamed;
			}
		}

		/// <summary>
		/// Returns an {@code AnnotatedType} object that represents the use of a type to
		/// specify the return type of the method/constructor represented by this
		/// Executable.
		/// 
		/// If this {@code Executable} object represents a constructor, the {@code
		/// AnnotatedType} object represents the type of the constructed object.
		/// 
		/// If this {@code Executable} object represents a method, the {@code
		/// AnnotatedType} object represents the use of a type to specify the return
		/// type of the method.
		/// </summary>
		/// <returns> an object representing the return type of the method
		/// or constructor represented by this {@code Executable}
		/// 
		/// @since 1.8 </returns>
		public abstract AnnotatedType AnnotatedReturnType {get;}

		/* Helper for subclasses of Executable.
		 *
		 * Returns an AnnotatedType object that represents the use of a type to
		 * specify the return type of the method/constructor represented by this
		 * Executable.
		 *
		 * @since 1.8
		 */
		internal virtual AnnotatedType GetAnnotatedReturnType0(Type returnType)
		{
			return TypeAnnotationParser.buildAnnotatedType(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, returnType, TypeAnnotation.TypeAnnotationTarget.METHOD_RETURN);
		}

		/// <summary>
		/// Returns an {@code AnnotatedType} object that represents the use of a
		/// type to specify the receiver type of the method/constructor represented
		/// by this Executable object. The receiver type of a method/constructor is
		/// available only if the method/constructor has a <em>receiver
		/// parameter</em> (JLS 8.4.1).
		/// 
		/// If this {@code Executable} object represents a constructor or instance
		/// method that does not have a receiver parameter, or has a receiver
		/// parameter with no annotations on its type, then the return value is an
		/// {@code AnnotatedType} object representing an element with no
		/// annotations.
		/// 
		/// If this {@code Executable} object represents a static method, then the
		/// return value is null.
		/// </summary>
		/// <returns> an object representing the receiver type of the method or
		/// constructor represented by this {@code Executable}
		/// 
		/// @since 1.8 </returns>
		public virtual AnnotatedType AnnotatedReceiverType
		{
			get
			{
				if (Modifier.IsStatic(this.Modifiers))
				{
					return AnnotatedElement_Fields.Null;
				}
				return TypeAnnotationParser.buildAnnotatedType(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, DeclaringClass, TypeAnnotation.TypeAnnotationTarget.METHOD_RECEIVER);
			}
		}

		/// <summary>
		/// Returns an array of {@code AnnotatedType} objects that represent the use
		/// of types to specify formal parameter types of the method/constructor
		/// represented by this Executable. The order of the objects in the array
		/// corresponds to the order of the formal parameter types in the
		/// declaration of the method/constructor.
		/// 
		/// Returns an array of length 0 if the method/constructor declares no
		/// parameters.
		/// </summary>
		/// <returns> an array of objects representing the types of the
		/// formal parameters of the method or constructor represented by this
		/// {@code Executable}
		/// 
		/// @since 1.8 </returns>
		public virtual AnnotatedType[] AnnotatedParameterTypes
		{
			get
			{
				return TypeAnnotationParser.buildAnnotatedTypes(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, AllGenericParameterTypes, TypeAnnotation.TypeAnnotationTarget.METHOD_FORMAL_PARAMETER);
			}
		}

		/// <summary>
		/// Returns an array of {@code AnnotatedType} objects that represent the use
		/// of types to specify the declared exceptions of the method/constructor
		/// represented by this Executable. The order of the objects in the array
		/// corresponds to the order of the exception types in the declaration of
		/// the method/constructor.
		/// 
		/// Returns an array of length 0 if the method/constructor declares no
		/// exceptions.
		/// </summary>
		/// <returns> an array of objects representing the declared
		/// exceptions of the method or constructor represented by this {@code
		/// Executable}
		/// 
		/// @since 1.8 </returns>
		public virtual AnnotatedType[] AnnotatedExceptionTypes
		{
			get
			{
				return TypeAnnotationParser.buildAnnotatedTypes(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, GenericExceptionTypes, TypeAnnotation.TypeAnnotationTarget.THROWS);
			}
		}

	}

}