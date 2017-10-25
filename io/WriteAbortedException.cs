using System;

/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that one of the ObjectStreamExceptions was thrown during a
	/// write operation.  Thrown during a read operation when one of the
	/// ObjectStreamExceptions was thrown during a write operation.  The
	/// exception that terminated the write can be found in the detail
	/// field. The stream is reset to it's initial state and all references
	/// to objects already deserialized are discarded.
	/// 
	/// <para>As of release 1.4, this exception has been retrofitted to conform to
	/// the general purpose exception-chaining mechanism.  The "exception causing
	/// the abort" that is provided at construction time and
	/// accessed via the public <seealso cref="#detail"/> field is now known as the
	/// <i>cause</i>, and may be accessed via the <seealso cref="Throwable#getCause()"/>
	/// method, as well as the aforementioned "legacy field."
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </para>
	/// </summary>
	public class WriteAbortedException : ObjectStreamException
	{
		private new const long SerialVersionUID = -3326426625597282442L;

		/// <summary>
		/// Exception that was caught while writing the ObjectStream.
		/// 
		/// <para>This field predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		public Exception Detail;

		/// <summary>
		/// Constructs a WriteAbortedException with a string describing
		/// the exception and the exception causing the abort. </summary>
		/// <param name="s">   String describing the exception. </param>
		/// <param name="ex">  Exception causing the abort. </param>
		public WriteAbortedException(String s, Exception ex) : base(s)
		{
			InitCause(null); // Disallow subsequent initCause
			Detail = ex;
		}

		/// <summary>
		/// Produce the message and include the message from the nested
		/// exception, if there is one.
		/// </summary>
		public override String Message
		{
			get
			{
				if (Detail == null)
				{
					return base.Message;
				}
				else
				{
					return base.Message + "; " + Detail.ToString();
				}
			}
		}

		/// <summary>
		/// Returns the exception that terminated the operation (the <i>cause</i>).
		/// </summary>
		/// <returns>  the exception that terminated the operation (the <i>cause</i>),
		///          which may be null.
		/// @since   1.4 </returns>
		public override Throwable Cause
		{
			get
			{
				return Detail;
			}
		}
	}

}