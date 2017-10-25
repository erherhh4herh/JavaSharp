/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	/// <summary>
	/// The management interface for a buffer pool, for example a pool of
	/// <seealso cref="java.nio.ByteBuffer#allocateDirect direct"/> or {@link
	/// java.nio.MappedByteBuffer mapped} buffers.
	/// 
	/// <para> A class implementing this interface is an
	/// <seealso cref="javax.management.MXBean"/>. A Java
	/// virtual machine has one or more implementations of this interface. The {@link
	/// java.lang.management.ManagementFactory#getPlatformMXBeans getPlatformMXBeans}
	/// method can be used to obtain the list of {@code BufferPoolMXBean} objects
	/// representing the management interfaces for pools of buffers as follows:
	/// <pre>
	///     List&lt;BufferPoolMXBean&gt; pools = ManagementFactory.getPlatformMXBeans(BufferPoolMXBean.class);
	/// </pre>
	/// 
	/// </para>
	/// <para> The management interfaces are also registered with the platform {@link
	/// javax.management.MBeanServer MBeanServer}. The {@link
	/// javax.management.ObjectName ObjectName} that uniquely identifies the
	/// management interface within the {@code MBeanServer} takes the form:
	/// <pre>
	///     java.nio:type=BufferPool,name=<i>pool name</i>
	/// </pre>
	/// where <em>pool name</em> is the <seealso cref="#getName name"/> of the buffer pool.
	/// 
	/// @since   1.7
	/// </para>
	/// </summary>
	public interface BufferPoolMXBean : PlatformManagedObject
	{

		/// <summary>
		/// Returns the name representing this buffer pool.
		/// </summary>
		/// <returns>  The name of this buffer pool. </returns>
		String Name {get;}

		/// <summary>
		/// Returns an estimate of the number of buffers in the pool.
		/// </summary>
		/// <returns>  An estimate of the number of buffers in this pool </returns>
		long Count {get;}

		/// <summary>
		/// Returns an estimate of the total capacity of the buffers in this pool.
		/// A buffer's capacity is the number of elements it contains and the value
		/// returned by this method is an estimate of the total capacity of buffers
		/// in the pool in bytes.
		/// </summary>
		/// <returns>  An estimate of the total capacity of the buffers in this pool
		///          in bytes </returns>
		long TotalCapacity {get;}

		/// <summary>
		/// Returns an estimate of the memory that the Java virtual machine is using
		/// for this buffer pool. The value returned by this method may differ
		/// from the estimate of the total <seealso cref="#getTotalCapacity capacity"/> of
		/// the buffers in this pool. This difference is explained by alignment,
		/// memory allocator, and other implementation specific reasons.
		/// </summary>
		/// <returns>  An estimate of the memory that the Java virtual machine is using
		///          for this buffer pool in bytes, or {@code -1L} if an estimate of
		///          the memory usage is not available </returns>
		long MemoryUsed {get;}
	}

}