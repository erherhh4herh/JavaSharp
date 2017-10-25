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


	internal class DirectByteBufferR : DirectByteBuffer, DirectBuffer
	{






































































		// Primary constructor
		//
		internal DirectByteBufferR(int cap) : base(cap) // package-private
		{

























		}

























		// For memory-mapped buffers -- invoked by FileChannelImpl via reflection
		//
		protected internal DirectByteBufferR(int cap, long addr, FileDescriptor fd, Runnable unmapper) : base(cap, addr, fd, unmapper)
		{







		}



		// For duplicates and slices
		//
		internal DirectByteBufferR(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(db, mark, pos, lim, cap, off) // package-private
		{









		}

		public override ByteBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 0);
			assert(off >= 0);
			return new DirectByteBufferR(this, -1, 0, rem, rem, off);
		}

		public override ByteBuffer Duplicate()
		{
			return new DirectByteBufferR(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override ByteBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}


























































		public override ByteBuffer Put(sbyte x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(int i, sbyte x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(ByteBuffer src)
		{




































			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(sbyte[] src, int offset, int length)
		{




























			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Compact()
		{












			throw new ReadOnlyBufferException();

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
				return true;
			}
		}
































































		internal override sbyte _get(int i) // package-private
		{
			return @unsafe.getByte(Address + i);
		}

		internal override void _put(int i, sbyte b) // package-private
		{



			throw new ReadOnlyBufferException();

		}






















		private ByteBuffer PutChar(long a, char x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutChar(char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutChar(int i, char x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (CharBuffer)(new ByteBufferAsCharBufferRB(this, -1, 0, size, size, off)) : (CharBuffer)(new ByteBufferAsCharBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (CharBuffer)(new DirectCharBufferRU(this, -1, 0, size, size, off)) : (CharBuffer)(new DirectCharBufferRS(this, -1, 0, size, size, off)));
			}
		}






















		private ByteBuffer PutShort(long a, short x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutShort(short x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutShort(int i, short x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (ShortBuffer)(new ByteBufferAsShortBufferRB(this, -1, 0, size, size, off)) : (ShortBuffer)(new ByteBufferAsShortBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (ShortBuffer)(new DirectShortBufferRU(this, -1, 0, size, size, off)) : (ShortBuffer)(new DirectShortBufferRS(this, -1, 0, size, size, off)));
			}
		}






















		private ByteBuffer PutInt(long a, int x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutInt(int x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutInt(int i, int x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (IntBuffer)(new ByteBufferAsIntBufferRB(this, -1, 0, size, size, off)) : (IntBuffer)(new ByteBufferAsIntBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (IntBuffer)(new DirectIntBufferRU(this, -1, 0, size, size, off)) : (IntBuffer)(new DirectIntBufferRS(this, -1, 0, size, size, off)));
			}
		}






















		private ByteBuffer PutLong(long a, long x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutLong(long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutLong(int i, long x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (LongBuffer)(new ByteBufferAsLongBufferRB(this, -1, 0, size, size, off)) : (LongBuffer)(new ByteBufferAsLongBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (LongBuffer)(new DirectLongBufferRU(this, -1, 0, size, size, off)) : (LongBuffer)(new DirectLongBufferRS(this, -1, 0, size, size, off)));
			}
		}






















		private ByteBuffer PutFloat(long a, float x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutFloat(float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutFloat(int i, float x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (FloatBuffer)(new ByteBufferAsFloatBufferRB(this, -1, 0, size, size, off)) : (FloatBuffer)(new ByteBufferAsFloatBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (FloatBuffer)(new DirectFloatBufferRU(this, -1, 0, size, size, off)) : (FloatBuffer)(new DirectFloatBufferRS(this, -1, 0, size, size, off)));
			}
		}






















		private ByteBuffer PutDouble(long a, double x)
		{









			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutDouble(double x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutDouble(int i, double x)
		{




			throw new ReadOnlyBufferException();

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
				return (BigEndian ? (DoubleBuffer)(new ByteBufferAsDoubleBufferRB(this, -1, 0, size, size, off)) : (DoubleBuffer)(new ByteBufferAsDoubleBufferRL(this, -1, 0, size, size, off)));
			}
			else
			{
				return (NativeByteOrder ? (DoubleBuffer)(new DirectDoubleBufferRU(this, -1, 0, size, size, off)) : (DoubleBuffer)(new DirectDoubleBufferRS(this, -1, 0, size, size, off)));
			}
		}

	}

}