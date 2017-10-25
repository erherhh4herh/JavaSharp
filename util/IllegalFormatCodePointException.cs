/*
 * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// Unchecked exception thrown when a character with an invalid Unicode code
	/// point as defined by <seealso cref="Character#isValidCodePoint"/> is passed to the
	/// <seealso cref="Formatter"/>.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to any
	/// method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class IllegalFormatCodePointException : IllegalFormatException
	{

		private new const long SerialVersionUID = 19080630L;

		private int c;

		/// <summary>
		/// Constructs an instance of this class with the specified illegal code
		/// point as defined by <seealso cref="Character#isValidCodePoint"/>.
		/// </summary>
		/// <param name="c">
		///         The illegal Unicode code point </param>
		public IllegalFormatCodePointException(int c)
		{
			this.c = c;
		}

		/// <summary>
		/// Returns the illegal code point as defined by {@link
		/// Character#isValidCodePoint}.
		/// </summary>
		/// <returns>  The illegal Unicode code point </returns>
		public virtual int CodePoint
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
	//JAVA TO C# CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
	//ORIGINAL LINE: return String.format("Code point = %#x", c);
				return string.Format("Code point = %#x", c);
			}
		}
	}

}