using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi
{

	/// <summary>
	/// {@code RMISecurityManager} implements a policy identical to the policy
	/// implemented by <seealso cref="SecurityManager"/>. RMI applications
	/// should use the {@code SecurityManager} class or another appropriate
	/// {@code SecurityManager} implementation instead of this class. RMI's class
	/// loader will download classes from remote locations only if a security
	/// manager has been set.
	/// 
	/// @implNote
	/// <para>Applets typically run in a container that already has a security
	/// manager, so there is generally no need for applets to set a security
	/// manager. If you have a standalone application, you might need to set a
	/// {@code SecurityManager} in order to enable class downloading. This can be
	/// done by adding the following to your code. (It needs to be executed before
	/// RMI can download code from remote hosts, so it most likely needs to appear
	/// in the {@code main} method of your application.)
	/// 
	/// <pre>{@code
	///    if (System.getSecurityManager() == null) {
	///        System.setSecurityManager(new SecurityManager());
	///    }
	/// }</pre>
	/// 
	/// @author  Roger Riggs
	/// @author  Peter Jones
	/// @since JDK1.1
	/// </para>
	/// </summary>
	/// @deprecated Use <seealso cref="SecurityManager"/> instead. 
	[Obsolete("Use <seealso cref="SecurityManager"/> instead.")]
	public class RMISecurityManager : SecurityManager
	{

		/// <summary>
		/// Constructs a new {@code RMISecurityManager}.
		/// @since JDK1.1
		/// </summary>
		public RMISecurityManager()
		{
		}
	}

}