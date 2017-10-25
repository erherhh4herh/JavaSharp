/*
 * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{


	/// <summary>
	/// Thrown to indicate that an operation could not complete because
	/// the input did not conform to the appropriate XML document type
	/// for a collection of properties, as per the <seealso cref="Properties"/>
	/// specification.<para>
	/// 
	/// Note, that although InvalidPropertiesFormatException inherits Serializable
	/// interface from Exception, it is not intended to be Serializable. Appropriate
	/// serialization methods are implemented to throw NotSerializableException.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     Properties
	/// @since   1.5
	/// @serial exclude </seealso>

	public class InvalidPropertiesFormatException : IOException
	{

		private new const long SerialVersionUID = 7763056076009360219L;

		/// <summary>
		/// Constructs an InvalidPropertiesFormatException with the specified
		/// cause.
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="Throwable#getCause()"/> method). </param>
		public InvalidPropertiesFormatException(Throwable cause) : base(cause == null ? null : cause.ToString())
		{
			this.InitCause(cause);
		}

	   /// <summary>
	   /// Constructs an InvalidPropertiesFormatException with the specified
	   /// detail message.
	   /// </summary>
	   /// <param name="message">   the detail message. The detail message is saved for
	   ///          later retrieval by the <seealso cref="Throwable#getMessage()"/> method. </param>
		public InvalidPropertiesFormatException(String message) : base(message)
		{
		}

		/// <summary>
		/// Throws NotSerializableException, since InvalidPropertiesFormatException
		/// objects are not intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.NotSerializableException
		private void WriteObject(java.io.ObjectOutputStream @out)
		{
			throw new NotSerializableException("Not serializable.");
		}

		/// <summary>
		/// Throws NotSerializableException, since InvalidPropertiesFormatException
		/// objects are not intended to be serializable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.NotSerializableException
		private void ReadObject(java.io.ObjectInputStream @in)
		{
			throw new NotSerializableException("Not serializable.");
		}

	}

}