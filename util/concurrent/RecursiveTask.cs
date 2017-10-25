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
	/// A recursive result-bearing <seealso cref="ForkJoinTask"/>.
	/// 
	/// <para>For a classic example, here is a task computing Fibonacci numbers:
	/// 
	///  <pre> {@code
	/// class Fibonacci extends RecursiveTask<Integer> {
	///   final int n;
	///   Fibonacci(int n) { this.n = n; }
	///   Integer compute() {
	///     if (n <= 1)
	///       return n;
	///     Fibonacci f1 = new Fibonacci(n - 1);
	///     f1.fork();
	///     Fibonacci f2 = new Fibonacci(n - 2);
	///     return f2.compute() + f1.join();
	///   }
	/// }}</pre>
	/// 
	/// However, besides being a dumb way to compute Fibonacci functions
	/// (there is a simple fast linear algorithm that you'd use in
	/// practice), this is likely to perform poorly because the smallest
	/// subtasks are too small to be worthwhile splitting up. Instead, as
	/// is the case for nearly all fork/join applications, you'd pick some
	/// minimum granularity size (for example 10 here) for which you always
	/// sequentially solve rather than subdividing.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public abstract class RecursiveTask<V> : ForkJoinTask<V>
	{
		private const long SerialVersionUID = 5232453952276485270L;

		/// <summary>
		/// The result of the computation.
		/// </summary>
		internal V Result;

		/// <summary>
		/// The main computation performed by this task. </summary>
		/// <returns> the result of the computation </returns>
		protected internal abstract V Compute();

		public V RawResult
		{
			get
			{
				return Result;
			}
			set
			{
				Result = value;
			}
		}


		/// <summary>
		/// Implements execution conventions for RecursiveTask.
		/// </summary>
		protected internal bool Exec()
		{
			Result = Compute();
			return true;
		}

	}

}