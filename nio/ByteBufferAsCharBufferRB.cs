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


	internal class ByteBufferAsCharBufferRB : ByteBufferAsCharBufferB // package-private
	{








		internal ByteBufferAsCharBufferRB(ByteBuffer bb) : base(bb) // package-private
		{













		}

		internal ByteBufferAsCharBufferRB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(bb, mark, pos, lim, cap, off)
		{






		}

		public override CharBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1) + Offset;
			assert(off >= 0);
			return new ByteBufferAsCharBufferRB(Bb, -1, 0, rem, rem, off);
		}

		public override CharBuffer Duplicate()
		{
			return new ByteBufferAsCharBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
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

		public override CharBuffer Compact()
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
			return new ByteBufferAsCharBufferRB(Bb, -1, pos + start, pos + end, Capacity(), Offset);
		}




		public override ByteOrder Order()
		{

			return ByteOrder.BIG_ENDIAN;




		}

	}

}