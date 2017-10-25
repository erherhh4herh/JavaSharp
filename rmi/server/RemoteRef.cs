using System;

/*
 * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// <code>RemoteRef</code> represents the handle for a remote object. A
	/// <code>RemoteStub</code> uses a remote reference to carry out a
	/// remote method invocation to a remote object.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1 </summary>
	/// <seealso cref=     java.rmi.server.RemoteStub </seealso>
	public interface RemoteRef : java.io.Externalizable
	{

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class. </summary>

		/// <summary>
		/// Initialize the server package prefix: assumes that the
		/// implementation of server ref classes (e.g., UnicastRef,
		/// UnicastServerRef) are located in the package defined by the
		/// prefix.
		/// </summary>

		/// <summary>
		/// Invoke a method. This form of delegating method invocation
		/// to the reference allows the reference to take care of
		/// setting up the connection to the remote host, marshaling
		/// some representation for the method and parameters, then
		/// communicating the method invocation to the remote host.
		/// This method either returns the result of a method invocation
		/// on the remote object which resides on the remote host or
		/// throws a RemoteException if the call failed or an
		/// application-level exception if the remote invocation throws
		/// an exception.
		/// </summary>
		/// <param name="obj"> the object that contains the RemoteRef (e.g., the
		///            RemoteStub for the object. </param>
		/// <param name="method"> the method to be invoked </param>
		/// <param name="params"> the parameter list </param>
		/// <param name="opnum">  a hash that may be used to represent the method </param>
		/// <returns> result of remote method invocation </returns>
		/// <exception cref="Exception"> if any exception occurs during remote method
		/// invocation
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object invoke(Remote obj, java.lang.reflect.Method method, Object[] params, long opnum) throws Exception;
		Object Invoke(Remote obj, System.Reflection.MethodInfo method, Object[] @params, long opnum);

		/// <summary>
		/// Creates an appropriate call object for a new remote method
		/// invocation on this object.  Passing operation array and index,
		/// allows the stubs generator to assign the operation indexes and
		/// interpret them. The remote reference may need the operation to
		/// encode in the call.
		/// 
		/// @since JDK1.1 </summary>
		/// @deprecated 1.2 style stubs no longer use this method. Instead of
		/// using a sequence of method calls on the stub's the remote reference
		/// (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		/// stub uses a single method, <code>invoke(Remote, Method, Object[],
		/// int)</code>, on the remote reference to carry out parameter
		/// marshalling, remote method executing and unmarshalling of the return
		/// value.
		/// 
		/// <param name="obj"> remote stub through which to make call </param>
		/// <param name="op"> array of stub operations </param>
		/// <param name="opnum"> operation number </param>
		/// <param name="hash"> stub/skeleton interface hash </param>
		/// <returns> call object representing remote call </returns>
		/// <exception cref="RemoteException"> if failed to initiate new remote call </exception>
		/// <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("1.2 style stubs no longer use this method. Instead of") RemoteCall newCall(RemoteObject obj, Operation[] op, int opnum, long hash) throws RemoteException;
		[Obsolete("1.2 style stubs no longer use this method. Instead of")]
		RemoteCall NewCall(RemoteObject obj, Operation[] op, int opnum, long hash);

		/// <summary>
		/// Executes the remote call.
		/// 
		/// Invoke will raise any "user" exceptions which
		/// should pass through and not be caught by the stub.  If any
		/// exception is raised during the remote invocation, invoke should
		/// take care of cleaning up the connection before raising the
		/// "user" or remote exception.
		/// 
		/// @since JDK1.1 </summary>
		/// @deprecated 1.2 style stubs no longer use this method. Instead of
		/// using a sequence of method calls to the remote reference
		/// (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		/// stub uses a single method, <code>invoke(Remote, Method, Object[],
		/// int)</code>, on the remote reference to carry out parameter
		/// marshalling, remote method executing and unmarshalling of the return
		/// value.
		/// 
		/// <param name="call"> object representing remote call </param>
		/// <exception cref="Exception"> if any exception occurs during remote method </exception>
		/// <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("1.2 style stubs no longer use this method. Instead of") void invoke(RemoteCall call) throws Exception;
		[Obsolete("1.2 style stubs no longer use this method. Instead of")]
		void Invoke(RemoteCall call);

		/// <summary>
		/// Allows the remote reference to clean up (or reuse) the connection.
		/// Done should only be called if the invoke returns successfully
		/// (non-exceptionally) to the stub.
		/// 
		/// @since JDK1.1 </summary>
		/// @deprecated 1.2 style stubs no longer use this method. Instead of
		/// using a sequence of method calls to the remote reference
		/// (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		/// stub uses a single method, <code>invoke(Remote, Method, Object[],
		/// int)</code>, on the remote reference to carry out parameter
		/// marshalling, remote method executing and unmarshalling of the return
		/// value.
		/// 
		/// <param name="call"> object representing remote call </param>
		/// <exception cref="RemoteException"> if remote error occurs during call cleanup </exception>
		/// <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("1.2 style stubs no longer use this method. Instead of") void done(RemoteCall call) throws RemoteException;
		[Obsolete("1.2 style stubs no longer use this method. Instead of")]
		void Done(RemoteCall call);

		/// <summary>
		/// Returns the class name of the ref type to be serialized onto
		/// the stream 'out'. </summary>
		/// <param name="out"> the output stream to which the reference will be serialized </param>
		/// <returns> the class name (without package qualification) of the reference
		/// type
		/// @since JDK1.1 </returns>
		String GetRefClass(java.io.ObjectOutput @out);

		/// <summary>
		/// Returns a hashcode for a remote object.  Two remote object stubs
		/// that refer to the same remote object will have the same hash code
		/// (in order to support remote objects as keys in hash tables).
		/// </summary>
		/// <returns> remote object hashcode </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since JDK1.1 </seealso>
		int RemoteHashCode();

		/// <summary>
		/// Compares two remote objects for equality.
		/// Returns a boolean that indicates whether this remote object is
		/// equivalent to the specified Object. This method is used when a
		/// remote object is stored in a hashtable. </summary>
		/// <param name="obj">     the Object to compare with </param>
		/// <returns>  true if these Objects are equal; false otherwise. </returns>
		/// <seealso cref=             java.util.Hashtable
		/// @since JDK1.1 </seealso>
		bool RemoteEquals(RemoteRef obj);

		/// <summary>
		/// Returns a String that represents the reference of this remote
		/// object. </summary>
		/// <returns> string representing remote object reference
		/// @since JDK1.1 </returns>
		String RemoteToString();

	}

	public static class RemoteRef_Fields
	{
		public const long SerialVersionUID = 3632638527362204081L;
		public const String PackagePrefix = "sun.rmi.server";
	}

}