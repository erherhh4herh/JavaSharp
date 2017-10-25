/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
namespace java.io
{

	/// <summary>
	/// Base class for character conversion exceptions.
	/// 
	/// @author      Asmus Freytag
	/// @since       JDK1.1
	/// </summary>
	public class CharConversionException : java.io.IOException
	{
		private new const long SerialVersionUID = -8680016352018427031L;

		/// <summary>
		/// This provides no detailed message.
		/// </summary>
		public CharConversionException()
		{
		}
		/// <summary>
		/// This provides a detailed message.
		/// </summary>
		/// <param name="s"> the detailed message associated with the exception. </param>
		public CharConversionException(String s) : base(s)
		{
		}
	}

}