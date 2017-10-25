using System.Collections.Generic;
using System.Runtime.InteropServices;

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

	using sun.security.action;

	/// <summary>
	/// This class represents a Network Interface made up of a name,
	/// and a list of IP addresses assigned to this interface.
	/// It is used to identify the local interface on which a multicast group
	/// is joined.
	/// 
	/// Interfaces are normally known by names such as "le0".
	/// 
	/// @since 1.4
	/// </summary>
	public sealed class NetworkInterface
	{
		private String Name_Renamed;
		private String DisplayName_Renamed;
		private int Index_Renamed;
		private InetAddress[] Addrs;
		private InterfaceAddress[] Bindings;
		private NetworkInterface[] Childs;
		private NetworkInterface Parent_Renamed = null;
		private bool @virtual = false;
		private static readonly NetworkInterface DefaultInterface;
		private static readonly int DefaultIndex; // index of defaultInterface

		static NetworkInterface()
		{
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

			init();
			DefaultInterface = DefaultInterface.Default;
			if (DefaultInterface != null)
			{
				DefaultIndex = DefaultInterface.Index;
			}
			else
			{
				DefaultIndex = 0;
			}
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

		/// <summary>
		/// Returns an NetworkInterface object with index set to 0 and name to null.
		/// Setting such an interface on a MulticastSocket will cause the
		/// kernel to choose one interface for sending multicast packets.
		/// 
		/// </summary>
		internal NetworkInterface()
		{
		}

		internal NetworkInterface(String name, int index, InetAddress[] addrs)
		{
			this.Name_Renamed = name;
			this.Index_Renamed = index;
			this.Addrs = addrs;
		}

		/// <summary>
		/// Get the name of this network interface.
		/// </summary>
		/// <returns> the name of this network interface </returns>
		public String Name
		{
			get
			{
					return Name_Renamed;
			}
		}

		/// <summary>
		/// Convenience method to return an Enumeration with all or a
		/// subset of the InetAddresses bound to this network interface.
		/// <para>
		/// If there is a security manager, its {@code checkConnect}
		/// method is called for each InetAddress. Only InetAddresses where
		/// the {@code checkConnect} doesn't throw a SecurityException
		/// will be returned in the Enumeration. However, if the caller has the
		/// <seealso cref="NetPermission"/>("getNetworkInformation") permission, then all
		/// InetAddresses are returned.
		/// </para>
		/// </summary>
		/// <returns> an Enumeration object with all or a subset of the InetAddresses
		/// bound to this network interface </returns>
		public IEnumerator<InetAddress> InetAddresses
		{
			get
			{
    
	//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
	//			class checkedAddresses implements java.util.Iterator<InetAddress>
		//		{
		//
		//			private int i=0, count=0;
		//			private InetAddress local_addrs[];
		//
		//			checkedAddresses()
		//			{
		//				local_addrs = new InetAddress[addrs.length];
		//				boolean trusted = true;
		//
		//				SecurityManager sec = System.getSecurityManager();
		//				if (sec != null)
		//				{
		//					try
		//					{
		//						sec.checkPermission(new NetPermission("getNetworkInformation"));
		//					}
		//					catch (SecurityException e)
		//					{
		//						trusted = false;
		//					}
		//				}
		//				for (int j=0; j<addrs.length; j++)
		//				{
		//					try
		//					{
		//						if (sec != null && !trusted)
		//						{
		//							sec.checkConnect(addrs[j].getHostAddress(), -1);
		//						}
		//						local_addrs[count++] = addrs[j];
		//					}
		//					catch (SecurityException e)
		//					{
		//					}
		//				}
		//
		//			}
		//
		//			public InetAddress nextElement()
		//			{
		//				if (i < count)
		//				{
		//					return local_addrs[i++];
		//				}
		//				else
		//				{
		//					throw new NoSuchElementException();
		//				}
		//			}
		//
		//			public boolean hasMoreElements()
		//			{
		//				return (i < count);
		//			}
		//		}
				return new checkedAddresses();
    
			}
		}

		/// <summary>
		/// Get a List of all or a subset of the {@code InterfaceAddresses}
		/// of this network interface.
		/// <para>
		/// If there is a security manager, its {@code checkConnect}
		/// method is called with the InetAddress for each InterfaceAddress.
		/// Only InterfaceAddresses where the {@code checkConnect} doesn't throw
		/// a SecurityException will be returned in the List.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code List} object with all or a subset of the
		///         InterfaceAddresss of this network interface
		/// @since 1.6 </returns>
		public IList<InterfaceAddress> InterfaceAddresses
		{
			get
			{
				IList<InterfaceAddress> lst = new List<InterfaceAddress>(1);
				SecurityManager sec = System.SecurityManager;
				for (int j = 0; j < Bindings.Length; j++)
				{
					try
					{
						if (sec != null)
						{
							sec.CheckConnect(Bindings[j].Address.HostAddress, -1);
						}
						lst.Add(Bindings[j]);
					}
					catch (SecurityException)
					{
					}
				}
				return lst;
			}
		}

		/// <summary>
		/// Get an Enumeration with all the subinterfaces (also known as virtual
		/// interfaces) attached to this network interface.
		/// <para>
		/// For instance eth0:1 will be a subinterface to eth0.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an Enumeration object with all of the subinterfaces
		/// of this network interface
		/// @since 1.6 </returns>
		public IEnumerator<NetworkInterface> SubInterfaces
		{
			get
			{
	//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
	//			class subIFs implements java.util.Iterator<NetworkInterface>
		//		{
		//
		//			private int i=0;
		//
		//			subIFs()
		//			{
		//			}
		//
		//			public NetworkInterface nextElement()
		//			{
		//				if (i < childs.length)
		//				{
		//					return childs[i++];
		//				}
		//				else
		//				{
		//					throw new NoSuchElementException();
		//				}
		//			}
		//
		//			public boolean hasMoreElements()
		//			{
		//				return (i < childs.length);
		//			}
		//		}
				return new subIFs();
    
			}
		}

		/// <summary>
		/// Returns the parent NetworkInterface of this interface if this is
		/// a subinterface, or {@code null} if it is a physical
		/// (non virtual) interface or has no parent.
		/// </summary>
		/// <returns> The {@code NetworkInterface} this interface is attached to.
		/// @since 1.6 </returns>
		public NetworkInterface Parent
		{
			get
			{
				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Returns the index of this network interface. The index is an integer greater
		/// or equal to zero, or {@code -1} for unknown. This is a system specific value
		/// and interfaces with the same name can have different indexes on different
		/// machines.
		/// </summary>
		/// <returns> the index of this network interface or {@code -1} if the index is
		///         unknown </returns>
		/// <seealso cref= #getByIndex(int)
		/// @since 1.7 </seealso>
		public int Index
		{
			get
			{
				return Index_Renamed;
			}
		}

		/// <summary>
		/// Get the display name of this network interface.
		/// A display name is a human readable String describing the network
		/// device.
		/// </summary>
		/// <returns> a non-empty string representing the display name of this network
		///         interface, or null if no display name is available. </returns>
		public String DisplayName
		{
			get
			{
				/* strict TCK conformance */
				return "".Equals(DisplayName_Renamed) ? null : DisplayName_Renamed;
			}
		}

		/// <summary>
		/// Searches for the network interface with the specified name.
		/// </summary>
		/// <param name="name">
		///          The name of the network interface.
		/// </param>
		/// <returns>  A {@code NetworkInterface} with the specified name,
		///          or {@code null} if there is no network interface
		///          with the specified name.
		/// </returns>
		/// <exception cref="SocketException">
		///          If an I/O error occurs.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the specified name is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static NetworkInterface getByName(String name) throws SocketException
		public static NetworkInterface GetByName(String name)
		{
			if (name == null)
			{
				throw new NullPointerException();
			}
			return getByName0(name);
		}

		/// <summary>
		/// Get a network interface given its index.
		/// </summary>
		/// <param name="index"> an integer, the index of the interface </param>
		/// <returns> the NetworkInterface obtained from its index, or {@code null} if
		///         there is no interface with such an index on the system </returns>
		/// <exception cref="SocketException">  if an I/O error occurs. </exception>
		/// <exception cref="IllegalArgumentException"> if index has a negative value </exception>
		/// <seealso cref= #getIndex()
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static NetworkInterface getByIndex(int index) throws SocketException
		public static NetworkInterface GetByIndex(int index)
		{
			if (index < 0)
			{
				throw new IllegalArgumentException("Interface index can't be negative");
			}
			return getByIndex0(index);
		}

		/// <summary>
		/// Convenience method to search for a network interface that
		/// has the specified Internet Protocol (IP) address bound to
		/// it.
		/// <para>
		/// If the specified IP address is bound to multiple network
		/// interfaces it is not defined which network interface is
		/// returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="addr">
		///          The {@code InetAddress} to search with.
		/// </param>
		/// <returns>  A {@code NetworkInterface}
		///          or {@code null} if there is no network interface
		///          with the specified IP address.
		/// </returns>
		/// <exception cref="SocketException">
		///          If an I/O error occurs.
		/// </exception>
		/// <exception cref="NullPointerException">
		///          If the specified address is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static NetworkInterface getByInetAddress(InetAddress addr) throws SocketException
		public static NetworkInterface GetByInetAddress(InetAddress addr)
		{
			if (addr == null)
			{
				throw new NullPointerException();
			}
			if (!(addr is Inet4Address || addr is Inet6Address))
			{
				throw new IllegalArgumentException("invalid address type");
			}
			return getByInetAddress0(addr);
		}

		/// <summary>
		/// Returns all the interfaces on this machine. The {@code Enumeration}
		/// contains at least one element, possibly representing a loopback
		/// interface that only supports communication between entities on
		/// this machine.
		/// 
		/// NOTE: can use getNetworkInterfaces()+getInetAddresses()
		///       to obtain all IP addresses for this node
		/// </summary>
		/// <returns> an Enumeration of NetworkInterfaces found on this machine </returns>
		/// <exception cref="SocketException">  if an I/O error occurs. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.Iterator<NetworkInterface> getNetworkInterfaces() throws SocketException
		public static IEnumerator<NetworkInterface> NetworkInterfaces
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final NetworkInterface[] netifs = getAll();
				NetworkInterface[] netifs = All;
    
				// specified to return null if no network interfaces
				if (netifs == null)
				{
					return null;
				}
    
				return new IteratorAnonymousInnerClassHelper(netifs);
			}
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<NetworkInterface>
		{
			private java.net.NetworkInterface[] Netifs;

			public IteratorAnonymousInnerClassHelper(java.net.NetworkInterface[] netifs)
			{
				this.Netifs = netifs;
			}

			private int i = 0;
			public virtual NetworkInterface NextElement()
			{
				if (Netifs != null && i < Netifs.Length)
				{
					NetworkInterface netif = Netifs[i++];
					return netif;
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual bool HasMoreElements()
			{
				return (Netifs != null && i < Netifs.Length);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static NetworkInterface[] getAll();

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static NetworkInterface getByName0(String name);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static NetworkInterface getByIndex0(int index);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static NetworkInterface getByInetAddress0(InetAddress addr);

		/// <summary>
		/// Returns whether a network interface is up and running.
		/// </summary>
		/// <returns>  {@code true} if the interface is up and running. </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isUp() throws SocketException
		public bool Up
		{
			get
			{
				return isUp0(Name_Renamed, Index_Renamed);
			}
		}

		/// <summary>
		/// Returns whether a network interface is a loopback interface.
		/// </summary>
		/// <returns>  {@code true} if the interface is a loopback interface. </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isLoopback() throws SocketException
		public bool Loopback
		{
			get
			{
				return isLoopback0(Name_Renamed, Index_Renamed);
			}
		}

		/// <summary>
		/// Returns whether a network interface is a point to point interface.
		/// A typical point to point interface would be a PPP connection through
		/// a modem.
		/// </summary>
		/// <returns>  {@code true} if the interface is a point to point
		///          interface. </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isPointToPoint() throws SocketException
		public bool PointToPoint
		{
			get
			{
				return isP2P0(Name_Renamed, Index_Renamed);
			}
		}

		/// <summary>
		/// Returns whether a network interface supports multicasting or not.
		/// </summary>
		/// <returns>  {@code true} if the interface supports Multicasting. </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean supportsMulticast() throws SocketException
		public bool SupportsMulticast()
		{
			return supportsMulticast0(Name_Renamed, Index_Renamed);
		}

		/// <summary>
		/// Returns the hardware address (usually MAC) of the interface if it
		/// has one and if it can be accessed given the current privileges.
		/// If a security manager is set, then the caller must have
		/// the permission <seealso cref="NetPermission"/>("getNetworkInformation").
		/// </summary>
		/// <returns>  a byte array containing the address, or {@code null} if
		///          the address doesn't exist, is not accessible or a security
		///          manager is set and the caller does not have the permission
		///          NetPermission("getNetworkInformation")
		/// </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getHardwareAddress() throws SocketException
		public sbyte[] HardwareAddress
		{
			get
			{
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					try
					{
						sec.CheckPermission(new NetPermission("getNetworkInformation"));
					}
					catch (SecurityException)
					{
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						if (!InetAddresses.hasMoreElements())
						{
							// don't have connect permission to any local address
							return null;
						}
					}
				}
				foreach (InetAddress addr in Addrs)
				{
					if (addr is Inet4Address)
					{
						return getMacAddr0(((Inet4Address)addr).Address, Name_Renamed, Index_Renamed);
					}
				}
				return getMacAddr0(null, Name_Renamed, Index_Renamed);
			}
		}

		/// <summary>
		/// Returns the Maximum Transmission Unit (MTU) of this interface.
		/// </summary>
		/// <returns> the value of the MTU for that interface. </returns>
		/// <exception cref="SocketException"> if an I/O error occurs.
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMTU() throws SocketException
		public int MTU
		{
			get
			{
				return getMTU0(Name_Renamed, Index_Renamed);
			}
		}

		/// <summary>
		/// Returns whether this interface is a virtual interface (also called
		/// subinterface).
		/// Virtual interfaces are, on some systems, interfaces created as a child
		/// of a physical interface and given different settings (like address or
		/// MTU). Usually the name of the interface will the name of the parent
		/// followed by a colon (:) and a number identifying the child since there
		/// can be several virtual interfaces attached to a single physical
		/// interface.
		/// </summary>
		/// <returns> {@code true} if this interface is a virtual interface.
		/// @since 1.6 </returns>
		public bool Virtual
		{
			get
			{
				return @virtual;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean isUp0(String name, int ind);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean isLoopback0(String name, int ind);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean supportsMulticast0(String name, int ind);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static boolean isP2P0(String name, int ind);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static byte[] getMacAddr0(sbyte[] inAddr, String name, int ind);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int getMTU0(String name, int ind);

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and it represents the same NetworkInterface
		/// as this object.
		/// <para>
		/// Two instances of {@code NetworkInterface} represent the same
		/// NetworkInterface if both name and addrs are the same for both.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the object to compare against. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     java.net.InetAddress#getAddress() </seealso>
		public override bool Equals(Object obj)
		{
			if (!(obj is NetworkInterface))
			{
				return false;
			}
			NetworkInterface that = (NetworkInterface)obj;
			if (this.Name_Renamed != null)
			{
				if (!this.Name_Renamed.Equals(that.Name_Renamed))
				{
					return false;
				}
			}
			else
			{
				if (that.Name_Renamed != null)
				{
					return false;
				}
			}

			if (this.Addrs == null)
			{
				return that.Addrs == null;
			}
			else if (that.Addrs == null)
			{
				return false;
			}

			/* Both addrs not null. Compare number of addresses */

			if (this.Addrs.Length != that.Addrs.Length)
			{
				return false;
			}

			InetAddress[] thatAddrs = that.Addrs;
			int count = thatAddrs.Length;

			for (int i = 0; i < count; i++)
			{
				bool found = false;
				for (int j = 0; j < count; j++)
				{
					if (Addrs[i].Equals(thatAddrs[j]))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					return false;
				}
			}
			return true;
		}

		public override int HashCode()
		{
			return Name_Renamed == null? 0: Name_Renamed.HashCode();
		}

		public override String ToString()
		{
			String result = "name:";
			result += Name_Renamed == null? "null": Name_Renamed;
			if (DisplayName_Renamed != null)
			{
				result += " (" + DisplayName_Renamed + ")";
			}
			return result;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void init();

		/// <summary>
		/// Returns the default network interface of this system
		/// </summary>
		/// <returns> the default interface </returns>
		internal static NetworkInterface Default
		{
			get
			{
				return DefaultInterface;
			}
		}
	}

}