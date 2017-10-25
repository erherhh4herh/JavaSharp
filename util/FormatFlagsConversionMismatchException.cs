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
	/// Unchecked exception thrown when a conversion and flag are incompatible.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class FormatFlagsConversionMismatchException : IllegalFormatException
	{
		private new const long SerialVersionUID = 19120414L;

		private String f;

		private char c;

		/// <summary>
		/// Constructs an instance of this class with the specified flag
		/// and conversion.
		/// </summary>
		/// <param name="f">
		///         The flag
		/// </param>
		/// <param name="c">
		///         The conversion </param>
		public FormatFlagsConversionMismatchException(String f, char c)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			this.f = f;
			this.c = c;
		}

		/// <summary>
		/// Returns the incompatible flag.
		/// </summary>
		/// <returns>  The flag </returns>
		 public virtual String Flags
		 {
			 get
			 {
				return f;
			 }
		 }

		/// <summary>
		/// Returns the incompatible conversion.
		/// </summary>
		/// <returns>  The conversion </returns>
		public virtual char Conversion
		{
			get
			{
				return c;
			}
		}

		public override String Message
		{
			get
			{
				return "Conversion = " + c + ", Flags = " + f;
			}
		}
	}

}