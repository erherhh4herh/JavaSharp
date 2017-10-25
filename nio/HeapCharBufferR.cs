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
	/// A read-only HeapCharBuffer.  This class extends the corresponding
	/// read/write class, overriding the mutation methods to throw a {@link
	/// ReadOnlyBufferException} and overriding the view-buffer methods to return an
	/// instance of this class rather than of the superclass.
	/// 
	/// </summary>

	internal class HeapCharBufferR : HeapCharBuffer
	{

		// For speed these fields are actually declared in X-Buffer;
		// these declarations are here as documentation
		/*
	
	
	
	
		*/

		internal HeapCharBufferR(int cap, int lim) : base(cap, lim) // package-private
		{







			this.IsReadOnly = true;

		}

		internal HeapCharBufferR(char[] buf, int off, int len) : base(buf, off, len) // package-private
		{







			this.IsReadOnly = true;

		}

		protected internal HeapCharBufferR(char[] buf, int mark, int pos, int lim, int cap, int off) : base(buf, mark, pos, lim, cap, off)
		{







			this.IsReadOnly = true;

		}

		public override CharBuffer Slice()
		{
			return new HeapCharBufferR(Hb, -1, 0, this.Remaining(), this.Remaining(), this.Position() + Offset);
		}

		public override CharBuffer Duplicate()
		{
			return new HeapCharBufferR(Hb, this.MarkValue(), this.Position(), this.Limit(), this.Capacity(), Offset);
		}

		public override CharBuffer AsReadOnlyBuffer()
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

		public override CharBuffer Put(char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(int i, char x)
		{




			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(char[] src, int offset, int length)
		{








			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Put(CharBuffer src)
		{























			throw new ReadOnlyBufferException();

		}

		public override CharBuffer Compact()
		{







			throw new ReadOnlyBufferException();

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
			return new HeapCharBufferR(Hb, -1, pos + start, pos + end, Capacity(), Offset);
		}






		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}



	}

}