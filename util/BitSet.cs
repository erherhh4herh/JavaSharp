using System;

/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{


	/// <summary>
	/// This class implements a vector of bits that grows as needed. Each
	/// component of the bit set has a {@code boolean} value. The
	/// bits of a {@code BitSet} are indexed by nonnegative integers.
	/// Individual indexed bits can be examined, set, or cleared. One
	/// {@code BitSet} may be used to modify the contents of another
	/// {@code BitSet} through logical AND, logical inclusive OR, and
	/// logical exclusive OR operations.
	/// 
	/// <para>By default, all bits in the set initially have the value
	/// {@code false}.
	/// 
	/// </para>
	/// <para>Every bit set has a current size, which is the number of bits
	/// of space currently in use by the bit set. Note that the size is
	/// related to the implementation of a bit set, so it may change with
	/// implementation. The length of a bit set relates to logical length
	/// of a bit set and is defined independently of implementation.
	/// 
	/// </para>
	/// <para>Unless otherwise noted, passing a null parameter to any of the
	/// methods in a {@code BitSet} will result in a
	/// {@code NullPointerException}.
	/// 
	/// </para>
	/// <para>A {@code BitSet} is not safe for multithreaded use without
	/// external synchronization.
	/// 
	/// @author  Arthur van Hoff
	/// @author  Michael McCloskey
	/// @author  Martin Buchholz
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class BitSet : Cloneable
	{
		/*
		 * BitSets are packed into arrays of "words."  Currently a word is
		 * a long, which consists of 64 bits, requiring 6 address bits.
		 * The choice of word size is determined purely by performance concerns.
		 */
		private const int ADDRESS_BITS_PER_WORD = 6;
		private static readonly int BITS_PER_WORD = 1 << ADDRESS_BITS_PER_WORD;
		private static readonly int BIT_INDEX_MASK = BITS_PER_WORD - 1;

		/* Used to shift left or right for a partial word mask */
		private const long WORD_MASK = unchecked((long)0xffffffffffffffffL);

		/// <summary>
		/// @serialField bits long[]
		/// 
		/// The bits in this BitSet.  The ith bit is stored in bits[i/64] at
		/// bit position i % 64 (where bit position 0 refers to the least
		/// significant bit and 63 refers to the most significant bit).
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("bits", typeof(long[]))};

		/// <summary>
		/// The internal field corresponding to the serialField "bits".
		/// </summary>
		private long[] Words;

		/// <summary>
		/// The number of words in the logical size of this BitSet.
		/// </summary>
		[NonSerialized]
		private int WordsInUse = 0;

		/// <summary>
		/// Whether the size of "words" is user-specified.  If so, we assume
		/// the user knows what he's doing and try harder to preserve it.
		/// </summary>
		[NonSerialized]
		private bool SizeIsSticky = false;

		/* use serialVersionUID from JDK 1.0.2 for interoperability */
		private const long SerialVersionUID = 7997698588986878753L;

		/// <summary>
		/// Given a bit index, return word index containing it.
		/// </summary>
		private static int WordIndex(int bitIndex)
		{
			return bitIndex >> ADDRESS_BITS_PER_WORD;
		}

		/// <summary>
		/// Every public method must preserve these invariants.
		/// </summary>
		private void CheckInvariants()
		{
			assert(WordsInUse == 0 || Words[WordsInUse - 1] != 0);
			assert(WordsInUse >= 0 && WordsInUse <= Words.Length);
			assert(WordsInUse == Words.Length || Words[WordsInUse] == 0);
		}

		/// <summary>
		/// Sets the field wordsInUse to the logical size in words of the bit set.
		/// WARNING:This method assumes that the number of words actually in use is
		/// less than or equal to the current value of wordsInUse!
		/// </summary>
		private void RecalculateWordsInUse()
		{
			// Traverse the bitset until a used word is found
			int i;
			for (i = WordsInUse-1; i >= 0; i--)
			{
				if (Words[i] != 0)
				{
					break;
				}
			}

			WordsInUse = i + 1; // The new logical size
		}

		/// <summary>
		/// Creates a new bit set. All bits are initially {@code false}.
		/// </summary>
		public BitSet()
		{
			InitWords(BITS_PER_WORD);
			SizeIsSticky = false;
		}

		/// <summary>
		/// Creates a bit set whose initial size is large enough to explicitly
		/// represent bits with indices in the range {@code 0} through
		/// {@code nbits-1}. All bits are initially {@code false}.
		/// </summary>
		/// <param name="nbits"> the initial size of the bit set </param>
		/// <exception cref="NegativeArraySizeException"> if the specified initial size
		///         is negative </exception>
		public BitSet(int nbits)
		{
			// nbits can't be negative; size 0 is OK
			if (nbits < 0)
			{
				throw new NegativeArraySizeException("nbits < 0: " + nbits);
			}

			InitWords(nbits);
			SizeIsSticky = true;
		}

		private void InitWords(int nbits)
		{
			Words = new long[WordIndex(nbits - 1) + 1];
		}

		/// <summary>
		/// Creates a bit set using words as the internal representation.
		/// The last word (if there is one) must be non-zero.
		/// </summary>
		private BitSet(long[] words)
		{
			this.Words = words;
			this.WordsInUse = words.Length;
			CheckInvariants();
		}

		/// <summary>
		/// Returns a new bit set containing all the bits in the given long array.
		/// 
		/// <para>More precisely,
		/// <br>{@code BitSet.valueOf(longs).get(n) == ((longs[n/64] & (1L<<(n%64))) != 0)}
		/// <br>for all {@code n < 64 * longs.length}.
		/// 
		/// </para>
		/// <para>This method is equivalent to
		/// {@code BitSet.valueOf(LongBuffer.wrap(longs))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="longs"> a long array containing a little-endian representation
		///        of a sequence of bits to be used as the initial bits of the
		///        new bit set </param>
		/// <returns> a {@code BitSet} containing all the bits in the long array
		/// @since 1.7 </returns>
		public static BitSet ValueOf(long[] longs)
		{
			int n;
			for (n = longs.Length; n > 0 && longs[n - 1] == 0; n--)
			{
				;
			}
			return new BitSet(Arrays.CopyOf(longs, n));
		}

		/// <summary>
		/// Returns a new bit set containing all the bits in the given long
		/// buffer between its position and limit.
		/// 
		/// <para>More precisely,
		/// <br>{@code BitSet.valueOf(lb).get(n) == ((lb.get(lb.position()+n/64) & (1L<<(n%64))) != 0)}
		/// <br>for all {@code n < 64 * lb.remaining()}.
		/// 
		/// </para>
		/// <para>The long buffer is not modified by this method, and no
		/// reference to the buffer is retained by the bit set.
		/// 
		/// </para>
		/// </summary>
		/// <param name="lb"> a long buffer containing a little-endian representation
		///        of a sequence of bits between its position and limit, to be
		///        used as the initial bits of the new bit set </param>
		/// <returns> a {@code BitSet} containing all the bits in the buffer in the
		///         specified range
		/// @since 1.7 </returns>
		public static BitSet ValueOf(LongBuffer lb)
		{
			lb = lb.Slice();
			int n;
			for (n = lb.Remaining(); n > 0 && lb.Get(n - 1) == 0; n--)
			{
				;
			}
			long[] words = new long[n];
			lb.Get(words);
			return new BitSet(words);
		}

		/// <summary>
		/// Returns a new bit set containing all the bits in the given byte array.
		/// 
		/// <para>More precisely,
		/// <br>{@code BitSet.valueOf(bytes).get(n) == ((bytes[n/8] & (1<<(n%8))) != 0)}
		/// <br>for all {@code n <  8 * bytes.length}.
		/// 
		/// </para>
		/// <para>This method is equivalent to
		/// {@code BitSet.valueOf(ByteBuffer.wrap(bytes))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes"> a byte array containing a little-endian
		///        representation of a sequence of bits to be used as the
		///        initial bits of the new bit set </param>
		/// <returns> a {@code BitSet} containing all the bits in the byte array
		/// @since 1.7 </returns>
		public static BitSet ValueOf(sbyte[] bytes)
		{
			return BitSet.ValueOf(ByteBuffer.Wrap(bytes));
		}

		/// <summary>
		/// Returns a new bit set containing all the bits in the given byte
		/// buffer between its position and limit.
		/// 
		/// <para>More precisely,
		/// <br>{@code BitSet.valueOf(bb).get(n) == ((bb.get(bb.position()+n/8) & (1<<(n%8))) != 0)}
		/// <br>for all {@code n < 8 * bb.remaining()}.
		/// 
		/// </para>
		/// <para>The byte buffer is not modified by this method, and no
		/// reference to the buffer is retained by the bit set.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bb"> a byte buffer containing a little-endian representation
		///        of a sequence of bits between its position and limit, to be
		///        used as the initial bits of the new bit set </param>
		/// <returns> a {@code BitSet} containing all the bits in the buffer in the
		///         specified range
		/// @since 1.7 </returns>
		public static BitSet ValueOf(ByteBuffer bb)
		{
			bb = bb.Slice().Order(ByteOrder.LITTLE_ENDIAN);
			int n;
			for (n = bb.Remaining(); n > 0 && bb.Get(n - 1) == 0; n--)
			{
				;
			}
			long[] words = new long[(n + 7) / 8];
			bb.Limit(n);
			int i = 0;
			while (bb.Remaining() >= 8)
			{
				words[i++] = bb.Long;
			}
			for (int remaining = bb.Remaining(), j = 0; j < remaining; j++)
			{
				words[i] |= (bb.Get() & 0xffL) << (8 * j);
			}
			return new BitSet(words);
		}

		/// <summary>
		/// Returns a new byte array containing all the bits in this bit set.
		/// 
		/// <para>More precisely, if
		/// <br>{@code byte[] bytes = s.toByteArray();}
		/// <br>then {@code bytes.length == (s.length()+7)/8} and
		/// <br>{@code s.get(n) == ((bytes[n/8] & (1<<(n%8))) != 0)}
		/// <br>for all {@code n < 8 * bytes.length}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a byte array containing a little-endian representation
		///         of all the bits in this bit set
		/// @since 1.7 </returns>
		public virtual sbyte[] ToByteArray()
		{
			int n = WordsInUse;
			if (n == 0)
			{
				return new sbyte[0];
			}
			int len = 8 * (n - 1);
			for (long x = Words[n - 1]; x != 0; x = (long)((ulong)x >> 8))
			{
				len++;
			}
			sbyte[] bytes = new sbyte[len];
			ByteBuffer bb = ByteBuffer.Wrap(bytes).Order(ByteOrder.LITTLE_ENDIAN);
			for (int i = 0; i < n - 1; i++)
			{
				bb.PutLong(Words[i]);
			}
			for (long x = Words[n - 1]; x != 0; x = (long)((ulong)x >> 8))
			{
				bb.Put(unchecked((sbyte)(x & 0xff)));
			}
			return bytes;
		}

		/// <summary>
		/// Returns a new long array containing all the bits in this bit set.
		/// 
		/// <para>More precisely, if
		/// <br>{@code long[] longs = s.toLongArray();}
		/// <br>then {@code longs.length == (s.length()+63)/64} and
		/// <br>{@code s.get(n) == ((longs[n/64] & (1L<<(n%64))) != 0)}
		/// <br>for all {@code n < 64 * longs.length}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a long array containing a little-endian representation
		///         of all the bits in this bit set
		/// @since 1.7 </returns>
		public virtual long[] ToLongArray()
		{
			return Arrays.CopyOf(Words, WordsInUse);
		}

		/// <summary>
		/// Ensures that the BitSet can hold enough words. </summary>
		/// <param name="wordsRequired"> the minimum acceptable number of words. </param>
		private void EnsureCapacity(int wordsRequired)
		{
			if (Words.Length < wordsRequired)
			{
				// Allocate larger of doubled size or required size
				int request = System.Math.Max(2 * Words.Length, wordsRequired);
				Words = Arrays.CopyOf(Words, request);
				SizeIsSticky = false;
			}
		}

		/// <summary>
		/// Ensures that the BitSet can accommodate a given wordIndex,
		/// temporarily violating the invariants.  The caller must
		/// restore the invariants before returning to the user,
		/// possibly using recalculateWordsInUse(). </summary>
		/// <param name="wordIndex"> the index to be accommodated. </param>
		private void ExpandTo(int wordIndex)
		{
			int wordsRequired = wordIndex + 1;
			if (WordsInUse < wordsRequired)
			{
				EnsureCapacity(wordsRequired);
				WordsInUse = wordsRequired;
			}
		}

		/// <summary>
		/// Checks that fromIndex ... toIndex is a valid range of bit indices.
		/// </summary>
		private static void CheckRange(int fromIndex, int toIndex)
		{
			if (fromIndex < 0)
			{
				throw new IndexOutOfBoundsException("fromIndex < 0: " + fromIndex);
			}
			if (toIndex < 0)
			{
				throw new IndexOutOfBoundsException("toIndex < 0: " + toIndex);
			}
			if (fromIndex > toIndex)
			{
				throw new IndexOutOfBoundsException("fromIndex: " + fromIndex + " > toIndex: " + toIndex);
			}
		}

		/// <summary>
		/// Sets the bit at the specified index to the complement of its
		/// current value.
		/// </summary>
		/// <param name="bitIndex"> the index of the bit to flip </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  1.4 </exception>
		public virtual void Flip(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new IndexOutOfBoundsException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = WordIndex(bitIndex);
			ExpandTo(wordIndex);

			Words[wordIndex] ^= (1L << bitIndex);

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Sets each bit from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to the complement of its current
		/// value.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to flip </param>
		/// <param name="toIndex"> index after the last bit to flip </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since  1.4 </exception>
		public virtual void Flip(int fromIndex, int toIndex)
		{
			CheckRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			int startWordIndex = WordIndex(fromIndex);
			int endWordIndex = WordIndex(toIndex - 1);
			ExpandTo(endWordIndex);

			long firstWordMask = WORD_MASK << fromIndex;
			long lastWordMask = (long)((ulong)WORD_MASK >> -toIndex);
			if (startWordIndex == endWordIndex)
			{
				// Case 1: One word
				Words[startWordIndex] ^= (firstWordMask & lastWordMask);
			}
			else
			{
				// Case 2: Multiple words
				// Handle first word
				Words[startWordIndex] ^= firstWordMask;

				// Handle intermediate words, if any
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					Words[i] ^= WORD_MASK;
				}

				// Handle last word
				Words[endWordIndex] ^= lastWordMask;
			}

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Sets the bit at the specified index to {@code true}.
		/// </summary>
		/// <param name="bitIndex"> a bit index </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  JDK1.0 </exception>
		public virtual void Set(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new IndexOutOfBoundsException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = WordIndex(bitIndex);
			ExpandTo(wordIndex);

			Words[wordIndex] |= (1L << bitIndex); // Restores invariants

			CheckInvariants();
		}

		/// <summary>
		/// Sets the bit at the specified index to the specified value.
		/// </summary>
		/// <param name="bitIndex"> a bit index </param>
		/// <param name="value"> a boolean value to set </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  1.4 </exception>
		public virtual void Set(int bitIndex, bool value)
		{
			if (value)
			{
				Set(bitIndex);
			}
			else
			{
				Clear(bitIndex);
			}
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to {@code true}.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be set </param>
		/// <param name="toIndex"> index after the last bit to be set </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since  1.4 </exception>
		public virtual void Set(int fromIndex, int toIndex)
		{
			CheckRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			// Increase capacity if necessary
			int startWordIndex = WordIndex(fromIndex);
			int endWordIndex = WordIndex(toIndex - 1);
			ExpandTo(endWordIndex);

			long firstWordMask = WORD_MASK << fromIndex;
			long lastWordMask = (long)((ulong)WORD_MASK >> -toIndex);
			if (startWordIndex == endWordIndex)
			{
				// Case 1: One word
				Words[startWordIndex] |= (firstWordMask & lastWordMask);
			}
			else
			{
				// Case 2: Multiple words
				// Handle first word
				Words[startWordIndex] |= firstWordMask;

				// Handle intermediate words, if any
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					Words[i] = WORD_MASK;
				}

				// Handle last word (restores invariants)
				Words[endWordIndex] |= lastWordMask;
			}

			CheckInvariants();
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to the specified value.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be set </param>
		/// <param name="toIndex"> index after the last bit to be set </param>
		/// <param name="value"> value to set the selected bits to </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since  1.4 </exception>
		public virtual void Set(int fromIndex, int toIndex, bool value)
		{
			if (value)
			{
				Set(fromIndex, toIndex);
			}
			else
			{
				Clear(fromIndex, toIndex);
			}
		}

		/// <summary>
		/// Sets the bit specified by the index to {@code false}.
		/// </summary>
		/// <param name="bitIndex"> the index of the bit to be cleared </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  JDK1.0 </exception>
		public virtual void Clear(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new IndexOutOfBoundsException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = WordIndex(bitIndex);
			if (wordIndex >= WordsInUse)
			{
				return;
			}

			Words[wordIndex] &= ~(1L << bitIndex);

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to {@code false}.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be cleared </param>
		/// <param name="toIndex"> index after the last bit to be cleared </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since  1.4 </exception>
		public virtual void Clear(int fromIndex, int toIndex)
		{
			CheckRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			int startWordIndex = WordIndex(fromIndex);
			if (startWordIndex >= WordsInUse)
			{
				return;
			}

			int endWordIndex = WordIndex(toIndex - 1);
			if (endWordIndex >= WordsInUse)
			{
				toIndex = Length();
				endWordIndex = WordsInUse - 1;
			}

			long firstWordMask = WORD_MASK << fromIndex;
			long lastWordMask = (long)((ulong)WORD_MASK >> -toIndex);
			if (startWordIndex == endWordIndex)
			{
				// Case 1: One word
				Words[startWordIndex] &= ~(firstWordMask & lastWordMask);
			}
			else
			{
				// Case 2: Multiple words
				// Handle first word
				Words[startWordIndex] &= ~firstWordMask;

				// Handle intermediate words, if any
				for (int i = startWordIndex + 1; i < endWordIndex; i++)
				{
					Words[i] = 0;
				}

				// Handle last word
				Words[endWordIndex] &= ~lastWordMask;
			}

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Sets all of the bits in this BitSet to {@code false}.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual void Clear()
		{
			while (WordsInUse > 0)
			{
				Words[--WordsInUse] = 0;
			}
		}

		/// <summary>
		/// Returns the value of the bit with the specified index. The value
		/// is {@code true} if the bit with the index {@code bitIndex}
		/// is currently set in this {@code BitSet}; otherwise, the result
		/// is {@code false}.
		/// </summary>
		/// <param name="bitIndex">   the bit index </param>
		/// <returns> the value of the bit with the specified index </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		public virtual bool Get(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new IndexOutOfBoundsException("bitIndex < 0: " + bitIndex);
			}

			CheckInvariants();

			int wordIndex = WordIndex(bitIndex);
			return (wordIndex < WordsInUse) && ((Words[wordIndex] & (1L << bitIndex)) != 0);
		}

		/// <summary>
		/// Returns a new {@code BitSet} composed of bits from this {@code BitSet}
		/// from {@code fromIndex} (inclusive) to {@code toIndex} (exclusive).
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to include </param>
		/// <param name="toIndex"> index after the last bit to include </param>
		/// <returns> a new {@code BitSet} from a range of this {@code BitSet} </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since  1.4 </exception>
		public virtual BitSet Get(int fromIndex, int toIndex)
		{
			CheckRange(fromIndex, toIndex);

			CheckInvariants();

			int len = Length();

			// If no set bits in range return empty bitset
			if (len <= fromIndex || fromIndex == toIndex)
			{
				return new BitSet(0);
			}

			// An optimization
			if (toIndex > len)
			{
				toIndex = len;
			}

			BitSet result = new BitSet(toIndex - fromIndex);
			int targetWords = WordIndex(toIndex - fromIndex - 1) + 1;
			int sourceIndex = WordIndex(fromIndex);
			bool wordAligned = ((fromIndex & BIT_INDEX_MASK) == 0);

			// Process all words but the last word
			for (int i = 0; i < targetWords - 1; i++, sourceIndex++)
			{
				result.Words[i] = wordAligned ? Words[sourceIndex] : ((long)((ulong)Words[sourceIndex] >> fromIndex)) | (Words[sourceIndex + 1] << -fromIndex);
			}

			// Process the last word
			long lastWordMask = (long)((ulong)WORD_MASK >> -toIndex);
			result.Words[targetWords - 1] = ((toIndex - 1) & BIT_INDEX_MASK) < (fromIndex & BIT_INDEX_MASK) ? (((long)((ulong)Words[sourceIndex] >> fromIndex)) | (Words[sourceIndex + 1] & lastWordMask) << -fromIndex) : ((int)((uint)(Words[sourceIndex] & lastWordMask) >> fromIndex)); // straddles source words

			// Set wordsInUse correctly
			result.WordsInUse = targetWords;
			result.RecalculateWordsInUse();
			result.CheckInvariants();

			return result;
		}

		/// <summary>
		/// Returns the index of the first bit that is set to {@code true}
		/// that occurs on or after the specified starting index. If no such
		/// bit exists then {@code -1} is returned.
		/// 
		/// <para>To iterate over the {@code true} bits in a {@code BitSet},
		/// use the following loop:
		/// 
		///  <pre> {@code
		/// for (int i = bs.nextSetBit(0); i >= 0; i = bs.nextSetBit(i+1)) {
		///     // operate on index i here
		///     if (i == Integer.MAX_VALUE) {
		///         break; // or (i+1) would overflow
		///     }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the next set bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  1.4 </exception>
		public virtual int NextSetBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				throw new IndexOutOfBoundsException("fromIndex < 0: " + fromIndex);
			}

			CheckInvariants();

			int u = WordIndex(fromIndex);
			if (u >= WordsInUse)
			{
				return -1;
			}

			long word = Words[u] & (WORD_MASK << fromIndex);

			while (true)
			{
				if (word != 0)
				{
					return (u * BITS_PER_WORD) + Long.NumberOfTrailingZeros(word);
				}
				if (++u == WordsInUse)
				{
					return -1;
				}
				word = Words[u];
			}
		}

		/// <summary>
		/// Returns the index of the first bit that is set to {@code false}
		/// that occurs on or after the specified starting index.
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the next clear bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since  1.4 </exception>
		public virtual int NextClearBit(int fromIndex)
		{
			// Neither spec nor implementation handle bitsets of maximal length.
			// See 4816253.
			if (fromIndex < 0)
			{
				throw new IndexOutOfBoundsException("fromIndex < 0: " + fromIndex);
			}

			CheckInvariants();

			int u = WordIndex(fromIndex);
			if (u >= WordsInUse)
			{
				return fromIndex;
			}

			long word = ~Words[u] & (WORD_MASK << fromIndex);

			while (true)
			{
				if (word != 0)
				{
					return (u * BITS_PER_WORD) + Long.NumberOfTrailingZeros(word);
				}
				if (++u == WordsInUse)
				{
					return WordsInUse * BITS_PER_WORD;
				}
				word = ~Words[u];
			}
		}

		/// <summary>
		/// Returns the index of the nearest bit that is set to {@code true}
		/// that occurs on or before the specified starting index.
		/// If no such bit exists, or if {@code -1} is given as the
		/// starting index, then {@code -1} is returned.
		/// 
		/// <para>To iterate over the {@code true} bits in a {@code BitSet},
		/// use the following loop:
		/// 
		///  <pre> {@code
		/// for (int i = bs.length(); (i = bs.previousSetBit(i-1)) >= 0; ) {
		///     // operate on index i here
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the previous set bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is less
		///         than {@code -1}
		/// @since  1.7 </exception>
		public virtual int PreviousSetBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				if (fromIndex == -1)
				{
					return -1;
				}
				throw new IndexOutOfBoundsException("fromIndex < -1: " + fromIndex);
			}

			CheckInvariants();

			int u = WordIndex(fromIndex);
			if (u >= WordsInUse)
			{
				return Length() - 1;
			}

			long word = Words[u] & ((long)((ulong)WORD_MASK >> -(fromIndex + 1)));

			while (true)
			{
				if (word != 0)
				{
					return (u + 1) * BITS_PER_WORD - 1 - Long.NumberOfLeadingZeros(word);
				}
				if (u-- == 0)
				{
					return -1;
				}
				word = Words[u];
			}
		}

		/// <summary>
		/// Returns the index of the nearest bit that is set to {@code false}
		/// that occurs on or before the specified starting index.
		/// If no such bit exists, or if {@code -1} is given as the
		/// starting index, then {@code -1} is returned.
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the previous clear bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is less
		///         than {@code -1}
		/// @since  1.7 </exception>
		public virtual int PreviousClearBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				if (fromIndex == -1)
				{
					return -1;
				}
				throw new IndexOutOfBoundsException("fromIndex < -1: " + fromIndex);
			}

			CheckInvariants();

			int u = WordIndex(fromIndex);
			if (u >= WordsInUse)
			{
				return fromIndex;
			}

			long word = ~Words[u] & ((long)((ulong)WORD_MASK >> -(fromIndex + 1)));

			while (true)
			{
				if (word != 0)
				{
					return (u + 1) * BITS_PER_WORD - 1 - Long.NumberOfLeadingZeros(word);
				}
				if (u-- == 0)
				{
					return -1;
				}
				word = ~Words[u];
			}
		}

		/// <summary>
		/// Returns the "logical size" of this {@code BitSet}: the index of
		/// the highest set bit in the {@code BitSet} plus one. Returns zero
		/// if the {@code BitSet} contains no set bits.
		/// </summary>
		/// <returns> the logical size of this {@code BitSet}
		/// @since  1.2 </returns>
		public virtual int Length()
		{
			if (WordsInUse == 0)
			{
				return 0;
			}

			return BITS_PER_WORD * (WordsInUse - 1) + (BITS_PER_WORD - Long.NumberOfLeadingZeros(Words[WordsInUse - 1]));
		}

		/// <summary>
		/// Returns true if this {@code BitSet} contains no bits that are set
		/// to {@code true}.
		/// </summary>
		/// <returns> boolean indicating whether this {@code BitSet} is empty
		/// @since  1.4 </returns>
		public virtual bool Empty
		{
			get
			{
				return WordsInUse == 0;
			}
		}

		/// <summary>
		/// Returns true if the specified {@code BitSet} has any bits set to
		/// {@code true} that are also set to {@code true} in this {@code BitSet}.
		/// </summary>
		/// <param name="set"> {@code BitSet} to intersect with </param>
		/// <returns> boolean indicating whether this {@code BitSet} intersects
		///         the specified {@code BitSet}
		/// @since  1.4 </returns>
		public virtual bool Intersects(BitSet set)
		{
			for (int i = System.Math.Min(WordsInUse, set.WordsInUse) - 1; i >= 0; i--)
			{
				if ((Words[i] & set.Words[i]) != 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the number of bits set to {@code true} in this {@code BitSet}.
		/// </summary>
		/// <returns> the number of bits set to {@code true} in this {@code BitSet}
		/// @since  1.4 </returns>
		public virtual int Cardinality()
		{
			int sum = 0;
			for (int i = 0; i < WordsInUse; i++)
			{
				sum += Long.BitCount(Words[i]);
			}
			return sum;
		}

		/// <summary>
		/// Performs a logical <b>AND</b> of this target bit set with the
		/// argument bit set. This bit set is modified so that each bit in it
		/// has the value {@code true} if and only if it both initially
		/// had the value {@code true} and the corresponding bit in the
		/// bit set argument also had the value {@code true}.
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void And(BitSet set)
		{
			if (this == set)
			{
				return;
			}

			while (WordsInUse > set.WordsInUse)
			{
				Words[--WordsInUse] = 0;
			}

			// Perform logical AND on words in common
			for (int i = 0; i < WordsInUse; i++)
			{
				Words[i] &= set.Words[i];
			}

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Performs a logical <b>OR</b> of this bit set with the bit set
		/// argument. This bit set is modified so that a bit in it has the
		/// value {@code true} if and only if it either already had the
		/// value {@code true} or the corresponding bit in the bit set
		/// argument has the value {@code true}.
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void Or(BitSet set)
		{
			if (this == set)
			{
				return;
			}

			int wordsInCommon = System.Math.Min(WordsInUse, set.WordsInUse);

			if (WordsInUse < set.WordsInUse)
			{
				EnsureCapacity(set.WordsInUse);
				WordsInUse = set.WordsInUse;
			}

			// Perform logical OR on words in common
			for (int i = 0; i < wordsInCommon; i++)
			{
				Words[i] |= set.Words[i];
			}

			// Copy any remaining words
			if (wordsInCommon < set.WordsInUse)
			{
				System.Array.Copy(set.Words, wordsInCommon, Words, wordsInCommon, WordsInUse - wordsInCommon);
			}

			// recalculateWordsInUse() is unnecessary
			CheckInvariants();
		}

		/// <summary>
		/// Performs a logical <b>XOR</b> of this bit set with the bit set
		/// argument. This bit set is modified so that a bit in it has the
		/// value {@code true} if and only if one of the following
		/// statements holds:
		/// <ul>
		/// <li>The bit initially has the value {@code true}, and the
		///     corresponding bit in the argument has the value {@code false}.
		/// <li>The bit initially has the value {@code false}, and the
		///     corresponding bit in the argument has the value {@code true}.
		/// </ul>
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void Xor(BitSet set)
		{
			int wordsInCommon = System.Math.Min(WordsInUse, set.WordsInUse);

			if (WordsInUse < set.WordsInUse)
			{
				EnsureCapacity(set.WordsInUse);
				WordsInUse = set.WordsInUse;
			}

			// Perform logical XOR on words in common
			for (int i = 0; i < wordsInCommon; i++)
			{
				Words[i] ^= set.Words[i];
			}

			// Copy any remaining words
			if (wordsInCommon < set.WordsInUse)
			{
				System.Array.Copy(set.Words, wordsInCommon, Words, wordsInCommon, set.WordsInUse - wordsInCommon);
			}

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Clears all of the bits in this {@code BitSet} whose corresponding
		/// bit is set in the specified {@code BitSet}.
		/// </summary>
		/// <param name="set"> the {@code BitSet} with which to mask this
		///         {@code BitSet}
		/// @since  1.2 </param>
		public virtual void AndNot(BitSet set)
		{
			// Perform logical (a & !b) on words in common
			for (int i = System.Math.Min(WordsInUse, set.WordsInUse) - 1; i >= 0; i--)
			{
				Words[i] &= ~set.Words[i];
			}

			RecalculateWordsInUse();
			CheckInvariants();
		}

		/// <summary>
		/// Returns the hash code value for this bit set. The hash code depends
		/// only on which bits are set within this {@code BitSet}.
		/// 
		/// <para>The hash code is defined to be the result of the following
		/// calculation:
		///  <pre> {@code
		/// public int hashCode() {
		///     long h = 1234;
		///     long[] words = toLongArray();
		///     for (int i = words.length; --i >= 0; )
		///         h ^= words[i] * (i + 1);
		///     return (int)((h >> 32) ^ h);
		/// }}</pre>
		/// Note that the hash code changes if the set of bits is altered.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this bit set </returns>
		public override int HashCode()
		{
			long h = 1234;
			for (int i = WordsInUse; --i >= 0;)
			{
				h ^= Words[i] * (i + 1);
			}

			return (int)((h >> 32) ^ h);
		}

		/// <summary>
		/// Returns the number of bits of space actually in use by this
		/// {@code BitSet} to represent bit values.
		/// The maximum element in the set is the size - 1st element.
		/// </summary>
		/// <returns> the number of bits currently in this bit set </returns>
		public virtual int Size()
		{
			return Words.Length * BITS_PER_WORD;
		}

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and is a {@code Bitset} object that has
		/// exactly the same set of bits set to {@code true} as this bit
		/// set. That is, for every nonnegative {@code int} index {@code k},
		/// <pre>((BitSet)obj).get(k) == this.get(k)</pre>
		/// must be true. The current sizes of the two bit sets are not compared.
		/// </summary>
		/// <param name="obj"> the object to compare with </param>
		/// <returns> {@code true} if the objects are the same;
		///         {@code false} otherwise </returns>
		/// <seealso cref=    #size() </seealso>
		public override bool Equals(Object obj)
		{
			if (!(obj is BitSet))
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}

			BitSet set = (BitSet) obj;

			CheckInvariants();
			set.CheckInvariants();

			if (WordsInUse != set.WordsInUse)
			{
				return false;
			}

			// Check words in use by both BitSets
			for (int i = 0; i < WordsInUse; i++)
			{
				if (Words[i] != set.Words[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Cloning this {@code BitSet} produces a new {@code BitSet}
		/// that is equal to it.
		/// The clone of the bit set is another bit set that has exactly the
		/// same bits set to {@code true} as this bit set.
		/// </summary>
		/// <returns> a clone of this bit set </returns>
		/// <seealso cref=    #size() </seealso>
		public virtual Object Clone()
		{
			if (!SizeIsSticky)
			{
				TrimToSize();
			}

			try
			{
				BitSet result = (BitSet) base.Clone();
				result.Words = Words.clone();
				result.CheckInvariants();
				return result;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Attempts to reduce internal storage used for the bits in this bit set.
		/// Calling this method may, but is not required to, affect the value
		/// returned by a subsequent call to the <seealso cref="#size()"/> method.
		/// </summary>
		private void TrimToSize()
		{
			if (WordsInUse != Words.Length)
			{
				Words = Arrays.CopyOf(Words, WordsInUse);
				CheckInvariants();
			}
		}

		/// <summary>
		/// Save the state of the {@code BitSet} instance to a stream (i.e.,
		/// serialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(ObjectOutputStream s) throws IOException
		private void WriteObject(ObjectOutputStream s)
		{

			CheckInvariants();

			if (!SizeIsSticky)
			{
				TrimToSize();
			}

			ObjectOutputStream.PutField fields = s.PutFields();
			fields.Put("bits", Words);
			s.WriteFields();
		}

		/// <summary>
		/// Reconstitute the {@code BitSet} instance from a stream (i.e.,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(ObjectInputStream s) throws IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream s)
		{

			ObjectInputStream.GetField fields = s.ReadFields();
			Words = (long[]) fields.Get("bits", null);

			// Assume maximum length then find real length
			// because recalculateWordsInUse assumes maintenance
			// or reduction in logical size
			WordsInUse = Words.Length;
			RecalculateWordsInUse();
			SizeIsSticky = (Words.Length > 0 && Words[Words.Length - 1] == 0L); // heuristic
			CheckInvariants();
		}

		/// <summary>
		/// Returns a string representation of this bit set. For every index
		/// for which this {@code BitSet} contains a bit in the set
		/// state, the decimal representation of that index is included in
		/// the result. Such indices are listed in order from lowest to
		/// highest, separated by ",&nbsp;" (a comma and a space) and
		/// surrounded by braces, resulting in the usual mathematical
		/// notation for a set of integers.
		/// 
		/// <para>Example:
		/// <pre>
		/// BitSet drPepper = new BitSet();</pre>
		/// Now {@code drPepper.toString()} returns "{@code {}}".
		/// <pre>
		/// drPepper.set(2);</pre>
		/// Now {@code drPepper.toString()} returns "{@code {2}}".
		/// <pre>
		/// drPepper.set(4);
		/// drPepper.set(10);</pre>
		/// Now {@code drPepper.toString()} returns "{@code {2, 4, 10}}".
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this bit set </returns>
		public override String ToString()
		{
			CheckInvariants();

			int numBits = (WordsInUse > 128) ? Cardinality() : WordsInUse * BITS_PER_WORD;
			StringBuilder b = new StringBuilder(6 * numBits + 2);
			b.Append('{');

			int i = NextSetBit(0);
			if (i != -1)
			{
				b.Append(i);
				while (true)
				{
					if (++i < 0)
					{
						break;
					}
					if ((i = NextSetBit(i)) < 0)
					{
						break;
					}
					int endOfRun = NextClearBit(i);
					do
					{
						b.Append(", ").Append(i);
					} while (++i != endOfRun);
				}
			}

			b.Append('}');
			return b.ToString();
		}

		/// <summary>
		/// Returns a stream of indices for which this {@code BitSet}
		/// contains a bit in the set state. The indices are returned
		/// in order, from lowest to highest. The size of the stream
		/// is the number of bits in the set state, equal to the value
		/// returned by the <seealso cref="#cardinality()"/> method.
		/// 
		/// <para>The bit set must remain constant during the execution of the
		/// terminal stream operation.  Otherwise, the result of the terminal
		/// stream operation is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a stream of integers representing set indices
		/// @since 1.8 </returns>
		public virtual IntStream Stream()
		{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class BitSetIterator implements PrimitiveIterator_OfInt
	//		{
	//			int next = nextSetBit(0);
	//
	//			@@Override public boolean hasNext()
	//			{
	//				return next != -1;
	//			}
	//
	//			@@Override public int nextInt()
	//			{
	//				if (next != -1)
	//				{
	//					int ret = next;
	//					next = nextSetBit(next+1);
	//					return ret;
	//				}
	//				else
	//				{
	//					throw new NoSuchElementException();
	//				}
	//			}
	//		}

//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return StreamSupport.IntStream(() => Spliterators.Spliterator(new BitSetIterator(), Cardinality(), Spliterator_Fields.ORDERED | Spliterator_Fields.DISTINCT | Spliterator_Fields.SORTED), Spliterator_Fields.SIZED | Spliterator_Fields.SUBSIZED | Spliterator_Fields.ORDERED | Spliterator_Fields.DISTINCT | Spliterator_Fields.SORTED, false);
		}
	}

}