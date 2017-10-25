/*
 * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	/// <summary>
	/// Thrown by a method invocation on a proxy instance if its invocation
	/// handler's <seealso cref="InvocationHandler#invoke invoke"/> method throws a
	/// checked exception (a {@code Throwable} that is not assignable
	/// to {@code RuntimeException} or {@code Error}) that
	/// is not assignable to any of the exception types declared in the
	/// {@code throws} clause of the method that was invoked on the
	/// proxy instance and dispatched to the invocation handler.
	/// 
	/// <para>An {@code UndeclaredThrowableException} instance contains
	/// the undeclared checked exception that was thrown by the invocation
	/// handler, and it can be retrieved with the
	/// {@code getUndeclaredThrowable()} method.
	/// {@code UndeclaredThrowableException} extends
	/// {@code RuntimeException}, so it is an unchecked exception
	/// that wraps a checked exception.
	/// 
	/// </para>
	/// <para>As of release 1.4, this exception has been retrofitted to
	/// conform to the general purpose exception-chaining mechanism.  The
	/// "undeclared checked exception that was thrown by the invocation
	/// handler" that may be provided at construction time and accessed via
	/// the <seealso cref="#getUndeclaredThrowable()"/> method is now known as the
	/// <i>cause</i>, and may be accessed via the {@link
	/// Throwable#getCause()} method, as well as the aforementioned "legacy
	/// method."
	/// 
	/// @author      Peter Jones
	/// </para>
	/// </summary>
	/// <seealso cref=         InvocationHandler
	/// @since       1.3 </seealso>
	public class UndeclaredThrowableException : RuntimeException
	{
		internal new const long SerialVersionUID = 330127114055056639L;

		/// <summary>
		/// the undeclared checked exception that was thrown
		/// @serial
		/// </summary>
		private Throwable UndeclaredThrowable_Renamed;

		/// <summary>
		/// Constructs an {@code UndeclaredThrowableException} with the
		/// specified {@code Throwable}.
		/// </summary>
		/// <param name="undeclaredThrowable"> the undeclared checked exception
		///          that was thrown </param>
		public UndeclaredThrowableException(Throwable undeclaredThrowable) : base((Throwable) null); / / Disallow initCause
		{
			this.UndeclaredThrowable_Renamed = undeclaredThrowable;
		}

		/// <summary>
		/// Constructs an {@code UndeclaredThrowableException} with the
		/// specified {@code Throwable} and a detail message.
		/// </summary>
		/// <param name="undeclaredThrowable"> the undeclared checked exception
		///          that was thrown </param>
		/// <param name="s"> the detail message </param>
		public UndeclaredThrowableException(Throwable undeclaredThrowable, String s) : base(s, null); / / Disallow initCause
		{
			this.UndeclaredThrowable_Renamed = undeclaredThrowable;
		}

		/// <summary>
		/// Returns the {@code Throwable} instance wrapped in this
		/// {@code UndeclaredThrowableException}, which may be {@code null}.
		/// 
		/// <para>This method predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the undeclared checked exception that was thrown </returns>
		public virtual Throwable UndeclaredThrowable
		{
			get
			{
				return UndeclaredThrowable_Renamed;
			}
		}

		/// <summary>
		/// Returns the cause of this exception (the {@code Throwable}
		/// instance wrapped in this {@code UndeclaredThrowableException},
		/// which may be {@code null}).
		/// </summary>
		/// <returns>  the cause of this exception.
		/// @since   1.4 </returns>
		public override Throwable Cause
		{
			get
			{
				return UndeclaredThrowable_Renamed;
			}
		}
	}

}