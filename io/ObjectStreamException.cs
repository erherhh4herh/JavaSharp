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
	/// Superclass of all exceptions specific to Object Stream classes.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public abstract class ObjectStreamException : IOException
	{

		private new const long SerialVersionUID = 7260898174833392607L;

		/// <summary>
		/// Create an ObjectStreamException with the specified argument.
		/// </summary>
		/// <param name="classname"> the detailed message for the exception </param>
		protected internal ObjectStreamException(String classname) : base(classname)
		{
		}

		/// <summary>
		/// Create an ObjectStreamException.
		/// </summary>
		protected internal ObjectStreamException() : base()
		{
		}
	}

}