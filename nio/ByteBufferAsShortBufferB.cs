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


	internal class ByteBufferAsShortBufferB : ShortBuffer // package-private
	{



		protected internal readonly ByteBuffer Bb;
		protected internal readonly new int Offset;



		internal ByteBufferAsShortBufferB(ByteBuffer bb) : base(-1, 0, bb.Remaining() >> 1, bb.Remaining() >> 1) // package-private
		{

			this.Bb = bb;
			// enforce limit == capacity
			int cap = this.Capacity();
			this.Limit(cap);
			int pos = this.Position();
			assert(pos <= cap);
			Offset = pos;



		}

		internal ByteBufferAsShortBufferB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap)
		{

			this.Bb = bb;
			Offset = off;



		}

		public override ShortBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1) + Offset;
			assert(off >= 0);
			return new ByteBufferAsShortBufferB(Bb, -1, 0, rem, rem, off);
		}

		public override ShortBuffer Duplicate()
		{
			return new ByteBufferAsShortBufferB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override ShortBuffer AsReadOnlyBuffer()
		{

			return new ByteBufferAsShortBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return (i << 1) + Offset;
		}

		public override short Get()
		{
			return Bits.GetShortB(Bb, Ix(NextGetIndex()));
		}

		public override short Get(int i)
		{
			return Bits.GetShortB(Bb, Ix(CheckIndex(i)));
		}









		public override ShortBuffer Put(short x)
		{

			Bits.PutShortB(Bb, Ix(NextPutIndex()), x);
			return this;



		}

		public override ShortBuffer Put(int i, short x)
		{

			Bits.PutShortB(Bb, Ix(CheckIndex(i)), x);
			return this;



		}

		public override ShortBuffer Compact()
		{

			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);

			ByteBuffer db = Bb.Duplicate();
			db.Limit(Ix(lim));
			db.Position(Ix(0));
			ByteBuffer sb = db.Slice();
			sb.Position(pos << 1);
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