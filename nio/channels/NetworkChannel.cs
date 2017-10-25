/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{


	/// <summary>
	/// A channel to a network socket.
	/// 
	/// <para> A channel that implements this interface is a channel to a network
	/// socket. The <seealso cref="#bind(SocketAddress) bind"/> method is used to bind the
	/// socket to a local <seealso cref="SocketAddress address"/>, the {@link #getLocalAddress()
	/// getLocalAddress} method returns the address that the socket is bound to, and
	/// the <seealso cref="#setOption(SocketOption,Object) setOption"/> and {@link
	/// #getOption(SocketOption) getOption} methods are used to set and query socket
	/// options.  An implementation of this interface should specify the socket options
	/// that it supports.
	/// 
	/// </para>
	/// <para> The <seealso cref="#bind bind"/> and <seealso cref="#setOption setOption"/> methods that do
	/// not otherwise have a value to return are specified to return the network
	/// channel upon which they are invoked. This allows method invocations to be
	/// chained. Implementations of this interface should specialize the return type
	/// so that method invocations on the implementation class can be chained.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface NetworkChannel : Channel
	{
		/// <summary>
		/// Binds the channel's socket to a local address.
		/// 
		/// <para> This method is used to establish an association between the socket and
		/// a local address. Once an association is established then the socket remains
		/// bound until the channel is closed. If the {@code local} parameter has the
		/// value {@code null} then the socket will be bound to an address that is
		/// assigned automatically.
		/// 
		/// </para>
		/// </summary>
		/// <param name="local">
		///          The address to bind the socket, or {@code null} to bind the socket
		///          to an automatically assigned socket address
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="AlreadyBoundException">
		///          If the socket is already bound </exception>
		/// <exception cref="UnsupportedAddressTypeException">
		///          If the type of the given address is not supported </exception>
		/// <exception cref="ClosedChannelException">
		///          If the channel is closed </exception>
		/// <exception cref="IOException">
		///          If some other I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          If a security manager is installed and it denies an unspecified
		///          permission. An implementation of this interface should specify
		///          any required permissions.
		/// </exception>
		/// <seealso cref= #getLocalAddress </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: NetworkChannel bind(java.net.SocketAddress local) throws java.io.IOException;
		NetworkChannel Bind(SocketAddress local);

		/// <summary>
		/// Returns the socket address that this channel's socket is bound to.
		/// 
		/// <para> Where the channel is <seealso cref="#bind bound"/> to an Internet Protocol
		/// socket address then the return value from this method is of type {@link
		/// java.net.InetSocketAddress}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The socket address that the socket is bound to, or {@code null}
		///          if the channel's socket is not bound
		/// </returns>
		/// <exception cref="ClosedChannelException">
		///          If the channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.net.SocketAddress getLocalAddress() throws java.io.IOException;
		SocketAddress LocalAddress {get;}

		/// <summary>
		/// Sets the value of a socket option.
		/// </summary>
		/// @param   <T>
		///          The type of the socket option value </param>
		/// <param name="name">
		///          The socket option </param>
		/// <param name="value">
		///          The value of the socket option. A value of {@code null} may be
		///          a valid value for some socket options.
		/// </param>
		/// <returns>  This channel
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If the socket option is not supported by this channel </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the value is not a valid value for this socket option </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <seealso cref= java.net.StandardSocketOptions </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> NetworkChannel setOption(java.net.SocketOption<T> name, T value) throws java.io.IOException;
		NetworkChannel setOption<T>(SocketOption<T> name, T value);

		/// <summary>
		/// Returns the value of a socket option.
		/// </summary>
		/// @param   <T>
		///          The type of the socket option value </param>
		/// <param name="name">
		///          The socket option
		/// </param>
		/// <returns>  The value of the socket option. A value of {@code null} may be
		///          a valid value for some socket options.
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If the socket option is not supported by this channel </exception>
		/// <exception cref="ClosedChannelException">
		///          If this channel is closed </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <seealso cref= java.net.StandardSocketOptions </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> T getOption(java.net.SocketOption<T> name) throws java.io.IOException;
		T getOption<T>(SocketOption<T> name);

		/// <summary>
		/// Returns a set of the socket options supported by this channel.
		/// 
		/// <para> This method will continue to return the set of options even after the
		/// channel has been closed.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A set of the socket options supported by this channel </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Set<java.net.SocketOption<?>> supportedOptions();
		Set<SocketOption<?>> SupportedOptions();
	}

}