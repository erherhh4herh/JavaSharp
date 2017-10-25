using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	/// <summary>
	/// LambdaConversionException
	/// </summary>
	public class LambdaConversionException : Exception
	{
		private new static readonly long SerialVersionUID = 292L + 8L;

		/// <summary>
		/// Constructs a {@code LambdaConversionException}.
		/// </summary>
		public LambdaConversionException()
		{
		}

		/// <summary>
		/// Constructs a {@code LambdaConversionException} with a message. </summary>
		/// <param name="message"> the detail message </param>
		public LambdaConversionException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a {@code LambdaConversionException} with a message and cause. </summary>
		/// <param name="message"> the detail message </param>
		/// <param name="cause"> the cause </param>
		public LambdaConversionException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a {@code LambdaConversionException} with a cause. </summary>
		/// <param name="cause"> the cause </param>
		public LambdaConversionException(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a {@code LambdaConversionException} with a message,
		/// cause, and other settings. </summary>
		/// <param name="message"> the detail message </param>
		/// <param name="cause"> the cause </param>
		/// <param name="enableSuppression"> whether or not suppressed exceptions are enabled </param>
		/// <param name="writableStackTrace"> whether or not the stack trace is writable </param>
		public LambdaConversionException(String message, Throwable cause, bool enableSuppression, bool writableStackTrace) : base(message, cause, enableSuppression, writableStackTrace)
		{
		}
	}

}