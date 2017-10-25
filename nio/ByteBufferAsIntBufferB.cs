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


	internal class ByteBufferAsIntBufferB : IntBuffer // package-private
	{



		protected internal readonly ByteBuffer Bb;
		protected internal readonly new int Offset;



		internal ByteBufferAsIntBufferB(ByteBuffer bb) : base(-1, 0, bb.Remaining() >> 2, bb.Remaining() >> 2) // package-private
		{

			this.Bb = bb;
			// enforce limit == capacity
			int cap = this.Capacity();
			this.Limit(cap);
			int pos = this.Position();
			assert(pos <= cap);
			Offset = pos;



		}

		internal ByteBufferAsIntBufferB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap)
		{

			this.Bb = bb;
			Offset = off;



		}

		public override IntBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 2) + Offset;
			assert(off >= 0);
			return new ByteBufferAsIntBufferB(Bb, -1, 0, rem, rem, off);
		}

		public override IntBuffer Duplicate()
		{
			return new ByteBufferAsIntBufferB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override IntBuffer AsReadOnlyBuffer()
		{

			return new ByteBufferAsIntBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return (i << 2) + Offset;
		}

		public override int Get()
		{
			return Bits.GetIntB(Bb, Ix(NextGetIndex()));
		}

		public override int Get(int i)
		{
			return Bits.GetIntB(Bb, Ix(CheckIndex(i)));
		}









		public override IntBuffer Put(int x)
		{

			Bits.PutIntB(Bb, Ix(NextPutIndex()), x);
			return this;



		}

		public override IntBuffer Put(int i, int x)
		{

			Bits.PutIntB(Bb, Ix(CheckIndex(i)), x);
			return this;



		}

		public override IntBuffer Compact()
		{

			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);

			ByteBuffer db = Bb.Duplicate();
			db.Limit(Ix(lim));
			db.Position(Ix(0));
			ByteBuffer sb = db.Slice();
			sb.Position(pos << 2);
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