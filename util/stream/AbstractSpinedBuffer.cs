using System;

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
	/// Base class for a data structure for gathering elements into a buffer and then
	/// iterating them. Maintains an array of increasingly sized arrays, so there is
	/// no copying cost associated with growing the data structure.
	/// @since 1.8
	/// </summary>
	internal abstract class AbstractSpinedBuffer
	{
		/// <summary>
		/// Minimum power-of-two for the first chunk.
		/// </summary>
		public const int MIN_CHUNK_POWER = 4;

		/// <summary>
		/// Minimum size for the first chunk.
		/// </summary>
		public static readonly int MIN_CHUNK_SIZE = 1 << MIN_CHUNK_POWER;

		/// <summary>
		/// Max power-of-two for chunks.
		/// </summary>
		public const int MAX_CHUNK_POWER = 30;

		/// <summary>
		/// Minimum array size for array-of-chunks.
		/// </summary>
		public const int MIN_SPINE_SIZE = 8;


		/// <summary>
		/// log2 of the size of the first chunk.
		/// </summary>
		protected internal readonly int InitialChunkPower;

		/// <summary>
		/// Index of the *next* element to write; may point into, or just outside of,
		/// the current chunk.
		/// </summary>
		protected internal int ElementIndex;

		/// <summary>
		/// Index of the *current* chunk in the spine array, if the spine array is
		/// non-null.
		/// </summary>
		protected internal int SpineIndex;

		/// <summary>
		/// Count of elements in all prior chunks.
		/// </summary>
		protected internal long[] PriorElementCount;

		/// <summary>
		/// Construct with an initial capacity of 16.
		/// </summary>
		protected internal AbstractSpinedBuffer()
		{
			this.InitialChunkPower = MIN_CHUNK_POWER;
		}

		/// <summary>
		/// Construct with a specified initial capacity.
		/// </summary>
		/// <param name="initialCapacity"> The minimum expected number of elements </param>
		protected internal AbstractSpinedBuffer(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException("Illegal Capacity: " + initialCapacity);
			}

			this.InitialChunkPower = System.Math.Max(MIN_CHUNK_POWER, sizeof(int) - Integer.NumberOfLeadingZeros(initialCapacity - 1));
		}

		/// <summary>
		/// Is the buffer currently empty?
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return (SpineIndex == 0) && (ElementIndex == 0);
			}
		}

		/// <summary>
		/// How many elements are currently in the buffer?
		/// </summary>
		public virtual long Count()
		{
			return (SpineIndex == 0) ? ElementIndex : PriorElementCount[SpineIndex] + ElementIndex;
		}

		/// <summary>
		/// How big should the nth chunk be?
		/// </summary>
		protected internal virtual int ChunkSize(int n)
		{
			int power = (n == 0 || n == 1) ? InitialChunkPower : System.Math.Min(InitialChunkPower + n - 1, AbstractSpinedBuffer.MAX_CHUNK_POWER);
			return 1 << power;
		}

		/// <summary>
		/// Remove all data from the buffer
		/// </summary>
		public abstract void Clear();
	}

}