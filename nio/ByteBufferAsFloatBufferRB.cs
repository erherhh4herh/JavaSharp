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


	internal class ByteBufferAsFloatBufferRB : ByteBufferAsFloatBufferB // package-private
	{








		internal ByteBufferAsFloatBufferRB(ByteBuffer bb) : base(bb) // package-private
		{













		}

		internal ByteBufferAsFloatBufferRB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(bb, mark, pos, lim, cap, off)
		{






		}

		public override FloatBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 2) + Offset;
			assert(off >= 0);
			return new ByteBufferAsFloatBufferRB(Bb, -1, 0, rem, rem, off);
		}

		public override FloatBuffer Duplicate()
		{
			return new ByteBufferAsFloatBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override FloatBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}























		public override FloatBuffer Put(float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Put(int i, float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Compact()
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