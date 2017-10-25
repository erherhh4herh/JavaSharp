/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// InvocationTargetException is a checked exception that wraps
	/// an exception thrown by an invoked method or constructor.
	/// 
	/// <para>As of release 1.4, this exception has been retrofitted to conform to
	/// the general purpose exception-chaining mechanism.  The "target exception"
	/// that is provided at construction time and accessed via the
	/// <seealso cref="#getTargetException()"/> method is now known as the <i>cause</i>,
	/// and may be accessed via the <seealso cref="Throwable#getCause()"/> method,
	/// as well as the aforementioned "legacy method."
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Method </seealso>
	/// <seealso cref= Constructor </seealso>
	public class InvocationTargetException : ReflectiveOperationException
	{
		/// <summary>
		/// Use serialVersionUID from JDK 1.1.X for interoperability
		/// </summary>
		private new const long SerialVersionUID = 4085088731926701167L;

		 /// <summary>
		 /// This field holds the target if the
		 /// InvocationTargetException(Throwable target) constructor was
		 /// used to instantiate the object
		 /// 
		 /// @serial
		 /// 
		 /// </summary>
		private Throwable Target;

		/// <summary>
		/// Constructs an {@code InvocationTargetException} with
		/// {@code null} as the target exception.
		/// </summary>
		protected internal InvocationTargetException() : base((Throwable)null); / / Disallow initCause
		{
		}

		/// <summary>
		/// Constructs a InvocationTargetException with a target exception.
		/// </summary>
		/// <param name="target"> the target exception </param>
		public InvocationTargetException(Throwable target) : base((Throwable)null); / / Disallow initCause
		{
			this.Target = target;
		}

		/// <summary>
		/// Constructs a InvocationTargetException with a target exception
		/// and a detail message.
		/// </summary>
		/// <param name="target"> the target exception </param>
		/// <param name="s">      the detail message </param>
		public InvocationTargetException(Throwable target, String s) : base(s, null); / / Disallow initCause
		{
			this.Target = target;
		}

		/// <summary>
		/// Get the thrown target exception.
		/// 
		/// <para>This method predates the general-purpose exception chaining facility.
		/// The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		/// obtaining this information.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the thrown target exception (cause of this exception). </returns>
		public virtual Throwable TargetException
		{
			get
			{
				return Target;
			}
		}

		/// <summary>
		/// Returns the cause of this exception (the thrown target exception,
		/// which may be {@code null}).
		/// </summary>
		/// <returns>  the cause of this exception.
		/// @since   1.4 </returns>
		public override Throwable Cause
		{
			get
			{
				return Target;
			}
		}
	}

}