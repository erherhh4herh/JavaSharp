/*
 * Copyright (c) 1996, 2002, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi.server
{

	using UnicastServerRef = sun.rmi.server.UnicastServerRef;
	using Log = sun.rmi.runtime.Log;

	/// <summary>
	/// The <code>RemoteServer</code> class is the common superclass to server
	/// implementations and provides the framework to support a wide range
	/// of remote reference semantics.  Specifically, the functions needed
	/// to create and export remote objects (i.e. to make them remotely
	/// available) are provided abstractly by <code>RemoteServer</code> and
	/// concretely by its subclass(es).
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </summary>
	public abstract class RemoteServer : RemoteObject
	{
		/* indicate compatibility with JDK 1.1.x version of class */
		private const long SerialVersionUID = -4100238210092549637L;

		/// <summary>
		/// Constructs a <code>RemoteServer</code>.
		/// @since JDK1.1
		/// </summary>
		protected internal RemoteServer() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>RemoteServer</code> with the given reference type.
		/// </summary>
		/// <param name="ref"> the remote reference
		/// @since JDK1.1 </param>
		protected internal RemoteServer(RemoteRef @ref) : base(@ref)
		{
		}

		/// <summary>
		/// Returns a string representation of the client host for the
		/// remote method invocation being processed in the current thread.
		/// </summary>
		/// <returns>  a string representation of the client host
		/// </returns>
		/// <exception cref="ServerNotActiveException"> if no remote method invocation
		/// is being processed in the current thread
		/// 
		/// @since   JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String getClientHost() throws ServerNotActiveException
		public static String ClientHost
		{
			get
			{
				return sun.rmi.transport.tcp.TCPTransport.ClientHost;
			}
		}

		/// <summary>
		/// Log RMI calls to the output stream <code>out</code>. If
		/// <code>out</code> is <code>null</code>, call logging is turned off.
		/// 
		/// <para>If there is a security manager, its
		/// <code>checkPermission</code> method will be invoked with a
		/// <code>java.util.logging.LoggingPermission("control")</code>
		/// permission; this could result in a <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream to which RMI calls should be logged </param>
		/// <exception cref="SecurityException">  if there is a security manager and
		///          the invocation of its <code>checkPermission</code> method
		///          fails </exception>
		/// <seealso cref= #getLog
		/// @since JDK1.1 </seealso>
		public static void SetLog(java.io.OutputStream @out)
		{
			LogNull = (@out == null);
			UnicastServerRef.callLog.OutputStream = @out;
		}

		/// <summary>
		/// Returns stream for the RMI call log. </summary>
		/// <returns> the call log </returns>
		/// <seealso cref= #setLog
		/// @since JDK1.1 </seealso>
		public static java.io.PrintStream GetLog()
		{
			return (LogNull ? null : UnicastServerRef.callLog.PrintStream);
		}

		// initialize log status
		private static bool LogNull = !UnicastServerRef.logCalls;
	}

}