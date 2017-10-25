/*
 * Copyright (c) 2008, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	/// <summary>
	/// Thrown to indicate that an {@code invokedynamic} instruction has
	/// failed to find its bootstrap method,
	/// or the bootstrap method has failed to provide a
	/// <seealso cref="java.lang.invoke.CallSite call site"/> with a <seealso cref="java.lang.invoke.CallSite#getTarget target"/>
	/// of the correct <seealso cref="java.lang.invoke.MethodHandle#type method type"/>.
	/// 
	/// @author John Rose, JSR 292 EG
	/// @since 1.7
	/// </summary>
	public class BootstrapMethodError : LinkageError
	{
		private new const long SerialVersionUID = 292L;

		/// <summary>
		/// Constructs a {@code BootstrapMethodError} with no detail message.
		/// </summary>
		public BootstrapMethodError() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code BootstrapMethodError} with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		public BootstrapMethodError(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a {@code BootstrapMethodError} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		/// <param name="cause"> the cause, may be {@code null}. </param>
		public BootstrapMethodError(String s, Throwable cause) : base(s, cause)
		{
		}

		/// <summary>
		/// Constructs a {@code BootstrapMethodError} with the specified
		/// cause.
		/// </summary>
		/// <param name="cause"> the cause, may be {@code null}. </param>
		public BootstrapMethodError(Throwable cause) : base(cause == null ? null : cause.ToString())
		{
			// cf. Throwable(Throwable cause) constructor.
			InitCause(cause);
		}
	}

}