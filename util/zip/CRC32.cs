using System.Runtime.InteropServices;

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

namespace java.util.zip
{

	using DirectBuffer = sun.nio.ch.DirectBuffer;

	/// <summary>
	/// A class that can be used to compute the CRC-32 of a data stream.
	/// 
	/// <para> Passing a {@code null} argument to a method in this class will cause
	/// a <seealso cref="NullPointerException"/> to be thrown.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=         Checksum
	/// @author      David Connelly </seealso>
	public class CRC32 : Checksum
	{
		private int Crc;

		/// <summary>
		/// Creates a new CRC32 object.
		/// </summary>
		public CRC32()
		{
		}


		/// <summary>
		/// Updates the CRC-32 checksum with the specified byte (the low
		/// eight bits of the argument b).
		/// </summary>
		/// <param name="b"> the byte to update the checksum with </param>
		public virtual void Update(int b)
		{
			Crc = update(Crc, b);
		}

		/// <summary>
		/// Updates the CRC-32 checksum with the specified array of bytes.
		/// </summary>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///          if {@code off} is negative, or {@code len} is negative,
		///          or {@code off+len} is greater than the length of the
		///          array {@code b} </exception>
		public virtual void Update(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			if (off < 0 || len < 0 || off > b.Length - len)
			{
				throw new ArrayIndexOutOfBoundsException();
			}
			Crc = updateBytes(Crc, b, off, len);
		}

		/// <summary>
		/// Updates the CRC-32 checksum with the specified array of bytes.
		/// </summary>
		/// <param name="b"> the array of bytes to update the checksum with </param>
		public virtual void Update(sbyte[] b)
		{
			Crc = updateBytes(Crc, b, 0, b.Length);
		}

		/// <summary>
		/// Updates the checksum with the bytes from the specified buffer.
		/// 
		/// The checksum is updated using
		/// buffer.<seealso cref="java.nio.Buffer#remaining() remaining()"/>
		/// bytes starting at
		/// buffer.<seealso cref="java.nio.Buffer#position() position()"/>
		/// Upon return, the buffer's position will
		/// be updated to its limit; its limit will not have been changed.
		/// </summary>
		/// <param name="buffer"> the ByteBuffer to update the checksum with
		/// @since 1.8 </param>
		public virtual void Update(ByteBuffer buffer)
		{
			int pos = buffer.Position();
			int limit = buffer.Limit();
			assert(pos <= limit);
			int rem = limit - pos;
			if (rem <= 0)
			{
				return;
			}
			if (buffer is DirectBuffer)
			{
				Crc = updateByteBuffer(Crc, ((DirectBuffer)buffer).address(), pos, rem);
			}
			else if (buffer.HasArray())
			{
				Crc = updateBytes(Crc, buffer.Array(), pos + buffer.ArrayOffset(), rem);
			}
			else
			{
				sbyte[] b = new sbyte[rem];
				buffer.Get(b);
				Crc = updateBytes(Crc, b, 0, b.Length);
			}
			buffer.Position(limit);
		}

		/// <summary>
		/// Resets CRC-32 to initial value.
		/// </summary>
		public virtual void Reset()
		{
			Crc = 0;
		}

		/// <summary>
		/// Returns CRC-32 value.
		/// </summary>
		public virtual long Value
		{
			get
			{
				return (long)Crc & 0xffffffffL;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int update(int crc, int b);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int updateBytes(int crc, sbyte[] b, int off, int len);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int updateByteBuffer(int adler, long addr, int off, int len);
	}

}