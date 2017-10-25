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
	/// A handler for tasks that cannot be executed by a <seealso cref="ThreadPoolExecutor"/>.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	public interface RejectedExecutionHandler
	{

		/// <summary>
		/// Method that may be invoked by a <seealso cref="ThreadPoolExecutor"/> when
		/// <seealso cref="ThreadPoolExecutor#execute execute"/> cannot accept a
		/// task.  This may occur when no more threads or queue slots are
		/// available because their bounds would be exceeded, or upon
		/// shutdown of the Executor.
		/// 
		/// <para>In the absence of other alternatives, the method may throw
		/// an unchecked <seealso cref="RejectedExecutionException"/>, which will be
		/// propagated to the caller of {@code execute}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="r"> the runnable task requested to be executed </param>
		/// <param name="executor"> the executor attempting to execute this task </param>
		/// <exception cref="RejectedExecutionException"> if there is no remedy </exception>
		void RejectedExecution(Runnable r, ThreadPoolExecutor executor);
	}

}