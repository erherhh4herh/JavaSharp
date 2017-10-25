/*
 * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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

	using Signal = sun.misc.Signal;
	using SignalHandler = sun.misc.SignalHandler;


	/// <summary>
	/// Package-private utility class for setting up and tearing down
	/// platform-specific support for termination-triggered shutdowns.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.3
	/// </summary>

	internal class Terminator
	{

		private static SignalHandler Handler = null;

		/* Invocations of setup and teardown are already synchronized
		 * on the shutdown lock, so no further synchronization is needed here
		 */

		internal static void Setup()
		{
			if (Handler != null)
			{
				return;
			}
			SignalHandler sh = new SignalHandlerAnonymousInnerClassHelper();
			Handler = sh;

			// When -Xrs is specified the user is responsible for
			// ensuring that shutdown hooks are run by calling
			// System.exit()
			try
			{
				Signal.handle(new Signal("INT"), sh);
			}
			catch (IllegalArgumentException)
			{
			}
			try
			{
				Signal.handle(new Signal("TERM"), sh);
			}
			catch (IllegalArgumentException)
			{
			}
		}

		private class SignalHandlerAnonymousInnerClassHelper : SignalHandler
		{
			public SignalHandlerAnonymousInnerClassHelper()
			{
			}

			public virtual void Handle(Signal sig)
			{
				Shutdown.Exit(sig.Number + 0x80);
			}
		}

		internal static void Teardown()
		{
			/* The current sun.misc.Signal class does not support
			 * the cancellation of handlers
			 */
		}

	}

}