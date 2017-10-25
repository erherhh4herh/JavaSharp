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
	/// A float buffer.
	/// 
	/// <para> This class defines four categories of operations upon
	/// float buffers:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> Absolute and relative <seealso cref="#get() <i>get</i>"/> and
	///   <seealso cref="#put(float) <i>put</i>"/> methods that read and write
	///   single floats; </para></li>
	/// 
	///   <li><para> Relative <seealso cref="#get(float[]) <i>bulk get</i>"/>
	///   methods that transfer contiguous sequences of floats from this buffer
	///   into an array; and</para></li>
	/// 
	///   <li><para> Relative <seealso cref="#put(float[]) <i>bulk put</i>"/>
	///   methods that transfer contiguous sequences of floats from a
	///   float array or some other float
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
	///   a float buffer.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Float buffers can be created either by {@link #allocate
	/// <i>allocation</i>}, which allocates space for the buffer's
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// content, by <seealso cref="#wrap(float[]) <i>wrapping</i>"/> an existing
	/// float array  into a buffer, or by creating a
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
	/// <para> Like a byte buffer, a float buffer is either <a
	/// href="ByteBuffer.html#direct"><i>direct</i> or <i>non-direct</i></a>.  A
	/// float buffer created via the <tt>wrap</tt> methods of this class will
	/// be non-direct.  A float buffer created as a view of a byte buffer will
	/// be direct if, and only if, the byte buffer itself is direct.  Whether or not
	/// a float buffer is direct may be determined by invoking the {@link
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

	public abstract class FloatBuffer : Buffer, Comparable<FloatBuffer>
	{

		// These fields are declared here rather than in Heap-X-Buffer in order to
		// reduce the number of virtual method invocations needed to access these
		// values, which is especially costly when coding small buffers.
		//
		internal readonly float[] Hb; // Non-null only for heap buffers
		internal readonly int Offset;
		internal bool IsReadOnly; // Valid only for heap buffers

		// Creates a new buffer with the given mark, position, limit, capacity,
		// backing array, and array offset
		//
		internal FloatBuffer(int mark, int pos, int lim, int cap, float[] hb, int offset) : base(mark, pos, lim, cap) // package-private
		{
			this.Hb = hb;
			this.Offset = offset;
		}

		// Creates a new buffer with the given mark, position, limit, and capacity
		//
		internal FloatBuffer(int mark, int pos, int lim, int cap) : this(mark, pos, lim, cap, null, 0) // package-private
		{
		}

























		/// <summary>
		/// Allocates a new float buffer.
		/// 
		/// <para> The new buffer's position will be zero, its limit will be its
		/// capacity, its mark will be undefined, and each of its elements will be
		/// initialized to zero.  It will have a <seealso cref="#array backing array"/>,
		/// and its <seealso cref="#arrayOffset array offset"/> will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="capacity">
		///         The new buffer's capacity, in floats
		/// </param>
		/// <returns>  The new float buffer
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <tt>capacity</tt> is a negative integer </exception>
		public static FloatBuffer Allocate(int capacity)
		{
			if (capacity < 0)
			{
				throw new IllegalArgumentException();
			}
			return new HeapFloatBuffer(capacity, capacity);
		}

		/// <summary>
		/// Wraps a float array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given float array;
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
		/// <returns>  The new float buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public static FloatBuffer Wrap(float[] array, int offset, int length)
		{
			try
			{
				return new HeapFloatBuffer(array, offset, length);
			}
			catch (IllegalArgumentException)
			{
				throw new IndexOutOfBoundsException();
			}
		}

		/// <summary>
		/// Wraps a float array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given float array;
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
		/// <returns>  The new float buffer </returns>
		public static FloatBuffer Wrap(float[] array)
		{
			return Wrap(array, 0, array.Length);
		}






























































































		/// <summary>
		/// Creates a new float buffer whose content is a shared subsequence of
		/// this buffer's content.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of floats remaining in this buffer, and its mark
		/// will be undefined.  The new buffer will be direct if, and only if, this
		/// buffer is direct, and it will be read-only if, and only if, this buffer
		/// is read-only.  </para>
		/// </summary>
		/// <returns>  The new float buffer </returns>
		public abstract FloatBuffer Slice();

		/// <summary>
		/// Creates a new float buffer that shares this buffer's content.
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
		/// <returns>  The new float buffer </returns>
		public abstract FloatBuffer Duplicate();

		/// <summary>
		/// Creates a new, read-only float buffer that shares this buffer's
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
		/// <returns>  The new, read-only float buffer </returns>
		public abstract FloatBuffer AsReadOnlyBuffer();


		// -- Singleton get/put methods --

		/// <summary>
		/// Relative <i>get</i> method.  Reads the float at this buffer's
		/// current position, and then increments the position.
		/// </summary>
		/// <returns>  The float at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If the buffer's current position is not smaller than its limit </exception>
		public abstract float Get();

		/// <summary>
		/// Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given float into this buffer at the current
		/// position, and then increments the position. </para>
		/// </summary>
		/// <param name="f">
		///         The float to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If this buffer's current position is not smaller than its limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract FloatBuffer Put(float f);

		/// <summary>
		/// Absolute <i>get</i> method.  Reads the float at the given
		/// index.
		/// </summary>
		/// <param name="index">
		///         The index from which the float will be read
		/// </param>
		/// <returns>  The float at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit </exception>
		public abstract float Get(int index);














		/// <summary>
		/// Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given float into this buffer at the given
		/// index. </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the float will be written
		/// </param>
		/// <param name="f">
		///         The float value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract FloatBuffer Put(int index, float f);


		// -- Bulk get operations --

		/// <summary>
		/// Relative bulk <i>get</i> method.
		/// 
		/// <para> This method transfers floats from this buffer into the given
		/// destination array.  If there are fewer floats remaining in the
		/// buffer than are required to satisfy the request, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// floats are transferred and a <seealso cref="BufferUnderflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> floats from this
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
		/// except that it first checks that there are sufficient floats in
		/// this buffer and it is potentially much more efficient.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dst">
		///         The array into which floats are to be written
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first float to be
		///         written; must be non-negative and no larger than
		///         <tt>dst.length</tt>
		/// </param>
		/// <param name="length">
		///         The maximum number of floats to be written to the given
		///         array; must be non-negative and no larger than
		///         <tt>dst.length - offset</tt>
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than <tt>length</tt> floats
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public virtual FloatBuffer Get(float[] dst, int offset, int length)
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
		/// <para> This method transfers floats from this buffer into the given
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
		///          If there are fewer than <tt>length</tt> floats
		///          remaining in this buffer </exception>
		public virtual FloatBuffer Get(float[] dst)
		{
			return Get(dst, 0, dst.Length);
		}


		// -- Bulk put operations --

		/// <summary>
		/// Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> This method transfers the floats remaining in the given source
		/// buffer into this buffer.  If there are more floats remaining in the
		/// source buffer than in this buffer, that is, if
		/// <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
		/// then no floats are transferred and a {@link
		/// BufferOverflowException} is thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies
		/// <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> floats from the given
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
		///         The source buffer from which floats are to be read;
		///         must not be this buffer
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there is insufficient space in this buffer
		///          for the remaining floats in the source buffer
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the source buffer is this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public virtual FloatBuffer Put(FloatBuffer src)
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
		/// <para> This method transfers floats into this buffer from the given
		/// source array.  If there are more floats to be copied from the array
		/// than remain in this buffer, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// floats are transferred and a <seealso cref="BufferOverflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> floats from the
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
		///         The array from which floats are to be read
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first float to be read;
		///         must be non-negative and no larger than <tt>array.length</tt>
		/// </param>
		/// <param name="length">
		///         The number of floats to be read from the given array;
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
		public virtual FloatBuffer Put(float[] src, int offset, int length)
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
		/// float array into this buffer.  An invocation of this method of the
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
		public FloatBuffer Put(float[] src)
		{
			return Put(src, 0, src.Length);
		}































































































		// -- Other stuff --

		/// <summary>
		/// Tells whether or not this buffer is backed by an accessible float
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
		/// Returns the float array that backs this
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
		public sealed override float[] Array()
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
		/// <para> The floats between the buffer's current position and its limit,
		/// if any, are copied to the beginning of the buffer.  That is, the
		/// float at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
		/// to index zero, the float at index <i>p</i>&nbsp;+&nbsp;1 is copied
		/// to index one, and so forth until the float at index
		/// <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
		/// <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
		/// The buffer's position is then set to <i>n+1</i> and its limit is set to
		/// its capacity.  The mark, if defined, is discarded.
		/// 
		/// </para>
		/// <para> The buffer's position is set to the number of floats copied,
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
		public abstract FloatBuffer Compact();

		/// <summary>
		/// Tells whether or not this float buffer is direct.
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
		/// <para> The hash code of a float buffer depends only upon its remaining
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
		/// <para> Two float buffers are equal if, and only if,
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
		///   This method considers two float elements {@code a} and {@code b}
		///   to be equal if
		///   {@code (a == b) || (Float.isNaN(a) && Float.isNaN(b))}.
		///   The values {@code -0.0} and {@code +0.0} are considered to be
		///   equal, unlike <seealso cref="Float#equals(Object)"/>.
		/// 
		///   </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> A float buffer is not equal to any other type of object.  </para>
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
			if (!(ob is FloatBuffer))
			{
				return false;
			}
			FloatBuffer that = (FloatBuffer)ob;
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

		private static bool Equals(float x, float y)
		{

			return (x == y) || (Float.IsNaN(x) && Float.IsNaN(y));



		}

		/// <summary>
		/// Compares this buffer to another.
		/// 
		/// <para> Two float buffers are compared by comparing their sequences of
		/// remaining elements lexicographically, without regard to the starting
		/// position of each sequence within its corresponding buffer.
		/// 
		/// Pairs of {@code float} elements are compared as if by invoking
		/// <seealso cref="Float#compare(float,float)"/>, except that
		/// {@code -0.0} and {@code 0.0} are considered to be equal.
		/// {@code Float.NaN} is considered by this method to be equal
		/// to itself and greater than all other {@code float} values
		/// (including {@code Float.POSITIVE_INFINITY}).
		/// 
		/// 
		/// 
		/// 
		/// 
		/// </para>
		/// <para> A float buffer is not comparable to any other type of object.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A negative integer, zero, or a positive integer as this buffer
		///          is less than, equal to, or greater than the given buffer </returns>
		public virtual int CompareTo(FloatBuffer that)
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

		private static int Compare(float x, float y)
		{

			return ((x < y) ? - 1 : (x > y) ? + 1 : (x == y) ? 0 : Float.IsNaN(x) ? (Float.IsNaN(y) ? 0 : +1) : -1);



		}

		// -- Other char stuff --


































































































































































































		// -- Other byte stuff: Access to binary data --



		/// <summary>
		/// Retrieves this buffer's byte order.
		/// 
		/// <para> The byte order of a float buffer created by allocation or by
		/// wrapping an existing <tt>float</tt> array is the {@link
		/// ByteOrder#nativeOrder native order} of the underlying
		/// hardware.  The byte order of a float buffer created as a <a
		/// href="ByteBuffer.html#views">view</a> of a byte buffer is that of the
		/// byte buffer at the moment that the view is created.  </para>
		/// </summary>
		/// <returns>  This buffer's byte order </returns>
		public abstract ByteOrder Order();

































































	}

}