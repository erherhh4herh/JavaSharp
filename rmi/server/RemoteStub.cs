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
	/// The {@code RemoteStub} class is the common superclass of
	/// statically generated client
	/// stubs and provides the framework to support a wide range of remote
	/// reference semantics.  Stub objects are surrogates that support
	/// exactly the same set of remote interfaces defined by the actual
	/// implementation of the remote object.
	/// 
	/// @author  Ann Wollrath
	/// @since   JDK1.1
	/// </summary>
	/// @deprecated Statically generated stubs are deprecated, since
	/// stubs are generated dynamically. See <seealso cref="UnicastRemoteObject"/>
	/// for information about dynamic stub generation. 
	[Obsolete("Statically generated stubs are deprecated, since")]
	public abstract class RemoteStub : RemoteObject
	{

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = -1585587260594494182L;

		/// <summary>
		/// Constructs a {@code RemoteStub}.
		/// </summary>
		protected internal RemoteStub() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code RemoteStub} with the specified remote
		/// reference.
		/// </summary>
		/// <param name="ref"> the remote reference
		/// @since JDK1.1 </param>
		protected internal RemoteStub(RemoteRef @ref) : base(@ref)
		{
		}

		/// <summary>
		/// Throws <seealso cref="UnsupportedOperationException"/>.
		/// </summary>
		/// <param name="stub"> the remote stub </param>
		/// <param name="ref"> the remote reference </param>
		/// <exception cref="UnsupportedOperationException"> always
		/// @since JDK1.1 </exception>
		/// @deprecated No replacement.  The {@code setRef} method
		/// was intended for setting the remote reference of a remote
		/// stub. This is unnecessary, since {@code RemoteStub}s can be created
		/// and initialized with a remote reference through use of
		/// the <seealso cref="#RemoteStub(RemoteRef)"/> constructor. 
		[Obsolete("No replacement.  The {@code setRef} method")]
		protected internal static void SetRef(RemoteStub stub, RemoteRef @ref)
		{
			throw new UnsupportedOperationException();
		}
	}

}