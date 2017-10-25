using System;
using System.Diagnostics;

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

namespace java.util
{

	/// <summary>
	/// A class that represents an immutable universally unique identifier (UUID).
	/// A UUID represents a 128-bit value.
	/// 
	/// <para> There exist different variants of these global identifiers.  The methods
	/// of this class are for manipulating the Leach-Salz variant, although the
	/// constructors allow the creation of any variant of UUID (described below).
	/// 
	/// </para>
	/// <para> The layout of a variant 2 (Leach-Salz) UUID is as follows:
	/// 
	/// The most significant long consists of the following unsigned fields:
	/// <pre>
	/// 0xFFFFFFFF00000000 time_low
	/// 0x00000000FFFF0000 time_mid
	/// 0x000000000000F000 version
	/// 0x0000000000000FFF time_hi
	/// </pre>
	/// The least significant long consists of the following unsigned fields:
	/// <pre>
	/// 0xC000000000000000 variant
	/// 0x3FFF000000000000 clock_seq
	/// 0x0000FFFFFFFFFFFF node
	/// </pre>
	/// 
	/// </para>
	/// <para> The variant field contains a value which identifies the layout of the
	/// {@code UUID}.  The bit layout described above is valid only for a {@code
	/// UUID} with a variant value of 2, which indicates the Leach-Salz variant.
	/// 
	/// </para>
	/// <para> The version field holds a value that describes the type of this {@code
	/// UUID}.  There are four different basic types of UUIDs: time-based, DCE
	/// security, name-based, and randomly generated UUIDs.  These types have a
	/// version value of 1, 2, 3 and 4, respectively.
	/// 
	/// </para>
	/// <para> For more information including algorithms used to create {@code UUID}s,
	/// see <a href="http://www.ietf.org/rfc/rfc4122.txt"> <i>RFC&nbsp;4122: A
	/// Universally Unique IDentifier (UUID) URN Namespace</i></a>, section 4.2
	/// &quot;Algorithms for Creating a Time-Based UUID&quot;.
	/// 
	/// @since   1.5
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class UUID : Comparable<UUID>
	{

		/// <summary>
		/// Explicit serialVersionUID for interoperability.
		/// </summary>
		private const long SerialVersionUID = -4856846361193249489L;

		/*
		 * The most significant 64 bits of this UUID.
		 *
		 * @serial
		 */
		private readonly long MostSigBits;

		/*
		 * The least significant 64 bits of this UUID.
		 *
		 * @serial
		 */
		private readonly long LeastSigBits;

		/*
		 * The random number generator used by this class to create random
		 * based UUIDs. In a holder class to defer initialization until needed.
		 */
		private class Holder
		{
			internal static readonly SecureRandom NumberGenerator = new SecureRandom();
		}

		// Constructors and Factories

		/*
		 * Private constructor which uses a byte array to construct the new UUID.
		 */
		private UUID(sbyte[] data)
		{
			long msb = 0;
			long lsb = 0;
			Debug.Assert(data.Length == 16, "data must be 16 bytes in length");
			for (int i = 0; i < 8; i++)
			{
				msb = (msb << 8) | (data[i] & 0xff);
			}
			for (int i = 8; i < 16; i++)
			{
				lsb = (lsb << 8) | (data[i] & 0xff);
			}
			this.MostSigBits = msb;
			this.LeastSigBits = lsb;
		}

		/// <summary>
		/// Constructs a new {@code UUID} using the specified data.  {@code
		/// mostSigBits} is used for the most significant 64 bits of the {@code
		/// UUID} and {@code leastSigBits} becomes the least significant 64 bits of
		/// the {@code UUID}.
		/// </summary>
		/// <param name="mostSigBits">
		///         The most significant bits of the {@code UUID}
		/// </param>
		/// <param name="leastSigBits">
		///         The least significant bits of the {@code UUID} </param>
		public UUID(long mostSigBits, long leastSigBits)
		{
			this.MostSigBits = mostSigBits;
			this.LeastSigBits = leastSigBits;
		}

		/// <summary>
		/// Static factory to retrieve a type 4 (pseudo randomly generated) UUID.
		/// 
		/// The {@code UUID} is generated using a cryptographically strong pseudo
		/// random number generator.
		/// </summary>
		/// <returns>  A randomly generated {@code UUID} </returns>
		public static UUID RandomUUID()
		{
			SecureRandom ng = Holder.NumberGenerator;

			sbyte[] randomBytes = new sbyte[16];
			ng.NextBytes(randomBytes);
			randomBytes[6] &= 0x0f; // clear version
			randomBytes[6] |= 0x40; // set to version 4
			randomBytes[8] &= 0x3f; // clear variant
			randomBytes[8] |= unchecked((sbyte)0x80); // set to IETF variant
			return new UUID(randomBytes);
		}

		/// <summary>
		/// Static factory to retrieve a type 3 (name based) {@code UUID} based on
		/// the specified byte array.
		/// </summary>
		/// <param name="name">
		///         A byte array to be used to construct a {@code UUID}
		/// </param>
		/// <returns>  A {@code UUID} generated from the specified array </returns>
		public static UUID NameUUIDFromBytes(sbyte[] name)
		{
			MessageDigest md;
			try
			{
				md = MessageDigest.GetInstance("MD5");
			}
			catch (NoSuchAlgorithmException nsae)
			{
				throw new InternalError("MD5 not supported", nsae);
			}
			sbyte[] md5Bytes = md.Digest(name);
			md5Bytes[6] &= 0x0f; // clear version
			md5Bytes[6] |= 0x30; // set to version 3
			md5Bytes[8] &= 0x3f; // clear variant
			md5Bytes[8] |= unchecked((sbyte)0x80); // set to IETF variant
			return new UUID(md5Bytes);
		}

		/// <summary>
		/// Creates a {@code UUID} from the string standard representation as
		/// described in the <seealso cref="#toString"/> method.
		/// </summary>
		/// <param name="name">
		///         A string that specifies a {@code UUID}
		/// </param>
		/// <returns>  A {@code UUID} with the specified value
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If name does not conform to the string representation as
		///          described in <seealso cref="#toString"/>
		///  </exception>
		public static UUID FromString(String name)
		{
			String[] components = name.Split("-");
			if (components.Length != 5)
			{
				throw new IllegalArgumentException("Invalid UUID string: " + name);
			}
			for (int i = 0; i < 5; i++)
			{
				components[i] = "0x" + components[i];
			}

			long mostSigBits = Long.Decode(components[0]).LongValue();
			mostSigBits <<= 16;
			mostSigBits |= Long.Decode(components[1]).LongValue();
			mostSigBits <<= 16;
			mostSigBits |= Long.Decode(components[2]).LongValue();

			long leastSigBits = Long.Decode(components[3]).LongValue();
			leastSigBits <<= 48;
			leastSigBits |= Long.Decode(components[4]).LongValue();

			return new UUID(mostSigBits, leastSigBits);
		}

		// Field Accessor Methods

		/// <summary>
		/// Returns the least significant 64 bits of this UUID's 128 bit value.
		/// </summary>
		/// <returns>  The least significant 64 bits of this UUID's 128 bit value </returns>
		public long LeastSignificantBits
		{
			get
			{
				return LeastSigBits;
			}
		}

		/// <summary>
		/// Returns the most significant 64 bits of this UUID's 128 bit value.
		/// </summary>
		/// <returns>  The most significant 64 bits of this UUID's 128 bit value </returns>
		public long MostSignificantBits
		{
			get
			{
				return MostSigBits;
			}
		}

		/// <summary>
		/// The version number associated with this {@code UUID}.  The version
		/// number describes how this {@code UUID} was generated.
		/// 
		/// The version number has the following meaning:
		/// <ul>
		/// <li>1    Time-based UUID
		/// <li>2    DCE security UUID
		/// <li>3    Name-based UUID
		/// <li>4    Randomly generated UUID
		/// </ul>
		/// </summary>
		/// <returns>  The version number of this {@code UUID} </returns>
		public int Version()
		{
			// Version is bits masked by 0x000000000000F000 in MS long
			return (int)((MostSigBits >> 12) & 0x0f);
		}

		/// <summary>
		/// The variant number associated with this {@code UUID}.  The variant
		/// number describes the layout of the {@code UUID}.
		/// 
		/// The variant number has the following meaning:
		/// <ul>
		/// <li>0    Reserved for NCS backward compatibility
		/// <li>2    <a href="http://www.ietf.org/rfc/rfc4122.txt">IETF&nbsp;RFC&nbsp;4122</a>
		/// (Leach-Salz), used by this class
		/// <li>6    Reserved, Microsoft Corporation backward compatibility
		/// <li>7    Reserved for future definition
		/// </ul>
		/// </summary>
		/// <returns>  The variant number of this {@code UUID} </returns>
		public int Variant()
		{
			// This field is composed of a varying number of bits.
			// 0    -    -    Reserved for NCS backward compatibility
			// 1    0    -    The IETF aka Leach-Salz variant (used by this class)
			// 1    1    0    Reserved, Microsoft backward compatibility
			// 1    1    1    Reserved for future definition.
			return (int)(((long)((ulong)LeastSigBits >> (64 - ((long)((ulong)LeastSigBits >> 62))))) & (LeastSigBits >> 63));
		}

		/// <summary>
		/// The timestamp value associated with this UUID.
		/// 
		/// <para> The 60 bit timestamp value is constructed from the time_low,
		/// time_mid, and time_hi fields of this {@code UUID}.  The resulting
		/// timestamp is measured in 100-nanosecond units since midnight,
		/// October 15, 1582 UTC.
		/// 
		/// </para>
		/// <para> The timestamp value is only meaningful in a time-based UUID, which
		/// has version type 1.  If this {@code UUID} is not a time-based UUID then
		/// this method throws UnsupportedOperationException.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException">
		///         If this UUID is not a version 1 UUID </exception>
		/// <returns> The timestamp of this {@code UUID}. </returns>
		public long Timestamp()
		{
			if (Version() != 1)
			{
				throw new UnsupportedOperationException("Not a time-based UUID");
			}

			return (MostSigBits & 0x0FFFL) << 48 | ((MostSigBits >> 16) & 0x0FFFFL) << 32 | (long)((ulong)MostSigBits >> 32);
		}

		/// <summary>
		/// The clock sequence value associated with this UUID.
		/// 
		/// <para> The 14 bit clock sequence value is constructed from the clock
		/// sequence field of this UUID.  The clock sequence field is used to
		/// guarantee temporal uniqueness in a time-based UUID.
		/// 
		/// </para>
		/// <para> The {@code clockSequence} value is only meaningful in a time-based
		/// UUID, which has version type 1.  If this UUID is not a time-based UUID
		/// then this method throws UnsupportedOperationException.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The clock sequence of this {@code UUID}
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this UUID is not a version 1 UUID </exception>
		public int ClockSequence()
		{
			if (Version() != 1)
			{
				throw new UnsupportedOperationException("Not a time-based UUID");
			}

			return (int)((int)((uint)(LeastSigBits & 0x3FFF000000000000L) >> 48));
		}

		/// <summary>
		/// The node value associated with this UUID.
		/// 
		/// <para> The 48 bit node value is constructed from the node field of this
		/// UUID.  This field is intended to hold the IEEE 802 address of the machine
		/// that generated this UUID to guarantee spatial uniqueness.
		/// 
		/// </para>
		/// <para> The node value is only meaningful in a time-based UUID, which has
		/// version type 1.  If this UUID is not a time-based UUID then this method
		/// throws UnsupportedOperationException.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The node value of this {@code UUID}
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this UUID is not a version 1 UUID </exception>
		public long Node()
		{
			if (Version() != 1)
			{
				throw new UnsupportedOperationException("Not a time-based UUID");
			}

			return LeastSigBits & 0x0000FFFFFFFFFFFFL;
		}

		// Object Inherited Methods

		/// <summary>
		/// Returns a {@code String} object representing this {@code UUID}.
		/// 
		/// <para> The UUID string representation is as described by this BNF:
		/// <blockquote><pre>
		/// {@code
		/// UUID                   = <time_low> "-" <time_mid> "-"
		///                          <time_high_and_version> "-"
		///                          <variant_and_sequence> "-"
		///                          <node>
		/// time_low               = 4*<hexOctet>
		/// time_mid               = 2*<hexOctet>
		/// time_high_and_version  = 2*<hexOctet>
		/// variant_and_sequence   = 2*<hexOctet>
		/// node                   = 6*<hexOctet>
		/// hexOctet               = <hexDigit><hexDigit>
		/// hexDigit               =
		///       "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
		///       | "a" | "b" | "c" | "d" | "e" | "f"
		///       | "A" | "B" | "C" | "D" | "E" | "F"
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A string representation of this {@code UUID} </returns>
		public override String ToString()
		{
			return (Digits(MostSigBits >> 32, 8) + "-" + Digits(MostSigBits >> 16, 4) + "-" + Digits(MostSigBits, 4) + "-" + Digits(LeastSigBits >> 48, 4) + "-" + Digits(LeastSigBits, 12));
		}

		/// <summary>
		/// Returns val represented by the specified number of hex digits. </summary>
		private static String Digits(long val, int digits)
		{
			long hi = 1L << (digits * 4);
			return (hi | (val & (hi - 1))).ToString("x").Substring(1);
		}

		/// <summary>
		/// Returns a hash code for this {@code UUID}.
		/// </summary>
		/// <returns>  A hash code value for this {@code UUID} </returns>
		public override int HashCode()
		{
			long hilo = MostSigBits ^ LeastSigBits;
			return ((int)(hilo >> 32)) ^ (int) hilo;
		}

		/// <summary>
		/// Compares this object to the specified object.  The result is {@code
		/// true} if and only if the argument is not {@code null}, is a {@code UUID}
		/// object, has the same variant, and contains the same value, bit for bit,
		/// as this {@code UUID}.
		/// </summary>
		/// <param name="obj">
		///         The object to be compared
		/// </param>
		/// <returns>  {@code true} if the objects are the same; {@code false}
		///          otherwise </returns>
		public override bool Equals(Object obj)
		{
			if ((null == obj) || (obj.GetType() != typeof(UUID)))
			{
				return false;
			}
			UUID id = (UUID)obj;
			return (MostSigBits == id.MostSigBits && LeastSigBits == id.LeastSigBits);
		}

		// Comparison Operations

		/// <summary>
		/// Compares this UUID with the specified UUID.
		/// 
		/// <para> The first of two UUIDs is greater than the second if the most
		/// significant field in which the UUIDs differ is greater for the first
		/// UUID.
		/// 
		/// </para>
		/// </summary>
		/// <param name="val">
		///         {@code UUID} to which this {@code UUID} is to be compared
		/// </param>
		/// <returns>  -1, 0 or 1 as this {@code UUID} is less than, equal to, or
		///          greater than {@code val}
		///  </returns>
		public int CompareTo(UUID val)
		{
			// The ordering is intentionally set up so that the UUIDs
			// can simply be numerically compared as two numbers
			return (this.MostSigBits < val.MostSigBits ? - 1 : (this.MostSigBits > val.MostSigBits ? 1 : (this.LeastSigBits < val.LeastSigBits ? - 1 : (this.LeastSigBits > val.LeastSigBits ? 1 : 0))));
		}
	}

}