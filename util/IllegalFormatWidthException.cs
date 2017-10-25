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
	/// Unchecked exception thrown when the format width is a negative value other
	/// than <tt>-1</tt> or is otherwise unsupported.
	/// 
	/// @since 1.5
	/// </summary>
	public class IllegalFormatWidthException : IllegalFormatException
	{

		private new const long SerialVersionUID = 16660902L;

		private int w;

		/// <summary>
		/// Constructs an instance of this class with the specified width.
		/// </summary>
		/// <param name="w">
		///         The width </param>
		public IllegalFormatWidthException(int w)
		{
			this.w = w;
		}

		/// <summary>
		/// Returns the width
		/// </summary>
		/// <returns>  The width </returns>
		public virtual int Width
		{
			get
			{
				return w;
			}
		}

		public override String Message
		{
			get
			{
				return Convert.ToString(w);
			}
		}
	}

}