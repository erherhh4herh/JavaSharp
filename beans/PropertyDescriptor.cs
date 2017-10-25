using System;

/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// A PropertyDescriptor describes one property that a Java Bean
	/// exports via a pair of accessor methods.
	/// </summary>
	public class PropertyDescriptor : FeatureDescriptor
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> propertyTypeRef;
		private Reference<?> PropertyTypeRef;
		private readonly MethodRef ReadMethodRef = new MethodRef();
		private readonly MethodRef WriteMethodRef = new MethodRef();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> propertyEditorClassRef;
		private Reference<?> PropertyEditorClassRef;

		private bool Bound_Renamed;
		private bool Constrained_Renamed;

		// The base name of the method name which will be prefixed with the
		// read and write method. If name == "foo" then the baseName is "Foo"
		private String BaseName_Renamed;

		private String WriteMethodName;
		private String ReadMethodName;

		/// <summary>
		/// Constructs a PropertyDescriptor for a property that follows
		/// the standard Java convention by having getFoo and setFoo
		/// accessor methods.  Thus if the argument name is "fred", it will
		/// assume that the writer method is "setFred" and the reader method
		/// is "getFred" (or "isFred" for a boolean property).  Note that the
		/// property name should start with a lower case character, which will
		/// be capitalized in the method names.
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="beanClass"> The Class object for the target bean.  For
		///          example sun.beans.OurButton.class. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PropertyDescriptor(String propertyName, Class beanClass) throws IntrospectionException
		public PropertyDescriptor(String propertyName, Class beanClass) : this(propertyName, beanClass, Introspector.IS_PREFIX + NameGenerator.Capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.Capitalize(propertyName))
		{
		}

		/// <summary>
		/// This constructor takes the name of a simple property, and method
		/// names for reading and writing the property.
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="beanClass"> The Class object for the target bean.  For
		///          example sun.beans.OurButton.class. </param>
		/// <param name="readMethodName"> The name of the method used for reading the property
		///           value.  May be null if the property is write-only. </param>
		/// <param name="writeMethodName"> The name of the method used for writing the property
		///           value.  May be null if the property is read-only. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PropertyDescriptor(String propertyName, Class beanClass, String readMethodName, String writeMethodName) throws IntrospectionException
		public PropertyDescriptor(String propertyName, Class beanClass, String readMethodName, String writeMethodName)
		{
			if (beanClass == null)
			{
				throw new IntrospectionException("Target Bean class is null");
			}
			if (propertyName == null || propertyName.Length() == 0)
			{
				throw new IntrospectionException("bad property name");
			}
			if ("".Equals(readMethodName) || "".Equals(writeMethodName))
			{
				throw new IntrospectionException("read or write method name should not be the empty string");
			}
			Name = propertyName;
			Class0 = beanClass;

			this.ReadMethodName = readMethodName;
			if (readMethodName != null && ReadMethod == null)
			{
				throw new IntrospectionException("Method not found: " + readMethodName);
			}
			this.WriteMethodName = writeMethodName;
			if (writeMethodName != null && WriteMethod == null)
			{
				throw new IntrospectionException("Method not found: " + writeMethodName);
			}
			// If this class or one of its base classes allow PropertyChangeListener,
			// then we assume that any properties we discover are "bound".
			// See Introspector.getTargetPropertyInfo() method.
			Class[] args = new Class[] {typeof(PropertyChangeListener)};
			this.Bound_Renamed = null != Introspector.FindMethod(beanClass, "addPropertyChangeListener", args.Length, args);
		}

		/// <summary>
		/// This constructor takes the name of a simple property, and Method
		/// objects for reading and writing the property.
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="readMethod"> The method used for reading the property value.
		///          May be null if the property is write-only. </param>
		/// <param name="writeMethod"> The method used for writing the property value.
		///          May be null if the property is read-only. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PropertyDescriptor(String propertyName, Method readMethod, Method writeMethod) throws IntrospectionException
		public PropertyDescriptor(String propertyName, Method readMethod, Method writeMethod)
		{
			if (propertyName == null || propertyName.Length() == 0)
			{
				throw new IntrospectionException("bad property name");
			}
			Name = propertyName;
			ReadMethod = readMethod;
			WriteMethod = writeMethod;
		}

		/// <summary>
		/// Creates <code>PropertyDescriptor</code> for the specified bean
		/// with the specified name and methods to read/write the property value.
		/// </summary>
		/// <param name="bean">   the type of the target bean </param>
		/// <param name="base">   the base name of the property (the rest of the method name) </param>
		/// <param name="read">   the method used for reading the property value </param>
		/// <param name="write">  the method used for writing the property value </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during introspection
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PropertyDescriptor(Class bean, String base, Method read, Method write) throws IntrospectionException
		internal PropertyDescriptor(Class bean, String @base, Method read, Method write)
		{
			if (bean == null)
			{
				throw new IntrospectionException("Target Bean class is null");
			}
			Class0 = bean;
			Name = Introspector.Decapitalize(@base);
			ReadMethod = read;
			WriteMethod = write;
			this.BaseName_Renamed = @base;
		}

		/// <summary>
		/// Returns the Java type info for the property.
		/// Note that the {@code Class} object may describe
		/// primitive Java types such as {@code int}.
		/// This type is returned by the read method
		/// or is used as the parameter type of the write method.
		/// Returns {@code null} if the type is an indexed property
		/// that does not support non-indexed access.
		/// </summary>
		/// <returns> the {@code Class} object that represents the Java type info,
		///         or {@code null} if the type cannot be determined </returns>
		public virtual Class PropertyType
		{
			get
			{
				lock (this)
				{
					Class type = PropertyType0;
					if (type == null)
					{
						try
						{
							type = FindPropertyType(ReadMethod, WriteMethod);
							PropertyType = type;
						}
						catch (IntrospectionException)
						{
							// Fall
						}
					}
					return type;
				}
			}
			set
			{
				this.PropertyTypeRef = GetWeakReference(value);
			}
		}


		private Class PropertyType0
		{
			get
			{
				return (this.PropertyTypeRef != null) ? this.PropertyTypeRef.get() : null;
			}
		}

		/// <summary>
		/// Gets the method that should be used to read the property value.
		/// </summary>
		/// <returns> The method that should be used to read the property value.
		/// May return null if the property can't be read. </returns>
		public virtual Method ReadMethod
		{
			get
			{
				lock (this)
				{
					Method readMethod = this.ReadMethodRef.Get();
					if (readMethod == null)
					{
						Class cls = Class0;
						if (cls == null || (ReadMethodName == null && !this.ReadMethodRef.Set))
						{
							// The read method was explicitly set to null.
							return null;
						}
						String nextMethodName = Introspector.GET_PREFIX + BaseName;
						if (ReadMethodName == null)
						{
							Class type = PropertyType0;
							if (type == typeof(bool) || type == null)
							{
								ReadMethodName = Introspector.IS_PREFIX + BaseName;
							}
							else
							{
								ReadMethodName = nextMethodName;
							}
						}
            
						// Since there can be multiple write methods but only one getter
						// method, find the getter method first so that you know what the
						// property type is.  For booleans, there can be "is" and "get"
						// methods.  If an "is" method exists, this is the official
						// reader method so look for this one first.
						readMethod = Introspector.FindMethod(cls, ReadMethodName, 0);
						if ((readMethod == null) && !ReadMethodName.Equals(nextMethodName))
						{
							ReadMethodName = nextMethodName;
							readMethod = Introspector.FindMethod(cls, ReadMethodName, 0);
						}
						try
						{
							ReadMethod = readMethod;
						}
						catch (IntrospectionException)
						{
							// fall
						}
					}
					return readMethod;
				}
			}
			set
			{
				lock (this)
				{
					this.ReadMethodRef.Set(value);
					if (value == null)
					{
						ReadMethodName = null;
						return;
					}
					// The property type is determined by the read method.
					PropertyType = FindPropertyType(value, this.WriteMethodRef.Get());
					Class0 = value.DeclaringClass;
            
					ReadMethodName = value.Name;
					SetTransient(value.getAnnotation(typeof(Transient)));
				}
			}
		}


		/// <summary>
		/// Gets the method that should be used to write the property value.
		/// </summary>
		/// <returns> The method that should be used to write the property value.
		/// May return null if the property can't be written. </returns>
		public virtual Method WriteMethod
		{
			get
			{
				lock (this)
				{
					Method writeMethod = this.WriteMethodRef.Get();
					if (writeMethod == null)
					{
						Class cls = Class0;
						if (cls == null || (WriteMethodName == null && !this.WriteMethodRef.Set))
						{
							// The write method was explicitly set to null.
							return null;
						}
            
						// We need the type to fetch the correct method.
						Class type = PropertyType0;
						if (type == null)
						{
							try
							{
								// Can't use getPropertyType since it will lead to recursive loop.
								type = FindPropertyType(ReadMethod, null);
								PropertyType = type;
							}
							catch (IntrospectionException)
							{
								// Without the correct property type we can't be guaranteed
								// to find the correct method.
								return null;
							}
						}
            
						if (WriteMethodName == null)
						{
							WriteMethodName = Introspector.SET_PREFIX + BaseName;
						}
            
						Class[] args = (type == null) ? null : new Class[] {type};
						writeMethod = Introspector.FindMethod(cls, WriteMethodName, 1, args);
						if (writeMethod != null)
						{
							if (!writeMethod.ReturnType.Equals(typeof(void)))
							{
								writeMethod = null;
							}
						}
						try
						{
							WriteMethod = writeMethod;
						}
						catch (IntrospectionException)
						{
							// fall through
						}
					}
					return writeMethod;
				}
			}
			set
			{
				lock (this)
				{
					this.WriteMethodRef.Set(value);
					if (value == null)
					{
						WriteMethodName = null;
						return;
					}
					// Set the property type - which validates the method
					PropertyType = FindPropertyType(ReadMethod, value);
					Class0 = value.DeclaringClass;
            
					WriteMethodName = value.Name;
					SetTransient(value.getAnnotation(typeof(Transient)));
				}
			}
		}


		/// <summary>
		/// Overridden to ensure that a super class doesn't take precedent
		/// </summary>
		internal override Class Class0
		{
			set
			{
				if (Class0 != null && Class0.IsSubclassOf(value))
				{
					// don't replace a subclass with a superclass
					return;
				}
				base.Class0 = value;
			}
		}

		/// <summary>
		/// Updates to "bound" properties will cause a "PropertyChange" event to
		/// get fired when the property is changed.
		/// </summary>
		/// <returns> True if this is a bound property. </returns>
		public virtual bool Bound
		{
			get
			{
				return Bound_Renamed;
			}
			set
			{
				this.Bound_Renamed = value;
			}
		}


		/// <summary>
		/// Attempted updates to "Constrained" properties will cause a "VetoableChange"
		/// event to get fired when the property is changed.
		/// </summary>
		/// <returns> True if this is a constrained property. </returns>
		public virtual bool Constrained
		{
			get
			{
				return Constrained_Renamed;
			}
			set
			{
				this.Constrained_Renamed = value;
			}
		}



		/// <summary>
		/// Normally PropertyEditors will be found using the PropertyEditorManager.
		/// However if for some reason you want to associate a particular
		/// PropertyEditor with a given property, then you can do it with
		/// this method.
		/// </summary>
		/// <param name="propertyEditorClass">  The Class for the desired PropertyEditor. </param>
		public virtual Class PropertyEditorClass
		{
			set
			{
				this.PropertyEditorClassRef = GetWeakReference(value);
			}
			get
			{
				return (this.PropertyEditorClassRef != null) ? this.PropertyEditorClassRef.get() : null;
			}
		}


		/// <summary>
		/// Constructs an instance of a property editor using the current
		/// property editor class.
		/// <para>
		/// If the property editor class has a public constructor that takes an
		/// Object argument then it will be invoked using the bean parameter
		/// as the argument. Otherwise, the default constructor will be invoked.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bean"> the source object </param>
		/// <returns> a property editor instance or null if a property editor has
		///         not been defined or cannot be created
		/// @since 1.5 </returns>
		public virtual PropertyEditor CreatePropertyEditor(Object bean)
		{
			Object editor = null;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class cls = getPropertyEditorClass();
			Class cls = PropertyEditorClass;
			if (cls != null && cls.IsSubclassOf(typeof(PropertyEditor)) && ReflectUtil.isPackageAccessible(cls))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> ctor = null;
				Constructor<?> ctor = null;
				if (bean != null)
				{
					try
					{
						ctor = cls.GetConstructor(new Class[] {typeof(Object)});
					}
					catch (Exception)
					{
						// Fall through
					}
				}
				try
				{
					if (ctor == null)
					{
						editor = cls.NewInstance();
					}
					else
					{
						editor = ctor.newInstance(new Object[] {bean});
					}
				}
				catch (Exception)
				{
					// Fall through
				}
			}
			return (PropertyEditor)editor;
		}


		/// <summary>
		/// Compares this <code>PropertyDescriptor</code> against the specified object.
		/// Returns true if the objects are the same. Two <code>PropertyDescriptor</code>s
		/// are the same if the read, write, property types, property editor and
		/// flags  are equivalent.
		/// 
		/// @since 1.4
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj != null && obj is PropertyDescriptor)
			{
				PropertyDescriptor other = (PropertyDescriptor)obj;
				Method otherReadMethod = other.ReadMethod;
				Method otherWriteMethod = other.WriteMethod;

				if (!CompareMethods(ReadMethod, otherReadMethod))
				{
					return false;
				}

				if (!CompareMethods(WriteMethod, otherWriteMethod))
				{
					return false;
				}

				if (PropertyType == other.PropertyType && PropertyEditorClass == other.PropertyEditorClass && Bound_Renamed == other.Bound && Constrained_Renamed == other.Constrained && WriteMethodName == other.WriteMethodName && ReadMethodName == other.ReadMethodName)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Package private helper method for Descriptor .equals methods.
		/// </summary>
		/// <param name="a"> first method to compare </param>
		/// <param name="b"> second method to compare </param>
		/// <returns> boolean to indicate that the methods are equivalent </returns>
		internal virtual bool CompareMethods(Method a, Method b)
		{
			// Note: perhaps this should be a protected method in FeatureDescriptor
			if ((a == null) != (b == null))
			{
				return false;
			}

			if (a != null && b != null)
			{
				if (!a.Equals(b))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Package-private constructor.
		/// Merge two property descriptors.  Where they conflict, give the
		/// second argument (y) priority over the first argument (x).
		/// </summary>
		/// <param name="x">  The first (lower priority) PropertyDescriptor </param>
		/// <param name="y">  The second (higher priority) PropertyDescriptor </param>
		internal PropertyDescriptor(PropertyDescriptor x, PropertyDescriptor y) : base(x,y)
		{

			if (y.BaseName_Renamed != null)
			{
				BaseName_Renamed = y.BaseName_Renamed;
			}
			else
			{
				BaseName_Renamed = x.BaseName_Renamed;
			}

			if (y.ReadMethodName != null)
			{
				ReadMethodName = y.ReadMethodName;
			}
			else
			{
				ReadMethodName = x.ReadMethodName;
			}

			if (y.WriteMethodName != null)
			{
				WriteMethodName = y.WriteMethodName;
			}
			else
			{
				WriteMethodName = x.WriteMethodName;
			}

			if (y.PropertyTypeRef != null)
			{
				PropertyTypeRef = y.PropertyTypeRef;
			}
			else
			{
				PropertyTypeRef = x.PropertyTypeRef;
			}

			// Figure out the merged read method.
			Method xr = x.ReadMethod;
			Method yr = y.ReadMethod;

			// Normally give priority to y's readMethod.
			try
			{
				if (IsAssignable(xr, yr))
				{
					ReadMethod = yr;
				}
				else
				{
					ReadMethod = xr;
				}
			}
			catch (IntrospectionException)
			{
				// fall through
			}

			// However, if both x and y reference read methods in the same class,
			// give priority to a boolean "is" method over a boolean "get" method.
			if (xr != null && yr != null && xr.DeclaringClass == yr.DeclaringClass && GetReturnType(Class0, xr) == typeof(bool) && GetReturnType(Class0, yr) == typeof(bool) && xr.Name.IndexOf(Introspector.IS_PREFIX, StringComparison.Ordinal) == 0 && yr.Name.IndexOf(Introspector.GET_PREFIX, StringComparison.Ordinal) == 0)
			{
				try
				{
					ReadMethod = xr;
				}
				catch (IntrospectionException)
				{
					// fall through
				}
			}

			Method xw = x.WriteMethod;
			Method yw = y.WriteMethod;

			try
			{
				if (yw != null)
				{
					WriteMethod = yw;
				}
				else
				{
					WriteMethod = xw;
				}
			}
			catch (IntrospectionException)
			{
				// Fall through
			}

			if (y.PropertyEditorClass != null)
			{
				PropertyEditorClass = y.PropertyEditorClass;
			}
			else
			{
				PropertyEditorClass = x.PropertyEditorClass;
			}


			Bound_Renamed = x.Bound_Renamed | y.Bound_Renamed;
			Constrained_Renamed = x.Constrained_Renamed | y.Constrained_Renamed;
		}

		/*
		 * Package-private dup constructor.
		 * This must isolate the new object from any changes to the old object.
		 */
		internal PropertyDescriptor(PropertyDescriptor old) : base(old)
		{
			PropertyTypeRef = old.PropertyTypeRef;
			this.ReadMethodRef.Set(old.ReadMethodRef.Get());
			this.WriteMethodRef.Set(old.WriteMethodRef.Get());
			PropertyEditorClassRef = old.PropertyEditorClassRef;

			WriteMethodName = old.WriteMethodName;
			ReadMethodName = old.ReadMethodName;
			BaseName_Renamed = old.BaseName_Renamed;

			Bound_Renamed = old.Bound_Renamed;
			Constrained_Renamed = old.Constrained_Renamed;
		}

		internal virtual void UpdateGenericsFor(Class type)
		{
			Class0 = type;
			try
			{
				PropertyType = FindPropertyType(this.ReadMethodRef.Get(), this.WriteMethodRef.Get());
			}
			catch (IntrospectionException)
			{
				PropertyType = null;
			}
		}

		/// <summary>
		/// Returns the property type that corresponds to the read and write method.
		/// The type precedence is given to the readMethod.
		/// </summary>
		/// <returns> the type of the property descriptor or null if both
		///         read and write methods are null. </returns>
		/// <exception cref="IntrospectionException"> if the read or write method is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class findPropertyType(Method readMethod, Method writeMethod) throws IntrospectionException
		private Class FindPropertyType(Method readMethod, Method writeMethod)
		{
			Class propertyType = null;
			try
			{
				if (readMethod != null)
				{
					Class[] @params = GetParameterTypes(Class0, readMethod);
					if (@params.Length != 0)
					{
						throw new IntrospectionException("bad read method arg count: " + readMethod);
					}
					propertyType = GetReturnType(Class0, readMethod);
					if (propertyType == Void.TYPE)
					{
						throw new IntrospectionException("read method " + readMethod.Name + " returns void");
					}
				}
				if (writeMethod != null)
				{
					Class[] @params = GetParameterTypes(Class0, writeMethod);
					if (@params.Length != 1)
					{
						throw new IntrospectionException("bad write method arg count: " + writeMethod);
					}
					if (propertyType != null && !propertyType.IsSubclassOf(@params[0]))
					{
						throw new IntrospectionException("type mismatch between read and write methods");
					}
					propertyType = @params[0];
				}
			}
			catch (IntrospectionException ex)
			{
				throw ex;
			}
			return propertyType;
		}


		/// <summary>
		/// Returns a hash code value for the object.
		/// See <seealso cref="java.lang.Object#hashCode"/> for a complete description.
		/// </summary>
		/// <returns> a hash code value for this object.
		/// @since 1.5 </returns>
		public override int HashCode()
		{
			int result = 7;

			result = 37 * result + ((PropertyType == null) ? 0 : PropertyType.HashCode());
			result = 37 * result + ((ReadMethod == null) ? 0 : ReadMethod.HashCode());
			result = 37 * result + ((WriteMethod == null) ? 0 : WriteMethod.HashCode());
			result = 37 * result + ((PropertyEditorClass == null) ? 0 : PropertyEditorClass.HashCode());
			result = 37 * result + ((WriteMethodName == null) ? 0 : WriteMethodName.HashCode());
			result = 37 * result + ((ReadMethodName == null) ? 0 : ReadMethodName.HashCode());
			result = 37 * result + Name.HashCode();
			result = 37 * result + ((Bound_Renamed == false) ? 0 : 1);
			result = 37 * result + ((Constrained_Renamed == false) ? 0 : 1);

			return result;
		}

		// Calculate once since capitalize() is expensive.
		internal virtual String BaseName
		{
			get
			{
				if (BaseName_Renamed == null)
				{
					BaseName_Renamed = NameGenerator.Capitalize(Name);
				}
				return BaseName_Renamed;
			}
		}

		internal override void AppendTo(StringBuilder sb)
		{
			AppendTo(sb, "bound", this.Bound_Renamed);
			AppendTo(sb, "constrained", this.Constrained_Renamed);
			AppendTo(sb, "propertyEditorClass", this.PropertyEditorClassRef);
			AppendTo(sb, "propertyType", this.PropertyTypeRef);
			AppendTo(sb, "readMethod", this.ReadMethodRef.Get());
			AppendTo(sb, "writeMethod", this.WriteMethodRef.Get());
		}

		private bool IsAssignable(Method m1, Method m2)
		{
			if (m1 == null)
			{
				return true; // choose second method
			}
			if (m2 == null)
			{
				return false; // choose first method
			}
			if (!m1.Name.Equals(m2.Name))
			{
				return true; // choose second method by default
			}
			Class type1 = m1.DeclaringClass;
			Class type2 = m2.DeclaringClass;
			if (!type2.IsSubclassOf(type1))
			{
				return false; // choose first method: it declared later
			}
			type1 = GetReturnType(Class0, m1);
			type2 = GetReturnType(Class0, m2);
			if (!type2.IsSubclassOf(type1))
			{
				return false; // choose first method: it overrides return type
			}
			Class[] args1 = GetParameterTypes(Class0, m1);
			Class[] args2 = GetParameterTypes(Class0, m2);
			if (args1.Length != args2.Length)
			{
				return true; // choose second method by default
			}
			for (int i = 0; i < args1.Length; i++)
			{
				if (!args2[i].IsSubclassOf(args1[i]))
				{
					return false; // choose first method: it overrides parameter
				}
			}
			return true; // choose second method
		}
	}

}