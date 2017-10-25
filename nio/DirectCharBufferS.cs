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


	internal class DirectCharBufferS : CharBuffer, DirectBuffer
	{



		// Cached unsafe-access object
		protected internal static readonly Unsafe @unsafe = Bits.@unsafe();

		// Cached array base offset
		private static readonly long ArrayBaseOffset = (long)@unsafe.arrayBaseOffset(typeof(char[]));

		// Cached unaligned-access capability
		protected internal static readonly bool Unaligned = Bits.Unaligned();

		// Base address, used in all indexing calculations
		// NOTE: moved up to Buffer.java for speed in JNI GetDirectBufferAddress
		//    protected long address;

		// An object attached to this buffer. If this buffer is a view of another
		// buffer then we use this field to keep a reference to that buffer to
		// ensure that its memory isn't freed before we are done with it.
		private readonly Object Att;

		public virtual Object Attachment()
		{
			return Att;
		}






































		public virtual Cleaner Cleaner()
		{
			return null;
		}
















































































		// For duplicates and slices
		//
		internal DirectCharBufferS(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap) // package-private
		{

			Address = db.address() + off;



			Att = db;



		}

		public override CharBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1);
			assert(off >= 0);
			return new DirectCharBufferS(this, -1, 0, rem, rem, off);
		}

		public override CharBuffer Duplicate()
		{
			return new DirectCharBufferS(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override CharBuffer AsReadOnlyBuffer()
		{

			return new DirectCharBufferRS(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);



		}



		public virtual long Address()
		{
			return Address;
		}

		private long Ix(int i)
		{
			return Address + ((long)i << 1);
		}

		public override char Get()
		{
			return (Bits.Swap(@unsafe.getChar(Ix(NextGetIndex()))));
		}

		public override char Get(int i)
		{
			return (Bits.Swap(@unsafe.getChar(Ix(CheckIndex(i)))));
		}


		internal override char GetUnchecked(int i)
		{
			return (Bits.Swap(@unsafe.getChar(Ix(i))));
		}


		public override CharBuffer Get(char[] dst, int offset, int length)
		{

			if (((long)length << 1) > Bits.JNI_COPY_TO_ARRAY_THRESHOLD)
			{
				CheckBounds(offset, length, dst.Length);
				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);
				if (length > rem)
				{
					throw new BufferUnderflowException();
				}


				if (Order() != ByteOrder.NativeOrder())
				{
					Bits.CopyToCharArray(Ix(pos), dst, (long)offset << 1, (long)length << 1);
				}
				else

				{
					Bits.CopyToArray(Ix(pos), dst, ArrayBaseOffset, (long)offset << 1, (long)length << 1);
				}
				Position(pos + length);
			}
			else
			{
				base.Get(dst, offset, length);
			}
			return this;



		}



		public override CharBuffer Put(char x)
		{

			@unsafe.putChar(Ix(NextPutIndex()), Bits.Swap((x)));
			return this;



		}

		public override CharBuffer Put(int i, char x)
		{

			@unsafe.putChar(Ix(CheckIndex(i)), Bits.Swap((x)));
			return this;



		}

		public override CharBuffer Put(CharBuffer src)
		{

			if (src is DirectCharBufferS)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				DirectCharBufferS sb = (DirectCharBufferS)src;

				int spos = sb.Position();
				int slim = sb.Limit();
				assert(spos <= slim);
				int srem = (spos <= slim ? slim - spos : 0);

				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);

				if (srem > rem)
				{
					throw new BufferOverflowException();
				}
				@unsafe.copyMemory(sb.Ix(spos), Ix(pos), (long)srem << 1);
				sb.Position(spos + srem);
				Position(pos + srem);
			}
			else if (src.Hb != null)
			{

				int spos = src.Position();
				int slim = src.Limit();
				assert(spos <= slim);
				int srem = (spos <= slim ? slim - spos : 0);

				Put(src.Hb, src.Offset + spos, srem);
				src.Position(spos + srem);

			}
			else
			{
				base.Put(src);
			}
			return this;



		}

		public override CharBuffer Put(char[] src, int offset, int length)
		{

			if (((long)length << 1) > Bits.JNI_COPY_FROM_ARRAY_THRESHOLD)
			{
				CheckBounds(offset, length, src.Length);
				int pos = Position();
				int lim = Limit();
				assert(pos <= lim);
				int rem = (pos <= lim ? lim - pos : 0);
				if (length > rem)
				{
					throw new BufferOverflowException();
				}


				if (Order() != ByteOrder.NativeOrder())
				{
					Bits.CopyFromCharArray(src, (long)offset << 1, Ix(pos), (long)length << 1);
				}
				else

				{
					Bits.CopyFromArray(src, ArrayBaseOffset, (long)offset << 1, Ix(pos), (long)length << 1);
				}
				Position(pos + length);
			}
			else
			{
				base.Put(src, offset, length);
			}
			return this;



		}

		public override CharBuffer Compact()
		{

			int pos = Position();
			int lim = Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);

			@unsafe.copyMemory(Ix(pos), Ix(0), (long)rem << 1);
			Position(rem);
			Limit(Capacity());
			DiscardMark();
			return this;



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
			return new DirectCharBufferS(this, -1, pos + start, pos + end, Capacity(), Offset);
		}







		public override ByteOrder Order()
		{

			return ((ByteOrder.NativeOrder() == ByteOrder.BIG_ENDIAN) ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);





		}


























	}

}