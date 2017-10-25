using System;

/*
 * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.color
{

	/// <summary>
	/// This exception is thrown when an error occurs in accessing or
	/// processing an ICC_Profile object.
	/// </summary>

	public class ProfileDataException : Exception
	{

		/// <summary>
		///  Constructs a ProfileDataException with the specified detail message. </summary>
		///  <param name="s"> the specified detail message </param>
		public ProfileDataException(String s) : base(s)
		{
		}
	}

}