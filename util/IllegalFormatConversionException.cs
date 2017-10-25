/*
 * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Unchecked exception thrown when the argument corresponding to the format
	/// specifier is of an incompatible type.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class IllegalFormatConversionException : IllegalFormatException
	{

		private new const long SerialVersionUID = 17000126L;

		private char c;
		private Class Arg;

		/// <summary>
		/// Constructs an instance of this class with the mismatched conversion and
		/// the corresponding argument class.
		/// </summary>
		/// <param name="c">
		///         Inapplicable conversion
		/// </param>
		/// <param name="arg">
		///         Class of the mismatched argument </param>
		public IllegalFormatConversionException(char c, Class arg)
		{
			if (arg == null)
			{
				throw new NullPointerException();
			}
			this.c = c;
			this.Arg = arg;
		}

		/// <summary>
		/// Returns the inapplicable conversion.
		/// </summary>
		/// <returns>  The inapplicable conversion </returns>
		public virtual char Conversion
		{
			get
			{
				return c;
			}
		}

		/// <summary>
		/// Returns the class of the mismatched argument.
		/// </summary>
		/// <returns>   The class of the mismatched argument </returns>
		public virtual Class ArgumentClass
		{
			get
			{
				return Arg;
			}
		}

		// javadoc inherited from Throwable.java
		public override String Message
		{
			get
			{
				return string.Format("{0} != {1}", c, Arg.Name);
			}
		}
	}

}