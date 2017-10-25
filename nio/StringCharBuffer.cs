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

namespace java.nio
{


	// ## If the sequence is a string, use reflection to share its array

	internal class StringCharBuffer : CharBuffer // package-private
	{
		internal CharSequence Str;

		internal StringCharBuffer(CharSequence s, int start, int end) : base(-1, start, end, s.Length()) // package-private
		{
			int n = s.Length();
			if ((start < 0) || (start > n) || (end < start) || (end > n))
			{
				throw new IndexOutOfBoundsException();
			}
			Str = s;
		}

		public override CharBuffer Slice()
		{
			return new StringCharBuffer(Str, -1, 0, this.Remaining(), this.Remaining(), Offset + this.Position());
		}

		private StringCharBuffer(CharSequence s, int mark, int pos, int limit, int cap, int offset) : base(mark, pos, limit, cap, null, offset)
		{
			Str = s;
		}

		public override CharBuffer Duplicate()
		{
			return new StringCharBuffer(Str, MarkValue(), Position(), Limit(), Capacity(), Offset);
		}

		public override CharBuffer AsReadOnlyBuffer()
		{
			return Duplicate();
		}

		public sealed override char Get()
		{
			return Str.CharAt(NextGetIndex() + Offset);
		}

		public sealed override char Get(int index)
		{
			return Str.CharAt(CheckIndex(index) + Offset);
		}

		internal override char GetUnchecked(int index)
		{
			return Str.CharAt(index + Offset);
		}

		// ## Override bulk get methods for better performance

		public sealed override CharBuffer Put(char c)
		{
			throw new ReadOnlyBufferException();
		}

		public sealed override CharBuffer Put(int index, char c)
		{
			throw new ReadOnlyBufferException();
		}

		public sealed override CharBuffer Compact()
		{
			throw new ReadOnlyBufferException();
		}

		public sealed override bool ReadOnly
		{
			get
			{
				return true;
			}
		}

		internal sealed override String ToString(int start, int end)
		{
			return StringHelperClass.SubstringSpecial(Str.ToString(), start + Offset, end + Offset);
		}

		public sealed override CharBuffer SubSequence(int start, int end)
		{
			try
			{
				int pos = Position();
				return new StringCharBuffer(Str, -1, pos + CheckIndex(start, pos), pos + CheckIndex(end, pos), Capacity(), Offset);
			}
			catch (IllegalArgumentException)
			{
				throw new IndexOutOfBoundsException();
			}
		}

		public override bool Direct
		{
			get
			{
				return false;
			}
		}

		public override ByteOrder Order()
		{
			return ByteOrder.NativeOrder();
		}

	}

}