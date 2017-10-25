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


	internal class ByteBufferAsDoubleBufferRB : ByteBufferAsDoubleBufferB // package-private
	{








		internal ByteBufferAsDoubleBufferRB(ByteBuffer bb) : base(bb) // package-private
		{













		}

		internal ByteBufferAsDoubleBufferRB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(bb, mark, pos, lim, cap, off)
		{






		}

		public override DoubleBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 3) + Offset;
			assert(off >= 0);
			return new ByteBufferAsDoubleBufferRB(Bb, -1, 0, rem, rem, off);
		}

		public override DoubleBuffer Duplicate()
		{
			return new ByteBufferAsDoubleBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override DoubleBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}























		public override DoubleBuffer Put(double x)
		{




			throw new ReadOnlyBufferException();

		}

		public override DoubleBuffer Put(int i, double x)
		{




			throw new ReadOnlyBufferException();

		}

		public override DoubleBuffer Compact()
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