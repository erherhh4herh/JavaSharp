/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Unchecked exception thrown when the format width is required.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class MissingFormatWidthException : IllegalFormatException
	{

		private new const long SerialVersionUID = 15560123L;

		private String s;

		/// <summary>
		/// Constructs an instance of this class with the specified format
		/// specifier.
		/// </summary>
		/// <param name="s">
		///         The format specifier which does not have a width </param>
		public MissingFormatWidthException(String s)
		{
			if (s == null)
			{
				throw new NullPointerException();
			}
			this.s = s;
		}

		/// <summary>
		/// Returns the format specifier which does not have a width.
		/// </summary>
		/// <returns>  The format specifier which does not have a width </returns>
		public virtual String FormatSpecifier
		{
			get
			{
				return s;
			}
		}

		public override String Message
		{
			get
			{
				return s;
			}
		}
	}

}