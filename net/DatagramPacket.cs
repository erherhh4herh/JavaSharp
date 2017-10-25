using System.Runtime.InteropServices;

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
	/// This class represents a datagram packet.
	/// <para>
	/// Datagram packets are used to implement a connectionless packet
	/// delivery service. Each message is routed from one machine to
	/// another based solely on information contained within that packet.
	/// Multiple packets sent from one machine to another might be routed
	/// differently, and might arrive in any order. Packet delivery is
	/// not guaranteed.
	/// 
	/// @author  Pavani Diwanji
	/// @author  Benjamin Renaud
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public sealed class DatagramPacket
	{

		/// <summary>
		/// Perform class initialization
		/// </summary>
		static DatagramPacket()
		{
			java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			init();
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual Void Run()
			{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//				System.loadLibrary("net");
				return null;
			}
		}

		/*
		 * The fields of this class are package-private since DatagramSocketImpl
		 * classes needs to access them.
		 */
		internal sbyte[] Buf;
		internal int Offset_Renamed;
		internal int Length_Renamed;
		internal int BufLength;
		internal InetAddress Address_Renamed;
		internal int Port_Renamed;

		/// <summary>
		/// Constructs a {@code DatagramPacket} for receiving packets of
		/// length {@code length}, specifying an offset into the buffer.
		/// <para>
		/// The {@code length} argument must be less than or equal to
		/// {@code buf.length}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="buf">      buffer for holding the incoming datagram. </param>
		/// <param name="offset">   the offset for the buffer </param>
		/// <param name="length">   the number of bytes to read.
		/// 
		/// @since 1.2 </param>
		public DatagramPacket(sbyte[] buf, int offset, int length)
		{
			SetData(buf, offset, length);
			this.Address_Renamed = null;
			this.Port_Renamed = -1;
		}

		/// <summary>
		/// Constructs a {@code DatagramPacket} for receiving packets of
		/// length {@code length}.
		/// <para>
		/// The {@code length} argument must be less than or equal to
		/// {@code buf.length}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="buf">      buffer for holding the incoming datagram. </param>
		/// <param name="length">   the number of bytes to read. </param>
		public DatagramPacket(sbyte[] buf, int length) : this(buf, 0, length)
		{
		}

		/// <summary>
		/// Constructs a datagram packet for sending packets of length
		/// {@code length} with offset {@code ioffset}to the
		/// specified port number on the specified host. The
		/// {@code length} argument must be less than or equal to
		/// {@code buf.length}.
		/// </summary>
		/// <param name="buf">      the packet data. </param>
		/// <param name="offset">   the packet data offset. </param>
		/// <param name="length">   the packet data length. </param>
		/// <param name="address">  the destination address. </param>
		/// <param name="port">     the destination port number. </param>
		/// <seealso cref= java.net.InetAddress
		/// 
		/// @since 1.2 </seealso>
		public DatagramPacket(sbyte[] buf, int offset, int length, InetAddress address, int port)
		{
			SetData(buf, offset, length);
			Address = address;
			Port = port;
		}

		/// <summary>
		/// Constructs a datagram packet for sending packets of length
		/// {@code length} with offset {@code ioffset}to the
		/// specified port number on the specified host. The
		/// {@code length} argument must be less than or equal to
		/// {@code buf.length}.
		/// </summary>
		/// <param name="buf">      the packet data. </param>
		/// <param name="offset">   the packet data offset. </param>
		/// <param name="length">   the packet data length. </param>
		/// <param name="address">  the destination socket address. </param>
		/// <exception cref="IllegalArgumentException"> if address type is not supported </exception>
		/// <seealso cref= java.net.InetAddress
		/// 
		/// @since 1.4 </seealso>
		public DatagramPacket(sbyte[] buf, int offset, int length, SocketAddress address)
		{
			SetData(buf, offset, length);
			SocketAddress = address;
		}

		/// <summary>
		/// Constructs a datagram packet for sending packets of length
		/// {@code length} to the specified port number on the specified
		/// host. The {@code length} argument must be less than or equal
		/// to {@code buf.length}.
		/// </summary>
		/// <param name="buf">      the packet data. </param>
		/// <param name="length">   the packet length. </param>
		/// <param name="address">  the destination address. </param>
		/// <param name="port">     the destination port number. </param>
		/// <seealso cref=     java.net.InetAddress </seealso>
		public DatagramPacket(sbyte[] buf, int length, InetAddress address, int port) : this(buf, 0, length, address, port)
		{
		}

		/// <summary>
		/// Constructs a datagram packet for sending packets of length
		/// {@code length} to the specified port number on the specified
		/// host. The {@code length} argument must be less than or equal
		/// to {@code buf.length}.
		/// </summary>
		/// <param name="buf">      the packet data. </param>
		/// <param name="length">   the packet length. </param>
		/// <param name="address">  the destination address. </param>
		/// <exception cref="IllegalArgumentException"> if address type is not supported
		/// @since 1.4 </exception>
		/// <seealso cref=     java.net.InetAddress </seealso>
		public DatagramPacket(sbyte[] buf, int length, SocketAddress address) : this(buf, 0, length, address)
		{
		}

		/// <summary>
		/// Returns the IP address of the machine to which this datagram is being
		/// sent or from which the datagram was received.
		/// </summary>
		/// <returns>  the IP address of the machine to which this datagram is being
		///          sent or from which the datagram was received. </returns>
		/// <seealso cref=     java.net.InetAddress </seealso>
		/// <seealso cref= #setAddress(java.net.InetAddress) </seealso>
		public InetAddress Address
		{
			get
			{
				lock (this)
				{
					return Address_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					Address_Renamed = value;
				}
			}
		}

		/// <summary>
		/// Returns the port number on the remote host to which this datagram is
		/// being sent or from which the datagram was received.
		/// </summary>
		/// <returns>  the port number on the remote host to which this datagram is
		///          being sent or from which the datagram was received. </returns>
		/// <seealso cref= #setPort(int) </seealso>
		public int Port
		{
			get
			{
				lock (this)
				{
					return Port_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					if (value < 0 || value > 0xFFFF)
					{
						throw new IllegalArgumentException("Port out of range:" + value);
					}
					Port_Renamed = value;
				}
			}
		}

		/// <summary>
		/// Returns the data buffer. The data received or the data to be sent
		/// starts from the {@code offset} in the buffer,
		/// and runs for {@code length} long.
		/// </summary>
		/// <returns>  the buffer used to receive or  send data </returns>
		/// <seealso cref= #setData(byte[], int, int) </seealso>
		public sbyte[] Data
		{
			get
			{
				lock (this)
				{
					return Buf;
				}
			}
			set
			{
				lock (this)
				{
					if (value == null)
					{
						throw new NullPointerException("null packet buffer");
					}
					this.Buf = value;
					this.Offset_Renamed = 0;
					this.Length_Renamed = value.Length;
					this.BufLength = value.Length;
				}
			}
		}

		/// <summary>
		/// Returns the offset of the data to be sent or the offset of the
		/// data received.
		/// </summary>
		/// <returns>  the offset of the data to be sent or the offset of the
		///          data received.
		/// 
		/// @since 1.2 </returns>
		public int Offset
		{
			get
			{
				lock (this)
				{
					return Offset_Renamed;
				}
			}
		}

		/// <summary>
		/// Returns the length of the data to be sent or the length of the
		/// data received.
		/// </summary>
		/// <returns>  the length of the data to be sent or the length of the
		///          data received. </returns>
		/// <seealso cref= #setLength(int) </seealso>
		public int Length
		{
			get
			{
				lock (this)
				{
					return Length_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					if ((value + Offset_Renamed) > Buf.Length || value < 0 || (value + Offset_Renamed) < 0)
					{
						throw new IllegalArgumentException("illegal length");
					}
					this.Length_Renamed = value;
					this.BufLength = this.Length_Renamed;
				}
			}
		}

		/// <summary>
		/// Set the data buffer for this packet. This sets the
		/// data, length and offset of the packet.
		/// </summary>
		/// <param name="buf"> the buffer to set for this packet
		/// </param>
		/// <param name="offset"> the offset into the data
		/// </param>
		/// <param name="length"> the length of the data
		///       and/or the length of the buffer used to receive data
		/// </param>
		/// <exception cref="NullPointerException"> if the argument is null
		/// </exception>
		/// <seealso cref= #getData </seealso>
		/// <seealso cref= #getOffset </seealso>
		/// <seealso cref= #getLength
		/// 
		/// @since 1.2 </seealso>
		public void SetData(sbyte[] buf, int offset, int length)
		{
			lock (this)
			{
				/* this will check to see if buf is null */
				if (length < 0 || offset < 0 || (length + offset) < 0 || ((length + offset) > buf.Length))
				{
					throw new IllegalArgumentException("illegal length or offset");
				}
				this.Buf = buf;
				this.Length_Renamed = length;
				this.BufLength = length;
				this.Offset_Renamed = offset;
			}
		}



		/// <summary>
		/// Sets the SocketAddress (usually IP address + port number) of the remote
		/// host to which this datagram is being sent.
		/// </summary>
		/// <param name="address"> the {@code SocketAddress} </param>
		/// <exception cref="IllegalArgumentException"> if address is null or is a
		///          SocketAddress subclass not supported by this socket
		/// 
		/// @since 1.4 </exception>
		/// <seealso cref= #getSocketAddress </seealso>
		public SocketAddress SocketAddress
		{
			set
			{
				lock (this)
				{
					if (value == null || !(value is InetSocketAddress))
					{
						throw new IllegalArgumentException("unsupported address type");
					}
					InetSocketAddress addr = (InetSocketAddress) value;
					if (addr.Unresolved)
					{
						throw new IllegalArgumentException("unresolved address");
					}
					Address = addr.Address;
					Port = addr.Port;
				}
			}
			get
			{
				lock (this)
				{
					return new InetSocketAddress(Address, Port);
				}
			}
		}




		/// <summary>
		/// Perform class load-time initializations.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void init();
	}

}