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


	internal class ByteBufferAsLongBufferRL : ByteBufferAsLongBufferL // package-private
	{








		internal ByteBufferAsLongBufferRL(ByteBuffer bb) : base(bb) // package-private
		{













		}

		internal ByteBufferAsLongBufferRL(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(bb, mark, pos, lim, cap, off)
		{






		}

		public override LongBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 3) + Offset;
			assert(off >= 0);
			return new ByteBufferAsLongBufferRL(Bb, -1, 0, rem, rem, off);
		}

		public override LongBuffer Duplicate()
		{
			return new ByteBufferAsLongBufferRL(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override LongBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}























		public override LongBuffer Put(long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Put(int i, long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Compact()
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




			return ByteOrder.LITTLE_ENDIAN;

		}

	}

}