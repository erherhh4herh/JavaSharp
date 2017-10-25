using System;
using System.Diagnostics;
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

	/// <summary>
	/// Reference queues, to which registered reference objects are appended by the
	/// garbage collector after the appropriate reachability changes are detected.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.2
	/// </summary>

	public class ReferenceQueue<T>
	{

		/// <summary>
		/// Constructs a new reference-object queue.
		/// </summary>
		public ReferenceQueue()
		{
		}

		private class Null<S> : ReferenceQueue<S>
		{
			internal virtual bool enqueue<T1>(Reference<T1> r) where T1 : S
			{
				return false;
			}
		}

		internal static ReferenceQueue<Object> NULL = new Null<Object>();
		internal static ReferenceQueue<Object> ENQUEUED = new Null<Object>();

		private class Lock
		{
		}
		private Lock @lock = new Lock();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private volatile Reference<? extends T> head = null;
		private volatile Reference<?> Head = null;
		private long QueueLength = 0;

		internal virtual bool enqueue<T1>(Reference<T1> r) where T1 : T // Called only by Reference class
		{
			lock (@lock)
			{
				// Check that since getting the lock this reference hasn't already been
				// enqueued (and even then removed)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ReferenceQueue<?> queue = r.queue;
				ReferenceQueue<?> queue = r.Queue;
				if ((queue == NULL) || (queue == ENQUEUED))
				{
					return false;
				}
				Debug.Assert(queue == this);
				r.Queue = ENQUEUED;
				r.Next = (Head == null) ? r : Head;
				Head = r;
				QueueLength++;
				if (r is FinalReference)
				{
					sun.misc.VM.addFinalRefCount(1);
				}
				Monitor.PulseAll(@lock);
				return true;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private Reference<? extends T> reallyPoll()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private Reference<? extends T> reallyPoll()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private Reference<?> ReallyPoll() where ? : T // Must hold lock
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<? extends T> r = head;
			Reference<?> r = Head;
			if (r != null)
			{
				Head = (r.Next == r) ? null : r.Next; // Unchecked due to the next field having a raw type in Reference
				r.Queue = NULL;
				r.Next = r;
				QueueLength--;
				if (r is FinalReference)
				{
					sun.misc.VM.addFinalRefCount(-1);
				}
				return r;
			}
			return null;
		}

		/// <summary>
		/// Polls this queue to see if a reference object is available.  If one is
		/// available without further delay then it is removed from the queue and
		/// returned.  Otherwise this method immediately returns <tt>null</tt>.
		/// </summary>
		/// <returns>  A reference object, if one was immediately available,
		///          otherwise <code>null</code> </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Reference<? extends T> poll()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Reference<? extends T> poll()
		public virtual Reference<?> Poll() where ? : T
		{
			if (Head == null)
			{
				return null;
			}
			lock (@lock)
			{
				return ReallyPoll();
			}
		}

		/// <summary>
		/// Removes the next reference object in this queue, blocking until either
		/// one becomes available or the given timeout period expires.
		/// 
		/// <para> This method does not offer real-time guarantees: It schedules the
		/// timeout as if by invoking the <seealso cref="Object#wait(long)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout">  If positive, block for up to <code>timeout</code>
		///                  milliseconds while waiting for a reference to be
		///                  added to this queue.  If zero, block indefinitely.
		/// </param>
		/// <returns>  A reference object, if one was available within the specified
		///          timeout period, otherwise <code>null</code>
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If the value of the timeout argument is negative
		/// </exception>
		/// <exception cref="InterruptedException">
		///          If the timeout wait is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reference<? extends T> remove(long timeout) throws IllegalArgumentException, InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reference<? extends T> remove(long timeout) throws IllegalArgumentException, InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual Reference<?> Remove(long timeout) where ? : T
		{
			if (timeout < 0)
			{
				throw new IllegalArgumentException("Negative timeout value");
			}
			lock (@lock)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<? extends T> r = reallyPoll();
				Reference<?> r = ReallyPoll();
				if (r != null)
				{
					return r;
				}
				long start = (timeout == 0) ? 0 : System.nanoTime();
				for (;;)
				{
					Monitor.Wait(@lock, TimeSpan.FromMilliseconds(timeout));
					r = ReallyPoll();
					if (r != null)
					{
						return r;
					}
					if (timeout != 0)
					{
						long end = System.nanoTime();
						timeout -= (end - start) / 1000000;
						if (timeout <= 0)
						{
							return null;
						}
						start = end;
					}
				}
			}
		}

		/// <summary>
		/// Removes the next reference object in this queue, blocking until one
		/// becomes available.
		/// </summary>
		/// <returns> A reference object, blocking until one becomes available </returns>
		/// <exception cref="InterruptedException">  If the wait is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reference<? extends T> remove() throws InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Reference<? extends T> remove() throws InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual Reference<?> Remove() where ? : T
		{
			return Remove(0);
		}

	}

}