using System.Diagnostics;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio
{


	/// <summary>
	/// A Spliterator.OfInt for sources that traverse and split elements
	/// maintained in a CharBuffer.
	/// 
	/// @implNote
	/// The implementation is based on the code for the Array-based spliterators.
	/// </summary>
	internal class CharBufferSpliterator : java.util.Spliterator_OfInt
	{
		private readonly CharBuffer Buffer;
		private int Index; // current index, modified on advance/split
		private readonly int Limit;

		internal CharBufferSpliterator(CharBuffer buffer) : this(buffer, buffer.Position(), buffer.Limit())
		{
		}

		internal CharBufferSpliterator(CharBuffer buffer, int origin, int limit)
		{
			Debug.Assert(origin <= limit);
			this.Buffer = buffer;
			this.Index = (origin <= limit) ? origin : limit;
			this.Limit = limit;
		}

		public virtual OfInt TrySplit()
		{
			int lo = Index, mid = (int)((uint)(lo + Limit) >> 1);
			return (lo >= mid) ? null : new CharBufferSpliterator(Buffer, lo, Index = mid);
		}

		public override void ForEachRemaining(IntConsumer action)
		{
			if (action == null)
			{
				throw new NullPointerException();
			}
			CharBuffer cb = Buffer;
			int i = Index;
			int hi = Limit;
			Index = hi;
			while (i < hi)
			{
				action.Accept(cb.GetUnchecked(i++));
			}
		}

		public virtual bool TryAdvance(IntConsumer action)
		{
			if (action == null)
			{
				throw new NullPointerException();
			}
			if (Index >= 0 && Index < Limit)
			{
				action.Accept(Buffer.GetUnchecked(Index++));
				return true;
			}
			return false;
		}

		public override long EstimateSize()
		{
			return (long)(Limit - Index);
		}

		public override int Characteristics()
		{
			return Buffer.SPLITERATOR_CHARACTERISTICS;
		}
	}

}