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
	/// The management interface for the operating system on which
	/// the Java virtual machine is running.
	/// 
	/// <para> A Java virtual machine has a single instance of the implementation
	/// class of this interface.  This instance implementing this interface is
	/// an <a href="ManagementFactory.html#MXBean">MXBean</a>
	/// that can be obtained by calling
	/// the <seealso cref="ManagementFactory#getOperatingSystemMXBean"/> method or
	/// from the {@link ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>} method.
	/// 
	/// </para>
	/// <para>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	/// the operating system within an MBeanServer is:
	/// <blockquote>
	///    {@link ManagementFactory#OPERATING_SYSTEM_MXBEAN_NAME
	///      <tt>java.lang:type=OperatingSystem</tt>}
	/// </blockquote>
	/// 
	/// It can be obtained by calling the
	/// <seealso cref="PlatformManagedObject#getObjectName"/> method.
	/// 
	/// </para>
	/// <para> This interface defines several convenient methods for accessing
	/// system properties about the operating system on which the Java
	/// virtual machine is running.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ManagementFactory#getPlatformMXBeans(Class) </seealso>
	/// <seealso cref= <a href="../../../javax/management/package-summary.html">
	///      JMX Specification.</a> </seealso>
	/// <seealso cref= <a href="package-summary.html#examples">
	///      Ways to Access MXBeans</a>
	/// 
	/// @author  Mandy Chung
	/// @since   1.5 </seealso>
	public interface OperatingSystemMXBean : PlatformManagedObject
	{
		/// <summary>
		/// Returns the operating system name.
		/// This method is equivalent to <tt>System.getProperty("os.name")</tt>.
		/// </summary>
		/// <returns> the operating system name.
		/// </returns>
		/// <exception cref="java.lang.SecurityException">
		///     if a security manager exists and its
		///     <code>checkPropertiesAccess</code> method doesn't allow access
		///     to this system property. </exception>
		/// <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		/// <seealso cref= java.lang.System#getProperty </seealso>
		String Name {get;}

		/// <summary>
		/// Returns the operating system architecture.
		/// This method is equivalent to <tt>System.getProperty("os.arch")</tt>.
		/// </summary>
		/// <returns> the operating system architecture.
		/// </returns>
		/// <exception cref="java.lang.SecurityException">
		///     if a security manager exists and its
		///     <code>checkPropertiesAccess</code> method doesn't allow access
		///     to this system property. </exception>
		/// <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		/// <seealso cref= java.lang.System#getProperty </seealso>
		String Arch {get;}

		/// <summary>
		/// Returns the operating system version.
		/// This method is equivalent to <tt>System.getProperty("os.version")</tt>.
		/// </summary>
		/// <returns> the operating system version.
		/// </returns>
		/// <exception cref="java.lang.SecurityException">
		///     if a security manager exists and its
		///     <code>checkPropertiesAccess</code> method doesn't allow access
		///     to this system property. </exception>
		/// <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		/// <seealso cref= java.lang.System#getProperty </seealso>
		String Version {get;}

		/// <summary>
		/// Returns the number of processors available to the Java virtual machine.
		/// This method is equivalent to the <seealso cref="Runtime#availableProcessors()"/>
		/// method.
		/// <para> This value may change during a particular invocation of
		/// the virtual machine.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the number of processors available to the virtual
		///          machine; never smaller than one. </returns>
		int AvailableProcessors {get;}

		/// <summary>
		/// Returns the system load average for the last minute.
		/// The system load average is the sum of the number of runnable entities
		/// queued to the <seealso cref="#getAvailableProcessors available processors"/>
		/// and the number of runnable entities running on the available processors
		/// averaged over a period of time.
		/// The way in which the load average is calculated is operating system
		/// specific but is typically a damped time-dependent average.
		/// <para>
		/// If the load average is not available, a negative value is returned.
		/// </para>
		/// <para>
		/// This method is designed to provide a hint about the system load
		/// and may be queried frequently.
		/// The load average may be unavailable on some platform where it is
		/// expensive to implement this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the system load average; or a negative value if not available.
		/// 
		/// @since 1.6 </returns>
		double SystemLoadAverage {get;}
	}

}