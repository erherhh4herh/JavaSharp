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


	internal class DirectShortBufferU : ShortBuffer, DirectBuffer
	{



		// Cached unsafe-access object
		protected internal static readonly Unsafe @unsafe = Bits.@unsafe();

		// Cached array base offset
		private static readonly long ArrayBaseOffset = (long)@unsafe.arrayBaseOffset(typeof(short[]));

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
		internal DirectShortBufferU(DirectBuffer db, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap) // package-private
		{

			Address = db.address() + off;



			Att = db;



		}

		public override ShortBuffer Slice()
		{
			int pos = this.Position();
			int lim = this.Limit();
			assert(pos <= lim);
			int rem = (pos <= lim ? lim - pos : 0);
			int off = (pos << 1);
			assert(off >= 0);
			return new DirectShortBufferU(this, -1, 0, rem, rem, off);
		}

		public override ShortBuffer Duplicate()
		{
			return new DirectShortBufferU(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);
		}

		public override ShortBuffer AsReadOnlyBuffer()
		{

			return new DirectShortBufferRU(this, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), 0);



		}



		public virtual long Address()
		{
			return Address;
		}

		private long Ix(int i)
		{
			return Address + ((long)i << 1);
		}

		public override short Get()
		{
			return ((@unsafe.getShort(Ix(NextGetIndex()))));
		}

		public override short Get(int i)
		{
			return ((@unsafe.getShort(Ix(CheckIndex(i)))));
		}







		public override ShortBuffer Get(short[] dst, int offset, int length)
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
					Bits.copyToShortArray(Ix(pos), dst, (long)offset << 1, (long)length << 1);
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



		public override ShortBuffer Put(short x)
		{

			@unsafe.putShort(Ix(NextPutIndex()), ((x)));
			return this;



		}

		public override ShortBuffer Put(int i, short x)
		{

			@unsafe.putShort(Ix(CheckIndex(i)), ((x)));
			return this;



		}

		public override ShortBuffer Put(ShortBuffer src)
		{

			if (src is DirectShortBufferU)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				DirectShortBufferU sb = (DirectShortBufferU)src;

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

		public override ShortBuffer Put(short[] src, int offset, int length)
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
					Bits.copyFromShortArray(src, (long)offset << 1, Ix(pos), (long)length << 1);
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

		public override ShortBuffer Compact()
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















































		public override ByteOrder Order()
		{





			return ((ByteOrder.NativeOrder() != ByteOrder.BIG_ENDIAN) ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);

		}


























	}

}