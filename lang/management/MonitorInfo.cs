/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using MonitorInfoCompositeData = sun.management.MonitorInfoCompositeData;

	/// <summary>
	/// Information about an object monitor lock.  An object monitor is locked
	/// when entering a synchronization block or method on that object.
	/// 
	/// <h3>MXBean Mapping</h3>
	/// <tt>MonitorInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	/// with attributes as specified in
	/// the <seealso cref="#from from"/> method.
	/// 
	/// @author  Mandy Chung
	/// @since   1.6
	/// </summary>
	public class MonitorInfo : LockInfo
	{

		private int StackDepth;
		private StackTraceElement StackFrame;

		/// <summary>
		/// Construct a <tt>MonitorInfo</tt> object.
		/// </summary>
		/// <param name="className"> the fully qualified name of the class of the lock object. </param>
		/// <param name="identityHashCode"> the {@link System#identityHashCode
		///                         identity hash code} of the lock object. </param>
		/// <param name="stackDepth"> the depth in the stack trace where the object monitor
		///                   was locked. </param>
		/// <param name="stackFrame"> the stack frame that locked the object monitor. </param>
		/// <exception cref="IllegalArgumentException"> if
		///    <tt>stackDepth</tt> &ge; 0 but <tt>stackFrame</tt> is <tt>null</tt>,
		///    or <tt>stackDepth</tt> &lt; 0 but <tt>stackFrame</tt> is not
		///       <tt>null</tt>. </exception>
		public MonitorInfo(String className, int identityHashCode, int stackDepth, StackTraceElement stackFrame) : base(className, identityHashCode)
		{
			if (stackDepth >= 0 && stackFrame == null)
			{
				throw new IllegalArgumentException("Parameter stackDepth is " + stackDepth + " but stackFrame is null");
			}
			if (stackDepth < 0 && stackFrame != null)
			{
				throw new IllegalArgumentException("Parameter stackDepth is " + stackDepth + " but stackFrame is not null");
			}
			this.StackDepth = stackDepth;
			this.StackFrame = stackFrame;
		}

		/// <summary>
		/// Returns the depth in the stack trace where the object monitor
		/// was locked.  The depth is the index to the <tt>StackTraceElement</tt>
		/// array returned in the <seealso cref="ThreadInfo#getStackTrace"/> method.
		/// </summary>
		/// <returns> the depth in the stack trace where the object monitor
		///         was locked, or a negative number if not available. </returns>
		public virtual int LockedStackDepth
		{
			get
			{
				return StackDepth;
			}
		}

		/// <summary>
		/// Returns the stack frame that locked the object monitor.
		/// </summary>
		/// <returns> <tt>StackTraceElement</tt> that locked the object monitor,
		///         or <tt>null</tt> if not available. </returns>
		public virtual StackTraceElement LockedStackFrame
		{
			get
			{
				return StackFrame;
			}
		}

		/// <summary>
		/// Returns a <tt>MonitorInfo</tt> object represented by the
		/// given <tt>CompositeData</tt>.
		/// The given <tt>CompositeData</tt> must contain the following attributes
		/// as well as the attributes specified in the
		/// <a href="LockInfo.html#MappedType">
		/// mapped type</a> for the <seealso cref="LockInfo"/> class:
		/// <blockquote>
		/// <table border summary="The attributes and their types the given CompositeData contains">
		/// <tr>
		///   <th align=left>Attribute Name</th>
		///   <th align=left>Type</th>
		/// </tr>
		/// <tr>
		///   <td>lockedStackFrame</td>
		///   <td><tt>CompositeData as specified in the
		///       <a href="ThreadInfo.html#StackTrace">stackTrace</a>
		///       attribute defined in the {@link ThreadInfo#from
		///       ThreadInfo.from} method.
		///       </tt></td>
		/// </tr>
		/// <tr>
		///   <td>lockedStackDepth</td>
		///   <td><tt>java.lang.Integer</tt></td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// </summary>
		/// <param name="cd"> <tt>CompositeData</tt> representing a <tt>MonitorInfo</tt>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		///   represent a <tt>MonitorInfo</tt> with the attributes described
		///   above.
		/// </exception>
		/// <returns> a <tt>MonitorInfo</tt> object represented
		///         by <tt>cd</tt> if <tt>cd</tt> is not <tt>null</tt>;
		///         <tt>null</tt> otherwise. </returns>
		public static MonitorInfo From(CompositeData cd)
		{
			if (cd == null)
			{
				return null;
			}

			if (cd is MonitorInfoCompositeData)
			{
				return ((MonitorInfoCompositeData) cd).MonitorInfo;
			}
			else
			{
				MonitorInfoCompositeData.validateCompositeData(cd);
				String className = MonitorInfoCompositeData.getClassName(cd);
				int identityHashCode = MonitorInfoCompositeData.getIdentityHashCode(cd);
				int stackDepth = MonitorInfoCompositeData.getLockedStackDepth(cd);
				StackTraceElement stackFrame = MonitorInfoCompositeData.getLockedStackFrame(cd);
				return new MonitorInfo(className, identityHashCode, stackDepth, stackFrame);
			}
		}

	}

}