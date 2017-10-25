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

namespace java.net
{


	/// <summary>
	/// Abstract datagram and multicast socket implementation base class.
	/// @author Pavani Diwanji
	/// @since  JDK1.1
	/// </summary>

	public abstract class DatagramSocketImpl : SocketOptions
	{
		public abstract Object GetOption(int optID);
		public abstract void SetOption(int optID, object value);

		/// <summary>
		/// The local port number.
		/// </summary>
		protected internal int LocalPort_Renamed;

		/// <summary>
		/// The file descriptor object.
		/// </summary>
		protected internal FileDescriptor Fd;

		internal virtual int DataAvailable()
		{
			// default impl returns zero, which disables the calling
			// functionality
			return 0;
		}

		/// <summary>
		/// The DatagramSocket or MulticastSocket
		/// that owns this impl
		/// </summary>
		internal DatagramSocket Socket;

		internal virtual DatagramSocket DatagramSocket
		{
			set
			{
				this.Socket = value;
			}
			get
			{
				return Socket;
			}
		}


		/// <summary>
		/// Creates a datagram socket. </summary>
		/// <exception cref="SocketException"> if there is an error in the
		/// underlying protocol, such as a TCP error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void create() throws SocketException;
		protected internal abstract void Create();

		/// <summary>
		/// Binds a datagram socket to a local port and address. </summary>
		/// <param name="lport"> the local port </param>
		/// <param name="laddr"> the local address </param>
		/// <exception cref="SocketException"> if there is an error in the
		/// underlying protocol, such as a TCP error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void bind(int lport, InetAddress laddr) throws SocketException;
		protected internal abstract void Bind(int lport, InetAddress laddr);

		/// <summary>
		/// Sends a datagram packet. The packet contains the data and the
		/// destination address to send the packet to. </summary>
		/// <param name="p"> the packet to be sent. </param>
		/// <exception cref="IOException"> if an I/O exception occurs while sending the
		/// datagram packet. </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		/// to a currently unreachable destination. Note, there is no guarantee that
		/// the exception will be thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void send(DatagramPacket p) throws java.io.IOException;
		protected internal abstract void Send(DatagramPacket p);

		/// <summary>
		/// Connects a datagram socket to a remote destination. This associates the remote
		/// address with the local socket so that datagrams may only be sent to this destination
		/// and received from this destination. This may be overridden to call a native
		/// system connect.
		/// 
		/// <para>If the remote destination to which the socket is connected does not
		/// exist, or is otherwise unreachable, and if an ICMP destination unreachable
		/// packet has been received for that address, then a subsequent call to
		/// send or receive may throw a PortUnreachableException.
		/// Note, there is no guarantee that the exception will be thrown.
		/// </para>
		/// </summary>
		/// <param name="address"> the remote InetAddress to connect to </param>
		/// <param name="port"> the remote port number </param>
		/// <exception cref="SocketException"> may be thrown if the socket cannot be
		/// connected to the remote destination
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void connect(InetAddress address, int port) throws SocketException
		protected internal virtual void Connect(InetAddress address, int port)
		{
		}

		/// <summary>
		/// Disconnects a datagram socket from its remote destination.
		/// @since 1.4
		/// </summary>
		protected internal virtual void Disconnect()
		{
		}

		/// <summary>
		/// Peek at the packet to see who it is from. Updates the specified {@code InetAddress}
		/// to the address which the packet came from. </summary>
		/// <param name="i"> an InetAddress object </param>
		/// <returns> the port number which the packet came from. </returns>
		/// <exception cref="IOException"> if an I/O exception occurs </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		///       to a currently unreachable destination. Note, there is no guarantee that the
		///       exception will be thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int peek(InetAddress i) throws java.io.IOException;
		protected internal abstract int Peek(InetAddress i);

		/// <summary>
		/// Peek at the packet to see who it is from. The data is copied into the specified
		/// {@code DatagramPacket}. The data is returned,
		/// but not consumed, so that a subsequent peekData/receive operation
		/// will see the same data. </summary>
		/// <param name="p"> the Packet Received. </param>
		/// <returns> the port number which the packet came from. </returns>
		/// <exception cref="IOException"> if an I/O exception occurs </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		///       to a currently unreachable destination. Note, there is no guarantee that the
		///       exception will be thrown.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int peekData(DatagramPacket p) throws java.io.IOException;
		protected internal abstract int PeekData(DatagramPacket p);
		/// <summary>
		/// Receive the datagram packet. </summary>
		/// <param name="p"> the Packet Received. </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while receiving the datagram packet. </exception>
		/// <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		///       to a currently unreachable destination. Note, there is no guarantee that the
		///       exception will be thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void receive(DatagramPacket p) throws java.io.IOException;
		protected internal abstract void Receive(DatagramPacket p);

		/// <summary>
		/// Set the TTL (time-to-live) option. </summary>
		/// <param name="ttl"> a byte specifying the TTL value
		/// </param>
		/// @deprecated use setTimeToLive instead. 
		/// <exception cref="IOException"> if an I/O exception occurs while setting
		/// the time-to-live option. </exception>
		/// <seealso cref= #getTTL() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use setTimeToLive instead.") protected abstract void setTTL(byte ttl) throws java.io.IOException;
		[Obsolete("use setTimeToLive instead.")]
		protected internal abstract sbyte TTL {set;get;}

		/// <summary>
		/// Retrieve the TTL (time-to-live) option.
		/// </summary>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while retrieving the time-to-live option </exception>
		/// @deprecated use getTimeToLive instead. 
		/// <returns> a byte representing the TTL value </returns>
		/// <seealso cref= #setTTL(byte) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use getTimeToLive instead.") protected abstract byte getTTL() throws java.io.IOException;

		/// <summary>
		/// Set the TTL (time-to-live) option. </summary>
		/// <param name="ttl"> an {@code int} specifying the time-to-live value </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while setting the time-to-live option. </exception>
		/// <seealso cref= #getTimeToLive() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void setTimeToLive(int ttl) throws java.io.IOException;
		protected internal abstract int TimeToLive {set;get;}

		/// <summary>
		/// Retrieve the TTL (time-to-live) option. </summary>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while retrieving the time-to-live option </exception>
		/// <returns> an {@code int} representing the time-to-live value </returns>
		/// <seealso cref= #setTimeToLive(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int getTimeToLive() throws java.io.IOException;

		/// <summary>
		/// Join the multicast group. </summary>
		/// <param name="inetaddr"> multicast address to join. </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while joining the multicast group. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void join(InetAddress inetaddr) throws java.io.IOException;
		protected internal abstract void Join(InetAddress inetaddr);

		/// <summary>
		/// Leave the multicast group. </summary>
		/// <param name="inetaddr"> multicast address to leave. </param>
		/// <exception cref="IOException"> if an I/O exception occurs
		/// while leaving the multicast group. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void leave(InetAddress inetaddr) throws java.io.IOException;
		protected internal abstract void Leave(InetAddress inetaddr);

		/// <summary>
		/// Join the multicast group. </summary>
		/// <param name="mcastaddr"> address to join. </param>
		/// <param name="netIf"> specifies the local interface to receive multicast
		///        datagram packets </param>
		/// <exception cref="IOException"> if an I/O exception occurs while joining
		/// the multicast group
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void joinGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws java.io.IOException;
		protected internal abstract void JoinGroup(SocketAddress mcastaddr, NetworkInterface netIf);

		/// <summary>
		/// Leave the multicast group. </summary>
		/// <param name="mcastaddr"> address to leave. </param>
		/// <param name="netIf"> specified the local interface to leave the group at </param>
		/// <exception cref="IOException"> if an I/O exception occurs while leaving
		/// the multicast group
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void leaveGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws java.io.IOException;
		protected internal abstract void LeaveGroup(SocketAddress mcastaddr, NetworkInterface netIf);

		/// <summary>
		/// Close the socket.
		/// </summary>
		protected internal abstract void Close();

		/// <summary>
		/// Gets the local port. </summary>
		/// <returns> an {@code int} representing the local port value </returns>
		protected internal virtual int LocalPort
		{
			get
			{
				return LocalPort_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> void setOption(SocketOption<T> name, T value) throws java.io.IOException
		internal virtual void setOption<T>(SocketOption<T> name, T value)
		{
			if (name == StandardSocketOptions.SO_SNDBUF)
			{
				SetOption(SocketOptions_Fields.SO_SNDBUF, value);
			}
			else if (name == StandardSocketOptions.SO_RCVBUF)
			{
				SetOption(SocketOptions_Fields.SO_RCVBUF, value);
			}
			else if (name == StandardSocketOptions.SO_REUSEADDR)
			{
				SetOption(SocketOptions_Fields.SO_REUSEADDR, value);
			}
			else if (name == StandardSocketOptions.IP_TOS)
			{
				SetOption(SocketOptions_Fields.IP_TOS, value);
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_IF && (DatagramSocket is MulticastSocket))
			{
				SetOption(SocketOptions_Fields.IP_MULTICAST_IF2, value);
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_TTL && (DatagramSocket is MulticastSocket))
			{
				if (!(value is Integer))
				{
					throw new IllegalArgumentException("not an integer");
				}
				TimeToLive = (Integer)value;
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_LOOP && (DatagramSocket is MulticastSocket))
			{
				SetOption(SocketOptions_Fields.IP_MULTICAST_LOOP, value);
			}
			else
			{
				throw new UnsupportedOperationException("unsupported option");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> T getOption(SocketOption<T> name) throws java.io.IOException
		internal virtual T getOption<T>(SocketOption<T> name)
		{
			if (name == StandardSocketOptions.SO_SNDBUF)
			{
				return (T) GetOption(SocketOptions_Fields.SO_SNDBUF);
			}
			else if (name == StandardSocketOptions.SO_RCVBUF)
			{
				return (T) GetOption(SocketOptions_Fields.SO_RCVBUF);
			}
			else if (name == StandardSocketOptions.SO_REUSEADDR)
			{
				return (T) GetOption(SocketOptions_Fields.SO_REUSEADDR);
			}
			else if (name == StandardSocketOptions.IP_TOS)
			{
				return (T) GetOption(SocketOptions_Fields.IP_TOS);
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_IF && (DatagramSocket is MulticastSocket))
			{
				return (T) GetOption(SocketOptions_Fields.IP_MULTICAST_IF2);
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_TTL && (DatagramSocket is MulticastSocket))
			{
				Integer ttl = TimeToLive;
				return (T)ttl;
			}
			else if (name == StandardSocketOptions.IP_MULTICAST_LOOP && (DatagramSocket is MulticastSocket))
			{
				return (T) GetOption(SocketOptions_Fields.IP_MULTICAST_LOOP);
			}
			else
			{
				throw new UnsupportedOperationException("unsupported option");
			}
		}

		/// <summary>
		/// Gets the datagram socket file descriptor. </summary>
		/// <returns> a {@code FileDescriptor} object representing the datagram socket
		/// file descriptor </returns>
		protected internal virtual FileDescriptor FileDescriptor
		{
			get
			{
				return Fd;
			}
		}
	}

}