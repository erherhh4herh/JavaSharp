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
	/// Unchecked exception thrown when the formatter has been closed.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class FormatterClosedException : IllegalStateException
	{

		private new const long SerialVersionUID = 18111216L;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		public FormatterClosedException()
		{
		}
	}

}