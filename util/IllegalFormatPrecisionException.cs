using System;

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
	/// Unchecked exception thrown when the precision is a negative value other than
	/// <tt>-1</tt>, the conversion does not support a precision, or the value is
	/// otherwise unsupported.
	/// 
	/// @since 1.5
	/// </summary>
	public class IllegalFormatPrecisionException : IllegalFormatException
	{

		private new const long SerialVersionUID = 18711008L;

		private int p;

		/// <summary>
		/// Constructs an instance of this class with the specified precision.
		/// </summary>
		/// <param name="p">
		///         The precision </param>
		public IllegalFormatPrecisionException(int p)
		{
			this.p = p;
		}

		/// <summary>
		/// Returns the precision
		/// </summary>
		/// <returns>  The precision </returns>
		public virtual int Precision
		{
			get
			{
				return p;
			}
		}

		public override String Message
		{
			get
			{
				return Convert.ToString(p);
			}
		}
	}

}