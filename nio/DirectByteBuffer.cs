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

// -- This file was mechanically generated: Do not edit! -- //

namespace java.nio
{

	using Cleaner = sun.misc.Cleaner;
	using Unsafe = sun.misc.Unsafe;
	using VM = sun.misc.VM;
	using DirectBuffer = sun.nio.ch.DirectBuffer;


	internal class DirectByteBuffer : MappedByteBuffer, DirectBuffer
	{



		// Cached unsafe-access object
		protected internal static readonly Unsafe @unsafe = Bits.@unsafe();

		// Cached array base offset
		private static readonly long ArrayBaseOffset = (long)@unsafe.arrayBaseOffset(typeof(sbyte[]));

		// Cached unaligned-access capability
		protected internal static readonly bool Unaligned = Bits.Unaligned();

		// Base address, used in all indexing calculations
		// NOTE: moved up to Buffer.java for speed in JNI GetDirectBufferAddress
		//    protected long address;

		// An object attached to this buffer. If this buffer is a view of another
		// buffer then we use this field to keep a reference to that buffer to
		// ensure that its memory isn't freed before we are done with it.
		private readonly Object Att;

		public virtual Object Attachment()
		{
			return Att;
		}



		private class Deallocator : Runnable
		{

			internal static Unsafe @unsafe = Unsafe.Unsafe;

			internal long Address;
			internal long Size;
			internal int Capacity;

			internal Deallocator(long address, long size, int capacity)
			{
				assert(address != 0);
				this.Address = address;
				this.Size = size;
				this.Capacity = capacity;
			}

			public virtual void Run()
			{
				if (Address == 0)
				{
					// Paranoia
					return;
				}
				@unsafe.freeMemory(Address);
				Address = 0;
				Bits.UnreserveMemory(Size, Capacity);
			}

		}

		private readonly Cleaner Cleaner_Renamed;

		public virtual Cleaner Cleaner()
		{
			return Cleaner_Renamed;
		}











		// Primary constructor
		//
		internal DirectByteBuffer(int cap) : base(-1, 0, cap, cap) // package-private
		{

			bool pa = VM.DirectMemoryPageAligned;
			int ps = Bits.PageSize();
			long size = System.Math.Max(1L, (long)cap + (pa ? ps : 0));
			Bits.ReserveMemory(size, cap);

			long @base = 0;
			try
			{
				@base = @unsafe.allocateMemory(size);
			}
			catch (OutOfMemoryError x)
			{
				Bits.UnreserveMemory(size, cap);
				throw x;
			}
			@unsafe.setMemory(@base, size, (sbyte) 0);
			if (pa && (@base % ps != 0))
			{
				// Round up to page boundary
				Address = @base + ps - (@base & (ps - 1));
			}
			else
			{
				Address = @base;
			}
			Cleaner_Renamed = Cleaner.create(this, new Deallocator(@base, size, cap));
			Att = null;



		}



		// Invoked to construct a direct ByteBuffer referring to the block of
		// memory. A given arbitrary object may also be attached to the buffer.
		//
		internal DirectByteBuffer(long addr, int cap, Object ob) : base(-1, 0, cap, cap)
		{
			Address = addr;
			Cleaner_Renamed = null;
			Att = ob;
		}


		// Invoked only by JNI: NewDirectByteBuffer(void*, long)
		//
		private DirectByteBuffer(long addr, int cap) : base(-1, 0, cap, cap)
		{
			Address = addr;
			Cleaner_Renamed = null;
			Att = null;
		}



		// For memory-mapped buffers -- invoked by FileChannelImpl via reflection
		//
		protected internal DirectByteBuffer(int cap, long addr, FileDescriptor fd, Runnable unmapper) : base(-1, 0, cap, cap, fd)
		{

			Address = addr;
			Cleaner_Renamed = Cleaner.create(this, unmapper);
			Att = null;



		}



		// For duplicates and slices
		//
		internal DirectByteBuffer(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap) // package-private
		{

			Address = db.address() + off;

			Cleaner_Renamed = null;

			Att = db;



		}

		public override ByteBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 0);
			assert(off >= 0);
			return new DirectByteBuffer(this, -1, 0, rem, rem, off);
		}

		public override ByteBuffer Duplicate()
		{
			return new DirectByteBuffer(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override ByteBuffer AsReadOnlyBuffer()
		{

			return new DirectByteBufferR(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);



		}



		public virtual long Address()
		{
			return Address;
		}

		private long Ix(int i)
		{
			return Address + ((long)i << 0);
		}

		public override sbyte Get()
		{
			return ((@unsafe.getByte(Ix(NextGetIndex()))));
		}

		public override sbyte Get(int i)
		{
			return ((@unsafe.getByte(Ix(CheckIndex(i)))));
		}







		public override ByteBuffer Get(sbyte[] dst, int offset, int length)
		{

			if (((long)length << 0) > Bits.JNI_COPY_TO_ARRAY_THRESHOLD)
			{
				CheckBounds(offset, length, dst.Length);
				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);
				if (length > rem)
				{
					throw new BufferUnderflowException();
				}








					Bits.CopyToArray(Ix(pos), dst, ArrayBaseOffset, (long)offset << 0, (long)length << 0);
				Position(pos + length);
			}
			else
			{
				base.Get(dst, offset, length);
			}
			return this;



		}



		public override ByteBuffer Put(sbyte x)
		{

			@unsafe.putByte(Ix(NextPutIndex()), ((x)));
			return this;



		}

		public override ByteBuffer Put(int i, sbyte x)
		{

			@unsafe.putByte(Ix(CheckIndex(i)), ((x)));
			return this;



		}

		public override ByteBuffer Put(ByteBuffer src)
		{

			if (src is DirectByteBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				DirectByteBuffer sb = (DirectByteBuffer)src;

				int spos = sb.Position();
				int slim = sb.Limit();
				assert(spos <= slim);
				int srem = (spos <= slim ? slim - spos : 0);

				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);

				if (srem > rem)
				{
					throw new BufferOverflowException();
				}
				@unsafe.copyMemory(sb.Ix(spos), Ix(pos), (long)srem << 0);
				sb.Position(spos + srem);
				Position(pos + srem);
			}
			else if (src.Hb != null)
			{

				int spos = src.Position();
				int slim = src.Limit();
				assert(spos <= slim);
				int srem = (spos <= slim ? slim - spos : 0);

				Put(src.Hb, src.Offset + spos, srem);
				src.Position(spos + srem);

			}
			else
			{
				base.Put(src);
			}
			return this;



		}

		public override ByteBuffer Put(sbyte[] src, int offset, int length)
		{

			if (((long)length << 0) > Bits.JNI_COPY_FROM_ARRAY_THRESHOLD)
			{
				CheckBounds(offset, length, src.Length);
				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);
				if (length > rem)
				{
					throw new BufferOverflowException();
				}









					Bits.CopyFromArray(src, ArrayBaseOffset, (long)offset << 0, Ix(pos), (long)length << 0);
				Position(pos + length);
			}
			else
			{
				base.Put(src, offset, length);
			}
			return this;



		}

		public override ByteBuffer Compact()
		{

			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);

			@unsafe.copyMemory(Ix(pos), Ix(0), (long)rem << 0);
			Position(rem);
			Limit(Capacity());
			DiscardMark();
			return this;



		}

		public override bool Direct
		{
			get
			{
				return true;
			}
		}

		public override bool ReadOnly
		{
			get
			{
				return false;
			}
		}
































































		internal override sbyte _get(int i) // package-private
		{
			return @unsafe.getByte(Address + i);
		}

		internal override void _put(int i, sbyte b) // package-private
		{

			@unsafe.putByte(Address + i, b);



		}




		private char GetChar(long a)
		{
			if (Unaligned)
			{
				char x = @unsafe.getChar(a);
				return (NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetChar(a, BigEndian);
		}

		public override char Char
		{
			get
			{
				return GetChar(Ix(NextGetIndex((1 << 1))));
			}
		}

		public override char GetChar(int i)
		{
			return GetChar(Ix(CheckIndex(i, (1 << 1))));
		}



		private ByteBuffer PutChar(long a, char x)
		{

			if (Unaligned)
			{
				char y = (x);
				@unsafe.putChar(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutChar(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutChar(char x)
		{

			PutChar(Ix(NextPutIndex((1 << 1))), x);
			return this;



		}

		public override ByteBuffer PutChar(int i, char x)
		{

			PutChar(Ix(CheckIndex(i, (1 << 1))), x);
			return this;



		}

		public override CharBuffer AsCharBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 1;
			if (!Unaligned && ((Address + off) % (1 << 1) != 0))
			{
				return (BigEndian ? (CharBuffer)(new ByteBufferAsCharBufferB(this, -1, 0, size, size, off)) : (CharBuffer)(new ByteBufferAsCharBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (CharBuffer)(new DirectCharBufferU(this, -1, 0, size, size, off)) : (CharBuffer)(new DirectCharBufferS(this, -1, 0, size, size, off)));
			}
		}




		private short GetShort(long a)
		{
			if (Unaligned)
			{
				short x = @unsafe.getShort(a);
				return (NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetShort(a, BigEndian);
		}

		public override short Short
		{
			get
			{
				return GetShort(Ix(NextGetIndex((1 << 1))));
			}
		}

		public override short GetShort(int i)
		{
			return GetShort(Ix(CheckIndex(i, (1 << 1))));
		}



		private ByteBuffer PutShort(long a, short x)
		{

			if (Unaligned)
			{
				short y = (x);
				@unsafe.putShort(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutShort(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutShort(short x)
		{

			PutShort(Ix(NextPutIndex((1 << 1))), x);
			return this;



		}

		public override ByteBuffer PutShort(int i, short x)
		{

			PutShort(Ix(CheckIndex(i, (1 << 1))), x);
			return this;



		}

		public override ShortBuffer AsShortBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 1;
			if (!Unaligned && ((Address + off) % (1 << 1) != 0))
			{
				return (BigEndian ? (ShortBuffer)(new ByteBufferAsShortBufferB(this, -1, 0, size, size, off)) : (ShortBuffer)(new ByteBufferAsShortBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (ShortBuffer)(new DirectShortBufferU(this, -1, 0, size, size, off)) : (ShortBuffer)(new DirectShortBufferS(this, -1, 0, size, size, off)));
			}
		}




		private int GetInt(long a)
		{
			if (Unaligned)
			{
				int x = @unsafe.getInt(a);
				return (NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetInt(a, BigEndian);
		}

		public override int Int
		{
			get
			{
				return GetInt(Ix(NextGetIndex((1 << 2))));
			}
		}

		public override int GetInt(int i)
		{
			return GetInt(Ix(CheckIndex(i, (1 << 2))));
		}



		private ByteBuffer PutInt(long a, int x)
		{

			if (Unaligned)
			{
				int y = (x);
				@unsafe.putInt(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutInt(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutInt(int x)
		{

			PutInt(Ix(NextPutIndex((1 << 2))), x);
			return this;



		}

		public override ByteBuffer PutInt(int i, int x)
		{

			PutInt(Ix(CheckIndex(i, (1 << 2))), x);
			return this;



		}

		public override IntBuffer AsIntBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 2;
			if (!Unaligned && ((Address + off) % (1 << 2) != 0))
			{
				return (BigEndian ? (IntBuffer)(new ByteBufferAsIntBufferB(this, -1, 0, size, size, off)) : (IntBuffer)(new ByteBufferAsIntBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (IntBuffer)(new DirectIntBufferU(this, -1, 0, size, size, off)) : (IntBuffer)(new DirectIntBufferS(this, -1, 0, size, size, off)));
			}
		}




		private long GetLong(long a)
		{
			if (Unaligned)
			{
				long x = @unsafe.getLong(a);
				return (NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetLong(a, BigEndian);
		}

		public override long Long
		{
			get
			{
				return GetLong(Ix(NextGetIndex((1 << 3))));
			}
		}

		public override long GetLong(int i)
		{
			return GetLong(Ix(CheckIndex(i, (1 << 3))));
		}



		private ByteBuffer PutLong(long a, long x)
		{

			if (Unaligned)
			{
				long y = (x);
				@unsafe.putLong(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutLong(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutLong(long x)
		{

			PutLong(Ix(NextPutIndex((1 << 3))), x);
			return this;



		}

		public override ByteBuffer PutLong(int i, long x)
		{

			PutLong(Ix(CheckIndex(i, (1 << 3))), x);
			return this;



		}

		public override LongBuffer AsLongBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 3;
			if (!Unaligned && ((Address + off) % (1 << 3) != 0))
			{
				return (BigEndian ? (LongBuffer)(new ByteBufferAsLongBufferB(this, -1, 0, size, size, off)) : (LongBuffer)(new ByteBufferAsLongBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (LongBuffer)(new DirectLongBufferU(this, -1, 0, size, size, off)) : (LongBuffer)(new DirectLongBufferS(this, -1, 0, size, size, off)));
			}
		}




		private float GetFloat(long a)
		{
			if (Unaligned)
			{
				int x = @unsafe.getInt(a);
				return Float.intBitsToFloat(NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetFloat(a, BigEndian);
		}

		public override float Float
		{
			get
			{
				return GetFloat(Ix(NextGetIndex((1 << 2))));
			}
		}

		public override float GetFloat(int i)
		{
			return GetFloat(Ix(CheckIndex(i, (1 << 2))));
		}



		private ByteBuffer PutFloat(long a, float x)
		{

			if (Unaligned)
			{
				int y = Float.floatToRawIntBits(x);
				@unsafe.putInt(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutFloat(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutFloat(float x)
		{

			PutFloat(Ix(NextPutIndex((1 << 2))), x);
			return this;



		}

		public override ByteBuffer PutFloat(int i, float x)
		{

			PutFloat(Ix(CheckIndex(i, (1 << 2))), x);
			return this;



		}

		public override FloatBuffer AsFloatBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 2;
			if (!Unaligned && ((Address + off) % (1 << 2) != 0))
			{
				return (BigEndian ? (FloatBuffer)(new ByteBufferAsFloatBufferB(this, -1, 0, size, size, off)) : (FloatBuffer)(new ByteBufferAsFloatBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (FloatBuffer)(new DirectFloatBufferU(this, -1, 0, size, size, off)) : (FloatBuffer)(new DirectFloatBufferS(this, -1, 0, size, size, off)));
			}
		}




		private double GetDouble(long a)
		{
			if (Unaligned)
			{
				long x = @unsafe.getLong(a);
				return Double.longBitsToDouble(NativeByteOrder ? x : Bits.Swap(x));
			}
			return Bits.GetDouble(a, BigEndian);
		}

		public override double Double
		{
			get
			{
				return GetDouble(Ix(NextGetIndex((1 << 3))));
			}
		}

		public override double GetDouble(int i)
		{
			return GetDouble(Ix(CheckIndex(i, (1 << 3))));
		}



		private ByteBuffer PutDouble(long a, double x)
		{

			if (Unaligned)
			{
				long y = Double.doubleToRawLongBits(x);
				@unsafe.putLong(a, (NativeByteOrder ? y : Bits.Swap(y)));
			}
			else
			{
				Bits.PutDouble(a, x, BigEndian);
			}
			return this;



		}

		public override ByteBuffer PutDouble(double x)
		{

			PutDouble(Ix(NextPutIndex((1 << 3))), x);
			return this;



		}

		public override ByteBuffer PutDouble(int i, double x)
		{

			PutDouble(Ix(CheckIndex(i, (1 << 3))), x);
			return this;



		}

		public override DoubleBuffer AsDoubleBuffer()
		{
			int off = this.Position();
			int lim = this.Limit();
			assert(off <= lim);
			int rem = (off <= lim ? lim - off : 0);

			int size = rem >> 3;
			if (!Unaligned && ((Address + off) % (1 << 3) != 0))
			{
				return (BigEndian ? (DoubleBuffer)(new ByteBufferAsDoubleBufferB(this, -1, 0, size, size, off)) : (DoubleBuffer)(new ByteBufferAsDoubleBufferL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (DoubleBuffer)(new DirectDoubleBufferU(this, -1, 0, size, size, off)) : (DoubleBuffer)(new DirectDoubleBufferS(this, -1, 0, size, size, off)));
			}
		}

	}

}