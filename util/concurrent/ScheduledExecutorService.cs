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
	/// An <seealso cref="ExecutorService"/> that can schedule commands to run after a given
	/// delay, or to execute periodically.
	/// 
	/// <para>The {@code schedule} methods create tasks with various delays
	/// and return a task object that can be used to cancel or check
	/// execution. The {@code scheduleAtFixedRate} and
	/// {@code scheduleWithFixedDelay} methods create and execute tasks
	/// that run periodically until cancelled.
	/// 
	/// </para>
	/// <para>Commands submitted using the <seealso cref="Executor#execute(Runnable)"/>
	/// and <seealso cref="ExecutorService"/> {@code submit} methods are scheduled
	/// with a requested delay of zero. Zero and negative delays (but not
	/// periods) are also allowed in {@code schedule} methods, and are
	/// treated as requests for immediate execution.
	/// 
	/// </para>
	/// <para>All {@code schedule} methods accept <em>relative</em> delays and
	/// periods as arguments, not absolute times or dates. It is a simple
	/// matter to transform an absolute time represented as a {@link
	/// java.util.Date} to the required form. For example, to schedule at
	/// a certain future {@code date}, you can use: {@code schedule(task,
	/// date.getTime() - System.currentTimeMillis(),
	/// TimeUnit.MILLISECONDS)}. Beware however that expiration of a
	/// relative delay need not coincide with the current {@code Date} at
	/// which the task is enabled due to network time synchronization
	/// protocols, clock drift, or other factors.
	/// 
	/// </para>
	/// <para>The <seealso cref="Executors"/> class provides convenient factory methods for
	/// the ScheduledExecutorService implementations provided in this package.
	/// 
	/// <h3>Usage Example</h3>
	/// 
	/// Here is a class with a method that sets up a ScheduledExecutorService
	/// to beep every ten seconds for an hour:
	/// 
	///  <pre> {@code
	/// import static java.util.concurrent.TimeUnit.*;
	/// class BeeperControl {
	///   private final ScheduledExecutorService scheduler =
	///     Executors.newScheduledThreadPool(1);
	/// 
	///   public void beepForAnHour() {
	///     final Runnable beeper = new Runnable() {
	///       public void run() { System.out.println("beep"); }
	///     };
	///     final ScheduledFuture<?> beeperHandle =
	///       scheduler.scheduleAtFixedRate(beeper, 10, 10, SECONDS);
	///     scheduler.schedule(new Runnable() {
	///       public void run() { beeperHandle.cancel(true); }
	///     }, 60 * 60, SECONDS);
	///   }
	/// }}</pre>
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public interface ScheduledExecutorService : ExecutorService
	{

		/// <summary>
		/// Creates and executes a one-shot action that becomes enabled
		/// after the given delay.
		/// </summary>
		/// <param name="command"> the task to execute </param>
		/// <param name="delay"> the time from now to delay execution </param>
		/// <param name="unit"> the time unit of the delay parameter </param>
		/// <returns> a ScheduledFuture representing pending completion of
		///         the task and whose {@code get()} method will return
		///         {@code null} upon completion </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if command is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> schedule(Runnable command, long delay, TimeUnit unit);
		ScheduledFuture<?> Schedule(Runnable command, long delay, TimeUnit unit);

		/// <summary>
		/// Creates and executes a ScheduledFuture that becomes enabled after the
		/// given delay.
		/// </summary>
		/// <param name="callable"> the function to execute </param>
		/// <param name="delay"> the time from now to delay execution </param>
		/// <param name="unit"> the time unit of the delay parameter </param>
		/// @param <V> the type of the callable's result </param>
		/// <returns> a ScheduledFuture that can be used to extract result or cancel </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if callable is null </exception>
		ScheduledFuture<V> schedule<V>(Callable<V> callable, long delay, TimeUnit unit);

		/// <summary>
		/// Creates and executes a periodic action that becomes enabled first
		/// after the given initial delay, and subsequently with the given
		/// period; that is executions will commence after
		/// {@code initialDelay} then {@code initialDelay+period}, then
		/// {@code initialDelay + 2 * period}, and so on.
		/// If any execution of the task
		/// encounters an exception, subsequent executions are suppressed.
		/// Otherwise, the task will only terminate via cancellation or
		/// termination of the executor.  If any execution of this task
		/// takes longer than its period, then subsequent executions
		/// may start late, but will not concurrently execute.
		/// </summary>
		/// <param name="command"> the task to execute </param>
		/// <param name="initialDelay"> the time to delay first execution </param>
		/// <param name="period"> the period between successive executions </param>
		/// <param name="unit"> the time unit of the initialDelay and period parameters </param>
		/// <returns> a ScheduledFuture representing pending completion of
		///         the task, and whose {@code get()} method will throw an
		///         exception upon cancellation </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if command is null </exception>
		/// <exception cref="IllegalArgumentException"> if period less than or equal to zero </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit);
		ScheduledFuture<?> ScheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit);

		/// <summary>
		/// Creates and executes a periodic action that becomes enabled first
		/// after the given initial delay, and subsequently with the
		/// given delay between the termination of one execution and the
		/// commencement of the next.  If any execution of the task
		/// encounters an exception, subsequent executions are suppressed.
		/// Otherwise, the task will only terminate via cancellation or
		/// termination of the executor.
		/// </summary>
		/// <param name="command"> the task to execute </param>
		/// <param name="initialDelay"> the time to delay first execution </param>
		/// <param name="delay"> the delay between the termination of one
		/// execution and the commencement of the next </param>
		/// <param name="unit"> the time unit of the initialDelay and delay parameters </param>
		/// <returns> a ScheduledFuture representing pending completion of
		///         the task, and whose {@code get()} method will throw an
		///         exception upon cancellation </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if command is null </exception>
		/// <exception cref="IllegalArgumentException"> if delay less than or equal to zero </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit);
		ScheduledFuture<?> ScheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit);

	}

}