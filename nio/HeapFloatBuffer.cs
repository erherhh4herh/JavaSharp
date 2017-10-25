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
	/// A read/write HeapFloatBuffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>

	internal class HeapFloatBuffer : FloatBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
		protected final float[] hb;
		protected final int offset;
	
		*/

		internal HeapFloatBuffer(int cap, int lim) : base(-1, 0, lim, cap, new float[cap], 0) // package-private
		{

			/*
			hb = new float[cap];
			offset = 0;
			*/




		}

		internal HeapFloatBuffer(float[] buf, int off, int len) : base(-1, off, off + len, buf.Length, buf, 0) // package-private
		{

			/*
			hb = buf;
			offset = 0;
			*/




		}

		protected internal HeapFloatBuffer(float[] buf, int mark, int pos, int lim, int cap, int off) : base(mark, pos, lim, cap, buf, off)
		{

			/*
			hb = buf;
			offset = off;
			*/




		}

		public override FloatBuffer Slice()
		{
			return new HeapFloatBuffer(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override FloatBuffer Duplicate()
		{
			return new HeapFloatBuffer(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override FloatBuffer AsReadOnlyBuffer()
		{

			return new HeapFloatBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);



		}



		protected internal virtual int Ix(int i)
		{
			return i + Offset;
		}

		public override float Get()
		{
			return Hb[Ix(NextGetIndex())];
		}

		public override float Get(int i)
		{
			return Hb[Ix(CheckIndex(i))];
		}







		public override FloatBuffer Get(float[] dst, int offset, int length)
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

		public override FloatBuffer Put(float x)
		{

			Hb[Ix(NextPutIndex())] = x;
			return this;



		}

		public override FloatBuffer Put(int i, float x)
		{

			Hb[Ix(CheckIndex(i))] = x;
			return this;



		}

		public override FloatBuffer Put(float[] src, int offset, int length)
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

		public override FloatBuffer Put(FloatBuffer src)
		{

			if (src is HeapFloatBuffer)
			{
				if (src == this)
				{
					throw new IllegalArgumentException();
				}
				HeapFloatBuffer sb = (HeapFloatBuffer)src;
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

		public override FloatBuffer Compact()
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