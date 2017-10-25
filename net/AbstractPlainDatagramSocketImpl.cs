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

	using ResourceManager = sun.net.ResourceManager;

	/// <summary>
	/// Abstract datagram and multicast socket implementation base class.
	/// Note: This is not a public class, so that applets cannot call
	/// into the implementation directly and hence cannot bypass the
	/// security checks present in the DatagramSocket and MulticastSocket
	/// classes.
	/// 
	/// @author Pavani Diwanji
	/// </summary>

	internal abstract class AbstractPlainDatagramSocketImpl : DatagramSocketImpl
	{
		/* timeout value for receive() */
		internal int Timeout = 0;
		internal bool Connected = false;
		private int TrafficClass = 0;
		protected internal InetAddress ConnectedAddress = null;
		private int ConnectedPort = -1;

		private static final String os = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("os.name")
	   );

		/// <summary>
		/// flag set if the native connect() call not to be used
		/// </summary>
		private final static bool ConnectDisabled = os.contains("OS X");

		/// <summary>
		/// Load net library into runtime.
		/// </summary>
		static AbstractPlainDatagramSocketImpl()
		{
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
			init();
		}

		/// <summary>
		/// Creates a datagram socket
		/// </summary>
		protected synchronized void create() throws SocketException
		{
			ResourceManager.beforeUdpCreate();
			Fd = new FileDescriptor();
			try
			{
				DatagramSocketCreate();
			}
			catch (SocketException ioe)
			{
				ResourceManager.afterUdpClose();
				Fd = null;
				throw ioe;
			}
		}

		/// <summary>
		/// Binds a datagram socket to a local port.
		/// </summary>
		protected synchronized void bind(int lport, InetAddress laddr) throws SocketException
		{
			Bind0(lport, laddr);
		}

		protected abstract void bind0(int lport, InetAddress laddr) throws SocketException;

		/// <summary>
		/// Sends a datagram packet. The packet contains the data and the
		/// destination address to send the packet to. </summary>
		/// <param name="p"> the packet to be sent. </param>
		protected abstract void send(DatagramPacket p) throws IOException;

		/// <summary>
		/// Connects a datagram socket to a remote destination. This associates the remote
		/// address with the local socket so that datagrams may only be sent to this destination
		/// and received from this destination. </summary>
		/// <param name="address"> the remote InetAddress to connect to </param>
		/// <param name="port"> the remote port number </param>
		protected void connect(InetAddress address, int port) throws SocketException
		{
			Connect0(address, port);
			ConnectedAddress = address;
			ConnectedPort = port;
			Connected = true;
		}

		/// <summary>
		/// Disconnects a previously connected socket. Does nothing if the socket was
		/// not connected already.
		/// </summary>
		protected void disconnect()
		{
			Disconnect0(ConnectedAddress.Holder().Family);
			Connected = false;
			ConnectedAddress = null;
			ConnectedPort = -1;
		}

		/// <summary>
		/// Peek at the packet to see who it is from. </summary>
		/// <param name="i"> the address to populate with the sender address </param>
		protected abstract int Peek(InetAddress i) throws IOException;
		protected abstract int PeekData(DatagramPacket p) throws IOException;
		/// <summary>
		/// Receive the datagram packet. </summary>
		/// <param name="p"> the packet to receive into </param>
		protected synchronized void receive(DatagramPacket p) throws IOException
		{
			Receive0(p);
		}

		protected abstract void receive0(DatagramPacket p) throws IOException;

		/// <summary>
		/// Set the TTL (time-to-live) option. </summary>
		/// <param name="ttl"> TTL to be set. </param>
		protected abstract void setTimeToLive(int ttl) throws IOException;

		/// <summary>
		/// Get the TTL (time-to-live) option.
		/// </summary>
		protected abstract int TimeToLive throws IOException;

		/// <summary>
		/// Set the TTL (time-to-live) option. </summary>
		/// <param name="ttl"> TTL to be set. </param>
		[Obsolete]
		protected abstract void setTTL(sbyte ttl) throws IOException;

		/// <summary>
		/// Get the TTL (time-to-live) option.
		/// </summary>
		[Obsolete]
		protected abstract sbyte TTL throws IOException;

		/// <summary>
		/// Join the multicast group. </summary>
		/// <param name="inetaddr"> multicast address to join. </param>
		protected void join(InetAddress inetaddr) throws IOException
		{
			Join(inetaddr, null);
		}

		/// <summary>
		/// Leave the multicast group. </summary>
		/// <param name="inetaddr"> multicast address to leave. </param>
		protected void leave(InetAddress inetaddr) throws IOException
		{
			Leave(inetaddr, null);
		}
		/// <summary>
		/// Join the multicast group. </summary>
		/// <param name="mcastaddr"> multicast address to join. </param>
		/// <param name="netIf"> specifies the local interface to receive multicast
		///        datagram packets </param>
		/// <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4 </exception>

		protected void joinGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws IOException
		{
			if (mcastaddr == null || !(mcastaddr is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			Join(((InetSocketAddress)mcastaddr).Address, netIf);
		}

		protected abstract void join(InetAddress inetaddr, NetworkInterface netIf) throws IOException;

		/// <summary>
		/// Leave the multicast group. </summary>
		/// <param name="mcastaddr">  multicast address to leave. </param>
		/// <param name="netIf"> specified the local interface to leave the group at </param>
		/// <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		///          SocketAddress subclass not supported by this socket
		/// @since 1.4 </exception>
		protected void leaveGroup(SocketAddress mcastaddr, NetworkInterface netIf) throws IOException
		{
			if (mcastaddr == null || !(mcastaddr is InetSocketAddress))
			{
				throw new IllegalArgumentException("Unsupported address type");
			}
			Leave(((InetSocketAddress)mcastaddr).Address, netIf);
		}

		protected abstract void leave(InetAddress inetaddr, NetworkInterface netIf) throws IOException;

		/// <summary>
		/// Close the socket.
		/// </summary>
		protected void close()
		{
			if (Fd != null)
			{
				DatagramSocketClose();
				ResourceManager.afterUdpClose();
				Fd = null;
			}
		}

		protected bool Closed
		{
			return (Fd == null) ? true : false;
		}

		protected void finalize()
		{
			Close();
		}

		/// <summary>
		/// set a value - since we only support (setting) binary options
		/// here, o must be a Boolean
		/// </summary>

		 public void setOption(int optID, Object o) throws SocketException
		 {
			 if (Closed)
			 {
				 throw new SocketException("Socket Closed");
			 }
			 switch (optID)
			 {
				/* check type safety b4 going native.  These should never
				 * fail, since only java.Socket* has access to
				 * PlainSocketImpl.setOption().
				 */
			 case SocketOptions_Fields.SO_TIMEOUT:
				 if (o == null || !(o is Integer))
				 {
					 throw new SocketException("bad argument for SO_TIMEOUT");
				 }
				 int tmp = ((Integer) o).IntValue();
				 if (tmp < 0)
				 {
					 throw new IllegalArgumentException("timeout < 0");
				 }
				 Timeout = tmp;
				 return;
			 case SocketOptions_Fields.IP_TOS:
				 if (o == null || !(o is Integer))
				 {
					 throw new SocketException("bad argument for IP_TOS");
				 }
				 TrafficClass = ((Integer)o).IntValue();
				 break;
			 case SocketOptions_Fields.SO_REUSEADDR:
				 if (o == null || !(o is Boolean))
				 {
					 throw new SocketException("bad argument for SO_REUSEADDR");
				 }
				 break;
			 case SocketOptions_Fields.SO_BROADCAST:
				 if (o == null || !(o is Boolean))
				 {
					 throw new SocketException("bad argument for SO_BROADCAST");
				 }
				 break;
			 case SocketOptions_Fields.SO_BINDADDR:
				 throw new SocketException("Cannot re-bind Socket");
			 case SocketOptions_Fields.SO_RCVBUF:
			 case SocketOptions_Fields.SO_SNDBUF:
				 if (o == null || !(o is Integer) || ((Integer)o).IntValue() < 0)
				 {
					 throw new SocketException("bad argument for SO_SNDBUF or " + "SO_RCVBUF");
				 }
				 break;
			 case SocketOptions_Fields.IP_MULTICAST_IF:
				 if (o == null || !(o is InetAddress))
				 {
					 throw new SocketException("bad argument for IP_MULTICAST_IF");
				 }
				 break;
			 case SocketOptions_Fields.IP_MULTICAST_IF2:
				 if (o == null || !(o is NetworkInterface))
				 {
					 throw new SocketException("bad argument for IP_MULTICAST_IF2");
				 }
				 break;
			 case SocketOptions_Fields.IP_MULTICAST_LOOP:
				 if (o == null || !(o is Boolean))
				 {
					 throw new SocketException("bad argument for IP_MULTICAST_LOOP");
				 }
				 break;
			 default:
				 throw new SocketException("invalid option: " + optID);
			 }
			 SocketSetOption(optID, o);
		 }

		/*
		 * get option's state - set or not
		 */

		public Object GetOption(int optID) throws SocketException
		{
			if (Closed)
			{
				throw new SocketException("Socket Closed");
			}

			Object result;

			switch (optID)
			{
				case SocketOptions_Fields.SO_TIMEOUT:
					result = new Integer(Timeout);
					break;

				case SocketOptions_Fields.IP_TOS:
					result = SocketGetOption(optID);
					if (((Integer)result).IntValue() == -1)
					{
						result = new Integer(TrafficClass);
					}
					break;

				case SocketOptions_Fields.SO_BINDADDR:
				case SocketOptions_Fields.IP_MULTICAST_IF:
				case SocketOptions_Fields.IP_MULTICAST_IF2:
				case SocketOptions_Fields.SO_RCVBUF:
				case SocketOptions_Fields.SO_SNDBUF:
				case SocketOptions_Fields.IP_MULTICAST_LOOP:
				case SocketOptions_Fields.SO_REUSEADDR:
				case SocketOptions_Fields.SO_BROADCAST:
					result = SocketGetOption(optID);
					break;

				default:
					throw new SocketException("invalid option: " + optID);
			}

			return result;
		}

		protected abstract void datagramSocketCreate() throws SocketException;
		protected abstract void datagramSocketClose();
		protected abstract void socketSetOption(int opt, Object val) throws SocketException;
		protected abstract Object SocketGetOption(int opt) throws SocketException;

		protected abstract void connect0(InetAddress address, int port) throws SocketException;
		protected abstract void disconnect0(int family);

		protected bool NativeConnectDisabled()
		{
			return ConnectDisabled;
		}

		native int DataAvailable();
		private static native void init();
	}


	private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
	{
		private readonly AbstractPlainDatagramSocketImpl outerInstance;

		public PrivilegedActionAnonymousInnerClassHelper(AbstractPlainDatagramSocketImpl outerInstance)
		{
			this.outerInstance = outerInstance;
		}

		public virtual Void Run()
		{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//			System.loadLibrary("net");
			return null;
		}
	}
}