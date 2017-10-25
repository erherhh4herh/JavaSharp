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

	using Cleaner = sun.misc.Cleaner;
	using Unsafe = sun.misc.Unsafe;
	using VM = sun.misc.VM;
	using DirectBuffer = sun.nio.ch.DirectBuffer;


	internal class DirectDoubleBufferRU : DirectDoubleBufferU, DirectBuffer
	{















































































































































		// For duplicates and slices
		//
		internal DirectDoubleBufferRU(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(db, mark, pos, lim, cap, off) // package-private
		{









		}

		public override DoubleBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 3);
			assert(off >= 0);
			return new DirectDoubleBufferRU(this, -1, 0, rem, rem, off);
		}

		public override DoubleBuffer Duplicate()
		{
			return new DirectDoubleBufferRU(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
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

		public override DoubleBuffer Put(DoubleBuffer src)
		{




































			throw new ReadOnlyBufferException();

		}

		public override DoubleBuffer Put(double[] src, int offset, int length)
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
				return true;
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





			return ((ByteOrder.NativeOrder() != ByteOrder.BIG_ENDIAN) ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);

		}


























	}

}