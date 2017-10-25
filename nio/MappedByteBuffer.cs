﻿using System.Runtime.InteropServices;

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

namespace java.nio
{

	using Unsafe = sun.misc.Unsafe;


	/// <summary>
	/// A direct byte buffer whose content is a memory-mapped region of a file.
	/// 
	/// <para> Mapped byte buffers are created via the {@link
	/// java.nio.channels.FileChannel#map FileChannel.map} method.  This class
	/// extends the <seealso cref="ByteBuffer"/> class with operations that are specific to
	/// memory-mapped file regions.
	/// 
	/// </para>
	/// <para> A mapped byte buffer and the file mapping that it represents remain
	/// valid until the buffer itself is garbage-collected.
	/// 
	/// </para>
	/// <para> The content of a mapped byte buffer can change at any time, for example
	/// if the content of the corresponding region of the mapped file is changed by
	/// this program or another.  Whether or not such changes occur, and when they
	/// occur, is operating-system dependent and therefore unspecified.
	/// 
	/// </para>
	/// <a name="inaccess"></a><para> All or part of a mapped byte buffer may become
	/// inaccessible at any time, for example if the mapped file is truncated.  An
	/// attempt to access an inaccessible region of a mapped byte buffer will not
	/// change the buffer's content and will cause an unspecified exception to be
	/// thrown either at the time of the access or at some later time.  It is
	/// therefore strongly recommended that appropriate precautions be taken to
	/// avoid the manipulation of a mapped file by this program, or by a
	/// concurrently running program, except to read or write the file's content.
	/// 
	/// </para>
	/// <para> Mapped byte buffers otherwise behave no differently than ordinary direct
	/// byte buffers. </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public abstract class MappedByteBuffer : ByteBuffer
	{

		// This is a little bit backwards: By rights MappedByteBuffer should be a
		// subclass of DirectByteBuffer, but to keep the spec clear and simple, and
		// for optimization purposes, it's easier to do it the other way around.
		// This works because DirectByteBuffer is a package-private class.

		// For mapped buffers, a FileDescriptor that may be used for mapping
		// operations if valid; null if the buffer is not mapped.
		private readonly FileDescriptor Fd;

		// This should only be invoked by the DirectByteBuffer constructors
		//
		internal MappedByteBuffer(int mark, int pos, int lim, int cap, FileDescriptor fd) : base(mark, pos, lim, cap) // package-private
		{
			this.Fd = fd;
		}

		internal MappedByteBuffer(int mark, int pos, int lim, int cap) : base(mark, pos, lim, cap) // package-private
		{
			this.Fd = null;
		}

		private void CheckMapped()
		{
			if (Fd == null)
			{
				// Can only happen if a luser explicitly casts a direct byte buffer
				throw new UnsupportedOperationException();
			}
		}

		// Returns the distance (in bytes) of the buffer from the page aligned address
		// of the mapping. Computed each time to avoid storing in every direct buffer.
		private long MappingOffset()
		{
			int ps = Bits.PageSize();
			long offset = Address % ps;
			return (offset >= 0) ? offset : (ps + offset);
		}

		private long MappingAddress(long mappingOffset)
		{
			return Address - mappingOffset;
		}

		private long MappingLength(long mappingOffset)
		{
			return (long)Capacity() + mappingOffset;
		}

		/// <summary>
		/// Tells whether or not this buffer's content is resident in physical
		/// memory.
		/// 
		/// <para> A return value of <tt>true</tt> implies that it is highly likely
		/// that all of the data in this buffer is resident in physical memory and
		/// may therefore be accessed without incurring any virtual-memory page
		/// faults or I/O operations.  A return value of <tt>false</tt> does not
		/// necessarily imply that the buffer's content is not resident in physical
		/// memory.
		/// 
		/// </para>
		/// <para> The returned value is a hint, rather than a guarantee, because the
		/// underlying operating system may have paged out some of the buffer's data
		/// by the time that an invocation of this method returns.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if it is likely that this buffer's content
		///          is resident in physical memory </returns>
		public bool Loaded
		{
			get
			{
				CheckMapped();
				if ((Address == 0) || (Capacity() == 0))
				{
					return true;
				}
				long offset = MappingOffset();
				long length = MappingLength(offset);
				return isLoaded0(MappingAddress(offset), length, Bits.PageCount(length));
			}
		}

		// not used, but a potential target for a store, see load() for details.
		private static sbyte Unused;

		/// <summary>
		/// Loads this buffer's content into physical memory.
		/// 
		/// <para> This method makes a best effort to ensure that, when it returns,
		/// this buffer's content is resident in physical memory.  Invoking this
		/// method may cause some number of page faults and I/O operations to
		/// occur. </para>
		/// </summary>
		/// <returns>  This buffer </returns>
		public MappedByteBuffer Load()
		{
			CheckMapped();
			if ((Address == 0) || (Capacity() == 0))
			{
				return this;
			}
			long offset = MappingOffset();
			long length = MappingLength(offset);
			load0(MappingAddress(offset), length);

			// Read a byte from each page to bring it into memory. A checksum
			// is computed as we go along to prevent the compiler from otherwise
			// considering the loop as dead code.
			Unsafe @unsafe = Unsafe.Unsafe;
			int ps = Bits.PageSize();
			int count = Bits.PageCount(length);
			long a = MappingAddress(offset);
			sbyte x = 0;
			for (int i = 0; i < count; i++)
			{
				x ^= @unsafe.getByte(a);
				a += ps;
			}
			if (Unused != 0)
			{
				Unused = x;
			}

			return this;
		}

		/// <summary>
		/// Forces any changes made to this buffer's content to be written to the
		/// storage device containing the mapped file.
		/// 
		/// <para> If the file mapped into this buffer resides on a local storage
		/// device then when this method returns it is guaranteed that all changes
		/// made to the buffer since it was created, or since this method was last
		/// invoked, will have been written to that device.
		/// 
		/// </para>
		/// <para> If the file does not reside on a local device then no such guarantee
		/// is made.
		/// 
		/// </para>
		/// <para> If this buffer was not mapped in read/write mode ({@link
		/// java.nio.channels.FileChannel.MapMode#READ_WRITE}) then invoking this
		/// method has no effect. </para>
		/// </summary>
		/// <returns>  This buffer </returns>
		public MappedByteBuffer Force()
		{
			CheckMapped();
			if ((Address != 0) && (Capacity() != 0))
			{
				long offset = MappingOffset();
				force0(Fd, MappingAddress(offset), MappingLength(offset));
			}
			return this;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern boolean isLoaded0(long address, long length, int pageCount);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void load0(long address, long length);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void force0(java.io.FileDescriptor fd, long address, long length);
	}

}