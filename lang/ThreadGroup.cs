using System;
using System.Threading;

/*
 * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	using VM = sun.misc.VM;

	/// <summary>
	/// A thread group represents a set of threads. In addition, a thread
	/// group can also include other thread groups. The thread groups form
	/// a tree in which every thread group except the initial thread group
	/// has a parent.
	/// <para>
	/// A thread is allowed to access information about its own thread
	/// group, but not to access information about its thread group's
	/// parent thread group or any other thread groups.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	/* The locking strategy for this code is to try to lock only one level of the
	 * tree wherever possible, but otherwise to lock from the bottom up.
	 * That is, from child thread groups to parents.
	 * This has the advantage of limiting the number of locks that need to be held
	 * and in particular avoids having to grab the lock for the root thread group,
	 * (or a global lock) which would be a source of contention on a
	 * multi-processor system with many thread groups.
	 * This policy often leads to taking a snapshot of the state of a thread group
	 * and working off of that snapshot, rather than holding the thread group locked
	 * while we work on the children.
	 */
	public class ThreadGroup : Thread.UncaughtExceptionHandler
	{
		private readonly ThreadGroup Parent_Renamed;
		internal String Name_Renamed;
		internal int MaxPriority_Renamed;
		internal bool Destroyed_Renamed;
		internal bool Daemon_Renamed;
		internal bool VmAllowSuspension;

		internal int NUnstartedThreads = 0;
		internal int Nthreads;
		internal Thread[] Threads;

		internal int Ngroups;
		internal ThreadGroup[] Groups;

		/// <summary>
		/// Creates an empty Thread group that is not in any Thread group.
		/// This method is used to create the system Thread group.
		/// </summary>
		private ThreadGroup() // called from C code
		{
			this.Name_Renamed = "system";
			this.MaxPriority_Renamed = Thread.MAX_PRIORITY;
			this.Parent_Renamed = null;
		}

		/// <summary>
		/// Constructs a new thread group. The parent of this new group is
		/// the thread group of the currently running thread.
		/// <para>
		/// The <code>checkAccess</code> method of the parent thread group is
		/// called with no arguments; this may result in a security exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the name of the new thread group. </param>
		/// <exception cref="SecurityException">  if the current thread cannot create a
		///               thread in the specified thread group. </exception>
		/// <seealso cref=     java.lang.ThreadGroup#checkAccess()
		/// @since   JDK1.0 </seealso>
		public ThreadGroup(String name) : this(Thread.CurrentThread.ThreadGroup, name)
		{
		}

		/// <summary>
		/// Creates a new thread group. The parent of this new group is the
		/// specified thread group.
		/// <para>
		/// The <code>checkAccess</code> method of the parent thread group is
		/// called with no arguments; this may result in a security exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the parent thread group. </param>
		/// <param name="name">     the name of the new thread group. </param>
		/// <exception cref="NullPointerException">  if the thread group argument is
		///               <code>null</code>. </exception>
		/// <exception cref="SecurityException">  if the current thread cannot create a
		///               thread in the specified thread group. </exception>
		/// <seealso cref=     java.lang.SecurityException </seealso>
		/// <seealso cref=     java.lang.ThreadGroup#checkAccess()
		/// @since   JDK1.0 </seealso>
		public ThreadGroup(ThreadGroup parent, String name) : this(CheckParentAccess(parent), parent, name)
		{
		}

		private ThreadGroup(Void unused, ThreadGroup parent, String name)
		{
			this.Name_Renamed = name;
			this.MaxPriority_Renamed = parent.MaxPriority_Renamed;
			this.Daemon_Renamed = parent.Daemon_Renamed;
			this.VmAllowSuspension = parent.VmAllowSuspension;
			this.Parent_Renamed = parent;
			parent.Add(this);
		}

		/*
		 * @throws  NullPointerException  if the parent argument is {@code null}
		 * @throws  SecurityException     if the current thread cannot create a
		 *                                thread in the specified thread group.
		 */
		private static Void CheckParentAccess(ThreadGroup parent)
		{
			parent.CheckAccess();
			return null;
		}

		/// <summary>
		/// Returns the name of this thread group.
		/// </summary>
		/// <returns>  the name of this thread group.
		/// @since   JDK1.0 </returns>
		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns the parent of this thread group.
		/// <para>
		/// First, if the parent is not <code>null</code>, the
		/// <code>checkAccess</code> method of the parent thread group is
		/// called with no arguments; this may result in a security exception.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the parent of this thread group. The top-level thread group
		///          is the only thread group whose parent is <code>null</code>. </returns>
		/// <exception cref="SecurityException">  if the current thread cannot modify
		///               this thread group. </exception>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess() </seealso>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.RuntimePermission
		/// @since   JDK1.0 </seealso>
		public ThreadGroup Parent
		{
			get
			{
				if (Parent_Renamed != null)
				{
					Parent_Renamed.CheckAccess();
				}
				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Returns the maximum priority of this thread group. Threads that are
		/// part of this group cannot have a higher priority than the maximum
		/// priority.
		/// </summary>
		/// <returns>  the maximum priority that a thread in this thread group
		///          can have. </returns>
		/// <seealso cref=     #setMaxPriority
		/// @since   JDK1.0 </seealso>
		public int MaxPriority
		{
			get
			{
				return MaxPriority_Renamed;
			}
			set
			{
				int ngroupsSnapshot;
				ThreadGroup[] groupsSnapshot;
				lock (this)
				{
					CheckAccess();
					if (value < Thread.MIN_PRIORITY || value > Thread.MAX_PRIORITY)
					{
						return;
					}
					MaxPriority_Renamed = (Parent_Renamed != null) ? System.Math.Min(value, Parent_Renamed.MaxPriority_Renamed) : value;
					ngroupsSnapshot = Ngroups;
					if (Groups != null)
					{
						groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
					}
					else
					{
						groupsSnapshot = null;
					}
				}
				for (int i = 0 ; i < ngroupsSnapshot ; i++)
				{
					groupsSnapshot[i].MaxPriority = value;
				}
			}
		}

		/// <summary>
		/// Tests if this thread group is a daemon thread group. A
		/// daemon thread group is automatically destroyed when its last
		/// thread is stopped or its last thread group is destroyed.
		/// </summary>
		/// <returns>  <code>true</code> if this thread group is a daemon thread group;
		///          <code>false</code> otherwise.
		/// @since   JDK1.0 </returns>
		public bool Daemon
		{
			get
			{
				return Daemon_Renamed;
			}
			set
			{
				CheckAccess();
				this.Daemon_Renamed = value;
			}
		}

		/// <summary>
		/// Tests if this thread group has been destroyed.
		/// </summary>
		/// <returns>  true if this object is destroyed
		/// @since   JDK1.1 </returns>
		public virtual bool Destroyed
		{
			get
			{
				lock (this)
				{
					return Destroyed_Renamed;
				}
			}
		}



		/// <summary>
		/// Tests if this thread group is either the thread group
		/// argument or one of its ancestor thread groups.
		/// </summary>
		/// <param name="g">   a thread group. </param>
		/// <returns>  <code>true</code> if this thread group is the thread group
		///          argument or one of its ancestor thread groups;
		///          <code>false</code> otherwise.
		/// @since   JDK1.0 </returns>
		public bool ParentOf(ThreadGroup g)
		{
			for (; g != null ; g = g.Parent_Renamed)
			{
				if (g == this)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines if the currently running thread has permission to
		/// modify this thread group.
		/// <para>
		/// If there is a security manager, its <code>checkAccess</code> method
		/// is called with this thread group as its argument. This may result
		/// in throwing a <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed to
		///               access this thread group. </exception>
		/// <seealso cref=        java.lang.SecurityManager#checkAccess(java.lang.ThreadGroup)
		/// @since      JDK1.0 </seealso>
		public void CheckAccess()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckAccess(this);
			}
		}

		/// <summary>
		/// Returns an estimate of the number of active threads in this thread
		/// group and its subgroups. Recursively iterates over all subgroups in
		/// this thread group.
		/// 
		/// <para> The value returned is only an estimate because the number of
		/// threads may change dynamically while this method traverses internal
		/// data structures, and might be affected by the presence of certain
		/// system threads. This method is intended primarily for debugging
		/// and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an estimate of the number of active threads in this thread
		///          group and in any other thread group that has this thread
		///          group as an ancestor
		/// 
		/// @since   JDK1.0 </returns>
		public virtual int ActiveCount()
		{
			int result;
			// Snapshot sub-group data so we don't hold this lock
			// while our children are computing.
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return 0;
				}
				result = Nthreads;
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				result += groupsSnapshot[i].ActiveCount();
			}
			return result;
		}

		/// <summary>
		/// Copies into the specified array every active thread in this
		/// thread group and its subgroups.
		/// 
		/// <para> An invocation of this method behaves in exactly the same
		/// way as the invocation
		/// 
		/// <blockquote>
		/// <seealso cref="#enumerate(Thread[], boolean) enumerate"/>{@code (list, true)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="list">
		///         an array into which to put the list of threads
		/// </param>
		/// <returns>  the number of threads put into the array
		/// </returns>
		/// <exception cref="SecurityException">
		///          if <seealso cref="#checkAccess checkAccess"/> determines that
		///          the current thread cannot access this thread group
		/// 
		/// @since   JDK1.0 </exception>
		public virtual int Enumerate(Thread[] list)
		{
			CheckAccess();
			return Enumerate(list, 0, true);
		}

		/// <summary>
		/// Copies into the specified array every active thread in this
		/// thread group. If {@code recurse} is {@code true},
		/// this method recursively enumerates all subgroups of this
		/// thread group and references to every active thread in these
		/// subgroups are also included. If the array is too short to
		/// hold all the threads, the extra threads are silently ignored.
		/// 
		/// <para> An application might use the <seealso cref="#activeCount activeCount"/>
		/// method to get an estimate of how big the array should be, however
		/// <i>if the array is too short to hold all the threads, the extra threads
		/// are silently ignored.</i>  If it is critical to obtain every active
		/// thread in this thread group, the caller should verify that the returned
		/// int value is strictly less than the length of {@code list}.
		/// 
		/// </para>
		/// <para> Due to the inherent race condition in this method, it is recommended
		/// that the method only be used for debugging and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="list">
		///         an array into which to put the list of threads
		/// </param>
		/// <param name="recurse">
		///         if {@code true}, recursively enumerate all subgroups of this
		///         thread group
		/// </param>
		/// <returns>  the number of threads put into the array
		/// </returns>
		/// <exception cref="SecurityException">
		///          if <seealso cref="#checkAccess checkAccess"/> determines that
		///          the current thread cannot access this thread group
		/// 
		/// @since   JDK1.0 </exception>
		public virtual int Enumerate(Thread[] list, bool recurse)
		{
			CheckAccess();
			return Enumerate(list, 0, recurse);
		}

		private int Enumerate(Thread[] list, int n, bool recurse)
		{
			int ngroupsSnapshot = 0;
			ThreadGroup[] groupsSnapshot = null;
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return 0;
				}
				int nt = Nthreads;
				if (nt > list.Length - n)
				{
					nt = list.Length - n;
				}
				for (int i = 0; i < nt; i++)
				{
					if (Threads[i].Alive)
					{
						list[n++] = Threads[i];
					}
				}
				if (recurse)
				{
					ngroupsSnapshot = Ngroups;
					if (Groups != null)
					{
						groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
					}
					else
					{
						groupsSnapshot = null;
					}
				}
			}
			if (recurse)
			{
				for (int i = 0 ; i < ngroupsSnapshot ; i++)
				{
					n = groupsSnapshot[i].Enumerate(list, n, true);
				}
			}
			return n;
		}

		/// <summary>
		/// Returns an estimate of the number of active groups in this
		/// thread group and its subgroups. Recursively iterates over
		/// all subgroups in this thread group.
		/// 
		/// <para> The value returned is only an estimate because the number of
		/// thread groups may change dynamically while this method traverses
		/// internal data structures. This method is intended primarily for
		/// debugging and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the number of active thread groups with this thread group as
		///          an ancestor
		/// 
		/// @since   JDK1.0 </returns>
		public virtual int ActiveGroupCount()
		{
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return 0;
				}
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
			}
			int n = ngroupsSnapshot;
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				n += groupsSnapshot[i].ActiveGroupCount();
			}
			return n;
		}

		/// <summary>
		/// Copies into the specified array references to every active
		/// subgroup in this thread group and its subgroups.
		/// 
		/// <para> An invocation of this method behaves in exactly the same
		/// way as the invocation
		/// 
		/// <blockquote>
		/// <seealso cref="#enumerate(ThreadGroup[], boolean) enumerate"/>{@code (list, true)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="list">
		///         an array into which to put the list of thread groups
		/// </param>
		/// <returns>  the number of thread groups put into the array
		/// </returns>
		/// <exception cref="SecurityException">
		///          if <seealso cref="#checkAccess checkAccess"/> determines that
		///          the current thread cannot access this thread group
		/// 
		/// @since   JDK1.0 </exception>
		public virtual int Enumerate(ThreadGroup[] list)
		{
			CheckAccess();
			return Enumerate(list, 0, true);
		}

		/// <summary>
		/// Copies into the specified array references to every active
		/// subgroup in this thread group. If {@code recurse} is
		/// {@code true}, this method recursively enumerates all subgroups of this
		/// thread group and references to every active thread group in these
		/// subgroups are also included.
		/// 
		/// <para> An application might use the
		/// <seealso cref="#activeGroupCount activeGroupCount"/> method to
		/// get an estimate of how big the array should be, however <i>if the
		/// array is too short to hold all the thread groups, the extra thread
		/// groups are silently ignored.</i>  If it is critical to obtain every
		/// active subgroup in this thread group, the caller should verify that
		/// the returned int value is strictly less than the length of
		/// {@code list}.
		/// 
		/// </para>
		/// <para> Due to the inherent race condition in this method, it is recommended
		/// that the method only be used for debugging and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="list">
		///         an array into which to put the list of thread groups
		/// </param>
		/// <param name="recurse">
		///         if {@code true}, recursively enumerate all subgroups
		/// </param>
		/// <returns>  the number of thread groups put into the array
		/// </returns>
		/// <exception cref="SecurityException">
		///          if <seealso cref="#checkAccess checkAccess"/> determines that
		///          the current thread cannot access this thread group
		/// 
		/// @since   JDK1.0 </exception>
		public virtual int Enumerate(ThreadGroup[] list, bool recurse)
		{
			CheckAccess();
			return Enumerate(list, 0, recurse);
		}

		private int Enumerate(ThreadGroup[] list, int n, bool recurse)
		{
			int ngroupsSnapshot = 0;
			ThreadGroup[] groupsSnapshot = null;
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return 0;
				}
				int ng = Ngroups;
				if (ng > list.Length - n)
				{
					ng = list.Length - n;
				}
				if (ng > 0)
				{
					System.Array.Copy(Groups, 0, list, n, ng);
					n += ng;
				}
				if (recurse)
				{
					ngroupsSnapshot = Ngroups;
					if (Groups != null)
					{
						groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
					}
					else
					{
						groupsSnapshot = null;
					}
				}
			}
			if (recurse)
			{
				for (int i = 0 ; i < ngroupsSnapshot ; i++)
				{
					n = groupsSnapshot[i].Enumerate(list, n, true);
				}
			}
			return n;
		}

		/// <summary>
		/// Stops all threads in this thread group.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread group is
		/// called with no arguments; this may result in a security exception.
		/// </para>
		/// <para>
		/// This method then calls the <code>stop</code> method on all the
		/// threads in this thread group and in all of its subgroups.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed
		///               to access this thread group or any of the threads in
		///               the thread group. </exception>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.Thread#stop() </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess()
		/// @since      JDK1.0 </seealso>
		/// @deprecated    This method is inherently unsafe.  See
		///     <seealso cref="Thread#stop"/> for details. 
		[Obsolete("   This method is inherently unsafe.  See")]
		public void Stop()
		{
			if (StopOrSuspend(false))
			{
				Thread.CurrentThread.Abort();
			}
		}

		/// <summary>
		/// Interrupts all threads in this thread group.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread group is
		/// called with no arguments; this may result in a security exception.
		/// </para>
		/// <para>
		/// This method then calls the <code>interrupt</code> method on all the
		/// threads in this thread group and in all of its subgroups.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed
		///               to access this thread group or any of the threads in
		///               the thread group. </exception>
		/// <seealso cref=        java.lang.Thread#interrupt() </seealso>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess()
		/// @since      1.2 </seealso>
		public void Interrupt()
		{
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				CheckAccess();
				for (int i = 0 ; i < Nthreads ; i++)
				{
					Threads[i].Interrupt();
				}
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				groupsSnapshot[i].Interrupt();
			}
		}

		/// <summary>
		/// Suspends all threads in this thread group.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread group is
		/// called with no arguments; this may result in a security exception.
		/// </para>
		/// <para>
		/// This method then calls the <code>suspend</code> method on all the
		/// threads in this thread group and in all of its subgroups.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed
		///               to access this thread group or any of the threads in
		///               the thread group. </exception>
		/// <seealso cref=        java.lang.Thread#suspend() </seealso>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess()
		/// @since      JDK1.0 </seealso>
		/// @deprecated    This method is inherently deadlock-prone.  See
		///     <seealso cref="Thread#suspend"/> for details. 
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("   This method is inherently deadlock-prone.  See") @SuppressWarnings("deprecation") public final void suspend()
		[Obsolete("   This method is inherently deadlock-prone.  See")]
		public void Suspend()
		{
			if (StopOrSuspend(true))
			{
				Thread.CurrentThread.Suspend();
			}
		}

		/// <summary>
		/// Helper method: recursively stops or suspends (as directed by the
		/// boolean argument) all of the threads in this thread group and its
		/// subgroups, except the current thread.  This method returns true
		/// if (and only if) the current thread is found to be in this thread
		/// group or one of its subgroups.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private boolean stopOrSuspend(boolean suspend)
		private bool StopOrSuspend(bool suspend)
		{
			bool suicide = false;
			Thread us = Thread.CurrentThread;
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot = null;
			lock (this)
			{
				CheckAccess();
				for (int i = 0 ; i < Nthreads ; i++)
				{
					if (Threads[i] == us)
					{
						suicide = true;
					}
					else if (suspend)
					{
						Threads[i].Suspend();
					}
					else
					{
						Threads[i].Stop();
					}
				}

				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				suicide = groupsSnapshot[i].StopOrSuspend(suspend) || suicide;
			}

			return suicide;
		}

		/// <summary>
		/// Resumes all threads in this thread group.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread group is
		/// called with no arguments; this may result in a security exception.
		/// </para>
		/// <para>
		/// This method then calls the <code>resume</code> method on all the
		/// threads in this thread group and in all of its sub groups.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed to
		///               access this thread group or any of the threads in the
		///               thread group. </exception>
		/// <seealso cref=        java.lang.SecurityException </seealso>
		/// <seealso cref=        java.lang.Thread#resume() </seealso>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess()
		/// @since      JDK1.0 </seealso>
		/// @deprecated    This method is used solely in conjunction with
		///      <tt>Thread.suspend</tt> and <tt>ThreadGroup.suspend</tt>,
		///       both of which have been deprecated, as they are inherently
		///       deadlock-prone.  See <seealso cref="Thread#suspend"/> for details. 
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("   This method is used solely in conjunction with") @SuppressWarnings("deprecation") public final void resume()
		[Obsolete("   This method is used solely in conjunction with")]
		public void Resume()
		{
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				CheckAccess();
				for (int i = 0 ; i < Nthreads ; i++)
				{
					Threads[i].Resume();
				}
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				groupsSnapshot[i].Resume();
			}
		}

		/// <summary>
		/// Destroys this thread group and all of its subgroups. This thread
		/// group must be empty, indicating that all threads that had been in
		/// this thread group have since stopped.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread group is
		/// called with no arguments; this may result in a security exception.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalThreadStateException">  if the thread group is not
		///               empty or if the thread group has already been destroyed. </exception>
		/// <exception cref="SecurityException">  if the current thread cannot modify this
		///               thread group. </exception>
		/// <seealso cref=        java.lang.ThreadGroup#checkAccess()
		/// @since      JDK1.0 </seealso>
		public void Destroy()
		{
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				CheckAccess();
				if (Destroyed_Renamed || (Nthreads > 0))
				{
					throw new IllegalThreadStateException();
				}
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
				if (Parent_Renamed != null)
				{
					Destroyed_Renamed = true;
					Ngroups = 0;
					Groups = null;
					Nthreads = 0;
					Threads = null;
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i += 1)
			{
				groupsSnapshot[i].Destroy();
			}
			if (Parent_Renamed != null)
			{
				Parent_Renamed.Remove(this);
			}
		}

		/// <summary>
		/// Adds the specified Thread group to this group. </summary>
		/// <param name="g"> the specified Thread group to be added </param>
		/// <exception cref="IllegalThreadStateException"> If the Thread group has been destroyed. </exception>
		private void Add(ThreadGroup g)
		{
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					throw new IllegalThreadStateException();
				}
				if (Groups == null)
				{
					Groups = new ThreadGroup[4];
				}
				else if (Ngroups == Groups.Length)
				{
					Groups = Arrays.CopyOf(Groups, Ngroups * 2);
				}
				Groups[Ngroups] = g;

				// This is done last so it doesn't matter in case the
				// thread is killed
				Ngroups++;
			}
		}

		/// <summary>
		/// Removes the specified Thread group from this group. </summary>
		/// <param name="g"> the Thread group to be removed </param>
		/// <returns> if this Thread has already been destroyed. </returns>
		private void Remove(ThreadGroup g)
		{
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return;
				}
				for (int i = 0 ; i < Ngroups ; i++)
				{
					if (Groups[i] == g)
					{
						Ngroups -= 1;
						System.Array.Copy(Groups, i + 1, Groups, i, Ngroups - i);
						// Zap dangling reference to the dead group so that
						// the garbage collector will collect it.
						Groups[Ngroups] = null;
						break;
					}
				}
				if (Nthreads == 0)
				{
					Monitor.PulseAll(this);
				}
				if (Daemon_Renamed && (Nthreads == 0) && (NUnstartedThreads == 0) && (Ngroups == 0))
				{
					Destroy();
				}
			}
		}


		/// <summary>
		/// Increments the count of unstarted threads in the thread group.
		/// Unstarted threads are not added to the thread group so that they
		/// can be collected if they are never started, but they must be
		/// counted so that daemon thread groups with unstarted threads in
		/// them are not destroyed.
		/// </summary>
		internal virtual void AddUnstarted()
		{
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					throw new IllegalThreadStateException();
				}
				NUnstartedThreads++;
			}
		}

		/// <summary>
		/// Adds the specified thread to this thread group.
		/// 
		/// <para> Note: This method is called from both library code
		/// and the Virtual Machine. It is called from VM to add
		/// certain system threads to the system thread group.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t">
		///         the Thread to be added
		/// </param>
		/// <exception cref="IllegalThreadStateException">
		///          if the Thread group has been destroyed </exception>
		internal virtual void Add(Thread t)
		{
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					throw new IllegalThreadStateException();
				}
				if (Threads == null)
				{
					Threads = new Thread[4];
				}
				else if (Nthreads == Threads.Length)
				{
					Threads = Arrays.CopyOf(Threads, Nthreads * 2);
				}
				Threads[Nthreads] = t;

				// This is done last so it doesn't matter in case the
				// thread is killed
				Nthreads++;

				// The thread is now a fully fledged member of the group, even
				// though it may, or may not, have been started yet. It will prevent
				// the group from being destroyed so the unstarted Threads count is
				// decremented.
				NUnstartedThreads--;
			}
		}

		/// <summary>
		/// Notifies the group that the thread {@code t} has failed
		/// an attempt to start.
		/// 
		/// <para> The state of this thread group is rolled back as if the
		/// attempt to start the thread has never occurred. The thread is again
		/// considered an unstarted member of the thread group, and a subsequent
		/// attempt to start the thread is permitted.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t">
		///         the Thread whose start method was invoked </param>
		internal virtual void ThreadStartFailed(Thread t)
		{
			lock (this)
			{
				Remove(t);
				NUnstartedThreads++;
			}
		}

		/// <summary>
		/// Notifies the group that the thread {@code t} has terminated.
		/// 
		/// <para> Destroy the group if all of the following conditions are
		/// true: this is a daemon thread group; there are no more alive
		/// or unstarted threads in the group; there are no subgroups in
		/// this thread group.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t">
		///         the Thread that has terminated </param>
		internal virtual void ThreadTerminated(Thread t)
		{
			lock (this)
			{
				Remove(t);

				if (Nthreads == 0)
				{
					Monitor.PulseAll(this);
				}
				if (Daemon_Renamed && (Nthreads == 0) && (NUnstartedThreads == 0) && (Ngroups == 0))
				{
					Destroy();
				}
			}
		}

		/// <summary>
		/// Removes the specified Thread from this group. Invoking this method
		/// on a thread group that has been destroyed has no effect.
		/// </summary>
		/// <param name="t">
		///         the Thread to be removed </param>
		private void Remove(Thread t)
		{
			lock (this)
			{
				if (Destroyed_Renamed)
				{
					return;
				}
				for (int i = 0 ; i < Nthreads ; i++)
				{
					if (Threads[i] == t)
					{
						System.Array.Copy(Threads, i + 1, Threads, i, --Nthreads - i);
						// Zap dangling reference to the dead thread so that
						// the garbage collector will collect it.
						Threads[Nthreads] = null;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Prints information about this thread group to the standard
		/// output. This method is useful only for debugging.
		/// 
		/// @since   JDK1.0
		/// </summary>
		public virtual void List()
		{
			List(System.out, 0);
		}
		internal virtual void List(PrintStream @out, int indent)
		{
			int ngroupsSnapshot;
			ThreadGroup[] groupsSnapshot;
			lock (this)
			{
				for (int j = 0 ; j < indent ; j++)
				{
					@out.Print(" ");
				}
				@out.Println(this);
				indent += 4;
				for (int i = 0 ; i < Nthreads ; i++)
				{
					for (int j = 0 ; j < indent ; j++)
					{
						@out.Print(" ");
					}
					@out.Println(Threads[i]);
				}
				ngroupsSnapshot = Ngroups;
				if (Groups != null)
				{
					groupsSnapshot = Arrays.CopyOf(Groups, ngroupsSnapshot);
				}
				else
				{
					groupsSnapshot = null;
				}
			}
			for (int i = 0 ; i < ngroupsSnapshot ; i++)
			{
				groupsSnapshot[i].List(@out, indent);
			}
		}

		/// <summary>
		/// Called by the Java Virtual Machine when a thread in this
		/// thread group stops because of an uncaught exception, and the thread
		/// does not have a specific <seealso cref="Thread.UncaughtExceptionHandler"/>
		/// installed.
		/// <para>
		/// The <code>uncaughtException</code> method of
		/// <code>ThreadGroup</code> does the following:
		/// <ul>
		/// <li>If this thread group has a parent thread group, the
		///     <code>uncaughtException</code> method of that parent is called
		///     with the same two arguments.
		/// <li>Otherwise, this method checks to see if there is a
		///     {@link Thread#getDefaultUncaughtExceptionHandler default
		///     uncaught exception handler} installed, and if so, its
		///     <code>uncaughtException</code> method is called with the same
		///     two arguments.
		/// <li>Otherwise, this method determines if the <code>Throwable</code>
		///     argument is an instance of <seealso cref="ThreadDeath"/>. If so, nothing
		///     special is done. Otherwise, a message containing the
		///     thread's name, as returned from the thread's {@link
		///     Thread#getName getName} method, and a stack backtrace,
		///     using the <code>Throwable</code>'s {@link
		///     Throwable#printStackTrace printStackTrace} method, is
		///     printed to the <seealso cref="System#err standard error stream"/>.
		/// </ul>
		/// </para>
		/// <para>
		/// Applications can override this method in subclasses of
		/// <code>ThreadGroup</code> to provide alternative handling of
		/// uncaught exceptions.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t">   the thread that is about to exit. </param>
		/// <param name="e">   the uncaught exception.
		/// @since   JDK1.0 </param>
		public virtual void UncaughtException(Thread t, Throwable e)
		{
			if (Parent_Renamed != null)
			{
				Parent_Renamed.UncaughtException(t, e);
			}
			else
			{
				Thread.UncaughtExceptionHandler ueh = Thread.DefaultUncaughtExceptionHandler;
				if (ueh != null)
				{
					ueh.UncaughtException(t, e);
				}
				else if (!(e is ThreadDeath))
				{
					System.Console.Error.Write("Exception in thread \"" + t.Name + "\" ");
					e.PrintStackTrace(System.err);
				}
			}
		}

		/// <summary>
		/// Used by VM to control lowmem implicit suspension.
		/// </summary>
		/// <param name="b"> boolean to allow or disallow suspension </param>
		/// <returns> true on success
		/// @since   JDK1.1 </returns>
		/// @deprecated The definition of this call depends on <seealso cref="#suspend"/>,
		///             which is deprecated.  Further, the behavior of this call
		///             was never specified. 
		[Obsolete("The definition of this call depends on <seealso cref="#suspend"/>,")]
		public virtual bool AllowThreadSuspension(bool b)
		{
			this.VmAllowSuspension = b;
			if (!b)
			{
				VM.unsuspendSomeThreads();
			}
			return true;
		}

		/// <summary>
		/// Returns a string representation of this Thread group.
		/// </summary>
		/// <returns>  a string representation of this thread group.
		/// @since   JDK1.0 </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[name=" + Name + ",maxpri=" + MaxPriority_Renamed + "]";
		}
	}

}