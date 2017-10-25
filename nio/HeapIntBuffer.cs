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
	/// A read/write HeapIntBuffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>

	internal class HeapIntBuffer : IntBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
		protected final int[] hb;
		protected final int offset;
	
		*/

		internal HeapIntBuffer(int cap, int lim) : base(-1, 0, lim, cap, new int[cap], 0) // package-private
		{

			/*
			hb = new int[cap];
			offset = 0;
			*/




		}

		internal HeapIntBuffer(int[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0) // package-private
		{

			/*
			hb = buf;
			offset = 0;
			*/




		}

		protected internal HeapIntBuffer(int[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
		{

			/*
			hb = buf;
			offset = off;
			*/




		}

		public override IntBuffer Slice()
		{
			return new HeapIntBuffer(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override IntBuffer Duplicate()
		{
			return new HeapIntBuffer(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override IntBuffer AsReadOnlyBuffer()
		{

			return new HeapIntBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return i + Offset;
		}

		public override int Get()
		{
			return Hb[Ix(NextGetIndex())];
		}

		public override int Get(int i)
		{
			return Hb[Ix(CheckIndex(i))];
		}







		public override IntBuffer Get(int[] dst, int offset, int length)
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

		public override IntBuffer Put(int x)
		{

			Hb[Ix(NextPutIndex())] = x;
			return this;



		}

		public override IntBuffer Put(int i, int x)
		{

			Hb[Ix(CheckIndex(i))] = x;
			return this;



		}

		public override IntBuffer Put(int[] src, int offset, int length)
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

		public override IntBuffer Put(IntBuffer src)
		{

			if (src is HeapIntBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				HeapIntBuffer sb = (HeapIntBuffer)src;
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

		public override IntBuffer Compact()
		{

			System.Array.Copy(Hb, Ix(Position()), Hb, Ix(0), Remaining());
			Position(Remaining());
			Limit(Capacity());
			DiscardMark();
			return this;



		}






































































































































































































































































































































































		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}