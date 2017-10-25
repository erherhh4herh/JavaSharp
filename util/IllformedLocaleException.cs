/*
 * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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

/*
 *******************************************************************************
 * Copyright (C) 2009-2010, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 *******************************************************************************
 */

namespace java.util
{

	/// <summary>
	/// Thrown by methods in <seealso cref="Locale"/> and <seealso cref="Locale.Builder"/> to
	/// indicate that an argument is not a well-formed BCP 47 tag.
	/// </summary>
	/// <seealso cref= Locale
	/// @since 1.7 </seealso>
	public class IllformedLocaleException : RuntimeException
	{

		private new const long SerialVersionUID = -5245986824925681401L;

		private int _errIdx = -1;

		/// <summary>
		/// Constructs a new <code>IllformedLocaleException</code> with no
		/// detail message and -1 as the error index.
		/// </summary>
		public IllformedLocaleException() : base()
		{
		}

		/// <summary>
		/// Constructs a new <code>IllformedLocaleException</code> with the
		/// given message and -1 as the error index.
		/// </summary>
		/// <param name="message"> the message </param>
		public IllformedLocaleException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new <code>IllformedLocaleException</code> with the
		/// given message and the error index.  The error index is the approximate
		/// offset from the start of the ill-formed value to the point where the
		/// parse first detected an error.  A negative error index value indicates
		/// either the error index is not applicable or unknown.
		/// </summary>
		/// <param name="message"> the message </param>
		/// <param name="errorIndex"> the index </param>
		public IllformedLocaleException(String message, int errorIndex) : base(message + ((errorIndex < 0) ? "" : " [at index " + errorIndex + "]"))
		{
			_errIdx = errorIndex;
		}

		/// <summary>
		/// Returns the index where the error was found. A negative value indicates
		/// either the error index is not applicable or unknown.
		/// </summary>
		/// <returns> the error index </returns>
		public virtual int ErrorIndex
		{
			get
			{
				return _errIdx;
			}
		}
	}

}