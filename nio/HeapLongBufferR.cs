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
	/// A read-only HeapLongBuffer.  This class extends the corresponding
	/// read/write class, overriding the mutation methods to throw a {@link
	/// ReadOnlyBufferException} and overriding the view-buffer methods to return an
	/// instance of this class rather than of the superclass.
	/// 
	/// </summary>

	internal class HeapLongBufferR : HeapLongBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
	
	
	
		*/

		internal HeapLongBufferR(int cap, int lim) : base(cap, lim) // package-private
		{







			this.IsReadOnly = true;

		}

		internal HeapLongBufferR(long[] buf, int off, int len) : base(buf, off, len) // package-private
		{







			this.IsReadOnly = true;

		}

		protected internal HeapLongBufferR(long[] buf, int mark, int pos, int lim, int cap, int off) : base(buf, mark, pos, lim, cap, off)
		{







			this.IsReadOnly = true;

		}

		public override LongBuffer Slice()
		{
			return new HeapLongBufferR(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override LongBuffer Duplicate()
		{
			return new HeapLongBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override LongBuffer AsReadOnlyBuffer()
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

		public override LongBuffer Put(long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Put(int i, long x)
		{




			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Put(long[] src, int offset, int length)
		{








			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Put(LongBuffer src)
		{























			throw new ReadOnlyBufferException();

		}

		public override LongBuffer Compact()
		{







			throw new ReadOnlyBufferException();

		}






































































































































































































































































































































































		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}