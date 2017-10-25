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
	/// A byte buffer.
	/// 
	/// <para> This class defines six categories of operations upon
	/// byte buffers:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> Absolute and relative <seealso cref="#get() <i>get</i>"/> and
	///   <seealso cref="#put(byte) <i>put</i>"/> methods that read and write
	///   single bytes; </para></li>
	/// 
	///   <li><para> Relative <seealso cref="#get(byte[]) <i>bulk get</i>"/>
	///   methods that transfer contiguous sequences of bytes from this buffer
	///   into an array; </para></li>
	/// 
	///   <li><para> Relative <seealso cref="#put(byte[]) <i>bulk put</i>"/>
	///   methods that transfer contiguous sequences of bytes from a
	///   byte array or some other byte
	///   buffer into this buffer; </para></li>
	/// 
	/// 
	/// 
	///   <li><para> Absolute and relative <seealso cref="#getChar() <i>get</i>"/>
	///   and <seealso cref="#putChar(char) <i>put</i>"/> methods that read and
	///   write values of other primitive types, translating them to and from
	///   sequences of bytes in a particular byte order; </para></li>
	/// 
	///   <li><para> Methods for creating <i><a href="#views">view buffers</a></i>,
	///   which allow a byte buffer to be viewed as a buffer containing values of
	///   some other primitive type; and </para></li>
	/// 
	/// 
	/// 
	///   <li><para> Methods for <seealso cref="#compact compacting"/>, {@link
	///   #duplicate duplicating}, and <seealso cref="#slice slicing"/>
	///   a byte buffer.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Byte buffers can be created either by {@link #allocate
	/// <i>allocation</i>}, which allocates space for the buffer's
	/// 
	/// 
	/// 
	/// content, or by <seealso cref="#wrap(byte[]) <i>wrapping</i>"/> an
	/// existing byte array  into a buffer.
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// <a name="direct"></a>
	/// <h2> Direct <i>vs.</i> non-direct buffers </h2>
	/// 
	/// </para>
	/// <para> A byte buffer is either <i>direct</i> or <i>non-direct</i>.  Given a
	/// direct byte buffer, the Java virtual machine will make a best effort to
	/// perform native I/O operations directly upon it.  That is, it will attempt to
	/// avoid copying the buffer's content to (or from) an intermediate buffer
	/// before (or after) each invocation of one of the underlying operating
	/// system's native I/O operations.
	/// 
	/// </para>
	/// <para> A direct byte buffer may be created by invoking the {@link
	/// #allocateDirect(int) allocateDirect} factory method of this class.  The
	/// buffers returned by this method typically have somewhat higher allocation
	/// and deallocation costs than non-direct buffers.  The contents of direct
	/// buffers may reside outside of the normal garbage-collected heap, and so
	/// their impact upon the memory footprint of an application might not be
	/// obvious.  It is therefore recommended that direct buffers be allocated
	/// primarily for large, long-lived buffers that are subject to the underlying
	/// system's native I/O operations.  In general it is best to allocate direct
	/// buffers only when they yield a measureable gain in program performance.
	/// 
	/// </para>
	/// <para> A direct byte buffer may also be created by {@link
	/// java.nio.channels.FileChannel#map mapping} a region of a file
	/// directly into memory.  An implementation of the Java platform may optionally
	/// support the creation of direct byte buffers from native code via JNI.  If an
	/// instance of one of these kinds of buffers refers to an inaccessible region
	/// of memory then an attempt to access that region will not change the buffer's
	/// content and will cause an unspecified exception to be thrown either at the
	/// time of the access or at some later time.
	/// 
	/// </para>
	/// <para> Whether a byte buffer is direct or non-direct may be determined by
	/// invoking its <seealso cref="#isDirect isDirect"/> method.  This method is provided so
	/// that explicit buffer management can be done in performance-critical code.
	/// 
	/// 
	/// <a name="bin"></a>
	/// <h2> Access to binary data </h2>
	/// 
	/// </para>
	/// <para> This class defines methods for reading and writing values of all other
	/// primitive types, except <tt>boolean</tt>.  Primitive values are translated
	/// to (or from) sequences of bytes according to the buffer's current byte
	/// order, which may be retrieved and modified via the <seealso cref="#order order"/>
	/// methods.  Specific byte orders are represented by instances of the {@link
	/// ByteOrder} class.  The initial order of a byte buffer is always {@link
	/// ByteOrder#BIG_ENDIAN BIG_ENDIAN}.
	/// 
	/// </para>
	/// <para> For access to heterogeneous binary data, that is, sequences of values of
	/// different types, this class defines a family of absolute and relative
	/// <i>get</i> and <i>put</i> methods for each type.  For 32-bit floating-point
	/// values, for example, this class defines:
	/// 
	/// <blockquote><pre>
	/// float  <seealso cref="#getFloat()"/>
	/// float  <seealso cref="#getFloat(int) getFloat(int index)"/>
	///  void  <seealso cref="#putFloat(float) putFloat(float f)"/>
	///  void  <seealso cref="#putFloat(int,float) putFloat(int index, float f)"/></pre></blockquote>
	/// 
	/// </para>
	/// <para> Corresponding methods are defined for the types <tt>char</tt>,
	/// <tt>short</tt>, <tt>int</tt>, <tt>long</tt>, and <tt>double</tt>.  The index
	/// parameters of the absolute <i>get</i> and <i>put</i> methods are in terms of
	/// bytes rather than of the type being read or written.
	/// 
	/// <a name="views"></a>
	/// 
	/// </para>
	/// <para> For access to homogeneous binary data, that is, sequences of values of
	/// the same type, this class defines methods that can create <i>views</i> of a
	/// given byte buffer.  A <i>view buffer</i> is simply another buffer whose
	/// content is backed by the byte buffer.  Changes to the byte buffer's content
	/// will be visible in the view buffer, and vice versa; the two buffers'
	/// position, limit, and mark values are independent.  The {@link
	/// #asFloatBuffer() asFloatBuffer} method, for example, creates an instance of
	/// the <seealso cref="FloatBuffer"/> class that is backed by the byte buffer upon which
	/// the method is invoked.  Corresponding view-creation methods are defined for
	/// the types <tt>char</tt>, <tt>short</tt>, <tt>int</tt>, <tt>long</tt>, and
	/// <tt>double</tt>.
	/// 
	/// </para>
	/// <para> View buffers have three important advantages over the families of
	/// type-specific <i>get</i> and <i>put</i> methods described above:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> A view buffer is indexed not in terms of bytes but rather in terms
	///   of the type-specific size of its values;  </para></li>
	/// 
	///   <li><para> A view buffer provides relative bulk <i>get</i> and <i>put</i>
	///   methods that can transfer contiguous sequences of values between a buffer
	///   and an array or some other buffer of the same type; and  </para></li>
	/// 
	///   <li><para> A view buffer is potentially much more efficient because it will
	///   be direct if, and only if, its backing byte buffer is direct.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> The byte order of a view buffer is fixed to be that of its byte buffer
	/// at the time that the view is created.  </para>
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// <h2> Invocation chaining </h2>
	/// 
	/// 
	/// <para> Methods in this class that do not otherwise have a value to return are
	/// specified to return the buffer upon which they are invoked.  This allows
	/// method invocations to be chained.
	/// 
	/// 
	/// 
	/// The sequence of statements
	/// 
	/// <blockquote><pre>
	/// bb.putInt(0xCAFEBABE);
	/// bb.putShort(3);
	/// bb.putShort(45);</pre></blockquote>
	/// 
	/// can, for example, be replaced by the single statement
	/// 
	/// <blockquote><pre>
	/// bb.putInt(0xCAFEBABE).putShort(3).putShort(45);</pre></blockquote>
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
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

	public abstract class ByteBuffer : Buffer, Comparable<ByteBuffer>
	{

		// These fields are declared here rather than in Heap-X-Buffer in order to
		// reduce the number of virtual method invocations needed to access these
		// values, which is especially costly when coding small buffers.
		//
		internal readonly sbyte[] Hb; // Non-null only for heap buffers
		internal readonly int Offset;
		internal bool IsReadOnly; // Valid only for heap buffers

		// Creates a new buffer with the given mark, position, limit, capacity,
		// backing array, and array offset
		//
		internal ByteBuffer(int mark, int pos, int lim, int cap, sbyte[] hb, int offset) : base(mark, pos, lim, cap) // package-private
		{
			this.Hb = hb;
			this.Offset = offset;
		}

		// Creates a new buffer with the given mark, position, limit, and capacity
		//
		internal ByteBuffer(int mark, int pos, int lim, int cap) : this(mark, pos, lim, cap, null, 0) // package-private
		{
		}



		/// <summary>
		/// Allocates a new direct byte buffer.
		/// 
		/// <para> The new buffer's position will be zero, its limit will be its
		/// capacity, its mark will be undefined, and each of its elements will be
		/// initialized to zero.  Whether or not it has a
		/// <seealso cref="#hasArray backing array"/> is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <param name="capacity">
		///         The new buffer's capacity, in bytes
		/// </param>
		/// <returns>  The new byte buffer
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <tt>capacity</tt> is a negative integer </exception>
		public static ByteBuffer AllocateDirect(int capacity)
		{
			return new DirectByteBuffer(capacity);
		}



		/// <summary>
		/// Allocates a new byte buffer.
		/// 
		/// <para> The new buffer's position will be zero, its limit will be its
		/// capacity, its mark will be undefined, and each of its elements will be
		/// initialized to zero.  It will have a <seealso cref="#array backing array"/>,
		/// and its <seealso cref="#arrayOffset array offset"/> will be zero.
		/// 
		/// </para>
		/// </summary>
		/// <param name="capacity">
		///         The new buffer's capacity, in bytes
		/// </param>
		/// <returns>  The new byte buffer
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the <tt>capacity</tt> is a negative integer </exception>
		public static ByteBuffer Allocate(int capacity)
		{
			if (capacity < 0)
			{
				throw new IllegalArgumentException();
			}
			return new HeapByteBuffer(capacity, capacity);
		}

		/// <summary>
		/// Wraps a byte array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given byte array;
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
		/// <returns>  The new byte buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public static ByteBuffer Wrap(sbyte[] array, int offset, int length)
		{
			try
			{
				return new HeapByteBuffer(array, offset, length);
			}
			catch (IllegalArgumentException)
			{
				throw new IndexOutOfBoundsException();
			}
		}

		/// <summary>
		/// Wraps a byte array into a buffer.
		/// 
		/// <para> The new buffer will be backed by the given byte array;
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
		/// <returns>  The new byte buffer </returns>
		public static ByteBuffer Wrap(sbyte[] array)
		{
			return Wrap(array, 0, array.Length);
		}






























































































		/// <summary>
		/// Creates a new byte buffer whose content is a shared subsequence of
		/// this buffer's content.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer, and its mark
		/// will be undefined.  The new buffer will be direct if, and only if, this
		/// buffer is direct, and it will be read-only if, and only if, this buffer
		/// is read-only.  </para>
		/// </summary>
		/// <returns>  The new byte buffer </returns>
		public abstract ByteBuffer Slice();

		/// <summary>
		/// Creates a new byte buffer that shares this buffer's content.
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
		/// <returns>  The new byte buffer </returns>
		public abstract ByteBuffer Duplicate();

		/// <summary>
		/// Creates a new, read-only byte buffer that shares this buffer's
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
		/// <returns>  The new, read-only byte buffer </returns>
		public abstract ByteBuffer AsReadOnlyBuffer();


		// -- Singleton get/put methods --

		/// <summary>
		/// Relative <i>get</i> method.  Reads the byte at this buffer's
		/// current position, and then increments the position.
		/// </summary>
		/// <returns>  The byte at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If the buffer's current position is not smaller than its limit </exception>
		public abstract sbyte Get();

		/// <summary>
		/// Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given byte into this buffer at the current
		/// position, and then increments the position. </para>
		/// </summary>
		/// <param name="b">
		///         The byte to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If this buffer's current position is not smaller than its limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer Put(sbyte b);

		/// <summary>
		/// Absolute <i>get</i> method.  Reads the byte at the given
		/// index.
		/// </summary>
		/// <param name="index">
		///         The index from which the byte will be read
		/// </param>
		/// <returns>  The byte at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit </exception>
		public abstract sbyte Get(int index);














		/// <summary>
		/// Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes the given byte into this buffer at the given
		/// index. </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the byte will be written
		/// </param>
		/// <param name="b">
		///         The byte value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer Put(int index, sbyte b);


		// -- Bulk get operations --

		/// <summary>
		/// Relative bulk <i>get</i> method.
		/// 
		/// <para> This method transfers bytes from this buffer into the given
		/// destination array.  If there are fewer bytes remaining in the
		/// buffer than are required to satisfy the request, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// bytes are transferred and a <seealso cref="BufferUnderflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> bytes from this
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
		/// except that it first checks that there are sufficient bytes in
		/// this buffer and it is potentially much more efficient.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dst">
		///         The array into which bytes are to be written
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first byte to be
		///         written; must be non-negative and no larger than
		///         <tt>dst.length</tt>
		/// </param>
		/// <param name="length">
		///         The maximum number of bytes to be written to the given
		///         array; must be non-negative and no larger than
		///         <tt>dst.length - offset</tt>
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than <tt>length</tt> bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
		///          parameters do not hold </exception>
		public virtual ByteBuffer Get(sbyte[] dst, int offset, int length)
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
		/// <para> This method transfers bytes from this buffer into the given
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
		///          If there are fewer than <tt>length</tt> bytes
		///          remaining in this buffer </exception>
		public virtual ByteBuffer Get(sbyte[] dst)
		{
			return Get(dst, 0, dst.Length);
		}


		// -- Bulk put operations --

		/// <summary>
		/// Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> This method transfers the bytes remaining in the given source
		/// buffer into this buffer.  If there are more bytes remaining in the
		/// source buffer than in this buffer, that is, if
		/// <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
		/// then no bytes are transferred and a {@link
		/// BufferOverflowException} is thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies
		/// <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> bytes from the given
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
		///         The source buffer from which bytes are to be read;
		///         must not be this buffer
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there is insufficient space in this buffer
		///          for the remaining bytes in the source buffer
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the source buffer is this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public virtual ByteBuffer Put(ByteBuffer src)
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
		/// <para> This method transfers bytes into this buffer from the given
		/// source array.  If there are more bytes to be copied from the array
		/// than remain in this buffer, that is, if
		/// <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
		/// bytes are transferred and a <seealso cref="BufferOverflowException"/> is
		/// thrown.
		/// 
		/// </para>
		/// <para> Otherwise, this method copies <tt>length</tt> bytes from the
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
		///         The array from which bytes are to be read
		/// </param>
		/// <param name="offset">
		///         The offset within the array of the first byte to be read;
		///         must be non-negative and no larger than <tt>array.length</tt>
		/// </param>
		/// <param name="length">
		///         The number of bytes to be read from the given array;
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
		public virtual ByteBuffer Put(sbyte[] src, int offset, int length)
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
		/// byte array into this buffer.  An invocation of this method of the
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
		public ByteBuffer Put(sbyte[] src)
		{
			return Put(src, 0, src.Length);
		}































































































		// -- Other stuff --

		/// <summary>
		/// Tells whether or not this buffer is backed by an accessible byte
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
		/// Returns the byte array that backs this
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
		public sealed override sbyte[] Array()
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
		/// <para> The bytes between the buffer's current position and its limit,
		/// if any, are copied to the beginning of the buffer.  That is, the
		/// byte at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
		/// to index zero, the byte at index <i>p</i>&nbsp;+&nbsp;1 is copied
		/// to index one, and so forth until the byte at index
		/// <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
		/// <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
		/// The buffer's position is then set to <i>n+1</i> and its limit is set to
		/// its capacity.  The mark, if defined, is discarded.
		/// 
		/// </para>
		/// <para> The buffer's position is set to the number of bytes copied,
		/// rather than to zero, so that an invocation of this method can be
		/// followed immediately by an invocation of another relative <i>put</i>
		/// method. </para>
		/// 
		/// 
		/// 
		/// <para> Invoke this method after writing data from a buffer in case the
		/// write was incomplete.  The following loop, for example, copies bytes
		/// from one channel to another via the buffer <tt>buf</tt>:
		/// 
		/// <blockquote><pre>{@code
		///   buf.clear();          // Prepare buffer for use
		///   while (in.read(buf) >= 0 || buf.position != 0) {
		///       buf.flip();
		///       out.write(buf);
		///       buf.compact();    // In case of partial write
		///   }
		/// }</pre></blockquote>
		/// 
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer Compact();

		/// <summary>
		/// Tells whether or not this byte buffer is direct.
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
		/// <para> The hash code of a byte buffer depends only upon its remaining
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
		/// <para> Two byte buffers are equal if, and only if,
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
		/// <para> A byte buffer is not equal to any other type of object.  </para>
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
			if (!(ob is ByteBuffer))
			{
				return false;
			}
			ByteBuffer that = (ByteBuffer)ob;
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

		private static bool Equals(sbyte x, sbyte y)
		{



			return x == y;

		}

		/// <summary>
		/// Compares this buffer to another.
		/// 
		/// <para> Two byte buffers are compared by comparing their sequences of
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
		/// Pairs of {@code byte} elements are compared as if by invoking
		/// <seealso cref="Byte#compare(byte,byte)"/>.
		/// 
		/// 
		/// </para>
		/// <para> A byte buffer is not comparable to any other type of object.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A negative integer, zero, or a positive integer as this buffer
		///          is less than, equal to, or greater than the given buffer </returns>
		public virtual int CompareTo(ByteBuffer that)
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

		private static int Compare(sbyte x, sbyte y)
		{






			return Byte.Compare(x, y);

		}

		// -- Other char stuff --


































































































































































































		// -- Other byte stuff: Access to binary data --





















		internal bool BigEndian = true; // package-private
		internal bool NativeByteOrder = (Bits.ByteOrder() == ByteOrder.BIG_ENDIAN); // package-private

		/// <summary>
		/// Retrieves this buffer's byte order.
		/// 
		/// <para> The byte order is used when reading or writing multibyte values, and
		/// when creating buffers that are views of this byte buffer.  The order of
		/// a newly-created byte buffer is always {@link ByteOrder#BIG_ENDIAN
		/// BIG_ENDIAN}.  </para>
		/// </summary>
		/// <returns>  This buffer's byte order </returns>
		public ByteOrder Order()
		{
			return BigEndian ? ByteOrder.BIG_ENDIAN : ByteOrder.LITTLE_ENDIAN;
		}

		/// <summary>
		/// Modifies this buffer's byte order.
		/// </summary>
		/// <param name="bo">
		///         The new byte order,
		///         either <seealso cref="ByteOrder#BIG_ENDIAN BIG_ENDIAN"/>
		///         or <seealso cref="ByteOrder#LITTLE_ENDIAN LITTLE_ENDIAN"/>
		/// </param>
		/// <returns>  This buffer </returns>
		public ByteBuffer Order(ByteOrder bo)
		{
			BigEndian = (bo == ByteOrder.BIG_ENDIAN);
			NativeByteOrder = (BigEndian == (Bits.ByteOrder() == ByteOrder.BIG_ENDIAN));
			return this;
		}

		// Unchecked accessors, for use by ByteBufferAs-X-Buffer classes
		//
		internal abstract sbyte _get(int i); // package-private
		internal abstract void _put(int i, sbyte b); // package-private


		/// <summary>
		/// Relative <i>get</i> method for reading a char value.
		/// 
		/// <para> Reads the next two bytes at this buffer's current position,
		/// composing them into a char value according to the current byte order,
		/// and then increments the position by two.  </para>
		/// </summary>
		/// <returns>  The char value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than two bytes
		///          remaining in this buffer </exception>
		public abstract char Char {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing a char
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes two bytes containing the given char value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by two.  </para>
		/// </summary>
		/// <param name="value">
		///         The char value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than two bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutChar(char value);

		/// <summary>
		/// Absolute <i>get</i> method for reading a char value.
		/// 
		/// <para> Reads two bytes at the given index, composing them into a
		/// char value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The char value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus one </exception>
		public abstract char GetChar(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing a char
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes two bytes containing the given char value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The char value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus one
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutChar(int index, char value);

		/// <summary>
		/// Creates a view of this byte buffer as a char buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// two, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new char buffer </returns>
		public abstract CharBuffer AsCharBuffer();


		/// <summary>
		/// Relative <i>get</i> method for reading a short value.
		/// 
		/// <para> Reads the next two bytes at this buffer's current position,
		/// composing them into a short value according to the current byte order,
		/// and then increments the position by two.  </para>
		/// </summary>
		/// <returns>  The short value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than two bytes
		///          remaining in this buffer </exception>
		public abstract short Short {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing a short
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes two bytes containing the given short value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by two.  </para>
		/// </summary>
		/// <param name="value">
		///         The short value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than two bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutShort(short value);

		/// <summary>
		/// Absolute <i>get</i> method for reading a short value.
		/// 
		/// <para> Reads two bytes at the given index, composing them into a
		/// short value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The short value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus one </exception>
		public abstract short GetShort(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing a short
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes two bytes containing the given short value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The short value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus one
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutShort(int index, short value);

		/// <summary>
		/// Creates a view of this byte buffer as a short buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// two, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new short buffer </returns>
		public abstract ShortBuffer AsShortBuffer();


		/// <summary>
		/// Relative <i>get</i> method for reading an int value.
		/// 
		/// <para> Reads the next four bytes at this buffer's current position,
		/// composing them into an int value according to the current byte order,
		/// and then increments the position by four.  </para>
		/// </summary>
		/// <returns>  The int value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than four bytes
		///          remaining in this buffer </exception>
		public abstract int Int {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing an int
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes four bytes containing the given int value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by four.  </para>
		/// </summary>
		/// <param name="value">
		///         The int value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than four bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutInt(int value);

		/// <summary>
		/// Absolute <i>get</i> method for reading an int value.
		/// 
		/// <para> Reads four bytes at the given index, composing them into a
		/// int value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The int value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus three </exception>
		public abstract int GetInt(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing an int
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes four bytes containing the given int value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The int value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus three
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutInt(int index, int value);

		/// <summary>
		/// Creates a view of this byte buffer as an int buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// four, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new int buffer </returns>
		public abstract IntBuffer AsIntBuffer();


		/// <summary>
		/// Relative <i>get</i> method for reading a long value.
		/// 
		/// <para> Reads the next eight bytes at this buffer's current position,
		/// composing them into a long value according to the current byte order,
		/// and then increments the position by eight.  </para>
		/// </summary>
		/// <returns>  The long value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than eight bytes
		///          remaining in this buffer </exception>
		public abstract long Long {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing a long
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes eight bytes containing the given long value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by eight.  </para>
		/// </summary>
		/// <param name="value">
		///         The long value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than eight bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutLong(long value);

		/// <summary>
		/// Absolute <i>get</i> method for reading a long value.
		/// 
		/// <para> Reads eight bytes at the given index, composing them into a
		/// long value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The long value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus seven </exception>
		public abstract long GetLong(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing a long
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes eight bytes containing the given long value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The long value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus seven
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutLong(int index, long value);

		/// <summary>
		/// Creates a view of this byte buffer as a long buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// eight, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new long buffer </returns>
		public abstract LongBuffer AsLongBuffer();


		/// <summary>
		/// Relative <i>get</i> method for reading a float value.
		/// 
		/// <para> Reads the next four bytes at this buffer's current position,
		/// composing them into a float value according to the current byte order,
		/// and then increments the position by four.  </para>
		/// </summary>
		/// <returns>  The float value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than four bytes
		///          remaining in this buffer </exception>
		public abstract float Float {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing a float
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes four bytes containing the given float value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by four.  </para>
		/// </summary>
		/// <param name="value">
		///         The float value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than four bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutFloat(float value);

		/// <summary>
		/// Absolute <i>get</i> method for reading a float value.
		/// 
		/// <para> Reads four bytes at the given index, composing them into a
		/// float value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The float value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus three </exception>
		public abstract float GetFloat(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing a float
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes four bytes containing the given float value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The float value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus three
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutFloat(int index, float value);

		/// <summary>
		/// Creates a view of this byte buffer as a float buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// four, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new float buffer </returns>
		public abstract FloatBuffer AsFloatBuffer();


		/// <summary>
		/// Relative <i>get</i> method for reading a double value.
		/// 
		/// <para> Reads the next eight bytes at this buffer's current position,
		/// composing them into a double value according to the current byte order,
		/// and then increments the position by eight.  </para>
		/// </summary>
		/// <returns>  The double value at the buffer's current position
		/// </returns>
		/// <exception cref="BufferUnderflowException">
		///          If there are fewer than eight bytes
		///          remaining in this buffer </exception>
		public abstract double Double {get;}

		/// <summary>
		/// Relative <i>put</i> method for writing a double
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes eight bytes containing the given double value, in the
		/// current byte order, into this buffer at the current position, and then
		/// increments the position by eight.  </para>
		/// </summary>
		/// <param name="value">
		///         The double value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="BufferOverflowException">
		///          If there are fewer than eight bytes
		///          remaining in this buffer
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutDouble(double value);

		/// <summary>
		/// Absolute <i>get</i> method for reading a double value.
		/// 
		/// <para> Reads eight bytes at the given index, composing them into a
		/// double value according to the current byte order.  </para>
		/// </summary>
		/// <param name="index">
		///         The index from which the bytes will be read
		/// </param>
		/// <returns>  The double value at the given index
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus seven </exception>
		public abstract double GetDouble(int index);

		/// <summary>
		/// Absolute <i>put</i> method for writing a double
		/// value&nbsp;&nbsp;<i>(optional operation)</i>.
		/// 
		/// <para> Writes eight bytes containing the given double value, in the
		/// current byte order, into this buffer at the given index.  </para>
		/// </summary>
		/// <param name="index">
		///         The index at which the bytes will be written
		/// </param>
		/// <param name="value">
		///         The double value to be written
		/// </param>
		/// <returns>  This buffer
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>index</tt> is negative
		///          or not smaller than the buffer's limit,
		///          minus seven
		/// </exception>
		/// <exception cref="ReadOnlyBufferException">
		///          If this buffer is read-only </exception>
		public abstract ByteBuffer PutDouble(int index, double value);

		/// <summary>
		/// Creates a view of this byte buffer as a double buffer.
		/// 
		/// <para> The content of the new buffer will start at this buffer's current
		/// position.  Changes to this buffer's content will be visible in the new
		/// buffer, and vice versa; the two buffers' position, limit, and mark
		/// values will be independent.
		/// 
		/// </para>
		/// <para> The new buffer's position will be zero, its capacity and its limit
		/// will be the number of bytes remaining in this buffer divided by
		/// eight, and its mark will be undefined.  The new buffer will be direct
		/// if, and only if, this buffer is direct, and it will be read-only if, and
		/// only if, this buffer is read-only.  </para>
		/// </summary>
		/// <returns>  A new double buffer </returns>
		public abstract DoubleBuffer AsDoubleBuffer();

	}

}