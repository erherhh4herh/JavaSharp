/*
 * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.jar
{

	/// <summary>
	/// Signals that an error of some sort has occurred while reading from
	/// or writing to a JAR file.
	/// 
	/// @author  David Connelly
	/// @since   1.2
	/// </summary>
	public class JarException : java.util.zip.ZipException
	{
		private new const long SerialVersionUID = 7159778400963954473L;

		/// <summary>
		/// Constructs a JarException with no detail message.
		/// </summary>
		public JarException()
		{
		}

		/// <summary>
		/// Constructs a JarException with the specified detail message. </summary>
		/// <param name="s"> the detail message </param>
		public JarException(String s) : base(s)
		{
		}
	}

}