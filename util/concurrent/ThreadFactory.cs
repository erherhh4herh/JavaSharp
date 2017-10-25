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
	/// An object that creates new threads on demand.  Using thread factories
	/// removes hardwiring of calls to <seealso cref="Thread#Thread(Runnable) new Thread"/>,
	/// enabling applications to use special thread subclasses, priorities, etc.
	/// 
	/// <para>
	/// The simplest implementation of this interface is just:
	///  <pre> {@code
	/// class SimpleThreadFactory implements ThreadFactory {
	///   public Thread newThread(Runnable r) {
	///     return new Thread(r);
	///   }
	/// }}</pre>
	/// 
	/// The <seealso cref="Executors#defaultThreadFactory"/> method provides a more
	/// useful simple implementation, that sets the created thread context
	/// to known values before returning it.
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public interface ThreadFactory
	{

		/// <summary>
		/// Constructs a new {@code Thread}.  Implementations may also initialize
		/// priority, name, daemon status, {@code ThreadGroup}, etc.
		/// </summary>
		/// <param name="r"> a runnable to be executed by new thread instance </param>
		/// <returns> constructed thread, or {@code null} if the request to
		///         create a thread is rejected </returns>
		Thread NewThread(Runnable r);
	}

}