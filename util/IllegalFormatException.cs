/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// Unchecked exception thrown when a format string contains an illegal syntax
	/// or a format specifier that is incompatible with the given arguments.  Only
	/// explicit subtypes of this exception which correspond to specific errors
	/// should be instantiated.
	/// 
	/// @since 1.5
	/// </summary>
	public class IllegalFormatException : IllegalArgumentException
	{

		private new const long SerialVersionUID = 18830826L;

		// package-private to prevent explicit instantiation
		internal IllegalFormatException()
		{
		}
	}

}