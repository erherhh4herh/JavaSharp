/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{


	/// <summary>
	/// A grouping of asynchronous channels for the purpose of resource sharing.
	/// 
	/// <para> An asynchronous channel group encapsulates the mechanics required to
	/// handle the completion of I/O operations initiated by {@link AsynchronousChannel
	/// asynchronous channels} that are bound to the group. A group has an associated
	/// thread pool to which tasks are submitted to handle I/O events and dispatch to
	/// <seealso cref="CompletionHandler completion-handlers"/> that consume the result of
	/// asynchronous operations performed on channels in the group. In addition to
	/// handling I/O events, the pooled threads may also execute other tasks required
	/// to support the execution of asynchronous I/O operations.
	/// 
	/// </para>
	/// <para> An asynchronous channel group is created by invoking the {@link
	/// #withFixedThreadPool withFixedThreadPool} or {@link #withCachedThreadPool
	/// withCachedThreadPool} methods defined here. Channels are bound to a group by
	/// specifying the group when constructing the channel. The associated thread
	/// pool is <em>owned</em> by the group; termination of the group results in the
	/// shutdown of the associated thread pool.
	/// 
	/// </para>
	/// <para> In addition to groups created explicitly, the Java virtual machine
	/// maintains a system-wide <em>default group</em> that is constructed
	/// automatically. Asynchronous channels that do not specify a group at
	/// construction time are bound to the default group. The default group has an
	/// associated thread pool that creates new threads as needed. The default group
	/// may be configured by means of system properties defined in the table below.
	/// Where the <seealso cref="java.util.concurrent.ThreadFactory ThreadFactory"/> for the
	/// default group is not configured then the pooled threads of the default group
	/// are <seealso cref="Thread#isDaemon daemon"/> threads.
	/// 
	/// <table border summary="System properties">
	///   <tr>
	///     <th>System property</th>
	///     <th>Description</th>
	///   </tr>
	///   <tr>
	///     <td> {@code java.nio.channels.DefaultThreadPool.threadFactory} </td>
	///     <td> The value of this property is taken to be the fully-qualified name
	///     of a concrete <seealso cref="java.util.concurrent.ThreadFactory ThreadFactory"/>
	///     class. The class is loaded using the system class loader and instantiated.
	///     The factory's {@link java.util.concurrent.ThreadFactory#newThread
	///     newThread} method is invoked to create each thread for the default
	///     group's thread pool. If the process to load and instantiate the value
	///     of the property fails then an unspecified error is thrown during the
	///     construction of the default group. </td>
	///   </tr>
	///   <tr>
	///     <td> {@code java.nio.channels.DefaultThreadPool.initialSize} </td>
	///     <td> The value of the {@code initialSize} parameter for the default
	///     group (see <seealso cref="#withCachedThreadPool withCachedThreadPool"/>).
	///     The value of the property is taken to be the {@code String}
	///     representation of an {@code Integer} that is the initial size parameter.
	///     If the value cannot be parsed as an {@code Integer} it causes an
	///     unspecified error to be thrown during the construction of the default
	///     group. </td>
	///   </tr>
	/// </table>
	/// 
	/// <a name="threading"></a><h2>Threading</h2>
	/// 
	/// </para>
	/// <para> The completion handler for an I/O operation initiated on a channel bound
	/// to a group is guaranteed to be invoked by one of the pooled threads in the
	/// group. This ensures that the completion handler is run by a thread with the
	/// expected <em>identity</em>.
	/// 
	/// </para>
	/// <para> Where an I/O operation completes immediately, and the initiating thread
	/// is one of the pooled threads in the group then the completion handler may
	/// be invoked directly by the initiating thread. To avoid stack overflow, an
	/// implementation may impose a limit as to the number of activations on the
	/// thread stack. Some I/O operations may prohibit invoking the completion
	/// handler directly by the initiating thread (see {@link
	/// AsynchronousServerSocketChannel#accept(Object,CompletionHandler) accept}).
	/// 
	/// <a name="shutdown"></a><h2>Shutdown and Termination</h2>
	/// 
	/// </para>
	/// <para> The <seealso cref="#shutdown() shutdown"/> method is used to initiate an <em>orderly
	/// shutdown</em> of a group. An orderly shutdown marks the group as shutdown;
	/// further attempts to construct a channel that binds to the group will throw
	/// <seealso cref="ShutdownChannelGroupException"/>. Whether or not a group is shutdown can
	/// be tested using the <seealso cref="#isShutdown() isShutdown"/> method. Once shutdown,
	/// the group <em>terminates</em> when all asynchronous channels that are bound to
	/// the group are closed, all actively executing completion handlers have run to
	/// completion, and resources used by the group are released. No attempt is made
	/// to stop or interrupt threads that are executing completion handlers. The
	/// <seealso cref="#isTerminated() isTerminated"/> method is used to test if the group has
	/// terminated, and the <seealso cref="#awaitTermination awaitTermination"/> method can be
	/// used to block until the group has terminated.
	/// 
	/// </para>
	/// <para> The <seealso cref="#shutdownNow() shutdownNow"/> method can be used to initiate a
	/// <em>forceful shutdown</em> of the group. In addition to the actions performed
	/// by an orderly shutdown, the {@code shutdownNow} method closes all open channels
	/// in the group as if by invoking the <seealso cref="AsynchronousChannel#close close"/>
	/// method.
	/// 
	/// @since 1.7
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= AsynchronousSocketChannel#open(AsynchronousChannelGroup) </seealso>
	/// <seealso cref= AsynchronousServerSocketChannel#open(AsynchronousChannelGroup) </seealso>

	public abstract class AsynchronousChannelGroup
	{
		private readonly AsynchronousChannelProvider Provider_Renamed;

		/// <summary>
		/// Initialize a new instance of this class.
		/// </summary>
		/// <param name="provider">
		///          The asynchronous channel provider for this group </param>
		protected internal AsynchronousChannelGroup(AsynchronousChannelProvider provider)
		{
			this.Provider_Renamed = provider;
		}

		/// <summary>
		/// Returns the provider that created this channel group.
		/// </summary>
		/// <returns>  The provider that created this channel group </returns>
		public AsynchronousChannelProvider Provider()
		{
			return Provider_Renamed;
		}

		/// <summary>
		/// Creates an asynchronous channel group with a fixed thread pool.
		/// 
		/// <para> The resulting asynchronous channel group reuses a fixed number of
		/// threads. At any point, at most {@code nThreads} threads will be active
		/// processing tasks that are submitted to handle I/O events and dispatch
		/// completion results for operations initiated on asynchronous channels in
		/// the group.
		/// 
		/// </para>
		/// <para> The group is created by invoking the {@link
		/// AsynchronousChannelProvider#openAsynchronousChannelGroup(int,ThreadFactory)
		/// openAsynchronousChannelGroup(int,ThreadFactory)} method of the system-wide
		/// default <seealso cref="AsynchronousChannelProvider"/> object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nThreads">
		///          The number of threads in the pool </param>
		/// <param name="threadFactory">
		///          The factory to use when creating new threads
		/// </param>
		/// <returns>  A new asynchronous channel group
		/// </returns>
		/// <exception cref="IllegalArgumentException">
		///          If {@code nThreads <= 0} </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AsynchronousChannelGroup withFixedThreadPool(int nThreads, java.util.concurrent.ThreadFactory threadFactory) throws java.io.IOException
		public static AsynchronousChannelGroup WithFixedThreadPool(int nThreads, ThreadFactory threadFactory)
		{
			return AsynchronousChannelProvider.Provider().OpenAsynchronousChannelGroup(nThreads, threadFactory);
		}

		/// <summary>
		/// Creates an asynchronous channel group with a given thread pool that
		/// creates new threads as needed.
		/// 
		/// <para> The {@code executor} parameter is an {@code ExecutorService} that
		/// creates new threads as needed to execute tasks that are submitted to
		/// handle I/O events and dispatch completion results for operations initiated
		/// on asynchronous channels in the group. It may reuse previously constructed
		/// threads when they are available.
		/// 
		/// </para>
		/// <para> The {@code initialSize} parameter may be used by the implementation
		/// as a <em>hint</em> as to the initial number of tasks it may submit. For
		/// example, it may be used to indicate the initial number of threads that
		/// wait on I/O events.
		/// 
		/// </para>
		/// <para> The executor is intended to be used exclusively by the resulting
		/// asynchronous channel group. Termination of the group results in the
		/// orderly  <seealso cref="ExecutorService#shutdown shutdown"/> of the executor
		/// service. Shutting down the executor service by other means results in
		/// unspecified behavior.
		/// 
		/// </para>
		/// <para> The group is created by invoking the {@link
		/// AsynchronousChannelProvider#openAsynchronousChannelGroup(ExecutorService,int)
		/// openAsynchronousChannelGroup(ExecutorService,int)} method of the system-wide
		/// default <seealso cref="AsynchronousChannelProvider"/> object.
		/// 
		/// </para>
		/// </summary>
		/// <param name="executor">
		///          The thread pool for the resulting group </param>
		/// <param name="initialSize">
		///          A value {@code >=0} or a negative value for implementation
		///          specific default
		/// </param>
		/// <returns>  A new asynchronous channel group
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs
		/// </exception>
		/// <seealso cref= java.util.concurrent.Executors#newCachedThreadPool </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AsynchronousChannelGroup withCachedThreadPool(java.util.concurrent.ExecutorService executor, int initialSize) throws java.io.IOException
		public static AsynchronousChannelGroup WithCachedThreadPool(ExecutorService executor, int initialSize)
		{
			return AsynchronousChannelProvider.Provider().OpenAsynchronousChannelGroup(executor, initialSize);
		}

		/// <summary>
		/// Creates an asynchronous channel group with a given thread pool.
		/// 
		/// <para> The {@code executor} parameter is an {@code ExecutorService} that
		/// executes tasks submitted to dispatch completion results for operations
		/// initiated on asynchronous channels in the group.
		/// 
		/// </para>
		/// <para> Care should be taken when configuring the executor service. It
		/// should support <em>direct handoff</em> or <em>unbounded queuing</em> of
		/// submitted tasks, and the thread that invokes the {@link
		/// ExecutorService#execute execute} method should never invoke the task
		/// directly. An implementation may mandate additional constraints.
		/// 
		/// </para>
		/// <para> The executor is intended to be used exclusively by the resulting
		/// asynchronous channel group. Termination of the group results in the
		/// orderly  <seealso cref="ExecutorService#shutdown shutdown"/> of the executor
		/// service. Shutting down the executor service by other means results in
		/// unspecified behavior.
		/// 
		/// </para>
		/// <para> The group is created by invoking the {@link
		/// AsynchronousChannelProvider#openAsynchronousChannelGroup(ExecutorService,int)
		/// openAsynchronousChannelGroup(ExecutorService,int)} method of the system-wide
		/// default <seealso cref="AsynchronousChannelProvider"/> object with an {@code
		/// initialSize} of {@code 0}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="executor">
		///          The thread pool for the resulting group
		/// </param>
		/// <returns>  A new asynchronous channel group
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AsynchronousChannelGroup withThreadPool(java.util.concurrent.ExecutorService executor) throws java.io.IOException
		public static AsynchronousChannelGroup WithThreadPool(ExecutorService executor)
		{
			return AsynchronousChannelProvider.Provider().OpenAsynchronousChannelGroup(executor, 0);
		}

		/// <summary>
		/// Tells whether or not this asynchronous channel group is shutdown.
		/// </summary>
		/// <returns>  {@code true} if this asynchronous channel group is shutdown or
		///          has been marked for shutdown. </returns>
		public abstract bool Shutdown {get;}

		/// <summary>
		/// Tells whether or not this group has terminated.
		/// 
		/// <para> Where this method returns {@code true}, then the associated thread
		/// pool has also <seealso cref="ExecutorService#isTerminated terminated"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  {@code true} if this group has terminated </returns>
		public abstract bool Terminated {get;}

		/// <summary>
		/// Initiates an orderly shutdown of the group.
		/// 
		/// <para> This method marks the group as shutdown. Further attempts to construct
		/// channel that binds to this group will throw <seealso cref="ShutdownChannelGroupException"/>.
		/// The group terminates when all asynchronous channels in the group are
		/// closed, all actively executing completion handlers have run to completion,
		/// and all resources have been released. This method has no effect if the
		/// group is already shutdown.
		/// </para>
		/// </summary>
		public abstract void Shutdown();

		/// <summary>
		/// Shuts down the group and closes all open channels in the group.
		/// 
		/// <para> In addition to the actions performed by the <seealso cref="#shutdown() shutdown"/>
		/// method, this method invokes the <seealso cref="AsynchronousChannel#close close"/>
		/// method on all open channels in the group. This method does not attempt to
		/// stop or interrupt threads that are executing completion handlers. The
		/// group terminates when all actively executing completion handlers have run
		/// to completion and all resources have been released. This method may be
		/// invoked at any time. If some other thread has already invoked it, then
		/// another invocation will block until the first invocation is complete,
		/// after which it will return without effect.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void shutdownNow() throws java.io.IOException;
		public abstract void ShutdownNow();

		/// <summary>
		/// Awaits termination of the group.
		/// 
		/// <para> This method blocks until the group has terminated, or the timeout
		/// occurs, or the current thread is interrupted, whichever happens first.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout">
		///          The maximum time to wait, or zero or less to not wait </param>
		/// <param name="unit">
		///          The time unit of the timeout argument
		/// </param>
		/// <returns>  {@code true} if the group has terminated; {@code false} if the
		///          timeout elapsed before termination
		/// </returns>
		/// <exception cref="InterruptedException">
		///          If interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean awaitTermination(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException;
		public abstract bool AwaitTermination(long timeout, TimeUnit unit);
	}

}