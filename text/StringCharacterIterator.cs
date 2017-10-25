/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

/*
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
 *
 * The original version of this source code and documentation
 * is copyrighted and owned by Taligent, Inc., a wholly-owned
 * subsidiary of IBM. These materials are provided under terms
 * of a License Agreement between Taligent and Sun. This technology
 * is protected by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{

	/// <summary>
	/// <code>StringCharacterIterator</code> implements the
	/// <code>CharacterIterator</code> protocol for a <code>String</code>.
	/// The <code>StringCharacterIterator</code> class iterates over the
	/// entire <code>String</code>.
	/// </summary>
	/// <seealso cref= CharacterIterator </seealso>

	public sealed class StringCharacterIterator : CharacterIterator
	{
		private String Text_Renamed;
		private int Begin;
		private int End;
		// invariant: begin <= pos <= end
		private int Pos;

		/// <summary>
		/// Constructs an iterator with an initial index of 0.
		/// </summary>
		/// <param name="text"> the {@code String} to be iterated over </param>
		public StringCharacterIterator(String text) : this(text, 0)
		{
		}

		/// <summary>
		/// Constructs an iterator with the specified initial index.
		/// </summary>
		/// <param name="text">   The String to be iterated over </param>
		/// <param name="pos">    Initial iterator position </param>
		public StringCharacterIterator(String text, int pos) : this(text, 0, text.Length(), pos)
		{
		}

		/// <summary>
		/// Constructs an iterator over the given range of the given string, with the
		/// index set at the specified position.
		/// </summary>
		/// <param name="text">   The String to be iterated over </param>
		/// <param name="begin">  Index of the first character </param>
		/// <param name="end">    Index of the character following the last character </param>
		/// <param name="pos">    Initial iterator position </param>
		public StringCharacterIterator(String text, int begin, int end, int pos)
		{
			if (text == null)
			{
				throw new NullPointerException();
			}
			this.Text_Renamed = text;

			if (begin < 0 || begin > end || end > text.Length())
			{
				throw new IllegalArgumentException("Invalid substring range");
			}

			if (pos < begin || pos > end)
			{
				throw new IllegalArgumentException("Invalid position");
			}

			this.Begin = begin;
			this.End = end;
			this.Pos = pos;
		}

		/// <summary>
		/// Reset this iterator to point to a new string.  This package-visible
		/// method is used by other java.text classes that want to avoid allocating
		/// new StringCharacterIterator objects every time their setText method
		/// is called.
		/// </summary>
		/// <param name="text">   The String to be iterated over
		/// @since 1.2 </param>
		public String Text
		{
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
				this.Text_Renamed = value;
				this.Begin = 0;
				this.End = value.Length();
				this.Pos = 0;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.first() for String. </summary>
		/// <seealso cref= CharacterIterator#first </seealso>
		public char First()
		{
			Pos = Begin;
			return Current();
		}

		/// <summary>
		/// Implements CharacterIterator.last() for String. </summary>
		/// <seealso cref= CharacterIterator#last </seealso>
		public char Last()
		{
			if (End != Begin)
			{
				Pos = End - 1;
			}
			else
			{
				Pos = End;
			}
			return Current();
		}

		/// <summary>
		/// Implements CharacterIterator.setIndex() for String. </summary>
		/// <seealso cref= CharacterIterator#setIndex </seealso>
		public char SetIndex(int p)
		{
		if (p < Begin || p > End)
		{
				throw new IllegalArgumentException("Invalid index");
		}
			Pos = p;
			return Current();
		}

		/// <summary>
		/// Implements CharacterIterator.current() for String. </summary>
		/// <seealso cref= CharacterIterator#current </seealso>
		public char Current()
		{
			if (Pos >= Begin && Pos < End)
			{
				return Text_Renamed.CharAt(Pos);
			}
			else
			{
				return CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.next() for String. </summary>
		/// <seealso cref= CharacterIterator#next </seealso>
		public char Next()
		{
			if (Pos < End - 1)
			{
				Pos++;
				return Text_Renamed.CharAt(Pos);
			}
			else
			{
				Pos = End;
				return CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.previous() for String. </summary>
		/// <seealso cref= CharacterIterator#previous </seealso>
		public char Previous()
		{
			if (Pos > Begin)
			{
				Pos--;
				return Text_Renamed.CharAt(Pos);
			}
			else
			{
				return CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.getBeginIndex() for String. </summary>
		/// <seealso cref= CharacterIterator#getBeginIndex </seealso>
		public int BeginIndex
		{
			get
			{
				return Begin;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.getEndIndex() for String. </summary>
		/// <seealso cref= CharacterIterator#getEndIndex </seealso>
		public int EndIndex
		{
			get
			{
				return End;
			}
		}

		/// <summary>
		/// Implements CharacterIterator.getIndex() for String. </summary>
		/// <seealso cref= CharacterIterator#getIndex </seealso>
		public int Index
		{
			get
			{
				return Pos;
			}
		}

		/// <summary>
		/// Compares the equality of two StringCharacterIterator objects. </summary>
		/// <param name="obj"> the StringCharacterIterator object to be compared with. </param>
		/// <returns> true if the given obj is the same as this
		/// StringCharacterIterator object; false otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is StringCharacterIterator))
			{
				return false;
			}

			StringCharacterIterator that = (StringCharacterIterator) obj;

			if (HashCode() != that.HashCode())
			{
				return false;
			}
			if (!Text_Renamed.Equals(that.Text_Renamed))
			{
				return false;
			}
			if (Pos != that.Pos || Begin != that.Begin || End != that.End)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Computes a hashcode for this iterator. </summary>
		/// <returns> A hash code </returns>
		public override int HashCode()
		{
			return Text_Renamed.HashCode() ^ Pos ^ Begin ^ End;
		}

		/// <summary>
		/// Creates a copy of this iterator. </summary>
		/// <returns> A copy of this </returns>
		public Object Clone()
		{
			try
			{
				StringCharacterIterator other = (StringCharacterIterator) base.Clone();
				return other;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

	}

}