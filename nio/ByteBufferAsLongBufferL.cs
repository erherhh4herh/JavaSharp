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


	internal class ByteBufferAsLongBufferL : LongBuffer // package-private
	{



		protected internal readonly ByteBuffer Bb;
		protected internal readonly new int Offset;



		internal ByteBufferAsLongBufferL(ByteBuffer bb) : base(-1, 0, bb.Remaining() >> 3, bb.Remaining() >> 3) // package-private
		{

			this.Bb = bb;
			// enforce limit == capacity
			int cap = this.Capacity();
			this.Limit(cap);
			int pos = this.Position();
			assert(pos <= cap);
			Offset = pos;



		}

		internal ByteBufferAsLongBufferL(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap)
		{

			this.Bb = bb;
			Offset = off;



		}

		public override LongBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 3) + Offset;
			assert(off >= 0);
			return new ByteBufferAsLongBufferL(Bb, -1, 0, rem, rem, off);
		}

		public override LongBuffer Duplicate()
		{
			return new ByteBufferAsLongBufferL(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override LongBuffer AsReadOnlyBuffer()
		{

			return new ByteBufferAsLongBufferRL(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return (i << 3) + Offset;
		}

		public override long Get()
		{
			return Bits.GetLongL(Bb, Ix(NextGetIndex()));
		}

		public override long Get(int i)
		{
			return Bits.GetLongL(Bb, Ix(CheckIndex(i)));
		}









		public override LongBuffer Put(long x)
		{

			Bits.PutLongL(Bb, Ix(NextPutIndex()), x);
			return this;



		}

		public override LongBuffer Put(int i, long x)
		{

			Bits.PutLongL(Bb, Ix(CheckIndex(i)), x);
			return this;



		}

		public override LongBuffer Compact()
		{

			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);

			ByteBuffer db = Bb.Duplicate();
			db.Limit(Ix(lim));
			db.Position(Ix(0));
			ByteBuffer sb = db.Slice();
			sb.Position(pos << 3);
			sb.Compact();
			Position(rem);
			Limit(Capacity());
			DiscardMark();
			return this;



		}

		public override bool Direct
		{
			get
			{
				return Bb.Direct;
			}
		}

		public override bool ReadOnly
		{
			get
			{
				return false;
			}
		}











































		public override ByteOrder Order()
		{




			return ByteOrder.LITTLE_ENDIAN;

		}

	}

}