/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The management interface for a memory pool.  A memory pool
	/// represents the memory resource managed by the Java virtual machine
	/// and is managed by one or more <seealso cref="MemoryManagerMXBean memory managers"/>.
	/// 
	/// <para> A Java virtual machine has one or more instances of the
	/// implementation class of this interface.  An instance
	/// implementing this interface is
	/// an <a href="ManagementFactory.html#MXBean">MXBean</a>
	/// that can be obtained by calling
	/// the <seealso cref="ManagementFactory#getMemoryPoolMXBeans"/> method or
	/// from the {@link ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>} method.
	/// 
	/// </para>
	/// <para>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	/// a memory pool within an <tt>MBeanServer</tt> is:
	/// <blockquote>
	///    {@link ManagementFactory#MEMORY_POOL_MXBEAN_DOMAIN_TYPE
	///    <tt>java.lang:type=MemoryPool</tt>}<tt>,name=</tt><i>pool's name</i>
	/// </blockquote>
	/// 
	/// It can be obtained by calling the
	/// <seealso cref="PlatformManagedObject#getObjectName"/> method.
	/// 
	/// <h3>Memory Type</h3>
	/// </para>
	/// <para>The Java virtual machine has a heap for object allocation and also
	/// maintains non-heap memory for the method area and the Java virtual
	/// machine execution.  The Java virtual machine can have one or more
	/// memory pools.  Each memory pool represents a memory area
	/// of one of the following types:
	/// <ul>
	///   <li><seealso cref="MemoryType#HEAP heap"/></li>
	///   <li><seealso cref="MemoryType#NON_HEAP non-heap"/></li>
	/// </ul>
	/// 
	/// <h3>Memory Usage Monitoring</h3>
	/// 
	/// A memory pool has the following attributes:
	/// <ul>
	///   <li><a href="#Usage">Memory usage</a></li>
	///   <li><a href="#PeakUsage">Peak memory usage</a></li>
	///   <li><a href="#UsageThreshold">Usage Threshold</a></li>
	///   <li><a href="#CollectionThreshold">Collection Usage Threshold</a>
	///       (only supported by some <em>garbage-collected</em> memory pools)</li>
	/// </ul>
	/// 
	/// <h3><a name="Usage">1. Memory Usage</a></h3>
	/// 
	/// The <seealso cref="#getUsage"/> method provides an estimate
	/// of the current usage of a memory pool.
	/// For a garbage-collected memory pool, the amount of used memory
	/// includes the memory occupied by all objects in the pool
	/// including both <em>reachable</em> and <em>unreachable</em> objects.
	/// 
	/// </para>
	/// <para>In general, this method is a lightweight operation for getting
	/// an approximate memory usage.  For some memory pools, for example,
	/// when objects are not packed contiguously, this method may be
	/// an expensive operation that requires some computation to determine
	/// the current memory usage.  An implementation should document when
	/// this is the case.
	/// 
	/// <h3><a name="PeakUsage">2. Peak Memory Usage</a></h3>
	/// 
	/// The Java virtual machine maintains the peak memory usage of a memory
	/// pool since the virtual machine was started or the peak was reset.
	/// The peak memory usage is returned by the <seealso cref="#getPeakUsage"/> method
	/// and reset by calling the <seealso cref="#resetPeakUsage"/> method.
	/// 
	/// <h3><a name="UsageThreshold">3. Usage Threshold</a></h3>
	/// 
	/// Each memory pool has a manageable attribute
	/// called the <i>usage threshold</i> which has a default value supplied
	/// by the Java virtual machine.  The default value is platform-dependent.
	/// The usage threshold can be set via the
	/// <seealso cref="#setUsageThreshold setUsageThreshold"/> method.
	/// If the threshold is set to a positive value, the usage threshold crossing
	/// checking is enabled in this memory pool.
	/// If the usage threshold is set to zero, usage
	/// threshold crossing checking on this memory pool is disabled.
	/// The <seealso cref="MemoryPoolMXBean#isUsageThresholdSupported"/> method can
	/// be used to determine if this functionality is supported.
	/// </para>
	/// <para>
	/// A Java virtual machine performs usage threshold crossing checking on a
	/// memory pool basis at its best appropriate time, typically,
	/// at garbage collection time.
	/// Each memory pool maintains a {@link #getUsageThresholdCount
	/// usage threshold count} that will get incremented
	/// every time when the Java virtual machine
	/// detects that the memory pool usage is crossing the threshold.
	/// </para>
	/// <para>
	/// This manageable usage threshold attribute is designed for monitoring the
	/// increasing trend of memory usage with low overhead.
	/// Usage threshold may not be appropriate for some memory pools.
	/// For example, a generational garbage collector, a common garbage collection
	/// algorithm used in many Java virtual machine implementations,
	/// manages two or more generations segregating objects by age.
	/// Most of the objects are allocated in
	/// the <em>youngest generation</em> (say a nursery memory pool).
	/// The nursery memory pool is designed to be filled up and
	/// collecting the nursery memory pool will free most of its memory space
	/// since it is expected to contain mostly short-lived objects
	/// and mostly are unreachable at garbage collection time.
	/// In this case, it is more appropriate for the nursery memory pool
	/// not to support a usage threshold.  In addition,
	/// if the cost of an object allocation
	/// in one memory pool is very low (for example, just atomic pointer exchange),
	/// the Java virtual machine would probably not support the usage threshold
	/// for that memory pool since the overhead in comparing the usage with
	/// the threshold is higher than the cost of object allocation.
	/// 
	/// </para>
	/// <para>
	/// The memory usage of the system can be monitored using
	/// <a href="#Polling">polling</a> or
	/// <a href="#ThresholdNotification">threshold notification</a> mechanisms.
	/// 
	/// <ol type="a">
	///   <li><a name="Polling"><b>Polling</b></a>
	/// </para>
	///       <para>
	///       An application can continuously monitor its memory usage
	///       by calling either the <seealso cref="#getUsage"/> method for all
	///       memory pools or the <seealso cref="#isUsageThresholdExceeded"/> method
	///       for those memory pools that support a usage threshold.
	///       Below is example code that has a thread dedicated for
	///       task distribution and processing.  At every interval,
	///       it will determine if it should receive and process new tasks based
	///       on its memory usage.  If the memory usage exceeds its usage threshold,
	///       it will redistribute all outstanding tasks to other VMs and
	///       stop receiving new tasks until the memory usage returns
	///       below its usage threshold.
	/// 
	///       <pre>
	///       // Assume the usage threshold is supported for this pool.
	///       // Set the threshold to myThreshold above which no new tasks
	///       // should be taken.
	///       pool.setUsageThreshold(myThreshold);
	///       ....
	/// 
	///       boolean lowMemory = false;
	///       while (true) {
	///          if (pool.isUsageThresholdExceeded()) {
	///              // potential low memory, so redistribute tasks to other VMs
	///              lowMemory = true;
	///              redistributeTasks();
	///              // stop receiving new tasks
	///              stopReceivingTasks();
	///          } else {
	///              if (lowMemory) {
	///                  // resume receiving tasks
	///                  lowMemory = false;
	///                  resumeReceivingTasks();
	///              }
	///              // processing outstanding task
	///              ...
	///          }
	///          // sleep for sometime
	///          try {
	///              Thread.sleep(sometime);
	///          } catch (InterruptedException e) {
	///              ...
	///          }
	///       }
	///       </pre>
	/// 
	/// <hr>
	///       The above example does not differentiate the case where
	///       the memory usage has temporarily dropped below the usage threshold
	///       from the case where the memory usage remains above the threshold
	///       between two iterations.  The usage threshold count returned by
	///       the <seealso cref="#getUsageThresholdCount"/> method
	///       can be used to determine
	///       if the memory usage has returned below the threshold
	///       between two polls.
	/// </para>
	///       <para>
	///       Below shows another example that takes some action if a
	///       memory pool is under low memory and ignores the memory usage
	///       changes during the action processing time.
	/// 
	///       <pre>
	///       // Assume the usage threshold is supported for this pool.
	///       // Set the threshold to myThreshold which determines if
	///       // the application will take some action under low memory condition.
	///       pool.setUsageThreshold(myThreshold);
	/// 
	///       int prevCrossingCount = 0;
	///       while (true) {
	///           // A busy loop to detect when the memory usage
	///           // has exceeded the threshold.
	///           while (!pool.isUsageThresholdExceeded() ||
	///                  pool.getUsageThresholdCount() == prevCrossingCount) {
	///               try {
	///                   Thread.sleep(sometime)
	///               } catch (InterruptException e) {
	///                   ....
	///               }
	///           }
	/// 
	///           // Do some processing such as check for memory usage
	///           // and issue a warning
	///           ....
	/// 
	///           // Gets the current threshold count. The busy loop will then
	///           // ignore any crossing of threshold happens during the processing.
	///           prevCrossingCount = pool.getUsageThresholdCount();
	///       }
	///       </pre><hr>
	///   </li>
	///   <li><a name="ThresholdNotification"><b>Usage Threshold Notifications</b></a>
	/// </para>
	///       <para>
	///       Usage threshold notification will be emitted by <seealso cref="MemoryMXBean"/>.
	///       When the Java virtual machine detects that the memory usage of
	///       a memory pool has reached or exceeded the usage threshold
	///       the virtual machine will trigger the <tt>MemoryMXBean</tt> to emit an
	///       {@link MemoryNotificationInfo#MEMORY_THRESHOLD_EXCEEDED
	///       usage threshold exceeded notification}.
	///       Another usage threshold exceeded notification will not be
	///       generated until the usage has fallen below the threshold and
	///       then exceeded it again.
	/// </para>
	///       <para>
	///       Below is an example code implementing the same logic as the
	///       first example above but using the usage threshold notification
	///       mechanism to detect low memory conditions instead of polling.
	///       In this example code, upon receiving notification, the notification
	///       listener notifies another thread to perform the actual action
	///       such as to redistribute outstanding tasks, stop receiving tasks,
	///       or resume receiving tasks.
	///       The <tt>handleNotification</tt> method should be designed to
	///       do a very minimal amount of work and return without delay to avoid
	///       causing delay in delivering subsequent notifications.  Time-consuming
	///       actions should be performed by a separate thread.
	///       The notification listener may be invoked by multiple threads
	///       concurrently; so the tasks performed by the listener
	///       should be properly synchronized.
	/// 
	///       <pre>
	///       class MyListener implements javax.management.NotificationListener {
	///            public void handleNotification(Notification notification, Object handback)  {
	///                String notifType = notification.getType();
	///                if (notifType.equals(MemoryNotificationInfo.MEMORY_THRESHOLD_EXCEEDED)) {
	///                    // potential low memory, notify another thread
	///                    // to redistribute outstanding tasks to other VMs
	///                    // and stop receiving new tasks.
	///                    lowMemory = true;
	///                    notifyAnotherThread(lowMemory);
	///                }
	///            }
	///       }
	/// 
	///       // Register MyListener with MemoryMXBean
	///       MemoryMXBean mbean = ManagementFactory.getMemoryMXBean();
	///       NotificationEmitter emitter = (NotificationEmitter) mbean;
	///       MyListener listener = new MyListener();
	///       emitter.addNotificationListener(listener, null, null);
	/// 
	///       // Assume this pool supports a usage threshold.
	///       // Set the threshold to myThreshold above which no new tasks
	///       // should be taken.
	///       pool.setUsageThreshold(myThreshold);
	/// 
	///       // Usage threshold detection is enabled and notification will be
	///       // handled by MyListener.  Continue for other processing.
	///       ....
	/// 
	///       </pre>
	/// <hr>
	/// </para>
	///       <para>
	///       There is no guarantee about when the <tt>MemoryMXBean</tt> will emit
	///       a threshold notification and when the notification will be delivered.
	///       When a notification listener is invoked, the memory usage of
	///       the memory pool may have crossed the usage threshold more
	///       than once.
	///       The <seealso cref="MemoryNotificationInfo#getCount"/> method returns the number
	///       of times that the memory usage has crossed the usage threshold
	///       at the point in time when the notification was constructed.
	///       It can be compared with the current usage threshold count returned
	///       by the <seealso cref="#getUsageThresholdCount"/> method to determine if
	///       such situation has occurred.
	///   </li>
	/// </ol>
	/// 
	/// <h3><a name="CollectionThreshold">4. Collection Usage Threshold</a></h3>
	/// 
	/// Collection usage threshold is a manageable attribute only applicable
	/// to some garbage-collected memory pools.
	/// After a Java virtual machine has expended effort in reclaiming memory
	/// space by recycling unused objects in a memory pool at garbage collection
	/// time, some number of bytes in the memory pools that are garbaged
	/// collected will still be in use.  The collection usage threshold
	/// allows a value to be set for this number of bytes such
	/// that if the threshold is exceeded,
	/// a {@link MemoryNotificationInfo#MEMORY_THRESHOLD_EXCEEDED
	/// collection usage threshold exceeded notification}
	/// will be emitted by the <seealso cref="MemoryMXBean"/>.
	/// In addition, the {@link #getCollectionUsageThresholdCount
	/// collection usage threshold count} will then be incremented.
	/// 
	/// </para>
	/// <para>
	/// The <seealso cref="MemoryPoolMXBean#isCollectionUsageThresholdSupported"/> method can
	/// be used to determine if this functionality is supported.
	/// 
	/// </para>
	/// <para>
	/// A Java virtual machine performs collection usage threshold checking
	/// on a memory pool basis.  This checking is enabled if the collection
	/// usage threshold is set to a positive value.
	/// If the collection usage threshold is set to zero, this checking
	/// is disabled on this memory pool.  Default value is zero.
	/// The Java virtual machine performs the collection usage threshold
	/// checking at garbage collection time.
	/// 
	/// </para>
	/// <para>
	/// Some garbage-collected memory pools may
	/// choose not to support the collection usage threshold.  For example,
	/// a memory pool is only managed by a continuous concurrent garbage
	/// collector.  Objects can be allocated in this memory pool by some thread
	/// while the unused objects are reclaimed by the concurrent garbage
	/// collector simultaneously.  Unless there is a well-defined
	/// garbage collection time which is the best appropriate time
	/// to check the memory usage, the collection usage threshold should not
	/// be supported.
	/// 
	/// </para>
	/// <para>
	/// The collection usage threshold is designed for monitoring the memory usage
	/// after the Java virtual machine has expended effort in reclaiming
	/// memory space.  The collection usage could also be monitored
	/// by the polling and threshold notification mechanism
	/// described above for the <a href="#UsageThreshold">usage threshold</a>
	/// in a similar fashion.
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
	public interface MemoryPoolMXBean : PlatformManagedObject
	{
		/// <summary>
		/// Returns the name representing this memory pool.
		/// </summary>
		/// <returns> the name of this memory pool. </returns>
		String Name {get;}

		/// <summary>
		/// Returns the type of this memory pool.
		/// 
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>MemoryType</tt> is <tt>String</tt>
		/// and the value is the name of the <tt>MemoryType</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the type of this memory pool. </returns>
		MemoryType Type {get;}

		/// <summary>
		/// Returns an estimate of the memory usage of this memory pool.
		/// This method returns <tt>null</tt>
		/// if this memory pool is not valid (i.e. no longer exists).
		/// 
		/// <para>
		/// This method requests the Java virtual machine to make
		/// a best-effort estimate of the current memory usage of this
		/// memory pool. For some memory pools, this method may be an
		/// expensive operation that requires some computation to determine
		/// the estimate.  An implementation should document when
		/// this is the case.
		/// 
		/// </para>
		/// <para>This method is designed for use in monitoring system
		/// memory usage and detecting low memory condition.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>MemoryUsage</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in
		/// <seealso cref="MemoryUsage#from MemoryUsage"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <seealso cref="MemoryUsage"/> object; or <tt>null</tt> if
		/// this pool not valid. </returns>
		MemoryUsage Usage {get;}

		/// <summary>
		/// Returns the peak memory usage of this memory pool since the
		/// Java virtual machine was started or since the peak was reset.
		/// This method returns <tt>null</tt>
		/// if this memory pool is not valid (i.e. no longer exists).
		/// 
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>MemoryUsage</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in
		/// <seealso cref="MemoryUsage#from MemoryUsage"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <seealso cref="MemoryUsage"/> object representing the peak
		/// memory usage; or <tt>null</tt> if this pool is not valid.
		///  </returns>
		MemoryUsage PeakUsage {get;}

		/// <summary>
		/// Resets the peak memory usage statistic of this memory pool
		/// to the current memory usage.
		/// </summary>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("control"). </exception>
		void ResetPeakUsage();

		/// <summary>
		/// Tests if this memory pool is valid in the Java virtual
		/// machine.  A memory pool becomes invalid once the Java virtual
		/// machine removes it from the memory system.
		/// </summary>
		/// <returns> <tt>true</tt> if the memory pool is valid in the running
		///              Java virtual machine;
		///         <tt>false</tt> otherwise. </returns>
		bool Valid {get;}

		/// <summary>
		/// Returns the name of memory managers that manages this memory pool.
		/// Each memory pool will be managed by at least one memory manager.
		/// </summary>
		/// <returns> an array of <tt>String</tt> objects, each is the name of
		/// a memory manager managing this memory pool. </returns>
		String[] MemoryManagerNames {get;}

		/// <summary>
		/// Returns the usage threshold value of this memory pool in bytes.
		/// Each memory pool has a platform-dependent default threshold value.
		/// The current usage threshold can be changed via the
		/// <seealso cref="#setUsageThreshold setUsageThreshold"/> method.
		/// </summary>
		/// <returns> the usage threshold value of this memory pool in bytes.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		///         does not support a usage threshold.
		/// </exception>
		/// <seealso cref= #isUsageThresholdSupported </seealso>
		long UsageThreshold {get;set;}


		/// <summary>
		/// Tests if the memory usage of this memory pool
		/// reaches or exceeds its usage threshold value.
		/// </summary>
		/// <returns> <tt>true</tt> if the memory usage of
		/// this memory pool reaches or exceeds the threshold value;
		/// <tt>false</tt> otherwise.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		///         does not support a usage threshold. </exception>
		bool UsageThresholdExceeded {get;}

		/// <summary>
		/// Returns the number of times that the memory usage has crossed
		/// the usage threshold.
		/// </summary>
		/// <returns> the number of times that the memory usage
		/// has crossed its usage threshold value.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		/// does not support a usage threshold. </exception>
		long UsageThresholdCount {get;}

		/// <summary>
		/// Tests if this memory pool supports usage threshold.
		/// </summary>
		/// <returns> <tt>true</tt> if this memory pool supports usage threshold;
		/// <tt>false</tt> otherwise. </returns>
		bool UsageThresholdSupported {get;}

		/// <summary>
		/// Returns the collection usage threshold value of this memory pool
		/// in bytes.  The default value is zero. The collection usage
		/// threshold can be changed via the
		/// <seealso cref="#setCollectionUsageThreshold setCollectionUsageThreshold"/> method.
		/// </summary>
		/// <returns> the collection usage threshold of this memory pool in bytes.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		///         does not support a collection usage threshold.
		/// </exception>
		/// <seealso cref= #isCollectionUsageThresholdSupported </seealso>
		long CollectionUsageThreshold {get;set;}


		/// <summary>
		/// Tests if the memory usage of this memory pool after
		/// the most recent collection on which the Java virtual
		/// machine has expended effort has reached or
		/// exceeded its collection usage threshold.
		/// This method does not request the Java virtual
		/// machine to perform any garbage collection other than its normal
		/// automatic memory management.
		/// </summary>
		/// <returns> <tt>true</tt> if the memory usage of this memory pool
		/// reaches or exceeds the collection usage threshold value
		/// in the most recent collection;
		/// <tt>false</tt> otherwise.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		///         does not support a usage threshold. </exception>
		bool CollectionUsageThresholdExceeded {get;}

		/// <summary>
		/// Returns the number of times that the Java virtual machine
		/// has detected that the memory usage has reached or
		/// exceeded the collection usage threshold.
		/// </summary>
		/// <returns> the number of times that the memory
		/// usage has reached or exceeded the collection usage threshold.
		/// </returns>
		/// <exception cref="UnsupportedOperationException"> if this memory pool
		///         does not support a collection usage threshold.
		/// </exception>
		/// <seealso cref= #isCollectionUsageThresholdSupported </seealso>
		long CollectionUsageThresholdCount {get;}

		/// <summary>
		/// Returns the memory usage after the Java virtual machine
		/// most recently expended effort in recycling unused objects
		/// in this memory pool.
		/// This method does not request the Java virtual
		/// machine to perform any garbage collection other than its normal
		/// automatic memory management.
		/// This method returns <tt>null</tt> if the Java virtual
		/// machine does not support this method.
		/// 
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>MemoryUsage</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in
		/// <seealso cref="MemoryUsage#from MemoryUsage"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <seealso cref="MemoryUsage"/> representing the memory usage of
		/// this memory pool after the Java virtual machine most recently
		/// expended effort in recycling unused objects;
		/// <tt>null</tt> if this method is not supported. </returns>
		MemoryUsage CollectionUsage {get;}

		/// <summary>
		/// Tests if this memory pool supports a collection usage threshold.
		/// </summary>
		/// <returns> <tt>true</tt> if this memory pool supports the
		/// collection usage threshold; <tt>false</tt> otherwise. </returns>
		bool CollectionUsageThresholdSupported {get;}
	}

}