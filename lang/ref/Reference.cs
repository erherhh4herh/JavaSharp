using System;
using System.Collections;
using System.Threading;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.@ref
{

	using Cleaner = sun.misc.Cleaner;

	/// <summary>
	/// Abstract base class for reference objects.  This class defines the
	/// operations common to all reference objects.  Because reference objects are
	/// implemented in close cooperation with the garbage collector, this class may
	/// not be subclassed directly.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.2
	/// </summary>

	public abstract class Reference<T>
	{

		/* A Reference instance is in one of four possible internal states:
		 *
		 *     Active: Subject to special treatment by the garbage collector.  Some
		 *     time after the collector detects that the reachability of the
		 *     referent has changed to the appropriate state, it changes the
		 *     instance's state to either Pending or Inactive, depending upon
		 *     whether or not the instance was registered with a queue when it was
		 *     created.  In the former case it also adds the instance to the
		 *     pending-Reference list.  Newly-created instances are Active.
		 *
		 *     Pending: An element of the pending-Reference list, waiting to be
		 *     enqueued by the Reference-handler thread.  Unregistered instances
		 *     are never in this state.
		 *
		 *     Enqueued: An element of the queue with which the instance was
		 *     registered when it was created.  When an instance is removed from
		 *     its ReferenceQueue, it is made Inactive.  Unregistered instances are
		 *     never in this state.
		 *
		 *     Inactive: Nothing more to do.  Once an instance becomes Inactive its
		 *     state will never change again.
		 *
		 * The state is encoded in the queue and next fields as follows:
		 *
		 *     Active: queue = ReferenceQueue with which instance is registered, or
		 *     ReferenceQueue.NULL if it was not registered with a queue; next =
		 *     null.
		 *
		 *     Pending: queue = ReferenceQueue with which instance is registered;
		 *     next = this
		 *
		 *     Enqueued: queue = ReferenceQueue.ENQUEUED; next = Following instance
		 *     in queue, or this if at end of list.
		 *
		 *     Inactive: queue = ReferenceQueue.NULL; next = this.
		 *
		 * With this scheme the collector need only examine the next field in order
		 * to determine whether a Reference instance requires special treatment: If
		 * the next field is null then the instance is active; if it is non-null,
		 * then the collector should treat the instance normally.
		 *
		 * To ensure that a concurrent collector can discover active Reference
		 * objects without interfering with application threads that may apply
		 * the enqueue() method to those objects, collectors should link
		 * discovered objects through the discovered field. The discovered
		 * field is also used for linking Reference objects in the pending list.
		 */

		private T Referent; // Treated specially by GC

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: volatile ReferenceQueue<? base T> queue;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal volatile ReferenceQueue<?> Queue;

		/* When active:   NULL
		 *     pending:   this
		 *    Enqueued:   next reference in queue (or this if last)
		 *    Inactive:   this
		 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") Reference next;
		internal Reference Next;

		/* When active:   next element in a discovered reference list maintained by GC (or this if last)
		 *     pending:   next element in the pending list (or null if last)
		 *   otherwise:   NULL
		 */
		[NonSerialized]
		private Reference<T> Discovered; // used by VM


		/* Object used to synchronize with the garbage collector.  The collector
		 * must acquire this lock at the beginning of each collection cycle.  It is
		 * therefore critical that any code holding this lock complete as quickly
		 * as possible, allocate no new objects, and avoid calling user code.
		 */
		private class Lock
		{
		}
		private static Lock @lock = new Lock();


		/* List of References waiting to be enqueued.  The collector adds
		 * References to this list, while the Reference-handler thread removes
		 * them.  This list is protected by the above lock object. The
		 * list uses the discovered field to link its elements.
		 */
		private static Reference<Object> Pending = null;

		/* High-priority thread to enqueue pending References
		 */
		private class ReferenceHandler : Thread
		{

			internal ReferenceHandler(ThreadGroup g, String name) : base(g, name)
			{
			}

			public override void Run()
			{
				for (;;)
				{
					Reference<Object> r;
					lock (@lock)
					{
						if (Pending != null)
						{
							r = Pending;
							Pending = r.Discovered;
							r.Discovered = null;
						}
						else
						{
							// The waiting on the lock may cause an OOME because it may try to allocate
							// exception objects, so also catch OOME here to avoid silent exit of the
							// reference handler thread.
							//
							// Explicitly define the order of the two exceptions we catch here
							// when waiting for the lock.
							//
							// We do not want to try to potentially load the InterruptedException class
							// (which would be done if this was its first use, and InterruptedException
							// were checked first) in this situation.
							//
							// This may lead to the VM not ever trying to load the InterruptedException
							// class again.
							try
							{
								try
								{
									Monitor.Wait(@lock);
								}
								catch (OutOfMemoryError)
								{
								}
							}
							catch (InterruptedException)
							{
							}
							continue;
						}
					}

					// Fast path for cleaners
					if (r is Cleaner)
					{
						((Cleaner)r).clean();
						continue;
					}

					ReferenceQueue<Object> q = r.Queue;
					if (q != ReferenceQueue.NULL)
					{
						q.Enqueue(r);
					}
				}
			}
		}

		static Reference()
		{
			ThreadGroup tg = Thread.CurrentThread.ThreadGroup;
			for (ThreadGroup tgn = tg; tgn != null; tg = tgn, tgn = tg.Parent)
			{
				;
			}
			Thread handler = new ReferenceHandler(tg, "Reference Handler");
			/* If there were a special system-only priority greater than
			 * MAX_PRIORITY, it would be used here
			 */
			handler.Priority = Thread.MAX_PRIORITY;
			handler.Daemon = true;
			handler.Start();
		}


		/* -- Referent accessor and setters -- */

		/// <summary>
		/// Returns this reference object's referent.  If this reference object has
		/// been cleared, either by the program or by the garbage collector, then
		/// this method returns <code>null</code>.
		/// </summary>
		/// <returns>   The object to which this reference refers, or
		///           <code>null</code> if this reference object has been cleared </returns>
		public virtual T Get()
		{
			return this.Referent;
		}

		/// <summary>
		/// Clears this reference object.  Invoking this method will not cause this
		/// object to be enqueued.
		/// 
		/// <para> This method is invoked only by Java code; when the garbage collector
		/// clears references it does so directly, without invoking this method.
		/// </para>
		/// </summary>
		public virtual void Clear()
		{
			this.Referent = null;
		}


		/* -- Queue operations -- */

		/// <summary>
		/// Tells whether or not this reference object has been enqueued, either by
		/// the program or by the garbage collector.  If this reference object was
		/// not registered with a queue when it was created, then this method will
		/// always return <code>false</code>.
		/// </summary>
		/// <returns>   <code>true</code> if and only if this reference object has
		///           been enqueued </returns>
		public virtual bool Enqueued
		{
			get
			{
				return (this.Queue == ReferenceQueue.ENQUEUED);
			}
		}

		/// <summary>
		/// Adds this reference object to the queue with which it is registered,
		/// if any.
		/// 
		/// <para> This method is invoked only by Java code; when the garbage collector
		/// enqueues references it does so directly, without invoking this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>   <code>true</code> if this reference object was successfully
		///           enqueued; <code>false</code> if it was already enqueued or if
		///           it was not registered with a queue when it was created </returns>
		public virtual bool Enqueue()
		{
			return this.Queue.Enqueue(this);
		}


		/* -- Constructors -- */

		internal Reference(T referent) : this(referent, null)
		{
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Reference(T referent, ReferenceQueue<? base T> queue)
		internal Reference<T1>(T referent, ReferenceQueue<T1> queue)
		{
			this.Referent = referent;
			this.Queue = (queue == null) ? ReferenceQueue.NULL : queue;
		}

	}

}