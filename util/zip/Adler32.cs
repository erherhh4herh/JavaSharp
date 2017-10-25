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
	/// A class that can be used to compute the Adler-32 checksum of a data
	/// stream. An Adler-32 checksum is almost as reliable as a CRC-32 but
	/// can be computed much faster.
	/// 
	/// <para> Passing a {@code null} argument to a method in this class will cause
	/// a <seealso cref="NullPointerException"/> to be thrown.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=         Checksum
	/// @author      David Connelly </seealso>
	public class Adler32 : Checksum
	{

		private int Adler = 1;

		/// <summary>
		/// Creates a new Adler32 object.
		/// </summary>
		public Adler32()
		{
		}

		/// <summary>
		/// Updates the checksum with the specified byte (the low eight
		/// bits of the argument b).
		/// </summary>
		/// <param name="b"> the byte to update the checksum with </param>
		public virtual void Update(int b)
		{
			Adler = update(Adler, b);
		}

		/// <summary>
		/// Updates the checksum with the specified array of bytes.
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
			Adler = updateBytes(Adler, b, off, len);
		}

		/// <summary>
		/// Updates the checksum with the specified array of bytes.
		/// </summary>
		/// <param name="b"> the byte array to update the checksum with </param>
		public virtual void Update(sbyte[] b)
		{
			Adler = updateBytes(Adler, b, 0, b.Length);
		}


		/// <summary>
		/// Updates the checksum with the bytes from the specified buffer.
		/// 
		/// The checksum is updated using
		/// buffer.<seealso cref="java.nio.Buffer#remaining() remaining()"/>
		/// bytes starting at
		/// buffer.<seealso cref="java.nio.Buffer#position() position()"/>
		/// Upon return, the buffer's position will be updated to its
		/// limit; its limit will not have been changed.
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
				Adler = updateByteBuffer(Adler, ((DirectBuffer)buffer).address(), pos, rem);
			}
			else if (buffer.HasArray())
			{
				Adler = updateBytes(Adler, buffer.Array(), pos + buffer.ArrayOffset(), rem);
			}
			else
			{
				sbyte[] b = new sbyte[rem];
				buffer.Get(b);
				Adler = updateBytes(Adler, b, 0, b.Length);
			}
			buffer.Position(limit);
		}

		/// <summary>
		/// Resets the checksum to initial value.
		/// </summary>
		public virtual void Reset()
		{
			Adler = 1;
		}

		/// <summary>
		/// Returns the checksum value.
		/// </summary>
		public virtual long Value
		{
			get
			{
				return (long)Adler & 0xffffffffL;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int update(int adler, int b);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int updateBytes(int adler, sbyte[] b, int off, int len);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int updateByteBuffer(int adler, long addr, int off, int len);
	}

}