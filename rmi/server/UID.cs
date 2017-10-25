using System;
using System.Threading;

/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>UID</code> represents an identifier that is unique over time
	/// with respect to the host it is generated on, or one of 2<sup>16</sup>
	/// "well-known" identifiers.
	/// 
	/// <para>The <seealso cref="#UID()"/> constructor can be used to generate an
	/// identifier that is unique over time with respect to the host it is
	/// generated on.  The <seealso cref="#UID(short)"/> constructor can be used to
	/// create one of 2<sup>16</sup> well-known identifiers.
	/// 
	/// </para>
	/// <para>A <code>UID</code> instance contains three primitive values:
	/// <ul>
	/// <li><code>unique</code>, an <code>int</code> that uniquely identifies
	/// the VM that this <code>UID</code> was generated in, with respect to its
	/// host and at the time represented by the <code>time</code> value (an
	/// example implementation of the <code>unique</code> value would be a
	/// process identifier),
	///  or zero for a well-known <code>UID</code>
	/// <li><code>time</code>, a <code>long</code> equal to a time (as returned
	/// by <seealso cref="System#currentTimeMillis()"/>) at which the VM that this
	/// <code>UID</code> was generated in was alive,
	/// or zero for a well-known <code>UID</code>
	/// <li><code>count</code>, a <code>short</code> to distinguish
	/// <code>UID</code>s generated in the same VM with the same
	/// <code>time</code> value
	/// </ul>
	/// 
	/// </para>
	/// <para>An independently generated <code>UID</code> instance is unique
	/// over time with respect to the host it is generated on as long as
	/// the host requires more than one millisecond to reboot and its system
	/// clock is never set backward.  A globally unique identifier can be
	/// constructed by pairing a <code>UID</code> instance with a unique host
	/// identifier, such as an IP address.
	/// 
	/// @author      Ann Wollrath
	/// @author      Peter Jones
	/// @since       JDK1.1
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class UID
	{

		private static int HostUnique;
		private static bool HostUniqueSet = false;

		private static readonly Object @lock = new Object();
		private static long LastTime = DateTimeHelperClass.CurrentUnixTimeMillis();
		private static short LastCount = Short.MinValue;

		/// <summary>
		/// indicate compatibility with JDK 1.1.x version of class </summary>
		private const long SerialVersionUID = 1086053664494604050L;

		/// <summary>
		/// number that uniquely identifies the VM that this <code>UID</code>
		/// was generated in with respect to its host and at the given time
		/// @serial
		/// </summary>
		private readonly int Unique;

		/// <summary>
		/// a time (as returned by <seealso cref="System#currentTimeMillis()"/>) at which
		/// the VM that this <code>UID</code> was generated in was alive
		/// @serial
		/// </summary>
		private readonly long Time;

		/// <summary>
		/// 16-bit number to distinguish <code>UID</code> instances created
		/// in the same VM with the same time value
		/// @serial
		/// </summary>
		private readonly short Count;

		/// <summary>
		/// Generates a <code>UID</code> that is unique over time with
		/// respect to the host that it was generated on.
		/// </summary>
		public UID()
		{

			lock (@lock)
			{
				if (!HostUniqueSet)
				{
					HostUnique = (new SecureRandom()).NextInt();
					HostUniqueSet = true;
				}
				Unique = HostUnique;
				if (LastCount == Short.MaxValue)
				{
					bool interrupted = Thread.Interrupted();
					bool done = false;
					while (!done)
					{
						long now = DateTimeHelperClass.CurrentUnixTimeMillis();
						if (now == LastTime)
						{
							// wait for time to change
							try
							{
								Thread.Sleep(1);
							}
							catch (InterruptedException)
							{
								interrupted = true;
							}
						}
						else
						{
							// If system time has gone backwards increase
							// original by 1ms to maintain uniqueness
							LastTime = (now < LastTime) ? LastTime+1 : now;
							LastCount = Short.MinValue;
							done = true;
						}
					}
					if (interrupted)
					{
						Thread.CurrentThread.Interrupt();
					}
				}
				Time = LastTime;
				Count = LastCount++;
			}
		}

		/// <summary>
		/// Creates a "well-known" <code>UID</code>.
		/// 
		/// There are 2<sup>16</sup> possible such well-known ids.
		/// 
		/// <para>A <code>UID</code> created via this constructor will not
		/// clash with any <code>UID</code>s generated via the no-arg
		/// constructor.
		/// 
		/// </para>
		/// </summary>
		/// <param name="num"> number for well-known <code>UID</code> </param>
		public UID(short num)
		{
			Unique = 0;
			Time = 0;
			Count = num;
		}

		/// <summary>
		/// Constructs a <code>UID</code> given data read from a stream.
		/// </summary>
		private UID(int unique, long time, short count)
		{
			this.Unique = unique;
			this.Time = time;
			this.Count = count;
		}

		/// <summary>
		/// Returns the hash code value for this <code>UID</code>.
		/// </summary>
		/// <returns>  the hash code value for this <code>UID</code> </returns>
		public override int HashCode()
		{
			return (int) Time + (int) Count;
		}

		/// <summary>
		/// Compares the specified object with this <code>UID</code> for
		/// equality.
		/// 
		/// This method returns <code>true</code> if and only if the
		/// specified object is a <code>UID</code> instance with the same
		/// <code>unique</code>, <code>time</code>, and <code>count</code>
		/// values as this one.
		/// </summary>
		/// <param name="obj"> the object to compare this <code>UID</code> to
		/// </param>
		/// <returns>  <code>true</code> if the given object is equivalent to
		/// this one, and <code>false</code> otherwise </returns>
		public override bool Equals(Object obj)
		{
			if (obj is UID)
			{
				UID uid = (UID) obj;
				return (Unique == uid.Unique && Count == uid.Count && Time == uid.Time);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a string representation of this <code>UID</code>.
		/// </summary>
		/// <returns>  a string representation of this <code>UID</code> </returns>
		public override String ToString()
		{
			return Convert.ToString(Unique,16) + ":" + Convert.ToString(Time,16) + ":" + Convert.ToString(Count,16);
		}

		/// <summary>
		/// Marshals a binary representation of this <code>UID</code> to
		/// a <code>DataOutput</code> instance.
		/// 
		/// <para>Specifically, this method first invokes the given stream's
		/// <seealso cref="DataOutput#writeInt(int)"/> method with this <code>UID</code>'s
		/// <code>unique</code> value, then it invokes the stream's
		/// <seealso cref="DataOutput#writeLong(long)"/> method with this <code>UID</code>'s
		/// <code>time</code> value, and then it invokes the stream's
		/// <seealso cref="DataOutput#writeShort(int)"/> method with this <code>UID</code>'s
		/// <code>count</code> value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the <code>DataOutput</code> instance to write
		/// this <code>UID</code> to
		/// </param>
		/// <exception cref="IOException"> if an I/O error occurs while performing
		/// this operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(java.io.DataOutput out) throws java.io.IOException
		public void Write(DataOutput @out)
		{
			@out.WriteInt(Unique);
			@out.WriteLong(Time);
			@out.WriteShort(Count);
		}

		/// <summary>
		/// Constructs and returns a new <code>UID</code> instance by
		/// unmarshalling a binary representation from an
		/// <code>DataInput</code> instance.
		/// 
		/// <para>Specifically, this method first invokes the given stream's
		/// <seealso cref="DataInput#readInt()"/> method to read a <code>unique</code> value,
		/// then it invoke's the stream's
		/// <seealso cref="DataInput#readLong()"/> method to read a <code>time</code> value,
		/// then it invoke's the stream's
		/// <seealso cref="DataInput#readShort()"/> method to read a <code>count</code> value,
		/// and then it creates and returns a new <code>UID</code> instance
		/// that contains the <code>unique</code>, <code>time</code>, and
		/// <code>count</code> values that were read from the stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> the <code>DataInput</code> instance to read
		/// <code>UID</code> from
		/// </param>
		/// <returns>  unmarshalled <code>UID</code> instance
		/// </returns>
		/// <exception cref="IOException"> if an I/O error occurs while performing
		/// this operation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static UID read(java.io.DataInput in) throws java.io.IOException
		public static UID Read(DataInput @in)
		{
			int unique = @in.ReadInt();
			long time = @in.ReadLong();
			short count = @in.ReadShort();
			return new UID(unique, time, count);
		}
	}

}