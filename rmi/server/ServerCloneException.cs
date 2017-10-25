using System;

/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// A <code>ServerCloneException</code> is thrown if a remote exception occurs
	/// during the cloning of a <code>UnicastRemoteObject</code>.
	/// 
	/// <para>As of release 1.4, this exception has been retrofitted to conform to
	/// the general purpose exception-chaining mechanism.  The "nested exception"
	/// that may be provided at construction time and accessed via the public
	/// <seealso cref="#detail"/> field is now known as the <i>cause</i>, and may be
	/// accessed via the <seealso cref="Throwable#getCause()"/> method, as well as
	/// the aforementioned "legacy field."
	/// 
	/// </para>
	/// <para>Invoking the method <seealso cref="Throwable#initCause(Throwable)"/> on an
	/// instance of <code>ServerCloneException</code> always throws {@link
	/// IllegalStateException}.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </para>
	/// </summary>
	/// <seealso cref=     java.rmi.server.UnicastRemoteObject#clone() </seealso>
	public class ServerCloneException : CloneNotSupportedException
	{

		/// <summary>
		/// The cause of the exception.
		/// 
		/// <para>This field predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		public Exception Detail;

		/* indicate compatibility with JDK 1.1.x version of class */
		private new const long SerialVersionUID = 6617456357664815945L;

		/// <summary>
		/// Constructs a <code>ServerCloneException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		public ServerCloneException(String s) : base(s)
		{
			InitCause(null); // Disallow subsequent initCause
		}

		/// <summary>
		/// Constructs a <code>ServerCloneException</code> with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		/// <param name="cause"> the cause </param>
		public ServerCloneException(String s, Exception cause) : base(s)
		{
			InitCause(null); // Disallow subsequent initCause
			Detail = cause;
		}

		/// <summary>
		/// Returns the detail message, including the message from the cause, if
		/// any, of this exception.
		/// </summary>
		/// <returns> the detail message </returns>
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
					return base.Message + "; nested exception is: \n\t" + Detail.ToString();
				}
			}
		}

		/// <summary>
		/// Returns the cause of this exception.  This method returns the value
		/// of the <seealso cref="#detail"/> field.
		/// </summary>
		/// <returns>  the cause, which may be <tt>null</tt>.
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