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
	/// Runtime exception thrown when a provider of the required type cannot be found.
	/// </summary>

	public class ProviderNotFoundException : RuntimeException
	{
		internal new const long SerialVersionUID = -1880012509822920354L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public ProviderNotFoundException()
		{
		}

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="msg">
		///          the detail message </param>
		public ProviderNotFoundException(String msg) : base(msg)
		{
		}
	}

}