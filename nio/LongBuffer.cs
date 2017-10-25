using System;

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










	/// <summary>
	/// A long buffer.
	/// 
	/// <para> This class defines four categories of operations upon
	/// long buffers:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> Absolute and relative <seealso cref="#get() <i>get</i>"/> and
	///   <seealso cref="#put(long) <i>put</i>"/> methods that read and write
	///   single longs; </para></li>
	/// 
	///   <li><para> Relative <seealso cref="#get(long[]) <i>bulk get</i>"/>
	///   methods that transfer contiguous sequences of longs from this buffer
	///   into an array; and</para></li>
	/// 
	///   <li><para> Relative <seealso cref="#put(long[]) <i>bulk put</i>"/>
	///   methods that transfer contiguous sequences of longs from a
	///   long array or some other long
	///   buffer into this buffer;&#32;and </para></li>
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	///   <li><para> Methods for <seealso cref="#compact compacting"/>, {@link
	///   #duplicate duplicating}, and <seealso cref="#slice slicing"/>
	///   a long buffer.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Long buffers can be created either by {@link #allocate
	/// <i>allocation</i>}, which allocates space for the buffer's
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// content, by <seealso cref="#wrap(long[]) <i>wrapping</i>"/> an existing
	/// long array  into a buffer, or by creating a
	/// <a href="ByteBuffer.html#views"><i>view</i></a> of an existing byte buffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </para>
	/// <para> Like a byte buffer, a long buffer is either <a
	/// href="ByteBuffer.html#direct"><i>direct</i> or <i>non-direct</i></a>.  A
	/// long buffer created via the <tt>wrap</tt> methods of this class will
	/// be non-direct.  A long buffer created as a view of a byte buffer will
	/// be direct if, and only if, the byte buffer itself is direct.  Whether or not
	/// a long buffer is direct may be determined by invoking the {@link
	/// #isDirect isDirect} method.  </para>
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// <para> Methods in this class that do not otherwise have a value to return are
	/// specified to return the buffer upon which they are invoked.  This allows
	/// method invocations to be chained.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </para>
	/// </summary>

	public abstract class LongBuffer : Buffer, Comparable<LongBuffer>
	{

		// These fields are declared here rather than in Heap-X-Buffer in order to
		// reduce the number of virtual method invocations needed to access these
		// values, which is especially costly when coding small buffers.
		//
		internal readonly long[] Hb; // Non-null only for heap buffers
		internal readonly int Offset;
		internal bool IsReadOnly; // Valid only for heap buffers

		// Creates a new buffer with the given mark, position, limit, capacity,
		// backing array, and array offset
		//
		internal LongBuffer(int mark, int pos, int lim, int cap, long[] hb, int offset) : base(mark, pos, lim, cap) // package-private
		{
			this.Hb = hb;
			this.Offset = offset;
		}

		// Creates a new buffer with the given mark, position, limit, and capacity
		//
		internal LongBuffer(int mark, int pos, int lim, int cap) : this(mark, pos, lim, cap, null, 0) // package-private
		{
		}

























		/// <summary>
		/// Allocates a new long buffer.
		/// 
		/// <para> The new buffer's position will be zero, its limit will be its
		/// capacity, its mark will be undefined, and each of its elements will be
		/// initialized to zero.  It will have a <seealso cref="#array backing array"/>,
		/// and its <seealso cref="#arrayOffset array offset"/> will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="capacity">
		///         The new buffer's capacity, in longs
		/// </param>
		/// <returns>  The new long buffer
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <tt>capacity</tt> is a negative integer </exception>
		public static LongBuffer Allocate(int capacity)
		{
			if (capacity < 0)
			{
				throw new IllegalArgumentException();
			}
			return new HeapLongBuffer(capacity, capacity);
		}

		/// <summary>
		/// Wraps a long array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given long array;
		/// that is, modifications to the buffer will cause the array to be modified
		/// and vice versa.  The new buffer's capacity will be
		/// <tt>array.length</tt>, its position will be <tt>offset</tt>, its limit
		/// will be <tt>offset + length</tt>, and its mark will be undefined.  Its
		/// <seealso cref="#array backing array"/> will be the given array, and
		/// its <seealso cref="#arrayOffset array offset"/> will be zero.  </para>
		/// </summary>
		/// <param name="array">
		///         The array that will back the new buffer
		/// </param>
		/// <param name="offset">
		///         The offset of the subarray to be used; must be non-negative and
		///         no larger than <tt>array.length</tt>.  The new buffer's position
		///         will be set to this value.
		/// </param>
		/// <param name="length">
		///         The length of the subarray to be used;
		///         must be non-negative and no larger than
		///         <tt>array.length - offset</tt>.
		///         The new buffer's limit will be set to <tt>offset + length</tt>.
		/// </param>
		/// <returns>  The new long buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public static LongBuffer Wrap(long[] array, int offset, int length)
		{
			try
			{
				return new HeapLongBuffer(array, offset, length);
			}
			catch (IllegalArgumentException)
			{
				throw new IndexOutOfBoundsException();
			}
		}

		/// <summary>
		/// Wraps a long array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given long array;
		/// that is, modifications to the buffer will cause the array to be modified
		/// and vice versa.  The new buffer's capacity and limit will be
		/// <tt>array.length</tt>, its position will be zero, and its mark will be
		/// undefined.  Its <seealso cref="#array backing array"/> will be the
		/// given array, and its <seealso cref="#arrayOffset array offset>"/> will
		/// be zero.  </para>
		/// </summary>
		/// <param name="array">
		///         The array that will back this buffer
		/// </param>
		/// <returns>  The new long buffer </returns>
		public static LongBuffer Wrap(long[] array)
		{
			return Wrap(array, 0, array.Length);
		}






























































































		/// <summary>
		/// Creates a new long buffer whose content is a shared subsequence of
		/// this buffer's content.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of longs remaining in this buffer, and its mark
		/// will be undefined.  The new buffer will be direct if, and only if, this
		/// buffer is direct, and it will be read-only if, and only if, this buffer
		/// is read-only.  </para>
		/// </summary>
		/// <returns>  The new long buffer </returns>
		public abstract LongBuffer Slice();

		/// <summary>
		/// Creates a new long buffer that shares this buffer's content.
		/// 
		/// <para> The content of the new buffer will be that of this buffer.  Changes
		/// to this buffer's content will be visible in the new buffer, and vice
		/// versa; the two buffers' position, limit, and mark values will be
		/// independent.
		/// 
		/// </para>
		/// <para> The new buffer's capacity, limit, position, and mark values will be
		/// identical to those of this buffer.  The new buffer will be direct if,
		/// and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  The new long buffer </returns>
		public abstract LongBuffer Duplicate();

		/// <summary>
		/// Creates a new, read-only long buffer that shares this buffer's
		/// content.
		/// 
		/// <para> The content of the new buffer will be that of this buffer.  Changes
		/// to this buffer's content will be visible in the new buffer; the new
		/// buffer itself, however, will be read-only and will not allow the shared
		/// content to be modified.  The two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's capacity, limit, position, and mark values will be
		/// identical to those of this buffer.
		/// 
		/// </para>
		/// <para> If this buffer is itself read-only then this method behaves in
		/// exactly the same way as the <seealso cref="#duplicate duplicate"/> method.  </para>
		/// </summary>
		/// <returns>  The new, read-only long buffer </returns>
		public abstract LongBuffer AsReadOnlyBuffer();


		// -- Singleton get/put methods --

		/// <summary>
		/// Relative <i>get</i> method.  Reads the long at this buffer's
		/// current position, and then increments the position.
		/// </summary>
		/// <returns>  The long at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If the buffer's current position is not smaller than its limit </exception>
		public abstract long Get();

		/// <summary>
		/// Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given long into this buffer at the current
		/// position, and then increments the position. </para>
		/// </summary>
		/// <param name="l">
		///         The long to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If this buffer's current position is not smaller than its limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract LongBuffer Put(long l);

		/// <summary>
		/// Absolute <i>get</i> method.  Reads the long at the given
		/// index.
		/// </summary>
		/// <param name="index">
		///         The index from which the long will be read
		/// </param>
		/// <returns>  The long at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit </exception>
		public abstract long Get(int index);














		/// <summary>
		/// Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given long into this buffer at the given
		/// index. </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the long will be written
		/// </param>
		/// <param name="l">
		///         The long value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract LongBuffer Put(int index, long l);


		// -- Bulk get operations --

		/// <summary>
		/// Relative bulk <i>get</i> method.
		/// 
		/// <para> This method transfers longs from this buffer into the given
		/// destination array.  If there are fewer longs remaining in the
		/// buffer than are required to satisfy the request, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// longs are transferred and a <seealso cref="BufferUnderflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> longs from this
		/// buffer into the given array, starting at the current position of this
		/// buffer and at the given offset in the array.  The position of this
		/// buffer is then incremented by <tt>length</tt>.
		/// 
		/// </para>
		/// <para> In other words, an invocation of this method of the form
		/// <tt>src.get(dst,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
		/// the loop
		/// 
		/// <pre>{@code
		///     for (int i = off; i < off + len; i++)
		///         dst[i] = src.get():
		/// }</pre>
		/// 
		/// except that it first checks that there are sufficient longs in
		/// this buffer and it is potentially much more efficient.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dst">
		///         The array into which longs are to be written
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first long to be
		///         written; must be non-negative and no larger than
		///         <tt>dst.length</tt>
		/// </param>
		/// <param name="length">
		///         The maximum number of longs to be written to the given
		///         array; must be non-negative and no larger than
		///         <tt>dst.length - offset</tt>
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than <tt>length</tt> longs
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public virtual LongBuffer Get(long[] dst, int offset, int length)
		{
			CheckBounds(offset, length, dst.Length);
			if (length > Remaining())
			{
				throw new BufferUnderflowException();
			}
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				dst[i] = Get();
			}
			return this;
		}

		/// <summary>
		/// Relative bulk <i>get</i> method.
		/// 
		/// <para> This method transfers longs from this buffer into the given
		/// destination array.  An invocation of this method of the form
		/// <tt>src.get(a)</tt> behaves in exactly the same way as the invocation
		/// 
		/// <pre>
		///     src.get(a, 0, a.length) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="dst">
		///          The destination array
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than <tt>length</tt> longs
		///          remaining in this buffer </exception>
		public virtual LongBuffer Get(long[] dst)
		{
			return Get(dst, 0, dst.Length);
		}


		// -- Bulk put operations --

		/// <summary>
		/// Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> This method transfers the longs remaining in the given source
		/// buffer into this buffer.  If there are more longs remaining in the
		/// source buffer than in this buffer, that is, if
		/// <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
		/// then no longs are transferred and a {@link
		/// BufferOverflowException} is thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies
		/// <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> longs from the given
		/// buffer into this buffer, starting at each buffer's current position.
		/// The positions of both buffers are then incremented by <i>n</i>.
		/// 
		/// </para>
		/// <para> In other words, an invocation of this method of the form
		/// <tt>dst.put(src)</tt> has exactly the same effect as the loop
		/// 
		/// <pre>
		///     while (src.hasRemaining())
		///         dst.put(src.get()); </pre>
		/// 
		/// except that it first checks that there is sufficient space in this
		/// buffer and it is potentially much more efficient.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">
		///         The source buffer from which longs are to be read;
		///         must not be this buffer
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there is insufficient space in this buffer
		///          for the remaining longs in the source buffer
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the source buffer is this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public virtual LongBuffer Put(LongBuffer src)
		{
			if (src == this)
			{
				throw new IllegalArgumentException();
			}
			if (ReadOnly)
			{
				throw new ReadOnlyBufferException();
			}
			int n = src.Remaining();
			if (n > Remaining())
			{
				throw new BufferOverflowException();
			}
			for (int i = 0; i < n; i++)
			{
				Put(src.Get());
			}
			return this;
		}

		/// <summary>
		/// Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> This method transfers longs into this buffer from the given
		/// source array.  If there are more longs to be copied from the array
		/// than remain in this buffer, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// longs are transferred and a <seealso cref="BufferOverflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> longs from the
		/// given array into this buffer, starting at the given offset in the array
		/// and at the current position of this buffer.  The position of this buffer
		/// is then incremented by <tt>length</tt>.
		/// 
		/// </para>
		/// <para> In other words, an invocation of this method of the form
		/// <tt>dst.put(src,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
		/// the loop
		/// 
		/// <pre>{@code
		///     for (int i = off; i < off + len; i++)
		///         dst.put(a[i]);
		/// }</pre>
		/// 
		/// except that it first checks that there is sufficient space in this
		/// buffer and it is potentially much more efficient.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">
		///         The array from which longs are to be read
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first long to be read;
		///         must be non-negative and no larger than <tt>array.length</tt>
		/// </param>
		/// <param name="length">
		///         The number of longs to be read from the given array;
		///         must be non-negative and no larger than
		///         <tt>array.length - offset</tt>
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there is insufficient space in this buffer
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public virtual LongBuffer Put(long[] src, int offset, int length)
		{
			CheckBounds(offset, length, src.Length);
			if (length > Remaining())
			{
				throw new BufferOverflowException();
			}
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				this.Put(src[i]);
			}
			return this;
		}

		/// <summary>
		/// Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> This method transfers the entire content of the given source
		/// long array into this buffer.  An invocation of this method of the
		/// form <tt>dst.put(a)</tt> behaves in exactly the same way as the
		/// invocation
		/// 
		/// <pre>
		///     dst.put(a, 0, a.length) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">
		///          The source array
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there is insufficient space in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public LongBuffer Put(long[] src)
		{
			return Put(src, 0, src.Length);
		}































































































		// -- Other stuff --

		/// <summary>
		/// Tells whether or not this buffer is backed by an accessible long
		/// array.
		/// 
		/// <para> If this method returns <tt>true</tt> then the <seealso cref="#array() array"/>
		/// and <seealso cref="#arrayOffset() arrayOffset"/> methods may safely be invoked.
		/// </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this buffer
		///          is backed by an array and is not read-only </returns>
		public sealed override bool HasArray()
		{
			return (Hb != null) && !IsReadOnly;
		}

		/// <summary>
		/// Returns the long array that backs this
		/// buffer&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Modifications to this buffer's content will cause the returned
		/// array's content to be modified, and vice versa.
		/// 
		/// </para>
		/// <para> Invoke the <seealso cref="#hasArray hasArray"/> method before invoking this
		/// method in order to ensure that this buffer has an accessible backing
		/// array.  </para>
		/// </summary>
		/// <returns>  The array that backs this buffer
		/// </returns>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is backed by an array but is read-only
		/// </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If this buffer is not backed by an accessible array </exception>
		public sealed override long[] Array()
		{
			if (Hb == null)
			{
				throw new UnsupportedOperationException();
			}
			if (IsReadOnly)
			{
				throw new ReadOnlyBufferException();
			}
			return Hb;
		}

		/// <summary>
		/// Returns the offset within this buffer's backing array of the first
		/// element of the buffer&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> If this buffer is backed by an array then buffer position <i>p</i>
		/// corresponds to array index <i>p</i>&nbsp;+&nbsp;<tt>arrayOffset()</tt>.
		/// 
		/// </para>
		/// <para> Invoke the <seealso cref="#hasArray hasArray"/> method before invoking this
		/// method in order to ensure that this buffer has an accessible backing
		/// array.  </para>
		/// </summary>
		/// <returns>  The offset within this buffer's array
		///          of the first element of the buffer
		/// </returns>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is backed by an array but is read-only
		/// </exception>
		/// <exception cref="UnsupportedOperationException">
		///          If this buffer is not backed by an accessible array </exception>
		public sealed override int ArrayOffset()
		{
			if (Hb == null)
			{
				throw new UnsupportedOperationException();
			}
			if (IsReadOnly)
			{
				throw new ReadOnlyBufferException();
			}
			return Offset;
		}

		/// <summary>
		/// Compacts this buffer&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> The longs between the buffer's current position and its limit,
		/// if any, are copied to the beginning of the buffer.  That is, the
		/// long at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
		/// to index zero, the long at index <i>p</i>&nbsp;+&nbsp;1 is copied
		/// to index one, and so forth until the long at index
		/// <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
		/// <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
		/// The buffer's position is then set to <i>n+1</i> and its limit is set to
		/// its capacity.  The mark, if defined, is discarded.
		/// 
		/// </para>
		/// <para> The buffer's position is set to the number of longs copied,
		/// rather than to zero, so that an invocation of this method can be
		/// followed immediately by an invocation of another relative <i>put</i>
		/// method. </para>
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// </summary>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract LongBuffer Compact();

		/// <summary>
		/// Tells whether or not this long buffer is direct.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this buffer is direct </returns>
		public override abstract bool Direct {get;}



		/// <summary>
		/// Returns a string summarizing the state of this buffer.
		/// </summary>
		/// <returns>  A summary string </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append(this.GetType().FullName);
			sb.Append("[pos=");
			sb.Append(Position());
			sb.Append(" lim=");
			sb.Append(Limit());
			sb.Append(" cap=");
			sb.Append(Capacity());
			sb.Append("]");
			return sb.ToString();
		}






		/// <summary>
		/// Returns the current hash code of this buffer.
		/// 
		/// <para> The hash code of a long buffer depends only upon its remaining
		/// elements; that is, upon the elements from <tt>position()</tt> up to, and
		/// including, the element at <tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>.
		/// 
		/// </para>
		/// <para> Because buffer hash codes are content-dependent, it is inadvisable
		/// to use buffers as keys in hash maps or similar data structures unless it
		/// is known that their contents will not change.  </para>
		/// </summary>
		/// <returns>  The current hash code of this buffer </returns>
		public override int HashCode()
		{
			int h = 1;
			int p = Position();
			for (int i = Limit() - 1; i >= p; i--)



			{
				h = 31 * h + (int)Get(i);
			}

			return h;
		}

		/// <summary>
		/// Tells whether or not this buffer is equal to another object.
		/// 
		/// <para> Two long buffers are equal if, and only if,
		/// 
		/// <ol>
		/// 
		/// </para>
		///   <li><para> They have the same element type,  </para></li>
		/// 
		///   <li><para> They have the same number of remaining elements, and
		///   </para></li>
		/// 
		///   <li><para> The two sequences of remaining elements, considered
		///   independently of their starting positions, are pointwise equal.
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		///   </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> A long buffer is not equal to any other type of object.  </para>
		/// </summary>
		/// <param name="ob">  The object to which this buffer is to be compared
		/// </param>
		/// <returns>  <tt>true</tt> if, and only if, this buffer is equal to the
		///           given object </returns>
		public override bool Equals(Object ob)
		{
			if (this == ob)
			{
				return true;
			}
			if (!(ob is LongBuffer))
			{
				return false;
			}
			LongBuffer that = (LongBuffer)ob;
			if (this.Remaining() != that.Remaining())
			{
				return false;
			}
			int p = this.Position();
			for (int i = this.Limit() - 1, j = that.Limit() - 1; i >= p; i--, j--)
			{
				if (!Equals(this.Get(i), that.Get(j)))
				{
					return false;
				}
			}
			return true;
		}

		private static bool Equals(long x, long y)
		{



			return x == y;

		}

		/// <summary>
		/// Compares this buffer to another.
		/// 
		/// <para> Two long buffers are compared by comparing their sequences of
		/// remaining elements lexicographically, without regard to the starting
		/// position of each sequence within its corresponding buffer.
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// Pairs of {@code long} elements are compared as if by invoking
		/// <seealso cref="Long#compare(long,long)"/>.
		/// 
		/// 
		/// </para>
		/// <para> A long buffer is not comparable to any other type of object.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A negative integer, zero, or a positive integer as this buffer
		///          is less than, equal to, or greater than the given buffer </returns>
		public virtual int CompareTo(LongBuffer that)
		{
			int n = this.Position() + System.Math.Min(this.Remaining(), that.Remaining());
			for (int i = this.Position(), j = that.Position(); i < n; i++, j++)
			{
				int cmp = Compare(this.Get(i), that.Get(j));
				if (cmp != 0)
				{
					return cmp;
				}
			}
			return this.Remaining() - that.Remaining();
		}

		private static int Compare(long x, long y)
		{






			return Long.Compare(x, y);

		}

		// -- Other char stuff --


































































































































































































		// -- Other byte stuff: Access to binary data --



		/// <summary>
		/// Retrieves this buffer's byte order.
		/// 
		/// <para> The byte order of a long buffer created by allocation or by
		/// wrapping an existing <tt>long</tt> array is the {@link
		/// ByteOrder#nativeOrder native order} of the underlying
		/// hardware.  The byte order of a long buffer created as a <a
		/// href="ByteBuffer.html#views">view</a> of a byte buffer is that of the
		/// byte buffer at the moment that the view is created.  </para>
		/// </summary>
		/// <returns>  This buffer's byte order </returns>
		public abstract ByteOrder Order();

































































	}

}