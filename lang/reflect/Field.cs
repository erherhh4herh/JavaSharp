using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
	using FieldAccessor = sun.reflect.FieldAccessor;
	using Reflection = sun.reflect.Reflection;
	using FieldRepository = sun.reflect.generics.repository.FieldRepository;
	using CoreReflectionFactory = sun.reflect.generics.factory.CoreReflectionFactory;
	using GenericsFactory = sun.reflect.generics.factory.GenericsFactory;
	using ClassScope = sun.reflect.generics.scope.ClassScope;
	using AnnotationParser = sun.reflect.annotation.AnnotationParser;
	using AnnotationSupport = sun.reflect.annotation.AnnotationSupport;
	using TypeAnnotation = sun.reflect.annotation.TypeAnnotation;
	using TypeAnnotationParser = sun.reflect.annotation.TypeAnnotationParser;

	/// <summary>
	/// A {@code Field} provides information about, and dynamic access to, a
	/// single field of a class or an interface.  The reflected field may
	/// be a class (static) field or an instance field.
	/// 
	/// <para>A {@code Field} permits widening conversions to occur during a get or
	/// set access operation, but throws an {@code IllegalArgumentException} if a
	/// narrowing conversion would occur.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Member </seealso>
	/// <seealso cref= java.lang.Class </seealso>
	/// <seealso cref= java.lang.Class#getFields() </seealso>
	/// <seealso cref= java.lang.Class#getField(String) </seealso>
	/// <seealso cref= java.lang.Class#getDeclaredFields() </seealso>
	/// <seealso cref= java.lang.Class#getDeclaredField(String)
	/// 
	/// @author Kenneth Russell
	/// @author Nakul Saraiya </seealso>
	public sealed class Field : AccessibleObject, Member
	{

		private Class Clazz;
		private int Slot;
		// This is guaranteed to be interned by the VM in the 1.4
		// reflection implementation
		private String Name_Renamed;
		private Class Type_Renamed;
		private int Modifiers_Renamed;
		// Generics and annotations support
		[NonSerialized]
		private String Signature;
		// generic info repository; lazily initialized
		[NonSerialized]
		private FieldRepository GenericInfo_Renamed;
		private sbyte[] Annotations;
		// Cached field accessor created without override
		private FieldAccessor FieldAccessor;
		// Cached field accessor created with override
		private FieldAccessor OverrideFieldAccessor;
		// For sharing of FieldAccessors. This branching structure is
		// currently only two levels deep (i.e., one root Field and
		// potentially many Field objects pointing to it.)
		//
		// If this branching structure would ever contain cycles, deadlocks can
		// occur in annotation code.
		private Field Root;

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
				Class c = DeclaringClass;
				// create scope and factory
				return CoreReflectionFactory.make(c, ClassScope.make(c));
			}
		}

		// Accessor for generic info repository
		private FieldRepository GenericInfo
		{
			get
			{
				// lazily initialize repository if necessary
				if (GenericInfo_Renamed == AnnotatedElement_Fields.Null)
				{
					// create and cache generic info repository
					GenericInfo_Renamed = FieldRepository.make(GenericSignature, Factory);
				}
				return GenericInfo_Renamed; //return cached repository
			}
		}


		/// <summary>
		/// Package-private constructor used by ReflectAccess to enable
		/// instantiation of these objects in Java code from the java.lang
		/// package via sun.reflect.LangReflectAccess.
		/// </summary>
		internal Field(Class declaringClass, String name, Class type, int modifiers, int slot, String signature, sbyte[] annotations)
		{
			this.Clazz = declaringClass;
			this.Name_Renamed = name;
			this.Type_Renamed = type;
			this.Modifiers_Renamed = modifiers;
			this.Slot = slot;
			this.Signature = signature;
			this.Annotations = annotations;
		}

		/// <summary>
		/// Package-private routine (exposed to java.lang.Class via
		/// ReflectAccess) which returns a copy of this Field. The copy's
		/// "root" field points to this Field.
		/// </summary>
		internal Field Copy()
		{
			// This routine enables sharing of FieldAccessor objects
			// among Field objects which refer to the same underlying
			// method in the VM. (All of this contortion is only necessary
			// because of the "accessibility" bit in AccessibleObject,
			// which implicitly requires that new java.lang.reflect
			// objects be fabricated for each reflective call on Class
			// objects.)
			if (this.Root != AnnotatedElement_Fields.Null)
			{
				throw new IllegalArgumentException("Can not copy a non-root Field");
			}

			Field res = new Field(Clazz, Name_Renamed, Type_Renamed, Modifiers_Renamed, Slot, Signature, Annotations);
			res.Root = this;
			// Might as well eagerly propagate this if already present
			res.FieldAccessor = FieldAccessor;
			res.OverrideFieldAccessor = OverrideFieldAccessor;

			return res;
		}

		/// <summary>
		/// Returns the {@code Class} object representing the class or interface
		/// that declares the field represented by this {@code Field} object.
		/// </summary>
		public Class DeclaringClass
		{
			get
			{
				return Clazz;
			}
		}

		/// <summary>
		/// Returns the name of the field represented by this {@code Field} object.
		/// </summary>
		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the Java language modifiers for the field represented
		/// by this {@code Field} object, as an integer. The {@code Modifier} class should
		/// be used to decode the modifiers.
		/// </summary>
		/// <seealso cref= Modifier </seealso>
		public int Modifiers
		{
			get
			{
				return Modifiers_Renamed;
			}
		}

		/// <summary>
		/// Returns {@code true} if this field represents an element of
		/// an enumerated type; returns {@code false} otherwise.
		/// </summary>
		/// <returns> {@code true} if and only if this field represents an element of
		/// an enumerated type.
		/// @since 1.5 </returns>
		public bool EnumConstant
		{
			get
			{
				return (Modifiers & Modifier.ENUM) != 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if this field is a synthetic
		/// field; returns {@code false} otherwise.
		/// </summary>
		/// <returns> true if and only if this field is a synthetic
		/// field as defined by the Java Language Specification.
		/// @since 1.5 </returns>
		public bool Synthetic
		{
			get
			{
				return Modifier.IsSynthetic(Modifiers);
			}
		}

		/// <summary>
		/// Returns a {@code Class} object that identifies the
		/// declared type for the field represented by this
		/// {@code Field} object.
		/// </summary>
		/// <returns> a {@code Class} object identifying the declared
		/// type of the field represented by this object </returns>
		public Class Type
		{
			get
			{
				return Type_Renamed;
			}
		}

		/// <summary>
		/// Returns a {@code Type} object that represents the declared type for
		/// the field represented by this {@code Field} object.
		/// 
		/// <para>If the {@code Type} is a parameterized type, the
		/// {@code Type} object returned must accurately reflect the
		/// actual type parameters used in the source code.
		/// 
		/// </para>
		/// <para>If the type of the underlying field is a type variable or a
		/// parameterized type, it is created. Otherwise, it is resolved.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Type} object that represents the declared type for
		///     the field represented by this {@code Field} object </returns>
		/// <exception cref="GenericSignatureFormatError"> if the generic field
		///     signature does not conform to the format specified in
		///     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		/// <exception cref="TypeNotPresentException"> if the generic type
		///     signature of the underlying field refers to a non-existent
		///     type declaration </exception>
		/// <exception cref="MalformedParameterizedTypeException"> if the generic
		///     signature of the underlying field refers to a parameterized type
		///     that cannot be instantiated for any reason
		/// @since 1.5 </exception>
		public Type GenericType
		{
			get
			{
				if (GenericSignature != AnnotatedElement_Fields.Null)
				{
					return GenericInfo.GenericType;
				}
				else
				{
					return Type;
				}
			}
		}


		/// <summary>
		/// Compares this {@code Field} against the specified object.  Returns
		/// true if the objects are the same.  Two {@code Field} objects are the same if
		/// they were declared by the same class and have the same name
		/// and type.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj != AnnotatedElement_Fields.Null && obj is Field)
			{
				Field other = (Field)obj;
				return (DeclaringClass == other.DeclaringClass) && (Name == other.Name) && (Type == other.Type);
			}
			return false;
		}

		/// <summary>
		/// Returns a hashcode for this {@code Field}.  This is computed as the
		/// exclusive-or of the hashcodes for the underlying field's
		/// declaring class name and its name.
		/// </summary>
		public override int HashCode()
		{
			return DeclaringClass.Name.HashCode() ^ Name.HashCode();
		}

		/// <summary>
		/// Returns a string describing this {@code Field}.  The format is
		/// the access modifiers for the field, if any, followed
		/// by the field type, followed by a space, followed by
		/// the fully-qualified name of the class declaring the field,
		/// followed by a period, followed by the name of the field.
		/// For example:
		/// <pre>
		///    public static final int java.lang.Thread.MIN_PRIORITY
		///    private int java.io.FileDescriptor.fd
		/// </pre>
		/// 
		/// <para>The modifiers are placed in canonical order as specified by
		/// "The Java Language Specification".  This is {@code public},
		/// {@code protected} or {@code private} first, and then other
		/// modifiers in the following order: {@code static}, {@code final},
		/// {@code transient}, {@code volatile}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Field}
		/// @jls 8.3.1 Field Modifiers </returns>
		public override String ToString()
		{
			int mod = Modifiers;
			return (((mod == 0) ? "" : (Modifier.ToString(mod) + " ")) + Type.TypeName + " " + DeclaringClass.TypeName + "." + Name);
		}

		/// <summary>
		/// Returns a string describing this {@code Field}, including
		/// its generic type.  The format is the access modifiers for the
		/// field, if any, followed by the generic field type, followed by
		/// a space, followed by the fully-qualified name of the class
		/// declaring the field, followed by a period, followed by the name
		/// of the field.
		/// 
		/// <para>The modifiers are placed in canonical order as specified by
		/// "The Java Language Specification".  This is {@code public},
		/// {@code protected} or {@code private} first, and then other
		/// modifiers in the following order: {@code static}, {@code final},
		/// {@code transient}, {@code volatile}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string describing this {@code Field}, including
		/// its generic type
		/// 
		/// @since 1.5
		/// @jls 8.3.1 Field Modifiers </returns>
		public String ToGenericString()
		{
			int mod = Modifiers;
			Type fieldType = GenericType;
			return (((mod == 0) ? "" : (Modifier.ToString(mod) + " ")) + fieldType.TypeName + " " + DeclaringClass.TypeName + "." + Name);
		}

		/// <summary>
		/// Returns the value of the field represented by this {@code Field}, on
		/// the specified object. The value is automatically wrapped in an
		/// object if it has a primitive type.
		/// 
		/// <para>The underlying field's value is obtained as follows:
		/// 
		/// </para>
		/// <para>If the underlying field is a static field, the {@code obj} argument
		/// is ignored; it may be null.
		/// 
		/// </para>
		/// <para>Otherwise, the underlying field is an instance field.  If the
		/// specified {@code obj} argument is null, the method throws a
		/// {@code NullPointerException}. If the specified object is not an
		/// instance of the class or interface declaring the underlying
		/// field, the method throws an {@code IllegalArgumentException}.
		/// 
		/// </para>
		/// <para>If this {@code Field} object is enforcing Java language access control, and
		/// the underlying field is inaccessible, the method throws an
		/// {@code IllegalAccessException}.
		/// If the underlying field is static, the class that declared the
		/// field is initialized if it has not already been initialized.
		/// 
		/// </para>
		/// <para>Otherwise, the value is retrieved from the underlying instance
		/// or static field.  If the field has a primitive type, the value
		/// is wrapped in an object before being returned, otherwise it is
		/// returned as is.
		/// 
		/// </para>
		/// <para>If the field is hidden in the type of {@code obj},
		/// the field's value is obtained according to the preceding rules.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> object from which the represented field's value is
		/// to be extracted </param>
		/// <returns> the value of the represented field in object
		/// {@code obj}; primitive values are wrapped in an appropriate
		/// object before being returned
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof). </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Object get(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public Object Get(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).get(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance {@code boolean} field.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code boolean} value
		/// from </param>
		/// <returns> the value of the {@code boolean} field
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code boolean} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public boolean getBoolean(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public bool GetBoolean(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getBoolean(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance {@code byte} field.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code byte} value
		/// from </param>
		/// <returns> the value of the {@code byte} field
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code byte} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public byte getByte(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public sbyte GetByte(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getByte(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code char} or of another primitive type convertible to
		/// type {@code char} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code char} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code char}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code char} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref= Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public char getChar(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public char GetChar(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getChar(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code short} or of another primitive type convertible to
		/// type {@code short} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code short} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code short}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code short} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public short getShort(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public short GetShort(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getShort(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code int} or of another primitive type convertible to
		/// type {@code int} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code int} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code int}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code int} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public int getInt(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public int GetInt(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getInt(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code long} or of another primitive type convertible to
		/// type {@code long} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code long} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code long}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code long} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public long getLong(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public long GetLong(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getLong(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code float} or of another primitive type convertible to
		/// type {@code float} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code float} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code float}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code float} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref= Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public float getFloat(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public float GetFloat(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getFloat(obj);
		}

		/// <summary>
		/// Gets the value of a static or instance field of type
		/// {@code double} or of another primitive type convertible to
		/// type {@code double} via a widening conversion.
		/// </summary>
		/// <param name="obj"> the object to extract the {@code double} value
		/// from </param>
		/// <returns> the value of the field converted to type {@code double}
		/// </returns>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is inaccessible. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not
		///              an instance of the class or interface declaring the
		///              underlying field (or a subclass or implementor
		///              thereof), or if the field value cannot be
		///              converted to the type {@code double} by a
		///              widening conversion. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#get </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public double getDouble(Object obj) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public double GetDouble(Object obj)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			return GetFieldAccessor(obj).getDouble(obj);
		}

		/// <summary>
		/// Sets the field represented by this {@code Field} object on the
		/// specified object argument to the specified new value. The new
		/// value is automatically unwrapped if the underlying field has a
		/// primitive type.
		/// 
		/// <para>The operation proceeds as follows:
		/// 
		/// </para>
		/// <para>If the underlying field is static, the {@code obj} argument is
		/// ignored; it may be null.
		/// 
		/// </para>
		/// <para>Otherwise the underlying field is an instance field.  If the
		/// specified object argument is null, the method throws a
		/// {@code NullPointerException}.  If the specified object argument is not
		/// an instance of the class or interface declaring the underlying
		/// field, the method throws an {@code IllegalArgumentException}.
		/// 
		/// </para>
		/// <para>If this {@code Field} object is enforcing Java language access control, and
		/// the underlying field is inaccessible, the method throws an
		/// {@code IllegalAccessException}.
		/// 
		/// </para>
		/// <para>If the underlying field is final, the method throws an
		/// {@code IllegalAccessException} unless {@code setAccessible(true)}
		/// has succeeded for this {@code Field} object
		/// and the field is non-static. Setting a final field in this way
		/// is meaningful only during deserialization or reconstruction of
		/// instances of classes with blank final fields, before they are
		/// made available for access by other parts of a program. Use in
		/// any other context may have unpredictable effects, including cases
		/// in which other parts of a program continue to use the original
		/// value of this field.
		/// 
		/// </para>
		/// <para>If the underlying field is of a primitive type, an unwrapping
		/// conversion is attempted to convert the new value to a value of
		/// a primitive type.  If this attempt fails, the method throws an
		/// {@code IllegalArgumentException}.
		/// 
		/// </para>
		/// <para>If, after possible unwrapping, the new value cannot be
		/// converted to the type of the underlying field by an identity or
		/// widening conversion, the method throws an
		/// {@code IllegalArgumentException}.
		/// 
		/// </para>
		/// <para>If the underlying field is static, the class that declared the
		/// field is initialized if it has not already been initialized.
		/// 
		/// </para>
		/// <para>The field is set to the possibly unwrapped and widened new value.
		/// 
		/// </para>
		/// <para>If the field is hidden in the type of {@code obj},
		/// the field's value is set according to the preceding rules.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="value"> the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void set(Object obj, Object value) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void Set(Object obj, Object value)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).set(obj, value);
		}

		/// <summary>
		/// Sets the value of a field as a {@code boolean} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, zObj)},
		/// where {@code zObj} is a {@code Boolean} object and
		/// {@code zObj.booleanValue() == z}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="z">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setBoolean(Object obj, boolean z) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetBoolean(Object obj, bool z)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setBoolean(obj, z);
		}

		/// <summary>
		/// Sets the value of a field as a {@code byte} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, bObj)},
		/// where {@code bObj} is a {@code Byte} object and
		/// {@code bObj.byteValue() == b}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="b">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setByte(Object obj, byte b) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetByte(Object obj, sbyte b)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setByte(obj, b);
		}

		/// <summary>
		/// Sets the value of a field as a {@code char} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, cObj)},
		/// where {@code cObj} is a {@code Character} object and
		/// {@code cObj.charValue() == c}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="c">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setChar(Object obj, char c) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetChar(Object obj, char c)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setChar(obj, c);
		}

		/// <summary>
		/// Sets the value of a field as a {@code short} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, sObj)},
		/// where {@code sObj} is a {@code Short} object and
		/// {@code sObj.shortValue() == s}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="s">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setShort(Object obj, short s) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetShort(Object obj, short s)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setShort(obj, s);
		}

		/// <summary>
		/// Sets the value of a field as an {@code int} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, iObj)},
		/// where {@code iObj} is a {@code Integer} object and
		/// {@code iObj.intValue() == i}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="i">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setInt(Object obj, int i) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetInt(Object obj, int i)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setInt(obj, i);
		}

		/// <summary>
		/// Sets the value of a field as a {@code long} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, lObj)},
		/// where {@code lObj} is a {@code Long} object and
		/// {@code lObj.longValue() == l}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="l">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setLong(Object obj, long l) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetLong(Object obj, long l)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setLong(obj, l);
		}

		/// <summary>
		/// Sets the value of a field as a {@code float} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, fObj)},
		/// where {@code fObj} is a {@code Float} object and
		/// {@code fObj.floatValue() == f}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="f">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setFloat(Object obj, float f) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetFloat(Object obj, float f)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setFloat(obj, f);
		}

		/// <summary>
		/// Sets the value of a field as a {@code double} on the specified object.
		/// This method is equivalent to
		/// {@code set(obj, dObj)},
		/// where {@code dObj} is a {@code Double} object and
		/// {@code dObj.doubleValue() == d}.
		/// </summary>
		/// <param name="obj"> the object whose field should be modified </param>
		/// <param name="d">   the new value for the field of {@code obj}
		/// being modified
		/// </param>
		/// <exception cref="IllegalAccessException">    if this {@code Field} object
		///              is enforcing Java language access control and the underlying
		///              field is either inaccessible or final. </exception>
		/// <exception cref="IllegalArgumentException">  if the specified object is not an
		///              instance of the class or interface declaring the underlying
		///              field (or a subclass or implementor thereof),
		///              or if an unwrapping conversion fails. </exception>
		/// <exception cref="NullPointerException">      if the specified object is null
		///              and the field is an instance field. </exception>
		/// <exception cref="ExceptionInInitializerError"> if the initialization provoked
		///              by this method fails. </exception>
		/// <seealso cref=       Field#set </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public void setDouble(Object obj, double d) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public void SetDouble(Object obj, double d)
		{
			if (!@override)
			{
				if (!Reflection.quickCheckMemberAccess(Clazz, Modifiers_Renamed))
				{
					Class caller = Reflection.CallerClass;
					CheckAccess(caller, Clazz, obj, Modifiers_Renamed);
				}
			}
			GetFieldAccessor(obj).setDouble(obj, d);
		}

		// security check is done before calling this method
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private sun.reflect.FieldAccessor getFieldAccessor(Object obj) throws IllegalAccessException
		private FieldAccessor GetFieldAccessor(Object obj)
		{
			bool ov = @override;
			FieldAccessor a = (ov) ? OverrideFieldAccessor : FieldAccessor;
			return (a != AnnotatedElement_Fields.Null) ? a : AcquireFieldAccessor(ov);
		}

		// NOTE that there is no synchronization used here. It is correct
		// (though not efficient) to generate more than one FieldAccessor
		// for a given Field. However, avoiding synchronization will
		// probably make the implementation more scalable.
		private FieldAccessor AcquireFieldAccessor(bool overrideFinalCheck)
		{
			// First check to see if one has been created yet, and take it
			// if so
			FieldAccessor tmp = AnnotatedElement_Fields.Null;
			if (Root != AnnotatedElement_Fields.Null)
			{
				tmp = Root.GetFieldAccessor(overrideFinalCheck);
			}
			if (tmp != AnnotatedElement_Fields.Null)
			{
				if (overrideFinalCheck)
				{
					OverrideFieldAccessor = tmp;
				}
				else
				{
					FieldAccessor = tmp;
				}
			}
			else
			{
				// Otherwise fabricate one and propagate it up to the root
				tmp = ReflectionFactory.newFieldAccessor(this, overrideFinalCheck);
				SetFieldAccessor(tmp, overrideFinalCheck);
			}

			return tmp;
		}

		// Returns FieldAccessor for this Field object, not looking up
		// the chain to the root
		private FieldAccessor GetFieldAccessor(bool overrideFinalCheck)
		{
			return (overrideFinalCheck)? OverrideFieldAccessor : FieldAccessor;
		}

		// Sets the FieldAccessor for this Field object and
		// (recursively) its root
		private void SetFieldAccessor(FieldAccessor accessor, bool overrideFinalCheck)
		{
			if (overrideFinalCheck)
			{
				OverrideFieldAccessor = accessor;
			}
			else
			{
				FieldAccessor = accessor;
			}
			// Propagate up
			if (Root != AnnotatedElement_Fields.Null)
			{
				Root.SetFieldAccessor(accessor, overrideFinalCheck);
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.5 </exception>
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
					Field root = this.Root;
					if (root != AnnotatedElement_Fields.Null)
					{
						DeclaredAnnotations_Renamed = root.DeclaredAnnotations();
					}
					else
					{
						DeclaredAnnotations_Renamed = AnnotationParser.parseAnnotations(Annotations, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), DeclaringClass);
					}
				}
				return DeclaredAnnotations_Renamed;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern byte[] getTypeAnnotationBytes0();

		/// <summary>
		/// Returns an AnnotatedType object that represents the use of a type to specify
		/// the declared type of the field represented by this Field. </summary>
		/// <returns> an object representing the declared type of the field
		/// represented by this Field
		/// 
		/// @since 1.8 </returns>
		public AnnotatedType AnnotatedType
		{
			get
			{
				return TypeAnnotationParser.buildAnnotatedType(TypeAnnotationBytes0, sun.misc.SharedSecrets.JavaLangAccess.getConstantPool(DeclaringClass), this, DeclaringClass, GenericType, TypeAnnotation.TypeAnnotationTarget.FIELD);
			}
		}
	}

}