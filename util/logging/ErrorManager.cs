using System;

/*
 * Copyright (c) 2001, 2004, Oracle and/or its affiliates. All rights reserved.
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


namespace java.util.logging
{

	/// <summary>
	/// ErrorManager objects can be attached to Handlers to process
	/// any error that occurs on a Handler during Logging.
	/// <para>
	/// When processing logging output, if a Handler encounters problems
	/// then rather than throwing an Exception back to the issuer of
	/// the logging call (who is unlikely to be interested) the Handler
	/// should call its associated ErrorManager.
	/// </para>
	/// </summary>

	public class ErrorManager
	{
	   private bool Reported = false;

		/*
		 * We declare standard error codes for important categories of errors.
		 */

		/// <summary>
		/// GENERIC_FAILURE is used for failure that don't fit
		/// into one of the other categories.
		/// </summary>
		public const int GENERIC_FAILURE = 0;
		/// <summary>
		/// WRITE_FAILURE is used when a write to an output stream fails.
		/// </summary>
		public const int WRITE_FAILURE = 1;
		/// <summary>
		/// FLUSH_FAILURE is used when a flush to an output stream fails.
		/// </summary>
		public const int FLUSH_FAILURE = 2;
		/// <summary>
		/// CLOSE_FAILURE is used when a close of an output stream fails.
		/// </summary>
		public const int CLOSE_FAILURE = 3;
		/// <summary>
		/// OPEN_FAILURE is used when an open of an output stream fails.
		/// </summary>
		public const int OPEN_FAILURE = 4;
		/// <summary>
		/// FORMAT_FAILURE is used when formatting fails for any reason.
		/// </summary>
		public const int FORMAT_FAILURE = 5;

		/// <summary>
		/// The error method is called when a Handler failure occurs.
		/// <para>
		/// This method may be overridden in subclasses.  The default
		/// behavior in this base class is that the first call is
		/// reported to System.err, and subsequent calls are ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="msg">    a descriptive string (may be null) </param>
		/// <param name="ex">     an exception (may be null) </param>
		/// <param name="code">   an error code defined in ErrorManager </param>
		public virtual void Error(String msg, Exception ex, int code)
		{
			lock (this)
			{
				if (Reported)
				{
					// We only report the first error, to avoid clogging
					// the screen.
					return;
				}
				Reported = true;
				String text = "java.util.logging.ErrorManager: " + code;
				if (msg != null)
				{
					text = text + ": " + msg;
				}
				System.Console.Error.WriteLine(text);
				if (ex != null)
				{
					ex.PrintStackTrace();
				}
			}
		}
	}

}