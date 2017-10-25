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


	internal class ByteBufferAsShortBufferRB : ByteBufferAsShortBufferB // package-private
	{








		internal ByteBufferAsShortBufferRB(ByteBuffer bb) : base(bb) // package-private
		{













		}

		internal ByteBufferAsShortBufferRB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(bb, mark, pos, lim, cap, off)
		{






		}

		public override ShortBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1) + Offset;
			assert(off >= 0);
			return new ByteBufferAsShortBufferRB(Bb, -1, 0, rem, rem, off);
		}

		public override ShortBuffer Duplicate()
		{
			return new ByteBufferAsShortBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override ShortBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}























		public override ShortBuffer Put(short x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ShortBuffer Put(int i, short x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ShortBuffer Compact()
		{

















			throw new ReadOnlyBufferException();

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
				return true;
			}
		}











































		public override ByteOrder Order()
		{

			return ByteOrder.BIG_ENDIAN;




		}

	}

}