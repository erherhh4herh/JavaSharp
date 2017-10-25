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
	/// The management interface for a memory manager.
	/// A memory manager manages one or more memory pools of the
	/// Java virtual machine.
	/// 
	/// <para> A Java virtual machine has one or more memory managers.
	/// An instance implementing this interface is
	/// an <a href="ManagementFactory.html#MXBean">MXBean</a>
	/// that can be obtained by calling
	/// the <seealso cref="ManagementFactory#getMemoryManagerMXBeans"/> method or
	/// from the {@link ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>} method.
	/// 
	/// </para>
	/// <para>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	/// a memory manager within an MBeanServer is:
	/// <blockquote>
	///   {@link ManagementFactory#MEMORY_MANAGER_MXBEAN_DOMAIN_TYPE
	///    <tt>java.lang:type=MemoryManager</tt>}<tt>,name=</tt><i>manager's name</i>
	/// </blockquote>
	/// 
	/// It can be obtained by calling the
	/// <seealso cref="PlatformManagedObject#getObjectName"/> method.
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
	public interface MemoryManagerMXBean : PlatformManagedObject
	{
		/// <summary>
		/// Returns the name representing this memory manager.
		/// </summary>
		/// <returns> the name of this memory manager. </returns>
		String Name {get;}

		/// <summary>
		/// Tests if this memory manager is valid in the Java virtual
		/// machine.  A memory manager becomes invalid once the Java virtual
		/// machine removes it from the memory system.
		/// </summary>
		/// <returns> <tt>true</tt> if the memory manager is valid in the
		///               Java virtual machine;
		///         <tt>false</tt> otherwise. </returns>
		bool Valid {get;}

		/// <summary>
		/// Returns the name of memory pools that this memory manager manages.
		/// </summary>
		/// <returns> an array of <tt>String</tt> objects, each is
		/// the name of a memory pool that this memory manager manages. </returns>
		String[] MemoryPoolNames {get;}
	}

}