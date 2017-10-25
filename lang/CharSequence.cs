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

namespace java.lang
{


	/// <summary>
	/// A <tt>CharSequence</tt> is a readable sequence of <code>char</code> values. This
	/// interface provides uniform, read-only access to many different kinds of
	/// <code>char</code> sequences.
	/// A <code>char</code> value represents a character in the <i>Basic
	/// Multilingual Plane (BMP)</i> or a surrogate. Refer to <a
	/// href="Character.html#unicode">Unicode Character Representation</a> for details.
	/// 
	/// <para> This interface does not refine the general contracts of the {@link
	/// java.lang.Object#equals(java.lang.Object) equals} and {@link
	/// java.lang.Object#hashCode() hashCode} methods.  The result of comparing two
	/// objects that implement <tt>CharSequence</tt> is therefore, in general,
	/// undefined.  Each object may be implemented by a different class, and there
	/// is no guarantee that each class will be capable of testing its instances
	/// for equality with those of the other.  It is therefore inappropriate to use
	/// arbitrary <tt>CharSequence</tt> instances as elements in a set or as keys in
	/// a map. </para>
	/// 
	/// @author Mike McCloskey
	/// @since 1.4
	/// @spec JSR-51
	/// </summary>

	public interface CharSequence
	{

		/// <summary>
		/// Returns the length of this character sequence.  The length is the number
		/// of 16-bit <code>char</code>s in the sequence.
		/// </summary>
		/// <returns>  the number of <code>char</code>s in this sequence </returns>
		int Length();

		/// <summary>
		/// Returns the <code>char</code> value at the specified index.  An index ranges from zero
		/// to <tt>length() - 1</tt>.  The first <code>char</code> value of the sequence is at
		/// index zero, the next at index one, and so on, as for array
		/// indexing.
		/// 
		/// <para>If the <code>char</code> value specified by the index is a
		/// <a href="{@docRoot}/java/lang/Character.html#unicode">surrogate</a>, the surrogate
		/// value is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the <code>char</code> value to be returned
		/// </param>
		/// <returns>  the specified <code>char</code> value
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          if the <tt>index</tt> argument is negative or not less than
		///          <tt>length()</tt> </exception>
		char CharAt(int index);

		/// <summary>
		/// Returns a <code>CharSequence</code> that is a subsequence of this sequence.
		/// The subsequence starts with the <code>char</code> value at the specified index and
		/// ends with the <code>char</code> value at index <tt>end - 1</tt>.  The length
		/// (in <code>char</code>s) of the
		/// returned sequence is <tt>end - start</tt>, so if <tt>start == end</tt>
		/// then an empty sequence is returned.
		/// </summary>
		/// <param name="start">   the start index, inclusive </param>
		/// <param name="end">     the end index, exclusive
		/// </param>
		/// <returns>  the specified subsequence
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          if <tt>start</tt> or <tt>end</tt> are negative,
		///          if <tt>end</tt> is greater than <tt>length()</tt>,
		///          or if <tt>start</tt> is greater than <tt>end</tt> </exception>
		CharSequence SubSequence(int start, int end);

		/// <summary>
		/// Returns a string containing the characters in this sequence in the same
		/// order as this sequence.  The length of the string will be the length of
		/// this sequence.
		/// </summary>
		/// <returns>  a string consisting of exactly this sequence of characters </returns>
		String ToString();

		/// <summary>
		/// Returns a stream of {@code int} zero-extending the {@code char} values
		/// from this sequence.  Any char which maps to a <a
		/// href="{@docRoot}/java/lang/Character.html#unicode">surrogate code
		/// point</a> is passed through uninterpreted.
		/// 
		/// <para>If the sequence is mutated while the stream is being read, the
		/// result is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an IntStream of char values from this sequence
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		public default java.util.stream.IntStream chars()
	//	{
	//
	//		return StreamSupport.intStream(() -> Spliterators.spliterator(new CharIterator(), length(), Spliterator.ORDERED), Spliterator.SUBSIZED | Spliterator.SIZED | Spliterator.ORDERED, false);
	//	}

		/// <summary>
		/// Returns a stream of code point values from this sequence.  Any surrogate
		/// pairs encountered in the sequence are combined as if by {@linkplain
		/// Character#toCodePoint Character.toCodePoint} and the result is passed
		/// to the stream. Any other code units, including ordinary BMP characters,
		/// unpaired surrogates, and undefined code units, are zero-extended to
		/// {@code int} values which are then passed to the stream.
		/// 
		/// <para>If the sequence is mutated while the stream is being read, the result
		/// is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an IntStream of Unicode code points from this sequence
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		public default java.util.stream.IntStream codePoints()
	//	{
	//
	//		return StreamSupport.intStream(() -> Spliterators.spliteratorUnknownSize(new CodePointIterator(), Spliterator.ORDERED), Spliterator.ORDERED, false);
	//	}
	}

		public class CharSequence_CharIterator : java.util.PrimitiveIterator_OfInt
		{
			internal int Cur = 0;

			public virtual bool HasNext()
			{
				return Cur < length();
			}

			public virtual int NextInt()
			{
				if (HasNext())
				{
					return charAt(Cur++);
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			public virtual void ForEachRemaining(IntConsumer block)
			{
				for (; Cur < length(); Cur++)
				{
					block.Accept(charAt(Cur));
				}
			}
		}

		public class CharSequence_CodePointIterator : java.util.PrimitiveIterator_OfInt
		{
			internal int Cur = 0;

			public virtual void ForEachRemaining(IntConsumer block)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = length();
				int length = length();
				int i = Cur;
				try
				{
					while (i < length)
					{
						char c1 = charAt(i++);
						if (!char.IsHighSurrogate(c1) || i >= length)
						{
							block.Accept(c1);
						}
						else
						{
							char c2 = charAt(i);
							if (char.IsLowSurrogate(c2))
							{
								i++;
								block.Accept(Character.ToCodePoint(c1, c2));
							}
							else
							{
								block.Accept(c1);
							}
						}
					}
				}
				finally
				{
					Cur = i;
				}
			}

			public virtual bool HasNext()
			{
				return Cur < length();
			}

			public virtual int NextInt()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = length();
				int length = length();

				if (Cur >= length)
				{
					throw new NoSuchElementException();
				}
				char c1 = charAt(Cur++);
				if (char.IsHighSurrogate(c1) && Cur < length)
				{
					char c2 = charAt(Cur);
					if (char.IsLowSurrogate(c2))
					{
						Cur++;
						return Character.ToCodePoint(c1, c2);
					}
				}
				return c1;
			}
		}

}