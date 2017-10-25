/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// Thrown when a syntactically malformed signature attribute is
	/// encountered by a reflective method that needs to interpret the
	/// generic signature information for a type, method or constructor.
	/// 
	/// @since 1.5
	/// </summary>
	public class GenericSignatureFormatError : ClassFormatError
	{
		private new const long SerialVersionUID = 6709919147137911034L;

		/// <summary>
		/// Constructs a new {@code GenericSignatureFormatError}.
		/// 
		/// </summary>
		public GenericSignatureFormatError() : base()
		{
		}

		/// <summary>
		/// Constructs a new {@code GenericSignatureFormatError} with the
		/// specified message.
		/// </summary>
		/// <param name="message"> the detail message, may be {@code null} </param>
		public GenericSignatureFormatError(String message) : base(message)
		{
		}
	}

}