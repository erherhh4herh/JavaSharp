using System;

/*
 * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.print
{

	/// <summary>
	/// The <code>PrinterException</code> class and its subclasses are used
	/// to indicate that an exceptional condition has occurred in the print
	/// system.
	/// </summary>

	public class PrinterException : Exception
	{

		/// <summary>
		/// Constructs a new <code>PrinterException</code> object
		/// without a detail message.
		/// </summary>
		public PrinterException()
		{

		}

		/// <summary>
		/// Constructs a new <code>PrinterException</code> object
		/// with the specified detail message. </summary>
		/// <param name="msg"> the message to generate when a
		/// <code>PrinterException</code> is thrown </param>
		public PrinterException(String msg) : base(msg)
		{
		}
	}

}