/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.annotation
{

	/// <summary>
	/// Thrown to indicate that a program has attempted to access an element of
	/// an annotation type that was added to the annotation type definition after
	/// the annotation was compiled (or serialized).  This exception will not be
	/// thrown if the new element has a default value.
	/// This exception can be thrown by the {@linkplain
	/// java.lang.reflect.AnnotatedElement API used to read annotations
	/// reflectively}.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     java.lang.reflect.AnnotatedElement
	/// @since 1.5 </seealso>
	public class IncompleteAnnotationException : RuntimeException
	{
		private new const long SerialVersionUID = 8445097402741811912L;

		private Class AnnotationType_Renamed;
		private String ElementName_Renamed;

		/// <summary>
		/// Constructs an IncompleteAnnotationException to indicate that
		/// the named element was missing from the specified annotation type.
		/// </summary>
		/// <param name="annotationType"> the Class object for the annotation type </param>
		/// <param name="elementName"> the name of the missing element </param>
		/// <exception cref="NullPointerException"> if either parameter is {@code null} </exception>
		public IncompleteAnnotationException(Class annotationType, String elementName) : base(annotationType.Name + " missing element " + elementName.ToString())
		{

			this.AnnotationType_Renamed = annotationType;
			this.ElementName_Renamed = elementName;
		}

		/// <summary>
		/// Returns the Class object for the annotation type with the
		/// missing element.
		/// </summary>
		/// <returns> the Class object for the annotation type with the
		///     missing element </returns>
		public virtual Class AnnotationType()
		{
			return AnnotationType_Renamed;
		}

		/// <summary>
		/// Returns the name of the missing element.
		/// </summary>
		/// <returns> the name of the missing element </returns>
		public virtual String ElementName()
		{
			return ElementName_Renamed;
		}
	}

}