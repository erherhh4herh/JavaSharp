/*
 * Copyright (c) 2004, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when the annotation parser attempts to read an annotation
	/// from a class file and determines that the annotation is malformed.
	/// This error can be thrown by the {@linkplain
	/// java.lang.reflect.AnnotatedElement API used to read annotations
	/// reflectively}.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     java.lang.reflect.AnnotatedElement
	/// @since   1.5 </seealso>
	public class AnnotationFormatError : Error
	{
		private new const long SerialVersionUID = -4256701562333669892L;

		/// <summary>
		/// Constructs a new <tt>AnnotationFormatError</tt> with the specified
		/// detail message.
		/// </summary>
		/// <param name="message">   the detail message. </param>
		public AnnotationFormatError(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new <tt>AnnotationFormatError</tt> with the specified
		/// detail message and cause.  Note that the detail message associated
		/// with <code>cause</code> is <i>not</i> automatically incorporated in
		/// this error's detail message.
		/// </summary>
		/// <param name="message"> the detail message </param>
		/// <param name="cause"> the cause (A <tt>null</tt> value is permitted, and
		///     indicates that the cause is nonexistent or unknown.) </param>
		public AnnotationFormatError(String message, Throwable cause) : base(message, cause)
		{
		}


		/// <summary>
		/// Constructs a new <tt>AnnotationFormatError</tt> with the specified
		/// cause and a detail message of
		/// <tt>(cause == null ? null : cause.toString())</tt> (which
		/// typically contains the class and detail message of <tt>cause</tt>).
		/// </summary>
		/// <param name="cause"> the cause (A <tt>null</tt> value is permitted, and
		///     indicates that the cause is nonexistent or unknown.) </param>
		public AnnotationFormatError(Throwable cause) : base(cause)
		{
		}
	}

}