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


	/// 
	/// <summary>
	/// A read/write HeapByteBuffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>

	internal class HeapByteBuffer : ByteBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
		protected final byte[] hb;
		protected final int offset;
	
		*/

		internal HeapByteBuffer(int cap, int lim) : base(-1, 0, lim, cap, new sbyte[cap], 0) // package-private
		{

			/*
			hb = new byte[cap];
			offset = 0;
			*/




		}

		internal HeapByteBuffer(sbyte[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0) // package-private
		{

			/*
			hb = buf;
			offset = 0;
			*/




		}

		protected internal HeapByteBuffer(sbyte[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
		{

			/*
			hb = buf;
			offset = off;
			*/




		}

		public override ByteBuffer Slice()
		{
			return new HeapByteBuffer(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override ByteBuffer Duplicate()
		{
			return new HeapByteBuffer(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override ByteBuffer AsReadOnlyBuffer()
		{

			return new HeapByteBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return i + Offset;
		}

		public override sbyte Get()
		{
			return Hb[Ix(NextGetIndex())];
		}

		public override sbyte Get(int i)
		{
			return Hb[Ix(CheckIndex(i))];
		}







		public override ByteBuffer Get(sbyte[] dst, int offset, int length)
		{
			CheckBounds(offset, length, dst.Length);
			if (length > Remaining())
			{
				throw new BufferUnderflowException();
			}
			System.Array.Copy(Hb, Ix(Position()), dst, offset, length);
			Position(Position() + length);
			return this;
		}

		public override bool Direct
		{
			get
			{
				return false;
			}
		}



		public override bool ReadOnly
		{
			get
			{
				return false;
			}
		}

		public override ByteBuffer Put(sbyte x)
		{

			Hb[Ix(NextPutIndex())] = x;
			return this;



		}

		public override ByteBuffer Put(int i, sbyte x)
		{

			Hb[Ix(CheckIndex(i))] = x;
			return this;



		}

		public override ByteBuffer Put(sbyte[] src, int offset, int length)
		{

			CheckBounds(offset, length, src.Length);
			if (length > Remaining())
			{
				throw new BufferOverflowException();
			}
			System.Array.Copy(src, offset, Hb, Ix(Position()), length);
			Position(Position() + length);
			return this;



		}

		public override ByteBuffer Put(ByteBuffer src)
		{

			if (src is HeapByteBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				HeapByteBuffer sb = (HeapByteBuffer)src;
				int n = sb.Remaining();
				if (n > Remaining())
				{
					throw new BufferOverflowException();
				}
				System.Array.Copy(sb.Hb, sb.Ix(sb.Position()), Hb, Ix(Position()), n);
				sb.Position(sb.Position() + n);
				Position(Position() + n);
			}
			else if (src.Direct)
			{
				int n = src.Remaining();
				if (n > Remaining())
				{
					throw new BufferOverflowException();
				}
				src.Get(Hb, Ix(Position()), n);
				Position(Position() + n);
			}
			else
			{
				base.Put(src);
			}
			return this;



		}

		public override ByteBuffer Compact()
		{

			System.Array.Copy(Hb, Ix(Position()), Hb, Ix(0), Remaining());
			Position(Remaining());
			Limit(Capacity());
			DiscardMark();
			return this;



		}





		internal override sbyte _get(int i) // package-private
		{
			return Hb[i];
		}

		internal override void _put(int i, sbyte b) // package-private
		{

			Hb[i] = b;



		}

		// char



		public override char Char
		{
			get
			{
				return Bits.GetChar(this, Ix(NextGetIndex(2)), BigEndian);
			}
		}

		public override char GetChar(int i)
		{
			return Bits.GetChar(this, Ix(CheckIndex(i, 2)), BigEndian);
		}



		public override ByteBuffer PutChar(char x)
		{

			Bits.PutChar(this, Ix(NextPutIndex(2)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutChar(int i, char x)
		{

			Bits.PutChar(this, Ix(CheckIndex(i, 2)), x, BigEndian);
			return this;



		}

		public override CharBuffer AsCharBuffer()
		{
			int size = this.Remaining() >> 1;
			int off = Offset + Position();
			return (BigEndian ? (CharBuffer)(new ByteBufferAsCharBufferB(this, -1, 0, size, size, off)) : (CharBuffer)(new ByteBufferAsCharBufferL(this, -1, 0, size, size, off)));
		}


		// short



		public override short Short
		{
			get
			{
				return Bits.GetShort(this, Ix(NextGetIndex(2)), BigEndian);
			}
		}

		public override short GetShort(int i)
		{
			return Bits.GetShort(this, Ix(CheckIndex(i, 2)), BigEndian);
		}



		public override ByteBuffer PutShort(short x)
		{

			Bits.PutShort(this, Ix(NextPutIndex(2)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutShort(int i, short x)
		{

			Bits.PutShort(this, Ix(CheckIndex(i, 2)), x, BigEndian);
			return this;



		}

		public override ShortBuffer AsShortBuffer()
		{
			int size = this.Remaining() >> 1;
			int off = Offset + Position();
			return (BigEndian ? (ShortBuffer)(new ByteBufferAsShortBufferB(this, -1, 0, size, size, off)) : (ShortBuffer)(new ByteBufferAsShortBufferL(this, -1, 0, size, size, off)));
		}


		// int



		public override int Int
		{
			get
			{
				return Bits.GetInt(this, Ix(NextGetIndex(4)), BigEndian);
			}
		}

		public override int GetInt(int i)
		{
			return Bits.GetInt(this, Ix(CheckIndex(i, 4)), BigEndian);
		}



		public override ByteBuffer PutInt(int x)
		{

			Bits.PutInt(this, Ix(NextPutIndex(4)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutInt(int i, int x)
		{

			Bits.PutInt(this, Ix(CheckIndex(i, 4)), x, BigEndian);
			return this;



		}

		public override IntBuffer AsIntBuffer()
		{
			int size = this.Remaining() >> 2;
			int off = Offset + Position();
			return (BigEndian ? (IntBuffer)(new ByteBufferAsIntBufferB(this, -1, 0, size, size, off)) : (IntBuffer)(new ByteBufferAsIntBufferL(this, -1, 0, size, size, off)));
		}


		// long



		public override long Long
		{
			get
			{
				return Bits.GetLong(this, Ix(NextGetIndex(8)), BigEndian);
			}
		}

		public override long GetLong(int i)
		{
			return Bits.GetLong(this, Ix(CheckIndex(i, 8)), BigEndian);
		}



		public override ByteBuffer PutLong(long x)
		{

			Bits.PutLong(this, Ix(NextPutIndex(8)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutLong(int i, long x)
		{

			Bits.PutLong(this, Ix(CheckIndex(i, 8)), x, BigEndian);
			return this;



		}

		public override LongBuffer AsLongBuffer()
		{
			int size = this.Remaining() >> 3;
			int off = Offset + Position();
			return (BigEndian ? (LongBuffer)(new ByteBufferAsLongBufferB(this, -1, 0, size, size, off)) : (LongBuffer)(new ByteBufferAsLongBufferL(this, -1, 0, size, size, off)));
		}


		// float



		public override float Float
		{
			get
			{
				return Bits.GetFloat(this, Ix(NextGetIndex(4)), BigEndian);
			}
		}

		public override float GetFloat(int i)
		{
			return Bits.GetFloat(this, Ix(CheckIndex(i, 4)), BigEndian);
		}



		public override ByteBuffer PutFloat(float x)
		{

			Bits.PutFloat(this, Ix(NextPutIndex(4)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutFloat(int i, float x)
		{

			Bits.PutFloat(this, Ix(CheckIndex(i, 4)), x, BigEndian);
			return this;



		}

		public override FloatBuffer AsFloatBuffer()
		{
			int size = this.Remaining() >> 2;
			int off = Offset + Position();
			return (BigEndian ? (FloatBuffer)(new ByteBufferAsFloatBufferB(this, -1, 0, size, size, off)) : (FloatBuffer)(new ByteBufferAsFloatBufferL(this, -1, 0, size, size, off)));
		}


		// double



		public override double Double
		{
			get
			{
				return Bits.GetDouble(this, Ix(NextGetIndex(8)), BigEndian);
			}
		}

		public override double GetDouble(int i)
		{
			return Bits.GetDouble(this, Ix(CheckIndex(i, 8)), BigEndian);
		}



		public override ByteBuffer PutDouble(double x)
		{

			Bits.PutDouble(this, Ix(NextPutIndex(8)), x, BigEndian);
			return this;



		}

		public override ByteBuffer PutDouble(int i, double x)
		{

			Bits.PutDouble(this, Ix(CheckIndex(i, 8)), x, BigEndian);
			return this;



		}

		public override DoubleBuffer AsDoubleBuffer()
		{
			int size = this.Remaining() >> 3;
			int off = Offset + Position();
			return (BigEndian ? (DoubleBuffer)(new ByteBufferAsDoubleBufferB(this, -1, 0, size, size, off)) : (DoubleBuffer)(new ByteBufferAsDoubleBufferL(this, -1, 0, size, size, off)));
		}











































	}

}