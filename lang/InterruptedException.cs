using System;

/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when a thread is waiting, sleeping, or otherwise occupied,
	/// and the thread is interrupted, either before or during the activity.
	/// Occasionally a method may wish to test whether the current
	/// thread has been interrupted, and if so, to immediately throw
	/// this exception.  The following code can be used to achieve
	/// this effect:
	/// <pre>
	///  if (Thread.interrupted())  // Clears interrupted status!
	///      throw new InterruptedException();
	/// </pre>
	/// 
	/// @author  Frank Yellin </summary>
	/// <seealso cref=     java.lang.Object#wait() </seealso>
	/// <seealso cref=     java.lang.Object#wait(long) </seealso>
	/// <seealso cref=     java.lang.Object#wait(long, int) </seealso>
	/// <seealso cref=     java.lang.Thread#sleep(long) </seealso>
	/// <seealso cref=     java.lang.Thread#interrupt() </seealso>
	/// <seealso cref=     java.lang.Thread#interrupted()
	/// @since   JDK1.0 </seealso>
	public class InterruptedException : Exception
	{
		private new const long SerialVersionUID = 6700697376100628473L;

		/// <summary>
		/// Constructs an <code>InterruptedException</code> with no detail  message.
		/// </summary>
		public InterruptedException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>InterruptedException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public InterruptedException(String s) : base(s)
		{
		}
	}

}