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
	/// A read-only HeapFloatBuffer.  This class extends the corresponding
	/// read/write class, overriding the mutation methods to throw a {@link
	/// ReadOnlyBufferException} and overriding the view-buffer methods to return an
	/// instance of this class rather than of the superclass.
	/// 
	/// </summary>

	internal class HeapFloatBufferR : HeapFloatBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
	
	
	
		*/

		internal HeapFloatBufferR(int cap, int lim) : base(cap, lim) // package-private
		{







			this.IsReadOnly = true;

		}

		internal HeapFloatBufferR(float[] buf, int off, int len) : base(buf, off, len) // package-private
		{







			this.IsReadOnly = true;

		}

		protected internal HeapFloatBufferR(float[] buf, int mark, int pos, int lim, int cap, int off) : base(buf, mark, pos, lim, cap, off)
		{







			this.IsReadOnly = true;

		}

		public override FloatBuffer Slice()
		{
			return new HeapFloatBufferR(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override FloatBuffer Duplicate()
		{
			return new HeapFloatBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override FloatBuffer AsReadOnlyBuffer()
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

		public override FloatBuffer Put(float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Put(int i, float x)
		{




			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Put(float[] src, int offset, int length)
		{








			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Put(FloatBuffer src)
		{























			throw new ReadOnlyBufferException();

		}

		public override FloatBuffer Compact()
		{







			throw new ReadOnlyBufferException();

		}






































































































































































































































































































































































		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}