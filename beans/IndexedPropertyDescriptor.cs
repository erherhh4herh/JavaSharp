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

namespace java.beans
{


	/// <summary>
	/// An IndexedPropertyDescriptor describes a property that acts like an
	/// array and has an indexed read and/or indexed write method to access
	/// specific elements of the array.
	/// <para>
	/// An indexed property may also provide simple non-indexed read and write
	/// methods.  If these are present, they read and write arrays of the type
	/// returned by the indexed read method.
	/// </para>
	/// </summary>

	public class IndexedPropertyDescriptor : PropertyDescriptor
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> indexedPropertyTypeRef;
		private Reference<?> IndexedPropertyTypeRef;
		private readonly MethodRef IndexedReadMethodRef = new MethodRef();
		private readonly MethodRef IndexedWriteMethodRef = new MethodRef();

		private String IndexedReadMethodName;
		private String IndexedWriteMethodName;

		/// <summary>
		/// This constructor constructs an IndexedPropertyDescriptor for a property
		/// that follows the standard Java conventions by having getFoo and setFoo
		/// accessor methods, for both indexed access and array access.
		/// <para>
		/// Thus if the argument name is "fred", it will assume that there
		/// is an indexed reader method "getFred", a non-indexed (array) reader
		/// method also called "getFred", an indexed writer method "setFred",
		/// and finally a non-indexed writer method "setFred".
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="beanClass"> The Class object for the target bean. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IndexedPropertyDescriptor(String propertyName, Class beanClass) throws IntrospectionException
		public IndexedPropertyDescriptor(String propertyName, Class beanClass) : this(propertyName, beanClass, Introspector.GET_PREFIX + NameGenerator.Capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.Capitalize(propertyName), Introspector.GET_PREFIX + NameGenerator.Capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.Capitalize(propertyName))
		{
		}

		/// <summary>
		/// This constructor takes the name of a simple property, and method
		/// names for reading and writing the property, both indexed
		/// and non-indexed.
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="beanClass">  The Class object for the target bean. </param>
		/// <param name="readMethodName"> The name of the method used for reading the property
		///           values as an array.  May be null if the property is write-only
		///           or must be indexed. </param>
		/// <param name="writeMethodName"> The name of the method used for writing the property
		///           values as an array.  May be null if the property is read-only
		///           or must be indexed. </param>
		/// <param name="indexedReadMethodName"> The name of the method used for reading
		///          an indexed property value.
		///          May be null if the property is write-only. </param>
		/// <param name="indexedWriteMethodName"> The name of the method used for writing
		///          an indexed property value.
		///          May be null if the property is read-only. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IndexedPropertyDescriptor(String propertyName, Class beanClass, String readMethodName, String writeMethodName, String indexedReadMethodName, String indexedWriteMethodName) throws IntrospectionException
		public IndexedPropertyDescriptor(String propertyName, Class beanClass, String readMethodName, String writeMethodName, String indexedReadMethodName, String indexedWriteMethodName) : base(propertyName, beanClass, readMethodName, writeMethodName)
		{

			this.IndexedReadMethodName = indexedReadMethodName;
			if (indexedReadMethodName != null && IndexedReadMethod == null)
			{
				throw new IntrospectionException("Method not found: " + indexedReadMethodName);
			}

			this.IndexedWriteMethodName = indexedWriteMethodName;
			if (indexedWriteMethodName != null && IndexedWriteMethod == null)
			{
				throw new IntrospectionException("Method not found: " + indexedWriteMethodName);
			}
			// Implemented only for type checking.
			FindIndexedPropertyType(IndexedReadMethod, IndexedWriteMethod);
		}

		/// <summary>
		/// This constructor takes the name of a simple property, and Method
		/// objects for reading and writing the property.
		/// </summary>
		/// <param name="propertyName"> The programmatic name of the property. </param>
		/// <param name="readMethod"> The method used for reading the property values as an array.
		///          May be null if the property is write-only or must be indexed. </param>
		/// <param name="writeMethod"> The method used for writing the property values as an array.
		///          May be null if the property is read-only or must be indexed. </param>
		/// <param name="indexedReadMethod"> The method used for reading an indexed property value.
		///          May be null if the property is write-only. </param>
		/// <param name="indexedWriteMethod"> The method used for writing an indexed property value.
		///          May be null if the property is read-only. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IndexedPropertyDescriptor(String propertyName, Method readMethod, Method writeMethod, Method indexedReadMethod, Method indexedWriteMethod) throws IntrospectionException
		public IndexedPropertyDescriptor(String propertyName, Method readMethod, Method writeMethod, Method indexedReadMethod, Method indexedWriteMethod) : base(propertyName, readMethod, writeMethod)
		{

			IndexedReadMethod0 = indexedReadMethod;
			IndexedWriteMethod0 = indexedWriteMethod;

			// Type checking
			IndexedPropertyType = FindIndexedPropertyType(indexedReadMethod, indexedWriteMethod);
		}

		/// <summary>
		/// Creates <code>PropertyDescriptor</code> for the specified bean
		/// with the specified name and methods to read/write the property value.
		/// </summary>
		/// <param name="bean">          the type of the target bean </param>
		/// <param name="base">          the base name of the property (the rest of the method name) </param>
		/// <param name="read">          the method used for reading the property value </param>
		/// <param name="write">         the method used for writing the property value </param>
		/// <param name="readIndexed">   the method used for reading an indexed property value </param>
		/// <param name="writeIndexed">  the method used for writing an indexed property value </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during introspection
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: IndexedPropertyDescriptor(Class bean, String base, Method read, Method write, Method readIndexed, Method writeIndexed) throws IntrospectionException
		internal IndexedPropertyDescriptor(Class bean, String @base, Method read, Method write, Method readIndexed, Method writeIndexed) : base(bean, @base, read, write)
		{

			IndexedReadMethod0 = readIndexed;
			IndexedWriteMethod0 = writeIndexed;

			// Type checking
			IndexedPropertyType = FindIndexedPropertyType(readIndexed, writeIndexed);
		}

		/// <summary>
		/// Gets the method that should be used to read an indexed
		/// property value.
		/// </summary>
		/// <returns> The method that should be used to read an indexed
		/// property value.
		/// May return null if the property isn't indexed or is write-only. </returns>
		public virtual Method IndexedReadMethod
		{
			get
			{
				lock (this)
				{
					Method indexedReadMethod = this.IndexedReadMethodRef.Get();
					if (indexedReadMethod == null)
					{
						Class cls = Class0;
						if (cls == null || (IndexedReadMethodName == null && !this.IndexedReadMethodRef.Set))
						{
							// the Indexed readMethod was explicitly set to null.
							return null;
						}
						String nextMethodName = Introspector.GET_PREFIX + BaseName;
						if (IndexedReadMethodName == null)
						{
							Class type = IndexedPropertyType0;
							if (type == typeof(bool) || type == null)
							{
								IndexedReadMethodName = Introspector.IS_PREFIX + BaseName;
							}
							else
							{
								IndexedReadMethodName = nextMethodName;
							}
						}
            
						Class[] args = new Class[] {typeof(int)};
						indexedReadMethod = Introspector.FindMethod(cls, IndexedReadMethodName, 1, args);
						if ((indexedReadMethod == null) && !IndexedReadMethodName.Equals(nextMethodName))
						{
							// no "is" method, so look for a "get" method.
							IndexedReadMethodName = nextMethodName;
							indexedReadMethod = Introspector.FindMethod(cls, IndexedReadMethodName, 1, args);
						}
						IndexedReadMethod0 = indexedReadMethod;
					}
					return indexedReadMethod;
				}
			}
			set
			{
				lock (this)
				{
            
					// the indexed property type is set by the reader.
					IndexedPropertyType = FindIndexedPropertyType(value, this.IndexedWriteMethodRef.Get());
					IndexedReadMethod0 = value;
				}
			}
		}


		private Method IndexedReadMethod0
		{
			set
			{
				this.IndexedReadMethodRef.Set(value);
				if (value == null)
				{
					IndexedReadMethodName = null;
					return;
				}
				Class0 = value.DeclaringClass;
    
				IndexedReadMethodName = value.Name;
				SetTransient(value.getAnnotation(typeof(Transient)));
			}
		}


		/// <summary>
		/// Gets the method that should be used to write an indexed property value.
		/// </summary>
		/// <returns> The method that should be used to write an indexed
		/// property value.
		/// May return null if the property isn't indexed or is read-only. </returns>
		public virtual Method IndexedWriteMethod
		{
			get
			{
				lock (this)
				{
					Method indexedWriteMethod = this.IndexedWriteMethodRef.Get();
					if (indexedWriteMethod == null)
					{
						Class cls = Class0;
						if (cls == null || (IndexedWriteMethodName == null && !this.IndexedWriteMethodRef.Set))
						{
							// the Indexed writeMethod was explicitly set to null.
							return null;
						}
            
						// We need the indexed type to ensure that we get the correct method.
						// Cannot use the getIndexedPropertyType method since that could
						// result in an infinite loop.
						Class type = IndexedPropertyType0;
						if (type == null)
						{
							try
							{
								type = FindIndexedPropertyType(IndexedReadMethod, null);
								IndexedPropertyType = type;
							}
							catch (IntrospectionException)
							{
								// Set iprop type to be the classic type
								Class propType = PropertyType;
								if (propType.Array)
								{
									type = propType.ComponentType;
								}
							}
						}
            
						if (IndexedWriteMethodName == null)
						{
							IndexedWriteMethodName = Introspector.SET_PREFIX + BaseName;
						}
            
						Class[] args = (type == null) ? null : new Class[] {typeof(int), type};
						indexedWriteMethod = Introspector.FindMethod(cls, IndexedWriteMethodName, 2, args);
						if (indexedWriteMethod != null)
						{
							if (!indexedWriteMethod.ReturnType.Equals(typeof(void)))
							{
								indexedWriteMethod = null;
							}
						}
						IndexedWriteMethod0 = indexedWriteMethod;
					}
					return indexedWriteMethod;
				}
			}
			set
			{
				lock (this)
				{
            
					// If the indexed property type has not been set, then set it.
					Class type = FindIndexedPropertyType(IndexedReadMethod, value);
					IndexedPropertyType = type;
					IndexedWriteMethod0 = value;
				}
			}
		}


		private Method IndexedWriteMethod0
		{
			set
			{
				this.IndexedWriteMethodRef.Set(value);
				if (value == null)
				{
					IndexedWriteMethodName = null;
					return;
				}
				Class0 = value.DeclaringClass;
    
				IndexedWriteMethodName = value.Name;
				SetTransient(value.getAnnotation(typeof(Transient)));
			}
		}

		/// <summary>
		/// Returns the Java type info for the indexed property.
		/// Note that the {@code Class} object may describe
		/// primitive Java types such as {@code int}.
		/// This type is returned by the indexed read method
		/// or is used as the parameter type of the indexed write method.
		/// </summary>
		/// <returns> the {@code Class} object that represents the Java type info,
		///         or {@code null} if the type cannot be determined </returns>
		public virtual Class IndexedPropertyType
		{
			get
			{
				lock (this)
				{
					Class type = IndexedPropertyType0;
					if (type == null)
					{
						try
						{
							type = FindIndexedPropertyType(IndexedReadMethod, IndexedWriteMethod);
							IndexedPropertyType = type;
						}
						catch (IntrospectionException)
						{
							// fall
						}
					}
					return type;
				}
			}
			set
			{
				this.IndexedPropertyTypeRef = GetWeakReference(value);
			}
		}

		// Private methods which set get/set the Reference objects


		private Class IndexedPropertyType0
		{
			get
			{
				return (this.IndexedPropertyTypeRef != null) ? this.IndexedPropertyTypeRef.get() : null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Class findIndexedPropertyType(Method indexedReadMethod, Method indexedWriteMethod) throws IntrospectionException
		private Class FindIndexedPropertyType(Method indexedReadMethod, Method indexedWriteMethod)
		{
			Class indexedPropertyType = null;

			if (indexedReadMethod != null)
			{
				Class[] @params = GetParameterTypes(Class0, indexedReadMethod);
				if (@params.Length != 1)
				{
					throw new IntrospectionException("bad indexed read method arg count");
				}
				if (@params[0] != Integer.TYPE)
				{
					throw new IntrospectionException("non int index to indexed read method");
				}
				indexedPropertyType = GetReturnType(Class0, indexedReadMethod);
				if (indexedPropertyType == Void.TYPE)
				{
					throw new IntrospectionException("indexed read method returns void");
				}
			}
			if (indexedWriteMethod != null)
			{
				Class[] @params = GetParameterTypes(Class0, indexedWriteMethod);
				if (@params.Length != 2)
				{
					throw new IntrospectionException("bad indexed write method arg count");
				}
				if (@params[0] != Integer.TYPE)
				{
					throw new IntrospectionException("non int index to indexed write method");
				}
				if (indexedPropertyType == null || indexedPropertyType.IsSubclassOf(@params[1]))
				{
					indexedPropertyType = @params[1];
				}
				else if (!@params[1].IsSubclassOf(indexedPropertyType))
				{
					throw new IntrospectionException("type mismatch between indexed read and indexed write methods: " + Name);
				}
			}
			Class propertyType = PropertyType;
			if (propertyType != null && (!propertyType.Array || propertyType.ComponentType != indexedPropertyType))
			{
				throw new IntrospectionException("type mismatch between indexed and non-indexed methods: " + Name);
			}
			return indexedPropertyType;
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
			// Note: This would be identical to PropertyDescriptor but they don't
			// share the same fields.
			if (this == obj)
			{
				return true;
			}

			if (obj != null && obj is IndexedPropertyDescriptor)
			{
				IndexedPropertyDescriptor other = (IndexedPropertyDescriptor)obj;
				Method otherIndexedReadMethod = other.IndexedReadMethod;
				Method otherIndexedWriteMethod = other.IndexedWriteMethod;

				if (!CompareMethods(IndexedReadMethod, otherIndexedReadMethod))
				{
					return false;
				}

				if (!CompareMethods(IndexedWriteMethod, otherIndexedWriteMethod))
				{
					return false;
				}

				if (IndexedPropertyType != other.IndexedPropertyType)
				{
					return false;
				}
				return base.Equals(obj);
			}
			return false;
		}

		/// <summary>
		/// Package-private constructor.
		/// Merge two property descriptors.  Where they conflict, give the
		/// second argument (y) priority over the first argumnnt (x).
		/// </summary>
		/// <param name="x">  The first (lower priority) PropertyDescriptor </param>
		/// <param name="y">  The second (higher priority) PropertyDescriptor </param>

		internal IndexedPropertyDescriptor(PropertyDescriptor x, PropertyDescriptor y) : base(x,y)
		{
			if (x is IndexedPropertyDescriptor)
			{
				IndexedPropertyDescriptor ix = (IndexedPropertyDescriptor)x;
				try
				{
					Method xr = ix.IndexedReadMethod;
					if (xr != null)
					{
						IndexedReadMethod = xr;
					}

					Method xw = ix.IndexedWriteMethod;
					if (xw != null)
					{
						IndexedWriteMethod = xw;
					}
				}
				catch (IntrospectionException ex)
				{
					// Should not happen
					throw new AssertionError(ex);
				}
			}
			if (y is IndexedPropertyDescriptor)
			{
				IndexedPropertyDescriptor iy = (IndexedPropertyDescriptor)y;
				try
				{
					Method yr = iy.IndexedReadMethod;
					if (yr != null && yr.DeclaringClass == Class0)
					{
						IndexedReadMethod = yr;
					}

					Method yw = iy.IndexedWriteMethod;
					if (yw != null && yw.DeclaringClass == Class0)
					{
						IndexedWriteMethod = yw;
					}
				}
				catch (IntrospectionException ex)
				{
					// Should not happen
					throw new AssertionError(ex);
				}
			}
		}

		/*
		 * Package-private dup constructor
		 * This must isolate the new object from any changes to the old object.
		 */
		internal IndexedPropertyDescriptor(IndexedPropertyDescriptor old) : base(old)
		{
			this.IndexedReadMethodRef.Set(old.IndexedReadMethodRef.Get());
			this.IndexedWriteMethodRef.Set(old.IndexedWriteMethodRef.Get());
			IndexedPropertyTypeRef = old.IndexedPropertyTypeRef;
			IndexedWriteMethodName = old.IndexedWriteMethodName;
			IndexedReadMethodName = old.IndexedReadMethodName;
		}

		internal override void UpdateGenericsFor(Class type)
		{
			base.UpdateGenericsFor(type);
			try
			{
				IndexedPropertyType = FindIndexedPropertyType(this.IndexedReadMethodRef.Get(), this.IndexedWriteMethodRef.Get());
			}
			catch (IntrospectionException)
			{
				IndexedPropertyType = null;
			}
		}

		/// <summary>
		/// Returns a hash code value for the object.
		/// See <seealso cref="java.lang.Object#hashCode"/> for a complete description.
		/// </summary>
		/// <returns> a hash code value for this object.
		/// @since 1.5 </returns>
		public override int HashCode()
		{
			int result = base.HashCode();

			result = 37 * result + ((IndexedWriteMethodName == null) ? 0 : IndexedWriteMethodName.HashCode());
			result = 37 * result + ((IndexedReadMethodName == null) ? 0 : IndexedReadMethodName.HashCode());
			result = 37 * result + ((IndexedPropertyType == null) ? 0 : IndexedPropertyType.HashCode());

			return result;
		}

		internal override void AppendTo(StringBuilder sb)
		{
			base.AppendTo(sb);
			AppendTo(sb, "indexedPropertyType", this.IndexedPropertyTypeRef);
			AppendTo(sb, "indexedReadMethod", this.IndexedReadMethodRef.Get());
			AppendTo(sb, "indexedWriteMethod", this.IndexedWriteMethodRef.Get());
		}
	}

}