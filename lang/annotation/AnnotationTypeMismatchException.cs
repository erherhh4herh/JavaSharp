/*
 * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// an annotation whose type has changed after the annotation was compiled
	/// (or serialized).
	/// This exception can be thrown by the {@linkplain
	/// java.lang.reflect.AnnotatedElement API used to read annotations
	/// reflectively}.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     java.lang.reflect.AnnotatedElement
	/// @since 1.5 </seealso>
	public class AnnotationTypeMismatchException : RuntimeException
	{
		private new const long SerialVersionUID = 8125925355765570191L;

		/// <summary>
		/// The <tt>Method</tt> object for the annotation element.
		/// </summary>
		private readonly Method Element_Renamed;

		/// <summary>
		/// The (erroneous) type of data found in the annotation.  This string
		/// may, but is not required to, contain the value as well.  The exact
		/// format of the string is unspecified.
		/// </summary>
		private readonly String FoundType_Renamed;

		/// <summary>
		/// Constructs an AnnotationTypeMismatchException for the specified
		/// annotation type element and found data type.
		/// </summary>
		/// <param name="element"> the <tt>Method</tt> object for the annotation element </param>
		/// <param name="foundType"> the (erroneous) type of data found in the annotation.
		///        This string may, but is not required to, contain the value
		///        as well.  The exact format of the string is unspecified. </param>
		public AnnotationTypeMismatchException(Method element, String foundType) : base("Incorrectly typed data found for annotation element " + element + " (Found data of type " + foundType + ")")
		{
			this.Element_Renamed = element;
			this.FoundType_Renamed = foundType;
		}

		/// <summary>
		/// Returns the <tt>Method</tt> object for the incorrectly typed element.
		/// </summary>
		/// <returns> the <tt>Method</tt> object for the incorrectly typed element </returns>
		public virtual Method Element()
		{
			return this.Element_Renamed;
		}

		/// <summary>
		/// Returns the type of data found in the incorrectly typed element.
		/// The returned string may, but is not required to, contain the value
		/// as well.  The exact format of the string is unspecified.
		/// </summary>
		/// <returns> the type of data found in the incorrectly typed element </returns>
		public virtual String FoundType()
		{
			return this.FoundType_Renamed;
		}
	}

}