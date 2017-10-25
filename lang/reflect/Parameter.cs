using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

	using AnnotationSupport = sun.reflect.annotation.AnnotationSupport;

	/// <summary>
	/// Information about method parameters.
	/// 
	/// A {@code Parameter} provides information about method parameters,
	/// including its name and modifiers.  It also provides an alternate
	/// means of obtaining attributes for the parameter.
	/// 
	/// @since 1.8
	/// </summary>
	public sealed class Parameter : AnnotatedElement
	{

		private readonly String Name_Renamed;
		private readonly int Modifiers_Renamed;
		private readonly Executable Executable;
		private readonly int Index;

		/// <summary>
		/// Package-private constructor for {@code Parameter}.
		/// 
		/// If method parameter data is present in the classfile, then the
		/// JVM creates {@code Parameter} objects directly.  If it is
		/// absent, however, then {@code Executable} uses this constructor
		/// to synthesize them.
		/// </summary>
		/// <param name="name"> The name of the parameter. </param>
		/// <param name="modifiers"> The modifier flags for the parameter. </param>
		/// <param name="executable"> The executable which defines this parameter. </param>
		/// <param name="index"> The index of the parameter. </param>
		internal Parameter(String name, int modifiers, Executable executable, int index)
		{
			this.Name_Renamed = name;
			this.Modifiers_Renamed = modifiers;
			this.Executable = executable;
			this.Index = index;
		}

		/// <summary>
		/// Compares based on the executable and the index.
		/// </summary>
		/// <param name="obj"> The object to compare. </param>
		/// <returns> Whether or not this is equal to the argument. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Parameter)
			{
				Parameter other = (Parameter)obj;
				return (other.Executable.Equals(Executable) && other.Index == Index);
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code based on the executable's hash code and the
		/// index.
		/// </summary>
		/// <returns> A hash code based on the executable's hash code. </returns>
		public override int HashCode()
		{
			return Executable.HashCode() ^ Index;
		}

		/// <summary>
		/// Returns true if the parameter has a name according to the class
		/// file; returns false otherwise. Whether a parameter has a name
		/// is determined by the {@literal MethodParameters} attribute of
		/// the method which declares the parameter.
		/// </summary>
		/// <returns> true if and only if the parameter has a name according
		/// to the class file. </returns>
		public bool NamePresent
		{
			get
			{
				return Executable.HasRealParameterData() && Name_Renamed != AnnotatedElement_Fields.Null;
			}
		}

		/// <summary>
		/// Returns a string describing this parameter.  The format is the
		/// modifiers for the parameter, if any, in canonical order as
		/// recommended by <cite>The Java&trade; Language
		/// Specification</cite>, followed by the fully- qualified type of
		/// the parameter (excluding the last [] if the parameter is
		/// variable arity), followed by "..." if the parameter is variable
		/// arity, followed by a space, followed by the name of the
		/// parameter.
		/// </summary>
		/// <returns> A string representation of the parameter and associated
		/// information. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Type type = getParameterizedType();
			Type type = ParameterizedType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String typename = type.getTypeName();
			String typename = type.TypeName;

			sb.Append(Modifier.ToString(Modifiers));

			if (0 != Modifiers_Renamed)
			{
				sb.Append(' ');
			}

			if (VarArgs)
			{
				sb.Append(typename.ReplaceFirst("\\[\\]$", "..."));
			}
			else
			{
				sb.Append(typename);
			}

			sb.Append(' ');
			sb.Append(Name);

			return sb.ToString();
		}

		/// <summary>
		/// Return the {@code Executable} which declares this parameter.
		/// </summary>
		/// <returns> The {@code Executable} declaring this parameter. </returns>
		public Executable DeclaringExecutable
		{
			get
			{
				return Executable;
			}
		}

		/// <summary>
		/// Get the modifier flags for this the parameter represented by
		/// this {@code Parameter} object.
		/// </summary>
		/// <returns> The modifier flags for this parameter. </returns>
		public int Modifiers
		{
			get
			{
				return Modifiers_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the parameter.  If the parameter's name is
		/// <seealso cref="#isNamePresent() present"/>, then this method returns
		/// the name provided by the class file. Otherwise, this method
		/// synthesizes a name of the form argN, where N is the index of
		/// the parameter in the descriptor of the method which declares
		/// the parameter.
		/// </summary>
		/// <returns> The name of the parameter, either provided by the class
		///         file or synthesized if the class file does not provide
		///         a name. </returns>
		public String Name
		{
			get
			{
				// Note: empty strings as paramete names are now outlawed.
				// The .equals("") is for compatibility with current JVM
				// behavior.  It may be removed at some point.
				if (Name_Renamed == AnnotatedElement_Fields.Null || Name_Renamed.Equals(""))
				{
					return "arg" + Index;
				}
				else
				{
					return Name_Renamed;
				}
			}
		}

		// Package-private accessor to the real name field.
		internal String RealName
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns a {@code Type} object that identifies the parameterized
		/// type for the parameter represented by this {@code Parameter}
		/// object.
		/// </summary>
		/// <returns> a {@code Type} object identifying the parameterized
		/// type of the parameter represented by this object </returns>
		public Type ParameterizedType
		{
			get
			{
				Type tmp = ParameterTypeCache;
				if (AnnotatedElement_Fields.Null == tmp)
				{
					tmp = Executable.AllGenericParameterTypes[Index];
					ParameterTypeCache = tmp;
				}
    
				return tmp;
			}
		}

		[NonSerialized]
		private volatile Type ParameterTypeCache = AnnotatedElement_Fields.Null;

		/// <summary>
		/// Returns a {@code Class} object that identifies the
		/// declared type for the parameter represented by this
		/// {@code Parameter} object.
		/// </summary>
		/// <returns> a {@code Class} object identifying the declared
		/// type of the parameter represented by this object </returns>
		public Class Type
		{
			get
			{
				Class tmp = ParameterClassCache;
				if (AnnotatedElement_Fields.Null == tmp)
				{
					tmp = Executable.ParameterTypes[Index];
					ParameterClassCache = tmp;
				}
				return tmp;
			}
		}

		/// <summary>
		/// Returns an AnnotatedType object that represents the use of a type to
		/// specify the type of the formal parameter represented by this Parameter.
		/// </summary>
		/// <returns> an {@code AnnotatedType} object representing the use of a type
		///         to specify the type of the formal parameter represented by this
		///         Parameter </returns>
		public AnnotatedType AnnotatedType
		{
			get
			{
				// no caching for now
				return Executable.AnnotatedParameterTypes[Index];
			}
		}

		[NonSerialized]
		private volatile Class ParameterClassCache = AnnotatedElement_Fields.Null;

		/// <summary>
		/// Returns {@code true} if this parameter is implicitly declared
		/// in source code; returns {@code false} otherwise.
		/// </summary>
		/// <returns> true if and only if this parameter is implicitly
		/// declared as defined by <cite>The Java&trade; Language
		/// Specification</cite>. </returns>
		public bool Implicit
		{
			get
			{
				return Modifier.IsMandated(Modifiers);
			}
		}

		/// <summary>
		/// Returns {@code true} if this parameter is neither implicitly
		/// nor explicitly declared in source code; returns {@code false}
		/// otherwise.
		/// 
		/// @jls 13.1 The Form of a Binary </summary>
		/// <returns> true if and only if this parameter is a synthetic
		/// construct as defined by
		/// <cite>The Java&trade; Language Specification</cite>. </returns>
		public bool Synthetic
		{
			get
			{
				return Modifier.IsSynthetic(Modifiers);
			}
		}

		/// <summary>
		/// Returns {@code true} if this parameter represents a variable
		/// argument list; returns {@code false} otherwise.
		/// </summary>
		/// <returns> {@code true} if an only if this parameter represents a
		/// variable argument list. </returns>
		public bool VarArgs
		{
			get
			{
				return Executable.VarArgs && Index == Executable.ParameterCount - 1;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public T getAnnotation<T>(Class annotationClass) where T : Annotation
		{
			Objects.RequireNonNull(annotationClass);
			return annotationClass.Cast(DeclaredAnnotations()[annotationClass]);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public override T[] getAnnotationsByType<T>(Class annotationClass) where T : Annotation
		{
			Objects.RequireNonNull(annotationClass);

			return AnnotationSupport.getDirectlyAndIndirectlyPresent(DeclaredAnnotations(), annotationClass);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public Annotation[] DeclaredAnnotations
		{
			get
			{
				return Executable.ParameterAnnotations[Index];
			}
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public T getDeclaredAnnotation<T>(Class annotationClass) where T : Annotation
		{
			// Only annotations on classes are inherited, for all other
			// objects getDeclaredAnnotation is the same as
			// getAnnotation.
			return GetAnnotation(annotationClass);
		}

		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public override T[] getDeclaredAnnotationsByType<T>(Class annotationClass) where T : Annotation
		{
			// Only annotations on classes are inherited, for all other
			// objects getDeclaredAnnotations is the same as
			// getAnnotations.
			return GetAnnotationsByType(annotationClass);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public Annotation[] Annotations
		{
			get
			{
				return DeclaredAnnotations;
			}
		}

		[NonSerialized]
		private IDictionary<Class, Annotation> DeclaredAnnotations_Renamed;

		private IDictionary<Class, Annotation> DeclaredAnnotations()
		{
			lock (this)
			{
				if (AnnotatedElement_Fields.Null == DeclaredAnnotations_Renamed)
				{
					DeclaredAnnotations_Renamed = new Dictionary<Class, Annotation>();
					Annotation[] ann = DeclaredAnnotations;
					for (int i = 0; i < ann.Length; i++)
					{
						DeclaredAnnotations_Renamed[ann[i].AnnotationType()] = ann[i];
					}
				}
				return DeclaredAnnotations_Renamed;
			}
		}

	}

}