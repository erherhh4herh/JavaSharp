/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The abstract class {@code SocketImpl} is a common superclass
	/// of all classes that actually implement sockets. It is used to
	/// create both client and server sockets.
	/// <para>
	/// A "plain" socket implements these methods exactly as
	/// described, without attempting to go through a firewall or proxy.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public abstract class SocketImpl : SocketOptions
	{
		public abstract Object GetOption(int optID);
		public abstract void SetOption(int optID, object value);
		/// <summary>
		/// The actual Socket object.
		/// </summary>
		internal Socket Socket_Renamed = null;
		internal ServerSocket ServerSocket_Renamed = null;

		/// <summary>
		/// The file descriptor object for this socket.
		/// </summary>
		protected internal FileDescriptor Fd;

		/// <summary>
		/// The IP address of the remote end of this socket.
		/// </summary>
		protected internal InetAddress Address;

		/// <summary>
		/// The port number on the remote host to which this socket is connected.
		/// </summary>
		protected internal int Port_Renamed;

		/// <summary>
		/// The local port number to which this socket is connected.
		/// </summary>
		protected internal int Localport;

		/// <summary>
		/// Creates either a stream or a datagram socket.
		/// </summary>
		/// <param name="stream">   if {@code true}, create a stream socket;
		///                      otherwise, create a datagram socket. </param>
		/// <exception cref="IOException">  if an I/O error occurs while creating the
		///               socket. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void create(boolean stream) throws java.io.IOException;
		protected internal abstract void Create(bool stream);

		/// <summary>
		/// Connects this socket to the specified port on the named host.
		/// </summary>
		/// <param name="host">   the name of the remote host. </param>
		/// <param name="port">   the port number. </param>
		/// <exception cref="IOException">  if an I/O error occurs when connecting to the
		///               remote host. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void connect(String host, int port) throws java.io.IOException;
		protected internal abstract void Connect(String host, int port);

		/// <summary>
		/// Connects this socket to the specified port number on the specified host.
		/// </summary>
		/// <param name="address">   the IP address of the remote host. </param>
		/// <param name="port">      the port number. </param>
		/// <exception cref="IOException">  if an I/O error occurs when attempting a
		///               connection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void connect(InetAddress address, int port) throws java.io.IOException;
		protected internal abstract void Connect(InetAddress address, int port);

		/// <summary>
		/// Connects this socket to the specified port number on the specified host.
		/// A timeout of zero is interpreted as an infinite timeout. The connection
		/// will then block until established or an error occurs.
		/// </summary>
		/// <param name="address">   the Socket address of the remote host. </param>
		/// <param name="timeout">  the timeout value, in milliseconds, or zero for no timeout. </param>
		/// <exception cref="IOException">  if an I/O error occurs when attempting a
		///               connection.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void connect(SocketAddress address, int timeout) throws java.io.IOException;
		protected internal abstract void Connect(SocketAddress address, int timeout);

		/// <summary>
		/// Binds this socket to the specified local IP address and port number.
		/// </summary>
		/// <param name="host">   an IP address that belongs to a local interface. </param>
		/// <param name="port">   the port number. </param>
		/// <exception cref="IOException">  if an I/O error occurs when binding this socket. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void bind(InetAddress host, int port) throws java.io.IOException;
		protected internal abstract void Bind(InetAddress host, int port);

		/// <summary>
		/// Sets the maximum queue length for incoming connection indications
		/// (a request to connect) to the {@code count} argument. If a
		/// connection indication arrives when the queue is full, the
		/// connection is refused.
		/// </summary>
		/// <param name="backlog">   the maximum length of the queue. </param>
		/// <exception cref="IOException">  if an I/O error occurs when creating the queue. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void listen(int backlog) throws java.io.IOException;
		protected internal abstract void Listen(int backlog);

		/// <summary>
		/// Accepts a connection.
		/// </summary>
		/// <param name="s">   the accepted connection. </param>
		/// <exception cref="IOException">  if an I/O error occurs when accepting the
		///               connection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void accept(SocketImpl s) throws java.io.IOException;
		protected internal abstract void Accept(SocketImpl s);

		/// <summary>
		/// Returns an input stream for this socket.
		/// </summary>
		/// <returns>     a stream for reading from this socket. </returns>
		/// <exception cref="IOException">  if an I/O error occurs when creating the
		///               input stream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract java.io.InputStream getInputStream() throws java.io.IOException;
		protected internal abstract InputStream InputStream {get;}

		/// <summary>
		/// Returns an output stream for this socket.
		/// </summary>
		/// <returns>     an output stream for writing to this socket. </returns>
		/// <exception cref="IOException">  if an I/O error occurs when creating the
		///               output stream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract java.io.OutputStream getOutputStream() throws java.io.IOException;
		protected internal abstract OutputStream OutputStream {get;}

		/// <summary>
		/// Returns the number of bytes that can be read from this socket
		/// without blocking.
		/// </summary>
		/// <returns>     the number of bytes that can be read from this socket
		///             without blocking. </returns>
		/// <exception cref="IOException">  if an I/O error occurs when determining the
		///               number of bytes available. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int available() throws java.io.IOException;
		protected internal abstract int Available();

		/// <summary>
		/// Closes this socket.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs when closing this socket. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void close() throws java.io.IOException;
		protected internal abstract void Close();

		/// <summary>
		/// Places the input stream for this socket at "end of stream".
		/// Any data sent to this socket is acknowledged and then
		/// silently discarded.
		/// 
		/// If you read from a socket input stream after invoking this method on the
		/// socket, the stream's {@code available} method will return 0, and its
		/// {@code read} methods will return {@code -1} (end of stream).
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs when shutting down this
		/// socket. </exception>
		/// <seealso cref= java.net.Socket#shutdownOutput() </seealso>
		/// <seealso cref= java.net.Socket#close() </seealso>
		/// <seealso cref= java.net.Socket#setSoLinger(boolean, int)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownInput() throws java.io.IOException
		protected internal virtual void ShutdownInput()
		{
		  throw new IOException("Method not implemented!");
		}

		/// <summary>
		/// Disables the output stream for this socket.
		/// For a TCP socket, any previously written data will be sent
		/// followed by TCP's normal connection termination sequence.
		/// 
		/// If you write to a socket output stream after invoking
		/// shutdownOutput() on the socket, the stream will throw
		/// an IOException.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs when shutting down this
		/// socket. </exception>
		/// <seealso cref= java.net.Socket#shutdownInput() </seealso>
		/// <seealso cref= java.net.Socket#close() </seealso>
		/// <seealso cref= java.net.Socket#setSoLinger(boolean, int)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void shutdownOutput() throws java.io.IOException
		protected internal virtual void ShutdownOutput()
		{
		  throw new IOException("Method not implemented!");
		}

		/// <summary>
		/// Returns the value of this socket's {@code fd} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code fd} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#fd </seealso>
		protected internal virtual FileDescriptor FileDescriptor
		{
			get
			{
				return Fd;
			}
		}

		/// <summary>
		/// Returns the value of this socket's {@code address} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code address} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#address </seealso>
		protected internal virtual InetAddress InetAddress
		{
			get
			{
				return Address;
			}
		}

		/// <summary>
		/// Returns the value of this socket's {@code port} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code port} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#port </seealso>
		protected internal virtual int Port
		{
			get
			{
				return Port_Renamed;
			}
		}

		/// <summary>
		/// Returns whether or not this SocketImpl supports sending
		/// urgent data. By default, false is returned
		/// unless the method is overridden in a sub-class
		/// </summary>
		/// <returns>  true if urgent data supported </returns>
		/// <seealso cref=     java.net.SocketImpl#address
		/// @since 1.4 </seealso>
		protected internal virtual bool SupportsUrgentData()
		{
			return false; // must be overridden in sub-class
		}

		/// <summary>
		/// Send one byte of urgent data on the socket.
		/// The byte to be sent is the low eight bits of the parameter </summary>
		/// <param name="data"> The byte of data to send </param>
		/// <exception cref="IOException"> if there is an error
		///  sending the data.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void sendUrgentData(int data) throws java.io.IOException;
		protected internal abstract void SendUrgentData(int data);

		/// <summary>
		/// Returns the value of this socket's {@code localport} field.
		/// </summary>
		/// <returns>  the value of this socket's {@code localport} field. </returns>
		/// <seealso cref=     java.net.SocketImpl#localport </seealso>
		protected internal virtual int LocalPort
		{
			get
			{
				return Localport;
			}
		}

		internal virtual Socket Socket
		{
			set
			{
				this.Socket_Renamed = value;
			}
			get
			{
				return Socket_Renamed;
			}
		}


		internal virtual ServerSocket ServerSocket
		{
			set
			{
				this.ServerSocket_Renamed = value;
			}
			get
			{
				return ServerSocket_Renamed;
			}
		}


		/// <summary>
		/// Returns the address and port of this socket as a {@code String}.
		/// </summary>
		/// <returns>  a string representation of this socket. </returns>
		public override String ToString()
		{
			return "Socket[addr=" + InetAddress + ",port=" + Port + ",localport=" + LocalPort + "]";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void reset() throws java.io.IOException
		internal virtual void Reset()
		{
			Address = null;
			Port_Renamed = 0;
			Localport = 0;
		}

		/// <summary>
		/// Sets performance preferences for this socket.
		/// 
		/// <para> Sockets use the TCP/IP protocol by default.  Some implementations
		/// may offer alternative protocols which have different performance
		/// characteristics than TCP/IP.  This method allows the application to
		/// express its own preferences as to how these tradeoffs should be made
		/// when the implementation chooses from the available protocols.
		/// 
		/// </para>
		/// <para> Performance preferences are described by three integers
		/// whose values indicate the relative importance of short connection time,
		/// low latency, and high bandwidth.  The absolute values of the integers
		/// are irrelevant; in order to choose a protocol the values are simply
		/// compared, with larger values indicating stronger preferences. Negative
		/// values represent a lower priority than positive values. If the
		/// application prefers short connection time over both low latency and high
		/// bandwidth, for example, then it could invoke this method with the values
		/// {@code (1, 0, 0)}.  If the application prefers high bandwidth above low
		/// latency, and low latency above short connection time, then it could
		/// invoke this method with the values {@code (0, 1, 2)}.
		/// 
		/// By default, this method does nothing, unless it is overridden in a
		/// a sub-class.
		/// 
		/// </para>
		/// </summary>
		/// <param name="connectionTime">
		///         An {@code int} expressing the relative importance of a short
		///         connection time
		/// </param>
		/// <param name="latency">
		///         An {@code int} expressing the relative importance of low
		///         latency
		/// </param>
		/// <param name="bandwidth">
		///         An {@code int} expressing the relative importance of high
		///         bandwidth
		/// 
		/// @since 1.5 </param>
		protected internal virtual void SetPerformancePreferences(int connectionTime, int latency, int bandwidth)
		{
			/* Not implemented yet */
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> void setOption(SocketOption<T> name, T value) throws java.io.IOException
		internal virtual void setOption<T>(SocketOption<T> name, T value)
		{
			if (name == StandardSocketOptions.SO_KEEPALIVE)
			{
				SetOption(SocketOptions_Fields.SO_KEEPALIVE, value);
			}
			else if (name == StandardSocketOptions.SO_SNDBUF)
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
			else if (name == StandardSocketOptions.SO_LINGER)
			{
				SetOption(SocketOptions_Fields.SO_LINGER, value);
			}
			else if (name == StandardSocketOptions.IP_TOS)
			{
				SetOption(SocketOptions_Fields.IP_TOS, value);
			}
			else if (name == StandardSocketOptions.TCP_NODELAY)
			{
				SetOption(SocketOptions_Fields.TCP_NODELAY, value);
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
			if (name == StandardSocketOptions.SO_KEEPALIVE)
			{
				return (T)GetOption(SocketOptions_Fields.SO_KEEPALIVE);
			}
			else if (name == StandardSocketOptions.SO_SNDBUF)
			{
				return (T)GetOption(SocketOptions_Fields.SO_SNDBUF);
			}
			else if (name == StandardSocketOptions.SO_RCVBUF)
			{
				return (T)GetOption(SocketOptions_Fields.SO_RCVBUF);
			}
			else if (name == StandardSocketOptions.SO_REUSEADDR)
			{
				return (T)GetOption(SocketOptions_Fields.SO_REUSEADDR);
			}
			else if (name == StandardSocketOptions.SO_LINGER)
			{
				return (T)GetOption(SocketOptions_Fields.SO_LINGER);
			}
			else if (name == StandardSocketOptions.IP_TOS)
			{
				return (T)GetOption(SocketOptions_Fields.IP_TOS);
			}
			else if (name == StandardSocketOptions.TCP_NODELAY)
			{
				return (T)GetOption(SocketOptions_Fields.TCP_NODELAY);
			}
			else
			{
				throw new UnsupportedOperationException("unsupported option");
			}
		}
	}

}