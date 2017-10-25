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

namespace java.lang
{

	/// <summary>
	/// Thrown when an application tries to access a type using a string
	/// representing the type's name, but no definition for the type with
	/// the specified name can be found.   This exception differs from
	/// <seealso cref="ClassNotFoundException"/> in that <tt>ClassNotFoundException</tt> is a
	/// checked exception, whereas this exception is unchecked.
	/// 
	/// <para>Note that this exception may be used when undefined type variables
	/// are accessed as well as when types (e.g., classes, interfaces or
	/// annotation types) are loaded.
	/// In particular, this exception can be thrown by the {@linkplain
	/// java.lang.reflect.AnnotatedElement API used to read annotations
	/// reflectively}.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.reflect.AnnotatedElement
	/// @since 1.5 </seealso>
	public class TypeNotPresentException : RuntimeException
	{
		private new const long SerialVersionUID = -5101214195716534496L;

		private String TypeName_Renamed;

		/// <summary>
		/// Constructs a <tt>TypeNotPresentException</tt> for the named type
		/// with the specified cause.
		/// </summary>
		/// <param name="typeName"> the fully qualified name of the unavailable type </param>
		/// <param name="cause"> the exception that was thrown when the system attempted to
		///    load the named type, or <tt>null</tt> if unavailable or inapplicable </param>
		public TypeNotPresentException(String typeName, Throwable cause) : base("Type " + typeName + " not present", cause)
		{
			this.TypeName_Renamed = typeName;
		}

		/// <summary>
		/// Returns the fully qualified name of the unavailable type.
		/// </summary>
		/// <returns> the fully qualified name of the unavailable type </returns>
		public virtual String TypeName()
		{
			return TypeName_Renamed;
		}
	}

}