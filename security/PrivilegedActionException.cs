using System;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// This exception is thrown by
	/// {@code doPrivileged(PrivilegedExceptionAction)} and
	/// {@code doPrivileged(PrivilegedExceptionAction,
	/// AccessControlContext context)} to indicate
	/// that the action being performed threw a checked exception.  The exception
	/// thrown by the action can be obtained by calling the
	/// {@code getException} method.  In effect, an
	/// {@code PrivilegedActionException} is a "wrapper"
	/// for an exception thrown by a privileged action.
	/// 
	/// <para>As of release 1.4, this exception has been retrofitted to conform to
	/// the general purpose exception-chaining mechanism.  The "exception thrown
	/// by the privileged computation" that is provided at construction time and
	/// accessed via the <seealso cref="#getException()"/> method is now known as the
	/// <i>cause</i>, and may be accessed via the <seealso cref="Throwable#getCause()"/>
	/// method, as well as the aforementioned "legacy method."
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= PrivilegedExceptionAction </seealso>
	/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
	/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
	public class PrivilegedActionException : Exception
	{
		// use serialVersionUID from JDK 1.2.2 for interoperability
		private new const long SerialVersionUID = 4724086851538908602L;

		/// <summary>
		/// @serial
		/// </summary>
		private Exception Exception_Renamed;

		/// <summary>
		/// Constructs a new PrivilegedActionException &quot;wrapping&quot;
		/// the specific Exception.
		/// </summary>
		/// <param name="exception"> The exception thrown </param>
		public PrivilegedActionException(Exception exception) : base((Throwable)null); / / Disallow initCause
		{
			this.Exception_Renamed = exception;
		}

		/// <summary>
		/// Returns the exception thrown by the privileged computation that
		/// resulted in this {@code PrivilegedActionException}.
		/// 
		/// <para>This method predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the exception thrown by the privileged computation that
		///         resulted in this {@code PrivilegedActionException}. </returns>
		/// <seealso cref= PrivilegedExceptionAction </seealso>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,
		///                                            AccessControlContext) </seealso>
		public virtual Exception Exception
		{
			get
			{
				return Exception_Renamed;
			}
		}

		/// <summary>
		/// Returns the cause of this exception (the exception thrown by
		/// the privileged computation that resulted in this
		/// {@code PrivilegedActionException}).
		/// </summary>
		/// <returns>  the cause of this exception.
		/// @since   1.4 </returns>
		public override Throwable Cause
		{
			get
			{
				return Exception_Renamed;
			}
		}

		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			String s = this.GetType().FullName;
			return (Exception_Renamed != null) ? (s + ": " + Exception_Renamed.ToString()) : s;
		}
	}

}