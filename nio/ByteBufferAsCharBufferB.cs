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


	internal class ByteBufferAsCharBufferB : CharBuffer // package-private
	{



		protected internal readonly ByteBuffer Bb;
		protected internal readonly new int Offset;



		internal ByteBufferAsCharBufferB(ByteBuffer bb) : base(-1, 0, bb.Remaining() >> 1, bb.Remaining() >> 1) // package-private
		{

			this.Bb = bb;
			// enforce limit == capacity
			int cap = this.Capacity();
			this.Limit(cap);
			int pos = this.Position();
			assert(pos <= cap);
			Offset = pos;



		}

		internal ByteBufferAsCharBufferB(ByteBuffer bb, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap)
		{

			this.Bb = bb;
			Offset = off;



		}

		public override CharBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1) + Offset;
			assert(off >= 0);
			return new ByteBufferAsCharBufferB(Bb, -1, 0, rem, rem, off);
		}

		public override CharBuffer Duplicate()
		{
			return new ByteBufferAsCharBufferB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override CharBuffer AsReadOnlyBuffer()
		{

			return new ByteBufferAsCharBufferRB(Bb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return (i << 1) + Offset;
		}

		public override char Get()
		{
			return Bits.GetCharB(Bb, Ix(NextGetIndex()));
		}

		public override char Get(int i)
		{
			return Bits.GetCharB(Bb, Ix(CheckIndex(i)));
		}


	   internal override char GetUnchecked(int i)
	   {
			return Bits.GetCharB(Bb, Ix(i));
	   }




		public override CharBuffer Put(char x)
		{

			Bits.PutCharB(Bb, Ix(NextPutIndex()), x);
			return this;



		}

		public override CharBuffer Put(int i, char x)
		{

			Bits.PutCharB(Bb, Ix(CheckIndex(i)), x);
			return this;



		}

		public override CharBuffer Compact()
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
			return new ByteBufferAsCharBufferB(Bb, -1, pos + start, pos + end, Capacity(), Offset);
		}




		public override ByteOrder Order()
		{

			return ByteOrder.BIG_ENDIAN;




		}

	}

}