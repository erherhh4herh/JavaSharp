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
	/// Thrown to indicate that a thread has attempted to wait on an
	/// object's monitor or to notify other threads waiting on an object's
	/// monitor without owning the specified monitor.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.lang.Object#notify() </seealso>
	/// <seealso cref=     java.lang.Object#notifyAll() </seealso>
	/// <seealso cref=     java.lang.Object#wait() </seealso>
	/// <seealso cref=     java.lang.Object#wait(long) </seealso>
	/// <seealso cref=     java.lang.Object#wait(long, int)
	/// @since   JDK1.0 </seealso>
	public class IllegalMonitorStateException : RuntimeException
	{
		private new const long SerialVersionUID = 3713306369498869069L;

		/// <summary>
		/// Constructs an <code>IllegalMonitorStateException</code> with no
		/// detail message.
		/// </summary>
		public IllegalMonitorStateException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalMonitorStateException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IllegalMonitorStateException(String s) : base(s)
		{
		}
	}

}