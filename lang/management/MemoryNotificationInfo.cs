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
	using MemoryNotifInfoCompositeData = sun.management.MemoryNotifInfoCompositeData;

	/// <summary>
	/// The information about a memory notification.
	/// 
	/// <para>
	/// A memory notification is emitted by <seealso cref="MemoryMXBean"/>
	/// when the Java virtual machine detects that the memory usage
	/// of a memory pool is exceeding a threshold value.
	/// The notification emitted will contain the memory notification
	/// information about the detected condition:
	/// <ul>
	///   <li>The name of the memory pool.</li>
	///   <li>The memory usage of the memory pool when the notification
	///       was constructed.</li>
	///   <li>The number of times that the memory usage has crossed
	///       a threshold when the notification was constructed.
	///       For usage threshold notifications, this count will be the
	///       {@link MemoryPoolMXBean#getUsageThresholdCount usage threshold
	///       count}.  For collection threshold notifications,
	///       this count will be the
	///       {@link MemoryPoolMXBean#getCollectionUsageThresholdCount
	///       collection usage threshold count}.
	///       </li>
	/// </ul>
	/// 
	/// </para>
	/// <para>
	/// A <seealso cref="CompositeData CompositeData"/> representing
	/// the <tt>MemoryNotificationInfo</tt> object
	/// is stored in the
	/// <seealso cref="javax.management.Notification#setUserData user data"/>
	/// of a <seealso cref="javax.management.Notification notification"/>.
	/// The <seealso cref="#from from"/> method is provided to convert from
	/// a <tt>CompositeData</tt> to a <tt>MemoryNotificationInfo</tt>
	/// object. For example:
	/// 
	/// <blockquote><pre>
	///      Notification notif;
	/// 
	///      // receive the notification emitted by MemoryMXBean and set to notif
	///      ...
	/// 
	///      String notifType = notif.getType();
	///      if (notifType.equals(MemoryNotificationInfo.MEMORY_THRESHOLD_EXCEEDED) ||
	///          notifType.equals(MemoryNotificationInfo.MEMORY_COLLECTION_THRESHOLD_EXCEEDED)) {
	///          // retrieve the memory notification information
	///          CompositeData cd = (CompositeData) notif.getUserData();
	///          MemoryNotificationInfo info = MemoryNotificationInfo.from(cd);
	///          ....
	///      }
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// The types of notifications emitted by <tt>MemoryMXBean</tt> are:
	/// <ul>
	///   <li>A {@link #MEMORY_THRESHOLD_EXCEEDED
	///       usage threshold exceeded notification}.
	///       <br>This notification will be emitted when
	///       the memory usage of a memory pool is increased and has reached
	///       or exceeded its
	///       <a href="MemoryPoolMXBean.html#UsageThreshold"> usage threshold</a> value.
	///       Subsequent crossing of the usage threshold value does not cause
	///       further notification until the memory usage has returned
	///       to become less than the usage threshold value.
	/// </para>
	///       <para></li>
	///   <li>A {@link #MEMORY_COLLECTION_THRESHOLD_EXCEEDED
	///       collection usage threshold exceeded notification}.
	///       <br>This notification will be emitted when
	///       the memory usage of a memory pool is greater than or equal to its
	///       <a href="MemoryPoolMXBean.html#CollectionThreshold">
	///       collection usage threshold</a> after the Java virtual machine
	///       has expended effort in recycling unused objects in that
	///       memory pool.</li>
	/// </ul>
	/// 
	/// @author  Mandy Chung
	/// @since   1.5
	/// 
	/// </para>
	/// </summary>
	public class MemoryNotificationInfo
	{
		private readonly String PoolName_Renamed;
		private readonly MemoryUsage Usage_Renamed;
		private readonly long Count_Renamed;

		/// <summary>
		/// Notification type denoting that
		/// the memory usage of a memory pool has
		/// reached or exceeded its
		/// <a href="MemoryPoolMXBean.html#UsageThreshold"> usage threshold</a> value.
		/// This notification is emitted by <seealso cref="MemoryMXBean"/>.
		/// Subsequent crossing of the usage threshold value does not cause
		/// further notification until the memory usage has returned
		/// to become less than the usage threshold value.
		/// The value of this notification type is
		/// <tt>java.management.memory.threshold.exceeded</tt>.
		/// </summary>
		public const String MEMORY_THRESHOLD_EXCEEDED = "java.management.memory.threshold.exceeded";

		/// <summary>
		/// Notification type denoting that
		/// the memory usage of a memory pool is greater than or equal to its
		/// <a href="MemoryPoolMXBean.html#CollectionThreshold">
		/// collection usage threshold</a> after the Java virtual machine
		/// has expended effort in recycling unused objects in that
		/// memory pool.
		/// This notification is emitted by <seealso cref="MemoryMXBean"/>.
		/// The value of this notification type is
		/// <tt>java.management.memory.collection.threshold.exceeded</tt>.
		/// </summary>
		public const String MEMORY_COLLECTION_THRESHOLD_EXCEEDED = "java.management.memory.collection.threshold.exceeded";

		/// <summary>
		/// Constructs a <tt>MemoryNotificationInfo</tt> object.
		/// </summary>
		/// <param name="poolName"> The name of the memory pool which triggers this notification. </param>
		/// <param name="usage"> Memory usage of the memory pool. </param>
		/// <param name="count"> The threshold crossing count. </param>
		public MemoryNotificationInfo(String poolName, MemoryUsage usage, long count)
		{
			if (poolName == null)
			{
				throw new NullPointerException("Null poolName");
			}
			if (usage == null)
			{
				throw new NullPointerException("Null usage");
			}

			this.PoolName_Renamed = poolName;
			this.Usage_Renamed = usage;
			this.Count_Renamed = count;
		}

		internal MemoryNotificationInfo(CompositeData cd)
		{
			MemoryNotifInfoCompositeData.validateCompositeData(cd);

			this.PoolName_Renamed = MemoryNotifInfoCompositeData.getPoolName(cd);
			this.Usage_Renamed = MemoryNotifInfoCompositeData.getUsage(cd);
			this.Count_Renamed = MemoryNotifInfoCompositeData.getCount(cd);
		}

		/// <summary>
		/// Returns the name of the memory pool that triggers this notification.
		/// The memory pool usage has crossed a threshold.
		/// </summary>
		/// <returns> the name of the memory pool that triggers this notification. </returns>
		public virtual String PoolName
		{
			get
			{
				return PoolName_Renamed;
			}
		}

		/// <summary>
		/// Returns the memory usage of the memory pool
		/// when this notification was constructed.
		/// </summary>
		/// <returns> the memory usage of the memory pool
		/// when this notification was constructed. </returns>
		public virtual MemoryUsage Usage
		{
			get
			{
				return Usage_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of times that the memory usage has crossed
		/// a threshold when the notification was constructed.
		/// For usage threshold notifications, this count will be the
		/// {@link MemoryPoolMXBean#getUsageThresholdCount threshold
		/// count}.  For collection threshold notifications,
		/// this count will be the
		/// {@link MemoryPoolMXBean#getCollectionUsageThresholdCount
		/// collection usage threshold count}.
		/// </summary>
		/// <returns> the number of times that the memory usage has crossed
		/// a threshold when the notification was constructed. </returns>
		public virtual long Count
		{
			get
			{
				return Count_Renamed;
			}
		}

		/// <summary>
		/// Returns a <tt>MemoryNotificationInfo</tt> object represented by the
		/// given <tt>CompositeData</tt>.
		/// The given <tt>CompositeData</tt> must contain
		/// the following attributes:
		/// <blockquote>
		/// <table border summary="The attributes and the types the given CompositeData contains">
		/// <tr>
		///   <th align=left>Attribute Name</th>
		///   <th align=left>Type</th>
		/// </tr>
		/// <tr>
		///   <td>poolName</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td>usage</td>
		///   <td><tt>javax.management.openmbean.CompositeData</tt></td>
		/// </tr>
		/// <tr>
		///   <td>count</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// </summary>
		/// <param name="cd"> <tt>CompositeData</tt> representing a
		///           <tt>MemoryNotificationInfo</tt>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		///   represent a <tt>MemoryNotificationInfo</tt> object.
		/// </exception>
		/// <returns> a <tt>MemoryNotificationInfo</tt> object represented
		///         by <tt>cd</tt> if <tt>cd</tt> is not <tt>null</tt>;
		///         <tt>null</tt> otherwise. </returns>
		public static MemoryNotificationInfo From(CompositeData cd)
		{
			if (cd == null)
			{
				return null;
			}

			if (cd is MemoryNotifInfoCompositeData)
			{
				return ((MemoryNotifInfoCompositeData) cd).MemoryNotifInfo;
			}
			else
			{
				return new MemoryNotificationInfo(cd);
			}
		}
	}

}