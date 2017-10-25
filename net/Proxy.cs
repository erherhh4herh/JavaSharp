using System;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class represents a proxy setting, typically a type (http, socks) and
	/// a socket address.
	/// A {@code Proxy} is an immutable object.
	/// </summary>
	/// <seealso cref=     java.net.ProxySelector
	/// @author Yingxian Wang
	/// @author Jean-Christophe Collet
	/// @since   1.5 </seealso>
	public class Proxy
	{

		/// <summary>
		/// Represents the proxy type.
		/// 
		/// @since 1.5
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Represents a direct connection, or the absence of a proxy.
			/// </summary>
			DIRECT,
			/// <summary>
			/// Represents proxy for high level protocols such as HTTP or FTP.
			/// </summary>
			HTTP,
			/// <summary>
			/// Represents a SOCKS (V4 or V5) proxy.
			/// </summary>
			SOCKS
		}

		private Type Type_Renamed;
		private SocketAddress Sa;

		/// <summary>
		/// A proxy setting that represents a {@code DIRECT} connection,
		/// basically telling the protocol handler not to use any proxying.
		/// Used, for instance, to create sockets bypassing any other global
		/// proxy settings (like SOCKS):
		/// <P>
		/// {@code Socket s = new Socket(Proxy.NO_PROXY);}
		/// 
		/// </summary>
		public static readonly Proxy NO_PROXY = new Proxy();

		// Creates the proxy that represents a {@code DIRECT} connection.
		private Proxy()
		{
			Type_Renamed = Type.DIRECT;
			Sa = null;
		}

		/// <summary>
		/// Creates an entry representing a PROXY connection.
		/// Certain combinations are illegal. For instance, for types Http, and
		/// Socks, a SocketAddress <b>must</b> be provided.
		/// <P>
		/// Use the {@code Proxy.NO_PROXY} constant
		/// for representing a direct connection.
		/// </summary>
		/// <param name="type"> the {@code Type} of the proxy </param>
		/// <param name="sa"> the {@code SocketAddress} for that proxy </param>
		/// <exception cref="IllegalArgumentException"> when the type and the address are
		/// incompatible </exception>
		public Proxy(Type type, SocketAddress sa)
		{
			if ((type == Type.DIRECT) || !(sa is InetSocketAddress))
			{
				throw new IllegalArgumentException("type " + type + " is not compatible with address " + sa);
			}
			this.Type_Renamed = type;
			this.Sa = sa;
		}

		/// <summary>
		/// Returns the proxy type.
		/// </summary>
		/// <returns> a Type representing the proxy type </returns>
		public virtual Type Type()
		{
			return Type_Renamed;
		}

		/// <summary>
		/// Returns the socket address of the proxy, or
		/// {@code null} if its a direct connection.
		/// </summary>
		/// <returns> a {@code SocketAddress} representing the socket end
		///         point of the proxy </returns>
		public virtual SocketAddress Address()
		{
			return Sa;
		}

		/// <summary>
		/// Constructs a string representation of this Proxy.
		/// This String is constructed by calling toString() on its type
		/// and concatenating " @ " and the toString() result from its address
		/// if its type is not {@code DIRECT}.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		public override String ToString()
		{
			if (Type() == Type.DIRECT)
			{
				return "DIRECT";
			}
			return Type() + " @ " + Address();
		}

			/// <summary>
			/// Compares this object against the specified object.
			/// The result is {@code true} if and only if the argument is
			/// not {@code null} and it represents the same proxy as
			/// this object.
			/// <para>
			/// Two instances of {@code Proxy} represent the same
			/// address if both the SocketAddresses and type are equal.
			/// 
			/// </para>
			/// </summary>
			/// <param name="obj">   the object to compare against. </param>
			/// <returns>  {@code true} if the objects are the same;
			///          {@code false} otherwise. </returns>
			/// <seealso cref= java.net.InetSocketAddress#equals(java.lang.Object) </seealso>
		public sealed override bool Equals(Object obj)
		{
			if (obj == null || !(obj is Proxy))
			{
				return false;
			}
			Proxy p = (Proxy) obj;
			if (p.Type() == Type())
			{
				if (Address() == null)
				{
					return (p.Address() == null);
				}
				else
				{
					return Address().Equals(p.Address());
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a hashcode for this Proxy.
		/// </summary>
		/// <returns>  a hash code value for this Proxy. </returns>
		public sealed override int HashCode()
		{
			if (Address() == null)
			{
				return Type().HashCode();
			}
			return Type().HashCode() + Address().HashCode();
		}
	}

}