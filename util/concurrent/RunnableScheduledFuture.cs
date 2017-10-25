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
	/// A <seealso cref="ScheduledFuture"/> that is <seealso cref="Runnable"/>. Successful
	/// execution of the {@code run} method causes completion of the
	/// {@code Future} and allows access to its results. </summary>
	/// <seealso cref= FutureTask </seealso>
	/// <seealso cref= Executor
	/// @since 1.6
	/// @author Doug Lea </seealso>
	/// @param <V> The result type returned by this Future's {@code get} method </param>
	public interface RunnableScheduledFuture<V> : RunnableFuture<V>, ScheduledFuture<V>
	{

		/// <summary>
		/// Returns {@code true} if this task is periodic. A periodic task may
		/// re-run according to some schedule. A non-periodic task can be
		/// run only once.
		/// </summary>
		/// <returns> {@code true} if this task is periodic </returns>
		bool Periodic {get;}
	}

}