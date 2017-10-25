using System;

/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi
{

	/// <summary>
	/// An <code>AlreadyBoundException</code> is thrown if an attempt
	/// is made to bind an object in the registry to a name that already
	/// has an associated binding.
	/// 
	/// @since   JDK1.1
	/// @author  Ann Wollrath
	/// @author  Roger Riggs </summary>
	/// <seealso cref=     java.rmi.Naming#bind(String, java.rmi.Remote) </seealso>
	/// <seealso cref=     java.rmi.registry.Registry#bind(String, java.rmi.Remote) </seealso>
	public class AlreadyBoundException : Exception
	{

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = 9218657361741657110L;

		/// <summary>
		/// Constructs an <code>AlreadyBoundException</code> with no
		/// specified detail message.
		/// @since JDK1.1
		/// </summary>
		public AlreadyBoundException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>AlreadyBoundException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message
		/// @since JDK1.1 </param>
		public AlreadyBoundException(String s) : base(s)
		{
		}
	}

}