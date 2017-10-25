/*
 * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// The management interface for the garbage collection of
	/// the Java virtual machine.  Garbage collection is the process
	/// that the Java virtual machine uses to find and reclaim unreachable
	/// objects to free up memory space.  A garbage collector is one type of
	/// <seealso cref="MemoryManagerMXBean memory manager"/>.
	/// 
	/// <para> A Java virtual machine may have one or more instances of
	/// the implementation class of this interface.
	/// An instance implementing this interface is
	/// an <a href="ManagementFactory.html#MXBean">MXBean</a>
	/// that can be obtained by calling
	/// the <seealso cref="ManagementFactory#getGarbageCollectorMXBeans"/> method or
	/// from the {@link ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>} method.
	/// 
	/// </para>
	/// <para>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	/// a garbage collector within an MBeanServer is:
	/// <blockquote>
	///   {@link ManagementFactory#GARBAGE_COLLECTOR_MXBEAN_DOMAIN_TYPE
	///    <tt>java.lang:type=GarbageCollector</tt>}<tt>,name=</tt><i>collector's name</i>
	/// </blockquote>
	/// 
	/// It can be obtained by calling the
	/// <seealso cref="PlatformManagedObject#getObjectName"/> method.
	/// 
	/// A platform usually includes additional platform-dependent information
	/// specific to a garbage collection algorithm for monitoring.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ManagementFactory#getPlatformMXBeans(Class) </seealso>
	/// <seealso cref= MemoryMXBean
	/// </seealso>
	/// <seealso cref= <a href="../../../javax/management/package-summary.html">
	///      JMX Specification.</a> </seealso>
	/// <seealso cref= <a href="package-summary.html#examples">
	///      Ways to Access MXBeans</a>
	/// 
	/// @author  Mandy Chung
	/// @since   1.5 </seealso>
	public interface GarbageCollectorMXBean : MemoryManagerMXBean
	{
		/// <summary>
		/// Returns the total number of collections that have occurred.
		/// This method returns <tt>-1</tt> if the collection count is undefined for
		/// this collector.
		/// </summary>
		/// <returns> the total number of collections that have occurred. </returns>
		long CollectionCount {get;}

		/// <summary>
		/// Returns the approximate accumulated collection elapsed time
		/// in milliseconds.  This method returns <tt>-1</tt> if the collection
		/// elapsed time is undefined for this collector.
		/// <para>
		/// The Java virtual machine implementation may use a high resolution
		/// timer to measure the elapsed time.  This method may return the
		/// same value even if the collection count has been incremented
		/// if the collection elapsed time is very short.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the approximate accumulated collection elapsed time
		/// in milliseconds. </returns>
		long CollectionTime {get;}


	}

}