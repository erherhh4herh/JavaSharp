using System;

/*
 * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.activation
{

	/// <summary>
	/// General exception used by the activation interfaces.
	/// 
	/// <para>As of release 1.4, this exception has been retrofitted to conform to
	/// the general purpose exception-chaining mechanism.  The "detail exception"
	/// that may be provided at construction time and accessed via the public
	/// <seealso cref="#detail"/> field is now known as the <i>cause</i>, and may be
	/// accessed via the <seealso cref="Throwable#getCause()"/> method, as well as
	/// the aforementioned "legacy field."
	/// 
	/// </para>
	/// <para>Invoking the method <seealso cref="Throwable#initCause(Throwable)"/> on an
	/// instance of <code>ActivationException</code> always throws {@link
	/// IllegalStateException}.
	/// 
	/// @author      Ann Wollrath
	/// @since       1.2
	/// </para>
	/// </summary>
	public class ActivationException : Exception
	{

		/// <summary>
		/// The cause of the activation exception.
		/// 
		/// <para>This field predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		public Throwable Detail;

		/// <summary>
		/// indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		private new const long SerialVersionUID = -4320118837291406071L;

		/// <summary>
		/// Constructs an <code>ActivationException</code>.
		/// </summary>
		public ActivationException()
		{
			InitCause(null); // Disallow subsequent initCause
		}

		/// <summary>
		/// Constructs an <code>ActivationException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message </param>
		public ActivationException(String s) : base(s)
		{
			InitCause(null); // Disallow subsequent initCause
		}

		/// <summary>
		/// Constructs an <code>ActivationException</code> with the specified
		/// detail message and cause.  This constructor sets the <seealso cref="#detail"/>
		/// field to the specified <code>Throwable</code>.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="cause"> the cause </param>
		public ActivationException(String s, Throwable cause) : base(s)
		{
			InitCause(null); // Disallow subsequent initCause
			Detail = cause;
		}

		/// <summary>
		/// Returns the detail message, including the message from the cause, if
		/// any, of this exception.
		/// </summary>
		/// <returns>  the detail message </returns>
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