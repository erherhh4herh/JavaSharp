/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// Unchecked exception thrown when an attempt is made to invoke a method on an
	/// object created by one file system provider with a parameter created by a
	/// different file system provider.
	/// </summary>
	public class ProviderMismatchException : System.ArgumentException
	{
		internal new const long SerialVersionUID = 4990847485741612530L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ProviderMismatchException()
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="msg">
		///          the detail message </param>
		public ProviderMismatchException(String msg) : base(msg)
		{
		}
	}

}