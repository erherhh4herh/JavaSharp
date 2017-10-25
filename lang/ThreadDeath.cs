﻿/*
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
	/// An instance of {@code ThreadDeath} is thrown in the victim thread
	/// when the (deprecated) <seealso cref="Thread#stop()"/> method is invoked.
	/// 
	/// <para>An application should catch instances of this class only if it
	/// must clean up after being terminated asynchronously.  If
	/// {@code ThreadDeath} is caught by a method, it is important that it
	/// be rethrown so that the thread actually dies.
	/// 
	/// </para>
	/// <para>The {@link ThreadGroup#uncaughtException top-level error
	/// handler} does not print out a message if {@code ThreadDeath} is
	/// never caught.
	/// 
	/// </para>
	/// <para>The class {@code ThreadDeath} is specifically a subclass of
	/// {@code Error} rather than {@code Exception}, even though it is a
	/// "normal occurrence", because many applications catch all
	/// occurrences of {@code Exception} and then discard the exception.
	/// 
	/// @since   JDK1.0
	/// </para>
	/// </summary>

	public class ThreadDeath : Error
	{
		private new const long SerialVersionUID = -4417128565033088268L;
	}

}