using System;

/*
 * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that an assertion has failed.
	/// 
	/// <para>The seven one-argument public constructors provided by this
	/// class ensure that the assertion error returned by the invocation:
	/// <pre>
	///     new AssertionError(<i>expression</i>)
	/// </pre>
	/// has as its detail message the <i>string conversion</i> of
	/// <i>expression</i> (as defined in section 15.18.1.1 of
	/// <cite>The Java&trade; Language Specification</cite>),
	/// regardless of the type of <i>expression</i>.
	/// 
	/// @since   1.4
	/// </para>
	/// </summary>
	public class AssertionError : Error
	{
		private new const long SerialVersionUID = -5013299493970297370L;

		/// <summary>
		/// Constructs an AssertionError with no detail message.
		/// </summary>
		public AssertionError()
		{
		}

		/// <summary>
		/// This internal constructor does no processing on its string argument,
		/// even if it is a null reference.  The public constructors will
		/// never call this constructor with a null argument.
		/// </summary>
		private AssertionError(String detailMessage) : base(detailMessage)
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified object, which is converted to a string as
		/// defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// <para>
		/// If the specified object is an instance of {@code Throwable}, it
		/// becomes the <i>cause</i> of the newly constructed assertion error.
		/// 
		/// </para>
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		/// <seealso cref=   Throwable#getCause() </seealso>
		public AssertionError(Object detailMessage) : this(Convert.ToString(detailMessage))
		{
			if (detailMessage is Throwable)
			{
				InitCause((Throwable) detailMessage);
			}
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>boolean</code>, which is converted to
		/// a string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(bool detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>char</code>, which is converted to a
		/// string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(char detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>int</code>, which is converted to a
		/// string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(int detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>long</code>, which is converted to a
		/// string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(long detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>float</code>, which is converted to a
		/// string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(float detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs an AssertionError with its detail message derived
		/// from the specified <code>double</code>, which is converted to a
		/// string as defined in section 15.18.1.1 of
		/// <cite>The Java&trade; Language Specification</cite>.
		/// </summary>
		/// <param name="detailMessage"> value to be used in constructing detail message </param>
		public AssertionError(double detailMessage) : this(Convert.ToString(detailMessage))
		{
		}

		/// <summary>
		/// Constructs a new {@code AssertionError} with the specified
		/// detail message and cause.
		/// 
		/// <para>Note that the detail message associated with
		/// {@code cause} is <i>not</i> automatically incorporated in
		/// this error's detail message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message, may be {@code null} </param>
		/// <param name="cause"> the cause, may be {@code null}
		/// 
		/// @since 1.7 </param>
		public AssertionError(String message, Throwable cause) : base(message, cause)
		{
		}
	}

}