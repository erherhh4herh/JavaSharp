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


	internal class DirectIntBufferRU : DirectIntBufferU, DirectBuffer
	{















































































































































		// For duplicates and slices
		//
		internal DirectIntBufferRU(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(db, mark, pos, lim, cap, off) // package-private
		{









		}

		public override IntBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 2);
			assert(off >= 0);
			return new DirectIntBufferRU(this, -1, 0, rem, rem, off);
		}

		public override IntBuffer Duplicate()
		{
			return new DirectIntBufferRU(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override IntBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}


























































		public override IntBuffer Put(int x)
		{




			throw new ReadOnlyBufferException();

		}

		public override IntBuffer Put(int i, int x)
		{




			throw new ReadOnlyBufferException();

		}

		public override IntBuffer Put(IntBuffer src)
		{




































			throw new ReadOnlyBufferException();

		}

		public override IntBuffer Put(int[] src, int offset, int length)
		{




























			throw new ReadOnlyBufferException();

		}

		public override IntBuffer Compact()
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