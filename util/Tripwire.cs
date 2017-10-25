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
namespace java.util
{

	using PlatformLogger = sun.util.logging.PlatformLogger;


	/// <summary>
	/// Utility class for detecting inadvertent uses of boxing in
	/// {@code java.util} classes.  The detection is turned on or off based on
	/// whether the system property {@code org.openjdk.java.util.stream.tripwire} is
	/// considered {@code true} according to <seealso cref="Boolean#getBoolean(String)"/>.
	/// This should normally be turned off for production use.
	/// 
	/// @apiNote
	/// Typical usage would be for boxing code to do:
	/// <pre>{@code
	///     if (Tripwire.ENABLED)
	///         Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfInt.nextInt()");
	/// }</pre>
	/// 
	/// @since 1.8
	/// </summary>
	internal sealed class Tripwire
	{
		private const String TRIPWIRE_PROPERTY = "org.openjdk.java.util.stream.tripwire";

		/// <summary>
		/// Should debugging checks be enabled? </summary>
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
		internal static readonly bool ENABLED = AccessController.doPrivileged((PrivilegedAction<Boolean>)() => Boolean.GetBoolean(TRIPWIRE_PROPERTY));

		private Tripwire()
		{
		}

		/// <summary>
		/// Produces a log warning, using {@code PlatformLogger.getLogger(className)},
		/// using the supplied message.  The class name of {@code trippingClass} will
		/// be used as the first parameter to the message.
		/// </summary>
		/// <param name="trippingClass"> Name of the class generating the message </param>
		/// <param name="msg"> A message format string of the type expected by
		/// <seealso cref="PlatformLogger"/> </param>
		internal static void Trip(Class trippingClass, String msg)
		{
			PlatformLogger.getLogger(trippingClass.Name).warning(msg, trippingClass.Name);
		}
	}

}