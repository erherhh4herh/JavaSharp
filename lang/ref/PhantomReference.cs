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
	/// Phantom reference objects, which are enqueued after the collector
	/// determines that their referents may otherwise be reclaimed.  Phantom
	/// references are most often used for scheduling pre-mortem cleanup actions in
	/// a more flexible way than is possible with the Java finalization mechanism.
	/// 
	/// <para> If the garbage collector determines at a certain point in time that the
	/// referent of a phantom reference is <a
	/// href="package-summary.html#reachability">phantom reachable</a>, then at that
	/// time or at some later time it will enqueue the reference.
	/// 
	/// </para>
	/// <para> In order to ensure that a reclaimable object remains so, the referent of
	/// a phantom reference may not be retrieved: The <code>get</code> method of a
	/// phantom reference always returns <code>null</code>.
	/// 
	/// </para>
	/// <para> Unlike soft and weak references, phantom references are not
	/// automatically cleared by the garbage collector as they are enqueued.  An
	/// object that is reachable via phantom references will remain so until all
	/// such references are cleared or themselves become unreachable.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.2
	/// </para>
	/// </summary>

	public class PhantomReference<T> : Reference<T>
	{

		/// <summary>
		/// Returns this reference object's referent.  Because the referent of a
		/// phantom reference is always inaccessible, this method always returns
		/// <code>null</code>.
		/// </summary>
		/// <returns>  <code>null</code> </returns>
		public virtual T Get()
		{
			return null;
		}

		/// <summary>
		/// Creates a new phantom reference that refers to the given object and
		/// is registered with the given queue.
		/// 
		/// <para> It is possible to create a phantom reference with a <tt>null</tt>
		/// queue, but such a reference is completely useless: Its <tt>get</tt>
		/// method will always return null and, since it does not have a queue, it
		/// will never be enqueued.
		/// 
		/// </para>
		/// </summary>
		/// <param name="referent"> the object the new phantom reference will refer to </param>
		/// <param name="q"> the queue with which the reference is to be registered,
		///          or <tt>null</tt> if registration is not required </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public PhantomReference(T referent, ReferenceQueue<? base T> q)
		public PhantomReference<T1>(T referent, ReferenceQueue<T1> q) : base(referent, q)
		{
		}

	}

}