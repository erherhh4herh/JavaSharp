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

namespace java.rmi.server
{

	/// <summary>
	/// A ServerRef represents the server-side handle for a remote object
	/// implementation.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1 </summary>
	/// @deprecated No replacement. This interface is unused and is obsolete. 
	[Obsolete("No replacement. This interface is unused and is obsolete.")]
	public interface ServerRef : RemoteRef
	{

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class. </summary>

		/// <summary>
		/// Creates a client stub object for the supplied Remote object.
		/// If the call completes successfully, the remote object should
		/// be able to accept incoming calls from clients. </summary>
		/// <param name="obj"> the remote object implementation </param>
		/// <param name="data"> information necessary to export the object </param>
		/// <returns> the stub for the remote object </returns>
		/// <exception cref="RemoteException"> if an exception occurs attempting
		/// to export the object (e.g., stub class could not be found)
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RemoteStub exportObject(Remote obj, Object data) throws RemoteException;
		RemoteStub ExportObject(Remote obj, Object data);

		/// <summary>
		/// Returns the hostname of the current client.  When called from a
		/// thread actively handling a remote method invocation the
		/// hostname of the client is returned. </summary>
		/// <returns> the client's host name </returns>
		/// <exception cref="ServerNotActiveException"> if called outside of servicing
		/// a remote method invocation
		/// @since JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getClientHost() throws ServerNotActiveException;
		String ClientHost {get;}
	}

	public static class ServerRef_Fields
	{
		public const long SerialVersionUID = -4557750989390278438L;
	}

}