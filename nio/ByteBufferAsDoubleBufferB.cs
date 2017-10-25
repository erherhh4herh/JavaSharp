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


	internal class ByteBufferAsDoubleBufferB : DoubleBuffer // package-private
	{



		protected internal readonly ByteBuffer Bb;
		protected internal readonly new int Offset;



		internal ByteBufferAsDoubleBufferB(ByteBuffer bb) : base(-1, 0, bb.Remaining() >> 3, bb.Remaining() >> 3) // package-private
		{

			this.Bb = bb;
			// enforce limit == capacity
			int cap = this.Capacity();
			this.Limit(cap);
			int pos = this.Position();
			assert(pos <= cap);
			Offset = pos;



		}

		internal ByteBufferAsDoubleBufferB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap)
		{

			this.Bb = bb;
			Offset = off;



		}

		public override DoubleBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 3) + Offset;
			assert(off >= 0);
			return new ByteBufferAsDoubleBufferB(Bb, -1, 0, rem, rem, off);
		}

		public override DoubleBuffer Duplicate()
		{
			return new ByteBufferAsDoubleBufferB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override DoubleBuffer AsReadOnlyBuffer()
		{

			return new ByteBufferAsDoubleBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return (i << 3) + Offset;
		}

		public override double Get()
		{
			return Bits.GetDoubleB(Bb, Ix(NextGetIndex()));
		}

		public override double Get(int i)
		{
			return Bits.GetDoubleB(Bb, Ix(CheckIndex(i)));
		}









		public override DoubleBuffer Put(double x)
		{

			Bits.PutDoubleB(Bb, Ix(NextPutIndex()), x);
			return this;



		}

		public override DoubleBuffer Put(int i, double x)
		{

			Bits.PutDoubleB(Bb, Ix(CheckIndex(i)), x);
			return this;



		}

		public override DoubleBuffer Compact()
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

			return ByteOrder.BIG_ENDIAN;




		}

	}

}