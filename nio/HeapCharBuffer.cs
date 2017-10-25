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


	/// 
	/// <summary>
	/// A read/write HeapCharBuffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>

	internal class HeapCharBuffer : CharBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
		protected final char[] hb;
		protected final int offset;
	
		*/

		internal HeapCharBuffer(int cap, int lim) : base(-1, 0, lim, cap, new char[cap], 0) // package-private
		{

			/*
			hb = new char[cap];
			offset = 0;
			*/




		}

		internal HeapCharBuffer(char[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0) // package-private
		{

			/*
			hb = buf;
			offset = 0;
			*/




		}

		protected internal HeapCharBuffer(char[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
		{

			/*
			hb = buf;
			offset = off;
			*/




		}

		public override CharBuffer Slice()
		{
			return new HeapCharBuffer(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override CharBuffer Duplicate()
		{
			return new HeapCharBuffer(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override CharBuffer AsReadOnlyBuffer()
		{

			return new HeapCharBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return i + Offset;
		}

		public override char Get()
		{
			return Hb[Ix(NextGetIndex())];
		}

		public override char Get(int i)
		{
			return Hb[Ix(CheckIndex(i))];
		}


		internal override char GetUnchecked(int i)
		{
		return Hb[Ix(i)];
		}


		public override CharBuffer Get(char[] dst, int offset, int length)
		{
			CheckBounds(offset, length, dst.Length);
			if (length > Remaining())
			{
				throw new BufferUnderflowException();
			}
			System.Array.Copy(Hb, Ix(Position()), dst, offset, length);
			Position(Position() + length);
			return this;
		}

		public override bool Direct
		{
			get
			{
				return false;
			}
		}



		public override bool ReadOnly
		{
			get
			{
				return false;
			}
		}

		public override CharBuffer Put(char x)
		{

			Hb[Ix(NextPutIndex())] = x;
			return this;



		}

		public override CharBuffer Put(int i, char x)
		{

			Hb[Ix(CheckIndex(i))] = x;
			return this;



		}

		public override CharBuffer Put(char[] src, int offset, int length)
		{

			CheckBounds(offset, length, src.Length);
			if (length > Remaining())
			{
				throw new BufferOverflowException();
			}
			System.Array.Copy(src, offset, Hb, Ix(Position()), length);
			Position(Position() + length);
			return this;



		}

		public override CharBuffer Put(CharBuffer src)
		{

			if (src is HeapCharBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				HeapCharBuffer sb = (HeapCharBuffer)src;
				int n = sb.Remaining();
				if (n > Remaining())
				{
					throw new BufferOverflowException();
				}
				System.Array.Copy(sb.Hb, sb.Ix(sb.Position()), Hb, Ix(Position()), n);
				sb.Position(sb.Position() + n);
				Position(Position() + n);
			}
			else if (src.Direct)
			{
				int n = src.Remaining();
				if (n > Remaining())
				{
					throw new BufferOverflowException();
				}
				src.Get(Hb, Ix(Position()), n);
				Position(Position() + n);
			}
			else
			{
				base.Put(src);
			}
			return this;



		}

		public override CharBuffer Compact()
		{

			System.Array.Copy(Hb, Ix(Position()), Hb, Ix(0), Remaining());
			Position(Remaining());
			Limit(Capacity());
			DiscardMark();
			return this;



		}








































































































































































































































































































































		internal override String ToString(int start, int end) // package-private
		{
			try
			{
				return new String(Hb, start + Offset, end - start);
			}
			catch (StringIndexOutOfBoundsException)
			{
				throw new IndexOutOfBoundsException();
			}
		}


		// --- Methods to support CharSequence ---

		public override CharBuffer SubSequence(int start, int end)
		{
			if ((start < 0) || (end > Length()) || (start > end))
			{
				throw new IndexOutOfBoundsException();
			}
			int pos = Position();
			return new HeapCharBuffer(Hb, -1, pos + start, pos + end, Capacity(), Offset);
		}






		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}