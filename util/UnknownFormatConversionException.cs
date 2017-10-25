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
	/// Unchecked exception thrown when an unknown conversion is given.
	/// 
	/// <para> Unless otherwise specified, passing a <tt>null</tt> argument to
	/// any method or constructor in this class will cause a {@link
	/// NullPointerException} to be thrown.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public class UnknownFormatConversionException : IllegalFormatException
	{

		private new const long SerialVersionUID = 19060418L;

		private String s;

		/// <summary>
		/// Constructs an instance of this class with the unknown conversion.
		/// </summary>
		/// <param name="s">
		///         Unknown conversion </param>
		public UnknownFormatConversionException(String s)
		{
			if (s == null)
			{
				throw new NullPointerException();
			}
			this.s = s;
		}

		/// <summary>
		/// Returns the unknown conversion.
		/// </summary>
		/// <returns>  The unknown conversion. </returns>
		public virtual String Conversion
		{
			get
			{
				return s;
			}
		}

		// javadoc inherited from Throwable.java
		public override String Message
		{
			get
			{
				return string.Format("Conversion = '{0}'", s);
			}
		}
	}

}