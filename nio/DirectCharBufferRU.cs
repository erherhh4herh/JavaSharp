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


	internal class DirectCharBufferRU : DirectCharBufferU, DirectBuffer
	{















































































































































		// For duplicates and slices
		//
		internal DirectCharBufferRU(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(db, mark, pos, lim, cap, off) // package-private
		{









		}

		public override CharBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1);
			assert(off >= 0);
			return new DirectCharBufferRU(this, -1, 0, rem, rem, off);
		}

		public override CharBuffer Duplicate()
		{
			return new DirectCharBufferRU(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override CharBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}


























































		public override CharBuffer Put(char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(int i, char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(CharBuffer src)
		{




































			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(char[] src, int offset, int length)
		{




























			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Compact()
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




		public override String ToString(int start, int end)
		{
			if ((end > Limit()) || (start > end))
			{
				throw new IndexOutOfBoundsException();
			}
			try
			{
				int len = end - start;
				char[] ca = new char[len];
				CharBuffer cb = CharBuffer.Wrap(ca);
				CharBuffer db = this.Duplicate();
				db.Position(start);
				db.Limit(end);
				cb.Put(db);
				return new String(ca);
			}
			catch (StringIndexOutOfBoundsException)
			{
				throw new IndexOutOfBoundsException();
			}
		}


		// --- Methods to support CharSequence ---

		public override CharBuffer SubSequence(int start, int end)
		{
			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			pos = (pos <= lim ? pos : lim);
			int len = lim - pos;

			if ((start < 0) || (end > len) || (start > end))
			{
				throw new IndexOutOfBoundsException();
			}
			return new DirectCharBufferRU(this, -1, pos + start, pos + end, Capacity(), Offset);
		}







		public override ByteOrder Order()
		{





			return ((ByteOrder.NativeOrder() != ByteOrder.BIG_ENDIAN) ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);

		}


























	}

}