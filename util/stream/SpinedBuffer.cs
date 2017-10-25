using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.util.stream
{


	/// <summary>
	/// An ordered collection of elements.  Elements can be added, but not removed.
	/// Goes through a building phase, during which elements can be added, and a
	/// traversal phase, during which elements can be traversed in order but no
	/// further modifications are possible.
	/// 
	/// <para> One or more arrays are used to store elements. The use of a multiple
	/// arrays has better performance characteristics than a single array used by
	/// <seealso cref="ArrayList"/>, as when the capacity of the list needs to be increased
	/// no copying of elements is required.  This is usually beneficial in the case
	/// where the results will be traversed a small number of times.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements in this list
	/// @since 1.8 </param>
	internal class SpinedBuffer<E> : AbstractSpinedBuffer, Consumer<E>, Iterable<E>
	{

		/*
		 * We optimistically hope that all the data will fit into the first chunk,
		 * so we try to avoid inflating the spine[] and priorElementCount[] arrays
		 * prematurely.  So methods must be prepared to deal with these arrays being
		 * null.  If spine is non-null, then spineIndex points to the current chunk
		 * within the spine, otherwise it is zero.  The spine and priorElementCount
		 * arrays are always the same size, and for any i <= spineIndex,
		 * priorElementCount[i] is the sum of the sizes of all the prior chunks.
		 *
		 * The curChunk pointer is always valid.  The elementIndex is the index of
		 * the next element to be written in curChunk; this may be past the end of
		 * curChunk so we have to check before writing. When we inflate the spine
		 * array, curChunk becomes the first element in it.  When we clear the
		 * buffer, we discard all chunks except the first one, which we clear,
		 * restoring it to the initial single-chunk state.
		 */

		/// <summary>
		/// Chunk that we're currently writing into; may or may not be aliased with
		/// the first element of the spine.
		/// </summary>
		protected internal E[] CurChunk;

		/// <summary>
		/// All chunks, or null if there is only one chunk.
		/// </summary>
		protected internal E[][] Spine;

		/// <summary>
		/// Constructs an empty list with the specified initial capacity.
		/// </summary>
		/// <param name="initialCapacity">  the initial capacity of the list </param>
		/// <exception cref="IllegalArgumentException"> if the specified initial capacity
		///         is negative </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") SpinedBuffer(int initialCapacity)
		internal SpinedBuffer(int initialCapacity) : base(initialCapacity)
		{
			CurChunk = (E[]) new Object[1 << InitialChunkPower];
		}

		/// <summary>
		/// Constructs an empty list with an initial capacity of sixteen.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") SpinedBuffer()
		internal SpinedBuffer() : base()
		{
			CurChunk = (E[]) new Object[1 << InitialChunkPower];
		}

		/// <summary>
		/// Returns the current capacity of the buffer
		/// </summary>
		protected internal virtual long Capacity()
		{
			return (SpineIndex == 0) ? CurChunk.Length : PriorElementCount[SpineIndex] + Spine[SpineIndex].Length;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void inflateSpine()
		private void InflateSpine()
		{
			if (Spine == null)
			{
				Spine = (E[][]) new Object[MIN_SPINE_SIZE][];
				PriorElementCount = new long[MIN_SPINE_SIZE];
				Spine[0] = CurChunk;
			}
		}

		/// <summary>
		/// Ensure that the buffer has at least capacity to hold the target size
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected final void ensureCapacity(long targetSize)
		protected internal void EnsureCapacity(long targetSize)
		{
			long capacity = Capacity();
			if (targetSize > capacity)
			{
				InflateSpine();
				for (int i = SpineIndex + 1; targetSize > capacity; i++)
				{
					if (i >= Spine.Length)
					{
						int newSpineSize = Spine.Length * 2;
						Spine = Arrays.CopyOf(Spine, newSpineSize);
						PriorElementCount = Arrays.CopyOf(PriorElementCount, newSpineSize);
					}
					int nextChunkSize = ChunkSize(i);
					Spine[i] = (E[]) new Object[nextChunkSize];
					PriorElementCount[i] = PriorElementCount[i - 1] + Spine[i - 1].Length;
					capacity += nextChunkSize;
				}
			}
		}

		/// <summary>
		/// Force the buffer to increase its capacity.
		/// </summary>
		protected internal virtual void IncreaseCapacity()
		{
			EnsureCapacity(Capacity() + 1);
		}

		/// <summary>
		/// Retrieve the element at the specified index.
		/// </summary>
		public virtual E Get(long index)
		{
			// @@@ can further optimize by caching last seen spineIndex,
			// which is going to be right most of the time

			// Casts to int are safe since the spine array index is the index minus
			// the prior element count from the current spine
			if (SpineIndex == 0)
			{
				if (index < ElementIndex)
				{
					return CurChunk[((int) index)];
				}
				else
				{
					throw new IndexOutOfBoundsException(Convert.ToString(index));
				}
			}

			if (index >= Count())
			{
				throw new IndexOutOfBoundsException(Convert.ToString(index));
			}

			for (int j = 0; j <= SpineIndex; j++)
			{
				if (index < PriorElementCount[j] + Spine[j].Length)
				{
					return Spine[j][((int)(index - PriorElementCount[j]))];
				}
			}

			throw new IndexOutOfBoundsException(Convert.ToString(index));
		}

		/// <summary>
		/// Copy the elements, starting at the specified offset, into the specified
		/// array.
		/// </summary>
		public virtual void CopyInto(E[] array, int offset)
		{
			long finalOffset = offset + Count();
			if (finalOffset > array.Length || finalOffset < offset)
			{
				throw new IndexOutOfBoundsException("does not fit");
			}

			if (SpineIndex == 0)
			{
				System.Array.Copy(CurChunk, 0, array, offset, ElementIndex);
			}
			else
			{
				// full chunks
				for (int i = 0; i < SpineIndex; i++)
				{
					System.Array.Copy(Spine[i], 0, array, offset, Spine[i].Length);
					offset += Spine[i].Length;
				}
				if (ElementIndex > 0)
				{
					System.Array.Copy(CurChunk, 0, array, offset, ElementIndex);
				}
			}
		}

		/// <summary>
		/// Create a new array using the specified array factory, and copy the
		/// elements into it.
		/// </summary>
		public virtual E[] AsArray(IntFunction<E[]> arrayFactory)
		{
			long size = Count();
			if (size >= Nodes.MAX_ARRAY_SIZE)
			{
				throw new IllegalArgumentException(Nodes.BAD_SIZE);
			}
			E[] result = arrayFactory.Apply((int) size);
			CopyInto(result, 0);
			return result;
		}

		public override void Clear()
		{
			if (Spine != null)
			{
				CurChunk = Spine[0];
				for (int i = 0; i < CurChunk.Length; i++)
				{
					CurChunk[i] = null;
				}
				Spine = null;
				PriorElementCount = null;
			}
			else
			{
				for (int i = 0; i < ElementIndex; i++)
				{
					CurChunk[i] = null;
				}
			}
			ElementIndex = 0;
			SpineIndex = 0;
		}

		public virtual IEnumerator<E> GetEnumerator()
		{
			return Spliterators.Iterator(Spliterator());
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base E> consumer)
		public override void forEach<T1>(Consumer<T1> consumer)
		{
			// completed chunks, if any
			for (int j = 0; j < SpineIndex; j++)
			{
				foreach (E t in Spine[j])
				{
					consumer.Accept(t);
				}
			}

			// current chunk
			for (int i = 0; i < ElementIndex; i++)
			{
				consumer.Accept(CurChunk[i]);
			}
		}

		public override void Accept(E e)
		{
			if (ElementIndex == CurChunk.Length)
			{
				InflateSpine();
				if (SpineIndex + 1 >= Spine.Length || Spine[SpineIndex + 1] == null)
				{
					IncreaseCapacity();
				}
				ElementIndex = 0;
				++SpineIndex;
				CurChunk = Spine[SpineIndex];
			}
			CurChunk[ElementIndex++] = e;
		}

		public override String ToString()
		{
			IList<E> list = new List<E>();
			ForEach(list::add);
			return "SpinedBuffer:" + list.ToString();
		}

		private static readonly int SPLITERATOR_CHARACTERISTICS = java.util.Spliterator_Fields.SIZED | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.SUBSIZED;

		/// <summary>
		/// Return a <seealso cref="Spliterator"/> describing the contents of the buffer.
		/// </summary>
		public virtual Spliterator<E> Spliterator()
		{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class Splitr implements java.util.Spliterator<E>
	//		{
	//			// The current spine index
	//			int splSpineIndex;
	//
	//			// Last spine index
	//			final int lastSpineIndex;
	//
	//			// The current element index into the current spine
	//			int splElementIndex;
	//
	//			// Last spine's last element index + 1
	//			final int lastSpineElementFence;
	//
	//			// When splSpineIndex >= lastSpineIndex and
	//			// splElementIndex >= lastSpineElementFence then
	//			// this spliterator is fully traversed
	//			// tryAdvance can set splSpineIndex > spineIndex if the last spine is full
	//
	//			// The current spine array
	//			E[] splChunk;
	//
	//			Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//			{
	//				this.splSpineIndex = firstSpineIndex;
	//				this.lastSpineIndex = lastSpineIndex;
	//				this.splElementIndex = firstSpineElementIndex;
	//				this.lastSpineElementFence = lastSpineElementFence;
	//				assert spine != null || firstSpineIndex == 0 && lastSpineIndex == 0;
	//				splChunk = (spine == null) ? curChunk : spine[firstSpineIndex];
	//			}
	//
	//			@@Override public long estimateSize()
	//			{
	//				return (splSpineIndex == lastSpineIndex) ? (long) lastSpineElementFence - splElementIndex : priorElementCount[lastSpineIndex] + lastSpineElementFence - priorElementCount[splSpineIndex] - splElementIndex; // # of elements prior to end -
	//					   // # of elements prior to current
	//			}
	//
	//			@@Override public int characteristics()
	//			{
	//				return SPLITERATOR_CHARACTERISTICS;
	//			}
	//
	//			@@Override public boolean tryAdvance(Consumer<? base E> consumer)
	//			{
	//				Objects.requireNonNull(consumer);
	//
	//				if (splSpineIndex < lastSpineIndex || (splSpineIndex == lastSpineIndex && splElementIndex < lastSpineElementFence))
	//			{
	//					consumer.accept(splChunk[splElementIndex++]);
	//
	//					if (splElementIndex == splChunk.length)
	//					{
	//						splElementIndex = 0;
	//						++splSpineIndex;
	//						if (spine != null && splSpineIndex <= lastSpineIndex)
	//							splChunk = spine[splSpineIndex];
	//					}
	//					return true;
	//				}
	//				return false;
	//			}
	//
	//			@@Override public void forEachRemaining(Consumer<? base E> consumer)
	//			{
	//				Objects.requireNonNull(consumer);
	//
	//				if (splSpineIndex < lastSpineIndex || (splSpineIndex == lastSpineIndex && splElementIndex < lastSpineElementFence))
	//			{
	//					int i = splElementIndex;
	//					// completed chunks, if any
	//					for (int sp = splSpineIndex; sp < lastSpineIndex; sp++)
	//					{
	//						E[] chunk = spine[sp];
	//						for (; i < chunk.length; i++)
	//						{
	//							consumer.accept(chunk[i]);
	//						}
	//						i = 0;
	//					}
	//					// last (or current uncompleted) chunk
	//					E[] chunk = (splSpineIndex == lastSpineIndex) ? splChunk : spine[lastSpineIndex];
	//					int hElementIndex = lastSpineElementFence;
	//					for (; i < hElementIndex; i++)
	//					{
	//						consumer.accept(chunk[i]);
	//					}
	//					// mark consumed
	//					splSpineIndex = lastSpineIndex;
	//					splElementIndex = lastSpineElementFence;
	//				}
	//			}
	//
	//			@@Override public Spliterator<E> trySplit()
	//			{
	//				if (splSpineIndex < lastSpineIndex)
	//				{
	//					// split just before last chunk (if it is full this means 50:50 split)
	//					Spliterator<E> ret = new Splitr(splSpineIndex, lastSpineIndex - 1, splElementIndex, spine[lastSpineIndex-1].length);
	//					// position to start of last chunk
	//					splSpineIndex = lastSpineIndex;
	//					splElementIndex = 0;
	//					splChunk = spine[splSpineIndex];
	//					return ret;
	//				}
	//				else if (splSpineIndex == lastSpineIndex)
	//				{
	//					int t = (lastSpineElementFence - splElementIndex) / 2;
	//					if (t == 0)
	//						return null;
	//					else
	//					{
	//						Spliterator<E> ret = Arrays.spliterator(splChunk, splElementIndex, splElementIndex + t);
	//						splElementIndex += t;
	//						return ret;
	//					}
	//				}
	//				else
	//				{
	//					return null;
	//				}
	//			}
	//		}
			return new Splitr(0, SpineIndex, 0, ElementIndex);
		}

		/// <summary>
		/// An ordered collection of primitive values.  Elements can be added, but
		/// not removed. Goes through a building phase, during which elements can be
		/// added, and a traversal phase, during which elements can be traversed in
		/// order but no further modifications are possible.
		/// 
		/// <para> One or more arrays are used to store elements. The use of a multiple
		/// arrays has better performance characteristics than a single array used by
		/// <seealso cref="ArrayList"/>, as when the capacity of the list needs to be increased
		/// no copying of elements is required.  This is usually beneficial in the case
		/// where the results will be traversed a small number of times.
		/// 
		/// </para>
		/// </summary>
		/// @param <E> the wrapper type for this primitive type </param>
		/// @param <T_ARR> the array type for this primitive type </param>
		/// @param <T_CONS> the Consumer type for this primitive type </param>
		internal abstract class OfPrimitive<E, T_ARR, T_CONS> : AbstractSpinedBuffer, Iterable<E>
		{

			/*
			 * We optimistically hope that all the data will fit into the first chunk,
			 * so we try to avoid inflating the spine[] and priorElementCount[] arrays
			 * prematurely.  So methods must be prepared to deal with these arrays being
			 * null.  If spine is non-null, then spineIndex points to the current chunk
			 * within the spine, otherwise it is zero.  The spine and priorElementCount
			 * arrays are always the same size, and for any i <= spineIndex,
			 * priorElementCount[i] is the sum of the sizes of all the prior chunks.
			 *
			 * The curChunk pointer is always valid.  The elementIndex is the index of
			 * the next element to be written in curChunk; this may be past the end of
			 * curChunk so we have to check before writing. When we inflate the spine
			 * array, curChunk becomes the first element in it.  When we clear the
			 * buffer, we discard all chunks except the first one, which we clear,
			 * restoring it to the initial single-chunk state.
			 */

			// The chunk we're currently writing into
			internal T_ARR CurChunk;

			// All chunks, or null if there is only one chunk
			internal T_ARR[] Spine;

			/// <summary>
			/// Constructs an empty list with the specified initial capacity.
			/// </summary>
			/// <param name="initialCapacity">  the initial capacity of the list </param>
			/// <exception cref="IllegalArgumentException"> if the specified initial capacity
			///         is negative </exception>
			internal OfPrimitive(int initialCapacity) : base(initialCapacity)
			{
				CurChunk = NewArray(1 << InitialChunkPower);
			}

			/// <summary>
			/// Constructs an empty list with an initial capacity of sixteen.
			/// </summary>
			internal OfPrimitive() : base()
			{
				CurChunk = NewArray(1 << InitialChunkPower);
			}

			public override abstract IEnumerator<E> Iterator();

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public abstract void forEach(java.util.function.Consumer<? base E> consumer);
			public override abstract void forEach<T1>(Consumer<T1> consumer);

			/// <summary>
			/// Create a new array-of-array of the proper type and size </summary>
			protected internal abstract T_ARR[] NewArrayArray(int size);

			/// <summary>
			/// Create a new array of the proper type and size </summary>
			public abstract T_ARR NewArray(int size);

			/// <summary>
			/// Get the length of an array </summary>
			protected internal abstract int ArrayLength(T_ARR array);

			/// <summary>
			/// Iterate an array with the provided consumer </summary>
			protected internal abstract void ArrayForEach(T_ARR array, int from, int to, T_CONS consumer);

			protected internal virtual long Capacity()
			{
				return (SpineIndex == 0) ? ArrayLength(CurChunk) : PriorElementCount[SpineIndex] + ArrayLength(Spine[SpineIndex]);
			}

			internal virtual void InflateSpine()
			{
				if (Spine == null)
				{
					Spine = NewArrayArray(MIN_SPINE_SIZE);
					PriorElementCount = new long[MIN_SPINE_SIZE];
					Spine[0] = CurChunk;
				}
			}

			protected internal void EnsureCapacity(long targetSize)
			{
				long capacity = Capacity();
				if (targetSize > capacity)
				{
					InflateSpine();
					for (int i = SpineIndex + 1; targetSize > capacity; i++)
					{
						if (i >= Spine.Length)
						{
							int newSpineSize = Spine.Length * 2;
							Spine = Arrays.CopyOf(Spine, newSpineSize);
							PriorElementCount = Arrays.CopyOf(PriorElementCount, newSpineSize);
						}
						int nextChunkSize = ChunkSize(i);
						Spine[i] = NewArray(nextChunkSize);
						PriorElementCount[i] = PriorElementCount[i - 1] + ArrayLength(Spine[i - 1]);
						capacity += nextChunkSize;
					}
				}
			}

			protected internal virtual void IncreaseCapacity()
			{
				EnsureCapacity(Capacity() + 1);
			}

			protected internal virtual int ChunkFor(long index)
			{
				if (SpineIndex == 0)
				{
					if (index < ElementIndex)
					{
						return 0;
					}
					else
					{
						throw new IndexOutOfBoundsException(Convert.ToString(index));
					}
				}

				if (index >= Count())
				{
					throw new IndexOutOfBoundsException(Convert.ToString(index));
				}

				for (int j = 0; j <= SpineIndex; j++)
				{
					if (index < PriorElementCount[j] + ArrayLength(Spine[j]))
					{
						return j;
					}
				}

				throw new IndexOutOfBoundsException(Convert.ToString(index));
			}

			public virtual void CopyInto(T_ARR array, int offset)
			{
				long finalOffset = offset + Count();
				if (finalOffset > ArrayLength(array) || finalOffset < offset)
				{
					throw new IndexOutOfBoundsException("does not fit");
				}

				if (SpineIndex == 0)
				{
					System.Array.Copy(CurChunk, 0, array, offset, ElementIndex);
				}
				else
				{
					// full chunks
					for (int i = 0; i < SpineIndex; i++)
					{
						System.Array.Copy(Spine[i], 0, array, offset, ArrayLength(Spine[i]));
						offset += ArrayLength(Spine[i]);
					}
					if (ElementIndex > 0)
					{
						System.Array.Copy(CurChunk, 0, array, offset, ElementIndex);
					}
				}
			}

			public virtual T_ARR AsPrimitiveArray()
			{
				long size = Count();
				if (size >= Nodes.MAX_ARRAY_SIZE)
				{
					throw new IllegalArgumentException(Nodes.BAD_SIZE);
				}
				T_ARR result = NewArray((int) size);
				CopyInto(result, 0);
				return result;
			}

			protected internal virtual void PreAccept()
			{
				if (ElementIndex == ArrayLength(CurChunk))
				{
					InflateSpine();
					if (SpineIndex + 1 >= Spine.Length || Spine[SpineIndex + 1] == null)
					{
						IncreaseCapacity();
					}
					ElementIndex = 0;
					++SpineIndex;
					CurChunk = Spine[SpineIndex];
				}
			}

			public override void Clear()
			{
				if (Spine != null)
				{
					CurChunk = Spine[0];
					Spine = null;
					PriorElementCount = null;
				}
				ElementIndex = 0;
				SpineIndex = 0;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("overloads") public void forEach(T_CONS consumer)
			public virtual void ForEach(T_CONS consumer)
			{
				// completed chunks, if any
				for (int j = 0; j < SpineIndex; j++)
				{
					ArrayForEach(Spine[j], 0, ArrayLength(Spine[j]), consumer);
				}

				// current chunk
				ArrayForEach(CurChunk, 0, ElementIndex, consumer);
			}

			internal abstract class BaseSpliterator<T_SPLITR> : java.util.Spliterator_OfPrimitive<E, T_CONS, T_SPLITR> where T_SPLITR : java.util.Spliterator_OfPrimitive<E, T_CONS, T_SPLITR>
			{
				private readonly SpinedBuffer.OfPrimitive<E, T_ARR, T_CONS> OuterInstance;

				// The current spine index
				internal int SplSpineIndex;

				// Last spine index
				internal readonly int LastSpineIndex;

				// The current element index into the current spine
				internal int SplElementIndex;

				// Last spine's last element index + 1
				internal readonly int LastSpineElementFence;

				// When splSpineIndex >= lastSpineIndex and
				// splElementIndex >= lastSpineElementFence then
				// this spliterator is fully traversed
				// tryAdvance can set splSpineIndex > spineIndex if the last spine is full

				// The current spine array
				internal T_ARR SplChunk;

				internal BaseSpliterator(SpinedBuffer.OfPrimitive<E, T_ARR, T_CONS> outerInstance, int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
				{
					this.OuterInstance = outerInstance;
					this.SplSpineIndex = firstSpineIndex;
					this.LastSpineIndex = lastSpineIndex;
					this.SplElementIndex = firstSpineElementIndex;
					this.LastSpineElementFence = lastSpineElementFence;
					Debug.Assert(outerInstance.Spine != null || firstSpineIndex == 0 && lastSpineIndex == 0);
					SplChunk = (outerInstance.Spine == null) ? outerInstance.CurChunk : outerInstance.Spine[firstSpineIndex];
				}

				internal abstract T_SPLITR NewSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence);

				internal abstract void ArrayForOne(T_ARR array, int index, T_CONS consumer);

				internal abstract T_SPLITR ArraySpliterator(T_ARR array, int offset, int len);

				public override long EstimateSize()
				{
					return (SplSpineIndex == LastSpineIndex) ? (long) LastSpineElementFence - SplElementIndex : outerInstance.PriorElementCount[LastSpineIndex] + LastSpineElementFence - outerInstance.PriorElementCount[SplSpineIndex] - SplElementIndex; // # of elements prior to end -
						   // # of elements prior to current
				}

				public override int Characteristics()
				{
					return SPLITERATOR_CHARACTERISTICS;
				}

				public override bool TryAdvance(T_CONS consumer)
				{
					Objects.RequireNonNull(consumer);

					if (SplSpineIndex < LastSpineIndex || (SplSpineIndex == LastSpineIndex && SplElementIndex < LastSpineElementFence))
					{
						ArrayForOne(SplChunk, SplElementIndex++, consumer);

						if (SplElementIndex == outerInstance.ArrayLength(SplChunk))
						{
							SplElementIndex = 0;
							++SplSpineIndex;
							if (outerInstance.Spine != null && SplSpineIndex <= LastSpineIndex)
							{
								SplChunk = outerInstance.Spine[SplSpineIndex];
							}
						}
						return true;
					}
					return false;
				}

				public override void ForEachRemaining(T_CONS consumer)
				{
					Objects.RequireNonNull(consumer);

					if (SplSpineIndex < LastSpineIndex || (SplSpineIndex == LastSpineIndex && SplElementIndex < LastSpineElementFence))
					{
						int i = SplElementIndex;
						// completed chunks, if any
						for (int sp = SplSpineIndex; sp < LastSpineIndex; sp++)
						{
							T_ARR chunk = outerInstance.Spine[sp];
							outerInstance.ArrayForEach(chunk, i, outerInstance.ArrayLength(chunk), consumer);
							i = 0;
						}
						// last (or current uncompleted) chunk
						T_ARR chunk = (SplSpineIndex == LastSpineIndex) ? SplChunk : outerInstance.Spine[LastSpineIndex];
						outerInstance.ArrayForEach(chunk, i, LastSpineElementFence, consumer);
						// mark consumed
						SplSpineIndex = LastSpineIndex;
						SplElementIndex = LastSpineElementFence;
					}
				}

				public override T_SPLITR TrySplit()
				{
					if (SplSpineIndex < LastSpineIndex)
					{
						// split just before last chunk (if it is full this means 50:50 split)
						T_SPLITR ret = NewSpliterator(SplSpineIndex, LastSpineIndex - 1, SplElementIndex, outerInstance.ArrayLength(outerInstance.Spine[LastSpineIndex - 1]));
						// position us to start of last chunk
						SplSpineIndex = LastSpineIndex;
						SplElementIndex = 0;
						SplChunk = outerInstance.Spine[SplSpineIndex];
						return ret;
					}
					else if (SplSpineIndex == LastSpineIndex)
					{
						int t = (LastSpineElementFence - SplElementIndex) / 2;
						if (t == 0)
						{
							return null;
						}
						else
						{
							T_SPLITR ret = ArraySpliterator(SplChunk, SplElementIndex, t);
							SplElementIndex += t;
							return ret;
						}
					}
					else
					{
						return null;
					}
				}
			}
		}

		/// <summary>
		/// An ordered collection of {@code int} values.
		/// </summary>
		internal class OfInt : SpinedBuffer.OfPrimitive<Integer, int[], IntConsumer>, IntConsumer
		{
			internal OfInt()
			{
			}

			internal OfInt(int initialCapacity) : base(initialCapacity)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base Integer> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				if (consumer is IntConsumer)
				{
					ForEach((IntConsumer) consumer);
				}
				else
				{
					if (Tripwire.ENABLED)
					{
						Tripwire.Trip(this.GetType(), "{0} calling SpinedBuffer.OfInt.forEach(Consumer)");
					}
					Spliterator().forEachRemaining(consumer);
				}
			}

			protected internal override int[][] NewArrayArray(int size)
			{
				return new int[size][];
			}

			public override int[] NewArray(int size)
			{
				return new int[size];
			}

			protected internal override int ArrayLength(int[] array)
			{
				return array.Length;
			}

			protected internal override void ArrayForEach(int[] array, int from, int to, IntConsumer consumer)
			{
				for (int i = from; i < to; i++)
				{
					consumer.Accept(array[i]);
				}
			}

			public virtual void Accept(int i)
			{
				PreAccept();
				CurChunk[ElementIndex++] = i;
			}

			public virtual int Get(long index)
			{
				// Casts to int are safe since the spine array index is the index minus
				// the prior element count from the current spine
				int ch = ChunkFor(index);
				if (SpineIndex == 0 && ch == 0)
				{
					return CurChunk[(int) index];
				}
				else
				{
					return Spine[ch][(int)(index - PriorElementCount[ch])];
				}
			}

			public override java.util.PrimitiveIterator_OfInt Iterator()
			{
				return Spliterators.Iterator(Spliterator());
			}

			public virtual java.util.Spliterator_OfInt Spliterator()
			{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//				class Splitr extends BaseSpliterator<java.util.Spliterator_OfInt> implements java.util.Spliterator_OfInt
	//			{
	//				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					base(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					return new Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override void arrayForOne(int[] array, int index, IntConsumer consumer)
	//				{
	//					consumer.accept(array[index]);
	//				}
	//
	//				@@Override Spliterator.OfInt arraySpliterator(int[] array, int offset, int len)
	//				{
	//					return Arrays.spliterator(array, offset, offset+len);
	//				}
	//			}
				return new Splitr(0, SpineIndex, 0, ElementIndex);
			}

			public override String ToString()
			{
				int[] array = AsPrimitiveArray();
				if (array.Length < 200)
				{
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array));
				}
				else
				{
					int[] array2 = Arrays.CopyOf(array, 200);
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}...", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array2));
				}
			}
		}

		/// <summary>
		/// An ordered collection of {@code long} values.
		/// </summary>
		internal class OfLong : SpinedBuffer.OfPrimitive<Long, long[], LongConsumer>, LongConsumer
		{
			internal OfLong()
			{
			}

			internal OfLong(int initialCapacity) : base(initialCapacity)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base Long> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				if (consumer is LongConsumer)
				{
					ForEach((LongConsumer) consumer);
				}
				else
				{
					if (Tripwire.ENABLED)
					{
						Tripwire.Trip(this.GetType(), "{0} calling SpinedBuffer.OfLong.forEach(Consumer)");
					}
					Spliterator().forEachRemaining(consumer);
				}
			}

			protected internal override long[][] NewArrayArray(int size)
			{
				return new long[size][];
			}

			public override long[] NewArray(int size)
			{
				return new long[size];
			}

			protected internal override int ArrayLength(long[] array)
			{
				return array.Length;
			}

			protected internal override void ArrayForEach(long[] array, int from, int to, LongConsumer consumer)
			{
				for (int i = from; i < to; i++)
				{
					consumer.Accept(array[i]);
				}
			}

			public virtual void Accept(long i)
			{
				PreAccept();
				CurChunk[ElementIndex++] = i;
			}

			public virtual long Get(long index)
			{
				// Casts to int are safe since the spine array index is the index minus
				// the prior element count from the current spine
				int ch = ChunkFor(index);
				if (SpineIndex == 0 && ch == 0)
				{
					return CurChunk[(int) index];
				}
				else
				{
					return Spine[ch][(int)(index - PriorElementCount[ch])];
				}
			}

			public override java.util.PrimitiveIterator_OfLong Iterator()
			{
				return Spliterators.Iterator(Spliterator());
			}


			public virtual java.util.Spliterator_OfLong Spliterator()
			{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//				class Splitr extends BaseSpliterator<java.util.Spliterator_OfLong> implements java.util.Spliterator_OfLong
	//			{
	//				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					base(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					return new Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override void arrayForOne(long[] array, int index, LongConsumer consumer)
	//				{
	//					consumer.accept(array[index]);
	//				}
	//
	//				@@Override Spliterator.OfLong arraySpliterator(long[] array, int offset, int len)
	//				{
	//					return Arrays.spliterator(array, offset, offset+len);
	//				}
	//			}
				return new Splitr(0, SpineIndex, 0, ElementIndex);
			}

			public override String ToString()
			{
				long[] array = AsPrimitiveArray();
				if (array.Length < 200)
				{
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array));
				}
				else
				{
					long[] array2 = Arrays.CopyOf(array, 200);
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}...", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array2));
				}
			}
		}

		/// <summary>
		/// An ordered collection of {@code double} values.
		/// </summary>
		internal class OfDouble : SpinedBuffer.OfPrimitive<Double, double[], DoubleConsumer>, DoubleConsumer
		{
			internal OfDouble()
			{
			}

			internal OfDouble(int initialCapacity) : base(initialCapacity)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.Consumer<? base Double> consumer)
			public override void forEach<T1>(Consumer<T1> consumer)
			{
				if (consumer is DoubleConsumer)
				{
					ForEach((DoubleConsumer) consumer);
				}
				else
				{
					if (Tripwire.ENABLED)
					{
						Tripwire.Trip(this.GetType(), "{0} calling SpinedBuffer.OfDouble.forEach(Consumer)");
					}
					Spliterator().forEachRemaining(consumer);
				}
			}

			protected internal override double[][] NewArrayArray(int size)
			{
				return new double[size][];
			}

			public override double[] NewArray(int size)
			{
				return new double[size];
			}

			protected internal override int ArrayLength(double[] array)
			{
				return array.Length;
			}

			protected internal override void ArrayForEach(double[] array, int from, int to, DoubleConsumer consumer)
			{
				for (int i = from; i < to; i++)
				{
					consumer.Accept(array[i]);
				}
			}

			public virtual void Accept(double i)
			{
				PreAccept();
				CurChunk[ElementIndex++] = i;
			}

			public virtual double Get(long index)
			{
				// Casts to int are safe since the spine array index is the index minus
				// the prior element count from the current spine
				int ch = ChunkFor(index);
				if (SpineIndex == 0 && ch == 0)
				{
					return CurChunk[(int) index];
				}
				else
				{
					return Spine[ch][(int)(index - PriorElementCount[ch])];
				}
			}

			public override java.util.PrimitiveIterator_OfDouble Iterator()
			{
				return Spliterators.Iterator(Spliterator());
			}

			public virtual java.util.Spliterator_OfDouble Spliterator()
			{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//				class Splitr extends BaseSpliterator<java.util.Spliterator_OfDouble> implements java.util.Spliterator_OfDouble
	//			{
	//				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					base(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	//				{
	//					return new Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	//				}
	//
	//				@@Override void arrayForOne(double[] array, int index, DoubleConsumer consumer)
	//				{
	//					consumer.accept(array[index]);
	//				}
	//
	//				@@Override Spliterator.OfDouble arraySpliterator(double[] array, int offset, int len)
	//				{
	//					return Arrays.spliterator(array, offset, offset+len);
	//				}
	//			}
				return new Splitr(0, SpineIndex, 0, ElementIndex);
			}

			public override String ToString()
			{
				double[] array = AsPrimitiveArray();
				if (array.Length < 200)
				{
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array));
				}
				else
				{
					double[] array2 = Arrays.CopyOf(array, 200);
					return string.Format("{0}[length={1:D}, chunks={2:D}]{3}...", this.GetType().Name, array.Length, SpineIndex, Arrays.ToString(array2));
				}
			}
		}
	}


}