/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// Thrown when an instance is required to have a Serializable interface.
	/// The serialization runtime or the class of the instance can throw
	/// this exception. The argument should be the name of the class.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public class NotSerializableException : ObjectStreamException
	{

		private new const long SerialVersionUID = 2906642554793891381L;

		/// <summary>
		/// Constructs a NotSerializableException object with message string.
		/// </summary>
		/// <param name="classname"> Class of the instance being serialized/deserialized. </param>
		public NotSerializableException(String classname) : base(classname)
		{
		}

		/// <summary>
		///  Constructs a NotSerializableException object.
		/// </summary>
		public NotSerializableException() : base()
		{
		}
	}

}