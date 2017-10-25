/*
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

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	/// <summary>
	/// A delayed result-bearing action that can be cancelled.
	/// Usually a scheduled future is the result of scheduling
	/// a task with a <seealso cref="ScheduledExecutorService"/>.
	/// 
	/// @since 1.5
	/// @author Doug Lea </summary>
	/// @param <V> The result type returned by this Future </param>
	public interface ScheduledFuture<V> : Delayed, Future<V>
	{
	}

}