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
	/// A read/write HeapLongBuffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>

	internal class HeapLongBuffer : LongBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
		protected final long[] hb;
		protected final int offset;
	
		*/

		internal HeapLongBuffer(int cap, int lim) : base(-1, 0, lim, cap, new long[cap], 0) // package-private
		{

			/*
			hb = new long[cap];
			offset = 0;
			*/




		}

		internal HeapLongBuffer(long[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0) // package-private
		{

			/*
			hb = buf;
			offset = 0;
			*/




		}

		protected internal HeapLongBuffer(long[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
		{

			/*
			hb = buf;
			offset = off;
			*/




		}

		public override LongBuffer Slice()
		{
			return new HeapLongBuffer(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override LongBuffer Duplicate()
		{
			return new HeapLongBuffer(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override LongBuffer AsReadOnlyBuffer()
		{

			return new HeapLongBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return i + Offset;
		}

		public override long Get()
		{
			return Hb[Ix(NextGetIndex())];
		}

		public override long Get(int i)
		{
			return Hb[Ix(CheckIndex(i))];
		}







		public override LongBuffer Get(long[] dst, int offset, int length)
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

		public override LongBuffer Put(long x)
		{

			Hb[Ix(NextPutIndex())] = x;
			return this;



		}

		public override LongBuffer Put(int i, long x)
		{

			Hb[Ix(CheckIndex(i))] = x;
			return this;



		}

		public override LongBuffer Put(long[] src, int offset, int length)
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

		public override LongBuffer Put(LongBuffer src)
		{

			if (src is HeapLongBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				HeapLongBuffer sb = (HeapLongBuffer)src;
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

		public override LongBuffer Compact()
		{

			System.Array.Copy(Hb, Ix(Position()), Hb, Ix(0), Remaining());
			Position(Remaining());
			Limit(Capacity());
			DiscardMark();
			return this;



		}






































































































































































































































































































































































		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}