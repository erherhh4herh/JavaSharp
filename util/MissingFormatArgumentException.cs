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
	/// Unchecked exception thrown when there is a format specifier which does not
	/// have a corresponding argument or if an argument index refers to an argument
	/// that does not exist.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class MissingFormatArgumentException : IllegalFormatException
	{

		private new const long SerialVersionUID = 19190115L;

		private String s;

		/// <summary>
		/// Constructs an instance of this class with the unmatched format
		/// specifier.
		/// </summary>
		/// <param name="s">
		///         Format specifier which does not have a corresponding argument </param>
		public MissingFormatArgumentException(String s)
		{
			if (s == null)
			{
				throw new NullPointerException();
			}
			this.s = s;
		}

		/// <summary>
		/// Returns the unmatched format specifier.
		/// </summary>
		/// <returns>  The unmatched format specifier </returns>
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
				return "Format specifier '" + s + "'";
			}
		}
	}

}