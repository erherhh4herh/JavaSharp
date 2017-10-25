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
	/// 
	/// 
	/// <summary>
	/// A read-only HeapByteBuffer.  This class extends the corresponding
	/// read/write class, overriding the mutation methods to throw a {@link
	/// ReadOnlyBufferException} and overriding the view-buffer methods to return an
	/// instance of this class rather than of the superclass.
	/// 
	/// </summary>

	internal class HeapByteBufferR : HeapByteBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
	
	
	
		*/

		internal HeapByteBufferR(int cap, int lim) : base(cap, lim) // package-private
		{







			this.IsReadOnly = true;

		}

		internal HeapByteBufferR(sbyte[] buf, int off, int len) : base(buf, off, len) // package-private
		{







			this.IsReadOnly = true;

		}

		protected internal HeapByteBufferR(sbyte[] buf, int mark, int pos, int lim, int cap, int off) : base(buf, mark, pos, lim, cap, off)
		{







			this.IsReadOnly = true;

		}

		public override ByteBuffer Slice()
		{
			return new HeapByteBufferR(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override ByteBuffer Duplicate()
		{
			return new HeapByteBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override ByteBuffer AsReadOnlyBuffer()
		{








			return Duplicate();

		}




































		public override bool ReadOnly
		{
			get
			{
				return true;
			}
		}

		public override ByteBuffer Put(sbyte x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(int i, sbyte x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(sbyte[] src, int offset, int length)
		{








			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Put(ByteBuffer src)
		{























			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer Compact()
		{







			throw new ReadOnlyBufferException();

		}





		internal override sbyte _get(int i) // package-private
		{
			return Hb[i];
		}

		internal override void _put(int i, sbyte b) // package-private
		{



			throw new ReadOnlyBufferException();

		}

		// char













		public override ByteBuffer PutChar(char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutChar(int i, char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override CharBuffer AsCharBuffer()
		{
			int size = this.Remaining() >> 1;
			int off = Offset + Position();
			return (BigEndian ? (CharBuffer)(new ByteBufferAsCharBufferRB(this, -1, 0, size, size, off)) : (CharBuffer)(new ByteBufferAsCharBufferRL(this, -1, 0, size, size, off)));
		}


		// short













		public override ByteBuffer PutShort(short x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutShort(int i, short x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ShortBuffer AsShortBuffer()
		{
			int size = this.Remaining() >> 1;
			int off = Offset + Position();
			return (BigEndian ? (ShortBuffer)(new ByteBufferAsShortBufferRB(this, -1, 0, size, size, off)) : (ShortBuffer)(new ByteBufferAsShortBufferRL(this, -1, 0, size, size, off)));
		}


		// int













		public override ByteBuffer PutInt(int x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutInt(int i, int x)
		{




			throw new ReadOnlyBufferException();

		}

		public override IntBuffer AsIntBuffer()
		{
			int size = this.Remaining() >> 2;
			int off = Offset + Position();
			return (BigEndian ? (IntBuffer)(new ByteBufferAsIntBufferRB(this, -1, 0, size, size, off)) : (IntBuffer)(new ByteBufferAsIntBufferRL(this, -1, 0, size, size, off)));
		}


		// long













		public override ByteBuffer PutLong(long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutLong(int i, long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override LongBuffer AsLongBuffer()
		{
			int size = this.Remaining() >> 3;
			int off = Offset + Position();
			return (BigEndian ? (LongBuffer)(new ByteBufferAsLongBufferRB(this, -1, 0, size, size, off)) : (LongBuffer)(new ByteBufferAsLongBufferRL(this, -1, 0, size, size, off)));
		}


		// float













		public override ByteBuffer PutFloat(float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutFloat(int i, float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer AsFloatBuffer()
		{
			int size = this.Remaining() >> 2;
			int off = Offset + Position();
			return (BigEndian ? (FloatBuffer)(new ByteBufferAsFloatBufferRB(this, -1, 0, size, size, off)) : (FloatBuffer)(new ByteBufferAsFloatBufferRL(this, -1, 0, size, size, off)));
		}


		// double













		public override ByteBuffer PutDouble(double x)
		{




			throw new ReadOnlyBufferException();

		}

		public override ByteBuffer PutDouble(int i, double x)
		{




			throw new ReadOnlyBufferException();

		}

		public override DoubleBuffer AsDoubleBuffer()
		{
			int size = this.Remaining() >> 3;
			int off = Offset + Position();
			return (BigEndian ? (DoubleBuffer)(new ByteBufferAsDoubleBufferRB(this, -1, 0, size, size, off)) : (DoubleBuffer)(new ByteBufferAsDoubleBufferRL(this, -1, 0, size, size, off)));
		}











































	}

}