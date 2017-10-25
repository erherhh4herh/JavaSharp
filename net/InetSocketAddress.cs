using System;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


	/// 
	/// <summary>
	/// This class implements an IP Socket Address (IP address + port number)
	/// It can also be a pair (hostname + port number), in which case an attempt
	/// will be made to resolve the hostname. If resolution fails then the address
	/// is said to be <I>unresolved</I> but can still be used on some circumstances
	/// like connecting through a proxy.
	/// <para>
	/// It provides an immutable object used by sockets for binding, connecting, or
	/// as returned values.
	/// </para>
	/// <para>
	/// The <i>wildcard</i> is a special local IP address. It usually means "any"
	/// and can only be used for {@code bind} operations.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.net.Socket </seealso>
	/// <seealso cref= java.net.ServerSocket
	/// @since 1.4 </seealso>
	public class InetSocketAddress : SocketAddress
	{
		// Private implementation class pointed to by all public methods.
		private class InetSocketAddressHolder
		{
			// The hostname of the Socket Address
			internal String Hostname;
			// The IP address of the Socket Address
			internal InetAddress Addr;
			// The port number of the Socket Address
			internal int Port_Renamed;

			internal InetSocketAddressHolder(String hostname, InetAddress addr, int port)
			{
				this.Hostname = hostname;
				this.Addr = addr;
				this.Port_Renamed = port;
			}

			internal virtual int Port
			{
				get
				{
					return Port_Renamed;
				}
			}

			internal virtual InetAddress Address
			{
				get
				{
					return Addr;
				}
			}

			internal virtual String HostName
			{
				get
				{
					if (Hostname != null)
					{
						return Hostname;
					}
					if (Addr != null)
					{
						return Addr.HostName;
					}
					return null;
				}
			}

			internal virtual String HostString
			{
				get
				{
					if (Hostname != null)
					{
						return Hostname;
					}
					if (Addr != null)
					{
						if (Addr.Holder().HostName != null)
						{
							return Addr.Holder().HostName;
						}
						else
						{
							return Addr.HostAddress;
						}
					}
					return null;
				}
			}

			internal virtual bool Unresolved
			{
				get
				{
					return Addr == null;
				}
			}

			public override String ToString()
			{
				if (Unresolved)
				{
					return Hostname + ":" + Port_Renamed;
				}
				else
				{
					return Addr.ToString() + ":" + Port_Renamed;
				}
			}

			public override sealed bool Equals(Object obj)
			{
				if (obj == null || !(obj is InetSocketAddressHolder))
				{
					return false;
				}
				InetSocketAddressHolder that = (InetSocketAddressHolder)obj;
				bool sameIP;
				if (Addr != null)
				{
					sameIP = Addr.Equals(that.Addr);
				}
				else if (Hostname != null)
				{
					sameIP = (that.Addr == null) && Hostname.EqualsIgnoreCase(that.Hostname);
				}
				else
				{
					sameIP = (that.Addr == null) && (that.Hostname == null);
				}
				return sameIP && (Port_Renamed == that.Port_Renamed);
			}

			public override sealed int HashCode()
			{
				if (Addr != null)
				{
					return Addr.HashCode() + Port_Renamed;
				}
				if (Hostname != null)
				{
					return Hostname.ToLowerCase().HashCode() + Port_Renamed;
				}
				return Port_Renamed;
			}
		}

		[NonSerialized]
		private readonly InetSocketAddressHolder Holder;

		private new const long SerialVersionUID = 5076001401234631237L;

		private static int CheckPort(int port)
		{
			if (port < 0 || port > 0xFFFF)
			{
				throw new IllegalArgumentException("port out of range:" + port);
			}
			return port;
		}

		private static String CheckHost(String hostname)
		{
			if (hostname == null)
			{
				throw new IllegalArgumentException("hostname can't be null");
			}
			return hostname;
		}

		/// <summary>
		/// Creates a socket address where the IP address is the wildcard address
		/// and the port number a specified value.
		/// <para>
		/// A valid port value is between 0 and 65535.
		/// A port number of {@code zero} will let the system pick up an
		/// ephemeral port in a {@code bind} operation.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="port">    The port number </param>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside the specified
		/// range of valid port values. </exception>
		public InetSocketAddress(int port) : this(InetAddress.AnyLocalAddress(), port)
		{
		}

		/// 
		/// <summary>
		/// Creates a socket address from an IP address and a port number.
		/// <para>
		/// A valid port value is between 0 and 65535.
		/// A port number of {@code zero} will let the system pick up an
		/// ephemeral port in a {@code bind} operation.
		/// <P>
		/// A {@code null} address will assign the <i>wildcard</i> address.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="addr">    The IP address </param>
		/// <param name="port">    The port number </param>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside the specified
		/// range of valid port values. </exception>
		public InetSocketAddress(InetAddress addr, int port)
		{
			Holder = new InetSocketAddressHolder(null, addr == null ? InetAddress.AnyLocalAddress() : addr, CheckPort(port));
		}

		/// 
		/// <summary>
		/// Creates a socket address from a hostname and a port number.
		/// <para>
		/// An attempt will be made to resolve the hostname into an InetAddress.
		/// If that attempt fails, the address will be flagged as <I>unresolved</I>.
		/// </para>
		/// <para>
		/// If there is a security manager, its {@code checkConnect} method
		/// is called with the host name as its argument to check the permission
		/// to resolve it. This could result in a SecurityException.
		/// <P>
		/// A valid port value is between 0 and 65535.
		/// A port number of {@code zero} will let the system pick up an
		/// ephemeral port in a {@code bind} operation.
		/// <P>
		/// </para>
		/// </summary>
		/// <param name="hostname"> the Host name </param>
		/// <param name="port">    The port number </param>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside the range
		/// of valid port values, or if the hostname parameter is <TT>null</TT>. </exception>
		/// <exception cref="SecurityException"> if a security manager is present and
		///                           permission to resolve the host name is
		///                           denied. </exception>
		/// <seealso cref=     #isUnresolved() </seealso>
		public InetSocketAddress(String hostname, int port)
		{
			CheckHost(hostname);
			InetAddress addr = null;
			String host = null;
			try
			{
				addr = InetAddress.GetByName(hostname);
			}
			catch (UnknownHostException)
			{
				host = hostname;
			}
			Holder = new InetSocketAddressHolder(host, addr, CheckPort(port));
		}

		// private constructor for creating unresolved instances
		private InetSocketAddress(int port, String hostname)
		{
			Holder = new InetSocketAddressHolder(hostname, null, port);
		}

		/// 
		/// <summary>
		/// Creates an unresolved socket address from a hostname and a port number.
		/// <para>
		/// No attempt will be made to resolve the hostname into an InetAddress.
		/// The address will be flagged as <I>unresolved</I>.
		/// </para>
		/// <para>
		/// A valid port value is between 0 and 65535.
		/// A port number of {@code zero} will let the system pick up an
		/// ephemeral port in a {@code bind} operation.
		/// <P>
		/// </para>
		/// </summary>
		/// <param name="host">    the Host name </param>
		/// <param name="port">    The port number </param>
		/// <exception cref="IllegalArgumentException"> if the port parameter is outside
		///                  the range of valid port values, or if the hostname
		///                  parameter is <TT>null</TT>. </exception>
		/// <seealso cref=     #isUnresolved() </seealso>
		/// <returns>  a {@code InetSocketAddress} representing the unresolved
		///          socket address
		/// @since 1.5 </returns>
		public static InetSocketAddress CreateUnresolved(String host, int port)
		{
			return new InetSocketAddress(CheckPort(port), CheckHost(host));
		}

		/// <summary>
		/// @serialField hostname String
		/// @serialField addr InetAddress
		/// @serialField port int
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("hostname", typeof(String)), new ObjectStreamField("addr", typeof(InetAddress)), new ObjectStreamField("port", typeof(int))};

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			// Don't call defaultWriteObject()
			 ObjectOutputStream.PutField pfields = @out.PutFields();
			 pfields.Put("hostname", Holder.Hostname);
			 pfields.Put("addr", Holder.Addr);
			 pfields.Put("port", Holder.Port_Renamed);
			 @out.WriteFields();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			// Don't call defaultReadObject()
			ObjectInputStream.GetField oisFields = @in.ReadFields();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String oisHostname = (String)oisFields.get("hostname", null);
			String oisHostname = (String)oisFields.Get("hostname", null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final InetAddress oisAddr = (InetAddress)oisFields.get("addr", null);
			InetAddress oisAddr = (InetAddress)oisFields.Get("addr", null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oisPort = oisFields.get("port", -1);
			int oisPort = oisFields.Get("port", -1);

			// Check that our invariants are satisfied
			CheckPort(oisPort);
			if (oisHostname == null && oisAddr == null)
			{
				throw new InvalidObjectException("hostname and addr " + "can't both be null");
			}

			InetSocketAddressHolder h = new InetSocketAddressHolder(oisHostname, oisAddr, oisPort);
			UNSAFE.putObject(this, FIELDS_OFFSET, h);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObjectNoData() throws java.io.ObjectStreamException
		private void ReadObjectNoData()
		{
			throw new InvalidObjectException("Stream data required");
		}

		private static readonly long FIELDS_OFFSET;
		private static readonly sun.misc.Unsafe UNSAFE;
		static InetSocketAddress()
		{
			try
			{
				sun.misc.Unsafe @unsafe = sun.misc.Unsafe.Unsafe;
				FIELDS_OFFSET = @unsafe.objectFieldOffset(typeof(InetSocketAddress).getDeclaredField("holder"));
				UNSAFE = @unsafe;
			}
			catch (ReflectiveOperationException e)
			{
				throw new Error(e);
			}
		}

		/// <summary>
		/// Gets the port number.
		/// </summary>
		/// <returns> the port number. </returns>
		public int Port
		{
			get
			{
				return Holder.Port;
			}
		}

		/// 
		/// <summary>
		/// Gets the {@code InetAddress}.
		/// </summary>
		/// <returns> the InetAdress or {@code null} if it is unresolved. </returns>
		public InetAddress Address
		{
			get
			{
				return Holder.Address;
			}
		}

		/// <summary>
		/// Gets the {@code hostname}.
		/// Note: This method may trigger a name service reverse lookup if the
		/// address was created with a literal IP address.
		/// </summary>
		/// <returns>  the hostname part of the address. </returns>
		public String HostName
		{
			get
			{
				return Holder.HostName;
			}
		}

		/// <summary>
		/// Returns the hostname, or the String form of the address if it
		/// doesn't have a hostname (it was created using a literal).
		/// This has the benefit of <b>not</b> attempting a reverse lookup.
		/// </summary>
		/// <returns> the hostname, or String representation of the address.
		/// @since 1.7 </returns>
		public String HostString
		{
			get
			{
				return Holder.HostString;
			}
		}

		/// <summary>
		/// Checks whether the address has been resolved or not.
		/// </summary>
		/// <returns> {@code true} if the hostname couldn't be resolved into
		///          an {@code InetAddress}. </returns>
		public bool Unresolved
		{
			get
			{
				return Holder.Unresolved;
			}
		}

		/// <summary>
		/// Constructs a string representation of this InetSocketAddress.
		/// This String is constructed by calling toString() on the InetAddress
		/// and concatenating the port number (with a colon). If the address
		/// is unresolved then the part before the colon will only contain the hostname.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		public override String ToString()
		{
			return Holder.ToString();
		}

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and it represents the same address as
		/// this object.
		/// <para>
		/// Two instances of {@code InetSocketAddress} represent the same
		/// address if both the InetAddresses (or hostnames if it is unresolved) and port
		/// numbers are equal.
		/// If both addresses are unresolved, then the hostname and the port number
		/// are compared.
		/// 
		/// Note: Hostnames are case insensitive. e.g. "FooBar" and "foobar" are
		/// considered equal.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare against. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		/// <seealso cref= java.net.InetAddress#equals(java.lang.Object) </seealso>
		public override sealed bool Equals(Object obj)
		{
			if (obj == null || !(obj is InetSocketAddress))
			{
				return false;
			}
			return Holder.Equals(((InetSocketAddress) obj).Holder);
		}

		/// <summary>
		/// Returns a hashcode for this socket address.
		/// </summary>
		/// <returns>  a hash code value for this socket address. </returns>
		public override sealed int HashCode()
		{
			return Holder.HashCode();
		}
	}

}