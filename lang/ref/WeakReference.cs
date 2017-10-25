/*
 * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// Weak reference objects, which do not prevent their referents from being
	/// made finalizable, finalized, and then reclaimed.  Weak references are most
	/// often used to implement canonicalizing mappings.
	/// 
	/// <para> Suppose that the garbage collector determines at a certain point in time
	/// that an object is <a href="package-summary.html#reachability">weakly
	/// reachable</a>.  At that time it will atomically clear all weak references to
	/// that object and all weak references to any other weakly-reachable objects
	/// from which that object is reachable through a chain of strong and soft
	/// references.  At the same time it will declare all of the formerly
	/// weakly-reachable objects to be finalizable.  At the same time or at some
	/// later time it will enqueue those newly-cleared weak references that are
	/// registered with reference queues.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.2
	/// </para>
	/// </summary>

	public class WeakReference<T> : Reference<T>
	{

		/// <summary>
		/// Creates a new weak reference that refers to the given object.  The new
		/// reference is not registered with any queue.
		/// </summary>
		/// <param name="referent"> object the new weak reference will refer to </param>
		public WeakReference(T referent) : base(referent)
		{
		}

		/// <summary>
		/// Creates a new weak reference that refers to the given object and is
		/// registered with the given queue.
		/// </summary>
		/// <param name="referent"> object the new weak reference will refer to </param>
		/// <param name="q"> the queue with which the reference is to be registered,
		///          or <tt>null</tt> if registration is not required </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public WeakReference(T referent, ReferenceQueue<? base T> q)
		public WeakReference<T1>(T referent, ReferenceQueue<T1> q) : base(referent, q)
		{
		}

	}

}