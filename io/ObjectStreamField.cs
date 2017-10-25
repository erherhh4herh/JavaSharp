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

namespace java.io
{

	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// A description of a Serializable field from a Serializable class.  An array
	/// of ObjectStreamFields is used to declare the Serializable fields of a class.
	/// 
	/// @author      Mike Warres
	/// @author      Roger Riggs </summary>
	/// <seealso cref= ObjectStreamClass
	/// @since 1.2 </seealso>
	public class ObjectStreamField : Comparable<Object>
	{

		/// <summary>
		/// field name </summary>
		private readonly String Name_Renamed;
		/// <summary>
		/// canonical JVM signature of field type </summary>
		private readonly String Signature_Renamed;
		/// <summary>
		/// field type (Object.class if unknown non-primitive type) </summary>
		private readonly Class Type_Renamed;
		/// <summary>
		/// whether or not to (de)serialize field values as unshared </summary>
		private readonly bool Unshared_Renamed;
		/// <summary>
		/// corresponding reflective field object, if any </summary>
		private readonly Field Field_Renamed;
		/// <summary>
		/// offset of field value in enclosing field group </summary>
		private int Offset_Renamed = 0;

		/// <summary>
		/// Create a Serializable field with the specified type.  This field should
		/// be documented with a <code>serialField</code> tag.
		/// </summary>
		/// <param name="name"> the name of the serializable field </param>
		/// <param name="type"> the <code>Class</code> object of the serializable field </param>
		public ObjectStreamField(String name, Class type) : this(name, type, false)
		{
		}

		/// <summary>
		/// Creates an ObjectStreamField representing a serializable field with the
		/// given name and type.  If unshared is false, values of the represented
		/// field are serialized and deserialized in the default manner--if the
		/// field is non-primitive, object values are serialized and deserialized as
		/// if they had been written and read by calls to writeObject and
		/// readObject.  If unshared is true, values of the represented field are
		/// serialized and deserialized as if they had been written and read by
		/// calls to writeUnshared and readUnshared.
		/// </summary>
		/// <param name="name"> field name </param>
		/// <param name="type"> field type </param>
		/// <param name="unshared"> if false, write/read field values in the same manner
		///          as writeObject/readObject; if true, write/read in the same
		///          manner as writeUnshared/readUnshared
		/// @since   1.4 </param>
		public ObjectStreamField(String name, Class type, bool unshared)
		{
			if (name == null)
			{
				throw new NullPointerException();
			}
			this.Name_Renamed = name;
			this.Type_Renamed = type;
			this.Unshared_Renamed = unshared;
			Signature_Renamed = GetClassSignature(type).intern();
			Field_Renamed = null;
		}

		/// <summary>
		/// Creates an ObjectStreamField representing a field with the given name,
		/// signature and unshared setting.
		/// </summary>
		internal ObjectStreamField(String name, String signature, bool unshared)
		{
			if (name == null)
			{
				throw new NullPointerException();
			}
			this.Name_Renamed = name;
			this.Signature_Renamed = signature.intern();
			this.Unshared_Renamed = unshared;
			Field_Renamed = null;

			switch (signature.CharAt(0))
			{
				case 'Z':
					Type_Renamed = Boolean.TYPE;
					break;
				case 'B':
					Type_Renamed = Byte.TYPE;
					break;
				case 'C':
					Type_Renamed = Character.TYPE;
					break;
				case 'S':
					Type_Renamed = Short.TYPE;
					break;
				case 'I':
					Type_Renamed = Integer.TYPE;
					break;
				case 'J':
					Type_Renamed = Long.TYPE;
					break;
				case 'F':
					Type_Renamed = Float.TYPE;
					break;
				case 'D':
					Type_Renamed = Double.TYPE;
					break;
				case 'L':
				case '[':
					Type_Renamed = typeof(Object);
					break;
				default:
					throw new IllegalArgumentException("illegal signature");
			}
		}

		/// <summary>
		/// Creates an ObjectStreamField representing the given field with the
		/// specified unshared setting.  For compatibility with the behavior of
		/// earlier serialization implementations, a "showType" parameter is
		/// necessary to govern whether or not a getType() call on this
		/// ObjectStreamField (if non-primitive) will return Object.class (as
		/// opposed to a more specific reference type).
		/// </summary>
		internal ObjectStreamField(Field field, bool unshared, bool showType)
		{
			this.Field_Renamed = field;
			this.Unshared_Renamed = unshared;
			Name_Renamed = field.Name;
			Class ftype = field.Type;
			Type_Renamed = (showType || ftype.Primitive) ? ftype : typeof(Object);
			Signature_Renamed = GetClassSignature(ftype).intern();
		}

		/// <summary>
		/// Get the name of this field.
		/// </summary>
		/// <returns>  a <code>String</code> representing the name of the serializable
		///          field </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Get the type of the field.  If the type is non-primitive and this
		/// <code>ObjectStreamField</code> was obtained from a deserialized {@link
		/// ObjectStreamClass} instance, then <code>Object.class</code> is returned.
		/// Otherwise, the <code>Class</code> object for the type of the field is
		/// returned.
		/// </summary>
		/// <returns>  a <code>Class</code> object representing the type of the
		///          serializable field </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public Class getType()
		public virtual Class Type
		{
			get
			{
				if (System.SecurityManager != null)
				{
					Class caller = Reflection.CallerClass;
					if (ReflectUtil.needsPackageAccessCheck(caller.ClassLoader, Type_Renamed.ClassLoader))
					{
						ReflectUtil.checkPackageAccess(Type_Renamed);
					}
				}
				return Type_Renamed;
			}
		}

		/// <summary>
		/// Returns character encoding of field type.  The encoding is as follows:
		/// <blockquote><pre>
		/// B            byte
		/// C            char
		/// D            double
		/// F            float
		/// I            int
		/// J            long
		/// L            class or interface
		/// S            short
		/// Z            boolean
		/// [            array
		/// </pre></blockquote>
		/// </summary>
		/// <returns>  the typecode of the serializable field </returns>
		// REMIND: deprecate?
		public virtual char TypeCode
		{
			get
			{
				return Signature_Renamed.CharAt(0);
			}
		}

		/// <summary>
		/// Return the JVM type signature.
		/// </summary>
		/// <returns>  null if this field has a primitive type. </returns>
		// REMIND: deprecate?
		public virtual String TypeString
		{
			get
			{
				return Primitive ? null : Signature_Renamed;
			}
		}

		/// <summary>
		/// Offset of field within instance data.
		/// </summary>
		/// <returns>  the offset of this field </returns>
		/// <seealso cref= #setOffset </seealso>
		// REMIND: deprecate?
		public virtual int Offset
		{
			get
			{
				return Offset_Renamed;
			}
			set
			{
				this.Offset_Renamed = value;
			}
		}


		/// <summary>
		/// Return true if this field has a primitive type.
		/// </summary>
		/// <returns>  true if and only if this field corresponds to a primitive type </returns>
		// REMIND: deprecate?
		public virtual bool Primitive
		{
			get
			{
				char tcode = Signature_Renamed.CharAt(0);
				return ((tcode != 'L') && (tcode != '['));
			}
		}

		/// <summary>
		/// Returns boolean value indicating whether or not the serializable field
		/// represented by this ObjectStreamField instance is unshared.
		/// </summary>
		/// <returns> {@code true} if this field is unshared
		/// 
		/// @since 1.4 </returns>
		public virtual bool Unshared
		{
			get
			{
				return Unshared_Renamed;
			}
		}

		/// <summary>
		/// Compare this field with another <code>ObjectStreamField</code>.  Return
		/// -1 if this is smaller, 0 if equal, 1 if greater.  Types that are
		/// primitives are "smaller" than object types.  If equal, the field names
		/// are compared.
		/// </summary>
		// REMIND: deprecate?
		public virtual int CompareTo(Object obj)
		{
			ObjectStreamField other = (ObjectStreamField) obj;
			bool isPrim = Primitive;
			if (isPrim != other.Primitive)
			{
				return isPrim ? - 1 : 1;
			}
			return Name_Renamed.CompareTo(other.Name_Renamed);
		}

		/// <summary>
		/// Return a string that describes this field.
		/// </summary>
		public override String ToString()
		{
			return Signature_Renamed + ' ' + Name_Renamed;
		}

		/// <summary>
		/// Returns field represented by this ObjectStreamField, or null if
		/// ObjectStreamField is not associated with an actual field.
		/// </summary>
		internal virtual Field Field
		{
			get
			{
				return Field_Renamed;
			}
		}

		/// <summary>
		/// Returns JVM type signature of field (similar to getTypeString, except
		/// that signature strings are returned for primitive fields as well).
		/// </summary>
		internal virtual String Signature
		{
			get
			{
				return Signature_Renamed;
			}
		}

		/// <summary>
		/// Returns JVM type signature for given class.
		/// </summary>
		private static String GetClassSignature(Class cl)
		{
			StringBuilder sbuf = new StringBuilder();
			while (cl.Array)
			{
				sbuf.Append('[');
				cl = cl.ComponentType;
			}
			if (cl.Primitive)
			{
				if (cl == Integer.TYPE)
				{
					sbuf.Append('I');
				}
				else if (cl == Byte.TYPE)
				{
					sbuf.Append('B');
				}
				else if (cl == Long.TYPE)
				{
					sbuf.Append('J');
				}
				else if (cl == Float.TYPE)
				{
					sbuf.Append('F');
				}
				else if (cl == Double.TYPE)
				{
					sbuf.Append('D');
				}
				else if (cl == Short.TYPE)
				{
					sbuf.Append('S');
				}
				else if (cl == Character.TYPE)
				{
					sbuf.Append('C');
				}
				else if (cl == Boolean.TYPE)
				{
					sbuf.Append('Z');
				}
				else if (cl == Void.TYPE)
				{
					sbuf.Append('V');
				}
				else
				{
					throw new InternalError();
				}
			}
			else
			{
				sbuf.Append('L' + cl.Name.Replace('.', '/') + ';');
			}
			return sbuf.ToString();
		}
	}

}