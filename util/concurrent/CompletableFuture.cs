using System;
using System.Collections;
using System.Threading;

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
	/// A <seealso cref="Future"/> that may be explicitly completed (setting its
	/// value and status), and may be used as a <seealso cref="CompletionStage"/>,
	/// supporting dependent functions and actions that trigger upon its
	/// completion.
	/// 
	/// <para>When two or more threads attempt to
	/// <seealso cref="#complete complete"/>,
	/// <seealso cref="#completeExceptionally completeExceptionally"/>, or
	/// <seealso cref="#cancel cancel"/>
	/// a CompletableFuture, only one of them succeeds.
	/// 
	/// </para>
	/// <para>In addition to these and related methods for directly
	/// manipulating status and results, CompletableFuture implements
	/// interface <seealso cref="CompletionStage"/> with the following policies: <ul>
	/// 
	/// <li>Actions supplied for dependent completions of
	/// <em>non-async</em> methods may be performed by the thread that
	/// completes the current CompletableFuture, or by any other caller of
	/// a completion method.</li>
	/// 
	/// <li>All <em>async</em> methods without an explicit Executor
	/// argument are performed using the <seealso cref="ForkJoinPool#commonPool()"/>
	/// (unless it does not support a parallelism level of at least two, in
	/// which case, a new Thread is created to run each task).  To simplify
	/// monitoring, debugging, and tracking, all generated asynchronous
	/// tasks are instances of the marker interface {@link
	/// AsynchronousCompletionTask}. </li>
	/// 
	/// <li>All CompletionStage methods are implemented independently of
	/// other public methods, so the behavior of one method is not impacted
	/// by overrides of others in subclasses.  </li> </ul>
	/// 
	/// </para>
	/// <para>CompletableFuture also implements <seealso cref="Future"/> with the following
	/// policies: <ul>
	/// 
	/// <li>Since (unlike <seealso cref="FutureTask"/>) this class has no direct
	/// control over the computation that causes it to be completed,
	/// cancellation is treated as just another form of exceptional
	/// completion.  Method <seealso cref="#cancel cancel"/> has the same effect as
	/// {@code completeExceptionally(new CancellationException())}. Method
	/// <seealso cref="#isCompletedExceptionally"/> can be used to determine if a
	/// CompletableFuture completed in any exceptional fashion.</li>
	/// 
	/// <li>In case of exceptional completion with a CompletionException,
	/// methods <seealso cref="#get()"/> and <seealso cref="#get(long, TimeUnit)"/> throw an
	/// <seealso cref="ExecutionException"/> with the same cause as held in the
	/// corresponding CompletionException.  To simplify usage in most
	/// contexts, this class also defines methods <seealso cref="#join()"/> and
	/// <seealso cref="#getNow"/> that instead throw the CompletionException directly
	/// in these cases.</li> </ul>
	/// 
	/// @author Doug Lea
	/// @since 1.8
	/// </para>
	/// </summary>
	public class CompletableFuture<T> : Future<T>, CompletionStage<T>
	{

		/*
		 * Overview:
		 *
		 * A CompletableFuture may have dependent completion actions,
		 * collected in a linked stack. It atomically completes by CASing
		 * a result field, and then pops off and runs those actions. This
		 * applies across normal vs exceptional outcomes, sync vs async
		 * actions, binary triggers, and various forms of completions.
		 *
		 * Non-nullness of field result (set via CAS) indicates done.  An
		 * AltResult is used to box null as a result, as well as to hold
		 * exceptions.  Using a single field makes completion simple to
		 * detect and trigger.  Encoding and decoding is straightforward
		 * but adds to the sprawl of trapping and associating exceptions
		 * with targets.  Minor simplifications rely on (static) NIL (to
		 * box null results) being the only AltResult with a null
		 * exception field, so we don't usually need explicit comparisons.
		 * Even though some of the generics casts are unchecked (see
		 * SuppressWarnings annotations), they are placed to be
		 * appropriate even if checked.
		 *
		 * Dependent actions are represented by Completion objects linked
		 * as Treiber stacks headed by field "stack". There are Completion
		 * classes for each kind of action, grouped into single-input
		 * (UniCompletion), two-input (BiCompletion), projected
		 * (BiCompletions using either (not both) of two inputs), shared
		 * (CoCompletion, used by the second of two sources), zero-input
		 * source actions, and Signallers that unblock waiters. Class
		 * Completion extends ForkJoinTask to enable async execution
		 * (adding no space overhead because we exploit its "tag" methods
		 * to maintain claims). It is also declared as Runnable to allow
		 * usage with arbitrary executors.
		 *
		 * Support for each kind of CompletionStage relies on a separate
		 * class, along with two CompletableFuture methods:
		 *
		 * * A Completion class with name X corresponding to function,
		 *   prefaced with "Uni", "Bi", or "Or". Each class contains
		 *   fields for source(s), actions, and dependent. They are
		 *   boringly similar, differing from others only with respect to
		 *   underlying functional forms. We do this so that users don't
		 *   encounter layers of adaptors in common usages. We also
		 *   include "Relay" classes/methods that don't correspond to user
		 *   methods; they copy results from one stage to another.
		 *
		 * * Boolean CompletableFuture method x(...) (for example
		 *   uniApply) takes all of the arguments needed to check that an
		 *   action is triggerable, and then either runs the action or
		 *   arranges its async execution by executing its Completion
		 *   argument, if present. The method returns true if known to be
		 *   complete.
		 *
		 * * Completion method tryFire(int mode) invokes the associated x
		 *   method with its held arguments, and on success cleans up.
		 *   The mode argument allows tryFire to be called twice (SYNC,
		 *   then ASYNC); the first to screen and trap exceptions while
		 *   arranging to execute, and the second when called from a
		 *   task. (A few classes are not used async so take slightly
		 *   different forms.)  The claim() callback suppresses function
		 *   invocation if already claimed by another thread.
		 *
		 * * CompletableFuture method xStage(...) is called from a public
		 *   stage method of CompletableFuture x. It screens user
		 *   arguments and invokes and/or creates the stage object.  If
		 *   not async and x is already complete, the action is run
		 *   immediately.  Otherwise a Completion c is created, pushed to
		 *   x's stack (unless done), and started or triggered via
		 *   c.tryFire.  This also covers races possible if x completes
		 *   while pushing.  Classes with two inputs (for example BiApply)
		 *   deal with races across both while pushing actions.  The
		 *   second completion is a CoCompletion pointing to the first,
		 *   shared so that at most one performs the action.  The
		 *   multiple-arity methods allOf and anyOf do this pairwise to
		 *   form trees of completions.
		 *
		 * Note that the generic type parameters of methods vary according
		 * to whether "this" is a source, dependent, or completion.
		 *
		 * Method postComplete is called upon completion unless the target
		 * is guaranteed not to be observable (i.e., not yet returned or
		 * linked). Multiple threads can call postComplete, which
		 * atomically pops each dependent action, and tries to trigger it
		 * via method tryFire, in NESTED mode.  Triggering can propagate
		 * recursively, so NESTED mode returns its completed dependent (if
		 * one exists) for further processing by its caller (see method
		 * postFire).
		 *
		 * Blocking methods get() and join() rely on Signaller Completions
		 * that wake up waiting threads.  The mechanics are similar to
		 * Treiber stack wait-nodes used in FutureTask, Phaser, and
		 * SynchronousQueue. See their internal documentation for
		 * algorithmic details.
		 *
		 * Without precautions, CompletableFutures would be prone to
		 * garbage accumulation as chains of Completions build up, each
		 * pointing back to its sources. So we null out fields as soon as
		 * possible (see especially method Completion.detach). The
		 * screening checks needed anyway harmlessly ignore null arguments
		 * that may have been obtained during races with threads nulling
		 * out fields.  We also try to unlink fired Completions from
		 * stacks that might never be popped (see method postFire).
		 * Completion fields need not be declared as final or volatile
		 * because they are only visible to other threads upon safe
		 * publication.
		 */

		internal volatile Object Result; // Either the result or boxed AltResult
		internal volatile Completion Stack; // Top of Treiber stack of dependent actions

		internal bool InternalComplete(Object r) // CAS from null to r
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, r);
		}

		internal bool CasStack(Completion cmp, Completion val)
		{
			return UNSAFE.compareAndSwapObject(this, STACK, cmp, val);
		}

		/// <summary>
		/// Returns true if successfully pushed c onto stack. </summary>
		internal bool TryPushStack(Completion c)
		{
			Completion h = Stack;
			LazySetNext(c, h);
			return UNSAFE.compareAndSwapObject(this, STACK, h, c);
		}

		/// <summary>
		/// Unconditionally pushes c onto stack, retrying if necessary. </summary>
		internal void PushStack(Completion c)
		{
			do
			{
			} while (!TryPushStack(c));
		}

		/* ------------- Encoding and decoding outcomes -------------- */

		internal sealed class AltResult // See above
		{
			internal readonly Throwable Ex; // null only for NIL
			internal AltResult(Throwable x)
			{
				this.Ex = x;
			}
		}

		/// <summary>
		/// The encoding of the null value. </summary>
		internal static readonly AltResult NIL = new AltResult(null);

		/// <summary>
		/// Completes with the null value, unless already completed. </summary>
		internal bool CompleteNull()
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, NIL);
		}

		/// <summary>
		/// Returns the encoding of the given non-exceptional value. </summary>
		internal Object EncodeValue(T t)
		{
			return (t == null) ? NIL : t;
		}

		/// <summary>
		/// Completes with a non-exceptional result, unless already completed. </summary>
		internal bool CompleteValue(T t)
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, (t == null) ? NIL : t);
		}

		/// <summary>
		/// Returns the encoding of the given (non-null) exception as a
		/// wrapped CompletionException unless it is one already.
		/// </summary>
		internal static AltResult EncodeThrowable(Throwable x)
		{
			return new AltResult((x is CompletionException) ? x : new CompletionException(x));
		}

		/// <summary>
		/// Completes with an exceptional result, unless already completed. </summary>
		internal bool CompleteThrowable(Throwable x)
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, EncodeThrowable(x));
		}

		/// <summary>
		/// Returns the encoding of the given (non-null) exception as a
		/// wrapped CompletionException unless it is one already.  May
		/// return the given Object r (which must have been the result of a
		/// source future) if it is equivalent, i.e. if this is a simple
		/// relay of an existing CompletionException.
		/// </summary>
		internal static Object EncodeThrowable(Throwable x, Object r)
		{
			if (!(x is CompletionException))
			{
				x = new CompletionException(x);
			}
			else if (r is AltResult && x == ((AltResult)r).Ex)
			{
				return r;
			}
			return new AltResult(x);
		}

		/// <summary>
		/// Completes with the given (non-null) exceptional result as a
		/// wrapped CompletionException unless it is one already, unless
		/// already completed.  May complete with the given Object r
		/// (which must have been the result of a source future) if it is
		/// equivalent, i.e. if this is a simple propagation of an
		/// existing CompletionException.
		/// </summary>
		internal bool CompleteThrowable(Throwable x, Object r)
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, EncodeThrowable(x, r));
		}

		/// <summary>
		/// Returns the encoding of the given arguments: if the exception
		/// is non-null, encodes as AltResult.  Otherwise uses the given
		/// value, boxed as NIL if null.
		/// </summary>
		internal virtual Object EncodeOutcome(T t, Throwable x)
		{
			return (x == null) ? (t == null) ? NIL : t : EncodeThrowable(x);
		}

		/// <summary>
		/// Returns the encoding of a copied outcome; if exceptional,
		/// rewraps as a CompletionException, else returns argument.
		/// </summary>
		internal static Object EncodeRelay(Object r)
		{
			Throwable x;
			return (((r is AltResult) && (x = ((AltResult)r).Ex) != null && !(x is CompletionException)) ? new AltResult(new CompletionException(x)) : r);
		}

		/// <summary>
		/// Completes with r or a copy of r, unless already completed.
		/// If exceptional, r is first coerced to a CompletionException.
		/// </summary>
		internal bool CompleteRelay(Object r)
		{
			return UNSAFE.compareAndSwapObject(this, RESULT, null, EncodeRelay(r));
		}

		/// <summary>
		/// Reports result using Future.get conventions.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <T> T reportGet(Object r) throws InterruptedException, java.util.concurrent.ExecutionException
		private static T reportGet<T>(Object r)
		{
			if (r == null) // by convention below, null means interrupted
			{
				throw new InterruptedException();
			}
			if (r is AltResult)
			{
				Throwable x, cause;
				if ((x = ((AltResult)r).Ex) == null)
				{
					return null;
				}
				if (x is CancellationException)
				{
					throw (CancellationException)x;
				}
				if ((x is CompletionException) && (cause = x.Cause) != null)
				{
					x = cause;
				}
				throw new ExecutionException(x);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T t = (T) r;
			T t = (T) r;
			return t;
		}

		/// <summary>
		/// Decodes outcome to return result or throw unchecked exception.
		/// </summary>
		private static T reportJoin<T>(Object r)
		{
			if (r is AltResult)
			{
				Throwable x;
				if ((x = ((AltResult)r).Ex) == null)
				{
					return null;
				}
				if (x is CancellationException)
				{
					throw (CancellationException)x;
				}
				if (x is CompletionException)
				{
					throw (CompletionException)x;
				}
				throw new CompletionException(x);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T t = (T) r;
			T t = (T) r;
			return t;
		}

		/* ------------- Async task preliminaries -------------- */

		/// <summary>
		/// A marker interface identifying asynchronous tasks produced by
		/// {@code async} methods. This may be useful for monitoring,
		/// debugging, and tracking asynchronous activities.
		/// 
		/// @since 1.8
		/// </summary>
		public interface AsynchronousCompletionTask
		{
		}

		private static readonly bool UseCommonPool = (ForkJoinPool.CommonPoolParallelism > 1);

		/// <summary>
		/// Default executor -- ForkJoinPool.commonPool() unless it cannot
		/// support parallelism.
		/// </summary>
		private static readonly Executor AsyncPool = UseCommonPool ? ForkJoinPool.CommonPool() : new ThreadPerTaskExecutor();

		/// <summary>
		/// Fallback if ForkJoinPool.commonPool() cannot support parallelism </summary>
		internal sealed class ThreadPerTaskExecutor : Executor
		{
			public void Execute(Runnable r)
			{
				(new Thread(r)).Start();
			}
		}

		/// <summary>
		/// Null-checks user executor argument, and translates uses of
		/// commonPool to asyncPool in case parallelism disabled.
		/// </summary>
		internal static Executor ScreenExecutor(Executor e)
		{
			if (!UseCommonPool && e == ForkJoinPool.CommonPool())
			{
				return AsyncPool;
			}
			if (e == null)
			{
				throw new NullPointerException();
			}
			return e;
		}

		// Modes for Completion.tryFire. Signedness matters.
		internal const int SYNC = 0;
		internal const int ASYNC = 1;
		internal const int NESTED = -1;

		/* ------------- Base Completion classes and operations -------------- */

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract static class Completion extends java.util.concurrent.ForkJoinTask<Void> implements Runnable, AsynchronousCompletionTask
		internal abstract class Completion : ForkJoinTask<Void>, Runnable, AsynchronousCompletionTask
		{
			internal volatile Completion Next; // Treiber stack link

			/// <summary>
			/// Performs completion action if triggered, returning a
			/// dependent that may need propagation, if one exists.
			/// </summary>
			/// <param name="mode"> SYNC, ASYNC, or NESTED </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: abstract CompletableFuture<?> tryFire(int mode);
			internal abstract CompletableFuture<?> TryFire(int mode);

			/// <summary>
			/// Returns true if possibly still triggerable. Used by cleanStack. </summary>
			internal abstract bool Live {get;}

			public void Run()
			{
				TryFire(ASYNC);
			}
			public bool Exec()
			{
				TryFire(ASYNC);
				return true;
			}
			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
		}

		internal static void LazySetNext(Completion c, Completion next)
		{
			UNSAFE.putOrderedObject(c, NEXT, next);
		}

		/// <summary>
		/// Pops and tries to trigger all reachable dependents.  Call only
		/// when known to be done.
		/// </summary>
		internal void PostComplete()
		{
			/*
			 * On each step, variable f holds current dependents to pop
			 * and run.  It is extended along only one path at a time,
			 * pushing others to avoid unbounded recursion.
			 */
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> f = this;
			CompletableFuture<?> f = this;
			Completion h;
			while ((h = f.Stack) != null || (f != this && (h = (f = this).stack) != null))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> d;
				CompletableFuture<?> d;
				Completion t;
				if (f.CasStack(h, t = h.Next))
				{
					if (t != null)
					{
						if (f != this)
						{
							PushStack(h);
							continue;
						}
						h.Next = null; // detach
					}
					f = (d = h.TryFire(NESTED)) == null ? this : d;
				}
			}
		}

		/// <summary>
		/// Traverses stack and unlinks dead Completions. </summary>
		internal void CleanStack()
		{
			for (Completion p = null, q = Stack; q != null;)
			{
				Completion s = q.next;
				if (q.Live)
				{
					p = q;
					q = s;
				}
				else if (p == null)
				{
					CasStack(q, s);
					q = Stack;
				}
				else
				{
					p.next = s;
					if (p.Live)
					{
						q = s;
					}
					else
					{
						p = null; // restart
						q = Stack;
					}
				}
			}
		}

		/* ------------- One-input Completions -------------- */

		/// <summary>
		/// A Completion with a source, dependent, and executor. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract static class UniCompletion<T,V> extends Completion
		internal abstract class UniCompletion<T, V> : Completion
		{
			internal Executor Executor; // executor to use (null if none)
			internal CompletableFuture<V> Dep; // the dependent to complete
			internal CompletableFuture<T> Src; // source for action

			internal UniCompletion(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src)
			{
				this.Executor = executor;
				this.Dep = dep;
				this.Src = src;
			}

			/// <summary>
			/// Returns true if action can be run. Call only when known to
			/// be triggerable. Uses FJ tag bit to ensure that only one
			/// thread claims ownership.  If async, starts as task -- a
			/// later call to tryFire will run action.
			/// </summary>
			internal bool Claim()
			{
				Executor e = Executor;
				if (CompareAndSetForkJoinTaskTag((short)0, (short)1))
				{
					if (e == null)
					{
						return true;
					}
					Executor = null; // disable
					e.Execute(this);
				}
				return false;
			}

			internal sealed override bool Live
			{
				get
				{
					return Dep != null;
				}
			}
		}

		/// <summary>
		/// Pushes the given completion (if it exists) unless done. </summary>
		internal void push<T1>(UniCompletion<T1> c)
		{
			if (c != null)
			{
				while (Result == null && !TryPushStack(c))
				{
					LazySetNext(c, null); // clear on failure
				}
			}
		}

		/// <summary>
		/// Post-processing by dependent after successful UniCompletion
		/// tryFire.  Tries to clean stack of source a, and then either runs
		/// postComplete or returns this to caller, depending on mode.
		/// </summary>
		internal CompletableFuture<T> PostFire(CompletableFuture<T1> a, int mode)
		{
			if (a != null && a.Stack != null)
			{
				if (mode < 0 || a.Result == null)
				{
					a.CleanStack();
				}
				else
				{
					a.PostComplete();
				}
			}
			if (Result != null && Stack != null)
			{
				if (mode < 0)
				{
					return this;
				}
				else
				{
					PostComplete();
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniApply<T,V> extends UniCompletion<T,V>
		internal sealed class UniApply<T, V> : UniCompletion<T, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Function<? base T,? extends V> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Function<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniApply(java.util.concurrent.Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, java.util.function.Function<? base T,? extends V> fn)
			internal UniApply<T1>(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, Function<T1> fn) where T1 : V : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<V> TryFire(int mode)
			{
				CompletableFuture<V> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniApply(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <S> boolean uniApply(CompletableFuture<S> a, java.util.function.Function<? base S,? extends T> f, UniApply<S,T> c)
		internal bool uniApply<S, T1>(CompletableFuture<S> a, Function<T1> f, UniApply<S, T> c) where T1 : T
		{
			Object r;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
						goto tryCompleteBreak;
					}
					r = null;
				}
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S s = (S) r;
					S s = (S) r;
					CompleteValue(f.Apply(s));
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <V> CompletableFuture<V> uniApplyStage(java.util.concurrent.Executor e, java.util.function.Function<? base T,? extends V> f)
		private CompletableFuture<V> uniApplyStage<V, T1>(Executor e, Function<T1> f) where T1 : V
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<V> d = new CompletableFuture<V>();
			if (e != null || !d.UniApply(this, f, null))
			{
				UniApply<T, V> c = new UniApply<T, V>(e, d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniAccept<T> extends UniCompletion<T,Void>
		internal sealed class UniAccept<T> : UniCompletion<T, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Consumer<? base T> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Consumer<?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniAccept(java.util.concurrent.Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, java.util.function.Consumer<? base T> fn)
			internal UniAccept<T1>(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, Consumer<T1> fn) : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniAccept(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <S> boolean uniAccept(CompletableFuture<S> a, java.util.function.Consumer<? base S> f, UniAccept<S> c)
		internal bool uniAccept<S, T1>(CompletableFuture<S> a, Consumer<T1> f, UniAccept<S> c)
		{
			Object r;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
						goto tryCompleteBreak;
					}
					r = null;
				}
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S s = (S) r;
					S s = (S) r;
					f.Accept(s);
					CompleteNull();
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private CompletableFuture<Void> uniAcceptStage(java.util.concurrent.Executor e, java.util.function.Consumer<? base T> f)
		private CompletableFuture<Void> UniAcceptStage(Executor e, Consumer<T1> f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.UniAccept(this, f, null))
			{
				UniAccept<T> c = new UniAccept<T>(e, d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniRun<T> extends UniCompletion<T,Void>
		internal sealed class UniRun<T> : UniCompletion<T, Void>
		{
			internal Runnable Fn;
			internal UniRun(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, Runnable fn) : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniRun(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

		internal bool uniRun<T1, T2>(CompletableFuture<T1> a, Runnable f, UniRun<T2> c)
		{
			Object r;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult && (x = ((AltResult)r).Ex) != null)
				{
					CompleteThrowable(x, r);
				}
				else
				{
					try
					{
						if (c != null && !c.Claim())
						{
							return false;
						}
						f.Run();
						CompleteNull();
					}
					catch (Throwable ex)
					{
						CompleteThrowable(ex);
					}
				}
			}
			return true;
		}

		private CompletableFuture<Void> UniRunStage(Executor e, Runnable f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.UniRun(this, f, null))
			{
				UniRun<T> c = new UniRun<T>(e, d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniWhenComplete<T> extends UniCompletion<T,T>
		internal sealed class UniWhenComplete<T> : UniCompletion<T, T>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<? base T, ? base Throwable> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal BiConsumer<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniWhenComplete(java.util.concurrent.Executor executor, CompletableFuture<T> dep, CompletableFuture<T> src, java.util.function.BiConsumer<? base T, ? base Throwable> fn)
			internal UniWhenComplete<T1>(Executor executor, CompletableFuture<T> dep, CompletableFuture<T> src, BiConsumer<T1> fn) : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<T> TryFire(int mode)
			{
				CompletableFuture<T> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniWhenComplete(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final boolean uniWhenComplete(CompletableFuture<T> a, java.util.function.BiConsumer<? base T,? base Throwable> f, UniWhenComplete<T> c)
		internal bool uniWhenComplete<T1>(CompletableFuture<T> a, BiConsumer<T1> f, UniWhenComplete<T> c)
		{
			Object r;
			T t;
			Throwable x = null;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
					if (r is AltResult)
					{
						x = ((AltResult)r).Ex;
						t = null;
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T tr = (T) r;
						T tr = (T) r;
						t = tr;
					}
					f.Accept(t, x);
					if (x == null)
					{
						InternalComplete(r);
						return true;
					}
				}
				catch (Throwable ex)
				{
					if (x == null)
					{
						x = ex;
					}
				}
				CompleteThrowable(x, r);
			}
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private CompletableFuture<T> uniWhenCompleteStage(java.util.concurrent.Executor e, java.util.function.BiConsumer<? base T, ? base Throwable> f)
		private CompletableFuture<T> UniWhenCompleteStage(Executor e, BiConsumer<T1> f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<T> d = new CompletableFuture<T>();
			if (e != null || !d.UniWhenComplete(this, f, null))
			{
				UniWhenComplete<T> c = new UniWhenComplete<T>(e, d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniHandle<T,V> extends UniCompletion<T,V>
		internal sealed class UniHandle<T, V> : UniCompletion<T, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiFunction<? base T, Throwable, ? extends V> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal BiFunction<?, Throwable, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniHandle(java.util.concurrent.Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, java.util.function.BiFunction<? base T, Throwable, ? extends V> fn)
			internal UniHandle<T1>(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, BiFunction<T1> fn) where T1 : V : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<V> TryFire(int mode)
			{
				CompletableFuture<V> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniHandle(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <S> boolean uniHandle(CompletableFuture<S> a, java.util.function.BiFunction<? base S, Throwable, ? extends T> f, UniHandle<S,T> c)
		internal bool uniHandle<S, T1>(CompletableFuture<S> a, BiFunction<T1> f, UniHandle<S, T> c) where T1 : T
		{
			Object r;
			S s;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
					if (r is AltResult)
					{
						x = ((AltResult)r).Ex;
						s = null;
					}
					else
					{
						x = null;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S ss = (S) r;
						S ss = (S) r;
						s = ss;
					}
					CompleteValue(f.Apply(s, x));
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <V> CompletableFuture<V> uniHandleStage(java.util.concurrent.Executor e, java.util.function.BiFunction<? base T, Throwable, ? extends V> f)
		private CompletableFuture<V> uniHandleStage<V, T1>(Executor e, BiFunction<T1> f) where T1 : V
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<V> d = new CompletableFuture<V>();
			if (e != null || !d.UniHandle(this, f, null))
			{
				UniHandle<T, V> c = new UniHandle<T, V>(e, d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniExceptionally<T> extends UniCompletion<T,T>
		internal sealed class UniExceptionally<T> : UniCompletion<T, T>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Function<? base Throwable, ? extends T> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Function<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniExceptionally(CompletableFuture<T> dep, CompletableFuture<T> src, java.util.function.Function<? base Throwable, ? extends T> fn)
			internal UniExceptionally<T1>(CompletableFuture<T> dep, CompletableFuture<T> src, Function<T1> fn) where T1 : T : base(null, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<T> TryFire(int mode) // never ASYNC
			{
				// assert mode != ASYNC;
				CompletableFuture<T> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniExceptionally(a = Src, Fn, this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final boolean uniExceptionally(CompletableFuture<T> a, java.util.function.Function<? base Throwable, ? extends T> f, UniExceptionally<T> c)
		internal bool uniExceptionally<T1>(CompletableFuture<T> a, Function<T1> f, UniExceptionally<T> c) where T1 : T
		{
			Object r;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (r is AltResult && (x = ((AltResult)r).Ex) != null)
					{
						if (c != null && !c.Claim())
						{
							return false;
						}
						CompleteValue(f.Apply(x));
					}
					else
					{
						InternalComplete(r);
					}
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			return true;
		}

		private CompletableFuture<T> UniExceptionallyStage(Function<T1> f) where T1 : T
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<T> d = new CompletableFuture<T>();
			if (!d.UniExceptionally(this, f, null))
			{
				UniExceptionally<T> c = new UniExceptionally<T>(d, this, f);
				Push(c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniRelay<T> extends UniCompletion<T,T>
		internal sealed class UniRelay<T> : UniCompletion<T, T> // for Compose
		{
			internal UniRelay(CompletableFuture<T> dep, CompletableFuture<T> src) : base(null, dep, src)
			{
			}
			internal CompletableFuture<T> TryFire(int mode)
			{
				CompletableFuture<T> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniRelay(a = Src))
				{
					return null;
				}
				Src = null;
				Dep = null;
				return d.PostFire(a, mode);
			}
		}

		internal bool UniRelay(CompletableFuture<T> a)
		{
			Object r;
			if (a == null || (r = a.Result) == null)
			{
				return false;
			}
			if (Result == null) // no need to claim
			{
				CompleteRelay(r);
			}
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class UniCompose<T,V> extends UniCompletion<T,V>
		internal sealed class UniCompose<T, V> : UniCompletion<T, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<V>> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Function<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: UniCompose(java.util.concurrent.Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<V>> fn)
			internal UniCompose<T1>(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, Function<T1> fn) where T1 : java.util.concurrent.CompletionStage<V> : base(executor, dep, src)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<V> TryFire(int mode)
			{
				CompletableFuture<V> d;
				CompletableFuture<T> a;
				if ((d = Dep) == null || !d.UniCompose(a = Src, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Fn = null;
				return d.PostFire(a, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <S> boolean uniCompose(CompletableFuture<S> a, java.util.function.Function<? base S, ? extends java.util.concurrent.CompletionStage<T>> f, UniCompose<S,T> c)
		internal bool uniCompose<S, T1>(CompletableFuture<S> a, Function<T1> f, UniCompose<S, T> c) where T1 : java.util.concurrent.CompletionStage<T>
		{
			Object r;
			Throwable x;
			if (a == null || (r = a.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
						goto tryCompleteBreak;
					}
					r = null;
				}
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S s = (S) r;
					S s = (S) r;
					CompletableFuture<T> g = f.Apply(s).toCompletableFuture();
					if (g.Result == null || !UniRelay(g))
					{
						UniRelay<T> copy = new UniRelay<T>(this, g);
						g.Push(copy);
						copy.TryFire(SYNC);
						if (Result == null)
						{
							return false;
						}
					}
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <V> CompletableFuture<V> uniComposeStage(java.util.concurrent.Executor e, java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<V>> f)
		private CompletableFuture<V> uniComposeStage<V, T1>(Executor e, Function<T1> f) where T1 : java.util.concurrent.CompletionStage<V>
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			Object r;
			Throwable x;
			if (e == null && (r = Result) != null)
			{
				// try to return function result directly
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						return new CompletableFuture<V>(EncodeThrowable(x, r));
					}
					r = null;
				}
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T t = (T) r;
					T t = (T) r;
					CompletableFuture<V> g = f.Apply(t).toCompletableFuture();
					Object s = g.Result;
					if (s != null)
					{
						return new CompletableFuture<V>(EncodeRelay(s));
					}
					CompletableFuture<V> d = new CompletableFuture<V>();
					UniRelay<V> copy = new UniRelay<V>(d, g);
					g.Push(copy);
					copy.TryFire(SYNC);
					return d;
				}
				catch (Throwable ex)
				{
					return new CompletableFuture<V>(EncodeThrowable(ex));
				}
			}
			CompletableFuture<V> d = new CompletableFuture<V>();
			UniCompose<T, V> c = new UniCompose<T, V>(e, d, this, f);
			Push(c);
			c.TryFire(SYNC);
			return d;
		}

		/* ------------- Two-input Completions -------------- */

		/// <summary>
		/// A Completion for an action with two sources </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract static class BiCompletion<T,U,V> extends UniCompletion<T,V>
		internal abstract class BiCompletion<T, U, V> : UniCompletion<T, V>
		{
			internal CompletableFuture<U> Snd; // second source for action
			internal BiCompletion(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, CompletableFuture<U> snd) : base(executor, dep, src)
			{
				this.Snd = snd;
			}
		}

		/// <summary>
		/// A Completion delegating to a BiCompletion </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class CoCompletion extends Completion
		internal sealed class CoCompletion : Completion
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: BiCompletion<?,?,?> base;
			internal BiCompletion<?, ?, ?> @base;
			internal CoCompletion<T1>(BiCompletion<T1> @base)
			{
				this.@base = @base;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final CompletableFuture<?> tryFire(int mode)
			internal override CompletableFuture<?> TryFire(int mode)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: BiCompletion<?,?,?> c;
				BiCompletion<?, ?, ?> c;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> d;
				CompletableFuture<?> d;
				if ((c = @base) == null || (d = c.TryFire(mode)) == null)
				{
					return null;
				}
				@base = null; // detach
				return d;
			}
			internal override bool Live
			{
				get
				{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: BiCompletion<?,?,?> c;
					BiCompletion<?, ?, ?> c;
					return (c = @base) != null && c.Dep != null;
				}
			}
		}

		/// <summary>
		/// Pushes completion to this and b unless both done. </summary>
		internal void bipush<T1, T2>(CompletableFuture<T1> b, BiCompletion<T2> c)
		{
			if (c != null)
			{
				Object r;
				while ((r = Result) == null && !TryPushStack(c))
				{
					LazySetNext(c, null); // clear on failure
				}
				if (b != null && b != this && b.Result == null)
				{
					Completion q = (r != null) ? c : new CoCompletion(c);
					while (b.Result == null && !b.TryPushStack(q))
					{
						LazySetNext(q, null); // clear on failure
					}
				}
			}
		}

		/// <summary>
		/// Post-processing after successful BiCompletion tryFire. </summary>
		internal CompletableFuture<T> PostFire(CompletableFuture<T1> a, CompletableFuture<T2> b, int mode)
		{
			if (b != null && b.Stack != null) // clean second source
			{
				if (mode < 0 || b.Result == null)
				{
					b.CleanStack();
				}
				else
				{
					b.PostComplete();
				}
			}
			return PostFire(a, mode);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class BiApply<T,U,V> extends BiCompletion<T,U,V>
		internal sealed class BiApply<T, U, V> : BiCompletion<T, U, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiFunction<? base T,? base U,? extends V> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal BiFunction<?, ?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: BiApply(java.util.concurrent.Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, CompletableFuture<U> snd, java.util.function.BiFunction<? base T,? base U,? extends V> fn)
			internal BiApply<T1>(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, CompletableFuture<U> snd, BiFunction<T1> fn) where T1 : V : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<V> TryFire(int mode)
			{
				CompletableFuture<V> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.BiApply(a = Src, b = Snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <R,S> boolean biApply(CompletableFuture<R> a, CompletableFuture<S> b, java.util.function.BiFunction<? base R,? base S,? extends T> f, BiApply<R,S,T> c)
		internal bool biApply<R, S, T1>(CompletableFuture<R> a, CompletableFuture<S> b, BiFunction<T1> f, BiApply<R, S, T> c) where T1 : T
		{
			Object r, s;
			Throwable x;
			if (a == null || (r = a.Result) == null || b == null || (s = b.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
						goto tryCompleteBreak;
					}
					r = null;
				}
				if (s is AltResult)
				{
					if ((x = ((AltResult)s).Ex) != null)
					{
						CompleteThrowable(x, s);
						goto tryCompleteBreak;
					}
					s = null;
				}
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") R rr = (R) r;
					R rr = (R) r;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S ss = (S) s;
					S ss = (S) s;
					CompleteValue(f.Apply(rr, ss));
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <U,V> CompletableFuture<V> biApplyStage(java.util.concurrent.Executor e, java.util.concurrent.CompletionStage<U> o, java.util.function.BiFunction<? base T,? base U,? extends V> f)
		private CompletableFuture<V> biApplyStage<U, V, T1>(Executor e, CompletionStage<U> o, BiFunction<T1> f) where T1 : V
		{
			CompletableFuture<U> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<V> d = new CompletableFuture<V>();
			if (e != null || !d.BiApply(this, b, f, null))
			{
				BiApply<T, U, V> c = new BiApply<T, U, V>(e, d, this, b, f);
				Bipush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class BiAccept<T,U> extends BiCompletion<T,U,Void>
		internal sealed class BiAccept<T, U> : BiCompletion<T, U, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.BiConsumer<? base T,? base U> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal BiConsumer<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: BiAccept(java.util.concurrent.Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, java.util.function.BiConsumer<? base T,? base U> fn)
			internal BiAccept<T1>(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, BiConsumer<T1> fn) : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.BiAccept(a = Src, b = Snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <R,S> boolean biAccept(CompletableFuture<R> a, CompletableFuture<S> b, java.util.function.BiConsumer<? base R,? base S> f, BiAccept<R,S> c)
		internal bool biAccept<R, S, T1>(CompletableFuture<R> a, CompletableFuture<S> b, BiConsumer<T1> f, BiAccept<R, S> c)
		{
			Object r, s;
			Throwable x;
			if (a == null || (r = a.Result) == null || b == null || (s = b.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult)
				{
					if ((x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
						goto tryCompleteBreak;
					}
					r = null;
				}
				if (s is AltResult)
				{
					if ((x = ((AltResult)s).Ex) != null)
					{
						CompleteThrowable(x, s);
						goto tryCompleteBreak;
					}
					s = null;
				}
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") R rr = (R) r;
					R rr = (R) r;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") S ss = (S) s;
					S ss = (S) s;
					f.Accept(rr, ss);
					CompleteNull();
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <U> CompletableFuture<Void> biAcceptStage(java.util.concurrent.Executor e, java.util.concurrent.CompletionStage<U> o, java.util.function.BiConsumer<? base T,? base U> f)
		private CompletableFuture<Void> biAcceptStage<U, T1>(Executor e, CompletionStage<U> o, BiConsumer<T1> f)
		{
			CompletableFuture<U> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.BiAccept(this, b, f, null))
			{
				BiAccept<T, U> c = new BiAccept<T, U>(e, d, this, b, f);
				Bipush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class BiRun<T,U> extends BiCompletion<T,U,Void>
		internal sealed class BiRun<T, U> : BiCompletion<T, U, Void>
		{
			internal Runnable Fn;
			internal BiRun(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, Runnable fn) : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.BiRun(a = Src, b = Snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

		internal bool biRun<T1, T2, T3>(CompletableFuture<T1> a, CompletableFuture<T2> b, Runnable f, BiRun<T3> c)
		{
			Object r, s;
			Throwable x;
			if (a == null || (r = a.Result) == null || b == null || (s = b.Result) == null || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult && (x = ((AltResult)r).Ex) != null)
				{
					CompleteThrowable(x, r);
				}
				else if (s is AltResult && (x = ((AltResult)s).Ex) != null)
				{
					CompleteThrowable(x, s);
				}
				else
				{
					try
					{
						if (c != null && !c.Claim())
						{
							return false;
						}
						f.Run();
						CompleteNull();
					}
					catch (Throwable ex)
					{
						CompleteThrowable(ex);
					}
				}
			}
			return true;
		}

		private CompletableFuture<Void> BiRunStage(Executor e, CompletionStage<T1> o, Runnable f)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> b;
			CompletableFuture<?> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.BiRun(this, b, f, null))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: BiRun<T,?> c = new BiRun<>(e, d, this, b, f);
				BiRun<T, ?> c = new BiRun<T, ?>(e, d, this, b, f);
				Bipush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class BiRelay<T,U> extends BiCompletion<T,U,Void>
		internal sealed class BiRelay<T, U> : BiCompletion<T, U, Void> // for And
		{
			internal BiRelay(CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd) : base(null, dep, src, snd)
			{
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.BiRelay(a = Src, b = Snd))
				{
					return null;
				}
				Src = null;
				Snd = null;
				Dep = null;
				return d.PostFire(a, b, mode);
			}
		}

		internal virtual bool biRelay<T1, T2>(CompletableFuture<T1> a, CompletableFuture<T2> b)
		{
			Object r, s;
			Throwable x;
			if (a == null || (r = a.Result) == null || b == null || (s = b.Result) == null)
			{
				return false;
			}
			if (Result == null)
			{
				if (r is AltResult && (x = ((AltResult)r).Ex) != null)
				{
					CompleteThrowable(x, r);
				}
				else if (s is AltResult && (x = ((AltResult)s).Ex) != null)
				{
					CompleteThrowable(x, s);
				}
				else
				{
					CompleteNull();
				}
			}
			return true;
		}

		/// <summary>
		/// Recursively constructs a tree of completions. </summary>
		internal static CompletableFuture<Void> AndTree(CompletableFuture<T1>[] cfs, int lo, int hi)
		{
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (lo > hi) // empty
			{
				d.Result = NIL;
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> a, b;
				CompletableFuture<?> a, b;
				int mid = (int)((uint)(lo + hi) >> 1);
				if ((a = (lo == mid ? cfs[lo] : AndTree(cfs, lo, mid))) == null || (b = (lo == hi ? a : (hi == mid + 1) ? cfs[hi] : AndTree(cfs, mid + 1, hi))) == null)
				{
					throw new NullPointerException();
				}
				if (!d.BiRelay(a, b))
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: BiRelay<?,?> c = new BiRelay<>(d, a, b);
					BiRelay<?, ?> c = new BiRelay<?, ?>(d, a, b);
					a.Bipush(b, c);
					c.TryFire(SYNC);
				}
			}
			return d;
		}

		/* ------------- Projected (Ored) BiCompletions -------------- */

		/// <summary>
		/// Pushes completion to this and b unless either done. </summary>
		internal void orpush<T1, T2>(CompletableFuture<T1> b, BiCompletion<T2> c)
		{
			if (c != null)
			{
				while ((b == null || b.Result == null) && Result == null)
				{
					if (TryPushStack(c))
					{
						if (b != null && b != this && b.Result == null)
						{
							Completion q = new CoCompletion(c);
							while (Result == null && b.Result == null && !b.TryPushStack(q))
							{
								LazySetNext(q, null); // clear on failure
							}
						}
						break;
					}
					LazySetNext(c, null); // clear on failure
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OrApply<T,U extends T,V> extends BiCompletion<T,U,V>
		internal sealed class OrApply<T, U, V> : BiCompletion<T, U, V> where U : T
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Function<? base T,? extends V> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Function<?, ?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: OrApply(java.util.concurrent.Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, CompletableFuture<U> snd, java.util.function.Function<? base T,? extends V> fn)
			internal OrApply<T1>(Executor executor, CompletableFuture<V> dep, CompletableFuture<T> src, CompletableFuture<U> snd, Function<T1> fn) where T1 : V : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<V> TryFire(int mode)
			{
				CompletableFuture<V> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = dep) == null || !d.OrApply(a = src, b = snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				dep = null;
				src = null;
				snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <R,S extends R> boolean orApply(CompletableFuture<R> a, CompletableFuture<S> b, java.util.function.Function<? base R, ? extends T> f, OrApply<R,S,T> c)
		internal bool orApply<R, S, T1>(CompletableFuture<R> a, CompletableFuture<S> b, Function<T1> f, OrApply<R, S, T> c) where S : R where T1 : T
		{
			Object r;
			Throwable x;
			if (a == null || b == null || ((r = a.Result) == null && (r = b.Result) == null) || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (c != null && !c.claim())
					{
						return false;
					}
					if (r is AltResult)
					{
						if ((x = ((AltResult)r).Ex) != null)
						{
							CompleteThrowable(x, r);
							goto tryCompleteBreak;
						}
						r = null;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") R rr = (R) r;
					R rr = (R) r;
					CompleteValue(f.Apply(rr));
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <U extends T,V> CompletableFuture<V> orApplyStage(java.util.concurrent.Executor e, java.util.concurrent.CompletionStage<U> o, java.util.function.Function<? base T, ? extends V> f)
		private CompletableFuture<V> orApplyStage<U, V, T1>(Executor e, CompletionStage<U> o, Function<T1> f) where U : T where T1 : V
		{
			CompletableFuture<U> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<V> d = new CompletableFuture<V>();
			if (e != null || !d.OrApply(this, b, f, null))
			{
				OrApply<T, U, V> c = new OrApply<T, U, V>(e, d, this, b, f);
				Orpush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OrAccept<T,U extends T> extends BiCompletion<T,U,Void>
		internal sealed class OrAccept<T, U> : BiCompletion<T, U, Void> where U : T
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.function.Consumer<? base T> fn;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Consumer<?> Fn;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: OrAccept(java.util.concurrent.Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, java.util.function.Consumer<? base T> fn)
			internal OrAccept<T1>(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, Consumer<T1> fn) : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = dep) == null || !d.OrAccept(a = src, b = snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				dep = null;
				src = null;
				snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final <R,S extends R> boolean orAccept(CompletableFuture<R> a, CompletableFuture<S> b, java.util.function.Consumer<? base R> f, OrAccept<R,S> c)
		internal bool orAccept<R, S, T1>(CompletableFuture<R> a, CompletableFuture<S> b, Consumer<T1> f, OrAccept<R, S> c) where S : R
		{
			Object r;
			Throwable x;
			if (a == null || b == null || ((r = a.Result) == null && (r = b.Result) == null) || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (c != null && !c.claim())
					{
						return false;
					}
					if (r is AltResult)
					{
						if ((x = ((AltResult)r).Ex) != null)
						{
							CompleteThrowable(x, r);
							goto tryCompleteBreak;
						}
						r = null;
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") R rr = (R) r;
					R rr = (R) r;
					f.Accept(rr);
					CompleteNull();
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			tryCompleteBreak:
			return true;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private <U extends T> CompletableFuture<Void> orAcceptStage(java.util.concurrent.Executor e, java.util.concurrent.CompletionStage<U> o, java.util.function.Consumer<? base T> f)
		private CompletableFuture<Void> orAcceptStage<U, T1>(Executor e, CompletionStage<U> o, Consumer<T1> f) where U : T
		{
			CompletableFuture<U> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.OrAccept(this, b, f, null))
			{
				OrAccept<T, U> c = new OrAccept<T, U>(e, d, this, b, f);
				Orpush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OrRun<T,U> extends BiCompletion<T,U,Void>
		internal sealed class OrRun<T, U> : BiCompletion<T, U, Void>
		{
			internal Runnable Fn;
			internal OrRun(Executor executor, CompletableFuture<Void> dep, CompletableFuture<T> src, CompletableFuture<U> snd, Runnable fn) : base(executor, dep, src, snd)
			{
				this.Fn = fn;
			}
			internal CompletableFuture<Void> TryFire(int mode)
			{
				CompletableFuture<Void> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.OrRun(a = Src, b = Snd, Fn, mode > 0 ? null : this))
				{
					return null;
				}
				Dep = null;
				Src = null;
				Snd = null;
				Fn = null;
				return d.PostFire(a, b, mode);
			}
		}

		internal bool orRun<T1, T2, T3>(CompletableFuture<T1> a, CompletableFuture<T2> b, Runnable f, OrRun<T3> c)
		{
			Object r;
			Throwable x;
			if (a == null || b == null || ((r = a.Result) == null && (r = b.Result) == null) || f == null)
			{
				return false;
			}
			if (Result == null)
			{
				try
				{
					if (c != null && !c.Claim())
					{
						return false;
					}
					if (r is AltResult && (x = ((AltResult)r).Ex) != null)
					{
						CompleteThrowable(x, r);
					}
					else
					{
						f.Run();
						CompleteNull();
					}
				}
				catch (Throwable ex)
				{
					CompleteThrowable(ex);
				}
			}
			return true;
		}

		private CompletableFuture<Void> OrRunStage(Executor e, CompletionStage<T1> o, Runnable f)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> b;
			CompletableFuture<?> b;
			if (f == null || (b = o.ToCompletableFuture()) == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			if (e != null || !d.OrRun(this, b, f, null))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: OrRun<T,?> c = new OrRun<>(e, d, this, b, f);
				OrRun<T, ?> c = new OrRun<T, ?>(e, d, this, b, f);
				Orpush(b, c);
				c.TryFire(SYNC);
			}
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class OrRelay<T,U> extends BiCompletion<T,U,Object>
		internal sealed class OrRelay<T, U> : BiCompletion<T, U, Object> // for Or
		{
			internal OrRelay(CompletableFuture<Object> dep, CompletableFuture<T> src, CompletableFuture<U> snd) : base(null, dep, src, snd)
			{
			}
			internal CompletableFuture<Object> TryFire(int mode)
			{
				CompletableFuture<Object> d;
				CompletableFuture<T> a;
				CompletableFuture<U> b;
				if ((d = Dep) == null || !d.OrRelay(a = Src, b = Snd))
				{
					return null;
				}
				Src = null;
				Snd = null;
				Dep = null;
				return d.PostFire(a, b, mode);
			}
		}

		internal bool orRelay<T1, T2>(CompletableFuture<T1> a, CompletableFuture<T2> b)
		{
			Object r;
			if (a == null || b == null || ((r = a.Result) == null && (r = b.Result) == null))
			{
				return false;
			}
			if (Result == null)
			{
				CompleteRelay(r);
			}
			return true;
		}

		/// <summary>
		/// Recursively constructs a tree of completions. </summary>
		internal static CompletableFuture<Object> OrTree(CompletableFuture<T1>[] cfs, int lo, int hi)
		{
			CompletableFuture<Object> d = new CompletableFuture<Object>();
			if (lo <= hi)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CompletableFuture<?> a, b;
				CompletableFuture<?> a, b;
				int mid = (int)((uint)(lo + hi) >> 1);
				if ((a = (lo == mid ? cfs[lo] : OrTree(cfs, lo, mid))) == null || (b = (lo == hi ? a : (hi == mid + 1) ? cfs[hi] : OrTree(cfs, mid + 1, hi))) == null)
				{
					throw new NullPointerException();
				}
				if (!d.OrRelay(a, b))
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: OrRelay<?,?> c = new OrRelay<>(d, a, b);
					OrRelay<?, ?> c = new OrRelay<?, ?>(d, a, b);
					a.Orpush(b, c);
					c.TryFire(SYNC);
				}
			}
			return d;
		}

		/* ------------- Zero-input Async forms -------------- */

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class AsyncSupply<T> extends java.util.concurrent.ForkJoinTask<Void> implements Runnable, AsynchronousCompletionTask
		internal sealed class AsyncSupply<T> : ForkJoinTask<Void>, Runnable, AsynchronousCompletionTask
		{
			internal CompletableFuture<T> Dep;
			internal Supplier<T> Fn;
			internal AsyncSupply(CompletableFuture<T> dep, Supplier<T> fn)
			{
				this.Dep = dep;
				this.Fn = fn;
			}

			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
			public bool Exec()
			{
				Run();
				return true;
			}

			public void Run()
			{
				CompletableFuture<T> d;
				Supplier<T> f;
				if ((d = Dep) != null && (f = Fn) != null)
				{
					Dep = null;
					Fn = null;
					if (d.Result == null)
					{
						try
						{
							d.CompleteValue(f.Get());
						}
						catch (Throwable ex)
						{
							d.CompleteThrowable(ex);
						}
					}
					d.PostComplete();
				}
			}
		}

		internal static CompletableFuture<U> asyncSupplyStage<U>(Executor e, Supplier<U> f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<U> d = new CompletableFuture<U>();
			e.Execute(new AsyncSupply<U>(d, f));
			return d;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class AsyncRun extends java.util.concurrent.ForkJoinTask<Void> implements Runnable, AsynchronousCompletionTask
		internal sealed class AsyncRun : ForkJoinTask<Void>, Runnable, AsynchronousCompletionTask
		{
			internal CompletableFuture<Void> Dep;
			internal Runnable Fn;
			internal AsyncRun(CompletableFuture<Void> dep, Runnable fn)
			{
				this.Dep = dep;
				this.Fn = fn;
			}

			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
			public bool Exec()
			{
				Run();
				return true;
			}

			public void Run()
			{
				CompletableFuture<Void> d;
				Runnable f;
				if ((d = Dep) != null && (f = Fn) != null)
				{
					Dep = null;
					Fn = null;
					if (d.Result == null)
					{
						try
						{
							f.Run();
							d.CompleteNull();
						}
						catch (Throwable ex)
						{
							d.CompleteThrowable(ex);
						}
					}
					d.PostComplete();
				}
			}
		}

		internal static CompletableFuture<Void> AsyncRunStage(Executor e, Runnable f)
		{
			if (f == null)
			{
				throw new NullPointerException();
			}
			CompletableFuture<Void> d = new CompletableFuture<Void>();
			e.Execute(new AsyncRun(d, f));
			return d;
		}

		/* ------------- Signallers -------------- */

		/// <summary>
		/// Completion for recording and releasing a waiting thread.  This
		/// class implements ManagedBlocker to avoid starvation when
		/// blocking actions pile up in ForkJoinPools.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class Signaller extends Completion implements java.util.concurrent.ForkJoinPool.ManagedBlocker
		internal sealed class Signaller : Completion, ForkJoinPool.ManagedBlocker
		{
			internal long Nanos; // wait time if timed
			internal readonly long Deadline; // non-zero if timed
			internal volatile int InterruptControl; // > 0: interruptible, < 0: interrupted
			internal volatile Thread Thread;

			internal Signaller(bool interruptible, long nanos, long deadline)
			{
				this.Thread = Thread.CurrentThread;
				this.InterruptControl = interruptible ? 1 : 0;
				this.Nanos = nanos;
				this.Deadline = deadline;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final CompletableFuture<?> tryFire(int ignore)
			internal override CompletableFuture<?> TryFire(int ignore)
			{
				Thread w; // no need to atomically claim
				if ((w = Thread) != null)
				{
					Thread = null;
					LockSupport.Unpark(w);
				}
				return null;
			}
			public bool Releasable
			{
				get
				{
					if (Thread == null)
					{
						return true;
					}
					if (Thread.Interrupted())
					{
						int i = InterruptControl;
						InterruptControl = -1;
						if (i > 0)
						{
							return true;
						}
					}
					if (Deadline != 0L && (Nanos <= 0L || (Nanos = Deadline - System.nanoTime()) <= 0L))
					{
						Thread = null;
						return true;
					}
					return false;
				}
			}
			public bool Block()
			{
				if (Releasable)
				{
					return true;
				}
				else if (Deadline == 0L)
				{
					LockSupport.Park(this);
				}
				else if (Nanos > 0L)
				{
					LockSupport.ParkNanos(this, Nanos);
				}
				return Releasable;
			}
			internal override bool Live
			{
				get
				{
					return Thread != null;
				}
			}
		}

		/// <summary>
		/// Returns raw result after waiting, or null if interruptible and
		/// interrupted.
		/// </summary>
		private Object WaitingGet(bool interruptible)
		{
			Signaller q = null;
			bool queued = false;
			int spins = -1;
			Object r;
			while ((r = Result) == null)
			{
				if (spins < 0)
				{
					spins = (Runtime.Runtime.availableProcessors() > 1) ? 1 << 8 : 0; // Use brief spin-wait on multiprocessors
				}
				else if (spins > 0)
				{
					if (ThreadLocalRandom.NextSecondarySeed() >= 0)
					{
						--spins;
					}
				}
				else if (q == null)
				{
					q = new Signaller(interruptible, 0L, 0L);
				}
				else if (!queued)
				{
					queued = TryPushStack(q);
				}
				else if (interruptible && q.InterruptControl < 0)
				{
					q.Thread = null;
					CleanStack();
					return null;
				}
				else if (q.Thread != null && Result == null)
				{
					try
					{
						ForkJoinPool.ManagedBlock(q);
					}
					catch (InterruptedException)
					{
						q.InterruptControl = -1;
					}
				}
			}
			if (q != null)
			{
				q.Thread = null;
				if (q.InterruptControl < 0)
				{
					if (interruptible)
					{
						r = null; // report interruption
					}
					else
					{
						Thread.CurrentThread.Interrupt();
					}
				}
			}
			PostComplete();
			return r;
		}

		/// <summary>
		/// Returns raw result after waiting, or null if interrupted, or
		/// throws TimeoutException on timeout.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object timedGet(long nanos) throws java.util.concurrent.TimeoutException
		private Object TimedGet(long nanos)
		{
			if (Thread.Interrupted())
			{
				return null;
			}
			if (nanos <= 0L)
			{
				throw new TimeoutException();
			}
			long d = System.nanoTime() + nanos;
			Signaller q = new Signaller(true, nanos, d == 0L ? 1L : d); // avoid 0
			bool queued = false;
			Object r;
			// We intentionally don't spin here (as waitingGet does) because
			// the call to nanoTime() above acts much like a spin.
			while ((r = Result) == null)
			{
				if (!queued)
				{
					queued = TryPushStack(q);
				}
				else if (q.InterruptControl < 0 || q.Nanos <= 0L)
				{
					q.Thread = null;
					CleanStack();
					if (q.InterruptControl < 0)
					{
						return null;
					}
					throw new TimeoutException();
				}
				else if (q.Thread != null && Result == null)
				{
					try
					{
						ForkJoinPool.ManagedBlock(q);
					}
					catch (InterruptedException)
					{
						q.InterruptControl = -1;
					}
				}
			}
			if (q.InterruptControl < 0)
			{
				r = null;
			}
			q.Thread = null;
			PostComplete();
			return r;
		}

		/* ------------- public methods -------------- */

		/// <summary>
		/// Creates a new incomplete CompletableFuture.
		/// </summary>
		public CompletableFuture()
		{
		}

		/// <summary>
		/// Creates a new complete CompletableFuture with given encoded result.
		/// </summary>
		private CompletableFuture(Object r)
		{
			this.Result = r;
		}

		/// <summary>
		/// Returns a new CompletableFuture that is asynchronously completed
		/// by a task running in the <seealso cref="ForkJoinPool#commonPool()"/> with
		/// the value obtained by calling the given Supplier.
		/// </summary>
		/// <param name="supplier"> a function returning the value to be used
		/// to complete the returned CompletableFuture </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletableFuture </returns>
		public static CompletableFuture<U> supplyAsync<U>(Supplier<U> supplier)
		{
			return AsyncSupplyStage(AsyncPool, supplier);
		}

		/// <summary>
		/// Returns a new CompletableFuture that is asynchronously completed
		/// by a task running in the given executor with the value obtained
		/// by calling the given Supplier.
		/// </summary>
		/// <param name="supplier"> a function returning the value to be used
		/// to complete the returned CompletableFuture </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletableFuture </returns>
		public static CompletableFuture<U> supplyAsync<U>(Supplier<U> supplier, Executor executor)
		{
			return AsyncSupplyStage(ScreenExecutor(executor), supplier);
		}

		/// <summary>
		/// Returns a new CompletableFuture that is asynchronously completed
		/// by a task running in the <seealso cref="ForkJoinPool#commonPool()"/> after
		/// it runs the given action.
		/// </summary>
		/// <param name="runnable"> the action to run before completing the
		/// returned CompletableFuture </param>
		/// <returns> the new CompletableFuture </returns>
		public static CompletableFuture<Void> RunAsync(Runnable runnable)
		{
			return AsyncRunStage(AsyncPool, runnable);
		}

		/// <summary>
		/// Returns a new CompletableFuture that is asynchronously completed
		/// by a task running in the given executor after it runs the given
		/// action.
		/// </summary>
		/// <param name="runnable"> the action to run before completing the
		/// returned CompletableFuture </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletableFuture </returns>
		public static CompletableFuture<Void> RunAsync(Runnable runnable, Executor executor)
		{
			return AsyncRunStage(ScreenExecutor(executor), runnable);
		}

		/// <summary>
		/// Returns a new CompletableFuture that is already completed with
		/// the given value.
		/// </summary>
		/// <param name="value"> the value </param>
		/// @param <U> the type of the value </param>
		/// <returns> the completed CompletableFuture </returns>
		public static CompletableFuture<U> completedFuture<U>(U value)
		{
			return new CompletableFuture<U>((value == null) ? NIL : value);
		}

		/// <summary>
		/// Returns {@code true} if completed in any fashion: normally,
		/// exceptionally, or via cancellation.
		/// </summary>
		/// <returns> {@code true} if completed </returns>
		public virtual bool Done
		{
			get
			{
				return Result != null;
			}
		}

		/// <summary>
		/// Waits if necessary for this future to complete, and then
		/// returns its result.
		/// </summary>
		/// <returns> the result value </returns>
		/// <exception cref="CancellationException"> if this future was cancelled </exception>
		/// <exception cref="ExecutionException"> if this future completed exceptionally </exception>
		/// <exception cref="InterruptedException"> if the current thread was interrupted
		/// while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T get() throws InterruptedException, java.util.concurrent.ExecutionException
		public virtual T Get()
		{
			Object r;
			return ReportGet((r = Result) == null ? WaitingGet(true) : r);
		}

		/// <summary>
		/// Waits if necessary for at most the given time for this future
		/// to complete, and then returns its result, if available.
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> the result value </returns>
		/// <exception cref="CancellationException"> if this future was cancelled </exception>
		/// <exception cref="ExecutionException"> if this future completed exceptionally </exception>
		/// <exception cref="InterruptedException"> if the current thread was interrupted
		/// while waiting </exception>
		/// <exception cref="TimeoutException"> if the wait timed out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T get(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.ExecutionException, java.util.concurrent.TimeoutException
		public virtual T Get(long timeout, TimeUnit unit)
		{
			Object r;
			long nanos = unit.ToNanos(timeout);
			return ReportGet((r = Result) == null ? TimedGet(nanos) : r);
		}

		/// <summary>
		/// Returns the result value when complete, or throws an
		/// (unchecked) exception if completed exceptionally. To better
		/// conform with the use of common functional forms, if a
		/// computation involved in the completion of this
		/// CompletableFuture threw an exception, this method throws an
		/// (unchecked) <seealso cref="CompletionException"/> with the underlying
		/// exception as its cause.
		/// </summary>
		/// <returns> the result value </returns>
		/// <exception cref="CancellationException"> if the computation was cancelled </exception>
		/// <exception cref="CompletionException"> if this future completed
		/// exceptionally or a completion computation threw an exception </exception>
		public virtual T Join()
		{
			Object r;
			return ReportJoin((r = Result) == null ? WaitingGet(false) : r);
		}

		/// <summary>
		/// Returns the result value (or throws any encountered exception)
		/// if completed, else returns the given valueIfAbsent.
		/// </summary>
		/// <param name="valueIfAbsent"> the value to return if not completed </param>
		/// <returns> the result value, if completed, else the given valueIfAbsent </returns>
		/// <exception cref="CancellationException"> if the computation was cancelled </exception>
		/// <exception cref="CompletionException"> if this future completed
		/// exceptionally or a completion computation threw an exception </exception>
		public virtual T GetNow(T valueIfAbsent)
		{
			Object r;
			return ((r = Result) == null) ? valueIfAbsent : ReportJoin(r);
		}

		/// <summary>
		/// If not already completed, sets the value returned by {@link
		/// #get()} and related methods to the given value.
		/// </summary>
		/// <param name="value"> the result value </param>
		/// <returns> {@code true} if this invocation caused this CompletableFuture
		/// to transition to a completed state, else {@code false} </returns>
		public virtual bool Complete(T value)
		{
			bool triggered = CompleteValue(value);
			PostComplete();
			return triggered;
		}

		/// <summary>
		/// If not already completed, causes invocations of <seealso cref="#get()"/>
		/// and related methods to throw the given exception.
		/// </summary>
		/// <param name="ex"> the exception </param>
		/// <returns> {@code true} if this invocation caused this CompletableFuture
		/// to transition to a completed state, else {@code false} </returns>
		public virtual bool CompleteExceptionally(Throwable ex)
		{
			if (ex == null)
			{
				throw new NullPointerException();
			}
			bool triggered = InternalComplete(new AltResult(ex));
			PostComplete();
			return triggered;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenApply(java.util.function.Function<? base T,? extends U> fn)
		public virtual CompletableFuture<U> thenApply<U, T1>(Function<T1> fn) where T1 : U
		{
			return UniApplyStage(null, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenApplyAsync(java.util.function.Function<? base T,? extends U> fn)
		public virtual CompletableFuture<U> thenApplyAsync<U, T1>(Function<T1> fn) where T1 : U
		{
			return UniApplyStage(AsyncPool, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenApplyAsync(java.util.function.Function<? base T,? extends U> fn, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<U> thenApplyAsync<U, T1>(Function<T1> fn, Executor executor) where T1 : U
		{
			return UniApplyStage(ScreenExecutor(executor), fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> thenAccept(java.util.function.Consumer<? base T> action)
		public virtual CompletableFuture<Void> ThenAccept(Consumer<T1> action)
		{
			return UniAcceptStage(null, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> thenAcceptAsync(java.util.function.Consumer<? base T> action)
		public virtual CompletableFuture<Void> ThenAcceptAsync(Consumer<T1> action)
		{
			return UniAcceptStage(AsyncPool, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> thenAcceptAsync(java.util.function.Consumer<? base T> action, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<Void> ThenAcceptAsync(Consumer<T1> action, Executor executor)
		{
			return UniAcceptStage(ScreenExecutor(executor), action);
		}

		public virtual CompletableFuture<Void> ThenRun(Runnable action)
		{
			return UniRunStage(null, action);
		}

		public virtual CompletableFuture<Void> ThenRunAsync(Runnable action)
		{
			return UniRunStage(AsyncPool, action);
		}

		public virtual CompletableFuture<Void> ThenRunAsync(Runnable action, Executor executor)
		{
			return UniRunStage(ScreenExecutor(executor), action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletableFuture<V> thenCombine(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn)
		public virtual CompletableFuture<V> thenCombine<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn) where T1 : U where T2 : V
		{
			return BiApplyStage(null, other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletableFuture<V> thenCombineAsync(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn)
		public virtual CompletableFuture<V> thenCombineAsync<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn) where T1 : U where T2 : V
		{
			return BiApplyStage(AsyncPool, other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletableFuture<V> thenCombineAsync(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<V> thenCombineAsync<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn, Executor executor) where T1 : U where T2 : V
		{
			return BiApplyStage(ScreenExecutor(executor), other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<Void> thenAcceptBoth(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action)
		public virtual CompletableFuture<Void> thenAcceptBoth<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action) where T1 : U
		{
			return BiAcceptStage(null, other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<Void> thenAcceptBothAsync(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action)
		public virtual CompletableFuture<Void> thenAcceptBothAsync<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action) where T1 : U
		{
			return BiAcceptStage(AsyncPool, other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<Void> thenAcceptBothAsync(java.util.concurrent.CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<Void> thenAcceptBothAsync<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action, Executor executor) where T1 : U
		{
			return BiAcceptStage(ScreenExecutor(executor), other, action);
		}

		public virtual CompletableFuture<Void> RunAfterBoth(CompletionStage<T1> other, Runnable action)
		{
			return BiRunStage(null, other, action);
		}

		public virtual CompletableFuture<Void> RunAfterBothAsync(CompletionStage<T1> other, Runnable action)
		{
			return BiRunStage(AsyncPool, other, action);
		}

		public virtual CompletableFuture<Void> RunAfterBothAsync(CompletionStage<T1> other, Runnable action, Executor executor)
		{
			return BiRunStage(ScreenExecutor(executor), other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> applyToEither(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn)
		public virtual CompletableFuture<U> applyToEither<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn) where T1 : T
		{
			return OrApplyStage(null, other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> applyToEitherAsync(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn)
		public virtual CompletableFuture<U> applyToEitherAsync<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn) where T1 : T
		{
			return OrApplyStage(AsyncPool, other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> applyToEitherAsync(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<U> applyToEitherAsync<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn, Executor executor) where T1 : T
		{
			return OrApplyStage(ScreenExecutor(executor), other, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> acceptEither(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action)
		public virtual CompletableFuture<Void> AcceptEither(CompletionStage<T1> other, Consumer<T2> action) where T1 : T
		{
			return OrAcceptStage(null, other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> acceptEitherAsync(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action)
		public virtual CompletableFuture<Void> AcceptEitherAsync(CompletionStage<T1> other, Consumer<T2> action) where T1 : T
		{
			return OrAcceptStage(AsyncPool, other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<Void> acceptEitherAsync(java.util.concurrent.CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<Void> AcceptEitherAsync(CompletionStage<T1> other, Consumer<T2> action, Executor executor) where T1 : T
		{
			return OrAcceptStage(ScreenExecutor(executor), other, action);
		}

		public virtual CompletableFuture<Void> RunAfterEither(CompletionStage<T1> other, Runnable action)
		{
			return OrRunStage(null, other, action);
		}

		public virtual CompletableFuture<Void> RunAfterEitherAsync(CompletionStage<T1> other, Runnable action)
		{
			return OrRunStage(AsyncPool, other, action);
		}

		public virtual CompletableFuture<Void> RunAfterEitherAsync(CompletionStage<T1> other, Runnable action, Executor executor)
		{
			return OrRunStage(ScreenExecutor(executor), other, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenCompose(java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<U>> fn)
		public virtual CompletableFuture<U> thenCompose<U, T1>(Function<T1> fn) where T1 : java.util.concurrent.CompletionStage<U>
		{
			return UniComposeStage(null, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenComposeAsync(java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<U>> fn)
		public virtual CompletableFuture<U> thenComposeAsync<U, T1>(Function<T1> fn) where T1 : java.util.concurrent.CompletionStage<U>
		{
			return UniComposeStage(AsyncPool, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> thenComposeAsync(java.util.function.Function<? base T, ? extends java.util.concurrent.CompletionStage<U>> fn, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<U> thenComposeAsync<U, T1>(Function<T1> fn, Executor executor) where T1 : java.util.concurrent.CompletionStage<U>
		{
			return UniComposeStage(ScreenExecutor(executor), fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<T> whenComplete(java.util.function.BiConsumer<? base T, ? base Throwable> action)
		public virtual CompletableFuture<T> WhenComplete(BiConsumer<T1> action)
		{
			return UniWhenCompleteStage(null, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<T> whenCompleteAsync(java.util.function.BiConsumer<? base T, ? base Throwable> action)
		public virtual CompletableFuture<T> WhenCompleteAsync(BiConsumer<T1> action)
		{
			return UniWhenCompleteStage(AsyncPool, action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletableFuture<T> whenCompleteAsync(java.util.function.BiConsumer<? base T, ? base Throwable> action, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<T> WhenCompleteAsync(BiConsumer<T1> action, Executor executor)
		{
			return UniWhenCompleteStage(ScreenExecutor(executor), action);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> handle(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn)
		public virtual CompletableFuture<U> handle<U, T1>(BiFunction<T1> fn) where T1 : U
		{
			return UniHandleStage(null, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> handleAsync(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn)
		public virtual CompletableFuture<U> handleAsync<U, T1>(BiFunction<T1> fn) where T1 : U
		{
			return UniHandleStage(AsyncPool, fn);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletableFuture<U> handleAsync(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn, java.util.concurrent.Executor executor)
		public virtual CompletableFuture<U> handleAsync<U, T1>(BiFunction<T1> fn, Executor executor) where T1 : U
		{
			return UniHandleStage(ScreenExecutor(executor), fn);
		}

		/// <summary>
		/// Returns this CompletableFuture.
		/// </summary>
		/// <returns> this CompletableFuture </returns>
		public virtual CompletableFuture<T> ToCompletableFuture()
		{
			return this;
		}

		// not in interface CompletionStage

		/// <summary>
		/// Returns a new CompletableFuture that is completed when this
		/// CompletableFuture completes, with the result of the given
		/// function of the exception triggering this CompletableFuture's
		/// completion when it completes exceptionally; otherwise, if this
		/// CompletableFuture completes normally, then the returned
		/// CompletableFuture also completes normally with the same value.
		/// Note: More flexible versions of this functionality are
		/// available using methods {@code whenComplete} and {@code handle}.
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of the
		/// returned CompletableFuture if this CompletableFuture completed
		/// exceptionally </param>
		/// <returns> the new CompletableFuture </returns>
		public virtual CompletableFuture<T> Exceptionally(Function<T1> fn) where T1 : T
		{
			return UniExceptionallyStage(fn);
		}

		/* ------------- Arbitrary-arity constructions -------------- */

		/// <summary>
		/// Returns a new CompletableFuture that is completed when all of
		/// the given CompletableFutures complete.  If any of the given
		/// CompletableFutures complete exceptionally, then the returned
		/// CompletableFuture also does so, with a CompletionException
		/// holding this exception as its cause.  Otherwise, the results,
		/// if any, of the given CompletableFutures are not reflected in
		/// the returned CompletableFuture, but may be obtained by
		/// inspecting them individually. If no CompletableFutures are
		/// provided, returns a CompletableFuture completed with the value
		/// {@code null}.
		/// 
		/// <para>Among the applications of this method is to await completion
		/// of a set of independent CompletableFutures before continuing a
		/// program, as in: {@code CompletableFuture.allOf(c1, c2,
		/// c3).join();}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cfs"> the CompletableFutures </param>
		/// <returns> a new CompletableFuture that is completed when all of the
		/// given CompletableFutures complete </returns>
		/// <exception cref="NullPointerException"> if the array or any of its elements are
		/// {@code null} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static CompletableFuture<Void> allOf(CompletableFuture<?>... cfs)
		public static CompletableFuture<Void> AllOf(params CompletableFuture<?>[] cfs)
		{
			return AndTree(cfs, 0, cfs.Length - 1);
		}

		/// <summary>
		/// Returns a new CompletableFuture that is completed when any of
		/// the given CompletableFutures complete, with the same result.
		/// Otherwise, if it completed exceptionally, the returned
		/// CompletableFuture also does so, with a CompletionException
		/// holding this exception as its cause.  If no CompletableFutures
		/// are provided, returns an incomplete CompletableFuture.
		/// </summary>
		/// <param name="cfs"> the CompletableFutures </param>
		/// <returns> a new CompletableFuture that is completed with the
		/// result or exception of any of the given CompletableFutures when
		/// one completes </returns>
		/// <exception cref="NullPointerException"> if the array or any of its elements are
		/// {@code null} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static CompletableFuture<Object> anyOf(CompletableFuture<?>... cfs)
		public static CompletableFuture<Object> AnyOf(params CompletableFuture<?>[] cfs)
		{
			return OrTree(cfs, 0, cfs.Length - 1);
		}

		/* ------------- Control and status methods -------------- */

		/// <summary>
		/// If not already completed, completes this CompletableFuture with
		/// a <seealso cref="CancellationException"/>. Dependent CompletableFutures
		/// that have not already completed will also complete
		/// exceptionally, with a <seealso cref="CompletionException"/> caused by
		/// this {@code CancellationException}.
		/// </summary>
		/// <param name="mayInterruptIfRunning"> this value has no effect in this
		/// implementation because interrupts are not used to control
		/// processing.
		/// </param>
		/// <returns> {@code true} if this task is now cancelled </returns>
		public virtual bool Cancel(bool mayInterruptIfRunning)
		{
			bool cancelled = (Result == null) && InternalComplete(new AltResult(new CancellationException()));
			PostComplete();
			return cancelled || Cancelled;
		}

		/// <summary>
		/// Returns {@code true} if this CompletableFuture was cancelled
		/// before it completed normally.
		/// </summary>
		/// <returns> {@code true} if this CompletableFuture was cancelled
		/// before it completed normally </returns>
		public virtual bool Cancelled
		{
			get
			{
				Object r;
				return ((r = Result) is AltResult) && (((AltResult)r).Ex is CancellationException);
			}
		}

		/// <summary>
		/// Returns {@code true} if this CompletableFuture completed
		/// exceptionally, in any way. Possible causes include
		/// cancellation, explicit invocation of {@code
		/// completeExceptionally}, and abrupt termination of a
		/// CompletionStage action.
		/// </summary>
		/// <returns> {@code true} if this CompletableFuture completed
		/// exceptionally </returns>
		public virtual bool CompletedExceptionally
		{
			get
			{
				Object r;
				return ((r = Result) is AltResult) && r != NIL;
			}
		}

		/// <summary>
		/// Forcibly sets or resets the value subsequently returned by
		/// method <seealso cref="#get()"/> and related methods, whether or not
		/// already completed. This method is designed for use only in
		/// error recovery actions, and even in such situations may result
		/// in ongoing dependent completions using established versus
		/// overwritten outcomes.
		/// </summary>
		/// <param name="value"> the completion value </param>
		public virtual void ObtrudeValue(T value)
		{
			Result = (value == null) ? NIL : value;
			PostComplete();
		}

		/// <summary>
		/// Forcibly causes subsequent invocations of method <seealso cref="#get()"/>
		/// and related methods to throw the given exception, whether or
		/// not already completed. This method is designed for use only in
		/// error recovery actions, and even in such situations may result
		/// in ongoing dependent completions using established versus
		/// overwritten outcomes.
		/// </summary>
		/// <param name="ex"> the exception </param>
		/// <exception cref="NullPointerException"> if the exception is null </exception>
		public virtual void ObtrudeException(Throwable ex)
		{
			if (ex == null)
			{
				throw new NullPointerException();
			}
			Result = new AltResult(ex);
			PostComplete();
		}

		/// <summary>
		/// Returns the estimated number of CompletableFutures whose
		/// completions are awaiting completion of this CompletableFuture.
		/// This method is designed for use in monitoring system state, not
		/// for synchronization control.
		/// </summary>
		/// <returns> the number of dependent CompletableFutures </returns>
		public virtual int NumberOfDependents
		{
			get
			{
				int count = 0;
				for (Completion p = Stack; p != null; p = p.Next)
				{
					++count;
				}
				return count;
			}
		}

		/// <summary>
		/// Returns a string identifying this CompletableFuture, as well as
		/// its completion state.  The state, in brackets, contains the
		/// String {@code "Completed Normally"} or the String {@code
		/// "Completed Exceptionally"}, or the String {@code "Not
		/// completed"} followed by the number of CompletableFutures
		/// dependent upon its completion, if any.
		/// </summary>
		/// <returns> a string identifying this CompletableFuture, as well as its state </returns>
		public override String ToString()
		{
			Object r = Result;
			int count;
			return base.ToString() + ((r == null) ? (((count = NumberOfDependents) == 0) ? "[Not completed]" : "[Not completed, " + count + " dependents]") : (((r is AltResult) && ((AltResult)r).Ex != null) ? "[Completed exceptionally]" : "[Completed normally]"));
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long RESULT;
		private static readonly long STACK;
		private static readonly long NEXT;
		static CompletableFuture()
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.misc.Unsafe u;
				sun.misc.Unsafe u;
				UNSAFE = u = sun.misc.Unsafe.Unsafe;
				Class k = typeof(CompletableFuture);
				RESULT = u.objectFieldOffset(k.GetDeclaredField("result"));
				STACK = u.objectFieldOffset(k.GetDeclaredField("stack"));
				NEXT = u.objectFieldOffset(typeof(Completion).getDeclaredField("next"));
			}
			catch (Exception x)
			{
				throw new Error(x);
			}
		}
	}

}