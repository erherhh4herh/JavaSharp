/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when the Serialization runtime detects one of the following
	/// problems with a Class.
	/// <UL>
	/// <LI> The serial version of the class does not match that of the class
	///      descriptor read from the stream
	/// <LI> The class contains unknown datatypes
	/// <LI> The class does not have an accessible no-arg constructor
	/// </UL>
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public class InvalidClassException : ObjectStreamException
	{

		private new const long SerialVersionUID = -4333316296251054416L;

		/// <summary>
		/// Name of the invalid class.
		/// 
		/// @serial Name of the invalid class.
		/// </summary>
		public String Classname;

		/// <summary>
		/// Report an InvalidClassException for the reason specified.
		/// </summary>
		/// <param name="reason">  String describing the reason for the exception. </param>
		public InvalidClassException(String reason) : base(reason)
		{
		}

		/// <summary>
		/// Constructs an InvalidClassException object.
		/// </summary>
		/// <param name="cname">   a String naming the invalid class. </param>
		/// <param name="reason">  a String describing the reason for the exception. </param>
		public InvalidClassException(String cname, String reason) : base(reason)
		{
			Classname = cname;
		}

		/// <summary>
		/// Produce the message and include the classname, if present.
		/// </summary>
		public override String Message
		{
			get
			{
				if (Classname == null)
				{
					return base.Message;
				}
				else
				{
					return Classname + "; " + base.Message;
				}
			}
		}
	}

}