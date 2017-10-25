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
	/// A stage of a possibly asynchronous computation, that performs an
	/// action or computes a value when another CompletionStage completes.
	/// A stage completes upon termination of its computation, but this may
	/// in turn trigger other dependent stages.  The functionality defined
	/// in this interface takes only a few basic forms, which expand out to
	/// a larger set of methods to capture a range of usage styles: <ul>
	/// 
	/// <li>The computation performed by a stage may be expressed as a
	/// Function, Consumer, or Runnable (using methods with names including
	/// <em>apply</em>, <em>accept</em>, or <em>run</em>, respectively)
	/// depending on whether it requires arguments and/or produces results.
	/// For example, {@code stage.thenApply(x -> square(x)).thenAccept(x ->
	/// System.out.print(x)).thenRun(() -> System.out.println())}. An
	/// additional form (<em>compose</em>) applies functions of stages
	/// themselves, rather than their results. </li>
	/// 
	/// <li> One stage's execution may be triggered by completion of a
	/// single stage, or both of two stages, or either of two stages.
	/// Dependencies on a single stage are arranged using methods with
	/// prefix <em>then</em>. Those triggered by completion of
	/// <em>both</em> of two stages may <em>combine</em> their results or
	/// effects, using correspondingly named methods. Those triggered by
	/// <em>either</em> of two stages make no guarantees about which of the
	/// results or effects are used for the dependent stage's
	/// computation.</li>
	/// 
	/// <li> Dependencies among stages control the triggering of
	/// computations, but do not otherwise guarantee any particular
	/// ordering. Additionally, execution of a new stage's computations may
	/// be arranged in any of three ways: default execution, default
	/// asynchronous execution (using methods with suffix <em>async</em>
	/// that employ the stage's default asynchronous execution facility),
	/// or custom (via a supplied <seealso cref="Executor"/>).  The execution
	/// properties of default and async modes are specified by
	/// CompletionStage implementations, not this interface. Methods with
	/// explicit Executor arguments may have arbitrary execution
	/// properties, and might not even support concurrent execution, but
	/// are arranged for processing in a way that accommodates asynchrony.
	/// 
	/// <li> Two method forms support processing whether the triggering
	/// stage completed normally or exceptionally: Method {@link
	/// #whenComplete whenComplete} allows injection of an action
	/// regardless of outcome, otherwise preserving the outcome in its
	/// completion. Method <seealso cref="#handle handle"/> additionally allows the
	/// stage to compute a replacement result that may enable further
	/// processing by other dependent stages.  In all other cases, if a
	/// stage's computation terminates abruptly with an (unchecked)
	/// exception or error, then all dependent stages requiring its
	/// completion complete exceptionally as well, with a {@link
	/// CompletionException} holding the exception as its cause.  If a
	/// stage is dependent on <em>both</em> of two stages, and both
	/// complete exceptionally, then the CompletionException may correspond
	/// to either one of these exceptions.  If a stage is dependent on
	/// <em>either</em> of two others, and only one of them completes
	/// exceptionally, no guarantees are made about whether the dependent
	/// stage completes normally or exceptionally. In the case of method
	/// {@code whenComplete}, when the supplied action itself encounters an
	/// exception, then the stage exceptionally completes with this
	/// exception if not already completed exceptionally.</li>
	/// 
	/// </ul>
	/// 
	/// <para>All methods adhere to the above triggering, execution, and
	/// exceptional completion specifications (which are not repeated in
	/// individual method specifications). Additionally, while arguments
	/// used to pass a completion result (that is, for parameters of type
	/// {@code T}) for methods accepting them may be null, passing a null
	/// value for any other parameter will result in a {@link
	/// NullPointerException} being thrown.
	/// 
	/// </para>
	/// <para>This interface does not define methods for initially creating,
	/// forcibly completing normally or exceptionally, probing completion
	/// status or results, or awaiting completion of a stage.
	/// Implementations of CompletionStage may provide means of achieving
	/// such effects, as appropriate.  Method <seealso cref="#toCompletableFuture"/>
	/// enables interoperability among different implementations of this
	/// interface by providing a common conversion type.
	/// 
	/// @author Doug Lea
	/// @since 1.8
	/// </para>
	/// </summary>
	public interface CompletionStage<T>
	{

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed with this stage's result as the argument
		/// to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenApply(java.util.function.Function<? base T,? extends U> fn);
		CompletionStage<U> thenApply<U, T1>(Function<T1> fn) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using this stage's default asynchronous
		/// execution facility, with this stage's result as the argument to
		/// the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenApplyAsync(java.util.function.Function<? base T,? extends U> fn);
		CompletionStage<U> thenApplyAsync<U, T1>(Function<T1> fn) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using the supplied Executor, with this
		/// stage's result as the argument to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenApplyAsync(java.util.function.Function<? base T,? extends U> fn, java.util.concurrent.Executor executor);
		CompletionStage<U> thenApplyAsync<U, T1>(Function<T1> fn, Executor executor) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed with this stage's result as the argument
		/// to the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> thenAccept(java.util.function.Consumer<? base T> action);
		CompletionStage<Void> ThenAccept(Consumer<T1> action);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using this stage's default asynchronous
		/// execution facility, with this stage's result as the argument to
		/// the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> thenAcceptAsync(java.util.function.Consumer<? base T> action);
		CompletionStage<Void> ThenAcceptAsync(Consumer<T1> action);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using the supplied Executor, with this
		/// stage's result as the argument to the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> thenAcceptAsync(java.util.function.Consumer<? base T> action, java.util.concurrent.Executor executor);
		CompletionStage<Void> ThenAcceptAsync(Consumer<T1> action, Executor executor);
		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, executes the given action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> ThenRun(Runnable action);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, executes the given action using this stage's default
		/// asynchronous execution facility.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> ThenRunAsync(Runnable action);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, executes the given action using the supplied Executor.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> ThenRunAsync(Runnable action, Executor executor);

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage both complete normally, is executed with the two
		/// results as arguments to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// @param <V> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletionStage<V> thenCombine(CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn);
		CompletionStage<V> thenCombine<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn) where T1 : U where T2 : V;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, is executed using this stage's
		/// default asynchronous execution facility, with the two results
		/// as arguments to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// @param <V> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletionStage<V> thenCombineAsync(CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn);
		CompletionStage<V> thenCombineAsync<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn) where T1 : U where T2 : V;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, is executed using the supplied
		/// executor, with the two results as arguments to the supplied
		/// function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// @param <V> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U,V> CompletionStage<V> thenCombineAsync(CompletionStage<? extends U> other, java.util.function.BiFunction<? base T,? base U,? extends V> fn, java.util.concurrent.Executor executor);
		CompletionStage<V> thenCombineAsync<U, V, T1, T2>(CompletionStage<T1> other, BiFunction<T2> fn, Executor executor) where T1 : U where T2 : V;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage both complete normally, is executed with the two
		/// results as arguments to the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<Void> thenAcceptBoth(CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action);
		CompletionStage<Void> thenAcceptBoth<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, is executed using this stage's
		/// default asynchronous execution facility, with the two results
		/// as arguments to the supplied action.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<Void> thenAcceptBothAsync(CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action);
		CompletionStage<Void> thenAcceptBothAsync<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, is executed using the supplied
		/// executor, with the two results as arguments to the supplied
		/// function.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the type of the other CompletionStage's result </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<Void> thenAcceptBothAsync(CompletionStage<? extends U> other, java.util.function.BiConsumer<? base T, ? base U> action, java.util.concurrent.Executor executor);
		CompletionStage<Void> thenAcceptBothAsync<U, T1, T2>(CompletionStage<T1> other, BiConsumer<T2> action, Executor executor) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage both complete normally, executes the given action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterBoth(CompletionStage<T1> other, Runnable action);
		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, executes the given action using
		/// this stage's default asynchronous execution facility.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterBothAsync(CompletionStage<T1> other, Runnable action);

		/// <summary>
		/// Returns a new CompletionStage that, when this and the other
		/// given stage complete normally, executes the given action using
		/// the supplied executor.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterBothAsync(CompletionStage<T1> other, Runnable action, Executor executor);
		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed with the
		/// corresponding result as argument to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> applyToEither(CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn);
		CompletionStage<U> applyToEither<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed using this
		/// stage's default asynchronous execution facility, with the
		/// corresponding result as argument to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> applyToEitherAsync(CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn);
		CompletionStage<U> applyToEitherAsync<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed using the
		/// supplied executor, with the corresponding result as argument to
		/// the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="fn"> the function to use to compute the value of
		/// the returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> applyToEitherAsync(CompletionStage<? extends T> other, java.util.function.Function<? base T, U> fn, java.util.concurrent.Executor executor);
		CompletionStage<U> applyToEitherAsync<U, T1, T2>(CompletionStage<T1> other, Function<T2> fn, Executor executor) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed with the
		/// corresponding result as argument to the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> acceptEither(CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action);
		CompletionStage<Void> AcceptEither(CompletionStage<T1> other, Consumer<T2> action) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed using this
		/// stage's default asynchronous execution facility, with the
		/// corresponding result as argument to the supplied action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> acceptEitherAsync(CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action);
		CompletionStage<Void> AcceptEitherAsync(CompletionStage<T1> other, Consumer<T2> action) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, is executed using the
		/// supplied executor, with the corresponding result as argument to
		/// the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<Void> acceptEitherAsync(CompletionStage<? extends T> other, java.util.function.Consumer<? base T> action, java.util.concurrent.Executor executor);
		CompletionStage<Void> AcceptEitherAsync(CompletionStage<T1> other, Consumer<T2> action, Executor executor) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, executes the given action.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterEither(CompletionStage<T1> other, Runnable action);

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, executes the given action
		/// using this stage's default asynchronous execution facility.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterEitherAsync(CompletionStage<T1> other, Runnable action);

		/// <summary>
		/// Returns a new CompletionStage that, when either this or the
		/// other given stage complete normally, executes the given action
		/// using the supplied executor.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="other"> the other CompletionStage </param>
		/// <param name="action"> the action to perform before completing the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<Void> RunAfterEitherAsync(CompletionStage<T1> other, Runnable action, Executor executor);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed with this stage as the argument
		/// to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function returning a new CompletionStage </param>
		/// @param <U> the type of the returned CompletionStage's result </param>
		/// <returns> the CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenCompose(java.util.function.Function<? base T, ? extends CompletionStage<U>> fn);
		CompletionStage<U> thenCompose<U, T1>(Function<T1> fn) where T1 : CompletionStage<U>;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using this stage's default asynchronous
		/// execution facility, with this stage as the argument to the
		/// supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function returning a new CompletionStage </param>
		/// @param <U> the type of the returned CompletionStage's result </param>
		/// <returns> the CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenComposeAsync(java.util.function.Function<? base T, ? extends CompletionStage<U>> fn);
		CompletionStage<U> thenComposeAsync<U, T1>(Function<T1> fn) where T1 : CompletionStage<U>;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// normally, is executed using the supplied Executor, with this
		/// stage's result as the argument to the supplied function.
		/// 
		/// See the <seealso cref="CompletionStage"/> documentation for rules
		/// covering exceptional completion.
		/// </summary>
		/// <param name="fn"> the function returning a new CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the type of the returned CompletionStage's result </param>
		/// <returns> the CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> thenComposeAsync(java.util.function.Function<? base T, ? extends CompletionStage<U>> fn, java.util.concurrent.Executor executor);
		CompletionStage<U> thenComposeAsync<U, T1>(Function<T1> fn, Executor executor) where T1 : CompletionStage<U>;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// exceptionally, is executed with this stage's exception as the
		/// argument to the supplied function.  Otherwise, if this stage
		/// completes normally, then the returned stage also completes
		/// normally with the same value.
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of the
		/// returned CompletionStage if this CompletionStage completed
		/// exceptionally </param>
		/// <returns> the new CompletionStage </returns>
		CompletionStage<T> Exceptionally(Function<T1> fn) where T1 : T;

		/// <summary>
		/// Returns a new CompletionStage with the same result or exception as
		/// this stage, that executes the given action when this stage completes.
		/// 
		/// <para>When this stage is complete, the given action is invoked with the
		/// result (or {@code null} if none) and the exception (or {@code null}
		/// if none) of this stage as arguments.  The returned stage is completed
		/// when the action returns.  If the supplied action itself encounters an
		/// exception, then the returned stage exceptionally completes with this
		/// exception unless this stage also completed exceptionally.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> the action to perform </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<T> whenComplete(java.util.function.BiConsumer<? base T, ? base Throwable> action);
		CompletionStage<T> WhenComplete(BiConsumer<T1> action);

		/// <summary>
		/// Returns a new CompletionStage with the same result or exception as
		/// this stage, that executes the given action using this stage's
		/// default asynchronous execution facility when this stage completes.
		/// 
		/// <para>When this stage is complete, the given action is invoked with the
		/// result (or {@code null} if none) and the exception (or {@code null}
		/// if none) of this stage as arguments.  The returned stage is completed
		/// when the action returns.  If the supplied action itself encounters an
		/// exception, then the returned stage exceptionally completes with this
		/// exception unless this stage also completed exceptionally.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> the action to perform </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<T> whenCompleteAsync(java.util.function.BiConsumer<? base T, ? base Throwable> action);
		CompletionStage<T> WhenCompleteAsync(BiConsumer<T1> action);

		/// <summary>
		/// Returns a new CompletionStage with the same result or exception as
		/// this stage, that executes the given action using the supplied
		/// Executor when this stage completes.
		/// 
		/// <para>When this stage is complete, the given action is invoked with the
		/// result (or {@code null} if none) and the exception (or {@code null}
		/// if none) of this stage as arguments.  The returned stage is completed
		/// when the action returns.  If the supplied action itself encounters an
		/// exception, then the returned stage exceptionally completes with this
		/// exception unless this stage also completed exceptionally.
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> the action to perform </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public CompletionStage<T> whenCompleteAsync(java.util.function.BiConsumer<? base T, ? base Throwable> action, java.util.concurrent.Executor executor);
		CompletionStage<T> WhenCompleteAsync(BiConsumer<T1> action, Executor executor);

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// either normally or exceptionally, is executed with this stage's
		/// result and exception as arguments to the supplied function.
		/// 
		/// <para>When this stage is complete, the given function is invoked
		/// with the result (or {@code null} if none) and the exception (or
		/// {@code null} if none) of this stage as arguments, and the
		/// function's result is used to complete the returned stage.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of the
		/// returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> handle(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn);
		CompletionStage<U> handle<U, T1>(BiFunction<T1> fn) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// either normally or exceptionally, is executed using this stage's
		/// default asynchronous execution facility, with this stage's
		/// result and exception as arguments to the supplied function.
		/// 
		/// <para>When this stage is complete, the given function is invoked
		/// with the result (or {@code null} if none) and the exception (or
		/// {@code null} if none) of this stage as arguments, and the
		/// function's result is used to complete the returned stage.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of the
		/// returned CompletionStage </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> handleAsync(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn);
		CompletionStage<U> handleAsync<U, T1>(BiFunction<T1> fn) where T1 : U;

		/// <summary>
		/// Returns a new CompletionStage that, when this stage completes
		/// either normally or exceptionally, is executed using the
		/// supplied executor, with this stage's result and exception as
		/// arguments to the supplied function.
		/// 
		/// <para>When this stage is complete, the given function is invoked
		/// with the result (or {@code null} if none) and the exception (or
		/// {@code null} if none) of this stage as arguments, and the
		/// function's result is used to complete the returned stage.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fn"> the function to use to compute the value of the
		/// returned CompletionStage </param>
		/// <param name="executor"> the executor to use for asynchronous execution </param>
		/// @param <U> the function's return type </param>
		/// <returns> the new CompletionStage </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> CompletionStage<U> handleAsync(java.util.function.BiFunction<? base T, Throwable, ? extends U> fn, java.util.concurrent.Executor executor);
		CompletionStage<U> handleAsync<U, T1>(BiFunction<T1> fn, Executor executor) where T1 : U;

		/// <summary>
		/// Returns a <seealso cref="CompletableFuture"/> maintaining the same
		/// completion properties as this stage. If this stage is already a
		/// CompletableFuture, this method may return this stage itself.
		/// Otherwise, invocation of this method may be equivalent in
		/// effect to {@code thenApply(x -> x)}, but returning an instance
		/// of type {@code CompletableFuture}. A CompletionStage
		/// implementation that does not choose to interoperate with others
		/// may throw {@code UnsupportedOperationException}.
		/// </summary>
		/// <returns> the CompletableFuture </returns>
		/// <exception cref="UnsupportedOperationException"> if this implementation
		/// does not interoperate with CompletableFuture </exception>
		CompletableFuture<T> ToCompletableFuture();

	}

}